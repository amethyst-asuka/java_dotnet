'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.login

	''' <summary>
	''' Signals that an account was not found.
	''' 
	''' <p> This exception may be thrown by a LoginModule if it is unable
	''' to locate an account necessary to perform authentication.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AccountNotFoundException
		Inherits AccountException

		Private Const serialVersionUID As Long = 1498349563916294614L

		''' <summary>
		''' Constructs a AccountNotFoundException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a AccountNotFoundException with the specified
		''' detail message. A detail message is a String that describes
		''' this particular exception.
		''' 
		''' <p>
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace