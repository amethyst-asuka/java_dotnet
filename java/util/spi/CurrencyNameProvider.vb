'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.spi


	''' <summary>
	''' An abstract class for service providers that
	''' provide localized currency symbols and display names for the
	''' <seealso cref="java.util.Currency Currency"/> class.
	''' Note that currency symbols are considered names when determining
	''' behaviors described in the
	''' <seealso cref="java.util.spi.LocaleServiceProvider LocaleServiceProvider"/>
	''' specification.
	''' 
	''' @since        1.6
	''' </summary>
	Public MustInherit Class CurrencyNameProvider
		Inherits LocaleServiceProvider

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Gets the symbol of the given currency code for the specified locale.
		''' For example, for "USD" (US Dollar), the symbol is "$" if the specified
		''' locale is the US, while for other locales it may be "US$". If no
		''' symbol can be determined, null should be returned.
		''' </summary>
		''' <param name="currencyCode"> the ISO 4217 currency code, which
		'''     consists of three upper-case letters between 'A' (U+0041) and
		'''     'Z' (U+005A) </param>
		''' <param name="locale"> the desired locale </param>
		''' <returns> the symbol of the given currency code for the specified locale, or null if
		'''     the symbol is not available for the locale </returns>
		''' <exception cref="NullPointerException"> if <code>currencyCode</code> or
		'''     <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not in
		'''     the form of three upper-case letters, or <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.util.Currency#getSymbol(java.util.Locale) </seealso>
		Public MustOverride Function getSymbol(  currencyCode As String,   locale As java.util.Locale) As String

		''' <summary>
		''' Returns a name for the currency that is appropriate for display to the
		''' user.  The default implementation returns null.
		''' </summary>
		''' <param name="currencyCode"> the ISO 4217 currency code, which
		'''     consists of three upper-case letters between 'A' (U+0041) and
		'''     'Z' (U+005A) </param>
		''' <param name="locale"> the desired locale </param>
		''' <returns> the name for the currency that is appropriate for display to the
		'''     user, or null if the name is not available for the locale </returns>
		''' <exception cref="IllegalArgumentException"> if <code>currencyCode</code> is not in
		'''     the form of three upper-case letters, or <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <exception cref="NullPointerException"> if <code>currencyCode</code> or
		'''     <code>locale</code> is <code>null</code>
		''' @since 1.7 </exception>
		Public Overridable Function getDisplayName(  currencyCode As String,   locale_Renamed As java.util.Locale) As String
			If currencyCode Is Nothing OrElse locale_Renamed Is Nothing Then Throw New NullPointerException

			' Check whether the currencyCode is valid
			Dim charray As Char() = currencyCode.ToCharArray()
			If charray.Length <> 3 Then Throw New IllegalArgumentException("The currencyCode is not in the form of three upper-case letters.")
			For Each c As Char In charray
				If c < "A"c OrElse c > "Z"c Then Throw New IllegalArgumentException("The currencyCode is not in the form of three upper-case letters.")
			Next c

			' Check whether the locale is valid
			Dim c As java.util.ResourceBundle.Control = java.util.ResourceBundle.Control.getNoFallbackControl(java.util.ResourceBundle.Control.FORMAT_DEFAULT)
			For Each l As java.util.Locale In availableLocales
				If c.getCandidateLocales("", l).contains(locale_Renamed) Then Return Nothing
			Next l

			Throw New IllegalArgumentException("The locale is not available")
		End Function
	End Class

End Namespace