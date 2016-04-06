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
Namespace java.time.chrono



	''' <summary>
	''' A date-time with a time-zone in an arbitrary chronology,
	''' intended for advanced globalization use cases.
	''' <p>
	''' <b>Most applications should declare method signatures, fields and variables
	''' as <seealso cref="ZonedDateTime"/>, not this interface.</b>
	''' <p>
	''' A {@code ChronoZonedDateTime} is the abstract representation of an offset date-time
	''' where the {@code Chronology chronology}, or calendar system, is pluggable.
	''' The date-time is defined in terms of fields expressed by <seealso cref="TemporalField"/>,
	''' where most common implementations are defined in <seealso cref="ChronoField"/>.
	''' The chronology defines how the calendar system operates and the meaning of
	''' the standard fields.
	''' 
	''' <h3>When to use this interface</h3>
	''' The design of the API encourages the use of {@code ZonedDateTime} rather than this
	''' interface, even in the case where the application needs to deal with multiple
	''' calendar systems. The rationale for this is explored in detail in <seealso cref="ChronoLocalDate"/>.
	''' <p>
	''' Ensure that the discussion in {@code ChronoLocalDate} has been read and understood
	''' before using this interface.
	''' 
	''' @implSpec
	''' This interface must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' Subclasses should be Serializable wherever possible.
	''' </summary>
	''' @param <D> the concrete type for the date of this date-time
	''' @since 1.8 </param>
	Public Interface ChronoZonedDateTime(Of D As ChronoLocalDate)
		Inherits java.time.temporal.Temporal, Comparable(Of ChronoZonedDateTime(Of JavaToDotNetGenericWildcard))

		''' <summary>
		''' Gets a comparator that compares {@code ChronoZonedDateTime} in
		''' time-line order ignoring the chronology.
		''' <p>
		''' This comparator differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying instant and not the chronology.
		''' This allows dates in different calendar systems to be compared based
		''' on the position of the date-time on the instant time-line.
		''' The underlying comparison is equivalent to comparing the epoch-second and nano-of-second.
		''' </summary>
		''' <returns> a comparator that compares in time-line order ignoring the chronology </returns>
		''' <seealso cref= #isAfter </seealso>
		''' <seealso cref= #isBefore </seealso>
		''' <seealso cref= #isEqual </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function timeLineOrder() As IComparer(Of ChronoZonedDateTime(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return AbstractChronology.INSTANT_ORDER;

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ChronoZonedDateTime} from a temporal object.
		''' <p>
		''' This creates a zoned date-time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ChronoZonedDateTime}.
		''' <p>
		''' The conversion extracts and combines the chronology, date, time and zone
		''' from the temporal object. The behavior is equivalent to using
		''' <seealso cref="Chronology#zonedDateTime(TemporalAccessor)"/> with the extracted chronology.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ChronoZonedDateTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code ChronoZonedDateTime} </exception>
		''' <seealso cref= Chronology#zonedDateTime(TemporalAccessor) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As ChronoZonedDateTime(Of ?)
			Sub [New](temporal   ChronoZonedDateTime As instanceof)
				Sub [New](Of T1)(   As ChronoZonedDateTime(Of T1))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(temporal, "temporal");
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Chronology chrono = temporal.query(java.time.temporal.TemporalQueries.chronology());
			Sub [New](chrono ==   [Nothing] As )
				throw Function java.time.DateTimeException("Unable to obtain ChronoZonedDateTime from TemporalAccessor: " & temporal.getClass()    As ) As New
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return chrono.zonedDateTime(temporal);

		'-----------------------------------------------------------------------
		default Overrides Function range(  field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			Sub [New](field   java.time.temporal.ChronoField As instanceof)
				Sub [New](field == INSTANT_SECONDS || field ==   OFFSET_SECONDS As )
					Function field.range() As [Return]
				Function toLocalDateTime() As [Return]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return field.rangeRefinedBy(Me);

		default Overrides Function [get](  field As java.time.temporal.TemporalField) As Integer
			Sub [New](field   java.time.temporal.ChronoField As instanceof)
				Sub [New]((java.time.temporal.ChronoField)   field As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					case INSTANT_SECONDS:
						throw Function java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'InstantSeconds' for get() method, use getLong() instead"    As ) As New
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					case OFFSET_SECONDS:
						ReadOnly Property offset As [Return]
				Function toLocalDateTime() As [Return]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return outerInstance.get(field);

		default Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
			Sub [New](field   java.time.temporal.ChronoField As instanceof)
				Sub [New]((java.time.temporal.ChronoField)   field As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					case INSTANT_SECONDS:
						Function toEpochSecond() As [Return]
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					case OFFSET_SECONDS:
						ReadOnly Property offset As [Return]
				Function toLocalDateTime() As [Return]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return field.getFrom(Me);

		''' <summary>
		''' Gets the local date part of this date-time.
		''' <p>
		''' This returns a local date with the same year, month and day
		''' as this date-time.
		''' </summary>
		''' <returns> the date part of this date-time, not null </returns>
		default Function toLocalDate() As D
			Function toLocalDateTime() As [Return]

		''' <summary>
		''' Gets the local time part of this date-time.
		''' <p>
		''' This returns a local time with the same hour, minute, second and
		''' nanosecond as this date-time.
		''' </summary>
		''' <returns> the time part of this date-time, not null </returns>
		default Function toLocalTime() As java.time.LocalTime
			Function toLocalDateTime() As [Return]

		''' <summary>
		''' Gets the local date-time part of this date-time.
		''' <p>
		''' This returns a local date with the same year, month and day
		''' as this date-time.
		''' </summary>
		''' <returns> the local date-time part of this date-time, not null </returns>
		Function toLocalDateTime() As ChronoLocalDateTime(Of D)

		''' <summary>
		''' Gets the chronology of this date-time.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		''' </summary>
		''' <returns> the chronology, not null </returns>
		ReadOnly Property default chronology As Chronology
			Function toLocalDate() As [Return]

		''' <summary>
		''' Gets the zone offset, such as '+01:00'.
		''' <p>
		''' This is the offset of the local date-time from UTC/Greenwich.
		''' </summary>
		''' <returns> the zone offset, not null </returns>
		ReadOnly Property offset As java.time.ZoneOffset

		''' <summary>
		''' Gets the zone ID, such as 'Europe/Paris'.
		''' <p>
		''' This returns the stored time-zone id used to determine the time-zone rules.
		''' </summary>
		''' <returns> the zone ID, not null </returns>
		ReadOnly Property zone As java.time.ZoneId

		'-----------------------------------------------------------------------
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
		''' <returns> a {@code ChronoZonedDateTime} based on this date-time with the earlier offset, not null </returns>
		''' <exception cref="DateTimeException"> if no rules can be found for the zone </exception>
		''' <exception cref="DateTimeException"> if no rules are valid for this date-time </exception>
		Function withEarlierOffsetAtOverlap() As ChronoZonedDateTime(Of D)

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
		''' <returns> a {@code ChronoZonedDateTime} based on this date-time with the later offset, not null </returns>
		''' <exception cref="DateTimeException"> if no rules can be found for the zone </exception>
		''' <exception cref="DateTimeException"> if no rules are valid for this date-time </exception>
		Function withLaterOffsetAtOverlap() As ChronoZonedDateTime(Of D)

		''' <summary>
		''' Returns a copy of this date-time with a different time-zone,
		''' retaining the local date-time if possible.
		''' <p>
		''' This method changes the time-zone and retains the local date-time.
		''' The local date-time is only changed if it is invalid for the new zone.
		''' <p>
		''' To change the zone and adjust the local date-time,
		''' use <seealso cref="#withZoneSameInstant(ZoneId)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="zone">  the time-zone to change to, not null </param>
		''' <returns> a {@code ChronoZonedDateTime} based on this date-time with the requested zone, not null </returns>
		Function withZoneSameLocal(  zone As java.time.ZoneId) As ChronoZonedDateTime(Of D)

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
		''' <returns> a {@code ChronoZonedDateTime} based on this date-time with the requested zone, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Function withZoneSameInstant(  zone As java.time.ZoneId) As ChronoZonedDateTime(Of D)

		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if the specified field can be queried on this date-time.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' The set of supported fields is defined by the chronology and normally includes
		''' all {@code ChronoField} fields.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field can be queried, false if not </returns>
		Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to or subtracted from this date-time.
		''' If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		''' <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		''' <p>
		''' The set of supported units is defined by the chronology and normally includes
		''' all {@code ChronoUnit} units except {@code FOREVER}.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.isSupportedBy(Temporal)}
		''' passing {@code this} as the argument.
		''' Whether the unit is supported is determined by the unit.
		''' </summary>
		''' <param name="unit">  the unit to check, null returns false </param>
		''' <returns> true if the unit can be added/subtracted, false if not </returns>
		default Overrides Function isSupported(  unit As java.time.temporal.TemporalUnit) As Boolean
			Sub [New](unit   java.time.temporal.ChronoUnit As instanceof)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return unit != FOREVER;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return unit != Nothing && unit.isSupportedBy(Me);

		'-----------------------------------------------------------------------
		' override for covariant return type
		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function [with](  adjuster As java.time.temporal.TemporalAdjuster) As ChronoZonedDateTime(Of D)
			Function ChronoZonedDateTimeImpl.ensureValid(getChronology()    As , outerInstance.with(adjuster)    As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Overrides Function [with](  field As java.time.temporal.TemporalField,   newValue As Long) As ChronoZonedDateTime(Of D)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function plus(  amount As java.time.temporal.TemporalAmount) As ChronoZonedDateTime(Of D)
			Function ChronoZonedDateTimeImpl.ensureValid(getChronology()    As , outerInstance.plus(amount)    As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Overrides Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As ChronoZonedDateTime(Of D)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function minus(  amount As java.time.temporal.TemporalAmount) As ChronoZonedDateTime(Of D)
			Function ChronoZonedDateTimeImpl.ensureValid(getChronology()    As , outerInstance.minus(amount)    As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function minus(  amountToSubtract As Long,   unit As java.time.temporal.TemporalUnit) As ChronoZonedDateTime(Of D)
			Function ChronoZonedDateTimeImpl.ensureValid(getChronology()    As , outerInstance.minus(amountToSubtract, unit)    As ) As [Return]

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
		default Overrides Function query(  query As java.time.temporal.TemporalQuery(Of R)) As R(Of R)
			Sub [New](query == java.time.temporal.TemporalQueries.zone() || query == java.time.temporal.TemporalQueries.zoneId()    As )
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) getZone();
			Function [if](query == java.time.temporal.TemporalQueries.offset()    As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) getOffset();
			Function [if](query == java.time.temporal.TemporalQueries.localTime()    As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) toLocalTime();
			Function [if](query == java.time.temporal.TemporalQueries.chronology()    As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) getChronology();
			Function [if](query == java.time.temporal.TemporalQueries.precision()    As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) NANOS;
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return query.queryFrom(Me);

		''' <summary>
		''' Formats this date-time using the specified formatter.
		''' <p>
		''' This date-time will be passed to the formatter to produce a string.
		''' <p>
		''' The default implementation must behave as follows:
		''' <pre>
		'''  return formatter.format(this);
		''' </pre>
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted date-time string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		default Function format(  formatter As java.time.format.DateTimeFormatter) As String
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(formatter, "formatter");
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return formatter.format(Me);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this date-time to an {@code Instant}.
		''' <p>
		''' This returns an {@code Instant} representing the same point on the
		''' time-line as this date-time. The calculation combines the
		''' <seealso cref="#toLocalDateTime() local date-time"/> and
		''' <seealso cref="#getOffset() offset"/>.
		''' </summary>
		''' <returns> an {@code Instant} representing the same instant, not null </returns>
		default Function toInstant() As java.time.Instant
			Function java.time.Instant.ofEpochSecond(toEpochSecond()    As , toLocalTime().getNano()    As ) As [Return]

		''' <summary>
		''' Converts this date-time to the number of seconds from the epoch
		''' of 1970-01-01T00:00:00Z.
		''' <p>
		''' This uses the <seealso cref="#toLocalDateTime() local date-time"/> and
		''' <seealso cref="#getOffset() offset"/> to calculate the epoch-second value,
		''' which is the number of elapsed seconds from 1970-01-01T00:00:00Z.
		''' Instants on the time-line after the epoch are positive, earlier are negative.
		''' </summary>
		''' <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		default Function toEpochSecond() As Long
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long epochDay = toLocalDate().toEpochDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long secs = epochDay * 86400 + toLocalTime().toSecondOfDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			secs -= getOffset().getTotalSeconds();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return secs;

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this date-time to another date-time, including the chronology.
		''' <p>
		''' The comparison is based first on the instant, then on the local date-time,
		''' then on the zone ID, then on the chronology.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' If all the date-time objects being compared are in the same chronology, then the
		''' additional chronology stage is not required.
		''' <p>
		''' This default implementation performs the comparison defined above.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		default Overrides Function compareTo(Of T1)(  other As ChronoZonedDateTime(Of T1)) As Integer
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			int cmp = java.lang.[Long].compare(toEpochSecond(), other.toEpochSecond());
			Sub [New](cmp ==   0 As )
				cmp = Function toLocalTime() As 
				Sub [New](cmp ==   0 As )
					cmp = Function toLocalDateTime() As 
					Sub [New](cmp ==   0 As )
						ReadOnly Property cmp = zone As
						Sub [New](cmp ==   0 As )
							ReadOnly Property cmp = chronology As
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return cmp;

		''' <summary>
		''' Checks if the instant of this date-time is before that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().isBefore(dateTime2.toInstant());}.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-second
		''' and nano-of-second.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this point is before the specified date-time </returns>
		default Function isBefore(Of T1)(  other As ChronoZonedDateTime(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long thisEpochSec = toEpochSecond();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long otherEpochSec = other.toEpochSecond();
			Return thisEpochSec < Function ||(thisEpochSec == otherEpochSec && toLocalTime().getNano() < other.toLocalTime().getNano()    As ) As otherEpochSec

		''' <summary>
		''' Checks if the instant of this date-time is after that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().isAfter(dateTime2.toInstant());}.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-second
		''' and nano-of-second.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this is after the specified date-time </returns>
		default Function isAfter(Of T1)(  other As ChronoZonedDateTime(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long thisEpochSec = toEpochSecond();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long otherEpochSec = other.toEpochSecond();
			Return thisEpochSec > Function ||(thisEpochSec == otherEpochSec && toLocalTime().getNano() > other.toLocalTime().getNano()    As ) As otherEpochSec

		''' <summary>
		''' Checks if the instant of this date-time is equal to that of the specified date-time.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> and <seealso cref="#equals"/>
		''' in that it only compares the instant of the date-time. This is equivalent to using
		''' {@code dateTime1.toInstant().equals(dateTime2.toInstant());}.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-second
		''' and nano-of-second.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if the instant equals the instant of the specified date-time </returns>
		default Function isEqual(Of T1)(  other As ChronoZonedDateTime(Of T1)) As Boolean
			Function toEpochSecond() As [Return]

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this date-time is equal to another date-time.
		''' <p>
		''' The comparison is based on the offset date-time and the zone.
		''' To compare for the same instant on the time-line, use <seealso cref="#compareTo"/>.
		''' Only objects of type {@code ChronoZonedDateTime} are compared, other types return false.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date-time </returns>
		Overrides Function Equals(  obj As Object) As Boolean

		''' <summary>
		''' A hash code for this date-time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Overrides Function GetHashCode() As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date-time as a {@code String}.
		''' <p>
		''' The output will include the full zoned date-time.
		''' </summary>
		''' <returns> a string representation of this date-time, not null </returns>
		Overrides Function ToString() As String

	End Interface

End Namespace