Imports System

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
	''' Unchecked exception thrown when the format width is a negative value other
	''' than <tt>-1</tt> or is otherwise unsupported.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class IllegalFormatWidthException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 16660902L

		Private w As Integer

		''' <summary>
		''' Constructs an instance of this class with the specified width.
		''' </summary>
		''' <param name="w">
		'''         The width </param>
		Public Sub New(ByVal w As Integer)
			Me.w = w
		End Sub

		''' <summary>
		''' Returns the width
		''' </summary>
		''' <returns>  The width </returns>
		Public Overridable Property width As Integer
			Get
				Return w
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return Convert.ToString(w)
			End Get
		End Property
	End Class

End Namespace