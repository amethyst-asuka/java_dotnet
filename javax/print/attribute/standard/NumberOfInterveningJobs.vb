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
	''' Class NumberOfInterveningJobs is an integer valued printing attribute that
	''' indicates the number of jobs that are ahead of this job in the relative
	''' chronological order of expected time to complete (i.e., the current
	''' scheduled order).
	''' <P>
	''' <B>IPP Compatibility:</B> The integer value gives the IPP integer value.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class NumberOfInterveningJobs
		Inherits javax.print.attribute.IntegerSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 2568141124844982746L

		''' <summary>
		''' Construct a new number of intervening jobs attribute with the given
		''' integer value.
		''' </summary>
		''' <param name="value">  Integer value.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''   (Unchecked exception) Thrown if <CODE>value</CODE> is less than 0. </exception>
		Public Sub New(ByVal value As Integer)
			MyBase.New(value, 0, Integer.MaxValue)
		End Sub

		''' <summary>
		''' Returns whether this number of intervening jobs attribute is equivalent
		''' to the passed in object. To be equivalent, all of the following
		''' conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class NumberOfInterveningJobs.
		''' <LI>
		''' This number of intervening jobs attribute's value and
		''' <CODE>object</CODE>'s value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this number of
		'''          intervening jobs attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is NumberOfInterveningJobs)
		End Function

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class NumberOfInterveningJobs, the
		''' category is class NumberOfInterveningJobs itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(NumberOfInterveningJobs)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class NumberOfInterveningJobs, the
		''' category name is <CODE>"number-of-intervening-jobs"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "number-of-intervening-jobs"
			End Get
		End Property

	End Class

End Namespace