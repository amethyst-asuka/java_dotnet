Imports System

'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
' *
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
' *
' 

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio.charset


	''' <summary>
	''' Unchecked exception thrown when a string that is not a
	''' <a href=Charset.html#names>legal charset name</a> is used as such.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class IllegalCharsetNameException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = 1457525358470002989L

		Private charsetName As String

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="charsetName">
		'''         The illegal charset name </param>
		Public Sub New(  charsetName As String)
			MyBase.New(Convert.ToString(charsetName))
		Me.charsetName = charsetName
		End Sub

		''' <summary>
		''' Retrieves the illegal charset name.
		''' </summary>
		''' <returns>  The illegal charset name </returns>
		Public Overridable Property charsetName As String
			Get
				Return charsetName
			End Get
		End Property

	End Class

End Namespace