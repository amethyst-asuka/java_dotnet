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
	''' An instantaneous point on the time-line.
	''' <p>
	''' This class models a single instantaneous point on the time-line.
	''' This might be used to record event time-stamps in the application.
	''' <p>
	''' The range of an instant requires the storage of a number larger than a {@code long}.
	''' To achieve this, the class stores a {@code long} representing epoch-seconds and an
	''' {@code int} representing nanosecond-of-second, which will always be between 0 and 999,999,999.
	''' The epoch-seconds are measured from the standard Java epoch of {@code 1970-01-01T00:00:00Z}
	''' where instants after the epoch have positive values, and earlier instants have negative values.
	''' For both the epoch-second and nanosecond parts, a larger value is always later on the time-line
	''' than a smaller value.
	''' 
	''' <h3>Time-scale</h3>
	''' <p>
	''' The length of the solar day is the standard way that humans measure time.
	''' This has traditionally been subdivided into 24 hours of 60 minutes of 60 seconds,
	''' forming a 86400 second day.
	''' <p>
	''' Modern timekeeping is based on atomic clocks which precisely define an SI second
	''' relative to the transitions of a Caesium atom. The length of an SI second was defined
	''' to be very close to the 86400th fraction of a day.
	''' <p>
	''' Unfortunately, as the Earth rotates the length of the day varies.
	''' In addition, over time the average length of the day is getting longer as the Earth slows.
	''' As a result, the length of a solar day in 2012 is slightly longer than 86400 SI seconds.
	''' The actual length of any given day and the amount by which the Earth is slowing
	''' are not predictable and can only be determined by measurement.
	''' The UT1 time-scale captures the accurate length of day, but is only available some
	''' time after the day has completed.
	''' <p>
	''' The UTC time-scale is a standard approach to bundle up all the additional fractions
	''' of a second from UT1 into whole seconds, known as <i>leap-seconds</i>.
	''' A leap-second may be added or removed depending on the Earth's rotational changes.
	''' As such, UTC permits a day to have 86399 SI seconds or 86401 SI seconds where
	''' necessary in order to keep the day aligned with the Sun.
	''' <p>
	''' The modern UTC time-scale was introduced in 1972, introducing the concept of whole leap-seconds.
	''' Between 1958 and 1972, the definition of UTC was complex, with minor sub-second leaps and
	''' alterations to the length of the notional second. As of 2012, discussions are underway
	''' to change the definition of UTC again, with the potential to remove leap seconds or
	''' introduce other changes.
	''' <p>
	''' Given the complexity of accurate timekeeping described above, this Java API defines
	''' its own time-scale, the <i>Java Time-Scale</i>.
	''' <p>
	''' The Java Time-Scale divides each calendar day into exactly 86400
	''' subdivisions, known as seconds.  These seconds may differ from the
	''' SI second.  It closely matches the de facto international civil time
	''' scale, the definition of which changes from time to time.
	''' <p>
	''' The Java Time-Scale has slightly different definitions for different
	''' segments of the time-line, each based on the consensus international
	''' time scale that is used as the basis for civil time. Whenever the
	''' internationally-agreed time scale is modified or replaced, a new
	''' segment of the Java Time-Scale must be defined for it.  Each segment
	''' must meet these requirements:
	''' <ul>
	''' <li>the Java Time-Scale shall closely match the underlying international
	'''  civil time scale;</li>
	''' <li>the Java Time-Scale shall exactly match the international civil
	'''  time scale at noon each day;</li>
	''' <li>the Java Time-Scale shall have a precisely-defined relationship to
	'''  the international civil time scale.</li>
	''' </ul>
	''' There are currently, as of 2013, two segments in the Java time-scale.
	''' <p>
	''' For the segment from 1972-11-03 (exact boundary discussed below) until
	''' further notice, the consensus international time scale is UTC (with
	''' leap seconds).  In this segment, the Java Time-Scale is identical to
	''' <a href="http://www.cl.cam.ac.uk/~mgk25/time/utc-sls/">UTC-SLS</a>.
	''' This is identical to UTC on days that do not have a leap second.
	''' On days that do have a leap second, the leap second is spread equally
	''' over the last 1000 seconds of the day, maintaining the appearance of
	''' exactly 86400 seconds per day.
	''' <p>
	''' For the segment prior to 1972-11-03, extending back arbitrarily far,
	''' the consensus international time scale is defined to be UT1, applied
	''' proleptically, which is equivalent to the (mean) solar time on the
	''' prime meridian (Greenwich). In this segment, the Java Time-Scale is
	''' identical to the consensus international time scale. The exact
	''' boundary between the two segments is the instant where UT1 = UTC
	''' between 1972-11-03T00:00 and 1972-11-04T12:00.
	''' <p>
	''' Implementations of the Java time-scale using the JSR-310 API are not
	''' required to provide any clock that is sub-second accurate, or that
	''' progresses monotonically or smoothly. Implementations are therefore
	''' not required to actually perform the UTC-SLS slew or to otherwise be
	''' aware of leap seconds. JSR-310 does, however, require that
	''' implementations must document the approach they use when defining a
	''' clock representing the current instant.
	''' See <seealso cref="Clock"/> for details on the available clocks.
	''' <p>
	''' The Java time-scale is used for all date-time classes.
	''' This includes {@code Instant}, {@code LocalDate}, {@code LocalTime}, {@code OffsetDateTime},
	''' {@code ZonedDateTime} and {@code Duration}.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code Instant} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Instant
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of Instant)

		''' <summary>
		''' Constant for the 1970-01-01T00:00:00Z epoch instant.
		''' </summary>
		Public Shared ReadOnly EPOCH As New Instant(0, 0)
		''' <summary>
		''' The minimum supported epoch second.
		''' </summary>
		Private Const MIN_SECOND As Long = -31557014167219200L
		''' <summary>
		''' The maximum supported epoch second.
		''' </summary>
		Private Const MAX_SECOND As Long = 31556889864403199L
		''' <summary>
		''' The minimum supported {@code Instant}, '-1000000000-01-01T00:00Z'.
		''' This could be used by an application as a "far past" instant.
		''' <p>
		''' This is one year earlier than the minimum {@code LocalDateTime}.
		''' This provides sufficient values to handle the range of {@code ZoneOffset}
		''' which affect the instant in addition to the local date-time.
		''' The value is also chosen such that the value of the year fits in
		''' an {@code int}.
		''' </summary>
		Public Shared ReadOnly MIN As Instant = Instant.ofEpochSecond(MIN_SECOND, 0)
		''' <summary>
		''' The maximum supported {@code Instant}, '1000000000-12-31T23:59:59.999999999Z'.
		''' This could be used by an application as a "far future" instant.
		''' <p>
		''' This is one year later than the maximum {@code LocalDateTime}.
		''' This provides sufficient values to handle the range of {@code ZoneOffset}
		''' which affect the instant in addition to the local date-time.
		''' The value is also chosen such that the value of the year fits in
		''' an {@code int}.
		''' </summary>
		Public Shared ReadOnly MAX As Instant = Instant.ofEpochSecond(MAX_SECOND, 999999999)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -665713676816604388L

		''' <summary>
		''' The number of seconds from the epoch of 1970-01-01T00:00:00Z.
		''' </summary>
		Private ReadOnly seconds As Long
		''' <summary>
		''' The number of nanoseconds, later along the time-line, from the seconds field.
		''' This is always positive, and never exceeds 999,999,999.
		''' </summary>
		Private ReadOnly nanos As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current instant from the system clock.
		''' <p>
		''' This will query the <seealso cref="Clock#systemUTC() system UTC clock"/> to
		''' obtain the current instant.
		''' <p>
		''' Using this method will prevent the ability to use an alternate time-source for
		''' testing because the clock is effectively hard-coded.
		''' </summary>
		''' <returns> the current instant using the system clock, not null </returns>
		Public Shared Function now() As Instant
			Return Clock.systemUTC().instant()
		End Function

		''' <summary>
		''' Obtains the current instant from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current time.
		''' <p>
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current instant, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As Instant
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Return clock_Renamed.instant()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Instant} using seconds from the
		''' epoch of 1970-01-01T00:00:00Z.
		''' <p>
		''' The nanosecond field is set to zero.
		''' </summary>
		''' <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		''' <returns> an instant, not null </returns>
		''' <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		Public Shared Function ofEpochSecond(ByVal epochSecond As Long) As Instant
			Return create(epochSecond, 0)
		End Function

		''' <summary>
		''' Obtains an instance of {@code Instant} using seconds from the
		''' epoch of 1970-01-01T00:00:00Z and nanosecond fraction of second.
		''' <p>
		''' This method allows an arbitrary number of nanoseconds to be passed in.
		''' The factory will alter the values of the second and nanosecond in order
		''' to ensure that the stored nanosecond is in the range 0 to 999,999,999.
		''' For example, the following will result in the exactly the same instant:
		''' <pre>
		'''  Instant.ofEpochSecond(3, 1);
		'''  Instant.ofEpochSecond(4, -999_999_999);
		'''  Instant.ofEpochSecond(2, 1000_000_001);
		''' </pre>
		''' </summary>
		''' <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		''' <param name="nanoAdjustment">  the nanosecond adjustment to the number of seconds, positive or negative </param>
		''' <returns> an instant, not null </returns>
		''' <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Shared Function ofEpochSecond(ByVal epochSecond As Long, ByVal nanoAdjustment As Long) As Instant
			Dim secs As Long = System.Math.addExact(epochSecond, System.Math.floorDiv(nanoAdjustment, NANOS_PER_SECOND))
			Dim nos As Integer = CInt (System.Math.floorMod(nanoAdjustment, NANOS_PER_SECOND))
			Return create(secs, nos)
		End Function

		''' <summary>
		''' Obtains an instance of {@code Instant} using milliseconds from the
		''' epoch of 1970-01-01T00:00:00Z.
		''' <p>
		''' The seconds and nanoseconds are extracted from the specified milliseconds.
		''' </summary>
		''' <param name="epochMilli">  the number of milliseconds from 1970-01-01T00:00:00Z </param>
		''' <returns> an instant, not null </returns>
		''' <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		Public Shared Function ofEpochMilli(ByVal epochMilli As Long) As Instant
			Dim secs As Long = System.Math.floorDiv(epochMilli, 1000)
			Dim mos As Integer = CInt (System.Math.floorMod(epochMilli, 1000))
			Return create(secs, mos * 1000000)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Instant} from a temporal object.
		''' <p>
		''' This obtains an instant based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code Instant}.
		''' <p>
		''' The conversion extracts the <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		''' and <seealso cref="ChronoField#NANO_OF_SECOND NANO_OF_SECOND"/> fields.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code Instant::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the instant, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code Instant} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As Instant
			If TypeOf temporal Is Instant Then Return CType(temporal, Instant)
			java.util.Objects.requireNonNull(temporal, "temporal")
			Try
				Dim instantSecs As Long = temporal.getLong(INSTANT_SECONDS)
				Dim nanoOfSecond As Integer = temporal.get(NANO_OF_SECOND)
				Return Instant.ofEpochSecond(instantSecs, nanoOfSecond)
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain Instant from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Instant} from a text string such as
		''' {@code 2007-12-03T10:15:30.00Z}.
		''' <p>
		''' The string must represent a valid instant in UTC and is parsed using
		''' <seealso cref="DateTimeFormatter#ISO_INSTANT"/>.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <returns> the parsed instant, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As Instant
			Return java.time.format.DateTimeFormatter.ISO_INSTANT.parse(text, Instant::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Instant} using seconds and nanoseconds.
		''' </summary>
		''' <param name="seconds">  the length of the duration in seconds </param>
		''' <param name="nanoOfSecond">  the nano-of-second, from 0 to 999,999,999 </param>
		''' <exception cref="DateTimeException"> if the instant exceeds the maximum or minimum instant </exception>
		Private Shared Function create(ByVal seconds As Long, ByVal nanoOfSecond As Integer) As Instant
			If (seconds Or nanoOfSecond) = 0 Then Return EPOCH
			If seconds < MIN_SECOND OrElse seconds > MAX_SECOND Then Throw New DateTimeException("Instant exceeds minimum or maximum instant")
			Return New Instant(seconds, nanoOfSecond)
		End Function

		''' <summary>
		''' Constructs an instance of {@code Instant} using seconds from the epoch of
		''' 1970-01-01T00:00:00Z and nanosecond fraction of second.
		''' </summary>
		''' <param name="epochSecond">  the number of seconds from 1970-01-01T00:00:00Z </param>
		''' <param name="nanos">  the nanoseconds within the second, must be positive </param>
		Private Sub New(ByVal epochSecond As Long, ByVal nanos As Integer)
			MyBase.New()
			Me.seconds = epochSecond
			Me.nanos = nanos
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this instant can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code NANO_OF_SECOND}
		''' <li>{@code MICRO_OF_SECOND}
		''' <li>{@code MILLI_OF_SECOND}
		''' <li>{@code INSTANT_SECONDS}
		''' </ul>
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this instant, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field = INSTANT_SECONDS OrElse field = NANO_OF_SECOND OrElse field = MICRO_OF_SECOND OrElse field = MILLI_OF_SECOND
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
		Public Overrides Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return unit.timeBased OrElse unit = DAYS
			Return unit IsNot Nothing AndAlso unit.isSupportedBy(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This instant is used to enhance the accuracy of the returned range.
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
		''' Gets the value of the specified field from this instant as an {@code int}.
		''' <p>
		''' This queries this instant for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this date-time, except {@code INSTANT_SECONDS} which is too
		''' large to fit in an {@code int} and throws a {@code DateTimeException}.
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
			If TypeOf field Is java.time.temporal.ChronoField Then
				Select Case CType(field, java.time.temporal.ChronoField)
					Case NANO_OF_SECOND
						Return nanos
					Case MICRO_OF_SECOND
						Return nanos \ 1000
					Case MILLI_OF_SECOND
						Return nanos \ 1000000
					Case INSTANT_SECONDS
						INSTANT_SECONDS.checkValidIntValue(seconds)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return range(field).checkValidIntValue(field.getFrom(Me), field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this instant as a {@code long}.
		''' <p>
		''' This queries this instant for the value of the specified field.
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
				Select Case CType(field, java.time.temporal.ChronoField)
					Case NANO_OF_SECOND
						Return nanos
					Case MICRO_OF_SECOND
						Return nanos \ 1000
					Case MILLI_OF_SECOND
						Return nanos \ 1000000
					Case INSTANT_SECONDS
						Return seconds
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the number of seconds from the Java epoch of 1970-01-01T00:00:00Z.
		''' <p>
		''' The epoch second count is a simple incrementing count of seconds where
		''' second 0 is 1970-01-01T00:00:00Z.
		''' The nanosecond part of the day is returned by {@code getNanosOfSecond}.
		''' </summary>
		''' <returns> the seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		Public Property epochSecond As Long
			Get
				Return seconds
			End Get
		End Property

		''' <summary>
		''' Gets the number of nanoseconds, later along the time-line, from the start
		''' of the second.
		''' <p>
		''' The nanosecond-of-second value measures the total number of nanoseconds from
		''' the second returned by {@code getEpochSecond}.
		''' </summary>
		''' <returns> the nanoseconds within the second, always positive, never exceeds 999,999,999 </returns>
		Public Property nano As Integer
			Get
				Return nanos
			End Get
		End Property

		'-------------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this instant.
		''' <p>
		''' This returns an {@code Instant}, based on this one, with the instant adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> an {@code Instant} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As Instant
			Return CType(adjuster.adjustInto(Me), Instant)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified field set to a new value.
		''' <p>
		''' This returns an {@code Instant}, based on this one, with the value
		''' for the specified field changed.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code NANO_OF_SECOND} -
		'''  Returns an {@code Instant} with the specified nano-of-second.
		'''  The epoch-second will be unchanged.
		''' <li>{@code MICRO_OF_SECOND} -
		'''  Returns an {@code Instant} with the nano-of-second replaced by the specified
		'''  micro-of-second multiplied by 1,000. The epoch-second will be unchanged.
		''' <li>{@code MILLI_OF_SECOND} -
		'''  Returns an {@code Instant} with the nano-of-second replaced by the specified
		'''  milli-of-second multiplied by 1,000,000. The epoch-second will be unchanged.
		''' <li>{@code INSTANT_SECONDS} -
		'''  Returns an {@code Instant} with the specified epoch-second.
		'''  The nano-of-second will be unchanged.
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
		''' <returns> an {@code Instant} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As Instant
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				f.checkValidValue(newValue)
				Select Case f
					Case MILLI_OF_SECOND
						Dim nval As Integer = CInt(newValue) * 1000000
						Return (If(nval <> nanos, create(seconds, nval), Me))
					Case MICRO_OF_SECOND
						Dim nval As Integer = CInt(newValue) * 1000
						Return (If(nval <> nanos, create(seconds, nval), Me))
					Case NANO_OF_SECOND
						Return (If(newValue <> nanos, create(seconds, CInt(newValue)), Me))
					Case INSTANT_SECONDS
						Return (If(newValue <> seconds, create(newValue, nanos), Me))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code Instant} truncated to the specified unit.
		''' <p>
		''' Truncating the instant returns a copy of the original with fields
		''' smaller than the specified unit set to zero.
		''' The fields are calculated on the basis of using a UTC offset as seen
		''' in {@code toString}.
		''' For example, truncating with the <seealso cref="ChronoUnit#MINUTES MINUTES"/> unit will
		''' round down to the nearest minute, setting the seconds and nanoseconds to zero.
		''' <p>
		''' The unit must have a <seealso cref="TemporalUnit#getDuration() duration"/>
		''' that divides into the length of a standard day without remainder.
		''' This includes all supplied time units on <seealso cref="ChronoUnit"/> and
		''' <seealso cref="ChronoUnit#DAYS DAYS"/>. Other units throw an exception.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="unit">  the unit to truncate to, not null </param>
		''' <returns> an {@code Instant} based on this instant with the time truncated, not null </returns>
		''' <exception cref="DateTimeException"> if the unit is invalid for truncation </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Function truncatedTo(ByVal unit As java.time.temporal.TemporalUnit) As Instant
			If unit = java.time.temporal.ChronoUnit.NANOS Then Return Me
			Dim unitDur As Duration = unit.duration
			If unitDur.seconds > LocalTime.SECONDS_PER_DAY Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unit is too large to be used for truncation")
			Dim dur As Long = unitDur.toNanos()
			If (LocalTime.NANOS_PER_DAY Mod dur) <> 0 Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unit must divide into a standard day without remainder")
			Dim nod As Long = (seconds Mod LocalTime.SECONDS_PER_DAY) * LocalTime.NANOS_PER_SECOND + nanos
			Dim result As Long = (nod \ dur) * dur
			Return plusNanos(result - nod)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this instant with the specified amount added.
		''' <p>
		''' This returns an {@code Instant}, based on this one, with the specified amount added.
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
		''' <returns> an {@code Instant} based on this instant with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As Instant
			Return CType(amountToAdd.addTo(Me), Instant)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified amount added.
		''' <p>
		''' This returns an {@code Instant}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code NANOS} -
		'''  Returns a {@code Instant} with the specified number of nanoseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/>.
		''' <li>{@code MICROS} -
		'''  Returns a {@code Instant} with the specified number of microseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		'''  multiplied by 1,000.
		''' <li>{@code MILLIS} -
		'''  Returns a {@code Instant} with the specified number of milliseconds added.
		'''  This is equivalent to <seealso cref="#plusNanos(long)"/> with the amount
		'''  multiplied by 1,000,000.
		''' <li>{@code SECONDS} -
		'''  Returns a {@code Instant} with the specified number of seconds added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/>.
		''' <li>{@code MINUTES} -
		'''  Returns a {@code Instant} with the specified number of minutes added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		'''  multiplied by 60.
		''' <li>{@code HOURS} -
		'''  Returns a {@code Instant} with the specified number of hours added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		'''  multiplied by 3,600.
		''' <li>{@code HALF_DAYS} -
		'''  Returns a {@code Instant} with the specified number of half-days added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		'''  multiplied by 43,200 (12 hours).
		''' <li>{@code DAYS} -
		'''  Returns a {@code Instant} with the specified number of days added.
		'''  This is equivalent to <seealso cref="#plusSeconds(long)"/> with the amount
		'''  multiplied by 86,400 (24 hours).
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
		''' <returns> an {@code Instant} based on this instant with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As Instant
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case NANOS
						Return plusNanos(amountToAdd)
					Case MICROS
						Return plus(amountToAdd \ 1000000, (amountToAdd Mod 1000000) * 1000)
					Case MILLIS
						Return plusMillis(amountToAdd)
					Case SECONDS
						Return plusSeconds(amountToAdd)
					Case MINUTES
						Return plusSeconds (System.Math.multiplyExact(amountToAdd, SECONDS_PER_MINUTE))
					Case HOURS
						Return plusSeconds (System.Math.multiplyExact(amountToAdd, SECONDS_PER_HOUR))
					Case HALF_DAYS
						Return plusSeconds (System.Math.multiplyExact(amountToAdd, SECONDS_PER_DAY / 2))
					Case DAYS
						Return plusSeconds (System.Math.multiplyExact(amountToAdd, SECONDS_PER_DAY))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this instant with the specified duration in seconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified seconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusSeconds(ByVal secondsToAdd As Long) As Instant
			Return plus(secondsToAdd, 0)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified duration in milliseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="millisToAdd">  the milliseconds to add, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified milliseconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusMillis(ByVal millisToAdd As Long) As Instant
			Return plus(millisToAdd \ 1000, (millisToAdd Mod 1000) * 1000000)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified duration in nanoseconds added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToAdd">  the nanoseconds to add, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified nanoseconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusNanos(ByVal nanosToAdd As Long) As Instant
			Return plus(0, nanosToAdd)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified duration added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToAdd">  the seconds to add, positive or negative </param>
		''' <param name="nanosToAdd">  the nanos to add, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified seconds added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Private Function plus(ByVal secondsToAdd As Long, ByVal nanosToAdd As Long) As Instant
			If (secondsToAdd Or nanosToAdd) = 0 Then Return Me
			Dim epochSec As Long = System.Math.addExact(seconds, secondsToAdd)
			epochSec = System.Math.addExact(epochSec, nanosToAdd / NANOS_PER_SECOND)
			nanosToAdd = nanosToAdd Mod NANOS_PER_SECOND
			Dim nanoAdjustment As Long = nanos + nanosToAdd ' safe int+NANOS_PER_SECOND
			Return ofEpochSecond(epochSec, nanoAdjustment)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this instant with the specified amount subtracted.
		''' <p>
		''' This returns an {@code Instant}, based on this one, with the specified amount subtracted.
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
		''' <returns> an {@code Instant} based on this instant with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As Instant
			Return CType(amountToSubtract.subtractFrom(Me), Instant)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified amount subtracted.
		''' <p>
		''' This returns a {@code Instant}, based on this one, with the amount
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
		''' <returns> an {@code Instant} based on this instant with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As Instant
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this instant with the specified duration in seconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="secondsToSubtract">  the seconds to subtract, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified seconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusSeconds(ByVal secondsToSubtract As Long) As Instant
			If secondsToSubtract = java.lang.[Long].MIN_VALUE Then Return plusSeconds(Long.Max_Value).plusSeconds(1)
			Return plusSeconds(-secondsToSubtract)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified duration in milliseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="millisToSubtract">  the milliseconds to subtract, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified milliseconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusMillis(ByVal millisToSubtract As Long) As Instant
			If millisToSubtract = java.lang.[Long].MIN_VALUE Then Return plusMillis(Long.Max_Value).plusMillis(1)
			Return plusMillis(-millisToSubtract)
		End Function

		''' <summary>
		''' Returns a copy of this instant with the specified duration in nanoseconds subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="nanosToSubtract">  the nanoseconds to subtract, positive or negative </param>
		''' <returns> an {@code Instant} based on this instant with the specified nanoseconds subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the maximum or minimum instant </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusNanos(ByVal nanosToSubtract As Long) As Instant
			If nanosToSubtract = java.lang.[Long].MIN_VALUE Then Return plusNanos(Long.Max_Value).plusNanos(1)
			Return plusNanos(-nanosToSubtract)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Queries this instant using the specified query.
		''' <p>
		''' This queries this instant using the specified query strategy object.
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
			If query_Renamed Is java.time.temporal.TemporalQueries.precision() Then Return CType(NANOS, R)
			' inline TemporalAccessor.super.query(query) as an optimization
			If query_Renamed Is java.time.temporal.TemporalQueries.chronology() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zoneId() OrElse query_Renamed Is java.time.temporal.TemporalQueries.zone() OrElse query_Renamed Is java.time.temporal.TemporalQueries.offset() OrElse query_Renamed Is java.time.temporal.TemporalQueries.localDate() OrElse query_Renamed Is java.time.temporal.TemporalQueries.localTime() Then Return Nothing
			Return query_Renamed.queryFrom(Me)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have this instant.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the instant changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' twice, passing <seealso cref="ChronoField#INSTANT_SECONDS"/> and
		''' <seealso cref="ChronoField#NANO_OF_SECOND"/> as the fields.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisInstant.adjustInto(temporal);
		'''   temporal = temporal.with(thisInstant);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			Return temporal.with(INSTANT_SECONDS, seconds).with(NANO_OF_SECOND, nanos)
		End Function

		''' <summary>
		''' Calculates the amount of time until another instant in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code Instant}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified instant.
		''' The result will be negative if the end is before the start.
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two instants.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code Instant} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in days between two dates can be calculated
		''' using {@code startInstant.until(endInstant, SECONDS)}.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method.
		''' The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		''' <pre>
		'''   // these two lines are equivalent
		'''   amount = start.until(end, SECONDS);
		'''   amount = SECONDS.between(start, end);
		''' </pre>
		''' The choice should be made based on which makes the code more readable.
		''' <p>
		''' The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		''' The units {@code NANOS}, {@code MICROS}, {@code MILLIS}, {@code SECONDS},
		''' {@code MINUTES}, {@code HOURS}, {@code HALF_DAYS} and {@code DAYS}
		''' are supported. Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to an {@code Instant}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this instant and the end instant </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to an {@code Instant} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As Instant = Instant.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim f As java.time.temporal.ChronoUnit = CType(unit, java.time.temporal.ChronoUnit)
				Select Case f
					Case NANOS
						Return nanosUntil([end])
					Case MICROS
						Return nanosUntil([end]) \ 1000
					Case MILLIS
						Return System.Math.subtractExact([end].toEpochMilli(), toEpochMilli())
					Case SECONDS
						Return secondsUntil([end])
					Case MINUTES
						Return secondsUntil([end]) / SECONDS_PER_MINUTE
					Case HOURS
						Return secondsUntil([end]) / SECONDS_PER_HOUR
					Case HALF_DAYS
						Return secondsUntil([end]) / (12 * SECONDS_PER_HOUR)
					Case DAYS
						Return secondsUntil([end]) / (SECONDS_PER_DAY)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.between(Me, [end])
		End Function

		Private Function nanosUntil(ByVal [end] As Instant) As Long
			Dim secsDiff As Long = System.Math.subtractExact([end].seconds, seconds)
			Dim totalNanos As Long = System.Math.multiplyExact(secsDiff, NANOS_PER_SECOND)
			Return System.Math.addExact(totalNanos, [end].nanos - nanos)
		End Function

		Private Function secondsUntil(ByVal [end] As Instant) As Long
			Dim secsDiff As Long = System.Math.subtractExact([end].seconds, seconds)
			Dim nanosDiff As Long = [end].nanos - nanos
			If secsDiff > 0 AndAlso nanosDiff < 0 Then
				secsDiff -= 1
			ElseIf secsDiff < 0 AndAlso nanosDiff > 0 Then
				secsDiff += 1
			End If
			Return secsDiff
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this instant with an offset to create an {@code OffsetDateTime}.
		''' <p>
		''' This returns an {@code OffsetDateTime} formed from this instant at the
		''' specified offset from UTC/Greenwich. An exception will be thrown if the
		''' instant is too large to fit into an offset date-time.
		''' <p>
		''' This method is equivalent to
		''' <seealso cref="OffsetDateTime#ofInstant(Instant, ZoneId) OffsetDateTime.ofInstant(this, offset)"/>.
		''' </summary>
		''' <param name="offset">  the offset to combine with, not null </param>
		''' <returns> the offset date-time formed from this instant and the specified offset, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function atOffset(ByVal offset As ZoneOffset) As OffsetDateTime
			Return OffsetDateTime.ofInstant(Me, offset)
		End Function

		''' <summary>
		''' Combines this instant with a time-zone to create a {@code ZonedDateTime}.
		''' <p>
		''' This returns an {@code ZonedDateTime} formed from this instant at the
		''' specified time-zone. An exception will be thrown if the instant is too
		''' large to fit into a zoned date-time.
		''' <p>
		''' This method is equivalent to
		''' <seealso cref="ZonedDateTime#ofInstant(Instant, ZoneId) ZonedDateTime.ofInstant(this, zone)"/>.
		''' </summary>
		''' <param name="zone">  the zone to combine with, not null </param>
		''' <returns> the zoned date-time formed from this instant and the specified zone, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function atZone(ByVal zone As ZoneId) As ZonedDateTime
			Return ZonedDateTime.ofInstant(Me, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this instant to the number of milliseconds from the epoch
		''' of 1970-01-01T00:00:00Z.
		''' <p>
		''' If this instant represents a point on the time-line too far in the future
		''' or past to fit in a {@code long} milliseconds, then an exception is thrown.
		''' <p>
		''' If this instant has greater than millisecond precision, then the conversion
		''' will drop any excess precision information as though the amount in nanoseconds
		''' was subject to integer division by one million.
		''' </summary>
		''' <returns> the number of milliseconds since the epoch of 1970-01-01T00:00:00Z </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function toEpochMilli() As Long
			Dim millis As Long = System.Math.multiplyExact(seconds, 1000)
			Return millis + nanos \ 1000000
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this instant to the specified instant.
		''' <p>
		''' The comparison is based on the time-line position of the instants.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="otherInstant">  the other instant to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		''' <exception cref="NullPointerException"> if otherInstant is null </exception>
		Public Overrides Function compareTo(ByVal otherInstant As Instant) As Integer Implements Comparable(Of Instant).compareTo
			Dim cmp As Integer = java.lang.[Long].Compare(seconds, otherInstant.seconds)
			If cmp <> 0 Then Return cmp
			Return nanos - otherInstant.nanos
		End Function

		''' <summary>
		''' Checks if this instant is after the specified instant.
		''' <p>
		''' The comparison is based on the time-line position of the instants.
		''' </summary>
		''' <param name="otherInstant">  the other instant to compare to, not null </param>
		''' <returns> true if this instant is after the specified instant </returns>
		''' <exception cref="NullPointerException"> if otherInstant is null </exception>
		Public Function isAfter(ByVal otherInstant As Instant) As Boolean
			Return compareTo(otherInstant) > 0
		End Function

		''' <summary>
		''' Checks if this instant is before the specified instant.
		''' <p>
		''' The comparison is based on the time-line position of the instants.
		''' </summary>
		''' <param name="otherInstant">  the other instant to compare to, not null </param>
		''' <returns> true if this instant is before the specified instant </returns>
		''' <exception cref="NullPointerException"> if otherInstant is null </exception>
		Public Function isBefore(ByVal otherInstant As Instant) As Boolean
			Return compareTo(otherInstant) < 0
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this instant is equal to the specified instant.
		''' <p>
		''' The comparison is based on the time-line position of the instants.
		''' </summary>
		''' <param name="otherInstant">  the other instant, null returns false </param>
		''' <returns> true if the other instant is equal to this one </returns>
		Public Overrides Function Equals(ByVal otherInstant As Object) As Boolean
			If Me Is otherInstant Then Return True
			If TypeOf otherInstant Is Instant Then
				Dim other As Instant = CType(otherInstant, Instant)
				Return Me.seconds = other.seconds AndAlso Me.nanos = other.nanos
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this instant.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (CInt(Fix(seconds Xor (CLng(CULng(seconds) >> 32))))) + 51 * nanos
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' A string representation of this instant using ISO-8601 representation.
		''' <p>
		''' The format used is the same as <seealso cref="DateTimeFormatter#ISO_INSTANT"/>.
		''' </summary>
		''' <returns> an ISO-8601 representation of this instant, not null </returns>
		Public Overrides Function ToString() As String
			Return java.time.format.DateTimeFormatter.ISO_INSTANT.format(Me)
		End Function

		' -----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(2);  // identifies an Instant
		'''  out.writeLong(seconds);
		'''  out.writeInt(nanos);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.INSTANT_TYPE, Me)
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
			out.writeLong(seconds)
			out.writeInt(nanos)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As Instant
			Dim seconds As Long = [in].readLong()
			Dim nanos As Integer = [in].readInt()
			Return Instant.ofEpochSecond(seconds, nanos)
		End Function

	End Class

End Namespace