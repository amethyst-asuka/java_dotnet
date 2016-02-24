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
	''' A date-time without a time-zone in the ISO-8601 calendar system,
	''' such as {@code 2007-12-03T10:15:30}.
	''' <p>
	''' {@code LocalDateTime} is an immutable date-time object that represents a date-time,
	''' often viewed as year-month-day-hour-minute-second. Other date and time fields,
	''' such as day-of-year, day-of-week and week-of-year, can also be accessed.
	''' Time is represented to nanosecond precision.
	''' For example, the value "2nd October 2007 at 13:45.30.123456789" can be
	''' stored in a {@code LocalDateTime}.
	''' <p>
	''' This class does not store or represent a time-zone.
	''' Instead, it is a description of the date, as used for birthdays, combined with
	''' the local time as seen on a wall clock.
	''' It cannot represent an instant on the time-line without additional information
	''' such as an offset or time-zone.
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
	''' {@code LocalDateTime} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class LocalDateTime
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, java.time.chrono.ChronoLocalDateTime(Of LocalDate)

		''' <summary>
		''' The minimum supported {@code LocalDateTime}, '-999999999-01-01T00:00:00'.
		''' This is the local date-time of midnight at the start of the minimum date.
		''' This combines <seealso cref="LocalDate#MIN"/> and <seealso cref="LocalTime#MIN"/>.
		''' This could be used by an application as a "far past" date-time.
		''' </summary>
		Public Shared ReadOnly MIN As LocalDateTime = LocalDateTime.of(LocalDate.MIN, LocalTime.MIN)
		''' <summary>
		''' The maximum supported {@code LocalDateTime}, '+999999999-12-31T23:59:59.999999999'.
		''' This is the local date-time just before midnight at the end of the maximum date.
		''' This combines <seealso cref="LocalDate#MAX"/> and <seealso cref="LocalTime#MAX"/>.
		''' This could be used by an application as a "far future" date-time.
		''' </summary>
		Public Shared ReadOnly MAX As LocalDateTime = LocalDateTime.of(LocalDate.MAX, LocalTime.MAX)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 6207766400415563566L

		''' <summary>
		''' The date part.
		''' </summary>
		Private ReadOnly [date] As LocalDate
		''' <summary>
		''' The time part.
		''' </summary>
		Private ReadOnly time As LocalTime

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current date-time from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date-time.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date-time using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As LocalDateTime
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current date-time from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date-time.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date-time using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As LocalDateTime
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current date-time from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date-time.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date-time, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As LocalDateTime
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Dim offset As ZoneOffset = clock_Renamed.zone.rules.getOffset(now_Renamed)
			Return ofEpochSecond(now_Renamed.epochSecond, now_Renamed.nano, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour and minute, setting the second and nanosecond to zero.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour and minute.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' The second and nanosecond fields will be set to zero.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Month, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour, minute and second, setting the nanosecond to zero.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour, minute and second.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' The nanosecond field will be set to zero.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Month, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute, second)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour, minute, second and nanosecond.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour, minute, second and nanosecond.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Month, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute, second, nanoOfSecond)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour and minute, setting the second and nanosecond to zero.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour and minute.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' The second and nanosecond fields will be set to zero.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour, minute and second, setting the nanosecond to zero.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour, minute and second.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' The nanosecond field will be set to zero.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute, second)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from year, month,
		''' day, hour, minute, second and nanosecond.
		''' <p>
		''' This returns a {@code LocalDateTime} with the specified year, month,
		''' day-of-month, hour, minute, second and nanosecond.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer, ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.of(year_Renamed, month, dayOfMonth)
			Dim time As LocalTime = LocalTime.of(hour, minute, second, nanoOfSecond)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from a date and time.
		''' </summary>
		''' <param name="date">  the local date, not null </param>
		''' <param name="time">  the local time, not null </param>
		''' <returns> the local date-time, not null </returns>
		Public Shared Function [of](ByVal [date] As LocalDate, ByVal time As LocalTime) As LocalDateTime
			java.util.Objects.requireNonNull(date_Renamed, "date")
			java.util.Objects.requireNonNull(time, "time")
			Return New LocalDateTime(date_Renamed, time)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from an {@code Instant} and zone ID.
		''' <p>
		''' This creates a local date-time based on the specified instant.
		''' First, the offset from UTC/Greenwich is obtained using the zone ID and instant,
		''' which is simple as there is only one valid offset for each instant.
		''' Then, the instant and offset are used to calculate the local date-time.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone, which may be an offset, not null </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Shared Function ofInstant(ByVal instant_Renamed As Instant, ByVal zone As ZoneId) As LocalDateTime
			java.util.Objects.requireNonNull(instant_Renamed, "instant")
			java.util.Objects.requireNonNull(zone, "zone")
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim offset As ZoneOffset = rules.getOffset(instant_Renamed)
			Return ofEpochSecond(instant_Renamed.epochSecond, instant_Renamed.nano, offset)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} using seconds from the
		''' epoch of 1970-01-01T00:00:00Z.
		''' <p>
		''' This allows the <seealso cref="ChronoField#INSTANT_SECONDS epoch-second"/> field
		''' to be converted to a local date-time. This is primarily intended for
		''' low-level conversions rather than general application usage.
		''' </summary>
		''' <param name="epochSecond">  the number of seconds from the epoch of 1970-01-01T00:00:00Z </param>
		''' <param name="nanoOfSecond">  the nanosecond within the second, from 0 to 999,999,999 </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range,
		'''  or if the nano-of-second is invalid </exception>
		Public Shared Function ofEpochSecond(ByVal epochSecond As Long, ByVal nanoOfSecond As Integer, ByVal offset As ZoneOffset) As LocalDateTime
			java.util.Objects.requireNonNull(offset, "offset")
			NANO_OF_SECOND.checkValidValue(nanoOfSecond)
			Dim localSecond As Long = epochSecond + offset.totalSeconds ' overflow caught later
			Dim localEpochDay As Long = Math.floorDiv(localSecond, SECONDS_PER_DAY)
			Dim secsOfDay As Integer = CInt(Math.floorMod(localSecond, SECONDS_PER_DAY))
			Dim date_Renamed As LocalDate = LocalDate.ofEpochDay(localEpochDay)
			Dim time As LocalTime = LocalTime.ofNanoOfDay(secsOfDay * NANOS_PER_SECOND + nanoOfSecond)
			Return New LocalDateTime(date_Renamed, time)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from a temporal object.
		''' <p>
		''' This obtains a local date-time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code LocalDateTime}.
		''' <p>
		''' The conversion extracts and combines the {@code LocalDate} and the
		''' {@code LocalTime} from the temporal object.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code LocalDateTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code LocalDateTime} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As LocalDateTime
			If TypeOf temporal Is LocalDateTime Then
				Return CType(temporal, LocalDateTime)
			ElseIf TypeOf temporal Is ZonedDateTime Then
				Return CType(temporal, ZonedDateTime).toLocalDateTime()
			ElseIf TypeOf temporal Is OffsetDateTime Then
				Return CType(temporal, OffsetDateTime).toLocalDateTime()
			End If
			Try
				Dim date_Renamed As LocalDate = LocalDate.from(temporal)
				Dim time As LocalTime = LocalTime.from(temporal)
				Return New LocalDateTime(date_Renamed, time)
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain LocalDateTime from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from a text string such as {@code 2007-12-03T10:15:30}.
		''' <p>
		''' The string must represent a valid date-time and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_DATE_TIME"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007-12-03T10:15:30", not null </param>
		''' <returns> the parsed local date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As LocalDateTime
			Return parse(text, java.time.format.DateTimeFormatter.ISO_LOCAL_DATE_TIME)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDateTime} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a date-time.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed local date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As LocalDateTime
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, LocalDateTime::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="date">  the date part of the date-time, validated not null </param>
		''' <param name="time">  the time part of the date-time, validated not null </param>
		Private Sub New(ByVal [date] As LocalDate, ByVal time As LocalTime)
			Me.date = date_Renamed
			Me.time = time
		End Sub

		''' <summary>
		''' Returns a copy of this date-time with the new date and time, checking
		''' to see if a new object is in fact required.
		''' </summary>
		''' <param name="newDate">  the date of the new date-time, not null </param>
		''' <param name="newTime">  the time of the new date-time, not null </param>
		''' <returns> the date-time, not null </returns>
		Private Function [with](ByVal newDate As LocalDate, ByVal newTime As LocalTime) As LocalDateTime
			If [date] Is newDate AndAlso time Is newTime Then Return Me
			Return New LocalDateTime(newDate, newTime)
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
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return f.dateBased OrElse f.timeBased
			End If
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
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
		Public Overrides Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean ' override for Javadoc
			Return outerInstance.isSupported(unit)
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
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.range(field), [date].range(field)))
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
		''' {@code EPOCH_DAY} and {@code PROLEPTIC_MONTH} which are too large to fit in
		''' an {@code int} and throw a {@code DateTimeException}.
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
		Public Overrides Function [get](ByVal field As java.time.temporal.TemporalField) As Integer
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.get(field), [date].get(field)))
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
		Public Overrides Function getLong(ByVal field As java.time.temporal.TemporalField) As Long
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.getLong(field), [date].getLong(field)))
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalDate} part of this date-time.
		''' <p>
		''' This returns a {@code LocalDate} with the same year, month and day
		''' as this date-time.
		''' </summary>
		''' <returns> the date part of this date-time, not null </returns>
		Public Overrides Function toLocalDate() As LocalDate
			Return [date]
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
				Return [date].year
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
				Return [date].monthValue
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
				Return [date].month
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
				Return [date].dayOfMonth
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
				Return [date].dayOfYear
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
				Return [date].dayOfWeek
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
		Public Overrides Function toLocalTime() As LocalTime
			Return time
		End Function

		''' <summary>
		''' Gets the hour-of-day field.
		''' </summary>
		''' <returns> the hour-of-day, from 0 to 23 </returns>
		Public Property hour As Integer
			Get
				Return time.hour
			End Get
		End Property

		''' <summary>
		''' Gets the minute-of-hour field.
		''' </summary>
		''' <returns> the minute-of-hour, from 0 to 59 </returns>
		Public Property minute As Integer
			Get
				Return time.minute
			End Get
		End Property

		''' <summary>
		''' Gets the second-of-minute field.
		''' </summary>
		''' <returns> the second-of-minute, from 0 to 59 </returns>
		Public Property second As Integer
			Get
				Return time.second
			End Get
		End Property

		''' <summary>
		''' Gets the nano-of-second field.
		''' </summary>
		''' <returns> the nano-of-second, from 0 to 999,999,999 </returns>
		Public Property nano As Integer
			Get
				Return time.nano
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this date-time.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the date-time adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the year field.
		''' A more complex adjuster might set the date to the last day of the month.
		''' <p>
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
		'''  result = localDateTime.with(JULY).with(lastDayOfMonth());
		''' </pre>
		''' <p>
		''' The classes <seealso cref="LocalDate"/> and <seealso cref="LocalTime"/> implement {@code TemporalAdjuster},
		''' thus this method can be used to change the date, time or offset:
		''' <pre>
		'''  result = localDateTime.with(date);
		'''  result = localDateTime.with(time);
		''' </pre>
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> a {@code LocalDateTime} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As LocalDateTime
			' optimizations
			If TypeOf adjuster Is LocalDate Then
				Return [with](CType(adjuster, LocalDate), time)
			ElseIf TypeOf adjuster Is LocalTime Then
				Return [with]([date], CType(adjuster, LocalTime))
			ElseIf TypeOf adjuster Is LocalDateTime Then
				Return CType(adjuster, LocalDateTime)
			End If
			Return CType(adjuster.adjustInto(Me), LocalDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified field set to a new value.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the value
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
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		''' the matching method on <seealso cref="LocalDate#with(TemporalField, long) LocalDate"/>
		''' or <seealso cref="LocalTime#with(TemporalField, long) LocalTime"/>.
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
		''' <returns> a {@code LocalDateTime} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As LocalDateTime
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				If f.timeBased Then
					Return [with]([date], time.with(field, newValue))
				Else
					Return [with]([date].with(field, newValue), time)
				End If
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the year altered.
		''' <p>
		''' The time does not affect the calculation and will be the same in the result.
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested year, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Function withYear(ByVal year_Renamed As Integer) As LocalDateTime
			Return [with]([date].withYear(year_Renamed), time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the month-of-year altered.
		''' <p>
		''' The time does not affect the calculation and will be the same in the result.
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(ByVal month As Integer) As LocalDateTime
			Return [with]([date].withMonth(month), time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the day-of-month altered.
		''' <p>
		''' If the resulting date-time is invalid, an exception is thrown.
		''' The time does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month value is invalid,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Function withDayOfMonth(ByVal dayOfMonth As Integer) As LocalDateTime
			Return [with]([date].withDayOfMonth(dayOfMonth), time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the day-of-year altered.
		''' <p>
		''' If the resulting date-time is invalid, an exception is thrown.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		''' <returns> a {@code LocalDateTime} based on this date with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-year value is invalid,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Function withDayOfYear(ByVal dayOfYear As Integer) As LocalDateTime
			Return [with]([date].withDayOfYear(dayOfYear), time)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the hour-of-day altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested hour, not null </returns>
		''' <exception cref="DateTimeException"> if the hour value is invalid </exception>
		Public Function withHour(ByVal hour As Integer) As LocalDateTime
			Dim newTime As LocalTime = time.withHour(hour)
			Return [with]([date], newTime)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the minute-of-hour altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested minute, not null </returns>
		''' <exception cref="DateTimeException"> if the minute value is invalid </exception>
		Public Function withMinute(ByVal minute As Integer) As LocalDateTime
			Dim newTime As LocalTime = time.withMinute(minute)
			Return [with]([date], newTime)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the second-of-minute altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested second, not null </returns>
		''' <exception cref="DateTimeException"> if the second value is invalid </exception>
		Public Function withSecond(ByVal second As Integer) As LocalDateTime
			Dim newTime As LocalTime = time.withSecond(second)
			Return [with]([date], newTime)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the nano-of-second altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the requested nanosecond, not null </returns>
		''' <exception cref="DateTimeException"> if the nano value is invalid </exception>
		Public Function withNano(ByVal nanoOfSecond As Integer) As LocalDateTime
			Dim newTime As LocalTime = time.withNano(nanoOfSecond)
			Return [with]([date], newTime)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the time truncated.
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
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="unit">  the unit to truncate to, not null </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if unable to truncate </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(ByVal unit As java.time.temporal.TemporalUnit) As LocalDateTime
			Return [with]([date], time.truncatedTo(unit))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the specified amount added.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As LocalDateTime
			If TypeOf amountToAdd Is Period Then
				Dim periodToAdd As Period = CType(amountToAdd, Period)
				Return [with]([date].plus(periodToAdd), time)
			End If
			java.util.Objects.requireNonNull(amountToAdd, "amountToAdd")
			Return CType(amountToAdd.addTo(Me), LocalDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' Date units are added as per <seealso cref="LocalDate#plus(long, TemporalUnit)"/>.
		''' Time units are added as per <seealso cref="LocalTime#plus(long, TemporalUnit)"/> with
		''' any overflow in days added equivalent to using <seealso cref="#plusDays(long)"/>.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalDateTime
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim f As java.time.temporal.ChronoUnit = CType(unit, java.time.temporal.ChronoUnit)
				Select Case f
					Case NANOS
						Return plusNanos(amountToAdd)
					Case MICROS
						Return plusDays(amountToAdd / MICROS_PER_DAY).plusNanos((amountToAdd Mod MICROS_PER_DAY) * 1000)
					Case MILLIS
						Return plusDays(amountToAdd / MILLIS_PER_DAY).plusNanos((amountToAdd Mod MILLIS_PER_DAY) * 1000000)
					Case SECONDS
						Return plusSeconds(amountToAdd)
					Case MINUTES
						Return plusMinutes(amountToAdd)
					Case HOURS
						Return plusHours(amountToAdd)
					Case HALF_DAYS ' no overflow (256 is multiple of 2)
						Return plusDays(amountToAdd \ 256).plusHours((amountToAdd Mod 256) * 12)
				End Select
				Return [with]([date].plus(amountToAdd, unit), time)
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of years added.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusYears(ByVal years As Long) As LocalDateTime
			Dim newDate As LocalDate = [date].plusYears(years)
			Return [with](newDate, time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of months added.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMonths(ByVal months As Long) As LocalDateTime
			Dim newDate As LocalDate = [date].plusMonths(months)
			Return [with](newDate, time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of weeks added.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the weeks added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusWeeks(ByVal weeks As Long) As LocalDateTime
			Dim newDate As LocalDate = [date].plusWeeks(weeks)
			Return [with](newDate, time)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of days added.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the days added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusDays(ByVal days As Long) As LocalDateTime
			Dim newDate As LocalDate = [date].plusDays(days)
			Return [with](newDate, time)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of hours added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to add, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the hours added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusHours(ByVal hours As Long) As LocalDateTime
			Return plusWithOverflow([date], hours, 0, 0, 0, 1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of minutes added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to add, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the minutes added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMinutes(ByVal minutes As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, minutes, 0, 0, 1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of seconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to add, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the seconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusSeconds(ByVal seconds As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, 0, seconds, 0, 1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of nanoseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to add, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the nanoseconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusNanos(ByVal nanos As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, 0, 0, nanos, 1)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the specified amount subtracted.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As LocalDateTime
			If TypeOf amountToSubtract Is Period Then
				Dim periodToSubtract As Period = CType(amountToSubtract, Period)
				Return [with]([date].minus(periodToSubtract), time)
			End If
			java.util.Objects.requireNonNull(amountToSubtract, "amountToSubtract")
			Return CType(amountToSubtract.subtractFrom(Me), LocalDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalDateTime}, based on this one, with the amount
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalDateTime
			Return (If(amountToSubtract = Long.MinValue, plus(Long.MaxValue, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of years subtracted.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusYears(ByVal years As Long) As LocalDateTime
			Return (If(years = Long.MinValue, plusYears(Long.MaxValue).plusYears(1), plusYears(-years)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of months subtracted.
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
		''' <returns> a {@code LocalDateTime} based on this date-time with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMonths(ByVal months As Long) As LocalDateTime
			Return (If(months = Long.MinValue, plusMonths(Long.MaxValue).plusMonths(1), plusMonths(-months)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of weeks subtracted.
		''' <p>
		''' This method subtracts the specified amount in weeks from the days field decrementing
		''' the month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2009-01-07 minus one week would result in 2008-12-31.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeks">  the weeks to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the weeks subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusWeeks(ByVal weeks As Long) As LocalDateTime
			Return (If(weeks = Long.MinValue, plusWeeks(Long.MaxValue).plusWeeks(1), plusWeeks(-weeks)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of days subtracted.
		''' <p>
		''' This method subtracts the specified amount from the days field decrementing the
		''' month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2009-01-01 minus one day would result in 2008-12-31.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the days subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusDays(ByVal days As Long) As LocalDateTime
			Return (If(days = Long.MinValue, plusDays(Long.MaxValue).plusDays(1), plusDays(-days)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of hours subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the hours subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusHours(ByVal hours As Long) As LocalDateTime
			Return plusWithOverflow([date], hours, 0, 0, 0, -1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of minutes subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the minutes subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMinutes(ByVal minutes As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, minutes, 0, 0, -1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of seconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the seconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusSeconds(ByVal seconds As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, 0, seconds, 0, -1)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified number of nanoseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to subtract, may be negative </param>
		''' <returns> a {@code LocalDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusNanos(ByVal nanos As Long) As LocalDateTime
			Return plusWithOverflow([date], 0, 0, 0, nanos, -1)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDateTime} with the specified period added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="newDate">  the new date to base the calculation on, not null </param>
		''' <param name="hours">  the hours to add, may be negative </param>
		''' <param name="minutes"> the minutes to add, may be negative </param>
		''' <param name="seconds"> the seconds to add, may be negative </param>
		''' <param name="nanos"> the nanos to add, may be negative </param>
		''' <param name="sign">  the sign to determine add or subtract </param>
		''' <returns> the combined result, not null </returns>
		Private Function plusWithOverflow(ByVal newDate As LocalDate, ByVal hours As Long, ByVal minutes As Long, ByVal seconds As Long, ByVal nanos As Long, ByVal sign As Integer) As LocalDateTime
			' 9223372036854775808 long, 2147483648 int
			If (hours Or minutes Or seconds Or nanos) = 0 Then Return [with](newDate, time)
			Dim totDays As Long = nanos / NANOS_PER_DAY + seconds / SECONDS_PER_DAY + minutes / MINUTES_PER_DAY + hours / HOURS_PER_DAY '   max/24 -    max/24*60 -    max/24*60*60 -    max/24*60*60*1B
			totDays *= sign ' total max*0.4237...
			Dim totNanos As Long = nanos Mod NANOS_PER_DAY + (seconds Mod SECONDS_PER_DAY) * NANOS_PER_SECOND + (minutes Mod MINUTES_PER_DAY) * NANOS_PER_MINUTE + (hours Mod HOURS_PER_DAY) * NANOS_PER_HOUR '   max  86400000000000 -    max  86400000000000 -    max  86400000000000 -    max  86400000000000
			Dim curNoD As Long = time.toNanoOfDay() '   max  86400000000000
			totNanos = totNanos * sign + curNoD ' total 432000000000000
			totDays += Math.floorDiv(totNanos, NANOS_PER_DAY)
			Dim newNoD As Long = Math.floorMod(totNanos, NANOS_PER_DAY)
			Dim newTime As LocalTime = (If(newNoD = curNoD, time, LocalTime.ofNanoOfDay(newNoD)))
			Return [with](newDate.plusDays(totDays), newTime)
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
		Public Overrides Function query(Of R)(ByVal query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R ' override for Javadoc
			If query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then Return CType([date], R)
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same date and time as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the date and time changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' twice, passing <seealso cref="ChronoField#EPOCH_DAY"/> and
		''' <seealso cref="ChronoField#NANO_OF_DAY"/> as the fields.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisLocalDateTime.adjustInto(temporal);
		'''   temporal = temporal.with(thisLocalDateTime);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal ' override for Javadoc
			Return outerInstance.adjustInto(temporal)
		End Function

		''' <summary>
		''' Calculates the amount of time until another date-time in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code LocalDateTime}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified date-time.
		''' The result will be negative if the end is before the start.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code LocalDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in days between two date-times can be calculated
		''' using {@code startDateTime.until(endDateTime, DAYS)}.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two date-times.
		''' For example, the amount in months between 2012-06-15T00:00 and 2012-08-14T23:59
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
		''' <param name="endExclusive">  the end date, exclusive, which is converted to a {@code LocalDateTime}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this date-time and the end date-time </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code LocalDateTime} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As LocalDateTime = LocalDateTime.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				If unit.timeBased Then
					Dim amount As Long = [date].daysUntil([end].date)
					If amount = 0 Then Return time.until([end].time, unit)
					Dim timePart As Long = [end].time.toNanoOfDay() - time.toNanoOfDay()
					If amount > 0 Then
						amount -= 1 ' safe
						timePart += NANOS_PER_DAY ' safe
					Else
						amount += 1 ' safe
						timePart -= NANOS_PER_DAY ' safe
					End If
					Select Case CType(unit, java.time.temporal.ChronoUnit)
						Case NANOS
							amount = Math.multiplyExact(amount, NANOS_PER_DAY)
						Case MICROS
							amount = Math.multiplyExact(amount, MICROS_PER_DAY)
							timePart = timePart \ 1000
						Case MILLIS
							amount = Math.multiplyExact(amount, MILLIS_PER_DAY)
							timePart = timePart \ 1000000
						Case SECONDS
							amount = Math.multiplyExact(amount, SECONDS_PER_DAY)
							timePart = timePart / NANOS_PER_SECOND
						Case MINUTES
							amount = Math.multiplyExact(amount, MINUTES_PER_DAY)
							timePart = timePart / NANOS_PER_MINUTE
						Case HOURS
							amount = Math.multiplyExact(amount, HOURS_PER_DAY)
							timePart = timePart / NANOS_PER_HOUR
						Case HALF_DAYS
							amount = Math.multiplyExact(amount, 2)
							timePart = timePart / (NANOS_PER_HOUR * 12)
					End Select
					Return Math.addExact(amount, timePart)
				End If
				Dim endDate As LocalDate = [end].date
				If endDate.isAfter([date]) AndAlso [end].time.isBefore(time) Then
					endDate = endDate.minusDays(1)
				ElseIf endDate.isBefore([date]) AndAlso [end].time.isAfter(time) Then
					endDate = endDate.plusDays(1)
				End If
				Return [date].until(endDate, unit)
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
		Public Overrides Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String ' override for Javadoc and performance
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this date-time with an offset to create an {@code OffsetDateTime}.
		''' <p>
		''' This returns an {@code OffsetDateTime} formed from this date-time at the specified offset.
		''' All possible combinations of date-time and offset are valid.
		''' </summary>
		''' <param name="offset">  the offset to combine with, not null </param>
		''' <returns> the offset date-time formed from this date-time and the specified offset, not null </returns>
		Public Function atOffset(ByVal offset As ZoneOffset) As OffsetDateTime
			Return OffsetDateTime.of(Me, offset)
		End Function

		''' <summary>
		''' Combines this date-time with a time-zone to create a {@code ZonedDateTime}.
		''' <p>
		''' This returns a {@code ZonedDateTime} formed from this date-time at the
		''' specified time-zone. The result will match this date-time as closely as possible.
		''' Time-zone rules, such as daylight savings, mean that not every local date-time
		''' is valid for the specified zone, thus the local date-time may be adjusted.
		''' <p>
		''' The local date-time is resolved to a single instant on the time-line.
		''' This is achieved by finding a valid offset from UTC/Greenwich for the local
		''' date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, where clocks are set back, there are two valid offsets.
		''' This method uses the earlier offset typically corresponding to "summer".
		''' <p>
		''' In the case of a gap, where clocks jump forward, there is no valid offset.
		''' Instead, the local date-time is adjusted to be later by the length of the gap.
		''' For a typical one hour daylight savings change, the local date-time will be
		''' moved one hour later into the offset typically corresponding to "summer".
		''' <p>
		''' To obtain the later offset during an overlap, call
		''' <seealso cref="ZonedDateTime#withLaterOffsetAtOverlap()"/> on the result of this method.
		''' To throw an exception when there is a gap or overlap, use
		''' <seealso cref="ZonedDateTime#ofStrict(LocalDateTime, ZoneOffset, ZoneId)"/>.
		''' </summary>
		''' <param name="zone">  the time-zone to use, not null </param>
		''' <returns> the zoned date-time formed from this date-time, not null </returns>
		Public Overrides Function atZone(ByVal zone As ZoneId) As ZonedDateTime
			Return ZonedDateTime.of(Me, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this date-time to another date-time.
		''' <p>
		''' The comparison is primarily based on the date-time, from earliest to latest.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' If all the date-times being compared are instances of {@code LocalDateTime},
		''' then the comparison will be entirely based on the date-time.
		''' If some dates being compared are in different chronologies, then the
		''' chronology is also considered, see <seealso cref="ChronoLocalDateTime#compareTo"/>.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(Of T1)(ByVal other As java.time.chrono.ChronoLocalDateTime(Of T1)) As Integer ' override for Javadoc and performance
			If TypeOf other Is LocalDateTime Then Return compareTo0(CType(other, LocalDateTime))
			Return outerInstance.CompareTo(other)
		End Function

		Private Function compareTo0(ByVal other As LocalDateTime) As Integer
			Dim cmp As Integer = [date].compareTo0(other.toLocalDate())
			If cmp = 0 Then cmp = time.CompareTo(other.toLocalTime())
			Return cmp
		End Function

		''' <summary>
		''' Checks if this date-time is after the specified date-time.
		''' <p>
		''' This checks to see if this date-time represents a point on the
		''' local time-line after the other date-time.
		''' <pre>
		'''   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		'''   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		'''   a.isAfter(b) == false
		'''   a.isAfter(a) == false
		'''   b.isAfter(a) == true
		''' </pre>
		''' <p>
		''' This method only considers the position of the two date-times on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		''' but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this date-time is after the specified date-time </returns>
		Public Overrides Function isAfter(Of T1)(ByVal other As java.time.chrono.ChronoLocalDateTime(Of T1)) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDateTime Then Return compareTo0(CType(other, LocalDateTime)) > 0
			Return outerInstance.isAfter(other)
		End Function

		''' <summary>
		''' Checks if this date-time is before the specified date-time.
		''' <p>
		''' This checks to see if this date-time represents a point on the
		''' local time-line before the other date-time.
		''' <pre>
		'''   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		'''   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		'''   a.isBefore(b) == true
		'''   a.isBefore(a) == false
		'''   b.isBefore(a) == false
		''' </pre>
		''' <p>
		''' This method only considers the position of the two date-times on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		''' but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this date-time is before the specified date-time </returns>
		Public Overrides Function isBefore(Of T1)(ByVal other As java.time.chrono.ChronoLocalDateTime(Of T1)) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDateTime Then Return compareTo0(CType(other, LocalDateTime)) < 0
			Return outerInstance.isBefore(other)
		End Function

		''' <summary>
		''' Checks if this date-time is equal to the specified date-time.
		''' <p>
		''' This checks to see if this date-time represents the same point on the
		''' local time-line as the other date-time.
		''' <pre>
		'''   LocalDate a = LocalDateTime.of(2012, 6, 30, 12, 00);
		'''   LocalDate b = LocalDateTime.of(2012, 7, 1, 12, 00);
		'''   a.isEqual(b) == false
		'''   a.isEqual(a) == true
		'''   b.isEqual(a) == false
		''' </pre>
		''' <p>
		''' This method only considers the position of the two date-times on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDateTime)"/>,
		''' but is the same approach as <seealso cref="ChronoLocalDateTime#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this date-time is equal to the specified date-time </returns>
		Public Overrides Function isEqual(Of T1)(ByVal other As java.time.chrono.ChronoLocalDateTime(Of T1)) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDateTime Then Return compareTo0(CType(other, LocalDateTime)) = 0
			Return outerInstance.isEqual(other)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this date-time is equal to another date-time.
		''' <p>
		''' Compares this {@code LocalDateTime} with another ensuring that the date-time is the same.
		''' Only objects of type {@code LocalDateTime} are compared, other types return false.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date-time </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is LocalDateTime Then
				Dim other As LocalDateTime = CType(obj, LocalDateTime)
				Return [date].Equals(other.date) AndAlso time.Equals(other.time)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this date-time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return [date].GetHashCode() Xor time.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date-time as a {@code String}, such as {@code 2007-12-03T10:15:30}.
		''' <p>
		''' The output will be one of the following ISO-8601 formats:
		''' <ul>
		''' <li>{@code uuuu-MM-dd'T'HH:mm}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSS}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSS}</li>
		''' <li>{@code uuuu-MM-dd'T'HH:mm:ss.SSSSSSSSS}</li>
		''' </ul>
		''' The format used will be the shortest that outputs the full value of
		''' the time where the omitted parts are implied to be zero.
		''' </summary>
		''' <returns> a string representation of this date-time, not null </returns>
		Public Overrides Function ToString() As String
			Return [date].ToString() & AscW("T"c) + time.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(5);  // identifies a LocalDateTime
		'''  // the <a href="../../serialized-form.html#java.time.LocalDate">date</a> excluding the one byte header
		'''  // the <a href="../../serialized-form.html#java.time.LocalTime">time</a> excluding the one byte header
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.LOCAL_DATE_TIME_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			[date].writeExternal(out)
			time.writeExternal(out)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As LocalDateTime
			Dim date_Renamed As LocalDate = LocalDate.readExternal([in])
			Dim time As LocalTime = LocalTime.readExternal([in])
			Return LocalDateTime.of(date_Renamed, time)
		End Function

	End Class

End Namespace