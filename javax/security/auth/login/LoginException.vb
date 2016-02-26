'
' * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' This is the basic login exception.
	''' </summary>
	''' <seealso cref= javax.security.auth.login.LoginContext </seealso>

	Public Class LoginException
		Inherits java.security.GeneralSecurityException

		Private Const serialVersionUID As Long = -4679091624035232488L

		''' <summary>
		''' Constructs a LoginException with no detail message. A detail
		''' message is a String that describes this particular exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a LoginException with the specified detail message.
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