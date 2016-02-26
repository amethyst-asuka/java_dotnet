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
	''' Thrown when an invalid expression is passed to a method for
	''' constructing a query.  This exception is used internally by JMX
	''' during the evaluation of a query.  User code does not usually see
	''' it.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class BadBinaryOpValueExpException
		Inherits Exception


		' Serial version 
		Private Const serialVersionUID As Long = 5068475589449021227L

		''' <summary>
		''' @serial the <seealso cref="ValueExp"/> that originated this exception
		''' </summary>
		Private exp As ValueExp


		''' <summary>
		''' Constructs a <CODE>BadBinaryOpValueExpException</CODE> with the specified <CODE>ValueExp</CODE>.
		''' </summary>
		''' <param name="exp"> the expression whose value was inappropriate. </param>
		Public Sub New(ByVal exp As ValueExp)
			Me.exp = exp
		End Sub


		''' <summary>
		''' Returns the <CODE>ValueExp</CODE> that originated the exception.
		''' </summary>
		''' <returns> the problematic <seealso cref="ValueExp"/>. </returns>
		Public Overridable Property exp As ValueExp
			Get
				Return exp
			End Get
		End Property

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "BadBinaryOpValueExpException: " & exp
		End Function

	End Class

End Namespace