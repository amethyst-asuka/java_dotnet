Imports System

'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.server


	''' <summary>
	''' The <code>RemoteObject</code> class implements the
	''' <code>java.lang.Object</code> behavior for remote objects.
	''' <code>RemoteObject</code> provides the remote semantics of Object by
	''' implementing methods for hashCode, equals, and toString.
	''' 
	''' @author      Ann Wollrath
	''' @author      Laird Dornin
	''' @author      Peter Jones
	''' @since       JDK1.1
	''' </summary>
	<Serializable> _
	Public MustInherit Class RemoteObject
		Implements java.rmi.Remote

		''' <summary>
		''' The object's remote reference. </summary>
		<NonSerialized> _
		Protected Friend ref As RemoteRef

		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class </summary>
		Private Const serialVersionUID As Long = -3215090123894869218L

		''' <summary>
		''' Creates a remote object.
		''' </summary>
		Protected Friend Sub New()
			ref = Nothing
		End Sub

		''' <summary>
		''' Creates a remote object, initialized with the specified remote
		''' reference. </summary>
		''' <param name="newref"> remote reference </param>
		Protected Friend Sub New(ByVal newref As RemoteRef)
			ref = newref
		End Sub

		''' <summary>
		''' Returns the remote reference for the remote object.
		''' 
		''' <p>Note: The object returned from this method may be an instance of
		''' an implementation-specific class.  The <code>RemoteObject</code>
		''' class ensures serialization portability of its instances' remote
		''' references through the behavior of its custom
		''' <code>writeObject</code> and <code>readObject</code> methods.  An
		''' instance of <code>RemoteRef</code> should not be serialized outside
		''' of its <code>RemoteObject</code> wrapper instance or the result may
		''' be unportable.
		''' </summary>
		''' <returns> remote reference for the remote object
		''' @since 1.2 </returns>
		Public Overridable Property ref As RemoteRef
			Get
				Return ref
			End Get
		End Property

		''' <summary>
		''' Returns the stub for the remote object <code>obj</code> passed
		''' as a parameter. This operation is only valid <i>after</i>
		''' the object has been exported. </summary>
		''' <param name="obj"> the remote object whose stub is needed </param>
		''' <returns> the stub for the remote object, <code>obj</code>. </returns>
		''' <exception cref="NoSuchObjectException"> if the stub for the
		''' remote object could not be found.
		''' @since 1.2 </exception>
		Public Shared Function toStub(ByVal obj As java.rmi.Remote) As java.rmi.Remote
			If TypeOf obj Is RemoteStub OrElse (obj IsNot Nothing AndAlso Proxy.isProxyClass(obj.GetType()) AndAlso TypeOf Proxy.getInvocationHandler(obj) Is RemoteObjectInvocationHandler) Then
				Return obj
			Else
				Return sun.rmi.transport.ObjectTable.getStub(obj)
			End If
		End Function

		''' <summary>
		''' Returns a hashcode for a remote object.  Two remote object stubs
		''' that refer to the same remote object will have the same hash code
		''' (in order to support remote objects as keys in hash tables).
		''' </summary>
		''' <seealso cref=             java.util.Hashtable </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return If(ref Is Nothing, MyBase.GetHashCode(), ref.remoteHashCode())
		End Function

		''' <summary>
		''' Compares two remote objects for equality.
		''' Returns a boolean that indicates whether this remote object is
		''' equivalent to the specified Object. This method is used when a
		''' remote object is stored in a hashtable.
		''' If the specified Object is not itself an instance of RemoteObject,
		''' then this method delegates by returning the result of invoking the
		''' <code>equals</code> method of its parameter with this remote object
		''' as the argument. </summary>
		''' <param name="obj">     the Object to compare with </param>
		''' <returns>  true if these Objects are equal; false otherwise. </returns>
		''' <seealso cref=             java.util.Hashtable </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is RemoteObject Then
				If ref Is Nothing Then
					Return obj Is Me
				Else
					Return ref.remoteEquals(CType(obj, RemoteObject).ref)
				End If
			ElseIf obj IsNot Nothing Then
	'            
	'             * Fix for 4099660: if object is not an instance of RemoteObject,
	'             * use the result of its equals method, to support symmetry is a
	'             * remote object implementation class that does not extend
	'             * RemoteObject wishes to support equality with its stub objects.
	'             
				Return obj.Equals(Me)
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns a String that represents the value of this remote object.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim classname As String = sun.rmi.server.Util.getUnqualifiedName(Me.GetType())
			Return If(ref Is Nothing, classname, classname & "[" & ref.remoteToString() & "]")
		End Function

		''' <summary>
		''' <code>writeObject</code> for custom serialization.
		''' 
		''' <p>This method writes this object's serialized form for this class
		''' as follows:
		''' 
		''' <p>The <seealso cref="RemoteRef#getRefClass(java.io.ObjectOutput) getRefClass"/>
		''' method is invoked on this object's <code>ref</code> field
		''' to obtain its external ref type name.
		''' If the value returned by <code>getRefClass</code> was
		''' a non-<code>null</code> string of length greater than zero,
		''' the <code>writeUTF</code> method is invoked on <code>out</code>
		''' with the value returned by <code>getRefClass</code>, and then
		''' the <code>writeExternal</code> method is invoked on
		''' this object's <code>ref</code> field passing <code>out</code>
		''' as the argument; otherwise,
		''' the <code>writeUTF</code> method is invoked on <code>out</code>
		''' with a zero-length string (<code>""</code>), and then
		''' the <code>writeObject</code> method is invoked on <code>out</code>
		''' passing this object's <code>ref</code> field as the argument.
		''' 
		''' @serialData
		''' 
		''' The serialized data for this class comprises a string (written with
		''' <code>ObjectOutput.writeUTF</code>) that is either the external
		''' ref type name of the contained <code>RemoteRef</code> instance
		''' (the <code>ref</code> field) or a zero-length string, followed by
		''' either the external form of the <code>ref</code> field as written by
		''' its <code>writeExternal</code> method if the string was of non-zero
		''' length, or the serialized form of the <code>ref</code> field as
		''' written by passing it to the serialization stream's
		''' <code>writeObject</code> if the string was of zero length.
		''' 
		''' <p>If this object is an instance of
		''' <seealso cref="RemoteStub"/> or <seealso cref="RemoteObjectInvocationHandler"/>
		''' that was returned from any of
		''' the <code>UnicastRemoteObject.exportObject</code> methods
		''' and custom socket factories are not used,
		''' the external ref type name is <code>"UnicastRef"</code>.
		''' 
		''' If this object is an instance of
		''' <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		''' that was returned from any of
		''' the <code>UnicastRemoteObject.exportObject</code> methods
		''' and custom socket factories are used,
		''' the external ref type name is <code>"UnicastRef2"</code>.
		''' 
		''' If this object is an instance of
		''' <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		''' that was returned from any of
		''' the <code>java.rmi.activation.Activatable.exportObject</code> methods,
		''' the external ref type name is <code>"ActivatableRef"</code>.
		''' 
		''' If this object is an instance of
		''' <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		''' that was returned from
		''' the <code>RemoteObject.toStub</code> method (and the argument passed
		''' to <code>toStub</code> was not itself a <code>RemoteStub</code>),
		''' the external ref type name is a function of how the remote object
		''' passed to <code>toStub</code> was exported, as described above.
		''' 
		''' If this object is an instance of
		''' <code>RemoteStub</code> or <code>RemoteObjectInvocationHandler</code>
		''' that was originally created via deserialization,
		''' the external ref type name is the same as that which was read
		''' when this object was deserialized.
		''' 
		''' <p>If this object is an instance of
		''' <code>java.rmi.server.UnicastRemoteObject</code> that does not
		''' use custom socket factories,
		''' the external ref type name is <code>"UnicastServerRef"</code>.
		''' 
		''' If this object is an instance of
		''' <code>UnicastRemoteObject</code> that does
		''' use custom socket factories,
		''' the external ref type name is <code>"UnicastServerRef2"</code>.
		''' 
		''' <p>Following is the data that must be written by the
		''' <code>writeExternal</code> method and read by the
		''' <code>readExternal</code> method of <code>RemoteRef</code>
		''' implementation classes that correspond to the each of the
		''' defined external ref type names:
		''' 
		''' <p>For <code>"UnicastRef"</code>:
		''' 
		''' <ul>
		''' 
		''' <li>the hostname of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		''' 
		''' <li>the port of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		''' 
		''' <li>the data written as a result of calling
		''' {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		''' on the <code>ObjID</code> instance contained in the reference
		''' 
		''' <li>the boolean value <code>false</code>,
		''' written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		''' 
		''' </ul>
		''' 
		''' <p>For <code>"UnicastRef2"</code> with a
		''' <code>null</code> client socket factory:
		''' 
		''' <ul>
		''' 
		''' <li>the byte value <code>0x00</code>
		''' (indicating <code>null</code> client socket factory),
		''' written by <seealso cref="java.io.ObjectOutput#writeByte(int)"/>
		''' 
		''' <li>the hostname of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		''' 
		''' <li>the port of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		''' 
		''' <li>the data written as a result of calling
		''' {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		''' on the <code>ObjID</code> instance contained in the reference
		''' 
		''' <li>the boolean value <code>false</code>,
		''' written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		''' 
		''' </ul>
		''' 
		''' <p>For <code>"UnicastRef2"</code> with a
		''' non-<code>null</code> client socket factory:
		''' 
		''' <ul>
		''' 
		''' <li>the byte value <code>0x01</code>
		''' (indicating non-<code>null</code> client socket factory),
		''' written by <seealso cref="java.io.ObjectOutput#writeByte(int)"/>
		''' 
		''' <li>the hostname of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		''' 
		''' <li>the port of the referenced remote object,
		''' written by <seealso cref="java.io.ObjectOutput#writeInt(int)"/>
		''' 
		''' <li>a client socket factory (object of type
		''' <code>java.rmi.server.RMIClientSocketFactory</code>),
		''' written by passing it to an invocation of
		''' <code>writeObject</code> on the stream instance
		''' 
		''' <li>the data written as a result of calling
		''' {link java.rmi.server.ObjID#write(java.io.ObjectOutput)}
		''' on the <code>ObjID</code> instance contained in the reference
		''' 
		''' <li>the boolean value <code>false</code>,
		''' written by <seealso cref="java.io.ObjectOutput#writeBoolean(boolean)"/>
		''' 
		''' </ul>
		''' 
		''' <p>For <code>"ActivatableRef"</code> with a
		''' <code>null</code> nested remote reference:
		''' 
		''' <ul>
		''' 
		''' <li>an instance of
		''' <code>java.rmi.activation.ActivationID</code>,
		''' written by passing it to an invocation of
		''' <code>writeObject</code> on the stream instance
		''' 
		''' <li>a zero-length string (<code>""</code>),
		''' written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		''' 
		''' </ul>
		''' 
		''' <p>For <code>"ActivatableRef"</code> with a
		''' non-<code>null</code> nested remote reference:
		''' 
		''' <ul>
		''' 
		''' <li>an instance of
		''' <code>java.rmi.activation.ActivationID</code>,
		''' written by passing it to an invocation of
		''' <code>writeObject</code> on the stream instance
		''' 
		''' <li>the external ref type name of the nested remote reference,
		''' which must be <code>"UnicastRef2"</code>,
		''' written by <seealso cref="java.io.ObjectOutput#writeUTF(String)"/>
		''' 
		''' <li>the external form of the nested remote reference,
		''' written by invoking its <code>writeExternal</code> method
		''' with the stream instance
		''' (see the description of the external form for
		''' <code>"UnicastRef2"</code> above)
		''' 
		''' </ul>
		''' 
		''' <p>For <code>"UnicastServerRef"</code> and
		''' <code>"UnicastServerRef2"</code>, no data is written by the
		''' <code>writeExternal</code> method or read by the
		''' <code>readExternal</code> method.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			If ref Is Nothing Then
				Throw New java.rmi.MarshalException("Invalid remote object")
			Else
				Dim refClassName As String = ref.getRefClass(out)
				If refClassName Is Nothing OrElse refClassName.length() = 0 Then
	'                
	'                 * No reference class name specified, so serialize
	'                 * remote reference.
	'                 
					out.writeUTF("")
					out.writeObject(ref)
				Else
	'                
	'                 * Built-in reference class specified, so delegate
	'                 * to reference to write out its external form.
	'                 
					out.writeUTF(refClassName)
					ref.writeExternal(out)
				End If
			End If
		End Sub

		''' <summary>
		''' <code>readObject</code> for custom serialization.
		''' 
		''' <p>This method reads this object's serialized form for this class
		''' as follows:
		''' 
		''' <p>The <code>readUTF</code> method is invoked on <code>in</code>
		''' to read the external ref type name for the <code>RemoteRef</code>
		''' instance to be filled in to this object's <code>ref</code> field.
		''' If the string returned by <code>readUTF</code> has length zero,
		''' the <code>readObject</code> method is invoked on <code>in</code>,
		''' and than the value returned by <code>readObject</code> is cast to
		''' <code>RemoteRef</code> and this object's <code>ref</code> field is
		''' set to that value.
		''' Otherwise, this object's <code>ref</code> field is set to a
		''' <code>RemoteRef</code> instance that is created of an
		''' implementation-specific class corresponding to the external ref
		''' type name returned by <code>readUTF</code>, and then
		''' the <code>readExternal</code> method is invoked on
		''' this object's <code>ref</code> field.
		''' 
		''' <p>If the external ref type name is
		''' <code>"UnicastRef"</code>, <code>"UnicastServerRef"</code>,
		''' <code>"UnicastRef2"</code>, <code>"UnicastServerRef2"</code>,
		''' or <code>"ActivatableRef"</code>, a corresponding
		''' implementation-specific class must be found, and its
		''' <code>readExternal</code> method must read the serial data
		''' for that external ref type name as specified to be written
		''' in the <b>serialData</b> documentation for this class.
		''' If the external ref type name is any other string (of non-zero
		''' length), a <code>ClassNotFoundException</code> will be thrown,
		''' unless the implementation provides an implementation-specific
		''' class corresponding to that external ref type name, in which
		''' case this object's <code>ref</code> field will be set to an
		''' instance of that implementation-specific class.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			Dim refClassName As String = [in].readUTF()
			If refClassName Is Nothing OrElse refClassName.length() = 0 Then
	'            
	'             * No reference class name specified, so construct
	'             * remote reference from its serialized form.
	'             
				ref = CType([in].readObject(), RemoteRef)
			Else
	'            
	'             * Built-in reference class specified, so delegate to
	'             * internal reference class to initialize its fields from
	'             * its external form.
	'             
				Dim internalRefClassName As String = RemoteRef.packagePrefix & "." & refClassName
				Dim refClass As  [Class] = Type.GetType(internalRefClassName)
				Try
					ref = CType(refClass.newInstance(), RemoteRef)

	'                
	'                 * If this step fails, assume we found an internal
	'                 * class that is not meant to be a serializable ref
	'                 * type.
	'                 
				Catch e As InstantiationException
					Throw New ClassNotFoundException(internalRefClassName, e)
				Catch e As IllegalAccessException
					Throw New ClassNotFoundException(internalRefClassName, e)
				Catch e As  [Class]CastException
					Throw New ClassNotFoundException(internalRefClassName, e)
				End Try
				ref.readExternal([in])
			End If
		End Sub
	End Class

End Namespace