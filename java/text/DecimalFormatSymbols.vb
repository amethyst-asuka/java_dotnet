Imports System

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
	''' This class represents the set of symbols (such as the decimal separator,
	''' the grouping separator, and so on) needed by <code>DecimalFormat</code>
	''' to format numbers. <code>DecimalFormat</code> creates for itself an instance of
	''' <code>DecimalFormatSymbols</code> from its locale data.  If you need to change any
	''' of these symbols, you can get the <code>DecimalFormatSymbols</code> object from
	''' your <code>DecimalFormat</code> and modify it.
	''' </summary>
	''' <seealso cref=          java.util.Locale </seealso>
	''' <seealso cref=          DecimalFormat
	''' @author       Mark Davis
	''' @author       Alan Liu </seealso>

	<Serializable> _
	Public Class DecimalFormatSymbols
		Implements Cloneable

		''' <summary>
		''' Create a DecimalFormatSymbols object for the default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' This constructor can only construct instances for the locales
		''' supported by the Java runtime environment, not for those
		''' supported by installed
		''' <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		''' implementations. For full locale coverage, use the
		''' <seealso cref="#getInstance(Locale) getInstance"/> method.
		''' <p>This is equivalent to calling
		''' {@link #DecimalFormatSymbols(Locale)
		'''     DecimalFormatSymbols(Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		Public Sub New()
			initialize(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Create a DecimalFormatSymbols object for the given locale.
		''' This constructor can only construct instances for the locales
		''' supported by the Java runtime environment, not for those
		''' supported by installed
		''' <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		''' implementations. For full locale coverage, use the
		''' <seealso cref="#getInstance(Locale) getInstance"/> method.
		''' If the specified locale contains the <seealso cref="java.util.Locale#UNICODE_LOCALE_EXTENSION"/>
		''' for the numbering system, the instance is initialized with the specified numbering
		''' system if the JRE implementation supports it. For example,
		''' <pre>
		''' NumberFormat.getNumberInstance(Locale.forLanguageTag("th-TH-u-nu-thai"))
		''' </pre>
		''' This may return a {@code NumberFormat} instance with the Thai numbering system,
		''' instead of the Latin numbering system.
		''' </summary>
		''' <param name="locale"> the desired locale </param>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Sub New(  locale As java.util.Locale)
			initialize(locale)
		End Sub

		''' <summary>
		''' Returns an array of all locales for which the
		''' <code>getInstance</code> methods of this class can return
		''' localized instances.
		''' The returned array represents the union of locales supported by the Java
		''' runtime and by installed
		''' <seealso cref="java.text.spi.DecimalFormatSymbolsProvider DecimalFormatSymbolsProvider"/>
		''' implementations.  It must contain at least a <code>Locale</code>
		''' instance equal to <seealso cref="java.util.Locale#US Locale.US"/>.
		''' </summary>
		''' <returns> an array of locales for which localized
		'''         <code>DecimalFormatSymbols</code> instances are available.
		''' @since 1.6 </returns>
		PublicShared ReadOnly PropertyavailableLocales As java.util.Locale()
			Get
				Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.text.spi.DecimalFormatSymbolsProvider))
				Return pool.availableLocales
			End Get
		End Property

		''' <summary>
		''' Gets the <code>DecimalFormatSymbols</code> instance for the default
		''' locale.  This method provides access to <code>DecimalFormatSymbols</code>
		''' instances for locales supported by the Java runtime itself as well
		''' as for those supported by installed
		''' {@link java.text.spi.DecimalFormatSymbolsProvider
		''' DecimalFormatSymbolsProvider} implementations.
		''' <p>This is equivalent to calling
		''' {@link #getInstance(Locale)
		'''     getInstance(Locale.getDefault(Locale.Category.FORMAT))}. </summary>
		''' <seealso cref= java.util.Locale#getDefault(java.util.Locale.Category) </seealso>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> a <code>DecimalFormatSymbols</code> instance.
		''' @since 1.6 </returns>
		PublicShared ReadOnly Propertyinstance As DecimalFormatSymbols
			Get
				Return getInstance(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			End Get
		End Property

		''' <summary>
		''' Gets the <code>DecimalFormatSymbols</code> instance for the specified
		''' locale.  This method provides access to <code>DecimalFormatSymbols</code>
		''' instances for locales supported by the Java runtime itself as well
		''' as for those supported by installed
		''' {@link java.text.spi.DecimalFormatSymbolsProvider
		''' DecimalFormatSymbolsProvider} implementations.
		''' If the specified locale contains the <seealso cref="java.util.Locale#UNICODE_LOCALE_EXTENSION"/>
		''' for the numbering system, the instance is initialized with the specified numbering
		''' system if the JRE implementation supports it. For example,
		''' <pre>
		''' NumberFormat.getNumberInstance(Locale.forLanguageTag("th-TH-u-nu-thai"))
		''' </pre>
		''' This may return a {@code NumberFormat} instance with the Thai numbering system,
		''' instead of the Latin numbering system.
		''' </summary>
		''' <param name="locale"> the desired locale. </param>
		''' <returns> a <code>DecimalFormatSymbols</code> instance. </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null
		''' @since 1.6 </exception>
		Public Shared Function getInstance(  locale As java.util.Locale) As DecimalFormatSymbols
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter
			adapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DecimalFormatSymbolsProvider), locale)
			Dim provider As java.text.spi.DecimalFormatSymbolsProvider = adapter.decimalFormatSymbolsProvider
			Dim dfsyms As DecimalFormatSymbols = provider.getInstance(locale)
			If dfsyms Is Nothing Then
				provider = sun.util.locale.provider.LocaleProviderAdapter.forJRE().decimalFormatSymbolsProvider
				dfsyms = provider.getInstance(locale)
			End If
			Return dfsyms
		End Function

		''' <summary>
		''' Gets the character used for zero. Different for Arabic, etc.
		''' </summary>
		''' <returns> the character used for zero </returns>
		Public Overridable Property zeroDigit As Char
			Get
				Return zeroDigit
			End Get
			Set(  zeroDigit As Char)
				Me.zeroDigit = zeroDigit
			End Set
		End Property


		''' <summary>
		''' Gets the character used for thousands separator. Different for French, etc.
		''' </summary>
		''' <returns> the grouping separator </returns>
		Public Overridable Property groupingSeparator As Char
			Get
				Return groupingSeparator
			End Get
			Set(  groupingSeparator As Char)
				Me.groupingSeparator = groupingSeparator
			End Set
		End Property


		''' <summary>
		''' Gets the character used for decimal sign. Different for French, etc.
		''' </summary>
		''' <returns> the character used for decimal sign </returns>
		Public Overridable Property decimalSeparator As Char
			Get
				Return decimalSeparator
			End Get
			Set(  decimalSeparator As Char)
				Me.decimalSeparator = decimalSeparator
			End Set
		End Property


		''' <summary>
		''' Gets the character used for per mille sign. Different for Arabic, etc.
		''' </summary>
		''' <returns> the character used for per mille sign </returns>
		Public Overridable Property perMill As Char
			Get
				Return perMill
			End Get
			Set(  perMill As Char)
				Me.perMill = perMill
			End Set
		End Property


		''' <summary>
		''' Gets the character used for percent sign. Different for Arabic, etc.
		''' </summary>
		''' <returns> the character used for percent sign </returns>
		Public Overridable Property percent As Char
			Get
				Return percent
			End Get
			Set(  percent As Char)
				Me.percent = percent
			End Set
		End Property


		''' <summary>
		''' Gets the character used for a digit in a pattern.
		''' </summary>
		''' <returns> the character used for a digit in a pattern </returns>
		Public Overridable Property digit As Char
			Get
				Return digit
			End Get
			Set(  digit As Char)
				Me.digit = digit
			End Set
		End Property


		''' <summary>
		''' Gets the character used to separate positive and negative subpatterns
		''' in a pattern.
		''' </summary>
		''' <returns> the pattern separator </returns>
		Public Overridable Property patternSeparator As Char
			Get
				Return patternSeparator
			End Get
			Set(  patternSeparator As Char)
				Me.patternSeparator = patternSeparator
			End Set
		End Property


		''' <summary>
		''' Gets the string used to represent infinity. Almost always left
		''' unchanged.
		''' </summary>
		''' <returns> the string representing infinity </returns>
		Public Overridable Property infinity As String
			Get
				Return infinity
			End Get
			Set(  infinity As String)
				Me.infinity = infinity
			End Set
		End Property


		''' <summary>
		''' Gets the string used to represent "not a number". Almost always left
		''' unchanged.
		''' </summary>
		''' <returns> the string representing "not a number" </returns>
		Public Overridable Property naN As String
			Get
				Return NaN
			End Get
			Set(  NaN As String)
				Me.NaN = NaN
			End Set
		End Property


		''' <summary>
		''' Gets the character used to represent minus sign. If no explicit
		''' negative format is specified, one is formed by prefixing
		''' minusSign to the positive format.
		''' </summary>
		''' <returns> the character representing minus sign </returns>
		Public Overridable Property minusSign As Char
			Get
				Return minusSign
			End Get
			Set(  minusSign As Char)
				Me.minusSign = minusSign
			End Set
		End Property


		''' <summary>
		''' Returns the currency symbol for the currency of these
		''' DecimalFormatSymbols in their locale.
		''' </summary>
		''' <returns> the currency symbol
		''' @since 1.2 </returns>
		Public Overridable Property currencySymbol As String
			Get
				Return currencySymbol
			End Get
			Set(  currency As String)
				currencySymbol = currency
			End Set
		End Property


		''' <summary>
		''' Returns the ISO 4217 currency code of the currency of these
		''' DecimalFormatSymbols.
		''' </summary>
		''' <returns> the currency code
		''' @since 1.2 </returns>
		Public Overridable Property internationalCurrencySymbol As String
			Get
				Return intlCurrencySymbol
			End Get
			Set(  currencyCode As String)
				intlCurrencySymbol = currencyCode
				currency = Nothing
				If currencyCode IsNot Nothing Then
					Try
						currency = java.util.Currency.getInstance(currencyCode)
						currencySymbol = currency.symbol
					Catch e As IllegalArgumentException
					End Try
				End If
			End Set
		End Property


		''' <summary>
		''' Gets the currency of these DecimalFormatSymbols. May be null if the
		''' currency symbol attribute was previously set to a value that's not
		''' a valid ISO 4217 currency code.
		''' </summary>
		''' <returns> the currency used, or null
		''' @since 1.4 </returns>
		Public Overridable Property currency As java.util.Currency
			Get
				Return currency
			End Get
			Set(  currency As java.util.Currency)
				If currency Is Nothing Then Throw New NullPointerException
				Me.currency = currency
				intlCurrencySymbol = currency.currencyCode
				currencySymbol = currency.getSymbol(locale)
			End Set
		End Property



		''' <summary>
		''' Returns the monetary decimal separator.
		''' </summary>
		''' <returns> the monetary decimal separator
		''' @since 1.2 </returns>
		Public Overridable Property monetaryDecimalSeparator As Char
			Get
				Return monetarySeparator
			End Get
			Set(  sep As Char)
				monetarySeparator = sep
			End Set
		End Property


		'------------------------------------------------------------
		' BEGIN   Package Private methods ... to be made public later
		'------------------------------------------------------------

		''' <summary>
		''' Returns the character used to separate the mantissa from the exponent.
		''' </summary>
		Friend Overridable Property exponentialSymbol As Char
			Get
				Return exponential
			End Get
			Set(  exp As Char)
				exponential = exp
			End Set
		End Property
	  ''' <summary>
	  ''' Returns the string used to separate the mantissa from the exponent.
	  ''' Examples: "x10^" for 1.23x10^4, "E" for 1.23E4.
	  ''' </summary>
	  ''' <returns> the exponent separator string </returns>
	  ''' <seealso cref= #setExponentSeparator(java.lang.String)
	  ''' @since 1.6 </seealso>
		Public Overridable Property exponentSeparator As String
			Get
				Return exponentialSeparator
			End Get
			Set(  exp As String)
				If exp Is Nothing Then Throw New NullPointerException
				exponentialSeparator = exp
			End Set
		End Property




		'------------------------------------------------------------
		' END     Package Private methods ... to be made public later
		'------------------------------------------------------------

		''' <summary>
		''' Standard override.
		''' </summary>
		Public Overrides Function clone() As Object
			Try
				Return CType(MyBase.clone(), DecimalFormatSymbols)
				' other fields are bit-copied
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Override equals.
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Me Is obj Then Return True
			If Me.GetType() IsNot obj.GetType() Then Return False
			Dim other As DecimalFormatSymbols = CType(obj, DecimalFormatSymbols)
			Return (zeroDigit = other.zeroDigit AndAlso groupingSeparator = other.groupingSeparator AndAlso decimalSeparator = other.decimalSeparator AndAlso percent = other.percent AndAlso perMill = other.perMill AndAlso digit = other.digit AndAlso minusSign = other.minusSign AndAlso patternSeparator = other.patternSeparator AndAlso infinity.Equals(other.infinity) AndAlso NaN.Equals(other.NaN) AndAlso currencySymbol.Equals(other.currencySymbol) AndAlso intlCurrencySymbol.Equals(other.intlCurrencySymbol) AndAlso currency Is other.currency AndAlso monetarySeparator = other.monetarySeparator AndAlso exponentialSeparator.Equals(other.exponentialSeparator) AndAlso locale.Equals(other.locale))
		End Function

		''' <summary>
		''' Override hashCode.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
				Dim result As Integer = AscW(zeroDigit)
				result = result * 37 + AscW(groupingSeparator)
				result = result * 37 + AscW(decimalSeparator)
				Return result
		End Function

		''' <summary>
		''' Initializes the symbols from the FormatData resource bundle.
		''' </summary>
		Private Sub initialize(  locale As java.util.Locale)
			Me.locale = locale

			' get resource bundle data
			Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DecimalFormatSymbolsProvider), locale)
			' Avoid potential recursions
			If Not(TypeOf adapter Is sun.util.locale.provider.ResourceBundleBasedAdapter) Then adapter = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased
			Dim data As Object() = adapter.getLocaleResources(locale).decimalFormatSymbolsData
			Dim numberElements As String() = CType(data(0), String())

			decimalSeparator = numberElements(0).Chars(0)
			groupingSeparator = numberElements(1).Chars(0)
			patternSeparator = numberElements(2).Chars(0)
			percent = numberElements(3).Chars(0)
			zeroDigit = numberElements(4).Chars(0) 'different for Arabic,etc.
			digit = numberElements(5).Chars(0)
			minusSign = numberElements(6).Chars(0)
			exponential = numberElements(7).Chars(0)
			exponentialSeparator = numberElements(7) 'string representation new since 1.6
			perMill = numberElements(8).Chars(0)
			infinity = numberElements(9)
			NaN = numberElements(10)

			' Try to obtain the currency used in the locale's country.
			' Check for empty country string separately because it's a valid
			' country ID for Locale (and used for the C locale), but not a valid
			' ISO 3166 country code, and exceptions are expensive.
			If locale.country.length() > 0 Then
				Try
					currency = java.util.Currency.getInstance(locale)
				Catch e As IllegalArgumentException
					' use default values below for compatibility
				End Try
			End If
			If currency IsNot Nothing Then
				intlCurrencySymbol = currency.currencyCode
				If data(1) IsNot Nothing AndAlso data(1) Is intlCurrencySymbol Then
					currencySymbol = CStr(data(2))
				Else
					currencySymbol = currency.getSymbol(locale)
					data(1) = intlCurrencySymbol
					data(2) = currencySymbol
				End If
			Else
				' default values
				intlCurrencySymbol = "XXX"
				Try
					currency = java.util.Currency.getInstance(intlCurrencySymbol)
				Catch e As IllegalArgumentException
				End Try
				currencySymbol = ChrW(&H00A4).ToString()
			End If
			' Currently the monetary decimal separator is the same as the
			' standard decimal separator for all locales that we support.
			' If that changes, add a new entry to NumberElements.
			monetarySeparator = decimalSeparator
		End Sub

		''' <summary>
		''' Reads the default serializable fields, provides default values for objects
		''' in older serial versions, and initializes non-serializable fields.
		''' If <code>serialVersionOnStream</code>
		''' is less than 1, initializes <code>monetarySeparator</code> to be
		''' the same as <code>decimalSeparator</code> and <code>exponential</code>
		''' to be 'E'.
		''' If <code>serialVersionOnStream</code> is less than 2,
		''' initializes <code>locale</code>to the root locale, and initializes
		''' If <code>serialVersionOnStream</code> is less than 3, it initializes
		''' <code>exponentialSeparator</code> using <code>exponential</code>.
		''' Sets <code>serialVersionOnStream</code> back to the maximum allowed value so that
		''' default serialization will work properly if this object is streamed out again.
		''' Initializes the currency from the intlCurrencySymbol field.
		''' 
		''' @since JDK 1.1.6
		''' </summary>
		Private Sub readObject(  stream As java.io.ObjectInputStream)
			stream.defaultReadObject()
			If serialVersionOnStream < 1 Then
				' Didn't have monetarySeparator or exponential field;
				' use defaults.
				monetarySeparator = decimalSeparator
				exponential = "E"c
			End If
			If serialVersionOnStream < 2 Then locale = java.util.Locale.ROOT
			If serialVersionOnStream < 3 Then exponentialSeparator = Char.ToString(exponential)
			serialVersionOnStream = currentSerialVersion

			If intlCurrencySymbol IsNot Nothing Then
				Try
					 currency = java.util.Currency.getInstance(intlCurrencySymbol)
				Catch e As IllegalArgumentException
				End Try
			End If
		End Sub

		''' <summary>
		''' Character used for zero.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getZeroDigit </seealso>
		Private zeroDigit As Char

		''' <summary>
		''' Character used for thousands separator.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getGroupingSeparator </seealso>
		Private groupingSeparator As Char

		''' <summary>
		''' Character used for decimal sign.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getDecimalSeparator </seealso>
		Private decimalSeparator As Char

		''' <summary>
		''' Character used for per mille sign.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getPerMill </seealso>
		Private perMill As Char

		''' <summary>
		''' Character used for percent sign.
		''' @serial </summary>
		''' <seealso cref= #getPercent </seealso>
		Private percent As Char

		''' <summary>
		''' Character used for a digit in a pattern.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getDigit </seealso>
		Private digit As Char

		''' <summary>
		''' Character used to separate positive and negative subpatterns
		''' in a pattern.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getPatternSeparator </seealso>
		Private patternSeparator As Char

		''' <summary>
		''' String used to represent infinity.
		''' @serial </summary>
		''' <seealso cref= #getInfinity </seealso>
		Private infinity As String

		''' <summary>
		''' String used to represent "not a number".
		''' @serial </summary>
		''' <seealso cref= #getNaN </seealso>
		Private NaN As String

		''' <summary>
		''' Character used to represent minus sign.
		''' @serial </summary>
		''' <seealso cref= #getMinusSign </seealso>
		Private minusSign As Char

		''' <summary>
		''' String denoting the local currency, e.g. "$".
		''' @serial </summary>
		''' <seealso cref= #getCurrencySymbol </seealso>
		Private currencySymbol As String

		''' <summary>
		''' ISO 4217 currency code denoting the local currency, e.g. "USD".
		''' @serial </summary>
		''' <seealso cref= #getInternationalCurrencySymbol </seealso>
		Private intlCurrencySymbol As String

		''' <summary>
		''' The decimal separator used when formatting currency values.
		''' @serial
		''' @since JDK 1.1.6 </summary>
		''' <seealso cref= #getMonetaryDecimalSeparator </seealso>
		Private monetarySeparator As Char ' Field new in JDK 1.1.6

		''' <summary>
		''' The character used to distinguish the exponent in a number formatted
		''' in exponential notation, e.g. 'E' for a number such as "1.23E45".
		''' <p>
		''' Note that the public API provides no way to set this field,
		''' even though it is supported by the implementation and the stream format.
		''' The intent is that this will be added to the API in the future.
		''' 
		''' @serial
		''' @since JDK 1.1.6
		''' </summary>
		Private exponential As Char ' Field new in JDK 1.1.6

	  ''' <summary>
	  ''' The string used to separate the mantissa from the exponent.
	  ''' Examples: "x10^" for 1.23x10^4, "E" for 1.23E4.
	  ''' <p>
	  ''' If both <code>exponential</code> and <code>exponentialSeparator</code>
	  ''' exist, this <code>exponentialSeparator</code> has the precedence.
	  ''' 
	  ''' @serial
	  ''' @since 1.6
	  ''' </summary>
		Private exponentialSeparator As String ' Field new in JDK 1.6

		''' <summary>
		''' The locale of these currency format symbols.
		''' 
		''' @serial
		''' @since 1.4
		''' </summary>
		Private locale As java.util.Locale

		' currency; only the ISO code is serialized.
		<NonSerialized> _
		Private currency As java.util.Currency

		' Proclaim JDK 1.1 FCS compatibility
		Friend Const serialVersionUID As Long = 5772796243397350300L

		' The internal serial version which says which version was written
		' - 0 (default) for version up to JDK 1.1.5
		' - 1 for version from JDK 1.1.6, which includes two new fields:
		'     monetarySeparator and exponential.
		' - 2 for version from J2SE 1.4, which includes locale field.
		' - 3 for version from J2SE 1.6, which includes exponentialSeparator field.
		Private Const currentSerialVersion As Integer = 3

		''' <summary>
		''' Describes the version of <code>DecimalFormatSymbols</code> present on the stream.
		''' Possible values are:
		''' <ul>
		''' <li><b>0</b> (or uninitialized): versions prior to JDK 1.1.6.
		''' 
		''' <li><b>1</b>: Versions written by JDK 1.1.6 or later, which include
		'''      two new fields: <code>monetarySeparator</code> and <code>exponential</code>.
		''' <li><b>2</b>: Versions written by J2SE 1.4 or later, which include a
		'''      new <code>locale</code> field.
		''' <li><b>3</b>: Versions written by J2SE 1.6 or later, which include a
		'''      new <code>exponentialSeparator</code> field.
		''' </ul>
		''' When streaming out a <code>DecimalFormatSymbols</code>, the most recent format
		''' (corresponding to the highest allowable <code>serialVersionOnStream</code>)
		''' is always written.
		''' 
		''' @serial
		''' @since JDK 1.1.6
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion
	End Class

End Namespace