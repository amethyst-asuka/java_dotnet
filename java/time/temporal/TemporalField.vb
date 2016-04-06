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
' * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.temporal


	''' <summary>
	''' A field of date-time, such as month-of-year or hour-of-minute.
	''' <p>
	''' Date and time is expressed using fields which partition the time-line into something
	''' meaningful for humans. Implementations of this interface represent those fields.
	''' <p>
	''' The most commonly used units are defined in <seealso cref="ChronoField"/>.
	''' Further fields are supplied in <seealso cref="IsoFields"/>, <seealso cref="WeekFields"/> and <seealso cref="JulianFields"/>.
	''' Fields can also be written by application code by implementing this interface.
	''' <p>
	''' The field works using double dispatch. Client code calls methods on a date-time like
	''' {@code LocalDateTime} which check if the field is a {@code ChronoField}.
	''' If it is, then the date-time must handle it.
	''' Otherwise, the method call is re-dispatched to the matching method in this interface.
	''' 
	''' @implSpec
	''' This interface must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' Implementations should be {@code Serializable} where possible.
	''' An enum is as effective implementation choice.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface TemporalField

		''' <summary>
		''' Gets the display name for the field in the requested locale.
		''' <p>
		''' If there is no display name for the locale then a suitable default must be returned.
		''' <p>
		''' The default implementation must check the locale is not null
		''' and return {@code toString()}.
		''' </summary>
		''' <param name="locale">  the locale to use, not null </param>
		''' <returns> the display name for the locale or a suitable default, not null </returns>
		default Function getDisplayName(  locale As java.util.Locale) As String
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(locale, "locale");
			Function toString() As [Return]

		''' <summary>
		''' Gets the unit that the field is measured in.
		''' <p>
		''' The unit of the field is the period that varies within the range.
		''' For example, in the field 'MonthOfYear', the unit is 'Months'.
		''' See also <seealso cref="#getRangeUnit()"/>.
		''' </summary>
		''' <returns> the unit defining the base unit of the field, not null </returns>
		ReadOnly Property baseUnit As TemporalUnit

		''' <summary>
		''' Gets the range that the field is bound by.
		''' <p>
		''' The range of the field is the period that the field varies within.
		''' For example, in the field 'MonthOfYear', the range is 'Years'.
		''' See also <seealso cref="#getBaseUnit()"/>.
		''' <p>
		''' The range is never null. For example, the 'Year' field is shorthand for
		''' 'YearOfForever'. It therefore has a unit of 'Years' and a range of 'Forever'.
		''' </summary>
		''' <returns> the unit defining the range of the field, not null </returns>
		ReadOnly Property rangeUnit As TemporalUnit

		''' <summary>
		''' Gets the range of valid values for the field.
		''' <p>
		''' All fields can be expressed as a {@code long}  java.lang.[Integer].
		''' This method returns an object that describes the valid range for that value.
		''' This method is generally only applicable to the ISO-8601 calendar system.
		''' <p>
		''' Note that the result only describes the minimum and maximum valid values
		''' and it is important not to read too much into them. For example, there
		''' could be values within the range that are invalid for the field.
		''' </summary>
		''' <returns> the range of valid values for the field, not null </returns>
		Function range() As ValueRange

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this field represents a component of a date.
		''' <p>
		''' A field is date-based if it can be derived from
		''' <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>.
		''' Note that it is valid for both {@code isDateBased()} and {@code isTimeBased()}
		''' to return false, such as when representing a field like minute-of-week.
		''' </summary>
		''' <returns> true if this field is a component of a date </returns>
		ReadOnly Property dateBased As Boolean

		''' <summary>
		''' Checks if this field represents a component of a time.
		''' <p>
		''' A field is time-based if it can be derived from
		''' <seealso cref="ChronoField#NANO_OF_DAY NANO_OF_DAY"/>.
		''' Note that it is valid for both {@code isDateBased()} and {@code isTimeBased()}
		''' to return false, such as when representing a field like minute-of-week.
		''' </summary>
		''' <returns> true if this field is a component of a time </returns>
		ReadOnly Property timeBased As Boolean

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this field is supported by the temporal object.
		''' <p>
		''' This determines whether the temporal accessor supports this field.
		''' If this returns false, then the temporal cannot be queried for this field.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method directly.
		''' The second is to use <seealso cref="TemporalAccessor#isSupported(TemporalField)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisField.isSupportedBy(temporal);
		'''   temporal = temporal.isSupported(thisField);
		''' </pre>
		''' It is recommended to use the second approach, {@code isSupported(TemporalField)},
		''' as it is a lot clearer to read in code.
		''' <p>
		''' Implementations should determine whether they are supported using the fields
		''' available in <seealso cref="ChronoField"/>.
		''' </summary>
		''' <param name="temporal">  the temporal object to query, not null </param>
		''' <returns> true if the date-time can be queried for this field, false if not </returns>
		Function isSupportedBy(  temporal As TemporalAccessor) As Boolean

		''' <summary>
		''' Get the range of valid values for this field using the temporal object to
		''' refine the result.
		''' <p>
		''' This uses the temporal object to find the range of valid values for the field.
		''' This is similar to <seealso cref="#range()"/>, however this method refines the result
		''' using the temporal. For example, if the field is {@code DAY_OF_MONTH} the
		''' {@code range} method is not accurate as there are four possible month lengths,
		''' 28, 29, 30 and 31 days. Using this method with a date allows the range to be
		''' accurate, returning just one of those four options.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method directly.
		''' The second is to use <seealso cref="TemporalAccessor#range(TemporalField)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisField.rangeRefinedBy(temporal);
		'''   temporal = temporal.range(thisField);
		''' </pre>
		''' It is recommended to use the second approach, {@code range(TemporalField)},
		''' as it is a lot clearer to read in code.
		''' <p>
		''' Implementations should perform any queries or calculations using the fields
		''' available in <seealso cref="ChronoField"/>.
		''' If the field is not supported an {@code UnsupportedTemporalTypeException} must be thrown.
		''' </summary>
		''' <param name="temporal">  the temporal object used to refine the result, not null </param>
		''' <returns> the range of valid values for this field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported by the temporal </exception>
		Function rangeRefinedBy(  temporal As TemporalAccessor) As ValueRange

		''' <summary>
		''' Gets the value of this field from the specified temporal object.
		''' <p>
		''' This queries the temporal object for the value of this field.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method directly.
		''' The second is to use <seealso cref="TemporalAccessor#getLong(TemporalField)"/>
		''' (or <seealso cref="TemporalAccessor#get(TemporalField)"/>):
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisField.getFrom(temporal);
		'''   temporal = temporal.getLong(thisField);
		''' </pre>
		''' It is recommended to use the second approach, {@code getLong(TemporalField)},
		''' as it is a lot clearer to read in code.
		''' <p>
		''' Implementations should perform any queries or calculations using the fields
		''' available in <seealso cref="ChronoField"/>.
		''' If the field is not supported an {@code UnsupportedTemporalTypeException} must be thrown.
		''' </summary>
		''' <param name="temporal">  the temporal object to query, not null </param>
		''' <returns> the value of this field, not null </returns>
		''' <exception cref="DateTimeException"> if a value for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported by the temporal </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Function getFrom(  temporal As TemporalAccessor) As Long

		''' <summary>
		''' Returns a copy of the specified temporal object with the value of this field set.
		''' <p>
		''' This returns a new temporal object based on the specified one with the value for
		''' this field changed. For example, on a {@code LocalDate}, this could be used to
		''' set the year, month or day-of-month.
		''' The returned object has the same observable type as the specified object.
		''' <p>
		''' In some cases, changing a field is not fully defined. For example, if the target object is
		''' a date representing the 31st January, then changing the month to February would be unclear.
		''' In cases like this, the implementation is responsible for resolving the result.
		''' Typically it will choose the previous valid date, which would be the last valid
		''' day of February in this example.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method directly.
		''' The second is to use <seealso cref="Temporal#with(TemporalField, long)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisField.adjustInto(temporal);
		'''   temporal = temporal.with(thisField);
		''' </pre>
		''' It is recommended to use the second approach, {@code with(TemporalField)},
		''' as it is a lot clearer to read in code.
		''' <p>
		''' Implementations should perform any queries or calculations using the fields
		''' available in <seealso cref="ChronoField"/>.
		''' If the field is not supported an {@code UnsupportedTemporalTypeException} must be thrown.
		''' <p>
		''' Implementations must not alter the specified temporal object.
		''' Instead, an adjusted copy of the original must be returned.
		''' This provides equivalent, safe behavior for immutable and mutable implementations.
		''' </summary>
		''' @param <R>  the type of the Temporal object </param>
		''' <param name="temporal"> the temporal object to adjust, not null </param>
		''' <param name="newValue"> the new value of the field </param>
		''' <returns> the adjusted temporal object, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported by the temporal </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		 Function adjustInto(Of R As Temporal)(  temporal As R,   newValue As Long) As R

		''' <summary>
		''' Resolves this field to provide a simpler alternative or a date.
		''' <p>
		''' This method is invoked during the resolve phase of parsing.
		''' It is designed to allow application defined fields to be simplified into
		''' more standard fields, such as those on {@code ChronoField}, or into a date.
		''' <p>
		''' Applications should not normally invoke this method directly.
		''' 
		''' @implSpec
		''' If an implementation represents a field that can be simplified, or
		''' combined with others, then this method must be implemented.
		''' <p>
		''' The specified map contains the current state of the parse.
		''' The map is mutable and must be mutated to resolve the field and
		''' any related fields. This method will only be invoked during parsing
		''' if the map contains this field, and implementations should therefore
		''' assume this field is present.
		''' <p>
		''' Resolving a field will consist of looking at the value of this field,
		''' and potentially other fields, and either updating the map with a
		''' simpler value, such as a {@code ChronoField}, or returning a
		''' complete {@code ChronoLocalDate}. If a resolve is successful,
		''' the code must remove all the fields that were resolved from the map,
		''' including this field.
		''' <p>
		''' For example, the {@code IsoFields} class contains the quarter-of-year
		''' and day-of-quarter fields. The implementation of this method in that class
		''' resolves the two fields plus the <seealso cref="ChronoField#YEAR YEAR"/> into a
		''' complete {@code LocalDate}. The resolve method will remove all three
		''' fields from the map before returning the {@code LocalDate}.
		''' <p>
		''' A partially complete temporal is used to allow the chronology and zone
		''' to be queried. In general, only the chronology will be needed.
		''' Querying items other than the zone or chronology is undefined and
		''' must not be relied on.
		''' The behavior of other methods such as {@code get}, {@code getLong},
		''' {@code range} and {@code isSupported} is unpredictable and the results undefined.
		''' <p>
		''' If resolution should be possible, but the data is invalid, the resolver
		''' style should be used to determine an appropriate level of leniency, which
		''' may require throwing a {@code DateTimeException} or {@code ArithmeticException}.
		''' If no resolution is possible, the resolve method must return null.
		''' <p>
		''' When resolving time fields, the map will be altered and null returned.
		''' When resolving date fields, the date is normally returned from the method,
		''' with the map altered to remove the resolved fields. However, it would also
		''' be acceptable for the date fields to be resolved into other {@code ChronoField}
		''' instances that can produce a date, such as {@code EPOCH_DAY}.
		''' <p>
		''' Not all {@code TemporalAccessor} implementations are accepted as return values.
		''' Implementations that call this method must accept {@code ChronoLocalDate},
		''' {@code ChronoLocalDateTime}, {@code ChronoZonedDateTime} and {@code LocalTime}.
		''' <p>
		''' The default implementation must return null.
		''' </summary>
		''' <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		''' <param name="partialTemporal">  the partially complete temporal to query for zone and
		'''  chronology; querying for other things is undefined and not recommended, not null </param>
		''' <param name="resolverStyle">  the requested type of resolve, not null </param>
		''' <returns> the resolved temporal object; null if resolving only
		'''  changed the map, or no resolve occurred </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		''' <exception cref="DateTimeException"> if resolving results in an error. This must not be thrown
		'''  by querying a field on the temporal without first checking if it is supported </exception>
		default Function resolve(  fieldValues As IDictionary(Of TemporalField, Long?),   partialTemporal As TemporalAccessor,   resolverStyle As java.time.format.ResolverStyle) As TemporalAccessor
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return Nothing;

		''' <summary>
		''' Gets a descriptive name for the field.
		''' <p>
		''' The should be of the format 'BaseOfRange', such as 'MonthOfYear',
		''' unless the field has a range of {@code FOREVER}, when only
		''' the base unit is mentioned, such as 'Year' or 'Era'.
		''' </summary>
		''' <returns> the name of the field, not null </returns>
		Overrides Function ToString() As String


	End Interface

End Namespace