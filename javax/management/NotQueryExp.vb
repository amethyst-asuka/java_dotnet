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
	''' This class is used by the query-building mechanism to represent negations
	''' of relational expressions.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class NotQueryExp
		Inherits QueryEval
		Implements QueryExp


		' Serial version 
		Private Const serialVersionUID As Long = 5269643775896723397L

		''' <summary>
		''' @serial The negated <seealso cref="QueryExp"/>
		''' </summary>
		Private exp As QueryExp


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new NotQueryExp for negating the specified QueryExp.
		''' </summary>
		Public Sub New(ByVal q As QueryExp)
			exp = q
		End Sub


		''' <summary>
		''' Returns the negated query expression of the query.
		''' </summary>
		Public Overridable Property negatedExp As QueryExp
			Get
				Return exp
			End Get
		End Property

		''' <summary>
		''' Applies the NotQueryExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the NotQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			Return exp.apply(name) = False
		End Function

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "not (" & exp & ")"
		End Function
	End Class

End Namespace