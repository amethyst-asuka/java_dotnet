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
	''' A year in the ISO-8601 calendar system, such as {@code 2007}.
	''' <p>
	''' {@code Year} is an immutable date-time object that represents a year.
	''' Any field that can be derived from a year can be obtained.
	''' <p>
	''' <b>Note that years in the ISO chronology only align with years in the
	''' Gregorian-Julian system for modern years. Parts of Russia did not switch to the
	''' modern Gregorian/ISO rules until 1920.
	''' As such, historical years must be treated with caution.</b>
	''' <p>
	''' This class does not store or represent a month, day, time or time-zone.
	''' For example, the value "2007" can be stored in a {@code Year}.
	''' <p>
	''' Years represented by this class follow the ISO-8601 standard and use
	''' the proleptic numbering system. Year 1 is preceded by year 0, then by year -1.
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
	''' {@code Year} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Year
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, Comparable(Of Year)

		''' <summary>
		''' The minimum supported year, '-999,999,999'.
		''' </summary>
		Public Const MIN_VALUE As Integer = -999999999
		''' <summary>
		''' The maximum supported year, '+999,999,999'.
		''' </summary>
		Public Const MAX_VALUE As Integer = 999999999

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -23038383694477807L
		''' <summary>
		''' Parser.
		''' </summary>
		Private Shared ReadOnly PARSER As java.time.format.DateTimeFormatter = (New java.time.format.DateTimeFormatterBuilder).appendValue(YEAR, 4, 10, java.time.format.SignStyle.EXCEEDS_PAD).toFormatter()

		''' <summary>
		''' The year being represented.
		''' </summary>
		Private ReadOnly year_Renamed As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current year from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current year.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current year using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As Year
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current year from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current year.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current year using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As Year
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current year from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current year.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current year, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As Year
			Dim now_Renamed As LocalDate = LocalDate.now(clock_Renamed) ' called once
			Return Year.of(now_Renamed.year)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Year}.
		''' <p>
		''' This method accepts a year value from the proleptic ISO calendar system.
		''' <p>
		''' The year 2AD/CE is represented by 2.<br>
		''' The year 1AD/CE is represented by 1.<br>
		''' The year 1BC/BCE is represented by 0.<br>
		''' The year 2BC/BCE is represented by -1.<br>
		''' </summary>
		''' <param name="isoYear">  the ISO proleptic year to represent, from {@code MIN_VALUE} to {@code MAX_VALUE} </param>
		''' <returns> the year, not null </returns>
		''' <exception cref="DateTimeException"> if the field is invalid </exception>
		Public Shared Function [of](ByVal isoYear As Integer) As Year
			YEAR.checkValidValue(isoYear)
			Return New Year(isoYear)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Year} from a temporal object.
		''' <p>
		''' This obtains a year based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code Year}.
		''' <p>
		''' The conversion extracts the <seealso cref="ChronoField#YEAR year"/> field.
		''' The extraction is only permitted if the temporal object has an ISO
		''' chronology, or can be converted to a {@code LocalDate}.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code Year::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the year, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code Year} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As Year
			If TypeOf temporal Is Year Then Return CType(temporal, Year)
			java.util.Objects.requireNonNull(temporal, "temporal")
			Try
				If java.time.chrono.IsoChronology.INSTANCE.Equals(java.time.chrono.Chronology.from(temporal)) = False Then temporal = LocalDate.from(temporal)
				Return [of](temporal.get(YEAR))
			Catch ex As DateTimeException
				Throw New DateTimeException("Unable to obtain Year from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Year} from a text string such as {@code 2007}.
		''' <p>
		''' The string must represent a valid year.
		''' Years outside the range 0000 to 9999 must be prefixed by the plus or minus symbol.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007", not null </param>
		''' <returns> the parsed year, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As Year
			Return parse(text, PARSER)
		End Function

		''' <summary>
		''' Obtains an instance of {@code Year} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a year.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed year, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As Year
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, Year::from)
		End Function

		'-------------------------------------------------------------------------
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
		''' <param name="year">  the year to check </param>
		''' <returns> true if the year is leap, false otherwise </returns>
		Public Shared Function isLeap(ByVal year_Renamed As Long) As Boolean
			Return ((year_Renamed And 3) = 0) AndAlso ((year Mod 100) <> 0 OrElse (year Mod 400) = 0)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="year">  the year to represent </param>
		Private Sub New(ByVal year_Renamed As Integer)
			Me.year_Renamed = year_Renamed
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the year value.
		''' <p>
		''' The year returned by this method is proleptic as per {@code get(YEAR)}.
		''' </summary>
		''' <returns> the year, {@code MIN_VALUE} to {@code MAX_VALUE} </returns>
		Public Property value As Integer
			Get
				Return year_Renamed
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this year can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
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
		''' <returns> true if the field is supported on this year, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			If TypeOf field Is java.time.temporal.ChronoField Then Return field = YEAR OrElse field = YEAR_OF_ERA OrElse field = ERA
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this year.
		''' If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		''' <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		''' <p>
		''' If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		''' The supported units are:
		''' <ul>
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
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return unit = YEARS OrElse unit = DECADES OrElse unit = CENTURIES OrElse unit = MILLENNIA OrElse unit = ERAS
			Return unit IsNot Nothing AndAlso unit.isSupportedBy(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This year is used to enhance the accuracy of the returned range.
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
			If field = YEAR_OF_ERA Then Return (If(year_Renamed <= 0, java.time.temporal.ValueRange.of(1, MAX_VALUE + 1), java.time.temporal.ValueRange.of(1, MAX_VALUE)))
			Return outerInstance.range(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this year as an {@code int}.
		''' <p>
		''' This queries this year for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this year.
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
		''' Gets the value of the specified field from this year as a {@code long}.
		''' <p>
		''' This queries this year for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this year.
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
		Public Property leap As Boolean
			Get
				Return Year.isLeap(year_Renamed)
			End Get
		End Property

		''' <summary>
		''' Checks if the month-day is valid for this year.
		''' <p>
		''' This method checks whether this year and the input month and day form
		''' a valid date.
		''' </summary>
		''' <param name="monthDay">  the month-day to validate, null returns false </param>
		''' <returns> true if the month and day are valid for this year </returns>
		Public Function isValidMonthDay(ByVal monthDay_Renamed As MonthDay) As Boolean
			Return monthDay_Renamed IsNot Nothing AndAlso monthDay_Renamed.isValidYear(year_Renamed)
		End Function

		''' <summary>
		''' Gets the length of this year in days.
		''' </summary>
		''' <returns> the length of this year in days, 365 or 366 </returns>
		Public Function length() As Integer
			Return If(leap, 366, 365)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this year.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the year adjusted.
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
		''' <returns> a {@code Year} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As Year
			Return CType(adjuster.adjustInto(Me), Year)
		End Function

		''' <summary>
		''' Returns a copy of this year with the specified field set to a new value.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the value
		''' for the specified field changed.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code YEAR_OF_ERA} -
		'''  Returns a {@code Year} with the specified year-of-era
		'''  The era will be unchanged.
		''' <li>{@code YEAR} -
		'''  Returns a {@code Year} with the specified year.
		'''  This completely replaces the date and is equivalent to <seealso cref="#of(int)"/>.
		''' <li>{@code ERA} -
		'''  Returns a {@code Year} with the specified era.
		'''  The year-of-era will be unchanged.
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
		''' <returns> a {@code Year} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As Year
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				f.checkValidValue(newValue)
				Select Case f
					Case YEAR_OF_ERA
						Return Year.of(CInt(Fix(If(year_Renamed < 1, 1 - newValue, newValue))))
					Case YEAR
						Return Year.of(CInt(newValue))
					Case ERA
						Return (If(getLong(ERA) = newValue, Me, Year.of(1 - year_Renamed)))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this year with the specified amount added.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the specified amount added.
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
		''' <returns> a {@code Year} based on this year with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As Year
			Return CType(amountToAdd.addTo(Me), Year)
		End Function

		''' <summary>
		''' Returns a copy of this year with the specified amount added.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code YEARS} -
		'''  Returns a {@code Year} with the specified number of years added.
		'''  This is equivalent to <seealso cref="#plusYears(long)"/>.
		''' <li>{@code DECADES} -
		'''  Returns a {@code Year} with the specified number of decades added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 10.
		''' <li>{@code CENTURIES} -
		'''  Returns a {@code Year} with the specified number of centuries added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 100.
		''' <li>{@code MILLENNIA} -
		'''  Returns a {@code Year} with the specified number of millennia added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 1,000.
		''' <li>{@code ERAS} -
		'''  Returns a {@code Year} with the specified number of eras added.
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
		''' <returns> a {@code Year} based on this year with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As Year
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
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
		''' Returns a copy of this {@code Year} with the specified number of years added.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToAdd">  the years to add, may be negative </param>
		''' <returns> a {@code Year} based on this year with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function plusYears(ByVal yearsToAdd As Long) As Year
			If yearsToAdd = 0 Then Return Me
			Return [of](YEAR.checkValidIntValue(year_Renamed + yearsToAdd)) ' overflow safe
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this year with the specified amount subtracted.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the specified amount subtracted.
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
		''' <returns> a {@code Year} based on this year with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As Year
			Return CType(amountToSubtract.subtractFrom(Me), Year)
		End Function

		''' <summary>
		''' Returns a copy of this year with the specified amount subtracted.
		''' <p>
		''' This returns a {@code Year}, based on this one, with the amount
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
		''' <returns> a {@code Year} based on this year with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As Year
			Return (If(amountToSubtract = Long.MinValue, plus(Long.MaxValue, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code Year} with the specified number of years subtracted.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		''' <returns> a {@code Year} based on this year with the year subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Function minusYears(ByVal yearsToSubtract As Long) As Year
			Return (If(yearsToSubtract = Long.MinValue, plusYears(Long.MaxValue).plusYears(1), plusYears(-yearsToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this year using the specified query.
		''' <p>
		''' This queries this year using the specified query strategy object.
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
				Return CType(YEARS, R)
			End If
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have this year.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the year changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#YEAR"/> as the field.
		''' If the specified temporal object does not use the ISO calendar system then
		''' a {@code DateTimeException} is thrown.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisYear.adjustInto(temporal);
		'''   temporal = temporal.with(thisYear);
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
			Return temporal.with(YEAR, year_Renamed)
		End Function

		''' <summary>
		''' Calculates the amount of time until another year in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code Year}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified year.
		''' The result will be negative if the end is before the start.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code Year} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in decades between two year can be calculated
		''' using {@code startYear.until(endYear, DECADES)}.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two years.
		''' For example, the amount in decades between 2012 and 2031
		''' will only be one decade as it is one year short of two decades.
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method.
		''' The second is to use <seealso cref="TemporalUnit#between(Temporal, Temporal)"/>:
		''' <pre>
		'''   // these two lines are equivalent
		'''   amount = start.until(end, YEARS);
		'''   amount = YEARS.between(start, end);
		''' </pre>
		''' The choice should be made based on which makes the code more readable.
		''' <p>
		''' The calculation is implemented in this method for <seealso cref="ChronoUnit"/>.
		''' The units {@code YEARS}, {@code DECADES}, {@code CENTURIES},
		''' {@code MILLENNIA} and {@code ERAS} are supported.
		''' Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to a {@code Year}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this year and the end year </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code Year} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As Year = Year.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim yearsUntil As Long = (CLng(Fix([end].year_Renamed))) - year_Renamed ' no overflow
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case YEARS
						Return yearsUntil
					Case DECADES
						Return yearsUntil \ 10
					Case CENTURIES
						Return yearsUntil \ 100
					Case MILLENNIA
						Return yearsUntil \ 1000
					Case ERAS
						Return [end].getLong(ERA) - getLong(ERA)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.between(Me, [end])
		End Function

		''' <summary>
		''' Formats this year using the specified formatter.
		''' <p>
		''' This year will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted year string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this year with a day-of-year to create a {@code LocalDate}.
		''' <p>
		''' This returns a {@code LocalDate} formed from this year and the specified day-of-year.
		''' <p>
		''' The day-of-year value 366 is only valid in a leap year.
		''' </summary>
		''' <param name="dayOfYear">  the day-of-year to use, from 1 to 365-366 </param>
		''' <returns> the local date formed from this year and the specified date of year, not null </returns>
		''' <exception cref="DateTimeException"> if the day of year is zero or less, 366 or greater or equal
		'''  to 366 and this is not a leap year </exception>
		Public Function atDay(ByVal dayOfYear As Integer) As LocalDate
			Return LocalDate.ofYearDay(year_Renamed, dayOfYear)
		End Function

		''' <summary>
		''' Combines this year with a month to create a {@code YearMonth}.
		''' <p>
		''' This returns a {@code YearMonth} formed from this year and the specified month.
		''' All possible combinations of year and month are valid.
		''' <p>
		''' This method can be used as part of a chain to produce a date:
		''' <pre>
		'''  LocalDate date = year.atMonth(month).atDay(day);
		''' </pre>
		''' </summary>
		''' <param name="month">  the month-of-year to use, not null </param>
		''' <returns> the year-month formed from this year and the specified month, not null </returns>
		Public Function atMonth(ByVal month As Month) As YearMonth
			Return YearMonth.of(year_Renamed, month)
		End Function

		''' <summary>
		''' Combines this year with a month to create a {@code YearMonth}.
		''' <p>
		''' This returns a {@code YearMonth} formed from this year and the specified month.
		''' All possible combinations of year and month are valid.
		''' <p>
		''' This method can be used as part of a chain to produce a date:
		''' <pre>
		'''  LocalDate date = year.atMonth(month).atDay(day);
		''' </pre>
		''' </summary>
		''' <param name="month">  the month-of-year to use, from 1 (January) to 12 (December) </param>
		''' <returns> the year-month formed from this year and the specified month, not null </returns>
		''' <exception cref="DateTimeException"> if the month is invalid </exception>
		Public Function atMonth(ByVal month As Integer) As YearMonth
			Return YearMonth.of(year_Renamed, month)
		End Function

		''' <summary>
		''' Combines this year with a month-day to create a {@code LocalDate}.
		''' <p>
		''' This returns a {@code LocalDate} formed from this year and the specified month-day.
		''' <p>
		''' A month-day of February 29th will be adjusted to February 28th in the resulting
		''' date if the year is not a leap year.
		''' </summary>
		''' <param name="monthDay">  the month-day to use, not null </param>
		''' <returns> the local date formed from this year and the specified month-day, not null </returns>
		Public Function atMonthDay(ByVal monthDay_Renamed As MonthDay) As LocalDate
			Return monthDay_Renamed.atYear(year_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this year to another year.
		''' <p>
		''' The comparison is based on the value of the year.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other year to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(ByVal other As Year) As Integer Implements Comparable(Of Year).compareTo
			Return year_Renamed - other.year_Renamed
		End Function

		''' <summary>
		''' Checks if this year is after the specified year.
		''' </summary>
		''' <param name="other">  the other year to compare to, not null </param>
		''' <returns> true if this is after the specified year </returns>
		Public Function isAfter(ByVal other As Year) As Boolean
			Return year_Renamed > other.year_Renamed
		End Function

		''' <summary>
		''' Checks if this year is before the specified year.
		''' </summary>
		''' <param name="other">  the other year to compare to, not null </param>
		''' <returns> true if this point is before the specified year </returns>
		Public Function isBefore(ByVal other As Year) As Boolean
			Return year_Renamed < other.year_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this year is equal to another year.
		''' <p>
		''' The comparison is based on the time-line position of the years.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other year </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is Year Then Return year_Renamed = CType(obj, Year).year_Renamed
			Return False
		End Function

		''' <summary>
		''' A hash code for this year.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return year_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this year as a {@code String}.
		''' </summary>
		''' <returns> a string representation of this year, not null </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString(year_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(11);  // identifies a Year
		'''  out.writeInt(year);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.YEAR_TYPE, Me)
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
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As Year
			Return Year.of([in].readInt())
		End Function

	End Class

End Namespace