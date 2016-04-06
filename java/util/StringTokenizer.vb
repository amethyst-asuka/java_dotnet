Imports Microsoft.VisualBasic

'
' * Copyright (c) 1994, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' The string tokenizer class allows an application to break a
	''' string into tokens. The tokenization method is much simpler than
	''' the one used by the <code>StreamTokenizer</code> class. The
	''' <code>StringTokenizer</code> methods do not distinguish among
	''' identifiers, numbers, and quoted strings, nor do they recognize
	''' and skip comments.
	''' <p>
	''' The set of delimiters (the characters that separate tokens) may
	''' be specified either at creation time or on a per-token basis.
	''' <p>
	''' An instance of <code>StringTokenizer</code> behaves in one of two
	''' ways, depending on whether it was created with the
	''' <code>returnDelims</code> flag having the value <code>true</code>
	''' or <code>false</code>:
	''' <ul>
	''' <li>If the flag is <code>false</code>, delimiter characters serve to
	'''     separate tokens. A token is a maximal sequence of consecutive
	'''     characters that are not delimiters.
	''' <li>If the flag is <code>true</code>, delimiter characters are themselves
	'''     considered to be tokens. A token is thus either one delimiter
	'''     character, or a maximal sequence of consecutive characters that are
	'''     not delimiters.
	''' </ul><p>
	''' A <tt>StringTokenizer</tt> object internally maintains a current
	''' position within the string to be tokenized. Some operations advance this
	''' current position past the characters processed.<p>
	''' A token is returned by taking a substring of the string that was used to
	''' create the <tt>StringTokenizer</tt> object.
	''' <p>
	''' The following is one example of the use of the tokenizer. The code:
	''' <blockquote><pre>
	'''     StringTokenizer st = new StringTokenizer("this is a test");
	'''     while (st.hasMoreTokens()) {
	'''         System.out.println(st.nextToken());
	'''     }
	''' </pre></blockquote>
	''' <p>
	''' prints the following output:
	''' <blockquote><pre>
	'''     this
	'''     is
	'''     a
	'''     test
	''' </pre></blockquote>
	''' 
	''' <p>
	''' <tt>StringTokenizer</tt> is a legacy class that is retained for
	''' compatibility reasons although its use is discouraged in new code. It is
	''' recommended that anyone seeking this functionality use the <tt>split</tt>
	''' method of <tt>String</tt> or the java.util.regex package instead.
	''' <p>
	''' The following example illustrates how the <tt>String.split</tt>
	''' method can be used to break up a string into its basic tokens:
	''' <blockquote><pre>
	'''     String[] result = "this is a test".split("\\s");
	'''     for (int x=0; x&lt;result.length; x++)
	'''         System.out.println(result[x]);
	''' </pre></blockquote>
	''' <p>
	''' prints the following output:
	''' <blockquote><pre>
	'''     this
	'''     is
	'''     a
	'''     test
	''' </pre></blockquote>
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.io.StreamTokenizer
	''' @since   JDK1.0 </seealso>
	Public Class StringTokenizer
		Implements Enumeration(Of Object)

		Private currentPosition As Integer
		Private newPosition As Integer
		Private maxPosition As Integer
		Private str As String
		Private delimiters As String
		Private retDelims As Boolean
		Private delimsChanged As Boolean

		''' <summary>
		''' maxDelimCodePoint stores the value of the delimiter character with the
		''' highest value. It is used to optimize the detection of delimiter
		''' characters.
		''' 
		''' It is unlikely to provide any optimization benefit in the
		''' hasSurrogates case because most string characters will be
		''' smaller than the limit, but we keep it so that the two code
		''' paths remain similar.
		''' </summary>
		Private maxDelimCodePoint As Integer

		''' <summary>
		''' If delimiters include any surrogates (including surrogate
		''' pairs), hasSurrogates is true and the tokenizer uses the
		''' different code path. This is because String.indexOf(int)
		''' doesn't handle unpaired surrogates as a single character.
		''' </summary>
		Private hasSurrogates As Boolean = False

		''' <summary>
		''' When hasSurrogates is true, delimiters are converted to code
		''' points and isDelimiter(int) is used to determine if the given
		''' codepoint is a delimiter.
		''' </summary>
		Private delimiterCodePoints As Integer()

		''' <summary>
		''' Set maxDelimCodePoint to the highest char in the delimiter set.
		''' </summary>
		Private Sub setMaxDelimCodePoint()
			If delimiters Is Nothing Then
				maxDelimCodePoint = 0
				Return
			End If

			Dim m As Integer = 0
			Dim c As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To delimiters.length() - 1 Step Character.charCount(c)
				c = AscW(delimiters.Chars(i))
				If c >= Character.MIN_HIGH_SURROGATE AndAlso c <= Character.MAX_LOW_SURROGATE Then
					c = delimiters.codePointAt(i)
					hasSurrogates = True
				End If
				If m < c Then m = c
				count += 1
			Next i
			maxDelimCodePoint = m

			If hasSurrogates Then
				delimiterCodePoints = New Integer(count - 1){}
				Dim i As Integer = 0
				Dim j As Integer = 0
				Do While i < count
					c = delimiters.codePointAt(j)
					delimiterCodePoints(i) = c
					i += 1
					j += Character.charCount(c)
				Loop
			End If
		End Sub

		''' <summary>
		''' Constructs a string tokenizer for the specified string. All
		''' characters in the <code>delim</code> argument are the delimiters
		''' for separating tokens.
		''' <p>
		''' If the <code>returnDelims</code> flag is <code>true</code>, then
		''' the delimiter characters are also returned as tokens. Each
		''' delimiter is returned as a string of length one. If the flag is
		''' <code>false</code>, the delimiter characters are skipped and only
		''' serve as separators between tokens.
		''' <p>
		''' Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
		''' not throw an exception. However, trying to invoke other methods on the
		''' resulting <tt>StringTokenizer</tt> may result in a
		''' <tt>NullPointerException</tt>.
		''' </summary>
		''' <param name="str">            a string to be parsed. </param>
		''' <param name="delim">          the delimiters. </param>
		''' <param name="returnDelims">   flag indicating whether to return the delimiters
		'''                         as tokens. </param>
		''' <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		Public Sub New(  str As String,   delim As String,   returnDelims As Boolean)
			currentPosition = 0
			newPosition = -1
			delimsChanged = False
			Me.str = str
			maxPosition = str.length()
			delimiters = delim
			retDelims = returnDelims
			maxDelimCodePointint()
		End Sub

		''' <summary>
		''' Constructs a string tokenizer for the specified string. The
		''' characters in the <code>delim</code> argument are the delimiters
		''' for separating tokens. Delimiter characters themselves will not
		''' be treated as tokens.
		''' <p>
		''' Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
		''' not throw an exception. However, trying to invoke other methods on the
		''' resulting <tt>StringTokenizer</tt> may result in a
		''' <tt>NullPointerException</tt>.
		''' </summary>
		''' <param name="str">     a string to be parsed. </param>
		''' <param name="delim">   the delimiters. </param>
		''' <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		Public Sub New(  str As String,   delim As String)
			Me.New(str, delim, False)
		End Sub

		''' <summary>
		''' Constructs a string tokenizer for the specified string. The
		''' tokenizer uses the default delimiter set, which is
		''' <code>"&nbsp;&#92;t&#92;n&#92;r&#92;f"</code>: the space character,
		''' the tab character, the newline character, the carriage-return character,
		''' and the form-feed character. Delimiter characters themselves will
		''' not be treated as tokens.
		''' </summary>
		''' <param name="str">   a string to be parsed. </param>
		''' <exception cref="NullPointerException"> if str is <CODE>null</CODE> </exception>
		Public Sub New(  str As String)
			Me.New(str, " " & vbTab & vbLf & vbCr & vbFormFeed, False)
		End Sub

		''' <summary>
		''' Skips delimiters starting from the specified position. If retDelims
		''' is false, returns the index of the first non-delimiter character at or
		''' after startPos. If retDelims is true, startPos is returned.
		''' </summary>
		Private Function skipDelimiters(  startPos As Integer) As Integer
			If delimiters Is Nothing Then Throw New NullPointerException

			Dim position As Integer = startPos
			Do While (Not retDelims) AndAlso position < maxPosition
				If Not hasSurrogates Then
					Dim c As Char = str.Chars(position)
					If (AscW(c) > maxDelimCodePoint) OrElse (delimiters.IndexOf(c) < 0) Then Exit Do
					position += 1
				Else
					Dim c As Integer = str.codePointAt(position)
					If (c > maxDelimCodePoint) OrElse (Not isDelimiter(c)) Then Exit Do
					position += Character.charCount(c)
				End If
			Loop
			Return position
		End Function

		''' <summary>
		''' Skips ahead from startPos and returns the index of the next delimiter
		''' character encountered, or maxPosition if no such delimiter is found.
		''' </summary>
		Private Function scanToken(  startPos As Integer) As Integer
			Dim position As Integer = startPos
			Do While position < maxPosition
				If Not hasSurrogates Then
					Dim c As Char = str.Chars(position)
					If (c <= maxDelimCodePoint) AndAlso (delimiters.IndexOf(c) >= 0) Then Exit Do
					position += 1
				Else
					Dim c As Integer = str.codePointAt(position)
					If (c <= maxDelimCodePoint) AndAlso isDelimiter(c) Then Exit Do
					position += Character.charCount(c)
				End If
			Loop
			If retDelims AndAlso (startPos = position) Then
				If Not hasSurrogates Then
					Dim c As Char = str.Chars(position)
					If (c <= maxDelimCodePoint) AndAlso (delimiters.IndexOf(c) >= 0) Then position += 1
				Else
					Dim c As Integer = str.codePointAt(position)
					If (c <= maxDelimCodePoint) AndAlso isDelimiter(c) Then position += Character.charCount(c)
				End If
			End If
			Return position
		End Function

		Private Function isDelimiter(  codePoint As Integer) As Boolean
			For i As Integer = 0 To delimiterCodePoints.Length - 1
				If delimiterCodePoints(i) = codePoint Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Tests if there are more tokens available from this tokenizer's string.
		''' If this method returns <tt>true</tt>, then a subsequent call to
		''' <tt>nextToken</tt> with no argument will successfully return a token.
		''' </summary>
		''' <returns>  <code>true</code> if and only if there is at least one token
		'''          in the string after the current position; <code>false</code>
		'''          otherwise. </returns>
		Public Overridable Function hasMoreTokens() As Boolean
	'        
	'         * Temporarily store this position and use it in the following
	'         * nextToken() method only if the delimiters haven't been changed in
	'         * that nextToken() invocation.
	'         
			newPosition = skipDelimiters(currentPosition)
			Return (newPosition < maxPosition)
		End Function

		''' <summary>
		''' Returns the next token from this string tokenizer.
		''' </summary>
		''' <returns>     the next token from this string tokenizer. </returns>
		''' <exception cref="NoSuchElementException">  if there are no more tokens in this
		'''               tokenizer's string. </exception>
		Public Overridable Function nextToken() As String
	'        
	'         * If next position already computed in hasMoreElements() and
	'         * delimiters have changed between the computation and this invocation,
	'         * then use the computed value.
	'         

			currentPosition = If(newPosition >= 0 AndAlso (Not delimsChanged), newPosition, skipDelimiters(currentPosition))

			' Reset these anyway 
			delimsChanged = False
			newPosition = -1

			If currentPosition >= maxPosition Then Throw New NoSuchElementException
			Dim start As Integer = currentPosition
			currentPosition = scanToken(currentPosition)
			Return str.Substring(start, currentPosition - start)
		End Function

		''' <summary>
		''' Returns the next token in this string tokenizer's string. First,
		''' the set of characters considered to be delimiters by this
		''' <tt>StringTokenizer</tt> object is changed to be the characters in
		''' the string <tt>delim</tt>. Then the next token in the string
		''' after the current position is returned. The current position is
		''' advanced beyond the recognized token.  The new delimiter set
		''' remains the default after this call.
		''' </summary>
		''' <param name="delim">   the new delimiters. </param>
		''' <returns>     the next token, after switching to the new delimiter set. </returns>
		''' <exception cref="NoSuchElementException">  if there are no more tokens in this
		'''               tokenizer's string. </exception>
		''' <exception cref="NullPointerException"> if delim is <CODE>null</CODE> </exception>
		Public Overridable Function nextToken(  delim As String) As String
			delimiters = delim

			' delimiter string specified, so set the appropriate flag. 
			delimsChanged = True

			maxDelimCodePointint()
			Return nextToken()
		End Function

		''' <summary>
		''' Returns the same value as the <code>hasMoreTokens</code>
		''' method. It exists so that this class can implement the
		''' <code>Enumeration</code> interface.
		''' </summary>
		''' <returns>  <code>true</code> if there are more tokens;
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.util.Enumeration </seealso>
		''' <seealso cref=     java.util.StringTokenizer#hasMoreTokens() </seealso>
		Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of Object).hasMoreElements
			Return hasMoreTokens()
		End Function

		''' <summary>
		''' Returns the same value as the <code>nextToken</code> method,
		''' except that its declared return value is <code>Object</code> rather than
		''' <code>String</code>. It exists so that this class can implement the
		''' <code>Enumeration</code> interface.
		''' </summary>
		''' <returns>     the next token in the string. </returns>
		''' <exception cref="NoSuchElementException">  if there are no more tokens in this
		'''               tokenizer's string. </exception>
		''' <seealso cref=        java.util.Enumeration </seealso>
		''' <seealso cref=        java.util.StringTokenizer#nextToken() </seealso>
		Public Overridable Function nextElement() As Object
			Return nextToken()
		End Function

		''' <summary>
		''' Calculates the number of times that this tokenizer's
		''' <code>nextToken</code> method can be called before it generates an
		''' exception. The current position is not advanced.
		''' </summary>
		''' <returns>  the number of tokens remaining in the string using the current
		'''          delimiter set. </returns>
		''' <seealso cref=     java.util.StringTokenizer#nextToken() </seealso>
		Public Overridable Function countTokens() As Integer
			Dim count As Integer = 0
			Dim currpos As Integer = currentPosition
			Do While currpos < maxPosition
				currpos = skipDelimiters(currpos)
				If currpos >= maxPosition Then Exit Do
				currpos = scanToken(currpos)
				count += 1
			Loop
			Return count
		End Function
	End Class

End Namespace