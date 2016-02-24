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
	''' Base class for character conversion exceptions.
	''' 
	''' @author      Asmus Freytag
	''' @since       JDK1.1
	''' </summary>
	Public Class CharConversionException
		Inherits java.io.IOException

		Private Shadows Const serialVersionUID As Long = -8680016352018427031L

		''' <summary>
		''' This provides no detailed message.
		''' </summary>
		Public Sub New()
		End Sub
		''' <summary>
		''' This provides a detailed message.
		''' </summary>
		''' <param name="s"> the detailed message associated with the exception. </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace