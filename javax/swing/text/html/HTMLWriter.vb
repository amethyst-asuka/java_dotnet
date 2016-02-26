Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.text

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
Namespace javax.swing.text.html


	''' <summary>
	''' This is a writer for HTMLDocuments.
	''' 
	''' @author  Sunita Mani
	''' </summary>


	Public Class HTMLWriter
		Inherits AbstractWriter

	'    
	'     * Stores all elements for which end tags have to
	'     * be emitted.
	'     
		Private blockElementStack As New Stack(Of Element)
		Private inContent As Boolean = False
		Private inPre As Boolean = False
		''' <summary>
		''' When inPre is true, this will indicate the end offset of the pre
		''' element. 
		''' </summary>
		Private preEndOffset As Integer
		Private inTextArea As Boolean = False
		Private newlineOutputed As Boolean = False
		Private completeDoc As Boolean

	'    
	'     * Stores all embedded tags. Embedded tags are tags that are
	'     * stored as attributes in other tags. Generally they're
	'     * character level attributes.  Examples include
	'     * &lt;b&gt;, &lt;i&gt;, &lt;font&gt;, and &lt;a&gt;.
	'     
		Private tags As New List(Of HTML.Tag)(10)

		''' <summary>
		''' Values for the tags.
		''' </summary>
		Private tagValues As New List(Of Object)(10)

		''' <summary>
		''' Used when writing out content.
		''' </summary>
		Private segment As Segment

	'    
	'     * This is used in closeOutUnwantedEmbeddedTags.
	'     
		Private tagsToRemove As New List(Of HTML.Tag)(10)

		''' <summary>
		''' Set to true after the head has been output.
		''' </summary>
		Private wroteHead As Boolean

		''' <summary>
		''' Set to true when entities (such as &lt;) should be replaced.
		''' </summary>
		Private replaceEntities As Boolean

		''' <summary>
		''' Temporary buffer.
		''' </summary>
		Private tempChars As Char()


		''' <summary>
		''' Creates a new HTMLWriter.
		''' </summary>
		''' <param name="w">   a Writer </param>
		''' <param name="doc">  an HTMLDocument
		'''  </param>
		Public Sub New(ByVal w As java.io.Writer, ByVal doc As HTMLDocument)
			Me.New(w, doc, 0, doc.length)
		End Sub

		''' <summary>
		''' Creates a new HTMLWriter.
		''' </summary>
		''' <param name="w">  a Writer </param>
		''' <param name="doc"> an HTMLDocument </param>
		''' <param name="pos"> the document location from which to fetch the content </param>
		''' <param name="len"> the amount to write out </param>
		Public Sub New(ByVal w As java.io.Writer, ByVal doc As HTMLDocument, ByVal pos As Integer, ByVal len As Integer)
			MyBase.New(w, doc, pos, len)
			completeDoc = (pos = 0 AndAlso len = doc.length)
			lineLength = 80
		End Sub

		''' <summary>
		''' Iterates over the
		''' Element tree and controls the writing out of
		''' all the tags and its attributes.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document.
		'''  </exception>
		Public Overrides Sub write()
			Dim it As ElementIterator = elementIterator
			Dim current As Element = Nothing
			Dim [next] As Element

			wroteHead = False
			currentLineLength = 0
			replaceEntities = False
			canWrapLines = False
			If segment Is Nothing Then segment = New Segment
			inPre = False
			Dim forcedBody As Boolean = False
			[next] = it.next()
			Do While [next] IsNot Nothing
				If Not inRange([next]) Then
					If completeDoc AndAlso [next].attributes.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.BODY Then
						forcedBody = True
					Else
						[next] = it.next()
						Continue Do
					End If
				End If
				If current IsNot Nothing Then

	'                
	'                  if next is child of current increment indent
	'                

					If indentNeedsIncrementing(current, [next]) Then
						incrIndent()
					ElseIf current.parentElement IsNot [next].parentElement Then
	'                    
	'                       next and current are not siblings
	'                       so emit end tags for items on the stack until the
	'                       item on top of the stack, is the parent of the
	'                       next.
	'                    
						Dim top As Element = blockElementStack.Peek()
						Do While top IsNot [next].parentElement
	'                        
	'                           pop() will return top.
	'                        
							blockElementStack.Pop()
							If Not synthesizedElement(top) Then
								Dim attrs As AttributeSet = top.attributes
								If (Not matchNameAttribute(attrs, HTML.Tag.PRE)) AndAlso (Not isFormElementWithContent(attrs)) Then decrIndent()
								endTag(top)
							End If
							top = blockElementStack.Peek()
						Loop
					ElseIf current.parentElement Is [next].parentElement Then
	'                    
	'                       if next and current are siblings the indent level
	'                       is correct.  But, we need to make sure that if current is
	'                       on the stack, we pop it off, and put out its end tag.
	'                    
						Dim top As Element = blockElementStack.Peek()
						If top Is current Then
							blockElementStack.Pop()
							endTag(top)
						End If
					End If
				End If
				If (Not [next].leaf) OrElse isFormElementWithContent([next].attributes) Then
					blockElementStack.Push([next])
					startTag([next])
				Else
					emptyTag([next])
				End If
				current = [next]
				[next] = it.next()
			Loop
			' Emit all remaining end tags 

	'         A null parameter ensures that all embedded tags
	'           currently in the tags vector have their
	'           corresponding end tags written out.
	'        
			closeOutUnwantedEmbeddedTags(Nothing)

			If forcedBody Then
				blockElementStack.Pop()
				endTag(current)
			End If
			Do While blockElementStack.Count > 0
				current = blockElementStack.Pop()
				If Not synthesizedElement(current) Then
					Dim attrs As AttributeSet = current.attributes
					If (Not matchNameAttribute(attrs, HTML.Tag.PRE)) AndAlso (Not isFormElementWithContent(attrs)) Then decrIndent()
					endTag(current)
				End If
			Loop

			If completeDoc Then writeAdditionalComments()

			segment.array = Nothing
		End Sub


		''' <summary>
		''' Writes out the attribute set.  Ignores all
		''' attributes with a key of type HTML.Tag,
		''' attributes with a key of type StyleConstants,
		''' and attributes with a key of type
		''' HTML.Attribute.ENDTAG.
		''' </summary>
		''' <param name="attr">   an AttributeSet </param>
		''' <exception cref="IOException"> on any I/O error
		'''  </exception>
		Protected Friend Overrides Sub writeAttributes(ByVal attr As AttributeSet)
			' translate css attributes to html
			convAttr.removeAttributes(convAttr)
			convertToHTML32(attr, convAttr)

			Dim names As System.Collections.IEnumerator = convAttr.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()
				If TypeOf name Is HTML.Tag OrElse TypeOf name Is StyleConstants OrElse name Is HTML.Attribute.ENDTAG Then Continue Do
				write(" " & name & "=""" & convAttr.getAttribute(name) & """")
			Loop
		End Sub

		''' <summary>
		''' Writes out all empty elements (all tags that have no
		''' corresponding end tag).
		''' </summary>
		''' <param name="elem">   an Element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overridable Sub emptyTag(ByVal elem As Element)

			If (Not inContent) AndAlso (Not inPre) Then indentSmart()

			Dim attr As AttributeSet = elem.attributes
			closeOutUnwantedEmbeddedTags(attr)
			writeEmbeddedTags(attr)

			If matchNameAttribute(attr, HTML.Tag.CONTENT) Then
				inContent = True
				text(elem)
			ElseIf matchNameAttribute(attr, HTML.Tag.COMMENT) Then
				comment(elem)
			Else
				Dim isBlock As Boolean = isBlockTag(elem.attributes)
				If inContent AndAlso isBlock Then
					writeLineSeparator()
					indentSmart()
				End If

				Dim nameTag As Object = If(attr IsNot Nothing, attr.getAttribute(StyleConstants.NameAttribute), Nothing)
				Dim endTag As Object = If(attr IsNot Nothing, attr.getAttribute(HTML.Attribute.ENDTAG), Nothing)

				Dim outputEndTag As Boolean = False
				' If an instance of an UNKNOWN Tag, or an instance of a
				' tag that is only visible during editing
				'
				If nameTag IsNot Nothing AndAlso endTag IsNot Nothing AndAlso (TypeOf endTag Is String) AndAlso endTag.Equals("true") Then outputEndTag = True

				If completeDoc AndAlso matchNameAttribute(attr, HTML.Tag.HEAD) Then
					If outputEndTag Then writeStyles(CType(document, HTMLDocument).styleSheet)
					wroteHead = True
				End If

				write("<"c)
				If outputEndTag Then write("/"c)
				write(elem.name)
				writeAttributes(attr)
				write(">"c)
				If matchNameAttribute(attr, HTML.Tag.TITLE) AndAlso (Not outputEndTag) Then
					Dim doc As Document = elem.document
					Dim title As String = CStr(doc.getProperty(Document.TitleProperty))
					write(title)
				ElseIf (Not inContent) OrElse isBlock Then
					writeLineSeparator()
					If isBlock AndAlso inContent Then indentSmart()
				End If
			End If
		End Sub

		''' <summary>
		''' Determines if the HTML.Tag associated with the
		''' element is a block tag.
		''' </summary>
		''' <param name="attr">  an AttributeSet </param>
		''' <returns>  true if tag is block tag, false otherwise. </returns>
		Protected Friend Overridable Function isBlockTag(ByVal attr As AttributeSet) As Boolean
			Dim o As Object = attr.getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim name As HTML.Tag = CType(o, HTML.Tag)
				Return name.block
			End If
			Return False
		End Function


		''' <summary>
		''' Writes out a start tag for the element.
		''' Ignores all synthesized elements.
		''' </summary>
		''' <param name="elem">   an Element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub startTag(ByVal elem As Element)

			If synthesizedElement(elem) Then Return

			' Determine the name, as an HTML.Tag.
			Dim attr As AttributeSet = elem.attributes
			Dim nameAttribute As Object = attr.getAttribute(StyleConstants.NameAttribute)
			Dim name As HTML.Tag
			If TypeOf nameAttribute Is HTML.Tag Then
				name = CType(nameAttribute, HTML.Tag)
			Else
				name = Nothing
			End If

			If name Is HTML.Tag.PRE Then
				inPre = True
				preEndOffset = elem.endOffset
			End If

			' write out end tags for item on stack
			closeOutUnwantedEmbeddedTags(attr)

			If inContent Then
				writeLineSeparator()
				inContent = False
				newlineOutputed = False
			End If

			If completeDoc AndAlso name Is HTML.Tag.BODY AndAlso (Not wroteHead) Then
				' If the head has not been output, output it and the styles.
				wroteHead = True
				indentSmart()
				write("<head>")
				writeLineSeparator()
				incrIndent()
				writeStyles(CType(document, HTMLDocument).styleSheet)
				decrIndent()
				writeLineSeparator()
				indentSmart()
				write("</head>")
				writeLineSeparator()
			End If

			indentSmart()
			write("<"c)
			write(elem.name)
			writeAttributes(attr)
			write(">"c)
			If name IsNot HTML.Tag.PRE Then writeLineSeparator()

			If name Is HTML.Tag.TEXTAREA Then
				textAreaContent(elem.attributes)
			ElseIf name Is HTML.Tag.SELECT Then
				selectContent(elem.attributes)
			ElseIf completeDoc AndAlso name Is HTML.Tag.BODY Then
				' Write out the maps, which is not stored as Elements in
				' the Document.
				writeMaps(CType(document, HTMLDocument).maps)
			ElseIf name Is HTML.Tag.HEAD Then
				Dim ___document As HTMLDocument = CType(document, HTMLDocument)
				wroteHead = True
				incrIndent()
				writeStyles(___document.styleSheet)
				If ___document.hasBaseTag() Then
					indentSmart()
					write("<base href=""" & ___document.base & """>")
					writeLineSeparator()
				End If
				decrIndent()
			End If

		End Sub


		''' <summary>
		''' Writes out text that is contained in a TEXTAREA form
		''' element.
		''' </summary>
		''' <param name="attr">  an AttributeSet </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overridable Sub textAreaContent(ByVal attr As AttributeSet)
			Dim doc As Document = CType(attr.getAttribute(StyleConstants.ModelAttribute), Document)
			If doc IsNot Nothing AndAlso doc.length > 0 Then
				If segment Is Nothing Then segment = New Segment
				doc.getText(0, doc.length, segment)
				If segment.count > 0 Then
					inTextArea = True
					incrIndent()
					indentSmart()
					canWrapLines = True
					replaceEntities = True
					write(segment.array, segment.offset, segment.count)
					replaceEntities = False
					canWrapLines = False
					writeLineSeparator()
					inTextArea = False
					decrIndent()
				End If
			End If
		End Sub


		''' <summary>
		''' Writes out text.  If a range is specified when the constructor
		''' is invoked, then only the appropriate range of text is written
		''' out.
		''' </summary>
		''' <param name="elem">   an Element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overrides Sub text(ByVal elem As Element)
			Dim start As Integer = Math.Max(startOffset, elem.startOffset)
			Dim [end] As Integer = Math.Min(endOffset, elem.endOffset)
			If start < [end] Then
				If segment Is Nothing Then segment = New Segment
				document.getText(start, [end] - start, segment)
				newlineOutputed = False
				If segment.count > 0 Then
					If segment.array(segment.offset + segment.count - 1) = ControlChars.Lf Then newlineOutputed = True
					If inPre AndAlso [end] = preEndOffset Then
						If segment.count > 1 Then
							segment.count -= 1
						Else
							Return
						End If
					End If
					replaceEntities = True
					canWrapLines = (Not inPre)
					write(segment.array, segment.offset, segment.count)
					canWrapLines = False
					replaceEntities = False
				End If
			End If
		End Sub

		''' <summary>
		''' Writes out the content of the SELECT form element.
		''' </summary>
		''' <param name="attr"> the AttributeSet associated with the form element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub selectContent(ByVal attr As AttributeSet)
			Dim model As Object = attr.getAttribute(StyleConstants.ModelAttribute)
			incrIndent()
			If TypeOf model Is OptionListModel Then
				Dim listModel As OptionListModel(Of [Option]) = CType(model, OptionListModel(Of [Option]))
				Dim size As Integer = listModel.size
				For i As Integer = 0 To size - 1
					Dim [option] As [Option] = listModel.getElementAt(i)
					writeOption([option])
				Next i
			ElseIf TypeOf model Is OptionComboBoxModel Then
				Dim comboBoxModel As OptionComboBoxModel(Of [Option]) = CType(model, OptionComboBoxModel(Of [Option]))
				Dim size As Integer = comboBoxModel.size
				For i As Integer = 0 To size - 1
					Dim [option] As [Option] = comboBoxModel.getElementAt(i)
					writeOption([option])
				Next i
			End If
			decrIndent()
		End Sub


		''' <summary>
		''' Writes out the content of the Option form element. </summary>
		''' <param name="option">  an Option </param>
		''' <exception cref="IOException"> on any I/O error
		'''  </exception>
		Protected Friend Overridable Sub writeOption(ByVal [option] As [Option])

			indentSmart()
			write("<"c)
			write("option")
			' PENDING: should this be changed to check for null first?
			Dim value As Object = [option].attributes.getAttribute(HTML.Attribute.VALUE)
			If value IsNot Nothing Then write(" value=" & value)
			If [option].selected Then write(" selected")
			write(">"c)
			If [option].label IsNot Nothing Then write([option].label)
			writeLineSeparator()
		End Sub

		''' <summary>
		''' Writes out an end tag for the element.
		''' </summary>
		''' <param name="elem">    an Element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub endTag(ByVal elem As Element)
			If synthesizedElement(elem) Then Return

			' write out end tags for item on stack
			closeOutUnwantedEmbeddedTags(elem.attributes)
			If inContent Then
				If (Not newlineOutputed) AndAlso (Not inPre) Then writeLineSeparator()
				newlineOutputed = False
				inContent = False
			End If
			If Not inPre Then indentSmart()
			If matchNameAttribute(elem.attributes, HTML.Tag.PRE) Then inPre = False
			write("<"c)
			write("/"c)
			write(elem.name)
			write(">"c)
			writeLineSeparator()
		End Sub



		''' <summary>
		''' Writes out comments.
		''' </summary>
		''' <param name="elem">    an Element </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overridable Sub comment(ByVal elem As Element)
			Dim [as] As AttributeSet = elem.attributes
			If matchNameAttribute([as], HTML.Tag.COMMENT) Then
				Dim comment As Object = [as].getAttribute(HTML.Attribute.COMMENT)
				If TypeOf comment Is String Then
					writeComment(CStr(comment))
				Else
					writeComment(Nothing)
				End If
			End If
		End Sub


		''' <summary>
		''' Writes out comment string.
		''' </summary>
		''' <param name="string">   the comment </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Friend Overridable Sub writeComment(ByVal [string] As String)
			write("<!--")
			If [string] IsNot Nothing Then write([string])
			write("-->")
			writeLineSeparator()
			indentSmart()
		End Sub


		''' <summary>
		''' Writes out any additional comments (comments outside of the body)
		''' stored under the property HTMLDocument.AdditionalComments.
		''' </summary>
		Friend Overridable Sub writeAdditionalComments()
			Dim comments As Object = document.getProperty(HTMLDocument.AdditionalComments)

			If TypeOf comments Is ArrayList Then
				Dim v As ArrayList = CType(comments, ArrayList)
				Dim counter As Integer = 0
				Dim maxCounter As Integer = v.Count
				Do While counter < maxCounter
					writeComment(v(counter).ToString())
					counter += 1
				Loop
			End If
		End Sub


		''' <summary>
		''' Returns true if the element is a
		''' synthesized element.  Currently we are only testing
		''' for the p-implied tag.
		''' </summary>
		Protected Friend Overridable Function synthesizedElement(ByVal elem As Element) As Boolean
			If matchNameAttribute(elem.attributes, HTML.Tag.IMPLIED) Then Return True
			Return False
		End Function


		''' <summary>
		''' Returns true if the StyleConstants.NameAttribute is
		''' equal to the tag that is passed in as a parameter.
		''' </summary>
		Protected Friend Overridable Function matchNameAttribute(ByVal attr As AttributeSet, ByVal tag As HTML.Tag) As Boolean
			Dim o As Object = attr.getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim name As HTML.Tag = CType(o, HTML.Tag)
				If name Is tag Then Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Searches for embedded tags in the AttributeSet
		''' and writes them out.  It also stores these tags in a vector
		''' so that when appropriate the corresponding end tags can be
		''' written out.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeEmbeddedTags(ByVal attr As AttributeSet)

			' translate css attributes to html
			attr = convertToHTML(attr, oConvAttr)

			Dim names As System.Collections.IEnumerator = attr.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()
				If TypeOf name Is HTML.Tag Then
					Dim tag As HTML.Tag = CType(name, HTML.Tag)
					If tag Is HTML.Tag.FORM OrElse tags.Contains(tag) Then Continue Do
					write("<"c)
					write(tag.ToString())
					Dim o As Object = attr.getAttribute(tag)
					If o IsNot Nothing AndAlso TypeOf o Is AttributeSet Then writeAttributes(CType(o, AttributeSet))
					write(">"c)
					tags.Add(tag)
					tagValues.Add(o)
				End If
			Loop
		End Sub


		''' <summary>
		''' Searches the attribute set for a tag, both of which
		''' are passed in as a parameter.  Returns true if no match is found
		''' and false otherwise.
		''' </summary>
		Private Function noMatchForTagInAttributes(ByVal attr As AttributeSet, ByVal t As HTML.Tag, ByVal tagValue As Object) As Boolean
			If attr IsNot Nothing AndAlso attr.isDefined(t) Then
				Dim newValue As Object = attr.getAttribute(t)

				If If(tagValue Is Nothing, (newValue Is Nothing), (newValue IsNot Nothing AndAlso tagValue.Equals(newValue))) Then Return False
			End If
			Return True
		End Function


		''' <summary>
		''' Searches the attribute set and for each tag
		''' that is stored in the tag vector.  If the tag is not found,
		''' then the tag is removed from the vector and a corresponding
		''' end tag is written out.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub closeOutUnwantedEmbeddedTags(ByVal attr As AttributeSet)

			tagsToRemove.Clear()

			' translate css attributes to html
			attr = convertToHTML(attr, Nothing)

			Dim t As HTML.Tag
			Dim tValue As Object
			Dim firstIndex As Integer = -1
			Dim size As Integer = tags.Count
			' First, find all the tags that need to be removed.
			For i As Integer = size - 1 To 0 Step -1
				t = tags(i)
				tValue = tagValues(i)
				If (attr Is Nothing) OrElse noMatchForTagInAttributes(attr, t, tValue) Then
					firstIndex = i
					tagsToRemove.Add(t)
				End If
			Next i
			If firstIndex <> -1 Then
				' Then close them out.
				Dim removeAll As Boolean = ((size - firstIndex) = tagsToRemove.Count)
				For i As Integer = size - 1 To firstIndex Step -1
					t = tags(i)
					If removeAll OrElse tagsToRemove.Contains(t) Then
						tags.RemoveAt(i)
						tagValues.RemoveAt(i)
					End If
					write("<"c)
					write("/"c)
					write(t.ToString())
					write(">"c)
				Next i
				' Have to output any tags after firstIndex that still remaing,
				' as we closed them out, but they should remain open.
				size = tags.Count
				For i As Integer = firstIndex To size - 1
					t = tags(i)
					write("<"c)
					write(t.ToString())
					Dim o As Object = tagValues(i)
					If o IsNot Nothing AndAlso TypeOf o Is AttributeSet Then writeAttributes(CType(o, AttributeSet))
					write(">"c)
				Next i
			End If
		End Sub


		''' <summary>
		''' Determines if the element associated with the attributeset
		''' is a TEXTAREA or SELECT.  If true, returns true else
		''' false
		''' </summary>
		Private Function isFormElementWithContent(ByVal attr As AttributeSet) As Boolean
			Return matchNameAttribute(attr, HTML.Tag.TEXTAREA) OrElse matchNameAttribute(attr, HTML.Tag.SELECT)
		End Function


		''' <summary>
		''' Determines whether a the indentation needs to be
		''' incremented.  Basically, if next is a child of current, and
		''' next is NOT a synthesized element, the indent level will be
		''' incremented.  If there is a parent-child relationship and "next"
		''' is a synthesized element, then its children must be indented.
		''' This state is maintained by the indentNext boolean.
		''' </summary>
		''' <returns> boolean that's true if indent level
		'''         needs incrementing. </returns>
		Private indentNext As Boolean = False
		Private Function indentNeedsIncrementing(ByVal current As Element, ByVal [next] As Element) As Boolean
			If ([next].parentElement Is current) AndAlso (Not inPre) Then
				If indentNext Then
					indentNext = False
					Return True
				ElseIf synthesizedElement([next]) Then
					indentNext = True
				ElseIf Not synthesizedElement(current) Then
					Return True
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Outputs the maps as elements. Maps are not stored as elements in
		''' the document, and as such this is used to output them.
		''' </summary>
		Friend Overridable Sub writeMaps(ByVal maps As System.Collections.IEnumerator)
			If maps IsNot Nothing Then
				Do While maps.hasMoreElements()
					Dim ___map As Map = CType(maps.nextElement(), Map)
					Dim name As String = ___map.name

					incrIndent()
					indentSmart()
					write("<map")
					If name IsNot Nothing Then
						write(" name=""")
						write(name)
						write(""">")
					Else
						write(">"c)
					End If
					writeLineSeparator()
					incrIndent()

					' Output the areas
					Dim areas As AttributeSet() = ___map.areas
					If areas IsNot Nothing Then
						Dim counter As Integer = 0
						Dim maxCounter As Integer = areas.Length
						Do While counter < maxCounter
							indentSmart()
							write("<area")
							writeAttributes(areas(counter))
							write("></area>")
							writeLineSeparator()
							counter += 1
						Loop
					End If
					decrIndent()
					indentSmart()
					write("</map>")
					writeLineSeparator()
					decrIndent()
				Loop
			End If
		End Sub

		''' <summary>
		''' Outputs the styles as a single element. Styles are not stored as
		''' elements, but part of the document. For the time being styles are
		''' written out as a comment, inside a style tag.
		''' </summary>
		Friend Overridable Sub writeStyles(ByVal sheet As StyleSheet)
			If sheet IsNot Nothing Then
				Dim styles As System.Collections.IEnumerator = sheet.styleNames
				If styles IsNot Nothing Then
					Dim outputStyle As Boolean = False
					Do While styles.hasMoreElements()
						Dim name As String = CStr(styles.nextElement())
						' Don't write out the default style.
						If (Not StyleContext.DEFAULT_STYLE.Equals(name)) AndAlso writeStyle(name, sheet.getStyle(name), outputStyle) Then outputStyle = True
					Loop
					If outputStyle Then writeStyleEndTag()
				End If
			End If
		End Sub

		''' <summary>
		''' Outputs the named style. <code>outputStyle</code> indicates
		''' whether or not a style has been output yet. This will return
		''' true if a style is written.
		''' </summary>
		Friend Overridable Function writeStyle(ByVal name As String, ByVal style As Style, ByVal outputStyle As Boolean) As Boolean
			Dim didOutputStyle As Boolean = False
			Dim attributes As System.Collections.IEnumerator = style.attributeNames
			If attributes IsNot Nothing Then
				Do While attributes.hasMoreElements()
					Dim attribute As Object = attributes.nextElement()
					If TypeOf attribute Is CSS.Attribute Then
						Dim value As String = style.getAttribute(attribute).ToString()
						If value IsNot Nothing Then
							If Not outputStyle Then
								writeStyleStartTag()
								outputStyle = True
							End If
							If Not didOutputStyle Then
								didOutputStyle = True
								indentSmart()
								write(name)
								write(" {")
							Else
								write(";")
							End If
							write(" "c)
							write(attribute.ToString())
							write(": ")
							write(value)
						End If
					End If
				Loop
			End If
			If didOutputStyle Then
				write(" }")
				writeLineSeparator()
			End If
			Return didOutputStyle
		End Function

		Friend Overridable Sub writeStyleStartTag()
			indentSmart()
			write("<style type=""text/css"">")
			incrIndent()
			writeLineSeparator()
			indentSmart()
			write("<!--")
			incrIndent()
			writeLineSeparator()
		End Sub

		Friend Overridable Sub writeStyleEndTag()
			decrIndent()
			indentSmart()
			write("-->")
			writeLineSeparator()
			decrIndent()
			indentSmart()
			write("</style>")
			writeLineSeparator()
			indentSmart()
		End Sub

		' --- conversion support ---------------------------

		''' <summary>
		''' Convert the give set of attributes to be html for
		''' the purpose of writing them out.  Any keys that
		''' have been converted will not appear in the resultant
		''' set.  Any keys not converted will appear in the
		''' resultant set the same as the received set.<p>
		''' This will put the converted values into <code>to</code>, unless
		''' it is null in which case a temporary AttributeSet will be returned.
		''' </summary>
		Friend Overridable Function convertToHTML(ByVal [from] As AttributeSet, ByVal [to] As MutableAttributeSet) As AttributeSet
			If [to] Is Nothing Then [to] = convAttr
			[to].removeAttributes([to])
			If writeCSS Then
				convertToHTML40([from], [to])
			Else
				convertToHTML32([from], [to])
			End If
			Return [to]
		End Function

		''' <summary>
		''' If true, the writer will emit CSS attributes in preference
		''' to HTML tags/attributes (i.e. It will emit an HTML 4.0
		''' style).
		''' </summary>
		Private writeCSS As Boolean = False

		''' <summary>
		''' Buffer for the purpose of attribute conversion
		''' </summary>
		Private convAttr As MutableAttributeSet = New SimpleAttributeSet

		''' <summary>
		''' Buffer for the purpose of attribute conversion. This can be
		''' used if convAttr is being used.
		''' </summary>
		Private oConvAttr As MutableAttributeSet = New SimpleAttributeSet

		''' <summary>
		''' Create an older style of HTML attributes.  This will
		''' convert character level attributes that have a StyleConstants
		''' mapping over to an HTML tag/attribute.  Other CSS attributes
		''' will be placed in an HTML style attribute.
		''' </summary>
		Private Shared Sub convertToHTML32(ByVal [from] As AttributeSet, ByVal [to] As MutableAttributeSet)
			If [from] Is Nothing Then Return
			Dim keys As System.Collections.IEnumerator = [from].attributeNames
			Dim value As String = ""
			Do While keys.hasMoreElements()
				Dim key As Object = keys.nextElement()
				If TypeOf key Is CSS.Attribute Then
					If (key Is CSS.Attribute.FONT_FAMILY) OrElse (key Is CSS.Attribute.FONT_SIZE) OrElse (key Is CSS.Attribute.COLOR) Then

						createFontAttribute(CType(key, CSS.Attribute), [from], [to])
					ElseIf key Is CSS.Attribute.FONT_WEIGHT Then
						' add a bold tag is weight is bold
						Dim weightValue As CSS.FontWeight = CType([from].getAttribute(CSS.Attribute.FONT_WEIGHT), CSS.FontWeight)
						If (weightValue IsNot Nothing) AndAlso (weightValue.value > 400) Then addAttribute([to], HTML.Tag.B, SimpleAttributeSet.EMPTY)
					ElseIf key Is CSS.Attribute.FONT_STYLE Then
						Dim s As String = [from].getAttribute(key).ToString()
						If s.IndexOf("italic") >= 0 Then addAttribute([to], HTML.Tag.I, SimpleAttributeSet.EMPTY)
					ElseIf key Is CSS.Attribute.TEXT_DECORATION Then
						Dim decor As String = [from].getAttribute(key).ToString()
						If decor.IndexOf("underline") >= 0 Then addAttribute([to], HTML.Tag.U, SimpleAttributeSet.EMPTY)
						If decor.IndexOf("line-through") >= 0 Then addAttribute([to], HTML.Tag.STRIKE, SimpleAttributeSet.EMPTY)
					ElseIf key Is CSS.Attribute.VERTICAL_ALIGN Then
						Dim vAlign As String = [from].getAttribute(key).ToString()
						If vAlign.IndexOf("sup") >= 0 Then addAttribute([to], HTML.Tag.SUP, SimpleAttributeSet.EMPTY)
						If vAlign.IndexOf("sub") >= 0 Then addAttribute([to], HTML.Tag.SUB, SimpleAttributeSet.EMPTY)
					ElseIf key Is CSS.Attribute.TEXT_ALIGN Then
						addAttribute([to], HTML.Attribute.ALIGN, [from].getAttribute(key).ToString())
					Else
						' default is to store in a HTML style attribute
						If value.Length > 0 Then value = value & "; "
						value = value + key & ": " & [from].getAttribute(key)
					End If
				Else
					Dim attr As Object = [from].getAttribute(key)
					If TypeOf attr Is AttributeSet Then attr = CType(attr, AttributeSet).copyAttributes()
					addAttribute([to], key, attr)
				End If
			Loop
			If value.Length > 0 Then [to].addAttribute(HTML.Attribute.STYLE, value)
		End Sub

		''' <summary>
		''' Add an attribute only if it doesn't exist so that we don't
		''' loose information replacing it with SimpleAttributeSet.EMPTY
		''' </summary>
		Private Shared Sub addAttribute(ByVal [to] As MutableAttributeSet, ByVal key As Object, ByVal value As Object)
			Dim attr As Object = [to].getAttribute(key)
			If attr Is Nothing OrElse attr Is SimpleAttributeSet.EMPTY Then
				[to].addAttribute(key, value)
			Else
				If TypeOf attr Is MutableAttributeSet AndAlso TypeOf value Is AttributeSet Then CType(attr, MutableAttributeSet).addAttributes(CType(value, AttributeSet))
			End If
		End Sub

		''' <summary>
		''' Create/update an HTML &lt;font&gt; tag attribute.  The
		''' value of the attribute should be a MutableAttributeSet so
		''' that the attributes can be updated as they are discovered.
		''' </summary>
		Private Shared Sub createFontAttribute(ByVal a As CSS.Attribute, ByVal [from] As AttributeSet, ByVal [to] As MutableAttributeSet)
			Dim fontAttr As MutableAttributeSet = CType([to].getAttribute(HTML.Tag.FONT), MutableAttributeSet)
			If fontAttr Is Nothing Then
				fontAttr = New SimpleAttributeSet
				[to].addAttribute(HTML.Tag.FONT, fontAttr)
			End If
			' edit the parameters to the font tag
			Dim htmlValue As String = [from].getAttribute(a).ToString()
			If a Is CSS.Attribute.FONT_FAMILY Then
				fontAttr.addAttribute(HTML.Attribute.FACE, htmlValue)
			ElseIf a Is CSS.Attribute.FONT_SIZE Then
				fontAttr.addAttribute(HTML.Attribute.SIZE, htmlValue)
			ElseIf a Is CSS.Attribute.COLOR Then
				fontAttr.addAttribute(HTML.Attribute.COLOR, htmlValue)
			End If
		End Sub

		''' <summary>
		''' Copies the given AttributeSet to a new set, converting
		''' any CSS attributes found to arguments of an HTML style
		''' attribute.
		''' </summary>
		Private Shared Sub convertToHTML40(ByVal [from] As AttributeSet, ByVal [to] As MutableAttributeSet)
			Dim keys As System.Collections.IEnumerator = [from].attributeNames
			Dim value As String = ""
			Do While keys.hasMoreElements()
				Dim key As Object = keys.nextElement()
				If TypeOf key Is CSS.Attribute Then
					value = value & " " & key & "=" & [from].getAttribute(key) & ";"
				Else
					[to].addAttribute(key, [from].getAttribute(key))
				End If
			Loop
			If value.Length > 0 Then [to].addAttribute(HTML.Attribute.STYLE, value)
		End Sub

		'
		' Overrides the writing methods to only break a string when
		' canBreakString is true.
		' In a future release it is likely AbstractWriter will get this
		' functionality.
		'

		''' <summary>
		''' Writes the line separator. This is overriden to make sure we don't
		''' replace the newline content in case it is outside normal ascii.
		''' @since 1.3
		''' </summary>
		Protected Friend Overrides Sub writeLineSeparator()
			Dim oldReplace As Boolean = replaceEntities
			replaceEntities = False
			MyBase.writeLineSeparator()
			replaceEntities = oldReplace
			indented = False
		End Sub

		''' <summary>
		''' This method is overriden to map any character entities, such as
		''' &lt; to &amp;lt;. <code>super.output</code> will be invoked to
		''' write the content.
		''' @since 1.3
		''' </summary>
		Protected Friend Overrides Sub output(ByVal chars As Char(), ByVal start As Integer, ByVal length As Integer)
			If Not replaceEntities Then
				MyBase.output(chars, start, length)
				Return
			End If
			Dim last As Integer = start
			length += start
			For counter As Integer = start To length - 1
				' This will change, we need better support character level
				' entities.
				Select Case chars(counter)
					' Character level entities.
				Case "<"c
					If counter > last Then MyBase.output(chars, last, counter - last)
					last = counter + 1
					output("&lt;")
				Case ">"c
					If counter > last Then MyBase.output(chars, last, counter - last)
					last = counter + 1
					output("&gt;")
				Case "&"c
					If counter > last Then MyBase.output(chars, last, counter - last)
					last = counter + 1
					output("&amp;")
				Case """"c
					If counter > last Then MyBase.output(chars, last, counter - last)
					last = counter + 1
					output("&quot;")
					' Special characters
				Case ControlChars.Lf, ControlChars.Tab, ControlChars.Cr
				Case Else
					If chars(counter) < " "c OrElse AscW(chars(counter)) > 127 Then
						If counter > last Then MyBase.output(chars, last, counter - last)
						last = counter + 1
						' If the character is outside of ascii, write the
						' numeric value.
						output("&#")
						output(Convert.ToString(AscW(chars(counter))))
						output(";")
					End If
				End Select
			Next counter
			If last < length Then MyBase.output(chars, last, length - last)
		End Sub

		''' <summary>
		''' This directly invokes super's <code>output</code> after converting
		''' <code>string</code> to a char[].
		''' </summary>
		Private Sub output(ByVal [string] As String)
			Dim length As Integer = [string].Length
			If tempChars Is Nothing OrElse tempChars.Length < length Then tempChars = New Char(length - 1){}
			[string].getChars(0, length, tempChars, 0)
			MyBase.output(tempChars, 0, length)
		End Sub

		Private indented As Boolean = False

		''' <summary>
		''' Writes indent only once per line.
		''' </summary>
		Private Sub indentSmart()
			If Not indented Then
				indent()
				indented = True
			End If
		End Sub
	End Class

End Namespace