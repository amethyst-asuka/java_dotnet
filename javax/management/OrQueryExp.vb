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
	''' This class is used by the query-building mechanism to represent
	''' disjunctions of relational expressions.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class OrQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = 2962973084421716523L

		''' <summary>
		''' @serial The left query expression
		''' </summary>
		Private exp1 As QueryExp

		''' <summary>
		''' @serial The right query expression
		''' </summary>
		Private exp2 As QueryExp


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new OrQueryExp with the specified ValueExps
		''' </summary>
		Public Sub New(ByVal q1 As QueryExp, ByVal q2 As QueryExp)
			exp1 = q1
			exp2 = q2
		End Sub


		''' <summary>
		''' Returns the left query expression.
		''' </summary>
		Public Overridable Property leftExp As QueryExp
			Get
				Return exp1
			End Get
		End Property

		''' <summary>
		''' Returns the right query expression.
		''' </summary>
		Public Overridable Property rightExp As QueryExp
			Get
				Return exp2
			End Get
		End Property

		''' <summary>
		''' Applies the OrQueryExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the OrQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' 
		''' </returns>
		''' <exception cref="BadStringOperationException"> The string passed to the method is invalid. </exception>
		''' <exception cref="BadBinaryOpValueExpException"> The expression passed to the method is invalid. </exception>
		''' <exception cref="BadAttributeValueExpException"> The attribute value passed to the method is invalid. </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			Return exp1.apply(name) OrElse exp2.apply(name)
		End Function

		''' <summary>
		''' Returns a string representation of this OrQueryExp
		''' </summary>
		Public Overrides Function ToString() As String
			Return "(" & exp1 & ") or (" & exp2 & ")"
		End Function
	End Class

End Namespace