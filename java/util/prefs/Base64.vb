Imports System

'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.prefs

	''' <summary>
	''' Static methods for translating Base64 encoded strings to byte arrays
	''' and vice-versa.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     Preferences
	''' @since   1.4 </seealso>
	Friend Class Base64
		''' <summary>
		''' Translates the specified byte array into a Base64 string as per
		''' Preferences.put(byte[]).
		''' </summary>
		Friend Shared Function byteArrayToBase64(ByVal a As SByte()) As String
			Return byteArrayToBase64(a, False)
		End Function

		''' <summary>
		''' Translates the specified byte array into an "alternate representation"
		''' Base64 string.  This non-standard variant uses an alphabet that does
		''' not contain the uppercase alphabetic characters, which makes it
		''' suitable for use in situations where case-folding occurs.
		''' </summary>
		Friend Shared Function byteArrayToAltBase64(ByVal a As SByte()) As String
			Return byteArrayToBase64(a, True)
		End Function

		Private Shared Function byteArrayToBase64(ByVal a As SByte(), ByVal alternate As Boolean) As String
			Dim aLen As Integer = a.Length
			Dim numFullGroups As Integer = aLen\3
			Dim numBytesInPartialGroup As Integer = aLen - 3*numFullGroups
			Dim resultLen As Integer = 4*((aLen + 2)\3)
			Dim result As New StringBuffer(resultLen)
			Dim intToAlpha As Char() = (If(alternate, intToAltBase64, intToBase64))

			' Translate all full groups from byte array elements to Base64
			Dim inCursor As Integer = 0
			For i As Integer = 0 To numFullGroups - 1
				Dim byte0 As Integer = a(inCursor) And &Hff
				inCursor += 1
				Dim byte1 As Integer = a(inCursor) And &Hff
				inCursor += 1
				Dim byte2 As Integer = a(inCursor) And &Hff
				inCursor += 1
				result.append(intToAlpha(byte0 >> 2))
				result.append(intToAlpha((byte0 << 4) And &H3f Or (byte1 >> 4)))
				result.append(intToAlpha((byte1 << 2) And &H3f Or (byte2 >> 6)))
				result.append(intToAlpha(byte2 And &H3f))
			Next i

			' Translate partial group if present
			If numBytesInPartialGroup <> 0 Then
				Dim byte0 As Integer = a(inCursor) And &Hff
				inCursor += 1
				result.append(intToAlpha(byte0 >> 2))
				If numBytesInPartialGroup = 1 Then
					result.append(intToAlpha((byte0 << 4) And &H3f))
					result.append("==")
				Else
					' assert numBytesInPartialGroup == 2;
					Dim byte1 As Integer = a(inCursor) And &Hff
					inCursor += 1
					result.append(intToAlpha((byte0 << 4) And &H3f Or (byte1 >> 4)))
					result.append(intToAlpha((byte1 << 2) And &H3f))
					result.append("="c)
				End If
			End If
			' assert inCursor == a.length;
			' assert result.length() == resultLen;
			Return result.ToString()
		End Function

		''' <summary>
		''' This array is a lookup table that translates 6-bit positive integer
		''' index values into their "Base64 Alphabet" equivalents as specified
		''' in Table 1 of RFC 2045.
		''' </summary>
		Private Shared ReadOnly intToBase64 As Char() = { "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "+"c, "/"c }

		''' <summary>
		''' This array is a lookup table that translates 6-bit positive integer
		''' index values into their "Alternate Base64 Alphabet" equivalents.
		''' This is NOT the real Base64 Alphabet as per in Table 1 of RFC 2045.
		''' This alternate alphabet does not use the capital letters.  It is
		''' designed for use in environments where "case folding" occurs.
		''' </summary>
		Private Shared ReadOnly intToAltBase64 As Char() = { "!"c, """"c, "#"c, "$"c, "%"c, "&"c, "'"c, "("c, ")"c, ","c, "-"c, "."c, ":"c, ";"c, "<"c, ">"c, "@"c, "["c, "]"c, "^"c, "`"c, "_"c, "{"c, "|"c, "}"c, "~"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "+"c, "?"c }

		''' <summary>
		''' Translates the specified Base64 string (as per Preferences.get(byte[]))
		''' into a byte array.
		''' 
		''' @throw IllegalArgumentException if <tt>s</tt> is not a valid Base64
		'''        string.
		''' </summary>
		Friend Shared Function base64ToByteArray(ByVal s As String) As SByte()
			Return base64ToByteArray(s, False)
		End Function

		''' <summary>
		''' Translates the specified "alternate representation" Base64 string
		''' into a byte array.
		''' 
		''' @throw IllegalArgumentException or ArrayOutOfBoundsException
		'''        if <tt>s</tt> is not a valid alternate representation
		'''        Base64 string.
		''' </summary>
		Friend Shared Function altBase64ToByteArray(ByVal s As String) As SByte()
			Return base64ToByteArray(s, True)
		End Function

		Private Shared Function base64ToByteArray(ByVal s As String, ByVal alternate As Boolean) As SByte()
			Dim alphaToInt As SByte() = (If(alternate, altBase64ToInt, base64ToInt_Renamed))
			Dim sLen As Integer = s.length()
			Dim numGroups As Integer = sLen\4
			If 4*numGroups <> sLen Then Throw New IllegalArgumentException("String length must be a multiple of four.")
			Dim missingBytesInLastGroup As Integer = 0
			Dim numFullGroups As Integer = numGroups
			If sLen <> 0 Then
				If s.Chars(sLen-1) = "="c Then
					missingBytesInLastGroup += 1
					numFullGroups -= 1
				End If
				If s.Chars(sLen-2) = "="c Then missingBytesInLastGroup += 1
			End If
			Dim result As SByte() = New SByte(3*numGroups - missingBytesInLastGroup - 1){}

			' Translate all full groups from base64 to byte array elements
			Dim inCursor As Integer = 0, outCursor As Integer = 0
			For i As Integer = 0 To numFullGroups - 1
				Dim ch0 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				Dim ch1 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				Dim ch2 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				Dim ch3 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				result(outCursor) = CByte((ch0 << 2) Or (ch1 >> 4))
				outCursor += 1
				result(outCursor) = CByte((ch1 << 4) Or (ch2 >> 2))
				outCursor += 1
				result(outCursor) = CByte((ch2 << 6) Or ch3)
				outCursor += 1
			Next i

			' Translate partial group, if present
			If missingBytesInLastGroup <> 0 Then
				Dim ch0 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				Dim ch1 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
				inCursor += 1
				result(outCursor) = CByte((ch0 << 2) Or (ch1 >> 4))
				outCursor += 1

				If missingBytesInLastGroup = 1 Then
					Dim ch2 As Integer = base64toInt(s.Chars(inCursor), alphaToInt)
					inCursor += 1
					result(outCursor) = CByte((ch1 << 4) Or (ch2 >> 2))
					outCursor += 1
				End If
			End If
			' assert inCursor == s.length()-missingBytesInLastGroup;
			' assert outCursor == result.length;
			Return result
		End Function

		''' <summary>
		''' Translates the specified character, which is assumed to be in the
		''' "Base 64 Alphabet" into its equivalent 6-bit positive integer.
		''' 
		''' @throw IllegalArgumentException or ArrayOutOfBoundsException if
		'''        c is not in the Base64 Alphabet.
		''' </summary>
		Private Shared Function base64toInt(ByVal c As Char, ByVal alphaToInt As SByte()) As Integer
			Dim result As Integer = alphaToInt(AscW(c))
			If result < 0 Then Throw New IllegalArgumentException("Illegal character " & AscW(c))
			Return result
		End Function

		''' <summary>
		''' This array is a lookup table that translates unicode characters
		''' drawn from the "Base64 Alphabet" (as specified in Table 1 of RFC 2045)
		''' into their 6-bit positive integer equivalents.  Characters that
		''' are not in the Base64 alphabet but fall within the bounds of the
		''' array are translated to -1.
		''' </summary>
		Private Shared ReadOnly base64ToInt_Renamed As SByte() = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51 }

		''' <summary>
		''' This array is the analogue of base64ToInt, but for the nonstandard
		''' variant that avoids the use of uppercase alphabetic characters.
		''' </summary>
		Private Shared ReadOnly altBase64ToInt As SByte() = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, 62, 9, 10, 11, -1, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 12, 13, 14, -1, 15, 63, 16, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 17, -1, 18, 19, 21, 20, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 22, 23, 24, 25 }

		Public Shared Sub Main(ByVal args As String())
			Dim numRuns As Integer = Convert.ToInt32(args(0))
			Dim numBytes As Integer = Convert.ToInt32(args(1))
			Dim rnd As New Random
			For i As Integer = 0 To numRuns - 1
				For j As Integer = 0 To numBytes - 1
					Dim arr As SByte() = New SByte(j - 1){}
					For k As Integer = 0 To j - 1
						arr(k) = CByte(rnd.Next())
					Next k

					Dim s As String = byteArrayToBase64(arr)
					Dim b As SByte() = base64ToByteArray(s)
					If Not Array.Equals(arr, b) Then Console.WriteLine("Dismal failure!")

					s = byteArrayToAltBase64(arr)
					b = altBase64ToByteArray(s)
					If Not Array.Equals(arr, b) Then Console.WriteLine("Alternate dismal failure!")
				Next j
			Next i
		End Sub
	End Class

End Namespace