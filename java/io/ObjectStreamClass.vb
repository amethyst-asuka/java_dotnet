Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

Namespace java.io


	''' <summary>
	''' Serialization's descriptor for classes.  It contains the name and
	''' serialVersionUID of the class.  The ObjectStreamClass for a specific class
	''' loaded in this Java VM can be found/created using the lookup method.
	''' 
	''' <p>The algorithm to compute the SerialVersionUID is described in
	''' <a href="../../../platform/serialization/spec/class.html#4100">Object
	''' Serialization Specification, Section 4.6, Stream Unique Identifiers</a>.
	''' 
	''' @author      Mike Warres
	''' @author      Roger Riggs </summary>
	''' <seealso cref= ObjectStreamField </seealso>
	''' <seealso cref= <a href="../../../platform/serialization/spec/class.html">Object Serialization Specification, Section 4, Class Descriptors</a>
	''' @since   JDK1.1 </seealso>
	<Serializable> _
	Public Class ObjectStreamClass

		''' <summary>
		''' serialPersistentFields value indicating no serializable fields </summary>
		Public Shared ReadOnly NO_FIELDS As ObjectStreamField() = New ObjectStreamField(){}

		Private Const serialVersionUID As Long = -6120832682080437368L
		Private Shared ReadOnly serialPersistentFields As ObjectStreamField() = NO_FIELDS

		''' <summary>
		''' reflection factory for obtaining serialization constructors </summary>
		Private Shared ReadOnly reflFactory As sun.reflect.ReflectionFactory = java.security.AccessController.doPrivileged(New sun.reflect.ReflectionFactory.GetReflectionFactoryAction)

		Private Class Caches
			''' <summary>
			''' cache mapping local classes -> descriptors </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Shared ReadOnly localDescs As java.util.concurrent.ConcurrentMap(Of WeakClassKey, Reference(Of ?)) = New ConcurrentDictionary(Of WeakClassKey, Reference(Of ?))

			''' <summary>
			''' cache mapping field group/local desc pairs -> field reflectors </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Shared ReadOnly reflectors As java.util.concurrent.ConcurrentMap(Of FieldReflectorKey, Reference(Of ?)) = New ConcurrentDictionary(Of FieldReflectorKey, Reference(Of ?))

			''' <summary>
			''' queue for WeakReferences to local classes </summary>
			Private Shared ReadOnly localDescsQueue As New ReferenceQueue(Of [Class])
			''' <summary>
			''' queue for WeakReferences to field reflectors keys </summary>
			Private Shared ReadOnly reflectorsQueue As New ReferenceQueue(Of [Class])
		End Class

		''' <summary>
		''' class associated with this descriptor (if any) </summary>
		Private cl As  [Class]
		''' <summary>
		''' name of class represented by this descriptor </summary>
		Private name As String
		''' <summary>
		''' serialVersionUID of represented class (null if not computed yet) </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private suid As Long?

		''' <summary>
		''' true if represents dynamic proxy class </summary>
		Private isProxy_Renamed As Boolean
		''' <summary>
		''' true if represents enum type </summary>
		Private isEnum_Renamed As Boolean
		''' <summary>
		''' true if represented class implements Serializable </summary>
		Private serializable As Boolean
		''' <summary>
		''' true if represented class implements Externalizable </summary>
		Private externalizable As Boolean
		''' <summary>
		''' true if desc has data written by class-defined writeObject method </summary>
		Private hasWriteObjectData_Renamed As Boolean
		''' <summary>
		''' true if desc has externalizable data written in block data format; this
		''' must be true by default to accommodate ObjectInputStream subclasses which
		''' override readClassDescriptor() to return class descriptors obtained from
		''' ObjectStreamClass.lookup() (see 4461737)
		''' </summary>
		Private hasBlockExternalData_Renamed As Boolean = True

		''' <summary>
		''' Contains information about InvalidClassException instances to be thrown
		''' when attempting operations on an invalid class. Note that instances of
		''' this class are immutable and are potentially shared among
		''' ObjectStreamClass instances.
		''' </summary>
		Private Class ExceptionInfo
			Private ReadOnly className As String
			Private ReadOnly message As String

			Friend Sub New(ByVal cn As String, ByVal msg As String)
				className = cn
				message = msg
			End Sub

			''' <summary>
			''' Returns (does not throw) an InvalidClassException instance created
			''' from the information in this object, suitable for being thrown by
			''' the caller.
			''' </summary>
			Friend Overridable Function newInvalidClassException() As InvalidClassException
				Return New InvalidClassException(className, message)
			End Function
		End Class

		''' <summary>
		''' exception (if any) thrown while attempting to resolve class </summary>
		Private resolveEx As  [Class]NotFoundException
		''' <summary>
		''' exception (if any) to throw if non-enum deserialization attempted </summary>
		Private deserializeEx As ExceptionInfo
		''' <summary>
		''' exception (if any) to throw if non-enum serialization attempted </summary>
		Private serializeEx As ExceptionInfo
		''' <summary>
		''' exception (if any) to throw if default serialization attempted </summary>
		Private defaultSerializeEx As ExceptionInfo

		''' <summary>
		''' serializable fields </summary>
		Private fields As ObjectStreamField()
		''' <summary>
		''' aggregate marshalled size of primitive fields </summary>
		Private primDataSize As Integer
		''' <summary>
		''' number of non-primitive fields </summary>
		Private numObjFields As Integer
		''' <summary>
		''' reflector for setting/getting serializable field values </summary>
		Private fieldRefl As FieldReflector
		''' <summary>
		''' data layout of serialized objects described by this class desc </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private dataLayout As  [Class]DataSlot()

		''' <summary>
		''' serialization-appropriate constructor, or null if none </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private cons As Constructor(Of ?)
		''' <summary>
		''' class-defined writeObject method, or null if none </summary>
		Private writeObjectMethod As Method
		''' <summary>
		''' class-defined readObject method, or null if none </summary>
		Private readObjectMethod As Method
		''' <summary>
		''' class-defined readObjectNoData method, or null if none </summary>
		Private readObjectNoDataMethod As Method
		''' <summary>
		''' class-defined writeReplace method, or null if none </summary>
		Private writeReplaceMethod As Method
		''' <summary>
		''' class-defined readResolve method, or null if none </summary>
		Private readResolveMethod As Method

		''' <summary>
		''' local class descriptor for represented class (may point to self) </summary>
		Private localDesc As ObjectStreamClass
		''' <summary>
		''' superclass descriptor appearing in stream </summary>
		Private superDesc As ObjectStreamClass

		''' <summary>
		''' true if, and only if, the object has been correctly initialized </summary>
		Private initialized As Boolean

		''' <summary>
		''' Initializes native code.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initNative()
		End Sub
		Shared Sub New()
			initNative()
		End Sub

		''' <summary>
		''' Find the descriptor for a class that can be serialized.  Creates an
		''' ObjectStreamClass instance if one does not exist yet for class. Null is
		''' returned if the specified class does not implement java.io.Serializable
		''' or java.io.Externalizable.
		''' </summary>
		''' <param name="cl"> class for which to get the descriptor </param>
		''' <returns>  the class descriptor for the specified class </returns>
		Public Shared Function lookup(ByVal cl As [Class]) As ObjectStreamClass
			Return lookup(cl, False)
		End Function

		''' <summary>
		''' Returns the descriptor for any [Class], regardless of whether it
		''' implements <seealso cref="Serializable"/>.
		''' </summary>
		''' <param name="cl"> class for which to get the descriptor </param>
		''' <returns>       the class descriptor for the specified class
		''' @since 1.6 </returns>
		Public Shared Function lookupAny(ByVal cl As [Class]) As ObjectStreamClass
			Return lookup(cl, True)
		End Function

		''' <summary>
		''' Returns the name of the class described by this descriptor.
		''' This method returns the name of the class in the format that
		''' is used by the <seealso cref="Class#getName"/> method.
		''' </summary>
		''' <returns> a string representing the name of the class </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Return the serialVersionUID for this class.  The serialVersionUID
		''' defines a set of classes all with the same name that have evolved from a
		''' common root class and agree to be serialized and deserialized using a
		''' common format.  NonSerializable classes have a serialVersionUID of 0L.
		''' </summary>
		''' <returns>  the SUID of the class described by this descriptor </returns>
		Public Overridable Property serialVersionUID As Long
			Get
				' REMIND: synchronize instead of relying on volatile?
				If suid Is Nothing Then
					suid = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				   )
				End If
				Return suid
			End Get
		End Property

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Long?
				Return computeDefaultSUID(outerInstance.cl)
			End Function
		End Class

		''' <summary>
		''' Return the class in the local VM that this version is mapped to.  Null
		''' is returned if there is no corresponding local class.
		''' </summary>
		''' <returns>  the <code>Class</code> instance that this descriptor represents </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function forClass() As  [Class]
			If cl Is Nothing Then Return Nothing
			requireInitialized()
			If System.securityManager IsNot Nothing Then
				Dim caller As  [Class] = sun.reflect.Reflection.callerClass
				If sun.reflect.misc.ReflectUtil.needsPackageAccessCheck(caller.classLoader, cl.classLoader) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(cl)
			End If
			Return cl
		End Function

		''' <summary>
		''' Return an array of the fields of this serializable class.
		''' </summary>
		''' <returns>  an array containing an element for each persistent field of
		'''          this class. Returns an array of length zero if there are no
		'''          fields.
		''' @since 1.2 </returns>
		Public Overridable Property fields As ObjectStreamField()
			Get
				Return getFields(True)
			End Get
		End Property

		''' <summary>
		''' Get the field of this class by name.
		''' </summary>
		''' <param name="name"> the name of the data field to look for </param>
		''' <returns>  The ObjectStreamField object of the named field or null if
		'''          there is no such named field. </returns>
		Public Overridable Function getField(ByVal name As String) As ObjectStreamField
			Return getField(name, Nothing)
		End Function

		''' <summary>
		''' Return a string describing this ObjectStreamClass.
		''' </summary>
		Public Overrides Function ToString() As String
			Return name & ": static final long serialVersionUID = " & serialVersionUID & "L;"
		End Function

		''' <summary>
		''' Looks up and returns class descriptor for given [Class], or null if class
		''' is non-serializable and "all" is set to false.
		''' </summary>
		''' <param name="cl"> class to look up </param>
		''' <param name="all"> if true, return descriptors for all classes; if false, only
		'''          return descriptors for serializable classes </param>
		Shared Function lookup(ByVal cl As [Class], ByVal all As Boolean) As ObjectStreamClass
			If Not(all OrElse cl.IsSubclassOf(GetType(Serializable))) Then Return Nothing
			processQueue(Caches.localDescsQueue, Caches.localDescs)
			Dim key As New WeakClassKey(cl, Caches.localDescsQueue)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ref As Reference(Of ?) = Caches.localDescs.get(key)
			Dim entry As Object = Nothing
			If ref IsNot Nothing Then entry = ref.get()
			Dim future As EntryFuture = Nothing
			If entry Is Nothing Then
				Dim newEntry As New EntryFuture
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim newRef As Reference(Of ?) = New SoftReference(Of ?)(newEntry)
				Do
					If ref IsNot Nothing Then Caches.localDescs.remove(key, ref)
					ref = Caches.localDescs.putIfAbsent(key, newRef)
					If ref IsNot Nothing Then entry = ref.get()
				Loop While ref IsNot Nothing AndAlso entry Is Nothing
				If entry Is Nothing Then future = newEntry
			End If

			If TypeOf entry Is ObjectStreamClass Then ' check common case first Return CType(entry, ObjectStreamClass)
			If TypeOf entry Is EntryFuture Then
				future = CType(entry, EntryFuture)
				If future.owner Is Thread.CurrentThread Then
	'                
	'                 * Handle nested call situation described by 4803747: waiting
	'                 * for future value to be set by a lookup() call further up the
	'                 * stack will result in deadlock, so calculate and set the
	'                 * future value here instead.
	'                 
					entry = Nothing
				Else
					entry = future.get()
				End If
			End If
			If entry Is Nothing Then
				Try
					entry = New ObjectStreamClass(cl)
				Catch th As Throwable
					entry = th
				End Try
				If future.set(entry) Then
					Caches.localDescs.put(key, New SoftReference(Of Object)(entry))
				Else
					' nested lookup call already set future
					entry = future.get()
				End If
			End If

			If TypeOf entry Is ObjectStreamClass Then
				Return CType(entry, ObjectStreamClass)
			ElseIf TypeOf entry Is RuntimeException Then
				Throw CType(entry, RuntimeException)
			ElseIf TypeOf entry Is Error Then
				Throw CType(entry, [Error])
			Else
				Throw New InternalError("unexpected entry: " & entry)
			End If
		End Function

		''' <summary>
		''' Placeholder used in class descriptor and field reflector lookup tables
		''' for an entry in the process of being initialized.  (Internal) callers
		''' which receive an EntryFuture belonging to another thread as the result
		''' of a lookup should call the get() method of the EntryFuture; this will
		''' return the actual entry once it is ready for use and has been set().  To
		''' conserve objects, EntryFutures synchronize on themselves.
		''' </summary>
		Private Class EntryFuture

			Private Shared ReadOnly unset As New Object
			Private ReadOnly owner As Thread = Thread.currentThread()
			Private entry As Object = unset

			''' <summary>
			''' Attempts to set the value contained by this EntryFuture.  If the
			''' EntryFuture's value has not been set already, then the value is
			''' saved, any callers blocked in the get() method are notified, and
			''' true is returned.  If the value has already been set, then no saving
			''' or notification occurs, and false is returned.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Function [set](ByVal entry As Object) As Boolean
				If Me.entry IsNot unset Then Return False
				Me.entry = entry
				notifyAll()
				Return True
			End Function

			''' <summary>
			''' Returns the value contained by this EntryFuture, blocking if
			''' necessary until a value is set.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Friend Overridable Function [get]() As Object
				Dim interrupted As Boolean = False
				Do While entry Is unset
					Try
						wait()
					Catch ex As InterruptedException
						interrupted = True
					End Try
				Loop
				If interrupted Then
					java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
				   )
				End If
				Return entry
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As Void
					Thread.CurrentThread.Interrupt()
					Return Nothing
				End Function
			End Class

			''' <summary>
			''' Returns the thread that created this EntryFuture.
			''' </summary>
			Friend Overridable Property owner As Thread
				Get
					Return owner
				End Get
			End Property
		End Class

		''' <summary>
		''' Creates local class descriptor representing given class.
		''' </summary>
		Private Sub New(ByVal cl As [Class])
			Me.cl = cl
			name = cl.name
			isProxy_Renamed = Proxy.isProxyClass(cl)
			isEnum_Renamed = cl.IsSubclassOf(GetType(System.Enum))
			serializable = cl.IsSubclassOf(GetType(Serializable))
			externalizable = cl.IsSubclassOf(GetType(Externalizable))

			Dim superCl As  [Class] = cl.BaseType
			superDesc = If(superCl IsNot Nothing, lookup(superCl, False), Nothing)
			localDesc = Me

			If serializable Then
				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Else
				suid = Convert.ToInt64(0)
				fields = NO_FIELDS
			End If

			Try
				fieldRefl = getReflector(fields, Me)
			Catch ex As InvalidClassException
				' field mismatches impossible when matching local fields vs. self
				Throw New InternalError(ex)
			End Try

			If deserializeEx Is Nothing Then
				If isEnum_Renamed Then
					deserializeEx = New ExceptionInfo(name, "enum type")
				ElseIf cons Is Nothing Then
					deserializeEx = New ExceptionInfo(name, "no valid constructor")
				End If
			End If
			For i As Integer = 0 To fields.Length - 1
				If fields(i).field Is Nothing Then defaultSerializeEx = New ExceptionInfo(name, "unmatched serializable field(s) declared")
			Next i
			initialized = True
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				If outerInstance.isEnum_Renamed Then
					outerInstance.suid = Convert.ToInt64(0)
					outerInstance.fields = NO_FIELDS
					Return Nothing
				End If
				If outerInstance.cl.array Then
					outerInstance.fields = NO_FIELDS
					Return Nothing
				End If

				outerInstance.suid = getDeclaredSUID(outerInstance.cl)
				Try
					outerInstance.fields = getSerialFields(outerInstance.cl)
					outerInstance.computeFieldOffsets()
				Catch e As InvalidClassException
						outerInstance.deserializeEx = New ExceptionInfo(e.classname, e.Message)
						outerInstance.serializeEx = outerInstance.deserializeEx
					outerInstance.fields = NO_FIELDS
				End Try

				If outerInstance.externalizable Then
					outerInstance.cons = getExternalizableConstructor(outerInstance.cl)
				Else
					outerInstance.cons = getSerializableConstructor(outerInstance.cl)
					outerInstance.writeObjectMethod = getPrivateMethod(outerInstance.cl, "writeObject", New [Class]() { GetType(ObjectOutputStream) }, Void.TYPE)
					outerInstance.readObjectMethod = getPrivateMethod(outerInstance.cl, "readObject", New [Class]() { GetType(ObjectInputStream) }, Void.TYPE)
					outerInstance.readObjectNoDataMethod = getPrivateMethod(outerInstance.cl, "readObjectNoData", Nothing, Void.TYPE)
					outerInstance.hasWriteObjectData_Renamed = (outerInstance.writeObjectMethod IsNot Nothing)
				End If
				outerInstance.writeReplaceMethod = getInheritableMethod(outerInstance.cl, "writeReplace", Nothing, GetType(Object))
				outerInstance.readResolveMethod = getInheritableMethod(outerInstance.cl, "readResolve", Nothing, GetType(Object))
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Creates blank class descriptor which should be initialized via a
		''' subsequent call to initProxy(), initNonProxy() or readNonProxy().
		''' </summary>
		Friend Sub New()
		End Sub

		''' <summary>
		''' Initializes class descriptor representing a proxy class.
		''' </summary>
		Friend Overridable Sub initProxy(ByVal cl As [Class], ByVal resolveEx As  [Class]NotFoundException, ByVal superDesc As ObjectStreamClass)
			Dim osc As ObjectStreamClass = Nothing
			If cl IsNot Nothing Then
				osc = lookup(cl, True)
				If Not osc.isProxy_Renamed Then Throw New InvalidClassException("cannot bind proxy descriptor to a non-proxy class")
			End If
			Me.cl = cl
			Me.resolveEx = resolveEx
			Me.superDesc = superDesc
			isProxy_Renamed = True
			serializable = True
			suid = Convert.ToInt64(0)
			fields = NO_FIELDS
			If osc IsNot Nothing Then
				localDesc = osc
				name = localDesc.name
				externalizable = localDesc.externalizable
				writeReplaceMethod = localDesc.writeReplaceMethod
				readResolveMethod = localDesc.readResolveMethod
				deserializeEx = localDesc.deserializeEx
				cons = localDesc.cons
			End If
			fieldRefl = getReflector(fields, localDesc)
			initialized = True
		End Sub

		''' <summary>
		''' Initializes class descriptor representing a non-proxy class.
		''' </summary>
		Friend Overridable Sub initNonProxy(ByVal model As ObjectStreamClass, ByVal cl As [Class], ByVal resolveEx As  [Class]NotFoundException, ByVal superDesc As ObjectStreamClass)
			Dim suid As Long = Convert.ToInt64(model.serialVersionUID)
			Dim osc As ObjectStreamClass = Nothing
			If cl IsNot Nothing Then
				osc = lookup(cl, True)
				If osc.isProxy_Renamed Then Throw New InvalidClassException("cannot bind non-proxy descriptor to a proxy class")
				If model.isEnum_Renamed <> osc.isEnum_Renamed Then Throw New InvalidClassException(If(model.isEnum_Renamed, "cannot bind enum descriptor to a non-enum class", "cannot bind non-enum descriptor to an enum class"))

				If model.serializable = osc.serializable AndAlso (Not cl.array) AndAlso suid <> osc.serialVersionUID Then Throw New InvalidClassException(osc.name, "local class incompatible: " & "stream classdesc serialVersionUID = " & suid & ", local class serialVersionUID = " & osc.serialVersionUID)

				If Not classNamesEqual(model.name, osc.name) Then Throw New InvalidClassException(osc.name, "local class name incompatible with stream class " & "name """ & model.name & """")

				If Not model.isEnum_Renamed Then
					If (model.serializable = osc.serializable) AndAlso (model.externalizable <> osc.externalizable) Then Throw New InvalidClassException(osc.name, "Serializable incompatible with Externalizable")

					If (model.serializable <> osc.serializable) OrElse (model.externalizable <> osc.externalizable) OrElse Not(model.serializable OrElse model.externalizable) Then deserializeEx = New ExceptionInfo(osc.name, "class invalid for deserialization")
				End If
			End If

			Me.cl = cl
			Me.resolveEx = resolveEx
			Me.superDesc = superDesc
			name = model.name
			Me.suid = suid
			isProxy_Renamed = False
			isEnum_Renamed = model.isEnum_Renamed
			serializable = model.serializable
			externalizable = model.externalizable
			hasBlockExternalData_Renamed = model.hasBlockExternalData_Renamed
			hasWriteObjectData_Renamed = model.hasWriteObjectData_Renamed
			fields = model.fields
			primDataSize = model.primDataSize
			numObjFields = model.numObjFields

			If osc IsNot Nothing Then
				localDesc = osc
				writeObjectMethod = localDesc.writeObjectMethod
				readObjectMethod = localDesc.readObjectMethod
				readObjectNoDataMethod = localDesc.readObjectNoDataMethod
				writeReplaceMethod = localDesc.writeReplaceMethod
				readResolveMethod = localDesc.readResolveMethod
				If deserializeEx Is Nothing Then deserializeEx = localDesc.deserializeEx
				cons = localDesc.cons
			End If

			fieldRefl = getReflector(fields, localDesc)
			' reassign to matched fields so as to reflect local unshared settings
			fields = fieldRefl.fields
			initialized = True
		End Sub

		''' <summary>
		''' Reads non-proxy class descriptor information from given input stream.
		''' The resulting class descriptor is not fully functional; it can only be
		''' used as input to the ObjectInputStream.resolveClass() and
		''' ObjectStreamClass.initNonProxy() methods.
		''' </summary>
		Friend Overridable Sub readNonProxy(ByVal [in] As ObjectInputStream)
			name = [in].readUTF()
			suid = Convert.ToInt64([in].readLong())
			isProxy_Renamed = False

			Dim flags As SByte = [in].readByte()
			hasWriteObjectData_Renamed = ((flags And ObjectStreamConstants.SC_WRITE_METHOD) <> 0)
			hasBlockExternalData_Renamed = ((flags And ObjectStreamConstants.SC_BLOCK_DATA) <> 0)
			externalizable = ((flags And ObjectStreamConstants.SC_EXTERNALIZABLE) <> 0)
			Dim sflag As Boolean = ((flags And ObjectStreamConstants.SC_SERIALIZABLE) <> 0)
			If externalizable AndAlso sflag Then Throw New InvalidClassException(name, "serializable and externalizable flags conflict")
			serializable = externalizable OrElse sflag
			isEnum_Renamed = ((flags And ObjectStreamConstants.SC_ENUM) <> 0)
			If isEnum_Renamed AndAlso suid <> 0L Then Throw New InvalidClassException(name, "enum descriptor has non-zero serialVersionUID: " & suid)

			Dim numFields As Integer = [in].readShort()
			If isEnum_Renamed AndAlso numFields <> 0 Then Throw New InvalidClassException(name, "enum descriptor has non-zero field count: " & numFields)
			fields = If(numFields > 0, New ObjectStreamField(numFields - 1){}, NO_FIELDS)
			For i As Integer = 0 To numFields - 1
				Dim tcode As Char = ChrW([in].readByte())
				Dim fname As String = [in].readUTF()
				Dim signature As String = If((tcode = "L"c) OrElse (tcode = "["c), [in].readTypeString(), New String(New Char() { tcode }))
				Try
					fields(i) = New ObjectStreamField(fname, signature, False)
				Catch e As RuntimeException
					Throw CType((New InvalidClassException(name, "invalid descriptor for field " & fname)).initCause(e), IOException)
				End Try
			Next i
			computeFieldOffsets()
		End Sub

		''' <summary>
		''' Writes non-proxy class descriptor information to given output stream.
		''' </summary>
		Friend Overridable Sub writeNonProxy(ByVal out As ObjectOutputStream)
			out.writeUTF(name)
			out.writeLong(serialVersionUID)

			Dim flags As SByte = 0
			If externalizable Then
				flags = flags Or ObjectStreamConstants.SC_EXTERNALIZABLE
				Dim protocol As Integer = out.protocolVersion
				If protocol <> ObjectStreamConstants.PROTOCOL_VERSION_1 Then flags = flags Or ObjectStreamConstants.SC_BLOCK_DATA
			ElseIf serializable Then
				flags = flags Or ObjectStreamConstants.SC_SERIALIZABLE
			End If
			If hasWriteObjectData_Renamed Then flags = flags Or ObjectStreamConstants.SC_WRITE_METHOD
			If isEnum_Renamed Then flags = flags Or ObjectStreamConstants.SC_ENUM
			out.writeByte(flags)

			out.writeShort(fields.Length)
			For i As Integer = 0 To fields.Length - 1
				Dim f As ObjectStreamField = fields(i)
				out.writeByte(f.typeCode)
				out.writeUTF(f.name)
				If Not f.primitive Then out.writeTypeString(f.typeString)
			Next i
		End Sub

		''' <summary>
		''' Returns ClassNotFoundException (if any) thrown while attempting to
		''' resolve local class corresponding to this class descriptor.
		''' </summary>
		Friend Overridable Property resolveException As  [Class]NotFoundException
			Get
				Return resolveEx
			End Get
		End Property

		''' <summary>
		''' Throws InternalError if not initialized.
		''' </summary>
		Private Sub requireInitialized()
			If Not initialized Then Throw New InternalError("Unexpected call when not initialized")
		End Sub

		''' <summary>
		''' Throws an InvalidClassException if object instances referencing this
		''' class descriptor should not be allowed to deserialize.  This method does
		''' not apply to deserialization of enum constants.
		''' </summary>
		Friend Overridable Sub checkDeserialize()
			requireInitialized()
			If deserializeEx IsNot Nothing Then Throw deserializeEx.newInvalidClassException()
		End Sub

		''' <summary>
		''' Throws an InvalidClassException if objects whose class is represented by
		''' this descriptor should not be allowed to serialize.  This method does
		''' not apply to serialization of enum constants.
		''' </summary>
		Friend Overridable Sub checkSerialize()
			requireInitialized()
			If serializeEx IsNot Nothing Then Throw serializeEx.newInvalidClassException()
		End Sub

		''' <summary>
		''' Throws an InvalidClassException if objects whose class is represented by
		''' this descriptor should not be permitted to use default serialization
		''' (e.g., if the class declares serializable fields that do not correspond
		''' to actual fields, and hence must use the GetField API).  This method
		''' does not apply to deserialization of enum constants.
		''' </summary>
		Friend Overridable Sub checkDefaultSerialize()
			requireInitialized()
			If defaultSerializeEx IsNot Nothing Then Throw defaultSerializeEx.newInvalidClassException()
		End Sub

		''' <summary>
		''' Returns superclass descriptor.  Note that on the receiving side, the
		''' superclass descriptor may be bound to a class that is not a superclass
		''' of the subclass descriptor's bound class.
		''' </summary>
		Friend Overridable Property superDesc As ObjectStreamClass
			Get
				requireInitialized()
				Return superDesc
			End Get
		End Property

		''' <summary>
		''' Returns the "local" class descriptor for the class associated with this
		''' class descriptor (i.e., the result of
		''' ObjectStreamClass.lookup(this.forClass())) or null if there is no class
		''' associated with this descriptor.
		''' </summary>
		Friend Overridable Property localDesc As ObjectStreamClass
			Get
				requireInitialized()
				Return localDesc
			End Get
		End Property

		''' <summary>
		''' Returns arrays of ObjectStreamFields representing the serializable
		''' fields of the represented class.  If copy is true, a clone of this class
		''' descriptor's field array is returned, otherwise the array itself is
		''' returned.
		''' </summary>
		Friend Overridable Function getFields(ByVal copy As Boolean) As ObjectStreamField()
			Return If(copy, fields.clone(), fields)
		End Function

		''' <summary>
		''' Looks up a serializable field of the represented class by name and type.
		''' A specified type of null matches all types, Object.class matches all
		''' non-primitive types, and any other non-null type matches assignable
		''' types only.  Returns matching field, or null if no match found.
		''' </summary>
		Friend Overridable Function getField(ByVal name As String, ByVal type As [Class]) As ObjectStreamField
			For i As Integer = 0 To fields.Length - 1
				Dim f As ObjectStreamField = fields(i)
				If f.name.Equals(name) Then
					If type Is Nothing OrElse (type Is GetType(Object) AndAlso (Not f.primitive)) Then Return f
					Dim ftype As  [Class] = f.type
					If ftype IsNot Nothing AndAlso ftype.IsSubclassOf(type) Then Return f
				End If
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Returns true if class descriptor represents a dynamic proxy [Class], false
		''' otherwise.
		''' </summary>
		Friend Overridable Property proxy As Boolean
			Get
				requireInitialized()
				Return isProxy_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns true if class descriptor represents an enum type, false
		''' otherwise.
		''' </summary>
		Friend Overridable Property [enum] As Boolean
			Get
				requireInitialized()
				Return isEnum_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns true if represented class implements Externalizable, false
		''' otherwise.
		''' </summary>
		Friend Overridable Property externalizable As Boolean
			Get
				requireInitialized()
				Return externalizable
			End Get
		End Property

		''' <summary>
		''' Returns true if represented class implements Serializable, false
		''' otherwise.
		''' </summary>
		Friend Overridable Property serializable As Boolean
			Get
				requireInitialized()
				Return serializable
			End Get
		End Property

		''' <summary>
		''' Returns true if class descriptor represents externalizable class that
		''' has written its data in 1.2 (block data) format, false otherwise.
		''' </summary>
		Friend Overridable Function hasBlockExternalData() As Boolean
			requireInitialized()
			Return hasBlockExternalData_Renamed
		End Function

		''' <summary>
		''' Returns true if class descriptor represents serializable (but not
		''' externalizable) class which has written its data via a custom
		''' writeObject() method, false otherwise.
		''' </summary>
		Friend Overridable Function hasWriteObjectData() As Boolean
			requireInitialized()
			Return hasWriteObjectData_Renamed
		End Function

		''' <summary>
		''' Returns true if represented class is serializable/externalizable and can
		''' be instantiated by the serialization runtime--i.e., if it is
		''' externalizable and defines a public no-arg constructor, or if it is
		''' non-externalizable and its first non-serializable superclass defines an
		''' accessible no-arg constructor.  Otherwise, returns false.
		''' </summary>
		Friend Overridable Property instantiable As Boolean
			Get
				requireInitialized()
				Return (cons IsNot Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns true if represented class is serializable (but not
		''' externalizable) and defines a conformant writeObject method.  Otherwise,
		''' returns false.
		''' </summary>
		Friend Overridable Function hasWriteObjectMethod() As Boolean
			requireInitialized()
			Return (writeObjectMethod IsNot Nothing)
		End Function

		''' <summary>
		''' Returns true if represented class is serializable (but not
		''' externalizable) and defines a conformant readObject method.  Otherwise,
		''' returns false.
		''' </summary>
		Friend Overridable Function hasReadObjectMethod() As Boolean
			requireInitialized()
			Return (readObjectMethod IsNot Nothing)
		End Function

		''' <summary>
		''' Returns true if represented class is serializable (but not
		''' externalizable) and defines a conformant readObjectNoData method.
		''' Otherwise, returns false.
		''' </summary>
		Friend Overridable Function hasReadObjectNoDataMethod() As Boolean
			requireInitialized()
			Return (readObjectNoDataMethod IsNot Nothing)
		End Function

		''' <summary>
		''' Returns true if represented class is serializable or externalizable and
		''' defines a conformant writeReplace method.  Otherwise, returns false.
		''' </summary>
		Friend Overridable Function hasWriteReplaceMethod() As Boolean
			requireInitialized()
			Return (writeReplaceMethod IsNot Nothing)
		End Function

		''' <summary>
		''' Returns true if represented class is serializable or externalizable and
		''' defines a conformant readResolve method.  Otherwise, returns false.
		''' </summary>
		Friend Overridable Function hasReadResolveMethod() As Boolean
			requireInitialized()
			Return (readResolveMethod IsNot Nothing)
		End Function

		''' <summary>
		''' Creates a new instance of the represented class.  If the class is
		''' externalizable, invokes its public no-arg constructor; otherwise, if the
		''' class is serializable, invokes the no-arg constructor of the first
		''' non-serializable superclass.  Throws UnsupportedOperationException if
		''' this class descriptor is not associated with a [Class], if the associated
		''' class is non-serializable or if the appropriate no-arg constructor is
		''' inaccessible/unavailable.
		''' </summary>
		Friend Overridable Function newInstance() As Object
			requireInitialized()
			If cons IsNot Nothing Then
				Try
					Return cons.newInstance()
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Function

		''' <summary>
		''' Invokes the writeObject method of the represented serializable class.
		''' Throws UnsupportedOperationException if this class descriptor is not
		''' associated with a [Class], or if the class is externalizable,
		''' non-serializable or does not define writeObject.
		''' </summary>
		Friend Overridable Sub invokeWriteObject(ByVal obj As Object, ByVal out As ObjectOutputStream)
			requireInitialized()
			If writeObjectMethod IsNot Nothing Then
				Try
					writeObjectMethod.invoke(obj, New Object(){ out })
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					If TypeOf th Is IOException Then
						Throw CType(th, IOException)
					Else
						throwMiscException(th)
					End If
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Sub

		''' <summary>
		''' Invokes the readObject method of the represented serializable class.
		''' Throws UnsupportedOperationException if this class descriptor is not
		''' associated with a [Class], or if the class is externalizable,
		''' non-serializable or does not define readObject.
		''' </summary>
		Friend Overridable Sub invokeReadObject(ByVal obj As Object, ByVal [in] As ObjectInputStream)
			requireInitialized()
			If readObjectMethod IsNot Nothing Then
				Try
					readObjectMethod.invoke(obj, New Object(){ [in] })
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					If TypeOf th Is ClassNotFoundException Then
						Throw CType(th, ClassNotFoundException)
					ElseIf TypeOf th Is IOException Then
						Throw CType(th, IOException)
					Else
						throwMiscException(th)
					End If
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Sub

		''' <summary>
		''' Invokes the readObjectNoData method of the represented serializable
		''' class.  Throws UnsupportedOperationException if this class descriptor is
		''' not associated with a [Class], or if the class is externalizable,
		''' non-serializable or does not define readObjectNoData.
		''' </summary>
		Friend Overridable Sub invokeReadObjectNoData(ByVal obj As Object)
			requireInitialized()
			If readObjectNoDataMethod IsNot Nothing Then
				Try
					readObjectNoDataMethod.invoke(obj, CType(Nothing, Object()))
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					If TypeOf th Is ObjectStreamException Then
						Throw CType(th, ObjectStreamException)
					Else
						throwMiscException(th)
					End If
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Sub

		''' <summary>
		''' Invokes the writeReplace method of the represented serializable class and
		''' returns the result.  Throws UnsupportedOperationException if this class
		''' descriptor is not associated with a [Class], or if the class is
		''' non-serializable or does not define writeReplace.
		''' </summary>
		Friend Overridable Function invokeWriteReplace(ByVal obj As Object) As Object
			requireInitialized()
			If writeReplaceMethod IsNot Nothing Then
				Try
					Return writeReplaceMethod.invoke(obj, CType(Nothing, Object()))
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					If TypeOf th Is ObjectStreamException Then
						Throw CType(th, ObjectStreamException)
					Else
						throwMiscException(th)
						Throw New InternalError(th) ' never reached
					End If
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Function

		''' <summary>
		''' Invokes the readResolve method of the represented serializable class and
		''' returns the result.  Throws UnsupportedOperationException if this class
		''' descriptor is not associated with a [Class], or if the class is
		''' non-serializable or does not define readResolve.
		''' </summary>
		Friend Overridable Function invokeReadResolve(ByVal obj As Object) As Object
			requireInitialized()
			If readResolveMethod IsNot Nothing Then
				Try
					Return readResolveMethod.invoke(obj, CType(Nothing, Object()))
				Catch ex As InvocationTargetException
					Dim th As Throwable = ex.targetException
					If TypeOf th Is ObjectStreamException Then
						Throw CType(th, ObjectStreamException)
					Else
						throwMiscException(th)
						Throw New InternalError(th) ' never reached
					End If
				Catch ex As IllegalAccessException
					' should not occur, as access checks have been suppressed
					Throw New InternalError(ex)
				End Try
			Else
				Throw New UnsupportedOperationException
			End If
		End Function

		''' <summary>
		''' Class representing the portion of an object's serialized form allotted
		''' to data described by a given class descriptor.  If "hasData" is false,
		''' the object's serialized form does not contain data associated with the
		''' class descriptor.
		''' </summary>
		Friend Class ClassDataSlot

			''' <summary>
			''' class descriptor "occupying" this slot </summary>
			Friend ReadOnly desc As ObjectStreamClass
			''' <summary>
			''' true if serialized form includes data for this slot's descriptor </summary>
			Friend ReadOnly hasData As Boolean

			Friend Sub New(ByVal desc As ObjectStreamClass, ByVal hasData As Boolean)
				Me.desc = desc
				Me.hasData = hasData
			End Sub
		End Class

		''' <summary>
		''' Returns array of ClassDataSlot instances representing the data layout
		''' (including superclass data) for serialized objects described by this
		''' class descriptor.  ClassDataSlots are ordered by inheritance with those
		''' containing "higher" superclasses appearing first.  The final
		''' ClassDataSlot contains a reference to this descriptor.
		''' </summary>
		Friend Overridable Property classDataLayout As  [Class]DataSlot()
			Get
				' REMIND: synchronize instead of relying on volatile?
				If dataLayout Is Nothing Then dataLayout = classDataLayout0
				Return dataLayout
			End Get
		End Property

		Private Property classDataLayout0 As  [Class]DataSlot()
			Get
				Dim slots As New List(Of ClassDataSlot)
				Dim start As  [Class] = cl, [end] As  [Class] = cl
    
				' locate closest non-serializable superclass
				Do While [end] IsNot Nothing AndAlso [end].IsSubclassOf(GetType(Serializable))
					[end] = [end].BaseType
				Loop
    
				Dim oscNames As New HashSet(Of String)(3)
    
				Dim d As ObjectStreamClass = Me
				Do While d IsNot Nothing
					If oscNames.Contains(d.name) Then
						Throw New InvalidClassException("Circular reference.")
					Else
						oscNames.Add(d.name)
					End If
    
					' search up inheritance hierarchy for class with matching name
					Dim searchName As String = If(d.cl IsNot Nothing, d.cl.name, d.name)
					Dim match As  [Class] = Nothing
					Dim c As  [Class] = start
					Do While c IsNot [end]
						If searchName.Equals(c.name) Then
							match = c
							Exit Do
						End If
						c = c.BaseType
					Loop
    
					' add "no data" slot for each unmatched class below match
					If match IsNot Nothing Then
						c = start
						Do While c IsNot match
							slots.Add(New ClassDataSlot(ObjectStreamClass.lookup(c, True), False))
							c = c.BaseType
						Loop
						start = match.BaseType
					End If
    
					' record descriptor/class pairing
					slots.Add(New ClassDataSlot(d.getVariantFor(match), True))
					d = d.superDesc
				Loop
    
				' add "no data" slot for any leftover unmatched classes
				Dim c As  [Class] = start
				Do While c IsNot [end]
					slots.Add(New ClassDataSlot(ObjectStreamClass.lookup(c, True), False))
					c = c.BaseType
				Loop
    
				' order slots from superclass -> subclass
				slots.Reverse()
				Return slots.ToArray()
			End Get
		End Property

		''' <summary>
		''' Returns aggregate size (in bytes) of marshalled primitive field values
		''' for represented class.
		''' </summary>
		Friend Overridable Property primDataSize As Integer
			Get
				Return primDataSize
			End Get
		End Property

		''' <summary>
		''' Returns number of non-primitive serializable fields of represented
		''' class.
		''' </summary>
		Friend Overridable Property numObjFields As Integer
			Get
				Return numObjFields
			End Get
		End Property

		''' <summary>
		''' Fetches the serializable primitive field values of object obj and
		''' marshals them into byte array buf starting at offset 0.  It is the
		''' responsibility of the caller to ensure that obj is of the proper type if
		''' non-null.
		''' </summary>
		Friend Overridable Sub getPrimFieldValues(ByVal obj As Object, ByVal buf As SByte())
			fieldRefl.getPrimFieldValues(obj, buf)
		End Sub

		''' <summary>
		''' Sets the serializable primitive fields of object obj using values
		''' unmarshalled from byte array buf starting at offset 0.  It is the
		''' responsibility of the caller to ensure that obj is of the proper type if
		''' non-null.
		''' </summary>
		Friend Overridable Sub setPrimFieldValues(ByVal obj As Object, ByVal buf As SByte())
			fieldRefl.primFieldValuesues(obj, buf)
		End Sub

		''' <summary>
		''' Fetches the serializable object field values of object obj and stores
		''' them in array vals starting at offset 0.  It is the responsibility of
		''' the caller to ensure that obj is of the proper type if non-null.
		''' </summary>
		Friend Overridable Sub getObjFieldValues(ByVal obj As Object, ByVal vals As Object())
			fieldRefl.getObjFieldValues(obj, vals)
		End Sub

		''' <summary>
		''' Sets the serializable object fields of object obj using values from
		''' array vals starting at offset 0.  It is the responsibility of the caller
		''' to ensure that obj is of the proper type if non-null.
		''' </summary>
		Friend Overridable Sub setObjFieldValues(ByVal obj As Object, ByVal vals As Object())
			fieldRefl.objFieldValuesues(obj, vals)
		End Sub

		''' <summary>
		''' Calculates and sets serializable field offsets, as well as primitive
		''' data size and object field count totals.  Throws InvalidClassException
		''' if fields are illegally ordered.
		''' </summary>
		Private Sub computeFieldOffsets()
			primDataSize = 0
			numObjFields = 0
			Dim firstObjIndex As Integer = -1

			For i As Integer = 0 To fields.Length - 1
				Dim f As ObjectStreamField = fields(i)
				Select Case f.typeCode
					Case "Z"c, "B"c
						f.offset = primDataSize
						primDataSize += 1

					Case "C"c, "S"c
						f.offset = primDataSize
						primDataSize += 2

					Case "I"c, "F"c
						f.offset = primDataSize
						primDataSize += 4

					Case "J"c, "D"c
						f.offset = primDataSize
						primDataSize += 8

					Case "["c, "L"c
						f.offset = numObjFields
						numObjFields += 1
						If firstObjIndex = -1 Then firstObjIndex = i

					Case Else
						Throw New InternalError
				End Select
			Next i
			If firstObjIndex <> -1 AndAlso firstObjIndex + numObjFields <> fields.Length Then Throw New InvalidClassException(name, "illegal field order")
		End Sub

		''' <summary>
		''' If given class is the same as the class associated with this class
		''' descriptor, returns reference to this class descriptor.  Otherwise,
		''' returns variant of this class descriptor bound to given class.
		''' </summary>
		Private Function getVariantFor(ByVal cl As [Class]) As ObjectStreamClass
			If Me.cl Is cl Then Return Me
			Dim desc As New ObjectStreamClass
			If isProxy_Renamed Then
				desc.initProxy(cl, Nothing, superDesc)
			Else
				desc.initNonProxy(Me, cl, Nothing, superDesc)
			End If
			Return desc
		End Function

		''' <summary>
		''' Returns public no-arg constructor of given [Class], or null if none found.
		''' Access checks are disabled on the returned constructor (if any), since
		''' the defining class may still be non-public.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getExternalizableConstructor(ByVal cl As [Class]) As Constructor(Of ?)
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cons As Constructor(Of ?) = cl.getDeclaredConstructor(CType(Nothing, Class()))
				cons.accessible = True
				Return If((cons.modifiers And Modifier.PUBLIC) <> 0, cons, Nothing)
			Catch ex As NoSuchMethodException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Returns subclass-accessible no-arg constructor of first non-serializable
		''' superclass, or null if none found.  Access checks are disabled on the
		''' returned constructor (if any).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getSerializableConstructor(ByVal cl As [Class]) As Constructor(Of ?)
			Dim initCl As  [Class] = cl
			Do While initCl.IsSubclassOf(GetType(Serializable))
				initCl = initCl.BaseType
				If initCl Is Nothing Then Return Nothing
			Loop
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cons As Constructor(Of ?) = initCl.getDeclaredConstructor(CType(Nothing, Class()))
				Dim mods As Integer = cons.modifiers
				If (mods And Modifier.PRIVATE) <> 0 OrElse ((mods And (Modifier.PUBLIC Or Modifier.PROTECTED)) = 0 AndAlso (Not packageEquals(cl, initCl))) Then Return Nothing
				cons = reflFactory.newConstructorForSerialization(cl, cons)
				cons.accessible = True
				Return cons
			Catch ex As NoSuchMethodException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Returns non-static, non-abstract method with given signature provided it
		''' is defined by or accessible (via inheritance) by the given [Class], or
		''' null if no match found.  Access checks are disabled on the returned
		''' method (if any).
		''' </summary>
		Private Shared Function getInheritableMethod(ByVal cl As [Class], ByVal name As String, ByVal argTypes As  [Class](), ByVal returnType As [Class]) As Method
			Dim meth As Method = Nothing
			Dim defCl As  [Class] = cl
			Do While defCl IsNot Nothing
				Try
					meth = defCl.getDeclaredMethod(name, argTypes)
					Exit Do
				Catch ex As NoSuchMethodException
					defCl = defCl.BaseType
				End Try
			Loop

			If (meth Is Nothing) OrElse (meth.returnType IsNot returnType) Then Return Nothing
			meth.accessible = True
			Dim mods As Integer = meth.modifiers
			If (mods And (Modifier.STATIC Or Modifier.ABSTRACT)) <> 0 Then
				Return Nothing
			ElseIf (mods And (Modifier.PUBLIC Or Modifier.PROTECTED)) <> 0 Then
				Return meth
			ElseIf (mods And Modifier.PRIVATE) <> 0 Then
				Return If(cl Is defCl, meth, Nothing)
			Else
				Return If(packageEquals(cl, defCl), meth, Nothing)
			End If
		End Function

		''' <summary>
		''' Returns non-static private method with given signature defined by given
		''' [Class], or null if none found.  Access checks are disabled on the
		''' returned method (if any).
		''' </summary>
		Private Shared Function getPrivateMethod(ByVal cl As [Class], ByVal name As String, ByVal argTypes As  [Class](), ByVal returnType As [Class]) As Method
			Try
				Dim meth As Method = cl.getDeclaredMethod(name, argTypes)
				meth.accessible = True
				Dim mods As Integer = meth.modifiers
				Return If((meth.returnType Is returnType) AndAlso ((mods And Modifier.STATIC) = 0) AndAlso ((mods And Modifier.PRIVATE) <> 0), meth, Nothing)
			Catch ex As NoSuchMethodException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Returns true if classes are defined in the same runtime package, false
		''' otherwise.
		''' </summary>
		Private Shared Function packageEquals(ByVal cl1 As [Class], ByVal cl2 As [Class]) As Boolean
			Return (cl1.classLoader Is cl2.classLoader AndAlso getPackageName(cl1).Equals(getPackageName(cl2)))
		End Function

		''' <summary>
		''' Returns package name of given class.
		''' </summary>
		Private Shared Function getPackageName(ByVal cl As [Class]) As String
			Dim s As String = cl.name
			Dim i As Integer = s.LastIndexOf("["c)
			If i >= 0 Then s = s.Substring(i + 2)
			i = s.LastIndexOf("."c)
			Return If(i >= 0, s.Substring(0, i), "")
		End Function

		''' <summary>
		''' Compares class names for equality, ignoring package names.  Returns true
		''' if class names equal, false otherwise.
		''' </summary>
		Private Shared Function classNamesEqual(ByVal name1 As String, ByVal name2 As String) As Boolean
			name1 = name1.Substring(name1.LastIndexOf("."c) + 1)
			name2 = name2.Substring(name2.LastIndexOf("."c) + 1)
			Return name1.Equals(name2)
		End Function

		''' <summary>
		''' Returns JVM type signature for given class.
		''' </summary>
		Private Shared Function getClassSignature(ByVal cl As [Class]) As String
			Dim sbuf As New StringBuilder
			Do While cl.array
				sbuf.append("["c)
				cl = cl.componentType
			Loop
			If cl.primitive Then
				If cl Is Integer.TYPE Then
					sbuf.append("I"c)
				ElseIf cl Is Byte.TYPE Then
					sbuf.append("B"c)
				ElseIf cl Is Long.TYPE Then
					sbuf.append("J"c)
				ElseIf cl Is Float.TYPE Then
					sbuf.append("F"c)
				ElseIf cl Is Double.TYPE Then
					sbuf.append("D"c)
				ElseIf cl Is Short.TYPE Then
					sbuf.append("S"c)
				ElseIf cl Is Character.TYPE Then
					sbuf.append("C"c)
				ElseIf cl Is Boolean.TYPE Then
					sbuf.append("Z"c)
				ElseIf cl Is Void.TYPE Then
					sbuf.append("V"c)
				Else
					Throw New InternalError
				End If
			Else
				sbuf.append(AscW("L"c) + cl.name.replace("."c, "/"c) + AscW(";"c))
			End If
			Return sbuf.ToString()
		End Function

		''' <summary>
		''' Returns JVM type signature for given list of parameters and return type.
		''' </summary>
		Private Shared Function getMethodSignature(ByVal paramTypes As  [Class](), ByVal retType As [Class]) As String
			Dim sbuf As New StringBuilder
			sbuf.append("("c)
			For i As Integer = 0 To paramTypes.Length - 1
				sbuf.append(getClassSignature(paramTypes(i)))
			Next i
			sbuf.append(")"c)
			sbuf.append(getClassSignature(retType))
			Return sbuf.ToString()
		End Function

		''' <summary>
		''' Convenience method for throwing an exception that is either a
		''' RuntimeException, Error, or of some unexpected type (in which case it is
		''' wrapped inside an IOException).
		''' </summary>
		Private Shared Sub throwMiscException(ByVal th As Throwable)
			If TypeOf th Is RuntimeException Then
				Throw CType(th, RuntimeException)
			ElseIf TypeOf th Is Error Then
				Throw CType(th, [Error])
			Else
				Dim ex As New IOException("unexpected exception type")
				ex.initCause(th)
				Throw ex
			End If
		End Sub

		''' <summary>
		''' Returns ObjectStreamField array describing the serializable fields of
		''' the given class.  Serializable fields backed by an actual field of the
		''' class are represented by ObjectStreamFields with corresponding non-null
		''' Field objects.  Throws InvalidClassException if the (explicitly
		''' declared) serializable fields are invalid.
		''' </summary>
		Private Shared Function getSerialFields(ByVal cl As [Class]) As ObjectStreamField()
			Dim fields_Renamed As ObjectStreamField()
			If cl.IsSubclassOf(GetType(Serializable)) AndAlso (Not cl.IsSubclassOf(GetType(Externalizable))) AndAlso (Not Proxy.isProxyClass(cl)) AndAlso (Not cl.interface) Then
				fields_Renamed = getDeclaredSerialFields(cl)
				If fields_Renamed Is Nothing Then fields_Renamed = getDefaultSerialFields(cl)
				java.util.Arrays.sort(fields_Renamed)
			Else
				fields_Renamed = NO_FIELDS
			End If
			Return fields_Renamed
		End Function

		''' <summary>
		''' Returns serializable fields of given class as defined explicitly by a
		''' "serialPersistentFields" field, or null if no appropriate
		''' "serialPersistentFields" field is defined.  Serializable fields backed
		''' by an actual field of the class are represented by ObjectStreamFields
		''' with corresponding non-null Field objects.  For compatibility with past
		''' releases, a "serialPersistentFields" field with a null value is
		''' considered equivalent to not declaring "serialPersistentFields".  Throws
		''' InvalidClassException if the declared serializable fields are
		''' invalid--e.g., if multiple fields share the same name.
		''' </summary>
		Private Shared Function getDeclaredSerialFields(ByVal cl As [Class]) As ObjectStreamField()
			Dim serialPersistentFields As ObjectStreamField() = Nothing
			Try
				Dim f As Field = cl.getDeclaredField("serialPersistentFields")
				Dim mask As Integer = Modifier.PRIVATE Or Modifier.STATIC Or Modifier.FINAL
				If (f.modifiers And mask) = mask Then
					f.accessible = True
					serialPersistentFields = CType(f.get(Nothing), ObjectStreamField())
				End If
			Catch ex As Exception
			End Try
			If serialPersistentFields Is Nothing Then
				Return Nothing
			ElseIf serialPersistentFields.Length = 0 Then
				Return NO_FIELDS
			End If

			Dim boundFields As ObjectStreamField() = New ObjectStreamField(serialPersistentFields.Length - 1){}
			Dim fieldNames As java.util.Set(Of String) = New HashSet(Of String)(serialPersistentFields.Length)

			For i As Integer = 0 To serialPersistentFields.Length - 1
				Dim spf As ObjectStreamField = serialPersistentFields(i)

				Dim fname As String = spf.name
				If fieldNames.contains(fname) Then Throw New InvalidClassException("multiple serializable fields named " & fname)
				fieldNames.add(fname)

				Try
					Dim f As Field = cl.getDeclaredField(fname)
					If (f.type Is spf.type) AndAlso ((f.modifiers And Modifier.STATIC) = 0) Then boundFields(i) = New ObjectStreamField(f, spf.unshared, True)
				Catch ex As NoSuchFieldException
				End Try
				If boundFields(i) Is Nothing Then boundFields(i) = New ObjectStreamField(fname, spf.type, spf.unshared)
			Next i
			Return boundFields
		End Function

		''' <summary>
		''' Returns array of ObjectStreamFields corresponding to all non-static
		''' non-transient fields declared by given class.  Each ObjectStreamField
		''' contains a Field object for the field it represents.  If no default
		''' serializable fields exist, NO_FIELDS is returned.
		''' </summary>
		Private Shared Function getDefaultSerialFields(ByVal cl As [Class]) As ObjectStreamField()
			Dim clFields As Field() = cl.declaredFields
			Dim list As New List(Of ObjectStreamField)
			Dim mask As Integer = Modifier.STATIC Or Modifier.TRANSIENT

			For i As Integer = 0 To clFields.Length - 1
				If (clFields(i).modifiers And mask) = 0 Then list.Add(New ObjectStreamField(clFields(i), False, True))
			Next i
			Dim size As Integer = list.Count
			Return If(size = 0, NO_FIELDS, list.ToArray())
		End Function

		''' <summary>
		''' Returns explicit serial version UID value declared by given [Class], or
		''' null if none.
		''' </summary>
		Private Shared Function getDeclaredSUID(ByVal cl As [Class]) As Long?
			Try
				Dim f As Field = cl.getDeclaredField("serialVersionUID")
				Dim mask As Integer = Modifier.STATIC Or Modifier.FINAL
				If (f.modifiers And mask) = mask Then
					f.accessible = True
					Return Convert.ToInt64(f.getLong(Nothing))
				End If
			Catch ex As Exception
			End Try
			Return Nothing
		End Function

		''' <summary>
		''' Computes the default serial version UID value for the given class.
		''' </summary>
		Private Shared Function computeDefaultSUID(ByVal cl As [Class]) As Long
			If (Not cl.IsSubclassOf(GetType(Serializable))) OrElse Proxy.isProxyClass(cl) Then Return 0L

			Try
				Dim bout As New ByteArrayOutputStream
				Dim dout As New DataOutputStream(bout)

				dout.writeUTF(cl.name)

				Dim classMods As Integer = cl.modifiers And (Modifier.PUBLIC Or Modifier.FINAL Or Modifier.INTERFACE Or Modifier.ABSTRACT)

	'            
	'             * compensate for javac bug in which ABSTRACT bit was set for an
	'             * interface only if the interface declared methods
	'             
				Dim methods As Method() = cl.declaredMethods
				If (classMods And Modifier.INTERFACE) <> 0 Then classMods = If(methods.Length > 0, (classMods Or Modifier.ABSTRACT), (classMods And (Not Modifier.ABSTRACT)))
				dout.writeInt(classMods)

				If Not cl.array Then
	'                
	'                 * compensate for change in 1.2FCS in which
	'                 * Class.getInterfaces() was modified to return Cloneable and
	'                 * Serializable for array classes.
	'                 
					Dim interfaces As  [Class]() = cl.interfaces
					Dim ifaceNames As String() = New String(interfaces.Length - 1){}
					For i As Integer = 0 To interfaces.Length - 1
						ifaceNames(i) = interfaces(i).name
					Next i
					java.util.Arrays.sort(ifaceNames)
					For i As Integer = 0 To ifaceNames.Length - 1
						dout.writeUTF(ifaceNames(i))
					Next i
				End If

				Dim fields_Renamed As Field() = cl.declaredFields
				Dim fieldSigs As MemberSignature() = New MemberSignature(fields_Renamed.Length - 1){}
				For i As Integer = 0 To fields_Renamed.Length - 1
					fieldSigs(i) = New MemberSignature(fields_Renamed(i))
				Next i
				java.util.Arrays.sort(fieldSigs, New ComparatorAnonymousInnerClassHelper(Of T)
				For i As Integer = 0 To fieldSigs.Length - 1
					Dim sig As MemberSignature = fieldSigs(i)
					Dim mods As Integer = sig.member.modifiers And (Modifier.PUBLIC Or Modifier.PRIVATE Or Modifier.PROTECTED Or Modifier.STATIC Or Modifier.FINAL Or Modifier.VOLATILE Or Modifier.TRANSIENT)
					If ((mods And Modifier.PRIVATE) = 0) OrElse ((mods And (Modifier.STATIC Or Modifier.TRANSIENT)) = 0) Then
						dout.writeUTF(sig.name)
						dout.writeInt(mods)
						dout.writeUTF(sig.signature)
					End If
				Next i

				If hasStaticInitializer(cl) Then
					dout.writeUTF("<clinit>")
					dout.writeInt(Modifier.STATIC)
					dout.writeUTF("()V")
				End If

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim cons As Constructor(Of ?)() = cl.declaredConstructors
				Dim consSigs As MemberSignature() = New MemberSignature(cons.Length - 1){}
				For i As Integer = 0 To cons.Length - 1
					consSigs(i) = New MemberSignature(cons(i))
				Next i
				java.util.Arrays.sort(consSigs, New ComparatorAnonymousInnerClassHelper2(Of T)
				For i As Integer = 0 To consSigs.Length - 1
					Dim sig As MemberSignature = consSigs(i)
					Dim mods As Integer = sig.member.modifiers And (Modifier.PUBLIC Or Modifier.PRIVATE Or Modifier.PROTECTED Or Modifier.STATIC Or Modifier.FINAL Or Modifier.SYNCHRONIZED Or Modifier.NATIVE Or Modifier.ABSTRACT Or Modifier.STRICT)
					If (mods And Modifier.PRIVATE) = 0 Then
						dout.writeUTF("<init>")
						dout.writeInt(mods)
						dout.writeUTF(sig.signature.replace("/"c, "."c))
					End If
				Next i

				Dim methSigs As MemberSignature() = New MemberSignature(methods.Length - 1){}
				For i As Integer = 0 To methods.Length - 1
					methSigs(i) = New MemberSignature(methods(i))
				Next i
				java.util.Arrays.sort(methSigs, New ComparatorAnonymousInnerClassHelper3(Of T)
				For i As Integer = 0 To methSigs.Length - 1
					Dim sig As MemberSignature = methSigs(i)
					Dim mods As Integer = sig.member.modifiers And (Modifier.PUBLIC Or Modifier.PRIVATE Or Modifier.PROTECTED Or Modifier.STATIC Or Modifier.FINAL Or Modifier.SYNCHRONIZED Or Modifier.NATIVE Or Modifier.ABSTRACT Or Modifier.STRICT)
					If (mods And Modifier.PRIVATE) = 0 Then
						dout.writeUTF(sig.name)
						dout.writeInt(mods)
						dout.writeUTF(sig.signature.replace("/"c, "."c))
					End If
				Next i

				dout.flush()

				Dim md As java.security.MessageDigest = java.security.MessageDigest.getInstance("SHA")
				Dim hashBytes As SByte() = md.digest(bout.toByteArray())
				Dim hash As Long = 0
				For i As Integer = Math.Min(hashBytes.Length, 8) - 1 To 0 Step -1
					hash = (hash << 8) Or (hashBytes(i) And &HFF)
				Next i
				Return hash
			Catch ex As IOException
				Throw New InternalError(ex)
			Catch ex As java.security.NoSuchAlgorithmException
				Throw New SecurityException(ex.Message)
			End Try
		End Function

		Private Class ComparatorAnonymousInnerClassHelper(Of T)
			Implements IComparer(Of T)

			Public Overridable Function compare(ByVal ms1 As MemberSignature, ByVal ms2 As MemberSignature) As Integer
				Return ms1.name.CompareTo(ms2.name)
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClassHelper2(Of T)
			Implements IComparer(Of T)

			Public Overridable Function compare(ByVal ms1 As MemberSignature, ByVal ms2 As MemberSignature) As Integer
				Return ms1.signature.CompareTo(ms2.signature)
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClassHelper3(Of T)
			Implements IComparer(Of T)

			Public Overridable Function compare(ByVal ms1 As MemberSignature, ByVal ms2 As MemberSignature) As Integer
				Dim comp As Integer = ms1.name.CompareTo(ms2.name)
				If comp = 0 Then comp = ms1.signature.CompareTo(ms2.signature)
				Return comp
			End Function
		End Class

		''' <summary>
		''' Returns true if the given class defines a static initializer method,
		''' false otherwise.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function hasStaticInitializer(ByVal cl As [Class]) As Boolean
		End Function

		''' <summary>
		''' Class for computing and caching field/constructor/method signatures
		''' during serialVersionUID calculation.
		''' </summary>
		Private Class MemberSignature

			Public ReadOnly member As Member
			Public ReadOnly name As String
			Public ReadOnly signature As String

			Public Sub New(ByVal field As Field)
				member = field
				name = field.name
				signature = getClassSignature(field.type)
			End Sub

			Public Sub New(Of T1)(ByVal cons As Constructor(Of T1))
				member = cons
				name = cons.name
				signature = getMethodSignature(cons.parameterTypes, Void.TYPE)
			End Sub

			Public Sub New(ByVal meth As Method)
				member = meth
				name = meth.name
				signature = getMethodSignature(meth.parameterTypes, meth.returnType)
			End Sub
		End Class

		''' <summary>
		''' Class for setting and retrieving serializable field values in batch.
		''' </summary>
		' REMIND: dynamically generate these?
		Private Class FieldReflector

			''' <summary>
			''' handle for performing unsafe operations </summary>
			Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

			''' <summary>
			''' fields to operate on </summary>
			Private ReadOnly fields As ObjectStreamField()
			''' <summary>
			''' number of primitive fields </summary>
			Private ReadOnly numPrimFields As Integer
			''' <summary>
			''' unsafe field keys for reading fields - may contain dupes </summary>
			Private ReadOnly readKeys As Long()
			''' <summary>
			''' unsafe fields keys for writing fields - no dupes </summary>
			Private ReadOnly writeKeys As Long()
			''' <summary>
			''' field data offsets </summary>
			Private ReadOnly offsets As Integer()
			''' <summary>
			''' field type codes </summary>
			Private ReadOnly typeCodes As Char()
			''' <summary>
			''' field types </summary>
			Private ReadOnly types As  [Class]()

			''' <summary>
			''' Constructs FieldReflector capable of setting/getting values from the
			''' subset of fields whose ObjectStreamFields contain non-null
			''' reflective Field objects.  ObjectStreamFields with null Fields are
			''' treated as filler, for which get operations return default values
			''' and set operations discard given values.
			''' </summary>
			Friend Sub New(ByVal fields As ObjectStreamField())
				Me.fields = fields
				Dim nfields As Integer = fields.Length
				readKeys = New Long(nfields - 1){}
				writeKeys = New Long(nfields - 1){}
				offsets = New Integer(nfields - 1){}
				typeCodes = New Char(nfields - 1){}
				Dim typeList As New List(Of [Class])
				Dim usedKeys As java.util.Set(Of Long?) = New HashSet(Of Long?)


				For i As Integer = 0 To nfields - 1
					Dim f As ObjectStreamField = fields(i)
					Dim rf As Field = f.field
					Dim key As Long = If(rf IsNot Nothing, unsafe.objectFieldOffset(rf), sun.misc.Unsafe.INVALID_FIELD_OFFSET)
					readKeys(i) = key
					writeKeys(i) = If(usedKeys.add(key), key, sun.misc.Unsafe.INVALID_FIELD_OFFSET)
					offsets(i) = f.offset
					typeCodes(i) = f.typeCode
					If Not f.primitive Then typeList.Add(If(rf IsNot Nothing, rf.type, Nothing))
				Next i

				types = typeList.ToArray()
				numPrimFields = nfields - types.Length
			End Sub

			''' <summary>
			''' Returns list of ObjectStreamFields representing fields operated on
			''' by this reflector.  The shared/unshared values and Field objects
			''' contained by ObjectStreamFields in the list reflect their bindings
			''' to locally defined serializable fields.
			''' </summary>
			Friend Overridable Property fields As ObjectStreamField()
				Get
					Return fields
				End Get
			End Property

			''' <summary>
			''' Fetches the serializable primitive field values of object obj and
			''' marshals them into byte array buf starting at offset 0.  The caller
			''' is responsible for ensuring that obj is of the proper type.
			''' </summary>
			Friend Overridable Sub getPrimFieldValues(ByVal obj As Object, ByVal buf As SByte())
				If obj Is Nothing Then Throw New NullPointerException
	'             assuming checkDefaultSerialize() has been called on the class
	'             * descriptor this FieldReflector was obtained from, no field keys
	'             * in array should be equal to Unsafe.INVALID_FIELD_OFFSET.
	'             
				For i As Integer = 0 To numPrimFields - 1
					Dim key As Long = readKeys(i)
					Dim [off] As Integer = offsets(i)
					Select Case typeCodes(i)
						Case "Z"c
							Bits.putBoolean(buf, [off], unsafe.getBoolean(obj, key))

						Case "B"c
							buf([off]) = unsafe.getByte(obj, key)

						Case "C"c
							Bits.putChar(buf, [off], unsafe.getChar(obj, key))

						Case "S"c
							Bits.putShort(buf, [off], unsafe.getShort(obj, key))

						Case "I"c
							Bits.putInt(buf, [off], unsafe.getInt(obj, key))

						Case "F"c
							Bits.putFloat(buf, [off], unsafe.getFloat(obj, key))

						Case "J"c
							Bits.putLong(buf, [off], unsafe.getLong(obj, key))

						Case "D"c
							Bits.putDouble(buf, [off], unsafe.getDouble(obj, key))

						Case Else
							Throw New InternalError
					End Select
				Next i
			End Sub

			''' <summary>
			''' Sets the serializable primitive fields of object obj using values
			''' unmarshalled from byte array buf starting at offset 0.  The caller
			''' is responsible for ensuring that obj is of the proper type.
			''' </summary>
			Friend Overridable Sub setPrimFieldValues(ByVal obj As Object, ByVal buf As SByte())
				If obj Is Nothing Then Throw New NullPointerException
				For i As Integer = 0 To numPrimFields - 1
					Dim key As Long = writeKeys(i)
					If key = sun.misc.Unsafe.INVALID_FIELD_OFFSET Then Continue For ' discard value
					Dim [off] As Integer = offsets(i)
					Select Case typeCodes(i)
						Case "Z"c
							unsafe.putBoolean(obj, key, Bits.getBoolean(buf, [off]))

						Case "B"c
							unsafe.putByte(obj, key, buf([off]))

						Case "C"c
							unsafe.putChar(obj, key, Bits.getChar(buf, [off]))

						Case "S"c
							unsafe.putShort(obj, key, Bits.getShort(buf, [off]))

						Case "I"c
							unsafe.putInt(obj, key, Bits.getInt(buf, [off]))

						Case "F"c
							unsafe.putFloat(obj, key, Bits.getFloat(buf, [off]))

						Case "J"c
							unsafe.putLong(obj, key, Bits.getLong(buf, [off]))

						Case "D"c
							unsafe.putDouble(obj, key, Bits.getDouble(buf, [off]))

						Case Else
							Throw New InternalError
					End Select
				Next i
			End Sub

			''' <summary>
			''' Fetches the serializable object field values of object obj and
			''' stores them in array vals starting at offset 0.  The caller is
			''' responsible for ensuring that obj is of the proper type.
			''' </summary>
			Friend Overridable Sub getObjFieldValues(ByVal obj As Object, ByVal vals As Object())
				If obj Is Nothing Then Throw New NullPointerException
	'             assuming checkDefaultSerialize() has been called on the class
	'             * descriptor this FieldReflector was obtained from, no field keys
	'             * in array should be equal to Unsafe.INVALID_FIELD_OFFSET.
	'             
				For i As Integer = numPrimFields To fields.Length - 1
					Select Case typeCodes(i)
						Case "L"c, "["c
							vals(offsets(i)) = unsafe.getObject(obj, readKeys(i))

						Case Else
							Throw New InternalError
					End Select
				Next i
			End Sub

			''' <summary>
			''' Sets the serializable object fields of object obj using values from
			''' array vals starting at offset 0.  The caller is responsible for
			''' ensuring that obj is of the proper type; however, attempts to set a
			''' field with a value of the wrong type will trigger an appropriate
			''' ClassCastException.
			''' </summary>
			Friend Overridable Sub setObjFieldValues(ByVal obj As Object, ByVal vals As Object())
				If obj Is Nothing Then Throw New NullPointerException
				For i As Integer = numPrimFields To fields.Length - 1
					Dim key As Long = writeKeys(i)
					If key = sun.misc.Unsafe.INVALID_FIELD_OFFSET Then Continue For ' discard value
					Select Case typeCodes(i)
						Case "L"c, "["c
							Dim val As Object = vals(offsets(i))
							If val IsNot Nothing AndAlso (Not types(i - numPrimFields).isInstance(val)) Then
								Dim f As Field = fields(i).field
								Throw New ClassCastException("cannot assign instance of " & val.GetType().name & " to field " & f.declaringClass.name & "." & f.name & " of type " & f.type.name & " in instance of " & obj.GetType().name)
							End If
							unsafe.putObject(obj, key, val)

						Case Else
							Throw New InternalError
					End Select
				Next i
			End Sub
		End Class

		''' <summary>
		''' Matches given set of serializable fields with serializable fields
		''' described by the given local class descriptor, and returns a
		''' FieldReflector instance capable of setting/getting values from the
		''' subset of fields that match (non-matching fields are treated as filler,
		''' for which get operations return default values and set operations
		''' discard given values).  Throws InvalidClassException if unresolvable
		''' type conflicts exist between the two sets of fields.
		''' </summary>
		Private Shared Function getReflector(ByVal fields As ObjectStreamField(), ByVal localDesc As ObjectStreamClass) As FieldReflector
			' class irrelevant if no fields
			Dim cl As  [Class] = If(localDesc IsNot Nothing AndAlso fields.Length > 0, localDesc.cl, Nothing)
			processQueue(Caches.reflectorsQueue, Caches.reflectors)
			Dim key As New FieldReflectorKey(cl, fields, Caches.reflectorsQueue)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ref As Reference(Of ?) = Caches.reflectors.get(key)
			Dim entry As Object = Nothing
			If ref IsNot Nothing Then entry = ref.get()
			Dim future As EntryFuture = Nothing
			If entry Is Nothing Then
				Dim newEntry As New EntryFuture
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim newRef As Reference(Of ?) = New SoftReference(Of ?)(newEntry)
				Do
					If ref IsNot Nothing Then Caches.reflectors.remove(key, ref)
					ref = Caches.reflectors.putIfAbsent(key, newRef)
					If ref IsNot Nothing Then entry = ref.get()
				Loop While ref IsNot Nothing AndAlso entry Is Nothing
				If entry Is Nothing Then future = newEntry
			End If

			If TypeOf entry Is FieldReflector Then ' check common case first
				Return CType(entry, FieldReflector)
			ElseIf TypeOf entry Is EntryFuture Then
				entry = CType(entry, EntryFuture).get()
			ElseIf entry Is Nothing Then
				Try
					entry = New FieldReflector(matchFields(fields, localDesc))
				Catch th As Throwable
					entry = th
				End Try
				future.set(entry)
				Caches.reflectors.put(key, New SoftReference(Of Object)(entry))
			End If

			If TypeOf entry Is FieldReflector Then
				Return CType(entry, FieldReflector)
			ElseIf TypeOf entry Is InvalidClassException Then
				Throw CType(entry, InvalidClassException)
			ElseIf TypeOf entry Is RuntimeException Then
				Throw CType(entry, RuntimeException)
			ElseIf TypeOf entry Is Error Then
				Throw CType(entry, [Error])
			Else
				Throw New InternalError("unexpected entry: " & entry)
			End If
		End Function

		''' <summary>
		''' FieldReflector cache lookup key.  Keys are considered equal if they
		''' refer to the same class and equivalent field formats.
		''' </summary>
		Private Class FieldReflectorKey
			Inherits WeakReference(Of [Class])

			Private ReadOnly sigs As String
			Private ReadOnly hash As Integer
			Private ReadOnly nullClass As Boolean

			Friend Sub New(ByVal cl As [Class], ByVal fields As ObjectStreamField(), ByVal queue As ReferenceQueue(Of [Class]))
				MyBase.New(cl, queue)
				nullClass = (cl Is Nothing)
				Dim sbuf As New StringBuilder
				For i As Integer = 0 To fields.Length - 1
					Dim f As ObjectStreamField = fields(i)
					sbuf.append(f.name).append(f.signature)
				Next i
				sigs = sbuf.ToString()
				hash = System.identityHashCode(cl) + sigs.GetHashCode()
			End Sub

			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If obj Is Me Then Return True

				If TypeOf obj Is FieldReflectorKey Then
					Dim other As FieldReflectorKey = CType(obj, FieldReflectorKey)
					Dim referent As  [Class]
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (If(nullClass, other.nullClass, ((referent = get()) IsNot Nothing) AndAlso (referent Is other.get()))) AndAlso sigs.Equals(other.sigs)
				Else
					Return False
				End If
			End Function
		End Class

		''' <summary>
		''' Matches given set of serializable fields with serializable fields
		''' obtained from the given local class descriptor (which contain bindings
		''' to reflective Field objects).  Returns list of ObjectStreamFields in
		''' which each ObjectStreamField whose signature matches that of a local
		''' field contains a Field object for that field; unmatched
		''' ObjectStreamFields contain null Field objects.  Shared/unshared settings
		''' of the returned ObjectStreamFields also reflect those of matched local
		''' ObjectStreamFields.  Throws InvalidClassException if unresolvable type
		''' conflicts exist between the two sets of fields.
		''' </summary>
		Private Shared Function matchFields(ByVal fields As ObjectStreamField(), ByVal localDesc As ObjectStreamClass) As ObjectStreamField()
			Dim localFields As ObjectStreamField() = If(localDesc IsNot Nothing, localDesc.fields, NO_FIELDS)

	'        
	'         * Even if fields == localFields, we cannot simply return localFields
	'         * here.  In previous implementations of serialization,
	'         * ObjectStreamField.getType() returned Object.class if the
	'         * ObjectStreamField represented a non-primitive field and belonged to
	'         * a non-local class descriptor.  To preserve this (questionable)
	'         * behavior, the ObjectStreamField instances returned by matchFields
	'         * cannot report non-primitive types other than Object.class; hence
	'         * localFields cannot be returned directly.
	'         

			Dim matches As ObjectStreamField() = New ObjectStreamField(fields.Length - 1){}
			For i As Integer = 0 To fields.Length - 1
				Dim f As ObjectStreamField = fields(i), m As ObjectStreamField = Nothing
				For j As Integer = 0 To localFields.Length - 1
					Dim lf As ObjectStreamField = localFields(j)
					If f.name.Equals(lf.name) Then
						If (f.primitive OrElse lf.primitive) AndAlso f.typeCode <> lf.typeCode Then Throw New InvalidClassException(localDesc.name, "incompatible types for field " & f.name)
						If lf.field IsNot Nothing Then
							m = New ObjectStreamField(lf.field, lf.unshared, False)
						Else
							m = New ObjectStreamField(lf.name, lf.signature, lf.unshared)
						End If
					End If
				Next j
				If m Is Nothing Then m = New ObjectStreamField(f.name, f.signature, False)
				m.offset = f.offset
				matches(i) = m
			Next i
			Return matches
		End Function

		''' <summary>
		''' Removes from the specified map any keys that have been enqueued
		''' on the specified reference queue.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Sub processQueue(Of T1 As WeakReference(Of [Class]), ?)(ByVal queue As ReferenceQueue(Of [Class]), ByVal map As java.util.concurrent.ConcurrentMap(Of T1))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ref As Reference(Of ? As [Class])
			ref = queue.poll()
			Do While ref IsNot Nothing
				map.remove(ref)
				ref = queue.poll()
			Loop
		End Sub

		''' <summary>
		'''  Weak key for Class objects.
		''' 
		''' 
		''' </summary>
		Friend Class WeakClassKey
			Inherits WeakReference(Of [Class])

			''' <summary>
			''' saved value of the referent's identity hash code, to maintain
			''' a consistent hash code after the referent has been cleared
			''' </summary>
			Private ReadOnly hash As Integer

			''' <summary>
			''' Create a new WeakClassKey to the given object, registered
			''' with a queue.
			''' </summary>
			Friend Sub New(ByVal cl As [Class], ByVal refQueue As ReferenceQueue(Of [Class]))
				MyBase.New(cl, refQueue)
				hash = System.identityHashCode(cl)
			End Sub

			''' <summary>
			''' Returns the identity hash code of the original referent.
			''' </summary>
			Public Overrides Function GetHashCode() As Integer
				Return hash
			End Function

			''' <summary>
			''' Returns true if the given object is this identical
			''' WeakClassKey instance, or, if this object's referent has not
			''' been cleared, if the given object is another WeakClassKey
			''' instance with the identical non-null referent as this one.
			''' </summary>
			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If obj Is Me Then Return True

				If TypeOf obj Is WeakClassKey Then
					Dim referent As Object = get()
					Return (referent IsNot Nothing) AndAlso (referent Is CType(obj, WeakClassKey).get())
				Else
					Return False
				End If
			End Function
		End Class
	End Class

End Namespace