'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.io

	''' <summary>
	''' The Character Encoding is not supported.
	''' 
	''' @author  Asmus Freytag
	''' @since   JDK1.1
	''' </summary>
	Public Class UnsupportedEncodingException
		Inherits IOException

		Private Shadows Const serialVersionUID As Long = -4274276298326136670L

		''' <summary>
		''' Constructs an UnsupportedEncodingException without a detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an UnsupportedEncodingException with a detail message. </summary>
		''' <param name="s"> Describes the reason for the exception. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace