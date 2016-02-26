'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that a user account has expired.
	''' 
	''' <p> This exception is thrown by LoginModules when they determine
	''' that an account has expired.  For example, a {@code LoginModule},
	''' after successfully authenticating a user, may determine that the
	''' user's account has expired.  In this case the {@code LoginModule}
	''' throws this exception to notify the application.  The application can
	''' then take the appropriate steps to notify the user.
	''' 
	''' </summary>
	Public Class AccountExpiredException
		Inherits AccountException

		Private Const serialVersionUID As Long = -6064064890162661560L

		''' <summary>
		''' Constructs a AccountExpiredException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a AccountExpiredException with the specified detail
		''' message.  A detail message is a String that describes this particular
		''' exception.
		''' 
		''' <p>
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace