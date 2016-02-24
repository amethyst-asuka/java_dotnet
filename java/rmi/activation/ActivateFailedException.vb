Imports System

'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is thrown by the RMI runtime when activation
	''' fails during a remote call to an activatable object.
	''' 
	''' @author      Ann Wollrath
	''' @since       1.2
	''' </summary>
	Public Class ActivateFailedException
		Inherits java.rmi.RemoteException

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Shadows Const serialVersionUID As Long = 4863550261346652506L

		''' <summary>
		''' Constructs an <code>ActivateFailedException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="s"> the detail message
		''' @since 1.2 </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs an <code>ActivateFailedException</code> with the specified
		''' detail message and nested exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="ex"> the nested exception
		''' @since 1.2 </param>
		Public Sub New(ByVal s As String, ByVal ex As Exception)
			MyBase.New(s, ex)
		End Sub
	End Class

End Namespace