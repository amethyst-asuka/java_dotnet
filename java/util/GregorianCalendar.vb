Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright Taligent, Inc. 1996-1998 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.util


	''' <summary>
	''' <code>GregorianCalendar</code> is a concrete subclass of
	''' <code>Calendar</code> and provides the standard calendar system
	''' used by most of the world.
	''' 
	''' <p> <code>GregorianCalendar</code> is a hybrid calendar that
	''' supports both the Julian and Gregorian calendar systems with the
	''' support of a single discontinuity, which corresponds by default to
	''' the Gregorian date when the Gregorian calendar was instituted
	''' (October 15, 1582 in some countries, later in others).  The cutover
	''' date may be changed by the caller by calling {@link
	''' #setGregorianChange(Date) setGregorianChange()}.
	''' 
	''' <p>
	''' Historically, in those countries which adopted the Gregorian calendar first,
	''' October 4, 1582 (Julian) was thus followed by October 15, 1582 (Gregorian). This calendar models
	''' this correctly.  Before the Gregorian cutover, <code>GregorianCalendar</code>
	''' implements the Julian calendar.  The only difference between the Gregorian
	''' and the Julian calendar is the leap year rule. The Julian calendar specifies
	''' leap years every four years, whereas the Gregorian calendar omits century
	''' years which are not divisible by 400.
	''' 
	''' <p>
	''' <code>GregorianCalendar</code> implements <em>proleptic</em> Gregorian and
	''' Julian calendars. That is, dates are computed by extrapolating the current
	''' rules indefinitely far backward and forward in time. As a result,
	''' <code>GregorianCalendar</code> may be used for all years to generate
	''' meaningful and consistent results. However, dates obtained using
	''' <code>GregorianCalendar</code> are historically accurate only from March 1, 4
	''' AD onward, when modern Julian calendar rules were adopted.  Before this date,
	''' leap year rules were applied irregularly, and before 45 BC the Julian
	''' calendar did not even exist.
	''' 
	''' <p>
	''' Prior to the institution of the Gregorian calendar, New Year's Day was
	''' March 25. To avoid confusion, this calendar always uses January 1. A manual
	''' adjustment may be made if desired for dates that are prior to the Gregorian
	''' changeover and which fall between January 1 and March 24.
	''' 
	''' <h3><a name="week_and_year">Week Of Year and Week Year</a></h3>
	''' 
	''' <p>Values calculated for the {@link Calendar#WEEK_OF_YEAR
	''' WEEK_OF_YEAR} field range from 1 to 53. The first week of a
	''' calendar year is the earliest seven day period starting on {@link
	''' Calendar#getFirstDayOfWeek() getFirstDayOfWeek()} that contains at
	''' least {@link Calendar#getMinimalDaysInFirstWeek()
	''' getMinimalDaysInFirstWeek()} days from that year. It thus depends
	''' on the values of {@code getMinimalDaysInFirstWeek()}, {@code
	''' getFirstDayOfWeek()}, and the day of the week of January 1. Weeks
	''' between week 1 of one year and week 1 of the following year
	''' (exclusive) are numbered sequentially from 2 to 52 or 53 (except
	''' for year(s) involved in the Julian-Gregorian transition).
	''' 
	''' <p>The {@code getFirstDayOfWeek()} and {@code
	''' getMinimalDaysInFirstWeek()} values are initialized using
	''' locale-dependent resources when constructing a {@code
	''' GregorianCalendar}. <a name="iso8601_compatible_setting">The week
	''' determination is compatible</a> with the ISO 8601 standard when {@code
	''' getFirstDayOfWeek()} is {@code MONDAY} and {@code
	''' getMinimalDaysInFirstWeek()} is 4, which values are used in locales
	''' where the standard is preferred. These values can explicitly be set by
	''' calling <seealso cref="Calendar#setFirstDayOfWeek(int) setFirstDayOfWeek()"/> and
	''' {@link Calendar#setMinimalDaysInFirstWeek(int)
	''' setMinimalDaysInFirstWeek()}.
	''' 
	''' <p>A <a name="week_year"><em>week year</em></a> is in sync with a
	''' {@code WEEK_OF_YEAR} cycle. All weeks between the first and last
	''' weeks (inclusive) have the same <em>week year</em> value.
	''' Therefore, the first and last days of a week year may have
	''' different calendar year values.
	''' 
	''' <p>For example, January 1, 1998 is a Thursday. If {@code
	''' getFirstDayOfWeek()} is {@code MONDAY} and {@code
	''' getMinimalDaysInFirstWeek()} is 4 (ISO 8601 standard compatible
	''' setting), then week 1 of 1998 starts on December 29, 1997, and ends
	''' on January 4, 1998. The week year is 1998 for the last three days
	''' of calendar year 1997. If, however, {@code getFirstDayOfWeek()} is
	''' {@code SUNDAY}, then week 1 of 1998 starts on January 4, 1998, and
	''' ends on January 10, 1998; the first three days of 1998 then are
	''' part of week 53 of 1997 and their week year is 1997.
	''' 
	''' <h4>Week Of Month</h4>
	''' 
	''' <p>Values calculated for the <code>WEEK_OF_MONTH</code> field range from 0
	''' to 6.  Week 1 of a month (the days with <code>WEEK_OF_MONTH =
	''' 1</code>) is the earliest set of at least
	''' <code>getMinimalDaysInFirstWeek()</code> contiguous days in that month,
	''' ending on the day before <code>getFirstDayOfWeek()</code>.  Unlike
	''' week 1 of a year, week 1 of a month may be shorter than 7 days, need
	''' not start on <code>getFirstDayOfWeek()</code>, and will not include days of
	''' the previous month.  Days of a month before week 1 have a
	''' <code>WEEK_OF_MONTH</code> of 0.
	''' 
	''' <p>For example, if <code>getFirstDayOfWeek()</code> is <code>SUNDAY</code>
	''' and <code>getMinimalDaysInFirstWeek()</code> is 4, then the first week of
	''' January 1998 is Sunday, January 4 through Saturday, January 10.  These days
	''' have a <code>WEEK_OF_MONTH</code> of 1.  Thursday, January 1 through
	''' Saturday, January 3 have a <code>WEEK_OF_MONTH</code> of 0.  If
	''' <code>getMinimalDaysInFirstWeek()</code> is changed to 3, then January 1
	''' through January 3 have a <code>WEEK_OF_MONTH</code> of 1.
	''' 
	''' <h4>Default Fields Values</h4>
	''' 
	''' <p>The <code>clear</code> method sets calendar field(s)
	''' undefined. <code>GregorianCalendar</code> uses the following
	''' default value for each calendar field if its value is undefined.
	''' 
	''' <table cellpadding="0" cellspacing="3" border="0"
	'''        summary="GregorianCalendar default field values"
	'''        style="text-align: left; width: 66%;">
	'''   <tbody>
	'''     <tr>
	'''       <th style="vertical-align: top; background-color: rgb(204, 204, 255);
	'''           text-align: center;">Field<br>
	'''       </th>
	'''       <th style="vertical-align: top; background-color: rgb(204, 204, 255);
	'''           text-align: center;">Default Value<br>
	'''       </th>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle;">
	'''              <code>ERA<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle;">
	'''              <code>AD<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	'''              <code>YEAR<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	'''              <code>1970<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle;">
	'''              <code>MONTH<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle;">
	'''              <code>JANUARY<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''              <code>DAY_OF_MONTH<br></code>
	'''       </td>
	'''       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''              <code>1<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle;">
	'''              <code>DAY_OF_WEEK<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle;">
	'''              <code>the first day of week<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''              <code>WEEK_OF_MONTH<br></code>
	'''       </td>
	'''       <td style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''              <code>0<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: top;">
	'''              <code>DAY_OF_WEEK_IN_MONTH<br></code>
	'''       </td>
	'''       <td style="vertical-align: top;">
	'''              <code>1<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	'''              <code>AM_PM<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle; background-color: rgb(238, 238, 255);">
	'''              <code>AM<br></code>
	'''       </td>
	'''     </tr>
	'''     <tr>
	'''       <td style="vertical-align: middle;">
	'''              <code>HOUR, HOUR_OF_DAY, MINUTE, SECOND, MILLISECOND<br></code>
	'''       </td>
	'''       <td style="vertical-align: middle;">
	'''              <code>0<br></code>
	'''       </td>
	'''     </tr>
	'''   </tbody>
	''' </table>
	''' <br>Default values are not applicable for the fields not listed above.
	''' 
	''' <p>
	''' <strong>Example:</strong>
	''' <blockquote>
	''' <pre>
	''' // get the supported ids for GMT-08:00 (Pacific Standard Time)
	''' String[] ids = TimeZone.getAvailableIDs(-8 * 60 * 60 * 1000);
	''' // if no ids were returned, something is wrong. get out.
	''' if (ids.length == 0)
	'''     System.exit(0);
	''' 
	'''  // begin output
	''' System.out.println("Current Time");
	''' 
	''' // create a Pacific Standard Time time zone
	''' SimpleTimeZone pdt = new SimpleTimeZone(-8 * 60 * 60 * 1000, ids[0]);
	''' 
	''' // set up rules for Daylight Saving Time
	''' pdt.setStartRule(Calendar.APRIL, 1, Calendar.SUNDAY, 2 * 60 * 60 * 1000);
	''' pdt.setEndRule(Calendar.OCTOBER, -1, Calendar.SUNDAY, 2 * 60 * 60 * 1000);
	''' 
	''' // create a GregorianCalendar with the Pacific Daylight time zone
	''' // and the current date and time
	''' Calendar calendar = new GregorianCalendar(pdt);
	''' Date trialTime = new Date();
	''' calendar.setTime(trialTime);
	''' 
	''' // print out a bunch of interesting things
	''' System.out.println("ERA: " + calendar.get(Calendar.ERA));
	''' System.out.println("YEAR: " + calendar.get(Calendar.YEAR));
	''' System.out.println("MONTH: " + calendar.get(Calendar.MONTH));
	''' System.out.println("WEEK_OF_YEAR: " + calendar.get(Calendar.WEEK_OF_YEAR));
	''' System.out.println("WEEK_OF_MONTH: " + calendar.get(Calendar.WEEK_OF_MONTH));
	''' System.out.println("DATE: " + calendar.get(Calendar.DATE));
	''' System.out.println("DAY_OF_MONTH: " + calendar.get(Calendar.DAY_OF_MONTH));
	''' System.out.println("DAY_OF_YEAR: " + calendar.get(Calendar.DAY_OF_YEAR));
	''' System.out.println("DAY_OF_WEEK: " + calendar.get(Calendar.DAY_OF_WEEK));
	''' System.out.println("DAY_OF_WEEK_IN_MONTH: "
	'''                    + calendar.get(Calendar.DAY_OF_WEEK_IN_MONTH));
	''' System.out.println("AM_PM: " + calendar.get(Calendar.AM_PM));
	''' System.out.println("HOUR: " + calendar.get(Calendar.HOUR));
	''' System.out.println("HOUR_OF_DAY: " + calendar.get(Calendar.HOUR_OF_DAY));
	''' System.out.println("MINUTE: " + calendar.get(Calendar.MINUTE));
	''' System.out.println("SECOND: " + calendar.get(Calendar.SECOND));
	''' System.out.println("MILLISECOND: " + calendar.get(Calendar.MILLISECOND));
	''' System.out.println("ZONE_OFFSET: "
	'''                    + (calendar.get(Calendar.ZONE_OFFSET)/(60*60*1000)));
	''' System.out.println("DST_OFFSET: "
	'''                    + (calendar.get(Calendar.DST_OFFSET)/(60*60*1000)));
	''' 
	''' System.out.println("Current Time, with hour reset to 3");
	''' calendar.clear(Calendar.HOUR_OF_DAY); // so doesn't override
	''' calendar.set(Calendar.HOUR, 3);
	''' System.out.println("ERA: " + calendar.get(Calendar.ERA));
	''' System.out.println("YEAR: " + calendar.get(Calendar.YEAR));
	''' System.out.println("MONTH: " + calendar.get(Calendar.MONTH));
	''' System.out.println("WEEK_OF_YEAR: " + calendar.get(Calendar.WEEK_OF_YEAR));
	''' System.out.println("WEEK_OF_MONTH: " + calendar.get(Calendar.WEEK_OF_MONTH));
	''' System.out.println("DATE: " + calendar.get(Calendar.DATE));
	''' System.out.println("DAY_OF_MONTH: " + calendar.get(Calendar.DAY_OF_MONTH));
	''' System.out.println("DAY_OF_YEAR: " + calendar.get(Calendar.DAY_OF_YEAR));
	''' System.out.println("DAY_OF_WEEK: " + calendar.get(Calendar.DAY_OF_WEEK));
	''' System.out.println("DAY_OF_WEEK_IN_MONTH: "
	'''                    + calendar.get(Calendar.DAY_OF_WEEK_IN_MONTH));
	''' System.out.println("AM_PM: " + calendar.get(Calendar.AM_PM));
	''' System.out.println("HOUR: " + calendar.get(Calendar.HOUR));
	''' System.out.println("HOUR_OF_DAY: " + calendar.get(Calendar.HOUR_OF_DAY));
	''' System.out.println("MINUTE: " + calendar.get(Calendar.MINUTE));
	''' System.out.println("SECOND: " + calendar.get(Calendar.SECOND));
	''' System.out.println("MILLISECOND: " + calendar.get(Calendar.MILLISECOND));
	''' System.out.println("ZONE_OFFSET: "
	'''        + (calendar.get(Calendar.ZONE_OFFSET)/(60*60*1000))); // in hours
	''' System.out.println("DST_OFFSET: "
	'''        + (calendar.get(Calendar.DST_OFFSET)/(60*60*1000))); // in hours
	''' </pre>
	''' </blockquote>
	''' </summary>
	''' <seealso cref=          TimeZone
	''' @author David Goldsmith, Mark Davis, Chen-Lieh Huang, Alan Liu
	''' @since JDK1.1 </seealso>
	Public Class GregorianCalendar
		Inherits Calendar

	'    
	'     * Implementation Notes
	'     *
	'     * The epoch is the number of days or milliseconds from some defined
	'     * starting point. The epoch for java.util.Date is used here; that is,
	'     * milliseconds from January 1, 1970 (Gregorian), midnight UTC.  Other
	'     * epochs which are used are January 1, year 1 (Gregorian), which is day 1
	'     * of the Gregorian calendar, and December 30, year 0 (Gregorian), which is
	'     * day 1 of the Julian calendar.
	'     *
	'     * We implement the proleptic Julian and Gregorian calendars.  This means we
	'     * implement the modern definition of the calendar even though the
	'     * historical usage differs.  For example, if the Gregorian change is set
	'     * to new Date(Long.MIN_VALUE), we have a pure Gregorian calendar which
	'     * labels dates preceding the invention of the Gregorian calendar in 1582 as
	'     * if the calendar existed then.
	'     *
	'     * Likewise, with the Julian calendar, we assume a consistent
	'     * 4-year leap year rule, even though the historical pattern of
	'     * leap years is irregular, being every 3 years from 45 BCE
	'     * through 9 BCE, then every 4 years from 8 CE onwards, with no
	'     * leap years in-between.  Thus date computations and functions
	'     * such as isLeapYear() are not intended to be historically
	'     * accurate.
	'     

	'////////////////
	' Class Variables
	'////////////////

		''' <summary>
		''' Value of the <code>ERA</code> field indicating
		''' the period before the common era (before Christ), also known as BCE.
		''' The sequence of years at the transition from <code>BC</code> to <code>AD</code> is
		''' ..., 2 BC, 1 BC, 1 AD, 2 AD,...
		''' </summary>
		''' <seealso cref= #ERA </seealso>
		Public Const BC As Integer = 0

		''' <summary>
		''' Value of the <seealso cref="#ERA"/> field indicating
		''' the period before the common era, the same value as <seealso cref="#BC"/>.
		''' </summary>
		''' <seealso cref= #CE </seealso>
		Friend Const BCE As Integer = 0

		''' <summary>
		''' Value of the <code>ERA</code> field indicating
		''' the common era (Anno Domini), also known as CE.
		''' The sequence of years at the transition from <code>BC</code> to <code>AD</code> is
		''' ..., 2 BC, 1 BC, 1 AD, 2 AD,...
		''' </summary>
		''' <seealso cref= #ERA </seealso>
		Public Const AD As Integer = 1

		''' <summary>
		''' Value of the <seealso cref="#ERA"/> field indicating
		''' the common era, the same value as <seealso cref="#AD"/>.
		''' </summary>
		''' <seealso cref= #BCE </seealso>
		Friend Const CE As Integer = 1

		Private Const EPOCH_OFFSET As Integer = 719163 ' Fixed date of January 1, 1970 (Gregorian)
		Private Const EPOCH_YEAR As Integer = 1970

		Friend Shared ReadOnly MONTH_LENGTH As Integer() = {31,28,31,30,31,30,31,31,30,31,30,31} ' 0-based
		Friend Shared ReadOnly LEAP_MONTH_LENGTH As Integer() = {31,29,31,30,31,30,31,31,30,31,30,31} ' 0-based

		' Useful millisecond constants.  Although ONE_DAY and ONE_WEEK can fit
		' into ints, they must be longs in order to prevent arithmetic overflow
		' when performing (bug 4173516).
		Private Const ONE_SECOND As Integer = 1000
		Private Shared ReadOnly ONE_MINUTE As Integer = 60*ONE_SECOND
		Private Shared ReadOnly ONE_HOUR As Integer = 60*ONE_MINUTE
		Private Shared ReadOnly ONE_DAY As Long = 24*ONE_HOUR
		Private Shared ReadOnly ONE_WEEK As Long = 7*ONE_DAY

	'    
	'     * <pre>
	'     *                            Greatest       Least
	'     * Field name        Minimum   Minimum     Maximum     Maximum
	'     * ----------        -------   -------     -------     -------
	'     * ERA                     0         0           1           1
	'     * YEAR                    1         1   292269054   292278994
	'     * MONTH                   0         0          11          11
	'     * WEEK_OF_YEAR            1         1          52*         53
	'     * WEEK_OF_MONTH           0         0           4*          6
	'     * DAY_OF_MONTH            1         1          28*         31
	'     * DAY_OF_YEAR             1         1         365*        366
	'     * DAY_OF_WEEK             1         1           7           7
	'     * DAY_OF_WEEK_IN_MONTH   -1        -1           4*          6
	'     * AM_PM                   0         0           1           1
	'     * HOUR                    0         0          11          11
	'     * HOUR_OF_DAY             0         0          23          23
	'     * MINUTE                  0         0          59          59
	'     * SECOND                  0         0          59          59
	'     * MILLISECOND             0         0         999         999
	'     * ZONE_OFFSET        -13:00    -13:00       14:00       14:00
	'     * DST_OFFSET           0:00      0:00        0:20        2:00
	'     * </pre>
	'     * *: depends on the Gregorian change date
	'     
		Friend Shared ReadOnly MIN_VALUES As Integer() = { BCE, 1, JANUARY, 1, 0, 1, 1, SUNDAY, 1, AM, 0, 0, 0, 0, 0, -13*ONE_HOUR, 0 }
		Friend Shared ReadOnly LEAST_MAX_VALUES As Integer() = { CE, 292269054, DECEMBER, 52, 4, 28, 365, SATURDAY, 4, PM, 11, 23, 59, 59, 999, 14*ONE_HOUR, 20*ONE_MINUTE }
		Friend Shared ReadOnly MAX_VALUES As Integer() = { CE, 292278994, DECEMBER, 53, 6, 31, 366, SATURDAY, 6, PM, 11, 23, 59, 59, 999, 14*ONE_HOUR, 2*ONE_HOUR }

		' Proclaim serialization compatibility with JDK 1.1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shadows Const serialVersionUID As Long = -8125100834729963327L

		' Reference to the sun.util.calendar.Gregorian instance (singleton).
		Private Shared ReadOnly gcal As sun.util.calendar.Gregorian = sun.util.calendar.CalendarSystem.gregorianCalendar

		' Reference to the JulianCalendar instance (singleton), set as needed. See
		' getJulianCalendarSystem().
		Private Shared jcal As sun.util.calendar.JulianCalendar

		' JulianCalendar eras. See getJulianCalendarSystem().
		Private Shared jeras As sun.util.calendar.Era()

		' The default value of gregorianCutover.
		Friend Const DEFAULT_GREGORIAN_CUTOVER As Long = -12219292800000L

	'///////////////////
	' Instance Variables
	'///////////////////

		''' <summary>
		''' The point at which the Gregorian calendar rules are used, measured in
		''' milliseconds from the standard epoch.  Default is October 15, 1582
		''' (Gregorian) 00:00:00 UTC or -12219292800000L.  For this value, October 4,
		''' 1582 (Julian) is followed by October 15, 1582 (Gregorian).  This
		''' corresponds to Julian day number 2299161.
		''' @serial
		''' </summary>
		Private gregorianCutover As Long = DEFAULT_GREGORIAN_CUTOVER

		''' <summary>
		''' The fixed date of the gregorianCutover.
		''' </summary>
		<NonSerialized> _
		Private gregorianCutoverDate As Long = (((DEFAULT_GREGORIAN_CUTOVER + 1)\ONE_DAY) - 1) + EPOCH_OFFSET ' == 577736

		''' <summary>
		''' The normalized year of the gregorianCutover in Gregorian, with
		''' 0 representing 1 BCE, -1 representing 2 BCE, etc.
		''' </summary>
		<NonSerialized> _
		Private gregorianCutoverYear As Integer = 1582

		''' <summary>
		''' The normalized year of the gregorianCutover in Julian, with 0
		''' representing 1 BCE, -1 representing 2 BCE, etc.
		''' </summary>
		<NonSerialized> _
		Private gregorianCutoverYearJulian As Integer = 1582

		''' <summary>
		''' gdate always has a sun.util.calendar.Gregorian.Date instance to
		''' avoid overhead of creating it. The assumption is that most
		''' applications will need only Gregorian calendar calculations.
		''' </summary>
		<NonSerialized> _
		Private gdate As sun.util.calendar.BaseCalendar.Date

		''' <summary>
		''' Reference to either gdate or a JulianCalendar.Date
		''' instance. After calling complete(), this value is guaranteed to
		''' be set.
		''' </summary>
		<NonSerialized> _
		Private [cdate] As sun.util.calendar.BaseCalendar.Date

		''' <summary>
		''' The CalendarSystem used to calculate the date in cdate. After
		''' calling complete(), this value is guaranteed to be set and
		''' consistent with the cdate value.
		''' </summary>
		<NonSerialized> _
		Private calsys As sun.util.calendar.BaseCalendar

		''' <summary>
		''' Temporary int[2] to get time zone offsets. zoneOffsets[0] gets
		''' the GMT offset value and zoneOffsets[1] gets the DST saving
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

	'/////////////
	' Constructors
	'/////////////

		''' <summary>
		''' Constructs a default <code>GregorianCalendar</code> using the current time
		''' in the default time zone with the default
		''' <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		''' </summary>
		Public Sub New()
			Me.New(TimeZone.defaultRef, Locale.getDefault(Locale.Category.FORMAT))
			zoneShared = True
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> based on the current time
		''' in the given time zone with the default
		''' <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		''' </summary>
		''' <param name="zone"> the given time zone. </param>
		Public Sub New(  zone As TimeZone)
			Me.New(zone, Locale.getDefault(Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> based on the current time
		''' in the default time zone with the given locale.
		''' </summary>
		''' <param name="aLocale"> the given locale. </param>
		Public Sub New(  aLocale As Locale)
			Me.New(TimeZone.defaultRef, aLocale)
			zoneShared = True
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> based on the current time
		''' in the given time zone with the given locale.
		''' </summary>
		''' <param name="zone"> the given time zone. </param>
		''' <param name="aLocale"> the given locale. </param>
		Public Sub New(  zone As TimeZone,   aLocale As Locale)
			MyBase.New(zone, aLocale)
			gdate = CType(gcal.newCalendarDate(zone), sun.util.calendar.BaseCalendar.Date)
			timeInMillis = System.currentTimeMillis()
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> with the given date set
		''' in the default time zone with the default locale.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		Public Sub New(  year As Integer,   month As Integer,   dayOfMonth As Integer)
			Me.New(year, month, dayOfMonth, 0, 0, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> with the given date
		''' and time set for the default time zone with the default locale.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		''' <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		''' in the calendar. </param>
		''' <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		''' in the calendar. </param>
		Public Sub New(  year As Integer,   month As Integer,   dayOfMonth As Integer,   hourOfDay As Integer,   minute As Integer)
			Me.New(year, month, dayOfMonth, hourOfDay, minute, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a GregorianCalendar with the given date
		''' and time set for the default time zone with the default locale.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		''' <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		''' in the calendar. </param>
		''' <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		''' in the calendar. </param>
		''' <param name="second"> the value used to set the <code>SECOND</code> calendar field
		''' in the calendar. </param>
		Public Sub New(  year As Integer,   month As Integer,   dayOfMonth As Integer,   hourOfDay As Integer,   minute As Integer,   second As Integer)
			Me.New(year, month, dayOfMonth, hourOfDay, minute, second, 0)
		End Sub

		''' <summary>
		''' Constructs a <code>GregorianCalendar</code> with the given date
		''' and time set for the default time zone with the default locale.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field in the calendar. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field in the calendar.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="dayOfMonth"> the value used to set the <code>DAY_OF_MONTH</code> calendar field in the calendar. </param>
		''' <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field
		''' in the calendar. </param>
		''' <param name="minute"> the value used to set the <code>MINUTE</code> calendar field
		''' in the calendar. </param>
		''' <param name="second"> the value used to set the <code>SECOND</code> calendar field
		''' in the calendar. </param>
		''' <param name="millis"> the value used to set the <code>MILLISECOND</code> calendar field </param>
		Friend Sub New(  year As Integer,   month As Integer,   dayOfMonth As Integer,   hourOfDay As Integer,   minute As Integer,   second As Integer,   millis As Integer)
			MyBase.New()
			gdate = CType(gcal.newCalendarDate(zone), sun.util.calendar.BaseCalendar.Date)
			Me.set(GregorianCalendar.YEAR, year)
			Me.set(GregorianCalendar.MONTH, month)
			Me.set(DAY_OF_MONTH, dayOfMonth)

			' Set AM_PM and HOUR here to set their stamp values before
			' setting HOUR_OF_DAY (6178071).
			If hourOfDay >= 12 AndAlso hourOfDay <= 23 Then
				' If hourOfDay is a valid PM hour, set the correct PM values
				' so that it won't throw an exception in case it's set to
				' non-lenient later.
				Me.internalSet(AM_PM, PM)
				Me.internalSet(HOUR, hourOfDay - 12)
			Else
				' The default value for AM_PM is AM.
				' We don't care any out of range value here for leniency.
				Me.internalSet(HOUR, hourOfDay)
			End If
			' The stamp values of AM_PM and HOUR must be COMPUTED. (6440854)
			fieldsComputed = HOUR_MASK Or AM_PM_MASK

			Me.set(HOUR_OF_DAY, hourOfDay)
			Me.set(GregorianCalendar.MINUTE, minute)
			Me.set(GregorianCalendar.SECOND, second)
			' should be changed to set() when this constructor is made
			' public.
			Me.internalSet(MILLISECOND, millis)
		End Sub

		''' <summary>
		''' Constructs an empty GregorianCalendar.
		''' </summary>
		''' <param name="zone">    the given time zone </param>
		''' <param name="aLocale"> the given locale </param>
		''' <param name="flag">    the flag requesting an empty instance </param>
		Friend Sub New(  zone As TimeZone,   locale_Renamed As Locale,   flag As Boolean)
			MyBase.New(zone, locale_Renamed)
			gdate = CType(gcal.newCalendarDate(zone), sun.util.calendar.BaseCalendar.Date)
		End Sub

	'///////////////
	' Public methods
	'///////////////

		''' <summary>
		''' Sets the <code>GregorianCalendar</code> change date. This is the point when the switch
		''' from Julian dates to Gregorian dates occurred. Default is October 15,
		''' 1582 (Gregorian). Previous to this, dates will be in the Julian calendar.
		''' <p>
		''' To obtain a pure Julian calendar, set the change date to
		''' <code>Date(Long.MAX_VALUE)</code>.  To obtain a pure Gregorian calendar,
		''' set the change date to <code>Date(Long.MIN_VALUE)</code>.
		''' </summary>
		''' <param name="date"> the given Gregorian cutover date. </param>
		Public Overridable Property gregorianChange As Date
			Set(  [date] As Date)
				Dim cutoverTime As Long = date_Renamed.time
				If cutoverTime = gregorianCutover Then Return
				' Before changing the cutover date, make sure to have the
				' time of this calendar.
				complete()
				gregorianChange = cutoverTime
			End Set
			Get
				Return New Date(gregorianCutover)
			End Get
		End Property

		Private Property gregorianChange As Long
			Set(  cutoverTime As Long)
				gregorianCutover = cutoverTime
				gregorianCutoverDate = sun.util.calendar.CalendarUtils.floorDivide(cutoverTime, ONE_DAY) + EPOCH_OFFSET
    
				' To provide the "pure" Julian calendar as advertised.
				' Strictly speaking, the last millisecond should be a
				' Gregorian date. However, the API doc specifies that setting
				' the cutover date to java.lang.[Long].MAX_VALUE will make this calendar
				' a pure Julian calendar. (See 4167995)
				If cutoverTime = java.lang.[Long].Max_Value Then gregorianCutoverDate += 1
    
				Dim d As sun.util.calendar.BaseCalendar.Date = gregorianCutoverDate
    
				' Set the cutover year (in the Gregorian year numbering)
				gregorianCutoverYear = d.year
    
				Dim julianCal As sun.util.calendar.BaseCalendar = julianCalendarSystem
				d = CType(julianCal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
				julianCal.getCalendarDateFromFixedDate(d, gregorianCutoverDate - 1)
				gregorianCutoverYearJulian = d.normalizedYear
    
				If time < gregorianCutover Then unnormalizedzed()
			End Set
		End Property


		''' <summary>
		''' Determines if the given year is a leap year. Returns <code>true</code> if
		''' the given year is a leap year. To specify BC year numbers,
		''' <code>1 - year number</code> must be given. For example, year BC 4 is
		''' specified as -3.
		''' </summary>
		''' <param name="year"> the given year. </param>
		''' <returns> <code>true</code> if the given year is a leap year; <code>false</code> otherwise. </returns>
		Public Overridable Function isLeapYear(  year As Integer) As Boolean
			If (year And 3) <> 0 Then Return False

			If year > gregorianCutoverYear Then Return (year Mod 100 <> 0) OrElse (year Mod 400 = 0) ' Gregorian
			If year < gregorianCutoverYearJulian Then Return True ' Julian
			Dim gregorian As Boolean
			' If the given year is the Gregorian cutover year, we need to
			' determine which calendar system to be applied to February in the year.
			If gregorianCutoverYear = gregorianCutoverYearJulian Then
				Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(gregorianCutoverDate) ' Gregorian
				gregorian = d.month < sun.util.calendar.BaseCalendar.MARCH
			Else
				gregorian = year = gregorianCutoverYear
			End If
			Return If(gregorian, (year Mod 100 <> 0) OrElse (year Mod 400 = 0), True)
		End Function

		''' <summary>
		''' Returns {@code "gregory"} as the calendar type.
		''' </summary>
		''' <returns> {@code "gregory"}
		''' @since 1.8 </returns>
		Public  Overrides ReadOnly Property  calendarType As String
			Get
				Return "gregory"
			End Get
		End Property

		''' <summary>
		''' Compares this <code>GregorianCalendar</code> to the specified
		''' <code>Object</code>. The result is <code>true</code> if and
		''' only if the argument is a <code>GregorianCalendar</code> object
		''' that represents the same time value (millisecond offset from
		''' the <a href="Calendar.html#Epoch">Epoch</a>) under the same
		''' <code>Calendar</code> parameters and Gregorian change date as
		''' this object.
		''' </summary>
		''' <param name="obj"> the object to compare with. </param>
		''' <returns> <code>true</code> if this object is equal to <code>obj</code>;
		''' <code>false</code> otherwise. </returns>
		''' <seealso cref= Calendar#compareTo(Calendar) </seealso>
		Public Overrides Function Equals(  obj As Object) As Boolean
			Return TypeOf obj Is GregorianCalendar AndAlso MyBase.Equals(obj) AndAlso gregorianCutover = CType(obj, GregorianCalendar).gregorianCutover
		End Function

		''' <summary>
		''' Generates the hash code for this <code>GregorianCalendar</code> object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode() Xor CInt(gregorianCutoverDate)
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
		Public Overrides Sub add(  field As Integer,   amount As Integer)
			' If amount == 0, do nothing even the given field is out of
			' range. This is tested by JCK.
			If amount = 0 Then Return ' Do nothing!

			If field < 0 OrElse field >= ZONE_OFFSET Then Throw New IllegalArgumentException

			' Sync the time and calendar fields.
			complete()

			If field = YEAR Then
				Dim year_Renamed As Integer = internalGet(YEAR)
				If internalGetEra() = CE Then
					year_Renamed += amount
					If year_Renamed > 0 Then
						[set](YEAR, year_Renamed) ' year <= 0
					Else
						[set](YEAR, 1 - year_Renamed)
						' if year == 0, you get 1 BCE.
						[set](ERA, BCE)
					End If
				Else ' era == BCE
					year_Renamed -= amount
					If year_Renamed > 0 Then
						[set](YEAR, year_Renamed) ' year <= 0
					Else
						[set](YEAR, 1 - year_Renamed)
						' if year == 0, you get 1 CE
						[set](ERA, CE)
					End If
				End If
				pinDayOfMonth()
			ElseIf field = MONTH Then
				Dim month_Renamed As Integer = internalGet(MONTH) + amount
				Dim year_Renamed As Integer = internalGet(YEAR)
				Dim y_amount As Integer

				If month_Renamed >= 0 Then
					y_amount = month_Renamed\12
				Else
					y_amount = (month_Renamed+1)\12 - 1
				End If
				If y_amount <> 0 Then
					If internalGetEra() = CE Then
						year_Renamed += y_amount
						If year_Renamed > 0 Then
							[set](YEAR, year_Renamed) ' year <= 0
						Else
							[set](YEAR, 1 - year_Renamed)
							' if year == 0, you get 1 BCE
							[set](ERA, BCE)
						End If
					Else ' era == BCE
						year_Renamed -= y_amount
						If year_Renamed > 0 Then
							[set](YEAR, year_Renamed) ' year <= 0
						Else
							[set](YEAR, 1 - year_Renamed)
							' if year == 0, you get 1 CE
							[set](ERA, CE)
						End If
					End If
				End If

				If month_Renamed >= 0 Then
					[set](MONTH, month Mod 12)
				Else
					' month < 0
					month_Renamed = month Mod 12
					If month_Renamed < 0 Then month_Renamed += 12
					[set](MONTH, JANUARY + month_Renamed)
				End If
				pinDayOfMonth()
			ElseIf field = ERA Then
				Dim era_Renamed As Integer = internalGet(ERA) + amount
				If era_Renamed < 0 Then era_Renamed = 0
				If era_Renamed > 1 Then era_Renamed = 1
				[set](ERA, era_Renamed)
			Else
				Dim delta As Long = amount
				Dim timeOfDay As Long = 0
				Select Case field
				' Handle the time fields here. Convert the given
				' amount to milliseconds and call setTimeInMillis.
				Case HOUR, HOUR_OF_DAY
					delta *= 60 * 60 * 1000 ' hours to minutes

				Case MINUTE
					delta *= 60 * 1000 ' minutes to seconds

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
				Dim fd As Long = currentFixedDate
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
					Dim fd2 As Long = currentFixedDate
					' If the adjustment has changed the date, then take
					' the previous one.
					If fd2 <> fd Then timeInMillis = time - zoneOffset
				End If
			End If
		End Sub

		''' <summary>
		''' Adds or subtracts (up/down) a single unit of time on the given time
		''' field without changing larger fields.
		''' <p>
		''' <em>Example</em>: Consider a <code>GregorianCalendar</code>
		''' originally set to December 31, 1999. Calling <seealso cref="#roll(int,boolean) roll(Calendar.MONTH, true)"/>
		''' sets the calendar to January 31, 1999.  The <code>YEAR</code> field is unchanged
		''' because it is a larger field than <code>MONTH</code>.</p>
		''' </summary>
		''' <param name="up"> indicates if the value of the specified calendar field is to be
		''' rolled up or rolled down. Use <code>true</code> if rolling up, <code>false</code> otherwise. </param>
		''' <exception cref="IllegalArgumentException"> if <code>field</code> is
		''' <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		''' or if any calendar fields have out-of-range values in
		''' non-lenient mode. </exception>
		''' <seealso cref= #add(int,int) </seealso>
		''' <seealso cref= #set(int,int) </seealso>
		Public Overrides Sub roll(  field As Integer,   up As Boolean)
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
		''' 
		''' <p>
		''' <em>Example</em>: Consider a <code>GregorianCalendar</code>
		''' originally set to August 31, 1999. Calling <code>roll(Calendar.MONTH,
		''' 8)</code> sets the calendar to April 30, <strong>1999</strong>. Using a
		''' <code>GregorianCalendar</code>, the <code>DAY_OF_MONTH</code> field cannot
		''' be 31 in the month April. <code>DAY_OF_MONTH</code> is set to the closest possible
		''' value, 30. The <code>YEAR</code> field maintains the value of 1999 because it
		''' is a larger field than <code>MONTH</code>.
		''' <p>
		''' <em>Example</em>: Consider a <code>GregorianCalendar</code>
		''' originally set to Sunday June 6, 1999. Calling
		''' <code>roll(Calendar.WEEK_OF_MONTH, -1)</code> sets the calendar to
		''' Tuesday June 1, 1999, whereas calling
		''' <code>add(Calendar.WEEK_OF_MONTH, -1)</code> sets the calendar to
		''' Sunday May 30, 1999. This is because the roll rule imposes an
		''' additional constraint: The <code>MONTH</code> must not change when the
		''' <code>WEEK_OF_MONTH</code> is rolled. Taken together with add rule 1,
		''' the resultant date must be between Tuesday June 1 and Saturday June
		''' 5. According to add rule 2, the <code>DAY_OF_WEEK</code>, an invariant
		''' when changing the <code>WEEK_OF_MONTH</code>, is set to Tuesday, the
		''' closest possible value to Sunday (where Sunday is the first day of the
		''' week).</p>
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <param name="amount"> the signed amount to add to <code>field</code>. </param>
		''' <exception cref="IllegalArgumentException"> if <code>field</code> is
		''' <code>ZONE_OFFSET</code>, <code>DST_OFFSET</code>, or unknown,
		''' or if any calendar fields have out-of-range values in
		''' non-lenient mode. </exception>
		''' <seealso cref= #roll(int,boolean) </seealso>
		''' <seealso cref= #add(int,int) </seealso>
		''' <seealso cref= #set(int,int)
		''' @since 1.2 </seealso>
		Public Overrides Sub roll(  field As Integer,   amount As Integer)
			' If amount == 0, do nothing even the given field is out of
			' range. This is tested by JCK.
			If amount = 0 Then Return

			If field < 0 OrElse field >= ZONE_OFFSET Then Throw New IllegalArgumentException

			' Sync the time and calendar fields.
			complete()

			Dim min As Integer = getMinimum(field)
			Dim max As Integer = getMaximum(field)

			Select Case field
			Case AM_PM, ERA, YEAR, MINUTE, SECOND, MILLISECOND
				' These fields are handled simply, since they have fixed minima
				' and maxima.  The field DAY_OF_MONTH is almost as simple.  Other
				' fields are complicated, since the range within they must roll
				' varies depending on the date.

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
					Dim d As sun.util.calendar.CalendarDate = calsys.getCalendarDate(time, zone)
					If internalGet(DAY_OF_MONTH) <> d.dayOfMonth Then
						d.dateate(internalGet(YEAR), internalGet(MONTH) + 1, internalGet(DAY_OF_MONTH))
						If field = HOUR Then
							assert(internalGet(AM_PM) = PM)
							d.addHours(+12) ' restore PM
						End If
						time = calsys.getTime(d)
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

			Case MONTH
				' Rolling the month involves both pinning the final value to [0, 11]
				' and adjusting the DAY_OF_MONTH if necessary.  We only adjust the
				' DAY_OF_MONTH if, after updating the MONTH field, it is illegal.
				' E.g., <jan31>.roll(MONTH, 1) -> <feb28> or <feb29>.
					If Not isCutoverYear([cdate].normalizedYear) Then
						Dim mon As Integer = (internalGet(MONTH) + amount) Mod 12
						If mon < 0 Then mon += 12
						[set](MONTH, mon)

						' Keep the day of month in the range.  We don't want to spill over
						' into the next month; e.g., we don't want jan31 + 1 mo -> feb31 ->
						' mar3.
						Dim monthLen As Integer = monthLength(mon)
						If internalGet(DAY_OF_MONTH) > monthLen Then [set](DAY_OF_MONTH, monthLen)
					Else
						' We need to take care of different lengths in
						' year and month due to the cutover.
						Dim yearLength As Integer = getActualMaximum(MONTH) + 1
						Dim mon As Integer = (internalGet(MONTH) + amount) Mod yearLength
						If mon < 0 Then mon += yearLength
						[set](MONTH, mon)
						Dim monthLen As Integer = getActualMaximum(DAY_OF_MONTH)
						If internalGet(DAY_OF_MONTH) > monthLen Then [set](DAY_OF_MONTH, monthLen)
					End If
					Return

			Case WEEK_OF_YEAR
					Dim y As Integer = [cdate].normalizedYear
					max = getActualMaximum(WEEK_OF_YEAR)
					[set](DAY_OF_WEEK, internalGet(DAY_OF_WEEK))
					Dim woy As Integer = internalGet(WEEK_OF_YEAR)
					Dim value As Integer = woy + amount
					If Not isCutoverYear(y) Then
						Dim weekYear_Renamed As Integer = weekYear
						If weekYear_Renamed = y Then
							' If the new value is in between min and max
							' (exclusive), then we can use the value.
							If value > min AndAlso value < max Then
								[set](WEEK_OF_YEAR, value)
								Return
							End If
							Dim fd As Long = currentFixedDate
							' Make sure that the min week has the current DAY_OF_WEEK
							' in the calendar year
							Dim day1 As Long = fd - (7 * (woy - min))
							If calsys.getYearFromFixedDate(day1) <> y Then min += 1

							' Make sure the same thing for the max week
							fd += 7 * (max - internalGet(WEEK_OF_YEAR))
							If calsys.getYearFromFixedDate(fd) <> y Then max -= 1
						Else
							' When WEEK_OF_YEAR and YEAR are out of sync,
							' adjust woy and amount to stay in the calendar year.
							If weekYear_Renamed > y Then
								If amount < 0 Then amount += 1
								woy = max
							Else
								If amount > 0 Then amount -= woy - max
								woy = min
							End If
						End If
						[set](field, getRolledValue(woy, amount, min, max))
						Return
					End If

					' Handle cutover here.
					Dim fd As Long = currentFixedDate
					Dim cal As sun.util.calendar.BaseCalendar
					If gregorianCutoverYear = gregorianCutoverYearJulian Then
						cal = cutoverCalendarSystem
					ElseIf y = gregorianCutoverYear Then
						cal = gcal
					Else
						cal = julianCalendarSystem
					End If
					Dim day1 As Long = fd - (7 * (woy - min))
					' Make sure that the min week has the current DAY_OF_WEEK
					If cal.getYearFromFixedDate(day1) <> y Then min += 1

					' Make sure the same thing for the max week
					fd += 7 * (max - woy)
					cal = If(fd >= gregorianCutoverDate, gcal, julianCalendarSystem)
					If cal.getYearFromFixedDate(fd) <> y Then max -= 1
					' value: the new WEEK_OF_YEAR which must be converted
					' to month and day of month.
					value = getRolledValue(woy, amount, min, max) - 1
					Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(day1 + value * 7)
					[set](MONTH, d.month - 1)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case WEEK_OF_MONTH
					Dim isCutoverYear As Boolean = isCutoverYear([cdate].normalizedYear)
					' dow: relative day of week from first day of week
					Dim dow As Integer = internalGet(DAY_OF_WEEK) - firstDayOfWeek
					If dow < 0 Then dow += 7

					Dim fd As Long = currentFixedDate
					Dim month1 As Long ' fixed date of the first day (usually 1) of the month
					Dim monthLength As Integer ' actual month length
					If isCutoverYear Then
						month1 = getFixedDateMonth1([cdate], fd)
						monthLength = actualMonthLength()
					Else
						month1 = fd - internalGet(DAY_OF_MONTH) + 1
						monthLength = calsys.getMonthLength([cdate])
					End If

					' the first day of week of the month.
					Dim monthDay1st As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(month1 + 6, firstDayOfWeek)
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
					Dim dayOfMonth As Integer
					If isCutoverYear Then
						' If we are in the cutover year, convert nfd to
						' its calendar date and use dayOfMonth.
						Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(nfd)
						dayOfMonth = d.dayOfMonth
					Else
						dayOfMonth = CInt(nfd - month1) + 1
					End If
					[set](DAY_OF_MONTH, dayOfMonth)
					Return

			Case DAY_OF_MONTH
					If Not isCutoverYear([cdate].normalizedYear) Then
						max = calsys.getMonthLength([cdate])
						Exit Select
					End If

					' Cutover year handling
					Dim fd As Long = currentFixedDate
					Dim month1 As Long = getFixedDateMonth1([cdate], fd)
					' It may not be a regular month. Convert the date and range to
					' the relative values, perform the roll, and
					' convert the result back to the rolled date.
					Dim value As Integer = getRolledValue(CInt(fd - month1), amount, 0, actualMonthLength() - 1)
					Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(month1 + value)
					Debug.Assert(d.month-1 = internalGet(MONTH))
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case DAY_OF_YEAR
					max = getActualMaximum(field)
					If Not isCutoverYear([cdate].normalizedYear) Then Exit Select

					' Handle cutover here.
					Dim fd As Long = currentFixedDate
					Dim jan1 As Long = fd - internalGet(DAY_OF_YEAR) + 1
					Dim value As Integer = getRolledValue(CInt(fd - jan1) + 1, amount, min, max)
					Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(jan1 + value - 1)
					[set](MONTH, d.month - 1)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return

			Case DAY_OF_WEEK
					If Not isCutoverYear([cdate].normalizedYear) Then
						' If the week of year is in the same year, we can
						' just change DAY_OF_WEEK.
						Dim weekOfYear As Integer = internalGet(WEEK_OF_YEAR)
						If weekOfYear > 1 AndAlso weekOfYear < 52 Then
							[set](WEEK_OF_YEAR, weekOfYear) ' update stamp[WEEK_OF_YEAR]
							max = SATURDAY
							Exit Select
						End If
					End If

					' We need to handle it in a different way around year
					' boundaries and in the cutover year. Note that
					' changing era and year values violates the roll
					' rule: not changing larger calendar fields...
					amount = amount Mod 7
					If amount = 0 Then Return
					Dim fd As Long = currentFixedDate
					Dim dowFirst As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(fd, firstDayOfWeek)
					fd += amount
					If fd < dowFirst Then
						fd += 7
					ElseIf fd >= dowFirst + 7 Then
						fd -= 7
					End If
					Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(fd)
					[set](ERA, (If(d.normalizedYear <= 0, BCE, CE)))
					[set](d.year, d.month - 1, d.dayOfMonth)
					Return

			Case DAY_OF_WEEK_IN_MONTH
					min = 1 ' after normalized, min should be 1.
					If Not isCutoverYear([cdate].normalizedYear) Then
						Dim dom As Integer = internalGet(DAY_OF_MONTH)
						Dim monthLength As Integer = calsys.getMonthLength([cdate])
						Dim lastDays As Integer = monthLength Mod 7
						max = monthLength \ 7
						Dim x As Integer = (dom - 1) Mod 7
						If x < lastDays Then max += 1
						[set](DAY_OF_WEEK, internalGet(DAY_OF_WEEK))
						Exit Select
					End If

					' Cutover year handling
					Dim fd As Long = currentFixedDate
					Dim month1 As Long = getFixedDateMonth1([cdate], fd)
					Dim monthLength As Integer = actualMonthLength()
					Dim lastDays As Integer = monthLength Mod 7
					max = monthLength \ 7
					Dim x As Integer = CInt(fd - month1) Mod 7
					If x < lastDays Then max += 1
					Dim value As Integer = getRolledValue(internalGet(field), amount, min, max) - 1
					fd = month1 + value * 7 + x
					Dim cal As sun.util.calendar.BaseCalendar = If(fd >= gregorianCutoverDate, gcal, julianCalendarSystem)
					Dim d As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
					cal.getCalendarDateFromFixedDate(d, fd)
					[set](DAY_OF_MONTH, d.dayOfMonth)
					Return
			End Select

			[set](field, getRolledValue(internalGet(field), amount, min, max))
		End Sub

		''' <summary>
		''' Returns the minimum value for the given calendar field of this
		''' <code>GregorianCalendar</code> instance. The minimum value is
		''' defined as the smallest value returned by the {@link
		''' Calendar#get(int) get} method for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getMinimum(  field As Integer) As Integer
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
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getMaximum(  field As Integer) As Integer
			Select Case field
			Case MONTH, DAY_OF_MONTH, DAY_OF_YEAR, WEEK_OF_YEAR, WEEK_OF_MONTH, DAY_OF_WEEK_IN_MONTH, YEAR
					' On or after Gregorian 200-3-1, Julian and Gregorian
					' calendar dates are the same or Gregorian dates are
					' larger (i.e., there is a "gap") after 300-3-1.
					If gregorianCutoverYear > 200 Then Exit Select
					' There might be "overlapping" dates.
					Dim gc As GregorianCalendar = CType(clone(), GregorianCalendar)
					gc.lenient = True
					gc.timeInMillis = gregorianCutover
					Dim v1 As Integer = gc.getActualMaximum(field)
					gc.timeInMillis = gregorianCutover-1
					Dim v2 As Integer = gc.getActualMaximum(field)
					Return System.Math.Max(MAX_VALUES(field), System.Math.Max(v1, v2))
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
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the highest minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getGreatestMinimum(  field As Integer) As Integer
			If field = DAY_OF_MONTH Then
				Dim d As sun.util.calendar.BaseCalendar.Date = gregorianCutoverDate
				Dim mon1 As Long = getFixedDateMonth1(d, gregorianCutoverDate)
				d = getCalendarDate(mon1)
				Return System.Math.Max(MIN_VALUES(field), d.dayOfMonth)
			End If
			Return MIN_VALUES(field)
		End Function

		''' <summary>
		''' Returns the lowest maximum value for the given calendar field
		''' of this <code>GregorianCalendar</code> instance. The lowest
		''' maximum value is defined as the smallest value returned by
		''' <seealso cref="#getActualMaximum(int)"/> for any possible time value,
		''' taking into consideration the current values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the lowest maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public Overrides Function getLeastMaximum(  field As Integer) As Integer
			Select Case field
			Case MONTH, DAY_OF_MONTH, DAY_OF_YEAR, WEEK_OF_YEAR, WEEK_OF_MONTH, DAY_OF_WEEK_IN_MONTH, YEAR
					Dim gc As GregorianCalendar = CType(clone(), GregorianCalendar)
					gc.lenient = True
					gc.timeInMillis = gregorianCutover
					Dim v1 As Integer = gc.getActualMaximum(field)
					gc.timeInMillis = gregorianCutover-1
					Dim v2 As Integer = gc.getActualMaximum(field)
					Return System.Math.Min(LEAST_MAX_VALUES(field), System.Math.Min(v1, v2))
			End Select
			Return LEAST_MAX_VALUES(field)
		End Function

		''' <summary>
		''' Returns the minimum value that this calendar field could have,
		''' taking into consideration the given time value and the current
		''' values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' 
		''' <p>For example, if the Gregorian change date is January 10,
		''' 1970 and the date of this <code>GregorianCalendar</code> is
		''' January 20, 1970, the actual minimum value of the
		''' <code>DAY_OF_MONTH</code> field is 10 because the previous date
		''' of January 10, 1970 is December 27, 1996 (in the Julian
		''' calendar). Therefore, December 28, 1969 to January 9, 1970
		''' don't exist.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the minimum of the given field for the time value of
		''' this <code>GregorianCalendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int)
		''' @since 1.2 </seealso>
		Public Overrides Function getActualMinimum(  field As Integer) As Integer
			If field = DAY_OF_MONTH Then
				Dim gc As GregorianCalendar = normalizedCalendar
				Dim year_Renamed As Integer = gc.cdate.normalizedYear
				If year_Renamed = gregorianCutoverYear OrElse year_Renamed = gregorianCutoverYearJulian Then
					Dim month1 As Long = getFixedDateMonth1(gc.cdate, gc.calsys.getFixedDate(gc.cdate))
					Dim d As sun.util.calendar.BaseCalendar.Date = getCalendarDate(month1)
					Return d.dayOfMonth
				End If
			End If
			Return getMinimum(field)
		End Function

		''' <summary>
		''' Returns the maximum value that this calendar field could have,
		''' taking into consideration the given time value and the current
		''' values of the
		''' <seealso cref="Calendar#getFirstDayOfWeek() getFirstDayOfWeek"/>,
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() getMinimalDaysInFirstWeek"/>,
		''' <seealso cref="#getGregorianChange() getGregorianChange"/> and
		''' <seealso cref="Calendar#getTimeZone() getTimeZone"/> methods.
		''' For example, if the date of this instance is February 1, 2004,
		''' the actual maximum value of the <code>DAY_OF_MONTH</code> field
		''' is 29 because 2004 is a leap year, and if the date of this
		''' instance is February 1, 2005, it's 28.
		''' 
		''' <p>This method calculates the maximum value of {@link
		''' Calendar#WEEK_OF_YEAR WEEK_OF_YEAR} based on the {@link
		''' Calendar#YEAR YEAR} (calendar year) value, not the <a
		''' href="#week_year">week year</a>. Call {@link
		''' #getWeeksInWeekYear()} to get the maximum value of {@code
		''' WEEK_OF_YEAR} in the week year of this {@code GregorianCalendar}.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the maximum of the given field for the time value of
		''' this <code>GregorianCalendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int)
		''' @since 1.2 </seealso>
		Public Overrides Function getActualMaximum(  field As Integer) As Integer
			Dim fieldsForFixedMax As Integer = ERA_MASK Or DAY_OF_WEEK_MASK Or HOUR_MASK Or AM_PM_MASK Or HOUR_OF_DAY_MASK Or MINUTE_MASK Or SECOND_MASK Or MILLISECOND_MASK Or ZONE_OFFSET_MASK Or DST_OFFSET_MASK
			If (fieldsForFixedMax And (1<<field)) <> 0 Then Return getMaximum(field)

			Dim gc As GregorianCalendar = normalizedCalendar
			Dim date_Renamed As sun.util.calendar.BaseCalendar.Date = gc.cdate
			Dim cal As sun.util.calendar.BaseCalendar = gc.calsys
			Dim normalizedYear As Integer = date_Renamed.normalizedYear

			Dim value As Integer = -1
			Select Case field
			Case MONTH
					If Not gc.isCutoverYear(normalizedYear) Then
						value = DECEMBER
						Exit Select
					End If

					' January 1 of the next year may or may not exist.
					Dim nextJan1 As Long
					Do
						normalizedYear += 1
						nextJan1 = gcal.getFixedDate(normalizedYear, sun.util.calendar.BaseCalendar.JANUARY, 1, Nothing)
					Loop While nextJan1 < gregorianCutoverDate
					Dim d As sun.util.calendar.BaseCalendar.Date = CType(date_Renamed.clone(), sun.util.calendar.BaseCalendar.Date)
					cal.getCalendarDateFromFixedDate(d, nextJan1 - 1)
					value = d.month - 1

			Case DAY_OF_MONTH
					value = cal.getMonthLength(date_Renamed)
					If (Not gc.isCutoverYear(normalizedYear)) OrElse date_Renamed.dayOfMonth = value Then Exit Select

					' Handle cutover year.
					Dim fd As Long = gc.currentFixedDate
					If fd >= gregorianCutoverDate Then Exit Select
					Dim monthLength As Integer = gc.actualMonthLength()
					Dim monthEnd As Long = gc.getFixedDateMonth1(gc.cdate, fd) + monthLength - 1
					' Convert the fixed date to its calendar date.
					Dim d As sun.util.calendar.BaseCalendar.Date = gc.getCalendarDate(monthEnd)
					value = d.dayOfMonth

			Case DAY_OF_YEAR
					If Not gc.isCutoverYear(normalizedYear) Then
						value = cal.getYearLength(date_Renamed)
						Exit Select
					End If

					' Handle cutover year.
					Dim jan1 As Long
					If gregorianCutoverYear = gregorianCutoverYearJulian Then
						Dim cocal As sun.util.calendar.BaseCalendar = gc.cutoverCalendarSystem
						jan1 = cocal.getFixedDate(normalizedYear, 1, 1, Nothing)
					ElseIf normalizedYear = gregorianCutoverYearJulian Then
						jan1 = cal.getFixedDate(normalizedYear, 1, 1, Nothing)
					Else
						jan1 = gregorianCutoverDate
					End If
					' January 1 of the next year may or may not exist.
					normalizedYear += 1
					Dim nextJan1 As Long = gcal.getFixedDate(normalizedYear, 1, 1, Nothing)
					If nextJan1 < gregorianCutoverDate Then nextJan1 = gregorianCutoverDate
					Debug.Assert(jan1 <= cal.getFixedDate(date_Renamed.normalizedYear, date_Renamed.month, date_Renamed.dayOfMonth, date_Renamed))
					Debug.Assert(nextJan1 >= cal.getFixedDate(date_Renamed.normalizedYear, date_Renamed.month, date_Renamed.dayOfMonth, date_Renamed))
					value = CInt(nextJan1 - jan1)

			Case WEEK_OF_YEAR
					If Not gc.isCutoverYear(normalizedYear) Then
						' Get the day of week of January 1 of the year
						Dim d As sun.util.calendar.CalendarDate = cal.newCalendarDate(TimeZone.NO_TIMEZONE)
						d.dateate(date_Renamed.year, sun.util.calendar.BaseCalendar.JANUARY, 1)
						Dim dayOfWeek As Integer = cal.getDayOfWeek(d)
						' Normalize the day of week with the firstDayOfWeek value
						dayOfWeek -= firstDayOfWeek
						If dayOfWeek < 0 Then dayOfWeek += 7
						value = 52
						Dim magic As Integer = dayOfWeek + minimalDaysInFirstWeek - 1
						If (magic = 6) OrElse (date_Renamed.leapYear AndAlso (magic = 5 OrElse magic = 12)) Then value += 1
						Exit Select
					End If

					If gc Is Me Then gc = CType(gc.clone(), GregorianCalendar)
					Dim maxDayOfYear As Integer = getActualMaximum(DAY_OF_YEAR)
					gc.set(DAY_OF_YEAR, maxDayOfYear)
					value = gc.get(WEEK_OF_YEAR)
					If internalGet(YEAR) <> gc.weekYear Then
						gc.set(DAY_OF_YEAR, maxDayOfYear - 7)
						value = gc.get(WEEK_OF_YEAR)
					End If

			Case WEEK_OF_MONTH
					If Not gc.isCutoverYear(normalizedYear) Then
						Dim d As sun.util.calendar.CalendarDate = cal.newCalendarDate(Nothing)
						d.dateate(date_Renamed.year, date_Renamed.month, 1)
						Dim dayOfWeek As Integer = cal.getDayOfWeek(d)
						Dim monthLength As Integer = cal.getMonthLength(d)
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
						Exit Select
					End If

					' Cutover year handling
					If gc Is Me Then gc = CType(gc.clone(), GregorianCalendar)
					Dim y As Integer = gc.internalGet(YEAR)
					Dim m As Integer = gc.internalGet(MONTH)
					Do
						value = gc.get(WEEK_OF_MONTH)
						gc.add(WEEK_OF_MONTH, +1)
					Loop While gc.get(YEAR) = y AndAlso gc.get(MONTH) = m

			Case DAY_OF_WEEK_IN_MONTH
					' may be in the Gregorian cutover month
					Dim ndays, dow1 As Integer
					Dim dow As Integer = date_Renamed.dayOfWeek
					If Not gc.isCutoverYear(normalizedYear) Then
						Dim d As sun.util.calendar.BaseCalendar.Date = CType(date_Renamed.clone(), sun.util.calendar.BaseCalendar.Date)
						ndays = cal.getMonthLength(d)
						d.dayOfMonth = 1
						cal.normalize(d)
						dow1 = d.dayOfWeek
					Else
						' Let a cloned GregorianCalendar take care of the cutover cases.
						If gc Is Me Then gc = CType(clone(), GregorianCalendar)
						ndays = gc.actualMonthLength()
						gc.set(DAY_OF_MONTH, gc.getActualMinimum(DAY_OF_MONTH))
						dow1 = gc.get(DAY_OF_WEEK)
					End If
					Dim x As Integer = dow - dow1
					If x < 0 Then x += 7
					ndays -= x
					value = (ndays + 6) \ 7

			Case YEAR
	'             The year computation is no different, in principle, from the
	'             * others, however, the range of possible maxima is large.  In
	'             * addition, the way we know we've exceeded the range is different.
	'             * For these reasons, we use the special case code below to handle
	'             * this field.
	'             *
	'             * The actual maxima for YEAR depend on the type of calendar:
	'             *
	'             *     Gregorian = May 17, 292275056 BCE - Aug 17, 292278994 CE
	'             *     Julian    = Dec  2, 292269055 BCE - Jan  3, 292272993 CE
	'             *     Hybrid    = Dec  2, 292269055 BCE - Aug 17, 292278994 CE
	'             *
	'             * We know we've exceeded the maximum when either the month, date,
	'             * time, or era changes in response to setting the year.  We don't
	'             * check for month, date, and time here because the year and era are
	'             * sufficient to detect an invalid year setting.  NOTE: If code is
	'             * added to check the month and date in the future for some reason,
	'             * Feb 29 must be allowed to shift to Mar 1 when setting the year.
	'             
					If gc Is Me Then gc = CType(clone(), GregorianCalendar)

					' Calculate the millisecond offset from the beginning
					' of the year of this calendar and adjust the max
					' year value if we are beyond the limit in the max
					' year.
					Dim current As Long = gc.yearOffsetInMillis

					If gc.internalGetEra() = CE Then
						gc.timeInMillis = java.lang.[Long].Max_Value
						value = gc.get(YEAR)
						Dim maxEnd As Long = gc.yearOffsetInMillis
						If current > maxEnd Then value -= 1
					Else
						Dim mincal As sun.util.calendar.CalendarSystem = If(gc.timeInMillis >= gregorianCutover, gcal, julianCalendarSystem)
						Dim d As sun.util.calendar.CalendarDate = mincal.getCalendarDate(Long.MIN_VALUE, zone)
						Dim maxEnd As Long = (cal.getDayOfYear(d) - 1) * 24 + d.hours
						maxEnd *= 60
						maxEnd += d.minutes
						maxEnd *= 60
						maxEnd += d.seconds
						maxEnd *= 1000
						maxEnd += d.millis
						value = d.year
						If value <= 0 Then
							Debug.Assert(mincal Is gcal)
							value = 1 - value
						End If
						If current < maxEnd Then value -= 1
					End If

			Case Else
				Throw New ArrayIndexOutOfBoundsException(field)
			End Select
			Return value
		End Function

		''' <summary>
		''' Returns the millisecond offset from the beginning of this
		''' year. This Calendar object must have been normalized.
		''' </summary>
		Private Property yearOffsetInMillis As Long
			Get
				Dim t As Long = (internalGet(DAY_OF_YEAR) - 1) * 24
				t += internalGet(HOUR_OF_DAY)
				t *= 60
				t += internalGet(MINUTE)
				t *= 60
				t += internalGet(SECOND)
				t *= 1000
				Return t + internalGet(MILLISECOND) - (internalGet(ZONE_OFFSET) + internalGet(DST_OFFSET))
			End Get
		End Property

		Public Overrides Function clone() As Object
			Dim other As GregorianCalendar = CType(MyBase.clone(), GregorianCalendar)

			other.gdate = CType(gdate.clone(), sun.util.calendar.BaseCalendar.Date)
			If [cdate] IsNot Nothing Then
				If [cdate] IsNot gdate Then
					other.cdate = CType([cdate].clone(), sun.util.calendar.BaseCalendar.Date)
				Else
					other.cdate = other.gdate
				End If
			End If
			other.originalFields = Nothing
			other.zoneOffsets = Nothing
			Return other
		End Function

		Public  Overrides ReadOnly Property  timeZone As TimeZone
			Get
				Dim zone_Renamed As TimeZone = MyBase.timeZone
				' To share the zone by CalendarDates
				gdate.zone = zone_Renamed
				If [cdate] IsNot Nothing AndAlso [cdate] IsNot gdate Then [cdate].zone = zone_Renamed
				Return zone_Renamed
			End Get
			Set(  zone As TimeZone)
				MyBase.timeZone = zone
				' To share the zone by CalendarDates
				gdate.zone = zone
				If [cdate] IsNot Nothing AndAlso [cdate] IsNot gdate Then [cdate].zone = zone
			End Set
		End Property


		''' <summary>
		''' Returns {@code true} indicating this {@code GregorianCalendar}
		''' supports week dates.
		''' </summary>
		''' <returns> {@code true} (always) </returns>
		''' <seealso cref= #getWeekYear() </seealso>
		''' <seealso cref= #setWeekDate(int,int,int) </seealso>
		''' <seealso cref= #getWeeksInWeekYear()
		''' @since 1.7 </seealso>
		Public Property NotOverridable Overrides weekDateSupported As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns the <a href="#week_year">week year</a> represented by this
		''' {@code GregorianCalendar}. The dates in the weeks between 1 and the
		''' maximum week number of the week year have the same week year value
		''' that may be one year before or after the <seealso cref="Calendar#YEAR YEAR"/>
		''' (calendar year) value.
		''' 
		''' <p>This method calls <seealso cref="Calendar#complete()"/> before
		''' calculating the week year.
		''' </summary>
		''' <returns> the week year represented by this {@code GregorianCalendar}.
		'''         If the <seealso cref="Calendar#ERA ERA"/> value is <seealso cref="#BC"/>, the year is
		'''         represented by 0 or a negative number: BC 1 is 0, BC 2
		'''         is -1, BC 3 is -2, and so on. </returns>
		''' <exception cref="IllegalArgumentException">
		'''         if any of the calendar fields is invalid in non-lenient mode. </exception>
		''' <seealso cref= #isWeekDateSupported() </seealso>
		''' <seealso cref= #getWeeksInWeekYear() </seealso>
		''' <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
		''' <seealso cref= Calendar#getMinimalDaysInFirstWeek()
		''' @since 1.7 </seealso>
		Public  Overrides ReadOnly Property  weekYear As Integer
			Get
				Dim year_Renamed As Integer = [get](YEAR) ' implicitly calls complete()
				If internalGetEra() = BCE Then year_Renamed = 1 - year_Renamed
    
				' Fast path for the Gregorian calendar years that are never
				' affected by the Julian-Gregorian transition
				If year_Renamed > gregorianCutoverYear + 1 Then
					Dim weekOfYear As Integer = internalGet(WEEK_OF_YEAR)
					If internalGet(MONTH) = JANUARY Then
						If weekOfYear >= 52 Then year_Renamed -= 1
					Else
						If weekOfYear = 1 Then year_Renamed += 1
					End If
					Return year_Renamed
				End If
    
				' General (slow) path
				Dim dayOfYear As Integer = internalGet(DAY_OF_YEAR)
				Dim maxDayOfYear As Integer = getActualMaximum(DAY_OF_YEAR)
				Dim minimalDays As Integer = minimalDaysInFirstWeek
    
				' Quickly check the possibility of year adjustments before
				' cloning this GregorianCalendar.
				If dayOfYear > minimalDays AndAlso dayOfYear < (maxDayOfYear - 6) Then Return year_Renamed
    
				' Create a clone to work on the calculation
				Dim cal As GregorianCalendar = CType(clone(), GregorianCalendar)
				cal.lenient = True
				' Use GMT so that intermediate date calculations won't
				' affect the time of day fields.
				cal.timeZone = TimeZone.getTimeZone("GMT")
				' Go to the first day of the year, which is usually January 1.
				cal.set(DAY_OF_YEAR, 1)
				cal.complete()
    
				' Get the first day of the first day-of-week in the year.
				Dim delta As Integer = firstDayOfWeek - cal.get(DAY_OF_WEEK)
				If delta <> 0 Then
					If delta < 0 Then delta += 7
					cal.add(DAY_OF_YEAR, delta)
				End If
				Dim minDayOfYear As Integer = cal.get(DAY_OF_YEAR)
				If dayOfYear < minDayOfYear Then
					If minDayOfYear <= minimalDays Then year_Renamed -= 1
				Else
					cal.set(YEAR, year_Renamed + 1)
					cal.set(DAY_OF_YEAR, 1)
					cal.complete()
					Dim del As Integer = firstDayOfWeek - cal.get(DAY_OF_WEEK)
					If del <> 0 Then
						If del < 0 Then del += 7
						cal.add(DAY_OF_YEAR, del)
					End If
					minDayOfYear = cal.get(DAY_OF_YEAR) - 1
					If minDayOfYear = 0 Then minDayOfYear = 7
					If minDayOfYear >= minimalDays Then
						Dim days As Integer = maxDayOfYear - dayOfYear + 1
						If days <= (7 - minDayOfYear) Then year_Renamed += 1
					End If
				End If
				Return year_Renamed
			End Get
		End Property

		''' <summary>
		''' Sets this {@code GregorianCalendar} to the date given by the
		''' date specifiers - <a href="#week_year">{@code weekYear}</a>,
		''' {@code weekOfYear}, and {@code dayOfWeek}. {@code weekOfYear}
		''' follows the <a href="#week_and_year">{@code WEEK_OF_YEAR}
		''' numbering</a>.  The {@code dayOfWeek} value must be one of the
		''' <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> values: {@link
		''' Calendar#SUNDAY SUNDAY} to <seealso cref="Calendar#SATURDAY SATURDAY"/>.
		''' 
		''' <p>Note that the numeric day-of-week representation differs from
		''' the ISO 8601 standard, and that the {@code weekOfYear}
		''' numbering is compatible with the standard when {@code
		''' getFirstDayOfWeek()} is {@code MONDAY} and {@code
		''' getMinimalDaysInFirstWeek()} is 4.
		''' 
		''' <p>Unlike the {@code set} method, all of the calendar fields
		''' and the instant of time value are calculated upon return.
		''' 
		''' <p>If {@code weekOfYear} is out of the valid week-of-year
		''' range in {@code weekYear}, the {@code weekYear}
		''' and {@code weekOfYear} values are adjusted in lenient
		''' mode, or an {@code IllegalArgumentException} is thrown in
		''' non-lenient mode.
		''' </summary>
		''' <param name="weekYear">    the week year </param>
		''' <param name="weekOfYear">  the week number based on {@code weekYear} </param>
		''' <param name="dayOfWeek">   the day of week value: one of the constants
		'''                    for the <seealso cref="#DAY_OF_WEEK DAY_OF_WEEK"/> field:
		'''                    <seealso cref="Calendar#SUNDAY SUNDAY"/>, ...,
		'''                    <seealso cref="Calendar#SATURDAY SATURDAY"/>. </param>
		''' <exception cref="IllegalArgumentException">
		'''            if any of the given date specifiers is invalid,
		'''            or if any of the calendar fields are inconsistent
		'''            with the given date specifiers in non-lenient mode </exception>
		''' <seealso cref= GregorianCalendar#isWeekDateSupported() </seealso>
		''' <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
		''' <seealso cref= Calendar#getMinimalDaysInFirstWeek()
		''' @since 1.7 </seealso>
		Public Overrides Sub setWeekDate(  weekYear As Integer,   weekOfYear As Integer,   dayOfWeek As Integer)
			If dayOfWeek < SUNDAY OrElse dayOfWeek > SATURDAY Then Throw New IllegalArgumentException("invalid dayOfWeek: " & dayOfWeek)

			' To avoid changing the time of day fields by date
			' calculations, use a clone with the GMT time zone.
			Dim gc As GregorianCalendar = CType(clone(), GregorianCalendar)
			gc.lenient = True
			Dim era_Renamed As Integer = gc.get(ERA)
			gc.clear()
			gc.timeZone = TimeZone.getTimeZone("GMT")
			gc.set(ERA, era_Renamed)
			gc.set(YEAR, weekYear)
			gc.set(WEEK_OF_YEAR, 1)
			gc.set(DAY_OF_WEEK, firstDayOfWeek)
			Dim days As Integer = dayOfWeek - firstDayOfWeek
			If days < 0 Then days += 7
			days += 7 * (weekOfYear - 1)
			If days <> 0 Then
				gc.add(DAY_OF_YEAR, days)
			Else
				gc.complete()
			End If

			If (Not lenient) AndAlso (gc.weekYear <> weekYear OrElse gc.internalGet(WEEK_OF_YEAR) <> weekOfYear OrElse gc.internalGet(DAY_OF_WEEK) <> dayOfWeek) Then Throw New IllegalArgumentException

			[set](ERA, gc.internalGet(ERA))
			[set](YEAR, gc.internalGet(YEAR))
			[set](MONTH, gc.internalGet(MONTH))
			[set](DAY_OF_MONTH, gc.internalGet(DAY_OF_MONTH))

			' to avoid throwing an IllegalArgumentException in
			' non-lenient, set WEEK_OF_YEAR internally
			internalSet(WEEK_OF_YEAR, weekOfYear)
			complete()
		End Sub

		''' <summary>
		''' Returns the number of weeks in the <a href="#week_year">week year</a>
		''' represented by this {@code GregorianCalendar}.
		''' 
		''' <p>For example, if this {@code GregorianCalendar}'s date is
		''' December 31, 2008 with <a href="#iso8601_compatible_setting">the ISO
		''' 8601 compatible setting</a>, this method will return 53 for the
		''' period: December 29, 2008 to January 3, 2010 while {@link
		''' #getActualMaximum(int) getActualMaximum(WEEK_OF_YEAR)} will return
		''' 52 for the period: December 31, 2007 to December 28, 2008.
		''' </summary>
		''' <returns> the number of weeks in the week year. </returns>
		''' <seealso cref= Calendar#WEEK_OF_YEAR </seealso>
		''' <seealso cref= #getWeekYear() </seealso>
		''' <seealso cref= #getActualMaximum(int)
		''' @since 1.7 </seealso>
		Public  Overrides ReadOnly Property  weeksInWeekYear As Integer
			Get
				Dim gc As GregorianCalendar = normalizedCalendar
				Dim weekYear_Renamed As Integer = gc.weekYear
				If weekYear_Renamed = gc.internalGet(YEAR) Then Return gc.getActualMaximum(WEEK_OF_YEAR)
    
				' Use the 2nd week for calculating the max of WEEK_OF_YEAR
				If gc Is Me Then gc = CType(gc.clone(), GregorianCalendar)
				gc.weekDateate(weekYear_Renamed, 2, internalGet(DAY_OF_WEEK))
				Return gc.getActualMaximum(WEEK_OF_YEAR)
			End Get
		End Property

	'///////////////////////////
	' Time => Fields computation
	'///////////////////////////

		''' <summary>
		''' The fixed date corresponding to gdate. If the value is
		''' java.lang.[Long].MIN_VALUE, the fixed date value is unknown. Currently,
		''' Julian calendar dates are not cached.
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
			Dim mask As Integer
			If partiallyNormalized Then
				' Determine which calendar fields need to be computed.
				mask = stateFieldslds
				Dim fieldMask As Integer = (Not mask) And ALL_FIELDS
				' We have to call computTime in case calsys == null in
				' order to set calsys and cdate. (6263644)
				If fieldMask <> 0 OrElse calsys Is Nothing Then
					mask = mask Or computeFields(fieldMask, mask And (ZONE_OFFSET_MASK Or DST_OFFSET_MASK))
					Debug.Assert(mask = ALL_FIELDS)
				End If
			Else
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
		Private Function computeFields(  fieldMask As Integer,   tzMask As Integer) As Integer
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

			Dim era_Renamed As Integer = CE
			Dim year_Renamed As Integer
			If fixedDate_Renamed >= gregorianCutoverDate Then
				' Handle Gregorian dates.
				Debug.Assert(cachedFixedDate = java.lang.[Long].MIN_VALUE OrElse gdate.normalized, "cache control: not normalized")
				Debug.Assert(cachedFixedDate = java.lang.[Long].MIN_VALUE OrElse gcal.getFixedDate(gdate.normalizedYear, gdate.month, gdate.dayOfMonth, gdate) = cachedFixedDate, "cache control: inconsictency" & ", cachedFixedDate=" & cachedFixedDate & ", computed=" & gcal.getFixedDate(gdate.normalizedYear, gdate.month, gdate.dayOfMonth, gdate) & ", date=" & gdate)

				' See if we can use gdate to avoid date calculation.
				If fixedDate_Renamed <> cachedFixedDate Then
					gcal.getCalendarDateFromFixedDate(gdate, fixedDate_Renamed)
					cachedFixedDate = fixedDate_Renamed
				End If

				year_Renamed = gdate.year
				If year_Renamed <= 0 Then
					year_Renamed = 1 - year_Renamed
					era_Renamed = BCE
				End If
				calsys = gcal
				[cdate] = gdate
				Debug.Assert([cdate].dayOfWeek > 0, "dow=" & [cdate].dayOfWeek & ", date=" & [cdate])
			Else
				' Handle Julian calendar dates.
				calsys = julianCalendarSystem
				[cdate] = CType(jcal.newCalendarDate(zone), sun.util.calendar.BaseCalendar.Date)
				jcal.getCalendarDateFromFixedDate([cdate], fixedDate_Renamed)
				Dim e As sun.util.calendar.Era = [cdate].era
				If e Is jeras(0) Then era_Renamed = BCE
				year_Renamed = [cdate].year
			End If

			' Always set the ERA and YEAR values.
			internalSet(ERA, era_Renamed)
			internalSet(YEAR, year_Renamed)
			Dim mask As Integer = fieldMask Or (ERA_MASK Or YEAR_MASK)

			Dim month_Renamed As Integer = [cdate].month - 1 ' 0-based
			Dim dayOfMonth As Integer = [cdate].dayOfMonth

			' Set the basic date fields.
			If (fieldMask And (MONTH_MASK Or DAY_OF_MONTH_MASK Or DAY_OF_WEEK_MASK)) <> 0 Then
				internalSet(MONTH, month_Renamed)
				internalSet(DAY_OF_MONTH, dayOfMonth)
				internalSet(DAY_OF_WEEK, [cdate].dayOfWeek)
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
				Dim normalizedYear As Integer = [cdate].normalizedYear
				Dim fixedDateJan1_Renamed As Long = calsys.getFixedDate(normalizedYear, 1, 1, [cdate])
				Dim dayOfYear As Integer = CInt(fixedDate_Renamed - fixedDateJan1_Renamed) + 1
				Dim fixedDateMonth1_Renamed As Long = fixedDate_Renamed - dayOfMonth + 1
				Dim cutoverGap As Integer = 0
				Dim cutoverYear_Renamed As Integer = If(calsys Is gcal, gregorianCutoverYear, gregorianCutoverYearJulian)
				Dim relativeDayOfMonth As Integer = dayOfMonth - 1

				' If we are in the cutover year, we need some special handling.
				If normalizedYear = cutoverYear_Renamed Then
					' Need to take care of the "missing" days.
					If gregorianCutoverYearJulian <= gregorianCutoverYear Then
						' We need to find out where we are. The cutover
						' gap could even be more than one year.  (One
						' year difference in ~48667 years.)
						fixedDateJan1_Renamed = getFixedDateJan1([cdate], fixedDate_Renamed)
						If fixedDate_Renamed >= gregorianCutoverDate Then fixedDateMonth1_Renamed = getFixedDateMonth1([cdate], fixedDate_Renamed)
					End If
					Dim realDayOfYear As Integer = CInt(fixedDate_Renamed - fixedDateJan1_Renamed) + 1
					cutoverGap = dayOfYear - realDayOfYear
					dayOfYear = realDayOfYear
					relativeDayOfMonth = CInt(fixedDate_Renamed - fixedDateMonth1_Renamed)
				End If
				internalSet(DAY_OF_YEAR, dayOfYear)
				internalSet(DAY_OF_WEEK_IN_MONTH, relativeDayOfMonth \ 7 + 1)

				Dim weekOfYear As Integer = getWeekNumber(fixedDateJan1_Renamed, fixedDate_Renamed)

				' The spec is to calculate WEEK_OF_YEAR in the
				' ISO8601-style. This creates problems, though.
				If weekOfYear = 0 Then
					' If the date belongs to the last week of the
					' previous year, use the week number of "12/31" of
					' the "previous" year. Again, if the previous year is
					' the Gregorian cutover year, we need to take care of
					' it.  Usually the previous day of January 1 is
					' December 31, which is not always true in
					' GregorianCalendar.
					Dim fixedDec31 As Long = fixedDateJan1_Renamed - 1
					Dim prevJan1 As Long = fixedDateJan1_Renamed - 365
					If normalizedYear > (cutoverYear_Renamed + 1) Then
						If sun.util.calendar.CalendarUtils.isGregorianLeapYear(normalizedYear - 1) Then prevJan1 -= 1
					ElseIf normalizedYear <= gregorianCutoverYearJulian Then
						If sun.util.calendar.CalendarUtils.isJulianLeapYear(normalizedYear - 1) Then prevJan1 -= 1
					Else
						Dim calForJan1 As sun.util.calendar.BaseCalendar = calsys
						'int prevYear = normalizedYear - 1;
						Dim prevYear As Integer = getCalendarDate(fixedDec31).normalizedYear
						If prevYear = gregorianCutoverYear Then
							calForJan1 = cutoverCalendarSystem
							If calForJan1 Is jcal Then
								prevJan1 = calForJan1.getFixedDate(prevYear, sun.util.calendar.BaseCalendar.JANUARY, 1, Nothing)
							Else
								prevJan1 = gregorianCutoverDate
								calForJan1 = gcal
							End If
						ElseIf prevYear <= gregorianCutoverYearJulian Then
							calForJan1 = julianCalendarSystem
							prevJan1 = calForJan1.getFixedDate(prevYear, sun.util.calendar.BaseCalendar.JANUARY, 1, Nothing)
						End If
					End If
					weekOfYear = getWeekNumber(prevJan1, fixedDec31)
				Else
					If normalizedYear > gregorianCutoverYear OrElse normalizedYear < (gregorianCutoverYearJulian - 1) Then
						' Regular years
						If weekOfYear >= 52 Then
							Dim nextJan1 As Long = fixedDateJan1_Renamed + 365
							If [cdate].leapYear Then nextJan1 += 1
							Dim nextJan1st As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, firstDayOfWeek)
							Dim ndays As Integer = CInt(nextJan1st - nextJan1)
							If ndays >= minimalDaysInFirstWeek AndAlso fixedDate_Renamed >= (nextJan1st - 7) Then weekOfYear = 1
						End If
					Else
						Dim calForJan1 As sun.util.calendar.BaseCalendar = calsys
						Dim nextYear As Integer = normalizedYear + 1
						If nextYear = (gregorianCutoverYearJulian + 1) AndAlso nextYear < gregorianCutoverYear Then nextYear = gregorianCutoverYear
						If nextYear = gregorianCutoverYear Then calForJan1 = cutoverCalendarSystem

						Dim nextJan1 As Long
						If nextYear > gregorianCutoverYear OrElse gregorianCutoverYearJulian = gregorianCutoverYear OrElse nextYear = gregorianCutoverYearJulian Then
							nextJan1 = calForJan1.getFixedDate(nextYear, sun.util.calendar.BaseCalendar.JANUARY, 1, Nothing)
						Else
							nextJan1 = gregorianCutoverDate
							calForJan1 = gcal
						End If

						Dim nextJan1st As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(nextJan1 + 6, firstDayOfWeek)
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
		Private Function getWeekNumber(  fixedDay1 As Long,   fixedDate As Long) As Integer
			' We can always use `gcal' since Julian and Gregorian are the
			' same thing for this calculation.
			Dim fixedDay1st As Long = sun.util.calendar.Gregorian.getDayOfWeekDateOnOrBefore(fixedDay1 + 6, firstDayOfWeek)
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

			' The year defaults to the epoch start. We don't check
			' fieldMask for YEAR because YEAR is a mandatory field to
			' determine the date.
			Dim year_Renamed As Integer = If(isSet(YEAR), internalGet(YEAR), EPOCH_YEAR)

			Dim era_Renamed As Integer = internalGetEra()
			If era_Renamed = BCE Then
				year_Renamed = 1 - year_Renamed
			ElseIf era_Renamed <> CE Then
				' Even in lenient mode we disallow ERA values other than CE & BCE.
				' (The same normalization rule as add()/roll() could be
				' applied here in lenient mode. But this checking is kept
				' unchanged for compatibility as of 1.5.)
				Throw New IllegalArgumentException("Invalid era")
			End If

			' If year is 0 or negative, we need to set the ERA value later.
			If year_Renamed <= 0 AndAlso (Not isSet(ERA)) Then
				fieldMask = fieldMask Or ERA_MASK
				fieldsComputed = ERA_MASK
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
			calculateFixedDate:
				Dim gfd, jfd As Long
				If year_Renamed > gregorianCutoverYear AndAlso year_Renamed > gregorianCutoverYearJulian Then
					gfd = fixedDate_Renamed + getFixedDate(gcal, year_Renamed, fieldMask)
					If gfd >= gregorianCutoverDate Then
						fixedDate_Renamed = gfd
						GoTo calculateFixedDate
					End If
					jfd = fixedDate_Renamed + getFixedDate(julianCalendarSystem, year_Renamed, fieldMask)
				ElseIf year_Renamed < gregorianCutoverYear AndAlso year_Renamed < gregorianCutoverYearJulian Then
					jfd = fixedDate_Renamed + getFixedDate(julianCalendarSystem, year_Renamed, fieldMask)
					If jfd < gregorianCutoverDate Then
						fixedDate_Renamed = jfd
						GoTo calculateFixedDate
					End If
					gfd = jfd
				Else
					jfd = fixedDate_Renamed + getFixedDate(julianCalendarSystem, year_Renamed, fieldMask)
					gfd = fixedDate_Renamed + getFixedDate(gcal, year_Renamed, fieldMask)
				End If

				' Now we have to determine which calendar date it is.

				' If the date is relative from the beginning of the year
				' in the Julian calendar, then use jfd;
				If isFieldSet(fieldMask, DAY_OF_YEAR) OrElse isFieldSet(fieldMask, WEEK_OF_YEAR) Then
					If gregorianCutoverYear = gregorianCutoverYearJulian Then
						fixedDate_Renamed = jfd
						GoTo calculateFixedDate
					ElseIf year_Renamed = gregorianCutoverYear Then
						fixedDate_Renamed = gfd
						GoTo calculateFixedDate
					End If
				End If

				If gfd >= gregorianCutoverDate Then
					If jfd >= gregorianCutoverDate Then
						fixedDate_Renamed = gfd
					Else
						' The date is in an "overlapping" period. No way
						' to disambiguate it. Determine it using the
						' previous date calculation.
						If calsys Is gcal OrElse calsys Is Nothing Then
							fixedDate_Renamed = gfd
						Else
							fixedDate_Renamed = jfd
						End If
					End If
				Else
					If jfd < gregorianCutoverDate Then
						fixedDate_Renamed = jfd
					Else
						' The date is in a "missing" period.
						If Not lenient Then Throw New IllegalArgumentException("the specified date doesn't exist")
						' Take the Julian date for compatibility, which
						' will produce a Gregorian date.
						fixedDate_Renamed = jfd
					End If
				End If

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
					Dim gmtOffset As Integer = If(isFieldSet(fieldMask, ZONE_OFFSET), internalGet(ZONE_OFFSET), zone_Renamed.rawOffset)
					zone_Renamed.getOffsets(millis - gmtOffset, zoneOffsets)
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
						Dim s As String = originalFields(field) & " -> " & internalGet(field)
						' Restore the original field values
						Array.Copy(originalFields, 0, fields, 0, fields.Length)
						Throw New IllegalArgumentException(getFieldName(field) & ": " & s)
					End If
				Next field
			End If
			fieldsNormalized = mask
		End Sub

		''' <summary>
		''' Computes the fixed date under either the Gregorian or the
		''' Julian calendar, using the given year and the specified calendar fields.
		''' </summary>
		''' <param name="cal"> the CalendarSystem to be used for the date calculation </param>
		''' <param name="year"> the normalized year number, with 0 indicating the
		''' year 1 BCE, -1 indicating 2 BCE, etc. </param>
		''' <param name="fieldMask"> the calendar fields to be used for the date calculation </param>
		''' <returns> the fixed date </returns>
		''' <seealso cref= Calendar#selectFields </seealso>
		Private Function getFixedDate(  cal As sun.util.calendar.BaseCalendar,   year As Integer,   fieldMask As Integer) As Long
			Dim month_Renamed As Integer = JANUARY
			If isFieldSet(fieldMask, MONTH) Then
				' No need to check if MONTH has been set (no isSet(MONTH)
				' call) since its unset value happens to be JANUARY (0).
				month_Renamed = internalGet(MONTH)

				' If the month is out of range, adjust it into range
				If month_Renamed > DECEMBER Then
					year += month_Renamed \ 12
					month_Renamed = month Mod 12
				ElseIf month_Renamed < JANUARY Then
					Dim [rem] As Integer() = New Integer(0){}
					year += sun.util.calendar.CalendarUtils.floorDivide(month_Renamed, 12, [rem])
					month_Renamed = [rem](0)
				End If
			End If

			' Get the fixed date since Jan 1, 1 (Gregorian). We are on
			' the first day of either `month' or January in 'year'.
			Dim fixedDate_Renamed As Long = cal.getFixedDate(year, month_Renamed + 1, 1,If(cal Is gcal, gdate, Nothing))
			If isFieldSet(fieldMask, MONTH) Then
				' Month-based calculations
				If isFieldSet(fieldMask, DAY_OF_MONTH) Then
					' We are on the first day of the month. Just add the
					' offset if DAY_OF_MONTH is set. If the isSet call
					' returns false, that means DAY_OF_MONTH has been
					' selected just because of the selected
					' combination. We don't need to add any since the
					' default value is the 1st.
					If isSet(DAY_OF_MONTH) Then
						' To avoid underflow with DAY_OF_MONTH-1, add
						' DAY_OF_MONTH, then subtract 1.
						fixedDate_Renamed += internalGet(DAY_OF_MONTH)
						fixedDate_Renamed -= 1
					End If
				Else
					If isFieldSet(fieldMask, WEEK_OF_MONTH) Then
						Dim firstDayOfWeek_Renamed As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + 6, firstDayOfWeek)
						' If we have enough days in the first week, then
						' move to the previous week.
						If (firstDayOfWeek_Renamed - fixedDate_Renamed) >= minimalDaysInFirstWeek Then firstDayOfWeek_Renamed -= 7
						If isFieldSet(fieldMask, DAY_OF_WEEK) Then firstDayOfWeek_Renamed = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek_Renamed + 6, internalGet(DAY_OF_WEEK))
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
							fixedDate_Renamed = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + (7 * dowim) - 1, dayOfWeek)
						Else
							' Go to the first day of the next week of
							' the specified week boundary.
							Dim lastDate As Integer = monthLength(month_Renamed, year) + (7 * (dowim + 1))
							' Then, get the day of week date on or before the last date.
							fixedDate_Renamed = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + lastDate - 1, dayOfWeek)
						End If
					End If
				End If
			Else
				If year = gregorianCutoverYear AndAlso cal Is gcal AndAlso fixedDate_Renamed < gregorianCutoverDate AndAlso gregorianCutoverYear <> gregorianCutoverYearJulian Then fixedDate_Renamed = gregorianCutoverDate
				' We are on the first day of the year.
				If isFieldSet(fieldMask, DAY_OF_YEAR) Then
					' Add the offset, then subtract 1. (Make sure to avoid underflow.)
					fixedDate_Renamed += internalGet(DAY_OF_YEAR)
					fixedDate_Renamed -= 1
				Else
					Dim firstDayOfWeek_Renamed As Long = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(fixedDate_Renamed + 6, firstDayOfWeek)
					' If we have enough days in the first week, then move
					' to the previous week.
					If (firstDayOfWeek_Renamed - fixedDate_Renamed) >= minimalDaysInFirstWeek Then firstDayOfWeek_Renamed -= 7
					If isFieldSet(fieldMask, DAY_OF_WEEK) Then
						Dim dayOfWeek As Integer = internalGet(DAY_OF_WEEK)
						If dayOfWeek <> firstDayOfWeek Then firstDayOfWeek_Renamed = sun.util.calendar.BaseCalendar.getDayOfWeekDateOnOrBefore(firstDayOfWeek_Renamed + 6, dayOfWeek)
					End If
					fixedDate_Renamed = firstDayOfWeek_Renamed + 7 * (CLng(internalGet(WEEK_OF_YEAR)) - 1)
				End If
			End If

			Return fixedDate_Renamed
		End Function

		''' <summary>
		''' Returns this object if it's normalized (all fields and time are
		''' in sync). Otherwise, a cloned object is returned after calling
		''' complete() in lenient mode.
		''' </summary>
		Private Property normalizedCalendar As GregorianCalendar
			Get
				Dim gc As GregorianCalendar
				If fullyNormalized Then
					gc = Me
				Else
					' Create a clone and normalize the calendar fields
					gc = CType(Me.clone(), GregorianCalendar)
					gc.lenient = True
					gc.complete()
				End If
				Return gc
			End Get
		End Property

		''' <summary>
		''' Returns the Julian calendar system instance (singleton). 'jcal'
		''' and 'jeras' are set upon the return.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		PrivateShared ReadOnly PropertyjulianCalendarSystem As sun.util.calendar.BaseCalendar
			Get
				If jcal Is Nothing Then
					jcal = CType(sun.util.calendar.CalendarSystem.forName("julian"), sun.util.calendar.JulianCalendar)
					jeras = jcal.eras
				End If
				Return jcal
			End Get
		End Property

		''' <summary>
		''' Returns the calendar system for dates before the cutover date
		''' in the cutover year. If the cutover date is January 1, the
		''' method returns Gregorian. Otherwise, Julian.
		''' </summary>
		Private Property cutoverCalendarSystem As sun.util.calendar.BaseCalendar
			Get
				If gregorianCutoverYearJulian < gregorianCutoverYear Then Return gcal
				Return julianCalendarSystem
			End Get
		End Property

		''' <summary>
		''' Determines if the specified year (normalized) is the Gregorian
		''' cutover year. This object must have been normalized.
		''' </summary>
		Private Function isCutoverYear(  normalizedYear As Integer) As Boolean
			Dim cutoverYear_Renamed As Integer = If(calsys Is gcal, gregorianCutoverYear, gregorianCutoverYearJulian)
			Return normalizedYear = cutoverYear_Renamed
		End Function

		''' <summary>
		''' Returns the fixed date of the first day of the year (usually
		''' January 1) before the specified date.
		''' </summary>
		''' <param name="date"> the date for which the first day of the year is
		''' calculated. The date has to be in the cut-over year (Gregorian
		''' or Julian). </param>
		''' <param name="fixedDate"> the fixed date representation of the date </param>
		Private Function getFixedDateJan1(  [date] As sun.util.calendar.BaseCalendar.Date,   fixedDate As Long) As Long
			Debug.Assert(date_Renamed.normalizedYear = gregorianCutoverYear OrElse date_Renamed.normalizedYear = gregorianCutoverYearJulian)
			If gregorianCutoverYear <> gregorianCutoverYearJulian Then
				If fixedDate >= gregorianCutoverDate Then Return gregorianCutoverDate
			End If
			' January 1 of the normalized year should exist.
			Dim juliancal As sun.util.calendar.BaseCalendar = julianCalendarSystem
			Return juliancal.getFixedDate(date_Renamed.normalizedYear, sun.util.calendar.BaseCalendar.JANUARY, 1, Nothing)
		End Function

		''' <summary>
		''' Returns the fixed date of the first date of the month (usually
		''' the 1st of the month) before the specified date.
		''' </summary>
		''' <param name="date"> the date for which the first day of the month is
		''' calculated. The date has to be in the cut-over year (Gregorian
		''' or Julian). </param>
		''' <param name="fixedDate"> the fixed date representation of the date </param>
		Private Function getFixedDateMonth1(  [date] As sun.util.calendar.BaseCalendar.Date,   fixedDate As Long) As Long
			Debug.Assert(date_Renamed.normalizedYear = gregorianCutoverYear OrElse date_Renamed.normalizedYear = gregorianCutoverYearJulian)
			Dim gCutover As sun.util.calendar.BaseCalendar.Date = gregorianCutoverDate
			If gCutover.month = sun.util.calendar.BaseCalendar.JANUARY AndAlso gCutover.dayOfMonth = 1 Then Return fixedDate - date_Renamed.dayOfMonth + 1

			Dim fixedDateMonth1_Renamed As Long
			' The cutover happened sometime during the year.
			If date_Renamed.month = gCutover.month Then
				' The cutover happened in the month.
				Dim jLastDate As sun.util.calendar.BaseCalendar.Date = lastJulianDate
				If gregorianCutoverYear = gregorianCutoverYearJulian AndAlso gCutover.month = jLastDate.month Then
					' The "gap" fits in the same month.
					fixedDateMonth1_Renamed = jcal.getFixedDate(date_Renamed.normalizedYear, date_Renamed.month, 1, Nothing)
				Else
					' Use the cutover date as the first day of the month.
					fixedDateMonth1_Renamed = gregorianCutoverDate
				End If
			Else
				' The cutover happened before the month.
				fixedDateMonth1_Renamed = fixedDate - date_Renamed.dayOfMonth + 1
			End If

			Return fixedDateMonth1_Renamed
		End Function

		''' <summary>
		''' Returns a CalendarDate produced from the specified fixed date.
		''' </summary>
		''' <param name="fd"> the fixed date </param>
		Private Function getCalendarDate(  fd As Long) As sun.util.calendar.BaseCalendar.Date
			Dim cal As sun.util.calendar.BaseCalendar = If(fd >= gregorianCutoverDate, gcal, julianCalendarSystem)
			Dim d As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
			cal.getCalendarDateFromFixedDate(d, fd)
			Return d
		End Function

		''' <summary>
		''' Returns the Gregorian cutover date as a BaseCalendar.Date. The
		''' date is a Gregorian date.
		''' </summary>
		Private Property gregorianCutoverDate As sun.util.calendar.BaseCalendar.Date
			Get
				Return getCalendarDate(gregorianCutoverDate)
			End Get
		End Property

		''' <summary>
		''' Returns the day before the Gregorian cutover date as a
		''' BaseCalendar.Date. The date is a Julian date.
		''' </summary>
		Private Property lastJulianDate As sun.util.calendar.BaseCalendar.Date
			Get
				Return getCalendarDate(gregorianCutoverDate - 1)
			End Get
		End Property

		''' <summary>
		''' Returns the length of the specified month in the specified
		''' year. The year number must be normalized.
		''' </summary>
		''' <seealso cref= #isLeapYear(int) </seealso>
		Private Function monthLength(  month As Integer,   year As Integer) As Integer
			Return If(isLeapYear(year), LEAP_MONTH_LENGTH(month), MONTH_LENGTH(month))
		End Function

		''' <summary>
		''' Returns the length of the specified month in the year provided
		''' by internalGet(YEAR).
		''' </summary>
		''' <seealso cref= #isLeapYear(int) </seealso>
		Private Function monthLength(  month As Integer) As Integer
			Dim year_Renamed As Integer = internalGet(YEAR)
			If internalGetEra() = BCE Then year_Renamed = 1 - year_Renamed
			Return monthLength(month, year_Renamed)
		End Function

		Private Function actualMonthLength() As Integer
			Dim year_Renamed As Integer = [cdate].normalizedYear
			If year_Renamed <> gregorianCutoverYear AndAlso year_Renamed <> gregorianCutoverYearJulian Then Return calsys.getMonthLength([cdate])
			Dim date_Renamed As sun.util.calendar.BaseCalendar.Date = CType([cdate].clone(), sun.util.calendar.BaseCalendar.Date)
			Dim fd As Long = calsys.getFixedDate(date_Renamed)
			Dim month1 As Long = getFixedDateMonth1(date_Renamed, fd)
			Dim next1 As Long = month1 + calsys.getMonthLength(date_Renamed)
			If next1 < gregorianCutoverDate Then Return CInt(next1 - month1)
			If [cdate] IsNot gdate Then date_Renamed = CType(gcal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
			gcal.getCalendarDateFromFixedDate(date_Renamed, next1)
			next1 = getFixedDateMonth1(date_Renamed, next1)
			Return CInt(next1 - month1)
		End Function

		''' <summary>
		''' Returns the length (in days) of the specified year. The year
		''' must be normalized.
		''' </summary>
		Private Function yearLength(  year As Integer) As Integer
			Return If(isLeapYear(year), 366, 365)
		End Function

		''' <summary>
		''' Returns the length (in days) of the year provided by
		''' internalGet(YEAR).
		''' </summary>
		Private Function yearLength() As Integer
			Dim year_Renamed As Integer = internalGet(YEAR)
			If internalGetEra() = BCE Then year_Renamed = 1 - year_Renamed
			Return yearLength(year_Renamed)
		End Function

		''' <summary>
		''' After adjustments such as add(MONTH), add(YEAR), we don't want the
		''' month to jump around.  E.g., we don't want Jan 31 + 1 month to go to Mar
		''' 3, we want it to go to Feb 28.  Adjustments which might run into this
		''' problem call this method to retain the proper month.
		''' </summary>
		Private Sub pinDayOfMonth()
			Dim year_Renamed As Integer = internalGet(YEAR)
			Dim monthLen As Integer
			If year_Renamed > gregorianCutoverYear OrElse year_Renamed < gregorianCutoverYearJulian Then
				monthLen = monthLength(internalGet(MONTH))
			Else
				Dim gc As GregorianCalendar = normalizedCalendar
				monthLen = gc.getActualMaximum(DAY_OF_MONTH)
			End If
			Dim dom As Integer = internalGet(DAY_OF_MONTH)
			If dom > monthLen Then [set](DAY_OF_MONTH, monthLen)
		End Sub

		''' <summary>
		''' Returns the fixed date value of this object. The time value and
		''' calendar fields must be in synch.
		''' </summary>
		Private Property currentFixedDate As Long
			Get
				Return If(calsys Is gcal, cachedFixedDate, calsys.getFixedDate([cdate]))
			End Get
		End Property

		''' <summary>
		''' Returns the new value after 'roll'ing the specified value and amount.
		''' </summary>
		Private Shared Function getRolledValue(  value As Integer,   amount As Integer,   min As Integer,   max As Integer) As Integer
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
		''' default ERA is CE, but a zero (unset) ERA is BCE.
		''' </summary>
		Private Function internalGetEra() As Integer
			Return If(isSet(ERA), internalGet(ERA), CE)
		End Function

		''' <summary>
		''' Updates internal state.
		''' </summary>
		Private Sub readObject(  stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			If gdate Is Nothing Then
				gdate = CType(gcal.newCalendarDate(zone), sun.util.calendar.BaseCalendar.Date)
				cachedFixedDate = java.lang.[Long].MIN_VALUE
			End If
			gregorianChange = gregorianCutover
		End Sub

		''' <summary>
		''' Converts this object to a {@code ZonedDateTime} that represents
		''' the same point on the time-line as this {@code GregorianCalendar}.
		''' <p>
		''' Since this object supports a Julian-Gregorian cutover date and
		''' {@code ZonedDateTime} does not, it is possible that the resulting year,
		''' month and day will have different values.  The result will represent the
		''' correct date in the ISO calendar system, which will also be the same value
		''' for Modified Julian Days.
		''' </summary>
		''' <returns> a zoned date-time representing the same point on the time-line
		'''  as this gregorian calendar
		''' @since 1.8 </returns>
		Public Overridable Function toZonedDateTime() As java.time.ZonedDateTime
			Return java.time.ZonedDateTime.ofInstant(java.time.Instant.ofEpochMilli(timeInMillis), timeZone.toZoneId())
		End Function

		''' <summary>
		''' Obtains an instance of {@code GregorianCalendar} with the default locale
		''' from a {@code ZonedDateTime} object.
		''' <p>
		''' Since {@code ZonedDateTime} does not support a Julian-Gregorian cutover
		''' date and uses ISO calendar system, the return GregorianCalendar is a pure
		''' Gregorian calendar and uses ISO 8601 standard for week definitions,
		''' which has {@code MONDAY} as the {@link Calendar#getFirstDayOfWeek()
		''' FirstDayOfWeek} and {@code 4} as the value of the
		''' <seealso cref="Calendar#getMinimalDaysInFirstWeek() MinimalDaysInFirstWeek"/>.
		''' <p>
		''' {@code ZoneDateTime} can store points on the time-line further in the
		''' future and further in the past than {@code GregorianCalendar}. In this
		''' scenario, this method will throw an {@code IllegalArgumentException}
		''' exception.
		''' </summary>
		''' <param name="zdt">  the zoned date-time object to convert </param>
		''' <returns>  the gregorian calendar representing the same point on the
		'''  time-line as the zoned date-time provided </returns>
		''' <exception cref="NullPointerException"> if {@code zdt} is null </exception>
		''' <exception cref="IllegalArgumentException"> if the zoned date-time is too
		''' large to represent as a {@code GregorianCalendar}
		''' @since 1.8 </exception>
		Public Shared Function [from](  zdt As java.time.ZonedDateTime) As GregorianCalendar
			Dim cal As New GregorianCalendar(TimeZone.getTimeZone(zdt.zone))
			cal.gregorianChange = New Date(Long.MIN_VALUE)
			cal.firstDayOfWeek = MONDAY
			cal.minimalDaysInFirstWeek = 4
			Try
				cal.timeInMillis = System.Math.addExact (System.Math.multiplyExact(zdt.toEpochSecond(), 1000), zdt.get(java.time.temporal.ChronoField.MILLI_OF_SECOND))
			Catch ex As ArithmeticException
				Throw New IllegalArgumentException(ex)
			End Try
			Return cal
		End Function
	End Class

End Namespace