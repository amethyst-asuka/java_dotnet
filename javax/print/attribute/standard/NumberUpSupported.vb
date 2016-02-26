Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.print.attribute.standard


	''' <summary>
	''' Class NumberUpSupported is a printing attribute class, a set of integers,
	''' that gives the supported values for a <seealso cref="NumberUp NumberUp"/> attribute.
	''' <P>
	''' <B>IPP Compatibility:</B> The NumberUpSupported attribute's canonical array
	''' form gives the lower and upper bound for each range of number-up to be
	''' included in an IPP "number-up-supported" attribute. See class {@link
	''' javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax} for an
	''' explanation of canonical array form. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class NumberUpSupported
		Inherits javax.print.attribute.SetOfIntegerSyntax
		Implements javax.print.attribute.SupportedValuesAttribute

		 Private Const serialVersionUID As Long = -1041573395759141805L


		''' <summary>
		''' Construct a new number up supported attribute with the given members.
		''' The supported values for NumberUp are specified in "array form;" see
		''' class
		''' <seealso cref="javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax"/>
		''' for an explanation of array form.
		''' </summary>
		''' <param name="members">  Set members in array form.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>members</CODE> is null or
		'''     any element of <CODE>members</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if any element of
		'''   <CODE>members</CODE> is not a length-one or length-two array. Also
		'''    thrown if <CODE>members</CODE> is a zero-length array or if any
		'''    member of the set is less than 1. </exception>
		Public Sub New(ByVal members As Integer()())
			MyBase.New(members)
			If members Is Nothing Then Throw New NullPointerException("members is null")
			Dim myMembers As Integer()() = members
			Dim n As Integer = myMembers.Length
			If n = 0 Then Throw New System.ArgumentException("members is zero-length")
			Dim i As Integer
			For i = 0 To n - 1
				If myMembers(i)(0) < 1 Then Throw New System.ArgumentException("Number up value must be > 0")
			Next i
		End Sub

		''' <summary>
		''' Construct a new number up supported attribute containing a single
		''' integer. That is, only the one value of NumberUp is supported.
		''' </summary>
		''' <param name="member">  Set member.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>member</CODE> is less than
		'''     1. </exception>
		Public Sub New(ByVal member As Integer)
			MyBase.New(member)
			If member < 1 Then Throw New System.ArgumentException("Number up value must be > 0")
		End Sub

		''' <summary>
		''' Construct a new number up supported attribute containing a single range
		''' of integers. That is, only those values of NumberUp in the one range are
		''' supported.
		''' </summary>
		''' <param name="lowerBound">  Lower bound of the range. </param>
		''' <param name="upperBound">  Upper bound of the range.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if a null range is specified or if a
		'''     non-null range is specified with <CODE>lowerBound</CODE> less than
		'''     1. </exception>
		Public Sub New(ByVal lowerBound As Integer, ByVal upperBound As Integer)
			MyBase.New(lowerBound, upperBound)
			If lowerBound > upperBound Then
				Throw New System.ArgumentException("Null range specified")
			ElseIf lowerBound < 1 Then
				Throw New System.ArgumentException("Number up value must be > 0")
			End If
		End Sub

		''' <summary>
		''' Returns whether this number up supported attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class NumberUpSupported.
		''' <LI>
		''' This number up supported attribute's members and <CODE>object</CODE>'s
		''' members are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this number up
		'''          supported attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is NumberUpSupported)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class NumberUpSupported, the
		''' category is class NumberUpSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(NumberUpSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class NumberUpSupported, the
		''' category name is <CODE>"number-up-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "number-up-supported"
			End Get
		End Property

	End Class

End Namespace