Imports System

'
' * Copyright (c) 1996, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' An <code>UnknownHostException</code> is thrown if a
	''' <code>java.net.UnknownHostException</code> occurs while creating
	''' a connection to the remote host for a remote method call.
	''' 
	''' @since   JDK1.1
	''' </summary>
	Public Class UnknownHostException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		 Private Shadows Const serialVersionUID As Long = -8152710247442114228L

		''' <summary>
		''' Constructs an <code>UnknownHostException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an <code>UnknownHostException</code> with the specified
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