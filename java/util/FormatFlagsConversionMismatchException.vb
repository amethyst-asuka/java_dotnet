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

Namespace java.util

	''' <summary>
	''' Unchecked exception thrown when a conversion and flag are incompatible.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class FormatFlagsConversionMismatchException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 19120414L

		Private f As String

		Private c As Char

		''' <summary>
		''' Constructs an instance of this class with the specified flag
		''' and conversion.
		''' </summary>
		''' <param name="f">
		'''         The flag
		''' </param>
		''' <param name="c">
		'''         The conversion </param>
		Public Sub New(ByVal f As String, ByVal c As Char)
			If f Is Nothing Then Throw New NullPointerException
			Me.f = f
			Me.c = c
		End Sub

		''' <summary>
		''' Returns the incompatible flag.
		''' </summary>
		''' <returns>  The flag </returns>
		 Public Overridable Property flags As String
			 Get
				Return f
			 End Get
		 End Property

		''' <summary>
		''' Returns the incompatible conversion.
		''' </summary>
		''' <returns>  The conversion </returns>
		Public Overridable Property conversion As Char
			Get
				Return c
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return "Conversion = " & AscW(c) & ", Flags = " & f
			End Get
		End Property
	End Class

End Namespace