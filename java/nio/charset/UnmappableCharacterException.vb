'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.charset


	''' <summary>
	''' Checked exception thrown when an input character (or byte) sequence
	''' is valid but cannot be mapped to an output byte (or character)
	''' sequence.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class UnmappableCharacterException
		Inherits CharacterCodingException

		Private Shadows Const serialVersionUID As Long = -7026962371537706123L

		Private inputLength As Integer

		''' <summary>
		''' Constructs an {@code UnmappableCharacterException} with the
		''' given length. </summary>
		''' <param name="inputLength"> the length of the input </param>
		Public Sub New(  inputLength As Integer)
			Me.inputLength = inputLength
		End Sub

		''' <summary>
		''' Returns the length of the input. </summary>
		''' <returns> the length of the input </returns>
		Public Overridable Property inputLength As Integer
			Get
				Return inputLength
			End Get
		End Property

		''' <summary>
		''' Returns the message. </summary>
		''' <returns> the message </returns>
		Public  Overrides ReadOnly Property  message As String
			Get
				Return "Input length = " & inputLength
			End Get
		End Property

	End Class

End Namespace