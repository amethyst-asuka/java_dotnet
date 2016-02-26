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
	''' Class DateTimeSyntax is an abstract base class providing the common
	''' implementation of all attributes whose value is a date and time.
	''' <P>
	''' Under the hood, a date-time attribute is stored as a value of class <code>
	''' java.util.Date</code>. You can get a date-time attribute's Date value by
	''' calling <seealso cref="#getValue() getValue()"/>. A date-time attribute's
	''' Date value is established when it is constructed (see {@link
	''' #DateTimeSyntax(Date) DateTimeSyntax(Date)}). Once
	''' constructed, a date-time attribute's value is immutable.
	''' <P>
	''' To construct a date-time attribute from separate values of the year, month,
	''' day, hour, minute, and so on, use a <code>java.util.Calendar</code>
	''' object to construct a <code>java.util.Date</code> object, then use the
	''' <code>java.util.Date</code> object to construct the date-time attribute.
	''' To convert
	''' a date-time attribute to separate values of the year, month, day, hour,
	''' minute, and so on, create a <code>java.util.Calendar</code> object and
	''' set it to the <code>java.util.Date</code> from the date-time attribute. Class
	''' DateTimeSyntax stores its value in the form of a <code>java.util.Date
	''' </code>
	''' rather than a <code>java.util.Calendar</code> because it typically takes
	''' less memory to store and less time to compare a <code>java.util.Date</code>
	''' than a <code>java.util.Calendar</code>.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class DateTimeSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = -1400819079791208582L

		' Hidden data members.

		''' <summary>
		''' This date-time attribute's<code>java.util.Date</code> value.
		''' @serial
		''' </summary>
		Private value As DateTime

		' Hidden constructors.

		''' <summary>
		''' Construct a new date-time attribute with the given
		''' <code>java.util.Date </code> value.
		''' </summary>
		''' <param name="value">   <code>java.util.Date</code> value.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>theValue</CODE> is null. </exception>
		Protected Friend Sub New(ByVal value As DateTime)
			If value Is Nothing Then Throw New NullPointerException("value is null")
			Me.value = value
		End Sub

		' Exported operations.

		''' <summary>
		''' Returns this date-time attribute's <code>java.util.Date</code>
		''' value. </summary>
		''' <returns> the Date. </returns>
		Public Overridable Property value As DateTime
			Get
				Return New DateTime(value)
			End Get
		End Property

		' Exported operations inherited and overridden from class Object.

		''' <summary>
		''' Returns whether this date-time attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class DateTimeSyntax.
		''' <LI>
		''' This date-time attribute's <code>java.util.Date</code> value and
		''' <CODE>object</CODE>'s <code>java.util.Date</code> value are
		''' equal. </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this date-time
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is DateTimeSyntax AndAlso value.Equals(CType([object], DateTimeSyntax).value))
		End Function

		''' <summary>
		''' Returns a hash code value for this date-time attribute. The hashcode is
		''' that of this attribute's <code>java.util.Date</code> value.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return value.GetHashCode()
		End Function

		''' <summary>
		''' Returns a string value corresponding to this date-time attribute.
		''' The string value is just this attribute's
		''' <code>java.util.Date</code>  value
		''' converted to a string.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "" & value
		End Function

	End Class

End Namespace