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
	''' A year-month in the ISO-8601 calendar system, such as {@code 2007-12}.
	''' <p>
	''' {@code YearMonth} is an immutable date-time object that represents the combination
	''' of a year and month. Any field that can be derived from a year and month, such as
	''' quarter-of-year, can be obtained.
	''' <p>
	''' This class does not store or represent a day, time or time-zone.
	''' For example, the value "October 2007" can be stored in a {@code YearMonth}.
	''' <p>
	''' The ISO-8601 calendar system is the modern civil calendar system used today
	''' in most of the world. It is equivalent to the proleptic Gregorian calendar
	''' system, in which today's rules for leap years are applied for all time.
	''' For most applications written today, the ISO-8601 rules are entirely suitable.
	''' However, any application that makes use of historical dates, and requires them
	''' to be accurate will find the ISO-8601 approach unsuitable.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code YearMonth} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class YearMonth
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of YearMonth)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 4183400860270640070L
		''' <summary>
		''' Parser.
		''' </summary>
		Private Shared ReadOnly PARSER As java.time.format.DateTimeFormatter = (New java.time.format.DateTimeFormatterBuilder).appendValue(YEAR, 4, 10, java.time.format.SignStyle.EXCEEDS_PAD).appendLiteral("-"c).appendValue(MONTH_OF_YEAR, 2).toFormatter()

		''' <summary>
		''' The year.
		''' </summary>
		Private ReadOnly year_Renamed As Integer
		''' <summary>
		''' The month-of-year, not null.
		''' </summary>
		Private ReadOnly month As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current year-month from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current year-month.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current year-month using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As YearMonth
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current year-month from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current year-month.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current year-month using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As YearMonth
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current year-month from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current year-month.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current year-month, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As YearMonth
			Dim now_Renamed As LocalDate = LocalDate.now(clock_Renamed) ' called once
			Return YearMonth.of(now_Renamed.year, now_Renamed.month)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code YearMonth} from a year and month.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <returns> the year-month, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Month) As YearMonth
			java.util.Objects.requireNonNull(month, "month")
			Return [of](year_Renamed, month.value)
		End Function

		''' <summary>
		''' Obtains an instance of {@code YearMonth} from a year and month.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <returns> the year-month, not null </returns>
		''' <exception cref="DateTimeException"> if either field value is invalid </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Integer) As YearMonth
			YEAR.checkValidValue(year_Renamed)
			MONTH_OF_YEAR.checkValidValue(month)
			Return New YearMonth(year_Renamed, month)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code YearMonth} from a temporal object.
		''' <p>
		''' This obtains a year-month based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code YearMonth}.
		''' <p>
		''' The conversion extracts the <seealso cref="ChronoField#YEAR YEAR"/> and
		''' <seealso cref="ChronoField#MONTH_OF_YEAR MONTH_OF_YEAR"/> fields.
		''' The extraction is only permitted if the temporal object has an ISO
		''' chronology, or can be converted to a {@code LocalDate}.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code YearMonth::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the year-month, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code YearMonth} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As YearMonth
			If TypeOf temporal Is YearMonth Then Return CType(temporal, YearMonth)
			java.util.Objects.requireNonNull(temporal, "temporal")
			Try
				If java.time.chrono.IsoChronology.INSTANCE.Equals(java.time.chrono.Chronology.from(temporal)) = False Then temporal = LocalDate.from(temporal)
				Return [of](temporal.get(YEAR), temporal.get(MONTH_OF_YEAR))
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain YearMonth from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code YearMonth} from a text string such as {@code 2007-12}.
		''' <p>
		''' The string must represent a valid year-month.
		''' The format must be {@code uuuu-MM}.
		''' Years outside the range 0000 to 9999 must be prefixed by the plus or minus symbol.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007-12", not null </param>
		''' <returns> the parsed year-month, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As YearMonth
			Return parse(text, PARSER)
		End Function

		''' <summary>
		''' Obtains an instance of {@code YearMonth} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a year-month.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed year-month, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As YearMonth
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, YearMonth::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, validated from 1 (January) to 12 (December) </param>
		Private Sub New(ByVal year_Renamed As Integer, ByVal month As Integer)
			Me.year_Renamed = year_Renamed
			Me.month = month
		End Sub

		''' <summary>
		''' Returns a copy of this year-month with the new year and month, checking
		''' to see if a new object is in fact required.
		''' </summary>
		''' <param name="newYear">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		''' <param name="newMonth">  the month-of-year to represent, validated not null </param>
		''' <returns> the year-month, not null </returns>
		Private Function [with](ByVal newYear As Integer, ByVal newMonth As Integer) As YearMonth
			If year_Renamed = newYear AndAlso month = newMonth Then Return Me
			Return New YearMonth(newYear, newMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this year-month can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code MONTH_OF_YEAR}
		''' <li>{@code PROLEPTIC_MONTH}
		''' <li>{@code YEAR_OF_ERA}
		''' <li>{@code YEAR}
		''' <li>{@code ERA}
		''' </ul>
		''' All other {@code ChronoField} instances will return false.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.isSupportedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the field is supported is determined by the field.
		''' </summary>
		''' <param name="field">  the field to check, null returns false </param>
		''' <returns> true if the field is supported on this year-month, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field = YEAR OrElse field = MONTH_OF_YEAR OrElse field = PROLEPTIC_MONTH OrElse field = YEAR_OF_ERA OrElse field = ERA
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this year-month.
		''' If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		''' <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		''' <p>
		''' If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		''' The supported units are:
		''' <ul>
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
		Public Overrides Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return unit = MONTHS OrElse unit = YEARS OrElse unit = DECADES OrElse unit = CENTURIES OrElse unit = MILLENNIA OrElse unit = ERAS
			Return unit IsNot Nothing AndAlso unit.isSupportedBy(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This year-month is used to enhance the accuracy of the returned range.
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
			If field = YEAR_OF_ERA Then Return (If(year <= 0, java.time.temporal.ValueRange.of(1, Year.MAX_VALUE + 1), java.time.temporal.ValueRange.of(1, Year.MAX_VALUE)))
			Return outerInstance.range(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this year-month as an {@code int}.
		''' <p>
		''' This queries this year-month for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this year-month, except {@code PROLEPTIC_MONTH} which is too
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
		Public Overrides Function [get](ByVal field As java.time.temporal.TemporalField) As Integer ' override for Javadoc
			Return range(field).checkValidIntValue(getLong(field), field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this year-month as a {@code long}.
		''' <p>
		''' This queries this year-month for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this year-month.
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
					Case MONTH_OF_YEAR
						Return month
					Case PROLEPTIC_MONTH
						Return prolepticMonth
					Case YEAR_OF_ERA
						Return (If(year_Renamed < 1, 1 - year_Renamed, year_Renamed))
					Case YEAR
						Return year_Renamed
					Case ERA
						Return (If(year_Renamed < 1, 0, 1))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.getFrom(Me)
		End Function

		Private Property prolepticMonth As Long
			Get
				Return (year_Renamed * 12L + month - 1)
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the year.
		''' <p>
		''' The year returned by this method is proleptic as per {@code get(YEAR)}.
		''' </summary>
		''' <returns> the year, from MIN_YEAR to MAX_YEAR </returns>
		Public Property year As Integer
			Get
				Return year_Renamed
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
				Return month
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
				Return Month.of(month)
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the year is a leap year, according to the ISO proleptic
		''' calendar system rules.
		''' <p>
		''' This method applies the current rules for leap years across the whole time-line.
		''' In general, a year is a leap year if it is divisible by four without
		''' remainder. However, years divisible by 100, are not leap years, with
		''' the exception of years divisible by 400 which are.
		''' <p>
		''' For example, 1904 is a leap year it is divisible by 4.
		''' 1900 was not a leap year as it is divisible by 100, however 2000 was a
		''' leap year as it is divisible by 400.
		''' <p>
		''' The calculation is proleptic - applying the same rules into the far future and far past.
		''' This is historically inaccurate, but is correct for the ISO-8601 standard.
		''' </summary>
		''' <returns> true if the year is leap, false otherwise </returns>
		Public Property leapYear As Boolean
			Get
				Return java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year_Renamed)
			End Get
		End Property

		''' <summary>
		''' Checks if the day-of-month is valid for this year-month.
		''' <p>
		''' This method checks whether this year and month and the input day form
		''' a valid date.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to validate, from 1 to 31, invalid value returns false </param>
		''' <returns> true if the day is valid for this year-month </returns>
		Public Function isValidDay(ByVal dayOfMonth As Integer) As Boolean
			Return dayOfMonth >= 1 AndAlso dayOfMonth <= lengthOfMonth()
		End Function

		''' <summary>
		''' Returns the length of the month, taking account of the year.
		''' <p>
		''' This returns the length of the month in days.
		''' For example, a date in January would return 31.
		''' </summary>
		''' <returns> the length of the month in days, from 28 to 31 </returns>
		Public Function lengthOfMonth() As Integer
			Return month.length(leapYear)
		End Function

		''' <summary>
		''' Returns the length of the year.
		''' <p>
		''' This returns the length of the year in days, either 365 or 366.
		''' </summary>
		''' <returns> 366 if the year is leap, 365 otherwise </returns>
		Public Function lengthOfYear() As Integer
			Return (If(leapYear, 366, 365))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this year-month.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the year-month adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the year field.
		''' A more complex adjuster might set the year-month to the next month that
		''' Halley's comet will pass the Earth.
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> a {@code YearMonth} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As YearMonth
			Return CType(adjuster.adjustInto(Me), YearMonth)
		End Function

		''' <summary>
		''' Returns a copy of this year-month with the specified field set to a new value.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the value
		''' for the specified field changed.
		''' This can be used to change any supported field, such as the year or month.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code MONTH_OF_YEAR} -
		'''  Returns a {@code YearMonth} with the specified month-of-year.
		'''  The year will be unchanged.
		''' <li>{@code PROLEPTIC_MONTH} -
		'''  Returns a {@code YearMonth} with the specified proleptic-month.
		'''  This completely replaces the year and month of this object.
		''' <li>{@code YEAR_OF_ERA} -
		'''  Returns a {@code YearMonth} with the specified year-of-era
		'''  The month and era will be unchanged.
		''' <li>{@code YEAR} -
		'''  Returns a {@code YearMonth} with the specified year.
		'''  The month will be unchanged.
		''' <li>{@code ERA} -
		'''  Returns a {@code YearMonth} with the specified era.
		'''  The month and year-of-era will be unchanged.
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
		''' <returns> a {@code YearMonth} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As YearMonth
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				f.checkValidValue(newValue)
				Select Case f
					Case MONTH_OF_YEAR
						Return withMonth(CInt(newValue))
					Case PROLEPTIC_MONTH
						Return plusMonths(newValue - prolepticMonth)
					Case YEAR_OF_ERA
						Return withYear(CInt(Fix(If(year_Renamed < 1, 1 - newValue, newValue))))
					Case YEAR
						Return withYear(CInt(newValue))
					Case ERA
						Return (If(getLong(ERA) = newValue, Me, withYear(1 - year_Renamed)))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the year altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the returned year-month, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> a {@code YearMonth} based on this year-month with the requested year, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Function withYear(ByVal year_Renamed As Integer) As YearMonth
			YEAR.checkValidValue(year_Renamed)
			Return [with](year_Renamed, month)
		End Function

		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the month-of-year altered.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the returned year-month, from 1 (January) to 12 (December) </param>
		''' <returns> a {@code YearMonth} based on this year-month with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(ByVal month As Integer) As YearMonth
			MONTH_OF_YEAR.checkValidValue(month)
			Return [with](year_Renamed, month)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this year-month with the specified amount added.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the specified amount added.
		''' The amount is typically <seealso cref="Period"/> but may be any other type implementing
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
		''' <returns> a {@code YearMonth} based on this year-month with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As YearMonth
			Return CType(amountToAdd.addTo(Me), YearMonth)
		End Function

		''' <summary>
		''' Returns a copy of this year-month with the specified amount added.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code MONTHS} -
		'''  Returns a {@code YearMonth} with the specified number of months added.
		'''  This is equivalent to <seealso cref="#plusMonths(long)"/>.
		''' <li>{@code YEARS} -
		'''  Returns a {@code YearMonth} with the specified number of years added.
		'''  This is equivalent to <seealso cref="#plusYears(long)"/>.
		''' <li>{@code DECADES} -
		'''  Returns a {@code YearMonth} with the specified number of decades added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 10.
		''' <li>{@code CENTURIES} -
		'''  Returns a {@code YearMonth} with the specified number of centuries added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 100.
		''' <li>{@code MILLENNIA} -
		'''  Returns a {@code YearMonth} with the specified number of millennia added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 1,000.
		''' <li>{@code ERAS} -
		'''  Returns a {@code YearMonth} with the specified number of eras added.
		'''  Only two eras are supported so the amount must be one, zero or minus one.
		'''  If the amount is non-zero then the year is changed such that the year-of-era
		'''  is unchanged.
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
		''' <returns> a {@code YearMonth} based on this year-month with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As YearMonth
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case MONTHS
						Return plusMonths(amountToAdd)
					Case YEARS
						Return plusYears(amountToAdd)
					Case DECADES
						Return plusYears(Math.multiplyExact(amountToAdd, 10))
					Case CENTURIES
						Return plusYears(Math.multiplyExact(amountToAdd, 100))
					Case MILLENNIA
						Return plusYears(Math.multiplyExact(amountToAdd, 1000))
					Case ERAS
						Return [with](ERA, Math.addExact(getLong(ERA), amountToAdd))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the specified number of years added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToAdd">  the years to add, may be negative </param>
		''' <returns> a {@code YearMonth} based on this year-month with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function plusYears(ByVal yearsToAdd As Long) As YearMonth
			If yearsToAdd = 0 Then Return Me
			Dim newYear As Integer = YEAR.checkValidIntValue(year_Renamed + yearsToAdd) ' safe overflow
			Return [with](newYear, month)
		End Function

		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the specified number of months added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToAdd">  the months to add, may be negative </param>
		''' <returns> a {@code YearMonth} based on this year-month with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function plusMonths(ByVal monthsToAdd As Long) As YearMonth
			If monthsToAdd = 0 Then Return Me
			Dim monthCount As Long = year_Renamed * 12L + (month - 1)
			Dim calcMonths As Long = monthCount + monthsToAdd ' safe overflow
			Dim newYear As Integer = YEAR.checkValidIntValue(Math.floorDiv(calcMonths, 12))
			Dim newMonth As Integer = CInt(Math.floorMod(calcMonths, 12)) + 1
			Return [with](newYear, newMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this year-month with the specified amount subtracted.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the specified amount subtracted.
		''' The amount is typically <seealso cref="Period"/> but may be any other type implementing
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
		''' <returns> a {@code YearMonth} based on this year-month with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As YearMonth
			Return CType(amountToSubtract.subtractFrom(Me), YearMonth)
		End Function

		''' <summary>
		''' Returns a copy of this year-month with the specified amount subtracted.
		''' <p>
		''' This returns a {@code YearMonth}, based on this one, with the amount
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
		''' <returns> a {@code YearMonth} based on this year-month with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As YearMonth
			Return (If(amountToSubtract = Long.MinValue, plus(Long.MaxValue, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the specified number of years subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		''' <returns> a {@code YearMonth} based on this year-month with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function minusYears(ByVal yearsToSubtract As Long) As YearMonth
			Return (If(yearsToSubtract = Long.MinValue, plusYears(Long.MaxValue).plusYears(1), plusYears(-yearsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code YearMonth} with the specified number of months subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		''' <returns> a {@code YearMonth} based on this year-month with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function minusMonths(ByVal monthsToSubtract As Long) As YearMonth
			Return (If(monthsToSubtract = Long.MinValue, plusMonths(Long.MaxValue).plusMonths(1), plusMonths(-monthsToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this year-month using the specified query.
		''' <p>
		''' This queries this year-month using the specified query strategy object.
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
			If query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then
				Return CType(java.time.chrono.IsoChronology.INSTANCE, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.precision() Then
				Return CType(MONTHS, R)
			End If
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have this year-month.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the year and month changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#PROLEPTIC_MONTH"/> as the field.
		''' If the specified temporal object does not use the ISO calendar system then
		''' a {@code DateTimeException} is thrown.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisYearMonth.adjustInto(temporal);
		'''   temporal = temporal.with(thisYearMonth);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			If java.time.chrono.Chronology.from(temporal).Equals(java.time.chrono.IsoChronology.INSTANCE) = False Then Throw New DateTimeException("Adjustment only supported on ISO date-time")
			Return temporal.with(PROLEPTIC_MONTH, prolepticMonth)
		End Function

		''' <summary>
		''' Calculates the amount of time until another year-month in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code YearMonth}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified year-month.
		''' The result will be negative if the end is before the start.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code YearMonth} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in years between two year-months can be calculated
		''' using {@code startYearMonth.until(endYearMonth, YEARS)}.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two year-months.
		''' For example, the amount in decades between 2012-06 and 2032-05
		''' will only be one decade as it is one month short of two decades.
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
		''' The units {@code MONTHS}, {@code YEARS}, {@code DECADES},
		''' {@code CENTURIES}, {@code MILLENNIA} and {@code ERAS} are supported.
		''' Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to a {@code YearMonth}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this year-month and the end year-month </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code YearMonth} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As YearMonth = YearMonth.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim monthsUntil As Long = [end].prolepticMonth - prolepticMonth ' no overflow
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case MONTHS
						Return monthsUntil
					Case YEARS
						Return monthsUntil \ 12
					Case DECADES
						Return monthsUntil \ 120
					Case CENTURIES
						Return monthsUntil \ 1200
					Case MILLENNIA
						Return monthsUntil \ 12000
					Case ERAS
						Return [end].getLong(ERA) - getLong(ERA)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.between(Me, [end])
		End Function

		''' <summary>
		''' Formats this year-month using the specified formatter.
		''' <p>
		''' This year-month will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted year-month string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this year-month with a day-of-month to create a {@code LocalDate}.
		''' <p>
		''' This returns a {@code LocalDate} formed from this year-month and the specified day-of-month.
		''' <p>
		''' The day-of-month value must be valid for the year-month.
		''' <p>
		''' This method can be used as part of a chain to produce a date:
		''' <pre>
		'''  LocalDate date = year.atMonth(month).atDay(day);
		''' </pre>
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to use, from 1 to 31 </param>
		''' <returns> the date formed from this year-month and the specified day, not null </returns>
		''' <exception cref="DateTimeException"> if the day is invalid for the year-month </exception>
		''' <seealso cref= #isValidDay(int) </seealso>
		Public Function atDay(ByVal dayOfMonth As Integer) As LocalDate
			Return LocalDate.of(year_Renamed, month, dayOfMonth)
		End Function

		''' <summary>
		''' Returns a {@code LocalDate} at the end of the month.
		''' <p>
		''' This returns a {@code LocalDate} based on this year-month.
		''' The day-of-month is set to the last valid day of the month, taking
		''' into account leap years.
		''' <p>
		''' This method can be used as part of a chain to produce a date:
		''' <pre>
		'''  LocalDate date = year.atMonth(month).atEndOfMonth();
		''' </pre>
		''' </summary>
		''' <returns> the last valid date of this year-month, not null </returns>
		Public Function atEndOfMonth() As LocalDate
			Return LocalDate.of(year_Renamed, month, lengthOfMonth())
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this year-month to another year-month.
		''' <p>
		''' The comparison is based first on the value of the year, then on the value of the month.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other year-month to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(ByVal other As YearMonth) As Integer Implements Comparable(Of YearMonth).compareTo
			Dim cmp As Integer = (year_Renamed - other.year_Renamed)
			If cmp = 0 Then cmp = (month - other.month)
			Return cmp
		End Function

		''' <summary>
		''' Checks if this year-month is after the specified year-month.
		''' </summary>
		''' <param name="other">  the other year-month to compare to, not null </param>
		''' <returns> true if this is after the specified year-month </returns>
		Public Function isAfter(ByVal other As YearMonth) As Boolean
			Return compareTo(other) > 0
		End Function

		''' <summary>
		''' Checks if this year-month is before the specified year-month.
		''' </summary>
		''' <param name="other">  the other year-month to compare to, not null </param>
		''' <returns> true if this point is before the specified year-month </returns>
		Public Function isBefore(ByVal other As YearMonth) As Boolean
			Return compareTo(other) < 0
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this year-month is equal to another year-month.
		''' <p>
		''' The comparison is based on the time-line position of the year-months.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other year-month </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is YearMonth Then
				Dim other As YearMonth = CType(obj, YearMonth)
				Return year_Renamed = other.year_Renamed AndAlso month = other.month
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this year-month.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return year_Renamed Xor (month << 27)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this year-month as a {@code String}, such as {@code 2007-12}.
		''' <p>
		''' The output will be in the format {@code uuuu-MM}:
		''' </summary>
		''' <returns> a string representation of this year-month, not null </returns>
		Public Overrides Function ToString() As String
			Dim absYear As Integer = Math.Abs(year_Renamed)
			Dim buf As New StringBuilder(9)
			If absYear < 1000 Then
				If year_Renamed < 0 Then
					buf.append(year_Renamed - 10000).deleteCharAt(1)
				Else
					buf.append(year_Renamed + 10000).deleteCharAt(0)
				End If
			Else
				buf.append(year_Renamed)
			End If
			Return buf.append(If(month < 10, "-0", "-")).append(month).ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(12);  // identifies a YearMonth
		'''  out.writeInt(year);
		'''  out.writeByte(month);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.YEAR_MONTH_TYPE, Me)
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
			out.writeInt(year_Renamed)
			out.writeByte(month)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As YearMonth
			Dim year_Renamed As Integer = [in].readInt()
			Dim month_Renamed As SByte = [in].readByte()
			Return YearMonth.of(year_Renamed, month_Renamed)
		End Function

	End Class

End Namespace