Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An interpreter for printf-style format strings.  This class provides support
	''' for layout justification and alignment, common formats for numeric, string,
	''' and date/time data, and locale-specific output.  Common Java types such as
	''' {@code byte}, <seealso cref="java.math.BigDecimal BigDecimal"/>, and <seealso cref="Calendar"/>
	''' are supported.  Limited formatting customization for arbitrary user types is
	''' provided through the <seealso cref="Formattable"/> interface.
	''' 
	''' <p> Formatters are not necessarily safe for multithreaded access.  Thread
	''' safety is optional and is the responsibility of users of methods in this
	''' class.
	''' 
	''' <p> Formatted printing for the Java language is heavily inspired by C's
	''' {@code printf}.  Although the format strings are similar to C, some
	''' customizations have been made to accommodate the Java language and exploit
	''' some of its features.  Also, Java formatting is more strict than C's; for
	''' example, if a conversion is incompatible with a flag, an exception will be
	''' thrown.  In C inapplicable flags are silently ignored.  The format strings
	''' are thus intended to be recognizable to C programmers but not necessarily
	''' completely compatible with those in C.
	''' 
	''' <p> Examples of expected usage:
	''' 
	''' <blockquote><pre>
	'''   StringBuilder sb = new StringBuilder();
	'''   // Send all output to the Appendable object sb
	'''   Formatter formatter = new Formatter(sb, Locale.US);
	''' 
	'''   // Explicit argument indices may be used to re-order output.
	'''   formatter.format("%4$2s %3$2s %2$2s %1$2s", "a", "b", "c", "d")
	'''   // -&gt; " d  c  b  a"
	''' 
	'''   // Optional locale as the first argument can be used to get
	'''   // locale-specific formatting of numbers.  The precision and width can be
	'''   // given to round and align the value.
	'''   formatter.format(Locale.FRANCE, "e = %+10.4f", System.Math.E);
	'''   // -&gt; "e =    +2,7183"
	''' 
	'''   // The '(' numeric flag may be used to format negative numbers with
	'''   // parentheses rather than a minus sign.  Group separators are
	'''   // automatically inserted.
	'''   formatter.format("Amount gained or lost since last statement: $ %(,.2f",
	'''                    balanceDelta);
	'''   // -&gt; "Amount gained or lost since last statement: $ (6,217.58)"
	''' </pre></blockquote>
	''' 
	''' <p> Convenience methods for common formatting requests exist as illustrated
	''' by the following invocations:
	''' 
	''' <blockquote><pre>
	'''   // Writes a formatted string to System.out.
	'''   System.out.format("Local time: %tT", Calendar.getInstance());
	'''   // -&gt; "Local time: 13:34:18"
	''' 
	'''   // Writes formatted output to System.err.
	'''   System.err.printf("Unable to open file '%1$s': %2$s",
	'''                     fileName, exception.getMessage());
	'''   // -&gt; "Unable to open file 'food': No such file or directory"
	''' </pre></blockquote>
	''' 
	''' <p> Like C's {@code sprintf(3)}, Strings may be formatted using the static
	''' method <seealso cref="String#format(String,Object...) String.format"/>:
	''' 
	''' <blockquote><pre>
	'''   // Format a string containing a date.
	'''   import java.util.Calendar;
	'''   import java.util.GregorianCalendar;
	'''   import static java.util.Calendar.*;
	''' 
	'''   Calendar c = new GregorianCalendar(1995, MAY, 23);
	'''   String s = String.format("Duke's Birthday: %1$tb %1$te, %1$tY", c);
	'''   // -&gt; s == "Duke's Birthday: May 23, 1995"
	''' </pre></blockquote>
	''' 
	''' <h3><a name="org">Organization</a></h3>
	''' 
	''' <p> This specification is divided into two sections.  The first section, <a
	''' href="#summary">Summary</a>, covers the basic formatting concepts.  This
	''' section is intended for users who want to get started quickly and are
	''' familiar with formatted printing in other programming languages.  The second
	''' section, <a href="#detail">Details</a>, covers the specific implementation
	''' details.  It is intended for users who want more precise specification of
	''' formatting behavior.
	''' 
	''' <h3><a name="summary">Summary</a></h3>
	''' 
	''' <p> This section is intended to provide a brief overview of formatting
	''' concepts.  For precise behavioral details, refer to the <a
	''' href="#detail">Details</a> section.
	''' 
	''' <h4><a name="syntax">Format String Syntax</a></h4>
	''' 
	''' <p> Every method which produces formatted output requires a <i>format
	''' string</i> and an <i>argument list</i>.  The format string is a {@link
	''' String} which may contain fixed text and one or more embedded <i>format
	''' specifiers</i>.  Consider the following example:
	''' 
	''' <blockquote><pre>
	'''   Calendar c = ...;
	'''   String s = String.format("Duke's Birthday: %1$tm %1$te,%1$tY", c);
	''' </pre></blockquote>
	''' 
	''' This format string is the first argument to the {@code format} method.  It
	''' contains three format specifiers "{@code %1$tm}", "{@code %1$te}", and
	''' "{@code %1$tY}" which indicate how the arguments should be processed and
	''' where they should be inserted in the text.  The remaining portions of the
	''' format string are fixed text including {@code "Dukes Birthday: "} and any
	''' other spaces or punctuation.
	''' 
	''' The argument list consists of all arguments passed to the method after the
	''' format string.  In the above example, the argument list is of size one and
	''' consists of the <seealso cref="java.util.Calendar Calendar"/> object {@code c}.
	''' 
	''' <ul>
	''' 
	''' <li> The format specifiers for general, character, and numeric types have
	''' the following syntax:
	''' 
	''' <blockquote><pre>
	'''   %[argument_index$][flags][width][.precision]conversion
	''' </pre></blockquote>
	''' 
	''' <p> The optional <i>argument_index</i> is a decimal integer indicating the
	''' position of the argument in the argument list.  The first argument is
	''' referenced by "{@code 1$}", the second by "{@code 2$}", etc.
	''' 
	''' <p> The optional <i>flags</i> is a set of characters that modify the output
	''' format.  The set of valid flags depends on the conversion.
	''' 
	''' <p> The optional <i>width</i> is a positive decimal integer indicating
	''' the minimum number of characters to be written to the output.
	''' 
	''' <p> The optional <i>precision</i> is a non-negative decimal integer usually
	''' used to restrict the number of characters.  The specific behavior depends on
	''' the conversion.
	''' 
	''' <p> The required <i>conversion</i> is a character indicating how the
	''' argument should be formatted.  The set of valid conversions for a given
	''' argument depends on the argument's data type.
	''' 
	''' <li> The format specifiers for types which are used to represents dates and
	''' times have the following syntax:
	''' 
	''' <blockquote><pre>
	'''   %[argument_index$][flags][width]conversion
	''' </pre></blockquote>
	''' 
	''' <p> The optional <i>argument_index</i>, <i>flags</i> and <i>width</i> are
	''' defined as above.
	''' 
	''' <p> The required <i>conversion</i> is a two character sequence.  The first
	''' character is {@code 't'} or {@code 'T'}.  The second character indicates
	''' the format to be used.  These characters are similar to but not completely
	''' identical to those defined by GNU {@code date} and POSIX
	''' {@code strftime(3c)}.
	''' 
	''' <li> The format specifiers which do not correspond to arguments have the
	''' following syntax:
	''' 
	''' <blockquote><pre>
	'''   %[flags][width]conversion
	''' </pre></blockquote>
	''' 
	''' <p> The optional <i>flags</i> and <i>width</i> is defined as above.
	''' 
	''' <p> The required <i>conversion</i> is a character indicating content to be
	''' inserted in the output.
	''' 
	''' </ul>
	''' 
	''' <h4> Conversions </h4>
	''' 
	''' <p> Conversions are divided into the following categories:
	''' 
	''' <ol>
	''' 
	''' <li> <b>General</b> - may be applied to any argument
	''' type
	''' 
	''' <li> <b>Character</b> - may be applied to basic types which represent
	''' Unicode characters: {@code char}, <seealso cref="Character"/>, {@code byte}, {@link
	''' Byte}, {@code short}, and <seealso cref="Short"/>. This conversion may also be
	''' applied to the types {@code int} and <seealso cref="Integer"/> when {@link
	''' Character#isValidCodePoint} returns {@code true}
	''' 
	''' <li> <b>Numeric</b>
	''' 
	''' <ol>
	''' 
	''' <li> <b>Integral</b> - may be applied to Java integral types: {@code byte},
	''' <seealso cref="Byte"/>, {@code short}, <seealso cref="Short"/>, {@code int} and {@link
	''' Integer}, {@code long}, <seealso cref="Long"/>, and {@link java.math.BigInteger
	''' BigInteger} (but not {@code char} or <seealso cref="Character"/>)
	''' 
	''' <li><b>Floating Point</b> - may be applied to Java floating-point types:
	''' {@code float}, <seealso cref="Float"/>, {@code double}, <seealso cref="Double"/>, and {@link
	''' java.math.BigDecimal BigDecimal}
	''' 
	''' </ol>
	''' 
	''' <li> <b>Date/Time</b> - may be applied to Java types which are capable of
	''' encoding a date or time: {@code long}, <seealso cref="Long"/>, <seealso cref="Calendar"/>,
	''' <seealso cref="Date"/> and <seealso cref="TemporalAccessor TemporalAccessor"/>
	''' 
	''' <li> <b>Percent</b> - produces a literal {@code '%'}
	''' (<tt>'&#92;u0025'</tt>)
	''' 
	''' <li> <b>Line Separator</b> - produces the platform-specific line separator
	''' 
	''' </ol>
	''' 
	''' <p> The following table summarizes the supported conversions.  Conversions
	''' denoted by an upper-case character (i.e. {@code 'B'}, {@code 'H'},
	''' {@code 'S'}, {@code 'C'}, {@code 'X'}, {@code 'E'}, {@code 'G'},
	''' {@code 'A'}, and {@code 'T'}) are the same as those for the corresponding
	''' lower-case conversion characters except that the result is converted to
	''' upper case according to the rules of the prevailing {@link java.util.Locale
	''' Locale}.  The result is equivalent to the following invocation of {@link
	''' String#toUpperCase()}
	''' 
	''' <pre>
	'''    out.toUpperCase() </pre>
	''' 
	''' <table cellpadding=5 summary="genConv">
	''' 
	''' <tr><th valign="bottom"> Conversion
	'''     <th valign="bottom"> Argument Category
	'''     <th valign="bottom"> Description
	''' 
	''' <tr><td valign="top"> {@code 'b'}, {@code 'B'}
	'''     <td valign="top"> general
	'''     <td> If the argument <i>arg</i> is {@code null}, then the result is
	'''     "{@code false}".  If <i>arg</i> is a {@code boolean} or {@link
	'''     Boolean}, then the result is the string returned by {@link
	'''     String#valueOf(boolean) String.valueOf(arg)}.  Otherwise, the result is
	'''     "true".
	''' 
	''' <tr><td valign="top"> {@code 'h'}, {@code 'H'}
	'''     <td valign="top"> general
	'''     <td> If the argument <i>arg</i> is {@code null}, then the result is
	'''     "{@code null}".  Otherwise, the result is obtained by invoking
	'''     {@code  java.lang.[Integer].toHexString(arg.hashCode())}.
	''' 
	''' <tr><td valign="top"> {@code 's'}, {@code 'S'}
	'''     <td valign="top"> general
	'''     <td> If the argument <i>arg</i> is {@code null}, then the result is
	'''     "{@code null}".  If <i>arg</i> implements <seealso cref="Formattable"/>, then
	'''     <seealso cref="Formattable#formatTo arg.formatTo"/> is invoked. Otherwise, the
	'''     result is obtained by invoking {@code arg.toString()}.
	''' 
	''' <tr><td valign="top">{@code 'c'}, {@code 'C'}
	'''     <td valign="top"> character
	'''     <td> The result is a Unicode character
	''' 
	''' <tr><td valign="top">{@code 'd'}
	'''     <td valign="top"> integral
	'''     <td> The result is formatted as a decimal integer
	''' 
	''' <tr><td valign="top">{@code 'o'}
	'''     <td valign="top"> integral
	'''     <td> The result is formatted as an octal integer
	''' 
	''' <tr><td valign="top">{@code 'x'}, {@code 'X'}
	'''     <td valign="top"> integral
	'''     <td> The result is formatted as a hexadecimal integer
	''' 
	''' <tr><td valign="top">{@code 'e'}, {@code 'E'}
	'''     <td valign="top"> floating point
	'''     <td> The result is formatted as a decimal number in computerized
	'''     scientific notation
	''' 
	''' <tr><td valign="top">{@code 'f'}
	'''     <td valign="top"> floating point
	'''     <td> The result is formatted as a decimal number
	''' 
	''' <tr><td valign="top">{@code 'g'}, {@code 'G'}
	'''     <td valign="top"> floating point
	'''     <td> The result is formatted using computerized scientific notation or
	'''     decimal format, depending on the precision and the value after rounding.
	''' 
	''' <tr><td valign="top">{@code 'a'}, {@code 'A'}
	'''     <td valign="top"> floating point
	'''     <td> The result is formatted as a hexadecimal floating-point number with
	'''     a significand and an exponent. This conversion is <b>not</b> supported
	'''     for the {@code BigDecimal} type despite the latter's being in the
	'''     <i>floating point</i> argument category.
	''' 
	''' <tr><td valign="top">{@code 't'}, {@code 'T'}
	'''     <td valign="top"> date/time
	'''     <td> Prefix for date and time conversion characters.  See <a
	'''     href="#dt">Date/Time Conversions</a>.
	''' 
	''' <tr><td valign="top">{@code '%'}
	'''     <td valign="top"> percent
	'''     <td> The result is a literal {@code '%'} (<tt>'&#92;u0025'</tt>)
	''' 
	''' <tr><td valign="top">{@code 'n'}
	'''     <td valign="top"> line separator
	'''     <td> The result is the platform-specific line separator
	''' 
	''' </table>
	''' 
	''' <p> Any characters not explicitly defined as conversions are illegal and are
	''' reserved for future extensions.
	''' 
	''' <h4><a name="dt">Date/Time Conversions</a></h4>
	''' 
	''' <p> The following date and time conversion suffix characters are defined for
	''' the {@code 't'} and {@code 'T'} conversions.  The types are similar to but
	''' not completely identical to those defined by GNU {@code date} and POSIX
	''' {@code strftime(3c)}.  Additional conversion types are provided to access
	''' Java-specific functionality (e.g. {@code 'L'} for milliseconds within the
	''' second).
	''' 
	''' <p> The following conversion characters are used for formatting times:
	''' 
	''' <table cellpadding=5 summary="time">
	''' 
	''' <tr><td valign="top"> {@code 'H'}
	'''     <td> Hour of the day for the 24-hour clock, formatted as two digits with
	'''     a leading zero as necessary i.e. {@code 00 - 23}.
	''' 
	''' <tr><td valign="top">{@code 'I'}
	'''     <td> Hour for the 12-hour clock, formatted as two digits with a leading
	'''     zero as necessary, i.e.  {@code 01 - 12}.
	''' 
	''' <tr><td valign="top">{@code 'k'}
	'''     <td> Hour of the day for the 24-hour clock, i.e. {@code 0 - 23}.
	''' 
	''' <tr><td valign="top">{@code 'l'}
	'''     <td> Hour for the 12-hour clock, i.e. {@code 1 - 12}.
	''' 
	''' <tr><td valign="top">{@code 'M'}
	'''     <td> Minute within the hour formatted as two digits with a leading zero
	'''     as necessary, i.e.  {@code 00 - 59}.
	''' 
	''' <tr><td valign="top">{@code 'S'}
	'''     <td> Seconds within the minute, formatted as two digits with a leading
	'''     zero as necessary, i.e. {@code 00 - 60} ("{@code 60}" is a special
	'''     value required to support leap seconds).
	''' 
	''' <tr><td valign="top">{@code 'L'}
	'''     <td> Millisecond within the second formatted as three digits with
	'''     leading zeros as necessary, i.e. {@code 000 - 999}.
	''' 
	''' <tr><td valign="top">{@code 'N'}
	'''     <td> Nanosecond within the second, formatted as nine digits with leading
	'''     zeros as necessary, i.e. {@code 000000000 - 999999999}.
	''' 
	''' <tr><td valign="top">{@code 'p'}
	'''     <td> Locale-specific {@linkplain
	'''     java.text.DateFormatSymbols#getAmPmStrings morning or afternoon} marker
	'''     in lower case, e.g."{@code am}" or "{@code pm}". Use of the conversion
	'''     prefix {@code 'T'} forces this output to upper case.
	''' 
	''' <tr><td valign="top">{@code 'z'}
	'''     <td> <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC&nbsp;822</a>
	'''     style numeric time zone offset from GMT, e.g. {@code -0800}.  This
	'''     value will be adjusted as necessary for Daylight Saving Time.  For
	'''     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	'''     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	'''     instance of the Java virtual machine.
	''' 
	''' <tr><td valign="top">{@code 'Z'}
	'''     <td> A string representing the abbreviation for the time zone.  This
	'''     value will be adjusted as necessary for Daylight Saving Time.  For
	'''     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the  time zone used is
	'''     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	'''     instance of the Java virtual machine.  The Formatter's locale will
	'''     supersede the locale of the argument (if any).
	''' 
	''' <tr><td valign="top">{@code 's'}
	'''     <td> Seconds since the beginning of the epoch starting at 1 January 1970
	'''     {@code 00:00:00} UTC, i.e. {@code java.lang.[Long].MIN_VALUE/1000} to
	'''     {@code java.lang.[Long].MAX_VALUE/1000}.
	''' 
	''' <tr><td valign="top">{@code 'Q'}
	'''     <td> Milliseconds since the beginning of the epoch starting at 1 January
	'''     1970 {@code 00:00:00} UTC, i.e. {@code java.lang.[Long].MIN_VALUE} to
	'''     {@code java.lang.[Long].MAX_VALUE}.
	''' 
	''' </table>
	''' 
	''' <p> The following conversion characters are used for formatting dates:
	''' 
	''' <table cellpadding=5 summary="date">
	''' 
	''' <tr><td valign="top">{@code 'B'}
	'''     <td> Locale-specific {@link java.text.DateFormatSymbols#getMonths
	'''     full month name}, e.g. {@code "January"}, {@code "February"}.
	''' 
	''' <tr><td valign="top">{@code 'b'}
	'''     <td> Locale-specific {@linkplain
	'''     java.text.DateFormatSymbols#getShortMonths abbreviated month name},
	'''     e.g. {@code "Jan"}, {@code "Feb"}.
	''' 
	''' <tr><td valign="top">{@code 'h'}
	'''     <td> Same as {@code 'b'}.
	''' 
	''' <tr><td valign="top">{@code 'A'}
	'''     <td> Locale-specific full name of the {@linkplain
	'''     java.text.DateFormatSymbols#getWeekdays day of the week},
	'''     e.g. {@code "Sunday"}, {@code "Monday"}
	''' 
	''' <tr><td valign="top">{@code 'a'}
	'''     <td> Locale-specific short name of the {@linkplain
	'''     java.text.DateFormatSymbols#getShortWeekdays day of the week},
	'''     e.g. {@code "Sun"}, {@code "Mon"}
	''' 
	''' <tr><td valign="top">{@code 'C'}
	'''     <td> Four-digit year divided by {@code 100}, formatted as two digits
	'''     with leading zero as necessary, i.e. {@code 00 - 99}
	''' 
	''' <tr><td valign="top">{@code 'Y'}
	'''     <td> Year, formatted as at least four digits with leading zeros as
	'''     necessary, e.g. {@code 0092} equals {@code 92} CE for the Gregorian
	'''     calendar.
	''' 
	''' <tr><td valign="top">{@code 'y'}
	'''     <td> Last two digits of the year, formatted with leading zeros as
	'''     necessary, i.e. {@code 00 - 99}.
	''' 
	''' <tr><td valign="top">{@code 'j'}
	'''     <td> Day of year, formatted as three digits with leading zeros as
	'''     necessary, e.g. {@code 001 - 366} for the Gregorian calendar.
	''' 
	''' <tr><td valign="top">{@code 'm'}
	'''     <td> Month, formatted as two digits with leading zeros as necessary,
	'''     i.e. {@code 01 - 13}.
	''' 
	''' <tr><td valign="top">{@code 'd'}
	'''     <td> Day of month, formatted as two digits with leading zeros as
	'''     necessary, i.e. {@code 01 - 31}
	''' 
	''' <tr><td valign="top">{@code 'e'}
	'''     <td> Day of month, formatted as two digits, i.e. {@code 1 - 31}.
	''' 
	''' </table>
	''' 
	''' <p> The following conversion characters are used for formatting common
	''' date/time compositions.
	''' 
	''' <table cellpadding=5 summary="composites">
	''' 
	''' <tr><td valign="top">{@code 'R'}
	'''     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM"}
	''' 
	''' <tr><td valign="top">{@code 'T'}
	'''     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM:%tS"}.
	''' 
	''' <tr><td valign="top">{@code 'r'}
	'''     <td> Time formatted for the 12-hour clock as {@code "%tI:%tM:%tS %Tp"}.
	'''     The location of the morning or afternoon marker ({@code '%Tp'}) may be
	'''     locale-dependent.
	''' 
	''' <tr><td valign="top">{@code 'D'}
	'''     <td> Date formatted as {@code "%tm/%td/%ty"}.
	''' 
	''' <tr><td valign="top">{@code 'F'}
	'''     <td> <a href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a>
	'''     complete date formatted as {@code "%tY-%tm-%td"}.
	''' 
	''' <tr><td valign="top">{@code 'c'}
	'''     <td> Date and time formatted as {@code "%ta %tb %td %tT %tZ %tY"},
	'''     e.g. {@code "Sun Jul 20 16:17:00 EDT 1969"}.
	''' 
	''' </table>
	''' 
	''' <p> Any characters not explicitly defined as date/time conversion suffixes
	''' are illegal and are reserved for future extensions.
	''' 
	''' <h4> Flags </h4>
	''' 
	''' <p> The following table summarizes the supported flags.  <i>y</i> means the
	''' flag is supported for the indicated argument types.
	''' 
	''' <table cellpadding=5 summary="genConv">
	''' 
	''' <tr><th valign="bottom"> Flag <th valign="bottom"> General
	'''     <th valign="bottom"> Character <th valign="bottom"> Integral
	'''     <th valign="bottom"> Floating Point
	'''     <th valign="bottom"> Date/Time
	'''     <th valign="bottom"> Description
	''' 
	''' <tr><td> '-' <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> y
	'''     <td> The result will be left-justified.
	''' 
	''' <tr><td> '#' <td align="center" valign="top"> y<sup>1</sup>
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y<sup>3</sup>
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> -
	'''     <td> The result should use a conversion-dependent alternate form
	''' 
	''' <tr><td> '+' <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y<sup>4</sup>
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> -
	'''     <td> The result will always include a sign
	''' 
	''' <tr><td> '&nbsp;&nbsp;' <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y<sup>4</sup>
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> -
	'''     <td> The result will include a leading space for positive values
	''' 
	''' <tr><td> '0' <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> y
	'''     <td align="center" valign="top"> -
	'''     <td> The result will be zero-padded
	''' 
	''' <tr><td> ',' <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y<sup>2</sup>
	'''     <td align="center" valign="top"> y<sup>5</sup>
	'''     <td align="center" valign="top"> -
	'''     <td> The result will include locale-specific {@linkplain
	'''     java.text.DecimalFormatSymbols#getGroupingSeparator grouping separators}
	''' 
	''' <tr><td> '(' <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> -
	'''     <td align="center" valign="top"> y<sup>4</sup>
	'''     <td align="center" valign="top"> y<sup>5</sup>
	'''     <td align="center"> -
	'''     <td> The result will enclose negative numbers in parentheses
	''' 
	''' </table>
	''' 
	''' <p> <sup>1</sup> Depends on the definition of <seealso cref="Formattable"/>.
	''' 
	''' <p> <sup>2</sup> For {@code 'd'} conversion only.
	''' 
	''' <p> <sup>3</sup> For {@code 'o'}, {@code 'x'}, and {@code 'X'}
	''' conversions only.
	''' 
	''' <p> <sup>4</sup> For {@code 'd'}, {@code 'o'}, {@code 'x'}, and
	''' {@code 'X'} conversions applied to <seealso cref="java.math.BigInteger BigInteger"/>
	''' or {@code 'd'} applied to {@code byte}, <seealso cref="Byte"/>, {@code short}, {@link
	''' Short}, {@code int} and <seealso cref="Integer"/>, {@code long}, and <seealso cref="Long"/>.
	''' 
	''' <p> <sup>5</sup> For {@code 'e'}, {@code 'E'}, {@code 'f'},
	''' {@code 'g'}, and {@code 'G'} conversions only.
	''' 
	''' <p> Any characters not explicitly defined as flags are illegal and are
	''' reserved for future extensions.
	''' 
	''' <h4> Width </h4>
	''' 
	''' <p> The width is the minimum number of characters to be written to the
	''' output.  For the line separator conversion, width is not applicable; if it
	''' is provided, an exception will be thrown.
	''' 
	''' <h4> Precision </h4>
	''' 
	''' <p> For general argument types, the precision is the maximum number of
	''' characters to be written to the output.
	''' 
	''' <p> For the floating-point conversions {@code 'a'}, {@code 'A'}, {@code 'e'},
	''' {@code 'E'}, and {@code 'f'} the precision is the number of digits after the
	''' radix point.  If the conversion is {@code 'g'} or {@code 'G'}, then the
	''' precision is the total number of digits in the resulting magnitude after
	''' rounding.
	''' 
	''' <p> For character, integral, and date/time argument types and the percent
	''' and line separator conversions, the precision is not applicable; if a
	''' precision is provided, an exception will be thrown.
	''' 
	''' <h4> Argument Index </h4>
	''' 
	''' <p> The argument index is a decimal integer indicating the position of the
	''' argument in the argument list.  The first argument is referenced by
	''' "{@code 1$}", the second by "{@code 2$}", etc.
	''' 
	''' <p> Another way to reference arguments by position is to use the
	''' {@code '<'} (<tt>'&#92;u003c'</tt>) flag, which causes the argument for
	''' the previous format specifier to be re-used.  For example, the following two
	''' statements would produce identical strings:
	''' 
	''' <blockquote><pre>
	'''   Calendar c = ...;
	'''   String s1 = String.format("Duke's Birthday: %1$tm %1$te,%1$tY", c);
	''' 
	'''   String s2 = String.format("Duke's Birthday: %1$tm %&lt;te,%&lt;tY", c);
	''' </pre></blockquote>
	''' 
	''' <hr>
	''' <h3><a name="detail">Details</a></h3>
	''' 
	''' <p> This section is intended to provide behavioral details for formatting,
	''' including conditions and exceptions, supported data types, localization, and
	''' interactions between flags, conversions, and data types.  For an overview of
	''' formatting concepts, refer to the <a href="#summary">Summary</a>
	''' 
	''' <p> Any characters not explicitly defined as conversions, date/time
	''' conversion suffixes, or flags are illegal and are reserved for
	''' future extensions.  Use of such a character in a format string will
	''' cause an <seealso cref="UnknownFormatConversionException"/> or {@link
	''' UnknownFormatFlagsException} to be thrown.
	''' 
	''' <p> If the format specifier contains a width or precision with an invalid
	''' value or which is otherwise unsupported, then a {@link
	''' IllegalFormatWidthException} or <seealso cref="IllegalFormatPrecisionException"/>
	''' respectively will be thrown.
	''' 
	''' <p> If a format specifier contains a conversion character that is not
	''' applicable to the corresponding argument, then an {@link
	''' IllegalFormatConversionException} will be thrown.
	''' 
	''' <p> All specified exceptions may be thrown by any of the {@code format}
	''' methods of {@code Formatter} as well as by any {@code format} convenience
	''' methods such as <seealso cref="String#format(String,Object...) String.format"/> and
	''' <seealso cref="java.io.PrintStream#printf(String,Object...) PrintStream.printf"/>.
	''' 
	''' <p> Conversions denoted by an upper-case character (i.e. {@code 'B'},
	''' {@code 'H'}, {@code 'S'}, {@code 'C'}, {@code 'X'}, {@code 'E'},
	''' {@code 'G'}, {@code 'A'}, and {@code 'T'}) are the same as those for the
	''' corresponding lower-case conversion characters except that the result is
	''' converted to upper case according to the rules of the prevailing {@link
	''' java.util.Locale Locale}.  The result is equivalent to the following
	''' invocation of <seealso cref="String#toUpperCase()"/>
	''' 
	''' <pre>
	'''    out.toUpperCase() </pre>
	''' 
	''' <h4><a name="dgen">General</a></h4>
	''' 
	''' <p> The following general conversions may be applied to any argument type:
	''' 
	''' <table cellpadding=5 summary="dgConv">
	''' 
	''' <tr><td valign="top"> {@code 'b'}
	'''     <td valign="top"> <tt>'&#92;u0062'</tt>
	'''     <td> Produces either "{@code true}" or "{@code false}" as returned by
	'''     <seealso cref="Boolean#toString(boolean)"/>.
	''' 
	'''     <p> If the argument is {@code null}, then the result is
	'''     "{@code false}".  If the argument is a {@code boolean} or {@link
	'''     Boolean}, then the result is the string returned by {@link
	'''     String#valueOf(boolean) String.valueOf()}.  Otherwise, the result is
	'''     "{@code true}".
	''' 
	'''     <p> If the {@code '#'} flag is given, then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'B'}
	'''     <td valign="top"> <tt>'&#92;u0042'</tt>
	'''     <td> The upper-case variant of {@code 'b'}.
	''' 
	''' <tr><td valign="top"> {@code 'h'}
	'''     <td valign="top"> <tt>'&#92;u0068'</tt>
	'''     <td> Produces a string representing the hash code value of the object.
	''' 
	'''     <p> If the argument, <i>arg</i> is {@code null}, then the
	'''     result is "{@code null}".  Otherwise, the result is obtained
	'''     by invoking {@code  java.lang.[Integer].toHexString(arg.hashCode())}.
	''' 
	'''     <p> If the {@code '#'} flag is given, then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'H'}
	'''     <td valign="top"> <tt>'&#92;u0048'</tt>
	'''     <td> The upper-case variant of {@code 'h'}.
	''' 
	''' <tr><td valign="top"> {@code 's'}
	'''     <td valign="top"> <tt>'&#92;u0073'</tt>
	'''     <td> Produces a string.
	''' 
	'''     <p> If the argument is {@code null}, then the result is
	'''     "{@code null}".  If the argument implements <seealso cref="Formattable"/>, then
	'''     its <seealso cref="Formattable#formatTo formatTo"/> method is invoked.
	'''     Otherwise, the result is obtained by invoking the argument's
	'''     {@code toString()} method.
	''' 
	'''     <p> If the {@code '#'} flag is given and the argument is not a {@link
	'''     Formattable} , then a <seealso cref="FormatFlagsConversionMismatchException"/>
	'''     will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'S'}
	'''     <td valign="top"> <tt>'&#92;u0053'</tt>
	'''     <td> The upper-case variant of {@code 's'}.
	''' 
	''' </table>
	''' 
	''' <p> The following <a name="dFlags">flags</a> apply to general conversions:
	''' 
	''' <table cellpadding=5 summary="dFlags">
	''' 
	''' <tr><td valign="top"> {@code '-'}
	'''     <td valign="top"> <tt>'&#92;u002d'</tt>
	'''     <td> Left justifies the output.  Spaces (<tt>'&#92;u0020'</tt>) will be
	'''     added at the end of the converted value as required to fill the minimum
	'''     width of the field.  If the width is not provided, then a {@link
	'''     MissingFormatWidthException} will be thrown.  If this flag is not given
	'''     then the output will be right-justified.
	''' 
	''' <tr><td valign="top"> {@code '#'}
	'''     <td valign="top"> <tt>'&#92;u0023'</tt>
	'''     <td> Requires the output use an alternate form.  The definition of the
	'''     form is specified by the conversion.
	''' 
	''' </table>
	''' 
	''' <p> The <a name="genWidth">width</a> is the minimum number of characters to
	''' be written to the
	''' output.  If the length of the converted value is less than the width then
	''' the output will be padded by <tt>'&nbsp;&nbsp;'</tt> (<tt>'&#92;u0020'</tt>)
	''' until the total number of characters equals the width.  The padding is on
	''' the left by default.  If the {@code '-'} flag is given, then the padding
	''' will be on the right.  If the width is not specified then there is no
	''' minimum.
	''' 
	''' <p> The precision is the maximum number of characters to be written to the
	''' output.  The precision is applied before the width, thus the output will be
	''' truncated to {@code precision} characters even if the width is greater than
	''' the precision.  If the precision is not specified then there is no explicit
	''' limit on the number of characters.
	''' 
	''' <h4><a name="dchar">Character</a></h4>
	''' 
	''' This conversion may be applied to {@code char} and <seealso cref="Character"/>.  It
	''' may also be applied to the types {@code byte}, <seealso cref="Byte"/>,
	''' {@code short}, and <seealso cref="Short"/>, {@code int} and <seealso cref="Integer"/> when
	''' <seealso cref="Character#isValidCodePoint"/> returns {@code true}.  If it returns
	''' {@code false} then an <seealso cref="IllegalFormatCodePointException"/> will be
	''' thrown.
	''' 
	''' <table cellpadding=5 summary="charConv">
	''' 
	''' <tr><td valign="top"> {@code 'c'}
	'''     <td valign="top"> <tt>'&#92;u0063'</tt>
	'''     <td> Formats the argument as a Unicode character as described in <a
	'''     href="../lang/Character.html#unicode">Unicode Character
	'''     Representation</a>.  This may be more than one 16-bit {@code char} in
	'''     the case where the argument represents a supplementary character.
	''' 
	'''     <p> If the {@code '#'} flag is given, then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'C'}
	'''     <td valign="top"> <tt>'&#92;u0043'</tt>
	'''     <td> The upper-case variant of {@code 'c'}.
	''' 
	''' </table>
	''' 
	''' <p> The {@code '-'} flag defined for <a href="#dFlags">General
	''' conversions</a> applies.  If the {@code '#'} flag is given, then a {@link
	''' FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <p> The width is defined as for <a href="#genWidth">General conversions</a>.
	''' 
	''' <p> The precision is not applicable.  If the precision is specified then an
	''' <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	''' 
	''' <h4><a name="dnum">Numeric</a></h4>
	''' 
	''' <p> Numeric conversions are divided into the following categories:
	''' 
	''' <ol>
	''' 
	''' <li> <a href="#dnint"><b>Byte, Short, Integer, and Long</b></a>
	''' 
	''' <li> <a href="#dnbint"><b>BigInteger</b></a>
	''' 
	''' <li> <a href="#dndec"><b>Float and Double</b></a>
	''' 
	''' <li> <a href="#dnbdec"><b>BigDecimal</b></a>
	''' 
	''' </ol>
	''' 
	''' <p> Numeric types will be formatted according to the following algorithm:
	''' 
	''' <p><b><a name="L10nAlgorithm"> Number Localization Algorithm</a></b>
	''' 
	''' <p> After digits are obtained for the integer part, fractional part, and
	''' exponent (as appropriate for the data type), the following transformation
	''' is applied:
	''' 
	''' <ol>
	''' 
	''' <li> Each digit character <i>d</i> in the string is replaced by a
	''' locale-specific digit computed relative to the current locale's
	''' <seealso cref="java.text.DecimalFormatSymbols#getZeroDigit() zero digit"/>
	''' <i>z</i>; that is <i>d&nbsp;-&nbsp;</i> {@code '0'}
	''' <i>&nbsp;+&nbsp;z</i>.
	''' 
	''' <li> If a decimal separator is present, a locale-specific {@linkplain
	''' java.text.DecimalFormatSymbols#getDecimalSeparator decimal separator} is
	''' substituted.
	''' 
	''' <li> If the {@code ','} (<tt>'&#92;u002c'</tt>)
	''' <a name="L10nGroup">flag</a> is given, then the locale-specific {@linkplain
	''' java.text.DecimalFormatSymbols#getGroupingSeparator grouping separator} is
	''' inserted by scanning the integer part of the string from least significant
	''' to most significant digits and inserting a separator at intervals defined by
	''' the locale's {@link java.text.DecimalFormat#getGroupingSize() grouping
	''' size}.
	''' 
	''' <li> If the {@code '0'} flag is given, then the locale-specific {@linkplain
	''' java.text.DecimalFormatSymbols#getZeroDigit() zero digits} are inserted
	''' after the sign character, if any, and before the first non-zero digit, until
	''' the length of the string is equal to the requested field width.
	''' 
	''' <li> If the value is negative and the {@code '('} flag is given, then a
	''' {@code '('} (<tt>'&#92;u0028'</tt>) is prepended and a {@code ')'}
	''' (<tt>'&#92;u0029'</tt>) is appended.
	''' 
	''' <li> If the value is negative (or floating-point negative zero) and
	''' {@code '('} flag is not given, then a {@code '-'} (<tt>'&#92;u002d'</tt>)
	''' is prepended.
	''' 
	''' <li> If the {@code '+'} flag is given and the value is positive or zero (or
	''' floating-point positive zero), then a {@code '+'} (<tt>'&#92;u002b'</tt>)
	''' will be prepended.
	''' 
	''' </ol>
	''' 
	''' <p> If the value is NaN or positive infinity the literal strings "NaN" or
	''' "Infinity" respectively, will be output.  If the value is negative infinity,
	''' then the output will be "(Infinity)" if the {@code '('} flag is given
	''' otherwise the output will be "-Infinity".  These values are not localized.
	''' 
	''' <p><a name="dnint"><b> Byte, Short, Integer, and Long </b></a>
	''' 
	''' <p> The following conversions may be applied to {@code byte}, <seealso cref="Byte"/>,
	''' {@code short}, <seealso cref="Short"/>, {@code int} and <seealso cref="Integer"/>,
	''' {@code long}, and <seealso cref="Long"/>.
	''' 
	''' <table cellpadding=5 summary="IntConv">
	''' 
	''' <tr><td valign="top"> {@code 'd'}
	'''     <td valign="top"> <tt>'&#92;u0064'</tt>
	'''     <td> Formats the argument as a decimal  java.lang.[Integer]. The <a
	'''     href="#L10nAlgorithm">localization algorithm</a> is applied.
	''' 
	'''     <p> If the {@code '0'} flag is given and the value is negative, then
	'''     the zero padding will occur after the sign.
	''' 
	'''     <p> If the {@code '#'} flag is given then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'o'}
	'''     <td valign="top"> <tt>'&#92;u006f'</tt>
	'''     <td> Formats the argument as an integer in base eight.  No localization
	'''     is applied.
	''' 
	'''     <p> If <i>x</i> is negative then the result will be an unsigned value
	'''     generated by adding 2<sup>n</sup> to the value where {@code n} is the
	'''     number of bits in the type as returned by the static {@code SIZE} field
	'''     in the <seealso cref="Byte#SIZE Byte"/>, <seealso cref="Short#SIZE Short"/>,
	'''     <seealso cref="Integer#SIZE Integer"/>, or <seealso cref="Long#SIZE Long"/>
	'''     classes as appropriate.
	''' 
	'''     <p> If the {@code '#'} flag is given then the output will always begin
	'''     with the radix indicator {@code '0'}.
	''' 
	'''     <p> If the {@code '0'} flag is given then the output will be padded
	'''     with leading zeros to the field width following any indication of sign.
	''' 
	'''     <p> If {@code '('}, {@code '+'}, '&nbsp;&nbsp;', or {@code ','} flags
	'''     are given then a <seealso cref="FormatFlagsConversionMismatchException"/> will be
	'''     thrown.
	''' 
	''' <tr><td valign="top"> {@code 'x'}
	'''     <td valign="top"> <tt>'&#92;u0078'</tt>
	'''     <td> Formats the argument as an integer in base sixteen. No
	'''     localization is applied.
	''' 
	'''     <p> If <i>x</i> is negative then the result will be an unsigned value
	'''     generated by adding 2<sup>n</sup> to the value where {@code n} is the
	'''     number of bits in the type as returned by the static {@code SIZE} field
	'''     in the <seealso cref="Byte#SIZE Byte"/>, <seealso cref="Short#SIZE Short"/>,
	'''     <seealso cref="Integer#SIZE Integer"/>, or <seealso cref="Long#SIZE Long"/>
	'''     classes as appropriate.
	''' 
	'''     <p> If the {@code '#'} flag is given then the output will always begin
	'''     with the radix indicator {@code "0x"}.
	''' 
	'''     <p> If the {@code '0'} flag is given then the output will be padded to
	'''     the field width with leading zeros after the radix indicator or sign (if
	'''     present).
	''' 
	'''     <p> If {@code '('}, <tt>'&nbsp;&nbsp;'</tt>, {@code '+'}, or
	'''     {@code ','} flags are given then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'X'}
	'''     <td valign="top"> <tt>'&#92;u0058'</tt>
	'''     <td> The upper-case variant of {@code 'x'}.  The entire string
	'''     representing the number will be converted to {@linkplain
	'''     String#toUpperCase upper case} including the {@code 'x'} (if any) and
	'''     all hexadecimal digits {@code 'a'} - {@code 'f'}
	'''     (<tt>'&#92;u0061'</tt> -  <tt>'&#92;u0066'</tt>).
	''' 
	''' </table>
	''' 
	''' <p> If the conversion is {@code 'o'}, {@code 'x'}, or {@code 'X'} and
	''' both the {@code '#'} and the {@code '0'} flags are given, then result will
	''' contain the radix indicator ({@code '0'} for octal and {@code "0x"} or
	''' {@code "0X"} for hexadecimal), some number of zeros (based on the width),
	''' and the value.
	''' 
	''' <p> If the {@code '-'} flag is not given, then the space padding will occur
	''' before the sign.
	''' 
	''' <p> The following <a name="intFlags">flags</a> apply to numeric integral
	''' conversions:
	''' 
	''' <table cellpadding=5 summary="intFlags">
	''' 
	''' <tr><td valign="top"> {@code '+'}
	'''     <td valign="top"> <tt>'&#92;u002b'</tt>
	'''     <td> Requires the output to include a positive sign for all positive
	'''     numbers.  If this flag is not given then only negative values will
	'''     include a sign.
	''' 
	'''     <p> If both the {@code '+'} and <tt>'&nbsp;&nbsp;'</tt> flags are given
	'''     then an <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	''' 
	''' <tr><td valign="top"> <tt>'&nbsp;&nbsp;'</tt>
	'''     <td valign="top"> <tt>'&#92;u0020'</tt>
	'''     <td> Requires the output to include a single extra space
	'''     (<tt>'&#92;u0020'</tt>) for non-negative values.
	''' 
	'''     <p> If both the {@code '+'} and <tt>'&nbsp;&nbsp;'</tt> flags are given
	'''     then an <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	''' 
	''' <tr><td valign="top"> {@code '0'}
	'''     <td valign="top"> <tt>'&#92;u0030'</tt>
	'''     <td> Requires the output to be padded with leading {@linkplain
	'''     java.text.DecimalFormatSymbols#getZeroDigit zeros} to the minimum field
	'''     width following any sign or radix indicator except when converting NaN
	'''     or infinity.  If the width is not provided, then a {@link
	'''     MissingFormatWidthException} will be thrown.
	''' 
	'''     <p> If both the {@code '-'} and {@code '0'} flags are given then an
	'''     <seealso cref="IllegalFormatFlagsException"/> will be thrown.
	''' 
	''' <tr><td valign="top"> {@code ','}
	'''     <td valign="top"> <tt>'&#92;u002c'</tt>
	'''     <td> Requires the output to include the locale-specific {@linkplain
	'''     java.text.DecimalFormatSymbols#getGroupingSeparator group separators} as
	'''     described in the <a href="#L10nGroup">"group" section</a> of the
	'''     localization algorithm.
	''' 
	''' <tr><td valign="top"> {@code '('}
	'''     <td valign="top"> <tt>'&#92;u0028'</tt>
	'''     <td> Requires the output to prepend a {@code '('}
	'''     (<tt>'&#92;u0028'</tt>) and append a {@code ')'}
	'''     (<tt>'&#92;u0029'</tt>) to negative values.
	''' 
	''' </table>
	''' 
	''' <p> If no <a name="intdFlags">flags</a> are given the default formatting is
	''' as follows:
	''' 
	''' <ul>
	''' 
	''' <li> The output is right-justified within the {@code width}
	''' 
	''' <li> Negative numbers begin with a {@code '-'} (<tt>'&#92;u002d'</tt>)
	''' 
	''' <li> Positive numbers and zero do not include a sign or extra leading
	''' space
	''' 
	''' <li> No grouping separators are included
	''' 
	''' </ul>
	''' 
	''' <p> The <a name="intWidth">width</a> is the minimum number of characters to
	''' be written to the output.  This includes any signs, digits, grouping
	''' separators, radix indicator, and parentheses.  If the length of the
	''' converted value is less than the width then the output will be padded by
	''' spaces (<tt>'&#92;u0020'</tt>) until the total number of characters equals
	''' width.  The padding is on the left by default.  If {@code '-'} flag is
	''' given then the padding will be on the right.  If width is not specified then
	''' there is no minimum.
	''' 
	''' <p> The precision is not applicable.  If precision is specified then an
	''' <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	''' 
	''' <p><a name="dnbint"><b> BigInteger </b></a>
	''' 
	''' <p> The following conversions may be applied to {@link
	''' java.math.BigInteger}.
	''' 
	''' <table cellpadding=5 summary="BIntConv">
	''' 
	''' <tr><td valign="top"> {@code 'd'}
	'''     <td valign="top"> <tt>'&#92;u0064'</tt>
	'''     <td> Requires the output to be formatted as a decimal  java.lang.[Integer]. The <a
	'''     href="#L10nAlgorithm">localization algorithm</a> is applied.
	''' 
	'''     <p> If the {@code '#'} flag is given {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'o'}
	'''     <td valign="top"> <tt>'&#92;u006f'</tt>
	'''     <td> Requires the output to be formatted as an integer in base eight.
	'''     No localization is applied.
	''' 
	'''     <p> If <i>x</i> is negative then the result will be a signed value
	'''     beginning with {@code '-'} (<tt>'&#92;u002d'</tt>).  Signed output is
	'''     allowed for this type because unlike the primitive types it is not
	'''     possible to create an unsigned equivalent without assuming an explicit
	'''     data-type size.
	''' 
	'''     <p> If <i>x</i> is positive or zero and the {@code '+'} flag is given
	'''     then the result will begin with {@code '+'} (<tt>'&#92;u002b'</tt>).
	''' 
	'''     <p> If the {@code '#'} flag is given then the output will always begin
	'''     with {@code '0'} prefix.
	''' 
	'''     <p> If the {@code '0'} flag is given then the output will be padded
	'''     with leading zeros to the field width following any indication of sign.
	''' 
	'''     <p> If the {@code ','} flag is given then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'x'}
	'''     <td valign="top"> <tt>'&#92;u0078'</tt>
	'''     <td> Requires the output to be formatted as an integer in base
	'''     sixteen.  No localization is applied.
	''' 
	'''     <p> If <i>x</i> is negative then the result will be a signed value
	'''     beginning with {@code '-'} (<tt>'&#92;u002d'</tt>).  Signed output is
	'''     allowed for this type because unlike the primitive types it is not
	'''     possible to create an unsigned equivalent without assuming an explicit
	'''     data-type size.
	''' 
	'''     <p> If <i>x</i> is positive or zero and the {@code '+'} flag is given
	'''     then the result will begin with {@code '+'} (<tt>'&#92;u002b'</tt>).
	''' 
	'''     <p> If the {@code '#'} flag is given then the output will always begin
	'''     with the radix indicator {@code "0x"}.
	''' 
	'''     <p> If the {@code '0'} flag is given then the output will be padded to
	'''     the field width with leading zeros after the radix indicator or sign (if
	'''     present).
	''' 
	'''     <p> If the {@code ','} flag is given then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'X'}
	'''     <td valign="top"> <tt>'&#92;u0058'</tt>
	'''     <td> The upper-case variant of {@code 'x'}.  The entire string
	'''     representing the number will be converted to {@linkplain
	'''     String#toUpperCase upper case} including the {@code 'x'} (if any) and
	'''     all hexadecimal digits {@code 'a'} - {@code 'f'}
	'''     (<tt>'&#92;u0061'</tt> - <tt>'&#92;u0066'</tt>).
	''' 
	''' </table>
	''' 
	''' <p> If the conversion is {@code 'o'}, {@code 'x'}, or {@code 'X'} and
	''' both the {@code '#'} and the {@code '0'} flags are given, then result will
	''' contain the base indicator ({@code '0'} for octal and {@code "0x"} or
	''' {@code "0X"} for hexadecimal), some number of zeros (based on the width),
	''' and the value.
	''' 
	''' <p> If the {@code '0'} flag is given and the value is negative, then the
	''' zero padding will occur after the sign.
	''' 
	''' <p> If the {@code '-'} flag is not given, then the space padding will occur
	''' before the sign.
	''' 
	''' <p> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	''' Long apply.  The <a href="#intdFlags">default behavior</a> when no flags are
	''' given is the same as for Byte, Short, Integer, and java.lang.[Long].
	''' 
	''' <p> The specification of <a href="#intWidth">width</a> is the same as
	''' defined for Byte, Short, Integer, and java.lang.[Long].
	''' 
	''' <p> The precision is not applicable.  If precision is specified then an
	''' <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	''' 
	''' <p><a name="dndec"><b> Float and Double</b></a>
	''' 
	''' <p> The following conversions may be applied to {@code float}, {@link
	''' Float}, {@code double} and <seealso cref="Double"/>.
	''' 
	''' <table cellpadding=5 summary="floatConv">
	''' 
	''' <tr><td valign="top"> {@code 'e'}
	'''     <td valign="top"> <tt>'&#92;u0065'</tt>
	'''     <td> Requires the output to be formatted using <a
	'''     name="scientific">computerized scientific notation</a>.  The <a
	'''     href="#L10nAlgorithm">localization algorithm</a> is applied.
	''' 
	'''     <p> The formatting of the magnitude <i>m</i> depends upon its value.
	''' 
	'''     <p> If <i>m</i> is NaN or infinite, the literal strings "NaN" or
	'''     "Infinity", respectively, will be output.  These values are not
	'''     localized.
	''' 
	'''     <p> If <i>m</i> is positive-zero or negative-zero, then the exponent
	'''     will be {@code "+00"}.
	''' 
	'''     <p> Otherwise, the result is a string that represents the sign and
	'''     magnitude (absolute value) of the argument.  The formatting of the sign
	'''     is described in the <a href="#L10nAlgorithm">localization
	'''     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	'''     value.
	''' 
	'''     <p> Let <i>n</i> be the unique integer such that 10<sup><i>n</i></sup>
	'''     &lt;= <i>m</i> &lt; 10<sup><i>n</i>+1</sup>; then let <i>a</i> be the
	'''     mathematically exact quotient of <i>m</i> and 10<sup><i>n</i></sup> so
	'''     that 1 &lt;= <i>a</i> &lt; 10. The magnitude is then represented as the
	'''     integer part of <i>a</i>, as a single decimal digit, followed by the
	'''     decimal separator followed by decimal digits representing the fractional
	'''     part of <i>a</i>, followed by the exponent symbol {@code 'e'}
	'''     (<tt>'&#92;u0065'</tt>), followed by the sign of the exponent, followed
	'''     by a representation of <i>n</i> as a decimal integer, as produced by the
	'''     method <seealso cref="Long#toString(long, int)"/>, and zero-padded to include at
	'''     least two digits.
	''' 
	'''     <p> The number of digits in the result for the fractional part of
	'''     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	'''     specified then the default value is {@code 6}. If the precision is less
	'''     than the number of digits which would appear after the decimal point in
	'''     the string returned by <seealso cref="Float#toString(float)"/> or {@link
	'''     Double#toString(double)} respectively, then the value will be rounded
	'''     using the {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	'''     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	'''     For a canonical representation of the value, use {@link
	'''     Float#toString(float)} or <seealso cref="Double#toString(double)"/> as
	'''     appropriate.
	''' 
	'''     <p>If the {@code ','} flag is given, then an {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'E'}
	'''     <td valign="top"> <tt>'&#92;u0045'</tt>
	'''     <td> The upper-case variant of {@code 'e'}.  The exponent symbol
	'''     will be {@code 'E'} (<tt>'&#92;u0045'</tt>).
	''' 
	''' <tr><td valign="top"> {@code 'g'}
	'''     <td valign="top"> <tt>'&#92;u0067'</tt>
	'''     <td> Requires the output to be formatted in general scientific notation
	'''     as described below. The <a href="#L10nAlgorithm">localization
	'''     algorithm</a> is applied.
	''' 
	'''     <p> After rounding for the precision, the formatting of the resulting
	'''     magnitude <i>m</i> depends on its value.
	''' 
	'''     <p> If <i>m</i> is greater than or equal to 10<sup>-4</sup> but less
	'''     than 10<sup>precision</sup> then it is represented in <i><a
	'''     href="#decimal">decimal format</a></i>.
	''' 
	'''     <p> If <i>m</i> is less than 10<sup>-4</sup> or greater than or equal to
	'''     10<sup>precision</sup>, then it is represented in <i><a
	'''     href="#scientific">computerized scientific notation</a></i>.
	''' 
	'''     <p> The total number of significant digits in <i>m</i> is equal to the
	'''     precision.  If the precision is not specified, then the default value is
	'''     {@code 6}.  If the precision is {@code 0}, then it is taken to be
	'''     {@code 1}.
	''' 
	'''     <p> If the {@code '#'} flag is given then an {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'G'}
	'''     <td valign="top"> <tt>'&#92;u0047'</tt>
	'''     <td> The upper-case variant of {@code 'g'}.
	''' 
	''' <tr><td valign="top"> {@code 'f'}
	'''     <td valign="top"> <tt>'&#92;u0066'</tt>
	'''     <td> Requires the output to be formatted using <a name="decimal">decimal
	'''     format</a>.  The <a href="#L10nAlgorithm">localization algorithm</a> is
	'''     applied.
	''' 
	'''     <p> The result is a string that represents the sign and magnitude
	'''     (absolute value) of the argument.  The formatting of the sign is
	'''     described in the <a href="#L10nAlgorithm">localization
	'''     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	'''     value.
	''' 
	'''     <p> If <i>m</i> NaN or infinite, the literal strings "NaN" or
	'''     "Infinity", respectively, will be output.  These values are not
	'''     localized.
	''' 
	'''     <p> The magnitude is formatted as the integer part of <i>m</i>, with no
	'''     leading zeroes, followed by the decimal separator followed by one or
	'''     more decimal digits representing the fractional part of <i>m</i>.
	''' 
	'''     <p> The number of digits in the result for the fractional part of
	'''     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	'''     specified then the default value is {@code 6}. If the precision is less
	'''     than the number of digits which would appear after the decimal point in
	'''     the string returned by <seealso cref="Float#toString(float)"/> or {@link
	'''     Double#toString(double)} respectively, then the value will be rounded
	'''     using the {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	'''     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	'''     For a canonical representation of the value, use {@link
	'''     Float#toString(float)} or <seealso cref="Double#toString(double)"/> as
	'''     appropriate.
	''' 
	''' <tr><td valign="top"> {@code 'a'}
	'''     <td valign="top"> <tt>'&#92;u0061'</tt>
	'''     <td> Requires the output to be formatted in hexadecimal exponential
	'''     form.  No localization is applied.
	''' 
	'''     <p> The result is a string that represents the sign and magnitude
	'''     (absolute value) of the argument <i>x</i>.
	''' 
	'''     <p> If <i>x</i> is negative or a negative-zero value then the result
	'''     will begin with {@code '-'} (<tt>'&#92;u002d'</tt>).
	''' 
	'''     <p> If <i>x</i> is positive or a positive-zero value and the
	'''     {@code '+'} flag is given then the result will begin with {@code '+'}
	'''     (<tt>'&#92;u002b'</tt>).
	''' 
	'''     <p> The formatting of the magnitude <i>m</i> depends upon its value.
	''' 
	'''     <ul>
	''' 
	'''     <li> If the value is NaN or infinite, the literal strings "NaN" or
	'''     "Infinity", respectively, will be output.
	''' 
	'''     <li> If <i>m</i> is zero then it is represented by the string
	'''     {@code "0x0.0p0"}.
	''' 
	'''     <li> If <i>m</i> is a {@code double} value with a normalized
	'''     representation then substrings are used to represent the significand and
	'''     exponent fields.  The significand is represented by the characters
	'''     {@code "0x1."} followed by the hexadecimal representation of the rest
	'''     of the significand as a fraction.  The exponent is represented by
	'''     {@code 'p'} (<tt>'&#92;u0070'</tt>) followed by a decimal string of the
	'''     unbiased exponent as if produced by invoking {@link
	'''     Integer#toString(int)  java.lang.[Integer].toString} on the exponent value.  If the
	'''     precision is specified, the value is rounded to the given number of
	'''     hexadecimal digits.
	''' 
	'''     <li> If <i>m</i> is a {@code double} value with a subnormal
	'''     representation then, unless the precision is specified to be in the range
	'''     1 through 12, inclusive, the significand is represented by the characters
	'''     {@code '0x0.'} followed by the hexadecimal representation of the rest of
	'''     the significand as a fraction, and the exponent represented by
	'''     {@code 'p-1022'}.  If the precision is in the interval
	'''     [1,&nbsp;12], the subnormal value is normalized such that it
	'''     begins with the characters {@code '0x1.'}, rounded to the number of
	'''     hexadecimal digits of precision, and the exponent adjusted
	'''     accordingly.  Note that there must be at least one nonzero digit in a
	'''     subnormal significand.
	''' 
	'''     </ul>
	''' 
	'''     <p> If the {@code '('} or {@code ','} flags are given, then a {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'A'}
	'''     <td valign="top"> <tt>'&#92;u0041'</tt>
	'''     <td> The upper-case variant of {@code 'a'}.  The entire string
	'''     representing the number will be converted to upper case including the
	'''     {@code 'x'} (<tt>'&#92;u0078'</tt>) and {@code 'p'}
	'''     (<tt>'&#92;u0070'</tt> and all hexadecimal digits {@code 'a'} -
	'''     {@code 'f'} (<tt>'&#92;u0061'</tt> - <tt>'&#92;u0066'</tt>).
	''' 
	''' </table>
	''' 
	''' <p> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	''' Long apply.
	''' 
	''' <p> If the {@code '#'} flag is given, then the decimal separator will
	''' always be present.
	''' 
	''' <p> If no <a name="floatdFlags">flags</a> are given the default formatting
	''' is as follows:
	''' 
	''' <ul>
	''' 
	''' <li> The output is right-justified within the {@code width}
	''' 
	''' <li> Negative numbers begin with a {@code '-'}
	''' 
	''' <li> Positive numbers and positive zero do not include a sign or extra
	''' leading space
	''' 
	''' <li> No grouping separators are included
	''' 
	''' <li> The decimal separator will only appear if a digit follows it
	''' 
	''' </ul>
	''' 
	''' <p> The <a name="floatDWidth">width</a> is the minimum number of characters
	''' to be written to the output.  This includes any signs, digits, grouping
	''' separators, decimal separators, exponential symbol, radix indicator,
	''' parentheses, and strings representing infinity and NaN as applicable.  If
	''' the length of the converted value is less than the width then the output
	''' will be padded by spaces (<tt>'&#92;u0020'</tt>) until the total number of
	''' characters equals width.  The padding is on the left by default.  If the
	''' {@code '-'} flag is given then the padding will be on the right.  If width
	''' is not specified then there is no minimum.
	''' 
	''' <p> If the <a name="floatDPrec">conversion</a> is {@code 'e'},
	''' {@code 'E'} or {@code 'f'}, then the precision is the number of digits
	''' after the decimal separator.  If the precision is not specified, then it is
	''' assumed to be {@code 6}.
	''' 
	''' <p> If the conversion is {@code 'g'} or {@code 'G'}, then the precision is
	''' the total number of significant digits in the resulting magnitude after
	''' rounding.  If the precision is not specified, then the default value is
	''' {@code 6}.  If the precision is {@code 0}, then it is taken to be
	''' {@code 1}.
	''' 
	''' <p> If the conversion is {@code 'a'} or {@code 'A'}, then the precision
	''' is the number of hexadecimal digits after the radix point.  If the
	''' precision is not provided, then all of the digits as returned by {@link
	''' Double#toHexString(double)} will be output.
	''' 
	''' <p><a name="dnbdec"><b> BigDecimal </b></a>
	''' 
	''' <p> The following conversions may be applied {@link java.math.BigDecimal
	''' BigDecimal}.
	''' 
	''' <table cellpadding=5 summary="floatConv">
	''' 
	''' <tr><td valign="top"> {@code 'e'}
	'''     <td valign="top"> <tt>'&#92;u0065'</tt>
	'''     <td> Requires the output to be formatted using <a
	'''     name="bscientific">computerized scientific notation</a>.  The <a
	'''     href="#L10nAlgorithm">localization algorithm</a> is applied.
	''' 
	'''     <p> The formatting of the magnitude <i>m</i> depends upon its value.
	''' 
	'''     <p> If <i>m</i> is positive-zero or negative-zero, then the exponent
	'''     will be {@code "+00"}.
	''' 
	'''     <p> Otherwise, the result is a string that represents the sign and
	'''     magnitude (absolute value) of the argument.  The formatting of the sign
	'''     is described in the <a href="#L10nAlgorithm">localization
	'''     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	'''     value.
	''' 
	'''     <p> Let <i>n</i> be the unique integer such that 10<sup><i>n</i></sup>
	'''     &lt;= <i>m</i> &lt; 10<sup><i>n</i>+1</sup>; then let <i>a</i> be the
	'''     mathematically exact quotient of <i>m</i> and 10<sup><i>n</i></sup> so
	'''     that 1 &lt;= <i>a</i> &lt; 10. The magnitude is then represented as the
	'''     integer part of <i>a</i>, as a single decimal digit, followed by the
	'''     decimal separator followed by decimal digits representing the fractional
	'''     part of <i>a</i>, followed by the exponent symbol {@code 'e'}
	'''     (<tt>'&#92;u0065'</tt>), followed by the sign of the exponent, followed
	'''     by a representation of <i>n</i> as a decimal integer, as produced by the
	'''     method <seealso cref="Long#toString(long, int)"/>, and zero-padded to include at
	'''     least two digits.
	''' 
	'''     <p> The number of digits in the result for the fractional part of
	'''     <i>m</i> or <i>a</i> is equal to the precision.  If the precision is not
	'''     specified then the default value is {@code 6}.  If the precision is
	'''     less than the number of digits to the right of the decimal point then
	'''     the value will be rounded using the
	'''     {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	'''     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	'''     For a canonical representation of the value, use {@link
	'''     BigDecimal#toString()}.
	''' 
	'''     <p> If the {@code ','} flag is given, then an {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'E'}
	'''     <td valign="top"> <tt>'&#92;u0045'</tt>
	'''     <td> The upper-case variant of {@code 'e'}.  The exponent symbol
	'''     will be {@code 'E'} (<tt>'&#92;u0045'</tt>).
	''' 
	''' <tr><td valign="top"> {@code 'g'}
	'''     <td valign="top"> <tt>'&#92;u0067'</tt>
	'''     <td> Requires the output to be formatted in general scientific notation
	'''     as described below. The <a href="#L10nAlgorithm">localization
	'''     algorithm</a> is applied.
	''' 
	'''     <p> After rounding for the precision, the formatting of the resulting
	'''     magnitude <i>m</i> depends on its value.
	''' 
	'''     <p> If <i>m</i> is greater than or equal to 10<sup>-4</sup> but less
	'''     than 10<sup>precision</sup> then it is represented in <i><a
	'''     href="#bdecimal">decimal format</a></i>.
	''' 
	'''     <p> If <i>m</i> is less than 10<sup>-4</sup> or greater than or equal to
	'''     10<sup>precision</sup>, then it is represented in <i><a
	'''     href="#bscientific">computerized scientific notation</a></i>.
	''' 
	'''     <p> The total number of significant digits in <i>m</i> is equal to the
	'''     precision.  If the precision is not specified, then the default value is
	'''     {@code 6}.  If the precision is {@code 0}, then it is taken to be
	'''     {@code 1}.
	''' 
	'''     <p> If the {@code '#'} flag is given then an {@link
	'''     FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <tr><td valign="top"> {@code 'G'}
	'''     <td valign="top"> <tt>'&#92;u0047'</tt>
	'''     <td> The upper-case variant of {@code 'g'}.
	''' 
	''' <tr><td valign="top"> {@code 'f'}
	'''     <td valign="top"> <tt>'&#92;u0066'</tt>
	'''     <td> Requires the output to be formatted using <a name="bdecimal">decimal
	'''     format</a>.  The <a href="#L10nAlgorithm">localization algorithm</a> is
	'''     applied.
	''' 
	'''     <p> The result is a string that represents the sign and magnitude
	'''     (absolute value) of the argument.  The formatting of the sign is
	'''     described in the <a href="#L10nAlgorithm">localization
	'''     algorithm</a>. The formatting of the magnitude <i>m</i> depends upon its
	'''     value.
	''' 
	'''     <p> The magnitude is formatted as the integer part of <i>m</i>, with no
	'''     leading zeroes, followed by the decimal separator followed by one or
	'''     more decimal digits representing the fractional part of <i>m</i>.
	''' 
	'''     <p> The number of digits in the result for the fractional part of
	'''     <i>m</i> or <i>a</i> is equal to the precision. If the precision is not
	'''     specified then the default value is {@code 6}.  If the precision is
	'''     less than the number of digits to the right of the decimal point
	'''     then the value will be rounded using the
	'''     {@link java.math.BigDecimal#ROUND_HALF_UP round half up
	'''     algorithm}.  Otherwise, zeros may be appended to reach the precision.
	'''     For a canonical representation of the value, use {@link
	'''     BigDecimal#toString()}.
	''' 
	''' </table>
	''' 
	''' <p> All <a href="#intFlags">flags</a> defined for Byte, Short, Integer, and
	''' Long apply.
	''' 
	''' <p> If the {@code '#'} flag is given, then the decimal separator will
	''' always be present.
	''' 
	''' <p> The <a href="#floatdFlags">default behavior</a> when no flags are
	''' given is the same as for Float and java.lang.[Double].
	''' 
	''' <p> The specification of <a href="#floatDWidth">width</a> and <a
	''' href="#floatDPrec">precision</a> is the same as defined for Float and
	''' java.lang.[Double].
	''' 
	''' <h4><a name="ddt">Date/Time</a></h4>
	''' 
	''' <p> This conversion may be applied to {@code long}, <seealso cref="Long"/>, {@link
	''' Calendar}, <seealso cref="Date"/> and <seealso cref="TemporalAccessor TemporalAccessor"/>
	''' 
	''' <table cellpadding=5 summary="DTConv">
	''' 
	''' <tr><td valign="top"> {@code 't'}
	'''     <td valign="top"> <tt>'&#92;u0074'</tt>
	'''     <td> Prefix for date and time conversion characters.
	''' <tr><td valign="top"> {@code 'T'}
	'''     <td valign="top"> <tt>'&#92;u0054'</tt>
	'''     <td> The upper-case variant of {@code 't'}.
	''' 
	''' </table>
	''' 
	''' <p> The following date and time conversion character suffixes are defined
	''' for the {@code 't'} and {@code 'T'} conversions.  The types are similar to
	''' but not completely identical to those defined by GNU {@code date} and
	''' POSIX {@code strftime(3c)}.  Additional conversion types are provided to
	''' access Java-specific functionality (e.g. {@code 'L'} for milliseconds
	''' within the second).
	''' 
	''' <p> The following conversion characters are used for formatting times:
	''' 
	''' <table cellpadding=5 summary="time">
	''' 
	''' <tr><td valign="top"> {@code 'H'}
	'''     <td valign="top"> <tt>'&#92;u0048'</tt>
	'''     <td> Hour of the day for the 24-hour clock, formatted as two digits with
	'''     a leading zero as necessary i.e. {@code 00 - 23}. {@code 00}
	'''     corresponds to midnight.
	''' 
	''' <tr><td valign="top">{@code 'I'}
	'''     <td valign="top"> <tt>'&#92;u0049'</tt>
	'''     <td> Hour for the 12-hour clock, formatted as two digits with a leading
	'''     zero as necessary, i.e.  {@code 01 - 12}.  {@code 01} corresponds to
	'''     one o'clock (either morning or afternoon).
	''' 
	''' <tr><td valign="top">{@code 'k'}
	'''     <td valign="top"> <tt>'&#92;u006b'</tt>
	'''     <td> Hour of the day for the 24-hour clock, i.e. {@code 0 - 23}.
	'''     {@code 0} corresponds to midnight.
	''' 
	''' <tr><td valign="top">{@code 'l'}
	'''     <td valign="top"> <tt>'&#92;u006c'</tt>
	'''     <td> Hour for the 12-hour clock, i.e. {@code 1 - 12}.  {@code 1}
	'''     corresponds to one o'clock (either morning or afternoon).
	''' 
	''' <tr><td valign="top">{@code 'M'}
	'''     <td valign="top"> <tt>'&#92;u004d'</tt>
	'''     <td> Minute within the hour formatted as two digits with a leading zero
	'''     as necessary, i.e.  {@code 00 - 59}.
	''' 
	''' <tr><td valign="top">{@code 'S'}
	'''     <td valign="top"> <tt>'&#92;u0053'</tt>
	'''     <td> Seconds within the minute, formatted as two digits with a leading
	'''     zero as necessary, i.e. {@code 00 - 60} ("{@code 60}" is a special
	'''     value required to support leap seconds).
	''' 
	''' <tr><td valign="top">{@code 'L'}
	'''     <td valign="top"> <tt>'&#92;u004c'</tt>
	'''     <td> Millisecond within the second formatted as three digits with
	'''     leading zeros as necessary, i.e. {@code 000 - 999}.
	''' 
	''' <tr><td valign="top">{@code 'N'}
	'''     <td valign="top"> <tt>'&#92;u004e'</tt>
	'''     <td> Nanosecond within the second, formatted as nine digits with leading
	'''     zeros as necessary, i.e. {@code 000000000 - 999999999}.  The precision
	'''     of this value is limited by the resolution of the underlying operating
	'''     system or hardware.
	''' 
	''' <tr><td valign="top">{@code 'p'}
	'''     <td valign="top"> <tt>'&#92;u0070'</tt>
	'''     <td> Locale-specific {@linkplain
	'''     java.text.DateFormatSymbols#getAmPmStrings morning or afternoon} marker
	'''     in lower case, e.g."{@code am}" or "{@code pm}".  Use of the
	'''     conversion prefix {@code 'T'} forces this output to upper case.  (Note
	'''     that {@code 'p'} produces lower-case output.  This is different from
	'''     GNU {@code date} and POSIX {@code strftime(3c)} which produce
	'''     upper-case output.)
	''' 
	''' <tr><td valign="top">{@code 'z'}
	'''     <td valign="top"> <tt>'&#92;u007a'</tt>
	'''     <td> <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC&nbsp;822</a>
	'''     style numeric time zone offset from GMT, e.g. {@code -0800}.  This
	'''     value will be adjusted as necessary for Daylight Saving Time.  For
	'''     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	'''     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	'''     instance of the Java virtual machine.
	''' 
	''' <tr><td valign="top">{@code 'Z'}
	'''     <td valign="top"> <tt>'&#92;u005a'</tt>
	'''     <td> A string representing the abbreviation for the time zone.  This
	'''     value will be adjusted as necessary for Daylight Saving Time.  For
	'''     {@code long}, <seealso cref="Long"/>, and <seealso cref="Date"/> the time zone used is
	'''     the <seealso cref="TimeZone#getDefault() default time zone"/> for this
	'''     instance of the Java virtual machine.  The Formatter's locale will
	'''     supersede the locale of the argument (if any).
	''' 
	''' <tr><td valign="top">{@code 's'}
	'''     <td valign="top"> <tt>'&#92;u0073'</tt>
	'''     <td> Seconds since the beginning of the epoch starting at 1 January 1970
	'''     {@code 00:00:00} UTC, i.e. {@code java.lang.[Long].MIN_VALUE/1000} to
	'''     {@code java.lang.[Long].MAX_VALUE/1000}.
	''' 
	''' <tr><td valign="top">{@code 'Q'}
	'''     <td valign="top"> <tt>'&#92;u004f'</tt>
	'''     <td> Milliseconds since the beginning of the epoch starting at 1 January
	'''     1970 {@code 00:00:00} UTC, i.e. {@code java.lang.[Long].MIN_VALUE} to
	'''     {@code java.lang.[Long].MAX_VALUE}. The precision of this value is limited by
	'''     the resolution of the underlying operating system or hardware.
	''' 
	''' </table>
	''' 
	''' <p> The following conversion characters are used for formatting dates:
	''' 
	''' <table cellpadding=5 summary="date">
	''' 
	''' <tr><td valign="top">{@code 'B'}
	'''     <td valign="top"> <tt>'&#92;u0042'</tt>
	'''     <td> Locale-specific {@link java.text.DateFormatSymbols#getMonths
	'''     full month name}, e.g. {@code "January"}, {@code "February"}.
	''' 
	''' <tr><td valign="top">{@code 'b'}
	'''     <td valign="top"> <tt>'&#92;u0062'</tt>
	'''     <td> Locale-specific {@linkplain
	'''     java.text.DateFormatSymbols#getShortMonths abbreviated month name},
	'''     e.g. {@code "Jan"}, {@code "Feb"}.
	''' 
	''' <tr><td valign="top">{@code 'h'}
	'''     <td valign="top"> <tt>'&#92;u0068'</tt>
	'''     <td> Same as {@code 'b'}.
	''' 
	''' <tr><td valign="top">{@code 'A'}
	'''     <td valign="top"> <tt>'&#92;u0041'</tt>
	'''     <td> Locale-specific full name of the {@linkplain
	'''     java.text.DateFormatSymbols#getWeekdays day of the week},
	'''     e.g. {@code "Sunday"}, {@code "Monday"}
	''' 
	''' <tr><td valign="top">{@code 'a'}
	'''     <td valign="top"> <tt>'&#92;u0061'</tt>
	'''     <td> Locale-specific short name of the {@linkplain
	'''     java.text.DateFormatSymbols#getShortWeekdays day of the week},
	'''     e.g. {@code "Sun"}, {@code "Mon"}
	''' 
	''' <tr><td valign="top">{@code 'C'}
	'''     <td valign="top"> <tt>'&#92;u0043'</tt>
	'''     <td> Four-digit year divided by {@code 100}, formatted as two digits
	'''     with leading zero as necessary, i.e. {@code 00 - 99}
	''' 
	''' <tr><td valign="top">{@code 'Y'}
	'''     <td valign="top"> <tt>'&#92;u0059'</tt> <td> Year, formatted to at least
	'''     four digits with leading zeros as necessary, e.g. {@code 0092} equals
	'''     {@code 92} CE for the Gregorian calendar.
	''' 
	''' <tr><td valign="top">{@code 'y'}
	'''     <td valign="top"> <tt>'&#92;u0079'</tt>
	'''     <td> Last two digits of the year, formatted with leading zeros as
	'''     necessary, i.e. {@code 00 - 99}.
	''' 
	''' <tr><td valign="top">{@code 'j'}
	'''     <td valign="top"> <tt>'&#92;u006a'</tt>
	'''     <td> Day of year, formatted as three digits with leading zeros as
	'''     necessary, e.g. {@code 001 - 366} for the Gregorian calendar.
	'''     {@code 001} corresponds to the first day of the year.
	''' 
	''' <tr><td valign="top">{@code 'm'}
	'''     <td valign="top"> <tt>'&#92;u006d'</tt>
	'''     <td> Month, formatted as two digits with leading zeros as necessary,
	'''     i.e. {@code 01 - 13}, where "{@code 01}" is the first month of the
	'''     year and ("{@code 13}" is a special value required to support lunar
	'''     calendars).
	''' 
	''' <tr><td valign="top">{@code 'd'}
	'''     <td valign="top"> <tt>'&#92;u0064'</tt>
	'''     <td> Day of month, formatted as two digits with leading zeros as
	'''     necessary, i.e. {@code 01 - 31}, where "{@code 01}" is the first day
	'''     of the month.
	''' 
	''' <tr><td valign="top">{@code 'e'}
	'''     <td valign="top"> <tt>'&#92;u0065'</tt>
	'''     <td> Day of month, formatted as two digits, i.e. {@code 1 - 31} where
	'''     "{@code 1}" is the first day of the month.
	''' 
	''' </table>
	''' 
	''' <p> The following conversion characters are used for formatting common
	''' date/time compositions.
	''' 
	''' <table cellpadding=5 summary="composites">
	''' 
	''' <tr><td valign="top">{@code 'R'}
	'''     <td valign="top"> <tt>'&#92;u0052'</tt>
	'''     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM"}
	''' 
	''' <tr><td valign="top">{@code 'T'}
	'''     <td valign="top"> <tt>'&#92;u0054'</tt>
	'''     <td> Time formatted for the 24-hour clock as {@code "%tH:%tM:%tS"}.
	''' 
	''' <tr><td valign="top">{@code 'r'}
	'''     <td valign="top"> <tt>'&#92;u0072'</tt>
	'''     <td> Time formatted for the 12-hour clock as {@code "%tI:%tM:%tS
	'''     %Tp"}.  The location of the morning or afternoon marker
	'''     ({@code '%Tp'}) may be locale-dependent.
	''' 
	''' <tr><td valign="top">{@code 'D'}
	'''     <td valign="top"> <tt>'&#92;u0044'</tt>
	'''     <td> Date formatted as {@code "%tm/%td/%ty"}.
	''' 
	''' <tr><td valign="top">{@code 'F'}
	'''     <td valign="top"> <tt>'&#92;u0046'</tt>
	'''     <td> <a href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a>
	'''     complete date formatted as {@code "%tY-%tm-%td"}.
	''' 
	''' <tr><td valign="top">{@code 'c'}
	'''     <td valign="top"> <tt>'&#92;u0063'</tt>
	'''     <td> Date and time formatted as {@code "%ta %tb %td %tT %tZ %tY"},
	'''     e.g. {@code "Sun Jul 20 16:17:00 EDT 1969"}.
	''' 
	''' </table>
	''' 
	''' <p> The {@code '-'} flag defined for <a href="#dFlags">General
	''' conversions</a> applies.  If the {@code '#'} flag is given, then a {@link
	''' FormatFlagsConversionMismatchException} will be thrown.
	''' 
	''' <p> The width is the minimum number of characters to
	''' be written to the output.  If the length of the converted value is less than
	''' the {@code width} then the output will be padded by spaces
	''' (<tt>'&#92;u0020'</tt>) until the total number of characters equals width.
	''' The padding is on the left by default.  If the {@code '-'} flag is given
	''' then the padding will be on the right.  If width is not specified then there
	''' is no minimum.
	''' 
	''' <p> The precision is not applicable.  If the precision is specified then an
	''' <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	''' 
	''' <h4><a name="dper">Percent</a></h4>
	''' 
	''' <p> The conversion does not correspond to any argument.
	''' 
	''' <table cellpadding=5 summary="DTConv">
	''' 
	''' <tr><td valign="top">{@code '%'}
	'''     <td> The result is a literal {@code '%'} (<tt>'&#92;u0025'</tt>)
	''' 
	''' <p> The width is the minimum number of characters to
	''' be written to the output including the {@code '%'}.  If the length of the
	''' converted value is less than the {@code width} then the output will be
	''' padded by spaces (<tt>'&#92;u0020'</tt>) until the total number of
	''' characters equals width.  The padding is on the left.  If width is not
	''' specified then just the {@code '%'} is output.
	''' 
	''' <p> The {@code '-'} flag defined for <a href="#dFlags">General
	''' conversions</a> applies.  If any other flags are provided, then a
	''' <seealso cref="FormatFlagsConversionMismatchException"/> will be thrown.
	''' 
	''' <p> The precision is not applicable.  If the precision is specified an
	''' <seealso cref="IllegalFormatPrecisionException"/> will be thrown.
	''' 
	''' </table>
	''' 
	''' <h4><a name="dls">Line Separator</a></h4>
	''' 
	''' <p> The conversion does not correspond to any argument.
	''' 
	''' <table cellpadding=5 summary="DTConv">
	''' 
	''' <tr><td valign="top">{@code 'n'}
	'''     <td> the platform-specific line separator as returned by {@link
	'''     System#getProperty System.getProperty("line.separator")}.
	''' 
	''' </table>
	''' 
	''' <p> Flags, width, and precision are not applicable.  If any are provided an
	''' <seealso cref="IllegalFormatFlagsException"/>, <seealso cref="IllegalFormatWidthException"/>,
	''' and <seealso cref="IllegalFormatPrecisionException"/>, respectively will be thrown.
	''' 
	''' <h4><a name="dpos">Argument Index</a></h4>
	''' 
	''' <p> Format specifiers can reference arguments in three ways:
	''' 
	''' <ul>
	''' 
	''' <li> <i>Explicit indexing</i> is used when the format specifier contains an
	''' argument index.  The argument index is a decimal integer indicating the
	''' position of the argument in the argument list.  The first argument is
	''' referenced by "{@code 1$}", the second by "{@code 2$}", etc.  An argument
	''' may be referenced more than once.
	''' 
	''' <p> For example:
	''' 
	''' <blockquote><pre>
	'''   formatter.format("%4$s %3$s %2$s %1$s %4$s %3$s %2$s %1$s",
	'''                    "a", "b", "c", "d")
	'''   // -&gt; "d c b a d c b a"
	''' </pre></blockquote>
	''' 
	''' <li> <i>Relative indexing</i> is used when the format specifier contains a
	''' {@code '<'} (<tt>'&#92;u003c'</tt>) flag which causes the argument for
	''' the previous format specifier to be re-used.  If there is no previous
	''' argument, then a <seealso cref="MissingFormatArgumentException"/> is thrown.
	''' 
	''' <blockquote><pre>
	'''    formatter.format("%s %s %&lt;s %&lt;s", "a", "b", "c", "d")
	'''    // -&gt; "a b b b"
	'''    // "c" and "d" are ignored because they are not referenced
	''' </pre></blockquote>
	''' 
	''' <li> <i>Ordinary indexing</i> is used when the format specifier contains
	''' neither an argument index nor a {@code '<'} flag.  Each format specifier
	''' which uses ordinary indexing is assigned a sequential implicit index into
	''' argument list which is independent of the indices used by explicit or
	''' relative indexing.
	''' 
	''' <blockquote><pre>
	'''   formatter.format("%s %s %s %s", "a", "b", "c", "d")
	'''   // -&gt; "a b c d"
	''' </pre></blockquote>
	''' 
	''' </ul>
	''' 
	''' <p> It is possible to have a format string which uses all forms of indexing,
	''' for example:
	''' 
	''' <blockquote><pre>
	'''   formatter.format("%2$s %s %&lt;s %s", "a", "b", "c", "d")
	'''   // -&gt; "b a a b"
	'''   // "c" and "d" are ignored because they are not referenced
	''' </pre></blockquote>
	''' 
	''' <p> The maximum number of arguments is limited by the maximum dimension of a
	''' Java array as defined by
	''' <cite>The Java&trade; Virtual Machine Specification</cite>.
	''' If the argument index is does not correspond to an
	''' available argument, then a <seealso cref="MissingFormatArgumentException"/> is thrown.
	''' 
	''' <p> If there are more arguments than format specifiers, the extra arguments
	''' are ignored.
	''' 
	''' <p> Unless otherwise specified, passing a {@code null} argument to any
	''' method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @author  Iris Clark
	''' @since 1.5
	''' </summary>
	Public NotInheritable Class Formatter
		Implements java.io.Closeable, java.io.Flushable

		Private a As Appendable
		Private ReadOnly l As Locale

		Private lastException As java.io.IOException

		Private ReadOnly zero As Char
		Private Shared scaleUp As Double

		' 1 (sign) + 19 (max # sig digits) + 1 ('.') + 1 ('e') + 1 (sign)
		' + 3 (max # exp digits) + 4 (error) = 30
		Private Const MAX_FD_CHARS As Integer = 30

		''' <summary>
		''' Returns a charset object for the given charset name. </summary>
		''' <exception cref="NullPointerException">          is csn is null </exception>
		''' <exception cref="UnsupportedEncodingException">  if the charset is not supported </exception>
		Private Shared Function toCharset(ByVal csn As String) As java.nio.charset.Charset
			Objects.requireNonNull(csn, "charsetName")
			Try
				Return java.nio.charset.Charset.forName(csn)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch java.nio.charset.IllegalCharsetNameException Or java.nio.charset.UnsupportedCharsetException unused
				' UnsupportedEncodingException should be thrown
				Throw New java.io.UnsupportedEncodingException(csn)
			End Try
		End Function

		Private Shared Function nonNullAppendable(ByVal a As Appendable) As Appendable
			If a Is Nothing Then Return New StringBuilder

			Return a
		End Function

		' Private constructors 
		Private Sub New(ByVal l As Locale, ByVal a As Appendable)
			Me.a = a
			Me.l = l
			Me.zero = getZero(l)
		End Sub

		Private Sub New(ByVal charset As java.nio.charset.Charset, ByVal l As Locale, ByVal file As java.io.File)
			Me.New(l, New java.io.BufferedWriter(New java.io.OutputStreamWriter(New java.io.FileOutputStream(file), charset)))
		End Sub

		''' <summary>
		''' Constructs a new formatter.
		''' 
		''' <p> The destination of the formatted output is a <seealso cref="StringBuilder"/>
		''' which may be retrieved by invoking <seealso cref="#out out()"/> and whose
		''' current content may be converted into a string by invoking {@link
		''' #toString toString()}.  The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		Public Sub New()
			Me.New(Locale.getDefault(Locale.Category.FORMAT), New StringBuilder)
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified destination.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="a">
		'''         Destination for the formatted output.  If {@code a} is
		'''         {@code null} then a <seealso cref="StringBuilder"/> will be created. </param>
		Public Sub New(ByVal a As Appendable)
			Me.New(Locale.getDefault(Locale.Category.FORMAT), nonNullAppendable(a))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified locale.
		''' 
		''' <p> The destination of the formatted output is a <seealso cref="StringBuilder"/>
		''' which may be retrieved by invoking <seealso cref="#out out()"/> and whose current
		''' content may be converted into a string by invoking {@link #toString
		''' toString()}.
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied. </param>
		Public Sub New(ByVal l As Locale)
			Me.New(l, New StringBuilder)
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified destination and locale.
		''' </summary>
		''' <param name="a">
		'''         Destination for the formatted output.  If {@code a} is
		'''         {@code null} then a <seealso cref="StringBuilder"/> will be created.
		''' </param>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied. </param>
		Public Sub New(ByVal a As Appendable, ByVal l As Locale)
			Me.New(l, nonNullAppendable(a))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file name.
		''' 
		''' <p> The charset used is the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this
		'''         formatter.  If the file exists then it will be truncated to
		'''         zero size; otherwise, a new file will be created.  The output
		'''         will be written to the file and is buffered.
		''' </param>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(fileName)} denies write
		'''          access to the file
		''' </exception>
		''' <exception cref="FileNotFoundException">
		'''          If the given file name does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file </exception>
		Public Sub New(ByVal fileName As String)
			Me.New(Locale.getDefault(Locale.Category.FORMAT), New java.io.BufferedWriter(New java.io.OutputStreamWriter(New java.io.FileOutputStream(fileName))))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file name and charset.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this
		'''         formatter.  If the file exists then it will be truncated to
		'''         zero size; otherwise, a new file will be created.  The output
		'''         will be written to the file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file name does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(fileName)} denies write
		'''          access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal fileName As String, ByVal csn As String)
			Me.New(fileName, csn, Locale.getDefault(Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file name, charset, and
		''' locale.
		''' </summary>
		''' <param name="fileName">
		'''         The name of the file to use as the destination of this
		'''         formatter.  If the file exists then it will be truncated to
		'''         zero size; otherwise, a new file will be created.  The output
		'''         will be written to the file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied.
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file name does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(fileName)} denies write
		'''          access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal fileName As String, ByVal csn As String, ByVal l As Locale)
			Me.New(toCharset(csn), l, New File(fileName))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file.
		''' 
		''' <p> The charset used is the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this formatter.  If the
		'''         file exists then it will be truncated to zero size; otherwise,
		'''         a new file will be created.  The output will be written to the
		'''         file and is buffered.
		''' </param>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		'''          write access to the file
		''' </exception>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file </exception>
		Public Sub New(ByVal file As java.io.File)
			Me.New(Locale.getDefault(Locale.Category.FORMAT), New java.io.BufferedWriter(New java.io.OutputStreamWriter(New java.io.FileOutputStream(file))))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file and charset.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this formatter.  If the
		'''         file exists then it will be truncated to zero size; otherwise,
		'''         a new file will be created.  The output will be written to the
		'''         file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		'''          write access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal file As java.io.File, ByVal csn As String)
			Me.New(file, csn, Locale.getDefault(Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified file, charset, and
		''' locale.
		''' </summary>
		''' <param name="file">
		'''         The file to use as the destination of this formatter.  If the
		'''         file exists then it will be truncated to zero size; otherwise,
		'''         a new file will be created.  The output will be written to the
		'''         file and is buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied.
		''' </param>
		''' <exception cref="FileNotFoundException">
		'''          If the given file object does not denote an existing, writable
		'''          regular file and a new regular file of that name cannot be
		'''          created, or if some other error occurs while opening or
		'''          creating the file
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager is present and {@link
		'''          SecurityManager#checkWrite checkWrite(file.getPath())} denies
		'''          write access to the file
		''' </exception>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal file As java.io.File, ByVal csn As String, ByVal l As Locale)
			Me.New(toCharset(csn), l, file)
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified print stream.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' 
		''' <p> Characters are written to the given {@link java.io.PrintStream
		''' PrintStream} object and are therefore encoded using that object's
		''' charset.
		''' </summary>
		''' <param name="ps">
		'''         The stream to use as the destination of this formatter. </param>
		Public Sub New(ByVal ps As java.io.PrintStream)
			Me.New(Locale.getDefault(Locale.Category.FORMAT), CType(Objects.requireNonNull(ps), Appendable))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified output stream.
		''' 
		''' <p> The charset used is the {@linkplain
		''' java.nio.charset.Charset#defaultCharset() default charset} for this
		''' instance of the Java virtual machine.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="os">
		'''         The output stream to use as the destination of this formatter.
		'''         The output will be buffered. </param>
		Public Sub New(ByVal os As java.io.OutputStream)
			Me.New(Locale.getDefault(Locale.Category.FORMAT), New java.io.BufferedWriter(New java.io.OutputStreamWriter(os)))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified output stream and
		''' charset.
		''' 
		''' <p> The locale used is the {@linkplain
		''' Locale#getDefault(Locale.Category) default locale} for
		''' <seealso cref="Locale.Category#FORMAT formatting"/> for this instance of the Java
		''' virtual machine.
		''' </summary>
		''' <param name="os">
		'''         The output stream to use as the destination of this formatter.
		'''         The output will be buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal os As java.io.OutputStream, ByVal csn As String)
			Me.New(os, csn, Locale.getDefault(Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Constructs a new formatter with the specified output stream, charset,
		''' and locale.
		''' </summary>
		''' <param name="os">
		'''         The output stream to use as the destination of this formatter.
		'''         The output will be buffered.
		''' </param>
		''' <param name="csn">
		'''         The name of a supported {@link java.nio.charset.Charset
		'''         charset}
		''' </param>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied.
		''' </param>
		''' <exception cref="UnsupportedEncodingException">
		'''          If the named charset is not supported </exception>
		Public Sub New(ByVal os As java.io.OutputStream, ByVal csn As String, ByVal l As Locale)
			Me.New(l, New java.io.BufferedWriter(New java.io.OutputStreamWriter(os, csn)))
		End Sub

		Private Shared Function getZero(ByVal l As Locale) As Char
			If (l IsNot Nothing) AndAlso (Not l.Equals(Locale.US)) Then
				Dim dfs As java.text.DecimalFormatSymbols = java.text.DecimalFormatSymbols.getInstance(l)
				Return dfs.zeroDigit
			Else
				Return "0"c
			End If
		End Function

		''' <summary>
		''' Returns the locale set by the construction of this formatter.
		''' 
		''' <p> The <seealso cref="#format(java.util.Locale,String,Object...) format"/> method
		''' for this object which has a locale argument does not change this value.
		''' </summary>
		''' <returns>  {@code null} if no localization is applied, otherwise a
		'''          locale
		''' </returns>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method </exception>
		Public Function locale() As Locale
			ensureOpen()
			Return l
		End Function

		''' <summary>
		''' Returns the destination for the output.
		''' </summary>
		''' <returns>  The destination for the output
		''' </returns>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method </exception>
		Public Function out() As Appendable
			ensureOpen()
			Return a
		End Function

		''' <summary>
		''' Returns the result of invoking {@code toString()} on the destination
		''' for the output.  For example, the following code formats text into a
		''' <seealso cref="StringBuilder"/> then retrieves the resultant string:
		''' 
		''' <blockquote><pre>
		'''   Formatter f = new Formatter();
		'''   f.format("Last reboot at %tc", lastRebootDate);
		'''   String s = f.toString();
		'''   // -&gt; s == "Last reboot at Sat Jan 01 00:00:00 PST 2000"
		''' </pre></blockquote>
		''' 
		''' <p> An invocation of this method behaves in exactly the same way as the
		''' invocation
		''' 
		''' <pre>
		'''     out().toString() </pre>
		''' 
		''' <p> Depending on the specification of {@code toString} for the {@link
		''' Appendable}, the returned string may or may not contain the characters
		''' written to the destination.  For instance, buffers typically return
		''' their contents in {@code toString()}, but streams cannot since the
		''' data is discarded.
		''' </summary>
		''' <returns>  The result of invoking {@code toString()} on the destination
		'''          for the output
		''' </returns>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method </exception>
		Public Overrides Function ToString() As String
			ensureOpen()
			Return a.ToString()
		End Function

		''' <summary>
		''' Flushes this formatter.  If the destination implements the {@link
		''' java.io.Flushable} interface, its {@code flush} method will be invoked.
		''' 
		''' <p> Flushing a formatter writes any buffered output in the destination
		''' to the underlying stream.
		''' </summary>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method </exception>
		Public Sub flush()
			ensureOpen()
			If TypeOf a Is java.io.Flushable Then
				Try
					CType(a, java.io.Flushable).flush()
				Catch ioe As java.io.IOException
					lastException = ioe
				End Try
			End If
		End Sub

		''' <summary>
		''' Closes this formatter.  If the destination implements the {@link
		''' java.io.Closeable} interface, its {@code close} method will be invoked.
		''' 
		''' <p> Closing a formatter allows it to release resources it may be holding
		''' (such as open files).  If the formatter is already closed, then invoking
		''' this method has no effect.
		''' 
		''' <p> Attempting to invoke any methods except <seealso cref="#ioException()"/> in
		''' this formatter after it has been closed will result in a {@link
		''' FormatterClosedException}.
		''' </summary>
		Public Sub close()
			If a Is Nothing Then Return
			Try
				If TypeOf a Is java.io.Closeable Then CType(a, java.io.Closeable).close()
			Catch ioe As java.io.IOException
				lastException = ioe
			Finally
				a = Nothing
			End Try
		End Sub

		Private Sub ensureOpen()
			If a Is Nothing Then Throw New FormatterClosedException
		End Sub

		''' <summary>
		''' Returns the {@code IOException} last thrown by this formatter's {@link
		''' Appendable}.
		''' 
		''' <p> If the destination's {@code append()} method never throws
		''' {@code IOException}, then this method will always return {@code null}.
		''' </summary>
		''' <returns>  The last exception thrown by the Appendable or {@code null} if
		'''          no such exception exists. </returns>
		Public Function ioException() As java.io.IOException
			Return lastException
		End Function

		''' <summary>
		''' Writes a formatted string to this object's destination using the
		''' specified format string and arguments.  The locale used is the one
		''' defined during the construction of this formatter.
		''' </summary>
		''' <param name="format">
		'''         A format string as described in <a href="#syntax">Format string
		'''         syntax</a>.
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <exception cref="IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a href="#detail">Details</a>
		'''          section of the formatter class specification.
		''' </exception>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method
		''' </exception>
		''' <returns>  This formatter </returns>
		Public Function format(ByVal format_Renamed As String, ParamArray ByVal args As Object()) As Formatter
			Return format(l, format_Renamed, args)
		End Function

		''' <summary>
		''' Writes a formatted string to this object's destination using the
		''' specified locale, format string, and arguments.
		''' </summary>
		''' <param name="l">
		'''         The <seealso cref="java.util.Locale locale"/> to apply during
		'''         formatting.  If {@code l} is {@code null} then no localization
		'''         is applied.  This does not change this object's locale that was
		'''         set during construction.
		''' </param>
		''' <param name="format">
		'''         A format string as described in <a href="#syntax">Format string
		'''         syntax</a>
		''' </param>
		''' <param name="args">
		'''         Arguments referenced by the format specifiers in the format
		'''         string.  If there are more arguments than format specifiers, the
		'''         extra arguments are ignored.  The maximum number of arguments is
		'''         limited by the maximum dimension of a Java array as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <exception cref="IllegalFormatException">
		'''          If a format string contains an illegal syntax, a format
		'''          specifier that is incompatible with the given arguments,
		'''          insufficient arguments given the format string, or other
		'''          illegal conditions.  For specification of all possible
		'''          formatting errors, see the <a href="#detail">Details</a>
		'''          section of the formatter class specification.
		''' </exception>
		''' <exception cref="FormatterClosedException">
		'''          If this formatter has been closed by invoking its {@link
		'''          #close()} method
		''' </exception>
		''' <returns>  This formatter </returns>
		Public Function format(ByVal l As Locale, ByVal format_Renamed As String, ParamArray ByVal args As Object()) As Formatter
			ensureOpen()

			' index of last argument referenced
			Dim last As Integer = -1
			' last ordinary index
			Dim lasto As Integer = -1

			Dim fsa As FormatString() = parse(format_Renamed)
			For i As Integer = 0 To fsa.Length - 1
				Dim fs As FormatString = fsa(i)
				Dim index As Integer = fs.index()
				Try
					Select Case index
					Case -2 ' fixed string, "%n", or "%%"
						fs.print(Nothing, l)
					Case -1 ' relative index
						If last < 0 OrElse (args IsNot Nothing AndAlso last > args.Length - 1) Then Throw New MissingFormatArgumentException(fs.ToString())
						fs.print((If(args Is Nothing, Nothing, args(last))), l)
					Case 0 ' ordinary index
						lasto += 1
						last = lasto
						If args IsNot Nothing AndAlso lasto > args.Length - 1 Then Throw New MissingFormatArgumentException(fs.ToString())
						fs.print((If(args Is Nothing, Nothing, args(lasto))), l)
					Case Else ' explicit index
						last = index - 1
						If args IsNot Nothing AndAlso last > args.Length - 1 Then Throw New MissingFormatArgumentException(fs.ToString())
						fs.print((If(args Is Nothing, Nothing, args(last))), l)
					End Select
				Catch x As java.io.IOException
					lastException = x
				End Try
			Next i
			Return Me
		End Function

		' %[argument_index$][flags][width][.precision][t]conversion
		Private Const formatSpecifier As String = "%(\d+\$)?([-#+ 0,(\<]*)?(\d+)?(\.\d+)?([tT])?([a-zA-Z%])"

		Private Shared fsPattern As java.util.regex.Pattern = java.util.regex.Pattern.compile(formatSpecifier)

		''' <summary>
		''' Finds format specifiers in the format string.
		''' </summary>
		Private Function parse(ByVal s As String) As FormatString()
			Dim al As New List(Of FormatString)
			Dim m As java.util.regex.Matcher = fsPattern.matcher(s)
			Dim i As Integer = 0
			Dim len As Integer = s.length()
			Do While i < len
				If m.find(i) Then
					' Anything between the start of the string and the beginning
					' of the format specifier is either fixed text or contains
					' an invalid format string.
					If m.start() <> i Then
						' Make sure we didn't miss any invalid format specifiers
						checkText(s, i, m.start())
						' Assume previous characters were fixed text
						al.add(New FixedString(Me, s.Substring(i, m.start() - i)))
					End If

					al.add(New FormatSpecifier(Me, m))
					i = m.end()
				Else
					' No more valid format specifiers.  Check for possible invalid
					' format specifiers.
					checkText(s, i, len)
					' The rest of the string is fixed text
					al.add(New FixedString(Me, s.Substring(i)))
					Exit Do
				End If
			Loop
			Return al.ToArray(New FormatString(al.size() - 1){})
		End Function

		Private Shared Sub checkText(ByVal s As String, ByVal start As Integer, ByVal [end] As Integer)
			For i As Integer = start To [end] - 1
				' Any '%' found in the region starts an invalid format specifier.
				If s.Chars(i) = "%"c Then
					Dim c As Char = If(i = [end] - 1, "%"c, s.Chars(i + 1))
					Throw New UnknownFormatConversionException(Convert.ToString(c))
				End If
			Next i
		End Sub

		Private Interface FormatString
			Function index() As Integer
			Sub print(ByVal arg As Object, ByVal l As Locale)
			Function ToString() As String
		End Interface

		Private Class FixedString
			Implements FormatString

			Private ReadOnly outerInstance As Formatter

			Private s As String
			Friend Sub New(ByVal outerInstance As Formatter, ByVal s As String)
					Me.outerInstance = outerInstance
				Me.s = s
			End Sub
			Public Overridable Function index() As Integer
				Return -2
			End Function
			Public Overridable Sub print(ByVal arg As Object, ByVal l As Locale)
				outerInstance.a.append(s)
			End Sub
			Public Overrides Function ToString() As String
				Return s
			End Function
		End Class

		''' <summary>
		''' Enum for {@code BigDecimal} formatting.
		''' </summary>
		Public Enum BigDecimalLayoutForm
			''' <summary>
			''' Format the {@code BigDecimal} in computerized scientific notation.
			''' </summary>
			SCIENTIFIC

			''' <summary>
			''' Format the {@code BigDecimal} as a decimal number.
			''' </summary>
			DECIMAL_FLOAT
		End Enum

		Private Class FormatSpecifier
			Implements FormatString

			Private ReadOnly outerInstance As Formatter

			Private index_Renamed As Integer = -1
			Private f As Flags = Flags.NONE
			Private width_Renamed As Integer
			Private precision_Renamed As Integer
			Private dt As Boolean = False
			Private c As Char

			Private Function index(ByVal s As String) As Integer
				If s IsNot Nothing Then
					Try
						index_Renamed = Convert.ToInt32(s.Substring(0, s.length() - 1))
					Catch x As NumberFormatException
						assert(False)
					End Try
				Else
					index_Renamed = 0
				End If
				Return index_Renamed
			End Function

			Public Overridable Function index() As Integer
				Return index_Renamed
			End Function

			Private Function flags(ByVal s As String) As Flags
				f = Flags.parse(s)
				If f.contains(Flags.PREVIOUS) Then index_Renamed = -1
				Return f
			End Function

			Friend Overridable Function flags() As Flags
				Return f
			End Function

			Private Function width(ByVal s As String) As Integer
				width_Renamed = -1
				If s IsNot Nothing Then
					Try
						width_Renamed = Convert.ToInt32(s)
						If width_Renamed < 0 Then Throw New IllegalFormatWidthException(width_Renamed)
					Catch x As NumberFormatException
						assert(False)
					End Try
				End If
				Return width_Renamed
			End Function

			Friend Overridable Function width() As Integer
				Return width_Renamed
			End Function

			Private Function precision(ByVal s As String) As Integer
				precision_Renamed = -1
				If s IsNot Nothing Then
					Try
						' remove the '.'
						precision_Renamed = Convert.ToInt32(s.Substring(1))
						If precision_Renamed < 0 Then Throw New IllegalFormatPrecisionException(precision_Renamed)
					Catch x As NumberFormatException
						assert(False)
					End Try
				End If
				Return precision_Renamed
			End Function

			Friend Overridable Function precision() As Integer
				Return precision_Renamed
			End Function

			Private Function conversion(ByVal s As String) As Char
				c = s.Chars(0)
				If Not dt Then
					If Not Conversion.isValid(c) Then Throw New UnknownFormatConversionException(Convert.ToString(c))
					If Char.IsUpper(c) Then f.add(Flags.UPPERCASE)
					c = Char.ToLower(c)
					If Conversion.isText(c) Then index_Renamed = -2
				End If
				Return c
			End Function

			Private Function conversion() As Char
				Return c
			End Function

			Friend Sub New(ByVal outerInstance As Formatter, ByVal m As java.util.regex.Matcher)
					Me.outerInstance = outerInstance
				Dim idx As Integer = 1

				index(m.group(idx))
				idx += 1
				flags(m.group(idx))
				idx += 1
				width(m.group(idx))
				idx += 1
				precision(m.group(idx))
				idx += 1

				Dim tT As String = m.group(idx)
				idx += 1
				If tT IsNot Nothing Then
					dt = True
					If tT.Equals("T") Then f.add(Flags.UPPERCASE)
				End If

				conversion(m.group(idx))

				If dt Then
					checkDateTime()
				ElseIf Conversion.isGeneral(c) Then
					checkGeneral()
				ElseIf Conversion.isCharacter(c) Then
					checkCharacter()
				ElseIf Conversion.isInteger(c) Then
					checkInteger()
				ElseIf Conversion.isFloat(c) Then
					checkFloat()
				ElseIf Conversion.isText(c) Then
					checkText()
				Else
					Throw New UnknownFormatConversionException(Convert.ToString(c))
				End If
			End Sub

			Public Overridable Sub print(ByVal arg As Object, ByVal l As Locale)
				If dt Then
					printDateTime(arg, l)
					Return
				End If
				Select Case c
				Case Conversion.DECIMAL_INTEGER, Conversion.OCTAL_INTEGER, Conversion.HEXADECIMAL_INTEGER
					printInteger(arg, l)
				Case Conversion.SCIENTIFIC, Conversion.GENERAL, Conversion.DECIMAL_FLOAT, Conversion.HEXADECIMAL_FLOAT
					printFloat(arg, l)
				Case Conversion.CHARACTER, Conversion.CHARACTER_UPPER
					printCharacter(arg)
				Case Conversion.BOOLEAN
					printBoolean(arg)
				Case Conversion.STRING
					printString(arg, l)
				Case Conversion.HASHCODE
					printHashCode(arg)
				Case Conversion.LINE_SEPARATOR
					outerInstance.a.append(System.lineSeparator())
				Case Conversion.PERCENT_SIGN
					outerInstance.a.append("%"c)
				Case Else
					Debug.Assert(False)
				End Select
			End Sub

			Private Sub printInteger(ByVal arg As Object, ByVal l As Locale)
				If arg Is Nothing Then
					print("null")
				ElseIf TypeOf arg Is Byte Then
					print(CByte(arg), l)
				ElseIf TypeOf arg Is Short? Then
					print(CShort(Fix(arg)), l)
				ElseIf TypeOf arg Is Integer? Then
					print(CInt(Fix(arg)), l)
				ElseIf TypeOf arg Is Long? Then
					print(CLng(Fix(arg)), l)
				ElseIf TypeOf arg Is System.Numerics.BigInteger Then
					print((CType(arg, System.Numerics.BigInteger)), l)
				Else
					failConversion(c, arg)
				End If
			End Sub

			Private Sub printFloat(ByVal arg As Object, ByVal l As Locale)
				If arg Is Nothing Then
					print("null")
				ElseIf TypeOf arg Is Float Then
					print(CSng(arg), l)
				ElseIf TypeOf arg Is Double? Then
					print(CDbl(arg), l)
				ElseIf TypeOf arg Is Decimal Then
					print((CDec(arg)), l)
				Else
					failConversion(c, arg)
				End If
			End Sub

			Private Sub printDateTime(ByVal arg As Object, ByVal l As Locale)
				If arg Is Nothing Then
					print("null")
					Return
				End If
				Dim cal As Calendar = Nothing

				' Instead of Calendar.setLenient(true), perhaps we should
				' wrap the IllegalArgumentException that might be thrown?
				If TypeOf arg Is Long? Then
					' Note that the following method uses an instance of the
					' default time zone (TimeZone.getDefaultRef().
					cal = Calendar.getInstance(If(l Is Nothing, Locale.US, l))
					cal.timeInMillis = CLng(Fix(arg))
				ElseIf TypeOf arg Is Date Then
					' Note that the following method uses an instance of the
					' default time zone (TimeZone.getDefaultRef().
					cal = Calendar.getInstance(If(l Is Nothing, Locale.US, l))
					cal.time = CDate(arg)
				ElseIf TypeOf arg Is Calendar Then
					cal = CType(CType(arg, Calendar).clone(), Calendar)
					cal.lenient = True
				ElseIf TypeOf arg Is java.time.temporal.TemporalAccessor Then
					print(CType(arg, java.time.temporal.TemporalAccessor), c, l)
					Return
				Else
					failConversion(c, arg)
				End If
				' Use the provided locale so that invocations of
				' localizedMagnitude() use optimizations for null.
				print(cal, c, l)
			End Sub

			Private Sub printCharacter(ByVal arg As Object)
				If arg Is Nothing Then
					print("null")
					Return
				End If
				Dim s As String = Nothing
				If TypeOf arg Is Character Then
					s = CChar(arg).ToString()
				ElseIf TypeOf arg Is Byte Then
					Dim i As SByte = CByte(arg)
					If Character.isValidCodePoint(i) Then
						s = New String(Character.toChars(i))
					Else
						Throw New IllegalFormatCodePointException(i)
					End If
				ElseIf TypeOf arg Is Short? Then
					Dim i As Short = CShort(Fix(arg))
					If Character.isValidCodePoint(i) Then
						s = New String(Character.toChars(i))
					Else
						Throw New IllegalFormatCodePointException(i)
					End If
				ElseIf TypeOf arg Is Integer? Then
					Dim i As Integer = CInt(Fix(arg))
					If Character.isValidCodePoint(i) Then
						s = New String(Character.toChars(i))
					Else
						Throw New IllegalFormatCodePointException(i)
					End If
				Else
					failConversion(c, arg)
				End If
				print(s)
			End Sub

			Private Sub printString(ByVal arg As Object, ByVal l As Locale)
				If TypeOf arg Is Formattable Then
					Dim fmt As Formatter = Formatter.this
					If fmt.locale() IsNot l Then fmt = New Formatter(fmt.out(), l)
					CType(arg, Formattable).formatTo(fmt, f.valueOf(), width_Renamed, precision_Renamed)
				Else
					If f.contains(Flags.ALTERNATE) Then failMismatch(Flags.ALTERNATE, "s"c)
					If arg Is Nothing Then
						print("null")
					Else
						print(arg.ToString())
					End If
				End If
			End Sub

			Private Sub printBoolean(ByVal arg As Object)
				Dim s As String
				If arg IsNot Nothing Then
					s = (If(TypeOf arg Is Boolean?, CBool(arg).ToString(), Convert.ToString(True)))
				Else
					s = Convert.ToString(False)
				End If
				print(s)
			End Sub

			Private Sub printHashCode(ByVal arg As Object)
				Dim s As String = (If(arg Is Nothing, "null",  java.lang.[Integer].toHexString(arg.GetHashCode())))
				print(s)
			End Sub

			Private Sub print(ByVal s As String)
				If precision_Renamed <> -1 AndAlso precision_Renamed < s.length() Then s = s.Substring(0, precision_Renamed)
				If f.contains(Flags.UPPERCASE) Then s = s.ToUpper()
				outerInstance.a.append(justify(s))
			End Sub

			Private Function justify(ByVal s As String) As String
				If width_Renamed = -1 Then Return s
				Dim sb As New StringBuilder
				Dim pad As Boolean = f.contains(Flags.LEFT_JUSTIFY)
				Dim sp As Integer = width_Renamed - s.length()
				If Not pad Then
					For i As Integer = 0 To sp - 1
						sb.append(" "c)
					Next i
				End If
				sb.append(s)
				If pad Then
					For i As Integer = 0 To sp - 1
						sb.append(" "c)
					Next i
				End If
				Return sb.ToString()
			End Function

			Public Overrides Function ToString() As String
				Dim sb As New StringBuilder("%")
				' Flags.UPPERCASE is set internally for legal conversions.
				Dim dupf As Flags = f.dup().remove(Flags.UPPERCASE)
				sb.append(dupf.ToString())
				If index_Renamed > 0 Then sb.append(index_Renamed).append("$"c)
				If width_Renamed <> -1 Then sb.append(width_Renamed)
				If precision_Renamed <> -1 Then sb.append("."c).append(precision_Renamed)
				If dt Then sb.append(If(f.contains(Flags.UPPERCASE), "T"c, "t"c))
				sb.append(If(f.contains(Flags.UPPERCASE), Char.ToUpper(c), c))
				Return sb.ToString()
			End Function

			Private Sub checkGeneral()
				If (c = Conversion.BOOLEAN OrElse c = Conversion.HASHCODE) AndAlso f.contains(Flags.ALTERNATE) Then failMismatch(Flags.ALTERNATE, c)
				' '-' requires a width
				If width_Renamed = -1 AndAlso f.contains(Flags.LEFT_JUSTIFY) Then Throw New MissingFormatWidthException(ToString())
				checkBadFlags(Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES)
			End Sub

			Private Sub checkDateTime()
				If precision_Renamed <> -1 Then Throw New IllegalFormatPrecisionException(precision_Renamed)
				If Not DateTime.isValid(c) Then Throw New UnknownFormatConversionException("t" & AscW(c))
				checkBadFlags(Flags.ALTERNATE, Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES)
				' '-' requires a width
				If width_Renamed = -1 AndAlso f.contains(Flags.LEFT_JUSTIFY) Then Throw New MissingFormatWidthException(ToString())
			End Sub

			Private Sub checkCharacter()
				If precision_Renamed <> -1 Then Throw New IllegalFormatPrecisionException(precision_Renamed)
				checkBadFlags(Flags.ALTERNATE, Flags.PLUS, Flags.LEADING_SPACE, Flags.ZERO_PAD, Flags.GROUP, Flags.PARENTHESES)
				' '-' requires a width
				If width_Renamed = -1 AndAlso f.contains(Flags.LEFT_JUSTIFY) Then Throw New MissingFormatWidthException(ToString())
			End Sub

			Private Sub checkInteger()
				checkNumeric()
				If precision_Renamed <> -1 Then Throw New IllegalFormatPrecisionException(precision_Renamed)

				If c = Conversion.DECIMAL_INTEGER Then
					checkBadFlags(Flags.ALTERNATE)
				ElseIf c = Conversion.OCTAL_INTEGER Then
					checkBadFlags(Flags.GROUP)
				Else
					checkBadFlags(Flags.GROUP)
				End If
			End Sub

			Private Sub checkBadFlags(ParamArray ByVal badFlags As Flags())
				For i As Integer = 0 To badFlags.Length - 1
					If f.contains(badFlags(i)) Then failMismatch(badFlags(i), c)
				Next i
			End Sub

			Private Sub checkFloat()
				checkNumeric()
				If c = Conversion.DECIMAL_FLOAT Then
				ElseIf c = Conversion.HEXADECIMAL_FLOAT Then
					checkBadFlags(Flags.PARENTHESES, Flags.GROUP)
				ElseIf c = Conversion.SCIENTIFIC Then
					checkBadFlags(Flags.GROUP)
				ElseIf c = Conversion.GENERAL Then
					checkBadFlags(Flags.ALTERNATE)
				End If
			End Sub

			Private Sub checkNumeric()
				If width_Renamed <> -1 AndAlso width_Renamed < 0 Then Throw New IllegalFormatWidthException(width_Renamed)

				If precision_Renamed <> -1 AndAlso precision_Renamed < 0 Then Throw New IllegalFormatPrecisionException(precision_Renamed)

				' '-' and '0' require a width
				If width_Renamed = -1 AndAlso (f.contains(Flags.LEFT_JUSTIFY) OrElse f.contains(Flags.ZERO_PAD)) Then Throw New MissingFormatWidthException(ToString())

				' bad combination
				If (f.contains(Flags.PLUS) AndAlso f.contains(Flags.LEADING_SPACE)) OrElse (f.contains(Flags.LEFT_JUSTIFY) AndAlso f.contains(Flags.ZERO_PAD)) Then Throw New IllegalFormatFlagsException(f.ToString())
			End Sub

			Private Sub checkText()
				If precision_Renamed <> -1 Then Throw New IllegalFormatPrecisionException(precision_Renamed)
				Select Case c
				Case Conversion.PERCENT_SIGN
					If f.valueOf() <> Flags.LEFT_JUSTIFY.valueOf() AndAlso f.valueOf() <> Flags.NONE.valueOf() Then Throw New IllegalFormatFlagsException(f.ToString())
					' '-' requires a width
					If width_Renamed = -1 AndAlso f.contains(Flags.LEFT_JUSTIFY) Then Throw New MissingFormatWidthException(ToString())
				Case Conversion.LINE_SEPARATOR
					If width_Renamed <> -1 Then Throw New IllegalFormatWidthException(width_Renamed)
					If f.valueOf() <> Flags.NONE.valueOf() Then Throw New IllegalFormatFlagsException(f.ToString())
				Case Else
					Debug.Assert(False)
				End Select
			End Sub

			Private Sub print(ByVal value As SByte, ByVal l As Locale)
				Dim v As Long = value
				If value < 0 AndAlso (c = Conversion.OCTAL_INTEGER OrElse c = Conversion.HEXADECIMAL_INTEGER) Then
					v += (1L << 8)
					Debug.Assert(v >= 0, v)
				End If
				print(v, l)
			End Sub

			Private Sub print(ByVal value As Short, ByVal l As Locale)
				Dim v As Long = value
				If value < 0 AndAlso (c = Conversion.OCTAL_INTEGER OrElse c = Conversion.HEXADECIMAL_INTEGER) Then
					v += (1L << 16)
					Debug.Assert(v >= 0, v)
				End If
				print(v, l)
			End Sub

			Private Sub print(ByVal value As Integer, ByVal l As Locale)
				Dim v As Long = value
				If value < 0 AndAlso (c = Conversion.OCTAL_INTEGER OrElse c = Conversion.HEXADECIMAL_INTEGER) Then
					v += (1L << 32)
					Debug.Assert(v >= 0, v)
				End If
				print(v, l)
			End Sub

			Private Sub print(ByVal value As Long, ByVal l As Locale)

				Dim sb As New StringBuilder

				If c = Conversion.DECIMAL_INTEGER Then
					Dim neg As Boolean = value < 0
					Dim va As Char()
					If value < 0 Then
						va = Convert.ToString(value, 10).Substring(1).ToCharArray()
					Else
						va = Convert.ToString(value, 10).ToCharArray()
					End If

					' leading sign indicator
					leadingSign(sb, neg)

					' the value
					localizedMagnitude(sb, va, f, adjustWidth(width_Renamed, f, neg), l)

					' trailing sign indicator
					trailingSign(sb, neg)
				ElseIf c = Conversion.OCTAL_INTEGER Then
					checkBadFlags(Flags.PARENTHESES, Flags.LEADING_SPACE, Flags.PLUS)
					Dim s As String = java.lang.[Long].toOctalString(value)
					Dim len As Integer = (If(f.contains(Flags.ALTERNATE), s.length() + 1, s.length()))

					' apply ALTERNATE (radix indicator for octal) before ZERO_PAD
					If f.contains(Flags.ALTERNATE) Then sb.append("0"c)
					If f.contains(Flags.ZERO_PAD) Then
						For i As Integer = 0 To width_Renamed - len - 1
							sb.append("0"c)
						Next i
					End If
					sb.append(s)
				ElseIf c = Conversion.HEXADECIMAL_INTEGER Then
					checkBadFlags(Flags.PARENTHESES, Flags.LEADING_SPACE, Flags.PLUS)
					Dim s As String = java.lang.[Long].toHexString(value)
					Dim len As Integer = (If(f.contains(Flags.ALTERNATE), s.length() + 2, s.length()))

					' apply ALTERNATE (radix indicator for hex) before ZERO_PAD
					If f.contains(Flags.ALTERNATE) Then sb.append(If(f.contains(Flags.UPPERCASE), "0X", "0x"))
					If f.contains(Flags.ZERO_PAD) Then
						For i As Integer = 0 To width_Renamed - len - 1
							sb.append("0"c)
						Next i
					End If
					If f.contains(Flags.UPPERCASE) Then s = s.ToUpper()
					sb.append(s)
				End If

				' justify based on width
				outerInstance.a.append(justify(sb.ToString()))
			End Sub

			' neg := val < 0
			Private Function leadingSign(ByVal sb As StringBuilder, ByVal neg As Boolean) As StringBuilder
				If Not neg Then
					If f.contains(Flags.PLUS) Then
						sb.append("+"c)
					ElseIf f.contains(Flags.LEADING_SPACE) Then
						sb.append(" "c)
					End If
				Else
					If f.contains(Flags.PARENTHESES) Then
						sb.append("("c)
					Else
						sb.append("-"c)
					End If
				End If
				Return sb
			End Function

			' neg := val < 0
			Private Function trailingSign(ByVal sb As StringBuilder, ByVal neg As Boolean) As StringBuilder
				If neg AndAlso f.contains(Flags.PARENTHESES) Then sb.append(")"c)
				Return sb
			End Function

			Private Sub print(ByVal value As System.Numerics.BigInteger, ByVal l As Locale)
				Dim sb As New StringBuilder
				Dim neg As Boolean = value.signum() = -1
				Dim v As System.Numerics.BigInteger = value.abs()

				' leading sign indicator
				leadingSign(sb, neg)

				' the value
				If c = Conversion.DECIMAL_INTEGER Then
					Dim va As Char() = v.ToString().ToCharArray()
					localizedMagnitude(sb, va, f, adjustWidth(width_Renamed, f, neg), l)
				ElseIf c = Conversion.OCTAL_INTEGER Then
					Dim s As String = v.ToString(8)

					Dim len As Integer = s.length() + sb.length()
					If neg AndAlso f.contains(Flags.PARENTHESES) Then len += 1

					' apply ALTERNATE (radix indicator for octal) before ZERO_PAD
					If f.contains(Flags.ALTERNATE) Then
						len += 1
						sb.append("0"c)
					End If
					If f.contains(Flags.ZERO_PAD) Then
						For i As Integer = 0 To width_Renamed - len - 1
							sb.append("0"c)
						Next i
					End If
					sb.append(s)
				ElseIf c = Conversion.HEXADECIMAL_INTEGER Then
					Dim s As String = v.ToString(16)

					Dim len As Integer = s.length() + sb.length()
					If neg AndAlso f.contains(Flags.PARENTHESES) Then len += 1

					' apply ALTERNATE (radix indicator for hex) before ZERO_PAD
					If f.contains(Flags.ALTERNATE) Then
						len += 2
						sb.append(If(f.contains(Flags.UPPERCASE), "0X", "0x"))
					End If
					If f.contains(Flags.ZERO_PAD) Then
						For i As Integer = 0 To width_Renamed - len - 1
							sb.append("0"c)
						Next i
					End If
					If f.contains(Flags.UPPERCASE) Then s = s.ToUpper()
					sb.append(s)
				End If

				' trailing sign indicator
				trailingSign(sb, (value.signum() = -1))

				' justify based on width
				outerInstance.a.append(justify(sb.ToString()))
			End Sub

			Private Sub print(ByVal value As Single, ByVal l As Locale)
				print(CDbl(value), l)
			End Sub

			Private Sub print(ByVal value As Double, ByVal l As Locale)
				Dim sb As New StringBuilder
				Dim neg As Boolean = java.lang.[Double].Compare(value, 0.0) = -1

				If Not java.lang.[Double].IsNaN(value) Then
					Dim v As Double = System.Math.Abs(value)

					' leading sign indicator
					leadingSign(sb, neg)

					' the value
					If Not java.lang.[Double].IsInfinity(v) Then
						print(sb, v, l, f, c, precision_Renamed, neg)
					Else
						sb.append(If(f.contains(Flags.UPPERCASE), "INFINITY", "Infinity"))
					End If

					' trailing sign indicator
					trailingSign(sb, neg)
				Else
					sb.append(If(f.contains(Flags.UPPERCASE), "NAN", "NaN"))
				End If

				' justify based on width
				outerInstance.a.append(justify(sb.ToString()))
			End Sub

			' !Double.isInfinite(value) && !Double.isNaN(value)
			Private Sub print(ByVal sb As StringBuilder, ByVal value As Double, ByVal l As Locale, ByVal f As Flags, ByVal c As Char, ByVal precision As Integer, ByVal neg As Boolean)
				If c = Conversion.SCIENTIFIC Then
					' Create a new FormattedFloatingDecimal with the desired
					' precision.
					Dim prec As Integer = (If(precision = -1, 6, precision))

					Dim fd As sun.misc.FormattedFloatingDecimal = sun.misc.FormattedFloatingDecimal.valueOf(value, prec, sun.misc.FormattedFloatingDecimal.Form.SCIENTIFIC)

					Dim mant As Char() = addZeros(fd.mantissa, prec)

					' If the precision is zero and the '#' flag is set, add the
					' requested decimal point.
					If f.contains(Flags.ALTERNATE) AndAlso (prec = 0) Then mant = addDot(mant)

					Dim exp As Char() = If(value = 0.0, New Char() {"+"c,"0"c,"0"c}, fd.exponent)

					Dim newW As Integer = width_Renamed
					If width_Renamed <> -1 Then newW = adjustWidth(width_Renamed - exp.Length - 1, f, neg)
					localizedMagnitude(sb, mant, f, newW, l)

					sb.append(If(f.contains(Flags.UPPERCASE), "E"c, "e"c))

					Dim flags_Renamed As Flags = f.dup().remove(Flags.GROUP)
					Dim sign As Char = exp(0)
					assert(sign = "+"c OrElse sign = "-"c)
					sb.append(sign)

					Dim tmp As Char() = New Char(exp.Length - 2){}
					Array.Copy(exp, 1, tmp, 0, exp.Length - 1)
					sb.append(localizedMagnitude(Nothing, tmp, flags_Renamed, -1, l))
				ElseIf c = Conversion.DECIMAL_FLOAT Then
					' Create a new FormattedFloatingDecimal with the desired
					' precision.
					Dim prec As Integer = (If(precision = -1, 6, precision))

					Dim fd As sun.misc.FormattedFloatingDecimal = sun.misc.FormattedFloatingDecimal.valueOf(value, prec, sun.misc.FormattedFloatingDecimal.Form.DECIMAL_FLOAT)

					Dim mant As Char() = addZeros(fd.mantissa, prec)

					' If the precision is zero and the '#' flag is set, add the
					' requested decimal point.
					If f.contains(Flags.ALTERNATE) AndAlso (prec = 0) Then mant = addDot(mant)

					Dim newW As Integer = width_Renamed
					If width_Renamed <> -1 Then newW = adjustWidth(width_Renamed, f, neg)
					localizedMagnitude(sb, mant, f, newW, l)
				ElseIf c = Conversion.GENERAL Then
					Dim prec As Integer = precision
					If precision = -1 Then
						prec = 6
					ElseIf precision = 0 Then
						prec = 1
					End If

					Dim exp As Char()
					Dim mant As Char()
					Dim expRounded As Integer
					If value = 0.0 Then
						exp = Nothing
						mant = New Char() {"0"c}
						expRounded = 0
					Else
						Dim fd As sun.misc.FormattedFloatingDecimal = sun.misc.FormattedFloatingDecimal.valueOf(value, prec, sun.misc.FormattedFloatingDecimal.Form.GENERAL)
						exp = fd.exponent
						mant = fd.mantissa
						expRounded = fd.exponentRounded
					End If

					If exp IsNot Nothing Then
						prec -= 1
					Else
						prec -= expRounded + 1
					End If

					mant = addZeros(mant, prec)
					' If the precision is zero and the '#' flag is set, add the
					' requested decimal point.
					If f.contains(Flags.ALTERNATE) AndAlso (prec = 0) Then mant = addDot(mant)

					Dim newW As Integer = width_Renamed
					If width_Renamed <> -1 Then
						If exp IsNot Nothing Then
							newW = adjustWidth(width_Renamed - exp.Length - 1, f, neg)
						Else
							newW = adjustWidth(width_Renamed, f, neg)
						End If
					End If
					localizedMagnitude(sb, mant, f, newW, l)

					If exp IsNot Nothing Then
						sb.append(If(f.contains(Flags.UPPERCASE), "E"c, "e"c))

						Dim flags_Renamed As Flags = f.dup().remove(Flags.GROUP)
						Dim sign As Char = exp(0)
						assert(sign = "+"c OrElse sign = "-"c)
						sb.append(sign)

						Dim tmp As Char() = New Char(exp.Length - 2){}
						Array.Copy(exp, 1, tmp, 0, exp.Length - 1)
						sb.append(localizedMagnitude(Nothing, tmp, flags_Renamed, -1, l))
					End If
				ElseIf c = Conversion.HEXADECIMAL_FLOAT Then
					Dim prec As Integer = precision
					If precision = -1 Then
						' assume that we want all of the digits
						prec = 0
					ElseIf precision = 0 Then
						prec = 1
					End If

					Dim s As String = hexDouble(value, prec)

					Dim va As Char()
					Dim upper As Boolean = f.contains(Flags.UPPERCASE)
					sb.append(If(upper, "0X", "0x"))

					If f.contains(Flags.ZERO_PAD) Then
						For i As Integer = 0 To width_Renamed - s.length() - 2 - 1
							sb.append("0"c)
						Next i
					End If

					Dim idx As Integer = s.IndexOf("p"c)
					va = s.Substring(0, idx).ToCharArray()
					If upper Then
						Dim tmp As New String(va)
						' don't localize hex
						tmp = tmp.ToUpper(Locale.US)
						va = tmp.ToCharArray()
					End If
					sb.append(If(prec <> 0, addZeros(va, prec), va))
					sb.append(If(upper, "P"c, "p"c))
					sb.append(s.Substring(idx+1))
				End If
			End Sub

			' Add zeros to the requested precision.
			Private Function addZeros(ByVal v As Char(), ByVal prec As Integer) As Char()
				' Look for the dot.  If we don't find one, the we'll need to add
				' it before we add the zeros.
				Dim i As Integer
				For i = 0 To v.Length - 1
					If v(i) = "."c Then Exit For
				Next i
				Dim needDot As Boolean = False
				If i = v.Length Then needDot = True

				' Determine existing precision.
				Dim outPrec As Integer = v.Length - i - (If(needDot, 0, 1))
				assert(outPrec <= prec)
				If outPrec = prec Then Return v

				' Create new array with existing contents.
				Dim tmp As Char() = New Char(v.Length + prec - outPrec + (If(needDot, 1, 0)) - 1){}
				Array.Copy(v, 0, tmp, 0, v.Length)

				' Add dot if previously determined to be necessary.
				Dim start As Integer = v.Length
				If needDot Then
					tmp(v.Length) = "."c
					start += 1
				End If

				' Add zeros.
				For j As Integer = start To tmp.Length - 1
					tmp(j) = "0"c
				Next j

				Return tmp
			End Function

			' Method assumes that d > 0.
			Private Function hexDouble(ByVal d As Double, ByVal prec As Integer) As String
				' Let java.lang.[Double].toHexString handle simple cases
				If (Not java.lang.[Double].isFinite(d)) OrElse d = 0.0 OrElse prec = 0 OrElse prec >= 13 Then
					' remove "0x"
					Return java.lang.[Double].toHexString(d).Substring(2)
				Else
					assert(prec >= 1 AndAlso prec <= 12)

					Dim exponent As Integer = System.Math.getExponent(d)
					Dim subnormal As Boolean = (exponent = sun.misc.DoubleConsts.MIN_EXPONENT - 1)

					' If this is subnormal input so normalize (could be faster to
					' do as integer operation).
					If subnormal Then
						scaleUp = System.Math.scalb(1.0, 54)
						d *= scaleUp
						' Calculate the exponent.  This is not just exponent + 54
						' since the former is not the normalized exponent.
						exponent = System.Math.getExponent(d)
						Debug.Assert(exponent >= sun.misc.DoubleConsts.MIN_EXPONENT AndAlso exponent <= sun.misc.DoubleConsts.MAX_EXPONENT, exponent)
					End If

					Dim precision As Integer = 1 + prec*4
					Dim shiftDistance As Integer = sun.misc.DoubleConsts.SIGNIFICAND_WIDTH - precision
					assert(shiftDistance >= 1 AndAlso shiftDistance < sun.misc.DoubleConsts.SIGNIFICAND_WIDTH)

					Dim doppel As Long = java.lang.[Double].doubleToLongBits(d)
					' Deterime the number of bits to keep.
					Dim newSignif As Long = (doppel And (sun.misc.DoubleConsts.EXP_BIT_MASK Or sun.misc.DoubleConsts.SIGNIF_BIT_MASK)) >> shiftDistance
					' Bits to round away.
					Dim roundingBits As Long = doppel And Not((Not 0L) << shiftDistance)

					' To decide how to round, look at the low-order bit of the
					' working significand, the highest order discarded bit (the
					' round bit) and whether any of the lower order discarded bits
					' are nonzero (the sticky bit).

					Dim leastZero As Boolean = (newSignif And &H1L) = 0L
					Dim round As Boolean = ((1L << (shiftDistance - 1)) And roundingBits) <> 0L
					Dim sticky As Boolean = shiftDistance > 1 AndAlso (Not(1L<< (shiftDistance - 1)) And roundingBits) <> 0
					If (leastZero AndAlso round AndAlso sticky) OrElse ((Not leastZero) AndAlso round) Then newSignif += 1

					Dim signBit As Long = doppel And sun.misc.DoubleConsts.SIGN_BIT_MASK
					newSignif = signBit Or (newSignif << shiftDistance)
					Dim result As Double = java.lang.[Double].longBitsToDouble(newSignif)

					If java.lang.[Double].IsInfinity(result) Then
						' Infinite result generated by rounding
						Return "1.0p1024"
					Else
						Dim res As String = java.lang.[Double].toHexString(result).Substring(2)
						If Not subnormal Then
							Return res
						Else
							' Create a normalized subnormal string.
							Dim idx As Integer = res.IndexOf("p"c)
							If idx = -1 Then
								' No 'p' character in hex string.
								Debug.Assert(False)
								Return Nothing
							Else
								' Get exponent and append at the end.
								Dim exp As String = res.Substring(idx + 1)
								Dim iexp As Integer = Convert.ToInt32(exp) -54
								Return res.Substring(0, idx) & "p" & Convert.ToString(iexp)
							End If
						End If
					End If
				End If
			End Function

			Private Sub print(ByVal value As Decimal, ByVal l As Locale)
				If c = Conversion.HEXADECIMAL_FLOAT Then failConversion(c, value)
				Dim sb As New StringBuilder
				Dim neg As Boolean = value.signum() = -1
				Dim v As Decimal = value.abs()
				' leading sign indicator
				leadingSign(sb, neg)

				' the value
				print(sb, v, l, f, c, precision_Renamed, neg)

				' trailing sign indicator
				trailingSign(sb, neg)

				' justify based on width
				outerInstance.a.append(justify(sb.ToString()))
			End Sub

			' value > 0
			Private Sub print(ByVal sb As StringBuilder, ByVal value As Decimal, ByVal l As Locale, ByVal f As Flags, ByVal c As Char, ByVal precision As Integer, ByVal neg As Boolean)
				If c = Conversion.SCIENTIFIC Then
					' Create a new BigDecimal with the desired precision.
					Dim prec As Integer = (If(precision = -1, 6, precision))
					Dim scale As Integer = value.scale()
					Dim origPrec As Integer = value.precision()
					Dim nzeros As Integer = 0
					Dim compPrec As Integer

					If prec > origPrec - 1 Then
						compPrec = origPrec
						nzeros = prec - (origPrec - 1)
					Else
						compPrec = prec + 1
					End If

					Dim mc As New java.math.MathContext(compPrec)
					Dim v As New Decimal(value.unscaledValue(), scale, mc)

					Dim bdl As New BigDecimalLayout(Me, v.unscaledValue(), v.scale(), BigDecimalLayoutForm.SCIENTIFIC)

					Dim mant As Char() = bdl.mantissa()

					' Add a decimal point if necessary.  The mantissa may not
					' contain a decimal point if the scale is zero (the internal
					' representation has no fractional part) or the original
					' precision is one. Append a decimal point if '#' is set or if
					' we require zero padding to get to the requested precision.
					If (origPrec = 1 OrElse (Not bdl.hasDot())) AndAlso (nzeros > 0 OrElse (f.contains(Flags.ALTERNATE))) Then mant = addDot(mant)

					' Add trailing zeros in the case precision is greater than
					' the number of available digits after the decimal separator.
					mant = trailingZeros(mant, nzeros)

					Dim exp As Char() = bdl.exponent()
					Dim newW As Integer = width_Renamed
					If width_Renamed <> -1 Then newW = adjustWidth(width_Renamed - exp.Length - 1, f, neg)
					localizedMagnitude(sb, mant, f, newW, l)

					sb.append(If(f.contains(Flags.UPPERCASE), "E"c, "e"c))

					Dim flags_Renamed As Flags = f.dup().remove(Flags.GROUP)
					Dim sign As Char = exp(0)
					assert(sign = "+"c OrElse sign = "-"c)
					sb.append(exp(0))

					Dim tmp As Char() = New Char(exp.Length - 2){}
					Array.Copy(exp, 1, tmp, 0, exp.Length - 1)
					sb.append(localizedMagnitude(Nothing, tmp, flags_Renamed, -1, l))
				ElseIf c = Conversion.DECIMAL_FLOAT Then
					' Create a new BigDecimal with the desired precision.
					Dim prec As Integer = (If(precision = -1, 6, precision))
					Dim scale As Integer = value.scale()

					If scale > prec Then
						' more "scale" digits than the requested "precision"
						Dim compPrec As Integer = value.precision()
						If compPrec <= scale Then
							' case of 0.xxxxxx
							value = value.scaleale(prec, java.math.RoundingMode.HALF_UP)
						Else
							compPrec -= (scale - prec)
							value = New Decimal(value.unscaledValue(), scale, New java.math.MathContext(compPrec))
						End If
					End If
					Dim bdl As New BigDecimalLayout(Me, value.unscaledValue(), value.scale(), BigDecimalLayoutForm.DECIMAL_FLOAT)

					Dim mant As Char() = bdl.mantissa()
					Dim nzeros As Integer = (If(bdl.scale() < prec, prec - bdl.scale(), 0))

					' Add a decimal point if necessary.  The mantissa may not
					' contain a decimal point if the scale is zero (the internal
					' representation has no fractional part).  Append a decimal
					' point if '#' is set or we require zero padding to get to the
					' requested precision.
					If bdl.scale() = 0 AndAlso (f.contains(Flags.ALTERNATE) OrElse nzeros > 0) Then mant = addDot(bdl.mantissa())

					' Add trailing zeros if the precision is greater than the
					' number of available digits after the decimal separator.
					mant = trailingZeros(mant, nzeros)

					localizedMagnitude(sb, mant, f, adjustWidth(width_Renamed, f, neg), l)
				ElseIf c = Conversion.GENERAL Then
					Dim prec As Integer = precision
					If precision = -1 Then
						prec = 6
					ElseIf precision = 0 Then
						prec = 1
					End If

					Dim tenToTheNegFour As Decimal = Decimal.valueOf(1, 4)
					Dim tenToThePrec As Decimal = Decimal.valueOf(1, -prec)
					If (value.Equals(Decimal.Zero)) OrElse ((value.CompareTo(tenToTheNegFour) <> -1) AndAlso (value.CompareTo(tenToThePrec) = -1)) Then

						Dim e As Integer = - value.scale() + (value.unscaledValue().ToString().Length - 1)

						' xxx.yyy
						'   g precision (# sig digits) = #x + #y
						'   f precision = #y
						'   exponent = #x - 1
						' => f precision = g precision - exponent - 1
						' 0.000zzz
						'   g precision (# sig digits) = #z
						'   f precision = #0 (after '.') + #z
						'   exponent = - #0 (after '.') - 1
						' => f precision = g precision - exponent - 1
						prec = prec - e - 1

						print(sb, value, l, f, Conversion.DECIMAL_FLOAT, prec, neg)
					Else
						print(sb, value, l, f, Conversion.SCIENTIFIC, prec - 1, neg)
					End If
				ElseIf c = Conversion.HEXADECIMAL_FLOAT Then
					' This conversion isn't supported.  The error should be
					' reported earlier.
					Debug.Assert(False)
				End If
			End Sub

			Private Class BigDecimalLayout
				Private ReadOnly outerInstance As Formatter.FormatSpecifier

				Private mant As StringBuilder
				Private exp As StringBuilder
				Private dot As Boolean = False
				Private scale_Renamed As Integer

				Public Sub New(ByVal outerInstance As Formatter.FormatSpecifier, ByVal intVal As System.Numerics.BigInteger, ByVal scale As Integer, ByVal form As BigDecimalLayoutForm)
						Me.outerInstance = outerInstance
					layout(intVal, scale, form)
				End Sub

				Public Overridable Function hasDot() As Boolean
					Return dot
				End Function

				Public Overridable Function scale() As Integer
					Return scale_Renamed
				End Function

				' char[] with canonical string representation
				Public Overridable Function layoutChars() As Char()
					Dim sb As New StringBuilder(mant)
					If exp IsNot Nothing Then
						sb.append("E"outerInstance.c)
						sb.append(exp)
					End If
					Return ToCharArray(sb)
				End Function

				Public Overridable Function mantissa() As Char()
					Return ToCharArray(mant)
				End Function

				' The exponent will be formatted as a sign ('+' or '-') followed
				' by the exponent zero-padded to include at least two digits.
				Public Overridable Function exponent() As Char()
					Return ToCharArray(exp)
				End Function

				Private Function toCharArray(ByVal sb As StringBuilder) As Char()
					If sb Is Nothing Then Return Nothing
					Dim result As Char() = New Char(sb.length() - 1){}
					sb.getChars(0, result.Length, result, 0)
					Return result
				End Function

				Private Sub layout(ByVal intVal As System.Numerics.BigInteger, ByVal scale As Integer, ByVal form As BigDecimalLayoutForm)
					Dim coeff As Char() = intVal.ToString().ToCharArray()
					Me.scale_Renamed = scale

					' Construct a buffer, with sufficient capacity for all cases.
					' If E-notation is needed, length will be: +1 if negative, +1
					' if '.' needed, +2 for "E+", + up to 10 for adjusted
					' exponent.  Otherwise it could have +1 if negative, plus
					' leading "0.00000"
					mant = New StringBuilder(coeff.Length + 14)

					If scale = 0 Then
						Dim len As Integer = coeff.Length
						If len > 1 Then
							mant.append(coeff(0))
							If form = BigDecimalLayoutForm.SCIENTIFIC Then
								mant.append("."outerInstance.c)
								dot = True
								mant.append(coeff, 1, len - 1)
								exp = New StringBuilder("+")
								If len < 10 Then
									exp.append("0").append(len - 1)
								Else
									exp.append(len - 1)
								End If
							Else
								mant.append(coeff, 1, len - 1)
							End If
						Else
							mant.append(coeff)
							If form = BigDecimalLayoutForm.SCIENTIFIC Then exp = New StringBuilder("+00")
						End If
						Return
					End If
					Dim adjusted As Long = -CLng(scale) + (coeff.Length - 1)
					If form = BigDecimalLayoutForm.DECIMAL_FLOAT Then
						' count of padding zeros
						Dim pad As Integer = scale - coeff.Length
						If pad >= 0 Then
							' 0.xxx form
							mant.append("0.")
							dot = True
							Do While pad > 0
								mant.append("0"outerInstance.c)
								pad -= 1
							Loop
							mant.append(coeff)
						Else
							If -pad < coeff.Length Then
								' xx.xx form
								mant.append(coeff, 0, -pad)
								mant.append("."outerInstance.c)
								dot = True
								mant.append(coeff, -pad, scale)
							Else
								' xx form
								mant.append(coeff, 0, coeff.Length)
								For i As Integer = 0 To -scale - 1
									mant.append("0"outerInstance.c)
								Next i
								Me.scale_Renamed = 0
							End If
						End If
					Else
						' x.xxx form
						mant.append(coeff(0))
						If coeff.Length > 1 Then
							mant.append("."outerInstance.c)
							dot = True
							mant.append(coeff, 1, coeff.Length-1)
						End If
						exp = New StringBuilder
						If adjusted <> 0 Then
							Dim abs As Long = System.Math.Abs(adjusted)
							' require sign
							exp.append(If(adjusted < 0, "-"outerInstance.c, "+"outerInstance.c))
							If abs < 10 Then exp.append("0"outerInstance.c)
							exp.append(abs)
						Else
							exp.append("+00")
						End If
					End If
				End Sub
			End Class

			Private Function adjustWidth(ByVal width As Integer, ByVal f As Flags, ByVal neg As Boolean) As Integer
				Dim newW As Integer = width
				If newW <> -1 AndAlso neg AndAlso f.contains(Flags.PARENTHESES) Then newW -= 1
				Return newW
			End Function

			' Add a '.' to th mantissa if required
			Private Function addDot(ByVal mant As Char()) As Char()
				Dim tmp As Char() = mant
				tmp = New Char(mant.Length){}
				Array.Copy(mant, 0, tmp, 0, mant.Length)
				tmp(tmp.Length - 1) = "."c
				Return tmp
			End Function

			' Add trailing zeros in the case precision is greater than the number
			' of available digits after the decimal separator.
			Private Function trailingZeros(ByVal mant As Char(), ByVal nzeros As Integer) As Char()
				Dim tmp As Char() = mant
				If nzeros > 0 Then
					tmp = New Char(mant.Length + nzeros - 1){}
					Array.Copy(mant, 0, tmp, 0, mant.Length)
					For i As Integer = mant.Length To tmp.Length - 1
						tmp(i) = "0"c
					Next i
				End If
				Return tmp
			End Function

			Private Sub print(ByVal t As Calendar, ByVal c As Char, ByVal l As Locale)
				Dim sb As New StringBuilder
				print(sb, t, c, l)

				' justify based on width
				Dim s As String = justify(sb.ToString())
				If f.contains(Flags.UPPERCASE) Then s = s.ToUpper()

				outerInstance.a.append(s)
			End Sub

			Private Function print(ByVal sb As StringBuilder, ByVal t As Calendar, ByVal c As Char, ByVal l As Locale) As Appendable
				If sb Is Nothing Then sb = New StringBuilder
				Select Case c
				Case DateTime.HOUR_OF_DAY_0, DateTime.HOUR_0, DateTime.HOUR_OF_DAY, DateTime.HOUR ' 'H' (00 - 23)
					Dim i As Integer = t.get(Calendar.HOUR_OF_DAY)
					If c = DateTime.HOUR_0 OrElse c = DateTime.HOUR Then i = (If(i = 0 OrElse i = 12, 12, i Mod 12))
					Dim flags_Renamed As Flags = (If(c = DateTime.HOUR_OF_DAY_0 OrElse c = DateTime.HOUR_0, Flags.ZERO_PAD, Flags.NONE))
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
					Exit Select
				Case DateTime.MINUTE ' 'M' (00 - 59)
					Dim i As Integer = t.get(Calendar.MINUTE)
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
					Exit Select
				Case DateTime.NANOSECOND ' 'N' (000000000 - 999999999)
					Dim i As Integer = t.get(Calendar.MILLISECOND) * 1000000
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 9, l))
					Exit Select
				Case DateTime.MILLISECOND ' 'L' (000 - 999)
					Dim i As Integer = t.get(Calendar.MILLISECOND)
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 3, l))
					Exit Select
				Case DateTime.MILLISECOND_SINCE_EPOCH ' 'Q' (0 - 99...?)
					Dim i As Long = t.timeInMillis
					Dim flags_Renamed As Flags = Flags.NONE
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, width_Renamed, l))
					Exit Select
				Case DateTime.AM_PM ' 'p' (am or pm)
					' Calendar.AM = 0, Calendar.PM = 1, LocaleElements defines upper
					Dim ampm As String() = { "AM", "PM" }
					If l IsNot Nothing AndAlso l IsNot Locale.US Then
						Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(l)
						ampm = dfs.amPmStrings
					End If
					Dim s As String = ampm(t.get(Calendar.AM_PM))
					sb.append(s.ToLower(If(l IsNot Nothing, l, Locale.US)))
					Exit Select
				Case DateTime.SECONDS_SINCE_EPOCH ' 's' (0 - 99...?)
					Dim i As Long = t.timeInMillis \ 1000
					Dim flags_Renamed As Flags = Flags.NONE
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, width_Renamed, l))
					Exit Select
				Case DateTime.SECOND ' 'S' (00 - 60 - leap second)
					Dim i As Integer = t.get(Calendar.SECOND)
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
					Exit Select
				Case DateTime.ZONE_NUMERIC ' 'z' ({-|+}####) - ls minus?
					Dim i As Integer = t.get(Calendar.ZONE_OFFSET) + t.get(Calendar.DST_OFFSET)
					Dim neg As Boolean = i < 0
					sb.append(If(neg, "-"c, "+"c))
					If neg Then i = -i
					Dim min As Integer = i \ 60000
					' combine minute and hour into a single integer
					Dim offset As Integer = (min \ 60) * 100 + (min Mod 60)
					Dim flags_Renamed As Flags = Flags.ZERO_PAD

					sb.append(localizedMagnitude(Nothing, offset, flags_Renamed, 4, l))
					Exit Select
				Case DateTime.ZONE ' 'Z' (symbol)
					Dim tz As TimeZone = t.timeZone
					sb.append(tz.getDisplayName((t.get(Calendar.DST_OFFSET) <> 0), TimeZone.SHORT,If(l Is Nothing, Locale.US, l)))
					Exit Select

				' Date
				Case DateTime.NAME_OF_DAY_ABBREV, DateTime.NAME_OF_DAY ' 'a'
					Dim i As Integer = t.get(Calendar.DAY_OF_WEEK)
					Dim lt As Locale = (If(l Is Nothing, Locale.US, l))
					Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(lt)
					If c = DateTime.NAME_OF_DAY Then
						sb.append(dfs.weekdays(i))
					Else
						sb.append(dfs.shortWeekdays(i))
					End If
					Exit Select
				Case DateTime.NAME_OF_MONTH_ABBREV, DateTime.NAME_OF_MONTH_ABBREV_X, DateTime.NAME_OF_MONTH ' 'b'
					Dim i As Integer = t.get(Calendar.MONTH)
					Dim lt As Locale = (If(l Is Nothing, Locale.US, l))
					Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(lt)
					If c = DateTime.NAME_OF_MONTH Then
						sb.append(dfs.months(i))
					Else
						sb.append(dfs.shortMonths(i))
					End If
					Exit Select
				Case DateTime.CENTURY, DateTime.YEAR_2, DateTime.YEAR_4 ' 'C' (00 - 99)
					Dim i As Integer = t.get(Calendar.YEAR)
					Dim size As Integer = 2
					Select Case c
					Case DateTime.CENTURY
						i \= 100
					Case DateTime.YEAR_2
						i = i Mod 100
					Case DateTime.YEAR_4
						size = 4
					End Select
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, size, l))
					Exit Select
				Case DateTime.DAY_OF_MONTH_0, DateTime.DAY_OF_MONTH ' 'd' (01 - 31)
					Dim i As Integer = t.get(Calendar.DATE)
					Dim flags_Renamed As Flags = (If(c = DateTime.DAY_OF_MONTH_0, Flags.ZERO_PAD, Flags.NONE))
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
					Exit Select
				Case DateTime.DAY_OF_YEAR ' 'j' (001 - 366)
					Dim i As Integer = t.get(Calendar.DAY_OF_YEAR)
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 3, l))
					Exit Select
				Case DateTime.MONTH ' 'm' (01 - 12)
					Dim i As Integer = t.get(Calendar.MONTH) + 1
					Dim flags_Renamed As Flags = Flags.ZERO_PAD
					sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
					Exit Select

				' Composites
				Case DateTime.TIME, DateTime.TIME_24_HOUR ' 'T' (24 hour hh:mm:ss - %tH:%tM:%tS)
					Dim sep As Char = ":"c
					print(sb, t, DateTime.HOUR_OF_DAY_0, l).append(sep)
					print(sb, t, DateTime.MINUTE, l)
					If c = DateTime.TIME Then
						sb.append(sep)
						print(sb, t, DateTime.SECOND, l)
					End If
					Exit Select
				Case DateTime.TIME_12_HOUR ' 'r' (hh:mm:ss [AP]M)
					Dim sep As Char = ":"c
					print(sb, t, DateTime.HOUR_0, l).append(sep)
					print(sb, t, DateTime.MINUTE, l).append(sep)
					print(sb, t, DateTime.SECOND, l).append(" "c)
					' this may be in wrong place for some locales
					Dim tsb As New StringBuilder
					print(tsb, t, DateTime.AM_PM, l)
					sb.append(tsb.ToString().ToUpper(If(l IsNot Nothing, l, Locale.US)))
					Exit Select
				Case DateTime.DATE_TIME ' 'c' (Sat Nov 04 12:02:33 EST 1999)
					Dim sep As Char = " "c
					print(sb, t, DateTime.NAME_OF_DAY_ABBREV, l).append(sep)
					print(sb, t, DateTime.NAME_OF_MONTH_ABBREV, l).append(sep)
					print(sb, t, DateTime.DAY_OF_MONTH_0, l).append(sep)
					print(sb, t, DateTime.TIME, l).append(sep)
					print(sb, t, DateTime.ZONE, l).append(sep)
					print(sb, t, DateTime.YEAR_4, l)
					Exit Select
				Case DateTime.DATE ' 'D' (mm/dd/yy)
					Dim sep As Char = "/"c
					print(sb, t, DateTime.MONTH, l).append(sep)
					print(sb, t, DateTime.DAY_OF_MONTH_0, l).append(sep)
					print(sb, t, DateTime.YEAR_2, l)
					Exit Select
				Case DateTime.ISO_STANDARD_DATE ' 'F' (%Y-%m-%d)
					Dim sep As Char = "-"c
					print(sb, t, DateTime.YEAR_4, l).append(sep)
					print(sb, t, DateTime.MONTH, l).append(sep)
					print(sb, t, DateTime.DAY_OF_MONTH_0, l)
					Exit Select
				Case Else
					Debug.Assert(False)
				End Select
				Return sb
			End Function

			Private Sub print(ByVal t As java.time.temporal.TemporalAccessor, ByVal c As Char, ByVal l As Locale)
				Dim sb As New StringBuilder
				print(sb, t, c, l)
				' justify based on width
				Dim s As String = justify(sb.ToString())
				If f.contains(Flags.UPPERCASE) Then s = s.ToUpper()
				outerInstance.a.append(s)
			End Sub

			Private Function print(ByVal sb As StringBuilder, ByVal t As java.time.temporal.TemporalAccessor, ByVal c As Char, ByVal l As Locale) As Appendable
				If sb Is Nothing Then sb = New StringBuilder
				Try
					Select Case c
					Case DateTime.HOUR_OF_DAY_0 ' 'H' (00 - 23)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.HOUR_OF_DAY)
						sb.append(localizedMagnitude(Nothing, i, Flags.ZERO_PAD, 2, l))
						Exit Select
					Case DateTime.HOUR_OF_DAY ' 'k' (0 - 23) -- like H
						Dim i As Integer = t.get(java.time.temporal.ChronoField.HOUR_OF_DAY)
						sb.append(localizedMagnitude(Nothing, i, Flags.NONE, 2, l))
						Exit Select
					Case DateTime.HOUR_0 ' 'I' (01 - 12)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.CLOCK_HOUR_OF_AMPM)
						sb.append(localizedMagnitude(Nothing, i, Flags.ZERO_PAD, 2, l))
						Exit Select
					Case DateTime.HOUR ' 'l' (1 - 12) -- like I
						Dim i As Integer = t.get(java.time.temporal.ChronoField.CLOCK_HOUR_OF_AMPM)
						sb.append(localizedMagnitude(Nothing, i, Flags.NONE, 2, l))
						Exit Select
					Case DateTime.MINUTE ' 'M' (00 - 59)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.MINUTE_OF_HOUR)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
						Exit Select
					Case DateTime.NANOSECOND ' 'N' (000000000 - 999999999)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.MILLI_OF_SECOND) * 1000000
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 9, l))
						Exit Select
					Case DateTime.MILLISECOND ' 'L' (000 - 999)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.MILLI_OF_SECOND)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 3, l))
						Exit Select
					Case DateTime.MILLISECOND_SINCE_EPOCH ' 'Q' (0 - 99...?)
						Dim i As Long = t.getLong(java.time.temporal.ChronoField.INSTANT_SECONDS) * 1000L + t.getLong(java.time.temporal.ChronoField.MILLI_OF_SECOND)
						Dim flags_Renamed As Flags = Flags.NONE
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, width_Renamed, l))
						Exit Select
					Case DateTime.AM_PM ' 'p' (am or pm)
						' Calendar.AM = 0, Calendar.PM = 1, LocaleElements defines upper
						Dim ampm As String() = { "AM", "PM" }
						If l IsNot Nothing AndAlso l IsNot Locale.US Then
							Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(l)
							ampm = dfs.amPmStrings
						End If
						Dim s As String = ampm(t.get(java.time.temporal.ChronoField.AMPM_OF_DAY))
						sb.append(s.ToLower(If(l IsNot Nothing, l, Locale.US)))
						Exit Select
					Case DateTime.SECONDS_SINCE_EPOCH ' 's' (0 - 99...?)
						Dim i As Long = t.getLong(java.time.temporal.ChronoField.INSTANT_SECONDS)
						Dim flags_Renamed As Flags = Flags.NONE
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, width_Renamed, l))
						Exit Select
					Case DateTime.SECOND ' 'S' (00 - 60 - leap second)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.SECOND_OF_MINUTE)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
						Exit Select
					Case DateTime.ZONE_NUMERIC ' 'z' ({-|+}####) - ls minus?
						Dim i As Integer = t.get(java.time.temporal.ChronoField.OFFSET_SECONDS)
						Dim neg As Boolean = i < 0
						sb.append(If(neg, "-"c, "+"c))
						If neg Then i = -i
						Dim min As Integer = i \ 60
						' combine minute and hour into a single integer
						Dim offset As Integer = (min \ 60) * 100 + (min Mod 60)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, offset, flags_Renamed, 4, l))
						Exit Select
					Case DateTime.ZONE ' 'Z' (symbol)
						Dim zid As java.time.ZoneId = t.query(java.time.temporal.TemporalQueries.zone())
						If zid Is Nothing Then Throw New IllegalFormatConversionException(c, t.GetType())
						If Not(TypeOf zid Is java.time.ZoneOffset) AndAlso t.isSupported(java.time.temporal.ChronoField.INSTANT_SECONDS) Then
							Dim instant As java.time.Instant = java.time.Instant.from(t)
							sb.append(TimeZone.getTimeZone(zid.id).getDisplayName(zid.rules.isDaylightSavings(instant), TimeZone.SHORT,If(l Is Nothing, Locale.US, l)))
							Exit Select
						End If
						sb.append(zid.id)
						Exit Select
					' Date
					Case DateTime.NAME_OF_DAY_ABBREV, DateTime.NAME_OF_DAY ' 'a'
						Dim i As Integer = t.get(java.time.temporal.ChronoField.DAY_OF_WEEK) Mod 7 + 1
						Dim lt As Locale = (If(l Is Nothing, Locale.US, l))
						Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(lt)
						If c = DateTime.NAME_OF_DAY Then
							sb.append(dfs.weekdays(i))
						Else
							sb.append(dfs.shortWeekdays(i))
						End If
						Exit Select
					Case DateTime.NAME_OF_MONTH_ABBREV, DateTime.NAME_OF_MONTH_ABBREV_X, DateTime.NAME_OF_MONTH ' 'b'
						Dim i As Integer = t.get(java.time.temporal.ChronoField.MONTH_OF_YEAR) - 1
						Dim lt As Locale = (If(l Is Nothing, Locale.US, l))
						Dim dfs As java.text.DateFormatSymbols = java.text.DateFormatSymbols.getInstance(lt)
						If c = DateTime.NAME_OF_MONTH Then
							sb.append(dfs.months(i))
						Else
							sb.append(dfs.shortMonths(i))
						End If
						Exit Select
					Case DateTime.CENTURY, DateTime.YEAR_2, DateTime.YEAR_4 ' 'C' (00 - 99)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.YEAR_OF_ERA)
						Dim size As Integer = 2
						Select Case c
						Case DateTime.CENTURY
							i \= 100
						Case DateTime.YEAR_2
							i = i Mod 100
						Case DateTime.YEAR_4
							size = 4
						End Select
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, size, l))
						Exit Select
					Case DateTime.DAY_OF_MONTH_0, DateTime.DAY_OF_MONTH ' 'd' (01 - 31)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.DAY_OF_MONTH)
						Dim flags_Renamed As Flags = (If(c = DateTime.DAY_OF_MONTH_0, Flags.ZERO_PAD, Flags.NONE))
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
						Exit Select
					Case DateTime.DAY_OF_YEAR ' 'j' (001 - 366)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.DAY_OF_YEAR)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 3, l))
						Exit Select
					Case DateTime.MONTH ' 'm' (01 - 12)
						Dim i As Integer = t.get(java.time.temporal.ChronoField.MONTH_OF_YEAR)
						Dim flags_Renamed As Flags = Flags.ZERO_PAD
						sb.append(localizedMagnitude(Nothing, i, flags_Renamed, 2, l))
						Exit Select

					' Composites
					Case DateTime.TIME, DateTime.TIME_24_HOUR ' 'T' (24 hour hh:mm:ss - %tH:%tM:%tS)
						Dim sep As Char = ":"c
						print(sb, t, DateTime.HOUR_OF_DAY_0, l).append(sep)
						print(sb, t, DateTime.MINUTE, l)
						If c = DateTime.TIME Then
							sb.append(sep)
							print(sb, t, DateTime.SECOND, l)
						End If
						Exit Select
					Case DateTime.TIME_12_HOUR ' 'r' (hh:mm:ss [AP]M)
						Dim sep As Char = ":"c
						print(sb, t, DateTime.HOUR_0, l).append(sep)
						print(sb, t, DateTime.MINUTE, l).append(sep)
						print(sb, t, DateTime.SECOND, l).append(" "c)
						' this may be in wrong place for some locales
						Dim tsb As New StringBuilder
						print(tsb, t, DateTime.AM_PM, l)
						sb.append(tsb.ToString().ToUpper(If(l IsNot Nothing, l, Locale.US)))
						Exit Select
					Case DateTime.DATE_TIME ' 'c' (Sat Nov 04 12:02:33 EST 1999)
						Dim sep As Char = " "c
						print(sb, t, DateTime.NAME_OF_DAY_ABBREV, l).append(sep)
						print(sb, t, DateTime.NAME_OF_MONTH_ABBREV, l).append(sep)
						print(sb, t, DateTime.DAY_OF_MONTH_0, l).append(sep)
						print(sb, t, DateTime.TIME, l).append(sep)
						print(sb, t, DateTime.ZONE, l).append(sep)
						print(sb, t, DateTime.YEAR_4, l)
						Exit Select
					Case DateTime.DATE ' 'D' (mm/dd/yy)
						Dim sep As Char = "/"c
						print(sb, t, DateTime.MONTH, l).append(sep)
						print(sb, t, DateTime.DAY_OF_MONTH_0, l).append(sep)
						print(sb, t, DateTime.YEAR_2, l)
						Exit Select
					Case DateTime.ISO_STANDARD_DATE ' 'F' (%Y-%m-%d)
						Dim sep As Char = "-"c
						print(sb, t, DateTime.YEAR_4, l).append(sep)
						print(sb, t, DateTime.MONTH, l).append(sep)
						print(sb, t, DateTime.DAY_OF_MONTH_0, l)
						Exit Select
					Case Else
						Debug.Assert(False)
					End Select
				Catch x As java.time.DateTimeException
					Throw New IllegalFormatConversionException(c, t.GetType())
				End Try
				Return sb
			End Function

			' -- Methods to support throwing exceptions --

			Private Sub failMismatch(ByVal f As Flags, ByVal c As Char)
				Dim fs As String = f.ToString()
				Throw New FormatFlagsConversionMismatchException(fs, c)
			End Sub

			Private Sub failConversion(ByVal c As Char, ByVal arg As Object)
				Throw New IllegalFormatConversionException(c, arg.GetType())
			End Sub

			Private Function getZero(ByVal l As Locale) As Char
				If (l IsNot Nothing) AndAlso (Not l.Equals(outerInstance.locale())) Then
					Dim dfs As java.text.DecimalFormatSymbols = java.text.DecimalFormatSymbols.getInstance(l)
					Return dfs.zeroDigit
				End If
				Return outerInstance.zero
			End Function

			Private Function localizedMagnitude(ByVal sb As StringBuilder, ByVal value As Long, ByVal f As Flags, ByVal width As Integer, ByVal l As Locale) As StringBuilder
				Dim va As Char() = Convert.ToString(value, 10).ToCharArray()
				Return localizedMagnitude(sb, va, f, width, l)
			End Function

			Private Function localizedMagnitude(ByVal sb As StringBuilder, ByVal value As Char(), ByVal f As Flags, ByVal width As Integer, ByVal l As Locale) As StringBuilder
				If sb Is Nothing Then sb = New StringBuilder
				Dim begin As Integer = sb.length()

				Dim zero_Renamed As Char = getZero(l)

				' determine localized grouping separator and size
				Dim grpSep As Char = ControlChars.NullChar
				Dim grpSize As Integer = -1
				Dim decSep As Char = ControlChars.NullChar

				Dim len As Integer = value.Length
				Dim dot As Integer = len
				For j As Integer = 0 To len - 1
					If value(j) = "."c Then
						dot = j
						Exit For
					End If
				Next j

				If dot < len Then
					If l Is Nothing OrElse l.Equals(Locale.US) Then
						decSep = "."c
					Else
						Dim dfs As java.text.DecimalFormatSymbols = java.text.DecimalFormatSymbols.getInstance(l)
						decSep = dfs.decimalSeparator
					End If
				End If

				If f.contains(Flags.GROUP) Then
					If l Is Nothing OrElse l.Equals(Locale.US) Then
						grpSep = ","c
						grpSize = 3
					Else
						Dim dfs As java.text.DecimalFormatSymbols = java.text.DecimalFormatSymbols.getInstance(l)
						grpSep = dfs.groupingSeparator
						Dim df As java.text.DecimalFormat = CType(java.text.NumberFormat.getIntegerInstance(l), java.text.DecimalFormat)
						grpSize = df.groupingSize
					End If
				End If

				' localize the digits inserting group separators as necessary
				For j As Integer = 0 To len - 1
					If j = dot Then
						sb.append(decSep)
						' no more group separators after the decimal separator
						grpSep = ControlChars.NullChar
						Continue For
					End If

					Dim c As Char = value(j)
					sb.append(ChrW((AscW(c) - AscW("0"c)) + AscW(zero_Renamed)))
					If grpSep <> ControlChars.NullChar AndAlso j <> dot - 1 AndAlso ((dot - j) Mod grpSize = 1) Then sb.append(grpSep)
				Next j

				' apply zero padding
				len = sb.length()
				If width <> -1 AndAlso f.contains(Flags.ZERO_PAD) Then
					For k As Integer = 0 To width - len - 1
						sb.insert(begin, zero_Renamed)
					Next k
				End If

				Return sb
			End Function
		End Class

		Private Class Flags
			Private flags_Renamed As Integer

			Friend Shared ReadOnly NONE As New Flags(0) ' ''

			' duplicate declarations from Formattable.java
			Friend Shared ReadOnly LEFT_JUSTIFY As New Flags(1<<0) ' '-'
			Friend Shared ReadOnly UPPERCASE As New Flags(1<<1) ' '^'
			Friend Shared ReadOnly ALTERNATE As New Flags(1<<2) ' '#'

			' numerics
			Friend Shared ReadOnly PLUS As New Flags(1<<3) ' '+'
			Friend Shared ReadOnly LEADING_SPACE As New Flags(1<<4) ' ' '
			Friend Shared ReadOnly ZERO_PAD As New Flags(1<<5) ' '0'
			Friend Shared ReadOnly GROUP As New Flags(1<<6) ' ','
			Friend Shared ReadOnly PARENTHESES As New Flags(1<<7) ' '('

			' indexing
			Friend Shared ReadOnly PREVIOUS As New Flags(1<<8) ' '<'

			Private Sub New(ByVal f As Integer)
				flags_Renamed = f
			End Sub

			Public Overridable Function valueOf() As Integer
				Return flags_Renamed
			End Function

			Public Overridable Function contains(ByVal f As Flags) As Boolean
				Return (flags_Renamed And f.valueOf()) = f.valueOf()
			End Function

			Public Overridable Function dup() As Flags
				Return New Flags(flags_Renamed)
			End Function

			Private Function add(ByVal f As Flags) As Flags
				flags_Renamed = flags_Renamed Or f.valueOf()
				Return Me
			End Function

			Public Overridable Function remove(ByVal f As Flags) As Flags
				flags_Renamed = flags_Renamed And Not f.valueOf()
				Return Me
			End Function

			Public Shared Function parse(ByVal s As String) As Flags
				Dim ca As Char() = s.ToCharArray()
				Dim f As New Flags(0)
				For i As Integer = 0 To ca.Length - 1
					Dim v As Flags = parse(ca(i))
					If f.contains(v) Then Throw New DuplicateFormatFlagsException(v.ToString())
					f.add(v)
				Next i
				Return f
			End Function

			' parse those flags which may be provided by users
			Private Shared Function parse(ByVal c As Char) As Flags
				Select Case c
				Case "-"c
					Return LEFT_JUSTIFY
				Case "#"c
					Return ALTERNATE
				Case "+"c
					Return PLUS
				Case " "c
					Return LEADING_SPACE
				Case "0"c
					Return ZERO_PAD
				Case ","c
					Return GROUP
				Case "("c
					Return PARENTHESES
				Case "<"c
					Return PREVIOUS
				Case Else
					Throw New UnknownFormatFlagsException(Convert.ToString(c))
				End Select
			End Function

			' Returns a string representation of the current {@code Flags}.
			Public Shared Function ToString(ByVal f As Flags) As String
				Return f.ToString()
			End Function

			Public Overrides Function ToString() As String
				Dim sb As New StringBuilder
				If contains(LEFT_JUSTIFY) Then sb.append("-"c)
				If contains(UPPERCASE) Then sb.append("^"c)
				If contains(ALTERNATE) Then sb.append("#"c)
				If contains(PLUS) Then sb.append("+"c)
				If contains(LEADING_SPACE) Then sb.append(" "c)
				If contains(ZERO_PAD) Then sb.append("0"c)
				If contains(GROUP) Then sb.append(","c)
				If contains(PARENTHESES) Then sb.append("("c)
				If contains(PREVIOUS) Then sb.append("<"c)
				Return sb.ToString()
			End Function
		End Class

		Private Class Conversion
			' Byte, Short, Integer, Long, BigInteger
			' (and associated primitives due to autoboxing)
			Friend Const DECIMAL_INTEGER As Char = "d"c
			Friend Const OCTAL_INTEGER As Char = "o"c
			Friend Const HEXADECIMAL_INTEGER As Char = "x"c
			Friend Const HEXADECIMAL_INTEGER_UPPER As Char = "X"c

			' Float, Double, BigDecimal
			' (and associated primitives due to autoboxing)
			Friend Const SCIENTIFIC As Char = "e"c
			Friend Const SCIENTIFIC_UPPER As Char = "E"c
			Friend Const GENERAL As Char = "g"c
			Friend Const GENERAL_UPPER As Char = "G"c
			Friend Const DECIMAL_FLOAT As Char = "f"c
			Friend Const HEXADECIMAL_FLOAT As Char = "a"c
			Friend Const HEXADECIMAL_FLOAT_UPPER As Char = "A"c

			' Character, Byte, Short, Integer
			' (and associated primitives due to autoboxing)
			Friend Const CHARACTER As Char = "c"c
			Friend Const CHARACTER_UPPER As Char = "C"c

			' java.util.Date, java.util.Calendar, long
			Friend Const DATE_TIME As Char = "t"c
			Friend Const DATE_TIME_UPPER As Char = "T"c

			' if (arg.TYPE != boolean) return boolean
			' if (arg != null) return true; else return false;
			Friend Const [BOOLEAN] As Char = "b"c
			Friend Const BOOLEAN_UPPER As Char = "B"c
			' if (arg instanceof Formattable) arg.formatTo()
			' else arg.toString();
			Friend Const [STRING] As Char = "s"c
			Friend Const STRING_UPPER As Char = "S"c
			' arg.hashCode()
			Friend Const HASHCODE As Char = "h"c
			Friend Const HASHCODE_UPPER As Char = "H"c

			Friend Const LINE_SEPARATOR As Char = "n"c
			Friend Const PERCENT_SIGN As Char = "%"c

			Friend Shared Function isValid(ByVal c As Char) As Boolean
				Return (isGeneral(c) OrElse isInteger(c) OrElse isFloat(c) OrElse isText(c) OrElse c = "t"c OrElse isCharacter(c))
			End Function

			' Returns true iff the Conversion is applicable to all objects.
			Friend Shared Function isGeneral(ByVal c As Char) As Boolean
				Select Case c
				Case [BOOLEAN], BOOLEAN_UPPER, [STRING], STRING_UPPER, HASHCODE, HASHCODE_UPPER
					Return True
				Case Else
					Return False
				End Select
			End Function

			' Returns true iff the Conversion is applicable to character.
			Friend Shared Function isCharacter(ByVal c As Char) As Boolean
				Select Case c
				Case CHARACTER, CHARACTER_UPPER
					Return True
				Case Else
					Return False
				End Select
			End Function

			' Returns true iff the Conversion is an integer type.
			Friend Shared Function isInteger(ByVal c As Char) As Boolean
				Select Case c
				Case DECIMAL_INTEGER, OCTAL_INTEGER, HEXADECIMAL_INTEGER, HEXADECIMAL_INTEGER_UPPER
					Return True
				Case Else
					Return False
				End Select
			End Function

			' Returns true iff the Conversion is a floating-point type.
			Friend Shared Function isFloat(ByVal c As Char) As Boolean
				Select Case c
				Case SCIENTIFIC, SCIENTIFIC_UPPER, GENERAL, GENERAL_UPPER, DECIMAL_FLOAT, HEXADECIMAL_FLOAT, HEXADECIMAL_FLOAT_UPPER
					Return True
				Case Else
					Return False
				End Select
			End Function

			' Returns true iff the Conversion does not require an argument
			Friend Shared Function isText(ByVal c As Char) As Boolean
				Select Case c
				Case LINE_SEPARATOR, PERCENT_SIGN
					Return True
				Case Else
					Return False
				End Select
			End Function
		End Class

		Private Class DateTime
			Friend Const HOUR_OF_DAY_0 As Char = "H"c ' (00 - 23)
			Friend Const HOUR_0 As Char = "I"c ' (01 - 12)
			Friend Const HOUR_OF_DAY As Char = "k"c ' (0 - 23) -- like H
			Friend Const HOUR As Char = "l"c ' (1 - 12) -- like I
			Friend Const MINUTE As Char = "M"c ' (00 - 59)
			Friend Const NANOSECOND As Char = "N"c ' (000000000 - 999999999)
			Friend Const MILLISECOND As Char = "L"c ' jdk, not in gnu (000 - 999)
			Friend Const MILLISECOND_SINCE_EPOCH As Char = "Q"c ' (0 - 99...?)
			Friend Const AM_PM As Char = "p"c ' (am or pm)
			Friend Const SECONDS_SINCE_EPOCH As Char = "s"c ' (0 - 99...?)
			Friend Const SECOND As Char = "S"c ' (00 - 60 - leap second)
			Friend Const TIME As Char = "T"c ' (24 hour hh:mm:ss)
			Friend Const ZONE_NUMERIC As Char = "z"c ' (-1200 - +1200) - ls minus?
			Friend Const ZONE As Char = "Z"c ' (symbol)

			' Date
			Friend Const NAME_OF_DAY_ABBREV As Char = "a"c ' 'a'
			Friend Const NAME_OF_DAY As Char = "A"c ' 'A'
			Friend Const NAME_OF_MONTH_ABBREV As Char = "b"c ' 'b'
			Friend Const NAME_OF_MONTH As Char = "B"c ' 'B'
			Friend Const CENTURY As Char = "C"c ' (00 - 99)
			Friend Const DAY_OF_MONTH_0 As Char = "d"c ' (01 - 31)
			Friend Const DAY_OF_MONTH As Char = "e"c ' (1 - 31) -- like d
	' *    static final char ISO_WEEK_OF_YEAR_2    = 'g'; // cross %y %V
	' *    static final char ISO_WEEK_OF_YEAR_4    = 'G'; // cross %Y %V
			Friend Const NAME_OF_MONTH_ABBREV_X As Char = "h"c ' -- same b
			Friend Const DAY_OF_YEAR As Char = "j"c ' (001 - 366)
			Friend Const MONTH As Char = "m"c ' (01 - 12)
	' *    static final char DAY_OF_WEEK_1         = 'u'; // (1 - 7) Monday
	' *    static final char WEEK_OF_YEAR_SUNDAY   = 'U'; // (0 - 53) Sunday+
	' *    static final char WEEK_OF_YEAR_MONDAY_01 = 'V'; // (01 - 53) Monday+
	' *    static final char DAY_OF_WEEK_0         = 'w'; // (0 - 6) Sunday
	' *    static final char WEEK_OF_YEAR_MONDAY   = 'W'; // (00 - 53) Monday
			Friend Const YEAR_2 As Char = "y"c ' (00 - 99)
			Friend Const YEAR_4 As Char = "Y"c ' (0000 - 9999)

			' Composites
			Friend Const TIME_12_HOUR As Char = "r"c ' (hh:mm:ss [AP]M)
			Friend Const TIME_24_HOUR As Char = "R"c ' (hh:mm same as %H:%M)
	' *    static final char LOCALE_TIME   = 'X'; // (%H:%M:%S) - parse format?
			Friend Const DATE_TIME As Char = "c"c
												' (Sat Nov 04 12:02:33 EST 1999)
			Friend Const [DATE] As Char = "D"c ' (mm/dd/yy)
			Friend Const ISO_STANDARD_DATE As Char = "F"c ' (%Y-%m-%d)
	' *    static final char LOCALE_DATE           = 'x'; // (mm/dd/yy)

			Friend Shared Function isValid(ByVal c As Char) As Boolean
				Select Case c
				Case HOUR_OF_DAY_0, HOUR_0, HOUR_OF_DAY, HOUR, MINUTE, NANOSECOND, MILLISECOND, MILLISECOND_SINCE_EPOCH, AM_PM, SECONDS_SINCE_EPOCH, SECOND, TIME, ZONE_NUMERIC, ZONE, NAME_OF_DAY_ABBREV, NAME_OF_DAY, NAME_OF_MONTH_ABBREV, NAME_OF_MONTH, CENTURY, DAY_OF_MONTH_0, DAY_OF_MONTH, NAME_OF_MONTH_ABBREV_X, DAY_OF_YEAR, MONTH, YEAR_2, YEAR_4, TIME_12_HOUR, TIME_24_HOUR, DATE_TIME, [DATE], ISO_STANDARD_DATE

				' Date
	' *        case ISO_WEEK_OF_YEAR_2:
	' *        case ISO_WEEK_OF_YEAR_4:
	' *        case DAY_OF_WEEK_1:
	' *        case WEEK_OF_YEAR_SUNDAY:
	' *        case WEEK_OF_YEAR_MONDAY_01:
	' *        case DAY_OF_WEEK_0:
	' *        case WEEK_OF_YEAR_MONDAY:

				' Composites
	' *        case LOCALE_TIME:
	' *        case LOCALE_DATE:
					Return True
				Case Else
					Return False
				End Select
			End Function
		End Class
	End Class

End Namespace