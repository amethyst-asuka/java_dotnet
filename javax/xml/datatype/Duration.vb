Imports System
Imports System.Text

'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.datatype



	''' <summary>
	''' <p>Immutable representation of a time span as defined in
	''' the W3C XML Schema 1.0 specification.</p>
	''' 
	''' <p>A Duration object represents a period of Gregorian time,
	''' which consists of six fields (years, months, days, hours,
	''' minutes, and seconds) plus a sign (+/-) field.</p>
	''' 
	''' <p>The first five fields have non-negative (>=0) integers or null
	''' (which represents that the field is not set),
	''' and the seconds field has a non-negative decimal or null.
	''' A negative sign indicates a negative duration.</p>
	''' 
	''' <p>This class provides a number of methods that make it easy
	''' to use for the duration datatype of XML Schema 1.0 with
	''' the errata.</p>
	''' 
	''' <h2>Order relationship</h2>
	''' <p>Duration objects only have partial order, where two values A and B
	''' maybe either:</p>
	''' <ol>
	'''  <li>A&lt;B (A is shorter than B)
	'''  <li>A&gt;B (A is longer than B)
	'''  <li>A==B   (A and B are of the same duration)
	'''  <li>A&lt;>B (Comparison between A and B is indeterminate)
	''' </ol>
	''' 
	''' <p>For example, 30 days cannot be meaningfully compared to one month.
	''' The <seealso cref="#compare(Duration duration)"/> method implements this
	''' relationship.</p>
	''' 
	''' <p>See the <seealso cref="#isLongerThan(Duration)"/> method for details about
	''' the order relationship among <code>Duration</code> objects.</p>
	''' 
	''' <h2>Operations over Duration</h2>
	''' <p>This class provides a set of basic arithmetic operations, such
	''' as addition, subtraction and multiplication.
	''' Because durations don't have total order, an operation could
	''' fail for some combinations of operations. For example, you cannot
	''' subtract 15 days from 1 month. See the javadoc of those methods
	''' for detailed conditions where this could happen.</p>
	''' 
	''' <p>Also, division of a duration by a number is not provided because
	''' the <code>Duration</code> class can only deal with finite precision
	''' decimal numbers. For example, one cannot represent 1 sec divided by 3.</p>
	''' 
	''' <p>However, you could substitute a division by 3 with multiplying
	''' by numbers such as 0.3 or 0.333.</p>
	''' 
	''' <h2>Range of allowed values</h2>
	''' <p>
	''' Because some operations of <code>Duration</code> rely on <seealso cref="Calendar"/>
	''' even though <seealso cref="Duration"/> can hold very large or very small values,
	''' some of the methods may not work correctly on such <code>Duration</code>s.
	''' The impacted methods document their dependency on <seealso cref="Calendar"/>.
	''' 
	''' @author <a href="mailto:Joseph.Fialli@Sun.COM">Joseph Fialli</a>
	''' @author <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @author <a href="mailto:Sunitha.Reddy@Sun.com">Sunitha Reddy</a> </summary>
	''' <seealso cref= XMLGregorianCalendar#add(Duration)
	''' @since 1.5 </seealso>
	Public MustInherit Class Duration

		''' <summary>
		''' <p>Debugging <code>true</code> or <code>false</code>.</p>
		''' </summary>
		Private Const DEBUG As Boolean = True

		''' <summary>
		''' Default no-arg constructor.
		''' 
		''' <p>Note: Always use the <seealso cref="DatatypeFactory"/> to
		''' construct an instance of <code>Duration</code>.
		''' The constructor on this class cannot be guaranteed to
		''' produce an object with a consistent state and may be
		''' removed in the future.</p>
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' <p>Return the name of the XML Schema date/time type that this instance
		''' maps to. Type is computed based on fields that are set,
		''' i.e. <seealso cref="#isSet(DatatypeConstants.Field field)"/> == <code>true</code>.</p>
		''' 
		''' <table border="2" rules="all" cellpadding="2">
		'''   <thead>
		'''     <tr>
		'''       <th align="center" colspan="7">
		'''         Required fields for XML Schema 1.0 Date/Time Datatypes.<br/>
		'''         <i>(timezone is optional for all date/time datatypes)</i>
		'''       </th>
		'''     </tr>
		'''   </thead>
		'''   <tbody>
		'''     <tr>
		'''       <td>Datatype</td>
		'''       <td>year</td>
		'''       <td>month</td>
		'''       <td>day</td>
		'''       <td>hour</td>
		'''       <td>minute</td>
		'''       <td>second</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#DURATION"/></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#DURATION_DAYTIME"/></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''     </tr>
		'''     <tr>
		'''       <td><seealso cref="DatatypeConstants#DURATION_YEARMONTH"/></td>
		'''       <td>X</td>
		'''       <td>X</td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''       <td></td>
		'''     </tr>
		'''   </tbody>
		''' </table>
		''' </summary>
		''' <returns> one of the following constants:
		'''   <seealso cref="DatatypeConstants#DURATION"/>,
		'''   <seealso cref="DatatypeConstants#DURATION_DAYTIME"/> or
		'''   <seealso cref="DatatypeConstants#DURATION_YEARMONTH"/>.
		''' </returns>
		''' <exception cref="IllegalStateException"> If the combination of set fields does not match one of the XML Schema date/time datatypes. </exception>
		Public Overridable Property xMLSchemaType As javax.xml.namespace.QName
			Get
    
				Dim yearSet As Boolean = isSet(DatatypeConstants.YEARS)
				Dim monthSet As Boolean = isSet(DatatypeConstants.MONTHS)
				Dim daySet As Boolean = isSet(DatatypeConstants.DAYS)
				Dim hourSet As Boolean = isSet(DatatypeConstants.HOURS)
				Dim minuteSet As Boolean = isSet(DatatypeConstants.MINUTES)
				Dim secondSet As Boolean = isSet(DatatypeConstants.SECONDS)
    
				' DURATION
				If yearSet AndAlso monthSet AndAlso daySet AndAlso hourSet AndAlso minuteSet AndAlso secondSet Then Return DatatypeConstants.DURATION
    
				' DURATION_DAYTIME
				If (Not yearSet) AndAlso (Not monthSet) AndAlso daySet AndAlso hourSet AndAlso minuteSet AndAlso secondSet Then Return DatatypeConstants.DURATION_DAYTIME
    
				' DURATION_YEARMONTH
				If yearSet AndAlso monthSet AndAlso (Not daySet) AndAlso (Not hourSet) AndAlso (Not minuteSet) AndAlso (Not secondSet) Then Return DatatypeConstants.DURATION_YEARMONTH
    
				' nothing matches
				Throw New IllegalStateException("javax.xml.datatype.Duration#getXMLSchemaType():" & " this Duration does not match one of the XML Schema date/time datatypes:" & " year set = " & yearSet & " month set = " & monthSet & " day set = " & daySet & " hour set = " & hourSet & " minute set = " & minuteSet & " second set = " & secondSet)
			End Get
		End Property

		''' <summary>
		''' Returns the sign of this duration in -1,0, or 1.
		''' 
		''' @return
		'''      -1 if this duration is negative, 0 if the duration is zero,
		'''      and 1 if the duration is positive.
		''' </summary>
		Public MustOverride ReadOnly Property sign As Integer

		''' <summary>
		''' <p>Get the years value of this <code>Duration</code> as an <code>int</code> or <code>0</code> if not present.</p>
		''' 
		''' <p><code>getYears()</code> is a convenience method for
		''' <seealso cref="#getField(DatatypeConstants.Field field) getField(DatatypeConstants.YEARS)"/>.</p>
		''' 
		''' <p>As the return value is an <code>int</code>, an incorrect value will be returned for <code>Duration</code>s
		''' with years that go beyond the range of an <code>int</code>.
		''' Use <seealso cref="#getField(DatatypeConstants.Field field) getField(DatatypeConstants.YEARS)"/> to avoid possible loss of precision.</p>
		''' </summary>
		''' <returns> If the years field is present, return its value as an <code>int</code>, else return <code>0</code>. </returns>
		Public Overridable Property years As Integer
			Get
				Return getField(DatatypeConstants.YEARS)
			End Get
		End Property

		''' <summary>
		''' Obtains the value of the MONTHS field as an integer value,
		''' or 0 if not present.
		''' 
		''' This method works just like <seealso cref="#getYears()"/> except
		''' that this method works on the MONTHS field.
		''' </summary>
		''' <returns> Months of this <code>Duration</code>. </returns>
		Public Overridable Property months As Integer
			Get
				Return getField(DatatypeConstants.MONTHS)
			End Get
		End Property

		''' <summary>
		''' Obtains the value of the DAYS field as an integer value,
		''' or 0 if not present.
		''' 
		''' This method works just like <seealso cref="#getYears()"/> except
		''' that this method works on the DAYS field.
		''' </summary>
		''' <returns> Days of this <code>Duration</code>. </returns>
		Public Overridable Property days As Integer
			Get
				Return getField(DatatypeConstants.DAYS)
			End Get
		End Property

		''' <summary>
		''' Obtains the value of the HOURS field as an integer value,
		''' or 0 if not present.
		''' 
		''' This method works just like <seealso cref="#getYears()"/> except
		''' that this method works on the HOURS field.
		''' </summary>
		''' <returns> Hours of this <code>Duration</code>.
		'''  </returns>
		Public Overridable Property hours As Integer
			Get
				Return getField(DatatypeConstants.HOURS)
			End Get
		End Property

		''' <summary>
		''' Obtains the value of the MINUTES field as an integer value,
		''' or 0 if not present.
		''' 
		''' This method works just like <seealso cref="#getYears()"/> except
		''' that this method works on the MINUTES field.
		''' </summary>
		''' <returns> Minutes of this <code>Duration</code>.
		'''  </returns>
		Public Overridable Property minutes As Integer
			Get
				Return getField(DatatypeConstants.MINUTES)
			End Get
		End Property

		''' <summary>
		''' Obtains the value of the SECONDS field as an integer value,
		''' or 0 if not present.
		''' 
		''' This method works just like <seealso cref="#getYears()"/> except
		''' that this method works on the SECONDS field.
		''' </summary>
		''' <returns> seconds in the integer value. The fraction of seconds
		'''   will be discarded (for example, if the actual value is 2.5,
		'''   this method returns 2) </returns>
		Public Overridable Property seconds As Integer
			Get
				Return getField(DatatypeConstants.SECONDS)
			End Get
		End Property

		''' <summary>
		''' <p>Returns the length of the duration in milli-seconds.</p>
		''' 
		''' <p>If the seconds field carries more digits than milli-second order,
		''' those will be simply discarded (or in other words, rounded to zero.)
		''' For example, for any Calendar value <code>x</code>,</p>
		''' <pre>
		''' <code>new Duration("PT10.00099S").getTimeInMills(x) == 10000</code>.
		''' <code>new Duration("-PT10.00099S").getTimeInMills(x) == -10000</code>.
		''' </pre>
		''' 
		''' <p>
		''' Note that this method uses the <seealso cref="#addTo(Calendar)"/> method,
		''' which may work incorrectly with <code>Duration</code> objects with
		''' very large values in its fields. See the <seealso cref="#addTo(Calendar)"/>
		''' method for details.
		''' </summary>
		''' <param name="startInstant">
		'''      The length of a month/year varies. The <code>startInstant</code> is
		'''      used to disambiguate this variance. Specifically, this method
		'''      returns the difference between <code>startInstant</code> and
		'''      <code>startInstant+duration</code>
		''' </param>
		''' <returns> milliseconds between <code>startInstant</code> and
		'''   <code>startInstant</code> plus this <code>Duration</code>
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>startInstant</code> parameter
		''' is null.
		'''  </exception>
		Public Overridable Function getTimeInMillis(ByVal startInstant As DateTime) As Long
			Dim cal As DateTime = CType(startInstant.clone(), DateTime)
			addTo(cal)
			Return getCalendarTimeInMillis(cal) - getCalendarTimeInMillis(startInstant)
		End Function

		''' <summary>
		''' <p>Returns the length of the duration in milli-seconds.</p>
		''' 
		''' <p>If the seconds field carries more digits than milli-second order,
		''' those will be simply discarded (or in other words, rounded to zero.)
		''' For example, for any <code>Date</code> value <code>x</code>,</p>
		''' <pre>
		''' <code>new Duration("PT10.00099S").getTimeInMills(x) == 10000</code>.
		''' <code>new Duration("-PT10.00099S").getTimeInMills(x) == -10000</code>.
		''' </pre>
		''' 
		''' <p>
		''' Note that this method uses the <seealso cref="#addTo(Date)"/> method,
		''' which may work incorrectly with <code>Duration</code> objects with
		''' very large values in its fields. See the <seealso cref="#addTo(Date)"/>
		''' method for details.
		''' </summary>
		''' <param name="startInstant">
		'''      The length of a month/year varies. The <code>startInstant</code> is
		'''      used to disambiguate this variance. Specifically, this method
		'''      returns the difference between <code>startInstant</code> and
		'''      <code>startInstant+duration</code>.
		''' </param>
		''' <exception cref="NullPointerException">
		'''      If the startInstant parameter is null.
		''' </exception>
		''' <returns> milliseconds between <code>startInstant</code> and
		'''   <code>startInstant</code> plus this <code>Duration</code>
		''' </returns>
		''' <seealso cref= #getTimeInMillis(Calendar) </seealso>
		Public Overridable Function getTimeInMillis(ByVal startInstant As DateTime) As Long
			Dim cal As DateTime = New java.util.GregorianCalendar
			cal = startInstant
			Me.addTo(cal)
			Return getCalendarTimeInMillis(cal) - startInstant
		End Function

		''' <summary>
		''' Gets the value of a field.
		''' 
		''' Fields of a duration object may contain arbitrary large value.
		''' Therefore this method is designed to return a <seealso cref="Number"/> object.
		''' 
		''' In case of YEARS, MONTHS, DAYS, HOURS, and MINUTES, the returned
		''' number will be a non-negative integer. In case of seconds,
		''' the returned number may be a non-negative decimal value.
		''' </summary>
		''' <param name="field">
		'''      one of the six Field constants (YEARS,MONTHS,DAYS,HOURS,
		'''      MINUTES, or SECONDS.)
		''' @return
		'''      If the specified field is present, this method returns
		'''      a non-null non-negative <seealso cref="Number"/> object that
		'''      represents its value. If it is not present, return null.
		'''      For YEARS, MONTHS, DAYS, HOURS, and MINUTES, this method
		'''      returns a <seealso cref="java.math.BigInteger"/> object. For SECONDS, this
		'''      method returns a <seealso cref="java.math.BigDecimal"/>.
		''' </param>
		''' <exception cref="NullPointerException"> If the <code>field</code> is <code>null</code>. </exception>
		Public MustOverride Function getField(ByVal field As DatatypeConstants.Field) As Number

		''' <summary>
		''' Checks if a field is set.
		''' 
		''' A field of a duration object may or may not be present.
		''' This method can be used to test if a field is present.
		''' </summary>
		''' <param name="field">
		'''      one of the six Field constants (YEARS,MONTHS,DAYS,HOURS,
		'''      MINUTES, or SECONDS.)
		''' @return
		'''      true if the field is present. false if not.
		''' </param>
		''' <exception cref="NullPointerException">
		'''      If the field parameter is null. </exception>
		Public MustOverride Function isSet(ByVal field As DatatypeConstants.Field) As Boolean

		''' <summary>
		''' <p>Computes a new duration whose value is <code>this+rhs</code>.</p>
		''' 
		''' <p>For example,</p>
		''' <pre>
		''' "1 day" + "-3 days" = "-2 days"
		''' "1 year" + "1 day" = "1 year and 1 day"
		''' "-(1 hour,50 minutes)" + "-20 minutes" = "-(1 hours,70 minutes)"
		''' "15 hours" + "-3 days" = "-(2 days,9 hours)"
		''' "1 year" + "-1 day" = IllegalStateException
		''' </pre>
		''' 
		''' <p>Since there's no way to meaningfully subtract 1 day from 1 month,
		''' there are cases where the operation fails in
		''' <seealso cref="IllegalStateException"/>.</p>
		''' 
		''' <p>
		''' Formally, the computation is defined as follows.</p>
		''' <p>
		''' Firstly, we can assume that two <code>Duration</code>s to be added
		''' are both positive without losing generality (i.e.,
		''' <code>(-X)+Y=Y-X</code>, <code>X+(-Y)=X-Y</code>,
		''' <code>(-X)+(-Y)=-(X+Y)</code>)
		''' 
		''' <p>
		''' Addition of two positive <code>Duration</code>s are simply defined as
		''' field by field addition where missing fields are treated as 0.
		''' <p>
		''' A field of the resulting <code>Duration</code> will be unset if and
		''' only if respective fields of two input <code>Duration</code>s are unset.
		''' <p>
		''' Note that <code>lhs.add(rhs)</code> will be always successful if
		''' <code>lhs.signum()*rhs.signum()!=-1</code> or both of them are
		''' normalized.</p>
		''' </summary>
		''' <param name="rhs"> <code>Duration</code> to add to this <code>Duration</code>
		''' 
		''' @return
		'''      non-null valid Duration object.
		''' </param>
		''' <exception cref="NullPointerException">
		'''      If the rhs parameter is null. </exception>
		''' <exception cref="IllegalStateException">
		'''      If two durations cannot be meaningfully added. For
		'''      example, adding negative one day to one month causes
		'''      this exception.
		''' 
		''' </exception>
		''' <seealso cref= #subtract(Duration) </seealso>
		Public MustOverride Function add(ByVal rhs As Duration) As Duration

		''' <summary>
		''' Adds this duration to a <seealso cref="Calendar"/> object.
		''' 
		''' <p>
		''' Calls <seealso cref="java.util.Calendar#add(int,int)"/> in the
		''' order of YEARS, MONTHS, DAYS, HOURS, MINUTES, SECONDS, and MILLISECONDS
		''' if those fields are present. Because the <seealso cref="Calendar"/> class
		''' uses int to hold values, there are cases where this method
		''' won't work correctly (for example if values of fields
		''' exceed the range of int.)
		''' </p>
		''' 
		''' <p>
		''' Also, since this duration class is a Gregorian duration, this
		''' method will not work correctly if the given <seealso cref="Calendar"/>
		''' object is based on some other calendar systems.
		''' </p>
		''' 
		''' <p>
		''' Any fractional parts of this <code>Duration</code> object
		''' beyond milliseconds will be simply ignored. For example, if
		''' this duration is "P1.23456S", then 1 is added to SECONDS,
		''' 234 is added to MILLISECONDS, and the rest will be unused.
		''' </p>
		''' 
		''' <p>
		''' Note that because <seealso cref="Calendar#add(int, int)"/> is using
		''' <code>int</code>, <code>Duration</code> with values beyond the
		''' range of <code>int</code> in its fields
		''' will cause overflow/underflow to the given <seealso cref="Calendar"/>.
		''' <seealso cref="XMLGregorianCalendar#add(Duration)"/> provides the same
		''' basic operation as this method while avoiding
		''' the overflow/underflow issues.
		''' </summary>
		''' <param name="calendar">
		'''      A calendar object whose value will be modified. </param>
		''' <exception cref="NullPointerException">
		'''      if the calendar parameter is null. </exception>
		Public MustOverride Sub addTo(ByVal calendar As DateTime)

		''' <summary>
		''' Adds this duration to a <seealso cref="Date"/> object.
		''' 
		''' <p>
		''' The given date is first converted into
		''' a <seealso cref="java.util.GregorianCalendar"/>, then the duration
		''' is added exactly like the <seealso cref="#addTo(Calendar)"/> method.
		''' 
		''' <p>
		''' The updated time instant is then converted back into a
		''' <seealso cref="Date"/> object and used to update the given <seealso cref="Date"/> object.
		''' 
		''' <p>
		''' This somewhat redundant computation is necessary to unambiguously
		''' determine the duration of months and years.
		''' </summary>
		''' <param name="date">
		'''      A date object whose value will be modified. </param>
		''' <exception cref="NullPointerException">
		'''      if the date parameter is null. </exception>
		Public Overridable Sub addTo(ByVal [date] As DateTime)

			' check data parameter
			If [date] Is Nothing Then Throw New NullPointerException("Cannot call " & Me.GetType().name & "#addTo(Date date) with date == null.")

			Dim cal As DateTime = New java.util.GregorianCalendar
			cal = [date]
			Me.addTo(cal)
			[date] = getCalendarTimeInMillis(cal)
		End Sub

		''' <summary>
		''' <p>Computes a new duration whose value is <code>this-rhs</code>.</p>
		''' 
		''' <p>For example:</p>
		''' <pre>
		''' "1 day" - "-3 days" = "4 days"
		''' "1 year" - "1 day" = IllegalStateException
		''' "-(1 hour,50 minutes)" - "-20 minutes" = "-(1hours,30 minutes)"
		''' "15 hours" - "-3 days" = "3 days and 15 hours"
		''' "1 year" - "-1 day" = "1 year and 1 day"
		''' </pre>
		''' 
		''' <p>Since there's no way to meaningfully subtract 1 day from 1 month,
		''' there are cases where the operation fails in <seealso cref="IllegalStateException"/>.</p>
		''' 
		''' <p>Formally the computation is defined as follows.
		''' First, we can assume that two <code>Duration</code>s are both positive
		''' without losing generality.  (i.e.,
		''' <code>(-X)-Y=-(X+Y)</code>, <code>X-(-Y)=X+Y</code>,
		''' <code>(-X)-(-Y)=-(X-Y)</code>)</p>
		''' 
		''' <p>Then two durations are subtracted field by field.
		''' If the sign of any non-zero field <code>F</code> is different from
		''' the sign of the most significant field,
		''' 1 (if <code>F</code> is negative) or -1 (otherwise)
		''' will be borrowed from the next bigger unit of <code>F</code>.</p>
		''' 
		''' <p>This process is repeated until all the non-zero fields have
		''' the same sign.</p>
		''' 
		''' <p>If a borrow occurs in the days field (in other words, if
		''' the computation needs to borrow 1 or -1 month to compensate
		''' days), then the computation fails by throwing an
		''' <seealso cref="IllegalStateException"/>.</p>
		''' </summary>
		''' <param name="rhs"> <code>Duration</code> to subtract from this <code>Duration</code>.
		''' </param>
		''' <returns> New <code>Duration</code> created from subtracting <code>rhs</code> from this <code>Duration</code>.
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''      If two durations cannot be meaningfully subtracted. For
		'''      example, subtracting one day from one month causes
		'''      this exception.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''      If the rhs parameter is null.
		''' </exception>
		''' <seealso cref= #add(Duration) </seealso>
		Public Overridable Function subtract(ByVal rhs As Duration) As Duration
			Return add(rhs.negate())
		End Function

		''' <summary>
		''' <p>Computes a new duration whose value is <code>factor</code> times
		''' longer than the value of this duration.</p>
		''' 
		''' <p>This method is provided for the convenience.
		''' It is functionally equivalent to the following code:</p>
		''' <pre>
		''' multiply(new BigDecimal(String.valueOf(factor)))
		''' </pre>
		''' </summary>
		''' <param name="factor"> Factor times longer of new <code>Duration</code> to create.
		''' </param>
		''' <returns> New <code>Duration</code> that is <code>factor</code>times longer than this <code>Duration</code>.
		''' </returns>
		''' <seealso cref= #multiply(BigDecimal) </seealso>
		Public Overridable Function multiply(ByVal factor As Integer) As Duration
			Return multiply(New Decimal(Convert.ToString(factor)))
		End Function

		''' <summary>
		''' Computes a new duration whose value is <code>factor</code> times
		''' longer than the value of this duration.
		''' 
		''' <p>
		''' For example,
		''' <pre>
		''' "P1M" (1 month) * "12" = "P12M" (12 months)
		''' "PT1M" (1 min) * "0.3" = "PT18S" (18 seconds)
		''' "P1M" (1 month) * "1.5" = IllegalStateException
		''' </pre>
		''' 
		''' <p>
		''' Since the <code>Duration</code> class is immutable, this method
		''' doesn't change the value of this object. It simply computes
		''' a new Duration object and returns it.
		''' 
		''' <p>
		''' The operation will be performed field by field with the precision
		''' of <seealso cref="BigDecimal"/>. Since all the fields except seconds are
		''' restricted to hold integers,
		''' any fraction produced by the computation will be
		''' carried down toward the next lower unit. For example,
		''' if you multiply "P1D" (1 day) with "0.5", then it will be 0.5 day,
		''' which will be carried down to "PT12H" (12 hours).
		''' When fractions of month cannot be meaningfully carried down
		''' to days, or year to months, this will cause an
		''' <seealso cref="IllegalStateException"/> to be thrown.
		''' For example if you multiple one month by 0.5.</p>
		''' 
		''' <p>
		''' To avoid <seealso cref="IllegalStateException"/>, use
		''' the <seealso cref="#normalizeWith(Calendar)"/> method to remove the years
		''' and months fields.
		''' </summary>
		''' <param name="factor"> to multiply by
		''' 
		''' @return
		'''      returns a non-null valid <code>Duration</code> object
		''' </param>
		''' <exception cref="IllegalStateException"> if operation produces fraction in
		''' the months field.
		''' </exception>
		''' <exception cref="NullPointerException"> if the <code>factor</code> parameter is
		''' <code>null</code>.
		'''  </exception>
		Public MustOverride Function multiply(ByVal factor As Decimal) As Duration

		''' <summary>
		''' Returns a new <code>Duration</code> object whose
		''' value is <code>-this</code>.
		''' 
		''' <p>
		''' Since the <code>Duration</code> class is immutable, this method
		''' doesn't change the value of this object. It simply computes
		''' a new Duration object and returns it.
		''' 
		''' @return
		'''      always return a non-null valid <code>Duration</code> object.
		''' </summary>
		Public MustOverride Function negate() As Duration

		''' <summary>
		''' <p>Converts the years and months fields into the days field
		''' by using a specific time instant as the reference point.</p>
		''' 
		''' <p>For example, duration of one month normalizes to 31 days
		''' given the start time instance "July 8th 2003, 17:40:32".</p>
		''' 
		''' <p>Formally, the computation is done as follows:</p>
		''' <ol>
		'''  <li>the given Calendar object is cloned</li>
		'''  <li>the years, months and days fields will be added to the <seealso cref="Calendar"/> object
		'''      by using the <seealso cref="Calendar#add(int,int)"/> method</li>
		'''  <li>the difference between the two Calendars in computed in milliseconds and converted to days,
		'''     if a remainder occurs due to Daylight Savings Time, it is discarded</li>
		'''  <li>the computed days, along with the hours, minutes and seconds
		'''      fields of this duration object is used to construct a new
		'''      Duration object.</li>
		''' </ol>
		''' 
		''' <p>Note that since the Calendar class uses <code>int</code> to
		''' hold the value of year and month, this method may produce
		''' an unexpected result if this duration object holds
		''' a very large value in the years or months fields.</p>
		''' </summary>
		''' <param name="startTimeInstant"> <code>Calendar</code> reference point.
		''' </param>
		''' <returns> <code>Duration</code> of years and months of this <code>Duration</code> as days.
		''' </returns>
		''' <exception cref="NullPointerException"> If the startTimeInstant parameter is null. </exception>
		Public MustOverride Function normalizeWith(ByVal startTimeInstant As DateTime) As Duration

		''' <summary>
		''' <p>Partial order relation comparison with this <code>Duration</code> instance.</p>
		''' 
		''' <p>Comparison result must be in accordance with
		''' <a href="http://www.w3.org/TR/xmlschema-2/#duration-order">W3C XML Schema 1.0 Part 2, Section 3.2.7.6.2,
		''' <i>Order relation on duration</i></a>.</p>
		''' 
		''' <p>Return:</p>
		''' <ul>
		'''   <li><seealso cref="DatatypeConstants#LESSER"/> if this <code>Duration</code> is shorter than <code>duration</code> parameter</li>
		'''   <li><seealso cref="DatatypeConstants#EQUAL"/> if this <code>Duration</code> is equal to <code>duration</code> parameter</li>
		'''   <li><seealso cref="DatatypeConstants#GREATER"/> if this <code>Duration</code> is longer than <code>duration</code> parameter</li>
		'''   <li><seealso cref="DatatypeConstants#INDETERMINATE"/> if a conclusive partial order relation cannot be determined</li>
		''' </ul>
		''' </summary>
		''' <param name="duration"> to compare
		''' </param>
		''' <returns> the relationship between <code>this</code> <code>Duration</code>and <code>duration</code> parameter as
		'''   <seealso cref="DatatypeConstants#LESSER"/>, <seealso cref="DatatypeConstants#EQUAL"/>, <seealso cref="DatatypeConstants#GREATER"/>
		'''   or <seealso cref="DatatypeConstants#INDETERMINATE"/>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> If the underlying implementation
		'''   cannot reasonably process the request, e.g. W3C XML Schema allows for
		'''   arbitrarily large/small/precise values, the request may be beyond the
		'''   implementations capability. </exception>
		''' <exception cref="NullPointerException"> if <code>duration</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #isShorterThan(Duration) </seealso>
		''' <seealso cref= #isLongerThan(Duration) </seealso>
		Public MustOverride Function compare(ByVal duration As Duration) As Integer

		''' <summary>
		''' <p>Checks if this duration object is strictly longer than
		''' another <code>Duration</code> object.</p>
		''' 
		''' <p>Duration X is "longer" than Y if and only if X>Y
		''' as defined in the section 3.2.6.2 of the XML Schema 1.0
		''' specification.</p>
		''' 
		''' <p>For example, "P1D" (one day) > "PT12H" (12 hours) and
		''' "P2Y" (two years) > "P23M" (23 months).</p>
		''' </summary>
		''' <param name="duration"> <code>Duration</code> to test this <code>Duration</code> against.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> If the underlying implementation
		'''   cannot reasonably process the request, e.g. W3C XML Schema allows for
		'''   arbitrarily large/small/precise values, the request may be beyond the
		'''   implementations capability. </exception>
		''' <exception cref="NullPointerException"> If <code>duration</code> is null.
		''' 
		''' @return
		'''      true if the duration represented by this object
		'''      is longer than the given duration. false otherwise.
		''' </exception>
		''' <seealso cref= #isShorterThan(Duration) </seealso>
		''' <seealso cref= #compare(Duration duration) </seealso>
		Public Overridable Function isLongerThan(ByVal duration As Duration) As Boolean
			Return compare(duration) = DatatypeConstants.GREATER
		End Function

		''' <summary>
		''' <p>Checks if this duration object is strictly shorter than
		''' another <code>Duration</code> object.</p>
		''' </summary>
		''' <param name="duration"> <code>Duration</code> to test this <code>Duration</code> against.
		''' </param>
		''' <returns> <code>true</code> if <code>duration</code> parameter is shorter than this <code>Duration</code>,
		'''   else <code>false</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> If the underlying implementation
		'''   cannot reasonably process the request, e.g. W3C XML Schema allows for
		'''   arbitrarily large/small/precise values, the request may be beyond the
		'''   implementations capability. </exception>
		''' <exception cref="NullPointerException"> if <code>duration</code> is null.
		''' </exception>
		''' <seealso cref= #isLongerThan(Duration duration) </seealso>
		''' <seealso cref= #compare(Duration duration) </seealso>
		Public Overridable Function isShorterThan(ByVal duration As Duration) As Boolean
			Return compare(duration) = DatatypeConstants.LESSER
		End Function

		''' <summary>
		''' <p>Checks if this duration object has the same duration
		''' as another <code>Duration</code> object.</p>
		''' 
		''' <p>For example, "P1D" (1 day) is equal to "PT24H" (24 hours).</p>
		''' 
		''' <p>Duration X is equal to Y if and only if time instant
		''' t+X and t+Y are the same for all the test time instants
		''' specified in the section 3.2.6.2 of the XML Schema 1.0
		''' specification.</p>
		''' 
		''' <p>Note that there are cases where two <code>Duration</code>s are
		''' "incomparable" to each other, like one month and 30 days.
		''' For example,</p>
		''' <pre>
		''' !new Duration("P1M").isShorterThan(new Duration("P30D"))
		''' !new Duration("P1M").isLongerThan(new Duration("P30D"))
		''' !new Duration("P1M").equals(new Duration("P30D"))
		''' </pre>
		''' </summary>
		''' <param name="duration">
		'''      The object to compare this <code>Duration</code> against.
		''' 
		''' @return
		'''      <code>true</code> if this duration is the same length as
		'''         <code>duration</code>.
		'''      <code>false</code> if <code>duration</code> is <code>null</code>,
		'''         is not a
		'''         <code>Duration</code> object,
		'''         or its length is different from this duration.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> If the underlying implementation
		'''   cannot reasonably process the request, e.g. W3C XML Schema allows for
		'''   arbitrarily large/small/precise values, the request may be beyond the
		'''   implementations capability.
		''' </exception>
		''' <seealso cref= #compare(Duration duration) </seealso>
		Public Overrides Function Equals(ByVal duration As Object) As Boolean

			If duration Is Nothing OrElse Not(TypeOf duration Is Duration) Then Return False

			Return compare(CType(duration, Duration)) = DatatypeConstants.EQUAL
		End Function

		''' <summary>
		''' Returns a hash code consistent with the definition of the equals method.
		''' </summary>
		''' <seealso cref= Object#hashCode() </seealso>
		Public MustOverride Function GetHashCode() As Integer

		''' <summary>
		''' <p>Returns a <code>String</code> representation of this <code>Duration</code> <code>Object</code>.</p>
		''' 
		''' <p>The result is formatted according to the XML Schema 1.0 spec and can be always parsed back later into the
		''' equivalent <code>Duration</code> <code>Object</code> by <seealso cref="DatatypeFactory#newDuration(String  lexicalRepresentation)"/>.</p>
		''' 
		''' <p>Formally, the following holds for any <code>Duration</code>
		''' <code>Object</code> x:</p>
		''' <pre>
		''' new Duration(x.toString()).equals(x)
		''' </pre>
		''' </summary>
		''' <returns> A non-<code>null</code> valid <code>String</code> representation of this <code>Duration</code>. </returns>
		Public Overrides Function ToString() As String

			Dim buf As New StringBuilder

			If sign < 0 Then buf.Append("-"c)
			buf.Append("P"c)

			Dim ___years As System.Numerics.BigInteger = CType(getField(DatatypeConstants.YEARS), System.Numerics.BigInteger)
			If ___years IsNot Nothing Then buf.Append(___years & "Y")

			Dim ___months As System.Numerics.BigInteger = CType(getField(DatatypeConstants.MONTHS), System.Numerics.BigInteger)
			If ___months IsNot Nothing Then buf.Append(___months & "M")

			Dim ___days As System.Numerics.BigInteger = CType(getField(DatatypeConstants.DAYS), System.Numerics.BigInteger)
			If ___days IsNot Nothing Then buf.Append(___days & "D")

			Dim ___hours As System.Numerics.BigInteger = CType(getField(DatatypeConstants.HOURS), System.Numerics.BigInteger)
			Dim ___minutes As System.Numerics.BigInteger = CType(getField(DatatypeConstants.MINUTES), System.Numerics.BigInteger)
			Dim ___seconds As Decimal = CDec(getField(DatatypeConstants.SECONDS))
			If ___hours IsNot Nothing OrElse ___minutes IsNot Nothing OrElse ___seconds IsNot Nothing Then
				buf.Append("T"c)
				If ___hours IsNot Nothing Then buf.Append(___hours & "H")
				If ___minutes IsNot Nothing Then buf.Append(___minutes & "M")
				If ___seconds IsNot Nothing Then buf.Append(ToString(___seconds) & "S")
			End If

			Return buf.ToString()
		End Function

		''' <summary>
		''' <p>Turns <seealso cref="BigDecimal"/> to a string representation.</p>
		''' 
		''' <p>Due to a behavior change in the <seealso cref="BigDecimal#toString()"/>
		''' method in JDK1.5, this had to be implemented here.</p>
		''' </summary>
		''' <param name="bd"> <code>BigDecimal</code> to format as a <code>String</code>
		''' </param>
		''' <returns>  <code>String</code> representation of <code>BigDecimal</code> </returns>
		Private Overrides Function ToString(ByVal bd As Decimal) As String
			Dim intString As String = bd.unscaledValue().ToString()
			Dim scale As Integer = bd.scale()

			If scale = 0 Then Return intString

			' Insert decimal point 
			Dim buf As StringBuilder
			Dim insertionPoint As Integer = intString.Length - scale
			If insertionPoint = 0 Then ' Point goes right before intVal
				Return "0." & intString ' Point goes inside intVal
			ElseIf insertionPoint > 0 Then
				buf = New StringBuilder(intString)
				buf.Insert(insertionPoint, "."c) ' We must insert zeros between point and intVal
			Else
				buf = New StringBuilder(3 - insertionPoint + intString.Length)
				buf.Append("0.")
				For i As Integer = 0 To -insertionPoint - 1
					buf.Append("0"c)
				Next i
				buf.Append(intString)
			End If
			Return buf.ToString()
		End Function


		''' <summary>
		''' <p>Calls the <seealso cref="Calendar#getTimeInMillis"/> method.
		''' Prior to JDK1.4, this method was protected and therefore
		''' cannot be invoked directly.</p>
		''' 
		''' <p>TODO: In future, this should be replaced by <code>cal.getTimeInMillis()</code>.</p>
		''' </summary>
		''' <param name="cal"> <code>Calendar</code> to get time in milliseconds.
		''' </param>
		''' <returns> Milliseconds of <code>cal</code>. </returns>
		Private Shared Function getCalendarTimeInMillis(ByVal cal As DateTime) As Long
			Return cal.time
		End Function
	End Class

End Namespace