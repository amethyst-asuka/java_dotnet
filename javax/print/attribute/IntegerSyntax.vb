Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.attribute


	''' <summary>
	''' Class IntegerSyntax is an abstract base class providing the common
	''' implementation of all attributes with integer values.
	''' <P>
	''' Under the hood, an integer attribute is just an integer. You can get an
	''' integer attribute's integer value by calling {@link #getValue()
	''' getValue()}. An integer attribute's integer value is
	''' established when it is constructed (see {@link #IntegerSyntax(int)
	''' IntegerSyntax(int)}). Once constructed, an integer attribute's
	''' value is immutable.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class IntegerSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = 3644574816328081943L

		''' <summary>
		''' This integer attribute's integer value.
		''' @serial
		''' </summary>
		Private value As Integer

		''' <summary>
		''' Construct a new integer attribute with the given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			Me.value = value
		End Sub

		''' <summary>
		''' Construct a new integer attribute with the given integer value, which
		''' must lie within the given range.
		''' </summary>
		''' <param name="value">       Integer value. </param>
		''' <param name="lowerBound">  Lower bound. </param>
		''' <param name="upperBound">  Upper bound.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>value</CODE> is less than
		'''     <CODE>lowerBound</CODE> or greater than
		'''     <CODE>upperBound</CODE>. </exception>
		Protected Friend Sub New(ByVal value As Integer, ByVal lowerBound As Integer, ByVal upperBound As Integer)
			If lowerBound > value OrElse value > upperBound Then Throw New System.ArgumentException("Value " & value & " not in range " & lowerBound & ".." & upperBound)
			Me.value = value
		End Sub

		''' <summary>
		''' Returns this integer attribute's integer value. </summary>
		''' <returns> the integer value </returns>
		Public Overridable Property value As Integer
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Returns whether this integer attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class IntegerSyntax.
		''' <LI>
		''' This integer attribute's value and <CODE>object</CODE>'s value are
		''' equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this integer
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean

			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is IntegerSyntax AndAlso value = CType([object], IntegerSyntax).value)
		End Function

		''' <summary>
		''' Returns a hash code value for this integer attribute. The hash code is
		''' just this integer attribute's integer value.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return value
		End Function

		''' <summary>
		''' Returns a string value corresponding to this integer attribute. The
		''' string value is just this integer attribute's integer value converted to
		''' a string.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "" & value
		End Function
	End Class

End Namespace