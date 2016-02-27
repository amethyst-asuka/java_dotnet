Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A simple text scanner which can parse primitive types and strings using
	''' regular expressions.
	''' 
	''' <p>A <code>Scanner</code> breaks its input into tokens using a
	''' delimiter pattern, which by default matches whitespace. The resulting
	''' tokens may then be converted into values of different types using the
	''' various <tt>next</tt> methods.
	''' 
	''' <p>For example, this code allows a user to read a number from
	''' <tt>System.in</tt>:
	''' <blockquote><pre>{@code
	'''     Scanner sc = new Scanner(System.in);
	'''     int i = sc.nextInt();
	''' }</pre></blockquote>
	''' 
	''' <p>As another example, this code allows <code>long</code> types to be
	''' assigned from entries in a file <code>myNumbers</code>:
	''' <blockquote><pre>{@code
	'''      Scanner sc = new Scanner(new File("myNumbers"));
	'''      while (sc.hasNextLong()) {
	'''          long aLong = sc.nextLong();
	'''      }
	''' }</pre></blockquote>
	''' 
	''' <p>The scanner can also use delimiters other than whitespace. This
	''' example reads several items in from a string:
	''' <blockquote><pre>{@code
	'''     String input = "1 fish 2 fish red fish blue fish";
	'''     Scanner s = new Scanner(input).useDelimiter("\\s*fish\\s*");
	'''     System.out.println(s.nextInt());
	'''     System.out.println(s.nextInt());
	'''     System.out.println(s.next());
	'''     System.out.println(s.next());
	'''     s.close();
	''' }</pre></blockquote>
	''' <p>
	''' prints the following output:
	''' <blockquote><pre>{@code
	'''     1
	'''     2
	'''     red
	'''     blue
	''' }</pre></blockquote>
	''' 
	''' <p>The same output can be generated with this code, which uses a regular
	''' expression to parse all four tokens at once:
	''' <blockquote><pre>{@code
	'''     String input = "1 fish 2 fish red fish blue fish";
	'''     Scanner s = new Scanner(input);
	'''     s.findInLine("(\\d+) fish (\\d+) fish (\\w+) fish (\\w+)");
	'''     MatchResult result = s.match();
	'''     for (int i=1; i<=result.groupCount(); i++)
	'''         System.out.println(result.group(i));
	'''     s.close();
	''' }</pre></blockquote>
	''' 
	''' <p>The <a name="default-delimiter">default whitespace delimiter</a> used
	''' by a scanner is as recognized by <seealso cref="java.lang.Character"/>.{@link
	''' java.lang.Character#isWhitespace(char) isWhitespace}. The <seealso cref="#reset"/>
	''' method will reset the value of the scanner's delimiter to the default
	''' whitespace delimiter regardless of whether it was previously changed.
	''' 
	''' <p>A scanning operation may block waiting for input.
	''' 
	''' <p>The <seealso cref="#next"/> and <seealso cref="#hasNext"/> methods and their
	''' primitive-type companion methods (such as <seealso cref="#nextInt"/> and
	''' <seealso cref="#hasNextInt"/>) first skip any input that matches the delimiter
	''' pattern, and then attempt to return the next token. Both <tt>hasNext</tt>
	''' and <tt>next</tt> methods may block waiting for further input.  Whether a
	''' <tt>hasNext</tt> method blocks has no connection to whether or not its
	''' associated <tt>next</tt> method will block.
	''' 
	''' <p> The <seealso cref="#findInLine"/>, <seealso cref="#findWithinHorizon"/>, and <seealso cref="#skip"/>
	''' methods operate independently of the delimiter pattern. These methods will
	''' attempt to match the specified pattern with no regard to delimiters in the
	''' input and thus can be used in special circumstances where delimiters are
	''' not relevant. These methods may block waiting for more input.
	''' 
	''' <p>When a scanner throws an <seealso cref="InputMismatchException"/>, the scanner
	''' will not pass the token that caused the exception, so that it may be
	''' retrieved or skipped via some other method.
	''' 
	''' <p>Depending upon the type of delimiting pattern, empty tokens may be
	''' returned. For example, the pattern <tt>"\\s+"</tt> will return no empty
	''' tokens since it matches multiple instances of the delimiter. The delimiting
	''' pattern <tt>"\\s"</tt> could return empty tokens since it only passes one
	''' space at a time.
	''' 
	''' <p> A scanner can read text from any object which implements the {@link
	''' java.lang.Readable} interface.  If an invocation of the underlying
	''' readable's <seealso cref="java.lang.Readable#read"/> method throws an {@link
	''' java.io.IOException} then the scanner assumes that the end of the input
	''' has been reached.  The most recent <tt>IOException</tt> thrown by the
	''' underlying readable can be retrieved via the <seealso cref="#ioException"/> method.
	''' 
	''' <p>When a <code>Scanner</code> is closed, it will close its input source
	''' if the source implements the <seealso cref="java.io.Closeable"/> interface.
	''' 
	''' <p>A <code>Scanner</code> is not safe for multithreaded use without
	''' external synchronization.
	''' 
	''' <p>Unless otherwise mentioned, passing a <code>null</code> parameter into
	''' any method of a <code>Scanner</code> will cause a
	''' <code>NullPointerException</code> to be thrown.
	''' 
	''' <p>A scanner will default to interpreting numbers as decimal unless a
	''' different radix has been set by using the <seealso cref="#useRadix"/> method. The
	''' <seealso cref="#reset"/> method will reset the value of the scanner's radix to
	''' <code>10</code> regardless of whether it was previously changed.
	''' 
	''' <h3> <a name="localized-numbers">Localized numbers</a> </h3>
	''' 
	''' <p> An instance of this class is capable of scanning numbers in the standard
	''' formats as well as in the formats of the scanner's locale. A scanner's
	''' <a name="initial-locale">initial locale </a>is the value returned by the {@link
	''' java.util.Locale#getDefault(Locale.Category)
	''' Locale.getDefault(Locale.Category.FORMAT)} method; it may be changed via the {@link
	''' #useLocale} method. The <seealso cref="#reset"/> method will reset the value of the
	''' scanner's locale to the initial locale regardless of whether it was
	''' previously changed.
	''' 
	''' <p>The localized formats are defined in terms of the following parameters,
	''' which for a particular locale are taken from that locale's {@link
	''' java.text.DecimalFormat DecimalFormat} object, <tt>df</tt>, and its and
	''' <seealso cref="java.text.DecimalFormatSymbols DecimalFormatSymbols"/> object,
	''' <tt>dfs</tt>.
	''' 
	''' <blockquote><dl>
	'''     <dt><i>LocalGroupSeparator&nbsp;&nbsp;</i>
	'''         <dd>The character used to separate thousands groups,
	'''         <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	'''         java.text.DecimalFormatSymbols#getGroupingSeparator
	'''         getGroupingSeparator()}
	'''     <dt><i>LocalDecimalSeparator&nbsp;&nbsp;</i>
	'''         <dd>The character used for the decimal point,
	'''     <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	'''     java.text.DecimalFormatSymbols#getDecimalSeparator
	'''     getDecimalSeparator()}
	'''     <dt><i>LocalPositivePrefix&nbsp;&nbsp;</i>
	'''         <dd>The string that appears before a positive number (may
	'''         be empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	'''         java.text.DecimalFormat#getPositivePrefix
	'''         getPositivePrefix()}
	'''     <dt><i>LocalPositiveSuffix&nbsp;&nbsp;</i>
	'''         <dd>The string that appears after a positive number (may be
	'''         empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	'''         java.text.DecimalFormat#getPositiveSuffix
	'''         getPositiveSuffix()}
	'''     <dt><i>LocalNegativePrefix&nbsp;&nbsp;</i>
	'''         <dd>The string that appears before a negative number (may
	'''         be empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	'''         java.text.DecimalFormat#getNegativePrefix
	'''         getNegativePrefix()}
	'''     <dt><i>LocalNegativeSuffix&nbsp;&nbsp;</i>
	'''         <dd>The string that appears after a negative number (may be
	'''         empty), <i>i.e.,</i>&nbsp;<tt>df.</tt>{@link
	'''     java.text.DecimalFormat#getNegativeSuffix
	'''     getNegativeSuffix()}
	'''     <dt><i>LocalNaN&nbsp;&nbsp;</i>
	'''         <dd>The string that represents not-a-number for
	'''         floating-point values,
	'''         <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	'''         java.text.DecimalFormatSymbols#getNaN
	'''         getNaN()}
	'''     <dt><i>LocalInfinity&nbsp;&nbsp;</i>
	'''         <dd>The string that represents infinity for floating-point
	'''         values, <i>i.e.,</i>&nbsp;<tt>dfs.</tt>{@link
	'''         java.text.DecimalFormatSymbols#getInfinity
	'''         getInfinity()}
	''' </dl></blockquote>
	''' 
	''' <h4> <a name="number-syntax">Number syntax</a> </h4>
	''' 
	''' <p> The strings that can be parsed as numbers by an instance of this class
	''' are specified in terms of the following regular-expression grammar, where
	''' Rmax is the highest digit in the radix being used (for example, Rmax is 9 in base 10).
	''' 
	''' <dl>
	'''   <dt><i>NonAsciiDigit</i>:
	'''       <dd>A non-ASCII character c for which
	'''            <seealso cref="java.lang.Character#isDigit Character.isDigit"/><tt>(c)</tt>
	'''                        returns&nbsp;true
	''' 
	'''   <dt><i>Non0Digit</i>:
	'''       <dd><tt>[1-</tt><i>Rmax</i><tt>] | </tt><i>NonASCIIDigit</i>
	''' 
	'''   <dt><i>Digit</i>:
	'''       <dd><tt>[0-</tt><i>Rmax</i><tt>] | </tt><i>NonASCIIDigit</i>
	''' 
	'''   <dt><i>GroupedNumeral</i>:
	'''       <dd><tt>(&nbsp;</tt><i>Non0Digit</i>
	'''                   <i>Digit</i><tt>?
	'''                   </tt><i>Digit</i><tt>?</tt>
	'''       <dd>&nbsp;&nbsp;&nbsp;&nbsp;<tt>(&nbsp;</tt><i>LocalGroupSeparator</i>
	'''                         <i>Digit</i>
	'''                         <i>Digit</i>
	'''                         <i>Digit</i><tt> )+ )</tt>
	''' 
	'''   <dt><i>Numeral</i>:
	'''       <dd><tt>( ( </tt><i>Digit</i><tt>+ )
	'''               | </tt><i>GroupedNumeral</i><tt> )</tt>
	''' 
	'''   <dt><a name="Integer-regex"><i>Integer</i>:</a>
	'''       <dd><tt>( [-+]? ( </tt><i>Numeral</i><tt>
	'''                               ) )</tt>
	'''       <dd><tt>| </tt><i>LocalPositivePrefix</i> <i>Numeral</i>
	'''                      <i>LocalPositiveSuffix</i>
	'''       <dd><tt>| </tt><i>LocalNegativePrefix</i> <i>Numeral</i>
	'''                 <i>LocalNegativeSuffix</i>
	''' 
	'''   <dt><i>DecimalNumeral</i>:
	'''       <dd><i>Numeral</i>
	'''       <dd><tt>| </tt><i>Numeral</i>
	'''                 <i>LocalDecimalSeparator</i>
	'''                 <i>Digit</i><tt>*</tt>
	'''       <dd><tt>| </tt><i>LocalDecimalSeparator</i>
	'''                 <i>Digit</i><tt>+</tt>
	''' 
	'''   <dt><i>Exponent</i>:
	'''       <dd><tt>( [eE] [+-]? </tt><i>Digit</i><tt>+ )</tt>
	''' 
	'''   <dt><a name="Decimal-regex"><i>Decimal</i>:</a>
	'''       <dd><tt>( [-+]? </tt><i>DecimalNumeral</i>
	'''                         <i>Exponent</i><tt>? )</tt>
	'''       <dd><tt>| </tt><i>LocalPositivePrefix</i>
	'''                 <i>DecimalNumeral</i>
	'''                 <i>LocalPositiveSuffix</i>
	'''                 <i>Exponent</i><tt>?</tt>
	'''       <dd><tt>| </tt><i>LocalNegativePrefix</i>
	'''                 <i>DecimalNumeral</i>
	'''                 <i>LocalNegativeSuffix</i>
	'''                 <i>Exponent</i><tt>?</tt>
	''' 
	'''   <dt><i>HexFloat</i>:
	'''       <dd><tt>[-+]? 0[xX][0-9a-fA-F]*\.[0-9a-fA-F]+
	'''                 ([pP][-+]?[0-9]+)?</tt>
	''' 
	'''   <dt><i>NonNumber</i>:
	'''       <dd><tt>NaN
	'''                          | </tt><i>LocalNan</i><tt>
	'''                          | Infinity
	'''                          | </tt><i>LocalInfinity</i>
	''' 
	'''   <dt><i>SignedNonNumber</i>:
	'''       <dd><tt>( [-+]? </tt><i>NonNumber</i><tt> )</tt>
	'''       <dd><tt>| </tt><i>LocalPositivePrefix</i>
	'''                 <i>NonNumber</i>
	'''                 <i>LocalPositiveSuffix</i>
	'''       <dd><tt>| </tt><i>LocalNegativePrefix</i>
	'''                 <i>NonNumber</i>
	'''                 <i>LocalNegativeSuffix</i>
	''' 
	'''   <dt><a name="Float-regex"><i>Float</i></a>:
	'''       <dd><i>Decimal</i>
	'''           <tt>| </tt><i>HexFloat</i>
	'''           <tt>| </tt><i>SignedNonNumber</i>
	''' 
	''' </dl>
	''' <p>Whitespace is not significant in the above regular expressions.
	''' 
	''' @since   1.5
	''' </summary>
	Public NotInheritable Class Scanner
		Implements Iterator(Of String), Closeable

		' Internal buffer used to hold input
		Private buf As CharBuffer

		' Size of internal character buffer
		Private Const BUFFER_SIZE As Integer = 1024 ' change to 1024;

		' The index into the buffer currently held by the Scanner
		Private position As Integer

		' Internal matcher used for finding delimiters
		Private matcher As Matcher

		' Pattern used to delimit tokens
		Private delimPattern As Pattern

		' Pattern found in last hasNext operation
		Private hasNextPattern As Pattern

		' Position after last hasNext operation
		Private hasNextPosition As Integer

		' Result after last hasNext operation
		Private hasNextResult As String

		' The input source
		Private source As Readable

		' Boolean is true if source is done
		Private sourceClosed As Boolean = False

		' Boolean indicating more input is required
		Private needInput As Boolean = False

		' Boolean indicating if a delim has been skipped this operation
		Private skipped As Boolean = False

		' A store of a position that the scanner may fall back to
		Private savedScannerPosition As Integer = -1

		' A cache of the last primitive type scanned
		Private typeCache As Object = Nothing

		' Boolean indicating if a match result is available
		Private matchValid As Boolean = False

		' Boolean indicating if this scanner has been closed
		Private closed As Boolean = False

		' The current radix used by this scanner
		Private radix_Renamed As Integer = 10

		' The default radix for this scanner
		Private defaultRadix As Integer = 10

		' The locale used by this scanner
		Private locale_Renamed As java.util.Locale = Nothing

		' A cache of the last few recently used Patterns
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private sun.misc.LRUCache<String,Pattern> patternCache = New sun.misc.LRUCache<String,Pattern>(7)
	'	{
	'		protected Pattern create(String s)
	'		{
	'			Return Pattern.compile(s);
	'		}
	'		protected boolean hasName(Pattern p, String s)
	'		{
	'			Return p.pattern().equals(s);
	'		}
	'	};

		' A holder of the last IOException encountered
		Private lastException As IOException

		' A pattern for java whitespace
		Private Shared WHITESPACE_PATTERN As Pattern = Pattern.compile("\p{javaWhitespace}+")

		' A pattern for any token
		Private Shared FIND_ANY_PATTERN As Pattern = Pattern.compile("(?s).*")

		' A pattern for non-ASCII digits
		Private Shared NON_ASCII_DIGIT As Pattern = Pattern.compile("[\p{javaDigit}&&[^0-9]]")

		' Fields and methods to support scanning primitive types

		''' <summary>
		''' Locale dependent values used to scan numbers
		''' </summary>
		Private groupSeparator As String = "\,"
		Private decimalSeparator As String = "\."
		Private nanString As String = "NaN"
		Private infinityString As String = "Infinity"
		Private positivePrefix As String = ""
		Private negativePrefix As String = "\-"
		Private positiveSuffix As String = ""
		Private negativeSuffix As String = ""

		''' <summary>
		''' Fields and an accessor method to match booleans
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared boolPattern_Renamed As Pattern
		Private Const BOOLEAN_PATTERN As String = "true|false"
		Private Shared Function boolPattern() As Pattern
			Dim bp As Pattern = boolPattern_Renamed
			If bp Is Nothing Then
					bp = Pattern.compile(BOOLEAN_PATTERN, Pattern.CASE_INSENSITIVE)
					boolPattern_Renamed = bp
			End If
			Return bp
		End Function

		''' <summary>
		''' Fields and methods to match bytes, shorts, ints, and longs
		''' </summary>
		Private integerPattern_Renamed As Pattern
		Private digits As String = "0123456789abcdefghijklmnopqrstuvwxyz"
		Private non0Digit As String = "[\p{javaDigit}&&[^0]]"
		Private SIMPLE_GROUP_INDEX As Integer = 5
		Private Function buildIntegerPatternString() As String
			Dim radixDigits As String = digits.Substring(0, radix_Renamed)
			' \\p{javaDigit} is not guaranteed to be appropriate
			' here but what can we do? The final authority will be
			' whatever parse method is invoked, so ultimately the
			' Scanner will do the right thing
			Dim digit As String = "((?i)[" & radixDigits & "]|\p{javaDigit})"
			Dim groupedNumeral As String = "(" & non0Digit+digit & "?" & digit & "?(" & groupSeparator+digit+digit+digit & ")+)"
			' digit++ is the possessive form which is necessary for reducing
			' backtracking that would otherwise cause unacceptable performance
			Dim numeral As String = "((" & digit & "++)|" & groupedNumeral & ")"
			Dim javaStyleInteger As String = "([-+]?(" & numeral & "))"
			Dim negativeInteger As String = negativePrefix + numeral + negativeSuffix
			Dim positiveInteger As String = positivePrefix + numeral + positiveSuffix
			Return "(" & javaStyleInteger & ")|(" & positiveInteger & ")|(" & negativeInteger & ")"
		End Function
		Private Function integerPattern() As Pattern
			If integerPattern_Renamed Is Nothing Then integerPattern_Renamed = patternCache.forName(buildIntegerPatternString())
			Return integerPattern_Renamed
		End Function

		''' <summary>
		''' Fields and an accessor method to match line separators
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared separatorPattern_Renamed As Pattern
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared linePattern_Renamed As Pattern
		Private Shared ReadOnly LINE_SEPARATOR_PATTERN As String = vbCrLf & "|[" & vbLf & vbCr & ChrW(&H2028).ToString() & ChrW(&H2029).ToString() & ChrW(&H0085).ToString() & "]"
		Private Shared ReadOnly LINE_PATTERN As String = ".*(" & LINE_SEPARATOR_PATTERN & ")|.+$"

		Private Shared Function separatorPattern() As Pattern
			Dim sp As Pattern = separatorPattern_Renamed
			If sp Is Nothing Then
					sp = Pattern.compile(LINE_SEPARATOR_PATTERN)
					separatorPattern_Renamed = sp
			End If
			Return sp
		End Function

		Private Shared Function linePattern() As Pattern
			Dim lp As Pattern = linePattern_Renamed
			If lp Is Nothing Then
					lp = Pattern.compile(LINE_PATTERN)
					linePattern_Renamed = lp
			End If
			Return lp
		End Function

		''' <summary>
		''' Fields and methods to match floats and doubles
		''' </summary>
		Private floatPattern_Renamed As Pattern
		Private decimalPattern_Renamed As Pattern
		Private Sub buildFloatAndDecimalPattern()
			' \\p{javaDigit} may not be perfect, see above
			Dim digit As String = "([0-9]|(\p{javaDigit}))"
			Dim exponent As String = "([eE][+-]?" & digit & "+)?"
			Dim groupedNumeral As String = "(" & non0Digit+digit & "?" & digit & "?(" & groupSeparator+digit+digit+digit & ")+)"
			' Once again digit++ is used for performance, as above
			Dim numeral As String = "((" & digit & "++)|" & groupedNumeral & ")"
			Dim decimalNumeral As String = "(" & numeral & "|" & numeral + decimalSeparator + digit & "*+|" & decimalSeparator + digit & "++)"
			Dim nonNumber As String = "(NaN|" & nanString & "|Infinity|" & infinityString & ")"
			Dim positiveFloat As String = "(" & positivePrefix + decimalNumeral + positiveSuffix + exponent & ")"
			Dim negativeFloat As String = "(" & negativePrefix + decimalNumeral + negativeSuffix + exponent & ")"
			Dim [decimal] As String = "(([-+]?" & decimalNumeral + exponent & ")|" & positiveFloat & "|" & negativeFloat & ")"
			Dim hexFloat As String = "[-+]?0[xX][0-9a-fA-F]*\.[0-9a-fA-F]+([pP][-+]?[0-9]+)?"
			Dim positiveNonNumber As String = "(" & positivePrefix + nonNumber + positiveSuffix & ")"
			Dim negativeNonNumber As String = "(" & negativePrefix + nonNumber + negativeSuffix & ")"
			Dim signedNonNumber As String = "(([-+]?" & nonNumber & ")|" & positiveNonNumber & "|" & negativeNonNumber & ")"
			floatPattern_Renamed = Pattern.compile([decimal] & "|" & hexFloat & "|" & signedNonNumber)
			decimalPattern_Renamed = Pattern.compile([decimal])
		End Sub
		Private Function floatPattern() As Pattern
			If floatPattern_Renamed Is Nothing Then buildFloatAndDecimalPattern()
			Return floatPattern_Renamed
		End Function
		Private Function decimalPattern() As Pattern
			If decimalPattern_Renamed Is Nothing Then buildFloatAndDecimalPattern()
			Return decimalPattern_Renamed
		End Function

		' Constructors

		''' <summary>
		''' Constructs a <code>Scanner</code> that returns values scanned
		''' from the specified source delimited by the specified pattern.
		''' </summary>
		''' <param name="source"> A character source implementing the Readable interface </param>
		''' <param name="pattern"> A delimiting pattern </param>
		Private Sub New(ByVal source As Readable, ByVal pattern_Renamed As Pattern)
			Debug.Assert(source IsNot Nothing, "source should not be null")
			Debug.Assert(pattern_Renamed IsNot Nothing, "pattern should not be null")
			Me.source = source
			delimPattern = pattern_Renamed
			buf = CharBuffer.allocate(BUFFER_SIZE)
			buf.limit(0)
			matcher = delimPattern.matcher(buf)
			matcher.useTransparentBounds(True)
			matcher.useAnchoringBounds(False)
			useLocale(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified source.
		''' </summary>
		''' <param name="source"> A character source implementing the <seealso cref="Readable"/>
		'''         interface </param>
		Public Sub New(ByVal source As Readable)
			Me.New(Objects.requireNonNull(source, "source"), WHITESPACE_PATTERN)
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified input stream. Bytes from the stream are converted
		''' into characters using the underlying platform's
		''' <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		''' </summary>
		''' <param name="source"> An input stream to be scanned </param>
		Public Sub New(ByVal source As InputStream)
			Me.New(New InputStreamReader(source), WHITESPACE_PATTERN)
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified input stream. Bytes from the stream are converted
		''' into characters using the specified charset.
		''' </summary>
		''' <param name="source"> An input stream to be scanned </param>
		''' <param name="charsetName"> The encoding type used to convert bytes from the
		'''        stream into characters to be scanned </param>
		''' <exception cref="IllegalArgumentException"> if the specified character set
		'''         does not exist </exception>
		Public Sub New(ByVal source As InputStream, ByVal charsetName As String)
			Me.New(makeReadable(Objects.requireNonNull(source, "source"), toCharset(charsetName)), WHITESPACE_PATTERN)
		End Sub

		''' <summary>
		''' Returns a charset object for the given charset name. </summary>
		''' <exception cref="NullPointerException">          is csn is null </exception>
		''' <exception cref="IllegalArgumentException">      if the charset is not supported </exception>
		Private Shared Function toCharset(ByVal csn As String) As Charset
			Objects.requireNonNull(csn, "charsetName")
			Try
				Return Charset.forName(csn)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch IllegalCharsetNameException Or UnsupportedCharsetException e
				' IllegalArgumentException should be thrown
				Throw New IllegalArgumentException(e)
			End Try
		End Function

		Private Shared Function makeReadable(ByVal source As InputStream, ByVal charset_Renamed As Charset) As Readable
			Return New InputStreamReader(source, charset_Renamed)
		End Function

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified file. Bytes from the file are converted into
		''' characters using the underlying platform's
		''' <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		''' </summary>
		''' <param name="source"> A file to be scanned </param>
		''' <exception cref="FileNotFoundException"> if source is not found </exception>
		Public Sub New(ByVal source As File)
			Me.New(CType((New FileInputStream(source)).channel, ReadableByteChannel))
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified file. Bytes from the file are converted into
		''' characters using the specified charset.
		''' </summary>
		''' <param name="source"> A file to be scanned </param>
		''' <param name="charsetName"> The encoding type used to convert bytes from the file
		'''        into characters to be scanned </param>
		''' <exception cref="FileNotFoundException"> if source is not found </exception>
		''' <exception cref="IllegalArgumentException"> if the specified encoding is
		'''         not found </exception>
		Public Sub New(ByVal source As File, ByVal charsetName As String)
			Me.New(Objects.requireNonNull(source), toDecoder(charsetName))
		End Sub

		Private Sub New(ByVal source As File, ByVal dec As CharsetDecoder)
			Me.New(makeReadable(CType((New FileInputStream(source)).channel, ReadableByteChannel), dec))
		End Sub

		Private Shared Function toDecoder(ByVal charsetName As String) As CharsetDecoder
			Objects.requireNonNull(charsetName, "charsetName")
			Try
				Return Charset.forName(charsetName).newDecoder()
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch IllegalCharsetNameException Or UnsupportedCharsetException unused
				Throw New IllegalArgumentException(charsetName)
			End Try
		End Function

		Private Shared Function makeReadable(ByVal source As ReadableByteChannel, ByVal dec As CharsetDecoder) As Readable
			Return Channels.newReader(source, dec, -1)
		End Function

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified file. Bytes from the file are converted into
		''' characters using the underlying platform's
		''' <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		''' </summary>
		''' <param name="source">
		'''          the path to the file to be scanned </param>
		''' <exception cref="IOException">
		'''          if an I/O error occurs opening source
		''' 
		''' @since   1.7 </exception>
		Public Sub New(ByVal source As java.nio.file.Path)
			Me.New(java.nio.file.Files.newInputStream(source))
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified file. Bytes from the file are converted into
		''' characters using the specified charset.
		''' </summary>
		''' <param name="source">
		'''          the path to the file to be scanned </param>
		''' <param name="charsetName">
		'''          The encoding type used to convert bytes from the file
		'''          into characters to be scanned </param>
		''' <exception cref="IOException">
		'''          if an I/O error occurs opening source </exception>
		''' <exception cref="IllegalArgumentException">
		'''          if the specified encoding is not found
		''' @since   1.7 </exception>
		Public Sub New(ByVal source As java.nio.file.Path, ByVal charsetName As String)
			Me.New(Objects.requireNonNull(source), toCharset(charsetName))
		End Sub

		Private Sub New(ByVal source As java.nio.file.Path, ByVal charset_Renamed As Charset)
			Me.New(makeReadable(java.nio.file.Files.newInputStream(source), charset_Renamed))
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified string.
		''' </summary>
		''' <param name="source"> A string to scan </param>
		Public Sub New(ByVal source As String)
			Me.New(New StringReader(source), WHITESPACE_PATTERN)
		End Sub

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified channel. Bytes from the source are converted into
		''' characters using the underlying platform's
		''' <seealso cref="java.nio.charset.Charset#defaultCharset() default charset"/>.
		''' </summary>
		''' <param name="source"> A channel to scan </param>
		Public Sub New(ByVal source As ReadableByteChannel)
			Me.New(makeReadable(Objects.requireNonNull(source, "source")), WHITESPACE_PATTERN)
		End Sub

		Private Shared Function makeReadable(ByVal source As ReadableByteChannel) As Readable
			Return makeReadable(source, Charset.defaultCharset().newDecoder())
		End Function

		''' <summary>
		''' Constructs a new <code>Scanner</code> that produces values scanned
		''' from the specified channel. Bytes from the source are converted into
		''' characters using the specified charset.
		''' </summary>
		''' <param name="source"> A channel to scan </param>
		''' <param name="charsetName"> The encoding type used to convert bytes from the
		'''        channel into characters to be scanned </param>
		''' <exception cref="IllegalArgumentException"> if the specified character set
		'''         does not exist </exception>
		Public Sub New(ByVal source As ReadableByteChannel, ByVal charsetName As String)
			Me.New(makeReadable(Objects.requireNonNull(source, "source"), toDecoder(charsetName)), WHITESPACE_PATTERN)
		End Sub

		' Private primitives used to support scanning

		Private Sub saveState()
			savedScannerPosition = position
		End Sub

		Private Sub revertState()
			Me.position = savedScannerPosition
			savedScannerPosition = -1
			skipped = False
		End Sub

		Private Function revertState(ByVal b As Boolean) As Boolean
			Me.position = savedScannerPosition
			savedScannerPosition = -1
			skipped = False
			Return b
		End Function

		Private Sub cacheResult()
			hasNextResult = matcher.group()
			hasNextPosition = matcher.end()
			hasNextPattern = matcher.pattern()
		End Sub

		Private Sub cacheResult(ByVal result As String)
			hasNextResult = result
			hasNextPosition = matcher.end()
			hasNextPattern = matcher.pattern()
		End Sub

		' Clears both regular cache and type cache
		Private Sub clearCaches()
			hasNextPattern = Nothing
			typeCache = Nothing
		End Sub

		' Also clears both the regular cache and the type cache
		Private Property cachedResult As String
			Get
				position = hasNextPosition
				hasNextPattern = Nothing
				typeCache = Nothing
				Return hasNextResult
			End Get
		End Property

		' Also clears both the regular cache and the type cache
		Private Sub useTypeCache()
			If closed Then Throw New IllegalStateException("Scanner closed")
			position = hasNextPosition
			hasNextPattern = Nothing
			typeCache = Nothing
		End Sub

		' Tries to read more input. May block.
		Private Sub readInput()
			If buf.limit() = buf.capacity() Then makeSpace()

			' Prepare to receive data
			Dim p As Integer = buf.position()
			buf.position(buf.limit())
			buf.limit(buf.capacity())

			Dim n As Integer = 0
			Try
				n = source.read(buf)
			Catch ioe As IOException
				lastException = ioe
				n = -1
			End Try

			If n = -1 Then
				sourceClosed = True
				needInput = False
			End If

			If n > 0 Then needInput = False

			' Restore current position and limit for reading
			buf.limit(buf.position())
			buf.position(p)
		End Sub

		' After this method is called there will either be an exception
		' or else there will be space in the buffer
		Private Function makeSpace() As Boolean
			clearCaches()
			Dim offset As Integer = If(savedScannerPosition = -1, position, savedScannerPosition)
			buf.position(offset)
			' Gain space by compacting buffer
			If offset > 0 Then
				buf.compact()
				translateSavedIndexes(offset)
				position -= offset
				buf.flip()
				Return True
			End If
			' Gain space by growing buffer
			Dim newSize As Integer = buf.capacity() * 2
			Dim newBuf As CharBuffer = CharBuffer.allocate(newSize)
			newBuf.put(buf)
			newBuf.flip()
			translateSavedIndexes(offset)
			position -= offset
			buf = newBuf
			matcher.reset(buf)
			Return True
		End Function

		' When a buffer compaction/reallocation occurs the saved indexes must
		' be modified appropriately
		Private Sub translateSavedIndexes(ByVal offset As Integer)
			If savedScannerPosition <> -1 Then savedScannerPosition -= offset
		End Sub

		' If we are at the end of input then NoSuchElement;
		' If there is still input left then InputMismatch
		Private Sub throwFor()
			skipped = False
			If (sourceClosed) AndAlso (position = buf.limit()) Then
				Throw New NoSuchElementException
			Else
				Throw New InputMismatchException
			End If
		End Sub

		' Returns true if a complete token or partial token is in the buffer.
		' It is not necessary to find a complete token since a partial token
		' means that there will be another token with or without more input.
		Private Function hasTokenInBuffer() As Boolean
			matchValid = False
			matcher.usePattern(delimPattern)
			matcher.region(position, buf.limit())

			' Skip delims first
			If matcher.lookingAt() Then position = matcher.end()

			' If we are sitting at the end, no more tokens in buffer
			If position = buf.limit() Then Return False

			Return True
		End Function

	'    
	'     * Returns a "complete token" that matches the specified pattern
	'     *
	'     * A token is complete if surrounded by delims; a partial token
	'     * is prefixed by delims but not postfixed by them
	'     *
	'     * The position is advanced to the end of that complete token
	'     *
	'     * Pattern == null means accept any token at all
	'     *
	'     * Triple return:
	'     * 1. valid string means it was found
	'     * 2. null with needInput=false means we won't ever find it
	'     * 3. null with needInput=true means try again after readInput
	'     
		Private Function getCompleteTokenInBuffer(ByVal pattern_Renamed As Pattern) As String
			matchValid = False

			' Skip delims first
			matcher.usePattern(delimPattern)
			If Not skipped Then ' Enforcing only one skip of leading delims
				matcher.region(position, buf.limit())
				If matcher.lookingAt() Then
					' If more input could extend the delimiters then we must wait
					' for more input
					If matcher.hitEnd() AndAlso (Not sourceClosed) Then
						needInput = True
						Return Nothing
					End If
					' The delims were whole and the matcher should skip them
					skipped = True
					position = matcher.end()
				End If
			End If

			' If we are sitting at the end, no more tokens in buffer
			If position = buf.limit() Then
				If sourceClosed Then Return Nothing
				needInput = True
				Return Nothing
			End If

			' Must look for next delims. Simply attempting to match the
			' pattern at this point may find a match but it might not be
			' the first longest match because of missing input, or it might
			' match a partial token instead of the whole thing.

			' Then look for next delims
			matcher.region(position, buf.limit())
			Dim foundNextDelim As Boolean = matcher.find()
			If foundNextDelim AndAlso (matcher.end() = position) Then foundNextDelim = matcher.find()
			If foundNextDelim Then
				' In the rare case that more input could cause the match
				' to be lost and there is more input coming we must wait
				' for more input. Note that hitting the end is okay as long
				' as the match cannot go away. It is the beginning of the
				' next delims we want to be sure about, we don't care if
				' they potentially extend further.
				If matcher.requireEnd() AndAlso (Not sourceClosed) Then
					needInput = True
					Return Nothing
				End If
				Dim tokenEnd As Integer = matcher.start()
				' There is a complete token.
				If pattern_Renamed Is Nothing Then pattern_Renamed = FIND_ANY_PATTERN
				'  Attempt to match against the desired pattern
				matcher.usePattern(pattern_Renamed)
				matcher.region(position, tokenEnd)
				If matcher.matches() Then
					Dim s As String = matcher.group()
					position = matcher.end()
					Return s ' Complete token but it does not match
				Else
					Return Nothing
				End If
			End If

			' If we can't find the next delims but no more input is coming,
			' then we can treat the remainder as a whole token
			If sourceClosed Then
				If pattern_Renamed Is Nothing Then pattern_Renamed = FIND_ANY_PATTERN
				' Last token; Match the pattern here or throw
				matcher.usePattern(pattern_Renamed)
				matcher.region(position, buf.limit())
				If matcher.matches() Then
					Dim s As String = matcher.group()
					position = matcher.end()
					Return s
				End If
				' Last piece does not match
				Return Nothing
			End If

			' There is a partial token in the buffer; must read more
			' to complete it
			needInput = True
			Return Nothing
		End Function

		' Finds the specified pattern in the buffer up to horizon.
		' Returns a match for the specified input pattern.
		Private Function findPatternInBuffer(ByVal pattern_Renamed As Pattern, ByVal horizon As Integer) As String
			matchValid = False
			matcher.usePattern(pattern_Renamed)
			Dim bufferLimit As Integer = buf.limit()
			Dim horizonLimit As Integer = -1
			Dim searchLimit As Integer = bufferLimit
			If horizon > 0 Then
				horizonLimit = position + horizon
				If horizonLimit < bufferLimit Then searchLimit = horizonLimit
			End If
			matcher.region(position, searchLimit)
			If matcher.find() Then
				If matcher.hitEnd() AndAlso ((Not sourceClosed)) Then
					' The match may be longer if didn't hit horizon or real end
					If searchLimit <> horizonLimit Then
						 ' Hit an artificial end; try to extend the match
						needInput = True
						Return Nothing
					End If
					' The match could go away depending on what is next
					If (searchLimit = horizonLimit) AndAlso matcher.requireEnd() Then
						' Rare case: we hit the end of input and it happens
						' that it is at the horizon and the end of input is
						' required for the match.
						needInput = True
						Return Nothing
					End If
				End If
				' Did not hit end, or hit real end, or hit horizon
				position = matcher.end()
				Return matcher.group()
			End If

			If sourceClosed Then Return Nothing

			' If there is no specified horizon, or if we have not searched
			' to the specified horizon yet, get more input
			If (horizon = 0) OrElse (searchLimit <> horizonLimit) Then needInput = True
			Return Nothing
		End Function

		' Returns a match for the specified input pattern anchored at
		' the current position
		Private Function matchPatternInBuffer(ByVal pattern_Renamed As Pattern) As String
			matchValid = False
			matcher.usePattern(pattern_Renamed)
			matcher.region(position, buf.limit())
			If matcher.lookingAt() Then
				If matcher.hitEnd() AndAlso ((Not sourceClosed)) Then
					' Get more input and try again
					needInput = True
					Return Nothing
				End If
				position = matcher.end()
				Return matcher.group()
			End If

			If sourceClosed Then Return Nothing

			' Read more to find pattern
			needInput = True
			Return Nothing
		End Function

		' Throws if the scanner is closed
		Private Sub ensureOpen()
			If closed Then Throw New IllegalStateException("Scanner closed")
		End Sub

		' Public methods

		''' <summary>
		''' Closes this scanner.
		''' 
		''' <p> If this scanner has not yet been closed then if its underlying
		''' <seealso cref="java.lang.Readable readable"/> also implements the {@link
		''' java.io.Closeable} interface then the readable's <tt>close</tt> method
		''' will be invoked.  If this scanner is already closed then invoking this
		''' method will have no effect.
		''' 
		''' <p>Attempting to perform search operations after a scanner has
		''' been closed will result in an <seealso cref="IllegalStateException"/>.
		''' 
		''' </summary>
		Public Sub close() Implements Closeable.close
			If closed Then Return
			If TypeOf source Is Closeable Then
				Try
					CType(source, Closeable).close()
				Catch ioe As IOException
					lastException = ioe
				End Try
			End If
			sourceClosed = True
			source = Nothing
			closed = True
		End Sub

		''' <summary>
		''' Returns the <code>IOException</code> last thrown by this
		''' <code>Scanner</code>'s underlying <code>Readable</code>. This method
		''' returns <code>null</code> if no such exception exists.
		''' </summary>
		''' <returns> the last exception thrown by this scanner's readable </returns>
		Public Function ioException() As IOException
			Return lastException
		End Function

		''' <summary>
		''' Returns the <code>Pattern</code> this <code>Scanner</code> is currently
		''' using to match delimiters.
		''' </summary>
		''' <returns> this scanner's delimiting pattern. </returns>
		Public Function delimiter() As Pattern
			Return delimPattern
		End Function

		''' <summary>
		''' Sets this scanner's delimiting pattern to the specified pattern.
		''' </summary>
		''' <param name="pattern"> A delimiting pattern </param>
		''' <returns> this scanner </returns>
		Public Function useDelimiter(ByVal pattern_Renamed As Pattern) As Scanner
			delimPattern = pattern_Renamed
			Return Me
		End Function

		''' <summary>
		''' Sets this scanner's delimiting pattern to a pattern constructed from
		''' the specified <code>String</code>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>useDelimiter(pattern)</tt> behaves in exactly the same way as the
		''' invocation <tt>useDelimiter(Pattern.compile(pattern))</tt>.
		''' 
		''' <p> Invoking the <seealso cref="#reset"/> method will set the scanner's delimiter
		''' to the <a href= "#default-delimiter">default</a>.
		''' </summary>
		''' <param name="pattern"> A string specifying a delimiting pattern </param>
		''' <returns> this scanner </returns>
		Public Function useDelimiter(ByVal pattern_Renamed As String) As Scanner
			delimPattern = patternCache.forName(pattern_Renamed)
			Return Me
		End Function

		''' <summary>
		''' Returns this scanner's locale.
		''' 
		''' <p>A scanner's locale affects many elements of its default
		''' primitive matching regular expressions; see
		''' <a href= "#localized-numbers">localized numbers</a> above.
		''' </summary>
		''' <returns> this scanner's locale </returns>
		Public Function locale() As java.util.Locale
			Return Me.locale_Renamed
		End Function

		''' <summary>
		''' Sets this scanner's locale to the specified locale.
		''' 
		''' <p>A scanner's locale affects many elements of its default
		''' primitive matching regular expressions; see
		''' <a href= "#localized-numbers">localized numbers</a> above.
		''' 
		''' <p>Invoking the <seealso cref="#reset"/> method will set the scanner's locale to
		''' the <a href= "#initial-locale">initial locale</a>.
		''' </summary>
		''' <param name="locale"> A string specifying the locale to use </param>
		''' <returns> this scanner </returns>
		Public Function useLocale(ByVal locale_Renamed As java.util.Locale) As Scanner
			If locale_Renamed.Equals(Me.locale_Renamed) Then Return Me

			Me.locale_Renamed = locale_Renamed
			Dim df As DecimalFormat = CType(NumberFormat.getNumberInstance(locale_Renamed), DecimalFormat)
			Dim dfs As DecimalFormatSymbols = DecimalFormatSymbols.getInstance(locale_Renamed)

			' These must be literalized to avoid collision with regex
			' metacharacters such as dot or parenthesis
			groupSeparator = "\" & AscW(dfs.groupingSeparator)
			decimalSeparator = "\" & AscW(dfs.decimalSeparator)

			' Quoting the nonzero length locale-specific things
			' to avoid potential conflict with metacharacters
			nanString = "\Q" & dfs.naN & "\E"
			infinityString = "\Q" & dfs.infinity & "\E"
			positivePrefix = df.positivePrefix
			If positivePrefix.length() > 0 Then positivePrefix = "\Q" & positivePrefix & "\E"
			negativePrefix = df.negativePrefix
			If negativePrefix.length() > 0 Then negativePrefix = "\Q" & negativePrefix & "\E"
			positiveSuffix = df.positiveSuffix
			If positiveSuffix.length() > 0 Then positiveSuffix = "\Q" & positiveSuffix & "\E"
			negativeSuffix = df.negativeSuffix
			If negativeSuffix.length() > 0 Then negativeSuffix = "\Q" & negativeSuffix & "\E"

			' Force rebuilding and recompilation of locale dependent
			' primitive patterns
			integerPattern_Renamed = Nothing
			floatPattern_Renamed = Nothing

			Return Me
		End Function

		''' <summary>
		''' Returns this scanner's default radix.
		''' 
		''' <p>A scanner's radix affects elements of its default
		''' number matching regular expressions; see
		''' <a href= "#localized-numbers">localized numbers</a> above.
		''' </summary>
		''' <returns> the default radix of this scanner </returns>
		Public Function radix() As Integer
			Return Me.defaultRadix
		End Function

		''' <summary>
		''' Sets this scanner's default radix to the specified radix.
		''' 
		''' <p>A scanner's radix affects elements of its default
		''' number matching regular expressions; see
		''' <a href= "#localized-numbers">localized numbers</a> above.
		''' 
		''' <p>If the radix is less than <code>Character.MIN_RADIX</code>
		''' or greater than <code>Character.MAX_RADIX</code>, then an
		''' <code>IllegalArgumentException</code> is thrown.
		''' 
		''' <p>Invoking the <seealso cref="#reset"/> method will set the scanner's radix to
		''' <code>10</code>.
		''' </summary>
		''' <param name="radix"> The radix to use when scanning numbers </param>
		''' <returns> this scanner </returns>
		''' <exception cref="IllegalArgumentException"> if radix is out of range </exception>
		Public Function useRadix(ByVal radix As Integer) As Scanner
			If (radix < Character.MIN_RADIX) OrElse (radix > Character.MAX_RADIX) Then Throw New IllegalArgumentException("radix:" & radix)

			If Me.defaultRadix = radix Then Return Me
			Me.defaultRadix = radix
			' Force rebuilding and recompilation of radix dependent patterns
			integerPattern_Renamed = Nothing
			Return Me
		End Function

		' The next operation should occur in the specified radix but
		' the default is left untouched.
		Private Property radix As Integer
			Set(ByVal radix As Integer)
				If Me.radix_Renamed <> radix Then
					' Force rebuilding and recompilation of radix dependent patterns
					integerPattern_Renamed = Nothing
					Me.radix_Renamed = radix
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the match result of the last scanning operation performed
		''' by this scanner. This method throws <code>IllegalStateException</code>
		''' if no match has been performed, or if the last match was
		''' not successful.
		''' 
		''' <p>The various <code>next</code>methods of <code>Scanner</code>
		''' make a match result available if they complete without throwing an
		''' exception. For instance, after an invocation of the <seealso cref="#nextInt"/>
		''' method that returned an int, this method returns a
		''' <code>MatchResult</code> for the search of the
		''' <a href="#Integer-regex"><i>Integer</i></a> regular expression
		''' defined above. Similarly the <seealso cref="#findInLine"/>,
		''' <seealso cref="#findWithinHorizon"/>, and <seealso cref="#skip"/> methods will make a
		''' match available if they succeed.
		''' </summary>
		''' <returns> a match result for the last match operation </returns>
		''' <exception cref="IllegalStateException">  If no match result is available </exception>
		Public Function match() As MatchResult
			If Not matchValid Then Throw New IllegalStateException("No match result available")
			Return matcher.toMatchResult()
		End Function

		''' <summary>
		''' <p>Returns the string representation of this <code>Scanner</code>. The
		''' string representation of a <code>Scanner</code> contains information
		''' that may be useful for debugging. The exact format is unspecified.
		''' </summary>
		''' <returns>  The string representation of this scanner </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			sb.append("java.util.Scanner")
			sb.append("[delimiters=" & delimPattern & "]")
			sb.append("[position=" & position & "]")
			sb.append("[match valid=" & matchValid & "]")
			sb.append("[need input=" & needInput & "]")
			sb.append("[source closed=" & sourceClosed & "]")
			sb.append("[skipped=" & skipped & "]")
			sb.append("[group separator=" & groupSeparator & "]")
			sb.append("[decimal separator=" & decimalSeparator & "]")
			sb.append("[positive prefix=" & positivePrefix & "]")
			sb.append("[negative prefix=" & negativePrefix & "]")
			sb.append("[positive suffix=" & positiveSuffix & "]")
			sb.append("[negative suffix=" & negativeSuffix & "]")
			sb.append("[NaN string=" & nanString & "]")
			sb.append("[infinity string=" & infinityString & "]")
			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns true if this scanner has another token in its input.
		''' This method may block while waiting for input to scan.
		''' The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner has another token </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		''' <seealso cref= java.util.Iterator </seealso>
		Public Function hasNext() As Boolean Implements Iterator(Of String).hasNext
			ensureOpen()
			saveState()
			Do While Not sourceClosed
				If hasTokenInBuffer() Then Return revertState(True)
				readInput()
			Loop
			Dim result As Boolean = hasTokenInBuffer()
			Return revertState(result)
		End Function

		''' <summary>
		''' Finds and returns the next complete token from this scanner.
		''' A complete token is preceded and followed by input that matches
		''' the delimiter pattern. This method may block while waiting for input
		''' to scan, even if a previous invocation of <seealso cref="#hasNext"/> returned
		''' <code>true</code>.
		''' </summary>
		''' <returns> the next token </returns>
		''' <exception cref="NoSuchElementException"> if no more tokens are available </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		''' <seealso cref= java.util.Iterator </seealso>
		Public Function [next]() As String
			ensureOpen()
			clearCaches()

			Do
				Dim token As String = getCompleteTokenInBuffer(Nothing)
				If token IsNot Nothing Then
					matchValid = True
					skipped = False
					Return token
				End If
				If needInput Then
					readInput()
				Else
					throwFor()
				End If
			Loop
		End Function

		''' <summary>
		''' The remove operation is not supported by this implementation of
		''' <code>Iterator</code>.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if this method is invoked. </exception>
		''' <seealso cref= java.util.Iterator </seealso>
		Public Sub remove() Implements Iterator(Of String).remove
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Returns true if the next token matches the pattern constructed from the
		''' specified string. The scanner does not advance past any input.
		''' 
		''' <p> An invocation of this method of the form <tt>hasNext(pattern)</tt>
		''' behaves in exactly the same way as the invocation
		''' <tt>hasNext(Pattern.compile(pattern))</tt>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to scan </param>
		''' <returns> true if and only if this scanner has another token matching
		'''         the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNext(ByVal pattern_Renamed As String) As Boolean
			Return hasNext(patternCache.forName(pattern_Renamed))
		End Function

		''' <summary>
		''' Returns the next token if it matches the pattern constructed from the
		''' specified string.  If the match is successful, the scanner advances
		''' past the input that matched the pattern.
		''' 
		''' <p> An invocation of this method of the form <tt>next(pattern)</tt>
		''' behaves in exactly the same way as the invocation
		''' <tt>next(Pattern.compile(pattern))</tt>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to scan </param>
		''' <returns> the next token </returns>
		''' <exception cref="NoSuchElementException"> if no such tokens are available </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function [next](ByVal pattern_Renamed As String) As String
			Return [next](patternCache.forName(pattern_Renamed))
		End Function

		''' <summary>
		''' Returns true if the next complete token matches the specified pattern.
		''' A complete token is prefixed and postfixed by input that matches
		''' the delimiter pattern. This method may block while waiting for input.
		''' The scanner does not advance past any input.
		''' </summary>
		''' <param name="pattern"> the pattern to scan for </param>
		''' <returns> true if and only if this scanner has another token matching
		'''         the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNext(ByVal pattern_Renamed As Pattern) As Boolean
			ensureOpen()
			If pattern_Renamed Is Nothing Then Throw New NullPointerException
			hasNextPattern = Nothing
			saveState()

			Do
				If getCompleteTokenInBuffer(pattern_Renamed) IsNot Nothing Then
					matchValid = True
					cacheResult()
					Return revertState(True)
				End If
				If needInput Then
					readInput()
				Else
					Return revertState(False)
				End If
			Loop
		End Function

		''' <summary>
		''' Returns the next token if it matches the specified pattern. This
		''' method may block while waiting for input to scan, even if a previous
		''' invocation of <seealso cref="#hasNext(Pattern)"/> returned <code>true</code>.
		''' If the match is successful, the scanner advances past the input that
		''' matched the pattern.
		''' </summary>
		''' <param name="pattern"> the pattern to scan for </param>
		''' <returns> the next token </returns>
		''' <exception cref="NoSuchElementException"> if no more tokens are available </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function [next](ByVal pattern_Renamed As Pattern) As String
			ensureOpen()
			If pattern_Renamed Is Nothing Then Throw New NullPointerException

			' Did we already find this pattern?
			If hasNextPattern Is pattern_Renamed Then Return cachedResult
			clearCaches()

			' Search for the pattern
			Do
				Dim token As String = getCompleteTokenInBuffer(pattern_Renamed)
				If token IsNot Nothing Then
					matchValid = True
					skipped = False
					Return token
				End If
				If needInput Then
					readInput()
				Else
					throwFor()
				End If
			Loop
		End Function

		''' <summary>
		''' Returns true if there is another line in the input of this scanner.
		''' This method may block while waiting for input. The scanner does not
		''' advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner has another line of input </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextLine() As Boolean
			saveState()

			Dim result As String = findWithinHorizon(linePattern(), 0)
			If result IsNot Nothing Then
				Dim mr As MatchResult = Me.match()
				Dim lineSep As String = mr.group(1)
				If lineSep IsNot Nothing Then
					result = result.Substring(0, result.length() - lineSep.length())
					cacheResult(result)

				Else
					cacheResult()
				End If
			End If
			revertState()
			Return (result IsNot Nothing)
		End Function

		''' <summary>
		''' Advances this scanner past the current line and returns the input
		''' that was skipped.
		''' 
		''' This method returns the rest of the current line, excluding any line
		''' separator at the end. The position is set to the beginning of the next
		''' line.
		''' 
		''' <p>Since this method continues to search through the input looking
		''' for a line separator, it may buffer all of the input searching for
		''' the line to skip if no line separators are present.
		''' </summary>
		''' <returns> the line that was skipped </returns>
		''' <exception cref="NoSuchElementException"> if no line was found </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextLine() As String
			If hasNextPattern Is linePattern() Then Return cachedResult
			clearCaches()

			Dim result As String = findWithinHorizon(linePattern_Renamed, 0)
			If result Is Nothing Then Throw New NoSuchElementException("No line found")
			Dim mr As MatchResult = Me.match()
			Dim lineSep As String = mr.group(1)
			If lineSep IsNot Nothing Then result = result.Substring(0, result.length() - lineSep.length())
			If result Is Nothing Then
				Throw New NoSuchElementException
			Else
				Return result
			End If
		End Function

		' Public methods that ignore delimiters

		''' <summary>
		''' Attempts to find the next occurrence of a pattern constructed from the
		''' specified string, ignoring delimiters.
		''' 
		''' <p>An invocation of this method of the form <tt>findInLine(pattern)</tt>
		''' behaves in exactly the same way as the invocation
		''' <tt>findInLine(Pattern.compile(pattern))</tt>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to search for </param>
		''' <returns> the text that matched the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function findInLine(ByVal pattern_Renamed As String) As String
			Return findInLine(patternCache.forName(pattern_Renamed))
		End Function

		''' <summary>
		''' Attempts to find the next occurrence of the specified pattern ignoring
		''' delimiters. If the pattern is found before the next line separator, the
		''' scanner advances past the input that matched and returns the string that
		''' matched the pattern.
		''' If no such pattern is detected in the input up to the next line
		''' separator, then <code>null</code> is returned and the scanner's
		''' position is unchanged. This method may block waiting for input that
		''' matches the pattern.
		''' 
		''' <p>Since this method continues to search through the input looking
		''' for the specified pattern, it may buffer all of the input searching for
		''' the desired token if no line separators are present.
		''' </summary>
		''' <param name="pattern"> the pattern to scan for </param>
		''' <returns> the text that matched the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function findInLine(ByVal pattern_Renamed As Pattern) As String
			ensureOpen()
			If pattern_Renamed Is Nothing Then Throw New NullPointerException
			clearCaches()
			' Expand buffer to include the next newline or end of input
			Dim endPosition As Integer = 0
			saveState()
			Do
				Dim token As String = findPatternInBuffer(separatorPattern(), 0)
				If token IsNot Nothing Then
					endPosition = matcher.start()
					Exit Do ' up to next newline
				End If
				If needInput Then
					readInput()
				Else
					endPosition = buf.limit()
					Exit Do ' up to end of input
				End If
			Loop
			revertState()
			Dim horizonForLine As Integer = endPosition - position
			' If there is nothing between the current pos and the next
			' newline simply return null, invoking findWithinHorizon
			' with "horizon=0" will scan beyond the line bound.
			If horizonForLine = 0 Then Return Nothing
			' Search for the pattern
			Return findWithinHorizon(pattern_Renamed, horizonForLine)
		End Function

		''' <summary>
		''' Attempts to find the next occurrence of a pattern constructed from the
		''' specified string, ignoring delimiters.
		''' 
		''' <p>An invocation of this method of the form
		''' <tt>findWithinHorizon(pattern)</tt> behaves in exactly the same way as
		''' the invocation
		''' <tt>findWithinHorizon(Pattern.compile(pattern, horizon))</tt>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to search for </param>
		''' <param name="horizon"> the search horizon </param>
		''' <returns> the text that matched the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		''' <exception cref="IllegalArgumentException"> if horizon is negative </exception>
		Public Function findWithinHorizon(ByVal pattern_Renamed As String, ByVal horizon As Integer) As String
			Return findWithinHorizon(patternCache.forName(pattern_Renamed), horizon)
		End Function

		''' <summary>
		''' Attempts to find the next occurrence of the specified pattern.
		''' 
		''' <p>This method searches through the input up to the specified
		''' search horizon, ignoring delimiters. If the pattern is found the
		''' scanner advances past the input that matched and returns the string
		''' that matched the pattern. If no such pattern is detected then the
		''' null is returned and the scanner's position remains unchanged. This
		''' method may block waiting for input that matches the pattern.
		''' 
		''' <p>A scanner will never search more than <code>horizon</code> code
		''' points beyond its current position. Note that a match may be clipped
		''' by the horizon; that is, an arbitrary match result may have been
		''' different if the horizon had been larger. The scanner treats the
		''' horizon as a transparent, non-anchoring bound (see {@link
		''' Matcher#useTransparentBounds} and <seealso cref="Matcher#useAnchoringBounds"/>).
		''' 
		''' <p>If horizon is <code>0</code>, then the horizon is ignored and
		''' this method continues to search through the input looking for the
		''' specified pattern without bound. In this case it may buffer all of
		''' the input searching for the pattern.
		''' 
		''' <p>If horizon is negative, then an IllegalArgumentException is
		''' thrown.
		''' </summary>
		''' <param name="pattern"> the pattern to scan for </param>
		''' <param name="horizon"> the search horizon </param>
		''' <returns> the text that matched the specified pattern </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		''' <exception cref="IllegalArgumentException"> if horizon is negative </exception>
		Public Function findWithinHorizon(ByVal pattern_Renamed As Pattern, ByVal horizon As Integer) As String
			ensureOpen()
			If pattern_Renamed Is Nothing Then Throw New NullPointerException
			If horizon < 0 Then Throw New IllegalArgumentException("horizon < 0")
			clearCaches()

			' Search for the pattern
			Do
				Dim token As String = findPatternInBuffer(pattern_Renamed, horizon)
				If token IsNot Nothing Then
					matchValid = True
					Return token
				End If
				If needInput Then
					readInput()
				Else
					Exit Do ' up to end of input
				End If
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Skips input that matches the specified pattern, ignoring delimiters.
		''' This method will skip input if an anchored match of the specified
		''' pattern succeeds.
		''' 
		''' <p>If a match to the specified pattern is not found at the
		''' current position, then no input is skipped and a
		''' <tt>NoSuchElementException</tt> is thrown.
		''' 
		''' <p>Since this method seeks to match the specified pattern starting at
		''' the scanner's current position, patterns that can match a lot of
		''' input (".*", for example) may cause the scanner to buffer a large
		''' amount of input.
		''' 
		''' <p>Note that it is possible to skip something without risking a
		''' <code>NoSuchElementException</code> by using a pattern that can
		''' match nothing, e.g., <code>sc.skip("[ \t]*")</code>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to skip over </param>
		''' <returns> this scanner </returns>
		''' <exception cref="NoSuchElementException"> if the specified pattern is not found </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function skip(ByVal pattern_Renamed As Pattern) As Scanner
			ensureOpen()
			If pattern_Renamed Is Nothing Then Throw New NullPointerException
			clearCaches()

			' Search for the pattern
			Do
				Dim token As String = matchPatternInBuffer(pattern_Renamed)
				If token IsNot Nothing Then
					matchValid = True
					position = matcher.end()
					Return Me
				End If
				If needInput Then
					readInput()
				Else
					Throw New NoSuchElementException
				End If
			Loop
		End Function

		''' <summary>
		''' Skips input that matches a pattern constructed from the specified
		''' string.
		''' 
		''' <p> An invocation of this method of the form <tt>skip(pattern)</tt>
		''' behaves in exactly the same way as the invocation
		''' <tt>skip(Pattern.compile(pattern))</tt>.
		''' </summary>
		''' <param name="pattern"> a string specifying the pattern to skip over </param>
		''' <returns> this scanner </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function skip(ByVal pattern_Renamed As String) As Scanner
			Return skip(patternCache.forName(pattern_Renamed))
		End Function

		' Convenience methods for scanning primitives

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a boolean value using a case insensitive pattern
		''' created from the string "true|false".  The scanner does not
		''' advance past the input that matched.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         boolean value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextBoolean() As Boolean
			Return hasNext(boolPattern())
		End Function

		''' <summary>
		''' Scans the next token of the input into a boolean value and returns
		''' that value. This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid boolean value.
		''' If the match is successful, the scanner advances past the input that
		''' matched.
		''' </summary>
		''' <returns> the boolean scanned from the input </returns>
		''' <exception cref="InputMismatchException"> if the next token is not a valid boolean </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextBoolean() As Boolean
			clearCaches()
			Return Convert.ToBoolean([next](boolPattern()))
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a byte value in the default radix using the
		''' <seealso cref="#nextByte"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         byte value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextByte() As Boolean
			Return hasNextByte(defaultRadix)
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a byte value in the specified radix using the
		''' <seealso cref="#nextByte"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as a byte value </param>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         byte value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextByte(ByVal radix As Integer) As Boolean
			radix = radix
			Dim result As Boolean = hasNext(integerPattern())
			If result Then ' Cache it
				Try
					Dim s As String = If(matcher.group(SIMPLE_GROUP_INDEX) Is Nothing, processIntegerToken(hasNextResult), hasNextResult)
					typeCache = Convert.ToByte(s, radix)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>byte</tt>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>nextByte()</tt> behaves in exactly the same way as the
		''' invocation <tt>nextByte(radix)</tt>, where <code>radix</code>
		''' is the default radix of this scanner.
		''' </summary>
		''' <returns> the <tt>byte</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextByte() As SByte
			 Return nextByte(defaultRadix)
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>byte</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid byte value as
		''' described below. If the translation is successful, the scanner advances
		''' past the input that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Integer-regex"><i>Integer</i></a> regular expression defined
		''' above then the token is converted into a <tt>byte</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Byte#parseByte(String, int) java.lang.[Byte].parseByte"/> with the
		''' specified radix.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as a byte value </param>
		''' <returns> the <tt>byte</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextByte(ByVal radix As Integer) As SByte
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Byte) AndAlso Me.radix_Renamed = radix Then
				Dim val As SByte = CByte(typeCache)
				useTypeCache()
				Return val
			End If
			radix = radix
			clearCaches()
			' Search for next byte
			Try
				Dim s As String = [next](integerPattern())
				If matcher.group(SIMPLE_GROUP_INDEX) Is Nothing Then s = processIntegerToken(s)
				Return Convert.ToByte(s, radix)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a short value in the default radix using the
		''' <seealso cref="#nextShort"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         short value in the default radix </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextShort() As Boolean
			Return hasNextShort(defaultRadix)
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a short value in the specified radix using the
		''' <seealso cref="#nextShort"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as a short value </param>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         short value in the specified radix </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextShort(ByVal radix As Integer) As Boolean
			radix = radix
			Dim result As Boolean = hasNext(integerPattern())
			If result Then ' Cache it
				Try
					Dim s As String = If(matcher.group(SIMPLE_GROUP_INDEX) Is Nothing, processIntegerToken(hasNextResult), hasNextResult)
					typeCache = Convert.ToInt16(s, radix)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>short</tt>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>nextShort()</tt> behaves in exactly the same way as the
		''' invocation <tt>nextShort(radix)</tt>, where <code>radix</code>
		''' is the default radix of this scanner.
		''' </summary>
		''' <returns> the <tt>short</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextShort() As Short
			Return nextShort(defaultRadix)
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>short</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid short value as
		''' described below. If the translation is successful, the scanner advances
		''' past the input that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Integer-regex"><i>Integer</i></a> regular expression defined
		''' above then the token is converted into a <tt>short</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Short#parseShort(String, int)  java.lang.[Short].parseShort"/> with the
		''' specified radix.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as a short value </param>
		''' <returns> the <tt>short</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextShort(ByVal radix As Integer) As Short
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Short?) AndAlso Me.radix_Renamed = radix Then
				Dim val As Short = CShort(Fix(typeCache))
				useTypeCache()
				Return val
			End If
			radix = radix
			clearCaches()
			' Search for next short
			Try
				Dim s As String = [next](integerPattern())
				If matcher.group(SIMPLE_GROUP_INDEX) Is Nothing Then s = processIntegerToken(s)
				Return Convert.ToInt16(s, radix)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as an int value in the default radix using the
		''' <seealso cref="#nextInt"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         int value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextInt() As Boolean
			Return hasNextInt(defaultRadix)
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as an int value in the specified radix using the
		''' <seealso cref="#nextInt"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as an int value </param>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         int value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextInt(ByVal radix As Integer) As Boolean
			radix = radix
			Dim result As Boolean = hasNext(integerPattern())
			If result Then ' Cache it
				Try
					Dim s As String = If(matcher.group(SIMPLE_GROUP_INDEX) Is Nothing, processIntegerToken(hasNextResult), hasNextResult)
					typeCache = Convert.ToInt32(s, radix)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' The integer token must be stripped of prefixes, group separators,
		''' and suffixes, non ascii digits must be converted into ascii digits
		''' before parse will accept it.
		''' </summary>
		Private Function processIntegerToken(ByVal token As String) As String
			Dim result As String = token.replaceAll("" & groupSeparator, "")
			Dim isNegative As Boolean = False
			Dim preLen As Integer = negativePrefix.length()
			If (preLen > 0) AndAlso result.StartsWith(negativePrefix) Then
				isNegative = True
				result = result.Substring(preLen)
			End If
			Dim sufLen As Integer = negativeSuffix.length()
			If (sufLen > 0) AndAlso result.EndsWith(negativeSuffix) Then
				isNegative = True
				result = result.Substring(result.length() - sufLen, result.length() - (result.length() - sufLen))
			End If
			If isNegative Then result = "-" & result
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as an <tt>int</tt>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>nextInt()</tt> behaves in exactly the same way as the
		''' invocation <tt>nextInt(radix)</tt>, where <code>radix</code>
		''' is the default radix of this scanner.
		''' </summary>
		''' <returns> the <tt>int</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextInt() As Integer
			Return nextInt(defaultRadix)
		End Function

		''' <summary>
		''' Scans the next token of the input as an <tt>int</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid int value as
		''' described below. If the translation is successful, the scanner advances
		''' past the input that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Integer-regex"><i>Integer</i></a> regular expression defined
		''' above then the token is converted into an <tt>int</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Integer#parseInt(String, int)  java.lang.[Integer].parseInt"/> with the
		''' specified radix.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as an int value </param>
		''' <returns> the <tt>int</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextInt(ByVal radix As Integer) As Integer
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Integer?) AndAlso Me.radix_Renamed = radix Then
				Dim val As Integer = CInt(Fix(typeCache))
				useTypeCache()
				Return val
			End If
			radix = radix
			clearCaches()
			' Search for next int
			Try
				Dim s As String = [next](integerPattern())
				If matcher.group(SIMPLE_GROUP_INDEX) Is Nothing Then s = processIntegerToken(s)
				Return Convert.ToInt32(s, radix)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a long value in the default radix using the
		''' <seealso cref="#nextLong"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         long value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextLong() As Boolean
			Return hasNextLong(defaultRadix)
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a long value in the specified radix using the
		''' <seealso cref="#nextLong"/> method. The scanner does not advance past any input.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as a long value </param>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         long value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextLong(ByVal radix As Integer) As Boolean
			radix = radix
			Dim result As Boolean = hasNext(integerPattern())
			If result Then ' Cache it
				Try
					Dim s As String = If(matcher.group(SIMPLE_GROUP_INDEX) Is Nothing, processIntegerToken(hasNextResult), hasNextResult)
					typeCache = Convert.ToInt64(s, radix)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>long</tt>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>nextLong()</tt> behaves in exactly the same way as the
		''' invocation <tt>nextLong(radix)</tt>, where <code>radix</code>
		''' is the default radix of this scanner.
		''' </summary>
		''' <returns> the <tt>long</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextLong() As Long
			Return nextLong(defaultRadix)
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>long</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid long value as
		''' described below. If the translation is successful, the scanner advances
		''' past the input that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Integer-regex"><i>Integer</i></a> regular expression defined
		''' above then the token is converted into a <tt>long</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Long#parseLong(String, int) java.lang.[Long].parseLong"/> with the
		''' specified radix.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as an int value </param>
		''' <returns> the <tt>long</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextLong(ByVal radix As Integer) As Long
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Long?) AndAlso Me.radix_Renamed = radix Then
				Dim val As Long = CLng(Fix(typeCache))
				useTypeCache()
				Return val
			End If
			radix = radix
			clearCaches()
			Try
				Dim s As String = [next](integerPattern())
				If matcher.group(SIMPLE_GROUP_INDEX) Is Nothing Then s = processIntegerToken(s)
				Return Convert.ToInt64(s, radix)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' The float token must be stripped of prefixes, group separators,
		''' and suffixes, non ascii digits must be converted into ascii digits
		''' before parseFloat will accept it.
		''' 
		''' If there are non-ascii digits in the token these digits must
		''' be processed before the token is passed to parseFloat.
		''' </summary>
		Private Function processFloatToken(ByVal token As String) As String
			Dim result As String = token.replaceAll(groupSeparator, "")
			If Not decimalSeparator.Equals("\.") Then result = result.replaceAll(decimalSeparator, ".")
			Dim isNegative As Boolean = False
			Dim preLen As Integer = negativePrefix.length()
			If (preLen > 0) AndAlso result.StartsWith(negativePrefix) Then
				isNegative = True
				result = result.Substring(preLen)
			End If
			Dim sufLen As Integer = negativeSuffix.length()
			If (sufLen > 0) AndAlso result.EndsWith(negativeSuffix) Then
				isNegative = True
				result = result.Substring(result.length() - sufLen, result.length() - (result.length() - sufLen))
			End If
			If result.Equals(nanString) Then result = "NaN"
			If result.Equals(infinityString) Then result = "Infinity"
			If isNegative Then result = "-" & result

			' Translate non-ASCII digits
			Dim m As Matcher = NON_ASCII_DIGIT.matcher(result)
			If m.find() Then
				Dim inASCII As New StringBuilder
				For i As Integer = 0 To result.length() - 1
					Dim nextChar As Char = result.Chars(i)
					If Char.IsDigit(nextChar) Then
						Dim d As Integer = Character.digit(nextChar, 10)
						If d <> -1 Then
							inASCII.append(d)
						Else
							inASCII.append(nextChar)
						End If
					Else
						inASCII.append(nextChar)
					End If
				Next i
				result = inASCII.ToString()
			End If

			Return result
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a float value using the <seealso cref="#nextFloat"/>
		''' method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         float value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextFloat() As Boolean
			radix = 10
			Dim result As Boolean = hasNext(floatPattern())
			If result Then ' Cache it
				Try
					Dim s As String = processFloatToken(hasNextResult)
					typeCache = Convert.ToSingle(Convert.ToSingle(s))
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>float</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid float value as
		''' described below. If the translation is successful, the scanner advances
		''' past the input that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Float-regex"><i>Float</i></a> regular expression defined above
		''' then the token is converted into a <tt>float</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Float#parseFloat Float.parseFloat"/>. If the token matches
		''' the localized NaN or infinity strings, then either "Nan" or "Infinity"
		''' is passed to <seealso cref="Float#parseFloat(String) Float.parseFloat"/> as
		''' appropriate.
		''' </summary>
		''' <returns> the <tt>float</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Float</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextFloat() As Single
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Float) Then
				Dim val As Single = CSng(typeCache)
				useTypeCache()
				Return val
			End If
			radix = 10
			clearCaches()
			Try
				Return Convert.ToSingle(processFloatToken([next](floatPattern())))
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a double value using the <seealso cref="#nextDouble"/>
		''' method. The scanner does not advance past any input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         double value </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextDouble() As Boolean
			radix = 10
			Dim result As Boolean = hasNext(floatPattern())
			If result Then ' Cache it
				Try
					Dim s As String = processFloatToken(hasNextResult)
					typeCache = Convert.ToDouble(Convert.ToDouble(s))
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a <tt>double</tt>.
		''' This method will throw <code>InputMismatchException</code>
		''' if the next token cannot be translated into a valid double value.
		''' If the translation is successful, the scanner advances past the input
		''' that matched.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Float-regex"><i>Float</i></a> regular expression defined above
		''' then the token is converted into a <tt>double</tt> value as if by
		''' removing all locale specific prefixes, group separators, and locale
		''' specific suffixes, then mapping non-ASCII digits into ASCII
		''' digits via <seealso cref="Character#digit Character.digit"/>, prepending a
		''' negative sign (-) if the locale specific negative prefixes and suffixes
		''' were present, and passing the resulting string to
		''' <seealso cref="Double#parseDouble java.lang.[Double].parseDouble"/>. If the token matches
		''' the localized NaN or infinity strings, then either "Nan" or "Infinity"
		''' is passed to <seealso cref="Double#parseDouble(String) java.lang.[Double].parseDouble"/> as
		''' appropriate.
		''' </summary>
		''' <returns> the <tt>double</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Float</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextDouble() As Double
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is Double?) Then
				Dim val As Double = CDbl(typeCache)
				useTypeCache()
				Return val
			End If
			radix = 10
			clearCaches()
			' Search for next float
			Try
				Return Convert.ToDouble(processFloatToken([next](floatPattern())))
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		' Convenience methods for scanning multi precision numbers

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a <code>BigInteger</code> in the default radix using the
		''' <seealso cref="#nextBigInteger"/> method. The scanner does not advance past any
		''' input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         <code>BigInteger</code> </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextBigInteger() As Boolean
			Return hasNextBigInteger(defaultRadix)
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a <code>BigInteger</code> in the specified radix using
		''' the <seealso cref="#nextBigInteger"/> method. The scanner does not advance past
		''' any input.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token as an integer </param>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         <code>BigInteger</code> </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextBigInteger(ByVal radix As Integer) As Boolean
			radix = radix
			Dim result As Boolean = hasNext(integerPattern())
			If result Then ' Cache it
				Try
					Dim s As String = If(matcher.group(SIMPLE_GROUP_INDEX) Is Nothing, processIntegerToken(hasNextResult), hasNextResult)
					typeCache = New BigInteger(s, radix)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a {@link java.math.BigInteger
		''' BigInteger}.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>nextBigInteger()</tt> behaves in exactly the same way as the
		''' invocation <tt>nextBigInteger(radix)</tt>, where <code>radix</code>
		''' is the default radix of this scanner.
		''' </summary>
		''' <returns> the <tt>BigInteger</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextBigInteger() As BigInteger
			Return nextBigInteger(defaultRadix)
		End Function

		''' <summary>
		''' Scans the next token of the input as a {@link java.math.BigInteger
		''' BigInteger}.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Integer-regex"><i>Integer</i></a> regular expression defined
		''' above then the token is converted into a <tt>BigInteger</tt> value as if
		''' by removing all group separators, mapping non-ASCII digits into ASCII
		''' digits via the <seealso cref="Character#digit Character.digit"/>, and passing the
		''' resulting string to the {@link
		''' java.math.BigInteger#BigInteger(java.lang.String)
		''' BigInteger(String, int)} constructor with the specified radix.
		''' </summary>
		''' <param name="radix"> the radix used to interpret the token </param>
		''' <returns> the <tt>BigInteger</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Integer</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextBigInteger(ByVal radix As Integer) As BigInteger
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is BigInteger) AndAlso Me.radix_Renamed = radix Then
				Dim val As BigInteger = CType(typeCache, BigInteger)
				useTypeCache()
				Return val
			End If
			radix = radix
			clearCaches()
			' Search for next int
			Try
				Dim s As String = [next](integerPattern())
				If matcher.group(SIMPLE_GROUP_INDEX) Is Nothing Then s = processIntegerToken(s)
				Return New BigInteger(s, radix)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Returns true if the next token in this scanner's input can be
		''' interpreted as a <code>BigDecimal</code> using the
		''' <seealso cref="#nextBigDecimal"/> method. The scanner does not advance past any
		''' input.
		''' </summary>
		''' <returns> true if and only if this scanner's next token is a valid
		'''         <code>BigDecimal</code> </returns>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function hasNextBigDecimal() As Boolean
			radix = 10
			Dim result As Boolean = hasNext(decimalPattern())
			If result Then ' Cache it
				Try
					Dim s As String = processFloatToken(hasNextResult)
					typeCache = New BigDecimal(s)
				Catch nfe As NumberFormatException
					result = False
				End Try
			End If
			Return result
		End Function

		''' <summary>
		''' Scans the next token of the input as a {@link java.math.BigDecimal
		''' BigDecimal}.
		''' 
		''' <p> If the next token matches the <a
		''' href="#Decimal-regex"><i>Decimal</i></a> regular expression defined
		''' above then the token is converted into a <tt>BigDecimal</tt> value as if
		''' by removing all group separators, mapping non-ASCII digits into ASCII
		''' digits via the <seealso cref="Character#digit Character.digit"/>, and passing the
		''' resulting string to the {@link
		''' java.math.BigDecimal#BigDecimal(java.lang.String) BigDecimal(String)}
		''' constructor.
		''' </summary>
		''' <returns> the <tt>BigDecimal</tt> scanned from the input </returns>
		''' <exception cref="InputMismatchException">
		'''         if the next token does not match the <i>Decimal</i>
		'''         regular expression, or is out of range </exception>
		''' <exception cref="NoSuchElementException"> if the input is exhausted </exception>
		''' <exception cref="IllegalStateException"> if this scanner is closed </exception>
		Public Function nextBigDecimal() As BigDecimal
			' Check cached result
			If (typeCache IsNot Nothing) AndAlso (TypeOf typeCache Is BigDecimal) Then
				Dim val As BigDecimal = CDec(typeCache)
				useTypeCache()
				Return val
			End If
			radix = 10
			clearCaches()
			' Search for next float
			Try
				Dim s As String = processFloatToken([next](decimalPattern()))
				Return New BigDecimal(s)
			Catch nfe As NumberFormatException
				position = matcher.start() ' don't skip bad token
				Throw New InputMismatchException(nfe.Message)
			End Try
		End Function

		''' <summary>
		''' Resets this scanner.
		''' 
		''' <p> Resetting a scanner discards all of its explicit state
		''' information which may have been changed by invocations of {@link
		''' #useDelimiter}, <seealso cref="#useLocale"/>, or <seealso cref="#useRadix"/>.
		''' 
		''' <p> An invocation of this method of the form
		''' <tt>scanner.reset()</tt> behaves in exactly the same way as the
		''' invocation
		''' 
		''' <blockquote><pre>{@code
		'''   scanner.useDelimiter("\\p{javaWhitespace}+")
		'''          .useLocale(Locale.getDefault(Locale.Category.FORMAT))
		'''          .useRadix(10);
		''' }</pre></blockquote>
		''' </summary>
		''' <returns> this scanner
		''' 
		''' @since 1.6 </returns>
		Public Function reset() As Scanner
			delimPattern = WHITESPACE_PATTERN
			useLocale(java.util.Locale.getDefault(java.util.Locale.Category.FORMAT))
			useRadix(10)
			clearCaches()
			Return Me
		End Function
	End Class

End Namespace