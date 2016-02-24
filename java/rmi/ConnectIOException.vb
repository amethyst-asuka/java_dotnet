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
	''' A <code>ConnectIOException</code> is thrown if an
	''' <code>IOException</code> occurs while making a connection
	''' to the remote host for a remote method call.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1
	''' </summary>
	Public Class ConnectIOException
		Inherits RemoteException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -8087809532704668744L

		''' <summary>
		''' Constructs a <code>ConnectIOException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since JDK1.1 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub


		''' <summary>
		''' Constructs a <code>ConnectIOException</code> with the specified
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