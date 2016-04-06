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
Namespace java.time.chrono



	''' <summary>
	''' An era of the time-line.
	''' <p>
	''' Most calendar systems have a single epoch dividing the time-line into two eras.
	''' However, some calendar systems, have multiple eras, such as one for the reign
	''' of each leader.
	''' In all cases, the era is conceptually the largest division of the time-line.
	''' Each chronology defines the Era's that are known Eras and a
	''' <seealso cref="Chronology#eras Chronology.eras"/> to get the valid eras.
	''' <p>
	''' For example, the Thai Buddhist calendar system divides time into two eras,
	''' before and after a single date. By contrast, the Japanese calendar system
	''' has one era for the reign of each Emperor.
	''' <p>
	''' Instances of {@code Era} may be compared using the {@code ==} operator.
	''' 
	''' @implSpec
	''' This interface must be implemented with care to ensure other classes operate correctly.
	''' All implementations must be singletons - final, immutable and thread-safe.
	''' It is recommended to use an enum whenever possible.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface Era
		Inherits java.time.temporal.TemporalAccessor, java.time.temporal.TemporalAdjuster

		''' <summary>
		''' Gets the numeric value associated with the era as defined by the chronology.
		''' Each chronology defines the predefined Eras and methods to list the Eras
		''' of the chronology.
		''' <p>
		''' All fields, including eras, have an associated numeric value.
		''' The meaning of the numeric value for era is determined by the chronology
		''' according to these principles:
		''' <ul>
		''' <li>The era in use at the epoch 1970-01-01 (ISO) has the value 1.
		''' <li>Later eras have sequentially higher values.
		''' <li>Earlier eras have sequentially lower values, which may be negative.
		''' </ul>
		''' </summary>
		''' <returns> the numeric era value </returns>
		ReadOnly Property value As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this era can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		''' <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code ERA} field returns true.
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this era, false if not </returns>
		default Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
			Sub [New](field   java.time.temporal.ChronoField As instanceof)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				Return field == ERA;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return field != Nothing && field.isSupportedBy(Me);

		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This era is used to enhance the accuracy of the returned range.
		''' If it is not possible to return the range, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code ERA} field returns the range.
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the range can be obtained is determined by the field.
		''' <p>
		''' The default implementation must return a range for {@code ERA} from
		''' zero to one, suitable for two era calendar systems such as ISO.
		''' </summary>
		''' <param name="field">  the field to query the range for, not null </param>
		''' <returns> the range of valid values for the field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		default Overrides Function range(  field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange ' override for Javadoc
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return outerInstance.range(field);

		''' <summary>
		''' Gets the value of the specified field from this era as an {@code int}.
		''' <p>
		''' This queries this era for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code ERA} field returns the value of the era.
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
		default Overrides Function [get](  field As java.time.temporal.TemporalField) As Integer ' override for Javadoc and performance
			Sub [New](field ==   ERA As )
				ReadOnly Property value As [Return]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return outerInstance.get(field);

		''' <summary>
		''' Gets the value of the specified field from this era as a {@code long}.
		''' <p>
		''' This queries this era for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code ERA} field returns the value of the era.
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
		default Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
			Sub [New](field ==   ERA As )
				ReadOnly Property value As [Return]
			Function [if](field   java.time.temporal.ChronoField As instanceof) As else
				throw Function java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " &   field As ) As New
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return field.getFrom(Me);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this era using the specified query.
		''' <p>
		''' This queries this era using the specified query strategy object.
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
			Sub [New](query == java.time.temporal.TemporalQueries.precision()    As )
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return (R) ERAS;
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return outerInstance.query(query);

		''' <summary>
		''' Adjusts the specified temporal object to have the same era as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the era changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#ERA"/> as the field.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisEra.adjustInto(temporal);
		'''   temporal = temporal.with(thisEra);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		default Overrides Function adjustInto(  temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return temporal.with(ERA, getValue());

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the textual representation of this era.
		''' <p>
		''' This returns the textual name used to identify the era,
		''' suitable for presentation to the user.
		''' The parameters control the style of the returned text and the locale.
		''' <p>
		''' If no textual mapping is found then the <seealso cref="#getValue() numeric value"/> is returned.
		''' <p>
		''' This default implementation is suitable for all implementations.
		''' </summary>
		''' <param name="style">  the style of the text required, not null </param>
		''' <param name="locale">  the locale to use, not null </param>
		''' <returns> the text value of the era, not null </returns>
		default Function getDisplayName(  style As java.time.format.TextStyle,   locale As java.util.Locale) As String
			Return Function java.time.format.DateTimeFormatterBuilder() As New

		' NOTE: methods to convert year-of-era/proleptic-year cannot be here as they may depend on month/day (Japanese)
	End Interface

End Namespace