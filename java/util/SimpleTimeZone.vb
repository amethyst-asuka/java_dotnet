Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - All Rights Reserved
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
	''' <code>SimpleTimeZone</code> is a concrete subclass of <code>TimeZone</code>
	''' that represents a time zone for use with a Gregorian calendar.
	''' The class holds an offset from GMT, called <em>raw offset</em>, and start
	''' and end rules for a daylight saving time schedule.  Since it only holds
	''' single values for each, it cannot handle historical changes in the offset
	''' from GMT and the daylight saving schedule, except that the {@link
	''' #setStartYear setStartYear} method can specify the year when the daylight
	''' saving time schedule starts in effect.
	''' <p>
	''' To construct a <code>SimpleTimeZone</code> with a daylight saving time
	''' schedule, the schedule can be described with a set of rules,
	''' <em>start-rule</em> and <em>end-rule</em>. A day when daylight saving time
	''' starts or ends is specified by a combination of <em>month</em>,
	''' <em>day-of-month</em>, and <em>day-of-week</em> values. The <em>month</em>
	''' value is represented by a Calendar <seealso cref="Calendar#MONTH MONTH"/> field
	''' value, such as <seealso cref="Calendar#MARCH"/>. The <em>day-of-week</em> value is
	''' represented by a Calendar <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> value,
	''' such as <seealso cref="Calendar#SUNDAY SUNDAY"/>. The meanings of value combinations
	''' are as follows.
	''' 
	''' <ul>
	''' <li><b>Exact day of month</b><br>
	''' To specify an exact day of month, set the <em>month</em> and
	''' <em>day-of-month</em> to an exact value, and <em>day-of-week</em> to zero. For
	''' example, to specify March 1, set the <em>month</em> to {@link Calendar#MARCH
	''' MARCH}, <em>day-of-month</em> to 1, and <em>day-of-week</em> to 0.</li>
	''' 
	''' <li><b>Day of week on or after day of month</b><br>
	''' To specify a day of week on or after an exact day of month, set the
	''' <em>month</em> to an exact month value, <em>day-of-month</em> to the day on
	''' or after which the rule is applied, and <em>day-of-week</em> to a negative {@link
	''' Calendar#DAY_OF_WEEK DAY_OF_WEEK} field value. For example, to specify the
	''' second Sunday of April, set <em>month</em> to <seealso cref="Calendar#APRIL APRIL"/>,
	''' <em>day-of-month</em> to 8, and <em>day-of-week</em> to <code>-</code>{@link
	''' Calendar#SUNDAY SUNDAY}.</li>
	''' 
	''' <li><b>Day of week on or before day of month</b><br>
	''' To specify a day of the week on or before an exact day of the month, set
	''' <em>day-of-month</em> and <em>day-of-week</em> to a negative value. For
	''' example, to specify the last Wednesday on or before the 21st of March, set
	''' <em>month</em> to <seealso cref="Calendar#MARCH MARCH"/>, <em>day-of-month</em> is -21
	''' and <em>day-of-week</em> is <code>-</code><seealso cref="Calendar#WEDNESDAY WEDNESDAY"/>. </li>
	''' 
	''' <li><b>Last day-of-week of month</b><br>
	''' To specify, the last day-of-week of the month, set <em>day-of-week</em> to a
	''' <seealso cref="Calendar#DAY_OF_WEEK DAY_OF_WEEK"/> value and <em>day-of-month</em> to
	''' -1. For example, to specify the last Sunday of October, set <em>month</em>
	''' to <seealso cref="Calendar#OCTOBER OCTOBER"/>, <em>day-of-week</em> to {@link
	''' Calendar#SUNDAY SUNDAY} and <em>day-of-month</em> to -1.  </li>
	''' 
	''' </ul>
	''' The time of the day at which daylight saving time starts or ends is
	''' specified by a millisecond value within the day. There are three kinds of
	''' <em>mode</em>s to specify the time: <seealso cref="#WALL_TIME"/>, {@link
	''' #STANDARD_TIME} and <seealso cref="#UTC_TIME"/>. For example, if daylight
	''' saving time ends
	''' at 2:00 am in the wall clock time, it can be specified by 7200000
	''' milliseconds in the <seealso cref="#WALL_TIME"/> mode. In this case, the wall clock time
	''' for an <em>end-rule</em> means the same thing as the daylight time.
	''' <p>
	''' The following are examples of parameters for constructing time zone objects.
	''' <pre><code>
	'''      // Base GMT offset: -8:00
	'''      // DST starts:      at 2:00am in standard time
	'''      //                  on the first Sunday in April
	'''      // DST ends:        at 2:00am in daylight time
	'''      //                  on the last Sunday in October
	'''      // Save:            1 hour
	'''      SimpleTimeZone(-28800000,
	'''                     "America/Los_Angeles",
	'''                     Calendar.APRIL, 1, -Calendar.SUNDAY,
	'''                     7200000,
	'''                     Calendar.OCTOBER, -1, Calendar.SUNDAY,
	'''                     7200000,
	'''                     3600000)
	''' 
	'''      // Base GMT offset: +1:00
	'''      // DST starts:      at 1:00am in UTC time
	'''      //                  on the last Sunday in March
	'''      // DST ends:        at 1:00am in UTC time
	'''      //                  on the last Sunday in October
	'''      // Save:            1 hour
	'''      SimpleTimeZone(3600000,
	'''                     "Europe/Paris",
	'''                     Calendar.MARCH, -1, Calendar.SUNDAY,
	'''                     3600000, SimpleTimeZone.UTC_TIME,
	'''                     Calendar.OCTOBER, -1, Calendar.SUNDAY,
	'''                     3600000, SimpleTimeZone.UTC_TIME,
	'''                     3600000)
	''' </code></pre>
	''' These parameter rules are also applicable to the set rule methods, such as
	''' <code>setStartRule</code>.
	''' 
	''' @since 1.1 </summary>
	''' <seealso cref=      Calendar </seealso>
	''' <seealso cref=      GregorianCalendar </seealso>
	''' <seealso cref=      TimeZone
	''' @author   David Goldsmith, Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>

	Public Class SimpleTimeZone
		Inherits TimeZone

		''' <summary>
		''' Constructs a SimpleTimeZone with the given base time zone offset from GMT
		''' and time zone ID with no daylight saving time schedule.
		''' </summary>
		''' <param name="rawOffset">  The base time zone offset in milliseconds to GMT. </param>
		''' <param name="ID">         The time zone name that is given to this instance. </param>
		Public Sub New(ByVal rawOffset As Integer, ByVal ID As String)
			Me.rawOffset = rawOffset
			iD = ID
			dstSavings = millisPerHour ' In case user sets rules later
		End Sub

		''' <summary>
		''' Constructs a SimpleTimeZone with the given base time zone offset from
		''' GMT, time zone ID, and rules for starting and ending the daylight
		''' time.
		''' Both <code>startTime</code> and <code>endTime</code> are specified to be
		''' represented in the wall clock time. The amount of daylight saving is
		''' assumed to be 3600000 milliseconds (i.e., one hour). This constructor is
		''' equivalent to:
		''' <pre><code>
		'''     SimpleTimeZone(rawOffset,
		'''                    ID,
		'''                    startMonth,
		'''                    startDay,
		'''                    startDayOfWeek,
		'''                    startTime,
		'''                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		'''                    endMonth,
		'''                    endDay,
		'''                    endDayOfWeek,
		'''                    endTime,
		'''                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		'''                    3600000)
		''' </code></pre>
		''' </summary>
		''' <param name="rawOffset">       The given base time zone offset from GMT. </param>
		''' <param name="ID">              The time zone ID which is given to this object. </param>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field value (0-based. e.g., 0
		'''                        for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startTime">       The daylight saving time starting time in local wall clock
		'''                        time (in milliseconds within the day), which is local
		'''                        standard time in this case. </param>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endTime">         The daylight saving ending time in local wall clock time,
		'''                        (in milliseconds within the day) which is local daylight
		'''                        time in this case. </param>
		''' <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, or time
		''' parameters are out of range for the start or end rule </exception>
		Public Sub New(ByVal rawOffset As Integer, ByVal ID As String, ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startDayOfWeek As Integer, ByVal startTime As Integer, ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endDayOfWeek As Integer, ByVal endTime As Integer)
			Me.New(rawOffset, ID, startMonth, startDay, startDayOfWeek, startTime, WALL_TIME, endMonth, endDay, endDayOfWeek, endTime, WALL_TIME, millisPerHour)
		End Sub

		''' <summary>
		''' Constructs a SimpleTimeZone with the given base time zone offset from
		''' GMT, time zone ID, and rules for starting and ending the daylight
		''' time.
		''' Both <code>startTime</code> and <code>endTime</code> are assumed to be
		''' represented in the wall clock time. This constructor is equivalent to:
		''' <pre><code>
		'''     SimpleTimeZone(rawOffset,
		'''                    ID,
		'''                    startMonth,
		'''                    startDay,
		'''                    startDayOfWeek,
		'''                    startTime,
		'''                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		'''                    endMonth,
		'''                    endDay,
		'''                    endDayOfWeek,
		'''                    endTime,
		'''                    SimpleTimeZone.<seealso cref="#WALL_TIME"/>,
		'''                    dstSavings)
		''' </code></pre>
		''' </summary>
		''' <param name="rawOffset">       The given base time zone offset from GMT. </param>
		''' <param name="ID">              The time zone ID which is given to this object. </param>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 0 for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startTime">       The daylight saving time starting time in local wall clock
		'''                        time, which is local standard time in this case. </param>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endTime">         The daylight saving ending time in local wall clock time,
		'''                        which is local daylight time in this case. </param>
		''' <param name="dstSavings">      The amount of time in milliseconds saved during
		'''                        daylight saving time. </param>
		''' <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, or time
		''' parameters are out of range for the start or end rule
		''' @since 1.2 </exception>
		Public Sub New(ByVal rawOffset As Integer, ByVal ID As String, ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startDayOfWeek As Integer, ByVal startTime As Integer, ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endDayOfWeek As Integer, ByVal endTime As Integer, ByVal dstSavings As Integer)
			Me.New(rawOffset, ID, startMonth, startDay, startDayOfWeek, startTime, WALL_TIME, endMonth, endDay, endDayOfWeek, endTime, WALL_TIME, dstSavings)
		End Sub

		''' <summary>
		''' Constructs a SimpleTimeZone with the given base time zone offset from
		''' GMT, time zone ID, and rules for starting and ending the daylight
		''' time.
		''' This constructor takes the full set of the start and end rules
		''' parameters, including modes of <code>startTime</code> and
		''' <code>endTime</code>. The mode specifies either {@link #WALL_TIME wall
		''' time} or <seealso cref="#STANDARD_TIME standard time"/> or {@link #UTC_TIME UTC
		''' time}.
		''' </summary>
		''' <param name="rawOffset">       The given base time zone offset from GMT. </param>
		''' <param name="ID">              The time zone ID which is given to this object. </param>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 0 for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startTime">       The daylight saving time starting time in the time mode
		'''                        specified by <code>startTimeMode</code>. </param>
		''' <param name="startTimeMode">   The mode of the start time specified by startTime. </param>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endTime">         The daylight saving ending time in time time mode
		'''                        specified by <code>endTimeMode</code>. </param>
		''' <param name="endTimeMode">     The mode of the end time specified by endTime </param>
		''' <param name="dstSavings">      The amount of time in milliseconds saved during
		'''                        daylight saving time.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the month, day, dayOfWeek, time more, or
		''' time parameters are out of range for the start or end rule, or if a time mode
		''' value is invalid.
		''' </exception>
		''' <seealso cref= #WALL_TIME </seealso>
		''' <seealso cref= #STANDARD_TIME </seealso>
		''' <seealso cref= #UTC_TIME
		''' 
		''' @since 1.4 </seealso>
		Public Sub New(ByVal rawOffset As Integer, ByVal ID As String, ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startDayOfWeek As Integer, ByVal startTime As Integer, ByVal startTimeMode As Integer, ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endDayOfWeek As Integer, ByVal endTime As Integer, ByVal endTimeMode As Integer, ByVal dstSavings As Integer)

			iD = ID
			Me.rawOffset = rawOffset
			Me.startMonth = startMonth
			Me.startDay = startDay
			Me.startDayOfWeek = startDayOfWeek
			Me.startTime = startTime
			Me.startTimeMode = startTimeMode
			Me.endMonth = endMonth
			Me.endDay = endDay
			Me.endDayOfWeek = endDayOfWeek
			Me.endTime = endTime
			Me.endTimeMode = endTimeMode
			Me.dstSavings = dstSavings

			' this.useDaylight is set by decodeRules
			decodeRules()
			If dstSavings <= 0 Then Throw New IllegalArgumentException("Illegal daylight saving value: " & dstSavings)
		End Sub

		''' <summary>
		''' Sets the daylight saving time starting year.
		''' </summary>
		''' <param name="year">  The daylight saving starting year. </param>
		Public Overridable Property startYear As Integer
			Set(ByVal year As Integer)
				startYear = year
				invalidateCache()
			End Set
		End Property

		''' <summary>
		''' Sets the daylight saving time start rule. For example, if daylight saving
		''' time starts on the first Sunday in April at 2 am in local wall clock
		''' time, you can set the start rule by calling:
		''' <pre><code>setStartRule(Calendar.APRIL, 1, Calendar.SUNDAY, 2*60*60*1000);</code></pre>
		''' </summary>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 0 for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startDayOfWeek">  The daylight saving time starting day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="startTime">       The daylight saving time starting time in local wall clock
		'''                        time, which is local standard time in this case. </param>
		''' <exception cref="IllegalArgumentException"> if the <code>startMonth</code>, <code>startDay</code>,
		''' <code>startDayOfWeek</code>, or <code>startTime</code> parameters are out of range </exception>
		Public Overridable Sub setStartRule(ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startDayOfWeek As Integer, ByVal startTime As Integer)
			Me.startMonth = startMonth
			Me.startDay = startDay
			Me.startDayOfWeek = startDayOfWeek
			Me.startTime = startTime
			startTimeMode = WALL_TIME
			decodeStartRule()
			invalidateCache()
		End Sub

		''' <summary>
		''' Sets the daylight saving time start rule to a fixed date within a month.
		''' This method is equivalent to:
		''' <pre><code>setStartRule(startMonth, startDay, 0, startTime)</code></pre>
		''' </summary>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 0 for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts. </param>
		''' <param name="startTime">       The daylight saving time starting time in local wall clock
		'''                        time, which is local standard time in this case.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <exception cref="IllegalArgumentException"> if the <code>startMonth</code>,
		''' <code>startDayOfMonth</code>, or <code>startTime</code> parameters are out of range
		''' @since 1.2 </exception>
		Public Overridable Sub setStartRule(ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startTime As Integer)
			startRuleule(startMonth, startDay, 0, startTime)
		End Sub

		''' <summary>
		''' Sets the daylight saving time start rule to a weekday before or after the given date within
		''' a month, e.g., the first Monday on or after the 8th.
		''' </summary>
		''' <param name="startMonth">      The daylight saving time starting month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 0 for January). </param>
		''' <param name="startDay">        The day of the month on which the daylight saving time starts. </param>
		''' <param name="startDayOfWeek">  The daylight saving time starting day-of-week. </param>
		''' <param name="startTime">       The daylight saving time starting time in local wall clock
		'''                        time, which is local standard time in this case. </param>
		''' <param name="after">           If true, this rule selects the first <code>dayOfWeek</code> on or
		'''                        <em>after</em> <code>dayOfMonth</code>.  If false, this rule
		'''                        selects the last <code>dayOfWeek</code> on or <em>before</em>
		'''                        <code>dayOfMonth</code>. </param>
		''' <exception cref="IllegalArgumentException"> if the <code>startMonth</code>, <code>startDay</code>,
		''' <code>startDayOfWeek</code>, or <code>startTime</code> parameters are out of range
		''' @since 1.2 </exception>
		Public Overridable Sub setStartRule(ByVal startMonth As Integer, ByVal startDay As Integer, ByVal startDayOfWeek As Integer, ByVal startTime As Integer, ByVal after As Boolean)
			' TODO: this method doesn't check the initial values of dayOfMonth or dayOfWeek.
			If after Then
				startRuleule(startMonth, startDay, -startDayOfWeek, startTime)
			Else
				startRuleule(startMonth, -startDay, -startDayOfWeek, startTime)
			End If
		End Sub

		''' <summary>
		''' Sets the daylight saving time end rule. For example, if daylight saving time
		''' ends on the last Sunday in October at 2 am in wall clock time,
		''' you can set the end rule by calling:
		''' <code>setEndRule(Calendar.OCTOBER, -1, Calendar.SUNDAY, 2*60*60*1000);</code>
		''' </summary>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endDayOfWeek">    The daylight saving time ending day-of-week.
		'''                        See the class description for the special cases of this parameter. </param>
		''' <param name="endTime">         The daylight saving ending time in local wall clock time,
		'''                        (in milliseconds within the day) which is local daylight
		'''                        time in this case. </param>
		''' <exception cref="IllegalArgumentException"> if the <code>endMonth</code>, <code>endDay</code>,
		''' <code>endDayOfWeek</code>, or <code>endTime</code> parameters are out of range </exception>
		Public Overridable Sub setEndRule(ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endDayOfWeek As Integer, ByVal endTime As Integer)
			Me.endMonth = endMonth
			Me.endDay = endDay
			Me.endDayOfWeek = endDayOfWeek
			Me.endTime = endTime
			Me.endTimeMode = WALL_TIME
			decodeEndRule()
			invalidateCache()
		End Sub

		''' <summary>
		''' Sets the daylight saving time end rule to a fixed date within a month.
		''' This method is equivalent to:
		''' <pre><code>setEndRule(endMonth, endDay, 0, endTime)</code></pre>
		''' </summary>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends. </param>
		''' <param name="endTime">         The daylight saving ending time in local wall clock time,
		'''                        (in milliseconds within the day) which is local daylight
		'''                        time in this case. </param>
		''' <exception cref="IllegalArgumentException"> the <code>endMonth</code>, <code>endDay</code>,
		''' or <code>endTime</code> parameters are out of range
		''' @since 1.2 </exception>
		Public Overridable Sub setEndRule(ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endTime As Integer)
			endRuleule(endMonth, endDay, 0, endTime)
		End Sub

		''' <summary>
		''' Sets the daylight saving time end rule to a weekday before or after the given date within
		''' a month, e.g., the first Monday on or after the 8th.
		''' </summary>
		''' <param name="endMonth">        The daylight saving time ending month. Month is
		'''                        a <seealso cref="Calendar#MONTH MONTH"/> field
		'''                        value (0-based. e.g., 9 for October). </param>
		''' <param name="endDay">          The day of the month on which the daylight saving time ends. </param>
		''' <param name="endDayOfWeek">    The daylight saving time ending day-of-week. </param>
		''' <param name="endTime">         The daylight saving ending time in local wall clock time,
		'''                        (in milliseconds within the day) which is local daylight
		'''                        time in this case. </param>
		''' <param name="after">           If true, this rule selects the first <code>endDayOfWeek</code> on
		'''                        or <em>after</em> <code>endDay</code>.  If false, this rule
		'''                        selects the last <code>endDayOfWeek</code> on or before
		'''                        <code>endDay</code> of the month. </param>
		''' <exception cref="IllegalArgumentException"> the <code>endMonth</code>, <code>endDay</code>,
		''' <code>endDayOfWeek</code>, or <code>endTime</code> parameters are out of range
		''' @since 1.2 </exception>
		Public Overridable Sub setEndRule(ByVal endMonth As Integer, ByVal endDay As Integer, ByVal endDayOfWeek As Integer, ByVal endTime As Integer, ByVal after As Boolean)
			If after Then
				endRuleule(endMonth, endDay, -endDayOfWeek, endTime)
			Else
				endRuleule(endMonth, -endDay, -endDayOfWeek, endTime)
			End If
		End Sub

		''' <summary>
		''' Returns the offset of this time zone from UTC at the given
		''' time. If daylight saving time is in effect at the given time,
		''' the offset value is adjusted with the amount of daylight
		''' saving.
		''' </summary>
		''' <param name="date"> the time at which the time zone offset is found </param>
		''' <returns> the amount of time in milliseconds to add to UTC to get
		''' local time.
		''' @since 1.4 </returns>
		Public Overrides Function getOffset(ByVal [date] As Long) As Integer
			Return getOffsets(date_Renamed, Nothing)
		End Function

		''' <seealso cref= TimeZone#getOffsets </seealso>
		Friend Overrides Function getOffsets(ByVal [date] As Long, ByVal offsets As Integer()) As Integer
			Dim offset_Renamed As Integer = rawOffset

		  computeOffset:
			If useDaylight Then
				SyncLock Me
					If cacheStart <> 0 Then
						If date_Renamed >= cacheStart AndAlso date_Renamed < cacheEnd Then
							offset_Renamed += dstSavings
							GoTo computeOffset
						End If
					End If
				End SyncLock
				Dim cal As sun.util.calendar.BaseCalendar = If(date_Renamed >= GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER, gcal, CType(sun.util.calendar.CalendarSystem.forName("julian"), sun.util.calendar.BaseCalendar))
				Dim [cdate] As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
				' Get the year in local time
				cal.getCalendarDate(date_Renamed + rawOffset, [cdate])
				Dim year As Integer = [cdate].normalizedYear
				If year >= startYear Then
					' Clear time elements for the transition calculations
					[cdate].timeOfDayDay(0, 0, 0, 0)
					offset_Renamed = getOffset(cal, [cdate], year, date_Renamed)
				End If
			End If

			If offsets IsNot Nothing Then
				offsets(0) = rawOffset
				offsets(1) = offset_Renamed - rawOffset
			End If
			Return offset_Renamed
		End Function

	   ''' <summary>
	   ''' Returns the difference in milliseconds between local time and
	   ''' UTC, taking into account both the raw offset and the effect of
	   ''' daylight saving, for the specified date and time.  This method
	   ''' assumes that the start and end month are distinct.  It also
	   ''' uses a default <seealso cref="GregorianCalendar"/> object as its
	   ''' underlying calendar, such as for determining leap years.  Do
	   ''' not use the result of this method with a calendar other than a
	   ''' default <code>GregorianCalendar</code>.
	   '''  
	   ''' <p><em>Note:  In general, clients should use
	   ''' <code>Calendar.get(ZONE_OFFSET) + Calendar.get(DST_OFFSET)</code>
	   ''' instead of calling this method.</em>
	   ''' </summary>
	   ''' <param name="era">       The era of the given date. </param>
	   ''' <param name="year">      The year in the given date. </param>
	   ''' <param name="month">     The month in the given date. Month is 0-based. e.g.,
	   '''                  0 for January. </param>
	   ''' <param name="day">       The day-in-month of the given date. </param>
	   ''' <param name="dayOfWeek"> The day-of-week of the given date. </param>
	   ''' <param name="millis">    The milliseconds in day in <em>standard</em> local time. </param>
	   ''' <returns>          The milliseconds to add to UTC to get local time. </returns>
	   ''' <exception cref="IllegalArgumentException"> the <code>era</code>,
	   '''                  <code>month</code>, <code>day</code>, <code>dayOfWeek</code>,
	   '''                  or <code>millis</code> parameters are out of range </exception>
		Public Overrides Function getOffset(ByVal era As Integer, ByVal year As Integer, ByVal month As Integer, ByVal day As Integer, ByVal dayOfWeek As Integer, ByVal millis As Integer) As Integer
			If era <> GregorianCalendar.AD AndAlso era <> GregorianCalendar.BC Then Throw New IllegalArgumentException("Illegal era " & era)

			Dim y As Integer = year
			If era = GregorianCalendar.BC Then y = 1 - y

			' If the year isn't representable with the 64-bit long
			' integer in milliseconds, convert the year to an
			' equivalent year. This is required to pass some JCK test cases
			' which are actually useless though because the specified years
			' can't be supported by the Java time system.
			If y >= 292278994 Then
				y = 2800 + y Mod 2800
			ElseIf y <= -292269054 Then
				' y %= 28 also produces an equivalent year, but positive
				' year numbers would be convenient to use the UNIX cal
				' command.
				y = CInt(Fix(sun.util.calendar.CalendarUtils.mod(CLng(y), 28)))
			End If

			' convert year to its 1-based month value
			Dim m As Integer = month + 1

			' First, calculate time as a Gregorian date.
			Dim cal As sun.util.calendar.BaseCalendar = gcal
			Dim [cdate] As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
			[cdate].dateate(y, m, day)
			Dim time As Long = cal.getTime([cdate]) ' normalize cdate
			time += millis - rawOffset ' UTC time

			' If the time value represents a time before the default
			' Gregorian cutover, recalculate time using the Julian
			' calendar system. For the Julian calendar system, the
			' normalized year numbering is ..., -2 (BCE 2), -1 (BCE 1),
			' 1, 2 ... which is different from the GregorianCalendar
			' style year numbering (..., -1, 0 (BCE 1), 1, 2, ...).
			If time < GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER Then
				cal = CType(sun.util.calendar.CalendarSystem.forName("julian"), sun.util.calendar.BaseCalendar)
				[cdate] = CType(cal.newCalendarDate(TimeZone.NO_TIMEZONE), sun.util.calendar.BaseCalendar.Date)
				[cdate].normalizedDateate(y, m, day)
				time = cal.getTime([cdate]) + millis - rawOffset
			End If

			If ([cdate].normalizedYear <> y) OrElse ([cdate].month <> m) OrElse ([cdate].dayOfMonth <> day) OrElse (dayOfWeek < DayOfWeek.Sunday OrElse dayOfWeek > DayOfWeek.Saturday) OrElse (millis < 0 OrElse millis >= (24*60*60*1000)) Then Throw New IllegalArgumentException

			If (Not useDaylight) OrElse year < startYear OrElse era <> GregorianCalendar.CE Then Return rawOffset

			Return getOffset(cal, [cdate], y, time)
		End Function

		Private Function getOffset(ByVal cal As sun.util.calendar.BaseCalendar, ByVal [cdate] As sun.util.calendar.BaseCalendar.Date, ByVal year As Integer, ByVal time As Long) As Integer
			SyncLock Me
				If cacheStart <> 0 Then
					If time >= cacheStart AndAlso time < cacheEnd Then Return rawOffset + dstSavings
					If year = cacheYear Then Return rawOffset
				End If
			End SyncLock

			Dim start_Renamed As Long = getStart(cal, [cdate], year)
			Dim end_Renamed As Long = getEnd(cal, [cdate], year)
			Dim offset_Renamed As Integer = rawOffset
			If start_Renamed <= end_Renamed Then
				If time >= start_Renamed AndAlso time < end_Renamed Then offset_Renamed += dstSavings
				SyncLock Me
					cacheYear = year
					cacheStart = start_Renamed
					cacheEnd = end_Renamed
				End SyncLock
			Else
				If time < end_Renamed Then
					' TODO: support Gregorian cutover. The previous year
					' may be in the other calendar system.
					start_Renamed = getStart(cal, [cdate], year - 1)
					If time >= start_Renamed Then offset_Renamed += dstSavings
				ElseIf time >= start_Renamed Then
					' TODO: support Gregorian cutover. The next year
					' may be in the other calendar system.
					end_Renamed = getEnd(cal, [cdate], year + 1)
					If time < end_Renamed Then offset_Renamed += dstSavings
				End If
				If start_Renamed <= end_Renamed Then
					SyncLock Me
						' The start and end transitions are in multiple years.
						cacheYear = CLng(startYear) - 1
						cacheStart = start_Renamed
						cacheEnd = end_Renamed
					End SyncLock
				End If
			End If
			Return offset_Renamed
		End Function

		Private Function getStart(ByVal cal As sun.util.calendar.BaseCalendar, ByVal [cdate] As sun.util.calendar.BaseCalendar.Date, ByVal year As Integer) As Long
			Dim time As Integer = startTime
			If startTimeMode <> UTC_TIME Then time -= rawOffset
			Return getTransition(cal, [cdate], startMode, year, startMonth, startDay, startDayOfWeek, time)
		End Function

		Private Function getEnd(ByVal cal As sun.util.calendar.BaseCalendar, ByVal [cdate] As sun.util.calendar.BaseCalendar.Date, ByVal year As Integer) As Long
			Dim time As Integer = endTime
			If endTimeMode <> UTC_TIME Then time -= rawOffset
			If endTimeMode = WALL_TIME Then time -= dstSavings
			Return getTransition(cal, [cdate], endMode, year, endMonth, endDay, endDayOfWeek, time)
		End Function

		Private Function getTransition(ByVal cal As sun.util.calendar.BaseCalendar, ByVal [cdate] As sun.util.calendar.BaseCalendar.Date, ByVal mode As Integer, ByVal year As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer, ByVal dayOfWeek As Integer, ByVal timeOfDay As Integer) As Long
			[cdate].normalizedYear = year
			[cdate].month = month + 1
			Select Case mode
			Case DOM_MODE
				[cdate].dayOfMonth = dayOfMonth

			Case DOW_IN_MONTH_MODE
				[cdate].dayOfMonth = 1
				If dayOfMonth < 0 Then [cdate].dayOfMonth = cal.getMonthLength([cdate])
				[cdate] = CType(cal.getNthDayOfWeek(dayOfMonth, dayOfWeek, [cdate]), sun.util.calendar.BaseCalendar.Date)

			Case DOW_GE_DOM_MODE
				[cdate].dayOfMonth = dayOfMonth
				[cdate] = CType(cal.getNthDayOfWeek(1, dayOfWeek, [cdate]), sun.util.calendar.BaseCalendar.Date)

			Case DOW_LE_DOM_MODE
				[cdate].dayOfMonth = dayOfMonth
				[cdate] = CType(cal.getNthDayOfWeek(-1, dayOfWeek, [cdate]), sun.util.calendar.BaseCalendar.Date)
			End Select
			Return cal.getTime([cdate]) + timeOfDay
		End Function

		''' <summary>
		''' Gets the GMT offset for this time zone. </summary>
		''' <returns> the GMT offset value in milliseconds </returns>
		''' <seealso cref= #setRawOffset </seealso>
		Public  Overrides ReadOnly Property  rawOffset As Integer
			Get
				' The given date will be taken into account while
				' we have the historical time zone data in place.
				Return rawOffset
			End Get
			Set(ByVal offsetMillis As Integer)
				Me.rawOffset = offsetMillis
			End Set
		End Property


		''' <summary>
		''' Sets the amount of time in milliseconds that the clock is advanced
		''' during daylight saving time. </summary>
		''' <param name="millisSavedDuringDST"> the number of milliseconds the time is
		''' advanced with respect to standard time when the daylight saving time rules
		''' are in effect. A positive number, typically one hour (3600000). </param>
		''' <seealso cref= #getDSTSavings
		''' @since 1.2 </seealso>
		Public Overridable Property dSTSavings As Integer
			Set(ByVal millisSavedDuringDST As Integer)
				If millisSavedDuringDST <= 0 Then Throw New IllegalArgumentException("Illegal daylight saving value: " & millisSavedDuringDST)
				dstSavings = millisSavedDuringDST
			End Set
			Get
				Return If(useDaylight, dstSavings, 0)
			End Get
		End Property


		''' <summary>
		''' Queries if this time zone uses daylight saving time. </summary>
		''' <returns> true if this time zone uses daylight saving time;
		''' false otherwise. </returns>
		Public Overrides Function useDaylightTime() As Boolean
			Return useDaylight
		End Function

		''' <summary>
		''' Returns {@code true} if this {@code SimpleTimeZone} observes
		''' Daylight Saving Time. This method is equivalent to {@link
		''' #useDaylightTime()}.
		''' </summary>
		''' <returns> {@code true} if this {@code SimpleTimeZone} observes
		''' Daylight Saving Time; {@code false} otherwise.
		''' @since 1.7 </returns>
		Public Overrides Function observesDaylightTime() As Boolean
			Return useDaylightTime()
		End Function

		''' <summary>
		''' Queries if the given date is in daylight saving time. </summary>
		''' <returns> true if daylight saving time is in effective at the
		''' given date; false otherwise. </returns>
		Public Overrides Function inDaylightTime(ByVal [date] As Date) As Boolean
			Return (getOffset(date_Renamed.time) <> rawOffset)
		End Function

		''' <summary>
		''' Returns a clone of this <code>SimpleTimeZone</code> instance. </summary>
		''' <returns> a clone of this instance. </returns>
		Public Overrides Function clone() As Object
			Return MyBase.clone()
		End Function

		''' <summary>
		''' Generates the hash code for the SimpleDateFormat object. </summary>
		''' <returns> the hash code for this object </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function GetHashCode() As Integer
			Return startMonth Xor startDay Xor startDayOfWeek Xor startTime Xor endMonth Xor endDay Xor endDayOfWeek Xor endTime Xor rawOffset
		End Function

		''' <summary>
		''' Compares the equality of two <code>SimpleTimeZone</code> objects.
		''' </summary>
		''' <param name="obj">  The <code>SimpleTimeZone</code> object to be compared with. </param>
		''' <returns>     True if the given <code>obj</code> is the same as this
		'''             <code>SimpleTimeZone</code> object; false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If Not(TypeOf obj Is SimpleTimeZone) Then Return False

			Dim that As SimpleTimeZone = CType(obj, SimpleTimeZone)

			Return iD.Equals(that.iD) AndAlso hasSameRules(that)
		End Function

		''' <summary>
		''' Returns <code>true</code> if this zone has the same rules and offset as another zone. </summary>
		''' <param name="other"> the TimeZone object to be compared with </param>
		''' <returns> <code>true</code> if the given zone is a SimpleTimeZone and has the
		''' same rules and offset as this one
		''' @since 1.2 </returns>
		Public Overrides Function hasSameRules(ByVal other As TimeZone) As Boolean
			If Me Is other Then Return True
			If Not(TypeOf other Is SimpleTimeZone) Then Return False
			Dim that As SimpleTimeZone = CType(other, SimpleTimeZone)
			Return rawOffset = that.rawOffset AndAlso useDaylight = that.useDaylight AndAlso ((Not useDaylight) OrElse (dstSavings = that.dstSavings AndAlso startMode = that.startMode AndAlso startMonth = that.startMonth AndAlso startDay = that.startDay AndAlso startDayOfWeek = that.startDayOfWeek AndAlso startTime = that.startTime AndAlso startTimeMode = that.startTimeMode AndAlso endMode = that.endMode AndAlso endMonth = that.endMonth AndAlso endDay = that.endDay AndAlso endDayOfWeek = that.endDayOfWeek AndAlso endTime = that.endTime AndAlso endTimeMode = that.endTimeMode AndAlso startYear = that.startYear))
				 ' Only check rules if using DST
		End Function

		''' <summary>
		''' Returns a string representation of this time zone. </summary>
		''' <returns> a string representation of this time zone. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[id=" & iD & ",offset=" & rawOffset & ",dstSavings=" & dstSavings & ",useDaylight=" & useDaylight & ",startYear=" & startYear & ",startMode=" & startMode & ",startMonth=" & startMonth & ",startDay=" & startDay & ",startDayOfWeek=" & startDayOfWeek & ",startTime=" & startTime & ",startTimeMode=" & startTimeMode & ",endMode=" & endMode & ",endMonth=" & endMonth & ",endDay=" & endDay & ",endDayOfWeek=" & endDayOfWeek & ",endTime=" & endTime & ",endTimeMode=" & endTimeMode + AscW("]"c)
		End Function

		' =======================privates===============================

		''' <summary>
		''' The month in which daylight saving time starts.  This value must be
		''' between <code>Calendar.JANUARY</code> and
		''' <code>Calendar.DECEMBER</code> inclusive.  This value must not equal
		''' <code>endMonth</code>.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private startMonth As Integer

		''' <summary>
		''' This field has two possible interpretations:
		''' <dl>
		''' <dt><code>startMode == DOW_IN_MONTH</code></dt>
		''' <dd>
		''' <code>startDay</code> indicates the day of the month of
		''' <code>startMonth</code> on which daylight
		''' saving time starts, from 1 to 28, 30, or 31, depending on the
		''' <code>startMonth</code>.
		''' </dd>
		''' <dt><code>startMode != DOW_IN_MONTH</code></dt>
		''' <dd>
		''' <code>startDay</code> indicates which <code>startDayOfWeek</code> in the
		''' month <code>startMonth</code> daylight
		''' saving time starts on.  For example, a value of +1 and a
		''' <code>startDayOfWeek</code> of <code>Calendar.SUNDAY</code> indicates the
		''' first Sunday of <code>startMonth</code>.  Likewise, +2 would indicate the
		''' second Sunday, and -1 the last Sunday.  A value of 0 is illegal.
		''' </dd>
		''' </dl>
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private startDay As Integer

		''' <summary>
		''' The day of the week on which daylight saving time starts.  This value
		''' must be between <code>Calendar.SUNDAY</code> and
		''' <code>Calendar.SATURDAY</code> inclusive.
		''' <p>If <code>useDaylight</code> is false or
		''' <code>startMode == DAY_OF_MONTH</code>, this value is ignored.
		''' @serial
		''' </summary>
		Private startDayOfWeek As Integer

		''' <summary>
		''' The time in milliseconds after midnight at which daylight saving
		''' time starts.  This value is expressed as wall time, standard time,
		''' or UTC time, depending on the setting of <code>startTimeMode</code>.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private startTime As Integer

		''' <summary>
		''' The format of startTime, either WALL_TIME, STANDARD_TIME, or UTC_TIME.
		''' @serial
		''' @since 1.3
		''' </summary>
		Private startTimeMode As Integer

		''' <summary>
		''' The month in which daylight saving time ends.  This value must be
		''' between <code>Calendar.JANUARY</code> and
		''' <code>Calendar.UNDECIMBER</code>.  This value must not equal
		''' <code>startMonth</code>.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private endMonth As Integer

		''' <summary>
		''' This field has two possible interpretations:
		''' <dl>
		''' <dt><code>endMode == DOW_IN_MONTH</code></dt>
		''' <dd>
		''' <code>endDay</code> indicates the day of the month of
		''' <code>endMonth</code> on which daylight
		''' saving time ends, from 1 to 28, 30, or 31, depending on the
		''' <code>endMonth</code>.
		''' </dd>
		''' <dt><code>endMode != DOW_IN_MONTH</code></dt>
		''' <dd>
		''' <code>endDay</code> indicates which <code>endDayOfWeek</code> in th
		''' month <code>endMonth</code> daylight
		''' saving time ends on.  For example, a value of +1 and a
		''' <code>endDayOfWeek</code> of <code>Calendar.SUNDAY</code> indicates the
		''' first Sunday of <code>endMonth</code>.  Likewise, +2 would indicate the
		''' second Sunday, and -1 the last Sunday.  A value of 0 is illegal.
		''' </dd>
		''' </dl>
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private endDay As Integer

		''' <summary>
		''' The day of the week on which daylight saving time ends.  This value
		''' must be between <code>Calendar.SUNDAY</code> and
		''' <code>Calendar.SATURDAY</code> inclusive.
		''' <p>If <code>useDaylight</code> is false or
		''' <code>endMode == DAY_OF_MONTH</code>, this value is ignored.
		''' @serial
		''' </summary>
		Private endDayOfWeek As Integer

		''' <summary>
		''' The time in milliseconds after midnight at which daylight saving
		''' time ends.  This value is expressed as wall time, standard time,
		''' or UTC time, depending on the setting of <code>endTimeMode</code>.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private endTime As Integer

		''' <summary>
		''' The format of endTime, either <code>WALL_TIME</code>,
		''' <code>STANDARD_TIME</code>, or <code>UTC_TIME</code>.
		''' @serial
		''' @since 1.3
		''' </summary>
		Private endTimeMode As Integer

		''' <summary>
		''' The year in which daylight saving time is first observed.  This is an <seealso cref="GregorianCalendar#AD AD"/>
		''' value.  If this value is less than 1 then daylight saving time is observed
		''' for all <code>AD</code> years.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' </summary>
		Private startYear As Integer

		''' <summary>
		''' The offset in milliseconds between this zone and GMT.  Negative offsets
		''' are to the west of Greenwich.  To obtain local <em>standard</em> time,
		''' add the offset to GMT time.  To obtain local wall time it may also be
		''' necessary to add <code>dstSavings</code>.
		''' @serial
		''' </summary>
		Private rawOffset As Integer

		''' <summary>
		''' A boolean value which is true if and only if this zone uses daylight
		''' saving time.  If this value is false, several other fields are ignored.
		''' @serial
		''' </summary>
		Private useDaylight As Boolean=False ' indicate if this time zone uses DST

		Private Const millisPerHour As Integer = 60*60*1000
		Private Shared ReadOnly millisPerDay As Integer = 24*millisPerHour

		''' <summary>
		''' This field was serialized in JDK 1.1, so we have to keep it that way
		''' to maintain serialization compatibility. However, there's no need to
		''' recreate the array each time we create a new time zone.
		''' @serial An array of bytes containing the values {31, 28, 31, 30, 31, 30,
		''' 31, 31, 30, 31, 30, 31}.  This is ignored as of the Java 2 platform v1.2, however, it must
		''' be streamed out for compatibility with JDK 1.1.
		''' </summary>
		Private ReadOnly monthLength As SByte() = staticMonthLength
		Private Shared ReadOnly staticMonthLength As SByte() = {31,28,31,30,31,30,31,31,30,31,30,31}
		Private Shared ReadOnly staticLeapMonthLength As SByte() = {31,29,31,30,31,30,31,31,30,31,30,31}

		''' <summary>
		''' Variables specifying the mode of the start rule.  Takes the following
		''' values:
		''' <dl>
		''' <dt><code>DOM_MODE</code></dt>
		''' <dd>
		''' Exact day of week; e.g., March 1.
		''' </dd>
		''' <dt><code>DOW_IN_MONTH_MODE</code></dt>
		''' <dd>
		''' Day of week in month; e.g., last Sunday in March.
		''' </dd>
		''' <dt><code>DOW_GE_DOM_MODE</code></dt>
		''' <dd>
		''' Day of week after day of month; e.g., Sunday on or after March 15.
		''' </dd>
		''' <dt><code>DOW_LE_DOM_MODE</code></dt>
		''' <dd>
		''' Day of week before day of month; e.g., Sunday on or before March 15.
		''' </dd>
		''' </dl>
		''' The setting of this field affects the interpretation of the
		''' <code>startDay</code> field.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' @since 1.1.4
		''' </summary>
		Private startMode As Integer

		''' <summary>
		''' Variables specifying the mode of the end rule.  Takes the following
		''' values:
		''' <dl>
		''' <dt><code>DOM_MODE</code></dt>
		''' <dd>
		''' Exact day of week; e.g., March 1.
		''' </dd>
		''' <dt><code>DOW_IN_MONTH_MODE</code></dt>
		''' <dd>
		''' Day of week in month; e.g., last Sunday in March.
		''' </dd>
		''' <dt><code>DOW_GE_DOM_MODE</code></dt>
		''' <dd>
		''' Day of week after day of month; e.g., Sunday on or after March 15.
		''' </dd>
		''' <dt><code>DOW_LE_DOM_MODE</code></dt>
		''' <dd>
		''' Day of week before day of month; e.g., Sunday on or before March 15.
		''' </dd>
		''' </dl>
		''' The setting of this field affects the interpretation of the
		''' <code>endDay</code> field.
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' @since 1.1.4
		''' </summary>
		Private endMode As Integer

		''' <summary>
		''' A positive value indicating the amount of time saved during DST in
		''' milliseconds.
		''' Typically one hour (3600000); sometimes 30 minutes (1800000).
		''' <p>If <code>useDaylight</code> is false, this value is ignored.
		''' @serial
		''' @since 1.1.4
		''' </summary>
		Private dstSavings As Integer

		Private Shared ReadOnly gcal As sun.util.calendar.Gregorian = sun.util.calendar.CalendarSystem.gregorianCalendar

		''' <summary>
		''' Cache values representing a single period of daylight saving
		''' time. When the cache values are valid, cacheStart is the start
		''' time (inclusive) of daylight saving time and cacheEnd is the
		''' end time (exclusive).
		''' 
		''' cacheYear has a year value if both cacheStart and cacheEnd are
		''' in the same year. cacheYear is set to startYear - 1 if
		''' cacheStart and cacheEnd are in different years. cacheStart is 0
		''' if the cache values are void. cacheYear is a long to support
		'''  java.lang.[Integer].MIN_VALUE - 1 (JCK requirement).
		''' </summary>
		<NonSerialized> _
		Private cacheYear As Long
		<NonSerialized> _
		Private cacheStart As Long
		<NonSerialized> _
		Private cacheEnd As Long

		''' <summary>
		''' Constants specifying values of startMode and endMode.
		''' </summary>
		Private Const DOM_MODE As Integer = 1 ' Exact day of month, "Mar 1"
		Private Const DOW_IN_MONTH_MODE As Integer = 2 ' Day of week in month, "lastSun"
		Private Const DOW_GE_DOM_MODE As Integer = 3 ' Day of week after day of month, "Sun>=15"
		Private Const DOW_LE_DOM_MODE As Integer = 4 ' Day of week before day of month, "Sun<=21"

		''' <summary>
		''' Constant for a mode of start or end time specified as wall clock
		''' time.  Wall clock time is standard time for the onset rule, and
		''' daylight time for the end rule.
		''' @since 1.4
		''' </summary>
		Public Const WALL_TIME As Integer = 0 ' Zero for backward compatibility

		''' <summary>
		''' Constant for a mode of start or end time specified as standard time.
		''' @since 1.4
		''' </summary>
		Public Const STANDARD_TIME As Integer = 1

		''' <summary>
		''' Constant for a mode of start or end time specified as UTC. European
		''' Union rules are specified as UTC time, for example.
		''' @since 1.4
		''' </summary>
		Public Const UTC_TIME As Integer = 2

		' Proclaim compatibility with 1.1
		Friend Shadows Const serialVersionUID As Long = -403250971215465050L

		' the internal serial version which says which version was written
		' - 0 (default) for version up to JDK 1.1.3
		' - 1 for version from JDK 1.1.4, which includes 3 new fields
		' - 2 for JDK 1.3, which includes 2 new fields
		Friend Const currentSerialVersion As Integer = 2

		''' <summary>
		''' The version of the serialized data on the stream.  Possible values:
		''' <dl>
		''' <dt><b>0</b> or not present on stream</dt>
		''' <dd>
		''' JDK 1.1.3 or earlier.
		''' </dd>
		''' <dt><b>1</b></dt>
		''' <dd>
		''' JDK 1.1.4 or later.  Includes three new fields: <code>startMode</code>,
		''' <code>endMode</code>, and <code>dstSavings</code>.
		''' </dd>
		''' <dt><b>2</b></dt>
		''' <dd>
		''' JDK 1.3 or later.  Includes two new fields: <code>startTimeMode</code>
		''' and <code>endTimeMode</code>.
		''' </dd>
		''' </dl>
		''' When streaming out this [Class], the most recent format
		''' and the highest allowable <code>serialVersionOnStream</code>
		''' is written.
		''' @serial
		''' @since 1.1.4
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub invalidateCache()
			cacheYear = startYear - 1
				cacheEnd = 0
				cacheStart = cacheEnd
		End Sub

		'----------------------------------------------------------------------
		' Rule representation
		'
		' We represent the following flavors of rules:
		'       5        the fifth of the month
		'       lastSun  the last Sunday in the month
		'       lastMon  the last Monday in the month
		'       Sun>=8   first Sunday on or after the eighth
		'       Sun<=25  last Sunday on or before the 25th
		' This is further complicated by the fact that we need to remain
		' backward compatible with the 1.1 FCS.  Finally, we need to minimize
		' API changes.  In order to satisfy these requirements, we support
		' three representation systems, and we translate between them.
		'
		' INTERNAL REPRESENTATION
		' This is the format SimpleTimeZone objects take after construction or
		' streaming in is complete.  Rules are represented directly, using an
		' unencoded format.  We will discuss the start rule only below; the end
		' rule is analogous.
		'   startMode      Takes on enumerated values DAY_OF_MONTH,
		'                  DOW_IN_MONTH, DOW_AFTER_DOM, or DOW_BEFORE_DOM.
		'   startDay       The day of the month, or for DOW_IN_MONTH mode, a
		'                  value indicating which DOW, such as +1 for first,
		'                  +2 for second, -1 for last, etc.
		'   startDayOfWeek The day of the week.  Ignored for DAY_OF_MONTH.
		'
		' ENCODED REPRESENTATION
		' This is the format accepted by the constructor and by setStartRule()
		' and setEndRule().  It uses various combinations of positive, negative,
		' and zero values to encode the different rules.  This representation
		' allows us to specify all the different rule flavors without altering
		' the API.
		'   MODE              startMonth    startDay    startDayOfWeek
		'   DOW_IN_MONTH_MODE >=0           !=0         >0
		'   DOM_MODE          >=0           >0          ==0
		'   DOW_GE_DOM_MODE   >=0           >0          <0
		'   DOW_LE_DOM_MODE   >=0           <0          <0
		'   (no DST)          don't care    ==0         don't care
		'
		' STREAMED REPRESENTATION
		' We must retain binary compatibility with the 1.1 FCS.  The 1.1 code only
		' handles DOW_IN_MONTH_MODE and non-DST mode, the latter indicated by the
		' flag useDaylight.  When we stream an object out, we translate into an
		' approximate DOW_IN_MONTH_MODE representation so the object can be parsed
		' and used by 1.1 code.  Following that, we write out the full
		' representation separately so that contemporary code can recognize and
		' parse it.  The full representation is written in a "packed" format,
		' consisting of a version number, a length, and an array of bytes.  Future
		' versions of this class may specify different versions.  If they wish to
		' include additional data, they should do so by storing them after the
		' packed representation below.
		'----------------------------------------------------------------------

		''' <summary>
		''' Given a set of encoded rules in startDay and startDayOfMonth, decode
		''' them and set the startMode appropriately.  Do the same for endDay and
		''' endDayOfMonth.  Upon entry, the day of week variables may be zero or
		''' negative, in order to indicate special modes.  The day of month
		''' variables may also be negative.  Upon exit, the mode variables will be
		''' set, and the day of week and day of month variables will be positive.
		''' This method also recognizes a startDay or endDay of zero as indicating
		''' no DST.
		''' </summary>
		Private Sub decodeRules()
			decodeStartRule()
			decodeEndRule()
		End Sub

		''' <summary>
		''' Decode the start rule and validate the parameters.  The parameters are
		''' expected to be in encoded form, which represents the various rule modes
		''' by negating or zeroing certain values.  Representation formats are:
		''' <p>
		''' <pre>
		'''            DOW_IN_MONTH  DOM    DOW>=DOM  DOW<=DOM  no DST
		'''            ------------  -----  --------  --------  ----------
		''' month       0..11        same    same      same     don't care
		''' day        -5..5         1..31   1..31    -1..-31   0
		''' dayOfWeek   1..7         0      -1..-7    -1..-7    don't care
		''' time        0..ONEDAY    same    same      same     don't care
		''' </pre>
		''' The range for month does not include UNDECIMBER since this class is
		''' really specific to GregorianCalendar, which does not use that month.
		''' The range for time includes ONEDAY (vs. ending at ONEDAY-1) because the
		''' end rule is an exclusive limit point.  That is, the range of times that
		''' are in DST include those >= the start and < the end.  For this reason,
		''' it should be possible to specify an end of ONEDAY in order to include the
		''' entire day.  Although this is equivalent to time 0 of the following day,
		''' it's not always possible to specify that, for example, on December 31.
		''' While arguably the start range should still be 0..ONEDAY-1, we keep
		''' the start and end ranges the same for consistency.
		''' </summary>
		Private Sub decodeStartRule()
			useDaylight = (startDay <> 0) AndAlso (endDay <> 0)
			If startDay <> 0 Then
				If startMonth < 1 OrElse startMonth > 12 Then Throw New IllegalArgumentException("Illegal start month " & startMonth)
				If startTime < 0 OrElse startTime > millisPerDay Then Throw New IllegalArgumentException("Illegal start time " & startTime)
				If startDayOfWeek = 0 Then
					startMode = DOM_MODE
				Else
					If startDayOfWeek > 0 Then
						startMode = DOW_IN_MONTH_MODE
					Else
						startDayOfWeek = -startDayOfWeek
						If startDay > 0 Then
							startMode = DOW_GE_DOM_MODE
						Else
							startDay = -startDay
							startMode = DOW_LE_DOM_MODE
						End If
					End If
					If startDayOfWeek > DayOfWeek.Saturday Then Throw New IllegalArgumentException("Illegal start day of week " & startDayOfWeek)
				End If
				If startMode = DOW_IN_MONTH_MODE Then
					If startDay < -5 OrElse startDay > 5 Then Throw New IllegalArgumentException("Illegal start day of week in month " & startDay)
				ElseIf startDay < 1 OrElse startDay > staticMonthLength(startMonth) Then
					Throw New IllegalArgumentException("Illegal start day " & startDay)
				End If
			End If
		End Sub

		''' <summary>
		''' Decode the end rule and validate the parameters.  This method is exactly
		''' analogous to decodeStartRule(). </summary>
		''' <seealso cref= decodeStartRule </seealso>
		Private Sub decodeEndRule()
			useDaylight = (startDay <> 0) AndAlso (endDay <> 0)
			If endDay <> 0 Then
				If endMonth < 1 OrElse endMonth > 12 Then Throw New IllegalArgumentException("Illegal end month " & endMonth)
				If endTime < 0 OrElse endTime > millisPerDay Then Throw New IllegalArgumentException("Illegal end time " & endTime)
				If endDayOfWeek = 0 Then
					endMode = DOM_MODE
				Else
					If endDayOfWeek > 0 Then
						endMode = DOW_IN_MONTH_MODE
					Else
						endDayOfWeek = -endDayOfWeek
						If endDay > 0 Then
							endMode = DOW_GE_DOM_MODE
						Else
							endDay = -endDay
							endMode = DOW_LE_DOM_MODE
						End If
					End If
					If endDayOfWeek > DayOfWeek.Saturday Then Throw New IllegalArgumentException("Illegal end day of week " & endDayOfWeek)
				End If
				If endMode = DOW_IN_MONTH_MODE Then
					If endDay < -5 OrElse endDay > 5 Then Throw New IllegalArgumentException("Illegal end day of week in month " & endDay)
				ElseIf endDay < 1 OrElse endDay > staticMonthLength(endMonth) Then
					Throw New IllegalArgumentException("Illegal end day " & endDay)
				End If
			End If
		End Sub

		''' <summary>
		''' Make rules compatible to 1.1 FCS code.  Since 1.1 FCS code only understands
		''' day-of-week-in-month rules, we must modify other modes of rules to their
		''' approximate equivalent in 1.1 FCS terms.  This method is used when streaming
		''' out objects of this class.  After it is called, the rules will be modified,
		''' with a possible loss of information.  startMode and endMode will NOT be
		''' altered, even though semantically they should be set to DOW_IN_MONTH_MODE,
		''' since the rule modification is only intended to be temporary.
		''' </summary>
		Private Sub makeRulesCompatible()
			Select Case startMode
			Case DOM_MODE
				startDay = 1 + (startDay \ 7)
				startDayOfWeek = DayOfWeek.Sunday

			Case DOW_GE_DOM_MODE
				' A day-of-month of 1 is equivalent to DOW_IN_MONTH_MODE
				' that is, Sun>=1 == firstSun.
				If startDay <> 1 Then startDay = 1 + (startDay \ 7)

			Case DOW_LE_DOM_MODE
				If startDay >= 30 Then
					startDay = -1
				Else
					startDay = 1 + (startDay \ 7)
				End If
			End Select

			Select Case endMode
			Case DOM_MODE
				endDay = 1 + (endDay \ 7)
				endDayOfWeek = DayOfWeek.Sunday

			Case DOW_GE_DOM_MODE
				' A day-of-month of 1 is equivalent to DOW_IN_MONTH_MODE
				' that is, Sun>=1 == firstSun.
				If endDay <> 1 Then endDay = 1 + (endDay \ 7)

			Case DOW_LE_DOM_MODE
				If endDay >= 30 Then
					endDay = -1
				Else
					endDay = 1 + (endDay \ 7)
				End If
			End Select

	'        
	'         * Adjust the start and end times to wall time.  This works perfectly
	'         * well unless it pushes into the next or previous day.  If that
	'         * happens, we attempt to adjust the day rule somewhat crudely.  The day
	'         * rules have been forced into DOW_IN_MONTH mode already, so we change
	'         * the day of week to move forward or back by a day.  It's possible to
	'         * make a more refined adjustment of the original rules first, but in
	'         * most cases this extra effort will go to waste once we adjust the day
	'         * rules anyway.
	'         
			Select Case startTimeMode
			Case UTC_TIME
				startTime += rawOffset
			End Select
			Do While startTime < 0
				startTime += millisPerDay
				startDayOfWeek = 1 + ((startDayOfWeek+5) Mod 7) ' Back 1 day
			Loop
			Do While startTime >= millisPerDay
				startTime -= millisPerDay
				startDayOfWeek = 1 + (startDayOfWeek Mod 7) ' Forward 1 day
			Loop

			Select Case endTimeMode
			Case UTC_TIME
				endTime += rawOffset + dstSavings
			Case STANDARD_TIME
				endTime += dstSavings
			End Select
			Do While endTime < 0
				endTime += millisPerDay
				endDayOfWeek = 1 + ((endDayOfWeek+5) Mod 7) ' Back 1 day
			Loop
			Do While endTime >= millisPerDay
				endTime -= millisPerDay
				endDayOfWeek = 1 + (endDayOfWeek Mod 7) ' Forward 1 day
			Loop
		End Sub

		''' <summary>
		''' Pack the start and end rules into an array of bytes.  Only pack
		''' data which is not preserved by makeRulesCompatible.
		''' </summary>
		Private Function packRules() As SByte()
			Dim rules As SByte() = New SByte(5){}
			rules(0) = CByte(startDay)
			rules(1) = CByte(startDayOfWeek)
			rules(2) = CByte(endDay)
			rules(3) = CByte(endDayOfWeek)

			' As of serial version 2, include time modes
			rules(4) = CByte(startTimeMode)
			rules(5) = CByte(endTimeMode)

			Return rules
		End Function

		''' <summary>
		''' Given an array of bytes produced by packRules, interpret them
		''' as the start and end rules.
		''' </summary>
		Private Sub unpackRules(ByVal rules As SByte())
			startDay = rules(0)
			startDayOfWeek = rules(1)
			endDay = rules(2)
			endDayOfWeek = rules(3)

			' As of serial version 2, include time modes
			If rules.Length >= 6 Then
				startTimeMode = rules(4)
				endTimeMode = rules(5)
			End If
		End Sub

		''' <summary>
		''' Pack the start and end times into an array of bytes.  This is required
		''' as of serial version 2.
		''' </summary>
		Private Function packTimes() As Integer()
			Dim times As Integer() = New Integer(1){}
			times(0) = startTime
			times(1) = endTime
			Return times
		End Function

		''' <summary>
		''' Unpack the start and end times from an array of bytes.  This is required
		''' as of serial version 2.
		''' </summary>
		Private Sub unpackTimes(ByVal times As Integer())
			startTime = times(0)
			endTime = times(1)
		End Sub

		''' <summary>
		''' Save the state of this object to a stream (i.e., serialize it).
		''' 
		''' @serialData We write out two formats, a JDK 1.1 compatible format, using
		''' <code>DOW_IN_MONTH_MODE</code> rules, in the required section, followed
		''' by the full rules, in packed format, in the optional section.  The
		''' optional section will be ignored by JDK 1.1 code upon stream in.
		''' <p> Contents of the optional section: The length of a byte array is
		''' emitted (int); this is 4 as of this release. The byte array of the given
		''' length is emitted. The contents of the byte array are the true values of
		''' the fields <code>startDay</code>, <code>startDayOfWeek</code>,
		''' <code>endDay</code>, and <code>endDayOfWeek</code>.  The values of these
		''' fields in the required section are approximate values suited to the rule
		''' mode <code>DOW_IN_MONTH_MODE</code>, which is the only mode recognized by
		''' JDK 1.1.
		''' </summary>
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			' Construct a binary rule
			Dim rules As SByte() = packRules()
			Dim times As Integer() = packTimes()

			' Convert to 1.1 FCS rules.  This step may cause us to lose information.
			makeRulesCompatible()

			' Write out the 1.1 FCS rules
			stream.defaultWriteObject()

			' Write out the binary rules in the optional data area of the stream.
			stream.writeInt(rules.Length)
			stream.write(rules)
			stream.writeObject(times)

			' Recover the original rules.  This recovers the information lost
			' by makeRulesCompatible.
			unpackRules(rules)
			unpackTimes(times)
		End Sub

		''' <summary>
		''' Reconstitute this object from a stream (i.e., deserialize it).
		''' 
		''' We handle both JDK 1.1
		''' binary formats and full formats with a packed byte array.
		''' </summary>
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			stream.defaultReadObject()

			If serialVersionOnStream < 1 Then
				' Fix a bug in the 1.1 SimpleTimeZone code -- namely,
				' startDayOfWeek and endDayOfWeek were usually uninitialized.  We can't do
				' too much, so we assume SUNDAY, which actually works most of the time.
				If startDayOfWeek = 0 Then startDayOfWeek = DayOfWeek.Sunday
				If endDayOfWeek = 0 Then endDayOfWeek = DayOfWeek.Sunday

				' The variables dstSavings, startMode, and endMode are post-1.1, so they
				' won't be present if we're reading from a 1.1 stream.  Fix them up.
					endMode = DOW_IN_MONTH_MODE
					startMode = endMode
				dstSavings = millisPerHour
			Else
				' For 1.1.4, in addition to the 3 new instance variables, we also
				' store the actual rules (which have not be made compatible with 1.1)
				' in the optional area.  Read them in here and parse them.
				Dim length As Integer = stream.readInt()
				Dim rules As SByte() = New SByte(length - 1){}
				stream.readFully(rules)
				unpackRules(rules)
			End If

			If serialVersionOnStream >= 2 Then
				Dim times As Integer() = CType(stream.readObject(), Integer())
				unpackTimes(times)
			End If

			serialVersionOnStream = currentSerialVersion
		End Sub
	End Class

End Namespace