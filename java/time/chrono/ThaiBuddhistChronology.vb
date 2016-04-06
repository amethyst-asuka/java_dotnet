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
	''' The Thai Buddhist calendar system.
	''' <p>
	''' This chronology defines the rules of the Thai Buddhist calendar system.
	''' This calendar system is primarily used in Thailand.
	''' Dates are aligned such that {@code 2484-01-01 (Buddhist)} is {@code 1941-01-01 (ISO)}.
	''' <p>
	''' The fields are defined as follows:
	''' <ul>
	''' <li>era - There are two eras, the current 'Buddhist' (ERA_BE) and the previous era (ERA_BEFORE_BE).
	''' <li>year-of-era - The year-of-era for the current era increases uniformly from the epoch at year one.
	'''  For the previous era the year increases from one as time goes backwards.
	'''  The value for the current era is equal to the ISO proleptic-year plus 543.
	''' <li>proleptic-year - The proleptic year is the same as the year-of-era for the
	'''  current era. For the previous era, years have zero, then negative values.
	'''  The value is equal to the ISO proleptic-year plus 543.
	''' <li>month-of-year - The ThaiBuddhist month-of-year exactly matches ISO.
	''' <li>day-of-month - The ThaiBuddhist day-of-month exactly matches ISO.
	''' <li>day-of-year - The ThaiBuddhist day-of-year exactly matches ISO.
	''' <li>leap-year - The ThaiBuddhist leap-year pattern exactly matches ISO, such that the two calendars
	'''  are never out of step.
	''' </ul>
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ThaiBuddhistChronology
		Inherits AbstractChronology

		''' <summary>
		''' Singleton instance of the Buddhist chronology.
		''' </summary>
		Public Shared ReadOnly INSTANCE As New ThaiBuddhistChronology

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 2775954514031616474L
		''' <summary>
		''' Containing the offset to add to the ISO year.
		''' </summary>
		Friend Const YEARS_DIFFERENCE As Integer = 543
		''' <summary>
		''' Narrow names for eras.
		''' </summary>
		Private Shared ReadOnly ERA_NARROW_NAMES As New Dictionary(Of String, String())
		''' <summary>
		''' Short names for eras.
		''' </summary>
		Private Shared ReadOnly ERA_SHORT_NAMES As New Dictionary(Of String, String())
		''' <summary>
		''' Full names for eras.
		''' </summary>
		Private Shared ReadOnly ERA_FULL_NAMES As New Dictionary(Of String, String())
		''' <summary>
		''' Fallback language for the era names.
		''' </summary>
		Private Const FALLBACK_LANGUAGE As String = "en"
		''' <summary>
		''' Language that has the era names.
		''' </summary>
		Private Const TARGET_LANGUAGE As String = "th"
		''' <summary>
		''' Name data.
		''' </summary>
		Shared Sub New()
			ERA_NARROW_NAMES(FALLBACK_LANGUAGE) = New String(){"BB", "BE"}
			ERA_NARROW_NAMES(TARGET_LANGUAGE) = New String(){"BB", "BE"}
			ERA_SHORT_NAMES(FALLBACK_LANGUAGE) = New String(){"B.B.", "B.E."}
			ERA_SHORT_NAMES(TARGET_LANGUAGE) = New String(){ChrW(&H0e1e).ToString() & "." & ChrW(&H0e28).ToString() & ".", ChrW(&H0e1b).ToString() & ChrW(&H0e35).ToString() & ChrW(&H0e01).ToString() & ChrW(&H0e48).ToString() & ChrW(&H0e2d).ToString() & ChrW(&H0e19).ToString() & ChrW(&H0e04).ToString() & ChrW(&H0e23).ToString() & ChrW(&H0e34).ToString() & ChrW(&H0e2a).ToString() & ChrW(&H0e15).ToString() & ChrW(&H0e4c).ToString() & ChrW(&H0e01).ToString() & ChrW(&H0e32).ToString() & ChrW(&H0e25).ToString() & ChrW(&H0e17).ToString() & ChrW(&H0e35).ToString() & ChrW(&H0e48).ToString()}
			ERA_FULL_NAMES(FALLBACK_LANGUAGE) = New String(){"Before Buddhist", "Budhhist Era"}
			ERA_FULL_NAMES(TARGET_LANGUAGE) = New String(){ChrW(&H0e1e).ToString() & ChrW(&H0e38).ToString() & ChrW(&H0e17).ToString() & ChrW(&H0e18).ToString() & ChrW(&H0e28).ToString() & ChrW(&H0e31).ToString() & ChrW(&H0e01).ToString() & ChrW(&H0e23).ToString() & ChrW(&H0e32).ToString() & ChrW(&H0e0a).ToString(), ChrW(&H0e1b).ToString() & ChrW(&H0e35).ToString() & ChrW(&H0e01).ToString() & ChrW(&H0e48).ToString() & ChrW(&H0e2d).ToString() & ChrW(&H0e19).ToString() & ChrW(&H0e04).ToString() & ChrW(&H0e23).ToString() & ChrW(&H0e34).ToString() & ChrW(&H0e2a).ToString() & ChrW(&H0e15).ToString() & ChrW(&H0e4c).ToString() & ChrW(&H0e01).ToString() & ChrW(&H0e32).ToString() & ChrW(&H0e25).ToString() & ChrW(&H0e17).ToString() & ChrW(&H0e35).ToString() & ChrW(&H0e48).ToString()}
		End Sub

		''' <summary>
		''' Restricted constructor.
		''' </summary>
		Private Sub New()
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the ID of the chronology - 'ThaiBuddhist'.
		''' <p>
		''' The ID uniquely identifies the {@code Chronology}.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <returns> the chronology ID - 'ThaiBuddhist' </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		Public  Overrides ReadOnly Property  id As String
			Get
				Return "ThaiBuddhist"
			End Get
		End Property

		''' <summary>
		''' Gets the calendar type of the underlying calendar system - 'buddhist'.
		''' <p>
		''' The calendar type is an identifier defined by the
		''' <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' It can also be used as part of a locale, accessible via
		''' <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		''' </summary>
		''' <returns> the calendar system type - 'buddhist' </returns>
		''' <seealso cref= #getId() </seealso>
		Public  Overrides ReadOnly Property  calendarType As String
			Get
				Return "buddhist"
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a local date in Thai Buddhist calendar system from the
		''' era, year-of-era, month-of-year and day-of-month fields.
		''' </summary>
		''' <param name="era">  the Thai Buddhist era, not null </param>
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Thai Buddhist local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code ThaiBuddhistEra} </exception>
		Public Overrides Function [date](  era As Era,   yearOfEra As Integer,   month As Integer,   dayOfMonth As Integer) As ThaiBuddhistDate
			Return [date](prolepticYear(era, yearOfEra), month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains a local date in Thai Buddhist calendar system from the
		''' proleptic-year, month-of-year and day-of-month fields.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Thai Buddhist local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function [date](  prolepticYear As Integer,   month As Integer,   dayOfMonth As Integer) As ThaiBuddhistDate
			Return New ThaiBuddhistDate(java.time.LocalDate.of(prolepticYear - YEARS_DIFFERENCE, month, dayOfMonth))
		End Function

		''' <summary>
		''' Obtains a local date in Thai Buddhist calendar system from the
		''' era, year-of-era and day-of-year fields.
		''' </summary>
		''' <param name="era">  the Thai Buddhist era, not null </param>
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Thai Buddhist local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code ThaiBuddhistEra} </exception>
		Public Overrides Function dateYearDay(  era As Era,   yearOfEra As Integer,   dayOfYear As Integer) As ThaiBuddhistDate
			Return dateYearDay(prolepticYear(era, yearOfEra), dayOfYear)
		End Function

		''' <summary>
		''' Obtains a local date in Thai Buddhist calendar system from the
		''' proleptic-year and day-of-year fields.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Thai Buddhist local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateYearDay(  prolepticYear As Integer,   dayOfYear As Integer) As ThaiBuddhistDate
			Return New ThaiBuddhistDate(java.time.LocalDate.ofYearDay(prolepticYear - YEARS_DIFFERENCE, dayOfYear))
		End Function

		''' <summary>
		''' Obtains a local date in the Thai Buddhist calendar system from the epoch-day.
		''' </summary>
		''' <param name="epochDay">  the epoch day </param>
		''' <returns> the Thai Buddhist local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateEpochDay(  epochDay As Long) As ThaiBuddhistDate ' override with covariant return type
			Return New ThaiBuddhistDate(java.time.LocalDate.ofEpochDay(epochDay))
		End Function

		Public Overrides Function dateNow() As ThaiBuddhistDate
			Return dateNow(java.time.Clock.systemDefaultZone())
		End Function

		Public Overrides Function dateNow(  zone As java.time.ZoneId) As ThaiBuddhistDate
			Return dateNow(java.time.Clock.system(zone))
		End Function

		Public Overrides Function dateNow(  clock_Renamed As java.time.Clock) As ThaiBuddhistDate
			Return [date](java.time.LocalDate.now(clock_Renamed))
		End Function

		Public Overrides Function [date](  temporal As java.time.temporal.TemporalAccessor) As ThaiBuddhistDate
			If TypeOf temporal Is ThaiBuddhistDate Then Return CType(temporal, ThaiBuddhistDate)
			Return New ThaiBuddhistDate(java.time.LocalDate.from(temporal))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function localDateTime(  temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDateTime(Of ThaiBuddhistDate)
			Return CType(MyBase.localDateTime(temporal), ChronoLocalDateTime(Of ThaiBuddhistDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(  temporal As java.time.temporal.TemporalAccessor) As ChronoZonedDateTime(Of ThaiBuddhistDate)
			Return CType(MyBase.zonedDateTime(temporal), ChronoZonedDateTime(Of ThaiBuddhistDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(  instant_Renamed As java.time.Instant,   zone As java.time.ZoneId) As ChronoZonedDateTime(Of ThaiBuddhistDate)
			Return CType(MyBase.zonedDateTime(instant_Renamed, zone), ChronoZonedDateTime(Of ThaiBuddhistDate))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified year is a leap year.
		''' <p>
		''' Thai Buddhist leap years occur exactly in line with ISO leap years.
		''' This method does not validate the year passed in, and only has a
		''' well-defined result for years in the supported range.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		''' <returns> true if the year is a leap year </returns>
		Public Overrides Function isLeapYear(  prolepticYear As Long) As Boolean
			Return IsoChronology.INSTANCE.isLeapYear(prolepticYear - YEARS_DIFFERENCE)
		End Function

		Public Overrides Function prolepticYear(  era As Era,   yearOfEra As Integer) As Integer
			If TypeOf era Is ThaiBuddhistEra = False Then Throw New ClassCastException("Era must be BuddhistEra")
			[Return] (If(era = ThaiBuddhistEra.BE, yearOfEra, 1 - yearOfEra))
		End Function

		Public Overrides Function eraOf(  eraValue As Integer) As ThaiBuddhistEra
			Return ThaiBuddhistEra.of(eraValue)
		End Function

		Public Overrides Function eras() As IList(Of Era)
			Return java.util.Arrays.asList(Of Era)(ThaiBuddhistEra.values())
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function range(  field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange
			Select Case field
				Case PROLEPTIC_MONTH
					Dim range_Renamed As java.time.temporal.ValueRange = PROLEPTIC_MONTH.range()
					Return java.time.temporal.ValueRange.of(range_Renamed.minimum + YEARS_DIFFERENCE * 12L, range_Renamed.maximum + YEARS_DIFFERENCE * 12L)
				Case YEAR_OF_ERA
					Dim range_Renamed As java.time.temporal.ValueRange = YEAR.range()
					Return java.time.temporal.ValueRange.of(1, -(range_Renamed.minimum + YEARS_DIFFERENCE) + 1, range_Renamed.maximum + YEARS_DIFFERENCE)
				Case YEAR
					Dim range_Renamed As java.time.temporal.ValueRange = YEAR.range()
					Return java.time.temporal.ValueRange.of(range_Renamed.minimum + YEARS_DIFFERENCE, range_Renamed.maximum + YEARS_DIFFERENCE)
			End Select
			Return field.range()
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function resolveDate(  fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?),   resolverStyle As java.time.format.ResolverStyle) As ThaiBuddhistDate ' override for return type
			Return CType(MyBase.resolveDate(fieldValues, resolverStyle), ThaiBuddhistDate)
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