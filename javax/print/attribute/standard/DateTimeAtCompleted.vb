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
	''' Class DateTimeAtCompleted is a printing attribute class, a date-time
	''' attribute, that indicates the date and time at which the Print Job completed
	''' (or was canceled or aborted).
	''' <P>
	''' To construct a DateTimeAtCompleted attribute from separate values of the
	''' year, month, day, hour, minute, and so on, use a {@link java.util.Calendar
	''' Calendar} object to construct a <seealso cref="java.util.Date Date"/> object, then use
	''' the <seealso cref="java.util.Date Date"/> object to construct the DateTimeAtCompleted
	''' attribute. To convert a DateTimeAtCompleted attribute to separate values of
	''' the year, month, day, hour, minute, and so on, create a {@link
	''' java.util.Calendar Calendar} object and set it to the {@link java.util.Date
	''' Date} from the DateTimeAtCompleted attribute.
	''' <P>
	''' <B>IPP Compatibility:</B> The information needed to construct an IPP
	''' "date-time-at-completed" attribute can be obtained as described above. The
	''' category name returned by <CODE>getName()</CODE> gives the IPP attribute
	''' name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public NotInheritable Class DateTimeAtCompleted
		Inherits javax.print.attribute.DateTimeSyntax
		Implements javax.print.attribute.PrintJobAttribute

		Private Const serialVersionUID As Long = 6497399708058490000L

		''' <summary>
		''' Construct a new date-time at completed attribute with the given {@link
		''' java.util.Date Date} value.
		''' </summary>
		''' <param name="dateTime">  <seealso cref="java.util.Date Date"/> value.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>dateTime</CODE> is null. </exception>
		Public Sub New(ByVal dateTime As DateTime)
			MyBase.New(dateTime)
		End Sub

		''' <summary>
		''' Returns whether this date-time at completed attribute is equivalent to
		''' the passed in object. To be equivalent, all of the following conditions
		''' must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class DateTimeAtCompleted.
		''' <LI>
		''' This date-time at completed attribute's <seealso cref="java.util.Date Date"/> value
		''' and <CODE>object</CODE>'s <seealso cref="java.util.Date Date"/> value are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this date-time
		'''          at completed attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return (MyBase.Equals([object]) AndAlso TypeOf [object] Is DateTimeAtCompleted)
		End Function

	' Exported operations inherited and implemented from interface Attribute.

		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class DateTimeAtCompleted, the category is class
		''' DateTimeAtCompleted itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(DateTimeAtCompleted)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class DateTimeAtCompleted, the category name is
		''' <CODE>"date-time-at-completed"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "date-time-at-completed"
			End Get
		End Property

	End Class

End Namespace