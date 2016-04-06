' This file was generated AUTOMATICALLY from a template file Fri Jan 29 17:43:04 PST 2016
'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The CharacterData class encapsulates the large tables found in
	'''    Java.lang.Character. 
	''' </summary>

	Friend Class CharacterDataLatin1
		Inherits CharacterData

	'     The character properties are currently encoded into 32 bits in the following manner:
	'        1 bit   mirrored property
	'        4 bits  directionality property
	'        9 bits  signed offset used for converting case
	'        1 bit   if 1, adding the signed offset converts the character to lowercase
	'        1 bit   if 1, subtracting the signed offset converts the character to uppercase
	'        1 bit   if 1, this character has a titlecase equivalent (possibly itself)
	'        3 bits  0  may not be part of an identifier
	'                1  ignorable control; may continue a Unicode identifier or Java identifier
	'                2  may continue a Java identifier but not a Unicode identifier (unused)
	'                3  may continue a Unicode identifier or Java identifier
	'                4  is a Java whitespace character
	'                5  may start or continue a Java identifier;
	'                   may continue but not start a Unicode identifier (underscores)
	'                6  may start or continue a Java identifier but not a Unicode identifier ($)
	'                7  may start or continue a Unicode identifier or Java identifier
	'                Thus:
	'                   5, 6, 7 may start a Java identifier
	'                   1, 2, 3, 5, 6, 7 may continue a Java identifier
	'                   7 may start a Unicode identifier
	'                   1, 3, 5, 7 may continue a Unicode identifier
	'                   1 is ignorable within an identifier
	'                   4 is Java whitespace
	'        2 bits  0  this character has no numeric property
	'                1  adding the digit offset to the character code and then
	'                   masking with 0x1F will produce the desired numeric value
	'                2  this character has a "strange" numeric value
	'                3  a Java supradecimal digit: adding the digit offset to the
	'                   character code, then masking with 0x1F, then adding 10
	'                   will produce the desired numeric value
	'        5 bits  digit offset
	'        5 bits  character type
	'
	'        The encoding of character properties is subject to change at any time.
	'     

		Friend Overrides Function getProperties(  ch As Integer) As Integer
			Dim offset As Char = ChrW(ch)
			Dim props As Integer = A(AscW(offset))
			Return props
		End Function

		Friend Overridable Function getPropertiesEx(  ch As Integer) As Integer
			Dim offset As Char = ChrW(ch)
			Dim props As Integer = AscW(B(AscW(offset)))
			Return props
		End Function

		Friend Overrides Function isOtherLowercase(  ch As Integer) As Boolean
			Dim props As Integer = getPropertiesEx(ch)
			Return (props And &H1) <> 0
		End Function

		Friend Overrides Function isOtherUppercase(  ch As Integer) As Boolean
			Dim props As Integer = getPropertiesEx(ch)
			Return (props And &H2) <> 0
		End Function

		Friend Overrides Function isOtherAlphabetic(  ch As Integer) As Boolean
			Dim props As Integer = getPropertiesEx(ch)
			Return (props And &H4) <> 0
		End Function

		Friend Overrides Function isIdeographic(  ch As Integer) As Boolean
			Dim props As Integer = getPropertiesEx(ch)
			Return (props And &H10) <> 0
		End Function

		Friend Overrides Function [getType](  ch As Integer) As Integer
			Dim props As Integer = getProperties(ch)
			Return (props And &H1F)
		End Function

		Friend Overrides Function isJavaIdentifierStart(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H7000) >= &H5000)
		End Function

		Friend Overrides Function isJavaIdentifierPart(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H3000) <> 0)
		End Function

		Friend Overrides Function isUnicodeIdentifierStart(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H7000) = &H7000)
		End Function

		Friend Overrides Function isUnicodeIdentifierPart(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H1000) <> 0)
		End Function

		Friend Overrides Function isIdentifierIgnorable(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H7000) = &H1000)
		End Function

		Friend Overrides Function toLowerCase(  ch As Integer) As Integer
			Dim mapChar As Integer = ch
			Dim val As Integer = getProperties(ch)

			If ((val And &H20000) <> 0) AndAlso ((val And &H7FC0000) <> &H7FC0000) Then
				Dim offset As Integer = val << 5 >> (5+18)
				mapChar = ch + offset
			End If
			Return mapChar
		End Function

		Friend Overrides Function toUpperCase(  ch As Integer) As Integer
			Dim mapChar As Integer = ch
			Dim val As Integer = getProperties(ch)

			If (val And &H10000) <> 0 Then
				If (val And &H7FC0000) <> &H7FC0000 Then
					Dim offset As Integer = val << 5 >> (5+18)
					mapChar = ch - offset
				ElseIf ch = &HB5 Then
					mapChar = &H39C
				End If
			End If
			Return mapChar
		End Function

		Friend Overrides Function toTitleCase(  ch As Integer) As Integer
			Return ToUpper(ch)
		End Function

		Friend Overrides Function digit(  ch As Integer,   radix As Integer) As Integer
			Dim value As Integer = -1
			If radix >= Character.MIN_RADIX AndAlso radix <= Character.MAX_RADIX Then
				Dim val As Integer = getProperties(ch)
				Dim kind As Integer = val And &H1F
				If kind = Character.DECIMAL_DIGIT_NUMBER Then
					value = ch + ((val And &H3E0) >> 5) And &H1F
				ElseIf (val And &HC00) = &HC00 Then
					' Java supradecimal digit
					value = (ch + ((val And &H3E0) >> 5) And &H1F) + 10
				End If
			End If
			Return If(value < radix, value, -1)
		End Function

		Friend Overrides Function getNumericValue(  ch As Integer) As Integer
			Dim val As Integer = getProperties(ch)
			Dim retval As Integer = -1

			Select Case val And &HC00
				Case Else ' cannot occur
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case (&H0) ' not numeric
					retval = -1
				Case (&H400) ' simple numeric
					retval = ch + ((val And &H3E0) >> 5) And &H1F
				Case (&H800) ' "strange" numeric
					 retval = -2
				Case (&HC00) ' Java supradecimal
					retval = (ch + ((val And &H3E0) >> 5) And &H1F) + 10
			End Select
			Return retval
		End Function

		Friend Overrides Function isWhitespace(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H7000) = &H4000)
		End Function

		Friend Overrides Function getDirectionality(  ch As Integer) As SByte
			Dim val As Integer = getProperties(ch)
			Dim directionality_Renamed As SByte = CByte((val And &H78000000) >> 27)

			If directionality_Renamed = &HF Then directionality_Renamed = -1
			Return directionality_Renamed
		End Function

		Friend Overrides Function isMirrored(  ch As Integer) As Boolean
			Dim props As Integer = getProperties(ch)
			Return ((props And &H80000000L) <> 0)
		End Function

		Friend Overrides Function toUpperCaseEx(  ch As Integer) As Integer
			Dim mapChar As Integer = ch
			Dim val As Integer = getProperties(ch)

			If (val And &H10000) <> 0 Then
				If (val And &H7FC0000) <> &H7FC0000 Then
					Dim offset As Integer = val << 5 >> (5+18)
					mapChar = ch - offset
				Else
					Select Case ch
						' map overflow characters
						Case &HB5
							mapChar = &H39C
						Case Else
							mapChar = Character.ERROR
					End Select
				End If
			End If
			Return mapChar
		End Function

		Friend Shared sharpsMap As Char() = {"S"c, "S"c}

		Friend Overrides Function toUpperCaseCharArray(  ch As Integer) As Char()
			Dim upperMap As Char() = {ChrW(ch)}
			If ch = &HDF Then upperMap = sharpsMap
			Return upperMap
		End Function

		Friend Shared ReadOnly instance As New CharacterDataLatin1
		Private Sub New()
		End Sub

		' The following tables and code generated using:
	  ' java GenerateCharacter -template d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/characterdata/CharacterDataLatin1.java.template -spec d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/UnicodeData.txt -specialcasing d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/SpecialCasing.txt -proplist d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/jdk/make/data/unicodedata/PropList.txt -o d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/build/windows-amd64/jdk/gensrc/java/lang/CharacterDataLatin1.java -string -usecharforbyte -latin1 8
	  ' The A table has 256 entries for a total of 1024 bytes.

	  Friend Shared ReadOnly A As Integer() = New Integer(255){}
	  Friend Shared ReadOnly A_DATA As String = ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H5800).ToString() & ChrW(&H400F).ToString() & ChrW(&H5000).ToString() & ChrW(&H400F).ToString() & ChrW(&H5800).ToString() & ChrW(&H400F).ToString() & ChrW(&H6000).ToString() & ChrW(&H400F).ToString() & ChrW(&H5000).ToString() & ChrW(&H400F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H5000).ToString() & ChrW(&H400F).ToString() & ChrW(&H5000).ToString() & ChrW(&H400F).ToString() & ChrW(&H5000).ToString() & ChrW(&H400F).ToString() & ChrW(&H5800).ToString() & ChrW(&H400F).ToString() & ChrW(&H6000).ToString() & ChrW(&H400C).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H2800).ToString() & ChrW(&H30).ToString() & ChrW(&H2800).ToString() & ChrW(&H601A).ToString() & ChrW(&H2800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&HE800).ToString() & ChrW(&H25).ToString() & ChrW(&HE800).ToString() & ChrW(&H26).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H2000).ToString() & ChrW(&H31).ToString() & ChrW(&H3800).ToString() & ChrW(&H30).ToString() & ChrW(&H2000).ToString() & ChrW(&H24).ToString() & ChrW(&H3800).ToString() & ChrW(&H30).ToString() & ChrW(&H3800).ToString() & ChrW(&H30).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H1800).ToString() & ChrW(&H3609).ToString() & ChrW(&H3800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&HE800).ToString() & ChrW(&H31).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & ChrW(&HE800).ToString() & ChrW(&H31).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & "\202" & ChrW(&H7FE1).ToString() & ChrW(&HE800).ToString() & ChrW(&H25).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&HE800).ToString() & ChrW(&H26).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & ChrW(&H6800).ToString() & ChrW(&H5017).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & "\201" & ChrW(&H7FE2).ToString() & ChrW(&HE800).ToString() & ChrW(&H25).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & ChrW(&HE800).ToString() & ChrW(&H26).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H5000).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H4800).ToString() & ChrW(&H100F).ToString() & ChrW(&H3800).ToString() & ChrW(&H14).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H2800).ToString() & ChrW(&H601A).ToString() & ChrW(&H2800).ToString() & ChrW(&H601A).ToString() & ChrW(&H2800).ToString() & ChrW(&H601A).ToString() & ChrW(&H2800).ToString() & ChrW(&H601A).ToString() & ChrW(&H6800).ToString() & ChrW(&H34).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & ChrW(&H6800).ToString() & ChrW(&H34).ToString() & ChrW(&H00).ToString() & ChrW(&H7005).ToString() & ChrW(&HE800).ToString() & ChrW(&H35).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & ChrW(&H4800).ToString() & ChrW(&H1010).ToString() & ChrW(&H6800).ToString() & ChrW(&H34).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & ChrW(&H2800).ToString() & ChrW(&H34).ToString() & ChrW(&H2800).ToString() & ChrW(&H31).ToString() & ChrW(&H1800).ToString() & ChrW(&H060B).ToString() & ChrW(&H1800).ToString() & ChrW(&H060B).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & ChrW(&H07FD).ToString() & ChrW(&H7002).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & ChrW(&H6800).ToString() & ChrW(&H33).ToString() & ChrW(&H1800).ToString() & ChrW(&H050B).ToString() & ChrW(&H00).ToString() & ChrW(&H7005).ToString() & ChrW(&HE800).ToString() & ChrW(&H36).ToString() & ChrW(&H6800).ToString() & ChrW(&H080B).ToString() & ChrW(&H6800).ToString() & ChrW(&H080B).ToString() & ChrW(&H6800).ToString() & ChrW(&H080B).ToString() & ChrW(&H6800).ToString() & ChrW(&H30).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & "\202" & ChrW(&H7001).ToString() & ChrW(&H07FD).ToString() & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & ChrW(&H6800).ToString() & ChrW(&H31).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & "\201" & ChrW(&H7002).ToString() & ChrW(&H061D).ToString() & ChrW(&H7002).ToString()

	  ' The B table has 256 entries for a total of 512 bytes.

	  Friend Shared ReadOnly B As Char() = (ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H01).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H01).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString() & ChrW(&H00).ToString()).toCharArray()

	  ' In all, the character property tables require 1024 bytes.

		Shared Sub New()
				Dim data As Char() = A_DATA.ToCharArray()
				assert(data.Length = (256 * 2))
				Dim i As Integer = 0, j As Integer = 0
				Do While i < (256 * 2)
					Dim entry As Integer = AscW(data(i)) << 16
					i += 1
					A(j) = entry Or AscW(data(i))
					i += 1
					j += 1
				Loop

		End Sub
	End Class


End Namespace