Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
	''' The <code>Calendar</code> class is an abstract class that provides methods
	''' for converting between a specific instant in time and a set of {@link
	''' #fields calendar fields} such as <code>YEAR</code>, <code>MONTH</code>,
	''' <code>DAY_OF_MONTH</code>, <code>HOUR</code>, and so on, and for
	''' manipulating the calendar fields, such as getting the date of the next
	''' week. An instant in time can be represented by a millisecond value that is
	''' an offset from the <a name="Epoch"><em>Epoch</em></a>, January 1, 1970
	''' 00:00:00.000 GMT (Gregorian).
	''' 
	''' <p>The class also provides additional fields and methods for
	''' implementing a concrete calendar system outside the package. Those
	''' fields and methods are defined as <code>protected</code>.
	''' 
	''' <p>
	''' Like other locale-sensitive classes, <code>Calendar</code> provides a
	''' class method, <code>getInstance</code>, for getting a generally useful
	''' object of this type. <code>Calendar</code>'s <code>getInstance</code> method
	''' returns a <code>Calendar</code> object whose
	''' calendar fields have been initialized with the current date and time:
	''' <blockquote>
	''' <pre>
	'''     Calendar rightNow = Calendar.getInstance();
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>A <code>Calendar</code> object can produce all the calendar field values
	''' needed to implement the date-time formatting for a particular language and
	''' calendar style (for example, Japanese-Gregorian, Japanese-Traditional).
	''' <code>Calendar</code> defines the range of values returned by
	''' certain calendar fields, as well as their meaning.  For example,
	''' the first month of the calendar system has value <code>MONTH ==
	''' JANUARY</code> for all calendars.  Other values are defined by the
	''' concrete subclass, such as <code>ERA</code>.  See individual field
	''' documentation and subclass documentation for details.
	''' 
	''' <h3>Getting and Setting Calendar Field Values</h3>
	''' 
	''' <p>The calendar field values can be set by calling the <code>set</code>
	''' methods. Any field values set in a <code>Calendar</code> will not be
	''' interpreted until it needs to calculate its time value (milliseconds from
	''' the Epoch) or values of the calendar fields. Calling the
	''' <code>get</code>, <code>getTimeInMillis</code>, <code>getTime</code>,
	''' <code>add</code> and <code>roll</code> involves such calculation.
	''' 
	''' <h4>Leniency</h4>
	''' 
	''' <p><code>Calendar</code> has two modes for interpreting the calendar
	''' fields, <em>lenient</em> and <em>non-lenient</em>.  When a
	''' <code>Calendar</code> is in lenient mode, it accepts a wider range of
	''' calendar field values than it produces.  When a <code>Calendar</code>
	''' recomputes calendar field values for return by <code>get()</code>, all of
	''' the calendar fields are normalized. For example, a lenient
	''' <code>GregorianCalendar</code> interprets <code>MONTH == JANUARY</code>,
	''' <code>DAY_OF_MONTH == 32</code> as February 1.
	''' 
	''' <p>When a <code>Calendar</code> is in non-lenient mode, it throws an
	''' exception if there is any inconsistency in its calendar fields. For
	''' example, a <code>GregorianCalendar</code> always produces
	''' <code>DAY_OF_MONTH</code> values between 1 and the length of the month. A
	''' non-lenient <code>GregorianCalendar</code> throws an exception upon
	''' calculating its time or calendar field values if any out-of-range field
	''' value has been set.
	''' 
	''' <h4><a name="first_week">First Week</a></h4>
	''' 
	''' <code>Calendar</code> defines a locale-specific seven day week using two
	''' parameters: the first day of the week and the minimal days in first week
	''' (from 1 to 7).  These numbers are taken from the locale resource data when a
	''' <code>Calendar</code> is constructed.  They may also be specified explicitly
	''' through the methods for setting their values.
	''' 
	''' <p>When setting or getting the <code>WEEK_OF_MONTH</code> or
	''' <code>WEEK_OF_YEAR</code> fields, <code>Calendar</code> must determine the
	''' first week of the month or year as a reference point.  The first week of a
	''' month or year is defined as the earliest seven day period beginning on
	''' <code>getFirstDayOfWeek()</code> and containing at least
	''' <code>getMinimalDaysInFirstWeek()</code> days of that month or year.  Weeks
	''' numbered ..., -1, 0 precede the first week; weeks numbered 2, 3,... follow
	''' it.  Note that the normalized numbering returned by <code>get()</code> may be
	''' different.  For example, a specific <code>Calendar</code> subclass may
	''' designate the week before week 1 of a year as week <code><i>n</i></code> of
	''' the previous year.
	''' 
	''' <h4>Calendar Fields Resolution</h4>
	''' 
	''' When computing a date and time from the calendar fields, there
	''' may be insufficient information for the computation (such as only
	''' year and month with no day of month), or there may be inconsistent
	''' information (such as Tuesday, July 15, 1996 (Gregorian) -- July 15,
	''' 1996 is actually a Monday). <code>Calendar</code> will resolve
	''' calendar field values to determine the date and time in the
	''' following way.
	''' 
	''' <p><a name="resolution">If there is any conflict in calendar field values,
	''' <code>Calendar</code> gives priorities to calendar fields that have been set
	''' more recently.</a> The following are the default combinations of the
	''' calendar fields. The most recent combination, as determined by the
	''' most recently set single field, will be used.
	''' 
	''' <p><a name="date_resolution">For the date fields</a>:
	''' <blockquote>
	''' <pre>
	''' YEAR + MONTH + DAY_OF_MONTH
	''' YEAR + MONTH + WEEK_OF_MONTH + DAY_OF_WEEK
	''' YEAR + MONTH + DAY_OF_WEEK_IN_MONTH + DAY_OF_WEEK
	''' YEAR + DAY_OF_YEAR
	''' YEAR + DAY_OF_WEEK + WEEK_OF_YEAR
	''' </pre></blockquote>
	''' 
	''' <a name="time_resolution">For the time of day fields</a>:
	''' <blockquote>
	''' <pre>
	''' HOUR_OF_DAY
	''' AM_PM + HOUR
	''' </pre></blockquote>
	''' 
	''' <p>If there are any calendar fields whose values haven't been set in the selected
	''' field combination, <code>Calendar</code> uses their default values. The default
	''' value of each field may vary by concrete calendar systems. For example, in
	''' <code>GregorianCalendar</code>, the default of a field is the same as that
	''' of the start of the Epoch: i.e., <code>YEAR = 1970</code>, <code>MONTH =
	''' JANUARY</code>, <code>DAY_OF_MONTH = 1</code>, etc.
	''' 
	''' <p>
	''' <strong>Note:</strong> There are certain possible ambiguities in
	''' interpretation of certain singular times, which are resolved in the
	''' following ways:
	''' <ol>
	'''     <li> 23:59 is the last minute of the day and 00:00 is the first
	'''          minute of the next day. Thus, 23:59 on Dec 31, 1999 &lt; 00:00 on
	'''          Jan 1, 2000 &lt; 00:01 on Jan 1, 2000.
	''' 
	'''     <li> Although historically not precise, midnight also belongs to "am",
	'''          and noon belongs to "pm", so on the same day,
	'''          12:00 am (midnight) &lt; 12:01 am, and 12:00 pm (noon) &lt; 12:01 pm
	''' </ol>
	''' 
	''' <p>
	''' The date or time format strings are not part of the definition of a
	''' calendar, as those must be modifiable or overridable by the user at
	''' runtime. Use <seealso cref="DateFormat"/>
	''' to format dates.
	''' 
	''' <h4>Field Manipulation</h4>
	''' 
	''' The calendar fields can be changed using three methods:
	''' <code>set()</code>, <code>add()</code>, and <code>roll()</code>.
	''' 
	''' <p><strong><code>set(f, value)</code></strong> changes calendar field
	''' <code>f</code> to <code>value</code>.  In addition, it sets an
	''' internal member variable to indicate that calendar field <code>f</code> has
	''' been changed. Although calendar field <code>f</code> is changed immediately,
	''' the calendar's time value in milliseconds is not recomputed until the next call to
	''' <code>get()</code>, <code>getTime()</code>, <code>getTimeInMillis()</code>,
	''' <code>add()</code>, or <code>roll()</code> is made. Thus, multiple calls to
	''' <code>set()</code> do not trigger multiple, unnecessary
	''' computations. As a result of changing a calendar field using
	''' <code>set()</code>, other calendar fields may also change, depending on the
	''' calendar field, the calendar field value, and the calendar system. In addition,
	''' <code>get(f)</code> will not necessarily return <code>value</code> set by
	''' the call to the <code>set</code> method
	''' after the calendar fields have been recomputed. The specifics are determined by
	''' the concrete calendar class.</p>
	''' 
	''' <p><em>Example</em>: Consider a <code>GregorianCalendar</code>
	''' originally set to August 31, 1999. Calling <code>set(Calendar.MONTH,
	''' Calendar.SEPTEMBER)</code> sets the date to September 31,
	''' 1999. This is a temporary internal representation that resolves to
	''' October 1, 1999 if <code>getTime()</code>is then called. However, a
	''' call to <code>set(Calendar.DAY_OF_MONTH, 30)</code> before the call to
	''' <code>getTime()</code> sets the date to September 30, 1999, since
	''' no recomputation occurs after <code>set()</code> itself.</p>
	''' 
	''' <p><strong><code>add(f, delta)</code></strong> adds <code>delta</code>
	''' to field <code>f</code>.  This is equivalent to calling <code>set(f,
	''' get(f) + delta)</code> with two adjustments:</p>
	''' 
	''' <blockquote>
	'''   <p><strong>Add rule 1</strong>. The value of field <code>f</code>
	'''   after the call minus the value of field <code>f</code> before the
	'''   call is <code>delta</code>, modulo any overflow that has occurred in
	'''   field <code>f</code>. Overflow occurs when a field value exceeds its
	'''   range and, as a result, the next larger field is incremented or
	'''   decremented and the field value is adjusted back into its range.</p>
	''' 
	'''   <p><strong>Add rule 2</strong>. If a smaller field is expected to be
	'''   invariant, but it is impossible for it to be equal to its
	'''   prior value because of changes in its minimum or maximum after field
	'''   <code>f</code> is changed or other constraints, such as time zone
	'''   offset changes, then its value is adjusted to be as close
	'''   as possible to its expected value. A smaller field represents a
	'''   smaller unit of time. <code>HOUR</code> is a smaller field than
	'''   <code>DAY_OF_MONTH</code>. No adjustment is made to smaller fields
	'''   that are not expected to be invariant. The calendar system
	'''   determines what fields are expected to be invariant.</p>
	''' </blockquote>
	''' 
	''' <p>In addition, unlike <code>set()</code>, <code>add()</code> forces
	''' an immediate recomputation of the calendar's milliseconds and all
	''' fields.</p>
	''' 
	''' <p><em>Example</em>: Consider a <code>GregorianCalendar</code>
	''' originally set to August 31, 1999. Calling <code>add(Calendar.MONTH,
	''' 13)</code> sets the calendar to September 30, 2000. <strong>Add rule
	''' 1</strong> sets the <code>MONTH</code> field to September, since
	''' adding 13 months to August gives September of the next year. Since
	''' <code>DAY_OF_MONTH</code> cannot be 31 in September in a
	''' <code>GregorianCalendar</code>, <strong>add rule 2</strong> sets the
	''' <code>DAY_OF_MONTH</code> to 30, the closest possible value. Although
	''' it is a smaller field, <code>DAY_OF_WEEK</code> is not adjusted by
	''' rule 2, since it is expected to change when the month changes in a
	''' <code>GregorianCalendar</code>.</p>
	''' 
	''' <p><strong><code>roll(f, delta)</code></strong> adds
	''' <code>delta</code> to field <code>f</code> without changing larger
	''' fields. This is equivalent to calling <code>add(f, delta)</code> with
	''' the following adjustment:</p>
	''' 
	''' <blockquote>
	'''   <p><strong>Roll rule</strong>. Larger fields are unchanged after the
	'''   call. A larger field represents a larger unit of
	'''   time. <code>DAY_OF_MONTH</code> is a larger field than
	'''   <code>HOUR</code>.</p>
	''' </blockquote>
	''' 
	''' <p><em>Example</em>: See <seealso cref="java.util.GregorianCalendar#roll(int, int)"/>.
	''' 
	''' <p><strong>Usage model</strong>. To motivate the behavior of
	''' <code>add()</code> and <code>roll()</code>, consider a user interface
	''' component with increment and decrement buttons for the month, day, and
	''' year, and an underlying <code>GregorianCalendar</code>. If the
	''' interface reads January 31, 1999 and the user presses the month
	''' increment button, what should it read? If the underlying
	''' implementation uses <code>set()</code>, it might read March 3, 1999. A
	''' better result would be February 28, 1999. Furthermore, if the user
	''' presses the month increment button again, it should read March 31,
	''' 1999, not March 28, 1999. By saving the original date and using either
	''' <code>add()</code> or <code>roll()</code>, depending on whether larger
	''' fields should be affected, the user interface can behave as most users
	''' will intuitively expect.</p>
	''' </summary>
	''' <seealso cref=          java.lang.System#currentTimeMillis() </seealso>
	''' <seealso cref=          Date </seealso>
	''' <seealso cref=          GregorianCalendar </seealso>
	''' <seealso cref=          TimeZone </seealso>
	''' <seealso cref=          java.text.DateFormat
	''' @author Mark Davis, David Goldsmith, Chen-Lieh Huang, Alan Liu
	''' @since JDK1.1 </seealso>
	<Serializable> _
	Public MustInherit Class Calendar
		Implements Cloneable, Comparable(Of Calendar)

		' Data flow in Calendar
		' ---------------------

		' The current time is represented in two ways by Calendar: as UTC
		' milliseconds from the epoch (1 January 1970 0:00 UTC), and as local
		' fields such as MONTH, HOUR, AM_PM, etc.  It is possible to compute the
		' millis from the fields, and vice versa.  The data needed to do this
		' conversion is encapsulated by a TimeZone object owned by the Calendar.
		' The data provided by the TimeZone object may also be overridden if the
		' user sets the ZONE_OFFSET and/or DST_OFFSET fields directly. The class
		' keeps track of what information was most recently set by the caller, and
		' uses that to compute any other information as needed.

		' If the user sets the fields using set(), the data flow is as follows.
		' This is implemented by the Calendar subclass's computeTime() method.
		' During this process, certain fields may be ignored.  The disambiguation
		' algorithm for resolving which fields to pay attention to is described
		' in the class documentation.

		'   local fields (YEAR, MONTH, DATE, HOUR, MINUTE, etc.)
		'           |
		'           | Using Calendar-specific algorithm
		'           V
		'   local standard millis
		'           |
		'           | Using TimeZone or user-set ZONE_OFFSET / DST_OFFSET
		'           V
		'   UTC millis (in time data member)

		' If the user sets the UTC millis using setTime() or setTimeInMillis(),
		' the data flow is as follows.  This is implemented by the Calendar
		' subclass's computeFields() method.

		'   UTC millis (in time data member)
		'           |
		'           | Using TimeZone getOffset()
		'           V
		'   local standard millis
		'           |
		'           | Using Calendar-specific algorithm
		'           V
		'   local fields (YEAR, MONTH, DATE, HOUR, MINUTE, etc.)

		' In general, a round trip from fields, through local and UTC millis, and
		' back out to fields is made when necessary.  This is implemented by the
		' complete() method.  Resolving a partial set of fields into a UTC millis
		' value allows all remaining fields to be generated from that value.  If
		' the Calendar is lenient, the fields are also renormalized to standard
		' ranges when they are regenerated.

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' era, e.g., AD or BC in the Julian calendar. This is a calendar-specific
		''' value; see subclass documentation.
		''' </summary>
		''' <seealso cref= GregorianCalendar#AD </seealso>
		''' <seealso cref= GregorianCalendar#BC </seealso>
		Public Const ERA As Integer = 0

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' year. This is a calendar-specific value; see subclass documentation.
		''' </summary>
		Public Const YEAR As Integer = 1

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' month. This is a calendar-specific value. The first month of
		''' the year in the Gregorian and Julian calendars is
		''' <code>JANUARY</code> which is 0; the last depends on the number
		''' of months in a year.
		''' </summary>
		''' <seealso cref= #JANUARY </seealso>
		''' <seealso cref= #FEBRUARY </seealso>
		''' <seealso cref= #MARCH </seealso>
		''' <seealso cref= #APRIL </seealso>
		''' <seealso cref= #MAY </seealso>
		''' <seealso cref= #JUNE </seealso>
		''' <seealso cref= #JULY </seealso>
		''' <seealso cref= #AUGUST </seealso>
		''' <seealso cref= #SEPTEMBER </seealso>
		''' <seealso cref= #OCTOBER </seealso>
		''' <seealso cref= #NOVEMBER </seealso>
		''' <seealso cref= #DECEMBER </seealso>
		''' <seealso cref= #UNDECIMBER </seealso>
		Public Const MONTH As Integer = 2

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' week number within the current year.  The first week of the year, as
		''' defined by <code>getFirstDayOfWeek()</code> and
		''' <code>getMinimalDaysInFirstWeek()</code>, has value 1.  Subclasses define
		''' the value of <code>WEEK_OF_YEAR</code> for days before the first week of
		''' the year.
		''' </summary>
		''' <seealso cref= #getFirstDayOfWeek </seealso>
		''' <seealso cref= #getMinimalDaysInFirstWeek </seealso>
		Public Const WEEK_OF_YEAR As Integer = 3

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' week number within the current month.  The first week of the month, as
		''' defined by <code>getFirstDayOfWeek()</code> and
		''' <code>getMinimalDaysInFirstWeek()</code>, has value 1.  Subclasses define
		''' the value of <code>WEEK_OF_MONTH</code> for days before the first week of
		''' the month.
		''' </summary>
		''' <seealso cref= #getFirstDayOfWeek </seealso>
		''' <seealso cref= #getMinimalDaysInFirstWeek </seealso>
		Public Const WEEK_OF_MONTH As Integer = 4

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' day of the month. This is a synonym for <code>DAY_OF_MONTH</code>.
		''' The first day of the month has value 1.
		''' </summary>
		''' <seealso cref= #DAY_OF_MONTH </seealso>
		Public Const [DATE] As Integer = 5

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' day of the month. This is a synonym for <code>DATE</code>.
		''' The first day of the month has value 1.
		''' </summary>
		''' <seealso cref= #DATE </seealso>
		Public Const DAY_OF_MONTH As Integer = 5

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the day
		''' number within the current year.  The first day of the year has value 1.
		''' </summary>
		Public Const DAY_OF_YEAR As Integer = 6

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the day
		''' of the week.  This field takes values <code>SUNDAY</code>,
		''' <code>MONDAY</code>, <code>TUESDAY</code>, <code>WEDNESDAY</code>,
		''' <code>THURSDAY</code>, <code>FRIDAY</code>, and <code>SATURDAY</code>.
		''' </summary>
		''' <seealso cref= #SUNDAY </seealso>
		''' <seealso cref= #MONDAY </seealso>
		''' <seealso cref= #TUESDAY </seealso>
		''' <seealso cref= #WEDNESDAY </seealso>
		''' <seealso cref= #THURSDAY </seealso>
		''' <seealso cref= #FRIDAY </seealso>
		''' <seealso cref= #SATURDAY </seealso>
		Public Const DAY_OF_WEEK As Integer = 7

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' ordinal number of the day of the week within the current month. Together
		''' with the <code>DAY_OF_WEEK</code> field, this uniquely specifies a day
		''' within a month.  Unlike <code>WEEK_OF_MONTH</code> and
		''' <code>WEEK_OF_YEAR</code>, this field's value does <em>not</em> depend on
		''' <code>getFirstDayOfWeek()</code> or
		''' <code>getMinimalDaysInFirstWeek()</code>.  <code>DAY_OF_MONTH 1</code>
		''' through <code>7</code> always correspond to <code>DAY_OF_WEEK_IN_MONTH
		''' 1</code>; <code>8</code> through <code>14</code> correspond to
		''' <code>DAY_OF_WEEK_IN_MONTH 2</code>, and so on.
		''' <code>DAY_OF_WEEK_IN_MONTH 0</code> indicates the week before
		''' <code>DAY_OF_WEEK_IN_MONTH 1</code>.  Negative values count back from the
		''' end of the month, so the last Sunday of a month is specified as
		''' <code>DAY_OF_WEEK = SUNDAY, DAY_OF_WEEK_IN_MONTH = -1</code>.  Because
		''' negative values count backward they will usually be aligned differently
		''' within the month than positive values.  For example, if a month has 31
		''' days, <code>DAY_OF_WEEK_IN_MONTH -1</code> will overlap
		''' <code>DAY_OF_WEEK_IN_MONTH 5</code> and the end of <code>4</code>.
		''' </summary>
		''' <seealso cref= #DAY_OF_WEEK </seealso>
		''' <seealso cref= #WEEK_OF_MONTH </seealso>
		Public Const DAY_OF_WEEK_IN_MONTH As Integer = 8

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating
		''' whether the <code>HOUR</code> is before or after noon.
		''' E.g., at 10:04:15.250 PM the <code>AM_PM</code> is <code>PM</code>.
		''' </summary>
		''' <seealso cref= #AM </seealso>
		''' <seealso cref= #PM </seealso>
		''' <seealso cref= #HOUR </seealso>
		Public Const AM_PM As Integer = 9

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' hour of the morning or afternoon. <code>HOUR</code> is used for the
		''' 12-hour clock (0 - 11). Noon and midnight are represented by 0, not by 12.
		''' E.g., at 10:04:15.250 PM the <code>HOUR</code> is 10.
		''' </summary>
		''' <seealso cref= #AM_PM </seealso>
		''' <seealso cref= #HOUR_OF_DAY </seealso>
		Public Const HOUR As Integer = 10

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' hour of the day. <code>HOUR_OF_DAY</code> is used for the 24-hour clock.
		''' E.g., at 10:04:15.250 PM the <code>HOUR_OF_DAY</code> is 22.
		''' </summary>
		''' <seealso cref= #HOUR </seealso>
		Public Const HOUR_OF_DAY As Integer = 11

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' minute within the hour.
		''' E.g., at 10:04:15.250 PM the <code>MINUTE</code> is 4.
		''' </summary>
		Public Const MINUTE As Integer = 12

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' second within the minute.
		''' E.g., at 10:04:15.250 PM the <code>SECOND</code> is 15.
		''' </summary>
		Public Const SECOND As Integer = 13

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' millisecond within the second.
		''' E.g., at 10:04:15.250 PM the <code>MILLISECOND</code> is 250.
		''' </summary>
		Public Const MILLISECOND As Integer = 14

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code>
		''' indicating the raw offset from GMT in milliseconds.
		''' <p>
		''' This field reflects the correct GMT offset value of the time
		''' zone of this <code>Calendar</code> if the
		''' <code>TimeZone</code> implementation subclass supports
		''' historical GMT offset changes.
		''' </summary>
		Public Const ZONE_OFFSET As Integer = 15

		''' <summary>
		''' Field number for <code>get</code> and <code>set</code> indicating the
		''' daylight saving offset in milliseconds.
		''' <p>
		''' This field reflects the correct daylight saving offset value of
		''' the time zone of this <code>Calendar</code> if the
		''' <code>TimeZone</code> implementation subclass supports
		''' historical Daylight Saving Time schedule changes.
		''' </summary>
		Public Const DST_OFFSET As Integer = 16

		''' <summary>
		''' The number of distinct fields recognized by <code>get</code> and <code>set</code>.
		''' Field numbers range from <code>0..FIELD_COUNT-1</code>.
		''' </summary>
		Public Const FIELD_COUNT As Integer = 17

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Sunday.
		''' </summary>
		Public Const SUNDAY As Integer = 1

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Monday.
		''' </summary>
		Public Const MONDAY As Integer = 2

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Tuesday.
		''' </summary>
		Public Const TUESDAY As Integer = 3

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Wednesday.
		''' </summary>
		Public Const WEDNESDAY As Integer = 4

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Thursday.
		''' </summary>
		Public Const THURSDAY As Integer = 5

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Friday.
		''' </summary>
		Public Const FRIDAY As Integer = 6

		''' <summary>
		''' Value of the <seealso cref="#DAY_OF_WEEK"/> field indicating
		''' Saturday.
		''' </summary>
		Public Const SATURDAY As Integer = 7

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' first month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const JANUARY As Integer = 0

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' second month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const FEBRUARY As Integer = 1

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' third month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const MARCH As Integer = 2

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' fourth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const APRIL As Integer = 3

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' fifth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const MAY As Integer = 4

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' sixth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const JUNE As Integer = 5

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' seventh month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const JULY As Integer = 6

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' eighth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const AUGUST As Integer = 7

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' ninth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const SEPTEMBER As Integer = 8

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' tenth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const OCTOBER As Integer = 9

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' eleventh month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const NOVEMBER As Integer = 10

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' twelfth month of the year in the Gregorian and Julian calendars.
		''' </summary>
		Public Const DECEMBER As Integer = 11

		''' <summary>
		''' Value of the <seealso cref="#MONTH"/> field indicating the
		''' thirteenth month of the year. Although <code>GregorianCalendar</code>
		''' does not use this value, lunar calendars do.
		''' </summary>
		Public Const UNDECIMBER As Integer = 12

		''' <summary>
		''' Value of the <seealso cref="#AM_PM"/> field indicating the
		''' period of the day from midnight to just before noon.
		''' </summary>
		Public Const AM As Integer = 0

		''' <summary>
		''' Value of the <seealso cref="#AM_PM"/> field indicating the
		''' period of the day from noon to just before midnight.
		''' </summary>
		Public Const PM As Integer = 1

		''' <summary>
		''' A style specifier for {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating names in all styles, such as
		''' "January" and "Jan".
		''' </summary>
		''' <seealso cref= #SHORT_FORMAT </seealso>
		''' <seealso cref= #LONG_FORMAT </seealso>
		''' <seealso cref= #SHORT_STANDALONE </seealso>
		''' <seealso cref= #LONG_STANDALONE </seealso>
		''' <seealso cref= #SHORT </seealso>
		''' <seealso cref= #LONG
		''' @since 1.6 </seealso>
		Public Const ALL_STYLES As Integer = 0

		Friend Const STANDALONE_MASK As Integer = &H8000

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} equivalent to <seealso cref="#SHORT_FORMAT"/>.
		''' </summary>
		''' <seealso cref= #SHORT_STANDALONE </seealso>
		''' <seealso cref= #LONG
		''' @since 1.6 </seealso>
		Public Const [SHORT] As Integer = 1

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} equivalent to <seealso cref="#LONG_FORMAT"/>.
		''' </summary>
		''' <seealso cref= #LONG_STANDALONE </seealso>
		''' <seealso cref= #SHORT
		''' @since 1.6 </seealso>
		Public Const [LONG] As Integer = 2

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a narrow name used for format. Narrow names
		''' are typically single character strings, such as "M" for Monday.
		''' </summary>
		''' <seealso cref= #NARROW_STANDALONE </seealso>
		''' <seealso cref= #SHORT_FORMAT </seealso>
		''' <seealso cref= #LONG_FORMAT
		''' @since 1.8 </seealso>
		Public Const NARROW_FORMAT As Integer = 4

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a narrow name independently. Narrow names
		''' are typically single character strings, such as "M" for Monday.
		''' </summary>
		''' <seealso cref= #NARROW_FORMAT </seealso>
		''' <seealso cref= #SHORT_STANDALONE </seealso>
		''' <seealso cref= #LONG_STANDALONE
		''' @since 1.8 </seealso>
		Public Shared ReadOnly NARROW_STANDALONE As Integer = NARROW_FORMAT Or STANDALONE_MASK

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a short name used for format.
		''' </summary>
		''' <seealso cref= #SHORT_STANDALONE </seealso>
		''' <seealso cref= #LONG_FORMAT </seealso>
		''' <seealso cref= #LONG_STANDALONE
		''' @since 1.8 </seealso>
		Public Const SHORT_FORMAT As Integer = 1

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a long name used for format.
		''' </summary>
		''' <seealso cref= #LONG_STANDALONE </seealso>
		''' <seealso cref= #SHORT_FORMAT </seealso>
		''' <seealso cref= #SHORT_STANDALONE
		''' @since 1.8 </seealso>
		Public Const LONG_FORMAT As Integer = 2

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a short name used independently,
		''' such as a month abbreviation as calendar headers.
		''' </summary>
		''' <seealso cref= #SHORT_FORMAT </seealso>
		''' <seealso cref= #LONG_FORMAT </seealso>
		''' <seealso cref= #LONG_STANDALONE
		''' @since 1.8 </seealso>
		Public Shared ReadOnly SHORT_STANDALONE As Integer = [SHORT] Or STANDALONE_MASK

		''' <summary>
		''' A style specifier for {@link #getDisplayName(int, int, Locale)
		''' getDisplayName} and {@link #getDisplayNames(int, int, Locale)
		''' getDisplayNames} indicating a long name used independently,
		''' such as a month name as calendar headers.
		''' </summary>
		''' <seealso cref= #LONG_FORMAT </seealso>
		''' <seealso cref= #SHORT_FORMAT </seealso>
		''' <seealso cref= #SHORT_STANDALONE
		''' @since 1.8 </seealso>
		Public Shared ReadOnly LONG_STANDALONE As Integer = [LONG] Or STANDALONE_MASK

		' Internal notes:
		' Calendar contains two kinds of time representations: current "time" in
		' milliseconds, and a set of calendar "fields" representing the current time.
		' The two representations are usually in sync, but can get out of sync
		' as follows.
		' 1. Initially, no fields are set, and the time is invalid.
		' 2. If the time is set, all fields are computed and in sync.
		' 3. If a single field is set, the time is invalid.
		' Recomputation of the time and fields happens when the object needs
		' to return a result to the user, or use a result for a computation.

		''' <summary>
		''' The calendar field values for the currently set time for this calendar.
		''' This is an array of <code>FIELD_COUNT</code> integers, with index values
		''' <code>ERA</code> through <code>DST_OFFSET</code>.
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend fields As Integer()

		''' <summary>
		''' The flags which tell if a specified calendar field for the calendar is set.
		''' A new object has no fields set.  After the first call to a method
		''' which generates the fields, they all remain set after that.
		''' This is an array of <code>FIELD_COUNT</code> booleans, with index values
		''' <code>ERA</code> through <code>DST_OFFSET</code>.
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend isSet_Renamed As Boolean()

		''' <summary>
		''' Pseudo-time-stamps which specify when each field was set. There
		''' are two special values, UNSET and COMPUTED. Values from
		''' MINIMUM_USER_SET to  java.lang.[Integer].MAX_VALUE are legal user set values.
		''' </summary>
		<NonSerialized> _
		Private stamp As Integer()

		''' <summary>
		''' The currently set time for this calendar, expressed in milliseconds after
		''' January 1, 1970, 0:00:00 GMT. </summary>
		''' <seealso cref= #isTimeSet
		''' @serial </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend time As Long

		''' <summary>
		''' True if then the value of <code>time</code> is valid.
		''' The time is made invalid by a change to an item of <code>field[]</code>. </summary>
		''' <seealso cref= #time
		''' @serial </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend isTimeSet As Boolean

		''' <summary>
		''' True if <code>fields[]</code> are in sync with the currently set time.
		''' If false, then the next attempt to get the value of a field will
		''' force a recomputation of all fields from the current value of
		''' <code>time</code>.
		''' @serial
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend areFieldsSet As Boolean

		''' <summary>
		''' True if all fields have been set.
		''' @serial
		''' </summary>
		<NonSerialized> _
		Friend areAllFieldsSet As Boolean

		''' <summary>
		''' <code>True</code> if this calendar allows out-of-range field values during computation
		''' of <code>time</code> from <code>fields[]</code>. </summary>
		''' <seealso cref= #setLenient </seealso>
		''' <seealso cref= #isLenient
		''' @serial </seealso>
		Private lenient As Boolean = True

		''' <summary>
		''' The <code>TimeZone</code> used by this calendar. <code>Calendar</code>
		''' uses the time zone data to translate between locale and GMT time.
		''' @serial
		''' </summary>
		Private zone As TimeZone

		''' <summary>
		''' <code>True</code> if zone references to a shared TimeZone object.
		''' </summary>
		<NonSerialized> _
		Private sharedZone As Boolean = False

		''' <summary>
		''' The first day of the week, with possible values <code>SUNDAY</code>,
		''' <code>MONDAY</code>, etc.  This is a locale-dependent value.
		''' @serial
		''' </summary>
		Private firstDayOfWeek As Integer

		''' <summary>
		''' The number of days required for the first week in a month or year,
		''' with possible values from 1 to 7.  This is a locale-dependent value.
		''' @serial
		''' </summary>
		Private minimalDaysInFirstWeek As Integer

		''' <summary>
		''' Cache to hold the firstDayOfWeek and minimalDaysInFirstWeek
		''' of a Locale.
		''' </summary>
		Private Shared ReadOnly cachedLocaleData As java.util.concurrent.ConcurrentMap(Of Locale, Integer()) = New ConcurrentDictionary(Of Locale, Integer())(3)

		' Special values of stamp[]
		''' <summary>
		''' The corresponding fields[] has no value.
		''' </summary>
		Private Const UNSET As Integer = 0

		''' <summary>
		''' The value of the corresponding fields[] has been calculated internally.
		''' </summary>
		Private Const COMPUTED As Integer = 1

		''' <summary>
		''' The value of the corresponding fields[] has been set externally. Stamp
		''' values which are greater than 1 represents the (pseudo) time when the
		''' corresponding fields[] value was set.
		''' </summary>
		Private Const MINIMUM_USER_STAMP As Integer = 2

		''' <summary>
		''' The mask value that represents all of the fields.
		''' </summary>
		Friend Shared ReadOnly ALL_FIELDS As Integer = (1 << FIELD_COUNT) - 1

		''' <summary>
		''' The next available value for <code>stamp[]</code>, an internal array.
		''' This actually should not be written out to the stream, and will probably
		''' be removed from the stream in the near future.  In the meantime,
		''' a value of <code>MINIMUM_USER_STAMP</code> should be used.
		''' @serial
		''' </summary>
		Private nextStamp As Integer = MINIMUM_USER_STAMP

		' the internal serial version which says which version was written
		' - 0 (default) for version up to JDK 1.1.5
		' - 1 for version from JDK 1.1.6, which writes a correct 'time' value
		'     as well as compatible values for other fields.  This is a
		'     transitional format.
		' - 2 (not implemented yet) a future version, in which fields[],
		'     areFieldsSet, and isTimeSet become transient, and isSet[] is
		'     removed. In JDK 1.1.6 we write a format compatible with version 2.
		Friend Const currentSerialVersion As Integer = 1

		''' <summary>
		''' The version of the serialized data on the stream.  Possible values:
		''' <dl>
		''' <dt><b>0</b> or not present on stream</dt>
		''' <dd>
		''' JDK 1.1.5 or earlier.
		''' </dd>
		''' <dt><b>1</b></dt>
		''' <dd>
		''' JDK 1.1.6 or later.  Writes a correct 'time' value
		''' as well as compatible values for other fields.  This is a
		''' transitional format.
		''' </dd>
		''' </dl>
		''' When streaming out this [Class], the most recent format
		''' and the highest allowable <code>serialVersionOnStream</code>
		''' is written.
		''' @serial
		''' @since JDK1.1.6
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion

		' Proclaim serialization compatibility with JDK 1.1
		Friend Const serialVersionUID As Long = -1807547505821590642L

		' Mask values for calendar fields
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared ReadOnly ERA_MASK As Integer = (1 << ERA)
		Friend Shared ReadOnly YEAR_MASK As Integer = (1 << YEAR)
		Friend Shared ReadOnly MONTH_MASK As Integer = (1 << MONTH)
		Friend Shared ReadOnly WEEK_OF_YEAR_MASK As Integer = (1 << WEEK_OF_YEAR)
		Friend Shared ReadOnly WEEK_OF_MONTH_MASK As Integer = (1 << WEEK_OF_MONTH)
		Friend Shared ReadOnly DAY_OF_MONTH_MASK As Integer = (1 << DAY_OF_MONTH)
		Friend Shared ReadOnly DATE_MASK As Integer = DAY_OF_MONTH_MASK
		Friend Shared ReadOnly DAY_OF_YEAR_MASK As Integer = (1 << DAY_OF_YEAR)
		Friend Shared ReadOnly DAY_OF_WEEK_MASK As Integer = (1 << DAY_OF_WEEK)
		Friend Shared ReadOnly DAY_OF_WEEK_IN_MONTH_MASK As Integer = (1 << DAY_OF_WEEK_IN_MONTH)
		Friend Shared ReadOnly AM_PM_MASK As Integer = (1 << AM_PM)
		Friend Shared ReadOnly HOUR_MASK As Integer = (1 << HOUR)
		Friend Shared ReadOnly HOUR_OF_DAY_MASK As Integer = (1 << HOUR_OF_DAY)
		Friend Shared ReadOnly MINUTE_MASK As Integer = (1 << MINUTE)
		Friend Shared ReadOnly SECOND_MASK As Integer = (1 << SECOND)
		Friend Shared ReadOnly MILLISECOND_MASK As Integer = (1 << MILLISECOND)
		Friend Shared ReadOnly ZONE_OFFSET_MASK As Integer = (1 << ZONE_OFFSET)
		Friend Shared ReadOnly DST_OFFSET_MASK As Integer = (1 << DST_OFFSET)

		''' <summary>
		''' {@code Calendar.Builder} is used for creating a {@code Calendar} from
		''' various date-time parameters.
		''' 
		''' <p>There are two ways to set a {@code Calendar} to a date-time value. One
		''' is to set the instant parameter to a millisecond offset from the <a
		''' href="Calendar.html#Epoch">Epoch</a>. The other is to set individual
		''' field parameters, such as <seealso cref="Calendar#YEAR YEAR"/>, to their desired
		''' values. These two ways can't be mixed. Trying to set both the instant and
		''' individual fields will cause an <seealso cref="IllegalStateException"/> to be
		''' thrown. However, it is permitted to override previous values of the
		''' instant or field parameters.
		''' 
		''' <p>If no enough field parameters are given for determining date and/or
		''' time, calendar specific default values are used when building a
		''' {@code Calendar}. For example, if the <seealso cref="Calendar#YEAR YEAR"/> value
		''' isn't given for the Gregorian calendar, 1970 will be used. If there are
		''' any conflicts among field parameters, the <a
		''' href="Calendar.html#resolution"> resolution rules</a> are applied.
		''' Therefore, the order of field setting matters.
		''' 
		''' <p>In addition to the date-time parameters,
		''' the <seealso cref="#setLocale(Locale) locale"/>,
		''' <seealso cref="#setTimeZone(TimeZone) time zone"/>,
		''' <seealso cref="#setWeekDefinition(int, int) week definition"/>, and
		''' <seealso cref="#setLenient(boolean) leniency mode"/> parameters can be set.
		''' 
		''' <p><b>Examples</b>
		''' <p>The following are sample usages. Sample code assumes that the
		''' {@code Calendar} constants are statically imported.
		''' 
		''' <p>The following code produces a {@code Calendar} with date 2012-12-31
		''' (Gregorian) because Monday is the first day of a week with the <a
		''' href="GregorianCalendar.html#iso8601_compatible_setting"> ISO 8601
		''' compatible week parameters</a>.
		''' <pre>
		'''   Calendar cal = new Calendar.Builder().setCalendarType("iso8601")
		'''                        .setWeekDate(2013, 1, MONDAY).build();</pre>
		''' <p>The following code produces a Japanese {@code Calendar} with date
		''' 1989-01-08 (Gregorian), assuming that the default <seealso cref="Calendar#ERA ERA"/>
		''' is <em>Heisei</em> that started on that day.
		''' <pre>
		'''   Calendar cal = new Calendar.Builder().setCalendarType("japanese")
		'''                        .setFields(YEAR, 1, DAY_OF_YEAR, 1).build();</pre>
		''' 
		''' @since 1.8 </summary>
		''' <seealso cref= Calendar#getInstance(TimeZone, Locale) </seealso>
		''' <seealso cref= Calendar#fields </seealso>
		Public Class Builder
			Private Shared ReadOnly NFIELDS As Integer = FIELD_COUNT + 1 ' +1 for WEEK_YEAR
			Private Shared ReadOnly WEEK_YEAR As Integer = FIELD_COUNT

			Private instant As Long
			' Calendar.stamp[] (lower half) and Calendar.fields[] (upper half) combined
			Private fields As Integer()
			' Pseudo timestamp starting from MINIMUM_USER_STAMP.
			' (COMPUTED is used to indicate that the instant has been set.)
			Private nextStamp As Integer
			' maxFieldIndex keeps the max index of fields which have been set.
			' (WEEK_YEAR is never included.)
			Private maxFieldIndex As Integer
			Private type As String
			Private zone As TimeZone
			Private lenient As Boolean = True
			Private locale As Locale
			Private firstDayOfWeek, minimalDaysInFirstWeek As Integer

			''' <summary>
			''' Constructs a {@code Calendar.Builder}.
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Sets the instant parameter to the given {@code instant} value that is
			''' a millisecond offset from <a href="Calendar.html#Epoch">the
			''' Epoch</a>.
			''' </summary>
			''' <param name="instant"> a millisecond offset from the Epoch </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="IllegalStateException"> if any of the field parameters have
			'''                               already been set </exception>
			''' <seealso cref= Calendar#setTime(Date) </seealso>
			''' <seealso cref= Calendar#setTimeInMillis(long) </seealso>
			''' <seealso cref= Calendar#time </seealso>
			Public Overridable Function setInstant(ByVal instant As Long) As Builder
				If fields IsNot Nothing Then Throw New IllegalStateException
				Me.instant = instant
				nextStamp = COMPUTED
				Return Me
			End Function

			''' <summary>
			''' Sets the instant parameter to the {@code instant} value given by a
			''' <seealso cref="Date"/>. This method is equivalent to a call to
			''' <seealso cref="#setInstant(long) setInstant(instant.getTime())"/>.
			''' </summary>
			''' <param name="instant"> a {@code Date} representing a millisecond offset from
			'''                the Epoch </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="NullPointerException">  if {@code instant} is {@code null} </exception>
			''' <exception cref="IllegalStateException"> if any of the field parameters have
			'''                               already been set </exception>
			''' <seealso cref= Calendar#setTime(Date) </seealso>
			''' <seealso cref= Calendar#setTimeInMillis(long) </seealso>
			''' <seealso cref= Calendar#time </seealso>
			Public Overridable Function setInstant(ByVal instant As Date) As Builder
				Return instantant(instant.time) ' NPE if instant == null
			End Function

			''' <summary>
			''' Sets the {@code field} parameter to the given {@code value}.
			''' {@code field} is an index to the <seealso cref="Calendar#fields"/>, such as
			''' <seealso cref="Calendar#DAY_OF_MONTH DAY_OF_MONTH"/>. Field value validation is
			''' not performed in this method. Any out of range values are either
			''' normalized in lenient mode or detected as an invalid value in
			''' non-lenient mode when building a {@code Calendar}.
			''' </summary>
			''' <param name="field"> an index to the {@code Calendar} fields </param>
			''' <param name="value"> the field value </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code field} is invalid </exception>
			''' <exception cref="IllegalStateException"> if the instant value has already been set,
			'''                      or if fields have been set too many
			'''                      (approximately <seealso cref="Integer#MAX_VALUE"/>) times. </exception>
			''' <seealso cref= Calendar#set(int, int) </seealso>
			Public Overridable Function [set](ByVal field As Integer, ByVal value As Integer) As Builder
				' Note: WEEK_YEAR can't be set with this method.
				If field < 0 OrElse field >= FIELD_COUNT Then Throw New IllegalArgumentException("field is invalid")
				If instantSet Then Throw New IllegalStateException("instant has been set")
				allocateFields()
				internalSet(field, value)
				Return Me
			End Function

			''' <summary>
			''' Sets field parameters to their values given by
			''' {@code fieldValuePairs} that are pairs of a field and its value.
			''' For example,
			''' <pre>
			'''   setFeilds(Calendar.YEAR, 2013,
			'''             Calendar.MONTH, Calendar.DECEMBER,
			'''             Calendar.DAY_OF_MONTH, 23);</pre>
			''' is equivalent to the sequence of the following
			''' <seealso cref="#set(int, int) set"/> calls:
			''' <pre>
			'''   set(Calendar.YEAR, 2013)
			'''   .set(Calendar.MONTH, Calendar.DECEMBER)
			'''   .set(Calendar.DAY_OF_MONTH, 23);</pre>
			''' </summary>
			''' <param name="fieldValuePairs"> field-value pairs </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="NullPointerException"> if {@code fieldValuePairs} is {@code null} </exception>
			''' <exception cref="IllegalArgumentException"> if any of fields are invalid,
			'''             or if {@code fieldValuePairs.length} is an odd number. </exception>
			''' <exception cref="IllegalStateException">    if the instant value has been set,
			'''             or if fields have been set too many (approximately
			'''             <seealso cref="Integer#MAX_VALUE"/>) times. </exception>
			Public Overridable Function setFields(ParamArray ByVal fieldValuePairs As Integer()) As Builder
				Dim len As Integer = fieldValuePairs.Length
				If (len Mod 2) <> 0 Then Throw New IllegalArgumentException
				If instantSet Then Throw New IllegalStateException("instant has been set")
				If (nextStamp + len \ 2) < 0 Then Throw New IllegalStateException("stamp counter overflow")
				allocateFields()
				Dim i As Integer = 0
				Do While i < len
					Dim field As Integer = fieldValuePairs(i)
					i += 1
					' Note: WEEK_YEAR can't be set with this method.
					If field < 0 OrElse field >= FIELD_COUNT Then Throw New IllegalArgumentException("field is invalid")
					internalSet(field, fieldValuePairs(i))
					i += 1
				Loop
				Return Me
			End Function

			''' <summary>
			''' Sets the date field parameters to the values given by {@code year},
			''' {@code month}, and {@code dayOfMonth}. This method is equivalent to
			''' a call to:
			''' <pre>
			'''   setFields(Calendar.YEAR, year,
			'''             Calendar.MONTH, month,
			'''             Calendar.DAY_OF_MONTH, dayOfMonth);</pre>
			''' </summary>
			''' <param name="year">       the <seealso cref="Calendar#YEAR YEAR"/> value </param>
			''' <param name="month">      the <seealso cref="Calendar#MONTH MONTH"/> value
			'''                   (the month numbering is <em>0-based</em>). </param>
			''' <param name="dayOfMonth"> the <seealso cref="Calendar#DAY_OF_MONTH DAY_OF_MONTH"/> value </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			Public Overridable Function setDate(ByVal year As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As Builder
				Return fieldslds(Builder.YEAR, year, Builder.MONTH, month, DAY_OF_MONTH, dayOfMonth)
			End Function

			''' <summary>
			''' Sets the time of day field parameters to the values given by
			''' {@code hourOfDay}, {@code minute}, and {@code second}. This method is
			''' equivalent to a call to:
			''' <pre>
			'''   setTimeOfDay(hourOfDay, minute, second, 0);</pre>
			''' </summary>
			''' <param name="hourOfDay"> the <seealso cref="Calendar#HOUR_OF_DAY HOUR_OF_DAY"/> value
			'''                  (24-hour clock) </param>
			''' <param name="minute">    the <seealso cref="Calendar#MINUTE MINUTE"/> value </param>
			''' <param name="second">    the <seealso cref="Calendar#SECOND SECOND"/> value </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			Public Overridable Function setTimeOfDay(ByVal hourOfDay As Integer, ByVal minute As Integer, ByVal second As Integer) As Builder
				Return timeOfDayDay(hourOfDay, minute, second, 0)
			End Function

			''' <summary>
			''' Sets the time of day field parameters to the values given by
			''' {@code hourOfDay}, {@code minute}, {@code second}, and
			''' {@code millis}. This method is equivalent to a call to:
			''' <pre>
			'''   setFields(Calendar.HOUR_OF_DAY, hourOfDay,
			'''             Calendar.MINUTE, minute,
			'''             Calendar.SECOND, second,
			'''             Calendar.MILLISECOND, millis);</pre>
			''' </summary>
			''' <param name="hourOfDay"> the <seealso cref="Calendar#HOUR_OF_DAY HOUR_OF_DAY"/> value
			'''                  (24-hour clock) </param>
			''' <param name="minute">    the <seealso cref="Calendar#MINUTE MINUTE"/> value </param>
			''' <param name="second">    the <seealso cref="Calendar#SECOND SECOND"/> value </param>
			''' <param name="millis">    the <seealso cref="Calendar#MILLISECOND MILLISECOND"/> value </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			Public Overridable Function setTimeOfDay(ByVal hourOfDay As Integer, ByVal minute As Integer, ByVal second As Integer, ByVal millis As Integer) As Builder
				Return fieldslds(HOUR_OF_DAY, hourOfDay, Builder.MINUTE, minute, Builder.SECOND, second, MILLISECOND, millis)
			End Function

			''' <summary>
			''' Sets the week-based date parameters to the values with the given
			''' date specifiers - week year, week of year, and day of week.
			''' 
			''' <p>If the specified calendar doesn't support week dates, the
			''' <seealso cref="#build() build"/> method will throw an <seealso cref="IllegalArgumentException"/>.
			''' </summary>
			''' <param name="weekYear">   the week year </param>
			''' <param name="weekOfYear"> the week number based on {@code weekYear} </param>
			''' <param name="dayOfWeek">  the day of week value: one of the constants
			'''     for the <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> field:
			'''     <seealso cref="Calendar#SUNDAY SUNDAY"/>, ..., <seealso cref="Calendar#SATURDAY SATURDAY"/>. </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <seealso cref= Calendar#setWeekDate(int, int, int) </seealso>
			''' <seealso cref= Calendar#isWeekDateSupported() </seealso>
			Public Overridable Function setWeekDate(ByVal weekYear As Integer, ByVal weekOfYear As Integer, ByVal dayOfWeek As Integer) As Builder
				allocateFields()
				internalSet(WEEK_YEAR, weekYear)
				internalSet(WEEK_OF_YEAR, weekOfYear)
				internalSet(DAY_OF_WEEK, dayOfWeek)
				Return Me
			End Function

			''' <summary>
			''' Sets the time zone parameter to the given {@code zone}. If no time
			''' zone parameter is given to this {@code Caledar.Builder}, the
			''' {@link TimeZone#getDefault() default
			''' <code>TimeZone</code>} will be used in the <seealso cref="#build() build"/>
			''' method.
			''' </summary>
			''' <param name="zone"> the <seealso cref="TimeZone"/> </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="NullPointerException"> if {@code zone} is {@code null} </exception>
			''' <seealso cref= Calendar#setTimeZone(TimeZone) </seealso>
			Public Overridable Function setTimeZone(ByVal zone As TimeZone) As Builder
				If zone Is Nothing Then Throw New NullPointerException
				Me.zone = zone
				Return Me
			End Function

			''' <summary>
			''' Sets the lenient mode parameter to the value given by {@code lenient}.
			''' If no lenient parameter is given to this {@code Calendar.Builder},
			''' lenient mode will be used in the <seealso cref="#build() build"/> method.
			''' </summary>
			''' <param name="lenient"> {@code true} for lenient mode;
			'''                {@code false} for non-lenient mode </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <seealso cref= Calendar#setLenient(boolean) </seealso>
			Public Overridable Function setLenient(ByVal lenient As Boolean) As Builder
				Me.lenient = lenient
				Return Me
			End Function

			''' <summary>
			''' Sets the calendar type parameter to the given {@code type}. The
			''' calendar type given by this method has precedence over any explicit
			''' or implicit calendar type given by the
			''' <seealso cref="#setLocale(Locale) locale"/>.
			''' 
			''' <p>In addition to the available calendar types returned by the
			''' <seealso cref="Calendar#getAvailableCalendarTypes() Calendar.getAvailableCalendarTypes"/>
			''' method, {@code "gregorian"} and {@code "iso8601"} as aliases of
			''' {@code "gregory"} can be used with this method.
			''' </summary>
			''' <param name="type"> the calendar type </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="NullPointerException"> if {@code type} is {@code null} </exception>
			''' <exception cref="IllegalArgumentException"> if {@code type} is unknown </exception>
			''' <exception cref="IllegalStateException"> if another calendar type has already been set </exception>
			''' <seealso cref= Calendar#getCalendarType() </seealso>
			''' <seealso cref= Calendar#getAvailableCalendarTypes() </seealso>
			Public Overridable Function setCalendarType(ByVal type As String) As Builder
				If type.Equals("gregorian") Then ' NPE if type == null type = "gregory"
				If (Not Calendar.availableCalendarTypes.contains(type)) AndAlso (Not type.Equals("iso8601")) Then Throw New IllegalArgumentException("unknown calendar type: " & type)
				If Me.type Is Nothing Then
					Me.type = type
				Else
					If Not Me.type.Equals(type) Then Throw New IllegalStateException("calendar type override")
				End If
				Return Me
			End Function

			''' <summary>
			''' Sets the locale parameter to the given {@code locale}. If no locale
			''' is given to this {@code Calendar.Builder}, the {@linkplain
			''' Locale#getDefault(Locale.Category) default <code>Locale</code>}
			''' for <seealso cref="Locale.Category#FORMAT"/> will be used.
			''' 
			''' <p>If no calendar type is explicitly given by a call to the
			''' <seealso cref="#setCalendarType(String) setCalendarType"/> method,
			''' the {@code Locale} value is used to determine what type of
			''' {@code Calendar} to be built.
			''' 
			''' <p>If no week definition parameters are explicitly given by a call to
			''' the <seealso cref="#setWeekDefinition(int,int) setWeekDefinition"/> method, the
			''' {@code Locale}'s default values are used.
			''' </summary>
			''' <param name="locale"> the <seealso cref="Locale"/> </param>
			''' <exception cref="NullPointerException"> if {@code locale} is {@code null} </exception>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <seealso cref= Calendar#getInstance(Locale) </seealso>
			Public Overridable Function setLocale(ByVal locale_Renamed As Locale) As Builder
				If locale_Renamed Is Nothing Then Throw New NullPointerException
				Me.locale = locale_Renamed
				Return Me
			End Function

			''' <summary>
			''' Sets the week definition parameters to the values given by
			''' {@code firstDayOfWeek} and {@code minimalDaysInFirstWeek} that are
			''' used to determine the <a href="Calendar.html#First_Week">first
			''' week</a> of a year. The parameters given by this method have
			''' precedence over the default values given by the
			''' <seealso cref="#setLocale(Locale) locale"/>.
			''' </summary>
			''' <param name="firstDayOfWeek"> the first day of a week; one of
			'''                       <seealso cref="Calendar#SUNDAY"/> to <seealso cref="Calendar#SATURDAY"/> </param>
			''' <param name="minimalDaysInFirstWeek"> the minimal number of days in the first
			'''                               week (1..7) </param>
			''' <returns> this {@code Calendar.Builder} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code firstDayOfWeek} or
			'''                                  {@code minimalDaysInFirstWeek} is invalid </exception>
			''' <seealso cref= Calendar#getFirstDayOfWeek() </seealso>
			''' <seealso cref= Calendar#getMinimalDaysInFirstWeek() </seealso>
			Public Overridable Function setWeekDefinition(ByVal firstDayOfWeek As Integer, ByVal minimalDaysInFirstWeek As Integer) As Builder
				If (Not isValidWeekParameter(firstDayOfWeek)) OrElse (Not isValidWeekParameter(minimalDaysInFirstWeek)) Then Throw New IllegalArgumentException
				Me.firstDayOfWeek = firstDayOfWeek
				Me.minimalDaysInFirstWeek = minimalDaysInFirstWeek
				Return Me
			End Function

			''' <summary>
			''' Returns a {@code Calendar} built from the parameters set by the
			''' setter methods. The calendar type given by the {@link #setCalendarType(String)
			''' setCalendarType} method or the <seealso cref="#setLocale(Locale) locale"/> is
			''' used to determine what {@code Calendar} to be created. If no explicit
			''' calendar type is given, the locale's default calendar is created.
			''' 
			''' <p>If the calendar type is {@code "iso8601"}, the
			''' <seealso cref="GregorianCalendar#setGregorianChange(Date) Gregorian change date"/>
			''' of a <seealso cref="GregorianCalendar"/> is set to {@code Date(Long.MIN_VALUE)}
			''' to be the <em>proleptic</em> Gregorian calendar. Its week definition
			''' parameters are also set to be <a
			''' href="GregorianCalendar.html#iso8601_compatible_setting">compatible
			''' with the ISO 8601 standard</a>. Note that the
			''' <seealso cref="GregorianCalendar#getCalendarType() getCalendarType"/> method of
			''' a {@code GregorianCalendar} created with {@code "iso8601"} returns
			''' {@code "gregory"}.
			''' 
			''' <p>The default values are used for locale and time zone if these
			''' parameters haven't been given explicitly.
			''' 
			''' <p>Any out of range field values are either normalized in lenient
			''' mode or detected as an invalid value in non-lenient mode.
			''' </summary>
			''' <returns> a {@code Calendar} built with parameters of this {@code
			'''         Calendar.Builder} </returns>
			''' <exception cref="IllegalArgumentException"> if the calendar type is unknown, or
			'''             if any invalid field values are given in non-lenient mode, or
			'''             if a week date is given for the calendar type that doesn't
			'''             support week dates. </exception>
			''' <seealso cref= Calendar#getInstance(TimeZone, Locale) </seealso>
			''' <seealso cref= Locale#getDefault(Locale.Category) </seealso>
			''' <seealso cref= TimeZone#getDefault() </seealso>
			Public Overridable Function build() As Calendar
				If locale Is Nothing Then locale = Locale.default
				If zone Is Nothing Then zone = TimeZone.default
				Dim cal As Calendar
				If type Is Nothing Then type = locale.getUnicodeLocaleType("ca")
				If type Is Nothing Then
					If locale.country = "TH" AndAlso locale.language = "th" Then
						type = "buddhist"
					Else
						type = "gregory"
					End If
				End If
				Select Case type
				Case "gregory"
					cal = New GregorianCalendar(zone, locale, True)
				Case "iso8601"
					Dim gcal As New GregorianCalendar(zone, locale, True)
					' make gcal a proleptic Gregorian
					gcal.gregorianChange = New Date(Long.MIN_VALUE)
					' and week definition to be compatible with ISO 8601
					weekDefinitionion(MONDAY, 4)
					cal = gcal
				Case "buddhist"
					cal = New sun.util.BuddhistCalendar(zone, locale)
					cal.clear()
				Case "japanese"
					cal = New JapaneseImperialCalendar(zone, locale, True)
				Case Else
					Throw New IllegalArgumentException("unknown calendar type: " & type)
				End Select
				cal.lenient = lenient
				If firstDayOfWeek <> 0 Then
					cal.firstDayOfWeek = firstDayOfWeek
					cal.minimalDaysInFirstWeek = minimalDaysInFirstWeek
				End If
				If instantSet Then
					cal.timeInMillis = instant
					cal.complete()
					Return cal
				End If

				If fields IsNot Nothing Then
					Dim weekDate_Renamed As Boolean = isSet(WEEK_YEAR) AndAlso fields(WEEK_YEAR) > fields(YEAR)
					If weekDate_Renamed AndAlso (Not cal.weekDateSupported) Then Throw New IllegalArgumentException("week date is unsupported by " & type)

					' Set the fields from the min stamp to the max stamp so that
					' the fields resolution works in the Calendar.
					For stamp As Integer = MINIMUM_USER_STAMP To nextStamp - 1
						For index As Integer = 0 To maxFieldIndex
							If fields(index) = stamp Then
								cal.set(index, fields(NFIELDS + index))
								Exit For
							End If
						Next index
					Next stamp

					If weekDate_Renamed Then
						Dim weekOfYear As Integer = If(isSet(WEEK_OF_YEAR), fields(NFIELDS + WEEK_OF_YEAR), 1)
						Dim dayOfWeek As Integer = If(isSet(DAY_OF_WEEK), fields(NFIELDS + DAY_OF_WEEK), cal.firstDayOfWeek)
						cal.weekDateate(fields(NFIELDS + WEEK_YEAR), weekOfYear, dayOfWeek)
					End If
					cal.complete()
				End If

				Return cal
			End Function

			Private Sub allocateFields()
				If fields Is Nothing Then
					fields = New Integer(NFIELDS * 2 - 1){}
					nextStamp = MINIMUM_USER_STAMP
					maxFieldIndex = -1
				End If
			End Sub

			Private Sub internalSet(ByVal field As Integer, ByVal value As Integer)
				fields(field) = nextStamp
				nextStamp += 1
				If nextStamp < 0 Then Throw New IllegalStateException("stamp counter overflow")
				fields(NFIELDS + field) = value
				If field > maxFieldIndex AndAlso field < WEEK_YEAR Then maxFieldIndex = field
			End Sub

			Private Property instantSet As Boolean
				Get
					Return nextStamp = COMPUTED
				End Get
			End Property

			Private Function isSet(ByVal index As Integer) As Boolean
				Return fields IsNot Nothing AndAlso fields(index) > UNSET
			End Function

			Private Function isValidWeekParameter(ByVal value As Integer) As Boolean
				Return value > 0 AndAlso value <= 7
			End Function
		End Class

		''' <summary>
		''' Constructs a Calendar with the default time zone
		''' and the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/>
		''' locale. </summary>
		''' <seealso cref=     TimeZone#getDefault </seealso>
		Protected Friend Sub New()
			Me.New(TimeZone.defaultRef, Locale.getDefault(Locale.Category.FORMAT))
			sharedZone = True
		End Sub

		''' <summary>
		''' Constructs a calendar with the specified time zone and locale.
		''' </summary>
		''' <param name="zone"> the time zone to use </param>
		''' <param name="aLocale"> the locale for the week data </param>
		Protected Friend Sub New(ByVal zone As TimeZone, ByVal aLocale As Locale)
			fields = New Integer(FIELD_COUNT - 1){}
			isSet_Renamed = New Boolean(FIELD_COUNT - 1){}
			stamp = New Integer(FIELD_COUNT - 1){}

			Me.zone = zone
			weekCountData = aLocale
		End Sub

		''' <summary>
		''' Gets a calendar using the default time zone and locale. The
		''' <code>Calendar</code> returned is based on the current time
		''' in the default time zone with the default
		''' <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		''' </summary>
		''' <returns> a Calendar. </returns>
		PublicShared ReadOnly Propertyinstance As Calendar
			Get
				Return createCalendar(TimeZone.default, Locale.getDefault(Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets a calendar using the specified time zone and default locale.
		''' The <code>Calendar</code> returned is based on the current time
		''' in the given time zone with the default
		''' <seealso cref="Locale.Category#FORMAT FORMAT"/> locale.
		''' </summary>
		''' <param name="zone"> the time zone to use </param>
		''' <returns> a Calendar. </returns>
		Public Shared Function getInstance(ByVal zone As TimeZone) As Calendar
			Return createCalendar(zone, Locale.getDefault(Locale.Category.FORMAT))
		End Function

		''' <summary>
		''' Gets a calendar using the default time zone and specified locale.
		''' The <code>Calendar</code> returned is based on the current time
		''' in the default time zone with the given locale.
		''' </summary>
		''' <param name="aLocale"> the locale for the week data </param>
		''' <returns> a Calendar. </returns>
		Public Shared Function getInstance(ByVal aLocale As Locale) As Calendar
			Return createCalendar(TimeZone.default, aLocale)
		End Function

		''' <summary>
		''' Gets a calendar with the specified time zone and locale.
		''' The <code>Calendar</code> returned is based on the current time
		''' in the given time zone with the given locale.
		''' </summary>
		''' <param name="zone"> the time zone to use </param>
		''' <param name="aLocale"> the locale for the week data </param>
		''' <returns> a Calendar. </returns>
		Public Shared Function getInstance(ByVal zone As TimeZone, ByVal aLocale As Locale) As Calendar
			Return createCalendar(zone, aLocale)
		End Function

		Private Shared Function createCalendar(ByVal zone As TimeZone, ByVal aLocale As Locale) As Calendar
			Dim provider As sun.util.spi.CalendarProvider = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(sun.util.spi.CalendarProvider), aLocale).calendarProvider
			If provider IsNot Nothing Then
				Try
					Return provider.getInstance(zone, aLocale)
				Catch iae As IllegalArgumentException
					' fall back to the default instantiation
				End Try
			End If

			Dim cal As Calendar = Nothing

			If aLocale.hasExtensions() Then
				Dim caltype As String = aLocale.getUnicodeLocaleType("ca")
				If caltype IsNot Nothing Then
					Select Case caltype
					Case "buddhist"
					cal = New sun.util.BuddhistCalendar(zone, aLocale)
					Case "japanese"
						cal = New JapaneseImperialCalendar(zone, aLocale)
					Case "gregory"
						cal = New GregorianCalendar(zone, aLocale)
					End Select
				End If
			End If
			If cal Is Nothing Then
				' If no known calendar type is explicitly specified,
				' perform the traditional way to create a Calendar:
				' create a BuddhistCalendar for th_TH locale,
				' a JapaneseImperialCalendar for ja_JP_JP locale, or
				' a GregorianCalendar for any other locales.
				' NOTE: The language, country and variant strings are interned.
				If aLocale.language = "th" AndAlso aLocale.country = "TH" Then
					cal = New sun.util.BuddhistCalendar(zone, aLocale)
				ElseIf aLocale.variant = "JP" AndAlso aLocale.language = "ja" AndAlso aLocale.country = "JP" Then
					cal = New JapaneseImperialCalendar(zone, aLocale)
				Else
					cal = New GregorianCalendar(zone, aLocale)
				End If
			End If
			Return cal
		End Function

		''' <summary>
		''' Returns an array of all locales for which the <code>getInstance</code>
		''' methods of this class can return localized instances.
		''' The array returned must contain at least a <code>Locale</code>
		''' instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>Calendar</code> instances are available. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		PublicShared ReadOnly PropertyavailableLocales As Locale()
			Get
				Return java.text.DateFormat.availableLocales
			End Get
		End Property

		''' <summary>
		''' Converts the current calendar field values in <seealso cref="#fields fields[]"/>
		''' to the millisecond time value
		''' <seealso cref="#time"/>.
		''' </summary>
		''' <seealso cref= #complete() </seealso>
		''' <seealso cref= #computeFields() </seealso>
		Protected Friend MustOverride Sub computeTime()

		''' <summary>
		''' Converts the current millisecond time value <seealso cref="#time"/>
		''' to calendar field values in <seealso cref="#fields fields[]"/>.
		''' This allows you to sync up the calendar field values with
		''' a new time that is set for the calendar.  The time is <em>not</em>
		''' recomputed first; to recompute the time, then the fields, call the
		''' <seealso cref="#complete()"/> method.
		''' </summary>
		''' <seealso cref= #computeTime() </seealso>
		Protected Friend MustOverride Sub computeFields()

		''' <summary>
		''' Returns a <code>Date</code> object representing this
		''' <code>Calendar</code>'s time value (millisecond offset from the <a
		''' href="#Epoch">Epoch</a>").
		''' </summary>
		''' <returns> a <code>Date</code> representing the time value. </returns>
		''' <seealso cref= #setTime(Date) </seealso>
		''' <seealso cref= #getTimeInMillis() </seealso>
		Public Property time As Date
			Get
				Return New Date(timeInMillis)
			End Get
			Set(ByVal [date] As Date)
				timeInMillis = date_Renamed.time
			End Set
		End Property


		''' <summary>
		''' Returns this Calendar's time value in milliseconds.
		''' </summary>
		''' <returns> the current time as UTC milliseconds from the epoch. </returns>
		''' <seealso cref= #getTime() </seealso>
		''' <seealso cref= #setTimeInMillis(long) </seealso>
		Public Overridable Property timeInMillis As Long
			Get
				If Not isTimeSet Then updateTime()
				Return time
			End Get
			Set(ByVal millis As Long)
				' If we don't need to recalculate the calendar field values,
				' do nothing.
				If time = millis AndAlso isTimeSet AndAlso areFieldsSet AndAlso areAllFieldsSet AndAlso (TypeOf zone Is sun.util.calendar.ZoneInfo) AndAlso (Not CType(zone, sun.util.calendar.ZoneInfo).dirty) Then Return
				time = millis
				isTimeSet = True
				areFieldsSet = False
				computeFields()
					areFieldsSet = True
					areAllFieldsSet = areFieldsSet
			End Set
		End Property


		''' <summary>
		''' Returns the value of the given calendar field. In lenient mode,
		''' all calendar fields are normalized. In non-lenient mode, all
		''' calendar fields are validated and this method throws an
		''' exception if any calendar fields have out-of-range values. The
		''' normalization and validation are handled by the
		''' <seealso cref="#complete()"/> method, which process is calendar
		''' system dependent.
		''' </summary>
		''' <param name="field"> the given calendar field. </param>
		''' <returns> the value for the given calendar field. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the specified field is out of range
		'''             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		''' <seealso cref= #set(int,int) </seealso>
		''' <seealso cref= #complete() </seealso>
		Public Overridable Function [get](ByVal field As Integer) As Integer
			complete()
			Return internalGet(field)
		End Function

		''' <summary>
		''' Returns the value of the given calendar field. This method does
		''' not involve normalization or validation of the field value.
		''' </summary>
		''' <param name="field"> the given calendar field. </param>
		''' <returns> the value for the given calendar field. </returns>
		''' <seealso cref= #get(int) </seealso>
		Protected Friend Function internalGet(ByVal field As Integer) As Integer
			Return fields(field)
		End Function

		''' <summary>
		''' Sets the value of the given calendar field. This method does
		''' not affect any setting state of the field in this
		''' <code>Calendar</code> instance.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if the specified field is out of range
		'''             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		''' <seealso cref= #areFieldsSet </seealso>
		''' <seealso cref= #isTimeSet </seealso>
		''' <seealso cref= #areAllFieldsSet </seealso>
		''' <seealso cref= #set(int,int) </seealso>
		Friend Sub internalSet(ByVal field As Integer, ByVal value As Integer)
			fields(field) = value
		End Sub

		''' <summary>
		''' Sets the given calendar field to the given value. The value is not
		''' interpreted by this method regardless of the leniency mode.
		''' </summary>
		''' <param name="field"> the given calendar field. </param>
		''' <param name="value"> the value to be set for the given calendar field. </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the specified field is out of range
		'''             (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>).
		''' in non-lenient mode. </exception>
		''' <seealso cref= #set(int,int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int,int) </seealso>
		''' <seealso cref= #get(int) </seealso>
		Public Overridable Sub [set](ByVal field As Integer, ByVal value As Integer)
			' If the fields are partially normalized, calculate all the
			' fields before changing any fields.
			If areFieldsSet AndAlso (Not areAllFieldsSet) Then computeFields()
			internalSet(field, value)
			isTimeSet = False
			areFieldsSet = False
			isSet_Renamed(field) = True
			stamp(field) = nextStamp
			nextStamp += 1
			If nextStamp =  java.lang.[Integer].Max_Value Then adjustStamp()
		End Sub

		''' <summary>
		''' Sets the values for the calendar fields <code>YEAR</code>,
		''' <code>MONTH</code>, and <code>DAY_OF_MONTH</code>.
		''' Previous values of other calendar fields are retained.  If this is not desired,
		''' call <seealso cref="#clear()"/> first.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		''' <seealso cref= #set(int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int,int) </seealso>
		Public Sub [set](ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer)
			[set](Calendar.YEAR, year)
			[set](Calendar.MONTH, month)
			[set](Calendar.DATE, date_Renamed)
		End Sub

		''' <summary>
		''' Sets the values for the calendar fields <code>YEAR</code>,
		''' <code>MONTH</code>, <code>DAY_OF_MONTH</code>,
		''' <code>HOUR_OF_DAY</code>, and <code>MINUTE</code>.
		''' Previous values of other fields are retained.  If this is not desired,
		''' call <seealso cref="#clear()"/> first.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		''' <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field. </param>
		''' <param name="minute"> the value used to set the <code>MINUTE</code> calendar field. </param>
		''' <seealso cref= #set(int,int) </seealso>
		''' <seealso cref= #set(int,int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int,int) </seealso>
		Public Sub [set](ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer, ByVal hourOfDay As Integer, ByVal minute As Integer)
			[set](Calendar.YEAR, year)
			[set](Calendar.MONTH, month)
			[set](Calendar.DATE, date_Renamed)
			[set](HOUR_OF_DAY, hourOfDay)
			[set](Calendar.MINUTE, minute)
		End Sub

		''' <summary>
		''' Sets the values for the fields <code>YEAR</code>, <code>MONTH</code>,
		''' <code>DAY_OF_MONTH</code>, <code>HOUR_OF_DAY</code>, <code>MINUTE</code>, and
		''' <code>SECOND</code>.
		''' Previous values of other fields are retained.  If this is not desired,
		''' call <seealso cref="#clear()"/> first.
		''' </summary>
		''' <param name="year"> the value used to set the <code>YEAR</code> calendar field. </param>
		''' <param name="month"> the value used to set the <code>MONTH</code> calendar field.
		''' Month value is 0-based. e.g., 0 for January. </param>
		''' <param name="date"> the value used to set the <code>DAY_OF_MONTH</code> calendar field. </param>
		''' <param name="hourOfDay"> the value used to set the <code>HOUR_OF_DAY</code> calendar field. </param>
		''' <param name="minute"> the value used to set the <code>MINUTE</code> calendar field. </param>
		''' <param name="second"> the value used to set the <code>SECOND</code> calendar field. </param>
		''' <seealso cref= #set(int,int) </seealso>
		''' <seealso cref= #set(int,int,int) </seealso>
		''' <seealso cref= #set(int,int,int,int,int) </seealso>
		Public Sub [set](ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer, ByVal hourOfDay As Integer, ByVal minute As Integer, ByVal second As Integer)
			[set](Calendar.YEAR, year)
			[set](Calendar.MONTH, month)
			[set](Calendar.DATE, date_Renamed)
			[set](HOUR_OF_DAY, hourOfDay)
			[set](Calendar.MINUTE, minute)
			[set](Calendar.SECOND, second)
		End Sub

		''' <summary>
		''' Sets all the calendar field values and the time value
		''' (millisecond offset from the <a href="#Epoch">Epoch</a>) of
		''' this <code>Calendar</code> undefined. This means that {@link
		''' #isSet(int) isSet()} will return <code>false</code> for all the
		''' calendar fields, and the date and time calculations will treat
		''' the fields as if they had never been set. A
		''' <code>Calendar</code> implementation class may use its specific
		''' default field values for date/time calculations. For example,
		''' <code>GregorianCalendar</code> uses 1970 if the
		''' <code>YEAR</code> field value is undefined.
		''' </summary>
		''' <seealso cref= #clear(int) </seealso>
		Public Sub clear()
			Dim i As Integer = 0
			Do While i < fields.Length
					fields(i) = 0
					stamp(i) = fields(i)
				isSet_Renamed(i) = False
				i += 1
			Loop
				areFieldsSet = False
				areAllFieldsSet = areFieldsSet
			isTimeSet = False
		End Sub

		''' <summary>
		''' Sets the given calendar field value and the time value
		''' (millisecond offset from the <a href="#Epoch">Epoch</a>) of
		''' this <code>Calendar</code> undefined. This means that {@link
		''' #isSet(int) isSet(field)} will return <code>false</code>, and
		''' the date and time calculations will treat the field as if it
		''' had never been set. A <code>Calendar</code> implementation
		''' class may use the field's specific default value for date and
		''' time calculations.
		''' 
		''' <p>The <seealso cref="#HOUR_OF_DAY"/>, <seealso cref="#HOUR"/> and <seealso cref="#AM_PM"/>
		''' fields are handled independently and the <a
		''' href="#time_resolution">the resolution rule for the time of
		''' day</a> is applied. Clearing one of the fields doesn't reset
		''' the hour of day value of this <code>Calendar</code>. Use {@link
		''' #set(int,int) set(Calendar.HOUR_OF_DAY, 0)} to reset the hour
		''' value.
		''' </summary>
		''' <param name="field"> the calendar field to be cleared. </param>
		''' <seealso cref= #clear() </seealso>
		Public Sub clear(ByVal field As Integer)
			fields(field) = 0
			stamp(field) = UNSET
			isSet_Renamed(field) = False

				areFieldsSet = False
				areAllFieldsSet = areFieldsSet
			isTimeSet = False
		End Sub

		''' <summary>
		''' Determines if the given calendar field has a value set,
		''' including cases that the value has been set by internal fields
		''' calculations triggered by a <code>get</code> method call.
		''' </summary>
		''' <param name="field"> the calendar field to test </param>
		''' <returns> <code>true</code> if the given calendar field has a value set;
		''' <code>false</code> otherwise. </returns>
		Public Function isSet(ByVal field As Integer) As Boolean
			Return stamp(field) <> UNSET
		End Function

		''' <summary>
		''' Returns the string representation of the calendar
		''' <code>field</code> value in the given <code>style</code> and
		''' <code>locale</code>.  If no string representation is
		''' applicable, <code>null</code> is returned. This method calls
		''' <seealso cref="Calendar#get(int) get(field)"/> to get the calendar
		''' <code>field</code> value if the string representation is
		''' applicable to the given calendar <code>field</code>.
		''' 
		''' <p>For example, if this <code>Calendar</code> is a
		''' <code>GregorianCalendar</code> and its date is 2005-01-01, then
		''' the string representation of the <seealso cref="#MONTH"/> field would be
		''' "January" in the long style in an English locale or "Jan" in
		''' the short style. However, no string representation would be
		''' available for the <seealso cref="#DAY_OF_MONTH"/> field, and this method
		''' would return <code>null</code>.
		''' 
		''' <p>The default implementation supports the calendar fields for
		''' which a <seealso cref="DateFormatSymbols"/> has names in the given
		''' <code>locale</code>.
		''' </summary>
		''' <param name="field">
		'''        the calendar field for which the string representation
		'''        is returned </param>
		''' <param name="style">
		'''        the style applied to the string representation; one of {@link
		'''        #SHORT_FORMAT} (<seealso cref="#SHORT"/>), <seealso cref="#SHORT_STANDALONE"/>,
		'''        <seealso cref="#LONG_FORMAT"/> (<seealso cref="#LONG"/>), <seealso cref="#LONG_STANDALONE"/>,
		'''        <seealso cref="#NARROW_FORMAT"/>, or <seealso cref="#NARROW_STANDALONE"/>. </param>
		''' <param name="locale">
		'''        the locale for the string representation
		'''        (any calendar types specified by {@code locale} are ignored) </param>
		''' <returns> the string representation of the given
		'''        {@code field} in the given {@code style}, or
		'''        {@code null} if no string representation is
		'''        applicable. </returns>
		''' <exception cref="IllegalArgumentException">
		'''        if {@code field} or {@code style} is invalid,
		'''        or if this {@code Calendar} is non-lenient and any
		'''        of the calendar fields have invalid values </exception>
		''' <exception cref="NullPointerException">
		'''        if {@code locale} is null
		''' @since 1.6 </exception>
		Public Overridable Function getDisplayName(ByVal field As Integer, ByVal style As Integer, ByVal locale_Renamed As Locale) As String
			If Not checkDisplayNameParams(field, style, [SHORT], NARROW_FORMAT, locale_Renamed, ERA_MASK Or MONTH_MASK Or DAY_OF_WEEK_MASK Or AM_PM_MASK) Then Return Nothing

			Dim calendarType_Renamed As String = calendarType
			Dim fieldValue As Integer = [get](field)
			' the standalone and narrow styles are supported only through CalendarDataProviders.
			If isStandaloneStyle(style) OrElse isNarrowFormatStyle(style) Then
				Dim val As String = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueName(calendarType_Renamed, field, fieldValue, style, locale_Renamed)
				' Perform fallback here to follow the CLDR rules
				If val Is Nothing Then
					If isNarrowFormatStyle(style) Then
						val = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueName(calendarType_Renamed, field, fieldValue, toStandaloneStyle(style), locale_Renamed)
					ElseIf isStandaloneStyle(style) Then
						val = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueName(calendarType_Renamed, field, fieldValue, getBaseStyle(style), locale_Renamed)
					End If
				End If
				Return val
			End If

			Dim symbols As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(locale_Renamed)
			Dim strings As String() = getFieldStrings(field, style, symbols)
			If strings IsNot Nothing Then
				If fieldValue < strings.Length Then Return strings(fieldValue)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns a {@code Map} containing all names of the calendar
		''' {@code field} in the given {@code style} and
		''' {@code locale} and their corresponding field values. For
		''' example, if this {@code Calendar} is a {@link
		''' GregorianCalendar}, the returned map would contain "Jan" to
		''' <seealso cref="#JANUARY"/>, "Feb" to <seealso cref="#FEBRUARY"/>, and so on, in the
		''' <seealso cref="#SHORT short"/> style in an English locale.
		''' 
		''' <p>Narrow names may not be unique due to use of single characters,
		''' such as "S" for Sunday and Saturday. In that case narrow names are not
		''' included in the returned {@code Map}.
		''' 
		''' <p>The values of other calendar fields may be taken into
		''' account to determine a set of display names. For example, if
		''' this {@code Calendar} is a lunisolar calendar system and
		''' the year value given by the <seealso cref="#YEAR"/> field has a leap
		''' month, this method would return month names containing the leap
		''' month name, and month names are mapped to their values specific
		''' for the year.
		''' 
		''' <p>The default implementation supports display names contained in
		''' a <seealso cref="DateFormatSymbols"/>. For example, if {@code field}
		''' is <seealso cref="#MONTH"/> and {@code style} is {@link
		''' #ALL_STYLES}, this method returns a {@code Map} containing
		''' all strings returned by <seealso cref="DateFormatSymbols#getShortMonths()"/>
		''' and <seealso cref="DateFormatSymbols#getMonths()"/>.
		''' </summary>
		''' <param name="field">
		'''        the calendar field for which the display names are returned </param>
		''' <param name="style">
		'''        the style applied to the string representation; one of {@link
		'''        #SHORT_FORMAT} (<seealso cref="#SHORT"/>), <seealso cref="#SHORT_STANDALONE"/>,
		'''        <seealso cref="#LONG_FORMAT"/> (<seealso cref="#LONG"/>), <seealso cref="#LONG_STANDALONE"/>,
		'''        <seealso cref="#NARROW_FORMAT"/>, or <seealso cref="#NARROW_STANDALONE"/> </param>
		''' <param name="locale">
		'''        the locale for the display names </param>
		''' <returns> a {@code Map} containing all display names in
		'''        {@code style} and {@code locale} and their
		'''        field values, or {@code null} if no display names
		'''        are defined for {@code field} </returns>
		''' <exception cref="IllegalArgumentException">
		'''        if {@code field} or {@code style} is invalid,
		'''        or if this {@code Calendar} is non-lenient and any
		'''        of the calendar fields have invalid values </exception>
		''' <exception cref="NullPointerException">
		'''        if {@code locale} is null
		''' @since 1.6 </exception>
		Public Overridable Function getDisplayNames(ByVal field As Integer, ByVal style As Integer, ByVal locale_Renamed As Locale) As Map(Of String, Integer?)
			If Not checkDisplayNameParams(field, style, ALL_STYLES, NARROW_FORMAT, locale_Renamed, ERA_MASK Or MONTH_MASK Or DAY_OF_WEEK_MASK Or AM_PM_MASK) Then Return Nothing

			Dim calendarType_Renamed As String = calendarType
			If style = ALL_STYLES OrElse isStandaloneStyle(style) OrElse isNarrowFormatStyle(style) Then
				Dim map As Map(Of String, Integer?)
				map = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueNames(calendarType_Renamed, field, style, locale_Renamed)

				' Perform fallback here to follow the CLDR rules
				If map Is Nothing Then
					If isNarrowFormatStyle(style) Then
						map = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueNames(calendarType_Renamed, field, toStandaloneStyle(style), locale_Renamed)
					ElseIf style <> ALL_STYLES Then
						map = sun.util.locale.provider.CalendarDataUtility.retrieveFieldValueNames(calendarType_Renamed, field, getBaseStyle(style), locale_Renamed)
					End If
				End If
				Return map
			End If

			' SHORT or LONG
			Return getDisplayNamesImpl(field, style, locale_Renamed)
		End Function

		Private Function getDisplayNamesImpl(ByVal field As Integer, ByVal style As Integer, ByVal locale_Renamed As Locale) As Map(Of String, Integer?)
			Dim symbols As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(locale_Renamed)
			Dim strings As String() = getFieldStrings(field, style, symbols)
			If strings IsNot Nothing Then
				Dim names As Map(Of String, Integer?) = New HashMap(Of String, Integer?)
				For i As Integer = 0 To strings.Length - 1
					If strings(i).length() = 0 Then Continue For
					names.put(strings(i), i)
				Next i
				Return names
			End If
			Return Nothing
		End Function

		Friend Overridable Function checkDisplayNameParams(ByVal field As Integer, ByVal style As Integer, ByVal minStyle As Integer, ByVal maxStyle As Integer, ByVal locale_Renamed As Locale, ByVal fieldMask As Integer) As Boolean
			Dim baseStyle_Renamed As Integer = getBaseStyle(style) ' Ignore the standalone mask
			If field < 0 OrElse field >= fields.Length OrElse baseStyle_Renamed < minStyle OrElse baseStyle_Renamed > maxStyle Then Throw New IllegalArgumentException
			If locale_Renamed Is Nothing Then Throw New NullPointerException
			Return isFieldSet(fieldMask, field)
		End Function

		Private Function getFieldStrings(ByVal field As Integer, ByVal style As Integer, ByVal symbols As java.text.DateFormatSymbols) As String()
			Dim baseStyle_Renamed As Integer = getBaseStyle(style) ' ignore the standalone mask

			' DateFormatSymbols doesn't support any narrow names.
			If baseStyle_Renamed = NARROW_FORMAT Then Return Nothing

			Dim strings As String() = Nothing
			Select Case field
			Case ERA
				strings = symbols.eras

			Case MONTH
				strings = If(baseStyle_Renamed = [LONG], symbols.months, symbols.shortMonths)

			Case DAY_OF_WEEK
				strings = If(baseStyle_Renamed = [LONG], symbols.weekdays, symbols.shortWeekdays)

			Case AM_PM
				strings = symbols.amPmStrings
			End Select
			Return strings
		End Function

		''' <summary>
		''' Fills in any unset fields in the calendar fields. First, the {@link
		''' #computeTime()} method is called if the time value (millisecond offset
		''' from the <a href="#Epoch">Epoch</a>) has not been calculated from
		''' calendar field values. Then, the <seealso cref="#computeFields()"/> method is
		''' called to calculate all calendar field values.
		''' </summary>
		Protected Friend Overridable Sub complete()
			If Not isTimeSet Then updateTime()
			If (Not areFieldsSet) OrElse (Not areAllFieldsSet) Then
				computeFields() ' fills in unset fields
					areFieldsSet = True
					areAllFieldsSet = areFieldsSet
			End If
		End Sub

		''' <summary>
		''' Returns whether the value of the specified calendar field has been set
		''' externally by calling one of the setter methods rather than by the
		''' internal time calculation.
		''' </summary>
		''' <returns> <code>true</code> if the field has been set externally,
		''' <code>false</code> otherwise. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the specified
		'''                <code>field</code> is out of range
		'''               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		''' <seealso cref= #selectFields() </seealso>
		''' <seealso cref= #setFieldsComputed(int) </seealso>
		Friend Function isExternallySet(ByVal field As Integer) As Boolean
			Return stamp(field) >= MINIMUM_USER_STAMP
		End Function

		''' <summary>
		''' Returns a field mask (bit mask) indicating all calendar fields that
		''' have the state of externally or internally set.
		''' </summary>
		''' <returns> a bit mask indicating set state fields </returns>
		Friend Property setStateFields As Integer
			Get
				Dim mask As Integer = 0
				For i As Integer = 0 To fields.Length - 1
					If stamp(i) <> UNSET Then mask = mask Or 1 << i
				Next i
				Return mask
			End Get
		End Property

		''' <summary>
		''' Sets the state of the specified calendar fields to
		''' <em>computed</em>. This state means that the specified calendar fields
		''' have valid values that have been set by internal time calculation
		''' rather than by calling one of the setter methods.
		''' </summary>
		''' <param name="fieldMask"> the field to be marked as computed. </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified
		'''                <code>field</code> is out of range
		'''               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		''' <seealso cref= #isExternallySet(int) </seealso>
		''' <seealso cref= #selectFields() </seealso>
		Friend Property fieldsComputed As Integer
			Set(ByVal fieldMask As Integer)
				If fieldMask = ALL_FIELDS Then
					For i As Integer = 0 To fields.Length - 1
						stamp(i) = COMPUTED
						isSet_Renamed(i) = True
					Next i
						areAllFieldsSet = True
						areFieldsSet = areAllFieldsSet
				Else
					For i As Integer = 0 To fields.Length - 1
						If (fieldMask And 1) = 1 Then
							stamp(i) = COMPUTED
							isSet_Renamed(i) = True
						Else
							If areAllFieldsSet AndAlso (Not isSet_Renamed(i)) Then areAllFieldsSet = False
						End If
						fieldMask >>>= 1
					Next i
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the state of the calendar fields that are <em>not</em> specified
		''' by <code>fieldMask</code> to <em>unset</em>. If <code>fieldMask</code>
		''' specifies all the calendar fields, then the state of this
		''' <code>Calendar</code> becomes that all the calendar fields are in sync
		''' with the time value (millisecond offset from the Epoch).
		''' </summary>
		''' <param name="fieldMask"> the field mask indicating which calendar fields are in
		''' sync with the time value. </param>
		''' <exception cref="IndexOutOfBoundsException"> if the specified
		'''                <code>field</code> is out of range
		'''               (<code>field &lt; 0 || field &gt;= FIELD_COUNT</code>). </exception>
		''' <seealso cref= #isExternallySet(int) </seealso>
		''' <seealso cref= #selectFields() </seealso>
		Friend Property fieldsNormalized As Integer
			Set(ByVal fieldMask As Integer)
				If fieldMask <> ALL_FIELDS Then
					For i As Integer = 0 To fields.Length - 1
						If (fieldMask And 1) = 0 Then
								fields(i) = 0
								stamp(i) = fields(i)
							isSet_Renamed(i) = False
						End If
						fieldMask >>= 1
					Next i
				End If
    
				' Some or all of the fields are in sync with the
				' milliseconds, but the stamp values are not normalized yet.
				areFieldsSet = True
				areAllFieldsSet = False
			End Set
		End Property

		''' <summary>
		''' Returns whether the calendar fields are partially in sync with the time
		''' value or fully in sync but not stamp values are not normalized yet.
		''' </summary>
		Friend Property partiallyNormalized As Boolean
			Get
				Return areFieldsSet AndAlso Not areAllFieldsSet
			End Get
		End Property

		''' <summary>
		''' Returns whether the calendar fields are fully in sync with the time
		''' value.
		''' </summary>
		Friend Property fullyNormalized As Boolean
			Get
				Return areFieldsSet AndAlso areAllFieldsSet
			End Get
		End Property

		''' <summary>
		''' Marks this Calendar as not sync'd.
		''' </summary>
		Friend Sub setUnnormalized()
				areAllFieldsSet = False
				areFieldsSet = areAllFieldsSet
		End Sub

		''' <summary>
		''' Returns whether the specified <code>field</code> is on in the
		''' <code>fieldMask</code>.
		''' </summary>
		Friend Shared Function isFieldSet(ByVal fieldMask As Integer, ByVal field As Integer) As Boolean
			Return (fieldMask And (1 << field)) <> 0
		End Function

		''' <summary>
		''' Returns a field mask indicating which calendar field values
		''' to be used to calculate the time value. The calendar fields are
		''' returned as a bit mask, each bit of which corresponds to a field, i.e.,
		''' the mask value of <code>field</code> is <code>(1 &lt;&lt;
		''' field)</code>. For example, 0x26 represents the <code>YEAR</code>,
		''' <code>MONTH</code>, and <code>DAY_OF_MONTH</code> fields (i.e., 0x26 is
		''' equal to
		''' <code>(1&lt;&lt;YEAR)|(1&lt;&lt;MONTH)|(1&lt;&lt;DAY_OF_MONTH))</code>.
		''' 
		''' <p>This method supports the calendar fields resolution as described in
		''' the class description. If the bit mask for a given field is on and its
		''' field has not been set (i.e., <code>isSet(field)</code> is
		''' <code>false</code>), then the default value of the field has to be
		''' used, which case means that the field has been selected because the
		''' selected combination involves the field.
		''' </summary>
		''' <returns> a bit mask of selected fields </returns>
		''' <seealso cref= #isExternallySet(int) </seealso>
		Friend Function selectFields() As Integer
			' This implementation has been taken from the GregorianCalendar class.

			' The YEAR field must always be used regardless of its SET
			' state because YEAR is a mandatory field to determine the date
			' and the default value (EPOCH_YEAR) may change through the
			' normalization process.
			Dim fieldMask As Integer = YEAR_MASK

			If stamp(ERA) <> UNSET Then fieldMask = fieldMask Or ERA_MASK
			' Find the most recent group of fields specifying the day within
			' the year.  These may be any of the following combinations:
			'   MONTH + DAY_OF_MONTH
			'   MONTH + WEEK_OF_MONTH + DAY_OF_WEEK
			'   MONTH + DAY_OF_WEEK_IN_MONTH + DAY_OF_WEEK
			'   DAY_OF_YEAR
			'   WEEK_OF_YEAR + DAY_OF_WEEK
			' We look for the most recent of the fields in each group to determine
			' the age of the group.  For groups involving a week-related field such
			' as WEEK_OF_MONTH, DAY_OF_WEEK_IN_MONTH, or WEEK_OF_YEAR, both the
			' week-related field and the DAY_OF_WEEK must be set for the group as a
			' whole to be considered.  (See bug 4153860 - liu 7/24/98.)
			Dim dowStamp As Integer = stamp(DAY_OF_WEEK)
			Dim monthStamp As Integer = stamp(MONTH)
			Dim domStamp As Integer = stamp(DAY_OF_MONTH)
			Dim womStamp As Integer = aggregateStamp(stamp(WEEK_OF_MONTH), dowStamp)
			Dim dowimStamp As Integer = aggregateStamp(stamp(DAY_OF_WEEK_IN_MONTH), dowStamp)
			Dim doyStamp As Integer = stamp(DAY_OF_YEAR)
			Dim woyStamp As Integer = aggregateStamp(stamp(WEEK_OF_YEAR), dowStamp)

			Dim bestStamp As Integer = domStamp
			If womStamp > bestStamp Then bestStamp = womStamp
			If dowimStamp > bestStamp Then bestStamp = dowimStamp
			If doyStamp > bestStamp Then bestStamp = doyStamp
			If woyStamp > bestStamp Then bestStamp = woyStamp

	'         No complete combination exists.  Look for WEEK_OF_MONTH,
	'         * DAY_OF_WEEK_IN_MONTH, or WEEK_OF_YEAR alone.  Treat DAY_OF_WEEK alone
	'         * as DAY_OF_WEEK_IN_MONTH.
	'         
			If bestStamp = UNSET Then
				womStamp = stamp(WEEK_OF_MONTH)
				dowimStamp = System.Math.Max(stamp(DAY_OF_WEEK_IN_MONTH), dowStamp)
				woyStamp = stamp(WEEK_OF_YEAR)
				bestStamp = System.Math.Max (System.Math.Max(womStamp, dowimStamp), woyStamp)

	'             Treat MONTH alone or no fields at all as DAY_OF_MONTH.  This may
	'             * result in bestStamp = domStamp = UNSET if no fields are set,
	'             * which indicates DAY_OF_MONTH.
	'             
				If bestStamp = UNSET Then
						domStamp = monthStamp
						bestStamp = domStamp
				End If
			End If

			If bestStamp = domStamp OrElse (bestStamp = womStamp AndAlso stamp(WEEK_OF_MONTH) >= stamp(WEEK_OF_YEAR)) OrElse (bestStamp = dowimStamp AndAlso stamp(DAY_OF_WEEK_IN_MONTH) >= stamp(WEEK_OF_YEAR)) Then
				fieldMask = fieldMask Or MONTH_MASK
				If bestStamp = domStamp Then
					fieldMask = fieldMask Or DAY_OF_MONTH_MASK
				Else
					assert(bestStamp = womStamp OrElse bestStamp = dowimStamp)
					If dowStamp <> UNSET Then fieldMask = fieldMask Or DAY_OF_WEEK_MASK
					If womStamp = dowimStamp Then
						' When they are equal, give the priority to
						' WEEK_OF_MONTH for compatibility.
						If stamp(WEEK_OF_MONTH) >= stamp(DAY_OF_WEEK_IN_MONTH) Then
							fieldMask = fieldMask Or WEEK_OF_MONTH_MASK
						Else
							fieldMask = fieldMask Or DAY_OF_WEEK_IN_MONTH_MASK
						End If
					Else
						If bestStamp = womStamp Then
							fieldMask = fieldMask Or WEEK_OF_MONTH_MASK
						Else
							assert(bestStamp = dowimStamp)
							If stamp(DAY_OF_WEEK_IN_MONTH) <> UNSET Then fieldMask = fieldMask Or DAY_OF_WEEK_IN_MONTH_MASK
						End If
					End If
				End If
			Else
				assert(bestStamp = doyStamp OrElse bestStamp = woyStamp OrElse bestStamp = UNSET)
				If bestStamp = doyStamp Then
					fieldMask = fieldMask Or DAY_OF_YEAR_MASK
				Else
					assert(bestStamp = woyStamp)
					If dowStamp <> UNSET Then fieldMask = fieldMask Or DAY_OF_WEEK_MASK
					fieldMask = fieldMask Or WEEK_OF_YEAR_MASK
				End If
			End If

			' Find the best set of fields specifying the time of day.  There
			' are only two possibilities here; the HOUR_OF_DAY or the
			' AM_PM and the HOUR.
			Dim hourOfDayStamp As Integer = stamp(HOUR_OF_DAY)
			Dim hourStamp As Integer = aggregateStamp(stamp(HOUR), stamp(AM_PM))
			bestStamp = If(hourStamp > hourOfDayStamp, hourStamp, hourOfDayStamp)

			' if bestStamp is still UNSET, then take HOUR or AM_PM. (See 4846659)
			If bestStamp = UNSET Then bestStamp = System.Math.Max(stamp(HOUR), stamp(AM_PM))

			' Hours
			If bestStamp <> UNSET Then
				If bestStamp = hourOfDayStamp Then
					fieldMask = fieldMask Or HOUR_OF_DAY_MASK
				Else
					fieldMask = fieldMask Or HOUR_MASK
					If stamp(AM_PM) <> UNSET Then fieldMask = fieldMask Or AM_PM_MASK
				End If
			End If
			If stamp(MINUTE) <> UNSET Then fieldMask = fieldMask Or MINUTE_MASK
			If stamp(SECOND) <> UNSET Then fieldMask = fieldMask Or SECOND_MASK
			If stamp(MILLISECOND) <> UNSET Then fieldMask = fieldMask Or MILLISECOND_MASK
			If stamp(ZONE_OFFSET) >= MINIMUM_USER_STAMP Then fieldMask = fieldMask Or ZONE_OFFSET_MASK
			If stamp(DST_OFFSET) >= MINIMUM_USER_STAMP Then fieldMask = fieldMask Or DST_OFFSET_MASK

			Return fieldMask
		End Function

		Friend Overridable Function getBaseStyle(ByVal style As Integer) As Integer
			Return style And Not STANDALONE_MASK
		End Function

		Private Function toStandaloneStyle(ByVal style As Integer) As Integer
			Return style Or STANDALONE_MASK
		End Function

		Private Function isStandaloneStyle(ByVal style As Integer) As Boolean
			Return (style And STANDALONE_MASK) <> 0
		End Function

		Private Function isNarrowStyle(ByVal style As Integer) As Boolean
			Return style = NARROW_FORMAT OrElse style = NARROW_STANDALONE
		End Function

		Private Function isNarrowFormatStyle(ByVal style As Integer) As Boolean
			Return style = NARROW_FORMAT
		End Function

		''' <summary>
		''' Returns the pseudo-time-stamp for two fields, given their
		''' individual pseudo-time-stamps.  If either of the fields
		''' is unset, then the aggregate is unset.  Otherwise, the
		''' aggregate is the later of the two stamps.
		''' </summary>
		Private Shared Function aggregateStamp(ByVal stamp_a As Integer, ByVal stamp_b As Integer) As Integer
			If stamp_a = UNSET OrElse stamp_b = UNSET Then Return UNSET
			Return If(stamp_a > stamp_b, stamp_a, stamp_b)
		End Function

		''' <summary>
		''' Returns an unmodifiable {@code Set} containing all calendar types
		''' supported by {@code Calendar} in the runtime environment. The available
		''' calendar types can be used for the <a
		''' href="Locale.html#def_locale_extension">Unicode locale extensions</a>.
		''' The {@code Set} returned contains at least {@code "gregory"}. The
		''' calendar types don't include aliases, such as {@code "gregorian"} for
		''' {@code "gregory"}.
		''' </summary>
		''' <returns> an unmodifiable {@code Set} containing all available calendar types
		''' @since 1.8 </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		''' <seealso cref= Calendar.Builder#setCalendarType(String) </seealso>
		''' <seealso cref= Locale#getUnicodeLocaleType(String) </seealso>
		PublicShared ReadOnly PropertyavailableCalendarTypes As [Set](Of String)
			Get
				Return AvailableCalendarTypes.SET
			End Get
		End Property

		Private Class AvailableCalendarTypes
			Private Shared ReadOnly [SET] As [Set](Of String)
			Shared Sub New()
				Dim set_Renamed As [Set](Of String) = New HashSet(Of String)(3)
				set_Renamed.add("gregory")
				set_Renamed.add("buddhist")
				set_Renamed.add("japanese")
				[SET] = Collections.unmodifiableSet(set_Renamed)
			End Sub
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Returns the calendar type of this {@code Calendar}. Calendar types are
		''' defined by the <em>Unicode Locale Data Markup Language (LDML)</em>
		''' specification.
		''' 
		''' <p>The default implementation of this method returns the class name of
		''' this {@code Calendar} instance. Any subclasses that implement
		''' LDML-defined calendar systems should override this method to return
		''' appropriate calendar types.
		''' </summary>
		''' <returns> the LDML-defined calendar type or the class name of this
		'''         {@code Calendar} instance
		''' @since 1.8 </returns>
		''' <seealso cref= <a href="Locale.html#def_extensions">Locale extensions</a> </seealso>
		''' <seealso cref= Locale.Builder#setLocale(Locale) </seealso>
		''' <seealso cref= Locale.Builder#setUnicodeLocaleKeyword(String, String) </seealso>
		Public Overridable Property calendarType As String
			Get
				Return Me.GetType().name
			End Get
		End Property

		''' <summary>
		''' Compares this <code>Calendar</code> to the specified
		''' <code>Object</code>.  The result is <code>true</code> if and only if
		''' the argument is a <code>Calendar</code> object of the same calendar
		''' system that represents the same time value (millisecond offset from the
		''' <a href="#Epoch">Epoch</a>) under the same
		''' <code>Calendar</code> parameters as this object.
		''' 
		''' <p>The <code>Calendar</code> parameters are the values represented
		''' by the <code>isLenient</code>, <code>getFirstDayOfWeek</code>,
		''' <code>getMinimalDaysInFirstWeek</code> and <code>getTimeZone</code>
		''' methods. If there is any difference in those parameters
		''' between the two <code>Calendar</code>s, this method returns
		''' <code>false</code>.
		''' 
		''' <p>Use the <seealso cref="#compareTo(Calendar) compareTo"/> method to
		''' compare only the time values.
		''' </summary>
		''' <param name="obj"> the object to compare with. </param>
		''' <returns> <code>true</code> if this object is equal to <code>obj</code>;
		''' <code>false</code> otherwise. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			Try
				Dim that As Calendar = CType(obj, Calendar)
				Return compareTo(getMillisOf(that)) = 0 AndAlso lenient = that.lenient AndAlso firstDayOfWeek = that.firstDayOfWeek AndAlso minimalDaysInFirstWeek = that.minimalDaysInFirstWeek AndAlso zone.Equals(that.zone)
			Catch e As Exception
				' Note: GregorianCalendar.computeTime throws
				' IllegalArgumentException if the ERA value is invalid
				' even it's in lenient mode.
			End Try
			Return False
		End Function

		''' <summary>
		''' Returns a hash code for this calendar.
		''' </summary>
		''' <returns> a hash code value for this object.
		''' @since 1.2 </returns>
		Public Overrides Function GetHashCode() As Integer
			' 'otheritems' represents the hash code for the previous versions.
			Dim otheritems As Integer = (If(lenient, 1, 0)) Or (firstDayOfWeek << 1) Or (minimalDaysInFirstWeek << 4) Or (zone.GetHashCode() << 7)
			Dim t As Long = getMillisOf(Me)
			Return CInt(t) Xor CInt(Fix(t >> 32)) Xor otheritems
		End Function

		''' <summary>
		''' Returns whether this <code>Calendar</code> represents a time
		''' before the time represented by the specified
		''' <code>Object</code>. This method is equivalent to:
		''' <pre>{@code
		'''         compareTo(when) < 0
		''' }</pre>
		''' if and only if <code>when</code> is a <code>Calendar</code>
		''' instance. Otherwise, the method returns <code>false</code>.
		''' </summary>
		''' <param name="when"> the <code>Object</code> to be compared </param>
		''' <returns> <code>true</code> if the time of this
		''' <code>Calendar</code> is before the time represented by
		''' <code>when</code>; <code>false</code> otherwise. </returns>
		''' <seealso cref=     #compareTo(Calendar) </seealso>
		Public Overridable Function before(ByVal [when] As Object) As Boolean
			Return TypeOf [when] Is Calendar AndAlso compareTo(CType([when], Calendar)) < 0
		End Function

		''' <summary>
		''' Returns whether this <code>Calendar</code> represents a time
		''' after the time represented by the specified
		''' <code>Object</code>. This method is equivalent to:
		''' <pre>{@code
		'''         compareTo(when) > 0
		''' }</pre>
		''' if and only if <code>when</code> is a <code>Calendar</code>
		''' instance. Otherwise, the method returns <code>false</code>.
		''' </summary>
		''' <param name="when"> the <code>Object</code> to be compared </param>
		''' <returns> <code>true</code> if the time of this <code>Calendar</code> is
		''' after the time represented by <code>when</code>; <code>false</code>
		''' otherwise. </returns>
		''' <seealso cref=     #compareTo(Calendar) </seealso>
		Public Overridable Function after(ByVal [when] As Object) As Boolean
			Return TypeOf [when] Is Calendar AndAlso compareTo(CType([when], Calendar)) > 0
		End Function

		''' <summary>
		''' Compares the time values (millisecond offsets from the <a
		''' href="#Epoch">Epoch</a>) represented by two
		''' <code>Calendar</code> objects.
		''' </summary>
		''' <param name="anotherCalendar"> the <code>Calendar</code> to be compared. </param>
		''' <returns> the value <code>0</code> if the time represented by the argument
		''' is equal to the time represented by this <code>Calendar</code>; a value
		''' less than <code>0</code> if the time of this <code>Calendar</code> is
		''' before the time represented by the argument; and a value greater than
		''' <code>0</code> if the time of this <code>Calendar</code> is after the
		''' time represented by the argument. </returns>
		''' <exception cref="NullPointerException"> if the specified <code>Calendar</code> is
		'''            <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the time value of the
		''' specified <code>Calendar</code> object can't be obtained due to
		''' any invalid calendar values.
		''' @since   1.5 </exception>
		Public Overrides Function compareTo(ByVal anotherCalendar As Calendar) As Integer Implements Comparable(Of Calendar).compareTo
			Return compareTo(getMillisOf(anotherCalendar))
		End Function

		''' <summary>
		''' Adds or subtracts the specified amount of time to the given calendar field,
		''' based on the calendar's rules. For example, to subtract 5 days from
		''' the current time of the calendar, you can achieve it by calling:
		''' <p><code>add(Calendar.DAY_OF_MONTH, -5)</code>.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <param name="amount"> the amount of date or time to be added to the field. </param>
		''' <seealso cref= #roll(int,int) </seealso>
		''' <seealso cref= #set(int,int) </seealso>
		Public MustOverride Sub add(ByVal field As Integer, ByVal amount As Integer)

		''' <summary>
		''' Adds or subtracts (up/down) a single unit of time on the given time
		''' field without changing larger fields. For example, to roll the current
		''' date up by one day, you can achieve it by calling:
		''' <p>roll(Calendar.DATE, true).
		''' When rolling on the year or Calendar.YEAR field, it will roll the year
		''' value in the range between 1 and the value returned by calling
		''' <code>getMaximum(Calendar.YEAR)</code>.
		''' When rolling on the month or Calendar.MONTH field, other fields like
		''' date might conflict and, need to be changed. For instance,
		''' rolling the month on the date 01/31/96 will result in 02/29/96.
		''' When rolling on the hour-in-day or Calendar.HOUR_OF_DAY field, it will
		''' roll the hour value in the range between 0 and 23, which is zero-based.
		''' </summary>
		''' <param name="field"> the time field. </param>
		''' <param name="up"> indicates if the value of the specified time field is to be
		''' rolled up or rolled down. Use true if rolling up, false otherwise. </param>
		''' <seealso cref= Calendar#add(int,int) </seealso>
		''' <seealso cref= Calendar#set(int,int) </seealso>
		Public MustOverride Sub roll(ByVal field As Integer, ByVal up As Boolean)

		''' <summary>
		''' Adds the specified (signed) amount to the specified calendar field
		''' without changing larger fields.  A negative amount means to roll
		''' down.
		''' 
		''' <p>NOTE:  This default implementation on <code>Calendar</code> just repeatedly calls the
		''' version of <seealso cref="#roll(int,boolean) roll()"/> that rolls by one unit.  This may not
		''' always do the right thing.  For example, if the <code>DAY_OF_MONTH</code> field is 31,
		''' rolling through February will leave it set to 28.  The <code>GregorianCalendar</code>
		''' version of this function takes care of this problem.  Other subclasses
		''' should also provide overrides of this function that do the right thing.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <param name="amount"> the signed amount to add to the calendar <code>field</code>.
		''' @since 1.2 </param>
		''' <seealso cref= #roll(int,boolean) </seealso>
		''' <seealso cref= #add(int,int) </seealso>
		''' <seealso cref= #set(int,int) </seealso>
		Public Overridable Sub roll(ByVal field As Integer, ByVal amount As Integer)
			Do While amount > 0
				roll(field, True)
				amount -= 1
			Loop
			Do While amount < 0
				roll(field, False)
				amount += 1
			Loop
		End Sub

		''' <summary>
		''' Sets the time zone with the given time zone value.
		''' </summary>
		''' <param name="value"> the given time zone. </param>
		Public Overridable Property timeZone As TimeZone
			Set(ByVal value As TimeZone)
				zone = value
				sharedZone = False
		'         Recompute the fields from the time using the new zone.  This also
		'         * works if isTimeSet is false (after a call to set()).  In that case
		'         * the time will be computed from the fields using the new zone, then
		'         * the fields will get recomputed from that.  Consider the sequence of
		'         * calls: cal.setTimeZone(EST); cal.set(HOUR, 1); cal.setTimeZone(PST).
		'         * Is cal set to 1 o'clock EST or 1 o'clock PST?  Answer: PST.  More
		'         * generally, a call to setTimeZone() affects calls to set() BEFORE AND
		'         * AFTER it up to the next call to complete().
		'         
					areFieldsSet = False
					areAllFieldsSet = areFieldsSet
			End Set
			Get
				' If the TimeZone object is shared by other Calendar instances, then
				' create a clone.
				If sharedZone Then
					zone = CType(zone.clone(), TimeZone)
					sharedZone = False
				End If
				Return zone
			End Get
		End Property


		''' <summary>
		''' Returns the time zone (without cloning).
		''' </summary>
		Friend Overridable Property zone As TimeZone
			Get
				Return zone
			End Get
		End Property

		''' <summary>
		''' Sets the sharedZone flag to <code>shared</code>.
		''' </summary>
		Friend Overridable Property zoneShared As Boolean
			Set(ByVal [shared] As Boolean)
				sharedZone = [shared]
			End Set
		End Property

		''' <summary>
		''' Specifies whether or not date/time interpretation is to be lenient.  With
		''' lenient interpretation, a date such as "February 942, 1996" will be
		''' treated as being equivalent to the 941st day after February 1, 1996.
		''' With strict (non-lenient) interpretation, such dates will cause an exception to be
		''' thrown. The default is lenient.
		''' </summary>
		''' <param name="lenient"> <code>true</code> if the lenient mode is to be turned
		''' on; <code>false</code> if it is to be turned off. </param>
		''' <seealso cref= #isLenient() </seealso>
		''' <seealso cref= java.text.DateFormat#setLenient </seealso>
		Public Overridable Property lenient As Boolean
			Set(ByVal lenient As Boolean)
				Me.lenient = lenient
			End Set
			Get
				Return lenient
			End Get
		End Property


		''' <summary>
		''' Sets what the first day of the week is; e.g., <code>SUNDAY</code> in the U.S.,
		''' <code>MONDAY</code> in France.
		''' </summary>
		''' <param name="value"> the given first day of the week. </param>
		''' <seealso cref= #getFirstDayOfWeek() </seealso>
		''' <seealso cref= #getMinimalDaysInFirstWeek() </seealso>
		Public Overridable Property firstDayOfWeek As Integer
			Set(ByVal value As Integer)
				If firstDayOfWeek = value Then Return
				firstDayOfWeek = value
				invalidateWeekFields()
			End Set
			Get
				Return firstDayOfWeek
			End Get
		End Property


		''' <summary>
		''' Sets what the minimal days required in the first week of the year are;
		''' For example, if the first week is defined as one that contains the first
		''' day of the first month of a year, call this method with value 1. If it
		''' must be a full week, use value 7.
		''' </summary>
		''' <param name="value"> the given minimal days required in the first week
		''' of the year. </param>
		''' <seealso cref= #getMinimalDaysInFirstWeek() </seealso>
		Public Overridable Property minimalDaysInFirstWeek As Integer
			Set(ByVal value As Integer)
				If minimalDaysInFirstWeek = value Then Return
				minimalDaysInFirstWeek = value
				invalidateWeekFields()
			End Set
			Get
				Return minimalDaysInFirstWeek
			End Get
		End Property


		''' <summary>
		''' Returns whether this {@code Calendar} supports week dates.
		''' 
		''' <p>The default implementation of this method returns {@code false}.
		''' </summary>
		''' <returns> {@code true} if this {@code Calendar} supports week dates;
		'''         {@code false} otherwise. </returns>
		''' <seealso cref= #getWeekYear() </seealso>
		''' <seealso cref= #setWeekDate(int,int,int) </seealso>
		''' <seealso cref= #getWeeksInWeekYear()
		''' @since 1.7 </seealso>
		Public Overridable Property weekDateSupported As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns the week year represented by this {@code Calendar}. The
		''' week year is in sync with the week cycle. The {@linkplain
		''' #getFirstDayOfWeek() first day of the first week} is the first
		''' day of the week year.
		''' 
		''' <p>The default implementation of this method throws an
		''' <seealso cref="UnsupportedOperationException"/>.
		''' </summary>
		''' <returns> the week year of this {@code Calendar} </returns>
		''' <exception cref="UnsupportedOperationException">
		'''            if any week year numbering isn't supported
		'''            in this {@code Calendar}. </exception>
		''' <seealso cref= #isWeekDateSupported() </seealso>
		''' <seealso cref= #getFirstDayOfWeek() </seealso>
		''' <seealso cref= #getMinimalDaysInFirstWeek()
		''' @since 1.7 </seealso>
		Public Overridable Property weekYear As Integer
			Get
				Throw New UnsupportedOperationException
			End Get
		End Property

		''' <summary>
		''' Sets the date of this {@code Calendar} with the the given date
		''' specifiers - week year, week of year, and day of week.
		''' 
		''' <p>Unlike the {@code set} method, all of the calendar fields
		''' and {@code time} values are calculated upon return.
		''' 
		''' <p>If {@code weekOfYear} is out of the valid week-of-year range
		''' in {@code weekYear}, the {@code weekYear} and {@code
		''' weekOfYear} values are adjusted in lenient mode, or an {@code
		''' IllegalArgumentException} is thrown in non-lenient mode.
		''' 
		''' <p>The default implementation of this method throws an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="weekYear">   the week year </param>
		''' <param name="weekOfYear"> the week number based on {@code weekYear} </param>
		''' <param name="dayOfWeek">  the day of week value: one of the constants
		'''                   for the <seealso cref="#DAY_OF_WEEK"/> field: {@link
		'''                   #SUNDAY}, ..., <seealso cref="#SATURDAY"/>. </param>
		''' <exception cref="IllegalArgumentException">
		'''            if any of the given date specifiers is invalid
		'''            or any of the calendar fields are inconsistent
		'''            with the given date specifiers in non-lenient mode </exception>
		''' <exception cref="UnsupportedOperationException">
		'''            if any week year numbering isn't supported in this
		'''            {@code Calendar}. </exception>
		''' <seealso cref= #isWeekDateSupported() </seealso>
		''' <seealso cref= #getFirstDayOfWeek() </seealso>
		''' <seealso cref= #getMinimalDaysInFirstWeek()
		''' @since 1.7 </seealso>
		Public Overridable Sub setWeekDate(ByVal weekYear As Integer, ByVal weekOfYear As Integer, ByVal dayOfWeek As Integer)
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Returns the number of weeks in the week year represented by this
		''' {@code Calendar}.
		''' 
		''' <p>The default implementation of this method throws an
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <returns> the number of weeks in the week year. </returns>
		''' <exception cref="UnsupportedOperationException">
		'''            if any week year numbering isn't supported in this
		'''            {@code Calendar}. </exception>
		''' <seealso cref= #WEEK_OF_YEAR </seealso>
		''' <seealso cref= #isWeekDateSupported() </seealso>
		''' <seealso cref= #getWeekYear() </seealso>
		''' <seealso cref= #getActualMaximum(int)
		''' @since 1.7 </seealso>
		Public Overridable Property weeksInWeekYear As Integer
			Get
				Throw New UnsupportedOperationException
			End Get
		End Property

		''' <summary>
		''' Returns the minimum value for the given calendar field of this
		''' <code>Calendar</code> instance. The minimum value is defined as
		''' the smallest value returned by the <seealso cref="#get(int) get"/> method
		''' for any possible time value.  The minimum value depends on
		''' calendar system specific parameters of the instance.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public MustOverride Function getMinimum(ByVal field As Integer) As Integer

		''' <summary>
		''' Returns the maximum value for the given calendar field of this
		''' <code>Calendar</code> instance. The maximum value is defined as
		''' the largest value returned by the <seealso cref="#get(int) get"/> method
		''' for any possible time value. The maximum value depends on
		''' calendar system specific parameters of the instance.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public MustOverride Function getMaximum(ByVal field As Integer) As Integer

		''' <summary>
		''' Returns the highest minimum value for the given calendar field
		''' of this <code>Calendar</code> instance. The highest minimum
		''' value is defined as the largest value returned by {@link
		''' #getActualMinimum(int)} for any possible time value. The
		''' greatest minimum value depends on calendar system specific
		''' parameters of the instance.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the highest minimum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public MustOverride Function getGreatestMinimum(ByVal field As Integer) As Integer

		''' <summary>
		''' Returns the lowest maximum value for the given calendar field
		''' of this <code>Calendar</code> instance. The lowest maximum
		''' value is defined as the smallest value returned by {@link
		''' #getActualMaximum(int)} for any possible time value. The least
		''' maximum value depends on calendar system specific parameters of
		''' the instance. For example, a <code>Calendar</code> for the
		''' Gregorian calendar system returns 28 for the
		''' <code>DAY_OF_MONTH</code> field, because the 28th is the last
		''' day of the shortest month of this calendar, February in a
		''' common year.
		''' </summary>
		''' <param name="field"> the calendar field. </param>
		''' <returns> the lowest maximum value for the given calendar field. </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int) </seealso>
		Public MustOverride Function getLeastMaximum(ByVal field As Integer) As Integer

		''' <summary>
		''' Returns the minimum value that the specified calendar field
		''' could have, given the time value of this <code>Calendar</code>.
		''' 
		''' <p>The default implementation of this method uses an iterative
		''' algorithm to determine the actual minimum value for the
		''' calendar field. Subclasses should, if possible, override this
		''' with a more efficient implementation - in many cases, they can
		''' simply return <code>getMinimum()</code>.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the minimum of the given calendar field for the time
		''' value of this <code>Calendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMaximum(int)
		''' @since 1.2 </seealso>
		Public Overridable Function getActualMinimum(ByVal field As Integer) As Integer
			Dim fieldValue As Integer = getGreatestMinimum(field)
			Dim endValue As Integer = getMinimum(field)

			' if we know that the minimum value is always the same, just return it
			If fieldValue = endValue Then Return fieldValue

			' clone the calendar so we don't mess with the real one, and set it to
			' accept anything for the field values
			Dim work As Calendar = CType(Me.clone(), Calendar)
			work.lenient = True

			' now try each value from getLeastMaximum() to getMaximum() one by one until
			' we get a value that normalizes to another value.  The last value that
			' normalizes to itself is the actual minimum for the current date
			Dim result As Integer = fieldValue

			Do
				work.set(field, fieldValue)
				If work.get(field) <> fieldValue Then
					Exit Do
				Else
					result = fieldValue
					fieldValue -= 1
				End If
			Loop While fieldValue >= endValue

			Return result
		End Function

		''' <summary>
		''' Returns the maximum value that the specified calendar field
		''' could have, given the time value of this
		''' <code>Calendar</code>. For example, the actual maximum value of
		''' the <code>MONTH</code> field is 12 in some years, and 13 in
		''' other years in the Hebrew calendar system.
		''' 
		''' <p>The default implementation of this method uses an iterative
		''' algorithm to determine the actual maximum value for the
		''' calendar field. Subclasses should, if possible, override this
		''' with a more efficient implementation.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the maximum of the given calendar field for the time
		''' value of this <code>Calendar</code> </returns>
		''' <seealso cref= #getMinimum(int) </seealso>
		''' <seealso cref= #getMaximum(int) </seealso>
		''' <seealso cref= #getGreatestMinimum(int) </seealso>
		''' <seealso cref= #getLeastMaximum(int) </seealso>
		''' <seealso cref= #getActualMinimum(int)
		''' @since 1.2 </seealso>
		Public Overridable Function getActualMaximum(ByVal field As Integer) As Integer
			Dim fieldValue As Integer = getLeastMaximum(field)
			Dim endValue As Integer = getMaximum(field)

			' if we know that the maximum value is always the same, just return it.
			If fieldValue = endValue Then Return fieldValue

			' clone the calendar so we don't mess with the real one, and set it to
			' accept anything for the field values.
			Dim work As Calendar = CType(Me.clone(), Calendar)
			work.lenient = True

			' if we're counting weeks, set the day of the week to Sunday.  We know the
			' last week of a month or year will contain the first day of the week.
			If field = WEEK_OF_YEAR OrElse field = WEEK_OF_MONTH Then work.set(DAY_OF_WEEK, firstDayOfWeek)

			' now try each value from getLeastMaximum() to getMaximum() one by one until
			' we get a value that normalizes to another value.  The last value that
			' normalizes to itself is the actual maximum for the current date
			Dim result As Integer = fieldValue

			Do
				work.set(field, fieldValue)
				If work.get(field) <> fieldValue Then
					Exit Do
				Else
					result = fieldValue
					fieldValue += 1
				End If
			Loop While fieldValue <= endValue

			Return result
		End Function

		''' <summary>
		''' Creates and returns a copy of this object.
		''' </summary>
		''' <returns> a copy of this object. </returns>
		Public Overrides Function clone() As Object
			Try
				Dim other As Calendar = CType(MyBase.clone(), Calendar)

				other.fields = New Integer(FIELD_COUNT - 1){}
				other.isSet_Renamed = New Boolean(FIELD_COUNT - 1){}
				other.stamp = New Integer(FIELD_COUNT - 1){}
				For i As Integer = 0 To FIELD_COUNT - 1
					other.fields(i) = fields(i)
					other.stamp(i) = stamp(i)
					other.isSet_Renamed(i) = isSet_Renamed(i)
				Next i
				other.zone = CType(zone.clone(), TimeZone)
				Return other
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function

		Private Shared ReadOnly FIELD_NAME As String() = { "ERA", "YEAR", "MONTH", "WEEK_OF_YEAR", "WEEK_OF_MONTH", "DAY_OF_MONTH", "DAY_OF_YEAR", "DAY_OF_WEEK", "DAY_OF_WEEK_IN_MONTH", "AM_PM", "HOUR", "HOUR_OF_DAY", "MINUTE", "SECOND", "MILLISECOND", "ZONE_OFFSET", "DST_OFFSET" }

		''' <summary>
		''' Returns the name of the specified calendar field.
		''' </summary>
		''' <param name="field"> the calendar field </param>
		''' <returns> the calendar field name </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>field</code> is negative,
		''' equal to or greater then <code>FIELD_COUNT</code>. </exception>
		Friend Shared Function getFieldName(ByVal field As Integer) As String
			Return FIELD_NAME(field)
		End Function

		''' <summary>
		''' Return a string representation of this calendar. This method
		''' is intended to be used only for debugging purposes, and the
		''' format of the returned string may vary between implementations.
		''' The returned string may be empty but may not be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this calendar. </returns>
		Public Overrides Function ToString() As String
			' NOTE: BuddhistCalendar.toString() interprets the string
			' produced by this method so that the Gregorian year number
			' is substituted by its B.E. year value. It relies on
			' "...,YEAR=<year>,..." or "...,YEAR=?,...".
			Dim buffer As New StringBuilder(800)
			buffer.append(Me.GetType().name).append("["c)
			appendValue(buffer, "time", isTimeSet, time)
			buffer.append(",areFieldsSet=").append(areFieldsSet)
			buffer.append(",areAllFieldsSet=").append(areAllFieldsSet)
			buffer.append(",lenient=").append(lenient)
			buffer.append(",zone=").append(zone)
			appendValue(buffer, ",firstDayOfWeek", True, CLng(firstDayOfWeek))
			appendValue(buffer, ",minimalDaysInFirstWeek", True, CLng(minimalDaysInFirstWeek))
			For i As Integer = 0 To FIELD_COUNT - 1
				buffer.append(","c)
				appendValue(buffer, FIELD_NAME(i), isSet(i), CLng(fields(i)))
			Next i
			buffer.append("]"c)
			Return buffer.ToString()
		End Function

		' =======================privates===============================

		Private Shared Sub appendValue(ByVal sb As StringBuilder, ByVal item As String, ByVal valid As Boolean, ByVal value As Long)
			sb.append(item).append("="c)
			If valid Then
				sb.append(value)
			Else
				sb.append("?"c)
			End If
		End Sub

		''' <summary>
		''' Both firstDayOfWeek and minimalDaysInFirstWeek are locale-dependent.
		''' They are used to figure out the week count for a specific date for
		''' a given locale. These must be set when a Calendar is constructed. </summary>
		''' <param name="desiredLocale"> the given locale. </param>
		Private Property weekCountData As Locale
			Set(ByVal desiredLocale As Locale)
				' try to get the Locale data from the cache 
				Dim data As Integer() = cachedLocaleData.get(desiredLocale)
				If data Is Nothing Then ' cache miss
					data = New Integer(1){}
					data(0) = sun.util.locale.provider.CalendarDataUtility.retrieveFirstDayOfWeek(desiredLocale)
					data(1) = sun.util.locale.provider.CalendarDataUtility.retrieveMinimalDaysInFirstWeek(desiredLocale)
					cachedLocaleData.putIfAbsent(desiredLocale, data)
				End If
				firstDayOfWeek = data(0)
				minimalDaysInFirstWeek = data(1)
			End Set
		End Property

		''' <summary>
		''' Recomputes the time and updates the status fields isTimeSet
		''' and areFieldsSet.  Callers should check isTimeSet and only
		''' call this method if isTimeSet is false.
		''' </summary>
		Private Sub updateTime()
			computeTime()
			' The areFieldsSet and areAllFieldsSet values are no longer
			' controlled here (as of 1.5).
			isTimeSet = True
		End Sub

		Private Function compareTo(ByVal t As Long) As Integer
			Dim thisTime As Long = getMillisOf(Me)
			Return If(thisTime > t, 1, If(thisTime = t, 0, -1))
		End Function

		Private Shared Function getMillisOf(ByVal calendar_Renamed As Calendar) As Long
			If calendar_Renamed.isTimeSet Then Return calendar_Renamed.time
			Dim cal As Calendar = CType(calendar_Renamed.clone(), Calendar)
			cal.lenient = True
			Return cal.timeInMillis
		End Function

		''' <summary>
		''' Adjusts the stamp[] values before nextStamp overflow. nextStamp
		''' is set to the next stamp value upon the return.
		''' </summary>
		Private Sub adjustStamp()
			Dim max As Integer = MINIMUM_USER_STAMP
			Dim newStamp As Integer = MINIMUM_USER_STAMP

			Do
				Dim min As Integer =  java.lang.[Integer].Max_Value
				For i As Integer = 0 To stamp.Length - 1
					Dim v As Integer = stamp(i)
					If v >= newStamp AndAlso min > v Then min = v
					If max < v Then max = v
				Next i
				If max <> min AndAlso min =  java.lang.[Integer].Max_Value Then Exit Do
				For i As Integer = 0 To stamp.Length - 1
					If stamp(i) = min Then stamp(i) = newStamp
				Next i
				newStamp += 1
				If min = max Then Exit Do
			Loop
			nextStamp = newStamp
		End Sub

		''' <summary>
		''' Sets the WEEK_OF_MONTH and WEEK_OF_YEAR fields to new values with the
		''' new parameter value if they have been calculated internally.
		''' </summary>
		Private Sub invalidateWeekFields()
			If stamp(WEEK_OF_MONTH) <> COMPUTED AndAlso stamp(WEEK_OF_YEAR) <> COMPUTED Then Return

			' We have to check the new values of these fields after changing
			' firstDayOfWeek and/or minimalDaysInFirstWeek. If the field values
			' have been changed, then set the new values. (4822110)
			Dim cal As Calendar = CType(clone(), Calendar)
			cal.lenient = True
			cal.clear(WEEK_OF_MONTH)
			cal.clear(WEEK_OF_YEAR)

			If stamp(WEEK_OF_MONTH) = COMPUTED Then
				Dim weekOfMonth As Integer = cal.get(WEEK_OF_MONTH)
				If fields(WEEK_OF_MONTH) <> weekOfMonth Then fields(WEEK_OF_MONTH) = weekOfMonth
			End If

			If stamp(WEEK_OF_YEAR) = COMPUTED Then
				Dim weekOfYear As Integer = cal.get(WEEK_OF_YEAR)
				If fields(WEEK_OF_YEAR) <> weekOfYear Then fields(WEEK_OF_YEAR) = weekOfYear
			End If
		End Sub

		''' <summary>
		''' Save the state of this object to a stream (i.e., serialize it).
		''' 
		''' Ideally, <code>Calendar</code> would only write out its state data and
		''' the current time, and not write any field data out, such as
		''' <code>fields[]</code>, <code>isTimeSet</code>, <code>areFieldsSet</code>,
		''' and <code>isSet[]</code>.  <code>nextStamp</code> also should not be part
		''' of the persistent state. Unfortunately, this didn't happen before JDK 1.1
		''' shipped. To be compatible with JDK 1.1, we will always have to write out
		''' the field values and state flags.  However, <code>nextStamp</code> can be
		''' removed from the serialization stream; this will probably happen in the
		''' near future.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			' Try to compute the time correctly, for the future (stream
			' version 2) in which we don't write out fields[] or isSet[].
			If Not isTimeSet Then
				Try
					updateTime()
				Catch e As IllegalArgumentException
				End Try
			End If

			' If this Calendar has a ZoneInfo, save it and set a
			' SimpleTimeZone equivalent (as a single DST schedule) for
			' backward compatibility.
			Dim savedZone As TimeZone = Nothing
			If TypeOf zone Is sun.util.calendar.ZoneInfo Then
				Dim stz As SimpleTimeZone = CType(zone, sun.util.calendar.ZoneInfo).lastRuleInstance
				If stz Is Nothing Then stz = New SimpleTimeZone(zone.rawOffset, zone.iD)
				savedZone = zone
				zone = stz
			End If

			' Write out the 1.1 FCS object.
			stream.defaultWriteObject()

			' Write out the ZoneInfo object
			' 4802409: we write out even if it is null, a temporary workaround
			' the real fix for bug 4844924 in corba-iiop
			stream.writeObject(savedZone)
			If savedZone IsNot Nothing Then zone = savedZone
		End Sub

		Private Class CalendarAccessControlContext
			Private Shared ReadOnly INSTANCE As java.security.AccessControlContext
			Shared Sub New()
				Dim perm As New RuntimePermission("accessClassInPackage.sun.util.calendar")
				Dim perms As java.security.PermissionCollection = perm.newPermissionCollection()
				perms.add(perm)
				INSTANCE = New java.security.AccessControlContext(New java.security.ProtectionDomain() { New java.security.ProtectionDomain(Nothing, perms)
													})
			End Sub
			Private Sub New()
			End Sub
		End Class

		''' <summary>
		''' Reconstitutes this object from a stream (i.e., deserialize it).
		''' </summary>
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			Dim input As java.io.ObjectInputStream = stream
			input.defaultReadObject()

			stamp = New Integer(FIELD_COUNT - 1){}

			' Starting with version 2 (not implemented yet), we expect that
			' fields[], isSet[], isTimeSet, and areFieldsSet may not be
			' streamed out anymore.  We expect 'time' to be correct.
			If serialVersionOnStream >= 2 Then
				isTimeSet = True
				If fields Is Nothing Then fields = New Integer(FIELD_COUNT - 1){}
				If isSet_Renamed Is Nothing Then isSet_Renamed = New Boolean(FIELD_COUNT - 1){}
			ElseIf serialVersionOnStream >= 0 Then
				For i As Integer = 0 To FIELD_COUNT - 1
					stamp(i) = If(isSet_Renamed(i), COMPUTED, UNSET)
				Next i
			End If

			serialVersionOnStream = currentSerialVersion

			' If there's a ZoneInfo object, use it for zone.
			Dim zi As sun.util.calendar.ZoneInfo = Nothing
			Try
				zi = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
						CalendarAccessControlContext.INSTANCE)
			Catch pae As java.security.PrivilegedActionException
				Dim e As Exception = pae.exception
				If Not(TypeOf e Is java.io.OptionalDataException) Then
					If TypeOf e Is RuntimeException Then
						Throw CType(e, RuntimeException)
					ElseIf TypeOf e Is java.io.IOException Then
						Throw CType(e, java.io.IOException)
					ElseIf TypeOf e Is ClassNotFoundException Then
						Throw CType(e, ClassNotFoundException)
					End If
					Throw New RuntimeException(e)
				End If
			End Try
			If zi IsNot Nothing Then zone = zi

			' If the deserialized object has a SimpleTimeZone, try to
			' replace it with a ZoneInfo equivalent (as of 1.4) in order
			' to be compatible with the SimpleTimeZone-based
			' implementation as much as possible.
			If TypeOf zone Is SimpleTimeZone Then
				Dim id As String = zone.iD
				Dim tz As TimeZone = TimeZone.getTimeZone(id)
				If tz IsNot Nothing AndAlso tz.hasSameRules(zone) AndAlso tz.iD.Equals(id) Then zone = tz
			End If
		End Sub

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overrides Function run() As sun.util.calendar.ZoneInfo
				Return CType(input.readObject(), sun.util.calendar.ZoneInfo)
			End Function
		End Class

		''' <summary>
		''' Converts this object to an <seealso cref="Instant"/>.
		''' <p>
		''' The conversion creates an {@code Instant} that represents the
		''' same point on the time-line as this {@code Calendar}.
		''' </summary>
		''' <returns> the instant representing the same point on the time-line
		''' @since 1.8 </returns>
		Public Function toInstant() As java.time.Instant
			Return java.time.Instant.ofEpochMilli(timeInMillis)
		End Function
	End Class

End Namespace