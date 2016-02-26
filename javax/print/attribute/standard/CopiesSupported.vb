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
	''' Class CopiesSupported is a printing attribute class, a set of integers, that
	''' gives the supported values for a <seealso cref="Copies Copies"/> attribute. It is
	''' restricted to a single contiguous range of integers; multiple non-overlapping
	''' ranges are not allowed.
	''' <P>
	''' <B>IPP Compatibility:</B> The CopiesSupported attribute's canonical array
	''' form gives the lower and upper bound for the range of copies to be included
	''' in an IPP "copies-supported" attribute. See class {@link
	''' javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax} for an
	''' explanation of canonical array form. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class CopiesSupported
		Inherits javax.print.attribute.SetOfIntegerSyntax
		Implements javax.print.attribute.SupportedValuesAttribute

		Private Const serialVersionUID As Long = 6927711687034846001L

		''' <summary>
		''' Construct a new copies supported attribute containing a single integer.
		''' That is, only the one value of Copies is supported.
		''' </summary>
		''' <param name="member">  Set member.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''  (Unchecked exception) Thrown if <CODE>member</CODE> is less than 1. </exception>
		Public Sub New(ByVal member As Integer)
			MyBase.New(member)
			If member < 1 Then Throw New System.ArgumentException("Copies value < 1 specified")
		End Sub

		''' <summary>
		''' Construct a new copies supported attribute containing a single range of
		''' integers. That is, only those values of Copies in the one range are
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
				Throw New System.ArgumentException("Copies value < 1 specified")
			End If
		End Sub

		''' <summary>
		''' Returns whether this copies supported attribute is equivalent to the
		''' passed in object. To be equivalent, all of the following conditions must
		''' be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class CopiesSupported.
		''' <LI>
		''' This copies supported attribute's members and <CODE>object</CODE>'s
		''' members are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this copies
		'''          supported attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return MyBase.Equals([object]) AndAlso TypeOf [object] Is CopiesSupported
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class CopiesSupported, the category
		''' is class CopiesSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(CopiesSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class CopiesSupported, the category
		''' name is <CODE>"copies-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "copies-supported"
			End Get
		End Property

	End Class

End Namespace