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
	''' <seealso cref="java.text.BreakIterator BreakIterator"/> class.
	''' 
	''' @since        1.6
	''' </summary>
	Public MustInherit Class BreakIteratorProvider
		Inherits java.util.spi.LocaleServiceProvider

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="../BreakIterator.html#word">word breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for word breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.text.BreakIterator#getWordInstance(java.util.Locale) </seealso>
		Public MustOverride Function getWordInstance(  locale As java.util.Locale) As java.text.BreakIterator

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="../BreakIterator.html#line">line breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for line breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.text.BreakIterator#getLineInstance(java.util.Locale) </seealso>
		Public MustOverride Function getLineInstance(  locale As java.util.Locale) As java.text.BreakIterator

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="../BreakIterator.html#character">character breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for character breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.text.BreakIterator#getCharacterInstance(java.util.Locale) </seealso>
		Public MustOverride Function getCharacterInstance(  locale As java.util.Locale) As java.text.BreakIterator

		''' <summary>
		''' Returns a new <code>BreakIterator</code> instance
		''' for <a href="../BreakIterator.html#sentence">sentence breaks</a>
		''' for the given locale. </summary>
		''' <param name="locale"> the desired locale </param>
		''' <returns> A break iterator for sentence breaks </returns>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <seealso cref= java.text.BreakIterator#getSentenceInstance(java.util.Locale) </seealso>
		Public MustOverride Function getSentenceInstance(  locale As java.util.Locale) As java.text.BreakIterator
	End Class

End Namespace