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
	''' Unchecked exception thrown when the precision is a negative value other than
	''' <tt>-1</tt>, the conversion does not support a precision, or the value is
	''' otherwise unsupported.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class IllegalFormatPrecisionException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 18711008L

		Private p As Integer

		''' <summary>
		''' Constructs an instance of this class with the specified precision.
		''' </summary>
		''' <param name="p">
		'''         The precision </param>
		Public Sub New(ByVal p As Integer)
			Me.p = p
		End Sub

		''' <summary>
		''' Returns the precision
		''' </summary>
		''' <returns>  The precision </returns>
		Public Overridable Property precision As Integer
			Get
				Return p
			End Get
		End Property

		Public Property Overrides message As String
			Get
				Return Convert.ToString(p)
			End Get
		End Property
	End Class

End Namespace