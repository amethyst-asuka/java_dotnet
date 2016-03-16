Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent
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
	''' An ObjectInputStream deserializes primitive data and objects previously
	''' written using an ObjectOutputStream.
	''' 
	''' <p>ObjectOutputStream and ObjectInputStream can provide an application with
	''' persistent storage for graphs of objects when used with a FileOutputStream
	''' and FileInputStream respectively.  ObjectInputStream is used to recover
	''' those objects previously serialized. Other uses include passing objects
	''' between hosts using a socket stream or for marshaling and unmarshaling
	''' arguments and parameters in a remote communication system.
	''' 
	''' <p>ObjectInputStream ensures that the types of all objects in the graph
	''' created from the stream match the classes present in the Java Virtual
	''' Machine.  Classes are loaded as required using the standard mechanisms.
	''' 
	''' <p>Only objects that support the java.io.Serializable or
	''' java.io.Externalizable interface can be read from streams.
	''' 
	''' <p>The method <code>readObject</code> is used to read an object from the
	''' stream.  Java's safe casting should be used to get the desired type.  In
	''' Java, strings and arrays are objects and are treated as objects during
	''' serialization. When read they need to be cast to the expected type.
	''' 
	''' <p>Primitive data types can be read from the stream using the appropriate
	''' method on DataInput.
	''' 
	''' <p>The default deserialization mechanism for objects restores the contents
	''' of each field to the value and type it had when it was written.  Fields
	''' declared as transient or static are ignored by the deserialization process.
	''' References to other objects cause those objects to be read from the stream
	''' as necessary.  Graphs of objects are restored correctly using a reference
	''' sharing mechanism.  New objects are always allocated when deserializing,
	''' which prevents existing objects from being overwritten.
	''' 
	''' <p>Reading an object is analogous to running the constructors of a new
	''' object.  Memory is allocated for the object and initialized to zero (NULL).
	''' No-arg constructors are invoked for the non-serializable classes and then
	''' the fields of the serializable classes are restored from the stream starting
	''' with the serializable class closest to java.lang.object and finishing with
	''' the object's most specific class.
	''' 
	''' <p>For example to read from a stream as written by the example in
	''' ObjectOutputStream:
	''' <br>
	''' <pre>
	'''      FileInputStream fis = new FileInputStream("t.tmp");
	'''      ObjectInputStream ois = new ObjectInputStream(fis);
	''' 
	'''      int i = ois.readInt();
	'''      String today = (String) ois.readObject();
	'''      Date date = (Date) ois.readObject();
	''' 
	'''      ois.close();
	''' </pre>
	''' 
	''' <p>Classes control how they are serialized by implementing either the
	''' java.io.Serializable or java.io.Externalizable interfaces.
	''' 
	''' <p>Implementing the Serializable interface allows object serialization to
	''' save and restore the entire state of the object and it allows classes to
	''' evolve between the time the stream is written and the time it is read.  It
	''' automatically traverses references between objects, saving and restoring
	''' entire graphs.
	''' 
	''' <p>Serializable classes that require special handling during the
	''' serialization and deserialization process should implement the following
	''' methods:
	''' 
	''' <pre>
	''' private  Sub  writeObject(java.io.ObjectOutputStream stream)
	'''     throws IOException;
	''' private  Sub  readObject(java.io.ObjectInputStream stream)
	'''     throws IOException, ClassNotFoundException;
	''' private  Sub  readObjectNoData()
	'''     throws ObjectStreamException;
	''' </pre>
	''' 
	''' <p>The readObject method is responsible for reading and restoring the state
	''' of the object for its particular class using data written to the stream by
	''' the corresponding writeObject method.  The method does not need to concern
	''' itself with the state belonging to its superclasses or subclasses.  State is
	''' restored by reading data from the ObjectInputStream for the individual
	''' fields and making assignments to the appropriate fields of the object.
	''' Reading primitive data types is supported by DataInput.
	''' 
	''' <p>Any attempt to read object data which exceeds the boundaries of the
	''' custom data written by the corresponding writeObject method will cause an
	''' OptionalDataException to be thrown with an eof field value of true.
	''' Non-object reads which exceed the end of the allotted data will reflect the
	''' end of data in the same way that they would indicate the end of the stream:
	''' bytewise reads will return -1 as the byte read or number of bytes read, and
	''' primitive reads will throw EOFExceptions.  If there is no corresponding
	''' writeObject method, then the end of default serialized data marks the end of
	''' the allotted data.
	''' 
	''' <p>Primitive and object read calls issued from within a readExternal method
	''' behave in the same manner--if the stream is already positioned at the end of
	''' data written by the corresponding writeExternal method, object reads will
	''' throw OptionalDataExceptions with eof set to true, bytewise reads will
	''' return -1, and primitive reads will throw EOFExceptions.  Note that this
	''' behavior does not hold for streams written with the old
	''' <code>ObjectStreamConstants.PROTOCOL_VERSION_1</code> protocol, in which the
	''' end of data written by writeExternal methods is not demarcated, and hence
	''' cannot be detected.
	''' 
	''' <p>The readObjectNoData method is responsible for initializing the state of
	''' the object for its particular class in the event that the serialization
	''' stream does not list the given class as a superclass of the object being
	''' deserialized.  This may occur in cases where the receiving party uses a
	''' different version of the deserialized instance's class than the sending
	''' party, and the receiver's version extends classes that are not extended by
	''' the sender's version.  This may also occur if the serialization stream has
	''' been tampered; hence, readObjectNoData is useful for initializing
	''' deserialized objects properly despite a "hostile" or incomplete source
	''' stream.
	''' 
	''' <p>Serialization does not read or assign values to the fields of any object
	''' that does not implement the java.io.Serializable interface.  Subclasses of
	''' Objects that are not serializable can be serializable. In this case the
	''' non-serializable class must have a no-arg constructor to allow its fields to
	''' be initialized.  In this case it is the responsibility of the subclass to
	''' save and restore the state of the non-serializable class. It is frequently
	''' the case that the fields of that class are accessible (public, package, or
	''' protected) or that there are get and set methods that can be used to restore
	''' the state.
	''' 
	''' <p>Any exception that occurs while deserializing an object will be caught by
	''' the ObjectInputStream and abort the reading process.
	''' 
	''' <p>Implementing the Externalizable interface allows the object to assume
	''' complete control over the contents and format of the object's serialized
	''' form.  The methods of the Externalizable interface, writeExternal and
	''' readExternal, are called to save and restore the objects state.  When
	''' implemented by a class they can write and read their own state using all of
	''' the methods of ObjectOutput and ObjectInput.  It is the responsibility of
	''' the objects to handle any versioning that occurs.
	''' 
	''' <p>Enum constants are deserialized differently than ordinary serializable or
	''' externalizable objects.  The serialized form of an enum constant consists
	''' solely of its name; field values of the constant are not transmitted.  To
	''' deserialize an enum constant, ObjectInputStream reads the constant name from
	''' the stream; the deserialized constant is then obtained by calling the static
	''' method <code>Enum.valueOf(Class, String)</code> with the enum constant's
	''' base type and the received constant name as arguments.  Like other
	''' serializable or externalizable objects, enum constants can function as the
	''' targets of back references appearing subsequently in the serialization
	''' stream.  The process by which enum constants are deserialized cannot be
	''' customized: any class-specific readObject, readObjectNoData, and readResolve
	''' methods defined by enum types are ignored during deserialization.
	''' Similarly, any serialPersistentFields or serialVersionUID field declarations
	''' are also ignored--all enum types have a fixed serialVersionUID of 0L.
	''' 
	''' @author      Mike Warres
	''' @author      Roger Riggs </summary>
	''' <seealso cref= java.io.DataInput </seealso>
	''' <seealso cref= java.io.ObjectOutputStream </seealso>
	''' <seealso cref= java.io.Serializable </seealso>
	''' <seealso cref= <a href="../../../platform/serialization/spec/input.html"> Object Serialization Specification, Section 3, Object Input Classes</a>
	''' @since   JDK1.1 </seealso>
	Public Class ObjectInputStream
		Inherits InputStream
		Implements ObjectInput, ObjectStreamConstants

		''' <summary>
		''' handle value representing null </summary>
		Private Const NULL_HANDLE As Integer = -1

		''' <summary>
		''' marker for unshared objects in internal handle table </summary>
		Private Shared ReadOnly unsharedMarker As New Object

		''' <summary>
		''' table mapping primitive type names to corresponding class objects </summary>
		Private Shared ReadOnly primClasses As New Dictionary(Of String, [Class])(8, 1.0F)
		Shared Sub New()
			primClasses("boolean") = GetType(Boolean)
			primClasses("byte") = GetType(SByte)
			primClasses("char") = GetType(Char)
			primClasses("short") = GetType(Short)
			primClasses("int") = GetType(Integer)
			primClasses("long") = GetType(Long)
			primClasses("float") = GetType(Single)
			primClasses("double") = GetType(Double)
			primClasses("void") = GetType(void)
		End Sub

		Private Class Caches
			''' <summary>
			''' cache of subclass security audit results </summary>
			Friend Shared ReadOnly subclassAudits As java.util.concurrent.ConcurrentMap(Of java.io.ObjectStreamClass.WeakClassKey, Boolean?) = New ConcurrentDictionary(Of java.io.ObjectStreamClass.WeakClassKey, Boolean?)

			''' <summary>
			''' queue for WeakReferences to audited subclasses </summary>
			Friend Shared ReadOnly subclassAuditsQueue As New ReferenceQueue(Of [Class])
		End Class

		''' <summary>
		''' filter stream for handling block data conversion </summary>
		Private ReadOnly bin As BlockDataInputStream
		''' <summary>
		''' validation callback list </summary>
		Private ReadOnly vlist As ValidationList
		''' <summary>
		''' recursion depth </summary>
		Private depth As Integer
		''' <summary>
		''' whether stream is closed </summary>
		Private closed As Boolean

		''' <summary>
		''' wire handle -> obj/exception map </summary>
		Private ReadOnly [handles] As HandleTable
		''' <summary>
		''' scratch field for passing handle values up/down call stack </summary>
		Private passHandle As Integer = NULL_HANDLE
		''' <summary>
		''' flag set when at end of field value block with no TC_ENDBLOCKDATA </summary>
		Private defaultDataEnd As Boolean = False

		''' <summary>
		''' buffer for reading primitive field values </summary>
		Private primVals As SByte()

		''' <summary>
		''' if true, invoke readObjectOverride() instead of readObject() </summary>
		Private ReadOnly enableOverride As Boolean
		''' <summary>
		''' if true, invoke resolveObject() </summary>
		Private enableResolve As Boolean

		''' <summary>
		''' Context during upcalls to class-defined readObject methods; holds
		''' object currently being deserialized and descriptor for current class.
		''' Null when not during readObject upcall.
		''' </summary>
		Private curContext As SerialCallbackContext

		''' <summary>
		''' Creates an ObjectInputStream that reads from the specified InputStream.
		''' A serialization stream header is read from the stream and verified.
		''' This constructor will block until the corresponding ObjectOutputStream
		''' has written and flushed the header.
		''' 
		''' <p>If a security manager is installed, this constructor will check for
		''' the "enableSubclassImplementation" SerializablePermission when invoked
		''' directly or indirectly by the constructor of a subclass which overrides
		''' the ObjectInputStream.readFields or ObjectInputStream.readUnshared
		''' methods.
		''' </summary>
		''' <param name="in"> input stream to read from </param>
		''' <exception cref="StreamCorruptedException"> if the stream header is incorrect </exception>
		''' <exception cref="IOException"> if an I/O error occurs while reading stream header </exception>
		''' <exception cref="SecurityException"> if untrusted subclass illegally overrides
		'''          security-sensitive methods </exception>
		''' <exception cref="NullPointerException"> if <code>in</code> is <code>null</code> </exception>
		''' <seealso cref=     ObjectInputStream#ObjectInputStream() </seealso>
		''' <seealso cref=     ObjectInputStream#readFields() </seealso>
		''' <seealso cref=     ObjectOutputStream#ObjectOutputStream(OutputStream) </seealso>
		Public Sub New(ByVal [in] As InputStream)
			verifySubclass()
			bin = New BlockDataInputStream(Me, [in])
			[handles] = New HandleTable(10)
			vlist = New ValidationList
			enableOverride = False
			readStreamHeader()
			bin.blockDataMode = True
		End Sub

		''' <summary>
		''' Provide a way for subclasses that are completely reimplementing
		''' ObjectInputStream to not have to allocate private data just used by this
		''' implementation of ObjectInputStream.
		''' 
		''' <p>If there is a security manager installed, this method first calls the
		''' security manager's <code>checkPermission</code> method with the
		''' <code>SerializablePermission("enableSubclassImplementation")</code>
		''' permission to ensure it's ok to enable subclassing.
		''' </summary>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''          <code>checkPermission</code> method denies enabling
		'''          subclassing. </exception>
		''' <exception cref="IOException"> if an I/O error occurs while creating this stream </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.io.SerializablePermission </seealso>
		Protected Friend Sub New()
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPermission(SUBCLASS_IMPLEMENTATION_PERMISSION)
			bin = Nothing
			[handles] = Nothing
			vlist = Nothing
			enableOverride = True
		End Sub

		''' <summary>
		''' Read an object from the ObjectInputStream.  The class of the object, the
		''' signature of the [Class], and the values of the non-transient and
		''' non-static fields of the class and all of its supertypes are read.
		''' Default deserializing for a class can be overriden using the writeObject
		''' and readObject methods.  Objects referenced by this object are read
		''' transitively so that a complete equivalent graph of objects is
		''' reconstructed by readObject.
		''' 
		''' <p>The root object is completely restored when all of its fields and the
		''' objects it references are completely restored.  At this point the object
		''' validation callbacks are executed in order based on their registered
		''' priorities. The callbacks are registered by objects (in the readObject
		''' special methods) as they are individually restored.
		''' 
		''' <p>Exceptions are thrown for problems with the InputStream and for
		''' classes that should not be deserialized.  All exceptions are fatal to
		''' the InputStream and leave it in an indeterminate state; it is up to the
		''' caller to ignore or recover the stream state.
		''' </summary>
		''' <exception cref="ClassNotFoundException"> Class of a serialized object cannot be
		'''          found. </exception>
		''' <exception cref="InvalidClassException"> Something is wrong with a class used by
		'''          serialization. </exception>
		''' <exception cref="StreamCorruptedException"> Control information in the
		'''          stream is inconsistent. </exception>
		''' <exception cref="OptionalDataException"> Primitive data was found in the
		'''          stream instead of objects. </exception>
		''' <exception cref="IOException"> Any of the usual Input/Output related exceptions. </exception>
		Public Function readObject() As Object Implements ObjectInput.readObject
			If enableOverride Then Return readObjectOverride()

			' if nested read, passHandle contains handle of enclosing object
			Dim outerHandle As Integer = passHandle
			Try
				Dim obj As Object = readObject0(False)
				[handles].markDependency(outerHandle, passHandle)
				Dim ex As  ClassNotFoundException = [handles].lookupException(passHandle)
				If ex IsNot Nothing Then Throw ex
				If depth = 0 Then vlist.doCallbacks()
				Return obj
			Finally
				passHandle = outerHandle
				If closed AndAlso depth = 0 Then clear()
			End Try
		End Function

		''' <summary>
		''' This method is called by trusted subclasses of ObjectOutputStream that
		''' constructed ObjectOutputStream using the protected no-arg constructor.
		''' The subclass is expected to provide an override method with the modifier
		''' "final".
		''' </summary>
		''' <returns>  the Object read from the stream. </returns>
		''' <exception cref="ClassNotFoundException"> Class definition of a serialized object
		'''          cannot be found. </exception>
		''' <exception cref="OptionalDataException"> Primitive data was found in the stream
		'''          instead of objects. </exception>
		''' <exception cref="IOException"> if I/O errors occurred while reading from the
		'''          underlying stream </exception>
		''' <seealso cref= #ObjectInputStream() </seealso>
		''' <seealso cref= #readObject()
		''' @since 1.2 </seealso>
		Protected Friend Overridable Function readObjectOverride() As Object
			Return Nothing
		End Function

		''' <summary>
		''' Reads an "unshared" object from the ObjectInputStream.  This method is
		''' identical to readObject, except that it prevents subsequent calls to
		''' readObject and readUnshared from returning additional references to the
		''' deserialized instance obtained via this call.  Specifically:
		''' <ul>
		'''   <li>If readUnshared is called to deserialize a back-reference (the
		'''       stream representation of an object which has been written
		'''       previously to the stream), an ObjectStreamException will be
		'''       thrown.
		''' 
		'''   <li>If readUnshared returns successfully, then any subsequent attempts
		'''       to deserialize back-references to the stream handle deserialized
		'''       by readUnshared will cause an ObjectStreamException to be thrown.
		''' </ul>
		''' Deserializing an object via readUnshared invalidates the stream handle
		''' associated with the returned object.  Note that this in itself does not
		''' always guarantee that the reference returned by readUnshared is unique;
		''' the deserialized object may define a readResolve method which returns an
		''' object visible to other parties, or readUnshared may return a Class
		''' object or enum constant obtainable elsewhere in the stream or through
		''' external means. If the deserialized object defines a readResolve method
		''' and the invocation of that method returns an array, then readUnshared
		''' returns a shallow clone of that array; this guarantees that the returned
		''' array object is unique and cannot be obtained a second time from an
		''' invocation of readObject or readUnshared on the ObjectInputStream,
		''' even if the underlying data stream has been manipulated.
		''' 
		''' <p>ObjectInputStream subclasses which override this method can only be
		''' constructed in security contexts possessing the
		''' "enableSubclassImplementation" SerializablePermission; any attempt to
		''' instantiate such a subclass without this permission will cause a
		''' SecurityException to be thrown.
		''' </summary>
		''' <returns>  reference to deserialized object </returns>
		''' <exception cref="ClassNotFoundException"> if class of an object to deserialize
		'''          cannot be found </exception>
		''' <exception cref="StreamCorruptedException"> if control information in the stream
		'''          is inconsistent </exception>
		''' <exception cref="ObjectStreamException"> if object to deserialize has already
		'''          appeared in stream </exception>
		''' <exception cref="OptionalDataException"> if primitive data is next in stream </exception>
		''' <exception cref="IOException"> if an I/O error occurs during deserialization
		''' @since   1.4 </exception>
		Public Overridable Function readUnshared() As Object
			' if nested read, passHandle contains handle of enclosing object
			Dim outerHandle As Integer = passHandle
			Try
				Dim obj As Object = readObject0(True)
				[handles].markDependency(outerHandle, passHandle)
				Dim ex As  ClassNotFoundException = [handles].lookupException(passHandle)
				If ex IsNot Nothing Then Throw ex
				If depth = 0 Then vlist.doCallbacks()
				Return obj
			Finally
				passHandle = outerHandle
				If closed AndAlso depth = 0 Then clear()
			End Try
		End Function

		''' <summary>
		''' Read the non-static and non-transient fields of the current class from
		''' this stream.  This may only be called from the readObject method of the
		''' class being deserialized. It will throw the NotActiveException if it is
		''' called otherwise.
		''' </summary>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''          could not be found. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		''' <exception cref="NotActiveException"> if the stream is not currently reading
		'''          objects. </exception>
		Public Overridable Sub defaultReadObject()
			Dim ctx As SerialCallbackContext = curContext
			If ctx Is Nothing Then Throw New NotActiveException("not in call to readObject")
			Dim curObj As Object = ctx.obj
			Dim curDesc As ObjectStreamClass = ctx.desc
			bin.blockDataMode = False
			defaultReadFields(curObj, curDesc)
			bin.blockDataMode = True
			If Not curDesc.hasWriteObjectData() Then defaultDataEnd = True
			Dim ex As  ClassNotFoundException = [handles].lookupException(passHandle)
			If ex IsNot Nothing Then Throw ex
		End Sub

		''' <summary>
		''' Reads the persistent fields from the stream and makes them available by
		''' name.
		''' </summary>
		''' <returns>  the <code>GetField</code> object representing the persistent
		'''          fields of the object being deserialized </returns>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized object
		'''          could not be found. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		''' <exception cref="NotActiveException"> if the stream is not currently reading
		'''          objects.
		''' @since 1.2 </exception>
		Public Overridable Function readFields() As ObjectInputStream.GetField
			Dim ctx As SerialCallbackContext = curContext
			If ctx Is Nothing Then Throw New NotActiveException("not in call to readObject")
			Dim curObj As Object = ctx.obj
			Dim curDesc As ObjectStreamClass = ctx.desc
			bin.blockDataMode = False
			Dim getField As New GetFieldImpl(Me, curDesc)
			getField.readFields()
			bin.blockDataMode = True
			If Not curDesc.hasWriteObjectData() Then defaultDataEnd = True

			Return getField
		End Function

		''' <summary>
		''' Register an object to be validated before the graph is returned.  While
		''' similar to resolveObject these validations are called after the entire
		''' graph has been reconstituted.  Typically, a readObject method will
		''' register the object with the stream so that when all of the objects are
		''' restored a final set of validations can be performed.
		''' </summary>
		''' <param name="obj"> the object to receive the validation callback. </param>
		''' <param name="prio"> controls the order of callbacks;zero is a good default.
		'''          Use higher numbers to be called back earlier, lower numbers for
		'''          later callbacks. Within a priority, callbacks are processed in
		'''          no particular order. </param>
		''' <exception cref="NotActiveException"> The stream is not currently reading objects
		'''          so it is invalid to register a callback. </exception>
		''' <exception cref="InvalidObjectException"> The validation object is null. </exception>
		Public Overridable Sub registerValidation(ByVal obj As ObjectInputValidation, ByVal prio As Integer)
			If depth = 0 Then Throw New NotActiveException("stream inactive")
			vlist.register(obj, prio)
		End Sub

		''' <summary>
		''' Load the local class equivalent of the specified stream class
		''' description.  Subclasses may implement this method to allow classes to
		''' be fetched from an alternate source.
		''' 
		''' <p>The corresponding method in <code>ObjectOutputStream</code> is
		''' <code>annotateClass</code>.  This method will be invoked only once for
		''' each unique class in the stream.  This method can be implemented by
		''' subclasses to use an alternate loading mechanism but must return a
		''' <code>Class</code> object. Once returned, if the class is not an array
		''' [Class], its serialVersionUID is compared to the serialVersionUID of the
		''' serialized [Class], and if there is a mismatch, the deserialization fails
		''' and an <seealso cref="InvalidClassException"/> is thrown.
		''' 
		''' <p>The default implementation of this method in
		''' <code>ObjectInputStream</code> returns the result of calling
		''' <pre>
		'''     Class.forName(desc.getName(), false, loader)
		''' </pre>
		''' where <code>loader</code> is determined as follows: if there is a
		''' method on the current thread's stack whose declaring class was
		''' defined by a user-defined class loader (and was not a generated to
		''' implement reflective invocations), then <code>loader</code> is class
		''' loader corresponding to the closest such method to the currently
		''' executing frame; otherwise, <code>loader</code> is
		''' <code>null</code>. If this call results in a
		''' <code>ClassNotFoundException</code> and the name of the passed
		''' <code>ObjectStreamClass</code> instance is the Java language keyword
		''' for a primitive type or void, then the <code>Class</code> object
		''' representing that primitive type or  Sub  will be returned
		''' (e.g., an <code>ObjectStreamClass</code> with the name
		''' <code>"int"</code> will be resolved to <code> java.lang.[Integer].TYPE</code>).
		''' Otherwise, the <code>ClassNotFoundException</code> will be thrown to
		''' the caller of this method.
		''' </summary>
		''' <param name="desc"> an instance of class <code>ObjectStreamClass</code> </param>
		''' <returns>  a <code>Class</code> object corresponding to <code>desc</code> </returns>
		''' <exception cref="IOException"> any of the usual Input/Output exceptions. </exception>
		''' <exception cref="ClassNotFoundException"> if class of a serialized object cannot
		'''          be found. </exception>
		Protected Friend Overridable Function resolveClass(ByVal desc As ObjectStreamClass) As  [Class]
			Dim name As String = desc.name
			Try
				Return Type.GetType(name, False, latestUserDefinedLoader())
			Catch ex As  ClassNotFoundException
				Dim cl As  [Class] = primClasses(name)
				If cl IsNot Nothing Then
					Return cl
				Else
					Throw ex
				End If
			End Try
		End Function

		''' <summary>
		''' Returns a proxy class that implements the interfaces named in a proxy
		''' class descriptor; subclasses may implement this method to read custom
		''' data from the stream along with the descriptors for dynamic proxy
		''' classes, allowing them to use an alternate loading mechanism for the
		''' interfaces and the proxy class.
		''' 
		''' <p>This method is called exactly once for each unique proxy class
		''' descriptor in the stream.
		''' 
		''' <p>The corresponding method in <code>ObjectOutputStream</code> is
		''' <code>annotateProxyClass</code>.  For a given subclass of
		''' <code>ObjectInputStream</code> that overrides this method, the
		''' <code>annotateProxyClass</code> method in the corresponding subclass of
		''' <code>ObjectOutputStream</code> must write any data or objects read by
		''' this method.
		''' 
		''' <p>The default implementation of this method in
		''' <code>ObjectInputStream</code> returns the result of calling
		''' <code>Proxy.getProxyClass</code> with the list of <code>Class</code>
		''' objects for the interfaces that are named in the <code>interfaces</code>
		''' parameter.  The <code>Class</code> object for each interface name
		''' <code>i</code> is the value returned by calling
		''' <pre>
		'''     Class.forName(i, false, loader)
		''' </pre>
		''' where <code>loader</code> is that of the first non-<code>null</code>
		''' class loader up the execution stack, or <code>null</code> if no
		''' non-<code>null</code> class loaders are on the stack (the same class
		''' loader choice used by the <code>resolveClass</code> method).  Unless any
		''' of the resolved interfaces are non-public, this same value of
		''' <code>loader</code> is also the class loader passed to
		''' <code>Proxy.getProxyClass</code>; if non-public interfaces are present,
		''' their class loader is passed instead (if more than one non-public
		''' interface class loader is encountered, an
		''' <code>IllegalAccessError</code> is thrown).
		''' If <code>Proxy.getProxyClass</code> throws an
		''' <code>IllegalArgumentException</code>, <code>resolveProxyClass</code>
		''' will throw a <code>ClassNotFoundException</code> containing the
		''' <code>IllegalArgumentException</code>.
		''' </summary>
		''' <param name="interfaces"> the list of interface names that were
		'''                deserialized in the proxy class descriptor </param>
		''' <returns>  a proxy class for the specified interfaces </returns>
		''' <exception cref="IOException"> any exception thrown by the underlying
		'''                <code>InputStream</code> </exception>
		''' <exception cref="ClassNotFoundException"> if the proxy class or any of the
		'''                named interfaces could not be found </exception>
		''' <seealso cref= ObjectOutputStream#annotateProxyClass(Class)
		''' @since 1.3 </seealso>
		Protected Friend Overridable Function resolveProxyClass(ByVal interfaces As String()) As  [Class]
			Dim latestLoader As  ClassLoader = latestUserDefinedLoader()
			Dim nonPublicLoader As  ClassLoader = Nothing
			Dim hasNonPublicInterface As Boolean = False

			' define proxy in class loader of non-public interface(s), if any
			Dim classObjs As  [Class]() = New [Class](interfaces.Length - 1){}
			For i As Integer = 0 To interfaces.Length - 1
				Dim cl As  [Class] = Type.GetType(interfaces(i), False, latestLoader)
				If (cl.modifiers And Modifier.PUBLIC) = 0 Then
					If hasNonPublicInterface Then
						If nonPublicLoader IsNot cl.classLoader Then Throw New IllegalAccessError("conflicting non-public interface class loaders")
					Else
						nonPublicLoader = cl.classLoader
						hasNonPublicInterface = True
					End If
				End If
				classObjs(i) = cl
			Next i
			Try
				Return Proxy.getProxyClass(If(hasNonPublicInterface, nonPublicLoader, latestLoader), classObjs)
			Catch e As IllegalArgumentException
				Throw New ClassNotFoundException(Nothing, e)
			End Try
		End Function

		''' <summary>
		''' This method will allow trusted subclasses of ObjectInputStream to
		''' substitute one object for another during deserialization. Replacing
		''' objects is disabled until enableResolveObject is called. The
		''' enableResolveObject method checks that the stream requesting to resolve
		''' object can be trusted. Every reference to serializable objects is passed
		''' to resolveObject.  To insure that the private state of objects is not
		''' unintentionally exposed only trusted streams may use resolveObject.
		''' 
		''' <p>This method is called after an object has been read but before it is
		''' returned from readObject.  The default resolveObject method just returns
		''' the same object.
		''' 
		''' <p>When a subclass is replacing objects it must insure that the
		''' substituted object is compatible with every field where the reference
		''' will be stored.  Objects whose type is not a subclass of the type of the
		''' field or array element abort the serialization by raising an exception
		''' and the object is not be stored.
		''' 
		''' <p>This method is called only once when each object is first
		''' encountered.  All subsequent references to the object will be redirected
		''' to the new object.
		''' </summary>
		''' <param name="obj"> object to be substituted </param>
		''' <returns>  the substituted object </returns>
		''' <exception cref="IOException"> Any of the usual Input/Output exceptions. </exception>
		Protected Friend Overridable Function resolveObject(ByVal obj As Object) As Object
			Return obj
		End Function

		''' <summary>
		''' Enable the stream to allow objects read from the stream to be replaced.
		''' When enabled, the resolveObject method is called for every object being
		''' deserialized.
		''' 
		''' <p>If <i>enable</i> is true, and there is a security manager installed,
		''' this method first calls the security manager's
		''' <code>checkPermission</code> method with the
		''' <code>SerializablePermission("enableSubstitution")</code> permission to
		''' ensure it's ok to enable the stream to allow objects read from the
		''' stream to be replaced.
		''' </summary>
		''' <param name="enable"> true for enabling use of <code>resolveObject</code> for
		'''          every object being deserialized </param>
		''' <returns>  the previous setting before this method was invoked </returns>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''          <code>checkPermission</code> method denies enabling the stream
		'''          to allow objects read from the stream to be replaced. </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.io.SerializablePermission </seealso>
		Protected Friend Overridable Function enableResolveObject(ByVal enable As Boolean) As Boolean
			If enable = enableResolve Then Return enable
			If enable Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(SUBSTITUTION_PERMISSION)
			End If
			enableResolve = enable
			Return Not enableResolve
		End Function

		''' <summary>
		''' The readStreamHeader method is provided to allow subclasses to read and
		''' verify their own stream headers. It reads and verifies the magic number
		''' and version number.
		''' </summary>
		''' <exception cref="IOException"> if there are I/O errors while reading from the
		'''          underlying <code>InputStream</code> </exception>
		''' <exception cref="StreamCorruptedException"> if control information in the stream
		'''          is inconsistent </exception>
		Protected Friend Overridable Sub readStreamHeader()
			Dim s0 As Short = bin.readShort()
			Dim s1 As Short = bin.readShort()
			If s0 <> STREAM_MAGIC OrElse s1 <> STREAM_VERSION Then Throw New StreamCorruptedException(String.Format("invalid stream header: {0:X4}{1:X4}", s0, s1))
		End Sub

		''' <summary>
		''' Read a class descriptor from the serialization stream.  This method is
		''' called when the ObjectInputStream expects a class descriptor as the next
		''' item in the serialization stream.  Subclasses of ObjectInputStream may
		''' override this method to read in class descriptors that have been written
		''' in non-standard formats (by subclasses of ObjectOutputStream which have
		''' overridden the <code>writeClassDescriptor</code> method).  By default,
		''' this method reads class descriptors according to the format defined in
		''' the Object Serialization specification.
		''' </summary>
		''' <returns>  the class descriptor read </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		''' <exception cref="ClassNotFoundException"> If the Class of a serialized object used
		'''          in the class descriptor representation cannot be found </exception>
		''' <seealso cref= java.io.ObjectOutputStream#writeClassDescriptor(java.io.ObjectStreamClass)
		''' @since 1.3 </seealso>
		Protected Friend Overridable Function readClassDescriptor() As ObjectStreamClass
			Dim desc As New ObjectStreamClass
			desc.readNonProxy(Me)
			Return desc
		End Function

		''' <summary>
		''' Reads a byte of data. This method will block if no input is available.
		''' </summary>
		''' <returns>  the byte read, or -1 if the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Public Overrides Function read() As Integer Implements ObjectInput.read
			Return bin.read()
		End Function

		''' <summary>
		''' Reads into an array of bytes.  This method will block until some input
		''' is available. Consider using java.io.DataInputStream.readFully to read
		''' exactly 'length' bytes.
		''' </summary>
		''' <param name="buf"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns>  the actual number of bytes read, -1 is returned when the end of
		'''          the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		''' <seealso cref= java.io.DataInputStream#readFully(byte[],int,int) </seealso>
		Public Overrides Function read(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			If buf Is Nothing Then Throw New NullPointerException
			Dim endoff As Integer = [off] + len
			If [off] < 0 OrElse len < 0 OrElse endoff > buf.Length OrElse endoff < 0 Then Throw New IndexOutOfBoundsException
			Return bin.read(buf, [off], len, False)
		End Function

		''' <summary>
		''' Returns the number of bytes that can be read without blocking.
		''' </summary>
		''' <returns>  the number of available bytes. </returns>
		''' <exception cref="IOException"> if there are I/O errors while reading from the
		'''          underlying <code>InputStream</code> </exception>
		Public Overrides Function available() As Integer Implements ObjectInput.available
			Return bin.available()
		End Function

		''' <summary>
		''' Closes the input stream. Must be called to release any resources
		''' associated with the stream.
		''' </summary>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Public Overrides Sub close() Implements ObjectInput.close
	'        
	'         * Even if stream already closed, propagate redundant close to
	'         * underlying stream to stay consistent with previous implementations.
	'         
			closed = True
			If depth = 0 Then clear()
			bin.close()
		End Sub

		''' <summary>
		''' Reads in a  java.lang.[Boolean].
		''' </summary>
		''' <returns>  the boolean read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readBoolean() As Boolean Implements DataInput.readBoolean
			Return bin.readBoolean()
		End Function

		''' <summary>
		''' Reads an 8 bit java.lang.[Byte].
		''' </summary>
		''' <returns>  the 8 bit byte read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readByte() As SByte Implements DataInput.readByte
			Return bin.readByte()
		End Function

		''' <summary>
		''' Reads an unsigned 8 bit java.lang.[Byte].
		''' </summary>
		''' <returns>  the 8 bit byte read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readUnsignedByte() As Integer Implements DataInput.readUnsignedByte
			Return bin.readUnsignedByte()
		End Function

		''' <summary>
		''' Reads a 16 bit char.
		''' </summary>
		''' <returns>  the 16 bit char read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readChar() As Char Implements DataInput.readChar
			Return bin.readChar()
		End Function

		''' <summary>
		''' Reads a 16 bit  java.lang.[Short].
		''' </summary>
		''' <returns>  the 16 bit short read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readShort() As Short Implements DataInput.readShort
			Return bin.readShort()
		End Function

		''' <summary>
		''' Reads an unsigned 16 bit  java.lang.[Short].
		''' </summary>
		''' <returns>  the 16 bit short read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readUnsignedShort() As Integer Implements DataInput.readUnsignedShort
			Return bin.readUnsignedShort()
		End Function

		''' <summary>
		''' Reads a 32 bit int.
		''' </summary>
		''' <returns>  the 32 bit integer read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readInt() As Integer Implements DataInput.readInt
			Return bin.readInt()
		End Function

		''' <summary>
		''' Reads a 64 bit java.lang.[Long].
		''' </summary>
		''' <returns>  the read 64 bit java.lang.[Long]. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readLong() As Long Implements DataInput.readLong
			Return bin.readLong()
		End Function

		''' <summary>
		''' Reads a 32 bit float.
		''' </summary>
		''' <returns>  the 32 bit float read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readFloat() As Single Implements DataInput.readFloat
			Return bin.readFloat()
		End Function

		''' <summary>
		''' Reads a 64 bit java.lang.[Double].
		''' </summary>
		''' <returns>  the 64 bit double read. </returns>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Function readDouble() As Double Implements DataInput.readDouble
			Return bin.readDouble()
		End Function

		''' <summary>
		''' Reads bytes, blocking until all bytes are read.
		''' </summary>
		''' <param name="buf"> the buffer into which the data is read </param>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Sub readFully(ByVal buf As SByte()) Implements DataInput.readFully
			bin.readFully(buf, 0, buf.Length, False)
		End Sub

		''' <summary>
		''' Reads bytes, blocking until all bytes are read.
		''' </summary>
		''' <param name="buf"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes to read </param>
		''' <exception cref="EOFException"> If end of file is reached. </exception>
		''' <exception cref="IOException"> If other I/O error has occurred. </exception>
		Public Overridable Sub readFully(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			Dim endoff As Integer = [off] + len
			If [off] < 0 OrElse len < 0 OrElse endoff > buf.Length OrElse endoff < 0 Then Throw New IndexOutOfBoundsException
			bin.readFully(buf, [off], len, False)
		End Sub

		''' <summary>
		''' Skips bytes.
		''' </summary>
		''' <param name="len"> the number of bytes to be skipped </param>
		''' <returns>  the actual number of bytes skipped. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		Public Overridable Function skipBytes(ByVal len As Integer) As Integer Implements DataInput.skipBytes
			Return bin.skipBytes(len)
		End Function

		''' <summary>
		''' Reads in a line that has been terminated by a \n, \r, \r\n or EOF.
		''' </summary>
		''' <returns>  a String copy of the line. </returns>
		''' <exception cref="IOException"> if there are I/O errors while reading from the
		'''          underlying <code>InputStream</code> </exception>
		''' @deprecated This method does not properly convert bytes to characters.
		'''          see DataInputStream for the details and alternatives. 
		<Obsolete("This method does not properly convert bytes to characters.")> _
		Public Overridable Function readLine() As String Implements DataInput.readLine
			Return bin.readLine()
		End Function

		''' <summary>
		''' Reads a String in
		''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		''' format.
		''' </summary>
		''' <returns>  the String. </returns>
		''' <exception cref="IOException"> if there are I/O errors while reading from the
		'''          underlying <code>InputStream</code> </exception>
		''' <exception cref="UTFDataFormatException"> if read bytes do not represent a valid
		'''          modified UTF-8 encoding of a string </exception>
		Public Overridable Function readUTF() As String Implements DataInput.readUTF
			Return bin.readUTF()
		End Function

		''' <summary>
		''' Provide access to the persistent fields read from the input stream.
		''' </summary>
		Public MustInherit Class GetField

			''' <summary>
			''' Get the ObjectStreamClass that describes the fields in the stream.
			''' </summary>
			''' <returns>  the descriptor class that describes the serializable fields </returns>
			Public MustOverride ReadOnly Property objectStreamClass As ObjectStreamClass

			''' <summary>
			''' Return true if the named field is defaulted and has no value in this
			''' stream.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <returns> true, if and only if the named field is defaulted </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from
			'''         the underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
			'''         correspond to a serializable field </exception>
			Public MustOverride Function defaulted(ByVal name As String) As Boolean

			''' <summary>
			''' Get the value of the named boolean field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>boolean</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Boolean) As Boolean

			''' <summary>
			''' Get the value of the named byte field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>byte</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As SByte) As SByte

			''' <summary>
			''' Get the value of the named char field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>char</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Char) As Char

			''' <summary>
			''' Get the value of the named short field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>short</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Short) As Short

			''' <summary>
			''' Get the value of the named int field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>int</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Integer) As Integer

			''' <summary>
			''' Get the value of the named long field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>long</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Long) As Long

			''' <summary>
			''' Get the value of the named float field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>float</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Single) As Single

			''' <summary>
			''' Get the value of the named double field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>double</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Double) As Double

			''' <summary>
			''' Get the value of the named Object field from the persistent field.
			''' </summary>
			''' <param name="name"> the name of the field </param>
			''' <param name="val"> the default value to use if <code>name</code> does not
			'''         have a value </param>
			''' <returns> the value of the named <code>Object</code> field </returns>
			''' <exception cref="IOException"> if there are I/O errors while reading from the
			'''         underlying <code>InputStream</code> </exception>
			''' <exception cref="IllegalArgumentException"> if type of <code>name</code> is
			'''         not serializable or if the field type is incorrect </exception>
			Public MustOverride Function [get](ByVal name As String, ByVal val As Object) As Object
		End Class

		''' <summary>
		''' Verifies that this (possibly subclass) instance can be constructed
		''' without violating security constraints: the subclass must not override
		''' security-sensitive non-final methods, or else the
		''' "enableSubclassImplementation" SerializablePermission is checked.
		''' </summary>
		Private Sub verifySubclass()
			Dim cl As  [Class] = Me.GetType()
			If cl Is GetType(ObjectInputStream) Then Return
			Dim sm As SecurityManager = System.securityManager
			If sm Is Nothing Then Return
			processQueue(Caches.subclassAuditsQueue, Caches.subclassAudits)
			Dim key As New java.io.ObjectStreamClass.WeakClassKey(cl, Caches.subclassAuditsQueue)
			Dim result As Boolean? = Caches.subclassAudits.get(key)
			If result Is Nothing Then
				result = Convert.ToBoolean(auditSubclass(cl))
				Caches.subclassAudits.putIfAbsent(key, result)
			End If
			If result Then Return
			sm.checkPermission(SUBCLASS_IMPLEMENTATION_PERMISSION)
		End Sub

		''' <summary>
		''' Performs reflective checks on given subclass to verify that it doesn't
		''' override security-sensitive non-final methods.  Returns true if subclass
		''' is "safe", false otherwise.
		''' </summary>
		Private Shared Function auditSubclass(ByVal subcl As [Class]) As Boolean
			Dim result As Boolean? = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		   )
			Return result
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Boolean?
				Dim cl As  [Class] = subcl
				Do While cl IsNot GetType(ObjectInputStream)
					Try
						cl.getDeclaredMethod("readUnshared", CType(Nothing, Class()))
						Return  java.lang.[Boolean].FALSE
					Catch ex As NoSuchMethodException
					End Try
					Try
						cl.getDeclaredMethod("readFields", CType(Nothing, Class()))
						Return  java.lang.[Boolean].FALSE
					Catch ex As NoSuchMethodException
					End Try
					cl = cl.BaseType
				Loop
				Return  java.lang.[Boolean].TRUE
			End Function
		End Class

		''' <summary>
		''' Clears internal data structures.
		''' </summary>
		Private Sub clear()
			[handles].clear()
			vlist.clear()
		End Sub

		''' <summary>
		''' Underlying readObject implementation.
		''' </summary>
		Private Function readObject0(ByVal unshared As Boolean) As Object
			Dim oldMode As Boolean = bin.blockDataMode
			If oldMode Then
				Dim remain As Integer = bin.currentBlockRemaining()
				If remain > 0 Then
					Throw New OptionalDataException(remain)
				ElseIf defaultDataEnd Then
	'                
	'                 * Fix for 4360508: stream is currently at the end of a field
	'                 * value block written via default serialization; since there
	'                 * is no terminating TC_ENDBLOCKDATA tag, simulate
	'                 * end-of-custom-data behavior explicitly.
	'                 
					Throw New OptionalDataException(True)
				End If
				bin.blockDataMode = False
			End If

			Dim tc As SByte
			tc = bin.peekByte()
			Do While tc = TC_RESET
				bin.readByte()
				handleReset()
				tc = bin.peekByte()
			Loop

			depth += 1
			Try
				Select Case tc
					Case TC_NULL
						Return readNull()

					Case TC_REFERENCE
						Return readHandle(unshared)

					Case TC_CLASS
						Return readClass(unshared)

					Case TC_CLASSDESC, TC_PROXYCLASSDESC
						Return readClassDesc(unshared)

					Case TC_STRING, TC_LONGSTRING
						Return checkResolve(readString(unshared))

					Case TC_ARRAY
						Return checkResolve(readArray(unshared))

					Case TC_ENUM
						Return checkResolve(readEnum(unshared))

					Case TC_OBJECT
						Return checkResolve(readOrdinaryObject(unshared))

					Case TC_EXCEPTION
						Dim ex As IOException = readFatalException()
						Throw New WriteAbortedException("writing aborted", ex)

					Case TC_BLOCKDATA, TC_BLOCKDATALONG
						If oldMode Then
							bin.blockDataMode = True
							bin.peek() ' force header read
							Throw New OptionalDataException(bin.currentBlockRemaining())
						Else
							Throw New StreamCorruptedException("unexpected block data")
						End If

					Case TC_ENDBLOCKDATA
						If oldMode Then
							Throw New OptionalDataException(True)
						Else
							Throw New StreamCorruptedException("unexpected end of block data")
						End If

					Case Else
						Throw New StreamCorruptedException(String.Format("invalid type code: {0:X2}", tc))
				End Select
			Finally
				depth -= 1
				bin.blockDataMode = oldMode
			End Try
		End Function

		''' <summary>
		''' If resolveObject has been enabled and given object does not have an
		''' exception associated with it, calls resolveObject to determine
		''' replacement for object, and updates handle table accordingly.  Returns
		''' replacement object, or echoes provided object if no replacement
		''' occurred.  Expects that passHandle is set to given object's handle prior
		''' to calling this method.
		''' </summary>
		Private Function checkResolve(ByVal obj As Object) As Object
			If (Not enableResolve) OrElse [handles].lookupException(passHandle) IsNot Nothing Then Return obj
			Dim rep As Object = resolveObject(obj)
			If rep IsNot obj Then [handles].objectect(passHandle, rep)
			Return rep
		End Function

		''' <summary>
		''' Reads string without allowing it to be replaced in stream.  Called from
		''' within ObjectStreamClass.read().
		''' </summary>
		Friend Overridable Function readTypeString() As String
			Dim oldHandle As Integer = passHandle
			Try
				Dim tc As SByte = bin.peekByte()
				Select Case tc
					Case TC_NULL
						Return CStr(readNull())

					Case TC_REFERENCE
						Return CStr(readHandle(False))

					Case TC_STRING, TC_LONGSTRING
						Return readString(False)

					Case Else
						Throw New StreamCorruptedException(String.Format("invalid type code: {0:X2}", tc))
				End Select
			Finally
				passHandle = oldHandle
			End Try
		End Function

		''' <summary>
		''' Reads in null code, sets passHandle to NULL_HANDLE and returns null.
		''' </summary>
		Private Function readNull() As Object
			If bin.readByte() <> TC_NULL Then Throw New InternalError
			passHandle = NULL_HANDLE
			Return Nothing
		End Function

		''' <summary>
		''' Reads in object handle, sets passHandle to the read handle, and returns
		''' object associated with the handle.
		''' </summary>
		Private Function readHandle(ByVal unshared As Boolean) As Object
			If bin.readByte() <> TC_REFERENCE Then Throw New InternalError
			passHandle = bin.readInt() - baseWireHandle
			If passHandle < 0 OrElse passHandle >= [handles].size() Then Throw New StreamCorruptedException(String.Format("invalid handle value: {0:X8}", passHandle + baseWireHandle))
			If unshared Then Throw New InvalidObjectException("cannot read back reference as unshared")

			Dim obj As Object = [handles].lookupObject(passHandle)
			If obj Is unsharedMarker Then Throw New InvalidObjectException("cannot read back reference to unshared object")
			Return obj
		End Function

		''' <summary>
		''' Reads in and returns class object.  Sets passHandle to class object's
		''' assigned handle.  Returns null if class is unresolvable (in which case a
		''' ClassNotFoundException will be associated with the class' handle in the
		''' handle table).
		''' </summary>
		Private Function readClass(ByVal unshared As Boolean) As  [Class]
			If bin.readByte() <> TC_CLASS Then Throw New InternalError
			Dim desc As ObjectStreamClass = readClassDesc(False)
			Dim cl As  [Class] = desc.forClass()
			passHandle = [handles].assign(If(unshared, unsharedMarker, cl))

			Dim resolveEx As  ClassNotFoundException = desc.resolveException
			If resolveEx IsNot Nothing Then [handles].markException(passHandle, resolveEx)

			[handles].finish(passHandle)
			Return cl
		End Function

		''' <summary>
		''' Reads in and returns (possibly null) class descriptor.  Sets passHandle
		''' to class descriptor's assigned handle.  If class descriptor cannot be
		''' resolved to a class in the local VM, a ClassNotFoundException is
		''' associated with the class descriptor's handle.
		''' </summary>
		Private Function readClassDesc(ByVal unshared As Boolean) As ObjectStreamClass
			Dim tc As SByte = bin.peekByte()
			Select Case tc
				Case TC_NULL
					Return CType(readNull(), ObjectStreamClass)

				Case TC_REFERENCE
					Return CType(readHandle(unshared), ObjectStreamClass)

				Case TC_PROXYCLASSDESC
					Return readProxyDesc(unshared)

				Case TC_CLASSDESC
					Return readNonProxyDesc(unshared)

				Case Else
					Throw New StreamCorruptedException(String.Format("invalid type code: {0:X2}", tc))
			End Select
		End Function

		Private Property customSubclass As Boolean
			Get
				' Return true if this class is a custom subclass of ObjectInputStream
				Return Me.GetType().classLoader <> GetType(ObjectInputStream).classLoader
			End Get
		End Property

		''' <summary>
		''' Reads in and returns class descriptor for a dynamic proxy class.  Sets
		''' passHandle to proxy class descriptor's assigned handle.  If proxy class
		''' descriptor cannot be resolved to a class in the local VM, a
		''' ClassNotFoundException is associated with the descriptor's handle.
		''' </summary>
		Private Function readProxyDesc(ByVal unshared As Boolean) As ObjectStreamClass
			If bin.readByte() <> TC_PROXYCLASSDESC Then Throw New InternalError

			Dim desc As New ObjectStreamClass
			Dim descHandle As Integer = [handles].assign(If(unshared, unsharedMarker, desc))
			passHandle = NULL_HANDLE

			Dim numIfaces As Integer = bin.readInt()
			Dim ifaces As String() = New String(numIfaces - 1){}
			For i As Integer = 0 To numIfaces - 1
				ifaces(i) = bin.readUTF()
			Next i

			Dim cl As  [Class] = Nothing
			Dim resolveEx As  ClassNotFoundException = Nothing
			bin.blockDataMode = True
			Try
				cl = resolveProxyClass(ifaces)
				If cl Is Nothing Then
					resolveEx = New ClassNotFoundException("null class")
				ElseIf Not Proxy.isProxyClass(cl) Then
					Throw New InvalidClassException("Not a proxy")
				Else
					' ReflectUtil.checkProxyPackageAccess makes a test
					' equivalent to isCustomSubclass so there's no need
					' to condition this call to isCustomSubclass == true here.
					sun.reflect.misc.ReflectUtil.checkProxyPackageAccess(Me.GetType().classLoader, cl.interfaces)
				End If
			Catch ex As  ClassNotFoundException
				resolveEx = ex
			End Try
			skipCustomData()

			desc.initProxy(cl, resolveEx, readClassDesc(False))

			[handles].finish(descHandle)
			passHandle = descHandle
			Return desc
		End Function

		''' <summary>
		''' Reads in and returns class descriptor for a class that is not a dynamic
		''' proxy class.  Sets passHandle to class descriptor's assigned handle.  If
		''' class descriptor cannot be resolved to a class in the local VM, a
		''' ClassNotFoundException is associated with the descriptor's handle.
		''' </summary>
		Private Function readNonProxyDesc(ByVal unshared As Boolean) As ObjectStreamClass
			If bin.readByte() <> TC_CLASSDESC Then Throw New InternalError

			Dim desc As New ObjectStreamClass
			Dim descHandle As Integer = [handles].assign(If(unshared, unsharedMarker, desc))
			passHandle = NULL_HANDLE

			Dim readDesc As ObjectStreamClass = Nothing
			Try
				readDesc = readClassDescriptor()
			Catch ex As  ClassNotFoundException
				Throw CType((New InvalidClassException("failed to read class descriptor")).initCause(ex), IOException)
			End Try

			Dim cl As  [Class] = Nothing
			Dim resolveEx As  ClassNotFoundException = Nothing
			bin.blockDataMode = True
			Dim checksRequired As Boolean = customSubclass
			Try
				cl = resolveClass(readDesc)
				If cl Is Nothing Then
					resolveEx = New ClassNotFoundException("null class")
				ElseIf checksRequired Then
					sun.reflect.misc.ReflectUtil.checkPackageAccess(cl)
				End If
			Catch ex As  ClassNotFoundException
				resolveEx = ex
			End Try
			skipCustomData()

			desc.initNonProxy(readDesc, cl, resolveEx, readClassDesc(False))

			[handles].finish(descHandle)
			passHandle = descHandle
			Return desc
		End Function

		''' <summary>
		''' Reads in and returns new string.  Sets passHandle to new string's
		''' assigned handle.
		''' </summary>
		Private Function readString(ByVal unshared As Boolean) As String
			Dim str As String
			Dim tc As SByte = bin.readByte()
			Select Case tc
				Case TC_STRING
					str = bin.readUTF()

				Case TC_LONGSTRING
					str = bin.readLongUTF()

				Case Else
					Throw New StreamCorruptedException(String.Format("invalid type code: {0:X2}", tc))
			End Select
			passHandle = [handles].assign(If(unshared, unsharedMarker, str))
			[handles].finish(passHandle)
			Return str
		End Function

		''' <summary>
		''' Reads in and returns array object, or null if array class is
		''' unresolvable.  Sets passHandle to array's assigned handle.
		''' </summary>
		Private Function readArray(ByVal unshared As Boolean) As Object
			If bin.readByte() <> TC_ARRAY Then Throw New InternalError

			Dim desc As ObjectStreamClass = readClassDesc(False)
			Dim len As Integer = bin.readInt()

			Dim array As Object = Nothing
			Dim cl As [Class], ccl As  [Class] = Nothing
			cl = desc.forClass()
			If cl IsNot Nothing Then
				ccl = cl.componentType
				array = Array.newInstance(ccl, len)
			End If

			Dim arrayHandle As Integer = [handles].assign(If(unshared, unsharedMarker, array))
			Dim resolveEx As  ClassNotFoundException = desc.resolveException
			If resolveEx IsNot Nothing Then [handles].markException(arrayHandle, resolveEx)

			If ccl Is Nothing Then
				For i As Integer = 0 To len - 1
					readObject0(False)
				Next i
			ElseIf ccl.primitive Then
				If ccl Is  java.lang.[Integer].TYPE Then
					bin.readInts(CType(array, Integer()), 0, len)
				ElseIf ccl Is java.lang.[Byte].TYPE Then
					bin.readFully(CType(array, SByte()), 0, len, True)
				ElseIf ccl Is java.lang.[Long].TYPE Then
					bin.readLongs(CType(array, Long()), 0, len)
				ElseIf ccl Is Float.TYPE Then
					bin.readFloats(CType(array, Single()), 0, len)
				ElseIf ccl Is java.lang.[Double].TYPE Then
					bin.readDoubles(CType(array, Double()), 0, len)
				ElseIf ccl Is  java.lang.[Short].TYPE Then
					bin.readShorts(CType(array, Short()), 0, len)
				ElseIf ccl Is Character.TYPE Then
					bin.readChars(CType(array, Char()), 0, len)
				ElseIf ccl Is  java.lang.[Boolean].TYPE Then
					bin.readBooleans(CType(array, Boolean()), 0, len)
				Else
					Throw New InternalError
				End If
			Else
				Dim oa As Object() = CType(array, Object())
				For i As Integer = 0 To len - 1
					oa(i) = readObject0(False)
					[handles].markDependency(arrayHandle, passHandle)
				Next i
			End If

			[handles].finish(arrayHandle)
			passHandle = arrayHandle
			Return array
		End Function

		''' <summary>
		''' Reads in and returns enum constant, or null if enum type is
		''' unresolvable.  Sets passHandle to enum constant's assigned handle.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Function readEnum(ByVal unshared As Boolean) As System.Enum(Of ?)
			If bin.readByte() <> TC_ENUM Then Throw New InternalError

			Dim desc As ObjectStreamClass = readClassDesc(False)
			If Not desc.enum Then Throw New InvalidClassException("non-enum class: " & desc)

			Dim enumHandle As Integer = [handles].assign(If(unshared, unsharedMarker, Nothing))
			Dim resolveEx As  ClassNotFoundException = desc.resolveException
			If resolveEx IsNot Nothing Then [handles].markException(enumHandle, resolveEx)

			Dim name As String = readString(False)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim result As System.Enum(Of ?) = Nothing
			Dim cl As  [Class] = desc.forClass()
			If cl IsNot Nothing Then
				Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim en As System.Enum(Of ?) = System.Enum.valueOf(CType(cl, [Class]), name)
					result = en
				Catch ex As IllegalArgumentException
					Throw CType((New InvalidObjectException("enum constant " & name & " does not exist in " & cl)).initCause(ex), IOException)
				End Try
				If Not unshared Then [handles].objectect(enumHandle, result)
			End If

			[handles].finish(enumHandle)
			passHandle = enumHandle
			Return result
		End Function

		''' <summary>
		''' Reads and returns "ordinary" (i.e., not a String, [Class],
		''' ObjectStreamClass, array, or enum constant) object, or null if object's
		''' class is unresolvable (in which case a ClassNotFoundException will be
		''' associated with object's handle).  Sets passHandle to object's assigned
		''' handle.
		''' </summary>
		Private Function readOrdinaryObject(ByVal unshared As Boolean) As Object
			If bin.readByte() <> TC_OBJECT Then Throw New InternalError

			Dim desc As ObjectStreamClass = readClassDesc(False)
			desc.checkDeserialize()

			Dim cl As  [Class] = desc.forClass()
			If cl Is GetType(String) OrElse cl Is GetType(Class) OrElse cl Is GetType(ObjectStreamClass) Then Throw New InvalidClassException("invalid class descriptor")

			Dim obj As Object
			Try
				obj = If(desc.instantiable, desc.newInstance(), Nothing)
			Catch ex As Exception
				Throw CType((New InvalidClassException(desc.forClass().name, "unable to create instance")).initCause(ex), IOException)
			End Try

			passHandle = [handles].assign(If(unshared, unsharedMarker, obj))
			Dim resolveEx As  ClassNotFoundException = desc.resolveException
			If resolveEx IsNot Nothing Then [handles].markException(passHandle, resolveEx)

			If desc.externalizable Then
				readExternalData(CType(obj, Externalizable), desc)
			Else
				readSerialData(obj, desc)
			End If

			[handles].finish(passHandle)

			If obj IsNot Nothing AndAlso [handles].lookupException(passHandle) Is Nothing AndAlso desc.hasReadResolveMethod() Then
				Dim rep As Object = desc.invokeReadResolve(obj)
				If unshared AndAlso rep.GetType().IsArray Then rep = cloneArray(rep)
				If rep IsNot obj Then [handles].objectect(passHandle, obj = rep)
			End If

			Return obj
		End Function

		''' <summary>
		''' If obj is non-null, reads externalizable data by invoking readExternal()
		''' method of obj; otherwise, attempts to skip over externalizable data.
		''' Expects that passHandle is set to obj's handle before this method is
		''' called.
		''' </summary>
		Private Sub readExternalData(ByVal obj As Externalizable, ByVal desc As ObjectStreamClass)
			Dim oldContext As SerialCallbackContext = curContext
			If oldContext IsNot Nothing Then oldContext.check()
			curContext = Nothing
			Try
				Dim blocked As Boolean = desc.hasBlockExternalData()
				If blocked Then bin.blockDataMode = True
				If obj IsNot Nothing Then
					Try
						obj.readExternal(Me)
					Catch ex As  ClassNotFoundException
	'                    
	'                     * In most cases, the handle table has already propagated
	'                     * a CNFException to passHandle at this point; this mark
	'                     * call is included to address cases where the readExternal
	'                     * method has cons'ed and thrown a new CNFException of its
	'                     * own.
	'                     
						 [handles].markException(passHandle, ex)
					End Try
				End If
				If blocked Then skipCustomData()
			Finally
				If oldContext IsNot Nothing Then oldContext.check()
				curContext = oldContext
			End Try
	'        
	'         * At this point, if the externalizable data was not written in
	'         * block-data form and either the externalizable class doesn't exist
	'         * locally (i.e., obj == null) or readExternal() just threw a
	'         * CNFException, then the stream is probably in an inconsistent state,
	'         * since some (or all) of the externalizable data may not have been
	'         * consumed.  Since there's no "correct" action to take in this case,
	'         * we mimic the behavior of past serialization implementations and
	'         * blindly hope that the stream is in sync; if it isn't and additional
	'         * externalizable data remains in the stream, a subsequent read will
	'         * most likely throw a StreamCorruptedException.
	'         
		End Sub

		''' <summary>
		''' Reads (or attempts to skip, if obj is null or is tagged with a
		''' ClassNotFoundException) instance data for each serializable class of
		''' object in stream, from superclass to subclass.  Expects that passHandle
		''' is set to obj's handle before this method is called.
		''' </summary>
		Private Sub readSerialData(ByVal obj As Object, ByVal desc As ObjectStreamClass)
			Dim slots As ObjectStreamClass.ClassDataSlot() = desc.classDataLayout
			For i As Integer = 0 To slots.Length - 1
				Dim slotDesc As ObjectStreamClass = slots(i).desc

				If slots(i).hasData Then
					If obj Is Nothing OrElse [handles].lookupException(passHandle) IsNot Nothing Then
						defaultReadFields(Nothing, slotDesc) ' skip field values
					ElseIf slotDesc.hasReadObjectMethod() Then
						Dim oldContext As SerialCallbackContext = curContext
						If oldContext IsNot Nothing Then oldContext.check()
						Try
							curContext = New SerialCallbackContext(obj, slotDesc)

							bin.blockDataMode = True
							slotDesc.invokeReadObject(obj, Me)
						Catch ex As  ClassNotFoundException
	'                        
	'                         * In most cases, the handle table has already
	'                         * propagated a CNFException to passHandle at this
	'                         * point; this mark call is included to address cases
	'                         * where the custom readObject method has cons'ed and
	'                         * thrown a new CNFException of its own.
	'                         
							[handles].markException(passHandle, ex)
						Finally
							curContext.usedsed()
							If oldContext IsNot Nothing Then oldContext.check()
							curContext = oldContext
						End Try

	'                    
	'                     * defaultDataEnd may have been set indirectly by custom
	'                     * readObject() method when calling defaultReadObject() or
	'                     * readFields(); clear it to restore normal read behavior.
	'                     
						defaultDataEnd = False
					Else
						defaultReadFields(obj, slotDesc)
					End If

					If slotDesc.hasWriteObjectData() Then
						skipCustomData()
					Else
						bin.blockDataMode = False
					End If
				Else
					If obj IsNot Nothing AndAlso slotDesc.hasReadObjectNoDataMethod() AndAlso [handles].lookupException(passHandle) Is Nothing Then slotDesc.invokeReadObjectNoData(obj)
				End If
			Next i
		End Sub

		''' <summary>
		''' Skips over all block data and objects until TC_ENDBLOCKDATA is
		''' encountered.
		''' </summary>
		Private Sub skipCustomData()
			Dim oldHandle As Integer = passHandle
			Do
				If bin.blockDataMode Then
					bin.skipBlockData()
					bin.blockDataMode = False
				End If
				Select Case bin.peekByte()
					Case TC_BLOCKDATA, TC_BLOCKDATALONG
						bin.blockDataMode = True

					Case TC_ENDBLOCKDATA
						bin.readByte()
						passHandle = oldHandle
						Return

					Case Else
						readObject0(False)
				End Select
			Loop
		End Sub

		''' <summary>
		''' Reads in values of serializable fields declared by given class
		''' descriptor.  If obj is non-null, sets field values in obj.  Expects that
		''' passHandle is set to obj's handle before this method is called.
		''' </summary>
		Private Sub defaultReadFields(ByVal obj As Object, ByVal desc As ObjectStreamClass)
			Dim cl As  [Class] = desc.forClass()
			If cl IsNot Nothing AndAlso obj IsNot Nothing AndAlso (Not cl.isInstance(obj)) Then Throw New ClassCastException

			Dim primDataSize As Integer = desc.primDataSize
			If primVals Is Nothing OrElse primVals.Length < primDataSize Then primVals = New SByte(primDataSize - 1){}
			bin.readFully(primVals, 0, primDataSize, False)
			If obj IsNot Nothing Then desc.primFieldValuesues(obj, primVals)

			Dim objHandle As Integer = passHandle
			Dim fields As ObjectStreamField() = desc.getFields(False)
			Dim objVals As Object() = New Object(desc.numObjFields - 1){}
			Dim numPrimFields As Integer = fields.Length - objVals.Length
			For i As Integer = 0 To objVals.Length - 1
				Dim f As ObjectStreamField = fields(numPrimFields + i)
				objVals(i) = readObject0(f.unshared)
				If f.field IsNot Nothing Then [handles].markDependency(objHandle, passHandle)
			Next i
			If obj IsNot Nothing Then desc.objFieldValuesues(obj, objVals)
			passHandle = objHandle
		End Sub

		''' <summary>
		''' Reads in and returns IOException that caused serialization to abort.
		''' All stream state is discarded prior to reading in fatal exception.  Sets
		''' passHandle to fatal exception's handle.
		''' </summary>
		Private Function readFatalException() As IOException
			If bin.readByte() <> TC_EXCEPTION Then Throw New InternalError
			clear()
			Return CType(readObject0(False), IOException)
		End Function

		''' <summary>
		''' If recursion depth is 0, clears internal data structures; otherwise,
		''' throws a StreamCorruptedException.  This method is called when a
		''' TC_RESET typecode is encountered.
		''' </summary>
		Private Sub handleReset()
			If depth > 0 Then Throw New StreamCorruptedException("unexpected reset; recursion depth: " & depth)
			clear()
		End Sub

		''' <summary>
		''' Converts specified span of bytes into float values.
		''' </summary>
		' REMIND: remove once hotspot inlines Float.intBitsToFloat
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub bytesToFloats(ByVal src As SByte(), ByVal srcpos As Integer, ByVal dst As Single(), ByVal dstpos As Integer, ByVal nfloats As Integer)
		End Sub

		''' <summary>
		''' Converts specified span of bytes into double values.
		''' </summary>
		' REMIND: remove once hotspot inlines java.lang.[Double].longBitsToDouble
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub bytesToDoubles(ByVal src As SByte(), ByVal srcpos As Integer, ByVal dst As Double(), ByVal dstpos As Integer, ByVal ndoubles As Integer)
		End Sub

		''' <summary>
		''' Returns the first non-null class loader (not counting class loaders of
		''' generated reflection implementation classes) up the execution stack, or
		''' null if only code from the null class loader is on the stack.  This
		''' method is also called via reflection by the following RMI-IIOP class:
		''' 
		'''     com.sun.corba.se.internal.util.JDKClassLoader
		''' 
		''' This method should not be removed or its signature changed without
		''' corresponding modifications to the above class.
		''' </summary>
		Private Shared Function latestUserDefinedLoader() As  ClassLoader
			Return sun.misc.VM.latestUserDefinedLoader()
		End Function

		''' <summary>
		''' Default GetField implementation.
		''' </summary>
		Private Class GetFieldImpl
			Inherits GetField

			Private ReadOnly outerInstance As ObjectInputStream


			''' <summary>
			''' class descriptor describing serializable fields </summary>
			Private ReadOnly desc As ObjectStreamClass
			''' <summary>
			''' primitive field values </summary>
			Private ReadOnly primVals As SByte()
			''' <summary>
			''' object field values </summary>
			Private ReadOnly objVals As Object()
			''' <summary>
			''' object field value handles </summary>
			Private ReadOnly objHandles As Integer()

			''' <summary>
			''' Creates GetFieldImpl object for reading fields defined in given
			''' class descriptor.
			''' </summary>
			Friend Sub New(ByVal outerInstance As ObjectInputStream, ByVal desc As ObjectStreamClass)
					Me.outerInstance = outerInstance
				Me.desc = desc
				primVals = New SByte(desc.primDataSize - 1){}
				objVals = New Object(desc.numObjFields - 1){}
				objHandles = New Integer(objVals.Length - 1){}
			End Sub

			Public Property Overrides objectStreamClass As ObjectStreamClass
				Get
					Return desc
				End Get
			End Property

			Public Overrides Function defaulted(ByVal name As String) As Boolean
				Return (getFieldOffset(name, Nothing) < 0)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Boolean) As Boolean
				Dim [off] As Integer = getFieldOffset(name,  java.lang.[Boolean].TYPE)
				Return If([off] >= 0, Bits.getBoolean(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As SByte) As SByte
				Dim [off] As Integer = getFieldOffset(name, java.lang.[Byte].TYPE)
				Return If([off] >= 0, primVals([off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Char) As Char
				Dim [off] As Integer = getFieldOffset(name, Character.TYPE)
				Return If([off] >= 0, Bits.getChar(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Short) As Short
				Dim [off] As Integer = getFieldOffset(name,  java.lang.[Short].TYPE)
				Return If([off] >= 0, Bits.getShort(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Integer) As Integer
				Dim [off] As Integer = getFieldOffset(name,  java.lang.[Integer].TYPE)
				Return If([off] >= 0, Bits.getInt(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Single) As Single
				Dim [off] As Integer = getFieldOffset(name, Float.TYPE)
				Return If([off] >= 0, Bits.getFloat(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Long) As Long
				Dim [off] As Integer = getFieldOffset(name, java.lang.[Long].TYPE)
				Return If([off] >= 0, Bits.getLong(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Double) As Double
				Dim [off] As Integer = getFieldOffset(name, java.lang.[Double].TYPE)
				Return If([off] >= 0, Bits.getDouble(primVals, [off]), val)
			End Function

			Public Overrides Function [get](ByVal name As String, ByVal val As Object) As Object
				Dim [off] As Integer = getFieldOffset(name, GetType(Object))
				If [off] >= 0 Then
					Dim objHandle As Integer = objHandles([off])
					outerInstance.handles.markDependency(outerInstance.passHandle, objHandle)
					Return If(outerInstance.handles.lookupException(objHandle) Is Nothing, objVals([off]), Nothing)
				Else
					Return val
				End If
			End Function

			''' <summary>
			''' Reads primitive and object field values from stream.
			''' </summary>
			Friend Overridable Sub readFields()
				outerInstance.bin.readFully(primVals, 0, primVals.Length, False)

				Dim oldHandle As Integer = outerInstance.passHandle
				Dim fields As ObjectStreamField() = desc.getFields(False)
				Dim numPrimFields As Integer = fields.Length - objVals.Length
				For i As Integer = 0 To objVals.Length - 1
					objVals(i) = outerInstance.readObject0(fields(numPrimFields + i).unshared)
					objHandles(i) = outerInstance.passHandle
				Next i
				outerInstance.passHandle = oldHandle
			End Sub

			''' <summary>
			''' Returns offset of field with given name and type.  A specified type
			''' of null matches all types, Object.class matches all non-primitive
			''' types, and any other non-null type matches assignable types only.
			''' If no matching field is found in the (incoming) class
			''' descriptor but a matching field is present in the associated local
			''' class descriptor, returns -1.  Throws IllegalArgumentException if
			''' neither incoming nor local class descriptor contains a match.
			''' </summary>
			Private Function getFieldOffset(ByVal name As String, ByVal type As [Class]) As Integer
				Dim field As ObjectStreamField = desc.getField(name, type)
				If field IsNot Nothing Then
					Return field.offset
				ElseIf desc.localDesc.getField(name, type) IsNot Nothing Then
					Return -1
				Else
					Throw New IllegalArgumentException("no such field " & name & " with type " & type)
				End If
			End Function
		End Class

		''' <summary>
		''' Prioritized list of callbacks to be performed once object graph has been
		''' completely deserialized.
		''' </summary>
		Private Class ValidationList

			Private Class Callback
				Friend ReadOnly obj As ObjectInputValidation
				Friend ReadOnly priority As Integer
				Friend [next] As Callback
				Friend ReadOnly acc As java.security.AccessControlContext

				Friend Sub New(ByVal obj As ObjectInputValidation, ByVal priority As Integer, ByVal [next] As Callback, ByVal acc As java.security.AccessControlContext)
					Me.obj = obj
					Me.priority = priority
					Me.next = [next]
					Me.acc = acc
				End Sub
			End Class

			''' <summary>
			''' linked list of callbacks </summary>
			Private list As Callback

			''' <summary>
			''' Creates new (empty) ValidationList.
			''' </summary>
			Friend Sub New()
			End Sub

			''' <summary>
			''' Registers callback.  Throws InvalidObjectException if callback
			''' object is null.
			''' </summary>
			Friend Overridable Sub register(ByVal obj As ObjectInputValidation, ByVal priority As Integer)
				If obj Is Nothing Then Throw New InvalidObjectException("null callback")

				Dim prev As Callback = Nothing, cur As Callback = list
				Do While cur IsNot Nothing AndAlso priority < cur.priority
					prev = cur
					cur = cur.next
				Loop
				Dim acc As java.security.AccessControlContext = java.security.AccessController.context
				If prev IsNot Nothing Then
					prev.next = New Callback(obj, priority, cur, acc)
				Else
					list = New Callback(obj, priority, list, acc)
				End If
			End Sub

			''' <summary>
			''' Invokes all registered callbacks and clears the callback list.
			''' Callbacks with higher priorities are called first; those with equal
			''' priorities may be called in any order.  If any of the callbacks
			''' throws an InvalidObjectException, the callback process is terminated
			''' and the exception propagated upwards.
			''' </summary>
			Friend Overridable Sub doCallbacks()
				Try
					Do While list IsNot Nothing
						java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
						list = list.next
					Loop
				Catch ex As java.security.PrivilegedActionException
					list = Nothing
					Throw CType(ex.exception, InvalidObjectException)
				End Try
			End Sub

			Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedExceptionAction(Of T)

				Public Overridable Function run() As Void
					outerInstance.list.obj.validateObject()
					Return Nothing
				End Function
			End Class

			''' <summary>
			''' Resets the callback list to its initial (empty) state.
			''' </summary>
			Public Overridable Sub clear()
				list = Nothing
			End Sub
		End Class

		''' <summary>
		''' Input stream supporting single-byte peek operations.
		''' </summary>
		Private Class PeekInputStream
			Inherits InputStream

			''' <summary>
			''' underlying stream </summary>
			Private ReadOnly [in] As InputStream
			''' <summary>
			''' peeked byte </summary>
			Private peekb As Integer = -1

			''' <summary>
			''' Creates new PeekInputStream on top of given underlying stream.
			''' </summary>
			Friend Sub New(ByVal [in] As InputStream)
				Me.in = [in]
			End Sub

			''' <summary>
			''' Peeks at next byte value in stream.  Similar to read(), except
			''' that it does not consume the read value.
			''' </summary>
			Friend Overridable Function peek() As Integer
					If peekb >= 0 Then
						Return peekb
					Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return (peekb = [in].read())
					End If
			End Function

			Public Overrides Function read() As Integer
				If peekb >= 0 Then
					Dim v As Integer = peekb
					peekb = -1
					Return v
				Else
					Return [in].read()
				End If
			End Function

			Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
				If len = 0 Then
					Return 0
				ElseIf peekb < 0 Then
					Return [in].read(b, [off], len)
				Else
					b([off]) = CByte(peekb)
					[off] += 1
					len -= 1
					peekb = -1
					Dim n As Integer = [in].read(b, [off], len)
					Return If(n >= 0, (n + 1), 1)
				End If
			End Function

			Friend Overridable Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				Dim n As Integer = 0
				Do While n < len
					Dim count As Integer = read(b, [off] + n, len - n)
					If count < 0 Then Throw New EOFException
					n += count
				Loop
			End Sub

			Public Overrides Function skip(ByVal n As Long) As Long
				If n <= 0 Then Return 0
				Dim skipped As Integer = 0
				If peekb >= 0 Then
					peekb = -1
					skipped += 1
					n -= 1
				End If
				Return skipped + skip(n)
			End Function

			Public Overrides Function available() As Integer
				Return [in].available() + (If(peekb >= 0, 1, 0))
			End Function

			Public Overrides Sub close()
				[in].close()
			End Sub
		End Class

		''' <summary>
		''' Input stream with two modes: in default mode, inputs data written in the
		''' same format as DataOutputStream; in "block data" mode, inputs data
		''' bracketed by block data markers (see object serialization specification
		''' for details).  Buffering depends on block data mode: when in default
		''' mode, no data is buffered in advance; when in block data mode, all data
		''' for the current data block is read in at once (and buffered).
		''' </summary>
		Private Class BlockDataInputStream
			Inherits InputStream
			Implements DataInput

			Private ReadOnly outerInstance As ObjectInputStream

			''' <summary>
			''' maximum data block length </summary>
			Private Const MAX_BLOCK_SIZE As Integer = 1024
			''' <summary>
			''' maximum data block header length </summary>
			Private Const MAX_HEADER_SIZE As Integer = 5
			''' <summary>
			''' (tunable) length of char buffer (for reading strings) </summary>
			Private Const CHAR_BUF_SIZE As Integer = 256
			''' <summary>
			''' readBlockHeader() return value indicating header read may block </summary>
			Private Const HEADER_BLOCKED As Integer = -2

			''' <summary>
			''' buffer for reading general/block data </summary>
			Private ReadOnly buf As SByte() = New SByte(MAX_BLOCK_SIZE - 1){}
			''' <summary>
			''' buffer for reading block data headers </summary>
			Private ReadOnly hbuf As SByte() = New SByte(MAX_HEADER_SIZE - 1){}
			''' <summary>
			''' char buffer for fast string reads </summary>
			Private ReadOnly cbuf As Char() = New Char(CHAR_BUF_SIZE - 1){}

			''' <summary>
			''' block data mode </summary>
			Private blkmode As Boolean = False

			' block data state fields; values meaningful only when blkmode true
			''' <summary>
			''' current offset into buf </summary>
			Private pos As Integer = 0
			''' <summary>
			''' end offset of valid data in buf, or -1 if no more block data </summary>
			Private [end] As Integer = -1
			''' <summary>
			''' number of bytes in current block yet to be read from stream </summary>
			Private unread As Integer = 0

			''' <summary>
			''' underlying stream (wrapped in peekable filter stream) </summary>
			Private ReadOnly [in] As PeekInputStream
			''' <summary>
			''' loopback stream (for data reads that span data blocks) </summary>
			Private ReadOnly din As DataInputStream

			''' <summary>
			''' Creates new BlockDataInputStream on top of given underlying stream.
			''' Block data mode is turned off by default.
			''' </summary>
			Friend Sub New(ByVal outerInstance As ObjectInputStream, ByVal [in] As InputStream)
					Me.outerInstance = outerInstance
				Me.in = New PeekInputStream([in])
				din = New DataInputStream(Me)
			End Sub

			''' <summary>
			''' Sets block data mode to the given mode (true == on, false == off)
			''' and returns the previous mode value.  If the new mode is the same as
			''' the old mode, no action is taken.  Throws IllegalStateException if
			''' block data mode is being switched from on to off while unconsumed
			''' block data is still present in the stream.
			''' </summary>
			Friend Overridable Function setBlockDataMode(ByVal newmode As Boolean) As Boolean
				If blkmode = newmode Then Return blkmode
				If newmode Then
					pos = 0
					[end] = 0
					unread = 0
				ElseIf pos < [end] Then
					Throw New IllegalStateException("unread block data")
				End If
				blkmode = newmode
				Return Not blkmode
			End Function

			''' <summary>
			''' Returns true if the stream is currently in block data mode, false
			''' otherwise.
			''' </summary>
			Friend Overridable Property blockDataMode As Boolean
				Get
					Return blkmode
				End Get
			End Property

			''' <summary>
			''' If in block data mode, skips to the end of the current group of data
			''' blocks (but does not unset block data mode).  If not in block data
			''' mode, throws an IllegalStateException.
			''' </summary>
			Friend Overridable Sub skipBlockData()
				If Not blkmode Then Throw New IllegalStateException("not in block data mode")
				Do While [end] >= 0
					refill()
				Loop
			End Sub

			''' <summary>
			''' Attempts to read in the next block data header (if any).  If
			''' canBlock is false and a full header cannot be read without possibly
			''' blocking, returns HEADER_BLOCKED, else if the next element in the
			''' stream is a block data header, returns the block data length
			''' specified by the header, else returns -1.
			''' </summary>
			Private Function readBlockHeader(ByVal canBlock As Boolean) As Integer
				If outerInstance.defaultDataEnd Then Return -1
				Try
					Do
						Dim avail As Integer = If(canBlock,  java.lang.[Integer].Max_Value, [in].available())
						If avail = 0 Then Return HEADER_BLOCKED

						Dim tc As Integer = [in].peek()
						Select Case tc
							Case TC_BLOCKDATA
								If avail < 2 Then Return HEADER_BLOCKED
								[in].readFully(hbuf, 0, 2)
								Return hbuf(1) And &HFF

							Case TC_BLOCKDATALONG
								If avail < 5 Then Return HEADER_BLOCKED
								[in].readFully(hbuf, 0, 5)
								Dim len As Integer = Bits.getInt(hbuf, 1)
								If len < 0 Then Throw New StreamCorruptedException("illegal block data header length: " & len)
								Return len

	'                        
	'                         * TC_RESETs may occur in between data blocks.
	'                         * Unfortunately, this case must be parsed at a lower
	'                         * level than other typecodes, since primitive data
	'                         * reads may span data blocks separated by a TC_RESET.
	'                         
							Case TC_RESET
								[in].read()
								outerInstance.handleReset()

							Case Else
								If tc >= 0 AndAlso (tc < TC_BASE OrElse tc > TC_MAX) Then Throw New StreamCorruptedException(String.Format("invalid type code: {0:X2}", tc))
								Return -1
						End Select
					Loop
				Catch ex As EOFException
					Throw New StreamCorruptedException("unexpected EOF while reading block data header")
				End Try
			End Function

			''' <summary>
			''' Refills internal buffer buf with block data.  Any data in buf at the
			''' time of the call is considered consumed.  Sets the pos, end, and
			''' unread fields to reflect the new amount of available block data; if
			''' the next element in the stream is not a data block, sets pos and
			''' unread to 0 and end to -1.
			''' </summary>
			Private Sub refill()
				Try
					Do
						pos = 0
						If unread > 0 Then
							Dim n As Integer = [in].read(buf, 0, System.Math.Min(unread, MAX_BLOCK_SIZE))
							If n >= 0 Then
								[end] = n
								unread -= n
							Else
								Throw New StreamCorruptedException("unexpected EOF in middle of data block")
							End If
						Else
							Dim n As Integer = readBlockHeader(True)
							If n >= 0 Then
								[end] = 0
								unread = n
							Else
								[end] = -1
								unread = 0
							End If
						End If
					Loop While pos = [end]
				Catch ex As IOException
					pos = 0
					[end] = -1
					unread = 0
					Throw ex
				End Try
			End Sub

			''' <summary>
			''' If in block data mode, returns the number of unconsumed bytes
			''' remaining in the current data block.  If not in block data mode,
			''' throws an IllegalStateException.
			''' </summary>
			Friend Overridable Function currentBlockRemaining() As Integer
				If blkmode Then
					Return If([end] >= 0, ([end] - pos) + unread, 0)
				Else
					Throw New IllegalStateException
				End If
			End Function

			''' <summary>
			''' Peeks at (but does not consume) and returns the next byte value in
			''' the stream, or -1 if the end of the stream/block data (if in block
			''' data mode) has been reached.
			''' </summary>
			Friend Overridable Function peek() As Integer
				If blkmode Then
					If pos = [end] Then refill()
					Return If([end] >= 0, (buf(pos) And &HFF), -1)
				Else
					Return [in].peek()
				End If
			End Function

			''' <summary>
			''' Peeks at (but does not consume) and returns the next byte value in
			''' the stream, or throws EOFException if end of stream/block data has
			''' been reached.
			''' </summary>
			Friend Overridable Function peekByte() As SByte
				Dim val As Integer = peek()
				If val < 0 Then Throw New EOFException
				Return CByte(val)
			End Function


			' ----------------- generic input stream methods ------------------ 
	'        
	'         * The following methods are equivalent to their counterparts in
	'         * InputStream, except that they interpret data block boundaries and
	'         * read the requested data from within data blocks when in block data
	'         * mode.
	'         

			Public Overrides Function read() As Integer
				If blkmode Then
					If pos = [end] Then refill()
						Dim tempVar As Integer = pos
						pos += 1
						Return If([end] >= 0, (buf(tempVar) And &HFF), -1)
				Else
					Return [in].read()
				End If
			End Function

			Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
				Return read(b, [off], len, False)
			End Function

			Public Overrides Function skip(ByVal len As Long) As Long
				Dim remain As Long = len
				Do While remain > 0
					If blkmode Then
						If pos = [end] Then refill()
						If [end] < 0 Then Exit Do
						Dim nread As Integer = CInt(Fix (System.Math.Min(remain, [end] - pos)))
						remain -= nread
						pos += nread
					Else
						Dim nread As Integer = CInt(Fix (System.Math.Min(remain, MAX_BLOCK_SIZE)))
						nread = [in].read(buf, 0, nread)
						If nread < 0 Then Exit Do
						remain -= nread
					End If
				Loop
				Return len - remain
			End Function

			Public Overrides Function available() As Integer
				If blkmode Then
					If (pos = [end]) AndAlso (unread = 0) Then
						Dim n As Integer
						n = readBlockHeader(False)
						Do While n = 0

							n = readBlockHeader(False)
						Loop
						Select Case n
							Case HEADER_BLOCKED

							Case -1
								pos = 0
								[end] = -1

							Case Else
								pos = 0
								[end] = 0
								unread = n
						End Select
					End If
					' avoid unnecessary call to in.available() if possible
					Dim unreadAvail As Integer = If(unread > 0, System.Math.Min([in].available(), unread), 0)
					Return If([end] >= 0, ([end] - pos) + unreadAvail, 0)
				Else
					Return [in].available()
				End If
			End Function

			Public Overrides Sub close()
				If blkmode Then
					pos = 0
					[end] = -1
					unread = 0
				End If
				[in].close()
			End Sub

			''' <summary>
			''' Attempts to read len bytes into byte array b at offset off.  Returns
			''' the number of bytes read, or -1 if the end of stream/block data has
			''' been reached.  If copy is true, reads values into an intermediate
			''' buffer before copying them to b (to avoid exposing a reference to
			''' b).
			''' </summary>
			Friend Overridable Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal copy As Boolean) As Integer
				If len = 0 Then
					Return 0
				ElseIf blkmode Then
					If pos = [end] Then refill()
					If [end] < 0 Then Return -1
					Dim nread As Integer = System.Math.Min(len, [end] - pos)
					Array.Copy(buf, pos, b, [off], nread)
					pos += nread
					Return nread
				ElseIf copy Then
					Dim nread As Integer = [in].read(buf, 0, System.Math.Min(len, MAX_BLOCK_SIZE))
					If nread > 0 Then Array.Copy(buf, 0, b, [off], nread)
					Return nread
				Else
					Return [in].read(b, [off], len)
				End If
			End Function

			' ----------------- primitive data input methods ------------------ 
	'        
	'         * The following methods are equivalent to their counterparts in
	'         * DataInputStream, except that they interpret data block boundaries
	'         * and read the requested data from within data blocks when in block
	'         * data mode.
	'         

			Public Overridable Sub readFully(ByVal b As SByte()) Implements DataInput.readFully
				readFully(b, 0, b.Length, False)
			End Sub

			Public Overridable Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				readFully(b, [off], len, False)
			End Sub

			Public Overridable Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal copy As Boolean)
				Do While len > 0
					Dim n As Integer = read(b, [off], len, copy)
					If n < 0 Then Throw New EOFException
					[off] += n
					len -= n
				Loop
			End Sub

			Public Overridable Function skipBytes(ByVal n As Integer) As Integer Implements DataInput.skipBytes
				Return din.skipBytes(n)
			End Function

			Public Overridable Function readBoolean() As Boolean Implements DataInput.readBoolean
				Dim v As Integer = read()
				If v < 0 Then Throw New EOFException
				Return (v <> 0)
			End Function

			Public Overridable Function readByte() As SByte Implements DataInput.readByte
				Dim v As Integer = read()
				If v < 0 Then Throw New EOFException
				Return CByte(v)
			End Function

			Public Overridable Function readUnsignedByte() As Integer Implements DataInput.readUnsignedByte
				Dim v As Integer = read()
				If v < 0 Then Throw New EOFException
				Return v
			End Function

			Public Overridable Function readChar() As Char Implements DataInput.readChar
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 2)
				ElseIf [end] - pos < 2 Then
					Return din.readChar()
				End If
				Dim v As Char = Bits.getChar(buf, pos)
				pos += 2
				Return v
			End Function

			Public Overridable Function readShort() As Short Implements DataInput.readShort
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 2)
				ElseIf [end] - pos < 2 Then
					Return din.readShort()
				End If
				Dim v As Short = Bits.getShort(buf, pos)
				pos += 2
				Return v
			End Function

			Public Overridable Function readUnsignedShort() As Integer Implements DataInput.readUnsignedShort
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 2)
				ElseIf [end] - pos < 2 Then
					Return din.readUnsignedShort()
				End If
				Dim v As Integer = Bits.getShort(buf, pos) And &HFFFF
				pos += 2
				Return v
			End Function

			Public Overridable Function readInt() As Integer Implements DataInput.readInt
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 4)
				ElseIf [end] - pos < 4 Then
					Return din.readInt()
				End If
				Dim v As Integer = Bits.getInt(buf, pos)
				pos += 4
				Return v
			End Function

			Public Overridable Function readFloat() As Single Implements DataInput.readFloat
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 4)
				ElseIf [end] - pos < 4 Then
					Return din.readFloat()
				End If
				Dim v As Single = Bits.getFloat(buf, pos)
				pos += 4
				Return v
			End Function

			Public Overridable Function readLong() As Long Implements DataInput.readLong
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 8)
				ElseIf [end] - pos < 8 Then
					Return din.readLong()
				End If
				Dim v As Long = Bits.getLong(buf, pos)
				pos += 8
				Return v
			End Function

			Public Overridable Function readDouble() As Double Implements DataInput.readDouble
				If Not blkmode Then
					pos = 0
					[in].readFully(buf, 0, 8)
				ElseIf [end] - pos < 8 Then
					Return din.readDouble()
				End If
				Dim v As Double = Bits.getDouble(buf, pos)
				pos += 8
				Return v
			End Function

			Public Overridable Function readUTF() As String Implements DataInput.readUTF
				Return readUTFBody(readUnsignedShort())
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function readLine() As String Implements DataInput.readLine
				Return din.readLine() ' deprecated, not worth optimizing
			End Function

			' -------------- primitive data array input methods --------------- 
	'        
	'         * The following methods read in spans of primitive data values.
	'         * Though equivalent to calling the corresponding primitive read
	'         * methods repeatedly, these methods are optimized for reading groups
	'         * of primitive data values more efficiently.
	'         

			Friend Overridable Sub readBooleans(ByVal v As Boolean(), ByVal [off] As Integer, ByVal len As Integer)
				Dim [stop] As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						Dim span As Integer = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE)
						[in].readFully(buf, 0, span)
						[stop] = [off] + span
						pos = 0
					ElseIf [end] - pos < 1 Then
						v([off]) = din.readBoolean()
						[off] += 1
						Continue Do
					Else
						[stop] = System.Math.Min(endoff, [off] + [end] - pos)
					End If

					Do While [off] < [stop]
						v([off]) = Bits.getBoolean(buf, pos)
						pos += 1
						[off] += 1
					Loop
				Loop
			End Sub

			Friend Overridable Sub readChars(ByVal v As Char(), ByVal [off] As Integer, ByVal len As Integer)
				Dim [stop] As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						Dim span As Integer = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 1)
						[in].readFully(buf, 0, span << 1)
						[stop] = [off] + span
						pos = 0
					ElseIf [end] - pos < 2 Then
						v([off]) = din.readChar()
						[off] += 1
						Continue Do
					Else
						[stop] = System.Math.Min(endoff, [off] + (([end] - pos) >> 1))
					End If

					Do While [off] < [stop]
						v([off]) = Bits.getChar(buf, pos)
						[off] += 1
						pos += 2
					Loop
				Loop
			End Sub

			Friend Overridable Sub readShorts(ByVal v As Short(), ByVal [off] As Integer, ByVal len As Integer)
				Dim [stop] As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						Dim span As Integer = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 1)
						[in].readFully(buf, 0, span << 1)
						[stop] = [off] + span
						pos = 0
					ElseIf [end] - pos < 2 Then
						v([off]) = din.readShort()
						[off] += 1
						Continue Do
					Else
						[stop] = System.Math.Min(endoff, [off] + (([end] - pos) >> 1))
					End If

					Do While [off] < [stop]
						v([off]) = Bits.getShort(buf, pos)
						[off] += 1
						pos += 2
					Loop
				Loop
			End Sub

			Friend Overridable Sub readInts(ByVal v As Integer(), ByVal [off] As Integer, ByVal len As Integer)
				Dim [stop] As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						Dim span As Integer = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 2)
						[in].readFully(buf, 0, span << 2)
						[stop] = [off] + span
						pos = 0
					ElseIf [end] - pos < 4 Then
						v([off]) = din.readInt()
						[off] += 1
						Continue Do
					Else
						[stop] = System.Math.Min(endoff, [off] + (([end] - pos) >> 2))
					End If

					Do While [off] < [stop]
						v([off]) = Bits.getInt(buf, pos)
						[off] += 1
						pos += 4
					Loop
				Loop
			End Sub

			Friend Overridable Sub readFloats(ByVal v As Single(), ByVal [off] As Integer, ByVal len As Integer)
				Dim span As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						span = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 2)
						[in].readFully(buf, 0, span << 2)
						pos = 0
					ElseIf [end] - pos < 4 Then
						v([off]) = din.readFloat()
						[off] += 1
						Continue Do
					Else
						span = System.Math.Min(endoff - [off], (([end] - pos) >> 2))
					End If

					bytesToFloats(buf, pos, v, [off], span)
					[off] += span
					pos += span << 2
				Loop
			End Sub

			Friend Overridable Sub readLongs(ByVal v As Long(), ByVal [off] As Integer, ByVal len As Integer)
				Dim [stop] As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						Dim span As Integer = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 3)
						[in].readFully(buf, 0, span << 3)
						[stop] = [off] + span
						pos = 0
					ElseIf [end] - pos < 8 Then
						v([off]) = din.readLong()
						[off] += 1
						Continue Do
					Else
						[stop] = System.Math.Min(endoff, [off] + (([end] - pos) >> 3))
					End If

					Do While [off] < [stop]
						v([off]) = Bits.getLong(buf, pos)
						[off] += 1
						pos += 8
					Loop
				Loop
			End Sub

			Friend Overridable Sub readDoubles(ByVal v As Double(), ByVal [off] As Integer, ByVal len As Integer)
				Dim span As Integer, endoff As Integer = [off] + len
				Do While [off] < endoff
					If Not blkmode Then
						span = System.Math.Min(endoff - [off], MAX_BLOCK_SIZE >> 3)
						[in].readFully(buf, 0, span << 3)
						pos = 0
					ElseIf [end] - pos < 8 Then
						v([off]) = din.readDouble()
						[off] += 1
						Continue Do
					Else
						span = System.Math.Min(endoff - [off], (([end] - pos) >> 3))
					End If

					bytesToDoubles(buf, pos, v, [off], span)
					[off] += span
					pos += span << 3
				Loop
			End Sub

			''' <summary>
			''' Reads in string written in "long" UTF format.  "Long" UTF format is
			''' identical to standard UTF, except that it uses an 8 byte header
			''' (instead of the standard 2 bytes) to convey the UTF encoding length.
			''' </summary>
			Friend Overridable Function readLongUTF() As String
				Return readUTFBody(readLong())
			End Function

			''' <summary>
			''' Reads in the "body" (i.e., the UTF representation minus the 2-byte
			''' or 8-byte length header) of a UTF encoding, which occupies the next
			''' utflen bytes.
			''' </summary>
			Private Function readUTFBody(ByVal utflen As Long) As String
				Dim sbuf As New StringBuilder
				If Not blkmode Then
						pos = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						= pos
				End If

				Do While utflen > 0
					Dim avail As Integer = [end] - pos
					If avail >= 3 OrElse CLng(avail) = utflen Then
						utflen -= readUTFSpan(sbuf, utflen)
					Else
						If blkmode Then
							' near block boundary, read one byte at a time
							utflen -= readUTFChar(sbuf, utflen)
						Else
							' shift and refill buffer manually
							If avail > 0 Then Array.Copy(buf, pos, buf, 0, avail)
							pos = 0
							[end] = CInt(Fix (System.Math.Min(MAX_BLOCK_SIZE, utflen)))
							[in].readFully(buf, avail, [end] - avail)
						End If
					End If
				Loop

				Return sbuf.ToString()
			End Function

			''' <summary>
			''' Reads span of UTF-encoded characters out of internal buffer
			''' (starting at offset pos and ending at or before offset end),
			''' consuming no more than utflen bytes.  Appends read characters to
			''' sbuf.  Returns the number of bytes consumed.
			''' </summary>
			Private Function readUTFSpan(ByVal sbuf As StringBuilder, ByVal utflen As Long) As Long
				Dim cpos As Integer = 0
				Dim start As Integer = pos
				Dim avail As Integer = System.Math.Min([end] - pos, CHAR_BUF_SIZE)
				' stop short of last char unless all of utf bytes in buffer
				Dim [stop] As Integer = pos + (If(utflen > avail, avail - 2, CInt(utflen)))
				Dim outOfBounds As Boolean = False

				Try
					Do While pos < [stop]
						Dim b1, b2, b3 As Integer
						b1 = buf(pos) And &HFF
						pos += 1
						Select Case b1 >> 4
							Case 0, 1, 2, 3, 4, 5, 6, 7
								cbuf(cpos) = ChrW(b1)
								cpos += 1

							Case 12, 13
								b2 = buf(pos)
								pos += 1
								If (b2 And &HC0) <> &H80 Then Throw New UTFDataFormatException
								cbuf(cpos) = CChar(((b1 And &H1F) << 6) Or ((b2 And &H3F) << 0))
								cpos += 1

							Case 14 ' 3 byte format: 1110xxxx 10xxxxxx 10xxxxxx
								b3 = buf(pos + 1)
								b2 = buf(pos + 0)
								pos += 2
								If (b2 And &HC0) <> &H80 OrElse (b3 And &HC0) <> &H80 Then Throw New UTFDataFormatException
								cbuf(cpos) = CChar(((b1 And &HF) << 12) Or ((b2 And &H3F) << 6) Or ((b3 And &H3F) << 0))
								cpos += 1

							Case Else ' 10xx xxxx, 1111 xxxx
								Throw New UTFDataFormatException
						End Select
					Loop
				Catch ex As ArrayIndexOutOfBoundsException
					outOfBounds = True
				Finally
					If outOfBounds OrElse (pos - start) > utflen Then
	'                    
	'                     * Fix for 4450867: if a malformed utf char causes the
	'                     * conversion loop to scan past the expected end of the utf
	'                     * string, only consume the expected number of utf bytes.
	'                     
						pos = start + CInt(utflen)
						Throw New UTFDataFormatException
					End If
				End Try

				sbuf.append(cbuf, 0, cpos)
				Return pos - start
			End Function

			''' <summary>
			''' Reads in single UTF-encoded character one byte at a time, appends
			''' the character to sbuf, and returns the number of bytes consumed.
			''' This method is used when reading in UTF strings written in block
			''' data mode to handle UTF-encoded characters which (potentially)
			''' straddle block-data boundaries.
			''' </summary>
			Private Function readUTFChar(ByVal sbuf As StringBuilder, ByVal utflen As Long) As Integer
				Dim b1, b2, b3 As Integer
				b1 = readByte() And &HFF
				Select Case b1 >> 4
					Case 0, 1, 2, 3, 4, 5, 6, 7
						sbuf.append(ChrW(b1))
						Return 1

					Case 12, 13
						If utflen < 2 Then Throw New UTFDataFormatException
						b2 = readByte()
						If (b2 And &HC0) <> &H80 Then Throw New UTFDataFormatException
						sbuf.append(CChar(((b1 And &H1F) << 6) Or ((b2 And &H3F) << 0)))
						Return 2

					Case 14 ' 3 byte format: 1110xxxx 10xxxxxx 10xxxxxx
						If utflen < 3 Then
							If utflen = 2 Then readByte() ' consume remaining byte
							Throw New UTFDataFormatException
						End If
						b2 = readByte()
						b3 = readByte()
						If (b2 And &HC0) <> &H80 OrElse (b3 And &HC0) <> &H80 Then Throw New UTFDataFormatException
						sbuf.append(CChar(((b1 And &HF) << 12) Or ((b2 And &H3F) << 6) Or ((b3 And &H3F) << 0)))
						Return 3

					Case Else ' 10xx xxxx, 1111 xxxx
						Throw New UTFDataFormatException
				End Select
			End Function
		End Class

		''' <summary>
		''' Unsynchronized table which tracks wire handle to object mappings, as
		''' well As  ClassNotFoundExceptions associated with deserialized objects.
		''' This class implements an exception-propagation algorithm for
		''' determining which objects should have ClassNotFoundExceptions associated
		''' with them, taking into account cycles and discontinuities (e.g., skipped
		''' fields) in the object graph.
		''' 
		''' <p>General use of the table is as follows: during deserialization, a
		''' given object is first assigned a handle by calling the assign method.
		''' This method leaves the assigned handle in an "open" state, wherein
		''' dependencies on the exception status of other handles can be registered
		''' by calling the markDependency method, or an exception can be directly
		''' associated with the handle by calling markException.  When a handle is
		''' tagged with an exception, the HandleTable assumes responsibility for
		''' propagating the exception to any other objects which depend
		''' (transitively) on the exception-tagged object.
		''' 
		''' <p>Once all exception information/dependencies for the handle have been
		''' registered, the handle should be "closed" by calling the finish method
		''' on it.  The act of finishing a handle allows the exception propagation
		''' algorithm to aggressively prune dependency links, lessening the
		''' performance/memory impact of exception tracking.
		''' 
		''' <p>Note that the exception propagation algorithm used depends on handles
		''' being assigned/finished in LIFO order; however, for simplicity as well
		''' as memory conservation, it does not enforce this constraint.
		''' </summary>
		' REMIND: add full description of exception propagation algorithm?
		Private Class HandleTable

			' status codes indicating whether object has associated exception 
			Private Const STATUS_OK As SByte = 1
			Private Const STATUS_UNKNOWN As SByte = 2
			Private Const STATUS_EXCEPTION As SByte = 3

			''' <summary>
			''' array mapping handle -> object status </summary>
			Friend status As SByte()
			''' <summary>
			''' array mapping handle -> object/exception (depending on status) </summary>
			Friend entries As Object()
			''' <summary>
			''' array mapping handle -> list of dependent handles (if any) </summary>
			Friend deps As HandleList()
			''' <summary>
			''' lowest unresolved dependency </summary>
			Friend lowDep As Integer = -1
			''' <summary>
			''' number of handles in table </summary>
			Friend size_Renamed As Integer = 0

			''' <summary>
			''' Creates handle table with the given initial capacity.
			''' </summary>
			Friend Sub New(ByVal initialCapacity As Integer)
				status = New SByte(initialCapacity - 1){}
				entries = New Object(initialCapacity - 1){}
				deps = New HandleList(initialCapacity - 1){}
			End Sub

			''' <summary>
			''' Assigns next available handle to given object, and returns assigned
			''' handle.  Once object has been completely deserialized (and all
			''' dependencies on other objects identified), the handle should be
			''' "closed" by passing it to finish().
			''' </summary>
			Friend Overridable Function assign(ByVal obj As Object) As Integer
				If size_Renamed >= entries.Length Then grow()
				status(size_Renamed) = STATUS_UNKNOWN
				entries(size_Renamed) = obj
					Dim tempVar As Integer = size_Renamed
					size_Renamed += 1
					Return tempVar
			End Function

			''' <summary>
			''' Registers a dependency (in exception status) of one handle on
			''' another.  The dependent handle must be "open" (i.e., assigned, but
			''' not finished yet).  No action is taken if either dependent or target
			''' handle is NULL_HANDLE.
			''' </summary>
			Friend Overridable Sub markDependency(ByVal dependent As Integer, ByVal target As Integer)
				If dependent = NULL_HANDLE OrElse target = NULL_HANDLE Then Return
				Select Case status(dependent)

					Case STATUS_UNKNOWN
						Select Case status(target)
							Case STATUS_OK
								' ignore dependencies on objs with no exception

							Case STATUS_EXCEPTION
								' eagerly propagate exception
								markException(dependent, CType(entries(target), ClassNotFoundException))

							Case STATUS_UNKNOWN
								' add to dependency list of target
								If deps(target) Is Nothing Then deps(target) = New HandleList
								deps(target).add(dependent)

								' remember lowest unresolved target seen
								If lowDep < 0 OrElse lowDep > target Then lowDep = target

							Case Else
								Throw New InternalError
						End Select

					Case STATUS_EXCEPTION

					Case Else
						Throw New InternalError
				End Select
			End Sub

			''' <summary>
			''' Associates a ClassNotFoundException (if one not already associated)
			''' with the currently active handle and propagates it to other
			''' referencing objects as appropriate.  The specified handle must be
			''' "open" (i.e., assigned, but not finished yet).
			''' </summary>
			Friend Overridable Sub markException(ByVal handle As Integer, ByVal ex As  ClassNotFoundException)
				Select Case status(handle)
					Case STATUS_UNKNOWN
						status(handle) = STATUS_EXCEPTION
						entries(handle) = ex

						' propagate exception to dependents
						Dim dlist As HandleList = deps(handle)
						If dlist IsNot Nothing Then
							Dim ndeps As Integer = dlist.size()
							For i As Integer = 0 To ndeps - 1
								markException(dlist.get(i), ex)
							Next i
							deps(handle) = Nothing
						End If

					Case STATUS_EXCEPTION

					Case Else
						Throw New InternalError
				End Select
			End Sub

			''' <summary>
			''' Marks given handle as finished, meaning that no new dependencies
			''' will be marked for handle.  Calls to the assign and finish methods
			''' must occur in LIFO order.
			''' </summary>
			Friend Overridable Sub finish(ByVal handle As Integer)
				Dim [end] As Integer
				If lowDep < 0 Then
					' no pending unknowns, only resolve current handle
					[end] = handle + 1
				ElseIf lowDep >= handle Then
					' pending unknowns now clearable, resolve all upward handles
					[end] = size_Renamed
					lowDep = -1
				Else
					' unresolved backrefs present, can't resolve anything yet
					Return
				End If

				' change STATUS_UNKNOWN -> STATUS_OK in selected span of handles
				For i As Integer = handle To [end] - 1
					Select Case status(i)
						Case STATUS_UNKNOWN
							status(i) = STATUS_OK
							deps(i) = Nothing

						Case STATUS_OK, STATUS_EXCEPTION

						Case Else
							Throw New InternalError
					End Select
				Next i
			End Sub

			''' <summary>
			''' Assigns a new object to the given handle.  The object previously
			''' associated with the handle is forgotten.  This method has no effect
			''' if the given handle already has an exception associated with it.
			''' This method may be called at any time after the handle is assigned.
			''' </summary>
			Friend Overridable Sub setObject(ByVal handle As Integer, ByVal obj As Object)
				Select Case status(handle)
					Case STATUS_UNKNOWN, STATUS_OK
						entries(handle) = obj

					Case STATUS_EXCEPTION

					Case Else
						Throw New InternalError
				End Select
			End Sub

			''' <summary>
			''' Looks up and returns object associated with the given handle.
			''' Returns null if the given handle is NULL_HANDLE, or if it has an
			''' associated ClassNotFoundException.
			''' </summary>
			Friend Overridable Function lookupObject(ByVal handle As Integer) As Object
				Return If(handle <> NULL_HANDLE AndAlso status(handle) <> STATUS_EXCEPTION, entries(handle), Nothing)
			End Function

			''' <summary>
			''' Looks up and returns ClassNotFoundException associated with the
			''' given handle.  Returns null if the given handle is NULL_HANDLE, or
			''' if there is no ClassNotFoundException associated with the handle.
			''' </summary>
			Friend Overridable Function lookupException(ByVal handle As Integer) As  ClassNotFoundException
				Return If(handle <> NULL_HANDLE AndAlso status(handle) = STATUS_EXCEPTION, CType(entries(handle), ClassNotFoundException), Nothing)
			End Function

			''' <summary>
			''' Resets table to its initial state.
			''' </summary>
			Friend Overridable Sub clear()
				java.util.Arrays.fill(status, 0, size_Renamed, CByte(0))
				java.util.Arrays.fill(entries, 0, size_Renamed, Nothing)
				java.util.Arrays.fill(deps, 0, size_Renamed, Nothing)
				lowDep = -1
				size_Renamed = 0
			End Sub

			''' <summary>
			''' Returns number of handles registered in table.
			''' </summary>
			Friend Overridable Function size() As Integer
				Return size_Renamed
			End Function

			''' <summary>
			''' Expands capacity of internal arrays.
			''' </summary>
			Private Sub grow()
				Dim newCapacity As Integer = (entries.Length << 1) + 1

				Dim newStatus As SByte() = New SByte(newCapacity - 1){}
				Dim newEntries As Object() = New Object(newCapacity - 1){}
				Dim newDeps As HandleList() = New HandleList(newCapacity - 1){}

				Array.Copy(status, 0, newStatus, 0, size_Renamed)
				Array.Copy(entries, 0, newEntries, 0, size_Renamed)
				Array.Copy(deps, 0, newDeps, 0, size_Renamed)

				status = newStatus
				entries = newEntries
				deps = newDeps
			End Sub

			''' <summary>
			''' Simple growable list of (integer) handles.
			''' </summary>
			Private Class HandleList
				Private list As Integer() = New Integer(3){}
				Private size_Renamed As Integer = 0

				Public Sub New()
				End Sub

				Public Overridable Sub add(ByVal handle As Integer)
					If size_Renamed >= list.Length Then
						Dim newList As Integer() = New Integer(list.Length << 1 - 1){}
						Array.Copy(list, 0, newList, 0, list.Length)
						list = newList
					End If
					list(size_Renamed) = handle
					size_Renamed += 1
				End Sub

				Public Overridable Function [get](ByVal index As Integer) As Integer
					If index >= size_Renamed Then Throw New ArrayIndexOutOfBoundsException
					Return list(index)
				End Function

				Public Overridable Function size() As Integer
					Return size_Renamed
				End Function
			End Class
		End Class

		''' <summary>
		''' Method for cloning arrays in case of using unsharing reading
		''' </summary>
		Private Shared Function cloneArray(ByVal array As Object) As Object
			If TypeOf array Is Object() Then
				Return CType(array, Object()).clone()
			ElseIf TypeOf array Is Boolean() Then
				Return CType(array, Boolean()).clone()
			ElseIf TypeOf array Is SByte() Then
				Return CType(array, SByte()).clone()
			ElseIf TypeOf array Is Char() Then
				Return CType(array, Char()).clone()
			ElseIf TypeOf array Is Double() Then
				Return CType(array, Double()).clone()
			ElseIf TypeOf array Is Single() Then
				Return CType(array, Single()).clone()
			ElseIf TypeOf array Is Integer() Then
				Return CType(array, Integer()).clone()
			ElseIf TypeOf array Is Long() Then
				Return CType(array, Long()).clone()
			ElseIf TypeOf array Is Short() Then
				Return CType(array, Short()).clone()
			Else
				Throw New AssertionError
			End If
		End Function

	End Class

End Namespace