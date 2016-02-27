Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util


	''' <summary>
	''' <code>JapaneseImperialCalendar</code> implements a Japanese
	''' calendar system in which the imperial era-based year numbering is
	''' supported from the Meiji era. The following are the eras supported
	''' by this calendar system.
	''' <pre><tt>
	''' ERA value   Era name    Since (in Gregorian)
	''' ------------------------------------------------------
	'''     0       N/A         N/A
	'''     1       Meiji       1868-01-01 midnight local time
	'''     2       Taisho      1912-07-30 midnight local time
	'''     3       Showa       1926-12-25 midnight local time
	'''     4       Heisei      1989-01-08 midnight local time
	''' ------------------------------------------------------
	''' </tt></pre>
	''' 
	''' <p><code>ERA</code> value 0 specifies the years before Meiji and
	''' the Gregorian year values are used. Unlike {@link
	''' GregorianCalendar}, the Julian to Gregorian transition is not
	''' supported because it doesn't make any sense to the Japanese
	''' calendar systems used before Meiji. To represent the years before
	''' Gregorian year 1, 0 and negative values are used. The Japanese
	''' Imperial rescripts and government decrees don't specify how to deal
	''' with time differences for applying the era transitions. This
	''' calendar implementation assumes local time for all transitions.
	''' 
	''' @author Masayoshi Okutsu
	''' @since 1.6
	''' </summary>
	Friend Class JapaneseImperialCalendar
		Inherits Calendar

	'    
	'     * Implementation Notes
	'     *
	'     * This implementation uses
	'     * sun.util.calendar.LocalGregorianCalendar to perform most of the
	'     * calendar calculations. LocalGregorianCalendar is configurable
	'     * and reads <JRE_HOME>/lib/calendars.properties at the start-up.
	'     

		''' <summary>
		''' The ERA constant designating the era before Meiji.
		''' </summary>
		Public Const BEFORE_MEIJI As Integer = 0

		''' <summary>
		''' The ERA constant designating the Meiji era.
		''' </summary>
		Public Const MEIJI As Integer = 1

		''' <summary>
		''' The ERA constant designating the Taisho era.
		''' </summary>
		Public Const TAISHO As Integer = 2

		''' <summary>
		''' The ERA constant designating the Showa era.
		''' </summary>
		Public Const SHOWA As Integer = 3

		''' <summary>
		''' The ERA constant designating the Heisei era.
		''' </summary>
		Public Const HEISEI As Integer = 4

		Private Const EPOCH_OFFSET As Integer = 719163 ' Fixed date of January 1, 1970 (Gregorian)
		Private Const EPOCH_YEAR As Integer = 1970

		' Useful millisecond constants.  Although ONE_DAY and ONE_WEEK can fit
		' into ints, they must be longs in order to prevent arithmetic overflow
		' when performing (bug 4173516).
		Private Const ONE_SECOND As Integer = 1000
		Private Shared ReadOnly ONE_MINUTE As Integer = 60*ONE_SECOND
		Private Shared ReadOnly ONE_HOUR As Integer = 60*ONE_MINUTE
		Private Shared ReadOnly ONE_DAY As Long = 24*ONE_HOUR
		Private Shared ReadOnly ONE_WEEK As Long = 7*ONE_DAY

		' Reference to the sun.util.calendar.LocalGregorianCalendar instance (singleton).
		Private Shared ReadOnly jcal As sun.util.calendar.LocalGregorianCalendar = CType(sun.util.calendar.CalendarSystem.forName("japanese"), sun.util.calendar.LocalGregorianCalendar)

		' Gregorian calendar instance. This is required because era
		' transition dates are given in Gregorian dates.
		Private Shared ReadOnly gcal As sun.util.calendar.Gregorian = sun.util.calendar.CalendarSystem.gregorianCalendar

		' The Era instance representing "before Meiji".
		Private Shared ReadOnly BEFORE_MEIJI_ERA As New sun.util.calendar.Era("BeforeMeiji", "BM", java.lang.[Long].MIN_VALUE, False)

		' Imperial eras. The sun.util.calendar.LocalGregorianCalendar
		' doesn't have an Era representing before Meiji, which is
		' inconvenient for a Calendar. So, era[0] is a reference to
		' BEFORE_MEIJI_ERA.
		Private Shared ReadOnly eras As sun.util.calendar.Era()

		' Fixed date of the first date of each era.
		Private Shared ReadOnly sinceFixedDates As Long()

	'    
	'     * <pre>
	'     *                                 Greatest       Least
	'     * Field name             Minimum   Minimum     Maximum     Maximum
	'     * ----------             -------   -------     -------     -------
	'     * ERA                          0         0           1           1
	'     * YEAR                -292275055         1           ?           ?
	'     * MONTH                        0         0          11          11
	'     * WEEK_OF_YEAR                 1         1          52*         53
	'     * WEEK_OF_MONTH                0         0           4*          6
	'     * DAY_OF_MONTH                 1         1          28*         31
	'     * DAY_OF_YEAR                  1         1         365*        366
	'     * DAY_OF_WEEK                  1         1           7           7
	'     * DAY_OF_WEEK_IN_MONTH        -1        -1           4*          6
	'     * AM_PM                        0         0           1           1
	'     * HOUR                         0         0          11          11
	'     * HOUR_OF_DAY                  0         0          23          23
	'     * MINUTE                       0         0          59          59
	'     * SECOND                       0         0          59          59
	'     * MILLISECOND                  0         0         999         999
	'     * ZONE_OFFSET             -13:00    -13:00       14:00       14:00
	'     * DST_OFFSET                0:00      0:00        0:20        2:00
	'     * </pre>
	'     * *: depends on eras
	'     
		Friend Shared ReadOnly MIN_VALUES As Integer() = { 0, -292275055, JANUARY, 1, 0, 1, 1, SUNDAY, 1, AM, 0, 0, 0, 0, 0, -13*ONE_HOUR, 0 }
		Friend Shared ReadOnly LEAST_MAX_VALUES As Integer() = { 0, 0, JANUARY, 0, 4, 28, 0, SATURDAY, 4, PM, 11, 23, 59, 59, 999, 14*ONE_HOUR, 20*ONE_MINUTE }
		Friend Shared ReadOnly MAX_VALUES As Integer() = { 0, 292278994, DECEMBER, 53, 6, 31, 366, SATURDAY, 6, PM, 11, 23, 59, 59, 999, 14*ONE_HOUR, 2*ONE_HOUR }

		' Proclaim serialization compatibility with JDK 1.6
		Private Shadows Const serialVersionUID As Long = -3364572813905467929L

		Shared Sub New()
			Dim es As sun.util.calendar.Era() = jcal.eras
			Dim length As Integer = es.Length + 1
			eras = New sun.util.calendar.Era(length - 1){}
			sinceFixedDates = New Long(length - 1){}

			' eras[BEFORE_MEIJI] and sinceFixedDate[BEFORE_MEIJI] are the
			' same as Gregorian.
			Dim index As Integer = BEFORE_MEIJI
			sinceFixedDates(index) = gcal.getFixedDate(BEFORE_MEIJI_ERA.sinceDate)
			eras(index) = BEFORE_MEIJI_ERA
			index += 1
			For Each e As sun.util.calendar.Era In es
				Dim d As sun.util.calendar.CalendarDate = e.sinceDate
				sinceFixedDates(index) = gcal.getFixedDate(d)
				eras(index) = e
				index += 1
			Next e

				MAX_VALUES(ERA) = eras.Length - 1
				LEAST_MAX_VALUES(ERA) = MAX_VALUES(ERA)

			' Calculate the least maximum year and least day of Year
			' values. The following code assumes that there's at most one
			' era transition in a Gregorian year.
			Dim year_Renamed As Integer =  java.lang.[Integer].Max_Value
			Dim dayOfYear As Integer =  java.lang.[Integer].Max_Value
			Dim date_Renamed As sun.util.calendar.CalendarDate = gcal.newCalendarDate(TimeZone.NO_TIMEZONE)
			For i As Integer = 1 To eras.Length - 1
				Dim fd As Long = sinceFixedDates(i)
				Dim transitionDate As sun.util.calendar.CalendarDate = eras(i).sinceDate
				date_Renamed.dateate(transitionDate.year, sun.util.calendar.BaseCalendar.JANUARY, 1)
				Dim fdd As Long = gcal.getFixedDate(date_Renamed)
				If fd <> fdd Then dayOfYear = System.Math.Min(CInt(fd - fdd) + 1, dayOfYear)
				date_Renamed.dateate(transitionDate.year, sun.util.calendar.BaseCalendar.DECEMBER, 31)
				fdd = gcal.getFixedDate(date_Renamed)
				If fd <> fdd Then dayOfYear = System.Math.Min(CInt(fdd - fd) + 1, dayOfYear)
				Dim lgd As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(fd - 1)
				Dim y As Integer = lgd.year
				' Unless the first year starts from January 1, the actual
				' max value could be one year  java.lang.[Short]. For example, if it's
				' Showa 63 January 8, 63 is the actual max value since
				' Showa 64 January 8 doesn't exist.
				If Not(lgd.month = sun.util.calendar.BaseCalendar.JANUARY AndAlso lgd.dayOfMonth = 1) Then y -= 1
				year_Renamed = System.Math.Min(y, year_Renamed)
			Next i
			LEAST_MAX_VALUES(YEAR) = year_Renamed ' Max year could be smaller than this value.
			LEAST_MAX_VALUES(DAY_OF_YEAR) = dayOfYear
		End Sub

		''' <summary>
		''' jdate always has a sun.util.calendar.LocalGregorianCalendar.Date instance to
		''' avoid overhead of creating it for each calculation.
		''' </summary>
		<NonSerialized> _
		Private jdate As sun.util.calendar.LocalGregorianCalendar.Date

		''' <summary>
		''' Temporary int[2] to get time zone offsets. zoneOffsets[0] gets
		''' the GMT offset value and zoneOffsets[1] gets the daylight saving
		''' value.
		''' </summary>
		<NonSerialized> _
		Private zoneOffsets As Integer()

		''' <summary>
		''' Temporary storage for saving original fields[] values in
		''' non-lenient mode.
		''' </summary>
		<NonSerialized> _
		Private originalFields As Integer()

		''' <summary>
		''' Constructs a <code>JapaneseImperialCalendar</code> based on the current time
		''' in the given time zone with the given locale.
		''' </summary>
		''' <param name="zone"> the given time zone. </param>
		''' <param name="aLocale"> the given locale. </param>
		Friend Sub New(ByVal zone As TimeZone, ByVal aLocale As Locale)
			MyBase.New(zone, aLocale)
			jdate = jcal.newCalendarDate(zone)
			timeInMillis = System.currentTimeMillis()
		End Sub

		''' <summary>
		''' Constructs an "empty" {@code JapaneseImperialCalendar}.
		''' </summary>
		''' <param name="zone">    the given time zone </param>
		''' <param name="aLocale"> the given locale </param>
		''' <param name="flag">    the flag requesting an empty instance </param>
		Friend Sub New(ByVal zone As TimeZone, ByVal aLocale As Locale, ByVal flag As Boolean)
			MyBase.New(zone, aLocale)
			jdate = jcal.newCalendarDate(zone)
		End Sub

		''' <summary>
		''' Returns {@code "japanese"} as the calendar type of this {@code
		''' JapaneseImperialCalendar}.
		''' </summary>
		''' <returns> {@code "japanese"} </returns>
		Public Property Overrides calendarType As String
			Get
				Return "japanese"
			End Get
		End Property

		''' <summary>
		''' Compares this <code>JapaneseImperialCalendar</code> to the specified
		''' <code>Object</code>. The result is <code>true</code> if and
		''' only if the argument is a <code>JapaneseImperialCalendar</code> object
		''' that represents the same time value (millisecond offset from
		''' the <a href="Calendar.html#Epoch">Epoch</a>) under the same
		''' <code>Calendar</code> parameters.
		''' </summary>
		''' <param name="obj"> the object to compare with. </param>
		''' <returns> <code>true</code> if this object is equal to <code>obj</code>;
		''' <code>false</code> otherwise. </returns>
		''' <seealso cref= Calendar#compareTo(Calendar) </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return TypeOf obj Is JapaneseImperialCalendar AndAlso MyBase.Equals(obj)
		End Function

		''' <summary>
		''' Generates the hash code for this
		''' <code>JapaneseImperialCalendar</code> object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode() Xor jdate.GetHashCode()
		End Function

		''' <summary>
		''' Adds the specified (signed) amount of time to the given calendar field,
		''' based on the calendar's rules.
		''' 
		''' <p><em>Add rule 1</em>. The value of <code>field</code>
		''' after the call minus the value of <code>field</code> before the
		''' call is <code>amount</code>, modulo any overflow that has occurred in
		''' <code>field</code>. Overflow occurs when a field value exceeds its
		''' range and, as a result, the next larger field is incremented or
		''' decremented and the field value is adjusted back into its range.</p>
		''' 
		''' <p><em>Add rule 2</em>. If a smaller field is expected to be
		''' invariant, but it is impossible for it to be equal to its
		''' prior value because of changes in its minimum or maximum after
		''' <code>field</code> is changed, then its value is adjusted to be as close
		''' as possible to its expected value. A smaller field represents a
		''' smaller unit of time. <code>HOUR</code> is a smaller field than
		''' <code>DAY_OF_MONTH</code>. No adjustment is made to smaller fields
		''' that are not expected to be invariant. The calendar system
		''' determines what fields are expected to be invariant.</p>
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <param name="amount"> the amount of date or time to be added to the field. </param>
		''' <exception cref="IllegalArgumentException"> if <code>field</code> is
		''' <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		''' or if any calendar fields have out-of-range values in
		''' non-lenient mode. </exception>
		Public Overrides Sub add(ByVal field As Integer, ByVal amount As Integer)
			' If amount == 0, do nothing even the given field is out of
			' range. This is tested by JCK.
			If amount = 0 Then Return ' Do nothing!

			If field < 0 OrElse field >= ZONE_OFFSET Then Throw New IllegalArgumentException

			' Sync the time and calendar fields.
			complete()

			If field = YEAR Then
				Dim d As sun.util.calendar.LocalGregorianCalendar.Date = CType(jdate.clone(), sun.util.calendar.LocalGregorianCalendar.Date)
				d.addYear(amount)
				pinDayOfMonth(d)
				[set](ERA, getEraIndex(d))
				[set](YEAR, d.year)
				[set](MONTH, d.month - 1)
				[set](DAY_OF_MONTH, d.dayOfMonth)
			ElseIf field = MONTH Then
				Dim d As sun.util.calendar.LocalGregorianCalendar.Date = CType(jdate.clone(), sun.util.calendar.LocalGregorianCalendar.Date)
				d.addMonth(amount)
				pinDayOfMonth(d)
				[set](ERA, getEraIndex(d))
				[set](YEAR, d.year)
				[set](MONTH, d.month - 1)
				[set](DAY_OF_MONTH, d.dayOfMonth)
			ElseIf field = ERA Then
				Dim era_Renamed As Integer = internalGet(ERA) + amount
				If era_Renamed < 0 Then
					era_Renamed = 0
				ElseIf era_Renamed > eras.Length - 1 Then
					era_Renamed = eras.Length - 1
				End If
				[set](ERA, era_Renamed)
			Else
				Dim delta As Long = amount
				Dim timeOfDay As Long = 0
				Select Case field
				' Handle the time fields here. Convert the given
				' amount to milliseconds and call setTimeInMillis.
				Case HOUR, HOUR_OF_DAY
					delta *= 60 * 60 * 1000 ' hours to milliseconds

				Case MINUTE
					delta *= 60 * 1000 ' minutes to milliseconds

				Case SECOND
					delta *= 1000 ' seconds to milliseconds

				Case MILLISECOND

				' Handle week, day and AM_PM fields which involves
				' time zone offset change adjustment. Convert the
				' given amount to the number of days.
				Case WEEK_OF_YEAR, WEEK_OF_MONTH, DAY_OF_WEEK_IN_MONTH
					delta *= 7

				Case DAY_OF_MONTH, DAY_OF_YEAR, DAY_OF_WEEK ' synonym of DATE

				Case AM_PM
					' Convert the amount to the number of days (delta)
					' and +12 or -12 hours (timeOfDay).
					delta = amount \ 2
					timeOfDay = 12 * (amount Mod 2)
				End Select

				' The time fields don't require time zone offset change
				' adjustment.
				If field >= HOUR Then
					timeInMillis = time + delta
					Return
				End If

				' The rest of the fields (week, day or AM_PM fields)
				' require time zone offset (both GMT and DST) change
				' adjustment.

				' Translate the current time to the fixed date and time
				' of the day.
				Dim fd As Long = cachedFixedDate
				timeOfDay += internalGet(HOUR_OF_DAY)
				timeOfDay *= 60
				timeOfDay += internalGet(MINUTE)
				timeOfDay *= 60
				timeOfDay += internalGet(SECOND)
				timeOfDay *= 1000
				timeOfDay += internalGet(MILLISECOND)
				If timeOfDay >= ONE_DAY Then
					fd += 1
					timeOfDay -= ONE_DAY
				ElseIf timeOfDay < 0 Then
					fd -= 1
					timeOfDay += ONE_DAY
				End If

				fd += delta ' fd is the expected fixed date after the calculation
				Dim zoneOffset As Integer = internalGet(ZONE_OFFSET) + internalGet(DST_OFFSET)
				timeInMillis = (fd - EPOCH_OFFSET) * ONE_DAY + timeOfDay - zoneOffset
				zoneOffset -= internalGet(ZONE_OFFSET) + internalGet(DST_OFFSET)
				' If the time zone offset has changed, then adjust the difference.
				If zoneOffset <> 0 Then
					timeInMillis = time + zoneOffset
					Dim fd2 As Long = cachedFixedDate
					' If the adjustment has changed the date, then take
					' the previous one.
					If fd2 <> fd Then timeInMillis = time - zoneOffset
				End If
			End If
		End Sub

		Public Overrides Sub roll(ByVal field As Integer, ByVal up As Boolean)
			roll(field,If(up, +1, -1))
		End Sub

		''' <summary>
		''' Adds a signed amount to the specified calendar field without changing larger fields.
		''' A negative roll amount means to subtract from field without changing
		''' larger fields. If the specified amount is 0, this method performs nothing.
		''' 
		''' <p>This method calls <seealso cref="#complete()"/> before adding the
		''' amount so that all the calendar fields are normalized. If there
		''' is any calendar field having an out-of-range value in non-lenient mode, then an
		''' <code>IllegalArgumentException</code> is thrown.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <param name="amount"> the signed amount to add to <code>field</code>. </param>
		''' <exception cref="IllegalArgumentException"> if <code>field</code> is
		''' <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		''' or if any calendar fields have out-of-range values in
		''' non-lenient mode. </exception>
		''' <seealso cref= #roll(int,boolean) </seealso>
		''' <seealso cref= #add(int,int) </seealso>
		''' <seealso cref= #set(int,int) </seealso>
		Public Overrides Sub roll(ByVal field As Integer, ByVal amount As Integer)
			' If amount == 0, do nothing even the given field is out of
			' range. This is tested by JCK.
			If amount = 0 Then Return

			If field < 0 OrElse field >= ZONE_OFFSET Then Throw New IllegalArgumentException

			' Sync the time and calendar fields.
			complete()

			Dim min As Integer = getMinimum(field)
			Dim max As Integer = getMaximum(field)

			Select Case field
			Case ERA, AM_PM, MINUTE, SECOND, MILLISECOND
				' These fields are handled simply, since they have fixed
				' minima and maxima. Other fields are complicated, since
				' the range within they must roll varies depending on the
				' date, a time zone and the era transitions.

			Case HOUR, HOUR_OF_DAY
					Dim unit As Integer = max + 1 ' 12 or 24 hours
					Dim h As Integer = internalGet(field)
					Dim nh As Integer = (h + amount) Mod unit
					If nh < 0 Then nh += unit
					time += ONE_HOUR * (nh - h)

					' The day might have changed, which could happen if
					' the daylight saving time transition brings it to
					' the next day, although it's very unlikely. But we
					' have to make sure not to change the larger fields.
					Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(time, zone)
					If internalGet(DAY_OF_MONTH) <> d.dayOfMonth Then
						d.era = jdate.era
						d.dateate(internalGet(YEAR), internalGet(MONTH) + 1, internalGet(DAY_OF_MONTH))
						If field = HOUR Then
							assert(internalGet(AM_PM) = PM)
							d.addHours(+12) ' restore PM
						End If
						time = jcal.getTime(d)
					End If
					Dim hourOfDay As Integer = d.hours
					internalSet(field, hourOfDay Mod unit)
					If field = HOUR Then
						internalSet(HOUR_OF_DAY, hourOfDay)
					Else
						internalSet(AM_PM, hourOfDay \ 12)
						internalSet(HOUR, hourOfDay Mod 12)
					End If

					' Time zone offset and/or daylight saving might have changed.
					Dim zoneOffset As Integer = d.zoneOffset
					Dim saving As Integer = d.daylightSaving
					internalSet(ZONE_OFFSET, zoneOffset - saving)
					internalSet(DST_OFFSET, saving)
					Return

			Case YEAR
				min = getActualMinimum(field)
				max = getActualMaximum(field)

			Case MONTH
				' Rolling the month involves both pinning the final value to [0, 11]
				' and adjusting the DAY_OF_MONTH if necessary.  We only adjust the
				' DAY_OF_MONTH if, after updating the MONTH field, it is illegal.
				' E.g., <jan31>.roll(MONTH, 1) -> <feb28> or <feb29>.
					If Not isTransitionYear(jdate.normalizedYear) Then
						Dim year_Renamed As Integer = jdate.year
						If year_Renamed = getMaximum(YEAR) Then
							Dim jd As sun.util.calendar.CalendarDate = jcal.getCalendarDate(time, zone)
							Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.Max_Value, zone)
							max = d.month - 1
							Dim n As Integer = getRolledValue(internalGet(field), amount, min, max)
							If n = max Then
								' To avoid overflow, use an equivalent year.
								jd.addYear(-400)
								jd.month = n + 1
								If jd.dayOfMonth > d.dayOfMonth Then
									jd.dayOfMonth = d.dayOfMonth
									jcal.normalize(jd)
								End If
								If jd.dayOfMonth = d.dayOfMonth AndAlso jd.timeOfDay > d.timeOfDay Then
									jd.month = n + 1
									jd.dayOfMonth = d.dayOfMonth - 1
									jcal.normalize(jd)
									' Month may have changed by the normalization.
									n = jd.month - 1
								End If
								[set](DAY_OF_MONTH, jd.dayOfMonth)
							End If
							[set](MONTH, n)
						ElseIf year_Renamed = getMinimum(YEAR) Then
							Dim jd As sun.util.calendar.CalendarDate = jcal.getCalendarDate(time, zone)
							Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
							min = d.month - 1
							Dim n As Integer = getRolledValue(internalGet(field), amount, min, max)
							If n = min Then
								' To avoid underflow, use an equivalent year.
								jd.addYear(+400)
								jd.month = n + 1
								If jd.dayOfMonth < d.dayOfMonth Then
									jd.dayOfMonth = d.dayOfMonth
									jcal.normalize(jd)
								End If
								If jd.dayOfMonth = d.dayOfMonth AndAlso jd.timeOfDay < d.timeOfDay Then
									jd.month = n + 1
									jd.dayOfMonth = d.dayOfMonth + 1
									jcal.normalize(jd)
									' Month may have changed by the normalization.
									n = jd.month - 1
								End If
								[set](DAY_OF_MONTH, jd.dayOfMonth)
							End If
							[set](MONTH, n)
						Else
							Dim mon As Integer = (internalGet(MONTH) + amount) Mod 12
							If mon < 0 Then mon += 12
							[set](MONTH, mon)

							' Keep the day of month in the range.  We
							' don't want to spill over into the next
							' month; e.g., we don't want jan31 + 1 mo ->
							' feb31 -> mar3.
							Dim monthLen As Integer = monthLength(mon)
							If internalGet(DAY_OF_MONTH) > monthLen Then [set](DAY_OF_MONTH, monthLen)
						End If
					Else
						Dim eraIndex_Renamed As Integer = getEraIndex(jdate)
						Dim transition As sun.util.calendar.CalendarDate = Nothing
						If jdate.year = 1 Then
							transition = eras(eraIndex_Renamed).sinceDate
							min = transition.month - 1
						Else
							If eraIndex_Renamed < eras.Length - 1 Then
								transition = eras(eraIndex_Renamed + 1).sinceDate
								If transition.year = jdate.normalizedYear Then
									max = transition.month - 1
									If transition.dayOfMonth = 1 Then max -= 1
								End If
							End If
						End If

						If min = max Then Return
						Dim n As Integer = getRolledValue(internalGet(field), amount, min, max)
						[set](MONTH, n)
						If n = min Then
							If Not(transition.month = sun.util.calendar.BaseCalendar.JANUARY AndAlso transition.dayOfMonth = 1) Then
								If jdate.dayOfMonth < transition.dayOfMonth Then [set](DAY_OF_MONTH, transition.dayOfMonth)
							End If
						ElseIf n = max AndAlso (transition.month - 1 = n) Then
							Dim dom As Integer = transition.dayOfMonth
							If jdate.dayOfMonth >= dom Then [set](DAY_OF_MONTH, dom - 1)
						End If
					End If
					Return

			Case WEEK_OF_YEAR
					Dim y As Integer = jdate.normalizedYear
					max = getActualMaximum(WEEK_OF_YEAR)
					[set](DAY_OF_WEEK, internalGet(DAY_OF_WEEK)) ' update stamp[field]
					Dim woy As Integer = internalGet(WEEK_OF_YEAR)
					Dim value As Integer = woy + amount
					If Not isTransitionYear(jdate.normalizedYear) Then
						Dim year_Renamed As Integer = jdate.year
						If year_Renamed = getMaximum(YEAR) Then
							max = getActualMaximum(WEEK_OF_YEAR)
						ElseIf year_Renamed = getMinimum(YEAR) Then
							min = getActualMinimum(WEEK_OF_YEAR)
							max = getActualMaximum(WEEK_OF_YEAR)
							If value > min AndAlso value < max Then
								[set](WEEK_OF_YEAR, value)
								Return
							End If

						End If
						' If the new value is in between min and max
						' (exclusive), then we can use the value.
						If value > min AndAlso value < max Then
							[set](WEEK_OF_YEAR, value)
							Return
						End If
						Dim fd As Long = cachedFixedDate
						' Make sure that the min week has the current DAY_OF_WEEK
						Dim day1 As Long = fd - (7 * (woy - min))
						If year_Renamed <> getMinimum(YEAR) Then
							If gcal.getYearFromFixedDate(day1) <> y Then min += 1
						Else
							Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
							If day1 < jcal.getFixedDate(d) Then min += 1
						End If

						' Make sure the same thing for the max week
						fd += 7 * (max - internalGet(WEEK_OF_YEAR))
						If gcal.getYearFromFixedDate(fd) <> y Then max -= 1
						Exit Select
					End If

					' Handle transition here.
					Dim fd As Long = cachedFixedDate
					Dim day1 As Long = fd - (7 * (woy - min))
					' Make sure that the min week has the current DAY_OF_WEEK
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(day1)
					If Not(d.era = jdate.era AndAlso d.year = jdate.year) Then min += 1

					' Make sure the same thing for the max week
					fd += 7 * (max - woy)
					jcal.getCalendarDateFromFixedDate(d, fd)
					If Not(d.era = jdate.era AndAlso d.year = jdate.year) Then max -= 1
					' value: the new WEEK_OF_YEAR which must be converted
					' to month and day of month.
					value = getRolledValue(woy, amount, min, max) - 1
					d = getCalendarDate(day1 + value * 7)
					[set](MONTH, d.month - 1)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case WEEK_OF_MONTH
					Dim isTransitionYear As Boolean = isTransitionYear(jdate.normalizedYear)
					' dow: relative day of week from the first day of week
					Dim dow As Integer = internalGet(DAY_OF_WEEK) - firstDayOfWeek
					If dow < 0 Then dow += 7

					Dim fd As Long = cachedFixedDate
					Dim month1 As Long ' fixed date of the first day (usually 1) of the month
					Dim monthLength As Integer ' actual month length
					If isTransitionYear Then
						month1 = getFixedDateMonth1(jdate, fd)
						monthLength = actualMonthLength()
					Else
						month1 = fd - internalGet(DAY_OF_MONTH) + 1
						monthLength = jcal.getMonthLength(jdate)
					End If

					' the first day of week of the month.
					Dim monthDay1st As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(month1 + 6, firstDayOfWeek)
					' if the week has enough days to form a week, the
					' week starts from the previous month.
					If CInt(monthDay1st - month1) >= minimalDaysInFirstWeek Then monthDay1st -= 7
					max = getActualMaximum(field)

					' value: the new WEEK_OF_MONTH value
					Dim value As Integer = getRolledValue(internalGet(field), amount, 1, max) - 1

					' nfd: fixed date of the rolled date
					Dim nfd As Long = monthDay1st + value * 7 + dow

					' Unlike WEEK_OF_YEAR, we need to change day of week if the
					' nfd is out of the month.
					If nfd < month1 Then
						nfd = month1
					ElseIf nfd >= (month1 + monthLength) Then
						nfd = month1 + monthLength - 1
					End If
					[set](DAY_OF_MONTH, CInt(nfd - month1) + 1)
					Return

			Case DAY_OF_MONTH
					If Not isTransitionYear(jdate.normalizedYear) Then
						max = jcal.getMonthLength(jdate)
						Exit Select
					End If

					' TODO: Need to change the spec to be usable DAY_OF_MONTH rolling...

					' Transition handling. We can't change year and era
					' values here due to the Calendar roll spec!
					Dim month1 As Long = getFixedDateMonth1(jdate, cachedFixedDate)

					' It may not be a regular month. Convert the date and range to
					' the relative values, perform the roll, and
					' convert the result back to the rolled date.
					Dim value As Integer = getRolledValue(CInt(cachedFixedDate - month1), amount, 0, actualMonthLength() - 1)
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(month1 + value)
					Debug.Assert(getEraIndex(d) = internalGetEra() AndAlso d.year = internalGet(YEAR) AndAlso d.month-1 = internalGet(MONTH))
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case DAY_OF_YEAR
					max = getActualMaximum(field)
					If Not isTransitionYear(jdate.normalizedYear) Then Exit Select

					' Handle transition. We can't change year and era values
					' here due to the Calendar roll spec.
					Dim value As Integer = getRolledValue(internalGet(DAY_OF_YEAR), amount, min, max)
					Dim jan0 As Long = cachedFixedDate - internalGet(DAY_OF_YEAR)
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(jan0 + value)
					Debug.Assert(getEraIndex(d) = internalGetEra() AndAlso d.year = internalGet(YEAR))
					[set](MONTH, d.month - 1)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case DAY_OF_WEEK
					Dim normalizedYear As Integer = jdate.normalizedYear
					If (Not isTransitionYear(normalizedYear)) AndAlso (Not isTransitionYear(normalizedYear - 1)) Then
						' If the week of year is in the same year, we can
						' just change DAY_OF_WEEK.
						Dim weekOfYear As Integer = internalGet(WEEK_OF_YEAR)
						If weekOfYear > 1 AndAlso weekOfYear < 52 Then
							[set](WEEK_OF_YEAR, internalGet(WEEK_OF_YEAR))
							max = SATURDAY
							Exit Select
						End If
					End If

					' We need to handle it in a different way around year
					' boundaries and in the transition year. Note that
					' changing era and year values violates the roll
					' rule: not changing larger calendar fields...
					amount = amount Mod 7
					If amount = 0 Then Return
					Dim fd As Long = cachedFixedDate
					Dim dowFirst As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fd, firstDayOfWeek)
					fd += amount
					If fd < dowFirst Then
						fd += 7
					ElseIf fd >= dowFirst + 7 Then
						fd -= 7
					End If
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(fd)
					[set](ERA, getEraIndex(d))
					[set](d.year, d.month - 1, d.dayOfMonth)
					Return

			Case DAY_OF_WEEK_IN_MONTH
					min = 1 ' after having normalized, min should be 1.
					If Not isTransitionYear(jdate.normalizedYear) Then
						Dim dom As Integer = internalGet(DAY_OF_MONTH)
						Dim monthLength As Integer = jcal.getMonthLength(jdate)
						Dim lastDays As Integer = monthLength Mod 7
						max = monthLength \ 7
						Dim x As Integer = (dom - 1) Mod 7
						If x < lastDays Then max += 1
						[set](DAY_OF_WEEK, internalGet(DAY_OF_WEEK))
						Exit Select
					End If

					' Transition year handling.
					Dim fd As Long = cachedFixedDate
					Dim month1 As Long = getFixedDateMonth1(jdate, fd)
					Dim monthLength As Integer = actualMonthLength()
					Dim lastDays As Integer = monthLength Mod 7
					max = monthLength \ 7
					Dim x As Integer = CInt(fd - month1) Mod 7
					If x < lastDays Then max += 1
					Dim value As Integer = getRolledValue(internalGet(field), amount, min, max) - 1
					fd = month1 + value * 7 + x
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(fd)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return
			End Select

			[set](field, getRolledValue(internalGet(field), amount, min, max))
		End Sub

		Public Overrides Function getDisplayName(ByVal field As Integer, ByVal style As Integer, ByVal locale_Renamed As Locale) As String
			If Not checkDisplayNameParams(field, style, [SHORT], NARROW_FORMAT, locale_Renamed, ERA_MASK Or YEAR_MASK Or MONTH_MASK Or DAY_OF_WEEK_MASK Or AM_PM_MASK) Then Return Nothing

			Dim fieldValue As Integer = [get](field)

			' "GanNen" is supported only in the LONG style.
			If field = YEAR AndAlso (getBaseStyle(style) <> [LONG] OrElse fieldValue <> 1 OrElse [get](ERA) = 0) Then Return Nothing

			Dim name As String = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueName(calendarType, field, fieldValue, style, locale_Renamed)
			' If the ERA value is null, then
			' try to get its name or abbreviation from the Era instance.
			If name Is Nothing AndAlso field = ERA AndAlso fieldValue < eras.Length Then
				Dim era_Renamed As sun.util.calendar.Era = eras(fieldValue)
				name = If(style = [SHORT], era_Renamed.abbreviation, era_Renamed.name)
			End If
			Return name
		End Function

		Public Overrides Function getDisplayNames(ByVal field As Integer, ByVal style As Integer, ByVal locale_Renamed As Locale) As Map(Of String, Integer?)
			If Not checkDisplayNameParams(field, style, ALL_STYLES, NARROW_FORMAT, locale_Renamed, ERA_MASK Or YEAR_MASK Or MONTH_MASK Or DAY_OF_WEEK_MASK Or AM_PM_MASK) Then Return Nothing
			Dim names As Map(Of String, Integer?)
			names = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueNames(calendarType, field, style, locale_Renamed)
			' If strings[] has fewer than eras[], get more names from eras[].
			If names IsNot Nothing Then
				If field = ERA Then
					Dim size As Integer = names.size()
					If style = ALL_STYLES Then
						Dim values As [Set](Of Integer?) = New HashSet(Of Integer?)
						' count unique era values
						For Each key As String In names.Keys
							values.add(names.get(key))
						Next key
						size = values.size()
					End If
					If size < eras.Length Then
						Dim baseStyle_Renamed As Integer = getBaseStyle(style)
						For i As Integer = size To eras.Length - 1
							Dim era_Renamed As sun.util.calendar.Era = eras(i)
							If baseStyle_Renamed = ALL_STYLES OrElse baseStyle_Renamed = [SHORT] OrElse baseStyle_Renamed = NARROW_FORMAT Then names.put(era_Renamed.abbreviation, i)
							If baseStyle_Renamed = ALL_STYLES OrElse baseStyle_Renamed = [LONG] Then names.put(era_Renamed.name, i)
						Next i
					End If
				End If
			End If
			Return names
		End Function

		''' <summary>
		''' Returns the minimum value for the given calendar field of this
		''' <code>Calendar</code> instance. The minimum value is
		''' defined as the smallest value returned by the {@link
		''' Calendar#get(int) get} method for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getMinimum(ByVal field As Integer) As Integer
			Return MIN_VALUES(field)
		End Function

		''' <summary>
		''' Returns the maximum value for the given calendar field of this
		''' <code>GregorianCalendar</code> instance. The maximum value is
		''' defined as the largest value returned by the {@link
		''' Calendar#get(int) get} method for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getMaximum(ByVal field As Integer) As Integer
			Select Case field
			Case YEAR
					' The value should depend on the time zone of this calendar.
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.Max_Value, zone)
					Return System.Math.Max(LEAST_MAX_VALUES(YEAR), d.year)
			End Select
			Return MAX_VALUES(field)
		End Function

		''' <summary>
		''' Returns the highest minimum value for the given calendar field
		''' of this <code>GregorianCalendar</code> instance. The highest
		''' minimum value is defined as the largest value returned by
		''' <seealso cref="#getActualMinimum(int)"/> for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the highest minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getGreatestMinimum(ByVal field As Integer) As Integer
			Return If(field = YEAR, 1, MIN_VALUES(field))
		End Function

		''' <summary>
		''' Returns the lowest maximum value for the given calendar field
		''' of this <code>GregorianCalendar</code> instance. The lowest
		''' maximum value is defined as the smallest value returned by
		''' <seealso cref="#getActualMaximum(int)"/> for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the lowest maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getLeastMaximum(ByVal field As Integer) As Integer
			Select Case field
			Case YEAR
					Return System.Math.Min(LEAST_MAX_VALUES(YEAR), getMaximum(YEAR))
			End Select
			Return LEAST_MAX_VALUES(field)
		End Function

		''' <summary>
		''' Returns the minimum value that this calendar field could have,
		''' taking into consideration the given time value and the current
		''' values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the minimum of the given field for the time value of
		''' this <code>JapaneseImperialCalendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getActualMinimum(ByVal field As Integer) As Integer
			If Not isFieldSet(YEAR_MASK Or MONTH_MASK Or WEEK_OF_YEAR_MASK, field) Then Return getMinimum(field)

			Dim value As Integer = 0
			Dim jc As JapaneseImperialCalendar = normalizedCalendar
			' Get a local date which includes time of day and time zone,
			' which are missing in jc.jdate.
			Dim jd As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(jc.timeInMillis, zone)
			Dim eraIndex_Renamed As Integer = getEraIndex(jd)
			Select Case field
			Case YEAR
					If eraIndex_Renamed > BEFORE_MEIJI Then
						value = 1
						Dim since As Long = eras(eraIndex_Renamed).getSince(zone)
						Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(since, zone)
						' Use the same year in jd to take care of leap
						' years. i.e., both jd and d must agree on leap
						' or common years.
						jd.year = d.year
						jcal.normalize(jd)
						Debug.Assert(jd.leapYear = d.leapYear)
						If getYearOffsetInMillis(jd) < getYearOffsetInMillis(d) Then value += 1
					Else
						value = getMinimum(field)
						Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
						' Use an equvalent year of d.getYear() if
						' possible. Otherwise, ignore the leap year and
						' common year difference.
						Dim y As Integer = d.year
						If y > 400 Then y -= 400
						jd.year = y
						jcal.normalize(jd)
						If getYearOffsetInMillis(jd) < getYearOffsetInMillis(d) Then value += 1
					End If

			Case MONTH
					' In Before Meiji and Meiji, January is the first month.
					If eraIndex_Renamed > MEIJI AndAlso jd.year = 1 Then
						Dim since As Long = eras(eraIndex_Renamed).getSince(zone)
						Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(since, zone)
						value = d.month - 1
						If jd.dayOfMonth < d.dayOfMonth Then value += 1
					End If

			Case WEEK_OF_YEAR
					value = 1
					Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
					' shift 400 years to avoid underflow
					d.addYear(+400)
					jcal.normalize(d)
					jd.era = d.era
					jd.year = d.year
					jcal.normalize(jd)

					Dim jan1 As Long = jcal.getFixedDate(d)
					Dim fd As Long = jcal.getFixedDate(jd)
					Dim woy As Integer = getWeekNumber(jan1, fd)
					Dim day1 As Long = fd - (7 * (woy - 1))
					If (day1 < jan1) OrElse (day1 = jan1 AndAlso jd.timeOfDay < d.timeOfDay) Then value += 1
			End Select
			Return value
		End Function

		''' <summary>
		''' Returns the maximum value that this calendar field could have,
		''' taking into consideration the given time value and the current
		''' values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' For example, if the date of this instance is Heisei 16February 1,
		''' the actual maximum value of the <code>DAY_OF_MONTH</code> field
		''' is 29 because Heisei 16 is a leap year, and if the date of this
		''' instance is Heisei 17 February 1, it's 28.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the maximum of the given field for the time value of
		''' this <code>JapaneseImperialCalendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		Public Overrides Function getActualMaximum(ByVal field As Integer) As Integer
			Dim fieldsForFixedMax As Integer = ERA_MASK Or DAY_OF_WEEK_MASK Or HOUR_MASK Or AM_PM_MASK Or HOUR_OF_DAY_MASK Or MINUTE_MASK Or SECOND_MASK Or MILLISECOND_MASK Or ZONE_OFFSET_MASK Or DST_OFFSET_MASK
			If (fieldsForFixedMax And (1<<field)) <> 0 Then Return getMaximum(field)

			Dim jc As JapaneseImperialCalendar = normalizedCalendar
			Dim date_Renamed As sun.util.calendar.LocalGregorianCalendar.Date = jc.jdate
			Dim normalizedYear As Integer = date_Renamed.normalizedYear

			Dim value As Integer = -1
			Select Case field
			Case MONTH
					value = DECEMBER
					If isTransitionYear(date_Renamed.normalizedYear) Then
						' TODO: there may be multiple transitions in a year.
						Dim eraIndex_Renamed As Integer = getEraIndex(date_Renamed)
						If date_Renamed.year <> 1 Then
							eraIndex_Renamed += 1
							Debug.Assert(eraIndex_Renamed < eras.Length)
						End If
						Dim transition As Long = sinceFixedDates(eraIndex_Renamed)
						Dim fd As Long = jc.cachedFixedDate
						If fd < transition Then
							Dim ldate As sun.util.calendar.LocalGregorianCalendar.Date = CType(date_Renamed.clone(), sun.util.calendar.LocalGregorianCalendar.Date)
							jcal.getCalendarDateFromFixedDate(ldate, transition - 1)
							value = ldate.month - 1
						End If
					Else
						Dim d As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.Max_Value, zone)
						If date_Renamed.era = d.era AndAlso date_Renamed.year = d.year Then value = d.month - 1
					End If

			Case DAY_OF_MONTH
				value = jcal.getMonthLength(date_Renamed)

			Case DAY_OF_YEAR
					If isTransitionYear(date_Renamed.normalizedYear) Then
						' Handle transition year.
						' TODO: there may be multiple transitions in a year.
						Dim eraIndex_Renamed As Integer = getEraIndex(date_Renamed)
						If date_Renamed.year <> 1 Then
							eraIndex_Renamed += 1
							Debug.Assert(eraIndex_Renamed < eras.Length)
						End If
						Dim transition As Long = sinceFixedDates(eraIndex_Renamed)
						Dim fd As Long = jc.cachedFixedDate
						Dim d As sun.util.calendar.CalendarDate = gcal.newCalendarDate(TimeZone.NO_TIMEZONE)
						d.dateate(date_Renamed.normalizedYear, sun.util.calendar.BaseCalendar.JANUARY, 1)
						If fd < transition Then
							value = CInt(Fix(transition - gcal.getFixedDate(d)))
						Else
							d.addYear(+1)
							value = CInt(Fix(gcal.getFixedDate(d) - transition))
						End If
					Else
						Dim d As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.Max_Value, zone)
						If date_Renamed.era = d.era AndAlso date_Renamed.year = d.year Then
							Dim fd As Long = jcal.getFixedDate(d)
							Dim jan1 As Long = getFixedDateJan1(d, fd)
							value = CInt(fd - jan1) + 1
						ElseIf date_Renamed.year = getMinimum(YEAR) Then
							Dim d1 As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
							Dim fd1 As Long = jcal.getFixedDate(d1)
							d1.addYear(1)
							d1.monthnth(sun.util.calendar.BaseCalendar.JANUARY).setDayOfMonth(1)
							jcal.normalize(d1)
							Dim fd2 As Long = jcal.getFixedDate(d1)
							value = CInt(fd2 - fd1)
						Else
							value = jcal.getYearLength(date_Renamed)
						End If
					End If

			Case WEEK_OF_YEAR
					If Not isTransitionYear(date_Renamed.normalizedYear) Then
						Dim jd As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.Max_Value, zone)
						If date_Renamed.era = jd.era AndAlso date_Renamed.year = jd.year Then
							Dim fd As Long = jcal.getFixedDate(jd)
							Dim jan1 As Long = getFixedDateJan1(jd, fd)
							value = getWeekNumber(jan1, fd)
						ElseIf date_Renamed.era Is Nothing AndAlso date_Renamed.year = getMinimum(YEAR) Then
							Dim d As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
							' shift 400 years to avoid underflow
							d.addYear(+400)
							jcal.normalize(d)
							jd.era = d.era
							jd.dateate(d.year + 1, sun.util.calendar.BaseCalendar.JANUARY, 1)
							jcal.normalize(jd)
							Dim jan1 As Long = jcal.getFixedDate(d)
							Dim nextJan1 As Long = jcal.getFixedDate(jd)
							Dim nextJan1st As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, firstDayOfWeek)
							Dim ndays As Integer = CInt(nextJan1st - nextJan1)
							If ndays >= minimalDaysInFirstWeek Then nextJan1st -= 7
							value = getWeekNumber(jan1, nextJan1st)
						Else
							' Get the day of week of January 1 of the year
							Dim d As sun.util.calendar.CalendarDate = gcal.newCalendarDate(TimeZone.NO_TIMEZONE)
							d.dateate(date_Renamed.normalizedYear, sun.util.calendar.BaseCalendar.JANUARY, 1)
							Dim dayOfWeek As Integer = gcal.getDayOfWeek(d)
							' Normalize the day of week with the firstDayOfWeek value
							dayOfWeek -= firstDayOfWeek
							If dayOfWeek < 0 Then dayOfWeek += 7
							value = 52
							Dim magic As Integer = dayOfWeek + minimalDaysInFirstWeek - 1
							If (magic = 6) OrElse (date_Renamed.leapYear AndAlso (magic = 5 OrElse magic = 12)) Then value += 1
						End If
						Exit Select
					End If

					If jc Is Me Then jc = CType(jc.clone(), JapaneseImperialCalendar)
					Dim max As Integer = getActualMaximum(DAY_OF_YEAR)
					jc.set(DAY_OF_YEAR, max)
					value = jc.get(WEEK_OF_YEAR)
					If value = 1 AndAlso max > 7 Then
						jc.add(WEEK_OF_YEAR, -1)
						value = jc.get(WEEK_OF_YEAR)
					End If

			Case WEEK_OF_MONTH
					Dim jd As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.Max_Value, zone)
					If Not(date_Renamed.era = jd.era AndAlso date_Renamed.year = jd.year) Then
						Dim d As sun.util.calendar.CalendarDate = gcal.newCalendarDate(TimeZone.NO_TIMEZONE)
						d.dateate(date_Renamed.normalizedYear, date_Renamed.month, 1)
						Dim dayOfWeek As Integer = gcal.getDayOfWeek(d)
						Dim monthLength As Integer = gcal.getMonthLength(d)
						dayOfWeek -= firstDayOfWeek
						If dayOfWeek < 0 Then dayOfWeek += 7
						Dim nDaysFirstWeek As Integer = 7 - dayOfWeek ' # of days in the first week
						value = 3
						If nDaysFirstWeek >= minimalDaysInFirstWeek Then value += 1
						monthLength -= nDaysFirstWeek + 7 * 3
						If monthLength > 0 Then
							value += 1
							If monthLength > 7 Then value += 1
						End If
					Else
						Dim fd As Long = jcal.getFixedDate(jd)
						Dim month1 As Long = fd - jd.dayOfMonth + 1
						value = getWeekNumber(month1, fd)
					End If

			Case DAY_OF_WEEK_IN_MONTH
					Dim ndays, dow1 As Integer
					Dim dow As Integer = date_Renamed.dayOfWeek
					Dim d As sun.util.calendar.BaseCalendar.Date = CType(date_Renamed.clone(), sun.util.calendar.BaseCalendar.Date)
					ndays = jcal.getMonthLength(d)
					d.dayOfMonth = 1
					jcal.normalize(d)
					dow1 = d.dayOfWeek
					Dim x As Integer = dow - dow1
					If x < 0 Then x += 7
					ndays -= x
					value = (ndays + 6) \ 7

			Case YEAR
					Dim jd As sun.util.calendar.CalendarDate = jcal.getCalendarDate(jc.timeInMillis, zone)
					Dim d As sun.util.calendar.CalendarDate
					Dim eraIndex_Renamed As Integer = getEraIndex(date_Renamed)
					If eraIndex_Renamed = eras.Length - 1 Then
						d = jcal.getCalendarDate(Long.Max_Value, zone)
						value = d.year
						' Use an equivalent year for the
						' getYearOffsetInMillis call to avoid overflow.
						If value > 400 Then jd.year = value - 400
					Else
						d = jcal.getCalendarDate(eras(eraIndex_Renamed + 1).getSince(zone) - 1, zone)
						value = d.year
						' Use the same year as d.getYear() to be
						' consistent with leap and common years.
						jd.year = value
					End If
					jcal.normalize(jd)
					If getYearOffsetInMillis(jd) > getYearOffsetInMillis(d) Then value -= 1

			Case Else
				Throw New ArrayIndexOutOfBoundsException(field)
			End Select
			Return value
		End Function

		''' <summary>
		''' Returns the millisecond offset from the beginning of the
		''' year. In the year for java.lang.[Long].MIN_VALUE, it's a pseudo value
		''' beyond the limit. The given CalendarDate object must have been
		''' normalized before calling this method.
		''' </summary>
		Private Function getYearOffsetInMillis(ByVal [date] As sun.util.calendar.CalendarDate) As Long
			Dim t As Long = (jcal.getDayOfYear(date_Renamed) - 1) * ONE_DAY
			Return t + date_Renamed.timeOfDay - date_Renamed.zoneOffset
		End Function

		Public Overrides Function clone() As Object
			Dim other As JapaneseImperialCalendar = CType(MyBase.clone(), JapaneseImperialCalendar)

			other.jdate = CType(jdate.clone(), sun.util.calendar.LocalGregorianCalendar.Date)
			other.originalFields = Nothing
			other.zoneOffsets = Nothing
			Return other
		End Function

		Public Property Overrides timeZone As TimeZone
			Get
				Dim zone_Renamed As TimeZone = MyBase.timeZone
				' To share the zone by the CalendarDate
				jdate.zone = zone_Renamed
				Return zone_Renamed
			End Get
			Set(ByVal zone As TimeZone)
				MyBase.timeZone = zone
				' To share the zone by the CalendarDate
				jdate.zone = zone
			End Set
		End Property


		''' <summary>
		''' The fixed date corresponding to jdate. If the value is
		''' java.lang.[Long].MIN_VALUE, the fixed date value is unknown.
		''' </summary>
		<NonSerialized> _
		Private cachedFixedDate As Long = java.lang.[Long].MIN_VALUE

		''' <summary>
		''' Converts the time value (millisecond offset from the <a
		''' href="Calendar.html#Epoch">Epoch</a>) to calendar field values.
		''' The time is <em>not</em>
		''' recomputed first; to recompute the time, then the fields, call the
		''' <code>complete</code> method.
		''' </summary>
		''' <seealso cref= Calendar#complete </seealso>
		Protected Friend Overrides Sub computeFields()
			Dim mask As Integer = 0
			If partiallyNormalized Then
				' Determine which calendar fields need to be computed.
				mask = stateFieldslds
				Dim fieldMask As Integer = (Not mask) And ALL_FIELDS
				If fieldMask <> 0 OrElse cachedFixedDate = java.lang.[Long].MIN_VALUE Then
					mask = mask Or computeFields(fieldMask, mask And (ZONE_OFFSET_MASK Or DST_OFFSET_MASK))
					Debug.Assert(mask = ALL_FIELDS)
				End If
			Else
				' Specify all fields
				mask = ALL_FIELDS
				computeFields(mask, 0)
			End If
			' After computing all the fields, set the field state to `COMPUTED'.
			fieldsComputed = mask
		End Sub

		''' <summary>
		''' This computeFields implements the conversion from UTC
		''' (millisecond offset from the Epoch) to calendar
		''' field values. fieldMask specifies which fields to change the
		''' setting state to COMPUTED, although all fields are set to
		''' the correct values. This is required to fix 4685354.
		''' </summary>
		''' <param name="fieldMask"> a bit mask to specify which fields to change
		''' the setting state. </param>
		''' <param name="tzMask"> a bit mask to specify which time zone offset
		''' fields to be used for time calculations </param>
		''' <returns> a new field mask that indicates what field values have
		''' actually been set. </returns>
		Private Function computeFields(ByVal fieldMask As Integer, ByVal tzMask As Integer) As Integer
			Dim zoneOffset As Integer = 0
			Dim tz As TimeZone = zone
			If zoneOffsets Is Nothing Then zoneOffsets = New Integer(1){}
			If tzMask <> (ZONE_OFFSET_MASK Or DST_OFFSET_MASK) Then
				If TypeOf tz Is sun.util.calendar.ZoneInfo Then
					zoneOffset = CType(tz, sun.util.calendar.ZoneInfo).getOffsets(time, zoneOffsets)
				Else
					zoneOffset = tz.getOffset(time)
					zoneOffsets(0) = tz.rawOffset
					zoneOffsets(1) = zoneOffset - zoneOffsets(0)
				End If
			End If
			If tzMask <> 0 Then
				If isFieldSet(tzMask, ZONE_OFFSET) Then zoneOffsets(0) = internalGet(ZONE_OFFSET)
				If isFieldSet(tzMask, DST_OFFSET) Then zoneOffsets(1) = internalGet(DST_OFFSET)
				zoneOffset = zoneOffsets(0) + zoneOffsets(1)
			End If

			' By computing time and zoneOffset separately, we can take
			' the wider range of time+zoneOffset than the previous
			' implementation.
			Dim fixedDate_Renamed As Long = zoneOffset \ ONE_DAY
			Dim timeOfDay As Integer = zoneOffset Mod CInt(ONE_DAY)
			fixedDate_Renamed += time \ ONE_DAY
			timeOfDay += CInt(Fix(time Mod ONE_DAY))
			If timeOfDay >= ONE_DAY Then
				timeOfDay -= ONE_DAY
				fixedDate_Renamed += 1
			Else
				Do While timeOfDay < 0
					timeOfDay += ONE_DAY
					fixedDate_Renamed -= 1
				Loop
			End If
			fixedDate_Renamed += EPOCH_OFFSET

			' See if we can use jdate to avoid date calculation.
			If fixedDate_Renamed <> cachedFixedDate OrElse fixedDate_Renamed < 0 Then
				jcal.getCalendarDateFromFixedDate(jdate, fixedDate_Renamed)
				cachedFixedDate = fixedDate_Renamed
			End If
			Dim era_Renamed As Integer = getEraIndex(jdate)
			Dim year_Renamed As Integer = jdate.year

			' Always set the ERA and YEAR values.
			internalSet(ERA, era_Renamed)
			internalSet(YEAR, year_Renamed)
			Dim mask As Integer = fieldMask Or (ERA_MASK Or YEAR_MASK)

			Dim month_Renamed As Integer = jdate.month - 1 ' 0-based
			Dim dayOfMonth As Integer = jdate.dayOfMonth

			' Set the basic date fields.
			If (fieldMask And (MONTH_MASK Or DAY_OF_MONTH_MASK Or DAY_OF_WEEK_MASK)) <> 0 Then
				internalSet(MONTH, month_Renamed)
				internalSet(DAY_OF_MONTH, dayOfMonth)
				internalSet(DAY_OF_WEEK, jdate.dayOfWeek)
				mask = mask Or MONTH_MASK Or DAY_OF_MONTH_MASK Or DAY_OF_WEEK_MASK
			End If

			If (fieldMask And (HOUR_OF_DAY_MASK Or AM_PM_MASK Or HOUR_MASK Or MINUTE_MASK Or SECOND_MASK Or MILLISECOND_MASK)) <> 0 Then
				If timeOfDay <> 0 Then
					Dim hours As Integer = timeOfDay \ ONE_HOUR
					internalSet(HOUR_OF_DAY, hours)
					internalSet(AM_PM, hours \ 12) ' Assume AM == 0
					internalSet(HOUR, hours Mod 12)
					Dim r As Integer = timeOfDay Mod ONE_HOUR
					internalSet(MINUTE, r \ ONE_MINUTE)
					r = r Mod ONE_MINUTE
					internalSet(SECOND, r \ ONE_SECOND)
					internalSet(MILLISECOND, r Mod ONE_SECOND)
				Else
					internalSet(HOUR_OF_DAY, 0)
					internalSet(AM_PM, AM)
					internalSet(HOUR, 0)
					internalSet(MINUTE, 0)
					internalSet(SECOND, 0)
					internalSet(MILLISECOND, 0)
				End If
				mask = mask Or (HOUR_OF_DAY_MASK Or AM_PM_MASK Or HOUR_MASK Or MINUTE_MASK Or SECOND_MASK Or MILLISECOND_MASK)
			End If

			If (fieldMask And (ZONE_OFFSET_MASK Or DST_OFFSET_MASK)) <> 0 Then
				internalSet(ZONE_OFFSET, zoneOffsets(0))
				internalSet(DST_OFFSET, zoneOffsets(1))
				mask = mask Or (ZONE_OFFSET_MASK Or DST_OFFSET_MASK)
			End If

			If (fieldMask And (DAY_OF_YEAR_MASK Or WEEK_OF_YEAR_MASK Or WEEK_OF_MONTH_MASK Or DAY_OF_WEEK_IN_MONTH_MASK)) <> 0 Then
				Dim normalizedYear As Integer = jdate.normalizedYear
				' If it's a year of an era transition, we need to handle
				' irregular year boundaries.
				Dim transitionYear_Renamed As Boolean = isTransitionYear(jdate.normalizedYear)
				Dim dayOfYear As Integer
				Dim fixedDateJan1_Renamed As Long
				If transitionYear_Renamed Then
					fixedDateJan1_Renamed = getFixedDateJan1(jdate, fixedDate_Renamed)
					dayOfYear = CInt(fixedDate_Renamed - fixedDateJan1_Renamed) + 1
				ElseIf normalizedYear = MIN_VALUES(YEAR) Then
					Dim dx As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
					fixedDateJan1_Renamed = jcal.getFixedDate(dx)
					dayOfYear = CInt(fixedDate_Renamed - fixedDateJan1_Renamed) + 1
				Else
					dayOfYear = CInt(Fix(jcal.getDayOfYear(jdate)))
					fixedDateJan1_Renamed = fixedDate_Renamed - dayOfYear + 1
				End If
				Dim fixedDateMonth1_Renamed As Long = If(transitionYear_Renamed, getFixedDateMonth1(jdate, fixedDate_Renamed), fixedDate_Renamed - dayOfMonth + 1)

				internalSet(DAY_OF_YEAR, dayOfYear)
				internalSet(DAY_OF_WEEK_IN_MONTH, (dayOfMonth - 1) \ 7 + 1)

				Dim weekOfYear As Integer = getWeekNumber(fixedDateJan1_Renamed, fixedDate_Renamed)

				' The spec is to calculate WEEK_OF_YEAR in the
				' ISO8601-style. This creates problems, though.
				If weekOfYear = 0 Then
					' If the date belongs to the last week of the
					' previous year, use the week number of "12/31" of
					' the "previous" year. Again, if the previous year is
					' a transition year, we need to take care of it.
					' Usually the previous day of the first day of a year
					' is December 31, which is not always true in the
					' Japanese imperial calendar system.
					Dim fixedDec31 As Long = fixedDateJan1_Renamed - 1
					Dim prevJan1 As Long
					Dim d As sun.util.calendar.LocalGregorianCalendar.Date = getCalendarDate(fixedDec31)
					If Not(transitionYear_Renamed OrElse isTransitionYear(d.normalizedYear)) Then
						prevJan1 = fixedDateJan1_Renamed - 365
						If d.leapYear Then prevJan1 -= 1
					ElseIf transitionYear_Renamed Then
						If jdate.year = 1 Then
							' As of Heisei (since Meiji) there's no case
							' that there are multiple transitions in a
							' year.  Historically there was such
							' case. There might be such case again in the
							' future.
							If era_Renamed > HEISEI Then
								Dim pd As sun.util.calendar.CalendarDate = eras(era_Renamed - 1).sinceDate
								If normalizedYear = pd.year Then d.monthnth(pd.month).setDayOfMonth(pd.dayOfMonth)
							Else
								d.monthnth(sun.util.calendar.LocalGregorianCalendar.JANUARY).setDayOfMonth(1)
							End If
							jcal.normalize(d)
							prevJan1 = jcal.getFixedDate(d)
						Else
							prevJan1 = fixedDateJan1_Renamed - 365
							If d.leapYear Then prevJan1 -= 1
						End If
					Else
						Dim cd As sun.util.calendar.CalendarDate = eras(getEraIndex(jdate)).sinceDate
						d.monthnth(cd.month).setDayOfMonth(cd.dayOfMonth)
						jcal.normalize(d)
						prevJan1 = jcal.getFixedDate(d)
					End If
					weekOfYear = getWeekNumber(prevJan1, fixedDec31)
				Else
					If Not transitionYear_Renamed Then
						' Regular years
						If weekOfYear >= 52 Then
							Dim nextJan1 As Long = fixedDateJan1_Renamed + 365
							If jdate.leapYear Then nextJan1 += 1
							Dim nextJan1st As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, firstDayOfWeek)
							Dim ndays As Integer = CInt(nextJan1st - nextJan1)
							If ndays >= minimalDaysInFirstWeek AndAlso fixedDate_Renamed >= (nextJan1st - 7) Then weekOfYear = 1
						End If
					Else
						Dim d As sun.util.calendar.LocalGregorianCalendar.Date = CType(jdate.clone(), sun.util.calendar.LocalGregorianCalendar.Date)
						Dim nextJan1 As Long
						If jdate.year = 1 Then
							d.addYear(+1)
							d.monthnth(sun.util.calendar.LocalGregorianCalendar.JANUARY).setDayOfMonth(1)
							nextJan1 = jcal.getFixedDate(d)
						Else
							Dim nextEraIndex As Integer = getEraIndex(d) + 1
							Dim cd As sun.util.calendar.CalendarDate = eras(nextEraIndex).sinceDate
							d.era = eras(nextEraIndex)
							d.dateate(1, cd.month, cd.dayOfMonth)
							jcal.normalize(d)
							nextJan1 = jcal.getFixedDate(d)
						End If
						Dim nextJan1st As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, firstDayOfWeek)
						Dim ndays As Integer = CInt(nextJan1st - nextJan1)
						If ndays >= minimalDaysInFirstWeek AndAlso fixedDate_Renamed >= (nextJan1st - 7) Then weekOfYear = 1
					End If
				End If
				internalSet(WEEK_OF_YEAR, weekOfYear)
				internalSet(WEEK_OF_MONTH, getWeekNumber(fixedDateMonth1_Renamed, fixedDate_Renamed))
				mask = mask Or (DAY_OF_YEAR_MASK Or WEEK_OF_YEAR_MASK Or WEEK_OF_MONTH_MASK Or DAY_OF_WEEK_IN_MONTH_MASK)
			End If
			Return mask
		End Function

		''' <summary>
		''' Returns the number of weeks in a period between fixedDay1 and
		''' fixedDate. The getFirstDayOfWeek-getMinimalDaysInFirstWeek rule
		''' is applied to calculate the number of weeks.
		''' </summary>
		''' <param name="fixedDay1"> the fixed date of the first day of the period </param>
		''' <param name="fixedDate"> the fixed date of the last day of the period </param>
		''' <returns> the number of weeks of the given period </returns>
		Private Function getWeekNumber(ByVal fixedDay1 As Long, ByVal fixedDate As Long) As Integer
			' We can always use `jcal' since Julian and Gregorian are the
			' same thing for this calculation.
			Dim fixedDay1st As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDay1 + 6, firstDayOfWeek)
			Dim ndays As Integer = CInt(fixedDay1st - fixedDay1)
			Debug.Assert(ndays <= 7)
			If ndays >= minimalDaysInFirstWeek Then fixedDay1st -= 7
			Dim normalizedDayOfPeriod As Integer = CInt(fixedDate - fixedDay1st)
			If normalizedDayOfPeriod >= 0 Then Return normalizedDayOfPeriod \ 7 + 1
			Return sun.util.calendar.CalendarUtils.floorDivide(normalizedDayOfPeriod, 7) + 1
		End Function

		''' <summary>
		''' Converts calendar field values to the time value (millisecond
		''' offset from the <a href="Calendar.html#Epoch">Epoch</a>).
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if any calendar fields are invalid. </exception>
		Protected Friend Overrides Sub computeTime()
			' In non-lenient mode, perform brief checking of calendar
			' fields which have been set externally. Through this
			' checking, the field values are stored in originalFields[]
			' to see if any of them are normalized later.
			If Not lenient Then
				If originalFields Is Nothing Then originalFields = New Integer(FIELD_COUNT - 1){}
				For field As Integer = 0 To FIELD_COUNT - 1
					Dim value As Integer = internalGet(field)
					If isExternallySet(field) Then
						' Quick validation for any out of range values
						If value < getMinimum(field) OrElse value > getMaximum(field) Then Throw New IllegalArgumentException(getFieldName(field))
					End If
					originalFields(field) = value
				Next field
			End If

			' Let the super class determine which calendar fields to be
			' used to calculate the time.
			Dim fieldMask As Integer = selectFields()

			Dim year_Renamed As Integer
			Dim era_Renamed As Integer

			If isSet(ERA) Then
				era_Renamed = internalGet(ERA)
				year_Renamed = If(isSet(YEAR), internalGet(YEAR), 1)
			Else
				If isSet(YEAR) Then
					era_Renamed = eras.Length - 1
					year_Renamed = internalGet(YEAR)
				Else
					' Equivalent to 1970 (Gregorian)
					era_Renamed = SHOWA
					year_Renamed = 45
				End If
			End If

			' Calculate the time of day. We rely on the convention that
			' an UNSET field has 0.
			Dim timeOfDay As Long = 0
			If isFieldSet(fieldMask, HOUR_OF_DAY) Then
				timeOfDay += CLng(internalGet(HOUR_OF_DAY))
			Else
				timeOfDay += internalGet(HOUR)
				' The default value of AM_PM is 0 which designates AM.
				If isFieldSet(fieldMask, AM_PM) Then timeOfDay += 12 * internalGet(AM_PM)
			End If
			timeOfDay *= 60
			timeOfDay += internalGet(MINUTE)
			timeOfDay *= 60
			timeOfDay += internalGet(SECOND)
			timeOfDay *= 1000
			timeOfDay += internalGet(MILLISECOND)

			' Convert the time of day to the number of days and the
			' millisecond offset from midnight.
			Dim fixedDate_Renamed As Long = timeOfDay \ ONE_DAY
			timeOfDay = timeOfDay Mod ONE_DAY
			Do While timeOfDay < 0
				timeOfDay += ONE_DAY
				fixedDate_Renamed -= 1
			Loop

			' Calculate the fixed date since January 1, 1 (Gregorian).
			fixedDate_Renamed += getFixedDate(era_Renamed, year_Renamed, fieldMask)

			' millis represents local wall-clock time in milliseconds.
			Dim millis As Long = (fixedDate_Renamed - EPOCH_OFFSET) * ONE_DAY + timeOfDay

			' Compute the time zone offset and DST offset.  There are two potential
			' ambiguities here.  We'll assume a 2:00 am (wall time) switchover time
			' for discussion purposes here.
			' 1. The transition into DST.  Here, a designated time of 2:00 am - 2:59 am
			'    can be in standard or in DST depending.  However, 2:00 am is an invalid
			'    representation (the representation jumps from 1:59:59 am Std to 3:00:00 am DST).
			'    We assume standard time.
			' 2. The transition out of DST.  Here, a designated time of 1:00 am - 1:59 am
			'    can be in standard or DST.  Both are valid representations (the rep
			'    jumps from 1:59:59 DST to 1:00:00 Std).
			'    Again, we assume standard time.
			' We use the TimeZone object, unless the user has explicitly set the ZONE_OFFSET
			' or DST_OFFSET fields; then we use those fields.
			Dim zone_Renamed As TimeZone = zone
			If zoneOffsets Is Nothing Then zoneOffsets = New Integer(1){}
			Dim tzMask As Integer = fieldMask And (ZONE_OFFSET_MASK Or DST_OFFSET_MASK)
			If tzMask <> (ZONE_OFFSET_MASK Or DST_OFFSET_MASK) Then
				If TypeOf zone_Renamed Is sun.util.calendar.ZoneInfo Then
					CType(zone_Renamed, sun.util.calendar.ZoneInfo).getOffsetsByWall(millis, zoneOffsets)
				Else
					zone_Renamed.getOffsets(millis - zone_Renamed.rawOffset, zoneOffsets)
				End If
			End If
			If tzMask <> 0 Then
				If isFieldSet(tzMask, ZONE_OFFSET) Then zoneOffsets(0) = internalGet(ZONE_OFFSET)
				If isFieldSet(tzMask, DST_OFFSET) Then zoneOffsets(1) = internalGet(DST_OFFSET)
			End If

			' Adjust the time zone offset values to get the UTC time.
			millis -= zoneOffsets(0) + zoneOffsets(1)

			' Set this calendar's time in milliseconds
			time = millis

			Dim mask As Integer = computeFields(fieldMask Or stateFieldslds, tzMask)

			If Not lenient Then
				For field As Integer = 0 To FIELD_COUNT - 1
					If Not isExternallySet(field) Then Continue For
					If originalFields(field) <> internalGet(field) Then
						Dim wrongValue As Integer = internalGet(field)
						' Restore the original field values
						Array.Copy(originalFields, 0, fields, 0, fields.Length)
						Throw New IllegalArgumentException(getFieldName(field) & "=" & wrongValue & ", expected " & originalFields(field))
					End If
				Next field
			End If
			fieldsNormalized = mask
		End Sub

		''' <summary>
		''' Computes the fixed date under either the Gregorian or the
		''' Julian calendar, using the given year and the specified calendar fields.
		''' </summary>
		''' <param name="era"> era index </param>
		''' <param name="year"> the normalized year number, with 0 indicating the
		''' year 1 BCE, -1 indicating 2 BCE, etc. </param>
		''' <param name="fieldMask"> the calendar fields to be used for the date calculation </param>
		''' <returns> the fixed date </returns>
		''' <seealso cref= Calendar#selectFields </seealso>
		Private Function getFixedDate(ByVal era As Integer, ByVal year As Integer, ByVal fieldMask As Integer) As Long
			Dim month_Renamed As Integer = JANUARY
			Dim firstDayOfMonth As Integer = 1
			If isFieldSet(fieldMask, MONTH) Then
				' No need to check if MONTH has been set (no isSet(MONTH)
				' call) since its unset value happens to be JANUARY (0).
				month_Renamed = internalGet(MONTH)

				' If the month is out of range, adjust it into range.
				If month_Renamed > DECEMBER Then
					year += month_Renamed \ 12
					month_Renamed = month Mod 12
				ElseIf month_Renamed < JANUARY Then
					Dim [rem] As Integer() = New Integer(0){}
					year += sun.util.calendar.CalendarUtils.floorDivide(month_Renamed, 12, [rem])
					month_Renamed = [rem](0)
				End If
			Else
				If year = 1 AndAlso era <> 0 Then
					Dim d As sun.util.calendar.CalendarDate = eras(era).sinceDate
					month_Renamed = d.month - 1
					firstDayOfMonth = d.dayOfMonth
				End If
			End If

			' Adjust the base date if year is the minimum value.
			If year = MIN_VALUES(JapaneseImperialCalendar.YEAR) Then
				Dim dx As sun.util.calendar.CalendarDate = jcal.getCalendarDate(Long.MIN_VALUE, zone)
				Dim m As Integer = dx.month - 1
				If month_Renamed < m Then month_Renamed = m
				If month_Renamed = m Then firstDayOfMonth = dx.dayOfMonth
			End If

			Dim date_Renamed As sun.util.calendar.LocalGregorianCalendar.Date = jcal.newCalendarDate(TimeZone.NO_TIMEZONE)
			date_Renamed.era = If(era > 0, eras(era), Nothing)
			date_Renamed.dateate(year, month_Renamed + 1, firstDayOfMonth)
			jcal.normalize(date_Renamed)

			' Get the fixed date since Jan 1, 1 (Gregorian). We are on
			' the first day of either `month' or January in 'year'.
			Dim fixedDate_Renamed As Long = jcal.getFixedDate(date_Renamed)

			If isFieldSet(fieldMask, MONTH) Then
				' Month-based calculations
				If isFieldSet(fieldMask, DAY_OF_MONTH) Then
					' We are on the "first day" of the month (which may
					' not be 1). Just add the offset if DAY_OF_MONTH is
					' set. If the isSet call returns false, that means
					' DAY_OF_MONTH has been selected just because of the
					' selected combination. We don't need to add any
					' since the default value is the "first day".
					If isSet(DAY_OF_MONTH) Then
						' To avoid underflow with DAY_OF_MONTH-firstDayOfMonth, add
						' DAY_OF_MONTH, then subtract firstDayOfMonth.
						fixedDate_Renamed += internalGet(DAY_OF_MONTH)
						fixedDate_Renamed -= firstDayOfMonth
					End If
				Else
					If isFieldSet(fieldMask, WEEK_OF_MONTH) Then
						Dim firstDayOfWeek_Renamed As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + 6, firstDayOfWeek)
						' If we have enough days in the first week, then
						' move to the previous week.
						If (firstDayOfWeek_Renamed - fixedDate_Renamed) >= minimalDaysInFirstWeek Then firstDayOfWeek_Renamed -= 7
						If isFieldSet(fieldMask, DAY_OF_WEEK) Then firstDayOfWeek_Renamed = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek_Renamed + 6, internalGet(DAY_OF_WEEK))
						' In lenient mode, we treat days of the previous
						' months as a part of the specified
						' WEEK_OF_MONTH. See 4633646.
						fixedDate_Renamed = firstDayOfWeek_Renamed + 7 * (internalGet(WEEK_OF_MONTH) - 1)
					Else
						Dim dayOfWeek As Integer
						If isFieldSet(fieldMask, DAY_OF_WEEK) Then
							dayOfWeek = internalGet(DAY_OF_WEEK)
						Else
							dayOfWeek = firstDayOfWeek
						End If
						' We are basing this on the day-of-week-in-month.  The only
						' trickiness occurs if the day-of-week-in-month is
						' negative.
						Dim dowim As Integer
						If isFieldSet(fieldMask, DAY_OF_WEEK_IN_MONTH) Then
							dowim = internalGet(DAY_OF_WEEK_IN_MONTH)
						Else
							dowim = 1
						End If
						If dowim >= 0 Then
							fixedDate_Renamed = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + (7 * dowim) - 1, dayOfWeek)
						Else
							' Go to the first day of the next week of
							' the specified week boundary.
							Dim lastDate As Integer = monthLength(month_Renamed, year) + (7 * (dowim + 1))
							' Then, get the day of week date on or before the last date.
							fixedDate_Renamed = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + lastDate - 1, dayOfWeek)
						End If
					End If
				End If
			Else
				' We are on the first day of the year.
				If isFieldSet(fieldMask, DAY_OF_YEAR) Then
					If isTransitionYear(date_Renamed.normalizedYear) Then fixedDate_Renamed = getFixedDateJan1(date_Renamed, fixedDate_Renamed)
					' Add the offset, then subtract 1. (Make sure to avoid underflow.)
					fixedDate_Renamed += internalGet(DAY_OF_YEAR)
					fixedDate_Renamed -= 1
				Else
					Dim firstDayOfWeek_Renamed As Long = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + 6, firstDayOfWeek)
					' If we have enough days in the first week, then move
					' to the previous week.
					If (firstDayOfWeek_Renamed - fixedDate_Renamed) >= minimalDaysInFirstWeek Then firstDayOfWeek_Renamed -= 7
					If isFieldSet(fieldMask, DAY_OF_WEEK) Then
						Dim dayOfWeek As Integer = internalGet(DAY_OF_WEEK)
						If dayOfWeek <> firstDayOfWeek Then firstDayOfWeek_Renamed = sun.util.calendar.LocalGregorianCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek_Renamed + 6, dayOfWeek)
					End If
					fixedDate_Renamed = firstDayOfWeek_Renamed + 7 * (CLng(internalGet(WEEK_OF_YEAR)) - 1)
				End If
			End If
			Return fixedDate_Renamed
		End Function

		''' <summary>
		''' Returns the fixed date of the first day of the year (usually
		''' January 1) before the specified date.
		''' </summary>
		''' <param name="date"> the date for which the first day of the year is
		''' calculated. The date has to be in the cut-over year. </param>
		''' <param name="fixedDate"> the fixed date representation of the date </param>
		Private Function getFixedDateJan1(ByVal [date] As sun.util.calendar.LocalGregorianCalendar.Date, ByVal fixedDate As Long) As Long
			Dim era_Renamed As sun.util.calendar.Era = date_Renamed.era
			If date_Renamed.era IsNot Nothing AndAlso date_Renamed.year = 1 Then
				For eraIndex_Renamed As Integer = getEraIndex(date_Renamed) To 1 Step -1
					Dim d As sun.util.calendar.CalendarDate = eras(eraIndex_Renamed).sinceDate
					Dim fd As Long = gcal.getFixedDate(d)
					' There might be multiple era transitions in a year.
					If fd > fixedDate Then Continue For
					Return fd
				Next eraIndex_Renamed
			End If
			Dim d As sun.util.calendar.CalendarDate = gcal.newCalendarDate(TimeZone.NO_TIMEZONE)
			d.dateate(date_Renamed.normalizedYear, sun.util.calendar.Gregorian.JANUARY, 1)
			Return gcal.getFixedDate(d)
		End Function

		''' <summary>
		''' Returns the fixed date of the first date of the month (usually
		''' the 1st of the month) before the specified date.
		''' </summary>
		''' <param name="date"> the date for which the first day of the month is
		''' calculated. The date must be in the era transition year. </param>
		''' <param name="fixedDate"> the fixed date representation of the date </param>
		Private Function getFixedDateMonth1(ByVal [date] As sun.util.calendar.LocalGregorianCalendar.Date, ByVal fixedDate As Long) As Long
			Dim eraIndex_Renamed As Integer = getTransitionEraIndex(date_Renamed)
			If eraIndex_Renamed <> -1 Then
				Dim transition As Long = sinceFixedDates(eraIndex_Renamed)
				' If the given date is on or after the transition date, then
				' return the transition date.
				If transition <= fixedDate Then Return transition
			End If

			' Otherwise, we can use the 1st day of the month.
			Return fixedDate - date_Renamed.dayOfMonth + 1
		End Function

		''' <summary>
		''' Returns a LocalGregorianCalendar.Date produced from the specified fixed date.
		''' </summary>
		''' <param name="fd"> the fixed date </param>
		Private Shared Function getCalendarDate(ByVal fd As Long) As sun.util.calendar.LocalGregorianCalendar.Date
			Dim d As sun.util.calendar.LocalGregorianCalendar.Date = jcal.newCalendarDate(TimeZone.NO_TIMEZONE)
			jcal.getCalendarDateFromFixedDate(d, fd)
			Return d
		End Function

		''' <summary>
		''' Returns the length of the specified month in the specified
		''' Gregorian year. The year number must be normalized.
		''' </summary>
		''' <seealso cref= GregorianCalendar#isLeapYear(int) </seealso>
		Private Function monthLength(ByVal month As Integer, ByVal gregorianYear As Integer) As Integer
			Return If(sun.util.calendar.CalendarUtils.isGregorianLeapYear(gregorianYear), GregorianCalendar.LEAP_MONTH_LENGTH(month), GregorianCalendar.MONTH_LENGTH(month))
		End Function

		''' <summary>
		''' Returns the length of the specified month in the year provided
		''' by internalGet(YEAR).
		''' </summary>
		''' <seealso cref= GregorianCalendar#isLeapYear(int) </seealso>
		Private Function monthLength(ByVal month As Integer) As Integer
			Debug.Assert(jdate.normalized)
			Return If(jdate.leapYear, GregorianCalendar.LEAP_MONTH_LENGTH(month), GregorianCalendar.MONTH_LENGTH(month))
		End Function

		Private Function actualMonthLength() As Integer
			Dim length As Integer = jcal.getMonthLength(jdate)
			Dim eraIndex_Renamed As Integer = getTransitionEraIndex(jdate)
			If eraIndex_Renamed = -1 Then
				Dim transitionFixedDate As Long = sinceFixedDates(eraIndex_Renamed)
				Dim d As sun.util.calendar.CalendarDate = eras(eraIndex_Renamed).sinceDate
				If transitionFixedDate <= cachedFixedDate Then
					length -= d.dayOfMonth - 1
				Else
					length = d.dayOfMonth - 1
				End If
			End If
			Return length
		End Function

		''' <summary>
		''' Returns the index to the new era if the given date is in a
		''' transition month.  For example, if the give date is Heisei 1
		''' (1989) January 20, then the era index for Heisei is
		''' returned. Likewise, if the given date is Showa 64 (1989)
		''' January 3, then the era index for Heisei is returned. If the
		''' given date is not in any transition month, then -1 is returned.
		''' </summary>
		Private Shared Function getTransitionEraIndex(ByVal [date] As sun.util.calendar.LocalGregorianCalendar.Date) As Integer
			Dim eraIndex_Renamed As Integer = getEraIndex(date_Renamed)
			Dim transitionDate As sun.util.calendar.CalendarDate = eras(eraIndex_Renamed).sinceDate
			If transitionDate.year = date_Renamed.normalizedYear AndAlso transitionDate.month = date_Renamed.month Then Return eraIndex_Renamed
			If eraIndex_Renamed < eras.Length - 1 Then
				eraIndex_Renamed += 1
				transitionDate = eras(eraIndex_Renamed).sinceDate
				If transitionDate.year = date_Renamed.normalizedYear AndAlso transitionDate.month = date_Renamed.month Then Return eraIndex_Renamed
			End If
			Return -1
		End Function

		Private Function isTransitionYear(ByVal normalizedYear As Integer) As Boolean
			For i As Integer = eras.Length - 1 To 1 Step -1
				Dim transitionYear_Renamed As Integer = eras(i).sinceDate.year
				If normalizedYear = transitionYear_Renamed Then Return True
				If normalizedYear > transitionYear_Renamed Then Exit For
			Next i
			Return False
		End Function

		Private Shared Function getEraIndex(ByVal [date] As sun.util.calendar.LocalGregorianCalendar.Date) As Integer
			Dim era_Renamed As sun.util.calendar.Era = date_Renamed.era
			For i As Integer = eras.Length - 1 To 1 Step -1
				If eras(i) Is era_Renamed Then Return i
			Next i
			Return 0
		End Function

		''' <summary>
		''' Returns this object if it's normalized (all fields and time are
		''' in sync). Otherwise, a cloned object is returned after calling
		''' complete() in lenient mode.
		''' </summary>
		Private Property normalizedCalendar As JapaneseImperialCalendar
			Get
				Dim jc As JapaneseImperialCalendar
				If fullyNormalized Then
					jc = Me
				Else
					' Create a clone and normalize the calendar fields
					jc = CType(Me.clone(), JapaneseImperialCalendar)
					jc.lenient = True
					jc.complete()
				End If
				Return jc
			End Get
		End Property

		''' <summary>
		''' After adjustments such as add(MONTH), add(YEAR), we don't want the
		''' month to jump around.  E.g., we don't want Jan 31 + 1 month to go to Mar
		''' 3, we want it to go to Feb 28.  Adjustments which might run into this
		''' problem call this method to retain the proper month.
		''' </summary>
		Private Sub pinDayOfMonth(ByVal [date] As sun.util.calendar.LocalGregorianCalendar.Date)
			Dim year_Renamed As Integer = date_Renamed.year
			Dim dom As Integer = date_Renamed.dayOfMonth
			If year_Renamed <> getMinimum(YEAR) Then
				date_Renamed.dayOfMonth = 1
				jcal.normalize(date_Renamed)
				Dim monthLength As Integer = jcal.getMonthLength(date_Renamed)
				If dom > monthLength Then
					date_Renamed.dayOfMonth = monthLength
				Else
					date_Renamed.dayOfMonth = dom
				End If
				jcal.normalize(date_Renamed)
			Else
				Dim d As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(Long.MIN_VALUE, zone)
				Dim realDate As sun.util.calendar.LocalGregorianCalendar.Date = jcal.getCalendarDate(time, zone)
				Dim tod As Long = realDate.timeOfDay
				' Use an equivalent year.
				realDate.addYear(+400)
				realDate.month = date_Renamed.month
				realDate.dayOfMonth = 1
				jcal.normalize(realDate)
				Dim monthLength As Integer = jcal.getMonthLength(realDate)
				If dom > monthLength Then
					realDate.dayOfMonth = monthLength
				Else
					If dom < d.dayOfMonth Then
						realDate.dayOfMonth = d.dayOfMonth
					Else
						realDate.dayOfMonth = dom
					End If
				End If
				If realDate.dayOfMonth = d.dayOfMonth AndAlso tod < d.timeOfDay Then realDate.dayOfMonth = System.Math.Min(dom + 1, monthLength)
				' restore the year.
				date_Renamed.dateate(year_Renamed, realDate.month, realDate.dayOfMonth)
				' Don't normalize date here so as not to cause underflow.
			End If
		End Sub

		''' <summary>
		''' Returns the new value after 'roll'ing the specified value and amount.
		''' </summary>
		Private Shared Function getRolledValue(ByVal value As Integer, ByVal amount As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
			Debug.Assert(value >= min AndAlso value <= max)
			Dim range As Integer = max - min + 1
			amount = amount Mod range
			Dim n As Integer = value + amount
			If n > max Then
				n -= range
			ElseIf n < min Then
				n += range
			End If
			Debug.Assert(n >= min AndAlso n <= max)
			Return n
		End Function

		''' <summary>
		''' Returns the ERA.  We need a special method for this because the
		''' default ERA is the current era, but a zero (unset) ERA means before Meiji.
		''' </summary>
		Private Function internalGetEra() As Integer
			Return If(isSet(ERA), internalGet(ERA), eras.Length - 1)
		End Function

		''' <summary>
		''' Updates internal state.
		''' </summary>
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			If jdate Is Nothing Then
				jdate = jcal.newCalendarDate(zone)
				cachedFixedDate = java.lang.[Long].MIN_VALUE
			End If
		End Sub
	End Class

End Namespace