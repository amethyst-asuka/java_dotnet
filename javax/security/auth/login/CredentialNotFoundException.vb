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
	''' Signals that a credential was not found.
	''' 
	''' <p> This exception may be thrown by a LoginModule if it is unable
	''' to locate a credential necessary to perform authentication.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class CredentialNotFoundException
		Inherits CredentialException

		Private Const serialVersionUID As Long = -7779934467214319475L

		''' <summary>
		''' Constructs a CredentialNotFoundException with no detail message.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CredentialNotFoundException with the specified
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