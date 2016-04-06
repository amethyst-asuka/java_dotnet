'
' * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is thrown if a key in the keystore cannot be recovered.
	''' 
	''' 
	''' @since 1.2
	''' </summary>

	Public Class UnrecoverableKeyException
		Inherits UnrecoverableEntryException

		Private Shadows Const serialVersionUID As Long = 7275063078190151277L

		''' <summary>
		''' Constructs an UnrecoverableKeyException with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an UnrecoverableKeyException with the specified detail
		''' message, which provides more information about why this exception
		''' has been thrown.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
	   Public Sub New(  msg As String)
		   MyBase.New(msg)
	   End Sub
	End Class

End Namespace