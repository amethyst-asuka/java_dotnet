Imports System

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

Namespace java.rmi.server

	''' <summary>
	''' An obsolete subclass of <seealso cref="ExportException"/>.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' @deprecated This class is obsolete. Use <seealso cref="ExportException"/> instead. 
	<Obsolete("This class is obsolete. Use <seealso cref="ExportException"/> instead.")> _
	Public Class SocketSecurityException
		Inherits ExportException

		' indicate compatibility with JDK 1.1.x version of class 
		Private Shadows Const serialVersionUID As Long = -7622072999407781979L

		''' <summary>
		''' Constructs an <code>SocketSecurityException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message.
		''' @since JDK1.1 </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an <code>SocketSecurityException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		''' <param name="ex"> the nested exception
		''' @since JDK1.1 </param>
		Public Sub New(  s As String,   ex As Exception)
			MyBase.New(s, ex)
		End Sub

	End Class

End Namespace