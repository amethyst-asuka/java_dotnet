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
	''' The Hijrah calendar is a lunar calendar supporting Islamic calendars.
	''' <p>
	''' The HijrahChronology follows the rules of the Hijrah calendar system. The Hijrah
	''' calendar has several variants based on differences in when the new moon is
	''' determined to have occurred and where the observation is made.
	''' In some variants the length of each month is
	''' computed algorithmically from the astronomical data for the moon and earth and
	''' in others the length of the month is determined by an authorized sighting
	''' of the new moon. For the algorithmically based calendars the calendar
	''' can project into the future.
	''' For sighting based calendars only historical data from past
	''' sightings is available.
	''' <p>
	''' The length of each month is 29 or 30 days.
	''' Ordinary years have 354 days; leap years have 355 days.
	''' 
	''' <p>
	''' CLDR and LDML identify variants:
	''' <table cellpadding="2" summary="Variants of Hijrah Calendars">
	''' <thead>
	''' <tr class="tableSubHeadingColor">
	''' <th class="colFirst" align="left" >Chronology ID</th>
	''' <th class="colFirst" align="left" >Calendar Type</th>
	''' <th class="colFirst" align="left" >Locale extension, see <seealso cref="java.util.Locale"/></th>
	''' <th class="colLast" align="left" >Description</th>
	''' </tr>
	''' </thead>
	''' <tbody>
	''' <tr class="altColor">
	''' <td>Hijrah-umalqura</td>
	''' <td>islamic-umalqura</td>
	''' <td>ca-islamic-umalqura</td>
	''' <td>Islamic - Umm Al-Qura calendar of Saudi Arabia</td>
	''' </tr>
	''' </tbody>
	''' </table>
	''' <p>Additional variants may be available through <seealso cref="Chronology#getAvailableChronologies()"/>.
	''' 
	''' <p>Example</p>
	''' <p>
	''' Selecting the chronology from the locale uses <seealso cref="Chronology#ofLocale"/>
	''' to find the Chronology based on Locale supported BCP 47 extension mechanism
	''' to request a specific calendar ("ca"). For example,
	''' </p>
	''' <pre>
	'''      Locale locale = Locale.forLanguageTag("en-US-u-ca-islamic-umalqura");
	'''      Chronology chrono = Chronology.ofLocale(locale);
	''' </pre>
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @implNote
	''' Each Hijrah variant is configured individually. Each variant is defined by a
	''' property resource that defines the {@code ID}, the {@code calendar type},
	''' the start of the calendar, the alignment with the
	''' ISO calendar, and the length of each month for a range of years.
	''' The variants are identified in the {@code calendars.properties} file.
	''' The new properties are prefixed with {@code "calendars.hijrah."}:
	''' <table cellpadding="2" border="0" summary="Configuration of Hijrah Calendar Variants">
	''' <thead>
	''' <tr class="tableSubHeadingColor">
	''' <th class="colFirst" align="left">Property Name</th>
	''' <th class="colFirst" align="left">Property value</th>
	''' <th class="colLast" align="left">Description </th>
	''' </tr>
	''' </thead>
	''' <tbody>
	''' <tr class="altColor">
	''' <td>calendars.hijrah.{ID}</td>
	''' <td>The property resource defining the {@code {ID}} variant</td>
	''' <td>The property resource is located with the {@code calendars.properties} file</td>
	''' </tr>
	''' <tr class="rowColor">
	''' <td>calendars.hijrah.{ID}.type</td>
	''' <td>The calendar type</td>
	''' <td>LDML defines the calendar type names</td>
	''' </tr>
	''' </tbody>
	''' </table>
	''' <p>
	''' The Hijrah property resource is a set of properties that describe the calendar.
	''' The syntax is defined by {@code java.util.Properties#load(Reader)}.
	''' <table cellpadding="2" summary="Configuration of Hijrah Calendar">
	''' <thead>
	''' <tr class="tableSubHeadingColor">
	''' <th class="colFirst" align="left" > Property Name</th>
	''' <th class="colFirst" align="left" > Property value</th>
	''' <th class="colLast" align="left" > Description </th>
	''' </tr>
	''' </thead>
	''' <tbody>
	''' <tr class="altColor">
	''' <td>id</td>
	''' <td>Chronology Id, for example, "Hijrah-umalqura"</td>
	''' <td>The Id of the calendar in common usage</td>
	''' </tr>
	''' <tr class="rowColor">
	''' <td>type</td>
	''' <td>Calendar type, for example, "islamic-umalqura"</td>
	''' <td>LDML defines the calendar types</td>
	''' </tr>
	''' <tr class="altColor">
	''' <td>version</td>
	''' <td>Version, for example: "1.8.0_1"</td>
	''' <td>The version of the Hijrah variant data</td>
	''' </tr>
	''' <tr class="rowColor">
	''' <td>iso-start</td>
	''' <td>ISO start date, formatted as {@code yyyy-MM-dd}, for example: "1900-04-30"</td>
	''' <td>The ISO date of the first day of the minimum Hijrah year.</td>
	''' </tr>
	''' <tr class="altColor">
	''' <td>yyyy - a numeric 4 digit year, for example "1434"</td>
	''' <td>The value is a sequence of 12 month lengths,
	''' for example: "29 30 29 30 29 30 30 30 29 30 29 29"</td>
	''' <td>The lengths of the 12 months of the year separated by whitespace.
	''' A numeric year property must be present for every year without any gaps.
	''' The month lengths must be between 29-32 inclusive.
	''' </td>
	''' </tr>
	''' </tbody>
	''' </table>
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class HijrahChronology
		Inherits AbstractChronology

		''' <summary>
		''' The Hijrah Calendar id.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly typeId As String
		''' <summary>
		''' The Hijrah calendarType.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly calendarType As String
		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 3127340209035924785L
		''' <summary>
		''' Singleton instance of the Islamic Umm Al-Qura calendar of Saudi Arabia.
		''' Other Hijrah chronology variants may be available from
		''' <seealso cref="Chronology#getAvailableChronologies"/>.
		''' </summary>
		Public Shared ReadOnly INSTANCE As HijrahChronology
		''' <summary>
		''' Flag to indicate the initialization of configuration data is complete. </summary>
		''' <seealso cref= #checkCalendarInit() </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private initComplete As Boolean
		''' <summary>
		''' Array of epoch days indexed by Hijrah Epoch month.
		''' Computed by <seealso cref="#loadCalendarData"/>.
		''' </summary>
		<NonSerialized> _
		Private hijrahEpochMonthStartDays As Integer()
		''' <summary>
		''' The minimum epoch day of this Hijrah calendar.
		''' Computed by <seealso cref="#loadCalendarData"/>.
		''' </summary>
		<NonSerialized> _
		Private minEpochDay As Integer
		''' <summary>
		''' The maximum epoch day for which calendar data is available.
		''' Computed by <seealso cref="#loadCalendarData"/>.
		''' </summary>
		<NonSerialized> _
		Private maxEpochDay As Integer
		''' <summary>
		''' The minimum epoch month.
		''' Computed by <seealso cref="#loadCalendarData"/>.
		''' </summary>
		<NonSerialized> _
		Private hijrahStartEpochMonth As Integer
		''' <summary>
		''' The minimum length of a month.
		''' Computed by <seealso cref="#createEpochMonths"/>.
		''' </summary>
		<NonSerialized> _
		Private minMonthLength As Integer
		''' <summary>
		''' The maximum length of a month.
		''' Computed by <seealso cref="#createEpochMonths"/>.
		''' </summary>
		<NonSerialized> _
		Private maxMonthLength As Integer
		''' <summary>
		''' The minimum length of a year in days.
		''' Computed by <seealso cref="#createEpochMonths"/>.
		''' </summary>
		<NonSerialized> _
		Private minYearLength As Integer
		''' <summary>
		''' The maximum length of a year in days.
		''' Computed by <seealso cref="#createEpochMonths"/>.
		''' </summary>
		<NonSerialized> _
		Private maxYearLength As Integer
		''' <summary>
		''' A reference to the properties stored in
		''' ${java.home}/lib/calendars.properties
		''' </summary>
		<NonSerialized> _
		Private ReadOnly Shared calendarProperties As java.util.Properties

		''' <summary>
		''' Prefix of property names for Hijrah calendar variants.
		''' </summary>
		Private Const PROP_PREFIX As String = "calendar.hijrah."
		''' <summary>
		''' Suffix of property names containing the calendar type of a variant.
		''' </summary>
		Private Const PROP_TYPE_SUFFIX As String = ".type"

		''' <summary>
		''' Static initialization of the predefined calendars found in the
		''' lib/calendars.properties file.
		''' </summary>
		Shared Sub New()
			Try
				calendarProperties = sun.util.calendar.BaseCalendar.calendarProperties
			Catch ioe As java.io.IOException
				Throw New InternalError("Can't initialize lib/calendars.properties", ioe)
			End Try

			Try
				INSTANCE = New HijrahChronology("Hijrah-umalqura")
				' Register it by its aliases
				AbstractChronology.registerChrono(INSTANCE, "Hijrah")
				AbstractChronology.registerChrono(INSTANCE, "islamic")
			Catch ex As java.time.DateTimeException
				' Absence of Hijrah calendar is fatal to initializing this class.
				Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.time.chrono")
				logger.severe("Unable to initialize Hijrah calendar: Hijrah-umalqura", ex)
				Throw New RuntimeException("Unable to initialize Hijrah-umalqura calendar", ex.InnerException)
			End Try
			registerVariants()
		End Sub

		''' <summary>
		''' For each Hijrah variant listed, create the HijrahChronology and register it.
		''' Exceptions during initialization are logged but otherwise ignored.
		''' </summary>
		Private Shared Sub registerVariants()
			For Each name As String In calendarProperties.stringPropertyNames()
				If name.StartsWith(PROP_PREFIX) Then
					Dim id_Renamed As String = name.Substring(PROP_PREFIX.length())
					If id_Renamed.IndexOf("."c) >= 0 Then Continue For ' no name or not a simple name of a calendar
					If id_Renamed.Equals(INSTANCE.id) Then Continue For ' do not duplicate the default
					Try
						' Create and register the variant
						Dim chrono As New HijrahChronology(id_Renamed)
						AbstractChronology.registerChrono(chrono)
					Catch ex As java.time.DateTimeException
						' Log error and continue
						Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.time.chrono")
						logger.severe("Unable to initialize Hijrah calendar: " & id_Renamed, ex)
					End Try
				End If
			Next name
		End Sub

		''' <summary>
		''' Create a HijrahChronology for the named variant.
		''' The resource and calendar type are retrieved from properties
		''' in the {@code calendars.properties}.
		''' The property names are {@code "calendar.hijrah." + id}
		''' and  {@code "calendar.hijrah." + id + ".type"} </summary>
		''' <param name="id"> the id of the calendar </param>
		''' <exception cref="DateTimeException"> if the calendar type is missing from the properties file. </exception>
		''' <exception cref="IllegalArgumentException"> if the id is empty </exception>
		Private Sub New(ByVal id As String)
			If id.empty Then Throw New IllegalArgumentException("calendar id is empty")
			Dim propName As String = PROP_PREFIX + id + PROP_TYPE_SUFFIX
			Dim calType As String = calendarProperties.getProperty(propName)
			If calType Is Nothing OrElse calType.empty Then Throw New java.time.DateTimeException("calendarType is missing or empty for: " & propName)
			Me.typeId = id
			Me.calendarType = calType
		End Sub

		''' <summary>
		''' Check and ensure that the calendar data has been initialized.
		''' The initialization check is performed at the boundary between
		''' public and package methods.  If a public calls another public method
		''' a check is not necessary in the caller.
		''' The constructors of HijrahDate call <seealso cref="#getEpochDay"/> or
		''' <seealso cref="#getHijrahDateInfo"/> so every call from HijrahDate to a
		''' HijrahChronology via package private methods has been checked.
		''' </summary>
		''' <exception cref="DateTimeException"> if the calendar data configuration is
		'''     malformed or IOExceptions occur loading the data </exception>
		Private Sub checkCalendarInit()
			' Keep this short so it can be inlined for performance
			If initComplete = False Then
				loadCalendarData()
				initComplete = True
			End If
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the ID of the chronology.
		''' <p>
		''' The ID uniquely identifies the {@code Chronology}. It can be used to
		''' lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <returns> the chronology ID, non-null </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		Public Property Overrides id As String
			Get
				Return typeId
			End Get
		End Property

		''' <summary>
		''' Gets the calendar type of the Islamic calendar.
		''' <p>
		''' The calendar type is an identifier defined by the
		''' <em>Unicode Locale Data Markup Language (LDML)</em> specification.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <returns> the calendar system type; non-null if the calendar has
		'''    a standard type, otherwise null </returns>
		''' <seealso cref= #getId() </seealso>
		Public Property Overrides calendarType As String
			Get
				Return calendarType
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a local date in Hijrah calendar system from the
		''' era, year-of-era, month-of-year and day-of-month fields.
		''' </summary>
		''' <param name="era">  the Hijrah era, not null </param>
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Hijrah local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code HijrahEra} </exception>
		Public Overrides Function [date](ByVal era As Era, ByVal yearOfEra As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As HijrahDate
			Return [date](prolepticYear(era, yearOfEra), month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains a local date in Hijrah calendar system from the
		''' proleptic-year, month-of-year and day-of-month fields.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="month">  the month-of-year </param>
		''' <param name="dayOfMonth">  the day-of-month </param>
		''' <returns> the Hijrah local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function [date](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As HijrahDate
			Return HijrahDate.of(Me, prolepticYear, month, dayOfMonth)
		End Function

		''' <summary>
		''' Obtains a local date in Hijrah calendar system from the
		''' era, year-of-era and day-of-year fields.
		''' </summary>
		''' <param name="era">  the Hijrah era, not null </param>
		''' <param name="yearOfEra">  the year-of-era </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Hijrah local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not a {@code HijrahEra} </exception>
		Public Overrides Function dateYearDay(ByVal era As Era, ByVal yearOfEra As Integer, ByVal dayOfYear As Integer) As HijrahDate
			Return dateYearDay(prolepticYear(era, yearOfEra), dayOfYear)
		End Function

		''' <summary>
		''' Obtains a local date in Hijrah calendar system from the
		''' proleptic-year and day-of-year fields.
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year </param>
		''' <param name="dayOfYear">  the day-of-year </param>
		''' <returns> the Hijrah local date, not null </returns>
		''' <exception cref="DateTimeException"> if the value of the year is out of range,
		'''  or if the day-of-year is invalid for the year </exception>
		Public Overrides Function dateYearDay(ByVal prolepticYear As Integer, ByVal dayOfYear As Integer) As HijrahDate
			Dim date_Renamed As HijrahDate = HijrahDate.of(Me, prolepticYear, 1, 1)
			If dayOfYear > date_Renamed.lengthOfYear() Then Throw New java.time.DateTimeException("Invalid dayOfYear: " & dayOfYear)
			Return date_Renamed.plusDays(dayOfYear - 1)
		End Function

		''' <summary>
		''' Obtains a local date in the Hijrah calendar system from the epoch-day.
		''' </summary>
		''' <param name="epochDay">  the epoch day </param>
		''' <returns> the Hijrah local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Public Overrides Function dateEpochDay(ByVal epochDay As Long) As HijrahDate ' override with covariant return type
			Return HijrahDate.ofEpochDay(Me, epochDay)
		End Function

		Public Overrides Function dateNow() As HijrahDate
			Return dateNow(java.time.Clock.systemDefaultZone())
		End Function

		Public Overrides Function dateNow(ByVal zone As java.time.ZoneId) As HijrahDate
			Return dateNow(java.time.Clock.system(zone))
		End Function

		Public Overrides Function dateNow(ByVal clock_Renamed As java.time.Clock) As HijrahDate
			Return [date](java.time.LocalDate.now(clock_Renamed))
		End Function

		Public Overrides Function [date](ByVal temporal As java.time.temporal.TemporalAccessor) As HijrahDate
			If TypeOf temporal Is HijrahDate Then Return CType(temporal, HijrahDate)
			Return HijrahDate.ofEpochDay(Me, temporal.getLong(EPOCH_DAY))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function localDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDateTime(Of HijrahDate)
			Return CType(MyBase.localDateTime(temporal), ChronoLocalDateTime(Of HijrahDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoZonedDateTime(Of HijrahDate)
			Return CType(MyBase.zonedDateTime(temporal), ChronoZonedDateTime(Of HijrahDate))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function zonedDateTime(ByVal instant_Renamed As java.time.Instant, ByVal zone As java.time.ZoneId) As ChronoZonedDateTime(Of HijrahDate)
			Return CType(MyBase.zonedDateTime(instant_Renamed, zone), ChronoZonedDateTime(Of HijrahDate))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function isLeapYear(ByVal prolepticYear As Long) As Boolean
			checkCalendarInit()
			Dim epochMonth As Integer = yearToEpochMonth(CInt(prolepticYear))
			If epochMonth < 0 OrElse epochMonth > maxEpochDay Then Throw New java.time.DateTimeException("Hijrah date out of range")
			Dim len As Integer = getYearLength(CInt(prolepticYear))
			[Return] (len > 354)
		End Function

		Public Overrides Function prolepticYear(ByVal era As Era, ByVal yearOfEra As Integer) As Integer
			If TypeOf era Is HijrahEra = False Then Throw New ClassCastException("Era must be HijrahEra")
			Return yearOfEra
		End Function

		Public Overrides Function eraOf(ByVal eraValue As Integer) As HijrahEra
			Select Case eraValue
				Case 1
					Return HijrahEra.AH
				Case Else
					Throw New java.time.DateTimeException("invalid Hijrah era")
			End Select
		End Function

		Public Overrides Function eras() As IList(Of Era)
			Return java.util.Arrays.asList(Of Era)(HijrahEra.values())
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function range(ByVal field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange
			checkCalendarInit()
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = field
				Select Case f
					Case DAY_OF_MONTH
						Return java.time.temporal.ValueRange.of(1, 1, minimumMonthLength, maximumMonthLength)
					Case DAY_OF_YEAR
						Return java.time.temporal.ValueRange.of(1, maximumDayOfYear)
					Case ALIGNED_WEEK_OF_MONTH
						Return java.time.temporal.ValueRange.of(1, 5)
					Case YEAR, YEAR_OF_ERA
						Return java.time.temporal.ValueRange.of(minimumYear, maximumYear)
					Case ERA
						Return java.time.temporal.ValueRange.of(1, 1)
					Case Else
						Return field.range()
				End Select
			End If
			Return field.range()
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function resolveDate(ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As HijrahDate ' override for return type
			Return CType(MyBase.resolveDate(fieldValues, resolverStyle), HijrahDate)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Check the validity of a year.
		''' </summary>
		''' <param name="prolepticYear"> the year to check </param>
		Friend Function checkValidYear(ByVal prolepticYear As Long) As Integer
			If prolepticYear < minimumYear OrElse prolepticYear > maximumYear Then Throw New java.time.DateTimeException("Invalid Hijrah year: " & prolepticYear)
			Return CInt(prolepticYear)
		End Function

		Friend Sub checkValidDayOfYear(ByVal dayOfYear As Integer)
			If dayOfYear < 1 OrElse dayOfYear > maximumDayOfYear Then Throw New java.time.DateTimeException("Invalid Hijrah day of year: " & dayOfYear)
		End Sub

		Friend Sub checkValidMonth(ByVal month As Integer)
			If month < 1 OrElse month > 12 Then Throw New java.time.DateTimeException("Invalid Hijrah month: " & month)
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns an array containing the Hijrah year, month and day
		''' computed from the epoch day.
		''' </summary>
		''' <param name="epochDay">  the EpochDay </param>
		''' <returns> int[0] = YEAR, int[1] = MONTH, int[2] = DATE </returns>
		Friend Function getHijrahDateInfo(ByVal epochDay As Integer) As Integer()
			checkCalendarInit() ' ensure that the chronology is initialized
			If epochDay < minEpochDay OrElse epochDay >= maxEpochDay Then Throw New java.time.DateTimeException("Hijrah date out of range")

			Dim epochMonth As Integer = epochDayToEpochMonth(epochDay)
			Dim year_Renamed As Integer = epochMonthToYear(epochMonth)
			Dim month As Integer = epochMonthToMonth(epochMonth)
			Dim day1 As Integer = epochMonthToEpochDay(epochMonth)
			Dim date_Renamed As Integer = epochDay - day1 ' epochDay - dayOfEpoch(year, month);

			Dim dateInfo As Integer() = New Integer(2){}
			dateInfo(0) = year_Renamed
			dateInfo(1) = month + 1 ' change to 1-based.
			dateInfo(2) = date_Renamed + 1 ' change to 1-based.
			Return dateInfo
		End Function

		''' <summary>
		''' Return the epoch day computed from Hijrah year, month, and day.
		''' </summary>
		''' <param name="prolepticYear"> the year to represent, 0-origin </param>
		''' <param name="monthOfYear"> the month-of-year to represent, 1-origin </param>
		''' <param name="dayOfMonth"> the day-of-month to represent, 1-origin </param>
		''' <returns> the epoch day </returns>
		Friend Function getEpochDay(ByVal prolepticYear As Integer, ByVal monthOfYear As Integer, ByVal dayOfMonth As Integer) As Long
			checkCalendarInit() ' ensure that the chronology is initialized
			checkValidMonth(monthOfYear)
			Dim epochMonth As Integer = yearToEpochMonth(prolepticYear) + (monthOfYear - 1)
			If epochMonth < 0 OrElse epochMonth >= hijrahEpochMonthStartDays.Length Then Throw New java.time.DateTimeException("Invalid Hijrah date, year: " & prolepticYear & ", month: " & monthOfYear)
			If dayOfMonth < 1 OrElse dayOfMonth > getMonthLength(prolepticYear, monthOfYear) Then Throw New java.time.DateTimeException("Invalid Hijrah day of month: " & dayOfMonth)
			Return epochMonthToEpochDay(epochMonth) + (dayOfMonth - 1)
		End Function

		''' <summary>
		''' Returns day of year for the year and month.
		''' </summary>
		''' <param name="prolepticYear"> a proleptic year </param>
		''' <param name="month"> a month, 1-origin </param>
		''' <returns> the day of year, 1-origin </returns>
		Friend Function getDayOfYear(ByVal prolepticYear As Integer, ByVal month As Integer) As Integer
			Return yearMonthToDayOfYear(prolepticYear, (month - 1))
		End Function

		''' <summary>
		''' Returns month length for the year and month.
		''' </summary>
		''' <param name="prolepticYear"> a proleptic year </param>
		''' <param name="monthOfYear"> a month, 1-origin. </param>
		''' <returns> the length of the month </returns>
		Friend Function getMonthLength(ByVal prolepticYear As Integer, ByVal monthOfYear As Integer) As Integer
			Dim epochMonth As Integer = yearToEpochMonth(prolepticYear) + (monthOfYear - 1)
			If epochMonth < 0 OrElse epochMonth >= hijrahEpochMonthStartDays.Length Then Throw New java.time.DateTimeException("Invalid Hijrah date, year: " & prolepticYear & ", month: " & monthOfYear)
			Return epochMonthLength(epochMonth)
		End Function

		''' <summary>
		''' Returns year length.
		''' Note: The 12th month must exist in the data.
		''' </summary>
		''' <param name="prolepticYear"> a proleptic year </param>
		''' <returns> year length in days </returns>
		Friend Function getYearLength(ByVal prolepticYear As Integer) As Integer
			Return yearMonthToDayOfYear(prolepticYear, 12)
		End Function

		''' <summary>
		''' Return the minimum supported Hijrah year.
		''' </summary>
		''' <returns> the minimum </returns>
		Friend Property minimumYear As Integer
			Get
				Return epochMonthToYear(0)
			End Get
		End Property

		''' <summary>
		''' Return the maximum supported Hijrah ear.
		''' </summary>
		''' <returns> the minimum </returns>
		Friend Property maximumYear As Integer
			Get
				Return epochMonthToYear(hijrahEpochMonthStartDays.Length - 1) - 1
			End Get
		End Property

		''' <summary>
		''' Returns maximum day-of-month.
		''' </summary>
		''' <returns> maximum day-of-month </returns>
		Friend Property maximumMonthLength As Integer
			Get
				Return maxMonthLength
			End Get
		End Property

		''' <summary>
		''' Returns smallest maximum day-of-month.
		''' </summary>
		''' <returns> smallest maximum day-of-month </returns>
		Friend Property minimumMonthLength As Integer
			Get
				Return minMonthLength
			End Get
		End Property

		''' <summary>
		''' Returns maximum day-of-year.
		''' </summary>
		''' <returns> maximum day-of-year </returns>
		Friend Property maximumDayOfYear As Integer
			Get
				Return maxYearLength
			End Get
		End Property

		''' <summary>
		''' Returns smallest maximum day-of-year.
		''' </summary>
		''' <returns> smallest maximum day-of-year </returns>
		Friend Property smallestMaximumDayOfYear As Integer
			Get
				Return minYearLength
			End Get
		End Property

		''' <summary>
		''' Returns the epochMonth found by locating the epochDay in the table. The
		''' epochMonth is the index in the table
		''' </summary>
		''' <param name="epochDay"> </param>
		''' <returns> The index of the element of the start of the month containing the
		''' epochDay. </returns>
		Private Function epochDayToEpochMonth(ByVal epochDay As Integer) As Integer
			' binary search
			Dim ndx As Integer = java.util.Arrays.binarySearch(hijrahEpochMonthStartDays, epochDay)
			If ndx < 0 Then ndx = -ndx - 2
			Return ndx
		End Function

		''' <summary>
		''' Returns the year computed from the epochMonth
		''' </summary>
		''' <param name="epochMonth"> the epochMonth </param>
		''' <returns> the Hijrah Year </returns>
		Private Function epochMonthToYear(ByVal epochMonth As Integer) As Integer
			[Return] (epochMonth + hijrahStartEpochMonth) / 12
		End Function

		''' <summary>
		''' Returns the epochMonth for the Hijrah Year.
		''' </summary>
		''' <param name="year"> the HijrahYear </param>
		''' <returns> the epochMonth for the beginning of the year. </returns>
		Private Function yearToEpochMonth(ByVal year_Renamed As Integer) As Integer
			[Return] (year_Renamed * 12) - hijrahStartEpochMonth
		End Function

		''' <summary>
		''' Returns the Hijrah month from the epochMonth.
		''' </summary>
		''' <param name="epochMonth"> the epochMonth </param>
		''' <returns> the month of the Hijrah Year </returns>
		Private Function epochMonthToMonth(ByVal epochMonth As Integer) As Integer
			[Return] (epochMonth + hijrahStartEpochMonth) Mod 12
		End Function

		''' <summary>
		''' Returns the epochDay for the start of the epochMonth.
		''' </summary>
		''' <param name="epochMonth"> the epochMonth </param>
		''' <returns> the epochDay for the start of the epochMonth. </returns>
		Private Function epochMonthToEpochDay(ByVal epochMonth As Integer) As Integer
			Return hijrahEpochMonthStartDays(epochMonth)

		End Function

		''' <summary>
		''' Returns the day of year for the requested HijrahYear and month.
		''' </summary>
		''' <param name="prolepticYear"> the Hijrah year </param>
		''' <param name="month"> the Hijrah month </param>
		''' <returns> the day of year for the start of the month of the year </returns>
		Private Function yearMonthToDayOfYear(ByVal prolepticYear As Integer, ByVal month As Integer) As Integer
			Dim epochMonthFirst As Integer = yearToEpochMonth(prolepticYear)
			Return epochMonthToEpochDay(epochMonthFirst + month) - epochMonthToEpochDay(epochMonthFirst)
		End Function

		''' <summary>
		''' Returns the length of the epochMonth. It is computed from the start of
		''' the following month minus the start of the requested month.
		''' </summary>
		''' <param name="epochMonth"> the epochMonth; assumed to be within range </param>
		''' <returns> the length in days of the epochMonth </returns>
		Private Function epochMonthLength(ByVal epochMonth As Integer) As Integer
			' The very last entry in the epochMonth table is not the start of a month
			Return hijrahEpochMonthStartDays(epochMonth + 1) - hijrahEpochMonthStartDays(epochMonth)
		End Function

		'-----------------------------------------------------------------------
		Private Const KEY_ID As String = "id"
		Private Const KEY_TYPE As String = "type"
		Private Const KEY_VERSION As String = "version"
		Private Const KEY_ISO_START As String = "iso-start"

		''' <summary>
		''' Return the configuration properties from the resource.
		''' <p>
		''' The default location of the variant configuration resource is:
		''' <pre>
		'''   "$java.home/lib/" + resource-name
		''' </pre>
		''' </summary>
		''' <param name="resource"> the name of the calendar property resource </param>
		''' <returns> a Properties containing the properties read from the resource. </returns>
		''' <exception cref="Exception"> if access to the property resource fails </exception>
		Private Shared Function readConfigProperties(ByVal resource As String) As java.util.Properties
			Try
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return java.security.AccessController.doPrivileged(CType(, java.security.PrivilegedExceptionAction(Of java.util.Properties)) -> { String libDir = System.getProperty("java.home") + File.separator & "lib"; File file = New File(libDir, resource); java.util.Properties props = New java.util.Properties; try(java.io.InputStream is = New java.io.FileInputStream(file)) { props.load(is); } Return props; })
			Catch pax As java.security.PrivilegedActionException
				Throw pax.exception
			End Try
		End Function

		''' <summary>
		''' Loads and processes the Hijrah calendar properties file for this calendarType.
		''' The starting Hijrah date and the corresponding ISO date are
		''' extracted and used to calculate the epochDate offset.
		''' The version number is identified and ignored.
		''' Everything else is the data for a year with containing the length of each
		''' of 12 months.
		''' </summary>
		''' <exception cref="DateTimeException"> if initialization of the calendar data from the
		'''     resource fails </exception>
		Private Sub loadCalendarData()
			Try
				Dim resourceName As String = calendarProperties.getProperty(PROP_PREFIX + typeId)
				java.util.Objects.requireNonNull(resourceName, "Resource missing for calendar: " & PROP_PREFIX + typeId)
				Dim props As java.util.Properties = readConfigProperties(resourceName)

				Dim years As IDictionary(Of Integer?, Integer()) = New Dictionary(Of Integer?, Integer())
				Dim minYear As Integer = Integer.MaxValue
				Dim maxYear As Integer = Integer.MinValue
				Dim id_Renamed As String = Nothing
				Dim type As String = Nothing
				Dim version As String = Nothing
				Dim isoStart As Integer = 0
				For Each entry As KeyValuePair(Of Object, Object) In props
					Dim key As String = CStr(entry.Key)
					Select Case key
						Case KEY_ID
							id_Renamed = CStr(entry.Value)
						Case KEY_TYPE
							type = CStr(entry.Value)
						Case KEY_VERSION
							version = CStr(entry.Value)
						Case KEY_ISO_START
							Dim ymd As Integer() = parseYMD(CStr(entry.Value))
							isoStart = CInt(java.time.LocalDate.of(ymd(0), ymd(1), ymd(2)).toEpochDay())
							Exit Select
						Case Else
							Try
								' Everything else is either a year or invalid
								Dim year_Renamed As Integer = Convert.ToInt32(key)
								Dim months As Integer() = parseMonths(CStr(entry.Value))
								years(year_Renamed) = months
								maxYear = Math.Max(maxYear, year_Renamed)
								minYear = Math.Min(minYear, year_Renamed)
							Catch nfe As NumberFormatException
								Throw New IllegalArgumentException("bad key: " & key)
							End Try
					End Select
				Next entry

				If Not id.Equals(id_Renamed) Then Throw New IllegalArgumentException("Configuration is for a different calendar: " & id_Renamed)
				If Not calendarType.Equals(type) Then Throw New IllegalArgumentException("Configuration is for a different calendar type: " & type)
				If version Is Nothing OrElse version.empty Then Throw New IllegalArgumentException("Configuration does not contain a version")
				If isoStart = 0 Then Throw New IllegalArgumentException("Configuration does not contain a ISO start date")

				' Now create and validate the array of epochDays indexed by epochMonth
				hijrahStartEpochMonth = minYear * 12
				minEpochDay = isoStart
				hijrahEpochMonthStartDays = createEpochMonths(minEpochDay, minYear, maxYear, years)
				maxEpochDay = hijrahEpochMonthStartDays(hijrahEpochMonthStartDays.Length - 1)

				' Compute the min and max year length in days.
				For year_Renamed As Integer = minYear To maxYear - 1
					Dim length As Integer = getYearLength(year_Renamed)
					minYearLength = Math.Min(minYearLength, length)
					maxYearLength = Math.Max(maxYearLength, length)
				Next year_Renamed
			Catch ex As Exception
				' Log error and throw a DateTimeException
				Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.time.chrono")
				logger.severe("Unable to initialize Hijrah calendar proxy: " & typeId, ex)
				Throw New java.time.DateTimeException("Unable to initialize HijrahCalendar: " & typeId, ex)
			End Try
		End Sub

		''' <summary>
		''' Converts the map of year to month lengths ranging from minYear to maxYear
		''' into a linear contiguous array of epochDays. The index is the hijrahMonth
		''' computed from year and month and offset by minYear. The value of each
		''' entry is the epochDay corresponding to the first day of the month.
		''' </summary>
		''' <param name="minYear"> The minimum year for which data is provided </param>
		''' <param name="maxYear"> The maximum year for which data is provided </param>
		''' <param name="years"> a Map of year to the array of 12 month lengths </param>
		''' <returns> array of epochDays for each month from min to max </returns>
		Private Function createEpochMonths(ByVal epochDay As Integer, ByVal minYear As Integer, ByVal maxYear As Integer, ByVal years As IDictionary(Of Integer?, Integer())) As Integer()
			' Compute the size for the array of dates
			Dim numMonths As Integer = (maxYear - minYear + 1) * 12 + 1

			' Initialize the running epochDay as the corresponding ISO Epoch day
			Dim epochMonth As Integer = 0 ' index into array of epochMonths
			Dim epochMonths As Integer() = New Integer(numMonths - 1){}
			minMonthLength = Integer.MaxValue
			maxMonthLength = Integer.MinValue

			' Only whole years are valid, any zero's in the array are illegal
			For year_Renamed As Integer = minYear To maxYear
				Dim months As Integer() = years(year_Renamed) ' must not be gaps
				For month As Integer = 0 To 11
					Dim length As Integer = months(month)
					epochMonths(epochMonth) = epochDay
					epochMonth += 1

					If length < 29 OrElse length > 32 Then Throw New IllegalArgumentException("Invalid month length in year: " & minYear)
					epochDay += length
					minMonthLength = Math.Min(minMonthLength, length)
					maxMonthLength = Math.Max(maxMonthLength, length)
				Next month
			Next year_Renamed

			' Insert the final epochDay
			epochMonths(epochMonth) = epochDay
			epochMonth += 1

			If epochMonth <> epochMonths.Length Then Throw New IllegalStateException("Did not fill epochMonths exactly: ndx = " & epochMonth & " should be " & epochMonths.Length)

			Return epochMonths
		End Function

		''' <summary>
		''' Parses the 12 months lengths from a property value for a specific year.
		''' </summary>
		''' <param name="line"> the value of a year property </param>
		''' <returns> an array of int[12] containing the 12 month lengths </returns>
		''' <exception cref="IllegalArgumentException"> if the number of months is not 12 </exception>
		''' <exception cref="NumberFormatException"> if the 12 tokens are not numbers </exception>
		Private Function parseMonths(ByVal line As String) As Integer()
			Dim months As Integer() = New Integer(11){}
			Dim numbers As String() = line.Split("\s")
			If numbers.Length <> 12 Then Throw New IllegalArgumentException("wrong number of months on line: " & java.util.Arrays.ToString(numbers) & "; count: " & numbers.Length)
			For i As Integer = 0 To 11
				Try
					months(i) = Convert.ToInt32(numbers(i))
				Catch nfe As NumberFormatException
					Throw New IllegalArgumentException("bad key: " & numbers(i))
				End Try
			Next i
			Return months
		End Function

		''' <summary>
		''' Parse yyyy-MM-dd into a 3 element array [yyyy, mm, dd].
		''' </summary>
		''' <param name="string"> the input string </param>
		''' <returns> the 3 element array with year, month, day </returns>
		Private Function parseYMD(ByVal [string] As String) As Integer()
			' yyyy-MM-dd
			string_Renamed = string_Renamed.Trim()
			Try
				If string_Renamed.Chars(4) <> "-"c OrElse string_Renamed.Chars(7) <> "-"c Then Throw New IllegalArgumentException("date must be yyyy-MM-dd")
				Dim ymd As Integer() = New Integer(2){}
				ymd(0) = Convert.ToInt32(string_Renamed.Substring(0, 4))
				ymd(1) = Convert.ToInt32(string_Renamed.Substring(5, 2))
				ymd(2) = Convert.ToInt32(string_Renamed.Substring(8, 2))
				Return ymd
			Catch ex As NumberFormatException
				Throw New IllegalArgumentException("date must be yyyy-MM-dd", ex)
			End Try
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