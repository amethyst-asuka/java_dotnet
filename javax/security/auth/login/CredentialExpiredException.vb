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
	''' Signals that a {@code Credential} has expired.
	''' 
	''' <p> This exception is thrown by LoginModules when they determine
	''' that a {@code Credential} has expired.
	''' For example, a {@code LoginModule} authenticating a user
	''' in its {@code login} method may determine that the user's
	''' password, although entered correctly, has expired.  In this case
	''' the {@code LoginModule} throws this exception to notify
	''' the application.  The application can then take the appropriate
	''' steps to assist the user in updating the password.
	''' 
	''' </summary>
	Public Class CredentialExpiredException
		Inherits CredentialException

		Private Const serialVersionUID As Long = -5344739593859737937L

		''' <summary>
		''' Constructs a CredentialExpiredException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CredentialExpiredException with the specified detail
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