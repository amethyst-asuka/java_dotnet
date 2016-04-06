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
	''' A date-time with an offset from UTC/Greenwich in the ISO-8601 calendar system,
	''' such as {@code 2007-12-03T10:15:30+01:00}.
	''' <p>
	''' {@code OffsetDateTime} is an immutable representation of a date-time with an offset.
	''' This class stores all date and time fields, to a precision of nanoseconds,
	''' as well as the offset from UTC/Greenwich. For example, the value
	''' "2nd October 2007 at 13:45.30.123456789 +02:00" can be stored in an {@code OffsetDateTime}.
	''' <p>
	''' {@code OffsetDateTime}, <seealso cref="java.time.ZonedDateTime"/> and <seealso cref="java.time.Instant"/> all store an instant
	''' on the time-line to nanosecond precision.
	''' {@code Instant} is the simplest, simply representing the instant.
	''' {@code OffsetDateTime} adds to the instant the offset from UTC/Greenwich, which allows
	''' the local date-time to be obtained.
	''' {@code ZonedDateTime} adds full time-zone rules.
	''' <p>
	''' It is intended that {@code ZonedDateTime} or {@code Instant} is used to model data
	''' in simpler applications. This class may be used when modeling date-time concepts in
	''' more detail, or when communicating to a database or in a network protocol.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code OffsetDateTime} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class OffsetDateTime
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of OffsetDateTime)

		''' <summary>
		''' The minimum supported {@code OffsetDateTime}, '-999999999-01-01T00:00:00+18:00'.
		''' This is the local date-time of midnight at the start of the minimum date
		''' in the maximum offset (larger offsets are earlier on the time-line).
		''' This combines <seealso cref="LocalDateTime#MIN"/> and <seealso cref="ZoneOffset#MAX"/>.
		''' This could be used by an application as a "far past" date-time.
		''' </summary>
		Public Shared ReadOnly MIN As OffsetDateTime = LocalDateTime.MIN.atOffset(ZoneOffset.MAX)
		''' <summary>
		''' The maximum supported {@code OffsetDateTime}, '+999999999-12-31T23:59:59.999999999-18:00'.
		''' This is the local date-time just before midnight at the end of the maximum date
		''' in the minimum offset (larger negative offsets are later on the time-line).
		''' This combines <seealso cref="LocalDateTime#MAX"/> and <seealso cref="ZoneOffset#MIN"/>.
		''' This could be used by an application as a "far future" date-time.
		''' </summary>
		Public Shared ReadOnly MAX As OffsetDateTime = LocalDateTime.MAX.atOffset(ZoneOffset.MIN)

		''' <summary>
		''' Gets a comparator that compares two {@code OffsetDateTime} instances
		''' based solely on the instant.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying instant.
		''' </summary>
		''' <returns> a comparator that compares in time-line order
		''' </returns>
		''' <seealso cref= #isAfter </seealso>
		''' <seealso cref= #isBefore </seealso>
		''' <seealso cref= #isEqual </seealso>
		Public Shared Function timeLineOrder() As IComparer(Of OffsetDateTime)
			Return OffsetDateTime::compareInstant
		End Function

		''' <summary>
		''' Compares this {@code OffsetDateTime} to another date-time.
		''' The comparison is based on the instant.
		''' </summary>
		''' <param name="datetime1">  the first date-time to compare, not null </param>
		''' <param name="datetime2">  the other date-time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Private Shared Function compareInstant(  datetime1 As OffsetDateTime,   datetime2 As OffsetDateTime) As Integer
			If datetime1.offset.Equals(datetime2.offset) Then Return datetime1.toLocalDateTime().CompareTo(datetime2.toLocalDateTime())
			Dim cmp As Integer = java.lang.[Long].Compare(datetime1.toEpochSecond(), datetime2.toEpochSecond())
			If cmp = 0 Then cmp = datetime1.toLocalTime().nano - datetime2.toLocalTime().nano
			Return cmp
		End Function

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 2287754244819255394L

		''' <summary>
		''' The local date-time.
		''' </summary>
		Private ReadOnly dateTime As LocalDateTime
		''' <summary>
		''' The offset from UTC/Greenwich.
		''' </summary>
		Private ReadOnly offset As ZoneOffset

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current date-time from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date-time.
		''' The offset will be calculated from the time-zone in the clock.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date-time using the system clock, not null </returns>
		Public Shared Function now() As OffsetDateTime
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current date-time from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date-time.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' The offset will be calculated from the specified time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date-time using the system clock, not null </returns>
		Public Shared Function now(  zone As ZoneId) As OffsetDateTime
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current date-time from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date-time.
		''' The offset will be calculated from the time-zone in the clock.
		''' <p>
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date-time, not null </returns>
		Public Shared Function now(  clock_Renamed As Clock) As OffsetDateTime
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Return ofInstant(now_Renamed, clock_Renamed.zone.rules.getOffset(now_Renamed))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a date, time and offset.
		''' <p>
		''' This creates an offset date-time with the specified local date, time and offset.
		''' </summary>
		''' <param name="date">  the local date, not null </param>
		''' <param name="time">  the local time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the offset date-time, not null </returns>
		Public Shared Function [of](  [date] As LocalDate,   time As LocalTime,   offset As ZoneOffset) As OffsetDateTime
			Dim dt As LocalDateTime = LocalDateTime.of(date_Renamed, time)
			Return New OffsetDateTime(dt, offset)
		End Function

		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a date-time and offset.
		''' <p>
		''' This creates an offset date-time with the specified local date-time and offset.
		''' </summary>
		''' <param name="dateTime">  the local date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the offset date-time, not null </returns>
		Public Shared Function [of](  dateTime As LocalDateTime,   offset As ZoneOffset) As OffsetDateTime
			Return New OffsetDateTime(dateTime, offset)
		End Function

		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a year, month, day,
		''' hour, minute, second, nanosecond and offset.
		''' <p>
		''' This creates an offset date-time with the seven specified fields.
		''' <p>
		''' This method exists primarily for writing test cases.
		''' Non test-code will typically use other methods to create an offset time.
		''' {@code LocalDateTime} has five additional convenience variants of the
		''' equivalent factory method taking fewer arguments.
		''' They are not provided here to reduce the footprint of the API.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the offset date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range, or
		'''  if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](  year_Renamed As Integer,   month As Integer,   dayOfMonth As Integer,   hour As Integer,   minute As Integer,   second As Integer,   nanoOfSecond As Integer,   offset As ZoneOffset) As OffsetDateTime
			Dim dt As LocalDateTime = LocalDateTime.of(year_Renamed, month, dayOfMonth, hour, minute, second, nanoOfSecond)
			Return New OffsetDateTime(dt, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from an {@code Instant} and zone ID.
		''' <p>
		''' This creates an offset date-time with the same instant as that specified.
		''' Finding the offset from UTC/Greenwich is simple as there is only one valid
		''' offset for each instant.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone, which may be an offset, not null </param>
		''' <returns> the offset date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Shared Function ofInstant(  instant_Renamed As Instant,   zone As ZoneId) As OffsetDateTime
			java.util.Objects.requireNonNull(instant_Renamed, "instant")
			java.util.Objects.requireNonNull(zone, "zone")
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim offset_Renamed As ZoneOffset = rules.getOffset(instant_Renamed)
			Dim ldt As LocalDateTime = LocalDateTime.ofEpochSecond(instant_Renamed.epochSecond, instant_Renamed.nano, offset_Renamed)
			Return New OffsetDateTime(ldt, offset_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a temporal object.
		''' <p>
		''' This obtains an offset date-time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code OffsetDateTime}.
		''' <p>
		''' The conversion will first obtain a {@code ZoneOffset} from the temporal object.
		''' It will then try to obtain a {@code LocalDateTime}, falling back to an {@code Instant} if necessary.
		''' The result will be the combination of {@code ZoneOffset} with either
		''' with {@code LocalDateTime} or {@code Instant}.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code OffsetDateTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the offset date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code OffsetDateTime} </exception>
		Public Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As OffsetDateTime
			If TypeOf temporal Is OffsetDateTime Then Return CType(temporal, OffsetDateTime)
			Try
				Dim offset_Renamed As ZoneOffset = ZoneOffset.from(temporal)
				Dim date_Renamed As LocalDate = temporal.query(java.time.temporal.TemporalQueries.localDate())
				Dim time As LocalTime = temporal.query(java.time.temporal.TemporalQueries.localTime())
				If date_Renamed IsNot Nothing AndAlso time IsNot Nothing Then
					Return OffsetDateTime.of(date_Renamed, time, offset_Renamed)
				Else
					Dim instant_Renamed As Instant = Instant.from(temporal)
					Return OffsetDateTime.ofInstant(instant_Renamed, offset_Renamed)
				End If
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain OffsetDateTime from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a text string
		''' such as {@code 2007-12-03T10:15:30+01:00}.
		''' <p>
		''' The string must represent a valid date-time and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_OFFSET_DATE_TIME"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007-12-03T10:15:30+01:00", not null </param>
		''' <returns> the parsed offset date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence) As OffsetDateTime
			Return parse(text, java.time.format.DateTimeFormatter.ISO_OFFSET_DATE_TIME)
		End Function

		''' <summary>
		''' Obtains an instance of {@code OffsetDateTime} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a date-time.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed offset date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence,   formatter As java.time.format.DateTimeFormatter) As OffsetDateTime
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, OffsetDateTime::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="dateTime">  the local date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		Private Sub New(  dateTime As LocalDateTime,   offset As ZoneOffset)
			Me.dateTime = java.util.Objects.requireNonNull(dateTime, "dateTime")
			Me.offset = java.util.Objects.requireNonNull(offset, "offset")
		End Sub

		''' <summary>
		''' Returns a new date-time based on this one, returning {@code this} where possible.
		''' </summary>
		''' <param name="dateTime">  the date-time to create with, not null </param>
		''' <param name="offset">  the zone offset to create with, not null </param>
		Private Function [with](  dateTime As LocalDateTime,   offset As ZoneOffset) As OffsetDateTime
			If Me.dateTime Is dateTime AndAlso Me.offset.Equals(offset) Then Return Me
			Return New OffsetDateTime(dateTime, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this date-time can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code NANO_OF_SECOND}
		''' <li>{@code NANO_OF_DAY}
		''' <li>{@code MICRO_OF_SECOND}
		''' <li>{@code MICRO_OF_DAY}
		''' <li>{@code MILLI_OF_SECOND}
		''' <li>{@code MILLI_OF_DAY}
		''' <li>{@code SECOND_OF_MINUTE}
		''' <li>{@code SECOND_OF_DAY}
		''' <li>{@code MINUTE_OF_HOUR}
		''' <li>{@code MINUTE_OF_DAY}
		''' <li>{@code HOUR_OF_AMPM}
		''' <li>{@code CLOCK_HOUR_OF_AMPM}
		''' <li>{@code HOUR_OF_DAY}
		''' <li>{@code CLOCK_HOUR_OF_DAY}
		''' <li>{@code AMPM_OF_DAY}
		''' <li>{@code DAY_OF_WEEK}
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_MONTH}
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_YEAR}
		''' <li>{@code DAY_OF_MONTH}
		''' <li>{@code DAY_OF_YEAR}
		''' <li>{@code EPOCH_DAY}
		''' <li>{@code ALIGNED_WEEK_OF_MONTH}
		''' <li>{@code ALIGNED_WEEK_OF_YEAR}
		''' <li>{@code MONTH_OF_YEAR}
		''' <li>{@code PROLEPTIC_MONTH}
		''' <li>{@code YEAR_OF_ERA}
		''' <li>{@code YEAR}
		''' <li>{@code ERA}
		''' <li>{@code INSTANT_SECONDS}
		''' <li>{@code OFFSET_SECONDS}
		''' </ul>
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this date-time, false if not </returns>
		Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
			Return TypeOf field Is java.time.temporal.ChronoField OrElse (field IsNot Nothing AndAlso field.isSupportedBy(Me))
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this date-time.
		''' If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		''' <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		''' <p>
		''' If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		''' The supported units are:
		''' <ul>
		''' <li>{@code NANOS}
		''' <li>{@code MICROS}
		''' <li>{@code MILLIS}
		''' <li>{@code SECONDS}
		''' <li>{@code MINUTES}
		''' <li>{@code HOURS}
		''' <li>{@code HALF_DAYS}
		''' <li>{@code DAYS}
		''' <li>{@code WEEKS}
		''' <li>{@code MONTHS}
		''' <li>{@code YEARS}
		''' <li>{@code DECADES}
		''' <li>{@code CENTURIES}
		''' <li>{@code MILLENNIA}
		''' <li>{@code ERAS}
		''' </ul>
		''' All other {@code ChronoUnit} instances will return false.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.isSupportedBy(Temporal)}
		''' passing {@code this} as the argument.
		''' Whether the unit is supported is determined by the unit.
		''' </summary>
		''' <param name="unit">  the unit to check, null returns false </param>
		''' <returns> true if the unit can be added/subtracted, false if not </returns>
		Public Overrides Function isSupported(  unit As java.time.temporal.TemporalUnit) As Boolean ' override for Javadoc
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return unit <> FOREVER
			Return unit IsNot Nothing AndAlso unit.isSupportedBy(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This date-time is used to enhance the accuracy of the returned range.
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
			If TypeOf field Is java.time.temporal.ChronoField Then
				If field = INSTANT_SECONDS OrElse field = OFFSET_SECONDS Then Return field.range()
				Return dateTime.range(field)
			End If
			Return field.rangeRefinedBy(Me)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this date-time as an {@code int}.
		''' <p>
		''' This queries this date-time for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this date-time, except {@code NANO_OF_DAY}, {@code MICRO_OF_DAY},
		''' {@code EPOCH_DAY}, {@code PROLEPTIC_MONTH} and {@code INSTANT_SECONDS} which are too
		''' large to fit in an {@code int} and throw a {@code DateTimeException}.
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
		Public Overrides Function [get](  field As java.time.temporal.TemporalField) As Integer
			If TypeOf field Is java.time.temporal.ChronoField Then
				Select Case CType(field, java.time.temporal.ChronoField)
					Case INSTANT_SECONDS
						Throw New java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'InstantSeconds' for get() method, use getLong() instead")
					Case OFFSET_SECONDS
						Return offset.totalSeconds
				End Select
				Return dateTime.get(field)
			End If
			Return outerInstance.get(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this date-time as a {@code long}.
		''' <p>
		''' This queries this date-time for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this date-time.
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
					Case INSTANT_SECONDS
						Return toEpochSecond()
					Case OFFSET_SECONDS
						Return offset.totalSeconds
				End Select
				Return dateTime.getLong(field)
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the zone offset, such as '+01:00'.
		''' <p>
		''' This is the offset of the local date-time from UTC/Greenwich.
		''' </summary>
		''' <returns> the zone offset, not null </returns>
		Public Property offset As ZoneOffset
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified offset ensuring
		''' that the result has the same local date-time.
		''' <p>
		''' This method returns an object with the same {@code LocalDateTime} and the specified {@code ZoneOffset}.
		''' No calculation is needed or performed.
		''' For example, if this time represents {@code 2007-12-03T10:30+02:00} and the offset specified is
		''' {@code +03:00}, then this method will return {@code 2007-12-03T10:30+03:00}.
		''' <p>
		''' To take into account the difference between the offsets, and adjust the time fields,
		''' use <seealso cref="#withOffsetSameInstant"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="offset">  the zone offset to change to, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested offset, not null </returns>
		Public Function withOffsetSameLocal(  offset As ZoneOffset) As OffsetDateTime
			Return [with](dateTime, offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified offset ensuring
		''' that the result is at the same instant.
		''' <p>
		''' This method returns an object with the specified {@code ZoneOffset} and a {@code LocalDateTime}
		''' adjusted by the difference between the two offsets.
		''' This will result in the old and new objects representing the same instant.
		''' This is useful for finding the local time in a different offset.
		''' For example, if this time represents {@code 2007-12-03T10:30+02:00} and the offset specified is
		''' {@code +03:00}, then this method will return {@code 2007-12-03T11:30+03:00}.
		''' <p>
		''' To change the offset without adjusting the local time use <seealso cref="#withOffsetSameLocal"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="offset">  the zone offset to change to, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested offset, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function withOffsetSameInstant(  offset As ZoneOffset) As OffsetDateTime
			If offset.Equals(Me.offset) Then Return Me
			Dim difference As Integer = offset.totalSeconds - Me.offset.totalSeconds
			Dim adjusted As LocalDateTime = dateTime.plusSeconds(difference)
			Return New OffsetDateTime(adjusted, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalDateTime} part of this date-time.
		''' <p>
		''' This returns a {@code LocalDateTime} with the same year, month, day and time
		''' as this date-time.
		''' </summary>
		''' <returns> the local date-time part of this date-time, not null </returns>
		Public Function toLocalDateTime() As LocalDateTime
			Return dateTime
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalDate} part of this date-time.
		''' <p>
		''' This returns a {@code LocalDate} with the same year, month and day
		''' as this date-time.
		''' </summary>
		''' <returns> the date part of this date-time, not null </returns>
		Public Function toLocalDate() As LocalDate
			Return dateTime.toLocalDate()
		End Function

		''' <summary>
		''' Gets the year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the year.
		''' <p>
		''' The year returned by this method is proleptic as per {@code get(YEAR)}.
		''' To obtain the year-of-era, use {@code get(YEAR_OF_ERA)}.
		''' </summary>
		''' <returns> the year, from MIN_YEAR to MAX_YEAR </returns>
		Public Property year As Integer
			Get
				Return dateTime.year
			End Get
		End Property

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
				Return dateTime.monthValue
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
				Return dateTime.month
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
				Return dateTime.dayOfMonth
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the day-of-year.
		''' </summary>
		''' <returns> the day-of-year, from 1 to 365, or 366 in a leap year </returns>
		Public Property dayOfYear As Integer
			Get
				Return dateTime.dayOfYear
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-week field, which is an enum {@code DayOfWeek}.
		''' <p>
		''' This method returns the enum <seealso cref="DayOfWeek"/> for the day-of-week.
		''' This avoids confusion as to what {@code int} values mean.
		''' If you need access to the primitive {@code int} value then the enum
		''' provides the <seealso cref="DayOfWeek#getValue() int value"/>.
		''' <p>
		''' Additional information can be obtained from the {@code DayOfWeek}.
		''' This includes textual names of the values.
		''' </summary>
		''' <returns> the day-of-week, not null </returns>
		Public Property dayOfWeek As DayOfWeek
			Get
				Return dateTime.dayOfWeek
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalTime} part of this date-time.
		''' <p>
		''' This returns a {@code LocalTime} with the same hour, minute, second and
		''' nanosecond as this date-time.
		''' </summary>
		''' <returns> the time part of this date-time, not null </returns>
		Public Function toLocalTime() As LocalTime
			Return dateTime.toLocalTime()
		End Function

		''' <summary>
		''' Gets the hour-of-day field.
		''' </summary>
		''' <returns> the hour-of-day, from 0 to 23 </returns>
		Public Property hour As Integer
			Get
				Return dateTime.hour
			End Get
		End Property

		''' <summary>
		''' Gets the minute-of-hour field.
		''' </summary>
		''' <returns> the minute-of-hour, from 0 to 59 </returns>
		Public Property minute As Integer
			Get
				Return dateTime.minute
			End Get
		End Property

		''' <summary>
		''' Gets the second-of-minute field.
		''' </summary>
		''' <returns> the second-of-minute, from 0 to 59 </returns>
		Public Property second As Integer
			Get
				Return dateTime.second
			End Get
		End Property

		''' <summary>
		''' Gets the nano-of-second field.
		''' </summary>
		''' <returns> the nano-of-second, from 0 to 999,999,999 </returns>
		Public Property nano As Integer
			Get
				Return dateTime.nano
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this date-time.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the date-time adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the year field.
		''' A more complex adjuster might set the date to the last day of the month.
		''' A selection of common adjustments is provided in
		''' <seealso cref="java.time.temporal.TemporalAdjusters TemporalAdjusters"/>.
		''' These include finding the "last day of the month" and "next Wednesday".
		''' Key date-time classes also implement the {@code TemporalAdjuster} interface,
		''' such as <seealso cref="Month"/> and <seealso cref="java.time.MonthDay MonthDay"/>.
		''' The adjuster is responsible for handling special cases, such as the varying
		''' lengths of month and leap years.
		''' <p>
		''' For example this code returns a date on the last day of July:
		''' <pre>
		'''  import static java.time.Month.*;
		'''  import static java.time.temporal.TemporalAdjusters.*;
		''' 
		'''  result = offsetDateTime.with(JULY).with(lastDayOfMonth());
		''' </pre>
		''' <p>
		''' The classes <seealso cref="LocalDate"/>, <seealso cref="LocalTime"/> and <seealso cref="ZoneOffset"/> implement
		''' {@code TemporalAdjuster}, thus this method can be used to change the date, time or offset:
		''' <pre>
		'''  result = offsetDateTime.with(date);
		'''  result = offsetDateTime.with(time);
		'''  result = offsetDateTime.with(offset);
		''' </pre>
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> an {@code OffsetDateTime} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](  adjuster As java.time.temporal.TemporalAdjuster) As OffsetDateTime
			' optimizations
			If TypeOf adjuster Is LocalDate OrElse TypeOf adjuster Is LocalTime OrElse TypeOf adjuster Is LocalDateTime Then
				Return [with](dateTime.with(adjuster), offset)
			ElseIf TypeOf adjuster Is Instant Then
				Return ofInstant(CType(adjuster, Instant), offset)
			ElseIf TypeOf adjuster Is ZoneOffset Then
				Return [with](dateTime, CType(adjuster, ZoneOffset))
			ElseIf TypeOf adjuster Is OffsetDateTime Then
				Return CType(adjuster, OffsetDateTime)
			End If
			Return CType(adjuster.adjustInto(Me), OffsetDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified field set to a new value.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the value
		''' for the specified field changed.
		''' This can be used to change any supported field, such as the year, month or day-of-month.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' In some cases, changing the specified field can cause the resulting date-time to become invalid,
		''' such as changing the month from 31st January to February would make the day-of-month invalid.
		''' In cases like this, the field is responsible for resolving the date. Typically it will choose
		''' the previous valid date, which would be the last valid day of February in this example.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' <p>
		''' The {@code INSTANT_SECONDS} field will return a date-time with the specified instant.
		''' The offset and nano-of-second are unchanged.
		''' If the new instant value is outside the valid range then a {@code DateTimeException} will be thrown.
		''' <p>
		''' The {@code OFFSET_SECONDS} field will return a date-time with the specified offset.
		''' The local date-time is unaltered. If the new offset value is outside the valid range
		''' then a {@code DateTimeException} will be thrown.
		''' <p>
		''' The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		''' the matching method on <seealso cref="LocalDateTime#with(TemporalField, long) LocalDateTime"/>.
		''' In this case, the offset is not part of the calculation and will be unchanged.
		''' <p>
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.adjustInto(Temporal, long)}
		''' passing {@code this} as the argument. In this case, the field determines
		''' whether and how to adjust the instant.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="field">  the field to set in the result, not null </param>
		''' <param name="newValue">  the new value of the field in the result </param>
		''' <returns> an {@code OffsetDateTime} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](  field As java.time.temporal.TemporalField,   newValue As Long) As OffsetDateTime
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Select Case f
					Case INSTANT_SECONDS
						Return ofInstant(Instant.ofEpochSecond(newValue, nano), offset)
					Case OFFSET_SECONDS
						Return [with](dateTime, ZoneOffset.ofTotalSeconds(f.checkValidIntValue(newValue)))
				End Select
				Return [with](dateTime.with(field, newValue), offset)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the year altered.
		''' <p>
		''' The time and offset do not affect the calculation and will be the same in the result.
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested year, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Function withYear(  year_Renamed As Integer) As OffsetDateTime
			Return [with](dateTime.withYear(year_Renamed), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the month-of-year altered.
		''' <p>
		''' The time and offset do not affect the calculation and will be the same in the result.
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(  month As Integer) As OffsetDateTime
			Return [with](dateTime.withMonth(month), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the day-of-month altered.
		''' <p>
		''' If the resulting {@code OffsetDateTime} is invalid, an exception is thrown.
		''' The time and offset do not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month value is invalid,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Function withDayOfMonth(  dayOfMonth As Integer) As OffsetDateTime
			Return [with](dateTime.withDayOfMonth(dayOfMonth), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the day-of-year altered.
		''' <p>
		''' The time and offset do not affect the calculation and will be the same in the result.
		''' If the resulting {@code OffsetDateTime} is invalid, an exception is thrown.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		''' <returns> an {@code OffsetDateTime} based on this date with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-year value is invalid,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Function withDayOfYear(  dayOfYear As Integer) As OffsetDateTime
			Return [with](dateTime.withDayOfYear(dayOfYear), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the hour-of-day altered.
		''' <p>
		''' The date and offset do not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested hour, not null </returns>
		''' <exception cref="DateTimeException"> if the hour value is invalid </exception>
		Public Function withHour(  hour As Integer) As OffsetDateTime
			Return [with](dateTime.withHour(hour), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the minute-of-hour altered.
		''' <p>
		''' The date and offset do not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested minute, not null </returns>
		''' <exception cref="DateTimeException"> if the minute value is invalid </exception>
		Public Function withMinute(  minute As Integer) As OffsetDateTime
			Return [with](dateTime.withMinute(minute), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the second-of-minute altered.
		''' <p>
		''' The date and offset do not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested second, not null </returns>
		''' <exception cref="DateTimeException"> if the second value is invalid </exception>
		Public Function withSecond(  second As Integer) As OffsetDateTime
			Return [with](dateTime.withSecond(second), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the nano-of-second altered.
		''' <p>
		''' The date and offset do not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the requested nanosecond, not null </returns>
		''' <exception cref="DateTimeException"> if the nano value is invalid </exception>
		Public Function withNano(  nanoOfSecond As Integer) As OffsetDateTime
			Return [with](dateTime.withNano(nanoOfSecond), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the time truncated.
		''' <p>
		''' Truncation returns a copy of the original date-time with fields
		''' smaller than the specified unit set to zero.
		''' For example, truncating with the <seealso cref="ChronoUnit#MINUTES minutes"/> unit
		''' will set the second-of-minute and nano-of-second field to zero.
		''' <p>
		''' The unit must have a <seealso cref="TemporalUnit#getDuration() duration"/>
		''' that divides into the length of a standard day without remainder.
		''' This includes all supplied time units on <seealso cref="ChronoUnit"/> and
		''' <seealso cref="ChronoUnit#DAYS DAYS"/>. Other units throw an exception.
		''' <p>
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="unit">  the unit to truncate to, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if unable to truncate </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(  unit As java.time.temporal.TemporalUnit) As OffsetDateTime
			Return [with](dateTime.truncatedTo(unit), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the specified amount added.
		''' The amount is typically <seealso cref="Period"/> or <seealso cref="Duration"/> but may be
		''' any other type implementing the <seealso cref="TemporalAmount"/> interface.
		''' <p>
		''' The calculation is delegated to the amount object by calling
		''' <seealso cref="TemporalAmount#addTo(Temporal)"/>. The amount implementation is free
		''' to implement the addition in any way it wishes, however it typically
		''' calls back to <seealso cref="#plus(long, TemporalUnit)"/>. Consult the documentation
		''' of the amount implementation to determine if it can be successfully added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToAdd">  the amount to add, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(  amountToAdd As java.time.temporal.TemporalAmount) As OffsetDateTime
			Return CType(amountToAdd.addTo(Me), OffsetDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented by
		''' <seealso cref="LocalDateTime#plus(long, TemporalUnit)"/>.
		''' The offset is not part of the calculation and will be unchanged in the result.
		''' <p>
		''' If the field is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.addTo(Temporal, long)}
		''' passing {@code this} as the argument. In this case, the unit determines
		''' whether and how to perform the addition.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToAdd">  the amount of the unit to add to the result, may be negative </param>
		''' <param name="unit">  the unit of the amount to add, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As OffsetDateTime
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return [with](dateTime.plus(amountToAdd, unit), offset)
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of years added.
		''' <p>
		''' This method adds the specified amount to the years field in three steps:
		''' <ol>
		''' <li>Add the input years to the year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2008-02-29 (leap year) plus one year would result in the
		''' invalid date 2009-02-29 (standard year). Instead of returning an invalid
		''' result, the last valid day of the month, 2009-02-28, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="years">  the years to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusYears(  years As Long) As OffsetDateTime
			Return [with](dateTime.plusYears(years), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of months added.
		''' <p>
		''' This method adds the specified amount to the months field in three steps:
		''' <ol>
		''' <li>Add the input months to the month-of-year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2007-03-31 plus one month would result in the invalid date
		''' 2007-04-31. Instead of returning an invalid result, the last valid day
		''' of the month, 2007-04-30, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="months">  the months to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMonths(  months As Long) As OffsetDateTime
			Return [with](dateTime.plusMonths(months), offset)
		End Function

		''' <summary>
		''' Returns a copy of this OffsetDateTime with the specified number of weeks added.
		''' <p>
		''' This method adds the specified amount in weeks to the days field incrementing
		''' the month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 plus one week would result in 2009-01-07.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeks">  the weeks to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the weeks added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusWeeks(  weeks As Long) As OffsetDateTime
			Return [with](dateTime.plusWeeks(weeks), offset)
		End Function

		''' <summary>
		''' Returns a copy of this OffsetDateTime with the specified number of days added.
		''' <p>
		''' This method adds the specified amount to the days field incrementing the
		''' month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 plus one day would result in 2009-01-01.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the days added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusDays(  days As Long) As OffsetDateTime
			Return [with](dateTime.plusDays(days), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of hours added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the hours added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusHours(  hours As Long) As OffsetDateTime
			Return [with](dateTime.plusHours(hours), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of minutes added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the minutes added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMinutes(  minutes As Long) As OffsetDateTime
			Return [with](dateTime.plusMinutes(minutes), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of seconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the seconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusSeconds(  seconds As Long) As OffsetDateTime
			Return [with](dateTime.plusSeconds(seconds), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of nanoseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to add, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the nanoseconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the unit cannot be added to this type </exception>
		Public Function plusNanos(  nanos As Long) As OffsetDateTime
			Return [with](dateTime.plusNanos(nanos), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the specified amount subtracted.
		''' The amount is typically <seealso cref="Period"/> or <seealso cref="Duration"/> but may be
		''' any other type implementing the <seealso cref="TemporalAmount"/> interface.
		''' <p>
		''' The calculation is delegated to the amount object by calling
		''' <seealso cref="TemporalAmount#subtractFrom(Temporal)"/>. The amount implementation is free
		''' to implement the subtraction in any way it wishes, however it typically
		''' calls back to <seealso cref="#minus(long, TemporalUnit)"/>. Consult the documentation
		''' of the amount implementation to determine if it can be successfully subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToSubtract">  the amount to subtract, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(  amountToSubtract As java.time.temporal.TemporalAmount) As OffsetDateTime
			Return CType(amountToSubtract.subtractFrom(Me), OffsetDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns an {@code OffsetDateTime}, based on this one, with the amount
		''' in terms of the unit subtracted. If it is not possible to subtract the amount,
		''' because the unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' This method is equivalent to <seealso cref="#plus(long, TemporalUnit)"/> with the amount negated.
		''' See that method for a full description of how addition, and thus subtraction, works.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToSubtract">  the amount of the unit to subtract from the result, may be negative </param>
		''' <param name="unit">  the unit of the amount to subtract, not null </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(  amountToSubtract As Long,   unit As java.time.temporal.TemporalUnit) As OffsetDateTime
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of years subtracted.
		''' <p>
		''' This method subtracts the specified amount from the years field in three steps:
		''' <ol>
		''' <li>Subtract the input years from the year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2008-02-29 (leap year) minus one year would result in the
		''' invalid date 2009-02-29 (standard year). Instead of returning an invalid
		''' result, the last valid day of the month, 2009-02-28, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="years">  the years to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusYears(  years As Long) As OffsetDateTime
			Return (If(years = java.lang.[Long].MIN_VALUE, plusYears(Long.Max_Value).plusYears(1), plusYears(-years)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of months subtracted.
		''' <p>
		''' This method subtracts the specified amount from the months field in three steps:
		''' <ol>
		''' <li>Subtract the input months from the month-of-year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2007-03-31 minus one month would result in the invalid date
		''' 2007-04-31. Instead of returning an invalid result, the last valid day
		''' of the month, 2007-04-30, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="months">  the months to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMonths(  months As Long) As OffsetDateTime
			Return (If(months = java.lang.[Long].MIN_VALUE, plusMonths(Long.Max_Value).plusMonths(1), plusMonths(-months)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of weeks subtracted.
		''' <p>
		''' This method subtracts the specified amount in weeks from the days field decrementing
		''' the month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 minus one week would result in 2009-01-07.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeks">  the weeks to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the weeks subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusWeeks(  weeks As Long) As OffsetDateTime
			Return (If(weeks = java.lang.[Long].MIN_VALUE, plusWeeks(Long.Max_Value).plusWeeks(1), plusWeeks(-weeks)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of days subtracted.
		''' <p>
		''' This method subtracts the specified amount from the days field decrementing the
		''' month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 minus one day would result in 2009-01-01.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the days subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusDays(  days As Long) As OffsetDateTime
			Return (If(days = java.lang.[Long].MIN_VALUE, plusDays(Long.Max_Value).plusDays(1), plusDays(-days)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of hours subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the hours subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusHours(  hours As Long) As OffsetDateTime
			Return (If(hours = java.lang.[Long].MIN_VALUE, plusHours(Long.Max_Value).plusHours(1), plusHours(-hours)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of minutes subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the minutes subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMinutes(  minutes As Long) As OffsetDateTime
			Return (If(minutes = java.lang.[Long].MIN_VALUE, plusMinutes(Long.Max_Value).plusMinutes(1), plusMinutes(-minutes)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of seconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the seconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusSeconds(  seconds As Long) As OffsetDateTime
			Return (If(seconds = java.lang.[Long].MIN_VALUE, plusSeconds(Long.Max_Value).plusSeconds(1), plusSeconds(-seconds)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetDateTime} with the specified number of nanoseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to subtract, may be negative </param>
		''' <returns> an {@code OffsetDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusNanos(  nanos As Long) As OffsetDateTime
			Return (If(nanos = java.lang.[Long].MIN_VALUE, plusNanos(Long.Max_Value).plusNanos(1), plusNanos(-nanos)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this date-time using the specified query.
		''' <p>
		''' This queries this date-time using the specified query strategy object.
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
			If query_Renamed Is java.time.temporal.TemporalQueries.offset() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zone() Then
				Return CType(offset, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.zoneId() Then
				Return Nothing
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then
				Return CType(toLocalDate(), R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localTime() Then
				Return CType(toLocalTime(), R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then
				Return CType(java.time.chrono.IsoChronology.INSTANCE, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.precision() Then
				Return CType(NANOS, R)
			End If
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
			Return query_Renamed.queryFrom(Me)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same offset, date
		''' and time as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the offset, date and time changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' three times, passing <seealso cref="ChronoField#EPOCH_DAY"/>,
		''' <seealso cref="ChronoField#NANO_OF_DAY"/> and <seealso cref="ChronoField#OFFSET_SECONDS"/> as the fields.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisOffsetDateTime.adjustInto(temporal);
		'''   temporal = temporal.with(thisOffsetDateTime);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(  temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			' OffsetDateTime is treated as three separate fields, not an instant
			' this produces the most consistent set of results overall
			' the offset is set after the date and time, as it is typically a small
			' tweak to the result, with ZonedDateTime frequently ignoring the offset
			Return temporal.with(EPOCH_DAY, toLocalDate().toEpochDay()).with(NANO_OF_DAY, toLocalTime().toNanoOfDay()).with(OFFSET_SECONDS, offset.totalSeconds)
		End Function

		''' <summary>
		''' Calculates the amount of time until another date-time in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code OffsetDateTime}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified date-time.
		''' The result will be negative if the end is before the start.
		''' For example, the amount in days between two date-times can be calculated
		''' using {@code startDateTime.until(endDateTime, DAYS)}.
		''' <p>
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code OffsetDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		''' If the offset differs between the two date-times, the specified
		''' end date-time is normalized to have the same offset as this date-time.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two date-times.
		''' For example, the amount in months between 2012-06-15T00:00Z and 2012-08-14T23:59Z
		''' will only be one month as it is one minute short of two months.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method.
		''' The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		''' <pre>
		'''   // these two lines are equivalent
		'''   amount = start.until(end, MONTHS);
		'''   amount = MONTHS.between(start, end);
		''' </pre>
		''' The choice should be made based on which makes the code more readable.
		''' <p>
		''' The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		''' The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		''' {@code MINUTES}, {@code HOURS} and {@code HALF_DAYS}, {@code DAYS},
		''' {@code WEEKS}, {@code MONTHS}, {@code YEARS}, {@code DECADES},
		''' {@code CENTURIES}, {@code MILLENNIA} and {@code ERAS} are supported.
		''' Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to an {@code OffsetDateTime}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this date-time and the end date-time </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to an {@code OffsetDateTime} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](  endExclusive As java.time.temporal.Temporal,   unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As OffsetDateTime = OffsetDateTime.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				[end] = [end].withOffsetSameInstant(offset)
				Return dateTime.until([end].dateTime, unit)
			End If
			Return unit.between(Me, [end])
		End Function

		''' <summary>
		''' Formats this date-time using the specified formatter.
		''' <p>
		''' This date-time will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted date-time string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Function format(  formatter As java.time.format.DateTimeFormatter) As String
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this date-time with a time-zone to create a {@code ZonedDateTime}
		''' ensuring that the result has the same instant.
		''' <p>
		''' This returns a {@code ZonedDateTime} formed from this date-time and the specified time-zone.
		''' This conversion will ignore the visible local date-time and use the underlying instant instead.
		''' This avoids any problems with local time-line gaps or overlaps.
		''' The result might have different values for fields such as hour, minute an even day.
		''' <p>
		''' To attempt to retain the values of the fields, use <seealso cref="#atZoneSimilarLocal(ZoneId)"/>.
		''' To use the offset as the zone ID, use <seealso cref="#toZonedDateTime()"/>.
		''' </summary>
		''' <param name="zone">  the time-zone to use, not null </param>
		''' <returns> the zoned date-time formed from this date-time, not null </returns>
		Public Function atZoneSameInstant(  zone As ZoneId) As ZonedDateTime
			Return ZonedDateTime.ofInstant(dateTime, offset, zone)
		End Function

		''' <summary>
		''' Combines this date-time with a time-zone to create a {@code ZonedDateTime}
		''' trying to keep the same local date and time.
		''' <p>
		''' This returns a {@code ZonedDateTime} formed from this date-time and the specified time-zone.
		''' Where possible, the result will have the same local date-time as this object.
		''' <p>
		''' Time-zone rules, such as daylight savings, mean that not every time on the
		''' local time-line exists. If the local date-time is in a gap or overlap according to
		''' the rules then a resolver is used to determine the resultant local time and offset.
		''' This method uses <seealso cref="ZonedDateTime#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		''' to retain the offset from this instance if possible.
		''' <p>
		''' Finer control over gaps and overlaps is available in two ways.
		''' If you simply want to use the later offset at overlaps then call
		''' <seealso cref="ZonedDateTime#withLaterOffsetAtOverlap()"/> immediately after this method.
		''' <p>
		''' To create a zoned date-time at the same instant irrespective of the local time-line,
		''' use <seealso cref="#atZoneSameInstant(ZoneId)"/>.
		''' To use the offset as the zone ID, use <seealso cref="#toZonedDateTime()"/>.
		''' </summary>
		''' <param name="zone">  the time-zone to use, not null </param>
		''' <returns> the zoned date-time formed from this date and the earliest valid time for the zone, not null </returns>
		Public Function atZoneSimilarLocal(  zone As ZoneId) As ZonedDateTime
			Return ZonedDateTime.ofLocal(dateTime, zone, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this date-time to an {@code OffsetTime}.
		''' <p>
		''' This returns an offset time with the same local time and offset.
		''' </summary>
		''' <returns> an OffsetTime representing the time and offset, not null </returns>
		Public Function toOffsetTime() As OffsetTime
			Return OffsetTime.of(dateTime.toLocalTime(), offset)
		End Function

		''' <summary>
		''' Converts this date-time to a {@code ZonedDateTime} using the offset as the zone ID.
		''' <p>
		''' This creates the simplest possible {@code ZonedDateTime} using the offset
		''' as the zone ID.
		''' <p>
		''' To control the time-zone used, see <seealso cref="#atZoneSameInstant(ZoneId)"/> and
		''' <seealso cref="#atZoneSimilarLocal(ZoneId)"/>.
		''' </summary>
		''' <returns> a zoned date-time representing the same local date-time and offset, not null </returns>
		Public Function toZonedDateTime() As ZonedDateTime
			Return ZonedDateTime.of(dateTime, offset)
		End Function

		''' <summary>
		''' Converts this date-time to an {@code Instant}.
		''' <p>
		''' This returns an {@code Instant} representing the same point on the
		''' time-line as this date-time.
		''' </summary>
		''' <returns> an {@code Instant} representing the same instant, not null </returns>
		Public Function toInstant() As Instant
			Return dateTime.toInstant(offset)
		End Function

		''' <summary>
		''' Converts this date-time to the number of seconds from the epoch of 1970-01-01T00:00:00Z.
		''' <p>
		''' This allows this date-time to be converted to a value of the
		''' <seealso cref="ChronoField#INSTANT_SECONDS epoch-seconds"/> field. This is primarily
		''' intended for low-level conversions rather than general application usage.
		''' </summary>
		''' <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		Public Function toEpochSecond() As Long
			Return dateTime.toEpochSecond(offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this date-time to another date-time.
		''' <p>
		''' The comparison is based on the instant then on the local date-time.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' For example, the following is the comparator order:
		''' <ol>
		''' <li>{@code 2008-12-03T10:30+01:00}</li>
		''' <li>{@code 2008-12-03T11:00+01:00}</li>
		''' <li>{@code 2008-12-03T12:00+02:00}</li>
		''' <li>{@code 2008-12-03T11:30+01:00}</li>
		''' <li>{@code 2008-12-03T12:00+01:00}</li>
		''' <li>{@code 2008-12-03T12:30+01:00}</li>
		''' </ol>
		''' Values #2 and #3 represent the same instant on the time-line.
		''' When two values represent the same instant, the local date-time is compared
		''' to distinguish them. This step is needed to make the ordering
		''' consistent with {@code equals()}.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(  other As OffsetDateTime) As Integer Implements Comparable(Of OffsetDateTime).compareTo
			Dim cmp As Integer = compareInstant(Me, other)
			If cmp = 0 Then cmp = toLocalDateTime().CompareTo(other.toLocalDateTime())
			Return cmp
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the instant of this date-time is after that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/> in that it
		''' only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().isAfter(dateTime2.toInstant());}.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this is after the instant of the specified date-time </returns>
		Public Function isAfter(  other As OffsetDateTime) As Boolean
			Dim thisEpochSec As Long = toEpochSecond()
			Dim otherEpochSec As Long = other.toEpochSecond()
			Return thisEpochSec > otherEpochSec OrElse (thisEpochSec = otherEpochSec AndAlso toLocalTime().nano > other.toLocalTime().nano)
		End Function

		''' <summary>
		''' Checks if the instant of this date-time is before that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().isBefore(dateTime2.toInstant());}.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this is before the instant of the specified date-time </returns>
		Public Function isBefore(  other As OffsetDateTime) As Boolean
			Dim thisEpochSec As Long = toEpochSecond()
			Dim otherEpochSec As Long = other.toEpochSecond()
			Return thisEpochSec < otherEpochSec OrElse (thisEpochSec = otherEpochSec AndAlso toLocalTime().nano < other.toLocalTime().nano)
		End Function

		''' <summary>
		''' Checks if the instant of this date-time is equal to that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		''' in that it only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().equals(dateTime2.toInstant());}.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if the instant equals the instant of the specified date-time </returns>
		Public Function isEqual(  other As OffsetDateTime) As Boolean
			Return toEpochSecond() = other.toEpochSecond() AndAlso toLocalTime().nano = other.toLocalTime().nano
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this date-time is equal to another date-time.
		''' <p>
		''' The comparison is based on the local date-time and the offset.
		''' To compare for the same instant on the time-line, use <seealso cref="#isEqual"/>.
		''' Only objects of type {@code OffsetDateTime} are compared, other types return false.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date-time </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is OffsetDateTime Then
				Dim other As OffsetDateTime = CType(obj, OffsetDateTime)
				Return dateTime.Equals(other.dateTime) AndAlso offset.Equals(other.offset)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this date-time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return dateTime.GetHashCode() Xor offset.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date-time as a {@code String}, such as {@code 2007-12-03T10:15:30+01:00}.
		''' <p>
		''' The output will be one of the following ISO-8601 formats:
		''' <ul>
		''' <li>{@code uuuu-MM-dd'T'HH:mmXXXXX}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ssXXXXX}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSXXXXX}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSXXXXX}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSSSSXXXXX}</li>
		''' </ul>
		''' The format used will be the shortest that outputs the full value of
		''' the time where the omitted parts are implied to be zero.
		''' </summary>
		''' <returns> a string representation of this date-time, not null </returns>
		Public Overrides Function ToString() As String
			Return dateTime.ToString() & offset.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(10);  // identifies an OffsetDateTime
		'''  // the <a href="../../serialized-form.html#java.time.LocalDateTime">datetime</a> excluding the one byte header
		'''  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.OFFSET_DATE_TIME_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(  out As java.io.ObjectOutput)
			dateTime.writeExternal(out)
			offset.writeExternal(out)
		End Sub

		Shared Function readExternal(  [in] As java.io.ObjectInput) As OffsetDateTime
			Dim dateTime As LocalDateTime = LocalDateTime.readExternal([in])
			Dim offset_Renamed As ZoneOffset = ZoneOffset.readExternal([in])
			Return OffsetDateTime.of(dateTime, offset_Renamed)
		End Function

	End Class

End Namespace