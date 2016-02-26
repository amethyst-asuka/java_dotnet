'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A generic credential exception.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class CredentialException
		Inherits LoginException

		Private Const serialVersionUID As Long = -4772893876810601859L

		''' <summary>
		''' Constructs a CredentialException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CredentialException with the specified detail message.
		''' A detail message is a String that describes this particular
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