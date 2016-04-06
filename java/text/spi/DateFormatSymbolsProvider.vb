'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.text.spi


	''' <summary>
	''' An abstract class for service providers that
	''' provide instances of the
	''' <seealso cref="java.text.DateFormatSymbols DateFormatSymbols"/> class.
	''' 
	''' @since        1.6
	''' </summary>
	Public MustInherit Class DateFormatSymbolsProvider
		Inherits java.util.spi.LocaleServiceProvider

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a new <code>DateFormatSymbols</code> instance for the
		''' specified locale.
		''' </summary>
		''' <param name="locale"> the desired locale </param>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <returns> a <code>DateFormatSymbols</code> instance. </returns>
		''' <seealso cref= java.text.DateFormatSymbols#getInstance(java.util.Locale) </seealso>
		Public MustOverride Function getInstance(  locale As java.util.Locale) As java.text.DateFormatSymbols
	End Class

End Namespace