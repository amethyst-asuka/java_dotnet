Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A calendar system, used to organize and identify dates.
	''' <p>
	''' The main date and time API is built on the ISO calendar system.
	''' The chronology operates behind the scenes to represent the general concept of a calendar system.
	''' For example, the Japanese, Minguo, Thai Buddhist and others.
	''' <p>
	''' Most other calendar systems also operate on the shared concepts of year, month and day,
	''' linked to the cycles of the Earth around the Sun, and the Moon around the Earth.
	''' These shared concepts are defined by <seealso cref="ChronoField"/> and are available
	''' for use by any {@code Chronology} implementation:
	''' <pre>
	'''   LocalDate isoDate = ...
	'''   ThaiBuddhistDate thaiDate = ...
	'''   int isoYear = isoDate.get(ChronoField.YEAR);
	'''   int thaiYear = thaiDate.get(ChronoField.YEAR);
	''' </pre>
	''' As shown, although the date objects are in different calendar systems, represented by different
	''' {@code Chronology} instances, both can be queried using the same constant on {@code ChronoField}.
	''' For a full discussion of the implications of this, see <seealso cref="ChronoLocalDate"/>.
	''' In general, the advice is to use the known ISO-based {@code LocalDate}, rather than
	''' {@code ChronoLocalDate}.
	''' <p>
	''' While a {@code Chronology} object typically uses {@code ChronoField} and is based on
	''' an era, year-of-era, month-of-year, day-of-month model of a date, this is not required.
	''' A {@code Chronology} instance may represent a totally different kind of calendar system,
	''' such as the Mayan.
	''' <p>
	''' In practical terms, the {@code Chronology} instance also acts as a factory.
	''' The <seealso cref="#of(String)"/> method allows an instance to be looked up by identifier,
	''' while the <seealso cref="#ofLocale(Locale)"/> method allows lookup by locale.
	''' <p>
	''' The {@code Chronology} instance provides a set of methods to create {@code ChronoLocalDate} instances.
	''' The date classes are used to manipulate specific dates.
	''' <ul>
	''' <li> <seealso cref="#dateNow() dateNow()"/>
	''' <li> <seealso cref="#dateNow(Clock) dateNow(clock)"/>
	''' <li> <seealso cref="#dateNow(ZoneId) dateNow(zone)"/>
	''' <li> <seealso cref="#date(int, int, int) date(yearProleptic, month, day)"/>
	''' <li> <seealso cref="#date(Era, int, int, int) date(era, yearOfEra, month, day)"/>
	''' <li> <seealso cref="#dateYearDay(int, int) dateYearDay(yearProleptic, dayOfYear)"/>
	''' <li> <seealso cref="#dateYearDay(Era, int, int) dateYearDay(era, yearOfEra, dayOfYear)"/>
	''' <li> <seealso cref="#date(TemporalAccessor) date(TemporalAccessor)"/>
	''' </ul>
	''' 
	''' <h3 id="addcalendars">Adding New Calendars</h3>
	''' The set of available chronologies can be extended by applications.
	''' Adding a new calendar system requires the writing of an implementation of
	''' {@code Chronology}, {@code ChronoLocalDate} and {@code Era}.
	''' The majority of the logic specific to the calendar system will be in the
	''' {@code ChronoLocalDate} implementation.
	''' The {@code Chronology} implementation acts as a factory.
	''' <p>
	''' To permit the discovery of additional chronologies, the <seealso cref="java.util.ServiceLoader ServiceLoader"/>
	''' is used. A file must be added to the {@code META-INF/services} directory with the
	''' name 'java.time.chrono.Chronology' listing the implementation classes.
	''' See the ServiceLoader for more details on service loading.
	''' For lookup by id or calendarType, the system provided calendars are found
	''' first followed by application provided calendars.
	''' <p>
	''' Each chronology must define a chronology ID that is unique within the system.
	''' If the chronology represents a calendar system defined by the
	''' CLDR specification then the calendar type is the concatenation of the
	''' CLDR type and, if applicable, the CLDR variant,
	''' 
	''' @implSpec
	''' This interface must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' Subclasses should be Serializable wherever possible.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface Chronology
		Inherits Comparable(Of Chronology)

		''' <summary>
		''' Obtains an instance of {@code Chronology} from a temporal object.
		''' <p>
		''' This obtains a chronology based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code Chronology}.
		''' <p>
		''' The conversion will obtain the chronology using <seealso cref="TemporalQueries#chronology()"/>.
		''' If the specified temporal object does not have a chronology, <seealso cref="IsoChronology"/> is returned.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code Chronology::from}.
		''' </summary>
		''' <param name="temporal">  the temporal to convert, not null </param>
		''' <returns> the chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to an {@code Chronology} </exception>
		Shared Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As Chronology
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(temporal, "temporal");
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Chronology obj = temporal.query(java.time.temporal.TemporalQueries.chronology());
			Sub [New](obj != Nothing ? obj : ByVal IsoChronology.INSTANCE As )

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Chronology} from a locale.
		''' <p>
		''' This returns a {@code Chronology} based on the specified locale,
		''' typically returning {@code IsoChronology}. Other calendar systems
		''' are only returned if they are explicitly selected within the locale.
		''' <p>
		''' The <seealso cref="Locale"/> class provide access to a range of information useful
		''' for localizing an application. This includes the language and region,
		''' such as "en-GB" for English as used in Great Britain.
		''' <p>
		''' The {@code Locale} class also supports an extension mechanism that
		''' can be used to identify a calendar system. The mechanism is a form
		''' of key-value pairs, where the calendar system has the key "ca".
		''' For example, the locale "en-JP-u-ca-japanese" represents the English
		''' language as used in Japan with the Japanese calendar system.
		''' <p>
		''' This method finds the desired calendar system by in a manner equivalent
		''' to passing "ca" to <seealso cref="Locale#getUnicodeLocaleType(String)"/>.
		''' If the "ca" key is not present, then {@code IsoChronology} is returned.
		''' <p>
		''' Note that the behavior of this method differs from the older
		''' <seealso cref="java.util.Calendar#getInstance(Locale)"/> method.
		''' If that method receives a locale of "th_TH" it will return {@code BuddhistCalendar}.
		''' By contrast, this method will return {@code IsoChronology}.
		''' Passing the locale "th-TH-u-ca-buddhist" into either method will
		''' result in the Thai Buddhist calendar system and is therefore the
		''' recommended approach going forward for Thai calendar system localization.
		''' <p>
		''' A similar, but simpler, situation occurs for the Japanese calendar system.
		''' The locale "jp_JP_JP" has previously been used to access the calendar.
		''' However, unlike the Thai locale, "ja_JP_JP" is automatically converted by
		''' {@code Locale} to the modern and recommended form of "ja-JP-u-ca-japanese".
		''' Thus, there is no difference in behavior between this method and
		''' {@code Calendar#getInstance(Locale)}.
		''' </summary>
		''' <param name="locale">  the locale to use to obtain the calendar system, not null </param>
		''' <returns> the calendar system associated with the locale, not null </returns>
		''' <exception cref="DateTimeException"> if the locale-specified calendar cannot be found </exception>
		Shared Function ofLocale(ByVal locale As java.util.Locale) As Chronology
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return AbstractChronology.ofLocale(locale);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Chronology} from a chronology ID or
		''' calendar system type.
		''' <p>
		''' This returns a chronology based on either the ID or the type.
		''' The <seealso cref="#getId() chronology ID"/> uniquely identifies the chronology.
		''' The <seealso cref="#getCalendarType() calendar system type"/> is defined by the
		''' CLDR specification.
		''' <p>
		''' The chronology may be a system chronology or a chronology
		''' provided by the application via ServiceLoader configuration.
		''' <p>
		''' Since some calendars can be customized, the ID or type typically refers
		''' to the default customization. For example, the Gregorian calendar can have multiple
		''' cutover dates from the Julian, but the lookup only provides the default cutover date.
		''' </summary>
		''' <param name="id">  the chronology ID or calendar system type, not null </param>
		''' <returns> the chronology with the identifier requested, not null </returns>
		''' <exception cref="DateTimeException"> if the chronology cannot be found </exception>
		Shared Function [of](ByVal id As String) As Chronology
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return AbstractChronology.of(id);

		''' <summary>
		''' Returns the available chronologies.
		''' <p>
		''' Each returned {@code Chronology} is available for use in the system.
		''' The set of chronologies includes the system chronologies and
		''' any chronologies provided by the application via ServiceLoader
		''' configuration.
		''' </summary>
		''' <returns> the independent, modifiable set of the available chronology IDs, not null </returns>
		ReadOnly Property Shared availableChronologies As java.util.Set(Of Chronology)
			ReadOnly Property AbstractChronology.getAvailableChronologies() As [Return]

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the ID of the chronology.
		''' <p>
		''' The ID uniquely identifies the {@code Chronology}.
		''' It can be used to lookup the {@code Chronology} using <seealso cref="#of(String)"/>.
		''' </summary>
		''' <returns> the chronology ID, not null </returns>
		''' <seealso cref= #getCalendarType() </seealso>
		ReadOnly Property id As String

		''' <summary>
		''' Gets the calendar type of the calendar system.
		''' <p>
		''' The calendar type is an identifier defined by the CLDR and
		''' <em>Unicode Locale Data Markup Language (LDML)</em> specifications
		''' to uniquely identification a calendar.
		''' The {@code getCalendarType} is the concatenation of the CLDR calendar type
		''' and the variant, if applicable, is appended separated by "-".
		''' The calendar type is used to lookup the {@code Chronology} using <seealso cref="#of(String)"/>.
		''' </summary>
		''' <returns> the calendar system type, null if the calendar is not defined by CLDR/LDML </returns>
		''' <seealso cref= #getId() </seealso>
		ReadOnly Property calendarType As String

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a local date in this chronology from the era, year-of-era,
		''' month-of-year and day-of-month fields.
		''' 
		''' @implSpec
		''' The default implementation combines the era and year-of-era into a proleptic
		''' year before calling <seealso cref="#date(int, int, int)"/>.
		''' </summary>
		''' <param name="era">  the era of the correct type for the chronology, not null </param>
		''' <param name="yearOfEra">  the chronology year-of-era </param>
		''' <param name="month">  the chronology month-of-year </param>
		''' <param name="dayOfMonth">  the chronology day-of-month </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not of the correct type for the chronology </exception>
		default Function [date](ByVal era As Era, ByVal yearOfEra As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As ChronoLocalDate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return date(prolepticYear(era, yearOfEra), month, dayOfMonth);

		''' <summary>
		''' Obtains a local date in this chronology from the proleptic-year,
		''' month-of-year and day-of-month fields.
		''' </summary>
		''' <param name="prolepticYear">  the chronology proleptic-year </param>
		''' <param name="month">  the chronology month-of-year </param>
		''' <param name="dayOfMonth">  the chronology day-of-month </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Function [date](ByVal prolepticYear As Integer, ByVal month As Integer, ByVal dayOfMonth As Integer) As ChronoLocalDate

		''' <summary>
		''' Obtains a local date in this chronology from the era, year-of-era and
		''' day-of-year fields.
		''' 
		''' @implSpec
		''' The default implementation combines the era and year-of-era into a proleptic
		''' year before calling <seealso cref="#dateYearDay(int, int)"/>.
		''' </summary>
		''' <param name="era">  the era of the correct type for the chronology, not null </param>
		''' <param name="yearOfEra">  the chronology year-of-era </param>
		''' <param name="dayOfYear">  the chronology day-of-year </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not of the correct type for the chronology </exception>
		default Function dateYearDay(ByVal era As Era, ByVal yearOfEra As Integer, ByVal dayOfYear As Integer) As ChronoLocalDate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return dateYearDay(prolepticYear(era, yearOfEra), dayOfYear);

		''' <summary>
		''' Obtains a local date in this chronology from the proleptic-year and
		''' day-of-year fields.
		''' </summary>
		''' <param name="prolepticYear">  the chronology proleptic-year </param>
		''' <param name="dayOfYear">  the chronology day-of-year </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Function dateYearDay(ByVal prolepticYear As Integer, ByVal dayOfYear As Integer) As ChronoLocalDate

		''' <summary>
		''' Obtains a local date in this chronology from the epoch-day.
		''' <p>
		''' The definition of <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/> is the same
		''' for all calendar systems, thus it can be used for conversion.
		''' </summary>
		''' <param name="epochDay">  the epoch day </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		Function dateEpochDay(ByVal epochDay As Long) As ChronoLocalDate

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains the current local date in this chronology from the system clock in the default time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#systemDefaultZone() system clock"/> in the default
		''' time-zone to obtain the current date.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' 
		''' @implSpec
		''' The default implementation invokes <seealso cref="#dateNow(Clock)"/>.
		''' </summary>
		''' <returns> the current local date using the system clock and default time-zone, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		default Function dateNow() As ChronoLocalDate
			Function dateNow(java.time.Clock.systemDefaultZone() ByVal  As ) As [Return]

		''' <summary>
		''' Obtains the current local date in this chronology from the system clock in the specified time-zone.
		''' <p>
		''' This will query the <seealso cref="Clock#system(ZoneId) system clock"/> to obtain the current date.
		''' Specifying the time-zone avoids dependence on the default time-zone.
		''' <p>
		''' Using this method will prevent the ability to use an alternate clock for testing
		''' because the clock is hard-coded.
		''' 
		''' @implSpec
		''' The default implementation invokes <seealso cref="#dateNow(Clock)"/>.
		''' </summary>
		''' <param name="zone">  the zone ID to use, not null </param>
		''' <returns> the current local date using the system clock, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		default Function dateNow(ByVal zone As java.time.ZoneId) As ChronoLocalDate
			Function dateNow(java.time.Clock.system(zone) ByVal  As ) As [Return]

		''' <summary>
		''' Obtains the current local date in this chronology from the specified clock.
		''' <p>
		''' This will query the specified clock to obtain the current date - today.
		''' Using this method allows the use of an alternate clock for testing.
		''' The alternate clock may be introduced using <seealso cref="Clock dependency injection"/>.
		''' 
		''' @implSpec
		''' The default implementation invokes <seealso cref="#date(TemporalAccessor)"/>.
		''' </summary>
		''' <param name="clock">  the clock to use, not null </param>
		''' <returns> the current local date, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		default Function dateNow(ByVal clock As java.time.Clock) As ChronoLocalDate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(clock, "clock");
			Function [date](java.time.LocalDate.now(clock) ByVal  As ) As [Return]

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a local date in this chronology from another temporal object.
		''' <p>
		''' This obtains a date in this chronology based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ChronoLocalDate}.
		''' <p>
		''' The conversion typically uses the <seealso cref="ChronoField#EPOCH_DAY EPOCH_DAY"/>
		''' field, which is standardized across calendar systems.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code aChronology::date}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the local date in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date </exception>
		''' <seealso cref= ChronoLocalDate#from(TemporalAccessor) </seealso>
		Function [date](ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDate

		''' <summary>
		''' Obtains a local date-time in this chronology from another temporal object.
		''' <p>
		''' This obtains a date-time in this chronology based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ChronoLocalDateTime}.
		''' <p>
		''' The conversion extracts and combines the {@code ChronoLocalDate} and the
		''' {@code LocalTime} from the temporal object.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' The result uses this chronology.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code aChronology::localDateTime}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the local date-time in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date-time </exception>
		''' <seealso cref= ChronoLocalDateTime#from(TemporalAccessor) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		default Function localDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDateTime(Of ? As ChronoLocalDate)
			Try
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				Return date(temporal).atTime(java.time.LocalTime.from(temporal));
			Sub [New](ByVal ex As java.time.DateTimeException)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				throw New java.time.DateTimeException("Unable to obtain ChronoLocalDateTime from TemporalAccessor: " & temporal.getClass(), ex);

		''' <summary>
		''' Obtains a {@code ChronoZonedDateTime} in this chronology from another temporal object.
		''' <p>
		''' This obtains a zoned date-time in this chronology based on the specified temporal.
		''' A {@code TemporalAccessor} represents an arbitrary set of date and time information,
		''' which this factory converts to an instance of {@code ChronoZonedDateTime}.
		''' <p>
		''' The conversion will first obtain a {@code ZoneId} from the temporal object,
		''' falling back to a {@code ZoneOffset} if necessary. It will then try to obtain
		''' an {@code Instant}, falling back to a {@code ChronoLocalDateTime} if necessary.
		''' The result will be either the combination of {@code ZoneId} or {@code ZoneOffset}
		''' with {@code Instant} or {@code ChronoLocalDateTime}.
		''' Implementations are permitted to perform optimizations such as accessing
		''' those fields that are equivalent to the relevant objects.
		''' The result uses this chronology.
		''' <p>
		''' This method matches the signature of the functional interface <seealso cref="TemporalQuery"/>
		''' allowing it to be used as a query via method reference, {@code aChronology::zonedDateTime}.
		''' </summary>
		''' <param name="temporal">  the temporal object to convert, not null </param>
		''' <returns> the zoned date-time in this chronology, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the date-time </exception>
		''' <seealso cref= ChronoZonedDateTime#from(TemporalAccessor) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		default Function zonedDateTime(ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoZonedDateTime(Of ? As ChronoLocalDate)
			Try
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'				java.time.ZoneId zone = java.time.ZoneId.from(temporal);
				Try
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					java.time.Instant instant = java.time.Instant.from(temporal);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					Return zonedDateTime(instant, zone);

				Sub [New](ByVal ex1 As java.time.DateTimeException)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'					ChronoLocalDateTimeImpl(Of JavaToDotNetGenericWildcard) cldt = ChronoLocalDateTimeImpl.ensureValid(Me, localDateTime(temporal));
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					Return ChronoZonedDateTimeImpl.ofBest(cldt, zone, Nothing);
			Sub [New](ByVal ex As java.time.DateTimeException)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				throw New java.time.DateTimeException("Unable to obtain ChronoZonedDateTime from TemporalAccessor: " & temporal.getClass(), ex);

		''' <summary>
		''' Obtains a {@code ChronoZonedDateTime} in this chronology from an {@code Instant}.
		''' <p>
		''' This obtains a zoned date-time with the same instant as that specified.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone, not null </param>
		''' <returns> the zoned date-time, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported range </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		default Function zonedDateTime(ByVal instant As java.time.Instant, ByVal zone As java.time.ZoneId) As ChronoZonedDateTime(Of ? As ChronoLocalDate)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return ChronoZonedDateTimeImpl.ofInstant(Me, instant, zone);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if the specified year is a leap year.
		''' <p>
		''' A leap-year is a year of a longer length than normal.
		''' The exact meaning is determined by the chronology according to the following constraints.
		''' <ul>
		''' <li>a leap-year must imply a year-length longer than a non leap-year.
		''' <li>a chronology that does not support the concept of a year must return false.
		''' </ul>
		''' </summary>
		''' <param name="prolepticYear">  the proleptic-year to check, not validated for range </param>
		''' <returns> true if the year is a leap year </returns>
		Function isLeapYear(ByVal prolepticYear As Long) As Boolean

		''' <summary>
		''' Calculates the proleptic-year given the era and year-of-era.
		''' <p>
		''' This combines the era and year-of-era into the single proleptic-year field.
		''' <p>
		''' If the chronology makes active use of eras, such as {@code JapaneseChronology}
		''' then the year-of-era will be validated against the era.
		''' For other chronologies, validation is optional.
		''' </summary>
		''' <param name="era">  the era of the correct type for the chronology, not null </param>
		''' <param name="yearOfEra">  the chronology year-of-era </param>
		''' <returns> the proleptic-year </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a proleptic-year,
		'''  such as if the year is invalid for the era </exception>
		''' <exception cref="ClassCastException"> if the {@code era} is not of the correct type for the chronology </exception>
		Function prolepticYear(ByVal era As Era, ByVal yearOfEra As Integer) As Integer

		''' <summary>
		''' Creates the chronology era object from the numeric value.
		''' <p>
		''' The era is, conceptually, the largest division of the time-line.
		''' Most calendar systems have a single epoch dividing the time-line into two eras.
		''' However, some have multiple eras, such as one for the reign of each leader.
		''' The exact meaning is determined by the chronology according to the following constraints.
		''' <p>
		''' The era in use at 1970-01-01 must have the value 1.
		''' Later eras must have sequentially higher values.
		''' Earlier eras must have sequentially lower values.
		''' Each chronology must refer to an enum or similar singleton to provide the era values.
		''' <p>
		''' This method returns the singleton era of the correct type for the specified era value.
		''' </summary>
		''' <param name="eraValue">  the era value </param>
		''' <returns> the calendar system era, not null </returns>
		''' <exception cref="DateTimeException"> if unable to create the era </exception>
		Function eraOf(ByVal eraValue As Integer) As Era

		''' <summary>
		''' Gets the list of eras for the chronology.
		''' <p>
		''' Most calendar systems have an era, within which the year has meaning.
		''' If the calendar system does not support the concept of eras, an empty
		''' list must be returned.
		''' </summary>
		''' <returns> the list of eras for the chronology, may be immutable, not null </returns>
		Function eras() As IList(Of Era)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' All fields can be expressed as a {@code long}  java.lang.[Integer].
		''' This method returns an object that describes the valid range for that value.
		''' <p>
		''' Note that the result only describes the minimum and maximum valid values
		''' and it is important not to read too much into them. For example, there
		''' could be values within the range that are invalid for the field.
		''' <p>
		''' This method will return a result whether or not the chronology supports the field.
		''' </summary>
		''' <param name="field">  the field to get the range for, not null </param>
		''' <returns> the range of valid values for the field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		Function range(ByVal field As java.time.temporal.ChronoField) As java.time.temporal.ValueRange

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the textual representation of this chronology.
		''' <p>
		''' This returns the textual name used to identify the chronology,
		''' suitable for presentation to the user.
		''' The parameters control the style of the returned text and the locale.
		''' 
		''' @implSpec
		''' The default implementation behaves as though the formatter was used to
		''' format the chronology textual name.
		''' </summary>
		''' <param name="style">  the style of the text required, not null </param>
		''' <param name="locale">  the locale to use, not null </param>
		''' <returns> the text value of the chronology, not null </returns>
		default Function getDisplayName(ByVal style As java.time.format.TextStyle, ByVal locale As java.util.Locale) As String
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			java.time.temporal.TemporalAccessor temporal = New TemporalAccessorAnonymousInnerClassHelper();
			Return Function java.time.format.DateTimeFormatterBuilder() As New

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
		''' The default implementation, which explains typical resolve behaviour,
		''' is provided in <seealso cref="AbstractChronology"/>.
		''' </summary>
		''' <param name="fieldValues">  the map of fields to values, which can be updated, not null </param>
		''' <param name="resolverStyle">  the requested type of resolve, not null </param>
		''' <returns> the resolved date, null if insufficient information to create a date </returns>
		''' <exception cref="DateTimeException"> if the date cannot be resolved, typically
		'''  because of a conflict in the input data </exception>
		Function resolveDate(ByVal fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?), ByVal resolverStyle As java.time.format.ResolverStyle) As ChronoLocalDate

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a period for this chronology based on years, months and days.
		''' <p>
		''' This returns a period tied to this chronology using the specified
		''' years, months and days.  All supplied chronologies use periods
		''' based on years, months and days, however the {@code ChronoPeriod} API
		''' allows the period to be represented using other units.
		''' 
		''' @implSpec
		''' The default implementation returns an implementation class suitable
		''' for most calendar systems. It is based solely on the three units.
		''' Normalization, addition and subtraction derive the number of months
		''' in a year from the <seealso cref="#range(ChronoField)"/>. If the number of
		''' months within a year is fixed, then the calculation approach for
		''' addition, subtraction and normalization is slightly different.
		''' <p>
		''' If implementing an unusual calendar system that is not based on
		''' years, months and days, or where you want direct control, then
		''' the {@code ChronoPeriod} interface must be directly implemented.
		''' <p>
		''' The returned period is immutable and thread-safe.
		''' </summary>
		''' <param name="years">  the number of years, may be negative </param>
		''' <param name="months">  the number of years, may be negative </param>
		''' <param name="days">  the number of years, may be negative </param>
		''' <returns> the period in terms of this chronology, not null </returns>
		default Function period(ByVal years As Integer, ByVal months As Integer, ByVal days As Integer) As ChronoPeriod
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return New ChronoPeriodImpl(Me, years, months, days);

		'-----------------------------------------------------------------------
		''' <summary>
		''' Compares this chronology to another chronology.
		''' <p>
		''' The comparison order first by the chronology ID string, then by any
		''' additional information specific to the subclass.
		''' It is "consistent with equals", as defined by <seealso cref="Comparable"/>.
		''' </summary>
		''' <param name="other">  the other chronology to compare to, not null </param>
		''' <returns> the comparator value, negative if less, positive if greater </returns>
		Overrides Function compareTo(ByVal other As Chronology) As Integer

		''' <summary>
		''' Checks if this chronology is equal to another chronology.
		''' <p>
		''' The comparison is based on the entire state of the object.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other chronology </returns>
		Overrides Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' A hash code for this chronology.
		''' <p>
		''' The hash code should be based on the entire state of the object.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Overrides Function GetHashCode() As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this chronology as a {@code String}.
		''' <p>
		''' The format should include the entire state of the object.
		''' </summary>
		''' <returns> a string representation of this chronology, not null </returns>
		Overrides Function ToString() As String

	End Interface


	Private Class TemporalAccessorAnonymousInnerClassHelper
		Implements java.time.temporal.TemporalAccessor

		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean
			Return False
		End Function
		Public Overrides Function getLong(ByVal field As java.time.temporal.TemporalField) As Long
			Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
		End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function query(Of R)(ByVal query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
			If query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then Return CType(Chronology.this, R)
			Return outerInstance.query(query_Renamed)
		End Function
	End Class
End Namespace