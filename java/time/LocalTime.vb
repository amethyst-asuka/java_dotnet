Imports Microsoft.VisualBasic
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
	''' A time without a time-zone in the ISO-8601 calendar system,
	''' such as {@code 10:15:30}.
	''' <p>
	''' {@code LocalTime} is an immutable date-time object that represents a time,
	''' often viewed as hour-minute-second.
	''' Time is represented to nanosecond precision.
	''' For example, the value "13:45.30.123456789" can be stored in a {@code LocalTime}.
	''' <p>
	''' This class does not store or represent a date or time-zone.
	''' Instead, it is a description of the local time as seen on a wall clock.
	''' It cannot represent an instant on the time-line without additional information
	''' such as an offset or time-zone.
	''' <p>
	''' The ISO-8601 calendar system is the modern civil calendar system used today
	''' in most of the world. This API assumes that all calendar systems use the same
	''' representation, this [Class], for time-of-day.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code LocalTime} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class LocalTime
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of LocalTime)

		''' <summary>
		''' The minimum supported {@code LocalTime}, '00:00'.
		''' This is the time of midnight at the start of the day.
		''' </summary>
		Public Shared ReadOnly MIN As LocalTime
		''' <summary>
		''' The maximum supported {@code LocalTime}, '23:59:59.999999999'.
		''' This is the time just before midnight at the end of the day.
		''' </summary>
		Public Shared ReadOnly MAX As LocalTime
		''' <summary>
		''' The time of midnight at the start of the day, '00:00'.
		''' </summary>
		Public Shared ReadOnly MIDNIGHT As LocalTime
		''' <summary>
		''' The time of noon in the middle of the day, '12:00'.
		''' </summary>
		Public Shared ReadOnly NOON As LocalTime
		''' <summary>
		''' Constants for the local time of each hour.
		''' </summary>
		Private Shared ReadOnly HOURS As LocalTime() = New LocalTime(23){}
		Shared Sub New()
			For i As Integer = 0 To HOURS.Length - 1
				HOURS(i) = New LocalTime(i, 0, 0, 0)
			Next i
			MIDNIGHT = HOURS(0)
			NOON = HOURS(12)
			MIN = HOURS(0)
			MAX = New LocalTime(23, 59, 59, 999999999)
		End Sub

		''' <summary>
		''' Hours per day.
		''' </summary>
		Friend Const HOURS_PER_DAY As Integer = 24
		''' <summary>
		''' Minutes per hour.
		''' </summary>
		Friend Const MINUTES_PER_HOUR As Integer = 60
		''' <summary>
		''' Minutes per day.
		''' </summary>
		Friend Shared ReadOnly MINUTES_PER_DAY As Integer = MINUTES_PER_HOUR * HOURS_PER_DAY
		''' <summary>
		''' Seconds per minute.
		''' </summary>
		Friend Const SECONDS_PER_MINUTE As Integer = 60
		''' <summary>
		''' Seconds per hour.
		''' </summary>
		Friend Shared ReadOnly SECONDS_PER_HOUR As Integer = SECONDS_PER_MINUTE * MINUTES_PER_HOUR
		''' <summary>
		''' Seconds per day.
		''' </summary>
		Friend Shared ReadOnly SECONDS_PER_DAY As Integer = SECONDS_PER_HOUR * HOURS_PER_DAY
		''' <summary>
		''' Milliseconds per day.
		''' </summary>
		Friend Shared ReadOnly MILLIS_PER_DAY As Long = SECONDS_PER_DAY * 1000L
		''' <summary>
		''' Microseconds per day.
		''' </summary>
		Friend Shared ReadOnly MICROS_PER_DAY As Long = SECONDS_PER_DAY * 1000_000L
		''' <summary>
		''' Nanos per second.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_SECOND As Long = 1000_000_000L
		''' <summary>
		''' Nanos per minute.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_MINUTE As Long = NANOS_PER_SECOND * SECONDS_PER_MINUTE
		''' <summary>
		''' Nanos per hour.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_HOUR As Long = NANOS_PER_MINUTE * MINUTES_PER_HOUR
		''' <summary>
		''' Nanos per day.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_DAY As Long = NANOS_PER_HOUR * HOURS_PER_DAY

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 6414437269572265201L

		''' <summary>
		''' The hour.
		''' </summary>
		Private ReadOnly hour As SByte
		''' <summary>
		''' The minute.
		''' </summary>
		Private ReadOnly minute As SByte
		''' <summary>
		''' The second.
		''' </summary>
		Private ReadOnly second As SByte
		''' <summary>
		''' The nanosecond.
		''' </summary>
		Private ReadOnly nano As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current time from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current time.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current time using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As LocalTime
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current time from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current time.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current time using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As LocalTime
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current time from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current time.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current time, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As LocalTime
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			' inline OffsetTime factory to avoid creating object and InstantProvider checks
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Dim offset As ZoneOffset = clock_Renamed.zone.rules.getOffset(now_Renamed)
			Dim localSecond As Long = now_Renamed.epochSecond + offset.totalSeconds ' overflow caught later
			Dim secsOfDay As Integer = CInt (System.Math.floorMod(localSecond, SECONDS_PER_DAY))
			Return ofNanoOfDay(secsOfDay * NANOS_PER_SECOND + now_Renamed.nano)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalTime} from an hour and minute.
		''' <p>
		''' This returns a {@code LocalTime} with the specified hour and minute.
		''' The second and nanosecond fields will be set to zero.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Shared Function [of](ByVal hour As Integer, ByVal minute As Integer) As LocalTime
			HOUR_OF_DAY.checkValidValue(hour)
			If minute = 0 Then Return HOURS(hour) ' for performance
			MINUTE_OF_HOUR.checkValidValue(minute)
			Return New LocalTime(hour, minute, 0, 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalTime} from an hour, minute and second.
		''' <p>
		''' This returns a {@code LocalTime} with the specified hour, minute and second.
		''' The nanosecond field will be set to zero.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Shared Function [of](ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer) As LocalTime
			HOUR_OF_DAY.checkValidValue(hour)
			If (minute Or second) = 0 Then Return HOURS(hour) ' for performance
			MINUTE_OF_HOUR.checkValidValue(minute)
			SECOND_OF_MINUTE.checkValidValue(second)
			Return New LocalTime(hour, minute, second, 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalTime} from an hour, minute, second and nanosecond.
		''' <p>
		''' This returns a {@code LocalTime} with the specified hour, minute, second and nanosecond.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Shared Function [of](ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer) As LocalTime
			HOUR_OF_DAY.checkValidValue(hour)
			MINUTE_OF_HOUR.checkValidValue(minute)
			SECOND_OF_MINUTE.checkValidValue(second)
			NANO_OF_SECOND.checkValidValue(nanoOfSecond)
			Return create(hour, minute, second, nanoOfSecond)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalTime} from a second-of-day value.
		''' <p>
		''' This returns a {@code LocalTime} with the specified second-of-day.
		''' The nanosecond field will be set to zero.
		''' </summary>
		''' <param name="secondOfDay">  the second-of-day, from {@code 0} to {@code 24 * 60 * 60 - 1} </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if the second-of-day value is invalid </exception>
		Public Shared Function ofSecondOfDay(ByVal secondOfDay As Long) As LocalTime
			SECOND_OF_DAY.checkValidValue(secondOfDay)
			Dim hours_Renamed As Integer = CInt(secondOfDay \ SECONDS_PER_HOUR)
			secondOfDay -= hours_Renamed * SECONDS_PER_HOUR
			Dim minutes As Integer = CInt(secondOfDay \ SECONDS_PER_MINUTE)
			secondOfDay -= minutes * SECONDS_PER_MINUTE
			Return create(hours_Renamed, minutes, CInt(secondOfDay), 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalTime} from a nanos-of-day value.
		''' <p>
		''' This returns a {@code LocalTime} with the specified nanosecond-of-day.
		''' </summary>
		''' <param name="nanoOfDay">  the nano of day, from {@code 0} to {@code 24 * 60 * 60 * 1,000,000,000 - 1} </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if the nanos of day value is invalid </exception>
		Public Shared Function ofNanoOfDay(ByVal nanoOfDay As Long) As LocalTime
			NANO_OF_DAY.checkValidValue(nanoOfDay)
			Dim hours_Renamed As Integer = CInt(nanoOfDay \ NANOS_PER_HOUR)
			nanoOfDay -= hours_Renamed * NANOS_PER_HOUR
			Dim minutes As Integer = CInt(nanoOfDay \ NANOS_PER_MINUTE)
			nanoOfDay -= minutes * NANOS_PER_MINUTE
			Dim seconds As Integer = CInt(nanoOfDay \ NANOS_PER_SECOND)
			nanoOfDay -= seconds * NANOS_PER_SECOND
			Return create(hours_Renamed, minutes, seconds, CInt(nanoOfDay))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalTime} from a temporal object.
		''' <p>
		''' This obtains a local time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code LocalTime}.
		''' <p>
		''' The conversion uses the <seealso cref="TemporalQueries#localTime()"/> query, which relies
		''' on extracting the <seealso cref="ChronoField#NANO_OF_DAY NANO_OF_DAY"/> field.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code LocalTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the local time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code LocalTime} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As LocalTime
			java.util.Objects.requireNonNull(temporal, "temporal")
			Dim time As LocalTime = temporal.query(java.time.temporal.TemporalQueries.localTime())
			If time Is Nothing Then Throw New DateTimeException("Unable to obtain LocalTime from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name)
			Return time
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalTime} from a text string such as {@code 10:15}.
		''' <p>
		''' The string must represent a valid time and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_TIME"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "10:15:30", not null </param>
		''' <returns> the parsed local time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As LocalTime
			Return parse(text, java.time.format.DateTimeFormatter.ISO_LOCAL_TIME)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalTime} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a time.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed local time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As LocalTime
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, LocalTime::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates a local time from the hour, minute, second and nanosecond fields.
		''' <p>
		''' This factory may return a cached value, but applications must not rely on this.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, validated from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, validated from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, validated from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, validated from 0 to 999,999,999 </param>
		''' <returns> the local time, not null </returns>
		Private Shared Function create(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer) As LocalTime
			If (minute Or second Or nanoOfSecond) = 0 Then Return HOURS(hour)
			Return New LocalTime(hour, minute, second, nanoOfSecond)
		End Function

		''' <summary>
		''' Constructor, previously validated.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, validated from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, validated from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, validated from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, validated from 0 to 999,999,999 </param>
		Private Sub New(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer)
			Me.hour = CByte(hour)
			Me.minute = CByte(minute)
			Me.second = CByte(second)
			Me.nano = nanoOfSecond
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this time can be queried for the specified field.
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
		''' </ul>
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this time, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field.timeBased
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this time.
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
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return unit.timeBased
			Return unit IsNot Nothing AndAlso unit.isSupportedBy(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This time is used to enhance the accuracy of the returned range.
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
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange ' override for Javadoc
			Return outerInstance.range(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this time as an {@code int}.
		''' <p>
		''' This queries this time for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this time, except {@code NANO_OF_DAY} and {@code MICRO_OF_DAY}
		''' which are too large to fit in an {@code int} and throw a {@code DateTimeException}.
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
		Public Overrides Function [get](ByVal field As java.time.temporal.TemporalField) As Integer ' override for Javadoc and performance
			If TypeOf field Is java.time.temporal.ChronoField Then Return get0(field)
			Return outerInstance.get(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this time as a {@code long}.
		''' <p>
		''' This queries this time for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this time.
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
				If field = NANO_OF_DAY Then Return toNanoOfDay()
				If field = MICRO_OF_DAY Then Return toNanoOfDay() \ 1000
				Return get0(field)
			End If
			Return field.getFrom(Me)
		End Function

		Private Function get0(ByVal field As java.time.temporal.TemporalField) As Integer
			Select Case CType(field, java.time.temporal.ChronoField)
				Case NANO_OF_SECOND
					Return nano
				Case NANO_OF_DAY
					Throw New java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'NanoOfDay' for get() method, use getLong() instead")
				Case MICRO_OF_SECOND
					Return nano \ 1000
				Case MICRO_OF_DAY
					Throw New java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'MicroOfDay' for get() method, use getLong() instead")
				Case MILLI_OF_SECOND
					Return nano \ 1000000
				Case MILLI_OF_DAY
					Return CInt(toNanoOfDay() \ 1000000)
				Case SECOND_OF_MINUTE
					Return second
				Case SECOND_OF_DAY
					Return toSecondOfDay()
				Case MINUTE_OF_HOUR
					Return minute
				Case MINUTE_OF_DAY
					Return hour * 60 + minute
				Case HOUR_OF_AMPM
					Return hour Mod 12
				Case CLOCK_HOUR_OF_AMPM
					Dim ham As Integer = hour Mod 12
					Return (If(ham Mod 12 = 0, 12, ham))
				Case HOUR_OF_DAY
					Return hour
				Case CLOCK_HOUR_OF_DAY
					Return (If(hour = 0, 24, hour))
				Case AMPM_OF_DAY
					Return hour \ 12
			End Select
			Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the hour-of-day field.
		''' </summary>
		''' <returns> the hour-of-day, from 0 to 23 </returns>
		Public Property hour As Integer
			Get
				Return hour
			End Get
		End Property

		''' <summary>
		''' Gets the minute-of-hour field.
		''' </summary>
		''' <returns> the minute-of-hour, from 0 to 59 </returns>
		Public Property minute As Integer
			Get
				Return minute
			End Get
		End Property

		''' <summary>
		''' Gets the second-of-minute field.
		''' </summary>
		''' <returns> the second-of-minute, from 0 to 59 </returns>
		Public Property second As Integer
			Get
				Return second
			End Get
		End Property

		''' <summary>
		''' Gets the nano-of-second field.
		''' </summary>
		''' <returns> the nano-of-second, from 0 to 999,999,999 </returns>
		Public Property nano As Integer
			Get
				Return nano
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this time.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the time adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the hour field.
		''' A more complex adjuster might set the time to the last hour of the day.
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> a {@code LocalTime} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As LocalTime
			' optimizations
			If TypeOf adjuster Is LocalTime Then Return CType(adjuster, LocalTime)
			Return CType(adjuster.adjustInto(Me), LocalTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified field set to a new value.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the value
		''' for the specified field changed.
		''' This can be used to change any supported field, such as the hour, minute or second.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code NANO_OF_SECOND} -
		'''  Returns a {@code LocalTime} with the specified nano-of-second.
		'''  The hour, minute and second will be unchanged.
		''' <li>{@code NANO_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified nano-of-day.
		'''  This completely replaces the time and is equivalent to <seealso cref="#ofNanoOfDay(long)"/>.
		''' <li>{@code MICRO_OF_SECOND} -
		'''  Returns a {@code LocalTime} with the nano-of-second replaced by the specified
		'''  micro-of-second multiplied by 1,000.
		'''  The hour, minute and second will be unchanged.
		''' <li>{@code MICRO_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified micro-of-day.
		'''  This completely replaces the time and is equivalent to using <seealso cref="#ofNanoOfDay(long)"/>
		'''  with the micro-of-day multiplied by 1,000.
		''' <li>{@code MILLI_OF_SECOND} -
		'''  Returns a {@code LocalTime} with the nano-of-second replaced by the specified
		'''  milli-of-second multiplied by 1,000,000.
		'''  The hour, minute and second will be unchanged.
		''' <li>{@code MILLI_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified milli-of-day.
		'''  This completely replaces the time and is equivalent to using <seealso cref="#ofNanoOfDay(long)"/>
		'''  with the milli-of-day multiplied by 1,000,000.
		''' <li>{@code SECOND_OF_MINUTE} -
		'''  Returns a {@code LocalTime} with the specified second-of-minute.
		'''  The hour, minute and nano-of-second will be unchanged.
		''' <li>{@code SECOND_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified second-of-day.
		'''  The nano-of-second will be unchanged.
		''' <li>{@code MINUTE_OF_HOUR} -
		'''  Returns a {@code LocalTime} with the specified minute-of-hour.
		'''  The hour, second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code MINUTE_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified minute-of-day.
		'''  The second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code HOUR_OF_AMPM} -
		'''  Returns a {@code LocalTime} with the specified hour-of-am-pm.
		'''  The AM/PM, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code CLOCK_HOUR_OF_AMPM} -
		'''  Returns a {@code LocalTime} with the specified clock-hour-of-am-pm.
		'''  The AM/PM, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code HOUR_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified hour-of-day.
		'''  The minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code CLOCK_HOUR_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified clock-hour-of-day.
		'''  The minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		''' <li>{@code AMPM_OF_DAY} -
		'''  Returns a {@code LocalTime} with the specified AM/PM.
		'''  The hour-of-am-pm, minute-of-hour, second-of-minute and nano-of-second will be unchanged.
		''' </ul>
		''' <p>
		''' In all cases, if the new value is outside the valid range of values for the field
		''' then a {@code DateTimeException} will be thrown.
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
		''' <returns> a {@code LocalTime} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As LocalTime
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				f.checkValidValue(newValue)
				Select Case f
					Case NANO_OF_SECOND
						Return withNano(CInt(newValue))
					Case NANO_OF_DAY
						Return LocalTime.ofNanoOfDay(newValue)
					Case MICRO_OF_SECOND
						Return withNano(CInt(newValue) * 1000)
					Case MICRO_OF_DAY
						Return LocalTime.ofNanoOfDay(newValue * 1000)
					Case MILLI_OF_SECOND
						Return withNano(CInt(newValue) * 1000000)
					Case MILLI_OF_DAY
						Return LocalTime.ofNanoOfDay(newValue * 1000000)
					Case SECOND_OF_MINUTE
						Return withSecond(CInt(newValue))
					Case SECOND_OF_DAY
						Return plusSeconds(newValue - toSecondOfDay())
					Case MINUTE_OF_HOUR
						Return withMinute(CInt(newValue))
					Case MINUTE_OF_DAY
						Return plusMinutes(newValue - (hour * 60 + minute))
					Case HOUR_OF_AMPM
						Return plusHours(newValue - (hour Mod 12))
					Case CLOCK_HOUR_OF_AMPM
						Return plusHours((If(newValue = 12, 0, newValue)) - (hour Mod 12))
					Case HOUR_OF_DAY
						Return withHour(CInt(newValue))
					Case CLOCK_HOUR_OF_DAY
						Return withHour(CInt(Fix(If(newValue = 24, 0, newValue))))
					Case AMPM_OF_DAY
						Return plusHours((newValue - (hour \ 12)) * 12)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the hour-of-day altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		''' <returns> a {@code LocalTime} based on this time with the requested hour, not null </returns>
		''' <exception cref="DateTimeException"> if the hour value is invalid </exception>
		Public Function withHour(ByVal hour As Integer) As LocalTime
			If Me.hour = hour Then Return Me
			HOUR_OF_DAY.checkValidValue(hour)
			Return create(hour, minute, second, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the minute-of-hour altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		''' <returns> a {@code LocalTime} based on this time with the requested minute, not null </returns>
		''' <exception cref="DateTimeException"> if the minute value is invalid </exception>
		Public Function withMinute(ByVal minute As Integer) As LocalTime
			If Me.minute = minute Then Return Me
			MINUTE_OF_HOUR.checkValidValue(minute)
			Return create(hour, minute, second, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the second-of-minute altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		''' <returns> a {@code LocalTime} based on this time with the requested second, not null </returns>
		''' <exception cref="DateTimeException"> if the second value is invalid </exception>
		Public Function withSecond(ByVal second As Integer) As LocalTime
			If Me.second = second Then Return Me
			SECOND_OF_MINUTE.checkValidValue(second)
			Return create(hour, minute, second, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the nano-of-second altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		''' <returns> a {@code LocalTime} based on this time with the requested nanosecond, not null </returns>
		''' <exception cref="DateTimeException"> if the nanos value is invalid </exception>
		Public Function withNano(ByVal nanoOfSecond As Integer) As LocalTime
			If Me.nano = nanoOfSecond Then Return Me
			NANO_OF_SECOND.checkValidValue(nanoOfSecond)
			Return create(hour, minute, second, nanoOfSecond)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the time truncated.
		''' <p>
		''' Truncation returns a copy of the original time with fields
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
		''' <returns> a {@code LocalTime} based on this time with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if unable to truncate </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(ByVal unit As java.time.temporal.TemporalUnit) As LocalTime
			If unit = java.time.temporal.ChronoUnit.NANOS Then Return Me
			Dim unitDur As Duration = unit.duration
			If unitDur.seconds > SECONDS_PER_DAY Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unit is too large to be used for truncation")
			Dim dur As Long = unitDur.toNanos()
			If (NANOS_PER_DAY Mod dur) <> 0 Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unit must divide into a standard day without remainder")
			Dim nod As Long = toNanoOfDay()
			Return ofNanoOfDay((nod \ dur) * dur)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this time with the specified amount added.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the specified amount added.
		''' The amount is typically <seealso cref="Duration"/> but may be any other type implementing
		''' the <seealso cref="TemporalAmount"/> interface.
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
		''' <returns> a {@code LocalTime} based on this time with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As LocalTime
			Return CType(amountToAdd.addTo(Me), LocalTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified amount added.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code NANOS} -
		'''  Returns a {@code LocalTime} with the specified number of nanoseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/>.
		''' <li>{@code MICROS} -
		'''  Returns a {@code LocalTime} with the specified number of microseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		'''  multiplied by 1,000.
		''' <li>{@code MILLIS} -
		'''  Returns a {@code LocalTime} with the specified number of milliseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		'''  multiplied by 1,000,000.
		''' <li>{@code SECONDS} -
		'''  Returns a {@code LocalTime} with the specified number of seconds added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/>.
		''' <li>{@code MINUTES} -
		'''  Returns a {@code LocalTime} with the specified number of minutes added.
		'''  This is equivalent to <seealso cref="#plusMinutes(long)"/>.
		''' <li>{@code HOURS} -
		'''  Returns a {@code LocalTime} with the specified number of hours added.
		'''  This is equivalent to <seealso cref="#plusHours(long)"/>.
		''' <li>{@code HALF_DAYS} -
		'''  Returns a {@code LocalTime} with the specified number of half-days added.
		'''  This is equivalent to <seealso cref="#plusHours(long)"/> with the amount
		'''  multiplied by 12.
		''' </ul>
		''' <p>
		''' All other {@code ChronoUnit} instances will throw an {@code UnsupportedTemporalTypeException}.
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
		''' <returns> a {@code LocalTime} based on this time with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalTime
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case NANOS
						Return plusNanos(amountToAdd)
					Case MICROS
						Return plusNanos((amountToAdd Mod MICROS_PER_DAY) * 1000)
					Case MILLIS
						Return plusNanos((amountToAdd Mod MILLIS_PER_DAY) * 1000000)
					Case SECONDS
						Return plusSeconds(amountToAdd)
					Case MINUTES
						Return plusMinutes(amountToAdd)
					Case HOURS
						Return plusHours(amountToAdd)
					Case HALF_DAYS
						Return plusHours((amountToAdd Mod 2) * 12)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of hours added.
		''' <p>
		''' This adds the specified number of hours to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hoursToAdd">  the hours to add, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the hours added, not null </returns>
		Public Function plusHours(ByVal hoursToAdd As Long) As LocalTime
			If hoursToAdd = 0 Then Return Me
			Dim newHour As Integer = (CInt(Fix(hoursToAdd Mod HOURS_PER_DAY)) + hour + HOURS_PER_DAY) Mod HOURS_PER_DAY
			Return create(newHour, minute, second, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of minutes added.
		''' <p>
		''' This adds the specified number of minutes to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutesToAdd">  the minutes to add, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the minutes added, not null </returns>
		Public Function plusMinutes(ByVal minutesToAdd As Long) As LocalTime
			If minutesToAdd = 0 Then Return Me
			Dim mofd As Integer = hour * MINUTES_PER_HOUR + minute
			Dim newMofd As Integer = (CInt(Fix(minutesToAdd Mod MINUTES_PER_DAY)) + mofd + MINUTES_PER_DAY) Mod MINUTES_PER_DAY
			If mofd = newMofd Then Return Me
			Dim newHour As Integer = newMofd \ MINUTES_PER_HOUR
			Dim newMinute As Integer = newMofd Mod MINUTES_PER_HOUR
			Return create(newHour, newMinute, second, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of seconds added.
		''' <p>
		''' This adds the specified number of seconds to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondstoAdd">  the seconds to add, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the seconds added, not null </returns>
		Public Function plusSeconds(ByVal secondstoAdd As Long) As LocalTime
			If secondstoAdd = 0 Then Return Me
			Dim sofd As Integer = hour * SECONDS_PER_HOUR + minute * SECONDS_PER_MINUTE + second
			Dim newSofd As Integer = (CInt(Fix(secondstoAdd Mod SECONDS_PER_DAY)) + sofd + SECONDS_PER_DAY) Mod SECONDS_PER_DAY
			If sofd = newSofd Then Return Me
			Dim newHour As Integer = newSofd \ SECONDS_PER_HOUR
			Dim newMinute As Integer = (newSofd \ SECONDS_PER_MINUTE) Mod MINUTES_PER_HOUR
			Dim newSecond As Integer = newSofd Mod SECONDS_PER_MINUTE
			Return create(newHour, newMinute, newSecond, nano)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of nanoseconds added.
		''' <p>
		''' This adds the specified number of nanoseconds to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToAdd">  the nanos to add, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the nanoseconds added, not null </returns>
		Public Function plusNanos(ByVal nanosToAdd As Long) As LocalTime
			If nanosToAdd = 0 Then Return Me
			Dim nofd As Long = toNanoOfDay()
			Dim newNofd As Long = ((nanosToAdd Mod NANOS_PER_DAY) + nofd + NANOS_PER_DAY) Mod NANOS_PER_DAY
			If nofd = newNofd Then Return Me
			Dim newHour As Integer = CInt(newNofd \ NANOS_PER_HOUR)
			Dim newMinute As Integer = CInt(Fix((newNofd \ NANOS_PER_MINUTE) Mod MINUTES_PER_HOUR))
			Dim newSecond As Integer = CInt(Fix((newNofd \ NANOS_PER_SECOND) Mod SECONDS_PER_MINUTE))
			Dim newNano As Integer = CInt(Fix(newNofd Mod NANOS_PER_SECOND))
			Return create(newHour, newMinute, newSecond, newNano)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the specified amount subtracted.
		''' The amount is typically <seealso cref="Duration"/> but may be any other type implementing
		''' the <seealso cref="TemporalAmount"/> interface.
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
		''' <returns> a {@code LocalTime} based on this time with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As LocalTime
			Return CType(amountToSubtract.subtractFrom(Me), LocalTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalTime}, based on this one, with the amount
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
		''' <returns> a {@code LocalTime} based on this time with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalTime
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of hours subtracted.
		''' <p>
		''' This subtracts the specified number of hours from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hoursToSubtract">  the hours to subtract, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the hours subtracted, not null </returns>
		Public Function minusHours(ByVal hoursToSubtract As Long) As LocalTime
			Return plusHours(-(hoursToSubtract Mod HOURS_PER_DAY))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of minutes subtracted.
		''' <p>
		''' This subtracts the specified number of minutes from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutesToSubtract">  the minutes to subtract, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the minutes subtracted, not null </returns>
		Public Function minusMinutes(ByVal minutesToSubtract As Long) As LocalTime
			Return plusMinutes(-(minutesToSubtract Mod MINUTES_PER_DAY))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of seconds subtracted.
		''' <p>
		''' This subtracts the specified number of seconds from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToSubtract">  the seconds to subtract, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the seconds subtracted, not null </returns>
		Public Function minusSeconds(ByVal secondsToSubtract As Long) As LocalTime
			Return plusSeconds(-(secondsToSubtract Mod SECONDS_PER_DAY))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalTime} with the specified number of nanoseconds subtracted.
		''' <p>
		''' This subtracts the specified number of nanoseconds from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToSubtract">  the nanos to subtract, may be negative </param>
		''' <returns> a {@code LocalTime} based on this time with the nanoseconds subtracted, not null </returns>
		Public Function minusNanos(ByVal nanosToSubtract As Long) As LocalTime
			Return plusNanos(-(nanosToSubtract Mod NANOS_PER_DAY))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this time using the specified query.
		''' <p>
		''' This queries this time using the specified query strategy object.
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
		Public Overrides Function query(Of R)(ByVal query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
			If query_Renamed Is java.time.temporal.TemporalQueries.chronology() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zoneId() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zone() OrElse query_Renamed Is java.time.temporal.TemporalQueries.offset() Then
				Return Nothing
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localTime() Then
				Return CType(Me, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then
				Return Nothing
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.precision() Then
				Return CType(NANOS, R)
			End If
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
			Return query_Renamed.queryFrom(Me)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same time as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the time changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#NANO_OF_DAY"/> as the field.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisLocalTime.adjustInto(temporal);
		'''   temporal = temporal.with(thisLocalTime);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			Return temporal.with(NANO_OF_DAY, toNanoOfDay())
		End Function

		''' <summary>
		''' Calculates the amount of time until another time in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code LocalTime}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified time.
		''' The result will be negative if the end is before the start.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code LocalTime} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in hours between two times can be calculated
		''' using {@code startTime.until(endTime, HOURS)}.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two times.
		''' For example, the amount in hours between 11:30 and 13:29 will only
		''' be one hour as it is one minute short of two hours.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method.
		''' The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		''' <pre>
		'''   // these two lines are equivalent
		'''   amount = start.until(end, MINUTES);
		'''   amount = MINUTES.between(start, end);
		''' </pre>
		''' The choice should be made based on which makes the code more readable.
		''' <p>
		''' The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		''' The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		''' {@code MINUTES}, {@code HOURS} and {@code HALF_DAYS} are supported.
		''' Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end time, exclusive, which is converted to a {@code LocalTime}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this time and the end time </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code LocalTime} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As LocalTime = LocalTime.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim nanosUntil As Long = [end].toNanoOfDay() - toNanoOfDay() ' no overflow
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case NANOS
						Return nanosUntil
					Case MICROS
						Return nanosUntil \ 1000
					Case MILLIS
						Return nanosUntil \ 1000000
					Case SECONDS
						Return nanosUntil \ NANOS_PER_SECOND
					Case MINUTES
						Return nanosUntil \ NANOS_PER_MINUTE
					Case HOURS
						Return nanosUntil \ NANOS_PER_HOUR
					Case HALF_DAYS
						Return nanosUntil \ (12 * NANOS_PER_HOUR)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.between(Me, [end])
		End Function

		''' <summary>
		''' Formats this time using the specified formatter.
		''' <p>
		''' This time will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted time string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this time with a date to create a {@code LocalDateTime}.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this time at the specified date.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="date">  the date to combine with, not null </param>
		''' <returns> the local date-time formed from this time and the specified date, not null </returns>
		Public Function atDate(ByVal [date] As LocalDate) As LocalDateTime
			Return LocalDateTime.of(date_Renamed, Me)
		End Function

		''' <summary>
		''' Combines this time with an offset to create an {@code OffsetTime}.
		''' <p>
		''' This returns an {@code OffsetTime} formed from this time at the specified offset.
		''' All possible combinations of time and offset are valid.
		''' </summary>
		''' <param name="offset">  the offset to combine with, not null </param>
		''' <returns> the offset time formed from this time and the specified offset, not null </returns>
		Public Function atOffset(ByVal offset As ZoneOffset) As OffsetTime
			Return OffsetTime.of(Me, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Extracts the time as seconds of day,
		''' from {@code 0} to {@code 24 * 60 * 60 - 1}.
		''' </summary>
		''' <returns> the second-of-day equivalent to this time </returns>
		Public Function toSecondOfDay() As Integer
			Dim total As Integer = hour * SECONDS_PER_HOUR
			total += minute * SECONDS_PER_MINUTE
			total += second
			Return total
		End Function

		''' <summary>
		''' Extracts the time as nanos of day,
		''' from {@code 0} to {@code 24 * 60 * 60 * 1,000,000,000 - 1}.
		''' </summary>
		''' <returns> the nano of day equivalent to this time </returns>
		Public Function toNanoOfDay() As Long
			Dim total As Long = hour * NANOS_PER_HOUR
			total += minute * NANOS_PER_MINUTE
			total += second * NANOS_PER_SECOND
			total += nano
			Return total
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this time to another time.
		''' <p>
		''' The comparison is based on the time-line position of the local times within a day.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overrides Function compareTo(ByVal other As LocalTime) As Integer Implements Comparable(Of LocalTime).compareTo
			Dim cmp As Integer =  java.lang.[Integer].Compare(hour, other.hour)
			If cmp = 0 Then
				cmp =  java.lang.[Integer].Compare(minute, other.minute)
				If cmp = 0 Then
					cmp =  java.lang.[Integer].Compare(second, other.second)
					If cmp = 0 Then cmp =  java.lang.[Integer].Compare(nano, other.nano)
				End If
			End If
			Return cmp
		End Function

		''' <summary>
		''' Checks if this time is after the specified time.
		''' <p>
		''' The comparison is based on the time-line position of the time within a day.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> true if this is after the specified time </returns>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Function isAfter(ByVal other As LocalTime) As Boolean
			Return compareTo(other) > 0
		End Function

		''' <summary>
		''' Checks if this time is before the specified time.
		''' <p>
		''' The comparison is based on the time-line position of the time within a day.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> true if this point is before the specified time </returns>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Function isBefore(ByVal other As LocalTime) As Boolean
			Return compareTo(other) < 0
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this time is equal to another time.
		''' <p>
		''' The comparison is based on the time-line position of the time within a day.
		''' <p>
		''' Only objects of type {@code LocalTime} are compared, other types return false.
		''' To compare the date of two {@code TemporalAccessor} instances, use
		''' <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other time </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is LocalTime Then
				Dim other As LocalTime = CType(obj, LocalTime)
				Return hour = other.hour AndAlso minute = other.minute AndAlso second = other.second AndAlso nano = other.nano
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim nod As Long = toNanoOfDay()
			Return CInt(Fix(nod Xor (CLng(CULng(nod) >> 32))))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this time as a {@code String}, such as {@code 10:15}.
		''' <p>
		''' The output will be one of the following ISO-8601 formats:
		''' <ul>
		''' <li>{@code HH:mm}</li>
		''' <li>{@code HH:mm:ss}</li>
		''' <li>{@code HH:mm:ss.SSS}</li>
		''' <li>{@code HH:mm:ss.SSSSSS}</li>
		''' <li>{@code HH:mm:ss.SSSSSSSSS}</li>
		''' </ul>
		''' The format used will be the shortest that outputs the full value of
		''' the time where the omitted parts are implied to be zero.
		''' </summary>
		''' <returns> a string representation of this time, not null </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder(18)
			Dim hourValue As Integer = hour
			Dim minuteValue As Integer = minute
			Dim secondValue As Integer = second
			Dim nanoValue As Integer = nano
			buf.append(If(hourValue < 10, "0", "")).append(hourValue).append(If(minuteValue < 10, ":0", ":")).append(minuteValue)
			If secondValue > 0 OrElse nanoValue > 0 Then
				buf.append(If(secondValue < 10, ":0", ":")).append(secondValue)
				If nanoValue > 0 Then
					buf.append("."c)
					If nanoValue Mod 1000000 = 0 Then
						buf.append(Convert.ToString((nanoValue \ 1000000) + 1000).Substring(1))
					ElseIf nanoValue Mod 1000 = 0 Then
						buf.append(Convert.ToString((nanoValue \ 1000) + 1000000).Substring(1))
					Else
						buf.append(Convert.ToString((nanoValue) + 1000000000).Substring(1))
					End If
				End If
			End If
			Return buf.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' A twos-complement value indicates the remaining values are not in the stream
		''' and should be set to zero.
		''' <pre>
		'''  out.writeByte(4);  // identifies a LocalTime
		'''  if (nano == 0) {
		'''    if (second == 0) {
		'''      if (minute == 0) {
		'''        out.writeByte(~hour);
		'''      } else {
		'''        out.writeByte(hour);
		'''        out.writeByte(~minute);
		'''      }
		'''    } else {
		'''      out.writeByte(hour);
		'''      out.writeByte(minute);
		'''      out.writeByte(~second);
		'''    }
		'''  } else {
		'''    out.writeByte(hour);
		'''    out.writeByte(minute);
		'''    out.writeByte(second);
		'''    out.writeInt(nano);
		'''  }
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.LOCAL_TIME_TYPE, Me)
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
			If nano = 0 Then
				If second = 0 Then
					If minute = 0 Then
						out.writeByte((Not hour))
					Else
						out.writeByte(hour)
						out.writeByte((Not minute))
					End If
				Else
					out.writeByte(hour)
					out.writeByte(minute)
					out.writeByte((Not second))
				End If
			Else
				out.writeByte(hour)
				out.writeByte(minute)
				out.writeByte(second)
				out.writeInt(nano)
			End If
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As LocalTime
			Dim hour_Renamed As Integer = [in].readByte()
			Dim minute_Renamed As Integer = 0
			Dim second_Renamed As Integer = 0
			Dim nano_Renamed As Integer = 0
			If hour_Renamed < 0 Then
				hour_Renamed = Not hour_Renamed
			Else
				minute_Renamed = [in].readByte()
				If minute_Renamed < 0 Then
					minute_Renamed = Not minute_Renamed
				Else
					second_Renamed = [in].readByte()
					If second_Renamed < 0 Then
						second_Renamed = Not second_Renamed
					Else
						nano_Renamed = [in].readInt()
					End If
				End If
			End If
			Return LocalTime.of(hour_Renamed, minute_Renamed, second_Renamed, nano_Renamed)
		End Function

	End Class

End Namespace