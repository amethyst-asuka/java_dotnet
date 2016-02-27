Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Runtime.InteropServices
Imports java.lang

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
	''' An ObjectOutputStream writes primitive data types and graphs of Java objects
	''' to an OutputStream.  The objects can be read (reconstituted) using an
	''' ObjectInputStream.  Persistent storage of objects can be accomplished by
	''' using a file for the stream.  If the stream is a network socket stream, the
	''' objects can be reconstituted on another host or in another process.
	''' 
	''' <p>Only objects that support the java.io.Serializable interface can be
	''' written to streams.  The class of each serializable object is encoded
	''' including the class name and signature of the [Class], the values of the
	''' object's fields and arrays, and the closure of any other objects referenced
	''' from the initial objects.
	''' 
	''' <p>The method writeObject is used to write an object to the stream.  Any
	''' object, including Strings and arrays, is written with writeObject. Multiple
	''' objects or primitives can be written to the stream.  The objects must be
	''' read back from the corresponding ObjectInputstream with the same types and
	''' in the same order as they were written.
	''' 
	''' <p>Primitive data types can also be written to the stream using the
	''' appropriate methods from DataOutput. Strings can also be written using the
	''' writeUTF method.
	''' 
	''' <p>The default serialization mechanism for an object writes the class of the
	''' object, the class signature, and the values of all non-transient and
	''' non-static fields.  References to other objects (except in transient or
	''' static fields) cause those objects to be written also. Multiple references
	''' to a single object are encoded using a reference sharing mechanism so that
	''' graphs of objects can be restored to the same shape as when the original was
	''' written.
	''' 
	''' <p>For example to write an object that can be read by the example in
	''' ObjectInputStream:
	''' <br>
	''' <pre>
	'''      FileOutputStream fos = new FileOutputStream("t.tmp");
	'''      ObjectOutputStream oos = new ObjectOutputStream(fos);
	''' 
	'''      oos.writeInt(12345);
	'''      oos.writeObject("Today");
	'''      oos.writeObject(new Date());
	''' 
	'''      oos.close();
	''' </pre>
	''' 
	''' <p>Classes that require special handling during the serialization and
	''' deserialization process must implement special methods with these exact
	''' signatures:
	''' <br>
	''' <pre>
	''' private void readObject(java.io.ObjectInputStream stream)
	'''     throws IOException, ClassNotFoundException;
	''' private void writeObject(java.io.ObjectOutputStream stream)
	'''     throws IOException
	''' private void readObjectNoData()
	'''     throws ObjectStreamException;
	''' </pre>
	''' 
	''' <p>The writeObject method is responsible for writing the state of the object
	''' for its particular class so that the corresponding readObject method can
	''' restore it.  The method does not need to concern itself with the state
	''' belonging to the object's superclasses or subclasses.  State is saved by
	''' writing the individual fields to the ObjectOutputStream using the
	''' writeObject method or by using the methods for primitive data types
	''' supported by DataOutput.
	''' 
	''' <p>Serialization does not write out the fields of any object that does not
	''' implement the java.io.Serializable interface.  Subclasses of Objects that
	''' are not serializable can be serializable. In this case the non-serializable
	''' class must have a no-arg constructor to allow its fields to be initialized.
	''' In this case it is the responsibility of the subclass to save and restore
	''' the state of the non-serializable class. It is frequently the case that the
	''' fields of that class are accessible (public, package, or protected) or that
	''' there are get and set methods that can be used to restore the state.
	''' 
	''' <p>Serialization of an object can be prevented by implementing writeObject
	''' and readObject methods that throw the NotSerializableException.  The
	''' exception will be caught by the ObjectOutputStream and abort the
	''' serialization process.
	''' 
	''' <p>Implementing the Externalizable interface allows the object to assume
	''' complete control over the contents and format of the object's serialized
	''' form.  The methods of the Externalizable interface, writeExternal and
	''' readExternal, are called to save and restore the objects state.  When
	''' implemented by a class they can write and read their own state using all of
	''' the methods of ObjectOutput and ObjectInput.  It is the responsibility of
	''' the objects to handle any versioning that occurs.
	''' 
	''' <p>Enum constants are serialized differently than ordinary serializable or
	''' externalizable objects.  The serialized form of an enum constant consists
	''' solely of its name; field values of the constant are not transmitted.  To
	''' serialize an enum constant, ObjectOutputStream writes the string returned by
	''' the constant's name method.  Like other serializable or externalizable
	''' objects, enum constants can function as the targets of back references
	''' appearing subsequently in the serialization stream.  The process by which
	''' enum constants are serialized cannot be customized; any class-specific
	''' writeObject and writeReplace methods defined by enum types are ignored
	''' during serialization.  Similarly, any serialPersistentFields or
	''' serialVersionUID field declarations are also ignored--all enum types have a
	''' fixed serialVersionUID of 0L.
	''' 
	''' <p>Primitive data, excluding serializable fields and externalizable data, is
	''' written to the ObjectOutputStream in block-data records. A block data record
	''' is composed of a header and data. The block data header consists of a marker
	''' and the number of bytes to follow the header.  Consecutive primitive data
	''' writes are merged into one block-data record.  The blocking factor used for
	''' a block-data record will be 1024 bytes.  Each block-data record will be
	''' filled up to 1024 bytes, or be written whenever there is a termination of
	''' block-data mode.  Calls to the ObjectOutputStream methods writeObject,
	''' defaultWriteObject and writeFields initially terminate any existing
	''' block-data record.
	''' 
	''' @author      Mike Warres
	''' @author      Roger Riggs </summary>
	''' <seealso cref= java.io.DataOutput </seealso>
	''' <seealso cref= java.io.ObjectInputStream </seealso>
	''' <seealso cref= java.io.Serializable </seealso>
	''' <seealso cref= java.io.Externalizable </seealso>
	''' <seealso cref= <a href="../../../platform/serialization/spec/output.html">Object Serialization Specification, Section 2, Object Output Classes</a>
	''' @since       JDK1.1 </seealso>
	Public Class ObjectOutputStream
		Inherits OutputStream
		Implements ObjectOutput, ObjectStreamConstants

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
		Private ReadOnly bout As BlockDataOutputStream
		''' <summary>
		''' obj -> wire handle map </summary>
		Private ReadOnly [handles] As HandleTable
		''' <summary>
		''' obj -> replacement obj map </summary>
		Private ReadOnly subs As ReplaceTable
		''' <summary>
		''' stream protocol version </summary>
		Private protocol As Integer = PROTOCOL_VERSION_2
		''' <summary>
		''' recursion depth </summary>
		Private depth As Integer

		''' <summary>
		''' buffer for writing primitive field values </summary>
		Private primVals As SByte()

		''' <summary>
		''' if true, invoke writeObjectOverride() instead of writeObject() </summary>
		Private ReadOnly enableOverride As Boolean
		''' <summary>
		''' if true, invoke replaceObject() </summary>
		Private enableReplace As Boolean

		' values below valid only during upcalls to writeObject()/writeExternal()
		''' <summary>
		''' Context during upcalls to class-defined writeObject methods; holds
		''' object currently being serialized and descriptor for current class.
		''' Null when not during writeObject upcall.
		''' </summary>
		Private curContext As java.io.SerialCallbackContext
		''' <summary>
		''' current PutField object </summary>
		Private curPut As PutFieldImpl

		''' <summary>
		''' custom storage for debug trace info </summary>
		Private ReadOnly debugInfoStack As DebugTraceInfoStack

		''' <summary>
		''' value of "sun.io.serialization.extendedDebugInfo" property,
		''' as true or false for extended information about exception's place
		''' </summary>
		Private Shared ReadOnly extendedDebugInfo As Boolean = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("sun.io.serialization.extendedDebugInfo"))

		''' <summary>
		''' Creates an ObjectOutputStream that writes to the specified OutputStream.
		''' This constructor writes the serialization stream header to the
		''' underlying stream; callers may wish to flush the stream immediately to
		''' ensure that constructors for receiving ObjectInputStreams will not block
		''' when reading the header.
		''' 
		''' <p>If a security manager is installed, this constructor will check for
		''' the "enableSubclassImplementation" SerializablePermission when invoked
		''' directly or indirectly by the constructor of a subclass which overrides
		''' the ObjectOutputStream.putFields or ObjectOutputStream.writeUnshared
		''' methods.
		''' </summary>
		''' <param name="out"> output stream to write to </param>
		''' <exception cref="IOException"> if an I/O error occurs while writing stream header </exception>
		''' <exception cref="SecurityException"> if untrusted subclass illegally overrides
		'''          security-sensitive methods </exception>
		''' <exception cref="NullPointerException"> if <code>out</code> is <code>null</code>
		''' @since   1.4 </exception>
		''' <seealso cref=     ObjectOutputStream#ObjectOutputStream() </seealso>
		''' <seealso cref=     ObjectOutputStream#putFields() </seealso>
		''' <seealso cref=     ObjectInputStream#ObjectInputStream(InputStream) </seealso>
		Public Sub New(ByVal out As OutputStream)
			verifySubclass()
			bout = New BlockDataOutputStream(out)
			[handles] = New HandleTable(10, CSng(3.00))
			subs = New ReplaceTable(10, CSng(3.00))
			enableOverride = False
			writeStreamHeader()
			bout.blockDataMode = True
			If extendedDebugInfo Then
				debugInfoStack = New DebugTraceInfoStack
			Else
				debugInfoStack = Nothing
			End If
		End Sub

		''' <summary>
		''' Provide a way for subclasses that are completely reimplementing
		''' ObjectOutputStream to not have to allocate private data just used by
		''' this implementation of ObjectOutputStream.
		''' 
		''' <p>If there is a security manager installed, this method first calls the
		''' security manager's <code>checkPermission</code> method with a
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
			bout = Nothing
			[handles] = Nothing
			subs = Nothing
			enableOverride = True
			debugInfoStack = Nothing
		End Sub

		''' <summary>
		''' Specify stream protocol version to use when writing the stream.
		''' 
		''' <p>This routine provides a hook to enable the current version of
		''' Serialization to write in a format that is backwards compatible to a
		''' previous version of the stream format.
		''' 
		''' <p>Every effort will be made to avoid introducing additional
		''' backwards incompatibilities; however, sometimes there is no
		''' other alternative.
		''' </summary>
		''' <param name="version"> use ProtocolVersion from java.io.ObjectStreamConstants. </param>
		''' <exception cref="IllegalStateException"> if called after any objects
		'''          have been serialized. </exception>
		''' <exception cref="IllegalArgumentException"> if invalid version is passed in. </exception>
		''' <exception cref="IOException"> if I/O errors occur </exception>
		''' <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_1 </seealso>
		''' <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_2
		''' @since   1.2 </seealso>
		Public Overridable Sub useProtocolVersion(ByVal version As Integer)
			If [handles].size() <> 0 Then Throw New IllegalStateException("stream non-empty")
			Select Case version
				Case PROTOCOL_VERSION_1, PROTOCOL_VERSION_2
					protocol = version

				Case Else
					Throw New IllegalArgumentException("unknown version: " & version)
			End Select
		End Sub

		''' <summary>
		''' Write the specified object to the ObjectOutputStream.  The class of the
		''' object, the signature of the [Class], and the values of the non-transient
		''' and non-static fields of the class and all of its supertypes are
		''' written.  Default serialization for a class can be overridden using the
		''' writeObject and the readObject methods.  Objects referenced by this
		''' object are written transitively so that a complete equivalent graph of
		''' objects can be reconstructed by an ObjectInputStream.
		''' 
		''' <p>Exceptions are thrown for problems with the OutputStream and for
		''' classes that should not be serialized.  All exceptions are fatal to the
		''' OutputStream, which is left in an indeterminate state, and it is up to
		''' the caller to ignore or recover the stream state.
		''' </summary>
		''' <exception cref="InvalidClassException"> Something is wrong with a class used by
		'''          serialization. </exception>
		''' <exception cref="NotSerializableException"> Some object to be serialized does not
		'''          implement the java.io.Serializable interface. </exception>
		''' <exception cref="IOException"> Any exception thrown by the underlying
		'''          OutputStream. </exception>
		Public Sub writeObject(ByVal obj As Object) Implements ObjectOutput.writeObject
			If enableOverride Then
				writeObjectOverride(obj)
				Return
			End If
			Try
				writeObject0(obj, False)
			Catch ex As IOException
				If depth = 0 Then writeFatalException(ex)
				Throw ex
			End Try
		End Sub

		''' <summary>
		''' Method used by subclasses to override the default writeObject method.
		''' This method is called by trusted subclasses of ObjectInputStream that
		''' constructed ObjectInputStream using the protected no-arg constructor.
		''' The subclass is expected to provide an override method with the modifier
		''' "final".
		''' </summary>
		''' <param name="obj"> object to be written to the underlying stream </param>
		''' <exception cref="IOException"> if there are I/O errors while writing to the
		'''          underlying stream </exception>
		''' <seealso cref= #ObjectOutputStream() </seealso>
		''' <seealso cref= #writeObject(Object)
		''' @since 1.2 </seealso>
		Protected Friend Overridable Sub writeObjectOverride(ByVal obj As Object)
		End Sub

		''' <summary>
		''' Writes an "unshared" object to the ObjectOutputStream.  This method is
		''' identical to writeObject, except that it always writes the given object
		''' as a new, unique object in the stream (as opposed to a back-reference
		''' pointing to a previously serialized instance).  Specifically:
		''' <ul>
		'''   <li>An object written via writeUnshared is always serialized in the
		'''       same manner as a newly appearing object (an object that has not
		'''       been written to the stream yet), regardless of whether or not the
		'''       object has been written previously.
		''' 
		'''   <li>If writeObject is used to write an object that has been previously
		'''       written with writeUnshared, the previous writeUnshared operation
		'''       is treated as if it were a write of a separate object.  In other
		'''       words, ObjectOutputStream will never generate back-references to
		'''       object data written by calls to writeUnshared.
		''' </ul>
		''' While writing an object via writeUnshared does not in itself guarantee a
		''' unique reference to the object when it is deserialized, it allows a
		''' single object to be defined multiple times in a stream, so that multiple
		''' calls to readUnshared by the receiver will not conflict.  Note that the
		''' rules described above only apply to the base-level object written with
		''' writeUnshared, and not to any transitively referenced sub-objects in the
		''' object graph to be serialized.
		''' 
		''' <p>ObjectOutputStream subclasses which override this method can only be
		''' constructed in security contexts possessing the
		''' "enableSubclassImplementation" SerializablePermission; any attempt to
		''' instantiate such a subclass without this permission will cause a
		''' SecurityException to be thrown.
		''' </summary>
		''' <param name="obj"> object to write to stream </param>
		''' <exception cref="NotSerializableException"> if an object in the graph to be
		'''          serialized does not implement the Serializable interface </exception>
		''' <exception cref="InvalidClassException"> if a problem exists with the class of an
		'''          object to be serialized </exception>
		''' <exception cref="IOException"> if an I/O error occurs during serialization
		''' @since 1.4 </exception>
		Public Overridable Sub writeUnshared(ByVal obj As Object)
			Try
				writeObject0(obj, True)
			Catch ex As IOException
				If depth = 0 Then writeFatalException(ex)
				Throw ex
			End Try
		End Sub

		''' <summary>
		''' Write the non-static and non-transient fields of the current class to
		''' this stream.  This may only be called from the writeObject method of the
		''' class being serialized. It will throw the NotActiveException if it is
		''' called otherwise.
		''' </summary>
		''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
		'''          <code>OutputStream</code> </exception>
		Public Overridable Sub defaultWriteObject()
			Dim ctx As java.io.SerialCallbackContext = curContext
			If ctx Is Nothing Then Throw New NotActiveException("not in call to writeObject")
			Dim curObj As Object = ctx.obj
			Dim curDesc As ObjectStreamClass = ctx.desc
			bout.blockDataMode = False
			defaultWriteFields(curObj, curDesc)
			bout.blockDataMode = True
		End Sub

		''' <summary>
		''' Retrieve the object used to buffer persistent fields to be written to
		''' the stream.  The fields will be written to the stream when writeFields
		''' method is called.
		''' </summary>
		''' <returns>  an instance of the class Putfield that holds the serializable
		'''          fields </returns>
		''' <exception cref="IOException"> if I/O errors occur
		''' @since 1.2 </exception>
		Public Overridable Function putFields() As ObjectOutputStream.PutField
			If curPut Is Nothing Then
				Dim ctx As java.io.SerialCallbackContext = curContext
				If ctx Is Nothing Then Throw New NotActiveException("not in call to writeObject")
				Dim curObj As Object = ctx.obj
				Dim curDesc As ObjectStreamClass = ctx.desc
				curPut = New PutFieldImpl(Me, curDesc)
			End If
			Return curPut
		End Function

		''' <summary>
		''' Write the buffered fields to the stream.
		''' </summary>
		''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
		'''          stream </exception>
		''' <exception cref="NotActiveException"> Called when a classes writeObject method was
		'''          not called to write the state of the object.
		''' @since 1.2 </exception>
		Public Overridable Sub writeFields()
			If curPut Is Nothing Then Throw New NotActiveException("no current PutField object")
			bout.blockDataMode = False
			curPut.writeFields()
			bout.blockDataMode = True
		End Sub

		''' <summary>
		''' Reset will disregard the state of any objects already written to the
		''' stream.  The state is reset to be the same as a new ObjectOutputStream.
		''' The current point in the stream is marked as reset so the corresponding
		''' ObjectInputStream will be reset at the same point.  Objects previously
		''' written to the stream will not be referred to as already being in the
		''' stream.  They will be written to the stream again.
		''' </summary>
		''' <exception cref="IOException"> if reset() is invoked while serializing an object. </exception>
		Public Overridable Sub reset()
			If depth <> 0 Then Throw New IOException("stream active")
			bout.blockDataMode = False
			bout.writeByte(TC_RESET)
			clear()
			bout.blockDataMode = True
		End Sub

		''' <summary>
		''' Subclasses may implement this method to allow class data to be stored in
		''' the stream. By default this method does nothing.  The corresponding
		''' method in ObjectInputStream is resolveClass.  This method is called
		''' exactly once for each unique class in the stream.  The class name and
		''' signature will have already been written to the stream.  This method may
		''' make free use of the ObjectOutputStream to save any representation of
		''' the class it deems suitable (for example, the bytes of the class file).
		''' The resolveClass method in the corresponding subclass of
		''' ObjectInputStream must read and use any data or objects written by
		''' annotateClass.
		''' </summary>
		''' <param name="cl"> the class to annotate custom data for </param>
		''' <exception cref="IOException"> Any exception thrown by the underlying
		'''          OutputStream. </exception>
		Protected Friend Overridable Sub annotateClass(ByVal cl As [Class])
		End Sub

		''' <summary>
		''' Subclasses may implement this method to store custom data in the stream
		''' along with descriptors for dynamic proxy classes.
		''' 
		''' <p>This method is called exactly once for each unique proxy class
		''' descriptor in the stream.  The default implementation of this method in
		''' <code>ObjectOutputStream</code> does nothing.
		''' 
		''' <p>The corresponding method in <code>ObjectInputStream</code> is
		''' <code>resolveProxyClass</code>.  For a given subclass of
		''' <code>ObjectOutputStream</code> that overrides this method, the
		''' <code>resolveProxyClass</code> method in the corresponding subclass of
		''' <code>ObjectInputStream</code> must read any data or objects written by
		''' <code>annotateProxyClass</code>.
		''' </summary>
		''' <param name="cl"> the proxy class to annotate custom data for </param>
		''' <exception cref="IOException"> any exception thrown by the underlying
		'''          <code>OutputStream</code> </exception>
		''' <seealso cref= ObjectInputStream#resolveProxyClass(String[])
		''' @since   1.3 </seealso>
		Protected Friend Overridable Sub annotateProxyClass(ByVal cl As [Class])
		End Sub

		''' <summary>
		''' This method will allow trusted subclasses of ObjectOutputStream to
		''' substitute one object for another during serialization. Replacing
		''' objects is disabled until enableReplaceObject is called. The
		''' enableReplaceObject method checks that the stream requesting to do
		''' replacement can be trusted.  The first occurrence of each object written
		''' into the serialization stream is passed to replaceObject.  Subsequent
		''' references to the object are replaced by the object returned by the
		''' original call to replaceObject.  To ensure that the private state of
		''' objects is not unintentionally exposed, only trusted streams may use
		''' replaceObject.
		''' 
		''' <p>The ObjectOutputStream.writeObject method takes a parameter of type
		''' Object (as opposed to type Serializable) to allow for cases where
		''' non-serializable objects are replaced by serializable ones.
		''' 
		''' <p>When a subclass is replacing objects it must insure that either a
		''' complementary substitution must be made during deserialization or that
		''' the substituted object is compatible with every field where the
		''' reference will be stored.  Objects whose type is not a subclass of the
		''' type of the field or array element abort the serialization by raising an
		''' exception and the object is not be stored.
		''' 
		''' <p>This method is called only once when each object is first
		''' encountered.  All subsequent references to the object will be redirected
		''' to the new object. This method should return the object to be
		''' substituted or the original object.
		''' 
		''' <p>Null can be returned as the object to be substituted, but may cause
		''' NullReferenceException in classes that contain references to the
		''' original object since they may be expecting an object instead of
		''' null.
		''' </summary>
		''' <param name="obj"> the object to be replaced </param>
		''' <returns>  the alternate object that replaced the specified one </returns>
		''' <exception cref="IOException"> Any exception thrown by the underlying
		'''          OutputStream. </exception>
		Protected Friend Overridable Function replaceObject(ByVal obj As Object) As Object
			Return obj
		End Function

		''' <summary>
		''' Enable the stream to do replacement of objects in the stream.  When
		''' enabled, the replaceObject method is called for every object being
		''' serialized.
		''' 
		''' <p>If <code>enable</code> is true, and there is a security manager
		''' installed, this method first calls the security manager's
		''' <code>checkPermission</code> method with a
		''' <code>SerializablePermission("enableSubstitution")</code> permission to
		''' ensure it's ok to enable the stream to do replacement of objects in the
		''' stream.
		''' </summary>
		''' <param name="enable"> boolean parameter to enable replacement of objects </param>
		''' <returns>  the previous setting before this method was invoked </returns>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''          <code>checkPermission</code> method denies enabling the stream
		'''          to do replacement of objects in the stream. </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.io.SerializablePermission </seealso>
		Protected Friend Overridable Function enableReplaceObject(ByVal enable As Boolean) As Boolean
			If enable = enableReplace Then Return enable
			If enable Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(SUBSTITUTION_PERMISSION)
			End If
			enableReplace = enable
			Return Not enableReplace
		End Function

		''' <summary>
		''' The writeStreamHeader method is provided so subclasses can append or
		''' prepend their own header to the stream.  It writes the magic number and
		''' version to the stream.
		''' </summary>
		''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
		'''          stream </exception>
		Protected Friend Overridable Sub writeStreamHeader()
			bout.writeShort(STREAM_MAGIC)
			bout.writeShort(STREAM_VERSION)
		End Sub

		''' <summary>
		''' Write the specified class descriptor to the ObjectOutputStream.  Class
		''' descriptors are used to identify the classes of objects written to the
		''' stream.  Subclasses of ObjectOutputStream may override this method to
		''' customize the way in which class descriptors are written to the
		''' serialization stream.  The corresponding method in ObjectInputStream,
		''' <code>readClassDescriptor</code>, should then be overridden to
		''' reconstitute the class descriptor from its custom stream representation.
		''' By default, this method writes class descriptors according to the format
		''' defined in the Object Serialization specification.
		''' 
		''' <p>Note that this method will only be called if the ObjectOutputStream
		''' is not using the old serialization stream format (set by calling
		''' ObjectOutputStream's <code>useProtocolVersion</code> method).  If this
		''' serialization stream is using the old format
		''' (<code>PROTOCOL_VERSION_1</code>), the class descriptor will be written
		''' internally in a manner that cannot be overridden or customized.
		''' </summary>
		''' <param name="desc"> class descriptor to write to the stream </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		''' <seealso cref= java.io.ObjectInputStream#readClassDescriptor() </seealso>
		''' <seealso cref= #useProtocolVersion(int) </seealso>
		''' <seealso cref= java.io.ObjectStreamConstants#PROTOCOL_VERSION_1
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub writeClassDescriptor(ByVal desc As ObjectStreamClass)
			desc.writeNonProxy(Me)
		End Sub

        ''' <summary>
        ''' Writes a java.lang.[Byte]. This method will block until the byte is actually
        ''' written.
        ''' </summary>
        ''' <param name="val"> the byte to be written to the stream </param>
        ''' <exception cref="IOException"> If an I/O error has occurred. </exception>
        Public Overrides Sub write(ByVal val As Integer) Implements ObjectOutput.write
            bout.write(val)
        End Sub

        ''' <summary>
        ''' Writes an array of bytes. This method will block until the bytes are
        ''' actually written.
        ''' </summary>
        ''' <param name="buf"> the data to be written </param>
        ''' <exception cref="IOException"> If an I/O error has occurred. </exception>
        Public Overrides Sub write(ByVal buf As SByte()) Implements ObjectOutput.write
            bout.write(buf, 0, buf.Length, False)
        End Sub

        ''' <summary>
        ''' Writes a sub array of bytes.
        ''' </summary>
        ''' <param name="buf"> the data to be written </param>
        ''' <param name="off"> the start offset in the data </param>
        ''' <param name="len"> the number of bytes that are written </param>
        ''' <exception cref="IOException"> If an I/O error has occurred. </exception>
        Public Overrides Sub write(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer)
            If buf Is Nothing Then Throw New NullPointerException
            Dim endoff As Integer = [off] + len
            If [off] < 0 OrElse len < 0 OrElse endoff > buf.Length OrElse endoff < 0 Then Throw New IndexOutOfBoundsException
            bout.write(buf, [off], len, False)
        End Sub

        ''' <summary>
        ''' Flushes the stream. This will write any buffered output bytes and flush
        ''' through to the underlying stream.
        ''' </summary>
        ''' <exception cref="IOException"> If an I/O error has occurred. </exception>
        Public Overrides Sub flush() Implements ObjectOutput.flush
            bout.flush()
        End Sub

        ''' <summary>
        ''' Drain any buffered data in ObjectOutputStream.  Similar to flush but
        ''' does not propagate the flush to the underlying stream.
        ''' </summary>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Protected Friend Overridable Sub drain()
            bout.drain()
        End Sub

        ''' <summary>
        ''' Closes the stream. This method must be called to release any resources
        ''' associated with the stream.
        ''' </summary>
        ''' <exception cref="IOException"> If an I/O error has occurred. </exception>
        Public Overrides Sub close() Implements ObjectOutput.close
            flush()
            clear()
            bout.close()
        End Sub

        ''' <summary>
        ''' Writes a  java.lang.[Boolean].
        ''' </summary>
        ''' <param name="val"> the boolean to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeBoolean(ByVal val As Boolean) Implements DataOutput.writeBoolean
            bout.writeBoolean(val)
        End Sub

        ''' <summary>
        ''' Writes an 8 bit java.lang.[Byte].
        ''' </summary>
        ''' <param name="val"> the byte value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeByte(ByVal val As Integer) Implements DataOutput.writeByte
            bout.writeByte(val)
        End Sub

        ''' <summary>
        ''' Writes a 16 bit  java.lang.[Short].
        ''' </summary>
        ''' <param name="val"> the short value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeShort(ByVal val As Integer) Implements DataOutput.writeShort
            bout.writeShort(val)
        End Sub

        ''' <summary>
        ''' Writes a 16 bit char.
        ''' </summary>
        ''' <param name="val"> the char value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeChar(ByVal val As Integer) Implements DataOutput.writeChar
            bout.writeChar(val)
        End Sub

        ''' <summary>
        ''' Writes a 32 bit int.
        ''' </summary>
        ''' <param name="val"> the integer value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeInt(ByVal val As Integer) Implements DataOutput.writeInt
            bout.writeInt(val)
        End Sub

        ''' <summary>
        ''' Writes a 64 bit java.lang.[Long].
        ''' </summary>
        ''' <param name="val"> the long value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeLong(ByVal val As Long) Implements DataOutput.writeLong
            bout.writeLong(val)
        End Sub

        ''' <summary>
        ''' Writes a 32 bit float.
        ''' </summary>
        ''' <param name="val"> the float value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeFloat(ByVal val As Single) Implements DataOutput.writeFloat
            bout.writeFloat(val)
        End Sub

        ''' <summary>
        ''' Writes a 64 bit java.lang.[Double].
        ''' </summary>
        ''' <param name="val"> the double value to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeDouble(ByVal val As Double) Implements DataOutput.writeDouble
            bout.writeDouble(val)
        End Sub

        ''' <summary>
        ''' Writes a String as a sequence of bytes.
        ''' </summary>
        ''' <param name="str"> the String of bytes to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeBytes(ByVal str As String) Implements DataOutput.writeBytes
            bout.writeBytes(str)
        End Sub

        ''' <summary>
        ''' Writes a String as a sequence of chars.
        ''' </summary>
        ''' <param name="str"> the String of chars to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeChars(ByVal str As String) Implements DataOutput.writeChars
            bout.writeChars(str)
        End Sub

        ''' <summary>
        ''' Primitive data write of this String in
        ''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
        ''' format.  Note that there is a
        ''' significant difference between writing a String into the stream as
        ''' primitive data or as an Object. A String instance written by writeObject
        ''' is written into the stream as a String initially. Future writeObject()
        ''' calls write references to the string into the stream.
        ''' </summary>
        ''' <param name="str"> the String to be written </param>
        ''' <exception cref="IOException"> if I/O errors occur while writing to the underlying
        '''          stream </exception>
        Public Overridable Sub writeUTF(ByVal str As String) Implements DataOutput.writeUTF
            bout.writeUTF(str)
        End Sub

        ''' <summary>
        ''' Provide programmatic access to the persistent fields to be written
        ''' to ObjectOutput.
        ''' 
        ''' @since 1.2
        ''' </summary>
        Public MustInherit Class PutField

            ''' <summary>
            ''' Put the value of the named boolean field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>boolean</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Boolean)

            ''' <summary>
            ''' Put the value of the named byte field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>byte</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As SByte)

            ''' <summary>
            ''' Put the value of the named char field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>char</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Char)

            ''' <summary>
            ''' Put the value of the named short field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>short</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Short)

            ''' <summary>
            ''' Put the value of the named int field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>int</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Integer)

            ''' <summary>
            ''' Put the value of the named long field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>long</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Long)

            ''' <summary>
            ''' Put the value of the named float field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>float</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Single)

            ''' <summary>
            ''' Put the value of the named double field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not
            ''' <code>double</code> </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Double)

            ''' <summary>
            ''' Put the value of the named Object field into the persistent field.
            ''' </summary>
            ''' <param name="name"> the name of the serializable field </param>
            ''' <param name="val"> the value to assign to the field
            '''         (which may be <code>null</code>) </param>
            ''' <exception cref="IllegalArgumentException"> if <code>name</code> does not
            ''' match the name of a serializable field for the class whose fields
            ''' are being written, or if the type of the named field is not a
            ''' reference type </exception>
            Public MustOverride Sub put(ByVal name As String, ByVal val As Object)

            ''' <summary>
            ''' Write the data and fields to the specified ObjectOutput stream,
            ''' which must be the same stream that produced this
            ''' <code>PutField</code> object.
            ''' </summary>
            ''' <param name="out"> the stream to write the data and fields to </param>
            ''' <exception cref="IOException"> if I/O errors occur while writing to the
            '''         underlying stream </exception>
            ''' <exception cref="IllegalArgumentException"> if the specified stream is not
            '''         the same stream that produced this <code>PutField</code>
            '''         object </exception>
            ''' @deprecated This method does not write the values contained by this
            '''         <code>PutField</code> object in a proper format, and may
            '''         result in corruption of the serialization stream.  The
            '''         correct way to write <code>PutField</code> data is by
            '''         calling the <seealso cref="java.io.ObjectOutputStream#writeFields()"/>
            '''         method. 
            <Obsolete("This method does not write the values contained by this")>
            Public MustOverride Sub write(ByVal out As ObjectOutput)
        End Class


        ''' <summary>
        ''' Returns protocol version in use.
        ''' </summary>
        Friend Overridable Property protocolVersion As Integer
            Get
                Return protocol
            End Get
        End Property

        ''' <summary>
        ''' Writes string without allowing it to be replaced in stream.  Used by
        ''' ObjectStreamClass to write class descriptor type strings.
        ''' </summary>
        Friend Overridable Sub writeTypeString(ByVal str As String)
            Dim handle As Integer
            If str Is Nothing Then
                writeNull()
            Else
                handle = [handles].lookup(str)
                If handle <> -1 Then
                    writeHandle(handle)
                Else
                    writeString(str, False)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Verifies that this (possibly subclass) instance can be constructed
        ''' without violating security constraints: the subclass must not override
        ''' security-sensitive non-final methods, or else the
        ''' "enableSubclassImplementation" SerializablePermission is checked.
        ''' </summary>
        Private Sub verifySubclass()
            Dim cl As [Class] = Me.GetType()
            If cl Is GetType(ObjectOutputStream) Then Return
            Dim sm As SecurityManager = System.securityManager
            If sm Is Nothing Then Return
            processQueue(Caches.subclassAuditsQueue, Caches.subclassAudits)
            Dim key As New java.io.ObjectStreamClass.WeakClassKey(cl, Caches.subclassAuditsQueue)
            Dim result As Boolean? = Caches.subclassAudits.Get(key)
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
                Dim cl As [Class] = subcl
                Do While cl IsNot GetType(ObjectOutputStream)
                    Try
                        cl.getDeclaredMethod("writeUnshared", New [Class]() {GetType(Object)})
                        Return java.lang.[Boolean].FALSE
                    Catch ex As NoSuchMethodException
                    End Try
                    Try
                        cl.getDeclaredMethod("putFields", CType(Nothing, Class()))
						Return java.lang.[Boolean].FALSE
                    Catch ex As NoSuchMethodException
                    End Try
                    cl = cl.BaseType
                Loop
                Return java.lang.[Boolean].TRUE
            End Function
        End Class

        ''' <summary>
        ''' Clears internal data structures.
        ''' </summary>
        Private Sub clear()
            subs.clear()
            [handles].clear()
        End Sub

        ''' <summary>
        ''' Underlying writeObject/writeUnshared implementation.
        ''' </summary>
        Private Sub writeObject0(ByVal obj As Object, ByVal unshared As Boolean)
            Dim oldMode As Boolean = bout.blockDataModeode(False)
            depth += 1
            Try
                ' handle previously written and non-replaceable objects
                Dim h As Integer
                obj = subs.lookup(obj)
                If obj Is Nothing Then
                    writeNull()
                    Return
                Else
                    h = [handles].lookup(obj)
                    If (Not unshared) AndAlso h <> -1 Then
                        writeHandle(h)
                        Return
                    ElseIf TypeOf obj Is Class Then
                        writeClass(CType(obj, [Class]), unshared)
                        Return
                    ElseIf TypeOf obj Is ObjectStreamClass Then
                        writeClassDesc(CType(obj, ObjectStreamClass), unshared)
                        Return
                    End If
                End If

                ' check for replacement object
                Dim orig As Object = obj
                Dim cl As [Class] = obj.GetType()
                Dim desc As ObjectStreamClass
                Do
                    ' REMIND: skip this check for strings/arrays?
                    Dim repCl As [Class]
                    desc = ObjectStreamClass.lookup(cl, True)
                    obj = desc.invokeWriteReplace(obj)
                    repCl = obj.GetType()
                    If (Not desc.hasWriteReplaceMethod()) OrElse obj Is Nothing OrElse repCl Is cl Then Exit Do
                    cl = repCl
                Loop
                If enableReplace Then
                    Dim rep As Object = replaceObject(obj)
                    If rep IsNot obj AndAlso rep IsNot Nothing Then
                        cl = rep.GetType()
                        desc = ObjectStreamClass.lookup(cl, True)
                    End If
                    obj = rep
                End If

                ' if object replaced, run through original checks a second time
                If obj IsNot orig Then
                    subs.assign(orig, obj)
                    If obj Is Nothing Then
                        writeNull()
                        Return
                    Else
                        h = [handles].lookup(obj)
                        If (Not unshared) AndAlso h <> -1 Then
                            writeHandle(h)
                            Return
                        ElseIf TypeOf obj Is Class Then
                            writeClass(CType(obj, [Class]), unshared)
                            Return
                        ElseIf TypeOf obj Is ObjectStreamClass Then
                            writeClassDesc(CType(obj, ObjectStreamClass), unshared)
                            Return
                        End If
                    End If
                End If

                ' remaining cases
                If TypeOf obj Is String Then
                    writeString(CStr(obj), unshared)
                ElseIf cl.array Then
                    writeArray(obj, desc, unshared)
                ElseIf TypeOf obj Is System.Enum Then
                    'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                    writeEnum(CType(obj, Enum(Of ?)), desc, unshared)
				ElseIf TypeOf obj Is Serializable Then
                    writeOrdinaryObject(obj, desc, unshared)
                Else
                    If extendedDebugInfo Then
                        Throw New NotSerializableException(cl.name & vbLf & debugInfoStack.ToString())
                    Else
                        Throw New NotSerializableException(cl.name)
                    End If
                End If
            Finally
                depth -= 1
                bout.blockDataMode = oldMode
            End Try
        End Sub

        ''' <summary>
        ''' Writes null code to stream.
        ''' </summary>
        Private Sub writeNull()
            bout.writeByte(TC_NULL)
        End Sub

        ''' <summary>
        ''' Writes given object handle to stream.
        ''' </summary>
        Private Sub writeHandle(ByVal handle As Integer)
            bout.writeByte(TC_REFERENCE)
            bout.writeInt(baseWireHandle + handle)
        End Sub

        ''' <summary>
        ''' Writes representation of given class to stream.
        ''' </summary>
        Private Sub writeClass(ByVal cl As [Class], ByVal unshared As Boolean)
            bout.writeByte(TC_CLASS)
            writeClassDesc(ObjectStreamClass.lookup(cl, True), False)
            [handles].assign(If(unshared, Nothing, cl))
        End Sub

        ''' <summary>
        ''' Writes representation of given class descriptor to stream.
        ''' </summary>
        Private Sub writeClassDesc(ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            Dim handle As Integer
            If desc Is Nothing Then
                writeNull()
            Else
                handle = [handles].lookup(desc)
                If (Not unshared) AndAlso handle <> -1 Then
                    writeHandle(handle)
                ElseIf desc.proxy Then
                    writeProxyDesc(desc, unshared)
                Else
                    writeNonProxyDesc(desc, unshared)
                End If
            End If
        End Sub

        Private Property customSubclass As Boolean
            Get
                ' Return true if this class is a custom subclass of ObjectOutputStream
                Return Me.GetType().classLoader <> GetType(ObjectOutputStream).classLoader
            End Get
        End Property

        ''' <summary>
        ''' Writes class descriptor representing a dynamic proxy class to stream.
        ''' </summary>
        Private Sub writeProxyDesc(ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            bout.writeByte(TC_PROXYCLASSDESC)
            [handles].assign(If(unshared, Nothing, desc))

            Dim cl As [Class] = desc.forClass()
            Dim ifaces As [Class]() = cl.interfaces
            bout.writeInt(ifaces.Length)
            For i As Integer = 0 To ifaces.Length - 1
                bout.writeUTF(ifaces(i).name)
            Next i

            bout.blockDataMode = True
            If cl IsNot Nothing AndAlso customSubclass Then sun.reflect.misc.ReflectUtil.checkPackageAccess(cl)
            annotateProxyClass(cl)
            bout.blockDataMode = False
            bout.writeByte(TC_ENDBLOCKDATA)

            writeClassDesc(desc.superDesc, False)
        End Sub

        ''' <summary>
        ''' Writes class descriptor representing a standard (i.e., not a dynamic
        ''' proxy) class to stream.
        ''' </summary>
        Private Sub writeNonProxyDesc(ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            bout.writeByte(TC_CLASSDESC)
            [handles].assign(If(unshared, Nothing, desc))

            If protocol = PROTOCOL_VERSION_1 Then
                ' do not invoke class descriptor write hook with old protocol
                desc.writeNonProxy(Me)
            Else
                writeClassDescriptor(desc)
            End If

            Dim cl As [Class] = desc.forClass()
            bout.blockDataMode = True
            If cl IsNot Nothing AndAlso customSubclass Then sun.reflect.misc.ReflectUtil.checkPackageAccess(cl)
            annotateClass(cl)
            bout.blockDataMode = False
            bout.writeByte(TC_ENDBLOCKDATA)

            writeClassDesc(desc.superDesc, False)
        End Sub

        ''' <summary>
        ''' Writes given string to stream, using standard or long UTF format
        ''' depending on string length.
        ''' </summary>
        Private Sub writeString(ByVal str As String, ByVal unshared As Boolean)
            [handles].assign(If(unshared, Nothing, str))
            Dim utflen As Long = bout.getUTFLength(str)
            If utflen <= &HFFFF Then
                bout.writeByte(TC_STRING)
                bout.writeUTF(str, utflen)
            Else
                bout.writeByte(TC_LONGSTRING)
                bout.writeLongUTF(str, utflen)
            End If
        End Sub

        ''' <summary>
        ''' Writes given array object to stream.
        ''' </summary>
        Private Sub writeArray(ByVal array As Object, ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            bout.writeByte(TC_ARRAY)
            writeClassDesc(desc, False)
            [handles].assign(If(unshared, Nothing, array))

            Dim ccl As [Class] = desc.forClass().componentType
            If ccl.primitive Then
                If ccl Is java.lang.[Integer].TYPE Then
                    Dim ia As Integer() = CType(array, Integer())
                    bout.writeInt(ia.Length)
                    bout.writeInts(ia, 0, ia.Length)
                ElseIf ccl Is java.lang.[Byte].TYPE Then
                    Dim ba As SByte() = CType(array, SByte())
                    bout.writeInt(ba.Length)
                    bout.write(ba, 0, ba.Length, True)
                ElseIf ccl Is java.lang.[Long].TYPE Then
                    Dim ja As Long() = CType(array, Long())
                    bout.writeInt(ja.Length)
                    bout.writeLongs(ja, 0, ja.Length)
                ElseIf ccl Is Float.TYPE Then
                    Dim fa As Single() = CType(array, Single())
                    bout.writeInt(fa.Length)
                    bout.writeFloats(fa, 0, fa.Length)
                ElseIf ccl Is java.lang.[Double].TYPE Then
                    Dim da As Double() = CType(array, Double())
                    bout.writeInt(da.Length)
                    bout.writeDoubles(da, 0, da.Length)
                ElseIf ccl Is java.lang.[Short].TYPE Then
                    Dim sa As Short() = CType(array, Short())
                    bout.writeInt(sa.Length)
                    bout.writeShorts(sa, 0, sa.Length)
                ElseIf ccl Is Character.TYPE Then
                    Dim ca As Char() = CType(array, Char())
                    bout.writeInt(ca.Length)
                    bout.writeChars(ca, 0, ca.Length)
                ElseIf ccl Is java.lang.[Boolean].TYPE Then
                    Dim za As Boolean() = CType(array, Boolean())
                    bout.writeInt(za.Length)
                    bout.writeBooleans(za, 0, za.Length)
                Else
                    Throw New InternalError
                End If
            Else
                Dim objs As Object() = CType(array, Object())
                Dim len As Integer = objs.Length
                bout.writeInt(len)
                If extendedDebugInfo Then debugInfoStack.push("array (class """ & array.GetType().Name & """, size: " & len & ")")
                Try
                    For i As Integer = 0 To len - 1
                        If extendedDebugInfo Then debugInfoStack.push("element of array (index: " & i & ")")
                        Try
                            writeObject0(objs(i), False)
                        Finally
                            If extendedDebugInfo Then debugInfoStack.pop()
                        End Try
                    Next i
                Finally
                    If extendedDebugInfo Then debugInfoStack.pop()
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Writes given enum constant to stream.
        ''' </summary>
        Private Sub writeEnum(Of T1)(ByVal en As System.Enum(Of T1), ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            bout.writeByte(TC_ENUM)
            Dim sdesc As ObjectStreamClass = desc.superDesc
            writeClassDesc(If(sdesc.forClass() Is GetType(System.Enum), desc, sdesc), False)
            [handles].assign(If(unshared, Nothing, en))
            writeString(en.name(), False)
        End Sub

        ''' <summary>
        ''' Writes representation of a "ordinary" (i.e., not a String, [Class],
        ''' ObjectStreamClass, array, or enum constant) serializable object to the
        ''' stream.
        ''' </summary>
        Private Sub writeOrdinaryObject(ByVal obj As Object, ByVal desc As ObjectStreamClass, ByVal unshared As Boolean)
            If extendedDebugInfo Then debugInfoStack.push((If(depth = 1, "root ", "")) & "object (class """ & obj.GetType().Name & """, " & obj.ToString() & ")")
            Try
                desc.checkSerialize()

                bout.writeByte(TC_OBJECT)
                writeClassDesc(desc, False)
                [handles].assign(If(unshared, Nothing, obj))
                If desc.externalizable AndAlso (Not desc.proxy) Then
                    writeExternalData(CType(obj, Externalizable))
                Else
                    writeSerialData(obj, desc)
                End If
            Finally
                If extendedDebugInfo Then debugInfoStack.pop()
            End Try
        End Sub

        ''' <summary>
        ''' Writes externalizable data of given object by invoking its
        ''' writeExternal() method.
        ''' </summary>
        Private Sub writeExternalData(ByVal obj As Externalizable)
            Dim oldPut As PutFieldImpl = curPut
            curPut = Nothing

            If extendedDebugInfo Then debugInfoStack.push("writeExternal data")
            Dim oldContext As java.io.SerialCallbackContext = curContext
            Try
                curContext = Nothing
                If protocol = PROTOCOL_VERSION_1 Then
                    obj.writeExternal(Me)
                Else
                    bout.blockDataMode = True
                    obj.writeExternal(Me)
                    bout.blockDataMode = False
                    bout.writeByte(TC_ENDBLOCKDATA)
                End If
            Finally
                curContext = oldContext
                If extendedDebugInfo Then debugInfoStack.pop()
            End Try

            curPut = oldPut
        End Sub

        ''' <summary>
        ''' Writes instance data for each serializable class of given object, from
        ''' superclass to subclass.
        ''' </summary>
        Private Sub writeSerialData(ByVal obj As Object, ByVal desc As ObjectStreamClass)
            Dim slots As ObjectStreamClass.ClassDataSlot() = desc.classDataLayout
            For i As Integer = 0 To slots.Length - 1
                Dim slotDesc As ObjectStreamClass = slots(i).desc
                If slotDesc.hasWriteObjectMethod() Then
                    Dim oldPut As PutFieldImpl = curPut
                    curPut = Nothing
                    Dim oldContext As java.io.SerialCallbackContext = curContext

                    If extendedDebugInfo Then debugInfoStack.push("custom writeObject data (class """ & slotDesc.name & """)")
                    Try
                        curContext = New java.io.SerialCallbackContext(obj, slotDesc)
                        bout.blockDataMode = True
                        slotDesc.invokeWriteObject(obj, Me)
                        bout.blockDataMode = False
                        bout.writeByte(TC_ENDBLOCKDATA)
                    Finally
                        curContext.usedsed()
                        curContext = oldContext
                        If extendedDebugInfo Then debugInfoStack.pop()
                    End Try

                    curPut = oldPut
                Else
                    defaultWriteFields(obj, slotDesc)
                End If
            Next i
        End Sub

        ''' <summary>
        ''' Fetches and writes values of serializable fields of given object to
        ''' stream.  The given class descriptor specifies which field values to
        ''' write, and in which order they should be written.
        ''' </summary>
        Private Sub defaultWriteFields(ByVal obj As Object, ByVal desc As ObjectStreamClass)
            Dim cl As [Class] = desc.forClass()
            If cl IsNot Nothing AndAlso obj IsNot Nothing AndAlso (Not cl.isInstance(obj)) Then Throw New ClassCastException

            desc.checkDefaultSerialize()

            Dim primDataSize As Integer = desc.primDataSize
            If primVals Is Nothing OrElse primVals.Length < primDataSize Then primVals = New SByte(primDataSize - 1) {}
            desc.getPrimFieldValues(obj, primVals)
            bout.write(primVals, 0, primDataSize, False)

            Dim fields As ObjectStreamField() = desc.getFields(False)
            Dim objVals As Object() = New Object(desc.numObjFields - 1) {}
            Dim numPrimFields As Integer = fields.Length - objVals.Length
            desc.getObjFieldValues(obj, objVals)
            For i As Integer = 0 To objVals.Length - 1
                If extendedDebugInfo Then debugInfoStack.push("field (class """ & desc.name & """, name: """ & fields(numPrimFields + i).name & """, type: """ & fields(numPrimFields + i).type & """)")
                Try
                    writeObject0(objVals(i), fields(numPrimFields + i).unshared)
                Finally
                    If extendedDebugInfo Then debugInfoStack.pop()
                End Try
            Next i
        End Sub

        ''' <summary>
        ''' Attempts to write to stream fatal IOException that has caused
        ''' serialization to abort.
        ''' </summary>
        Private Sub writeFatalException(ByVal ex As IOException)
            '        
            '         * Note: the serialization specification states that if a second
            '         * IOException occurs while attempting to serialize the original fatal
            '         * exception to the stream, then a StreamCorruptedException should be
            '         * thrown (section 2.1).  However, due to a bug in previous
            '         * implementations of serialization, StreamCorruptedExceptions were
            '         * rarely (if ever) actually thrown--the "root" exceptions from
            '         * underlying streams were thrown instead.  This historical behavior is
            '         * followed here for consistency.
            '         
            clear()
            Dim oldMode As Boolean = bout.blockDataModeode(False)
            Try
                bout.writeByte(TC_EXCEPTION)
                writeObject0(ex, False)
                clear()
            Finally
                bout.blockDataMode = oldMode
            End Try
        End Sub

        ''' <summary>
        ''' Converts specified span of float values into byte values.
        ''' </summary>
        ' REMIND: remove once hotspot inlines Float.floatToIntBits
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub floatsToBytes(ByVal src As Single(), ByVal srcpos As Integer, ByVal dst As SByte(), ByVal dstpos As Integer, ByVal nfloats As Integer)
        End Sub

        ''' <summary>
        ''' Converts specified span of double values into byte values.
        ''' </summary>
        ' REMIND: remove once hotspot inlines java.lang.[Double].doubleToLongBits
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub doublesToBytes(ByVal src As Double(), ByVal srcpos As Integer, ByVal dst As SByte(), ByVal dstpos As Integer, ByVal ndoubles As Integer)
        End Sub

        ''' <summary>
        ''' Default PutField implementation.
        ''' </summary>
        Private Class PutFieldImpl
            Inherits PutField

            Private ReadOnly outerInstance As ObjectOutputStream


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
            ''' Creates PutFieldImpl object for writing fields defined in given
            ''' class descriptor.
            ''' </summary>
            Friend Sub New(ByVal outerInstance As ObjectOutputStream, ByVal desc As ObjectStreamClass)
                Me.outerInstance = outerInstance
                Me.desc = desc
                primVals = New SByte(desc.primDataSize - 1) {}
                objVals = New Object(desc.numObjFields - 1) {}
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Boolean)
                Bits.putBoolean(primVals, getFieldOffset(name, java.lang.[Boolean].TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As SByte)
                primVals(getFieldOffset(name, java.lang.[Byte].TYPE)) = val
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Char)
                Bits.putChar(primVals, getFieldOffset(name, Character.TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Short)
                Bits.putShort(primVals, getFieldOffset(name, java.lang.[Short].TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Integer)
                Bits.putInt(primVals, getFieldOffset(name, java.lang.[Integer].TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Single)
                Bits.putFloat(primVals, getFieldOffset(name, Float.TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Long)
                Bits.putLong(primVals, getFieldOffset(name, java.lang.[Long].TYPE), val)
            End Sub

            Public Overrides Sub put(ByVal name As String, ByVal val As Double)
                Bits.putDouble(primVals, getFieldOffset(name, java.lang.[Double].TYPE), val)
            End Sub

			Public Overrides Sub put(ByVal name As String, ByVal val As Object)
                objVals(getFieldOffset(name, GetType(java.lang.Object))) = val
            End Sub

			' deprecated in ObjectOutputStream.PutField
			Public Overrides Sub write(ByVal out As ObjectOutput)
	'            
	'             * Applications should *not* use this method to write PutField
	'             * data, as it will lead to stream corruption if the PutField
	'             * object writes any primitive data (since block data mode is not
	'             * unset/set properly, as is done in OOS.writeFields()).  This
	'             * broken implementation is being retained solely for behavioral
	'             * compatibility, in order to support applications which use
	'             * OOS.PutField.write() for writing only non-primitive data.
	'             *
	'             * Serialization of unshared objects is not implemented here since
	'             * it is not necessary for backwards compatibility; also, unshared
	'             * semantics may not be supported by the given ObjectOutput
	'             * instance.  Applications which write unshared objects using the
	'             * PutField API must use OOS.writeFields().
	'             
				If ObjectOutputStream.this IsNot out Then Throw New IllegalArgumentException("wrong stream")
				out.write(primVals, 0, primVals.Length)

				Dim fields As ObjectStreamField() = desc.getFields(False)
				Dim numPrimFields As Integer = fields.Length - objVals.Length
				' REMIND: warn if numPrimFields > 0?
				For i As Integer = 0 To objVals.Length - 1
					If fields(numPrimFields + i).unshared Then Throw New IOException("cannot write unshared object")
					out.writeObject(objVals(i))
				Next i
			End Sub

			''' <summary>
			''' Writes buffered primitive data and object fields to stream.
			''' </summary>
			Friend Overridable Sub writeFields()
				outerInstance.bout.write(primVals, 0, primVals.Length, False)

				Dim fields As ObjectStreamField() = desc.getFields(False)
				Dim numPrimFields As Integer = fields.Length - objVals.Length
				For i As Integer = 0 To objVals.Length - 1
					If extendedDebugInfo Then outerInstance.debugInfoStack.push("field (class """ & desc.name & """, name: """ & fields(numPrimFields + i).name & """, type: """ & fields(numPrimFields + i).type & """)")
					Try
						outerInstance.writeObject0(objVals(i), fields(numPrimFields + i).unshared)
					Finally
						If extendedDebugInfo Then outerInstance.debugInfoStack.pop()
					End Try
				Next i
			End Sub

			''' <summary>
			''' Returns offset of field with given name and type.  A specified type
			''' of null matches all types, Object.class matches all non-primitive
			''' types, and any other non-null type matches assignable types only.
			''' Throws IllegalArgumentException if no matching field found.
			''' </summary>
			Private Function getFieldOffset(ByVal name As String, ByVal type As [Class]) As Integer
				Dim field As ObjectStreamField = desc.getField(name, type)
				If field Is Nothing Then Throw New IllegalArgumentException("no such field " & name & " with type " & type)
				Return field.offset
			End Function
		End Class

		''' <summary>
		''' Buffered output stream with two modes: in default mode, outputs data in
		''' same format as DataOutputStream; in "block data" mode, outputs data
		''' bracketed by block data markers (see object serialization specification
		''' for details).
		''' </summary>
		Private Class BlockDataOutputStream
			Inherits OutputStream
			Implements DataOutput

			''' <summary>
			''' maximum data block length </summary>
			Private Const MAX_BLOCK_SIZE As Integer = 1024
			''' <summary>
			''' maximum data block header length </summary>
			Private Const MAX_HEADER_SIZE As Integer = 5
			''' <summary>
			''' (tunable) length of char buffer (for writing strings) </summary>
			Private Const CHAR_BUF_SIZE As Integer = 256

			''' <summary>
			''' buffer for writing general/block data </summary>
			Private ReadOnly buf As SByte() = New SByte(MAX_BLOCK_SIZE - 1){}
			''' <summary>
			''' buffer for writing block data headers </summary>
			Private ReadOnly hbuf As SByte() = New SByte(MAX_HEADER_SIZE - 1){}
			''' <summary>
			''' char buffer for fast string writes </summary>
			Private ReadOnly cbuf As Char() = New Char(CHAR_BUF_SIZE - 1){}

			''' <summary>
			''' block data mode </summary>
			Private blkmode As Boolean = False
			''' <summary>
			''' current offset into buf </summary>
			Private pos As Integer = 0

			''' <summary>
			''' underlying output stream </summary>
			Private ReadOnly out As OutputStream
			''' <summary>
			''' loopback stream (for data writes that span data blocks) </summary>
			Private ReadOnly dout As DataOutputStream

			''' <summary>
			''' Creates new BlockDataOutputStream on top of given underlying stream.
			''' Block data mode is turned off by default.
			''' </summary>
			Friend Sub New(ByVal out As OutputStream)
				Me.out = out
				dout = New DataOutputStream(Me)
			End Sub

			''' <summary>
			''' Sets block data mode to the given mode (true == on, false == off)
			''' and returns the previous mode value.  If the new mode is the same as
			''' the old mode, no action is taken.  If the new mode differs from the
			''' old mode, any buffered data is flushed before switching to the new
			''' mode.
			''' </summary>
			Friend Overridable Function setBlockDataMode(ByVal mode As Boolean) As Boolean
				If blkmode = mode Then Return blkmode
				drain()
				blkmode = mode
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

			' ----------------- generic output stream methods ----------------- 
	'        
	'         * The following methods are equivalent to their counterparts in
	'         * OutputStream, except that they partition written data into data
	'         * blocks when in block data mode.
	'         

			Public Overrides Sub write(ByVal b As Integer) Implements DataOutput.write
				If pos >= MAX_BLOCK_SIZE Then drain()
				buf(pos) = CByte(b)
				pos += 1
			End Sub

			Public Overrides Sub write(ByVal b As SByte()) Implements DataOutput.write
				write(b, 0, b.Length, False)
			End Sub

			Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				write(b, [off], len, False)
			End Sub

			Public Overrides Sub flush()
				drain()
				out.flush()
			End Sub

			Public Overrides Sub close()
				flush()
				out.close()
			End Sub

			''' <summary>
			''' Writes specified span of byte values from given array.  If copy is
			''' true, copies the values to an intermediate buffer before writing
			''' them to underlying stream (to avoid exposing a reference to the
			''' original byte array).
			''' </summary>
			Friend Overridable Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal copy As Boolean)
				If Not(copy OrElse blkmode) Then ' write directly
					drain()
					out.write(b, [off], len)
					Return
				End If

				Do While len > 0
					If pos >= MAX_BLOCK_SIZE Then drain()
					If len >= MAX_BLOCK_SIZE AndAlso (Not copy) AndAlso pos = 0 Then
						' avoid unnecessary copy
						writeBlockHeader(MAX_BLOCK_SIZE)
						out.write(b, [off], MAX_BLOCK_SIZE)
						[off] += MAX_BLOCK_SIZE
						len -= MAX_BLOCK_SIZE
					Else
						Dim wlen As Integer = System.Math.Min(len, MAX_BLOCK_SIZE - pos)
						Array.Copy(b, [off], buf, pos, wlen)
						pos += wlen
						[off] += wlen
						len -= wlen
					End If
				Loop
			End Sub

			''' <summary>
			''' Writes all buffered data from this stream to the underlying stream,
			''' but does not flush underlying stream.
			''' </summary>
			Friend Overridable Sub drain()
				If pos = 0 Then Return
				If blkmode Then writeBlockHeader(pos)
				out.write(buf, 0, pos)
				pos = 0
			End Sub

			''' <summary>
			''' Writes block data header.  Data blocks shorter than 256 bytes are
			''' prefixed with a 2-byte header; all others start with a 5-byte
			''' header.
			''' </summary>
			Private Sub writeBlockHeader(ByVal len As Integer)
				If len <= &HFF Then
					hbuf(0) = TC_BLOCKDATA
					hbuf(1) = CByte(len)
					out.write(hbuf, 0, 2)
				Else
					hbuf(0) = TC_BLOCKDATALONG
					Bits.putInt(hbuf, 1, len)
					out.write(hbuf, 0, 5)
				End If
			End Sub


			' ----------------- primitive data output methods ----------------- 
	'        
	'         * The following methods are equivalent to their counterparts in
	'         * DataOutputStream, except that they partition written data into data
	'         * blocks when in block data mode.
	'         

			Public Overridable Sub writeBoolean(ByVal v As Boolean) Implements DataOutput.writeBoolean
				If pos >= MAX_BLOCK_SIZE Then drain()
				Bits.putBoolean(buf, pos, v)
				pos += 1
			End Sub

			Public Overridable Sub writeByte(ByVal v As Integer) Implements DataOutput.writeByte
				If pos >= MAX_BLOCK_SIZE Then drain()
				buf(pos) = CByte(v)
				pos += 1
			End Sub

			Public Overridable Sub writeChar(ByVal v As Integer) Implements DataOutput.writeChar
				If pos + 2 <= MAX_BLOCK_SIZE Then
					Bits.putChar(buf, pos, ChrW(v))
					pos += 2
				Else
					dout.writeChar(v)
				End If
			End Sub

			Public Overridable Sub writeShort(ByVal v As Integer) Implements DataOutput.writeShort
				If pos + 2 <= MAX_BLOCK_SIZE Then
					Bits.putShort(buf, pos, CShort(v))
					pos += 2
				Else
					dout.writeShort(v)
				End If
			End Sub

			Public Overridable Sub writeInt(ByVal v As Integer) Implements DataOutput.writeInt
				If pos + 4 <= MAX_BLOCK_SIZE Then
					Bits.putInt(buf, pos, v)
					pos += 4
				Else
					dout.writeInt(v)
				End If
			End Sub

			Public Overridable Sub writeFloat(ByVal v As Single) Implements DataOutput.writeFloat
				If pos + 4 <= MAX_BLOCK_SIZE Then
					Bits.putFloat(buf, pos, v)
					pos += 4
				Else
					dout.writeFloat(v)
				End If
			End Sub

			Public Overridable Sub writeLong(ByVal v As Long) Implements DataOutput.writeLong
				If pos + 8 <= MAX_BLOCK_SIZE Then
					Bits.putLong(buf, pos, v)
					pos += 8
				Else
					dout.writeLong(v)
				End If
			End Sub

			Public Overridable Sub writeDouble(ByVal v As Double) Implements DataOutput.writeDouble
				If pos + 8 <= MAX_BLOCK_SIZE Then
					Bits.putDouble(buf, pos, v)
					pos += 8
				Else
					dout.writeDouble(v)
				End If
			End Sub

			Public Overridable Sub writeBytes(ByVal s As String) Implements DataOutput.writeBytes
				Dim endoff As Integer = s.length()
				Dim cpos As Integer = 0
				Dim csize As Integer = 0
				Dim [off] As Integer = 0
				Do While [off] < endoff
					If cpos >= csize Then
						cpos = 0
						csize = System.Math.Min(endoff - [off], CHAR_BUF_SIZE)
						s.getChars([off], [off] + csize, cbuf, 0)
					End If
					If pos >= MAX_BLOCK_SIZE Then drain()
					Dim n As Integer = System.Math.Min(csize - cpos, MAX_BLOCK_SIZE - pos)
					Dim [stop] As Integer = pos + n
					Do While pos < [stop]
						buf(pos) = AscW(cbuf(cpos))
						cpos += 1
						pos += 1
					Loop
					[off] += n
				Loop
			End Sub

			Public Overridable Sub writeChars(ByVal s As String) Implements DataOutput.writeChars
				Dim endoff As Integer = s.length()
				Dim [off] As Integer = 0
				Do While [off] < endoff
					Dim csize As Integer = System.Math.Min(endoff - [off], CHAR_BUF_SIZE)
					s.getChars([off], [off] + csize, cbuf, 0)
					writeChars(cbuf, 0, csize)
					[off] += csize
				Loop
			End Sub

			Public Overridable Sub writeUTF(ByVal s As String) Implements DataOutput.writeUTF
				writeUTF(s, getUTFLength(s))
			End Sub


			' -------------- primitive data array output methods -------------- 
	'        
	'         * The following methods write out spans of primitive data values.
	'         * Though equivalent to calling the corresponding primitive write
	'         * methods repeatedly, these methods are optimized for writing groups
	'         * of primitive data values more efficiently.
	'         

			Friend Overridable Sub writeBooleans(ByVal v As Boolean(), ByVal [off] As Integer, ByVal len As Integer)
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos >= MAX_BLOCK_SIZE Then drain()
					Dim [stop] As Integer = System.Math.Min(endoff, [off] + (MAX_BLOCK_SIZE - pos))
					Do While [off] < [stop]
						Bits.putBoolean(buf, pos, v([off]))
						[off] += 1
						pos += 1
					Loop
				Loop
			End Sub

			Friend Overridable Sub writeChars(ByVal v As Char(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 2
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 1
						Dim [stop] As Integer = System.Math.Min(endoff, [off] + avail)
						Do While [off] < [stop]
							Bits.putChar(buf, pos, v([off]))
							[off] += 1
							pos += 2
						Loop
					Else
						dout.writeChar(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			Friend Overridable Sub writeShorts(ByVal v As Short(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 2
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 1
						Dim [stop] As Integer = System.Math.Min(endoff, [off] + avail)
						Do While [off] < [stop]
							Bits.putShort(buf, pos, v([off]))
							[off] += 1
							pos += 2
						Loop
					Else
						dout.writeShort(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			Friend Overridable Sub writeInts(ByVal v As Integer(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 4
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 2
						Dim [stop] As Integer = System.Math.Min(endoff, [off] + avail)
						Do While [off] < [stop]
							Bits.putInt(buf, pos, v([off]))
							[off] += 1
							pos += 4
						Loop
					Else
						dout.writeInt(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			Friend Overridable Sub writeFloats(ByVal v As Single(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 4
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 2
						Dim chunklen As Integer = System.Math.Min(endoff - [off], avail)
						floatsToBytes(v, [off], buf, pos, chunklen)
						[off] += chunklen
						pos += chunklen << 2
					Else
						dout.writeFloat(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			Friend Overridable Sub writeLongs(ByVal v As Long(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 8
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 3
						Dim [stop] As Integer = System.Math.Min(endoff, [off] + avail)
						Do While [off] < [stop]
							Bits.putLong(buf, pos, v([off]))
							[off] += 1
							pos += 8
						Loop
					Else
						dout.writeLong(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			Friend Overridable Sub writeDoubles(ByVal v As Double(), ByVal [off] As Integer, ByVal len As Integer)
				Dim limit As Integer = MAX_BLOCK_SIZE - 8
				Dim endoff As Integer = [off] + len
				Do While [off] < endoff
					If pos <= limit Then
						Dim avail As Integer = (MAX_BLOCK_SIZE - pos) >> 3
						Dim chunklen As Integer = System.Math.Min(endoff - [off], avail)
						doublesToBytes(v, [off], buf, pos, chunklen)
						[off] += chunklen
						pos += chunklen << 3
					Else
						dout.writeDouble(v([off]))
						[off] += 1
					End If
				Loop
			End Sub

			''' <summary>
			''' Returns the length in bytes of the UTF encoding of the given string.
			''' </summary>
			Friend Overridable Function getUTFLength(ByVal s As String) As Long
				Dim len As Integer = s.length()
				Dim utflen As Long = 0
				Dim [off] As Integer = 0
				Do While [off] < len
					Dim csize As Integer = System.Math.Min(len - [off], CHAR_BUF_SIZE)
					s.getChars([off], [off] + csize, cbuf, 0)
					For cpos As Integer = 0 To csize - 1
						Dim c As Char = cbuf(cpos)
						If c >= &H1 AndAlso c <= &H7F Then
							utflen += 1
						ElseIf AscW(c) > &H7FF Then
							utflen += 3
						Else
							utflen += 2
						End If
					Next cpos
					[off] += csize
				Loop
				Return utflen
			End Function

			''' <summary>
			''' Writes the given string in UTF format.  This method is used in
			''' situations where the UTF encoding length of the string is already
			''' known; specifying it explicitly avoids a prescan of the string to
			''' determine its UTF length.
			''' </summary>
			Friend Overridable Sub writeUTF(ByVal s As String, ByVal utflen As Long)
				If utflen > &HFFFFL Then Throw New UTFDataFormatException
				writeShort(CInt(utflen))
				If utflen = CLng(s.length()) Then
					writeBytes(s)
				Else
					writeUTFBody(s)
				End If
			End Sub

			''' <summary>
			''' Writes given string in "long" UTF format.  "Long" UTF format is
			''' identical to standard UTF, except that it uses an 8 byte header
			''' (instead of the standard 2 bytes) to convey the UTF encoding length.
			''' </summary>
			Friend Overridable Sub writeLongUTF(ByVal s As String)
				writeLongUTF(s, getUTFLength(s))
			End Sub

			''' <summary>
			''' Writes given string in "long" UTF format, where the UTF encoding
			''' length of the string is already known.
			''' </summary>
			Friend Overridable Sub writeLongUTF(ByVal s As String, ByVal utflen As Long)
				writeLong(utflen)
				If utflen = CLng(s.length()) Then
					writeBytes(s)
				Else
					writeUTFBody(s)
				End If
			End Sub

			''' <summary>
			''' Writes the "body" (i.e., the UTF representation minus the 2-byte or
			''' 8-byte length header) of the UTF encoding for the given string.
			''' </summary>
			Private Sub writeUTFBody(ByVal s As String)
				Dim limit As Integer = MAX_BLOCK_SIZE - 3
				Dim len As Integer = s.length()
				Dim [off] As Integer = 0
				Do While [off] < len
					Dim csize As Integer = System.Math.Min(len - [off], CHAR_BUF_SIZE)
					s.getChars([off], [off] + csize, cbuf, 0)
					For cpos As Integer = 0 To csize - 1
						Dim c As Char = cbuf(cpos)
						If pos <= limit Then
							If c <= &H7F AndAlso AscW(c) <> 0 Then
								buf(pos) = AscW(c)
								pos += 1
							ElseIf AscW(c) > &H7FF Then
								buf(pos + 2) = CByte(&H80 Or ((AscW(c) >> 0) And &H3F))
								buf(pos + 1) = CByte(&H80 Or ((AscW(c) >> 6) And &H3F))
								buf(pos + 0) = CByte(&HE0 Or ((AscW(c) >> 12) And &HF))
								pos += 3
							Else
								buf(pos + 1) = CByte(&H80 Or ((AscW(c) >> 0) And &H3F))
								buf(pos + 0) = CByte(&HC0 Or ((AscW(c) >> 6) And &H1F))
								pos += 2
							End If ' write one byte at a time to normalize block
						Else
							If c <= &H7F AndAlso AscW(c) <> 0 Then
								write(c)
							ElseIf AscW(c) > &H7FF Then
								write(&HE0 Or ((AscW(c) >> 12) And &HF))
								write(&H80 Or ((AscW(c) >> 6) And &H3F))
								write(&H80 Or ((AscW(c) >> 0) And &H3F))
							Else
								write(&HC0 Or ((AscW(c) >> 6) And &H1F))
								write(&H80 Or ((AscW(c) >> 0) And &H3F))
							End If
						End If
					Next cpos
					[off] += csize
				Loop
			End Sub
		End Class

		''' <summary>
		''' Lightweight identity hash table which maps objects to integer handles,
		''' assigned in ascending order.
		''' </summary>
		Private Class HandleTable

			' number of mappings in table/next available handle 
			Private size_Renamed As Integer
			' size threshold determining when to expand hash spine 
			Private threshold As Integer
			' factor for computing size threshold 
			Private ReadOnly loadFactor As Single
			' maps hash value -> candidate handle value 
			Private spine As Integer()
			' maps handle value -> next candidate handle value 
			Private [next] As Integer()
			' maps handle value -> associated object 
			Private objs As Object()

			''' <summary>
			''' Creates new HandleTable with given capacity and load factor.
			''' </summary>
			Friend Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
				Me.loadFactor = loadFactor
				spine = New Integer(initialCapacity - 1){}
				[next] = New Integer(initialCapacity - 1){}
				objs = New Object(initialCapacity - 1){}
				threshold = CInt(Fix(initialCapacity * loadFactor))
				clear()
			End Sub

			''' <summary>
			''' Assigns next available handle to given object, and returns handle
			''' value.  Handles are assigned in ascending order starting at 0.
			''' </summary>
			Friend Overridable Function assign(ByVal obj As Object) As Integer
				If size_Renamed >= [next].Length Then growEntries()
				If size_Renamed >= threshold Then growSpine()
				insert(obj, size_Renamed)
					Dim tempVar As Integer = size_Renamed
					size_Renamed += 1
					Return tempVar
			End Function

			''' <summary>
			''' Looks up and returns handle associated with given object, or -1 if
			''' no mapping found.
			''' </summary>
			Friend Overridable Function lookup(ByVal obj As Object) As Integer
				If size_Renamed = 0 Then Return -1
				Dim index As Integer = hash(obj) Mod spine.Length
				Dim i As Integer = spine(index)
				Do While i >= 0
					If objs(i) Is obj Then Return i
					i = [next](i)
				Loop
				Return -1
			End Function

			''' <summary>
			''' Resets table to its initial (empty) state.
			''' </summary>
			Friend Overridable Sub clear()
				java.util.Arrays.fill(spine, -1)
				java.util.Arrays.fill(objs, 0, size_Renamed, Nothing)
				size_Renamed = 0
			End Sub

			''' <summary>
			''' Returns the number of mappings currently in table.
			''' </summary>
			Friend Overridable Function size() As Integer
				Return size_Renamed
			End Function

			''' <summary>
			''' Inserts mapping object -> handle mapping into table.  Assumes table
			''' is large enough to accommodate new mapping.
			''' </summary>
			Private Sub insert(ByVal obj As Object, ByVal handle As Integer)
				Dim index As Integer = hash(obj) Mod spine.Length
				objs(handle) = obj
				[next](handle) = spine(index)
				spine(index) = handle
			End Sub

			''' <summary>
			''' Expands the hash "spine" -- equivalent to increasing the number of
			''' buckets in a conventional hash table.
			''' </summary>
			Private Sub growSpine()
				spine = New Integer((spine.Length << 1)){}
				threshold = CInt(Fix(spine.Length * loadFactor))
				java.util.Arrays.fill(spine, -1)
				For i As Integer = 0 To size_Renamed - 1
					insert(objs(i), i)
				Next i
			End Sub

			''' <summary>
			''' Increases hash table capacity by lengthening entry arrays.
			''' </summary>
			Private Sub growEntries()
				Dim newLength As Integer = ([next].Length << 1) + 1
				Dim newNext As Integer() = New Integer(newLength - 1){}
				Array.Copy([next], 0, newNext, 0, size_Renamed)
				[next] = newNext

				Dim newObjs As Object() = New Object(newLength - 1){}
				Array.Copy(objs, 0, newObjs, 0, size_Renamed)
				objs = newObjs
			End Sub

			''' <summary>
			''' Returns hash value for given object.
			''' </summary>
			Private Function hash(ByVal obj As Object) As Integer
				Return System.identityHashCode(obj) And &H7FFFFFFF
			End Function
		End Class

		''' <summary>
		''' Lightweight identity hash table which maps objects to replacement
		''' objects.
		''' </summary>
		Private Class ReplaceTable

			' maps object -> index 
			Private ReadOnly htab As HandleTable
			' maps index -> replacement object 
			Private reps As Object()

			''' <summary>
			''' Creates new ReplaceTable with given capacity and load factor.
			''' </summary>
			Friend Sub New(ByVal initialCapacity As Integer, ByVal loadFactor As Single)
				htab = New HandleTable(initialCapacity, loadFactor)
				reps = New Object(initialCapacity - 1){}
			End Sub

			''' <summary>
			''' Enters mapping from object to replacement object.
			''' </summary>
			Friend Overridable Sub assign(ByVal obj As Object, ByVal rep As Object)
				Dim index As Integer = htab.assign(obj)
				Do While index >= reps.Length
					grow()
				Loop
				reps(index) = rep
			End Sub

			''' <summary>
			''' Looks up and returns replacement for given object.  If no
			''' replacement is found, returns the lookup object itself.
			''' </summary>
			Friend Overridable Function lookup(ByVal obj As Object) As Object
				Dim index As Integer = htab.lookup(obj)
				Return If(index >= 0, reps(index), obj)
			End Function

			''' <summary>
			''' Resets table to its initial (empty) state.
			''' </summary>
			Friend Overridable Sub clear()
				java.util.Arrays.fill(reps, 0, htab.size(), Nothing)
				htab.clear()
			End Sub

			''' <summary>
			''' Returns the number of mappings currently in table.
			''' </summary>
			Friend Overridable Function size() As Integer
				Return htab.size()
			End Function

			''' <summary>
			''' Increases table capacity.
			''' </summary>
			Private Sub grow()
				Dim newReps As Object() = New Object((reps.Length << 1)){}
				Array.Copy(reps, 0, newReps, 0, reps.Length)
				reps = newReps
			End Sub
		End Class

		''' <summary>
		''' Stack to keep debug information about the state of the
		''' serialization process, for embedding in exception messages.
		''' </summary>
		Private Class DebugTraceInfoStack
			Private ReadOnly stack As IList(Of String)

			Friend Sub New()
				stack = New List(Of )
			End Sub

			''' <summary>
			''' Removes all of the elements from enclosed list.
			''' </summary>
			Friend Overridable Sub clear()
				stack.Clear()
			End Sub

			''' <summary>
			''' Removes the object at the top of enclosed list.
			''' </summary>
			Friend Overridable Sub pop()
				stack.Remove(stack.Count-1)
			End Sub

			''' <summary>
			''' Pushes a String onto the top of enclosed list.
			''' </summary>
			Friend Overridable Sub push(ByVal entry As String)
				stack.Add(vbTab & "- " & entry)
			End Sub

			''' <summary>
			''' Returns a string representation of this object
			''' </summary>
			Public Overrides Function ToString() As String
				Dim buffer As New StringBuilder
				If stack.Count > 0 Then
					For i As Integer = stack.Count To 1 Step -1
						buffer.append(stack(i-1) + (If(i <> 1, vbLf, "")))
					Next i
				End If
				Return buffer.ToString()
			End Function
		End Class

	End Class

End Namespace