Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing.text

'
' * Copyright (c) 1998, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.text.html

	''' <summary>
	''' MinimalHTMLWriter is a fallback writer used by the
	''' HTMLEditorKit to write out HTML for a document that
	''' is a not produced by the EditorKit.
	''' 
	''' The format for the document is:
	''' <pre>
	''' &lt;html&gt;
	'''   &lt;head&gt;
	'''     &lt;style&gt;
	'''        &lt;!-- list of named styles
	'''         p.normal {
	'''            font-family: SansSerif;
	'''            margin-height: 0;
	'''            font-size: 14
	'''         }
	'''        --&gt;
	'''      &lt;/style&gt;
	'''   &lt;/head&gt;
	'''   &lt;body&gt;
	'''    &lt;p style=normal&gt;
	'''        <b>Bold, italic, and underline attributes
	'''        of the run are emitted as HTML tags.
	'''        The remaining attributes are emitted as
	'''        part of the style attribute of a &lt;span&gt; tag.
	'''        The syntax is similar to inline styles.</b>
	'''    &lt;/p&gt;
	'''   &lt;/body&gt;
	''' &lt;/html&gt;
	''' </pre>
	''' 
	''' @author Sunita Mani
	''' </summary>

	Public Class MinimalHTMLWriter
		Inherits AbstractWriter

		''' <summary>
		''' These static finals are used to
		''' tweak and query the fontMask about which
		''' of these tags need to be generated or
		''' terminated.
		''' </summary>
		Private Const BOLD As Integer = &H1
		Private Const ITALIC As Integer = &H2
		Private Const UNDERLINE As Integer = &H4

		' Used to map StyleConstants to CSS.
		Private Shared ReadOnly css As New CSS

		Private fontMask As Integer = 0

		Friend startOffset As Integer = 0
		Friend endOffset As Integer = 0

		''' <summary>
		''' Stores the attributes of the previous run.
		''' Used to compare with the current run's
		''' attributeset.  If identical, then a
		''' &lt;span&gt; tag is not emitted.
		''' </summary>
		Private fontAttributes As AttributeSet

		''' <summary>
		''' Maps from style name as held by the Document, to the archived
		''' style name (style name written out). These may differ.
		''' </summary>
		Private styleNameMapping As Dictionary(Of String, String)

		''' <summary>
		''' Creates a new MinimalHTMLWriter.
		''' </summary>
		''' <param name="w">  Writer </param>
		''' <param name="doc"> StyledDocument
		'''  </param>
		Public Sub New(ByVal w As java.io.Writer, ByVal doc As StyledDocument)
			MyBase.New(w, doc)
		End Sub

		''' <summary>
		''' Creates a new MinimalHTMLWriter.
		''' </summary>
		''' <param name="w">  Writer </param>
		''' <param name="doc"> StyledDocument </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content. </param>
		''' <param name="len"> The amount to write out.
		'''  </param>
		Public Sub New(ByVal w As java.io.Writer, ByVal doc As StyledDocument, ByVal pos As Integer, ByVal len As Integer)
			MyBase.New(w, doc, pos, len)
		End Sub

		''' <summary>
		''' Generates HTML output
		''' from a StyledDocument.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document.
		'''  </exception>
		Public Overrides Sub write()
			styleNameMapping = New Dictionary(Of String, String)
			writeStartTag("<html>")
			writeHeader()
			writeBody()
			writeEndTag("</html>")
		End Sub


		''' <summary>
		''' Writes out all the attributes for the
		''' following types:
		'''  StyleConstants.ParagraphConstants,
		'''  StyleConstants.CharacterConstants,
		'''  StyleConstants.FontConstants,
		'''  StyleConstants.ColorConstants.
		''' The attribute name and value are separated by a colon.
		''' Each pair is separated by a semicolon.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overrides Sub writeAttributes(ByVal attr As AttributeSet)
			Dim attributeNames As System.Collections.IEnumerator = attr.attributeNames
			Do While attributeNames.hasMoreElements()
				Dim name As Object = attributeNames.nextElement()
				If (TypeOf name Is StyleConstants.ParagraphConstants) OrElse (TypeOf name Is StyleConstants.CharacterConstants) OrElse (TypeOf name Is StyleConstants.FontConstants) OrElse (TypeOf name Is StyleConstants.ColorConstants) Then
					indent()
					write(name.ToString())
					write(":"c)
					write(css.styleConstantsValueToCSSValue(CType(name, StyleConstants), attr.getAttribute(name)).ToString())
					write(";"c)
					write(NEWLINE)
				End If
			Loop
		End Sub


		''' <summary>
		''' Writes out text.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overrides Sub text(ByVal elem As Element)
			Dim contentStr As String = getText(elem)
			If (contentStr.Length > 0) AndAlso (contentStr.Chars(contentStr.Length-1) = NEWLINE) Then contentStr = contentStr.Substring(0, contentStr.Length-1)
			If contentStr.Length > 0 Then write(contentStr)
		End Sub

		''' <summary>
		''' Writes out a start tag appropriately
		''' indented.  Also increments the indent level.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeStartTag(ByVal tag As String)
			indent()
			write(tag)
			write(NEWLINE)
			incrIndent()
		End Sub


		''' <summary>
		''' Writes out an end tag appropriately
		''' indented.  Also decrements the indent level.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeEndTag(ByVal endTag As String)
			decrIndent()
			indent()
			write(endTag)
			write(NEWLINE)
		End Sub


		''' <summary>
		''' Writes out the &lt;head&gt; and &lt;style&gt;
		''' tags, and then invokes writeStyles() to write
		''' out all the named styles as the content of the
		''' &lt;style&gt; tag.  The content is surrounded by
		''' valid HTML comment markers to ensure that the
		''' document is viewable in applications/browsers
		''' that do not support the tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeHeader()
			writeStartTag("<head>")
			writeStartTag("<style>")
			writeStartTag("<!--")
			writeStyles()
			writeEndTag("-->")
			writeEndTag("</style>")
			writeEndTag("</head>")
		End Sub



		''' <summary>
		''' Writes out all the named styles as the
		''' content of the &lt;style&gt; tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeStyles()
	'        
	'         *  Access to DefaultStyledDocument done to workaround
	'         *  a missing API in styled document to access the
	'         *  stylenames.
	'         
			Dim styledDoc As DefaultStyledDocument = (CType(document, DefaultStyledDocument))
			Dim styleNames As System.Collections.IEnumerator = styledDoc.styleNames

			Do While styleNames.hasMoreElements()
				Dim s As Style = styledDoc.getStyle(CStr(styleNames.nextElement()))

				''' <summary>
				''' PENDING: Once the name attribute is removed
				'''    from the list we check check for 0. *
				''' </summary>
				If s.attributeCount = 1 AndAlso s.isDefined(StyleConstants.NameAttribute) Then Continue Do
				indent()
				write("p." & addStyleName(s.name))
				write(" {" & vbLf)
				incrIndent()
				writeAttributes(s)
				decrIndent()
				indent()
				write("}" & vbLf)
			Loop
		End Sub


		''' <summary>
		''' Iterates over the elements in the document
		''' and processes elements based on whether they are
		''' branch elements or leaf elements.  This method specially handles
		''' leaf elements that are text.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeBody()
			Dim it As ElementIterator = elementIterator

	'        
	'          This will be a section element for a styled document.
	'          We represent this element in HTML as the body tags.
	'          Therefore we ignore it.
	'         
			it.current()

			Dim [next] As Element

			writeStartTag("<body>")

			Dim inContent As Boolean = False

			[next] = it.next()
			Do While [next] IsNot Nothing
				If Not inRange([next]) Then
					[next] = it.next()
					Continue Do
				End If
				If TypeOf [next] Is AbstractDocument.BranchElement Then
					If inContent Then
						writeEndParagraph()
						inContent = False
						fontMask = 0
					End If
					writeStartParagraph([next])
				ElseIf isText([next]) Then
					writeContent([next], (Not inContent))
					inContent = True
				Else
					writeLeaf([next])
					inContent = True
				End If
				[next] = it.next()
			Loop
			If inContent Then writeEndParagraph()
			writeEndTag("</body>")
		End Sub


		''' <summary>
		''' Emits an end tag for a &lt;p&gt;
		''' tag.  Before writing out the tag, this method ensures
		''' that all other tags that have been opened are
		''' appropriately closed off.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeEndParagraph()
			writeEndMask(fontMask)
			If inFontTag() Then
				endSpanTag()
			Else
				write(NEWLINE)
			End If
			writeEndTag("</p>")
		End Sub


		''' <summary>
		''' Emits the start tag for a paragraph. If
		''' the paragraph has a named style associated with it,
		''' then this method also generates a class attribute for the
		''' &lt;p&gt; tag and sets its value to be the name of the
		''' style.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeStartParagraph(ByVal elem As Element)
			Dim attr As AttributeSet = elem.attributes
			Dim resolveAttr As Object = attr.getAttribute(StyleConstants.ResolveAttribute)
			If TypeOf resolveAttr Is StyleContext.NamedStyle Then
				writeStartTag("<p class=" & mapStyleName(CType(resolveAttr, StyleContext.NamedStyle).name) & ">")
			Else
				writeStartTag("<p>")
			End If
		End Sub


		''' <summary>
		''' Responsible for writing out other non-text leaf
		''' elements.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeLeaf(ByVal elem As Element)
			indent()
			If elem.name = StyleConstants.IconElementName Then
				writeImage(elem)
			ElseIf elem.name = StyleConstants.ComponentElementName Then
				writeComponent(elem)
			End If
		End Sub


		''' <summary>
		''' Responsible for handling Icon Elements;
		''' deliberately unimplemented.  How to implement this method is
		''' an issue of policy.  For example, if you're generating
		''' an &lt;img&gt; tag, how should you
		''' represent the src attribute (the location of the image)?
		''' In certain cases it could be a URL, in others it could
		''' be read from a stream.
		''' </summary>
		''' <param name="elem"> element of type StyleConstants.IconElementName </param>
		Protected Friend Overridable Sub writeImage(ByVal elem As Element)
		End Sub


		''' <summary>
		''' Responsible for handling Component Elements;
		''' deliberately unimplemented.
		''' How this method is implemented is a matter of policy.
		''' </summary>
		Protected Friend Overridable Sub writeComponent(ByVal elem As Element)
		End Sub


		''' <summary>
		''' Returns true if the element is a text element.
		''' 
		''' </summary>
		Protected Friend Overridable Function isText(ByVal elem As Element) As Boolean
			Return (elem.name = AbstractDocument.ContentElementName)
		End Function


		''' <summary>
		''' Writes out the attribute set
		''' in an HTML-compliant manner.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overridable Sub writeContent(ByVal elem As Element, ByVal needsIndenting As Boolean)

			Dim attr As AttributeSet = elem.attributes
			writeNonHTMLAttributes(attr)
			If needsIndenting Then indent()
			writeHTMLTags(attr)
			text(elem)
		End Sub


		''' <summary>
		''' Generates
		''' bold &lt;b&gt;, italic &lt;i&gt;, and &lt;u&gt; tags for the
		''' text based on its attribute settings.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>

		Protected Friend Overridable Sub writeHTMLTags(ByVal attr As AttributeSet)

			Dim oldMask As Integer = fontMask
			fontMask = attr

			Dim endMask As Integer = 0
			Dim startMask As Integer = 0
			If (oldMask And BOLD) <> 0 Then
				If (fontMask And BOLD) = 0 Then endMask = endMask Or BOLD
			ElseIf (fontMask And BOLD) <> 0 Then
				startMask = startMask Or BOLD
			End If

			If (oldMask And ITALIC) <> 0 Then
				If (fontMask And ITALIC) = 0 Then endMask = endMask Or ITALIC
			ElseIf (fontMask And ITALIC) <> 0 Then
				startMask = startMask Or ITALIC
			End If

			If (oldMask And UNDERLINE) <> 0 Then
				If (fontMask And UNDERLINE) = 0 Then endMask = endMask Or UNDERLINE
			ElseIf (fontMask And UNDERLINE) <> 0 Then
				startMask = startMask Or UNDERLINE
			End If
			writeEndMask(endMask)
			writeStartMask(startMask)
		End Sub


		''' <summary>
		''' Tweaks the appropriate bits of fontMask
		''' to reflect whether the text is to be displayed in
		''' bold, italic, and/or with an underline.
		''' 
		''' </summary>
		Private Property fontMask As AttributeSet
			Set(ByVal attr As AttributeSet)
				If StyleConstants.isBold(attr) Then fontMask = fontMask Or BOLD
    
				If StyleConstants.isItalic(attr) Then fontMask = fontMask Or ITALIC
    
				If StyleConstants.isUnderline(attr) Then fontMask = fontMask Or UNDERLINE
			End Set
		End Property




		''' <summary>
		''' Writes out start tags &lt;u&gt;, &lt;i&gt;, and &lt;b&gt; based on
		''' the mask settings.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Private Sub writeStartMask(ByVal mask As Integer)
			If mask <> 0 Then
				If (mask And UNDERLINE) <> 0 Then write("<u>")
				If (mask And ITALIC) <> 0 Then write("<i>")
				If (mask And BOLD) <> 0 Then write("<b>")
			End If
		End Sub

		''' <summary>
		''' Writes out end tags for &lt;u&gt;, &lt;i&gt;, and &lt;b&gt; based on
		''' the mask settings.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Private Sub writeEndMask(ByVal mask As Integer)
			If mask <> 0 Then
				If (mask And BOLD) <> 0 Then write("</b>")
				If (mask And ITALIC) <> 0 Then write("</i>")
				If (mask And UNDERLINE) <> 0 Then write("</u>")
			End If
		End Sub


		''' <summary>
		''' Writes out the remaining
		''' character-level attributes (attributes other than bold,
		''' italic, and underline) in an HTML-compliant way.  Given that
		''' attributes such as font family and font size have no direct
		''' mapping to HTML tags, a &lt;span&gt; tag is generated and its
		''' style attribute is set to contain the list of remaining
		''' attributes just like inline styles.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeNonHTMLAttributes(ByVal attr As AttributeSet)

			Dim style As String = ""
			Dim separator As String = "; "

			If inFontTag() AndAlso fontAttributes.isEqual(attr) Then Return

			Dim first As Boolean = True
			Dim color As java.awt.Color = CType(attr.getAttribute(StyleConstants.Foreground), java.awt.Color)
			If color IsNot Nothing Then
				style &= "color: " & css.styleConstantsValueToCSSValue(CType(StyleConstants.Foreground, StyleConstants), color)
				first = False
			End If
			Dim size As Integer? = CInt(Fix(attr.getAttribute(StyleConstants.FontSize)))
			If size IsNot Nothing Then
				If Not first Then style += separator
				style &= "font-size: " & size & "pt"
				first = False
			End If

			Dim family As String = CStr(attr.getAttribute(StyleConstants.FontFamily))
			If family IsNot Nothing Then
				If Not first Then style += separator
				style &= "font-family: " & family
				first = False
			End If

			If style.Length > 0 Then
				If fontMask <> 0 Then
					writeEndMask(fontMask)
					fontMask = 0
				End If
				startSpanTag(style)
				fontAttributes = attr
			ElseIf fontAttributes IsNot Nothing Then
				writeEndMask(fontMask)
				fontMask = 0
				endSpanTag()
			End If
		End Sub


		''' <summary>
		''' Returns true if we are currently in a &lt;font&gt; tag.
		''' </summary>
		Protected Friend Overridable Function inFontTag() As Boolean
			Return (fontAttributes IsNot Nothing)
		End Function

		''' <summary>
		''' This is no longer used, instead &lt;span&gt; will be written out.
		''' <p>
		''' Writes out an end tag for the &lt;font&gt; tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub endFontTag()
			write(NEWLINE)
			writeEndTag("</font>")
			fontAttributes = Nothing
		End Sub


		''' <summary>
		''' This is no longer used, instead &lt;span&gt; will be written out.
		''' <p>
		''' Writes out a start tag for the &lt;font&gt; tag.
		''' Because font tags cannot be nested,
		''' this method closes out
		''' any enclosing font tag before writing out a
		''' new start tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub startFontTag(ByVal style As String)
			Dim callIndent As Boolean = False
			If inFontTag() Then
				endFontTag()
				callIndent = True
			End If
			writeStartTag("<font style=""" & style & """>")
			If callIndent Then indent()
		End Sub

		''' <summary>
		''' Writes out a start tag for the &lt;font&gt; tag.
		''' Because font tags cannot be nested,
		''' this method closes out
		''' any enclosing font tag before writing out a
		''' new start tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Private Sub startSpanTag(ByVal style As String)
			Dim callIndent As Boolean = False
			If inFontTag() Then
				endSpanTag()
				callIndent = True
			End If
			writeStartTag("<span style=""" & style & """>")
			If callIndent Then indent()
		End Sub

		''' <summary>
		''' Writes out an end tag for the &lt;span&gt; tag.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Private Sub endSpanTag()
			write(NEWLINE)
			writeEndTag("</span>")
			fontAttributes = Nothing
		End Sub

		''' <summary>
		''' Adds the style named <code>style</code> to the style mapping. This
		''' returns the name that should be used when outputting. CSS does not
		''' allow the full Unicode set to be used as a style name.
		''' </summary>
		Private Function addStyleName(ByVal style As String) As String
			If styleNameMapping Is Nothing Then Return style
			Dim sb As StringBuilder = Nothing
			For counter As Integer = style.Length - 1 To 0 Step -1
				If Not isValidCharacter(style.Chars(counter)) Then
					If sb Is Nothing Then sb = New StringBuilder(style)
					sb(counter) = "a"c
				End If
			Next counter
			Dim mappedName As String = If(sb IsNot Nothing, sb.ToString(), style)
			Do While styleNameMapping(mappedName) IsNot Nothing
				mappedName = mappedName + AscW("x"c)
			Loop
			styleNameMapping(style) = mappedName
			Return mappedName
		End Function

		''' <summary>
		''' Returns the mapped style name corresponding to <code>style</code>.
		''' </summary>
		Private Function mapStyleName(ByVal style As String) As String
			If styleNameMapping Is Nothing Then Return style
			Dim retValue As String = styleNameMapping(style)
			Return If(retValue Is Nothing, style, retValue)
		End Function

		Private Function isValidCharacter(ByVal character As Char) As Boolean
			Return ((character >= "a"c AndAlso character <= "z"c) OrElse (character >= "A"c AndAlso character <= "Z"c))
		End Function
	End Class

End Namespace