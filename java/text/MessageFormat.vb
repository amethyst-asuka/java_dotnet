Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text



	''' <summary>
	''' <code>MessageFormat</code> provides a means to produce concatenated
	''' messages in a language-neutral way. Use this to construct messages
	''' displayed for end users.
	''' 
	''' <p>
	''' <code>MessageFormat</code> takes a set of objects, formats them, then
	''' inserts the formatted strings into the pattern at the appropriate places.
	''' 
	''' <p>
	''' <strong>Note:</strong>
	''' <code>MessageFormat</code> differs from the other <code>Format</code>
	''' classes in that you create a <code>MessageFormat</code> object with one
	''' of its constructors (not with a <code>getInstance</code> style factory
	''' method). The factory methods aren't necessary because <code>MessageFormat</code>
	''' itself doesn't implement locale specific behavior. Any locale specific
	''' behavior is defined by the pattern that you provide as well as the
	''' subformats used for inserted arguments.
	''' 
	''' <h3><a name="patterns">Patterns and Their Interpretation</a></h3>
	''' 
	''' <code>MessageFormat</code> uses patterns of the following form:
	''' <blockquote><pre>
	''' <i>MessageFormatPattern:</i>
	'''         <i>String</i>
	'''         <i>MessageFormatPattern</i> <i>FormatElement</i> <i>String</i>
	''' 
	''' <i>FormatElement:</i>
	'''         { <i>ArgumentIndex</i> }
	'''         { <i>ArgumentIndex</i> , <i>FormatType</i> }
	'''         { <i>ArgumentIndex</i> , <i>FormatType</i> , <i>FormatStyle</i> }
	''' 
	''' <i>FormatType: one of </i>
	'''         number date time choice
	''' 
	''' <i>FormatStyle:</i>
	'''         short
	'''         medium
	'''         long
	'''         full
	'''         integer
	'''         currency
	'''         percent
	'''         <i>SubformatPattern</i>
	''' </pre></blockquote>
	''' 
	''' <p>Within a <i>String</i>, a pair of single quotes can be used to
	''' quote any arbitrary characters except single quotes. For example,
	''' pattern string <code>"'{0}'"</code> represents string
	''' <code>"{0}"</code>, not a <i>FormatElement</i>. A single quote itself
	''' must be represented by doubled single quotes {@code ''} throughout a
	''' <i>String</i>.  For example, pattern string <code>"'{''}'"</code> is
	''' interpreted as a sequence of <code>'{</code> (start of quoting and a
	''' left curly brace), <code>''</code> (a single quote), and
	''' <code>}'</code> (a right curly brace and end of quoting),
	''' <em>not</em> <code>'{'</code> and <code>'}'</code> (quoted left and
	''' right curly braces): representing string <code>"{'}"</code>,
	''' <em>not</em> <code>"{}"</code>.
	''' 
	''' <p>A <i>SubformatPattern</i> is interpreted by its corresponding
	''' subformat, and subformat-dependent pattern rules apply. For example,
	''' pattern string <code>"{1,number,<u>$'#',##</u>}"</code>
	''' (<i>SubformatPattern</i> with underline) will produce a number format
	''' with the pound-sign quoted, with a result such as: {@code
	''' "$#31,45"}. Refer to each {@code Format} subclass documentation for
	''' details.
	''' 
	''' <p>Any unmatched quote is treated as closed at the end of the given
	''' pattern. For example, pattern string {@code "'{0}"} is treated as
	''' pattern {@code "'{0}'"}.
	''' 
	''' <p>Any curly braces within an unquoted pattern must be balanced. For
	''' example, <code>"ab {0} de"</code> and <code>"ab '}' de"</code> are
	''' valid patterns, but <code>"ab {0'}' de"</code>, <code>"ab } de"</code>
	''' and <code>"''{''"</code> are not.
	''' 
	''' <dl><dt><b>Warning:</b><dd>The rules for using quotes within message
	''' format patterns unfortunately have shown to be somewhat confusing.
	''' In particular, it isn't always obvious to localizers whether single
	''' quotes need to be doubled or not. Make sure to inform localizers about
	''' the rules, and tell them (for example, by using comments in resource
	''' bundle source files) which strings will be processed by {@code MessageFormat}.
	''' Note that localizers may need to use single quotes in translated
	''' strings where the original version doesn't have them.
	''' </dl>
	''' <p>
	''' The <i>ArgumentIndex</i> value is a non-negative integer written
	''' using the digits {@code '0'} through {@code '9'}, and represents an index into the
	''' {@code arguments} array passed to the {@code format} methods
	''' or the result array returned by the {@code parse} methods.
	''' <p>
	''' The <i>FormatType</i> and <i>FormatStyle</i> values are used to create
	''' a {@code Format} instance for the format element. The following
	''' table shows how the values map to {@code Format} instances. Combinations not
	''' shown in the table are illegal. A <i>SubformatPattern</i> must
	''' be a valid pattern string for the {@code Format} subclass used.
	''' 
	''' <table border=1 summary="Shows how FormatType and FormatStyle values map to Format instances">
	'''    <tr>
	'''       <th id="ft" class="TableHeadingColor">FormatType
	'''       <th id="fs" class="TableHeadingColor">FormatStyle
	'''       <th id="sc" class="TableHeadingColor">Subformat Created
	'''    <tr>
	'''       <td headers="ft"><i>(none)</i>
	'''       <td headers="fs"><i>(none)</i>
	'''       <td headers="sc"><code>null</code>
	'''    <tr>
	'''       <td headers="ft" rowspan=5><code>number</code>
	'''       <td headers="fs"><i>(none)</i>
	'''       <td headers="sc"><seealso cref="NumberFormat#getInstance(Locale) NumberFormat.getInstance"/>{@code (getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>integer</code>
	'''       <td headers="sc"><seealso cref="NumberFormat#getIntegerInstance(Locale) NumberFormat.getIntegerInstance"/>{@code (getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>currency</code>
	'''       <td headers="sc"><seealso cref="NumberFormat#getCurrencyInstance(Locale) NumberFormat.getCurrencyInstance"/>{@code (getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>percent</code>
	'''       <td headers="sc"><seealso cref="NumberFormat#getPercentInstance(Locale) NumberFormat.getPercentInstance"/>{@code (getLocale())}
	'''    <tr>
	'''       <td headers="fs"><i>SubformatPattern</i>
	'''       <td headers="sc">{@code new} <seealso cref="DecimalFormat#DecimalFormat(String,DecimalFormatSymbols) DecimalFormat"/>{@code (subformatPattern,} <seealso cref="DecimalFormatSymbols#getInstance(Locale) DecimalFormatSymbols.getInstance"/>{@code (getLocale()))}
	'''    <tr>
	'''       <td headers="ft" rowspan=6><code>date</code>
	'''       <td headers="fs"><i>(none)</i>
	'''       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>short</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#SHORT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>medium</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>long</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#LONG"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>full</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getDateInstance(int,Locale) DateFormat.getDateInstance"/>{@code (}<seealso cref="DateFormat#FULL"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><i>SubformatPattern</i>
	'''       <td headers="sc">{@code new} <seealso cref="SimpleDateFormat#SimpleDateFormat(String,Locale) SimpleDateFormat"/>{@code (subformatPattern, getLocale())}
	'''    <tr>
	'''       <td headers="ft" rowspan=6><code>time</code>
	'''       <td headers="fs"><i>(none)</i>
	'''       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>short</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#SHORT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>medium</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#DEFAULT"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>long</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#LONG"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><code>full</code>
	'''       <td headers="sc"><seealso cref="DateFormat#getTimeInstance(int,Locale) DateFormat.getTimeInstance"/>{@code (}<seealso cref="DateFormat#FULL"/>{@code , getLocale())}
	'''    <tr>
	'''       <td headers="fs"><i>SubformatPattern</i>
	'''       <td headers="sc">{@code new} <seealso cref="SimpleDateFormat#SimpleDateFormat(String,Locale) SimpleDateFormat"/>{@code (subformatPattern, getLocale())}
	'''    <tr>
	'''       <td headers="ft"><code>choice</code>
	'''       <td headers="fs"><i>SubformatPattern</i>
	'''       <td headers="sc">{@code new} <seealso cref="ChoiceFormat#ChoiceFormat(String) ChoiceFormat"/>{@code (subformatPattern)}
	''' </table>
	''' 
	''' <h4>Usage Information</h4>
	''' 
	''' <p>
	''' Here are some examples of usage.
	''' In real internationalized programs, the message format pattern and other
	''' static strings will, of course, be obtained from resource bundles.
	''' Other parameters will be dynamically determined at runtime.
	''' <p>
	''' The first example uses the static method <code>MessageFormat.format</code>,
	''' which internally creates a <code>MessageFormat</code> for one-time use:
	''' <blockquote><pre>
	''' int planet = 7;
	''' String event = "a disturbance in the Force";
	''' 
	''' String result = MessageFormat.format(
	'''     "At {1,time} on {1,date}, there was {2} on planet {0,number,integer}.",
	'''     planet, new Date(), event);
	''' </pre></blockquote>
	''' The output is:
	''' <blockquote><pre>
	''' At 12:30 PM on Jul 3, 2053, there was a disturbance in the Force on planet 7.
	''' </pre></blockquote>
	''' 
	''' <p>
	''' The following example creates a <code>MessageFormat</code> instance that
	''' can be used repeatedly:
	''' <blockquote><pre>
	''' int fileCount = 1273;
	''' String diskName = "MyDisk";
	''' Object[] testArgs = {new Long(fileCount), diskName};
	''' 
	''' MessageFormat form = new MessageFormat(
	'''     "The disk \"{1}\" contains {0} file(s).");
	''' 
	''' System.out.println(form.format(testArgs));
	''' </pre></blockquote>
	''' The output with different values for <code>fileCount</code>:
	''' <blockquote><pre>
	''' The disk "MyDisk" contains 0 file(s).
	''' The disk "MyDisk" contains 1 file(s).
	''' The disk "MyDisk" contains 1,273 file(s).
	''' </pre></blockquote>
	''' 
	''' <p>
	''' For more sophisticated patterns, you can use a <code>ChoiceFormat</code>
	''' to produce correct forms for singular and plural:
	''' <blockquote><pre>
	''' MessageFormat form = new MessageFormat("The disk \"{1}\" contains {0}.");
	''' double[] filelimits = {0,1,2};
	''' String[] filepart = {"no files","one file","{0,number} files"};
	''' ChoiceFormat fileform = new ChoiceFormat(filelimits, filepart);
	''' form.setFormatByArgumentIndex(0, fileform);
	''' 
	''' int fileCount = 1273;
	''' String diskName = "MyDisk";
	''' Object[] testArgs = {new Long(fileCount), diskName};
	''' 
	''' System.out.println(form.format(testArgs));
	''' </pre></blockquote>
	''' The output with different values for <code>fileCount</code>:
	''' <blockquote><pre>
	''' The disk "MyDisk" contains no files.
	''' The disk "MyDisk" contains one file.
	''' The disk "MyDisk" contains 1,273 files.
	''' </pre></blockquote>
	''' 
	''' <p>
	''' You can create the <code>ChoiceFormat</code> programmatically, as in the
	''' above example, or by using a pattern. See <seealso cref="ChoiceFormat"/>
	''' for more information.
	''' <blockquote><pre>{@code
	''' form.applyPattern(
	'''    "There {0,choice,0#are no files|1#is one file|1<are {0,number,integer} files}.");
	''' }</pre></blockquote>
	''' 
	''' <p>
	''' <strong>Note:</strong> As we see above, the string produced
	''' by a <code>ChoiceFormat</code> in <code>MessageFormat</code> is treated as special;
	''' occurrences of '{' are used to indicate subformats, and cause recursion.
	''' If you create both a <code>MessageFormat</code> and <code>ChoiceFormat</code>
	''' programmatically (instead of using the string patterns), then be careful not to
	''' produce a format that recurses on itself, which will cause an infinite loop.
	''' <p>
	''' When a single argument is parsed more than once in the string, the last match
	''' will be the final result of the parsing.  For example,
	''' <blockquote><pre>
	''' MessageFormat mf = new MessageFormat("{0,number,#.##}, {0,number,#.#}");
	''' Object[] objs = {new Double(3.1415)};
	''' String result = mf.format( objs );
	''' // result now equals "3.14, 3.1"
	''' objs = null;
	''' objs = mf.parse(result, new ParsePosition(0));
	''' // objs now equals {new Double(3.1)}
	''' </pre></blockquote>
	''' 
	''' <p>
	''' Likewise, parsing with a {@code MessageFormat} object using patterns containing
	''' multiple occurrences of the same argument would return the last match.  For
	''' example,
	''' <blockquote><pre>
	''' MessageFormat mf = new MessageFormat("{0}, {0}, {0}");
	''' String forParsing = "x, y, z";
	''' Object[] objs = mf.parse(forParsing, new ParsePosition(0));
	''' // result now equals {new String("z")}
	''' </pre></blockquote>
	''' 
	''' <h4><a name="synchronization">Synchronization</a></h4>
	''' 
	''' <p>
	''' Message formats are not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' </summary>
	''' <seealso cref=          java.util.Locale </seealso>
	''' <seealso cref=          Format </seealso>
	''' <seealso cref=          NumberFormat </seealso>
	''' <seealso cref=          DecimalFormat </seealso>
	''' <seealso cref=          DecimalFormatSymbols </seealso>
	''' <seealso cref=          ChoiceFormat </seealso>
	''' <seealso cref=          DateFormat </seealso>
	''' <seealso cref=          SimpleDateFormat
	''' 
	''' @author       Mark Davis </seealso>

	Public Class MessageFormat
		Inherits Format

		Private Const serialVersionUID As Long = 6479157306784022952L

		''' <summary>
		''' Constructs a MessageFormat for the default
		''' <seealso cref="java.util.Locale.Category#FORMAT FORMAT"/> locale and the
		''' specified pattern.
		''' The constructor first sets the locale, then parses the pattern and
		''' creates a list of subformats for the format elements contained in it.
		''' Patterns and their interpretation are specified in the
		''' <a href="#patterns">class description</a>.
		''' </summary>
		''' <param name="pattern"> the pattern for this message format </param>
		''' <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
		Public Sub New(  pattern As String)
			Me.locale = java.util.Locale.getDefault(java.util.Locale.Category.FORMAT)
			applyPattern(pattern)
		End Sub

		''' <summary>
		''' Constructs a MessageFormat for the specified locale and
		''' pattern.
		''' The constructor first sets the locale, then parses the pattern and
		''' creates a list of subformats for the format elements contained in it.
		''' Patterns and their interpretation are specified in the
		''' <a href="#patterns">class description</a>.
		''' </summary>
		''' <param name="pattern"> the pattern for this message format </param>
		''' <param name="locale"> the locale for this message format </param>
		''' <exception cref="IllegalArgumentException"> if the pattern is invalid
		''' @since 1.4 </exception>
		Public Sub New(  pattern As String,   locale As java.util.Locale)
			Me.locale = locale
			applyPattern(pattern)
		End Sub

		''' <summary>
		''' Sets the locale to be used when creating or comparing subformats.
		''' This affects subsequent calls
		''' <ul>
		''' <li>to the <seealso cref="#applyPattern applyPattern"/>
		'''     and <seealso cref="#toPattern toPattern"/> methods if format elements specify
		'''     a format type and therefore have the subformats created in the
		'''     <code>applyPattern</code> method, as well as
		''' <li>to the <code>format</code> and
		'''     <seealso cref="#formatToCharacterIterator formatToCharacterIterator"/> methods
		'''     if format elements do not specify a format type and therefore have
		'''     the subformats created in the formatting methods.
		''' </ul>
		''' Subformats that have already been created are not affected.
		''' </summary>
		''' <param name="locale"> the locale to be used when creating or comparing subformats </param>
		Public Overridable Property locale As java.util.Locale
			Set(  locale As java.util.Locale)
				Me.locale = locale
			End Set
			Get
				Return locale
			End Get
		End Property



		''' <summary>
		''' Sets the pattern used by this message format.
		''' The method parses the pattern and creates a list of subformats
		''' for the format elements contained in it.
		''' Patterns and their interpretation are specified in the
		''' <a href="#patterns">class description</a>.
		''' </summary>
		''' <param name="pattern"> the pattern for this message format </param>
		''' <exception cref="IllegalArgumentException"> if the pattern is invalid </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub applyPattern(  pattern As String) ' fallthrough in switch is expected, suppress it
				Dim segments As StringBuilder() = New StringBuilder(3){}
				' Allocate only segments[SEG_RAW] here. The rest are
				' allocated on demand.
				segments(SEG_RAW) = New StringBuilder

				Dim part As Integer = SEG_RAW
				Dim formatNumber As Integer = 0
				Dim inQuote As Boolean = False
				Dim braceStack As Integer = 0
				maxOffset = -1
				For i As Integer = 0 To pattern.length() - 1
					Dim ch As Char = pattern.Chars(i)
					If part = SEG_RAW Then
						If ch = "'"c Then
							If i + 1 < pattern.length() AndAlso pattern.Chars(i+1) = "'"c Then
								segments(part).append(ch) ' handle doubles
								i += 1
							Else
								inQuote = Not inQuote
							End If
						ElseIf ch = "{"c AndAlso (Not inQuote) Then
							part = SEG_INDEX
							If segments(SEG_INDEX) Is Nothing Then segments(SEG_INDEX) = New StringBuilder
						Else
							segments(part).append(ch)
						End If
					Else
						If inQuote Then ' just copy quotes in parts
							segments(part).append(ch)
							If ch = "'"c Then inQuote = False
						Else
							Select Case ch
							Case ","c
								If part < SEG_MODIFIER Then
									part += 1
									If segments(part) Is Nothing Then segments(part) = New StringBuilder
								Else
									segments(part).append(ch)
								End If
							Case "{"c
								braceStack += 1
								segments(part).append(ch)
							Case "}"c
								If braceStack = 0 Then
									part = SEG_RAW
									makeFormat(i, formatNumber, segments)
									formatNumber += 1
									' throw away other segments
									segments(SEG_INDEX) = Nothing
									segments(SEG_TYPE) = Nothing
									segments(SEG_MODIFIER) = Nothing
								Else
									braceStack -= 1
									segments(part).append(ch)
								End If
							Case " "c
								' Skip any leading space chars for SEG_TYPE.
								If part <> SEG_TYPE OrElse segments(SEG_TYPE).length() > 0 Then segments(part).append(ch)
							Case "'"c
								inQuote = True
								' fall through, so we keep quotes in other parts
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
							Case Else
								segments(part).append(ch)
							End Select
						End If
					End If
				Next i
				If braceStack = 0 AndAlso part <> 0 Then
					maxOffset = -1
					Throw New IllegalArgumentException("Unmatched braces in the pattern.")
				End If
				Me.pattern = segments(0).ToString()
		End Sub


		''' <summary>
		''' Returns a pattern representing the current state of the message format.
		''' The string is constructed from internal information and therefore
		''' does not necessarily equal the previously applied pattern.
		''' </summary>
		''' <returns> a pattern representing the current state of the message format </returns>
		Public Overridable Function toPattern() As String
			' later, make this more extensible
			Dim lastOffset As Integer = 0
			Dim result As New StringBuilder
			For i As Integer = 0 To maxOffset
				copyAndFixQuotes(pattern, lastOffset, offsets(i), result)
				lastOffset = offsets(i)
				result.append("{"c).append(argumentNumbers(i))
				Dim fmt As Format = formats(i)
				If fmt Is Nothing Then
					' do nothing, string format
				ElseIf TypeOf fmt Is NumberFormat Then
					If fmt.Equals(NumberFormat.getInstance(locale)) Then
						result.append(",number")
					ElseIf fmt.Equals(NumberFormat.getCurrencyInstance(locale)) Then
						result.append(",number,currency")
					ElseIf fmt.Equals(NumberFormat.getPercentInstance(locale)) Then
						result.append(",number,percent")
					ElseIf fmt.Equals(NumberFormat.getIntegerInstance(locale)) Then
						result.append(",number,integer")
					Else
						If TypeOf fmt Is java.text.DecimalFormat Then
							result.append(",number,").append(CType(fmt, java.text.DecimalFormat).toPattern())
						ElseIf TypeOf fmt Is ChoiceFormat Then
							result.append(",choice,").append(CType(fmt, ChoiceFormat).toPattern())
						Else
							' UNKNOWN
						End If
					End If
				ElseIf TypeOf fmt Is DateFormat Then
					Dim index As Integer
					For index = MODIFIER_DEFAULT To DATE_TIME_MODIFIERS.Length - 1
						Dim df As DateFormat = DateFormat.getDateInstance(DATE_TIME_MODIFIERS(index), locale)
						If fmt.Equals(df) Then
							result.append(",date")
							Exit For
						End If
						df = DateFormat.getTimeInstance(DATE_TIME_MODIFIERS(index), locale)
						If fmt.Equals(df) Then
							result.append(",time")
							Exit For
						End If
					Next index
					If index >= DATE_TIME_MODIFIERS.Length Then
						If TypeOf fmt Is SimpleDateFormat Then
							result.append(",date,").append(CType(fmt, SimpleDateFormat).toPattern())
						Else
							' UNKNOWN
						End If
					ElseIf index <> MODIFIER_DEFAULT Then
						result.append(","c).append(DATE_TIME_MODIFIER_KEYWORDS(index))
					End If
				Else
					'result.append(", unknown");
				End If
				result.append("}"c)
			Next i
			copyAndFixQuotes(pattern, lastOffset, pattern.length(), result)
			Return result.ToString()
		End Function

		''' <summary>
		''' Sets the formats to use for the values passed into
		''' <code>format</code> methods or returned from <code>parse</code>
		''' methods. The indices of elements in <code>newFormats</code>
		''' correspond to the argument indices used in the previously set
		''' pattern string.
		''' The order of formats in <code>newFormats</code> thus corresponds to
		''' the order of elements in the <code>arguments</code> array passed
		''' to the <code>format</code> methods or the result array returned
		''' by the <code>parse</code> methods.
		''' <p>
		''' If an argument index is used for more than one format element
		''' in the pattern string, then the corresponding new format is used
		''' for all such format elements. If an argument index is not used
		''' for any format element in the pattern string, then the
		''' corresponding new format is ignored. If fewer formats are provided
		''' than needed, then only the formats for argument indices less
		''' than <code>newFormats.length</code> are replaced.
		''' </summary>
		''' <param name="newFormats"> the new formats to use </param>
		''' <exception cref="NullPointerException"> if <code>newFormats</code> is null
		''' @since 1.4 </exception>
		Public Overridable Property formatsByArgumentIndex As Format()
			Set(  newFormats As Format())
				For i As Integer = 0 To maxOffset
					Dim j As Integer = argumentNumbers(i)
					If j < newFormats.Length Then formats(i) = newFormats(j)
				Next i
			End Set
			Get
				Dim maximumArgumentNumber As Integer = -1
				For i As Integer = 0 To maxOffset
					If argumentNumbers(i) > maximumArgumentNumber Then maximumArgumentNumber = argumentNumbers(i)
				Next i
				Dim resultArray As Format() = New Format(maximumArgumentNumber){}
				For i As Integer = 0 To maxOffset
					resultArray(argumentNumbers(i)) = formats(i)
				Next i
				Return resultArray
			End Get
		End Property

		''' <summary>
		''' Sets the formats to use for the format elements in the
		''' previously set pattern string.
		''' The order of formats in <code>newFormats</code> corresponds to
		''' the order of format elements in the pattern string.
		''' <p>
		''' If more formats are provided than needed by the pattern string,
		''' the remaining ones are ignored. If fewer formats are provided
		''' than needed, then only the first <code>newFormats.length</code>
		''' formats are replaced.
		''' <p>
		''' Since the order of format elements in a pattern string often
		''' changes during localization, it is generally better to use the
		''' <seealso cref="#setFormatsByArgumentIndex setFormatsByArgumentIndex"/>
		''' method, which assumes an order of formats corresponding to the
		''' order of elements in the <code>arguments</code> array passed to
		''' the <code>format</code> methods or the result array returned by
		''' the <code>parse</code> methods.
		''' </summary>
		''' <param name="newFormats"> the new formats to use </param>
		''' <exception cref="NullPointerException"> if <code>newFormats</code> is null </exception>
		Public Overridable Property formats As Format()
			Set(  newFormats As Format())
				Dim runsToCopy As Integer = newFormats.Length
				If runsToCopy > maxOffset + 1 Then runsToCopy = maxOffset + 1
				For i As Integer = 0 To runsToCopy - 1
					formats(i) = newFormats(i)
				Next i
			End Set
			Get
				Dim resultArray As Format() = New Format(maxOffset){}
				Array.Copy(formats, 0, resultArray, 0, maxOffset + 1)
				Return resultArray
			End Get
		End Property

		''' <summary>
		''' Sets the format to use for the format elements within the
		''' previously set pattern string that use the given argument
		''' index.
		''' The argument index is part of the format element definition and
		''' represents an index into the <code>arguments</code> array passed
		''' to the <code>format</code> methods or the result array returned
		''' by the <code>parse</code> methods.
		''' <p>
		''' If the argument index is used for more than one format element
		''' in the pattern string, then the new format is used for all such
		''' format elements. If the argument index is not used for any format
		''' element in the pattern string, then the new format is ignored.
		''' </summary>
		''' <param name="argumentIndex"> the argument index for which to use the new format </param>
		''' <param name="newFormat"> the new format to use
		''' @since 1.4 </param>
		Public Overridable Sub setFormatByArgumentIndex(  argumentIndex As Integer,   newFormat As Format)
			For j As Integer = 0 To maxOffset
				If argumentNumbers(j) = argumentIndex Then formats(j) = newFormat
			Next j
		End Sub

		''' <summary>
		''' Sets the format to use for the format element with the given
		''' format element index within the previously set pattern string.
		''' The format element index is the zero-based number of the format
		''' element counting from the start of the pattern string.
		''' <p>
		''' Since the order of format elements in a pattern string often
		''' changes during localization, it is generally better to use the
		''' <seealso cref="#setFormatByArgumentIndex setFormatByArgumentIndex"/>
		''' method, which accesses format elements based on the argument
		''' index they specify.
		''' </summary>
		''' <param name="formatElementIndex"> the index of a format element within the pattern </param>
		''' <param name="newFormat"> the format to use for the specified format element </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code formatElementIndex} is equal to or
		'''            larger than the number of format elements in the pattern string </exception>
		Public Overridable Sub setFormat(  formatElementIndex As Integer,   newFormat As Format)
			formats(formatElementIndex) = newFormat
		End Sub



		''' <summary>
		''' Formats an array of objects and appends the <code>MessageFormat</code>'s
		''' pattern, with format elements replaced by the formatted objects, to the
		''' provided <code>StringBuffer</code>.
		''' <p>
		''' The text substituted for the individual format elements is derived from
		''' the current subformat of the format element and the
		''' <code>arguments</code> element at the format element's argument index
		''' as indicated by the first matching line of the following table. An
		''' argument is <i>unavailable</i> if <code>arguments</code> is
		''' <code>null</code> or has fewer than argumentIndex+1 elements.
		''' 
		''' <table border=1 summary="Examples of subformat,argument,and formatted text">
		'''    <tr>
		'''       <th>Subformat
		'''       <th>Argument
		'''       <th>Formatted Text
		'''    <tr>
		'''       <td><i>any</i>
		'''       <td><i>unavailable</i>
		'''       <td><code>"{" + argumentIndex + "}"</code>
		'''    <tr>
		'''       <td><i>any</i>
		'''       <td><code>null</code>
		'''       <td><code>"null"</code>
		'''    <tr>
		'''       <td><code>instanceof ChoiceFormat</code>
		'''       <td><i>any</i>
		'''       <td><code>subformat.format(argument).indexOf('{') &gt;= 0 ?<br>
		'''           (new MessageFormat(subformat.format(argument), getLocale())).format(argument) :
		'''           subformat.format(argument)</code>
		'''    <tr>
		'''       <td><code>!= null</code>
		'''       <td><i>any</i>
		'''       <td><code>subformat.format(argument)</code>
		'''    <tr>
		'''       <td><code>null</code>
		'''       <td><code>instanceof Number</code>
		'''       <td><code>NumberFormat.getInstance(getLocale()).format(argument)</code>
		'''    <tr>
		'''       <td><code>null</code>
		'''       <td><code>instanceof Date</code>
		'''       <td><code>DateFormat.getDateTimeInstance(DateFormat.SHORT, DateFormat.SHORT, getLocale()).format(argument)</code>
		'''    <tr>
		'''       <td><code>null</code>
		'''       <td><code>instanceof String</code>
		'''       <td><code>argument</code>
		'''    <tr>
		'''       <td><code>null</code>
		'''       <td><i>any</i>
		'''       <td><code>argument.toString()</code>
		''' </table>
		''' <p>
		''' If <code>pos</code> is non-null, and refers to
		''' <code>Field.ARGUMENT</code>, the location of the first formatted
		''' string will be returned.
		''' </summary>
		''' <param name="arguments"> an array of objects to be formatted and substituted. </param>
		''' <param name="result"> where text is appended. </param>
		''' <param name="pos"> On input: an alignment field, if desired.
		'''            On output: the offsets of the alignment field. </param>
		''' <returns> the string buffer passed in as {@code result}, with formatted
		''' text appended </returns>
		''' <exception cref="IllegalArgumentException"> if an argument in the
		'''            <code>arguments</code> array is not of the type
		'''            expected by the format element(s) that use it. </exception>
		Public NotOverridable Overrides Function format(  arguments As Object(),   result As StringBuffer,   pos As FieldPosition) As StringBuffer
			Return subformat(arguments, result, pos, Nothing)
		End Function

		''' <summary>
		''' Creates a MessageFormat with the given pattern and uses it
		''' to format the given arguments. This is equivalent to
		''' <blockquote>
		'''     <code>(new <seealso cref="#MessageFormat(String) MessageFormat"/>(pattern)).<seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>(arguments, new StringBuffer(), null).toString()</code>
		''' </blockquote>
		''' </summary>
		''' <param name="pattern">   the pattern string </param>
		''' <param name="arguments"> object(s) to format </param>
		''' <returns> the formatted string </returns>
		''' <exception cref="IllegalArgumentException"> if the pattern is invalid,
		'''            or if an argument in the <code>arguments</code> array
		'''            is not of the type expected by the format element(s)
		'''            that use it. </exception>
		Public Shared Function format(  pattern As String, ParamArray   arguments As Object()) As String
			Dim temp As New MessageFormat(pattern)
			Return temp.format(arguments)
		End Function

		' Overrides
		''' <summary>
		''' Formats an array of objects and appends the <code>MessageFormat</code>'s
		''' pattern, with format elements replaced by the formatted objects, to the
		''' provided <code>StringBuffer</code>.
		''' This is equivalent to
		''' <blockquote>
		'''     <code><seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>((Object[]) arguments, result, pos)</code>
		''' </blockquote>
		''' </summary>
		''' <param name="arguments"> an array of objects to be formatted and substituted. </param>
		''' <param name="result"> where text is appended. </param>
		''' <param name="pos"> On input: an alignment field, if desired.
		'''            On output: the offsets of the alignment field. </param>
		''' <exception cref="IllegalArgumentException"> if an argument in the
		'''            <code>arguments</code> array is not of the type
		'''            expected by the format element(s) that use it. </exception>
		Public NotOverridable Overrides Function format(  arguments As Object,   result As StringBuffer,   pos As FieldPosition) As StringBuffer
			Return subformat(CType(arguments, Object()), result, pos, Nothing)
		End Function

		''' <summary>
		''' Formats an array of objects and inserts them into the
		''' <code>MessageFormat</code>'s pattern, producing an
		''' <code>AttributedCharacterIterator</code>.
		''' You can use the returned <code>AttributedCharacterIterator</code>
		''' to build the resulting String, as well as to determine information
		''' about the resulting String.
		''' <p>
		''' The text of the returned <code>AttributedCharacterIterator</code> is
		''' the same that would be returned by
		''' <blockquote>
		'''     <code><seealso cref="#format(java.lang.Object[], java.lang.StringBuffer, java.text.FieldPosition) format"/>(arguments, new StringBuffer(), null).toString()</code>
		''' </blockquote>
		''' <p>
		''' In addition, the <code>AttributedCharacterIterator</code> contains at
		''' least attributes indicating where text was generated from an
		''' argument in the <code>arguments</code> array. The keys of these attributes are of
		''' type <code>MessageFormat.Field</code>, their values are
		''' <code>Integer</code> objects indicating the index in the <code>arguments</code>
		''' array of the argument from which the text was generated.
		''' <p>
		''' The attributes/value from the underlying <code>Format</code>
		''' instances that <code>MessageFormat</code> uses will also be
		''' placed in the resulting <code>AttributedCharacterIterator</code>.
		''' This allows you to not only find where an argument is placed in the
		''' resulting String, but also which fields it contains in turn.
		''' </summary>
		''' <param name="arguments"> an array of objects to be formatted and substituted. </param>
		''' <returns> AttributedCharacterIterator describing the formatted value. </returns>
		''' <exception cref="NullPointerException"> if <code>arguments</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if an argument in the
		'''            <code>arguments</code> array is not of the type
		'''            expected by the format element(s) that use it.
		''' @since 1.4 </exception>
		Public Overrides Function formatToCharacterIterator(  arguments As Object) As AttributedCharacterIterator
			Dim result As New StringBuffer
			Dim iterators As New List(Of AttributedCharacterIterator)

			If arguments Is Nothing Then Throw New NullPointerException("formatToCharacterIterator must be passed non-null object")
			subformat(CType(arguments, Object()), result, Nothing, iterators)
			If iterators.Count = 0 Then Return createAttributedCharacterIterator("")
			Return createAttributedCharacterIterator(iterators.ToArray())
		End Function

		''' <summary>
		''' Parses the string.
		''' 
		''' <p>Caveats: The parse may fail in a number of circumstances.
		''' For example:
		''' <ul>
		''' <li>If one of the arguments does not occur in the pattern.
		''' <li>If the format of an argument loses information, such as
		'''     with a choice format where a large number formats to "many".
		''' <li>Does not yet handle recursion (where
		'''     the substituted strings contain {n} references.)
		''' <li>Will not always find a match (or the correct match)
		'''     if some part of the parse is ambiguous.
		'''     For example, if the pattern "{1},{2}" is used with the
		'''     string arguments {"a,b", "c"}, it will format as "a,b,c".
		'''     When the result is parsed, it will return {"a", "b,c"}.
		''' <li>If a single argument is parsed more than once in the string,
		'''     then the later parse wins.
		''' </ul>
		''' When the parse fails, use ParsePosition.getErrorIndex() to find out
		''' where in the string the parsing failed.  The returned error
		''' index is the starting offset of the sub-patterns that the string
		''' is comparing with.  For example, if the parsing string "AAA {0} BBB"
		''' is comparing against the pattern "AAD {0} BBB", the error index is
		''' 0. When an error occurs, the call to this method will return null.
		''' If the source is null, return an empty array.
		''' </summary>
		''' <param name="source"> the string to parse </param>
		''' <param name="pos">    the parse position </param>
		''' <returns> an array of parsed objects </returns>
		Public Overridable Function parse(  source As String,   pos As ParsePosition) As Object()
			If source Is Nothing Then
				Dim empty As Object() = {}
				Return empty
			End If

			Dim maximumArgumentNumber As Integer = -1
			For i As Integer = 0 To maxOffset
				If argumentNumbers(i) > maximumArgumentNumber Then maximumArgumentNumber = argumentNumbers(i)
			Next i
			Dim resultArray As Object() = New Object(maximumArgumentNumber){}

			Dim patternOffset As Integer = 0
			Dim sourceOffset As Integer = pos.index
			Dim tempStatus As New ParsePosition(0)
			For i As Integer = 0 To maxOffset
				' match up to format
				Dim len As Integer = offsets(i) - patternOffset
				If len = 0 OrElse pattern.regionMatches(patternOffset, source, sourceOffset, len) Then
					sourceOffset += len
					patternOffset += len
				Else
					pos.errorIndex = sourceOffset
					Return Nothing ' leave index as is to signal error
				End If

				' now use format
				If formats(i) Is Nothing Then ' string format
					' if at end, use longest possible match
					' otherwise uses first match to intervening string
					' does NOT recursively try all possibilities
					Dim tempLength As Integer = If(i <> maxOffset, offsets(i+1), pattern.length())

					Dim [next] As Integer
					If patternOffset >= tempLength Then
						[next] = source.length()
					Else
						[next] = source.IndexOf(pattern.Substring(patternOffset, tempLength - patternOffset), sourceOffset)
					End If

					If [next] < 0 Then
						pos.errorIndex = sourceOffset
						Return Nothing ' leave index as is to signal error
					Else
						Dim strValue As String= source.Substring(sourceOffset, [next] - sourceOffset)
						If Not strValue.Equals("{" & argumentNumbers(i) & "}") Then resultArray(argumentNumbers(i)) = source.Substring(sourceOffset, [next] - sourceOffset)
						sourceOffset = [next]
					End If
				Else
					tempStatus.index = sourceOffset
					resultArray(argumentNumbers(i)) = formats(i).parseObject(source,tempStatus)
					If tempStatus.index = sourceOffset Then
						pos.errorIndex = sourceOffset
						Return Nothing ' leave index as is to signal error
					End If
					sourceOffset = tempStatus.index ' update
				End If
			Next i
			Dim len As Integer = pattern.length() - patternOffset
			If len = 0 OrElse pattern.regionMatches(patternOffset, source, sourceOffset, len) Then
				pos.index = sourceOffset + len
			Else
				pos.errorIndex = sourceOffset
				Return Nothing ' leave index as is to signal error
			End If
			Return resultArray
		End Function

		''' <summary>
		''' Parses text from the beginning of the given string to produce an object
		''' array.
		''' The method may not use the entire text of the given string.
		''' <p>
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on message parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		''' <returns> An <code>Object</code> array parsed from the string. </returns>
		''' <exception cref="ParseException"> if the beginning of the specified string
		'''            cannot be parsed. </exception>
		Public Overridable Function parse(  source As String) As Object()
			Dim pos As New ParsePosition(0)
			Dim result As Object() = parse(source, pos)
			If pos.index = 0 Then ' unchanged, returned object is null Throw New ParseException("MessageFormat parse error!", pos.errorIndex)

			Return result
		End Function

		''' <summary>
		''' Parses text from a string to produce an object array.
		''' <p>
		''' The method attempts to parse text starting at the index given by
		''' <code>pos</code>.
		''' If parsing succeeds, then the index of <code>pos</code> is updated
		''' to the index after the last character used (parsing does not necessarily
		''' use all characters up to the end of the string), and the parsed
		''' object array is returned. The updated <code>pos</code> can be used to
		''' indicate the starting point for the next call to this method.
		''' If an error occurs, then the index of <code>pos</code> is not
		''' changed, the error index of <code>pos</code> is set to the index of
		''' the character where the error occurred, and null is returned.
		''' <p>
		''' See the <seealso cref="#parse(String, ParsePosition)"/> method for more information
		''' on message parsing.
		''' </summary>
		''' <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		''' <param name="pos"> A <code>ParsePosition</code> object with index and error
		'''            index information as described above. </param>
		''' <returns> An <code>Object</code> array parsed from the string. In case of
		'''         error, returns null. </returns>
		''' <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		Public Overrides Function parseObject(  source As String,   pos As ParsePosition) As Object
			Return parse(source, pos)
		End Function

		''' <summary>
		''' Creates and returns a copy of this object.
		''' </summary>
		''' <returns> a clone of this instance. </returns>
		Public Overrides Function clone() As Object
			Dim other As MessageFormat = CType(MyBase.clone(), MessageFormat)

			' clone arrays. Can't do with utility because of bug in Cloneable
			other.formats = formats.clone() ' shallow clone
			For i As Integer = 0 To formats.Length - 1
				If formats(i) IsNot Nothing Then other.formats(i) = CType(formats(i).clone(), Format)
			Next i
			' for primitives or immutables, shallow clone is enough
			other.offsets = offsets.clone()
			other.argumentNumbers = argumentNumbers.clone()

			Return other
		End Function

		''' <summary>
		''' Equality comparison between two message format objects
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then ' quick check Return True
			If obj Is Nothing OrElse Me.GetType() IsNot obj.GetType() Then Return False
			Dim other As MessageFormat = CType(obj, MessageFormat)
			Return (maxOffset = other.maxOffset AndAlso pattern.Equals(other.pattern) AndAlso ((locale IsNot Nothing AndAlso locale.Equals(other.locale)) OrElse (locale Is Nothing AndAlso other.locale Is Nothing)) AndAlso java.util.Arrays.Equals(offsets,other.offsets) AndAlso java.util.Arrays.Equals(argumentNumbers,other.argumentNumbers) AndAlso java.util.Arrays.Equals(formats,other.formats))
		End Function

		''' <summary>
		''' Generates a hash code for the message format object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return pattern.GetHashCode() ' enough for reasonable distribution
		End Function


		''' <summary>
		''' Defines constants that are used as attribute keys in the
		''' <code>AttributedCharacterIterator</code> returned
		''' from <code>MessageFormat.formatToCharacterIterator</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Class Field
			Inherits Format.Field

			' Proclaim serial compatibility with 1.4 FCS
			Private Const serialVersionUID As Long = 7899943957617360810L

			''' <summary>
			''' Creates a Field with the specified name.
			''' </summary>
			''' <param name="name"> Name of the attribute </param>
			Protected Friend Sub New(  name As String)
				MyBase.New(name)
			End Sub

			''' <summary>
			''' Resolves instances being deserialized to the predefined constants.
			''' </summary>
			''' <exception cref="InvalidObjectException"> if the constant could not be
			'''         resolved. </exception>
			''' <returns> resolved MessageFormat.Field constant </returns>
			Protected Friend Overridable Function readResolve() As Object
				If Me.GetType() IsNot GetType(MessageFormat.Field) Then Throw New java.io.InvalidObjectException("subclass didn't correctly implement readResolve")

				Return ARGUMENT
			End Function

			'
			' The constants
			'

			''' <summary>
			''' Constant identifying a portion of a message that was generated
			''' from an argument passed into <code>formatToCharacterIterator</code>.
			''' The value associated with the key will be an <code>Integer</code>
			''' indicating the index in the <code>arguments</code> array of the
			''' argument from which the text was generated.
			''' </summary>
			Public Shared ReadOnly ARGUMENT As New Field("message argument field")
		End Class

		' ===========================privates============================

		''' <summary>
		''' The locale to use for formatting numbers and dates.
		''' @serial
		''' </summary>
		Private locale As java.util.Locale

		''' <summary>
		''' The string that the formatted values are to be plugged into.  In other words, this
		''' is the pattern supplied on construction with all of the {} expressions taken out.
		''' @serial
		''' </summary>
		Private pattern As String = ""

		''' <summary>
		''' The initially expected number of subformats in the format </summary>
		Private Const INITIAL_FORMATS As Integer = 10

		''' <summary>
		''' An array of formatters, which are used to format the arguments.
		''' @serial
		''' </summary>
		Private formats As Format() = New Format(INITIAL_FORMATS - 1){}

		''' <summary>
		''' The positions where the results of formatting each argument are to be inserted
		''' into the pattern.
		''' @serial
		''' </summary>
		Private offsets As Integer() = New Integer(INITIAL_FORMATS - 1){}

		''' <summary>
		''' The argument numbers corresponding to each formatter.  (The formatters are stored
		''' in the order they occur in the pattern, not in the order in which the arguments
		''' are specified.)
		''' @serial
		''' </summary>
		Private argumentNumbers As Integer() = New Integer(INITIAL_FORMATS - 1){}

		''' <summary>
		''' One less than the number of entries in <code>offsets</code>.  Can also be thought of
		''' as the index of the highest-numbered element in <code>offsets</code> that is being used.
		''' All of these arrays should have the same number of elements being used as <code>offsets</code>
		''' does, and so this variable suffices to tell us how many entries are in all of them.
		''' @serial
		''' </summary>
		Private maxOffset As Integer = -1

		''' <summary>
		''' Internal routine used by format. If <code>characterIterators</code> is
		''' non-null, AttributedCharacterIterator will be created from the
		''' subformats as necessary. If <code>characterIterators</code> is null
		''' and <code>fp</code> is non-null and identifies
		''' <code>Field.MESSAGE_ARGUMENT</code>, the location of
		''' the first replaced argument will be set in it.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if an argument in the
		'''            <code>arguments</code> array is not of the type
		'''            expected by the format element(s) that use it. </exception>
		Private Function subformat(  arguments As Object(),   result As StringBuffer,   fp As FieldPosition,   characterIterators As IList(Of AttributedCharacterIterator)) As StringBuffer
			' note: this implementation assumes a fast substring & index.
			' if this is not true, would be better to append chars one by one.
			Dim lastOffset As Integer = 0
			Dim last As Integer = result.length()
			For i As Integer = 0 To maxOffset
				result.append(pattern.Substring(lastOffset, offsets(i) - lastOffset))
				lastOffset = offsets(i)
				Dim argumentNumber As Integer = argumentNumbers(i)
				If arguments Is Nothing OrElse argumentNumber >= arguments.Length Then
					result.append("{"c).append(argumentNumber).append("}"c)
					Continue For
				End If
				' int argRecursion = ((recursionProtection >> (argumentNumber*2)) & 0x3);
				If False Then ' if (argRecursion == 3){
					' prevent loop!!!
					result.append(ChrW(&HFFFD))
				Else
					Dim obj As Object = arguments(argumentNumber)
					Dim arg As String = Nothing
					Dim subFormatter As Format = Nothing
					If obj Is Nothing Then
						arg = "null"
					ElseIf formats(i) IsNot Nothing Then
						subFormatter = formats(i)
						If TypeOf subFormatter Is ChoiceFormat Then
							arg = formats(i).format(obj)
							If arg.IndexOf("{"c) >= 0 Then
								subFormatter = New MessageFormat(arg, locale)
								obj = arguments
								arg = Nothing
							End If
						End If
					ElseIf TypeOf obj Is Number Then
						' format number if can
						subFormatter = NumberFormat.getInstance(locale)
					ElseIf TypeOf obj Is DateTime? Then
						' format a Date if can
						subFormatter = DateFormat.getDateTimeInstance(DateFormat.SHORT, DateFormat.SHORT, locale) 'fix
					ElseIf TypeOf obj Is String Then
						arg = CStr(obj)

					Else
						arg = obj.ToString()
						If arg Is Nothing Then arg = "null"
					End If

					' At this point we are in two states, either subFormatter
					' is non-null indicating we should format obj using it,
					' or arg is non-null and we should use it as the value.

					If characterIterators IsNot Nothing Then
						' If characterIterators is non-null, it indicates we need
						' to get the CharacterIterator from the child formatter.
						If last IsNot result.length() Then
							characterIterators.Add(createAttributedCharacterIterator(result.Substring(last)))
							last = result.length()
						End If
						If subFormatter IsNot Nothing Then
							Dim subIterator As AttributedCharacterIterator = subFormatter.formatToCharacterIterator(obj)

							append(result, subIterator)
							If last IsNot result.length() Then
								characterIterators.Add(createAttributedCharacterIterator(subIterator, Field.ARGUMENT, Convert.ToInt32(argumentNumber)))
								last = result.length()
							End If
							arg = Nothing
						End If
						If arg IsNot Nothing AndAlso arg.length() > 0 Then
							result.append(arg)
							characterIterators.Add(createAttributedCharacterIterator(arg, Field.ARGUMENT, Convert.ToInt32(argumentNumber)))
							last = result.length()
						End If
					Else
						If subFormatter IsNot Nothing Then arg = subFormatter.format(obj)
						last = result.length()
						result.append(arg)
						If i = 0 AndAlso fp IsNot Nothing AndAlso Field.ARGUMENT.Equals(fp.fieldAttribute) Then
							fp.beginIndex = last
							fp.endIndex = result.length()
						End If
						last = result.length()
					End If
				End If
			Next i
			result.append(pattern.Substring(lastOffset, pattern.length() - lastOffset))
			If characterIterators IsNot Nothing AndAlso last IsNot result.length() Then characterIterators.Add(createAttributedCharacterIterator(result.Substring(last)))
			Return result
		End Function

		''' <summary>
		''' Convenience method to append all the characters in
		''' <code>iterator</code> to the StringBuffer <code>result</code>.
		''' </summary>
		Private Sub append(  result As StringBuffer,   [iterator] As CharacterIterator)
			If [iterator].first() <> CharacterIterator.DONE Then
				Dim aChar As Char

				result.append([iterator].first())
				aChar = [iterator].next()
				Do While aChar <> CharacterIterator.DONE
					result.append(aChar)
					aChar = [iterator].next()
				Loop
			End If
		End Sub

		' Indices for segments
		Private Const SEG_RAW As Integer = 0
		Private Const SEG_INDEX As Integer = 1
		Private Const SEG_TYPE As Integer = 2
		Private Const SEG_MODIFIER As Integer = 3 ' modifier or subformat

		' Indices for type keywords
		Private Const TYPE_NULL As Integer = 0
		Private Const TYPE_NUMBER As Integer = 1
		Private Const TYPE_DATE As Integer = 2
		Private Const TYPE_TIME As Integer = 3
		Private Const TYPE_CHOICE As Integer = 4

		Private Shared ReadOnly TYPE_KEYWORDS As String() = { "", "number", "date", "time", "choice" }

		' Indices for number modifiers
		Private Const MODIFIER_DEFAULT As Integer = 0 ' common in number and date-time
		Private Const MODIFIER_CURRENCY As Integer = 1
		Private Const MODIFIER_PERCENT As Integer = 2
		Private Const MODIFIER_INTEGER As Integer = 3

		Private Shared ReadOnly NUMBER_MODIFIER_KEYWORDS As String() = { "", "currency", "percent", "integer" }

		' Indices for date-time modifiers
		Private Const MODIFIER_SHORT As Integer = 1
		Private Const MODIFIER_MEDIUM As Integer = 2
		Private Const MODIFIER_LONG As Integer = 3
		Private Const MODIFIER_FULL As Integer = 4

		Private Shared ReadOnly DATE_TIME_MODIFIER_KEYWORDS As String() = { "", "short", "medium", "long", "full" }

		' Date-time style values corresponding to the date-time modifiers.
		Private Shared ReadOnly DATE_TIME_MODIFIERS As Integer() = { DateFormat.DEFAULT, DateFormat.SHORT, DateFormat.MEDIUM, DateFormat.LONG, DateFormat.FULL }

		Private Sub makeFormat(  position As Integer,   offsetNumber As Integer,   textSegments As StringBuilder())
			Dim segments As String() = New String(textSegments.Length - 1){}
			For i As Integer = 0 To textSegments.Length - 1
				Dim oneseg As StringBuilder = textSegments(i)
				segments(i) = If(oneseg IsNot Nothing, oneseg.ToString(), "")
			Next i

			' get the argument number
			Dim argumentNumber As Integer
			Try
				argumentNumber = Convert.ToInt32(segments(SEG_INDEX)) ' always unlocalized!
			Catch e As NumberFormatException
				Throw New IllegalArgumentException("can't parse argument number: " & segments(SEG_INDEX), e)
			End Try
			If argumentNumber < 0 Then Throw New IllegalArgumentException("negative argument number: " & argumentNumber)

			' resize format information arrays if necessary
			If offsetNumber >= formats.Length Then
				Dim newLength As Integer = formats.Length * 2
				Dim newFormats As Format() = New Format(newLength - 1){}
				Dim newOffsets As Integer() = New Integer(newLength - 1){}
				Dim newArgumentNumbers As Integer() = New Integer(newLength - 1){}
				Array.Copy(formats, 0, newFormats, 0, maxOffset + 1)
				Array.Copy(offsets, 0, newOffsets, 0, maxOffset + 1)
				Array.Copy(argumentNumbers, 0, newArgumentNumbers, 0, maxOffset + 1)
				formats = newFormats
				offsets = newOffsets
				argumentNumbers = newArgumentNumbers
			End If
			Dim oldMaxOffset As Integer = maxOffset
			maxOffset = offsetNumber
			offsets(offsetNumber) = segments(SEG_RAW).length()
			argumentNumbers(offsetNumber) = argumentNumber

			' now get the format
			Dim newFormat As Format = Nothing
			If segments(SEG_TYPE).length() <> 0 Then
				Dim type As Integer = findKeyword(segments(SEG_TYPE), TYPE_KEYWORDS)
				Select Case type
				Case TYPE_NULL
					' Type "" is allowed. e.g., "{0,}", "{0,,}", and "{0,,#}"
					' are treated as "{0}".

				Case TYPE_NUMBER
					Select Case findKeyword(segments(SEG_MODIFIER), NUMBER_MODIFIER_KEYWORDS)
					Case MODIFIER_DEFAULT
						newFormat = NumberFormat.getInstance(locale)
					Case MODIFIER_CURRENCY
						newFormat = NumberFormat.getCurrencyInstance(locale)
					Case MODIFIER_PERCENT
						newFormat = NumberFormat.getPercentInstance(locale)
					Case MODIFIER_INTEGER
						newFormat = NumberFormat.getIntegerInstance(locale)
					Case Else ' DecimalFormat pattern
						Try
							newFormat = New java.text.DecimalFormat(segments(SEG_MODIFIER), DecimalFormatSymbols.getInstance(locale))
						Catch e As IllegalArgumentException
							maxOffset = oldMaxOffset
							Throw e
						End Try
					End Select

				Case TYPE_DATE, TYPE_TIME
					Dim [mod] As Integer = findKeyword(segments(SEG_MODIFIER), DATE_TIME_MODIFIER_KEYWORDS)
					If [mod] >= 0 AndAlso [mod] < DATE_TIME_MODIFIER_KEYWORDS.Length Then
						If type = TYPE_DATE Then
							newFormat = DateFormat.getDateInstance(DATE_TIME_MODIFIERS([mod]), locale)
						Else
							newFormat = DateFormat.getTimeInstance(DATE_TIME_MODIFIERS([mod]), locale)
						End If
					Else
						' SimpleDateFormat pattern
						Try
							newFormat = New SimpleDateFormat(segments(SEG_MODIFIER), locale)
						Catch e As IllegalArgumentException
							maxOffset = oldMaxOffset
							Throw e
						End Try
					End If

				Case TYPE_CHOICE
					Try
						' ChoiceFormat pattern
						newFormat = New ChoiceFormat(segments(SEG_MODIFIER))
					Catch e As Exception
						maxOffset = oldMaxOffset
						Throw New IllegalArgumentException("Choice Pattern incorrect: " & segments(SEG_MODIFIER), e)
					End Try

				Case Else
					maxOffset = oldMaxOffset
					Throw New IllegalArgumentException("unknown format type: " & segments(SEG_TYPE))
				End Select
			End If
			formats(offsetNumber) = newFormat
		End Sub

		Private Shared Function findKeyword(  s As String,   list As String()) As Integer
			For i As Integer = 0 To list.Length - 1
				If s.Equals(list(i)) Then Return i
			Next i

			' Try trimmed lowercase.
			Dim ls As String = s.Trim().ToLower(java.util.Locale.ROOT)
			If ls <> s Then
				For i As Integer = 0 To list.Length - 1
					If ls.Equals(list(i)) Then Return i
				Next i
			End If
			Return -1
		End Function

		Private Shared Sub copyAndFixQuotes(  source As String,   start As Integer,   [end] As Integer,   target As StringBuilder)
			Dim quoted As Boolean = False

			For i As Integer = start To [end] - 1
				Dim ch As Char = source.Chars(i)
				If ch = "{"c Then
					If Not quoted Then
						target.append("'"c)
						quoted = True
					End If
					target.append(ch)
				ElseIf ch = "'"c Then
					target.append("''")
				Else
					If quoted Then
						target.append("'"c)
						quoted = False
					End If
					target.append(ch)
				End If
			Next i
			If quoted Then target.append("'"c)
		End Sub

		''' <summary>
		''' After reading an object from the input stream, do a simple verification
		''' to maintain class invariants. </summary>
		''' <exception cref="InvalidObjectException"> if the objects read from the stream is invalid. </exception>
		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			Dim isValid As Boolean = maxOffset >= -1 AndAlso formats.Length > maxOffset AndAlso offsets.Length > maxOffset AndAlso argumentNumbers.Length > maxOffset
			If isValid Then
				Dim lastOffset As Integer = pattern.length() + 1
				For i As Integer = maxOffset To 0 Step -1
					If (offsets(i) < 0) OrElse (offsets(i) > lastOffset) Then
						isValid = False
						Exit For
					Else
						lastOffset = offsets(i)
					End If
				Next i
			End If
			If Not isValid Then Throw New java.io.InvalidObjectException("Could not reconstruct MessageFormat from corrupt stream.")
		End Sub
	End Class

End Namespace