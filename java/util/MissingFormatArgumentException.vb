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
	''' Unchecked exception thrown when there is a format specifier which does not
	''' have a corresponding argument or if an argument index refers to an argument
	''' that does not exist.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MissingFormatArgumentException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 19190115L

		Private s As String

		''' <summary>
		''' Constructs an instance of this class with the unmatched format
		''' specifier.
		''' </summary>
		''' <param name="s">
		'''         Format specifier which does not have a corresponding argument </param>
		Public Sub New(  s As String)
			If s Is Nothing Then Throw New NullPointerException
			Me.s = s
		End Sub

		''' <summary>
		''' Returns the unmatched format specifier.
		''' </summary>
		''' <returns>  The unmatched format specifier </returns>
		Public Overridable Property formatSpecifier As String
			Get
				Return s
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return "Format specifier '" & s & "'"
			End Get
		End Property
	End Class

End Namespace