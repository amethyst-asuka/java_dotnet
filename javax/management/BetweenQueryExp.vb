'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used by the query-building mechanism to represent binary
	''' relations.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class BetweenQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -2933597532866307444L

		''' <summary>
		''' @serial The checked value
		''' </summary>
		Private exp1 As ValueExp

		''' <summary>
		''' @serial The lower bound value
		''' </summary>
		Private exp2 As ValueExp

		''' <summary>
		''' @serial The upper bound value
		''' </summary>
		Private exp3 As ValueExp


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new BetweenQueryExp with v1 checked value, v2 lower bound
		''' and v3 upper bound values.
		''' </summary>
		Public Sub New(ByVal v1 As ValueExp, ByVal v2 As ValueExp, ByVal v3 As ValueExp)
			exp1 = v1
			exp2 = v2
			exp3 = v3
		End Sub


		''' <summary>
		''' Returns the checked value of the query.
		''' </summary>
		Public Overridable Property checkedValue As ValueExp
			Get
				Return exp1
			End Get
		End Property

		''' <summary>
		''' Returns the lower bound value of the query.
		''' </summary>
		Public Overridable Property lowerBound As ValueExp
			Get
				Return exp2
			End Get
		End Property

		''' <summary>
		''' Returns the upper bound value of the query.
		''' </summary>
		Public Overridable Property upperBound As ValueExp
			Get
				Return exp3
			End Get
		End Property

		''' <summary>
		''' Applies the BetweenQueryExp on an MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the BetweenQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			Dim val1 As ValueExp = exp1.apply(name)
			Dim val2 As ValueExp = exp2.apply(name)
			Dim val3 As ValueExp = exp3.apply(name)
			Dim numeric As Boolean = TypeOf val1 Is NumericValueExp

			If numeric Then
				If CType(val1, NumericValueExp).long Then
					Dim lval1 As Long = CType(val1, NumericValueExp)
					Dim lval2 As Long = CType(val2, NumericValueExp)
					Dim lval3 As Long = CType(val3, NumericValueExp)
					Return lval2 <= lval1 AndAlso lval1 <= lval3
				Else
					Dim dval1 As Double = CType(val1, NumericValueExp)
					Dim dval2 As Double = CType(val2, NumericValueExp)
					Dim dval3 As Double = CType(val3, NumericValueExp)
					Return dval2 <= dval1 AndAlso dval1 <= dval3
				End If

			Else
				Dim sval1 As String = CType(val1, StringValueExp).value
				Dim sval2 As String = CType(val2, StringValueExp).value
				Dim sval3 As String = CType(val3, StringValueExp).value
				Return sval2.CompareTo(sval1) <= 0 AndAlso sval1.CompareTo(sval3) <= 0
			End If
		End Function

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "(" & exp1 & ") between (" & exp2 & ") and (" & exp3 & ")"
		End Function
	End Class

End Namespace