'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.regex


	''' <summary>
	''' Utility class that implements the standard C ctype functionality.
	''' 
	''' @author Hong Zhang
	''' </summary>

	Friend NotInheritable Class ASCII

		Friend Const UPPER As Integer = &H100

		Friend Const LOWER As Integer = &H200

		Friend Const DIGIT As Integer = &H400

		Friend Const SPACE As Integer = &H800

		Friend Const PUNCT As Integer = &H1000

		Friend Const CNTRL As Integer = &H2000

		Friend Const BLANK As Integer = &H4000

		Friend Const HEX As Integer = &H8000

		Friend Const UNDER As Integer = &H10000

		Friend Const ASCII As Integer = &HFF00

		Friend Shared ReadOnly ALPHA As Integer = (UPPER Or LOWER)

		Friend Shared ReadOnly ALNUM As Integer = (UPPER Or LOWER Or DIGIT)

		Friend Shared ReadOnly GRAPH As Integer = (PUNCT Or UPPER Or LOWER Or DIGIT)

		Friend Shared ReadOnly WORD As Integer = (UPPER Or LOWER Or UNDER Or DIGIT)

		Friend Shared ReadOnly XDIGIT As Integer = (HEX)

		Private Shared ReadOnly [ctype] As Integer() = { CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, SPACE+CNTRL+BLANK, SPACE+CNTRL, SPACE+CNTRL, SPACE+CNTRL, SPACE+CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, CNTRL, SPACE+BLANK, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, DIGIT+HEX+0, DIGIT+HEX+1, DIGIT+HEX+2, DIGIT+HEX+3, DIGIT+HEX+4, DIGIT+HEX+5, DIGIT+HEX+6, DIGIT+HEX+7, DIGIT+HEX+8, DIGIT+HEX+9, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT, UPPER+HEX+10, UPPER+HEX+11, UPPER+HEX+12, UPPER+HEX+13, UPPER+HEX+14, UPPER+HEX+15, UPPER+16, UPPER+17, UPPER+18, UPPER+19, UPPER+20, UPPER+21, UPPER+22, UPPER+23, UPPER+24, UPPER+25, UPPER+26, UPPER+27, UPPER+28, UPPER+29, UPPER+30, UPPER+31, UPPER+32, UPPER+33, UPPER+34, UPPER+35, PUNCT, PUNCT, PUNCT, PUNCT, PUNCT Or UNDER, PUNCT, LOWER+HEX+10, LOWER+HEX+11, LOWER+HEX+12, LOWER+HEX+13, LOWER+HEX+14, LOWER+HEX+15, LOWER+16, LOWER+17, LOWER+18, LOWER+19, LOWER+20, LOWER+21, LOWER+22, LOWER+23, LOWER+24, LOWER+25, LOWER+26, LOWER+27, LOWER+28, LOWER+29, LOWER+30, LOWER+31, LOWER+32, LOWER+33, LOWER+34, LOWER+35, PUNCT, PUNCT, PUNCT, PUNCT, CNTRL }

		Friend Shared Function [getType](ByVal ch As Integer) As Integer
			Return (If((ch And &HFFFFFF80L) = 0, [ctype](ch), 0))
		End Function

		Friend Shared Function isType(ByVal ch As Integer, ByVal type As Integer) As Boolean
			Return ([getType](ch) And type) <> 0
		End Function

		Friend Shared Function isAscii(ByVal ch As Integer) As Boolean
			Return ((ch And &HFFFFFF80L) = 0)
		End Function

		Friend Shared Function isAlpha(ByVal ch As Integer) As Boolean
			Return isType(ch, ALPHA)
		End Function

		Friend Shared Function isDigit(ByVal ch As Integer) As Boolean
			Return ((ch-AscW("0"c)) Or (AscW("9"c)-ch)) >= 0
		End Function

		Friend Shared Function isAlnum(ByVal ch As Integer) As Boolean
			Return isType(ch, ALNUM)
		End Function

		Friend Shared Function isGraph(ByVal ch As Integer) As Boolean
			Return isType(ch, GRAPH)
		End Function

		Friend Shared Function isPrint(ByVal ch As Integer) As Boolean
			Return ((ch-&H20) Or (&H7E-ch)) >= 0
		End Function

		Friend Shared Function isPunct(ByVal ch As Integer) As Boolean
			Return isType(ch, PUNCT)
		End Function

		Friend Shared Function isSpace(ByVal ch As Integer) As Boolean
			Return isType(ch, SPACE)
		End Function

		Friend Shared Function isHexDigit(ByVal ch As Integer) As Boolean
			Return isType(ch, HEX)
		End Function

		Friend Shared Function isOctDigit(ByVal ch As Integer) As Boolean
			Return ((ch-AscW("0"c)) Or (AscW("7"c)-ch)) >= 0
		End Function

		Friend Shared Function isCntrl(ByVal ch As Integer) As Boolean
			Return isType(ch, CNTRL)
		End Function

		Friend Shared Function isLower(ByVal ch As Integer) As Boolean
			Return ((ch-AscW("a"c)) Or (AscW("z"c)-ch)) >= 0
		End Function

		Friend Shared Function isUpper(ByVal ch As Integer) As Boolean
			Return ((ch-AscW("A"c)) Or (AscW("Z"c)-ch)) >= 0
		End Function

		Friend Shared Function isWord(ByVal ch As Integer) As Boolean
			Return isType(ch, WORD)
		End Function

		Friend Shared Function toDigit(ByVal ch As Integer) As Integer
			Return ([ctype](ch And &H7F) And &H3F)
		End Function

		Friend Shared Function toLower(ByVal ch As Integer) As Integer
			Return If(isUpper(ch), (ch + &H20), ch)
		End Function

		Friend Shared Function toUpper(ByVal ch As Integer) As Integer
			Return If(isLower(ch), (ch - &H20), ch)
		End Function

	End Class

End Namespace