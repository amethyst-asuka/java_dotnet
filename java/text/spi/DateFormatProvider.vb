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
	''' <seealso cref="java.text.DateFormat DateFormat"/> class.
	''' 
	''' @since        1.6
	''' </summary>
	Public MustInherit Class DateFormatProvider
		Inherits java.util.spi.LocaleServiceProvider

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a new <code>DateFormat</code> instance which formats time
		''' with the given formatting style for the specified locale. </summary>
		''' <param name="style"> the given formatting style.  Either one of
		'''     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		'''     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		'''     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		'''     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		''' <param name="locale"> the desired locale. </param>
		''' <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		'''     or if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <returns> a time formatter. </returns>
		''' <seealso cref= java.text.DateFormat#getTimeInstance(int, java.util.Locale) </seealso>
		Public MustOverride Function getTimeInstance(  style As Integer,   locale As java.util.Locale) As java.text.DateFormat

		''' <summary>
		''' Returns a new <code>DateFormat</code> instance which formats date
		''' with the given formatting style for the specified locale. </summary>
		''' <param name="style"> the given formatting style.  Either one of
		'''     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		'''     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		'''     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		'''     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		''' <param name="locale"> the desired locale. </param>
		''' <exception cref="IllegalArgumentException"> if <code>style</code> is invalid,
		'''     or if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <returns> a date formatter. </returns>
		''' <seealso cref= java.text.DateFormat#getDateInstance(int, java.util.Locale) </seealso>
		Public MustOverride Function getDateInstance(  style As Integer,   locale As java.util.Locale) As java.text.DateFormat

		''' <summary>
		''' Returns a new <code>DateFormat</code> instance which formats date and time
		''' with the given formatting style for the specified locale. </summary>
		''' <param name="dateStyle"> the given date formatting style.  Either one of
		'''     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		'''     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		'''     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		'''     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		''' <param name="timeStyle"> the given time formatting style.  Either one of
		'''     <seealso cref="java.text.DateFormat#SHORT DateFormat.SHORT"/>,
		'''     <seealso cref="java.text.DateFormat#MEDIUM DateFormat.MEDIUM"/>,
		'''     <seealso cref="java.text.DateFormat#LONG DateFormat.LONG"/>, or
		'''     <seealso cref="java.text.DateFormat#FULL DateFormat.FULL"/>. </param>
		''' <param name="locale"> the desired locale. </param>
		''' <exception cref="IllegalArgumentException"> if <code>dateStyle</code> or
		'''     <code>timeStyle</code> is invalid,
		'''     or if <code>locale</code> isn't
		'''     one of the locales returned from
		'''     {@link java.util.spi.LocaleServiceProvider#getAvailableLocales()
		'''     getAvailableLocales()}. </exception>
		''' <exception cref="NullPointerException"> if <code>locale</code> is null </exception>
		''' <returns> a date/time formatter. </returns>
		''' <seealso cref= java.text.DateFormat#getDateTimeInstance(int, int, java.util.Locale) </seealso>
		Public MustOverride Function getDateTimeInstance(  dateStyle As Integer,   timeStyle As Integer,   locale As java.util.Locale) As java.text.DateFormat
	End Class

End Namespace