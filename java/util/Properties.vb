Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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

Namespace java.util



	''' <summary>
	''' The {@code Properties} class represents a persistent set of
	''' properties. The {@code Properties} can be saved to a stream
	''' or loaded from a stream. Each key and its corresponding value in
	''' the property list is a string.
	''' <p>
	''' A property list can contain another property list as its
	''' "defaults"; this second property list is searched if
	''' the property key is not found in the original property list.
	''' <p>
	''' Because {@code Properties} inherits from {@code Hashtable}, the
	''' {@code put} and {@code putAll} methods can be applied to a
	''' {@code Properties} object.  Their use is strongly discouraged as they
	''' allow the caller to insert entries whose keys or values are not
	''' {@code Strings}.  The {@code setProperty} method should be used
	''' instead.  If the {@code store} or {@code save} method is called
	''' on a "compromised" {@code Properties} object that contains a
	''' non-{@code String} key or value, the call will fail. Similarly,
	''' the call to the {@code propertyNames} or {@code list} method
	''' will fail if it is called on a "compromised" {@code Properties}
	''' object that contains a non-{@code String} key.
	''' 
	''' <p>
	''' The <seealso cref="#load(java.io.Reader) load(Reader)"/> <tt>/</tt>
	''' <seealso cref="#store(java.io.Writer, java.lang.String) store(Writer, String)"/>
	''' methods load and store properties from and to a character based stream
	''' in a simple line-oriented format specified below.
	''' 
	''' The <seealso cref="#load(java.io.InputStream) load(InputStream)"/> <tt>/</tt>
	''' <seealso cref="#store(java.io.OutputStream, java.lang.String) store(OutputStream, String)"/>
	''' methods work the same way as the load(Reader)/store(Writer, String) pair, except
	''' the input/output stream is encoded in ISO 8859-1 character encoding.
	''' Characters that cannot be directly represented in this encoding can be written using
	''' Unicode escapes as defined in section 3.3 of
	''' <cite>The Java&trade; Language Specification</cite>;
	''' only a single 'u' character is allowed in an escape
	''' sequence. The native2ascii tool can be used to convert property files to and
	''' from other character encodings.
	''' 
	''' <p> The <seealso cref="#loadFromXML(InputStream)"/> and {@link
	''' #storeToXML(OutputStream, String, String)} methods load and store properties
	''' in a simple XML format.  By default the UTF-8 character encoding is used,
	''' however a specific encoding may be specified if required. Implementations
	''' are required to support UTF-8 and UTF-16 and may support other encodings.
	''' An XML properties document has the following DOCTYPE declaration:
	''' 
	''' <pre>
	''' &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
	''' </pre>
	''' Note that the system URI (http://java.sun.com/dtd/properties.dtd) is
	''' <i>not</i> accessed when exporting or importing properties; it merely
	''' serves as a string to uniquely identify the DTD, which is:
	''' <pre>
	'''    &lt;?xml version="1.0" encoding="UTF-8"?&gt;
	''' 
	'''    &lt;!-- DTD for properties --&gt;
	''' 
	'''    &lt;!ELEMENT properties ( comment?, entry* ) &gt;
	''' 
	'''    &lt;!ATTLIST properties version CDATA #FIXED "1.0"&gt;
	''' 
	'''    &lt;!ELEMENT comment (#PCDATA) &gt;
	''' 
	'''    &lt;!ELEMENT entry (#PCDATA) &gt;
	''' 
	'''    &lt;!ATTLIST entry key CDATA #REQUIRED&gt;
	''' </pre>
	''' 
	''' <p>This class is thread-safe: multiple threads can share a single
	''' <tt>Properties</tt> object without the need for external synchronization.
	''' </summary>
	''' <seealso cref= <a href="../../../technotes/tools/solaris/native2ascii.html">native2ascii tool for Solaris</a> </seealso>
	''' <seealso cref= <a href="../../../technotes/tools/windows/native2ascii.html">native2ascii tool for Windows</a>
	''' 
	''' @author  Arthur van Hoff
	''' @author  Michael McCloskey
	''' @author  Xueming Shen
	''' @since   JDK1.0 </seealso>
	Public Class Properties
		Inherits Dictionary(Of Object, Object)

		''' <summary>
		''' use serialVersionUID from JDK 1.1.X for interoperability
		''' </summary>
		 Private Const serialVersionUID As Long = 4112578634029874840L

		''' <summary>
		''' A property list that contains default values for any keys not
		''' found in this property list.
		''' 
		''' @serial
		''' </summary>
		Protected Friend defaults As Properties

		''' <summary>
		''' Creates an empty property list with no default values.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Creates an empty property list with the specified defaults.
		''' </summary>
		''' <param name="defaults">   the defaults. </param>
		Public Sub New(ByVal defaults As Properties)
			Me.defaults = defaults
		End Sub

		''' <summary>
		''' Calls the <tt>Hashtable</tt> method {@code put}. Provided for
		''' parallelism with the <tt>getProperty</tt> method. Enforces use of
		''' strings for property keys and values. The value returned is the
		''' result of the <tt>Hashtable</tt> call to {@code put}.
		''' </summary>
		''' <param name="key"> the key to be placed into this property list. </param>
		''' <param name="value"> the value corresponding to <tt>key</tt>. </param>
		''' <returns>     the previous value of the specified key in this property
		'''             list, or {@code null} if it did not have one. </returns>
		''' <seealso cref= #getProperty
		''' @since    1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function setProperty(ByVal key As String, ByVal value As String) As Object
			Return put(key, value)
		End Function


		''' <summary>
		''' Reads a property list (key and element pairs) from the input
		''' character stream in a simple line-oriented format.
		''' <p>
		''' Properties are processed in terms of lines. There are two
		''' kinds of line, <i>natural lines</i> and <i>logical lines</i>.
		''' A natural line is defined as a line of
		''' characters that is terminated either by a set of line terminator
		''' characters ({@code \n} or {@code \r} or {@code \r\n})
		''' or by the end of the stream. A natural line may be either a blank line,
		''' a comment line, or hold all or some of a key-element pair. A logical
		''' line holds all the data of a key-element pair, which may be spread
		''' out across several adjacent natural lines by escaping
		''' the line terminator sequence with a backslash character
		''' {@code \}.  Note that a comment line cannot be extended
		''' in this manner; every natural line that is a comment must have
		''' its own comment indicator, as described below. Lines are read from
		''' input until the end of the stream is reached.
		''' 
		''' <p>
		''' A natural line that contains only white space characters is
		''' considered blank and is ignored.  A comment line has an ASCII
		''' {@code '#'} or {@code '!'} as its first non-white
		''' space character; comment lines are also ignored and do not
		''' encode key-element information.  In addition to line
		''' terminators, this format considers the characters space
		''' ({@code ' '}, {@code '\u005Cu0020'}), tab
		''' ({@code '\t'}, {@code '\u005Cu0009'}), and form feed
		''' ({@code '\f'}, {@code '\u005Cu000C'}) to be white
		''' space.
		''' 
		''' <p>
		''' If a logical line is spread across several natural lines, the
		''' backslash escaping the line terminator sequence, the line
		''' terminator sequence, and any white space at the start of the
		''' following line have no affect on the key or element values.
		''' The remainder of the discussion of key and element parsing
		''' (when loading) will assume all the characters constituting
		''' the key and element appear on a single natural line after
		''' line continuation characters have been removed.  Note that
		''' it is <i>not</i> sufficient to only examine the character
		''' preceding a line terminator sequence to decide if the line
		''' terminator is escaped; there must be an odd number of
		''' contiguous backslashes for the line terminator to be escaped.
		''' Since the input is processed from left to right, a
		''' non-zero even number of 2<i>n</i> contiguous backslashes
		''' before a line terminator (or elsewhere) encodes <i>n</i>
		''' backslashes after escape processing.
		''' 
		''' <p>
		''' The key contains all of the characters in the line starting
		''' with the first non-white space character and up to, but not
		''' including, the first unescaped {@code '='},
		''' {@code ':'}, or white space character other than a line
		''' terminator. All of these key termination characters may be
		''' included in the key by escaping them with a preceding backslash
		''' character; for example,<p>
		''' 
		''' {@code \:\=}<p>
		''' 
		''' would be the two-character key {@code ":="}.  Line
		''' terminator characters can be included using {@code \r} and
		''' {@code \n} escape sequences.  Any white space after the
		''' key is skipped; if the first non-white space character after
		''' the key is {@code '='} or {@code ':'}, then it is
		''' ignored and any white space characters after it are also
		''' skipped.  All remaining characters on the line become part of
		''' the associated element string; if there are no remaining
		''' characters, the element is the empty string
		''' {@code ""}.  Once the raw character sequences
		''' constituting the key and element are identified, escape
		''' processing is performed as described above.
		''' 
		''' <p>
		''' As an example, each of the following three lines specifies the key
		''' {@code "Truth"} and the associated element value
		''' {@code "Beauty"}:
		''' <pre>
		''' Truth = Beauty
		'''  Truth:Beauty
		''' Truth                    :Beauty
		''' </pre>
		''' As another example, the following three lines specify a single
		''' property:
		''' <pre>
		''' fruits                           apple, banana, pear, \
		'''                                  cantaloupe, watermelon, \
		'''                                  kiwi, mango
		''' </pre>
		''' The key is {@code "fruits"} and the associated element is:
		''' <pre>"apple, banana, pear, cantaloupe, watermelon, kiwi, mango"</pre>
		''' Note that a space appears before each {@code \} so that a space
		''' will appear after each comma in the final result; the {@code \},
		''' line terminator, and leading white space on the continuation line are
		''' merely discarded and are <i>not</i> replaced by one or more other
		''' characters.
		''' <p>
		''' As a third example, the line:
		''' <pre>cheeses
		''' </pre>
		''' specifies that the key is {@code "cheeses"} and the associated
		''' element is the empty string {@code ""}.
		''' <p>
		''' <a name="unicodeescapes"></a>
		''' Characters in keys and elements can be represented in escape
		''' sequences similar to those used for character and string literals
		''' (see sections 3.3 and 3.10.6 of
		''' <cite>The Java&trade; Language Specification</cite>).
		''' 
		''' The differences from the character escape sequences and Unicode
		''' escapes used for characters and strings are:
		''' 
		''' <ul>
		''' <li> Octal escapes are not recognized.
		''' 
		''' <li> The character sequence {@code \b} does <i>not</i>
		''' represent a backspace character.
		''' 
		''' <li> The method does not treat a backslash character,
		''' {@code \}, before a non-valid escape character as an
		''' error; the backslash is silently dropped.  For example, in a
		''' Java string the sequence {@code "\z"} would cause a
		''' compile time error.  In contrast, this method silently drops
		''' the backslash.  Therefore, this method treats the two character
		''' sequence {@code "\b"} as equivalent to the single
		''' character {@code 'b'}.
		''' 
		''' <li> Escapes are not necessary for single and double quotes;
		''' however, by the rule above, single and double quote characters
		''' preceded by a backslash still yield single and double quote
		''' characters, respectively.
		''' 
		''' <li> Only a single 'u' character is allowed in a Unicode escape
		''' sequence.
		''' 
		''' </ul>
		''' <p>
		''' The specified stream remains open after this method returns.
		''' </summary>
		''' <param name="reader">   the input character stream. </param>
		''' <exception cref="IOException">  if an error occurred when reading from the
		'''          input stream. </exception>
		''' <exception cref="IllegalArgumentException"> if a malformed Unicode escape
		'''          appears in the input.
		''' @since   1.6 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub load(ByVal reader As java.io.Reader)
			load0(New LineReader(Me, reader))
		End Sub

		''' <summary>
		''' Reads a property list (key and element pairs) from the input
		''' byte stream. The input stream is in a simple line-oriented
		''' format as specified in
		''' <seealso cref="#load(java.io.Reader) load(Reader)"/> and is assumed to use
		''' the ISO 8859-1 character encoding; that is each byte is one Latin1
		''' character. Characters not in Latin1, and certain special characters,
		''' are represented in keys and elements using Unicode escapes as defined in
		''' section 3.3 of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' <p>
		''' The specified stream remains open after this method returns.
		''' </summary>
		''' <param name="inStream">   the input stream. </param>
		''' <exception cref="IOException">  if an error occurred when reading from the
		'''             input stream. </exception>
		''' <exception cref="IllegalArgumentException"> if the input stream contains a
		'''             malformed Unicode escape sequence.
		''' @since 1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub load(ByVal inStream As java.io.InputStream)
			load0(New LineReader(Me, inStream))
		End Sub

		Private Sub load0(ByVal lr As LineReader)
			Dim convtBuf As Char() = New Char(1023){}
			Dim limit As Integer
			Dim keyLen As Integer
			Dim valueStart As Integer
			Dim c As Char
			Dim hasSep As Boolean
			Dim precedingBackslash As Boolean

			limit = lr.readLine()
			Do While limit >= 0
				c = 0
				keyLen = 0
				valueStart = limit
				hasSep = False

				'System.out.println("line=<" + new String(lineBuf, 0, limit) + ">");
				precedingBackslash = False
				Do While keyLen < limit
					c = lr.lineBuf(keyLen)
					'need check if escaped.
					If (c = "="c OrElse c = ":"c) AndAlso (Not precedingBackslash) Then
						valueStart = keyLen + 1
						hasSep = True
						Exit Do
					ElseIf (c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.FormFeed) AndAlso (Not precedingBackslash) Then
						valueStart = keyLen + 1
						Exit Do
					End If
					If c = "\"c Then
						precedingBackslash = Not precedingBackslash
					Else
						precedingBackslash = False
					End If
					keyLen += 1
				Loop
				Do While valueStart < limit
					c = lr.lineBuf(valueStart)
					If c <> " "c AndAlso c <> ControlChars.Tab AndAlso c <> ControlChars.FormFeed Then
						If (Not hasSep) AndAlso (c = "="c OrElse c = ":"c) Then
							hasSep = True
						Else
							Exit Do
						End If
					End If
					valueStart += 1
				Loop
				Dim key As String = loadConvert(lr.lineBuf, 0, keyLen, convtBuf)
				Dim value As String = loadConvert(lr.lineBuf, valueStart, limit - valueStart, convtBuf)
				put(key, value)
				limit = lr.readLine()
			Loop
		End Sub

	'     Read in a "logical line" from an InputStream/Reader, skip all comment
	'     * and blank lines and filter out those leading whitespace characters
	'     * (\u0020, \u0009 and \u000c) from the beginning of a "natural line".
	'     * Method returns the char length of the "logical line" and stores
	'     * the line in "lineBuf".
	'     
		Friend Class LineReader
			Private ReadOnly outerInstance As Properties

			Public Sub New(ByVal outerInstance As Properties, ByVal inStream As java.io.InputStream)
					Me.outerInstance = outerInstance
				Me.inStream = inStream
				inByteBuf = New SByte(8191){}
			End Sub

			Public Sub New(ByVal outerInstance As Properties, ByVal reader As java.io.Reader)
					Me.outerInstance = outerInstance
				Me.reader = reader
				inCharBuf = New Char(8191){}
			End Sub

			Friend inByteBuf As SByte()
			Friend inCharBuf As Char()
			Friend lineBuf As Char() = New Char(1023){}
			Friend inLimit As Integer = 0
			Friend inOff As Integer = 0
			Friend inStream As java.io.InputStream
			Friend reader As java.io.Reader

			Friend Overridable Function readLine() As Integer
				Dim len As Integer = 0
				Dim c As Char = 0

				Dim skipWhiteSpace As Boolean = True
				Dim isCommentLine As Boolean = False
				Dim isNewLine As Boolean = True
				Dim appendedLineBegin As Boolean = False
				Dim precedingBackslash As Boolean = False
				Dim skipLF As Boolean = False

				Do
					If inOff >= inLimit Then
						inLimit = If(inStream Is Nothing, reader.read(inCharBuf), inStream.read(inByteBuf))
						inOff = 0
						If inLimit <= 0 Then
							If len = 0 OrElse isCommentLine Then Return -1
							If precedingBackslash Then len -= 1
							Return len
						End If
					End If
					If inStream IsNot Nothing Then
						'The line below is equivalent to calling a
						'ISO8859-1 decoder.
						c = CChar(&Hff And inByteBuf(inOff))
						inOff += 1
					Else
						c = inCharBuf(inOff)
						inOff += 1
					End If
					If skipLF Then
						skipLF = False
						If c = ControlChars.Lf Then Continue Do
					End If
					If skipWhiteSpace Then
						If c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.FormFeed Then Continue Do
						If (Not appendedLineBegin) AndAlso (c = ControlChars.Cr OrElse c = ControlChars.Lf) Then Continue Do
						skipWhiteSpace = False
						appendedLineBegin = False
					End If
					If isNewLine Then
						isNewLine = False
						If c = "#"c OrElse c = "!"c Then
							isCommentLine = True
							Continue Do
						End If
					End If

					If c <> ControlChars.Lf AndAlso c <> ControlChars.Cr Then
						lineBuf(len) = c
						len += 1
						If len = lineBuf.Length Then
							Dim newLength As Integer = lineBuf.Length * 2
							If newLength < 0 Then newLength = Integer.MaxValue
							Dim buf As Char() = New Char(newLength - 1){}
							Array.Copy(lineBuf, 0, buf, 0, lineBuf.Length)
							lineBuf = buf
						End If
						'flip the preceding backslash flag
						If c = "\"c Then
							precedingBackslash = Not precedingBackslash
						Else
							precedingBackslash = False
						End If
					Else
						' reached EOL
						If isCommentLine OrElse len = 0 Then
							isCommentLine = False
							isNewLine = True
							skipWhiteSpace = True
							len = 0
							Continue Do
						End If
						If inOff >= inLimit Then
							inLimit = If(inStream Is Nothing, reader.read(inCharBuf), inStream.read(inByteBuf))
							inOff = 0
							If inLimit <= 0 Then
								If precedingBackslash Then len -= 1
								Return len
							End If
						End If
						If precedingBackslash Then
							len -= 1
							'skip the leading whitespace characters in following line
							skipWhiteSpace = True
							appendedLineBegin = True
							precedingBackslash = False
							If c = ControlChars.Cr Then skipLF = True
						Else
							Return len
						End If
					End If
				Loop
			End Function
		End Class

	'    
	'     * Converts encoded &#92;uxxxx to unicode chars
	'     * and changes special saved chars to their original forms
	'     
		Private Function loadConvert(ByVal [in] As Char(), ByVal [off] As Integer, ByVal len As Integer, ByVal convtBuf As Char()) As String
			If convtBuf.Length < len Then
				Dim newLen As Integer = len * 2
				If newLen < 0 Then newLen = Integer.MaxValue
				convtBuf = New Char(newLen - 1){}
			End If
			Dim aChar As Char
			Dim out As Char() = convtBuf
			Dim outLen As Integer = 0
			Dim [end] As Integer = [off] + len

			Do While [off] < [end]
				aChar = [in]([off])
				[off] += 1
				If aChar = "\"c Then
					aChar = [in]([off])
					[off] += 1
					If aChar = "u"c Then
						' Read the xxxx
						Dim value As Integer=0
						For i As Integer = 0 To 3
							aChar = [in]([off])
							[off] += 1
							Select Case aChar
							  Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c
								 value = (value << 4) + AscW(aChar) - AscW("0"c)
							  Case "a"c, "b"c, "c"c, "d"c, "e"c, "f"c
								 value = (value << 4) + 10 + AscW(aChar) - AscW("a"c)
							  Case "A"c, "B"c, "C"c, "D"c, "E"c, "F"c
								 value = (value << 4) + 10 + AscW(aChar) - AscW("A"c)
							  Case Else
								  Throw New IllegalArgumentException("Malformed \uxxxx encoding.")
							End Select
						Next i
						out(outLen) = ChrW(value)
						outLen += 1
					Else
						If aChar = "t"c Then
							aChar = ControlChars.Tab
						ElseIf aChar = "r"c Then
							aChar = ControlChars.Cr
						ElseIf aChar = "n"c Then
							aChar = ControlChars.Lf
						ElseIf aChar = "f"c Then
							aChar = ControlChars.FormFeed
						End If
						out(outLen) = aChar
						outLen += 1
					End If
				Else
					out(outLen) = aChar
					outLen += 1
				End If
			Loop
			Return New String(out, 0, outLen)
		End Function

	'    
	'     * Converts unicodes to encoded &#92;uxxxx and escapes
	'     * special characters with a preceding slash
	'     
		Private Function saveConvert(ByVal theString As String, ByVal escapeSpace As Boolean, ByVal escapeUnicode As Boolean) As String
			Dim len As Integer = theString.length()
			Dim bufLen As Integer = len * 2
			If bufLen < 0 Then bufLen = Integer.MaxValue
			Dim outBuffer As New StringBuffer(bufLen)

			For x As Integer = 0 To len - 1
				Dim aChar As Char = theString.Chars(x)
				' Handle common case first, selecting largest block that
				' avoids the specials below
				If (AscW(aChar) > 61) AndAlso (AscW(aChar) < 127) Then
					If aChar = "\"c Then
						outBuffer.append("\"c)
						outBuffer.append("\"c)
						Continue For
					End If
					outBuffer.append(aChar)
					Continue For
				End If
				Select Case aChar
					Case " "c
						If x = 0 OrElse escapeSpace Then outBuffer.append("\"c)
						outBuffer.append(" "c)
					Case ControlChars.Tab
						outBuffer.append("\"c)
						outBuffer.append("t"c)
					Case ControlChars.Lf
						outBuffer.append("\"c)
						outBuffer.append("n"c)
					Case ControlChars.Cr
						outBuffer.append("\"c)
						outBuffer.append("r"c)
					Case ControlChars.FormFeed
						outBuffer.append("\"c)
						outBuffer.append("f"c)
					Case "="c, ":"c, "#"c, "!"c ' Fall through
						outBuffer.append("\"c)
						outBuffer.append(aChar)
					Case Else
						If ((AscW(aChar) < &H20) OrElse (AscW(aChar) > &H7e)) And escapeUnicode Then
							outBuffer.append("\"c)
							outBuffer.append("u"c)
							outBuffer.append(toHex((AscW(aChar) >> 12) And &HF))
							outBuffer.append(toHex((AscW(aChar) >> 8) And &HF))
							outBuffer.append(toHex((AscW(aChar) >> 4) And &HF))
							outBuffer.append(toHex(AscW(aChar) And &HF))
						Else
							outBuffer.append(aChar)
						End If
				End Select
			Next x
			Return outBuffer.ToString()
		End Function

		Private Shared Sub writeComments(ByVal bw As java.io.BufferedWriter, ByVal comments As String)
			bw.write("#")
			Dim len As Integer = comments.length()
			Dim current As Integer = 0
			Dim last As Integer = 0
			Dim uu As Char() = New Char(5){}
			uu(0) = "\"c
			uu(1) = "u"c
			Do While current < len
				Dim c As Char = comments.Chars(current)
				If c > ChrW(&H00ff) OrElse c = ControlChars.Lf OrElse c = ControlChars.Cr Then
					If last <> current Then bw.write(comments.Substring(last, current - last))
					If c > ChrW(&H00ff) Then
						uu(2) = toHex((AscW(c) >> 12) And &Hf)
						uu(3) = toHex((AscW(c) >> 8) And &Hf)
						uu(4) = toHex((AscW(c) >> 4) And &Hf)
						uu(5) = toHex(AscW(c) And &Hf)
						bw.write(New String(uu))
					Else
						bw.newLine()
						If c = ControlChars.Cr AndAlso current <> len - 1 AndAlso comments.Chars(current + 1) = ControlChars.Lf Then current += 1
						If current = len - 1 OrElse (comments.Chars(current + 1) <> "#"c AndAlso comments.Chars(current + 1) <> "!"c) Then bw.write("#")
					End If
					last = current + 1
				End If
				current += 1
			Loop
			If last <> current Then bw.write(comments.Substring(last, current - last))
			bw.newLine()
		End Sub

		''' <summary>
		''' Calls the {@code store(OutputStream out, String comments)} method
		''' and suppresses IOExceptions that were thrown.
		''' </summary>
		''' @deprecated This method does not throw an IOException if an I/O error
		''' occurs while saving the property list.  The preferred way to save a
		''' properties list is via the {@code store(OutputStream out,
		''' String comments)} method or the
		''' {@code storeToXML(OutputStream os, String comment)} method.
		''' 
		''' <param name="out">      an output stream. </param>
		''' <param name="comments">   a description of the property list. </param>
		''' <exception cref="ClassCastException">  if this {@code Properties} object
		'''             contains any keys or values that are not
		'''             {@code Strings}. </exception>
		<Obsolete("This method does not throw an IOException if an I/O error")> _
		Public Overridable Sub save(ByVal out As java.io.OutputStream, ByVal comments As String)
			Try
				store(out, comments)
			Catch e As java.io.IOException
			End Try
		End Sub

		''' <summary>
		''' Writes this property list (key and element pairs) in this
		''' {@code Properties} table to the output character stream in a
		''' format suitable for using the <seealso cref="#load(java.io.Reader) load(Reader)"/>
		''' method.
		''' <p>
		''' Properties from the defaults table of this {@code Properties}
		''' table (if any) are <i>not</i> written out by this method.
		''' <p>
		''' If the comments argument is not null, then an ASCII {@code #}
		''' character, the comments string, and a line separator are first written
		''' to the output stream. Thus, the {@code comments} can serve as an
		''' identifying comment. Any one of a line feed ('\n'), a carriage
		''' return ('\r'), or a carriage return followed immediately by a line feed
		''' in comments is replaced by a line separator generated by the {@code Writer}
		''' and if the next character in comments is not character {@code #} or
		''' character {@code !} then an ASCII {@code #} is written out
		''' after that line separator.
		''' <p>
		''' Next, a comment line is always written, consisting of an ASCII
		''' {@code #} character, the current date and time (as if produced
		''' by the {@code toString} method of {@code Date} for the
		''' current time), and a line separator as generated by the {@code Writer}.
		''' <p>
		''' Then every entry in this {@code Properties} table is
		''' written out, one per line. For each entry the key string is
		''' written, then an ASCII {@code =}, then the associated
		''' element string. For the key, all space characters are
		''' written with a preceding {@code \} character.  For the
		''' element, leading space characters, but not embedded or trailing
		''' space characters, are written with a preceding {@code \}
		''' character. The key and element characters {@code #},
		''' {@code !}, {@code =}, and {@code :} are written
		''' with a preceding backslash to ensure that they are properly loaded.
		''' <p>
		''' After the entries have been written, the output stream is flushed.
		''' The output stream remains open after this method returns.
		''' <p>
		''' </summary>
		''' <param name="writer">      an output character stream writer. </param>
		''' <param name="comments">   a description of the property list. </param>
		''' <exception cref="IOException"> if writing this property list to the specified
		'''             output stream throws an <tt>IOException</tt>. </exception>
		''' <exception cref="ClassCastException">  if this {@code Properties} object
		'''             contains any keys or values that are not {@code Strings}. </exception>
		''' <exception cref="NullPointerException">  if {@code writer} is null.
		''' @since 1.6 </exception>
		Public Overridable Sub store(ByVal writer As java.io.Writer, ByVal comments As String)
			store0(If(TypeOf writer Is java.io.BufferedWriter, CType(writer, java.io.BufferedWriter), New java.io.BufferedWriter(writer)), comments, False)
		End Sub

		''' <summary>
		''' Writes this property list (key and element pairs) in this
		''' {@code Properties} table to the output stream in a format suitable
		''' for loading into a {@code Properties} table using the
		''' <seealso cref="#load(InputStream) load(InputStream)"/> method.
		''' <p>
		''' Properties from the defaults table of this {@code Properties}
		''' table (if any) are <i>not</i> written out by this method.
		''' <p>
		''' This method outputs the comments, properties keys and values in
		''' the same format as specified in
		''' <seealso cref="#store(java.io.Writer, java.lang.String) store(Writer)"/>,
		''' with the following differences:
		''' <ul>
		''' <li>The stream is written using the ISO 8859-1 character encoding.
		''' 
		''' <li>Characters not in Latin-1 in the comments are written as
		''' {@code \u005Cu}<i>xxxx</i> for their appropriate unicode
		''' hexadecimal value <i>xxxx</i>.
		''' 
		''' <li>Characters less than {@code \u005Cu0020} and characters greater
		''' than {@code \u005Cu007E} in property keys or values are written
		''' as {@code \u005Cu}<i>xxxx</i> for the appropriate hexadecimal
		''' value <i>xxxx</i>.
		''' </ul>
		''' <p>
		''' After the entries have been written, the output stream is flushed.
		''' The output stream remains open after this method returns.
		''' <p> </summary>
		''' <param name="out">      an output stream. </param>
		''' <param name="comments">   a description of the property list. </param>
		''' <exception cref="IOException"> if writing this property list to the specified
		'''             output stream throws an <tt>IOException</tt>. </exception>
		''' <exception cref="ClassCastException">  if this {@code Properties} object
		'''             contains any keys or values that are not {@code Strings}. </exception>
		''' <exception cref="NullPointerException">  if {@code out} is null.
		''' @since 1.2 </exception>
		Public Overridable Sub store(ByVal out As java.io.OutputStream, ByVal comments As String)
			store0(New java.io.BufferedWriter(New java.io.OutputStreamWriter(out, "8859_1")), comments, True)
		End Sub

		Private Sub store0(ByVal bw As java.io.BufferedWriter, ByVal comments As String, ByVal escUnicode As Boolean)
			If comments IsNot Nothing Then writeComments(bw, comments)
			bw.write("#" & DateTime.Now.ToString())
			bw.newLine()
			SyncLock Me
				Dim e As Enumeration(Of ?) = keys()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Do While e.hasMoreElements()
					Dim key As String = CStr(e.nextElement())
					Dim val As String = CStr([get](key))
					key = saveConvert(key, True, escUnicode)
	'                 No need to escape embedded and trailing spaces for value, hence
	'                 * pass false to flag.
	'                 
					val = saveConvert(val, False, escUnicode)
					bw.write(key & "=" & val)
					bw.newLine()
				Loop
			End SyncLock
			bw.flush()
		End Sub

		''' <summary>
		''' Loads all of the properties represented by the XML document on the
		''' specified input stream into this properties table.
		''' 
		''' <p>The XML document must have the following DOCTYPE declaration:
		''' <pre>
		''' &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
		''' </pre>
		''' Furthermore, the document must satisfy the properties DTD described
		''' above.
		''' 
		''' <p> An implementation is required to read XML documents that use the
		''' "{@code UTF-8}" or "{@code UTF-16}" encoding. An implementation may
		''' support additional encodings.
		''' 
		''' <p>The specified stream is closed after this method returns.
		''' </summary>
		''' <param name="in"> the input stream from which to read the XML document. </param>
		''' <exception cref="IOException"> if reading from the specified input stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="java.io.UnsupportedEncodingException"> if the document's encoding
		'''         declaration can be read and it specifies an encoding that is not
		'''         supported </exception>
		''' <exception cref="InvalidPropertiesFormatException"> Data on input stream does not
		'''         constitute a valid XML document with the mandated document type. </exception>
		''' <exception cref="NullPointerException"> if {@code in} is null. </exception>
		''' <seealso cref=    #storeToXML(OutputStream, String, String) </seealso>
		''' <seealso cref=    <a href="http://www.w3.org/TR/REC-xml/#charencoding">Character
		'''         Encoding in Entities</a>
		''' @since 1.5 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub loadFromXML(ByVal [in] As java.io.InputStream)
			XmlSupport.load(Me, Objects.requireNonNull([in]))
			[in].close()
		End Sub

		''' <summary>
		''' Emits an XML document representing all of the properties contained
		''' in this table.
		''' 
		''' <p> An invocation of this method of the form <tt>props.storeToXML(os,
		''' comment)</tt> behaves in exactly the same way as the invocation
		''' <tt>props.storeToXML(os, comment, "UTF-8");</tt>.
		''' </summary>
		''' <param name="os"> the output stream on which to emit the XML document. </param>
		''' <param name="comment"> a description of the property list, or {@code null}
		'''        if no comment is desired. </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="NullPointerException"> if {@code os} is null. </exception>
		''' <exception cref="ClassCastException">  if this {@code Properties} object
		'''         contains any keys or values that are not
		'''         {@code Strings}. </exception>
		''' <seealso cref=    #loadFromXML(InputStream)
		''' @since 1.5 </seealso>
		Public Overridable Sub storeToXML(ByVal os As java.io.OutputStream, ByVal comment As String)
			storeToXML(os, comment, "UTF-8")
		End Sub

		''' <summary>
		''' Emits an XML document representing all of the properties contained
		''' in this table, using the specified encoding.
		''' 
		''' <p>The XML document will have the following DOCTYPE declaration:
		''' <pre>
		''' &lt;!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd"&gt;
		''' </pre>
		''' 
		''' <p>If the specified comment is {@code null} then no comment
		''' will be stored in the document.
		''' 
		''' <p> An implementation is required to support writing of XML documents
		''' that use the "{@code UTF-8}" or "{@code UTF-16}" encoding. An
		''' implementation may support additional encodings.
		''' 
		''' <p>The specified stream remains open after this method returns.
		''' </summary>
		''' <param name="os">        the output stream on which to emit the XML document. </param>
		''' <param name="comment">   a description of the property list, or {@code null}
		'''                  if no comment is desired. </param>
		''' <param name="encoding"> the name of a supported
		'''                  <a href="../lang/package-summary.html#charenc">
		'''                  character encoding</a>
		''' </param>
		''' <exception cref="IOException"> if writing to the specified output stream
		'''         results in an <tt>IOException</tt>. </exception>
		''' <exception cref="java.io.UnsupportedEncodingException"> if the encoding is not
		'''         supported by the implementation. </exception>
		''' <exception cref="NullPointerException"> if {@code os} is {@code null},
		'''         or if {@code encoding} is {@code null}. </exception>
		''' <exception cref="ClassCastException">  if this {@code Properties} object
		'''         contains any keys or values that are not
		'''         {@code Strings}. </exception>
		''' <seealso cref=    #loadFromXML(InputStream) </seealso>
		''' <seealso cref=    <a href="http://www.w3.org/TR/REC-xml/#charencoding">Character
		'''         Encoding in Entities</a>
		''' @since 1.5 </seealso>
		Public Overridable Sub storeToXML(ByVal os As java.io.OutputStream, ByVal comment As String, ByVal encoding As String)
			XmlSupport.save(Me, Objects.requireNonNull(os), comment, Objects.requireNonNull(encoding))
		End Sub

		''' <summary>
		''' Searches for the property with the specified key in this property list.
		''' If the key is not found in this property list, the default property list,
		''' and its defaults, recursively, are then checked. The method returns
		''' {@code null} if the property is not found.
		''' </summary>
		''' <param name="key">   the property key. </param>
		''' <returns>  the value in this property list with the specified key value. </returns>
		''' <seealso cref=     #setProperty </seealso>
		''' <seealso cref=     #defaults </seealso>
		Public Overridable Function getProperty(ByVal key As String) As String
			Dim oval As Object = MyBase.get(key)
			Dim sval As String = If(TypeOf oval Is String, CStr(oval), Nothing)
			Return If((sval Is Nothing) AndAlso (defaults IsNot Nothing), defaults.getProperty(key), sval)
		End Function

		''' <summary>
		''' Searches for the property with the specified key in this property list.
		''' If the key is not found in this property list, the default property list,
		''' and its defaults, recursively, are then checked. The method returns the
		''' default value argument if the property is not found.
		''' </summary>
		''' <param name="key">            the hashtable key. </param>
		''' <param name="defaultValue">   a default value.
		''' </param>
		''' <returns>  the value in this property list with the specified key value. </returns>
		''' <seealso cref=     #setProperty </seealso>
		''' <seealso cref=     #defaults </seealso>
		Public Overridable Function getProperty(ByVal key As String, ByVal defaultValue As String) As String
			Dim val As String = getProperty(key)
			Return If(val Is Nothing, defaultValue, val)
		End Function

		''' <summary>
		''' Returns an enumeration of all the keys in this property list,
		''' including distinct keys in the default property list if a key
		''' of the same name has not already been found from the main
		''' properties list.
		''' </summary>
		''' <returns>  an enumeration of all the keys in this property list, including
		'''          the keys in the default property list. </returns>
		''' <exception cref="ClassCastException"> if any key in this property list
		'''          is not a string. </exception>
		''' <seealso cref=     java.util.Enumeration </seealso>
		''' <seealso cref=     java.util.Properties#defaults </seealso>
		''' <seealso cref=     #stringPropertyNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function propertyNames() As Enumeration(Of ?)
			Dim h As New Dictionary(Of String, Object)
			enumerate(h)
			Return h.keys()
		End Function

		''' <summary>
		''' Returns a set of keys in this property list where
		''' the key and its corresponding value are strings,
		''' including distinct keys in the default property list if a key
		''' of the same name has not already been found from the main
		''' properties list.  Properties whose key or value is not
		''' of type <tt>String</tt> are omitted.
		''' <p>
		''' The returned set is not backed by the <tt>Properties</tt> object.
		''' Changes to this <tt>Properties</tt> are not reflected in the set,
		''' or vice versa.
		''' </summary>
		''' <returns>  a set of keys in this property list where
		'''          the key and its corresponding value are strings,
		'''          including the keys in the default property list. </returns>
		''' <seealso cref=     java.util.Properties#defaults
		''' @since   1.6 </seealso>
		Public Overridable Function stringPropertyNames() As [Set](Of String)
			Dim h As New Dictionary(Of String, String)
			enumerateStringProperties(h)
			Return h.Keys
		End Function

		''' <summary>
		''' Prints this property list out to the specified output stream.
		''' This method is useful for debugging.
		''' </summary>
		''' <param name="out">   an output stream. </param>
		''' <exception cref="ClassCastException"> if any key in this property list
		'''          is not a string. </exception>
		Public Overridable Sub list(ByVal out As java.io.PrintStream)
			out.println("-- listing properties --")
			Dim h As New Dictionary(Of String, Object)
			enumerate(h)
			Dim e As Enumeration(Of String) = h.keys()
			Do While e.hasMoreElements()
				Dim key As String = e.nextElement()
				Dim val As String = CStr(h.get(key))
				If val.length() > 40 Then val = val.Substring(0, 37) & "..."
				out.println(key & "=" & val)
			Loop
		End Sub

		''' <summary>
		''' Prints this property list out to the specified output stream.
		''' This method is useful for debugging.
		''' </summary>
		''' <param name="out">   an output stream. </param>
		''' <exception cref="ClassCastException"> if any key in this property list
		'''          is not a string.
		''' @since   JDK1.1 </exception>
	'    
	'     * Rather than use an anonymous inner class to share common code, this
	'     * method is duplicated in order to ensure that a non-1.1 compiler can
	'     * compile this file.
	'     
		Public Overridable Sub list(ByVal out As java.io.PrintWriter)
			out.println("-- listing properties --")
			Dim h As New Dictionary(Of String, Object)
			enumerate(h)
			Dim e As Enumeration(Of String) = h.keys()
			Do While e.hasMoreElements()
				Dim key As String = e.nextElement()
				Dim val As String = CStr(h.get(key))
				If val.length() > 40 Then val = val.Substring(0, 37) & "..."
				out.println(key & "=" & val)
			Loop
		End Sub

		''' <summary>
		''' Enumerates all key/value pairs in the specified hashtable. </summary>
		''' <param name="h"> the hashtable </param>
		''' <exception cref="ClassCastException"> if any of the property keys
		'''         is not of String type. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub enumerate(ByVal h As Dictionary(Of String, Object))
			If defaults IsNot Nothing Then defaults.enumerate(h)
			Dim e As Enumeration(Of ?) = keys()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While e.hasMoreElements()
				Dim key As String = CStr(e.nextElement())
				h.put(key, [get](key))
			Loop
		End Sub

		''' <summary>
		''' Enumerates all key/value pairs in the specified hashtable
		''' and omits the property if the key or value is not a string. </summary>
		''' <param name="h"> the hashtable </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub enumerateStringProperties(ByVal h As Dictionary(Of String, String))
			If defaults IsNot Nothing Then defaults.enumerateStringProperties(h)
			Dim e As Enumeration(Of ?) = keys()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Do While e.hasMoreElements()
				Dim k As Object = e.nextElement()
				Dim v As Object = [get](k)
				If TypeOf k Is String AndAlso TypeOf v Is String Then h.put(CStr(k), CStr(v))
			Loop
		End Sub

		''' <summary>
		''' Convert a nibble to a hex character </summary>
		''' <param name="nibble">  the nibble to convert. </param>
		Private Shared Function toHex(ByVal nibble As Integer) As Char
			Return hexDigit((nibble And &HF))
		End Function

		''' <summary>
		''' A table of hex digits </summary>
		Private Shared ReadOnly hexDigit As Char() = { "0"c,"1"c,"2"c,"3"c,"4"c,"5"c,"6"c,"7"c,"8"c,"9"c,"A"c,"B"c,"C"c,"D"c,"E"c,"F"c }

		''' <summary>
		''' Supporting class for loading/storing properties in XML format.
		''' 
		''' <p> The {@code load} and {@code store} methods defined here delegate to a
		''' system-wide {@code XmlPropertiesProvider}. On first invocation of either
		''' method then the system-wide provider is located as follows: </p>
		''' 
		''' <ol>
		'''   <li> If the system property {@code sun.util.spi.XmlPropertiesProvider}
		'''   is defined then it is taken to be the full-qualified name of a concrete
		'''   provider class. The class is loaded with the system class loader as the
		'''   initiating loader. If it cannot be loaded or instantiated using a zero
		'''   argument constructor then an unspecified error is thrown. </li>
		''' 
		'''   <li> If the system property is not defined then the service-provider
		'''   loading facility defined by the <seealso cref="ServiceLoader"/> class is used to
		'''   locate a provider with the system class loader as the initiating
		'''   loader and {@code sun.util.spi.XmlPropertiesProvider} as the service
		'''   type. If this process fails then an unspecified error is thrown. If
		'''   there is more than one service provider installed then it is
		'''   not specified as to which provider will be used. </li>
		''' 
		'''   <li> If the provider is not found by the above means then a system
		'''   default provider will be instantiated and used. </li>
		''' </ol>
		''' </summary>
		Private Class XmlSupport

			Private Shared Function loadProviderFromProperty(ByVal cl As ClassLoader) As sun.util.spi.XmlPropertiesProvider
				Dim cn As String = System.getProperty("sun.util.spi.XmlPropertiesProvider")
				If cn Is Nothing Then Return Nothing
				Try
					Dim c As Class = Type.GetType(cn, True, cl)
					Return CType(c.newInstance(), sun.util.spi.XmlPropertiesProvider)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch ClassNotFoundException Or IllegalAccessException Or InstantiationException x
					Throw New ServiceConfigurationError(Nothing, x)
				End Try
			End Function

			Private Shared Function loadProviderAsService(ByVal cl As ClassLoader) As sun.util.spi.XmlPropertiesProvider
				Dim [iterator] As [Iterator](Of sun.util.spi.XmlPropertiesProvider) = ServiceLoader.load(GetType(sun.util.spi.XmlPropertiesProvider), cl).GetEnumerator()
				Return If([iterator].hasNext(), [iterator].next(), Nothing)
			End Function

			Private Shared Function loadProvider() As sun.util.spi.XmlPropertiesProvider
				Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As sun.util.spi.XmlPropertiesProvider
					Dim cl As ClassLoader = ClassLoader.systemClassLoader
					Dim provider As sun.util.spi.XmlPropertiesProvider = loadProviderFromProperty(cl)
					If provider IsNot Nothing Then Return provider
					provider = loadProviderAsService(cl)
					If provider IsNot Nothing Then Return provider
					Return New jdk.internal.util.xml.BasicXmlPropertiesProvider
				End Function
			End Class

			Private Shared ReadOnly PROVIDER As sun.util.spi.XmlPropertiesProvider = loadProvider()

			Friend Shared Sub load(ByVal props As Properties, ByVal [in] As java.io.InputStream)
				PROVIDER.load(props, [in])
			End Sub

			Friend Shared Sub save(ByVal props As Properties, ByVal os As java.io.OutputStream, ByVal comment As String, ByVal encoding As String)
				PROVIDER.store(props, os, comment, encoding)
			End Sub
		End Class
	End Class

End Namespace