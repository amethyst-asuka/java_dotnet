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
	''' A date without a time-zone in the ISO-8601 calendar system,
	''' such as {@code 2007-12-03}.
	''' <p>
	''' {@code LocalDate} is an immutable date-time object that represents a date,
	''' often viewed as year-month-day. Other date fields, such as day-of-year,
	''' day-of-week and week-of-year, can also be accessed.
	''' For example, the value "2nd October 2007" can be stored in a {@code LocalDate}.
	''' <p>
	''' This class does not store or represent a time or time-zone.
	''' Instead, it is a description of the date, as used for birthdays.
	''' It cannot represent an instant on the time-line without additional information
	''' such as an offset or time-zone.
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
	''' {@code LocalDate} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class LocalDate
		Implements java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster, java.time.chrono.ChronoLocalDate

		''' <summary>
		''' The minimum supported {@code LocalDate}, '-999999999-01-01'.
		''' This could be used by an application as a "far past" date.
		''' </summary>
		Public Shared ReadOnly MIN As LocalDate = LocalDate.of(Year.MIN_VALUE, 1, 1)
		''' <summary>
		''' The maximum supported {@code LocalDate}, '+999999999-12-31'.
		''' This could be used by an application as a "far future" date.
		''' </summary>
		Public Shared ReadOnly MAX As LocalDate = LocalDate.of(Year.MAX_VALUE, 12, 31)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 2942565459149668126L
		''' <summary>
		''' The number of days in a 400 year cycle.
		''' </summary>
		Private Const DAYS_PER_CYCLE As Integer = 146097
		''' <summary>
		''' The number of days from year zero to year 1970.
		''' There are five 400 year cycles from year zero to 2000.
		''' There are 7 leap years from 1970 to 2000.
		''' </summary>
		Friend Shared ReadOnly DAYS_0000_TO_1970 As Long = (DAYS_PER_CYCLE * 5L) - (30L * 365L + 7L)

		''' <summary>
		''' The year.
		''' </summary>
		Private ReadOnly year As Integer
		''' <summary>
		''' The month-of-year.
		''' </summary>
		Private ReadOnly month As Short
		''' <summary>
		''' The day-of-month.
		''' </summary>
		Private ReadOnly day As Short

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current date from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As LocalDate
			Return now(Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current date from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As ZoneId) As LocalDate
			Return now(Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current date from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date, not null </returns>
		Public Shared Function now(ByVal clock_Renamed As Clock) As LocalDate
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			' inline to avoid creating object and Instant checks
			Dim now_Renamed As Instant = clock_Renamed.instant() ' called once
			Dim offset As ZoneOffset = clock_Renamed.zone.rules.getOffset(now_Renamed)
			Dim epochSec As Long = now_Renamed.epochSecond + offset.totalSeconds ' overflow caught later
			Dim epochDay As Long = System.Math.floorDiv(epochSec, SECONDS_PER_DAY)
			Return LocalDate.ofEpochDay(epochDay)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a year, month and day.
		''' <p>
		''' This returns a {@code LocalDate} with the specified year, month and day-of-month.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Month, ByVal dayOfMonth As Integer) As LocalDate
			YEAR.checkValidValue(year_Renamed)
			java.util.Objects.requireNonNull(month, "month")
			DAY_OF_MONTH.checkValidValue(dayOfMonth)
			Return create(year_Renamed, month.value, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a year, month and day.
		''' <p>
		''' This returns a {@code LocalDate} with the specified year, month and day-of-month.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 (January) to 12 (December) </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 31 </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As LocalDate
			YEAR.checkValidValue(year_Renamed)
			MONTH_OF_YEAR.checkValidValue(month)
			DAY_OF_MONTH.checkValidValue(dayOfMonth)
			Return create(year_Renamed, month, dayOfMonth)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a year and day-of-year.
		''' <p>
		''' This returns a {@code LocalDate} with the specified year and day-of-year.
		''' The day-of-year must be valid for the year, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="dayOfYear">  the day-of-year to represent, from 1 to 366 </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Shared Function ofYearDay(ByVal year_Renamed As Integer, ByVal dayOfYear As Integer) As LocalDate
			YEAR.checkValidValue(year_Renamed)
			DAY_OF_YEAR.checkValidValue(dayOfYear)
			Dim leap As Boolean = java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year_Renamed)
			If dayOfYear = 366 AndAlso leap = False Then Throw New DateTimeException("Invalid date 'DayOfYear 366' as '" & year_Renamed & "' is not a leap year")
			Dim moy As Month = Month.of((dayOfYear - 1) \ 31 + 1)
			Dim monthEnd As Integer = moy.firstDayOfYear(leap) + moy.length(leap) - 1
			If dayOfYear > monthEnd Then moy = moy.plus(1)
			Dim dom As Integer = dayOfYear - moy.firstDayOfYear(leap) + 1
			Return New LocalDate(year_Renamed, moy.value, dom)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDate} from the epoch day count.
		''' <p>
		''' This returns a {@code LocalDate} with the specified epoch-day.
		''' The <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> is a simple incrementing count
		''' of days where day 0 is 1970-01-01. Negative numbers represent earlier days.
		''' </summary>
		''' <param name="epochDay">  the Epoch Day to convert, based on the epoch 1970-01-01 </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if the epoch day exceeds the supported date range </exception>
		Public Shared Function ofEpochDay(ByVal epochDay As Long) As LocalDate
			Dim zeroDay As Long = epochDay + DAYS_0000_TO_1970
			' find the march-based year
			zeroDay -= 60 ' adjust to 0000-03-01 so leap day is at end of four year cycle
			Dim adjust As Long = 0
			If zeroDay < 0 Then
				' adjust negative years to positive for calculation
				Dim adjustCycles As Long = (zeroDay + 1) \ DAYS_PER_CYCLE - 1
				adjust = adjustCycles * 400
				zeroDay += -adjustCycles * DAYS_PER_CYCLE
			End If
			Dim yearEst As Long = (400 * zeroDay + 591) \ DAYS_PER_CYCLE
			Dim doyEst As Long = zeroDay - (365 * yearEst + yearEst \ 4 - yearEst \ 100 + yearEst \ 400)
			If doyEst < 0 Then
				' fix estimate
				yearEst -= 1
				doyEst = zeroDay - (365 * yearEst + yearEst \ 4 - yearEst \ 100 + yearEst \ 400)
			End If
			yearEst += adjust ' reset any negative year
			Dim marchDoy0 As Integer = CInt(doyEst)

			' convert march-based values back to january-based
			Dim marchMonth0 As Integer = (marchDoy0 * 5 + 2) \ 153
			Dim month_Renamed As Integer = (marchMonth0 + 2) Mod 12 + 1
			Dim dom As Integer = marchDoy0 - (marchMonth0 * 306 + 5) \ 10 + 1
			yearEst += marchMonth0 \ 10

			' check year now we are certain it is correct
			Dim year_Renamed As Integer = YEAR.checkValidIntValue(yearEst)
			Return New LocalDate(year_Renamed, month_Renamed, dom)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a temporal object.
		''' <p>
		''' This obtains a local date based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code LocalDate}.
		''' <p>
		''' The conversion uses the <seealso cref="TemporalQueries#localDate()"/> query, which relies
		''' on extracting the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> field.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code LocalDate::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code LocalDate} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As LocalDate
			java.util.Objects.requireNonNull(temporal, "temporal")
			Dim date_Renamed As LocalDate = temporal.query(java.time.temporal.TemporalQueries.localDate())
			If date_Renamed Is Nothing Then Throw New DateTimeException("Unable to obtain LocalDate from TemporalAccessor: " & temporal & " of type " & temporal.GetType().name)
			Return date_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a text string such as {@code 2007-12-03}.
		''' <p>
		''' The string must represent a valid date and is parsed using
		''' <seealso cref="java.time.format.DateTimeFormatter#ISO_LOCAL_DATE"/>.
		''' </summary>
		''' <param name="text">  the text to parse such as "2007-12-03", not null </param>
		''' <returns> the parsed local date, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence) As LocalDate
			Return parse(text, java.time.format.DateTimeFormatter.ISO_LOCAL_DATE)
		End Function

		''' <summary>
		''' Obtains an instance of {@code LocalDate} from a text string using a specific formatter.
		''' <p>
		''' The text is parsed using the formatter, returning a date.
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the parsed local date, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed </exception>
		Public Shared Function parse(ByVal text As CharSequence, ByVal formatter As java.time.format.DateTimeFormatter) As LocalDate
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.parse(text, LocalDate::from)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates a local date from the year, month and day fields.
		''' </summary>
		''' <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, from 1 to 12, validated </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, validated from 1 to 31 </param>
		''' <returns> the local date, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month is invalid for the month-year </exception>
		Private Shared Function create(ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As LocalDate
			If dayOfMonth > 28 Then
				Dim dom As Integer = 31
				Select Case month
					Case 2
						dom = (If(java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year_Renamed), 29, 28))
					Case 4, 6, 9, 11
						dom = 30
				End Select
				If dayOfMonth > dom Then
					If dayOfMonth = 29 Then
						Throw New DateTimeException("Invalid date 'February 29' as '" & year_Renamed & "' is not a leap year")
					Else
						Throw New DateTimeException("Invalid date '" & Month.of(month).name() & " " & dayOfMonth & "'")
					End If
				End If
			End If
			Return New LocalDate(year_Renamed, month, dayOfMonth)
		End Function

		''' <summary>
		''' Resolves the date, resolving days past the end of month.
		''' </summary>
		''' <param name="year">  the year to represent, validated from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, validated from 1 to 12 </param>
		''' <param name="day">  the day-of-month to represent, validated from 1 to 31 </param>
		''' <returns> the resolved date, not null </returns>
		Private Shared Function resolvePreviousValid(ByVal year_Renamed As Integer, ByVal month As Integer, ByVal day As Integer) As LocalDate
			Select Case month
				Case 2
					day = System.Math.Min(day,If(java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year_Renamed), 29, 28))
				Case 4, 6, 9, 11
					day = System.Math.Min(day, 30)
			End Select
			Return New LocalDate(year_Renamed, month, day)
		End Function

		''' <summary>
		''' Constructor, previously validated.
		''' </summary>
		''' <param name="year">  the year to represent, from MIN_YEAR to MAX_YEAR </param>
		''' <param name="month">  the month-of-year to represent, not null </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, valid for year-month, from 1 to 31 </param>
		Private Sub New(ByVal year_Renamed As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer)
			Me.year = year_Renamed
			Me.month = CShort(month)
			Me.day = CShort(dayOfMonth)
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this date can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/>,
		''' <seealso cref="#get(TemporalField) get"/> and <seealso cref="#with(TemporalField, long)"/>
		''' methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code DAY_OF_WEEK}
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_MONTH}
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_YEAR}
		''' <li>{@code DAY_OF_MONTH}
		''' <li>{@code DAY_OF_YEAR}
		''' <li>{@code EPOCH_DAY}
		''' <li>{@code ALIGNED_WEEK_OF_MONTH}
		''' <li>{@code ALIGNED_WEEK_OF_YEAR}
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
		''' <returns> true if the field is supported on this date, false if not </returns>
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean ' override for Javadoc
			Return outerInstance.isSupported(field)
		End Function

		''' <summary>
		''' Checks if the specified unit is supported.
		''' <p>
		''' This checks if the specified unit can be added to, or subtracted from, this date.
		''' If false, then calling the <seealso cref="#plus(long, TemporalUnit)"/> and
		''' <seealso cref="#minus(long, TemporalUnit) minus"/> methods will throw an exception.
		''' <p>
		''' If the unit is a <seealso cref="ChronoUnit"/> then the query is implemented here.
		''' The supported units are:
		''' <ul>
		''' <li>{@code DAYS}
		''' <li>{@code WEEKS}
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
		Public Overrides Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean ' override for Javadoc
			Return outerInstance.isSupported(unit)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This date is used to enhance the accuracy of the returned range.
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
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				If f.dateBased Then
					Select Case f
						Case DAY_OF_MONTH
							Return java.time.temporal.ValueRange.of(1, lengthOfMonth())
						Case DAY_OF_YEAR
							Return java.time.temporal.ValueRange.of(1, lengthOfYear())
						Case ALIGNED_WEEK_OF_MONTH
							Return java.time.temporal.ValueRange.of(1,If(month Is Month.FEBRUARY AndAlso leapYear = False, 4, 5))
						Case YEAR_OF_ERA
							Return (If(year <= 0, java.time.temporal.ValueRange.of(1, Year.MAX_VALUE + 1), java.time.temporal.ValueRange.of(1, Year.MAX_VALUE)))
					End Select
					Return field.range()
				End If
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.rangeRefinedBy(Me)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this date as an {@code int}.
		''' <p>
		''' This queries this date for the value of the specified field.
		''' The returned value will always be within the valid range of values for the field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this date, except {@code EPOCH_DAY} and {@code PROLEPTIC_MONTH}
		''' which are too large to fit in an {@code int} and throw a {@code DateTimeException}.
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
			If TypeOf field Is java.time.temporal.ChronoField Then Return get0(field)
			Return outerInstance.get(field)
		End Function

		''' <summary>
		''' Gets the value of the specified field from this date as a {@code long}.
		''' <p>
		''' This queries this date for the value of the specified field.
		''' If it is not possible to return the value, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The <seealso cref="#isSupported(TemporalField) supported fields"/> will return valid
		''' values based on this date.
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
				If field = EPOCH_DAY Then Return toEpochDay()
				If field = PROLEPTIC_MONTH Then Return prolepticMonth
				Return get0(field)
			End If
			Return field.getFrom(Me)
		End Function

		Private Function get0(ByVal field As java.time.temporal.TemporalField) As Integer
			Select Case CType(field, java.time.temporal.ChronoField)
				Case DAY_OF_WEEK
					Return dayOfWeek.value
				Case ALIGNED_DAY_OF_WEEK_IN_MONTH
					Return ((day - 1) Mod 7) + 1
				Case ALIGNED_DAY_OF_WEEK_IN_YEAR
					Return ((dayOfYear - 1) Mod 7) + 1
				Case DAY_OF_MONTH
					Return day
				Case DAY_OF_YEAR
					Return dayOfYear
				Case EPOCH_DAY
					Throw New java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'EpochDay' for get() method, use getLong() instead")
				Case ALIGNED_WEEK_OF_MONTH
					Return ((day - 1) \ 7) + 1
				Case ALIGNED_WEEK_OF_YEAR
					Return ((dayOfYear - 1) \ 7) + 1
				Case MONTH_OF_YEAR
					Return month
				Case PROLEPTIC_MONTH
					Throw New java.time.temporal.UnsupportedTemporalTypeException("Invalid field 'ProlepticMonth' for get() method, use getLong() instead")
				Case YEAR_OF_ERA
					Return (If(year >= 1, year, 1 - year))
				Case YEAR
					Return year
				Case ERA
					Return (If(year >= 1, 1, 0))
			End Select
			Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
		End Function

		Private Property prolepticMonth As Long
			Get
				Return (year * 12L + month - 1)
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the chronology of this date, which is the ISO calendar system.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The ISO-8601 calendar system is the modern civil calendar system used today
		''' in most of the world. It is equivalent to the proleptic Gregorian calendar
		''' system, in which today's rules for leap years are applied for all time.
		''' </summary>
		''' <returns> the ISO chronology, not null </returns>
		Public Property Overrides chronology As java.time.chrono.IsoChronology
			Get
				Return java.time.chrono.IsoChronology.INSTANCE
			End Get
		End Property

		''' <summary>
		''' Gets the era applicable at this date.
		''' <p>
		''' The official ISO-8601 standard does not define eras, however {@code IsoChronology} does.
		''' It defines two eras, 'CE' from year one onwards and 'BCE' from year zero backwards.
		''' Since dates before the Julian-Gregorian cutover are not in line with history,
		''' the cutover between 'BCE' and 'CE' is also not aligned with the commonly used
		''' eras, often referred to using 'BC' and 'AD'.
		''' <p>
		''' Users of this class should typically ignore this method as it exists primarily
		''' to fulfill the <seealso cref="ChronoLocalDate"/> contract where it is necessary to support
		''' the Japanese calendar system.
		''' <p>
		''' The returned era will be a singleton capable of being compared with the constants
		''' in <seealso cref="IsoChronology"/> using the {@code ==} operator.
		''' </summary>
		''' <returns> the {@code IsoChronology} era constant applicable at this date, not null </returns>
		Public Property Overrides era As java.time.chrono.Era
			Get
				Return outerInstance.era
			End Get
		End Property

		''' <summary>
		''' Gets the year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the year.
		''' <p>
		''' The year returned by this method is proleptic as per {@code get(YEAR)}.
		''' To obtain the year-of-era, use {@code get(YEAR_OF_ERA)}.
		''' </summary>
		''' <returns> the year, from MIN_YEAR to MAX_YEAR </returns>
		Public Property year As Integer
			Get
				Return year
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

		''' <summary>
		''' Gets the day-of-month field.
		''' <p>
		''' This method returns the primitive {@code int} value for the day-of-month.
		''' </summary>
		''' <returns> the day-of-month, from 1 to 31 </returns>
		Public Property dayOfMonth As Integer
			Get
				Return day
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the day-of-year.
		''' </summary>
		''' <returns> the day-of-year, from 1 to 365, or 366 in a leap year </returns>
		Public Property dayOfYear As Integer
			Get
				Return month.firstDayOfYear(leapYear) + day - 1
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-week field, which is an enum {@code DayOfWeek}.
		''' <p>
		''' This method returns the enum <seealso cref="DayOfWeek"/> for the day-of-week.
		''' This avoids confusion as to what {@code int} values mean.
		''' If you need access to the primitive {@code int} value then the enum
		''' provides the <seealso cref="DayOfWeek#getValue() int value"/>.
		''' <p>
		''' Additional information can be obtained from the {@code DayOfWeek}.
		''' This includes textual names of the values.
		''' </summary>
		''' <returns> the day-of-week, not null </returns>
		Public Property dayOfWeek As DayOfWeek
			Get
				Dim dow0 As Integer = CInt (System.Math.floorMod(toEpochDay() + 3, 7))
				Return DayOfWeek.of(dow0 + 1)
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
		Public Property Overrides leapYear As Boolean
			Get
				Return java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year)
			End Get
		End Property

		''' <summary>
		''' Returns the length of the month represented by this date.
		''' <p>
		''' This returns the length of the month in days.
		''' For example, a date in January would return 31.
		''' </summary>
		''' <returns> the length of the month in days </returns>
		Public Overrides Function lengthOfMonth() As Integer
			Select Case month
				Case 2
					Return (If(leapYear, 29, 28))
				Case 4, 6, 9, 11
					Return 30
				Case Else
					Return 31
			End Select
		End Function

		''' <summary>
		''' Returns the length of the year represented by this date.
		''' <p>
		''' This returns the length of the year in days, either 365 or 366.
		''' </summary>
		''' <returns> 366 if the year is leap, 365 otherwise </returns>
		Public Overrides Function lengthOfYear() As Integer ' override for Javadoc and performance
			Return (If(leapYear, 366, 365))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an adjusted copy of this date.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the date adjusted.
		''' The adjustment takes place using the specified adjuster strategy object.
		''' Read the documentation of the adjuster to understand what adjustment will be made.
		''' <p>
		''' A simple adjuster might simply set the one of the fields, such as the year field.
		''' A more complex adjuster might set the date to the last day of the month.
		''' <p>
		''' A selection of common adjustments is provided in
		''' <seealso cref="java.time.temporal.TemporalAdjusters TemporalAdjusters"/>.
		''' These include finding the "last day of the month" and "next Wednesday".
		''' Key date-time classes also implement the {@code TemporalAdjuster} interface,
		''' such as <seealso cref="Month"/> and <seealso cref="java.time.MonthDay MonthDay"/>.
		''' The adjuster is responsible for handling special cases, such as the varying
		''' lengths of month and leap years.
		''' <p>
		''' For example this code returns a date on the last day of July:
		''' <pre>
		'''  import static java.time.Month.*;
		'''  import static java.time.temporal.TemporalAdjusters.*;
		''' 
		'''  result = localDate.with(JULY).with(lastDayOfMonth());
		''' </pre>
		''' <p>
		''' The result of this method is obtained by invoking the
		''' <seealso cref="TemporalAdjuster#adjustInto(Temporal)"/> method on the
		''' specified adjuster passing {@code this} as the argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="adjuster"> the adjuster to use, not null </param>
		''' <returns> a {@code LocalDate} based on {@code this} with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if the adjustment cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As LocalDate
			' optimizations
			If TypeOf adjuster Is LocalDate Then Return CType(adjuster, LocalDate)
			Return CType(adjuster.adjustInto(Me), LocalDate)
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified field set to a new value.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the value
		''' for the specified field changed.
		''' This can be used to change any supported field, such as the year, month or day-of-month.
		''' If it is not possible to set the value, because the field is not supported or for
		''' some other reason, an exception is thrown.
		''' <p>
		''' In some cases, changing the specified field can cause the resulting date to become invalid,
		''' such as changing the month from 31st January to February would make the day-of-month invalid.
		''' In cases like this, the field is responsible for resolving the date. Typically it will choose
		''' the previous valid date, which would be the last valid day of February in this example.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the adjustment is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code DAY_OF_WEEK} -
		'''  Returns a {@code LocalDate} with the specified day-of-week.
		'''  The date is adjusted up to 6 days forward or backward within the boundary
		'''  of a Monday to Sunday week.
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		'''  Returns a {@code LocalDate} with the specified aligned-day-of-week.
		'''  The date is adjusted to the specified month-based aligned-day-of-week.
		'''  Aligned weeks are counted such that the first week of a given month starts
		'''  on the first day of that month.
		'''  This may cause the date to be moved up to 6 days into the following month.
		''' <li>{@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		'''  Returns a {@code LocalDate} with the specified aligned-day-of-week.
		'''  The date is adjusted to the specified year-based aligned-day-of-week.
		'''  Aligned weeks are counted such that the first week of a given year starts
		'''  on the first day of that year.
		'''  This may cause the date to be moved up to 6 days into the following year.
		''' <li>{@code DAY_OF_MONTH} -
		'''  Returns a {@code LocalDate} with the specified day-of-month.
		'''  The month and year will be unchanged. If the day-of-month is invalid for the
		'''  year and month, then a {@code DateTimeException} is thrown.
		''' <li>{@code DAY_OF_YEAR} -
		'''  Returns a {@code LocalDate} with the specified day-of-year.
		'''  The year will be unchanged. If the day-of-year is invalid for the
		'''  year, then a {@code DateTimeException} is thrown.
		''' <li>{@code EPOCH_DAY} -
		'''  Returns a {@code LocalDate} with the specified epoch-day.
		'''  This completely replaces the date and is equivalent to <seealso cref="#ofEpochDay(long)"/>.
		''' <li>{@code ALIGNED_WEEK_OF_MONTH} -
		'''  Returns a {@code LocalDate} with the specified aligned-week-of-month.
		'''  Aligned weeks are counted such that the first week of a given month starts
		'''  on the first day of that month.
		'''  This adjustment moves the date in whole week chunks to match the specified week.
		'''  The result will have the same day-of-week as this date.
		'''  This may cause the date to be moved into the following month.
		''' <li>{@code ALIGNED_WEEK_OF_YEAR} -
		'''  Returns a {@code LocalDate} with the specified aligned-week-of-year.
		'''  Aligned weeks are counted such that the first week of a given year starts
		'''  on the first day of that year.
		'''  This adjustment moves the date in whole week chunks to match the specified week.
		'''  The result will have the same day-of-week as this date.
		'''  This may cause the date to be moved into the following year.
		''' <li>{@code MONTH_OF_YEAR} -
		'''  Returns a {@code LocalDate} with the specified month-of-year.
		'''  The year will be unchanged. The day-of-month will also be unchanged,
		'''  unless it would be invalid for the new month and year. In that case, the
		'''  day-of-month is adjusted to the maximum valid value for the new month and year.
		''' <li>{@code PROLEPTIC_MONTH} -
		'''  Returns a {@code LocalDate} with the specified proleptic-month.
		'''  The day-of-month will be unchanged, unless it would be invalid for the new month
		'''  and year. In that case, the day-of-month is adjusted to the maximum valid value
		'''  for the new month and year.
		''' <li>{@code YEAR_OF_ERA} -
		'''  Returns a {@code LocalDate} with the specified year-of-era.
		'''  The era and month will be unchanged. The day-of-month will also be unchanged,
		'''  unless it would be invalid for the new month and year. In that case, the
		'''  day-of-month is adjusted to the maximum valid value for the new month and year.
		''' <li>{@code YEAR} -
		'''  Returns a {@code LocalDate} with the specified year.
		'''  The month will be unchanged. The day-of-month will also be unchanged,
		'''  unless it would be invalid for the new month and year. In that case, the
		'''  day-of-month is adjusted to the maximum valid value for the new month and year.
		''' <li>{@code ERA} -
		'''  Returns a {@code LocalDate} with the specified era.
		'''  The year-of-era and month will be unchanged. The day-of-month will also be unchanged,
		'''  unless it would be invalid for the new month and year. In that case, the
		'''  day-of-month is adjusted to the maximum valid value for the new month and year.
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
		''' <returns> a {@code LocalDate} based on {@code this} with the specified field set, not null </returns>
		''' <exception cref="DateTimeException"> if the field cannot be set </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the field is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As LocalDate
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				f.checkValidValue(newValue)
				Select Case f
					Case DAY_OF_WEEK
						Return plusDays(newValue - dayOfWeek.value)
					Case ALIGNED_DAY_OF_WEEK_IN_MONTH
						Return plusDays(newValue - getLong(ALIGNED_DAY_OF_WEEK_IN_MONTH))
					Case ALIGNED_DAY_OF_WEEK_IN_YEAR
						Return plusDays(newValue - getLong(ALIGNED_DAY_OF_WEEK_IN_YEAR))
					Case DAY_OF_MONTH
						Return withDayOfMonth(CInt(newValue))
					Case DAY_OF_YEAR
						Return withDayOfYear(CInt(newValue))
					Case EPOCH_DAY
						Return LocalDate.ofEpochDay(newValue)
					Case ALIGNED_WEEK_OF_MONTH
						Return plusWeeks(newValue - getLong(ALIGNED_WEEK_OF_MONTH))
					Case ALIGNED_WEEK_OF_YEAR
						Return plusWeeks(newValue - getLong(ALIGNED_WEEK_OF_YEAR))
					Case MONTH_OF_YEAR
						Return withMonth(CInt(newValue))
					Case PROLEPTIC_MONTH
						Return plusMonths(newValue - prolepticMonth)
					Case YEAR_OF_ERA
						Return withYear(CInt(Fix(If(year >= 1, newValue, 1 - newValue))))
					Case YEAR
						Return withYear(CInt(newValue))
					Case ERA
						Return (If(getLong(ERA) = newValue, Me, withYear(1 - year)))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.adjustInto(Me, newValue)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the year altered.
		''' <p>
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the result, from MIN_YEAR to MAX_YEAR </param>
		''' <returns> a {@code LocalDate} based on this date with the requested year, not null </returns>
		''' <exception cref="DateTimeException"> if the year value is invalid </exception>
		Public Function withYear(ByVal year_Renamed As Integer) As LocalDate
			If Me.year = year_Renamed Then Return Me
			YEAR.checkValidValue(year_Renamed)
			Return resolvePreviousValid(year_Renamed, month, day)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the month-of-year altered.
		''' <p>
		''' If the day-of-month is invalid for the year, it will be changed to the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="month">  the month-of-year to set in the result, from 1 (January) to 12 (December) </param>
		''' <returns> a {@code LocalDate} based on this date with the requested month, not null </returns>
		''' <exception cref="DateTimeException"> if the month-of-year value is invalid </exception>
		Public Function withMonth(ByVal month As Integer) As LocalDate
			If Me.month = month Then Return Me
			MONTH_OF_YEAR.checkValidValue(month)
			Return resolvePreviousValid(year, month, day)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the day-of-month altered.
		''' <p>
		''' If the resulting date is invalid, an exception is thrown.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfMonth">  the day-of-month to set in the result, from 1 to 28-31 </param>
		''' <returns> a {@code LocalDate} based on this date with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-month value is invalid,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Function withDayOfMonth(ByVal dayOfMonth As Integer) As LocalDate
			If Me.day = dayOfMonth Then Return Me
			Return [of](year, month, dayOfMonth)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the day-of-year altered.
		''' <p>
		''' If the resulting date is invalid, an exception is thrown.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="dayOfYear">  the day-of-year to set in the result, from 1 to 365-366 </param>
		''' <returns> a {@code LocalDate} based on this date with the requested day, not null </returns>
		''' <exception cref="DateTimeException"> if the day-of-year value is invalid,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Function withDayOfYear(ByVal dayOfYear As Integer) As LocalDate
			If Me.dayOfYear = dayOfYear Then Return Me
			Return ofYearDay(year, dayOfYear)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date with the specified amount added.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the specified amount added.
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
		''' <returns> a {@code LocalDate} based on this date with the addition made, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As LocalDate
			If TypeOf amountToAdd Is Period Then
				Dim periodToAdd As Period = CType(amountToAdd, Period)
				Return plusMonths(periodToAdd.toTotalMonths()).plusDays(periodToAdd.days)
			End If
			java.util.Objects.requireNonNull(amountToAdd, "amountToAdd")
			Return CType(amountToAdd.addTo(Me), LocalDate)
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified amount added.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the amount
		''' in terms of the unit added. If it is not possible to add the amount, because the
		''' unit is not supported or for some other reason, an exception is thrown.
		''' <p>
		''' In some cases, adding the amount can cause the resulting date to become invalid.
		''' For example, adding one month to 31st January would result in 31st February.
		''' In cases like this, the unit is responsible for resolving the date.
		''' Typically it will choose the previous valid date, which would be the last valid
		''' day of February in this example.
		''' <p>
		''' If the field is a <seealso cref="ChronoUnit"/> then the addition is implemented here.
		''' The supported fields behave as follows:
		''' <ul>
		''' <li>{@code DAYS} -
		'''  Returns a {@code LocalDate} with the specified number of days added.
		'''  This is equivalent to <seealso cref="#plusDays(long)"/>.
		''' <li>{@code WEEKS} -
		'''  Returns a {@code LocalDate} with the specified number of weeks added.
		'''  This is equivalent to <seealso cref="#plusWeeks(long)"/> and uses a 7 day week.
		''' <li>{@code MONTHS} -
		'''  Returns a {@code LocalDate} with the specified number of months added.
		'''  This is equivalent to <seealso cref="#plusMonths(long)"/>.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
		''' <li>{@code YEARS} -
		'''  Returns a {@code LocalDate} with the specified number of years added.
		'''  This is equivalent to <seealso cref="#plusYears(long)"/>.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
		''' <li>{@code DECADES} -
		'''  Returns a {@code LocalDate} with the specified number of decades added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 10.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
		''' <li>{@code CENTURIES} -
		'''  Returns a {@code LocalDate} with the specified number of centuries added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 100.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
		''' <li>{@code MILLENNIA} -
		'''  Returns a {@code LocalDate} with the specified number of millennia added.
		'''  This is equivalent to calling <seealso cref="#plusYears(long)"/> with the amount
		'''  multiplied by 1,000.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
		''' <li>{@code ERAS} -
		'''  Returns a {@code LocalDate} with the specified number of eras added.
		'''  Only two eras are supported so the amount must be one, zero or minus one.
		'''  If the amount is non-zero then the year is changed such that the year-of-era
		'''  is unchanged.
		'''  The day-of-month will be unchanged unless it would be invalid for the new
		'''  month and year. In that case, the day-of-month is adjusted to the maximum
		'''  valid value for the new month and year.
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
		''' <returns> a {@code LocalDate} based on this date with the specified amount added, not null </returns>
		''' <exception cref="DateTimeException"> if the addition cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalDate
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim f As java.time.temporal.ChronoUnit = CType(unit, java.time.temporal.ChronoUnit)
				Select Case f
					Case DAYS
						Return plusDays(amountToAdd)
					Case WEEKS
						Return plusWeeks(amountToAdd)
					Case MONTHS
						Return plusMonths(amountToAdd)
					Case YEARS
						Return plusYears(amountToAdd)
					Case DECADES
						Return plusYears (System.Math.multiplyExact(amountToAdd, 10))
					Case CENTURIES
						Return plusYears (System.Math.multiplyExact(amountToAdd, 100))
					Case MILLENNIA
						Return plusYears (System.Math.multiplyExact(amountToAdd, 1000))
					Case ERAS
						Return [with](ERA, System.Math.addExact(getLong(ERA), amountToAdd))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.addTo(Me, amountToAdd)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of years added.
		''' <p>
		''' This method adds the specified amount to the years field in three steps:
		''' <ol>
		''' <li>Add the input years to the year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2008-02-29 (leap year) plus one year would result in the
		''' invalid date 2009-02-29 (standard year). Instead of returning an invalid
		''' result, the last valid day of the month, 2009-02-28, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToAdd">  the years to add, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusYears(ByVal yearsToAdd As Long) As LocalDate
			If yearsToAdd = 0 Then Return Me
			Dim newYear As Integer = YEAR.checkValidIntValue(year + yearsToAdd) ' safe overflow
			Return resolvePreviousValid(newYear, month, day)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of months added.
		''' <p>
		''' This method adds the specified amount to the months field in three steps:
		''' <ol>
		''' <li>Add the input months to the month-of-year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2007-03-31 plus one month would result in the invalid date
		''' 2007-04-31. Instead of returning an invalid result, the last valid day
		''' of the month, 2007-04-30, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToAdd">  the months to add, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusMonths(ByVal monthsToAdd As Long) As LocalDate
			If monthsToAdd = 0 Then Return Me
			Dim monthCount As Long = year * 12L + (month - 1)
			Dim calcMonths As Long = monthCount + monthsToAdd ' safe overflow
			Dim newYear As Integer = YEAR.checkValidIntValue (System.Math.floorDiv(calcMonths, 12))
			Dim newMonth As Integer = CInt (System.Math.floorMod(calcMonths, 12)) + 1
			Return resolvePreviousValid(newYear, newMonth, day)
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of weeks added.
		''' <p>
		''' This method adds the specified amount in weeks to the days field incrementing
		''' the month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 plus one week would result in 2009-01-07.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeksToAdd">  the weeks to add, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the weeks added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusWeeks(ByVal weeksToAdd As Long) As LocalDate
			Return plusDays (System.Math.multiplyExact(weeksToAdd, 7))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of days added.
		''' <p>
		''' This method adds the specified amount to the days field incrementing the
		''' month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2008-12-31 plus one day would result in 2009-01-01.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToAdd">  the days to add, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the days added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function plusDays(ByVal daysToAdd As Long) As LocalDate
			If daysToAdd = 0 Then Return Me
			Dim mjDay As Long = System.Math.addExact(toEpochDay(), daysToAdd)
			Return LocalDate.ofEpochDay(mjDay)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the specified amount subtracted.
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
		''' <returns> a {@code LocalDate} based on this date with the subtraction made, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As LocalDate
			If TypeOf amountToSubtract Is Period Then
				Dim periodToSubtract As Period = CType(amountToSubtract, Period)
				Return minusMonths(periodToSubtract.toTotalMonths()).minusDays(periodToSubtract.days)
			End If
			java.util.Objects.requireNonNull(amountToSubtract, "amountToSubtract")
			Return CType(amountToSubtract.subtractFrom(Me), LocalDate)
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified amount subtracted.
		''' <p>
		''' This returns a {@code LocalDate}, based on this one, with the amount
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
		''' <returns> a {@code LocalDate} based on this date with the specified amount subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the subtraction cannot be made </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As LocalDate
			Return (If(amountToSubtract = java.lang.[Long].MIN_VALUE, plus(Long.Max_Value, unit).plus(1, unit), plus(-amountToSubtract, unit)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of years subtracted.
		''' <p>
		''' This method subtracts the specified amount from the years field in three steps:
		''' <ol>
		''' <li>Subtract the input years from the year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2008-02-29 (leap year) minus one year would result in the
		''' invalid date 2007-02-29 (standard year). Instead of returning an invalid
		''' result, the last valid day of the month, 2007-02-28, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusYears(ByVal yearsToSubtract As Long) As LocalDate
			Return (If(yearsToSubtract = java.lang.[Long].MIN_VALUE, plusYears(Long.Max_Value).plusYears(1), plusYears(-yearsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of months subtracted.
		''' <p>
		''' This method subtracts the specified amount from the months field in three steps:
		''' <ol>
		''' <li>Subtract the input months from the month-of-year field</li>
		''' <li>Check if the resulting date would be invalid</li>
		''' <li>Adjust the day-of-month to the last valid day if necessary</li>
		''' </ol>
		''' <p>
		''' For example, 2007-03-31 minus one month would result in the invalid date
		''' 2007-02-31. Instead of returning an invalid result, the last valid day
		''' of the month, 2007-02-28, is selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusMonths(ByVal monthsToSubtract As Long) As LocalDate
			Return (If(monthsToSubtract = java.lang.[Long].MIN_VALUE, plusMonths(Long.Max_Value).plusMonths(1), plusMonths(-monthsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of weeks subtracted.
		''' <p>
		''' This method subtracts the specified amount in weeks from the days field decrementing
		''' the month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2009-01-07 minus one week would result in 2008-12-31.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeksToSubtract">  the weeks to subtract, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the weeks subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusWeeks(ByVal weeksToSubtract As Long) As LocalDate
			Return (If(weeksToSubtract = java.lang.[Long].MIN_VALUE, plusWeeks(Long.Max_Value).plusWeeks(1), plusWeeks(-weeksToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this {@code LocalDate} with the specified number of days subtracted.
		''' <p>
		''' This method subtracts the specified amount from the days field decrementing the
		''' month and year fields as necessary to ensure the result remains valid.
		''' The result is only invalid if the maximum/minimum year is exceeded.
		''' <p>
		''' For example, 2009-01-01 minus one day would result in 2008-12-31.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToSubtract">  the days to subtract, may be negative </param>
		''' <returns> a {@code LocalDate} based on this date with the days subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Public Function minusDays(ByVal daysToSubtract As Long) As LocalDate
			Return (If(daysToSubtract = java.lang.[Long].MIN_VALUE, plusDays(Long.Max_Value).plusDays(1), plusDays(-daysToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Queries this date using the specified query.
		''' <p>
		''' This queries this date using the specified query strategy object.
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
			If query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then Return CType(Me, R)
			Return outerInstance.query(query_Renamed)
		End Function

		''' <summary>
		''' Adjusts the specified temporal object to have the same date as this object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with the date changed to be the same as this.
		''' <p>
		''' The adjustment is equivalent to using <seealso cref="Temporal#with(TemporalField, long)"/>
		''' passing <seealso cref="ChronoField#EPOCH_DAY"/> as the field.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#with(TemporalAdjuster)"/>:
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   temporal = thisLocalDate.adjustInto(temporal);
		'''   temporal = temporal.with(thisLocalDate);
		''' </pre>
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the target object to be adjusted, not null </param>
		''' <returns> the adjusted object, not null </returns>
		''' <exception cref="DateTimeException"> if unable to make the adjustment </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal ' override for Javadoc
			Return outerInstance.adjustInto(temporal)
		End Function

		''' <summary>
		''' Calculates the amount of time until another date in terms of the specified unit.
		''' <p>
		''' This calculates the amount of time between two {@code LocalDate}
		''' objects in terms of a single {@code TemporalUnit}.
		''' The start and end points are {@code this} and the specified date.
		''' The result will be negative if the end is before the start.
		''' The {@code Temporal} passed to this method is converted to a
		''' {@code LocalDate} using <seealso cref="#from(TemporalAccessor)"/>.
		''' For example, the amount in days between two dates can be calculated
		''' using {@code startDate.until(endDate, DAYS)}.
		''' <p>
		''' The calculation returns a whole number, representing the number of
		''' complete units between the two dates.
		''' For example, the amount in months between 2012-06-15 and 2012-08-14
		''' will only be one month as it is one day short of two months.
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
		''' The units {@code DAYS}, {@code WEEKS}, {@code MONTHS}, {@code YEARS},
		''' {@code DECADES}, {@code CENTURIES}, {@code MILLENNIA} and {@code ERAS}
		''' are supported. Other {@code ChronoUnit} values will throw an exception.
		''' <p>
		''' If the unit is not a {@code ChronoUnit}, then the result of this method
		''' is obtained by invoking {@code TemporalUnit.between(Temporal, Temporal)}
		''' passing {@code this} as the first argument and the converted input temporal
		''' as the second argument.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="endExclusive">  the end date, exclusive, which is converted to a {@code LocalDate}, not null </param>
		''' <param name="unit">  the unit to measure the amount in, not null </param>
		''' <returns> the amount of time between this date and the end date </returns>
		''' <exception cref="DateTimeException"> if the amount cannot be calculated, or the end
		'''  temporal cannot be converted to a {@code LocalDate} </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			Dim [end] As LocalDate = LocalDate.from(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case DAYS
						Return daysUntil([end])
					Case WEEKS
						Return daysUntil([end]) \ 7
					Case MONTHS
						Return monthsUntil([end])
					Case YEARS
						Return monthsUntil([end]) \ 12
					Case DECADES
						Return monthsUntil([end]) \ 120
					Case CENTURIES
						Return monthsUntil([end]) \ 1200
					Case MILLENNIA
						Return monthsUntil([end]) \ 12000
					Case ERAS
						Return [end].getLong(ERA) - getLong(ERA)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return unit.between(Me, [end])
		End Function

		Friend Function daysUntil(ByVal [end] As LocalDate) As Long
			Return [end].toEpochDay() - toEpochDay() ' no overflow
		End Function

		Private Function monthsUntil(ByVal [end] As LocalDate) As Long
			Dim packed1 As Long = prolepticMonth * 32L + dayOfMonth ' no overflow
			Dim packed2 As Long = [end].prolepticMonth * 32L + [end].dayOfMonth ' no overflow
			Return (packed2 - packed1) \ 32
		End Function

		''' <summary>
		''' Calculates the period between this date and another date as a {@code Period}.
		''' <p>
		''' This calculates the period between two dates in terms of years, months and days.
		''' The start and end points are {@code this} and the specified date.
		''' The result will be negative if the end is before the start.
		''' The negative sign will be the same in each of year, month and day.
		''' <p>
		''' The calculation is performed using the ISO calendar system.
		''' If necessary, the input date will be converted to ISO.
		''' <p>
		''' The start date is included, but the end date is not.
		''' The period is calculated by removing complete months, then calculating
		''' the remaining number of days, adjusting to ensure that both have the same sign.
		''' The number of months is then normalized into years and months based on a 12 month year.
		''' A month is considered to be complete if the end day-of-month is greater
		''' than or equal to the start day-of-month.
		''' For example, from {@code 2010-01-15} to {@code 2011-03-18} is "1 year, 2 months and 3 days".
		''' <p>
		''' There are two equivalent ways of using this method.
		''' The first is to invoke this method.
		''' The second is to use <seealso cref="Period#between(LocalDate, LocalDate)"/>:
		''' <pre>
		'''   // these two lines are equivalent
		'''   period = start.until(end);
		'''   period = Period.between(start, end);
		''' </pre>
		''' The choice should be made based on which makes the code more readable.
		''' </summary>
		''' <param name="endDateExclusive">  the end date, exclusive, which may be in any chronology, not null </param>
		''' <returns> the period between this date and the end date, not null </returns>
		Public Overrides Function [until](ByVal endDateExclusive As java.time.chrono.ChronoLocalDate) As Period
			Dim [end] As LocalDate = LocalDate.from(endDateExclusive)
			Dim totalMonths As Long = [end].prolepticMonth - Me.prolepticMonth ' safe
			Dim days As Integer = [end].day - Me.day
			If totalMonths > 0 AndAlso days < 0 Then
				totalMonths -= 1
				Dim calcDate As LocalDate = Me.plusMonths(totalMonths)
				days = CInt([end].toEpochDay() - calcDate.toEpochDay()) ' safe
			ElseIf totalMonths < 0 AndAlso days > 0 Then
				totalMonths += 1
				days -= [end].lengthOfMonth()
			End If
			Dim years As Long = totalMonths \ 12 ' safe
			Dim months As Integer = CInt(Fix(totalMonths Mod 12)) ' safe
			Return Period.of (System.Math.toIntExact(years), months, days)
		End Function

		''' <summary>
		''' Formats this date using the specified formatter.
		''' <p>
		''' This date will be passed to the formatter to produce a string.
		''' </summary>
		''' <param name="formatter">  the formatter to use, not null </param>
		''' <returns> the formatted date string, not null </returns>
		''' <exception cref="DateTimeException"> if an error occurs during printing </exception>
		Public Overrides Function format(ByVal formatter As java.time.format.DateTimeFormatter) As String ' override for Javadoc and performance
			java.util.Objects.requireNonNull(formatter, "formatter")
			Return formatter.format(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Combines this date with a time to create a {@code LocalDateTime}.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this date at the specified time.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="time">  the time to combine with, not null </param>
		''' <returns> the local date-time formed from this date and the specified time, not null </returns>
		Public Overrides Function atTime(ByVal time As LocalTime) As LocalDateTime
			Return LocalDateTime.of(Me, time)
		End Function

		''' <summary>
		''' Combines this date with a time to create a {@code LocalDateTime}.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this date at the
		''' specified hour and minute.
		''' The seconds and nanosecond fields will be set to zero.
		''' The individual time fields must be within their valid range.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		''' <returns> the local date-time formed from this date and the specified time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Function atTime(ByVal hour As Integer, ByVal minute As Integer) As LocalDateTime
			Return atTime(LocalTime.of(hour, minute))
		End Function

		''' <summary>
		''' Combines this date with a time to create a {@code LocalDateTime}.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this date at the
		''' specified hour, minute and second.
		''' The nanosecond field will be set to zero.
		''' The individual time fields must be within their valid range.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <returns> the local date-time formed from this date and the specified time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Function atTime(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer) As LocalDateTime
			Return atTime(LocalTime.of(hour, minute, second))
		End Function

		''' <summary>
		''' Combines this date with a time to create a {@code LocalDateTime}.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this date at the
		''' specified hour, minute, second and nanosecond.
		''' The individual time fields must be within their valid range.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="hour">  the hour-of-day to use, from 0 to 23 </param>
		''' <param name="minute">  the minute-of-hour to use, from 0 to 59 </param>
		''' <param name="second">  the second-of-minute to represent, from 0 to 59 </param>
		''' <param name="nanoOfSecond">  the nano-of-second to represent, from 0 to 999,999,999 </param>
		''' <returns> the local date-time formed from this date and the specified time, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Public Function atTime(ByVal hour As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal nanoOfSecond As Integer) As LocalDateTime
			Return atTime(LocalTime.of(hour, minute, second, nanoOfSecond))
		End Function

		''' <summary>
		''' Combines this date with an offset time to create an {@code OffsetDateTime}.
		''' <p>
		''' This returns an {@code OffsetDateTime} formed from this date at the specified time.
		''' All possible combinations of date and time are valid.
		''' </summary>
		''' <param name="time">  the time to combine with, not null </param>
		''' <returns> the offset date-time formed from this date and the specified time, not null </returns>
		Public Function atTime(ByVal time As OffsetTime) As OffsetDateTime
			Return OffsetDateTime.of(LocalDateTime.of(Me, time.toLocalTime()), time.offset)
		End Function

		''' <summary>
		''' Combines this date with the time of midnight to create a {@code LocalDateTime}
		''' at the start of this date.
		''' <p>
		''' This returns a {@code LocalDateTime} formed from this date at the time of
		''' midnight, 00:00, at the start of this date.
		''' </summary>
		''' <returns> the local date-time of midnight at the start of this date, not null </returns>
		Public Function atStartOfDay() As LocalDateTime
			Return LocalDateTime.of(Me, LocalTime.MIDNIGHT)
		End Function

		''' <summary>
		''' Returns a zoned date-time from this date at the earliest valid time according
		''' to the rules in the time-zone.
		''' <p>
		''' Time-zone rules, such as daylight savings, mean that not every local date-time
		''' is valid for the specified zone, thus the local date-time may not be midnight.
		''' <p>
		''' In most cases, there is only one valid offset for a local date-time.
		''' In the case of an overlap, there are two valid offsets, and the earlier one is used,
		''' corresponding to the first occurrence of midnight on the date.
		''' In the case of a gap, the zoned date-time will represent the instant just after the gap.
		''' <p>
		''' If the zone ID is a <seealso cref="ZoneOffset"/>, then the result always has a time of midnight.
		''' <p>
		''' To convert to a specific time in a given time-zone call <seealso cref="#atTime(LocalTime)"/>
		''' followed by <seealso cref="LocalDateTime#atZone(ZoneId)"/>.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the zoned date-time formed from this date and the earliest valid time for the zone, not null </returns>
		Public Function atStartOfDay(ByVal zone As ZoneId) As ZonedDateTime
			java.util.Objects.requireNonNull(zone, "zone")
			' need to handle case where there is a gap from 11:30 to 00:30
			' standard ZDT factory would result in 01:00 rather than 00:30
			Dim ldt As LocalDateTime = atTime(LocalTime.MIDNIGHT)
			If TypeOf zone Is ZoneOffset = False Then
				Dim rules As java.time.zone.ZoneRules = zone.rules
				Dim trans As java.time.zone.ZoneOffsetTransition = rules.getTransition(ldt)
				If trans IsNot Nothing AndAlso trans.gap Then ldt = trans.dateTimeAfter
			End If
			Return ZonedDateTime.of(ldt, zone)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function toEpochDay() As Long
			Dim y As Long = year
			Dim m As Long = month
			Dim total As Long = 0
			total += 365 * y
			If y >= 0 Then
				total += (y + 3) \ 4 - (y + 99) \ 100 + (y + 399) \ 400
			Else
				total -= y / -4 - y / -100 + y / -400
			End If
			total += ((367 * m - 362) \ 12)
			total += day - 1
			If m > 2 Then
				total -= 1
				If leapYear = False Then total -= 1
			End If
			Return total - DAYS_0000_TO_1970
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this date to another date.
		''' <p>
		''' The comparison is primarily based on the date, from earliest to latest.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' <p>
		''' If all the dates being compared are instances of {@code LocalDate},
		''' then the comparison will be entirely based on the date.
		''' If some dates being compared are in different chronologies, then the
		''' chronology is also considered, see <seealso cref="java.time.chrono.ChronoLocalDate#compareTo"/>.
		''' </summary>
		''' <param name="other">  the other date to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Public Overrides Function compareTo(ByVal other As java.time.chrono.ChronoLocalDate) As Integer ' override for Javadoc and performance
			If TypeOf other Is LocalDate Then Return compareTo0(CType(other, LocalDate))
			Return outerInstance.CompareTo(other)
		End Function

		Friend Function compareTo0(ByVal otherDate As LocalDate) As Integer
			Dim cmp As Integer = (year - otherDate.year)
			If cmp = 0 Then
				cmp = (month - otherDate.month)
				If cmp = 0 Then cmp = (day - otherDate.day)
			End If
			Return cmp
		End Function

		''' <summary>
		''' Checks if this date is after the specified date.
		''' <p>
		''' This checks to see if this date represents a point on the
		''' local time-line after the other date.
		''' <pre>
		'''   LocalDate a = LocalDate.of(2012, 6, 30);
		'''   LocalDate b = LocalDate.of(2012, 7, 1);
		'''   a.isAfter(b) == false
		'''   a.isAfter(a) == false
		'''   b.isAfter(a) == true
		''' </pre>
		''' <p>
		''' This method only considers the position of the two dates on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>,
		''' but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date to compare to, not null </param>
		''' <returns> true if this date is after the specified date </returns>
		Public Overrides Function isAfter(ByVal other As java.time.chrono.ChronoLocalDate) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDate Then Return compareTo0(CType(other, LocalDate)) > 0
			Return outerInstance.isAfter(other)
		End Function

		''' <summary>
		''' Checks if this date is before the specified date.
		''' <p>
		''' This checks to see if this date represents a point on the
		''' local time-line before the other date.
		''' <pre>
		'''   LocalDate a = LocalDate.of(2012, 6, 30);
		'''   LocalDate b = LocalDate.of(2012, 7, 1);
		'''   a.isBefore(b) == true
		'''   a.isBefore(a) == false
		'''   b.isBefore(a) == false
		''' </pre>
		''' <p>
		''' This method only considers the position of the two dates on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>,
		''' but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date to compare to, not null </param>
		''' <returns> true if this date is before the specified date </returns>
		Public Overrides Function isBefore(ByVal other As java.time.chrono.ChronoLocalDate) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDate Then Return compareTo0(CType(other, LocalDate)) < 0
			Return outerInstance.isBefore(other)
		End Function

		''' <summary>
		''' Checks if this date is equal to the specified date.
		''' <p>
		''' This checks to see if this date represents the same point on the
		''' local time-line as the other date.
		''' <pre>
		'''   LocalDate a = LocalDate.of(2012, 6, 30);
		'''   LocalDate b = LocalDate.of(2012, 7, 1);
		'''   a.isEqual(b) == false
		'''   a.isEqual(a) == true
		'''   b.isEqual(a) == false
		''' </pre>
		''' <p>
		''' This method only considers the position of the two dates on the local time-line.
		''' It does not take into account the chronology, or calendar system.
		''' This is different from the comparison in <seealso cref="#compareTo(ChronoLocalDate)"/>
		''' but is the same approach as <seealso cref="ChronoLocalDate#timeLineOrder()"/>.
		''' </summary>
		''' <param name="other">  the other date to compare to, not null </param>
		''' <returns> true if this date is equal to the specified date </returns>
		Public Overrides Function isEqual(ByVal other As java.time.chrono.ChronoLocalDate) As Boolean ' override for Javadoc and performance
			If TypeOf other Is LocalDate Then Return compareTo0(CType(other, LocalDate)) = 0
			Return outerInstance.isEqual(other)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this date is equal to another date.
		''' <p>
		''' Compares this {@code LocalDate} with another ensuring that the date is the same.
		''' <p>
		''' Only objects of type {@code LocalDate} are compared, other types return false.
		''' To compare the dates of two {@code TemporalAccessor} instances, including dates
		''' in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is LocalDate Then Return compareTo0(CType(obj, LocalDate)) = 0
			Return False
		End Function

		''' <summary>
		''' A hash code for this date.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim yearValue As Integer = year
			Dim monthValue_Renamed As Integer = month
			Dim dayValue As Integer = day
			Return (yearValue And &HFFFFF800L) Xor ((yearValue << 11) + (monthValue_Renamed << 6) + (dayValue))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this date as a {@code String}, such as {@code 2007-12-03}.
		''' <p>
		''' The output will be in the ISO-8601 format {@code uuuu-MM-dd}.
		''' </summary>
		''' <returns> a string representation of this date, not null </returns>
		Public Overrides Function ToString() As String
			Dim yearValue As Integer = year
			Dim monthValue_Renamed As Integer = month
			Dim dayValue As Integer = day
			Dim absYear As Integer = System.Math.Abs(yearValue)
			Dim buf As New StringBuilder(10)
			If absYear < 1000 Then
				If yearValue < 0 Then
					buf.append(yearValue - 10000).deleteCharAt(1)
				Else
					buf.append(yearValue + 10000).deleteCharAt(0)
				End If
			Else
				If yearValue > 9999 Then buf.append("+"c)
				buf.append(yearValue)
			End If
			Return buf.append(If(monthValue_Renamed < 10, "-0", "-")).append(monthValue_Renamed).append(If(dayValue < 10, "-0", "-")).append(dayValue).ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(3);  // identifies a LocalDate
		'''  out.writeInt(year);
		'''  out.writeByte(month);
		'''  out.writeByte(day);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.LOCAL_DATE_TYPE, Me)
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
			out.writeInt(year)
			out.writeByte(month)
			out.writeByte(day)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As LocalDate
			Dim year_Renamed As Integer = [in].readInt()
			Dim month_Renamed As Integer = [in].readByte()
			Dim dayOfMonth_Renamed As Integer = [in].readByte()
			Return LocalDate.of(year_Renamed, month_Renamed, dayOfMonth_Renamed)
		End Function

	End Class

End Namespace