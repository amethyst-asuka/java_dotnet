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
	''' Signals that user authentication failed.
	''' 
	''' <p> This exception is thrown by LoginModules if authentication failed.
	''' For example, a {@code LoginModule} throws this exception if
	''' the user entered an incorrect password.
	''' 
	''' </summary>
	Public Class FailedLoginException
		Inherits LoginException

		Private Const serialVersionUID As Long = 802556922354616286L

		''' <summary>
		''' Constructs a FailedLoginException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a FailedLoginException with the specified detail
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