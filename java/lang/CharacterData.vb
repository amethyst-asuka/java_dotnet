'
' * Copyright (c) 2006, 2011, Oracle and/or its affiliates. All rights reserved.
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

	Friend MustInherit Class CharacterData
		Friend MustOverride Function getProperties(  ch As Integer) As Integer
		Friend MustOverride Function [getType](  ch As Integer) As Integer
		Friend MustOverride Function isWhitespace(  ch As Integer) As Boolean
		Friend MustOverride Function isMirrored(  ch As Integer) As Boolean
		Friend MustOverride Function isJavaIdentifierStart(  ch As Integer) As Boolean
		Friend MustOverride Function isJavaIdentifierPart(  ch As Integer) As Boolean
		Friend MustOverride Function isUnicodeIdentifierStart(  ch As Integer) As Boolean
		Friend MustOverride Function isUnicodeIdentifierPart(  ch As Integer) As Boolean
		Friend MustOverride Function isIdentifierIgnorable(  ch As Integer) As Boolean
		Friend MustOverride Function toLowerCase(  ch As Integer) As Integer
		Friend MustOverride Function toUpperCase(  ch As Integer) As Integer
		Friend MustOverride Function toTitleCase(  ch As Integer) As Integer
		Friend MustOverride Function digit(  ch As Integer,   radix As Integer) As Integer
		Friend MustOverride Function getNumericValue(  ch As Integer) As Integer
		Friend MustOverride Function getDirectionality(  ch As Integer) As SByte

		'need to implement for JSR204
		Friend Overridable Function toUpperCaseEx(  ch As Integer) As Integer
			Return ToUpper(ch)
		End Function

		Friend Overridable Function toUpperCaseCharArray(  ch As Integer) As Char()
			Return Nothing
		End Function

		Friend Overridable Function isOtherLowercase(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overridable Function isOtherUppercase(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overridable Function isOtherAlphabetic(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overridable Function isIdeographic(  ch As Integer) As Boolean
			Return False
		End Function

		' Character <= 0xff (basic latin) is handled by internal fast-path
		' to avoid initializing large tables.
		' Note: performance of this "fast-path" code may be sub-optimal
		' in negative cases for some accessors due to complicated ranges.
		' Should revisit after optimization of table initialization.

		Shared Function [of](  ch As Integer) As CharacterData
			If CInt(CUInt(ch) >> 8 = 0) Then ' fast-path
				Return CharacterDataLatin1.instance
			Else
				Select Case CInt(CUInt(ch) >> 16) 'plane 00-16
				Case(0)
					Return CharacterData00.instance
				Case(1)
					Return CharacterData01.instance
				Case(2)
					Return CharacterData02.instance
				Case(14)
					Return CharacterData0E.instance
				Case(15), (16) ' Private Use
					Return CharacterDataPrivateUse.instance
				Case Else
					Return CharacterDataUndefined.instance
				End Select
			End If
		End Function
	End Class

End Namespace