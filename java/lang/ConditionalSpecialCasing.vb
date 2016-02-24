Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang



	''' <summary>
	''' This is a utility class for <code>String.toLowerCase()</code> and
	''' <code>String.toUpperCase()</code>, that handles special casing with
	''' conditions.  In other words, it handles the mappings with conditions
	''' that are defined in
	''' <a href="http://www.unicode.org/Public/UNIDATA/SpecialCasing.txt">Special
	''' Casing Properties</a> file.
	''' <p>
	''' Note that the unconditional case mappings (including 1:M mappings)
	''' are handled in <code>Character.toLower/UpperCase()</code>.
	''' </summary>
	Friend NotInheritable Class ConditionalSpecialCasing

		' context conditions.
		Friend Const FINAL_CASED As Integer = 1
		Friend Const AFTER_SOFT_DOTTED As Integer = 2
		Friend Const MORE_ABOVE As Integer = 3
		Friend Const AFTER_I As Integer = 4
		Friend Const NOT_BEFORE_DOT As Integer = 5

		' combining class definitions
		Friend Const COMBINING_CLASS_ABOVE As Integer = 230

		' Special case mapping entries
		Friend Shared entry As Entry() = { New Entry(&H3A3, New Char(){&H3C2}, New Char(){&H3A3}, Nothing, FINAL_CASED), New Entry(&H130, New Char(){&H69, &H307}, New Char(){&H130}, Nothing, 0), New Entry(&H307, New Char(){&H307}, New Char(){}, "lt", AFTER_SOFT_DOTTED), New Entry(&H49, New Char(){&H69, &H307}, New Char(){&H49}, "lt", MORE_ABOVE), New Entry(&H4A, New Char(){&H6A, &H307}, New Char(){&H4A}, "lt", MORE_ABOVE), New Entry(&H12E, New Char(){&H12F, &H307}, New Char(){&H12E}, "lt", MORE_ABOVE), New Entry(&HCC, New Char(){&H69, &H307, &H300}, New Char(){&HCC}, "lt", 0), New Entry(&HCD, New Char(){&H69, &H307, &H301}, New Char(){&HCD}, "lt", 0), New Entry(&H128, New Char(){&H69, &H307, &H303}, New Char(){&H128}, "lt", 0), New Entry(&H130, New Char(){&H69}, New Char(){&H130}, "tr", 0), New Entry(&H130, New Char(){&H69}, New Char(){&H130}, "az", 0), New Entry(&H307, New Char(){}, New Char(){&H307}, "tr", AFTER_I), New Entry(&H307, New Char(){}, New Char(){&H307}, "az", AFTER_I), New Entry(&H49, New Char(){&H131}, New Char(){&H49}, "tr", NOT_BEFORE_DOT), New Entry(&H49, New Char(){&H131}, New Char(){&H49}, "az", NOT_BEFORE_DOT), New Entry(&H69, New Char(){&H69}, New Char(){&H130}, "tr", 0), New Entry(&H69, New Char(){&H69}, New Char(){&H130}, "az", 0) }

		' A hash table that contains the above entries
		Friend Shared entryTable As New Dictionary(Of Integer?, HashSet(Of Entry))
		Shared Sub New()
			' create hashtable from the entry
			For i As Integer = 0 To entry.Length - 1
				Dim cur As Entry = entry(i)
				Dim cp As Integer? = New Integer?(cur.codePoint)
				Dim [set] As HashSet(Of Entry) = entryTable(cp)
				If [set] Is Nothing Then [set] = New HashSet(Of Entry)
				[set].Add(cur)
				entryTable(cp) = [set]
			Next i
		End Sub

		Friend Shared Function toLowerCaseEx(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale) As Integer
			Dim result As Char() = lookUpTable(src, index, locale, True)

			If result IsNot Nothing Then
				If result.Length = 1 Then
					Return result(0)
				Else
					Return Character.ERROR
				End If
			Else
				' default to Character class' one
				Return Char.ToLower(src.codePointAt(index))
			End If
		End Function

		Friend Shared Function toUpperCaseEx(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale) As Integer
			Dim result As Char() = lookUpTable(src, index, locale, False)

			If result IsNot Nothing Then
				If result.Length = 1 Then
					Return result(0)
				Else
					Return Character.ERROR
				End If
			Else
				' default to Character class' one
				Return Character.toUpperCaseEx(src.codePointAt(index))
			End If
		End Function

		Friend Shared Function toLowerCaseCharArray(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale) As Char()
			Return lookUpTable(src, index, locale, True)
		End Function

		Friend Shared Function toUpperCaseCharArray(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale) As Char()
			Dim result As Char() = lookUpTable(src, index, locale, False)
			If result IsNot Nothing Then
				Return result
			Else
				Return Character.toUpperCaseCharArray(src.codePointAt(index))
			End If
		End Function

		Private Shared Function lookUpTable(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale, ByVal bLowerCasing As Boolean) As Char()
			Dim [set] As HashSet(Of Entry) = entryTable(New Integer?(src.codePointAt(index)))
			Dim ret As Char() = Nothing

			If [set] IsNot Nothing Then
				Dim iter As IEnumerator(Of Entry) = [set].GetEnumerator()
				Dim currentLang As String = locale.language
				Do While iter.MoveNext()
					Dim entry As Entry = iter.Current
					Dim conditionLang As String = entry.language
					If ((conditionLang Is Nothing) OrElse (conditionLang.Equals(currentLang))) AndAlso isConditionMet(src, index, locale, entry.condition) Then
						ret = If(bLowerCasing, entry.lowerCase, entry.upperCase)
						If conditionLang IsNot Nothing Then Exit Do
					End If
				Loop
			End If

			Return ret
		End Function

		Private Shared Function isConditionMet(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale, ByVal condition As Integer) As Boolean
			Select Case condition
			Case FINAL_CASED
				Return isFinalCased(src, index, locale)

			Case AFTER_SOFT_DOTTED
				Return isAfterSoftDotted(src, index)

			Case MORE_ABOVE
				Return isMoreAbove(src, index)

			Case AFTER_I
				Return isAfterI(src, index)

			Case NOT_BEFORE_DOT
				Return Not isBeforeDot(src, index)

			Case Else
				Return True
			End Select
		End Function

		''' <summary>
		''' Implements the "Final_Cased" condition
		''' 
		''' Specification: Within the closest word boundaries containing C, there is a cased
		''' letter before C, and there is no cased letter after C.
		''' 
		''' Regular Expression:
		'''   Before C: [{cased==true}][{wordBoundary!=true}]*
		'''   After C: !([{wordBoundary!=true}]*[{cased}])
		''' </summary>
		Private Shared Function isFinalCased(ByVal src As String, ByVal index As Integer, ByVal locale As java.util.Locale) As Boolean
			Dim wordBoundary As java.text.BreakIterator = java.text.BreakIterator.getWordInstance(locale)
			wordBoundary.text = src
			Dim ch As Integer

			' Look for a preceding 'cased' letter
			Dim i As Integer = index
			Do While (i >= 0) AndAlso Not wordBoundary.isBoundary(i)

				ch = src.codePointBefore(i)
				If isCased(ch) Then

					Dim len As Integer = src.length()
					' Check that there is no 'cased' letter after the index
					i = index + Character.charCount(src.codePointAt(index))
					Do While (i < len) AndAlso Not wordBoundary.isBoundary(i)

						ch = src.codePointAt(i)
						If isCased(ch) Then Return False
						i += Character.charCount(ch)
					Loop

					Return True
				End If
				i -= Character.charCount(ch)
			Loop

			Return False
		End Function

		''' <summary>
		''' Implements the "After_I" condition
		''' 
		''' Specification: The last preceding base character was an uppercase I,
		''' and there is no intervening combining character class 230 (ABOVE).
		''' 
		''' Regular Expression:
		'''   Before C: [I]([{cc!=230}&{cc!=0}])*
		''' </summary>
		Private Shared Function isAfterI(ByVal src As String, ByVal index As Integer) As Boolean
			Dim ch As Integer
			Dim cc As Integer

			' Look for the last preceding base character
			For i As Integer = index To 1 Step -Character.charCount(ch)

				ch = src.codePointBefore(i)

				If ch = AscW("I"c) Then
					Return True
				Else
					cc = sun.text.Normalizer.getCombiningClass(ch)
					If (cc = 0) OrElse (cc = COMBINING_CLASS_ABOVE) Then Return False
				End If
			Next i

			Return False
		End Function

		''' <summary>
		''' Implements the "After_Soft_Dotted" condition
		''' 
		''' Specification: The last preceding character with combining class
		''' of zero before C was Soft_Dotted, and there is no intervening
		''' combining character class 230 (ABOVE).
		''' 
		''' Regular Expression:
		'''   Before C: [{Soft_Dotted==true}]([{cc!=230}&{cc!=0}])*
		''' </summary>
		Private Shared Function isAfterSoftDotted(ByVal src As String, ByVal index As Integer) As Boolean
			Dim ch As Integer
			Dim cc As Integer

			' Look for the last preceding character
			For i As Integer = index To 1 Step -Character.charCount(ch)

				ch = src.codePointBefore(i)

				If isSoftDotted(ch) Then
					Return True
				Else
					cc = sun.text.Normalizer.getCombiningClass(ch)
					If (cc = 0) OrElse (cc = COMBINING_CLASS_ABOVE) Then Return False
				End If
			Next i

			Return False
		End Function

		''' <summary>
		''' Implements the "More_Above" condition
		''' 
		''' Specification: C is followed by one or more characters of combining
		''' class 230 (ABOVE) in the combining character sequence.
		''' 
		''' Regular Expression:
		'''   After C: [{cc!=0}]*[{cc==230}]
		''' </summary>
		Private Shared Function isMoreAbove(ByVal src As String, ByVal index As Integer) As Boolean
			Dim ch As Integer
			Dim cc As Integer
			Dim len As Integer = src.length()

			' Look for a following ABOVE combining class character
			For i As Integer = index + Character.charCount(src.codePointAt(index)) To len - 1 Step Character.charCount(ch)

				ch = src.codePointAt(i)
				cc = sun.text.Normalizer.getCombiningClass(ch)

				If cc = COMBINING_CLASS_ABOVE Then
					Return True
				ElseIf cc = 0 Then
					Return False
				End If
			Next i

			Return False
		End Function

		''' <summary>
		''' Implements the "Before_Dot" condition
		''' 
		''' Specification: C is followed by <code>U+0307 COMBINING DOT ABOVE</code>.
		''' Any sequence of characters with a combining class that is
		''' neither 0 nor 230 may intervene between the current character
		''' and the combining dot above.
		''' 
		''' Regular Expression:
		'''   After C: ([{cc!=230}&{cc!=0}])*[\u0307]
		''' </summary>
		Private Shared Function isBeforeDot(ByVal src As String, ByVal index As Integer) As Boolean
			Dim ch As Integer
			Dim cc As Integer
			Dim len As Integer = src.length()

			' Look for a following COMBINING DOT ABOVE
			For i As Integer = index + Character.charCount(src.codePointAt(index)) To len - 1 Step Character.charCount(ch)

				ch = src.codePointAt(i)

				If ch = ChrW(&H0307) Then
					Return True
				Else
					cc = sun.text.Normalizer.getCombiningClass(ch)
					If (cc = 0) OrElse (cc = COMBINING_CLASS_ABOVE) Then Return False
				End If
			Next i

			Return False
		End Function

		''' <summary>
		''' Examines whether a character is 'cased'.
		''' 
		''' A character C is defined to be 'cased' if and only if at least one of
		''' following are true for C: uppercase==true, or lowercase==true, or
		''' general_category==titlecase_letter.
		''' 
		''' The uppercase and lowercase property values are specified in the data
		''' file DerivedCoreProperties.txt in the Unicode Character Database.
		''' </summary>
		Private Shared Function isCased(ByVal ch As Integer) As Boolean
			Dim type As Integer = Character.getType(ch)
			If type = Character.LOWERCASE_LETTER OrElse type = Character.UPPERCASE_LETTER OrElse type = Character.TITLECASE_LETTER Then
				Return True
			Else
				' Check for Other_Lowercase and Other_Uppercase
				'
				If (ch >= &H2B0) AndAlso (ch <= &H2B8) Then
					' MODIFIER LETTER SMALL H..MODIFIER LETTER SMALL Y
					Return True
				ElseIf (ch >= &H2C0) AndAlso (ch <= &H2C1) Then
					' MODIFIER LETTER GLOTTAL STOP..MODIFIER LETTER REVERSED GLOTTAL STOP
					Return True
				ElseIf (ch >= &H2E0) AndAlso (ch <= &H2E4) Then
					' MODIFIER LETTER SMALL GAMMA..MODIFIER LETTER SMALL REVERSED GLOTTAL STOP
					Return True
				ElseIf ch = &H345 Then
					' COMBINING GREEK YPOGEGRAMMENI
					Return True
				ElseIf ch = &H37A Then
					' GREEK YPOGEGRAMMENI
					Return True
				ElseIf (ch >= &H1D2C) AndAlso (ch <= &H1D61) Then
					' MODIFIER LETTER CAPITAL A..MODIFIER LETTER SMALL CHI
					Return True
				ElseIf (ch >= &H2160) AndAlso (ch <= &H217F) Then
					' ROMAN NUMERAL ONE..ROMAN NUMERAL ONE THOUSAND
					' SMALL ROMAN NUMERAL ONE..SMALL ROMAN NUMERAL ONE THOUSAND
					Return True
				ElseIf (ch >= &H24B6) AndAlso (ch <= &H24E9) Then
					' CIRCLED LATIN CAPITAL LETTER A..CIRCLED LATIN CAPITAL LETTER Z
					' CIRCLED LATIN SMALL LETTER A..CIRCLED LATIN SMALL LETTER Z
					Return True
				Else
					Return False
				End If
			End If
		End Function

		Private Shared Function isSoftDotted(ByVal ch As Integer) As Boolean
			Select Case ch
			Case &H69, &H6A, &H12F, &H268, &H456, &H458, &H1D62, &H1E2D, &H1ECB, &H2071 ' Soft_Dotted # L&       LATIN SMALL LETTER I
				Return True
			Case Else
				Return False
			End Select
		End Function

		''' <summary>
		''' An internal class that represents an entry in the Special Casing Properties.
		''' </summary>
		Friend Class Entry
			Friend ch As Integer
			Friend lower As Char()
			Friend upper As Char()
			Friend lang As String
			Friend condition As Integer

			Friend Sub New(ByVal ch As Integer, ByVal lower As Char(), ByVal upper As Char(), ByVal lang As String, ByVal condition As Integer)
				Me.ch = ch
				Me.lower = lower
				Me.upper = upper
				Me.lang = lang
				Me.condition = condition
			End Sub

			Friend Overridable Property codePoint As Integer
				Get
					Return ch
				End Get
			End Property

			Friend Overridable Property lowerCase As Char()
				Get
					Return lower
				End Get
			End Property

			Friend Overridable Property upperCase As Char()
				Get
					Return upper
				End Get
			End Property

			Friend Overridable Property language As String
				Get
					Return lang
				End Get
			End Property

			Friend Overridable Property condition As Integer
				Get
					Return condition
				End Get
			End Property
		End Class
	End Class

End Namespace