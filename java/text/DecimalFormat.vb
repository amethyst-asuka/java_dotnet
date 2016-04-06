Imports Microsoft.VisualBasic
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
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
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
	''' <code>DecimalFormat</code> is a concrete subclass of
	''' <code>NumberFormat</code> that formats decimal numbers. It has a variety of
	''' features designed to make it possible to parse and format numbers in any
	''' locale, including support for Western, Arabic, and Indic digits.  It also
	''' supports different kinds of numbers, including integers (123), fixed-point
	''' numbers (123.4), scientific notation (1.23E4), percentages (12%), and
	''' currency amounts ($123).  All of these can be localized.
	''' 
	''' <p>To obtain a <code>NumberFormat</code> for a specific locale, including the
	''' default locale, call one of <code>NumberFormat</code>'s factory methods, such
	''' as <code>getInstance()</code>.  In general, do not call the
	''' <code>DecimalFormat</code> constructors directly, since the
	''' <code>NumberFormat</code> factory methods may return subclasses other than
	''' <code>DecimalFormat</code>. If you need to customize the format object, do
	''' something like this:
	''' 
	''' <blockquote><pre>
	''' NumberFormat f = NumberFormat.getInstance(loc);
	''' if (f instanceof DecimalFormat) {
	'''     ((DecimalFormat) f).setDecimalSeparatorAlwaysShown(true);
	''' }
	''' </pre></blockquote>
	''' 
	''' <p>A <code>DecimalFormat</code> comprises a <em>pattern</em> and a set of
	''' <em>symbols</em>.  The pattern may be set directly using
	''' <code>applyPattern()</code>, or indirectly using the API methods.  The
	''' symbols are stored in a <code>DecimalFormatSymbols</code> object.  When using
	''' the <code>NumberFormat</code> factory methods, the pattern and symbols are
	''' read from localized <code>ResourceBundle</code>s.
	''' 
	''' <h3>Patterns</h3>
	''' 
	''' <code>DecimalFormat</code> patterns have the following syntax:
	''' <blockquote><pre>
	''' <i>Pattern:</i>
	'''         <i>PositivePattern</i>
	'''         <i>PositivePattern</i> ; <i>NegativePattern</i>
	''' <i>PositivePattern:</i>
	'''         <i>Prefix<sub>opt</sub></i> <i>Number</i> <i>Suffix<sub>opt</sub></i>
	''' <i>NegativePattern:</i>
	'''         <i>Prefix<sub>opt</sub></i> <i>Number</i> <i>Suffix<sub>opt</sub></i>
	''' <i>Prefix:</i>
	'''         any Unicode characters except &#92;uFFFE, &#92;uFFFF, and special characters
	''' <i>Suffix:</i>
	'''         any Unicode characters except &#92;uFFFE, &#92;uFFFF, and special characters
	''' <i>Number:</i>
	'''         <i>Integer</i> <i>Exponent<sub>opt</sub></i>
	'''         <i>Integer</i> . <i>Fraction</i> <i>Exponent<sub>opt</sub></i>
	''' <i>Integer:</i>
	'''         <i>MinimumInteger</i>
	'''         #
	'''         # <i>Integer</i>
	'''         # , <i>Integer</i>
	''' <i>MinimumInteger:</i>
	'''         0
	'''         0 <i>MinimumInteger</i>
	'''         0 , <i>MinimumInteger</i>
	''' <i>Fraction:</i>
	'''         <i>MinimumFraction<sub>opt</sub></i> <i>OptionalFraction<sub>opt</sub></i>
	''' <i>MinimumFraction:</i>
	'''         0 <i>MinimumFraction<sub>opt</sub></i>
	''' <i>OptionalFraction:</i>
	'''         # <i>OptionalFraction<sub>opt</sub></i>
	''' <i>Exponent:</i>
	'''         E <i>MinimumExponent</i>
	''' <i>MinimumExponent:</i>
	'''         0 <i>MinimumExponent<sub>opt</sub></i>
	''' </pre></blockquote>
	''' 
	''' <p>A <code>DecimalFormat</code> pattern contains a positive and negative
	''' subpattern, for example, <code>"#,##0.00;(#,##0.00)"</code>.  Each
	''' subpattern has a prefix, numeric part, and suffix. The negative subpattern
	''' is optional; if absent, then the positive subpattern prefixed with the
	''' localized minus sign (<code>'-'</code> in most locales) is used as the
	''' negative subpattern. That is, <code>"0.00"</code> alone is equivalent to
	''' <code>"0.00;-0.00"</code>.  If there is an explicit negative subpattern, it
	''' serves only to specify the negative prefix and suffix; the number of digits,
	''' minimal digits, and other characteristics are all the same as the positive
	''' pattern. That means that <code>"#,##0.0#;(#)"</code> produces precisely
	''' the same behavior as <code>"#,##0.0#;(#,##0.0#)"</code>.
	''' 
	''' <p>The prefixes, suffixes, and various symbols used for infinity, digits,
	''' thousands separators, decimal separators, etc. may be set to arbitrary
	''' values, and they will appear properly during formatting.  However, care must
	''' be taken that the symbols and strings do not conflict, or parsing will be
	''' unreliable.  For example, either the positive and negative prefixes or the
	''' suffixes must be distinct for <code>DecimalFormat.parse()</code> to be able
	''' to distinguish positive from negative values.  (If they are identical, then
	''' <code>DecimalFormat</code> will behave as if no negative subpattern was
	''' specified.)  Another example is that the decimal separator and thousands
	''' separator should be distinct characters, or parsing will be impossible.
	''' 
	''' <p>The grouping separator is commonly used for thousands, but in some
	''' countries it separates ten-thousands. The grouping size is a constant number
	''' of digits between the grouping characters, such as 3 for 100,000,000 or 4 for
	''' 1,0000,0000.  If you supply a pattern with multiple grouping characters, the
	''' interval between the last one and the end of the integer is the one that is
	''' used. So <code>"#,##,###,####"</code> == <code>"######,####"</code> ==
	''' <code>"##,####,####"</code>.
	''' 
	''' <h4>Special Pattern Characters</h4>
	''' 
	''' <p>Many characters in a pattern are taken literally; they are matched during
	''' parsing and output unchanged during formatting.  Special characters, on the
	''' other hand, stand for other characters, strings, or classes of characters.
	''' They must be quoted, unless noted otherwise, if they are to appear in the
	''' prefix or suffix as literals.
	''' 
	''' <p>The characters listed here are used in non-localized patterns.  Localized
	''' patterns use the corresponding characters taken from this formatter's
	''' <code>DecimalFormatSymbols</code> object instead, and these characters lose
	''' their special status.  Two exceptions are the currency sign and quote, which
	''' are not localized.
	''' 
	''' <blockquote>
	''' <table border=0 cellspacing=3 cellpadding=0 summary="Chart showing symbol,
	'''  location, localized, and meaning.">
	'''     <tr style="background-color: rgb(204, 204, 255);">
	'''          <th align=left>Symbol
	'''          <th align=left>Location
	'''          <th align=left>Localized?
	'''          <th align=left>Meaning
	'''     <tr valign=top>
	'''          <td><code>0</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Digit
	'''     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''          <td><code>#</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Digit, zero shows as absent
	'''     <tr valign=top>
	'''          <td><code>.</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Decimal separator or monetary decimal separator
	'''     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''          <td><code>-</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Minus sign
	'''     <tr valign=top>
	'''          <td><code>,</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Grouping separator
	'''     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''          <td><code>E</code>
	'''          <td>Number
	'''          <td>Yes
	'''          <td>Separates mantissa and exponent in scientific notation.
	'''              <em>Need not be quoted in prefix or suffix.</em>
	'''     <tr valign=top>
	'''          <td><code>;</code>
	'''          <td>Subpattern boundary
	'''          <td>Yes
	'''          <td>Separates positive and negative subpatterns
	'''     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''          <td><code>%</code>
	'''          <td>Prefix or suffix
	'''          <td>Yes
	'''          <td>Multiply by 100 and show as percentage
	'''     <tr valign=top>
	'''          <td><code>&#92;u2030</code>
	'''          <td>Prefix or suffix
	'''          <td>Yes
	'''          <td>Multiply by 1000 and show as per mille value
	'''     <tr style="vertical-align: top; background-color: rgb(238, 238, 255);">
	'''          <td><code>&#164;</code> (<code>&#92;u00A4</code>)
	'''          <td>Prefix or suffix
	'''          <td>No
	'''          <td>Currency sign, replaced by currency symbol.  If
	'''              doubled, replaced by international currency symbol.
	'''              If present in a pattern, the monetary decimal separator
	'''              is used instead of the decimal separator.
	'''     <tr valign=top>
	'''          <td><code>'</code>
	'''          <td>Prefix or suffix
	'''          <td>No
	'''          <td>Used to quote special characters in a prefix or suffix,
	'''              for example, <code>"'#'#"</code> formats 123 to
	'''              <code>"#123"</code>.  To create a single quote
	'''              itself, use two in a row: <code>"# o''clock"</code>.
	''' </table>
	''' </blockquote>
	''' 
	''' <h4>Scientific Notation</h4>
	''' 
	''' <p>Numbers in scientific notation are expressed as the product of a mantissa
	''' and a power of ten, for example, 1234 can be expressed as 1.234 x 10^3.  The
	''' mantissa is often in the range 1.0 &le; x {@literal <} 10.0, but it need not
	''' be.
	''' <code>DecimalFormat</code> can be instructed to format and parse scientific
	''' notation <em>only via a pattern</em>; there is currently no factory method
	''' that creates a scientific notation format.  In a pattern, the exponent
	''' character immediately followed by one or more digit characters indicates
	''' scientific notation.  Example: <code>"0.###E0"</code> formats the number
	''' 1234 as <code>"1.234E3"</code>.
	''' 
	''' <ul>
	''' <li>The number of digit characters after the exponent character gives the
	''' minimum exponent digit count.  There is no maximum.  Negative exponents are
	''' formatted using the localized minus sign, <em>not</em> the prefix and suffix
	''' from the pattern.  This allows patterns such as <code>"0.###E0 m/s"</code>.
	''' 
	''' <li>The minimum and maximum number of integer digits are interpreted
	''' together:
	''' 
	''' <ul>
	''' <li>If the maximum number of integer digits is greater than their minimum number
	''' and greater than 1, it forces the exponent to be a multiple of the maximum
	''' number of integer digits, and the minimum number of integer digits to be
	''' interpreted as 1.  The most common use of this is to generate
	''' <em>engineering notation</em>, in which the exponent is a multiple of three,
	''' e.g., <code>"##0.#####E0"</code>. Using this pattern, the number 12345
	''' formats to <code>"12.345E3"</code>, and 123456 formats to
	''' <code>"123.456E3"</code>.
	''' 
	''' <li>Otherwise, the minimum number of integer digits is achieved by adjusting the
	''' exponent.  Example: 0.00123 formatted with <code>"00.###E0"</code> yields
	''' <code>"12.3E-4"</code>.
	''' </ul>
	''' 
	''' <li>The number of significant digits in the mantissa is the sum of the
	''' <em>minimum integer</em> and <em>maximum fraction</em> digits, and is
	''' unaffected by the maximum integer digits.  For example, 12345 formatted with
	''' <code>"##0.##E0"</code> is <code>"12.3E3"</code>. To show all digits, set
	''' the significant digits count to zero.  The number of significant digits
	''' does not affect parsing.
	''' 
	''' <li>Exponential patterns may not contain grouping separators.
	''' </ul>
	''' 
	''' <h4>Rounding</h4>
	''' 
	''' <code>DecimalFormat</code> provides rounding modes defined in
	''' <seealso cref="java.math.RoundingMode"/> for formatting.  By default, it uses
	''' <seealso cref="java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN"/>.
	''' 
	''' <h4>Digits</h4>
	''' 
	''' For formatting, <code>DecimalFormat</code> uses the ten consecutive
	''' characters starting with the localized zero digit defined in the
	''' <code>DecimalFormatSymbols</code> object as digits. For parsing, these
	''' digits as well as all Unicode decimal digits, as defined by
	''' <seealso cref="Character#digit Character.digit"/>, are recognized.
	''' 
	''' <h4>Special Values</h4>
	''' 
	''' <p><code>NaN</code> is formatted as a string, which typically has a single character
	''' <code>&#92;uFFFD</code>.  This string is determined by the
	''' <code>DecimalFormatSymbols</code> object.  This is the only value for which
	''' the prefixes and suffixes are not used.
	''' 
	''' <p>Infinity is formatted as a string, which typically has a single character
	''' <code>&#92;u221E</code>, with the positive or negative prefixes and suffixes
	''' applied.  The infinity string is determined by the
	''' <code>DecimalFormatSymbols</code> object.
	''' 
	''' <p>Negative zero (<code>"-0"</code>) parses to
	''' <ul>
	''' <li><code>BigDecimal(0)</code> if <code>isParseBigDecimal()</code> is
	''' true,
	''' <li><code>Long(0)</code> if <code>isParseBigDecimal()</code> is false
	'''     and <code>isParseIntegerOnly()</code> is true,
	''' <li><code>Double(-0.0)</code> if both <code>isParseBigDecimal()</code>
	''' and <code>isParseIntegerOnly()</code> are false.
	''' </ul>
	''' 
	''' <h4><a name="synchronization">Synchronization</a></h4>
	''' 
	''' <p>
	''' Decimal formats are generally not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' 
	''' <h4>Example</h4>
	''' 
	''' <blockquote><pre>{@code
	''' <strong>// Print out a number using the localized number, integer, currency,
	''' // and percent format for each locale</strong>
	''' Locale[] locales = NumberFormat.getAvailableLocales();
	''' double myNumber = -1234.56;
	''' NumberFormat form;
	''' for (int j = 0; j < 4; ++j) {
	'''     System.out.println("FORMAT");
	'''     for (int i = 0; i < locales.length; ++i) {
	'''         if (locales[i].getCountry().length() == 0) {
	'''            continue; // Skip language-only locales
	'''         }
	'''         System.out.print(locales[i].getDisplayName());
	'''         switch (j) {
	'''         case 0:
	'''             form = NumberFormat.getInstance(locales[i]); break;
	'''         case 1:
	'''             form = NumberFormat.getIntegerInstance(locales[i]); break;
	'''         case 2:
	'''             form = NumberFormat.getCurrencyInstance(locales[i]); break;
	'''         default:
	'''             form = NumberFormat.getPercentInstance(locales[i]); break;
	'''         }
	'''         if (form instanceof DecimalFormat) {
	'''             System.out.print(": " + ((DecimalFormat) form).toPattern());
	'''         }
	'''         System.out.print(" -> " + form.format(myNumber));
	'''         try {
	'''             System.out.println(" -> " + form.parse(form.format(myNumber)));
	'''         } catch (ParseException e) {}
	'''     }
	''' }
	''' }</pre></blockquote>
	''' </summary>
	''' <seealso cref=          <a href="https://docs.oracle.com/javase/tutorial/i18n/format/decimalFormat.html">Java Tutorial</a> </seealso>
	''' <seealso cref=          NumberFormat </seealso>
	''' <seealso cref=          DecimalFormatSymbols </seealso>
	''' <seealso cref=          ParsePosition
	''' @author       Mark Davis
	''' @author       Alan Liu </seealso>
	Public Class DecimalFormat
		Inherits NumberFormat

		''' <summary>
		''' Creates a DecimalFormat using the default pattern and symbols
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' This is a convenient way to obtain a
		''' DecimalFormat when internationalization is not the main concern.
		''' <p>
		''' To obtain standard formats for a given locale, use the factory methods
		''' on NumberFormat such as getNumberInstance. These factories will
		''' return the most appropriate sub-class of NumberFormat for a given
		''' locale.
		''' </summary>
		''' <seealso cref= java.text.NumberFormat#getInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		Public Sub New()
			' Get the pattern for the default locale.
			Dim def As java.util.Locale = java.util.Locale.getDefault(java.util.Locale.Category.FORMAT)
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.NumberFormatProvider), def)
			If Not(TypeOf adapter Is sun.util.locale.provider.ResourceBundleBasedAdapter) Then adapter = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased
			Dim all As String() = adapter.getLocaleResources(def).numberPatterns

			' Always applyPattern after the symbols are set
			Me.symbols = DecimalFormatSymbols.getInstance(def)
			applyPattern(all(0), False)
		End Sub


		''' <summary>
		''' Creates a DecimalFormat using the given pattern and the symbols
		''' for the default <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' This is a convenient way to obtain a
		''' DecimalFormat when internationalization is not the main concern.
		''' <p>
		''' To obtain standard formats for a given locale, use the factory methods
		''' on NumberFormat such as getNumberInstance. These factories will
		''' return the most appropriate sub-class of NumberFormat for a given
		''' locale.
		''' </summary>
		''' <param name="pattern"> a non-localized pattern string. </param>
		''' <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		''' <seealso cref= java.text.NumberFormat#getInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		Public Sub New(  pattern As String)
			' Always applyPattern after the symbols are set
			Me.symbols = DecimalFormatSymbols.getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			applyPattern(pattern, False)
		End Sub


		''' <summary>
		''' Creates a DecimalFormat using the given pattern and symbols.
		''' Use this constructor when you need to completely customize the
		''' behavior of the format.
		''' <p>
		''' To obtain standard formats for a given
		''' locale, use the factory methods on NumberFormat such as
		''' getInstance or getCurrencyInstance. If you need only minor adjustments
		''' to a standard format, you can modify the format returned by
		''' a NumberFormat factory method.
		''' </summary>
		''' <param name="pattern"> a non-localized pattern string </param>
		''' <param name="symbols"> the set of symbols to be used </param>
		''' <exception cref="NullPointerException"> if any of the given arguments is null </exception>
		''' <exception cref="IllegalArgumentException"> if the given pattern is invalid </exception>
		''' <seealso cref= java.text.NumberFormat#getInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getNumberInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getCurrencyInstance </seealso>
		''' <seealso cref= java.text.NumberFormat#getPercentInstance </seealso>
		''' <seealso cref= java.text.DecimalFormatSymbols </seealso>
		Public Sub New(  pattern As String,   symbols As DecimalFormatSymbols)
			' Always applyPattern after the symbols are set
			Me.symbols = CType(symbols.clone(), DecimalFormatSymbols)
			applyPattern(pattern, False)
		End Sub


		' Overrides
		''' <summary>
		''' Formats a number and appends the resulting text to the given string
		''' buffer.
		''' The number can be of any subclass of <seealso cref="java.lang.Number"/>.
		''' <p>
		''' This implementation uses the maximum precision permitted. </summary>
		''' <param name="number">     the number to format </param>
		''' <param name="toAppendTo"> the <code>StringBuffer</code> to which the formatted
		'''                   text is to be appended </param>
		''' <param name="pos">        On input: an alignment field, if desired.
		'''                   On output: the offsets of the alignment field. </param>
		''' <returns>           the value passed in as <code>toAppendTo</code> </returns>
		''' <exception cref="IllegalArgumentException"> if <code>number</code> is
		'''                   null or not an instance of <code>Number</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>toAppendTo</code> or
		'''                   <code>pos</code> is null </exception>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <seealso cref=              java.text.FieldPosition </seealso>
		Public NotOverridable Overrides Function format(  number As Object,   toAppendTo As StringBuffer,   pos As FieldPosition) As StringBuffer
			If TypeOf number Is Long? OrElse TypeOf number Is Integer? OrElse TypeOf number Is Short? OrElse TypeOf number Is Byte OrElse TypeOf number Is java.util.concurrent.atomic.AtomicInteger OrElse TypeOf number Is java.util.concurrent.atomic.AtomicLong OrElse (TypeOf number Is System.Numerics.BigInteger AndAlso CType(number, System.Numerics.BigInteger).bitLength() < 64) Then
				Return format(CType(number, Number), toAppendTo, pos)
			ElseIf TypeOf number Is Decimal Then
				Return format(CDec(number), toAppendTo, pos)
			ElseIf TypeOf number Is System.Numerics.BigInteger Then
				Return format(CType(number, System.Numerics.BigInteger), toAppendTo, pos)
			ElseIf TypeOf number Is Number Then
				Return format(CType(number, Number), toAppendTo, pos)
			Else
				Throw New IllegalArgumentException("Cannot format given Object as a Number")
			End If
		End Function

		''' <summary>
		''' Formats a double to produce a string. </summary>
		''' <param name="number">    The double to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="fieldPosition">    On input: an alignment field, if desired.
		''' On output: the offsets of the alignment field. </param>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''            mode being set to RoundingMode.UNNECESSARY </exception>
		''' <returns> The formatted number string </returns>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Public Overrides Function format(  number As Double,   result As StringBuffer,   fieldPosition As FieldPosition) As StringBuffer
			' If fieldPosition is a DontCareFieldPosition instance we can
			' try to go to fast-path code.
			Dim tryFastPath As Boolean = False
			If fieldPosition Is DontCareFieldPosition.INSTANCE Then
				tryFastPath = True
			Else
				fieldPosition.beginIndex = 0
				fieldPosition.endIndex = 0
			End If

			If tryFastPath Then
				Dim tempResult As String = fastFormat(number)
				If tempResult IsNot Nothing Then
					result.append(tempResult)
					Return result
				End If
			End If

			' if fast-path could not work, we fallback to standard code.
			Return format(number, result, fieldPosition.fieldDelegate)
		End Function

		''' <summary>
		''' Formats a double to produce a string. </summary>
		''' <param name="number">    The double to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="delegate"> notified of locations of sub fields </param>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                  mode being set to RoundingMode.UNNECESSARY </exception>
		''' <returns> The formatted number string </returns>
		Private Function format(  number As Double,   result As StringBuffer,   [delegate] As FieldDelegate) As StringBuffer
			If java.lang.[Double].IsNaN(number) OrElse (Double.IsInfinity(number) AndAlso multiplier = 0) Then
				Dim iFieldStart As Integer = result.length()
				result.append(symbols.naN)
				[delegate].formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.length(), result)
				Return result
			End If

	'         Detecting whether a double is negative is easy with the exception of
	'         * the value -0.0.  This is a double which has a zero mantissa (and
	'         * exponent), but a negative sign bit.  It is semantically distinct from
	'         * a zero with a positive sign bit, and this distinction is important
	'         * to certain kinds of computations.  However, it's a little tricky to
	'         * detect, since (-0.0 == 0.0) and !(-0.0 < 0.0).  How then, you may
	'         * ask, does it behave distinctly from +0.0?  Well, 1/(-0.0) ==
	'         * -Infinity.  Proper detection of -0.0 is needed to deal with the
	'         * issues raised by bugs 4106658, 4106667, and 4147706.  Liu 7/6/98.
	'         
			Dim isNegative As Boolean = ((number < 0.0) OrElse (number = 0.0 AndAlso 1/number < 0.0)) Xor (multiplier < 0)

			If multiplier <> 1 Then number *= multiplier

			If java.lang.[Double].IsInfinity(number) Then
				If isNegative Then
					append(result, negativePrefix, [delegate], negativePrefixFieldPositions, Field.SIGN)
				Else
					append(result, positivePrefix, [delegate], positivePrefixFieldPositions, Field.SIGN)
				End If

				Dim iFieldStart As Integer = result.length()
				result.append(symbols.infinity)
				[delegate].formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.length(), result)

				If isNegative Then
					append(result, negativeSuffix, [delegate], negativeSuffixFieldPositions, Field.SIGN)
				Else
					append(result, positiveSuffix, [delegate], positiveSuffixFieldPositions, Field.SIGN)
				End If

				Return result
			End If

			If isNegative Then number = -number

			' at this point we are guaranteed a nonnegative finite number.
			assert(number >= 0 AndAlso (Not java.lang.[Double].IsInfinity(number)))

			SyncLock digitList
				Dim maxIntDigits As Integer = MyBase.maximumIntegerDigits
				Dim minIntDigits As Integer = MyBase.minimumIntegerDigits
				Dim maxFraDigits As Integer = MyBase.maximumFractionDigits
				Dim minFraDigits As Integer = MyBase.minimumFractionDigits

				digitList.set(isNegative, number,If(useExponentialNotation, maxIntDigits + maxFraDigits, maxFraDigits), (Not useExponentialNotation))
				Return subformat(result, [delegate], isNegative, False, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits)
			End SyncLock
		End Function

		''' <summary>
		''' Format a long to produce a string. </summary>
		''' <param name="number">    The long to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="fieldPosition">    On input: an alignment field, if desired.
		''' On output: the offsets of the alignment field. </param>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                  mode being set to RoundingMode.UNNECESSARY </exception>
		''' <returns> The formatted number string </returns>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Public Overrides Function format(  number As Long,   result As StringBuffer,   fieldPosition As FieldPosition) As StringBuffer
			fieldPosition.beginIndex = 0
			fieldPosition.endIndex = 0

			Return format(number, result, fieldPosition.fieldDelegate)
		End Function

		''' <summary>
		''' Format a long to produce a string. </summary>
		''' <param name="number">    The long to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="delegate"> notified of locations of sub fields </param>
		''' <returns> The formatted number string </returns>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Private Function format(  number As Long,   result As StringBuffer,   [delegate] As FieldDelegate) As StringBuffer
			Dim isNegative As Boolean = (number < 0)
			If isNegative Then number = -number

			' In general, long values always represent real finite numbers, so
			' we don't have to check for +/- Infinity or NaN.  However, there
			' is one case we have to be careful of:  The multiplier can push
			' a number near MIN_VALUE or MAX_VALUE outside the legal range.  We
			' check for this before multiplying, and if it happens we use
			' BigInteger instead.
			Dim useBigInteger As Boolean = False
			If number < 0 Then ' This can only happen if number == java.lang.[Long].MIN_VALUE.
				If multiplier <> 0 Then useBigInteger = True
			ElseIf multiplier <> 1 AndAlso multiplier <> 0 Then
				Dim cutoff As Long = java.lang.[Long].Max_Value / multiplier
				If cutoff < 0 Then cutoff = -cutoff
				useBigInteger = (number > cutoff)
			End If

			If useBigInteger Then
				If isNegative Then number = -number
				Dim bigIntegerValue As System.Numerics.BigInteger = System.Numerics.Big java.lang.[Integer].valueOf(number)
				Return format(bigIntegerValue, result, [delegate], True)
			End If

			number *= multiplier
			If number = 0 Then
				isNegative = False
			Else
				If multiplier < 0 Then
					number = -number
					isNegative = Not isNegative
				End If
			End If

			SyncLock digitList
				Dim maxIntDigits As Integer = MyBase.maximumIntegerDigits
				Dim minIntDigits As Integer = MyBase.minimumIntegerDigits
				Dim maxFraDigits As Integer = MyBase.maximumFractionDigits
				Dim minFraDigits As Integer = MyBase.minimumFractionDigits

				digitList.set(isNegative, number,If(useExponentialNotation, maxIntDigits + maxFraDigits, 0))

				Return subformat(result, [delegate], isNegative, True, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits)
			End SyncLock
		End Function

		''' <summary>
		''' Formats a BigDecimal to produce a string. </summary>
		''' <param name="number">    The BigDecimal to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="fieldPosition">    On input: an alignment field, if desired.
		''' On output: the offsets of the alignment field. </param>
		''' <returns> The formatted number string </returns>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Private Function format(  number As Decimal,   result As StringBuffer,   fieldPosition As FieldPosition) As StringBuffer
			fieldPosition.beginIndex = 0
			fieldPosition.endIndex = 0
			Return format(number, result, fieldPosition.fieldDelegate)
		End Function

		''' <summary>
		''' Formats a BigDecimal to produce a string. </summary>
		''' <param name="number">    The BigDecimal to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="delegate"> notified of locations of sub fields </param>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <returns> The formatted number string </returns>
		Private Function format(  number As Decimal,   result As StringBuffer,   [delegate] As FieldDelegate) As StringBuffer
			If multiplier <> 1 Then number = number * bigDecimalMultiplier
			Dim isNegative As Boolean = number.signum() = -1
			If isNegative Then number = -number

			SyncLock digitList
				Dim maxIntDigits As Integer = maximumIntegerDigits
				Dim minIntDigits As Integer = minimumIntegerDigits
				Dim maxFraDigits As Integer = maximumFractionDigits
				Dim minFraDigits As Integer = minimumFractionDigits
				Dim maximumDigits As Integer = maxIntDigits + maxFraDigits

				digitList.set(isNegative, number,If(useExponentialNotation, (If(maximumDigits < 0,  java.lang.[Integer].Max_Value, maximumDigits)), maxFraDigits), (Not useExponentialNotation))

				Return subformat(result, [delegate], isNegative, False, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits)
			End SyncLock
		End Function

		''' <summary>
		''' Format a BigInteger to produce a string. </summary>
		''' <param name="number">    The BigInteger to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="fieldPosition">    On input: an alignment field, if desired.
		''' On output: the offsets of the alignment field. </param>
		''' <returns> The formatted number string </returns>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Private Function format(  number As System.Numerics.BigInteger,   result As StringBuffer,   fieldPosition As FieldPosition) As StringBuffer
			fieldPosition.beginIndex = 0
			fieldPosition.endIndex = 0

			Return format(number, result, fieldPosition.fieldDelegate, False)
		End Function

		''' <summary>
		''' Format a BigInteger to produce a string. </summary>
		''' <param name="number">    The BigInteger to format </param>
		''' <param name="result">    where the text is to be appended </param>
		''' <param name="delegate"> notified of locations of sub fields </param>
		''' <returns> The formatted number string </returns>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Private Function format(  number As System.Numerics.BigInteger,   result As StringBuffer,   [delegate] As FieldDelegate,   formatLong As Boolean) As StringBuffer
			If multiplier <> 1 Then number = number * bigIntegerMultiplier
			Dim isNegative As Boolean = number.signum() = -1
			If isNegative Then number = -number

			SyncLock digitList
				Dim maxIntDigits, minIntDigits, maxFraDigits, minFraDigits, maximumDigits As Integer
				If formatLong Then
					maxIntDigits = MyBase.maximumIntegerDigits
					minIntDigits = MyBase.minimumIntegerDigits
					maxFraDigits = MyBase.maximumFractionDigits
					minFraDigits = MyBase.minimumFractionDigits
					maximumDigits = maxIntDigits + maxFraDigits
				Else
					maxIntDigits = maximumIntegerDigits
					minIntDigits = minimumIntegerDigits
					maxFraDigits = maximumFractionDigits
					minFraDigits = minimumFractionDigits
					maximumDigits = maxIntDigits + maxFraDigits
					If maximumDigits < 0 Then maximumDigits =  java.lang.[Integer].Max_Value
				End If

				digitList.set(isNegative, number,If(useExponentialNotation, maximumDigits, 0))

				Return subformat(result, [delegate], isNegative, True, maxIntDigits, minIntDigits, maxFraDigits, minFraDigits)
			End SyncLock
		End Function

		''' <summary>
		''' Formats an Object producing an <code>AttributedCharacterIterator</code>.
		''' You can use the returned <code>AttributedCharacterIterator</code>
		''' to build the resulting String, as well as to determine information
		''' about the resulting String.
		''' <p>
		''' Each attribute key of the AttributedCharacterIterator will be of type
		''' <code>NumberFormat.Field</code>, with the attribute value being the
		''' same as the attribute key.
		''' </summary>
		''' <exception cref="NullPointerException"> if obj is null. </exception>
		''' <exception cref="IllegalArgumentException"> when the Format cannot format the
		'''            given object. </exception>
		''' <exception cref="ArithmeticException"> if rounding is needed with rounding
		'''                   mode being set to RoundingMode.UNNECESSARY </exception>
		''' <param name="obj"> The object to format </param>
		''' <returns> AttributedCharacterIterator describing the formatted value.
		''' @since 1.4 </returns>
		Public Overrides Function formatToCharacterIterator(  obj As Object) As AttributedCharacterIterator
			Dim [delegate] As New CharacterIteratorFieldDelegate
			Dim sb As New StringBuffer

			If TypeOf obj Is Double? OrElse TypeOf obj Is Float Then
				format(CType(obj, Number), sb, [delegate])
			ElseIf TypeOf obj Is Long? OrElse TypeOf obj Is Integer? OrElse TypeOf obj Is Short? OrElse TypeOf obj Is Byte OrElse TypeOf obj Is java.util.concurrent.atomic.AtomicInteger OrElse TypeOf obj Is java.util.concurrent.atomic.AtomicLong Then
				format(CType(obj, Number), sb, [delegate])
			ElseIf TypeOf obj Is Decimal Then
				format(CDec(obj), sb, [delegate])
			ElseIf TypeOf obj Is System.Numerics.BigInteger Then
				format(CType(obj, System.Numerics.BigInteger), sb, [delegate], False)
			ElseIf obj Is Nothing Then
				Throw New NullPointerException("formatToCharacterIterator must be passed non-null object")
			Else
				Throw New IllegalArgumentException("Cannot format given Object as a Number")
			End If
			Return [delegate].getIterator(sb.ToString())
		End Function

		' ==== Begin fast-path formating logic for double =========================

	'     Fast-path formatting will be used for format(double ...) methods iff a
	'     * number of conditions are met (see checkAndSetFastPathStatus()):
	'     * - Only if instance properties meet the right predefined conditions.
	'     * - The abs value of the double to format is <=  java.lang.[Integer].MAX_VALUE.
	'     *
	'     * The basic approach is to split the binary to decimal conversion of a
	'     * double value into two phases:
	'     * * The conversion of the integer portion of the java.lang.[Double].
	'     * * The conversion of the fractional portion of the double
	'     *   (limited to two or three digits).
	'     *
	'     * The isolation and conversion of the integer portion of the double is
	'     * straightforward. The conversion of the fraction is more subtle and relies
	'     * on some rounding properties of double to the decimal precisions in
	'     * question.  Using the terminology of BigDecimal, this fast-path algorithm
	'     * is applied when a double value has a magnitude less than  java.lang.[Integer].MAX_VALUE
	'     * and rounding is to nearest even and the destination format has two or
	'     * three digits of *scale* (digits after the decimal point).
	'     *
	'     * Under a rounding to nearest even policy, the returned result is a digit
	'     * string of a number in the (in this case decimal) destination format
	'     * closest to the exact numerical value of the (in this case binary) input
	'     * value.  If two destination format numbers are equally distant, the one
	'     * with the last digit even is returned.  To compute such a correctly rounded
	'     * value, some information about digits beyond the smallest returned digit
	'     * position needs to be consulted.
	'     *
	'     * In general, a guard digit, a round digit, and a sticky *bit* are needed
	'     * beyond the returned digit position.  If the discarded portion of the input
	'     * is sufficiently large, the returned digit string is incremented.  In round
	'     * to nearest even, this threshold to increment occurs near the half-way
	'     * point between digits.  The sticky bit records if there are any remaining
	'     * trailing digits of the exact input value in the new format; the sticky bit
	'     * is consulted only in close to half-way rounding cases.
	'     *
	'     * Given the computation of the digit and bit values, rounding is then
	'     * reduced to a table lookup problem.  For decimal, the even/odd cases look
	'     * like this:
	'     *
	'     * Last   Round   Sticky
	'     * 6      5       0      => 6   // exactly halfway, return even digit.
	'     * 6      5       1      => 7   // a little bit more than halfway, round up.
	'     * 7      5       0      => 8   // exactly halfway, round up to even.
	'     * 7      5       1      => 8   // a little bit more than halfway, round up.
	'     * With analogous entries for other even and odd last-returned digits.
	'     *
	'     * However, decimal negative powers of 5 smaller than 0.5 are *not* exactly
	'     * representable as binary fraction.  In particular, 0.005 (the round limit
	'     * for a two-digit scale) and 0.0005 (the round limit for a three-digit
	'     * scale) are not representable. Therefore, for input values near these cases
	'     * the sticky bit is known to be set which reduces the rounding logic to:
	'     *
	'     * Last   Round   Sticky
	'     * 6      5       1      => 7   // a little bit more than halfway, round up.
	'     * 7      5       1      => 8   // a little bit more than halfway, round up.
	'     *
	'     * In other words, if the round digit is 5, the sticky bit is known to be
	'     * set.  If the round digit is something other than 5, the sticky bit is not
	'     * relevant.  Therefore, some of the logic about whether or not to increment
	'     * the destination *decimal* value can occur based on tests of *binary*
	'     * computations of the binary input number.
	'     

		''' <summary>
		''' Check validity of using fast-path for this instance. If fast-path is valid
		''' for this instance, sets fast-path state as true and initializes fast-path
		''' utility fields as needed.
		''' 
		''' This method is supposed to be called rarely, otherwise that will break the
		''' fast-path performance. That means avoiding frequent changes of the
		''' properties of the instance, since for most properties, each time a change
		''' happens, a call to this method is needed at the next format call.
		''' 
		''' FAST-PATH RULES:
		'''  Similar to the default DecimalFormat instantiation case.
		'''  More precisely:
		'''  - HALF_EVEN rounding mode,
		'''  - isGroupingUsed() is true,
		'''  - groupingSize of 3,
		'''  - multiplier is 1,
		'''  - Decimal separator not mandatory,
		'''  - No use of exponential notation,
		'''  - minimumIntegerDigits is exactly 1 and maximumIntegerDigits at least 10
		'''  - For number of fractional digits, the exact values found in the default case:
		'''     Currency : min = max = 2.
		'''     Decimal  : min = 0. max = 3.
		''' 
		''' </summary>
		Private Sub checkAndSetFastPathStatus()

			Dim fastPathWasOn As Boolean = isFastPath

			If (roundingMode = java.math.RoundingMode.HALF_EVEN) AndAlso (groupingUsed) AndAlso (groupingSize = 3) AndAlso (multiplier = 1) AndAlso ((Not decimalSeparatorAlwaysShown)) AndAlso ((Not useExponentialNotation)) Then

				' The fast-path algorithm is semi-hardcoded against
				'  minimumIntegerDigits and maximumIntegerDigits.
				isFastPath = ((minimumIntegerDigits = 1) AndAlso (maximumIntegerDigits >= 10))

				' The fast-path algorithm is hardcoded against
				'  minimumFractionDigits and maximumFractionDigits.
				If isFastPath Then
					If isCurrencyFormat Then
						If (minimumFractionDigits <> 2) OrElse (maximumFractionDigits <> 2) Then isFastPath = False
					ElseIf (minimumFractionDigits <> 0) OrElse (maximumFractionDigits <> 3) Then
						isFastPath = False
					End If
				End If
			Else
				isFastPath = False
			End If

			' Since some instance properties may have changed while still falling
			' in the fast-path case, we need to reinitialize fastPathData anyway.
			If isFastPath Then
				' We need to instantiate fastPathData if not already done.
				If fastPathData Is Nothing Then fastPathData = New FastPathData

				' Sets up the locale specific constants used when formatting.
				' '0' is our default representation of zero.
				fastPathData.zeroDelta = AscW(AscW(symbols.zeroDigit) - AscW("0"c))
				fastPathData.groupingChar = symbols.groupingSeparator

				' Sets up fractional constants related to currency/decimal pattern.
				fastPathData.fractionalMaxIntBound = If(isCurrencyFormat, 99, 999)
				fastPathData.fractionalScaleFactor = If(isCurrencyFormat, 100.0R, 1000.0R)

				' Records the need for adding prefix or suffix
				fastPathData.positiveAffixesRequired = (positivePrefix.length() <> 0) OrElse (positiveSuffix.length() <> 0)
				fastPathData.negativeAffixesRequired = (negativePrefix.length() <> 0) OrElse (negativeSuffix.length() <> 0)

				' Creates a cached char container for result, with max possible size.
				Dim maxNbIntegralDigits As Integer = 10
				Dim maxNbGroups As Integer = 3
				Dim containerSize As Integer = System.Math.Max(positivePrefix.length(), negativePrefix.length()) + maxNbIntegralDigits + maxNbGroups + 1 + maximumFractionDigits + System.Math.Max(positiveSuffix.length(), negativeSuffix.length())

				fastPathData.fastPathContainer = New Char(containerSize - 1){}

				' Sets up prefix and suffix char arrays constants.
				fastPathData.charsPositiveSuffix = positiveSuffix.ToCharArray()
				fastPathData.charsNegativeSuffix = negativeSuffix.ToCharArray()
				fastPathData.charsPositivePrefix = positivePrefix.ToCharArray()
				fastPathData.charsNegativePrefix = negativePrefix.ToCharArray()

				' Sets up fixed index positions for integral and fractional digits.
				' Sets up decimal point in cached result container.
				Dim longestPrefixLength As Integer = System.Math.Max(positivePrefix.length(), negativePrefix.length())
				Dim decimalPointIndex As Integer = maxNbIntegralDigits + maxNbGroups + longestPrefixLength

				fastPathData.integralLastIndex = decimalPointIndex - 1
				fastPathData.fractionalFirstIndex = decimalPointIndex + 1
				fastPathData.fastPathContainer(decimalPointIndex) = If(isCurrencyFormat, symbols.monetaryDecimalSeparator, symbols.decimalSeparator)

			ElseIf fastPathWasOn Then
				' Previous state was fast-path and is no more.
				' Resets cached array constants.
				fastPathData.fastPathContainer = Nothing
				fastPathData.charsPositiveSuffix = Nothing
				fastPathData.charsNegativeSuffix = Nothing
				fastPathData.charsPositivePrefix = Nothing
				fastPathData.charsNegativePrefix = Nothing
			End If

			fastPathCheckNeeded = False
		End Sub

		''' <summary>
		''' Returns true if rounding-up must be done on {@code scaledFractionalPartAsInt},
		''' false otherwise.
		''' 
		''' This is a utility method that takes correct half-even rounding decision on
		''' passed fractional value at the scaled decimal point (2 digits for currency
		''' case and 3 for decimal case), when the approximated fractional part after
		''' scaled decimal point is exactly 0.5d.  This is done by means of exact
		''' calculations on the {@code fractionalPart} floating-point value.
		''' 
		''' This method is supposed to be called by private {@code fastDoubleFormat}
		''' method only.
		''' 
		''' The algorithms used for the exact calculations are :
		''' 
		''' The <b><i>FastTwoSum</i></b> algorithm, from T.J.Dekker, described in the
		''' papers  "<i>A  Floating-Point   Technique  for  Extending  the  Available
		''' Precision</i>"  by Dekker, and  in "<i>Adaptive  Precision Floating-Point
		''' Arithmetic and Fast Robust Geometric Predicates</i>" from J.Shewchuk.
		''' 
		''' A modified version of <b><i>Sum2S</i></b> cascaded summation described in
		''' "<i>Accurate Sum and Dot Product</i>" from Takeshi Ogita and All.  As
		''' Ogita says in this paper this is an equivalent of the Kahan-Babuska's
		''' summation algorithm because we order the terms by magnitude before summing
		''' them. For this reason we can use the <i>FastTwoSum</i> algorithm rather
		''' than the more expensive Knuth's <i>TwoSum</i>.
		''' 
		''' We do this to avoid a more expensive exact "<i>TwoProduct</i>" algorithm,
		''' like those described in Shewchuk's paper above. See comments in the code
		''' below.
		''' </summary>
		''' <param name="fractionalPart"> The  fractional value  on which  we  take rounding
		''' decision. </param>
		''' <param name="scaledFractionalPartAsInt"> The integral part of the scaled
		''' fractional value.
		''' </param>
		''' <returns> the decision that must be taken regarding half-even rounding. </returns>
		Private Function exactRoundUp(  fractionalPart As Double,   scaledFractionalPartAsInt As Integer) As Boolean

	'         exactRoundUp() method is called by fastDoubleFormat() only.
	'         * The precondition expected to be verified by the passed parameters is :
	'         * scaledFractionalPartAsInt ==
	'         *     (int) (fractionalPart * fastPathData.fractionalScaleFactor).
	'         * This is ensured by fastDoubleFormat() code.
	'         

	'         We first calculate roundoff error made by fastDoubleFormat() on
	'         * the scaled fractional part. We do this with exact calculation on the
	'         * passed fractionalPart. Rounding decision will then be taken from roundoff.
	'         

	'         ---- TwoProduct(fractionalPart, scale factor (i.e. 1000.0d or 100.0d)).
	'         *
	'         * The below is an optimized exact "TwoProduct" calculation of passed
	'         * fractional part with scale factor, using Ogita's Sum2S cascaded
	'         * summation adapted as Kahan-Babuska equivalent by using FastTwoSum
	'         * (much faster) rather than Knuth's TwoSum.
	'         *
	'         * We can do this because we order the summation from smallest to
	'         * greatest, so that FastTwoSum can be used without any additional error.
	'         *
	'         * The "TwoProduct" exact calculation needs 17 flops. We replace this by
	'         * a cascaded summation of FastTwoSum calculations, each involving an
	'         * exact multiply by a power of 2.
	'         *
	'         * Doing so saves overall 4 multiplications and 1 addition compared to
	'         * using traditional "TwoProduct".
	'         *
	'         * The scale factor is either 100 (currency case) or 1000 (decimal case).
	'         * - when 1000, we replace it by (1024 - 16 - 8) = 1000.
	'         * - when 100,  we replace it by (128  - 32 + 4) =  100.
	'         * Every multiplication by a power of 2 (1024, 128, 32, 16, 8, 4) is exact.
	'         *
	'         
			Dim approxMax As Double ' Will always be positive.
			Dim approxMedium As Double ' Will always be negative.
			Dim approxMin As Double

			Dim fastTwoSumApproximation As Double = 0.0R
			Dim fastTwoSumRoundOff As Double = 0.0R
			Dim bVirtual As Double = 0.0R

			If isCurrencyFormat Then
				' Scale is 100 = 128 - 32 + 4.
				' Multiply by 2**n is a shift. No roundoff. No error.
				approxMax = fractionalPart * 128.00R
				approxMedium = - (fractionalPart * 32.00R)
				approxMin = fractionalPart * 4.00R
			Else
				' Scale is 1000 = 1024 - 16 - 8.
				' Multiply by 2**n is a shift. No roundoff. No error.
				approxMax = fractionalPart * 1024.00R
				approxMedium = - (fractionalPart * 16.00R)
				approxMin = - (fractionalPart * 8.00R)
			End If

			' Shewchuk/Dekker's FastTwoSum(approxMedium, approxMin).
			assert(-approxMedium >= System.Math.Abs(approxMin))
			fastTwoSumApproximation = approxMedium + approxMin
			bVirtual = fastTwoSumApproximation - approxMedium
			fastTwoSumRoundOff = approxMin - bVirtual
			Dim approxS1 As Double = fastTwoSumApproximation
			Dim roundoffS1 As Double = fastTwoSumRoundOff

			' Shewchuk/Dekker's FastTwoSum(approxMax, approxS1);
			assert(approxMax >= System.Math.Abs(approxS1))
			fastTwoSumApproximation = approxMax + approxS1
			bVirtual = fastTwoSumApproximation - approxMax
			fastTwoSumRoundOff = approxS1 - bVirtual
			Dim roundoff1000 As Double = fastTwoSumRoundOff
			Dim approx1000 As Double = fastTwoSumApproximation
			Dim roundoffTotal As Double = roundoffS1 + roundoff1000

			' Shewchuk/Dekker's FastTwoSum(approx1000, roundoffTotal);
			assert(approx1000 >= System.Math.Abs(roundoffTotal))
			fastTwoSumApproximation = approx1000 + roundoffTotal
			bVirtual = fastTwoSumApproximation - approx1000

			' Now we have got the roundoff for the scaled fractional
			Dim scaledFractionalRoundoff As Double = roundoffTotal - bVirtual

			' ---- TwoProduct(fractionalPart, scale (i.e. 1000.0d or 100.0d)) end.

	'         ---- Taking the rounding decision
	'         *
	'         * We take rounding decision based on roundoff and half-even rounding
	'         * rule.
	'         *
	'         * The above TwoProduct gives us the exact roundoff on the approximated
	'         * scaled fractional, and we know that this approximation is exactly
	'         * 0.5d, since that has already been tested by the caller
	'         * (fastDoubleFormat).
	'         *
	'         * Decision comes first from the sign of the calculated exact roundoff.
	'         * - Since being exact roundoff, it cannot be positive with a scaled
	'         *   fractional less than 0.5d, as well as negative with a scaled
	'         *   fractional greater than 0.5d. That leaves us with following 3 cases.
	'         * - positive, thus scaled fractional == 0.500....0fff ==> round-up.
	'         * - negative, thus scaled fractional == 0.499....9fff ==> don't round-up.
	'         * - is zero,  thus scaled fractioanl == 0.5 ==> half-even rounding applies :
	'         *    we round-up only if the integral part of the scaled fractional is odd.
	'         *
	'         
			If scaledFractionalRoundoff > 0.0 Then
				Return True
			ElseIf scaledFractionalRoundoff < 0.0 Then
				Return False
			ElseIf (scaledFractionalPartAsInt And 1) <> 0 Then
				Return True
			End If

			Return False

			' ---- Taking the rounding decision end
		End Function

		''' <summary>
		''' Collects integral digits from passed {@code number}, while setting
		''' grouping chars as needed. Updates {@code firstUsedIndex} accordingly.
		''' 
		''' Loops downward starting from {@code backwardIndex} position (inclusive).
		''' </summary>
		''' <param name="number">  The int value from which we collect digits. </param>
		''' <param name="digitsBuffer"> The char array container where digits and grouping chars
		'''  are stored. </param>
		''' <param name="backwardIndex"> the position from which we start storing digits in
		'''  digitsBuffer.
		'''  </param>
		Private Sub collectIntegralDigits(  number As Integer,   digitsBuffer As Char(),   backwardIndex As Integer)
			Dim index As Integer = backwardIndex
			Dim q As Integer
			Dim r As Integer
			Do While number > 999
				' Generates 3 digits per iteration.
				q = number \ 1000
				r = number - (q << 10) + (q << 4) + (q << 3) ' -1024 +16 +8 = 1000.
				number = q

				digitsBuffer(index) = DigitArrays.DigitOnes1000(r)
				index -= 1
				digitsBuffer(index) = DigitArrays.DigitTens1000(r)
				index -= 1
				digitsBuffer(index) = DigitArrays.DigitHundreds1000(r)
				index -= 1
				digitsBuffer(index) = fastPathData.groupingChar
				index -= 1
			Loop

			' Collects last 3 or less digits.
			digitsBuffer(index) = DigitArrays.DigitOnes1000(number)
			If number > 9 Then
				index -= 1
				digitsBuffer(index) = DigitArrays.DigitTens1000(number)
				If number > 99 Then
					index -= 1
					digitsBuffer(index) = DigitArrays.DigitHundreds1000(number)
				End If
			End If

			fastPathData.firstUsedIndex = index
		End Sub

		''' <summary>
		''' Collects the 2 (currency) or 3 (decimal) fractional digits from passed
		''' {@code number}, starting at {@code startIndex} position
		''' inclusive.  There is no punctuation to set here (no grouping chars).
		''' Updates {@code fastPathData.lastFreeIndex} accordingly.
		''' 
		''' </summary>
		''' <param name="number">  The int value from which we collect digits. </param>
		''' <param name="digitsBuffer"> The char array container where digits are stored. </param>
		''' <param name="startIndex"> the position from which we start storing digits in
		'''  digitsBuffer.
		'''  </param>
		Private Sub collectFractionalDigits(  number As Integer,   digitsBuffer As Char(),   startIndex As Integer)
			Dim index As Integer = startIndex

			Dim digitOnes As Char = DigitArrays.DigitOnes1000(number)
			Dim digitTens As Char = DigitArrays.DigitTens1000(number)

			If isCurrencyFormat Then
				' Currency case. Always collects fractional digits.
				digitsBuffer(index) = digitTens
				index += 1
				digitsBuffer(index) = digitOnes
				index += 1
			ElseIf number <> 0 Then
				' Decimal case. Hundreds will always be collected
				digitsBuffer(index) = DigitArrays.DigitHundreds1000(number)
				index += 1

				' Ending zeros won't be collected.
				If digitOnes <> "0"c Then
					digitsBuffer(index) = digitTens
					index += 1
					digitsBuffer(index) = digitOnes
					index += 1
				ElseIf digitTens <> "0"c Then
					digitsBuffer(index) = digitTens
					index += 1
				End If

			Else
				' This is decimal pattern and fractional part is zero.
				' We must remove decimal point from result.
				index -= 1
			End If

			fastPathData.lastFreeIndex = index
		End Sub

		''' <summary>
		''' Internal utility.
		''' Adds the passed {@code prefix} and {@code suffix} to {@code container}.
		''' </summary>
		''' <param name="container">  Char array container which to prepend/append the
		'''  prefix/suffix. </param>
		''' <param name="prefix">     Char sequence to prepend as a prefix. </param>
		''' <param name="suffix">     Char sequence to append as a suffix.
		'''  </param>
		'    private  Sub  addAffixes(boolean isNegative, char[] container) {
		Private Sub addAffixes(  container As Char(),   prefix As Char(),   suffix As Char())

			' We add affixes only if needed (affix length > 0).
			Dim pl As Integer = prefix.Length
			Dim sl As Integer = suffix.Length
			If pl <> 0 Then prependPrefix(prefix, pl, container)
			If sl <> 0 Then appendSuffix(suffix, sl, container)

		End Sub

		''' <summary>
		''' Prepends the passed {@code prefix} chars to given result
		''' {@code container}.  Updates {@code fastPathData.firstUsedIndex}
		''' accordingly.
		''' </summary>
		''' <param name="prefix"> The prefix characters to prepend to result. </param>
		''' <param name="len"> The number of chars to prepend. </param>
		''' <param name="container"> Char array container which to prepend the prefix </param>
		Private Sub prependPrefix(  prefix As Char(),   len As Integer,   container As Char())

			fastPathData.firstUsedIndex -= len
			Dim startIndex As Integer = fastPathData.firstUsedIndex

			' If prefix to prepend is only 1 char long, just assigns this char.
			' If prefix is less or equal 4, we use a dedicated algorithm that
			'  has shown to run faster than System.arraycopy.
			' If more than 4, we use System.arraycopy.
			If len = 1 Then
				container(startIndex) = prefix(0)
			ElseIf len <= 4 Then
				Dim dstLower As Integer = startIndex
				Dim dstUpper As Integer = dstLower + len - 1
				Dim srcUpper As Integer = len - 1
				container(dstLower) = prefix(0)
				container(dstUpper) = prefix(srcUpper)

				If len > 2 Then
					dstLower += 1
					container(dstLower) = prefix(1)
				End If
				If len = 4 Then
					dstUpper -= 1
					container(dstUpper) = prefix(2)
				End If
			Else
				Array.Copy(prefix, 0, container, startIndex, len)
			End If
		End Sub

		''' <summary>
		''' Appends the passed {@code suffix} chars to given result
		''' {@code container}.  Updates {@code fastPathData.lastFreeIndex}
		''' accordingly.
		''' </summary>
		''' <param name="suffix"> The suffix characters to append to result. </param>
		''' <param name="len"> The number of chars to append. </param>
		''' <param name="container"> Char array container which to append the suffix </param>
		Private Sub appendSuffix(  suffix As Char(),   len As Integer,   container As Char())

			Dim startIndex As Integer = fastPathData.lastFreeIndex

			' If suffix to append is only 1 char long, just assigns this char.
			' If suffix is less or equal 4, we use a dedicated algorithm that
			'  has shown to run faster than System.arraycopy.
			' If more than 4, we use System.arraycopy.
			If len = 1 Then
				container(startIndex) = suffix(0)
			ElseIf len <= 4 Then
				Dim dstLower As Integer = startIndex
				Dim dstUpper As Integer = dstLower + len - 1
				Dim srcUpper As Integer = len - 1
				container(dstLower) = suffix(0)
				container(dstUpper) = suffix(srcUpper)

				If len > 2 Then
					dstLower += 1
					container(dstLower) = suffix(1)
				End If
				If len = 4 Then
					dstUpper -= 1
					container(dstUpper) = suffix(2)
				End If
			Else
				Array.Copy(suffix, 0, container, startIndex, len)
			End If

			fastPathData.lastFreeIndex += len
		End Sub

		''' <summary>
		''' Converts digit chars from {@code digitsBuffer} to current locale.
		''' 
		''' Must be called before adding affixes since we refer to
		''' {@code fastPathData.firstUsedIndex} and {@code fastPathData.lastFreeIndex},
		''' and do not support affixes (for speed reason).
		''' 
		''' We loop backward starting from last used index in {@code fastPathData}.
		''' </summary>
		''' <param name="digitsBuffer"> The char array container where the digits are stored. </param>
		Private Sub localizeDigits(  digitsBuffer As Char())

			' We will localize only the digits, using the groupingSize,
			' and taking into account fractional part.

			' First take into account fractional part.
			Dim digitsCounter As Integer = fastPathData.lastFreeIndex - fastPathData.fractionalFirstIndex

			' The case when there is no fractional digits.
			If digitsCounter < 0 Then digitsCounter = groupingSize

			' Only the digits remains to localize.
			For cursor As Integer = fastPathData.lastFreeIndex - 1 To fastPathData.firstUsedIndex Step -1
				If digitsCounter <> 0 Then
					' This is a digit char, we must localize it.
					AscW(digitsBuffer(cursor)) += fastPathData.zeroDelta
					digitsCounter -= 1
				Else
					' Decimal separator or grouping char. Reinit counter only.
					digitsCounter = groupingSize
				End If
			Next cursor
		End Sub

		''' <summary>
		''' This is the main entry point for the fast-path format algorithm.
		''' 
		''' At this point we are sure to be in the expected conditions to run it.
		''' This algorithm builds the formatted result and puts it in the dedicated
		''' {@code fastPathData.fastPathContainer}.
		''' </summary>
		''' <param name="d"> the double value to be formatted. </param>
		''' <param name="negative"> Flag precising if {@code d} is negative. </param>
		Private Sub fastDoubleFormat(  d As Double,   negative As Boolean)

			Dim container As Char() = fastPathData.fastPathContainer

	'        
	'         * The principle of the algorithm is to :
	'         * - Break the passed double into its integral and fractional parts
	'         *    converted into integers.
	'         * - Then decide if rounding up must be applied or not by following
	'         *    the half-even rounding rule, first using approximated scaled
	'         *    fractional part.
	'         * - For the difficult cases (approximated scaled fractional part
	'         *    being exactly 0.5d), we refine the rounding decision by calling
	'         *    exactRoundUp utility method that both calculates the exact roundoff
	'         *    on the approximation and takes correct rounding decision.
	'         * - We round-up the fractional part if needed, possibly propagating the
	'         *    rounding to integral part if we meet a "all-nine" case for the
	'         *    scaled fractional part.
	'         * - We then collect digits from the resulting integral and fractional
	'         *   parts, also setting the required grouping chars on the fly.
	'         * - Then we localize the collected digits if needed, and
	'         * - Finally prepend/append prefix/suffix if any is needed.
	'         

			' Exact integral part of d.
			Dim integralPartAsInt As Integer = CInt(Fix(d))

			' Exact fractional part of d (since we subtract it's integral part).
			Dim exactFractionalPart As Double = d - CDbl(integralPartAsInt)

			' Approximated scaled fractional part of d (due to multiplication).
			Dim scaledFractional As Double = exactFractionalPart * fastPathData.fractionalScaleFactor

			' Exact integral part of scaled fractional above.
			Dim fractionalPartAsInt As Integer = CInt(Fix(scaledFractional))

			' Exact fractional part of scaled fractional above.
			scaledFractional = scaledFractional - CDbl(fractionalPartAsInt)

			' Only when scaledFractional is exactly 0.5d do we have to do exact
			' calculations and take fine-grained rounding decision, since
			' approximated results above may lead to incorrect decision.
			' Otherwise comparing against 0.5d (strictly greater or less) is ok.
			Dim roundItUp As Boolean = False
			If scaledFractional >= 0.5R Then
				If scaledFractional = 0.5R Then
					' Rounding need fine-grained decision.
					roundItUp = exactRoundUp(exactFractionalPart, fractionalPartAsInt)
				Else
					roundItUp = True
				End If

				If roundItUp Then
					' Rounds up both fractional part (and also integral if needed).
					If fractionalPartAsInt < fastPathData.fractionalMaxIntBound Then
						fractionalPartAsInt += 1
					Else
						' Propagates rounding to integral part since "all nines" case.
						fractionalPartAsInt = 0
						integralPartAsInt += 1
					End If
				End If
			End If

			' Collecting digits.
			collectFractionalDigits(fractionalPartAsInt, container, fastPathData.fractionalFirstIndex)
			collectIntegralDigits(integralPartAsInt, container, fastPathData.integralLastIndex)

			' Localizing digits.
			If fastPathData.zeroDelta <> 0 Then localizeDigits(container)

			' Adding prefix and suffix.
			If negative Then
				If fastPathData.negativeAffixesRequired Then addAffixes(container, fastPathData.charsNegativePrefix, fastPathData.charsNegativeSuffix)
			ElseIf fastPathData.positiveAffixesRequired Then
				addAffixes(container, fastPathData.charsPositivePrefix, fastPathData.charsPositiveSuffix)
			End If
		End Sub

		''' <summary>
		''' A fast-path shortcut of format(double) to be called by NumberFormat, or by
		''' format(double, ...) public methods.
		''' 
		''' If instance can be applied fast-path and passed double is not NaN or
		''' Infinity, is in the integer range, we call {@code fastDoubleFormat}
		''' after changing {@code d} to its positive value if necessary.
		''' 
		''' Otherwise returns null by convention since fast-path can't be exercized.
		''' </summary>
		''' <param name="d"> The double value to be formatted
		''' </param>
		''' <returns> the formatted result for {@code d} as a string. </returns>
		Friend Overrides Function fastFormat(  d As Double) As String
			' (Re-)Evaluates fast-path status if needed.
			If fastPathCheckNeeded Then checkAndSetFastPathStatus()

			If Not isFastPath Then Return Nothing

			If Not java.lang.[Double].isFinite(d) Then Return Nothing

			' Extracts and records sign of double value, possibly changing it
			' to a positive one, before calling fastDoubleFormat().
			Dim negative As Boolean = False
			If d < 0.0R Then
				negative = True
				d = -d
			ElseIf d = 0.0R Then
				negative =  (System.Math.copySign(1.0R, d) = -1.0R)
				d = +0.0R
			End If

			If d > MAX_INT_AS_DOUBLE Then
				' Filters out values that are outside expected fast-path range
				Return Nothing
			Else
				fastDoubleFormat(d, negative)
			End If

			' Returns a new string from updated fastPathContainer.
			Return New String(fastPathData.fastPathContainer, fastPathData.firstUsedIndex, fastPathData.lastFreeIndex - fastPathData.firstUsedIndex)

		End Function

		' ======== End fast-path formating logic for double =========================

		''' <summary>
		''' Complete the formatting of a finite number.  On entry, the digitList must
		''' be filled in with the correct digits.
		''' </summary>
		Private Function subformat(  result As StringBuffer,   [delegate] As FieldDelegate,   isNegative As Boolean,   isInteger As Boolean,   maxIntDigits As Integer,   minIntDigits As Integer,   maxFraDigits As Integer,   minFraDigits As Integer) As StringBuffer
			' NOTE: This isn't required anymore because DigitList takes care of this.
			'
			'  // The negative of the exponent represents the number of leading
			'  // zeros between the decimal and the first non-zero digit, for
			'  // a value < 0.1 (e.g., for 0.00123, -fExponent == 2).  If this
			'  // is more than the maximum fraction digits, then we have an underflow
			'  // for the printed representation.  We recognize this here and set
			'  // the DigitList representation to zero in this situation.
			'
			'  if (-digitList.decimalAt >= getMaximumFractionDigits())
			'  {
			'      digitList.count = 0;
			'  }

			Dim zero As Char = symbols.zeroDigit
			Dim zeroDelta As Integer = AscW(zero) - AscW("0"c) ' '0' is the DigitList representation of zero
			Dim grouping As Char = symbols.groupingSeparator
			Dim [decimal] As Char = If(isCurrencyFormat, symbols.monetaryDecimalSeparator, symbols.decimalSeparator)

	'         Per bug 4147706, DecimalFormat must respect the sign of numbers which
	'         * format as zero.  This allows sensible computations and preserves
	'         * relations such as signum(1/x) = signum(x), where x is +Infinity or
	'         * -Infinity.  Prior to this fix, we always formatted zero values as if
	'         * they were positive.  Liu 7/6/98.
	'         
			If digitList.zero Then digitList.decimalAt = 0 ' Normalize

			If isNegative Then
				append(result, negativePrefix, [delegate], negativePrefixFieldPositions, Field.SIGN)
			Else
				append(result, positivePrefix, [delegate], positivePrefixFieldPositions, Field.SIGN)
			End If

			If useExponentialNotation Then
				Dim iFieldStart As Integer = result.length()
				Dim iFieldEnd As Integer = -1
				Dim fFieldStart As Integer = -1

				' Minimum integer digits are handled in exponential format by
				' adjusting the exponent.  For example, 0.01234 with 3 minimum
				' integer digits is "123.4E-4".

				' Maximum integer digits are interpreted as indicating the
				' repeating range.  This is useful for engineering notation, in
				' which the exponent is restricted to a multiple of 3.  For
				' example, 0.01234 with 3 maximum integer digits is "12.34e-3".
				' If maximum integer digits are > 1 and are larger than
				' minimum integer digits, then minimum integer digits are
				' ignored.
				Dim exponent As Integer = digitList.decimalAt
				Dim repeat As Integer = maxIntDigits
				Dim minimumIntegerDigits_Renamed As Integer = minIntDigits
				If repeat > 1 AndAlso repeat > minIntDigits Then
					' A repeating range is defined; adjust to it as follows.
					' If repeat == 3, we have 6,5,4=>3; 3,2,1=>0; 0,-1,-2=>-3;
					' -3,-4,-5=>-6, etc. This takes into account that the
					' exponent we have here is off by one from what we expect;
					' it is for the format 0.MMMMMx10^n.
					If exponent >= 1 Then
						exponent = ((exponent - 1) \ repeat) * repeat
					Else
						' integer division rounds towards 0
						exponent = ((exponent - repeat) \ repeat) * repeat
					End If
					minimumIntegerDigits_Renamed = 1
				Else
					' No repeating range is defined; use minimum integer digits.
					exponent -= minimumIntegerDigits_Renamed
				End If

				' We now output a minimum number of digits, and more if there
				' are more digits, up to the maximum number of digits.  We
				' place the decimal point after the "integer" digits, which
				' are the first (decimalAt - exponent) digits.
				Dim minimumDigits As Integer = minIntDigits + minFraDigits
				If minimumDigits < 0 Then ' overflow? minimumDigits =  java.lang.[Integer].Max_Value

				' The number of integer digits is handled specially if the number
				' is zero, since then there may be no digits.
				Dim integerDigits As Integer = If(digitList.zero, minimumIntegerDigits_Renamed, digitList.decimalAt - exponent)
				If minimumDigits < integerDigits Then minimumDigits = integerDigits
				Dim totalDigits As Integer = digitList.count
				If minimumDigits > totalDigits Then totalDigits = minimumDigits
				Dim addedDecimalSeparator As Boolean = False

				For i As Integer = 0 To totalDigits - 1
					If i = integerDigits Then
						' Record field information for caller.
						iFieldEnd = result.length()

						result.append([decimal])
						addedDecimalSeparator = True

						' Record field information for caller.
						fFieldStart = result.length()
					End If
					result.append(If(i < digitList.count, CChar(AscW(digitList.digits(i)) + zeroDelta), zero))
				Next i

				If decimalSeparatorAlwaysShown AndAlso totalDigits = integerDigits Then
					' Record field information for caller.
					iFieldEnd = result.length()

					result.append([decimal])
					addedDecimalSeparator = True

					' Record field information for caller.
					fFieldStart = result.length()
				End If

				' Record field information
				If iFieldEnd = -1 Then iFieldEnd = result.length()
				[delegate].formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, iFieldEnd, result)
				If addedDecimalSeparator Then [delegate].formatted(Field.DECIMAL_SEPARATOR, Field.DECIMAL_SEPARATOR, iFieldEnd, fFieldStart, result)
				If fFieldStart = -1 Then fFieldStart = result.length()
				[delegate].formatted(FRACTION_FIELD, Field.FRACTION, Field.FRACTION, fFieldStart, result.length(), result)

				' The exponent is output using the pattern-specified minimum
				' exponent digits.  There is no maximum limit to the exponent
				' digits, since truncating the exponent would result in an
				' unacceptable inaccuracy.
				Dim fieldStart As Integer = result.length()

				result.append(symbols.exponentSeparator)

				[delegate].formatted(Field.EXPONENT_SYMBOL, Field.EXPONENT_SYMBOL, fieldStart, result.length(), result)

				' For zero values, we force the exponent to zero.  We
				' must do this here, and not earlier, because the value
				' is used to determine integer digit count above.
				If digitList.zero Then exponent = 0

				Dim negativeExponent As Boolean = exponent < 0
				If negativeExponent Then
					exponent = -exponent
					fieldStart = result.length()
					result.append(symbols.minusSign)
					[delegate].formatted(Field.EXPONENT_SIGN, Field.EXPONENT_SIGN, fieldStart, result.length(), result)
				End If
				digitList.set(negativeExponent, exponent)

				Dim eFieldStart As Integer = result.length()

				For i As Integer = digitList.decimalAt To minExponentDigits - 1
					result.append(zero)
				Next i
				For i As Integer = 0 To digitList.decimalAt - 1
					result.append(If(i < digitList.count, CChar(AscW(digitList.digits(i)) + zeroDelta), zero))
				Next i
				[delegate].formatted(Field.EXPONENT, Field.EXPONENT, eFieldStart, result.length(), result)
			Else
				Dim iFieldStart As Integer = result.length()

				' Output the integer portion.  Here 'count' is the total
				' number of integer digits we will display, including both
				' leading zeros required to satisfy getMinimumIntegerDigits,
				' and actual digits present in the number.
				Dim count As Integer = minIntDigits
				Dim digitIndex As Integer = 0 ' Index into digitList.fDigits[]
				If digitList.decimalAt > 0 AndAlso count < digitList.decimalAt Then count = digitList.decimalAt

				' Handle the case where getMaximumIntegerDigits() is smaller
				' than the real number of integer digits.  If this is so, we
				' output the least significant max integer digits.  For example,
				' the value 1997 printed with 2 max integer digits is just "97".
				If count > maxIntDigits Then
					count = maxIntDigits
					digitIndex = digitList.decimalAt - count
				End If

				Dim sizeBeforeIntegerPart As Integer = result.length()
				For i As Integer = count-1 To 0 Step -1
					If i < digitList.decimalAt AndAlso digitIndex < digitList.count Then
						' Output a real digit
						result.append(CChar(AscW(digitList.digits(digitIndex)) + zeroDelta))
						digitIndex += 1
					Else
						' Output a leading zero
						result.append(zero)
					End If

					' Output grouping separator if necessary.  Don't output a
					' grouping separator if i==0 though; that's at the end of
					' the integer part.
					If groupingUsed AndAlso i>0 AndAlso (groupingSize <> 0) AndAlso (i Mod groupingSize = 0) Then
						Dim gStart As Integer = result.length()
						result.append(grouping)
						[delegate].formatted(Field.GROUPING_SEPARATOR, Field.GROUPING_SEPARATOR, gStart, result.length(), result)
					End If
				Next i

				' Determine whether or not there are any printable fractional
				' digits.  If we've used up the digits we know there aren't.
				Dim fractionPresent As Boolean = (minFraDigits > 0) OrElse ((Not isInteger) AndAlso digitIndex < digitList.count)

				' If there is no fraction present, and we haven't printed any
				' integer digits, then print a zero.  Otherwise we won't print
				' _any_ digits, and we won't be able to parse this string.
				If (Not fractionPresent) AndAlso result.length() Is sizeBeforeIntegerPart Then result.append(zero)

				[delegate].formatted(INTEGER_FIELD, Field.INTEGER, Field.INTEGER, iFieldStart, result.length(), result)

				' Output the decimal separator if we always do so.
				Dim sStart As Integer = result.length()
				If decimalSeparatorAlwaysShown OrElse fractionPresent Then result.append([decimal])

				If sStart IsNot result.length() Then [delegate].formatted(Field.DECIMAL_SEPARATOR, Field.DECIMAL_SEPARATOR, sStart, result.length(), result)
				Dim fFieldStart As Integer = result.length()

				For i As Integer = 0 To maxFraDigits - 1
					' Here is where we escape from the loop.  We escape if we've
					' output the maximum fraction digits (specified in the for
					' expression above).
					' We also stop when we've output the minimum digits and either:
					' we have an integer, so there is no fractional stuff to
					' display, or we're out of significant digits.
					If i >= minFraDigits AndAlso (isInteger OrElse digitIndex >= digitList.count) Then Exit For

					' Output leading fractional zeros. These are zeros that come
					' after the decimal but before any significant digits. These
					' are only output if abs(number being formatted) < 1.0.
					If -1-i > (digitList.decimalAt-1) Then
						result.append(zero)
						Continue For
					End If

					' Output a digit, if we have any precision left, or a
					' zero if we don't.  We don't want to output noise digits.
					If (Not isInteger) AndAlso digitIndex < digitList.count Then
						result.append(CChar(AscW(digitList.digits(digitIndex)) + zeroDelta))
						digitIndex += 1
					Else
						result.append(zero)
					End If
				Next i

				' Record field information for caller.
				[delegate].formatted(FRACTION_FIELD, Field.FRACTION, Field.FRACTION, fFieldStart, result.length(), result)
			End If

			If isNegative Then
				append(result, negativeSuffix, [delegate], negativeSuffixFieldPositions, Field.SIGN)
			Else
				append(result, positiveSuffix, [delegate], positiveSuffixFieldPositions, Field.SIGN)
			End If

			Return result
		End Function

		''' <summary>
		''' Appends the String <code>string</code> to <code>result</code>.
		''' <code>delegate</code> is notified of all  the
		''' <code>FieldPosition</code>s in <code>positions</code>.
		''' <p>
		''' If one of the <code>FieldPosition</code>s in <code>positions</code>
		''' identifies a <code>SIGN</code> attribute, it is mapped to
		''' <code>signAttribute</code>. This is used
		''' to map the <code>SIGN</code> attribute to the <code>EXPONENT</code>
		''' attribute as necessary.
		''' <p>
		''' This is used by <code>subformat</code> to add the prefix/suffix.
		''' </summary>
		Private Sub append(  result As StringBuffer,   [string] As String,   [delegate] As FieldDelegate,   positions As FieldPosition(),   signAttribute As Format.Field)
			Dim start As Integer = result.length()

			If string_Renamed.length() > 0 Then
				result.append(string_Renamed)
				Dim counter As Integer = 0
				Dim max As Integer = positions.Length
				Do While counter < max
					Dim fp As FieldPosition = positions(counter)
					Dim attribute As Format.Field = fp.fieldAttribute

					If attribute Is Field.SIGN Then attribute = signAttribute
					[delegate].formatted(attribute, attribute, start + fp.beginIndex, start + fp.endIndex, result)
					counter += 1
				Loop
			End If
		End Sub

		''' <summary>
		''' Parses text from a string to produce a <code>Number</code>.
		''' <p>
		''' The method attempts to parse text starting at the index given by
		''' <code>pos</code>.
		''' If parsing succeeds, then the index of <code>pos</code> is updated
		''' to the index after the last character used (parsing does not necessarily
		''' use all characters up to the end of the string), and the parsed
		''' number is returned. The updated <code>pos</code> can be used to
		''' indicate the starting point for the next call to this method.
		''' If an error occurs, then the index of <code>pos</code> is not
		''' changed, the error index of <code>pos</code> is set to the index of
		''' the character where the error occurred, and null is returned.
		''' <p>
		''' The subclass returned depends on the value of <seealso cref="#isParseBigDecimal"/>
		''' as well as on the string being parsed.
		''' <ul>
		'''   <li>If <code>isParseBigDecimal()</code> is false (the default),
		'''       most integer values are returned as <code>Long</code>
		'''       objects, no matter how they are written: <code>"17"</code> and
		'''       <code>"17.000"</code> both parse to <code>Long(17)</code>.
		'''       Values that cannot fit into a <code>Long</code> are returned as
		'''       <code>Double</code>s. This includes values with a fractional part,
		'''       infinite values, <code>NaN</code>, and the value -0.0.
		'''       <code>DecimalFormat</code> does <em>not</em> decide whether to
		'''       return a <code>Double</code> or a <code>Long</code> based on the
		'''       presence of a decimal separator in the source string. Doing so
		'''       would prevent integers that overflow the mantissa of a double,
		'''       such as <code>"-9,223,372,036,854,775,808.00"</code>, from being
		'''       parsed accurately.
		'''       <p>
		'''       Callers may use the <code>Number</code> methods
		'''       <code>doubleValue</code>, <code>longValue</code>, etc., to obtain
		'''       the type they want.
		'''   <li>If <code>isParseBigDecimal()</code> is true, values are returned
		'''       as <code>BigDecimal</code> objects. The values are the ones
		'''       constructed by <seealso cref="java.math.BigDecimal#BigDecimal(String)"/>
		'''       for corresponding strings in locale-independent format. The
		'''       special cases negative and positive infinity and NaN are returned
		'''       as <code>Double</code> instances holding the values of the
		'''       corresponding <code>Double</code> constants.
		''' </ul>
		''' <p>
		''' <code>DecimalFormat</code> parses all Unicode characters that represent
		''' decimal digits, as defined by <code>Character.digit()</code>. In
		''' addition, <code>DecimalFormat</code> also recognizes as digits the ten
		''' consecutive characters starting with the localized zero digit defined in
		''' the <code>DecimalFormatSymbols</code> object.
		''' </summary>
		''' <param name="text"> the string to be parsed </param>
		''' <param name="pos">  A <code>ParsePosition</code> object with index and error
		'''             index information as described above. </param>
		''' <returns>     the parsed value, or <code>null</code> if the parse fails </returns>
		''' <exception cref="NullPointerException"> if <code>text</code> or
		'''             <code>pos</code> is null. </exception>
		Public Overrides Function parse(  text As String,   pos As ParsePosition) As Number
			' special case NaN
			If text.regionMatches(pos.index, symbols.naN, 0, symbols.naN.length()) Then
				pos.index = pos.index + symbols.naN.length()
				Return New Double?(Double.NaN)
			End If

			Dim status As Boolean() = New Boolean(STATUS_LENGTH - 1){}
			If Not subparse(text, pos, positivePrefix, negativePrefix, digitList, False, status) Then Return Nothing

			' special case INFINITY
			If status(STATUS_INFINITE) Then
				If status(STATUS_POSITIVE) = (multiplier >= 0) Then
					Return New Double?(Double.PositiveInfinity)
				Else
					Return New Double?(Double.NegativeInfinity)
				End If
			End If

			If multiplier = 0 Then
				If digitList.zero Then
					Return New Double?(Double.NaN)
				ElseIf status(STATUS_POSITIVE) Then
					Return New Double?(Double.PositiveInfinity)
				Else
					Return New Double?(Double.NegativeInfinity)
				End If
			End If

			If parseBigDecimal Then
				Dim bigDecimalResult As Decimal = digitList.bigDecimal

				If multiplier <> 1 Then
					Try
						bigDecimalResult = bigDecimalResult / bigDecimalMultiplier
					Catch e As ArithmeticException ' non-terminating decimal expansion
						bigDecimalResult = bigDecimalResult.divide(bigDecimalMultiplier, roundingMode)
					End Try
				End If

				If Not status(STATUS_POSITIVE) Then bigDecimalResult = -bigDecimalResult
				Return bigDecimalResult
			Else
				Dim gotDouble As Boolean = True
				Dim gotLongMinimum As Boolean = False
				Dim doubleResult As Double = 0.0
				Dim longResult As Long = 0

				' Finally, have DigitList parse the digits into a value.
				If digitList.fitsIntoLong(status(STATUS_POSITIVE), parseIntegerOnly) Then
					gotDouble = False
					longResult = digitList.long
					If longResult < 0 Then ' got java.lang.[Long].MIN_VALUE gotLongMinimum = True
				Else
					doubleResult = digitList.double
				End If

				' Divide by multiplier. We have to be careful here not to do
				' unneeded conversions between double and java.lang.[Long].
				If multiplier <> 1 Then
					If gotDouble Then
						doubleResult /= multiplier
					Else
						' Avoid converting to double if we can
						If longResult Mod multiplier = 0 Then
							longResult \= multiplier
						Else
							doubleResult = (CDbl(longResult)) / multiplier
							gotDouble = True
						End If
					End If
				End If

				If (Not status(STATUS_POSITIVE)) AndAlso (Not gotLongMinimum) Then
					doubleResult = -doubleResult
					longResult = -longResult
				End If

				' At this point, if we divided the result by the multiplier, the
				' result may fit into a java.lang.[Long].  We check for this case and return
				' a long if possible.
				' We must do this AFTER applying the negative (if appropriate)
				' in order to handle the case of LONG_MIN; otherwise, if we do
				' this with a positive value -LONG_MIN, the double is > 0, but
				' the long is < 0. We also must retain a double in the case of
				' -0.0, which will compare as == to a long 0 cast to a double
				' (bug 4162852).
				If multiplier <> 1 AndAlso gotDouble Then
					longResult = CLng(Fix(doubleResult))
					gotDouble = ((doubleResult <> CDbl(longResult)) OrElse (doubleResult = 0.0 AndAlso 1/doubleResult < 0.0)) AndAlso Not parseIntegerOnly
				End If

				Return If(gotDouble, CType(New Double?(doubleResult), Number), CType(New Long?(longResult), Number))
			End If
		End Function

		''' <summary>
		''' Return a BigInteger multiplier.
		''' </summary>
		Private Property bigIntegerMultiplier As System.Numerics.BigInteger
			Get
				If bigIntegerMultiplier Is Nothing Then bigIntegerMultiplier = System.Numerics.Big java.lang.[Integer].valueOf(multiplier)
				Return bigIntegerMultiplier
			End Get
		End Property
		<NonSerialized> _
		Private bigIntegerMultiplier As System.Numerics.BigInteger

		''' <summary>
		''' Return a BigDecimal multiplier.
		''' </summary>
		Private Property bigDecimalMultiplier As Decimal
			Get
				If bigDecimalMultiplier Is Nothing Then bigDecimalMultiplier = New Decimal(multiplier)
				Return bigDecimalMultiplier
			End Get
		End Property
		<NonSerialized> _
		Private bigDecimalMultiplier As Decimal

		Private Const STATUS_INFINITE As Integer = 0
		Private Const STATUS_POSITIVE As Integer = 1
		Private Const STATUS_LENGTH As Integer = 2

		''' <summary>
		''' Parse the given text into a number.  The text is parsed beginning at
		''' parsePosition, until an unparseable character is seen. </summary>
		''' <param name="text"> The string to parse. </param>
		''' <param name="parsePosition"> The position at which to being parsing.  Upon
		''' return, the first unparseable character. </param>
		''' <param name="digits"> The DigitList to set to the parsed value. </param>
		''' <param name="isExponent"> If true, parse an exponent.  This means no
		''' infinite values and integer only. </param>
		''' <param name="status"> Upon return contains boolean status flags indicating
		''' whether the value was infinite and whether it was positive. </param>
		Private Function subparse(  text As String,   parsePosition As ParsePosition,   positivePrefix As String,   negativePrefix As String,   digits As DigitList,   isExponent As Boolean,   status As Boolean()) As Boolean
			Dim position As Integer = parsePosition.index
			Dim oldStart As Integer = parsePosition.index
			Dim backup As Integer
			Dim gotPositive, gotNegative As Boolean

			' check for positivePrefix; take longest
			gotPositive = text.regionMatches(position, positivePrefix, 0, positivePrefix.length())
			gotNegative = text.regionMatches(position, negativePrefix, 0, negativePrefix.length())

			If gotPositive AndAlso gotNegative Then
				If positivePrefix.length() > negativePrefix.length() Then
					gotNegative = False
				ElseIf positivePrefix.length() < negativePrefix.length() Then
					gotPositive = False
				End If
			End If

			If gotPositive Then
				position += positivePrefix.length()
			ElseIf gotNegative Then
				position += negativePrefix.length()
			Else
				parsePosition.errorIndex = position
				Return False
			End If

			' process digits or Inf, find decimal position
			status(STATUS_INFINITE) = False
			If (Not isExponent) AndAlso text.regionMatches(position,symbols.infinity,0, symbols.infinity.length()) Then
				position += symbols.infinity.length()
				status(STATUS_INFINITE) = True
			Else
				' We now have a string of digits, possibly with grouping symbols,
				' and decimal points.  We want to process these into a DigitList.
				' We don't want to put a bunch of leading zeros into the DigitList
				' though, so we keep track of the location of the decimal point,
				' put only significant digits into the DigitList, and adjust the
				' exponent as needed.

					digits.count = 0
					digits.decimalAt = digits.count
				Dim zero As Char = symbols.zeroDigit
				Dim [decimal] As Char = If(isCurrencyFormat, symbols.monetaryDecimalSeparator, symbols.decimalSeparator)
				Dim grouping As Char = symbols.groupingSeparator
				Dim exponentString As String = symbols.exponentSeparator
				Dim sawDecimal As Boolean = False
				Dim sawExponent As Boolean = False
				Dim sawDigit As Boolean = False
				Dim exponent As Integer = 0 ' Set to the exponent value, if any

				' We have to track digitCount ourselves, because digits.count will
				' pin when the maximum allowable digits is reached.
				Dim digitCount As Integer = 0

				backup = -1
				Do While position < text.length()
					Dim ch As Char = text.Chars(position)

	'                 We recognize all digit ranges, not only the Latin digit range
	'                 * '0'..'9'.  We do so by using the Character.digit() method,
	'                 * which converts a valid Unicode digit to the range 0..9.
	'                 *
	'                 * The character 'ch' may be a digit.  If so, place its value
	'                 * from 0 to 9 in 'digit'.  First try using the locale digit,
	'                 * which may or MAY NOT be a standard Unicode digit range.  If
	'                 * this fails, try using the standard Unicode digit ranges by
	'                 * calling Character.digit().  If this also fails, digit will
	'                 * have a value outside the range 0..9.
	'                 
					Dim digit As Integer = AscW(ch) - AscW(zero)
					If digit < 0 OrElse digit > 9 Then digit = Character.digit(ch, 10)

					If digit = 0 Then
						' Cancel out backup setting (see grouping handler below)
						backup = -1 ' Do this BEFORE continue statement below!!!
						sawDigit = True

						' Handle leading zeros
						If digits.count = 0 Then
							' Ignore leading zeros in integer part of number.
							If Not sawDecimal Then
								position += 1
								Continue Do
							End If

							' If we have seen the decimal, but no significant
							' digits yet, then we account for leading zeros by
							' decrementing the digits.decimalAt into negative
							' values.
							digits.decimalAt -= 1
						Else
							digitCount += 1
							digits.append(ChrW(digit + AscW("0"c)))
						End If ' [sic] digit==0 handled above
					ElseIf digit > 0 AndAlso digit <= 9 Then
						sawDigit = True
						digitCount += 1
						digits.append(ChrW(digit + AscW("0"c)))

						' Cancel out backup setting (see grouping handler below)
						backup = -1
					ElseIf (Not isExponent) AndAlso ch = [decimal] Then
						' If we're only parsing integers, or if we ALREADY saw the
						' decimal, then don't parse this one.
						If parseIntegerOnly OrElse sawDecimal Then Exit Do
						digits.decimalAt = digitCount ' Not digits.count!
						sawDecimal = True
					ElseIf (Not isExponent) AndAlso ch = grouping AndAlso groupingUsed Then
						If sawDecimal Then Exit Do
						' Ignore grouping characters, if we are using them, but
						' require that they be followed by a digit.  Otherwise
						' we backup and reprocess them.
						backup = position
					ElseIf (Not isExponent) AndAlso text.regionMatches(position, exponentString, 0, exponentString.length()) AndAlso (Not sawExponent) Then
						' Process the exponent by recursively calling this method.
						 Dim pos As New ParsePosition(position + exponentString.length())
						Dim stat As Boolean() = New Boolean(STATUS_LENGTH - 1){}
						Dim exponentDigits As New DigitList

						If subparse(text, pos, "", Char.ToString(symbols.minusSign), exponentDigits, True, stat) AndAlso exponentDigits.fitsIntoLong(stat(STATUS_POSITIVE), True) Then
							position = pos.index ' Advance past the exponent
							exponent = CInt(exponentDigits.long)
							If Not stat(STATUS_POSITIVE) Then exponent = -exponent
							sawExponent = True
						End If
						Exit Do ' Whether we fail or succeed, we exit this loop
					Else
						Exit Do
					End If
					position += 1
				Loop

				If backup <> -1 Then position = backup

				' If there was no decimal point we have an integer
				If Not sawDecimal Then digits.decimalAt = digitCount ' Not digits.count!

				' Adjust for exponent, if any
				digits.decimalAt += exponent

				' If none of the text string was recognized.  For example, parse
				' "x" with pattern "#0.00" (return index and error index both 0)
				' parse "$" with pattern "$#0.00". (return index 0 and error
				' index 1).
				If (Not sawDigit) AndAlso digitCount = 0 Then
					parsePosition.index = oldStart
					parsePosition.errorIndex = oldStart
					Return False
				End If
			End If

			' check for suffix
			If Not isExponent Then
				If gotPositive Then gotPositive = text.regionMatches(position,positiveSuffix,0, positiveSuffix.length())
				If gotNegative Then gotNegative = text.regionMatches(position,negativeSuffix,0, negativeSuffix.length())

			' if both match, take longest
			If gotPositive AndAlso gotNegative Then
				If positiveSuffix.length() > negativeSuffix.length() Then
					gotNegative = False
				ElseIf positiveSuffix.length() < negativeSuffix.length() Then
					gotPositive = False
				End If
			End If

			' fail if neither or both
			If gotPositive = gotNegative Then
				parsePosition.errorIndex = position
				Return False
			End If

			parsePosition.index = position + (If(gotPositive, positiveSuffix.length(), negativeSuffix.length())) ' mark success!
			Else
				parsePosition.index = position
			End If

			status(STATUS_POSITIVE) = gotPositive
			If parsePosition.index = oldStart Then
				parsePosition.errorIndex = position
				Return False
			End If
			Return True
		End Function

		''' <summary>
		''' Returns a copy of the decimal format symbols, which is generally not
		''' changed by the programmer or user. </summary>
		''' <returns> a copy of the desired DecimalFormatSymbols </returns>
		''' <seealso cref= java.text.DecimalFormatSymbols </seealso>
		Public Overridable Property decimalFormatSymbols As DecimalFormatSymbols
			Get
				Try
					' don't allow multiple references
					Return CType(symbols.clone(), DecimalFormatSymbols)
				Catch foo As Exception
					Return Nothing ' should never happen
				End Try
			End Get
			Set(  newSymbols As DecimalFormatSymbols)
				Try
					' don't allow multiple references
					symbols = CType(newSymbols.clone(), DecimalFormatSymbols)
					expandAffixes()
					fastPathCheckNeeded = True
				Catch foo As Exception
					' should never happen
				End Try
			End Set
		End Property



		''' <summary>
		''' Get the positive prefix.
		''' <P>Examples: +123, $123, sFr123
		''' </summary>
		''' <returns> the positive prefix </returns>
		Public Overridable Property positivePrefix As String
			Get
				Return positivePrefix
			End Get
			Set(  newValue As String)
				positivePrefix = newValue
				posPrefixPattern = Nothing
				positivePrefixFieldPositions = Nothing
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Returns the FieldPositions of the fields in the prefix used for
		''' positive numbers. This is not used if the user has explicitly set
		''' a positive prefix via <code>setPositivePrefix</code>. This is
		''' lazily created.
		''' </summary>
		''' <returns> FieldPositions in positive prefix </returns>
		Private Property positivePrefixFieldPositions As FieldPosition()
			Get
				If positivePrefixFieldPositions Is Nothing Then
					If posPrefixPattern IsNot Nothing Then
						positivePrefixFieldPositions = expandAffix(posPrefixPattern)
					Else
						positivePrefixFieldPositions = EmptyFieldPositionArray
					End If
				End If
				Return positivePrefixFieldPositions
			End Get
		End Property

		''' <summary>
		''' Get the negative prefix.
		''' <P>Examples: -123, ($123) (with negative suffix), sFr-123
		''' </summary>
		''' <returns> the negative prefix </returns>
		Public Overridable Property negativePrefix As String
			Get
				Return negativePrefix
			End Get
			Set(  newValue As String)
				negativePrefix = newValue
				negPrefixPattern = Nothing
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Returns the FieldPositions of the fields in the prefix used for
		''' negative numbers. This is not used if the user has explicitly set
		''' a negative prefix via <code>setNegativePrefix</code>. This is
		''' lazily created.
		''' </summary>
		''' <returns> FieldPositions in positive prefix </returns>
		Private Property negativePrefixFieldPositions As FieldPosition()
			Get
				If negativePrefixFieldPositions Is Nothing Then
					If negPrefixPattern IsNot Nothing Then
						negativePrefixFieldPositions = expandAffix(negPrefixPattern)
					Else
						negativePrefixFieldPositions = EmptyFieldPositionArray
					End If
				End If
				Return negativePrefixFieldPositions
			End Get
		End Property

		''' <summary>
		''' Get the positive suffix.
		''' <P>Example: 123%
		''' </summary>
		''' <returns> the positive suffix </returns>
		Public Overridable Property positiveSuffix As String
			Get
				Return positiveSuffix
			End Get
			Set(  newValue As String)
				positiveSuffix = newValue
				posSuffixPattern = Nothing
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Returns the FieldPositions of the fields in the suffix used for
		''' positive numbers. This is not used if the user has explicitly set
		''' a positive suffix via <code>setPositiveSuffix</code>. This is
		''' lazily created.
		''' </summary>
		''' <returns> FieldPositions in positive prefix </returns>
		Private Property positiveSuffixFieldPositions As FieldPosition()
			Get
				If positiveSuffixFieldPositions Is Nothing Then
					If posSuffixPattern IsNot Nothing Then
						positiveSuffixFieldPositions = expandAffix(posSuffixPattern)
					Else
						positiveSuffixFieldPositions = EmptyFieldPositionArray
					End If
				End If
				Return positiveSuffixFieldPositions
			End Get
		End Property

		''' <summary>
		''' Get the negative suffix.
		''' <P>Examples: -123%, ($123) (with positive suffixes)
		''' </summary>
		''' <returns> the negative suffix </returns>
		Public Overridable Property negativeSuffix As String
			Get
				Return negativeSuffix
			End Get
			Set(  newValue As String)
				negativeSuffix = newValue
				negSuffixPattern = Nothing
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Returns the FieldPositions of the fields in the suffix used for
		''' negative numbers. This is not used if the user has explicitly set
		''' a negative suffix via <code>setNegativeSuffix</code>. This is
		''' lazily created.
		''' </summary>
		''' <returns> FieldPositions in positive prefix </returns>
		Private Property negativeSuffixFieldPositions As FieldPosition()
			Get
				If negativeSuffixFieldPositions Is Nothing Then
					If negSuffixPattern IsNot Nothing Then
						negativeSuffixFieldPositions = expandAffix(negSuffixPattern)
					Else
						negativeSuffixFieldPositions = EmptyFieldPositionArray
					End If
				End If
				Return negativeSuffixFieldPositions
			End Get
		End Property

		''' <summary>
		''' Gets the multiplier for use in percent, per mille, and similar
		''' formats.
		''' </summary>
		''' <returns> the multiplier </returns>
		''' <seealso cref= #setMultiplier(int) </seealso>
		Public Overridable Property multiplier As Integer
			Get
				Return multiplier
			End Get
			Set(  newValue As Integer)
				multiplier = newValue
				bigDecimalMultiplier = Nothing
				bigIntegerMultiplier = Nothing
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property groupingUsed As Boolean
			Set(  newValue As Boolean)
				MyBase.groupingUsed = newValue
				fastPathCheckNeeded = True
			End Set
		End Property

		''' <summary>
		''' Return the grouping size. Grouping size is the number of digits between
		''' grouping separators in the integer portion of a number.  For example,
		''' in the number "123,456.78", the grouping size is 3.
		''' </summary>
		''' <returns> the grouping size </returns>
		''' <seealso cref= #setGroupingSize </seealso>
		''' <seealso cref= java.text.NumberFormat#isGroupingUsed </seealso>
		''' <seealso cref= java.text.DecimalFormatSymbols#getGroupingSeparator </seealso>
		Public Overridable Property groupingSize As Integer
			Get
				Return groupingSize
			End Get
			Set(  newValue As Integer)
				groupingSize = CByte(newValue)
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Allows you to get the behavior of the decimal separator with integers.
		''' (The decimal separator will always appear with decimals.)
		''' <P>Example: Decimal ON: 12345 &rarr; 12345.; OFF: 12345 &rarr; 12345
		''' </summary>
		''' <returns> {@code true} if the decimal separator is always shown;
		'''         {@code false} otherwise </returns>
		Public Overridable Property decimalSeparatorAlwaysShown As Boolean
			Get
				Return decimalSeparatorAlwaysShown
			End Get
			Set(  newValue As Boolean)
				decimalSeparatorAlwaysShown = newValue
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Returns whether the <seealso cref="#parse(java.lang.String, java.text.ParsePosition)"/>
		''' method returns <code>BigDecimal</code>. The default value is false.
		''' </summary>
		''' <returns> {@code true} if the parse method returns BigDecimal;
		'''         {@code false} otherwise </returns>
		''' <seealso cref= #setParseBigDecimal
		''' @since 1.5 </seealso>
		Public Overridable Property parseBigDecimal As Boolean
			Get
				Return parseBigDecimal
			End Get
			Set(  newValue As Boolean)
				parseBigDecimal = newValue
			End Set
		End Property


		''' <summary>
		''' Standard override; no change in semantics.
		''' </summary>
		Public Overrides Function clone() As Object
			Dim other As DecimalFormat = CType(MyBase.clone(), DecimalFormat)
			other.symbols = CType(symbols.clone(), DecimalFormatSymbols)
			other.digitList = CType(digitList.clone(), DigitList)

			' Fast-path is almost stateless algorithm. The only logical state is the
			' isFastPath flag. In addition fastPathCheckNeeded is a sentinel flag
			' that forces recalculation of all fast-path fields when set to true.
			'
			' There is thus no need to clone all the fast-path fields.
			' We just only need to set fastPathCheckNeeded to true when cloning,
			' and init fastPathData to null as if it were a truly new instance.
			' Every fast-path field will be recalculated (only once) at next usage of
			' fast-path algorithm.
			other.fastPathCheckNeeded = True
			other.isFastPath = False
			other.fastPathData = Nothing

			Return other
		End Function

		''' <summary>
		''' Overrides equals
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Not MyBase.Equals(obj) Then Return False ' super does class check
			Dim other As DecimalFormat = CType(obj, DecimalFormat)
			Return ((posPrefixPattern = other.posPrefixPattern AndAlso positivePrefix.Equals(other.positivePrefix)) OrElse (posPrefixPattern IsNot Nothing AndAlso posPrefixPattern.Equals(other.posPrefixPattern))) AndAlso ((posSuffixPattern = other.posSuffixPattern AndAlso positiveSuffix.Equals(other.positiveSuffix)) OrElse (posSuffixPattern IsNot Nothing AndAlso posSuffixPattern.Equals(other.posSuffixPattern))) AndAlso ((negPrefixPattern = other.negPrefixPattern AndAlso negativePrefix.Equals(other.negativePrefix)) OrElse (negPrefixPattern IsNot Nothing AndAlso negPrefixPattern.Equals(other.negPrefixPattern))) AndAlso ((negSuffixPattern = other.negSuffixPattern AndAlso negativeSuffix.Equals(other.negativeSuffix)) OrElse (negSuffixPattern IsNot Nothing AndAlso negSuffixPattern.Equals(other.negSuffixPattern))) AndAlso multiplier = other.multiplier AndAlso groupingSize = other.groupingSize AndAlso decimalSeparatorAlwaysShown = other.decimalSeparatorAlwaysShown AndAlso parseBigDecimal = other.parseBigDecimal AndAlso useExponentialNotation = other.useExponentialNotation AndAlso ((Not useExponentialNotation) OrElse minExponentDigits = other.minExponentDigits) AndAlso maximumIntegerDigits = other.maximumIntegerDigits AndAlso minimumIntegerDigits = other.minimumIntegerDigits AndAlso maximumFractionDigits = other.maximumFractionDigits AndAlso minimumFractionDigits = other.minimumFractionDigits AndAlso roundingMode = other.roundingMode AndAlso symbols.Equals(other.symbols)
		End Function

		''' <summary>
		''' Overrides hashCode
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode() * 37 + positivePrefix.GetHashCode()
			' just enough fields for a reasonable distribution
		End Function

		''' <summary>
		''' Synthesizes a pattern string that represents the current state
		''' of this Format object.
		''' </summary>
		''' <returns> a pattern string </returns>
		''' <seealso cref= #applyPattern </seealso>
		Public Overridable Function toPattern() As String
			Return toPattern(False)
		End Function

		''' <summary>
		''' Synthesizes a localized pattern string that represents the current
		''' state of this Format object.
		''' </summary>
		''' <returns> a localized pattern string </returns>
		''' <seealso cref= #applyPattern </seealso>
		Public Overridable Function toLocalizedPattern() As String
			Return toPattern(True)
		End Function

		''' <summary>
		''' Expand the affix pattern strings into the expanded affix strings.  If any
		''' affix pattern string is null, do not expand it.  This method should be
		''' called any time the symbols or the affix patterns change in order to keep
		''' the expanded affix strings up to date.
		''' </summary>
		Private Sub expandAffixes()
			' Reuse one StringBuffer for better performance
			Dim buffer As New StringBuffer
			If posPrefixPattern IsNot Nothing Then
				positivePrefix = expandAffix(posPrefixPattern, buffer)
				positivePrefixFieldPositions = Nothing
			End If
			If posSuffixPattern IsNot Nothing Then
				positiveSuffix = expandAffix(posSuffixPattern, buffer)
				positiveSuffixFieldPositions = Nothing
			End If
			If negPrefixPattern IsNot Nothing Then
				negativePrefix = expandAffix(negPrefixPattern, buffer)
				negativePrefixFieldPositions = Nothing
			End If
			If negSuffixPattern IsNot Nothing Then
				negativeSuffix = expandAffix(negSuffixPattern, buffer)
				negativeSuffixFieldPositions = Nothing
			End If
		End Sub

		''' <summary>
		''' Expand an affix pattern into an affix string.  All characters in the
		''' pattern are literal unless prefixed by QUOTE.  The following characters
		''' after QUOTE are recognized: PATTERN_PERCENT, PATTERN_PER_MILLE,
		''' PATTERN_MINUS, and CURRENCY_SIGN.  If CURRENCY_SIGN is doubled (QUOTE +
		''' CURRENCY_SIGN + CURRENCY_SIGN), it is interpreted as an ISO 4217
		''' currency code.  Any other character after a QUOTE represents itself.
		''' QUOTE must be followed by another character; QUOTE may not occur by
		''' itself at the end of the pattern.
		''' </summary>
		''' <param name="pattern"> the non-null, possibly empty pattern </param>
		''' <param name="buffer"> a scratch StringBuffer; its contents will be lost </param>
		''' <returns> the expanded equivalent of pattern </returns>
		Private Function expandAffix(  pattern As String,   buffer As StringBuffer) As String
			buffer.length = 0
			Dim i As Integer=0
			Do While i<pattern.length()
				Dim c As Char = pattern.Chars(i)
				i += 1
				If c = QUOTE Then
					c = pattern.Chars(i)
					i += 1
					Select Case c
					Case CURRENCY_SIGN
						If i<pattern.length() AndAlso pattern.Chars(i) = CURRENCY_SIGN Then
							i += 1
							buffer.append(symbols.internationalCurrencySymbol)
						Else
							buffer.append(symbols.currencySymbol)
						End If
						Continue Do
					Case PATTERN_PERCENT
						c = symbols.percent
					Case PATTERN_PER_MILLE
						c = symbols.perMill
					Case PATTERN_MINUS
						c = symbols.minusSign
					End Select
				End If
				buffer.append(c)
			Loop
			Return buffer.ToString()
		End Function

		''' <summary>
		''' Expand an affix pattern into an array of FieldPositions describing
		''' how the pattern would be expanded.
		''' All characters in the
		''' pattern are literal unless prefixed by QUOTE.  The following characters
		''' after QUOTE are recognized: PATTERN_PERCENT, PATTERN_PER_MILLE,
		''' PATTERN_MINUS, and CURRENCY_SIGN.  If CURRENCY_SIGN is doubled (QUOTE +
		''' CURRENCY_SIGN + CURRENCY_SIGN), it is interpreted as an ISO 4217
		''' currency code.  Any other character after a QUOTE represents itself.
		''' QUOTE must be followed by another character; QUOTE may not occur by
		''' itself at the end of the pattern.
		''' </summary>
		''' <param name="pattern"> the non-null, possibly empty pattern </param>
		''' <returns> FieldPosition array of the resulting fields. </returns>
		Private Function expandAffix(  pattern As String) As FieldPosition()
			Dim positions As List(Of FieldPosition) = Nothing
			Dim stringIndex As Integer = 0
			Dim i As Integer=0
			Do While i<pattern.length()
				Dim c As Char = pattern.Chars(i)
				i += 1
				If c = QUOTE Then
					Dim field_Renamed As Integer = -1
					Dim fieldID As Format.Field = Nothing
					c = pattern.Chars(i)
					i += 1
					Select Case c
					Case CURRENCY_SIGN
						Dim string_Renamed As String
						If i<pattern.length() AndAlso pattern.Chars(i) = CURRENCY_SIGN Then
							i += 1
							string_Renamed = symbols.internationalCurrencySymbol
						Else
							string_Renamed = symbols.currencySymbol
						End If
						If string_Renamed.length() > 0 Then
							If positions Is Nothing Then positions = New List(Of )(2)
							Dim fp As New FieldPosition(Field.CURRENCY)
							fp.beginIndex = stringIndex
							fp.endIndex = stringIndex + string_Renamed.length()
							positions.Add(fp)
							stringIndex += string_Renamed.length()
						End If
						Continue Do
					Case PATTERN_PERCENT
						c = symbols.percent
						field_Renamed = -1
						fieldID = Field.PERCENT
					Case PATTERN_PER_MILLE
						c = symbols.perMill
						field_Renamed = -1
						fieldID = Field.PERMILLE
					Case PATTERN_MINUS
						c = symbols.minusSign
						field_Renamed = -1
						fieldID = Field.SIGN
					End Select
					If fieldID IsNot Nothing Then
						If positions Is Nothing Then positions = New List(Of )(2)
						Dim fp As New FieldPosition(fieldID, field_Renamed)
						fp.beginIndex = stringIndex
						fp.endIndex = stringIndex + 1
						positions.Add(fp)
					End If
				End If
				stringIndex += 1
			Loop
			If positions IsNot Nothing Then Return positions.ToArray(EmptyFieldPositionArray)
			Return EmptyFieldPositionArray
		End Function

		''' <summary>
		''' Appends an affix pattern to the given StringBuffer, quoting special
		''' characters as needed.  Uses the internal affix pattern, if that exists,
		''' or the literal affix, if the internal affix pattern is null.  The
		''' appended string will generate the same affix pattern (or literal affix)
		''' when passed to toPattern().
		''' </summary>
		''' <param name="buffer"> the affix string is appended to this </param>
		''' <param name="affixPattern"> a pattern such as posPrefixPattern; may be null </param>
		''' <param name="expAffix"> a corresponding expanded affix, such as positivePrefix.
		''' Ignored unless affixPattern is null.  If affixPattern is null, then
		''' expAffix is appended as a literal affix. </param>
		''' <param name="localized"> true if the appended pattern should contain localized
		''' pattern characters; otherwise, non-localized pattern chars are appended </param>
		Private Sub appendAffix(  buffer As StringBuffer,   affixPattern As String,   expAffix As String,   localized As Boolean)
			If affixPattern Is Nothing Then
				appendAffix(buffer, expAffix, localized)
			Else
				Dim i As Integer
				Dim pos As Integer=0
				Do While pos<affixPattern.length()
					i = affixPattern.IndexOf(QUOTE, pos)
					If i < 0 Then
						appendAffix(buffer, affixPattern.Substring(pos), localized)
						Exit Do
					End If
					If i > pos Then appendAffix(buffer, affixPattern.Substring(pos, i - pos), localized)
					i += 1
					Dim c As Char = affixPattern.Chars(i)
					i += 1
					If c = QUOTE Then
						buffer.append(c)
						' Fall through and append another QUOTE below
					ElseIf c = CURRENCY_SIGN AndAlso i<affixPattern.length() AndAlso affixPattern.Chars(i) = CURRENCY_SIGN Then
						i += 1
						buffer.append(c)
						' Fall through and append another CURRENCY_SIGN below
					ElseIf localized Then
						Select Case c
						Case PATTERN_PERCENT
							c = symbols.percent
						Case PATTERN_PER_MILLE
							c = symbols.perMill
						Case PATTERN_MINUS
							c = symbols.minusSign
						End Select
					End If
					buffer.append(c)
					pos=i
				Loop
			End If
		End Sub

		''' <summary>
		''' Append an affix to the given StringBuffer, using quotes if
		''' there are special characters.  Single quotes themselves must be
		''' escaped in either case.
		''' </summary>
		Private Sub appendAffix(  buffer As StringBuffer,   affix As String,   localized As Boolean)
			Dim needQuote As Boolean
			If localized Then
				needQuote = affix.IndexOf(symbols.zeroDigit) >= 0 OrElse affix.IndexOf(symbols.groupingSeparator) >= 0 OrElse affix.IndexOf(symbols.decimalSeparator) >= 0 OrElse affix.IndexOf(symbols.percent) >= 0 OrElse affix.IndexOf(symbols.perMill) >= 0 OrElse affix.IndexOf(symbols.digit) >= 0 OrElse affix.IndexOf(symbols.patternSeparator) >= 0 OrElse affix.IndexOf(symbols.minusSign) >= 0 OrElse affix.IndexOf(CURRENCY_SIGN) >= 0
			Else
				needQuote = affix.IndexOf(PATTERN_ZERO_DIGIT) >= 0 OrElse affix.IndexOf(PATTERN_GROUPING_SEPARATOR) >= 0 OrElse affix.IndexOf(PATTERN_DECIMAL_SEPARATOR) >= 0 OrElse affix.IndexOf(PATTERN_PERCENT) >= 0 OrElse affix.IndexOf(PATTERN_PER_MILLE) >= 0 OrElse affix.IndexOf(PATTERN_DIGIT) >= 0 OrElse affix.IndexOf(PATTERN_SEPARATOR) >= 0 OrElse affix.IndexOf(PATTERN_MINUS) >= 0 OrElse affix.IndexOf(CURRENCY_SIGN) >= 0
			End If
			If needQuote Then buffer.append("'"c)
			If affix.IndexOf("'"c) < 0 Then
				buffer.append(affix)
			Else
				For j As Integer = 0 To affix.length() - 1
					Dim c As Char = affix.Chars(j)
					buffer.append(c)
					If c = "'"c Then buffer.append(c)
				Next j
			End If
			If needQuote Then buffer.append("'"c)
		End Sub

		''' <summary>
		''' Does the real work of generating a pattern.  
		''' </summary>
		Private Function toPattern(  localized As Boolean) As String
			Dim result As New StringBuffer
			For j As Integer = 1 To 0 Step -1
				If j = 1 Then
					appendAffix(result, posPrefixPattern, positivePrefix, localized)
				Else
					appendAffix(result, negPrefixPattern, negativePrefix, localized)
				End If
				Dim i As Integer
				Dim digitCount As Integer = If(useExponentialNotation, maximumIntegerDigits, System.Math.Max(groupingSize, minimumIntegerDigits)+1)
				For i = digitCount To 1 Step -1
					If i <> digitCount AndAlso groupingUsed AndAlso groupingSize <> 0 AndAlso i Mod groupingSize = 0 Then result.append(If(localized, symbols.groupingSeparator, PATTERN_GROUPING_SEPARATOR))
					result.append(If(i <= minimumIntegerDigits, (If(localized, symbols.zeroDigit, PATTERN_ZERO_DIGIT)), (If(localized, symbols.digit, PATTERN_DIGIT))))
				Next i
				If maximumFractionDigits > 0 OrElse decimalSeparatorAlwaysShown Then result.append(If(localized, symbols.decimalSeparator, PATTERN_DECIMAL_SEPARATOR))
				For i = 0 To maximumFractionDigits - 1
					If i < minimumFractionDigits Then
						result.append(If(localized, symbols.zeroDigit, PATTERN_ZERO_DIGIT))
					Else
						result.append(If(localized, symbols.digit, PATTERN_DIGIT))
					End If
				Next i
			If useExponentialNotation Then
				result.append(If(localized, symbols.exponentSeparator, PATTERN_EXPONENT))
			For i = 0 To minExponentDigits - 1
						result.append(If(localized, symbols.zeroDigit, PATTERN_ZERO_DIGIT))
			Next i
			End If
				If j = 1 Then
					appendAffix(result, posSuffixPattern, positiveSuffix, localized)
					If (negSuffixPattern = posSuffixPattern AndAlso negativeSuffix.Equals(positiveSuffix)) OrElse (negSuffixPattern IsNot Nothing AndAlso negSuffixPattern.Equals(posSuffixPattern)) Then ' n == p == null
						If (negPrefixPattern IsNot Nothing AndAlso posPrefixPattern IsNot Nothing AndAlso negPrefixPattern.Equals("'-" & posPrefixPattern)) OrElse (negPrefixPattern = posPrefixPattern AndAlso negativePrefix.Equals(AscW(symbols.minusSign) + positivePrefix)) Then ' n == p == null Exit For
					End If
					result.append(If(localized, symbols.patternSeparator, PATTERN_SEPARATOR))
				Else
					appendAffix(result, negSuffixPattern, negativeSuffix, localized)
				End If
			Next j
			Return result.ToString()
		End Function

		''' <summary>
		''' Apply the given pattern to this Format object.  A pattern is a
		''' short-hand specification for the various formatting properties.
		''' These properties can also be changed individually through the
		''' various setter methods.
		''' <p>
		''' There is no limit to integer digits set
		''' by this routine, since that is the typical end-user desire;
		''' use setMaximumInteger if you want to set a real value.
		''' For negative numbers, use a second pattern, separated by a semicolon
		''' <P>Example <code>"#,#00.0#"</code> &rarr; 1,234.56
		''' <P>This means a minimum of 2 integer digits, 1 fraction digit, and
		''' a maximum of 2 fraction digits.
		''' <p>Example: <code>"#,#00.0#;(#,#00.0#)"</code> for negatives in
		''' parentheses.
		''' <p>In negative patterns, the minimum and maximum counts are ignored;
		''' these are presumed to be set in the positive pattern.
		''' </summary>
		''' <param name="pattern"> a new pattern </param>
		''' <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		Public Overridable Sub applyPattern(  pattern As String)
			applyPattern(pattern, False)
		End Sub

		''' <summary>
		''' Apply the given pattern to this Format object.  The pattern
		''' is assumed to be in a localized notation. A pattern is a
		''' short-hand specification for the various formatting properties.
		''' These properties can also be changed individually through the
		''' various setter methods.
		''' <p>
		''' There is no limit to integer digits set
		''' by this routine, since that is the typical end-user desire;
		''' use setMaximumInteger if you want to set a real value.
		''' For negative numbers, use a second pattern, separated by a semicolon
		''' <P>Example <code>"#,#00.0#"</code> &rarr; 1,234.56
		''' <P>This means a minimum of 2 integer digits, 1 fraction digit, and
		''' a maximum of 2 fraction digits.
		''' <p>Example: <code>"#,#00.0#;(#,#00.0#)"</code> for negatives in
		''' parentheses.
		''' <p>In negative patterns, the minimum and maximum counts are ignored;
		''' these are presumed to be set in the positive pattern.
		''' </summary>
		''' <param name="pattern"> a new pattern </param>
		''' <exception cref="NullPointerException"> if <code>pattern</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if the given pattern is invalid. </exception>
		Public Overridable Sub applyLocalizedPattern(  pattern As String)
			applyPattern(pattern, True)
		End Sub

		''' <summary>
		''' Does the real work of applying a pattern.
		''' </summary>
		Private Sub applyPattern(  pattern As String,   localized As Boolean)
			Dim zeroDigit As Char = PATTERN_ZERO_DIGIT
			Dim groupingSeparator As Char = PATTERN_GROUPING_SEPARATOR
			Dim decimalSeparator As Char = PATTERN_DECIMAL_SEPARATOR
			Dim percent As Char = PATTERN_PERCENT
			Dim perMill As Char = PATTERN_PER_MILLE
			Dim digit As Char = PATTERN_DIGIT
			Dim separator As Char = PATTERN_SEPARATOR
			Dim exponent As String = PATTERN_EXPONENT
			Dim minus As Char = PATTERN_MINUS
			If localized Then
				zeroDigit = symbols.zeroDigit
				groupingSeparator = symbols.groupingSeparator
				decimalSeparator = symbols.decimalSeparator
				percent = symbols.percent
				perMill = symbols.perMill
				digit = symbols.digit
				separator = symbols.patternSeparator
				exponent = symbols.exponentSeparator
				minus = symbols.minusSign
			End If
			Dim gotNegative As Boolean = False
			decimalSeparatorAlwaysShown = False
			isCurrencyFormat = False
			useExponentialNotation = False

			' Two variables are used to record the subrange of the pattern
			' occupied by phase 1.  This is used during the processing of the
			' second pattern (the one representing negative numbers) to ensure
			' that no deviation exists in phase 1 between the two patterns.
			Dim phaseOneStart As Integer = 0
			Dim phaseOneLength As Integer = 0

			Dim start As Integer = 0
			Dim j As Integer = 1
			Do While j >= 0 AndAlso start < pattern.length()
				Dim inQuote As Boolean = False
				Dim prefix As New StringBuffer
				Dim suffix As New StringBuffer
				Dim decimalPos As Integer = -1
				Dim multiplier_Renamed As Integer = 1
				Dim digitLeftCount As Integer = 0, zeroDigitCount As Integer = 0, digitRightCount As Integer = 0
				Dim groupingCount As SByte = -1

				' The phase ranges from 0 to 2.  Phase 0 is the prefix.  Phase 1 is
				' the section of the pattern with digits, decimal separator,
				' grouping characters.  Phase 2 is the suffix.  In phases 0 and 2,
				' percent, per mille, and currency symbols are recognized and
				' translated.  The separation of the characters into phases is
				' strictly enforced; if phase 1 characters are to appear in the
				' suffix, for example, they must be quoted.
				Dim phase As Integer = 0

				' The affix is either the prefix or the suffix.
				Dim affix As StringBuffer = prefix

				For pos As Integer = start To pattern.length() - 1
					Dim ch As Char = pattern.Chars(pos)
					Select Case phase
					Case 0, 2
						' Process the prefix / suffix characters
						If inQuote Then
							' A quote within quotes indicates either the closing
							' quote or two quotes, which is a quote literal. That
							' is, we have the second quote in 'do' or 'don''t'.
							If ch = QUOTE Then
								If (pos+1) < pattern.length() AndAlso pattern.Chars(pos+1) = QUOTE Then
									pos += 1
									affix.append("''") ' 'don''t'
								Else
									inQuote = False ' 'do'
								End If
								Continue For
							End If
						Else
							' Process unquoted characters seen in prefix or suffix
							' phase.
							If ch = digit OrElse ch = zeroDigit OrElse ch = groupingSeparator OrElse ch = decimalSeparator Then
								phase = 1
								If j = 1 Then phaseOneStart = pos
								pos -= 1 ' Reprocess this character
								Continue For
							ElseIf ch = CURRENCY_SIGN Then
								' Use lookahead to determine if the currency sign
								' is doubled or not.
								Dim doubled As Boolean = (pos + 1) < pattern.length() AndAlso pattern.Chars(pos + 1) = CURRENCY_SIGN
								If doubled Then ' Skip over the doubled character pos += 1
								isCurrencyFormat = True
								affix.append(If(doubled, "'" & ChrW(&H00A4).ToString() & ChrW(&H00A4).ToString(), "'" & ChrW(&H00A4).ToString()))
								Continue For
							ElseIf ch = QUOTE Then
								' A quote outside quotes indicates either the
								' opening quote or two quotes, which is a quote
								' literal. That is, we have the first quote in 'do'
								' or o''clock.
								If ch = QUOTE Then
									If (pos+1) < pattern.length() AndAlso pattern.Chars(pos+1) = QUOTE Then
										pos += 1
										affix.append("''") ' o''clock
									Else
										inQuote = True ' 'do'
									End If
									Continue For
								End If
							ElseIf ch = separator Then
								' Don't allow separators before we see digit
								' characters of phase 1, and don't allow separators
								' in the second pattern (j == 0).
								If phase = 0 OrElse j = 0 Then Throw New IllegalArgumentException("Unquoted special character '" & AscW(ch) & "' in pattern """ & pattern & """"c)
								start = pos + 1
								pos = pattern.length()
								Continue For

							' Next handle characters which are appended directly.
							ElseIf ch = percent Then
								If multiplier_Renamed <> 1 Then Throw New IllegalArgumentException("Too many percent/per mille characters in pattern """ & pattern & """"c)
								multiplier_Renamed = 100
								affix.append("'%")
								Continue For
							ElseIf ch = perMill Then
								If multiplier_Renamed <> 1 Then Throw New IllegalArgumentException("Too many percent/per mille characters in pattern """ & pattern & """"c)
								multiplier_Renamed = 1000
								affix.append("'" & ChrW(&H2030).ToString())
								Continue For
							ElseIf ch = minus Then
								affix.append("'-")
								Continue For
							End If
						End If
						' Note that if we are within quotes, or if this is an
						' unquoted, non-special character, then we usually fall
						' through to here.
						affix.append(ch)

					Case 1
						' Phase one must be identical in the two sub-patterns. We
						' enforce this by doing a direct comparison. While
						' processing the first sub-pattern, we just record its
						' length. While processing the second, we compare
						' characters.
						If j = 1 Then
							phaseOneLength += 1
						Else
							phaseOneLength -= 1
							If phaseOneLength = 0 Then
								phase = 2
								affix = suffix
							End If
							Continue For
						End If

						' Process the digits, decimal, and grouping characters. We
						' record five pieces of information. We expect the digits
						' to occur in the pattern ####0000.####, and we record the
						' number of left digits, zero (central) digits, and right
						' digits. The position of the last grouping character is
						' recorded (should be somewhere within the first two blocks
						' of characters), as is the position of the decimal point,
						' if any (should be in the zero digits). If there is no
						' decimal point, then there should be no right digits.
						If ch = digit Then
							If zeroDigitCount > 0 Then
								digitRightCount += 1
							Else
								digitLeftCount += 1
							End If
							If groupingCount >= 0 AndAlso decimalPos < 0 Then groupingCount += 1
						ElseIf ch = zeroDigit Then
							If digitRightCount > 0 Then Throw New IllegalArgumentException("Unexpected '0' in pattern """ & pattern & """"c)
							zeroDigitCount += 1
							If groupingCount >= 0 AndAlso decimalPos < 0 Then groupingCount += 1
						ElseIf ch = groupingSeparator Then
							groupingCount = 0
						ElseIf ch = decimalSeparator Then
							If decimalPos >= 0 Then Throw New IllegalArgumentException("Multiple decimal separators in pattern """ & pattern & """"c)
							decimalPos = digitLeftCount + zeroDigitCount + digitRightCount
						ElseIf pattern.regionMatches(pos, exponent, 0, exponent.length()) Then
							If useExponentialNotation Then Throw New IllegalArgumentException("Multiple exponential " & "symbols in pattern """ & pattern & """"c)
							useExponentialNotation = True
							minExponentDigits = 0

							' Use lookahead to parse out the exponential part
							' of the pattern, then jump into phase 2.
							pos = pos+exponent.length()
							 Do While pos < pattern.length() AndAlso pattern.Chars(pos) = zeroDigit
								minExponentDigits += 1
								phaseOneLength += 1
								pos += 1
							 Loop

							If (digitLeftCount + zeroDigitCount) < 1 OrElse minExponentDigits < 1 Then Throw New IllegalArgumentException("Malformed exponential " & "pattern """ & pattern & """"c)

							' Transition to phase 2
							phase = 2
							affix = suffix
							pos -= 1
							Continue For
						Else
							phase = 2
							affix = suffix
							pos -= 1
							phaseOneLength -= 1
							Continue For
						End If
					End Select
				Next pos

				' Handle patterns with no '0' pattern character. These patterns
				' are legal, but must be interpreted.  "##.###" -> "#0.###".
				' ".###" -> ".0##".
	'             We allow patterns of the form "####" to produce a zeroDigitCount
	'             * of zero (got that?); although this seems like it might make it
	'             * possible for format() to produce empty strings, format() checks
	'             * for this condition and outputs a zero digit in this situation.
	'             * Having a zeroDigitCount of zero yields a minimum integer digits
	'             * of zero, which allows proper round-trip patterns.  That is, we
	'             * don't want "#" to become "#0" when toPattern() is called (even
	'             * though that's what it really is, semantically).
	'             
				If zeroDigitCount = 0 AndAlso digitLeftCount > 0 AndAlso decimalPos >= 0 Then
					' Handle "###.###" and "###." and ".###"
					Dim n As Integer = decimalPos
					If n = 0 Then ' Handle ".###" n += 1
					digitRightCount = digitLeftCount - n
					digitLeftCount = n - 1
					zeroDigitCount = 1
				End If

				' Do syntax checking on the digits.
				If (decimalPos < 0 AndAlso digitRightCount > 0) OrElse (decimalPos >= 0 AndAlso (decimalPos < digitLeftCount OrElse decimalPos > (digitLeftCount + zeroDigitCount))) OrElse groupingCount = 0 OrElse inQuote Then Throw New IllegalArgumentException("Malformed pattern """ & pattern & """"c)

				If j = 1 Then
					posPrefixPattern = prefix.ToString()
					posSuffixPattern = suffix.ToString()
					negPrefixPattern = posPrefixPattern ' assume these for now
					negSuffixPattern = posSuffixPattern
					Dim digitTotalCount As Integer = digitLeftCount + zeroDigitCount + digitRightCount
	'                 The effectiveDecimalPos is the position the decimal is at or
	'                 * would be at if there is no decimal. Note that if decimalPos<0,
	'                 * then digitTotalCount == digitLeftCount + zeroDigitCount.
	'                 
					Dim effectiveDecimalPos As Integer = If(decimalPos >= 0, decimalPos, digitTotalCount)
					minimumIntegerDigits = effectiveDecimalPos - digitLeftCount
					maximumIntegerDigits = If(useExponentialNotation, digitLeftCount + minimumIntegerDigits, MAXIMUM_INTEGER_DIGITS)
					maximumFractionDigits = If(decimalPos >= 0, (digitTotalCount - decimalPos), 0)
					minimumFractionDigits = If(decimalPos >= 0, (digitLeftCount + zeroDigitCount - decimalPos), 0)
					groupingUsed = groupingCount > 0
					Me.groupingSize = If(groupingCount > 0, groupingCount, 0)
					Me.multiplier = multiplier_Renamed
					decimalSeparatorAlwaysShown = decimalPos = 0 OrElse decimalPos = digitTotalCount
				Else
					negPrefixPattern = prefix.ToString()
					negSuffixPattern = suffix.ToString()
					gotNegative = True
				End If
				j -= 1
			Loop

			If pattern.length() = 0 Then
					posSuffixPattern = ""
					posPrefixPattern = posSuffixPattern
				minimumIntegerDigits = 0
				maximumIntegerDigits = MAXIMUM_INTEGER_DIGITS
				minimumFractionDigits = 0
				maximumFractionDigits = MAXIMUM_FRACTION_DIGITS
			End If

			' If there was no negative pattern, or if the negative pattern is
			' identical to the positive pattern, then prepend the minus sign to
			' the positive pattern to form the negative pattern.
			If (Not gotNegative) OrElse (negPrefixPattern.Equals(posPrefixPattern) AndAlso negSuffixPattern.Equals(posSuffixPattern)) Then
				negSuffixPattern = posSuffixPattern
				negPrefixPattern = "'-" & posPrefixPattern
			End If

			expandAffixes()
		End Sub

		''' <summary>
		''' Sets the maximum number of digits allowed in the integer portion of a
		''' number.
		''' For formatting numbers other than <code>BigInteger</code> and
		''' <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		''' 309 is used. Negative input values are replaced with 0. </summary>
		''' <seealso cref= NumberFormat#setMaximumIntegerDigits </seealso>
		Public Overrides Property maximumIntegerDigits As Integer
			Set(  newValue As Integer)
				maximumIntegerDigits = System.Math.Min (System.Math.Max(0, newValue), MAXIMUM_INTEGER_DIGITS)
				MyBase.maximumIntegerDigits = If(maximumIntegerDigits > DOUBLE_INTEGER_DIGITS, DOUBLE_INTEGER_DIGITS, maximumIntegerDigits)
				If minimumIntegerDigits > maximumIntegerDigits Then
					minimumIntegerDigits = maximumIntegerDigits
					MyBase.minimumIntegerDigits = If(minimumIntegerDigits > DOUBLE_INTEGER_DIGITS, DOUBLE_INTEGER_DIGITS, minimumIntegerDigits)
				End If
				fastPathCheckNeeded = True
			End Set
			Get
				Return maximumIntegerDigits
			End Get
		End Property

		''' <summary>
		''' Sets the minimum number of digits allowed in the integer portion of a
		''' number.
		''' For formatting numbers other than <code>BigInteger</code> and
		''' <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		''' 309 is used. Negative input values are replaced with 0. </summary>
		''' <seealso cref= NumberFormat#setMinimumIntegerDigits </seealso>
		Public Overrides Property minimumIntegerDigits As Integer
			Set(  newValue As Integer)
				minimumIntegerDigits = System.Math.Min (System.Math.Max(0, newValue), MAXIMUM_INTEGER_DIGITS)
				MyBase.minimumIntegerDigits = If(minimumIntegerDigits > DOUBLE_INTEGER_DIGITS, DOUBLE_INTEGER_DIGITS, minimumIntegerDigits)
				If minimumIntegerDigits > maximumIntegerDigits Then
					maximumIntegerDigits = minimumIntegerDigits
					MyBase.maximumIntegerDigits = If(maximumIntegerDigits > DOUBLE_INTEGER_DIGITS, DOUBLE_INTEGER_DIGITS, maximumIntegerDigits)
				End If
				fastPathCheckNeeded = True
			End Set
			Get
				Return minimumIntegerDigits
			End Get
		End Property

		''' <summary>
		''' Sets the maximum number of digits allowed in the fraction portion of a
		''' number.
		''' For formatting numbers other than <code>BigInteger</code> and
		''' <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		''' 340 is used. Negative input values are replaced with 0. </summary>
		''' <seealso cref= NumberFormat#setMaximumFractionDigits </seealso>
		Public Overrides Property maximumFractionDigits As Integer
			Set(  newValue As Integer)
				maximumFractionDigits = System.Math.Min (System.Math.Max(0, newValue), MAXIMUM_FRACTION_DIGITS)
				MyBase.maximumFractionDigits = If(maximumFractionDigits > DOUBLE_FRACTION_DIGITS, DOUBLE_FRACTION_DIGITS, maximumFractionDigits)
				If minimumFractionDigits > maximumFractionDigits Then
					minimumFractionDigits = maximumFractionDigits
					MyBase.minimumFractionDigits = If(minimumFractionDigits > DOUBLE_FRACTION_DIGITS, DOUBLE_FRACTION_DIGITS, minimumFractionDigits)
				End If
				fastPathCheckNeeded = True
			End Set
			Get
				Return maximumFractionDigits
			End Get
		End Property

		''' <summary>
		''' Sets the minimum number of digits allowed in the fraction portion of a
		''' number.
		''' For formatting numbers other than <code>BigInteger</code> and
		''' <code>BigDecimal</code> objects, the lower of <code>newValue</code> and
		''' 340 is used. Negative input values are replaced with 0. </summary>
		''' <seealso cref= NumberFormat#setMinimumFractionDigits </seealso>
		Public Overrides Property minimumFractionDigits As Integer
			Set(  newValue As Integer)
				minimumFractionDigits = System.Math.Min (System.Math.Max(0, newValue), MAXIMUM_FRACTION_DIGITS)
				MyBase.minimumFractionDigits = If(minimumFractionDigits > DOUBLE_FRACTION_DIGITS, DOUBLE_FRACTION_DIGITS, minimumFractionDigits)
				If minimumFractionDigits > maximumFractionDigits Then
					maximumFractionDigits = minimumFractionDigits
					MyBase.maximumFractionDigits = If(maximumFractionDigits > DOUBLE_FRACTION_DIGITS, DOUBLE_FRACTION_DIGITS, maximumFractionDigits)
				End If
				fastPathCheckNeeded = True
			End Set
			Get
				Return minimumFractionDigits
			End Get
		End Property





		''' <summary>
		''' Gets the currency used by this decimal format when formatting
		''' currency values.
		''' The currency is obtained by calling
		''' <seealso cref="DecimalFormatSymbols#getCurrency DecimalFormatSymbols.getCurrency"/>
		''' on this number format's symbols.
		''' </summary>
		''' <returns> the currency used by this decimal format, or <code>null</code>
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  currency As java.util.Currency
			Get
				Return symbols.currency
			End Get
			Set(  currency As java.util.Currency)
				If currency IsNot symbols.currency Then
					symbols.currency = currency
					If isCurrencyFormat Then expandAffixes()
				End If
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Gets the <seealso cref="java.math.RoundingMode"/> used in this DecimalFormat.
		''' </summary>
		''' <returns> The <code>RoundingMode</code> used for this DecimalFormat. </returns>
		''' <seealso cref= #setRoundingMode(RoundingMode)
		''' @since 1.6 </seealso>
		Public  Overrides ReadOnly Property  roundingMode As java.math.RoundingMode
			Get
				Return roundingMode
			End Get
			Set(  roundingMode As java.math.RoundingMode)
				If roundingMode Is Nothing Then Throw New NullPointerException
    
				Me.roundingMode = roundingMode
				digitList.roundingMode = roundingMode
				fastPathCheckNeeded = True
			End Set
		End Property


		''' <summary>
		''' Reads the default serializable fields from the stream and performs
		''' validations and adjustments for older serialized versions. The
		''' validations and adjustments are:
		''' <ol>
		''' <li>
		''' Verify that the superclass's digit count fields correctly reflect
		''' the limits imposed on formatting numbers other than
		''' <code>BigInteger</code> and <code>BigDecimal</code> objects. These
		''' limits are stored in the superclass for serialization compatibility
		''' with older versions, while the limits for <code>BigInteger</code> and
		''' <code>BigDecimal</code> objects are kept in this class.
		''' If, in the superclass, the minimum or maximum integer digit count is
		''' larger than <code>DOUBLE_INTEGER_DIGITS</code> or if the minimum or
		''' maximum fraction digit count is larger than
		''' <code>DOUBLE_FRACTION_DIGITS</code>, then the stream data is invalid
		''' and this method throws an <code>InvalidObjectException</code>.
		''' <li>
		''' If <code>serialVersionOnStream</code> is less than 4, initialize
		''' <code>roundingMode</code> to {@link java.math.RoundingMode#HALF_EVEN
		''' RoundingMode.HALF_EVEN}.  This field is new with version 4.
		''' <li>
		''' If <code>serialVersionOnStream</code> is less than 3, then call
		''' the setters for the minimum and maximum integer and fraction digits with
		''' the values of the corresponding superclass getters to initialize the
		''' fields in this class. The fields in this class are new with version 3.
		''' <li>
		''' If <code>serialVersionOnStream</code> is less than 1, indicating that
		''' the stream was written by JDK 1.1, initialize
		''' <code>useExponentialNotation</code>
		''' to false, since it was not present in JDK 1.1.
		''' <li>
		''' Set <code>serialVersionOnStream</code> to the maximum allowed value so
		''' that default serialization will work properly if this object is streamed
		''' out again.
		''' </ol>
		''' 
		''' <p>Stream versions older than 2 will not have the affix pattern variables
		''' <code>posPrefixPattern</code> etc.  As a result, they will be initialized
		''' to <code>null</code>, which means the affix strings will be taken as
		''' literal values.  This is exactly what we want, since that corresponds to
		''' the pre-version-2 behavior.
		''' </summary>
		Private Sub readObject(  stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			digitList = New DigitList

			' We force complete fast-path reinitialization when the instance is
			' deserialized. See clone() comment on fastPathCheckNeeded.
			fastPathCheckNeeded = True
			isFastPath = False
			fastPathData = Nothing

			If serialVersionOnStream < 4 Then
				roundingMode = java.math.RoundingMode.HALF_EVEN
			Else
				roundingMode = roundingMode
			End If

			' We only need to check the maximum counts because NumberFormat
			' .readObject has already ensured that the maximum is greater than the
			' minimum count.
			If MyBase.maximumIntegerDigits > DOUBLE_INTEGER_DIGITS OrElse MyBase.maximumFractionDigits > DOUBLE_FRACTION_DIGITS Then Throw New java.io.InvalidObjectException("Digit count out of range")
			If serialVersionOnStream < 3 Then
				maximumIntegerDigits = MyBase.maximumIntegerDigits
				minimumIntegerDigits = MyBase.minimumIntegerDigits
				maximumFractionDigits = MyBase.maximumFractionDigits
				minimumFractionDigits = MyBase.minimumFractionDigits
			End If
			If serialVersionOnStream < 1 Then useExponentialNotation = False
			serialVersionOnStream = currentSerialVersion
		End Sub

		'----------------------------------------------------------------------
		' INSTANCE VARIABLES
		'----------------------------------------------------------------------

		<NonSerialized> _
		Private digitList As New DigitList

		''' <summary>
		''' The symbol used as a prefix when formatting positive numbers, e.g. "+".
		''' 
		''' @serial </summary>
		''' <seealso cref= #getPositivePrefix </seealso>
		Private positivePrefix As String = ""

		''' <summary>
		''' The symbol used as a suffix when formatting positive numbers.
		''' This is often an empty string.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getPositiveSuffix </seealso>
		Private positiveSuffix As String = ""

		''' <summary>
		''' The symbol used as a prefix when formatting negative numbers, e.g. "-".
		''' 
		''' @serial </summary>
		''' <seealso cref= #getNegativePrefix </seealso>
		Private negativePrefix As String = "-"

		''' <summary>
		''' The symbol used as a suffix when formatting negative numbers.
		''' This is often an empty string.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getNegativeSuffix </seealso>
		Private negativeSuffix As String = ""

		''' <summary>
		''' The prefix pattern for non-negative numbers.  This variable corresponds
		''' to <code>positivePrefix</code>.
		''' 
		''' <p>This pattern is expanded by the method <code>expandAffix()</code> to
		''' <code>positivePrefix</code> to update the latter to reflect changes in
		''' <code>symbols</code>.  If this variable is <code>null</code> then
		''' <code>positivePrefix</code> is taken as a literal value that does not
		''' change when <code>symbols</code> changes.  This variable is always
		''' <code>null</code> for <code>DecimalFormat</code> objects older than
		''' stream version 2 restored from stream.
		''' 
		''' @serial
		''' @since 1.3
		''' </summary>
		Private posPrefixPattern As String

		''' <summary>
		''' The suffix pattern for non-negative numbers.  This variable corresponds
		''' to <code>positiveSuffix</code>.  This variable is analogous to
		''' <code>posPrefixPattern</code>; see that variable for further
		''' documentation.
		''' 
		''' @serial
		''' @since 1.3
		''' </summary>
		Private posSuffixPattern As String

		''' <summary>
		''' The prefix pattern for negative numbers.  This variable corresponds
		''' to <code>negativePrefix</code>.  This variable is analogous to
		''' <code>posPrefixPattern</code>; see that variable for further
		''' documentation.
		''' 
		''' @serial
		''' @since 1.3
		''' </summary>
		Private negPrefixPattern As String

		''' <summary>
		''' The suffix pattern for negative numbers.  This variable corresponds
		''' to <code>negativeSuffix</code>.  This variable is analogous to
		''' <code>posPrefixPattern</code>; see that variable for further
		''' documentation.
		''' 
		''' @serial
		''' @since 1.3
		''' </summary>
		Private negSuffixPattern As String

		''' <summary>
		''' The multiplier for use in percent, per mille, etc.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMultiplier </seealso>
		Private multiplier As Integer = 1

		''' <summary>
		''' The number of digits between grouping separators in the integer
		''' portion of a number.  Must be greater than 0 if
		''' <code>NumberFormat.groupingUsed</code> is true.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getGroupingSize </seealso>
		''' <seealso cref= java.text.NumberFormat#isGroupingUsed </seealso>
		Private groupingSize As SByte = 3 ' invariant, > 0 if useThousands

		''' <summary>
		''' If true, forces the decimal separator to always appear in a formatted
		''' number, even if the fractional part of the number is zero.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isDecimalSeparatorAlwaysShown </seealso>
		Private decimalSeparatorAlwaysShown As Boolean = False

		''' <summary>
		''' If true, parse returns BigDecimal wherever possible.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isParseBigDecimal
		''' @since 1.5 </seealso>
		Private parseBigDecimal As Boolean = False


		''' <summary>
		''' True if this object represents a currency format.  This determines
		''' whether the monetary decimal separator is used instead of the normal one.
		''' </summary>
		<NonSerialized> _
		Private isCurrencyFormat As Boolean = False

		''' <summary>
		''' The <code>DecimalFormatSymbols</code> object used by this format.
		''' It contains the symbols used to format numbers, e.g. the grouping separator,
		''' decimal separator, and so on.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setDecimalFormatSymbols </seealso>
		''' <seealso cref= java.text.DecimalFormatSymbols </seealso>
		Private symbols As DecimalFormatSymbols = Nothing ' LIU new DecimalFormatSymbols();

		''' <summary>
		''' True to force the use of exponential (i.e. scientific) notation when formatting
		''' numbers.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private useExponentialNotation As Boolean ' Newly persistent in the Java 2 platform v.1.2

		''' <summary>
		''' FieldPositions describing the positive prefix String. This is
		''' lazily created. Use <code>getPositivePrefixFieldPositions</code>
		''' when needed.
		''' </summary>
		<NonSerialized> _
		Private positivePrefixFieldPositions As FieldPosition()

		''' <summary>
		''' FieldPositions describing the positive suffix String. This is
		''' lazily created. Use <code>getPositiveSuffixFieldPositions</code>
		''' when needed.
		''' </summary>
		<NonSerialized> _
		Private positiveSuffixFieldPositions As FieldPosition()

		''' <summary>
		''' FieldPositions describing the negative prefix String. This is
		''' lazily created. Use <code>getNegativePrefixFieldPositions</code>
		''' when needed.
		''' </summary>
		<NonSerialized> _
		Private negativePrefixFieldPositions As FieldPosition()

		''' <summary>
		''' FieldPositions describing the negative suffix String. This is
		''' lazily created. Use <code>getNegativeSuffixFieldPositions</code>
		''' when needed.
		''' </summary>
		<NonSerialized> _
		Private negativeSuffixFieldPositions As FieldPosition()

		''' <summary>
		''' The minimum number of digits used to display the exponent when a number is
		''' formatted in exponential notation.  This field is ignored if
		''' <code>useExponentialNotation</code> is not true.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private minExponentDigits As SByte ' Newly persistent in the Java 2 platform v.1.2

		''' <summary>
		''' The maximum number of digits allowed in the integer portion of a
		''' <code>BigInteger</code> or <code>BigDecimal</code> number.
		''' <code>maximumIntegerDigits</code> must be greater than or equal to
		''' <code>minimumIntegerDigits</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximumIntegerDigits
		''' @since 1.5 </seealso>
		Private maximumIntegerDigits As Integer = MyBase.maximumIntegerDigits

		''' <summary>
		''' The minimum number of digits allowed in the integer portion of a
		''' <code>BigInteger</code> or <code>BigDecimal</code> number.
		''' <code>minimumIntegerDigits</code> must be less than or equal to
		''' <code>maximumIntegerDigits</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimumIntegerDigits
		''' @since 1.5 </seealso>
		Private minimumIntegerDigits As Integer = MyBase.minimumIntegerDigits

		''' <summary>
		''' The maximum number of digits allowed in the fractional portion of a
		''' <code>BigInteger</code> or <code>BigDecimal</code> number.
		''' <code>maximumFractionDigits</code> must be greater than or equal to
		''' <code>minimumFractionDigits</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximumFractionDigits
		''' @since 1.5 </seealso>
		Private maximumFractionDigits As Integer = MyBase.maximumFractionDigits

		''' <summary>
		''' The minimum number of digits allowed in the fractional portion of a
		''' <code>BigInteger</code> or <code>BigDecimal</code> number.
		''' <code>minimumFractionDigits</code> must be less than or equal to
		''' <code>maximumFractionDigits</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimumFractionDigits
		''' @since 1.5 </seealso>
		Private minimumFractionDigits As Integer = MyBase.minimumFractionDigits

		''' <summary>
		''' The <seealso cref="java.math.RoundingMode"/> used in this DecimalFormat.
		''' 
		''' @serial
		''' @since 1.6
		''' </summary>
		Private roundingMode As java.math.RoundingMode = java.math.RoundingMode.HALF_EVEN

		' ------ DecimalFormat fields for fast-path for double algorithm  ------

		''' <summary>
		''' Helper inner utility class for storing the data used in the fast-path
		''' algorithm. Almost all fields related to fast-path are encapsulated in
		''' this class.
		''' 
		''' Any {@code DecimalFormat} instance has a {@code fastPathData}
		''' reference field that is null unless both the properties of the instance
		''' are such that the instance is in the "fast-path" state, and a format call
		''' has been done at least once while in this state.
		''' 
		''' Almost all fields are related to the "fast-path" state only and don't
		''' change until one of the instance properties is changed.
		''' 
		''' {@code firstUsedIndex} and {@code lastFreeIndex} are the only
		''' two fields that are used and modified while inside a call to
		''' {@code fastDoubleFormat}.
		''' 
		''' </summary>
		Private Class FastPathData
			' --- Temporary fields used in fast-path, shared by several methods.

			''' <summary>
			''' The first unused index at the end of the formatted result. </summary>
			Friend lastFreeIndex As Integer

			''' <summary>
			''' The first used index at the beginning of the formatted result </summary>
			Friend firstUsedIndex As Integer

			' --- State fields related to fast-path status. Changes due to a
			'     property change only. Set by checkAndSetFastPathStatus() only.

			''' <summary>
			''' Difference between locale zero and default zero representation. </summary>
			Friend zeroDelta As Integer

			''' <summary>
			''' Locale char for grouping separator. </summary>
			Friend groupingChar As Char

			''' <summary>
			'''  Fixed index position of last integral digit of formatted result </summary>
			Friend integralLastIndex As Integer

			''' <summary>
			'''  Fixed index position of first fractional digit of formatted result </summary>
			Friend fractionalFirstIndex As Integer

			''' <summary>
			''' Fractional constants depending on decimal|currency state </summary>
			Friend fractionalScaleFactor As Double
			Friend fractionalMaxIntBound As Integer


			''' <summary>
			''' The char array buffer that will contain the formatted result </summary>
			Friend fastPathContainer As Char()

			''' <summary>
			''' Suffixes recorded as char array for efficiency. </summary>
			Friend charsPositivePrefix As Char()
			Friend charsNegativePrefix As Char()
			Friend charsPositiveSuffix As Char()
			Friend charsNegativeSuffix As Char()
			Friend positiveAffixesRequired As Boolean = True
			Friend negativeAffixesRequired As Boolean = True
		End Class

		''' <summary>
		''' The format fast-path status of the instance. Logical state. </summary>
		<NonSerialized> _
		Private isFastPath As Boolean = False

		''' <summary>
		''' Flag stating need of check and reinit fast-path status on next format call. </summary>
		<NonSerialized> _
		Private fastPathCheckNeeded As Boolean = True

		''' <summary>
		''' DecimalFormat reference to its FastPathData </summary>
		<NonSerialized> _
		Private fastPathData As FastPathData


		'----------------------------------------------------------------------

		Friend Shadows Const currentSerialVersion As Integer = 4

		''' <summary>
		''' The internal serial version which says which version was written.
		''' Possible values are:
		''' <ul>
		''' <li><b>0</b> (default): versions before the Java 2 platform v1.2
		''' <li><b>1</b>: version for 1.2, which includes the two new fields
		'''      <code>useExponentialNotation</code> and
		'''      <code>minExponentDigits</code>.
		''' <li><b>2</b>: version for 1.3 and later, which adds four new fields:
		'''      <code>posPrefixPattern</code>, <code>posSuffixPattern</code>,
		'''      <code>negPrefixPattern</code>, and <code>negSuffixPattern</code>.
		''' <li><b>3</b>: version for 1.5 and later, which adds five new fields:
		'''      <code>maximumIntegerDigits</code>,
		'''      <code>minimumIntegerDigits</code>,
		'''      <code>maximumFractionDigits</code>,
		'''      <code>minimumFractionDigits</code>, and
		'''      <code>parseBigDecimal</code>.
		''' <li><b>4</b>: version for 1.6 and later, which adds one new field:
		'''      <code>roundingMode</code>.
		''' </ul>
		''' @since 1.2
		''' @serial
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion

		'----------------------------------------------------------------------
		' CONSTANTS
		'----------------------------------------------------------------------

		' ------ Fast-Path for double Constants ------

		''' <summary>
		''' Maximum valid integer value for applying fast-path algorithm </summary>
		Private Shared ReadOnly MAX_INT_AS_DOUBLE As Double = CDbl( java.lang.[Integer].MAX_VALUE)

		''' <summary>
		''' The digit arrays used in the fast-path methods for collecting digits.
		''' Using 3 constants arrays of chars ensures a very fast collection of digits
		''' </summary>
		Private Class DigitArrays
			Friend Shared ReadOnly DigitOnes1000 As Char() = New Char(999){}
			Friend Shared ReadOnly DigitTens1000 As Char() = New Char(999){}
			Friend Shared ReadOnly DigitHundreds1000 As Char() = New Char(999){}

			' initialize on demand holder class idiom for arrays of digits
			Shared Sub New()
				Dim tenIndex As Integer = 0
				Dim hundredIndex As Integer = 0
				Dim digitOne As Char = "0"c
				Dim digitTen As Char = "0"c
				Dim digitHundred As Char = "0"c
				For i As Integer = 0 To 999

					DigitOnes1000(i) = digitOne
					If digitOne = "9"c Then
						digitOne = "0"c
					Else
						digitOne = ChrW(AscW(digitOne) + 1)
					End If

					DigitTens1000(i) = digitTen
					If i = (tenIndex + 9) Then
						tenIndex += 10
						If digitTen = "9"c Then
							digitTen = "0"c
						Else
							digitTen = ChrW(AscW(digitTen) + 1)
						End If
					End If

					DigitHundreds1000(i) = digitHundred
					If i = (hundredIndex + 99) Then
						digitHundred = ChrW(AscW(digitHundred) + 1)
						hundredIndex += 100
					End If
				Next i
			End Sub
		End Class
		' ------ Fast-Path for double Constants end ------

		' Constants for characters used in programmatic (unlocalized) patterns.
		Private Const PATTERN_ZERO_DIGIT As Char = "0"c
		Private Const PATTERN_GROUPING_SEPARATOR As Char = ","c
		Private Const PATTERN_DECIMAL_SEPARATOR As Char = "."c
		Private Shared ReadOnly PATTERN_PER_MILLE As Char = ChrW(&H2030)
		Private Const PATTERN_PERCENT As Char = "%"c
		Private Const PATTERN_DIGIT As Char = "#"c
		Private Const PATTERN_SEPARATOR As Char = ";"c
		Private Const PATTERN_EXPONENT As String = "E"
		Private Const PATTERN_MINUS As Char = "-"c

		''' <summary>
		''' The CURRENCY_SIGN is the standard Unicode symbol for currency.  It
		''' is used in patterns and substituted with either the currency symbol,
		''' or if it is doubled, with the international currency symbol.  If the
		''' CURRENCY_SIGN is seen in a pattern, then the decimal separator is
		''' replaced with the monetary decimal separator.
		''' 
		''' The CURRENCY_SIGN is not localized.
		''' </summary>
		Private Shared ReadOnly CURRENCY_SIGN As Char = ChrW(&H00A4)

		Private Const QUOTE As Char = "'"c

		Private Shared EmptyFieldPositionArray As FieldPosition() = New FieldPosition(){}

		' Upper limit on integer and fraction digits for a Java double
		Friend Const DOUBLE_INTEGER_DIGITS As Integer = 309
		Friend Const DOUBLE_FRACTION_DIGITS As Integer = 340

		' Upper limit on integer and fraction digits for BigDecimal and BigInteger
		Friend Shared ReadOnly MAXIMUM_INTEGER_DIGITS As Integer =  java.lang.[Integer].MAX_VALUE
		Friend Shared ReadOnly MAXIMUM_FRACTION_DIGITS As Integer =  java.lang.[Integer].MAX_VALUE

		' Proclaim JDK 1.1 serial compatibility.
		Friend Shadows Const serialVersionUID As Long = 864413376551465018L
	End Class

End Namespace