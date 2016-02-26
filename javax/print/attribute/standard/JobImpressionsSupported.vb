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
	''' Class JobImpressionsSupported is a printing attribute class, a set of
	''' integers, that gives the supported values for a {@link JobImpressions
	''' JobImpressions} attribute. It is restricted to a single contiguous range of
	''' integers; multiple non-overlapping ranges are not allowed. This gives the
	''' lower and upper bounds of the total sizes of print jobs in number of
	''' impressions that the printer will accept.
	''' <P>
	''' <B>IPP Compatibility:</B> The JobImpressionsSupported attribute's canonical
	''' array form gives the lower and upper bound for the range of values to be
	''' included in an IPP "job-impressions-supported" attribute. See class {@link
	''' javax.print.attribute.SetOfIntegerSyntax SetOfIntegerSyntax} for an
	''' explanation of canonical array form. The category name returned by
	''' <CODE>getName()</CODE> gives the IPP attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class JobImpressionsSupported
		Inherits javax.print.attribute.SetOfIntegerSyntax
		Implements javax.print.attribute.SupportedValuesAttribute

		Private Const serialVersionUID As Long = -4887354803843173692L


		''' <summary>
		''' Construct a new job impressions supported attribute containing a single
		''' range of integers. That is, only those values of JobImpressions in the
		''' one range are supported.
		''' </summary>
		''' <param name="lowerBound">  Lower bound of the range. </param>
		''' <param name="upperBound">  Upper bound of the range.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if a null range is specified or if a
		'''     non-null range is specified with <CODE>lowerBound</CODE> less than
		'''     0. </exception>
		Public Sub New(ByVal lowerBound As Integer, ByVal upperBound As Integer)
			MyBase.New(lowerBound, upperBound)
			If lowerBound > upperBound Then
				Throw New System.ArgumentException("Null range specified")
			ElseIf lowerBound < 0 Then
				Throw New System.ArgumentException("Job K octets value < 0 specified")
			End If
		End Sub


		''' <summary>
		''' Returns whether this job impressions supported attribute is equivalent
		''' to the passed in object. To be equivalent, all of the following
		''' conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class JobImpressionsSupported.
		''' <LI>
		''' This job impressions supported attribute's members and
		''' <CODE>object</CODE>'s members are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this job
		'''          impressions supported attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is JobImpressionsSupported)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class JobImpressionsSupported, the category is class
		''' JobImpressionsSupported itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(JobImpressionsSupported)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class JobImpressionsSupported, the category name is
		''' <CODE>"job-impressions-supported"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "job-impressions-supported"
			End Get
		End Property

	End Class

End Namespace