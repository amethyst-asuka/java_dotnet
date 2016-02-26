Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.text

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf

	''' <summary>
	''' Takes a sequence of RTF tokens and text and appends the text
	''' described by the RTF to a <code>StyledDocument</code> (the <em>target</em>).
	''' The RTF is lexed
	''' from the character stream by the <code>RTFParser</code> which is this class's
	''' superclass.
	''' 
	''' This class is an indirect subclass of OutputStream. It must be closed
	''' in order to guarantee that all of the text has been sent to
	''' the text acceptor.
	''' </summary>
	'''   <seealso cref= RTFParser </seealso>
	'''   <seealso cref= java.io.OutputStream </seealso>
	Friend Class RTFReader
		Inherits RTFParser

	  ''' <summary>
	  ''' The object to which the parsed text is sent. </summary>
	  Friend target As StyledDocument

	  ''' <summary>
	  ''' Miscellaneous information about the parser's state. This
	  '''  dictionary is saved and restored when an RTF group begins
	  '''  or ends. 
	  ''' </summary>
	  Friend parserState As Dictionary(Of Object, Object) ' Current parser state
	  ''' <summary>
	  ''' This is the "dst" item from parserState. rtfDestination
	  '''  is the current rtf destination. It is cached in an instance
	  '''  variable for speed. 
	  ''' </summary>
	  Friend rtfDestination As Destination
	  ''' <summary>
	  ''' This holds the current document attributes. </summary>
	  Friend documentAttributes As MutableAttributeSet

	  ''' <summary>
	  ''' This Dictionary maps Integer font numbers to String font names. </summary>
	  Friend fontTable As Dictionary(Of Integer?, String)
	  ''' <summary>
	  ''' This array maps color indices to Color objects. </summary>
	  Friend colorTable As java.awt.Color()
	  ''' <summary>
	  ''' This array maps character style numbers to Style objects. </summary>
	  Friend characterStyles As Style()
	  ''' <summary>
	  ''' This array maps paragraph style numbers to Style objects. </summary>
	  Friend paragraphStyles As Style()
	  ''' <summary>
	  ''' This array maps section style numbers to Style objects. </summary>
	  Friend sectionStyles As Style()

	  ''' <summary>
	  ''' This is the RTF version number, extracted from the \rtf keyword.
	  '''  The version information is currently not used. 
	  ''' </summary>
	  Friend rtfversion As Integer

	  ''' <summary>
	  ''' <code>true</code> to indicate that if the next keyword is unknown,
	  '''  the containing group should be ignored. 
	  ''' </summary>
	  Friend ignoreGroupIfUnknownKeyword As Boolean

	  ''' <summary>
	  ''' The parameter of the most recently parsed \\ucN keyword,
	  '''  used for skipping alternative representations after a
	  '''  Unicode character. 
	  ''' </summary>
	  Friend skippingCharacters As Integer

	  Private Shared straightforwardAttributes As Dictionary(Of String, RTFAttribute)
	  Shared Sub New()
		  straightforwardAttributes = RTFAttributes.attributesByKeyword()
		  textKeywords = New Dictionary(Of String, String)
		  textKeywords.put("\", "\")
		  textKeywords.put("{", "{")
		  textKeywords.put("}", "}")
		  textKeywords.put(" ", ChrW(&H00A0).ToString()) ' not in the spec...
		  textKeywords.put("~", ChrW(&H00A0).ToString()) ' nonbreaking space
		  textKeywords.put("_", ChrW(&H2011).ToString()) ' nonbreaking hyphen
		  textKeywords.put("bullet", ChrW(&H2022).ToString())
		  textKeywords.put("emdash", ChrW(&H2014).ToString())
		  textKeywords.put("emspace", ChrW(&H2003).ToString())
		  textKeywords.put("endash", ChrW(&H2013).ToString())
		  textKeywords.put("enspace", ChrW(&H2002).ToString())
		  textKeywords.put("ldblquote", ChrW(&H201C).ToString())
		  textKeywords.put("lquote", ChrW(&H2018).ToString())
		  textKeywords.put("ltrmark", ChrW(&H200E).ToString())
		  textKeywords.put("rdblquote", ChrW(&H201D).ToString())
		  textKeywords.put("rquote", ChrW(&H2019).ToString())
		  textKeywords.put("rtlmark", ChrW(&H200F).ToString())
		  textKeywords.put("tab", vbTab)
		  textKeywords.put("zwj", ChrW(&H200D).ToString())
		  textKeywords.put("zwnj", ChrW(&H200C).ToString())

	'       There is no Unicode equivalent to an optional hyphen, as far as
	'         I can tell. 
		  textKeywords.put("-", ChrW(&H2027).ToString()) ' TODO: optional hyphen
		  characterSets = New Dictionary(Of String, Char())
	  End Sub

	  Private mockery As MockAttributeSet

	  ' this should be final, but there's a bug in javac... 
	  ''' <summary>
	  ''' textKeywords maps RTF keywords to single-character strings,
	  '''  for those keywords which simply insert some text. 
	  ''' </summary>
	  Friend Shared textKeywords As Dictionary(Of String, String) = Nothing

	  ' some entries in parserState 
	  Friend Const TabAlignmentKey As String = "tab_alignment"
	  Friend Const TabLeaderKey As String = "tab_leader"

	  Friend Shared characterSets As Dictionary(Of String, Char())
	  Friend Shared useNeXTForAnsi As Boolean = False

	' TODO: per-font font encodings ( \fcharset control word ) ? 

	''' <summary>
	''' Creates a new RTFReader instance. Text will be sent to
	''' the specified TextAcceptor.
	''' </summary>
	''' <param name="destination"> The TextAcceptor which is to receive the text. </param>
	Public Sub New(ByVal destination As StyledDocument)
		Dim i As Integer

		target = destination
		parserState = New Dictionary(Of Object, Object)
		fontTable = New Dictionary(Of Integer?, String)

		rtfversion = -1

		mockery = New MockAttributeSet
		documentAttributes = New SimpleAttributeSet
	End Sub

	''' <summary>
	''' Called when the RTFParser encounters a bin keyword in the
	'''  RTF stream.
	''' </summary>
	'''  <seealso cref= RTFParser </seealso>
	Public Overrides Sub handleBinaryBlob(ByVal data As SByte())
		If skippingCharacters > 0 Then
			' a blob only counts as one character for skipping purposes 
			skippingCharacters -= 1
			Return
		End If

		' someday, someone will want to do something with blobs 
	End Sub


	''' <summary>
	''' Handles any pure text (containing no control characters) in the input
	''' stream. Called by the superclass. 
	''' </summary>
	Public Overrides Sub handleText(ByVal text As String)
		If skippingCharacters > 0 Then
			If skippingCharacters >= text.Length Then
				skippingCharacters -= text.Length
				Return
			Else
				text = text.Substring(skippingCharacters)
				skippingCharacters = 0
			End If
		End If

		If rtfDestination IsNot Nothing Then
			rtfDestination.handleText(text)
			Return
		End If

		warning("Text with no destination. oops.")
	End Sub

	''' <summary>
	''' The default color for text which has no specified color. </summary>
	Friend Overridable Function defaultColor() As java.awt.Color
		Return java.awt.Color.black
	End Function

	''' <summary>
	''' Called by the superclass when a new RTF group is begun.
	'''  This implementation saves the current <code>parserState</code>, and gives
	'''  the current destination a chance to save its own state. </summary>
	''' <seealso cref= RTFParser#begingroup </seealso>
	Public Overrides Sub begingroup()
		If skippingCharacters > 0 Then skippingCharacters = 0

	'     we do this little dance to avoid cloning the entire state stack and
	'       immediately throwing it away. 
		Dim oldSaveState As Object = parserState.get("_savedState")
		If oldSaveState IsNot Nothing Then parserState.remove("_savedState")
		Dim saveState As Dictionary(Of String, Object) = CType(CType(parserState, Hashtable).clone(), Dictionary(Of String, Object))
		If oldSaveState IsNot Nothing Then saveState.put("_savedState", oldSaveState)
		parserState.put("_savedState", saveState)

		If rtfDestination IsNot Nothing Then rtfDestination.begingroup()
	End Sub

	''' <summary>
	''' Called by the superclass when the current RTF group is closed.
	'''  This restores the parserState saved by <code>begingroup()</code>
	'''  as well as invoking the endgroup method of the current
	'''  destination. </summary>
	''' <seealso cref= RTFParser#endgroup </seealso>
	Public Overrides Sub endgroup()
		If skippingCharacters > 0 Then skippingCharacters = 0

		Dim restoredState As Dictionary(Of Object, Object) = CType(parserState.get("_savedState"), Dictionary(Of Object, Object))
		Dim restoredDestination As Destination = CType(restoredState.get("dst"), Destination)
		If restoredDestination IsNot rtfDestination Then
			rtfDestination.close() ' allow the destination to clean up
			rtfDestination = restoredDestination
		End If
		Dim oldParserState As Dictionary = parserState
		parserState = restoredState
		If rtfDestination IsNot Nothing Then rtfDestination.endgroup(oldParserState)
	End Sub

	Protected Friend Overridable Property rTFDestination As Destination
		Set(ByVal newDestination As Destination)
		'     Check that setting the destination won't close the
		'       current destination (should never happen) 
			Dim previousState As Dictionary = CType(parserState.get("_savedState"), Dictionary)
			If previousState IsNot Nothing Then
				If rtfDestination IsNot previousState.get("dst") Then
					warning("Warning, RTF destination overridden, invalid RTF.")
					rtfDestination.close()
				End If
			End If
			rtfDestination = newDestination
			parserState.put("dst", rtfDestination)
		End Set
	End Property

	''' <summary>
	''' Called by the user when there is no more input (<i>i.e.</i>,
	''' at the end of the RTF file.)
	''' </summary>
	''' <seealso cref= OutputStream#close </seealso>
	Public Overrides Sub close()
		Dim docProps As System.Collections.IEnumerator = documentAttributes.attributeNames
		Do While docProps.hasMoreElements()
			Dim propName As Object = docProps.nextElement()
			target.putProperty(propName, documentAttributes.getAttribute(propName))
		Loop

		' RTFParser should have ensured that all our groups are closed 

		warning("RTF filter done.")

		MyBase.close()
	End Sub

	''' <summary>
	''' Handles a parameterless RTF keyword. This is called by the superclass
	''' (RTFParser) when a keyword is found in the input stream.
	''' 
	''' @returns <code>true</code> if the keyword is recognized and handled;
	'''          <code>false</code> otherwise </summary>
	''' <seealso cref= RTFParser#handleKeyword </seealso>
	Public Overrides Function handleKeyword(ByVal keyword As String) As Boolean
		Dim item As String
		Dim ignoreGroupIfUnknownKeywordSave As Boolean = ignoreGroupIfUnknownKeyword

		If skippingCharacters > 0 Then
			skippingCharacters -= 1
			Return True
		End If

		ignoreGroupIfUnknownKeyword = False

		item = textKeywords.get(keyword)
		If item IsNot Nothing Then
			handleText(item)
			Return True
		End If

		If keyword.Equals("fonttbl") Then
			rTFDestination = New FonttblDestination(Me)
			Return True
		End If

		If keyword.Equals("colortbl") Then
			rTFDestination = New ColortblDestination(Me)
			Return True
		End If

		If keyword.Equals("stylesheet") Then
			rTFDestination = New StylesheetDestination(Me)
			Return True
		End If

		If keyword.Equals("info") Then
			rTFDestination = New InfoDestination(Me)
			Return False
		End If

		If keyword.Equals("mac") Then
			characterSet = "mac"
			Return True
		End If

		If keyword.Equals("ansi") Then
			If useNeXTForAnsi Then
				characterSet = "NeXT"
			Else
				characterSet = "ansi"
			End If
			Return True
		End If

		If keyword.Equals("next") Then
			characterSet = "NeXT"
			Return True
		End If

		If keyword.Equals("pc") Then
			characterSet = "cpg437" ' IBM Code Page 437
			Return True
		End If

		If keyword.Equals("pca") Then
			characterSet = "cpg850" ' IBM Code Page 850
			Return True
		End If

		If keyword.Equals("*") Then
			ignoreGroupIfUnknownKeyword = True
			Return True
		End If

		If rtfDestination IsNot Nothing Then
			If rtfDestination.handleKeyword(keyword) Then Return True
		End If

		' this point is reached only if the keyword is unrecognized 

		' other destinations we don't understand and therefore ignore 
		If keyword.Equals("aftncn") OrElse keyword.Equals("aftnsep") OrElse keyword.Equals("aftnsepc") OrElse keyword.Equals("annotation") OrElse keyword.Equals("atnauthor") OrElse keyword.Equals("atnicn") OrElse keyword.Equals("atnid") OrElse keyword.Equals("atnref") OrElse keyword.Equals("atntime") OrElse keyword.Equals("atrfend") OrElse keyword.Equals("atrfstart") OrElse keyword.Equals("bkmkend") OrElse keyword.Equals("bkmkstart") OrElse keyword.Equals("datafield") OrElse keyword.Equals("do") OrElse keyword.Equals("dptxbxtext") OrElse keyword.Equals("falt") OrElse keyword.Equals("field") OrElse keyword.Equals("file") OrElse keyword.Equals("filetbl") OrElse keyword.Equals("fname") OrElse keyword.Equals("fontemb") OrElse keyword.Equals("fontfile") OrElse keyword.Equals("footer") OrElse keyword.Equals("footerf") OrElse keyword.Equals("footerl") OrElse keyword.Equals("footerr") OrElse keyword.Equals("footnote") OrElse keyword.Equals("ftncn") OrElse keyword.Equals("ftnsep") OrElse keyword.Equals("ftnsepc") OrElse keyword.Equals("header") OrElse keyword.Equals("headerf") OrElse keyword.Equals("headerl") OrElse keyword.Equals("headerr") OrElse keyword.Equals("keycode") OrElse keyword.Equals("nextfile") OrElse keyword.Equals("object") OrElse keyword.Equals("pict") OrElse keyword.Equals("pn") OrElse keyword.Equals("pnseclvl") OrElse keyword.Equals("pntxtb") OrElse keyword.Equals("pntxta") OrElse keyword.Equals("revtbl") OrElse keyword.Equals("rxe") OrElse keyword.Equals("tc") OrElse keyword.Equals("template") OrElse keyword.Equals("txe") OrElse keyword.Equals("xe") Then ignoreGroupIfUnknownKeywordSave = True

		If ignoreGroupIfUnknownKeywordSave Then rTFDestination = New DiscardingDestination(Me)

		Return False
	End Function

	''' <summary>
	''' Handles an RTF keyword and its integer parameter.
	''' This is called by the superclass
	''' (RTFParser) when a keyword is found in the input stream.
	''' 
	''' @returns <code>true</code> if the keyword is recognized and handled;
	'''          <code>false</code> otherwise </summary>
	''' <seealso cref= RTFParser#handleKeyword </seealso>
	Public Overrides Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
		Dim ignoreGroupIfUnknownKeywordSave As Boolean = ignoreGroupIfUnknownKeyword

		If skippingCharacters > 0 Then
			skippingCharacters -= 1
			Return True
		End If

		ignoreGroupIfUnknownKeyword = False

		If keyword.Equals("uc") Then
			' count of characters to skip after a unicode character 
			parserState.put("UnicodeSkip", Convert.ToInt32(parameter))
			Return True
		End If
		If keyword.Equals("u") Then
			If parameter < 0 Then parameter = parameter + 65536
			handleText(ChrW(parameter))
			Dim skip As Number = CType(parserState.get("UnicodeSkip"), Number)
			If skip IsNot Nothing Then
				skippingCharacters = skip
			Else
				skippingCharacters = 1
			End If
			Return True
		End If

		If keyword.Equals("rtf") Then
			rtfversion = parameter
			rTFDestination = New DocumentDestination(Me)
			Return True
		End If

		If keyword.StartsWith("NeXT") OrElse keyword.Equals("private") Then ignoreGroupIfUnknownKeywordSave = True

		If rtfDestination IsNot Nothing Then
			If rtfDestination.handleKeyword(keyword, parameter) Then Return True
		End If

		' this point is reached only if the keyword is unrecognized 

		If ignoreGroupIfUnknownKeywordSave Then rTFDestination = New DiscardingDestination(Me)

		Return False
	End Function

	Private Sub setTargetAttribute(ByVal name As String, ByVal value As Object)
	'    target.changeAttributes(new LFDictionary(LFArray.arrayWithObject(value), LFArray.arrayWithObject(name)));
	End Sub

	''' <summary>
	''' setCharacterSet sets the current translation table to correspond with
	''' the named character set. The character set is loaded if necessary.
	''' </summary>
	''' <seealso cref= AbstractFilter </seealso>
	Public Overridable Property characterSet As String
		Set(ByVal name As String)
			Dim [set] As Object
    
			Try
				[set] = getCharacterSet(name)
			Catch e As Exception
				warning("Exception loading RTF character set """ & name & """: " & e)
				[set] = Nothing
			End Try
    
			If [set] IsNot Nothing Then
				translationTable = CType([set], Char())
			Else
				warning("Unknown RTF character set """ & name & """")
				If Not name.Equals("ansi") Then
					Try
						translationTable = CType(getCharacterSet("ansi"), Char())
					Catch e As IOException
						Throw New InternalError("RTFReader: Unable to find character set resources (" & e & ")", e)
					End Try
				End If
			End If
    
			targetAttributeute(Constants.RTFCharacterSet, name)
		End Set
	End Property

	''' <summary>
	''' Adds a character set to the RTFReader's list
	'''  of known character sets 
	''' </summary>
	Public Shared Sub defineCharacterSet(ByVal name As String, ByVal table As Char())
		If table.Length < 256 Then Throw New System.ArgumentException("Translation table must have 256 entries.")
		characterSets.put(name, table)
	End Sub

	''' <summary>
	''' Looks up a named character set. A character set is a 256-entry
	'''  array of characters, mapping unsigned byte values to their Unicode
	'''  equivalents. The character set is loaded if necessary.
	''' 
	'''  @returns the character set
	''' </summary>
	Public Shared Function getCharacterSet(ByVal name As String) As Object
		Dim [set] As Char() = characterSets.get(name)
		If [set] Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			InputStream charsetStream = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<InputStream>()
	'		{
	'					public InputStream run()
	'					{
	'						Return RTFReader.class.getResourceAsStream("charsets/" + name + ".txt");
	'					}
	'				});
			[set] = readCharset(charsetStream)
			defineCharacterSet(name, [set])
		End If
		Return [set]
	End Function

	''' <summary>
	''' Parses a character set from an InputStream. The character set
	''' must contain 256 decimal integers, separated by whitespace, with
	''' no punctuation. B- and C- style comments are allowed.
	''' 
	''' @returns the newly read character set
	''' </summary>
	Friend Shared Function readCharset(ByVal strm As InputStream) As Char()
		Dim values As Char() = New Char(255){}
		Dim i As Integer
		Dim [in] As New StreamTokenizer(New BufferedReader(New InputStreamReader(strm, "ISO-8859-1")))

		[in].eolIsSignificant(False)
		[in].commentChar("#"c)
		[in].slashSlashComments(True)
		[in].slashStarComments(True)

		i = 0
		Do While i < 256
			Dim ttype As Integer
			Try
				ttype = [in].nextToken()
			Catch e As Exception
				Throw New IOException("Unable to read from character set file (" & e & ")")
			End Try
			If ttype <> [in].TT_NUMBER Then Throw New IOException("Unexpected token in character set file")
			values(i) = CChar([in].nval)
			i += 1
		Loop

		Return values
	End Function

	Friend Shared Function readCharset(ByVal href As java.net.URL) As Char()
		Return readCharset(href.openStream())
	End Function

	''' <summary>
	''' An interface (could be an entirely abstract class) describing
	'''  a destination. The RTF reader always has a current destination
	'''  which is where text is sent.
	''' </summary>
	'''  <seealso cref= RTFReader </seealso>
	Friend Interface Destination
		Sub handleBinaryBlob(ByVal data As SByte())
		Sub handleText(ByVal text As String)
		Function handleKeyword(ByVal keyword As String) As Boolean
		Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean

		Sub begingroup()
		Sub endgroup(ByVal oldState As Dictionary)

		Sub close()
	End Interface

	''' <summary>
	''' This data-sink class is used to implement ignored destinations
	'''  (e.g. {\*\blegga blah blah blah} )
	'''  It accepts all keywords and text but does nothing with them. 
	''' </summary>
	Friend Class DiscardingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

			Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			End Sub

		Public Overridable Sub handleBinaryBlob(ByVal data As SByte())
			' Discard binary blobs. 
		End Sub

		Public Overridable Sub handleText(ByVal text As String)
			' Discard text. 
		End Sub

		Public Overridable Function handleKeyword(ByVal text As String) As Boolean
			' Accept and discard keywords. 
			Return True
		End Function

		Public Overridable Function handleKeyword(ByVal text As String, ByVal parameter As Integer) As Boolean
			' Accept and discard parameterized keywords. 
			Return True
		End Function

		Public Overridable Sub begingroup()
	'         Ignore groups --- the RTFReader will keep track of the
	'           current group level as necessary 
		End Sub

		Public Overridable Sub endgroup(ByVal oldState As Dictionary)
			' Ignore groups 
		End Sub

		Public Overridable Sub close()
			' No end-of-destination cleanup needed 
		End Sub
	End Class

	''' <summary>
	''' Reads the fonttbl group, inserting fonts into the RTFReader's
	'''  fontTable dictionary. 
	''' </summary>
	Friend Class FonttblDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

			Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			End Sub

		Friend nextFontNumber As Integer
		Friend fontNumberKey As Integer? = Nothing
		Friend nextFontFamily As String

		Public Overridable Sub handleBinaryBlob(ByVal data As SByte())
		End Sub

		Public Overridable Sub handleText(ByVal text As String)
			Dim semicolon As Integer = text.IndexOf(";"c)
			Dim fontName As String

			If semicolon > -1 Then
				fontName = text.Substring(0, semicolon)
			Else
				fontName = text
			End If


			' TODO: do something with the font family. 

			If nextFontNumber = -1 AndAlso fontNumberKey IsNot Nothing Then
				'font name might be broken across multiple calls
				fontName = outerInstance.fontTable.get(fontNumberKey) + fontName
			Else
				fontNumberKey = Convert.ToInt32(nextFontNumber)
			End If
			outerInstance.fontTable.put(fontNumberKey, fontName)

			nextFontNumber = -1
			nextFontFamily = Nothing
		End Sub

		Public Overridable Function handleKeyword(ByVal keyword As String) As Boolean
			If keyword.Chars(0) = "f"c Then
				nextFontFamily = keyword.Substring(1)
				Return True
			End If

			Return False
		End Function

		Public Overridable Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
			If keyword.Equals("f") Then
				nextFontNumber = parameter
				Return True
			End If

			Return False
		End Function

		' Groups are irrelevant. 
		Public Overridable Sub begingroup()
		End Sub
		Public Overridable Sub endgroup(ByVal oldState As Dictionary)
		End Sub

	'     currently, the only thing we do when the font table ends is
	'       dump its contents to the debugging log. 
		Public Overridable Sub close()
			Dim nums As System.Collections.IEnumerator(Of Integer?) = outerInstance.fontTable.keys()
			outerInstance.warning("Done reading font table.")
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While nums.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim num As Integer? = nums.nextElement()
				outerInstance.warning("Number " & num & ": " & outerInstance.fontTable.get(num))
			Loop
		End Sub
	End Class

	''' <summary>
	''' Reads the colortbl group. Upon end-of-group, the RTFReader's
	'''  color table is set to an array containing the read colors. 
	''' </summary>
	Friend Class ColortblDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

		Friend red, green, blue As Integer
		Friend proTemTable As List(Of java.awt.Color)

		Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			red = 0
			green = 0
			blue = 0
			proTemTable = New List(Of java.awt.Color)
		End Sub

		Public Overridable Sub handleText(ByVal text As String)
			Dim index As Integer

			For index = 0 To text.Length - 1
				If text.Chars(index) = ";"c Then
					Dim newColor As java.awt.Color
					newColor = New java.awt.Color(red, green, blue)
					proTemTable.Add(newColor)
				End If
			Next index
		End Sub

		Public Overridable Sub close()
			Dim count As Integer = proTemTable.Count
			outerInstance.warning("Done reading color table, " & count & " entries.")
			outerInstance.colorTable = New java.awt.Color(count - 1){}
			proTemTable.CopyTo(outerInstance.colorTable)
		End Sub

		Public Overridable Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
			If keyword.Equals("red") Then
				red = parameter
			ElseIf keyword.Equals("green") Then
				green = parameter
			ElseIf keyword.Equals("blue") Then
				blue = parameter
			Else
				Return False
			End If

			Return True
		End Function

		' Colortbls don't understand any parameterless keywords 
		Public Overridable Function handleKeyword(ByVal keyword As String) As Boolean
			Return False
		End Function

		' Groups are irrelevant. 
		Public Overridable Sub begingroup()
		End Sub
		Public Overridable Sub endgroup(ByVal oldState As Dictionary)
		End Sub

		' Shouldn't see any binary blobs ... 
		Public Overridable Sub handleBinaryBlob(ByVal data As SByte())
		End Sub
	End Class

	''' <summary>
	''' Handles the stylesheet keyword. Styles are read and sorted
	'''  into the three style arrays in the RTFReader. 
	''' </summary>
	Friend Class StylesheetDestination
		Inherits DiscardingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

		Friend definedStyles As Dictionary(Of Integer?, StyleDefiningDestination)

		Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			definedStyles = New Dictionary(Of Integer?, StyleDefiningDestination)
		End Sub

		Public Overrides Sub begingroup()
			outerInstance.rTFDestination = New StyleDefiningDestination(Me)
		End Sub

		Public Overrides Sub close()
			Dim chrStyles As New List(Of Style)
			Dim pgfStyles As New List(Of Style)
			Dim secStyles As New List(Of Style)
			Dim styles As System.Collections.IEnumerator(Of StyleDefiningDestination) = definedStyles.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While styles.hasMoreElements()
				Dim style As StyleDefiningDestination
				Dim defined As Style
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				style = styles.nextElement()
				defined = style.realize()
				outerInstance.warning("Style " & style.number & " (" & style.styleName & "): " & defined)
				Dim stype As String = CStr(defined.getAttribute(Constants.StyleType))
				Dim toSet As List(Of Style)
				If stype.Equals(Constants.STSection) Then
					toSet = secStyles
				ElseIf stype.Equals(Constants.STCharacter) Then
					toSet = chrStyles
				Else
					toSet = pgfStyles
				End If
				If toSet.Count <= style.number Then toSet.Capacity = style.number + 1
				toSet(style.number) = defined
			Loop
			If Not(chrStyles.Count = 0) Then
				Dim styleArray As Style() = New Style(chrStyles.Count - 1){}
				chrStyles.CopyTo(styleArray)
				outerInstance.characterStyles = styleArray
			End If
			If Not(pgfStyles.Count = 0) Then
				Dim styleArray As Style() = New Style(pgfStyles.Count - 1){}
				pgfStyles.CopyTo(styleArray)
				outerInstance.paragraphStyles = styleArray
			End If
			If Not(secStyles.Count = 0) Then
				Dim styleArray As Style() = New Style(secStyles.Count - 1){}
				secStyles.CopyTo(styleArray)
				outerInstance.sectionStyles = styleArray
			End If

	' (old debugging code)
	'        int i, m;
	'        if (characterStyles != null) {
	'          m = characterStyles.length;
	'          for(i=0;i<m;i++)
	'            warnings.println("chrStyle["+i+"]="+characterStyles[i]);
	'        } else warnings.println("No character styles.");
	'        if (paragraphStyles != null) {
	'          m = paragraphStyles.length;
	'          for(i=0;i<m;i++)
	'            warnings.println("pgfStyle["+i+"]="+paragraphStyles[i]);
	'        } else warnings.println("No paragraph styles.");
	'        if (sectionStyles != null) {
	'          m = characterStyles.length;
	'          for(i=0;i<m;i++)
	'            warnings.println("secStyle["+i+"]="+sectionStyles[i]);
	'        } else warnings.println("No section styles.");
	'
		End Sub

		''' <summary>
		''' This subclass handles an individual style </summary>
		Friend Class StyleDefiningDestination
			Inherits AttributeTrackingDestination
			Implements Destination

			Private ReadOnly outerInstance As RTFReader.StylesheetDestination

			Friend ReadOnly STYLENUMBER_NONE As Integer = 222
			Friend additive As Boolean
			Friend characterStyle As Boolean
			Friend sectionStyle As Boolean
			Public styleName As String
			Public number As Integer
			Friend basedOn As Integer
			Friend nextStyle As Integer
			Friend hidden As Boolean

			Friend realizedStyle As Style

			Public Sub New(ByVal outerInstance As RTFReader.StylesheetDestination)
					Me.outerInstance = outerInstance
				additive = False
				characterStyle = False
				sectionStyle = False
				styleName = Nothing
				number = 0
				basedOn = STYLENUMBER_NONE
				nextStyle = STYLENUMBER_NONE
				hidden = False
			End Sub

			Public Overrides Sub handleText(ByVal text As String)
				If styleName IsNot Nothing Then
					styleName = styleName + text
				Else
					styleName = text
				End If
			End Sub

			Public Overrides Sub close()
				Dim semicolon As Integer = If(styleName Is Nothing, 0, styleName.IndexOf(";"c))
				If semicolon > 0 Then styleName = styleName.Substring(0, semicolon)
				outerInstance.definedStyles.put(Convert.ToInt32(number), Me)
				MyBase.close()
			End Sub

			Public Overrides Function handleKeyword(ByVal keyword As String) As Boolean
				If keyword.Equals("additive") Then
					additive = True
					Return True
				End If
				If keyword.Equals("shidden") Then
					hidden = True
					Return True
				End If
				Return MyBase.handleKeyword(keyword)
			End Function

			Public Overrides Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
				If keyword.Equals("s") Then
					characterStyle = False
					sectionStyle = False
					number = parameter
				ElseIf keyword.Equals("cs") Then
					characterStyle = True
					sectionStyle = False
					number = parameter
				ElseIf keyword.Equals("ds") Then
					characterStyle = False
					sectionStyle = True
					number = parameter
				ElseIf keyword.Equals("sbasedon") Then
					basedOn = parameter
				ElseIf keyword.Equals("snext") Then
					nextStyle = parameter
				Else
					Return MyBase.handleKeyword(keyword, parameter)
				End If
				Return True
			End Function

			Public Overridable Function realize() As Style
				Dim basis As Style = Nothing
				Dim [next] As Style = Nothing

				If realizedStyle IsNot Nothing Then Return realizedStyle

				If basedOn <> STYLENUMBER_NONE Then
					Dim styleDest As StyleDefiningDestination
					styleDest = outerInstance.definedStyles.get(Convert.ToInt32(basedOn))
					If styleDest IsNot Nothing AndAlso styleDest IsNot Me Then basis = styleDest.realize()
				End If

	'             NB: Swing StyleContext doesn't allow distinct styles with
	'               the same name; RTF apparently does. This may confuse the
	'               user. 
				realizedStyle = target.addStyle(styleName, basis)

				If characterStyle Then
					realizedStyle.addAttributes(currentTextAttributes())
					realizedStyle.addAttribute(Constants.StyleType, Constants.STCharacter)
				ElseIf sectionStyle Then
					realizedStyle.addAttributes(currentSectionAttributes())
					realizedStyle.addAttribute(Constants.StyleType, Constants.STSection) ' must be a paragraph style
				Else
					realizedStyle.addAttributes(currentParagraphAttributes())
					realizedStyle.addAttribute(Constants.StyleType, Constants.STParagraph)
				End If

				If nextStyle <> STYLENUMBER_NONE Then
					Dim styleDest As StyleDefiningDestination
					styleDest = outerInstance.definedStyles.get(Convert.ToInt32(nextStyle))
					If styleDest IsNot Nothing Then [next] = styleDest.realize()
				End If

				If [next] IsNot Nothing Then realizedStyle.addAttribute(Constants.StyleNext, [next])
				realizedStyle.addAttribute(Constants.StyleAdditive, Convert.ToBoolean(additive))
				realizedStyle.addAttribute(Constants.StyleHidden, Convert.ToBoolean(hidden))

				Return realizedStyle
			End Function
		End Class
	End Class

	''' <summary>
	''' Handles the info group. Currently no info keywords are recognized
	'''  so this is a subclass of DiscardingDestination. 
	''' </summary>
	Friend Class InfoDestination
		Inherits DiscardingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

			Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			End Sub

	End Class

	''' <summary>
	''' RTFReader.TextHandlingDestination is an abstract RTF destination
	'''  which simply tracks the attributes specified by the RTF control words
	'''  in internal form and can produce acceptable AttributeSets for the
	'''  current character, paragraph, and section attributes. It is up
	'''  to the subclasses to determine what is done with the actual text. 
	''' </summary>
	Friend MustInherit Class AttributeTrackingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

		''' <summary>
		''' This is the "chr" element of parserState, cached for
		'''  more efficient use 
		''' </summary>
		Friend characterAttributes As MutableAttributeSet
		''' <summary>
		''' This is the "pgf" element of parserState, cached for
		'''  more efficient use 
		''' </summary>
		Friend paragraphAttributes As MutableAttributeSet
		''' <summary>
		''' This is the "sec" element of parserState, cached for
		'''  more efficient use 
		''' </summary>
		Friend sectionAttributes As MutableAttributeSet

		Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			characterAttributes = rootCharacterAttributes()
			outerInstance.parserState.put("chr", characterAttributes)
			paragraphAttributes = rootParagraphAttributes()
			outerInstance.parserState.put("pgf", paragraphAttributes)
			sectionAttributes = rootSectionAttributes()
			outerInstance.parserState.put("sec", sectionAttributes)
		End Sub

		Public MustOverride Sub handleText(ByVal text As String)

		Public Overridable Sub handleBinaryBlob(ByVal data As SByte())
	'         This should really be in TextHandlingDestination, but
	'         * since *nobody* does anything with binary blobs, this
	'         * is more convenient. 
			outerInstance.warning("Unexpected binary data in RTF file.")
		End Sub

		Public Overridable Sub begingroup()
			Dim characterParent As AttributeSet = currentTextAttributes()
			Dim paragraphParent As AttributeSet = currentParagraphAttributes()
			Dim sectionParent As AttributeSet = currentSectionAttributes()

	'         It would probably be more efficient to use the
	'         * resolver property of the attributes set for
	'         * implementing rtf groups,
	'         * but that's needed for styles. 

			' update the cached attribute dictionaries 
			characterAttributes = New SimpleAttributeSet
			characterAttributes.addAttributes(characterParent)
			outerInstance.parserState.put("chr", characterAttributes)

			paragraphAttributes = New SimpleAttributeSet
			paragraphAttributes.addAttributes(paragraphParent)
			outerInstance.parserState.put("pgf", paragraphAttributes)

			sectionAttributes = New SimpleAttributeSet
			sectionAttributes.addAttributes(sectionParent)
			outerInstance.parserState.put("sec", sectionAttributes)
		End Sub

		Public Overridable Sub endgroup(ByVal oldState As Dictionary)
			characterAttributes = CType(outerInstance.parserState.get("chr"), MutableAttributeSet)
			paragraphAttributes = CType(outerInstance.parserState.get("pgf"), MutableAttributeSet)
			sectionAttributes = CType(outerInstance.parserState.get("sec"), MutableAttributeSet)
		End Sub

		Public Overridable Sub close()
		End Sub

		Public Overridable Function handleKeyword(ByVal keyword As String) As Boolean
			If keyword.Equals("ulnone") Then Return handleKeyword("ul", 0)

				Dim attr As RTFAttribute = straightforwardAttributes.get(keyword)
				If attr IsNot Nothing Then
					Dim ok As Boolean

					Select Case attr.domain()
					  Case RTFAttribute.D_CHARACTER
						ok = attr.set(characterAttributes)
					  Case RTFAttribute.D_PARAGRAPH
						ok = attr.set(paragraphAttributes)
					  Case RTFAttribute.D_SECTION
						ok = attr.set(sectionAttributes)
					  Case RTFAttribute.D_META
						outerInstance.mockery.backing = outerInstance.parserState
						ok = attr.set(outerInstance.mockery)
						outerInstance.mockery.backing = Nothing
					  Case RTFAttribute.D_DOCUMENT
						ok = attr.set(outerInstance.documentAttributes)
					  Case Else
						' should never happen 
						ok = False
					End Select
					If ok Then Return True
				End If


			If keyword.Equals("plain") Then
				resetCharacterAttributes()
				Return True
			End If

			If keyword.Equals("pard") Then
				resetParagraphAttributes()
				Return True
			End If

			If keyword.Equals("sectd") Then
				resetSectionAttributes()
				Return True
			End If

			Return False
		End Function

		Public Overridable Function handleKeyword(ByVal keyword As String, ByVal parameter As Integer) As Boolean
			Dim booleanParameter As Boolean = (parameter <> 0)

			If keyword.Equals("fc") Then keyword = "cf" ' whatEVER, dude.

			If keyword.Equals("f") Then
				outerInstance.parserState.put(keyword, Convert.ToInt32(parameter))
				Return True
			End If
			If keyword.Equals("cf") Then
				outerInstance.parserState.put(keyword, Convert.ToInt32(parameter))
				Return True
			End If

				Dim attr As RTFAttribute = straightforwardAttributes.get(keyword)
				If attr IsNot Nothing Then
					Dim ok As Boolean

					Select Case attr.domain()
					  Case RTFAttribute.D_CHARACTER
						ok = attr.set(characterAttributes, parameter)
					  Case RTFAttribute.D_PARAGRAPH
						ok = attr.set(paragraphAttributes, parameter)
					  Case RTFAttribute.D_SECTION
						ok = attr.set(sectionAttributes, parameter)
					  Case RTFAttribute.D_META
						outerInstance.mockery.backing = outerInstance.parserState
						ok = attr.set(outerInstance.mockery, parameter)
						outerInstance.mockery.backing = Nothing
					  Case RTFAttribute.D_DOCUMENT
						ok = attr.set(outerInstance.documentAttributes, parameter)
					  Case Else
						' should never happen 
						ok = False
					End Select
					If ok Then Return True
				End If

			If keyword.Equals("fs") Then
				StyleConstants.fontSizeize(characterAttributes, (parameter \ 2))
				Return True
			End If

			' TODO: superscript/subscript 

			If keyword.Equals("sl") Then
				If parameter = 1000 Then ' magic value!
					characterAttributes.removeAttribute(StyleConstants.LineSpacing)
				Else
	'                 TODO: The RTF sl attribute has special meaning if it's
	'                   negative. Make sure that SwingText has the same special
	'                   meaning, or find a way to imitate that. When SwingText
	'                   handles this, also recognize the slmult keyword. 
					StyleConstants.lineSpacinging(characterAttributes, parameter / 20f)
				End If
				Return True
			End If

			' TODO: Other kinds of underlining 

			If keyword.Equals("tx") OrElse keyword.Equals("tb") Then
				Dim tabPosition As Single = parameter / 20f
				Dim tabAlignment, tabLeader As Integer
				Dim item As Number

				tabAlignment = TabStop.ALIGN_LEFT
				item = CType(outerInstance.parserState.get("tab_alignment"), Number)
				If item IsNot Nothing Then tabAlignment = item
				tabLeader = TabStop.LEAD_NONE
				item = CType(outerInstance.parserState.get("tab_leader"), Number)
				If item IsNot Nothing Then tabLeader = item
				If keyword.Equals("tb") Then tabAlignment = TabStop.ALIGN_BAR

				outerInstance.parserState.remove("tab_alignment")
				outerInstance.parserState.remove("tab_leader")

				Dim newStop As New TabStop(tabPosition, tabAlignment, tabLeader)
				Dim tabs As Dictionary(Of Object, Object)
				Dim stopCount As Integer?

				tabs = CType(outerInstance.parserState.get("_tabs"), Dictionary(Of Object, Object))
				If tabs Is Nothing Then
					tabs = New Dictionary(Of Object, Object)
					outerInstance.parserState.put("_tabs", tabs)
					stopCount = Convert.ToInt32(1)
				Else
					stopCount = CInt(Fix(tabs.get("stop count")))
					stopCount = Convert.ToInt32(1 + stopCount)
				End If
				tabs.put(stopCount, newStop)
				tabs.put("stop count", stopCount)
				outerInstance.parserState.remove("_tabs_immutable")

				Return True
			End If

			If keyword.Equals("s") AndAlso outerInstance.paragraphStyles IsNot Nothing Then
				outerInstance.parserState.put("paragraphStyle", outerInstance.paragraphStyles(parameter))
				Return True
			End If

			If keyword.Equals("cs") AndAlso outerInstance.characterStyles IsNot Nothing Then
				outerInstance.parserState.put("characterStyle", outerInstance.characterStyles(parameter))
				Return True
			End If

			If keyword.Equals("ds") AndAlso outerInstance.sectionStyles IsNot Nothing Then
				outerInstance.parserState.put("sectionStyle", outerInstance.sectionStyles(parameter))
				Return True
			End If

			Return False
		End Function

		''' <summary>
		''' Returns a new MutableAttributeSet containing the
		'''  default character attributes 
		''' </summary>
		Protected Friend Overridable Function rootCharacterAttributes() As MutableAttributeSet
			Dim [set] As MutableAttributeSet = New SimpleAttributeSet

			' TODO: default font 

			StyleConstants.italiclic([set], False)
			StyleConstants.boldold([set], False)
			StyleConstants.underlineine([set], False)
			StyleConstants.foregroundund([set], outerInstance.defaultColor())

			Return [set]
		End Function

		''' <summary>
		''' Returns a new MutableAttributeSet containing the
		'''  default paragraph attributes 
		''' </summary>
		Protected Friend Overridable Function rootParagraphAttributes() As MutableAttributeSet
			Dim [set] As MutableAttributeSet = New SimpleAttributeSet

			StyleConstants.leftIndentent([set], 0f)
			StyleConstants.rightIndentent([set], 0f)
			StyleConstants.firstLineIndentent([set], 0f)

			' TODO: what should this be, really? 
			[set].setResolveParent(outerInstance.target.getStyle(StyleContext.DEFAULT_STYLE))

			Return [set]
		End Function

		''' <summary>
		''' Returns a new MutableAttributeSet containing the
		'''  default section attributes 
		''' </summary>
		Protected Friend Overridable Function rootSectionAttributes() As MutableAttributeSet
			Dim [set] As MutableAttributeSet = New SimpleAttributeSet

			Return [set]
		End Function

		''' <summary>
		''' Calculates the current text (character) attributes in a form suitable
		''' for SwingText from the current parser state.
		''' 
		''' @returns a new MutableAttributeSet containing the text attributes.
		''' </summary>
		Friend Overridable Function currentTextAttributes() As MutableAttributeSet
			Dim attributes As MutableAttributeSet = New SimpleAttributeSet(characterAttributes)
			Dim fontnum As Integer?
			Dim stateItem As Integer?

			' figure out the font name 
	'         TODO: catch exceptions for undefined attributes,
	'           bad font indices, etc.? (as it stands, it is the caller's
	'           job to clean up after corrupt RTF) 
			fontnum = CInt(Fix(outerInstance.parserState.get("f")))
			' note setFontFamily() can not handle a null font 
			Dim fontFamily As String
			If fontnum IsNot Nothing Then
				fontFamily = outerInstance.fontTable.get(fontnum)
			Else
				fontFamily = Nothing
			End If
			If fontFamily IsNot Nothing Then
				StyleConstants.fontFamilyily(attributes, fontFamily)
			Else
				attributes.removeAttribute(StyleConstants.FontFamily)
			End If

			If outerInstance.colorTable IsNot Nothing Then
				stateItem = CInt(Fix(outerInstance.parserState.get("cf")))
				If stateItem IsNot Nothing Then
					Dim fg As java.awt.Color = outerInstance.colorTable(stateItem)
					StyleConstants.foregroundund(attributes, fg)
				Else
					' AttributeSet dies if you set a value to null 
					attributes.removeAttribute(StyleConstants.Foreground)
				End If
			End If

			If outerInstance.colorTable IsNot Nothing Then
				stateItem = CInt(Fix(outerInstance.parserState.get("cb")))
				If stateItem IsNot Nothing Then
					Dim bg As java.awt.Color = outerInstance.colorTable(stateItem)
					attributes.addAttribute(StyleConstants.Background, bg)
				Else
					' AttributeSet dies if you set a value to null 
					attributes.removeAttribute(StyleConstants.Background)
				End If
			End If

			Dim characterStyle As Style = CType(outerInstance.parserState.get("characterStyle"), Style)
			If characterStyle IsNot Nothing Then attributes.resolveParent = characterStyle

			' Other attributes are maintained directly in "attributes" 

			Return attributes
		End Function

		''' <summary>
		''' Calculates the current paragraph attributes (with keys
		''' as given in StyleConstants) from the current parser state.
		''' 
		''' @returns a newly created MutableAttributeSet. </summary>
		''' <seealso cref= StyleConstants </seealso>
		Friend Overridable Function currentParagraphAttributes() As MutableAttributeSet
			' NB if there were a mutableCopy() method we should use it 
			Dim bld As MutableAttributeSet = New SimpleAttributeSet(paragraphAttributes)

			Dim stateItem As Integer?

			''' <summary>
			'''* Tab stops ** </summary>
			Dim tabs As TabStop()

			tabs = CType(outerInstance.parserState.get("_tabs_immutable"), TabStop())
			If tabs Is Nothing Then
				Dim workingTabs As Dictionary = CType(outerInstance.parserState.get("_tabs"), Dictionary)
				If workingTabs IsNot Nothing Then
					Dim count As Integer = CInt(Fix(workingTabs.get("stop count")))
					tabs = New TabStop(count - 1){}
					For ix As Integer = 1 To count
						tabs(ix-1) = CType(workingTabs.get(Convert.ToInt32(ix)), TabStop)
					Next ix
					outerInstance.parserState.put("_tabs_immutable", tabs)
				End If
			End If
			If tabs IsNot Nothing Then bld.addAttribute(Constants.Tabs, tabs)

			Dim paragraphStyle As Style = CType(outerInstance.parserState.get("paragraphStyle"), Style)
			If paragraphStyle IsNot Nothing Then bld.resolveParent = paragraphStyle

			Return bld
		End Function

		''' <summary>
		''' Calculates the current section attributes
		''' from the current parser state.
		''' 
		''' @returns a newly created MutableAttributeSet.
		''' </summary>
		Public Overridable Function currentSectionAttributes() As AttributeSet
			Dim attributes As MutableAttributeSet = New SimpleAttributeSet(sectionAttributes)

			Dim sectionStyle As Style = CType(outerInstance.parserState.get("sectionStyle"), Style)
			If sectionStyle IsNot Nothing Then attributes.resolveParent = sectionStyle

			Return attributes
		End Function

		''' <summary>
		''' Resets the filter's internal notion of the current character
		'''  attributes to their default values. Invoked to handle the
		'''  \plain keyword. 
		''' </summary>
		Protected Friend Overridable Sub resetCharacterAttributes()
			handleKeyword("f", 0)
			handleKeyword("cf", 0)

			handleKeyword("fs", 24) ' 12 pt.

			Dim attributes As System.Collections.IEnumerator(Of RTFAttribute) = straightforwardAttributes.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While attributes.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim attr As RTFAttribute = attributes.nextElement()
				If attr.domain() = RTFAttribute.D_CHARACTER Then attr.default = characterAttributes
			Loop

			handleKeyword("sl", 1000)

			outerInstance.parserState.remove("characterStyle")
		End Sub

		''' <summary>
		''' Resets the filter's internal notion of the current paragraph's
		'''  attributes to their default values. Invoked to handle the
		'''  \pard keyword. 
		''' </summary>
		Protected Friend Overridable Sub resetParagraphAttributes()
			outerInstance.parserState.remove("_tabs")
			outerInstance.parserState.remove("_tabs_immutable")
			outerInstance.parserState.remove("paragraphStyle")

			StyleConstants.alignmentent(paragraphAttributes, StyleConstants.ALIGN_LEFT)

			Dim attributes As System.Collections.IEnumerator(Of RTFAttribute) = straightforwardAttributes.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While attributes.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim attr As RTFAttribute = attributes.nextElement()
				If attr.domain() = RTFAttribute.D_PARAGRAPH Then attr.default = characterAttributes
			Loop
		End Sub

		''' <summary>
		''' Resets the filter's internal notion of the current section's
		'''  attributes to their default values. Invoked to handle the
		'''  \sectd keyword. 
		''' </summary>
		Protected Friend Overridable Sub resetSectionAttributes()
			Dim attributes As System.Collections.IEnumerator(Of RTFAttribute) = straightforwardAttributes.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While attributes.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim attr As RTFAttribute = attributes.nextElement()
				If attr.domain() = RTFAttribute.D_SECTION Then attr.default = characterAttributes
			Loop

			outerInstance.parserState.remove("sectionStyle")
		End Sub
	End Class

	''' <summary>
	''' RTFReader.TextHandlingDestination provides basic text handling
	'''  functionality. Subclasses must implement: <dl>
	'''  <dt>deliverText()<dd>to handle a run of text with the same
	'''                       attributes
	'''  <dt>finishParagraph()<dd>to end the current paragraph and
	'''                           set the paragraph's attributes
	'''  <dt>endSection()<dd>to end the current section
	'''  </dl>
	''' </summary>
	Friend MustInherit Class TextHandlingDestination
		Inherits AttributeTrackingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

		''' <summary>
		''' <code>true</code> if the reader has not just finished
		'''  a paragraph; false upon startup 
		''' </summary>
		Friend inParagraph As Boolean

		Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			MyBase.New()
			inParagraph = False
		End Sub

		Public Overrides Sub handleText(ByVal text As String)
			If Not inParagraph Then beginParagraph()

			deliverText(text, currentTextAttributes())
		End Sub

		Friend MustOverride Sub deliverText(ByVal text As String, ByVal characterAttributes As AttributeSet)

		Public Overrides Sub close()
			If inParagraph Then endParagraph()

			MyBase.close()
		End Sub

		Public Overrides Function handleKeyword(ByVal keyword As String) As Boolean
			If keyword.Equals(vbCr) OrElse keyword.Equals(vbLf) Then keyword = "par"

			If keyword.Equals("par") Then
	'          warnings.println("Ending paragraph.");
				endParagraph()
				Return True
			End If

			If keyword.Equals("sect") Then
	'          warnings.println("Ending section.");
				endSection()
				Return True
			End If

			Return MyBase.handleKeyword(keyword)
		End Function

		Protected Friend Overridable Sub beginParagraph()
			inParagraph = True
		End Sub

		Protected Friend Overridable Sub endParagraph()
			Dim pgfAttributes As AttributeSet = currentParagraphAttributes()
			Dim chrAttributes As AttributeSet = currentTextAttributes()
			finishParagraph(pgfAttributes, chrAttributes)
			inParagraph = False
		End Sub

		Friend MustOverride Sub finishParagraph(ByVal pgfA As AttributeSet, ByVal chrA As AttributeSet)

		Friend MustOverride Sub endSection()
	End Class

	''' <summary>
	''' RTFReader.DocumentDestination is a concrete subclass of
	'''  TextHandlingDestination which appends the text to the
	'''  StyledDocument given by the <code>target</code> ivar of the
	'''  containing RTFReader.
	''' </summary>
	Friend Class DocumentDestination
		Inherits TextHandlingDestination
		Implements Destination

			Private ReadOnly outerInstance As RTFReader

			Public Sub New(ByVal outerInstance As RTFReader)
				Me.outerInstance = outerInstance
			End Sub

		Public Overrides Sub deliverText(ByVal text As String, ByVal characterAttributes As AttributeSet)
			Try
				outerInstance.target.insertString(outerInstance.target.length, text, currentTextAttributes())
			Catch ble As BadLocationException
				' This shouldn't be able to happen, of course 
				' TODO is InternalError the correct error to throw? 
				Throw New InternalError(ble.Message, ble)
			End Try
		End Sub

		Public Overrides Sub finishParagraph(ByVal pgfAttributes As AttributeSet, ByVal chrAttributes As AttributeSet)
			Dim pgfEndPosition As Integer = outerInstance.target.length
			Try
				outerInstance.target.insertString(pgfEndPosition, vbLf, chrAttributes)
				outerInstance.target.paragraphAttributestes(pgfEndPosition, 1, pgfAttributes, True)
			Catch ble As BadLocationException
				' This shouldn't be able to happen, of course 
				' TODO is InternalError the correct error to throw? 
				Throw New InternalError(ble.Message, ble)
			End Try
		End Sub

		Public Overrides Sub endSection()
			' If we implemented sections, we'd end 'em here 
		End Sub
	End Class

	End Class

End Namespace