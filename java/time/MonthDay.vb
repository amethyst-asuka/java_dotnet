Imports System

'
' * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights reserved.
' *
' * Redistribution and use in source and binary forms, with or without
' * modification, are permitted provided that the following conditions are met:
' *
' *  * Redistributions of source code must retain the above copyright notice,
' *    this list of conditions and the following disclaimer.
' *
' *  * Redistributions in binary form must reproduce the above copyright notice,
' *    this list of conditions and the following disclaimer in the documentation
' *    and/or other materials provided with the distribution.
' *
' *  * Neither the name of JSR-310 nor the names of its contributors
' *    may be used to endorse or promote products derived from this software
' *    without specific prior written permission.
' *
' * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
' * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
' * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
' * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
' * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
' * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
' * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
' * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
' * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
' * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
' 
Namespace java.time



	''' <summary>
	''' A month-day in the ISO-8601 calendar system, such as {@code --12-03}.
	''' <p>
	''' {@code MonthDay} is an immutable date-time object that represents the combination
	''' of a month and day-of-month. Any field that can be derived from a month and day,
	''' such as quarter-of-year, can be obtained.
	''' <p>
	''' This class does not store or represent a year, time or time-zone.
	''' For example, the value "December 3rd" can be stored in a {@code MonthDay}.
	''' <p>
	''' Since a {@code MonthDay} does not possess a year, the leap day of
	''' February 29th is considered valid.
	''' <p>
	''' This class implements <seealso cref="TemporalAccessor"/> rather than <seealso cref="Temporal"/>.
	''' This is because it is not possible to define whether February 29th is valid or not
	''' without external information, preventing the implementation of plus/minus.
	''' Related to this, {@code MonthDay} only provides access to query and set the fields
	''' {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH}.
	''' <p>
	''' The ISO-8601 calendar system is the modern civil calendar system used today
	''' in most of the world. It is equivalent to the proleptic Gregorian calendar
	''' system, in which today's rules for leap years are applied for all time.
	''' For most applications written today, the ISO-8601 rules are entirely suitable.
	''' However, any application that makes use of historical dates, and requires them
	''' to be accurate will find the ISO-8601 approach unsuitable.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code MonthDay} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class MonthDay
		Implements java.time.temporal.TemporalAccessor, java.time.temporal.TemporalAdjuster, Comparable(Of MonthDay)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -939150713474957432L
		''' <summary>
		''' Parser.
		''' </summary>
		Private Shared ReadOnly PARSER As java.time.format.DateTimeFormatter = (New java.time.format.DateTimeFormatterBuilder).appendLiteral("--").appendValue(MONTH_OF_YEAR, 2).appendLiteral("-"c).appendValue(DAY_OF_MONTH, 2).toFormatter()

		''' <summary>
		''' The month-of-year, not null.
		''' </summary>
		Private ReadOnly month As Integer
		''' <summary>
		''' The day-of-month.
		''' </summary>
		Private ReadOnly day As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current month-day from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current month-day.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current month-day using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As MonthDay
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current month-day from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current month-day.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current month-day using the system clock, not null </returns>
		Public Shared Function now(  zone As ZoneId) As MonthDay
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current month-day from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current month-day.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current month-day, not null </returns>
		Public Shared Function now(  clock_Renamed As Clock) As MonthDay
			Dim now_Renamed As LocalDate = LocalDate.now(clock_Renamed) ' called once
			Return MonthDay.of(now_Renamed.month, now_Renamed.dayOfMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code MonthDay}.
		''' <p>
		''' The day-of-month must be valid for the month within a leap year.
		''' Hence, for February, day 29 is valid.
		''' <p>
		''' For example, passing in April and day 31 will throw an exception, as
		''' there can never be April 31st in any year. By contrast, passing in
		''' February 29th is permitted, as that month-day can sometimes be valid.
		''' </summary>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <returns> the month-day, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month </exception>
		Public Shared Function [of](  month As Month,   dayOfMonth As Integer) As MonthDay
			java.util.Objects.requireNonNull(month, "month")
			DAY_OF_MONTH.checkValidValue(dayOfMonth)
			If dayOfMonth > month.maxLength() Then Throw New DateTimeException("Illegal value for DayOfMonth field, value " & dayOfMonth & " is not valid for month " & month.name())
			Return New MonthDay(month.value, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains an instance of {@code MonthDay}.
		''' <p>
		''' The day-of-month must be valid for the month within a leap year.
		''' Hence, for month 2 (February), day 29 is valid.
		''' <p>
		''' For example, passing in month 4 (April) and day 31 will throw an exception, as
		''' there can never be April 31st in any year. By contrast, passing in
		''' February 29th is permitted, as that month-day can sometimes be valid.
		''' </summary>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <returns> the month-day, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month </exception>
		Public Shared Function [of](  month As Integer,   dayOfMonth As Integer) As MonthDay
			Return [of](Month.of(month), dayOfMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code MonthDay} from a temporal object.
		''' <p>
		''' This obtains a month-day based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code MonthDay}.
		''' <p>
		''' The conversion extracts the <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> and
		''' <seealso cref="ChronoField#DAY_OF_MONTH DAY_OF_MONTH"/> fields.
		''' The extraction is only permitted if the temporal object has an ISO
		''' chronology, or can be converted to a {@code LocalDate}.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code MonthDay::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the month-day, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code MonthDay} </exception>
		Public Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As MonthDay
			If TypeOf temporal Is MonthDay Then Return CType(temporal, MonthDay)
			Try
				If java.time.chrono.IsoChronology.INSTANCE.Equals(java.time.chrono.Chronology.from(temporal)) = False Then temporal = LocalDate.from(temporal)
				Return [of](temporal.get(MONTH_OF_YEAR), temporal.get(DAY_OF_MONTH))
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain MonthDay from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code MonthDay} from a text string such as {@code --12-03}.
		''' <p>
		''' The string must represent a valid month-day.
		''' The format is {@code --MM-dd}.
		''' </summary>
		''' <param name="text">  the text to parse such as "--12-03", not null </param>
		''' <returns> the parsed month-day, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence) As MonthDay
			Return parse(text, PARSER)
		End Function

		''' <summary>
		''' Obtains an instance of {@code MonthDay} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a month-day.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed month-day, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence,   formatter As java.time.format.DateTimeFormatter) As MonthDay
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, MonthDay::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor, previously validated.
		''' </summary>
		''' <param name="month">  the month-of-year to represent, validated from 1 to 12 </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, validated from 1 to 29-31 </param>
		Private Sub New(  month As Integer,   dayOfMonth As Integer)
			Me.month = month
			Me.day = dayOfMonth
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this month-day can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		''' <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code MONTH_OF_YEAR}
		''' <li>{@code YEAR}
		''' </ul>
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this month-day, false if not </returns>
		Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field = MONTH_OF_YEAR OrElse field = DAY_OF_MONTH
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This month-day is used to enhance the accuracy of the returned range.
		''' If it is not possible to return the range, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return
		''' appropriate range instances.
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the range can be obtained is determined by the field.
		''' </summary>
		''' <param name="field">  the field to query the range for, not null </param>
		''' <returns> the range of valid values for the field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		Public Overrides Function range(  field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If field = MONTH_OF_YEAR Then
				Return field.range()
			ElseIf field = DAY_OF_MONTH Then
				Return java.time.temporal.ValueRange.of(1, month.minLength(), month.maxLength())
			End If
			Return outerInstance.range(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this month-day as an {@code int}.
		''' <p>
		''' This queries this month-day for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this month-day.
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		''' passing {@code this} as the argument. Whether the value can be obtained,
		''' and what the value represents, is determined by the field.
		''' </summary>
		''' <param name="field">  the field to get, not null </param>
		''' <returns> the value for the field </returns>
		''' <exception cref="DateTimeException"> if a value for the field cannot be obtained or
		'''         the value is outside the range of valid values for the field </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported or
		'''         the range of values exceeds an {@code int} </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [get](  field As java.time.temporal.TemporalField) As Integer ' override for Javadoc
			Return range(field).checkValidIntValue(getLong(field), field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this month-day as a {@code long}.
		''' <p>
		''' This queries this month-day for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this month-day.
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		''' passing {@code this} as the argument. Whether the value can be obtained,
		''' and what the value represents, is determined by the field.
		''' </summary>
		''' <param name="field">  the field to get, not null </param>
		''' <returns> the value for the field </returns>
		''' <exception cref="DateTimeException"> if a value for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
			If TypeOf field Is java.time.temporal.ChronoField Then
				Select Case CType(field, java.time.temporal.ChronoField)
					' alignedDOW and alignedWOM not supported because they cannot be set in with()
					Case DAY_OF_MONTH
						Return day
					Case MONTH_OF_YEAR
						Return month
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the month-of-year field from 1 to 12.
		''' <p>
		''' This method returns the month as an {@code int} from 1 to 12.
		''' Application code is frequently clearer if the enum <seealso cref="Month"/>
		''' is used by calling <seealso cref="#getMonth()"/>.
		''' </summary>
		''' <returns> the month-of-year, from 1 to 12 </returns>
		''' <seealso cref= #getMonth() </seealso>
		Public Property monthValue As Integer
			Get
				Return month
			End Get
		End Property

		''' <summary>
		''' Gets the month-of-year field using the {@code Month} enum.
		''' <p>
		''' This method returns the enum <seealso cref="Month"/> for the month.
		''' This avoids confusion as to what {@code int} values mean.
		''' If you need access to the primitive {@code int} value then the enum
		''' provides the <seealso cref="Month#getValue() int value"/>.
		''' </summary>
		''' <returns> the month-of-year, not null </returns>
		''' <seealso cref= #getMonthValue() </seealso>
		Public Property month As Month
			Get
				Return Month.of(month)
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-month field.
		''' <p>
		''' This method returns the primitive {@code int} value for the day-of-month.
		''' </summary>
		''' <returns> the day-of-month, from 1 to 31 </returns>
		Public Property dayOfMonth As Integer
			Get
				Return day
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the year is valid for this month-day.
		''' <p>
		''' This method checks whether this month and day and the input year form
		''' a valid date. This can only return false for February 29th.
		''' </summary>
		''' <param name="year">  the year to validate </param>
		''' <returns> true if the year is valid for this month-day </returns>
		''' <seealso cref= Year#isValidMonthDay(MonthDay) </seealso>
		Public Function isValidYear(  year_Renamed As Integer) As Boolean
			Return (day = 29 AndAlso month = 2 AndAlso Year.isLeap(year_Renamed) = False) = False
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code MonthDay} with the month-of-year altered.
		''' <p>
		''' This returns a month-day with the specified month.
		''' If the day-of-month is invalid for the specified month, the day will
		''' be adjusted to the last valid day-of-month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the returned month-day, from 1 (January) to 12 (December) </param>
		''' <returns> a {@code MonthDay} based on this month-day with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(  month As Integer) As MonthDay
			Return [with](Month.of(month))
		End Function

		''' <summary>
		''' Returns a copy of this {@code MonthDay} with the month-of-year altered.
		''' <p>
		''' This returns a month-day with the specified month.
		''' If the day-of-month is invalid for the specified month, the day will
		''' be adjusted to the last valid day-of-month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the returned month-day, not null </param>
		''' <returns> a {@code MonthDay} based on this month-day with the requested month, not null </returns>
		Public Function [with](  month As Month) As MonthDay
			java.util.Objects.requireNonNull(month, "month")
			If month.value = Me.month Then Return Me
			Dim day As Integer = System.Math.Min(Me.day, month.maxLength())
			Return New MonthDay(month.value, day)
		End Function

		''' <summary>
		''' Returns a copy of this {@code MonthDay} with the day-of-month altered.
		''' <p>
		''' This returns a month-day with the specified day-of-month.
		''' If the day-of-month is invalid for the month, an exception is thrown.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to set in the return month-day, from 1 to 31 </param>
		''' <returns> a {@code MonthDay} based on this month-day with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month value is invalid,
		'''  or if the day-of-month is invalid for the month </exception>
		Public Function withDayOfMonth(  dayOfMonth As Integer) As MonthDay
			If dayOfMonth = Me.day Then Return Me
			Return [of](month, dayOfMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this month-day using the specified query.
		''' <p>
		''' This queries this month-day using the specified query strategy object.
		''' The {@code TemporalQuery} object defines the logic to be used to
		''' obtain the result. Read the documentation of the query to understand
		''' what the result of this method will be.
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalQuery#queryFrom(TemporalAccessor)"/> method on the
		''' specified query passing {@code this} as the argument.
		''' </summary>
		''' @param <R> the type of the result </param>
		''' <param name="query">  the query to invoke, not null </param>
		''' <returns> the query result, null may be returned (defined by the query) </returns>
		''' <exception cref="DateTimeException"> if unable to query (defined by the query) </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs (defined by the query) </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function query(Of R)(  query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
			If query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then Return CType(java.time.chrono.IsoChronology.INSTANCE, R)
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have this month-day.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the month and day-of-month changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' twice, passing <seealso cref="ChronoField#MONTH_OF_YEAR"/> and
		''' <seealso cref="ChronoField#DAY_OF_MONTH"/> as the fields.
		''' If the specified temporal object does not use the ISO calendar system then
		''' a {@code DateTimeException} is thrown.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisMonthDay.adjustInto(temporal);
		'''   temporal = temporal.with(thisMonthDay);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(  temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			If java.time.chrono.Chronology.from(temporal).Equals(java.time.chrono.IsoChronology.INSTANCE) = False Then Throw New DateTimeException("Adjustment only supported on ISO date-time")
			temporal = temporal.with(MONTH_OF_YEAR, month)
			Return temporal.with(DAY_OF_MONTH, System.Math.Min(temporal.range(DAY_OF_MONTH).maximum, day))
		End Function

		''' <summary>
		''' Formats this month-day using the specified formatter.
		''' <p>
		''' This month-day will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted month-day string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Function format(  formatter As java.time.format.DateTimeFormatter) As String
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this month-day with a year to create a {@code LocalDate}.
		''' <p>
		''' This returns a {@code LocalDate} formed from this month-day and the specified year.
		''' <p>
		''' A month-day of February 29th will be adjusted to February 28th in the resulting
		''' date if the year is not a leap year.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to use, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> the local date formed from this month-day and the specified year, not null </returns>
		''' <exception cref="DateTimeException"> if the year is outside the valid range of years </exception>
		Public Function atYear(  year_Renamed As Integer) As LocalDate
			Return LocalDate.of(year_Renamed, month,If(isValidYear(year_Renamed), day, 28))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this month-day to another month-day.
		''' <p>
		''' The comparison is based first on value of the month, then on the value of the day.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other month-day to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(  other As MonthDay) As Integer Implements Comparable(Of MonthDay).compareTo
			Dim cmp As Integer = (month - other.month)
			If cmp = 0 Then cmp = (day - other.day)
			Return cmp
		End Function

		''' <summary>
		''' Checks if this month-day is after the specified month-day.
		''' </summary>
		''' <param name="other">  the other month-day to compare to, not null </param>
		''' <returns> true if this is after the specified month-day </returns>
		Public Function isAfter(  other As MonthDay) As Boolean
			Return compareTo(other) > 0
		End Function

		''' <summary>
		''' Checks if this month-day is before the specified month-day.
		''' </summary>
		''' <param name="other">  the other month-day to compare to, not null </param>
		''' <returns> true if this point is before the specified month-day </returns>
		Public Function isBefore(  other As MonthDay) As Boolean
			Return compareTo(other) < 0
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this month-day is equal to another month-day.
		''' <p>
		''' The comparison is based on the time-line position of the month-day within a year.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other month-day </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is MonthDay Then
				Dim other As MonthDay = CType(obj, MonthDay)
				Return month = other.month AndAlso day = other.day
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this month-day.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (month << 6) + day
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this month-day as a {@code String}, such as {@code --12-03}.
		''' <p>
		''' The output will be in the format {@code --MM-dd}:
		''' </summary>
		''' <returns> a string representation of this month-day, not null </returns>
		Public Overrides Function ToString() As String
			Return (New StringBuilder(10)).append("--").append(If(month < 10, "0", "")).append(month).append(If(day < 10, "-0", "-")).append(day).ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(13);  // identifies a MonthDay
		'''  out.writeByte(month);
		'''  out.writeByte(day);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.MONTH_DAY_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(  out As java.io.DataOutput)
			out.writeByte(month)
			out.writeByte(day)
		End Sub

		Shared Function readExternal(  [in] As java.io.DataInput) As MonthDay
			Dim month_Renamed As SByte = [in].readByte()
			Dim day As SByte = [in].readByte()
			Return MonthDay.of(month_Renamed, day)
		End Function

	End Class

End Namespace