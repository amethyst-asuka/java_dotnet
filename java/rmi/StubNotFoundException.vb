Imports System

'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi

	''' <summary>
	''' A <code>StubNotFoundException</code> is thrown if a valid stub class
	''' could not be found for a remote object when it is exported.
	''' A <code>StubNotFoundException</code> may also be
	''' thrown when an activatable object is registered via the
	''' <code>java.rmi.activation.Activatable.register</code> method.
	''' 
	''' @author  Roger Riggs
	''' @since   JDK1.1 </summary>
	''' <seealso cref=     java.rmi.server.UnicastRemoteObject </seealso>
	''' <seealso cref=     java.rmi.activation.Activatable </seealso>
	Public Class StubNotFoundException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -7088199405468872373L

		''' <summary>
		''' Constructs a <code>StubNotFoundException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a <code>StubNotFoundException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the nested exception
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String, ByVal ex As Exception)
			MyBase.New(s, ex)
		End Sub
	End Class

End Namespace