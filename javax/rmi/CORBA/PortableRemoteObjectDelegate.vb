Imports System

'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' Supports delegation for method implementations in <seealso cref="javax.rmi.PortableRemoteObject"/>.
	''' The delegate is a singleton instance of a class that implements this
	''' interface and provides a replacement implementation for all the
	''' methods of <code>javax.rmi.PortableRemoteObject</code>.
	''' 
	''' Delegates are enabled by providing the delegate's class name as the
	''' value of the
	''' <code>javax.rmi.CORBA.PortableRemoteObjectClass</code>
	''' system property.
	''' </summary>
	''' <seealso cref= javax.rmi.PortableRemoteObject </seealso>
	Public Interface PortableRemoteObjectDelegate

		''' <summary>
		''' Delegation call for <seealso cref="javax.rmi.PortableRemoteObject#exportObject"/>.
		''' </summary>
		Sub exportObject(ByVal obj As java.rmi.Remote)

		''' <summary>
		''' Delegation call for <seealso cref="javax.rmi.PortableRemoteObject#toStub"/>.
		''' </summary>
		Function toStub(ByVal obj As java.rmi.Remote) As java.rmi.Remote

		''' <summary>
		''' Delegation call for <seealso cref="javax.rmi.PortableRemoteObject#unexportObject"/>.
		''' </summary>
		Sub unexportObject(ByVal obj As java.rmi.Remote)

		''' <summary>
		''' Delegation call for <seealso cref="javax.rmi.PortableRemoteObject#narrow"/>.
		''' </summary>
		Function narrow(ByVal narrowFrom As Object, ByVal narrowTo As Type) As Object

		''' <summary>
		''' Delegation call for <seealso cref="javax.rmi.PortableRemoteObject#connect"/>.
		''' </summary>
		Sub connect(ByVal target As java.rmi.Remote, ByVal source As java.rmi.Remote)

	End Interface

End Namespace