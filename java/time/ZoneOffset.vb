Imports System
Imports System.Collections.Concurrent

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
	''' A time-zone offset from Greenwich/UTC, such as {@code +02:00}.
	''' <p>
	''' A time-zone offset is the amount of time that a time-zone differs from Greenwich/UTC.
	''' This is usually a fixed number of hours and minutes.
	''' <p>
	''' Different parts of the world have different time-zone offsets.
	''' The rules for how offsets vary by place and time of year are captured in the
	''' <seealso cref="ZoneId"/> class.
	''' <p>
	''' For example, Paris is one hour ahead of Greenwich/UTC in winter and two hours
	''' ahead in summer. The {@code ZoneId} instance for Paris will reference two
	''' {@code ZoneOffset} instances - a {@code +01:00} instance for winter,
	''' and a {@code +02:00} instance for summer.
	''' <p>
	''' In 2008, time-zone offsets around the world extended from -12:00 to +14:00.
	''' To prevent any problems with that range being extended, yet still provide
	''' validation, the range of offsets is restricted to -18:00 to 18:00 inclusive.
	''' <p>
	''' This class is designed for use with the ISO calendar system.
	''' The fields of hours, minutes and seconds make assumptions that are valid for the
	''' standard ISO definitions of those fields. This class may be used with other
	''' calendar systems providing the definition of the time fields matches those
	''' of the ISO calendar system.
	''' <p>
	''' Instances of {@code ZoneOffset} must be compared using <seealso cref="#equals"/>.
	''' Implementations may choose to cache certain common offsets, however
	''' applications must not rely on such caching.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code ZoneOffset} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ZoneOffset
		Inherits ZoneId
		Implements java.time.temporal.TemporalAccessor, java.time.temporal.TemporalAdjuster, Comparable(Of ZoneOffset)

		''' <summary>
		''' Cache of time-zone offset by offset in seconds. </summary>
		Private Shared ReadOnly SECONDS_CACHE As java.util.concurrent.ConcurrentMap(Of Integer?, ZoneOffset) = New ConcurrentDictionary(Of Integer?, ZoneOffset)(16, 0.75f, 4)
		''' <summary>
		''' Cache of time-zone offset by ID. </summary>
		Private Shared ReadOnly ID_CACHE As java.util.concurrent.ConcurrentMap(Of String, ZoneOffset) = New ConcurrentDictionary(Of String, ZoneOffset)(16, 0.75f, 4)

		''' <summary>
		''' The abs maximum seconds.
		''' </summary>
		Private Shared ReadOnly MAX_SECONDS As Integer = 18 * SECONDS_PER_HOUR
		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 2357656521762053153L

		''' <summary>
		''' The time-zone offset for UTC, with an ID of 'Z'.
		''' </summary>
		Public Shared ReadOnly UTC As ZoneOffset = ZoneOffset.ofTotalSeconds(0)
		''' <summary>
		''' Constant for the maximum supported offset.
		''' </summary>
		Public Shared ReadOnly MIN As ZoneOffset = ZoneOffset.ofTotalSeconds(-MAX_SECONDS)
		''' <summary>
		''' Constant for the maximum supported offset.
		''' </summary>
		Public Shared ReadOnly MAX As ZoneOffset = ZoneOffset.ofTotalSeconds(MAX_SECONDS)

		''' <summary>
		''' The total offset in seconds.
		''' </summary>
		Private ReadOnly totalSeconds_Renamed As Integer
		''' <summary>
		''' The string form of the time-zone offset.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly id As String

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} using the ID.
		''' <p>
		''' This method parses the string ID of a {@code ZoneOffset} to
		''' return an instance. The parsing accepts all the formats generated by
		''' <seealso cref="#getId()"/>, plus some additional formats:
		''' <ul>
		''' <li>{@code Z} - for UTC
		''' <li>{@code +h}
		''' <li>{@code +hh}
		''' <li>{@code +hh:mm}
		''' <li>{@code -hh:mm}
		''' <li>{@code +hhmm}
		''' <li>{@code -hhmm}
		''' <li>{@code +hh:mm:ss}
		''' <li>{@code -hh:mm:ss}
		''' <li>{@code +hhmmss}
		''' <li>{@code -hhmmss}
		''' </ul>
		''' Note that &plusmn; means either the plus or minus symbol.
		''' <p>
		''' The ID of the returned offset will be normalized to one of the formats
		''' described by <seealso cref="#getId()"/>.
		''' <p>
		''' The maximum supported range is from +18:00 to -18:00 inclusive.
		''' </summary>
		''' <param name="offsetId">  the offset ID, not null </param>
		''' <returns> the zone-offset, not null </returns>
		''' <exception cref="DateTimeException"> if the offset ID is invalid </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function [of](ByVal offsetId As String) As ZoneOffset
			java.util.Objects.requireNonNull(offsetId, "offsetId")
			' "Z" is always in the cache
			Dim offset As ZoneOffset = ID_CACHE.get(offsetId)
			If offset IsNot Nothing Then Return offset

			' parse - +h, +hh, +hhmm, +hh:mm, +hhmmss, +hh:mm:ss
			Dim hours, minutes, seconds As Integer
			Select Case offsetId.length()
				Case 2
					offsetId = AscW(offsetId.Chars(0)) & "0" & AscW(offsetId.Chars(1)) ' fallthru
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 3
					hours = parseNumber(offsetId, 1, False)
					minutes = 0
					seconds = 0
				Case 5
					hours = parseNumber(offsetId, 1, False)
					minutes = parseNumber(offsetId, 3, False)
					seconds = 0
				Case 6
					hours = parseNumber(offsetId, 1, False)
					minutes = parseNumber(offsetId, 4, True)
					seconds = 0
				Case 7
					hours = parseNumber(offsetId, 1, False)
					minutes = parseNumber(offsetId, 3, False)
					seconds = parseNumber(offsetId, 5, False)
				Case 9
					hours = parseNumber(offsetId, 1, False)
					minutes = parseNumber(offsetId, 4, True)
					seconds = parseNumber(offsetId, 7, True)
				Case Else
					Throw New DateTimeException("Invalid ID for ZoneOffset, invalid format: " & offsetId)
			End Select
			Dim first As Char = offsetId.Chars(0)
			If first <> "+"c AndAlso first <> "-"c Then Throw New DateTimeException("Invalid ID for ZoneOffset, plus/minus not found when expected: " & offsetId)
			If first = "-"c Then
				Return ofHoursMinutesSeconds(-hours, -minutes, -seconds)
			Else
				Return ofHoursMinutesSeconds(hours, minutes, seconds)
			End If
		End Function

		''' <summary>
		''' Parse a two digit zero-prefixed number.
		''' </summary>
		''' <param name="offsetId">  the offset ID, not null </param>
		''' <param name="pos">  the position to parse, valid </param>
		''' <param name="precededByColon">  should this number be prefixed by a precededByColon </param>
		''' <returns> the parsed number, from 0 to 99 </returns>
		Private Shared Function parseNumber(ByVal offsetId As CharSequence, ByVal pos As Integer, ByVal precededByColon As Boolean) As Integer
			If precededByColon AndAlso offsetId.Chars(pos - 1) IsNot ":"c Then Throw New DateTimeException("Invalid ID for ZoneOffset, colon not found when expected: " & offsetId)
			Dim ch1 As Char = offsetId.Chars(pos)
			Dim ch2 As Char = offsetId.Chars(pos + 1)
			If ch1 < "0"c OrElse ch1 > "9"c OrElse ch2 < "0"c OrElse ch2 > "9"c Then Throw New DateTimeException("Invalid ID for ZoneOffset, non numeric characters found: " & offsetId)
			Return (AscW(ch1) - 48) * 10 + (AscW(ch2) - 48)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} using an offset in hours.
		''' </summary>
		''' <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		''' <returns> the zone-offset, not null </returns>
		''' <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		Public Shared Function ofHours(ByVal hours As Integer) As ZoneOffset
			Return ofHoursMinutesSeconds(hours, 0, 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} using an offset in
		''' hours and minutes.
		''' <p>
		''' The sign of the hours and minutes components must match.
		''' Thus, if the hours is negative, the minutes must be negative or zero.
		''' If the hours is zero, the minutes may be positive, negative or zero.
		''' </summary>
		''' <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		''' <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours </param>
		''' <returns> the zone-offset, not null </returns>
		''' <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		Public Shared Function ofHoursMinutes(ByVal hours As Integer, ByVal minutes As Integer) As ZoneOffset
			Return ofHoursMinutesSeconds(hours, minutes, 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} using an offset in
		''' hours, minutes and seconds.
		''' <p>
		''' The sign of the hours, minutes and seconds components must match.
		''' Thus, if the hours is negative, the minutes and seconds must be negative or zero.
		''' </summary>
		''' <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		''' <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours and seconds </param>
		''' <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59, sign matches hours and minutes </param>
		''' <returns> the zone-offset, not null </returns>
		''' <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		Public Shared Function ofHoursMinutesSeconds(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer) As ZoneOffset
			validate(hours, minutes, seconds)
			Dim totalSeconds_Renamed As Integer = totalSeconds(hours, minutes, seconds)
			Return ofTotalSeconds(totalSeconds_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} from a temporal object.
		''' <p>
		''' This obtains an offset based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ZoneOffset}.
		''' <p>
		''' A {@code TemporalAccessor} represents some form of date and time information.
		''' This factory converts the arbitrary temporal object to an instance of {@code ZoneOffset}.
		''' <p>
		''' The conversion uses the <seealso cref="TemporalQueries#offset()"/> query, which relies
		''' on extracting the <seealso cref="ChronoField#OFFSET_SECONDS OFFSET_SECONDS"/> field.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ZoneOffset::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the zone-offset, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code ZoneOffset} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As ZoneOffset
			java.util.Objects.requireNonNull(temporal, "temporal")
			Dim offset As ZoneOffset = temporal.query(java.time.temporal.TemporalQueries.offset())
			If offset Is Nothing Then Throw New DateTimeException("Unable to obtain ZoneOffset from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name)
			Return offset
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Validates the offset fields.
		''' </summary>
		''' <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		''' <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59 </param>
		''' <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59 </param>
		''' <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		Private Shared Sub validate(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer)
			If hours < -18 OrElse hours > 18 Then Throw New DateTimeException("Zone offset hours not in valid range: value " & hours & " is not in the range -18 to 18")
			If hours > 0 Then
				If minutes < 0 OrElse seconds < 0 Then Throw New DateTimeException("Zone offset minutes and seconds must be positive because hours is positive")
			ElseIf hours < 0 Then
				If minutes > 0 OrElse seconds > 0 Then Throw New DateTimeException("Zone offset minutes and seconds must be negative because hours is negative")
			ElseIf (minutes > 0 AndAlso seconds < 0) OrElse (minutes < 0 AndAlso seconds > 0) Then
				Throw New DateTimeException("Zone offset minutes and seconds must have the same sign")
			End If
			If Math.Abs(minutes) > 59 Then Throw New DateTimeException("Zone offset minutes not in valid range: abs(value) " & Math.Abs(minutes) & " is not in the range 0 to 59")
			If Math.Abs(seconds) > 59 Then Throw New DateTimeException("Zone offset seconds not in valid range: abs(value) " & Math.Abs(seconds) & " is not in the range 0 to 59")
			If Math.Abs(hours) = 18 AndAlso (Math.Abs(minutes) > 0 OrElse Math.Abs(seconds) > 0) Then Throw New DateTimeException("Zone offset not in valid range: -18:00 to +18:00")
		End Sub

		''' <summary>
		''' Calculates the total offset in seconds.
		''' </summary>
		''' <param name="hours">  the time-zone offset in hours, from -18 to +18 </param>
		''' <param name="minutes">  the time-zone offset in minutes, from 0 to &plusmn;59, sign matches hours and seconds </param>
		''' <param name="seconds">  the time-zone offset in seconds, from 0 to &plusmn;59, sign matches hours and minutes </param>
		''' <returns> the total in seconds </returns>
		Private Shared Function totalSeconds(ByVal hours As Integer, ByVal minutes As Integer, ByVal seconds As Integer) As Integer
			Return hours * SECONDS_PER_HOUR + minutes * SECONDS_PER_MINUTE + seconds
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ZoneOffset} specifying the total offset in seconds
		''' <p>
		''' The offset must be in the range {@code -18:00} to {@code +18:00}, which corresponds to -64800 to +64800.
		''' </summary>
		''' <param name="totalSeconds">  the total time-zone offset in seconds, from -64800 to +64800 </param>
		''' <returns> the ZoneOffset, not null </returns>
		''' <exception cref="DateTimeException"> if the offset is not in the required range </exception>
		Public Shared Function ofTotalSeconds(ByVal totalSeconds As Integer) As ZoneOffset
			If Math.Abs(totalSeconds) > MAX_SECONDS Then Throw New DateTimeException("Zone offset not in valid range: -18:00 to +18:00")
			If totalSeconds Mod (15 * SECONDS_PER_MINUTE) = 0 Then
				Dim totalSecs As Integer? = totalSeconds
				Dim result As ZoneOffset = SECONDS_CACHE.get(totalSecs)
				If result Is Nothing Then
					result = New ZoneOffset(totalSeconds)
					SECONDS_CACHE.putIfAbsent(totalSecs, result)
					result = SECONDS_CACHE.get(totalSecs)
					ID_CACHE.putIfAbsent(result.id, result)
				End If
				Return result
			Else
				Return New ZoneOffset(totalSeconds)
			End If
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="totalSeconds">  the total time-zone offset in seconds, from -64800 to +64800 </param>
		Private Sub New(ByVal totalSeconds As Integer)
			MyBase.New()
			Me.totalSeconds_Renamed = totalSeconds
			id = buildId(totalSeconds)
		End Sub

		Private Shared Function buildId(ByVal totalSeconds As Integer) As String
			If totalSeconds = 0 Then
				Return "Z"
			Else
				Dim absTotalSeconds As Integer = Math.Abs(totalSeconds)
				Dim buf As New StringBuilder
				Dim absHours As Integer = absTotalSeconds / SECONDS_PER_HOUR
				Dim absMinutes As Integer = (absTotalSeconds / SECONDS_PER_MINUTE) Mod MINUTES_PER_HOUR
				buf.append(If(totalSeconds < 0, "-", "+")).append(If(absHours < 10, "0", "")).append(absHours).append(If(absMinutes < 10, ":0", ":")).append(absMinutes)
				Dim absSeconds As Integer = absTotalSeconds Mod SECONDS_PER_MINUTE
				If absSeconds <> 0 Then buf.append(If(absSeconds < 10, ":0", ":")).append(absSeconds)
				Return buf.ToString()
			End If
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the total zone offset in seconds.
		''' <p>
		''' This is the primary way to access the offset amount.
		''' It returns the total of the hours, minutes and seconds fields as a
		''' single offset that can be added to a time.
		''' </summary>
		''' <returns> the total zone offset amount in seconds </returns>
		Public Property totalSeconds As Integer
			Get
				Return totalSeconds_Renamed
			End Get
		End Property

		''' <summary>
		''' Gets the normalized zone offset ID.
		''' <p>
		''' The ID is minor variation to the standard ISO-8601 formatted string
		''' for the offset. There are three formats:
		''' <ul>
		''' <li>{@code Z} - for UTC (ISO-8601)
		''' <li>{@code +hh:mm} or {@code -hh:mm} - if the seconds are zero (ISO-8601)
		''' <li>{@code +hh:mm:ss} or {@code -hh:mm:ss} - if the seconds are non-zero (not ISO-8601)
		''' </ul>
		''' </summary>
		''' <returns> the zone offset ID, not null </returns>
		Public Property Overrides id As String
			Get
				Return id
			End Get
		End Property

		''' <summary>
		''' Gets the associated time-zone rules.
		''' <p>
		''' The rules will always return this offset when queried.
		''' The implementation class is immutable, thread-safe and serializable.
		''' </summary>
		''' <returns> the rules, not null </returns>
		Public Property Overrides rules As java.time.zone.ZoneRules
			Get
				Return java.time.zone.ZoneRules.of(Me)
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this offset can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		''' <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code OFFSET_SECONDS} field returns true.
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this offset, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field = OFFSET_SECONDS
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This offset is used to enhance the accuracy of the returned range.
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
		''' Gets the value of the specified field from this offset as an {@code int}.
		''' <p>
		''' This queries this offset for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code OFFSET_SECONDS} field returns the value of the offset.
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
			If field = OFFSET_SECONDS Then
				Return totalSeconds_Renamed
			ElseIf TypeOf field Is java.time.temporal.ChronoField Then
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return range(field).checkValidIntValue(getLong(field), field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this offset as a {@code long}.
		''' <p>
		''' This queries this offset for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code OFFSET_SECONDS} field returns the value of the offset.
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
			If field = OFFSET_SECONDS Then
				Return totalSeconds_Renamed
			ElseIf TypeOf field Is java.time.temporal.ChronoField Then
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this offset using the specified query.
		''' <p>
		''' This queries this offset using the specified query strategy object.
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
			If query_Renamed Is java.time.temporal.TemporalQueries.offset() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zone() Then Return CType(Me, R)
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same offset as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the offset changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#OFFSET_SECONDS"/> as the field.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisOffset.adjustInto(temporal);
		'''   temporal = temporal.with(thisOffset);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			Return temporal.with(OFFSET_SECONDS, totalSeconds_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this offset to another offset in descending order.
		''' <p>
		''' The offsets are compared in the order that they occur for the same time
		''' of day around the world. Thus, an offset of {@code +10:00} comes before an
		''' offset of {@code +09:00} and so on down to {@code -18:00}.
		''' <p>
		''' The comparison is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other date to compare to, not null </param>
		''' <returns> the comparator value, negative if less, postive if greater </returns>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overrides Function compareTo(ByVal other As ZoneOffset) As Integer Implements Comparable(Of ZoneOffset).compareTo
			Return other.totalSeconds_Renamed - totalSeconds_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this offset is equal to another offset.
		''' <p>
		''' The comparison is based on the amount of the offset in seconds.
		''' This is equivalent to a comparison by ID.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other offset </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ZoneOffset Then Return totalSeconds_Renamed = CType(obj, ZoneOffset).totalSeconds_Renamed
			Return False
		End Function

		''' <summary>
		''' A hash code for this offset.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return totalSeconds_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this offset as a {@code String}, using the normalized ID.
		''' </summary>
		''' <returns> a string representation of this offset, not null </returns>
		Public Overrides Function ToString() As String
			Return id
		End Function

		' -----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(8);                  // identifies a ZoneOffset
		'''  int offsetByte = totalSeconds % 900 == 0 ? totalSeconds / 900 : 127;
		'''  out.writeByte(offsetByte);
		'''  if (offsetByte == 127) {
		'''      out.writeInt(totalSeconds);
		'''  }
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZONE_OFFSET_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Overrides Sub write(ByVal out As java.io.DataOutput)
			out.writeByte(Ser.ZONE_OFFSET_TYPE)
			writeExternal(out)
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			Dim offsetSecs As Integer = totalSeconds_Renamed
			Dim offsetByte As Integer = If(offsetSecs Mod 900 = 0, offsetSecs \ 900, 127) ' compress to -72 to +72
			out.writeByte(offsetByte)
			If offsetByte = 127 Then out.writeInt(offsetSecs)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ZoneOffset
			Dim offsetByte As Integer = [in].readByte()
			Return (If(offsetByte = 127, ZoneOffset.ofTotalSeconds([in].readInt()), ZoneOffset.ofTotalSeconds(offsetByte * 900)))
		End Function

	End Class

End Namespace