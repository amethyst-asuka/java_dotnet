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
	''' Unchecked exception thrown when an illegal combination flags is given.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class IllegalFormatFlagsException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 790824L

		Private flags As String

		''' <summary>
		''' Constructs an instance of this class with the specified flags.
		''' </summary>
		''' <param name="f">
		'''         The set of format flags which contain an illegal combination </param>
		Public Sub New(ByVal f As String)
			If f Is Nothing Then Throw New NullPointerException
			Me.flags = f
		End Sub

		''' <summary>
		''' Returns the set of flags which contains an illegal combination.
		''' </summary>
		''' <returns>  The flags </returns>
		Public Overridable Property flags As String
			Get
				Return flags
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return "Flags = '" & flags & "'"
			End Get
		End Property
	End Class

End Namespace