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
	''' The ISO calendar system.
	''' <p>
	''' This chronology defines the rules of the ISO calendar system.
	''' This calendar system is based on the ISO-8601 standard, which is the
	''' <i>de facto</i> world calendar.
	''' <p>
	''' The fields are defined as follows:
	''' <ul>
	''' <li>era - There are two eras, 'Current Era' (CE) and 'Before Current Era' (BCE).
	''' <li>year-of-era - The year-of-era is the same as the proleptic-year for the current CE era.
	'''  For the BCE era before the ISO epoch the year increases from 1 upwards as time goes backwards.
	''' <li>proleptic-year - The proleptic year is the same as the year-of-era for the
	'''  current era. For the previous era, years have zero, then negative values.
	''' <li>month-of-year - There are 12 months in an ISO year, numbered from 1 to 12.
	''' <li>day-of-month - There are between 28 and 31 days in each of the ISO month, numbered from 1 to 31.
	'''  Months 4, 6, 9 and 11 have 30 days, Months 1, 3, 5, 7, 8, 10 and 12 have 31 days.
	'''  Month 2 has 28 days, or 29 in a leap year.
	''' <li>day-of-year - There are 365 days in a standard ISO year and 366 in a leap year.
	'''  The days are numbered from 1 to 365 or 1 to 366.
	''' <li>leap-year - Leap years occur every 4 years, except where the year is divisble by 100 and not divisble by 400.
	''' </ul>
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class IsoChronology
		Inherits AbstractChronology

		''' <summary>
		''' Singleton instance of the ISO chronology.
		''' </summary>
		Public Shared ReadOnly INSTANCE As New IsoChronology

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -1440403870442975015L

		''' <summary>
		''' Restricted constructor.
		''' </summary>
		Private Sub New()
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the ID of the chronology - 'ISO'.
		''' <p>
		''' The ID uniquely identifies the {@code Chronology}.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <returns> the chronology ID - 'ISO' </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		Public  Overrides ReadOnly Property  id As String
			Get
				Return "ISO"
			End Get
		End Property

		''' <summary>
		''' Gets the calendar type of the underlying calendar system - 'iso8601'.
		''' <p>
		''' The calendar type is an identifier defined by the
		''' <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' It can also be used as part of a locale, accessible via
		''' <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		''' </summary>
		''' <returns> the calendar system type - 'iso8601' </returns>
		''' <seealso cref= #getId() </seealso>
		Public  Overrides ReadOnly Property  calendarType As String
			Get
				Return "iso8601"
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an ISO local date from the era, year-of-era, month-of-year
		''' and day-of-month fields.
		''' </summary>
		''' <param name="era">  the ISO era, not null </param>
		''' <param name="yearOfEra">  the ISO year-of-era </param>
		''' <param name="month">  the ISO month-of-year </param>
		''' <param name="dayOfMonth">  the ISO day-of-month </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the type of {@code era} is not {@code IsoEra} </exception>
		Public Overrides Function [date](  era As Era,   yearOfEra As Integer,   month As Integer,   dayOfMonth As Integer) As java.time.LocalDate ' override with covariant return type
			Return [date](prolepticYear(era, yearOfEra), month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains an ISO local date from the proleptic-year, month-of-year
		''' and day-of-month fields.
		''' <p>
		''' This is equivalent to <seealso cref="LocalDate#of(int, int, int)"/>.
		''' </summary>
		''' <param name="prolepticYear">  the ISO proleptic-year </param>
		''' <param name="month">  the ISO month-of-year </param>
		''' <param name="dayOfMonth">  the ISO day-of-month </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function [date](  prolepticYear As Integer,   month As Integer,   dayOfMonth As Integer) As java.time.LocalDate ' override with covariant return type
			Return java.time.LocalDate.of(prolepticYear, month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains an ISO local date from the era, year-of-era and day-of-year fields.
		''' </summary>
		''' <param name="era">  the ISO era, not null </param>
		''' <param name="yearOfEra">  the ISO year-of-era </param>
		''' <param name="dayOfYear">  the ISO day-of-year </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateYearDay(  era As Era,   yearOfEra As Integer,   dayOfYear As Integer) As java.time.LocalDate ' override with covariant return type
			Return dateYearDay(prolepticYear(era, yearOfEra), dayOfYear)
		End Function

		''' <summary>
		''' Obtains an ISO local date from the proleptic-year and day-of-year fields.
		''' <p>
		''' This is equivalent to <seealso cref="LocalDate#ofYearDay(int, int)"/>.
		''' </summary>
		''' <param name="prolepticYear">  the ISO proleptic-year </param>
		''' <param name="dayOfYear">  the ISO day-of-year </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateYearDay(  prolepticYear As Integer,   dayOfYear As Integer) As java.time.LocalDate ' override with covariant return type
			Return java.time.LocalDate.ofYearDay(prolepticYear, dayOfYear)
		End Function

		''' <summary>
		''' Obtains an ISO local date from the epoch-day.
		''' <p>
		''' This is equivalent to <seealso cref="LocalDate#ofEpochDay(long)"/>.
		''' </summary>
		''' <param name="epochDay">  the epoch day </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateEpochDay(  epochDay As Long) As java.time.LocalDate ' override with covariant return type
			Return java.time.LocalDate.ofEpochDay(epochDay)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an ISO local date from another date-time object.
		''' <p>
		''' This is equivalent to <seealso cref="LocalDate#from(TemporalAccessor)"/>.
		''' </summary>
		''' <param name="temporal">  the date-time object to convert, not null </param>
		''' <returns> the ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function [date](  temporal As java.time.temporal.TemporalAccessor) As java.time.LocalDate ' override with covariant return type
			Return java.time.LocalDate.from(temporal)
		End Function

		''' <summary>
		''' Obtains an ISO local date-time from another date-time object.
		''' <p>
		''' This is equivalent to <seealso cref="LocalDateTime#from(TemporalAccessor)"/>.
		''' </summary>
		''' <param name="temporal">  the date-time object to convert, not null </param>
		''' <returns> the ISO local date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date-time </exception>
		Public Overrides Function localDateTime(  temporal As java.time.temporal.TemporalAccessor) As java.time.LocalDateTime ' override with covariant return type
			Return java.time.LocalDateTime.from(temporal)
		End Function

		''' <summary>
		''' Obtains an ISO zoned date-time from another date-time object.
		''' <p>
		''' This is equivalent to <seealso cref="ZonedDateTime#from(TemporalAccessor)"/>.
		''' </summary>
		''' <param name="temporal">  the date-time object to convert, not null </param>
		''' <returns> the ISO zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date-time </exception>
		Public Overrides Function zonedDateTime(  temporal As java.time.temporal.TemporalAccessor) As java.time.ZonedDateTime ' override with covariant return type
			Return java.time.ZonedDateTime.from(temporal)
		End Function

		''' <summary>
		''' Obtains an ISO zoned date-time in this chronology from an {@code Instant}.
		''' <p>
		''' This is equivalent to <seealso cref="ZonedDateTime#ofInstant(Instant, ZoneId)"/>.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
		Public Overrides Function zonedDateTime(  instant_Renamed As java.time.Instant,   zone As java.time.ZoneId) As java.time.ZonedDateTime
			Return java.time.ZonedDateTime.ofInstant(instant_Renamed, zone)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current ISO local date from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current ISO local date using the system clock and default time-zone, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateNow() As java.time.LocalDate ' override with covariant return type
			Return dateNow(java.time.Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current ISO local date from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current ISO local date using the system clock, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateNow(  zone As java.time.ZoneId) As java.time.LocalDate ' override with covariant return type
			Return dateNow(java.time.Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current ISO local date from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current ISO local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateNow(  clock_Renamed As java.time.Clock) As java.time.LocalDate ' override with covariant return type
			java.util.Objects.requireNonNull(clock_Renamed, "clock")
			Return [date](java.time.LocalDate.now(clock_Renamed))
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
		''' <param name="prolepticYear">  the ISO proleptic year to check </param>
		''' <returns> true if the year is leap, false otherwise </returns>
		Public Overrides Function isLeapYear(  prolepticYear As Long) As Boolean
			[Return] ((prolepticYear And 3) = 0) AndAlso ((prolepticYear Mod 100) <> 0 OrElse (prolepticYear Mod 400) = 0)
		End Function

		Public Overrides Function prolepticYear(  era As Era,   yearOfEra As Integer) As Integer
			If TypeOf era Is IsoEra = False Then Throw New ClassCastException("Era must be IsoEra")
			[Return] (If(era = IsoEra.CE, yearOfEra, 1 - yearOfEra))
		End Function

		Public Overrides Function eraOf(  eraValue As Integer) As IsoEra
			Return IsoEra.of(eraValue)
		End Function

		Public Overrides Function eras() As IList(Of Era)
			Return java.util.Arrays.asList(Of Era)(IsoEra.values())
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Resolves parsed {@code ChronoField} values into a date during parsing.
		''' <p>
		''' Most {@code TemporalField} implementations are resolved using the
		''' resolve method on the field. By contrast, the {@code ChronoField} class
		''' defines fields that only have meaning relative to the chronology.
		''' As such, {@code ChronoField} date fields are resolved here in the
		''' context of a specific chronology.
		''' <p>
		''' {@code ChronoField} instances on the ISO calendar system are resolved
		''' as follows.
		''' <ul>
		''' <li>{@code EPOCH_DAY} - If present, this is converted to a {@code LocalDate}
		'''  and all other date fields are then cross-checked against the date.
		''' <li>{@code PROLEPTIC_MONTH} - If present, then it is split into the
		'''  {@code YEAR} and {@code MONTH_OF_YEAR}. If the mode is strict or smart
		'''  then the field is validated.
		''' <li>{@code YEAR_OF_ERA} and {@code ERA} - If both are present, then they
		'''  are combined to form a {@code YEAR}. In lenient mode, the {@code YEAR_OF_ERA}
		'''  range is not validated, in smart and strict mode it is. The {@code ERA} is
		'''  validated for range in all three modes. If only the {@code YEAR_OF_ERA} is
		'''  present, and the mode is smart or lenient, then the current era (CE/AD)
		'''  is assumed. In strict mode, no era is assumed and the {@code YEAR_OF_ERA} is
		'''  left untouched. If only the {@code ERA} is present, then it is left untouched.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH} -
		'''  If all three are present, then they are combined to form a {@code LocalDate}.
		'''  In all three modes, the {@code YEAR} is validated. If the mode is smart or strict,
		'''  then the month and day are validated, with the day validated from 1 to 31.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first of January in the requested year, then adding
		'''  the difference in months, then the difference in days.
		'''  If the mode is smart, and the day-of-month is greater than the maximum for
		'''  the year-month, then the day-of-month is adjusted to the last day-of-month.
		'''  If the mode is strict, then the three fields must form a valid date.
		''' <li>{@code YEAR} and {@code DAY_OF_YEAR} -
		'''  If both are present, then they are combined to form a {@code LocalDate}.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first of January in the requested year, then adding
		'''  the difference in days.
		'''  If the mode is smart or strict, then the two fields must form a valid date.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		'''  {@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		'''  If all four are present, then they are combined to form a {@code LocalDate}.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first of January in the requested year, then adding
		'''  the difference in months, then the difference in weeks, then in days.
		'''  If the mode is smart or strict, then the all four fields are validated to
		'''  their outer ranges. The date is then combined in a manner equivalent to
		'''  creating a date on the first day of the requested year and month, then adding
		'''  the amount in weeks and days to reach their values. If the mode is strict,
		'''  the date is additionally validated to check that the day and week adjustment
		'''  did not change the month.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		'''  {@code DAY_OF_WEEK} - If all four are present, then they are combined to
		'''  form a {@code LocalDate}. The approach is the same as described above for
		'''  years, months and weeks in {@code ALIGNED_DAY_OF_WEEK_IN_MONTH}.
		'''  The day-of-week is adjusted as the next or same matching day-of-week once
		'''  the years, months and weeks have been handled.
		''' <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		'''  If all three are present, then they are combined to form a {@code LocalDate}.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first of January in the requested year, then adding
		'''  the difference in weeks, then in days.
		'''  If the mode is smart or strict, then the all three fields are validated to
		'''  their outer ranges. The date is then combined in a manner equivalent to
		'''  creating a date on the first day of the requested year, then adding
		'''  the amount in weeks and days to reach their values. If the mode is strict,
		'''  the date is additionally validated to check that the day and week adjustment
		'''  did not change the year.
		''' <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code DAY_OF_WEEK} -
		'''  If all three are present, then they are combined to form a {@code LocalDate}.
		'''  The approach is the same as described above for years and weeks in
		'''  {@code ALIGNED_DAY_OF_WEEK_IN_YEAR}. The day-of-week is adjusted as the
		'''  next or same matching day-of-week once the years and weeks have been handled.
		''' </ul>
		''' </summary>
		''' <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		''' <param name="resolverStyle">  the requested type of resolve, not null </param>
		''' <returns> the resolved date, null if insufficient information to create a date </returns>
		''' <exception cref="DateTimeException"> if the date cannot be resolved, typically
		'''  because of a conflict in the input data </exception>
		Public Overrides Function resolveDate(  fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?),   resolverStyle As java.time.format.ResolverStyle) As java.time.LocalDate ' override for performance
			Return CType(MyBase.resolveDate(fieldValues, resolverStyle), java.time.LocalDate)
		End Function

		Friend Overrides Sub resolveProlepticMonth(  fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?),   resolverStyle As java.time.format.ResolverStyle) ' override for better proleptic algorithm
			Dim pMonth As Long? = fieldValues.Remove(PROLEPTIC_MONTH)
			If pMonth IsNot Nothing Then
				If resolverStyle <> java.time.format.ResolverStyle.LENIENT Then PROLEPTIC_MONTH.checkValidValue(pMonth)
				addFieldValue(fieldValues, MONTH_OF_YEAR, System.Math.floorMod(pMonth, 12) + 1)
				addFieldValue(fieldValues, YEAR, System.Math.floorDiv(pMonth, 12))
			End If
		End Sub

		Friend Overrides Function resolveYearOfEra(  fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?),   resolverStyle As java.time.format.ResolverStyle) As java.time.LocalDate ' override for enhanced behaviour
			Dim yoeLong As Long? = fieldValues.Remove(YEAR_OF_ERA)
			If yoeLong IsNot Nothing Then
				If resolverStyle <> java.time.format.ResolverStyle.LENIENT Then YEAR_OF_ERA.checkValidValue(yoeLong)
				Dim era As Long? = fieldValues.Remove(ERA)
				If era Is Nothing Then
					Dim year_Renamed As Long? = fieldValues(YEAR)
					If resolverStyle = java.time.format.ResolverStyle.STRICT Then
						' do not invent era if strict, but do cross-check with year
						If year_Renamed IsNot Nothing Then
							addFieldValue(fieldValues, YEAR, (If(year_Renamed > 0, yoeLong, System.Math.subtractExact(1, yoeLong))))
						Else
							' reinstate the field removed earlier, no cross-check issues
							fieldValues(YEAR_OF_ERA) = yoeLong
						End If
					Else
						' invent era
						addFieldValue(fieldValues, YEAR, (If(year_Renamed Is Nothing OrElse year_Renamed > 0, yoeLong, System.Math.subtractExact(1, yoeLong))))
					End If
				ElseIf era = 1L Then
					addFieldValue(fieldValues, YEAR, yoeLong)
				ElseIf era = 0L Then
					addFieldValue(fieldValues, YEAR, System.Math.subtractExact(1, yoeLong))
				Else
					Throw New java.time.DateTimeException("Invalid value for era: " & era)
				End If
			ElseIf fieldValues.ContainsKey(ERA) Then
				ERA.checkValidValue(fieldValues(ERA)) ' always validated
			End If
			Return Nothing
		End Function

		Friend Overrides Function resolveYMD(j   fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?),   resolverStyle As java.time.format.ResolverStyle) As java.time.LocalDate ' override for performance
			Dim y As Integer = YEAR.checkValidIntValue(fieldValues.Remove(YEAR))
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim months As Long = System.Math.subtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1)
				Dim days As Long = System.Math.subtractExact(fieldValues.Remove(DAY_OF_MONTH), 1)
				Return java.time.LocalDate.of(y, 1, 1).plusMonths(months).plusDays(days)
			End If
			Dim moy As Integer = MONTH_OF_YEAR.checkValidIntValue(fieldValues.Remove(MONTH_OF_YEAR))
			Dim dom As Integer = DAY_OF_MONTH.checkValidIntValue(fieldValues.Remove(DAY_OF_MONTH))
			If resolverStyle = java.time.format.ResolverStyle.SMART Then ' previous valid
				If moy = 4 OrElse moy = 6 OrElse moy = 9 OrElse moy = 11 Then
					dom = System.Math.Min(dom, 30)
				ElseIf moy = 2 Then
					dom = System.Math.Min(dom, java.time.Month.FEBRUARY.length(java.time.Year.isLeap(y)))

				End If
			End If
			Return java.time.LocalDate.of(y, moy, dom)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function range(  field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange
			Return field.range()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a period for this chronology based on years, months and days.
		''' <p>
		''' This returns a period tied to the ISO chronology using the specified
		''' years, months and days. See <seealso cref="Period"/> for further details.
		''' </summary>
		''' <param name="years">  the number of years, may be negative </param>
		''' <param name="months">  the number of years, may be negative </param>
		''' <param name="days">  the number of years, may be negative </param>
		''' <returns> the period in terms of this chronology, not null </returns>
		''' <returns> the ISO period, not null </returns>
		Public Overrides Function period(  years As Integer,   months As Integer,   days As Integer) As java.time.Period ' override with covariant return type
			Return java.time.Period.of(years, months, days)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the Chronology using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(1);     // identifies a Chronology
		'''  out.writeUTF(getId());
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Friend Overrides Function writeReplace() As Object
			Return MyBase.writeReplace()
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub
	End Class

End Namespace