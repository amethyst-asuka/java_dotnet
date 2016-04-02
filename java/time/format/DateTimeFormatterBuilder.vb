Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
' * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights hg qreserved.
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
Namespace java.time.format




	''' <summary>
	''' Builder to create date-time formatters.
	''' <p>
	''' This allows a {@code DateTimeFormatter} to be created.
	''' All date-time formatters are created ultimately using this builder.
	''' <p>
	''' The basic elements of date-time can all be added:
	''' <ul>
	''' <li>Value - a numeric value</li>
	''' <li>Fraction - a fractional value including the decimal place. Always use this when
	''' outputting fractions to ensure that the fraction is parsed correctly</li>
	''' <li>Text - the textual equivalent for the value</li>
	''' <li>OffsetId/Offset - the <seealso cref="ZoneOffset zone offset"/></li>
	''' <li>ZoneId - the <seealso cref="ZoneId time-zone"/> id</li>
	''' <li>ZoneText - the name of the time-zone</li>
	''' <li>ChronologyId - the <seealso cref="Chronology chronology"/> id</li>
	''' <li>ChronologyText - the name of the chronology</li>
	''' <li>Literal - a text literal</li>
	''' <li>Nested and Optional - formats can be nested or made optional</li>
	''' </ul>
	''' In addition, any of the elements may be decorated by padding, either with spaces or any other character.
	''' <p>
	''' Finally, a shorthand pattern, mostly compatible with {@code java.text.SimpleDateFormat SimpleDateFormat}
	''' can be used, see <seealso cref="#appendPattern(String)"/>.
	''' In practice, this simply parses the pattern and calls other methods on the builder.
	''' 
	''' @implSpec
	''' This class is a mutable builder intended for use from a single thread.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class DateTimeFormatterBuilder

		''' <summary>
		''' Query for a time-zone that is region-only.
		''' </summary>
		private static final java.time.temporal.TemporalQuery(Of java.time.ZoneId) QUERY_REGION_ONLY = (temporal) ->
			Dim zone As java.time.ZoneId = temporal.query(java.time.temporal.TemporalQueries.zoneId())
			Return (If(zone IsNot Nothing AndAlso TypeOf zone Is java.time.ZoneOffset = False, zone, Nothing))

		''' <summary>
		''' The currently active builder, used by the outermost builder.
		''' </summary>
		private DateTimeFormatterBuilder active = Me
		''' <summary>
		''' The parent builder, null for the outermost builder.
		''' </summary>
		private final DateTimeFormatterBuilder parent
		''' <summary>
		''' The list of printers that will be used.
		''' </summary>
		private final IList(Of DateTimePrinterParser) printerParsers = New List(Of )
		''' <summary>
		''' Whether this builder produces an optional formatter.
		''' </summary>
		private final Boolean [optional]
		''' <summary>
		''' The width to pad the next field to.
		''' </summary>
		private Integer padNextWidth
		''' <summary>
		''' The character to pad the next field with.
		''' </summary>
		private Char padNextChar
		''' <summary>
		''' The index of the last variable width value parser.
		''' </summary>
		private Integer valueParserIndex = -1

		''' <summary>
		''' Gets the formatting pattern for date and time styles for a locale and chronology.
		''' The locale and chronology are used to lookup the locale specific format
		''' for the requested dateStyle and/or timeStyle.
		''' </summary>
		''' <param name="dateStyle">  the FormatStyle for the date, null for time-only pattern </param>
		''' <param name="timeStyle">  the FormatStyle for the time, null for date-only pattern </param>
		''' <param name="chrono">  the Chronology, non-null </param>
		''' <param name="locale">  the locale, non-null </param>
		''' <returns> the locale and Chronology specific formatting pattern </returns>
		''' <exception cref="IllegalArgumentException"> if both dateStyle and timeStyle are null </exception>
		Public Shared String getLocalizedDateTimePattern(FormatStyle dateStyle, FormatStyle timeStyle, java.time.chrono.Chronology chrono, java.util.Locale locale)
			java.util.Objects.requireNonNull(locale, "locale")
			java.util.Objects.requireNonNull(chrono, "chrono")
			If dateStyle Is Nothing AndAlso timeStyle Is Nothing Then Throw New IllegalArgumentException("Either dateStyle or timeStyle must be non-null")
			Dim lr As sun.util.locale.provider.LocaleResources = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased.getLocaleResources(locale)
			Dim pattern As String = lr.getJavaTimeDateTimePattern(convertStyle(timeStyle), convertStyle(dateStyle), chrono.calendarType)
			Return pattern

		''' <summary>
		''' Converts the given FormatStyle to the java.text.DateFormat style.
		''' </summary>
		''' <param name="style">  the FormatStyle style </param>
		''' <returns> the int style, or -1 if style is null, indicating un-required </returns>
		private static Integer convertStyle(FormatStyle style)
			If style Is Nothing Then Return -1
			Return style.ordinal() ' indices happen to align

		''' <summary>
		''' Constructs a new instance of the builder.
		''' </summary>
		public DateTimeFormatterBuilder()
			MyBase()
			parent = Nothing
			[optional] = False

		''' <summary>
		''' Constructs a new instance of the builder.
		''' </summary>
		''' <param name="parent">  the parent builder, not null </param>
		''' <param name="optional">  whether the formatter is optional, not null </param>
		private DateTimeFormatterBuilder(DateTimeFormatterBuilder parent, Boolean [optional])
			MyBase()
			Me.parent = parent
			Me.optional = [optional]

		'-----------------------------------------------------------------------
		''' <summary>
		''' Changes the parse style to be case sensitive for the remainder of the formatter.
		''' <p>
		''' Parsing can be case sensitive or insensitive - by default it is case sensitive.
		''' This method allows the case sensitivity setting of parsing to be changed.
		''' <p>
		''' Calling this method changes the state of the builder such that all
		''' subsequent builder method calls will parse text in case sensitive mode.
		''' See <seealso cref="#parseCaseInsensitive"/> for the opposite setting.
		''' The parse case sensitive/insensitive methods may be called at any point
		''' in the builder, thus the parser can swap between case parsing modes
		''' multiple times during the parse.
		''' <p>
		''' Since the default is case sensitive, this method should only be used after
		''' a previous call to {@code #parseCaseInsensitive}.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder parseCaseSensitive()
			appendInternal(SettingsParser.SENSITIVE)
			Return Me

		''' <summary>
		''' Changes the parse style to be case insensitive for the remainder of the formatter.
		''' <p>
		''' Parsing can be case sensitive or insensitive - by default it is case sensitive.
		''' This method allows the case sensitivity setting of parsing to be changed.
		''' <p>
		''' Calling this method changes the state of the builder such that all
		''' subsequent builder method calls will parse text in case insensitive mode.
		''' See <seealso cref="#parseCaseSensitive()"/> for the opposite setting.
		''' The parse case sensitive/insensitive methods may be called at any point
		''' in the builder, thus the parser can swap between case parsing modes
		''' multiple times during the parse.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder parseCaseInsensitive()
			appendInternal(SettingsParser.INSENSITIVE)
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Changes the parse style to be strict for the remainder of the formatter.
		''' <p>
		''' Parsing can be strict or lenient - by default its strict.
		''' This controls the degree of flexibility in matching the text and sign styles.
		''' <p>
		''' When used, this method changes the parsing to be strict from this point onwards.
		''' As strict is the default, this is normally only needed after calling <seealso cref="#parseLenient()"/>.
		''' The change will remain in force until the end of the formatter that is eventually
		''' constructed or until {@code parseLenient} is called.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder parseStrict()
			appendInternal(SettingsParser.STRICT)
			Return Me

		''' <summary>
		''' Changes the parse style to be lenient for the remainder of the formatter.
		''' Note that case sensitivity is set separately to this method.
		''' <p>
		''' Parsing can be strict or lenient - by default its strict.
		''' This controls the degree of flexibility in matching the text and sign styles.
		''' Applications calling this method should typically also call <seealso cref="#parseCaseInsensitive()"/>.
		''' <p>
		''' When used, this method changes the parsing to be lenient from this point onwards.
		''' The change will remain in force until the end of the formatter that is eventually
		''' constructed or until {@code parseStrict} is called.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder parseLenient()
			appendInternal(SettingsParser.LENIENT)
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends a default value for a field to the formatter for use in parsing.
		''' <p>
		''' This appends an instruction to the builder to inject a default value
		''' into the parsed result. This is especially useful in conjunction with
		''' optional parts of the formatter.
		''' <p>
		''' For example, consider a formatter that parses the year, followed by
		''' an optional month, with a further optional day-of-month. Using such a
		''' formatter would require the calling code to check whether a full date,
		''' year-month or just a year had been parsed. This method can be used to
		''' default the month and day-of-month to a sensible value, such as the
		''' first of the month, allowing the calling code to always get a date.
		''' <p>
		''' During formatting, this method has no effect.
		''' <p>
		''' During parsing, the current state of the parse is inspected.
		''' If the specified field has no associated value, because it has not been
		''' parsed successfully at that point, then the specified value is injected
		''' into the parse result. Injection is immediate, thus the field-value pair
		''' will be visible to any subsequent elements in the formatter.
		''' As such, this method is normally called at the end of the builder.
		''' </summary>
		''' <param name="field">  the field to default the value of, not null </param>
		''' <param name="value">  the value to default the field to </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder parseDefaulting(java.time.temporal.TemporalField field, Long value)
			java.util.Objects.requireNonNull(field, "field")
			appendInternal(New DefaultValueParser(field, value))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the value of a date-time field to the formatter using a normal
		''' output style.
		''' <p>
		''' The value of the field will be output during a format.
		''' If the value cannot be obtained then an exception will be thrown.
		''' <p>
		''' The value will be printed as per the normal format of an integer value.
		''' Only negative numbers will be signed. No padding will be added.
		''' <p>
		''' The parser for a variable width value such as this normally behaves greedily,
		''' requiring one digit, but accepting as many digits as possible.
		''' This behavior can be affected by 'adjacent value parsing'.
		''' See <seealso cref="#appendValue(java.time.temporal.TemporalField, int)"/> for full details.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendValue(java.time.temporal.TemporalField field)
			java.util.Objects.requireNonNull(field, "field")
			appendValue(New NumberPrinterParser(field, 1, 19, SignStyle.NORMAL))
			Return Me

		''' <summary>
		''' Appends the value of a date-time field to the formatter using a fixed
		''' width, zero-padded approach.
		''' <p>
		''' The value of the field will be output during a format.
		''' If the value cannot be obtained then an exception will be thrown.
		''' <p>
		''' The value will be zero-padded on the left. If the size of the value
		''' means that it cannot be printed within the width then an exception is thrown.
		''' If the value of the field is negative then an exception is thrown during formatting.
		''' <p>
		''' This method supports a special technique of parsing known as 'adjacent value parsing'.
		''' This technique solves the problem where a value, variable or fixed width, is followed by one or more
		''' fixed length values. The standard parser is greedy, and thus it would normally
		''' steal the digits that are needed by the fixed width value parsers that follow the
		''' variable width one.
		''' <p>
		''' No action is required to initiate 'adjacent value parsing'.
		''' When a call to {@code appendValue} is made, the builder
		''' enters adjacent value parsing setup mode. If the immediately subsequent method
		''' call or calls on the same builder are for a fixed width value, then the parser will reserve
		''' space so that the fixed width values can be parsed.
		''' <p>
		''' For example, consider {@code builder.appendValue(YEAR).appendValue(MONTH_OF_YEAR, 2);}
		''' The year is a variable width parse of between 1 and 19 digits.
		''' The month is a fixed width parse of 2 digits.
		''' Because these were appended to the same builder immediately after one another,
		''' the year parser will reserve two digits for the month to parse.
		''' Thus, the text '201106' will correctly parse to a year of 2011 and a month of 6.
		''' Without adjacent value parsing, the year would greedily parse all six digits and leave
		''' nothing for the month.
		''' <p>
		''' Adjacent value parsing applies to each set of fixed width not-negative values in the parser
		''' that immediately follow any kind of value, variable or fixed width.
		''' Calling any other append method will end the setup of adjacent value parsing.
		''' Thus, in the unlikely event that you need to avoid adjacent value parsing behavior,
		''' simply add the {@code appendValue} to another {@code DateTimeFormatterBuilder}
		''' and add that to this builder.
		''' <p>
		''' If adjacent parsing is active, then parsing must match exactly the specified
		''' number of digits in both strict and lenient modes.
		''' In addition, no positive or negative sign is permitted.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="width">  the width of the printed field, from 1 to 19 </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the width is invalid </exception>
		public DateTimeFormatterBuilder appendValue(java.time.temporal.TemporalField field, Integer width)
			java.util.Objects.requireNonNull(field, "field")
			If width < 1 OrElse width > 19 Then Throw New IllegalArgumentException("The width must be from 1 to 19 inclusive but was " & width)
			Dim pp As New NumberPrinterParser(field, width, width, SignStyle.NOT_NEGATIVE)
			appendValue(pp)
			Return Me

		''' <summary>
		''' Appends the value of a date-time field to the formatter providing full
		''' control over formatting.
		''' <p>
		''' The value of the field will be output during a format.
		''' If the value cannot be obtained then an exception will be thrown.
		''' <p>
		''' This method provides full control of the numeric formatting, including
		''' zero-padding and the positive/negative sign.
		''' <p>
		''' The parser for a variable width value such as this normally behaves greedily,
		''' accepting as many digits as possible.
		''' This behavior can be affected by 'adjacent value parsing'.
		''' See <seealso cref="#appendValue(java.time.temporal.TemporalField, int)"/> for full details.
		''' <p>
		''' In strict parsing mode, the minimum number of parsed digits is {@code minWidth}
		''' and the maximum is {@code maxWidth}.
		''' In lenient parsing mode, the minimum number of parsed digits is one
		''' and the maximum is 19 (except as limited by adjacent value parsing).
		''' <p>
		''' If this method is invoked with equal minimum and maximum widths and a sign style of
		''' {@code NOT_NEGATIVE} then it delegates to {@code appendValue(TemporalField,int)}.
		''' In this scenario, the formatting and parsing behavior described there occur.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="minWidth">  the minimum field width of the printed field, from 1 to 19 </param>
		''' <param name="maxWidth">  the maximum field width of the printed field, from 1 to 19 </param>
		''' <param name="signStyle">  the positive/negative output style, not null </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the widths are invalid </exception>
		public DateTimeFormatterBuilder appendValue(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, SignStyle signStyle)
			If minWidth = maxWidth AndAlso signStyle = SignStyle.NOT_NEGATIVE Then Return appendValue(field, maxWidth)
			java.util.Objects.requireNonNull(field, "field")
			java.util.Objects.requireNonNull(signStyle, "signStyle")
			If minWidth < 1 OrElse minWidth > 19 Then Throw New IllegalArgumentException("The minimum width must be from 1 to 19 inclusive but was " & minWidth)
			If maxWidth < 1 OrElse maxWidth > 19 Then Throw New IllegalArgumentException("The maximum width must be from 1 to 19 inclusive but was " & maxWidth)
			If maxWidth < minWidth Then Throw New IllegalArgumentException("The maximum width must exceed or equal the minimum width but " & maxWidth & " < " & minWidth)
			Dim pp As New NumberPrinterParser(field, minWidth, maxWidth, signStyle)
			appendValue(pp)
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the reduced value of a date-time field to the formatter.
		''' <p>
		''' Since fields such as year vary by chronology, it is recommended to use the
		''' <seealso cref="#appendValueReduced(TemporalField, int, int, ChronoLocalDate)"/> date}
		''' variant of this method in most cases. This variant is suitable for
		''' simple fields or working with only the ISO chronology.
		''' <p>
		''' For formatting, the {@code width} and {@code maxWidth} are used to
		''' determine the number of characters to format.
		''' If they are equal then the format is fixed width.
		''' If the value of the field is within the range of the {@code baseValue} using
		''' {@code width} characters then the reduced value is formatted otherwise the value is
		''' truncated to fit {@code maxWidth}.
		''' The rightmost characters are output to match the width, left padding with zero.
		''' <p>
		''' For strict parsing, the number of characters allowed by {@code width} to {@code maxWidth} are parsed.
		''' For lenient parsing, the number of characters must be at least 1 and less than 10.
		''' If the number of digits parsed is equal to {@code width} and the value is positive,
		''' the value of the field is computed to be the first number greater than
		''' or equal to the {@code baseValue} with the same least significant characters,
		''' otherwise the value parsed is the field value.
		''' This allows a reduced value to be entered for values in range of the baseValue
		''' and width and absolute values can be entered for values outside the range.
		''' <p>
		''' For example, a base value of {@code 1980} and a width of {@code 2} will have
		''' valid values from {@code 1980} to {@code 2079}.
		''' During parsing, the text {@code "12"} will result in the value {@code 2012} as that
		''' is the value within the range where the last two characters are "12".
		''' By contrast, parsing the text {@code "1915"} will result in the value {@code 1915}.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="width">  the field width of the printed and parsed field, from 1 to 10 </param>
		''' <param name="maxWidth">  the maximum field width of the printed field, from 1 to 10 </param>
		''' <param name="baseValue">  the base value of the range of valid values </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the width or base value is invalid </exception>
		public DateTimeFormatterBuilder appendValueReduced(java.time.temporal.TemporalField field, Integer width, Integer maxWidth, Integer baseValue)
			java.util.Objects.requireNonNull(field, "field")
			Dim pp As New ReducedPrinterParser(field, width, maxWidth, baseValue, Nothing)
			appendValue(pp)
			Return Me

		''' <summary>
		''' Appends the reduced value of a date-time field to the formatter.
		''' <p>
		''' This is typically used for formatting and parsing a two digit year.
		''' <p>
		''' The base date is used to calculate the full value during parsing.
		''' For example, if the base date is 1950-01-01 then parsed values for
		''' a two digit year parse will be in the range 1950-01-01 to 2049-12-31.
		''' Only the year would be extracted from the date, thus a base date of
		''' 1950-08-25 would also parse to the range 1950-01-01 to 2049-12-31.
		''' This behavior is necessary to support fields such as week-based-year
		''' or other calendar systems where the parsed value does not align with
		''' standard ISO years.
		''' <p>
		''' The exact behavior is as follows. Parse the full set of fields and
		''' determine the effective chronology using the last chronology if
		''' it appears more than once. Then convert the base date to the
		''' effective chronology. Then extract the specified field from the
		''' chronology-specific base date and use it to determine the
		''' {@code baseValue} used below.
		''' <p>
		''' For formatting, the {@code width} and {@code maxWidth} are used to
		''' determine the number of characters to format.
		''' If they are equal then the format is fixed width.
		''' If the value of the field is within the range of the {@code baseValue} using
		''' {@code width} characters then the reduced value is formatted otherwise the value is
		''' truncated to fit {@code maxWidth}.
		''' The rightmost characters are output to match the width, left padding with zero.
		''' <p>
		''' For strict parsing, the number of characters allowed by {@code width} to {@code maxWidth} are parsed.
		''' For lenient parsing, the number of characters must be at least 1 and less than 10.
		''' If the number of digits parsed is equal to {@code width} and the value is positive,
		''' the value of the field is computed to be the first number greater than
		''' or equal to the {@code baseValue} with the same least significant characters,
		''' otherwise the value parsed is the field value.
		''' This allows a reduced value to be entered for values in range of the baseValue
		''' and width and absolute values can be entered for values outside the range.
		''' <p>
		''' For example, a base value of {@code 1980} and a width of {@code 2} will have
		''' valid values from {@code 1980} to {@code 2079}.
		''' During parsing, the text {@code "12"} will result in the value {@code 2012} as that
		''' is the value within the range where the last two characters are "12".
		''' By contrast, parsing the text {@code "1915"} will result in the value {@code 1915}.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="width">  the field width of the printed and parsed field, from 1 to 10 </param>
		''' <param name="maxWidth">  the maximum field width of the printed field, from 1 to 10 </param>
		''' <param name="baseDate">  the base date used to calculate the base value for the range
		'''  of valid values in the parsed chronology, not null </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the width or base value is invalid </exception>
		public DateTimeFormatterBuilder appendValueReduced(java.time.temporal.TemporalField field, Integer width, Integer maxWidth, java.time.chrono.ChronoLocalDate baseDate)
			java.util.Objects.requireNonNull(field, "field")
			java.util.Objects.requireNonNull(baseDate, "baseDate")
			Dim pp As New ReducedPrinterParser(field, width, maxWidth, 0, baseDate)
			appendValue(pp)
			Return Me

		''' <summary>
		''' Appends a fixed or variable width printer-parser handling adjacent value mode.
		''' If a PrinterParser is not active then the new PrinterParser becomes
		''' the active PrinterParser.
		''' Otherwise, the active PrinterParser is modified depending on the new PrinterParser.
		''' If the new PrinterParser is fixed width and has sign style {@code NOT_NEGATIVE}
		''' then its width is added to the active PP and
		''' the new PrinterParser is forced to be fixed width.
		''' If the new PrinterParser is variable width, the active PrinterParser is changed
		''' to be fixed width and the new PrinterParser becomes the active PP.
		''' </summary>
		''' <param name="pp">  the printer-parser, not null </param>
		''' <returns> this, for chaining, not null </returns>
		private DateTimeFormatterBuilder appendValue(NumberPrinterParser pp)
			If active.valueParserIndex >= 0 Then
				Dim activeValueParser As Integer = active.valueParserIndex

				' adjacent parsing mode, update setting in previous parsers
				Dim basePP As NumberPrinterParser = CType(active.printerParsers(activeValueParser), NumberPrinterParser)
				If pp.minWidth = pp.maxWidth AndAlso pp.signStyle = SignStyle.NOT_NEGATIVE Then
					' Append the width to the subsequentWidth of the active parser
					basePP = basePP.withSubsequentWidth(pp.maxWidth)
					' Append the new parser as a fixed width
					appendInternal(pp.withFixedWidth())
					' Retain the previous active parser
					active.valueParserIndex = activeValueParser
				Else
					' Modify the active parser to be fixed width
					basePP = basePP.withFixedWidth()
					' The new parser becomes the mew active parser
					active.valueParserIndex = appendInternal(pp)
				End If
				' Replace the modified parser with the updated one
				active.printerParsers(activeValueParser) = basePP
			Else
				' The new Parser becomes the active parser
				active.valueParserIndex = appendInternal(pp)
			End If
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the fractional value of a date-time field to the formatter.
		''' <p>
		''' The fractional value of the field will be output including the
		''' preceding decimal point. The preceding value is not output.
		''' For example, the second-of-minute value of 15 would be output as {@code .25}.
		''' <p>
		''' The width of the printed fraction can be controlled. Setting the
		''' minimum width to zero will cause no output to be generated.
		''' The printed fraction will have the minimum width necessary between
		''' the minimum and maximum widths - trailing zeroes are omitted.
		''' No rounding occurs due to the maximum width - digits are simply dropped.
		''' <p>
		''' When parsing in strict mode, the number of parsed digits must be between
		''' the minimum and maximum width. When parsing in lenient mode, the minimum
		''' width is considered to be zero and the maximum is nine.
		''' <p>
		''' If the value cannot be obtained then an exception will be thrown.
		''' If the value is negative an exception will be thrown.
		''' If the field does not have a fixed set of valid values then an
		''' exception will be thrown.
		''' If the field value in the date-time to be printed is invalid it
		''' cannot be printed and an exception will be thrown.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="minWidth">  the minimum width of the field excluding the decimal point, from 0 to 9 </param>
		''' <param name="maxWidth">  the maximum width of the field excluding the decimal point, from 1 to 9 </param>
		''' <param name="decimalPoint">  whether to output the localized decimal point symbol </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the field has a variable set of valid values or
		'''  either width is invalid </exception>
		public DateTimeFormatterBuilder appendFraction(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, Boolean decimalPoint)
			appendInternal(New FractionPrinterParser(field, minWidth, maxWidth, decimalPoint))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the text of a date-time field to the formatter using the full
		''' text style.
		''' <p>
		''' The text of the field will be output during a format.
		''' The value must be within the valid range of the field.
		''' If the value cannot be obtained then an exception will be thrown.
		''' If the field has no textual representation, then the numeric value will be used.
		''' <p>
		''' The value will be printed as per the normal format of an integer value.
		''' Only negative numbers will be signed. No padding will be added.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendText(java.time.temporal.TemporalField field)
			Return appendText(field, TextStyle.FULL)

		''' <summary>
		''' Appends the text of a date-time field to the formatter.
		''' <p>
		''' The text of the field will be output during a format.
		''' The value must be within the valid range of the field.
		''' If the value cannot be obtained then an exception will be thrown.
		''' If the field has no textual representation, then the numeric value will be used.
		''' <p>
		''' The value will be printed as per the normal format of an integer value.
		''' Only negative numbers will be signed. No padding will be added.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="textStyle">  the text style to use, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendText(java.time.temporal.TemporalField field, TextStyle textStyle)
			java.util.Objects.requireNonNull(field, "field")
			java.util.Objects.requireNonNull(textStyle, "textStyle")
			appendInternal(New TextPrinterParser(field, textStyle, DateTimeTextProvider.instance))
			Return Me

		''' <summary>
		''' Appends the text of a date-time field to the formatter using the specified
		''' map to supply the text.
		''' <p>
		''' The standard text outputting methods use the localized text in the JDK.
		''' This method allows that text to be specified directly.
		''' The supplied map is not validated by the builder to ensure that formatting or
		''' parsing is possible, thus an invalid map may throw an error during later use.
		''' <p>
		''' Supplying the map of text provides considerable flexibility in formatting and parsing.
		''' For example, a legacy application might require or supply the months of the
		''' year as "JNY", "FBY", "MCH" etc. These do not match the standard set of text
		''' for localized month names. Using this method, a map can be created which
		''' defines the connection between each value and the text:
		''' <pre>
		''' Map&lt;Long, String&gt; map = new HashMap&lt;&gt;();
		''' map.put(1L, "JNY");
		''' map.put(2L, "FBY");
		''' map.put(3L, "MCH");
		''' ...
		''' builder.appendText(MONTH_OF_YEAR, map);
		''' </pre>
		''' <p>
		''' Other uses might be to output the value with a suffix, such as "1st", "2nd", "3rd",
		''' or as Roman numerals "I", "II", "III", "IV".
		''' <p>
		''' During formatting, the value is obtained and checked that it is in the valid range.
		''' If text is not available for the value then it is output as a number.
		''' During parsing, the parser will match against the map of text and numeric values.
		''' </summary>
		''' <param name="field">  the field to append, not null </param>
		''' <param name="textLookup">  the map from the value to the text </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendText(java.time.temporal.TemporalField field, IDictionary(Of Long?, String) textLookup)
			java.util.Objects.requireNonNull(field, "field")
			java.util.Objects.requireNonNull(textLookup, "textLookup")
			Dim copy As IDictionary(Of Long?, String) = New java.util.LinkedHashMap(Of Long?, String)(textLookup)
			Dim map As IDictionary(Of TextStyle, IDictionary(Of Long?, String)) = java.util.Collections.singletonMap(TextStyle.FULL, copy)
			Dim store As New java.time.format.DateTimeTextProvider.LocaleStore(map)
			Dim provider As DateTimeTextProvider = New DateTimeTextProviderAnonymousInnerClassHelper
			appendInternal(New TextPrinterParser(field, TextStyle.FULL, provider))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends an instant using ISO-8601 to the formatter, formatting fractional
		''' digits in groups of three.
		''' <p>
		''' Instants have a fixed output format.
		''' They are converted to a date-time with a zone-offset of UTC and formatted
		''' using the standard ISO-8601 format.
		''' With this method, formatting nano-of-second outputs zero, three, six
		''' or nine digits digits as necessary.
		''' The localized decimal style is not used.
		''' <p>
		''' The instant is obtained using <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		''' and optionally (@code NANO_OF_SECOND). The value of {@code INSTANT_SECONDS}
		''' may be outside the maximum range of {@code LocalDateTime}.
		''' <p>
		''' The <seealso cref="ResolverStyle resolver style"/> has no effect on instant parsing.
		''' The end-of-day time of '24:00' is handled as midnight at the start of the following day.
		''' The leap-second time of '23:59:59' is handled to some degree, see
		''' <seealso cref="DateTimeFormatter#parsedLeapSecond()"/> for full details.
		''' <p>
		''' An alternative to this method is to format/parse the instant as a single
		''' epoch-seconds value. That is achieved using {@code appendValue(INSTANT_SECONDS)}.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendInstant()
			appendInternal(New InstantPrinterParser(-2))
			Return Me

		''' <summary>
		''' Appends an instant using ISO-8601 to the formatter with control over
		''' the number of fractional digits.
		''' <p>
		''' Instants have a fixed output format, although this method provides some
		''' control over the fractional digits. They are converted to a date-time
		''' with a zone-offset of UTC and printed using the standard ISO-8601 format.
		''' The localized decimal style is not used.
		''' <p>
		''' The {@code fractionalDigits} parameter allows the output of the fractional
		''' second to be controlled. Specifying zero will cause no fractional digits
		''' to be output. From 1 to 9 will output an increasing number of digits, using
		''' zero right-padding if necessary. The special value -1 is used to output as
		''' many digits as necessary to avoid any trailing zeroes.
		''' <p>
		''' When parsing in strict mode, the number of parsed digits must match the
		''' fractional digits. When parsing in lenient mode, any number of fractional
		''' digits from zero to nine are accepted.
		''' <p>
		''' The instant is obtained using <seealso cref="ChronoField#INSTANT_SECONDS INSTANT_SECONDS"/>
		''' and optionally (@code NANO_OF_SECOND). The value of {@code INSTANT_SECONDS}
		''' may be outside the maximum range of {@code LocalDateTime}.
		''' <p>
		''' The <seealso cref="ResolverStyle resolver style"/> has no effect on instant parsing.
		''' The end-of-day time of '24:00' is handled as midnight at the start of the following day.
		''' The leap-second time of '23:59:60' is handled to some degree, see
		''' <seealso cref="DateTimeFormatter#parsedLeapSecond()"/> for full details.
		''' <p>
		''' An alternative to this method is to format/parse the instant as a single
		''' epoch-seconds value. That is achieved using {@code appendValue(INSTANT_SECONDS)}.
		''' </summary>
		''' <param name="fractionalDigits">  the number of fractional second digits to format with,
		'''  from 0 to 9, or -1 to use as many digits as necessary </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendInstant(Integer fractionalDigits)
			If fractionalDigits < -1 OrElse fractionalDigits > 9 Then Throw New IllegalArgumentException("The fractional digits must be from -1 to 9 inclusive but was " & fractionalDigits)
			appendInternal(New InstantPrinterParser(fractionalDigits))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the zone offset, such as '+01:00', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the offset ID to the builder.
		''' This is equivalent to calling {@code appendOffset("+HH:MM:ss", "Z")}.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendOffsetId()
			appendInternal(OffsetIdPrinterParser.INSTANCE_ID_Z)
			Return Me

		''' <summary>
		''' Appends the zone offset, such as '+01:00', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the offset ID to the builder.
		''' <p>
		''' During formatting, the offset is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#offset()"/>.
		''' It will be printed using the format defined below.
		''' If the offset cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, the offset is parsed using the format defined below.
		''' If the offset cannot be parsed then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' The format of the offset is controlled by a pattern which must be one
		''' of the following:
		''' <ul>
		''' <li>{@code +HH} - hour only, ignoring minute and second
		''' <li>{@code +HHmm} - hour, with minute if non-zero, ignoring second, no colon
		''' <li>{@code +HH:mm} - hour, with minute if non-zero, ignoring second, with colon
		''' <li>{@code +HHMM} - hour and minute, ignoring second, no colon
		''' <li>{@code +HH:MM} - hour and minute, ignoring second, with colon
		''' <li>{@code +HHMMss} - hour and minute, with second if non-zero, no colon
		''' <li>{@code +HH:MM:ss} - hour and minute, with second if non-zero, with colon
		''' <li>{@code +HHMMSS} - hour, minute and second, no colon
		''' <li>{@code +HH:MM:SS} - hour, minute and second, with colon
		''' </ul>
		''' The "no offset" text controls what text is printed when the total amount of
		''' the offset fields to be output is zero.
		''' Example values would be 'Z', '+00:00', 'UTC' or 'GMT'.
		''' Three formats are accepted for parsing UTC - the "no offset" text, and the
		''' plus and minus versions of zero defined by the pattern.
		''' </summary>
		''' <param name="pattern">  the pattern to use, not null </param>
		''' <param name="noOffsetText">  the text to use when the offset is zero, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendOffset(String pattern, String noOffsetText)
			appendInternal(New OffsetIdPrinterParser(pattern, noOffsetText))
			Return Me

		''' <summary>
		''' Appends the localized zone offset, such as 'GMT+01:00', to the formatter.
		''' <p>
		''' This appends a localized zone offset to the builder, the format of the
		''' localized offset is controlled by the specified <seealso cref="FormatStyle style"/>
		''' to this method:
		''' <ul>
		''' <li><seealso cref="TextStyle#FULL full"/> - formats with localized offset text, such
		''' as 'GMT, 2-digit hour and minute field, optional second field if non-zero,
		''' and colon.
		''' <li><seealso cref="TextStyle#SHORT short"/> - formats with localized offset text,
		''' such as 'GMT, hour without leading zero, optional 2-digit minute and
		''' second if non-zero, and colon.
		''' </ul>
		''' <p>
		''' During formatting, the offset is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#offset()"/>.
		''' If the offset cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, the offset is parsed using the format defined above.
		''' If the offset cannot be parsed then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p> </summary>
		''' <param name="style">  the format style to use, not null </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if style is neither {@link TextStyle#FULL
		''' full} nor <seealso cref="TextStyle#SHORT short"/> </exception>
		public DateTimeFormatterBuilder appendLocalizedOffset(TextStyle style)
			java.util.Objects.requireNonNull(style, "style")
			If style <> TextStyle.FULL AndAlso style <> TextStyle.SHORT Then Throw New IllegalArgumentException("Style must be either full or short")
			appendInternal(New LocalizedOffsetIdPrinterParser(style))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the time-zone ID, such as 'Europe/Paris' or '+02:00', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the zone ID to the builder.
		''' The zone ID is obtained in a strict manner suitable for {@code ZonedDateTime}.
		''' By contrast, {@code OffsetDateTime} does not have a zone ID suitable
		''' for use with this method, see <seealso cref="#appendZoneOrOffsetId()"/>.
		''' <p>
		''' During formatting, the zone is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		''' It will be printed using the result of <seealso cref="ZoneId#getId()"/>.
		''' If the zone cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, the text must match a known zone or offset.
		''' There are two types of zone ID, offset-based, such as '+01:30' and
		''' region-based, such as 'Europe/London'. These are parsed differently.
		''' If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		''' expects an offset-based zone and will not match region-based zones.
		''' The offset ID, such as '+02:30', may be at the start of the parse,
		''' or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		''' equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		''' arguments 'HH:MM:ss' and the no offset string '0'.
		''' If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		''' match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		''' In all other cases, the list of known region-based zones is used to
		''' find the longest available match. If no match is found, and the parse
		''' starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		''' The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		''' <p>
		''' For example, the following will parse:
		''' <pre>
		'''   "Europe/London"           -- ZoneId.of("Europe/London")
		'''   "Z"                       -- ZoneOffset.UTC
		'''   "UT"                      -- ZoneId.of("UT")
		'''   "UTC"                     -- ZoneId.of("UTC")
		'''   "GMT"                     -- ZoneId.of("GMT")
		'''   "+01:30"                  -- ZoneOffset.of("+01:30")
		'''   "UT+01:30"                -- ZoneOffset.of("+01:30")
		'''   "UTC+01:30"               -- ZoneOffset.of("+01:30")
		'''   "GMT+01:30"               -- ZoneOffset.of("+01:30")
		''' </pre>
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		''' <seealso cref= #appendZoneRegionId() </seealso>
		public DateTimeFormatterBuilder appendZoneId()
			appendInternal(New ZoneIdPrinterParser(java.time.temporal.TemporalQueries.zoneId(), "ZoneId()"))
			Return Me

		''' <summary>
		''' Appends the time-zone region ID, such as 'Europe/Paris', to the formatter,
		''' rejecting the zone ID if it is a {@code ZoneOffset}.
		''' <p>
		''' This appends an instruction to format/parse the zone ID to the builder
		''' only if it is a region-based ID.
		''' <p>
		''' During formatting, the zone is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		''' If the zone is a {@code ZoneOffset} or it cannot be obtained then
		''' an exception is thrown unless the section of the formatter is optional.
		''' If the zone is not an offset, then the zone will be printed using
		''' the zone ID from <seealso cref="ZoneId#getId()"/>.
		''' <p>
		''' During parsing, the text must match a known zone or offset.
		''' There are two types of zone ID, offset-based, such as '+01:30' and
		''' region-based, such as 'Europe/London'. These are parsed differently.
		''' If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		''' expects an offset-based zone and will not match region-based zones.
		''' The offset ID, such as '+02:30', may be at the start of the parse,
		''' or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		''' equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		''' arguments 'HH:MM:ss' and the no offset string '0'.
		''' If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		''' match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		''' In all other cases, the list of known region-based zones is used to
		''' find the longest available match. If no match is found, and the parse
		''' starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		''' The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		''' <p>
		''' For example, the following will parse:
		''' <pre>
		'''   "Europe/London"           -- ZoneId.of("Europe/London")
		'''   "Z"                       -- ZoneOffset.UTC
		'''   "UT"                      -- ZoneId.of("UT")
		'''   "UTC"                     -- ZoneId.of("UTC")
		'''   "GMT"                     -- ZoneId.of("GMT")
		'''   "+01:30"                  -- ZoneOffset.of("+01:30")
		'''   "UT+01:30"                -- ZoneOffset.of("+01:30")
		'''   "UTC+01:30"               -- ZoneOffset.of("+01:30")
		'''   "GMT+01:30"               -- ZoneOffset.of("+01:30")
		''' </pre>
		''' <p>
		''' Note that this method is identical to {@code appendZoneId()} except
		''' in the mechanism used to obtain the zone.
		''' Note also that parsing accepts offsets, whereas formatting will never
		''' produce one.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		''' <seealso cref= #appendZoneId() </seealso>
		public DateTimeFormatterBuilder appendZoneRegionId()
			appendInternal(New ZoneIdPrinterParser(QUERY_REGION_ONLY, "ZoneRegionId()"))
			Return Me

		''' <summary>
		''' Appends the time-zone ID, such as 'Europe/Paris' or '+02:00', to
		''' the formatter, using the best available zone ID.
		''' <p>
		''' This appends an instruction to format/parse the best available
		''' zone or offset ID to the builder.
		''' The zone ID is obtained in a lenient manner that first attempts to
		''' find a true zone ID, such as that on {@code ZonedDateTime}, and
		''' then attempts to find an offset, such as that on {@code OffsetDateTime}.
		''' <p>
		''' During formatting, the zone is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#zone()"/>.
		''' It will be printed using the result of <seealso cref="ZoneId#getId()"/>.
		''' If the zone cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, the text must match a known zone or offset.
		''' There are two types of zone ID, offset-based, such as '+01:30' and
		''' region-based, such as 'Europe/London'. These are parsed differently.
		''' If the parse starts with '+', '-', 'UT', 'UTC' or 'GMT', then the parser
		''' expects an offset-based zone and will not match region-based zones.
		''' The offset ID, such as '+02:30', may be at the start of the parse,
		''' or prefixed by  'UT', 'UTC' or 'GMT'. The offset ID parsing is
		''' equivalent to using <seealso cref="#appendOffset(String, String)"/> using the
		''' arguments 'HH:MM:ss' and the no offset string '0'.
		''' If the parse starts with 'UT', 'UTC' or 'GMT', and the parser cannot
		''' match a following offset ID, then <seealso cref="ZoneOffset#UTC"/> is selected.
		''' In all other cases, the list of known region-based zones is used to
		''' find the longest available match. If no match is found, and the parse
		''' starts with 'Z', then {@code ZoneOffset.UTC} is selected.
		''' The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		''' <p>
		''' For example, the following will parse:
		''' <pre>
		'''   "Europe/London"           -- ZoneId.of("Europe/London")
		'''   "Z"                       -- ZoneOffset.UTC
		'''   "UT"                      -- ZoneId.of("UT")
		'''   "UTC"                     -- ZoneId.of("UTC")
		'''   "GMT"                     -- ZoneId.of("GMT")
		'''   "+01:30"                  -- ZoneOffset.of("+01:30")
		'''   "UT+01:30"                -- ZoneOffset.of("UT+01:30")
		'''   "UTC+01:30"               -- ZoneOffset.of("UTC+01:30")
		'''   "GMT+01:30"               -- ZoneOffset.of("GMT+01:30")
		''' </pre>
		''' <p>
		''' Note that this method is identical to {@code appendZoneId()} except
		''' in the mechanism used to obtain the zone.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		''' <seealso cref= #appendZoneId() </seealso>
		public DateTimeFormatterBuilder appendZoneOrOffsetId()
			appendInternal(New ZoneIdPrinterParser(java.time.temporal.TemporalQueries.zone(), "ZoneOrOffsetId()"))
			Return Me

		''' <summary>
		''' Appends the time-zone name, such as 'British Summer Time', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the textual name of the zone to
		''' the builder.
		''' <p>
		''' During formatting, the zone is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		''' If the zone is a {@code ZoneOffset} it will be printed using the
		''' result of <seealso cref="ZoneOffset#getId()"/>.
		''' If the zone is not an offset, the textual name will be looked up
		''' for the locale set in the <seealso cref="DateTimeFormatter"/>.
		''' If the temporal object being printed represents an instant, then the text
		''' will be the summer or winter time text as appropriate.
		''' If the lookup for text does not find any suitable result, then the
		''' <seealso cref="ZoneId#getId() ID"/> will be printed instead.
		''' If the zone cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, either the textual zone name, the zone ID or the offset
		''' is accepted. Many textual zone names are not unique, such as CST can be
		''' for both "Central Standard Time" and "China Standard Time". In this
		''' situation, the zone id will be determined by the region information from
		''' formatter's  <seealso cref="DateTimeFormatter#getLocale() locale"/> and the standard
		''' zone id for that area, for example, America/New_York for the America Eastern
		''' zone. The <seealso cref="#appendZoneText(TextStyle, Set)"/> may be used
		''' to specify a set of preferred <seealso cref="ZoneId"/> in this situation.
		''' </summary>
		''' <param name="textStyle">  the text style to use, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendZoneText(TextStyle textStyle)
			appendInternal(New ZoneTextPrinterParser(textStyle, Nothing))
			Return Me

		''' <summary>
		''' Appends the time-zone name, such as 'British Summer Time', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the textual name of the zone to
		''' the builder.
		''' <p>
		''' During formatting, the zone is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#zoneId()"/>.
		''' If the zone is a {@code ZoneOffset} it will be printed using the
		''' result of <seealso cref="ZoneOffset#getId()"/>.
		''' If the zone is not an offset, the textual name will be looked up
		''' for the locale set in the <seealso cref="DateTimeFormatter"/>.
		''' If the temporal object being printed represents an instant, then the text
		''' will be the summer or winter time text as appropriate.
		''' If the lookup for text does not find any suitable result, then the
		''' <seealso cref="ZoneId#getId() ID"/> will be printed instead.
		''' If the zone cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, either the textual zone name, the zone ID or the offset
		''' is accepted. Many textual zone names are not unique, such as CST can be
		''' for both "Central Standard Time" and "China Standard Time". In this
		''' situation, the zone id will be determined by the region information from
		''' formatter's  <seealso cref="DateTimeFormatter#getLocale() locale"/> and the standard
		''' zone id for that area, for example, America/New_York for the America Eastern
		''' zone. This method also allows a set of preferred <seealso cref="ZoneId"/> to be
		''' specified for parsing. The matched preferred zone id will be used if the
		''' textural zone name being parsed is not unique.
		''' <p>
		''' If the zone cannot be parsed then an exception is thrown unless the
		''' section of the formatter is optional.
		''' </summary>
		''' <param name="textStyle">  the text style to use, not null </param>
		''' <param name="preferredZones">  the set of preferred zone ids, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendZoneText(TextStyle textStyle, java.util.Set(Of java.time.ZoneId) preferredZones)
			java.util.Objects.requireNonNull(preferredZones, "preferredZones")
			appendInternal(New ZoneTextPrinterParser(textStyle, preferredZones))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the chronology ID, such as 'ISO' or 'ThaiBuddhist', to the formatter.
		''' <p>
		''' This appends an instruction to format/parse the chronology ID to the builder.
		''' <p>
		''' During formatting, the chronology is obtained using a mechanism equivalent
		''' to querying the temporal with <seealso cref="TemporalQueries#chronology()"/>.
		''' It will be printed using the result of <seealso cref="Chronology#getId()"/>.
		''' If the chronology cannot be obtained then an exception is thrown unless the
		''' section of the formatter is optional.
		''' <p>
		''' During parsing, the chronology is parsed and must match one of the chronologies
		''' in <seealso cref="Chronology#getAvailableChronologies()"/>.
		''' If the chronology cannot be parsed then an exception is thrown unless the
		''' section of the formatter is optional.
		''' The parser uses the <seealso cref="#parseCaseInsensitive() case sensitive"/> setting.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendChronologyId()
			appendInternal(New ChronoPrinterParser(Nothing))
			Return Me

		''' <summary>
		''' Appends the chronology name to the formatter.
		''' <p>
		''' The calendar system name will be output during a format.
		''' If the chronology cannot be obtained then an exception will be thrown.
		''' </summary>
		''' <param name="textStyle">  the text style to use, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendChronologyText(TextStyle textStyle)
			java.util.Objects.requireNonNull(textStyle, "textStyle")
			appendInternal(New ChronoPrinterParser(textStyle))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends a localized date-time pattern to the formatter.
		''' <p>
		''' This appends a localized section to the builder, suitable for outputting
		''' a date, time or date-time combination. The format of the localized
		''' section is lazily looked up based on four items:
		''' <ul>
		''' <li>the {@code dateStyle} specified to this method
		''' <li>the {@code timeStyle} specified to this method
		''' <li>the {@code Locale} of the {@code DateTimeFormatter}
		''' <li>the {@code Chronology}, selecting the best available
		''' </ul>
		''' During formatting, the chronology is obtained from the temporal object
		''' being formatted, which may have been overridden by
		''' <seealso cref="DateTimeFormatter#withChronology(Chronology)"/>.
		''' <p>
		''' During parsing, if a chronology has already been parsed, then it is used.
		''' Otherwise the default from {@code DateTimeFormatter.withChronology(Chronology)}
		''' is used, with {@code IsoChronology} as the fallback.
		''' <p>
		''' Note that this method provides similar functionality to methods on
		''' {@code DateFormat} such as <seealso cref="java.text.DateFormat#getDateTimeInstance(int, int)"/>.
		''' </summary>
		''' <param name="dateStyle">  the date style to use, null means no date required </param>
		''' <param name="timeStyle">  the time style to use, null means no time required </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if both the date and time styles are null </exception>
		public DateTimeFormatterBuilder appendLocalized(FormatStyle dateStyle, FormatStyle timeStyle)
			If dateStyle Is Nothing AndAlso timeStyle Is Nothing Then Throw New IllegalArgumentException("Either the date or time style must be non-null")
			appendInternal(New LocalizedPrinterParser(dateStyle, timeStyle))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends a character literal to the formatter.
		''' <p>
		''' This character will be output during a format.
		''' </summary>
		''' <param name="literal">  the literal to append, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendLiteral(Char literal)
			appendInternal(New CharLiteralPrinterParser(literal))
			Return Me

		''' <summary>
		''' Appends a string literal to the formatter.
		''' <p>
		''' This string will be output during a format.
		''' <p>
		''' If the literal is empty, nothing is added to the formatter.
		''' </summary>
		''' <param name="literal">  the literal to append, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendLiteral(String literal)
			java.util.Objects.requireNonNull(literal, "literal")
			If literal.length() > 0 Then
				If literal.length() = 1 Then
					appendInternal(New CharLiteralPrinterParser(literal.Chars(0)))
				Else
					appendInternal(New StringLiteralPrinterParser(literal))
				End If
			End If
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends all the elements of a formatter to the builder.
		''' <p>
		''' This method has the same effect as appending each of the constituent
		''' parts of the formatter directly to this builder.
		''' </summary>
		''' <param name="formatter">  the formatter to add, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder append(DateTimeFormatter formatter)
			java.util.Objects.requireNonNull(formatter, "formatter")
			appendInternal(formatter.toPrinterParser(False))
			Return Me

		''' <summary>
		''' Appends a formatter to the builder which will optionally format/parse.
		''' <p>
		''' This method has the same effect as appending each of the constituent
		''' parts directly to this builder surrounded by an <seealso cref="#optionalStart()"/> and
		''' <seealso cref="#optionalEnd()"/>.
		''' <p>
		''' The formatter will format if data is available for all the fields contained within it.
		''' The formatter will parse if the string matches, otherwise no error is returned.
		''' </summary>
		''' <param name="formatter">  the formatter to add, not null </param>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder appendOptional(DateTimeFormatter formatter)
			java.util.Objects.requireNonNull(formatter, "formatter")
			appendInternal(formatter.toPrinterParser(True))
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends the elements defined by the specified pattern to the builder.
		''' <p>
		''' All letters 'A' to 'Z' and 'a' to 'z' are reserved as pattern letters.
		''' The characters '#', '{' and '}' are reserved for future use.
		''' The characters '[' and ']' indicate optional patterns.
		''' The following pattern letters are defined:
		''' <pre>
		'''  Symbol  Meaning                     Presentation      Examples
		'''  ------  -------                     ------------      -------
		'''   G       era                         text              AD; Anno Domini; A
		'''   u       year                        year              2004; 04
		'''   y       year-of-era                 year              2004; 04
		'''   D       day-of-year                 number            189
		'''   M/L     month-of-year               number/text       7; 07; Jul; July; J
		'''   d       day-of-month                number            10
		''' 
		'''   Q/q     quarter-of-year             number/text       3; 03; Q3; 3rd quarter
		'''   Y       week-based-year             year              1996; 96
		'''   w       week-of-week-based-year     number            27
		'''   W       week-of-month               number            4
		'''   E       day-of-week                 text              Tue; Tuesday; T
		'''   e/c     localized day-of-week       number/text       2; 02; Tue; Tuesday; T
		'''   F       week-of-month               number            3
		''' 
		'''   a       am-pm-of-day                text              PM
		'''   h       clock-hour-of-am-pm (1-12)  number            12
		'''   K       hour-of-am-pm (0-11)        number            0
		'''   k       clock-hour-of-am-pm (1-24)  number            0
		''' 
		'''   H       hour-of-day (0-23)          number            0
		'''   m       minute-of-hour              number            30
		'''   s       second-of-minute            number            55
		'''   S       fraction-of-second          fraction          978
		'''   A       milli-of-day                number            1234
		'''   n       nano-of-second              number            987654321
		'''   N       nano-of-day                 number            1234000000
		''' 
		'''   V       time-zone ID                zone-id           America/Los_Angeles; Z; -08:30
		'''   z       time-zone name              zone-name         Pacific Standard Time; PST
		'''   O       localized zone-offset       offset-O          GMT+8; GMT+08:00; UTC-08:00;
		'''   X       zone-offset 'Z' for zero    offset-X          Z; -08; -0830; -08:30; -083015; -08:30:15;
		'''   x       zone-offset                 offset-x          +0000; -08; -0830; -08:30; -083015; -08:30:15;
		'''   Z       zone-offset                 offset-Z          +0000; -0800; -08:00;
		''' 
		'''   p       pad next                    pad modifier      1
		''' 
		'''   '       escape for text             delimiter
		'''   ''      single quote                literal           '
		'''   [       optional section start
		'''   ]       optional section end
		'''   #       reserved for future use
		'''   {       reserved for future use
		'''   }       reserved for future use
		''' </pre>
		''' <p>
		''' The count of pattern letters determine the format.
		''' See <a href="DateTimeFormatter.html#patterns">DateTimeFormatter</a> for a user-focused description of the patterns.
		''' The following tables define how the pattern letters map to the builder.
		''' <p>
		''' <b>Date fields</b>: Pattern letters to output a date.
		''' <pre>
		'''  Pattern  Count  Equivalent builder methods
		'''  -------  -----  --------------------------
		'''    G       1      appendText(ChronoField.ERA, TextStyle.SHORT)
		'''    GG      2      appendText(ChronoField.ERA, TextStyle.SHORT)
		'''    GGG     3      appendText(ChronoField.ERA, TextStyle.SHORT)
		'''    GGGG    4      appendText(ChronoField.ERA, TextStyle.FULL)
		'''    GGGGG   5      appendText(ChronoField.ERA, TextStyle.NARROW)
		''' 
		'''    u       1      appendValue(ChronoField.YEAR, 1, 19, SignStyle.NORMAL);
		'''    uu      2      appendValueReduced(ChronoField.YEAR, 2, 2000);
		'''    uuu     3      appendValue(ChronoField.YEAR, 3, 19, SignStyle.NORMAL);
		'''    u..u    4..n   appendValue(ChronoField.YEAR, n, 19, SignStyle.EXCEEDS_PAD);
		'''    y       1      appendValue(ChronoField.YEAR_OF_ERA, 1, 19, SignStyle.NORMAL);
		'''    yy      2      appendValueReduced(ChronoField.YEAR_OF_ERA, 2, 2000);
		'''    yyy     3      appendValue(ChronoField.YEAR_OF_ERA, 3, 19, SignStyle.NORMAL);
		'''    y..y    4..n   appendValue(ChronoField.YEAR_OF_ERA, n, 19, SignStyle.EXCEEDS_PAD);
		'''    Y       1      append special localized WeekFields element for numeric week-based-year
		'''    YY      2      append special localized WeekFields element for reduced numeric week-based-year 2 digits;
		'''    YYY     3      append special localized WeekFields element for numeric week-based-year (3, 19, SignStyle.NORMAL);
		'''    Y..Y    4..n   append special localized WeekFields element for numeric week-based-year (n, 19, SignStyle.EXCEEDS_PAD);
		''' 
		'''    Q       1      appendValue(IsoFields.QUARTER_OF_YEAR);
		'''    QQ      2      appendValue(IsoFields.QUARTER_OF_YEAR, 2);
		'''    QQQ     3      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.SHORT)
		'''    QQQQ    4      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.FULL)
		'''    QQQQQ   5      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.NARROW)
		'''    q       1      appendValue(IsoFields.QUARTER_OF_YEAR);
		'''    qq      2      appendValue(IsoFields.QUARTER_OF_YEAR, 2);
		'''    qqq     3      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.SHORT_STANDALONE)
		'''    qqqq    4      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.FULL_STANDALONE)
		'''    qqqqq   5      appendText(IsoFields.QUARTER_OF_YEAR, TextStyle.NARROW_STANDALONE)
		''' 
		'''    M       1      appendValue(ChronoField.MONTH_OF_YEAR);
		'''    MM      2      appendValue(ChronoField.MONTH_OF_YEAR, 2);
		'''    MMM     3      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.SHORT)
		'''    MMMM    4      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.FULL)
		'''    MMMMM   5      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.NARROW)
		'''    L       1      appendValue(ChronoField.MONTH_OF_YEAR);
		'''    LL      2      appendValue(ChronoField.MONTH_OF_YEAR, 2);
		'''    LLL     3      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.SHORT_STANDALONE)
		'''    LLLL    4      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.FULL_STANDALONE)
		'''    LLLLL   5      appendText(ChronoField.MONTH_OF_YEAR, TextStyle.NARROW_STANDALONE)
		''' 
		'''    w       1      append special localized WeekFields element for numeric week-of-year
		'''    ww      2      append special localized WeekFields element for numeric week-of-year, zero-padded
		'''    W       1      append special localized WeekFields element for numeric week-of-month
		'''    d       1      appendValue(ChronoField.DAY_OF_MONTH)
		'''    dd      2      appendValue(ChronoField.DAY_OF_MONTH, 2)
		'''    D       1      appendValue(ChronoField.DAY_OF_YEAR)
		'''    DD      2      appendValue(ChronoField.DAY_OF_YEAR, 2)
		'''    DDD     3      appendValue(ChronoField.DAY_OF_YEAR, 3)
		'''    F       1      appendValue(ChronoField.ALIGNED_DAY_OF_WEEK_IN_MONTH)
		'''    E       1      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		'''    EE      2      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		'''    EEE     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		'''    EEEE    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL)
		'''    EEEEE   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW)
		'''    e       1      append special localized WeekFields element for numeric day-of-week
		'''    ee      2      append special localized WeekFields element for numeric day-of-week, zero-padded
		'''    eee     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT)
		'''    eeee    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL)
		'''    eeeee   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW)
		'''    c       1      append special localized WeekFields element for numeric day-of-week
		'''    ccc     3      appendText(ChronoField.DAY_OF_WEEK, TextStyle.SHORT_STANDALONE)
		'''    cccc    4      appendText(ChronoField.DAY_OF_WEEK, TextStyle.FULL_STANDALONE)
		'''    ccccc   5      appendText(ChronoField.DAY_OF_WEEK, TextStyle.NARROW_STANDALONE)
		''' </pre>
		''' <p>
		''' <b>Time fields</b>: Pattern letters to output a time.
		''' <pre>
		'''  Pattern  Count  Equivalent builder methods
		'''  -------  -----  --------------------------
		'''    a       1      appendText(ChronoField.AMPM_OF_DAY, TextStyle.SHORT)
		'''    h       1      appendValue(ChronoField.CLOCK_HOUR_OF_AMPM)
		'''    hh      2      appendValue(ChronoField.CLOCK_HOUR_OF_AMPM, 2)
		'''    H       1      appendValue(ChronoField.HOUR_OF_DAY)
		'''    HH      2      appendValue(ChronoField.HOUR_OF_DAY, 2)
		'''    k       1      appendValue(ChronoField.CLOCK_HOUR_OF_DAY)
		'''    kk      2      appendValue(ChronoField.CLOCK_HOUR_OF_DAY, 2)
		'''    K       1      appendValue(ChronoField.HOUR_OF_AMPM)
		'''    KK      2      appendValue(ChronoField.HOUR_OF_AMPM, 2)
		'''    m       1      appendValue(ChronoField.MINUTE_OF_HOUR)
		'''    mm      2      appendValue(ChronoField.MINUTE_OF_HOUR, 2)
		'''    s       1      appendValue(ChronoField.SECOND_OF_MINUTE)
		'''    ss      2      appendValue(ChronoField.SECOND_OF_MINUTE, 2)
		''' 
		'''    S..S    1..n   appendFraction(ChronoField.NANO_OF_SECOND, n, n, false)
		'''    A       1      appendValue(ChronoField.MILLI_OF_DAY)
		'''    A..A    2..n   appendValue(ChronoField.MILLI_OF_DAY, n)
		'''    n       1      appendValue(ChronoField.NANO_OF_SECOND)
		'''    n..n    2..n   appendValue(ChronoField.NANO_OF_SECOND, n)
		'''    N       1      appendValue(ChronoField.NANO_OF_DAY)
		'''    N..N    2..n   appendValue(ChronoField.NANO_OF_DAY, n)
		''' </pre>
		''' <p>
		''' <b>Zone ID</b>: Pattern letters to output {@code ZoneId}.
		''' <pre>
		'''  Pattern  Count  Equivalent builder methods
		'''  -------  -----  --------------------------
		'''    VV      2      appendZoneId()
		'''    z       1      appendZoneText(TextStyle.SHORT)
		'''    zz      2      appendZoneText(TextStyle.SHORT)
		'''    zzz     3      appendZoneText(TextStyle.SHORT)
		'''    zzzz    4      appendZoneText(TextStyle.FULL)
		''' </pre>
		''' <p>
		''' <b>Zone offset</b>: Pattern letters to output {@code ZoneOffset}.
		''' <pre>
		'''  Pattern  Count  Equivalent builder methods
		'''  -------  -----  --------------------------
		'''    O       1      appendLocalizedOffsetPrefixed(TextStyle.SHORT);
		'''    OOOO    4      appendLocalizedOffsetPrefixed(TextStyle.FULL);
		'''    X       1      appendOffset("+HHmm","Z")
		'''    XX      2      appendOffset("+HHMM","Z")
		'''    XXX     3      appendOffset("+HH:MM","Z")
		'''    XXXX    4      appendOffset("+HHMMss","Z")
		'''    XXXXX   5      appendOffset("+HH:MM:ss","Z")
		'''    x       1      appendOffset("+HHmm","+00")
		'''    xx      2      appendOffset("+HHMM","+0000")
		'''    xxx     3      appendOffset("+HH:MM","+00:00")
		'''    xxxx    4      appendOffset("+HHMMss","+0000")
		'''    xxxxx   5      appendOffset("+HH:MM:ss","+00:00")
		'''    Z       1      appendOffset("+HHMM","+0000")
		'''    ZZ      2      appendOffset("+HHMM","+0000")
		'''    ZZZ     3      appendOffset("+HHMM","+0000")
		'''    ZZZZ    4      appendLocalizedOffset(TextStyle.FULL);
		'''    ZZZZZ   5      appendOffset("+HH:MM:ss","Z")
		''' </pre>
		''' <p>
		''' <b>Modifiers</b>: Pattern letters that modify the rest of the pattern:
		''' <pre>
		'''  Pattern  Count  Equivalent builder methods
		'''  -------  -----  --------------------------
		'''    [       1      optionalStart()
		'''    ]       1      optionalEnd()
		'''    p..p    1..n   padNext(n)
		''' </pre>
		''' <p>
		''' Any sequence of letters not specified above, unrecognized letter or
		''' reserved character will throw an exception.
		''' Future versions may add to the set of patterns.
		''' It is recommended to use single quotes around all characters that you want
		''' to output directly to ensure that future changes do not break your application.
		''' <p>
		''' Note that the pattern string is similar, but not identical, to
		''' <seealso cref="java.text.SimpleDateFormat SimpleDateFormat"/>.
		''' The pattern string is also similar, but not identical, to that defined by the
		''' Unicode Common Locale Data Repository (CLDR/LDML).
		''' Pattern letters 'X' and 'u' are aligned with Unicode CLDR/LDML.
		''' By contrast, {@code SimpleDateFormat} uses 'u' for the numeric day of week.
		''' Pattern letters 'y' and 'Y' parse years of two digits and more than 4 digits differently.
		''' Pattern letters 'n', 'A', 'N', and 'p' are added.
		''' Number types will reject large numbers.
		''' </summary>
		''' <param name="pattern">  the pattern to add, not null </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		public DateTimeFormatterBuilder appendPattern(String pattern)
			java.util.Objects.requireNonNull(pattern, "pattern")
			parsePattern(pattern)
			Return Me

		private  Sub  parsePattern(String pattern)
			For pos As Integer = 0 To pattern.length() - 1
				Dim cur As Char = pattern.Chars(pos)
				If (cur >= "A"c AndAlso cur <= "Z"c) OrElse (cur >= "a"c AndAlso cur <= "z"c) Then
					Dim start As Integer = pos
					pos += 1
					Do While pos < pattern.length() AndAlso pattern.Chars(pos) = cur ' short loop

						pos += 1
					Loop
					Dim count As Integer = pos - start
					' padding
					If cur = "p"c Then
						Dim pad As Integer = 0
						If pos < pattern.length() Then
							cur = pattern.Chars(pos)
							If (cur >= "A"c AndAlso cur <= "Z"c) OrElse (cur >= "a"c AndAlso cur <= "z"c) Then
								pad = count
								start = pos
								pos += 1
								Do While pos < pattern.length() AndAlso pattern.Chars(pos) = cur ' short loop

									pos += 1
								Loop
								count = pos - start
							End If
						End If
						If pad = 0 Then Throw New IllegalArgumentException("Pad letter 'p' must be followed by valid pad pattern: " & pattern)
						padNext(pad) ' pad and continue parsing
					End If
					' main rules
					Dim field As java.time.temporal.TemporalField = FIELD_MAP(cur)
					If field IsNot Nothing Then
						parseField(cur, count, field)
					ElseIf cur = "z"c Then
						If count > 4 Then
							Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						ElseIf count = 4 Then
							appendZoneText(TextStyle.FULL)
						Else
							appendZoneText(TextStyle.SHORT)
						End If
					ElseIf cur = "V"c Then
						If count <> 2 Then Throw New IllegalArgumentException("Pattern letter count must be 2: " & AscW(cur))
						appendZoneId()
					ElseIf cur = "Z"c Then
						If count < 4 Then
							appendOffset("+HHMM", "+0000")
						ElseIf count = 4 Then
							appendLocalizedOffset(TextStyle.FULL)
						ElseIf count = 5 Then
							appendOffset("+HH:MM:ss","Z")
						Else
							Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						End If
					ElseIf cur = "O"c Then
						If count = 1 Then
							appendLocalizedOffset(TextStyle.SHORT)
						ElseIf count = 4 Then
							appendLocalizedOffset(TextStyle.FULL)
						Else
							Throw New IllegalArgumentException("Pattern letter count must be 1 or 4: " & AscW(cur))
						End If
					ElseIf cur = "X"c Then
						If count > 5 Then Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						appendOffset(OffsetIdPrinterParser.PATTERNS(count + (If(count = 1, 0, 1))), "Z")
					ElseIf cur = "x"c Then
						If count > 5 Then Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						Dim zero As String = (If(count = 1, "+00", (If(count Mod 2 = 0, "+0000", "+00:00"))))
						appendOffset(OffsetIdPrinterParser.PATTERNS(count + (If(count = 1, 0, 1))), zero)
					ElseIf cur = "W"c Then
						' Fields defined by Locale
						If count > 1 Then Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						appendInternal(New WeekBasedFieldPrinterParser(cur, count))
					ElseIf cur = "w"c Then
						' Fields defined by Locale
						If count > 2 Then Throw New IllegalArgumentException("Too many pattern letters: " & AscW(cur))
						appendInternal(New WeekBasedFieldPrinterParser(cur, count))
					ElseIf cur = "Y"c Then
						' Fields defined by Locale
						appendInternal(New WeekBasedFieldPrinterParser(cur, count))
					Else
						Throw New IllegalArgumentException("Unknown pattern letter: " & AscW(cur))
					End If
					pos -= 1

				ElseIf cur = "'"c Then
					' parse literals
					Dim start As Integer = pos
					pos += 1
					Do While pos < pattern.length()
						If pattern.Chars(pos) = "'"c Then
							If pos + 1 < pattern.length() AndAlso pattern.Chars(pos + 1) = "'"c Then
								pos += 1
							Else
								Exit Do ' end of literal
							End If
						End If
						pos += 1
					Loop
					If pos >= pattern.length() Then Throw New IllegalArgumentException("Pattern ends with an incomplete string literal: " & pattern)
					Dim str As String = pattern.Substring(start + 1, pos - (start + 1))
					If str.length() = 0 Then
						appendLiteral("'"c)
					Else
						appendLiteral(str.replace("''", "'"))
					End If

				ElseIf cur = "["c Then
					optionalStart()

				ElseIf cur = "]"c Then
					If active.parent Is Nothing Then Throw New IllegalArgumentException("Pattern invalid as it contains ] without previous [")
					optionalEnd()

				ElseIf cur = "{"c OrElse cur = "}"c OrElse cur = "#"c Then
					Throw New IllegalArgumentException("Pattern includes reserved character: '" & AscW(cur) & "'")
				Else
					appendLiteral(cur)
				End If
			Next pos

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		private  Sub  parseField(Char cur, Integer count, java.time.temporal.TemporalField field)
			Dim standalone As Boolean = False
			Select Case cur
				Case "u"c, "y"c
					If count = 2 Then
						appendValueReduced(field, 2, 2, ReducedPrinterParser.BASE_DATE)
					ElseIf count < 4 Then
						appendValue(field, count, 19, SignStyle.NORMAL)
					Else
						appendValue(field, count, 19, SignStyle.EXCEEDS_PAD)
					End If
				Case "c"c
					If count = 2 Then Throw New IllegalArgumentException("Invalid pattern ""cc""")
					'fallthrough
				Case "L"c, "q"c
					standalone = True
					'fallthrough
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case "M"c, "Q"c, "E"c, "e"c
					Select Case count
						Case 1, 2
							If cur = "c"c OrElse cur = "e"c Then
								appendInternal(New WeekBasedFieldPrinterParser(cur, count))
							ElseIf cur = "E"c Then
								appendText(field, TextStyle.SHORT)
							Else
								If count = 1 Then
									appendValue(field)
								Else
									appendValue(field, 2)
								End If
							End If
						Case 3
							appendText(field,If(standalone, TextStyle.SHORT_STANDALONE, TextStyle.SHORT))
						Case 4
							appendText(field,If(standalone, TextStyle.FULL_STANDALONE, TextStyle.FULL))
						Case 5
							appendText(field,If(standalone, TextStyle.NARROW_STANDALONE, TextStyle.NARROW))
						Case Else
							Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End Select
				Case "a"c
					If count = 1 Then
						appendText(field, TextStyle.SHORT)
					Else
						Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End If
				Case "G"c
					Select Case count
						Case 1, 2, 3
							appendText(field, TextStyle.SHORT)
						Case 4
							appendText(field, TextStyle.FULL)
						Case 5
							appendText(field, TextStyle.NARROW)
						Case Else
							Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End Select
				Case "S"c
					appendFraction(NANO_OF_SECOND, count, count, False)
				Case "F"c
					If count = 1 Then
						appendValue(field)
					Else
						Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End If
				Case "d"c, "h"c, "H"c, "k"c, "K"c, "m"c, "s"c
					If count = 1 Then
						appendValue(field)
					ElseIf count = 2 Then
						appendValue(field, count)
					Else
						Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End If
				Case "D"c
					If count = 1 Then
						appendValue(field)
					ElseIf count <= 3 Then
						appendValue(field, count)
					Else
						Throw New IllegalArgumentException("Too many pattern letters: " & cur)
					End If
				Case Else
					If count = 1 Then
						appendValue(field)
					Else
						appendValue(field, count)
					End If
			End Select

		''' <summary>
		''' Map of letters to fields. </summary>
		private static final IDictionary(Of Character, java.time.temporal.TemporalField) FIELD_MAP = New Dictionary(Of )
		static DateTimeFormatterBuilder()
			' SDF = SimpleDateFormat
			FIELD_MAP("G"c) = java.time.temporal.ChronoField.ERA ' SDF, LDML (different to both for 1/2 chars)
			FIELD_MAP("y"c) = java.time.temporal.ChronoField.YEAR_OF_ERA ' SDF, LDML
			FIELD_MAP("u"c) = java.time.temporal.ChronoField.YEAR ' LDML (different in SDF)
			FIELD_MAP("Q"c) = java.time.temporal.IsoFields.QUARTER_OF_YEAR ' LDML (removed quarter from 310)
			FIELD_MAP("q"c) = java.time.temporal.IsoFields.QUARTER_OF_YEAR ' LDML (stand-alone)
			FIELD_MAP("M"c) = java.time.temporal.ChronoField.MONTH_OF_YEAR ' SDF, LDML
			FIELD_MAP("L"c) = java.time.temporal.ChronoField.MONTH_OF_YEAR ' SDF, LDML (stand-alone)
			FIELD_MAP("D"c) = java.time.temporal.ChronoField.DAY_OF_YEAR ' SDF, LDML
			FIELD_MAP("d"c) = java.time.temporal.ChronoField.DAY_OF_MONTH ' SDF, LDML
			FIELD_MAP("F"c) = java.time.temporal.ChronoField.ALIGNED_DAY_OF_WEEK_IN_MONTH ' SDF, LDML
			FIELD_MAP("E"c) = java.time.temporal.ChronoField.DAY_OF_WEEK ' SDF, LDML (different to both for 1/2 chars)
			FIELD_MAP("c"c) = java.time.temporal.ChronoField.DAY_OF_WEEK ' LDML (stand-alone)
			FIELD_MAP("e"c) = java.time.temporal.ChronoField.DAY_OF_WEEK ' LDML (needs localized week number)
			FIELD_MAP("a"c) = java.time.temporal.ChronoField.AMPM_OF_DAY ' SDF, LDML
			FIELD_MAP("H"c) = java.time.temporal.ChronoField.HOUR_OF_DAY ' SDF, LDML
			FIELD_MAP("k"c) = java.time.temporal.ChronoField.CLOCK_HOUR_OF_DAY ' SDF, LDML
			FIELD_MAP("K"c) = java.time.temporal.ChronoField.HOUR_OF_AMPM ' SDF, LDML
			FIELD_MAP("h"c) = java.time.temporal.ChronoField.CLOCK_HOUR_OF_AMPM ' SDF, LDML
			FIELD_MAP("m"c) = java.time.temporal.ChronoField.MINUTE_OF_HOUR ' SDF, LDML
			FIELD_MAP("s"c) = java.time.temporal.ChronoField.SECOND_OF_MINUTE ' SDF, LDML
			FIELD_MAP("S"c) = java.time.temporal.ChronoField.NANO_OF_SECOND ' LDML (SDF uses milli-of-second number)
			FIELD_MAP("A"c) = java.time.temporal.ChronoField.MILLI_OF_DAY ' LDML
			FIELD_MAP("n"c) = java.time.temporal.ChronoField.NANO_OF_SECOND ' 310 (proposed for LDML)
			FIELD_MAP("N"c) = java.time.temporal.ChronoField.NANO_OF_DAY ' 310 (proposed for LDML)
			' 310 - z - time-zone names, matches LDML and SimpleDateFormat 1 to 4
			' 310 - Z - matches SimpleDateFormat and LDML
			' 310 - V - time-zone id, matches LDML
			' 310 - p - prefix for padding
			' 310 - X - matches LDML, almost matches SDF for 1, exact match 2&3, extended 4&5
			' 310 - x - matches LDML
			' 310 - w, W, and Y are localized forms matching LDML
			' LDML - U - cycle year name, not supported by 310 yet
			' LDML - l - deprecated
			' LDML - j - not relevant
			' LDML - g - modified-julian-day
			' LDML - v,V - extended time-zone names

		'-----------------------------------------------------------------------
		''' <summary>
		''' Causes the next added printer/parser to pad to a fixed width using a space.
		''' <p>
		''' This padding will pad to a fixed width using spaces.
		''' <p>
		''' During formatting, the decorated element will be output and then padded
		''' to the specified width. An exception will be thrown during formatting if
		''' the pad width is exceeded.
		''' <p>
		''' During parsing, the padding and decorated element are parsed.
		''' If parsing is lenient, then the pad width is treated as a maximum.
		''' The padding is parsed greedily. Thus, if the decorated element starts with
		''' the pad character, it will not be parsed.
		''' </summary>
		''' <param name="padWidth">  the pad width, 1 or greater </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if pad width is too small </exception>
		public DateTimeFormatterBuilder padNext(Integer padWidth)
			Return padNext(padWidth, " "c)

		''' <summary>
		''' Causes the next added printer/parser to pad to a fixed width.
		''' <p>
		''' This padding is intended for padding other than zero-padding.
		''' Zero-padding should be achieved using the appendValue methods.
		''' <p>
		''' During formatting, the decorated element will be output and then padded
		''' to the specified width. An exception will be thrown during formatting if
		''' the pad width is exceeded.
		''' <p>
		''' During parsing, the padding and decorated element are parsed.
		''' If parsing is lenient, then the pad width is treated as a maximum.
		''' If parsing is case insensitive, then the pad character is matched ignoring case.
		''' The padding is parsed greedily. Thus, if the decorated element starts with
		''' the pad character, it will not be parsed.
		''' </summary>
		''' <param name="padWidth">  the pad width, 1 or greater </param>
		''' <param name="padChar">  the pad character </param>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalArgumentException"> if pad width is too small </exception>
		public DateTimeFormatterBuilder padNext(Integer padWidth, Char padChar)
			If padWidth < 1 Then Throw New IllegalArgumentException("The pad width must be at least one but was " & padWidth)
			active.padNextWidth = padWidth
			active.padNextChar = padChar
			active.valueParserIndex = -1
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Mark the start of an optional section.
		''' <p>
		''' The output of formatting can include optional sections, which may be nested.
		''' An optional section is started by calling this method and ended by calling
		''' <seealso cref="#optionalEnd()"/> or by ending the build process.
		''' <p>
		''' All elements in the optional section are treated as optional.
		''' During formatting, the section is only output if data is available in the
		''' {@code TemporalAccessor} for all the elements in the section.
		''' During parsing, the whole section may be missing from the parsed string.
		''' <p>
		''' For example, consider a builder setup as
		''' {@code builder.appendValue(HOUR_OF_DAY,2).optionalStart().appendValue(MINUTE_OF_HOUR,2)}.
		''' The optional section ends automatically at the end of the builder.
		''' During formatting, the minute will only be output if its value can be obtained from the date-time.
		''' During parsing, the input will be successfully parsed whether the minute is present or not.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		public DateTimeFormatterBuilder optionalStart()
			active.valueParserIndex = -1
			active = New DateTimeFormatterBuilder(active, True)
			Return Me

		''' <summary>
		''' Ends an optional section.
		''' <p>
		''' The output of formatting can include optional sections, which may be nested.
		''' An optional section is started by calling <seealso cref="#optionalStart()"/> and ended
		''' using this method (or at the end of the builder).
		''' <p>
		''' Calling this method without having previously called {@code optionalStart}
		''' will throw an exception.
		''' Calling this method immediately after calling {@code optionalStart} has no effect
		''' on the formatter other than ending the (empty) optional section.
		''' <p>
		''' All elements in the optional section are treated as optional.
		''' During formatting, the section is only output if data is available in the
		''' {@code TemporalAccessor} for all the elements in the section.
		''' During parsing, the whole section may be missing from the parsed string.
		''' <p>
		''' For example, consider a builder setup as
		''' {@code builder.appendValue(HOUR_OF_DAY,2).optionalStart().appendValue(MINUTE_OF_HOUR,2).optionalEnd()}.
		''' During formatting, the minute will only be output if its value can be obtained from the date-time.
		''' During parsing, the input will be successfully parsed whether the minute is present or not.
		''' </summary>
		''' <returns> this, for chaining, not null </returns>
		''' <exception cref="IllegalStateException"> if there was no previous call to {@code optionalStart} </exception>
		public DateTimeFormatterBuilder optionalEnd()
			If active.parent Is Nothing Then Throw New IllegalStateException("Cannot call optionalEnd() as there was no previous call to optionalStart()")
			If active.printerParsers.Count > 0 Then
				Dim cpp As New CompositePrinterParser(active.printerParsers, active.optional)
				active = active.parent
				appendInternal(cpp)
			Else
				active = active.parent
			End If
			Return Me

		'-----------------------------------------------------------------------
		''' <summary>
		''' Appends a printer and/or parser to the internal list handling padding.
		''' </summary>
		''' <param name="pp">  the printer-parser to add, not null </param>
		''' <returns> the index into the active parsers list </returns>
		private Integer appendInternal(DateTimePrinterParser pp)
			java.util.Objects.requireNonNull(pp, "pp")
			If active.padNextWidth > 0 Then
				If pp IsNot Nothing Then pp = New PadPrinterParserDecorator(pp, active.padNextWidth, active.padNextChar)
				active.padNextWidth = 0
				active.padNextChar = 0
			End If
			active.printerParsers.Add(pp)
			active.valueParserIndex = -1
			Return active.printerParsers.Count - 1

		'-----------------------------------------------------------------------
		''' <summary>
		''' Completes this builder by creating the {@code DateTimeFormatter}
		''' using the default locale.
		''' <p>
		''' This will create a formatter with the <seealso cref="Locale#getDefault(Locale.Category) default FORMAT locale"/>.
		''' Numbers will be printed and parsed using the standard DecimalStyle.
		''' The resolver style will be <seealso cref="ResolverStyle#SMART SMART"/>.
		''' <p>
		''' Calling this method will end any open optional sections by repeatedly
		''' calling <seealso cref="#optionalEnd()"/> before creating the formatter.
		''' <p>
		''' This builder can still be used after creating the formatter if desired,
		''' although the state may have been changed by calls to {@code optionalEnd}.
		''' </summary>
		''' <returns> the created formatter, not null </returns>
		public DateTimeFormatter toFormatter()
			Return toFormatter(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))

		''' <summary>
		''' Completes this builder by creating the {@code DateTimeFormatter}
		''' using the specified locale.
		''' <p>
		''' This will create a formatter with the specified locale.
		''' Numbers will be printed and parsed using the standard DecimalStyle.
		''' The resolver style will be <seealso cref="ResolverStyle#SMART SMART"/>.
		''' <p>
		''' Calling this method will end any open optional sections by repeatedly
		''' calling <seealso cref="#optionalEnd()"/> before creating the formatter.
		''' <p>
		''' This builder can still be used after creating the formatter if desired,
		''' although the state may have been changed by calls to {@code optionalEnd}.
		''' </summary>
		''' <param name="locale">  the locale to use for formatting, not null </param>
		''' <returns> the created formatter, not null </returns>
		public DateTimeFormatter toFormatter(java.util.Locale locale)
			Return toFormatter(locale, ResolverStyle.SMART, Nothing)

		''' <summary>
		''' Completes this builder by creating the formatter.
		''' This uses the default locale.
		''' </summary>
		''' <param name="resolverStyle">  the resolver style to use, not null </param>
		''' <returns> the created formatter, not null </returns>
		DateTimeFormatter toFormatter(ResolverStyle resolverStyle, java.time.chrono.Chronology chrono)
			Return toFormatter(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), resolverStyle, chrono)

		''' <summary>
		''' Completes this builder by creating the formatter.
		''' </summary>
		''' <param name="locale">  the locale to use for formatting, not null </param>
		''' <param name="chrono">  the chronology to use, may be null </param>
		''' <returns> the created formatter, not null </returns>
		private DateTimeFormatter toFormatter(java.util.Locale locale, ResolverStyle resolverStyle, java.time.chrono.Chronology chrono)
			java.util.Objects.requireNonNull(locale, "locale")
			Do While active.parent IsNot Nothing
				optionalEnd()
			Loop
			Dim pp As New CompositePrinterParser(printerParsers, False)
			Return New DateTimeFormatter(pp, locale, DecimalStyle.STANDARD, resolverStyle, Nothing, chrono, Nothing)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Strategy for formatting/parsing date-time information.
		''' <p>
		''' The printer may format any part, or the whole, of the input date-time object.
		''' Typically, a complete format is constructed from a number of smaller
		''' units, each outputting a single field.
		''' <p>
		''' The parser may parse any piece of text from the input, storing the result
		''' in the context. Typically, each individual parser will just parse one
		''' field, such as the day-of-month, storing the value in the context.
		''' Once the parse is complete, the caller will then resolve the parsed values
		''' to create the desired object, such as a {@code LocalDate}.
		''' <p>
		''' The parse position will be updated during the parse. Parsing will start at
		''' the specified index and the return value specifies the new parse position
		''' for the next parser. If an error occurs, the returned index will be negative
		''' and will have the error position encoded using the complement operator.
		''' 
		''' @implSpec
		''' This interface must be implemented with care to ensure other classes operate correctly.
		''' All implementations that can be instantiated must be final, immutable and thread-safe.
		''' <p>
		''' The context is not a thread-safe object and a new instance will be created
		''' for each format that occurs. The context must not be stored in an instance
		''' variable or shared with any other threads.
		''' </summary>
		Dim DateTimePrinterParser As interface

			''' <summary>
			''' Prints the date-time object to the buffer.
			''' <p>
			''' The context holds information to use during the format.
			''' It also contains the date-time information to be printed.
			''' <p>
			''' The buffer must not be mutated beyond the content controlled by the implementation.
			''' </summary>
			''' <param name="context">  the context to format using, not null </param>
			''' <param name="buf">  the buffer to append to, not null </param>
			''' <returns> false if unable to query the value from the date-time, true otherwise </returns>
			''' <exception cref="DateTimeException"> if the date-time cannot be printed successfully </exception>
			Boolean format(DateTimePrintContext context, StringBuilder buf)

			''' <summary>
			''' Parses text into date-time information.
			''' <p>
			''' The context holds information to use during the parse.
			''' It is also used to store the parsed date-time information.
			''' </summary>
			''' <param name="context">  the context to use and parse into, not null </param>
			''' <param name="text">  the input text to parse, not null </param>
			''' <param name="position">  the position to start parsing at, from 0 to the text length </param>
			''' <returns> the new parse position, where negative means an error with the
			'''  error position encoded using the complement ~ operator </returns>
			''' <exception cref="NullPointerException"> if the context or text is null </exception>
			''' <exception cref="IndexOutOfBoundsException"> if the position is invalid </exception>
			Integer parse(DateTimeParseContext context, CharSequence text, Integer position)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Composite printer and parser.
		''' </summary>
		static final class CompositePrinterParser implements DateTimePrinterParser
			private final DateTimePrinterParser() printerParsers
			private final Boolean [optional]

			CompositePrinterParser(IList(Of DateTimePrinterParser) printerParsers, Boolean [optional])
				Me(printerParsers.ToArray(), [optional])

			CompositePrinterParser(DateTimePrinterParser() printerParsers, Boolean [optional])
				Me.printerParsers = printerParsers
				Me.optional = [optional]

			''' <summary>
			''' Returns a copy of this printer-parser with the optional flag changed.
			''' </summary>
			''' <param name="optional">  the optional flag to set in the copy </param>
			''' <returns> the new printer-parser, not null </returns>
			public CompositePrinterParser withOptional(Boolean [optional])
				If [optional] = Me.optional Then Return Me
				Return New CompositePrinterParser(printerParsers, [optional])

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim length As Integer = buf.length()
				If [optional] Then context.startOptional()
				Try
					For Each pp As DateTimePrinterParser In printerParsers
						If pp.format(context, buf) = False Then
							buf.length = length ' reset buffer
							Return True
						End If
					Next pp
				Finally
					If [optional] Then context.endOptional()
				End Try
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				If [optional] Then
					context.startOptional()
					Dim pos As Integer = position
					For Each pp As DateTimePrinterParser In printerParsers
						pos = pp.parse(context, text, pos)
						If pos < 0 Then
							context.endOptional(False)
							Return position ' return original position
						End If
					Next pp
					context.endOptional(True)
					Return pos
				Else
					For Each pp As DateTimePrinterParser In printerParsers
						position = pp.parse(context, text, position)
						If position < 0 Then Exit For
					Next pp
					Return position
				End If

			public String ToString()
				Dim buf As New StringBuilder
				If printerParsers IsNot Nothing Then
					buf.append(If([optional], "[", "("))
					For Each pp As DateTimePrinterParser In printerParsers
						buf.append(pp)
					Next pp
					buf.append(If([optional], "]", ")"))
				End If
				Return buf.ToString()

		'-----------------------------------------------------------------------
		''' <summary>
		''' Pads the output to a fixed width.
		''' </summary>
		static final class PadPrinterParserDecorator implements DateTimePrinterParser
			private final DateTimePrinterParser printerParser
			private final Integer padWidth
			private final Char padChar

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="printerParser">  the printer, not null </param>
			''' <param name="padWidth">  the width to pad to, 1 or greater </param>
			''' <param name="padChar">  the pad character </param>
			PadPrinterParserDecorator(DateTimePrinterParser printerParser, Integer padWidth, Char padChar)
				' input checked by DateTimeFormatterBuilder
				Me.printerParser = printerParser
				Me.padWidth = padWidth
				Me.padChar = padChar

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim preLen As Integer = buf.length()
				If printerParser.format(context, buf) = False Then Return False
				Dim len As Integer = buf.length() - preLen
				If len > padWidth Then Throw New java.time.DateTimeException("Cannot print as output of " & len & " characters exceeds pad width of " & padWidth)
				For i As Integer = 0 To padWidth - len - 1
					buf.insert(preLen, padChar)
				Next i
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				' cache context before changed by decorated parser
				Dim [strict] As Boolean = context.strict
				' parse
				If position > text.length() Then Throw New IndexOutOfBoundsException
				If position = text.length() Then Return Not position ' no more characters in the string
				Dim endPos As Integer = position + padWidth
				If endPos > text.length() Then
					If [strict] Then Return Not position ' not enough characters in the string to meet the parse width
					endPos = text.length()
				End If
				Dim pos As Integer = position
				Do While pos < endPos AndAlso context.charEquals(text.Chars(pos), padChar)
					pos += 1
				Loop
				text = text.subSequence(0, endPos)
				Dim resultPos As Integer = printerParser.parse(context, text, pos)
				If resultPos <> endPos AndAlso [strict] Then Return Not(position + pos) ' parse of decorated field didn't parse to the end
				Return resultPos

			public String ToString()
				Return "Pad(" & printerParser & "," & padWidth + (If(padChar = " "c, ")", ",'" & padChar & "')"))

		'-----------------------------------------------------------------------
		''' <summary>
		''' Enumeration to apply simple parse settings.
		''' </summary>
		static enum SettingsParser implements DateTimePrinterParser
			SENSITIVE,
			INSENSITIVE,
			STRICT,
			LENIENT

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Return True ' nothing to do here

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				' using ordinals to avoid javac synthetic inner class
				Select Case ordinal()
					Case 0
						context.caseSensitive = True
					Case 1
						context.caseSensitive = False
					Case 2
						context.strict = True
					Case 3
						context.strict = False
				End Select
				Return position

			public String ToString()
				' using ordinals to avoid javac synthetic inner class
				Select Case ordinal()
					Case 0
						Return "ParseCaseSensitive(true)"
					Case 1
						Return "ParseCaseSensitive(false)"
					Case 2
						Return "ParseStrict(true)"
					Case 3
						Return "ParseStrict(false)"
				End Select
				Throw New IllegalStateException("Unreachable")

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defaults a value into the parse if not currently present.
		''' </summary>
		static class DefaultValueParser implements DateTimePrinterParser
			private final java.time.temporal.TemporalField field
			private final Long value

			DefaultValueParser(java.time.temporal.TemporalField field, Long value)
				Me.field = field
				Me.value = value

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				If context.getParsed(field) Is Nothing Then context.parsedFieldeld(field, value, position, position)
				Return position

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a character literal.
		''' </summary>
		static final class CharLiteralPrinterParser implements DateTimePrinterParser
			private final Char literal

			CharLiteralPrinterParser(Char literal)
				Me.literal = literal

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				buf.append(literal)
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim length As Integer = text.length()
				If position = length Then Return Not position
				Dim ch As Char = text.Chars(position)
				If ch <> literal Then
					If context.caseSensitive OrElse (Char.ToUpper(ch) <> Char.ToUpper(literal) AndAlso Char.ToLower(ch) <> Char.ToLower(literal)) Then Return Not position
				End If
				Return position + 1

			public String ToString()
				If literal = "'"c Then Return "''"
				Return "'" & literal & "'"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a string literal.
		''' </summary>
		static final class StringLiteralPrinterParser implements DateTimePrinterParser
			private final String literal

			StringLiteralPrinterParser(String literal)
				Me.literal = literal ' validated by caller

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				buf.append(literal)
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim length As Integer = text.length()
				If position > length OrElse position < 0 Then Throw New IndexOutOfBoundsException
				If context.subSequenceEquals(text, position, literal, 0, literal.length()) = False Then Return Not position
				Return position + literal.length()

			public String ToString()
				Dim converted As String = literal.replace("'", "''")
				Return "'" & converted & "'"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints and parses a numeric date-time field with optional padding.
		''' </summary>
		static class NumberPrinterParser implements DateTimePrinterParser

			''' <summary>
			''' Array of 10 to the power of n.
			''' </summary>
			static final Long() EXCEED_POINTS = New Long() { 0L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 1000000000L, 10000000000L }

			Dim field As java.time.temporal.TemporalField
			Dim minWidth As Integer
			Dim maxWidth As Integer
			private final SignStyle signStyle
			Dim subsequentWidth As Integer

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="field">  the field to format, not null </param>
			''' <param name="minWidth">  the minimum field width, from 1 to 19 </param>
			''' <param name="maxWidth">  the maximum field width, from minWidth to 19 </param>
			''' <param name="signStyle">  the positive/negative sign style, not null </param>
			NumberPrinterParser(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, SignStyle signStyle)
				' validated by caller
				Me.field = field
				Me.minWidth = minWidth
				Me.maxWidth = maxWidth
				Me.signStyle = signStyle
				Me.subsequentWidth = 0

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="field">  the field to format, not null </param>
			''' <param name="minWidth">  the minimum field width, from 1 to 19 </param>
			''' <param name="maxWidth">  the maximum field width, from minWidth to 19 </param>
			''' <param name="signStyle">  the positive/negative sign style, not null </param>
			''' <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater,
			'''  -1 if fixed width due to active adjacent parsing </param>
			protected NumberPrinterParser(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, SignStyle signStyle, Integer subsequentWidth)
				' validated by caller
				Me.field = field
				Me.minWidth = minWidth
				Me.maxWidth = maxWidth
				Me.signStyle = signStyle
				Me.subsequentWidth = subsequentWidth

			''' <summary>
			''' Returns a new instance with fixed width flag set.
			''' </summary>
			''' <returns> a new updated printer-parser, not null </returns>
			NumberPrinterParser withFixedWidth()
				If subsequentWidth = -1 Then Return Me
				Return New NumberPrinterParser(field, minWidth, maxWidth, signStyle, -1)

			''' <summary>
			''' Returns a new instance with an updated subsequent width.
			''' </summary>
			''' <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater </param>
			''' <returns> a new updated printer-parser, not null </returns>
			NumberPrinterParser withSubsequentWidth(Integer subsequentWidth)
				Return New NumberPrinterParser(field, minWidth, maxWidth, signStyle, Me.subsequentWidth + subsequentWidth)

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim valueLong As Long? = context.getValue(field)
				If valueLong Is Nothing Then Return False
				Dim value As Long = getValue(context, valueLong)
				Dim decimalStyle_Renamed As DecimalStyle = context.decimalStyle
				Dim str As String = (If(value = java.lang.[Long].MIN_VALUE, "9223372036854775808", Convert.ToString (System.Math.Abs(value))))
				If str.length() > maxWidth Then Throw New java.time.DateTimeException("Field " & field & " cannot be printed as the value " & value & " exceeds the maximum print width of " & maxWidth)
				str = decimalStyle_Renamed.convertNumberToI18N(str)

				If value >= 0 Then
					Select Case signStyle
						Case EXCEEDS_PAD
							If minWidth < 19 AndAlso value >= EXCEED_POINTS(minWidth) Then buf.append(decimalStyle_Renamed.positiveSign)
						Case ALWAYS
							buf.append(decimalStyle_Renamed.positiveSign)
					End Select
				Else
					Select Case signStyle
						Case NORMAL, EXCEEDS_PAD, ALWAYS
							buf.append(decimalStyle_Renamed.negativeSign)
						Case NOT_NEGATIVE
							Throw New java.time.DateTimeException("Field " & field & " cannot be printed as the value " & value & " cannot be negative according to the SignStyle")
					End Select
				End If
				For i As Integer = 0 To minWidth - str.length() - 1
					buf.append(decimalStyle_Renamed.zeroDigit)
				Next i
				buf.append(str)
				Return True

			''' <summary>
			''' Gets the value to output.
			''' </summary>
			''' <param name="context">  the context </param>
			''' <param name="value">  the value of the field, not null </param>
			''' <returns> the value </returns>
			Long getValue(DateTimePrintContext context, Long value)
				Return value

			''' <summary>
			''' For NumberPrinterParser, the width is fixed depending on the
			''' minWidth, maxWidth, signStyle and whether subsequent fields are fixed. </summary>
			''' <param name="context"> the context </param>
			''' <returns> true if the field is fixed width </returns>
			''' <seealso cref= DateTimeFormatterBuilder#appendValue(java.time.temporal.TemporalField, int) </seealso>
			Boolean isFixedWidth(DateTimeParseContext context)
				Return subsequentWidth = -1 OrElse (subsequentWidth > 0 AndAlso minWidth = maxWidth AndAlso signStyle = SignStyle.NOT_NEGATIVE)

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim length As Integer = text.length()
				If position = length Then Return Not position
				Dim sign As Char = text.Chars(position) ' IOOBE if invalid position
				Dim negative As Boolean = False
				Dim positive As Boolean = False
				If sign = context.decimalStyle.positiveSign Then
					If signStyle.parse(True, context.strict, minWidth = maxWidth) = False Then Return Not position
					positive = True
					position += 1
				ElseIf sign = context.decimalStyle.negativeSign Then
					If signStyle.parse(False, context.strict, minWidth = maxWidth) = False Then Return Not position
					negative = True
					position += 1
				Else
					If signStyle = SignStyle.ALWAYS AndAlso context.strict Then Return Not position
				End If
				Dim effMinWidth As Integer = (If(context.strict OrElse isFixedWidth(context), minWidth, 1))
				Dim minEndPos As Integer = position + effMinWidth
				If minEndPos > length Then Return Not position
				Dim effMaxWidth As Integer = (If(context.strict OrElse isFixedWidth(context), maxWidth, 9)) + System.Math.Max(subsequentWidth, 0)
				Dim total As Long = 0
				Dim totalBig As System.Numerics.BigInteger = Nothing
				Dim pos As Integer = position
				For pass As Integer = 0 To 1
					Dim maxEndPos As Integer = System.Math.Min(pos + effMaxWidth, length)
					Do While pos < maxEndPos
						Dim ch As Char = text.Chars(pos)
						pos += 1
						Dim digit As Integer = context.decimalStyle.convertToDigit(ch)
						If digit < 0 Then
							pos -= 1
							If pos < minEndPos Then Return Not position ' need at least min width digits
							Exit Do
						End If
						If (pos - position) > 18 Then
							If totalBig Is Nothing Then totalBig = System.Numerics.Big java.lang.[Integer].valueOf(total)
							totalBig = totalBig * System.Numerics.Big java.lang.[Integer].TEN.add(System.Numerics.Big java.lang.[Integer].valueOf(digit))
						Else
							total = total * 10 + digit
						End If
					Loop
					If subsequentWidth > 0 AndAlso pass = 0 Then
						' re-parse now we know the correct width
						Dim parseLen As Integer = pos - position
						effMaxWidth = System.Math.Max(effMinWidth, parseLen - subsequentWidth)
						pos = position
						total = 0
						totalBig = Nothing
					Else
						Exit For
					End If
				Next pass
				If negative Then
					If totalBig IsNot Nothing Then
						If totalBig.Equals(System.Numerics.Big java.lang.[Integer].ZERO) AndAlso context.strict Then Return Not(position - 1) ' minus zero not allowed
						totalBig = -totalBig
					Else
						If total = 0 AndAlso context.strict Then Return Not(position - 1) ' minus zero not allowed
						total = -total
					End If
				ElseIf signStyle = SignStyle.EXCEEDS_PAD AndAlso context.strict Then
					Dim parseLen As Integer = pos - position
					If positive Then
						If parseLen <= minWidth Then Return Not(position - 1) ' '+' only parsed if minWidth exceeded
					Else
						If parseLen > minWidth Then Return Not position ' '+' must be parsed if minWidth exceeded
					End If
				End If
				If totalBig IsNot Nothing Then
					If totalBig.bitLength() > 63 Then
						' overflow, parse 1 less digit
						totalBig = totalBig / System.Numerics.Big java.lang.[Integer].TEN
						pos -= 1
					End If
					Return valuelue(context, totalBig, position, pos)
				End If
				Return valuelue(context, total, position, pos)

			''' <summary>
			''' Stores the value.
			''' </summary>
			''' <param name="context">  the context to store into, not null </param>
			''' <param name="value">  the value </param>
			''' <param name="errorPos">  the position of the field being parsed </param>
			''' <param name="successPos">  the position after the field being parsed </param>
			''' <returns> the new position </returns>
			Integer valuelue(DateTimeParseContext context, Long value, Integer errorPos, Integer successPos)
				Return context.parsedFieldeld(field, value, errorPos, successPos)

			public String ToString()
				If minWidth = 1 AndAlso maxWidth = 19 AndAlso signStyle = SignStyle.NORMAL Then Return "Value(" & field & ")"
				If minWidth = maxWidth AndAlso signStyle = SignStyle.NOT_NEGATIVE Then Return "Value(" & field & "," & minWidth & ")"
				Return "Value(" & field & "," & minWidth & "," & maxWidth & "," & signStyle & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints and parses a reduced numeric date-time field.
		''' </summary>
		static final class ReducedPrinterParser extends NumberPrinterParser
			''' <summary>
			''' The base date for reduced value parsing.
			''' </summary>
			static final java.time.LocalDate BASE_DATE = java.time.LocalDate.of(2000, 1, 1)

			private final Integer baseValue
			private final java.time.chrono.ChronoLocalDate baseDate

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="field">  the field to format, validated not null </param>
			''' <param name="minWidth">  the minimum field width, from 1 to 10 </param>
			''' <param name="maxWidth">  the maximum field width, from 1 to 10 </param>
			''' <param name="baseValue">  the base value </param>
			''' <param name="baseDate">  the base date </param>
			ReducedPrinterParser(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, Integer baseValue, java.time.chrono.ChronoLocalDate baseDate)
				Me(field, minWidth, maxWidth, baseValue, baseDate, 0)
				If minWidth < 1 OrElse minWidth > 10 Then Throw New IllegalArgumentException("The minWidth must be from 1 to 10 inclusive but was " & minWidth)
				If maxWidth < 1 OrElse maxWidth > 10 Then Throw New IllegalArgumentException("The maxWidth must be from 1 to 10 inclusive but was " & minWidth)
				If maxWidth < minWidth Then Throw New IllegalArgumentException("Maximum width must exceed or equal the minimum width but " & maxWidth & " < " & minWidth)
				If baseDate Is Nothing Then
					If field.range().isValidValue(baseValue) = False Then Throw New IllegalArgumentException("The base value must be within the range of the field")
					If ((CLng(Fix(baseValue))) + EXCEED_POINTS(maxWidth)) >  java.lang.[Integer].Max_Value Then Throw New java.time.DateTimeException("Unable to add printer-parser as the range exceeds the capacity of an int")
				End If

			''' <summary>
			''' Constructor.
			''' The arguments have already been checked.
			''' </summary>
			''' <param name="field">  the field to format, validated not null </param>
			''' <param name="minWidth">  the minimum field width, from 1 to 10 </param>
			''' <param name="maxWidth">  the maximum field width, from 1 to 10 </param>
			''' <param name="baseValue">  the base value </param>
			''' <param name="baseDate">  the base date </param>
			''' <param name="subsequentWidth"> the subsequentWidth for this instance </param>
			private ReducedPrinterParser(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, Integer baseValue, java.time.chrono.ChronoLocalDate baseDate, Integer subsequentWidth)
				MyBase(field, minWidth, maxWidth, SignStyle.NOT_NEGATIVE, subsequentWidth)
				Me.baseValue = baseValue
				Me.baseDate = baseDate

			Long getValue(DateTimePrintContext context, Long value)
				Dim absValue As Long = System.Math.Abs(value)
				Dim baseValue As Integer = Me.baseValue
				If baseDate IsNot Nothing Then
					Dim chrono As java.time.chrono.Chronology = java.time.chrono.Chronology.from(context.temporal)
					baseValue = chrono.date(baseDate).get(field)
				End If
				If value >= baseValue AndAlso value < baseValue + EXCEED_POINTS(minWidth) Then Return absValue Mod EXCEED_POINTS(minWidth)
				' Otherwise truncate to fit in maxWidth
				Return absValue Mod EXCEED_POINTS(maxWidth)

			Integer valuelue(DateTimeParseContext context, Long value, Integer errorPos, Integer successPos)
				Dim baseValue As Integer = Me.baseValue
				If baseDate IsNot Nothing Then
					Dim chrono As java.time.chrono.Chronology = context.effectiveChronology
					baseValue = chrono.date(baseDate).get(field)

					' In case the Chronology is changed later, add a callback when/if it changes
					Dim initialValue As Long = value
					context.addChronoChangedListener((_unused) -> { valuelue(context, initialValue, errorPos, successPos); })
	'                             Repeat the set of the field using the current Chronology
	'                             * The success/error position is ignored because the value is
	'                             * intentionally being overwritten.
	'                             
				End If
				Dim parseLen As Integer = successPos - errorPos
				If parseLen = minWidth AndAlso value >= 0 Then
					Dim range As Long = EXCEED_POINTS(minWidth)
					Dim lastPart As Long = baseValue Mod range
					Dim basePart As Long = baseValue - lastPart
					If baseValue > 0 Then
						value = basePart + value
					Else
						value = basePart - value
					End If
					If value < baseValue Then value += range
				End If
				Return context.parsedFieldeld(field, value, errorPos, successPos)

			''' <summary>
			''' Returns a new instance with fixed width flag set.
			''' </summary>
			''' <returns> a new updated printer-parser, not null </returns>
			ReducedPrinterParser withFixedWidth()
				If subsequentWidth = -1 Then Return Me
				Return New ReducedPrinterParser(field, minWidth, maxWidth, baseValue, baseDate, -1)

			''' <summary>
			''' Returns a new instance with an updated subsequent width.
			''' </summary>
			''' <param name="subsequentWidth">  the width of subsequent non-negative numbers, 0 or greater </param>
			''' <returns> a new updated printer-parser, not null </returns>
			ReducedPrinterParser withSubsequentWidth(Integer subsequentWidth)
				Return New ReducedPrinterParser(field, minWidth, maxWidth, baseValue, baseDate, Me.subsequentWidth + subsequentWidth)

			''' <summary>
			''' For a ReducedPrinterParser, fixed width is false if the mode is strict,
			''' otherwise it is set as for NumberPrinterParser. </summary>
			''' <param name="context"> the context </param>
			''' <returns> if the field is fixed width </returns>
			''' <seealso cref= DateTimeFormatterBuilder#appendValueReduced(java.time.temporal.TemporalField, int, int, int) </seealso>
			Boolean isFixedWidth(DateTimeParseContext context)
			   If context.strict = False Then Return False
			   Return MyBase.isFixedWidth(context)

			public String ToString()
				Return "ReducedValue(" & field & "," & minWidth & "," & maxWidth & "," & (If(baseDate IsNot Nothing, baseDate, baseValue)) & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints and parses a numeric date-time field with optional padding.
		''' </summary>
		static final class FractionPrinterParser implements DateTimePrinterParser
			private final java.time.temporal.TemporalField field
			private final Integer minWidth
			private final Integer maxWidth
			private final Boolean decimalPoint

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="field">  the field to output, not null </param>
			''' <param name="minWidth">  the minimum width to output, from 0 to 9 </param>
			''' <param name="maxWidth">  the maximum width to output, from 0 to 9 </param>
			''' <param name="decimalPoint">  whether to output the localized decimal point symbol </param>
			FractionPrinterParser(java.time.temporal.TemporalField field, Integer minWidth, Integer maxWidth, Boolean decimalPoint)
				java.util.Objects.requireNonNull(field, "field")
				If field.range().fixed = False Then Throw New IllegalArgumentException("Field must have a fixed set of values: " & field)
				If minWidth < 0 OrElse minWidth > 9 Then Throw New IllegalArgumentException("Minimum width must be from 0 to 9 inclusive but was " & minWidth)
				If maxWidth < 1 OrElse maxWidth > 9 Then Throw New IllegalArgumentException("Maximum width must be from 1 to 9 inclusive but was " & maxWidth)
				If maxWidth < minWidth Then Throw New IllegalArgumentException("Maximum width must exceed or equal the minimum width but " & maxWidth & " < " & minWidth)
				Me.field = field
				Me.minWidth = minWidth
				Me.maxWidth = maxWidth
				Me.decimalPoint = decimalPoint

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim value As Long? = context.getValue(field)
				If value Is Nothing Then Return False
				Dim decimalStyle_Renamed As DecimalStyle = context.decimalStyle
				Dim fraction As Decimal = convertToFraction(value)
				If fraction.scale() = 0 Then ' scale is zero if value is zero
					If minWidth > 0 Then
						If decimalPoint Then buf.append(decimalStyle_Renamed.decimalSeparator)
						For i As Integer = 0 To minWidth - 1
							buf.append(decimalStyle_Renamed.zeroDigit)
						Next i
					End If
				Else
					Dim outputScale As Integer = System.Math.Min (System.Math.Max(fraction.scale(), minWidth), maxWidth)
					fraction = fraction.scaleale(outputScale, java.math.RoundingMode.FLOOR)
					Dim str As String = fraction.toPlainString().Substring(2)
					str = decimalStyle_Renamed.convertNumberToI18N(str)
					If decimalPoint Then buf.append(decimalStyle_Renamed.decimalSeparator)
					buf.append(str)
				End If
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim effectiveMin As Integer = (If(context.strict, minWidth, 0))
				Dim effectiveMax As Integer = (If(context.strict, maxWidth, 9))
				Dim length As Integer = text.length()
				If position = length Then Return (If(effectiveMin > 0, (Not position), position))
				If decimalPoint Then
					If text.Chars(position) <> context.decimalStyle.decimalSeparator Then Return (If(effectiveMin > 0, (Not position), position))
					position += 1
				End If
				Dim minEndPos As Integer = position + effectiveMin
				If minEndPos > length Then Return Not position ' need at least min width digits
				Dim maxEndPos As Integer = System.Math.Min(position + effectiveMax, length)
				Dim total As Integer = 0 ' can use int because we are only parsing up to 9 digits
				Dim pos As Integer = position
				Do While pos < maxEndPos
					Dim ch As Char = text.Chars(pos)
					pos += 1
					Dim digit As Integer = context.decimalStyle.convertToDigit(ch)
					If digit < 0 Then
						If pos < minEndPos Then Return Not position ' need at least min width digits
						pos -= 1
						Exit Do
					End If
					total = total * 10 + digit
				Loop
				Dim fraction As (New Decimal(total)).movePointLeft(pos - position)
				Dim value As Long = convertFromFraction(fraction)
				Return context.parsedFieldeld(field, value, position, pos)

			''' <summary>
			''' Converts a value for this field to a fraction between 0 and 1.
			''' <p>
			''' The fractional value is between 0 (inclusive) and 1 (exclusive).
			''' It can only be returned if the <seealso cref="java.time.temporal.TemporalField#range() value range"/> is fixed.
			''' The fraction is obtained by calculation from the field range using 9 decimal
			''' places and a rounding mode of <seealso cref="RoundingMode#FLOOR FLOOR"/>.
			''' The calculation is inaccurate if the values do not run continuously from smallest to largest.
			''' <p>
			''' For example, the second-of-minute value of 15 would be returned as 0.25,
			''' assuming the standard definition of 60 seconds in a minute.
			''' </summary>
			''' <param name="value">  the value to convert, must be valid for this rule </param>
			''' <returns> the value as a fraction within the range, from 0 to 1, not null </returns>
			''' <exception cref="DateTimeException"> if the value cannot be converted to a fraction </exception>
			private Decimal convertToFraction(Long value)
				Dim range As java.time.temporal.ValueRange = field.range()
				range.checkValidValue(value, field)
				Dim minBD As Decimal = Decimal.valueOf(range.minimum)
				Dim rangeBD As Decimal = Decimal.valueOf(range.maximum) - minBD + Decimal.One
				Dim valueBD As Decimal = Decimal.valueOf(value) - minBD
				Dim fraction As Decimal = valueBD.divide(rangeBD, 9, java.math.RoundingMode.FLOOR)
				' stripTrailingZeros bug
				Return If(fraction.CompareTo(Decimal.Zero) = 0, Decimal.Zero, fraction.stripTrailingZeros())

			''' <summary>
			''' Converts a fraction from 0 to 1 for this field to a value.
			''' <p>
			''' The fractional value must be between 0 (inclusive) and 1 (exclusive).
			''' It can only be returned if the <seealso cref="java.time.temporal.TemporalField#range() value range"/> is fixed.
			''' The value is obtained by calculation from the field range and a rounding
			''' mode of <seealso cref="RoundingMode#FLOOR FLOOR"/>.
			''' The calculation is inaccurate if the values do not run continuously from smallest to largest.
			''' <p>
			''' For example, the fractional second-of-minute of 0.25 would be converted to 15,
			''' assuming the standard definition of 60 seconds in a minute.
			''' </summary>
			''' <param name="fraction">  the fraction to convert, not null </param>
			''' <returns> the value of the field, valid for this rule </returns>
			''' <exception cref="DateTimeException"> if the value cannot be converted </exception>
			private Long convertFromFraction(Decimal fraction)
				Dim range As java.time.temporal.ValueRange = field.range()
				Dim minBD As Decimal = Decimal.valueOf(range.minimum)
				Dim rangeBD As Decimal = Decimal.valueOf(range.maximum) - minBD + Decimal.One
				Dim valueBD As Decimal = fraction.multiply(rangeBD).scaleale(0, java.math.RoundingMode.FLOOR).add(minBD)
				Return valueBD.longValueExact()

			public String ToString()
				Dim [decimal] As String = (If(decimalPoint, ",DecimalPoint", ""))
				Return "Fraction(" & field & "," & minWidth & "," & maxWidth + [decimal] & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses field text.
		''' </summary>
		static final class TextPrinterParser implements DateTimePrinterParser
			private final java.time.temporal.TemporalField field
			private final TextStyle textStyle
			private final DateTimeTextProvider provider
			''' <summary>
			''' The cached number printer parser.
			''' Immutable and volatile, so no synchronization needed.
			''' </summary>
			private volatile NumberPrinterParser numberPrinterParser

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="field">  the field to output, not null </param>
			''' <param name="textStyle">  the text style, not null </param>
			''' <param name="provider">  the text provider, not null </param>
			TextPrinterParser(java.time.temporal.TemporalField field, TextStyle textStyle, DateTimeTextProvider provider)
				' validated by caller
				Me.field = field
				Me.textStyle = textStyle
				Me.provider = provider

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim value As Long? = context.getValue(field)
				If value Is Nothing Then Return False
				Dim text As String
				Dim chrono As java.time.chrono.Chronology = context.temporal.query(java.time.temporal.TemporalQueries.chronology())
				If chrono Is Nothing OrElse chrono Is java.time.chrono.IsoChronology.INSTANCE Then
					text = provider.getText(field, value, textStyle, context.locale)
				Else
					text = provider.getText(chrono, field, value, textStyle, context.locale)
				End If
				If text Is Nothing Then Return numberPrinterParser().format(context, buf)
				buf.append(text)
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence parseText, Integer position)
				Dim length As Integer = parseText.length()
				If position < 0 OrElse position > length Then Throw New IndexOutOfBoundsException
				Dim style As TextStyle = (If(context.strict, textStyle, Nothing))
				Dim chrono As java.time.chrono.Chronology = context.effectiveChronology
				Dim it As IEnumerator(Of KeyValuePair(Of String, Long?))
				If chrono Is Nothing OrElse chrono Is java.time.chrono.IsoChronology.INSTANCE Then
					it = provider.getTextIterator(field, style, context.locale)
				Else
					it = provider.getTextIterator(chrono, field, style, context.locale)
				End If
				If it IsNot Nothing Then
					Do While it.MoveNext()
						Dim entry As KeyValuePair(Of String, Long?) = it.Current
						Dim itText As String = entry.Key
						If context.subSequenceEquals(itText, 0, parseText, position, itText.length()) Then Return context.parsedFieldeld(field, entry.Value, position, position + itText.length())
					Loop
					If context.strict Then Return Not position
				End If
				Return numberPrinterParser().parse(context, parseText, position)

			''' <summary>
			''' Create and cache a number printer parser. </summary>
			''' <returns> the number printer parser for this field, not null </returns>
			private NumberPrinterParser numberPrinterParser()
				If numberPrinterParser Is Nothing Then numberPrinterParser = New NumberPrinterParser(field, 1, 19, SignStyle.NORMAL)
				Return numberPrinterParser

			public String ToString()
				If textStyle = TextStyle.FULL Then Return "Text(" & field & ")"
				Return "Text(" & field & "," & textStyle & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses an ISO-8601 instant.
		''' </summary>
		static final class InstantPrinterParser implements DateTimePrinterParser
			' days in a 400 year cycle = 146097
			' days in a 10,000 year cycle = 146097 * 25
			' seconds per day = 86400
			private static final Long SECONDS_PER_10000_YEARS = 146097L * 25L * 86400L
			private static final Long SECONDS_0000_TO_1970 = ((146097L * 5L) - (30L * 365L + 7L)) * 86400L
			private final Integer fractionalDigits

			InstantPrinterParser(Integer fractionalDigits)
				Me.fractionalDigits = fractionalDigits

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				' use INSTANT_SECONDS, thus this code is not bound by Instant.MAX
				Dim inSecs As Long? = context.getValue(INSTANT_SECONDS)
				Dim inNanos As Long? = Nothing
				If context.temporal.isSupported(NANO_OF_SECOND) Then inNanos = context.temporal.getLong(NANO_OF_SECOND)
				If inSecs Is Nothing Then Return False
				Dim inSec As Long = inSecs
				Dim inNano As Integer = NANO_OF_SECOND.checkValidIntValue(If(inNanos IsNot Nothing, inNanos, 0))
				' format mostly using LocalDateTime.toString
				If inSec >= -SECONDS_0000_TO_1970 Then
					' current era
					Dim zeroSecs As Long = inSec - SECONDS_PER_10000_YEARS + SECONDS_0000_TO_1970
					Dim hi As Long = System.Math.floorDiv(zeroSecs, SECONDS_PER_10000_YEARS) + 1
					Dim lo As Long = System.Math.floorMod(zeroSecs, SECONDS_PER_10000_YEARS)
					Dim ldt As java.time.LocalDateTime = java.time.LocalDateTime.ofEpochSecond(lo - SECONDS_0000_TO_1970, 0, java.time.ZoneOffset.UTC)
					If hi > 0 Then buf.append("+"c).append(hi)
					buf.append(ldt)
					If ldt.second = 0 Then buf.append(":00")
				Else
					' before current era
					Dim zeroSecs As Long = inSec + SECONDS_0000_TO_1970
					Dim hi As Long = zeroSecs / SECONDS_PER_10000_YEARS
					Dim lo As Long = zeroSecs Mod SECONDS_PER_10000_YEARS
					Dim ldt As java.time.LocalDateTime = java.time.LocalDateTime.ofEpochSecond(lo - SECONDS_0000_TO_1970, 0, java.time.ZoneOffset.UTC)
					Dim pos As Integer = buf.length()
					buf.append(ldt)
					If ldt.second = 0 Then buf.append(":00")
					If hi < 0 Then
						If ldt.year = -10000 Then
							buf.replace(pos, pos + 2, Convert.ToString(hi - 1))
						ElseIf lo = 0 Then
							buf.insert(pos, hi)
						Else
							buf.insert(pos + 1, System.Math.Abs(hi))
						End If
					End If
				End If
				' add fraction
				If (fractionalDigits < 0 AndAlso inNano > 0) OrElse fractionalDigits > 0 Then
					buf.append("."c)
					Dim div As Integer = 100000000
					Dim i As Integer = 0
					Do While ((fractionalDigits = -1 AndAlso inNano > 0) OrElse (fractionalDigits = -2 AndAlso (inNano > 0 OrElse (i Mod 3) <> 0)) OrElse i < fractionalDigits)
						Dim digit As Integer = inNano \ div
						buf.append(ChrW(digit + AscW("0"c)))
						inNano = inNano - (digit * div)
						div = div \ 10
						i += 1
					Loop
				End If
				buf.append("Z"c)
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				' new context to avoid overwriting fields like year/month/day
				Dim minDigits As Integer = (If(fractionalDigits < 0, 0, fractionalDigits))
				Dim maxDigits As Integer = (If(fractionalDigits < 0, 9, fractionalDigits))
				Dim parser As CompositePrinterParser = (New DateTimeFormatterBuilder).append(DateTimeFormatter.ISO_LOCAL_DATE).appendLiteral("T"c).appendValue(HOUR_OF_DAY, 2).appendLiteral(":"c).appendValue(MINUTE_OF_HOUR, 2).appendLiteral(":"c).appendValue(SECOND_OF_MINUTE, 2).appendFraction(NANO_OF_SECOND, minDigits, maxDigits, True).appendLiteral("Z"c).toFormatter().toPrinterParser(False)
				Dim newContext As DateTimeParseContext = context.copy()
				Dim pos As Integer = parser.parse(newContext, text, position)
				If pos < 0 Then Return pos
				' parser restricts most fields to 2 digits, so definitely int
				' correctly parsed nano is also guaranteed to be valid
				Dim yearParsed As Long = newContext.getParsed(YEAR)
				Dim month As Integer = newContext.getParsed(MONTH_OF_YEAR)
				Dim day As Integer = newContext.getParsed(DAY_OF_MONTH)
				Dim hour As Integer = newContext.getParsed(HOUR_OF_DAY)
				Dim min As Integer = newContext.getParsed(MINUTE_OF_HOUR)
				Dim secVal As Long? = newContext.getParsed(SECOND_OF_MINUTE)
				Dim nanoVal As Long? = newContext.getParsed(NANO_OF_SECOND)
				Dim sec As Integer = (If(secVal IsNot Nothing, secVal, 0))
				Dim nano As Integer = (If(nanoVal IsNot Nothing, nanoVal, 0))
				Dim days As Integer = 0
				If hour = 24 AndAlso min = 0 AndAlso sec = 0 AndAlso nano = 0 Then
					hour = 0
					days = 1
				ElseIf hour = 23 AndAlso min = 59 AndAlso sec = 60 Then
					context.parsedLeapSecondond()
					sec = 59
				End If
				Dim year_Renamed As Integer = CInt(yearParsed) Mod 10000
				Dim instantSecs As Long
				Try
					Dim ldt As java.time.LocalDateTime = java.time.LocalDateTime.of(year_Renamed, month, day, hour, min, sec, 0).plusDays(days)
					instantSecs = ldt.toEpochSecond(java.time.ZoneOffset.UTC)
					instantSecs += System.Math.multiplyExact(yearParsed \ 10_000L, SECONDS_PER_10000_YEARS)
				Catch ex As RuntimeException
					Return Not position
				End Try
				Dim successPos As Integer = pos
				successPos = context.parsedFieldeld(INSTANT_SECONDS, instantSecs, position, successPos)
				Return context.parsedFieldeld(NANO_OF_SECOND, nano, position, successPos)

			public String ToString()
				Return "Instant()"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses an offset ID.
		''' </summary>
		static final class OffsetIdPrinterParser implements DateTimePrinterParser
			static final String() PATTERNS = New String() { "+HH", "+HHmm", "+HH:mm", "+HHMM", "+HH:MM", "+HHMMss", "+HH:MM:ss", "+HHMMSS", "+HH:MM:SS" }
			static final OffsetIdPrinterParser INSTANCE_ID_Z = New OffsetIdPrinterParser("+HH:MM:ss", "Z")
			static final OffsetIdPrinterParser INSTANCE_ID_ZERO = New OffsetIdPrinterParser("+HH:MM:ss", "0")

			private final String noOffsetText
			private final Integer type

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="pattern">  the pattern </param>
			''' <param name="noOffsetText">  the text to use for UTC, not null </param>
			OffsetIdPrinterParser(String pattern, String noOffsetText)
				java.util.Objects.requireNonNull(pattern, "pattern")
				java.util.Objects.requireNonNull(noOffsetText, "noOffsetText")
				Me.type = checkPattern(pattern)
				Me.noOffsetText = noOffsetText

			private Integer checkPattern(String pattern)
				For i As Integer = 0 To PATTERNS.length - 1
					If PATTERNS(i).Equals(pattern) Then Return i
				Next i
				Throw New IllegalArgumentException("Invalid zone offset pattern: " & pattern)

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim offsetSecs As Long? = context.getValue(OFFSET_SECONDS)
				If offsetSecs Is Nothing Then Return False
				Dim totalSecs As Integer = System.Math.toIntExact(offsetSecs)
				If totalSecs = 0 Then
					buf.append(noOffsetText)
				Else
					Dim absHours As Integer = System.Math.Abs((totalSecs \ 3600) Mod 100) ' anything larger than 99 silently dropped
					Dim absMinutes As Integer = System.Math.Abs((totalSecs \ 60) Mod 60)
					Dim absSeconds As Integer = System.Math.Abs(totalSecs Mod 60)
					Dim bufPos As Integer = buf.length()
					Dim output As Integer = absHours
					buf.append(If(totalSecs < 0, "-", "+")).append(ChrW(absHours \ 10 + AscW("0"c))).append(ChrW(absHours Mod 10 + AscW("0"c)))
					If type >= 3 OrElse (type >= 1 AndAlso absMinutes > 0) Then
						buf.append(If((type Mod 2) = 0, ":", "")).append(ChrW(absMinutes \ 10 + AscW("0"c))).append(ChrW(absMinutes Mod 10 + AscW("0"c)))
						output += absMinutes
						If type >= 7 OrElse (type >= 5 AndAlso absSeconds > 0) Then
							buf.append(If((type Mod 2) = 0, ":", "")).append(ChrW(absSeconds \ 10 + AscW("0"c))).append(ChrW(absSeconds Mod 10 + AscW("0"c)))
							output += absSeconds
						End If
					End If
					If output = 0 Then
						buf.length = bufPos
						buf.append(noOffsetText)
					End If
				End If
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim length As Integer = text.length()
				Dim noOffsetLen As Integer = noOffsetText.length()
				If noOffsetLen = 0 Then
					If position = length Then Return context.parsedFieldeld(OFFSET_SECONDS, 0, position, position)
				Else
					If position = length Then Return Not position
					If context.subSequenceEquals(text, position, noOffsetText, 0, noOffsetLen) Then Return context.parsedFieldeld(OFFSET_SECONDS, 0, position, position + noOffsetLen)
				End If

				' parse normal plus/minus offset
				Dim sign As Char = text.Chars(position) ' IOOBE if invalid position
				If sign = "+"c OrElse sign = "-"c Then
					' starts
					Dim negative As Integer = (If(sign = "-"c, -1, 1))
					Dim array As Integer() = New Integer(3){}
					array(0) = position + 1
					If (parseNumber(array, 1, text, True) OrElse parseNumber(array, 2, text, type >=3) OrElse parseNumber(array, 3, text, False)) = False Then
						' success
						Dim offsetSecs As Long = negative * (array(1) * 3600L + array(2) * 60L + array(3))
						Return context.parsedFieldeld(OFFSET_SECONDS, offsetSecs, position, array(0))
					End If
				End If
				' handle special case of empty no offset text
				If noOffsetLen = 0 Then Return context.parsedFieldeld(OFFSET_SECONDS, 0, position, position + noOffsetLen)
				Return Not position

			''' <summary>
			''' Parse a two digit zero-prefixed number.
			''' </summary>
			''' <param name="array">  the array of parsed data, 0=pos,1=hours,2=mins,3=secs, not null </param>
			''' <param name="arrayIndex">  the index to parse the value into </param>
			''' <param name="parseText">  the offset ID, not null </param>
			''' <param name="required">  whether this number is required </param>
			''' <returns> true if an error occurred </returns>
			private Boolean parseNumber(Integer() array, Integer arrayIndex, CharSequence parseText, Boolean required)
				If (type + 3) / 2 < arrayIndex Then Return False ' ignore seconds/minutes
				Dim pos As Integer = array(0)
				If (type Mod 2) = 0 AndAlso arrayIndex > 1 Then
					If pos + 1 > parseText.length() OrElse parseText.Chars(pos) <> ":"c Then Return required
					pos += 1
				End If
				If pos + 2 > parseText.length() Then Return required
				Dim ch1 As Char = parseText.Chars(pos)
				pos += 1
				Dim ch2 As Char = parseText.Chars(pos)
				pos += 1
				If ch1 < "0"c OrElse ch1 > "9"c OrElse ch2 < "0"c OrElse ch2 > "9"c Then Return required
				Dim value As Integer = (AscW(ch1) - 48) * 10 + (AscW(ch2) - 48)
				If value < 0 OrElse value > 59 Then Return required
				array(arrayIndex) = value
				array(0) = pos
				Return False

			public String ToString()
				Dim converted As String = noOffsetText.replace("'", "''")
				Return "Offset(" & PATTERNS(type) & ",'" & converted & "')"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses an offset ID.
		''' </summary>
		static final class LocalizedOffsetIdPrinterParser implements DateTimePrinterParser
			private final TextStyle style

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="style">  the style, not null </param>
			LocalizedOffsetIdPrinterParser(TextStyle style)
				Me.style = style

			private static StringBuilder appendHMS(StringBuilder buf, Integer t)
				Return buf.append(ChrW(t / 10 + AscW("0"c))).append(ChrW(t Mod 10 + AscW("0"c)))

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim offsetSecs As Long? = context.getValue(OFFSET_SECONDS)
				If offsetSecs Is Nothing Then Return False
				Dim gmtText As String = "GMT" ' TODO: get localized version of 'GMT'
				If gmtText IsNot Nothing Then buf.append(gmtText)
				Dim totalSecs As Integer = System.Math.toIntExact(offsetSecs)
				If totalSecs <> 0 Then
					Dim absHours As Integer = System.Math.Abs((totalSecs \ 3600) Mod 100) ' anything larger than 99 silently dropped
					Dim absMinutes As Integer = System.Math.Abs((totalSecs \ 60) Mod 60)
					Dim absSeconds As Integer = System.Math.Abs(totalSecs Mod 60)
					buf.append(If(totalSecs < 0, "-", "+"))
					If style = TextStyle.FULL Then
						appendHMS(buf, absHours)
						buf.append(":"c)
						appendHMS(buf, absMinutes)
						If absSeconds <> 0 Then
						   buf.append(":"c)
						   appendHMS(buf, absSeconds)
						End If
					Else
						If absHours >= 10 Then buf.append(ChrW(absHours \ 10 + AscW("0"c)))
						buf.append(ChrW(absHours Mod 10 + AscW("0"c)))
						If absMinutes <> 0 OrElse absSeconds <> 0 Then
							buf.append(":"c)
							appendHMS(buf, absMinutes)
							If absSeconds <> 0 Then
								buf.append(":"c)
								appendHMS(buf, absSeconds)
							End If
						End If
					End If
				End If
				Return True

			Integer getDigit(CharSequence text, Integer position)
				Dim c As Char = text.Chars(position)
				If c < "0"c OrElse c > "9"c Then Return -1
				Return AscW(c) - AscW("0"c)

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim pos As Integer = position
				Dim [end] As Integer = pos + text.length()
				Dim gmtText As String = "GMT" ' TODO: get localized version of 'GMT'
				If gmtText IsNot Nothing Then
					If Not context.subSequenceEquals(text, pos, gmtText, 0, gmtText.length()) Then Return Not position
					pos += gmtText.length()
				End If
				' parse normal plus/minus offset
				Dim negative As Integer = 0
				If pos = [end] Then Return context.parsedFieldeld(OFFSET_SECONDS, 0, position, pos)
				Dim sign As Char = text.Chars(pos) ' IOOBE if invalid position
				If sign = "+"c Then
					negative = 1
				ElseIf sign = "-"c Then
					negative = -1
				Else
					Return context.parsedFieldeld(OFFSET_SECONDS, 0, position, pos)
				End If
				pos += 1
				Dim h As Integer = 0
				Dim m As Integer = 0
				Dim s As Integer = 0
				If style = TextStyle.FULL Then
					Dim h1 As Integer = getDigit(text, pos)
					pos += 1
					Dim h2 As Integer = getDigit(text, pos)
					pos += 1
					Dim tempVar As Boolean = h1 < 0 OrElse h2 < 0 OrElse text.Chars(pos) <> ":"c
					pos += 1
					If tempVar Then Return Not position
					h = h1 * 10 + h2
					Dim m1 As Integer = getDigit(text, pos)
					pos += 1
					Dim m2 As Integer = getDigit(text, pos)
					pos += 1
					If m1 < 0 OrElse m2 < 0 Then Return Not position
					m = m1 * 10 + m2
					If pos + 2 < [end] AndAlso text.Chars(pos) = ":"c Then
						Dim s1 As Integer = getDigit(text, pos + 1)
						Dim s2 As Integer = getDigit(text, pos + 2)
						If s1 >= 0 AndAlso s2 >= 0 Then
							s = s1 * 10 + s2
							pos += 3
						End If
					End If
				Else
					h = getDigit(text, pos)
					pos += 1
					If h < 0 Then Return Not position
					If pos < [end] Then
						Dim h2 As Integer = getDigit(text, pos)
						If h2 >=0 Then
							h = h * 10 + h2
							pos += 1
						End If
						If pos + 2 < [end] AndAlso text.Chars(pos) = ":"c Then
							If pos + 2 < [end] AndAlso text.Chars(pos) = ":"c Then
								Dim m1 As Integer = getDigit(text, pos + 1)
								Dim m2 As Integer = getDigit(text, pos + 2)
								If m1 >= 0 AndAlso m2 >= 0 Then
									m = m1 * 10 + m2
									pos += 3
									If pos + 2 < [end] AndAlso text.Chars(pos) = ":"c Then
										Dim s1 As Integer = getDigit(text, pos + 1)
										Dim s2 As Integer = getDigit(text, pos + 2)
										If s1 >= 0 AndAlso s2 >= 0 Then
											s = s1 * 10 + s2
											pos += 3
										End If
									End If
								End If
							End If
						End If
					End If
				End If
				Dim offsetSecs As Long = negative * (h * 3600L + m * 60L + s)
				Return context.parsedFieldeld(OFFSET_SECONDS, offsetSecs, position, pos)

			public String ToString()
				Return "LocalizedOffset(" & style & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a zone ID.
		''' </summary>
		static final class ZoneTextPrinterParser extends ZoneIdPrinterParser

			''' <summary>
			''' The text style to output. </summary>
			private final TextStyle textStyle

			''' <summary>
			''' The preferred zoneid map </summary>
			private java.util.Set(Of String) preferredZones

			ZoneTextPrinterParser(TextStyle textStyle, java.util.Set(Of java.time.ZoneId) preferredZones)
				MyBase(java.time.temporal.TemporalQueries.zone(), "ZoneText(" & textStyle & ")")
				Me.textStyle = java.util.Objects.requireNonNull(textStyle, "textStyle")
				If preferredZones IsNot Nothing AndAlso preferredZones.size() <> 0 Then
					Me.preferredZones = New HashSet(Of )
					For Each id As java.time.ZoneId In preferredZones
						Me.preferredZones.add(id.id)
					Next id
				End If

			private static final Integer STD = 0
			private static final Integer DST = 1
			private static final Integer GENERIC = 2
			private static final IDictionary(Of String, SoftReference(Of IDictionary(Of java.util.Locale, String()))) cache = New ConcurrentDictionary(Of )

			private String getDisplayName(String id, Integer type, java.util.Locale locale)
				If textStyle = TextStyle.NARROW Then Return Nothing
				Dim names As String()
				Dim ref As SoftReference(Of IDictionary(Of java.util.Locale, String())) = cache.get(id)
				Dim perLocale As IDictionary(Of java.util.Locale, String()) = Nothing
				perLocale = ref.get()
				names = perLocale(locale)
				If ref Is Nothing OrElse perLocale Is Nothing OrElse names Is Nothing Then
					names = sun.util.locale.provider.TimeZoneNameUtility.retrieveDisplayNames(id, locale)
					If names Is Nothing Then Return Nothing
					names = java.util.Arrays.copyOfRange(names, 0, 7)
					names(5) = sun.util.locale.provider.TimeZoneNameUtility.retrieveGenericDisplayName(id, java.util.TimeZone.LONG, locale)
					If names(5) Is Nothing Then names(5) = names(0) ' use the id
					names(6) = sun.util.locale.provider.TimeZoneNameUtility.retrieveGenericDisplayName(id, java.util.TimeZone.SHORT, locale)
					If names(6) Is Nothing Then names(6) = names(0)
					If perLocale Is Nothing Then perLocale = New ConcurrentDictionary(Of )
					perLocale(locale) = names
					cache.put(id, New SoftReference(Of )(perLocale))
				End If
				Select Case type
				Case STD
					Return names(textStyle.zoneNameStyleIndex() + 1)
				Case DST
					Return names(textStyle.zoneNameStyleIndex() + 3)
				End Select
				Return names(textStyle.zoneNameStyleIndex() + 5)

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim zone As java.time.ZoneId = context.getValue(java.time.temporal.TemporalQueries.zoneId())
				If zone Is Nothing Then Return False
				Dim zname As String = zone.id
				If Not(TypeOf zone Is java.time.ZoneOffset) Then
					Dim dt As java.time.temporal.TemporalAccessor = context.temporal
					Dim name As String = getDisplayName(zname,If(dt.isSupported(java.time.temporal.ChronoField.INSTANT_SECONDS), (If(zone.rules.isDaylightSavings(java.time.Instant.from(dt)), DST, STD)), GENERIC), context.locale)
					If name IsNot Nothing Then zname = name
				End If
				buf.append(zname)
				Return True

			' cache per instance for now
			private final IDictionary(Of java.util.Locale, KeyValuePair(Of Integer?, SoftReference(Of PrefixTree))) cachedTree = New Dictionary(Of )
			private final IDictionary(Of java.util.Locale, KeyValuePair(Of Integer?, SoftReference(Of PrefixTree))) cachedTreeCI = New Dictionary(Of )

			protected PrefixTree getTree(DateTimeParseContext context)
				If textStyle = TextStyle.NARROW Then Return MyBase.getTree(context)
				Dim locale As java.util.Locale = context.locale
				Dim isCaseSensitive As Boolean = context.caseSensitive
				Dim regionIds As java.util.Set(Of String) = java.time.zone.ZoneRulesProvider.availableZoneIds
				Dim regionIdsSize As Integer = regionIds.size()

				Dim cached As IDictionary(Of java.util.Locale, KeyValuePair(Of Integer?, SoftReference(Of PrefixTree))) = If(isCaseSensitive, cachedTree, cachedTreeCI)

				Dim entry As KeyValuePair(Of Integer?, SoftReference(Of PrefixTree)) = Nothing
				Dim tree As PrefixTree = Nothing
				Dim zoneStrings As String()() = Nothing
				entry = cached(locale)
				tree = entry.Value.get()
				If entry Is Nothing OrElse (entry.Key <> regionIdsSize OrElse tree Is Nothing) Then
					tree = PrefixTree.newTree(context)
					zoneStrings = sun.util.locale.provider.TimeZoneNameUtility.getZoneStrings(locale)
					For Each names As String() In zoneStrings
						Dim zid As String = names(0)
						If Not regionIds.contains(zid) Then Continue For
						tree.add(zid, zid) ' don't convert zid -> metazone
						zid = ZoneName.toZid(zid, locale)
						Dim i As Integer = If(textStyle = TextStyle.FULL, 1, 2)
						Do While i < names.Length
							tree.add(names(i), zid)
							i += 2
						Loop
					Next names
					' if we have a set of preferred zones, need a copy and
					' add the preferred zones again to overwrite
					If preferredZones IsNot Nothing Then
						For Each names As String() In zoneStrings
							Dim zid As String = names(0)
							If (Not preferredZones.contains(zid)) OrElse (Not regionIds.contains(zid)) Then Continue For
							Dim i As Integer = If(textStyle = TextStyle.FULL, 1, 2)
							Do While i < names.Length
								tree.add(names(i), zid)
								i += 2
							Loop
						Next names
					End If
					cached(locale) = New java.util.AbstractMap.SimpleImmutableEntry(Of )(regionIdsSize, New SoftReference(Of )(tree))
				End If
				Return tree

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a zone ID.
		''' </summary>
		static class ZoneIdPrinterParser implements DateTimePrinterParser
			private final java.time.temporal.TemporalQuery(Of java.time.ZoneId) query
			private final String description

			ZoneIdPrinterParser(java.time.temporal.TemporalQuery(Of java.time.ZoneId) query, String description)
				Me.query = query
				Me.description = description

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim zone As java.time.ZoneId = context.getValue(query)
				If zone Is Nothing Then Return False
				buf.append(zone.id)
				Return True

			''' <summary>
			''' The cached tree to speed up parsing.
			''' </summary>
			private static volatile KeyValuePair(Of Integer?, PrefixTree) cachedPrefixTree
			private static volatile KeyValuePair(Of Integer?, PrefixTree) cachedPrefixTreeCI

			protected PrefixTree getTree(DateTimeParseContext context)
				' prepare parse tree
				Dim regionIds As java.util.Set(Of String) = java.time.zone.ZoneRulesProvider.availableZoneIds
				Dim regionIdsSize As Integer = regionIds.size()
				Dim cached As KeyValuePair(Of Integer?, PrefixTree) = If(context.caseSensitive, cachedPrefixTree, cachedPrefixTreeCI)
				If cached Is Nothing OrElse cached.Key <> regionIdsSize Then
					SyncLock Me
						cached = If(context.caseSensitive, cachedPrefixTree, cachedPrefixTreeCI)
						If cached Is Nothing OrElse cached.Key <> regionIdsSize Then
							cached = New java.util.AbstractMap.SimpleImmutableEntry(Of )(regionIdsSize, PrefixTree.newTree(regionIds, context))
							If context.caseSensitive Then
								cachedPrefixTree = cached
							Else
								cachedPrefixTreeCI = cached
							End If
						End If
					End SyncLock
				End If
				Return cached.Value

			''' <summary>
			''' This implementation looks for the longest matching string.
			''' For example, parsing Etc/GMT-2 will return Etc/GMC-2 rather than just
			''' Etc/GMC although both are valid.
			''' </summary>
			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim length As Integer = text.length()
				If position > length Then Throw New IndexOutOfBoundsException
				If position = length Then Return Not position

				' handle fixed time-zone IDs
				Dim nextChar As Char = text.Chars(position)
				If nextChar = "+"c OrElse nextChar = "-"c Then
					Return parseOffsetBased(context, text, position, position, OffsetIdPrinterParser.INSTANCE_ID_Z)
				ElseIf length >= position + 2 Then
					Dim nextNextChar As Char = text.Chars(position + 1)
					If context.charEquals(nextChar, "U"c) AndAlso context.charEquals(nextNextChar, "T"c) Then
						If length >= position + 3 AndAlso context.charEquals(text.Chars(position + 2), "C"c) Then Return parseOffsetBased(context, text, position, position + 3, OffsetIdPrinterParser.INSTANCE_ID_ZERO)
						Return parseOffsetBased(context, text, position, position + 2, OffsetIdPrinterParser.INSTANCE_ID_ZERO)
					ElseIf context.charEquals(nextChar, "G"c) AndAlso length >= position + 3 AndAlso context.charEquals(nextNextChar, "M"c) AndAlso context.charEquals(text.Chars(position + 2), "T"c) Then
						Return parseOffsetBased(context, text, position, position + 3, OffsetIdPrinterParser.INSTANCE_ID_ZERO)
					End If
				End If

				' parse
				Dim tree As PrefixTree = getTree(context)
				Dim ppos As New java.text.ParsePosition(position)
				Dim parsedZoneId As String = tree.match(text, ppos)
				If parsedZoneId Is Nothing Then
					If context.charEquals(nextChar, "Z"c) Then
						context.parsed = java.time.ZoneOffset.UTC
						Return position + 1
					End If
					Return Not position
				End If
				context.parsed = java.time.ZoneId.of(parsedZoneId)
				Return ppos.index

			''' <summary>
			''' Parse an offset following a prefix and set the ZoneId if it is valid.
			''' To matching the parsing of ZoneId.of the values are not normalized
			''' to ZoneOffsets.
			''' </summary>
			''' <param name="context"> the parse context </param>
			''' <param name="text"> the input text </param>
			''' <param name="prefixPos"> start of the prefix </param>
			''' <param name="position"> start of text after the prefix </param>
			''' <param name="parser"> parser for the value after the prefix </param>
			''' <returns> the position after the parse </returns>
			private Integer parseOffsetBased(DateTimeParseContext context, CharSequence text, Integer prefixPos, Integer position, OffsetIdPrinterParser parser)
				Dim prefix As String = text.ToString().Substring(prefixPos, position - prefixPos).ToUpper()
				If position >= text.length() Then
					context.parsed = java.time.ZoneId.of(prefix)
					Return position
				End If

				' '0' or 'Z' after prefix is not part of a valid ZoneId; use bare prefix
				If text.Chars(position) = "0"c OrElse context.charEquals(text.Chars(position), "Z"c) Then
					context.parsed = java.time.ZoneId.of(prefix)
					Return position
				End If

				Dim newContext As DateTimeParseContext = context.copy()
				Dim endPos As Integer = parser.parse(newContext, text, position)
				Try
					If endPos < 0 Then
						If parser Is OffsetIdPrinterParser.INSTANCE_ID_Z Then Return Not prefixPos
						context.parsed = java.time.ZoneId.of(prefix)
						Return position
					End If
					Dim offset As Integer = CInt(Fix(newContext.getParsed(OFFSET_SECONDS)))
					Dim zoneOffset_Renamed As java.time.ZoneOffset = java.time.ZoneOffset.ofTotalSeconds(offset)
					context.parsed = java.time.ZoneId.ofOffset(prefix, zoneOffset_Renamed)
					Return endPos
				Catch dte As java.time.DateTimeException
					Return Not prefixPos
				End Try

			public String ToString()
				Return description

		'-----------------------------------------------------------------------
		''' <summary>
		''' A String based prefix tree for parsing time-zone names.
		''' </summary>
		static class PrefixTree
			protected String key
			protected String value
			protected Char c0 ' performance optimization to avoid the
								  ' boundary check cost of key.charat(0)
			protected PrefixTree child
			protected PrefixTree sibling

			private PrefixTree(String k, String v, PrefixTree child)
				Me.key = k
				Me.value = v
				Me.child = child
				If k.length() = 0 Then
					c0 = &Hffff
				Else
					c0 = key.Chars(0)
				End If

			''' <summary>
			''' Creates a new prefix parsing tree based on parse context.
			''' </summary>
			''' <param name="context">  the parse context </param>
			''' <returns> the tree, not null </returns>
			Public Shared PrefixTree newTree(DateTimeParseContext context)
				'if (!context.isStrict()) {
				'    return new LENIENT("", null, null);
				'}
				If context.caseSensitive Then Return New PrefixTree("", Nothing, Nothing)
				Return New CI("", Nothing, Nothing)

			''' <summary>
			''' Creates a new prefix parsing tree.
			''' </summary>
			''' <param name="keys">  a set of strings to build the prefix parsing tree, not null </param>
			''' <param name="context">  the parse context </param>
			''' <returns> the tree, not null </returns>
			Public Shared PrefixTree newTree(java.util.Set(Of String) keys, DateTimeParseContext context)
				Dim tree As PrefixTree = newTree(context)
				For Each k As String In keys
					tree.add0(k, k)
				Next k
				Return tree

			''' <summary>
			''' Clone a copy of this tree
			''' </summary>
			public PrefixTree copyTree()
				Dim copy As New PrefixTree(key, value, Nothing)
				If child IsNot Nothing Then copy.child = child.copyTree()
				If sibling IsNot Nothing Then copy.sibling = sibling.copyTree()
				Return copy


			''' <summary>
			''' Adds a pair of {key, value} into the prefix tree.
			''' </summary>
			''' <param name="k">  the key, not null </param>
			''' <param name="v">  the value, not null </param>
			''' <returns>  true if the pair is added successfully </returns>
			public Boolean add(String k, String v)
				Return add0(k, v)

			private Boolean add0(String k, String v)
				k = toKey(k)
				Dim prefixLen As Integer = prefixLength(k)
				If prefixLen = key.length() Then
					If prefixLen < k.length() Then ' down the tree
						Dim subKey As String = k.Substring(prefixLen)
						Dim c As PrefixTree = child
						Do While c IsNot Nothing
							If isEqual(c.c0, subKey.Chars(0)) Then Return c.add0(subKey, v)
							c = c.sibling
						Loop
						' add the node as the child of the current node
						c = newNode(subKey, v, Nothing)
						c.sibling = child
						child = c
						Return True
					End If
					' have an existing <key, value> already, overwrite it
					' if (value != null) {
					'    return false;
					'}
					value = v
					Return True
				End If
				' split the existing node
				Dim n1 As PrefixTree = newNode(key.Substring(prefixLen), value, child)
				key = k.Substring(0, prefixLen)
				child = n1
				If prefixLen < k.length() Then
					Dim n2 As PrefixTree = newNode(k.Substring(prefixLen), v, Nothing)
					child.sibling = n2
					value = Nothing
				Else
					value = v
				End If
				Return True

			''' <summary>
			''' Match text with the prefix tree.
			''' </summary>
			''' <param name="text">  the input text to parse, not null </param>
			''' <param name="off">  the offset position to start parsing at </param>
			''' <param name="end">  the end position to stop parsing </param>
			''' <returns> the resulting string, or null if no match found. </returns>
			public String match(CharSequence text, Integer off, Integer end)
				If Not prefixOf(text, off, end) Then Return Nothing
				off += key.length()
				If child IsNot Nothing AndAlso off <> end Then
					Dim c As PrefixTree = child
					Do
						If isEqual(c.c0, text.Chars(off)) Then
							Dim found As String = c.match(text, off, end)
							If found IsNot Nothing Then Return found
							Return value
						End If
						c = c.sibling
					Loop While c IsNot Nothing
				End If
				Return value

			''' <summary>
			''' Match text with the prefix tree.
			''' </summary>
			''' <param name="text">  the input text to parse, not null </param>
			''' <param name="pos">  the position to start parsing at, from 0 to the text
			'''  length. Upon return, position will be updated to the new parse
			'''  position, or unchanged, if no match found. </param>
			''' <returns> the resulting string, or null if no match found. </returns>
			public String match(CharSequence text, java.text.ParsePosition pos)
				Dim [off] As Integer = pos.index
				Dim [end] As Integer = text.length()
				If Not prefixOf(text, [off], [end]) Then Return Nothing
				[off] += key.length()
				If child IsNot Nothing AndAlso [off] <> [end] Then
					Dim c As PrefixTree = child
					Do
						If isEqual(c.c0, text.Chars([off])) Then
							pos.index = [off]
							Dim found As String = c.match(text, pos)
							If found IsNot Nothing Then Return found
							Exit Do
						End If
						c = c.sibling
					Loop While c IsNot Nothing
				End If
				pos.index = [off]
				Return value

			protected String toKey(String k)
				Return k

			protected PrefixTree newNode(String k, String v, PrefixTree child)
				Return New PrefixTree(k, v, child)

			protected Boolean isEqual(Char c1, Char c2)
				Return c1 = c2

			protected Boolean prefixOf(CharSequence text, Integer off, Integer end)
				If TypeOf text Is String Then Return CStr(text).StartsWith(key, off)
				Dim len As Integer = key.length()
				If len > end - off Then Return False
				Dim off0 As Integer = 0
				Dim tempVar2 As Boolean = len > 0
				len -= 1
				Do While tempVar2
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim tempVar3 As Boolean = Not isEqual(key.Chars(off0++), text.Chars(off++))
					If tempVar3 Then Return False
					tempVar2 = len > 0
					len -= 1
				Loop
				Return True

			private Integer prefixLength(String k)
				Dim [off] As Integer = 0
				Do While [off] < k.length() AndAlso [off] < key.length()
					If Not isEqual(k.Chars([off]), key.Chars([off])) Then Return [off]
					[off] += 1
				Loop
				Return [off]

			''' <summary>
			''' Case Insensitive prefix tree.
			''' </summary>
			private static class CI extends PrefixTree

				private CI(String k, String v, PrefixTree child)
					MyBase(k, v, child)

				protected CI newNode(String k, String v, PrefixTree child)
					Return New CI(k, v, child)

				protected Boolean isEqual(Char c1, Char c2)
					Return DateTimeParseContext.charEqualsIgnoreCase(c1, c2)

				protected Boolean prefixOf(CharSequence text, Integer off, Integer end)
					Dim len As Integer = key.length()
					If len > end - off Then Return False
					Dim off0 As Integer = 0
					Dim tempVar4 As Boolean = len > 0
					len -= 1
					Do While tempVar4
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim tempVar5 As Boolean = Not isEqual(key.Chars(off0++), text.Chars(off++))
						If tempVar5 Then Return False
						tempVar4 = len > 0
						len -= 1
					Loop
					Return True

			''' <summary>
			''' Lenient prefix tree. Case insensitive and ignores characters
			''' like space, underscore and slash.
			''' </summary>
			private static class LENIENT extends CI

				private LENIENT(String k, String v, PrefixTree child)
					MyBase(k, v, child)

				protected CI newNode(String k, String v, PrefixTree child)
					Return New LENIENT(k, v, child)

				private Boolean isLenientChar(Char c)
					Return c = " "c OrElse c = "_"c OrElse c = "/"c

				protected String toKey(String k)
					For i As Integer = 0 To k.length() - 1
						If isLenientChar(k.Chars(i)) Then
							Dim sb As New StringBuilder(k.length())
							sb.append(k, 0, i)
							i += 1
							Do While i < k.length()
								If Not isLenientChar(k.Chars(i)) Then sb.append(k.Chars(i))
								i += 1
							Loop
							Return sb.ToString()
						End If
					Next i
					Return k

				public String match(CharSequence text, java.text.ParsePosition pos)
					Dim [off] As Integer = pos.index
					Dim [end] As Integer = text.length()
					Dim len As Integer = key.length()
					Dim koff As Integer = 0
					Do While koff < len AndAlso [off] < [end]
						If isLenientChar(text.Chars([off])) Then
							[off] += 1
							Continue Do
						End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Dim tempVar6 As Boolean = Not isEqual(key.Chars(koff++), text.Chars([off]++))
						If tempVar6 Then Return Nothing
					Loop
					If koff <> len Then Return Nothing
					If child IsNot Nothing AndAlso [off] <> [end] Then
						Dim off0 As Integer = [off]
						Do While off0 < [end] AndAlso isLenientChar(text.Chars(off0))
							off0 += 1
						Loop
						If off0 < [end] Then
							Dim c As PrefixTree = child
							Do
								If isEqual(c.c0, text.Chars(off0)) Then
									pos.index = off0
									Dim found As String = c.match(text, pos)
									If found IsNot Nothing Then Return found
									Exit Do
								End If
								c = c.sibling
							Loop While c IsNot Nothing
						End If
					End If
					pos.index = [off]
					Return value

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a chronology.
		''' </summary>
		static final class ChronoPrinterParser implements DateTimePrinterParser
			''' <summary>
			''' The text style to output, null means the ID. </summary>
			private final TextStyle textStyle

			ChronoPrinterParser(TextStyle textStyle)
				' validated by caller
				Me.textStyle = textStyle

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim chrono As java.time.chrono.Chronology = context.getValue(java.time.temporal.TemporalQueries.chronology())
				If chrono Is Nothing Then Return False
				If textStyle Is Nothing Then
					buf.append(chrono.id)
				Else
					buf.append(getChronologyName(chrono, context.locale))
				End If
				Return True

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				' simple looping parser to find the chronology
				If position < 0 OrElse position > text.length() Then Throw New IndexOutOfBoundsException
				Dim chronos As java.util.Set(Of java.time.chrono.Chronology) = java.time.chrono.Chronology.availableChronologies
				Dim bestMatch As java.time.chrono.Chronology = Nothing
				Dim matchLen As Integer = -1
				For Each chrono As java.time.chrono.Chronology In chronos
					Dim name As String
					If textStyle Is Nothing Then
						name = chrono.id
					Else
						name = getChronologyName(chrono, context.locale)
					End If
					Dim nameLen As Integer = name.length()
					If nameLen > matchLen AndAlso context.subSequenceEquals(text, position, name, 0, nameLen) Then
						bestMatch = chrono
						matchLen = nameLen
					End If
				Next chrono
				If bestMatch Is Nothing Then Return Not position
				context.parsed = bestMatch
				Return position + matchLen

			''' <summary>
			''' Returns the chronology name of the given chrono in the given locale
			''' if available, or the chronology Id otherwise. The regular ResourceBundle
			''' search path is used for looking up the chronology name.
			''' </summary>
			''' <param name="chrono">  the chronology, not null </param>
			''' <param name="locale">  the locale, not null </param>
			''' <returns> the chronology name of chrono in locale, or the id if no name is available </returns>
			''' <exception cref="NullPointerException"> if chrono or locale is null </exception>
			private String getChronologyName(java.time.chrono.Chronology chrono, java.util.Locale locale)
				Dim key As String = "calendarname." & chrono.calendarType
				Dim name As String = DateTimeTextProvider.getLocalizedResource(key, locale)
				Return If(name IsNot Nothing, name, chrono.id)

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a localized pattern.
		''' </summary>
		static final class LocalizedPrinterParser implements DateTimePrinterParser
			''' <summary>
			''' Cache of formatters. </summary>
			private static final java.util.concurrent.ConcurrentMap(Of String, DateTimeFormatter) FORMATTER_CACHE = New ConcurrentDictionary(Of )(16, 0.75f, 2)

			private final FormatStyle dateStyle
			private final FormatStyle timeStyle

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="dateStyle">  the date style to use, may be null </param>
			''' <param name="timeStyle">  the time style to use, may be null </param>
			LocalizedPrinterParser(FormatStyle dateStyle, FormatStyle timeStyle)
				' validated by caller
				Me.dateStyle = dateStyle
				Me.timeStyle = timeStyle

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Dim chrono As java.time.chrono.Chronology = java.time.chrono.Chronology.from(context.temporal)
				Return formatter(context.locale, chrono).toPrinterParser(False).format(context, buf)

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Dim chrono As java.time.chrono.Chronology = context.effectiveChronology
				Return formatter(context.locale, chrono).toPrinterParser(False).parse(context, text, position)

			''' <summary>
			''' Gets the formatter to use.
			''' <p>
			''' The formatter will be the most appropriate to use for the date and time style in the locale.
			''' For example, some locales will use the month name while others will use the number.
			''' </summary>
			''' <param name="locale">  the locale to use, not null </param>
			''' <param name="chrono">  the chronology to use, not null </param>
			''' <returns> the formatter, not null </returns>
			''' <exception cref="IllegalArgumentException"> if the formatter cannot be found </exception>
			private DateTimeFormatter formatter(java.util.Locale locale, java.time.chrono.Chronology chrono)
				Dim key As String = chrono.id + AscW("|"c) + locale.ToString() & AscW("|"c) + dateStyle + timeStyle
				Dim formatter As DateTimeFormatter = FORMATTER_CACHE.get(key)
				If formatter Is Nothing Then
					Dim pattern As String = getLocalizedDateTimePattern(dateStyle, timeStyle, chrono, locale)
					formatter = (New DateTimeFormatterBuilder).appendPattern(pattern).toFormatter(locale)
					Dim old As DateTimeFormatter = FORMATTER_CACHE.putIfAbsent(key, formatter)
					If old IsNot Nothing Then formatter = old
				End If
				Return formatter

			public String ToString()
				Return "Localized(" & (If(dateStyle IsNot Nothing, dateStyle, "")) & "," & (If(timeStyle IsNot Nothing, timeStyle, "")) & ")"

		'-----------------------------------------------------------------------
		''' <summary>
		''' Prints or parses a localized pattern from a localized field.
		''' The specific formatter and parameters is not selected until the
		''' the field is to be printed or parsed.
		''' The locale is needed to select the proper WeekFields from which
		''' the field for day-of-week, week-of-month, or week-of-year is selected.
		''' </summary>
		static final class WeekBasedFieldPrinterParser implements DateTimePrinterParser
			private Char chr
			private Integer count

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="chr"> the pattern format letter that added this PrinterParser. </param>
			''' <param name="count"> the repeat count of the format letter </param>
			WeekBasedFieldPrinterParser(Char chr, Integer count)
				Me.chr = chr
				Me.count = count

			public Boolean format(DateTimePrintContext context, StringBuilder buf)
				Return printerParser(context.locale).format(context, buf)

			public Integer parse(DateTimeParseContext context, CharSequence text, Integer position)
				Return printerParser(context.locale).parse(context, text, position)

			''' <summary>
			''' Gets the printerParser to use based on the field and the locale.
			''' </summary>
			''' <param name="locale">  the locale to use, not null </param>
			''' <returns> the formatter, not null </returns>
			''' <exception cref="IllegalArgumentException"> if the formatter cannot be found </exception>
			private DateTimePrinterParser printerParser(java.util.Locale locale)
				Dim weekDef As java.time.temporal.WeekFields = java.time.temporal.WeekFields.of(locale)
				Dim field As java.time.temporal.TemporalField = Nothing
				Select Case chr
					Case "Y"c
						field = weekDef.weekBasedYear()
						If count = 2 Then
							Return New ReducedPrinterParser(field, 2, 2, 0, ReducedPrinterParser.BASE_DATE, 0)
						Else
							Return New NumberPrinterParser(field, count, 19,If(count < 4, SignStyle.NORMAL, SignStyle.EXCEEDS_PAD), -1)
						End If
					Case "e"c, "c"c
						field = weekDef.dayOfWeek()
					Case "w"c
						field = weekDef.weekOfWeekBasedYear()
					Case "W"c
						field = weekDef.weekOfMonth()
					Case Else
						Throw New IllegalStateException("unreachable")
				End Select
				Return New NumberPrinterParser(field, (If(count = 2, 2, 1)), 2, SignStyle.NOT_NEGATIVE)

			public String ToString()
				Dim sb As New StringBuilder(30)
				sb.append("Localized(")
				If chr = "Y"c Then
					If count = 1 Then
						sb.append("WeekBasedYear")
					ElseIf count = 2 Then
						sb.append("ReducedValue(WeekBasedYear,2,2,2000-01-01)")
					Else
						sb.append("WeekBasedYear,").append(count).append(",").append(19).append(",").append(If(count < 4, SignStyle.NORMAL, SignStyle.EXCEEDS_PAD))
					End If
				Else
					Select Case chr
						Case "c"c, "e"c
							sb.append("DayOfWeek")
						Case "w"c
							sb.append("WeekOfWeekBasedYear")
						Case "W"c
							sb.append("WeekOfMonth")
						Case Else
					End Select
					sb.append(",")
					sb.append(count)
				End If
				sb.append(")")
				Return sb.ToString()

		'-------------------------------------------------------------------------
		''' <summary>
		''' Length comparator.
		''' </summary>
		static final IComparer(Of String) LENGTH_SORT = New ComparatorAnonymousInnerClassHelper(Of T)
	End Class


	Private Class DateTimeTextProviderAnonymousInnerClassHelper
		Inherits DateTimeTextProvider

		Public Overrides Function getText(ByVal field As java.time.temporal.TemporalField, ByVal value As Long, ByVal style As TextStyle, ByVal locale As java.util.Locale) As String
			Return store.getText(value, style)
		End Function
		Public Overrides Function getTextIterator(ByVal field As java.time.temporal.TemporalField, ByVal style As TextStyle, ByVal locale As java.util.Locale) As IEnumerator(Of KeyValuePair(Of String, Long?))
			Return store.getTextIterator(style)
		End Function
	End Class

	Private Class ComparatorAnonymousInnerClassHelper(Of T)
		Implements IComparer(Of T)

		Public Overrides Function compare(ByVal str1 As String, ByVal str2 As String) As Integer
			Return If(str1.length() = str2.length(), str1.CompareTo(str2), str1.length() - str2.length())
		End Function
	End Class
End Namespace