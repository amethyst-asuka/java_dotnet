Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports java.lang

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
' * (C) Copyright Taligent, Inc. 1996 - All Rights Reserved
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

Namespace java.text


    ''' <summary>
    ''' <code>SimpleDateFormat</code> is a concrete class for formatting and
    ''' parsing dates in a locale-sensitive manner. It allows for formatting
    ''' (date &rarr; text), parsing (text &rarr; date), and normalization.
    ''' 
    ''' <p>
    ''' <code>SimpleDateFormat</code> allows you to start by choosing
    ''' any user-defined patterns for date-time formatting. However, you
    ''' are encouraged to create a date-time formatter with either
    ''' <code>getTimeInstance</code>, <code>getDateInstance</code>, or
    ''' <code>getDateTimeInstance</code> in <code>DateFormat</code>. Each
    ''' of these class methods can return a date/time formatter initialized
    ''' with a default format pattern. You may modify the format pattern
    ''' using the <code>applyPattern</code> methods as desired.
    ''' For more information on using these methods, see
    ''' <seealso cref="DateFormat"/>.
    ''' 
    ''' <h3>Date and Time Patterns</h3>
    ''' <p>
    ''' Date and time formats are specified by <em>date and time pattern</em>
    ''' strings.
    ''' Within date and time pattern strings, unquoted letters from
    ''' <code>'A'</code> to <code>'Z'</code> and from <code>'a'</code> to
    ''' <code>'z'</code> are interpreted as pattern letters representing the
    ''' components of a date or time string.
    ''' Text can be quoted using single quotes (<code>'</code>) to avoid
    ''' interpretation.
    ''' <code>"''"</code> represents a single quote.
    ''' All other characters are not interpreted; they're simply copied into the
    ''' output string during formatting or matched against the input string
    ''' during parsing.
    ''' <p>
    ''' The following pattern letters are defined (all other characters from
    ''' <code>'A'</code> to <code>'Z'</code> and from <code>'a'</code> to
    ''' <code>'z'</code> are reserved):
    ''' <blockquote>
    ''' <table border=0 cellspacing=3 cellpadding=0 summary="Chart shows pattern letters, date/time component, presentation, and examples.">
    '''     <tr style="background-color: rgb(204, 204, 255);">
    '''         <th align=left>Letter
    '''         <th align=left>Date or Time Component
    '''         <th align=left>Presentation
    '''         <th align=left>Examples
    '''     <tr>
    '''         <td><code>G</code>
    '''         <td>Era designator
    '''         <td><a href="#text">Text</a>
    '''         <td><code>AD</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>y</code>
    '''         <td>Year
    '''         <td><a href="#year">Year</a>
    '''         <td><code>1996</code>; <code>96</code>
    '''     <tr>
    '''         <td><code>Y</code>
    '''         <td>Week year
    '''         <td><a href="#year">Year</a>
    '''         <td><code>2009</code>; <code>09</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>M</code>
    '''         <td>Month in year (context sensitive)
    '''         <td><a href="#month">Month</a>
    '''         <td><code>July</code>; <code>Jul</code>; <code>07</code>
    '''     <tr>
    '''         <td><code>L</code>
    '''         <td>Month in year (standalone form)
    '''         <td><a href="#month">Month</a>
    '''         <td><code>July</code>; <code>Jul</code>; <code>07</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>w</code>
    '''         <td>Week in year
    '''         <td><a href="#number">Number</a>
    '''         <td><code>27</code>
    '''     <tr>
    '''         <td><code>W</code>
    '''         <td>Week in month
    '''         <td><a href="#number">Number</a>
    '''         <td><code>2</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>D</code>
    '''         <td>Day in year
    '''         <td><a href="#number">Number</a>
    '''         <td><code>189</code>
    '''     <tr>
    '''         <td><code>d</code>
    '''         <td>Day in month
    '''         <td><a href="#number">Number</a>
    '''         <td><code>10</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>F</code>
    '''         <td>Day of week in month
    '''         <td><a href="#number">Number</a>
    '''         <td><code>2</code>
    '''     <tr>
    '''         <td><code>E</code>
    '''         <td>Day name in week
    '''         <td><a href="#text">Text</a>
    '''         <td><code>Tuesday</code>; <code>Tue</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>u</code>
    '''         <td>Day number of week (1 = Monday, ..., 7 = Sunday)
    '''         <td><a href="#number">Number</a>
    '''         <td><code>1</code>
    '''     <tr>
    '''         <td><code>a</code>
    '''         <td>Am/pm marker
    '''         <td><a href="#text">Text</a>
    '''         <td><code>PM</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>H</code>
    '''         <td>Hour in day (0-23)
    '''         <td><a href="#number">Number</a>
    '''         <td><code>0</code>
    '''     <tr>
    '''         <td><code>k</code>
    '''         <td>Hour in day (1-24)
    '''         <td><a href="#number">Number</a>
    '''         <td><code>24</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>K</code>
    '''         <td>Hour in am/pm (0-11)
    '''         <td><a href="#number">Number</a>
    '''         <td><code>0</code>
    '''     <tr>
    '''         <td><code>h</code>
    '''         <td>Hour in am/pm (1-12)
    '''         <td><a href="#number">Number</a>
    '''         <td><code>12</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>m</code>
    '''         <td>Minute in hour
    '''         <td><a href="#number">Number</a>
    '''         <td><code>30</code>
    '''     <tr>
    '''         <td><code>s</code>
    '''         <td>Second in minute
    '''         <td><a href="#number">Number</a>
    '''         <td><code>55</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>S</code>
    '''         <td>Millisecond
    '''         <td><a href="#number">Number</a>
    '''         <td><code>978</code>
    '''     <tr>
    '''         <td><code>z</code>
    '''         <td>Time zone
    '''         <td><a href="#timezone">General time zone</a>
    '''         <td><code>Pacific Standard Time</code>; <code>PST</code>; <code>GMT-08:00</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>Z</code>
    '''         <td>Time zone
    '''         <td><a href="#rfc822timezone">RFC 822 time zone</a>
    '''         <td><code>-0800</code>
    '''     <tr>
    '''         <td><code>X</code>
    '''         <td>Time zone
    '''         <td><a href="#iso8601timezone">ISO 8601 time zone</a>
    '''         <td><code>-08</code>; <code>-0800</code>;  <code>-08:00</code>
    ''' </table>
    ''' </blockquote>
    ''' Pattern letters are usually repeated, as their number determines the
    ''' exact presentation:
    ''' <ul>
    ''' <li><strong><a name="text">Text:</a></strong>
    '''     For formatting, if the number of pattern letters is 4 or more,
    '''     the full form is used; otherwise a short or abbreviated form
    '''     is used if available.
    '''     For parsing, both forms are accepted, independent of the number
    '''     of pattern letters.<br><br></li>
    ''' <li><strong><a name="number">Number:</a></strong>
    '''     For formatting, the number of pattern letters is the minimum
    '''     number of digits, and shorter numbers are zero-padded to this amount.
    '''     For parsing, the number of pattern letters is ignored unless
    '''     it's needed to separate two adjacent fields.<br><br></li>
    ''' <li><strong><a name="year">Year:</a></strong>
    '''     If the formatter's <seealso cref="#getCalendar() Calendar"/> is the Gregorian
    '''     calendar, the following rules are applied.<br>
    '''     <ul>
    '''     <li>For formatting, if the number of pattern letters is 2, the year
    '''         is truncated to 2 digits; otherwise it is interpreted as a
    '''         <a href="#number">number</a>.
    '''     <li>For parsing, if the number of pattern letters is more than 2,
    '''         the year is interpreted literally, regardless of the number of
    '''         digits. So using the pattern "MM/dd/yyyy", "01/11/12" parses to
    '''         Jan 11, 12 A.D.
    '''     <li>For parsing with the abbreviated year pattern ("y" or "yy"),
    '''         <code>SimpleDateFormat</code> must interpret the abbreviated year
    '''         relative to some century.  It does this by adjusting dates to be
    '''         within 80 years before and 20 years after the time the <code>SimpleDateFormat</code>
    '''         instance is created. For example, using a pattern of "MM/dd/yy" and a
    '''         <code>SimpleDateFormat</code> instance created on Jan 1, 1997,  the string
    '''         "01/11/12" would be interpreted as Jan 11, 2012 while the string "05/04/64"
    '''         would be interpreted as May 4, 1964.
    '''         During parsing, only strings consisting of exactly two digits, as defined by
    '''         <seealso cref="Character#isDigit(char)"/>, will be parsed into the default century.
    '''         Any other numeric string, such as a one digit string, a three or more digit
    '''         string, or a two digit string that isn't all digits (for example, "-1"), is
    '''         interpreted literally.  So "01/02/3" or "01/02/003" are parsed, using the
    '''         same pattern, as Jan 2, 3 AD.  Likewise, "01/02/-3" is parsed as Jan 2, 4 BC.
    '''     </ul>
    '''     Otherwise, calendar system specific forms are applied.
    '''     For both formatting and parsing, if the number of pattern
    '''     letters is 4 or more, a calendar specific {@linkplain
    '''     Calendar#LONG long form} is used. Otherwise, a calendar
    '''     specific <seealso cref="Calendar#SHORT short or abbreviated form"/>
    '''     is used.<br>
    '''     <br>
    '''     If week year {@code 'Y'} is specified and the {@linkplain
    '''     #getCalendar() calendar} doesn't support any <a
    '''     href="../util/GregorianCalendar.html#week_year"> week
    '''     years</a>, the calendar year ({@code 'y'}) is used instead. The
    '''     support of week years can be tested with a call to {@link
    '''     DateFormat#getCalendar() getCalendar()}.{@link
    '''     java.util.Calendar#isWeekDateSupported()
    '''     isWeekDateSupported()}.<br><br></li>
    ''' <li><strong><a name="month">Month:</a></strong>
    '''     If the number of pattern letters is 3 or more, the month is
    '''     interpreted as <a href="#text">text</a>; otherwise,
    '''     it is interpreted as a <a href="#number">number</a>.<br>
    '''     <ul>
    '''     <li>Letter <em>M</em> produces context-sensitive month names, such as the
    '''         embedded form of names. If a {@code DateFormatSymbols} has been set
    '''         explicitly with constructor {@link #SimpleDateFormat(String,
    '''         DateFormatSymbols)} or method {@link
    '''         #setDateFormatSymbols(DateFormatSymbols)}, the month names given by
    '''         the {@code DateFormatSymbols} are used.</li>
    '''     <li>Letter <em>L</em> produces the standalone form of month names.</li>
    '''     </ul>
    '''     <br></li>
    ''' <li><strong><a name="timezone">General time zone:</a></strong>
    '''     Time zones are interpreted as <a href="#text">text</a> if they have
    '''     names. For time zones representing a GMT offset value, the
    '''     following syntax is used:
    '''     <pre>
    '''     <a name="GMTOffsetTimeZone"><i>GMTOffsetTimeZone:</i></a>
    '''             <code>GMT</code> <i>Sign</i> <i>Hours</i> <code>:</code> <i>Minutes</i>
    '''     <i>Sign:</i> one of
    '''             <code>+ -</code>
    '''     <i>Hours:</i>
    '''             <i>Digit</i>
    '''             <i>Digit</i> <i>Digit</i>
    '''     <i>Minutes:</i>
    '''             <i>Digit</i> <i>Digit</i>
    '''     <i>Digit:</i> one of
    '''             <code>0 1 2 3 4 5 6 7 8 9</code></pre>
    '''     <i>Hours</i> must be between 0 and 23, and <i>Minutes</i> must be between
    '''     00 and 59. The format is locale independent and digits must be taken
    '''     from the Basic Latin block of the Unicode standard.
    '''     <p>For parsing, <a href="#rfc822timezone">RFC 822 time zones</a> are also
    '''     accepted.<br><br></li>
    ''' <li><strong><a name="rfc822timezone">RFC 822 time zone:</a></strong>
    '''     For formatting, the RFC 822 4-digit time zone format is used:
    ''' 
    '''     <pre>
    '''     <i>RFC822TimeZone:</i>
    '''             <i>Sign</i> <i>TwoDigitHours</i> <i>Minutes</i>
    '''     <i>TwoDigitHours:</i>
    '''             <i>Digit Digit</i></pre>
    '''     <i>TwoDigitHours</i> must be between 00 and 23. Other definitions
    '''     are as for <a href="#timezone">general time zones</a>.
    ''' 
    '''     <p>For parsing, <a href="#timezone">general time zones</a> are also
    '''     accepted.
    ''' <li><strong><a name="iso8601timezone">ISO 8601 Time zone:</a></strong>
    '''     The number of pattern letters designates the format for both formatting
    '''     and parsing as follows:
    '''     <pre>
    '''     <i>ISO8601TimeZone:</i>
    '''             <i>OneLetterISO8601TimeZone</i>
    '''             <i>TwoLetterISO8601TimeZone</i>
    '''             <i>ThreeLetterISO8601TimeZone</i>
    '''     <i>OneLetterISO8601TimeZone:</i>
    '''             <i>Sign</i> <i>TwoDigitHours</i>
    '''             {@code Z}
    '''     <i>TwoLetterISO8601TimeZone:</i>
    '''             <i>Sign</i> <i>TwoDigitHours</i> <i>Minutes</i>
    '''             {@code Z}
    '''     <i>ThreeLetterISO8601TimeZone:</i>
    '''             <i>Sign</i> <i>TwoDigitHours</i> {@code :} <i>Minutes</i>
    '''             {@code Z}</pre>
    '''     Other definitions are as for <a href="#timezone">general time zones</a> or
    '''     <a href="#rfc822timezone">RFC 822 time zones</a>.
    ''' 
    '''     <p>For formatting, if the offset value from GMT is 0, {@code "Z"} is
    '''     produced. If the number of pattern letters is 1, any fraction of an hour
    '''     is ignored. For example, if the pattern is {@code "X"} and the time zone is
    '''     {@code "GMT+05:30"}, {@code "+05"} is produced.
    ''' 
    '''     <p>For parsing, {@code "Z"} is parsed as the UTC time zone designator.
    '''     <a href="#timezone">General time zones</a> are <em>not</em> accepted.
    ''' 
    '''     <p>If the number of pattern letters is 4 or more, {@link
    '''     IllegalArgumentException} is thrown when constructing a {@code
    '''     SimpleDateFormat} or {@link #applyPattern(String) applying a
    '''     pattern}.
    ''' </ul>
    ''' <code>SimpleDateFormat</code> also supports <em>localized date and time
    ''' pattern</em> strings. In these strings, the pattern letters described above
    ''' may be replaced with other, locale dependent, pattern letters.
    ''' <code>SimpleDateFormat</code> does not deal with the localization of text
    ''' other than the pattern letters; that's up to the client of the class.
    ''' 
    ''' <h4>Examples</h4>
    ''' 
    ''' The following examples show how date and time patterns are interpreted in
    ''' the U.S. locale. The given date and time are 2001-07-04 12:08:56 local time
    ''' in the U.S. Pacific Time time zone.
    ''' <blockquote>
    ''' <table border=0 cellspacing=3 cellpadding=0 summary="Examples of date and time patterns interpreted in the U.S. locale">
    '''     <tr style="background-color: rgb(204, 204, 255);">
    '''         <th align=left>Date and Time Pattern
    '''         <th align=left>Result
    '''     <tr>
    '''         <td><code>"yyyy.MM.dd G 'at' HH:mm:ss z"</code>
    '''         <td><code>2001.07.04 AD at 12:08:56 PDT</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>"EEE, MMM d, ''yy"</code>
    '''         <td><code>Wed, Jul 4, '01</code>
    '''     <tr>
    '''         <td><code>"h:mm a"</code>
    '''         <td><code>12:08 PM</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>"hh 'o''clock' a, zzzz"</code>
    '''         <td><code>12 o'clock PM, Pacific Daylight Time</code>
    '''     <tr>
    '''         <td><code>"K:mm a, z"</code>
    '''         <td><code>0:08 PM, PDT</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>"yyyyy.MMMMM.dd GGG hh:mm aaa"</code>
    '''         <td><code>02001.July.04 AD 12:08 PM</code>
    '''     <tr>
    '''         <td><code>"EEE, d MMM yyyy HH:mm:ss Z"</code>
    '''         <td><code>Wed, 4 Jul 2001 12:08:56 -0700</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>"yyMMddHHmmssZ"</code>
    '''         <td><code>010704120856-0700</code>
    '''     <tr>
    '''         <td><code>"yyyy-MM-dd'T'HH:mm:ss.SSSZ"</code>
    '''         <td><code>2001-07-04T12:08:56.235-0700</code>
    '''     <tr style="background-color: rgb(238, 238, 255);">
    '''         <td><code>"yyyy-MM-dd'T'HH:mm:ss.SSSXXX"</code>
    '''         <td><code>2001-07-04T12:08:56.235-07:00</code>
    '''     <tr>
    '''         <td><code>"YYYY-'W'ww-u"</code>
    '''         <td><code>2001-W27-3</code>
    ''' </table>
    ''' </blockquote>
    ''' 
    ''' <h4><a name="synchronization">Synchronization</a></h4>
    ''' 
    ''' <p>
    ''' Date formats are not synchronized.
    ''' It is recommended to create separate format instances for each thread.
    ''' If multiple threads access a format concurrently, it must be synchronized
    ''' externally.
    ''' </summary>
    ''' <seealso cref=          <a href="https://docs.oracle.com/javase/tutorial/i18n/format/simpleDateFormat.html">Java Tutorial</a> </seealso>
    ''' <seealso cref=          java.util.Calendar </seealso>
    ''' <seealso cref=          java.util.TimeZone </seealso>
    ''' <seealso cref=          DateFormat </seealso>
    ''' <seealso cref=          DateFormatSymbols
    ''' @author       Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>
    Public Class SimpleDateFormat
        Inherits DateFormat

        ' the official serial version ID which says cryptically
        ' which version we're compatible with
        Friend Const serialVersionUID As Long = 4774881970558875024L

        ' the internal serial version which says which version was written
        ' - 0 (default) for version up to JDK 1.1.3
        ' - 1 for version from JDK 1.1.4, which includes a new field
        Friend Const currentSerialVersion As Integer = 1

        ''' <summary>
        ''' The version of the serialized data on the stream.  Possible values:
        ''' <ul>
        ''' <li><b>0</b> or not present on stream: JDK 1.1.3.  This version
        ''' has no <code>defaultCenturyStart</code> on stream.
        ''' <li><b>1</b> JDK 1.1.4 or later.  This version adds
        ''' <code>defaultCenturyStart</code>.
        ''' </ul>
        ''' When streaming out this [Class], the most recent format
        ''' and the highest allowable <code>serialVersionOnStream</code>
        ''' is written.
        ''' @serial
        ''' @since JDK1.1.4
        ''' </summary>
        Private serialVersionOnStream As Integer = currentSerialVersion

        ''' <summary>
        ''' The pattern string of this formatter.  This is always a non-localized
        ''' pattern.  May not be null.  See class documentation for details.
        ''' @serial
        ''' </summary>
        Private pattern As String

        ''' <summary>
        ''' Saved numberFormat and pattern. </summary>
        ''' <seealso cref= SimpleDateFormat#checkNegativeNumberExpression </seealso>
        <NonSerialized>
        Private originalNumberFormat As NumberFormat
        <NonSerialized>
        Private originalNumberPattern As String

        ''' <summary>
        ''' The minus sign to be used with format and parse.
        ''' </summary>
        <NonSerialized>
        Private minusSign As Char = "-"c

        ''' <summary>
        ''' True when a negative sign follows a number.
        ''' (True as default in Arabic.)
        ''' </summary>
        <NonSerialized>
        Private hasFollowingMinusSign As Boolean = False

        ''' <summary>
        ''' True if standalone form needs to be used.
        ''' </summary>
        <NonSerialized>
        Private forceStandaloneForm As Boolean = False

        ''' <summary>
        ''' The compiled pattern.
        ''' </summary>
        <NonSerialized>
        Private compiledPattern As Char()

        ''' <summary>
        ''' Tags for the compiled pattern.
        ''' </summary>
        Private Const TAG_QUOTE_ASCII_CHAR As Integer = 100
        Private Const TAG_QUOTE_CHARS As Integer = 101

        ''' <summary>
        ''' Locale dependent digit zero. </summary>
        ''' <seealso cref= #zeroPaddingNumber </seealso>
        ''' <seealso cref= java.text.DecimalFormatSymbols#getZeroDigit </seealso>
        <NonSerialized>
        Private zeroDigit As Char

        ''' <summary>
        ''' The symbols used by this formatter for week names, month names,
        ''' etc.  May not be null.
        ''' @serial </summary>
        ''' <seealso cref= java.text.DateFormatSymbols </seealso>
        Private formatData As DateFormatSymbols

        ''' <summary>
        ''' We map dates with two-digit years into the century starting at
        ''' <code>defaultCenturyStart</code>, which may be any date.  May
        ''' not be null.
        ''' @serial
        ''' @since JDK1.1.4
        ''' </summary>
        Private defaultCenturyStart As DateTime?

        <NonSerialized>
        Private defaultCenturyStartYear As Integer

        Private Const MILLIS_PER_MINUTE As Integer = 60 * 1000

        ' For time zones that have no names, use strings GMT+minutes and
        ' GMT-minutes. For instance, in France the time zone is GMT+60.
        Private Const GMT As String = "GMT"

        ''' <summary>
        ''' Cache NumberFormat instances with Locale key.
        ''' </summary>
        Private Shared ReadOnly cachedNumberFormatData As java.util.concurrent.ConcurrentMap(Of java.util.Locale, NumberFormat) = New ConcurrentDictionary(Of java.util.Locale, NumberFormat)(3)

        ''' <summary>
        ''' The Locale used to instantiate this
        ''' <code>SimpleDateFormat</code>. The value may be null if this object
        ''' has been created by an older <code>SimpleDateFormat</code> and
        ''' deserialized.
        ''' 
        ''' @serial
        ''' @since 1.6
        ''' </summary>
        Private locale As java.util.Locale

        ''' <summary>
        ''' Indicates whether this <code>SimpleDateFormat</code> should use
        ''' the DateFormatSymbols. If true, the format and parse methods
        ''' use the DateFormatSymbols values. If false, the format and
        ''' parse methods call Calendar.getDisplayName or
        ''' Calendar.getDisplayNames.
        ''' </summary>
        <NonSerialized>
        Friend useDateFormatSymbols_Renamed As Boolean

        ''' <summary>
        ''' Constructs a <code>SimpleDateFormat</code> using the default pattern and
        ''' date format symbols for the default
        ''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
        ''' <b>Note:</b> This constructor may not support all locales.
        ''' For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
        ''' class.
        ''' </summary>
        Public Sub New()
            Me.New("", java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
            applyPatternImpl(sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased.getLocaleResources(locale).getDateTimePattern([SHORT], [SHORT], calendar))
        End Sub

        ''' <summary>
        ''' Constructs a <code>SimpleDateFormat</code> using the given pattern and
        ''' the default date format symbols for the default
        ''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
        ''' <b>Note:</b> This constructor may not support all locales.
        ''' For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
        ''' class.
        ''' <p>This is equivalent to calling
        ''' {@link #SimpleDateFormat(String, Locale)
        '''     SimpleDateFormat(pattern, Locale.getDefault(Locale.Category.FORMAT))}.
        ''' </summary>
        ''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
        ''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
        ''' <param name="pattern"> the pattern describing the date and time format </param>
        ''' <exception cref="NullPointerException"> if the given pattern is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Public Sub New(ByVal pattern As String)
            Me.New(pattern, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
        End Sub

        ''' <summary>
        ''' Constructs a <code>SimpleDateFormat</code> using the given pattern and
        ''' the default date format symbols for the given locale.
        ''' <b>Note:</b> This constructor may not support all locales.
        ''' For full coverage, use the factory methods in the <seealso cref="DateFormat"/>
        ''' class.
        ''' </summary>
        ''' <param name="pattern"> the pattern describing the date and time format </param>
        ''' <param name="locale"> the locale whose date format symbols should be used </param>
        ''' <exception cref="NullPointerException"> if the given pattern or locale is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Public Sub New(ByVal pattern As String, ByVal locale As java.util.Locale)
            If pattern Is Nothing OrElse locale Is Nothing Then Throw New NullPointerException

            initializeCalendar(locale)
            Me.pattern = pattern
            Me.formatData = DateFormatSymbols.getInstanceRef(locale)
            Me.locale = locale
            initialize(locale)
        End Sub

        ''' <summary>
        ''' Constructs a <code>SimpleDateFormat</code> using the given pattern and
        ''' date format symbols.
        ''' </summary>
        ''' <param name="pattern"> the pattern describing the date and time format </param>
        ''' <param name="formatSymbols"> the date format symbols to be used for formatting </param>
        ''' <exception cref="NullPointerException"> if the given pattern or formatSymbols is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Public Sub New(ByVal pattern As String, ByVal formatSymbols As DateFormatSymbols)
            If pattern Is Nothing OrElse formatSymbols Is Nothing Then Throw New NullPointerException

            Me.pattern = pattern
            Me.formatData = CType(formatSymbols.clone(), DateFormatSymbols)
            Me.locale = java.util.Locale.getDefault(java.util.Locale.Category.FORMAT)
            initializeCalendar(Me.locale)
            initialize(Me.locale)
            useDateFormatSymbols_Renamed = True
        End Sub

        ' Initialize compiledPattern and numberFormat fields 
        Private Sub initialize(ByVal loc As java.util.Locale)
            ' Verify and compile the given pattern.
            compiledPattern = compile(pattern)

            ' try the cache first 
            numberFormat = cachedNumberFormatData.Get(loc)
            If numberFormat Is Nothing Then ' cache miss
                numberFormat = numberFormat.getIntegerInstance(loc)
                numberFormat.groupingUsed = False

                ' update cache 
                cachedNumberFormatData.putIfAbsent(loc, numberFormat)
            End If
            numberFormat = CType(numberFormat.clone(), NumberFormat)

            initializeDefaultCentury()
        End Sub

        Private Sub initializeCalendar(ByVal loc As java.util.Locale)
            If calendar Is Nothing Then
                Debug.Assert(loc IsNot Nothing)
                ' The format object must be constructed using the symbols for this zone.
                ' However, the calendar should use the current default TimeZone.
                ' If this is not contained in the locale zone strings, then the zone
                ' will be formatted using generic GMT+/-H:MM nomenclature.
                calendar = DateTime.getInstance(java.util.TimeZone.default, loc)
            End If
        End Sub

        ''' <summary>
        ''' Returns the compiled form of the given pattern. The syntax of
        ''' the compiled pattern is:
        ''' <blockquote>
        ''' CompiledPattern:
        '''     EntryList
        ''' EntryList:
        '''     Entry
        '''     EntryList Entry
        ''' Entry:
        '''     TagField
        '''     TagField data
        ''' TagField:
        '''     Tag Length
        '''     TaggedData
        ''' Tag:
        '''     pattern_char_index
        '''     TAG_QUOTE_CHARS
        ''' Length:
        '''     short_length
        '''     long_length
        ''' TaggedData:
        '''     TAG_QUOTE_ASCII_CHAR ascii_char
        ''' 
        ''' </blockquote>
        ''' 
        ''' where `short_length' is an 8-bit unsigned integer between 0 and
        ''' 254.  `long_length' is a sequence of an 8-bit integer 255 and a
        ''' 32-bit signed integer value which is split into upper and lower
        ''' 16-bit fields in two char's. `pattern_char_index' is an 8-bit
        ''' integer between 0 and 18. `ascii_char' is an 7-bit ASCII
        ''' character value. `data' depends on its Tag value.
        ''' <p>
        ''' If Length is short_length, Tag and short_length are packed in a
        ''' single char, as illustrated below.
        ''' <blockquote>
        '''     char[0] = (Tag << 8) | short_length;
        ''' </blockquote>
        ''' 
        ''' If Length is long_length, Tag and 255 are packed in the first
        ''' char and a 32-bit integer, as illustrated below.
        ''' <blockquote>
        '''     char[0] = (Tag << 8) | 255;
        '''     char[1] = (char) (long_length >>> 16);
        '''     char[2] = (char) (long_length & 0xffff);
        ''' </blockquote>
        ''' <p>
        ''' If Tag is a pattern_char_index, its Length is the number of
        ''' pattern characters. For example, if the given pattern is
        ''' "yyyy", Tag is 1 and Length is 4, followed by no data.
        ''' <p>
        ''' If Tag is TAG_QUOTE_CHARS, its Length is the number of char's
        ''' following the TagField. For example, if the given pattern is
        ''' "'o''clock'", Length is 7 followed by a char sequence of
        ''' <code>o&nbs;'&nbs;c&nbs;l&nbs;o&nbs;c&nbs;k</code>.
        ''' <p>
        ''' TAG_QUOTE_ASCII_CHAR is a special tag and has an ASCII
        ''' character in place of Length. For example, if the given pattern
        ''' is "'o'", the TaggedData entry is
        ''' <code>((TAG_QUOTE_ASCII_CHAR&nbs;<<&nbs;8)&nbs;|&nbs;'o')</code>.
        ''' </summary>
        ''' <exception cref="NullPointerException"> if the given pattern is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Private Function compile(ByVal pattern As String) As Char()
            Dim length As Integer = pattern.Length()
            Dim inQuote As Boolean = False
            Dim compiledCode As New StringBuilder(length * 2)
            Dim tmpBuffer As StringBuilder = Nothing
            Dim count As Integer = 0, tagcount As Integer = 0
            Dim lastTag As Integer = -1, prevTag As Integer = -1

            For i As Integer = 0 To length - 1
                Dim c As Char = pattern.Chars(i)

                If c = "'"c Then
                    ' '' is treated as a single quote regardless of being
                    ' in a quoted section.
                    If (i + 1) < length Then
                        c = pattern.Chars(i + 1)
                        If c = "'"c Then
                            i += 1
                            If count <> 0 Then
                                encode(lastTag, count, compiledCode)
                                tagcount += 1
                                prevTag = lastTag
                                lastTag = -1
                                count = 0
                            End If
                            If inQuote Then
                                tmpBuffer.append(c)
                            Else
                                compiledCode.append(CChar(TAG_QUOTE_ASCII_CHAR << 8 Or AscW(c)))
                            End If
                            Continue For
                        End If
                    End If
                    If Not inQuote Then
                        If count <> 0 Then
                            encode(lastTag, count, compiledCode)
                            tagcount += 1
                            prevTag = lastTag
                            lastTag = -1
                            count = 0
                        End If
                        If tmpBuffer Is Nothing Then
                            tmpBuffer = New StringBuilder(length)
                        Else
                            tmpBuffer.length = 0
                        End If
                        inQuote = True
                    Else
                        Dim len As Integer = tmpBuffer.length()
                        If len = 1 Then
                            Dim ch As Char = tmpBuffer.Chars(0)
                            If AscW(ch) < 128 Then
                                compiledCode.append(CChar(TAG_QUOTE_ASCII_CHAR << 8 Or AscW(ch)))
                            Else
                                compiledCode.append(CChar(TAG_QUOTE_CHARS << 8 Or 1))
                                compiledCode.append(ch)
                            End If
                        Else
                            encode(TAG_QUOTE_CHARS, len, compiledCode)
                            compiledCode.append(tmpBuffer)
                        End If
                        inQuote = False
                    End If
                    Continue For
                End If
                If inQuote Then
                    tmpBuffer.append(c)
                    Continue For
                End If
                If Not (c >= "a"c AndAlso c <= "z"c OrElse c >= "A"c AndAlso c <= "Z"c) Then
                    If count <> 0 Then
                        encode(lastTag, count, compiledCode)
                        tagcount += 1
                        prevTag = lastTag
                        lastTag = -1
                        count = 0
                    End If
                    If AscW(c) < 128 Then
                        ' In most cases, c would be a delimiter, such as ':'.
                        compiledCode.append(CChar(TAG_QUOTE_ASCII_CHAR << 8 Or AscW(c)))
                    Else
                        ' Take any contiguous non-ASCII alphabet characters and
                        ' put them in a single TAG_QUOTE_CHARS.
                        Dim j As Integer
                        For j = i + 1 To length - 1
                            Dim d As Char = pattern.Chars(j)
                            If d = "'"c OrElse (d >= "a"c AndAlso d <= "z"c OrElse d >= "A"c AndAlso d <= "Z"c) Then Exit For
                        Next j
                        compiledCode.append(CChar(TAG_QUOTE_CHARS << 8 Or (j - i)))
                        Do While i < j
                            compiledCode.append(pattern.Chars(i))
                            i += 1
                        Loop
                        i -= 1
                    End If
                    Continue For
                End If

                Dim tag As Integer
                tag = DateFormatSymbols.patternChars.IndexOf(c)
                If tag = -1 Then Throw New IllegalArgumentException("Illegal pattern character " & "'" & AscW(c) & "'")
                If lastTag = -1 OrElse lastTag = tag Then
                    lastTag = tag
                    count += 1
                    Continue For
                End If
                encode(lastTag, count, compiledCode)
                tagcount += 1
                prevTag = lastTag
                lastTag = tag
                count = 1
            Next i

            If inQuote Then Throw New IllegalArgumentException("Unterminated quote")

            If count <> 0 Then
                encode(lastTag, count, compiledCode)
                tagcount += 1
                prevTag = lastTag
            End If

            forceStandaloneForm = (tagcount = 1 AndAlso prevTag = PATTERN_MONTH)

            ' Copy the compiled pattern to a char array
            Dim len As Integer = compiledCode.length()
            Dim r As Char() = New Char(len - 1) {}
            compiledCode.getChars(0, len, r, 0)
            Return r
        End Function

        ''' <summary>
        ''' Encodes the given tag and length and puts encoded char(s) into buffer.
        ''' </summary>
        Private Shared Sub encode(ByVal tag As Integer, ByVal length As Integer, ByVal buffer As StringBuilder)
            If tag = PATTERN_ISO_ZONE AndAlso length >= 4 Then Throw New IllegalArgumentException("invalid ISO 8601 format: length=" & length)
            If length < 255 Then
                buffer.append(CChar(tag << 8 Or length))
            Else
                buffer.append(CChar((tag << 8) Or &HFF))
                buffer.append(ChrW(CInt(CUInt(length) >> 16)))
                buffer.append(CChar(length And &HFFFF))
            End If
        End Sub

        '     Initialize the fields we use to disambiguate ambiguous years. Separate
        '     * so we can call it from readObject().
        '     
        Private Sub initializeDefaultCentury()
            calendar.Value.timeInMillis = System.currentTimeMillis()
            calendar.Value.add(DateTime.Year, -80)
            parseAmbiguousDatesAsAfter(calendar.Value.time)
        End Sub

        '     Define one-century window into which to disambiguate dates using
        '     * two-digit years.
        '     
        Private Sub parseAmbiguousDatesAsAfter(ByVal startDate As DateTime?)
            defaultCenturyStart = startDate
            calendar.Value.time = startDate
            defaultCenturyStartYear = calendar.Value.get(DateTime.Year)
        End Sub

        ''' <summary>
        ''' Sets the 100-year period 2-digit years will be interpreted as being in
        ''' to begin on the date the user specifies.
        ''' </summary>
        ''' <param name="startDate"> During parsing, two digit years will be placed in the range
        ''' <code>startDate</code> to <code>startDate + 100 years</code>. </param>
        ''' <seealso cref= #get2DigitYearStart
        ''' @since 1.2 </seealso>
        Public Overridable Sub set2DigitYearStart(ByVal startDate As DateTime?)
            parseAmbiguousDatesAsAfter(New DateTime?(startDate.Value.time))
        End Sub

        ''' <summary>
        ''' Returns the beginning date of the 100-year period 2-digit years are interpreted
        ''' as being within.
        ''' </summary>
        ''' <returns> the start of the 100-year period into which two digit years are
        ''' parsed </returns>
        ''' <seealso cref= #set2DigitYearStart
        ''' @since 1.2 </seealso>
        Public Overridable Function get2DigitYearStart() As DateTime?
            Return CDate(defaultCenturyStart.Value.clone())
        End Function

        ''' <summary>
        ''' Formats the given <code>Date</code> into a date/time string and appends
        ''' the result to the given <code>StringBuffer</code>.
        ''' </summary>
        ''' <param name="date"> the date-time value to be formatted into a date-time string. </param>
        ''' <param name="toAppendTo"> where the new date-time text is to be appended. </param>
        ''' <param name="pos"> the formatting position. On input: an alignment field,
        ''' if desired. On output: the offsets of the alignment field. </param>
        ''' <returns> the formatted date-time string. </returns>
        ''' <exception cref="NullPointerException"> if the given {@code date} is {@code null}. </exception>
        Public Overrides Function format(ByVal [date] As DateTime?, ByVal toAppendTo As StringBuffer, ByVal pos As FieldPosition) As StringBuffer
            pos.endIndex = 0
            pos.beginIndex = pos.endIndex
            Return format(date_Renamed, toAppendTo, pos.fieldDelegate)
        End Function

        ' Called from Format after creating a FieldDelegate
        Private Function format(ByVal [date] As DateTime?, ByVal toAppendTo As StringBuffer, ByVal [delegate] As FieldDelegate) As StringBuffer
            ' Convert input date to time field list
            calendar.Value.time = date_Renamed

            Dim useDateFormatSymbols As Boolean = useDateFormatSymbols()

            Dim i As Integer = 0
            Do While i < compiledPattern.Length
                Dim tag As Integer = CInt(CUInt(compiledPattern(i)) >> 8)
                Dim count As Integer = AscW(compiledPattern(i)) And &HFF
                i += 1
                If count = 255 Then
                    count = AscW(compiledPattern(i)) << 16
                    i += 1
                    count = count Or compiledPattern(i)
                    i += 1
                End If

                Select Case tag
                    Case TAG_QUOTE_ASCII_CHAR
                        toAppendTo.append(ChrW(count))

                    Case TAG_QUOTE_CHARS
                        toAppendTo.append(compiledPattern, i, count)
                        i += count

                    Case Else
                        subFormat(tag, count, [delegate], toAppendTo, useDateFormatSymbols)
                End Select
            Loop
            Return toAppendTo
        End Function

        ''' <summary>
        ''' Formats an Object producing an <code>AttributedCharacterIterator</code>.
        ''' You can use the returned <code>AttributedCharacterIterator</code>
        ''' to build the resulting String, as well as to determine information
        ''' about the resulting String.
        ''' <p>
        ''' Each attribute key of the AttributedCharacterIterator will be of type
        ''' <code>DateFormat.Field</code>, with the corresponding attribute value
        ''' being the same as the attribute key.
        ''' </summary>
        ''' <exception cref="NullPointerException"> if obj is null. </exception>
        ''' <exception cref="IllegalArgumentException"> if the Format cannot format the
        '''            given object, or if the Format's pattern string is invalid. </exception>
        ''' <param name="obj"> The object to format </param>
        ''' <returns> AttributedCharacterIterator describing the formatted value.
        ''' @since 1.4 </returns>
        Public Overrides Function formatToCharacterIterator(ByVal obj As Object) As AttributedCharacterIterator
            Dim sb As New StringBuffer
            Dim [delegate] As New CharacterIteratorFieldDelegate

            If TypeOf obj Is DateTime? Then
                format(CDate(obj), sb, [delegate])
            ElseIf TypeOf obj Is Number Then
                format(New DateTime?(CType(obj, Number)), sb, [delegate])
            ElseIf obj Is Nothing Then
                Throw New NullPointerException("formatToCharacterIterator must be passed non-null object")
            Else
                Throw New IllegalArgumentException("Cannot format given Object as a Date")
            End If
            Return [delegate].getIterator(sb.ToString())
        End Function

        ' Map index into pattern character string to Calendar field number
        Private Shared ReadOnly PATTERN_INDEX_TO_CALENDAR_FIELD As Integer() = {DateTime.ERA, DateTime.Year, DateTime.Month, DateTime.Date, DateTime.HOUR_OF_DAY, DateTime.HOUR_OF_DAY, DateTime.Minute, DateTime.Second, DateTime.Millisecond, DateTime.DAY_OF_WEEK, DateTime.DAY_OF_YEAR, DateTime.DAY_OF_WEEK_IN_MONTH, DateTime.WEEK_OF_YEAR, DateTime.WEEK_OF_MONTH, DateTime.AM_PM, DateTime.Hour, DateTime.Hour, DateTime.ZONE_OFFSET, DateTime.ZONE_OFFSET, CalendarBuilder.WEEK_YEAR, CalendarBuilder.ISO_DAY_OF_WEEK, DateTime.ZONE_OFFSET, DateTime.Month}

        ' Map index into pattern character string to DateFormat field number
        Private Shared ReadOnly PATTERN_INDEX_TO_DATE_FORMAT_FIELD As Integer() = {DateFormat.ERA_FIELD, DateFormat.YEAR_FIELD, DateFormat.MONTH_FIELD, DateFormat.DATE_FIELD, DateFormat.HOUR_OF_DAY1_FIELD, DateFormat.HOUR_OF_DAY0_FIELD, DateFormat.MINUTE_FIELD, DateFormat.SECOND_FIELD, DateFormat.MILLISECOND_FIELD, DateFormat.DAY_OF_WEEK_FIELD, DateFormat.DAY_OF_YEAR_FIELD, DateFormat.DAY_OF_WEEK_IN_MONTH_FIELD, DateFormat.WEEK_OF_YEAR_FIELD, DateFormat.WEEK_OF_MONTH_FIELD, DateFormat.AM_PM_FIELD, DateFormat.HOUR1_FIELD, DateFormat.HOUR0_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.YEAR_FIELD, DateFormat.DAY_OF_WEEK_FIELD, DateFormat.TIMEZONE_FIELD, DateFormat.MONTH_FIELD}

        ' Maps from DecimalFormatSymbols index to Field constant
        Private Shared ReadOnly PATTERN_INDEX_TO_DATE_FORMAT_FIELD_ID As Field() = {Field.ERA, Field.YEAR, Field.MONTH, Field.DAY_OF_MONTH, Field.HOUR_OF_DAY1, Field.HOUR_OF_DAY0, Field.MINUTE, Field.SECOND, Field.MILLISECOND, Field.DAY_OF_WEEK, Field.DAY_OF_YEAR, Field.DAY_OF_WEEK_IN_MONTH, Field.WEEK_OF_YEAR, Field.WEEK_OF_MONTH, Field.AM_PM, Field.HOUR1, Field.HOUR0, Field.TIME_ZONE, Field.TIME_ZONE, Field.YEAR, Field.DAY_OF_WEEK, Field.TIME_ZONE, Field.MONTH}

        ''' <summary>
        ''' Private member function that does the real date/time formatting.
        ''' </summary>
        Private Sub subFormat(ByVal patternCharIndex As Integer, ByVal count As Integer, ByVal [delegate] As FieldDelegate, ByVal buffer As StringBuffer, ByVal useDateFormatSymbols As Boolean)
            Dim maxIntCount As Integer =  java.lang.[Integer].Max_Value
            Dim current As String = Nothing
            Dim beginOffset As Integer = buffer.length()

            Dim field_Renamed As Integer = PATTERN_INDEX_TO_CALENDAR_FIELD(patternCharIndex)
            Dim value As Integer
            If field_Renamed = CalendarBuilder.WEEK_YEAR Then
                If calendar.Value.weekDateSupported Then
                    value = calendar.Value.weekYear
                Else
                    ' use calendar year 'y' instead
                    patternCharIndex = PATTERN_YEAR
                    field_Renamed = PATTERN_INDEX_TO_CALENDAR_FIELD(patternCharIndex)
                    value = calendar.Value.get(field_Renamed)
                End If
            ElseIf field_Renamed = CalendarBuilder.ISO_DAY_OF_WEEK Then
                value = CalendarBuilder.toISODayOfWeek(calendar.Value.get(DateTime.DAY_OF_WEEK))
            Else
                value = calendar.Value.get(field_Renamed)
            End If

            Dim style As Integer = If(count >= 4, DateTime.LONG, DateTime.SHORT)
            If (Not useDateFormatSymbols) AndAlso field_Renamed < DateTime.ZONE_OFFSET AndAlso patternCharIndex <> PATTERN_MONTH_STANDALONE Then current = calendar.Value.getDisplayName(field_Renamed, style, locale)

            ' Note: zeroPaddingNumber() assumes that maxDigits is either
            ' 2 or maxIntCount. If we make any changes to this,
            ' zeroPaddingNumber() must be fixed.

            Select Case patternCharIndex
                Case PATTERN_ERA ' 'G'
                    If useDateFormatSymbols Then
                        Dim eras As String() = formatData.eras
                        If value < eras.Length Then current = eras(value)
                    End If
                    If current Is Nothing Then current = ""

                Case PATTERN_WEEK_YEAR, PATTERN_YEAR ' 'Y'
                    If TypeOf calendar Is java.util.GregorianCalendar Then
                        If count <> 2 Then
                            zeroPaddingNumber(value, count, maxIntCount, buffer)
                        Else
                            zeroPaddingNumber(value, 2, 2, buffer)
                        End If ' clip 1996 to 96
                    Else
                        If current Is Nothing Then zeroPaddingNumber(value, If(style = DateTime.LONG, 1, count), maxIntCount, buffer)
                    End If

                Case PATTERN_MONTH ' 'M' (context seinsive)
                    If useDateFormatSymbols Then
                        Dim months As String()
                        If count >= 4 Then
                            months = formatData.months
                            current = months(value)
                        ElseIf count = 3 Then
                            months = formatData.shortMonths
                            current = months(value)
                        End If
                    Else
                        If count < 3 Then
                            current = Nothing
                        ElseIf forceStandaloneForm Then
                            current = calendar.Value.getDisplayName(field_Renamed, style Or &H8000, locale)
                            If current Is Nothing Then current = calendar.Value.getDisplayName(field_Renamed, style, locale)
                        End If
                    End If
                    If current Is Nothing Then zeroPaddingNumber(value + 1, count, maxIntCount, buffer)

                Case PATTERN_MONTH_STANDALONE ' 'L'
                    Debug.Assert(current Is Nothing)
                    If locale Is Nothing Then
                        Dim months As String()
                        If count >= 4 Then
                            months = formatData.months
                            current = months(value)
                        ElseIf count = 3 Then
                            months = formatData.shortMonths
                            current = months(value)
                        End If
                    Else
                        If count >= 3 Then current = calendar.Value.getDisplayName(field_Renamed, style Or &H8000, locale)
                    End If
                    If current Is Nothing Then zeroPaddingNumber(value + 1, count, maxIntCount, buffer)

                Case PATTERN_HOUR_OF_DAY1 ' 'k' 1-based.  eg, 23:59 + 1 hour =>> 24:59
                    If current Is Nothing Then
                        If value = 0 Then
                            zeroPaddingNumber(calendar.Value.getMaximum(DateTime.HOUR_OF_DAY) + 1, count, maxIntCount, buffer)
                        Else
                            zeroPaddingNumber(value, count, maxIntCount, buffer)
                        End If
                    End If

                Case PATTERN_DAY_OF_WEEK ' 'E'
                    If useDateFormatSymbols Then
                        Dim weekdays As String()
                        If count >= 4 Then
                            weekdays = formatData.weekdays
                            current = weekdays(value) ' count < 4, use abbreviated form if exists
                        Else
                            weekdays = formatData.shortWeekdays
                            current = weekdays(value)
                        End If
                    End If

                Case PATTERN_AM_PM ' 'a'
                    If useDateFormatSymbols Then
                        Dim ampm As String() = formatData.amPmStrings
                        current = ampm(value)
                    End If

                Case PATTERN_HOUR1 ' 'h' 1-based.  eg, 11PM + 1 hour =>> 12 AM
                    If current Is Nothing Then
                        If value = 0 Then
                            zeroPaddingNumber(calendar.Value.getLeastMaximum(DateTime.Hour) + 1, count, maxIntCount, buffer)
                        Else
                            zeroPaddingNumber(value, count, maxIntCount, buffer)
                        End If
                    End If

                Case PATTERN_ZONE_NAME ' 'z'
                    If current Is Nothing Then
                        If formatData.locale Is Nothing OrElse formatData.isZoneStringsSet Then
                            Dim zoneIndex As Integer = formatData.getZoneIndex(calendar.Value.timeZone.iD)
                            If zoneIndex = -1 Then
                                value = calendar.Value.get(DateTime.ZONE_OFFSET) + calendar.Value.get(DateTime.DST_OFFSET)
                                buffer.append(sun.util.calendar.ZoneInfoFile.toCustomID(value))
                            Else
                                Dim index As Integer = If(calendar.Value.get(DateTime.DST_OFFSET) = 0, 1, 3)
                                If count < 4 Then index += 1
                                Dim zoneStrings As String()() = formatData.zoneStringsWrapper
                                buffer.append(zoneStrings(zoneIndex)(index))
                            End If
                        Else
                            Dim tz As java.util.TimeZone = calendar.Value.timeZone
                            Dim daylight As Boolean = (calendar.Value.get(DateTime.DST_OFFSET) <> 0)
                            Dim tzstyle As Integer = (If(count < 4, java.util.TimeZone.SHORT, java.util.TimeZone.LONG))
                            buffer.append(tz.getDisplayName(daylight, tzstyle, formatData.locale))
                        End If
                    End If

                Case PATTERN_ZONE_VALUE ' 'Z' ("-/+hhmm" form)
                    value = (calendar.Value.get(DateTime.ZONE_OFFSET) + calendar.Value.get(DateTime.DST_OFFSET)) / 60000

                    Dim width As Integer = 4
                    If value >= 0 Then
                        buffer.append("+"c)
                    Else
                        width += 1
                    End If

                    Dim num As Integer = (value \ 60) * 100 + (value Mod 60)
                    sun.util.calendar.CalendarUtils.sprintf0d(buffer, num, width)

                Case PATTERN_ISO_ZONE ' 'X'
                    value = calendar.Value.get(DateTime.ZONE_OFFSET) + calendar.Value.get(DateTime.DST_OFFSET)

                    If value = 0 Then
                        buffer.append("Z"c)
                        Exit Select
                    End If

                    value \= 60000
                    If value >= 0 Then
                        buffer.append("+"c)
                    Else
                        buffer.append("-"c)
                        value = -value
                    End If

                    sun.util.calendar.CalendarUtils.sprintf0d(buffer, value \ 60, 2)
                    If count = 1 Then Exit Select

                    If count = 3 Then buffer.append(":"c)
                    sun.util.calendar.CalendarUtils.sprintf0d(buffer, value Mod 60, 2)

                Case Else
                    ' case PATTERN_DAY_OF_MONTH:         // 'd'
                    ' case PATTERN_HOUR_OF_DAY0:         // 'H' 0-based.  eg, 23:59 + 1 hour =>> 00:59
                    ' case PATTERN_MINUTE:               // 'm'
                    ' case PATTERN_SECOND:               // 's'
                    ' case PATTERN_MILLISECOND:          // 'S'
                    ' case PATTERN_DAY_OF_YEAR:          // 'D'
                    ' case PATTERN_DAY_OF_WEEK_IN_MONTH: // 'F'
                    ' case PATTERN_WEEK_OF_YEAR:         // 'w'
                    ' case PATTERN_WEEK_OF_MONTH:        // 'W'
                    ' case PATTERN_HOUR0:                // 'K' eg, 11PM + 1 hour =>> 0 AM
                    ' case PATTERN_ISO_DAY_OF_WEEK:      // 'u' pseudo field, Monday = 1, ..., Sunday = 7
                    If current Is Nothing Then zeroPaddingNumber(value, count, maxIntCount, buffer)
                    Exit Select
            End Select ' switch (patternCharIndex)

            If current IsNot Nothing Then buffer.append(current)

            Dim fieldID As Integer = PATTERN_INDEX_TO_DATE_FORMAT_FIELD(patternCharIndex)
            Dim f As Field = PATTERN_INDEX_TO_DATE_FORMAT_FIELD_ID(patternCharIndex)

            [delegate].formatted(fieldID, f, f, beginOffset, buffer.length(), buffer)
        End Sub

        ''' <summary>
        ''' Formats a number with the specified minimum and maximum number of digits.
        ''' </summary>
        Private Sub zeroPaddingNumber(ByVal value As Integer, ByVal minDigits As Integer, ByVal maxDigits As Integer, ByVal buffer As StringBuffer)
            ' Optimization for 1, 2 and 4 digit numbers. This should
            ' cover most cases of formatting date/time related items.
            ' Note: This optimization code assumes that maxDigits is
            ' either 2 or  java.lang.[Integer].MAX_VALUE (maxIntCount in format()).
            Try
                If AscW(zeroDigit) = 0 Then zeroDigit = CType(numberFormat, DecimalFormat).decimalFormatSymbols.zeroDigit
                If value >= 0 Then
                    If value < 100 AndAlso minDigits >= 1 AndAlso minDigits <= 2 Then
                        If value < 10 Then
                            If minDigits = 2 Then buffer.append(zeroDigit)
                            buffer.append(CChar(AscW(zeroDigit) + value))
                        Else
                            buffer.append(CChar(AscW(zeroDigit) + value \ 10))
                            buffer.append(CChar(AscW(zeroDigit) + value Mod 10))
                        End If
                        Return
                    ElseIf value >= 1000 AndAlso value < 10000 Then
                        If minDigits = 4 Then
                            buffer.append(CChar(AscW(zeroDigit) + value \ 1000))
                            value = value Mod 1000
                            buffer.append(CChar(AscW(zeroDigit) + value \ 100))
                            value = value Mod 100
                            buffer.append(CChar(AscW(zeroDigit) + value \ 10))
                            buffer.append(CChar(AscW(zeroDigit) + value Mod 10))
                            Return
                        End If
                        If minDigits = 2 AndAlso maxDigits = 2 Then
                            zeroPaddingNumber(value Mod 100, 2, 2, buffer)
                            Return
                        End If
                    End If
                End If
            Catch e As Exception
            End Try

            numberFormat.minimumIntegerDigits = minDigits
            numberFormat.maximumIntegerDigits = maxDigits
            numberFormat.format(CLng(value), buffer, DontCareFieldPosition.INSTANCE)
        End Sub


        ''' <summary>
        ''' Parses text from a string to produce a <code>Date</code>.
        ''' <p>
        ''' The method attempts to parse text starting at the index given by
        ''' <code>pos</code>.
        ''' If parsing succeeds, then the index of <code>pos</code> is updated
        ''' to the index after the last character used (parsing does not necessarily
        ''' use all characters up to the end of the string), and the parsed
        ''' date is returned. The updated <code>pos</code> can be used to
        ''' indicate the starting point for the next call to this method.
        ''' If an error occurs, then the index of <code>pos</code> is not
        ''' changed, the error index of <code>pos</code> is set to the index of
        ''' the character where the error occurred, and null is returned.
        ''' 
        ''' <p>This parsing operation uses the {@link DateFormat#calendar
        ''' calendar} to produce a {@code Date}. All of the {@code
        ''' calendar}'s date-time fields are {@link Calendar#clear()
        ''' cleared} before parsing, and the {@code calendar}'s default
        ''' values of the date-time fields are used for any missing
        ''' date-time information. For example, the year value of the
        ''' parsed {@code Date} is 1970 with <seealso cref="GregorianCalendar"/> if
        ''' no year value is given from the parsing operation.  The {@code
        ''' TimeZone} value may be overwritten, depending on the given
        ''' pattern and the time zone value in {@code text}. Any {@code
        ''' TimeZone} value that has previously been set by a call to
        ''' <seealso cref="#setTimeZone(java.util.TimeZone) setTimeZone"/> may need
        ''' to be restored for further operations.
        ''' </summary>
        ''' <param name="text">  A <code>String</code>, part of which should be parsed. </param>
        ''' <param name="pos">   A <code>ParsePosition</code> object with index and error
        '''              index information as described above. </param>
        ''' <returns> A <code>Date</code> parsed from the string. In case of
        '''         error, returns null. </returns>
        ''' <exception cref="NullPointerException"> if <code>text</code> or <code>pos</code> is null. </exception>
        Public Overrides Function parse(ByVal text As String, ByVal pos As ParsePosition) As DateTime?
            checkNegativeNumberExpression()

            Dim start As Integer = pos.index
            Dim oldStart As Integer = start
            Dim textLength As Integer = text.Length()

            Dim ambiguousYear As Boolean() = {False}

            Dim calb As New CalendarBuilder

            Dim i As Integer = 0
            Do While i < compiledPattern.Length
                Dim tag As Integer = CInt(CUInt(compiledPattern(i)) >> 8)
                Dim count As Integer = AscW(compiledPattern(i)) And &HFF
                i += 1
                If count = 255 Then
                    count = AscW(compiledPattern(i)) << 16
                    i += 1
                    count = count Or compiledPattern(i)
                    i += 1
                End If

                Select Case tag
                    Case TAG_QUOTE_ASCII_CHAR
                        If start >= textLength OrElse text.Chars(start) <> ChrW(count) Then
                            pos.index = oldStart
                            pos.errorIndex = start
                            Return Nothing
                        End If
                        start += 1

                    Case TAG_QUOTE_CHARS
                        Dim tempVar As Boolean = count > 0
                        count -= 1
                        Do While tempVar
                            Dim tempVar2 As Boolean = start >= textLength OrElse text.Chars(start) <> compiledPattern(i)
                            i += 1
                            If tempVar2 Then
                                pos.index = oldStart
                                pos.errorIndex = start
                                Return Nothing
                            End If
                            start += 1
                            tempVar = count > 0
                            count -= 1
                        Loop

                    Case Else
                        ' Peek the next pattern to determine if we need to
                        ' obey the number of pattern letters for
                        ' parsing. It's required when parsing contiguous
                        ' digit text (e.g., "20010704") with a pattern which
                        ' has no delimiters between fields, like "yyyyMMdd".
                        Dim obeyCount As Boolean = False

                        ' In Arabic, a minus sign for a negative number is put after
                        ' the number. Even in another locale, a minus sign can be
                        ' put after a number using DateFormat.setNumberFormat().
                        ' If both the minus sign and the field-delimiter are '-',
                        ' subParse() needs to determine whether a '-' after a number
                        ' in the given text is a delimiter or is a minus sign for the
                        ' preceding number. We give subParse() a clue based on the
                        ' information in compiledPattern.
                        Dim useFollowingMinusSignAsDelimiter As Boolean = False

                        If i < compiledPattern.Length Then
                            Dim nextTag As Integer = CInt(CUInt(compiledPattern(i)) >> 8)
                            If Not (nextTag = TAG_QUOTE_ASCII_CHAR OrElse nextTag = TAG_QUOTE_CHARS) Then obeyCount = True

                            If hasFollowingMinusSign AndAlso (nextTag = TAG_QUOTE_ASCII_CHAR OrElse nextTag = TAG_QUOTE_CHARS) Then
                                Dim c As Integer
                                If nextTag = TAG_QUOTE_ASCII_CHAR Then
                                    c = AscW(compiledPattern(i)) And &HFF
                                Else
                                    c = AscW(compiledPattern(i + 1))
                                End If

                                If c = AscW(minusSign) Then useFollowingMinusSignAsDelimiter = True
                            End If
                        End If
                        start = subParse(text, start, tag, count, obeyCount, ambiguousYear, pos, useFollowingMinusSignAsDelimiter, calb)
                        If start < 0 Then
                            pos.index = oldStart
                            Return Nothing
                        End If
                End Select
            Loop

            ' At this point the fields of Calendar have been set.  Calendar
            ' will fill in default values for missing fields when the time
            ' is computed.

            pos.index = start

            Dim parsedDate As DateTime?
            Try
                parsedDate = calb.establish(calendar).Value.time
                ' If the year value is ambiguous,
                ' then the two-digit year == the default start year
                If ambiguousYear(0) Then
                    If parsedDate.Value.before(defaultCenturyStart) Then parsedDate = calb.addYear(100).establish(calendar).Value.time
                End If
                ' An IllegalArgumentException will be thrown by Calendar.getTime()
                ' if any fields are out of range, e.g., MONTH == 17.
            Catch e As IllegalArgumentException
                pos.errorIndex = start
                pos.index = oldStart
                Return Nothing
            End Try

            Return parsedDate
        End Function

        ''' <summary>
        ''' Private code-size reduction function used by subParse. </summary>
        ''' <param name="text"> the time text being parsed. </param>
        ''' <param name="start"> where to start parsing. </param>
        ''' <param name="field"> the date field being parsed. </param>
        ''' <param name="data"> the string array to parsed. </param>
        ''' <returns> the new start position if matching succeeded; a negative number
        ''' indicating matching failure, otherwise. </returns>
        Private Function matchString(ByVal text As String, ByVal start As Integer, ByVal field_Renamed As Integer, ByVal data As String(), ByVal calb As CalendarBuilder) As Integer
            Dim i As Integer = 0
            Dim count As Integer = data.Length

            If field_Renamed = DateTime.DAY_OF_WEEK Then i = 1

            ' There may be multiple strings in the data[] array which begin with
            ' the same prefix (e.g., Cerven and Cervenec (June and July) in Czech).
            ' We keep track of the longest match, and return that.  Note that this
            ' unfortunately requires us to test all array elements.
            Dim bestMatchLength As Integer = 0, bestMatch As Integer = -1
            Do While i < count
                Dim length As Integer = data(i).Length()
                ' Always compare if we have no match yet; otherwise only compare
                ' against potentially better matches (longer strings).
                If length > bestMatchLength AndAlso text.regionMatches(True, start, data(i), 0, length) Then
                    bestMatch = i
                    bestMatchLength = length
                End If
                i += 1
            Loop
            If bestMatch >= 0 Then
                calb.set(field_Renamed, bestMatch)
                Return start + bestMatchLength
            End If
            Return -start
        End Function

        ''' <summary>
        ''' Performs the same thing as matchString(String, int, int,
        ''' String[]). This method takes a Map<String, Integer> instead of
        ''' String[].
        ''' </summary>
        Private Function matchString(ByVal text As String, ByVal start As Integer, ByVal field_Renamed As Integer, ByVal data As IDictionary(Of String, Integer?), ByVal calb As CalendarBuilder) As Integer
            If data IsNot Nothing Then
                ' TODO: make this default when it's in the spec.
                If TypeOf data Is java.util.SortedMap Then
                    For Each name As String In data.Keys
                        If text.regionMatches(True, start, name, 0, name.Length()) Then
                            calb.set(field_Renamed, data(name))
                            Return start + name.Length()
                        End If
                    Next name
                    Return -start
                End If

                Dim bestMatch As String = Nothing

                For Each name As String In data.Keys
                    Dim length As Integer = name.Length()
                    If bestMatch Is Nothing OrElse length > bestMatch.Length() Then
                        If text.regionMatches(True, start, name, 0, length) Then bestMatch = name
                    End If
                Next name

                If bestMatch IsNot Nothing Then
                    calb.set(field_Renamed, data(bestMatch))
                    Return start + bestMatch.Length()
                End If
            End If
            Return -start
        End Function

        Private Function matchZoneString(ByVal text As String, ByVal start As Integer, ByVal zoneNames As String()) As Integer
            For i As Integer = 1 To 4
                ' Checking long and short zones [1 & 2],
                ' and long and short daylight [3 & 4].
                Dim zoneName As String = zoneNames(i)
                If text.regionMatches(True, start, zoneName, 0, zoneName.Length()) Then Return i
            Next i
            Return -1
        End Function

        Private Function matchDSTString(ByVal text As String, ByVal start As Integer, ByVal zoneIndex As Integer, ByVal standardIndex As Integer, ByVal zoneStrings As String()()) As Boolean
            Dim index As Integer = standardIndex + 2
            Dim zoneName As String = zoneStrings(zoneIndex)(index)
            If text.regionMatches(True, start, zoneName, 0, zoneName.Length()) Then Return True
            Return False
        End Function

        ''' <summary>
        ''' find time zone 'text' matched zoneStrings and set to internal
        ''' calendar.
        ''' </summary>
        Private Function subParseZoneString(ByVal text As String, ByVal start As Integer, ByVal calb As CalendarBuilder) As Integer
            Dim useSameName As Boolean = False ' true if standard and daylight time use the same abbreviation.
            Dim currentTimeZone As java.util.TimeZone = timeZone

            ' At this point, check for named time zones by looking through
            ' the locale data from the TimeZoneNames strings.
            ' Want to be able to parse both short and long forms.
            Dim zoneIndex As Integer = formatData.getZoneIndex(currentTimeZone.iD)
            Dim tz As java.util.TimeZone = Nothing
            Dim zoneStrings As String()() = formatData.zoneStringsWrapper
            Dim zoneNames As String() = Nothing
            Dim nameIndex As Integer = 0
            If zoneIndex <> -1 Then
                zoneNames = zoneStrings(zoneIndex)
                nameIndex = matchZoneString(text, start, zoneNames)
                If nameIndex > 0 Then
                    If nameIndex <= 2 Then useSameName = zoneNames(nameIndex).equalsIgnoreCase(zoneNames(nameIndex + 2))
                    tz = java.util.TimeZone.getTimeZone(zoneNames(0))
                End If
            End If
            If tz Is Nothing Then
                zoneIndex = formatData.getZoneIndex(java.util.TimeZone.default.iD)
                If zoneIndex <> -1 Then
                    zoneNames = zoneStrings(zoneIndex)
                    nameIndex = matchZoneString(text, start, zoneNames)
                    If nameIndex > 0 Then
                        If nameIndex <= 2 Then useSameName = zoneNames(nameIndex).equalsIgnoreCase(zoneNames(nameIndex + 2))
                        tz = java.util.TimeZone.getTimeZone(zoneNames(0))
                    End If
                End If
            End If

            If tz Is Nothing Then
                Dim len As Integer = zoneStrings.Length
                For i As Integer = 0 To len - 1
                    zoneNames = zoneStrings(i)
                    nameIndex = matchZoneString(text, start, zoneNames)
                    If nameIndex > 0 Then
                        If nameIndex <= 2 Then useSameName = zoneNames(nameIndex).equalsIgnoreCase(zoneNames(nameIndex + 2))
                        tz = java.util.TimeZone.getTimeZone(zoneNames(0))
                        Exit For
                    End If
                Next i
            End If
            If tz IsNot Nothing Then ' Matched any ?
                If Not tz.Equals(currentTimeZone) Then timeZone = tz
                ' If the time zone matched uses the same name
                ' (abbreviation) for both standard and daylight time,
                ' let the time zone in the Calendar decide which one.
                '
                ' Also if tz.getDSTSaving() returns 0 for DST, use tz to
                ' determine the local time. (6645292)
                Dim dstAmount As Integer = If(nameIndex >= 3, tz.dSTSavings, 0)
                If Not (useSameName OrElse (nameIndex >= 3 AndAlso dstAmount = 0)) Then calb.clear(DateTime.ZONE_OFFSET).set(DateTime.DST_OFFSET, dstAmount)
                Return (start + zoneNames(nameIndex).Length())
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Parses numeric forms of time zone offset, such as "hh:mm", and
        ''' sets calb to the parsed value.
        ''' </summary>
        ''' <param name="text">  the text to be parsed </param>
        ''' <param name="start"> the character position to start parsing </param>
        ''' <param name="sign">  1: positive; -1: negative </param>
        ''' <param name="count"> 0: 'Z' or "GMT+hh:mm" parsing; 1 - 3: the number of 'X's </param>
        ''' <param name="colon"> true - colon required between hh and mm; false - no colon required </param>
        ''' <param name="calb">  a CalendarBuilder in which the parsed value is stored </param>
        ''' <returns> updated parsed position, or its negative value to indicate a parsing error </returns>
        Private Function subParseNumericZone(ByVal text As String, ByVal start As Integer, ByVal sign As Integer, ByVal count As Integer, ByVal colon As Boolean, ByVal calb As CalendarBuilder) As Integer
            Dim index As Integer = start

parse:
            Try
                Dim c As Char = text.Chars(index)
                index += 1
                ' Parse hh
                Dim hours As Integer
                If Not isDigit(c) Then GoTo parse
                hours = AscW(c) - AscW("0"c)
                c = text.Chars(index)
                index += 1
                If isDigit(c) Then
                    hours = hours * 10 + (AscW(c) - AscW("0"c))
                Else
                    ' If no colon in RFC 822 or 'X' (ISO), two digits are
                    ' required.
                    If count > 0 OrElse (Not colon) Then GoTo parse
                    index -= 1
                End If
                If hours > 23 Then GoTo parse
                Dim minutes As Integer = 0
                If count <> 1 Then
                    ' Proceed with parsing mm
                    c = text.Chars(index)
                    index += 1
                    If colon Then
                        If c <> ":"c Then GoTo parse
                        c = text.Chars(index)
                        index += 1
                    End If
                    If Not isDigit(c) Then GoTo parse
                    minutes = AscW(c) - AscW("0"c)
                    c = text.Chars(index)
                    index += 1
                    If Not isDigit(c) Then GoTo parse
                    minutes = minutes * 10 + (AscW(c) - AscW("0"c))
                    If minutes > 59 Then GoTo parse
                End If
                minutes += hours * 60
                calb.set(DateTime.ZONE_OFFSET, minutes * MILLIS_PER_MINUTE * sign).set(DateTime.DST_OFFSET, 0)
                Return index
            Catch e As IndexOutOfBoundsException
            End Try
            Return 1 - index ' -(index - 1)
        End Function

        Private Function isDigit(ByVal c As Char) As Boolean
            Return c >= "0"c AndAlso c <= "9"c
        End Function

        ''' <summary>
        ''' Private member function that converts the parsed date strings into
        ''' timeFields. Returns -start (for ParsePosition) if failed. </summary>
        ''' <param name="text"> the time text to be parsed. </param>
        ''' <param name="start"> where to start parsing. </param>
        ''' <param name="patternCharIndex"> the index of the pattern character. </param>
        ''' <param name="count"> the count of a pattern character. </param>
        ''' <param name="obeyCount"> if true, then the next field directly abuts this one,
        ''' and we should use the count to know when to stop parsing. </param>
        ''' <param name="ambiguousYear"> return parameter; upon return, if ambiguousYear[0]
        ''' is true, then a two-digit year was parsed and may need to be readjusted. </param>
        ''' <param name="origPos"> origPos.errorIndex is used to return an error index
        ''' at which a parse error occurred, if matching failure occurs. </param>
        ''' <returns> the new start position if matching succeeded; -1 indicating
        ''' matching failure, otherwise. In case matching failure occurred,
        ''' an error index is set to origPos.errorIndex. </returns>
        Private Function subParse(ByVal text As String, ByVal start As Integer, ByVal patternCharIndex As Integer, ByVal count As Integer, ByVal obeyCount As Boolean, ByVal ambiguousYear As Boolean(), ByVal origPos As ParsePosition, ByVal useFollowingMinusSignAsDelimiter As Boolean, ByVal calb As CalendarBuilder) As Integer
            Dim number As Number
            Dim value As Integer = 0
            Dim pos As New ParsePosition(0)
            pos.index = start
            If patternCharIndex = PATTERN_WEEK_YEAR AndAlso (Not calendar.Value.weekDateSupported) Then patternCharIndex = PATTERN_YEAR
            Dim field_Renamed As Integer = PATTERN_INDEX_TO_CALENDAR_FIELD(patternCharIndex)

            ' If there are any spaces here, skip over them.  If we hit the end
            ' of the string, then fail.
            Do
                If pos.index >= text.Length() Then
                    origPos.errorIndex = start
                    Return -1
                End If
                Dim c As Char = text.Chars(pos.index)
                If c <> " "c AndAlso c <> ControlChars.Tab Then Exit Do
                pos.index += 1
            Loop
            ' Remember the actual start index
            Dim actualStart As Integer = pos.index

parsing:
            ' We handle a few special cases here where we need to parse
            ' a number value.  We handle further, more generic cases below.  We need
            ' to handle some of them here because some fields require extra processing on
            ' the parsed value.
            If patternCharIndex = PATTERN_HOUR_OF_DAY1 OrElse patternCharIndex = PATTERN_HOUR1 OrElse (patternCharIndex = PATTERN_MONTH AndAlso count <= 2) OrElse patternCharIndex = PATTERN_YEAR OrElse patternCharIndex = PATTERN_WEEK_YEAR Then
                ' It would be good to unify this with the obeyCount logic below,
                ' but that's going to be difficult.
                If obeyCount Then
                    If (start + count) > text.Length() Then GoTo parsing
                    number = numberFormat.parse(text.Substring(0, start + count), pos)
                Else
                    number = numberFormat.parse(text, pos)
                End If
                If number Is Nothing Then
                    If patternCharIndex <> PATTERN_YEAR OrElse TypeOf calendar Is java.util.GregorianCalendar Then GoTo parsing
                Else
                    value = number

                    If useFollowingMinusSignAsDelimiter AndAlso (value < 0) AndAlso (((pos.index < text.Length()) AndAlso (text.Chars(pos.index) <> minusSign)) OrElse ((pos.index = text.Length()) AndAlso (text.Chars(pos.index - 1) = minusSign))) Then
                        value = -value
                        pos.index -= 1
                    End If
                End If
            End If

            Dim useDateFormatSymbols As Boolean = useDateFormatSymbols()

            Dim index As Integer
            Select Case patternCharIndex
                Case PATTERN_ERA ' 'G'
                    If useDateFormatSymbols Then
                        index = matchString(text, start, DateTime.ERA, formatData.eras, calb)
                        If index > 0 Then Return index
                    Else
                        Dim map As IDictionary(Of String, Integer?) = getDisplayNamesMap(field_Renamed, locale)
                        index = matchString(text, start, field_Renamed, map, calb)
                        If index > 0 Then Return index
                    End If
                    GoTo parsing

                Case PATTERN_WEEK_YEAR, PATTERN_YEAR ' 'Y'
                    If Not (TypeOf calendar Is java.util.GregorianCalendar) Then
                        ' calendar might have text representations for year values,
                        ' such as "\u5143" in JapaneseImperialCalendar.
                        Dim style As Integer = If(count >= 4, DateTime.LONG, DateTime.SHORT)
                        Dim map As IDictionary(Of String, Integer?) = calendar.Value.getDisplayNames(field_Renamed, style, locale)
                        If map IsNot Nothing Then
                            index = matchString(text, start, field_Renamed, map, calb)
                            If index > 0 Then Return index
                        End If
                        calb.set(field_Renamed, value)
                        Return pos.index
                    End If

                    ' If there are 3 or more YEAR pattern characters, this indicates
                    ' that the year value is to be treated literally, without any
                    ' two-digit year adjustments (e.g., from "01" to 2001).  Otherwise
                    ' we made adjustments to place the 2-digit year in the proper
                    ' century, for parsed strings from "00" to "99".  Any other string
                    ' is treated literally:  "2250", "-1", "1", "002".
                    If count <= 2 AndAlso (pos.index - actualStart) = 2 AndAlso Char.IsDigit(text.Chars(actualStart)) AndAlso Char.IsDigit(text.Chars(actualStart + 1)) Then
                        ' Assume for example that the defaultCenturyStart is 6/18/1903.
                        ' This means that two-digit years will be forced into the range
                        ' 6/18/1903 to 6/17/2003.  As a result, years 00, 01, and 02
                        ' correspond to 2000, 2001, and 2002.  Years 04, 05, etc. correspond
                        ' to 1904, 1905, etc.  If the year is 03, then it is 2003 if the
                        ' other fields specify a date before 6/18, or 1903 if they specify a
                        ' date afterwards.  As a result, 03 is an ambiguous year.  All other
                        ' two-digit years are unambiguous.
                        Dim ambiguousTwoDigitYear As Integer = defaultCenturyStartYear Mod 100
                        ambiguousYear(0) = value = ambiguousTwoDigitYear
                        value += (defaultCenturyStartYear \ 100) * 100 + (If(value < ambiguousTwoDigitYear, 100, 0))
                    End If
                    calb.set(field_Renamed, value)
                    Return pos.index

                Case PATTERN_MONTH ' 'M'
                    If count <= 2 Then ' i.e., M or MM.
                        ' Don't want to parse the month if it is a string
                        ' while pattern uses numeric style: M or MM.
                        ' [We computed 'value' above.]
                        calb.set(DateTime.Month, value - 1)
                        Return pos.index
                    End If

                    If useDateFormatSymbols Then
                        ' count >= 3 // i.e., MMM or MMMM
                        ' Want to be able to parse both short and long forms.
                        ' Try count == 4 first:
                        Dim newStart As Integer
                        newStart = matchString(text, start, DateTime.Month, formatData.months, calb)
                        If newStart > 0 Then Return newStart
                        ' count == 4 failed, now try count == 3
                        index = matchString(text, start, DateTime.Month, formatData.shortMonths, calb)
                        If index > 0 Then Return index
                    Else
                        Dim map As IDictionary(Of String, Integer?) = getDisplayNamesMap(field_Renamed, locale)
                        index = matchString(text, start, field_Renamed, map, calb)
                        If index > 0 Then Return index
                    End If
                    GoTo parsing

                Case PATTERN_HOUR_OF_DAY1 ' 'k' 1-based.  eg, 23:59 + 1 hour =>> 24:59
                    If Not lenient Then
                        ' Validate the hour value in non-lenient
                        If value < 1 OrElse value > 24 Then GoTo parsing
                    End If
                    ' [We computed 'value' above.]
                    If value = calendar.Value.getMaximum(DateTime.HOUR_OF_DAY) + 1 Then value = 0
                    calb.set(DateTime.HOUR_OF_DAY, value)
                    Return pos.index

                Case PATTERN_DAY_OF_WEEK ' 'E'
                    If useDateFormatSymbols Then
                        ' Want to be able to parse both short and long forms.
                        ' Try count == 4 (DDDD) first:
                        Dim newStart As Integer
                        newStart = matchString(text, start, DateTime.DAY_OF_WEEK, formatData.weekdays, calb)
                        If newStart > 0 Then Return newStart
                        ' DDDD failed, now try DDD
                        index = matchString(text, start, DateTime.DAY_OF_WEEK, formatData.shortWeekdays, calb)
                        If index > 0 Then Return index
                    Else
                        Dim styles As Integer() = {DateTime.LONG, DateTime.SHORT}
                        For Each style As Integer In styles
                            Dim map As IDictionary(Of String, Integer?) = calendar.Value.getDisplayNames(field_Renamed, style, locale)
                            index = matchString(text, start, field_Renamed, map, calb)
                            If index > 0 Then Return index
                        Next style
                    End If
                    GoTo parsing

                Case PATTERN_AM_PM ' 'a'
                    If useDateFormatSymbols Then
                        index = matchString(text, start, DateTime.AM_PM, formatData.amPmStrings, calb)
                        If index > 0 Then Return index
                    Else
                        Dim map As IDictionary(Of String, Integer?) = getDisplayNamesMap(field_Renamed, locale)
                        index = matchString(text, start, field_Renamed, map, calb)
                        If index > 0 Then Return index
                    End If
                    GoTo parsing

                Case PATTERN_HOUR1 ' 'h' 1-based.  eg, 11PM + 1 hour =>> 12 AM
                    If Not lenient Then
                        ' Validate the hour value in non-lenient
                        If value < 1 OrElse value > 12 Then GoTo parsing
                    End If
                    ' [We computed 'value' above.]
                    If value = calendar.Value.getLeastMaximum(DateTime.Hour) + 1 Then value = 0
                    calb.set(DateTime.Hour, value)
                    Return pos.index

                Case PATTERN_ZONE_NAME, PATTERN_ZONE_VALUE ' 'z'
                    Dim sign As Integer = 0
                    Try
                        Dim c As Char = text.Chars(pos.index)
                        If c = "+"c Then
                            sign = 1
                        ElseIf c = "-"c Then
                            sign = -1
                        End If
                        If sign = 0 Then
                            ' Try parsing a custom time zone "GMT+hh:mm" or "GMT".
                            If (c = "G"c OrElse c = "g"c) AndAlso (text.Length() - start) >= GMT.Length() AndAlso text.regionMatches(True, start, GMT, 0, GMT.Length()) Then
                                pos.index = start + GMT.Length()

                                If (text.Length() - pos.index) > 0 Then
                                    c = text.Chars(pos.index)
                                    If c = "+"c Then
                                        sign = 1
                                    ElseIf c = "-"c Then
                                        sign = -1
                                    End If
                                End If

                                If sign = 0 Then ' "GMT" without offset
                                    calb.set(DateTime.ZONE_OFFSET, 0).set(DateTime.DST_OFFSET, 0)
                                    Return pos.index
                                End If

                                ' Parse the rest as "hh:mm"
                                pos.index += 1
                                Dim i As Integer = subParseNumericZone(text, pos.index, sign, 0, True, calb)
                                If i > 0 Then Return i
                                pos.index = -i
                            Else
                                ' Try parsing the text as a time zone
                                ' name or abbreviation.
                                Dim i As Integer = subParseZoneString(text, pos.index, calb)
                                If i > 0 Then Return i
                                pos.index = -i
                            End If
                        Else
                            ' Parse the rest as "hhmm" (RFC 822)
                            pos.index += 1
                            Dim i As Integer = subParseNumericZone(text, pos.index, sign, 0, False, calb)
                            If i > 0 Then Return i
                            pos.index = -i
                        End If
                    Catch e As IndexOutOfBoundsException
                    End Try
                    GoTo parsing

                Case PATTERN_ISO_ZONE ' 'X'
                    If (text.Length() - pos.index) <= 0 Then GoTo parsing

                    Dim sign As Integer
                    Dim c As Char = text.Chars(pos.index)
                    If c = "Z"c Then
                        calb.set(DateTime.ZONE_OFFSET, 0).set(DateTime.DST_OFFSET, 0)
                        pos.index += 1
                        Return pos.index
                    End If

                    ' parse text as "+/-hh[[:]mm]" based on count
                    If c = "+"c Then
                        sign = 1
                    ElseIf c = "-"c Then
                        sign = -1
                    Else
                        pos.index += 1
                        GoTo parsing
                    End If
                    pos.index += 1
                    Dim i As Integer = subParseNumericZone(text, pos.index, sign, count, count = 3, calb)
                    If i > 0 Then Return i
                    pos.index = -i
                    GoTo parsing

                Case Else
                    ' case PATTERN_DAY_OF_MONTH:         // 'd'
                    ' case PATTERN_HOUR_OF_DAY0:         // 'H' 0-based.  eg, 23:59 + 1 hour =>> 00:59
                    ' case PATTERN_MINUTE:               // 'm'
                    ' case PATTERN_SECOND:               // 's'
                    ' case PATTERN_MILLISECOND:          // 'S'
                    ' case PATTERN_DAY_OF_YEAR:          // 'D'
                    ' case PATTERN_DAY_OF_WEEK_IN_MONTH: // 'F'
                    ' case PATTERN_WEEK_OF_YEAR:         // 'w'
                    ' case PATTERN_WEEK_OF_MONTH:        // 'W'
                    ' case PATTERN_HOUR0:                // 'K' 0-based.  eg, 11PM + 1 hour =>> 0 AM
                    ' case PATTERN_ISO_DAY_OF_WEEK:      // 'u' (pseudo field);

                    ' Handle "generic" fields
                    If obeyCount Then
                        If (start + count) > text.Length() Then GoTo parsing
                        number = numberFormat.parse(text.Substring(0, start + count), pos)
                    Else
                        number = numberFormat.parse(text, pos)
                    End If
                    If number IsNot Nothing Then
                        value = number

                        If useFollowingMinusSignAsDelimiter AndAlso (value < 0) AndAlso (((pos.index < text.Length()) AndAlso (text.Chars(pos.index) <> minusSign)) OrElse ((pos.index = text.Length()) AndAlso (text.Chars(pos.index - 1) = minusSign))) Then
                            value = -value
                            pos.index -= 1
                        End If

                        calb.set(field_Renamed, value)
                        Return pos.index
                    End If
                    GoTo parsing
            End Select

            ' Parsing failed.
            origPos.errorIndex = pos.index
            Return -1
        End Function

        ''' <summary>
        ''' Returns true if the DateFormatSymbols has been set explicitly or locale
        ''' is null.
        ''' </summary>
        Private Function useDateFormatSymbols() As Boolean
            Return useDateFormatSymbols_Renamed OrElse locale Is Nothing
        End Function

        ''' <summary>
        ''' Translates a pattern, mapping each character in the from string to the
        ''' corresponding character in the to string.
        ''' </summary>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Private Function translatePattern(ByVal pattern As String, ByVal [from] As String, ByVal [to] As String) As String
            Dim result As New StringBuilder
            Dim inQuote As Boolean = False
            For i As Integer = 0 To pattern.Length() - 1
                Dim c As Char = pattern.Chars(i)
                If inQuote Then
                    If c = "'"c Then inQuote = False
                Else
                    If c = "'"c Then
                        inQuote = True
                    ElseIf (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "A"c AndAlso c <= "Z"c) Then
                        Dim ci As Integer = [from].IndexOf(c)
                        If ci >= 0 Then
                            ' patternChars is longer than localPatternChars due
                            ' to serialization compatibility. The pattern letters
                            ' unsupported by localPatternChars pass through.
                            If ci < [to].Length() Then c = [to].Chars(ci)
                        Else
                            Throw New IllegalArgumentException("Illegal pattern " & " character '" & AscW(c) & "'")
                        End If
                    End If
                End If
                result.append(c)
            Next i
            If inQuote Then Throw New IllegalArgumentException("Unfinished quote in pattern")
            Return result.ToString()
        End Function

        ''' <summary>
        ''' Returns a pattern string describing this date format.
        ''' </summary>
        ''' <returns> a pattern string describing this date format. </returns>
        Public Overridable Function toPattern() As String
            Return pattern
        End Function

        ''' <summary>
        ''' Returns a localized pattern string describing this date format.
        ''' </summary>
        ''' <returns> a localized pattern string describing this date format. </returns>
        Public Overridable Function toLocalizedPattern() As String
            Return translatePattern(pattern, DateFormatSymbols.patternChars, formatData.localPatternChars)
        End Function

        ''' <summary>
        ''' Applies the given pattern string to this date format.
        ''' </summary>
        ''' <param name="pattern"> the new date and time pattern for this date format </param>
        ''' <exception cref="NullPointerException"> if the given pattern is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Public Overridable Sub applyPattern(ByVal pattern As String)
            applyPatternImpl(pattern)
        End Sub

        Private Sub applyPatternImpl(ByVal pattern As String)
            compiledPattern = compile(pattern)
            Me.pattern = pattern
        End Sub

        ''' <summary>
        ''' Applies the given localized pattern string to this date format.
        ''' </summary>
        ''' <param name="pattern"> a String to be mapped to the new date and time format
        '''        pattern for this format </param>
        ''' <exception cref="NullPointerException"> if the given pattern is null </exception>
        ''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
        Public Overridable Sub applyLocalizedPattern(ByVal pattern As String)
            Dim p As String = translatePattern(pattern, formatData.localPatternChars, DateFormatSymbols.patternChars)
            compiledPattern = compile(p)
            Me.pattern = p
        End Sub

        ''' <summary>
        ''' Gets a copy of the date and time format symbols of this date format.
        ''' </summary>
        ''' <returns> the date and time format symbols of this date format </returns>
        ''' <seealso cref= #setDateFormatSymbols </seealso>
        Public Overridable Property dateFormatSymbols As DateFormatSymbols
            Get
                Return CType(formatData.clone(), DateFormatSymbols)
            End Get
            Set(ByVal newFormatSymbols As DateFormatSymbols)
                Me.formatData = CType(newFormatSymbols.clone(), DateFormatSymbols)
                useDateFormatSymbols_Renamed = True
            End Set
        End Property


        ''' <summary>
        ''' Creates a copy of this <code>SimpleDateFormat</code>. This also
        ''' clones the format's date format symbols.
        ''' </summary>
        ''' <returns> a clone of this <code>SimpleDateFormat</code> </returns>
        Public Overrides Function clone() As Object
            Dim other As SimpleDateFormat = CType(MyBase.clone(), SimpleDateFormat)
            other.formatData = CType(formatData.clone(), DateFormatSymbols)
            Return other
        End Function

        ''' <summary>
        ''' Returns the hash code value for this <code>SimpleDateFormat</code> object.
        ''' </summary>
        ''' <returns> the hash code value for this <code>SimpleDateFormat</code> object. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return pattern.GetHashCode()
            ' just enough fields for a reasonable distribution
        End Function

        ''' <summary>
        ''' Compares the given object with this <code>SimpleDateFormat</code> for
        ''' equality.
        ''' </summary>
        ''' <returns> true if the given object is equal to this
        ''' <code>SimpleDateFormat</code> </returns>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False ' super does class check
            Dim that As SimpleDateFormat = CType(obj, SimpleDateFormat)
            Return (pattern.Equals(that.pattern) AndAlso formatData.Equals(that.formatData))
        End Function

        Private Shared ReadOnly REST_OF_STYLES As Integer() = {DateTime.SHORT_STANDALONE, DateTime.LONG_FORMAT, DateTime.LONG_STANDALONE}
        Private Function getDisplayNamesMap(ByVal field_Renamed As Integer, ByVal locale As java.util.Locale) As IDictionary(Of String, Integer?)
            Dim map As IDictionary(Of String, Integer?) = calendar.Value.getDisplayNames(field_Renamed, DateTime.SHORT_FORMAT, locale)
            ' Get all SHORT and LONG styles (avoid NARROW styles).
            For Each style As Integer In REST_OF_STYLES
                Dim m As IDictionary(Of String, Integer?) = calendar.Value.getDisplayNames(field_Renamed, style, locale)
                If m IsNot Nothing Then map.putAll(m)
            Next style
            Return map
        End Function

        ''' <summary>
        ''' After reading an object from the input stream, the format
        ''' pattern in the object is verified.
        ''' <p> </summary>
        ''' <exception cref="InvalidObjectException"> if the pattern is invalid </exception>
        Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
            stream.defaultReadObject()

            Try
                compiledPattern = compile(pattern)
            Catch e As Exception
                Throw New java.io.InvalidObjectException("invalid pattern")
            End Try

            If serialVersionOnStream < 1 Then
                ' didn't have defaultCenturyStart field
                initializeDefaultCentury()
            Else
                ' fill in dependent transient field
                parseAmbiguousDatesAsAfter(defaultCenturyStart)
            End If
            serialVersionOnStream = currentSerialVersion

            ' If the deserialized object has a SimpleTimeZone, try
            ' to replace it with a ZoneInfo equivalent in order to
            ' be compatible with the SimpleTimeZone-based
            ' implementation as much as possible.
            Dim tz As java.util.TimeZone = timeZone
            If TypeOf tz Is java.util.SimpleTimeZone Then
                Dim id As String = tz.iD
                Dim zi As java.util.TimeZone = java.util.TimeZone.getTimeZone(id)
                If zi IsNot Nothing AndAlso zi.hasSameRules(tz) AndAlso zi.iD.Equals(id) Then timeZone = zi
            End If
        End Sub

        ''' <summary>
        ''' Analyze the negative subpattern of DecimalFormat and set/update values
        ''' as necessary.
        ''' </summary>
        Private Sub checkNegativeNumberExpression()
            If (TypeOf numberFormat Is DecimalFormat) AndAlso (Not numberFormat.Equals(originalNumberFormat)) Then
                Dim numberPattern As String = CType(numberFormat, DecimalFormat).toPattern()
                If Not numberPattern.Equals(originalNumberPattern) Then
                    hasFollowingMinusSign = False

                    Dim separatorIndex As Integer = numberPattern.IndexOf(";"c)
                    ' If the negative subpattern is not absent, we have to analayze
                    ' it in order to check if it has a following minus sign.
                    If separatorIndex > -1 Then
                        Dim minusIndex As Integer = numberPattern.IndexOf("-"c, separatorIndex)
                        If (minusIndex > numberPattern.LastIndexOf("0"c)) AndAlso (minusIndex > numberPattern.LastIndexOf("#"c)) Then
                            hasFollowingMinusSign = True
                            minusSign = CType(numberFormat, DecimalFormat).decimalFormatSymbols.minusSign
                        End If
                    End If
                    originalNumberPattern = numberPattern
                End If
                originalNumberFormat = numberFormat
            End If
        End Sub

    End Class

End Namespace