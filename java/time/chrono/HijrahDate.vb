Imports Microsoft.VisualBasic
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
	''' A date in the Hijrah calendar system.
	''' <p>
	''' This date operates using one of several variants of the
	''' <seealso cref="HijrahChronology Hijrah calendar"/>.
	''' <p>
	''' The Hijrah calendar has a different total of days in a year than
	''' Gregorian calendar, and the length of each month is based on the period
	''' of a complete revolution of the moon around the earth
	''' (as between successive new moons).
	''' Refer to the <seealso cref="HijrahChronology"/> for details of supported variants.
	''' <p>
	''' Each HijrahDate is created bound to a particular HijrahChronology,
	''' The same chronology is propagated to each HijrahDate computed from the date.
	''' To use a different Hijrah variant, its HijrahChronology can be used
	''' to create new HijrahDate instances.
	''' Alternatively, the <seealso cref="#withVariant"/> method can be used to convert
	''' to a new HijrahChronology.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code HijrahDate} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class HijrahDate
		Inherits ChronoLocalDateImpl(Of HijrahDate)
		Implements ChronoLocalDate

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -5207853542612002020L
		''' <summary>
		''' The Chronology of this HijrahDate.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly chrono As HijrahChronology
		''' <summary>
		''' The proleptic year.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly prolepticYear As Integer
		''' <summary>
		''' The month-of-year.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly monthOfYear As Integer
		''' <summary>
		''' The day-of-month.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly dayOfMonth As Integer

		'-------------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code HijrahDate} from the Hijrah proleptic year,
		''' month-of-year and day-of-month.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic year to represent in the Hijrah calendar </param>
		''' <param name="monthOfYear">  the month-of-year to represent, from 1 to 12 </param>
		''' <param name="dayOfMonth">  the day-of-month to represent, from 1 to 30 </param>
		''' <returns> the Hijrah date, never null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range </exception>
		Shared Function [of](ByVal chrono As HijrahChronology, ByVal prolepticYear As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer) As HijrahDate
			Return New HijrahDate(chrono, prolepticYear, monthOfYear, dayOfMonth)
		End Function

		''' <summary>
		''' Returns a HijrahDate for the chronology and epochDay. </summary>
		''' <param name="chrono"> The Hijrah chronology </param>
		''' <param name="epochDay"> the epoch day </param>
		''' <returns> a HijrahDate for the epoch day; non-null </returns>
		Shared Function ofEpochDay(ByVal chrono As HijrahChronology, ByVal epochDay As Long) As HijrahDate
			Return New HijrahDate(chrono, epochDay)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		''' in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As HijrahDate
			Return now(java.time.Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		''' in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As java.time.ZoneId) As HijrahDate
			Return now(java.time.Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		''' from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date, not null </returns>
		''' <exception cref="DateTimeException"> if the current date cannot be obtained </exception>
		Public Shared Function now(ByVal clock_Renamed As java.time.Clock) As HijrahDate
			Return HijrahDate.ofEpochDay(HijrahChronology.INSTANCE, java.time.LocalDate.now(clock_Renamed).toEpochDay())
		End Function

		''' <summary>
		''' Obtains a {@code HijrahDate} of the Islamic Umm Al-Qura calendar
		''' from the proleptic-year, month-of-year and day-of-month fields.
		''' <p>
		''' This returns a {@code HijrahDate} with the specified fields.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="prolepticYear">  the Hijrah proleptic-year </param>
		''' <param name="month">  the Hijrah month-of-year, from 1 to 12 </param>
		''' <param name="dayOfMonth">  the Hijrah day-of-month, from 1 to 30 </param>
		''' <returns> the date in Hijrah calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As HijrahDate
			Return HijrahChronology.INSTANCE.date(prolepticYear, month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains a {@code HijrahDate} of the Islamic Umm Al-Qura calendar from a temporal object.
		''' <p>
		''' This obtains a date in the Hijrah calendar system based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code HijrahDate}.
		''' <p>
		''' The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		''' field, which is standardized across calendar systems.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code HijrahDate::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the date in Hijrah calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code HijrahDate} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As HijrahDate
			Return HijrahChronology.INSTANCE.date(temporal)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructs an {@code HijrahDate} with the proleptic-year, month-of-year and
		''' day-of-month fields.
		''' </summary>
		''' <param name="chrono"> The chronology to create the date with </param>
		''' <param name="prolepticYear"> the proleptic year </param>
		''' <param name="monthOfYear"> the month of year </param>
		''' <param name="dayOfMonth"> the day of month </param>
		Private Sub New(ByVal chrono As HijrahChronology, ByVal prolepticYear As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer)
			' Computing the Gregorian day checks the valid ranges
			chrono.getEpochDay(prolepticYear, monthOfYear, dayOfMonth)

			Me.chrono = chrono
			Me.prolepticYear = prolepticYear
			Me.monthOfYear = monthOfYear
			Me.dayOfMonth = dayOfMonth
		End Sub

		''' <summary>
		''' Constructs an instance with the Epoch Day.
		''' </summary>
		''' <param name="epochDay">  the epochDay </param>
		Private Sub New(ByVal chrono As HijrahChronology, ByVal epochDay As Long)
			Dim dateInfo As Integer() = chrono.getHijrahDateInfo(CInt(epochDay))

			Me.chrono = chrono
			Me.prolepticYear = dateInfo(0)
			Me.monthOfYear = dateInfo(1)
			Me.dayOfMonth = dateInfo(2)
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the chronology of this date, which is the Hijrah calendar system.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		''' </summary>
		''' <returns> the Hijrah chronology, not null </returns>
		Public  Overrides ReadOnly Property  chronology As HijrahChronology
			Get
				Return chrono
			End Get
		End Property

		''' <summary>
		''' Gets the era applicable at this date.
		''' <p>
		''' The Hijrah calendar system has one era, 'AH',
		''' defined by <seealso cref="HijrahEra"/>.
		''' </summary>
		''' <returns> the era applicable at this date, not null </returns>
		Public  Overrides ReadOnly Property  era As HijrahEra
			Get
				Return HijrahEra.AH
			End Get
		End Property

		''' <summary>
		''' Returns the length of the month represented by this date.
		''' <p>
		''' This returns the length of the month in days.
		''' Month lengths in the Hijrah calendar system vary between 29 and 30 days.
		''' </summary>
		''' <returns> the length of the month in days </returns>
		Public Overrides Function lengthOfMonth() As Integer Implements ChronoLocalDate.lengthOfMonth
			Return chrono.getMonthLength(prolepticYear, monthOfYear)
		End Function

		''' <summary>
		''' Returns the length of the year represented by this date.
		''' <p>
		''' This returns the length of the year in days.
		''' A Hijrah calendar system year is typically shorter than
		''' that of the ISO calendar system.
		''' </summary>
		''' <returns> the length of the year in days </returns>
		Public Overrides Function lengthOfYear() As Integer Implements ChronoLocalDate.lengthOfYear
			Return chrono.getYearLength(prolepticYear)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				If isSupported(field) Then
					Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
					Select Case f
						Case DAY_OF_MONTH
							Return java.time.temporal.ValueRange.of(1, lengthOfMonth())
						Case DAY_OF_YEAR
							Return java.time.temporal.ValueRange.of(1, lengthOfYear())
						Case ALIGNED_WEEK_OF_MONTH ' TODO
							Return java.time.temporal.ValueRange.of(1, 5)
						' TODO does the limited range of valid years cause years to
						' start/end part way through? that would affect range
					End Select
					Return chronology.range(f)
				End If
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.rangeRefinedBy(Me)
		End Function

		Public Overrides Function getLong(ByVal field As java.time.temporal.TemporalField) As Long
			If TypeOf field Is java.time.temporal.ChronoField Then
				Select Case CType(field, java.time.temporal.ChronoField)
					Case DAY_OF_WEEK
						Return dayOfWeek
					Case ALIGNED_DAY_OF_WEEK_IN_MONTH
						[Return] ((dayOfWeek - 1) Mod 7) + 1
					Case ALIGNED_DAY_OF_WEEK_IN_YEAR
						[Return] ((dayOfYear - 1) Mod 7) + 1
					Case DAY_OF_MONTH
						Return Me.dayOfMonth
					Case DAY_OF_YEAR
						Return Me.dayOfYear
					Case EPOCH_DAY
						Return toEpochDay()
					Case ALIGNED_WEEK_OF_MONTH
						[Return] ((dayOfMonth - 1) \ 7) + 1
					Case ALIGNED_WEEK_OF_YEAR
						[Return] ((dayOfYear - 1) \ 7) + 1
					Case MONTH_OF_YEAR
						Return monthOfYear
					Case PROLEPTIC_MONTH
						Return prolepticMonth
					Case YEAR_OF_ERA
						Return prolepticYear
					Case YEAR
						Return prolepticYear
					Case ERA
						Return eraValue
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return field.getFrom(Me)
		End Function

		Private Property prolepticMonth As Long
			Get
				Return prolepticYear * 12L + monthOfYear - 1
			End Get
		End Property

		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As HijrahDate
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				' not using checkValidIntValue so EPOCH_DAY and PROLEPTIC_MONTH work
				chrono.range(f).checkValidValue(newValue, f) ' TODO: validate value
				Dim nvalue As Integer = CInt(newValue)
				Select Case f
					Case DAY_OF_WEEK
						Return plusDays(newValue - dayOfWeek)
					Case ALIGNED_DAY_OF_WEEK_IN_MONTH
						Return plusDays(newValue - getLong(ALIGNED_DAY_OF_WEEK_IN_MONTH))
					Case ALIGNED_DAY_OF_WEEK_IN_YEAR
						Return plusDays(newValue - getLong(ALIGNED_DAY_OF_WEEK_IN_YEAR))
					Case DAY_OF_MONTH
						Return resolvePreviousValid(prolepticYear, monthOfYear, nvalue)
					Case DAY_OF_YEAR
						Return plusDays (System.Math.Min(nvalue, lengthOfYear()) - dayOfYear)
					Case EPOCH_DAY
						Return New HijrahDate(chrono, newValue)
					Case ALIGNED_WEEK_OF_MONTH
						Return plusDays((newValue - getLong(ALIGNED_WEEK_OF_MONTH)) * 7)
					Case ALIGNED_WEEK_OF_YEAR
						Return plusDays((newValue - getLong(ALIGNED_WEEK_OF_YEAR)) * 7)
					Case MONTH_OF_YEAR
						Return resolvePreviousValid(prolepticYear, nvalue, dayOfMonth)
					Case PROLEPTIC_MONTH
						Return plusMonths(newValue - prolepticMonth)
					Case YEAR_OF_ERA
						Return resolvePreviousValid(If(prolepticYear >= 1, nvalue, 1 - nvalue), monthOfYear, dayOfMonth)
					Case YEAR
						Return resolvePreviousValid(nvalue, monthOfYear, dayOfMonth)
					Case ERA
						Return resolvePreviousValid(1 - prolepticYear, monthOfYear, dayOfMonth)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			End If
			Return MyBase.with(field, newValue)
		End Function

		Private Function resolvePreviousValid(ByVal prolepticYear As Integer, ByVal month As Integer, ByVal day As Integer) As HijrahDate
			Dim monthDays As Integer = chrono.getMonthLength(prolepticYear, month)
			If day > monthDays Then day = monthDays
			Return HijrahDate.of(chrono, prolepticYear, month, day)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> if unable to make the adjustment.
		'''     For example, if the adjuster requires an ISO chronology </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As HijrahDate
			Return MyBase.with(adjuster)
		End Function

		''' <summary>
		''' Returns a {@code HijrahDate} with the Chronology requested.
		''' <p>
		''' The year, month, and day are checked against the new requested
		''' HijrahChronology.  If the chronology has a shorter month length
		''' for the month, the day is reduced to be the last day of the month.
		''' </summary>
		''' <param name="chronology"> the new HijrahChonology, non-null </param>
		''' <returns> a HijrahDate with the requested HijrahChronology, non-null </returns>
		Public Function withVariant(ByVal chronology As HijrahChronology) As HijrahDate
			If chrono Is chronology Then Return Me
			' Like resolvePreviousValid the day is constrained to stay in the same month
			Dim monthDays As Integer = chronology.getDayOfYear(prolepticYear, monthOfYear)
			Return HijrahDate.of(chronology, prolepticYear, monthOfYear,If(dayOfMonth > monthDays, monthDays, dayOfMonth))
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function plus(ByVal amount As java.time.temporal.TemporalAmount) As HijrahDate
			Return MyBase.plus(amount)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function minus(ByVal amount As java.time.temporal.TemporalAmount) As HijrahDate
			Return MyBase.minus(amount)
		End Function

		Public Overrides Function toEpochDay() As Long Implements ChronoLocalDate.toEpochDay
			Return chrono.getEpochDay(prolepticYear, monthOfYear, dayOfMonth)
		End Function

		''' <summary>
		''' Gets the day-of-year field.
		''' <p>
		''' This method returns the primitive {@code int} value for the day-of-year.
		''' </summary>
		''' <returns> the day-of-year </returns>
		Private Property dayOfYear As Integer
			Get
				Return chrono.getDayOfYear(prolepticYear, monthOfYear) + dayOfMonth
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-week value.
		''' </summary>
		''' <returns> the day-of-week; computed from the epochday </returns>
		Private Property dayOfWeek As Integer
			Get
				Dim dow0 As Integer = CInt (System.Math.floorMod(toEpochDay() + 3, 7))
				Return dow0 + 1
			End Get
		End Property

		''' <summary>
		''' Gets the Era of this date.
		''' </summary>
		''' <returns> the Era of this date; computed from epochDay </returns>
		Private Property eraValue As Integer
			Get
				[Return] (If(prolepticYear > 1, 1, 0))
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the year is a leap year, according to the Hijrah calendar system rules.
		''' </summary>
		''' <returns> true if this date is in a leap year </returns>
		Public  Overrides ReadOnly Property  leapYear As Boolean Implements ChronoLocalDate.isLeapYear
			Get
				Return chrono.isLeapYear(prolepticYear)
			End Get
		End Property

		'-----------------------------------------------------------------------
		Friend Overrides Function plusYears(ByVal years As Long) As HijrahDate
			If years = 0 Then Return Me
			Dim newYear As Integer = System.Math.addExact(Me.prolepticYear, CInt(years))
			Return resolvePreviousValid(newYear, monthOfYear, dayOfMonth)
		End Function

		Friend Overrides Function plusMonths(ByVal monthsToAdd As Long) As HijrahDate
			If monthsToAdd = 0 Then Return Me
			Dim monthCount As Long = prolepticYear * 12L + (monthOfYear - 1)
			Dim calcMonths As Long = monthCount + monthsToAdd ' safe overflow
			Dim newYear As Integer = chrono.checkValidYear (System.Math.floorDiv(calcMonths, 12L))
			Dim newMonth As Integer = CInt (System.Math.floorMod(calcMonths, 12L)) + 1
			Return resolvePreviousValid(newYear, newMonth, dayOfMonth)
		End Function

		Friend Overrides Function plusWeeks(ByVal weeksToAdd As Long) As HijrahDate
			Return MyBase.plusWeeks(weeksToAdd)
		End Function

		Friend Overrides Function plusDays(ByVal days As Long) As HijrahDate
			Return New HijrahDate(chrono, toEpochDay() + days)
		End Function

		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As HijrahDate
			Return MyBase.plus(amountToAdd, unit)
		End Function

		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As HijrahDate
			Return MyBase.minus(amountToSubtract, unit)
		End Function

		Friend Overrides Function minusYears(ByVal yearsToSubtract As Long) As HijrahDate
			Return MyBase.minusYears(yearsToSubtract)
		End Function

		Friend Overrides Function minusMonths(ByVal monthsToSubtract As Long) As HijrahDate
			Return MyBase.minusMonths(monthsToSubtract)
		End Function

		Friend Overrides Function minusWeeks(ByVal weeksToSubtract As Long) As HijrahDate
			Return MyBase.minusWeeks(weeksToSubtract)
		End Function

		Friend Overrides Function minusDays(ByVal daysToSubtract As Long) As HijrahDate
			Return MyBase.minusDays(daysToSubtract)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function atTime(ByVal localTime_Renamed As java.time.LocalTime) As ChronoLocalDateTime(Of HijrahDate) Implements ChronoLocalDate.atTime ' for javadoc and covariant return type
			Return CType(MyBase.atTime(localTime_Renamed), ChronoLocalDateTime(Of HijrahDate))
		End Function

		Public Overrides Function [until](ByVal endDate As ChronoLocalDate) As ChronoPeriod Implements ChronoLocalDate.until
			' TODO: untested
			Dim [end] As HijrahDate = chronology.date(endDate)
			Dim totalMonths As Long = ([end].prolepticYear - Me.prolepticYear) * 12 + ([end].monthOfYear - Me.monthOfYear) ' safe
			Dim days As Integer = [end].dayOfMonth - Me.dayOfMonth
			If totalMonths > 0 AndAlso days < 0 Then
				totalMonths -= 1
				Dim calcDate As HijrahDate = Me.plusMonths(totalMonths)
				days = CInt([end].toEpochDay() - calcDate.toEpochDay()) ' safe
			ElseIf totalMonths < 0 AndAlso days > 0 Then
				totalMonths += 1
				days -= [end].lengthOfMonth()
			End If
			Dim years As Long = totalMonths \ 12 ' safe
			Dim months As Integer = CInt(Fix(totalMonths Mod 12)) ' safe
			Return chronology.period (System.Math.toIntExact(years), months, days)
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Compares this date to another date, including the chronology.
		''' <p>
		''' Compares this {@code HijrahDate} with another ensuring that the date is the same.
		''' <p>
		''' Only objects of type {@code HijrahDate} are compared, other types return false.
		''' To compare the dates of two {@code TemporalAccessor} instances, including dates
		''' in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date and the Chronologies are equal </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean ' override for performance
			If Me Is obj Then Return True
			If TypeOf obj Is HijrahDate Then
				Dim otherDate As HijrahDate = CType(obj, HijrahDate)
				Return prolepticYear = otherDate.prolepticYear AndAlso Me.monthOfYear = otherDate.monthOfYear AndAlso Me.dayOfMonth = otherDate.dayOfMonth AndAlso chronology.Equals(otherDate.chronology)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this date.
		''' </summary>
		''' <returns> a suitable hash code based only on the Chronology and the date </returns>
		Public Overrides Function GetHashCode() As Integer ' override for performance
			Dim yearValue As Integer = prolepticYear
			Dim monthValue As Integer = monthOfYear
			Dim dayValue As Integer = dayOfMonth
			Return chronology.id.GetHashCode() Xor (yearValue And &HFFFFF800L) Xor ((yearValue << 11) + (monthValue << 6) + (dayValue))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(6);                 // identifies a HijrahDate
		'''  out.writeObject(chrono);          // the HijrahChronology variant
		'''  out.writeInt(get(YEAR));
		'''  out.writeByte(get(MONTH_OF_YEAR));
		'''  out.writeByte(get(DAY_OF_MONTH));
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.HIJRAH_DATE_TYPE, Me)
		End Function

		Friend Sub writeExternal(ByVal out As java.io.ObjectOutput)
			' HijrahChronology is implicit in the Hijrah_DATE_TYPE
			out.writeObject(chronology)
			out.writeInt(get(YEAR))
			out.writeByte(get(MONTH_OF_YEAR))
			out.writeByte(get(DAY_OF_MONTH))
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.ObjectInput) As HijrahDate
			Dim chrono As HijrahChronology = CType([in].readObject(), HijrahChronology)
			Dim year_Renamed As Integer = [in].readInt()
			Dim month As Integer = [in].readByte()
			Dim dayOfMonth As Integer = [in].readByte()
			Return chrono.date(year_Renamed, month, dayOfMonth)
		End Function

	End Class

End Namespace