Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
' *
' *
' *
' *
' *
' * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.temporal



	''' <summary>
	''' Localized definitions of the day-of-week, week-of-month and week-of-year fields.
	''' <p>
	''' A standard week is seven days long, but cultures have different definitions for some
	''' other aspects of a week. This class represents the definition of the week, for the
	''' purpose of providing <seealso cref="TemporalField"/> instances.
	''' <p>
	''' WeekFields provides five fields,
	''' <seealso cref="#dayOfWeek()"/>, <seealso cref="#weekOfMonth()"/>, <seealso cref="#weekOfYear()"/>,
	''' <seealso cref="#weekOfWeekBasedYear()"/>, and <seealso cref="#weekBasedYear()"/>
	''' that provide access to the values from any <seealso cref="Temporal temporal object"/>.
	''' <p>
	''' The computations for day-of-week, week-of-month, and week-of-year are based
	''' on the  <seealso cref="ChronoField#YEAR proleptic-year"/>,
	''' <seealso cref="ChronoField#MONTH_OF_YEAR month-of-year"/>,
	''' <seealso cref="ChronoField#DAY_OF_MONTH day-of-month"/>, and
	''' <seealso cref="ChronoField#DAY_OF_WEEK ISO day-of-week"/> which are based on the
	''' <seealso cref="ChronoField#EPOCH_DAY epoch-day"/> and the chronology.
	''' The values may not be aligned with the <seealso cref="ChronoField#YEAR_OF_ERA year-of-Era"/>
	''' depending on the Chronology.
	''' <p>A week is defined by:
	''' <ul>
	''' <li>The first day-of-week.
	''' For example, the ISO-8601 standard considers Monday to be the first day-of-week.
	''' <li>The minimal number of days in the first week.
	''' For example, the ISO-8601 standard counts the first week as needing at least 4 days.
	''' </ul>
	''' Together these two values allow a year or month to be divided into weeks.
	''' 
	''' <h3>Week of Month</h3>
	''' One field is used: week-of-month.
	''' The calculation ensures that weeks never overlap a month boundary.
	''' The month is divided into periods where each period starts on the defined first day-of-week.
	''' The earliest period is referred to as week 0 if it has less than the minimal number of days
	''' and week 1 if it has at least the minimal number of days.
	''' 
	''' <table cellpadding="0" cellspacing="3" border="0" style="text-align: left; width: 50%;">
	''' <caption>Examples of WeekFields</caption>
	''' <tr><th>Date</th><td>Day-of-week</td>
	'''  <td>First day: Monday<br>Minimal days: 4</td><td>First day: Monday<br>Minimal days: 5</td></tr>
	''' <tr><th>2008-12-31</th><td>Wednesday</td>
	'''  <td>Week 5 of December 2008</td><td>Week 5 of December 2008</td></tr>
	''' <tr><th>2009-01-01</th><td>Thursday</td>
	'''  <td>Week 1 of January 2009</td><td>Week 0 of January 2009</td></tr>
	''' <tr><th>2009-01-04</th><td>Sunday</td>
	'''  <td>Week 1 of January 2009</td><td>Week 0 of January 2009</td></tr>
	''' <tr><th>2009-01-05</th><td>Monday</td>
	'''  <td>Week 2 of January 2009</td><td>Week 1 of January 2009</td></tr>
	''' </table>
	''' 
	''' <h3>Week of Year</h3>
	''' One field is used: week-of-year.
	''' The calculation ensures that weeks never overlap a year boundary.
	''' The year is divided into periods where each period starts on the defined first day-of-week.
	''' The earliest period is referred to as week 0 if it has less than the minimal number of days
	''' and week 1 if it has at least the minimal number of days.
	''' 
	''' <h3>Week Based Year</h3>
	''' Two fields are used for week-based-year, one for the
	''' <seealso cref="#weekOfWeekBasedYear() week-of-week-based-year"/> and one for
	''' <seealso cref="#weekBasedYear() week-based-year"/>.  In a week-based-year, each week
	''' belongs to only a single year.  Week 1 of a year is the first week that
	''' starts on the first day-of-week and has at least the minimum number of days.
	''' The first and last weeks of a year may contain days from the
	''' previous calendar year or next calendar year respectively.
	''' 
	''' <table cellpadding="0" cellspacing="3" border="0" style="text-align: left; width: 50%;">
	''' <caption>Examples of WeekFields for week-based-year</caption>
	''' <tr><th>Date</th><td>Day-of-week</td>
	'''  <td>First day: Monday<br>Minimal days: 4</td><td>First day: Monday<br>Minimal days: 5</td></tr>
	''' <tr><th>2008-12-31</th><td>Wednesday</td>
	'''  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	''' <tr><th>2009-01-01</th><td>Thursday</td>
	'''  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	''' <tr><th>2009-01-04</th><td>Sunday</td>
	'''  <td>Week 1 of 2009</td><td>Week 53 of 2008</td></tr>
	''' <tr><th>2009-01-05</th><td>Monday</td>
	'''  <td>Week 2 of 2009</td><td>Week 1 of 2009</td></tr>
	''' </table>
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class WeekFields
		' implementation notes
		' querying week-of-month or week-of-year should return the week value bound within the month/year
		' however, setting the week value should be lenient (use plus/minus weeks)
		' allow week-of-month outer range [0 to 6]
		' allow week-of-year outer range [0 to 54]
		' this is because callers shouldn't be expected to know the details of validity

		''' <summary>
		''' The cache of rules by firstDayOfWeek plus minimalDays.
		''' Initialized first to be available for definition of ISO, etc.
		''' </summary>
		Private Shared ReadOnly CACHE As java.util.concurrent.ConcurrentMap(Of String, WeekFields) = New ConcurrentDictionary(Of String, WeekFields)(4, 0.75f, 2)

		''' <summary>
		''' The ISO-8601 definition, where a week starts on Monday and the first week
		''' has a minimum of 4 days.
		''' <p>
		''' The ISO-8601 standard defines a calendar system based on weeks.
		''' It uses the week-based-year and week-of-week-based-year concepts to split
		''' up the passage of days instead of the standard year/month/day.
		''' <p>
		''' Note that the first week may start in the previous calendar year.
		''' Note also that the first few days of a calendar year may be in the
		''' week-based-year corresponding to the previous calendar year.
		''' </summary>
		Public Shared ReadOnly ISO As New WeekFields(java.time.DayOfWeek.MONDAY, 4)

		''' <summary>
		''' The common definition of a week that starts on Sunday and the first week
		''' has a minimum of 1 day.
		''' <p>
		''' Defined as starting on Sunday and with a minimum of 1 day in the month.
		''' This week definition is in use in the US and other European countries.
		''' </summary>
		Public Shared ReadOnly SUNDAY_START As WeekFields = WeekFields.of(java.time.DayOfWeek.SUNDAY, 1)

		''' <summary>
		''' The unit that represents week-based-years for the purpose of addition and subtraction.
		''' <p>
		''' This allows a number of week-based-years to be added to, or subtracted from, a date.
		''' The unit is equal to either 52 or 53 weeks.
		''' The estimated duration of a week-based-year is the same as that of a standard ISO
		''' year at {@code 365.2425 Days}.
		''' <p>
		''' The rules for addition add the number of week-based-years to the existing value
		''' for the week-based-year field retaining the week-of-week-based-year
		''' and day-of-week, unless the week number it too large for the target year.
		''' In that case, the week is set to the last week of the year
		''' with the same day-of-week.
		''' <p>
		''' This unit is an immutable and thread-safe singleton.
		''' </summary>
		Public Shared ReadOnly WEEK_BASED_YEARS As TemporalUnit = IsoFields.WEEK_BASED_YEARS

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -1177360819670808121L

		''' <summary>
		''' The first day-of-week.
		''' </summary>
		Private ReadOnly firstDayOfWeek As java.time.DayOfWeek
		''' <summary>
		''' The minimal number of days in the first week.
		''' </summary>
		Private ReadOnly minimalDays As Integer
		''' <summary>
		''' The field used to access the computed DayOfWeek.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly dayOfWeek_Renamed As TemporalField = ComputedDayOfField.ofDayOfWeekField(Me)
		''' <summary>
		''' The field used to access the computed WeekOfMonth.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly weekOfMonth_Renamed As TemporalField = ComputedDayOfField.ofWeekOfMonthField(Me)
		''' <summary>
		''' The field used to access the computed WeekOfYear.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly weekOfYear_Renamed As TemporalField = ComputedDayOfField.ofWeekOfYearField(Me)
		''' <summary>
		''' The field that represents the week-of-week-based-year.
		''' <p>
		''' This field allows the week of the week-based-year value to be queried and set.
		''' <p>
		''' This unit is an immutable and thread-safe singleton.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly weekOfWeekBasedYear_Renamed As TemporalField = ComputedDayOfField.ofWeekOfWeekBasedYearField(Me)
		''' <summary>
		''' The field that represents the week-based-year.
		''' <p>
		''' This field allows the week-based-year value to be queried and set.
		''' <p>
		''' This unit is an immutable and thread-safe singleton.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly weekBasedYear_Renamed As TemporalField = ComputedDayOfField.ofWeekBasedYearField(Me)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code WeekFields} appropriate for a locale.
		''' <p>
		''' This will look up appropriate values from the provider of localization data.
		''' </summary>
		''' <param name="locale">  the locale to use, not null </param>
		''' <returns> the week-definition, not null </returns>
		Public Shared Function [of](  locale As java.util.Locale) As WeekFields
			java.util.Objects.requireNonNull(locale, "locale")
			locale = New java.util.Locale(locale.language, locale.country) ' elminate variants

			Dim calDow As Integer = sun.util.locale.provider.CalendarDataUtility.retrieveFirstDayOfWeek(locale)
			Dim dow As java.time.DayOfWeek = java.time.DayOfWeek.SUNDAY.plus(calDow - 1)
			Dim minDays As Integer = sun.util.locale.provider.CalendarDataUtility.retrieveMinimalDaysInFirstWeek(locale)
			Return WeekFields.of(dow, minDays)
		End Function

		''' <summary>
		''' Obtains an instance of {@code WeekFields} from the first day-of-week and minimal days.
		''' <p>
		''' The first day-of-week defines the ISO {@code DayOfWeek} that is day 1 of the week.
		''' The minimal number of days in the first week defines how many days must be present
		''' in a month or year, starting from the first day-of-week, before the week is counted
		''' as the first week. A value of 1 will count the first day of the month or year as part
		''' of the first week, whereas a value of 7 will require the whole seven days to be in
		''' the new month or year.
		''' <p>
		''' WeekFields instances are singletons; for each unique combination
		''' of {@code firstDayOfWeek} and {@code minimalDaysInFirstWeek} the
		''' the same instance will be returned.
		''' </summary>
		''' <param name="firstDayOfWeek">  the first day of the week, not null </param>
		''' <param name="minimalDaysInFirstWeek">  the minimal number of days in the first week, from 1 to 7 </param>
		''' <returns> the week-definition, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the minimal days value is less than one
		'''      or greater than 7 </exception>
		Public Shared Function [of](  firstDayOfWeek As java.time.DayOfWeek,   minimalDaysInFirstWeek As Integer) As WeekFields
			Dim key As String = firstDayOfWeek.ToString() & minimalDaysInFirstWeek
			Dim rules As WeekFields = CACHE.get(key)
			If rules Is Nothing Then
				rules = New WeekFields(firstDayOfWeek, minimalDaysInFirstWeek)
				CACHE.putIfAbsent(key, rules)
				rules = CACHE.get(key)
			End If
			Return rules
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance of the definition.
		''' </summary>
		''' <param name="firstDayOfWeek">  the first day of the week, not null </param>
		''' <param name="minimalDaysInFirstWeek">  the minimal number of days in the first week, from 1 to 7 </param>
		''' <exception cref="IllegalArgumentException"> if the minimal days value is invalid </exception>
		Private Sub New(  firstDayOfWeek As java.time.DayOfWeek,   minimalDaysInFirstWeek As Integer)
			java.util.Objects.requireNonNull(firstDayOfWeek, "firstDayOfWeek")
			If minimalDaysInFirstWeek < 1 OrElse minimalDaysInFirstWeek > 7 Then Throw New IllegalArgumentException("Minimal number of days is invalid")
			Me.firstDayOfWeek = firstDayOfWeek
			Me.minimalDays = minimalDaysInFirstWeek
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Restore the state of a WeekFields from the stream.
		''' Check that the values are valid.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> if the serialized object has an invalid
		'''     value for firstDayOfWeek or minimalDays. </exception>
		''' <exception cref="ClassNotFoundException"> if a class cannot be resolved </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If firstDayOfWeek Is Nothing Then Throw New java.io.InvalidObjectException("firstDayOfWeek is null")

			If minimalDays < 1 OrElse minimalDays > 7 Then Throw New java.io.InvalidObjectException("Minimal number of days is invalid")
		End Sub

		''' <summary>
		''' Return the singleton WeekFields associated with the
		''' {@code firstDayOfWeek} and {@code minimalDays}. </summary>
		''' <returns> the singleton WeekFields for the firstDayOfWeek and minimalDays. </returns>
		''' <exception cref="InvalidObjectException"> if the serialized object has invalid
		'''     values for firstDayOfWeek or minimalDays. </exception>
		Private Function readResolve() As Object
			Try
				Return WeekFields.of(firstDayOfWeek, minimalDays)
			Catch iae As IllegalArgumentException
				Throw New java.io.InvalidObjectException("Invalid serialized WeekFields: " & iae.Message)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the first day-of-week.
		''' <p>
		''' The first day-of-week varies by culture.
		''' For example, the US uses Sunday, while France and the ISO-8601 standard use Monday.
		''' This method returns the first day using the standard {@code DayOfWeek} enum.
		''' </summary>
		''' <returns> the first day-of-week, not null </returns>
		Public Property firstDayOfWeek As java.time.DayOfWeek
			Get
				Return firstDayOfWeek
			End Get
		End Property

		''' <summary>
		''' Gets the minimal number of days in the first week.
		''' <p>
		''' The number of days considered to define the first week of a month or year
		''' varies by culture.
		''' For example, the ISO-8601 requires 4 days (more than half a week) to
		''' be present before counting the first week.
		''' </summary>
		''' <returns> the minimal number of days in the first week of a month or year, from 1 to 7 </returns>
		Public Property minimalDaysInFirstWeek As Integer
			Get
				Return minimalDays
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a field to access the day of week based on this {@code WeekFields}.
		''' <p>
		''' This is similar to <seealso cref="ChronoField#DAY_OF_WEEK"/> but uses values for
		''' the day-of-week based on this {@code WeekFields}.
		''' The days are numbered from 1 to 7 where the
		''' <seealso cref="#getFirstDayOfWeek() first day-of-week"/> is assigned the value 1.
		''' <p>
		''' For example, if the first day-of-week is Sunday, then that will have the
		''' value 1, with other days ranging from Monday as 2 to Saturday as 7.
		''' <p>
		''' In the resolving phase of parsing, a localized day-of-week will be converted
		''' to a standardized {@code ChronoField} day-of-week.
		''' The day-of-week must be in the valid range 1 to 7.
		''' Other fields in this class build dates using the standardized day-of-week.
		''' </summary>
		''' <returns> a field providing access to the day-of-week with localized numbering, not null </returns>
		Public Function dayOfWeek() As TemporalField
			Return dayOfWeek_Renamed
		End Function

		''' <summary>
		''' Returns a field to access the week of month based on this {@code WeekFields}.
		''' <p>
		''' This represents the concept of the count of weeks within the month where weeks
		''' start on a fixed day-of-week, such as Monday.
		''' This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/>.
		''' <p>
		''' Week one (1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		''' where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the month.
		''' Thus, week one may start up to {@code minDays} days before the start of the month.
		''' If the first week starts after the start of the month then the period before is week zero (0).
		''' <p>
		''' For example:<br>
		''' - if the 1st day of the month is a Monday, week one starts on the 1st and there is no week zero<br>
		''' - if the 2nd day of the month is a Monday, week one starts on the 2nd and the 1st is in week zero<br>
		''' - if the 4th day of the month is a Monday, week one starts on the 4th and the 1st to 3rd is in week zero<br>
		''' - if the 5th day of the month is a Monday, week two starts on the 5th and the 1st to 4th is in week one<br>
		''' <p>
		''' This field can be used with any calendar system.
		''' <p>
		''' In the resolving phase of parsing, a date can be created from a year,
		''' week-of-month, month-of-year and day-of-week.
		''' <p>
		''' In <seealso cref="ResolverStyle#STRICT strict mode"/>, all four fields are
		''' validated against their range of valid values. The week-of-month field
		''' is validated to ensure that the resulting month is the month requested.
		''' <p>
		''' In <seealso cref="ResolverStyle#SMART smart mode"/>, all four fields are
		''' validated against their range of valid values. The week-of-month field
		''' is validated from 0 to 6, meaning that the resulting date can be in a
		''' different month to that specified.
		''' <p>
		''' In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		''' are validated against the range of valid values. The resulting date is calculated
		''' equivalent to the following four stage approach.
		''' First, create a date on the first day of the first week of January in the requested year.
		''' Then take the month-of-year, subtract one, and add the amount in months to the date.
		''' Then take the week-of-month, subtract one, and add the amount in weeks to the date.
		''' Finally, adjust to the correct day-of-week within the localized week.
		''' </summary>
		''' <returns> a field providing access to the week-of-month, not null </returns>
		Public Function weekOfMonth() As TemporalField
			Return weekOfMonth_Renamed
		End Function

		''' <summary>
		''' Returns a field to access the week of year based on this {@code WeekFields}.
		''' <p>
		''' This represents the concept of the count of weeks within the year where weeks
		''' start on a fixed day-of-week, such as Monday.
		''' This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/>.
		''' <p>
		''' Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		''' where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		''' Thus, week one may start up to {@code minDays} days before the start of the year.
		''' If the first week starts after the start of the year then the period before is week zero (0).
		''' <p>
		''' For example:<br>
		''' - if the 1st day of the year is a Monday, week one starts on the 1st and there is no week zero<br>
		''' - if the 2nd day of the year is a Monday, week one starts on the 2nd and the 1st is in week zero<br>
		''' - if the 4th day of the year is a Monday, week one starts on the 4th and the 1st to 3rd is in week zero<br>
		''' - if the 5th day of the year is a Monday, week two starts on the 5th and the 1st to 4th is in week one<br>
		''' <p>
		''' This field can be used with any calendar system.
		''' <p>
		''' In the resolving phase of parsing, a date can be created from a year,
		''' week-of-year and day-of-week.
		''' <p>
		''' In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-year field
		''' is validated to ensure that the resulting year is the year requested.
		''' <p>
		''' In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-year field
		''' is validated from 0 to 54, meaning that the resulting date can be in a
		''' different year to that specified.
		''' <p>
		''' In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		''' are validated against the range of valid values. The resulting date is calculated
		''' equivalent to the following three stage approach.
		''' First, create a date on the first day of the first week in the requested year.
		''' Then take the week-of-year, subtract one, and add the amount in weeks to the date.
		''' Finally, adjust to the correct day-of-week within the localized week.
		''' </summary>
		''' <returns> a field providing access to the week-of-year, not null </returns>
		Public Function weekOfYear() As TemporalField
			Return weekOfYear_Renamed
		End Function

		''' <summary>
		''' Returns a field to access the week of a week-based-year based on this {@code WeekFields}.
		''' <p>
		''' This represents the concept of the count of weeks within the year where weeks
		''' start on a fixed day-of-week, such as Monday and each week belongs to exactly one year.
		''' This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/> and
		''' <seealso cref="WeekFields#weekBasedYear()"/>.
		''' <p>
		''' Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		''' where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		''' If the first week starts after the start of the year then the period before
		''' is in the last week of the previous year.
		''' <p>
		''' For example:<br>
		''' - if the 1st day of the year is a Monday, week one starts on the 1st<br>
		''' - if the 2nd day of the year is a Monday, week one starts on the 2nd and
		'''   the 1st is in the last week of the previous year<br>
		''' - if the 4th day of the year is a Monday, week one starts on the 4th and
		'''   the 1st to 3rd is in the last week of the previous year<br>
		''' - if the 5th day of the year is a Monday, week two starts on the 5th and
		'''   the 1st to 4th is in week one<br>
		''' <p>
		''' This field can be used with any calendar system.
		''' <p>
		''' In the resolving phase of parsing, a date can be created from a week-based-year,
		''' week-of-year and day-of-week.
		''' <p>
		''' In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-year field
		''' is validated to ensure that the resulting week-based-year is the
		''' week-based-year requested.
		''' <p>
		''' In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-week-based-year field
		''' is validated from 1 to 53, meaning that the resulting date can be in the
		''' following week-based-year to that specified.
		''' <p>
		''' In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		''' are validated against the range of valid values. The resulting date is calculated
		''' equivalent to the following three stage approach.
		''' First, create a date on the first day of the first week in the requested week-based-year.
		''' Then take the week-of-week-based-year, subtract one, and add the amount in weeks to the date.
		''' Finally, adjust to the correct day-of-week within the localized week.
		''' </summary>
		''' <returns> a field providing access to the week-of-week-based-year, not null </returns>
		Public Function weekOfWeekBasedYear() As TemporalField
			Return weekOfWeekBasedYear_Renamed
		End Function

		''' <summary>
		''' Returns a field to access the year of a week-based-year based on this {@code WeekFields}.
		''' <p>
		''' This represents the concept of the year where weeks start on a fixed day-of-week,
		''' such as Monday and each week belongs to exactly one year.
		''' This field is typically used with <seealso cref="WeekFields#dayOfWeek()"/> and
		''' <seealso cref="WeekFields#weekOfWeekBasedYear()"/>.
		''' <p>
		''' Week one(1) is the week starting on the <seealso cref="WeekFields#getFirstDayOfWeek"/>
		''' where there are at least <seealso cref="WeekFields#getMinimalDaysInFirstWeek()"/> days in the year.
		''' Thus, week one may start before the start of the year.
		''' If the first week starts after the start of the year then the period before
		''' is in the last week of the previous year.
		''' <p>
		''' This field can be used with any calendar system.
		''' <p>
		''' In the resolving phase of parsing, a date can be created from a week-based-year,
		''' week-of-year and day-of-week.
		''' <p>
		''' In <seealso cref="ResolverStyle#STRICT strict mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-year field
		''' is validated to ensure that the resulting week-based-year is the
		''' week-based-year requested.
		''' <p>
		''' In <seealso cref="ResolverStyle#SMART smart mode"/>, all three fields are
		''' validated against their range of valid values. The week-of-week-based-year field
		''' is validated from 1 to 53, meaning that the resulting date can be in the
		''' following week-based-year to that specified.
		''' <p>
		''' In <seealso cref="ResolverStyle#LENIENT lenient mode"/>, the year and day-of-week
		''' are validated against the range of valid values. The resulting date is calculated
		''' equivalent to the following three stage approach.
		''' First, create a date on the first day of the first week in the requested week-based-year.
		''' Then take the week-of-week-based-year, subtract one, and add the amount in weeks to the date.
		''' Finally, adjust to the correct day-of-week within the localized week.
		''' </summary>
		''' <returns> a field providing access to the week-based-year, not null </returns>
		Public Function weekBasedYear() As TemporalField
			Return weekBasedYear_Renamed
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this {@code WeekFields} is equal to the specified object.
		''' <p>
		''' The comparison is based on the entire state of the rules, which is
		''' the first day-of-week and minimal days.
		''' </summary>
		''' <param name="object">  the other rules to compare to, null returns false </param>
		''' <returns> true if this is equal to the specified rules </returns>
		Public Overrides Function Equals(  [object] As Object) As Boolean
			If Me Is object_Renamed Then Return True
			If TypeOf object_Renamed Is WeekFields Then Return GetHashCode() = object_Renamed.GetHashCode()
			Return False
		End Function

		''' <summary>
		''' A hash code for this {@code WeekFields}.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return firstDayOfWeek.ordinal() * 7 + minimalDays
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' A string representation of this {@code WeekFields} instance.
		''' </summary>
		''' <returns> the string representation, not null </returns>
		Public Overrides Function ToString() As String
			Return "WeekFields[" & firstDayOfWeek + AscW(","c) + minimalDays + AscW("]"c)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Field type that computes DayOfWeek, WeekOfMonth, and WeekOfYear
		''' based on a WeekFields.
		''' A separate Field instance is required for each different WeekFields;
		''' combination of start of week and minimum number of days.
		''' Constructors are provided to create fields for DayOfWeek, WeekOfMonth,
		''' and WeekOfYear.
		''' </summary>
		Friend Class ComputedDayOfField
			Implements TemporalField

			''' <summary>
			''' Returns a field to access the day of week,
			''' computed based on a WeekFields.
			''' <p>
			''' The WeekDefintion of the first day of the week is used with
			''' the ISO DAY_OF_WEEK field to compute week boundaries.
			''' </summary>
			Shared Function ofDayOfWeekField(  weekDef As WeekFields) As ComputedDayOfField
				Return New ComputedDayOfField("DayOfWeek", weekDef, DAYS, WEEKS, DAY_OF_WEEK_RANGE)
			End Function

			''' <summary>
			''' Returns a field to access the week of month,
			''' computed based on a WeekFields. </summary>
			''' <seealso cref= WeekFields#weekOfMonth() </seealso>
			Shared Function ofWeekOfMonthField(  weekDef As WeekFields) As ComputedDayOfField
				Return New ComputedDayOfField("WeekOfMonth", weekDef, WEEKS, MONTHS, WEEK_OF_MONTH_RANGE)
			End Function

			''' <summary>
			''' Returns a field to access the week of year,
			''' computed based on a WeekFields. </summary>
			''' <seealso cref= WeekFields#weekOfYear() </seealso>
			Shared Function ofWeekOfYearField(  weekDef As WeekFields) As ComputedDayOfField
				Return New ComputedDayOfField("WeekOfYear", weekDef, WEEKS, YEARS, WEEK_OF_YEAR_RANGE)
			End Function

			''' <summary>
			''' Returns a field to access the week of week-based-year,
			''' computed based on a WeekFields. </summary>
			''' <seealso cref= WeekFields#weekOfWeekBasedYear() </seealso>
			Shared Function ofWeekOfWeekBasedYearField(  weekDef As WeekFields) As ComputedDayOfField
				Return New ComputedDayOfField("WeekOfWeekBasedYear", weekDef, WEEKS, IsoFields.WEEK_BASED_YEARS, WEEK_OF_WEEK_BASED_YEAR_RANGE)
			End Function

			''' <summary>
			''' Returns a field to access the week of week-based-year,
			''' computed based on a WeekFields. </summary>
			''' <seealso cref= WeekFields#weekBasedYear() </seealso>
			Shared Function ofWeekBasedYearField(  weekDef As WeekFields) As ComputedDayOfField
				Return New ComputedDayOfField("WeekBasedYear", weekDef, IsoFields.WEEK_BASED_YEARS, FOREVER, ChronoField.YEAR.range())
			End Function

			''' <summary>
			''' Return a new week-based-year date of the Chronology, year, week-of-year,
			''' and dow of week. </summary>
			''' <param name="chrono"> The chronology of the new date </param>
			''' <param name="yowby"> the year of the week-based-year </param>
			''' <param name="wowby"> the week of the week-based-year </param>
			''' <param name="dow"> the day of the week </param>
			''' <returns> a ChronoLocalDate for the requested year, week of year, and day of week </returns>
			Private Function ofWeekBasedYear(  chrono As java.time.chrono.Chronology,   yowby As Integer,   wowby As Integer,   dow As Integer) As java.time.chrono.ChronoLocalDate
				Dim date_Renamed As java.time.chrono.ChronoLocalDate = chrono.date(yowby, 1, 1)
				Dim ldow As Integer = localizedDayOfWeek(date_Renamed)
				Dim offset As Integer = startOfWeekOffset(1, ldow)

				' Clamp the week of year to keep it in the same year
				Dim yearLen As Integer = date_Renamed.lengthOfYear()
				Dim newYearWeek As Integer = computeWeek(offset, yearLen + weekDef.minimalDaysInFirstWeek)
				wowby = System.Math.Min(wowby, newYearWeek - 1)

				Dim days As Integer = -offset + (dow - 1) + (wowby - 1) * 7
				Return date_Renamed.plus(days, DAYS)
			End Function

			Private ReadOnly name As String
			Private ReadOnly weekDef As WeekFields
			Private ReadOnly baseUnit As TemporalUnit
			Private ReadOnly rangeUnit As TemporalUnit
			Private ReadOnly range_Renamed As ValueRange

			Private Sub New(  name As String,   weekDef As WeekFields,   baseUnit As TemporalUnit,   rangeUnit As TemporalUnit,   range As ValueRange)
				Me.name = name
				Me.weekDef = weekDef
				Me.baseUnit = baseUnit
				Me.rangeUnit = rangeUnit
				Me.range_Renamed = range
			End Sub

			Private Shared ReadOnly DAY_OF_WEEK_RANGE As ValueRange = ValueRange.of(1, 7)
			Private Shared ReadOnly WEEK_OF_MONTH_RANGE As ValueRange = ValueRange.of(0, 1, 4, 6)
			Private Shared ReadOnly WEEK_OF_YEAR_RANGE As ValueRange = ValueRange.of(0, 1, 52, 54)
			Private Shared ReadOnly WEEK_OF_WEEK_BASED_YEAR_RANGE As ValueRange = ValueRange.of(1, 52, 53)

			Public Overrides Function getFrom(  temporal As TemporalAccessor) As Long Implements TemporalField.getFrom
				If rangeUnit = WEEKS Then ' day-of-week
					Return localizedDayOfWeek(temporal) ' week-of-month
				ElseIf rangeUnit = MONTHS Then
					Return localizedWeekOfMonth(temporal) ' week-of-year
				ElseIf rangeUnit = YEARS Then
					Return localizedWeekOfYear(temporal)
				ElseIf rangeUnit = WEEK_BASED_YEARS Then
					Return localizedWeekOfWeekBasedYear(temporal)
				ElseIf rangeUnit = FOREVER Then
					Return localizedWeekBasedYear(temporal)
				Else
					Throw New IllegalStateException("unreachable, rangeUnit: " & rangeUnit & ", this: " & Me)
				End If
			End Function

			Private Function localizedDayOfWeek(  temporal As TemporalAccessor) As Integer
				Dim sow As Integer = weekDef.firstDayOfWeek.value
				Dim isoDow As Integer = temporal.get(DAY_OF_WEEK)
				Return System.Math.floorMod(isoDow - sow, 7) + 1
			End Function

			Private Function localizedDayOfWeek(  isoDow As Integer) As Integer
				Dim sow As Integer = weekDef.firstDayOfWeek.value
				Return System.Math.floorMod(isoDow - sow, 7) + 1
			End Function

			Private Function localizedWeekOfMonth(  temporal As TemporalAccessor) As Long
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim dom As Integer = temporal.get(DAY_OF_MONTH)
				Dim offset As Integer = startOfWeekOffset(dom, dow)
				Return computeWeek(offset, dom)
			End Function

			Private Function localizedWeekOfYear(  temporal As TemporalAccessor) As Long
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim doy As Integer = temporal.get(DAY_OF_YEAR)
				Dim offset As Integer = startOfWeekOffset(doy, dow)
				Return computeWeek(offset, doy)
			End Function

			''' <summary>
			''' Returns the year of week-based-year for the temporal.
			''' The year can be the previous year, the current year, or the next year. </summary>
			''' <param name="temporal"> a date of any chronology, not null </param>
			''' <returns> the year of week-based-year for the date </returns>
			Private Function localizedWeekBasedYear(  temporal As TemporalAccessor) As Integer
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim year_Renamed As Integer = temporal.get(YEAR)
				Dim doy As Integer = temporal.get(DAY_OF_YEAR)
				Dim offset As Integer = startOfWeekOffset(doy, dow)
				Dim week As Integer = computeWeek(offset, doy)
				If week = 0 Then
					' Day is in end of week of previous year; return the previous year
					Return year_Renamed - 1
				Else
					' If getting close to end of year, use higher precision logic
					' Check if date of year is in partial week associated with next year
					Dim dayRange As ValueRange = temporal.range(DAY_OF_YEAR)
					Dim yearLen As Integer = CInt(dayRange.maximum)
					Dim newYearWeek As Integer = computeWeek(offset, yearLen + weekDef.minimalDaysInFirstWeek)
					If week >= newYearWeek Then Return year_Renamed + 1
				End If
				Return year_Renamed
			End Function

			''' <summary>
			''' Returns the week of week-based-year for the temporal.
			''' The week can be part of the previous year, the current year,
			''' or the next year depending on the week start and minimum number
			''' of days. </summary>
			''' <param name="temporal">  a date of any chronology </param>
			''' <returns> the week of the year </returns>
			''' <seealso cref= #localizedWeekBasedYear(java.time.temporal.TemporalAccessor) </seealso>
			Private Function localizedWeekOfWeekBasedYear(  temporal As TemporalAccessor) As Integer
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim doy As Integer = temporal.get(DAY_OF_YEAR)
				Dim offset As Integer = startOfWeekOffset(doy, dow)
				Dim week As Integer = computeWeek(offset, doy)
				If week = 0 Then
					' Day is in end of week of previous year
					' Recompute from the last day of the previous year
					Dim date_Renamed As java.time.chrono.ChronoLocalDate = java.time.chrono.Chronology.from(temporal).date(temporal)
					date_Renamed = date_Renamed.minus(doy, DAYS) ' Back down into previous year
					Return localizedWeekOfWeekBasedYear(date_Renamed)
				ElseIf week > 50 Then
					' If getting close to end of year, use higher precision logic
					' Check if date of year is in partial week associated with next year
					Dim dayRange As ValueRange = temporal.range(DAY_OF_YEAR)
					Dim yearLen As Integer = CInt(dayRange.maximum)
					Dim newYearWeek As Integer = computeWeek(offset, yearLen + weekDef.minimalDaysInFirstWeek)
					If week >= newYearWeek Then week = week - newYearWeek + 1
				End If
				Return week
			End Function

			''' <summary>
			''' Returns an offset to align week start with a day of month or day of year.
			''' </summary>
			''' <param name="day">  the day; 1 through infinity </param>
			''' <param name="dow">  the day of the week of that day; 1 through 7 </param>
			''' <returns>  an offset in days to align a day with the start of the first 'full' week </returns>
			Private Function startOfWeekOffset(  day As Integer,   dow As Integer) As Integer
				' offset of first day corresponding to the day of week in first 7 days (zero origin)
				Dim weekStart As Integer = System.Math.floorMod(day - dow, 7)
				Dim offset As Integer = -weekStart
				If weekStart + 1 > weekDef.minimalDaysInFirstWeek Then offset = 7 - weekStart
				Return offset
			End Function

			''' <summary>
			''' Returns the week number computed from the reference day and reference dayOfWeek.
			''' </summary>
			''' <param name="offset"> the offset to align a date with the start of week
			'''     from <seealso cref="#startOfWeekOffset"/>. </param>
			''' <param name="day">  the day for which to compute the week number </param>
			''' <returns> the week number where zero is used for a partial week and 1 for the first full week </returns>
			Private Function computeWeek(  offset As Integer,   day As Integer) As Integer
				Return ((7 + offset + (day - 1)) \ 7)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function adjustInto(Of R As Temporal)(  temporal As R,   newValue As Long) As R Implements TemporalField.adjustInto
				' Check the new value and get the old value of the field
				Dim newVal As Integer = range_Renamed.checkValidIntValue(newValue, Me) ' lenient check range
				Dim currentVal As Integer = temporal.get(Me)
				If newVal = currentVal Then Return temporal

				If rangeUnit = FOREVER Then ' replace year of WeekBasedYear
					' Create a new date object with the same chronology,
					' the desired year and the same week and dow.
					Dim idow As Integer = temporal.get(weekDef.dayOfWeek)
					Dim wowby As Integer = temporal.get(weekDef.weekOfWeekBasedYear)
					Return CType(ofWeekBasedYear(java.time.chrono.Chronology.from(temporal), CInt(newValue), wowby, idow), R)
				Else
					' Compute the difference and add that using the base unit of the field
					Return CType(temporal.plus(newVal - currentVal, baseUnit), R)
				End If
			End Function

			Public Overrides Function resolve(  fieldValues As IDictionary(Of TemporalField, Long?),   partialTemporal As TemporalAccessor,   resolverStyle As java.time.format.ResolverStyle) As java.time.chrono.ChronoLocalDate
				Dim value As Long = fieldValues(Me)
				Dim newValue As Integer = System.Math.toIntExact(value) ' broad limit makes overflow checking lighter
				' first convert localized day-of-week to ISO day-of-week
				' doing this first handles case where both ISO and localized were parsed and might mismatch
				' day-of-week is always strict as two different day-of-week values makes lenient complex
				If rangeUnit = WEEKS Then ' day-of-week
					Dim checkedValue As Integer = range_Renamed.checkValidIntValue(value, Me) ' no leniency as too complex
					Dim startDow As Integer = weekDef.firstDayOfWeek.value
					Dim isoDow As Long = System.Math.floorMod((startDow - 1) + (checkedValue - 1), 7) + 1
					fieldValues.Remove(Me)
					fieldValues(DAY_OF_WEEK) = isoDow
					Return Nothing
				End If

				' can only build date if ISO day-of-week is present
				If fieldValues.ContainsKey(DAY_OF_WEEK) = False Then Return Nothing
				Dim isoDow As Integer = DAY_OF_WEEK.checkValidIntValue(fieldValues(DAY_OF_WEEK))
				Dim dow As Integer = localizedDayOfWeek(isoDow)

				' build date
				Dim chrono As java.time.chrono.Chronology = java.time.chrono.Chronology.from(partialTemporal)
				If fieldValues.ContainsKey(YEAR) Then
					Dim year_Renamed As Integer = YEAR.checkValidIntValue(fieldValues(YEAR)) ' validate
					If rangeUnit = MONTHS AndAlso fieldValues.ContainsKey(MONTH_OF_YEAR) Then ' week-of-month
						Dim month As Long = fieldValues(MONTH_OF_YEAR) ' not validated yet
						Return resolveWoM(fieldValues, chrono, year_Renamed, month, newValue, dow, resolverStyle)
					End If
					If rangeUnit = YEARS Then ' week-of-year Return resolveWoY(fieldValues, chrono, year_Renamed, newValue, dow, resolverStyle)
				ElseIf (rangeUnit = WEEK_BASED_YEARS OrElse rangeUnit = FOREVER) AndAlso fieldValues.ContainsKey(weekDef.weekBasedYear) AndAlso fieldValues.ContainsKey(weekDef.weekOfWeekBasedYear) Then ' week-of-week-based-year and year-of-week-based-year
					Return resolveWBY(fieldValues, chrono, dow, resolverStyle)
				End If
				Return Nothing
			End Function

			Private Function resolveWoM(  fieldValues As IDictionary(Of TemporalField, Long?),   chrono As java.time.chrono.Chronology,   year_Renamed As Integer,   month As Long,   wom As Long,   localDow As Integer,   resolverStyle As java.time.format.ResolverStyle) As java.time.chrono.ChronoLocalDate
				Dim date_Renamed As java.time.chrono.ChronoLocalDate
				If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
					date_Renamed = chrono.date(year_Renamed, 1, 1).plus (System.Math.subtractExact(month, 1), MONTHS)
					Dim weeks As Long = System.Math.subtractExact(wom, localizedWeekOfMonth(date_Renamed))
					Dim days As Integer = localDow - localizedDayOfWeek(date_Renamed) ' safe from overflow
					date_Renamed = date_Renamed.plus (System.Math.addExact (System.Math.multiplyExact(weeks, 7), days), DAYS)
				Else
					Dim monthValid As Integer = MONTH_OF_YEAR.checkValidIntValue(month) ' validate
					date_Renamed = chrono.date(year_Renamed, monthValid, 1)
					Dim womInt As Integer = range_Renamed.checkValidIntValue(wom, Me) ' validate
					Dim weeks As Integer = CInt(womInt - localizedWeekOfMonth(date_Renamed)) ' safe from overflow
					Dim days As Integer = localDow - localizedDayOfWeek(date_Renamed) ' safe from overflow
					date_Renamed = date_Renamed.plus(weeks * 7 + days, DAYS)
					If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.getLong(MONTH_OF_YEAR) IsNot month Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different month")
				End If
				fieldValues.Remove(Me)
				fieldValues.Remove(YEAR)
				fieldValues.Remove(MONTH_OF_YEAR)
				fieldValues.Remove(DAY_OF_WEEK)
				Return date_Renamed
			End Function

			Private Function resolveWoY(  fieldValues As IDictionary(Of TemporalField, Long?),   chrono As java.time.chrono.Chronology,   year_Renamed As Integer,   woy As Long,   localDow As Integer,   resolverStyle As java.time.format.ResolverStyle) As java.time.chrono.ChronoLocalDate
				Dim date_Renamed As java.time.chrono.ChronoLocalDate = chrono.date(year_Renamed, 1, 1)
				If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
					Dim weeks As Long = System.Math.subtractExact(woy, localizedWeekOfYear(date_Renamed))
					Dim days As Integer = localDow - localizedDayOfWeek(date_Renamed) ' safe from overflow
					date_Renamed = date_Renamed.plus (System.Math.addExact (System.Math.multiplyExact(weeks, 7), days), DAYS)
				Else
					Dim womInt As Integer = range_Renamed.checkValidIntValue(woy, Me) ' validate
					Dim weeks As Integer = CInt(womInt - localizedWeekOfYear(date_Renamed)) ' safe from overflow
					Dim days As Integer = localDow - localizedDayOfWeek(date_Renamed) ' safe from overflow
					date_Renamed = date_Renamed.plus(weeks * 7 + days, DAYS)
					If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.getLong(YEAR) IsNot year_Renamed Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different year")
				End If
				fieldValues.Remove(Me)
				fieldValues.Remove(YEAR)
				fieldValues.Remove(DAY_OF_WEEK)
				Return date_Renamed
			End Function

			Private Function resolveWBY(  fieldValues As IDictionary(Of TemporalField, Long?),   chrono As java.time.chrono.Chronology,   localDow As Integer,   resolverStyle As java.time.format.ResolverStyle) As java.time.chrono.ChronoLocalDate
				Dim yowby As Integer = weekDef.weekBasedYear.range().checkValidIntValue(fieldValues(weekDef.weekBasedYear), weekDef.weekBasedYear)
				Dim date_Renamed As java.time.chrono.ChronoLocalDate
				If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
					date_Renamed = ofWeekBasedYear(chrono, yowby, 1, localDow)
					Dim wowby As Long = fieldValues(weekDef.weekOfWeekBasedYear)
					Dim weeks As Long = System.Math.subtractExact(wowby, 1)
					date_Renamed = date_Renamed.plus(weeks, WEEKS)
				Else
					Dim wowby As Integer = weekDef.weekOfWeekBasedYear.range().checkValidIntValue(fieldValues(weekDef.weekOfWeekBasedYear), weekDef.weekOfWeekBasedYear) ' validate
					date_Renamed = ofWeekBasedYear(chrono, yowby, wowby, localDow)
					If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso localizedWeekBasedYear(date_Renamed) <> yowby Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different week-based-year")
				End If
				fieldValues.Remove(Me)
				fieldValues.Remove(weekDef.weekBasedYear)
				fieldValues.Remove(weekDef.weekOfWeekBasedYear)
				fieldValues.Remove(DAY_OF_WEEK)
				Return date_Renamed
			End Function

			'-----------------------------------------------------------------------
			Public Overrides Function getDisplayName(  locale As java.util.Locale) As String Implements TemporalField.getDisplayName
				java.util.Objects.requireNonNull(locale, "locale")
				If rangeUnit = YEARS Then ' only have values for week-of-year
					Dim lr As sun.util.locale.provider.LocaleResources = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased.getLocaleResources(locale)
					Dim rb As java.util.ResourceBundle = lr.javaTimeFormatData
					Return If(rb.containsKey("field.week"), rb.getString("field.week"), name)
				End If
				Return name
			End Function

			Public  Overrides ReadOnly Property  baseUnit As TemporalUnit Implements TemporalField.getBaseUnit
				Get
					Return baseUnit
				End Get
			End Property

			Public  Overrides ReadOnly Property  rangeUnit As TemporalUnit Implements TemporalField.getRangeUnit
				Get
					Return rangeUnit
				End Get
			End Property

			Public  Overrides ReadOnly Property  dateBased As Boolean Implements TemporalField.isDateBased
				Get
					Return True
				End Get
			End Property

			Public  Overrides ReadOnly Property  timeBased As Boolean Implements TemporalField.isTimeBased
				Get
					Return False
				End Get
			End Property

			Public Overrides Function range() As ValueRange Implements TemporalField.range
				Return range_Renamed
			End Function

			'-----------------------------------------------------------------------
			Public Overrides Function isSupportedBy(  temporal As TemporalAccessor) As Boolean Implements TemporalField.isSupportedBy
				If temporal.isSupported(DAY_OF_WEEK) Then
					If rangeUnit = WEEKS Then ' day-of-week
						Return True ' week-of-month
					ElseIf rangeUnit = MONTHS Then
						Return temporal.isSupported(DAY_OF_MONTH) ' week-of-year
					ElseIf rangeUnit = YEARS Then
						Return temporal.isSupported(DAY_OF_YEAR)
					ElseIf rangeUnit = WEEK_BASED_YEARS Then
						Return temporal.isSupported(DAY_OF_YEAR)
					ElseIf rangeUnit = FOREVER Then
						Return temporal.isSupported(YEAR)
					End If
				End If
				Return False
			End Function

			Public Overrides Function rangeRefinedBy(  temporal As TemporalAccessor) As ValueRange Implements TemporalField.rangeRefinedBy
				If rangeUnit = ChronoUnit.WEEKS Then ' day-of-week
					Return range_Renamed ' week-of-month
				ElseIf rangeUnit = MONTHS Then
					Return rangeByWeek(temporal, DAY_OF_MONTH) ' week-of-year
				ElseIf rangeUnit = YEARS Then
					Return rangeByWeek(temporal, DAY_OF_YEAR)
				ElseIf rangeUnit = WEEK_BASED_YEARS Then
					Return rangeWeekOfWeekBasedYear(temporal)
				ElseIf rangeUnit = FOREVER Then
					Return YEAR.range()
				Else
					Throw New IllegalStateException("unreachable, rangeUnit: " & rangeUnit & ", this: " & Me)
				End If
			End Function

			''' <summary>
			''' Map the field range to a week range </summary>
			''' <param name="temporal"> the temporal </param>
			''' <param name="field"> the field to get the range of </param>
			''' <returns> the ValueRange with the range adjusted to weeks. </returns>
			Private Function rangeByWeek(  temporal As TemporalAccessor,   field As TemporalField) As ValueRange
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim offset As Integer = startOfWeekOffset(temporal.get(field), dow)
				Dim fieldRange As ValueRange = temporal.range(field)
				Return ValueRange.of(computeWeek(offset, CInt(fieldRange.minimum)), computeWeek(offset, CInt(fieldRange.maximum)))
			End Function

			''' <summary>
			''' Map the field range to a week range of a week year. </summary>
			''' <param name="temporal">  the temporal </param>
			''' <returns> the ValueRange with the range adjusted to weeks. </returns>
			Private Function rangeWeekOfWeekBasedYear(  temporal As TemporalAccessor) As ValueRange
				If Not temporal.isSupported(DAY_OF_YEAR) Then Return WEEK_OF_YEAR_RANGE
				Dim dow As Integer = localizedDayOfWeek(temporal)
				Dim doy As Integer = temporal.get(DAY_OF_YEAR)
				Dim offset As Integer = startOfWeekOffset(doy, dow)
				Dim week As Integer = computeWeek(offset, doy)
				If week = 0 Then
					' Day is in end of week of previous year
					' Recompute from the last day of the previous year
					Dim date_Renamed As java.time.chrono.ChronoLocalDate = java.time.chrono.Chronology.from(temporal).date(temporal)
					date_Renamed = date_Renamed.minus(doy + 7, DAYS) ' Back down into previous year
					Return rangeWeekOfWeekBasedYear(date_Renamed)
				End If
				' Check if day of year is in partial week associated with next year
				Dim dayRange As ValueRange = temporal.range(DAY_OF_YEAR)
				Dim yearLen As Integer = CInt(dayRange.maximum)
				Dim newYearWeek As Integer = computeWeek(offset, yearLen + weekDef.minimalDaysInFirstWeek)

				If week >= newYearWeek Then
					' Overlaps with weeks of following year; recompute from a week in following year
					Dim date_Renamed As java.time.chrono.ChronoLocalDate = java.time.chrono.Chronology.from(temporal).date(temporal)
					date_Renamed = date_Renamed.plus(yearLen - doy + 1 + 7, ChronoUnit.DAYS)
					Return rangeWeekOfWeekBasedYear(date_Renamed)
				End If
				Return ValueRange.of(1, newYearWeek-1)
			End Function

			'-----------------------------------------------------------------------
			Public Overrides Function ToString() As String
				Return name & "[" & weekDef.ToString() & "]"
			End Function
		End Class
	End Class

End Namespace