'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace javax.rmi.CORBA


	''' <summary>
	''' Supports delegation for method implementations in <seealso cref="Stub"/>.
	''' A delegate is an instance of a class that implements this
	''' interface and provides a replacement implementation for all the
	''' methods of <code>javax.rmi.CORBA.Stub</code>.  If delegation is
	''' enabled, each stub has an associated delegate.
	''' 
	''' Delegates are enabled by providing the delegate's class name as the
	''' value of the
	''' <code>javax.rmi.CORBA.StubClass</code>
	''' system property.
	''' </summary>
	''' <seealso cref= Stub </seealso>
	Public Interface StubDelegate

		''' <summary>
		''' Delegation call for <seealso cref="Stub#hashCode"/>.
		''' </summary>
		Function GetHashCode(ByVal self As Stub) As Integer

		''' <summary>
		''' Delegation call for <seealso cref="Stub#equals"/>.
		''' </summary>
		Function Equals(ByVal self As Stub, ByVal obj As Object) As Boolean

		''' <summary>
		''' Delegation call for <seealso cref="Stub#toString"/>.
		''' </summary>
		Function ToString(ByVal self As Stub) As String

		''' <summary>
		''' Delegation call for <seealso cref="Stub#connect"/>.
		''' </summary>
		Sub connect(ByVal self As Stub, ByVal orb As org.omg.CORBA.ORB)

		' _REVISIT_ cannot link to Stub.readObject directly... why not?
		''' <summary>
		''' Delegation call for
		''' <a href="{@docRoot}/serialized-form.html#javax.rmi.CORBA.Stub"><code>Stub.readObject(java.io.ObjectInputStream)</code></a>.
		''' </summary>
		Sub readObject(ByVal self As Stub, ByVal s As java.io.ObjectInputStream)

		' _REVISIT_ cannot link to Stub.writeObject directly... why not?
		''' <summary>
		''' Delegation call for
		''' <a href="{@docRoot}/serialized-form.html#javax.rmi.CORBA.Stub"><code>Stub.writeObject(java.io.ObjectOutputStream)</code></a>.
		''' </summary>
		Sub writeObject(ByVal self As Stub, ByVal s As java.io.ObjectOutputStream)

	End Interface

End Namespace