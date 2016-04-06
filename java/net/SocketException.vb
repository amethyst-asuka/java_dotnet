'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net


	''' <summary>
	''' Thrown to indicate that there is an error creating or accessing a Socket.
	''' 
	''' @author  Jonathan Payne
	''' @since   JDK1.0
	''' </summary>
	Public Class SocketException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -5935874303556886934L

		''' <summary>
		''' Constructs a new {@code SocketException} with the
		''' specified detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub

		''' <summary>
		''' Constructs a new {@code SocketException} with no detail message.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace