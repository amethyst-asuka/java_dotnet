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
	''' Unchecked exception thrown when duplicate flags are provided in the format
	''' specifier.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class DuplicateFormatFlagsException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 18890531L

		Private flags As String

		''' <summary>
		''' Constructs an instance of this class with the specified flags.
		''' </summary>
		''' <param name="f">
		'''         The set of format flags which contain a duplicate flag. </param>
		Public Sub New(  f As String)
			If f Is Nothing Then Throw New NullPointerException
			Me.flags = f
		End Sub

		''' <summary>
		''' Returns the set of flags which contains a duplicate flag.
		''' </summary>
		''' <returns>  The flags </returns>
		Public Overridable Property flags As String
			Get
				Return flags
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return String.Format("Flags = '{0}'", flags)
			End Get
		End Property
	End Class

End Namespace