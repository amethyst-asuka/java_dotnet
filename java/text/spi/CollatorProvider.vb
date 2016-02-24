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
	''' provide concrete implementations of the
	''' <seealso cref="java.text.Collator Collator"/> class.
	''' 
	''' @since        1.6
	''' </summary>
	Public MustInherit Class CollatorProvider
		Inherits java.util.spi.LocaleServiceProvider

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a new <code>Collator</code> instance for the specified locale. </summary>
		''' <param name="locale"> the desired locale. </param>
		''' <returns> the <code>Collator</code> for the desired locale. </returns>
		''' <exception cref="NullPointerException"> if
		''' <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.text.Collator#getInstance(java.util.Locale) </seealso>
		Public MustOverride Function getInstance(ByVal locale As java.util.Locale) As java.text.Collator
	End Class

End Namespace