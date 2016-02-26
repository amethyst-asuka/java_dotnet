Imports System

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an attempt is made to apply either of the following: A
	''' subquery expression to an MBean or a qualified attribute expression
	''' to an MBean of the wrong class.  This exception is used internally
	''' by JMX during the evaluation of a query.  User code does not
	''' usually see it.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class InvalidApplicationException
		Inherits Exception


		' Serial version 
		Private Const serialVersionUID As Long = -3048022274675537269L

		''' <summary>
		''' @serial The object representing the class of the MBean
		''' </summary>
		Private val As Object


		''' <summary>
		''' Constructs an <CODE>InvalidApplicationException</CODE> with the specified <CODE>Object</CODE>.
		''' </summary>
		''' <param name="val"> the detail message of this exception. </param>
		Public Sub New(ByVal val As Object)
			Me.val = val
		End Sub
	End Class

End Namespace