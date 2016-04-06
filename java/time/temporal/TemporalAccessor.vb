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
	''' Framework-level interface defining read-only access to a temporal object,
	''' such as a date, time, offset or some combination of these.
	''' <p>
	''' This is the base interface type for date, time and offset objects.
	''' It is implemented by those classes that can provide information
	''' as <seealso cref="TemporalField fields"/> or <seealso cref="TemporalQuery queries"/>.
	''' <p>
	''' Most date and time information can be represented as a number.
	''' These are modeled using {@code TemporalField} with the number held using
	''' a {@code long} to handle large values. Year, month and day-of-month are
	''' simple examples of fields, but they also include instant and offsets.
	''' See <seealso cref="ChronoField"/> for the standard set of fields.
	''' <p>
	''' Two pieces of date/time information cannot be represented by numbers,
	''' the <seealso cref="java.time.chrono.Chronology chronology"/> and the
	''' <seealso cref="java.time.ZoneId time-zone"/>.
	''' These can be accessed via <seealso cref="#query(TemporalQuery) queries"/> using
	''' the static methods defined on <seealso cref="TemporalQuery"/>.
	''' <p>
	''' A sub-interface, <seealso cref="Temporal"/>, extends this definition to one that also
	''' supports adjustment and manipulation on more complete temporal objects.
	''' <p>
	''' This interface is a framework-level interface that should not be widely
	''' used in application code. Instead, applications should create and pass
	''' around instances of concrete types, such as {@code LocalDate}.
	''' There are many reasons for this, part of which is that implementations
	''' of this interface may be in calendar systems other than ISO.
	''' See <seealso cref="java.time.chrono.ChronoLocalDate"/> for a fuller discussion of the issues.
	''' 
	''' @implSpec
	''' This interface places no restrictions on the mutability of implementations,
	''' however immutability is strongly recommended.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface TemporalAccessor

		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if the date-time can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/> and <seealso cref="#get(TemporalField) get"/>
		''' methods will throw an exception.
		''' 
		''' @implSpec
		''' Implementations must check and handle all fields defined in <seealso cref="ChronoField"/>.
		''' If the field is supported, then true must be returned, otherwise false must be returned.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' <p>
		''' Implementations must ensure that no observable state is altered when this
		''' read-only method is invoked.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if this date-time can be queried for the field, false if not </returns>
		Function isSupported(  field As TemporalField) As Boolean

		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' All fields can be expressed as a {@code long}  java.lang.[Integer].
		''' This method returns an object that describes the valid range for that value.
		''' The value of this temporal object is used to enhance the accuracy of the returned range.
		''' If the date-time cannot return the range, because the field is unsupported or for
		''' some other reason, an exception will be thrown.
		''' <p>
		''' Note that the result only describes the minimum and maximum valid values
		''' and it is important not to read too much into them. For example, there
		''' could be values within the range that are invalid for the field.
		''' 
		''' @implSpec
		''' Implementations must check and handle all fields defined in <seealso cref="ChronoField"/>.
		''' If the field is supported, then the range of the field must be returned.
		''' If unsupported, then an {@code UnsupportedTemporalTypeException} must be thrown.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessorl)}
		''' passing {@code this} as the argument.
		''' <p>
		''' Implementations must ensure that no observable state is altered when this
		''' read-only method is invoked.
		''' <p>
		''' The default implementation must behave equivalent to this code:
		''' <pre>
		'''  if (field instanceof ChronoField) {
		'''    if (isSupported(field)) {
		'''      return field.range();
		'''    }
		'''    throw new UnsupportedTemporalTypeException("Unsupported field: " + field);
		'''  }
		'''  return field.rangeRefinedBy(this);
		''' </pre>
		''' </summary>
		''' <param name="field">  the field to query the range for, not null </param>
		''' <returns> the range of valid values for the field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		default Function range(  field As TemporalField) As ValueRange
			Sub [New](field   ChronoField As instanceof)
				Sub [New](isSupported(field)    As )
					Function field.range() As [Return]
				throw Function UnsupportedTemporalTypeException("Unsupported field: " &   field As ) As New
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(field, "field");
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return field.rangeRefinedBy(Me);

		''' <summary>
		''' Gets the value of the specified field as an {@code int}.
		''' <p>
		''' This queries the date-time for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If the date-time cannot return the value, because the field is unsupported or for
		''' some other reason, an exception will be thrown.
		''' 
		''' @implSpec
		''' Implementations must check and handle all fields defined in <seealso cref="ChronoField"/>.
		''' If the field is supported and has an {@code int} range, then the value of
		''' the field must be returned.
		''' If unsupported, then an {@code UnsupportedTemporalTypeException} must be thrown.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' <p>
		''' Implementations must ensure that no observable state is altered when this
		''' read-only method is invoked.
		''' <p>
		''' The default implementation must behave equivalent to this code:
		''' <pre>
		'''  if (range(field).isIntValue()) {
		'''    return range(field).checkValidIntValue(getLong(field), field);
		'''  }
		'''  throw new UnsupportedTemporalTypeException("Invalid field " + field + " + for get() method, use getLong() instead");
		''' </pre>
		''' </summary>
		''' <param name="field">  the field to get, not null </param>
		''' <returns> the value for the field, within the valid range of values </returns>
		''' <exception cref="DateTimeException"> if a value for the field cannot be obtained or
		'''         the value is outside the range of valid values for the field </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported or
		'''         the range of values exceeds an {@code int} </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		default Function [get](  field As TemporalField) As Integer
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			ValueRange range = range(field);
			Sub [New](range.isIntValue() ==   [False] As )
				throw Function UnsupportedTemporalTypeException("Invalid field " & field & " for get() method, use getLong() instead"    As ) As New
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			long value = getLong(field);
			Sub [New](range.isValidValue(value) ==   [False] As )
				throw Function java.time.DateTimeException("Invalid value for " & field & " (valid values " & range & "): " &   value As ) As New
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (int) value;

		''' <summary>
		''' Gets the value of the specified field as a {@code long}.
		''' <p>
		''' This queries the date-time for the value of the specified field.
		''' The returned value may be outside the valid range of values for the field.
		''' If the date-time cannot return the value, because the field is unsupported or for
		''' some other reason, an exception will be thrown.
		''' 
		''' @implSpec
		''' Implementations must check and handle all fields defined in <seealso cref="ChronoField"/>.
		''' If the field is supported, then the value of the field must be returned.
		''' If unsupported, then an {@code UnsupportedTemporalTypeException} must be thrown.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.getFrom(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' <p>
		''' Implementations must ensure that no observable state is altered when this
		''' read-only method is invoked.
		''' </summary>
		''' <param name="field">  the field to get, not null </param>
		''' <returns> the value for the field </returns>
		''' <exception cref="DateTimeException"> if a value for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Function getLong(  field As TemporalField) As Long

		''' <summary>
		''' Queries this date-time.
		''' <p>
		''' This queries this date-time using the specified query strategy object.
		''' <p>
		''' Queries are a key tool for extracting information from date-times.
		''' They exists to externalize the process of querying, permitting different
		''' approaches, as per the strategy design pattern.
		''' Examples might be a query that checks if the date is the day before February 29th
		''' in a leap year, or calculates the number of days to your next birthday.
		''' <p>
		''' The most common query implementations are method references, such as
		''' {@code LocalDate::from} and {@code ZoneId::from}.
		''' Additional implementations are provided as static methods on <seealso cref="TemporalQuery"/>.
		''' 
		''' @implSpec
		''' The default implementation must behave equivalent to this code:
		''' <pre>
		'''  if (query == TemporalQueries.zoneId() ||
		'''        query == TemporalQueries.chronology() || query == TemporalQueries.precision()) {
		'''    return null;
		'''  }
		'''  return query.queryFrom(this);
		''' </pre>
		''' Future versions are permitted to add further queries to the if statement.
		''' <p>
		''' All classes implementing this interface and overriding this method must call
		''' {@code TemporalAccessor.super.query(query)}. JDK classes may avoid calling
		''' super if they provide behavior equivalent to the default behaviour, however
		''' non-JDK classes may not utilize this optimization and must call {@code super}.
		''' <p>
		''' If the implementation can supply a value for one of the queries listed in the
		''' if statement of the default implementation, then it must do so.
		''' For example, an application-defined {@code HourMin} class storing the hour
		''' and minute must override this method as follows:
		''' <pre>
		'''  if (query == TemporalQueries.precision()) {
		'''    return MINUTES;
		'''  }
		'''  return TemporalAccessor.super.query(query);
		''' </pre>
		''' <p>
		''' Implementations must ensure that no observable state is altered when this
		''' read-only method is invoked.
		''' </summary>
		''' @param <R> the type of the result </param>
		''' <param name="query">  the query to invoke, not null </param>
		''' <returns> the query result, null may be returned (defined by the query) </returns>
		''' <exception cref="DateTimeException"> if unable to query </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		default Function query(  query As TemporalQuery(Of R)) As R(Of R)
			Sub [New](query == TemporalQueries.zoneId() || query == TemporalQueries.chronology() || query == TemporalQueries.precision()    As )
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return Nothing;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return query.queryFrom(Me);

	End Interface

End Namespace