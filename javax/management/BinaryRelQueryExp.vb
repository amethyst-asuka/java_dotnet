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
	''' operations.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class BinaryRelQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -5690656271650491000L

		''' <summary>
		''' @serial The operator
		''' </summary>
		Private relOp As Integer

		''' <summary>
		''' @serial The first value
		''' </summary>
		Private exp1 As ValueExp

		''' <summary>
		''' @serial The second value
		''' </summary>
		Private exp2 As ValueExp


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new BinaryRelQueryExp with operator op applied on v1 and
		''' v2 values.
		''' </summary>
		Public Sub New(ByVal op As Integer, ByVal v1 As ValueExp, ByVal v2 As ValueExp)
			relOp = op
			exp1 = v1
			exp2 = v2
		End Sub


		''' <summary>
		''' Returns the operator of the query.
		''' </summary>
		Public Overridable Property [operator] As Integer
			Get
				Return relOp
			End Get
		End Property

		''' <summary>
		''' Returns the left value of the query.
		''' </summary>
		Public Overridable Property leftValue As ValueExp
			Get
				Return exp1
			End Get
		End Property

		''' <summary>
		''' Returns the right value of the query.
		''' </summary>
		Public Overridable Property rightValue As ValueExp
			Get
				Return exp2
			End Get
		End Property

		''' <summary>
		''' Applies the BinaryRelQueryExp on an MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the BinaryRelQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			Dim val1 As Object = exp1.apply(name)
			Dim val2 As Object = exp2.apply(name)
			Dim numeric As Boolean = TypeOf val1 Is NumericValueExp
			Dim bool As Boolean = TypeOf val1 Is BooleanValueExp
			If numeric Then
				If CType(val1, NumericValueExp).long Then
					Dim lval1 As Long = CType(val1, NumericValueExp)
					Dim lval2 As Long = CType(val2, NumericValueExp)

					Select Case relOp
					Case Query.___GT
						Return lval1 > lval2
					Case Query.___LT
						Return lval1 < lval2
					Case Query.GE
						Return lval1 >= lval2
					Case Query.LE
						Return lval1 <= lval2
					Case Query.___EQ
						Return lval1 = lval2
					End Select
				Else
					Dim dval1 As Double = CType(val1, NumericValueExp)
					Dim dval2 As Double = CType(val2, NumericValueExp)

					Select Case relOp
					Case Query.___GT
						Return dval1 > dval2
					Case Query.___LT
						Return dval1 < dval2
					Case Query.GE
						Return dval1 >= dval2
					Case Query.LE
						Return dval1 <= dval2
					Case Query.___EQ
						Return dval1 = dval2
					End Select
				End If

			ElseIf bool Then

				Dim bval1 As Boolean = CType(val1, BooleanValueExp).value
				Dim bval2 As Boolean = CType(val2, BooleanValueExp).value

				Select Case relOp
				Case Query.___GT
					Return bval1 AndAlso Not bval2
				Case Query.___LT
					Return (Not bval1) AndAlso bval2
				Case Query.GE
					Return bval1 OrElse Not bval2
				Case Query.LE
					Return (Not bval1) OrElse bval2
				Case Query.___EQ
					Return bval1 = bval2
				End Select

			Else
				Dim sval1 As String = CType(val1, StringValueExp).value
				Dim sval2 As String = CType(val2, StringValueExp).value

				Select Case relOp
				Case Query.___GT
					Return sval1.CompareTo(sval2) > 0
				Case Query.___LT
					Return sval1.CompareTo(sval2) < 0
				Case Query.GE
					Return sval1.CompareTo(sval2) >= 0
				Case Query.LE
					Return sval1.CompareTo(sval2) <= 0
				Case Query.___EQ
					Return sval1.CompareTo(sval2) = 0
				End Select
			End If

			Return False
		End Function

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "(" & exp1 & ") " & relOpString() & " (" & exp2 & ")"
		End Function

		Private Function relOpString() As String
			Select Case relOp
			Case Query.___GT
				Return ">"
			Case Query.___LT
				Return "<"
			Case Query.GE
				Return ">="
			Case Query.LE
				Return "<="
			Case Query.___EQ
				Return "="
			End Select

			Return "="
		End Function

	End Class

End Namespace