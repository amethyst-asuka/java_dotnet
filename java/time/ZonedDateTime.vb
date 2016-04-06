Imports System
Imports System.Collections.Generic

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
	''' A date-time with a time-zone in the ISO-8601 calendar system,
	''' such as {@code 2007-12-03T10:15:30+01:00 Europe/Paris}.
	''' <p>
	''' {@code ZonedDateTime} is an immutable representation of a date-time with a time-zone.
	''' This class stores all date and time fields, to a precision of nanoseconds,
	''' and a time-zone, with a zone offset used to handle ambiguous local date-times.
	''' For example, the value
	''' "2nd October 2007 at 13:45.30.123456789 +02:00 in the Europe/Paris time-zone"
	''' can be stored in a {@code ZonedDateTime}.
	''' <p>
	''' This class handles conversion from the local time-line of {@code LocalDateTime}
	''' to the instant time-line of {@code Instant}.
	''' The difference between the two time-lines is the offset from UTC/Greenwich,
	''' represented by a {@code ZoneOffset}.
	''' <p>
	''' Converting between the two time-lines involves calculating the offset using the
	''' <seealso cref="ZoneRules rules"/> accessed from the {@code ZoneId}.
	''' Obtaining the offset for an instant is simple, as there is exactly one valid
	''' offset for each instant. By contrast, obtaining the offset for a local date-time
	''' is not straightforward. There are three cases:
	''' <ul>
	''' <li>Normal, with one valid offset. For the vast majority of the year, the normal
	'''  case applies, where there is a single valid offset for the local date-time.</li>
	''' <li>Gap, with zero valid offsets. This is when clocks jump forward typically
	'''  due to the spring daylight savings change from "winter" to "summer".
	'''  In a gap there are local date-time values with no valid offset.</li>
	''' <li>Overlap, with two valid offsets. This is when clocks are set back typically
	'''  due to the autumn daylight savings change from "summer" to "winter".
	'''  In an overlap there are local date-time values with two valid offsets.</li>
	''' </ul>
	''' <p>
	''' Any method that converts directly or implicitly from a local date-time to an
	''' instant by obtaining the offset has the potential to be complicated.
	''' <p>
	''' For Gaps, the general strategy is that if the local date-time falls in the
	''' middle of a Gap, then the resulting zoned date-time will have a local date-time
	''' shifted forwards by the length of the Gap, resulting in a date-time in the later
	''' offset, typically "summer" time.
	''' <p>
	''' For Overlaps, the general strategy is that if the local date-time falls in the
	''' middle of an Overlap, then the previous offset will be retained. If there is no
	''' previous offset, or the previous offset is invalid, then the earlier offset is
	''' used, typically "summer" time.. Two additional methods,
	''' <seealso cref="#withEarlierOffsetAtOverlap()"/> and <seealso cref="#withLaterOffsetAtOverlap()"/>,
	''' help manage the case of an overlap.
	''' <p>
	''' In terms of design, this class should be viewed primarily as the combination
	''' of a {@code LocalDateTime} and a {@code ZoneId}. The {@code ZoneOffset} is
	''' a vital, but secondary, piece of information, used to ensure that the class
	''' represents an instant, especially during a daylight savings overlap.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code ZonedDateTime} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' A {@code ZonedDateTime} holds state equivalent to three separate objects,
	''' a {@code LocalDateTime}, a {@code ZoneId} and the resolved {@code ZoneOffset}.
	''' The offset and local date-time are used to define an instant when necessary.
	''' The zone ID is used to obtain the rules for how and when the offset changes.
	''' The offset cannot be freely set, as the zone controls which offsets are valid.
	''' <p>
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ZonedDateTime
		Implements java.time.temporal.Temporal, java.time.chrono.ChronoZonedDateTime(Of LocalDate)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -6260982410461394882L

		''' <summary>
		''' The local date-time.
		''' </summary>
		Private ReadOnly dateTime As LocalDateTime
		''' <summary>
		''' The offset from UTC/Greenwich.
		''' </summary>
		Private ReadOnly offset As ZoneOffset
		''' <summary>
		''' The time-zone.
		''' </summary>
		Private ReadOnly zone As ZoneId

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current date-time from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date-time.
		''' The zone and offset will be set based on the time-zone in the clock.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date-time using the system clock, not null </returns>
		Public Shared Function now() As ZonedDateTime
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
		Public Shared Function now(  zone As ZoneId) As ZonedDateTime
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current date-time from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date-time.
		''' The zone and offset will be set based on the time-zone in the clock.
		''' <p>
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date-time, not null </returns>
		Public Shared Function now(  clock_Renamed As Clock) As ZonedDateTime
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Return ofInstant(now_Renamed, clock_Renamed.zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a local date and time.
		''' <p>
		''' This creates a zoned date-time matching the input local date and time as closely as possible.
		''' Time-zone rules, such as daylight savings, mean that not every local date-time
		''' is valid for the specified zone, thus the local date-time may be adjusted.
		''' <p>
		''' The local date time and first combined to form a local date-time.
		''' The local date-time is then resolved to a single instant on the time-line.
		''' This is achieved by finding a valid offset from UTC/Greenwich for the local
		''' date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, when clocks are set back, there are two valid offsets.
		''' This method uses the earlier offset typically corresponding to "summer".
		''' <p>
		''' In the case of a gap, when clocks jump forward, there is no valid offset.
		''' Instead, the local date-time is adjusted to be later by the length of the gap.
		''' For a typical one hour daylight savings change, the local date-time will be
		''' moved one hour later into the offset typically corresponding to "summer".
		''' </summary>
		''' <param name="date">  the local date, not null </param>
		''' <param name="time">  the local time, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the offset date-time, not null </returns>
		Public Shared Function [of](  [date] As LocalDate,   time As LocalTime,   zone As ZoneId) As ZonedDateTime
			Return [of](LocalDateTime.of(date_Renamed, time), zone)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a local date-time.
		''' <p>
		''' This creates a zoned date-time matching the input local date-time as closely as possible.
		''' Time-zone rules, such as daylight savings, mean that not every local date-time
		''' is valid for the specified zone, thus the local date-time may be adjusted.
		''' <p>
		''' The local date-time is resolved to a single instant on the time-line.
		''' This is achieved by finding a valid offset from UTC/Greenwich for the local
		''' date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, when clocks are set back, there are two valid offsets.
		''' This method uses the earlier offset typically corresponding to "summer".
		''' <p>
		''' In the case of a gap, when clocks jump forward, there is no valid offset.
		''' Instead, the local date-time is adjusted to be later by the length of the gap.
		''' For a typical one hour daylight savings change, the local date-time will be
		''' moved one hour later into the offset typically corresponding to "summer".
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Public Shared Function [of](  localDateTime_Renamed As LocalDateTime,   zone As ZoneId) As ZonedDateTime
			Return ofLocal(localDateTime_Renamed, zone, Nothing)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a year, month, day,
		''' hour, minute, second, nanosecond and time-zone.
		''' <p>
		''' This creates a zoned date-time matching the local date-time of the seven
		''' specified fields as closely as possible.
		''' Time-zone rules, such as daylight savings, mean that not every local date-time
		''' is valid for the specified zone, thus the local date-time may be adjusted.
		''' <p>
		''' The local date-time is resolved to a single instant on the time-line.
		''' This is achieved by finding a valid offset from UTC/Greenwich for the local
		''' date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, when clocks are set back, there are two valid offsets.
		''' This method uses the earlier offset typically corresponding to "summer".
		''' <p>
		''' In the case of a gap, when clocks jump forward, there is no valid offset.
		''' Instead, the local date-time is adjusted to be later by the length of the gap.
		''' For a typical one hour daylight savings change, the local date-time will be
		''' moved one hour later into the offset typically corresponding to "summer".
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
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the offset date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range, or
		'''  if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](  year_Renamed As Integer,   month As Integer,   dayOfMonth As Integer,   hour As Integer,   minute As Integer,   second As Integer,   nanoOfSecond As Integer,   zone As ZoneId) As ZonedDateTime
			Dim dt As LocalDateTime = LocalDateTime.of(year_Renamed, month, dayOfMonth, hour, minute, second, nanoOfSecond)
			Return ofLocal(dt, zone, Nothing)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a local date-time
		''' using the preferred offset if possible.
		''' <p>
		''' The local date-time is resolved to a single instant on the time-line.
		''' This is achieved by finding a valid offset from UTC/Greenwich for the local
		''' date-time as defined by the <seealso cref="ZoneRules rules"/> of the zone ID.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, where clocks are set back, there are two valid offsets.
		''' If the preferred offset is one of the valid offsets then it is used.
		''' Otherwise the earlier valid offset is used, typically corresponding to "summer".
		''' <p>
		''' In the case of a gap, where clocks jump forward, there is no valid offset.
		''' Instead, the local date-time is adjusted to be later by the length of the gap.
		''' For a typical one hour daylight savings change, the local date-time will be
		''' moved one hour later into the offset typically corresponding to "summer".
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <param name="preferredOffset">  the zone offset, null if no preference </param>
		''' <returns> the zoned date-time, not null </returns>
		Public Shared Function ofLocal(  localDateTime_Renamed As LocalDateTime,   zone As ZoneId,   preferredOffset As ZoneOffset) As ZonedDateTime
			java.util.Objects.requireNonNull(localDateTime_Renamed, "localDateTime")
			java.util.Objects.requireNonNull(zone, "zone")
			If TypeOf zone Is ZoneOffset Then Return New ZonedDateTime(localDateTime_Renamed, CType(zone, ZoneOffset), zone)
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim validOffsets As IList(Of ZoneOffset) = rules.getValidOffsets(localDateTime_Renamed)
			Dim offset_Renamed As ZoneOffset
			If validOffsets.Count = 1 Then
				offset_Renamed = validOffsets(0)
			ElseIf validOffsets.Count = 0 Then
				Dim trans As java.time.zone.ZoneOffsetTransition = rules.getTransition(localDateTime_Renamed)
				localDateTime_Renamed = localDateTime_Renamed.plusSeconds(trans.duration.seconds)
				offset_Renamed = trans.offsetAfter
			Else
				If preferredOffset IsNot Nothing AndAlso validOffsets.Contains(preferredOffset) Then
					offset_Renamed = preferredOffset
				Else
					offset_Renamed = java.util.Objects.requireNonNull(validOffsets(0), "offset") ' protect against bad ZoneRules
				End If
			End If
			Return New ZonedDateTime(localDateTime_Renamed, offset_Renamed, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from an {@code Instant}.
		''' <p>
		''' This creates a zoned date-time with the same instant as that specified.
		''' Calling <seealso cref="#toInstant()"/> will return an instant equal to the one used here.
		''' <p>
		''' Converting an instant to a zoned date-time is simple as there is only one valid
		''' offset for each instant.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Shared Function ofInstant(  instant_Renamed As Instant,   zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(instant_Renamed, "instant")
			java.util.Objects.requireNonNull(zone, "zone")
			Return create(instant_Renamed.epochSecond, instant_Renamed.nano, zone)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from the instant formed by combining
		''' the local date-time and offset.
		''' <p>
		''' This creates a zoned date-time by <seealso cref="LocalDateTime#toInstant(ZoneOffset) combining"/>
		''' the {@code LocalDateTime} and {@code ZoneOffset}.
		''' This combination uniquely specifies an instant without ambiguity.
		''' <p>
		''' Converting an instant to a zoned date-time is simple as there is only one valid
		''' offset for each instant. If the valid offset is different to the offset specified,
		''' then the date-time and offset of the zoned date-time will differ from those specified.
		''' <p>
		''' If the {@code ZoneId} to be used is a {@code ZoneOffset}, this method is equivalent
		''' to <seealso cref="#of(LocalDateTime, ZoneId)"/>.
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Public Shared Function ofInstant(  localDateTime_Renamed As LocalDateTime,   offset As ZoneOffset,   zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(localDateTime_Renamed, "localDateTime")
			java.util.Objects.requireNonNull(offset, "offset")
			java.util.Objects.requireNonNull(zone, "zone")
			If zone.rules.isValidOffset(localDateTime_Renamed, offset) Then Return New ZonedDateTime(localDateTime_Renamed, offset, zone)
			Return create(localDateTime_Renamed.toEpochSecond(offset), localDateTime_Renamed.nano, zone)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} using seconds from the
		''' epoch of 1970-01-01T00:00:00Z.
		''' </summary>
		''' <param name="epochSecond">  the number of seconds from the epoch of 1970-01-01T00:00:00Z </param>
		''' <param name="nanoOfSecond">  the nanosecond within the second, from 0 to 999,999,999 </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Private Shared Function create(  epochSecond As Long,   nanoOfSecond As Integer,   zone As ZoneId) As ZonedDateTime
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim instant_Renamed As Instant = Instant.ofEpochSecond(epochSecond, nanoOfSecond) ' TODO: rules should be queryable by epochSeconds
			Dim offset_Renamed As ZoneOffset = rules.getOffset(instant_Renamed)
			Dim ldt As LocalDateTime = LocalDateTime.ofEpochSecond(epochSecond, nanoOfSecond, offset_Renamed)
			Return New ZonedDateTime(ldt, offset_Renamed, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} strictly validating the
		''' combination of local date-time, offset and zone ID.
		''' <p>
		''' This creates a zoned date-time ensuring that the offset is valid for the
		''' local date-time according to the rules of the specified zone.
		''' If the offset is invalid, an exception is thrown.
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Public Shared Function ofStrict(  localDateTime_Renamed As LocalDateTime,   offset As ZoneOffset,   zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(localDateTime_Renamed, "localDateTime")
			java.util.Objects.requireNonNull(offset, "offset")
			java.util.Objects.requireNonNull(zone, "zone")
			Dim rules As java.time.zone.ZoneRules = zone.rules
			If rules.isValidOffset(localDateTime_Renamed, offset) = False Then
				Dim trans As java.time.zone.ZoneOffsetTransition = rules.getTransition(localDateTime_Renamed)
				If trans IsNot Nothing AndAlso trans.gap Then Throw New DateTimeException("LocalDateTime '" & localDateTime_Renamed & "' does not exist in zone '" & zone & "' due to a gap in the local time-line, typically caused by daylight savings")
				Throw New DateTimeException("ZoneOffset '" & offset & "' is not valid for LocalDateTime '" & localDateTime_Renamed & "' in zone '" & zone & "'")
			End If
			Return New ZonedDateTime(localDateTime_Renamed, offset, zone)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} leniently, for advanced use cases,
		''' allowing any combination of local date-time, offset and zone ID.
		''' <p>
		''' This creates a zoned date-time with no checks other than no nulls.
		''' This means that the resulting zoned date-time may have an offset that is in conflict
		''' with the zone ID.
		''' <p>
		''' This method is intended for advanced use cases.
		''' For example, consider the case where a zoned date-time with valid fields is created
		''' and then stored in a database or serialization-based store. At some later point,
		''' the object is then re-loaded. However, between those points in time, the government
		''' that defined the time-zone has changed the rules, such that the originally stored
		''' local date-time now does not occur. This method can be used to create the object
		''' in an "invalid" state, despite the change in rules.
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Private Shared Function ofLenient(  localDateTime_Renamed As LocalDateTime,   offset As ZoneOffset,   zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(localDateTime_Renamed, "localDateTime")
			java.util.Objects.requireNonNull(offset, "offset")
			java.util.Objects.requireNonNull(zone, "zone")
			If TypeOf zone Is ZoneOffset AndAlso offset.Equals(zone) = False Then Throw New IllegalArgumentException("ZoneId must match ZoneOffset")
			Return New ZonedDateTime(localDateTime_Renamed, offset, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a temporal object.
		''' <p>
		''' This obtains a zoned date-time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ZonedDateTime}.
		''' <p>
		''' The conversion will first obtain a {@code ZoneId} from the temporal object,
		''' falling back to a {@code ZoneOffset} if necessary. It will then try to obtain
		''' an {@code Instant}, falling back to a {@code LocalDateTime} if necessary.
		''' The result will be either the combination of {@code ZoneId} or {@code ZoneOffset}
		''' with {@code Instant} or {@code LocalDateTime}.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ZonedDateTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code ZonedDateTime} </exception>
		Public Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As ZonedDateTime
			If TypeOf temporal Is ZonedDateTime Then Return CType(temporal, ZonedDateTime)
			Try
				Dim zone_Renamed As ZoneId = ZoneId.from(temporal)
				If temporal.isSupported(INSTANT_SECONDS) Then
					Dim epochSecond As Long = temporal.getLong(INSTANT_SECONDS)
					Dim nanoOfSecond As Integer = temporal.get(NANO_OF_SECOND)
					Return create(epochSecond, nanoOfSecond, zone_Renamed)
				Else
					Dim date_Renamed As LocalDate = LocalDate.from(temporal)
					Dim time As LocalTime = LocalTime.from(temporal)
					Return [of](date_Renamed, time, zone_Renamed)
				End If
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain ZonedDateTime from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a text string such as
		''' {@code 2007-12-03T10:15:30+01:00[Europe/Paris]}.
		''' <p>
		''' The string must represent a valid date-time and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_ZONED_DATE_TIME"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007-12-03T10:15:30+01:00[Europe/Paris]", not null </param>
		''' <returns> the parsed zoned date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence) As ZonedDateTime
			Return parse(text, java.time.format.DateTimeFormatter.ISO_ZONED_DATE_TIME)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZonedDateTime} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a date-time.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed zoned date-time, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(  text As CharSequence,   formatter As java.time.format.DateTimeFormatter) As ZonedDateTime
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, ZonedDateTime::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="dateTime">  the date-time, validated as not null </param>
		''' <param name="offset">  the zone offset, validated as not null </param>
		''' <param name="zone">  the time-zone, validated as not null </param>
		Private Sub New(  dateTime As LocalDateTime,   offset As ZoneOffset,   zone As ZoneId)
			Me.dateTime = dateTime
			Me.offset = offset
			Me.zone = zone
		End Sub

		''' <summary>
		''' Resolves the new local date-time using this zone ID, retaining the offset if possible.
		''' </summary>
		''' <param name="newDateTime">  the new local date-time, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Private Function resolveLocal(  newDateTime As LocalDateTime) As ZonedDateTime
			Return ofLocal(newDateTime, zone, offset)
		End Function

		''' <summary>
		''' Resolves the new local date-time using the offset to identify the instant.
		''' </summary>
		''' <param name="newDateTime">  the new local date-time, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Private Function resolveInstant(  newDateTime As LocalDateTime) As ZonedDateTime
			Return ofInstant(newDateTime, offset, zone)
		End Function

		''' <summary>
		''' Resolves the offset into this zoned date-time for the with methods.
		''' <p>
		''' This typically ignores the offset, unless it can be used to switch offset in a DST overlap.
		''' </summary>
		''' <param name="offset">  the offset, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		Private Function resolveOffset(  offset As ZoneOffset) As ZonedDateTime
			If offset.Equals(Me.offset) = False AndAlso zone.rules.isValidOffset(dateTime, offset) Then Return New ZonedDateTime(dateTime, offset, zone)
			Return Me
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
		Public Overrides Function [get](  field As java.time.temporal.TemporalField) As Integer ' override for Javadoc and performance
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
		Public  Overrides ReadOnly Property  offset As ZoneOffset
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this date-time changing the zone offset to the
		''' earlier of the two valid offsets at a local time-line overlap.
		''' <p>
		''' This method only has any effect when the local time-line overlaps, such as
		''' at an autumn daylight savings cutover. In this scenario, there are two
		''' valid offsets for the local date-time. Calling this method will return
		''' a zoned date-time with the earlier of the two selected.
		''' <p>
		''' If this method is called when it is not an overlap, {@code this}
		''' is returned.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the earlier offset, not null </returns>
		Public Overrides Function withEarlierOffsetAtOverlap() As ZonedDateTime
			Dim trans As java.time.zone.ZoneOffsetTransition = zone.rules.getTransition(dateTime)
			If trans IsNot Nothing AndAlso trans.overlap Then
				Dim earlierOffset As ZoneOffset = trans.offsetBefore
				If earlierOffset.Equals(offset) = False Then Return New ZonedDateTime(dateTime, earlierOffset, zone)
			End If
			Return Me
		End Function

		''' <summary>
		''' Returns a copy of this date-time changing the zone offset to the
		''' later of the two valid offsets at a local time-line overlap.
		''' <p>
		''' This method only has any effect when the local time-line overlaps, such as
		''' at an autumn daylight savings cutover. In this scenario, there are two
		''' valid offsets for the local date-time. Calling this method will return
		''' a zoned date-time with the later of the two selected.
		''' <p>
		''' If this method is called when it is not an overlap, {@code this}
		''' is returned.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the later offset, not null </returns>
		Public Overrides Function withLaterOffsetAtOverlap() As ZonedDateTime
			Dim trans As java.time.zone.ZoneOffsetTransition = zone.rules.getTransition(toLocalDateTime())
			If trans IsNot Nothing Then
				Dim laterOffset As ZoneOffset = trans.offsetAfter
				If laterOffset.Equals(offset) = False Then Return New ZonedDateTime(dateTime, laterOffset, zone)
			End If
			Return Me
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the time-zone, such as 'Europe/Paris'.
		''' <p>
		''' This returns the zone ID. This identifies the time-zone <seealso cref="ZoneRules rules"/>
		''' that determine when and how the offset from UTC/Greenwich changes.
		''' <p>
		''' The zone ID may be same as the <seealso cref="#getOffset() offset"/>.
		''' If this is true, then any future calculations, such as addition or subtraction,
		''' have no complex edge cases due to time-zone rules.
		''' See also <seealso cref="#withFixedOffsetZone()"/>.
		''' </summary>
		''' <returns> the time-zone, not null </returns>
		Public  Overrides ReadOnly Property  zone As ZoneId
			Get
				Return zone
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this date-time with a different time-zone,
		''' retaining the local date-time if possible.
		''' <p>
		''' This method changes the time-zone and retains the local date-time.
		''' The local date-time is only changed if it is invalid for the new zone,
		''' determined using the same approach as
		''' <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>.
		''' <p>
		''' To change the zone and adjust the local date-time,
		''' use <seealso cref="#withZoneSameInstant(ZoneId)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="zone">  the time-zone to change to, not null </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested zone, not null </returns>
		Public Overrides Function withZoneSameLocal(  zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(zone, "zone")
			Return If(Me.zone.Equals(zone), Me, ofLocal(dateTime, zone, offset))
		End Function

		''' <summary>
		''' Returns a copy of this date-time with a different time-zone,
		''' retaining the instant.
		''' <p>
		''' This method changes the time-zone and retains the instant.
		''' This normally results in a change to the local date-time.
		''' <p>
		''' This method is based on retaining the same instant, thus gaps and overlaps
		''' in the local time-line have no effect on the result.
		''' <p>
		''' To change the offset while keeping the local time,
		''' use <seealso cref="#withZoneSameLocal(ZoneId)"/>.
		''' </summary>
		''' <param name="zone">  the time-zone to change to, not null </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested zone, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Overrides Function withZoneSameInstant(  zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(zone, "zone")
			Return If(Me.zone.Equals(zone), Me, create(dateTime.toEpochSecond(offset), dateTime.nano, zone))
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the zone ID set to the offset.
		''' <p>
		''' This returns a zoned date-time where the zone ID is the same as <seealso cref="#getOffset()"/>.
		''' The local date-time, offset and instant of the result will be the same as in this date-time.
		''' <p>
		''' Setting the date-time to a fixed single offset means that any future
		''' calculations, such as addition or subtraction, have no complex edge cases
		''' due to time-zone rules.
		''' This might also be useful when sending a zoned date-time across a network,
		''' as most protocols, such as ISO-8601, only handle offsets,
		''' and not region-based zone IDs.
		''' <p>
		''' This is equivalent to {@code ZonedDateTime.of(zdt.toLocalDateTime(), zdt.getOffset())}.
		''' </summary>
		''' <returns> a {@code ZonedDateTime} with the zone ID set to the offset, not null </returns>
		Public Function withFixedOffsetZone() As ZonedDateTime
			Return If(Me.zone.Equals(offset), Me, New ZonedDateTime(dateTime, offset, offset))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the {@code LocalDateTime} part of this date-time.
		''' <p>
		''' This returns a {@code LocalDateTime} with the same year, month, day and time
		''' as this date-time.
		''' </summary>
		''' <returns> the local date-time part of this date-time, not null </returns>
		Public Overrides Function toLocalDateTime() As LocalDateTime ' override for return type
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
		Public Overrides Function toLocalDate() As LocalDate ' override for return type
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
		Public Overrides Function toLocalTime() As LocalTime ' override for Javadoc and performance
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
		''' This returns a {@code ZonedDateTime}, based on this one, with the date-time adjusted.
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
		'''  result = zonedDateTime.with(JULY).with(lastDayOfMonth());
		''' </pre>
		''' <p>
		''' The classes <seealso cref="LocalDate"/> and <seealso cref="LocalTime"/> implement {@code TemporalAdjuster},
		''' thus this method can be used to change the date, time or offset:
		''' <pre>
		'''  result = zonedDateTime.with(date);
		'''  result = zonedDateTime.with(time);
		''' </pre>
		''' <p>
		''' <seealso cref="ZoneOffset"/> also implements {@code TemporalAdjuster} however using it
		''' as an argument typically has no effect. The offset of a {@code ZonedDateTime} is
		''' controlled primarily by the time-zone. As such, changing the offset does not generally
		''' make sense, because there is only one valid offset for the local date-time and zone.
		''' If the zoned date-time is in a daylight savings overlap, then the offset is used
		''' to switch between the two valid offsets. In all other cases, the offset is ignored.
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> a {@code ZonedDateTime} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](  adjuster As java.time.temporal.TemporalAdjuster) As ZonedDateTime
			' optimizations
			If TypeOf adjuster Is LocalDate Then
				Return resolveLocal(LocalDateTime.of(CType(adjuster, LocalDate), dateTime.toLocalTime()))
			ElseIf TypeOf adjuster Is LocalTime Then
				Return resolveLocal(LocalDateTime.of(dateTime.toLocalDate(), CType(adjuster, LocalTime)))
			ElseIf TypeOf adjuster Is LocalDateTime Then
				Return resolveLocal(CType(adjuster, LocalDateTime))
			ElseIf TypeOf adjuster Is OffsetDateTime Then
				Dim odt As OffsetDateTime = CType(adjuster, OffsetDateTime)
				Return ofLocal(odt.toLocalDateTime(), zone, odt.offset)
			ElseIf TypeOf adjuster Is Instant Then
				Dim instant_Renamed As Instant = CType(adjuster, Instant)
				Return create(instant_Renamed.epochSecond, instant_Renamed.nano, zone)
			ElseIf TypeOf adjuster Is ZoneOffset Then
				Return resolveOffset(CType(adjuster, ZoneOffset))
			End If
			Return CType(adjuster.adjustInto(Me), ZonedDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified field set to a new value.
		''' <p>
		''' This returns a {@code ZonedDateTime}, based on this one, with the value
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
		''' The zone and nano-of-second are unchanged.
		''' The result will have an offset derived from the new instant and original zone.
		''' If the new instant value is outside the valid range then a {@code DateTimeException} will be thrown.
		''' <p>
		''' The {@code OFFSET_SECONDS} field will typically be ignored.
		''' The offset of a {@code ZonedDateTime} is controlled primarily by the time-zone.
		''' As such, changing the offset does not generally make sense, because there is only
		''' one valid offset for the local date-time and zone.
		''' If the zoned date-time is in a daylight savings overlap, then the offset is used
		''' to switch between the two valid offsets. In all other cases, the offset is ignored.
		''' If the new offset value is outside the valid range then a {@code DateTimeException} will be thrown.
		''' <p>
		''' The other <seealso cref="#isSupported(TemporalField) supported fields"/> will behave as per
		''' the matching method on <seealso cref="LocalDateTime#with(TemporalField, long) LocalDateTime"/>.
		''' The zone is not part of the calculation and will be unchanged.
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
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
		''' <returns> a {@code ZonedDateTime} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](  field As java.time.temporal.TemporalField,   newValue As Long) As ZonedDateTime
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Select Case f
					Case INSTANT_SECONDS
						Return create(newValue, nano, zone)
					Case OFFSET_SECONDS
						Dim offset_Renamed As ZoneOffset = ZoneOffset.ofTotalSeconds(f.checkValidIntValue(newValue))
						Return resolveOffset(offset_Renamed)
				End Select
				Return resolveLocal(dateTime.with(field, newValue))
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the year altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withYear(int) changing the year"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested year, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Function withYear(  year_Renamed As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withYear(year_Renamed))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the month-of-year altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withMonth(int) changing the month"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(  month As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withMonth(month))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the day-of-month altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withDayOfMonth(int) changing the day-of-month"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month value is invalid,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Function withDayOfMonth(  dayOfMonth As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withDayOfMonth(dayOfMonth))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the day-of-year altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withDayOfYear(int) changing the day-of-year"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		''' <returns> a {@code ZonedDateTime} based on this date with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-year value is invalid,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Function withDayOfYear(  dayOfYear As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withDayOfYear(dayOfYear))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the hour-of-day altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withHour(int) changing the time"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hour">  the hour-of-day to set in the result, from 0 to 23 </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested hour, not null </returns>
		''' <exception cref="DateTimeException"> if the hour value is invalid </exception>
		Public Function withHour(  hour As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withHour(hour))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the minute-of-hour altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withMinute(int) changing the time"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minute">  the minute-of-hour to set in the result, from 0 to 59 </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested minute, not null </returns>
		''' <exception cref="DateTimeException"> if the minute value is invalid </exception>
		Public Function withMinute(  minute As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withMinute(minute))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the second-of-minute altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withSecond(int) changing the time"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="second">  the second-of-minute to set in the result, from 0 to 59 </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested second, not null </returns>
		''' <exception cref="DateTimeException"> if the second value is invalid </exception>
		Public Function withSecond(  second As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withSecond(second))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the nano-of-second altered.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#withNano(int) changing the time"/> of the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to set in the result, from 0 to 999,999,999 </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the requested nanosecond, not null </returns>
		''' <exception cref="DateTimeException"> if the nano value is invalid </exception>
		Public Function withNano(  nanoOfSecond As Integer) As ZonedDateTime
			Return resolveLocal(dateTime.withNano(nanoOfSecond))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the time truncated.
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
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#truncatedTo(TemporalUnit) truncating"/>
		''' the underlying local date-time. This is then converted back to a
		''' {@code ZonedDateTime}, using the zone ID to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="unit">  the unit to truncate to, not null </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if unable to truncate </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(  unit As java.time.temporal.TemporalUnit) As ZonedDateTime
			Return resolveLocal(dateTime.truncatedTo(unit))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns a {@code ZonedDateTime}, based on this one, with the specified amount added.
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
		''' <returns> a {@code ZonedDateTime} based on this date-time with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(  amountToAdd As java.time.temporal.TemporalAmount) As ZonedDateTime
			If TypeOf amountToAdd Is Period Then
				Dim periodToAdd As Period = CType(amountToAdd, Period)
				Return resolveLocal(dateTime.plus(periodToAdd))
			End If
			java.util.Objects.requireNonNull(amountToAdd, "amountToAdd")
			Return CType(amountToAdd.addTo(Me), ZonedDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount added.
		''' <p>
		''' This returns a {@code ZonedDateTime}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The zone is not part of the calculation and will be unchanged in the result.
		''' The calculation for date and time units differ.
		''' <p>
		''' Date units operate on the local time-line.
		''' The period is first added to the local date-time, then converted back
		''' to a zoned date-time using the zone ID.
		''' The conversion uses <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		''' with the offset before the addition.
		''' <p>
		''' Time units operate on the instant time-line.
		''' The period is first added to the local date-time, then converted back to
		''' a zoned date-time using the zone ID.
		''' The conversion uses <seealso cref="#ofInstant(LocalDateTime, ZoneOffset, ZoneId)"/>
		''' with the offset before the addition.
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
		''' <returns> a {@code ZonedDateTime} based on this date-time with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As ZonedDateTime
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				If unit.dateBased Then
					Return resolveLocal(dateTime.plus(amountToAdd, unit))
				Else
					Return resolveInstant(dateTime.plus(amountToAdd, unit))
				End If
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of years added.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#plusYears(long) adding years"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="years">  the years to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusYears(  years As Long) As ZonedDateTime
			Return resolveLocal(dateTime.plusYears(years))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of months added.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#plusMonths(long) adding months"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="months">  the months to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMonths(  months As Long) As ZonedDateTime
			Return resolveLocal(dateTime.plusMonths(months))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of weeks added.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#plusWeeks(long) adding weeks"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeks">  the weeks to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the weeks added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusWeeks(  weeks As Long) As ZonedDateTime
			Return resolveLocal(dateTime.plusWeeks(weeks))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of days added.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#plusDays(long) adding days"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the days added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusDays(  days As Long) As ZonedDateTime
			Return resolveLocal(dateTime.plusDays(days))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of hours added.
		''' <p>
		''' This operates on the instant time-line, such that adding one hour will
		''' always be a duration of one hour later.
		''' This may cause the local date-time to change by an amount other than one hour.
		''' Note that this is a different approach to that used by days, months and years,
		''' thus adding one day is not the same as adding 24 hours.
		''' <p>
		''' For example, consider a time-zone where the spring DST cutover means that the
		''' local times 01:00 to 01:59 occur twice changing from offset +02:00 to +01:00.
		''' <ul>
		''' <li>Adding one hour to 00:30+02:00 will result in 01:30+02:00
		''' <li>Adding one hour to 01:30+02:00 will result in 01:30+01:00
		''' <li>Adding one hour to 01:30+01:00 will result in 02:30+01:00
		''' <li>Adding three hours to 00:30+02:00 will result in 02:30+01:00
		''' </ul>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the hours added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusHours(  hours As Long) As ZonedDateTime
			Return resolveInstant(dateTime.plusHours(hours))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of minutes added.
		''' <p>
		''' This operates on the instant time-line, such that adding one minute will
		''' always be a duration of one minute later.
		''' This may cause the local date-time to change by an amount other than one minute.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the minutes added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMinutes(  minutes As Long) As ZonedDateTime
			Return resolveInstant(dateTime.plusMinutes(minutes))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of seconds added.
		''' <p>
		''' This operates on the instant time-line, such that adding one second will
		''' always be a duration of one second later.
		''' This may cause the local date-time to change by an amount other than one second.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the seconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusSeconds(  seconds As Long) As ZonedDateTime
			Return resolveInstant(dateTime.plusSeconds(seconds))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of nanoseconds added.
		''' <p>
		''' This operates on the instant time-line, such that adding one nano will
		''' always be a duration of one nano later.
		''' This may cause the local date-time to change by an amount other than one nano.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to add, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the nanoseconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusNanos(  nanos As Long) As ZonedDateTime
			Return resolveInstant(dateTime.plusNanos(nanos))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code ZonedDateTime}, based on this one, with the specified amount subtracted.
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
		''' <returns> a {@code ZonedDateTime} based on this date-time with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(  amountToSubtract As java.time.temporal.TemporalAmount) As ZonedDateTime
			If TypeOf amountToSubtract Is Period Then
				Dim periodToSubtract As Period = CType(amountToSubtract, Period)
				Return resolveLocal(dateTime.minus(periodToSubtract))
			End If
			java.util.Objects.requireNonNull(amountToSubtract, "amountToSubtract")
			Return CType(amountToSubtract.subtractFrom(Me), ZonedDateTime)
		End Function

		''' <summary>
		''' Returns a copy of this date-time with the specified amount subtracted.
		''' <p>
		''' This returns a {@code ZonedDateTime}, based on this one, with the amount
		''' in terms of the unit subtracted. If it is not possible to subtract the amount,
		''' because the unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' The calculation for date and time units differ.
		''' <p>
		''' Date units operate on the local time-line.
		''' The period is first subtracted from the local date-time, then converted back
		''' to a zoned date-time using the zone ID.
		''' The conversion uses <seealso cref="#ofLocal(LocalDateTime, ZoneId, ZoneOffset)"/>
		''' with the offset before the subtraction.
		''' <p>
		''' Time units operate on the instant time-line.
		''' The period is first subtracted from the local date-time, then converted back to
		''' a zoned date-time using the zone ID.
		''' The conversion uses <seealso cref="#ofInstant(LocalDateTime, ZoneOffset, ZoneId)"/>
		''' with the offset before the subtraction.
		''' <p>
		''' This method is equivalent to <seealso cref="#plus(long, TemporalUnit)"/> with the amount negated.
		''' See that method for a full description of how addition, and thus subtraction, works.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToSubtract">  the amount of the unit to subtract from the result, may be negative </param>
		''' <param name="unit">  the unit of the amount to subtract, not null </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(  amountToSubtract As Long,   unit As java.time.temporal.TemporalUnit) As ZonedDateTime
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of years subtracted.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#minusYears(long) subtracting years"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="years">  the years to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusYears(  years As Long) As ZonedDateTime
			Return (If(years = java.lang.[Long].MIN_VALUE, plusYears(Long.Max_Value).plusYears(1), plusYears(-years)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of months subtracted.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#minusMonths(long) subtracting months"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="months">  the months to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMonths(  months As Long) As ZonedDateTime
			Return (If(months = java.lang.[Long].MIN_VALUE, plusMonths(Long.Max_Value).plusMonths(1), plusMonths(-months)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of weeks subtracted.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#minusWeeks(long) subtracting weeks"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeks">  the weeks to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the weeks subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusWeeks(  weeks As Long) As ZonedDateTime
			Return (If(weeks = java.lang.[Long].MIN_VALUE, plusWeeks(Long.Max_Value).plusWeeks(1), plusWeeks(-weeks)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of days subtracted.
		''' <p>
		''' This operates on the local time-line,
		''' <seealso cref="LocalDateTime#minusDays(long) subtracting days"/> to the local date-time.
		''' This is then converted back to a {@code ZonedDateTime}, using the zone ID
		''' to obtain the offset.
		''' <p>
		''' When converting back to {@code ZonedDateTime}, if the local date-time is in an overlap,
		''' then the offset will be retained if possible, otherwise the earlier offset will be used.
		''' If in a gap, the local date-time will be adjusted forward by the length of the gap.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the days subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusDays(  days As Long) As ZonedDateTime
			Return (If(days = java.lang.[Long].MIN_VALUE, plusDays(Long.Max_Value).plusDays(1), plusDays(-days)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of hours subtracted.
		''' <p>
		''' This operates on the instant time-line, such that subtracting one hour will
		''' always be a duration of one hour earlier.
		''' This may cause the local date-time to change by an amount other than one hour.
		''' Note that this is a different approach to that used by days, months and years,
		''' thus subtracting one day is not the same as adding 24 hours.
		''' <p>
		''' For example, consider a time-zone where the spring DST cutover means that the
		''' local times 01:00 to 01:59 occur twice changing from offset +02:00 to +01:00.
		''' <ul>
		''' <li>Subtracting one hour from 02:30+01:00 will result in 01:30+02:00
		''' <li>Subtracting one hour from 01:30+01:00 will result in 01:30+02:00
		''' <li>Subtracting one hour from 01:30+02:00 will result in 00:30+01:00
		''' <li>Subtracting three hours from 02:30+01:00 will result in 00:30+02:00
		''' </ul>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hours">  the hours to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the hours subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusHours(  hours As Long) As ZonedDateTime
			Return (If(hours = java.lang.[Long].MIN_VALUE, plusHours(Long.Max_Value).plusHours(1), plusHours(-hours)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of minutes subtracted.
		''' <p>
		''' This operates on the instant time-line, such that subtracting one minute will
		''' always be a duration of one minute earlier.
		''' This may cause the local date-time to change by an amount other than one minute.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutes">  the minutes to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the minutes subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMinutes(  minutes As Long) As ZonedDateTime
			Return (If(minutes = java.lang.[Long].MIN_VALUE, plusMinutes(Long.Max_Value).plusMinutes(1), plusMinutes(-minutes)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of seconds subtracted.
		''' <p>
		''' This operates on the instant time-line, such that subtracting one second will
		''' always be a duration of one second earlier.
		''' This may cause the local date-time to change by an amount other than one second.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the seconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusSeconds(  seconds As Long) As ZonedDateTime
			Return (If(seconds = java.lang.[Long].MIN_VALUE, plusSeconds(Long.Max_Value).plusSeconds(1), plusSeconds(-seconds)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code ZonedDateTime} with the specified number of nanoseconds subtracted.
		''' <p>
		''' This operates on the instant time-line, such that subtracting one nano will
		''' always be a duration of one nano earlier.
		''' This may cause the local date-time to change by an amount other than one nano.
		''' Note that this is a different approach to that used by days, months and years.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanos">  the nanos to subtract, may be negative </param>
		''' <returns> a {@code ZonedDateTime} based on this date-time with the nanoseconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusNanos(  nanos As Long) As ZonedDateTime
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
		Public Overrides Function query(Of R)(  query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R ' override for Javadoc
			If query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then Return CType(toLocalDate(), R)
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Calculates the amount of time until another date-time in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code ZonedDateTime}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified date-time.
		''' The result will be negative if the end is before the start.
		''' For example, the amount in days between two date-times can be calculated
		''' using {@code startDateTime.until(endDateTime, DAYS)}.
		''' <p>
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code ZonedDateTime} using <seealso cref="#from(TemporalAccessor)"/>.
		''' If the time-zone differs between the two zoned date-times, the specified
		''' end date-time is normalized to have the same zone as this date-time.
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
		''' The calculation for date and time units differ.
		''' <p>
		''' Date units operate on the local time-line, using the local date-time.
		''' For example, the period from noon on day 1 to noon the following day
		''' in days will always be counted as exactly one day, irrespective of whether
		''' there was a daylight savings change or not.
		''' <p>
		''' Time units operate on the instant time-line.
		''' The calculation effectively converts both zoned date-times to instants
		''' and then calculates the period between the instants.
		''' For example, the period from noon on day 1 to noon the following day
		''' in hours may be 23, 24 or 25 hours (or some other amount) depending on
		''' whether there was a daylight savings change or not.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to a {@code ZonedDateTime}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this date-time and the end date-time </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code ZonedDateTime} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](  endExclusive As java.time.temporal.Temporal,   unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As ZonedDateTime = ZonedDateTime.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				[end] = [end].withZoneSameInstant(zone)
				If unit.dateBased Then
					Return dateTime.until([end].dateTime, unit)
				Else
					Return toOffsetDateTime().until([end].toOffsetDateTime(), unit)
				End If
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
		Public Overrides Function format(  formatter As java.time.format.DateTimeFormatter) As String ' override for Javadoc and performance
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this date-time to an {@code OffsetDateTime}.
		''' <p>
		''' This creates an offset date-time using the local date-time and offset.
		''' The zone ID is ignored.
		''' </summary>
		''' <returns> an offset date-time representing the same local date-time and offset, not null </returns>
		Public Function toOffsetDateTime() As OffsetDateTime
			Return OffsetDateTime.of(dateTime, offset)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this date-time is equal to another date-time.
		''' <p>
		''' The comparison is based on the offset date-time and the zone.
		''' Only objects of type {@code ZonedDateTime} are compared, other types return false.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date-time </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ZonedDateTime Then
				Dim other As ZonedDateTime = CType(obj, ZonedDateTime)
				Return dateTime.Equals(other.dateTime) AndAlso offset.Equals(other.offset) AndAlso zone.Equals(other.zone)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this date-time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return dateTime.GetHashCode() Xor offset.GetHashCode() Xor  java.lang.[Integer].rotateLeft(zone.GetHashCode(), 3)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date-time as a {@code String}, such as
		''' {@code 2007-12-03T10:15:30+01:00[Europe/Paris]}.
		''' <p>
		''' The format consists of the {@code LocalDateTime} followed by the {@code ZoneOffset}.
		''' If the {@code ZoneId} is not the same as the offset, then the ID is output.
		''' The output is compatible with ISO-8601 if the offset and ID are the same.
		''' </summary>
		''' <returns> a string representation of this date-time, not null </returns>
		Public Overrides Function ToString() As String ' override for Javadoc
			Dim str As String = dateTime.ToString() & offset.ToString()
			If offset IsNot zone Then str += AscW("["c) + zone.ToString() & AscW("]"c)
			Return str
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(6);  // identifies a ZonedDateTime
		'''  // the <a href="../../serialized-form.html#java.time.LocalDateTime">dateTime</a> excluding the one byte header
		'''  // the <a href="../../serialized-form.html#java.time.ZoneOffset">offset</a> excluding the one byte header
		'''  // the <a href="../../serialized-form.html#java.time.ZoneId">zone ID</a> excluding the one byte header
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZONE_DATE_TIME_TYPE, Me)
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
			dateTime.writeExternal(out)
			offset.writeExternal(out)
			zone.write(out)
		End Sub

		Shared Function readExternal(  [in] As java.io.ObjectInput) As ZonedDateTime
			Dim dateTime As LocalDateTime = LocalDateTime.readExternal([in])
			Dim offset_Renamed As ZoneOffset = ZoneOffset.readExternal([in])
			Dim zone_Renamed As ZoneId = CType(Ser.read([in]), ZoneId)
			Return ZonedDateTime.ofLenient(dateTime, offset_Renamed, zone_Renamed)
		End Function

	End Class

End Namespace