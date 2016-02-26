Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.text

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Generates an RTF output stream (java.io.OutputStream) from rich text
	''' (handed off through a series of LTTextAcceptor calls).  Can be used to
	''' generate RTF from any object which knows how to write to a text acceptor
	''' (e.g., LTAttributedText and LTRTFFilter).
	''' 
	''' <p>Note that this is a lossy conversion since RTF's model of
	''' text does not exactly correspond with LightText's.
	''' </summary>
	''' <seealso cref= LTAttributedText </seealso>
	''' <seealso cref= LTRTFFilter </seealso>
	''' <seealso cref= LTTextAcceptor </seealso>
	''' <seealso cref= java.io.OutputStream </seealso>

	Friend Class RTFGenerator
		Inherits Object

	'     These dictionaries map Colors, font names, or Style objects
	'       to Integers 
		Friend colorTable As Dictionary(Of Object, Integer?)
		Friend colorCount As Integer
		Friend fontTable As Dictionary(Of String, Integer?)
		Friend fontCount As Integer
		Friend styleTable As Dictionary(Of AttributeSet, Integer?)
		Friend styleCount As Integer

		' where all the text is going 
		Friend outputStream As java.io.OutputStream

		Friend afterKeyword As Boolean

		Friend outputAttributes As MutableAttributeSet

		' the value of the last \\ucN keyword emitted 
		Friend unicodeCount As Integer

		' for efficiency's sake (ha) 
		Private workingSegment As Segment

		Friend outputConversion As Integer()

		''' <summary>
		''' The default color, used for text without an explicit color
		'''  attribute. 
		''' </summary>
		Public Shared ReadOnly defaultRTFColor As java.awt.Color = java.awt.Color.black

		Public Const defaultFontSize As Single = 12f

		Public Const defaultFontFamily As String = "Helvetica"

		' constants so we can avoid allocating objects in inner loops 
		Private Shared ReadOnly MagicToken As Object

	'     An array of character-keyword pairs. This could be done
	'       as a dictionary (and lookup would be quicker), but that
	'       would require allocating an object for every character
	'       written (slow!). 
		Friend Class CharacterKeywordPair
			  Public character As Char
			  Public keyword As String
		End Class
		Protected Friend Shared textKeywords As CharacterKeywordPair()

		Shared Sub New()
			MagicToken = New Object

			Dim textKeywordDictionary As Dictionary = RTFReader.textKeywords
			Dim keys As System.Collections.IEnumerator = textKeywordDictionary.keys()
			Dim tempPairs As New List(Of CharacterKeywordPair)
			Do While keys.hasMoreElements()
				Dim pair As New CharacterKeywordPair
				pair.keyword = CStr(keys.nextElement())
				pair.character = CStr(textKeywordDictionary.get(pair.keyword)).Chars(0)
				tempPairs.Add(pair)
			Loop
			textKeywords = New CharacterKeywordPair(tempPairs.Count - 1){}
			tempPairs.CopyTo(textKeywords)
		End Sub

		Friend Shared ReadOnly hexdigits As Char() = { "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c }

	Public Shared Sub writeDocument(ByVal d As Document, ByVal [to] As java.io.OutputStream)
		Dim gen As New RTFGenerator([to])
		Dim root As Element = d.defaultRootElement

		gen.examineElement(root)
		gen.writeRTFHeader()
		gen.writeDocumentProperties(d)

	'     TODO this assumes a particular element structure; is there
	'       a way to iterate more generically ? 
		Dim max As Integer = root.elementCount
		For idx As Integer = 0 To max - 1
			gen.writeParagraphElement(root.getElement(idx))
		Next idx

		gen.writeRTFTrailer()
	End Sub

	Public Sub New(ByVal [to] As java.io.OutputStream)
		colorTable = New Dictionary(Of Object, Integer?)
		colorTable.put(defaultRTFColor, Convert.ToInt32(0))
		colorCount = 1

		fontTable = New Dictionary(Of String, Integer?)
		fontCount = 0

		styleTable = New Dictionary(Of AttributeSet, Integer?)
		' TODO: put default style in style table 
		styleCount = 0

		workingSegment = New Segment

		outputStream = [to]

		unicodeCount = 1
	End Sub

	Public Overridable Sub examineElement(ByVal el As Element)
		Dim a As AttributeSet = el.attributes
		Dim fontName As String
		Dim foregroundColor, backgroundColor As Object

		tallyStyles(a)

		If a IsNot Nothing Then
			' TODO: default color must be color 0! 

			foregroundColor = StyleConstants.getForeground(a)
			If foregroundColor IsNot Nothing AndAlso colorTable.get(foregroundColor) Is Nothing Then
				colorTable.put(foregroundColor, New Integer?(colorCount))
				colorCount += 1
			End If

			backgroundColor = a.getAttribute(StyleConstants.Background)
			If backgroundColor IsNot Nothing AndAlso colorTable.get(backgroundColor) Is Nothing Then
				colorTable.put(backgroundColor, New Integer?(colorCount))
				colorCount += 1
			End If

			fontName = StyleConstants.getFontFamily(a)

			If fontName Is Nothing Then fontName = defaultFontFamily

			If fontName IsNot Nothing AndAlso fontTable.get(fontName) Is Nothing Then
				fontTable.put(fontName, New Integer?(fontCount))
				fontCount += 1
			End If
		End If

		Dim el_count As Integer = el.elementCount
		For el_idx As Integer = 0 To el_count - 1
			examineElement(el.getElement(el_idx))
		Next el_idx
	End Sub

	Private Sub tallyStyles(ByVal a As AttributeSet)
		Do While a IsNot Nothing
			If TypeOf a Is Style Then
				Dim aNum As Integer? = styleTable.get(a)
				If aNum Is Nothing Then
					styleCount = styleCount + 1
					aNum = New Integer?(styleCount)
					styleTable.put(a, aNum)
				End If
			End If
			a = a.resolveParent
		Loop
	End Sub

	Private Function findStyle(ByVal a As AttributeSet) As Style
		Do While a IsNot Nothing
			If TypeOf a Is Style Then
				Dim aNum As Object = styleTable.get(a)
				If aNum IsNot Nothing Then Return CType(a, Style)
			End If
			a = a.resolveParent
		Loop
		Return Nothing
	End Function

	Private Function findStyleNumber(ByVal a As AttributeSet, ByVal domain As String) As Integer?
		Do While a IsNot Nothing
			If TypeOf a Is Style Then
				Dim aNum As Integer? = styleTable.get(a)
				If aNum IsNot Nothing Then
					If domain Is Nothing OrElse domain.Equals(a.getAttribute(Constants.StyleType)) Then Return aNum
				End If

			End If
			a = a.resolveParent
		Loop
		Return Nothing
	End Function

	Private Shared Function attrDiff(ByVal oldAttrs As MutableAttributeSet, ByVal newAttrs As AttributeSet, ByVal key As Object, ByVal dfl As Object) As Object
		Dim oldValue, newValue As Object

		oldValue = oldAttrs.getAttribute(key)
		newValue = newAttrs.getAttribute(key)

		If newValue Is oldValue Then Return Nothing
		If newValue Is Nothing Then
			oldAttrs.removeAttribute(key)
			If dfl IsNot Nothing AndAlso (Not dfl.Equals(oldValue)) Then
				Return dfl
			Else
				Return Nothing
			End If
		End If
		If oldValue Is Nothing OrElse (Not equalArraysOK(oldValue, newValue)) Then
			oldAttrs.addAttribute(key, newValue)
			Return newValue
		End If
		Return Nothing
	End Function

	Private Shared Function equalArraysOK(ByVal a As Object, ByVal b As Object) As Boolean
		Dim aa, bb As Object()
		If a Is b Then Return True
		If a Is Nothing OrElse b Is Nothing Then Return False
		If a.Equals(b) Then Return True
		If Not(a.GetType().IsArray AndAlso b.GetType().IsArray) Then Return False
		aa = CType(a, Object())
		bb = CType(b, Object())
		If aa.Length <> bb.Length Then Return False

		Dim i As Integer
		Dim l As Integer = aa.Length
		For i = 0 To l - 1
			If Not equalArraysOK(aa(i), bb(i)) Then Return False
		Next i

		Return True
	End Function

	' Writes a line break to the output file, for ease in debugging 
	Public Overridable Sub writeLineBreak()
		writeRawString(vbLf)
		afterKeyword = False
	End Sub


	Public Overridable Sub writeRTFHeader()
		Dim index As Integer

	'     TODO: Should the writer attempt to examine the text it's writing
	'       and pick a character set which will most compactly represent the
	'       document? (currently the writer always uses the ansi character
	'       set, which is roughly ISO-8859 Latin-1, and uses Unicode escapes
	'       for all other characters. However Unicode is a relatively
	'       recent addition to RTF, and not all readers will understand it.) 
		writeBegingroup()
		writeControlWord("rtf", 1)
		writeControlWord("ansi")
		outputConversion = outputConversionForName("ansi")
		writeLineBreak()

		' write font table 
		Dim sortedFontTable As String() = New String(fontCount - 1){}
		Dim fonts As System.Collections.IEnumerator(Of String) = fontTable.keys()
		Dim font As String
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Do While fonts.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			font = fonts.nextElement()
			Dim num As Integer? = fontTable.get(font)
			sortedFontTable(num) = font
		Loop
		writeBegingroup()
		writeControlWord("fonttbl")
		For index = 0 To fontCount - 1
			writeControlWord("f", index)
			writeControlWord("fnil") ' TODO: supply correct font style
			writeText(sortedFontTable(index))
			writeText(";")
		Next index
		writeEndgroup()
		writeLineBreak()

		' write color table 
		If colorCount > 1 Then
			Dim sortedColorTable As java.awt.Color() = New java.awt.Color(colorCount - 1){}
			Dim colors As System.Collections.IEnumerator = colorTable.keys()
			Dim color As java.awt.Color
			Do While colors.hasMoreElements()
				color = CType(colors.nextElement(), java.awt.Color)
				Dim num As Integer? = colorTable.get(color)
				sortedColorTable(num) = color
			Loop
			writeBegingroup()
			writeControlWord("colortbl")
			For index = 0 To colorCount - 1
				color = sortedColorTable(index)
				If color IsNot Nothing Then
					writeControlWord("red", color.red)
					writeControlWord("green", color.green)
					writeControlWord("blue", color.blue)
				End If
				writeRawString(";")
			Next index
			writeEndgroup()
			writeLineBreak()
		End If

		' write the style sheet 
		If styleCount > 1 Then
			writeBegingroup()
			writeControlWord("stylesheet")
			Dim styles As System.Collections.IEnumerator(Of AttributeSet) = styleTable.keys()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While styles.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim style As Style = CType(styles.nextElement(), Style)
				Dim styleNumber As Integer = styleTable.get(style)
				writeBegingroup()
				Dim styleType As String = CStr(style.getAttribute(Constants.StyleType))
				If styleType Is Nothing Then styleType = Constants.STParagraph
				If styleType.Equals(Constants.STCharacter) Then
					writeControlWord("*")
					writeControlWord("cs", styleNumber)
				ElseIf styleType.Equals(Constants.STSection) Then
					writeControlWord("*")
					writeControlWord("ds", styleNumber)
				Else
					writeControlWord("s", styleNumber)
				End If

				Dim basis As AttributeSet = style.resolveParent
				Dim goat As MutableAttributeSet
				If basis Is Nothing Then
					goat = New SimpleAttributeSet
				Else
					goat = New SimpleAttributeSet(basis)
				End If

				updateSectionAttributes(goat, style, False)
				updateParagraphAttributes(goat, style, False)
				updateCharacterAttributes(goat, style, False)

				basis = style.resolveParent
				If basis IsNot Nothing AndAlso TypeOf basis Is Style Then
					Dim basedOn As Integer? = styleTable.get(basis)
					If basedOn IsNot Nothing Then writeControlWord("sbasedon", basedOn)
				End If

				Dim nextStyle As Style = CType(style.getAttribute(Constants.StyleNext), Style)
				If nextStyle IsNot Nothing Then
					Dim nextNum As Integer? = styleTable.get(nextStyle)
					If nextNum IsNot Nothing Then writeControlWord("snext", nextNum)
				End If

				Dim hidden As Boolean? = CBool(style.getAttribute(Constants.StyleHidden))
				If hidden IsNot Nothing AndAlso hidden Then writeControlWord("shidden")

				Dim additive As Boolean? = CBool(style.getAttribute(Constants.StyleAdditive))
				If additive IsNot Nothing AndAlso additive Then writeControlWord("additive")


				writeText(style.name)
				writeText(";")
				writeEndgroup()
			Loop
			writeEndgroup()
			writeLineBreak()
		End If

		outputAttributes = New SimpleAttributeSet
	End Sub

	Friend Overridable Sub writeDocumentProperties(ByVal doc As Document)
		' Write the document properties 
		Dim i As Integer
		Dim wroteSomething As Boolean = False

		For i = 0 To RTFAttributes.attributes.Length - 1
			Dim attr As RTFAttribute = RTFAttributes.attributes(i)
			If attr.domain() <> RTFAttribute.D_DOCUMENT Then Continue For
			Dim prop As Object = doc.getProperty(attr.swingName())
			Dim ok As Boolean = attr.writeValue(prop, Me, False)
			If ok Then wroteSomething = True
		Next i

		If wroteSomething Then writeLineBreak()
	End Sub

	Public Overridable Sub writeRTFTrailer()
		writeEndgroup()
		writeLineBreak()
	End Sub

	Protected Friend Overridable Sub checkNumericControlWord(ByVal currentAttributes As MutableAttributeSet, ByVal newAttributes As AttributeSet, ByVal attrName As Object, ByVal controlWord As String, ByVal dflt As Single, ByVal scale As Single)
		Dim parm As Object

		parm = attrDiff(currentAttributes, newAttributes, attrName, MagicToken)
		If parm IsNot Nothing Then
			Dim targ As Single
			If parm Is MagicToken Then
				targ = dflt
			Else
				targ = CType(parm, Number)
			End If
			writeControlWord(controlWord, Math.Round(targ * scale))
		End If
	End Sub

	Protected Friend Overridable Sub checkControlWord(ByVal currentAttributes As MutableAttributeSet, ByVal newAttributes As AttributeSet, ByVal word As RTFAttribute)
		Dim parm As Object

		parm = attrDiff(currentAttributes, newAttributes, word.swingName(), MagicToken)
		If parm IsNot Nothing Then
			If parm Is MagicToken Then parm = Nothing
			word.writeValue(parm, Me, True)
		End If
	End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
	protected void checkControlWords(MutableAttributeSet currentAttributes, AttributeSet newAttributes, RTFAttribute words() , int domain) throws java.io.IOException
		Dim wordIndex As Integer
		Dim wordCount As Integer = words.length
		For wordIndex = 0 To wordCount - 1
			Dim attr As RTFAttribute = words(wordIndex)
			If attr.domain() = domain Then checkControlWord(currentAttributes, newAttributes, attr)
		Next wordIndex

	void updateSectionAttributes(MutableAttributeSet current, AttributeSet newAttributes, Boolean emitStyleChanges) throws java.io.IOException
		If emitStyleChanges Then
			Dim oldStyle As Object = current.getAttribute("sectionStyle")
			Dim newStyle As Object = findStyleNumber(newAttributes, Constants.STSection)
			If oldStyle IsNot newStyle Then
				If oldStyle IsNot Nothing Then resetSectionAttributes(current)
				If newStyle IsNot Nothing Then
					writeControlWord("ds", CInt(Fix(newStyle)))
					current.addAttribute("sectionStyle", newStyle)
				Else
					current.removeAttribute("sectionStyle")
				End If
			End If
		End If

		checkControlWords(current, newAttributes, RTFAttributes.attributes, RTFAttribute.D_SECTION)

	protected void resetSectionAttributes(MutableAttributeSet currentAttributes) throws java.io.IOException
		writeControlWord("sectd")

		Dim wordIndex As Integer
		Dim wordCount As Integer = RTFAttributes.attributes.Length
		For wordIndex = 0 To wordCount - 1
			Dim attr As RTFAttribute = RTFAttributes.attributes(wordIndex)
			If attr.domain() = RTFAttribute.D_SECTION Then attr.default = currentAttributes
		Next wordIndex

		currentAttributes.removeAttribute("sectionStyle")

	void updateParagraphAttributes(MutableAttributeSet current, AttributeSet newAttributes, Boolean emitStyleChanges) throws java.io.IOException
		Dim parm As Object
		Dim oldStyle, newStyle As Object

	'     The only way to get rid of tabs or styles is with the \pard keyword,
	'       emitted by resetParagraphAttributes(). Ideally we should avoid
	'       emitting \pard if the new paragraph's tabs are a superset of the old
	'       paragraph's tabs. 

		If emitStyleChanges Then
			oldStyle = current.getAttribute("paragraphStyle")
			newStyle = findStyleNumber(newAttributes, Constants.STParagraph)
			If oldStyle IsNot newStyle Then
				If oldStyle IsNot Nothing Then
					resetParagraphAttributes(current)
					oldStyle = Nothing
				End If
			End If
		Else
			oldStyle = Nothing
			newStyle = Nothing
		End If

		Dim oldTabs As Object = current.getAttribute(Constants.Tabs)
		Dim newTabs As Object = newAttributes.getAttribute(Constants.Tabs)
		If oldTabs IsNot newTabs Then
			If oldTabs IsNot Nothing Then
				resetParagraphAttributes(current)
				oldTabs = Nothing
				oldStyle = Nothing
			End If
		End If

		If oldStyle IsNot newStyle AndAlso newStyle IsNot Nothing Then
			writeControlWord("s", CInt(Fix(newStyle)))
			current.addAttribute("paragraphStyle", newStyle)
		End If

		checkControlWords(current, newAttributes, RTFAttributes.attributes, RTFAttribute.D_PARAGRAPH)

		If oldTabs IsNot newTabs AndAlso newTabs IsNot Nothing Then
			Dim tabs As TabStop() = CType(newTabs, TabStop())
			Dim index As Integer
			For index = 0 To tabs.Length - 1
				Dim tab As TabStop = tabs(index)
				Select Case tab.alignment
				  Case TabStop.ALIGN_LEFT, TabStop.ALIGN_BAR
				  Case TabStop.ALIGN_RIGHT
					writeControlWord("tqr")
				  Case TabStop.ALIGN_CENTER
					writeControlWord("tqc")
				  Case TabStop.ALIGN_DECIMAL
					writeControlWord("tqdec")
				End Select
				Select Case tab.leader
				  Case TabStop.LEAD_NONE
				  Case TabStop.LEAD_DOTS
					writeControlWord("tldot")
				  Case TabStop.LEAD_HYPHENS
					writeControlWord("tlhyph")
				  Case TabStop.LEAD_UNDERLINE
					writeControlWord("tlul")
				  Case TabStop.LEAD_THICKLINE
					writeControlWord("tlth")
				  Case TabStop.LEAD_EQUALS
					writeControlWord("tleq")
				End Select
				Dim twips As Integer = Math.Round(20f * tab.position)
				If tab.alignment = TabStop.ALIGN_BAR Then
					writeControlWord("tb", twips)
				Else
					writeControlWord("tx", twips)
				End If
			Next index
			current.addAttribute(Constants.Tabs, tabs)
		End If

	public void writeParagraphElement(Element el) throws java.io.IOException
		updateParagraphAttributes(outputAttributes, el.attributes, True)

		Dim sub_count As Integer = el.elementCount
		For idx As Integer = 0 To sub_count - 1
			writeTextElement(el.getElement(idx))
		Next idx

		writeControlWord("par")
		writeLineBreak() ' makes the raw file more readable

	' debugging. TODO: remove.
	'private static String tabdump(Object tso)
	'{
	'    String buf;
	'    int i;
	'
	'    if (tso == null)
	'        return "[none]";
	'
	'    TabStop[] ts = (TabStop[])tso;
	'
	'    buf = "[";
	'    for(i = 0; i < ts.length; i++) {
	'        buf = buf + ts[i].toString();
	'        if ((i+1) < ts.length)
	'            buf = buf + ",";
	'    }
	'    return buf + "]";
	'}
	'

	protected void resetParagraphAttributes(MutableAttributeSet currentAttributes) throws java.io.IOException
		writeControlWord("pard")

		currentAttributes.addAttribute(StyleConstants.Alignment, Convert.ToInt32(0))

		Dim wordIndex As Integer
		Dim wordCount As Integer = RTFAttributes.attributes.Length
		For wordIndex = 0 To wordCount - 1
			Dim attr As RTFAttribute = RTFAttributes.attributes(wordIndex)
			If attr.domain() = RTFAttribute.D_PARAGRAPH Then attr.default = currentAttributes
		Next wordIndex

		currentAttributes.removeAttribute("paragraphStyle")
		currentAttributes.removeAttribute(Constants.Tabs)

	void updateCharacterAttributes(MutableAttributeSet current, AttributeSet newAttributes, Boolean updateStyleChanges) throws java.io.IOException
		Dim parm As Object

		If updateStyleChanges Then
			Dim oldStyle As Object = current.getAttribute("characterStyle")
			Dim newStyle As Object = findStyleNumber(newAttributes, Constants.STCharacter)
			If oldStyle IsNot newStyle Then
				If oldStyle IsNot Nothing Then resetCharacterAttributes(current)
				If newStyle IsNot Nothing Then
					writeControlWord("cs", CInt(Fix(newStyle)))
					current.addAttribute("characterStyle", newStyle)
				Else
					current.removeAttribute("characterStyle")
				End If
			End If
		End If

		parm = attrDiff(current, newAttributes, StyleConstants.FontFamily, Nothing)
		If parm IsNot Nothing Then
			Dim fontNum As Integer? = fontTable.get(parm)
			writeControlWord("f", fontNum)
		End If

		checkNumericControlWord(current, newAttributes, StyleConstants.FontSize, "fs", defaultFontSize, 2f)

		checkControlWords(current, newAttributes, RTFAttributes.attributes, RTFAttribute.D_CHARACTER)

		checkNumericControlWord(current, newAttributes, StyleConstants.LineSpacing, "sl", 0, 20f) ' TODO: sl wackiness

		parm = attrDiff(current, newAttributes, StyleConstants.Background, MagicToken)
		If parm IsNot Nothing Then
			Dim colorNum As Integer
			If parm Is MagicToken Then
				colorNum = 0
			Else
				colorNum = colorTable.get(parm)
			End If
			writeControlWord("cb", colorNum)
		End If

		parm = attrDiff(current, newAttributes, StyleConstants.Foreground, Nothing)
		If parm IsNot Nothing Then
			Dim colorNum As Integer
			If parm Is MagicToken Then
				colorNum = 0
			Else
				colorNum = colorTable.get(parm)
			End If
			writeControlWord("cf", colorNum)
		End If

	protected void resetCharacterAttributes(MutableAttributeSet currentAttributes) throws java.io.IOException
		writeControlWord("plain")

		Dim wordIndex As Integer
		Dim wordCount As Integer = RTFAttributes.attributes.Length
		For wordIndex = 0 To wordCount - 1
			Dim attr As RTFAttribute = RTFAttributes.attributes(wordIndex)
			If attr.domain() = RTFAttribute.D_CHARACTER Then attr.default = currentAttributes
		Next wordIndex

		StyleConstants.fontFamilyily(currentAttributes, defaultFontFamily)
		currentAttributes.removeAttribute(StyleConstants.FontSize) ' =default
		currentAttributes.removeAttribute(StyleConstants.Background)
		currentAttributes.removeAttribute(StyleConstants.Foreground)
		currentAttributes.removeAttribute(StyleConstants.LineSpacing)
		currentAttributes.removeAttribute("characterStyle")

	public void writeTextElement(Element el) throws java.io.IOException
		updateCharacterAttributes(outputAttributes, el.attributes, True)

		If el.leaf Then
			Try
				el.document.getText(el.startOffset, el.endOffset - el.startOffset, Me.workingSegment)
			Catch ble As BadLocationException
				' TODO is this the correct error to raise? 
				Console.WriteLine(ble.ToString())
				Console.Write(ble.StackTrace)
				Throw New InternalError(ble.Message)
			End Try
			writeText(Me.workingSegment)
		Else
			Dim sub_count As Integer = el.elementCount
			For idx As Integer = 0 To sub_count - 1
				writeTextElement(el.getElement(idx))
			Next idx
		End If

	public void writeText(Segment s) throws java.io.IOException
		Dim pos, [end] As Integer
		Dim array As Char()

		pos = s.offset
		[end] = pos + s.count
		array = s.array
		Do While pos < [end]
			writeCharacter(array(pos))
			pos += 1
		Loop

	public void writeText(String s) throws java.io.IOException
		Dim pos, [end] As Integer

		pos = 0
		[end] = s.length()
		Do While pos < [end]
			writeCharacter(s.Chars(pos))
			pos += 1
		Loop

	public void writeRawString(String str) throws java.io.IOException
		Dim strlen As Integer = str.length()
		For offset As Integer = 0 To strlen - 1
			outputStream.write(AscW(str.Chars(offset)))
		Next offset

	public void writeControlWord(String keyword) throws java.io.IOException
		outputStream.write("\"c)
		writeRawString(keyword)
		afterKeyword = True

	public void writeControlWord(String keyword, Integer arg) throws java.io.IOException
		outputStream.write("\"c)
		writeRawString(keyword)
		writeRawString(Convert.ToString(arg)) ' TODO: correct in all cases?
		afterKeyword = True

	public void writeBegingroup() throws java.io.IOException
		outputStream.write("{"c)
		afterKeyword = False

	public void writeEndgroup() throws java.io.IOException
		outputStream.write("}"c)
		afterKeyword = False

	public void writeCharacter(Char ch) throws java.io.IOException
	'     Nonbreaking space is in most RTF encodings, but the keyword is
	'       preferable; same goes for tabs 
		If ch = &HA0 Then ' nonbreaking space
			outputStream.write(&H5C) ' backslash
			outputStream.write(&H7E) ' tilde
			afterKeyword = False ' non-alpha keywords are self-terminating
			Return
		End If

		If ch = &H9 Then ' horizontal tab
			writeControlWord("tab")
			Return
		End If

		If ch = 10 OrElse ch = 13 Then ' newline / paragraph Return

		Dim b As Integer = convertCharacter(outputConversion, ch)
		If b = 0 Then
			' Unicode characters which have corresponding RTF keywords 
			Dim i As Integer
			For i = 0 To textKeywords.Length - 1
				If textKeywords(i).character = ch Then
					writeControlWord(textKeywords(i).keyword)
					Return
				End If
			Next i
	'         In some cases it would be reasonable to check to see if the
	'           glyph being written out is in the Symbol encoding, and if so,
	'           to switch to the Symbol font for this character. TODO. 
	'         Currently all unrepresentable characters are written as
	'           Unicode escapes. 
			Dim approximation As String = approximationForUnicode(ch)
			If approximation.Length <> unicodeCount Then
				unicodeCount = approximation.Length
				writeControlWord("uc", unicodeCount)
			End If
			writeControlWord("u", CInt(Fix(ch)))
			writeRawString(" ")
			writeRawString(approximation)
			afterKeyword = False
			Return
		End If

		If b > 127 Then
			Dim nybble As Integer
			outputStream.write("\"c)
			outputStream.write("'"c)
			nybble = CInt(CUInt((b And &HF0)) >> 4)
			outputStream.write(hexdigits(nybble))
			nybble = (b And &HF)
			outputStream.write(hexdigits(nybble))
			afterKeyword = False
			Return
		End If

		Select Case b
		Case "}"c, "{"c, "\"c
			outputStream.write(&H5C) ' backslash
			afterKeyword = False ' in a keyword, actually ...
			' fall through 
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
		Case Else
			If afterKeyword Then
				outputStream.write(&H20) ' space
				afterKeyword = False
			End If
			outputStream.write(b)
		End Select

	String approximationForUnicode(Char ch)
	'     TODO: Find reasonable approximations for all Unicode characters
	'       in all RTF code pages... heh, heh... 
		Return "?"

	''' <summary>
	''' Takes a translation table (a 256-element array of characters)
	''' and creates an output conversion table for use by
	''' convertCharacter(). 
	''' </summary>
	'     Not very efficient at all. Could be changed to sort the table
	'       for binary search. TODO. (Even though this is inefficient however,
	'       writing RTF is still much faster than reading it.) 
	static Integer() outputConversionFromTranslationTable(Char() table)
		Dim conversion As Integer() = New Integer(2 * table.length - 1){}

		Dim index As Integer

		For index = 0 To table.length - 1
			conversion(index * 2) = table(index)
			conversion((index * 2) + 1) = index
		Next index

		Return conversion

	static Integer() outputConversionForName(String name) throws java.io.IOException
		Dim table As Char() = CType(RTFReader.getCharacterSet(name), Char())
		Return outputConversionFromTranslationTable(table)

	''' <summary>
	''' Takes a char and a conversion table (an int[] in the current
	''' implementation, but conversion tables should be treated as an opaque
	''' type) and returns the
	''' corresponding byte value (as an int, since bytes are signed).
	''' </summary>
		' Not very efficient. TODO. 
	protected static Integer convertCharacter(Integer() conversion, Char ch)
	   Dim index As Integer

	   For index = 0 To conversion.length - 1 Step 2
		   If conversion(index) = ch Then Return conversion(index + 1)
	   Next index

	   Return 0 ' 0 indicates an unrepresentable character

	End Class

End Namespace