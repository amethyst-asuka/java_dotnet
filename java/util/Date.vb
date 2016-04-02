Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The class <code>Date</code> represents a specific instant
	''' in time, with millisecond precision.
	''' <p>
	''' Prior to JDK&nbsp;1.1, the class <code>Date</code> had two additional
	''' functions.  It allowed the interpretation of dates as year, month, day, hour,
	''' minute, and second values.  It also allowed the formatting and parsing
	''' of date strings.  Unfortunately, the API for these functions was not
	''' amenable to internationalization.  As of JDK&nbsp;1.1, the
	''' <code>Calendar</code> class should be used to convert between dates and time
	''' fields and the <code>DateFormat</code> class should be used to format and
	''' parse date strings.
	''' The corresponding methods in <code>Date</code> are deprecated.
	''' <p>
	''' Although the <code>Date</code> class is intended to reflect
	''' coordinated universal time (UTC), it may not do so exactly,
	''' depending on the host environment of the Java Virtual Machine.
	''' Nearly all modern operating systems assume that 1&nbsp;day&nbsp;=
	''' 24&nbsp;&times;&nbsp;60&nbsp;&times;&nbsp;60&nbsp;= 86400 seconds
	''' in all cases. In UTC, however, about once every year or two there
	''' is an extra second, called a "leap second." The leap
	''' second is always added as the last second of the day, and always
	''' on December 31 or June 30. For example, the last minute of the
	''' year 1995 was 61 seconds long, thanks to an added leap second.
	''' Most computer clocks are not accurate enough to be able to reflect
	''' the leap-second distinction.
	''' <p>
	''' Some computer standards are defined in terms of Greenwich mean
	''' time (GMT), which is equivalent to universal time (UT).  GMT is
	''' the "civil" name for the standard; UT is the
	''' "scientific" name for the same standard. The
	''' distinction between UTC and UT is that UTC is based on an atomic
	''' clock and UT is based on astronomical observations, which for all
	''' practical purposes is an invisibly fine hair to split. Because the
	''' earth's rotation is not uniform (it slows down and speeds up
	''' in complicated ways), UT does not always flow uniformly. Leap
	''' seconds are introduced as needed into UTC so as to keep UTC within
	''' 0.9 seconds of UT1, which is a version of UT with certain
	''' corrections applied. There are other time and date systems as
	''' well; for example, the time scale used by the satellite-based
	''' global positioning system (GPS) is synchronized to UTC but is
	''' <i>not</i> adjusted for leap seconds. An interesting source of
	''' further information is the U.S. Naval Observatory, particularly
	''' the Directorate of Time at:
	''' <blockquote><pre>
	'''     <a href=http://tycho.usno.navy.mil>http://tycho.usno.navy.mil</a>
	''' </pre></blockquote>
	''' <p>
	''' and their definitions of "Systems of Time" at:
	''' <blockquote><pre>
	'''     <a href=http://tycho.usno.navy.mil/systime.html>http://tycho.usno.navy.mil/systime.html</a>
	''' </pre></blockquote>
	''' <p>
	''' In all methods of class <code>Date</code> that accept or return
	''' year, month, date, hours, minutes, and seconds values, the
	''' following representations are used:
	''' <ul>
	''' <li>A year <i>y</i> is represented by the integer
	'''     <i>y</i>&nbsp;<code>-&nbsp;1900</code>.
	''' <li>A month is represented by an integer from 0 to 11; 0 is January,
	'''     1 is February, and so forth; thus 11 is December.
	''' <li>A date (day of month) is represented by an integer from 1 to 31
	'''     in the usual manner.
	''' <li>An hour is represented by an integer from 0 to 23. Thus, the hour
	'''     from midnight to 1 a.m. is hour 0, and the hour from noon to 1
	'''     p.m. is hour 12.
	''' <li>A minute is represented by an integer from 0 to 59 in the usual manner.
	''' <li>A second is represented by an integer from 0 to 61; the values 60 and
	'''     61 occur only for leap seconds and even then only in Java
	'''     implementations that actually track leap seconds correctly. Because
	'''     of the manner in which leap seconds are currently introduced, it is
	'''     extremely unlikely that two leap seconds will occur in the same
	'''     minute, but this specification follows the date and time conventions
	'''     for ISO C.
	''' </ul>
	''' <p>
	''' In all cases, arguments given to methods for these purposes need
	''' not fall within the indicated ranges; for example, a date may be
	''' specified as January 32 and is interpreted as meaning February 1.
	''' 
	''' @author  James Gosling
	''' @author  Arthur van Hoff
	''' @author  Alan Liu </summary>
	''' <seealso cref=     java.text.DateFormat </seealso>
	''' <seealso cref=     java.util.Calendar </seealso>
	''' <seealso cref=     java.util.TimeZone
	''' @since   JDK1.0 </seealso>
	<Serializable> _
	Public Class [Date]
		Implements Cloneable, Comparable(Of Date)

		Private Shared ReadOnly gcal As sun.util.calendar.BaseCalendar = sun.util.calendar.CalendarSystem.gregorianCalendar
		Private Shared jcal As sun.util.calendar.BaseCalendar

		<NonSerialized> _
		Private fastTime As Long

	'    
	'     * If cdate is null, then fastTime indicates the time in millis.
	'     * If cdate.isNormalized() is true, then fastTime and cdate are in
	'     * synch. Otherwise, fastTime is ignored, and cdate indicates the
	'     * time.
	'     
		<NonSerialized> _
		Private [cdate] As sun.util.calendar.BaseCalendar.Date

		' Initialized just before the value is used. See parse().
		Private Shared defaultCenturyStart As Integer

	'     use serialVersionUID from modified java.util.Date for
	'     * interoperability with JDK1.1. The Date was modified to write
	'     * and read only the UTC time.
	'     
		Private Const serialVersionUID As Long = 7523967970034938905L

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it so that
		''' it represents the time at which it was allocated, measured to the
		''' nearest millisecond.
		''' </summary>
		''' <seealso cref=     java.lang.System#currentTimeMillis() </seealso>
		Public Sub New()
			Me.New(System.currentTimeMillis())
		End Sub

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it to
		''' represent the specified number of milliseconds since the
		''' standard base time known as "the epoch", namely January 1,
		''' 1970, 00:00:00 GMT.
		''' </summary>
		''' <param name="date">   the milliseconds since January 1, 1970, 00:00:00 GMT. </param>
		''' <seealso cref=     java.lang.System#currentTimeMillis() </seealso>
		Public Sub New(ByVal [date] As Long)
			fastTime = date_Renamed
		End Sub

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it so that
		''' it represents midnight, local time, at the beginning of the day
		''' specified by the <code>year</code>, <code>month</code>, and
		''' <code>date</code> arguments.
		''' </summary>
		''' <param name="year">    the year minus 1900. </param>
		''' <param name="month">   the month between 0-11. </param>
		''' <param name="date">    the day of the month between 1-31. </param>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.set(year + 1900, month, date)</code>
		''' or <code>GregorianCalendar(year + 1900, month, date)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Sub New(ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer)
			Me.New(year, month, date_Renamed, 0, 0, 0)
		End Sub

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it so that
		''' it represents the instant at the start of the minute specified by
		''' the <code>year</code>, <code>month</code>, <code>date</code>,
		''' <code>hrs</code>, and <code>min</code> arguments, in the local
		''' time zone.
		''' </summary>
		''' <param name="year">    the year minus 1900. </param>
		''' <param name="month">   the month between 0-11. </param>
		''' <param name="date">    the day of the month between 1-31. </param>
		''' <param name="hrs">     the hours between 0-23. </param>
		''' <param name="min">     the minutes between 0-59. </param>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.set(year + 1900, month, date,
		''' hrs, min)</code> or <code>GregorianCalendar(year + 1900,
		''' month, date, hrs, min)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Sub New(ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer, ByVal hrs As Integer, ByVal min As Integer)
			Me.New(year, month, date_Renamed, hrs, min, 0)
		End Sub

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it so that
		''' it represents the instant at the start of the second specified
		''' by the <code>year</code>, <code>month</code>, <code>date</code>,
		''' <code>hrs</code>, <code>min</code>, and <code>sec</code> arguments,
		''' in the local time zone.
		''' </summary>
		''' <param name="year">    the year minus 1900. </param>
		''' <param name="month">   the month between 0-11. </param>
		''' <param name="date">    the day of the month between 1-31. </param>
		''' <param name="hrs">     the hours between 0-23. </param>
		''' <param name="min">     the minutes between 0-59. </param>
		''' <param name="sec">     the seconds between 0-59. </param>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.set(year + 1900, month, date,
		''' hrs, min, sec)</code> or <code>GregorianCalendar(year + 1900,
		''' month, date, hrs, min, sec)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Sub New(ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer, ByVal hrs As Integer, ByVal min As Integer, ByVal sec As Integer)
			Dim y As Integer = year + 1900
			' month is 0-based. So we have to normalize month to support java.lang.[Long].MAX_VALUE.
			If month >= 12 Then
				y += month \ 12
				month = month Mod 12
			ElseIf month < 0 Then
				y += sun.util.calendar.CalendarUtils.floorDivide(month, 12)
				month = sun.util.calendar.CalendarUtils.mod(month, 12)
			End If
			Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(y)
			[cdate] = CType(cal.newCalendarDate(TimeZone.defaultRef), sun.util.calendar.BaseCalendar.Date)
			[cdate].normalizedDateate(y, month + 1, date_Renamed).setTimeOfDay(hrs, min, sec, 0)
			timeImpl
			[cdate] = Nothing
		End Sub

		''' <summary>
		''' Allocates a <code>Date</code> object and initializes it so that
		''' it represents the date and time indicated by the string
		''' <code>s</code>, which is interpreted as if by the
		''' <seealso cref="Date#parse"/> method.
		''' </summary>
		''' <param name="s">   a string representation of the date. </param>
		''' <seealso cref=     java.text.DateFormat </seealso>
		''' <seealso cref=     java.util.Date#parse(java.lang.String) </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>DateFormat.parse(String s)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Sub New(ByVal s As String)
			Me.New(parse(s))
		End Sub

		''' <summary>
		''' Return a copy of this object.
		''' </summary>
		Public Overridable Function clone() As Object
			Dim d As Date = Nothing
			Try
				d = CDate(MyBase.clone())
				If [cdate] IsNot Nothing Then
					d.cdate = CType([cdate].clone(), sun.util.calendar.BaseCalendar.Date)
				End If ' Won't happen
			Catch e As CloneNotSupportedException
			End Try
			Return d
		End Function

		''' <summary>
		''' Determines the date and time based on the arguments. The
		''' arguments are interpreted as a year, month, day of the month,
		''' hour of the day, minute within the hour, and second within the
		''' minute, exactly as for the <tt>Date</tt> constructor with six
		''' arguments, except that the arguments are interpreted relative
		''' to UTC rather than to the local time zone. The time indicated is
		''' returned represented as the distance, measured in milliseconds,
		''' of that time from the epoch (00:00:00 GMT on January 1, 1970).
		''' </summary>
		''' <param name="year">    the year minus 1900. </param>
		''' <param name="month">   the month between 0-11. </param>
		''' <param name="date">    the day of the month between 1-31. </param>
		''' <param name="hrs">     the hours between 0-23. </param>
		''' <param name="min">     the minutes between 0-59. </param>
		''' <param name="sec">     the seconds between 0-59. </param>
		''' <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT for
		'''          the date and time specified by the arguments. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.set(year + 1900, month, date,
		''' hrs, min, sec)</code> or <code>GregorianCalendar(year + 1900,
		''' month, date, hrs, min, sec)</code>, using a UTC
		''' <code>TimeZone</code>, followed by <code>Calendar.getTime().getTime()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Shared Function UTC(ByVal year As Integer, ByVal month As Integer, ByVal [date] As Integer, ByVal hrs As Integer, ByVal min As Integer, ByVal sec As Integer) As Long
			Dim y As Integer = year + 1900
			' month is 0-based. So we have to normalize month to support java.lang.[Long].MAX_VALUE.
			If month >= 12 Then
				y += month \ 12
				month = month Mod 12
			ElseIf month < 0 Then
				y += sun.util.calendar.CalendarUtils.floorDivide(month, 12)
				month = sun.util.calendar.CalendarUtils.mod(month, 12)
			End If
			Dim m As Integer = month + 1
			Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(y)
			Dim udate As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(Nothing), sun.util.calendar.BaseCalendar.Date)
			udate.normalizedDateate(y, m, date_Renamed).setTimeOfDay(hrs, min, sec, 0)

			' Use a Date instance to perform normalization. Its fastTime
			' is the UTC value after the normalization.
			Dim d As New DateTime
			d.normalize(udate)
			Return d.fastTime
		End Function

		''' <summary>
		''' Attempts to interpret the string <tt>s</tt> as a representation
		''' of a date and time. If the attempt is successful, the time
		''' indicated is returned represented as the distance, measured in
		''' milliseconds, of that time from the epoch (00:00:00 GMT on
		''' January 1, 1970). If the attempt fails, an
		''' <tt>IllegalArgumentException</tt> is thrown.
		''' <p>
		''' It accepts many syntaxes; in particular, it recognizes the IETF
		''' standard date syntax: "Sat, 12 Aug 1995 13:30:00 GMT". It also
		''' understands the continental U.S. time-zone abbreviations, but for
		''' general use, a time-zone offset should be used: "Sat, 12 Aug 1995
		''' 13:30:00 GMT+0430" (4 hours, 30 minutes west of the Greenwich
		''' meridian). If no time zone is specified, the local time zone is
		''' assumed. GMT and UTC are considered equivalent.
		''' <p>
		''' The string <tt>s</tt> is processed from left to right, looking for
		''' data of interest. Any material in <tt>s</tt> that is within the
		''' ASCII parenthesis characters <tt>(</tt> and <tt>)</tt> is ignored.
		''' Parentheses may be nested. Otherwise, the only characters permitted
		''' within <tt>s</tt> are these ASCII characters:
		''' <blockquote><pre>
		''' abcdefghijklmnopqrstuvwxyz
		''' ABCDEFGHIJKLMNOPQRSTUVWXYZ
		''' 0123456789,+-:/</pre></blockquote>
		''' and whitespace characters.<p>
		''' A consecutive sequence of decimal digits is treated as a decimal
		''' number:<ul>
		''' <li>If a number is preceded by <tt>+</tt> or <tt>-</tt> and a year
		'''     has already been recognized, then the number is a time-zone
		'''     offset. If the number is less than 24, it is an offset measured
		'''     in hours. Otherwise, it is regarded as an offset in minutes,
		'''     expressed in 24-hour time format without punctuation. A
		'''     preceding <tt>-</tt> means a westward offset. Time zone offsets
		'''     are always relative to UTC (Greenwich). Thus, for example,
		'''     <tt>-5</tt> occurring in the string would mean "five hours west
		'''     of Greenwich" and <tt>+0430</tt> would mean "four hours and
		'''     thirty minutes east of Greenwich." It is permitted for the
		'''     string to specify <tt>GMT</tt>, <tt>UT</tt>, or <tt>UTC</tt>
		'''     redundantly-for example, <tt>GMT-5</tt> or <tt>utc+0430</tt>.
		''' <li>The number is regarded as a year number if one of the
		'''     following conditions is true:
		''' <ul>
		'''     <li>The number is equal to or greater than 70 and followed by a
		'''         space, comma, slash, or end of string
		'''     <li>The number is less than 70, and both a month and a day of
		'''         the month have already been recognized</li>
		''' </ul>
		'''     If the recognized year number is less than 100, it is
		'''     interpreted as an abbreviated year relative to a century of
		'''     which dates are within 80 years before and 19 years after
		'''     the time when the Date class is initialized.
		'''     After adjusting the year number, 1900 is subtracted from
		'''     it. For example, if the current year is 1999 then years in
		'''     the range 19 to 99 are assumed to mean 1919 to 1999, while
		'''     years from 0 to 18 are assumed to mean 2000 to 2018.  Note
		'''     that this is slightly different from the interpretation of
		'''     years less than 100 that is used in <seealso cref="java.text.SimpleDateFormat"/>.
		''' <li>If the number is followed by a colon, it is regarded as an hour,
		'''     unless an hour has already been recognized, in which case it is
		'''     regarded as a minute.
		''' <li>If the number is followed by a slash, it is regarded as a month
		'''     (it is decreased by 1 to produce a number in the range <tt>0</tt>
		'''     to <tt>11</tt>), unless a month has already been recognized, in
		'''     which case it is regarded as a day of the month.
		''' <li>If the number is followed by whitespace, a comma, a hyphen, or
		'''     end of string, then if an hour has been recognized but not a
		'''     minute, it is regarded as a minute; otherwise, if a minute has
		'''     been recognized but not a second, it is regarded as a second;
		'''     otherwise, it is regarded as a day of the month. </ul><p>
		''' A consecutive sequence of letters is regarded as a word and treated
		''' as follows:<ul>
		''' <li>A word that matches <tt>AM</tt>, ignoring case, is ignored (but
		'''     the parse fails if an hour has not been recognized or is less
		'''     than <tt>1</tt> or greater than <tt>12</tt>).
		''' <li>A word that matches <tt>PM</tt>, ignoring case, adds <tt>12</tt>
		'''     to the hour (but the parse fails if an hour has not been
		'''     recognized or is less than <tt>1</tt> or greater than <tt>12</tt>).
		''' <li>Any word that matches any prefix of <tt>SUNDAY, MONDAY, TUESDAY,
		'''     WEDNESDAY, THURSDAY, FRIDAY</tt>, or <tt>SATURDAY</tt>, ignoring
		'''     case, is ignored. For example, <tt>sat, Friday, TUE</tt>, and
		'''     <tt>Thurs</tt> are ignored.
		''' <li>Otherwise, any word that matches any prefix of <tt>JANUARY,
		'''     FEBRUARY, MARCH, APRIL, MAY, JUNE, JULY, AUGUST, SEPTEMBER,
		'''     OCTOBER, NOVEMBER</tt>, or <tt>DECEMBER</tt>, ignoring case, and
		'''     considering them in the order given here, is recognized as
		'''     specifying a month and is converted to a number (<tt>0</tt> to
		'''     <tt>11</tt>). For example, <tt>aug, Sept, april</tt>, and
		'''     <tt>NOV</tt> are recognized as months. So is <tt>Ma</tt>, which
		'''     is recognized as <tt>MARCH</tt>, not <tt>MAY</tt>.
		''' <li>Any word that matches <tt>GMT, UT</tt>, or <tt>UTC</tt>, ignoring
		'''     case, is treated as referring to UTC.
		''' <li>Any word that matches <tt>EST, CST, MST</tt>, or <tt>PST</tt>,
		'''     ignoring case, is recognized as referring to the time zone in
		'''     North America that is five, six, seven, or eight hours west of
		'''     Greenwich, respectively. Any word that matches <tt>EDT, CDT,
		'''     MDT</tt>, or <tt>PDT</tt>, ignoring case, is recognized as
		'''     referring to the same time zone, respectively, during daylight
		'''     saving time.</ul><p>
		''' Once the entire string s has been scanned, it is converted to a time
		''' result in one of two ways. If a time zone or time-zone offset has been
		''' recognized, then the year, month, day of month, hour, minute, and
		''' second are interpreted in UTC and then the time-zone offset is
		''' applied. Otherwise, the year, month, day of month, hour, minute, and
		''' second are interpreted in the local time zone.
		''' </summary>
		''' <param name="s">   a string to be parsed as a date. </param>
		''' <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT
		'''          represented by the string argument. </returns>
		''' <seealso cref=     java.text.DateFormat </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>DateFormat.parse(String s)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Shared Function parse(ByVal s As String) As Long
			Dim year_Renamed As Integer =  java.lang.[Integer].MIN_VALUE
			Dim mon As Integer = -1
			Dim mday As Integer = -1
			Dim hour As Integer = -1
			Dim min As Integer = -1
			Dim sec As Integer = -1
			Dim millis As Integer = -1
			Dim c As Integer = -1
			Dim i As Integer = 0
			Dim n As Integer = -1
			Dim wst As Integer = -1
			Dim tzoffset As Integer = -1
			Dim prevc As Integer = 0
		syntax:
				If s Is Nothing Then GoTo syntax
				Dim limit As Integer = s.length()
				Do While i < limit
					c = AscW(s.Chars(i))
					i += 1
					If c <= " "c OrElse c = AscW(","c) Then Continue Do
					If c = AscW("("c) Then ' skip comments
						Dim depth As Integer = 1
						Do While i < limit
							c = AscW(s.Chars(i))
							i += 1
							If c = AscW("("c) Then
								depth += 1
							ElseIf c = AscW(")"c) Then
								depth -= 1
								If depth <= 0 Then Exit Do
							End If
						Loop
						Continue Do
					End If
					If "0"c <= c AndAlso c <= "9"c Then
						n = c - AscW("0"c)
						c = AscW(s.Chars(i))
						Do While i < limit AndAlso "0"c <= c AndAlso c <= "9"c
							n = n * 10 + c - AscW("0"c)
							i += 1
							c = AscW(s.Chars(i))
						Loop
						If prevc = AscW("+"c) OrElse prevc = AscW("-"c) AndAlso year_Renamed <>  java.lang.[Integer].MIN_VALUE Then
							' timezone offset
							If n < 24 Then
								n = n * 60 ' EG. "GMT-3"
							Else
								n = n Mod 100 + n \ 100 * 60 ' eg "GMT-0430"
							End If
							If prevc = AscW("+"c) Then ' plus means east of GMT n = -n
							If tzoffset <> 0 AndAlso tzoffset <> -1 Then GoTo syntax
							tzoffset = n
						ElseIf n >= 70 Then
							If year_Renamed <>  java.lang.[Integer].MIN_VALUE Then
								GoTo syntax
							ElseIf c <= " "c OrElse c = AscW(","c) OrElse c = AscW("/"c) OrElse i >= limit Then
								' year = n < 1900 ? n : n - 1900;
								year_Renamed = n
							Else
								GoTo syntax
							End If
						ElseIf c = AscW(":"c) Then
							If hour < 0 Then
								hour = CByte(n)
							ElseIf min < 0 Then
								min = CByte(n)
							Else
								GoTo syntax
							End If
						ElseIf c = AscW("/"c) Then
							If mon < 0 Then
								mon = CByte(n - 1)
							ElseIf mday < 0 Then
								mday = CByte(n)
							Else
								GoTo syntax
							End If
						ElseIf i < limit AndAlso c <> AscW(","c) AndAlso c > AscW(" "c) AndAlso c <> AscW("-"c) Then
							GoTo syntax
						ElseIf hour >= 0 AndAlso min < 0 Then
							min = CByte(n)
						ElseIf min >= 0 AndAlso sec < 0 Then
							sec = CByte(n)
						ElseIf mday < 0 Then
							mday = CByte(n)
						' Handle two-digit years < 70 (70-99 handled above).
						ElseIf year_Renamed =  java.lang.[Integer].MIN_VALUE AndAlso mon >= 0 AndAlso mday >= 0 Then
							year_Renamed = n
						Else
							GoTo syntax
						End If
						prevc = 0
					ElseIf c = AscW("/"c) OrElse c = AscW(":"c) OrElse c = AscW("+"c) OrElse c = AscW("-"c) Then
						prevc = c
					Else
						Dim st As Integer = i - 1
						Do While i < limit
							c = AscW(s.Chars(i))
							If Not("A"c <= c AndAlso c <= "Z"c OrElse "a"c <= c AndAlso c <= "z"c) Then Exit Do
							i += 1
						Loop
						If i <= st + 1 Then GoTo syntax
						Dim k As Integer
						k = wtb.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Do While k -= 1 >= 0
							If wtb(k).regionMatches(True, 0, s, st, i - st) Then
								Dim action As Integer = ttb(k)
								If action <> 0 Then
									If action = 1 Then ' pm
										If hour > 12 OrElse hour < 1 Then
											GoTo syntax
										ElseIf hour < 12 Then
											hour += 12
										End If ' am
									ElseIf action = 14 Then
										If hour > 12 OrElse hour < 1 Then
											GoTo syntax
										ElseIf hour = 12 Then
											hour = 0
										End If ' month!
									ElseIf action <= 13 Then
										If mon < 0 Then
											mon = CByte(action - 2)
										Else
											GoTo syntax
										End If
									Else
										tzoffset = action - 10000
									End If
								End If
								Exit Do
							End If
						Loop
						If k < 0 Then GoTo syntax
						prevc = 0
					End If
				Loop
				If year_Renamed =  java.lang.[Integer].MIN_VALUE OrElse mon < 0 OrElse mday < 0 Then GoTo syntax
				' Parse 2-digit years within the correct default century.
				If year_Renamed < 100 Then
					SyncLock GetType(Date)
						If defaultCenturyStart = 0 Then defaultCenturyStart = gcal.calendarDate.year - 80
					End SyncLock
					year_Renamed += (defaultCenturyStart \ 100) * 100
					If year_Renamed < defaultCenturyStart Then year_Renamed += 100
				End If
				If sec < 0 Then sec = 0
				If min < 0 Then min = 0
				If hour < 0 Then hour = 0
				Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(year_Renamed)
				If tzoffset = -1 Then ' no time zone specified, have to use local
					Dim ldate As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(TimeZone.defaultRef), sun.util.calendar.BaseCalendar.Date)
					ldate.dateate(year_Renamed, mon + 1, mday)
					ldate.timeOfDayDay(hour, min, sec, 0)
					Return cal.getTime(ldate)
				End If
				Dim udate As sun.util.calendar.BaseCalendar.Date = CType(cal.newCalendarDate(Nothing), sun.util.calendar.BaseCalendar.Date) ' no time zone
				udate.dateate(year_Renamed, mon + 1, mday)
				udate.timeOfDayDay(hour, min, sec, 0)
				Return cal.getTime(udate) + tzoffset * (60 * 1000)
			' syntax error
			Throw New IllegalArgumentException
		End Function
		Private Shared ReadOnly wtb As String() = { "am", "pm", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday", "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december", "gmt", "ut", "utc", "est", "edt", "cst", "cdt", "mst", "mdt", "pst", "pdt" }
		Private Shared ReadOnly ttb As Integer() = { 14, 1, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 10000 + 0, 10000 + 0, 10000 + 0, 10000 + 5 * 60, 10000 + 4 * 60, 10000 + 6 * 60, 10000 + 5 * 60, 10000 + 7 * 60, 10000 + 6 * 60, 10000 + 8 * 60, 10000 + 7 * 60 }

		''' <summary>
		''' Returns a value that is the result of subtracting 1900 from the
		''' year that contains or begins with the instant in time represented
		''' by this <code>Date</code> object, as interpreted in the local
		''' time zone.
		''' </summary>
		''' <returns>  the year represented by this date, minus 1900. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.YEAR) - 1900</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property year As Integer
			Get
				Return normalize().year - 1900
			End Get
			Set(ByVal year As Integer)
				calendarDate.normalizedYear = year + 1900
			End Set
		End Property


		''' <summary>
		''' Returns a number representing the month that contains or begins
		''' with the instant in time represented by this <tt>Date</tt> object.
		''' The value returned is between <code>0</code> and <code>11</code>,
		''' with the value <code>0</code> representing January.
		''' </summary>
		''' <returns>  the month represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.MONTH)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property month As Integer
			Get
				Return normalize().month - 1 ' adjust 1-based to 0-based
			End Get
			Set(ByVal month As Integer)
				Dim y As Integer = 0
				If month >= 12 Then
					y = month \ 12
					month = month Mod 12
				ElseIf month < 0 Then
					y = sun.util.calendar.CalendarUtils.floorDivide(month, 12)
					month = sun.util.calendar.CalendarUtils.mod(month, 12)
				End If
				Dim d As sun.util.calendar.BaseCalendar.Date = calendarDate
				If y <> 0 Then d.normalizedYear = d.normalizedYear + y
				d.month = month + 1 ' adjust 0-based to 1-based month numbering
			End Set
		End Property


		''' <summary>
		''' Returns the day of the month represented by this <tt>Date</tt> object.
		''' The value returned is between <code>1</code> and <code>31</code>
		''' representing the day of the month that contains or begins with the
		''' instant in time represented by this <tt>Date</tt> object, as
		''' interpreted in the local time zone.
		''' </summary>
		''' <returns>  the day of the month represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.DAY_OF_MONTH)</code>.
		''' @deprecated 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property [date] As Integer
			Get
				Return normalize().dayOfMonth
			End Get
			Set(ByVal [date] As Integer)
				calendarDate.dayOfMonth = date_Renamed
			End Set
		End Property


		''' <summary>
		''' Returns the day of the week represented by this date. The
		''' returned value (<tt>0</tt> = Sunday, <tt>1</tt> = Monday,
		''' <tt>2</tt> = Tuesday, <tt>3</tt> = Wednesday, <tt>4</tt> =
		''' Thursday, <tt>5</tt> = Friday, <tt>6</tt> = Saturday)
		''' represents the day of the week that contains or begins with
		''' the instant in time represented by this <tt>Date</tt> object,
		''' as interpreted in the local time zone.
		''' </summary>
		''' <returns>  the day of the week represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.DAY_OF_WEEK)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property day As Integer
			Get
				Return normalize().dayOfWeek - sun.util.calendar.BaseCalendar.SUNDAY
			End Get
		End Property

		''' <summary>
		''' Returns the hour represented by this <tt>Date</tt> object. The
		''' returned value is a number (<tt>0</tt> through <tt>23</tt>)
		''' representing the hour within the day that contains or begins
		''' with the instant in time represented by this <tt>Date</tt>
		''' object, as interpreted in the local time zone.
		''' </summary>
		''' <returns>  the hour represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.HOUR_OF_DAY)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property hours As Integer
			Get
				Return normalize().hours
			End Get
			Set(ByVal hours As Integer)
				calendarDate.hours = hours
			End Set
		End Property


		''' <summary>
		''' Returns the number of minutes past the hour represented by this date,
		''' as interpreted in the local time zone.
		''' The value returned is between <code>0</code> and <code>59</code>.
		''' </summary>
		''' <returns>  the number of minutes past the hour represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.MINUTE)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property minutes As Integer
			Get
				Return normalize().minutes
			End Get
			Set(ByVal minutes As Integer)
				calendarDate.minutes = minutes
			End Set
		End Property


		''' <summary>
		''' Returns the number of seconds past the minute represented by this date.
		''' The value returned is between <code>0</code> and <code>61</code>. The
		''' values <code>60</code> and <code>61</code> can only occur on those
		''' Java Virtual Machines that take leap seconds into account.
		''' </summary>
		''' <returns>  the number of seconds past the minute represented by this date. </returns>
		''' <seealso cref=     java.util.Calendar </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Calendar.get(Calendar.SECOND)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property seconds As Integer
			Get
				Return normalize().seconds
			End Get
			Set(ByVal seconds As Integer)
				calendarDate.seconds = seconds
			End Set
		End Property


		''' <summary>
		''' Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT
		''' represented by this <tt>Date</tt> object.
		''' </summary>
		''' <returns>  the number of milliseconds since January 1, 1970, 00:00:00 GMT
		'''          represented by this date. </returns>
		Public Overridable Property time As Long
			Get
				Return timeImpl
			End Get
			Set(ByVal time As Long)
				fastTime = time
				[cdate] = Nothing
			End Set
		End Property

		Private Property timeImpl As Long
			Get
				If [cdate] IsNot Nothing AndAlso (Not [cdate].normalized) Then normalize()
				Return fastTime
			End Get
		End Property


		''' <summary>
		''' Tests if this date is before the specified date.
		''' </summary>
		''' <param name="when">   a date. </param>
		''' <returns>  <code>true</code> if and only if the instant of time
		'''            represented by this <tt>Date</tt> object is strictly
		'''            earlier than the instant represented by <tt>when</tt>;
		'''          <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException"> if <code>when</code> is null. </exception>
		Public Overridable Function before(ByVal [when] As Date) As Boolean
			Return getMillisOf(Me) < getMillisOf([when])
		End Function

		''' <summary>
		''' Tests if this date is after the specified date.
		''' </summary>
		''' <param name="when">   a date. </param>
		''' <returns>  <code>true</code> if and only if the instant represented
		'''          by this <tt>Date</tt> object is strictly later than the
		'''          instant represented by <tt>when</tt>;
		'''          <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException"> if <code>when</code> is null. </exception>
		Public Overridable Function after(ByVal [when] As Date) As Boolean
			Return getMillisOf(Me) > getMillisOf([when])
		End Function

		''' <summary>
		''' Compares two dates for equality.
		''' The result is <code>true</code> if and only if the argument is
		''' not <code>null</code> and is a <code>Date</code> object that
		''' represents the same point in time, to the millisecond, as this object.
		''' <p>
		''' Thus, two <code>Date</code> objects are equal if and only if the
		''' <code>getTime</code> method returns the same <code>long</code>
		''' value for both.
		''' </summary>
		''' <param name="obj">   the object to compare with. </param>
		''' <returns>  <code>true</code> if the objects are the same;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.util.Date#getTime() </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return TypeOf obj Is Date AndAlso time = CDate(obj).time
		End Function

		''' <summary>
		''' Returns the millisecond value of this <code>Date</code> object
		''' without affecting its internal state.
		''' </summary>
		Shared Function getMillisOf(ByVal [date] As Date) As Long
			If date_Renamed.cdate Is Nothing OrElse date_Renamed.cdate.normalized Then Return date_Renamed.fastTime
			Dim d As sun.util.calendar.BaseCalendar.Date = CType(date_Renamed.cdate.clone(), sun.util.calendar.BaseCalendar.Date)
			Return gcal.getTime(d)
		End Function

		''' <summary>
		''' Compares two Dates for ordering.
		''' </summary>
		''' <param name="anotherDate">   the <code>Date</code> to be compared. </param>
		''' <returns>  the value <code>0</code> if the argument Date is equal to
		'''          this Date; a value less than <code>0</code> if this Date
		'''          is before the Date argument; and a value greater than
		'''      <code>0</code> if this Date is after the Date argument.
		''' @since   1.2 </returns>
		''' <exception cref="NullPointerException"> if <code>anotherDate</code> is null. </exception>
		Public Overridable Function compareTo(ByVal anotherDate As Date) As Integer Implements Comparable(Of Date).compareTo
			Dim thisTime As Long = getMillisOf(Me)
			Dim anotherTime As Long = getMillisOf(anotherDate)
			Return (If(thisTime<anotherTime, -1, (If(thisTime=anotherTime, 0, 1))))
		End Function

		''' <summary>
		''' Returns a hash code value for this object. The result is the
		''' exclusive OR of the two halves of the primitive <tt>long</tt>
		''' value returned by the <seealso cref="Date#getTime"/>
		''' method. That is, the hash code is the value of the expression:
		''' <blockquote><pre>{@code
		''' (int)(this.getTime()^(this.getTime() >>> 32))
		''' }</pre></blockquote>
		''' </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim ht As Long = Me.time
			Return CInt(ht) Xor CInt(Fix(ht >> 32))
		End Function

		''' <summary>
		''' Converts this <code>Date</code> object to a <code>String</code>
		''' of the form:
		''' <blockquote><pre>
		''' dow mon dd hh:mm:ss zzz yyyy</pre></blockquote>
		''' where:<ul>
		''' <li><tt>dow</tt> is the day of the week (<tt>Sun, Mon, Tue, Wed,
		'''     Thu, Fri, Sat</tt>).
		''' <li><tt>mon</tt> is the month (<tt>Jan, Feb, Mar, Apr, May, Jun,
		'''     Jul, Aug, Sep, Oct, Nov, Dec</tt>).
		''' <li><tt>dd</tt> is the day of the month (<tt>01</tt> through
		'''     <tt>31</tt>), as two decimal digits.
		''' <li><tt>hh</tt> is the hour of the day (<tt>00</tt> through
		'''     <tt>23</tt>), as two decimal digits.
		''' <li><tt>mm</tt> is the minute within the hour (<tt>00</tt> through
		'''     <tt>59</tt>), as two decimal digits.
		''' <li><tt>ss</tt> is the second within the minute (<tt>00</tt> through
		'''     <tt>61</tt>, as two decimal digits.
		''' <li><tt>zzz</tt> is the time zone (and may reflect daylight saving
		'''     time). Standard time zone abbreviations include those
		'''     recognized by the method <tt>parse</tt>. If time zone
		'''     information is not available, then <tt>zzz</tt> is empty -
		'''     that is, it consists of no characters at all.
		''' <li><tt>yyyy</tt> is the year, as four decimal digits.
		''' </ul>
		''' </summary>
		''' <returns>  a string representation of this date. </returns>
		''' <seealso cref=     java.util.Date#toLocaleString() </seealso>
		''' <seealso cref=     java.util.Date#toGMTString() </seealso>
		Public Overrides Function ToString() As String
			' "EEE MMM dd HH:mm:ss zzz yyyy";
			Dim date_Renamed As sun.util.calendar.BaseCalendar.Date = normalize()
			Dim sb As New StringBuilder(28)
			Dim index As Integer = date_Renamed.dayOfWeek
			If index = sun.util.calendar.BaseCalendar.SUNDAY Then index = 8
			convertToAbbr(sb, wtb(index)).append(" "c) ' EEE
			convertToAbbr(sb, wtb(date_Renamed.month - 1 + 2 + 7)).append(" "c) ' MMM
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.dayOfMonth, 2).append(" "c) ' dd

			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.hours, 2).append(":"c) ' HH
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.minutes, 2).append(":"c) ' mm
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.seconds, 2).append(" "c) ' ss
			Dim zi As TimeZone = date_Renamed.zone
			If zi IsNot Nothing Then
				sb.append(zi.getDisplayName(date_Renamed.daylightTime, TimeZone.SHORT, Locale.US)) ' zzz
			Else
				sb.append("GMT")
			End If
			sb.append(" "c).append(date_Renamed.year) ' yyyy
			Return sb.ToString()
		End Function

		''' <summary>
		''' Converts the given name to its 3-letter abbreviation (e.g.,
		''' "monday" -> "Mon") and stored the abbreviation in the given
		''' <code>StringBuilder</code>.
		''' </summary>
		Private Shared Function convertToAbbr(ByVal sb As StringBuilder, ByVal name As String) As StringBuilder
			sb.append(Char.ToUpper(name.Chars(0)))
			sb.append(name.Chars(1)).append(name.Chars(2))
			Return sb
		End Function

		''' <summary>
		''' Creates a string representation of this <tt>Date</tt> object in an
		''' implementation-dependent form. The intent is that the form should
		''' be familiar to the user of the Java application, wherever it may
		''' happen to be running. The intent is comparable to that of the
		''' "<code>%c</code>" format supported by the <code>strftime()</code>
		''' function of ISO&nbsp;C.
		''' </summary>
		''' <returns>  a string representation of this date, using the locale
		'''          conventions. </returns>
		''' <seealso cref=     java.text.DateFormat </seealso>
		''' <seealso cref=     java.util.Date#toString() </seealso>
		''' <seealso cref=     java.util.Date#toGMTString() </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>DateFormat.format(Date date)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function toLocaleString() As String
			Dim formatter As java.text.DateFormat = java.text.DateFormat.dateTimeInstance
			Return formatter.format(Me)
		End Function

		''' <summary>
		''' Creates a string representation of this <tt>Date</tt> object of
		''' the form:
		''' <blockquote><pre>
		''' d mon yyyy hh:mm:ss GMT</pre></blockquote>
		''' where:<ul>
		''' <li><i>d</i> is the day of the month (<tt>1</tt> through <tt>31</tt>),
		'''     as one or two decimal digits.
		''' <li><i>mon</i> is the month (<tt>Jan, Feb, Mar, Apr, May, Jun, Jul,
		'''     Aug, Sep, Oct, Nov, Dec</tt>).
		''' <li><i>yyyy</i> is the year, as four decimal digits.
		''' <li><i>hh</i> is the hour of the day (<tt>00</tt> through <tt>23</tt>),
		'''     as two decimal digits.
		''' <li><i>mm</i> is the minute within the hour (<tt>00</tt> through
		'''     <tt>59</tt>), as two decimal digits.
		''' <li><i>ss</i> is the second within the minute (<tt>00</tt> through
		'''     <tt>61</tt>), as two decimal digits.
		''' <li><i>GMT</i> is exactly the ASCII letters "<tt>GMT</tt>" to indicate
		'''     Greenwich Mean Time.
		''' </ul><p>
		''' The result does not depend on the local time zone.
		''' </summary>
		''' <returns>  a string representation of this date, using the Internet GMT
		'''          conventions. </returns>
		''' <seealso cref=     java.text.DateFormat </seealso>
		''' <seealso cref=     java.util.Date#toString() </seealso>
		''' <seealso cref=     java.util.Date#toLocaleString() </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>DateFormat.format(Date date)</code>, using a
		''' GMT <code>TimeZone</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function toGMTString() As String
			' d MMM yyyy HH:mm:ss 'GMT'
			Dim t As Long = time
			Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(t)
			Dim date_Renamed As sun.util.calendar.BaseCalendar.Date = CType(cal.getCalendarDate(time, CType(Nothing, TimeZone)), sun.util.calendar.BaseCalendar.Date)
			Dim sb As New StringBuilder(32)
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.dayOfMonth, 1).append(" "c) ' d
			convertToAbbr(sb, wtb(date_Renamed.month - 1 + 2 + 7)).append(" "c) ' MMM
			sb.append(date_Renamed.year).append(" "c) ' yyyy
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.hours, 2).append(":"c) ' HH
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.minutes, 2).append(":"c) ' mm
			sun.util.calendar.CalendarUtils.sprintf0d(sb, date_Renamed.seconds, 2) ' ss
			sb.append(" GMT") ' ' GMT'
			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the offset, measured in minutes, for the local time zone
		''' relative to UTC that is appropriate for the time represented by
		''' this <code>Date</code> object.
		''' <p>
		''' For example, in Massachusetts, five time zones west of Greenwich:
		''' <blockquote><pre>
		''' new Date(96, 1, 14).getTimezoneOffset() returns 300</pre></blockquote>
		''' because on February 14, 1996, standard time (Eastern Standard Time)
		''' is in use, which is offset five hours from UTC; but:
		''' <blockquote><pre>
		''' new Date(96, 5, 1).getTimezoneOffset() returns 240</pre></blockquote>
		''' because on June 1, 1996, daylight saving time (Eastern Daylight Time)
		''' is in use, which is offset only four hours from UTC.<p>
		''' This method produces the same result as if it computed:
		''' <blockquote><pre>
		''' (this.getTime() - UTC(this.getYear(),
		'''                       this.getMonth(),
		'''                       this.getDate(),
		'''                       this.getHours(),
		'''                       this.getMinutes(),
		'''                       this.getSeconds())) / (60 * 1000)
		''' </pre></blockquote>
		''' </summary>
		''' <returns>  the time-zone offset, in minutes, for the current time zone. </returns>
		''' <seealso cref=     java.util.Calendar#ZONE_OFFSET </seealso>
		''' <seealso cref=     java.util.Calendar#DST_OFFSET </seealso>
		''' <seealso cref=     java.util.TimeZone#getDefault </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>-(Calendar.get(Calendar.ZONE_OFFSET) +
		''' Calendar.get(Calendar.DST_OFFSET)) / (60 * 1000)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property timezoneOffset As Integer
			Get
				Dim zoneOffset As Integer
				If [cdate] Is Nothing Then
					Dim tz As TimeZone = TimeZone.defaultRef
					If TypeOf tz Is sun.util.calendar.ZoneInfo Then
						zoneOffset = CType(tz, sun.util.calendar.ZoneInfo).getOffsets(fastTime, Nothing)
					Else
						zoneOffset = tz.getOffset(fastTime)
					End If
				Else
					normalize()
					zoneOffset = [cdate].zoneOffset
				End If
				Return -zoneOffset\60000 ' convert to minutes
			End Get
		End Property

		Private Property calendarDate As sun.util.calendar.BaseCalendar.Date
			Get
				If [cdate] Is Nothing Then
					Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(fastTime)
					[cdate] = CType(cal.getCalendarDate(fastTime, TimeZone.defaultRef), sun.util.calendar.BaseCalendar.Date)
				End If
				Return [cdate]
			End Get
		End Property

		Private Function normalize() As sun.util.calendar.BaseCalendar.Date
			If [cdate] Is Nothing Then
				Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(fastTime)
				[cdate] = CType(cal.getCalendarDate(fastTime, TimeZone.defaultRef), sun.util.calendar.BaseCalendar.Date)
				Return [cdate]
			End If

			' Normalize cdate with the TimeZone in cdate first. This is
			' required for the compatible behavior.
			If Not [cdate].normalized Then [cdate] = normalize([cdate])

			' If the default TimeZone has changed, then recalculate the
			' fields with the new TimeZone.
			Dim tz As TimeZone = TimeZone.defaultRef
			If tz IsNot [cdate].zone Then
				[cdate].zone = tz
				Dim cal As sun.util.calendar.CalendarSystem = getCalendarSystem([cdate])
				cal.getCalendarDate(fastTime, [cdate])
			End If
			Return [cdate]
		End Function

		' fastTime and the returned data are in sync upon return.
		Private Function normalize(ByVal [date] As sun.util.calendar.BaseCalendar.Date) As sun.util.calendar.BaseCalendar.Date
			Dim y As Integer = date_Renamed.normalizedYear
			Dim m As Integer = date_Renamed.month
			Dim d As Integer = date_Renamed.dayOfMonth
			Dim hh As Integer = date_Renamed.hours
			Dim mm As Integer = date_Renamed.minutes
			Dim ss As Integer = date_Renamed.seconds
			Dim ms As Integer = date_Renamed.millis
			Dim tz As TimeZone = date_Renamed.zone

			' If the specified year can't be handled using a long value
			' in milliseconds, GregorianCalendar is used for full
			' compatibility with underflow and overflow. This is required
			' by some JCK tests. The limits are based max year values -
			' years that can be represented by max values of d, hh, mm,
			' ss and ms. Also, let GregorianCalendar handle the default
			' cutover year so that we don't need to worry about the
			' transition here.
			If y = 1582 OrElse y > 280000000 OrElse y < -280000000 Then
				If tz Is Nothing Then tz = TimeZone.getTimeZone("GMT")
				Dim gc As New GregorianCalendar(tz)
				gc.clear()
				gc.set(GregorianCalendar.MILLISECOND, ms)
				gc.set(y, m-1, d, hh, mm, ss)
				fastTime = gc.timeInMillis
				Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(fastTime)
				date_Renamed = CType(cal.getCalendarDate(fastTime, tz), sun.util.calendar.BaseCalendar.Date)
				Return date_Renamed
			End If

			Dim cal As sun.util.calendar.BaseCalendar = getCalendarSystem(y)
			If cal IsNot getCalendarSystem(date_Renamed) Then
				date_Renamed = CType(cal.newCalendarDate(tz), sun.util.calendar.BaseCalendar.Date)
				date_Renamed.normalizedDateate(y, m, d).setTimeOfDay(hh, mm, ss, ms)
			End If
			' Perform the GregorianCalendar-style normalization.
			fastTime = cal.getTime(date_Renamed)

			' In case the normalized date requires the other calendar
			' system, we need to recalculate it using the other one.
			Dim ncal As sun.util.calendar.BaseCalendar = getCalendarSystem(fastTime)
			If ncal IsNot cal Then
				date_Renamed = CType(ncal.newCalendarDate(tz), sun.util.calendar.BaseCalendar.Date)
				date_Renamed.normalizedDateate(y, m, d).setTimeOfDay(hh, mm, ss, ms)
				fastTime = ncal.getTime(date_Renamed)
			End If
			Return date_Renamed
		End Function

		''' <summary>
		''' Returns the Gregorian or Julian calendar system to use with the
		''' given date. Use Gregorian from October 15, 1582.
		''' </summary>
		''' <param name="year"> normalized calendar year (not -1900) </param>
		''' <returns> the CalendarSystem to use for the specified date </returns>
		Private Shared Function getCalendarSystem(ByVal year As Integer) As sun.util.calendar.BaseCalendar
			If year >= 1582 Then Return gcal
			Return julianCalendar
		End Function

		Private Shared Function getCalendarSystem(ByVal utc As Long) As sun.util.calendar.BaseCalendar
			' Quickly check if the time stamp given by `utc' is the Epoch
			' or later. If it's before 1970, we convert the cutover to
			' local time to compare.
			If utc >= 0 OrElse utc >= GregorianCalendar.DEFAULT_GREGORIAN_CUTOVER - TimeZone.defaultRef.getOffset(utc) Then Return gcal
			Return julianCalendar
		End Function

		Private Shared Function getCalendarSystem(ByVal [cdate] As sun.util.calendar.BaseCalendar.Date) As sun.util.calendar.BaseCalendar
			If jcal Is Nothing Then Return gcal
			If [cdate].era IsNot Nothing Then Return jcal
			Return gcal
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		PrivateShared ReadOnly PropertyjulianCalendar As sun.util.calendar.BaseCalendar
			Get
				If jcal Is Nothing Then jcal = CType(sun.util.calendar.CalendarSystem.forName("julian"), sun.util.calendar.BaseCalendar)
				Return jcal
			End Get
		End Property

		''' <summary>
		''' Save the state of this object to a stream (i.e., serialize it).
		''' 
		''' @serialData The value returned by <code>getTime()</code>
		'''             is emitted (long).  This represents the offset from
		'''             January 1, 1970, 00:00:00 GMT in milliseconds.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.writeLong(timeImpl)
		End Sub

		''' <summary>
		''' Reconstitute this object from a stream (i.e., deserialize it).
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			fastTime = s.readLong()
		End Sub

		''' <summary>
		''' Obtains an instance of {@code Date} from an {@code Instant} object.
		''' <p>
		''' {@code Instant} uses a precision of nanoseconds, whereas {@code Date}
		''' uses a precision of milliseconds.  The conversion will trancate any
		''' excess precision information as though the amount in nanoseconds was
		''' subject to integer division by one million.
		''' <p>
		''' {@code Instant} can store points on the time-line further in the future
		''' and further in the past than {@code Date}. In this scenario, this method
		''' will throw an exception.
		''' </summary>
		''' <param name="instant">  the instant to convert </param>
		''' <returns> a {@code Date} representing the same point on the time-line as
		'''  the provided instant </returns>
		''' <exception cref="NullPointerException"> if {@code instant} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the instant is too large to
		'''  represent as a {@code Date}
		''' @since 1.8 </exception>
		Public Shared Function [from](ByVal instant As java.time.Instant) As Date
			Try
				Return New Date(instant.toEpochMilli())
			Catch ex As ArithmeticException
				Throw New IllegalArgumentException(ex)
			End Try
		End Function

		''' <summary>
		''' Converts this {@code Date} object to an {@code Instant}.
		''' <p>
		''' The conversion creates an {@code Instant} that represents the same
		''' point on the time-line as this {@code Date}.
		''' </summary>
		''' <returns> an instant representing the same point on the time-line as
		'''  this {@code Date} object
		''' @since 1.8 </returns>
		Public Overridable Function toInstant() As java.time.Instant
			Return java.time.Instant.ofEpochMilli(time)
		End Function
	End Class

End Namespace