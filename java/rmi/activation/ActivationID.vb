Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.activation


	''' <summary>
	''' Activation makes use of special identifiers to denote remote
	''' objects that can be activated over time. An activation identifier
	''' (an instance of the class <code>ActivationID</code>) contains several
	''' pieces of information needed for activating an object:
	''' <ul>
	''' <li> a remote reference to the object's activator (a {@link
	''' java.rmi.server.RemoteRef RemoteRef}
	''' instance), and
	''' <li> a unique identifier (a <seealso cref="java.rmi.server.UID UID"/>
	''' instance) for the object. </ul> <p>
	''' 
	''' An activation identifier for an object can be obtained by registering
	''' an object with the activation system. Registration is accomplished
	''' in a few ways: <ul>
	''' <li>via the <code>Activatable.register</code> method
	''' <li>via the first <code>Activatable</code> constructor (that takes
	''' three arguments and both registers and exports the object, and
	''' <li>via the first <code>Activatable.exportObject</code> method
	''' that takes the activation descriptor, object and port as arguments;
	''' this method both registers and exports the object. </ul>
	''' 
	''' @author      Ann Wollrath </summary>
	''' <seealso cref=         Activatable
	''' @since       1.2 </seealso>
	<Serializable> _
	Public Class ActivationID
		''' <summary>
		''' the object's activator
		''' </summary>
		<NonSerialized> _
		Private activator As Activator

		''' <summary>
		''' the object's unique id
		''' </summary>
		<NonSerialized> _
		Private uid As New java.rmi.server.UID

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Const serialVersionUID As Long = -4608673054848209235L

		''' <summary>
		''' The constructor for <code>ActivationID</code> takes a single
		''' argument, activator, that specifies a remote reference to the
		''' activator responsible for activating the object associated with
		''' this identifier. An instance of <code>ActivationID</code> is globally
		''' unique.
		''' </summary>
		''' <param name="activator"> reference to the activator responsible for
		''' activating the object </param>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		'''         not supported by this implementation
		''' @since 1.2 </exception>
		Public Sub New(  activator As Activator)
			Me.activator = activator
		End Sub

		''' <summary>
		''' Activate the object for this id.
		''' </summary>
		''' <param name="force"> if true, forces the activator to contact the group
		''' when activating the object (instead of returning a cached reference);
		''' if false, returning a cached value is acceptable. </param>
		''' <returns> the reference to the active remote object </returns>
		''' <exception cref="ActivationException"> if activation fails </exception>
		''' <exception cref="UnknownObjectException"> if the object is unknown </exception>
		''' <exception cref="RemoteException"> if remote call fails
		''' @since 1.2 </exception>
		Public Overridable Function activate(  force As Boolean) As java.rmi.Remote
			Try
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim mobj As java.rmi.MarshalledObject(Of ? As java.rmi.Remote) = activator.activate(Me, force)
				Return mobj.get()
			Catch e As java.rmi.RemoteException
				Throw e
			Catch e As java.io.IOException
				Throw New java.rmi.UnmarshalException("activation failed", e)
			Catch e As  ClassNotFoundException
				Throw New java.rmi.UnmarshalException("activation failed", e)
			End Try

		End Function

		''' <summary>
		''' Returns a hashcode for the activation id.  Two identifiers that
		''' refer to the same remote object will have the same hash code.
		''' </summary>
		''' <seealso cref= java.util.Hashtable
		''' @since 1.2 </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return uid.GetHashCode()
		End Function

		''' <summary>
		''' Compares two activation ids for content equality.
		''' Returns true if both of the following conditions are true:
		''' 1) the unique identifiers equivalent (by content), and
		''' 2) the activator specified in each identifier
		'''    refers to the same remote object.
		''' </summary>
		''' <param name="obj">     the Object to compare with </param>
		''' <returns>  true if these Objects are equal; false otherwise. </returns>
		''' <seealso cref=             java.util.Hashtable
		''' @since 1.2 </seealso>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If TypeOf obj Is ActivationID Then
				Dim id As ActivationID = CType(obj, ActivationID)
				Return (uid.Equals(id.uid) AndAlso activator.Equals(id.activator))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' <code>writeObject</code> for custom serialization.
		''' 
		''' <p>This method writes this object's serialized form for
		''' this class as follows:
		''' 
		''' <p>The <code>writeObject</code> method is invoked on
		''' <code>out</code> passing this object's unique identifier
		''' (a <seealso cref="java.rmi.server.UID UID"/> instance) as the argument.
		''' 
		''' <p>Next, the {@link
		''' java.rmi.server.RemoteRef#getRefClass(java.io.ObjectOutput)
		''' getRefClass} method is invoked on the activator's
		''' <code>RemoteRef</code> instance to obtain its external ref
		''' type name.  Next, the <code>writeUTF</code> method is
		''' invoked on <code>out</code> with the value returned by
		''' <code>getRefClass</code>, and then the
		''' <code>writeExternal</code> method is invoked on the
		''' <code>RemoteRef</code> instance passing <code>out</code>
		''' as the argument.
		''' 
		''' @serialData The serialized data for this class comprises a
		''' <code>java.rmi.server.UID</code> (written with
		''' <code>ObjectOutput.writeObject</code>) followed by the
		''' external ref type name of the activator's
		''' <code>RemoteRef</code> instance (a string written with
		''' <code>ObjectOutput.writeUTF</code>), followed by the
		''' external form of the <code>RemoteRef</code> instance as
		''' written by its <code>writeExternal</code> method.
		''' 
		''' <p>The external ref type name of the
		''' <code>RemoteRef</Code> instance is
		''' determined using the definitions of external ref type
		''' names specified in the {@link java.rmi.server.RemoteObject
		''' RemoteObject} <code>writeObject</code> method
		''' <b>serialData</b> specification.  Similarly, the data
		''' written by the <code>writeExternal</code> method and read
		''' by the <code>readExternal</code> method of
		''' <code>RemoteRef</code> implementation classes
		''' corresponding to each of the defined external ref type
		''' names is specified in the {@link
		''' java.rmi.server.RemoteObject RemoteObject}
		''' <code>writeObject</code> method <b>serialData</b>
		''' specification.
		''' 
		''' </summary>
		Private Sub writeObject(  out As java.io.ObjectOutputStream)
			out.writeObject(uid)

			Dim ref As java.rmi.server.RemoteRef
			If TypeOf activator Is java.rmi.server.RemoteObject Then
				ref = CType(activator, java.rmi.server.RemoteObject).ref
			ElseIf Proxy.isProxyClass(activator.GetType()) Then
				Dim handler As InvocationHandler = Proxy.getInvocationHandler(activator)
				If Not(TypeOf handler Is java.rmi.server.RemoteObjectInvocationHandler) Then Throw New java.io.InvalidObjectException("unexpected invocation handler")
				ref = CType(handler, java.rmi.server.RemoteObjectInvocationHandler).ref

			Else
				Throw New java.io.InvalidObjectException("unexpected activator type")
			End If
			out.writeUTF(ref.getRefClass(out))
			ref.writeExternal(out)
		End Sub

		''' <summary>
		''' <code>readObject</code> for custom serialization.
		''' 
		''' <p>This method reads this object's serialized form for this
		''' class as follows:
		''' 
		''' <p>The <code>readObject</code> method is invoked on
		''' <code>in</code> to read this object's unique identifier
		''' (a <seealso cref="java.rmi.server.UID UID"/> instance).
		''' 
		''' <p>Next, the <code>readUTF</code> method is invoked on
		''' <code>in</code> to read the external ref type name of the
		''' <code>RemoteRef</code> instance for this object's
		''' activator.  Next, the <code>RemoteRef</code>
		''' instance is created of an implementation-specific class
		''' corresponding to the external ref type name (returned by
		''' <code>readUTF</code>), and the <code>readExternal</code>
		''' method is invoked on that <code>RemoteRef</code> instance
		''' to read the external form corresponding to the external
		''' ref type name.
		''' 
		''' <p>Note: If the external ref type name is
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
		''' case the <code>RemoteRef</code> will be an instance of
		''' that implementation-specific class.
		''' </summary>
		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			uid = CType([in].readObject(), java.rmi.server.UID)

			Try
				Dim refClass As  [Class] = Type.GetType(java.rmi.server.RemoteRef.packagePrefix & "." & [in].readUTF()).asSubclass(GetType(java.rmi.server.RemoteRef))
				Dim ref As java.rmi.server.RemoteRef = refClass.newInstance()
				ref.readExternal([in])
				activator = CType(Proxy.newProxyInstance(Nothing, New [Class]() { GetType(Activator) }, New java.rmi.server.RemoteObjectInvocationHandler(ref)), Activator)

			Catch e As InstantiationException
				Throw CType((New java.io.InvalidObjectException("Unable to create remote reference")).initCause(e), java.io.IOException)
			Catch e As IllegalAccessException
				Throw CType((New java.io.InvalidObjectException("Unable to create remote reference")).initCause(e), java.io.IOException)
			End Try
		End Sub
	End Class

End Namespace