'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Unchecked exception thrown when a character with an invalid Unicode code
	''' point as defined by <seealso cref="Character#isValidCodePoint"/> is passed to the
	''' <seealso cref="Formatter"/>.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class IllegalFormatCodePointException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 19080630L

		Private c As Integer

		''' <summary>
		''' Constructs an instance of this class with the specified illegal code
		''' point as defined by <seealso cref="Character#isValidCodePoint"/>.
		''' </summary>
		''' <param name="c">
		'''         The illegal Unicode code point </param>
		Public Sub New(  c As Integer)
			Me.c = c
		End Sub

		''' <summary>
		''' Returns the illegal code point as defined by {@link
		''' Character#isValidCodePoint}.
		''' </summary>
		''' <returns>  The illegal Unicode code point </returns>
		Public Overridable Property codePoint As Integer
			Get
				Return c
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
	'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Return String.Format("Code point = %#x", c)
			End Get
		End Property
	End Class

End Namespace