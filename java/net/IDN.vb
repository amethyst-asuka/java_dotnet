Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.net



	''' <summary>
	''' Provides methods to convert internationalized domain names (IDNs) between
	''' a normal Unicode representation and an ASCII Compatible Encoding (ACE) representation.
	''' Internationalized domain names can use characters from the entire range of
	''' Unicode, while traditional domain names are restricted to ASCII characters.
	''' ACE is an encoding of Unicode strings that uses only ASCII characters and
	''' can be used with software (such as the Domain Name System) that only
	''' understands traditional domain names.
	''' 
	''' <p>Internationalized domain names are defined in <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
	''' RFC 3490 defines two operations: ToASCII and ToUnicode. These 2 operations employ
	''' <a href="http://www.ietf.org/rfc/rfc3491.txt">Nameprep</a> algorithm, which is a
	''' profile of <a href="http://www.ietf.org/rfc/rfc3454.txt">Stringprep</a>, and
	''' <a href="http://www.ietf.org/rfc/rfc3492.txt">Punycode</a> algorithm to convert
	''' domain name string back and forth.
	''' 
	''' <p>The behavior of aforementioned conversion process can be adjusted by various flags:
	'''   <ul>
	'''     <li>If the ALLOW_UNASSIGNED flag is used, the domain name string to be converted
	'''         can contain code points that are unassigned in Unicode 3.2, which is the
	'''         Unicode version on which IDN conversion is based. If the flag is not used,
	'''         the presence of such unassigned code points is treated as an error.
	'''     <li>If the USE_STD3_ASCII_RULES flag is used, ASCII strings are checked against <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC 1122</a> and <a href="http://www.ietf.org/rfc/rfc1123.txt">RFC 1123</a>.
	'''         It is an error if they don't meet the requirements.
	'''   </ul>
	''' These flags can be logically OR'ed together.
	''' 
	''' <p>The security consideration is important with respect to internationalization
	''' domain name support. For example, English domain names may be <i>homographed</i>
	''' - maliciously misspelled by substitution of non-Latin letters.
	''' <a href="http://www.unicode.org/reports/tr36/">Unicode Technical Report #36</a>
	''' discusses security issues of IDN support as well as possible solutions.
	''' Applications are responsible for taking adequate security measures when using
	''' international domain names.
	''' 
	''' @author Edward Wang
	''' @since 1.6
	''' 
	''' </summary>
	Public NotInheritable Class IDN
		''' <summary>
		''' Flag to allow processing of unassigned code points
		''' </summary>
		Public Const ALLOW_UNASSIGNED As Integer = &H1

		''' <summary>
		''' Flag to turn on the check against STD-3 ASCII rules
		''' </summary>
		Public Const USE_STD3_ASCII_RULES As Integer = &H2


		''' <summary>
		''' Translates a string from Unicode to ASCII Compatible Encoding (ACE),
		''' as defined by the ToASCII operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		''' 
		''' <p>ToASCII operation can fail. ToASCII fails if any step of it fails.
		''' If ToASCII operation fails, an IllegalArgumentException will be thrown.
		''' In this case, the input string should not be used in an internationalized domain name.
		''' 
		''' <p> A label is an individual part of a domain name. The original ToASCII operation,
		''' as defined in RFC 3490, only operates on a single label. This method can handle
		''' both label and entire domain name, by assuming that labels in a domain name are
		''' always separated by dots. The following characters are recognized as dots:
		''' &#0092;u002E (full stop), &#0092;u3002 (ideographic full stop), &#0092;uFF0E (fullwidth full stop),
		''' and &#0092;uFF61 (halfwidth ideographic full stop). if dots are
		''' used as label separators, this method also changes all of them to &#0092;u002E (full stop)
		''' in output translated string.
		''' </summary>
		''' <param name="input">     the string to be processed </param>
		''' <param name="flag">      process flag; can be 0 or any logical OR of possible flags
		''' </param>
		''' <returns>          the translated {@code String}
		''' </returns>
		''' <exception cref="IllegalArgumentException">   if the input string doesn't conform to RFC 3490 specification </exception>
		Public Shared Function toASCII(  input As String,   flag As Integer) As String
			Dim p As Integer = 0, q As Integer = 0
			Dim out As New StringBuffer

			If isRootLabel(input) Then Return "."

			Do While p < input.length()
				q = searchDots(input, p)
				out.append(toASCIIInternal(input.Substring(p, q - p), flag))
				If q <> (input.length()) Then out.append("."c)
				p = q + 1
			Loop

			Return out.ToString()
		End Function


		''' <summary>
		''' Translates a string from Unicode to ASCII Compatible Encoding (ACE),
		''' as defined by the ToASCII operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		''' 
		''' <p> This convenience method works as if by invoking the
		''' two-argument counterpart as follows:
		''' <blockquote>
		''' <seealso cref="#toASCII(String, int) toASCII"/>(input,&nbsp;0);
		''' </blockquote>
		''' </summary>
		''' <param name="input">     the string to be processed
		''' </param>
		''' <returns>          the translated {@code String}
		''' </returns>
		''' <exception cref="IllegalArgumentException">   if the input string doesn't conform to RFC 3490 specification </exception>
		Public Shared Function toASCII(  input As String) As String
			Return toASCII(input, 0)
		End Function


		''' <summary>
		''' Translates a string from ASCII Compatible Encoding (ACE) to Unicode,
		''' as defined by the ToUnicode operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		''' 
		''' <p>ToUnicode never fails. In case of any error, the input string is returned unmodified.
		''' 
		''' <p> A label is an individual part of a domain name. The original ToUnicode operation,
		''' as defined in RFC 3490, only operates on a single label. This method can handle
		''' both label and entire domain name, by assuming that labels in a domain name are
		''' always separated by dots. The following characters are recognized as dots:
		''' &#0092;u002E (full stop), &#0092;u3002 (ideographic full stop), &#0092;uFF0E (fullwidth full stop),
		''' and &#0092;uFF61 (halfwidth ideographic full stop).
		''' </summary>
		''' <param name="input">     the string to be processed </param>
		''' <param name="flag">      process flag; can be 0 or any logical OR of possible flags
		''' </param>
		''' <returns>          the translated {@code String} </returns>
		Public Shared Function toUnicode(  input As String,   flag As Integer) As String
			Dim p As Integer = 0, q As Integer = 0
			Dim out As New StringBuffer

			If isRootLabel(input) Then Return "."

			Do While p < input.length()
				q = searchDots(input, p)
				out.append(toUnicodeInternal(input.Substring(p, q - p), flag))
				If q <> (input.length()) Then out.append("."c)
				p = q + 1
			Loop

			Return out.ToString()
		End Function


		''' <summary>
		''' Translates a string from ASCII Compatible Encoding (ACE) to Unicode,
		''' as defined by the ToUnicode operation of <a href="http://www.ietf.org/rfc/rfc3490.txt">RFC 3490</a>.
		''' 
		''' <p> This convenience method works as if by invoking the
		''' two-argument counterpart as follows:
		''' <blockquote>
		''' <seealso cref="#toUnicode(String, int) toUnicode"/>(input,&nbsp;0);
		''' </blockquote>
		''' </summary>
		''' <param name="input">     the string to be processed
		''' </param>
		''' <returns>          the translated {@code String} </returns>
		Public Shared Function toUnicode(  input As String) As String
			Return toUnicode(input, 0)
		End Function


		' ---------------- Private members -------------- 

		' ACE Prefix is "xn--"
		Private Const ACE_PREFIX As String = "xn--"
		Private Shared ReadOnly ACE_PREFIX_LENGTH As Integer = ACE_PREFIX.length()

		Private Const MAX_LABEL_LENGTH As Integer = 63

		' single instance of nameprep
		Private Shared namePrep As sun.net.idn.StringPrep = Nothing

		Shared Sub New()
			Dim stream As java.io.InputStream = Nothing

			Try
				Const IDN_PROFILE As String = "uidna.spp"
				If System.securityManager IsNot Nothing Then
					stream = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				Else
					stream = GetType(sun.net.idn.StringPrep).getResourceAsStream(IDN_PROFILE)
				End If

				namePrep = New sun.net.idn.StringPrep(stream)
				stream.close()
			Catch e As java.io.IOException
				' should never reach here
				Debug.Assert(False)
			End Try
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As java.io.InputStream
				Return GetType(sun.net.idn.StringPrep).getResourceAsStream(IDN_PROFILE)
			End Function
		End Class


		' ---------------- Private operations -------------- 


		'
		' to suppress the default zero-argument constructor
		'
		Private Sub New()
		End Sub

		'
		' toASCII operation; should only apply to a single label
		'
		Private Shared Function toASCIIInternal(  label As String,   flag As Integer) As String
			' step 1
			' Check if the string contains code points outside the ASCII range 0..0x7c.
			Dim isASCII As Boolean = isAllASCII(label)
			Dim dest As StringBuffer

			' step 2
			' perform the nameprep operation; flag ALLOW_UNASSIGNED is used here
			If Not isASCII Then
				Dim iter As sun.text.normalizer.UCharacterIterator = sun.text.normalizer.UCharacterIterator.getInstance(label)
				Try
					dest = namePrep.prepare(iter, flag)
				Catch e As java.text.ParseException
					Throw New IllegalArgumentException(e)
				End Try
			Else
				dest = New StringBuffer(label)
			End If

			' step 8, move forward to check the smallest number of the code points
			' the length must be inside 1..63
			If dest.length() = 0 Then Throw New IllegalArgumentException("Empty label is not a legal name")

			' step 3
			' Verify the absence of non-LDH ASCII code points
			'   0..0x2c, 0x2e..0x2f, 0x3a..0x40, 0x5b..0x60, 0x7b..0x7f
			' Verify the absence of leading and trailing hyphen
			Dim useSTD3ASCIIRules As Boolean = ((flag And USE_STD3_ASCII_RULES) <> 0)
			If useSTD3ASCIIRules Then
				For i As Integer = 0 To dest.length() - 1
					Dim c As Integer = AscW(dest.Chars(i))
					If isNonLDHAsciiCodePoint(c) Then Throw New IllegalArgumentException("Contains non-LDH ASCII characters")
				Next i

				If dest.Chars(0) Is "-"c OrElse dest.Chars(dest.length() - 1) Is "-"c Then Throw New IllegalArgumentException("Has leading or trailing hyphen")
			End If

			If Not isASCII Then
				' step 4
				' If all code points are inside 0..0x7f, skip to step 8
				If Not isAllASCII(dest.ToString()) Then
					' step 5
					' verify the sequence does not begin with ACE prefix
					If Not startsWithACEPrefix(dest) Then

						' step 6
						' encode the sequence with punycode
						Try
							dest = sun.net.idn.Punycode.encode(dest, Nothing)
						Catch e As java.text.ParseException
							Throw New IllegalArgumentException(e)
						End Try

						dest = toASCIILower(dest)

						' step 7
						' prepend the ACE prefix
						dest.insert(0, ACE_PREFIX)
					Else
						Throw New IllegalArgumentException("The input starts with the ACE Prefix")
					End If

				End If
			End If

			' step 8
			' the length must be inside 1..63
			If dest.length() > MAX_LABEL_LENGTH Then Throw New IllegalArgumentException("The label in the input is too long")

			Return dest.ToString()
		End Function

		'
		' toUnicode operation; should only apply to a single label
		'
		Private Shared Function toUnicodeInternal(  label As String,   flag As Integer) As String
			Dim caseFlags As Boolean() = Nothing
			Dim dest As StringBuffer

			' step 1
			' find out if all the codepoints in input are ASCII
			Dim isASCII As Boolean = isAllASCII(label)

			If Not isASCII Then
				' step 2
				' perform the nameprep operation; flag ALLOW_UNASSIGNED is used here
				Try
					Dim iter As sun.text.normalizer.UCharacterIterator = sun.text.normalizer.UCharacterIterator.getInstance(label)
					dest = namePrep.prepare(iter, flag)
				Catch e As Exception
					' toUnicode never fails; if any step fails, return the input string
					Return label
				End Try
			Else
				dest = New StringBuffer(label)
			End If

			' step 3
			' verify ACE Prefix
			If startsWithACEPrefix(dest) Then

				' step 4
				' Remove the ACE Prefix
				Dim temp As String = dest.Substring(ACE_PREFIX_LENGTH, dest.length() - ACE_PREFIX_LENGTH)

				Try
					' step 5
					' Decode using punycode
					Dim decodeOut As StringBuffer = sun.net.idn.Punycode.decode(New StringBuffer(temp), Nothing)

					' step 6
					' Apply toASCII
					Dim toASCIIOut As String = toASCII(decodeOut.ToString(), flag)

					' step 7
					' verify
					If toASCIIOut.equalsIgnoreCase(dest.ToString()) Then Return decodeOut.ToString()
				Catch ignored As Exception
					' no-op
				End Try
			End If

			' just return the input
			Return label
		End Function


		'
		' LDH stands for "letter/digit/hyphen", with characters restricted to the
		' 26-letter Latin alphabet <A-Z a-z>, the digits <0-9>, and the hyphen
		' <->.
		' Non LDH refers to characters in the ASCII range, but which are not
		' letters, digits or the hypen.
		'
		' non-LDH = 0..0x2C, 0x2E..0x2F, 0x3A..0x40, 0x5B..0x60, 0x7B..0x7F
		'
		Private Shared Function isNonLDHAsciiCodePoint(  ch As Integer) As Boolean
			Return (&H0 <= ch AndAlso ch <= &H2C) OrElse (&H2E <= ch AndAlso ch <= &H2F) OrElse (&H3A <= ch AndAlso ch <= &H40) OrElse (&H5B <= ch AndAlso ch <= &H60) OrElse (&H7B <= ch AndAlso ch <= &H7F)
		End Function

		'
		' search dots in a string and return the index of that character;
		' or if there is no dots, return the length of input string
		' dots might be: \u002E (full stop), \u3002 (ideographic full stop), \uFF0E (fullwidth full stop),
		' and \uFF61 (halfwidth ideographic full stop).
		'
		Private Shared Function searchDots(  s As String,   start As Integer) As Integer
			Dim i As Integer
			For i = start To s.length() - 1
				If isLabelSeparator(s.Chars(i)) Then Exit For
			Next i

			Return i
		End Function

		'
		' to check if a string is a root label, ".".
		'
		Private Shared Function isRootLabel(  s As String) As Boolean
			Return (s.length() = 1 AndAlso isLabelSeparator(s.Chars(0)))
		End Function

		'
		' to check if a character is a label separator, i.e. a dot character.
		'
		Private Shared Function isLabelSeparator(  c As Char) As Boolean
			Return (c = "."c OrElse c = ChrW(&H3002) OrElse c = ChrW(&HFF0E) OrElse c = ChrW(&HFF61))
		End Function

		'
		' to check if a string only contains US-ASCII code point
		'
		Private Shared Function isAllASCII(  input As String) As Boolean
			Dim isASCII As Boolean = True
			For i As Integer = 0 To input.length() - 1
				Dim c As Integer = AscW(input.Chars(i))
				If c > &H7F Then
					isASCII = False
					Exit For
				End If
			Next i
			Return isASCII
		End Function

		'
		' to check if a string starts with ACE-prefix
		'
		Private Shared Function startsWithACEPrefix(  input As StringBuffer) As Boolean
			Dim startsWithPrefix As Boolean = True

			If input.length() < ACE_PREFIX_LENGTH Then Return False
			For i As Integer = 0 To ACE_PREFIX_LENGTH - 1
				If toASCIILower(input.Chars(i)) IsNot ACE_PREFIX.Chars(i) Then startsWithPrefix = False
			Next i
			Return startsWithPrefix
		End Function

		Private Shared Function toASCIILower(  ch As Char) As Char
			If "A"c <= ch AndAlso ch <= "Z"c Then Return ChrW(AscW(ch) + AscW("a"c) - AscW("A"c))
			Return ch
		End Function

		Private Shared Function toASCIILower(  input As StringBuffer) As StringBuffer
			Dim dest As New StringBuffer
			For i As Integer = 0 To input.length() - 1
				dest.append(toASCIILower(input.Chars(i)))
			Next i
			Return dest
		End Function
	End Class

End Namespace