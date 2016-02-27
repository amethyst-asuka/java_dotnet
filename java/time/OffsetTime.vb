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
	''' A time with an offset from UTC/Greenwich in the ISO-8601 calendar system,
	''' such as {@code 10:15:30+01:00}.
	''' <p>
	''' {@code OffsetTime} is an immutable date-time object that represents a time, often
	''' viewed as hour-minute-second-offset.
	''' This class stores all time fields, to a precision of nanoseconds,
	''' as well as a zone offset.
	''' For example, the value "13:45.30.123456789+02:00" can be stored
	''' in an {@code OffsetTime}.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code OffsetTime} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class OffsetTime
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of OffsetTime)

		''' <summary>
		''' The minimum supported {@code OffsetTime}, '00:00:00+18:00'.
		''' This is the time of midnight at the start of the day in the maximum offset
		''' (larger offsets are earlier on the time-line).
		''' This combines <seealso cref="LocalTime#MIN"/> and <seealso cref="ZoneOffset#MAX"/>.
		''' This could be used by an application as a "far past" date.
		''' </summary>
		Public Shared ReadOnly MIN As OffsetTime = LocalTime.MIN.atOffset(ZoneOffset.MAX)
		''' <summary>
		''' The maximum supported {@code OffsetTime}, '23:59:59.999999999-18:00'.
		''' This is the time just before midnight at the end of the day in the minimum offset
		''' (larger negative offsets are later on the time-line).
		''' This combines <seealso cref="LocalTime#MAX"/> and <seealso cref="ZoneOffset#MIN"/>.
		''' This could be used by an application as a "far future" date.
		''' </summary>
		Public Shared ReadOnly MAX As OffsetTime = LocalTime.MAX.atOffset(ZoneOffset.MIN)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 7264499704384272492L

		''' <summary>
		''' The local date-time.
		''' </summary>
		Private ReadOnly time As LocalTime
		''' <summary>
		''' The offset from UTC/Greenwich.
		''' </summary>
		Private ReadOnly offset As ZoneOffset

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current time from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current time.
		''' The offset will be calculated from the time-zone in the clock.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current time using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As OffsetTime
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current time from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current time.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' The offset will be calculated from the specified time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current time using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As OffsetTime
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current time from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current time.
		''' The offset will be calculated from the time-zone in the clock.
		''' <p>
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current time, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As OffsetTime
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Return ofInstant(now_Renamed, clock_Renamed.zone.rules.getOffset(now_Renamed))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from a local time and an offset.
		''' </summary>
		''' <param name="time">  the local time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the offset time, not null </returns>
		Public Shared Function [of](ByVal time As LocalTime, ByVal offset As ZoneOffset) As OffsetTime
			Return New OffsetTime(time, offset)
		End Function

		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from an hour, minute, second and nanosecond.
		''' <p>
		''' This creates an offset time with the four specified fields.
		''' <p>
		''' This method exists primarily for writing test cases.
		''' Non test-code will typically use other methods to create an offset time.
		''' {@code LocalTime} has two additional convenience variants of the
		''' equivalent factory method taking fewer arguments.
		''' They are not provided here to reduce the footprint of the API.
		''' </summary>
		''' <param name="hour">  the hour-of-day to represent, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to represent, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <returns> the offset time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Shared Function [of](ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer, ByVal offset As ZoneOffset) As OffsetTime
			Return New OffsetTime(LocalTime.of(hour, minute, second, nanoOfSecond), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from an {@code Instant} and zone ID.
		''' <p>
		''' This creates an offset time with the same instant as that specified.
		''' Finding the offset from UTC/Greenwich is simple as there is only one valid
		''' offset for each instant.
		''' <p>
		''' The date component of the instant is dropped during the conversion.
		''' This means that the conversion can never fail due to the instant being
		''' out of the valid range of dates.
		''' </summary>
		''' <param name="instant">  the instant to create the time from, not null </param>
		''' <param name="zone">  the time-zone, which may be an offset, not null </param>
		''' <returns> the offset time, not null </returns>
		Public Shared Function ofInstant(ByVal instant_Renamed As Instant, ByVal zone As ZoneId) As OffsetTime
			java.util.Objects.requireNonNull(instant_Renamed, "instant")
			java.util.Objects.requireNonNull(zone, "zone")
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim offset_Renamed As ZoneOffset = rules.getOffset(instant_Renamed)
			Dim localSecond As Long = instant_Renamed.epochSecond + offset_Renamed.totalSeconds ' overflow caught later
			Dim secsOfDay As Integer = CInt (System.Math.floorMod(localSecond, SECONDS_PER_DAY))
			Dim time As LocalTime = LocalTime.ofNanoOfDay(secsOfDay * NANOS_PER_SECOND + instant_Renamed.nano)
			Return New OffsetTime(time, offset_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from a temporal object.
		''' <p>
		''' This obtains an offset time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code OffsetTime}.
		''' <p>
		''' The conversion extracts and combines the {@code ZoneOffset} and the
		''' {@code LocalTime} from the temporal object.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code OffsetTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the offset time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code OffsetTime} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As OffsetTime
			If TypeOf temporal Is OffsetTime Then Return CType(temporal, OffsetTime)
			Try
				Dim time As LocalTime = LocalTime.from(temporal)
				Dim offset_Renamed As ZoneOffset = ZoneOffset.from(temporal)
				Return New OffsetTime(time, offset_Renamed)
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain OffsetTime from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from a text string such as {@code 10:15:30+01:00}.
		''' <p>
		''' The string must represent a valid time and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_OFFSET_TIME"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "10:15:30+01:00", not null </param>
		''' <returns> the parsed local time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As OffsetTime
			Return parse(text, java.time.format.DateTimeFormatter.ISO_OFFSET_TIME)
		End Function

		''' <summary>
		''' Obtains an instance of {@code OffsetTime} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a time.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed offset time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As OffsetTime
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, OffsetTime::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="time">  the local time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		Private Sub New(ByVal time As LocalTime, ByVal offset As ZoneOffset)
			Me.time = java.util.Objects.requireNonNull(time, "time")
			Me.offset = java.util.Objects.requireNonNull(offset, "offset")
		End Sub

		''' <summary>
		''' Returns a new time based on this one, returning {@code this} where possible.
		''' </summary>
		''' <param name="time">  the time to create with, not null </param>
		''' <param name="offset">  the zone offset to create with, not null </param>
		Private Function [with](ByVal time As LocalTime, ByVal offset As ZoneOffset) As OffsetTime
			If Me.time Is time AndAlso Me.offset.Equals(offset) Then Return Me
			Return New OffsetTime(time, offset)
		End Function

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
		''' <returns> true if the field is supported on this time, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field.timeBased OrElse field = OFFSET_SECONDS
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this offset-time.
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
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				If field = OFFSET_SECONDS Then Return field.range()
				Return time.range(field)
			End If
			Return field.rangeRefinedBy(Me)
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
		Public Overrides Function [get](ByVal field As java.time.temporal.TemporalField) As Integer ' override for Javadoc
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
				If field = OFFSET_SECONDS Then Return offset.totalSeconds
				Return time.getLong(field)
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the zone offset, such as '+01:00'.
		''' <p>
		''' This is the offset of the local time from UTC/Greenwich.
		''' </summary>
		''' <returns> the zone offset, not null </returns>
		Public Property offset As ZoneOffset
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified offset ensuring
		''' that the result has the same local time.
		''' <p>
		''' This method returns an object with the same {@code LocalTime} and the specified {@code ZoneOffset}.
		''' No calculation is needed or performed.
		''' For example, if this time represents {@code 10:30+02:00} and the offset specified is
		''' {@code +03:00}, then this method will return {@code 10:30+03:00}.
		''' <p>
		''' To take into account the difference between the offsets, and adjust the time fields,
		''' use <seealso cref="#withOffsetSameInstant"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="offset">  the zone offset to change to, not null </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested offset, not null </returns>
		Public Function withOffsetSameLocal(ByVal offset As ZoneOffset) As OffsetTime
			Return If(offset IsNot Nothing AndAlso offset.Equals(Me.offset), Me, New OffsetTime(time, offset))
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified offset ensuring
		''' that the result is at the same instant on an implied day.
		''' <p>
		''' This method returns an object with the specified {@code ZoneOffset} and a {@code LocalTime}
		''' adjusted by the difference between the two offsets.
		''' This will result in the old and new objects representing the same instant on an implied day.
		''' This is useful for finding the local time in a different offset.
		''' For example, if this time represents {@code 10:30+02:00} and the offset specified is
		''' {@code +03:00}, then this method will return {@code 11:30+03:00}.
		''' <p>
		''' To change the offset without adjusting the local time use <seealso cref="#withOffsetSameLocal"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="offset">  the zone offset to change to, not null </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested offset, not null </returns>
		Public Function withOffsetSameInstant(ByVal offset As ZoneOffset) As OffsetTime
			If offset.Equals(Me.offset) Then Return Me
			Dim difference As Integer = offset.totalSeconds - Me.offset.totalSeconds
			Dim adjusted As LocalTime = time.plusSeconds(difference)
			Return New OffsetTime(adjusted, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalTime} part of this date-time.
		''' <p>
		''' This returns a {@code LocalTime} with the same hour, minute, second and
		''' nanosecond as this date-time.
		''' </summary>
		''' <returns> the time part of this date-time, not null </returns>
		Public Function toLocalTime() As LocalTime
			Return time
		End Function

		'-----------------------------------------------------------------------
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
		''' Returns an adjusted copy of this time.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the time adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the hour field.
		''' A more complex adjuster might set the time to the last hour of the day.
		''' <p>
		''' The classes <seealso cref="LocalTime"/> and <seealso cref="ZoneOffset"/> implement {@code TemporalAdjuster},
		''' thus this method can be used to change the time or offset:
		''' <pre>
		'''  result = offsetTime.with(time);
		'''  result = offsetTime.with(offset);
		''' </pre>
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> an {@code OffsetTime} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As OffsetTime
			' optimizations
			If TypeOf adjuster Is LocalTime Then
				Return [with](CType(adjuster, LocalTime), offset)
			ElseIf TypeOf adjuster Is ZoneOffset Then
				Return [with](time, CType(adjuster, ZoneOffset))
			ElseIf TypeOf adjuster Is OffsetTime Then
				Return CType(adjuster, OffsetTime)
			End If
			Return CType(adjuster.adjustInto(Me), OffsetTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified field set to a new value.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the value
		''' for the specified field changed.
		''' This can be used to change any supported field, such as the hour, minute or second.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' <p>
		''' The {@code OFFSET_SECONDS} field will return a time with the specified offset.
		''' The local time is unaltered. If the new offset value is outside the valid range
		''' then a {@code DateTimeException} will be thrown.
		''' <p>
		''' The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		''' the matching method on <seealso cref="LocalTime#with(TemporalField, long)"/> LocalTime}.
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
		''' <returns> an {@code OffsetTime} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As OffsetTime
			If TypeOf field Is java.time.temporal.ChronoField Then
				If field = OFFSET_SECONDS Then
					Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
					Return [with](time, ZoneOffset.ofTotalSeconds(f.checkValidIntValue(newValue)))
				End If
				Return [with](time.with(field, newValue), offset)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the hour-of-day altered.
		''' <p>
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested hour, not null </returns>
		''' <exception cref="DateTimeException"> if the hour value is invalid </exception>
		Public Function withHour(ByVal hour As Integer) As OffsetTime
			Return [with](time.withHour(hour), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the minute-of-hour altered.
		''' <p>
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested minute, not null </returns>
		''' <exception cref="DateTimeException"> if the minute value is invalid </exception>
		Public Function withMinute(ByVal minute As Integer) As OffsetTime
			Return [with](time.withMinute(minute), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the second-of-minute altered.
		''' <p>
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested second, not null </returns>
		''' <exception cref="DateTimeException"> if the second value is invalid </exception>
		Public Function withSecond(ByVal second As Integer) As OffsetTime
			Return [with](time.withSecond(second), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the nano-of-second altered.
		''' <p>
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		''' <returns> an {@code OffsetTime} based on this time with the requested nanosecond, not null </returns>
		''' <exception cref="DateTimeException"> if the nanos value is invalid </exception>
		Public Function withNano(ByVal nanoOfSecond As Integer) As OffsetTime
			Return [with](time.withNano(nanoOfSecond), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the time truncated.
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
		''' The offset does not affect the calculation and will be the same in the result.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="unit">  the unit to truncate to, not null </param>
		''' <returns> an {@code OffsetTime} based on this time with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if unable to truncate </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(ByVal unit As java.time.temporal.TemporalUnit) As OffsetTime
			Return [with](time.truncatedTo(unit), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this time with the specified amount added.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the specified amount added.
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
		''' <returns> an {@code OffsetTime} based on this time with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As OffsetTime
			Return CType(amountToAdd.addTo(Me), OffsetTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified amount added.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented by
		''' <seealso cref="LocalTime#plus(long, TemporalUnit)"/>.
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
		''' <returns> an {@code OffsetTime} based on this time with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As OffsetTime
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return [with](time.plus(amountToAdd, unit), offset)
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of hours added.
		''' <p>
		''' This adds the specified number of hours to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to add, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the hours added, not null </returns>
		Public Function plusHours(ByVal hours As Long) As OffsetTime
			Return [with](time.plusHours(hours), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of minutes added.
		''' <p>
		''' This adds the specified number of minutes to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to add, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the minutes added, not null </returns>
		Public Function plusMinutes(ByVal minutes As Long) As OffsetTime
			Return [with](time.plusMinutes(minutes), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of seconds added.
		''' <p>
		''' This adds the specified number of seconds to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to add, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the seconds added, not null </returns>
		Public Function plusSeconds(ByVal seconds As Long) As OffsetTime
			Return [with](time.plusSeconds(seconds), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of nanoseconds added.
		''' <p>
		''' This adds the specified number of nanoseconds to this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to add, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the nanoseconds added, not null </returns>
		Public Function plusNanos(ByVal nanos As Long) As OffsetTime
			Return [with](time.plusNanos(nanos), offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this time with the specified amount subtracted.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the specified amount subtracted.
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
		''' <returns> an {@code OffsetTime} based on this time with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As OffsetTime
			Return CType(amountToSubtract.subtractFrom(Me), OffsetTime)
		End Function

		''' <summary>
		''' Returns a copy of this time with the specified amount subtracted.
		''' <p>
		''' This returns an {@code OffsetTime}, based on this one, with the amount
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
		''' <returns> an {@code OffsetTime} based on this time with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As OffsetTime
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of hours subtracted.
		''' <p>
		''' This subtracts the specified number of hours from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to subtract, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the hours subtracted, not null </returns>
		Public Function minusHours(ByVal hours As Long) As OffsetTime
			Return [with](time.minusHours(hours), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of minutes subtracted.
		''' <p>
		''' This subtracts the specified number of minutes from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to subtract, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the minutes subtracted, not null </returns>
		Public Function minusMinutes(ByVal minutes As Long) As OffsetTime
			Return [with](time.minusMinutes(minutes), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of seconds subtracted.
		''' <p>
		''' This subtracts the specified number of seconds from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to subtract, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the seconds subtracted, not null </returns>
		Public Function minusSeconds(ByVal seconds As Long) As OffsetTime
			Return [with](time.minusSeconds(seconds), offset)
		End Function

		''' <summary>
		''' Returns a copy of this {@code OffsetTime} with the specified number of nanoseconds subtracted.
		''' <p>
		''' This subtracts the specified number of nanoseconds from this time, returning a new time.
		''' The calculation wraps around midnight.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to subtract, may be negative </param>
		''' <returns> an {@code OffsetTime} based on this time with the nanoseconds subtracted, not null </returns>
		Public Function minusNanos(ByVal nanos As Long) As OffsetTime
			Return [with](time.minusNanos(nanos), offset)
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
			If query_Renamed Is java.time.temporal.TemporalQueries.offset() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zone() Then
				Return CType(offset, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.zoneId() Or query_Renamed Is java.time.temporal.TemporalQueries.chronology() OrElse query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then
				Return Nothing
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localTime() Then
				Return CType(time, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.precision() Then
				Return CType(NANOS, R)
			End If
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
			Return query_Renamed.queryFrom(Me)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same offset and time
		''' as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the offset and time changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' twice, passing <seealso cref="ChronoField#NANO_OF_DAY"/> and
		''' <seealso cref="ChronoField#OFFSET_SECONDS"/> as the fields.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisOffsetTime.adjustInto(temporal);
		'''   temporal = temporal.with(thisOffsetTime);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			Return temporal.with(NANO_OF_DAY, time.toNanoOfDay()).with(OFFSET_SECONDS, offset.totalSeconds)
		End Function

		''' <summary>
		''' Calculates the amount of time until another time in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code OffsetTime}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified time.
		''' The result will be negative if the end is before the start.
		''' For example, the amount in hours between two times can be calculated
		''' using {@code startTime.until(endTime, HOURS)}.
		''' <p>
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code OffsetTime} using <seealso cref="#from(TemporalAccessor)"/>.
		''' If the offset differs between the two times, then the specified
		''' end time is normalized to have the same offset as this time.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two times.
		''' For example, the amount in hours between 11:30Z and 13:29Z will only
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
		''' <param name="endExclusive">  the end time, exclusive, which is converted to an {@code OffsetTime}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this time and the end time </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to an {@code OffsetTime} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As OffsetTime = OffsetTime.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim nanosUntil As Long = [end].toEpochNano() - toEpochNano() ' no overflow
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case NANOS
						Return nanosUntil
					Case MICROS
						Return nanosUntil \ 1000
					Case MILLIS
						Return nanosUntil \ 1000000
					Case SECONDS
						Return nanosUntil / NANOS_PER_SECOND
					Case MINUTES
						Return nanosUntil / NANOS_PER_MINUTE
					Case HOURS
						Return nanosUntil / NANOS_PER_HOUR
					Case HALF_DAYS
						Return nanosUntil / (12 * NANOS_PER_HOUR)
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
		''' Combines this time with a date to create an {@code OffsetDateTime}.
		''' <p>
		''' This returns an {@code OffsetDateTime} formed from this time and the specified date.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="date">  the date to combine with, not null </param>
		''' <returns> the offset date-time formed from this time and the specified date, not null </returns>
		Public Function atDate(ByVal [date] As LocalDate) As OffsetDateTime
			Return OffsetDateTime.of(date_Renamed, time, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this time to epoch nanos based on 1970-01-01Z.
		''' </summary>
		''' <returns> the epoch nanos value </returns>
		Private Function toEpochNano() As Long
			Dim nod As Long = time.toNanoOfDay()
			Dim offsetNanos As Long = offset.totalSeconds * NANOS_PER_SECOND
			Return nod - offsetNanos
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this {@code OffsetTime} to another time.
		''' <p>
		''' The comparison is based first on the UTC equivalent instant, then on the local time.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' For example, the following is the comparator order:
		''' <ol>
		''' <li>{@code 10:30+01:00}</li>
		''' <li>{@code 11:00+01:00}</li>
		''' <li>{@code 12:00+02:00}</li>
		''' <li>{@code 11:30+01:00}</li>
		''' <li>{@code 12:00+01:00}</li>
		''' <li>{@code 12:30+01:00}</li>
		''' </ol>
		''' Values #2 and #3 represent the same instant on the time-line.
		''' When two values represent the same instant, the local time is compared
		''' to distinguish them. This step is needed to make the ordering
		''' consistent with {@code equals()}.
		''' <p>
		''' To compare the underlying local time of two {@code TemporalAccessor} instances,
		''' use <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overrides Function compareTo(ByVal other As OffsetTime) As Integer Implements Comparable(Of OffsetTime).compareTo
			If offset.Equals(other.offset) Then Return time.CompareTo(other.time)
			Dim compare As Integer = java.lang.[Long].Compare(toEpochNano(), other.toEpochNano())
			If compare = 0 Then compare = time.CompareTo(other.time)
			Return compare
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the instant of this {@code OffsetTime} is after that of the
		''' specified time applying both times to a common date.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the instant of the time. This is equivalent to converting both
		''' times to an instant using the same date and comparing the instants.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> true if this is after the instant of the specified time </returns>
		Public Function isAfter(ByVal other As OffsetTime) As Boolean
			Return toEpochNano() > other.toEpochNano()
		End Function

		''' <summary>
		''' Checks if the instant of this {@code OffsetTime} is before that of the
		''' specified time applying both times to a common date.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the instant of the time. This is equivalent to converting both
		''' times to an instant using the same date and comparing the instants.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> true if this is before the instant of the specified time </returns>
		Public Function isBefore(ByVal other As OffsetTime) As Boolean
			Return toEpochNano() < other.toEpochNano()
		End Function

		''' <summary>
		''' Checks if the instant of this {@code OffsetTime} is equal to that of the
		''' specified time applying both times to a common date.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		''' in that it only compares the instant of the time. This is equivalent to converting both
		''' times to an instant using the same date and comparing the instants.
		''' </summary>
		''' <param name="other">  the other time to compare to, not null </param>
		''' <returns> true if this is equal to the instant of the specified time </returns>
		Public Function isEqual(ByVal other As OffsetTime) As Boolean
			Return toEpochNano() = other.toEpochNano()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this time is equal to another time.
		''' <p>
		''' The comparison is based on the local-time and the offset.
		''' To compare for the same instant on the time-line, use <seealso cref="#isEqual(OffsetTime)"/>.
		''' <p>
		''' Only objects of type {@code OffsetTime} are compared, other types return false.
		''' To compare the underlying local time of two {@code TemporalAccessor} instances,
		''' use <seealso cref="ChronoField#NANO_OF_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other time </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is OffsetTime Then
				Dim other As OffsetTime = CType(obj, OffsetTime)
				Return time.Equals(other.time) AndAlso offset.Equals(other.offset)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return time.GetHashCode() Xor offset.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this time as a {@code String}, such as {@code 10:15:30+01:00}.
		''' <p>
		''' The output will be one of the following ISO-8601 formats:
		''' <ul>
		''' <li>{@code HH:mmXXXXX}</li>
		''' <li>{@code HH:mm:ssXXXXX}</li>
		''' <li>{@code HH:mm:ss.SSSXXXXX}</li>
		''' <li>{@code HH:mm:ss.SSSSSSXXXXX}</li>
		''' <li>{@code HH:mm:ss.SSSSSSSSSXXXXX}</li>
		''' </ul>
		''' The format used will be the shortest that outputs the full value of
		''' the time where the omitted parts are implied to be zero.
		''' </summary>
		''' <returns> a string representation of this time, not null </returns>
		Public Overrides Function ToString() As String
			Return time.ToString() & offset.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(9);  // identifies an OffsetTime
		'''  // the <a href="../../serialized-form.html#java.time.LocalTime">time</a> excluding the one byte header
		'''  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.OFFSET_TIME_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.ObjectOutput)
			time.writeExternal(out)
			offset.writeExternal(out)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.ObjectInput) As OffsetTime
			Dim time As LocalTime = LocalTime.readExternal([in])
			Dim offset_Renamed As ZoneOffset = ZoneOffset.readExternal([in])
			Return OffsetTime.of(time, offset_Renamed)
		End Function

	End Class

End Namespace