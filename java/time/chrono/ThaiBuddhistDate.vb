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
	''' A date in the Thai Buddhist calendar system.
	''' <p>
	''' This date operates using the <seealso cref="ThaiBuddhistChronology Thai Buddhist calendar"/>.
	''' This calendar system is primarily used in Thailand.
	''' Dates are aligned such that {@code 2484-01-01 (Buddhist)} is {@code 1941-01-01 (ISO)}.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code ThaiBuddhistDate} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ThaiBuddhistDate
		Inherits ChronoLocalDateImpl(Of ThaiBuddhistDate)
		Implements ChronoLocalDate

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -8722293800195731463L

		''' <summary>
		''' The underlying date.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly isoDate As java.time.LocalDate

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current {@code ThaiBuddhistDate} from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <returns> the current date using the system clock and default time-zone, not null </returns>
		Public Shared Function now() As ThaiBuddhistDate
			Return now(java.time.Clock.systemDefaultZone())
		End Function

		''' <summary>
		''' Obtains the current {@code ThaiBuddhistDate} from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current date using the system clock, not null </returns>
		Public Shared Function now(ByVal zone As java.time.ZoneId) As ThaiBuddhistDate
			Return now(java.time.Clock.system(zone))
		End Function

		''' <summary>
		''' Obtains the current {@code ThaiBuddhistDate} from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current date, not null </returns>
		''' <exception cref="DateTimeException"> if the current date cannot be obtained </exception>
		Public Shared Function now(ByVal clock_Renamed As java.time.Clock) As ThaiBuddhistDate
			Return New ThaiBuddhistDate(java.time.LocalDate.now(clock_Renamed))
		End Function

		''' <summary>
		''' Obtains a {@code ThaiBuddhistDate} representing a date in the Thai Buddhist calendar
		''' system from the proleptic-year, month-of-year and day-of-month fields.
		''' <p>
		''' This returns a {@code ThaiBuddhistDate} with the specified fields.
		''' The day must be valid for the year and month, otherwise an exception will be thrown.
		''' </summary>
		''' <param name="prolepticYear">  the Thai Buddhist proleptic-year </param>
		''' <param name="month">  the Thai Buddhist month-of-year, from 1 to 12 </param>
		''' <param name="dayOfMonth">  the Thai Buddhist day-of-month, from 1 to 31 </param>
		''' <returns> the date in Thai Buddhist calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if the value of any field is out of range,
		'''  or if the day-of-month is invalid for the month-year </exception>
		Public Shared Function [of](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As ThaiBuddhistDate
			Return New ThaiBuddhistDate(java.time.LocalDate.of(prolepticYear - YEARS_DIFFERENCE, month, dayOfMonth))
		End Function

		''' <summary>
		''' Obtains a {@code ThaiBuddhistDate} from a temporal object.
		''' <p>
		''' This obtains a date in the Thai Buddhist calendar system based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ThaiBuddhistDate}.
		''' <p>
		''' The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		''' field, which is standardized across calendar systems.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code ThaiBuddhistDate::from}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the date in Thai Buddhist calendar system, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code ThaiBuddhistDate} </exception>
		Public Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As ThaiBuddhistDate
			Return ThaiBuddhistChronology.INSTANCE.date(temporal)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance from an ISO date.
		''' </summary>
		''' <param name="isoDate">  the standard local date, validated not null </param>
		Friend Sub New(ByVal isoDate As java.time.LocalDate)
			java.util.Objects.requireNonNull(isoDate, "isoDate")
			Me.isoDate = isoDate
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the chronology of this date, which is the Thai Buddhist calendar system.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The era and other fields in <seealso cref="ChronoField"/> are defined by the chronology.
		''' </summary>
		''' <returns> the Thai Buddhist chronology, not null </returns>
		Public  Overrides ReadOnly Property  chronology As ThaiBuddhistChronology
			Get
				Return ThaiBuddhistChronology.INSTANCE
			End Get
		End Property

		''' <summary>
		''' Gets the era applicable at this date.
		''' <p>
		''' The Thai Buddhist calendar system has two eras, 'BE' and 'BEFORE_BE',
		''' defined by <seealso cref="ThaiBuddhistEra"/>.
		''' </summary>
		''' <returns> the era applicable at this date, not null </returns>
		Public  Overrides ReadOnly Property  era As ThaiBuddhistEra
			Get
				[Return] (If(prolepticYear >= 1, ThaiBuddhistEra.BE, ThaiBuddhistEra.BEFORE_BE))
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

		'-----------------------------------------------------------------------
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				If isSupported(field) Then
					Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
					Select Case f
						Case DAY_OF_MONTH, DAY_OF_YEAR, ALIGNED_WEEK_OF_MONTH
							Return isoDate.range(field)
						Case YEAR_OF_ERA
							Dim range_Renamed As java.time.temporal.ValueRange = YEAR.range()
							Dim max As Long = (If(prolepticYear <= 0, -(range_Renamed.minimum + YEARS_DIFFERENCE) + 1, range_Renamed.maximum + YEARS_DIFFERENCE))
							Return java.time.temporal.ValueRange.of(1, max)
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
					Case PROLEPTIC_MONTH
						Return prolepticMonth
					Case YEAR_OF_ERA
						Dim prolepticYear_Renamed As Integer = prolepticYear
						[Return] (If(prolepticYear_Renamed >= 1, prolepticYear_Renamed, 1 - prolepticYear_Renamed))
					Case YEAR
						Return prolepticYear
					Case ERA
						[Return] (If(prolepticYear >= 1, 1, 0))
				End Select
				Return isoDate.getLong(field)
			End If
			Return field.getFrom(Me)
		End Function

		Private Property prolepticMonth As Long
			Get
				Return prolepticYear * 12L + isoDate.monthValue - 1
			End Get
		End Property

		Private Property prolepticYear As Integer
			Get
				Return isoDate.year + YEARS_DIFFERENCE
			End Get
		End Property

		'-----------------------------------------------------------------------
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As ThaiBuddhistDate
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				If getLong(f) = newValue Then Return Me
				Select Case f
					Case PROLEPTIC_MONTH
						chronology.range(f).checkValidValue(newValue, f)
						Return plusMonths(newValue - prolepticMonth)
					Case YEAR_OF_ERA, YEAR, ERA
						Dim nvalue As Integer = chronology.range(f).checkValidIntValue(newValue, f)
						Select Case f
							Case YEAR_OF_ERA
								Return [with](isoDate.withYear((If(prolepticYear >= 1, nvalue, 1 - nvalue)) - YEARS_DIFFERENCE))
							Case YEAR
								Return [with](isoDate.withYear(nvalue - YEARS_DIFFERENCE))
							Case ERA
								Return [with](isoDate.withYear((1 - prolepticYear) - YEARS_DIFFERENCE))
						End Select
				End Select
				Return [with](isoDate.with(field, newValue))
			End If
			Return MyBase.with(field, newValue)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As ThaiBuddhistDate
			Return MyBase.with(adjuster)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function plus(ByVal amount As java.time.temporal.TemporalAmount) As ThaiBuddhistDate
			Return MyBase.plus(amount)
		End Function

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="DateTimeException"> {@inheritDoc} </exception>
		''' <exception cref="ArithmeticException"> {@inheritDoc} </exception>
		Public Overrides Function minus(ByVal amount As java.time.temporal.TemporalAmount) As ThaiBuddhistDate
			Return MyBase.minus(amount)
		End Function

		'-----------------------------------------------------------------------
		Friend Overrides Function plusYears(ByVal years As Long) As ThaiBuddhistDate
			Return [with](isoDate.plusYears(years))
		End Function

		Friend Overrides Function plusMonths(ByVal months As Long) As ThaiBuddhistDate
			Return [with](isoDate.plusMonths(months))
		End Function

		Friend Overrides Function plusWeeks(ByVal weeksToAdd As Long) As ThaiBuddhistDate
			Return MyBase.plusWeeks(weeksToAdd)
		End Function

		Friend Overrides Function plusDays(ByVal days As Long) As ThaiBuddhistDate
			Return [with](isoDate.plusDays(days))
		End Function

		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As ThaiBuddhistDate
			Return MyBase.plus(amountToAdd, unit)
		End Function

		Public Overrides Function minus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As ThaiBuddhistDate
			Return MyBase.minus(amountToAdd, unit)
		End Function

		Friend Overrides Function minusYears(ByVal yearsToSubtract As Long) As ThaiBuddhistDate
			Return MyBase.minusYears(yearsToSubtract)
		End Function

		Friend Overrides Function minusMonths(ByVal monthsToSubtract As Long) As ThaiBuddhistDate
			Return MyBase.minusMonths(monthsToSubtract)
		End Function

		Friend Overrides Function minusWeeks(ByVal weeksToSubtract As Long) As ThaiBuddhistDate
			Return MyBase.minusWeeks(weeksToSubtract)
		End Function

		Friend Overrides Function minusDays(ByVal daysToSubtract As Long) As ThaiBuddhistDate
			Return MyBase.minusDays(daysToSubtract)
		End Function

		Private Function [with](ByVal newDate As java.time.LocalDate) As ThaiBuddhistDate
			[Return] (If(newDate.Equals(isoDate), Me, New ThaiBuddhistDate(newDate)))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function atTime(ByVal localTime_Renamed As java.time.LocalTime) As ChronoLocalDateTime(Of ThaiBuddhistDate) Implements ChronoLocalDate.atTime ' for javadoc and covariant return type
			Return CType(MyBase.atTime(localTime_Renamed), ChronoLocalDateTime(Of ThaiBuddhistDate))
		End Function

		Public Overrides Function [until](ByVal endDate As ChronoLocalDate) As ChronoPeriod Implements ChronoLocalDate.until
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
		''' Compares this {@code ThaiBuddhistDate} with another ensuring that the date is the same.
		''' <p>
		''' Only objects of type {@code ThaiBuddhistDate} are compared, other types return false.
		''' To compare the dates of two {@code TemporalAccessor} instances, including dates
		''' in two different chronologies, use <seealso cref="ChronoField#EPOCH_DAY"/> as a comparator.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean ' override for performance
			If Me Is obj Then Return True
			If TypeOf obj Is ThaiBuddhistDate Then
				Dim otherDate As ThaiBuddhistDate = CType(obj, ThaiBuddhistDate)
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
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(10);                // identifies a ThaiBuddhistDate
		'''  out.writeInt(get(YEAR));
		'''  out.writeByte(get(MONTH_OF_YEAR));
		'''  out.writeByte(get(DAY_OF_MONTH));
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.THAIBUDDHIST_DATE_TYPE, Me)
		End Function

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			' ThaiBuddhistChronology is implicit in the THAIBUDDHIST_DATE_TYPE
			out.writeInt(Me.get(YEAR))
			out.writeByte(Me.get(MONTH_OF_YEAR))
			out.writeByte(Me.get(DAY_OF_MONTH))
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ThaiBuddhistDate
			Dim year_Renamed As Integer = [in].readInt()
			Dim month As Integer = [in].readByte()
			Dim dayOfMonth As Integer = [in].readByte()
			Return ThaiBuddhistChronology.INSTANCE.date(year_Renamed, month, dayOfMonth)
		End Function

	End Class

End Namespace