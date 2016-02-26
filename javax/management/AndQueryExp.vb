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
	''' This class is used by the query building mechanism to represent conjunctions
	''' of relational expressions.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class AndQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -1081892073854801359L

		''' <summary>
		''' @serial The first QueryExp of the conjunction
		''' </summary>
		Private exp1 As QueryExp

		''' <summary>
		''' @serial The second QueryExp of the conjunction
		''' </summary>
		Private exp2 As QueryExp


		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new AndQueryExp with q1 and q2 QueryExp.
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
		''' Applies the AndQueryExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the AndQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' 
		''' </returns>
		''' <exception cref="BadStringOperationException"> The string passed to the method is invalid. </exception>
		''' <exception cref="BadBinaryOpValueExpException"> The expression passed to the method is invalid. </exception>
		''' <exception cref="BadAttributeValueExpException"> The attribute value passed to the method is invalid. </exception>
		''' <exception cref="InvalidApplicationException">  An attempt has been made to apply a subquery expression to a
		''' managed object or a qualified attribute expression to a managed object of the wrong class. </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			Return exp1.apply(name) AndAlso exp2.apply(name)
		End Function

	   ''' <summary>
	   ''' Returns a string representation of this AndQueryExp
	   ''' </summary>
		Public Overrides Function ToString() As String
			Return "(" & exp1 & ") and (" & exp2 & ")"
		End Function
	End Class

End Namespace