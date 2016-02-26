Imports System.Text

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
	Friend Class InQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -5801329450358952434L

		''' <summary>
		''' @serial The <seealso cref="ValueExp"/> to be found
		''' </summary>
		Private val As ValueExp

		''' <summary>
		''' @serial The array of <seealso cref="ValueExp"/> to be searched
		''' </summary>
		Private valueList As ValueExp()


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new InQueryExp with the specified ValueExp to be found in
		''' a specified array of ValueExp.
		''' </summary>
		Public Sub New(ByVal v1 As ValueExp, ByVal items As ValueExp())
			val = v1
			valueList = items
		End Sub


		''' <summary>
		''' Returns the checked value of the query.
		''' </summary>
		Public Overridable Property checkedValue As ValueExp
			Get
				Return val
			End Get
		End Property

		''' <summary>
		''' Returns the array of values of the query.
		''' </summary>
		Public Overridable Property explicitValues As ValueExp()
			Get
				Return valueList
			End Get
		End Property

		''' <summary>
		''' Applies the InQueryExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the InQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply
			If valueList IsNot Nothing Then
				Dim v As ValueExp = val.apply(name)
				Dim numeric As Boolean = TypeOf v Is NumericValueExp

				For Each element As ValueExp In valueList
					element = element.apply(name)
					If numeric Then
						If CType(element, NumericValueExp) = CType(v, NumericValueExp) Then Return True
					Else
						If CType(element, StringValueExp).value.Equals(CType(v, StringValueExp).value) Then Return True
					End If
				Next element
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return val & " in (" & generateValueList() & ")"
		End Function


		Private Function generateValueList() As String
			If valueList Is Nothing OrElse valueList.Length = 0 Then Return ""

			Dim result As New StringBuilder(valueList(0).ToString())

			For i As Integer = 1 To valueList.Length - 1
				result.Append(", ")
				result.Append(valueList(i))
			Next i

			Return result.ToString()
		End Function

	End Class

End Namespace