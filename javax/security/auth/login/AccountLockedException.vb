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
	''' Signals that an account was locked.
	''' 
	''' <p> This exception may be thrown by a LoginModule if it
	''' determines that authentication is being attempted on a
	''' locked account.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AccountLockedException
		Inherits AccountException

		Private Const serialVersionUID As Long = 8280345554014066334L

		''' <summary>
		''' Constructs a AccountLockedException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a AccountLockedException with the specified
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