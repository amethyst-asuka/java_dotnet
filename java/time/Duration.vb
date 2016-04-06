Imports Microsoft.VisualBasic
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
	''' A time-based amount of time, such as '34.5 seconds'.
	''' <p>
	''' This class models a quantity or amount of time in terms of seconds and nanoseconds.
	''' It can be accessed using other duration-based units, such as minutes and hours.
	''' In addition, the <seealso cref="ChronoUnit#DAYS DAYS"/> unit can be used and is treated as
	''' exactly equal to 24 hours, thus ignoring daylight savings effects.
	''' See <seealso cref="Period"/> for the date-based equivalent to this class.
	''' <p>
	''' A physical duration could be of infinite length.
	''' For practicality, the duration is stored with constraints similar to <seealso cref="Instant"/>.
	''' The duration uses nanosecond resolution with a maximum value of the seconds that can
	''' be held in a {@code long}. This is greater than the current estimated age of the universe.
	''' <p>
	''' The range of a duration requires the storage of a number larger than a {@code long}.
	''' To achieve this, the class stores a {@code long} representing seconds and an {@code int}
	''' representing nanosecond-of-second, which will always be between 0 and 999,999,999.
	''' The model is of a directed duration, meaning that the duration may be negative.
	''' <p>
	''' The duration is measured in "seconds", but these are not necessarily identical to
	''' the scientific "SI second" definition based on atomic clocks.
	''' This difference only impacts durations measured near a leap-second and should not affect
	''' most applications.
	''' See <seealso cref="Instant"/> for a discussion as to the meaning of the second and time-scales.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code Duration} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Duration
		Implements java.time.temporal.TemporalAmount, Comparable(Of Duration)

		''' <summary>
		''' Constant for a duration of zero.
		''' </summary>
		Public Shared ReadOnly ZERO As New Duration(0, 0)
		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 3078945930695997490L
		''' <summary>
		''' Constant for nanos per second.
		''' </summary>
		Private Shared ReadOnly BI_NANOS_PER_SECOND As System.Numerics.BigInteger = System.Numerics.Big java.lang.[Integer].valueOf(NANOS_PER_SECOND)
		''' <summary>
		''' The pattern for parsing.
		''' </summary>
		Private Shared ReadOnly PATTERN As java.util.regex.Pattern = java.util.regex.Pattern.compile("([-+]?)P(?:([-+]?[0-9]+)D)?" & "(T(?:([-+]?[0-9]+)H)?(?:([-+]?[0-9]+)M)?(?:([-+]?[0-9]+)(?:[.,]([0-9]{0,9}))?S)?)?", java.util.regex.Pattern.CASE_INSENSITIVE)

		''' <summary>
		''' The number of seconds in the duration.
		''' </summary>
		Private ReadOnly seconds As Long
		''' <summary>
		''' The number of nanoseconds in the duration, expressed as a fraction of the
		''' number of seconds. This is always positive, and never exceeds 999,999,999.
		''' </summary>
		Private ReadOnly nanos As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing a number of standard 24 hour days.
		''' <p>
		''' The seconds are calculated based on the standard definition of a day,
		''' where each day is 86400 seconds which implies a 24 hour day.
		''' The nanosecond in second field is set to zero.
		''' </summary>
		''' <param name="days">  the number of days, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="ArithmeticException"> if the input days exceeds the capacity of {@code Duration} </exception>
		Public Shared Function ofDays(  days As Long) As Duration
			Return create (System.Math.multiplyExact(days, SECONDS_PER_DAY), 0)
		End Function

		''' <summary>
		''' Obtains a {@code Duration} representing a number of standard hours.
		''' <p>
		''' The seconds are calculated based on the standard definition of an hour,
		''' where each hour is 3600 seconds.
		''' The nanosecond in second field is set to zero.
		''' </summary>
		''' <param name="hours">  the number of hours, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="ArithmeticException"> if the input hours exceeds the capacity of {@code Duration} </exception>
		Public Shared Function ofHours(  hours As Long) As Duration
			Return create (System.Math.multiplyExact(hours, SECONDS_PER_HOUR), 0)
		End Function

		''' <summary>
		''' Obtains a {@code Duration} representing a number of standard minutes.
		''' <p>
		''' The seconds are calculated based on the standard definition of a minute,
		''' where each minute is 60 seconds.
		''' The nanosecond in second field is set to zero.
		''' </summary>
		''' <param name="minutes">  the number of minutes, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="ArithmeticException"> if the input minutes exceeds the capacity of {@code Duration} </exception>
		Public Shared Function ofMinutes(  minutes As Long) As Duration
			Return create (System.Math.multiplyExact(minutes, SECONDS_PER_MINUTE), 0)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing a number of seconds.
		''' <p>
		''' The nanosecond in second field is set to zero.
		''' </summary>
		''' <param name="seconds">  the number of seconds, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		Public Shared Function ofSeconds(  seconds As Long) As Duration
			Return create(seconds, 0)
		End Function

		''' <summary>
		''' Obtains a {@code Duration} representing a number of seconds and an
		''' adjustment in nanoseconds.
		''' <p>
		''' This method allows an arbitrary number of nanoseconds to be passed in.
		''' The factory will alter the values of the second and nanosecond in order
		''' to ensure that the stored nanosecond is in the range 0 to 999,999,999.
		''' For example, the following will result in the exactly the same duration:
		''' <pre>
		'''  Duration.ofSeconds(3, 1);
		'''  Duration.ofSeconds(4, -999_999_999);
		'''  Duration.ofSeconds(2, 1000_000_001);
		''' </pre>
		''' </summary>
		''' <param name="seconds">  the number of seconds, positive or negative </param>
		''' <param name="nanoAdjustment">  the nanosecond adjustment to the number of seconds, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="ArithmeticException"> if the adjustment causes the seconds to exceed the capacity of {@code Duration} </exception>
		Public Shared Function ofSeconds(  seconds As Long,   nanoAdjustment As Long) As Duration
			Dim secs As Long = System.Math.addExact(seconds, System.Math.floorDiv(nanoAdjustment, NANOS_PER_SECOND))
			Dim nos As Integer = CInt (System.Math.floorMod(nanoAdjustment, NANOS_PER_SECOND))
			Return create(secs, nos)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing a number of milliseconds.
		''' <p>
		''' The seconds and nanoseconds are extracted from the specified milliseconds.
		''' </summary>
		''' <param name="millis">  the number of milliseconds, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		Public Shared Function ofMillis(  millis As Long) As Duration
			Dim secs As Long = millis \ 1000
			Dim mos As Integer = CInt(Fix(millis Mod 1000))
			If mos < 0 Then
				mos += 1000
				secs -= 1
			End If
			Return create(secs, mos * 1000000)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing a number of nanoseconds.
		''' <p>
		''' The seconds and nanoseconds are extracted from the specified nanoseconds.
		''' </summary>
		''' <param name="nanos">  the number of nanoseconds, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		Public Shared Function ofNanos(  nanos As Long) As Duration
			Dim secs As Long = nanos / NANOS_PER_SECOND
			Dim nos As Integer = CInt(Fix(nanos Mod NANOS_PER_SECOND))
			If nos < 0 Then
				nos += NANOS_PER_SECOND
				secs -= 1
			End If
			Return create(secs, nos)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing an amount in the specified unit.
		''' <p>
		''' The parameters represent the two parts of a phrase like '6 Hours'. For example:
		''' <pre>
		'''  Duration.of(3, SECONDS);
		'''  Duration.of(465, HOURS);
		''' </pre>
		''' Only a subset of units are accepted by this method.
		''' The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		''' be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		''' </summary>
		''' <param name="amount">  the amount of the duration, measured in terms of the unit, positive or negative </param>
		''' <param name="unit">  the unit that the duration is measured in, must have an exact duration, not null </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="DateTimeException"> if the period unit has an estimated duration </exception>
		''' <exception cref="ArithmeticException"> if a numeric overflow occurs </exception>
		Public Shared Function [of](  amount As Long,   unit As java.time.temporal.TemporalUnit) As Duration
			Return ZERO.plus(amount, unit)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Duration} from a temporal amount.
		''' <p>
		''' This obtains a duration based on the specified amount.
		''' A {@code TemporalAmount} represents an  amount of time, which may be
		''' date-based or time-based, which this factory extracts to a duration.
		''' <p>
		''' The conversion loops around the set of units from the amount and uses
		''' the <seealso cref="TemporalUnit#getDuration() duration"/> of the unit to
		''' calculate the total {@code Duration}.
		''' Only a subset of units are accepted by this method. The unit must either
		''' have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/>
		''' or be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours.
		''' If any other units are found then an exception is thrown.
		''' </summary>
		''' <param name="amount">  the temporal amount to convert, not null </param>
		''' <returns> the equivalent duration, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code Duration} </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Shared Function [from](  amount As java.time.temporal.TemporalAmount) As Duration
			java.util.Objects.requireNonNull(amount, "amount")
			Dim duration_Renamed As Duration = ZERO
			For Each unit As java.time.temporal.TemporalUnit In amount.units
				duration_Renamed = duration_Renamed.plus(amount.get(unit), unit)
			Next unit
			Return duration_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} from a text string such as {@code PnDTnHnMn.nS}.
		''' <p>
		''' This will parse a textual representation of a duration, including the
		''' string produced by {@code toString()}. The formats accepted are based
		''' on the ISO-8601 duration format {@code PnDTnHnMn.nS} with days
		''' considered to be exactly 24 hours.
		''' <p>
		''' The string starts with an optional sign, denoted by the ASCII negative
		''' or positive symbol. If negative, the whole period is negated.
		''' The ASCII letter "P" is next in upper or lower case.
		''' There are then four sections, each consisting of a number and a suffix.
		''' The sections have suffixes in ASCII of "D", "H", "M" and "S" for
		''' days, hours, minutes and seconds, accepted in upper or lower case.
		''' The suffixes must occur in order. The ASCII letter "T" must occur before
		''' the first occurrence, if any, of an hour, minute or second section.
		''' At least one of the four sections must be present, and if "T" is present
		''' there must be at least one section after the "T".
		''' The number part of each section must consist of one or more ASCII digits.
		''' The number may be prefixed by the ASCII negative or positive symbol.
		''' The number of days, hours and minutes must parse to an {@code long}.
		''' The number of seconds must parse to an {@code long} with optional fraction.
		''' The decimal point may be either a dot or a comma.
		''' The fractional part may have from zero to 9 digits.
		''' <p>
		''' The leading plus/minus sign, and negative values for other units are
		''' not part of the ISO-8601 standard.
		''' <p>
		''' Examples:
		''' <pre>
		'''    "PT20.345S" -- parses as "20.345 seconds"
		'''    "PT15M"     -- parses as "15 minutes" (where a minute is 60 seconds)
		'''    "PT10H"     -- parses as "10 hours" (where an hour is 3600 seconds)
		'''    "P2D"       -- parses as "2 days" (where a day is 24 hours or 86400 seconds)
		'''    "P2DT3H4M"  -- parses as "2 days, 3 hours and 4 minutes"
		'''    "P-6H3M"    -- parses as "-6 hours and +3 minutes"
		'''    "-P6H3M"    -- parses as "-6 hours and -3 minutes"
		'''    "-P-6H+3M"  -- parses as "+6 hours and -3 minutes"
		''' </pre>
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <returns> the parsed duration, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed to a duration </exception>
		Public Shared Function parse(  text As CharSequence) As Duration
			java.util.Objects.requireNonNull(text, "text")
			Dim matcher As java.util.regex.Matcher = PATTERN.matcher(text)
			If matcher.matches() Then
				' check for letter T but no time sections
				If "T".Equals(matcher.group(3)) = False Then
					Dim negate As Boolean = "-".Equals(matcher.group(1))
					Dim dayMatch As String = matcher.group(2)
					Dim hourMatch As String = matcher.group(4)
					Dim minuteMatch As String = matcher.group(5)
					Dim secondMatch As String = matcher.group(6)
					Dim fractionMatch As String = matcher.group(7)
					If dayMatch IsNot Nothing OrElse hourMatch IsNot Nothing OrElse minuteMatch IsNot Nothing OrElse secondMatch IsNot Nothing Then
						Dim daysAsSecs As Long = parseNumber(text, dayMatch, SECONDS_PER_DAY, "days")
						Dim hoursAsSecs As Long = parseNumber(text, hourMatch, SECONDS_PER_HOUR, "hours")
						Dim minsAsSecs As Long = parseNumber(text, minuteMatch, SECONDS_PER_MINUTE, "minutes")
						Dim seconds_Renamed As Long = parseNumber(text, secondMatch, 1, "seconds")
						Dim nanos As Integer = parseFraction(text, fractionMatch,If(seconds_Renamed < 0, -1, 1))
						Try
							Return create(negate, daysAsSecs, hoursAsSecs, minsAsSecs, seconds_Renamed, nanos)
						Catch ex As ArithmeticException
							Throw CType((New java.time.format.DateTimeParseException("Text cannot be parsed to a Duration: overflow", text, 0)).initCause(ex), java.time.format.DateTimeParseException)
						End Try
					End If
				End If
			End If
			Throw New java.time.format.DateTimeParseException("Text cannot be parsed to a Duration", text, 0)
		End Function

		Private Shared Function parseNumber(  text As CharSequence,   parsed As String,   multiplier As Integer,   errorText As String) As Long
			' regex limits to [-+]?[0-9]+
			If parsed Is Nothing Then Return 0
			Try
				Dim val As Long = Convert.ToInt64(parsed)
				Return System.Math.multiplyExact(val, multiplier)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch NumberFormatException Or ArithmeticException ex
				Throw CType((New java.time.format.DateTimeParseException("Text cannot be parsed to a Duration: " & errorText, text, 0)).initCause(ex), java.time.format.DateTimeParseException)
			End Try
		End Function

		Private Shared Function parseFraction(  text As CharSequence,   parsed As String,   negate As Integer) As Integer
			' regex limits to [0-9]{0,9}
			If parsed Is Nothing OrElse parsed.length() = 0 Then Return 0
			Try
				parsed = (parsed & "000000000").Substring(0, 9)
				Return Convert.ToInt32(parsed) * negate
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch NumberFormatException Or ArithmeticException ex
				Throw CType((New java.time.format.DateTimeParseException("Text cannot be parsed to a Duration: fraction", text, 0)).initCause(ex), java.time.format.DateTimeParseException)
			End Try
		End Function

		Private Shared Function create(  negate As Boolean,   daysAsSecs As Long,   hoursAsSecs As Long,   minsAsSecs As Long,   secs As Long,   nanos As Integer) As Duration
			Dim seconds_Renamed As Long = System.Math.addExact(daysAsSecs, System.Math.addExact(hoursAsSecs, System.Math.addExact(minsAsSecs, secs)))
			If negate Then Return ofSeconds(seconds_Renamed, nanos).negated()
			Return ofSeconds(seconds_Renamed, nanos)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Duration} representing the duration between two temporal objects.
		''' <p>
		''' This calculates the duration between two temporal objects. If the objects
		''' are of different types, then the duration is calculated based on the type
		''' of the first object. For example, if the first argument is a {@code LocalTime}
		''' then the second argument is converted to a {@code LocalTime}.
		''' <p>
		''' The specified temporal objects must support the <seealso cref="ChronoUnit#SECONDS SECONDS"/> unit.
		''' For full accuracy, either the <seealso cref="ChronoUnit#NANOS NANOS"/> unit or the
		''' <seealso cref="ChronoField#NANO_OF_SECOND NANO_OF_SECOND"/> field should be supported.
		''' <p>
		''' The result of this method can be a negative period if the end is before the start.
		''' To guarantee to obtain a positive duration call <seealso cref="#abs()"/> on the result.
		''' </summary>
		''' <param name="startInclusive">  the start instant, inclusive, not null </param>
		''' <param name="endExclusive">  the end instant, exclusive, not null </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="DateTimeException"> if the seconds between the temporals cannot be obtained </exception>
		''' <exception cref="ArithmeticException"> if the calculation exceeds the capacity of {@code Duration} </exception>
		Public Shared Function between(  startInclusive As java.time.temporal.Temporal,   endExclusive As java.time.temporal.Temporal) As Duration
			Try
				Return ofNanos(startInclusive.until(endExclusive, NANOS))
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch DateTimeException Or ArithmeticException ex
				Dim secs As Long = startInclusive.until(endExclusive, SECONDS)
				Dim nanos As Long
				Try
					nanos = endExclusive.getLong(NANO_OF_SECOND) - startInclusive.getLong(NANO_OF_SECOND)
					If secs > 0 AndAlso nanos < 0 Then
						secs += 1
					ElseIf secs < 0 AndAlso nanos > 0 Then
						secs -= 1
					End If
				Catch ex2 As DateTimeException
					nanos = 0
				End Try
				Return ofSeconds(secs, nanos)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Duration} using seconds and nanoseconds.
		''' </summary>
		''' <param name="seconds">  the length of the duration in seconds, positive or negative </param>
		''' <param name="nanoAdjustment">  the nanosecond adjustment within the second, from 0 to 999,999,999 </param>
		Private Shared Function create(  seconds As Long,   nanoAdjustment As Integer) As Duration
			If (seconds Or nanoAdjustment) = 0 Then Return ZERO
			Return New Duration(seconds, nanoAdjustment)
		End Function

		''' <summary>
		''' Constructs an instance of {@code Duration} using seconds and nanoseconds.
		''' </summary>
		''' <param name="seconds">  the length of the duration in seconds, positive or negative </param>
		''' <param name="nanos">  the nanoseconds within the second, from 0 to 999,999,999 </param>
		Private Sub New(  seconds As Long,   nanos As Integer)
			MyBase.New()
			Me.seconds = seconds
			Me.nanos = nanos
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the value of the requested unit.
		''' <p>
		''' This returns a value for each of the two supported units,
		''' <seealso cref="ChronoUnit#SECONDS SECONDS"/> and <seealso cref="ChronoUnit#NANOS NANOS"/>.
		''' All other units throw an exception.
		''' </summary>
		''' <param name="unit"> the {@code TemporalUnit} for which to return the value </param>
		''' <returns> the long value of the unit </returns>
		''' <exception cref="DateTimeException"> if the unit is not supported </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Overrides Function [get](  unit As java.time.temporal.TemporalUnit) As Long
			If unit = SECONDS Then
				Return seconds
			ElseIf unit = NANOS Then
				Return nanos
			Else
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
		End Function

		''' <summary>
		''' Gets the set of units supported by this duration.
		''' <p>
		''' The supported units are <seealso cref="ChronoUnit#SECONDS SECONDS"/>,
		''' and <seealso cref="ChronoUnit#NANOS NANOS"/>.
		''' They are returned in the order seconds, nanos.
		''' <p>
		''' This set can be used in conjunction with <seealso cref="#get(TemporalUnit)"/>
		''' to access the entire state of the duration.
		''' </summary>
		''' <returns> a list containing the seconds and nanos units, not null </returns>
		Public  Overrides ReadOnly Property  units As IList(Of java.time.temporal.TemporalUnit)
			Get
				Return DurationUnits.UNITS
			End Get
		End Property

		''' <summary>
		''' Private class to delay initialization of this list until needed.
		''' The circular dependency between Duration and ChronoUnit prevents
		''' the simple initialization in Duration.
		''' </summary>
		Private Class DurationUnits
			Friend Shared ReadOnly UNITS As IList(Of java.time.temporal.TemporalUnit) = java.util.Collections.unmodifiableList(java.util.Arrays.asList(Of java.time.temporal.TemporalUnit)(SECONDS, NANOS))
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this duration is zero length.
		''' <p>
		''' A {@code Duration} represents a directed distance between two points on
		''' the time-line and can therefore be positive, zero or negative.
		''' This method checks whether the length is zero.
		''' </summary>
		''' <returns> true if this duration has a total length equal to zero </returns>
		Public Property zero As Boolean
			Get
				Return (seconds Or nanos) = 0
			End Get
		End Property

		''' <summary>
		''' Checks if this duration is negative, excluding zero.
		''' <p>
		''' A {@code Duration} represents a directed distance between two points on
		''' the time-line and can therefore be positive, zero or negative.
		''' This method checks whether the length is less than zero.
		''' </summary>
		''' <returns> true if this duration has a total length less than zero </returns>
		Public Property negative As Boolean
			Get
				Return seconds < 0
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the number of seconds in this duration.
		''' <p>
		''' The length of the duration is stored using two fields - seconds and nanoseconds.
		''' The nanoseconds part is a value from 0 to 999,999,999 that is an adjustment to
		''' the length in seconds.
		''' The total duration is defined by calling this method and <seealso cref="#getNano()"/>.
		''' <p>
		''' A {@code Duration} represents a directed distance between two points on the time-line.
		''' A negative duration is expressed by the negative sign of the seconds part.
		''' A duration of -1 nanosecond is stored as -1 seconds plus 999,999,999 nanoseconds.
		''' </summary>
		''' <returns> the whole seconds part of the length of the duration, positive or negative </returns>
		Public Property seconds As Long
			Get
				Return seconds
			End Get
		End Property

		''' <summary>
		''' Gets the number of nanoseconds within the second in this duration.
		''' <p>
		''' The length of the duration is stored using two fields - seconds and nanoseconds.
		''' The nanoseconds part is a value from 0 to 999,999,999 that is an adjustment to
		''' the length in seconds.
		''' The total duration is defined by calling this method and <seealso cref="#getSeconds()"/>.
		''' <p>
		''' A {@code Duration} represents a directed distance between two points on the time-line.
		''' A negative duration is expressed by the negative sign of the seconds part.
		''' A duration of -1 nanosecond is stored as -1 seconds plus 999,999,999 nanoseconds.
		''' </summary>
		''' <returns> the nanoseconds within the second part of the length of the duration, from 0 to 999,999,999 </returns>
		Public Property nano As Integer
			Get
				Return nanos
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the specified amount of seconds.
		''' <p>
		''' This returns a duration with the specified seconds, retaining the
		''' nano-of-second part of this duration.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="seconds">  the seconds to represent, may be negative </param>
		''' <returns> a {@code Duration} based on this period with the requested seconds, not null </returns>
		Public Function withSeconds(  seconds As Long) As Duration
			Return create(seconds, nanos)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified nano-of-second.
		''' <p>
		''' This returns a duration with the specified nano-of-second, retaining the
		''' seconds part of this duration.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <returns> a {@code Duration} based on this period with the requested nano-of-second, not null </returns>
		''' <exception cref="DateTimeException"> if the nano-of-second is invalid </exception>
		Public Function withNanos(  nanoOfSecond As Integer) As Duration
			NANO_OF_SECOND.checkValidIntValue(nanoOfSecond)
			Return create(seconds, nanoOfSecond)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the specified duration added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="duration">  the duration to add, positive or negative, not null </param>
		''' <returns> a {@code Duration} based on this duration with the specified duration added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plus(  duration_Renamed As Duration) As Duration
			Return plus(duration_Renamed.seconds, duration_Renamed.nano)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration added.
		''' <p>
		''' The duration amount is measured in terms of the specified unit.
		''' Only a subset of units are accepted by this method.
		''' The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		''' be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToAdd">  the amount to add, measured in terms of the unit, positive or negative </param>
		''' <param name="unit">  the unit that the amount is measured in, must have an exact duration, not null </param>
		''' <returns> a {@code Duration} based on this duration with the specified duration added, not null </returns>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As Duration
			java.util.Objects.requireNonNull(unit, "unit")
			If unit = DAYS Then Return plus (System.Math.multiplyExact(amountToAdd, SECONDS_PER_DAY), 0)
			If unit.durationEstimated Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unit must not have an estimated duration")
			If amountToAdd = 0 Then Return Me
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case NANOS
						Return plusNanos(amountToAdd)
					Case MICROS
						Return plusSeconds((amountToAdd / (1000_000L * 1000)) * 1000).plusNanos((amountToAdd Mod (1000_000L * 1000)) * 1000)
					Case MILLIS
						Return plusMillis(amountToAdd)
					Case SECONDS
						Return plusSeconds(amountToAdd)
				End Select
				Return plusSeconds (System.Math.multiplyExact(unit.duration.seconds, amountToAdd))
			End If
			Dim duration_Renamed As Duration = unit.duration.multipliedBy(amountToAdd)
			Return plusSeconds(duration_Renamed.seconds).plusNanos(duration_Renamed.nano)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the specified duration in standard 24 hour days added.
		''' <p>
		''' The number of days is multiplied by 86400 to obtain the number of seconds to add.
		''' This is based on the standard definition of a day as 24 hours.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToAdd">  the days to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified days added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusDays(  daysToAdd As Long) As Duration
			Return plus (System.Math.multiplyExact(daysToAdd, SECONDS_PER_DAY), 0)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in hours added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hoursToAdd">  the hours to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified hours added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusHours(  hoursToAdd As Long) As Duration
			Return plus (System.Math.multiplyExact(hoursToAdd, SECONDS_PER_HOUR), 0)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in minutes added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutesToAdd">  the minutes to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified minutes added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusMinutes(  minutesToAdd As Long) As Duration
			Return plus (System.Math.multiplyExact(minutesToAdd, SECONDS_PER_MINUTE), 0)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in seconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified seconds added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusSeconds(  secondsToAdd As Long) As Duration
			Return plus(secondsToAdd, 0)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in milliseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="millisToAdd">  the milliseconds to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified milliseconds added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusMillis(  millisToAdd As Long) As Duration
			Return plus(millisToAdd \ 1000, (millisToAdd Mod 1000) * 1000000)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in nanoseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToAdd">  the nanoseconds to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified nanoseconds added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusNanos(  nanosToAdd As Long) As Duration
			Return plus(0, nanosToAdd)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		''' <param name="nanosToAdd">  the nanos to add, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified seconds added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Private Function plus(  secondsToAdd As Long,   nanosToAdd As Long) As Duration
			If (secondsToAdd Or nanosToAdd) = 0 Then Return Me
			Dim epochSec As Long = System.Math.addExact(seconds, secondsToAdd)
			epochSec = System.Math.addExact(epochSec, nanosToAdd / NANOS_PER_SECOND)
			nanosToAdd = nanosToAdd Mod NANOS_PER_SECOND
			Dim nanoAdjustment As Long = nanos + nanosToAdd ' safe int+NANOS_PER_SECOND
			Return ofSeconds(epochSec, nanoAdjustment)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the specified duration subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="duration">  the duration to subtract, positive or negative, not null </param>
		''' <returns> a {@code Duration} based on this duration with the specified duration subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minus(  duration_Renamed As Duration) As Duration
			Dim secsToSubtract As Long = duration_Renamed.seconds
			Dim nanosToSubtract As Integer = duration_Renamed.nano
			If secsToSubtract = java.lang.[Long].MIN_VALUE Then Return plus(Long.Max_Value, -nanosToSubtract).plus(1, 0)
			Return plus(-secsToSubtract, -nanosToSubtract)
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration subtracted.
		''' <p>
		''' The duration amount is measured in terms of the specified unit.
		''' Only a subset of units are accepted by this method.
		''' The unit must either have an <seealso cref="TemporalUnit#isDurationEstimated() exact duration"/> or
		''' be <seealso cref="ChronoUnit#DAYS"/> which is treated as 24 hours. Other units throw an exception.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToSubtract">  the amount to subtract, measured in terms of the unit, positive or negative </param>
		''' <param name="unit">  the unit that the amount is measured in, must have an exact duration, not null </param>
		''' <returns> a {@code Duration} based on this duration with the specified duration subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minus(  amountToSubtract As Long,   unit As java.time.temporal.TemporalUnit) As Duration
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the specified duration in standard 24 hour days subtracted.
		''' <p>
		''' The number of days is multiplied by 86400 to obtain the number of seconds to subtract.
		''' This is based on the standard definition of a day as 24 hours.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToSubtract">  the days to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified days subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusDays(  daysToSubtract As Long) As Duration
			Return (If(daysToSubtract = java.lang.[Long].MIN_VALUE, plusDays(Long.Max_Value).plusDays(1), plusDays(-daysToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in hours subtracted.
		''' <p>
		''' The number of hours is multiplied by 3600 to obtain the number of seconds to subtract.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="hoursToSubtract">  the hours to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified hours subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusHours(  hoursToSubtract As Long) As Duration
			Return (If(hoursToSubtract = java.lang.[Long].MIN_VALUE, plusHours(Long.Max_Value).plusHours(1), plusHours(-hoursToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in minutes subtracted.
		''' <p>
		''' The number of hours is multiplied by 60 to obtain the number of seconds to subtract.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="minutesToSubtract">  the minutes to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified minutes subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusMinutes(  minutesToSubtract As Long) As Duration
			Return (If(minutesToSubtract = java.lang.[Long].MIN_VALUE, plusMinutes(Long.Max_Value).plusMinutes(1), plusMinutes(-minutesToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in seconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToSubtract">  the seconds to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified seconds subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusSeconds(  secondsToSubtract As Long) As Duration
			Return (If(secondsToSubtract = java.lang.[Long].MIN_VALUE, plusSeconds(Long.Max_Value).plusSeconds(1), plusSeconds(-secondsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in milliseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="millisToSubtract">  the milliseconds to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified milliseconds subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusMillis(  millisToSubtract As Long) As Duration
			Return (If(millisToSubtract = java.lang.[Long].MIN_VALUE, plusMillis(Long.Max_Value).plusMillis(1), plusMillis(-millisToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this duration with the specified duration in nanoseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToSubtract">  the nanoseconds to subtract, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration with the specified nanoseconds subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusNanos(  nanosToSubtract As Long) As Duration
			Return (If(nanosToSubtract = java.lang.[Long].MIN_VALUE, plusNanos(Long.Max_Value).plusNanos(1), plusNanos(-nanosToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration multiplied by the scalar.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="multiplicand">  the value to multiply the duration by, positive or negative </param>
		''' <returns> a {@code Duration} based on this duration multiplied by the specified scalar, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function multipliedBy(  multiplicand As Long) As Duration
			If multiplicand = 0 Then Return ZERO
			If multiplicand = 1 Then Return Me
			Return create(toSeconds() * Decimal.valueOf(multiplicand))
		End Function

		''' <summary>
		''' Returns a copy of this duration divided by the specified value.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="divisor">  the value to divide the duration by, positive or negative, not zero </param>
		''' <returns> a {@code Duration} based on this duration divided by the specified divisor, not null </returns>
		''' <exception cref="ArithmeticException"> if the divisor is zero or if numeric overflow occurs </exception>
		Public Function dividedBy(  divisor As Long) As Duration
			If divisor = 0 Then Throw New ArithmeticException("Cannot divide by zero")
			If divisor = 1 Then Return Me
			Return create(toSeconds().divide(Decimal.valueOf(divisor), java.math.RoundingMode.DOWN))
		End Function

		''' <summary>
		''' Converts this duration to the total length in seconds and
		''' fractional nanoseconds expressed as a {@code BigDecimal}.
		''' </summary>
		''' <returns> the total length of the duration in seconds, with a scale of 9, not null </returns>
		Private Function toSeconds() As Decimal
			Return Decimal.valueOf(seconds) + Decimal.valueOf(nanos, 9)
		End Function

		''' <summary>
		''' Creates an instance of {@code Duration} from a number of seconds.
		''' </summary>
		''' <param name="seconds">  the number of seconds, up to scale 9, positive or negative </param>
		''' <returns> a {@code Duration}, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Private Shared Function create(  seconds As Decimal) As Duration
			Dim nanos As System.Numerics.BigInteger = seconds.movePointRight(9).toBigIntegerExact()
			Dim divRem As System.Numerics.BigInteger() = nanos.divideAndRemainder(BI_NANOS_PER_SECOND)
			If divRem(0).bitLength() > 63 Then Throw New ArithmeticException("Exceeds capacity of Duration: " & nanos)
			Return ofSeconds(divRem(0), divRem(1))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this duration with the length negated.
		''' <p>
		''' This method swaps the sign of the total length of this duration.
		''' For example, {@code PT1.3S} will be returned as {@code PT-1.3S}.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> a {@code Duration} based on this duration with the amount negated, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function negated() As Duration
			Return multipliedBy(-1)
		End Function

		''' <summary>
		''' Returns a copy of this duration with a positive length.
		''' <p>
		''' This method returns a positive duration by effectively removing the sign from any negative total length.
		''' For example, {@code PT-1.3S} will be returned as {@code PT1.3S}.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> a {@code Duration} based on this duration with an absolute length, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function abs() As Duration
			Return If(negative, negated(), Me)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Adds this duration to the specified temporal object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with this duration added.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#plus(TemporalAmount)"/>.
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   dateTime = thisDuration.addTo(dateTime);
		'''   dateTime = dateTime.plus(thisDuration);
		''' </pre>
		''' <p>
		''' The calculation will add the seconds, then nanos.
		''' Only non-zero amounts will be added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the temporal object to adjust, not null </param>
		''' <returns> an object of the same type with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if unable to add </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function addTo(  temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			If seconds <> 0 Then temporal = temporal.plus(seconds, SECONDS)
			If nanos <> 0 Then temporal = temporal.plus(nanos, NANOS)
			Return temporal
		End Function

		''' <summary>
		''' Subtracts this duration from the specified temporal object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with this duration subtracted.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#minus(TemporalAmount)"/>.
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   dateTime = thisDuration.subtractFrom(dateTime);
		'''   dateTime = dateTime.minus(thisDuration);
		''' </pre>
		''' <p>
		''' The calculation will subtract the seconds, then nanos.
		''' Only non-zero amounts will be added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the temporal object to adjust, not null </param>
		''' <returns> an object of the same type with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if unable to subtract </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function subtractFrom(  temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			If seconds <> 0 Then temporal = temporal.minus(seconds, SECONDS)
			If nanos <> 0 Then temporal = temporal.minus(nanos, NANOS)
			Return temporal
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the number of days in this duration.
		''' <p>
		''' This returns the total number of days in the duration by dividing the
		''' number of seconds by 86400.
		''' This is based on the standard definition of a day as 24 hours.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> the number of days in the duration, may be negative </returns>
		Public Function toDays() As Long
			Return seconds / SECONDS_PER_DAY
		End Function

		''' <summary>
		''' Gets the number of hours in this duration.
		''' <p>
		''' This returns the total number of hours in the duration by dividing the
		''' number of seconds by 3600.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> the number of hours in the duration, may be negative </returns>
		Public Function toHours() As Long
			Return seconds / SECONDS_PER_HOUR
		End Function

		''' <summary>
		''' Gets the number of minutes in this duration.
		''' <p>
		''' This returns the total number of minutes in the duration by dividing the
		''' number of seconds by 60.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> the number of minutes in the duration, may be negative </returns>
		Public Function toMinutes() As Long
			Return seconds / SECONDS_PER_MINUTE
		End Function

		''' <summary>
		''' Converts this duration to the total length in milliseconds.
		''' <p>
		''' If this duration is too large to fit in a {@code long} milliseconds, then an
		''' exception is thrown.
		''' <p>
		''' If this duration has greater than millisecond precision, then the conversion
		''' will drop any excess precision information as though the amount in nanoseconds
		''' was subject to integer division by one million.
		''' </summary>
		''' <returns> the total length of the duration in milliseconds </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function toMillis() As Long
			Dim millis As Long = System.Math.multiplyExact(seconds, 1000)
			millis = System.Math.addExact(millis, nanos \ 1000000)
			Return millis
		End Function

		''' <summary>
		''' Converts this duration to the total length in nanoseconds expressed as a {@code long}.
		''' <p>
		''' If this duration is too large to fit in a {@code long} nanoseconds, then an
		''' exception is thrown.
		''' </summary>
		''' <returns> the total length of the duration in nanoseconds </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function toNanos() As Long
			Dim totalNanos As Long = System.Math.multiplyExact(seconds, NANOS_PER_SECOND)
			totalNanos = System.Math.addExact(totalNanos, nanos)
			Return totalNanos
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this duration to the specified {@code Duration}.
		''' <p>
		''' The comparison is based on the total length of the durations.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="otherDuration">  the other duration to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(  otherDuration As Duration) As Integer Implements Comparable(Of Duration).compareTo
			Dim cmp As Integer = java.lang.[Long].Compare(seconds, otherDuration.seconds)
			If cmp <> 0 Then Return cmp
			Return nanos - otherDuration.nanos
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this duration is equal to the specified {@code Duration}.
		''' <p>
		''' The comparison is based on the total length of the durations.
		''' </summary>
		''' <param name="otherDuration">  the other duration, null returns false </param>
		''' <returns> true if the other duration is equal to this one </returns>
		Public Overrides Function Equals(  otherDuration As Object) As Boolean
			If Me Is otherDuration Then Return True
			If TypeOf otherDuration Is Duration Then
				Dim other As Duration = CType(otherDuration, Duration)
				Return Me.seconds = other.seconds AndAlso Me.nanos = other.nanos
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this duration.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (CInt(Fix(seconds Xor (CLng(CULng(seconds) >> 32))))) + (51 * nanos)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' A string representation of this duration using ISO-8601 seconds
		''' based representation, such as {@code PT8H6M12.345S}.
		''' <p>
		''' The format of the returned string will be {@code PTnHnMnS}, where n is
		''' the relevant hours, minutes or seconds part of the duration.
		''' Any fractional seconds are placed after a decimal point i the seconds section.
		''' If a section has a zero value, it is omitted.
		''' The hours, minutes and seconds will all have the same sign.
		''' <p>
		''' Examples:
		''' <pre>
		'''    "20.345 seconds"                 -- "PT20.345S
		'''    "15 minutes" (15 * 60 seconds)   -- "PT15M"
		'''    "10 hours" (10 * 3600 seconds)   -- "PT10H"
		'''    "2 days" (2 * 86400 seconds)     -- "PT48H"
		''' </pre>
		''' Note that multiples of 24 hours are not output as days to avoid confusion
		''' with {@code Period}.
		''' </summary>
		''' <returns> an ISO-8601 representation of this duration, not null </returns>
		Public Overrides Function ToString() As String
			If Me Is ZERO Then Return "PT0S"
			Dim hours As Long = seconds / SECONDS_PER_HOUR
			Dim minutes As Integer = CInt(Fix((seconds Mod SECONDS_PER_HOUR) / SECONDS_PER_MINUTE))
			Dim secs As Integer = CInt(Fix(seconds Mod SECONDS_PER_MINUTE))
			Dim buf As New StringBuilder(24)
			buf.append("PT")
			If hours <> 0 Then buf.append(hours).append("H"c)
			If minutes <> 0 Then buf.append(minutes).append("M"c)
			If secs = 0 AndAlso nanos = 0 AndAlso buf.length() > 2 Then Return buf.ToString()
			If secs < 0 AndAlso nanos > 0 Then
				If secs = -1 Then
					buf.append("-0")
				Else
					buf.append(secs + 1)
				End If
			Else
				buf.append(secs)
			End If
			If nanos > 0 Then
				Dim pos As Integer = buf.length()
				If secs < 0 Then
					buf.append(2 * NANOS_PER_SECOND - nanos)
				Else
					buf.append(nanos + NANOS_PER_SECOND)
				End If
				Do While buf.Chars(buf.length() - 1) Is "0"c
					buf.length = buf.length() - 1
				Loop
				buf.charAtrAt(pos, "."c)
			End If
			buf.append("S"c)
			Return buf.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(1);  // identifies a Duration
		'''  out.writeLong(seconds);
		'''  out.writeInt(nanos);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.DURATION_TYPE, Me)
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
			out.writeLong(seconds)
			out.writeInt(nanos)
		End Sub

		Shared Function readExternal(  [in] As java.io.DataInput) As Duration
			Dim seconds_Renamed As Long = [in].readLong()
			Dim nanos As Integer = [in].readInt()
			Return Duration.ofSeconds(seconds_Renamed, nanos)
		End Function

	End Class

End Namespace