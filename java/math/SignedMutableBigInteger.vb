'
' * Copyright (c) 1999, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.math

	''' <summary>
	''' A class used to represent multiprecision integers that makes efficient
	''' use of allocated space by allowing a number to occupy only part of
	''' an array so that the arrays do not have to be reallocated as often.
	''' When performing an operation with many iterations the array used to
	''' hold a number is only increased when necessary and does not have to
	''' be the same size as the number it represents. A mutable number allows
	''' calculations to occur on the same number without having to create
	''' a new number for every step of the calculation as occurs with
	''' BigIntegers.
	''' 
	''' Note that SignedMutableBigIntegers only support signed addition and
	''' subtraction. All other operations occur as with MutableBigIntegers.
	''' </summary>
	''' <seealso cref=     BigInteger
	''' @author  Michael McCloskey
	''' @since   1.3 </seealso>

	Friend Class SignedMutableBigInteger
		Inherits MutableBigInteger

	   ''' <summary>
	   ''' The sign of this MutableBigInteger.
	   ''' </summary>
		Friend sign As Integer = 1

		' Constructors

		''' <summary>
		''' The default constructor. An empty MutableBigInteger is created with
		''' a one word capacity.
		''' </summary>
		Friend Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with a magnitude specified by
		''' the int val.
		''' </summary>
		Friend Sub New(ByVal val As Integer)
			MyBase.New(val)
		End Sub

		''' <summary>
		''' Construct a new MutableBigInteger with a magnitude equal to the
		''' specified MutableBigInteger.
		''' </summary>
		Friend Sub New(ByVal val As MutableBigInteger)
			MyBase.New(val)
		End Sub

	   ' Arithmetic Operations

	   ''' <summary>
	   ''' Signed addition built upon unsigned add and subtract.
	   ''' </summary>
		Friend Overridable Sub signedAdd(ByVal addend As SignedMutableBigInteger)
			If sign = addend.sign Then
				add(addend)
			Else
				sign = sign * subtract(addend)
			End If

		End Sub

	   ''' <summary>
	   ''' Signed addition built upon unsigned add and subtract.
	   ''' </summary>
		Friend Overridable Sub signedAdd(ByVal addend As MutableBigInteger)
			If sign = 1 Then
				add(addend)
			Else
				sign = sign * subtract(addend)
			End If

		End Sub

	   ''' <summary>
	   ''' Signed subtraction built upon unsigned add and subtract.
	   ''' </summary>
		Friend Overridable Sub signedSubtract(ByVal addend As SignedMutableBigInteger)
			If sign = addend.sign Then
				sign = sign * subtract(addend)
			Else
				add(addend)
			End If

		End Sub

	   ''' <summary>
	   ''' Signed subtraction built upon unsigned add and subtract.
	   ''' </summary>
		Friend Overridable Sub signedSubtract(ByVal addend As MutableBigInteger)
			If sign = 1 Then
				sign = sign * subtract(addend)
			Else
				add(addend)
			End If
			If intLen = 0 Then sign = 1
		End Sub

		''' <summary>
		''' Print out the first intLen ints of this MutableBigInteger's value
		''' array starting at offset.
		''' </summary>
		Public Overrides Function ToString() As String
			Return Me.toBigInteger(sign).ToString()
		End Function

	End Class

End Namespace