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
	''' Unchecked exception thrown when no support is available
	''' for a requested charset.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class UnsupportedCharsetException
		Inherits IllegalArgumentException

		Private Shadows Const serialVersionUID As Long = 1490765524727386367L

		Private charsetName As String

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="charsetName">
		'''         The name of the unsupported charset </param>
		Public Sub New(  charsetName As String)
			MyBase.New(Convert.ToString(charsetName))
		Me.charsetName = charsetName
		End Sub

		''' <summary>
		''' Retrieves the name of the unsupported charset.
		''' </summary>
		''' <returns>  The name of the unsupported charset </returns>
		Public Overridable Property charsetName As String
			Get
				Return charsetName
			End Get
		End Property

	End Class

End Namespace