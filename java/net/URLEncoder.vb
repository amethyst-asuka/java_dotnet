Imports System
Imports System.Collections

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Utility class for HTML form encoding. This class contains static methods
	''' for converting a String to the <CODE>application/x-www-form-urlencoded</CODE> MIME
	''' format. For more information about HTML form encoding, consult the HTML
	''' <A HREF="http://www.w3.org/TR/html4/">specification</A>.
	''' 
	''' <p>
	''' When encoding a String, the following rules apply:
	''' 
	''' <ul>
	''' <li>The alphanumeric characters &quot;{@code a}&quot; through
	'''     &quot;{@code z}&quot;, &quot;{@code A}&quot; through
	'''     &quot;{@code Z}&quot; and &quot;{@code 0}&quot;
	'''     through &quot;{@code 9}&quot; remain the same.
	''' <li>The special characters &quot;{@code .}&quot;,
	'''     &quot;{@code -}&quot;, &quot;{@code *}&quot;, and
	'''     &quot;{@code _}&quot; remain the same.
	''' <li>The space character &quot; &nbsp; &quot; is
	'''     converted into a plus sign &quot;{@code +}&quot;.
	''' <li>All other characters are unsafe and are first converted into
	'''     one or more bytes using some encoding scheme. Then each byte is
	'''     represented by the 3-character string
	'''     &quot;<i>{@code %xy}</i>&quot;, where <i>xy</i> is the
	'''     two-digit hexadecimal representation of the java.lang.[Byte].
	'''     The recommended encoding scheme to use is UTF-8. However,
	'''     for compatibility reasons, if an encoding is not specified,
	'''     then the default encoding of the platform is used.
	''' </ul>
	''' 
	''' <p>
	''' For example using UTF-8 as the encoding scheme the string &quot;The
	''' string &#252;@foo-bar&quot; would get converted to
	''' &quot;The+string+%C3%BC%40foo-bar&quot; because in UTF-8 the character
	''' &#252; is encoded as two bytes C3 (hex) and BC (hex), and the
	''' character @ is encoded as one byte 40 (hex).
	''' 
	''' @author  Herb Jellinek
	''' @since   JDK1.0
	''' </summary>
	Public Class URLEncoder
		Friend Shared dontNeedEncoding As BitArray
		Friend Shared ReadOnly caseDiff As Integer = (AscW("a"c) - AscW("A"c))
		Friend Shared dfltEncName As String = Nothing

		Shared Sub New()

	'         The list of characters that are not encoded has been
	'         * determined as follows:
	'         *
	'         * RFC 2396 states:
	'         * -----
	'         * Data characters that are allowed in a URI but do not have a
	'         * reserved purpose are called unreserved.  These include upper
	'         * and lower case letters, decimal digits, and a limited set of
	'         * punctuation marks and symbols.
	'         *
	'         * unreserved  = alphanum | mark
	'         *
	'         * mark        = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
	'         *
	'         * Unreserved characters can be escaped without changing the
	'         * semantics of the URI, but this should not be done unless the
	'         * URI is being used in a context that does not allow the
	'         * unescaped character to appear.
	'         * -----
	'         *
	'         * It appears that both Netscape and Internet Explorer escape
	'         * all special characters from this list with the exception
	'         * of "-", "_", ".", "*". While it is not clear why they are
	'         * escaping the other characters, perhaps it is safest to
	'         * assume that there might be contexts in which the others
	'         * are unsafe if not escaped. Therefore, we will use the same
	'         * list. It is also noteworthy that this is consistent with
	'         * O'Reilly's "HTML: The Definitive Guide" (page 164).
	'         *
	'         * As a last note, Intenet Explorer does not encode the "@"
	'         * character which is clearly not unreserved according to the
	'         * RFC. We are being consistent with the RFC in this matter,
	'         * as is Netscape.
	'         *
	'         

			dontNeedEncoding = New BitArray(256)
			Dim i As Integer
			For i = AscW("a"c) To AscW("z"c)
				dontNeedEncoding.Set(i, True)
			Next i
			For i = AscW("A"c) To AscW("Z"c)
				dontNeedEncoding.Set(i, True)
			Next i
			For i = AscW("0"c) To AscW("9"c)
				dontNeedEncoding.Set(i, True)
			Next i
			dontNeedEncoding.Set(" "c, True) ' encoding a space to a + is done
	'                                    * in the encode() method 
			dontNeedEncoding.Set("-"c, True)
			dontNeedEncoding.Set("_"c, True)
			dontNeedEncoding.Set("."c, True)
			dontNeedEncoding.Set("*"c, True)

			dfltEncName = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("file.encoding")
		   )
		End Sub

		''' <summary>
		''' You can't call the constructor.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Translates a string into {@code x-www-form-urlencoded}
		''' format. This method uses the platform's default encoding
		''' as the encoding scheme to obtain the bytes for unsafe characters.
		''' </summary>
		''' <param name="s">   {@code String} to be translated. </param>
		''' @deprecated The resulting string may vary depending on the platform's
		'''             default encoding. Instead, use the encode(String,String)
		'''             method to specify the encoding. 
		''' <returns>  the translated {@code String}. </returns>
		<Obsolete("The resulting string may vary depending on the platform's")> _
		Public Shared Function encode(ByVal s As String) As String

			Dim str As String = Nothing

			Try
				str = encode(s, dfltEncName)
			Catch e As java.io.UnsupportedEncodingException
				' The system should always have the platform default
			End Try

			Return str
		End Function

		''' <summary>
		''' Translates a string into {@code application/x-www-form-urlencoded}
		''' format using a specific encoding scheme. This method uses the
		''' supplied encoding scheme to obtain the bytes for unsafe
		''' characters.
		''' <p>
		''' <em><strong>Note:</strong> The <a href=
		''' "http://www.w3.org/TR/html40/appendix/notes.html#non-ascii-chars">
		''' World Wide Web Consortium Recommendation</a> states that
		''' UTF-8 should be used. Not doing so may introduce
		''' incompatibilities.</em>
		''' </summary>
		''' <param name="s">   {@code String} to be translated. </param>
		''' <param name="enc">   The name of a supported
		'''    <a href="../lang/package-summary.html#charenc">character
		'''    encoding</a>. </param>
		''' <returns>  the translated {@code String}. </returns>
		''' <exception cref="UnsupportedEncodingException">
		'''             If the named encoding is not supported </exception>
		''' <seealso cref= URLDecoder#decode(java.lang.String, java.lang.String)
		''' @since 1.4 </seealso>
		Public Shared Function encode(ByVal s As String, ByVal enc As String) As String

			Dim needToChange As Boolean = False
			Dim out As New StringBuffer(s.length())
			Dim charset As java.nio.charset.Charset
			Dim charArrayWriter As New java.io.CharArrayWriter

			If enc Is Nothing Then Throw New NullPointerException("charsetName")

			Try
				charset = java.nio.charset.Charset.forName(enc)
			Catch e As java.nio.charset.IllegalCharsetNameException
				Throw New java.io.UnsupportedEncodingException(enc)
			Catch e As java.nio.charset.UnsupportedCharsetException
				Throw New java.io.UnsupportedEncodingException(enc)
			End Try

			Dim i As Integer = 0
			Do While i < s.length()
				Dim c As Integer = AscW(s.Chars(i))
				'System.out.println("Examining character: " + c);
				If dontNeedEncoding.Get(c) Then
					If c = AscW(" "c) Then
						c = AscW("+"c)
						needToChange = True
					End If
					'System.out.println("Storing: " + c);
					out.append(ChrW(c))
					i += 1
				Else
					' convert to external encoding before hex conversion
					Do
						charArrayWriter.write(c)
	'                    
	'                     * If this character represents the start of a Unicode
	'                     * surrogate pair, then pass in two characters. It's not
	'                     * clear what should be done if a bytes reserved in the
	'                     * surrogate pairs range occurs outside of a legal
	'                     * surrogate pair. For now, just treat it as if it were
	'                     * any other character.
	'                     
						If c >= &HD800 AndAlso c <= &HDBFF Then
	'                        
	'                          System.out.println( java.lang.[Integer].toHexString(c)
	'                          + " is high surrogate");
	'                        
							If (i+1) < s.length() Then
								Dim d As Integer = AscW(s.Chars(i+1))
	'                            
	'                              System.out.println("\tExamining "
	'                              +  java.lang.[Integer].toHexString(d));
	'                            
								If d >= &HDC00 AndAlso d <= &HDFFF Then
	'                                
	'                                  System.out.println("\t"
	'                                  +  java.lang.[Integer].toHexString(d)
	'                                  + " is low surrogate");
	'                                
									charArrayWriter.write(d)
									i += 1
								End If
							End If
						End If
						i += 1
						c = AscW(s.Chars(i))
					Loop While i < s.length() AndAlso Not dontNeedEncoding.Get(c)

					charArrayWriter.flush()
					Dim str As New String(charArrayWriter.ToCharArray())
					Dim ba As SByte() = str.getBytes(charset)
					For j As Integer = 0 To ba.Length - 1
						out.append("%"c)
						Dim ch As Char = Character.forDigit((ba(j) >> 4) And &HF, 16)
						' converting to use uppercase letter as part of
						' the hex value if ch is a letter.
						If Char.IsLetter(ch) Then AscW(ch) -= caseDiff
						out.append(ch)
						ch = Character.forDigit(ba(j) And &HF, 16)
						If Char.IsLetter(ch) Then AscW(ch) -= caseDiff
						out.append(ch)
					Next j
					charArrayWriter.reset()
					needToChange = True
				End If
			Loop

			Return (If(needToChange, out.ToString(), s))
		End Function
	End Class

End Namespace