Imports System

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A date in the Japanese Imperial calendar system.
	''' <p>
	''' This date operates using the <seealso cref="JapaneseChronology Japanese Imperial calendar"/>.
	''' This calendar system is primarily used in Japan.
	''' <p>
	''' The Japanese Imperial calendar system is the same as the ISO calendar system
	''' apart from the era-based year numbering. The proleptic-year is defined to be
	''' equal to the ISO proleptic-year.
	''' <p>
	''' Japan introduced the Gregorian calendar starting with Meiji 6.
	''' Only Meiji and later eras are supported;
	''' dates before Meiji 6, January 1 are not supported.
	''' <p>
	''' For example, the Japanese year "Heisei 24" corresponds to ISO year "2012".<br>
	''' Calling {@code japaneseDate.get(YEAR_OF_ERA)} will return 24.<br>
	''' Calling {@code japaneseDate.get(YEAR)} will return 2012.<br>
	''' Calling {@code japaneseDate.get(ERA)} will return 2, corresponding to
	''' {@code JapaneseChronology.ERA_HEISEI}.<br>
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code JapaneseDate} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class JapaneseDate
		Inherits ChronoLocalDateImpl(Of JapaneseDate)
		Implements ChronoLocalDate

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -305327627230580483L

		''' <summary>
		''' The underlying ISO local date.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly isoDate As java.time.LocalDate
		''' <summary>
		''' The JapaneseEra of this date.
		''' </summary>
		<NonSerialized> _
		Private era As JapaneseEra
		''' <summary>
		''' The Japanese imperial calendar year of this date.
		''' </summary>
		<NonSerialized> _
		Private yearOfEra As Integer

		''' <summary>
		''' The first day supported by the JapaneseChronology is Meiji 6, January 1st.
		''' </summary>
		Friend Shared ReadOnly MEIJI_6_ISODATE As java.time.LocalDate = java.time.LocalDate.of(1873, 1, 1)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current {@code JapaneseDate} from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As JapaneseDate
			Return now(java.time.Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current {@code JapaneseDate} from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date using the system clock, not null </returns>
		Public Shared Function now(  zone As java.time.ZoneId) As JapaneseDate
			Return now(java.time.Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current {@code JapaneseDate} from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date, not null </returns>
		''' <exception cref="DateTimeException"> if the current date cannot be obtained </exception>
		Public Shared Function now(  clock_Renamed As java.time.Clock) As JapaneseDate
			Return New JapaneseDate(java.time.LocalDate.now(clock_Renamed))
		End Function

		''' <summary>
		''' Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		''' system from the era, year-of-era, month-of-year and day-of-month fields.
		''' <p>
		''' This returns a {@code JapaneseDate} with the specified fields.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' <p>
		''' The Japanese month and day-of-month are the same as those in the
		''' ISO calendar system. They are not reset when the era changes.
		''' For example:
		''' <pre>
		'''  6th Jan Showa 64 = ISO 1989-01-06
		'''  7th Jan Showa 64 = ISO 1989-01-07
		'''  8th Jan Heisei 1 = ISO 1989-01-08
		'''  9th Jan Heisei 1 = ISO 1989-01-09
		''' </pre>
		''' </summary>
		''' <param name="era">  the Japanese era, not null </param>
		''' <param name="yearOfEra">  the Japanese year-of-era </param>
		''' <param name="month">  the Japanese month-of-year, from 1 to 12 </param>
		''' <param name="dayOfMonth">  the Japanese day-of-month, from 1 to 31 </param>
		''' <returns> the date in Japanese calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year,
		'''  or if the date is not a Japanese era </exception>
		Public Shared Function [of](  era As JapaneseEra,   yearOfEra As Integer,   month As Integer,   dayOfMonth As Integer) As JapaneseDate
			java.util.Objects.requireNonNull(era, "era")
			Dim jdate As sun.util.calendar.LocalGregorianCalendar.Date = JapaneseChronology.JCAL.newCalendarDate(Nothing)
			jdate.eraEra(era.privateEra).setDate(yearOfEra, month, dayOfMonth)
			If Not JapaneseChronology.JCAL.validate(jdate) Then Throw New java.time.DateTimeException("year, month, and day not valid for Era")
			Dim date_Renamed As java.time.LocalDate = java.time.LocalDate.of(jdate.normalizedYear, month, dayOfMonth)
			Return New JapaneseDate(era, yearOfEra, date_Renamed)
		End Function

		''' <summary>
		''' Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		''' system from the proleptic-year, month-of-year and day-of-month fields.
		''' <p>
		''' This returns a {@code JapaneseDate} with the specified fields.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' <p>
		''' The Japanese proleptic year, month and day-of-month are the same as those
		''' in the ISO calendar system. They are not reset when the era changes.
		''' </summary>
		''' <param name="prolepticYear">  the Japanese proleptic-year </param>
		''' <param name="month">  the Japanese month-of-year, from 1 to 12 </param>
		''' <param name="dayOfMonth">  the Japanese day-of-month, from 1 to 31 </param>
		''' <returns> the date in Japanese calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](  prolepticYear As Integer,   month As Integer,   dayOfMonth As Integer) As JapaneseDate
			Return New JapaneseDate(java.time.LocalDate.of(prolepticYear, month, dayOfMonth))
		End Function

		''' <summary>
		''' Obtains a {@code JapaneseDate} representing a date in the Japanese calendar
		''' system from the era, year-of-era and day-of-year fields.
		''' <p>
		''' This returns a {@code JapaneseDate} with the specified fields.
		''' The day must be valid for the year, otherwise an exception will be thrown.
		''' <p>
		''' The day-of-year in this factory is expressed relative to the start of the year-of-era.
		''' This definition changes the normal meaning of day-of-year only in those years
		''' where the year-of-era is reset to one due to a change in the era.
		''' For example:
		''' <pre>
		'''  6th Jan Showa 64 = day-of-year 6
		'''  7th Jan Showa 64 = day-of-year 7
		'''  8th Jan Heisei 1 = day-of-year 1
		'''  9th Jan Heisei 1 = day-of-year 2
		''' </pre>
		''' </summary>
		''' <param name="era">  the Japanese era, not null </param>
		''' <param name="yearOfEra">  the Japanese year-of-era </param>
		''' <param name="dayOfYear">  the chronology day-of-year, from 1 to 366 </param>
		''' <returns> the date in Japanese calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-year is invalid for the year </exception>
		Shared Function ofYearDay(  era As JapaneseEra,   yearOfEra As Integer,   dayOfYear As Integer) As JapaneseDate
			java.util.Objects.requireNonNull(era, "era")
			Dim firstDay As sun.util.calendar.CalendarDate = era.privateEra.sinceDate
			Dim jdate As sun.util.calendar.LocalGregorianCalendar.Date = JapaneseChronology.JCAL.newCalendarDate(Nothing)
			jdate.era = era.privateEra
			If yearOfEra = 1 Then
				jdate.dateate(yearOfEra, firstDay.month, firstDay.dayOfMonth + dayOfYear - 1)
			Else
				jdate.dateate(yearOfEra, 1, dayOfYear)
			End If
			JapaneseChronology.JCAL.normalize(jdate)
			If era.privateEra IsNot jdate.era OrElse yearOfEra <> jdate.year Then Throw New java.time.DateTimeException("Invalid parameters")
			Dim localdate As java.time.LocalDate = java.time.LocalDate.of(jdate.normalizedYear, jdate.month, jdate.dayOfMonth)
			Return New JapaneseDate(era, yearOfEra, localdate)
		End Function

		''' <summary>
		''' Obtains a {@code JapaneseDate} from a temporal object.
		''' <p>
		''' This obtains a date in the Japanese calendar system based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code JapaneseDate}.
		''' <p>
		''' The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		''' field, which is standardized across calendar systems.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code JapaneseDate::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the date in Japanese calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code JapaneseDate} </exception>
		Public Shared Function [from](  temporal As java.time.temporal.TemporalAccessor) As JapaneseDate
			Return JapaneseChronology.INSTANCE.date(temporal)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance from an ISO date.
		''' </summary>
		''' <param name="isoDate">  the standard local date, validated not null </param>
		Friend Sub New(  isoDate As java.time.LocalDate)
			If isoDate.isBefore(MEIJI_6_ISODATE) Then Throw New java.time.DateTimeException("JapaneseDate before Meiji 6 is not supported")
			Dim jdate As sun.util.calendar.LocalGregorianCalendar.Date = toPrivateJapaneseDate(isoDate)
			Me.era = JapaneseEra.toJapaneseEra(jdate.era)
			Me.yearOfEra = jdate.year
			Me.isoDate = isoDate
		End Sub

		''' <summary>
		''' Constructs a {@code JapaneseDate}. This constructor does NOT validate the given parameters,
		''' and {@code era} and {@code year} must agree with {@code isoDate}.
		''' </summary>
		''' <param name="era">  the era, validated not null </param>
		''' <param name="year">  the year-of-era, validated </param>
		''' <param name="isoDate">  the standard local date, validated not null </param>
		Friend Sub New(  era As JapaneseEra,   year_Renamed As Integer,   isoDate As java.time.LocalDate)
			If isoDate.isBefore(MEIJI_6_ISODATE) Then Throw New java.time.DateTimeException("JapaneseDate before Meiji 6 is not supported")
			Me.era = era
			Me.yearOfEra = year_Renamed
			Me.isoDate = isoDate
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the chronology of this date, which is the Japanese calendar system.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		''' </summary>
		''' <returns> the Japanese chronology, not null </returns>
		Public  Overrides ReadOnly Property  chronology As JapaneseChronology
			Get
				Return JapaneseChronology.INSTANCE
			End Get
		End Property

		''' <summary>
		''' Gets the era applicable at this date.
		''' <p>
		''' The Japanese calendar system has multiple eras defined by <seealso cref="JapaneseEra"/>.
		''' </summary>
		''' <returns> the era applicable at this date, not null </returns>
		Public  Overrides ReadOnly Property  era As JapaneseEra
			Get
				Return era
			End Get
		End Property

		''' <summary>
		''' Returns the length of the month represented by this date.
		''' <p>
		''' This returns the length of the month in days.
		''' Month lengths match those of the ISO calendar system.
		''' </summary>
		''' <returns> the length of the month in days </returns>
		Public Overrides Function lengthOfMonth() As Integer Implements ChronoLocalDate.lengthOfMonth
			Return isoDate.lengthOfMonth()
		End Function

		Public Overrides Function lengthOfYear() As Integer Implements ChronoLocalDate.lengthOfYear
			Dim jcal As DateTime? = DateTime.getInstance(JapaneseChronology.LOCALE)
			jcal.Value.set(DateTime.ERA, era.value + JapaneseEra.ERA_OFFSET)
			jcal.Value.set(yearOfEra, isoDate.monthValue - 1, isoDate.dayOfMonth)
			Return jcal.Value.getActualMaximum(DateTime.DAY_OF_YEAR)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified field is supported.
		''' <p>
		''' This checks if this date can be queried for the specified field.
		''' If false, then calling the <seealso cref="#range(TemporalField) range"/> and
		''' <seealso cref="#get(TemporalField) get"/> methods will throw an exception.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The supported fields are:
		''' <ul>
		''' <li>{@code DAY_OF_WEEK}
		''' <li>{@code DAY_OF_MONTH}
		''' <li>{@code DAY_OF_YEAR}
		''' <li>{@code EPOCH_DAY}
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
		Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean Implements ChronoLocalDate.isSupported
			If field = ALIGNED_DAY_OF_WEEK_IN_MONTH OrElse field = ALIGNED_DAY_OF_WEEK_IN_YEAR OrElse field = ALIGNED_WEEK_OF_MONTH OrElse field = ALIGNED_WEEK_OF_YEAR Then Return False
			Return outerInstance.isSupported(field)
		End Function

		Public Overrides Function range(  field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				If isSupported(field) Then
					Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
					Select Case f
						Case DAY_OF_MONTH
							Return java.time.temporal.ValueRange.of(1, lengthOfMonth())
						Case DAY_OF_YEAR
							Return java.time.temporal.ValueRange.of(1, lengthOfYear())
						Case YEAR_OF_ERA
							Dim jcal As DateTime? = DateTime.getInstance(JapaneseChronology.LOCALE)
							jcal.Value.set(DateTime.ERA, era.value + JapaneseEra.ERA_OFFSET)
							jcal.Value.set(yearOfEra, isoDate.monthValue - 1, isoDate.dayOfMonth)
							Return java.time.temporal.ValueRange.of(1, jcal.Value.getActualMaximum(DateTime.YEAR))
					End Select
					Return chronology.range(f)
				End If
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.rangeRefinedBy(Me)
		End Function

		Public Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
			If TypeOf field Is java.time.temporal.ChronoField Then
				' same as ISO:
				' DAY_OF_WEEK, DAY_OF_MONTH, EPOCH_DAY, MONTH_OF_YEAR, PROLEPTIC_MONTH, YEAR
				'
				' calendar specific fields
				' DAY_OF_YEAR, YEAR_OF_ERA, ERA
				Select Case CType(field, java.time.temporal.ChronoField)
					Case ALIGNED_DAY_OF_WEEK_IN_MONTH, ALIGNED_DAY_OF_WEEK_IN_YEAR, ALIGNED_WEEK_OF_MONTH, ALIGNED_WEEK_OF_YEAR
						Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
					Case YEAR_OF_ERA
						Return yearOfEra
					Case ERA
						Return era.value
					Case DAY_OF_YEAR
						Dim jcal As DateTime? = DateTime.getInstance(JapaneseChronology.LOCALE)
						jcal.Value.set(DateTime.ERA, era.value + JapaneseEra.ERA_OFFSET)
						jcal.Value.set(yearOfEra, isoDate.monthValue - 1, isoDate.dayOfMonth)
						Return jcal.Value.get(DateTime.DAY_OF_YEAR)
				End Select
				Return isoDate.getLong(field)
			End If
			Return field.getFrom(Me)
		End Function

		''' <summary>
		''' Returns a {@code LocalGregorianCalendar.Date} converted from the given {@code isoDate}.
		''' </summary>
		''' <param name="isoDate">  the local date, not null </param>
		''' <returns> a {@code LocalGregorianCalendar.Date}, not null </returns>
		Private Shared Function toPrivateJapaneseDate(  isoDate As java.time.LocalDate) As sun.util.calendar.LocalGregorianCalendar.Date
			Dim jdate As sun.util.calendar.LocalGregorianCalendar.Date = JapaneseChronology.JCAL.newCalendarDate(Nothing)
			Dim sunEra As sun.util.calendar.Era = JapaneseEra.privateEraFrom(isoDate)
			Dim year_Renamed As Integer = isoDate.year
			If sunEra IsNot Nothing Then year_Renamed -= sunEra.sinceDate.year - 1
			jdate.eraEra(sunEra).setYear(year_Renamed).setMonth(isoDate.monthValue).setDayOfMonth(isoDate.dayOfMonth)
			JapaneseChronology.JCAL.normalize(jdate)
			Return jdate
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function [with](  field As java.time.temporal.TemporalField,   newValue As Long) As JapaneseDate
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				If getLong(f) = newValue Then ' getLong() validates for supported fields Return Me
				Select Case f
					Case YEAR_OF_ERA, YEAR, ERA
						Dim nvalue As Integer = chronology.range(f).checkValidIntValue(newValue, f)
						Select Case f
							Case YEAR_OF_ERA
								Return Me.withYear(nvalue)
							Case YEAR
								Return [with](isoDate.withYear(nvalue))
							Case ERA
								Return Me.withYear(JapaneseEra.of(nvalue), yearOfEra)
						End Select
				End Select
				' YEAR, PROLEPTIC_MONTH and others are same as ISO
				Return [with](isoDate.with(field, newValue))
			End If
			Return MyBase.with(field, newValue)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function [with](  adjuster As java.time.temporal.TemporalAdjuster) As JapaneseDate
			Return MyBase.with(adjuster)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function plus(  amount As java.time.temporal.TemporalAmount) As JapaneseDate
			Return MyBase.plus(amount)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function minus(  amount As java.time.temporal.TemporalAmount) As JapaneseDate
			Return MyBase.minus(amount)
		End Function
		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date with the year altered.
		''' <p>
		''' This method changes the year of the date.
		''' If the month-day is invalid for the year, then the previous valid day
		''' will be selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="era">  the era to set in the result, not null </param>
		''' <param name="yearOfEra">  the year-of-era to set in the returned date </param>
		''' <returns> a {@code JapaneseDate} based on this date with the requested year, never null </returns>
		''' <exception cref="DateTimeException"> if {@code year} is invalid </exception>
		Private Function withYear(  era As JapaneseEra,   yearOfEra As Integer) As JapaneseDate
			Dim year_Renamed As Integer = JapaneseChronology.INSTANCE.prolepticYear(era, yearOfEra)
			Return [with](isoDate.withYear(year_Renamed))
		End Function

		''' <summary>
		''' Returns a copy of this date with the year-of-era altered.
		''' <p>
		''' This method changes the year-of-era of the date.
		''' If the month-day is invalid for the year, then the previous valid day
		''' will be selected instead.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="year">  the year to set in the returned date </param>
		''' <returns> a {@code JapaneseDate} based on this date with the requested year-of-era, never null </returns>
		''' <exception cref="DateTimeException"> if {@code year} is invalid </exception>
		Private Function withYear(  year_Renamed As Integer) As JapaneseDate
			Return withYear(era, year_Renamed)
		End Function

		'-----------------------------------------------------------------------
		Friend Overrides Function plusYears(  years As Long) As JapaneseDate
			Return [with](isoDate.plusYears(years))
		End Function

		Friend Overrides Function plusMonths(  months As Long) As JapaneseDate
			Return [with](isoDate.plusMonths(months))
		End Function

		Friend Overrides Function plusWeeks(  weeksToAdd As Long) As JapaneseDate
			Return [with](isoDate.plusWeeks(weeksToAdd))
		End Function

		Friend Overrides Function plusDays(  days As Long) As JapaneseDate
			Return [with](isoDate.plusDays(days))
		End Function

		Public Overrides Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As JapaneseDate
			Return MyBase.plus(amountToAdd, unit)
		End Function

		Public Overrides Function minus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As JapaneseDate
			Return MyBase.minus(amountToAdd, unit)
		End Function

		Friend Overrides Function minusYears(  yearsToSubtract As Long) As JapaneseDate
			Return MyBase.minusYears(yearsToSubtract)
		End Function

		Friend Overrides Function minusMonths(  monthsToSubtract As Long) As JapaneseDate
			Return MyBase.minusMonths(monthsToSubtract)
		End Function

		Friend Overrides Function minusWeeks(  weeksToSubtract As Long) As JapaneseDate
			Return MyBase.minusWeeks(weeksToSubtract)
		End Function

		Friend Overrides Function minusDays(  daysToSubtract As Long) As JapaneseDate
			Return MyBase.minusDays(daysToSubtract)
		End Function

		Private Function [with](  newDate As java.time.LocalDate) As JapaneseDate
			[Return] (If(newDate.Equals(isoDate), Me, New JapaneseDate(newDate)))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function atTime(  localTime_Renamed As java.time.LocalTime) As ChronoLocalDateTime(Of JapaneseDate) Implements ChronoLocalDate.atTime ' for javadoc and covariant return type
			Return CType(MyBase.atTime(localTime_Renamed), ChronoLocalDateTime(Of JapaneseDate))
		End Function

		Public Overrides Function [until](  endDate As ChronoLocalDate) As ChronoPeriod Implements ChronoLocalDate.until
			Dim period_Renamed As java.time.Period = isoDate.until(endDate)
			Return chronology.period(period_Renamed.years, period_Renamed.months, period_Renamed.days)
		End Function

		Public Overrides Function toEpochDay() As Long Implements ChronoLocalDate.toEpochDay ' override for performance
			Return isoDate.toEpochDay()
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Compares this date to another date, including the chronology.
		''' <p>
		''' Compares this {@code JapaneseDate} with another ensuring that the date is the same.
		''' <p>
		''' Only objects of type {@code JapaneseDate} are compared, other types return false.
		''' To compare the dates of two {@code TemporalAccessor} instances, including dates
		''' in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean ' override for performance
			If Me Is obj Then Return True
			If TypeOf obj Is JapaneseDate Then
				Dim otherDate As JapaneseDate = CType(obj, JapaneseDate)
				Return Me.isoDate.Equals(otherDate.isoDate)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this date.
		''' </summary>
		''' <returns> a suitable hash code based only on the Chronology and the date </returns>
		Public Overrides Function GetHashCode() As Integer ' override for performance
			Return chronology.id.GetHashCode() Xor isoDate.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(4);                 // identifies a JapaneseDate
		'''  out.writeInt(get(YEAR));
		'''  out.writeByte(get(MONTH_OF_YEAR));
		'''  out.writeByte(get(DAY_OF_MONTH));
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.JAPANESE_DATE_TYPE, Me)
		End Function

		Friend Sub writeExternal(  out As java.io.DataOutput)
			' JapaneseChronology is implicit in the JAPANESE_DATE_TYPE
			out.writeInt(get(YEAR))
			out.writeByte(get(MONTH_OF_YEAR))
			out.writeByte(get(DAY_OF_MONTH))
		End Sub

		Shared Function readExternal(  [in] As java.io.DataInput) As JapaneseDate
			Dim year_Renamed As Integer = [in].readInt()
			Dim month As Integer = [in].readByte()
			Dim dayOfMonth As Integer = [in].readByte()
			Return JapaneseChronology.INSTANCE.date(year_Renamed, month, dayOfMonth)
		End Function

	End Class

End Namespace