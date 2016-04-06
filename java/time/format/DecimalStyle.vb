Imports System.Collections.Generic
Imports System.Collections.Concurrent

'
' * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
' *
' *
' *
' *
' *
' * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights reserved.
' *
' * Redistribution and use in source and binary forms, with or without
' * modification, are permitted provided that the following conditions are met:
' *
' *  * Redistributions of source code must retain the above copyright notice,
' *    this list of conditions and the following disclaimer.
' *
' *  * Redistributions in binary form must reproduce the above copyright notice,
' *    this list of conditions and the following disclaimer in the documentation
' *    and/or other materials provided with the distribution.
' *
' *  * Neither the name of JSR-310 nor the names of its contributors
' *    may be used to endorse or promote products derived from this software
' *    without specific prior written permission.
' *
' * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
' * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
' * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
' * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
' * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
' * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
' * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
' * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
' * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
' * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
' 
Namespace java.time.format


	''' <summary>
	''' Localized decimal style used in date and time formatting.
	''' <p>
	''' A significant part of dealing with dates and times is the localization.
	''' This class acts as a central point for accessing the information.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	Public NotInheritable Class DecimalStyle

		''' <summary>
		''' The standard set of non-localized decimal style symbols.
		''' <p>
		''' This uses standard ASCII characters for zero, positive, negative and a dot for the decimal point.
		''' </summary>
		Public Shared ReadOnly STANDARD As New DecimalStyle("0"c, "+"c, "-"c, "."c)
		''' <summary>
		''' The cache of DecimalStyle instances.
		''' </summary>
		Private Shared ReadOnly CACHE As java.util.concurrent.ConcurrentMap(Of java.util.Locale, DecimalStyle) = New ConcurrentDictionary(Of java.util.Locale, DecimalStyle)(16, 0.75f, 2)

		''' <summary>
		''' The zero digit.
		''' </summary>
		Private ReadOnly zeroDigit As Char
		''' <summary>
		''' The positive sign.
		''' </summary>
		Private ReadOnly positiveSign As Char
		''' <summary>
		''' The negative sign.
		''' </summary>
		Private ReadOnly negativeSign As Char
		''' <summary>
		''' The decimal separator.
		''' </summary>
		Private ReadOnly decimalSeparator As Char

		'-----------------------------------------------------------------------
		''' <summary>
		''' Lists all the locales that are supported.
		''' <p>
		''' The locale 'en_US' will always be present.
		''' </summary>
		''' <returns> a Set of Locales for which localization is supported </returns>
		PublicShared ReadOnly PropertyavailableLocales As java.util.Set(Of java.util.Locale)
			Get
				Dim l As java.util.Locale() = java.text.DecimalFormatSymbols.availableLocales
				Dim locales As java.util.Set(Of java.util.Locale) = New HashSet(Of java.util.Locale)(l.Length)
				java.util.Collections.addAll(locales, l)
				Return locales
			End Get
		End Property

		''' <summary>
		''' Obtains the DecimalStyle for the default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale.
		''' <p>
		''' This method provides access to locale sensitive decimal style symbols.
		''' <p>
		''' This is equivalent to calling
		''' {@link #of(Locale)
		'''     of(Locale.getDefault(Locale.Category.FORMAT))}.
		''' </summary>
		''' <seealso cref= java.util.Locale.Category#FORMAT </seealso>
		''' <returns> the decimal style, not null </returns>
		Public Shared Function ofDefaultLocale() As DecimalStyle
			Return [of](java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Function

		''' <summary>
		''' Obtains the DecimalStyle for the specified locale.
		''' <p>
		''' This method provides access to locale sensitive decimal style symbols.
		''' </summary>
		''' <param name="locale">  the locale, not null </param>
		''' <returns> the decimal style, not null </returns>
		Public Shared Function [of](  locale As java.util.Locale) As DecimalStyle
			java.util.Objects.requireNonNull(locale, "locale")
			Dim info As DecimalStyle = CACHE.get(locale)
			If info Is Nothing Then
				info = create(locale)
				CACHE.putIfAbsent(locale, info)
				info = CACHE.get(locale)
			End If
			Return info
		End Function

		Private Shared Function create(  locale As java.util.Locale) As DecimalStyle
			Dim oldSymbols As java.text.DecimalFormatSymbols = java.text.DecimalFormatSymbols.getInstance(locale)
			Dim zeroDigit_Renamed As Char = oldSymbols.zeroDigit
			Dim positiveSign_Renamed As Char = "+"c
			Dim negativeSign_Renamed As Char = oldSymbols.minusSign
			Dim decimalSeparator_Renamed As Char = oldSymbols.decimalSeparator
			If zeroDigit_Renamed = "0"c AndAlso negativeSign_Renamed = "-"c AndAlso decimalSeparator_Renamed = "."c Then Return STANDARD
			Return New DecimalStyle(zeroDigit_Renamed, positiveSign_Renamed, negativeSign_Renamed, decimalSeparator_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Restricted constructor.
		''' </summary>
		''' <param name="zeroChar">  the character to use for the digit of zero </param>
		''' <param name="positiveSignChar">  the character to use for the positive sign </param>
		''' <param name="negativeSignChar">  the character to use for the negative sign </param>
		''' <param name="decimalPointChar">  the character to use for the decimal point </param>
		Private Sub New(  zeroChar As Char,   positiveSignChar As Char,   negativeSignChar As Char,   decimalPointChar As Char)
			Me.zeroDigit = zeroChar
			Me.positiveSign = positiveSignChar
			Me.negativeSign = negativeSignChar
			Me.decimalSeparator = decimalPointChar
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the character that represents zero.
		''' <p>
		''' The character used to represent digits may vary by culture.
		''' This method specifies the zero character to use, which implies the characters for one to nine.
		''' </summary>
		''' <returns> the character for zero </returns>
		Public Property zeroDigit As Char
			Get
				Return zeroDigit
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the info with a new character that represents zero.
		''' <p>
		''' The character used to represent digits may vary by culture.
		''' This method specifies the zero character to use, which implies the characters for one to nine.
		''' </summary>
		''' <param name="zeroDigit">  the character for zero </param>
		''' <returns>  a copy with a new character that represents zero, not null
		'''  </returns>
		Public Function withZeroDigit(  zeroDigit As Char) As DecimalStyle
			If zeroDigit = Me.zeroDigit Then Return Me
			Return New DecimalStyle(zeroDigit, positiveSign, negativeSign, decimalSeparator)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the character that represents the positive sign.
		''' <p>
		''' The character used to represent a positive number may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <returns> the character for the positive sign </returns>
		Public Property positiveSign As Char
			Get
				Return positiveSign
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the info with a new character that represents the positive sign.
		''' <p>
		''' The character used to represent a positive number may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <param name="positiveSign">  the character for the positive sign </param>
		''' <returns>  a copy with a new character that represents the positive sign, not null </returns>
		Public Function withPositiveSign(  positiveSign As Char) As DecimalStyle
			If positiveSign = Me.positiveSign Then Return Me
			Return New DecimalStyle(zeroDigit, positiveSign, negativeSign, decimalSeparator)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the character that represents the negative sign.
		''' <p>
		''' The character used to represent a negative number may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <returns> the character for the negative sign </returns>
		Public Property negativeSign As Char
			Get
				Return negativeSign
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the info with a new character that represents the negative sign.
		''' <p>
		''' The character used to represent a negative number may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <param name="negativeSign">  the character for the negative sign </param>
		''' <returns>  a copy with a new character that represents the negative sign, not null </returns>
		Public Function withNegativeSign(  negativeSign As Char) As DecimalStyle
			If negativeSign = Me.negativeSign Then Return Me
			Return New DecimalStyle(zeroDigit, positiveSign, negativeSign, decimalSeparator)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the character that represents the decimal point.
		''' <p>
		''' The character used to represent a decimal point may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <returns> the character for the decimal point </returns>
		Public Property decimalSeparator As Char
			Get
				Return decimalSeparator
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the info with a new character that represents the decimal point.
		''' <p>
		''' The character used to represent a decimal point may vary by culture.
		''' This method specifies the character to use.
		''' </summary>
		''' <param name="decimalSeparator">  the character for the decimal point </param>
		''' <returns>  a copy with a new character that represents the decimal point, not null </returns>
		Public Function withDecimalSeparator(  decimalSeparator As Char) As DecimalStyle
			If decimalSeparator = Me.decimalSeparator Then Return Me
			Return New DecimalStyle(zeroDigit, positiveSign, negativeSign, decimalSeparator)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks whether the character is a digit, based on the currently set zero character.
		''' </summary>
		''' <param name="ch">  the character to check </param>
		''' <returns> the value, 0 to 9, of the character, or -1 if not a digit </returns>
		Friend Function convertToDigit(  ch As Char) As Integer
			Dim val As Integer = AscW(ch) - AscW(zeroDigit)
			Return If(val >= 0 AndAlso val <= 9, val, -1)
		End Function

		''' <summary>
		''' Converts the input numeric text to the internationalized form using the zero character.
		''' </summary>
		''' <param name="numericText">  the text, consisting of digits 0 to 9, to convert, not null </param>
		''' <returns> the internationalized text, not null </returns>
		Friend Function convertNumberToI18N(  numericText As String) As String
			If zeroDigit = "0"c Then Return numericText
			Dim diff As Integer = AscW(zeroDigit) - AscW("0"c)
			Dim array As Char() = numericText.ToCharArray()
			For i As Integer = 0 To array.Length - 1
				array(i) = CChar(AscW(array(i)) + diff)
			Next i
			Return New String(array)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this DecimalStyle is equal to another DecimalStyle.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other date </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is DecimalStyle Then
				Dim other As DecimalStyle = CType(obj, DecimalStyle)
				Return (zeroDigit = other.zeroDigit AndAlso positiveSign = other.positiveSign AndAlso negativeSign = other.negativeSign AndAlso decimalSeparator = other.decimalSeparator)
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this DecimalStyle.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return AscW(zeroDigit) + AscW(positiveSign) + AscW(negativeSign) + AscW(decimalSeparator)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a string describing this DecimalStyle.
		''' </summary>
		''' <returns> a string description, not null </returns>
		Public Overrides Function ToString() As String
			Return "DecimalStyle[" & AscW(zeroDigit) + AscW(positiveSign) + AscW(negativeSign) + AscW(decimalSeparator) & "]"
		End Function

	End Class

End Namespace