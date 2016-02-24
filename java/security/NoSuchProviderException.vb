'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception is thrown when a particular security provider is
	''' requested but is not available in the environment.
	''' 
	''' @author Benjamin Renaud
	''' </summary>

	Public Class NoSuchProviderException
		Inherits GeneralSecurityException

		Private Shadows Const serialVersionUID As Long = 8488111756688534474L

		''' <summary>
		''' Constructs a NoSuchProviderException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a NoSuchProviderException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace