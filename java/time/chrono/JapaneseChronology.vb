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
	''' The Japanese Imperial calendar system.
	''' <p>
	''' This chronology defines the rules of the Japanese Imperial calendar system.
	''' This calendar system is primarily used in Japan.
	''' The Japanese Imperial calendar system is the same as the ISO calendar system
	''' apart from the era-based year numbering.
	''' <p>
	''' Japan introduced the Gregorian calendar starting with Meiji 6.
	''' Only Meiji and later eras are supported;
	''' dates before Meiji 6, January 1 are not supported.
	''' <p>
	''' The supported {@code ChronoField} instances are:
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
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class JapaneseChronology
		Inherits AbstractChronology

		Friend Shared ReadOnly JCAL As sun.util.calendar.LocalGregorianCalendar = CType(sun.util.calendar.CalendarSystem.forName("japanese"), sun.util.calendar.LocalGregorianCalendar)

		' Locale for creating a JapaneseImpericalCalendar.
		Friend Shared ReadOnly LOCALE As java.util.Locale = java.util.Locale.forLanguageTag("ja-JP-u-ca-japanese")

		''' <summary>
		''' Singleton instance for Japanese chronology.
		''' </summary>
		Public Shared ReadOnly INSTANCE As New JapaneseChronology

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 459996390165777884L

		'-----------------------------------------------------------------------
		''' <summary>
		''' Restricted constructor.
		''' </summary>
		Private Sub New()
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the ID of the chronology - 'Japanese'.
		''' <p>
		''' The ID uniquely identifies the {@code Chronology}.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <returns> the chronology ID - 'Japanese' </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		Public Property Overrides id As String
			Get
				Return "Japanese"
			End Get
		End Property

		''' <summary>
		''' Gets the calendar type of the underlying calendar system - 'japanese'.
		''' <p>
		''' The calendar type is an identifier defined by the
		''' <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' It can also be used as part of a locale, accessible via
		''' <seealso cref="Locale#getUnicodeLocaleType(String)"/> with the key 'ca'.
		''' </summary>
		''' <returns> the calendar system type - 'japanese' </returns>
		''' <seealso cref= #getId() </seealso>
		Public Property Overrides calendarType As String
			Get
				Return "japanese"
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a local date in Japanese calendar system from the
		''' era, year-of-era, month-of-year and day-of-month fields.
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
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Japanese local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code JapaneseEra} </exception>
		Public Overrides Function [date](ByVal era As Era, ByVal yearOfEra As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As JapaneseDate
			If TypeOf era Is JapaneseEra = False Then Throw New ClassCastException("Era must be JapaneseEra")
			Return JapaneseDate.of(CType(era, JapaneseEra), yearOfEra, month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains a local date in Japanese calendar system from the
		''' proleptic-year, month-of-year and day-of-month fields.
		''' <p>
		''' The Japanese proleptic year, month and day-of-month are the same as those
		''' in the ISO calendar system. They are not reset when the era changes.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Japanese local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function [date](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As JapaneseDate
			Return New JapaneseDate(java.time.LocalDate.of(prolepticYear, month, dayOfMonth))
		End Function

		''' <summary>
		''' Obtains a local date in Japanese calendar system from the
		''' era, year-of-era and day-of-year fields.
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
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Japanese local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code JapaneseEra} </exception>
		Public Overrides Function dateYearDay(ByVal era As Era, ByVal yearOfEra As Integer, ByVal dayOfYear As Integer) As JapaneseDate
			Return JapaneseDate.ofYearDay(CType(era, JapaneseEra), yearOfEra, dayOfYear)
		End Function

		''' <summary>
		''' Obtains a local date in Japanese calendar system from the
		''' proleptic-year and day-of-year fields.
		''' <p>
		''' The day-of-year in this factory is expressed relative to the start of the proleptic year.
		''' The Japanese proleptic year and day-of-year are the same as those in the ISO calendar system.
		''' They are not reset when the era changes.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Japanese local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateYearDay(ByVal prolepticYear As Integer, ByVal dayOfYear As Integer) As JapaneseDate
			Return New JapaneseDate(java.time.LocalDate.ofYearDay(prolepticYear, dayOfYear))
		End Function

		''' <summary>
		''' Obtains a local date in the Japanese calendar system from the epoch-day.
		''' </summary>
		''' <param name="epochDay">  the epoch day </param>
		''' <returns> the Japanese local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateEpochDay(ByVal epochDay As Long) As JapaneseDate ' override with covariant return type
			Return New JapaneseDate(java.time.LocalDate.ofEpochDay(epochDay))
		End Function

		Public Overrides Function dateNow() As JapaneseDate
			Return dateNow(java.time.Clock.systemDefaultZone())
		End Function

		Public Overrides Function dateNow(ByVal zone As java.time.ZoneId) As JapaneseDate
			Return dateNow(java.time.Clock.system(zone))
		End Function

		Public Overrides Function dateNow(ByVal clock_Renamed As java.time.Clock) As JapaneseDate
			Return [date](java.time.LocalDate.now(clock_Renamed))
		End Function

		Public Overrides Function [date](ByVal temporal As java.time.temporal.TemporalAccessor) As JapaneseDate
			If TypeOf temporal Is JapaneseDate Then Return CType(temporal, JapaneseDate)
			Return New JapaneseDate(java.time.LocalDate.from(temporal))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function localDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDateTime(Of JapaneseDate)
			Return CType(MyBase.localDateTime(temporal), ChronoLocalDateTime(Of JapaneseDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoZonedDateTime(Of JapaneseDate)
			Return CType(MyBase.zonedDateTime(temporal), ChronoZonedDateTime(Of JapaneseDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(ByVal instant_Renamed As java.time.Instant, ByVal zone As java.time.ZoneId) As ChronoZonedDateTime(Of JapaneseDate)
			Return CType(MyBase.zonedDateTime(instant_Renamed, zone), ChronoZonedDateTime(Of JapaneseDate))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified year is a leap year.
		''' <p>
		''' Japanese calendar leap years occur exactly in line with ISO leap years.
		''' This method does not validate the year passed in, and only has a
		''' well-defined result for years in the supported range.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		''' <returns> true if the year is a leap year </returns>
		Public Overrides Function isLeapYear(ByVal prolepticYear As Long) As Boolean
			Return IsoChronology.INSTANCE.isLeapYear(prolepticYear)
		End Function

		Public Overrides Function prolepticYear(ByVal era As Era, ByVal yearOfEra As Integer) As Integer
			If TypeOf era Is JapaneseEra = False Then Throw New ClassCastException("Era must be JapaneseEra")

			Dim jera As JapaneseEra = CType(era, JapaneseEra)
			Dim gregorianYear As Integer = jera.privateEra.sinceDate.year + yearOfEra - 1
			If yearOfEra = 1 Then Return gregorianYear
			If gregorianYear >= java.time.Year.MIN_VALUE AndAlso gregorianYear <= java.time.Year.MAX_VALUE Then
				Dim jdate As sun.util.calendar.LocalGregorianCalendar.Date = JCAL.newCalendarDate(Nothing)
				jdate.eraEra(jera.privateEra).setDate(yearOfEra, 1, 1)
				If JapaneseChronology.JCAL.validate(jdate) Then Return gregorianYear
			End If
			Throw New java.time.DateTimeException("Invalid yearOfEra value")
		End Function

		''' <summary>
		''' Returns the calendar system era object from the given numeric value.
		''' 
		''' See the description of each Era for the numeric values of:
		''' <seealso cref="JapaneseEra#HEISEI"/>, <seealso cref="JapaneseEra#SHOWA"/>,<seealso cref="JapaneseEra#TAISHO"/>,
		''' <seealso cref="JapaneseEra#MEIJI"/>), only Meiji and later eras are supported.
		''' </summary>
		''' <param name="eraValue">  the era value </param>
		''' <returns> the Japanese {@code Era} for the given numeric era value </returns>
		''' <exception cref="DateTimeException"> if {@code eraValue} is invalid </exception>
		Public Overrides Function eraOf(ByVal eraValue As Integer) As JapaneseEra
			Return JapaneseEra.of(eraValue)
		End Function

		Public Overrides Function eras() As IList(Of Era)
			Return java.util.Arrays.asList(Of Era)(JapaneseEra.values())
		End Function

		Friend Property currentEra As JapaneseEra
			Get
				' Assume that the last JapaneseEra is the current one.
				Dim eras As JapaneseEra() = JapaneseEra.values()
				Return eras(eras.Length - 1)
			End Get
		End Property

		'-----------------------------------------------------------------------
		Public Overrides Function range(ByVal field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange
			Select Case field
				Case ALIGNED_DAY_OF_WEEK_IN_MONTH, ALIGNED_DAY_OF_WEEK_IN_YEAR, ALIGNED_WEEK_OF_MONTH, ALIGNED_WEEK_OF_YEAR
					Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
				Case YEAR_OF_ERA
					Dim jcal_Renamed As DateTime? = DateTime.getInstance(LOCALE)
					Dim startYear As Integer = currentEra.privateEra.sinceDate.year
					Return java.time.temporal.ValueRange.of(1, jcal_Renamed.Value.getGreatestMinimum(DateTime.YEAR), jcal_Renamed.Value.getLeastMaximum(DateTime.YEAR) + 1, java.time.Year.MAX_VALUE - startYear) ' +1 due to the different definitions
				Case DAY_OF_YEAR
					Dim jcal_Renamed As DateTime? = DateTime.getInstance(LOCALE)
					Dim fieldIndex As Integer = DateTime.DAY_OF_YEAR
					Return java.time.temporal.ValueRange.of(jcal_Renamed.Value.getMinimum(fieldIndex), jcal_Renamed.Value.getGreatestMinimum(fieldIndex), jcal_Renamed.Value.getLeastMaximum(fieldIndex), jcal_Renamed.Value.getMaximum(fieldIndex))
				Case YEAR
					Return java.time.temporal.ValueRange.of(JapaneseDate.MEIJI_6_ISODATE.year, java.time.Year.MAX_VALUE)
				Case ERA
					Return java.time.temporal.ValueRange.of(JapaneseEra.MEIJI.value, currentEra.value)
				Case Else
					Return field.range()
			End Select
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function resolveDate(j ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As JapaneseDate ' override for return type
			Return CType(MyBase.resolveDate(fieldValues, resolverStyle), JapaneseDate)
		End Function

		Friend Overrides Function resolveYearOfEra(ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As ChronoLocalDate ' override for special Japanese behavior
			' validate era and year-of-era
			Dim eraLong As Long? = fieldValues(ERA)
			Dim era As JapaneseEra = Nothing
			If eraLong IsNot Nothing Then era = eraOf(range(ERA).checkValidIntValue(eraLong, ERA)) ' always validated
			Dim yoeLong As Long? = fieldValues(YEAR_OF_ERA)
			Dim yoe As Integer = 0
			If yoeLong IsNot Nothing Then yoe = range(YEAR_OF_ERA).checkValidIntValue(yoeLong, YEAR_OF_ERA) ' always validated
			' if only year-of-era and no year then invent era unless strict
			If era Is Nothing AndAlso yoeLong IsNot Nothing AndAlso fieldValues.ContainsKey(YEAR) = False AndAlso resolverStyle <> java.time.format.ResolverStyle.STRICT Then era = JapaneseEra.values()(JapaneseEra.values().Length - 1)
			' if both present, then try to create date
			If yoeLong IsNot Nothing AndAlso era IsNot Nothing Then
				If fieldValues.ContainsKey(MONTH_OF_YEAR) Then
					If fieldValues.ContainsKey(DAY_OF_MONTH) Then Return resolveYMD(era, yoe, fieldValues, resolverStyle)
				End If
				If fieldValues.ContainsKey(DAY_OF_YEAR) Then Return resolveYD(era, yoe, fieldValues, resolverStyle)
			End If
			Return Nothing
		End Function

		Private Function prolepticYearLenient(ByVal era As JapaneseEra, ByVal yearOfEra As Integer) As Integer
			Return era.privateEra.sinceDate.year + yearOfEra - 1
		End Function

		 Private Function resolveYMD(ByVal era As JapaneseEra, ByVal yoe As Integer, ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As ChronoLocalDate
			 fieldValues.Remove(ERA)
			 fieldValues.Remove(YEAR_OF_ERA)
			 If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				 Dim y As Integer = prolepticYearLenient(era, yoe)
				 Dim months As Long = Math.subtractExact(fieldValues.Remove(MONTH_OF_YEAR), 1)
				 Dim days As Long = Math.subtractExact(fieldValues.Remove(DAY_OF_MONTH), 1)
				 Return [date](y, 1, 1).plus(months, MONTHS).plus(days, DAYS)
			 End If
			 Dim moy As Integer = range(MONTH_OF_YEAR).checkValidIntValue(fieldValues.Remove(MONTH_OF_YEAR), MONTH_OF_YEAR)
			 Dim dom As Integer = range(DAY_OF_MONTH).checkValidIntValue(fieldValues.Remove(DAY_OF_MONTH), DAY_OF_MONTH)
			 If resolverStyle = java.time.format.ResolverStyle.SMART Then ' previous valid
				 If yoe < 1 Then Throw New java.time.DateTimeException("Invalid YearOfEra: " & yoe)
				 Dim y As Integer = prolepticYearLenient(era, yoe)
				 Dim result As JapaneseDate
				 Try
					 result = [date](y, moy, dom)
				 Catch ex As java.time.DateTimeException
					 result = [date](y, moy, 1).with(java.time.temporal.TemporalAdjusters.lastDayOfMonth())
				 End Try
				 ' handle the era being changed
				 ' only allow if the new date is in the same Jan-Dec as the era change
				 ' determine by ensuring either original yoe or result yoe is 1
				 If result.era IsNot era AndAlso result.get(YEAR_OF_ERA) > 1 AndAlso yoe > 1 Then Throw New java.time.DateTimeException("Invalid YearOfEra for Era: " & era & " " & yoe)
				 Return result
			 End If
			 Return [date](era, yoe, moy, dom)
		 End Function

		Private Function resolveYD(ByVal era As JapaneseEra, ByVal yoe As Integer, j ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As ChronoLocalDate
			fieldValues.Remove(ERA)
			fieldValues.Remove(YEAR_OF_ERA)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim y As Integer = prolepticYearLenient(era, yoe)
				Dim days As Long = Math.subtractExact(fieldValues.Remove(DAY_OF_YEAR), 1)
				Return dateYearDay(y, 1).plus(days, DAYS)
			End If
			Dim doy As Integer = range(DAY_OF_YEAR).checkValidIntValue(fieldValues.Remove(DAY_OF_YEAR), DAY_OF_YEAR)
			Return dateYearDay(era, yoe, doy) ' smart is same as strict
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
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub
	End Class

End Namespace