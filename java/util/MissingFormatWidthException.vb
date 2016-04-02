'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Unchecked exception thrown when the format width is required.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MissingFormatWidthException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 15560123L

		Private s As String

		''' <summary>
		''' Constructs an instance of this class with the specified format
		''' specifier.
		''' </summary>
		''' <param name="s">
		'''         The format specifier which does not have a width </param>
		Public Sub New(ByVal s As String)
			If s Is Nothing Then Throw New NullPointerException
			Me.s = s
		End Sub

		''' <summary>
		''' Returns the format specifier which does not have a width.
		''' </summary>
		''' <returns>  The format specifier which does not have a width </returns>
		Public Overridable Property formatSpecifier As String
			Get
				Return s
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return s
			End Get
		End Property
	End Class

End Namespace