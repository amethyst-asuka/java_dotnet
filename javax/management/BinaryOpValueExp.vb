Imports System

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
	Friend Class BinaryOpValueExp
		Inherits QueryEval
		Implements ValueExp

		' Serial version 
		Private Const serialVersionUID As Long = 1216286847881456786L

		''' <summary>
		''' @serial The operator
		''' </summary>
		Private op As Integer

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
		''' Creates a new BinaryOpValueExp using operator o applied on v1 and
		''' v2 values.
		''' </summary>
		Public Sub New(ByVal o As Integer, ByVal v1 As ValueExp, ByVal v2 As ValueExp)
			op = o
			exp1 = v1
			exp2 = v2
		End Sub


		''' <summary>
		''' Returns the operator of the value expression.
		''' </summary>
		Public Overridable Property [operator] As Integer
			Get
				Return op
			End Get
		End Property

		''' <summary>
		''' Returns the left value of the value expression.
		''' </summary>
		Public Overridable Property leftValue As ValueExp
			Get
				Return exp1
			End Get
		End Property

		''' <summary>
		''' Returns the right value of the value expression.
		''' </summary>
		Public Overridable Property rightValue As ValueExp
			Get
				Return exp2
			End Get
		End Property

		''' <summary>
		''' Applies the BinaryOpValueExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the BinaryOpValueExp will be applied.
		''' </param>
		''' <returns>  The ValueExp.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As ValueExp Implements ValueExp.apply
			Dim val1 As ValueExp = exp1.apply(name)
			Dim val2 As ValueExp = exp2.apply(name)
			Dim sval1 As String
			Dim sval2 As String
			Dim dval1 As Double
			Dim dval2 As Double
			Dim lval1 As Long
			Dim lval2 As Long
			Dim numeric As Boolean = TypeOf val1 Is NumericValueExp

			If numeric Then
				If CType(val1, NumericValueExp).long Then
					lval1 = CType(val1, NumericValueExp)
					lval2 = CType(val2, NumericValueExp)

					Select Case op
					Case Query.___PLUS
						Return Query.value(lval1 + lval2)
					Case Query.___TIMES
						Return Query.value(lval1 * lval2)
					Case Query.___MINUS
						Return Query.value(lval1 - lval2)
					Case Query.___DIV
						Return Query.value(lval1 \ lval2)
					End Select

				Else
					dval1 = CType(val1, NumericValueExp)
					dval2 = CType(val2, NumericValueExp)

					Select Case op
					Case Query.___PLUS
						Return Query.value(dval1 + dval2)
					Case Query.___TIMES
						Return Query.value(dval1 * dval2)
					Case Query.___MINUS
						Return Query.value(dval1 - dval2)
					Case Query.___DIV
						Return Query.value(dval1 / dval2)
					End Select
				End If
			Else
				sval1 = CType(val1, StringValueExp).value
				sval2 = CType(val2, StringValueExp).value

				Select Case op
				Case Query.___PLUS
					Return New StringValueExp(sval1 + sval2)
				Case Else
					Throw New BadStringOperationException(opString())
				End Select
			End If

			Throw New BadBinaryOpValueExpException(Me)
		End Function

		''' <summary>
		''' Returns the string representing the object
		''' </summary>
		Public Overrides Function ToString() As String
			Try
				Return parens(exp1, True) & " " & opString() & " " & parens(exp2, False)
			Catch ex As BadBinaryOpValueExpException
				Return "invalid expression"
			End Try
		End Function

	'    
	'     * Add parentheses to the given subexpression if necessary to
	'     * preserve meaning.  Suppose this BinaryOpValueExp is
	'     * Query.times(Query.plus(Query.attr("A"), Query.attr("B")), Query.attr("C")).
	'     * Then the original toString() logic would return A + B * C.
	'     * We check precedences in order to return (A + B) * C, which is the
	'     * meaning of the ValueExp.
	'     *
	'     * We need to add parentheses if the unparenthesized expression would
	'     * be parsed as a different ValueExp from the original.
	'     * We cannot omit parentheses even when mathematically
	'     * the result would be equivalent, because we do not know whether the
	'     * numeric values will be integer or floating-point.  Addition and
	'     * multiplication are associative for integers but not always for
	'     * floating-point.
	'     *
	'     * So the rule is that we omit parentheses if the ValueExp
	'     * is (A op1 B) op2 C and the precedence of op1 is greater than or
	'     * equal to that of op2; or if the ValueExp is A op1 (B op2 C) and
	'     * the precedence of op2 is greater than that of op1.  (There are two
	'     * precedences: that of * and / is greater than that of + and -.)
	'     * The case of (A op1 B) op2 (C op3 D) applies each rule in turn.
	'     *
	'     * The following examples show the rules in action.  On the left,
	'     * the original ValueExp.  On the right, the string representation.
	'     *
	'     * (A + B) + C     A + B + C
	'     * (A * B) + C     A * B + C
	'     * (A + B) * C     (A + B) * C
	'     * (A * B) * C     A * B * C
	'     * A + (B + C)     A + (B + C)
	'     * A + (B * C)     A + B * C
	'     * A * (B + C)     A * (B + C)
	'     * A * (B * C)     A * (B * C)
	'     
		Private Function parens(ByVal subexp As ValueExp, ByVal left As Boolean) As String
			Dim omit As Boolean
			If TypeOf subexp Is BinaryOpValueExp Then
				Dim subop As Integer = CType(subexp, BinaryOpValueExp).op
				If left Then
					omit = (precedence(subop) >= precedence(op))
				Else
					omit = (precedence(subop) > precedence(op))
				End If
			Else
				omit = True
			End If

			If omit Then
				Return subexp.ToString()
			Else
				Return "(" & subexp & ")"
			End If
		End Function

		Private Function precedence(ByVal xop As Integer) As Integer
			Select Case xop
				Case Query.___PLUS, Query.___MINUS
						Return 0
				Case Query.___TIMES, Query.___DIV
						Return 1
				Case Else
					Throw New BadBinaryOpValueExpException(Me)
			End Select
		End Function

		Private Function opString() As String
			Select Case op
			Case Query.___PLUS
				Return "+"
			Case Query.___TIMES
				Return "*"
			Case Query.___MINUS
				Return "-"
			Case Query.___DIV
				Return "/"
			End Select

			Throw New BadBinaryOpValueExpException(Me)
		End Function

		<Obsolete> _
		Public Overrides Property mBeanServer Implements ValueExp.setMBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
				MyBase.mBeanServer = s
			End Set
		End Property
	End Class

End Namespace