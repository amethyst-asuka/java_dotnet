Imports System
Imports System.Collections.Generic

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

Namespace java.text


	''' <summary>
	''' {@code DateFormat} is an abstract class for date/time formatting subclasses which
	''' formats and parses dates or time in a language-independent manner.
	''' The date/time formatting subclass, such as <seealso cref="SimpleDateFormat"/>, allows for
	''' formatting (i.e., date &rarr; text), parsing (text &rarr; date), and
	''' normalization.  The date is represented as a <code>Date</code> object or
	''' as the milliseconds since January 1, 1970, 00:00:00 GMT.
	''' 
	''' <p>{@code DateFormat} provides many class methods for obtaining default date/time
	''' formatters based on the default or a given locale and a number of formatting
	''' styles. The formatting styles include <seealso cref="#FULL"/>, <seealso cref="#LONG"/>, <seealso cref="#MEDIUM"/>, and <seealso cref="#SHORT"/>. More
	''' detail and examples of using these styles are provided in the method
	''' descriptions.
	''' 
	''' <p>{@code DateFormat} helps you to format and parse dates for any locale.
	''' Your code can be completely independent of the locale conventions for
	''' months, days of the week, or even the calendar format: lunar vs. solar.
	''' 
	''' <p>To format a date for the current Locale, use one of the
	''' static factory methods:
	''' <blockquote>
	''' <pre>{@code
	''' myString = DateFormat.getDateInstance().format(myDate);
	''' }</pre>
	''' </blockquote>
	''' <p>If you are formatting multiple dates, it is
	''' more efficient to get the format and use it multiple times so that
	''' the system doesn't have to fetch the information about the local
	''' language and country conventions multiple times.
	''' <blockquote>
	''' <pre>{@code
	''' DateFormat df = DateFormat.getDateInstance();
	''' for (int i = 0; i < myDate.length; ++i) {
	'''     output.println(df.format(myDate[i]) + "; ");
	''' }
	''' }</pre>
	''' </blockquote>
	''' <p>To format a date for a different Locale, specify it in the
	''' call to <seealso cref="#getDateInstance(int, Locale) getDateInstance()"/>.
	''' <blockquote>
	''' <pre>{@code
	''' DateFormat df = DateFormat.getDateInstance(DateFormat.LONG, Locale.FRANCE);
	''' }</pre>
	''' </blockquote>
	''' <p>You can use a DateFormat to parse also.
	''' <blockquote>
	''' <pre>{@code
	''' myDate = df.parse(myString);
	''' }</pre>
	''' </blockquote>
	''' <p>Use {@code getDateInstance} to get the normal date format for that country.
	''' There are other static factory methods available.
	''' Use {@code getTimeInstance} to get the time format for that country.
	''' Use {@code getDateTimeInstance} to get a date and time format. You can pass in
	''' different options to these factory methods to control the length of the
	''' result; from <seealso cref="#SHORT"/> to <seealso cref="#MEDIUM"/> to <seealso cref="#LONG"/> to <seealso cref="#FULL"/>. The exact result depends
	''' on the locale, but generally:
	''' <ul><li><seealso cref="#SHORT"/> is completely numeric, such as {@code 12.13.52} or {@code 3:30pm}
	''' <li><seealso cref="#MEDIUM"/> is longer, such as {@code Jan 12, 1952}
	''' <li><seealso cref="#LONG"/> is longer, such as {@code January 12, 1952} or {@code 3:30:32pm}
	''' <li><seealso cref="#FULL"/> is pretty completely specified, such as
	''' {@code Tuesday, April 12, 1952 AD or 3:30:42pm PST}.
	''' </ul>
	''' 
	''' <p>You can also set the time zone on the format if you wish.
	''' If you want even more control over the format or parsing,
	''' (or want to give your users more control),
	''' you can try casting the {@code DateFormat} you get from the factory methods
	''' to a <seealso cref="SimpleDateFormat"/>. This will work for the majority
	''' of countries; just remember to put it in a {@code try} block in case you
	''' encounter an unusual one.
	''' 
	''' <p>You can also use forms of the parse and format methods with
	''' <seealso cref="ParsePosition"/> and <seealso cref="FieldPosition"/> to
	''' allow you to
	''' <ul><li>progressively parse through pieces of a string.
	''' <li>align any particular field, or find out where it is for selection
	''' on the screen.
	''' </ul>
	''' 
	''' <h3><a name="synchronization">Synchronization</a></h3>
	''' 
	''' <p>
	''' Date formats are not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' </summary>
	''' <seealso cref=          Format </seealso>
	''' <seealso cref=          NumberFormat </seealso>
	''' <seealso cref=          SimpleDateFormat </seealso>
	''' <seealso cref=          java.util.Calendar </seealso>
	''' <seealso cref=          java.util.GregorianCalendar </seealso>
	''' <seealso cref=          java.util.TimeZone
	''' @author       Mark Davis, Chen-Lieh Huang, Alan Liu </seealso>
	Public MustInherit Class DateFormat
		Inherits Format

		''' <summary>
		''' The <seealso cref="Calendar"/> instance used for calculating the date-time fields
		''' and the instant of time. This field is used for both formatting and
		''' parsing.
		''' 
		''' <p>Subclasses should initialize this field to a <seealso cref="Calendar"/>
		''' appropriate for the <seealso cref="Locale"/> associated with this
		''' <code>DateFormat</code>.
		''' @serial
		''' </summary>
		Protected Friend calendar As DateTime?

		''' <summary>
		''' The number formatter that <code>DateFormat</code> uses to format numbers
		''' in dates and times.  Subclasses should initialize this to a number format
		''' appropriate for the locale associated with this <code>DateFormat</code>.
		''' @serial
		''' </summary>
		Protected Friend numberFormat As NumberFormat

		''' <summary>
		''' Useful constant for ERA field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const ERA_FIELD As Integer = 0
		''' <summary>
		''' Useful constant for YEAR field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const YEAR_FIELD As Integer = 1
		''' <summary>
		''' Useful constant for MONTH field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const MONTH_FIELD As Integer = 2
		''' <summary>
		''' Useful constant for DATE field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const DATE_FIELD As Integer = 3
		''' <summary>
		''' Useful constant for one-based HOUR_OF_DAY field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' HOUR_OF_DAY1_FIELD is used for the one-based 24-hour clock.
		''' For example, 23:59 + 01:00 results in 24:59.
		''' </summary>
		Public Const HOUR_OF_DAY1_FIELD As Integer = 4
		''' <summary>
		''' Useful constant for zero-based HOUR_OF_DAY field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' HOUR_OF_DAY0_FIELD is used for the zero-based 24-hour clock.
		''' For example, 23:59 + 01:00 results in 00:59.
		''' </summary>
		Public Const HOUR_OF_DAY0_FIELD As Integer = 5
		''' <summary>
		''' Useful constant for MINUTE field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const MINUTE_FIELD As Integer = 6
		''' <summary>
		''' Useful constant for SECOND field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const SECOND_FIELD As Integer = 7
		''' <summary>
		''' Useful constant for MILLISECOND field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const MILLISECOND_FIELD As Integer = 8
		''' <summary>
		''' Useful constant for DAY_OF_WEEK field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const DAY_OF_WEEK_FIELD As Integer = 9
		''' <summary>
		''' Useful constant for DAY_OF_YEAR field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const DAY_OF_YEAR_FIELD As Integer = 10
		''' <summary>
		''' Useful constant for DAY_OF_WEEK_IN_MONTH field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const DAY_OF_WEEK_IN_MONTH_FIELD As Integer = 11
		''' <summary>
		''' Useful constant for WEEK_OF_YEAR field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const WEEK_OF_YEAR_FIELD As Integer = 12
		''' <summary>
		''' Useful constant for WEEK_OF_MONTH field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const WEEK_OF_MONTH_FIELD As Integer = 13
		''' <summary>
		''' Useful constant for AM_PM field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const AM_PM_FIELD As Integer = 14
		''' <summary>
		''' Useful constant for one-based HOUR field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' HOUR1_FIELD is used for the one-based 12-hour clock.
		''' For example, 11:30 PM + 1 hour results in 12:30 AM.
		''' </summary>
		Public Const HOUR1_FIELD As Integer = 15
		''' <summary>
		''' Useful constant for zero-based HOUR field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' HOUR0_FIELD is used for the zero-based 12-hour clock.
		''' For example, 11:30 PM + 1 hour results in 00:30 AM.
		''' </summary>
		Public Const HOUR0_FIELD As Integer = 16
		''' <summary>
		''' Useful constant for TIMEZONE field alignment.
		''' Used in FieldPosition of date/time formatting.
		''' </summary>
		Public Const TIMEZONE_FIELD As Integer = 17

		' Proclaim serial compatibility with 1.1 FCS
		Private Const serialVersionUID As Long = 7218322306649953788L

		''' <summary>
		''' Overrides Format.
		''' Formats a time object into a time string. Examples of time objects
		''' are a time value expressed in milliseconds and a Date object. </summary>
		''' <param name="obj"> must be a Number or a Date. </param>
		''' <param name="toAppendTo"> the string buffer for the returning time string. </param>
		''' <returns> the string buffer passed in as toAppendTo, with formatted text appended. </returns>
		''' <param name="fieldPosition"> keeps track of the position of the field
		''' within the returned string.
		''' On input: an alignment field,
		''' if desired. On output: the offsets of the alignment field. For
		''' example, given a time text "1996.07.10 AD at 15:08:56 PDT",
		''' if the given fieldPosition is DateFormat.YEAR_FIELD, the
		''' begin index and end index of fieldPosition will be set to
		''' 0 and 4, respectively.
		''' Notice that if the same time field appears
		''' more than once in a pattern, the fieldPosition will be set for the first
		''' occurrence of that time field. For instance, formatting a Date to
		''' the time string "1 PM PDT (Pacific Daylight Time)" using the pattern
		''' "h a z (zzzz)" and the alignment field DateFormat.TIMEZONE_FIELD,
		''' the begin index and end index of fieldPosition will be set to
		''' 5 and 8, respectively, for the first occurrence of the timezone
		''' pattern character 'z'. </param>
		''' <seealso cref= java.text.Format </seealso>
		Public NotOverridable Overrides Function format(ByVal obj As Object, ByVal toAppendTo As StringBuffer, ByVal fieldPosition As FieldPosition) As StringBuffer
			If TypeOf obj Is DateTime? Then
				Return format(CDate(obj), toAppendTo, fieldPosition)
			ElseIf TypeOf obj Is Number Then
				Return format(New DateTime?(CType(obj, Number)), toAppendTo, fieldPosition)
			Else
				Throw New IllegalArgumentException("Cannot format given Object as a Date")
			End If
		End Function

		''' <summary>
		''' Formats a Date into a date/time string. </summary>
		''' <param name="date"> a Date to be formatted into a date/time string. </param>
		''' <param name="toAppendTo"> the string buffer for the returning date/time string. </param>
		''' <param name="fieldPosition"> keeps track of the position of the field
		''' within the returned string.
		''' On input: an alignment field,
		''' if desired. On output: the offsets of the alignment field. For
		''' example, given a time text "1996.07.10 AD at 15:08:56 PDT",
		''' if the given fieldPosition is DateFormat.YEAR_FIELD, the
		''' begin index and end index of fieldPosition will be set to
		''' 0 and 4, respectively.
		''' Notice that if the same time field appears
		''' more than once in a pattern, the fieldPosition will be set for the first
		''' occurrence of that time field. For instance, formatting a Date to
		''' the time string "1 PM PDT (Pacific Daylight Time)" using the pattern
		''' "h a z (zzzz)" and the alignment field DateFormat.TIMEZONE_FIELD,
		''' the begin index and end index of fieldPosition will be set to
		''' 5 and 8, respectively, for the first occurrence of the timezone
		''' pattern character 'z'. </param>
		''' <returns> the string buffer passed in as toAppendTo, with formatted text appended. </returns>
		Public MustOverride Function format(ByVal [date] As DateTime?, ByVal toAppendTo As StringBuffer, ByVal fieldPosition As FieldPosition) As StringBuffer

		''' <summary>
		''' Formats a Date into a date/time string. </summary>
		''' <param name="date"> the time value to be formatted into a time string. </param>
		''' <returns> the formatted time string. </returns>
		Public Function format(ByVal [date] As DateTime?) As String
			Return format(date_Renamed, New StringBuffer, DontCareFieldPosition.INSTANCE).ToString()
		End Function

		''' <summary>
		''' Parses text from the beginning of the given string to produce a date.
		''' The method may not use the entire text of the given string.
		''' <p>
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on date parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		''' <returns> A <code>Date</code> parsed from the string. </returns>
		''' <exception cref="ParseException"> if the beginning of the specified string
		'''            cannot be parsed. </exception>
		Public Overridable Function parse(ByVal source As String) As DateTime?
			Dim pos As New ParsePosition(0)
			Dim result As DateTime? = parse(source, pos)
			If pos.index = 0 Then Throw New ParseException("Unparseable date: """ & source & """", pos.errorIndex)
			Return result
		End Function

		''' <summary>
		''' Parse a date/time string according to the given parse position.  For
		''' example, a time text {@code "07/10/96 4:5 PM, PDT"} will be parsed into a {@code Date}
		''' that is equivalent to {@code Date(837039900000L)}.
		''' 
		''' <p> By default, parsing is lenient: If the input is not in the form used
		''' by this object's format method but can still be parsed as a date, then
		''' the parse succeeds.  Clients may insist on strict adherence to the
		''' format by calling <seealso cref="#setLenient(boolean) setLenient(false)"/>.
		''' 
		''' <p>This parsing operation uses the <seealso cref="#calendar"/> to produce
		''' a {@code Date}. As a result, the {@code calendar}'s date-time
		''' fields and the {@code TimeZone} value may have been
		''' overwritten, depending on subclass implementations. Any {@code
		''' TimeZone} value that has previously been set by a call to
		''' <seealso cref="#setTimeZone(java.util.TimeZone) setTimeZone"/> may need
		''' to be restored for further operations.
		''' </summary>
		''' <param name="source">  The date/time string to be parsed
		''' </param>
		''' <param name="pos">   On input, the position at which to start parsing; on
		'''              output, the position at which parsing terminated, or the
		'''              start position if the parse failed.
		''' </param>
		''' <returns>      A {@code Date}, or {@code null} if the input could not be parsed </returns>
		Public MustOverride Function parse(ByVal source As String, ByVal pos As ParsePosition) As DateTime?

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
		''' <p>
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on date parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		''' <param name="pos"> A <code>ParsePosition</code> object with index and error
		'''            index information as described above. </param>
		''' <returns> A <code>Date</code> parsed from the string. In case of
		'''         error, returns null. </returns>
		''' <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		Public Overrides Function parseObject(ByVal source As String, ByVal pos As ParsePosition) As Object
			Return parse(source, pos)
		End Function

		''' <summary>
		''' Constant for full style pattern.
		''' </summary>
		Public Const FULL As Integer = 0
		''' <summary>
		''' Constant for long style pattern.
		''' </summary>
		Public Const [LONG] As Integer = 1
		''' <summary>
		''' Constant for medium style pattern.
		''' </summary>
		Public Const MEDIUM As Integer = 2
		''' <summary>
		''' Constant for short style pattern.
		''' </summary>
		Public Const [SHORT] As Integer = 3
		''' <summary>
		''' Constant for default style pattern.  Its value is MEDIUM.
		''' </summary>
		Public Const [DEFAULT] As Integer = MEDIUM

		''' <summary>
		''' Gets the time formatter with the default formatting style
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getTimeInstance(int, Locale) getTimeInstance(DEFAULT,
		'''     Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a time formatter. </returns>
		Public Property Shared timeInstance As DateFormat
			Get
				Return [get]([DEFAULT], 0, 1, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets the time formatter with the given formatting style
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getTimeInstance(int, Locale) getTimeInstance(style,
		'''     Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <param name="style"> the given formatting style. For example,
		''' SHORT for "h:mm a" in the US locale. </param>
		''' <returns> a time formatter. </returns>
		Public Shared Function getTimeInstance(ByVal style As Integer) As DateFormat
			Return [get](style, 0, 1, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Function

		''' <summary>
		''' Gets the time formatter with the given formatting style
		''' for the given locale. </summary>
		''' <param name="style"> the given formatting style. For example,
		''' SHORT for "h:mm a" in the US locale. </param>
		''' <param name="aLocale"> the given locale. </param>
		''' <returns> a time formatter. </returns>
		Public Shared Function getTimeInstance(ByVal style As Integer, ByVal aLocale As java.util.Locale) As DateFormat
			Return [get](style, 0, 1, aLocale)
		End Function

		''' <summary>
		''' Gets the date formatter with the default formatting style
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getDateInstance(int, Locale) getDateInstance(DEFAULT,
		'''     Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a date formatter. </returns>
		Public Property Shared dateInstance As DateFormat
			Get
				Return [get](0, [DEFAULT], 2, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets the date formatter with the given formatting style
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getDateInstance(int, Locale) getDateInstance(style,
		'''     Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <param name="style"> the given formatting style. For example,
		''' SHORT for "M/d/yy" in the US locale. </param>
		''' <returns> a date formatter. </returns>
		Public Shared Function getDateInstance(ByVal style As Integer) As DateFormat
			Return [get](0, style, 2, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Function

		''' <summary>
		''' Gets the date formatter with the given formatting style
		''' for the given locale. </summary>
		''' <param name="style"> the given formatting style. For example,
		''' SHORT for "M/d/yy" in the US locale. </param>
		''' <param name="aLocale"> the given locale. </param>
		''' <returns> a date formatter. </returns>
		Public Shared Function getDateInstance(ByVal style As Integer, ByVal aLocale As java.util.Locale) As DateFormat
			Return [get](0, style, 2, aLocale)
		End Function

		''' <summary>
		''' Gets the date/time formatter with the default formatting style
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getDateTimeInstance(int, int, Locale) getDateTimeInstance(DEFAULT,
		'''     DEFAULT, Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a date/time formatter. </returns>
		Public Property Shared dateTimeInstance As DateFormat
			Get
				Return [get]([DEFAULT], [DEFAULT], 3, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets the date/time formatter with the given date and time
		''' formatting styles for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getDateTimeInstance(int, int, Locale) getDateTimeInstance(dateStyle,
		'''     timeStyle, Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <param name="dateStyle"> the given date formatting style. For example,
		''' SHORT for "M/d/yy" in the US locale. </param>
		''' <param name="timeStyle"> the given time formatting style. For example,
		''' SHORT for "h:mm a" in the US locale. </param>
		''' <returns> a date/time formatter. </returns>
		Public Shared Function getDateTimeInstance(ByVal dateStyle As Integer, ByVal timeStyle As Integer) As DateFormat
			Return [get](timeStyle, dateStyle, 3, java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Function

		''' <summary>
		''' Gets the date/time formatter with the given formatting styles
		''' for the given locale. </summary>
		''' <param name="dateStyle"> the given date formatting style. </param>
		''' <param name="timeStyle"> the given time formatting style. </param>
		''' <param name="aLocale"> the given locale. </param>
		''' <returns> a date/time formatter. </returns>
		Public Shared Function getDateTimeInstance(ByVal dateStyle As Integer, ByVal timeStyle As Integer, ByVal aLocale As java.util.Locale) As DateFormat
			Return [get](timeStyle, dateStyle, 3, aLocale)
		End Function

		''' <summary>
		''' Get a default date/time formatter that uses the SHORT style for both the
		''' date and the time.
		''' </summary>
		''' <returns> a date/time formatter </returns>
		Public Property Shared instance As DateFormat
			Get
				Return getDateTimeInstance([SHORT], [SHORT])
			End Get
		End Property

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>get*Instance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported by the Java
		''' runtime and by installed
		''' <seealso cref="java.text.spi.DateFormatProvider DateFormatProvider"/> implementations.
		''' It must contain at least a <code>Locale</code> instance equal to
		''' <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>DateFormat</code> instances are available. </returns>
		Public Property Shared availableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.DateFormatProvider))
				Return pool.availableLocales
			End Get
		End Property

		''' <summary>
		''' Set the calendar to be used by this date format.  Initially, the default
		''' calendar for the specified or default locale is used.
		''' 
		''' <p>Any <seealso cref="java.util.TimeZone TimeZone"/> and {@linkplain
		''' #isLenient() leniency} values that have previously been set are
		''' overwritten by {@code newCalendar}'s values.
		''' </summary>
		''' <param name="newCalendar"> the new {@code Calendar} to be used by the date format </param>
		Public Overridable Property calendar As DateTime?
			Set(ByVal newCalendar As DateTime?)
				Me.calendar = newCalendar
			End Set
			Get
				Return calendar
			End Get
		End Property


		''' <summary>
		''' Allows you to set the number formatter. </summary>
		''' <param name="newNumberFormat"> the given new NumberFormat. </param>
		Public Overridable Property numberFormat As NumberFormat
			Set(ByVal newNumberFormat As NumberFormat)
				Me.numberFormat = newNumberFormat
			End Set
			Get
				Return numberFormat
			End Get
		End Property


		''' <summary>
		''' Sets the time zone for the calendar of this {@code DateFormat} object.
		''' This method is equivalent to the following call.
		''' <blockquote><pre>{@code
		''' getCalendar().setTimeZone(zone)
		''' }</pre></blockquote>
		''' 
		''' <p>The {@code TimeZone} set by this method is overwritten by a
		''' <seealso cref="#setCalendar(java.util.Calendar) setCalendar"/> call.
		''' 
		''' <p>The {@code TimeZone} set by this method may be overwritten as
		''' a result of a call to the parse method.
		''' </summary>
		''' <param name="zone"> the given new time zone. </param>
		Public Overridable Property timeZone As java.util.TimeZone
			Set(ByVal zone As java.util.TimeZone)
				calendar.Value.timeZone = zone
			End Set
			Get
				Return calendar.Value.timeZone
			End Get
		End Property


		''' <summary>
		''' Specify whether or not date/time parsing is to be lenient.  With
		''' lenient parsing, the parser may use heuristics to interpret inputs that
		''' do not precisely match this object's format.  With strict parsing,
		''' inputs must match this object's format.
		''' 
		''' <p>This method is equivalent to the following call.
		''' <blockquote><pre>{@code
		''' getCalendar().setLenient(lenient)
		''' }</pre></blockquote>
		''' 
		''' <p>This leniency value is overwritten by a call to {@link
		''' #setCalendar(java.util.Calendar) setCalendar()}.
		''' </summary>
		''' <param name="lenient"> when {@code true}, parsing is lenient </param>
		''' <seealso cref= java.util.Calendar#setLenient(boolean) </seealso>
		Public Overridable Property lenient As Boolean
			Set(ByVal lenient As Boolean)
				calendar.Value.lenient = lenient
			End Set
			Get
				Return calendar.Value.lenient
			End Get
		End Property


		''' <summary>
		''' Overrides hashCode
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return numberFormat.GetHashCode()
			' just enough fields for a reasonable distribution
		End Function

		''' <summary>
		''' Overrides equals
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If obj Is Nothing OrElse Me.GetType() IsNot obj.GetType() Then Return False
			Dim other As DateFormat = CType(obj, DateFormat)
			Return (calendar.Value.firstDayOfWeek = other.calendar.Value.Value.firstDayOfWeek AndAlso calendar.Value.minimalDaysInFirstWeek = other.calendar.Value.Value.minimalDaysInFirstWeek AndAlso calendar.Value.lenient = other.calendar.Value.Value.lenient AndAlso calendar.Value.timeZone.Equals(other.calendar.Value.Value.timeZone) AndAlso numberFormat.Equals(other.numberFormat)) ' calendar.equivalentTo(other.calendar) // THIS API DOESN'T EXIST YET!
		End Function

		''' <summary>
		''' Overrides Cloneable
		''' </summary>
		Public Overrides Function clone() As Object
			Dim other As DateFormat = CType(MyBase.clone(), DateFormat)
			other.calendar = CType(calendar.Value.clone(), DateTime?)
			other.numberFormat = CType(numberFormat.clone(), NumberFormat)
			Return other
		End Function

		''' <summary>
		''' Creates a DateFormat with the given time and/or date style in the given
		''' locale. </summary>
		''' <param name="timeStyle"> a value from 0 to 3 indicating the time format,
		''' ignored if flags is 2 </param>
		''' <param name="dateStyle"> a value from 0 to 3 indicating the time format,
		''' ignored if flags is 1 </param>
		''' <param name="flags"> either 1 for a time format, 2 for a date format,
		''' or 3 for a date/time format </param>
		''' <param name="loc"> the locale for the format </param>
		Private Shared Function [get](ByVal timeStyle As Integer, ByVal dateStyle As Integer, ByVal flags As Integer, ByVal loc As java.util.Locale) As DateFormat
			If (flags And 1) <> 0 Then
				If timeStyle < 0 OrElse timeStyle > 3 Then Throw New IllegalArgumentException("Illegal time style " & timeStyle)
			Else
				timeStyle = -1
			End If
			If (flags And 2) <> 0 Then
				If dateStyle < 0 OrElse dateStyle > 3 Then Throw New IllegalArgumentException("Illegal date style " & dateStyle)
			Else
				dateStyle = -1
			End If

			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DateFormatProvider), loc)
			Dim dateFormat_Renamed As DateFormat = [get](adapter, timeStyle, dateStyle, loc)
			If dateFormat_Renamed Is Nothing Then dateFormat_Renamed = [get](sun.util.locale.provider.LocaleProviderAdapter.forJRE(), timeStyle, dateStyle, loc)
			Return dateFormat_Renamed
		End Function

		Private Shared Function [get](ByVal adapter As sun.util.locale.provider.LocaleProviderAdapter, ByVal timeStyle As Integer, ByVal dateStyle As Integer, ByVal loc As java.util.Locale) As DateFormat
			Dim provider As java.text.spi.DateFormatProvider = adapter.dateFormatProvider
			Dim dateFormat_Renamed As DateFormat
			If timeStyle = -1 Then
				dateFormat_Renamed = provider.getDateInstance(dateStyle, loc)
			Else
				If dateStyle = -1 Then
					dateFormat_Renamed = provider.getTimeInstance(timeStyle, loc)
				Else
					dateFormat_Renamed = provider.getDateTimeInstance(dateStyle, timeStyle, loc)
				End If
			End If
			Return dateFormat_Renamed
		End Function

		''' <summary>
		''' Create a new date format.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Defines constants that are used as attribute keys in the
		''' <code>AttributedCharacterIterator</code> returned
		''' from <code>DateFormat.formatToCharacterIterator</code> and as
		''' field identifiers in <code>FieldPosition</code>.
		''' <p>
		''' The class also provides two methods to map
		''' between its constants and the corresponding Calendar constants.
		''' 
		''' @since 1.4 </summary>
		''' <seealso cref= java.util.Calendar </seealso>
		Public Class Field
			Inherits Format.Field

			' Proclaim serial compatibility with 1.4 FCS
			Private Const serialVersionUID As Long = 7441350119349544720L

			' table of all instances in this class, used by readResolve
			Private Shared ReadOnly instanceMap As IDictionary(Of String, Field) = New Dictionary(Of String, Field)(18)
			' Maps from Calendar constant (such as Calendar.ERA) to Field
			' constant (such as Field.ERA).
			Private Shared ReadOnly calendarToFieldMapping As Field() = New Field(DateTime.FIELD_COUNT - 1){}

			''' <summary>
			''' Calendar field. </summary>
			Private calendarField As Integer

			''' <summary>
			''' Returns the <code>Field</code> constant that corresponds to
			''' the <code>Calendar</code> constant <code>calendarField</code>.
			''' If there is no direct mapping between the <code>Calendar</code>
			''' constant and a <code>Field</code>, null is returned.
			''' </summary>
			''' <exception cref="IllegalArgumentException"> if <code>calendarField</code> is
			'''         not the value of a <code>Calendar</code> field constant. </exception>
			''' <param name="calendarField"> Calendar field constant </param>
			''' <returns> Field instance representing calendarField. </returns>
			''' <seealso cref= java.util.Calendar </seealso>
			Public Shared Function ofCalendarField(ByVal calendarField As Integer) As Field
				If calendarField < 0 OrElse calendarField >= calendarToFieldMapping.Length Then Throw New IllegalArgumentException("Unknown Calendar constant " & calendarField)
				Return calendarToFieldMapping(calendarField)
			End Function

			''' <summary>
			''' Creates a <code>Field</code>.
			''' </summary>
			''' <param name="name"> the name of the <code>Field</code> </param>
			''' <param name="calendarField"> the <code>Calendar</code> constant this
			'''        <code>Field</code> corresponds to; any value, even one
			'''        outside the range of legal <code>Calendar</code> values may
			'''        be used, but <code>-1</code> should be used for values
			'''        that don't correspond to legal <code>Calendar</code> values </param>
			Protected Friend Sub New(ByVal name As String, ByVal calendarField As Integer)
				MyBase.New(name)
				Me.calendarField = calendarField
				If Me.GetType() Is GetType(DateFormat.Field) Then
					instanceMap(name) = Me
					If calendarField >= 0 Then calendarToFieldMapping(calendarField) = Me
				End If
			End Sub

			''' <summary>
			''' Returns the <code>Calendar</code> field associated with this
			''' attribute. For example, if this represents the hours field of
			''' a <code>Calendar</code>, this would return
			''' <code>Calendar.HOUR</code>. If there is no corresponding
			''' <code>Calendar</code> constant, this will return -1.
			''' </summary>
			''' <returns> Calendar constant for this field </returns>
			''' <seealso cref= java.util.Calendar </seealso>
			Public Overridable Property calendarField As Integer
				Get
					Return calendarField
				End Get
			End Property

			''' <summary>
			''' Resolves instances being deserialized to the predefined constants.
			''' </summary>
			''' <exception cref="InvalidObjectException"> if the constant could not be
			'''         resolved. </exception>
			''' <returns> resolved DateFormat.Field constant </returns>
			Protected Friend Overrides Function readResolve() As Object
				If Me.GetType() IsNot GetType(DateFormat.Field) Then Throw New java.io.InvalidObjectException("subclass didn't correctly implement readResolve")

				Dim instance As Object = instanceMap(name)
				If instance IsNot Nothing Then
					Return instance
				Else
					Throw New java.io.InvalidObjectException("unknown attribute name")
				End If
			End Function

			'
			' The constants
			'

			''' <summary>
			''' Constant identifying the era field.
			''' </summary>
			Public Shared ReadOnly ERA As New Field("era", DateTime.ERA)

			''' <summary>
			''' Constant identifying the year field.
			''' </summary>
			Public Shared ReadOnly YEAR As New Field("year", DateTime.YEAR)

			''' <summary>
			''' Constant identifying the month field.
			''' </summary>
			Public Shared ReadOnly MONTH As New Field("month", DateTime.MONTH)

			''' <summary>
			''' Constant identifying the day of month field.
			''' </summary>
			Public Shared ReadOnly DAY_OF_MONTH As New Field("day of month", DateTime.DAY_OF_MONTH)

			''' <summary>
			''' Constant identifying the hour of day field, where the legal values
			''' are 1 to 24.
			''' </summary>
			Public Shared ReadOnly HOUR_OF_DAY1 As New Field("hour of day 1",-1)

			''' <summary>
			''' Constant identifying the hour of day field, where the legal values
			''' are 0 to 23.
			''' </summary>
			Public Shared ReadOnly HOUR_OF_DAY0 As New Field("hour of day", DateTime.HOUR_OF_DAY)

			''' <summary>
			''' Constant identifying the minute field.
			''' </summary>
			Public Shared ReadOnly MINUTE As New Field("minute", DateTime.MINUTE)

			''' <summary>
			''' Constant identifying the second field.
			''' </summary>
			Public Shared ReadOnly SECOND As New Field("second", DateTime.SECOND)

			''' <summary>
			''' Constant identifying the millisecond field.
			''' </summary>
			Public Shared ReadOnly MILLISECOND As New Field("millisecond", DateTime.MILLISECOND)

			''' <summary>
			''' Constant identifying the day of week field.
			''' </summary>
			Public Shared ReadOnly DAY_OF_WEEK As New Field("day of week", DateTime.DAY_OF_WEEK)

			''' <summary>
			''' Constant identifying the day of year field.
			''' </summary>
			Public Shared ReadOnly DAY_OF_YEAR As New Field("day of year", DateTime.DAY_OF_YEAR)

			''' <summary>
			''' Constant identifying the day of week field.
			''' </summary>
			Public Shared ReadOnly DAY_OF_WEEK_IN_MONTH As New Field("day of week in month", DateTime.DAY_OF_WEEK_IN_MONTH)

			''' <summary>
			''' Constant identifying the week of year field.
			''' </summary>
			Public Shared ReadOnly WEEK_OF_YEAR As New Field("week of year", DateTime.WEEK_OF_YEAR)

			''' <summary>
			''' Constant identifying the week of month field.
			''' </summary>
			Public Shared ReadOnly WEEK_OF_MONTH As New Field("week of month", DateTime.WEEK_OF_MONTH)

			''' <summary>
			''' Constant identifying the time of day indicator
			''' (e.g. "a.m." or "p.m.") field.
			''' </summary>
			Public Shared ReadOnly AM_PM As New Field("am pm", DateTime.AM_PM)

			''' <summary>
			''' Constant identifying the hour field, where the legal values are
			''' 1 to 12.
			''' </summary>
			Public Shared ReadOnly HOUR1 As New Field("hour 1", -1)

			''' <summary>
			''' Constant identifying the hour field, where the legal values are
			''' 0 to 11.
			''' </summary>
			Public Shared ReadOnly HOUR0 As New Field("hour", DateTime.HOUR)

			''' <summary>
			''' Constant identifying the time zone field.
			''' </summary>
			Public Shared ReadOnly TIME_ZONE As New Field("time zone", -1)
		End Class
	End Class

End Namespace