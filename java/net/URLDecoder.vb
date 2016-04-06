Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Utility class for HTML form decoding. This class contains static methods
	''' for decoding a String from the <CODE>application/x-www-form-urlencoded</CODE>
	''' MIME format.
	''' <p>
	''' The conversion process is the reverse of that used by the URLEncoder class. It is assumed
	''' that all characters in the encoded string are one of the following:
	''' &quot;{@code a}&quot; through &quot;{@code z}&quot;,
	''' &quot;{@code A}&quot; through &quot;{@code Z}&quot;,
	''' &quot;{@code 0}&quot; through &quot;{@code 9}&quot;, and
	''' &quot;{@code -}&quot;, &quot;{@code _}&quot;,
	''' &quot;{@code .}&quot;, and &quot;{@code *}&quot;. The
	''' character &quot;{@code %}&quot; is allowed but is interpreted
	''' as the start of a special escaped sequence.
	''' <p>
	''' The following rules are applied in the conversion:
	''' 
	''' <ul>
	''' <li>The alphanumeric characters &quot;{@code a}&quot; through
	'''     &quot;{@code z}&quot;, &quot;{@code A}&quot; through
	'''     &quot;{@code Z}&quot; and &quot;{@code 0}&quot;
	'''     through &quot;{@code 9}&quot; remain the same.
	''' <li>The special characters &quot;{@code .}&quot;,
	'''     &quot;{@code -}&quot;, &quot;{@code *}&quot;, and
	'''     &quot;{@code _}&quot; remain the same.
	''' <li>The plus sign &quot;{@code +}&quot; is converted into a
	'''     space character &quot; &nbsp; &quot; .
	''' <li>A sequence of the form "<i>{@code %xy}</i>" will be
	'''     treated as representing a byte where <i>xy</i> is the two-digit
	'''     hexadecimal representation of the 8 bits. Then, all substrings
	'''     that contain one or more of these byte sequences consecutively
	'''     will be replaced by the character(s) whose encoding would result
	'''     in those consecutive bytes.
	'''     The encoding scheme used to decode these characters may be specified,
	'''     or if unspecified, the default encoding of the platform will be used.
	''' </ul>
	''' <p>
	''' There are two possible ways in which this decoder could deal with
	''' illegal strings.  It could either leave illegal characters alone or
	''' it could throw an <seealso cref="java.lang.IllegalArgumentException"/>.
	''' Which approach the decoder takes is left to the
	''' implementation.
	''' 
	''' @author  Mark Chamness
	''' @author  Michael McCloskey
	''' @since   1.2
	''' </summary>

	Public Class URLDecoder

		' The platform default encoding
		Friend Shared dfltEncName As String = URLEncoder.dfltEncName

		''' <summary>
		''' Decodes a {@code x-www-form-urlencoded} string.
		''' The platform's default encoding is used to determine what characters
		''' are represented by any consecutive sequences of the form
		''' "<i>{@code %xy}</i>". </summary>
		''' <param name="s"> the {@code String} to decode </param>
		''' @deprecated The resulting string may vary depending on the platform's
		'''          default encoding. Instead, use the decode(String,String) method
		'''          to specify the encoding. 
		''' <returns> the newly decoded {@code String} </returns>
		<Obsolete("The resulting string may vary depending on the platform's")> _
		Public Shared Function decode(  s As String) As String

			Dim str As String = Nothing

			Try
				str = decode(s, dfltEncName)
			Catch e As UnsupportedEncodingException
				' The system should always have the platform default
			End Try

			Return str
		End Function

		''' <summary>
		''' Decodes a {@code application/x-www-form-urlencoded} string using a specific
		''' encoding scheme.
		''' The supplied encoding is used to determine
		''' what characters are represented by any consecutive sequences of the
		''' form "<i>{@code %xy}</i>".
		''' <p>
		''' <em><strong>Note:</strong> The <a href=
		''' "http://www.w3.org/TR/html40/appendix/notes.html#non-ascii-chars">
		''' World Wide Web Consortium Recommendation</a> states that
		''' UTF-8 should be used. Not doing so may introduce
		''' incompatibilities.</em>
		''' </summary>
		''' <param name="s"> the {@code String} to decode </param>
		''' <param name="enc">   The name of a supported
		'''    <a href="../lang/package-summary.html#charenc">character
		'''    encoding</a>. </param>
		''' <returns> the newly decoded {@code String} </returns>
		''' <exception cref="UnsupportedEncodingException">
		'''             If character encoding needs to be consulted, but
		'''             named character encoding is not supported </exception>
		''' <seealso cref= URLEncoder#encode(java.lang.String, java.lang.String)
		''' @since 1.4 </seealso>
		Public Shared Function decode(  s As String,   enc As String) As String

			Dim needToChange As Boolean = False
			Dim numChars As Integer = s.length()
			Dim sb As New StringBuffer(If(numChars > 500, numChars \ 2, numChars))
			Dim i As Integer = 0

			If enc.length() = 0 Then Throw New UnsupportedEncodingException("URLDecoder: empty string enc parameter")

			Dim c As Char
			Dim bytes As SByte() = Nothing
			Do While i < numChars
				c = s.Chars(i)
				Select Case c
				Case "+"c
					sb.append(" "c)
					i += 1
					needToChange = True
				Case "%"c
	'                
	'                 * Starting with this instance of %, process all
	'                 * consecutive substrings of the form %xy. Each
	'                 * substring %xy will yield a java.lang.[Byte]. Convert all
	'                 * consecutive  bytes obtained this way to whatever
	'                 * character(s) they represent in the provided
	'                 * encoding.
	'                 

					Try

						' (numChars-i)/3 is an upper bound for the number
						' of remaining bytes
						If bytes Is Nothing Then bytes = New SByte((numChars-i)\3 - 1){}
						Dim pos As Integer = 0

						Do While ((i+2) < numChars) AndAlso (c="%"c)
							Dim v As Integer = Convert.ToInt32(s.Substring(i+1, i+3 - (i+1)),16)
							If v < 0 Then Throw New IllegalArgumentException("URLDecoder: Illegal hex characters in escape (%) pattern - negative value")
							bytes(pos) = CByte(v)
							pos += 1
							i+= 3
							If i < numChars Then c = s.Chars(i)
						Loop

						' A trailing, incomplete byte encoding such as
						' "%x" will cause an exception to be thrown

						If (i < numChars) AndAlso (c="%"c) Then Throw New IllegalArgumentException("URLDecoder: Incomplete trailing escape (%) pattern")

						sb.append(New String(bytes, 0, pos, enc))
					Catch e As NumberFormatException
						Throw New IllegalArgumentException("URLDecoder: Illegal hex characters in escape (%) pattern - " & e.Message)
					End Try
					needToChange = True
				Case Else
					sb.append(c)
					i += 1
				End Select
			Loop

			Return (If(needToChange, sb.ToString(), s))
		End Function
	End Class

End Namespace