Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Collections.Concurrent

'
' * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents a currency. Currencies are identified by their ISO 4217 currency
	''' codes. Visit the <a href="http://www.iso.org/iso/home/standards/currency_codes.htm">
	''' ISO web site</a> for more information.
	''' <p>
	''' The class is designed so that there's never more than one
	''' <code>Currency</code> instance for any given currency. Therefore, there's
	''' no public constructor. You obtain a <code>Currency</code> instance using
	''' the <code>getInstance</code> methods.
	''' <p>
	''' Users can supersede the Java runtime currency data by means of the system
	''' property {@code java.util.currency.data}. If this system property is
	''' defined then its value is the location of a properties file, the contents of
	''' which are key/value pairs of the ISO 3166 country codes and the ISO 4217
	''' currency data respectively.  The value part consists of three ISO 4217 values
	''' of a currency, i.e., an alphabetic code, a numeric code, and a minor unit.
	''' Those three ISO 4217 values are separated by commas.
	''' The lines which start with '#'s are considered comment lines. An optional UTC
	''' timestamp may be specified per currency entry if users need to specify a
	''' cutover date indicating when the new data comes into effect. The timestamp is
	''' appended to the end of the currency properties and uses a comma as a separator.
	''' If a UTC datestamp is present and valid, the JRE will only use the new currency
	''' properties if the current UTC date is later than the date specified at class
	''' loading time. The format of the timestamp must be of ISO 8601 format :
	''' {@code 'yyyy-MM-dd'T'HH:mm:ss'}. For example,
	''' <p>
	''' <code>
	''' #Sample currency properties<br>
	''' JP=JPZ,999,0
	''' </code>
	''' <p>
	''' will supersede the currency data for Japan.
	''' 
	''' <p>
	''' <code>
	''' #Sample currency properties with cutover date<br>
	''' JP=JPZ,999,0,2014-01-01T00:00:00
	''' </code>
	''' <p>
	''' will supersede the currency data for Japan if {@code Currency} class is loaded after
	''' 1st January 2014 00:00:00 GMT.
	''' <p>
	''' Where syntactically malformed entries are encountered, the entry is ignored
	''' and the remainder of entries in file are processed. For instances where duplicate
	''' country code entries exist, the behavior of the Currency information for that
	''' {@code Currency} is undefined and the remainder of entries in file are processed.
	''' 
	''' @since 1.4
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Currency

		Private Const serialVersionUID As Long = -158308464356906721L

		''' <summary>
		''' ISO 4217 currency code for this currency.
		''' 
		''' @serial
		''' </summary>
		Private ReadOnly currencyCode As String

		''' <summary>
		''' Default fraction digits for this currency.
		''' Set from currency data tables.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly defaultFractionDigits As Integer

		''' <summary>
		''' ISO 4217 numeric code for this currency.
		''' Set from currency data tables.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly numericCode As Integer


		' class data: instance map

		Private Shared instances As java.util.concurrent.ConcurrentMap(Of String, Currency) = New ConcurrentDictionary(Of String, Currency)(7)
		Private Shared available As HashSet(Of Currency)

		' Class data: currency data obtained from currency.data file.
		' Purpose:
		' - determine valid country codes
		' - determine valid currency codes
		' - map country codes to currency codes
		' - obtain default fraction digits for currency codes
		'
		' sc = special case; dfd = default fraction digits
		' Simple countries are those where the country code is a prefix of the
		' currency code, and there are no known plans to change the currency.
		'
		' table formats:
		' - mainTable:
		'   - maps country code to 32-bit int
		'   - 26*26 entries, corresponding to [A-Z]*[A-Z]
		'   - \u007F -> not valid country
		'   - bits 20-31: unused
		'   - bits 10-19: numeric code (0 to 1023)
		'   - bit 9: 1 - special case, bits 0-4 indicate which one
		'            0 - simple country, bits 0-4 indicate final char of currency code
		'   - bits 5-8: fraction digits for simple countries, 0 for special cases
		'   - bits 0-4: final char for currency code for simple country, or ID of special case
		' - special case IDs:
		'   - 0: country has no currency
		'   - other: index into sc* arrays + 1
		' - scCutOverTimes: cut-over time in millis as returned by
		'   System.currentTimeMillis for special case countries that are changing
		'   currencies; Long.MAX_VALUE for countries that are not changing currencies
		' - scOldCurrencies: old currencies for special case countries
		' - scNewCurrencies: new currencies for special case countries that are
		'   changing currencies; null for others
		' - scOldCurrenciesDFD: default fraction digits for old currencies
		' - scNewCurrenciesDFD: default fraction digits for new currencies, 0 for
		'   countries that are not changing currencies
		' - otherCurrencies: concatenation of all currency codes that are not the
		'   main currency of a simple country, separated by "-"
		' - otherCurrenciesDFD: decimal format digits for currencies in otherCurrencies, same order

		Friend Shared formatVersion As Integer
		Friend Shared dataVersion As Integer
		Friend Shared mainTable As Integer()
		Friend Shared scCutOverTimes As Long()
		Friend Shared scOldCurrencies As String()
		Friend Shared scNewCurrencies As String()
		Friend Shared scOldCurrenciesDFD As Integer()
		Friend Shared scNewCurrenciesDFD As Integer()
		Friend Shared scOldCurrenciesNumericCode As Integer()
		Friend Shared scNewCurrenciesNumericCode As Integer()
		Friend Shared otherCurrencies As String
		Friend Shared otherCurrenciesDFD As Integer()
		Friend Shared otherCurrenciesNumericCode As Integer()

		' handy constants - must match definitions in GenerateCurrencyData
		' magic number
		Private Const MAGIC_NUMBER As Integer = &H43757244
		' number of characters from A to Z
		Private Shared ReadOnly A_TO_Z As Integer = (AscW("Z"c) - AscW("A"c)) + 1
		' entry for invalid country codes
		Private Const INVALID_COUNTRY_ENTRY As Integer = &H7F
		' entry for countries without currency
		Private Const COUNTRY_WITHOUT_CURRENCY_ENTRY As Integer = &H200
		' mask for simple case country entries
		Private Const SIMPLE_CASE_COUNTRY_MASK As Integer = &H0
		' mask for simple case country entry final character
		Private Const SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK As Integer = &H1F
		' mask for simple case country entry default currency digits
		Private Const SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK As Integer = &H1E0
		' shift count for simple case country entry default currency digits
		Private Const SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT As Integer = 5
		' maximum number for simple case country entry default currency digits
		Private Const SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS As Integer = 9
		' mask for special case country entries
		Private Const SPECIAL_CASE_COUNTRY_MASK As Integer = &H200
		' mask for special case country index
		Private Const SPECIAL_CASE_COUNTRY_INDEX_MASK As Integer = &H1F
		' delta from entry index component in main table to index into special case tables
		Private Const SPECIAL_CASE_COUNTRY_INDEX_DELTA As Integer = 1
		' mask for distinguishing simple and special case countries
		Private Shared ReadOnly COUNTRY_TYPE_MASK As Integer = SIMPLE_CASE_COUNTRY_MASK Or SPECIAL_CASE_COUNTRY_MASK
		' mask for the numeric code of the currency
		Private Const NUMERIC_CODE_MASK As Integer = &HFFC00
		' shift count for the numeric code of the currency
		Private Const NUMERIC_CODE_SHIFT As Integer = 10

		' Currency data format version
		Private Const VALID_FORMAT_VERSION As Integer = 2

		Shared Sub New()
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As Void
				Dim homeDir As String = System.getProperty("java.home")
				Try
					Dim dataFile As String = homeDir + File.separator & "lib" & File.separator & "currency.data"
					Using dis As New java.io.DataInputStream(New java.io.BufferedInputStream(New java.io.FileInputStream(dataFile)))
						If dis.readInt() <> MAGIC_NUMBER Then Throw New InternalError("Currency data is possibly corrupted")
						formatVersion = dis.readInt()
						If formatVersion <> VALID_FORMAT_VERSION Then Throw New InternalError("Currency data format is incorrect")
						dataVersion = dis.readInt()
						mainTable = readIntArray(dis, A_TO_Z * A_TO_Z)
						Dim scCount As Integer = dis.readInt()
						scCutOverTimes = readLongArray(dis, scCount)
						scOldCurrencies = readStringArray(dis, scCount)
						scNewCurrencies = readStringArray(dis, scCount)
						scOldCurrenciesDFD = readIntArray(dis, scCount)
						scNewCurrenciesDFD = readIntArray(dis, scCount)
						scOldCurrenciesNumericCode = readIntArray(dis, scCount)
						scNewCurrenciesNumericCode = readIntArray(dis, scCount)
						Dim ocCount As Integer = dis.readInt()
						otherCurrencies = dis.readUTF()
						otherCurrenciesDFD = readIntArray(dis, ocCount)
						otherCurrenciesNumericCode = readIntArray(dis, ocCount)
					End Using
				Catch e As java.io.IOException
					Throw New InternalError(e)
				End Try

				' look for the properties file for overrides
				Dim propsFile As String = System.getProperty("java.util.currency.data")
				If propsFile Is Nothing Then propsFile = homeDir + File.separator & "lib" & File.separator & "currency.properties"
				Try
					Dim propFile As New File(propsFile)
					If propFile.exists() Then
						Dim props As New Properties
						Using fr As New java.io.FileReader(propFile)
							props.load(fr)
						End Using
						Dim keys As [Set](Of String) = props.stringPropertyNames()
						Dim propertiesPattern As java.util.regex.Pattern = java.util.regex.Pattern.compile("([A-Z]{3})\s*,\s*(\d{3})\s*,\s*" & "(\d+)\s*,?\s*(\d{4}-\d{2}-\d{2}T\d{2}:" & "\d{2}:\d{2})?")
						For Each key As String In keys
						   replaceCurrencyData(propertiesPattern, key.ToUpper(Locale.ROOT), props.getProperty(key).ToUpper(Locale.ROOT))
						Next key
					End If
				Catch e As java.io.IOException
					info("currency.properties is ignored because of an IOException", e)
				End Try
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Constants for retrieving localized names from the name providers.
		''' </summary>
		Private Const SYMBOL As Integer = 0
		Private Const DISPLAYNAME As Integer = 1


		''' <summary>
		''' Constructs a <code>Currency</code> instance. The constructor is private
		''' so that we can insure that there's never more than one instance for a
		''' given currency.
		''' </summary>
		Private Sub New(ByVal currencyCode As String, ByVal defaultFractionDigits As Integer, ByVal numericCode As Integer)
			Me.currencyCode = currencyCode
			Me.defaultFractionDigits = defaultFractionDigits
			Me.numericCode = numericCode
		End Sub

		''' <summary>
		''' Returns the <code>Currency</code> instance for the given currency code.
		''' </summary>
		''' <param name="currencyCode"> the ISO 4217 code of the currency </param>
		''' <returns> the <code>Currency</code> instance for the given currency code </returns>
		''' <exception cref="NullPointerException"> if <code>currencyCode</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not
		''' a supported ISO 4217 code. </exception>
		Public Shared Function getInstance(ByVal currencyCode As String) As Currency
			Return getInstance(currencyCode, Integer.MinValue, 0)
		End Function

		Private Shared Function getInstance(ByVal currencyCode As String, ByVal defaultFractionDigits As Integer, ByVal numericCode As Integer) As Currency
			' Try to look up the currency code in the instances table.
			' This does the null pointer check as a side effect.
			' Also, if there already is an entry, the currencyCode must be valid.
			Dim instance_Renamed As Currency = instances.get(currencyCode)
			If instance_Renamed IsNot Nothing Then Return instance_Renamed

			If defaultFractionDigits = Integer.MinValue Then
				' Currency code not internally generated, need to verify first
				' A currency code must have 3 characters and exist in the main table
				' or in the list of other currencies.
				If currencyCode.length() <> 3 Then Throw New IllegalArgumentException
				Dim char1 As Char = currencyCode.Chars(0)
				Dim char2 As Char = currencyCode.Chars(1)
				Dim tableEntry As Integer = getMainTableEntry(char1, char2)
				If (tableEntry And COUNTRY_TYPE_MASK) = SIMPLE_CASE_COUNTRY_MASK AndAlso tableEntry <> INVALID_COUNTRY_ENTRY AndAlso AscW(currencyCode.Chars(2)) - AscW("A"c) = (tableEntry And SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK) Then
					defaultFractionDigits = (tableEntry And SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT
					numericCode = (tableEntry And NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT
				Else
					' Check for '-' separately so we don't get false hits in the table.
					If currencyCode.Chars(2) = "-"c Then Throw New IllegalArgumentException
					Dim index As Integer = otherCurrencies.IndexOf(currencyCode)
					If index = -1 Then Throw New IllegalArgumentException
					defaultFractionDigits = otherCurrenciesDFD(index \ 4)
					numericCode = otherCurrenciesNumericCode(index \ 4)
				End If
			End If

			Dim currencyVal As New Currency(currencyCode, defaultFractionDigits, numericCode)
			instance_Renamed = instances.putIfAbsent(currencyCode, currencyVal)
			Return (If(instance_Renamed IsNot Nothing, instance_Renamed, currencyVal))
		End Function

		''' <summary>
		''' Returns the <code>Currency</code> instance for the country of the
		''' given locale. The language and variant components of the locale
		''' are ignored. The result may vary over time, as countries change their
		''' currencies. For example, for the original member countries of the
		''' European Monetary Union, the method returns the old national currencies
		''' until December 31, 2001, and the Euro from January 1, 2002, local time
		''' of the respective countries.
		''' <p>
		''' The method returns <code>null</code> for territories that don't
		''' have a currency, such as Antarctica.
		''' </summary>
		''' <param name="locale"> the locale for whose country a <code>Currency</code>
		''' instance is needed </param>
		''' <returns> the <code>Currency</code> instance for the country of the given
		''' locale, or {@code null} </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> or its country
		''' code is {@code null} </exception>
		''' <exception cref="IllegalArgumentException"> if the country of the given {@code locale}
		''' is not a supported ISO 3166 country code. </exception>
		Public Shared Function getInstance(ByVal locale_Renamed As Locale) As Currency
			Dim country As String = locale_Renamed.country
			If country Is Nothing Then Throw New NullPointerException

			If country.length() <> 2 Then Throw New IllegalArgumentException

			Dim char1 As Char = country.Chars(0)
			Dim char2 As Char = country.Chars(1)
			Dim tableEntry As Integer = getMainTableEntry(char1, char2)
			If (tableEntry And COUNTRY_TYPE_MASK) = SIMPLE_CASE_COUNTRY_MASK AndAlso tableEntry <> INVALID_COUNTRY_ENTRY Then
				Dim finalChar As Char = ChrW((tableEntry And SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK) + AscW("A"c))
				Dim defaultFractionDigits_Renamed As Integer = (tableEntry And SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT
				Dim numericCode_Renamed As Integer = (tableEntry And NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT
				Dim sb As New StringBuilder(country)
				sb.append(finalChar)
				Return getInstance(sb.ToString(), defaultFractionDigits_Renamed, numericCode_Renamed)
			Else
				' special cases
				If tableEntry = INVALID_COUNTRY_ENTRY Then Throw New IllegalArgumentException
				If tableEntry = COUNTRY_WITHOUT_CURRENCY_ENTRY Then
					Return Nothing
				Else
					Dim index As Integer = (tableEntry And SPECIAL_CASE_COUNTRY_INDEX_MASK) - SPECIAL_CASE_COUNTRY_INDEX_DELTA
					If scCutOverTimes(index) = Long.MaxValue OrElse System.currentTimeMillis() < scCutOverTimes(index) Then
						Return getInstance(scOldCurrencies(index), scOldCurrenciesDFD(index), scOldCurrenciesNumericCode(index))
					Else
						Return getInstance(scNewCurrencies(index), scNewCurrenciesDFD(index), scNewCurrenciesNumericCode(index))
					End If
				End If
			End If
		End Function

		''' <summary>
		''' Gets the set of available currencies.  The returned set of currencies
		''' contains all of the available currencies, which may include currencies
		''' that represent obsolete ISO 4217 codes.  The set can be modified
		''' without affecting the available currencies in the runtime.
		''' </summary>
		''' <returns> the set of available currencies.  If there is no currency
		'''    available in the runtime, the returned set is empty.
		''' @since 1.7 </returns>
		Public Property Shared availableCurrencies As [Set](Of Currency)
			Get
				SyncLock GetType(Currency)
					If available Is Nothing Then
						available = New HashSet(Of )(256)
    
						' Add simple currencies first
						For c1 As Char = AscW("A"c) To AscW("Z"c)
							For c2 As Char = AscW("A"c) To AscW("Z"c)
								Dim tableEntry As Integer = getMainTableEntry(c1, c2)
								If (tableEntry And COUNTRY_TYPE_MASK) = SIMPLE_CASE_COUNTRY_MASK AndAlso tableEntry <> INVALID_COUNTRY_ENTRY Then
									Dim finalChar As Char = ChrW((tableEntry And SIMPLE_CASE_COUNTRY_FINAL_CHAR_MASK) + AscW("A"c))
									Dim defaultFractionDigits_Renamed As Integer = (tableEntry And SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_MASK) >> SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT
									Dim numericCode_Renamed As Integer = (tableEntry And NUMERIC_CODE_MASK) >> NUMERIC_CODE_SHIFT
									Dim sb As New StringBuilder
									sb.append(c1)
									sb.append(c2)
									sb.append(finalChar)
									available.add(getInstance(sb.ToString(), defaultFractionDigits_Renamed, numericCode_Renamed))
								End If
							Next c2
						Next c1
    
						' Now add other currencies
						Dim st As New StringTokenizer(otherCurrencies, "-")
						Do While st.hasMoreElements()
							available.add(getInstance(CStr(st.nextElement())))
						Loop
					End If
				End SyncLock
    
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim result As [Set](Of Currency) = CType(available.clone(), Set(Of Currency))
				Return result
			End Get
		End Property

		''' <summary>
		''' Gets the ISO 4217 currency code of this currency.
		''' </summary>
		''' <returns> the ISO 4217 currency code of this currency. </returns>
		Public Property currencyCode As String
			Get
				Return currencyCode
			End Get
		End Property

		''' <summary>
		''' Gets the symbol of this currency for the default
		''' <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		''' For example, for the US Dollar, the symbol is "$" if the default
		''' locale is the US, while for other locales it may be "US$". If no
		''' symbol can be determined, the ISO 4217 currency code is returned.
		''' <p>
		''' This is equivalent to calling
		''' {@link #getSymbol(Locale)
		'''     getSymbol(Locale.getDefault(Locale.Category.DISPLAY))}.
		''' </summary>
		''' <returns> the symbol of this currency for the default
		'''     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale </returns>
		Public Property symbol As String
			Get
				Return getSymbol(Locale.getDefault(Locale.Category.DISPLAY))
			End Get
		End Property

		''' <summary>
		''' Gets the symbol of this currency for the specified locale.
		''' For example, for the US Dollar, the symbol is "$" if the specified
		''' locale is the US, while for other locales it may be "US$". If no
		''' symbol can be determined, the ISO 4217 currency code is returned.
		''' </summary>
		''' <param name="locale"> the locale for which a display name for this currency is
		''' needed </param>
		''' <returns> the symbol of this currency for the specified locale </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		Public Function getSymbol(ByVal locale_Renamed As Locale) As String
			Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.util.spi.CurrencyNameProvider))
			Dim symbol_Renamed As String = pool.getLocalizedObject(CurrencyNameGetter.INSTANCE, locale_Renamed, currencyCode, SYMBOL)
			If symbol_Renamed IsNot Nothing Then Return symbol_Renamed

			' use currency code as symbol of last resort
			Return currencyCode
		End Function

		''' <summary>
		''' Gets the default number of fraction digits used with this currency.
		''' For example, the default number of fraction digits for the Euro is 2,
		''' while for the Japanese Yen it's 0.
		''' In the case of pseudo-currencies, such as IMF Special Drawing Rights,
		''' -1 is returned.
		''' </summary>
		''' <returns> the default number of fraction digits used with this currency </returns>
		Public Property defaultFractionDigits As Integer
			Get
				Return defaultFractionDigits
			End Get
		End Property

		''' <summary>
		''' Returns the ISO 4217 numeric code of this currency.
		''' </summary>
		''' <returns> the ISO 4217 numeric code of this currency
		''' @since 1.7 </returns>
		Public Property numericCode As Integer
			Get
				Return numericCode
			End Get
		End Property

		''' <summary>
		''' Gets the name that is suitable for displaying this currency for
		''' the default <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale.
		''' If there is no suitable display name found
		''' for the default locale, the ISO 4217 currency code is returned.
		''' <p>
		''' This is equivalent to calling
		''' {@link #getDisplayName(Locale)
		'''     getDisplayName(Locale.getDefault(Locale.Category.DISPLAY))}.
		''' </summary>
		''' <returns> the display name of this currency for the default
		'''     <seealso cref="Locale.Category#DISPLAY DISPLAY"/> locale
		''' @since 1.7 </returns>
		Public Property displayName As String
			Get
				Return getDisplayName(Locale.getDefault(Locale.Category.DISPLAY))
			End Get
		End Property

		''' <summary>
		''' Gets the name that is suitable for displaying this currency for
		''' the specified locale.  If there is no suitable display name found
		''' for the specified locale, the ISO 4217 currency code is returned.
		''' </summary>
		''' <param name="locale"> the locale for which a display name for this currency is
		''' needed </param>
		''' <returns> the display name of this currency for the specified locale </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null
		''' @since 1.7 </exception>
		Public Function getDisplayName(ByVal locale_Renamed As Locale) As String
			Dim pool As sun.util.locale.provider.LocaleServiceProviderPool = sun.util.locale.provider.LocaleServiceProviderPool.getPool(GetType(java.util.spi.CurrencyNameProvider))
			Dim result As String = pool.getLocalizedObject(CurrencyNameGetter.INSTANCE, locale_Renamed, currencyCode, DISPLAYNAME)
			If result IsNot Nothing Then Return result

			' use currency code as symbol of last resort
			Return currencyCode
		End Function

		''' <summary>
		''' Returns the ISO 4217 currency code of this currency.
		''' </summary>
		''' <returns> the ISO 4217 currency code of this currency </returns>
		Public Overrides Function ToString() As String
			Return currencyCode
		End Function

		''' <summary>
		''' Resolves instances being deserialized to a single instance per currency.
		''' </summary>
		Private Function readResolve() As Object
			Return getInstance(currencyCode)
		End Function

		''' <summary>
		''' Gets the main table entry for the country whose country code consists
		''' of char1 and char2.
		''' </summary>
		Private Shared Function getMainTableEntry(ByVal char1 As Char, ByVal char2 As Char) As Integer
			If char1 < "A"c OrElse char1 > "Z"c OrElse char2 < "A"c OrElse char2 > "Z"c Then Throw New IllegalArgumentException
			Return mainTable((AscW(char1) - AscW("A"c)) * A_TO_Z + (AscW(char2) - AscW("A"c)))
		End Function

		''' <summary>
		''' Sets the main table entry for the country whose country code consists
		''' of char1 and char2.
		''' </summary>
		Private Shared Sub setMainTableEntry(ByVal char1 As Char, ByVal char2 As Char, ByVal entry As Integer)
			If char1 < "A"c OrElse char1 > "Z"c OrElse char2 < "A"c OrElse char2 > "Z"c Then Throw New IllegalArgumentException
			mainTable((AscW(char1) - AscW("A"c)) * A_TO_Z + (AscW(char2) - AscW("A"c))) = entry
		End Sub

		''' <summary>
		''' Obtains a localized currency names from a CurrencyNameProvider
		''' implementation.
		''' </summary>
		Private Class CurrencyNameGetter
			Implements sun.util.locale.provider.LocaleServiceProviderPool.LocalizedObjectGetter(Of java.util.spi.CurrencyNameProvider, String)

			Private Shared ReadOnly INSTANCE As New CurrencyNameGetter

			Public Overrides Function getObject(ByVal currencyNameProvider As java.util.spi.CurrencyNameProvider, ByVal locale_Renamed As Locale, ByVal key As String, ParamArray ByVal params As Object()) As String
				Debug.Assert(params.Length = 1)
				Dim type As Integer = CInt(Fix(params(0)))

				Select Case type
				Case SYMBOL
					Return currencyNameProvider.getSymbol(key, locale_Renamed)
				Case DISPLAYNAME
					Return currencyNameProvider.getDisplayName(key, locale_Renamed)
				Case Else
					Debug.Assert(False) ' shouldn't happen
				End Select

				Return Nothing
			End Function
		End Class

		Private Shared Function readIntArray(ByVal dis As java.io.DataInputStream, ByVal count As Integer) As Integer()
			Dim ret As Integer() = New Integer(count - 1){}
			For i As Integer = 0 To count - 1
				ret(i) = dis.readInt()
			Next i

			Return ret
		End Function

		Private Shared Function readLongArray(ByVal dis As java.io.DataInputStream, ByVal count As Integer) As Long()
			Dim ret As Long() = New Long(count - 1){}
			For i As Integer = 0 To count - 1
				ret(i) = dis.readLong()
			Next i

			Return ret
		End Function

		Private Shared Function readStringArray(ByVal dis As java.io.DataInputStream, ByVal count As Integer) As String()
			Dim ret As String() = New String(count - 1){}
			For i As Integer = 0 To count - 1
				ret(i) = dis.readUTF()
			Next i

			Return ret
		End Function

		''' <summary>
		''' Replaces currency data found in the currencydata.properties file
		''' </summary>
		''' <param name="pattern"> regex pattern for the properties </param>
		''' <param name="ctry"> country code </param>
		''' <param name="curdata"> currency data.  This is a comma separated string that
		'''    consists of "three-letter alphabet code", "three-digit numeric code",
		'''    and "one-digit (0-9) default fraction digit".
		'''    For example, "JPZ,392,0".
		'''    An optional UTC date can be appended to the string (comma separated)
		'''    to allow a currency change take effect after date specified.
		'''    For example, "JP=JPZ,999,0,2014-01-01T00:00:00" has no effect unless
		'''    UTC time is past 1st January 2014 00:00:00 GMT. </param>
		Private Shared Sub replaceCurrencyData(ByVal pattern As java.util.regex.Pattern, ByVal ctry As String, ByVal curdata As String)

			If ctry.length() <> 2 Then
				' ignore invalid country code
				info("currency.properties entry for " & ctry & " is ignored because of the invalid country code.", Nothing)
				Return
			End If

			Dim m As java.util.regex.Matcher = pattern.matcher(curdata)
			If (Not m.find()) OrElse (m.group(4) Is Nothing AndAlso countOccurrences(curdata, ","c) >= 3) Then
				' format is not recognized.  ignore the data
				' if group(4) date string is null and we've 4 values, bad date value
				info("currency.properties entry for " & ctry & " ignored because the value format is not recognized.", Nothing)
				Return
			End If

			Try
				If m.group(4) IsNot Nothing AndAlso (Not isPastCutoverDate(m.group(4))) Then
					info("currency.properties entry for " & ctry & " ignored since cutover date has not passed :" & curdata, Nothing)
					Return
				End If
			Catch ex As java.text.ParseException
				info("currency.properties entry for " & ctry & " ignored since exception encountered :" & ex.Message, Nothing)
				Return
			End Try

			Dim code As String = m.group(1)
			Dim numeric As Integer = Convert.ToInt32(m.group(2))
			Dim entry As Integer = numeric << NUMERIC_CODE_SHIFT
			Dim fraction As Integer = Convert.ToInt32(m.group(3))
			If fraction > SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS Then
				info("currency.properties entry for " & ctry & " ignored since the fraction is more than " & SIMPLE_CASE_COUNTRY_MAX_DEFAULT_DIGITS & ":" & curdata, Nothing)
				Return
			End If

			Dim index As Integer
			For index = 0 To scOldCurrencies.Length - 1
				If scOldCurrencies(index).Equals(code) Then Exit For
			Next index

			If index = scOldCurrencies.Length Then
				' simple case
				entry = entry Or (fraction << SIMPLE_CASE_COUNTRY_DEFAULT_DIGITS_SHIFT) Or (AscW(code.Chars(2)) - AscW("A"c))
			Else
				' special case
				entry = entry Or SPECIAL_CASE_COUNTRY_MASK Or (index + SPECIAL_CASE_COUNTRY_INDEX_DELTA)
			End If
			mainTableEntrytry(ctry.Chars(0), ctry.Chars(1), entry)
		End Sub

		Private Shared Function isPastCutoverDate(ByVal s As String) As Boolean
			Dim format As New java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss", Locale.ROOT)
			format.timeZone = TimeZone.getTimeZone("UTC")
			format.lenient = False
			Dim time As Long = format.parse(s.Trim()).Value.time
			Return System.currentTimeMillis() > time

		End Function

		Private Shared Function countOccurrences(ByVal value As String, ByVal match As Char) As Integer
			Dim count As Integer = 0
			For Each c As Char In value.ToCharArray()
				If c = match Then count += 1
			Next c
			Return count
		End Function

		Private Shared Sub info(ByVal message As String, ByVal t As Throwable)
			Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.util.Currency")
			If logger.isLoggable(sun.util.logging.PlatformLogger.Level.INFO) Then
				If t IsNot Nothing Then
					logger.info(message, t)
				Else
					logger.info(message)
				End If
			End If
		End Sub
	End Class

End Namespace