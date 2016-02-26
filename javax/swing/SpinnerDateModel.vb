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

Namespace javax.swing



	''' <summary>
	''' A <code>SpinnerModel</code> for sequences of <code>Date</code>s.
	''' The upper and lower bounds of the sequence are defined by properties called
	''' <code>start</code> and <code>end</code> and the size
	''' of the increase or decrease computed by the <code>nextValue</code>
	''' and <code>previousValue</code> methods is defined by a property
	''' called <code>calendarField</code>.  The <code>start</code>
	''' and <code>end</code> properties can be <code>null</code> to
	''' indicate that the sequence has no lower or upper limit.
	''' <p>
	''' The value of the <code>calendarField</code> property must be one of the
	''' <code>java.util.Calendar</code> constants that specify a field
	''' within a <code>Calendar</code>.  The <code>getNextValue</code>
	''' and <code>getPreviousValue</code>
	''' methods change the date forward or backwards by this amount.
	''' For example, if <code>calendarField</code> is <code>Calendar.DAY_OF_WEEK</code>,
	''' then <code>nextValue</code> produces a <code>Date</code> that's 24
	''' hours after the current <code>value</code>, and <code>previousValue</code>
	''' produces a <code>Date</code> that's 24 hours earlier.
	''' <p>
	''' The legal values for <code>calendarField</code> are:
	''' <ul>
	'''   <li><code>Calendar.ERA</code>
	'''   <li><code>Calendar.YEAR</code>
	'''   <li><code>Calendar.MONTH</code>
	'''   <li><code>Calendar.WEEK_OF_YEAR</code>
	'''   <li><code>Calendar.WEEK_OF_MONTH</code>
	'''   <li><code>Calendar.DAY_OF_MONTH</code>
	'''   <li><code>Calendar.DAY_OF_YEAR</code>
	'''   <li><code>Calendar.DAY_OF_WEEK</code>
	'''   <li><code>Calendar.DAY_OF_WEEK_IN_MONTH</code>
	'''   <li><code>Calendar.AM_PM</code>
	'''   <li><code>Calendar.HOUR</code>
	'''   <li><code>Calendar.HOUR_OF_DAY</code>
	'''   <li><code>Calendar.MINUTE</code>
	'''   <li><code>Calendar.SECOND</code>
	'''   <li><code>Calendar.MILLISECOND</code>
	''' </ul>
	''' However some UIs may set the calendarField before committing the edit
	''' to spin the field under the cursor. If you only want one field to
	''' spin you can subclass and ignore the setCalendarField calls.
	''' <p>
	''' This model inherits a <code>ChangeListener</code>.  The
	''' <code>ChangeListeners</code> are notified whenever the models
	''' <code>value</code>, <code>calendarField</code>,
	''' <code>start</code>, or <code>end</code> properties changes.
	''' </summary>
	''' <seealso cref= JSpinner </seealso>
	''' <seealso cref= SpinnerModel </seealso>
	''' <seealso cref= AbstractSpinnerModel </seealso>
	''' <seealso cref= SpinnerListModel </seealso>
	''' <seealso cref= SpinnerNumberModel </seealso>
	''' <seealso cref= Calendar#add
	''' 
	''' @author Hans Muller
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class SpinnerDateModel
		Inherits AbstractSpinnerModel

		Private start, [end] As IComparable
		Private value As DateTime
		Private calendarField As Integer


		Private Function calendarFieldOK(ByVal calendarField As Integer) As Boolean
			Select Case calendarField
			Case DateTime.ERA, DateTime.YEAR, DateTime.MONTH, DateTime.WEEK_OF_YEAR, DateTime.WEEK_OF_MONTH, DateTime.DAY_OF_MONTH, DateTime.DAY_OF_YEAR, DateTime.DAY_OF_WEEK, DateTime.DAY_OF_WEEK_IN_MONTH, DateTime.AM_PM, DateTime.HOUR, DateTime.HOUR_OF_DAY, DateTime.MINUTE, DateTime.SECOND, DateTime.MILLISECOND
				Return True
			Case Else
				Return False
			End Select
		End Function


		''' <summary>
		''' Creates a <code>SpinnerDateModel</code> that represents a sequence of dates
		''' between <code>start</code> and <code>end</code>.  The
		''' <code>nextValue</code> and <code>previousValue</code> methods
		''' compute elements of the sequence by advancing or reversing
		''' the current date <code>value</code> by the
		''' <code>calendarField</code> time unit.  For a precise description
		''' of what it means to increment or decrement a <code>Calendar</code>
		''' <code>field</code>, see the <code>add</code> method in
		''' <code>java.util.Calendar</code>.
		''' <p>
		''' The <code>start</code> and <code>end</code> parameters can be
		''' <code>null</code> to indicate that the range doesn't have an
		''' upper or lower bound.  If <code>value</code> or
		''' <code>calendarField</code> is <code>null</code>, or if both
		''' <code>start</code> and <code>end</code> are specified and
		''' <code>minimum &gt; maximum</code> then an
		''' <code>IllegalArgumentException</code> is thrown.
		''' Similarly if <code>(minimum &lt;= value &lt;= maximum)</code> is false,
		''' an IllegalArgumentException is thrown.
		''' </summary>
		''' <param name="value"> the current (non <code>null</code>) value of the model </param>
		''' <param name="start"> the first date in the sequence or <code>null</code> </param>
		''' <param name="end"> the last date in the sequence or <code>null</code> </param>
		''' <param name="calendarField"> one of
		'''   <ul>
		'''    <li><code>Calendar.ERA</code>
		'''    <li><code>Calendar.YEAR</code>
		'''    <li><code>Calendar.MONTH</code>
		'''    <li><code>Calendar.WEEK_OF_YEAR</code>
		'''    <li><code>Calendar.WEEK_OF_MONTH</code>
		'''    <li><code>Calendar.DAY_OF_MONTH</code>
		'''    <li><code>Calendar.DAY_OF_YEAR</code>
		'''    <li><code>Calendar.DAY_OF_WEEK</code>
		'''    <li><code>Calendar.DAY_OF_WEEK_IN_MONTH</code>
		'''    <li><code>Calendar.AM_PM</code>
		'''    <li><code>Calendar.HOUR</code>
		'''    <li><code>Calendar.HOUR_OF_DAY</code>
		'''    <li><code>Calendar.MINUTE</code>
		'''    <li><code>Calendar.SECOND</code>
		'''    <li><code>Calendar.MILLISECOND</code>
		'''   </ul>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>value</code> or
		'''    <code>calendarField</code> are <code>null</code>,
		'''    if <code>calendarField</code> isn't valid,
		'''    or if the following expression is
		'''    false: <code>(start &lt;= value &lt;= end)</code>.
		''' </exception>
		''' <seealso cref= Calendar#add </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setStart </seealso>
		''' <seealso cref= #setEnd </seealso>
		''' <seealso cref= #setCalendarField </seealso>
		Public Sub New(ByVal value As DateTime, ByVal start As IComparable, ByVal [end] As IComparable, ByVal calendarField As Integer)
			If value Is Nothing Then Throw New System.ArgumentException("value is null")
			If Not calendarFieldOK(calendarField) Then Throw New System.ArgumentException("invalid calendarField")
			If Not(((start Is Nothing) OrElse (start.CompareTo(value) <= 0)) AndAlso (([end] Is Nothing) OrElse ([end].CompareTo(value) >= 0))) Then Throw New System.ArgumentException("(start <= value <= end) is false")
			Me.value = New DateTime
			Me.start = start
			Me.end = [end]
			Me.calendarField = calendarField

			Me.value = value
		End Sub


		''' <summary>
		''' Constructs a <code>SpinnerDateModel</code> whose initial
		''' <code>value</code> is the current date, <code>calendarField</code>
		''' is equal to <code>Calendar.DAY_OF_MONTH</code>, and for which
		''' there are no <code>start</code>/<code>end</code> limits.
		''' </summary>
		Public Sub New()
			Me.New(DateTime.Now, Nothing, Nothing, DateTime.DAY_OF_MONTH)
		End Sub


		''' <summary>
		''' Changes the lower limit for Dates in this sequence.
		''' If <code>start</code> is <code>null</code>,
		''' then there is no lower limit.  No bounds checking is done here:
		''' the new start value may invalidate the
		''' <code>(start &lt;= value &lt;= end)</code>
		''' invariant enforced by the constructors.  This is to simplify updating
		''' the model.  Naturally one should ensure that the invariant is true
		''' before calling the <code>nextValue</code>, <code>previousValue</code>,
		''' or <code>setValue</code> methods.
		''' <p>
		''' Typically this property is a <code>Date</code> however it's possible to use
		''' a <code>Comparable</code> with a <code>compareTo</code> method for Dates.
		''' For example <code>start</code> might be an instance of a class like this:
		''' <pre>
		''' MyStartDate implements Comparable {
		'''     long t = 12345;
		'''     public int compareTo(Date d) {
		'''            return (t &lt; d.getTime() ? -1 : (t == d.getTime() ? 0 : 1));
		'''     }
		'''     public int compareTo(Object o) {
		'''            return compareTo((Date)o);
		'''     }
		''' }
		''' </pre>
		''' Note that the above example will throw a <code>ClassCastException</code>
		''' if the <code>Object</code> passed to <code>compareTo(Object)</code>
		''' is not a <code>Date</code>.
		''' <p>
		''' This method fires a <code>ChangeEvent</code> if the
		''' <code>start</code> has changed.
		''' </summary>
		''' <param name="start"> defines the first date in the sequence </param>
		''' <seealso cref= #getStart </seealso>
		''' <seealso cref= #setEnd </seealso>
		''' <seealso cref= #addChangeListener </seealso>
		Public Overridable Property start As IComparable
			Set(ByVal start As IComparable)
				If If(start Is Nothing, (Me.start IsNot Nothing), (Not start.Equals(Me.start))) Then
					Me.start = start
					fireStateChanged()
				End If
			End Set
			Get
				Return start
			End Get
		End Property




		''' <summary>
		''' Changes the upper limit for <code>Date</code>s in this sequence.
		''' If <code>start</code> is <code>null</code>, then there is no upper
		''' limit.  No bounds checking is done here: the new
		''' start value may invalidate the <code>(start &lt;= value &lt;= end)</code>
		''' invariant enforced by the constructors.  This is to simplify updating
		''' the model.  Naturally, one should ensure that the invariant is true
		''' before calling the <code>nextValue</code>, <code>previousValue</code>,
		''' or <code>setValue</code> methods.
		''' <p>
		''' Typically this property is a <code>Date</code> however it's possible to use
		''' <code>Comparable</code> with a <code>compareTo</code> method for
		''' <code>Date</code>s.  See <code>setStart</code> for an example.
		''' <p>
		''' This method fires a <code>ChangeEvent</code> if the <code>end</code>
		''' has changed.
		''' </summary>
		''' <param name="end"> defines the last date in the sequence </param>
		''' <seealso cref= #getEnd </seealso>
		''' <seealso cref= #setStart </seealso>
		''' <seealso cref= #addChangeListener </seealso>
		Public Overridable Property [end] As IComparable
			Set(ByVal [end] As IComparable)
				If If([end] Is Nothing, (Me.end IsNot Nothing), (Not [end].Equals(Me.end))) Then
					Me.end = [end]
					fireStateChanged()
				End If
			End Set
			Get
				Return [end]
			End Get
		End Property




		''' <summary>
		''' Changes the size of the date value change computed
		''' by the <code>nextValue</code> and <code>previousValue</code> methods.
		''' The <code>calendarField</code> parameter must be one of the
		''' <code>Calendar</code> field constants like <code>Calendar.MONTH</code>
		''' or <code>Calendar.MINUTE</code>.
		''' The <code>nextValue</code> and <code>previousValue</code> methods
		''' simply move the specified <code>Calendar</code> field forward or backward
		''' by one unit with the <code>Calendar.add</code> method.
		''' You should use this method with care as some UIs may set the
		''' calendarField before committing the edit to spin the field under
		''' the cursor. If you only want one field to spin you can subclass
		''' and ignore the setCalendarField calls.
		''' </summary>
		''' <param name="calendarField"> one of
		'''  <ul>
		'''    <li><code>Calendar.ERA</code>
		'''    <li><code>Calendar.YEAR</code>
		'''    <li><code>Calendar.MONTH</code>
		'''    <li><code>Calendar.WEEK_OF_YEAR</code>
		'''    <li><code>Calendar.WEEK_OF_MONTH</code>
		'''    <li><code>Calendar.DAY_OF_MONTH</code>
		'''    <li><code>Calendar.DAY_OF_YEAR</code>
		'''    <li><code>Calendar.DAY_OF_WEEK</code>
		'''    <li><code>Calendar.DAY_OF_WEEK_IN_MONTH</code>
		'''    <li><code>Calendar.AM_PM</code>
		'''    <li><code>Calendar.HOUR</code>
		'''    <li><code>Calendar.HOUR_OF_DAY</code>
		'''    <li><code>Calendar.MINUTE</code>
		'''    <li><code>Calendar.SECOND</code>
		'''    <li><code>Calendar.MILLISECOND</code>
		'''  </ul>
		''' <p>
		''' This method fires a <code>ChangeEvent</code> if the
		''' <code>calendarField</code> has changed.
		''' </param>
		''' <seealso cref= #getCalendarField </seealso>
		''' <seealso cref= #getNextValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		''' <seealso cref= Calendar#add </seealso>
		''' <seealso cref= #addChangeListener </seealso>
		Public Overridable Property calendarField As Integer
			Set(ByVal calendarField As Integer)
				If Not calendarFieldOK(calendarField) Then Throw New System.ArgumentException("invalid calendarField")
				If calendarField <> Me.calendarField Then
					Me.calendarField = calendarField
					fireStateChanged()
				End If
			End Set
			Get
				Return calendarField
			End Get
		End Property




		''' <summary>
		''' Returns the next <code>Date</code> in the sequence, or <code>null</code> if
		''' the next date is after <code>end</code>.
		''' </summary>
		''' <returns> the next <code>Date</code> in the sequence, or <code>null</code> if
		'''     the next date is after <code>end</code>.
		''' </returns>
		''' <seealso cref= SpinnerModel#getNextValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		''' <seealso cref= #setCalendarField </seealso>
		Public Property Overrides nextValue As Object
			Get
				Dim cal As DateTime = New DateTime
				cal = value
				cal.add(calendarField, 1)
				Dim [next] As DateTime = cal
				Return If(([end] Is Nothing) OrElse ([end].CompareTo([next]) >= 0), [next], Nothing)
			End Get
		End Property


		''' <summary>
		''' Returns the previous <code>Date</code> in the sequence, or <code>null</code>
		''' if the previous date is before <code>start</code>.
		''' </summary>
		''' <returns> the previous <code>Date</code> in the sequence, or
		'''     <code>null</code> if the previous date
		'''     is before <code>start</code>
		''' </returns>
		''' <seealso cref= SpinnerModel#getPreviousValue </seealso>
		''' <seealso cref= #getNextValue </seealso>
		''' <seealso cref= #setCalendarField </seealso>
		Public Property Overrides previousValue As Object
			Get
				Dim cal As DateTime = New DateTime
				cal = value
				cal.add(calendarField, -1)
				Dim prev As DateTime = cal
				Return If((start Is Nothing) OrElse (start.CompareTo(prev) <= 0), prev, Nothing)
			End Get
		End Property


		''' <summary>
		''' Returns the current element in this sequence of <code>Date</code>s.
		''' This method is equivalent to <code>(Date)getValue</code>.
		''' </summary>
		''' <returns> the <code>value</code> property </returns>
		''' <seealso cref= #setValue </seealso>
		Public Overridable Property [date] As DateTime
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Returns the current element in this sequence of <code>Date</code>s.
		''' </summary>
		''' <returns> the <code>value</code> property </returns>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #getDate </seealso>
		Public Property Overrides value As Object
			Get
				Return value
			End Get
			Set(ByVal value As Object)
				If (value Is Nothing) OrElse Not(TypeOf value Is DateTime) Then Throw New System.ArgumentException("illegal value")
				If Not value.Equals(Me.value) Then
					Me.value = CDate(value)
					fireStateChanged()
				End If
			End Set
		End Property


	End Class

End Namespace