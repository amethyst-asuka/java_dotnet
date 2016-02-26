Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


	''' <summary>
	''' The {@code StreamTokenizer} class takes an input stream and
	''' parses it into "tokens", allowing the tokens to be
	''' read one at a time. The parsing process is controlled by a table
	''' and a number of flags that can be set to various states. The
	''' stream tokenizer can recognize identifiers, numbers, quoted
	''' strings, and various comment styles.
	''' <p>
	''' Each byte read from the input stream is regarded as a character
	''' in the range {@code '\u005Cu0000'} through {@code '\u005Cu00FF'}.
	''' The character value is used to look up five possible attributes of
	''' the character: <i>white space</i>, <i>alphabetic</i>,
	''' <i>numeric</i>, <i>string quote</i>, and <i>comment character</i>.
	''' Each character can have zero or more of these attributes.
	''' <p>
	''' In addition, an instance has four flags. These flags indicate:
	''' <ul>
	''' <li>Whether line terminators are to be returned as tokens or treated
	'''     as white space that merely separates tokens.
	''' <li>Whether C-style comments are to be recognized and skipped.
	''' <li>Whether C++-style comments are to be recognized and skipped.
	''' <li>Whether the characters of identifiers are converted to lowercase.
	''' </ul>
	''' <p>
	''' A typical application first constructs an instance of this [Class],
	''' sets up the syntax tables, and then repeatedly loops calling the
	''' {@code nextToken} method in each iteration of the loop until
	''' it returns the value {@code TT_EOF}.
	''' 
	''' @author  James Gosling </summary>
	''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
	''' <seealso cref=     java.io.StreamTokenizer#TT_EOF
	''' @since   JDK1.0 </seealso>

	Public Class StreamTokenizer

		' Only one of these will be non-null 
		Private reader As Reader = Nothing
		Private input As InputStream = Nothing

		Private buf As Char() = New Char(19){}

		''' <summary>
		''' The next character to be considered by the nextToken method.  May also
		''' be NEED_CHAR to indicate that a new character should be read, or SKIP_LF
		''' to indicate that a new character should be read and, if it is a '\n'
		''' character, it should be discarded and a second new character should be
		''' read.
		''' </summary>
		Private peekc As Integer = NEED_CHAR

		Private Shared ReadOnly NEED_CHAR As Integer =  [Integer].MAX_VALUE
		Private Shared ReadOnly SKIP_LF As Integer =  [Integer].MAX_VALUE - 1

		Private pushedBack As Boolean
		Private forceLower As Boolean
		''' <summary>
		''' The line number of the last token read </summary>
		Private LINENO_Renamed As Integer = 1

		Private eolIsSignificantP As Boolean = False
		Private slashSlashCommentsP As Boolean = False
		Private slashStarCommentsP As Boolean = False

		Private [ctype] As SByte() = New SByte(255){}
		Private Const CT_WHITESPACE As SByte = 1
		Private Const CT_DIGIT As SByte = 2
		Private Const CT_ALPHA As SByte = 4
		Private Const CT_QUOTE As SByte = 8
		Private Const CT_COMMENT As SByte = 16

		''' <summary>
		''' After a call to the {@code nextToken} method, this field
		''' contains the type of the token just read. For a single character
		''' token, its value is the single character, converted to an integer.
		''' For a quoted string token, its value is the quote character.
		''' Otherwise, its value is one of the following:
		''' <ul>
		''' <li>{@code TT_WORD} indicates that the token is a word.
		''' <li>{@code TT_NUMBER} indicates that the token is a number.
		''' <li>{@code TT_EOL} indicates that the end of line has been read.
		'''     The field can only have this value if the
		'''     {@code eolIsSignificant} method has been called with the
		'''     argument {@code true}.
		''' <li>{@code TT_EOF} indicates that the end of the input stream
		'''     has been reached.
		''' </ul>
		''' <p>
		''' The initial value of this field is -4.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#eolIsSignificant(boolean) </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#quoteChar(int) </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_EOF </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_EOL </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		Public ttype As Integer = TT_NOTHING

		''' <summary>
		''' A constant indicating that the end of the stream has been read.
		''' </summary>
		Public Const TT_EOF As Integer = -1

		''' <summary>
		''' A constant indicating that the end of the line has been read.
		''' </summary>
		Public Shared ReadOnly TT_EOL As Integer = ControlChars.Lf

		''' <summary>
		''' A constant indicating that a number token has been read.
		''' </summary>
		Public Const TT_NUMBER As Integer = -2

		''' <summary>
		''' A constant indicating that a word token has been read.
		''' </summary>
		Public Const TT_WORD As Integer = -3

	'     A constant indicating that no token has been read, used for
	'     * initializing ttype.  FIXME This could be made public and
	'     * made available as the part of the API in a future release.
	'     
		Private Const TT_NOTHING As Integer = -4

		''' <summary>
		''' If the current token is a word token, this field contains a
		''' string giving the characters of the word token. When the current
		''' token is a quoted string token, this field contains the body of
		''' the string.
		''' <p>
		''' The current token is a word when the value of the
		''' {@code ttype} field is {@code TT_WORD}. The current token is
		''' a quoted string token when the value of the {@code ttype} field is
		''' a quote character.
		''' <p>
		''' The initial value of this field is null.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#quoteChar(int) </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public sval As String

		''' <summary>
		''' If the current token is a number, this field contains the value
		''' of that number. The current token is a number when the value of
		''' the {@code ttype} field is {@code TT_NUMBER}.
		''' <p>
		''' The initial value of this field is 0.0.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public nval As Double

		''' <summary>
		''' Private constructor that initializes everything except the streams. </summary>
		Private Sub New()
			wordChars("a"c, "z"c)
			wordChars("A"c, "Z"c)
			wordChars(128 + 32, 255)
			whitespaceChars(0, " "c)
			commentChar("/"c)
			quoteChar(""""c)
			quoteChar("'"c)
			parseNumbers()
		End Sub

		''' <summary>
		''' Creates a stream tokenizer that parses the specified input
		''' stream. The stream tokenizer is initialized to the following
		''' default state:
		''' <ul>
		''' <li>All byte values {@code 'A'} through {@code 'Z'},
		'''     {@code 'a'} through {@code 'z'}, and
		'''     {@code '\u005Cu00A0'} through {@code '\u005Cu00FF'} are
		'''     considered to be alphabetic.
		''' <li>All byte values {@code '\u005Cu0000'} through
		'''     {@code '\u005Cu0020'} are considered to be white space.
		''' <li>{@code '/'} is a comment character.
		''' <li>Single quote {@code '\u005C''} and double quote {@code '"'}
		'''     are string quote characters.
		''' <li>Numbers are parsed.
		''' <li>Ends of lines are treated as white space, not as separate tokens.
		''' <li>C-style and C++-style comments are not recognized.
		''' </ul>
		''' </summary>
		''' @deprecated As of JDK version 1.1, the preferred way to tokenize an
		''' input stream is to convert it into a character stream, for example:
		''' <blockquote><pre>
		'''   Reader r = new BufferedReader(new InputStreamReader(is));
		'''   StreamTokenizer st = new StreamTokenizer(r);
		''' </pre></blockquote>
		''' 
		''' <param name="is">        an input stream. </param>
		''' <seealso cref=        java.io.BufferedReader </seealso>
		''' <seealso cref=        java.io.InputStreamReader </seealso>
		''' <seealso cref=        java.io.StreamTokenizer#StreamTokenizer(java.io.Reader) </seealso>
		<Obsolete("As of JDK version 1.1, the preferred way to tokenize an")> _
		Public Sub New(ByVal [is] As InputStream)
			Me.New()
			If [is] Is Nothing Then Throw New NullPointerException
			input = [is]
		End Sub

		''' <summary>
		''' Create a tokenizer that parses the given character stream.
		''' </summary>
		''' <param name="r">  a Reader object providing the input stream.
		''' @since   JDK1.1 </param>
		Public Sub New(ByVal r As Reader)
			Me.New()
			If r Is Nothing Then Throw New NullPointerException
			reader = r
		End Sub

		''' <summary>
		''' Resets this tokenizer's syntax table so that all characters are
		''' "ordinary." See the {@code ordinaryChar} method
		''' for more information on a character being ordinary.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#ordinaryChar(int) </seealso>
		Public Overridable Sub resetSyntax()
			Dim i As Integer = [ctype].Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 >= 0
				[ctype](i) = 0
			Loop
		End Sub

		''' <summary>
		''' Specifies that all characters <i>c</i> in the range
		''' <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		''' are word constituents. A word token consists of a word constituent
		''' followed by zero or more word constituents or number constituents.
		''' </summary>
		''' <param name="low">   the low end of the range. </param>
		''' <param name="hi">    the high end of the range. </param>
		Public Overridable Sub wordChars(ByVal low As Integer, ByVal hi As Integer)
			If low < 0 Then low = 0
			If hi >= [ctype].Length Then hi = [ctype].Length - 1
			Do While low <= hi
				[ctype](low) = [ctype](low) Or CT_ALPHA
				low += 1
			Loop
		End Sub

		''' <summary>
		''' Specifies that all characters <i>c</i> in the range
		''' <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		''' are white space characters. White space characters serve only to
		''' separate tokens in the input stream.
		''' 
		''' <p>Any other attribute settings for the characters in the specified
		''' range are cleared.
		''' </summary>
		''' <param name="low">   the low end of the range. </param>
		''' <param name="hi">    the high end of the range. </param>
		Public Overridable Sub whitespaceChars(ByVal low As Integer, ByVal hi As Integer)
			If low < 0 Then low = 0
			If hi >= [ctype].Length Then hi = [ctype].Length - 1
			Do While low <= hi
				[ctype](low) = CT_WHITESPACE
				low += 1
			Loop
		End Sub

		''' <summary>
		''' Specifies that all characters <i>c</i> in the range
		''' <code>low&nbsp;&lt;=&nbsp;<i>c</i>&nbsp;&lt;=&nbsp;high</code>
		''' are "ordinary" in this tokenizer. See the
		''' {@code ordinaryChar} method for more information on a
		''' character being ordinary.
		''' </summary>
		''' <param name="low">   the low end of the range. </param>
		''' <param name="hi">    the high end of the range. </param>
		''' <seealso cref=     java.io.StreamTokenizer#ordinaryChar(int) </seealso>
		Public Overridable Sub ordinaryChars(ByVal low As Integer, ByVal hi As Integer)
			If low < 0 Then low = 0
			If hi >= [ctype].Length Then hi = [ctype].Length - 1
			Do While low <= hi
				[ctype](low) = 0
				low += 1
			Loop
		End Sub

		''' <summary>
		''' Specifies that the character argument is "ordinary"
		''' in this tokenizer. It removes any special significance the
		''' character has as a comment character, word component, string
		''' delimiter, white space, or number character. When such a character
		''' is encountered by the parser, the parser treats it as a
		''' single-character token and sets {@code ttype} field to the
		''' character value.
		''' 
		''' <p>Making a line terminator character "ordinary" may interfere
		''' with the ability of a {@code StreamTokenizer} to count
		''' lines. The {@code lineno} method may no longer reflect
		''' the presence of such terminator characters in its line count.
		''' </summary>
		''' <param name="ch">   the character. </param>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public Overridable Sub ordinaryChar(ByVal ch As Integer)
			If ch >= 0 AndAlso ch < [ctype].Length Then [ctype](ch) = 0
		End Sub

		''' <summary>
		''' Specified that the character argument starts a single-line
		''' comment. All characters from the comment character to the end of
		''' the line are ignored by this stream tokenizer.
		''' 
		''' <p>Any other attribute settings for the specified character are cleared.
		''' </summary>
		''' <param name="ch">   the character. </param>
		Public Overridable Sub commentChar(ByVal ch As Integer)
			If ch >= 0 AndAlso ch < [ctype].Length Then [ctype](ch) = CT_COMMENT
		End Sub

		''' <summary>
		''' Specifies that matching pairs of this character delimit string
		''' constants in this tokenizer.
		''' <p>
		''' When the {@code nextToken} method encounters a string
		''' constant, the {@code ttype} field is set to the string
		''' delimiter and the {@code sval} field is set to the body of
		''' the string.
		''' <p>
		''' If a string quote character is encountered, then a string is
		''' recognized, consisting of all characters after (but not including)
		''' the string quote character, up to (but not including) the next
		''' occurrence of that same string quote character, or a line
		''' terminator, or end of file. The usual escape sequences such as
		''' {@code "\u005Cn"} and {@code "\u005Ct"} are recognized and
		''' converted to single characters as the string is parsed.
		''' 
		''' <p>Any other attribute settings for the specified character are cleared.
		''' </summary>
		''' <param name="ch">   the character. </param>
		''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public Overridable Sub quoteChar(ByVal ch As Integer)
			If ch >= 0 AndAlso ch < [ctype].Length Then [ctype](ch) = CT_QUOTE
		End Sub

		''' <summary>
		''' Specifies that numbers should be parsed by this tokenizer. The
		''' syntax table of this tokenizer is modified so that each of the twelve
		''' characters:
		''' <blockquote><pre>
		'''      0 1 2 3 4 5 6 7 8 9 . -
		''' </pre></blockquote>
		''' <p>
		''' has the "numeric" attribute.
		''' <p>
		''' When the parser encounters a word token that has the format of a
		''' double precision floating-point number, it treats the token as a
		''' number rather than a word, by setting the {@code ttype}
		''' field to the value {@code TT_NUMBER} and putting the numeric
		''' value of the token into the {@code nval} field.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_NUMBER </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public Overridable Sub parseNumbers()
			For i As Integer = AscW("0"c) To AscW("9"c)
				[ctype](i) = [ctype](i) Or CT_DIGIT
			Next i
			[ctype](AscW("."c)) = [ctype](AscW("."c)) Or CT_DIGIT
			[ctype](AscW("-"c)) = [ctype](AscW("-"c)) Or CT_DIGIT
		End Sub

		''' <summary>
		''' Determines whether or not ends of line are treated as tokens.
		''' If the flag argument is true, this tokenizer treats end of lines
		''' as tokens; the {@code nextToken} method returns
		''' {@code TT_EOL} and also sets the {@code ttype} field to
		''' this value when an end of line is read.
		''' <p>
		''' A line is a sequence of characters ending with either a
		''' carriage-return character ({@code '\u005Cr'}) or a newline
		''' character ({@code '\u005Cn'}). In addition, a carriage-return
		''' character followed immediately by a newline character is treated
		''' as a single end-of-line token.
		''' <p>
		''' If the {@code flag} is false, end-of-line characters are
		''' treated as white space and serve only to separate tokens.
		''' </summary>
		''' <param name="flag">   {@code true} indicates that end-of-line characters
		'''                 are separate tokens; {@code false} indicates that
		'''                 end-of-line characters are white space. </param>
		''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_EOL </seealso>
		Public Overridable Sub eolIsSignificant(ByVal flag As Boolean)
			eolIsSignificantP = flag
		End Sub

		''' <summary>
		''' Determines whether or not the tokenizer recognizes C-style comments.
		''' If the flag argument is {@code true}, this stream tokenizer
		''' recognizes C-style comments. All text between successive
		''' occurrences of {@code /*} and <code>*&#47;</code> are discarded.
		''' <p>
		''' If the flag argument is {@code false}, then C-style comments
		''' are not treated specially.
		''' </summary>
		''' <param name="flag">   {@code true} indicates to recognize and ignore
		'''                 C-style comments. </param>
		Public Overridable Sub slashStarComments(ByVal flag As Boolean)
			slashStarCommentsP = flag
		End Sub

		''' <summary>
		''' Determines whether or not the tokenizer recognizes C++-style comments.
		''' If the flag argument is {@code true}, this stream tokenizer
		''' recognizes C++-style comments. Any occurrence of two consecutive
		''' slash characters ({@code '/'}) is treated as the beginning of
		''' a comment that extends to the end of the line.
		''' <p>
		''' If the flag argument is {@code false}, then C++-style
		''' comments are not treated specially.
		''' </summary>
		''' <param name="flag">   {@code true} indicates to recognize and ignore
		'''                 C++-style comments. </param>
		Public Overridable Sub slashSlashComments(ByVal flag As Boolean)
			slashSlashCommentsP = flag
		End Sub

		''' <summary>
		''' Determines whether or not word token are automatically lowercased.
		''' If the flag argument is {@code true}, then the value in the
		''' {@code sval} field is lowercased whenever a word token is
		''' returned (the {@code ttype} field has the
		''' value {@code TT_WORD} by the {@code nextToken} method
		''' of this tokenizer.
		''' <p>
		''' If the flag argument is {@code false}, then the
		''' {@code sval} field is not modified.
		''' </summary>
		''' <param name="fl">   {@code true} indicates that all word tokens should
		'''               be lowercased. </param>
		''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#TT_WORD </seealso>
		Public Overridable Sub lowerCaseMode(ByVal fl As Boolean)
			forceLower = fl
		End Sub

		''' <summary>
		''' Read the next character </summary>
		Private Function read() As Integer
			If reader IsNot Nothing Then
				Return reader.read()
			ElseIf input IsNot Nothing Then
				Return input.read()
			Else
				Throw New IllegalStateException
			End If
		End Function

		''' <summary>
		''' Parses the next token from the input stream of this tokenizer.
		''' The type of the next token is returned in the {@code ttype}
		''' field. Additional information about the token may be in the
		''' {@code nval} field or the {@code sval} field of this
		''' tokenizer.
		''' <p>
		''' Typical clients of this
		''' class first set up the syntax tables and then sit in a loop
		''' calling nextToken to parse successive tokens until TT_EOF
		''' is returned.
		''' </summary>
		''' <returns>     the value of the {@code ttype} field. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.StreamTokenizer#nval </seealso>
		''' <seealso cref=        java.io.StreamTokenizer#sval </seealso>
		''' <seealso cref=        java.io.StreamTokenizer#ttype </seealso>
		Public Overridable Function nextToken() As Integer
			If pushedBack Then
				pushedBack = False
				Return ttype
			End If
			Dim ct As SByte() = [ctype]
			sval = Nothing

			Dim c As Integer = peekc
			If c < 0 Then c = NEED_CHAR
			If c = SKIP_LF Then
				c = read()
				If c < 0 Then
						ttype = TT_EOF
						Return ttype
				End If
				If c = ControlChars.Lf Then c = NEED_CHAR
			End If
			If c = NEED_CHAR Then
				c = read()
				If c < 0 Then
						ttype = TT_EOF
						Return ttype
				End If
			End If
			ttype = c ' Just to be safe

	'         Set peekc so that the next invocation of nextToken will read
	'         * another character unless peekc is reset in this invocation
	'         
			peekc = NEED_CHAR

			Dim [ctype] As Integer = If(c < 256, ct(c), CT_ALPHA)
			Do While ([ctype] And CT_WHITESPACE) <> 0
				If c = ControlChars.Cr Then
					LINENO_Renamed += 1
					If eolIsSignificantP Then
						peekc = SKIP_LF
							ttype = TT_EOL
							Return ttype
					End If
					c = read()
					If c = ControlChars.Lf Then c = read()
				Else
					If c = ControlChars.Lf Then
						LINENO_Renamed += 1
						If eolIsSignificantP Then
								ttype = TT_EOL
								Return ttype
						End If
					End If
					c = read()
				End If
				If c < 0 Then
						ttype = TT_EOF
						Return ttype
				End If
				[ctype] = If(c < 256, ct(c), CT_ALPHA)
			Loop

			If ([ctype] And CT_DIGIT) <> 0 Then
				Dim neg As Boolean = False
				If c = AscW("-"c) Then
					c = read()
					If c <> AscW("."c) AndAlso (c < AscW("0"c) OrElse c > AscW("9"c)) Then
						peekc = c
							ttype = AscW("-"c)
							Return ttype
					End If
					neg = True
				End If
				Dim v As Double = 0
				Dim decexp As Integer = 0
				Dim seendot As Integer = 0
				Do
					If c = AscW("."c) AndAlso seendot = 0 Then
						seendot = 1
					ElseIf "0"c <= c AndAlso c <= "9"c Then
						v = v * 10 + (c - AscW("0"c))
						decexp += seendot
					Else
						Exit Do
					End If
					c = read()
				Loop
				peekc = c
				If decexp <> 0 Then
					Dim denom As Double = 10
					decexp -= 1
					Do While decexp > 0
						denom *= 10
						decexp -= 1
					Loop
					' Do one division of a likely-to-be-more-accurate number 
					v = v / denom
				End If
				nval = If(neg, -v, v)
					ttype = TT_NUMBER
					Return ttype
			End If

			If ([ctype] And CT_ALPHA) <> 0 Then
				Dim i As Integer = 0
				Do
					If i >= buf.Length Then buf = java.util.Arrays.copyOf(buf, buf.Length * 2)
					buf(i) = ChrW(c)
					i += 1
					c = read()
					[ctype] = If(c < 0, CT_WHITESPACE, If(c < 256, ct(c), CT_ALPHA))
				Loop While ([ctype] And (CT_ALPHA Or CT_DIGIT)) <> 0
				peekc = c
				sval = String.copyValueOf(buf, 0, i)
				If forceLower Then sval = sval.ToLower()
					ttype = TT_WORD
					Return ttype
			End If

			If ([ctype] And CT_QUOTE) <> 0 Then
				ttype = c
				Dim i As Integer = 0
	'             Invariants (because \Octal needs a lookahead):
	'             *   (i)  c contains char value
	'             *   (ii) d contains the lookahead
	'             
				Dim d As Integer = read()
				Do While d >= 0 AndAlso d <> ttype AndAlso d <> ControlChars.Lf AndAlso d <> ControlChars.Cr
					If d = AscW("\"c) Then
						c = read()
						Dim first As Integer = c ' To allow \377, but not \477
						If c >= "0"c AndAlso c <= "7"c Then
							c = c - AscW("0"c)
							Dim c2 As Integer = read()
							If "0"c <= c2 AndAlso c2 <= "7"c Then
								c = (c << 3) + (c2 - AscW("0"c))
								c2 = read()
								If "0"c <= c2 AndAlso c2 <= "7"c AndAlso first <= "3"c Then
									c = (c << 3) + (c2 - AscW("0"c))
									d = read()
								Else
									d = c2
								End If
							Else
							  d = c2
							End If
						Else
							Select Case c
							Case "a"c
								c = &H7
							Case "b"c
								c = ControlChars.Back
							Case "f"c
								c = &HC
							Case "n"c
								c = ControlChars.Lf
							Case "r"c
								c = ControlChars.Cr
							Case "t"c
								c = ControlChars.Tab
							Case "v"c
								c = &HB
							End Select
							d = read()
						End If
					Else
						c = d
						d = read()
					End If
					If i >= buf.Length Then buf = java.util.Arrays.copyOf(buf, buf.Length * 2)
					buf(i) = ChrW(c)
					i += 1
				Loop

	'             If we broke out of the loop because we found a matching quote
	'             * character then arrange to read a new character next time
	'             * around; otherwise, save the character.
	'             
				peekc = If(d = ttype, NEED_CHAR, d)

				sval = String.copyValueOf(buf, 0, i)
				Return ttype
			End If

			If c = AscW("/"c) AndAlso (slashSlashCommentsP OrElse slashStarCommentsP) Then
				c = read()
				If c = AscW("*"c) AndAlso slashStarCommentsP Then
					Dim prevc As Integer = 0
					c = read()
					Do While c <> AscW("/"c) OrElse prevc <> AscW("*"c)
						If c = ControlChars.Cr Then
							LINENO_Renamed += 1
							c = read()
							If c = ControlChars.Lf Then c = read()
						Else
							If c = ControlChars.Lf Then
								LINENO_Renamed += 1
								c = read()
							End If
						End If
						If c < 0 Then
								ttype = TT_EOF
								Return ttype
						End If
						prevc = c
						c = read()
					Loop
					Return nextToken()
				ElseIf c = AscW("/"c) AndAlso slashSlashCommentsP Then
					c = read()
					Do While c <> ControlChars.Lf AndAlso c <> ControlChars.Cr AndAlso c >= 0

						c = read()
					Loop
					peekc = c
					Return nextToken()
				Else
					' Now see if it is still a single line comment 
					If (ct(AscW("/"c)) And CT_COMMENT) <> 0 Then
						c = read()
						Do While c <> ControlChars.Lf AndAlso c <> ControlChars.Cr AndAlso c >= 0

							c = read()
						Loop
						peekc = c
						Return nextToken()
					Else
						peekc = c
							ttype = AscW("/"c)
							Return ttype
					End If
				End If
			End If

			If ([ctype] And CT_COMMENT) <> 0 Then
				c = read()
				Do While c <> ControlChars.Lf AndAlso c <> ControlChars.Cr AndAlso c >= 0

					c = read()
				Loop
				peekc = c
				Return nextToken()
			End If

				ttype = c
				Return ttype
		End Function

		''' <summary>
		''' Causes the next call to the {@code nextToken} method of this
		''' tokenizer to return the current value in the {@code ttype}
		''' field, and not to modify the value in the {@code nval} or
		''' {@code sval} field.
		''' </summary>
		''' <seealso cref=     java.io.StreamTokenizer#nextToken() </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public Overridable Sub pushBack()
			If ttype <> TT_NOTHING Then ' No-op if nextToken() not called pushedBack = True
		End Sub

		''' <summary>
		''' Return the current line number.
		''' </summary>
		''' <returns>  the current line number of this stream tokenizer. </returns>
		Public Overridable Function lineno() As Integer
			Return LINENO_Renamed
		End Function

		''' <summary>
		''' Returns the string representation of the current stream token and
		''' the line number it occurs on.
		''' 
		''' <p>The precise string returned is unspecified, although the following
		''' example can be considered typical:
		''' 
		''' <blockquote><pre>Token['a'], line 10</pre></blockquote>
		''' </summary>
		''' <returns>  a string representation of the token </returns>
		''' <seealso cref=     java.io.StreamTokenizer#nval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#sval </seealso>
		''' <seealso cref=     java.io.StreamTokenizer#ttype </seealso>
		Public Overrides Function ToString() As String
			Dim ret As String
			Select Case ttype
			  Case TT_EOF
				ret = "EOF"
			  Case TT_EOL
				ret = "EOL"
			  Case TT_WORD
				ret = sval
			  Case TT_NUMBER
				ret = "n=" & nval
			  Case TT_NOTHING
				ret = "NOTHING"
			  Case Else
	'                
	'                 * ttype is the first character of either a quoted string or
	'                 * is an ordinary character. ttype can definitely not be less
	'                 * than 0, since those are reserved values used in the previous
	'                 * case statements
	'                 
					If ttype < 256 AndAlso (([ctype](ttype) And CT_QUOTE) <> 0) Then
						ret = sval
						Exit Select
					End If

					Dim s As Char() = New Char(2){}
						s(2) = "'"c
						s(0) = s(2)
					s(1) = ChrW(ttype)
					ret = New String(s)
					Exit Select
			End Select
			Return "Token[" & ret & "], line " & LINENO_Renamed
		End Function

	End Class

End Namespace