'
' * Copyright (c) 2003, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Unchecked exception thrown when the argument corresponding to the format
	''' specifier is of an incompatible type.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class IllegalFormatConversionException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 17000126L

		Private c As Char
		Private arg As  [Class]

		''' <summary>
		''' Constructs an instance of this class with the mismatched conversion and
		''' the corresponding argument class.
		''' </summary>
		''' <param name="c">
		'''         Inapplicable conversion
		''' </param>
		''' <param name="arg">
		'''         Class of the mismatched argument </param>
		Public Sub New(ByVal c As Char, ByVal arg As [Class])
			If arg Is Nothing Then Throw New NullPointerException
			Me.c = c
			Me.arg = arg
		End Sub

		''' <summary>
		''' Returns the inapplicable conversion.
		''' </summary>
		''' <returns>  The inapplicable conversion </returns>
		Public Overridable Property conversion As Char
			Get
				Return c
			End Get
		End Property

		''' <summary>
		''' Returns the class of the mismatched argument.
		''' </summary>
		''' <returns>   The class of the mismatched argument </returns>
		Public Overridable Property argumentClass As  [Class]
			Get
				Return arg
			End Get
		End Property

		' javadoc inherited from Throwable.java
		Public  Overrides ReadOnly Property  message As String
			Get
				Return String.Format("{0} != {1}", c, arg.name)
			End Get
		End Property
	End Class

End Namespace