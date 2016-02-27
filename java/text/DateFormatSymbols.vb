Imports System
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
	''' <code>DateFormatSymbols</code> is a public class for encapsulating
	''' localizable date-time formatting data, such as the names of the
	''' months, the names of the days of the week, and the time zone data.
	''' <code>SimpleDateFormat</code> uses
	''' <code>DateFormatSymbols</code> to encapsulate this information.
	''' 
	''' <p>
	''' Typically you shouldn't use <code>DateFormatSymbols</code> directly.
	''' Rather, you are encouraged to create a date-time formatter with the
	''' <code>DateFormat</code> class's factory methods: <code>getTimeInstance</code>,
	''' <code>getDateInstance</code>, or <code>getDateTimeInstance</code>.
	''' These methods automatically create a <code>DateFormatSymbols</code> for
	''' the formatter so that you don't have to. After the
	''' formatter is created, you may modify its format pattern using the
	''' <code>setPattern</code> method. For more information about
	''' creating formatters using <code>DateFormat</code>'s factory methods,
	''' see <seealso cref="DateFormat"/>.
	''' 
	''' <p>
	''' If you decide to create a date-time formatter with a specific
	''' format pattern for a specific locale, you can do so with:
	''' <blockquote>
	''' <pre>
	''' new SimpleDateFormat(aPattern, DateFormatSymbols.getInstance(aLocale)).
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' <code>DateFormatSymbols</code> objects are cloneable. When you obtain
	''' a <code>DateFormatSymbols</code> object, feel free to modify the
	''' date-time formatting data. For instance, you can replace the localized
	''' date-time format pattern characters with the ones that you feel easy
	''' to remember. Or you can change the representative cities
	''' to your favorite ones.
	''' 
	''' <p>
	''' New <code>DateFormatSymbols</code> subclasses may be added to support
	''' <code>SimpleDateFormat</code> for date-time formatting for additional locales.
	''' </summary>
	''' <seealso cref=          DateFormat </seealso>
	''' <seealso cref=          SimpleDateFormat </seealso>
	''' <seealso cref=          java.util.SimpleTimeZone
	''' @author       Chen-Lieh Huang </seealso>
	<Serializable> _
	Public Class DateFormatSymbols
		Implements Cloneable

		''' <summary>
		''' Construct a DateFormatSymbols object by loading format data from
		''' resources for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/>
		''' locale. This constructor can only
		''' construct instances for the locales supported by the Java
		''' runtime environment, not for those supported by installed
		''' <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		''' implementations. For full locale coverage, use the
		''' <seealso cref="#getInstance(Locale) getInstance"/> method.
		''' <p>This is equivalent to calling
		''' {@link #DateFormatSymbols(Locale)
		'''     DateFormatSymbols(Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= #getInstance() </seealso>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <exception cref="java.util.MissingResourceException">
		'''             if the resources for the default locale cannot be
		'''             found or cannot be loaded. </exception>
		Public Sub New()
			initializeData(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Construct a DateFormatSymbols object by loading format data from
		''' resources for the given locale. This constructor can only
		''' construct instances for the locales supported by the Java
		''' runtime environment, not for those supported by installed
		''' <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		''' implementations. For full locale coverage, use the
		''' <seealso cref="#getInstance(Locale) getInstance"/> method.
		''' </summary>
		''' <param name="locale"> the desired locale </param>
		''' <seealso cref= #getInstance(Locale) </seealso>
		''' <exception cref="java.util.MissingResourceException">
		'''             if the resources for the specified locale cannot be
		'''             found or cannot be loaded. </exception>
		Public Sub New(ByVal locale As java.util.Locale)
			initializeData(locale)
		End Sub

		''' <summary>
		''' Era strings. For example: "AD" and "BC".  An array of 2 strings,
		''' indexed by <code>Calendar.BC</code> and <code>Calendar.AD</code>.
		''' @serial
		''' </summary>
		Friend eras As String() = Nothing

		''' <summary>
		''' Month strings. For example: "January", "February", etc.  An array
		''' of 13 strings (some calendars have 13 months), indexed by
		''' <code>Calendar.JANUARY</code>, <code>Calendar.FEBRUARY</code>, etc.
		''' @serial
		''' </summary>
		Friend months As String() = Nothing

		''' <summary>
		''' Short month strings. For example: "Jan", "Feb", etc.  An array of
		''' 13 strings (some calendars have 13 months), indexed by
		''' <code>Calendar.JANUARY</code>, <code>Calendar.FEBRUARY</code>, etc.
		''' 
		''' @serial
		''' </summary>
		Friend shortMonths As String() = Nothing

		''' <summary>
		''' Weekday strings. For example: "Sunday", "Monday", etc.  An array
		''' of 8 strings, indexed by <code>Calendar.SUNDAY</code>,
		''' <code>Calendar.MONDAY</code>, etc.
		''' The element <code>weekdays[0]</code> is ignored.
		''' @serial
		''' </summary>
		Friend weekdays As String() = Nothing

		''' <summary>
		''' Short weekday strings. For example: "Sun", "Mon", etc.  An array
		''' of 8 strings, indexed by <code>Calendar.SUNDAY</code>,
		''' <code>Calendar.MONDAY</code>, etc.
		''' The element <code>shortWeekdays[0]</code> is ignored.
		''' @serial
		''' </summary>
		Friend shortWeekdays As String() = Nothing

		''' <summary>
		''' AM and PM strings. For example: "AM" and "PM".  An array of
		''' 2 strings, indexed by <code>Calendar.AM</code> and
		''' <code>Calendar.PM</code>.
		''' @serial
		''' </summary>
		Friend ampms As String() = Nothing

		''' <summary>
		''' Localized names of time zones in this locale.  This is a
		''' two-dimensional array of strings of size <em>n</em> by <em>m</em>,
		''' where <em>m</em> is at least 5.  Each of the <em>n</em> rows is an
		''' entry containing the localized names for a single <code>TimeZone</code>.
		''' Each such row contains (with <code>i</code> ranging from
		''' 0..<em>n</em>-1):
		''' <ul>
		''' <li><code>zoneStrings[i][0]</code> - time zone ID</li>
		''' <li><code>zoneStrings[i][1]</code> - long name of zone in standard
		''' time</li>
		''' <li><code>zoneStrings[i][2]</code> - short name of zone in
		''' standard time</li>
		''' <li><code>zoneStrings[i][3]</code> - long name of zone in daylight
		''' saving time</li>
		''' <li><code>zoneStrings[i][4]</code> - short name of zone in daylight
		''' saving time</li>
		''' </ul>
		''' The zone ID is <em>not</em> localized; it's one of the valid IDs of
		''' the <seealso cref="java.util.TimeZone TimeZone"/> class that are not
		''' <a href="../java/util/TimeZone.html#CustomID">custom IDs</a>.
		''' All other entries are localized names. </summary>
		''' <seealso cref= java.util.TimeZone
		''' @serial </seealso>
		Friend zoneStrings As String()() = Nothing

		''' <summary>
		''' Indicates that zoneStrings is set externally with setZoneStrings() method.
		''' </summary>
		<NonSerialized> _
		Friend isZoneStringsSet As Boolean = False

		''' <summary>
		''' Unlocalized date-time pattern characters. For example: 'y', 'd', etc.
		''' All locales use the same these unlocalized pattern characters.
		''' </summary>
		Friend Const patternChars As String = "GyMdkHmsSEDFwWahKzZYuXL"

		Friend Const PATTERN_ERA As Integer = 0 ' G
		Friend Const PATTERN_YEAR As Integer = 1 ' y
		Friend Const PATTERN_MONTH As Integer = 2 ' M
		Friend Const PATTERN_DAY_OF_MONTH As Integer = 3 ' d
		Friend Const PATTERN_HOUR_OF_DAY1 As Integer = 4 ' k
		Friend Const PATTERN_HOUR_OF_DAY0 As Integer = 5 ' H
		Friend Const PATTERN_MINUTE As Integer = 6 ' m
		Friend Const PATTERN_SECOND As Integer = 7 ' s
		Friend Const PATTERN_MILLISECOND As Integer = 8 ' S
		Friend Const PATTERN_DAY_OF_WEEK As Integer = 9 ' E
		Friend Const PATTERN_DAY_OF_YEAR As Integer = 10 ' D
		Friend Const PATTERN_DAY_OF_WEEK_IN_MONTH As Integer = 11 ' F
		Friend Const PATTERN_WEEK_OF_YEAR As Integer = 12 ' w
		Friend Const PATTERN_WEEK_OF_MONTH As Integer = 13 ' W
		Friend Const PATTERN_AM_PM As Integer = 14 ' a
		Friend Const PATTERN_HOUR1 As Integer = 15 ' h
		Friend Const PATTERN_HOUR0 As Integer = 16 ' K
		Friend Const PATTERN_ZONE_NAME As Integer = 17 ' z
		Friend Const PATTERN_ZONE_VALUE As Integer = 18 ' Z
		Friend Const PATTERN_WEEK_YEAR As Integer = 19 ' Y
		Friend Const PATTERN_ISO_DAY_OF_WEEK As Integer = 20 ' u
		Friend Const PATTERN_ISO_ZONE As Integer = 21 ' X
		Friend Const PATTERN_MONTH_STANDALONE As Integer = 22 ' L

		''' <summary>
		''' Localized date-time pattern characters. For example, a locale may
		''' wish to use 'u' rather than 'y' to represent years in its date format
		''' pattern strings.
		''' This string must be exactly 18 characters long, with the index of
		''' the characters described by <code>DateFormat.ERA_FIELD</code>,
		''' <code>DateFormat.YEAR_FIELD</code>, etc.  Thus, if the string were
		''' "Xz...", then localized patterns would use 'X' for era and 'z' for year.
		''' @serial
		''' </summary>
		Friend localPatternChars As String = Nothing

		''' <summary>
		''' The locale which is used for initializing this DateFormatSymbols object.
		''' 
		''' @since 1.6
		''' @serial
		''' </summary>
		Friend locale As java.util.Locale = Nothing

		' use serialVersionUID from JDK 1.1.4 for interoperability 
		Friend Const serialVersionUID As Long = -5987973545549424702L

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>getInstance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported by the
		''' Java runtime and by installed
		''' <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		''' implementations.  It must contain at least a <code>Locale</code>
		''' instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>DateFormatSymbols</code> instances are available.
		''' @since 1.6 </returns>
		Public Property Shared availableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool= sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.DateFormatSymbolsProvider))
				Return pool.availableLocales
			End Get
		End Property

		''' <summary>
		''' Gets the <code>DateFormatSymbols</code> instance for the default
		''' locale.  This method provides access to <code>DateFormatSymbols</code>
		''' instances for locales supported by the Java runtime itself as well
		''' as for those supported by installed
		''' <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		''' implementations.
		''' <p>This is equivalent to calling {@link #getInstance(Locale)
		'''     getInstance(Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a <code>DateFormatSymbols</code> instance.
		''' @since 1.6 </returns>
		Public Property Shared instance As DateFormatSymbols
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets the <code>DateFormatSymbols</code> instance for the specified
		''' locale.  This method provides access to <code>DateFormatSymbols</code>
		''' instances for locales supported by the Java runtime itself as well
		''' as for those supported by installed
		''' <seealso cref="java.text.spi.DateFormatSymbolsProvider DateFormatSymbolsProvider"/>
		''' implementations. </summary>
		''' <param name="locale"> the given locale. </param>
		''' <returns> a <code>DateFormatSymbols</code> instance. </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null
		''' @since 1.6 </exception>
		Public Shared Function getInstance(ByVal locale As java.util.Locale) As DateFormatSymbols
			Dim dfs As DateFormatSymbols = getProviderInstance(locale)
			If dfs IsNot Nothing Then Return dfs
			Throw New RuntimeException("DateFormatSymbols instance creation failed.")
		End Function

		''' <summary>
		''' Returns a DateFormatSymbols provided by a provider or found in
		''' the cache. Note that this method returns a cached instance,
		''' not its clone. Therefore, the instance should never be given to
		''' an application.
		''' </summary>
		Shared Function getInstanceRef(ByVal locale As java.util.Locale) As DateFormatSymbols
			Dim dfs As DateFormatSymbols = getProviderInstance(locale)
			If dfs IsNot Nothing Then Return dfs
			Throw New RuntimeException("DateFormatSymbols instance creation failed.")
		End Function

		Private Shared Function getProviderInstance(ByVal locale As java.util.Locale) As DateFormatSymbols
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DateFormatSymbolsProvider), locale)
			Dim provider As java.text.spi.DateFormatSymbolsProvider = adapter.dateFormatSymbolsProvider
			Dim dfsyms As DateFormatSymbols = provider.getInstance(locale)
			If dfsyms Is Nothing Then
				provider = sun.util.locale.provider.LocaleProviderAdapter.forJRE().dateFormatSymbolsProvider
				dfsyms = provider.getInstance(locale)
			End If
			Return dfsyms
		End Function

		''' <summary>
		''' Gets era strings. For example: "AD" and "BC". </summary>
		''' <returns> the era strings. </returns>
		Public Overridable Property eras As String()
			Get
				Return java.util.Arrays.copyOf(eras, eras.Length)
			End Get
			Set(ByVal newEras As String())
				eras = java.util.Arrays.copyOf(newEras, newEras.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets month strings. For example: "January", "February", etc.
		''' 
		''' <p>If the language requires different forms for formatting and
		''' stand-alone usages, this method returns month names in the
		''' formatting form. For example, the preferred month name for
		''' January in the Czech language is <em>ledna</em> in the
		''' formatting form, while it is <em>leden</em> in the stand-alone
		''' form. This method returns {@code "ledna"} in this case. Refer
		''' to the <a href="http://unicode.org/reports/tr35/#Calendar_Elements">
		''' Calendar Elements in the Unicode Locale Data Markup Language
		''' (LDML) specification</a> for more details.
		''' </summary>
		''' <returns> the month strings. </returns>
		Public Overridable Property months As String()
			Get
				Return java.util.Arrays.copyOf(months, months.Length)
			End Get
			Set(ByVal newMonths As String())
				months = java.util.Arrays.copyOf(newMonths, newMonths.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets short month strings. For example: "Jan", "Feb", etc.
		''' 
		''' <p>If the language requires different forms for formatting and
		''' stand-alone usages, This method returns short month names in
		''' the formatting form. For example, the preferred abbreviation
		''' for January in the Catalan language is <em>de gen.</em> in the
		''' formatting form, while it is <em>gen.</em> in the stand-alone
		''' form. This method returns {@code "de gen."} in this case. Refer
		''' to the <a href="http://unicode.org/reports/tr35/#Calendar_Elements">
		''' Calendar Elements in the Unicode Locale Data Markup Language
		''' (LDML) specification</a> for more details.
		''' </summary>
		''' <returns> the short month strings. </returns>
		Public Overridable Property shortMonths As String()
			Get
				Return java.util.Arrays.copyOf(shortMonths, shortMonths.Length)
			End Get
			Set(ByVal newShortMonths As String())
				shortMonths = java.util.Arrays.copyOf(newShortMonths, newShortMonths.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets weekday strings. For example: "Sunday", "Monday", etc. </summary>
		''' <returns> the weekday strings. Use <code>Calendar.SUNDAY</code>,
		''' <code>Calendar.MONDAY</code>, etc. to index the result array. </returns>
		Public Overridable Property weekdays As String()
			Get
				Return java.util.Arrays.copyOf(weekdays, weekdays.Length)
			End Get
			Set(ByVal newWeekdays As String())
				weekdays = java.util.Arrays.copyOf(newWeekdays, newWeekdays.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets short weekday strings. For example: "Sun", "Mon", etc. </summary>
		''' <returns> the short weekday strings. Use <code>Calendar.SUNDAY</code>,
		''' <code>Calendar.MONDAY</code>, etc. to index the result array. </returns>
		Public Overridable Property shortWeekdays As String()
			Get
				Return java.util.Arrays.copyOf(shortWeekdays, shortWeekdays.Length)
			End Get
			Set(ByVal newShortWeekdays As String())
				shortWeekdays = java.util.Arrays.copyOf(newShortWeekdays, newShortWeekdays.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets ampm strings. For example: "AM" and "PM". </summary>
		''' <returns> the ampm strings. </returns>
		Public Overridable Property amPmStrings As String()
			Get
				Return java.util.Arrays.copyOf(ampms, ampms.Length)
			End Get
			Set(ByVal newAmpms As String())
				ampms = java.util.Arrays.copyOf(newAmpms, newAmpms.Length)
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets time zone strings.  Use of this method is discouraged; use
		''' <seealso cref="java.util.TimeZone#getDisplayName() TimeZone.getDisplayName()"/>
		''' instead.
		''' <p>
		''' The value returned is a
		''' two-dimensional array of strings of size <em>n</em> by <em>m</em>,
		''' where <em>m</em> is at least 5.  Each of the <em>n</em> rows is an
		''' entry containing the localized names for a single <code>TimeZone</code>.
		''' Each such row contains (with <code>i</code> ranging from
		''' 0..<em>n</em>-1):
		''' <ul>
		''' <li><code>zoneStrings[i][0]</code> - time zone ID</li>
		''' <li><code>zoneStrings[i][1]</code> - long name of zone in standard
		''' time</li>
		''' <li><code>zoneStrings[i][2]</code> - short name of zone in
		''' standard time</li>
		''' <li><code>zoneStrings[i][3]</code> - long name of zone in daylight
		''' saving time</li>
		''' <li><code>zoneStrings[i][4]</code> - short name of zone in daylight
		''' saving time</li>
		''' </ul>
		''' The zone ID is <em>not</em> localized; it's one of the valid IDs of
		''' the <seealso cref="java.util.TimeZone TimeZone"/> class that are not
		''' <a href="../util/TimeZone.html#CustomID">custom IDs</a>.
		''' All other entries are localized names.  If a zone does not implement
		''' daylight saving time, the daylight saving time names should not be used.
		''' <p>
		''' If <seealso cref="#setZoneStrings(String[][]) setZoneStrings"/> has been called
		''' on this <code>DateFormatSymbols</code> instance, then the strings
		''' provided by that call are returned. Otherwise, the returned array
		''' contains names provided by the Java runtime and by installed
		''' <seealso cref="java.util.spi.TimeZoneNameProvider TimeZoneNameProvider"/>
		''' implementations.
		''' </summary>
		''' <returns> the time zone strings. </returns>
		''' <seealso cref= #setZoneStrings(String[][]) </seealso>
		Public Overridable Property zoneStrings As String()()
			Get
				Return getZoneStringsImpl(True)
			End Get
			Set(ByVal newZoneStrings As String()())
				Dim aCopy As String()() = New String(newZoneStrings.Length - 1)(){}
				For i As Integer = 0 To newZoneStrings.Length - 1
					Dim len As Integer = newZoneStrings(i).Length
					If len < 5 Then Throw New IllegalArgumentException
					aCopy(i) = java.util.Arrays.copyOf(newZoneStrings(i), len)
				Next i
				zoneStrings = aCopy
				isZoneStringsSet = True
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Gets localized date-time pattern characters. For example: 'u', 't', etc. </summary>
		''' <returns> the localized date-time pattern characters. </returns>
		Public Overridable Property localPatternChars As String
			Get
				Return localPatternChars
			End Get
			Set(ByVal newLocalPatternChars As String)
				' Call toString() to throw an NPE in case the argument is null
				localPatternChars = newLocalPatternChars.ToString()
				cachedHashCode = 0
			End Set
		End Property


		''' <summary>
		''' Overrides Cloneable
		''' </summary>
		Public Overridable Function clone() As Object
			Try
				Dim other As DateFormatSymbols = CType(MyBase.clone(), DateFormatSymbols)
				copyMembers(Me, other)
				Return other
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Override hashCode.
		''' Generates a hash code for the DateFormatSymbols object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = cachedHashCode
			If hashCode = 0 Then
				hashCode = 5
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(eras)
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(months)
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(shortMonths)
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(weekdays)
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(shortWeekdays)
				hashCode = 11 * hashCode + java.util.Arrays.hashCode(ampms)
				hashCode = 11 * hashCode + java.util.Arrays.deepHashCode(zoneStringsWrapper)
				hashCode = 11 * hashCode + java.util.Objects.hashCode(localPatternChars)
				cachedHashCode = hashCode
			End If

			Return hashCode
		End Function

		''' <summary>
		''' Override equals
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If obj Is Nothing OrElse Me.GetType() IsNot obj.GetType() Then Return False
			Dim that As DateFormatSymbols = CType(obj, DateFormatSymbols)
			Return (java.util.Arrays.Equals(eras, that.eras) AndAlso java.util.Arrays.Equals(months, that.months) AndAlso java.util.Arrays.Equals(shortMonths, that.shortMonths) AndAlso java.util.Arrays.Equals(weekdays, that.weekdays) AndAlso java.util.Arrays.Equals(shortWeekdays, that.shortWeekdays) AndAlso java.util.Arrays.Equals(ampms, that.ampms) AndAlso java.util.Arrays.deepEquals(zoneStringsWrapper, that.zoneStringsWrapper) AndAlso ((localPatternChars IsNot Nothing AndAlso localPatternChars.Equals(that.localPatternChars)) OrElse (localPatternChars Is Nothing AndAlso that.localPatternChars Is Nothing)))
		End Function

		' =======================privates===============================

		''' <summary>
		''' Useful constant for defining time zone offsets.
		''' </summary>
		Friend Const millisPerHour As Integer = 60*60*1000

		''' <summary>
		''' Cache to hold DateFormatSymbols instances per Locale.
		''' </summary>
		Private Shared ReadOnly cachedInstances As java.util.concurrent.ConcurrentMap(Of java.util.Locale, SoftReference(Of DateFormatSymbols)) = New ConcurrentDictionary(Of java.util.Locale, SoftReference(Of DateFormatSymbols))(3)

		<NonSerialized> _
		Private lastZoneIndex As Integer = 0

		''' <summary>
		''' Cached hash code
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend cachedHashCode As Integer = 0

		Private Sub initializeData(ByVal desiredLocale As java.util.Locale)
			locale = desiredLocale

			' Copy values of a cached instance if any.
			Dim ref As SoftReference(Of DateFormatSymbols) = cachedInstances.get(locale)
			Dim dfs As DateFormatSymbols
			dfs = ref.get()
			If ref IsNot Nothing AndAlso dfs IsNot Nothing Then
				copyMembers(dfs, Me)
				Return
			End If

			' Initialize the fields from the ResourceBundle for locale.
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DateFormatSymbolsProvider), locale)
			' Avoid any potential recursions
			If Not(TypeOf adapter Is sun.util.locale.provider.ResourceBundleBasedAdapter) Then adapter = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased
			Dim resource As java.util.ResourceBundle = CType(adapter, sun.util.locale.provider.ResourceBundleBasedAdapter).localeData.getDateFormatData(locale)

			' JRE and CLDR use different keys
			' JRE: Eras,  java.lang.[Short].Eras and narrow.Eras
			' CLDR: java.lang.[Long].Eras, Eras and narrow.Eras
			If resource.containsKey("Eras") Then
				eras = resource.getStringArray("Eras")
			ElseIf resource.containsKey("long.Eras") Then
				eras = resource.getStringArray("long.Eras")
			ElseIf resource.containsKey(" java.lang.[Short].Eras") Then
				eras = resource.getStringArray(" java.lang.[Short].Eras")
			End If
			months = resource.getStringArray("MonthNames")
			shortMonths = resource.getStringArray("MonthAbbreviations")
			ampms = resource.getStringArray("AmPmMarkers")
			localPatternChars = resource.getString("DateTimePatternChars")

			' Day of week names are stored in a 1-based array.
			weekdays = toOneBasedArray(resource.getStringArray("DayNames"))
			shortWeekdays = toOneBasedArray(resource.getStringArray("DayAbbreviations"))

			' Put a clone in the cache
			ref = New SoftReference(Of )(CType(Me.clone(), DateFormatSymbols))
			Dim x As SoftReference(Of DateFormatSymbols) = cachedInstances.putIfAbsent(locale, ref)
			If x IsNot Nothing Then
				Dim y As DateFormatSymbols = x.get()
				If y Is Nothing Then cachedInstances.put(locale, ref)
			End If
		End Sub

		Private Shared Function toOneBasedArray(ByVal src As String()) As String()
			Dim len As Integer = src.Length
			Dim dst As String() = New String(len){}
			dst(0) = ""
			For i As Integer = 0 To len - 1
				dst(i + 1) = src(i)
			Next i
			Return dst
		End Function

		''' <summary>
		''' Package private: used by SimpleDateFormat
		''' Gets the index for the given time zone ID to obtain the time zone
		''' strings for formatting. The time zone ID is just for programmatic
		''' lookup. NOT LOCALIZED!!! </summary>
		''' <param name="ID"> the given time zone ID. </param>
		''' <returns> the index of the given time zone ID.  Returns -1 if
		''' the given time zone ID can't be located in the DateFormatSymbols object. </returns>
		''' <seealso cref= java.util.SimpleTimeZone </seealso>
		Friend Function getZoneIndex(ByVal ID As String) As Integer
			Dim zoneStrings_Renamed As String()() = zoneStringsWrapper

	'        
	'         * getZoneIndex has been re-written for performance reasons. instead of
	'         * traversing the zoneStrings array every time, we cache the last used zone
	'         * index
	'         
			If lastZoneIndex < zoneStrings_Renamed.Length AndAlso ID.Equals(zoneStrings_Renamed(lastZoneIndex)(0)) Then Return lastZoneIndex

			' slow path, search entire list 
			For index As Integer = 0 To zoneStrings_Renamed.Length - 1
				If ID.Equals(zoneStrings_Renamed(index)(0)) Then
					lastZoneIndex = index
					Return index
				End If
			Next index

			Return -1
		End Function

		''' <summary>
		''' Wrapper method to the getZoneStrings(), which is called from inside
		''' the java.text package and not to mutate the returned arrays, so that
		''' it does not need to create a defensive copy.
		''' </summary>
		Friend Property zoneStringsWrapper As String()()
			Get
				If subclassObject Then
					Return zoneStrings
				Else
					Return getZoneStringsImpl(False)
				End If
			End Get
		End Property

		Private Function getZoneStringsImpl(ByVal needsCopy As Boolean) As String()()
			If zoneStrings Is Nothing Then zoneStrings = sun.util.locale.provider.TimeZoneNameUtility.getZoneStrings(locale)

			If Not needsCopy Then Return zoneStrings

			Dim len As Integer = zoneStrings.Length
			Dim aCopy As String()() = New String(len - 1)(){}
			For i As Integer = 0 To len - 1
				aCopy(i) = java.util.Arrays.copyOf(zoneStrings(i), zoneStrings(i).Length)
			Next i
			Return aCopy
		End Function

		Private Property subclassObject As Boolean
			Get
				Return Not Me.GetType().name.Equals("java.text.DateFormatSymbols")
			End Get
		End Property

		''' <summary>
		''' Clones all the data members from the source DateFormatSymbols to
		''' the target DateFormatSymbols. This is only for subclasses. </summary>
		''' <param name="src"> the source DateFormatSymbols. </param>
		''' <param name="dst"> the target DateFormatSymbols. </param>
		Private Sub copyMembers(ByVal src As DateFormatSymbols, ByVal dst As DateFormatSymbols)
			dst.eras = java.util.Arrays.copyOf(src.eras, src.eras.Length)
			dst.months = java.util.Arrays.copyOf(src.months, src.months.Length)
			dst.shortMonths = java.util.Arrays.copyOf(src.shortMonths, src.shortMonths.Length)
			dst.weekdays = java.util.Arrays.copyOf(src.weekdays, src.weekdays.Length)
			dst.shortWeekdays = java.util.Arrays.copyOf(src.shortWeekdays, src.shortWeekdays.Length)
			dst.ampms = java.util.Arrays.copyOf(src.ampms, src.ampms.Length)
			If src.zoneStrings IsNot Nothing Then
				dst.zoneStrings = src.getZoneStringsImpl(True)
			Else
				dst.zoneStrings = Nothing
			End If
			dst.localPatternChars = src.localPatternChars
			dst.cachedHashCode = 0
		End Sub

		''' <summary>
		''' Write out the default serializable data, after ensuring the
		''' <code>zoneStrings</code> field is initialized in order to make
		''' sure the backward compatibility.
		''' 
		''' @since 1.6
		''' </summary>
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			If zoneStrings Is Nothing Then zoneStrings = sun.util.locale.provider.TimeZoneNameUtility.getZoneStrings(locale)
			stream.defaultWriteObject()
		End Sub
	End Class

End Namespace