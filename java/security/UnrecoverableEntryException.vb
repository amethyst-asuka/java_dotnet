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

Namespace java.security

	''' <summary>
	''' This exception is thrown if an entry in the keystore cannot be recovered.
	''' 
	''' 
	''' @since 1.5
	''' </summary>

	Public Class UnrecoverableEntryException
		Inherits GeneralSecurityException

		Private Shadows Const serialVersionUID As Long = -4527142945246286535L

		''' <summary>
		''' Constructs an UnrecoverableEntryException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an UnrecoverableEntryException with the specified detail
		''' message, which provides more information about why this exception
		''' has been thrown.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
	   Public Sub New(  msg As String)
		   MyBase.New(msg)
	   End Sub
	End Class

End Namespace