Imports Microsoft.VisualBasic
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
' * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.chrono




	''' <summary>
	''' An abstract implementation of a calendar system, used to organize and identify dates.
	''' <p>
	''' The main date and time API is built on the ISO calendar system.
	''' The chronology operates behind the scenes to represent the general concept of a calendar system.
	''' <p>
	''' See <seealso cref="Chronology"/> for more details.
	''' 
	''' @implSpec
	''' This class is separated from the {@code Chronology} interface so that the static methods
	''' are not inherited. While {@code Chronology} can be implemented directly, it is strongly
	''' recommended to extend this abstract class instead.
	''' <p>
	''' This class must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' Subclasses should be Serializable wherever possible.
	''' 
	''' @since 1.8
	''' </summary>
	Public MustInherit Class AbstractChronology
		Implements Chronology

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return ChronoPeriodImpl(Me, years, months, days);
			Public MustOverride Function period(ByVal years As Integer, ByVal months As Integer, ByVal days As Integer) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return DateTimeFormatterBuilder(temporal);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return query(query);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract  Return(R);
			Public Function [if](query = java.time.temporal.TemporalQueries.chronology() ByVal As ) As [MustOverride]
			Public MustOverride Function query(ByVal query As java.time.temporal.TemporalQuery(Of R)) As R Implements Chronology.query
			Public MustOverride Function UnsupportedTemporalTypeException("Unsupported field: " & ByVal field As ) As throw
			Public MustOverride Function getLong(ByVal field As java.time.temporal.TemporalField) As Long Implements Chronology.getLong
			Public MustOverride Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean Implements Chronology.isSupported
			Public MustOverride Function getDisplayName(ByVal style As java.time.format.TextStyle, ByVal locale As java.util.Locale) As default
			Public MustOverride Function range(ByVal field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange Implements Chronology.range
			Public MustOverride Function eras() As IList(Of Era) Implements Chronology.eras
			Public MustOverride Function eraOf(ByVal eraValue As Integer) As Era Implements Chronology.eraOf
			Public MustOverride Function prolepticYear(ByVal era As Era, ByVal yearOfEra As Integer) As Integer Implements Chronology.prolepticYear
			Public MustOverride Function isLeapYear(ByVal prolepticYear As Long) As Boolean Implements Chronology.isLeapYear
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return ofInstant(Me, instant, zone);
			Public MustOverride Function zonedDateTime(ByVal instant As java.time.Instant, ByVal zone As java.time.ZoneId) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract throw java.time.DateTimeException("Unable to obtain ChronoZonedDateTime from TemporalAccessor: " & temporal.getClass(), ex);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return ofBest(cldt, zone, Nothing);
			Public Function [catch](ByVal ex1 As java.time.DateTimeException) As [MustOverride]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return zonedDateTime(instant, zone);
			Public MustOverride Function zonedDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract throw java.time.DateTimeException("Unable to obtain ChronoLocalDateTime from TemporalAccessor: " & temporal.getClass(), ex);
			Public Function [catch](ByVal ex As java.time.DateTimeException) As [MustOverride]
			Public MustOverride Function [date](java.time.LocalTime.from(temporal) ByVal As ) As [Return] Implements Chronology.date
			Public MustOverride Function localDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As default
			Public MustOverride Function [date](ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDate Implements Chronology.date
			Public MustOverride Function [date](java.time.LocalDate.now(clock) ByVal As ) As [Return] Implements Chronology.date
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract  requireNonNull(clock, "clock");
			Public MustOverride Function dateNow(ByVal clock As java.time.Clock) As default
			Public MustOverride Function dateNow(java.time.Clock.system(zone) ByVal As ) As [Return] Implements Chronology.dateNow
			Public MustOverride Function dateNow(ByVal zone As java.time.ZoneId) As default
			Public MustOverride Function dateNow(java.time.Clock.systemDefaultZone() ByVal As ) As [Return] Implements Chronology.dateNow
			Public MustOverride Function dateNow() As default
			Public MustOverride Function dateEpochDay(ByVal epochDay As Long) As ChronoLocalDate Implements Chronology.dateEpochDay
			Public MustOverride Function dateYearDay(ByVal prolepticYear As Integer, ByVal dayOfYear As Integer) As ChronoLocalDate Implements Chronology.dateYearDay
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return dateYearDay(prolepticYear(era, yearOfEra), dayOfYear);
			Public MustOverride Function dateYearDay(ByVal era As Era, ByVal yearOfEra As Integer, ByVal dayOfYear As Integer) As default
			Public MustOverride Function [date](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As ChronoLocalDate Implements Chronology.date
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return date(prolepticYear(era, yearOfEra), month, dayOfMonth);
			Public MustOverride Function [date](ByVal era As Era, ByVal yearOfEra As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As default
			Public MustOverride ReadOnly Property calendarType As String Implements Chronology.getCalendarType
			Public MustOverride ReadOnly Property id As String Implements Chronology.getId
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return of(id);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract Return ofLocale(locale);
			Public Function [Return](obj <> Nothing ? obj : ByVal IsoChronology.INSTANCE As ) As [MustOverride]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public abstract  requireNonNull(temporal, "temporal");
			Public MustOverride Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As Chronology Implements Chronology.from

		''' <summary>
		''' ChronoLocalDate order constant.
		''' </summary>
		static final IComparer(Of ChronoLocalDate) DATE_ORDER = (IComparer(Of ChronoLocalDate) And java.io.Serializable)(date1, date2) ->
				Return java.lang.[Long].Compare(date1.toEpochDay(), date2.toEpochDay())
		''' <summary>
		''' ChronoLocalDateTime order constant.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		static final IComparer(Of ChronoLocalDateTime(Of ? As ChronoLocalDate)) DATE_TIME_ORDER = (IComparer(Of ChronoLocalDateTime(Of ? As ChronoLocalDate)) And java.io.Serializable)(dateTime1, dateTime2) ->
				Dim cmp As Integer = java.lang.[Long].Compare(dateTime1.toLocalDate().toEpochDay(), dateTime2.toLocalDate().toEpochDay())
				If cmp = 0 Then cmp = java.lang.[Long].Compare(dateTime1.toLocalTime().toNanoOfDay(), dateTime2.toLocalTime().toNanoOfDay())
				Return cmp
		''' <summary>
		''' ChronoZonedDateTime order constant.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		static final IComparer(Of ChronoZonedDateTime(Of ?)) INSTANT_ORDER = (IComparer(Of ChronoZonedDateTime(Of ?)) And java.io.Serializable)(dateTime1, dateTime2) ->
					Dim cmp As Integer = java.lang.[Long].Compare(dateTime1.toEpochSecond(), dateTime2.toEpochSecond())
					If cmp = 0 Then cmp = java.lang.[Long].Compare(dateTime1.toLocalTime().nano, dateTime2.toLocalTime().nano)
					Return cmp

		''' <summary>
		''' Map of available calendars by ID.
		''' </summary>
		private static final ConcurrentDictionary(Of String, Chronology) CHRONOS_BY_ID = New ConcurrentDictionary(Of )
		''' <summary>
		''' Map of available calendars by calendar type.
		''' </summary>
		private static final ConcurrentDictionary(Of String, Chronology) CHRONOS_BY_TYPE = New ConcurrentDictionary(Of )

		''' <summary>
		''' Register a Chronology by its ID and type for lookup by <seealso cref="#of(String)"/>.
		''' Chronologies must not be registered until they are completely constructed.
		''' Specifically, not in the constructor of Chronology.
		''' </summary>
		''' <param name="chrono"> the chronology to register; not null </param>
		''' <returns> the already registered Chronology if any, may be null </returns>
		static Chronology registerChrono(Chronology chrono)
			Return registerChrono(chrono, chrono.id)

		''' <summary>
		''' Register a Chronology by ID and type for lookup by <seealso cref="#of(String)"/>.
		''' Chronos must not be registered until they are completely constructed.
		''' Specifically, not in the constructor of Chronology.
		''' </summary>
		''' <param name="chrono"> the chronology to register; not null </param>
		''' <param name="id"> the ID to register the chronology; not null </param>
		''' <returns> the already registered Chronology if any, may be null </returns>
		static Chronology registerChrono(Chronology chrono, String id)
			Dim prev As Chronology = CHRONOS_BY_ID.GetOrAdd(id, chrono)
			If prev Is Nothing Then
				Dim type As String = chrono.calendarType
				If type IsNot Nothing Then CHRONOS_BY_TYPE.GetOrAdd(type, chrono)
			End If
			Return prev

		''' <summary>
		''' Initialization of the maps from id and type to Chronology.
		''' The ServiceLoader is used to find and register any implementations
		''' of <seealso cref="java.time.chrono.AbstractChronology"/> found in the bootclass loader.
		''' The built-in chronologies are registered explicitly.
		''' Calendars configured via the Thread's context classloader are local
		''' to that thread and are ignored.
		''' <p>
		''' The initialization is done only once using the registration
		''' of the IsoChronology as the test and the final step.
		''' Multiple threads may perform the initialization concurrently.
		''' Only the first registration of each Chronology is retained by the
		''' ConcurrentHashMap. </summary>
		''' <returns> true if the cache was initialized </returns>
		private static Boolean initCache()
			If CHRONOS_BY_ID("ISO") Is Nothing Then
				' Initialization is incomplete

				' Register built-in Chronologies
				registerChrono(HijrahChronology.INSTANCE)
				registerChrono(JapaneseChronology.INSTANCE)
				registerChrono(MinguoChronology.INSTANCE)
				registerChrono(ThaiBuddhistChronology.INSTANCE)

				' Register Chronologies from the ServiceLoader
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim loader As java.util.ServiceLoader(Of AbstractChronology) = java.util.ServiceLoader.load(GetType(AbstractChronology), Nothing)
				For Each chrono As AbstractChronology In loader
					Dim id_Renamed As String = chrono.id
					If id_Renamed.Equals("ISO") OrElse registerChrono(chrono) IsNot Nothing Then
						' Log the attempt to replace an existing Chronology
						Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.time.chrono")
						logger.warning("Ignoring duplicate Chronology, from ServiceLoader configuration " & id_Renamed)
					End If
				Next chrono

				' finally, register IsoChronology to mark initialization is complete
				registerChrono(IsoChronology.INSTANCE)
				Return True
			End If
			Return False

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Chronology} from a locale.
		''' <p>
		''' See <seealso cref="Chronology#ofLocale(Locale)"/>.
		''' </summary>
		''' <param name="locale">  the locale to use to obtain the calendar system, not null </param>
		''' <returns> the calendar system associated with the locale, not null </returns>
		''' <exception cref="java.time.DateTimeException"> if the locale-specified calendar cannot be found </exception>
		static Chronology ofLocale(java.util.Locale locale)
			java.util.Objects.requireNonNull(locale, "locale")
			Dim type As String = locale.getUnicodeLocaleType("ca")
			If type Is Nothing OrElse "iso".Equals(type) OrElse "iso8601".Equals(type) Then Return IsoChronology.INSTANCE
			' Not pre-defined; lookup by the type
			Do
				Dim chrono As Chronology = CHRONOS_BY_TYPE(type)
				If chrono IsNot Nothing Then Return chrono
				' If not found, do the initialization (once) and repeat the lookup
			Loop While initCache()

			' Look for a Chronology using ServiceLoader of the Thread's ContextClassLoader
			' Application provided Chronologies must not be cached
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim loader As java.util.ServiceLoader(Of Chronology) = java.util.ServiceLoader.load(GetType(Chronology))
			For Each chrono As Chronology In loader
				If type.Equals(chrono.calendarType) Then Return chrono
			Next chrono
			Throw New java.time.DateTimeException("Unknown calendar system: " & type)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Chronology} from a chronology ID or
		''' calendar system type.
		''' <p>
		''' See <seealso cref="Chronology#of(String)"/>.
		''' </summary>
		''' <param name="id">  the chronology ID or calendar system type, not null </param>
		''' <returns> the chronology with the identifier requested, not null </returns>
		''' <exception cref="java.time.DateTimeException"> if the chronology cannot be found </exception>
		static Chronology [of](String id)
			java.util.Objects.requireNonNull(id, "id")
			Do
				Dim chrono As Chronology = of0(id)
				If chrono IsNot Nothing Then Return chrono
				' If not found, do the initialization (once) and repeat the lookup
			Loop While initCache()

			' Look for a Chronology using ServiceLoader of the Thread's ContextClassLoader
			' Application provided Chronologies must not be cached
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim loader As java.util.ServiceLoader(Of Chronology) = java.util.ServiceLoader.load(GetType(Chronology))
			For Each chrono As Chronology In loader
				If id.Equals(chrono.id) OrElse id.Equals(chrono.calendarType) Then Return chrono
			Next chrono
			Throw New java.time.DateTimeException("Unknown chronology: " & id)

		''' <summary>
		''' Obtains an instance of {@code Chronology} from a chronology ID or
		''' calendar system type.
		''' </summary>
		''' <param name="id">  the chronology ID or calendar system type, not null </param>
		''' <returns> the chronology with the identifier requested, or {@code null} if not found </returns>
		private static Chronology of0(String id)
			Dim chrono As Chronology = CHRONOS_BY_ID(id)
			If chrono Is Nothing Then chrono = CHRONOS_BY_TYPE(id)
			Return chrono

		''' <summary>
		''' Returns the available chronologies.
		''' <p>
		''' Each returned {@code Chronology} is available for use in the system.
		''' The set of chronologies includes the system chronologies and
		''' any chronologies provided by the application via ServiceLoader
		''' configuration.
		''' </summary>
		''' <returns> the independent, modifiable set of the available chronology IDs, not null </returns>
		static java.util.Set(Of Chronology) availableChronologies
			initCache() ' force initialization
			Dim chronos As New HashSet(Of Chronology)(CHRONOS_BY_ID.Values)

			'/ Add in Chronologies from the ServiceLoader configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim loader As java.util.ServiceLoader(Of Chronology) = java.util.ServiceLoader.load(GetType(Chronology))
			For Each chrono As Chronology In loader
				chronos.Add(chrono)
			Next chrono
			Return chronos

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance.
		''' </summary>
		protected AbstractChronology()

		'-----------------------------------------------------------------------
		''' <summary>
		''' Resolves parsed {@code ChronoField} values into a date during parsing.
		''' <p>
		''' Most {@code TemporalField} implementations are resolved using the
		''' resolve method on the field. By contrast, the {@code ChronoField} class
		''' defines fields that only have meaning relative to the chronology.
		''' As such, {@code ChronoField} date fields are resolved here in the
		''' context of a specific chronology.
		''' <p>
		''' {@code ChronoField} instances are resolved by this method, which may
		''' be overridden in subclasses.
		''' <ul>
		''' <li>{@code EPOCH_DAY} - If present, this is converted to a date and
		'''  all other date fields are then cross-checked against the date.
		''' <li>{@code PROLEPTIC_MONTH} - If present, then it is split into the
		'''  {@code YEAR} and {@code MONTH_OF_YEAR}. If the mode is strict or smart
		'''  then the field is validated.
		''' <li>{@code YEAR_OF_ERA} and {@code ERA} - If both are present, then they
		'''  are combined to form a {@code YEAR}. In lenient mode, the {@code YEAR_OF_ERA}
		'''  range is not validated, in smart and strict mode it is. The {@code ERA} is
		'''  validated for range in all three modes. If only the {@code YEAR_OF_ERA} is
		'''  present, and the mode is smart or lenient, then the last available era
		'''  is assumed. In strict mode, no era is assumed and the {@code YEAR_OF_ERA} is
		'''  left untouched. If only the {@code ERA} is present, then it is left untouched.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR} and {@code DAY_OF_MONTH} -
		'''  If all three are present, then they are combined to form a date.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is smart or strict, then the month and day are validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first day of the first month in the requested year,
		'''  then adding the difference in months, then the difference in days.
		'''  If the mode is smart, and the day-of-month is greater than the maximum for
		'''  the year-month, then the day-of-month is adjusted to the last day-of-month.
		'''  If the mode is strict, then the three fields must form a valid date.
		''' <li>{@code YEAR} and {@code DAY_OF_YEAR} -
		'''  If both are present, then they are combined to form a date.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first day of the requested year, then adding
		'''  the difference in days.
		'''  If the mode is smart or strict, then the two fields must form a valid date.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		'''  {@code ALIGNED_DAY_OF_WEEK_IN_MONTH} -
		'''  If all four are present, then they are combined to form a date.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first day of the first month in the requested year, then adding
		'''  the difference in months, then the difference in weeks, then in days.
		'''  If the mode is smart or strict, then the all four fields are validated to
		'''  their outer ranges. The date is then combined in a manner equivalent to
		'''  creating a date on the first day of the requested year and month, then adding
		'''  the amount in weeks and days to reach their values. If the mode is strict,
		'''  the date is additionally validated to check that the day and week adjustment
		'''  did not change the month.
		''' <li>{@code YEAR}, {@code MONTH_OF_YEAR}, {@code ALIGNED_WEEK_OF_MONTH} and
		'''  {@code DAY_OF_WEEK} - If all four are present, then they are combined to
		'''  form a date. The approach is the same as described above for
		'''  years, months and weeks in {@code ALIGNED_DAY_OF_WEEK_IN_MONTH}.
		'''  The day-of-week is adjusted as the next or same matching day-of-week once
		'''  the years, months and weeks have been handled.
		''' <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code ALIGNED_DAY_OF_WEEK_IN_YEAR} -
		'''  If all three are present, then they are combined to form a date.
		'''  In all three modes, the {@code YEAR} is validated.
		'''  If the mode is lenient, then the date is combined in a manner equivalent to
		'''  creating a date on the first day of the requested year, then adding
		'''  the difference in weeks, then in days.
		'''  If the mode is smart or strict, then the all three fields are validated to
		'''  their outer ranges. The date is then combined in a manner equivalent to
		'''  creating a date on the first day of the requested year, then adding
		'''  the amount in weeks and days to reach their values. If the mode is strict,
		'''  the date is additionally validated to check that the day and week adjustment
		'''  did not change the year.
		''' <li>{@code YEAR}, {@code ALIGNED_WEEK_OF_YEAR} and {@code DAY_OF_WEEK} -
		'''  If all three are present, then they are combined to form a date.
		'''  The approach is the same as described above for years and weeks in
		'''  {@code ALIGNED_DAY_OF_WEEK_IN_YEAR}. The day-of-week is adjusted as the
		'''  next or same matching day-of-week once the years and weeks have been handled.
		''' </ul>
		''' <p>
		''' The default implementation is suitable for most calendar systems.
		''' If <seealso cref="java.time.temporal.ChronoField#YEAR_OF_ERA"/> is found without an <seealso cref="java.time.temporal.ChronoField#ERA"/>
		''' then the last era in <seealso cref="#eras()"/> is used.
		''' The implementation assumes a 7 day week, that the first day-of-month
		''' has the value 1, that first day-of-year has the value 1, and that the
		''' first of the month and year always exists.
		''' </summary>
		''' <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		''' <param name="resolverStyle">  the requested type of resolve, not null </param>
		''' <returns> the resolved date, null if insufficient information to create a date </returns>
		''' <exception cref="java.time.DateTimeException"> if the date cannot be resolved, typically
		'''  because of a conflict in the input data </exception>
		public ChronoLocalDate resolveDate(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			' check epoch-day before inventing era
			If fieldValues.containsKey(EPOCH_DAY) Then Return dateEpochDay(fieldValues.remove(EPOCH_DAY))

			' fix proleptic month before inventing era
			resolveProlepticMonth(fieldValues, resolverStyle)

			' invent era if necessary to resolve year-of-era
			Dim resolved As ChronoLocalDate = resolveYearOfEra(fieldValues, resolverStyle)
			If resolved IsNot Nothing Then Return resolved

			' build date
			If fieldValues.containsKey(YEAR) Then
				If fieldValues.containsKey(MONTH_OF_YEAR) Then
					If fieldValues.containsKey(DAY_OF_MONTH) Then Return resolveYMD(fieldValues, resolverStyle)
					If fieldValues.containsKey(ALIGNED_WEEK_OF_MONTH) Then
						If fieldValues.containsKey(ALIGNED_DAY_OF_WEEK_IN_MONTH) Then Return resolveYMAA(fieldValues, resolverStyle)
						If fieldValues.containsKey(DAY_OF_WEEK) Then Return resolveYMAD(fieldValues, resolverStyle)
					End If
				End If
				If fieldValues.containsKey(DAY_OF_YEAR) Then Return resolveYD(fieldValues, resolverStyle)
				If fieldValues.containsKey(ALIGNED_WEEK_OF_YEAR) Then
					If fieldValues.containsKey(ALIGNED_DAY_OF_WEEK_IN_YEAR) Then Return resolveYAA(fieldValues, resolverStyle)
					If fieldValues.containsKey(DAY_OF_WEEK) Then Return resolveYAD(fieldValues, resolverStyle)
				End If
			End If
			Return Nothing

		void resolveProlepticMonth(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim pMonth As Long? = fieldValues.remove(PROLEPTIC_MONTH)
			If pMonth IsNot Nothing Then
				If resolverStyle <> java.time.format.ResolverStyle.LENIENT Then PROLEPTIC_MONTH.checkValidValue(pMonth)
				' first day-of-month is likely to be safest for setting proleptic-month
				' cannot add to year zero, as not all chronologies have a year zero
				Dim chronoDate As ChronoLocalDate = dateNow().with(DAY_OF_MONTH, 1).with(PROLEPTIC_MONTH, pMonth)
				addFieldValue(fieldValues, MONTH_OF_YEAR, chronoDate.get(MONTH_OF_YEAR))
				addFieldValue(fieldValues, YEAR, chronoDate.get(YEAR))
			End If

		ChronoLocalDate resolveYearOfEra(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim yoeLong As Long? = fieldValues.remove(YEAR_OF_ERA)
			If yoeLong IsNot Nothing Then
				Dim eraLong As Long? = fieldValues.remove(ERA)
				Dim yoe As Integer
				If resolverStyle <> java.time.format.ResolverStyle.LENIENT Then
					yoe = range(YEAR_OF_ERA).checkValidIntValue(yoeLong, YEAR_OF_ERA)
				Else
					yoe = System.Math.toIntExact(yoeLong)
				End If
				If eraLong IsNot Nothing Then
					Dim eraObj As Era = eraOf(range(ERA).checkValidIntValue(eraLong, ERA))
					addFieldValue(fieldValues, YEAR, prolepticYear(eraObj, yoe))
				Else
					If fieldValues.containsKey(YEAR) Then
						Dim year_Renamed As Integer = range(YEAR).checkValidIntValue(fieldValues.get(YEAR), YEAR)
						Dim chronoDate As ChronoLocalDate = dateYearDay(year_Renamed, 1)
						addFieldValue(fieldValues, YEAR, prolepticYear(chronoDate.era, yoe))
					ElseIf resolverStyle = java.time.format.ResolverStyle.STRICT Then
						' do not invent era if strict
						' reinstate the field removed earlier, no cross-check issues
						fieldValues.put(YEAR_OF_ERA, yoeLong)
					Else
						Dim eras As IList(Of Era) = eras()
						If eras.Count = 0 Then
							addFieldValue(fieldValues, YEAR, yoe)
						Else
							Dim eraObj As Era = eras(eras.Count - 1)
							addFieldValue(fieldValues, YEAR, prolepticYear(eraObj, yoe))
						End If
					End If
				End If
			ElseIf fieldValues.containsKey(ERA) Then
				range(ERA).checkValidValue(fieldValues.get(ERA), ERA) ' always validated
			End If
			Return Nothing

		ChronoLocalDate resolveYMD(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim months As Long = System.Math.subtractExact(fieldValues.remove(MONTH_OF_YEAR), 1)
				Dim days As Long = System.Math.subtractExact(fieldValues.remove(DAY_OF_MONTH), 1)
				Return [date](y, 1, 1).plus(months, MONTHS).plus(days, DAYS)
			End If
			Dim moy As Integer = range(MONTH_OF_YEAR).checkValidIntValue(fieldValues.remove(MONTH_OF_YEAR), MONTH_OF_YEAR)
			Dim domRange As java.time.temporal.ValueRange = range(DAY_OF_MONTH)
			Dim dom As Integer = domRange.checkValidIntValue(fieldValues.remove(DAY_OF_MONTH), DAY_OF_MONTH)
			If resolverStyle = java.time.format.ResolverStyle.SMART Then ' previous valid
				Try
					Return [date](y, moy, dom)
				Catch ex As java.time.DateTimeException
					Return [date](y, moy, 1).with(java.time.temporal.TemporalAdjusters.lastDayOfMonth())
				End Try
			End If
			Return [date](y, moy, dom)

		ChronoLocalDate resolveYD(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim days As Long = System.Math.subtractExact(fieldValues.remove(DAY_OF_YEAR), 1)
				Return dateYearDay(y, 1).plus(days, DAYS)
			End If
			Dim doy As Integer = range(DAY_OF_YEAR).checkValidIntValue(fieldValues.remove(DAY_OF_YEAR), DAY_OF_YEAR)
			Return dateYearDay(y, doy) ' smart is same as strict

		ChronoLocalDate resolveYMAA(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim months As Long = System.Math.subtractExact(fieldValues.remove(MONTH_OF_YEAR), 1)
				Dim weeks As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_WEEK_OF_MONTH), 1)
				Dim days As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_DAY_OF_WEEK_IN_MONTH), 1)
				Return [date](y, 1, 1).plus(months, MONTHS).plus(weeks, WEEKS).plus(days, DAYS)
			End If
			Dim moy As Integer = range(MONTH_OF_YEAR).checkValidIntValue(fieldValues.remove(MONTH_OF_YEAR), MONTH_OF_YEAR)
			Dim aw As Integer = range(ALIGNED_WEEK_OF_MONTH).checkValidIntValue(fieldValues.remove(ALIGNED_WEEK_OF_MONTH), ALIGNED_WEEK_OF_MONTH)
			Dim ad As Integer = range(ALIGNED_DAY_OF_WEEK_IN_MONTH).checkValidIntValue(fieldValues.remove(ALIGNED_DAY_OF_WEEK_IN_MONTH), ALIGNED_DAY_OF_WEEK_IN_MONTH)
			Dim date_Renamed As ChronoLocalDate = [date](y, moy, 1).plus((aw - 1) * 7 + (ad - 1), DAYS)
			If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.get(MONTH_OF_YEAR) <> moy Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different month")
			Return date_Renamed

		ChronoLocalDate resolveYMAD(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim months As Long = System.Math.subtractExact(fieldValues.remove(MONTH_OF_YEAR), 1)
				Dim weeks As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_WEEK_OF_MONTH), 1)
				Dim dow As Long = System.Math.subtractExact(fieldValues.remove(DAY_OF_WEEK), 1)
				Return resolveAligned([date](y, 1, 1), months, weeks, dow)
			End If
			Dim moy As Integer = range(MONTH_OF_YEAR).checkValidIntValue(fieldValues.remove(MONTH_OF_YEAR), MONTH_OF_YEAR)
			Dim aw As Integer = range(ALIGNED_WEEK_OF_MONTH).checkValidIntValue(fieldValues.remove(ALIGNED_WEEK_OF_MONTH), ALIGNED_WEEK_OF_MONTH)
			Dim dow As Integer = range(DAY_OF_WEEK).checkValidIntValue(fieldValues.remove(DAY_OF_WEEK), DAY_OF_WEEK)
			Dim date_Renamed As ChronoLocalDate = [date](y, moy, 1).plus((aw - 1) * 7, DAYS).with(nextOrSame(java.time.DayOfWeek.of(dow)))
			If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.get(MONTH_OF_YEAR) <> moy Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different month")
			Return date_Renamed

		ChronoLocalDate resolveYAA(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim weeks As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_WEEK_OF_YEAR), 1)
				Dim days As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_DAY_OF_WEEK_IN_YEAR), 1)
				Return dateYearDay(y, 1).plus(weeks, WEEKS).plus(days, DAYS)
			End If
			Dim aw As Integer = range(ALIGNED_WEEK_OF_YEAR).checkValidIntValue(fieldValues.remove(ALIGNED_WEEK_OF_YEAR), ALIGNED_WEEK_OF_YEAR)
			Dim ad As Integer = range(ALIGNED_DAY_OF_WEEK_IN_YEAR).checkValidIntValue(fieldValues.remove(ALIGNED_DAY_OF_WEEK_IN_YEAR), ALIGNED_DAY_OF_WEEK_IN_YEAR)
			Dim date_Renamed As ChronoLocalDate = dateYearDay(y, 1).plus((aw - 1) * 7 + (ad - 1), DAYS)
			If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.get(YEAR) <> y Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different year")
			Return date_Renamed

		ChronoLocalDate resolveYAD(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.format.ResolverStyle resolverStyle)
			Dim y As Integer = range(YEAR).checkValidIntValue(fieldValues.remove(YEAR), YEAR)
			If resolverStyle = java.time.format.ResolverStyle.LENIENT Then
				Dim weeks As Long = System.Math.subtractExact(fieldValues.remove(ALIGNED_WEEK_OF_YEAR), 1)
				Dim dow As Long = System.Math.subtractExact(fieldValues.remove(DAY_OF_WEEK), 1)
				Return resolveAligned(dateYearDay(y, 1), 0, weeks, dow)
			End If
			Dim aw As Integer = range(ALIGNED_WEEK_OF_YEAR).checkValidIntValue(fieldValues.remove(ALIGNED_WEEK_OF_YEAR), ALIGNED_WEEK_OF_YEAR)
			Dim dow As Integer = range(DAY_OF_WEEK).checkValidIntValue(fieldValues.remove(DAY_OF_WEEK), DAY_OF_WEEK)
			Dim date_Renamed As ChronoLocalDate = dateYearDay(y, 1).plus((aw - 1) * 7, DAYS).with(nextOrSame(java.time.DayOfWeek.of(dow)))
			If resolverStyle = java.time.format.ResolverStyle.STRICT AndAlso date_Renamed.get(YEAR) <> y Then Throw New java.time.DateTimeException("Strict mode rejected resolved date as it is in a different year")
			Return date_Renamed

		ChronoLocalDate resolveAligned(ChronoLocalDate base, Long months, Long weeks, Long dow)
			Dim date_Renamed As ChronoLocalDate = base.plus(months, MONTHS).plus(weeks, WEEKS)
			If dow > 7 Then
				date_Renamed = date_Renamed.plus((dow - 1) / 7, WEEKS)
				dow = ((dow - 1) Mod 7) + 1
			ElseIf dow < 1 Then
				date_Renamed = date_Renamed.plus (System.Math.subtractExact(dow, 7) \ 7, WEEKS)
				dow = ((dow + 6) Mod 7) + 1
			End If
			Return date_Renamed.with(nextOrSame(java.time.DayOfWeek.of(CInt(Fix(dow)))))

		''' <summary>
		''' Adds a field-value pair to the map, checking for conflicts.
		''' <p>
		''' If the field is not already present, then the field-value pair is added to the map.
		''' If the field is already present and it has the same value as that specified, no action occurs.
		''' If the field is already present and it has a different value to that specified, then
		''' an exception is thrown.
		''' </summary>
		''' <param name="field">  the field to add, not null </param>
		''' <param name="value">  the value to add, not null </param>
		''' <exception cref="java.time.DateTimeException"> if the field is already present with a different value </exception>
		void addFieldValue(IDictionary(Of java.time.temporal.TemporalField, Long?) fieldValues, java.time.temporal.ChronoField field, Long value)
			Dim old As Long? = fieldValues.get(field) ' check first for better error message
			If old IsNot Nothing AndAlso old <> value Then Throw New java.time.DateTimeException("Conflict found: " & field & " " & old & " differs from " & field & " " & value)
			fieldValues.put(field, value)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this chronology to another chronology.
		''' <p>
		''' The comparison order first by the chronology ID string, then by any
		''' additional information specific to the subclass.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' 
		''' @implSpec
		''' This implementation compares the chronology ID.
		''' Subclasses must compare any additional state that they store.
		''' </summary>
		''' <param name="other">  the other chronology to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		public Integer compareTo(Chronology other)
			Return id.CompareTo(other.id)

		''' <summary>
		''' Checks if this chronology is equal to another chronology.
		''' <p>
		''' The comparison is based on the entire state of the object.
		''' 
		''' @implSpec
		''' This implementation checks the type and calls
		''' <seealso cref="#compareTo(java.time.chrono.Chronology)"/>.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other chronology </returns>
		public Boolean Equals(Object obj)
			If Me Is obj Then Return True
			If TypeOf obj Is AbstractChronology Then Return compareTo(CType(obj, AbstractChronology)) = 0
			Return False

		''' <summary>
		''' A hash code for this chronology.
		''' <p>
		''' The hash code should be based on the entire state of the object.
		''' 
		''' @implSpec
		''' This implementation is based on the chronology ID and class.
		''' Subclasses should add any additional state that they store.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		public Integer GetHashCode()
			Return Me.GetType().GetHashCode() Xor id.GetHashCode()

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this chronology as a {@code String}, using the chronology ID.
		''' </summary>
		''' <returns> a string representation of this chronology, not null </returns>
		public String ToString()
			Return id

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the Chronology using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' <pre>
		'''  out.writeByte(1);  // identifies this as a Chronology
		'''  out.writeUTF(getId());
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Object writeReplace()
			Return New Ser(Ser.CHRONO_TYPE, Me)

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="java.io.InvalidObjectException"> always </exception>
		private  Sub  readObject(java.io.ObjectInputStream s) throws java.io.ObjectStreamException
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")

		void writeExternal(java.io.DataOutput out) throws java.io.IOException
			out.writeUTF(id)

		static Chronology readExternal(java.io.DataInput in) throws java.io.IOException
			Dim id_Renamed As String = in.readUTF()
			Return Chronology.of(id_Renamed)

	End Class

End Namespace