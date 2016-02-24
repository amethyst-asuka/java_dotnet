Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>TimeZone</code> represents a time zone offset, and also figures out daylight
	''' savings.
	''' 
	''' <p>
	''' Typically, you get a <code>TimeZone</code> using <code>getDefault</code>
	''' which creates a <code>TimeZone</code> based on the time zone where the program
	''' is running. For example, for a program running in Japan, <code>getDefault</code>
	''' creates a <code>TimeZone</code> object based on Japanese Standard Time.
	''' 
	''' <p>
	''' You can also get a <code>TimeZone</code> using <code>getTimeZone</code>
	''' along with a time zone ID. For instance, the time zone ID for the
	''' U.S. Pacific Time zone is "America/Los_Angeles". So, you can get a
	''' U.S. Pacific Time <code>TimeZone</code> object with:
	''' <blockquote><pre>
	''' TimeZone tz = TimeZone.getTimeZone("America/Los_Angeles");
	''' </pre></blockquote>
	''' You can use the <code>getAvailableIDs</code> method to iterate through
	''' all the supported time zone IDs. You can then choose a
	''' supported ID to get a <code>TimeZone</code>.
	''' If the time zone you want is not represented by one of the
	''' supported IDs, then a custom time zone ID can be specified to
	''' produce a TimeZone. The syntax of a custom time zone ID is:
	''' 
	''' <blockquote><pre>
	''' <a name="CustomID"><i>CustomID:</i></a>
	'''         <code>GMT</code> <i>Sign</i> <i>Hours</i> <code>:</code> <i>Minutes</i>
	'''         <code>GMT</code> <i>Sign</i> <i>Hours</i> <i>Minutes</i>
	'''         <code>GMT</code> <i>Sign</i> <i>Hours</i>
	''' <i>Sign:</i> one of
	'''         <code>+ -</code>
	''' <i>Hours:</i>
	'''         <i>Digit</i>
	'''         <i>Digit</i> <i>Digit</i>
	''' <i>Minutes:</i>
	'''         <i>Digit</i> <i>Digit</i>
	''' <i>Digit:</i> one of
	'''         <code>0 1 2 3 4 5 6 7 8 9</code>
	''' </pre></blockquote>
	''' 
	''' <i>Hours</i> must be between 0 to 23 and <i>Minutes</i> must be
	''' between 00 to 59.  For example, "GMT+10" and "GMT+0010" mean ten
	''' hours and ten minutes ahead of GMT, respectively.
	''' <p>
	''' The format is locale independent and digits must be taken from the
	''' Basic Latin block of the Unicode standard. No daylight saving time
	''' transition schedule can be specified with a custom time zone ID. If
	''' the specified string doesn't match the syntax, <code>"GMT"</code>
	''' is used.
	''' <p>
	''' When creating a <code>TimeZone</code>, the specified custom time
	''' zone ID is normalized in the following syntax:
	''' <blockquote><pre>
	''' <a name="NormalizedCustomID"><i>NormalizedCustomID:</i></a>
	'''         <code>GMT</code> <i>Sign</i> <i>TwoDigitHours</i> <code>:</code> <i>Minutes</i>
	''' <i>Sign:</i> one of
	'''         <code>+ -</code>
	''' <i>TwoDigitHours:</i>
	'''         <i>Digit</i> <i>Digit</i>
	''' <i>Minutes:</i>
	'''         <i>Digit</i> <i>Digit</i>
	''' <i>Digit:</i> one of
	'''         <code>0 1 2 3 4 5 6 7 8 9</code>
	''' </pre></blockquote>
	''' For example, TimeZone.getTimeZone("GMT-8").getID() returns "GMT-08:00".
	''' 
	''' <h3>Three-letter time zone IDs</h3>
	''' 
	''' For compatibility with JDK 1.1.x, some other three-letter time zone IDs
	''' (such as "PST", "CTT", "AST") are also supported. However, <strong>their
	''' use is deprecated</strong> because the same abbreviation is often used
	''' for multiple time zones (for example, "CST" could be U.S. "Central Standard
	''' Time" and "China Standard Time"), and the Java platform can then only
	''' recognize one of them.
	''' 
	''' </summary>
	''' <seealso cref=          Calendar </seealso>
	''' <seealso cref=          GregorianCalendar </seealso>
	''' <seealso cref=          SimpleTimeZone
	''' @author       Mark Davis, David Goldsmith, Chen-Lieh Huang, Alan Liu
	''' @since        JDK1.1 </seealso>
	<Serializable> _
	Public MustInherit Class TimeZone
		Implements Cloneable

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' A style specifier for <code>getDisplayName()</code> indicating
		''' a short name, such as "PST." </summary>
		''' <seealso cref= #LONG
		''' @since 1.2 </seealso>
		Public Const [SHORT] As Integer = 0

		''' <summary>
		''' A style specifier for <code>getDisplayName()</code> indicating
		''' a long name, such as "Pacific Standard Time." </summary>
		''' <seealso cref= #SHORT
		''' @since 1.2 </seealso>
		Public Const [LONG] As Integer = 1

		' Constants used internally; unit is milliseconds
		Private Const ONE_MINUTE As Integer = 60*1000
		Private Shared ReadOnly ONE_HOUR As Integer = 60*ONE_MINUTE
		Private Shared ReadOnly ONE_DAY As Integer = 24*ONE_HOUR

		' Proclaim serialization compatibility with JDK 1.1
		Friend Const serialVersionUID As Long = 3581463369166924961L

		''' <summary>
		''' Gets the time zone offset, for current date, modified in case of
		''' daylight savings. This is the offset to add to UTC to get local time.
		''' <p>
		''' This method returns a historically correct offset if an
		''' underlying <code>TimeZone</code> implementation subclass
		''' supports historical Daylight Saving Time schedule and GMT
		''' offset changes.
		''' </summary>
		''' <param name="era"> the era of the given date. </param>
		''' <param name="year"> the year in the given date. </param>
		''' <param name="month"> the month in the given date.
		''' Month is 0-based. e.g., 0 for January. </param>
		''' <param name="day"> the day-in-month of the given date. </param>
		''' <param name="dayOfWeek"> the day-of-week of the given date. </param>
		''' <param name="milliseconds"> the milliseconds in day in <em>standard</em>
		''' local time.
		''' </param>
		''' <returns> the offset in milliseconds to add to GMT to get local time.
		''' </returns>
		''' <seealso cref= Calendar#ZONE_OFFSET </seealso>
		''' <seealso cref= Calendar#DST_OFFSET </seealso>
		Public MustOverride Function getOffset(ByVal era As Integer, ByVal year As Integer, ByVal month As Integer, ByVal day As Integer, ByVal dayOfWeek As Integer, ByVal milliseconds As Integer) As Integer

		''' <summary>
		''' Returns the offset of this time zone from UTC at the specified
		''' date. If Daylight Saving Time is in effect at the specified
		''' date, the offset value is adjusted with the amount of daylight
		''' saving.
		''' <p>
		''' This method returns a historically correct offset value if an
		''' underlying TimeZone implementation subclass supports historical
		''' Daylight Saving Time schedule and GMT offset changes.
		''' </summary>
		''' <param name="date"> the date represented in milliseconds since January 1, 1970 00:00:00 GMT </param>
		''' <returns> the amount of time in milliseconds to add to UTC to get local time.
		''' </returns>
		''' <seealso cref= Calendar#ZONE_OFFSET </seealso>
		''' <seealso cref= Calendar#DST_OFFSET
		''' @since 1.4 </seealso>
		Public Overridable Function getOffset(ByVal [date] As Long) As Integer
			If inDaylightTime(New Date(date_Renamed)) Then Return rawOffset + dSTSavings
			Return rawOffset
		End Function

		''' <summary>
		''' Gets the raw GMT offset and the amount of daylight saving of this
		''' time zone at the given time. </summary>
		''' <param name="date"> the milliseconds (since January 1, 1970,
		''' 00:00:00.000 GMT) at which the time zone offset and daylight
		''' saving amount are found </param>
		''' <param name="offsets"> an array of int where the raw GMT offset
		''' (offset[0]) and daylight saving amount (offset[1]) are stored,
		''' or null if those values are not needed. The method assumes that
		''' the length of the given array is two or larger. </param>
		''' <returns> the total amount of the raw GMT offset and daylight
		''' saving at the specified date.
		''' </returns>
		''' <seealso cref= Calendar#ZONE_OFFSET </seealso>
		''' <seealso cref= Calendar#DST_OFFSET </seealso>
		Friend Overridable Function getOffsets(ByVal [date] As Long, ByVal offsets As Integer()) As Integer
			Dim rawoffset_Renamed As Integer = rawOffset
			Dim dstoffset As Integer = 0
			If inDaylightTime(New Date(date_Renamed)) Then dstoffset = dSTSavings
			If offsets IsNot Nothing Then
				offsets(0) = rawoffset_Renamed
				offsets(1) = dstoffset
			End If
			Return rawoffset_Renamed + dstoffset
		End Function

		''' <summary>
		''' Sets the base time zone offset to GMT.
		''' This is the offset to add to UTC to get local time.
		''' <p>
		''' If an underlying <code>TimeZone</code> implementation subclass
		''' supports historical GMT offset changes, the specified GMT
		''' offset is set as the latest GMT offset and the difference from
		''' the known latest GMT offset value is used to adjust all
		''' historical GMT offset values.
		''' </summary>
		''' <param name="offsetMillis"> the given base time zone offset to GMT. </param>
		Public MustOverride Property rawOffset As Integer


		''' <summary>
		''' Gets the ID of this time zone. </summary>
		''' <returns> the ID of this time zone. </returns>
		Public Overridable Property iD As String
			Get
				Return ID
			End Get
			Set(ByVal ID As String)
				If ID Is Nothing Then Throw New NullPointerException
				Me.ID = ID
			End Set
		End Property


		''' <summary>
		''' Returns a long standard time name of this {@code TimeZone} suitable for
		''' presentation to the user in the default locale.
		''' 
		''' <p>This method is equivalent to:
		''' <blockquote><pre>
		''' getDisplayName(false, <seealso cref="#LONG"/>,
		'''                Locale.getDefault(<seealso cref="Locale.Category#DISPLAY"/>))
		''' </pre></blockquote>
		''' </summary>
		''' <returns> the human-readable name of this time zone in the default locale.
		''' @since 1.2 </returns>
		''' <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		''' <seealso cref= Locale#getDefault(Locale.Category) </seealso>
		''' <seealso cref= Locale.Category </seealso>
		Public Property displayName As String
			Get
				Return getDisplayName(False, [LONG], Locale.getDefault(Locale.Category.DISPLAY))
			End Get
		End Property

		''' <summary>
		''' Returns a long standard time name of this {@code TimeZone} suitable for
		''' presentation to the user in the specified {@code locale}.
		''' 
		''' <p>This method is equivalent to:
		''' <blockquote><pre>
		''' getDisplayName(false, <seealso cref="#LONG"/>, locale)
		''' </pre></blockquote>
		''' </summary>
		''' <param name="locale"> the locale in which to supply the display name. </param>
		''' <returns> the human-readable name of this time zone in the given locale. </returns>
		''' <exception cref="NullPointerException"> if {@code locale} is {@code null}.
		''' @since 1.2 </exception>
		''' <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		Public Function getDisplayName(ByVal locale_Renamed As Locale) As String
			Return getDisplayName(False, [LONG], locale_Renamed)
		End Function

		''' <summary>
		''' Returns a name in the specified {@code style} of this {@code TimeZone}
		''' suitable for presentation to the user in the default locale. If the
		''' specified {@code daylight} is {@code true}, a Daylight Saving Time name
		''' is returned (even if this {@code TimeZone} doesn't observe Daylight Saving
		''' Time). Otherwise, a Standard Time name is returned.
		''' 
		''' <p>This method is equivalent to:
		''' <blockquote><pre>
		''' getDisplayName(daylight, style,
		'''                Locale.getDefault(<seealso cref="Locale.Category#DISPLAY"/>))
		''' </pre></blockquote>
		''' </summary>
		''' <param name="daylight"> {@code true} specifying a Daylight Saving Time name, or
		'''                 {@code false} specifying a Standard Time name </param>
		''' <param name="style"> either <seealso cref="#LONG"/> or <seealso cref="#SHORT"/> </param>
		''' <returns> the human-readable name of this time zone in the default locale. </returns>
		''' <exception cref="IllegalArgumentException"> if {@code style} is invalid.
		''' @since 1.2 </exception>
		''' <seealso cref= #getDisplayName(boolean, int, Locale) </seealso>
		''' <seealso cref= Locale#getDefault(Locale.Category) </seealso>
		''' <seealso cref= Locale.Category </seealso>
		''' <seealso cref= java.text.DateFormatSymbols#getZoneStrings() </seealso>
		Public Function getDisplayName(ByVal daylight As Boolean, ByVal style As Integer) As String
			Return getDisplayName(daylight, style, Locale.getDefault(Locale.Category.DISPLAY))
		End Function

		''' <summary>
		''' Returns a name in the specified {@code style} of this {@code TimeZone}
		''' suitable for presentation to the user in the specified {@code
		''' locale}. If the specified {@code daylight} is {@code true}, a Daylight
		''' Saving Time name is returned (even if this {@code TimeZone} doesn't
		''' observe Daylight Saving Time). Otherwise, a Standard Time name is
		''' returned.
		''' 
		''' <p>When looking up a time zone name, the {@linkplain
		''' ResourceBundle.Control#getCandidateLocales(String,Locale) default
		''' <code>Locale</code> search path of <code>ResourceBundle</code>} derived
		''' from the specified {@code locale} is used. (No {@linkplain
		''' ResourceBundle.Control#getFallbackLocale(String,Locale) fallback
		''' <code>Locale</code>} search is performed.) If a time zone name in any
		''' {@code Locale} of the search path, including <seealso cref="Locale#ROOT"/>, is
		''' found, the name is returned. Otherwise, a string in the
		''' <a href="#NormalizedCustomID">normalized custom ID format</a> is returned.
		''' </summary>
		''' <param name="daylight"> {@code true} specifying a Daylight Saving Time name, or
		'''                 {@code false} specifying a Standard Time name </param>
		''' <param name="style"> either <seealso cref="#LONG"/> or <seealso cref="#SHORT"/> </param>
		''' <param name="locale">   the locale in which to supply the display name. </param>
		''' <returns> the human-readable name of this time zone in the given locale. </returns>
		''' <exception cref="IllegalArgumentException"> if {@code style} is invalid. </exception>
		''' <exception cref="NullPointerException"> if {@code locale} is {@code null}.
		''' @since 1.2 </exception>
		''' <seealso cref= java.text.DateFormatSymbols#getZoneStrings() </seealso>
		Public Overridable Function getDisplayName(ByVal daylight As Boolean, ByVal style As Integer, ByVal locale_Renamed As Locale) As String
			If style <> [SHORT] AndAlso style <> [LONG] Then Throw New IllegalArgumentException("Illegal style: " & style)
			Dim id_Renamed As String = iD
			Dim name As String = sun.util.locale.provider.TimeZoneNameUtility.retrieveDisplayName(id_Renamed, daylight, style, locale_Renamed)
			If name IsNot Nothing Then Return name

			If id_Renamed.StartsWith("GMT") AndAlso id_Renamed.length() > 3 Then
				Dim sign As Char = id_Renamed.Chars(3)
				If sign = "+"c OrElse sign = "-"c Then Return id_Renamed
			End If
			Dim offset_Renamed As Integer = rawOffset
			If daylight Then offset_Renamed += dSTSavings
			Return sun.util.calendar.ZoneInfoFile.toCustomID(offset_Renamed)
		End Function

		Private Shared Function getDisplayNames(ByVal id As String, ByVal locale_Renamed As Locale) As String()
			Return sun.util.locale.provider.TimeZoneNameUtility.retrieveDisplayNames(id, locale_Renamed)
		End Function

		''' <summary>
		''' Returns the amount of time to be added to local standard time
		''' to get local wall clock time.
		''' 
		''' <p>The default implementation returns 3600000 milliseconds
		''' (i.e., one hour) if a call to <seealso cref="#useDaylightTime()"/>
		''' returns {@code true}. Otherwise, 0 (zero) is returned.
		''' 
		''' <p>If an underlying {@code TimeZone} implementation subclass
		''' supports historical and future Daylight Saving Time schedule
		''' changes, this method returns the amount of saving time of the
		''' last known Daylight Saving Time rule that can be a future
		''' prediction.
		''' 
		''' <p>If the amount of saving time at any given time stamp is
		''' required, construct a <seealso cref="Calendar"/> with this {@code
		''' TimeZone} and the time stamp, and call {@link Calendar#get(int)
		''' Calendar.get}{@code (}<seealso cref="Calendar#DST_OFFSET"/>{@code )}.
		''' </summary>
		''' <returns> the amount of saving time in milliseconds
		''' @since 1.4 </returns>
		''' <seealso cref= #inDaylightTime(Date) </seealso>
		''' <seealso cref= #getOffset(long) </seealso>
		''' <seealso cref= #getOffset(int,int,int,int,int,int) </seealso>
		''' <seealso cref= Calendar#ZONE_OFFSET </seealso>
		Public Overridable Property dSTSavings As Integer
			Get
				If useDaylightTime() Then Return 3600000
				Return 0
			End Get
		End Property

		''' <summary>
		''' Queries if this {@code TimeZone} uses Daylight Saving Time.
		''' 
		''' <p>If an underlying {@code TimeZone} implementation subclass
		''' supports historical and future Daylight Saving Time schedule
		''' changes, this method refers to the last known Daylight Saving Time
		''' rule that can be a future prediction and may not be the same as
		''' the current rule. Consider calling <seealso cref="#observesDaylightTime()"/>
		''' if the current rule should also be taken into account.
		''' </summary>
		''' <returns> {@code true} if this {@code TimeZone} uses Daylight Saving Time,
		'''         {@code false}, otherwise. </returns>
		''' <seealso cref= #inDaylightTime(Date) </seealso>
		''' <seealso cref= Calendar#DST_OFFSET </seealso>
		Public MustOverride Function useDaylightTime() As Boolean

		''' <summary>
		''' Returns {@code true} if this {@code TimeZone} is currently in
		''' Daylight Saving Time, or if a transition from Standard Time to
		''' Daylight Saving Time occurs at any future time.
		''' 
		''' <p>The default implementation returns {@code true} if
		''' {@code useDaylightTime()} or {@code inDaylightTime(new Date())}
		''' returns {@code true}.
		''' </summary>
		''' <returns> {@code true} if this {@code TimeZone} is currently in
		''' Daylight Saving Time, or if a transition from Standard Time to
		''' Daylight Saving Time occurs at any future time; {@code false}
		''' otherwise.
		''' @since 1.7 </returns>
		''' <seealso cref= #useDaylightTime() </seealso>
		''' <seealso cref= #inDaylightTime(Date) </seealso>
		''' <seealso cref= Calendar#DST_OFFSET </seealso>
		Public Overridable Function observesDaylightTime() As Boolean
			Return useDaylightTime() OrElse inDaylightTime(DateTime.Now)
		End Function

		''' <summary>
		''' Queries if the given {@code date} is in Daylight Saving Time in
		''' this time zone.
		''' </summary>
		''' <param name="date"> the given Date. </param>
		''' <returns> {@code true} if the given date is in Daylight Saving Time,
		'''         {@code false}, otherwise. </returns>
		Public MustOverride Function inDaylightTime(ByVal [date] As Date) As Boolean

		''' <summary>
		''' Gets the <code>TimeZone</code> for the given ID.
		''' </summary>
		''' <param name="ID"> the ID for a <code>TimeZone</code>, either an abbreviation
		''' such as "PST", a full name such as "America/Los_Angeles", or a custom
		''' ID such as "GMT-8:00". Note that the support of abbreviations is
		''' for JDK 1.1.x compatibility only and full names should be used.
		''' </param>
		''' <returns> the specified <code>TimeZone</code>, or the GMT zone if the given ID
		''' cannot be understood. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function getTimeZone(ByVal ID As String) As TimeZone
			Return getTimeZone(ID, True)
		End Function

		''' <summary>
		''' Gets the {@code TimeZone} for the given {@code zoneId}.
		''' </summary>
		''' <param name="zoneId"> a <seealso cref="ZoneId"/> from which the time zone ID is obtained </param>
		''' <returns> the specified {@code TimeZone}, or the GMT zone if the given ID
		'''         cannot be understood. </returns>
		''' <exception cref="NullPointerException"> if {@code zoneId} is {@code null}
		''' @since 1.8 </exception>
		Public Shared Function getTimeZone(ByVal zoneId As java.time.ZoneId) As TimeZone
			Dim tzid As String = zoneId.id ' throws an NPE if null
			Dim c As Char = tzid.Chars(0)
			If c = "+"c OrElse c = "-"c Then
				tzid = "GMT" & tzid
			ElseIf c = "Z"c AndAlso tzid.length() = 1 Then
				tzid = "UTC"
			End If
			Return getTimeZone(tzid, True)
		End Function

		''' <summary>
		''' Converts this {@code TimeZone} object to a {@code ZoneId}.
		''' </summary>
		''' <returns> a {@code ZoneId} representing the same time zone as this
		'''         {@code TimeZone}
		''' @since 1.8 </returns>
		Public Overridable Function toZoneId() As java.time.ZoneId
			Dim id_Renamed As String = iD
			If sun.util.calendar.ZoneInfoFile.useOldMapping() AndAlso id_Renamed.length() = 3 Then
				If "EST".Equals(id_Renamed) Then Return java.time.ZoneId.of("America/New_York")
				If "MST".Equals(id_Renamed) Then Return java.time.ZoneId.of("America/Denver")
				If "HST".Equals(id_Renamed) Then Return java.time.ZoneId.of("America/Honolulu")
			End If
			Return java.time.ZoneId.of(id_Renamed, java.time.ZoneId.SHORT_IDS)
		End Function

		Private Shared Function getTimeZone(ByVal ID As String, ByVal fallback As Boolean) As TimeZone
			Dim tz As TimeZone = sun.util.calendar.ZoneInfo.getTimeZone(ID)
			If tz Is Nothing Then
				tz = parseCustomTimeZone(ID)
				If tz Is Nothing AndAlso fallback Then tz = New sun.util.calendar.ZoneInfo(GMT_ID, 0)
			End If
			Return tz
		End Function

		''' <summary>
		''' Gets the available IDs according to the given time zone offset in milliseconds.
		''' </summary>
		''' <param name="rawOffset"> the given time zone GMT offset in milliseconds. </param>
		''' <returns> an array of IDs, where the time zone for that ID has
		''' the specified GMT offset. For example, "America/Phoenix" and "America/Denver"
		''' both have GMT-07:00, but differ in daylight saving behavior. </returns>
		''' <seealso cref= #getRawOffset() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function getAvailableIDs(ByVal rawOffset As Integer) As String()
			Return sun.util.calendar.ZoneInfo.getAvailableIDs(rawOffset)
		End Function

		''' <summary>
		''' Gets all the available IDs supported. </summary>
		''' <returns> an array of IDs. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared availableIDs As String()
			Get
				Return sun.util.calendar.ZoneInfo.availableIDs
			End Get
		End Property

		''' <summary>
		''' Gets the platform defined TimeZone ID.
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getSystemTimeZoneID(ByVal javaHome As String) As String
		End Function

		''' <summary>
		''' Gets the custom time zone ID based on the GMT offset of the
		''' platform. (e.g., "GMT+08:00")
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getSystemGMTOffsetID() As String
		End Function

		''' <summary>
		''' Gets the default {@code TimeZone} of the Java virtual machine. If the
		''' cached default {@code TimeZone} is available, its clone is returned.
		''' Otherwise, the method takes the following steps to determine the default
		''' time zone.
		''' 
		''' <ul>
		''' <li>Use the {@code user.timezone} property value as the default
		''' time zone ID if it's available.</li>
		''' <li>Detect the platform time zone ID. The source of the
		''' platform time zone and ID mapping may vary with implementation.</li>
		''' <li>Use {@code GMT} as the last resort if the given or detected
		''' time zone ID is unknown.</li>
		''' </ul>
		''' 
		''' <p>The default {@code TimeZone} created from the ID is cached,
		''' and its clone is returned. The {@code user.timezone} property
		''' value is set to the ID upon return.
		''' </summary>
		''' <returns> the default {@code TimeZone} </returns>
		''' <seealso cref= #setDefault(TimeZone) </seealso>
		Public Property Shared [default] As TimeZone
			Get
				Return CType(defaultRef.clone(), TimeZone)
			End Get
		End Property

		''' <summary>
		''' Returns the reference to the default TimeZone object. This
		''' method doesn't create a clone.
		''' </summary>
		Shared defaultRef As TimeZone
			Get
				Dim defaultZone_Renamed As TimeZone = defaultTimeZone
				If defaultZone_Renamed Is Nothing Then
					' Need to initialize the default time zone.
					defaultZone_Renamed = defaultZoneone()
					Debug.Assert(defaultZone_Renamed IsNot Nothing)
				End If
				' Don't clone here.
				Return defaultZone_Renamed
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function setDefaultZone() As TimeZone
			Dim tz As TimeZone
			' get the time zone ID from the system properties
			Dim zoneID As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.timezone"))

			' if the time zone ID is not set (yet), perform the
			' platform to Java time zone ID mapping.
			If zoneID Is Nothing OrElse zoneID.empty Then
				Dim javaHome As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.home"))
				Try
					zoneID = getSystemTimeZoneID(javaHome)
					If zoneID Is Nothing Then zoneID = GMT_ID
				Catch e As NullPointerException
					zoneID = GMT_ID
				End Try
			End If

			' Get the time zone for zoneID. But not fall back to
			' "GMT" here.
			tz = getTimeZone(zoneID, False)

			If tz Is Nothing Then
				' If the given zone ID is unknown in Java, try to
				' get the GMT-offset-based time zone ID,
				' a.k.a. custom time zone ID (e.g., "GMT-08:00").
				Dim gmtOffsetID As String = systemGMTOffsetID
				If gmtOffsetID IsNot Nothing Then zoneID = gmtOffsetID
				tz = getTimeZone(zoneID, True)
			End If
			Debug.Assert(tz IsNot Nothing)

			Dim id_Renamed As String = zoneID
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			defaultTimeZone = tz
			Return tz
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As Void
					System.propertyrty("user.timezone", id)
					Return Nothing
			End Function
		End Class

		''' <summary>
		''' Sets the {@code TimeZone} that is returned by the {@code getDefault}
		''' method. {@code zone} is cached. If {@code zone} is null, the cached
		''' default {@code TimeZone} is cleared. This method doesn't change the value
		''' of the {@code user.timezone} property.
		''' </summary>
		''' <param name="zone"> the new default {@code TimeZone}, or null </param>
		''' <exception cref="SecurityException"> if the security manager's {@code checkPermission}
		'''                           denies {@code PropertyPermission("user.timezone",
		'''                           "write")} </exception>
		''' <seealso cref= #getDefault </seealso>
		''' <seealso cref= PropertyPermission </seealso>
		Public Shared Property [default] As TimeZone
			Set(ByVal zone As TimeZone)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New PropertyPermission("user.timezone", "write"))
				defaultTimeZone = zone
			End Set
		End Property

		''' <summary>
		''' Returns true if this zone has the same rule and offset as another zone.
		''' That is, if this zone differs only in ID, if at all.  Returns false
		''' if the other zone is null. </summary>
		''' <param name="other"> the <code>TimeZone</code> object to be compared with </param>
		''' <returns> true if the other zone is not null and is the same as this one,
		''' with the possible exception of the ID
		''' @since 1.2 </returns>
		Public Overridable Function hasSameRules(ByVal other As TimeZone) As Boolean
			Return other IsNot Nothing AndAlso rawOffset = other.rawOffset AndAlso useDaylightTime() = other.useDaylightTime()
		End Function

		''' <summary>
		''' Creates a copy of this <code>TimeZone</code>.
		''' </summary>
		''' <returns> a clone of this <code>TimeZone</code> </returns>
		Public Overridable Function clone() As Object
			Try
				Dim other As TimeZone = CType(MyBase.clone(), TimeZone)
				other.ID = ID
				Return other
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' The null constant as a TimeZone.
		''' </summary>
		Friend Const NO_TIMEZONE As TimeZone = Nothing

		' =======================privates===============================

		''' <summary>
		''' The string identifier of this <code>TimeZone</code>.  This is a
		''' programmatic identifier used internally to look up <code>TimeZone</code>
		''' objects from the system table and also to map them to their localized
		''' display names.  <code>ID</code> values are unique in the system
		''' table but may not be for dynamically created zones.
		''' @serial
		''' </summary>
		Private ID As String
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared defaultTimeZone As TimeZone

		Friend Const GMT_ID As String = "GMT"
		Private Const GMT_ID_LENGTH As Integer = 3

		' a static TimeZone we can reference if no AppContext is in place
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared mainAppContextDefault As TimeZone

		''' <summary>
		''' Parses a custom time zone identifier and returns a corresponding zone.
		''' This method doesn't support the RFC 822 time zone format. (e.g., +hhmm)
		''' </summary>
		''' <param name="id"> a string of the <a href="#CustomID">custom ID form</a>. </param>
		''' <returns> a newly created TimeZone with the given offset and
		''' no daylight saving time, or null if the id cannot be parsed. </returns>
		Private Shared Function parseCustomTimeZone(ByVal id As String) As TimeZone
			Dim length As Integer

			' Error if the length of id isn't long enough or id doesn't
			' start with "GMT".
			length = id.length()
			If length < (GMT_ID_LENGTH + 2) OrElse id.IndexOf(GMT_ID) <> 0 Then Return Nothing

			Dim zi As sun.util.calendar.ZoneInfo

			' First, we try to find it in the cache with the given
			' id. Even the id is not normalized, the returned ZoneInfo
			' should have its normalized id.
			zi = sun.util.calendar.ZoneInfoFile.getZoneInfo(id)
			If zi IsNot Nothing Then Return zi

			Dim index As Integer = GMT_ID_LENGTH
			Dim negative As Boolean = False
			Dim c As Char = id.Chars(index)
			index += 1
			If c = "-"c Then
				negative = True
			ElseIf c <> "+"c Then
				Return Nothing
			End If

			Dim hours As Integer = 0
			Dim num As Integer = 0
			Dim countDelim As Integer = 0
			Dim len As Integer = 0
			Do While index < length
				c = id.Chars(index)
				index += 1
				If c = ":"c Then
					If countDelim > 0 Then Return Nothing
					If len > 2 Then Return Nothing
					hours = num
					countDelim += 1
					num = 0
					len = 0
					Continue Do
				End If
				If c < "0"c OrElse c > "9"c Then Return Nothing
				num = num * 10 + (AscW(c) - AscW("0"c))
				len += 1
			Loop
			If index <> length Then Return Nothing
			If countDelim = 0 Then
				If len <= 2 Then
					hours = num
					num = 0
				Else
					hours = num \ 100
					num = num Mod 100
				End If
			Else
				If len <> 2 Then Return Nothing
			End If
			If hours > 23 OrElse num > 59 Then Return Nothing
			Dim gmtOffset As Integer = (hours * 60 + num) * 60 * 1000

			If gmtOffset = 0 Then
				zi = sun.util.calendar.ZoneInfoFile.getZoneInfo(GMT_ID)
				If negative Then
					zi.iD = "GMT-00:00"
				Else
					zi.iD = "GMT+00:00"
				End If
			Else
				zi = sun.util.calendar.ZoneInfoFile.getCustomTimeZone(id,If(negative, -gmtOffset, gmtOffset))
			End If
			Return zi
		End Function
	End Class

End Namespace