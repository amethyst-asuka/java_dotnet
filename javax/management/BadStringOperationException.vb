Imports System

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management

	''' <summary>
	''' Thrown when an invalid string operation is passed
	''' to a method for constructing a query.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class BadStringOperationException
		Inherits Exception


		' Serial version 
		Private Const serialVersionUID As Long = 7802201238441662100L

		''' <summary>
		''' @serial The description of the operation that originated this exception
		''' </summary>
		Private op As String

		''' <summary>
		''' Constructs a <CODE>BadStringOperationException</CODE> with the specified detail
		''' message.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			Me.op = message
		End Sub


		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "BadStringOperationException: " & op
		End Function

	End Class

End Namespace