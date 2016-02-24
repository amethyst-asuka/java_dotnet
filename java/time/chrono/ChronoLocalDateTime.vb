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
	''' A date-time without a time-zone in an arbitrary chronology, intended
	''' for advanced globalization use cases.
	''' <p>
	''' <b>Most applications should declare method signatures, fields and variables
	''' as <seealso cref="LocalDateTime"/>, not this interface.</b>
	''' <p>
	''' A {@code ChronoLocalDateTime} is the abstract representation of a local date-time
	''' where the {@code Chronology chronology}, or calendar system, is pluggable.
	''' The date-time is defined in terms of fields expressed by <seealso cref="TemporalField"/>,
	''' where most common implementations are defined in <seealso cref="ChronoField"/>.
	''' The chronology defines how the calendar system operates and the meaning of
	''' the standard fields.
	''' 
	''' <h3>When to use this interface</h3>
	''' The design of the API encourages the use of {@code LocalDateTime} rather than this
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
	Public Interface ChronoLocalDateTime(Of D As ChronoLocalDate)
		Inherits java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of ChronoLocalDateTime(Of JavaToDotNetGenericWildcard))

		''' <summary>
		''' Gets a comparator that compares {@code ChronoLocalDateTime} in
		''' time-line order ignoring the chronology.
		''' <p>
		''' This comparator differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying date-time and not the chronology.
		''' This allows dates in different calendar systems to be compared based
		''' on the position of the date-time on the local time-line.
		''' The underlying comparison is equivalent to comparing the epoch-day and nano-of-day.
		''' </summary>
		''' <returns> a comparator that compares in time-line order ignoring the chronology </returns>
		''' <seealso cref= #isAfter </seealso>
		''' <seealso cref= #isBefore </seealso>
		''' <seealso cref= #isEqual </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function timeLineOrder() As IComparer(Of ChronoLocalDateTime(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return AbstractChronology.DATE_TIME_ORDER;

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ChronoLocalDateTime} from a temporal object.
		''' <p>
		''' This obtains a local date-time based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ChronoLocalDateTime}.
		''' <p>
		''' The conversion extracts and combines the chronology and the date-time
		''' from the temporal object. The behavior is equivalent to using
		''' <seealso cref="Chronology#localDateTime(TemporalAccessor)"/> with the extracted chronology.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ChronoLocalDateTime::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code ChronoLocalDateTime} </exception>
		''' <seealso cref= Chronology#localDateTime(TemporalAccessor) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDateTime(Of ?)
			Sub [New](temporal ByVal ChronoLocalDateTime As instanceof)
				Sub [New](Of T1)(ByVal  As ChronoLocalDateTime(Of T1))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(temporal, "temporal");
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Chronology chrono = temporal.query(java.time.temporal.TemporalQueries.chronology());
			Sub [New](chrono == ByVal [Nothing] As )
				throw Function java.time.DateTimeException("Unable to obtain ChronoLocalDateTime from TemporalAccessor: " & temporal.getClass() ByVal  As ) As New
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return chrono.localDateTime(temporal);

		'-----------------------------------------------------------------------
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
		''' Gets the local date part of this date-time.
		''' <p>
		''' This returns a local date with the same year, month and day
		''' as this date-time.
		''' </summary>
		''' <returns> the date part of this date-time, not null </returns>
		Function toLocalDate() As D

		''' <summary>
		''' Gets the local time part of this date-time.
		''' <p>
		''' This returns a local time with the same hour, minute, second and
		''' nanosecond as this date-time.
		''' </summary>
		''' <returns> the time part of this date-time, not null </returns>
		Function toLocalTime() As java.time.LocalTime

		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if the specified field can be queried on this date-time.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' The set of supported fields is defined by the chronology and normally includes
		''' all {@code ChronoField} date and time fields.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field can be queried, false if not </returns>
		Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean

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
		default Overrides Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean
			Sub [New](unit ByVal java.time.temporal.ChronoUnit As instanceof)
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
		default Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As ChronoLocalDateTime(Of D)
			Function ChronoLocalDateTimeImpl.ensureValid(getChronology() ByVal  As , outerInstance.with(adjuster) ByVal  As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As ChronoLocalDateTime(Of D)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function plus(ByVal amount As java.time.temporal.TemporalAmount) As ChronoLocalDateTime(Of D)
			Function ChronoLocalDateTimeImpl.ensureValid(getChronology() ByVal  As , outerInstance.plus(amount) ByVal  As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As ChronoLocalDateTime(Of D)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function minus(ByVal amount As java.time.temporal.TemporalAmount) As ChronoLocalDateTime(Of D)
			Function ChronoLocalDateTimeImpl.ensureValid(getChronology() ByVal  As , outerInstance.minus(amount) ByVal  As ) As [Return]

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		default Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As ChronoLocalDateTime(Of D)
			Function ChronoLocalDateTimeImpl.ensureValid(getChronology() ByVal  As , outerInstance.minus(amountToSubtract, unit) ByVal  As ) As [Return]

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
		default Overrides Function query(ByVal query As java.time.temporal.TemporalQuery(Of R)) As R(Of R)
			Sub [New](query == java.time.temporal.TemporalQueries.zoneId() || query == java.time.temporal.TemporalQueries.zone() || query == java.time.temporal.TemporalQueries.offset() ByVal  As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return Nothing;
			Function [if](query == java.time.temporal.TemporalQueries.localTime() ByVal  As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) toLocalTime();
			Function [if](query == java.time.temporal.TemporalQueries.chronology() ByVal  As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) getChronology();
			Function [if](query == java.time.temporal.TemporalQueries.precision() ByVal  As ) As else
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) NANOS;
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return query.queryFrom(Me);

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
		default Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return temporal.with(EPOCH_DAY, toLocalDate().toEpochDay()).with(NANO_OF_DAY, toLocalTime().toNanoOfDay());

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
		default Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(formatter, "formatter");
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return formatter.format(Me);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this time with a time-zone to create a {@code ChronoZonedDateTime}.
		''' <p>
		''' This returns a {@code ChronoZonedDateTime} formed from this date-time at the
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
		''' <seealso cref="ChronoZonedDateTime#withLaterOffsetAtOverlap()"/> on the result of this method.
		''' </summary>
		''' <param name="zone">  the time-zone to use, not null </param>
		''' <returns> the zoned date-time formed from this date-time, not null </returns>
		Function atZone(ByVal zone As java.time.ZoneId) As ChronoZonedDateTime(Of D)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Converts this date-time to an {@code Instant}.
		''' <p>
		''' This combines this local date-time and the specified offset to form
		''' an {@code Instant}.
		''' <p>
		''' This default implementation calculates from the epoch-day of the date and the
		''' second-of-day of the time.
		''' </summary>
		''' <param name="offset">  the offset to use for the conversion, not null </param>
		''' <returns> an {@code Instant} representing the same instant, not null </returns>
		default Function toInstant(ByVal offset As java.time.ZoneOffset) As java.time.Instant
			Function java.time.Instant.ofEpochSecond(toEpochSecond(offset) ByVal  As , toLocalTime().getNano() ByVal  As ) As [Return]

		''' <summary>
		''' Converts this date-time to the number of seconds from the epoch
		''' of 1970-01-01T00:00:00Z.
		''' <p>
		''' This combines this local date-time and the specified offset to calculate the
		''' epoch-second value, which is the number of elapsed seconds from 1970-01-01T00:00:00Z.
		''' Instants on the time-line after the epoch are positive, earlier are negative.
		''' <p>
		''' This default implementation calculates from the epoch-day of the date and the
		''' second-of-day of the time.
		''' </summary>
		''' <param name="offset">  the offset to use for the conversion, not null </param>
		''' <returns> the number of seconds from the epoch of 1970-01-01T00:00:00Z </returns>
		default Function toEpochSecond(ByVal offset As java.time.ZoneOffset) As Long
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(offset, "offset");
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long epochDay = toLocalDate().toEpochDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long secs = epochDay * 86400 + toLocalTime().toSecondOfDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			secs -= offset.getTotalSeconds();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return secs;

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this date-time to another date-time, including the chronology.
		''' <p>
		''' The comparison is based first on the underlying time-line date-time, then
		''' on the chronology.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' For example, the following is the comparator order:
		''' <ol>
		''' <li>{@code 2012-12-03T12:00 (ISO)}</li>
		''' <li>{@code 2012-12-04T12:00 (ISO)}</li>
		''' <li>{@code 2555-12-04T12:00 (ThaiBuddhist)}</li>
		''' <li>{@code 2012-12-05T12:00 (ISO)}</li>
		''' </ol>
		''' Values #2 and #3 represent the same date-time on the time-line.
		''' When two values represent the same date-time, the chronology ID is compared to distinguish them.
		''' This step is needed to make the ordering "consistent with equals".
		''' <p>
		''' If all the date-time objects being compared are in the same chronology, then the
		''' additional chronology stage is not required and only the local date-time is used.
		''' <p>
		''' This default implementation performs the comparison defined above.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		default Overrides Function compareTo(Of T1)(ByVal other As ChronoLocalDateTime(Of T1)) As Integer
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			int cmp = toLocalDate().compareTo(other.toLocalDate());
			Sub [New](cmp == ByVal 0 As )
				cmp = Function toLocalTime() As 
				Sub [New](cmp == ByVal 0 As )
					ReadOnly Property cmp = chronology As
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return cmp;

		''' <summary>
		''' Checks if this date-time is after the specified date-time ignoring the chronology.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying date-time and not the chronology.
		''' This allows dates in different calendar systems to be compared based
		''' on the time-line position.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-day
		''' and nano-of-day.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this is after the specified date-time </returns>
		default Function isAfter(Of T1)(ByVal other As ChronoLocalDateTime(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long thisEpDay = Me.toLocalDate().toEpochDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long otherEpDay = other.toLocalDate().toEpochDay();
			Return thisEpDay > Function ||(thisEpDay == otherEpDay && Me.toLocalTime().toNanoOfDay() > other.toLocalTime().toNanoOfDay() ByVal  As ) As otherEpDay

		''' <summary>
		''' Checks if this date-time is before the specified date-time ignoring the chronology.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying date-time and not the chronology.
		''' This allows dates in different calendar systems to be compared based
		''' on the time-line position.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-day
		''' and nano-of-day.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if this is before the specified date-time </returns>
		default Function isBefore(Of T1)(ByVal other As ChronoLocalDateTime(Of T1)) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long thisEpDay = Me.toLocalDate().toEpochDay();
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long otherEpDay = other.toLocalDate().toEpochDay();
			Return thisEpDay < Function ||(thisEpDay == otherEpDay && Me.toLocalTime().toNanoOfDay() < other.toLocalTime().toNanoOfDay() ByVal  As ) As otherEpDay

		''' <summary>
		''' Checks if this date-time is equal to the specified date-time ignoring the chronology.
		''' <p>
		''' This method differs from the comparison in <seealso cref="#compareTo"/> in that it
		''' only compares the underlying date and time and not the chronology.
		''' This allows date-times in different calendar systems to be compared based
		''' on the time-line position.
		''' <p>
		''' This default implementation performs the comparison based on the epoch-day
		''' and nano-of-day.
		''' </summary>
		''' <param name="other">  the other date-time to compare to, not null </param>
		''' <returns> true if the underlying date-time is equal to the specified date-time on the timeline </returns>
		default Function isEqual(Of T1)(ByVal other As ChronoLocalDateTime(Of T1)) As Boolean
			' Do the time check first, it is cheaper than computing EPOCH day.
			Function Me.toLocalTime() As [Return]

		''' <summary>
		''' Checks if this date-time is equal to another date-time, including the chronology.
		''' <p>
		''' Compares this date-time with another ensuring that the date-time and chronology are the same.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date </returns>
		Overrides Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' A hash code for this date-time.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Overrides Function GetHashCode() As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date-time as a {@code String}.
		''' <p>
		''' The output will include the full local date-time.
		''' </summary>
		''' <returns> a string representation of this date-time, not null </returns>
		Overrides Function ToString() As String

	End Interface

End Namespace