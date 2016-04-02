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
	''' <code>NumberFormat</code> is the abstract base class for all number
	''' formats. This class provides the interface for formatting and parsing
	''' numbers. <code>NumberFormat</code> also provides methods for determining
	''' which locales have number formats, and what their names are.
	''' 
	''' <p>
	''' <code>NumberFormat</code> helps you to format and parse numbers for any locale.
	''' Your code can be completely independent of the locale conventions for
	''' decimal points, thousands-separators, or even the particular decimal
	''' digits used, or whether the number format is even decimal.
	''' 
	''' <p>
	''' To format a number for the current Locale, use one of the factory
	''' class methods:
	''' <blockquote>
	''' <pre>{@code
	''' myString = NumberFormat.getInstance().format(myNumber);
	''' }</pre>
	''' </blockquote>
	''' If you are formatting multiple numbers, it is
	''' more efficient to get the format and use it multiple times so that
	''' the system doesn't have to fetch the information about the local
	''' language and country conventions multiple times.
	''' <blockquote>
	''' <pre>{@code
	''' NumberFormat nf = NumberFormat.getInstance();
	''' for (int i = 0; i < myNumber.length; ++i) {
	'''     output.println(nf.format(myNumber[i]) + "; ");
	''' }
	''' }</pre>
	''' </blockquote>
	''' To format a number for a different Locale, specify it in the
	''' call to <code>getInstance</code>.
	''' <blockquote>
	''' <pre>{@code
	''' NumberFormat nf = NumberFormat.getInstance(Locale.FRENCH);
	''' }</pre>
	''' </blockquote>
	''' You can also use a <code>NumberFormat</code> to parse numbers:
	''' <blockquote>
	''' <pre>{@code
	''' myNumber = nf.parse(myString);
	''' }</pre>
	''' </blockquote>
	''' Use <code>getInstance</code> or <code>getNumberInstance</code> to get the
	''' normal number format. Use <code>getIntegerInstance</code> to get an
	''' integer number format. Use <code>getCurrencyInstance</code> to get the
	''' currency number format. And use <code>getPercentInstance</code> to get a
	''' format for displaying percentages. With this format, a fraction like
	''' 0.53 is displayed as 53%.
	''' 
	''' <p>
	''' You can also control the display of numbers with such methods as
	''' <code>setMinimumFractionDigits</code>.
	''' If you want even more control over the format or parsing,
	''' or want to give your users more control,
	''' you can try casting the <code>NumberFormat</code> you get from the factory methods
	''' to a <code>DecimalFormat</code>. This will work for the vast majority
	''' of locales; just remember to put it in a <code>try</code> block in case you
	''' encounter an unusual one.
	''' 
	''' <p>
	''' NumberFormat and DecimalFormat are designed such that some controls
	''' work for formatting and others work for parsing.  The following is
	''' the detailed description for each these control methods,
	''' <p>
	''' setParseIntegerOnly : only affects parsing, e.g.
	''' if true,  "3456.78" &rarr; 3456 (and leaves the parse position just after index 6)
	''' if false, "3456.78" &rarr; 3456.78 (and leaves the parse position just after index 8)
	''' This is independent of formatting.  If you want to not show a decimal point
	''' where there might be no digits after the decimal point, use
	''' setDecimalSeparatorAlwaysShown.
	''' <p>
	''' setDecimalSeparatorAlwaysShown : only affects formatting, and only where
	''' there might be no digits after the decimal point, such as with a pattern
	''' like "#,##0.##", e.g.,
	''' if true,  3456.00 &rarr; "3,456."
	''' if false, 3456.00 &rarr; "3456"
	''' This is independent of parsing.  If you want parsing to stop at the decimal
	''' point, use setParseIntegerOnly.
	''' 
	''' <p>
	''' You can also use forms of the <code>parse</code> and <code>format</code>
	''' methods with <code>ParsePosition</code> and <code>FieldPosition</code> to
	''' allow you to:
	''' <ul>
	''' <li> progressively parse through pieces of a string
	''' <li> align the decimal point and other areas
	''' </ul>
	''' For example, you can align numbers in two ways:
	''' <ol>
	''' <li> If you are using a monospaced font with spacing for alignment,
	'''      you can pass the <code>FieldPosition</code> in your format call, with
	'''      <code>field</code> = <code>INTEGER_FIELD</code>. On output,
	'''      <code>getEndIndex</code> will be set to the offset between the
	'''      last character of the integer and the decimal. Add
	'''      (desiredSpaceCount - getEndIndex) spaces at the front of the string.
	''' 
	''' <li> If you are using proportional fonts,
	'''      instead of padding with spaces, measure the width
	'''      of the string in pixels from the start to <code>getEndIndex</code>.
	'''      Then move the pen by
	'''      (desiredPixelWidth - widthToAlignmentPoint) before drawing the text.
	'''      It also works where there is no decimal, but possibly additional
	'''      characters at the end, e.g., with parentheses in negative
	'''      numbers: "(12)" for -12.
	''' </ol>
	''' 
	''' <h3><a name="synchronization">Synchronization</a></h3>
	''' 
	''' <p>
	''' Number formats are generally not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' </summary>
	''' <seealso cref=          DecimalFormat </seealso>
	''' <seealso cref=          ChoiceFormat
	''' @author       Mark Davis
	''' @author       Helena Shih </seealso>
	Public MustInherit Class NumberFormat
		Inherits Format

		''' <summary>
		''' Field constant used to construct a FieldPosition object. Signifies that
		''' the position of the integer part of a formatted number should be returned. </summary>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Public Const INTEGER_FIELD As Integer = 0

		''' <summary>
		''' Field constant used to construct a FieldPosition object. Signifies that
		''' the position of the fraction part of a formatted number should be returned. </summary>
		''' <seealso cref= java.text.FieldPosition </seealso>
		Public Const FRACTION_FIELD As Integer = 1

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Formats a number and appends the resulting text to the given string
		''' buffer.
		''' The number can be of any subclass of <seealso cref="java.lang.Number"/>.
		''' <p>
		''' This implementation extracts the number's value using
		''' <seealso cref="java.lang.Number#longValue()"/> for all integral type values that
		''' can be converted to <code>long</code> without loss of information,
		''' including <code>BigInteger</code> values with a
		''' <seealso cref="java.math.BigInteger#bitLength() bit length"/> of less than 64,
		''' and <seealso cref="java.lang.Number#doubleValue()"/> for all other types. It
		''' then calls
		''' <seealso cref="#format(long,java.lang.StringBuffer,java.text.FieldPosition)"/>
		''' or <seealso cref="#format(double,java.lang.StringBuffer,java.text.FieldPosition)"/>.
		''' This may result in loss of magnitude information and precision for
		''' <code>BigInteger</code> and <code>BigDecimal</code> values. </summary>
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
		Public Overrides Function format(ByVal number As Object, ByVal toAppendTo As StringBuffer, ByVal pos As FieldPosition) As StringBuffer
			If TypeOf number Is Long? OrElse TypeOf number Is Integer? OrElse TypeOf number Is Short? OrElse TypeOf number Is Byte OrElse TypeOf number Is java.util.concurrent.atomic.AtomicInteger OrElse TypeOf number Is java.util.concurrent.atomic.AtomicLong OrElse (TypeOf number Is System.Numerics.BigInteger AndAlso CType(number, System.Numerics.BigInteger).bitLength() < 64) Then
				Return format(CType(number, Number), toAppendTo, pos)
			ElseIf TypeOf number Is Number Then
				Return format(CType(number, Number), toAppendTo, pos)
			Else
				Throw New IllegalArgumentException("Cannot format given Object as a Number")
			End If
		End Function

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
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on number parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		''' <param name="pos"> A <code>ParsePosition</code> object with index and error
		'''            index information as described above. </param>
		''' <returns> A <code>Number</code> parsed from the string. In case of
		'''         error, returns null. </returns>
		''' <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		Public NotOverridable Overrides Function parseObject(ByVal source As String, ByVal pos As ParsePosition) As Object
			Return parse(source, pos)
		End Function

	   ''' <summary>
	   ''' Specialization of format.
	   ''' </summary>
	   ''' <param name="number"> the double number to format </param>
	   ''' <returns> the formatted String </returns>
	   ''' <exception cref="ArithmeticException"> if rounding is needed with rounding
	   '''                   mode being set to RoundingMode.UNNECESSARY </exception>
	   ''' <seealso cref= java.text.Format#format </seealso>
		Public Function format(ByVal number As Double) As String
			' Use fast-path for double result if that works
			Dim result As String = fastFormat(number)
			If result IsNot Nothing Then Return result

			Return format(number, New StringBuffer, DontCareFieldPosition.INSTANCE).ToString()
		End Function

	'    
	'     * fastFormat() is supposed to be implemented in concrete subclasses only.
	'     * Default implem always returns null.
	'     
		Friend Overridable Function fastFormat(ByVal number As Double) As String
			Return Nothing
		End Function

	   ''' <summary>
	   ''' Specialization of format.
	   ''' </summary>
	   ''' <param name="number"> the long number to format </param>
	   ''' <returns> the formatted String </returns>
	   ''' <exception cref="ArithmeticException"> if rounding is needed with rounding
	   '''                   mode being set to RoundingMode.UNNECESSARY </exception>
	   ''' <seealso cref= java.text.Format#format </seealso>
		Public Function format(ByVal number As Long) As String
			Return format(number, New StringBuffer, DontCareFieldPosition.INSTANCE).ToString()
		End Function

	   ''' <summary>
	   ''' Specialization of format.
	   ''' </summary>
	   ''' <param name="number">     the double number to format </param>
	   ''' <param name="toAppendTo"> the StringBuffer to which the formatted text is to be
	   '''                   appended </param>
	   ''' <param name="pos">        the field position </param>
	   ''' <returns> the formatted StringBuffer </returns>
	   ''' <exception cref="ArithmeticException"> if rounding is needed with rounding
	   '''                   mode being set to RoundingMode.UNNECESSARY </exception>
	   ''' <seealso cref= java.text.Format#format </seealso>
		Public MustOverride Function format(ByVal number As Double, ByVal toAppendTo As StringBuffer, ByVal pos As FieldPosition) As StringBuffer

	   ''' <summary>
	   ''' Specialization of format.
	   ''' </summary>
	   ''' <param name="number">     the long number to format </param>
	   ''' <param name="toAppendTo"> the StringBuffer to which the formatted text is to be
	   '''                   appended </param>
	   ''' <param name="pos">        the field position </param>
	   ''' <returns> the formatted StringBuffer </returns>
	   ''' <exception cref="ArithmeticException"> if rounding is needed with rounding
	   '''                   mode being set to RoundingMode.UNNECESSARY </exception>
	   ''' <seealso cref= java.text.Format#format </seealso>
		Public MustOverride Function format(ByVal number As Long, ByVal toAppendTo As StringBuffer, ByVal pos As FieldPosition) As StringBuffer

	   ''' <summary>
	   ''' Returns a Long if possible (e.g., within the range [Long.MIN_VALUE,
	   ''' java.lang.[Long].MAX_VALUE] and with no decimals), otherwise a java.lang.[Double].
	   ''' If IntegerOnly is set, will stop at a decimal
	   ''' point (or equivalent; e.g., for rational numbers "1 2/3", will stop
	   ''' after the 1).
	   ''' Does not throw an exception; if no object can be parsed, index is
	   ''' unchanged!
	   ''' </summary>
	   ''' <param name="source"> the String to parse </param>
	   ''' <param name="parsePosition"> the parse position </param>
	   ''' <returns> the parsed value </returns>
	   ''' <seealso cref= java.text.NumberFormat#isParseIntegerOnly </seealso>
	   ''' <seealso cref= java.text.Format#parseObject </seealso>
		Public MustOverride Function parse(ByVal source As String, ByVal parsePosition As ParsePosition) As Number

		''' <summary>
		''' Parses text from the beginning of the given string to produce a number.
		''' The method may not use the entire text of the given string.
		''' <p>
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on number parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		''' <returns> A <code>Number</code> parsed from the string. </returns>
		''' <exception cref="ParseException"> if the beginning of the specified string
		'''            cannot be parsed. </exception>
		Public Overridable Function parse(ByVal source As String) As Number
			Dim parsePosition As New ParsePosition(0)
			Dim result As Number = parse(source, parsePosition)
			If parsePosition.index = 0 Then Throw New ParseException("Unparseable number: """ & source & """", parsePosition.errorIndex)
			Return result
		End Function

		''' <summary>
		''' Returns true if this format will parse numbers as integers only.
		''' For example in the English locale, with ParseIntegerOnly true, the
		''' string "1234." would be parsed as the integer value 1234 and parsing
		''' would stop at the "." character.  Of course, the exact format accepted
		''' by the parse operation is locale dependant and determined by sub-classes
		''' of NumberFormat.
		''' </summary>
		''' <returns> {@code true} if numbers should be parsed as integers only;
		'''         {@code false} otherwise </returns>
		Public Overridable Property parseIntegerOnly As Boolean
			Get
				Return parseIntegerOnly
			End Get
			Set(ByVal value As Boolean)
				parseIntegerOnly = value
			End Set
		End Property


		'============== Locale Stuff =====================

		''' <summary>
		''' Returns a general-purpose number format for the current default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' This is the same as calling
		''' <seealso cref="#getNumberInstance() getNumberInstance()"/>.
		''' </summary>
		''' <returns> the {@code NumberFormat} instance for general-purpose number
		''' formatting </returns>
		PublicShared ReadOnly Propertyinstance As NumberFormat
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), NUMBERSTYLE)
			End Get
		End Property

		''' <summary>
		''' Returns a general-purpose number format for the specified locale.
		''' This is the same as calling
		''' <seealso cref="#getNumberInstance(java.util.Locale) getNumberInstance(inLocale)"/>.
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		''' <returns> the {@code NumberFormat} instance for general-purpose number
		''' formatting </returns>
		Public Shared Function getInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, NUMBERSTYLE)
		End Function

		''' <summary>
		''' Returns a general-purpose number format for the current default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getNumberInstance(Locale)
		'''     getNumberInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		''' </summary>
		''' <returns> the {@code NumberFormat} instance for general-purpose number
		''' formatting </returns>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		PublicShared ReadOnly PropertynumberInstance As NumberFormat
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), NUMBERSTYLE)
			End Get
		End Property

		''' <summary>
		''' Returns a general-purpose number format for the specified locale.
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		''' <returns> the {@code NumberFormat} instance for general-purpose number
		''' formatting </returns>
		Public Shared Function getNumberInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, NUMBERSTYLE)
		End Function

		''' <summary>
		''' Returns an integer number format for the current default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale. The
		''' returned number format is configured to round floating point numbers
		''' to the nearest integer using half-even rounding (see {@link
		''' java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN}) for formatting,
		''' and to parse only the integer part of an input string (see {@link
		''' #isParseIntegerOnly isParseIntegerOnly}).
		''' <p>This is equivalent to calling
		''' {@link #getIntegerInstance(Locale)
		'''     getIntegerInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		''' </summary>
		''' <seealso cref= #getRoundingMode() </seealso>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a number format for integer values
		''' @since 1.4 </returns>
		PublicShared ReadOnly PropertyintegerInstance As NumberFormat
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), INTEGERSTYLE)
			End Get
		End Property

		''' <summary>
		''' Returns an integer number format for the specified locale. The
		''' returned number format is configured to round floating point numbers
		''' to the nearest integer using half-even rounding (see {@link
		''' java.math.RoundingMode#HALF_EVEN RoundingMode.HALF_EVEN}) for formatting,
		''' and to parse only the integer part of an input string (see {@link
		''' #isParseIntegerOnly isParseIntegerOnly}).
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		''' <seealso cref= #getRoundingMode() </seealso>
		''' <returns> a number format for integer values
		''' @since 1.4 </returns>
		Public Shared Function getIntegerInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, INTEGERSTYLE)
		End Function

		''' <summary>
		''' Returns a currency format for the current default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getCurrencyInstance(Locale)
		'''     getCurrencyInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		''' </summary>
		''' <returns> the {@code NumberFormat} instance for currency formatting </returns>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		PublicShared ReadOnly PropertycurrencyInstance As NumberFormat
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), CURRENCYSTYLE)
			End Get
		End Property

		''' <summary>
		''' Returns a currency format for the specified locale.
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		''' <returns> the {@code NumberFormat} instance for currency formatting </returns>
		Public Shared Function getCurrencyInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, CURRENCYSTYLE)
		End Function

		''' <summary>
		''' Returns a percentage format for the current default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>This is equivalent to calling
		''' {@link #getPercentInstance(Locale)
		'''     getPercentInstance(Locale.getDefault(Locale.Category.FORMAT))}.
		''' </summary>
		''' <returns> the {@code NumberFormat} instance for percentage formatting </returns>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		PublicShared ReadOnly PropertypercentInstance As NumberFormat
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), PERCENTSTYLE)
			End Get
		End Property

		''' <summary>
		''' Returns a percentage format for the specified locale.
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		''' <returns> the {@code NumberFormat} instance for percentage formatting </returns>
		Public Shared Function getPercentInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, PERCENTSTYLE)
		End Function

		''' <summary>
		''' Returns a scientific format for the current default locale.
		''' </summary>
		'public
	 Shared scientificInstance As NumberFormat
		 Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT), SCIENTIFICSTYLE)
		 End Get
	 End Property

		''' <summary>
		''' Returns a scientific format for the specified locale.
		''' </summary>
		''' <param name="inLocale"> the desired locale </param>
		'public
	 Shared Function getScientificInstance(ByVal inLocale As java.util.Locale) As NumberFormat
			Return getInstance(inLocale, SCIENTIFICSTYLE)
	 End Function

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>get*Instance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported by the Java
		''' runtime and by installed
		''' <seealso cref="java.text.spi.NumberFormatProvider NumberFormatProvider"/> implementations.
		''' It must contain at least a <code>Locale</code> instance equal to
		''' <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> An array of locales for which localized
		'''         <code>NumberFormat</code> instances are available. </returns>
		PublicShared ReadOnly PropertyavailableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.NumberFormatProvider))
				Return pool.availableLocales
			End Get
		End Property

		''' <summary>
		''' Overrides hashCode.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return maximumIntegerDigits * 37 + maxFractionDigits
			' just enough fields for a reasonable distribution
		End Function

		''' <summary>
		''' Overrides equals.
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Me Is obj Then Return True
			If Me.GetType() IsNot obj.GetType() Then Return False
			Dim other As NumberFormat = CType(obj, NumberFormat)
			Return (maximumIntegerDigits = other.maximumIntegerDigits AndAlso minimumIntegerDigits = other.minimumIntegerDigits AndAlso maximumFractionDigits = other.maximumFractionDigits AndAlso minimumFractionDigits = other.minimumFractionDigits AndAlso groupingUsed = other.groupingUsed AndAlso parseIntegerOnly = other.parseIntegerOnly)
		End Function

		''' <summary>
		''' Overrides Cloneable.
		''' </summary>
		Public Overrides Function clone() As Object
			Dim other As NumberFormat = CType(MyBase.clone(), NumberFormat)
			Return other
		End Function

		''' <summary>
		''' Returns true if grouping is used in this format. For example, in the
		''' English locale, with grouping on, the number 1234567 might be formatted
		''' as "1,234,567". The grouping separator as well as the size of each group
		''' is locale dependant and is determined by sub-classes of NumberFormat.
		''' </summary>
		''' <returns> {@code true} if grouping is used;
		'''         {@code false} otherwise </returns>
		''' <seealso cref= #setGroupingUsed </seealso>
		Public Overridable Property groupingUsed As Boolean
			Get
				Return groupingUsed
			End Get
			Set(ByVal newValue As Boolean)
				groupingUsed = newValue
			End Set
		End Property


		''' <summary>
		''' Returns the maximum number of digits allowed in the integer portion of a
		''' number.
		''' </summary>
		''' <returns> the maximum number of digits </returns>
		''' <seealso cref= #setMaximumIntegerDigits </seealso>
		Public Overridable Property maximumIntegerDigits As Integer
			Get
				Return maximumIntegerDigits
			End Get
			Set(ByVal newValue As Integer)
				maximumIntegerDigits = System.Math.Max(0,newValue)
				If minimumIntegerDigits > maximumIntegerDigits Then minimumIntegerDigits = maximumIntegerDigits
			End Set
		End Property


		''' <summary>
		''' Returns the minimum number of digits allowed in the integer portion of a
		''' number.
		''' </summary>
		''' <returns> the minimum number of digits </returns>
		''' <seealso cref= #setMinimumIntegerDigits </seealso>
		Public Overridable Property minimumIntegerDigits As Integer
			Get
				Return minimumIntegerDigits
			End Get
			Set(ByVal newValue As Integer)
				minimumIntegerDigits = System.Math.Max(0,newValue)
				If minimumIntegerDigits > maximumIntegerDigits Then maximumIntegerDigits = minimumIntegerDigits
			End Set
		End Property


		''' <summary>
		''' Returns the maximum number of digits allowed in the fraction portion of a
		''' number.
		''' </summary>
		''' <returns> the maximum number of digits. </returns>
		''' <seealso cref= #setMaximumFractionDigits </seealso>
		Public Overridable Property maximumFractionDigits As Integer
			Get
				Return maximumFractionDigits
			End Get
			Set(ByVal newValue As Integer)
				maximumFractionDigits = System.Math.Max(0,newValue)
				If maximumFractionDigits < minimumFractionDigits Then minimumFractionDigits = maximumFractionDigits
			End Set
		End Property


		''' <summary>
		''' Returns the minimum number of digits allowed in the fraction portion of a
		''' number.
		''' </summary>
		''' <returns> the minimum number of digits </returns>
		''' <seealso cref= #setMinimumFractionDigits </seealso>
		Public Overridable Property minimumFractionDigits As Integer
			Get
				Return minimumFractionDigits
			End Get
			Set(ByVal newValue As Integer)
				minimumFractionDigits = System.Math.Max(0,newValue)
				If maximumFractionDigits < minimumFractionDigits Then maximumFractionDigits = minimumFractionDigits
			End Set
		End Property


		''' <summary>
		''' Gets the currency used by this number format when formatting
		''' currency values. The initial value is derived in a locale dependent
		''' way. The returned value may be null if no valid
		''' currency could be determined and no currency has been set using
		''' <seealso cref="#setCurrency(java.util.Currency) setCurrency"/>.
		''' <p>
		''' The default implementation throws
		''' <code>UnsupportedOperationException</code>.
		''' </summary>
		''' <returns> the currency used by this number format, or <code>null</code> </returns>
		''' <exception cref="UnsupportedOperationException"> if the number format class
		''' doesn't implement currency formatting
		''' @since 1.4 </exception>
		Public Overridable Property currency As java.util.Currency
			Get
				Throw New UnsupportedOperationException
			End Get
			Set(ByVal currency As java.util.Currency)
				Throw New UnsupportedOperationException
			End Set
		End Property


		''' <summary>
		''' Gets the <seealso cref="java.math.RoundingMode"/> used in this NumberFormat.
		''' The default implementation of this method in NumberFormat
		''' always throws <seealso cref="java.lang.UnsupportedOperationException"/>.
		''' Subclasses which handle different rounding modes should override
		''' this method.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> The default implementation
		'''     always throws this exception </exception>
		''' <returns> The <code>RoundingMode</code> used for this NumberFormat. </returns>
		''' <seealso cref= #setRoundingMode(RoundingMode)
		''' @since 1.6 </seealso>
		Public Overridable Property roundingMode As java.math.RoundingMode
			Get
				Throw New UnsupportedOperationException
			End Get
			Set(ByVal roundingMode As java.math.RoundingMode)
				Throw New UnsupportedOperationException
			End Set
		End Property


		' =======================privates===============================

		Private Shared Function getInstance(ByVal desiredLocale As java.util.Locale, ByVal choice As Integer) As NumberFormat
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter
			adapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.NumberFormatProvider), desiredLocale)
			Dim numberFormat_Renamed As NumberFormat = getInstance(adapter, desiredLocale, choice)
			If numberFormat_Renamed Is Nothing Then numberFormat_Renamed = getInstance(sun.util.locale.provider.LocaleProviderAdapter.forJRE(), desiredLocale, choice)
			Return numberFormat_Renamed
		End Function

		Private Shared Function getInstance(ByVal adapter As sun.util.locale.provider.LocaleProviderAdapter, ByVal locale As java.util.Locale, ByVal choice As Integer) As NumberFormat
			Dim provider As java.text.spi.NumberFormatProvider = adapter.numberFormatProvider
			Dim numberFormat_Renamed As NumberFormat = Nothing
			Select Case choice
			Case NUMBERSTYLE
				numberFormat_Renamed = provider.getNumberInstance(locale)
			Case PERCENTSTYLE
				numberFormat_Renamed = provider.getPercentInstance(locale)
			Case CURRENCYSTYLE
				numberFormat_Renamed = provider.getCurrencyInstance(locale)
			Case INTEGERSTYLE
				numberFormat_Renamed = provider.getIntegerInstance(locale)
			End Select
			Return numberFormat_Renamed
		End Function

		''' <summary>
		''' First, read in the default serializable data.
		''' 
		''' Then, if <code>serialVersionOnStream</code> is less than 1, indicating that
		''' the stream was written by JDK 1.1,
		''' set the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		''' to be equal to the <code>byte</code> fields such as <code>maxIntegerDigits</code>,
		''' since the <code>int</code> fields were not present in JDK 1.1.
		''' Finally, set serialVersionOnStream back to the maximum allowed value so that
		''' default serialization will work properly if this object is streamed out again.
		''' 
		''' <p>If <code>minimumIntegerDigits</code> is greater than
		''' <code>maximumIntegerDigits</code> or <code>minimumFractionDigits</code>
		''' is greater than <code>maximumFractionDigits</code>, then the stream data
		''' is invalid and this method throws an <code>InvalidObjectException</code>.
		''' In addition, if any of these values is negative, then this method throws
		''' an <code>InvalidObjectException</code>.
		''' 
		''' @since 1.2
		''' </summary>
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			If serialVersionOnStream < 1 Then
				' Didn't have additional int fields, reassign to use them.
				maximumIntegerDigits = maxIntegerDigits
				minimumIntegerDigits = minIntegerDigits
				maximumFractionDigits = maxFractionDigits
				minimumFractionDigits = minFractionDigits
			End If
			If minimumIntegerDigits > maximumIntegerDigits OrElse minimumFractionDigits > maximumFractionDigits OrElse minimumIntegerDigits < 0 OrElse minimumFractionDigits < 0 Then Throw New java.io.InvalidObjectException("Digit count range invalid")
			serialVersionOnStream = currentSerialVersion
		End Sub

		''' <summary>
		''' Write out the default serializable data, after first setting
		''' the <code>byte</code> fields such as <code>maxIntegerDigits</code> to be
		''' equal to the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		''' (or to <code>Byte.MAX_VALUE</code>, whichever is smaller), for compatibility
		''' with the JDK 1.1 version of the stream format.
		''' 
		''' @since 1.2
		''' </summary>
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			maxIntegerDigits = If(maximumIntegerDigits > java.lang.[Byte].Max_Value, java.lang.[Byte].Max_Value, CByte(maximumIntegerDigits))
			minIntegerDigits = If(minimumIntegerDigits > java.lang.[Byte].Max_Value, java.lang.[Byte].Max_Value, CByte(minimumIntegerDigits))
			maxFractionDigits = If(maximumFractionDigits > java.lang.[Byte].Max_Value, java.lang.[Byte].Max_Value, CByte(maximumFractionDigits))
			minFractionDigits = If(minimumFractionDigits > java.lang.[Byte].Max_Value, java.lang.[Byte].Max_Value, CByte(minimumFractionDigits))
			stream.defaultWriteObject()
		End Sub

		' Constants used by factory methods to specify a style of format.
		Private Const NUMBERSTYLE As Integer = 0
		Private Const CURRENCYSTYLE As Integer = 1
		Private Const PERCENTSTYLE As Integer = 2
		Private Const SCIENTIFICSTYLE As Integer = 3
		Private Const INTEGERSTYLE As Integer = 4

		''' <summary>
		''' True if the grouping (i.e. thousands) separator is used when
		''' formatting and parsing numbers.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isGroupingUsed </seealso>
		Private groupingUsed As Boolean = True

		''' <summary>
		''' The maximum number of digits allowed in the integer portion of a
		''' number.  <code>maxIntegerDigits</code> must be greater than or equal to
		''' <code>minIntegerDigits</code>.
		''' <p>
		''' <strong>Note:</strong> This field exists only for serialization
		''' compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		''' <code>int</code> field <code>maximumIntegerDigits</code> is used instead.
		''' When writing to a stream, <code>maxIntegerDigits</code> is set to
		''' <code>maximumIntegerDigits</code> or <code>Byte.MAX_VALUE</code>,
		''' whichever is smaller.  When reading from a stream, this field is used
		''' only if <code>serialVersionOnStream</code> is less than 1.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximumIntegerDigits </seealso>
		Private maxIntegerDigits As SByte = 40

		''' <summary>
		''' The minimum number of digits allowed in the integer portion of a
		''' number.  <code>minimumIntegerDigits</code> must be less than or equal to
		''' <code>maximumIntegerDigits</code>.
		''' <p>
		''' <strong>Note:</strong> This field exists only for serialization
		''' compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		''' <code>int</code> field <code>minimumIntegerDigits</code> is used instead.
		''' When writing to a stream, <code>minIntegerDigits</code> is set to
		''' <code>minimumIntegerDigits</code> or <code>Byte.MAX_VALUE</code>,
		''' whichever is smaller.  When reading from a stream, this field is used
		''' only if <code>serialVersionOnStream</code> is less than 1.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimumIntegerDigits </seealso>
		Private minIntegerDigits As SByte = 1

		''' <summary>
		''' The maximum number of digits allowed in the fractional portion of a
		''' number.  <code>maximumFractionDigits</code> must be greater than or equal to
		''' <code>minimumFractionDigits</code>.
		''' <p>
		''' <strong>Note:</strong> This field exists only for serialization
		''' compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		''' <code>int</code> field <code>maximumFractionDigits</code> is used instead.
		''' When writing to a stream, <code>maxFractionDigits</code> is set to
		''' <code>maximumFractionDigits</code> or <code>Byte.MAX_VALUE</code>,
		''' whichever is smaller.  When reading from a stream, this field is used
		''' only if <code>serialVersionOnStream</code> is less than 1.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMaximumFractionDigits </seealso>
		Private maxFractionDigits As SByte = 3 ' invariant, >= minFractionDigits

		''' <summary>
		''' The minimum number of digits allowed in the fractional portion of a
		''' number.  <code>minimumFractionDigits</code> must be less than or equal to
		''' <code>maximumFractionDigits</code>.
		''' <p>
		''' <strong>Note:</strong> This field exists only for serialization
		''' compatibility with JDK 1.1.  In Java platform 2 v1.2 and higher, the new
		''' <code>int</code> field <code>minimumFractionDigits</code> is used instead.
		''' When writing to a stream, <code>minFractionDigits</code> is set to
		''' <code>minimumFractionDigits</code> or <code>Byte.MAX_VALUE</code>,
		''' whichever is smaller.  When reading from a stream, this field is used
		''' only if <code>serialVersionOnStream</code> is less than 1.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMinimumFractionDigits </seealso>
		Private minFractionDigits As SByte = 0

		''' <summary>
		''' True if this format will parse numbers as integers only.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isParseIntegerOnly </seealso>
		Private parseIntegerOnly As Boolean = False

		' new fields for 1.2.  byte is too small for integer digits.

		''' <summary>
		''' The maximum number of digits allowed in the integer portion of a
		''' number.  <code>maximumIntegerDigits</code> must be greater than or equal to
		''' <code>minimumIntegerDigits</code>.
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #getMaximumIntegerDigits </seealso>
		Private maximumIntegerDigits As Integer = 40

		''' <summary>
		''' The minimum number of digits allowed in the integer portion of a
		''' number.  <code>minimumIntegerDigits</code> must be less than or equal to
		''' <code>maximumIntegerDigits</code>.
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #getMinimumIntegerDigits </seealso>
		Private minimumIntegerDigits As Integer = 1

		''' <summary>
		''' The maximum number of digits allowed in the fractional portion of a
		''' number.  <code>maximumFractionDigits</code> must be greater than or equal to
		''' <code>minimumFractionDigits</code>.
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #getMaximumFractionDigits </seealso>
		Private maximumFractionDigits As Integer = 3 ' invariant, >= minFractionDigits

		''' <summary>
		''' The minimum number of digits allowed in the fractional portion of a
		''' number.  <code>minimumFractionDigits</code> must be less than or equal to
		''' <code>maximumFractionDigits</code>.
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #getMinimumFractionDigits </seealso>
		Private minimumFractionDigits As Integer = 0

		Friend Const currentSerialVersion As Integer = 1

		''' <summary>
		''' Describes the version of <code>NumberFormat</code> present on the stream.
		''' Possible values are:
		''' <ul>
		''' <li><b>0</b> (or uninitialized): the JDK 1.1 version of the stream format.
		'''     In this version, the <code>int</code> fields such as
		'''     <code>maximumIntegerDigits</code> were not present, and the <code>byte</code>
		'''     fields such as <code>maxIntegerDigits</code> are used instead.
		''' 
		''' <li><b>1</b>: the 1.2 version of the stream format.  The values of the
		'''     <code>byte</code> fields such as <code>maxIntegerDigits</code> are ignored,
		'''     and the <code>int</code> fields such as <code>maximumIntegerDigits</code>
		'''     are used instead.
		''' </ul>
		''' When streaming out a <code>NumberFormat</code>, the most recent format
		''' (corresponding to the highest allowable <code>serialVersionOnStream</code>)
		''' is always written.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion

		' Removed "implements Cloneable" clause.  Needs to update serialization
		' ID for backward compatibility.
		Friend Const serialVersionUID As Long = -2308460125733713944L


		'
		' class for AttributedCharacterIterator attributes
		'
		''' <summary>
		''' Defines constants that are used as attribute keys in the
		''' <code>AttributedCharacterIterator</code> returned
		''' from <code>NumberFormat.formatToCharacterIterator</code> and as
		''' field identifiers in <code>FieldPosition</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Class Field
			Inherits Format.Field

			' Proclaim serial compatibility with 1.4 FCS
			Private Const serialVersionUID As Long = 7494728892700160890L

			' table of all instances in this [Class], used by readResolve
			Private Shared ReadOnly instanceMap As IDictionary(Of String, Field) = New Dictionary(Of String, Field)(11)

			''' <summary>
			''' Creates a Field instance with the specified
			''' name.
			''' </summary>
			''' <param name="name"> Name of the attribute </param>
			Protected Friend Sub New(ByVal name As String)
				MyBase.New(name)
				If Me.GetType() Is GetType(NumberFormat.Field) Then instanceMap(name) = Me
			End Sub

			''' <summary>
			''' Resolves instances being deserialized to the predefined constants.
			''' </summary>
			''' <exception cref="InvalidObjectException"> if the constant could not be resolved. </exception>
			''' <returns> resolved NumberFormat.Field constant </returns>
			Protected Friend Overrides Function readResolve() As Object
				If Me.GetType() IsNot GetType(NumberFormat.Field) Then Throw New java.io.InvalidObjectException("subclass didn't correctly implement readResolve")

				Dim instance As Object = instanceMap(name)
				If instance IsNot Nothing Then
					Return instance
				Else
					Throw New java.io.InvalidObjectException("unknown attribute name")
				End If
			End Function

			''' <summary>
			''' Constant identifying the integer field.
			''' </summary>
			Public Shared ReadOnly [INTEGER] As New Field("integer")

			''' <summary>
			''' Constant identifying the fraction field.
			''' </summary>
			Public Shared ReadOnly FRACTION As New Field("fraction")

			''' <summary>
			''' Constant identifying the exponent field.
			''' </summary>
			Public Shared ReadOnly EXPONENT As New Field("exponent")

			''' <summary>
			''' Constant identifying the decimal separator field.
			''' </summary>
			Public Shared ReadOnly DECIMAL_SEPARATOR As New Field("decimal separator")

			''' <summary>
			''' Constant identifying the sign field.
			''' </summary>
			Public Shared ReadOnly SIGN As New Field("sign")

			''' <summary>
			''' Constant identifying the grouping separator field.
			''' </summary>
			Public Shared ReadOnly GROUPING_SEPARATOR As New Field("grouping separator")

			''' <summary>
			''' Constant identifying the exponent symbol field.
			''' </summary>
			Public Shared ReadOnly EXPONENT_SYMBOL As New Field("exponent symbol")

			''' <summary>
			''' Constant identifying the percent field.
			''' </summary>
			Public Shared ReadOnly PERCENT As New Field("percent")

			''' <summary>
			''' Constant identifying the permille field.
			''' </summary>
			Public Shared ReadOnly PERMILLE As New Field("per mille")

			''' <summary>
			''' Constant identifying the currency field.
			''' </summary>
			Public Shared ReadOnly CURRENCY As New Field("currency")

			''' <summary>
			''' Constant identifying the exponent sign field.
			''' </summary>
			Public Shared ReadOnly EXPONENT_SIGN As New Field("exponent sign")
		End Class
	End Class

End Namespace