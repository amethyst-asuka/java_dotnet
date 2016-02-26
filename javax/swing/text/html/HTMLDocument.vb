Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.text
Imports javax.swing.undo
import static sun.swing.SwingUtilities2.IMPLIED_CR

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A document that models HTML.  The purpose of this model is to
	''' support both browsing and editing.  As a result, the structure
	''' described by an HTML document is not exactly replicated by default.
	''' The element structure that is modeled by default, is built by the
	''' class <code>HTMLDocument.HTMLReader</code>, which implements the
	''' <code>HTMLEditorKit.ParserCallback</code> protocol that the parser
	''' expects.  To change the structure one can subclass
	''' <code>HTMLReader</code>, and reimplement the method {@link
	''' #getReader(int)} to return the new reader implementation.  The
	''' documentation for <code>HTMLReader</code> should be consulted for
	''' the details of the default structure created.  The intent is that
	''' the document be non-lossy (although reproducing the HTML format may
	''' result in a different format).
	''' 
	''' <p>The document models only HTML, and makes no attempt to store
	''' view attributes in it.  The elements are identified by the
	''' <code>StyleContext.NameAttribute</code> attribute, which should
	''' always have a value of type <code>HTML.Tag</code> that identifies
	''' the kind of element.  Some of the elements (such as comments) are
	''' synthesized.  The <code>HTMLFactory</code> uses this attribute to
	''' determine what kind of view to build.</p>
	''' 
	''' <p>This document supports incremental loading.  The
	''' <code>TokenThreshold</code> property controls how much of the parse
	''' is buffered before trying to update the element structure of the
	''' document.  This property is set by the <code>EditorKit</code> so
	''' that subclasses can disable it.</p>
	''' 
	''' <p>The <code>Base</code> property determines the URL against which
	''' relative URLs are resolved.  By default, this will be the
	''' <code>Document.StreamDescriptionProperty</code> if the value of the
	''' property is a URL.  If a &lt;BASE&gt; tag is encountered, the base
	''' will become the URL specified by that tag.  Because the base URL is
	''' a property, it can of course be set directly.</p>
	''' 
	''' <p>The default content storage mechanism for this document is a gap
	''' buffer (<code>GapContent</code>).  Alternatives can be supplied by
	''' using the constructor that takes a <code>Content</code>
	''' implementation.</p>
	''' 
	''' <h2>Modifying HTMLDocument</h2>
	''' 
	''' <p>In addition to the methods provided by Document and
	''' StyledDocument for mutating an HTMLDocument, HTMLDocument provides
	''' a number of convenience methods.  The following methods can be used
	''' to insert HTML content into an existing document.</p>
	''' 
	''' <ul>
	'''   <li><seealso cref="#setInnerHTML(Element, String)"/></li>
	'''   <li><seealso cref="#setOuterHTML(Element, String)"/></li>
	'''   <li><seealso cref="#insertBeforeStart(Element, String)"/></li>
	'''   <li><seealso cref="#insertAfterStart(Element, String)"/></li>
	'''   <li><seealso cref="#insertBeforeEnd(Element, String)"/></li>
	'''   <li><seealso cref="#insertAfterEnd(Element, String)"/></li>
	''' </ul>
	''' 
	''' <p>The following examples illustrate using these methods.  Each
	''' example assumes the HTML document is initialized in the following
	''' way:</p>
	''' 
	''' <pre>
	''' JEditorPane p = new JEditorPane();
	''' p.setContentType("text/html");
	''' p.setText("..."); // Document text is provided below.
	''' HTMLDocument d = (HTMLDocument) p.getDocument();
	''' </pre>
	''' 
	''' <p>With the following HTML content:</p>
	''' 
	''' <pre>
	''' &lt;html&gt;
	'''   &lt;head&gt;
	'''     &lt;title&gt;An example HTMLDocument&lt;/title&gt;
	'''     &lt;style type="text/css"&gt;
	'''       div { background-color: silver; }
	'''       ul { color: red; }
	'''     &lt;/style&gt;
	'''   &lt;/head&gt;
	'''   &lt;body&gt;
	'''     &lt;div id="BOX"&gt;
	'''       &lt;p&gt;Paragraph 1&lt;/p&gt;
	'''       &lt;p&gt;Paragraph 2&lt;/p&gt;
	'''     &lt;/div&gt;
	'''   &lt;/body&gt;
	''' &lt;/html&gt;
	''' </pre>
	''' 
	''' <p>All the methods for modifying an HTML document require an {@link
	''' Element}.  Elements can be obtained from an HTML document by using
	''' the method {@link #getElement(Element e, Object attribute, Object
	''' value)}.  It returns the first descendant element that contains the
	''' specified attribute with the given value, in depth-first order.
	''' For example, <code>d.getElement(d.getDefaultRootElement(),
	''' StyleConstants.NameAttribute, HTML.Tag.P)</code> returns the first
	''' paragraph element.</p>
	''' 
	''' <p>A convenient shortcut for locating elements is the method {@link
	''' #getElement(String)}; returns an element whose <code>ID</code>
	''' attribute matches the specified value.  For example,
	''' <code>d.getElement("BOX")</code> returns the <code>DIV</code>
	''' element.</p>
	''' 
	''' <p>The <seealso cref="#getIterator(HTML.Tag t)"/> method can also be used for
	''' finding all occurrences of the specified HTML tag in the
	''' document.</p>
	''' 
	''' <h3>Inserting elements</h3>
	''' 
	''' <p>Elements can be inserted before or after the existing children
	''' of any non-leaf element by using the methods
	''' <code>insertAfterStart</code> and <code>insertBeforeEnd</code>.
	''' For example, if <code>e</code> is the <code>DIV</code> element,
	''' <code>d.insertAfterStart(e, "&lt;ul&gt;&lt;li&gt;List
	''' Item&lt;/li&gt;&lt;/ul&gt;")</code> inserts the list before the first
	''' paragraph, and <code>d.insertBeforeEnd(e, "&lt;ul&gt;&lt;li&gt;List
	''' Item&lt;/li&gt;&lt;/ul&gt;")</code> inserts the list after the last
	''' paragraph.  The <code>DIV</code> block becomes the parent of the
	''' newly inserted elements.</p>
	''' 
	''' <p>Sibling elements can be inserted before or after any element by
	''' using the methods <code>insertBeforeStart</code> and
	''' <code>insertAfterEnd</code>.  For example, if <code>e</code> is the
	''' <code>DIV</code> element, <code>d.insertBeforeStart(e,
	''' "&lt;ul&gt;&lt;li&gt;List Item&lt;/li&gt;&lt;/ul&gt;")</code> inserts the list
	''' before the <code>DIV</code> element, and <code>d.insertAfterEnd(e,
	''' "&lt;ul&gt;&lt;li&gt;List Item&lt;/li&gt;&lt;/ul&gt;")</code> inserts the list
	''' after the <code>DIV</code> element.  The newly inserted elements
	''' become siblings of the <code>DIV</code> element.</p>
	''' 
	''' <h3>Replacing elements</h3>
	''' 
	''' <p>Elements and all their descendants can be replaced by using the
	''' methods <code>setInnerHTML</code> and <code>setOuterHTML</code>.
	''' For example, if <code>e</code> is the <code>DIV</code> element,
	''' <code>d.setInnerHTML(e, "&lt;ul&gt;&lt;li&gt;List
	''' Item&lt;/li&gt;&lt;/ul&gt;")</code> replaces all children paragraphs with
	''' the list, and <code>d.setOuterHTML(e, "&lt;ul&gt;&lt;li&gt;List
	''' Item&lt;/li&gt;&lt;/ul&gt;")</code> replaces the <code>DIV</code> element
	''' itself.  In latter case the parent of the list is the
	''' <code>BODY</code> element.
	''' 
	''' <h3>Summary</h3>
	''' 
	''' <p>The following table shows the example document and the results
	''' of various methods described above.</p>
	''' 
	''' <table border=1 cellspacing=0>
	'''   <tr>
	'''     <th>Example</th>
	'''     <th><code>insertAfterStart</code></th>
	'''     <th><code>insertBeforeEnd</code></th>
	'''     <th><code>insertBeforeStart</code></th>
	'''     <th><code>insertAfterEnd</code></th>
	'''     <th><code>setInnerHTML</code></th>
	'''     <th><code>setOuterHTML</code></th>
	'''   </tr>
	'''   <tr valign="top">
	'''     <td style="white-space:nowrap">
	'''       <div style="background-color: silver;">
	'''         <p>Paragraph 1</p>
	'''         <p>Paragraph 2</p>
	'''       </div>
	'''     </td>
	''' <!--insertAfterStart-->
	'''     <td style="white-space:nowrap">
	'''       <div style="background-color: silver;">
	'''         <ul style="color: red;">
	'''           <li>List Item</li>
	'''         </ul>
	'''         <p>Paragraph 1</p>
	'''         <p>Paragraph 2</p>
	'''       </div>
	'''     </td>
	''' <!--insertBeforeEnd-->
	'''     <td style="white-space:nowrap">
	'''       <div style="background-color: silver;">
	'''         <p>Paragraph 1</p>
	'''         <p>Paragraph 2</p>
	'''         <ul style="color: red;">
	'''           <li>List Item</li>
	'''         </ul>
	'''       </div>
	'''     </td>
	''' <!--insertBeforeStart-->
	'''     <td style="white-space:nowrap">
	'''       <ul style="color: red;">
	'''         <li>List Item</li>
	'''       </ul>
	'''       <div style="background-color: silver;">
	'''         <p>Paragraph 1</p>
	'''         <p>Paragraph 2</p>
	'''       </div>
	'''     </td>
	''' <!--insertAfterEnd-->
	'''     <td style="white-space:nowrap">
	'''       <div style="background-color: silver;">
	'''         <p>Paragraph 1</p>
	'''         <p>Paragraph 2</p>
	'''       </div>
	'''       <ul style="color: red;">
	'''         <li>List Item</li>
	'''       </ul>
	'''     </td>
	''' <!--setInnerHTML-->
	'''     <td style="white-space:nowrap">
	'''       <div style="background-color: silver;">
	'''         <ul style="color: red;">
	'''           <li>List Item</li>
	'''         </ul>
	'''       </div>
	'''     </td>
	''' <!--setOuterHTML-->
	'''     <td style="white-space:nowrap">
	'''       <ul style="color: red;">
	'''         <li>List Item</li>
	'''       </ul>
	'''     </td>
	'''   </tr>
	''' </table>
	''' 
	''' <p><strong>Warning:</strong> Serialized objects of this class will
	''' not be compatible with future Swing releases. The current
	''' serialization support is appropriate for short term storage or RMI
	''' between applications running the same version of Swing.  As of 1.4,
	''' support for long term storage of all JavaBeans&trade;
	''' has been added to the
	''' <code>java.beans</code> package.  Please see {@link
	''' java.beans.XMLEncoder}.</p>
	''' 
	''' @author  Timothy Prinzing
	''' @author  Scott Violet
	''' @author  Sunita Mani
	''' </summary>
	Public Class HTMLDocument
		Inherits DefaultStyledDocument

		''' <summary>
		''' Constructs an HTML document using the default buffer size
		''' and a default <code>StyleSheet</code>.  This is a convenience
		''' method for the constructor
		''' <code>HTMLDocument(Content, StyleSheet)</code>.
		''' </summary>
		Public Sub New()
			Me.New(New GapContent(BUFFER_SIZE_DEFAULT), New StyleSheet)
		End Sub

		''' <summary>
		''' Constructs an HTML document with the default content
		''' storage implementation and the specified style/attribute
		''' storage mechanism.  This is a convenience method for the
		''' constructor
		''' <code>HTMLDocument(Content, StyleSheet)</code>.
		''' </summary>
		''' <param name="styles">  the styles </param>
		Public Sub New(ByVal styles As StyleSheet)
			Me.New(New GapContent(BUFFER_SIZE_DEFAULT), styles)
		End Sub

		''' <summary>
		''' Constructs an HTML document with the given content
		''' storage implementation and the given style/attribute
		''' storage mechanism.
		''' </summary>
		''' <param name="c">  the container for the content </param>
		''' <param name="styles"> the styles </param>
		Public Sub New(ByVal c As Content, ByVal styles As StyleSheet)
			MyBase.New(c, styles)
		End Sub

		''' <summary>
		''' Fetches the reader for the parser to use when loading the document
		''' with HTML.  This is implemented to return an instance of
		''' <code>HTMLDocument.HTMLReader</code>.
		''' Subclasses can reimplement this
		''' method to change how the document gets structured if desired.
		''' (For example, to handle custom tags, or structurally represent character
		''' style elements.)
		''' </summary>
		''' <param name="pos"> the starting position </param>
		''' <returns> the reader used by the parser to load the document </returns>
		Public Overridable Function getReader(ByVal pos As Integer) As HTMLEditorKit.ParserCallback
			Dim desc As Object = getProperty(Document.StreamDescriptionProperty)
			If TypeOf desc Is java.net.URL Then base = CType(desc, java.net.URL)
			Dim ___reader As New HTMLReader(Me, pos)
			Return ___reader
		End Function

		''' <summary>
		''' Returns the reader for the parser to use to load the document
		''' with HTML.  This is implemented to return an instance of
		''' <code>HTMLDocument.HTMLReader</code>.
		''' Subclasses can reimplement this
		''' method to change how the document gets structured if desired.
		''' (For example, to handle custom tags, or structurally represent character
		''' style elements.)
		''' <p>This is a convenience method for
		''' <code>getReader(int, int, int, HTML.Tag, TRUE)</code>.
		''' </summary>
		''' <param name="popDepth">   the number of <code>ElementSpec.EndTagTypes</code>
		'''          to generate before inserting </param>
		''' <param name="pushDepth">  the number of <code>ElementSpec.StartTagTypes</code>
		'''          with a direction of <code>ElementSpec.JoinNextDirection</code>
		'''          that should be generated before inserting,
		'''          but after the end tags have been generated </param>
		''' <param name="insertTag">  the first tag to start inserting into document </param>
		''' <returns> the reader used by the parser to load the document </returns>
		Public Overridable Function getReader(ByVal pos As Integer, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal insertTag As HTML.Tag) As HTMLEditorKit.ParserCallback
			Return getReader(pos, popDepth, pushDepth, insertTag, True)
		End Function

		''' <summary>
		''' Fetches the reader for the parser to use to load the document
		''' with HTML.  This is implemented to return an instance of
		''' HTMLDocument.HTMLReader.  Subclasses can reimplement this
		''' method to change how the document get structured if desired
		''' (e.g. to handle custom tags, structurally represent character
		''' style elements, etc.).
		''' </summary>
		''' <param name="popDepth">   the number of <code>ElementSpec.EndTagTypes</code>
		'''          to generate before inserting </param>
		''' <param name="pushDepth">  the number of <code>ElementSpec.StartTagTypes</code>
		'''          with a direction of <code>ElementSpec.JoinNextDirection</code>
		'''          that should be generated before inserting,
		'''          but after the end tags have been generated </param>
		''' <param name="insertTag">  the first tag to start inserting into document </param>
		''' <param name="insertInsertTag">  false if all the Elements after insertTag should
		'''        be inserted; otherwise insertTag will be inserted </param>
		''' <returns> the reader used by the parser to load the document </returns>
		Friend Overridable Function getReader(ByVal pos As Integer, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal insertTag As HTML.Tag, ByVal insertInsertTag As Boolean) As HTMLEditorKit.ParserCallback
			Dim desc As Object = getProperty(Document.StreamDescriptionProperty)
			If TypeOf desc Is java.net.URL Then base = CType(desc, java.net.URL)
			Dim ___reader As New HTMLReader(Me, pos, popDepth, pushDepth, insertTag, insertInsertTag, False, True)
			Return ___reader
		End Function

		''' <summary>
		''' Returns the location to resolve relative URLs against.  By
		''' default this will be the document's URL if the document
		''' was loaded from a URL.  If a base tag is found and
		''' can be parsed, it will be used as the base location.
		''' </summary>
		''' <returns> the base location </returns>
		Public Overridable Property base As java.net.URL
			Get
				Return base
			End Get
			Set(ByVal u As java.net.URL)
				base = u
				styleSheet.base = u
			End Set
		End Property


		''' <summary>
		''' Inserts new elements in bulk.  This is how elements get created
		''' in the document.  The parsing determines what structure is needed
		''' and creates the specification as a set of tokens that describe the
		''' edit while leaving the document free of a write-lock.  This method
		''' can then be called in bursts by the reader to acquire a write-lock
		''' for a shorter duration (i.e. while the document is actually being
		''' altered).
		''' </summary>
		''' <param name="offset"> the starting offset </param>
		''' <param name="data"> the element data </param>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document. </exception>
		Protected Friend Overrides Sub insert(ByVal offset As Integer, ByVal data As ElementSpec())
			MyBase.insert(offset, data)
		End Sub

		''' <summary>
		''' Updates document structure as a result of text insertion.  This
		''' will happen within a write lock.  This implementation simply
		''' parses the inserted content for line breaks and builds up a set
		''' of instructions for the element buffer.
		''' </summary>
		''' <param name="chng"> a description of the document change </param>
		''' <param name="attr"> the attributes </param>
		Protected Friend Overrides Sub insertUpdate(ByVal chng As DefaultDocumentEvent, ByVal attr As AttributeSet)
			If attr Is Nothing Then
				attr = contentAttributeSet

			' If this is the composed text element, merge the content attribute to it
			ElseIf attr.isDefined(StyleConstants.ComposedTextAttribute) Then
				CType(attr, MutableAttributeSet).addAttributes(contentAttributeSet)
			End If

			If attr.isDefined(IMPLIED_CR) Then CType(attr, MutableAttributeSet).removeAttribute(IMPLIED_CR)

			MyBase.insertUpdate(chng, attr)
		End Sub

		''' <summary>
		''' Replaces the contents of the document with the given
		''' element specifications.  This is called before insert if
		''' the loading is done in bursts.  This is the only method called
		''' if loading the document entirely in one burst.
		''' </summary>
		''' <param name="data">  the new contents of the document </param>
		Protected Friend Overrides Sub create(ByVal data As ElementSpec())
			MyBase.create(data)
		End Sub

		''' <summary>
		''' Sets attributes for a paragraph.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offset"> the offset into the paragraph (must be at least 0) </param>
		''' <param name="length"> the number of characters affected (must be at least 0) </param>
		''' <param name="s"> the attributes </param>
		''' <param name="replace"> whether to replace existing attributes, or merge them </param>
		Public Overrides Sub setParagraphAttributes(ByVal offset As Integer, ByVal length As Integer, ByVal s As AttributeSet, ByVal replace As Boolean)
			Try
				writeLock()
				' Make sure we send out a change for the length of the paragraph.
				Dim [end] As Integer = Math.Min(offset + length, length)
				Dim e As Element = getParagraphElement(offset)
				offset = e.startOffset
				e = getParagraphElement([end])
				length = Math.Max(0, e.endOffset - offset)
				Dim changes As New DefaultDocumentEvent(offset, length, DocumentEvent.EventType.CHANGE)
				Dim sCopy As AttributeSet = s.copyAttributes()
				Dim lastEnd As Integer = Integer.MaxValue
				Dim pos As Integer = offset
				Do While pos <= [end]
					Dim paragraph As Element = getParagraphElement(pos)
					If lastEnd = paragraph.endOffset Then
						lastEnd += 1
					Else
						lastEnd = paragraph.endOffset
					End If
					Dim attr As MutableAttributeSet = CType(paragraph.attributes, MutableAttributeSet)
					changes.addEdit(New AttributeUndoableEdit(paragraph, sCopy, replace))
					If replace Then attr.removeAttributes(attr)
					attr.addAttributes(s)
					pos = lastEnd
				Loop
				changes.end()
				fireChangedUpdate(changes)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, changes))
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Fetches the <code>StyleSheet</code> with the document-specific display
		''' rules (CSS) that were specified in the HTML document itself.
		''' </summary>
		''' <returns> the <code>StyleSheet</code> </returns>
		Public Overridable Property styleSheet As StyleSheet
			Get
				Return CType(attributeContext, StyleSheet)
			End Get
		End Property

		''' <summary>
		''' Fetches an iterator for the specified HTML tag.
		''' This can be used for things like iterating over the
		''' set of anchors contained, or iterating over the input
		''' elements.
		''' </summary>
		''' <param name="t"> the requested <code>HTML.Tag</code> </param>
		''' <returns> the <code>Iterator</code> for the given HTML tag </returns>
		''' <seealso cref= javax.swing.text.html.HTML.Tag </seealso>
		Public Overridable Function getIterator(ByVal t As HTML.Tag) As [Iterator]
			If t.block Then Return Nothing
			Return New LeafIterator(t, Me)
		End Function

		''' <summary>
		''' Creates a document leaf element that directly represents
		''' text (doesn't have any children).  This is implemented
		''' to return an element of type
		''' <code>HTMLDocument.RunElement</code>.
		''' </summary>
		''' <param name="parent"> the parent element </param>
		''' <param name="a"> the attributes for the element </param>
		''' <param name="p0"> the beginning of the range (must be at least 0) </param>
		''' <param name="p1"> the end of the range (must be at least p0) </param>
		''' <returns> the new element </returns>
		Protected Friend Overrides Function createLeafElement(ByVal parent As Element, ByVal a As AttributeSet, ByVal p0 As Integer, ByVal p1 As Integer) As Element
			Return New RunElement(Me, parent, a, p0, p1)
		End Function

		''' <summary>
		''' Creates a document branch element, that can contain other elements.
		''' This is implemented to return an element of type
		''' <code>HTMLDocument.BlockElement</code>.
		''' </summary>
		''' <param name="parent"> the parent element </param>
		''' <param name="a"> the attributes </param>
		''' <returns> the element </returns>
		Protected Friend Overrides Function createBranchElement(ByVal parent As Element, ByVal a As AttributeSet) As Element
			Return New BlockElement(Me, parent, a)
		End Function

		''' <summary>
		''' Creates the root element to be used to represent the
		''' default document structure.
		''' </summary>
		''' <returns> the element base </returns>
		Protected Friend Overrides Function createDefaultRoot() As AbstractElement
			' grabs a write-lock for this initialization and
			' abandon it during initialization so in normal
			' operation we can detect an illegitimate attempt
			' to mutate attributes.
			writeLock()
			Dim a As MutableAttributeSet = New SimpleAttributeSet
			a.addAttribute(StyleConstants.NameAttribute, HTML.Tag.HTML)
			Dim html As New BlockElement(Me, Nothing, a.copyAttributes())
			a.removeAttributes(a)
			a.addAttribute(StyleConstants.NameAttribute, HTML.Tag.BODY)
			Dim body As New BlockElement(Me, html, a.copyAttributes())
			a.removeAttributes(a)
			a.addAttribute(StyleConstants.NameAttribute, HTML.Tag.P)
			styleSheet.addCSSAttributeFromHTML(a, CSS.Attribute.MARGIN_TOP, "0")
			Dim paragraph As New BlockElement(Me, body, a.copyAttributes())
			a.removeAttributes(a)
			a.addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
			Dim brk As New RunElement(Me, paragraph, a, 0, 1)
			Dim buff As Element() = New Element(0){}
			buff(0) = brk
			paragraph.replace(0, 0, buff)
			buff(0) = paragraph
			body.replace(0, 0, buff)
			buff(0) = body
			html.replace(0, 0, buff)
			writeUnlock()
			Return html
		End Function

		''' <summary>
		''' Sets the number of tokens to buffer before trying to update
		''' the documents element structure.
		''' </summary>
		''' <param name="n">  the number of tokens to buffer </param>
		Public Overridable Property tokenThreshold As Integer
			Set(ByVal n As Integer)
				putProperty(TokenThreshold, New Integer?(n))
			End Set
			Get
				Dim i As Integer? = CInt(Fix(getProperty(TokenThreshold)))
				If i IsNot Nothing Then Return i
				Return Integer.MaxValue
			End Get
		End Property


		''' <summary>
		''' Determines how unknown tags are handled by the parser.
		''' If set to true, unknown
		''' tags are put in the model, otherwise they are dropped.
		''' </summary>
		''' <param name="preservesTags">  true if unknown tags should be
		'''          saved in the model, otherwise tags are dropped </param>
		''' <seealso cref= javax.swing.text.html.HTML.Tag </seealso>
		Public Overridable Property preservesUnknownTags As Boolean
			Set(ByVal preservesTags As Boolean)
				preservesUnknownTags = preservesTags
			End Set
			Get
				Return preservesUnknownTags
			End Get
		End Property


		''' <summary>
		''' Processes <code>HyperlinkEvents</code> that
		''' are generated by documents in an HTML frame.
		''' The <code>HyperlinkEvent</code> type, as the parameter suggests,
		''' is <code>HTMLFrameHyperlinkEvent</code>.
		''' In addition to the typical information contained in a
		''' <code>HyperlinkEvent</code>,
		''' this event contains the element that corresponds to the frame in
		''' which the click happened (the source element) and the
		''' target name.  The target name has 4 possible values:
		''' <ul>
		''' <li>  _self
		''' <li>  _parent
		''' <li>  _top
		''' <li>  a named frame
		''' </ul>
		''' 
		''' If target is _self, the action is to change the value of the
		''' <code>HTML.Attribute.SRC</code> attribute and fires a
		''' <code>ChangedUpdate</code> event.
		''' <p>
		''' If the target is _parent, then it deletes the parent element,
		''' which is a &lt;FRAMESET&gt; element, and inserts a new &lt;FRAME&gt;
		''' element, and sets its <code>HTML.Attribute.SRC</code> attribute
		''' to have a value equal to the destination URL and fire a
		''' <code>RemovedUpdate</code> and <code>InsertUpdate</code>.
		''' <p>
		''' If the target is _top, this method does nothing. In the implementation
		''' of the view for a frame, namely the <code>FrameView</code>,
		''' the processing of _top is handled.  Given that _top implies
		''' replacing the entire document, it made sense to handle this outside
		''' of the document that it will replace.
		''' <p>
		''' If the target is a named frame, then the element hierarchy is searched
		''' for an element with a name equal to the target, its
		''' <code>HTML.Attribute.SRC</code> attribute is updated and a
		''' <code>ChangedUpdate</code> event is fired.
		''' </summary>
		''' <param name="e"> the event </param>
		Public Overridable Sub processHTMLFrameHyperlinkEvent(ByVal e As HTMLFrameHyperlinkEvent)
			Dim frameName As String = e.target
			Dim ___element As Element = e.sourceElement
			Dim urlStr As String = e.uRL.ToString()

			If frameName.Equals("_self") Then
	'            
	'              The source and destination elements
	'              are the same.
	'            
				updateFrame(___element, urlStr)
			ElseIf frameName.Equals("_parent") Then
	'            
	'              The destination is the parent of the frame.
	'            
				updateFrameSet(___element.parentElement, urlStr)
			Else
	'            
	'              locate a named frame
	'            
				Dim targetElement As Element = findFrame(frameName)
				If targetElement IsNot Nothing Then updateFrame(targetElement, urlStr)
			End If
		End Sub


		''' <summary>
		''' Searches the element hierarchy for an FRAME element
		''' that has its name attribute equal to the <code>frameName</code>.
		''' </summary>
		''' <param name="frameName"> </param>
		''' <returns> the element whose NAME attribute has a value of
		'''          <code>frameName</code>; returns <code>null</code>
		'''          if not found </returns>
		Private Function findFrame(ByVal frameName As String) As Element
			Dim it As New ElementIterator(Me)
			Dim [next] As Element

			[next] = it.next()
			Do While [next] IsNot Nothing
				Dim attr As AttributeSet = [next].attributes
				If matchNameAttribute(attr, HTML.Tag.FRAME) Then
					Dim frameTarget As String = CStr(attr.getAttribute(HTML.Attribute.NAME))
					If frameTarget IsNot Nothing AndAlso frameTarget.Equals(frameName) Then Exit Do
				End If
				[next] = it.next()
			Loop
			Return [next]
		End Function

		''' <summary>
		''' Returns true if <code>StyleConstants.NameAttribute</code> is
		''' equal to the tag that is passed in as a parameter.
		''' </summary>
		''' <param name="attr"> the attributes to be matched </param>
		''' <param name="tag"> the value to be matched </param>
		''' <returns> true if there is a match, false otherwise </returns>
		''' <seealso cref= javax.swing.text.html.HTML.Attribute </seealso>
		Friend Shared Function matchNameAttribute(ByVal attr As AttributeSet, ByVal tag As HTML.Tag) As Boolean
			Dim o As Object = attr.getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim name As HTML.Tag = CType(o, HTML.Tag)
				If name Is tag Then Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Replaces a frameset branch Element with a frame leaf element.
		''' </summary>
		''' <param name="element"> the frameset element to remove </param>
		''' <param name="url">     the value for the SRC attribute for the
		'''                new frame that will replace the frameset </param>
		Private Sub updateFrameSet(ByVal element As Element, ByVal url As String)
			Try
				Dim startOffset As Integer = element.startOffset
				Dim endOffset As Integer = Math.Min(length, element.endOffset)
				Dim html As String = "<frame"
				If url IsNot Nothing Then html &= " src=""" & url & """"
				html &= ">"
				installParserIfNecessary()
				outerHTMLTML(element, html)
			Catch e1 As BadLocationException
				' Should handle this better
			Catch ioe As IOException
				' Should handle this better
			End Try
		End Sub


		''' <summary>
		''' Updates the Frame elements <code>HTML.Attribute.SRC attribute</code>
		''' and fires a <code>ChangedUpdate</code> event.
		''' </summary>
		''' <param name="element"> a FRAME element whose SRC attribute will be updated </param>
		''' <param name="url">     a string specifying the new value for the SRC attribute </param>
		Private Sub updateFrame(ByVal element As Element, ByVal url As String)

			Try
				writeLock()
				Dim changes As New DefaultDocumentEvent(element.startOffset, 1, DocumentEvent.EventType.CHANGE)
				Dim sCopy As AttributeSet = element.attributes.copyAttributes()
				Dim attr As MutableAttributeSet = CType(element.attributes, MutableAttributeSet)
				changes.addEdit(New AttributeUndoableEdit(element, sCopy, False))
				attr.removeAttribute(HTML.Attribute.SRC)
				attr.addAttribute(HTML.Attribute.SRC, url)
				changes.end()
				fireChangedUpdate(changes)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, changes))
			Finally
				writeUnlock()
			End Try
		End Sub


		''' <summary>
		''' Returns true if the document will be viewed in a frame. </summary>
		''' <returns> true if document will be viewed in a frame, otherwise false </returns>
		Friend Overridable Property frameDocument As Boolean
			Get
				Return frameDocument
			End Get
		End Property

		''' <summary>
		''' Sets a boolean state about whether the document will be
		''' viewed in a frame. </summary>
		''' <param name="frameDoc">  true if the document will be viewed in a frame,
		'''          otherwise false </param>
		Friend Overridable Property frameDocumentState As Boolean
			Set(ByVal frameDoc As Boolean)
				Me.frameDocument = frameDoc
			End Set
		End Property

		''' <summary>
		''' Adds the specified map, this will remove a Map that has been
		''' previously registered with the same name.
		''' </summary>
		''' <param name="map">  the <code>Map</code> to be registered </param>
		Friend Overridable Sub addMap(ByVal ___map As Map)
			Dim name As String = ___map.name

			If name IsNot Nothing Then
				Dim ___maps As Object = getProperty(MAP_PROPERTY)

				If ___maps Is Nothing Then
					___maps = New Hashtable(11)
					putProperty(MAP_PROPERTY, ___maps)
				End If
				If TypeOf ___maps Is Hashtable Then CType(___maps, Hashtable)("#" & name) = ___map
			End If
		End Sub

		''' <summary>
		''' Removes a previously registered map. </summary>
		''' <param name="map"> the <code>Map</code> to be removed </param>
		Friend Overridable Sub removeMap(ByVal ___map As Map)
			Dim name As String = ___map.name

			If name IsNot Nothing Then
				Dim ___maps As Object = getProperty(MAP_PROPERTY)

				If TypeOf ___maps Is Hashtable Then CType(___maps, Hashtable).Remove("#" & name)
			End If
		End Sub

		''' <summary>
		''' Returns the Map associated with the given name. </summary>
		''' <param name="name"> the name of the desired <code>Map</code> </param>
		''' <returns> the <code>Map</code> or <code>null</code> if it can't
		'''          be found, or if <code>name</code> is <code>null</code> </returns>
		Friend Overridable Function getMap(ByVal name As String) As Map
			If name IsNot Nothing Then
				Dim ___maps As Object = getProperty(MAP_PROPERTY)

				If ___maps IsNot Nothing AndAlso (TypeOf ___maps Is Hashtable) Then Return CType(CType(___maps, Hashtable)(name), Map)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns an <code>Enumeration</code> of the possible Maps. </summary>
		''' <returns> the enumerated list of maps, or <code>null</code>
		'''          if the maps are not an instance of <code>Hashtable</code> </returns>
		Friend Overridable Property maps As System.Collections.IEnumerator
			Get
				Dim ___maps As Object = getProperty(MAP_PROPERTY)
    
				If TypeOf ___maps Is Hashtable Then Return CType(___maps, Hashtable).Values.GetEnumerator()
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Sets the content type language used for style sheets that do not
		''' explicitly specify the type. The default is text/css. </summary>
		''' <param name="contentType">  the content type language for the style sheets </param>
		' public 
		Friend Overridable Property defaultStyleSheetType As String
			Set(ByVal contentType As String)
				putProperty(StyleType, contentType)
			End Set
			Get
				Dim retValue As String = CStr(getProperty(StyleType))
				If retValue Is Nothing Then Return "text/css"
				Return retValue
			End Get
		End Property

		''' <summary>
		''' Returns the content type language used for style sheets. The default
		''' is text/css. </summary>
		''' <returns> the content type language used for the style sheets </returns>
		' public 

		''' <summary>
		''' Sets the parser that is used by the methods that insert html
		''' into the existing document, such as <code>setInnerHTML</code>,
		''' and <code>setOuterHTML</code>.
		''' <p>
		''' <code>HTMLEditorKit.createDefaultDocument</code> will set the parser
		''' for you. If you create an <code>HTMLDocument</code> by hand,
		''' be sure and set the parser accordingly. </summary>
		''' <param name="parser"> the parser to be used for text insertion
		''' 
		''' @since 1.3 </param>
		Public Overridable Property parser As HTMLEditorKit.Parser
			Set(ByVal parser As HTMLEditorKit.Parser)
				Me.parser = parser
				putProperty("__PARSER__", Nothing)
			End Set
			Get
				Dim p As Object = getProperty("__PARSER__")
    
				If TypeOf p Is HTMLEditorKit.Parser Then Return CType(p, HTMLEditorKit.Parser)
				Return parser
			End Get
		End Property


		''' <summary>
		''' Replaces the children of the given element with the contents
		''' specified as an HTML string.
		''' 
		''' <p>This will be seen as at least two events, n inserts followed by
		''' a remove.</p>
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>setInnerHTML(elem, "&lt;ul&gt;&lt;li&gt;")</code>
		''' results in the following structure (new elements are <font
		''' color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''         \
		'''         <font color="red">&lt;ul&gt;</font>
		'''           \
		'''           <font color="red">&lt;li&gt;</font>
		''' </pre>
		''' 
		''' <p>Parameter <code>elem</code> must not be a leaf element,
		''' otherwise an <code>IllegalArgumentException</code> is thrown.
		''' If either <code>elem</code> or <code>htmlText</code> parameter
		''' is <code>null</code>, no changes are made to the document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' <code>HTMLEditorKit.Parser</code> set. This will be the case
		''' if the document was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the branch element whose children will be replaced </param>
		''' <param name="htmlText"> the string to be parsed and assigned to <code>elem</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>elem</code> is a leaf </exception>
		''' <exception cref="IllegalStateException"> if an <code>HTMLEditorKit.Parser</code>
		'''         has not been defined
		''' @since 1.3 </exception>
		Public Overridable Sub setInnerHTML(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()
			If elem IsNot Nothing AndAlso elem.leaf Then Throw New System.ArgumentException("Can not set inner HTML of a leaf")
			If elem IsNot Nothing AndAlso htmlText IsNot Nothing Then
				Dim oldCount As Integer = elem.elementCount
				Dim insertPosition As Integer = elem.startOffset
				insertHTML(elem, elem.startOffset, htmlText, True)
				If elem.elementCount > oldCount Then removeElements(elem, elem.elementCount - oldCount, oldCount)
			End If
		End Sub

		''' <summary>
		''' Replaces the given element in the parent with the contents
		''' specified as an HTML string.
		''' 
		''' <p>This will be seen as at least two events, n inserts followed by
		''' a remove.</p>
		''' 
		''' <p>When replacing a leaf this will attempt to make sure there is
		''' a newline present if one is needed. This may result in an additional
		''' element being inserted. Consider, if you were to replace a character
		''' element that contained a newline with &lt;img&gt; this would create
		''' two elements, one for the image, and one for the newline.</p>
		''' 
		''' <p>If you try to replace the element at length you will most
		''' likely end up with two elements, eg
		''' <code>setOuterHTML(getCharacterElement (getLength()),
		''' "blah")</code> will result in two leaf elements at the end, one
		''' representing 'blah', and the other representing the end
		''' element.</p>
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>setOuterHTML(elem, "&lt;ul&gt;&lt;li&gt;")</code>
		''' results in the following structure (new elements are <font
		''' color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''    &lt;body&gt;
		'''      |
		'''     <font color="red">&lt;ul&gt;</font>
		'''       \
		'''       <font color="red">&lt;li&gt;</font>
		''' </pre>
		''' 
		''' <p>If either <code>elem</code> or <code>htmlText</code>
		''' parameter is <code>null</code>, no changes are made to the
		''' document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' HTMLEditorKit.Parser set. This will be the case if the document
		''' was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the element to replace </param>
		''' <param name="htmlText"> the string to be parsed and inserted in place of <code>elem</code> </param>
		''' <exception cref="IllegalStateException"> if an HTMLEditorKit.Parser has not
		'''         been set
		''' @since 1.3 </exception>
		Public Overridable Sub setOuterHTML(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()
			If elem IsNot Nothing AndAlso elem.parentElement IsNot Nothing AndAlso htmlText IsNot Nothing Then
				Dim start As Integer = elem.startOffset
				Dim [end] As Integer = elem.endOffset
				Dim startLength As Integer = length
				' We don't want a newline if elem is a leaf, and doesn't contain
				' a newline.
				Dim wantsNewline As Boolean = Not elem.leaf
				If (Not wantsNewline) AndAlso ([end] > startLength OrElse getText([end] - 1, 1).Chars(0) = NEWLINE(0)) Then wantsNewline = True
				Dim parent As Element = elem.parentElement
				Dim oldCount As Integer = parent.elementCount
				insertHTML(parent, start, htmlText, wantsNewline)
				' Remove old.
				Dim newLength As Integer = length
				If oldCount <> parent.elementCount Then
					Dim removeIndex As Integer = parent.getElementIndex(start + newLength - startLength)
					removeElements(parent, removeIndex, 1)
				End If
			End If
		End Sub

		''' <summary>
		''' Inserts the HTML specified as a string at the start
		''' of the element.
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>insertAfterStart(elem,
		''' "&lt;ul&gt;&lt;li&gt;")</code> results in the following structure
		''' (new elements are <font color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''        &lt;body&gt;
		'''          |
		'''        <b>&lt;div&gt;</b>
		'''       /  |  \
		'''    <font color="red">&lt;ul&gt;</font> &lt;p&gt; &lt;p&gt;
		'''     /
		'''  <font color="red">&lt;li&gt;</font>
		''' </pre>
		''' 
		''' <p>Unlike the <code>insertBeforeStart</code> method, new
		'''  elements become <em>children</em> of the specified element,
		'''  not siblings.</p>
		''' 
		''' <p>Parameter <code>elem</code> must not be a leaf element,
		''' otherwise an <code>IllegalArgumentException</code> is thrown.
		''' If either <code>elem</code> or <code>htmlText</code> parameter
		''' is <code>null</code>, no changes are made to the document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' <code>HTMLEditorKit.Parser</code> set. This will be the case
		''' if the document was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the branch element to be the root for the new text </param>
		''' <param name="htmlText"> the string to be parsed and assigned to <code>elem</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>elem</code> is a leaf </exception>
		''' <exception cref="IllegalStateException"> if an HTMLEditorKit.Parser has not
		'''         been set on the document
		''' @since 1.3 </exception>
		Public Overridable Sub insertAfterStart(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()

			If elem Is Nothing OrElse htmlText Is Nothing Then Return

			If elem.leaf Then Throw New System.ArgumentException("Can not insert HTML after start of a leaf")
			insertHTML(elem, elem.startOffset, htmlText, False)
		End Sub

		''' <summary>
		''' Inserts the HTML specified as a string at the end of
		''' the element.
		''' 
		''' <p> If <code>elem</code>'s children are leaves, and the
		''' character at a <code>elem.getEndOffset() - 1</code> is a newline,
		''' this will insert before the newline so that there isn't text after
		''' the newline.</p>
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>insertBeforeEnd(elem, "&lt;ul&gt;&lt;li&gt;")</code>
		''' results in the following structure (new elements are <font
		''' color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''        &lt;body&gt;
		'''          |
		'''        <b>&lt;div&gt;</b>
		'''       /  |  \
		'''     &lt;p&gt; &lt;p&gt; <font color="red">&lt;ul&gt;</font>
		'''               \
		'''               <font color="red">&lt;li&gt;</font>
		''' </pre>
		''' 
		''' <p>Unlike the <code>insertAfterEnd</code> method, new elements
		''' become <em>children</em> of the specified element, not
		''' siblings.</p>
		''' 
		''' <p>Parameter <code>elem</code> must not be a leaf element,
		''' otherwise an <code>IllegalArgumentException</code> is thrown.
		''' If either <code>elem</code> or <code>htmlText</code> parameter
		''' is <code>null</code>, no changes are made to the document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' <code>HTMLEditorKit.Parser</code> set. This will be the case
		''' if the document was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the element to be the root for the new text </param>
		''' <param name="htmlText"> the string to be parsed and assigned to <code>elem</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>elem</code> is a leaf </exception>
		''' <exception cref="IllegalStateException"> if an HTMLEditorKit.Parser has not
		'''         been set on the document
		''' @since 1.3 </exception>
		Public Overridable Sub insertBeforeEnd(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()
			If elem IsNot Nothing AndAlso elem.leaf Then Throw New System.ArgumentException("Can not set inner HTML before end of leaf")
			If elem IsNot Nothing Then
				Dim offset As Integer = elem.endOffset
				If elem.getElement(elem.getElementIndex(offset - 1)).leaf AndAlso getText(offset - 1, 1).Chars(0) = NEWLINE(0) Then offset -= 1
				insertHTML(elem, offset, htmlText, False)
			End If
		End Sub

		''' <summary>
		''' Inserts the HTML specified as a string before the start of
		''' the given element.
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>insertBeforeStart(elem,
		''' "&lt;ul&gt;&lt;li&gt;")</code> results in the following structure
		''' (new elements are <font color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''        &lt;body&gt;
		'''         /  \
		'''      <font color="red">&lt;ul&gt;</font> <b>&lt;div&gt;</b>
		'''       /    /  \
		'''     <font color="red">&lt;li&gt;</font> &lt;p&gt;  &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Unlike the <code>insertAfterStart</code> method, new
		''' elements become <em>siblings</em> of the specified element, not
		''' children.</p>
		''' 
		''' <p>If either <code>elem</code> or <code>htmlText</code>
		''' parameter is <code>null</code>, no changes are made to the
		''' document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' <code>HTMLEditorKit.Parser</code> set. This will be the case
		''' if the document was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the element the content is inserted before </param>
		''' <param name="htmlText"> the string to be parsed and inserted before <code>elem</code> </param>
		''' <exception cref="IllegalStateException"> if an HTMLEditorKit.Parser has not
		'''         been set on the document
		''' @since 1.3 </exception>
		Public Overridable Sub insertBeforeStart(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()
			If elem IsNot Nothing Then
				Dim parent As Element = elem.parentElement

				If parent IsNot Nothing Then insertHTML(parent, elem.startOffset, htmlText, False)
			End If
		End Sub

		''' <summary>
		''' Inserts the HTML specified as a string after the the end of the
		''' given element.
		''' 
		''' <p>Consider the following structure (the <code>elem</code>
		''' parameter is <b>in bold</b>).</p>
		''' 
		''' <pre>
		'''     &lt;body&gt;
		'''       |
		'''     <b>&lt;div&gt;</b>
		'''      /  \
		'''    &lt;p&gt;   &lt;p&gt;
		''' </pre>
		''' 
		''' <p>Invoking <code>insertAfterEnd(elem, "&lt;ul&gt;&lt;li&gt;")</code>
		''' results in the following structure (new elements are <font
		''' color="red">in red</font>).</p>
		''' 
		''' <pre>
		'''        &lt;body&gt;
		'''         /  \
		'''      <b>&lt;div&gt;</b> <font color="red">&lt;ul&gt;</font>
		'''       / \    \
		'''     &lt;p&gt; &lt;p&gt;  <font color="red">&lt;li&gt;</font>
		''' </pre>
		''' 
		''' <p>Unlike the <code>insertBeforeEnd</code> method, new elements
		''' become <em>siblings</em> of the specified element, not
		''' children.</p>
		''' 
		''' <p>If either <code>elem</code> or <code>htmlText</code>
		''' parameter is <code>null</code>, no changes are made to the
		''' document.</p>
		''' 
		''' <p>For this to work correctly, the document must have an
		''' <code>HTMLEditorKit.Parser</code> set. This will be the case
		''' if the document was created from an HTMLEditorKit via the
		''' <code>createDefaultDocument</code> method.</p>
		''' </summary>
		''' <param name="elem"> the element the content is inserted after </param>
		''' <param name="htmlText"> the string to be parsed and inserted after <code>elem</code> </param>
		''' <exception cref="IllegalStateException"> if an HTMLEditorKit.Parser has not
		'''         been set on the document
		''' @since 1.3 </exception>
		Public Overridable Sub insertAfterEnd(ByVal elem As Element, ByVal htmlText As String)
			verifyParser()
			If elem IsNot Nothing Then
				Dim parent As Element = elem.parentElement

				If parent IsNot Nothing Then
					' If we are going to insert the string into the body
					' section, it is necessary to set the corrsponding flag.
					If HTML.Tag.BODY.name.Equals(parent.name) Then insertInBody = True
					Dim offset As Integer = elem.endOffset
					If offset > (length + 1) Then
						offset -= 1
					ElseIf elem.leaf AndAlso getText(offset - 1, 1).Chars(0) = NEWLINE(0) Then
						offset -= 1
					End If
					insertHTML(parent, offset, htmlText, False)
					' Cleanup the flag, if any.
					If insertInBody Then insertInBody = False
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the element that has the given id <code>Attribute</code>.
		''' If the element can't be found, <code>null</code> is returned.
		''' Note that this method works on an <code>Attribute</code>,
		''' <i>not</i> a character tag.  In the following HTML snippet:
		''' <code>&lt;a id="HelloThere"&gt;</code> the attribute is
		''' 'id' and the character tag is 'a'.
		''' This is a convenience method for
		''' <code>getElement(RootElement, HTML.Attribute.id, id)</code>.
		''' This is not thread-safe.
		''' </summary>
		''' <param name="id">  the string representing the desired <code>Attribute</code> </param>
		''' <returns> the element with the specified <code>Attribute</code>
		'''          or <code>null</code> if it can't be found,
		'''          or <code>null</code> if <code>id</code> is <code>null</code> </returns>
		''' <seealso cref= javax.swing.text.html.HTML.Attribute
		''' @since 1.3 </seealso>
		Public Overridable Function getElement(ByVal id As String) As Element
			If id Is Nothing Then Return Nothing
			Return getElement(defaultRootElement, HTML.Attribute.ID, id, True)
		End Function

		''' <summary>
		''' Returns the child element of <code>e</code> that contains the
		''' attribute, <code>attribute</code> with value <code>value</code>, or
		''' <code>null</code> if one isn't found. This is not thread-safe.
		''' </summary>
		''' <param name="e"> the root element where the search begins </param>
		''' <param name="attribute"> the desired <code>Attribute</code> </param>
		''' <param name="value"> the values for the specified <code>Attribute</code> </param>
		''' <returns> the element with the specified <code>Attribute</code>
		'''          and the specified <code>value</code>, or <code>null</code>
		'''          if it can't be found </returns>
		''' <seealso cref= javax.swing.text.html.HTML.Attribute
		''' @since 1.3 </seealso>
		Public Overridable Function getElement(ByVal e As Element, ByVal attribute As Object, ByVal value As Object) As Element
			Return getElement(e, attribute, value, True)
		End Function

		''' <summary>
		''' Returns the child element of <code>e</code> that contains the
		''' attribute, <code>attribute</code> with value <code>value</code>, or
		''' <code>null</code> if one isn't found. This is not thread-safe.
		''' <p>
		''' If <code>searchLeafAttributes</code> is true, and <code>e</code> is
		''' a leaf, any attributes that are instances of <code>HTML.Tag</code>
		''' with a value that is an <code>AttributeSet</code> will also be checked.
		''' </summary>
		''' <param name="e"> the root element where the search begins </param>
		''' <param name="attribute"> the desired <code>Attribute</code> </param>
		''' <param name="value"> the values for the specified <code>Attribute</code> </param>
		''' <returns> the element with the specified <code>Attribute</code>
		'''          and the specified <code>value</code>, or <code>null</code>
		'''          if it can't be found </returns>
		''' <seealso cref= javax.swing.text.html.HTML.Attribute </seealso>
		Private Function getElement(ByVal e As Element, ByVal attribute As Object, ByVal value As Object, ByVal searchLeafAttributes As Boolean) As Element
			Dim attr As AttributeSet = e.attributes

			If attr IsNot Nothing AndAlso attr.isDefined(attribute) Then
				If value.Equals(attr.getAttribute(attribute)) Then Return e
			End If
			If Not e.leaf Then
				Dim counter As Integer = 0
				Dim maxCounter As Integer = e.elementCount
				Do While counter < maxCounter
					Dim retValue As Element = getElement(e.getElement(counter), attribute, value, searchLeafAttributes)

					If retValue IsNot Nothing Then Return retValue
					counter += 1
				Loop
			ElseIf searchLeafAttributes AndAlso attr IsNot Nothing Then
				' For some leaf elements we store the actual attributes inside
				' the AttributeSet of the Element (such as anchors).
				Dim names As System.Collections.IEnumerator = attr.attributeNames
				If names IsNot Nothing Then
					Do While names.hasMoreElements()
						Dim name As Object = names.nextElement()
						If (TypeOf name Is HTML.Tag) AndAlso (TypeOf attr.getAttribute(name) Is AttributeSet) Then

							Dim check As AttributeSet = CType(attr.getAttribute(name), AttributeSet)
							If check.isDefined(attribute) AndAlso value.Equals(check.getAttribute(attribute)) Then Return e
						End If
					Loop
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Verifies the document has an <code>HTMLEditorKit.Parser</code> set.
		''' If <code>getParser</code> returns <code>null</code>, this will throw an
		''' IllegalStateException.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the document does not have a Parser </exception>
		Private Sub verifyParser()
			If parser Is Nothing Then Throw New IllegalStateException("No HTMLEditorKit.Parser")
		End Sub

		''' <summary>
		''' Installs a default Parser if one has not been installed yet.
		''' </summary>
		Private Sub installParserIfNecessary()
			If parser Is Nothing Then parser = (New HTMLEditorKit).parser
		End Sub

		''' <summary>
		''' Inserts a string of HTML into the document at the given position.
		''' <code>parent</code> is used to identify the location to insert the
		''' <code>html</code>. If <code>parent</code> is a leaf this can have
		''' unexpected results.
		''' </summary>
		Private Sub insertHTML(ByVal parent As Element, ByVal offset As Integer, ByVal html As String, ByVal wantsTrailingNewline As Boolean)
			If parent IsNot Nothing AndAlso html IsNot Nothing Then
				Dim ___parser As HTMLEditorKit.Parser = parser
				If ___parser IsNot Nothing Then
					Dim lastOffset As Integer = Math.Max(0, offset - 1)
					Dim charElement As Element = getCharacterElement(lastOffset)
					Dim commonParent As Element = parent
					Dim pop As Integer = 0
					Dim push As Integer = 0

					If parent.startOffset > lastOffset Then
						Do While commonParent IsNot Nothing AndAlso commonParent.startOffset > lastOffset
							commonParent = commonParent.parentElement
							push += 1
						Loop
						If commonParent Is Nothing Then Throw New BadLocationException("No common parent", offset)
					End If
					Do While charElement IsNot Nothing AndAlso charElement IsNot commonParent
						pop += 1
						charElement = charElement.parentElement
					Loop
					If charElement IsNot Nothing Then
						' Found it, do the insert.
						Dim ___reader As New HTMLReader(Me, offset, pop - 1, push, Nothing, False, True, wantsTrailingNewline)

						___parser.parse(New StringReader(html), ___reader, True)
						___reader.flush()
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Removes child Elements of the passed in Element <code>e</code>. This
		''' will do the necessary cleanup to ensure the element representing the
		''' end character is correctly created.
		''' <p>This is not a general purpose method, it assumes that <code>e</code>
		''' will still have at least one child after the remove, and it assumes
		''' the character at <code>e.getStartOffset() - 1</code> is a newline and
		''' is of length 1.
		''' </summary>
		Private Sub removeElements(ByVal e As Element, ByVal index As Integer, ByVal count As Integer)
			writeLock()
			Try
				Dim start As Integer = e.getElement(index).startOffset
				Dim [end] As Integer = e.getElement(index + count - 1).endOffset
				If [end] > length Then
					removeElementsAtEnd(e, index, count, start, [end])
				Else
					removeElements(e, index, count, start, [end])
				End If
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Called to remove child elements of <code>e</code> when one of the
		''' elements to remove is representing the end character.
		''' <p>Since the Content will not allow a removal to the end character
		''' this will do a remove from <code>start - 1</code> to <code>end</code>.
		''' The end Element(s) will be removed, and the element representing
		''' <code>start - 1</code> to <code>start</code> will be recreated. This
		''' Element has to be recreated as after the content removal its offsets
		''' become <code>start - 1</code> to <code>start - 1</code>.
		''' </summary>
		Private Sub removeElementsAtEnd(ByVal e As Element, ByVal index As Integer, ByVal count As Integer, ByVal start As Integer, ByVal [end] As Integer)
			' index must be > 0 otherwise no insert would have happened.
			Dim isLeaf As Boolean = (e.getElement(index - 1).leaf)
			Dim dde As New DefaultDocumentEvent(start - 1, [end] - start + 1, DocumentEvent.EventType.REMOVE)

			If isLeaf Then
				Dim endE As Element = getCharacterElement(length)
				' e.getElement(index - 1) should represent the newline.
				index -= 1
				If endE.parentElement IsNot e Then
					' The hiearchies don't match, we'll have to manually
					' recreate the leaf at e.getElement(index - 1)
					count += 1
					replace(dde, e, index, count, start, [end], True, True)
				Else
					' The hierarchies for the end Element and
					' e.getElement(index - 1), match, we can safely remove
					' the Elements and the end content will be aligned
					' appropriately.
					replace(dde, e, index, count, start, [end], True, False)
				End If
			Else
				' Not a leaf, descend until we find the leaf representing
				' start - 1 and remove it.
				Dim newLineE As Element = e.getElement(index - 1)
				Do While Not newLineE.leaf
					newLineE = newLineE.getElement(newLineE.elementCount - 1)
				Loop
				newLineE = newLineE.parentElement
				replace(dde, e, index, count, start, [end], False, False)
				replace(dde, newLineE, newLineE.elementCount - 1, 1, start, [end], True, True)
			End If
			postRemoveUpdate(dde)
			dde.end()
			fireRemoveUpdate(dde)
			fireUndoableEditUpdate(New UndoableEditEvent(Me, dde))
		End Sub

		''' <summary>
		''' This is used by <code>removeElementsAtEnd</code>, it removes
		''' <code>count</code> elements starting at <code>start</code> from
		''' <code>e</code>.  If <code>remove</code> is true text of length
		''' <code>start - 1</code> to <code>end - 1</code> is removed.  If
		''' <code>create</code> is true a new leaf is created of length 1.
		''' </summary>
		Private Sub replace(ByVal dde As DefaultDocumentEvent, ByVal e As Element, ByVal index As Integer, ByVal count As Integer, ByVal start As Integer, ByVal [end] As Integer, ByVal remove As Boolean, ByVal create As Boolean)
			Dim added As Element()
			Dim attrs As AttributeSet = e.getElement(index).attributes
			Dim removed As Element() = New Element(count - 1){}

			For counter As Integer = 0 To count - 1
				removed(counter) = e.getElement(counter + index)
			Next counter
			If remove Then
				Dim u As UndoableEdit = content.remove(start - 1, [end] - start)
				If u IsNot Nothing Then dde.addEdit(u)
			End If
			If create Then
				added = New Element(0){}
				added(0) = createLeafElement(e, attrs, start - 1, start)
			Else
				added = New Element(){}
			End If
			dde.addEdit(New ElementEdit(e, index, removed, added))
			CType(e, AbstractDocument.BranchElement).replace(index, removed.Length, added)
		End Sub

		''' <summary>
		''' Called to remove child Elements when the end is not touched.
		''' </summary>
		Private Sub removeElements(ByVal e As Element, ByVal index As Integer, ByVal count As Integer, ByVal start As Integer, ByVal [end] As Integer)
			Dim removed As Element() = New Element(count - 1){}
			Dim added As Element() = New Element(){}
			For counter As Integer = 0 To count - 1
				removed(counter) = e.getElement(counter + index)
			Next counter
			Dim dde As New DefaultDocumentEvent(start, [end] - start, DocumentEvent.EventType.REMOVE)
			CType(e, AbstractDocument.BranchElement).replace(index, removed.Length, added)
			dde.addEdit(New ElementEdit(e, index, removed, added))
			Dim u As UndoableEdit = content.remove(start, [end] - start)
			If u IsNot Nothing Then dde.addEdit(u)
			postRemoveUpdate(dde)
			dde.end()
			fireRemoveUpdate(dde)
			If u IsNot Nothing Then fireUndoableEditUpdate(New UndoableEditEvent(Me, dde))
		End Sub


		' These two are provided for inner class access. The are named different
		' than the super class as the super class implementations are final.
		Friend Overridable Sub obtainLock()
			writeLock()
		End Sub

		Friend Overridable Sub releaseLock()
			writeUnlock()
		End Sub

		'
		' Provided for inner class access.
		'

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overrides Sub fireChangedUpdate(ByVal e As DocumentEvent)
			MyBase.fireChangedUpdate(e)
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overrides Sub fireUndoableEditUpdate(ByVal e As UndoableEditEvent)
			MyBase.fireUndoableEditUpdate(e)
		End Sub

		Friend Overridable Function hasBaseTag() As Boolean
			Return ___hasBaseTag
		End Function

		Friend Overridable Property baseTarget As String
			Get
				Return baseTarget
			End Get
		End Property

	'    
	'     * state defines whether the document is a frame document
	'     * or not.
	'     
		Private frameDocument As Boolean = False
		Private preservesUnknownTags As Boolean = True

	'    
	'     * Used to store button groups for radio buttons in
	'     * a form.
	'     
		Private radioButtonGroupsMap As Dictionary(Of String, ButtonGroup)

		''' <summary>
		''' Document property for the number of tokens to buffer
		''' before building an element subtree to represent them.
		''' </summary>
		Friend Const TokenThreshold As String = "token threshold"

		Private Const MaxThreshold As Integer = 10000

		Private Const StepThreshold As Integer = 5


		''' <summary>
		''' Document property key value. The value for the key will be a Vector
		''' of Strings that are comments not found in the body.
		''' </summary>
		Public Const AdditionalComments As String = "AdditionalComments"

		''' <summary>
		''' Document property key value. The value for the key will be a
		''' String indicating the default type of stylesheet links.
		''' </summary>
		' public 
	 Friend Const StyleType As String = "StyleType"

		''' <summary>
		''' The location to resolve relative URLs against.  By
		''' default this will be the document's URL if the document
		''' was loaded from a URL.  If a base tag is found and
		''' can be parsed, it will be used as the base location.
		''' </summary>
		Friend base As java.net.URL

		''' <summary>
		''' does the document have base tag
		''' </summary>
		Friend ___hasBaseTag As Boolean = False

		''' <summary>
		''' BASE tag's TARGET attribute value
		''' </summary>
		Private baseTarget As String = Nothing

		''' <summary>
		''' The parser that is used when inserting html into the existing
		''' document.
		''' </summary>
		Private parser As HTMLEditorKit.Parser

		''' <summary>
		''' Used for inserts when a null AttributeSet is supplied.
		''' </summary>
		Private Shared contentAttributeSet As AttributeSet

		''' <summary>
		''' Property Maps are registered under, will be a Hashtable.
		''' </summary>
		Friend Shared MAP_PROPERTY As String = "__MAP__"

		Private Shared NEWLINE As Char()

		''' <summary>
		''' Indicates that direct insertion to body section takes place.
		''' </summary>
		Private insertInBody As Boolean = False

		''' <summary>
		''' I18N property key.
		''' </summary>
		''' <seealso cref= AbstractDocument#I18NProperty </seealso>
		Private Shadows Const I18NProperty As String = "i18n"

		Shared Sub New()
			contentAttributeSet = New SimpleAttributeSet
			CType(contentAttributeSet, MutableAttributeSet).addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
			NEWLINE = New Char(0){}
			NEWLINE(0) = ControlChars.Lf
		End Sub


		''' <summary>
		''' An iterator to iterate over a particular type of
		''' tag.  The iterator is not thread safe.  If reliable
		''' access to the document is not already ensured by
		''' the context under which the iterator is being used,
		''' its use should be performed under the protection of
		''' Document.render.
		''' </summary>
		Public MustInherit Class [Iterator]

			''' <summary>
			''' Return the attributes for this tag. </summary>
			''' <returns> the <code>AttributeSet</code> for this tag, or
			'''      <code>null</code> if none can be found </returns>
			Public MustOverride ReadOnly Property attributes As AttributeSet

			''' <summary>
			''' Returns the start of the range for which the current occurrence of
			''' the tag is defined and has the same attributes.
			''' </summary>
			''' <returns> the start of the range, or -1 if it can't be found </returns>
			Public MustOverride ReadOnly Property startOffset As Integer

			''' <summary>
			''' Returns the end of the range for which the current occurrence of
			''' the tag is defined and has the same attributes.
			''' </summary>
			''' <returns> the end of the range </returns>
			Public MustOverride ReadOnly Property endOffset As Integer

			''' <summary>
			''' Move the iterator forward to the next occurrence
			''' of the tag it represents.
			''' </summary>
			Public MustOverride Sub [next]()

			''' <summary>
			''' Indicates if the iterator is currently
			''' representing an occurrence of a tag.  If
			''' false there are no more tags for this iterator. </summary>
			''' <returns> true if the iterator is currently representing an
			'''              occurrence of a tag, otherwise returns false </returns>
			Public MustOverride ReadOnly Property valid As Boolean

			''' <summary>
			''' Type of tag this iterator represents.
			''' </summary>
			Public MustOverride ReadOnly Property tag As HTML.Tag
		End Class

		''' <summary>
		''' An iterator to iterate over a particular type of tag.
		''' </summary>
		Friend Class LeafIterator
			Inherits [Iterator]

			Friend Sub New(ByVal t As HTML.Tag, ByVal doc As Document)
				tag = t
				pos = New ElementIterator(doc)
				endOffset = 0
				[next]()
			End Sub

			''' <summary>
			''' Returns the attributes for this tag. </summary>
			''' <returns> the <code>AttributeSet</code> for this tag,
			'''              or <code>null</code> if none can be found </returns>
			Public Property Overrides attributes As AttributeSet
				Get
					Dim elem As Element = pos.current()
					If elem IsNot Nothing Then
						Dim a As AttributeSet = CType(elem.attributes.getAttribute(tag), AttributeSet)
						If a Is Nothing Then a = elem.attributes
						Return a
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Returns the start of the range for which the current occurrence of
			''' the tag is defined and has the same attributes.
			''' </summary>
			''' <returns> the start of the range, or -1 if it can't be found </returns>
			Public Property Overrides startOffset As Integer
				Get
					Dim elem As Element = pos.current()
					If elem IsNot Nothing Then Return elem.startOffset
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the end of the range for which the current occurrence of
			''' the tag is defined and has the same attributes.
			''' </summary>
			''' <returns> the end of the range </returns>
			Public Property Overrides endOffset As Integer
				Get
					Return endOffset
				End Get
			End Property

			''' <summary>
			''' Moves the iterator forward to the next occurrence
			''' of the tag it represents.
			''' </summary>
			Public Overrides Sub [next]()
				nextLeaf(pos)
				Do While valid
					Dim elem As Element = pos.current()
					If elem.startOffset >= endOffset Then
						Dim a As AttributeSet = pos.current().attributes

						If a.isDefined(tag) OrElse a.getAttribute(StyleConstants.NameAttribute) Is tag Then

							' we found the next one
							endOffsetset()
							Exit Do
						End If
					End If
					nextLeaf(pos)
				Loop
			End Sub

			''' <summary>
			''' Returns the type of tag this iterator represents.
			''' </summary>
			''' <returns> the <code>HTML.Tag</code> that this iterator represents. </returns>
			''' <seealso cref= javax.swing.text.html.HTML.Tag </seealso>
			Public Property Overrides tag As HTML.Tag
				Get
					Return tag
				End Get
			End Property

			''' <summary>
			''' Returns true if the current position is not <code>null</code>. </summary>
			''' <returns> true if current position is not <code>null</code>,
			'''              otherwise returns false </returns>
			Public Property Overrides valid As Boolean
				Get
					Return (pos.current() IsNot Nothing)
				End Get
			End Property

			''' <summary>
			''' Moves the given iterator to the next leaf element. </summary>
			''' <param name="iter">  the iterator to be scanned </param>
			Friend Overridable Sub nextLeaf(ByVal iter As ElementIterator)
				iter.next()
				Do While iter.current() IsNot Nothing
					Dim e As Element = iter.current()
					If e.leaf Then Exit Do
					iter.next()
				Loop
			End Sub

			''' <summary>
			''' Marches a cloned iterator forward to locate the end
			''' of the run.  This sets the value of <code>endOffset</code>.
			''' </summary>
			Friend Overridable Sub setEndOffset()
				Dim a0 As AttributeSet = attributes
				endOffset = pos.current().endOffset
				Dim fwd As ElementIterator = CType(pos.clone(), ElementIterator)
				nextLeaf(fwd)
				Do While fwd.current() IsNot Nothing
					Dim e As Element = fwd.current()
					Dim a1 As AttributeSet = CType(e.attributes.getAttribute(tag), AttributeSet)
					If (a1 Is Nothing) OrElse ((Not a1.Equals(a0))) Then Exit Do
					endOffset = e.endOffset
					nextLeaf(fwd)
				Loop
			End Sub

			Private endOffset As Integer
			Private tag As HTML.Tag
			Private pos As ElementIterator

		End Class

		''' <summary>
		''' An HTML reader to load an HTML document with an HTML
		''' element structure.  This is a set of callbacks from
		''' the parser, implemented to create a set of elements
		''' tagged with attributes.  The parse builds up tokens
		''' (ElementSpec) that describe the element subtree desired,
		''' and burst it into the document under the protection of
		''' a write lock using the insert method on the document
		''' outer class.
		''' <p>
		''' The reader can be configured by registering actions
		''' (of type <code>HTMLDocument.HTMLReader.TagAction</code>)
		''' that describe how to handle the action.  The idea behind
		''' the actions provided is that the most natural text editing
		''' operations can be provided if the element structure boils
		''' down to paragraphs with runs of some kind of style
		''' in them.  Some things are more naturally specified
		''' structurally, so arbitrary structure should be allowed
		''' above the paragraphs, but will need to be edited with structural
		''' actions.  The implication of this is that some of the
		''' HTML elements specified in the stream being parsed will
		''' be collapsed into attributes, and in some cases paragraphs
		''' will be synthesized.  When HTML elements have been
		''' converted to attributes, the attribute key will be of
		''' type HTML.Tag, and the value will be of type AttributeSet
		''' so that no information is lost.  This enables many of the
		''' existing actions to work so that the user can type input,
		''' hit the return key, backspace, delete, etc and have a
		''' reasonable result.  Selections can be created, and attributes
		''' applied or removed, etc.  With this in mind, the work done
		''' by the reader can be categorized into the following kinds
		''' of tasks:
		''' <dl>
		''' <dt>Block
		''' <dd>Build the structure like it's specified in the stream.
		''' This produces elements that contain other elements.
		''' <dt>Paragraph
		''' <dd>Like block except that it's expected that the element
		''' will be used with a paragraph view so a paragraph element
		''' won't need to be synthesized.
		''' <dt>Character
		''' <dd>Contribute the element as an attribute that will start
		''' and stop at arbitrary text locations.  This will ultimately
		''' be mixed into a run of text, with all of the currently
		''' flattened HTML character elements.
		''' <dt>Special
		''' <dd>Produce an embedded graphical element.
		''' <dt>Form
		''' <dd>Produce an element that is like the embedded graphical
		''' element, except that it also has a component model associated
		''' with it.
		''' <dt>Hidden
		''' <dd>Create an element that is hidden from view when the
		''' document is being viewed read-only, and visible when the
		''' document is being edited.  This is useful to keep the
		''' model from losing information, and used to store things
		''' like comments and unrecognized tags.
		''' 
		''' </dl>
		''' <p>
		''' Currently, &lt;APPLET&gt;, &lt;PARAM&gt;, &lt;MAP&gt;, &lt;AREA&gt;, &lt;LINK&gt;,
		''' &lt;SCRIPT&gt; and &lt;STYLE&gt; are unsupported.
		''' 
		''' <p>
		''' The assignment of the actions described is shown in the
		''' following table for the tags defined in <code>HTML.Tag</code>.
		''' <table border=1 summary="HTML tags and assigned actions">
		''' <tr><th>Tag</th><th>Action</th></tr>
		''' <tr><td><code>HTML.Tag.A</code>         <td>CharacterAction
		''' <tr><td><code>HTML.Tag.ADDRESS</code>   <td>CharacterAction
		''' <tr><td><code>HTML.Tag.APPLET</code>    <td>HiddenAction
		''' <tr><td><code>HTML.Tag.AREA</code>      <td>AreaAction
		''' <tr><td><code>HTML.Tag.B</code>         <td>CharacterAction
		''' <tr><td><code>HTML.Tag.BASE</code>      <td>BaseAction
		''' <tr><td><code>HTML.Tag.BASEFONT</code>  <td>CharacterAction
		''' <tr><td><code>HTML.Tag.BIG</code>       <td>CharacterAction
		''' <tr><td><code>HTML.Tag.BLOCKQUOTE</code><td>BlockAction
		''' <tr><td><code>HTML.Tag.BODY</code>      <td>BlockAction
		''' <tr><td><code>HTML.Tag.BR</code>        <td>SpecialAction
		''' <tr><td><code>HTML.Tag.CAPTION</code>   <td>BlockAction
		''' <tr><td><code>HTML.Tag.CENTER</code>    <td>BlockAction
		''' <tr><td><code>HTML.Tag.CITE</code>      <td>CharacterAction
		''' <tr><td><code>HTML.Tag.CODE</code>      <td>CharacterAction
		''' <tr><td><code>HTML.Tag.DD</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.DFN</code>       <td>CharacterAction
		''' <tr><td><code>HTML.Tag.DIR</code>       <td>BlockAction
		''' <tr><td><code>HTML.Tag.DIV</code>       <td>BlockAction
		''' <tr><td><code>HTML.Tag.DL</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.DT</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.EM</code>        <td>CharacterAction
		''' <tr><td><code>HTML.Tag.FONT</code>      <td>CharacterAction
		''' <tr><td><code>HTML.Tag.FORM</code>      <td>As of 1.4 a BlockAction
		''' <tr><td><code>HTML.Tag.FRAME</code>     <td>SpecialAction
		''' <tr><td><code>HTML.Tag.FRAMESET</code>  <td>BlockAction
		''' <tr><td><code>HTML.Tag.H1</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.H2</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.H3</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.H4</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.H5</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.H6</code>        <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.HEAD</code>      <td>HeadAction
		''' <tr><td><code>HTML.Tag.HR</code>        <td>SpecialAction
		''' <tr><td><code>HTML.Tag.HTML</code>      <td>BlockAction
		''' <tr><td><code>HTML.Tag.I</code>         <td>CharacterAction
		''' <tr><td><code>HTML.Tag.IMG</code>       <td>SpecialAction
		''' <tr><td><code>HTML.Tag.INPUT</code>     <td>FormAction
		''' <tr><td><code>HTML.Tag.ISINDEX</code>   <td>IsndexAction
		''' <tr><td><code>HTML.Tag.KBD</code>       <td>CharacterAction
		''' <tr><td><code>HTML.Tag.LI</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.LINK</code>      <td>LinkAction
		''' <tr><td><code>HTML.Tag.MAP</code>       <td>MapAction
		''' <tr><td><code>HTML.Tag.MENU</code>      <td>BlockAction
		''' <tr><td><code>HTML.Tag.META</code>      <td>MetaAction
		''' <tr><td><code>HTML.Tag.NOFRAMES</code>  <td>BlockAction
		''' <tr><td><code>HTML.Tag.OBJECT</code>    <td>SpecialAction
		''' <tr><td><code>HTML.Tag.OL</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.OPTION</code>    <td>FormAction
		''' <tr><td><code>HTML.Tag.P</code>         <td>ParagraphAction
		''' <tr><td><code>HTML.Tag.PARAM</code>     <td>HiddenAction
		''' <tr><td><code>HTML.Tag.PRE</code>       <td>PreAction
		''' <tr><td><code>HTML.Tag.SAMP</code>      <td>CharacterAction
		''' <tr><td><code>HTML.Tag.SCRIPT</code>    <td>HiddenAction
		''' <tr><td><code>HTML.Tag.SELECT</code>    <td>FormAction
		''' <tr><td><code>HTML.Tag.SMALL</code>     <td>CharacterAction
		''' <tr><td><code>HTML.Tag.STRIKE</code>    <td>CharacterAction
		''' <tr><td><code>HTML.Tag.S</code>         <td>CharacterAction
		''' <tr><td><code>HTML.Tag.STRONG</code>    <td>CharacterAction
		''' <tr><td><code>HTML.Tag.STYLE</code>     <td>StyleAction
		''' <tr><td><code>HTML.Tag.SUB</code>       <td>CharacterAction
		''' <tr><td><code>HTML.Tag.SUP</code>       <td>CharacterAction
		''' <tr><td><code>HTML.Tag.TABLE</code>     <td>BlockAction
		''' <tr><td><code>HTML.Tag.TD</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.TEXTAREA</code>  <td>FormAction
		''' <tr><td><code>HTML.Tag.TH</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.TITLE</code>     <td>TitleAction
		''' <tr><td><code>HTML.Tag.TR</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.TT</code>        <td>CharacterAction
		''' <tr><td><code>HTML.Tag.U</code>         <td>CharacterAction
		''' <tr><td><code>HTML.Tag.UL</code>        <td>BlockAction
		''' <tr><td><code>HTML.Tag.VAR</code>       <td>CharacterAction
		''' </table>
		''' <p>
		''' Once &lt;/html&gt; is encountered, the Actions are no longer notified.
		''' </summary>
		Public Class HTMLReader
			Inherits HTMLEditorKit.ParserCallback

			Private ReadOnly outerInstance As HTMLDocument


			Public Sub New(ByVal outerInstance As HTMLDocument, ByVal offset As Integer)
					Me.outerInstance = outerInstance
				Me.New(offset, 0, 0, Nothing)
			End Sub

			Public Sub New(ByVal outerInstance As HTMLDocument, ByVal offset As Integer, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal insertTag As HTML.Tag)
					Me.outerInstance = outerInstance
				Me.New(offset, popDepth, pushDepth, insertTag, True, False, True)
			End Sub

			''' <summary>
			''' Generates a RuntimeException (will eventually generate
			''' a BadLocationException when API changes are alloced) if inserting
			''' into non empty document, <code>insertTag</code> is
			''' non-<code>null</code>, and <code>offset</code> is not in the body.
			''' </summary>
			' PENDING(sky): Add throws BadLocationException and remove
			' RuntimeException
			Friend Sub New(ByVal outerInstance As HTMLDocument, ByVal offset As Integer, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal insertTag As HTML.Tag, ByVal insertInsertTag As Boolean, ByVal insertAfterImplied As Boolean, ByVal wantsTrailingNewline As Boolean)
					Me.outerInstance = outerInstance
				emptyDocument = (outerInstance.length = 0)
				isStyleCSS = "text/css".Equals(outerInstance.defaultStyleSheetType)
				Me.offset = offset
				threshold = outerInstance.tokenThreshold
				tagMap = New Dictionary(Of HTML.Tag, TagAction)(57)
				Dim na As New TagAction(Me)
				Dim ba As TagAction = New BlockAction(Me)
				Dim pa As TagAction = New ParagraphAction(Me)
				Dim ca As TagAction = New CharacterAction(Me)
				Dim sa As TagAction = New SpecialAction(Me)
				Dim fa As TagAction = New FormAction(Me)
				Dim ha As TagAction = New HiddenAction(Me)
				Dim conv As TagAction = New ConvertAction(Me)

				' register handlers for the well known tags
				tagMap(HTML.Tag.A) = New AnchorAction(Me)
				tagMap(HTML.Tag.ADDRESS) = ca
				tagMap(HTML.Tag.APPLET) = ha
				tagMap(HTML.Tag.AREA) = New AreaAction(Me)
				tagMap(HTML.Tag.B) = conv
				tagMap(HTML.Tag.BASE) = New BaseAction(Me)
				tagMap(HTML.Tag.BASEFONT) = ca
				tagMap(HTML.Tag.BIG) = ca
				tagMap(HTML.Tag.BLOCKQUOTE) = ba
				tagMap(HTML.Tag.BODY) = ba
				tagMap(HTML.Tag.BR) = sa
				tagMap(HTML.Tag.CAPTION) = ba
				tagMap(HTML.Tag.CENTER) = ba
				tagMap(HTML.Tag.CITE) = ca
				tagMap(HTML.Tag.CODE) = ca
				tagMap(HTML.Tag.DD) = ba
				tagMap(HTML.Tag.DFN) = ca
				tagMap(HTML.Tag.DIR) = ba
				tagMap(HTML.Tag.DIV) = ba
				tagMap(HTML.Tag.DL) = ba
				tagMap(HTML.Tag.DT) = pa
				tagMap(HTML.Tag.EM) = ca
				tagMap(HTML.Tag.FONT) = conv
				tagMap(HTML.Tag.FORM) = New FormTagAction(Me)
				tagMap(HTML.Tag.FRAME) = sa
				tagMap(HTML.Tag.FRAMESET) = ba
				tagMap(HTML.Tag.H1) = pa
				tagMap(HTML.Tag.H2) = pa
				tagMap(HTML.Tag.H3) = pa
				tagMap(HTML.Tag.H4) = pa
				tagMap(HTML.Tag.H5) = pa
				tagMap(HTML.Tag.H6) = pa
				tagMap(HTML.Tag.HEAD) = New HeadAction(Me)
				tagMap(HTML.Tag.HR) = sa
				tagMap(HTML.Tag.HTML) = ba
				tagMap(HTML.Tag.I) = conv
				tagMap(HTML.Tag.IMG) = sa
				tagMap(HTML.Tag.INPUT) = fa
				tagMap(HTML.Tag.ISINDEX) = New IsindexAction(Me)
				tagMap(HTML.Tag.KBD) = ca
				tagMap(HTML.Tag.LI) = ba
				tagMap(HTML.Tag.LINK) = New LinkAction(Me)
				tagMap(HTML.Tag.MAP) = New MapAction(Me)
				tagMap(HTML.Tag.MENU) = ba
				tagMap(HTML.Tag.META) = New MetaAction(Me)
				tagMap(HTML.Tag.NOBR) = ca
				tagMap(HTML.Tag.NOFRAMES) = ba
				tagMap(HTML.Tag.OBJECT) = sa
				tagMap(HTML.Tag.OL) = ba
				tagMap(HTML.Tag.OPTION) = fa
				tagMap(HTML.Tag.P) = pa
				tagMap(HTML.Tag.PARAM) = New ObjectAction(Me)
				tagMap(HTML.Tag.PRE) = New PreAction(Me)
				tagMap(HTML.Tag.SAMP) = ca
				tagMap(HTML.Tag.SCRIPT) = ha
				tagMap(HTML.Tag.SELECT) = fa
				tagMap(HTML.Tag.SMALL) = ca
				tagMap(HTML.Tag.SPAN) = ca
				tagMap(HTML.Tag.STRIKE) = conv
				tagMap(HTML.Tag.S) = ca
				tagMap(HTML.Tag.STRONG) = ca
				tagMap(HTML.Tag.STYLE) = New StyleAction(Me)
				tagMap(HTML.Tag.SUB) = conv
				tagMap(HTML.Tag.SUP) = conv
				tagMap(HTML.Tag.TABLE) = ba
				tagMap(HTML.Tag.TD) = ba
				tagMap(HTML.Tag.TEXTAREA) = fa
				tagMap(HTML.Tag.TH) = ba
				tagMap(HTML.Tag.TITLE) = New TitleAction(Me)
				tagMap(HTML.Tag.TR) = ba
				tagMap(HTML.Tag.TT) = ca
				tagMap(HTML.Tag.U) = conv
				tagMap(HTML.Tag.UL) = ba
				tagMap(HTML.Tag.VAR) = ca

				If insertTag IsNot Nothing Then
					Me.insertTag = insertTag
					Me.popDepth = popDepth
					Me.pushDepth = pushDepth
					Me.insertInsertTag = insertInsertTag
					___foundInsertTag = False
				Else
					___foundInsertTag = True
				End If
				If insertAfterImplied Then
					Me.popDepth = popDepth
					Me.pushDepth = pushDepth
					Me.insertAfterImplied = True
					___foundInsertTag = False
					midInsert = False
					Me.insertInsertTag = True
					Me.wantsTrailingNewline = wantsTrailingNewline
				Else
					midInsert = ((Not emptyDocument) AndAlso insertTag Is Nothing)
					If midInsert Then generateEndsSpecsForMidInsert()
				End If

				''' <summary>
				''' This block initializes the <code>inParagraph</code> flag.
				''' It is left in <code>false</code> value automatically
				''' if the target document is empty or future inserts
				''' were positioned into the 'body' tag.
				''' </summary>
				If (Not emptyDocument) AndAlso (Not midInsert) Then
					Dim targetOffset As Integer = Math.Max(Me.offset - 1, 0)
					Dim elem As Element = outerInstance.getCharacterElement(targetOffset)
					' Going up by the left document structure path 
					For i As Integer = 0 To Me.popDepth
						elem = elem.parentElement
					Next i
					' Going down by the right document structure path 
					For i As Integer = 0 To Me.pushDepth - 1
						Dim index As Integer = elem.getElementIndex(Me.offset)
						elem = elem.getElement(index)
					Next i
					Dim attrs As AttributeSet = elem.attributes
					If attrs IsNot Nothing Then
						Dim tagToInsertInto As HTML.Tag = CType(attrs.getAttribute(StyleConstants.NameAttribute), HTML.Tag)
						If tagToInsertInto IsNot Nothing Then Me.inParagraph = tagToInsertInto.paragraph
					End If
				End If
			End Sub

			''' <summary>
			''' Generates an initial batch of end <code>ElementSpecs</code>
			''' in parseBuffer to position future inserts into the body.
			''' </summary>
			Private Sub generateEndsSpecsForMidInsert()
				Dim count As Integer = heightToElementWithName(HTML.Tag.BODY, Math.Max(0, offset - 1))
				Dim joinNext As Boolean = False

				If count = -1 AndAlso offset > 0 Then
					count = heightToElementWithName(HTML.Tag.BODY, offset)
					If count <> -1 Then
						' Previous isn't in body, but current is. Have to
						' do some end specs, followed by join next.
						count = depthTo(offset - 1) - 1
						joinNext = True
					End If
				End If
				If count = -1 Then Throw New Exception("Must insert new content into body element-")
				If count <> -1 Then
					' Insert a newline, if necessary.
					Try
						If (Not joinNext) AndAlso offset > 0 AndAlso (Not outerInstance.getText(offset - 1, 1).Equals(vbLf)) Then
							Dim newAttrs As New SimpleAttributeSet
							newAttrs.addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
							Dim spec As New ElementSpec(newAttrs, ElementSpec.ContentType, NEWLINE, 0, 1)
							parseBuffer.Add(spec)
						End If
						' Should never throw, but will catch anyway.
					Catch ble As BadLocationException
					End Try
					Dim tempVar As Boolean = count > 0
					count -= 1
					Do While tempVar
						parseBuffer.Add(New ElementSpec(Nothing, ElementSpec.EndTagType))
						tempVar = count > 0
						count -= 1
					Loop
					If joinNext Then
						Dim spec As New ElementSpec(Nothing, ElementSpec.StartTagType)

						spec.direction = ElementSpec.JoinNextDirection
						parseBuffer.Add(spec)
					End If
				End If
				' We should probably throw an exception if (count == -1)
				' Or look for the body and reset the offset.
			End Sub

			''' <returns> number of parents to reach the child at offset. </returns>
			Private Function depthTo(ByVal offset As Integer) As Integer
				Dim e As Element = outerInstance.defaultRootElement
				Dim count As Integer = 0

				Do While Not e.leaf
					count += 1
					e = e.getElement(e.getElementIndex(offset))
				Loop
				Return count
			End Function

			''' <returns> number of parents of the leaf at <code>offset</code>
			'''         until a parent with name, <code>name</code> has been
			'''         found. -1 indicates no matching parent with
			'''         <code>name</code>. </returns>
			Private Function heightToElementWithName(ByVal name As Object, ByVal offset As Integer) As Integer
				Dim e As Element = outerInstance.getCharacterElement(offset).parentElement
				Dim count As Integer = 0

				Do While e IsNot Nothing AndAlso e.attributes.getAttribute(StyleConstants.NameAttribute) IsNot name
					count += 1
					e = e.parentElement
				Loop
				Return If(e Is Nothing, -1, count)
			End Function

			''' <summary>
			''' This will make sure there aren't two BODYs (the second is
			''' typically created when you do a remove all, and then an insert).
			''' </summary>
			Private Sub adjustEndElement()
				Dim length As Integer = outerInstance.length
				If length = 0 Then Return
				outerInstance.obtainLock()
				Try
					Dim pPath As Element() = getPathTo(length - 1)
					Dim pLength As Integer = pPath.Length
					If pLength > 1 AndAlso pPath(1).attributes.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.BODY AndAlso pPath(1).endOffset = length Then
						Dim lastText As String = outerInstance.getText(length - 1, 1)
						Dim [event] As DefaultDocumentEvent
						Dim added As Element()
						Dim removed As Element()
						Dim index As Integer
						' Remove the fake second body.
						added = New Element(){}
						removed = New Element(0){}
						index = pPath(0).getElementIndex(length)
						removed(0) = pPath(0).getElement(index)
						CType(pPath(0), BranchElement).replace(index, 1, added)
						Dim firstEdit As New ElementEdit(pPath(0), index, removed, added)

						' Insert a new element to represent the end that the
						' second body was representing.
						Dim sas As New SimpleAttributeSet
						sas.addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
						sas.addAttribute(IMPLIED_CR, Boolean.TRUE)
						added = New Element(0){}
						added(0) = outerInstance.createLeafElement(pPath(pLength - 1), sas, length, length + 1)
						index = pPath(pLength - 1).elementCount
						CType(pPath(pLength - 1), BranchElement).replace(index, 0, added)
						[event] = New DefaultDocumentEvent(length, 1, DocumentEvent.EventType.CHANGE)
						[event].addEdit(New ElementEdit(pPath(pLength - 1), index, New Element(){}, added))
						[event].addEdit(firstEdit)
						[event].end()
						outerInstance.fireChangedUpdate([event])
						outerInstance.fireUndoableEditUpdate(New UndoableEditEvent(Me, [event]))

						If lastText.Equals(vbLf) Then
							' We now have two \n's, one part of the Document.
							' We need to remove one
							[event] = New DefaultDocumentEvent(length - 1, 1, DocumentEvent.EventType.REMOVE)
							outerInstance.removeUpdate([event])
							Dim u As UndoableEdit = outerInstance.content.remove(length - 1, 1)
							If u IsNot Nothing Then [event].addEdit(u)
							outerInstance.postRemoveUpdate([event])
							' Mark the edit as done.
							[event].end()
							outerInstance.fireRemoveUpdate([event])
							outerInstance.fireUndoableEditUpdate(New UndoableEditEvent(Me, [event]))
						End If
					End If
				Catch ble As BadLocationException
				Finally
					outerInstance.releaseLock()
				End Try
			End Sub

			Private Function getPathTo(ByVal offset As Integer) As Element()
				Dim elements As New Stack(Of Element)
				Dim e As Element = outerInstance.defaultRootElement
				Dim index As Integer
				Do While Not e.leaf
					elements.Push(e)
					e = e.getElement(e.getElementIndex(offset))
				Loop
				Dim retValue As Element() = New Element(elements.Count - 1){}
				elements.copyInto(retValue)
				Return retValue
			End Function

			' -- HTMLEditorKit.ParserCallback methods --------------------

			''' <summary>
			''' The last method called on the reader.  It allows
			''' any pending changes to be flushed into the document.
			''' Since this is currently loading synchronously, the entire
			''' set of changes are pushed in at this point.
			''' </summary>
			Public Overridable Sub flush()
				If emptyDocument AndAlso (Not insertAfterImplied) Then
					If outerInstance.length > 0 OrElse parseBuffer.Count > 0 Then
						flushBuffer(True)
						adjustEndElement()
					End If
					' We won't insert when
				Else
					flushBuffer(True)
				End If
			End Sub

			''' <summary>
			''' Called by the parser to indicate a block of text was
			''' encountered.
			''' </summary>
			Public Overridable Sub handleText(ByVal data As Char(), ByVal pos As Integer)
				If receivedEndHTML OrElse (midInsert AndAlso (Not inBody)) Then Return

				' see if complex glyph layout support is needed
				If outerInstance.getProperty(I18NProperty).Equals(Boolean.FALSE) Then
					' if a default direction of right-to-left has been specified,
					' we want complex layout even if the text is all left to right.
					Dim d As Object = outerInstance.getProperty(java.awt.font.TextAttribute.RUN_DIRECTION)
					If (d IsNot Nothing) AndAlso (d.Equals(java.awt.font.TextAttribute.RUN_DIRECTION_RTL)) Then
						outerInstance.putProperty(I18NProperty, Boolean.TRUE)
					Else
						If sun.swing.SwingUtilities2.isComplexLayout(data, 0, data.Length) Then outerInstance.putProperty(I18NProperty, Boolean.TRUE)
					End If
				End If

				If inTextArea Then
					textAreaContent(data)
				ElseIf inPre Then
					preContent(data)
				ElseIf inTitle Then
					outerInstance.putProperty(Document.TitleProperty, New String(data))
				ElseIf [option] IsNot Nothing Then
					[option].label = New String(data)
				ElseIf inStyle Then
					If styles IsNot Nothing Then styles.Add(New String(data))
				ElseIf inBlock > 0 Then
					If (Not ___foundInsertTag) AndAlso insertAfterImplied Then
						' Assume content should be added.
						foundInsertTag(False)
						___foundInsertTag = True
						' If content is added directly to the body, it should
						' be wrapped by p-implied.
							impliedP = Not outerInstance.insertInBody
							inParagraph = impliedP
					End If
					If data.Length >= 1 Then addContent(data, 0, data.Length)
				End If
			End Sub

			''' <summary>
			''' Callback from the parser.  Route to the appropriate
			''' handler for the tag.
			''' </summary>
			Public Overridable Sub handleStartTag(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet, ByVal pos As Integer)
				If receivedEndHTML Then Return
				If midInsert AndAlso (Not inBody) Then
					If t Is HTML.Tag.BODY Then
						inBody = True
						' Increment inBlock since we know we are in the body,
						' this is needed incase an implied-p is needed. If
						' inBlock isn't incremented, and an implied-p is
						' encountered, addContent won't be called!
						inBlock += 1
					End If
					Return
				End If
				If (Not inBody) AndAlso t Is HTML.Tag.BODY Then inBody = True
				If isStyleCSS AndAlso a.isDefined(HTML.Attribute.STYLE) Then
					' Map the style attributes.
					Dim decl As String = CStr(a.getAttribute(HTML.Attribute.STYLE))
					a.removeAttribute(HTML.Attribute.STYLE)
					styleAttributes = outerInstance.styleSheet.getDeclaration(decl)
					a.addAttributes(styleAttributes)
				Else
					styleAttributes = Nothing
				End If
				Dim action As TagAction = tagMap(t)

				If action IsNot Nothing Then action.start(t, a)
			End Sub

			Public Overridable Sub handleComment(ByVal data As Char(), ByVal pos As Integer)
				If receivedEndHTML Then
					addExternalComment(New String(data))
					Return
				End If
				If inStyle Then
					If styles IsNot Nothing Then styles.Add(New String(data))
				ElseIf outerInstance.preservesUnknownTags Then
					If inBlock = 0 AndAlso (___foundInsertTag OrElse insertTag IsNot HTML.Tag.COMMENT) Then
						' Comment outside of body, will not be able to show it,
						' but can add it as a property on the Document.
						addExternalComment(New String(data))
						Return
					End If
					Dim sas As New SimpleAttributeSet
					sas.addAttribute(HTML.Attribute.COMMENT, New String(data))
					addSpecialElement(HTML.Tag.COMMENT, sas)
				End If

				Dim action As TagAction = tagMap(HTML.Tag.COMMENT)
				If action IsNot Nothing Then
					action.start(HTML.Tag.COMMENT, New SimpleAttributeSet)
					action.end(HTML.Tag.COMMENT)
				End If
			End Sub

			''' <summary>
			''' Adds the comment <code>comment</code> to the set of comments
			''' maintained outside of the scope of elements.
			''' </summary>
			Private Sub addExternalComment(ByVal comment As String)
				Dim comments As Object = outerInstance.getProperty(AdditionalComments)
				If comments IsNot Nothing AndAlso Not(TypeOf comments Is ArrayList) Then Return
				If comments Is Nothing Then
					comments = New ArrayList
					outerInstance.putProperty(AdditionalComments, comments)
				End If
				CType(comments, ArrayList).Add(comment)
			End Sub

			''' <summary>
			''' Callback from the parser.  Route to the appropriate
			''' handler for the tag.
			''' </summary>
			Public Overridable Sub handleEndTag(ByVal t As HTML.Tag, ByVal pos As Integer)
				If receivedEndHTML OrElse (midInsert AndAlso (Not inBody)) Then Return
				If t Is HTML.Tag.HTML Then receivedEndHTML = True
				If t Is HTML.Tag.BODY Then
					inBody = False
					If midInsert Then inBlock -= 1
				End If
				Dim action As TagAction = tagMap(t)
				If action IsNot Nothing Then action.end(t)
			End Sub

			''' <summary>
			''' Callback from the parser.  Route to the appropriate
			''' handler for the tag.
			''' </summary>
			Public Overridable Sub handleSimpleTag(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet, ByVal pos As Integer)
				If receivedEndHTML OrElse (midInsert AndAlso (Not inBody)) Then Return

				If isStyleCSS AndAlso a.isDefined(HTML.Attribute.STYLE) Then
					' Map the style attributes.
					Dim decl As String = CStr(a.getAttribute(HTML.Attribute.STYLE))
					a.removeAttribute(HTML.Attribute.STYLE)
					styleAttributes = outerInstance.styleSheet.getDeclaration(decl)
					a.addAttributes(styleAttributes)
				Else
					styleAttributes = Nothing
				End If

				Dim action As TagAction = tagMap(t)
				If action IsNot Nothing Then
					action.start(t, a)
					action.end(t)
				ElseIf outerInstance.preservesUnknownTags Then
					' unknown tag, only add if should preserve it.
					addSpecialElement(t, a)
				End If
			End Sub

			''' <summary>
			''' This is invoked after the stream has been parsed, but before
			''' <code>flush</code>. <code>eol</code> will be one of \n, \r
			''' or \r\n, which ever is encountered the most in parsing the
			''' stream.
			''' 
			''' @since 1.3
			''' </summary>
			Public Overridable Sub handleEndOfLineString(ByVal eol As String)
				If emptyDocument AndAlso eol IsNot Nothing Then outerInstance.putProperty(DefaultEditorKit.EndOfLineStringProperty, eol)
			End Sub

			' ---- tag handling support ------------------------------

			''' <summary>
			''' Registers a handler for the given tag.  By default
			''' all of the well-known tags will have been registered.
			''' This can be used to change the handling of a particular
			''' tag or to add support for custom tags.
			''' </summary>
			Protected Friend Overridable Sub registerTag(ByVal t As HTML.Tag, ByVal a As TagAction)
				tagMap(t) = a
			End Sub

			''' <summary>
			''' An action to be performed in response
			''' to parsing a tag.  This allows customization
			''' of how each tag is handled and avoids a large
			''' switch statement.
			''' </summary>
			Public Class TagAction
				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				''' <summary>
				''' Called when a start tag is seen for the
				''' type of tag this action was registered
				''' to.  The tag argument indicates the actual
				''' tag for those actions that are shared across
				''' many tags.  By default this does nothing and
				''' completely ignores the tag.
				''' </summary>
				Public Overridable Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
				End Sub

				''' <summary>
				''' Called when an end tag is seen for the
				''' type of tag this action was registered
				''' to.  The tag argument indicates the actual
				''' tag for those actions that are shared across
				''' many tags.  By default this does nothing and
				''' completely ignores the tag.
				''' </summary>
				Public Overridable Sub [end](ByVal t As HTML.Tag)
				End Sub

			End Class

			Public Class BlockAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					outerInstance.blockOpen(t, attr)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.blockClose(t)
				End Sub
			End Class


			''' <summary>
			''' Action used for the actual element form tag. This is named such
			''' as there was already a public class named FormAction.
			''' </summary>
			Private Class FormTagAction
				Inherits BlockAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub

				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					MyBase.start(t, attr)
					' initialize a ButtonGroupsMap when
					' FORM tag is encountered.  This will
					' be used for any radio buttons that
					' might be defined in the FORM.
					' for new group new ButtonGroup will be created (fix for 4529702)
					' group name is a key in radioButtonGroupsMap
					radioButtonGroupsMap = New Dictionary(Of String, ButtonGroup)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					MyBase.end(t)
					' reset the button group to null since
					' the form has ended.
					radioButtonGroupsMap = Nothing
				End Sub
			End Class


			Public Class ParagraphAction
				Inherits BlockAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					MyBase.start(t, a)
					outerInstance.inParagraph = True
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					MyBase.end(t)
					outerInstance.inParagraph = False
				End Sub
			End Class

			Public Class SpecialAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					outerInstance.addSpecialElement(t, a)
				End Sub

			End Class

			Public Class IsindexAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					outerInstance.blockOpen(HTML.Tag.IMPLIED, New SimpleAttributeSet)
					outerInstance.addSpecialElement(t, a)
					outerInstance.blockClose(HTML.Tag.IMPLIED)
				End Sub

			End Class


			Public Class HiddenAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					outerInstance.addSpecialElement(t, a)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					If Not isEmpty(t) Then
						Dim a As MutableAttributeSet = New SimpleAttributeSet
						a.addAttribute(HTML.Attribute.ENDTAG, "true")
						outerInstance.addSpecialElement(t, a)
					End If
				End Sub

				Friend Overridable Function isEmpty(ByVal t As HTML.Tag) As Boolean
					If t Is HTML.Tag.APPLET OrElse t Is HTML.Tag.SCRIPT Then Return False
					Return True
				End Function
			End Class


			''' <summary>
			''' Subclass of HiddenAction to set the content type for style sheets,
			''' and to set the name of the default style sheet.
			''' </summary>
			Friend Class MetaAction
				Inherits HiddenAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					Dim equiv As Object = a.getAttribute(HTML.Attribute.HTTPEQUIV)
					If equiv IsNot Nothing Then
						equiv = CStr(equiv).ToLower()
						If equiv.Equals("content-style-type") Then
							Dim value As String = CStr(a.getAttribute(HTML.Attribute.CONTENT))
							defaultStyleSheetType = value
							outerInstance.isStyleCSS = "text/css".Equals(defaultStyleSheetType)
						ElseIf equiv.Equals("default-style") Then
							outerInstance.defaultStyle = CStr(a.getAttribute(HTML.Attribute.CONTENT))
						End If
					End If
					MyBase.start(t, a)
				End Sub

				Friend Overrides Function isEmpty(ByVal t As HTML.Tag) As Boolean
					Return True
				End Function
			End Class


			''' <summary>
			''' End if overridden to create the necessary stylesheets that
			''' are referenced via the link tag. It is done in this manner
			''' as the meta tag can be used to specify an alternate style sheet,
			''' and is not guaranteed to come before the link tags.
			''' </summary>
			Friend Class HeadAction
				Inherits BlockAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					outerInstance.inHead = True
					' This check of the insertTag is put in to avoid considering
					' the implied-p that is generated for the head. This allows
					' inserts for HR to work correctly.
					If (outerInstance.insertTag Is Nothing AndAlso (Not outerInstance.insertAfterImplied)) OrElse (outerInstance.insertTag Is HTML.Tag.HEAD) OrElse (outerInstance.insertAfterImplied AndAlso (outerInstance.___foundInsertTag OrElse (Not a.isDefined(IMPLIED)))) Then MyBase.start(t, a)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
						outerInstance.inStyle = False
						outerInstance.inHead = outerInstance.inStyle
					' See if there is a StyleSheet to link to.
					If outerInstance.styles IsNot Nothing Then
						Dim isDefaultCSS As Boolean = outerInstance.isStyleCSS
						Dim counter As Integer = 0
						Dim maxCounter As Integer = outerInstance.styles.Count
						Do While counter < maxCounter
							Dim value As Object = outerInstance.styles(counter)
							If value Is HTML.Tag.LINK Then
								counter += 1
								handleLink(CType(outerInstance.styles(counter), AttributeSet))
								counter += 1
							Else
								' Rule.
								' First element gives type.
								counter += 1
								Dim type As String = CStr(outerInstance.styles(counter))
								Dim isCSS As Boolean = If(type Is Nothing, isDefaultCSS, type.Equals("text/css"))
								counter += 1
								Do While counter < maxCounter AndAlso (TypeOf outerInstance.styles(counter) Is String)
									If isCSS Then outerInstance.addCSSRules(CStr(outerInstance.styles(counter)))
									counter += 1
								Loop
							End If
						Loop
					End If
					If (outerInstance.insertTag Is Nothing AndAlso (Not outerInstance.insertAfterImplied)) OrElse outerInstance.insertTag Is HTML.Tag.HEAD OrElse (outerInstance.insertAfterImplied AndAlso outerInstance.___foundInsertTag) Then MyBase.end(t)
				End Sub

				Friend Overridable Function isEmpty(ByVal t As HTML.Tag) As Boolean
					Return False
				End Function

				Private Sub handleLink(ByVal attr As AttributeSet)
					' Link.
					Dim type As String = CStr(attr.getAttribute(HTML.Attribute.TYPE))
					If type Is Nothing Then type = defaultStyleSheetType
					' Only choose if type==text/css
					' Select link if rel==stylesheet.
					' Otherwise if rel==alternate stylesheet and
					'   title matches default style.
					If type.Equals("text/css") Then
						Dim rel As String = CStr(attr.getAttribute(HTML.Attribute.REL))
						Dim title As String = CStr(attr.getAttribute(HTML.Attribute.TITLE))
						Dim media As String = CStr(attr.getAttribute(HTML.Attribute.MEDIA))
						If media Is Nothing Then
							media = "all"
						Else
							media = media.ToLower()
						End If
						If rel IsNot Nothing Then
							rel = rel.ToLower()
							If (media.IndexOf("all") <> -1 OrElse media.IndexOf("screen") <> -1) AndAlso (rel.Equals("stylesheet") OrElse (rel.Equals("alternate stylesheet") AndAlso title.Equals(outerInstance.defaultStyle))) Then outerInstance.linkCSSStyleSheet(CStr(attr.getAttribute(HTML.Attribute.HREF)))
						End If
					End If
				End Sub
			End Class


			''' <summary>
			''' A subclass to add the AttributeSet to styles if the
			''' attributes contains an attribute for 'rel' with value
			''' 'stylesheet' or 'alternate stylesheet'.
			''' </summary>
			Friend Class LinkAction
				Inherits HiddenAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					Dim rel As String = CStr(a.getAttribute(HTML.Attribute.REL))
					If rel IsNot Nothing Then
						rel = rel.ToLower()
						If rel.Equals("stylesheet") OrElse rel.Equals("alternate stylesheet") Then
							If outerInstance.styles Is Nothing Then outerInstance.styles = New List(Of Object)(3)
							outerInstance.styles.Add(t)
							outerInstance.styles.Add(a.copyAttributes())
						End If
					End If
					MyBase.start(t, a)
				End Sub
			End Class

			Friend Class MapAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					outerInstance.lastMap = New Map(CStr(a.getAttribute(HTML.Attribute.NAME)))
					addMap(outerInstance.lastMap)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
				End Sub
			End Class


			Friend Class AreaAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					If outerInstance.lastMap IsNot Nothing Then outerInstance.lastMap.addArea(a.copyAttributes())
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
				End Sub
			End Class


			Friend Class StyleAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					If outerInstance.inHead Then
						If outerInstance.styles Is Nothing Then outerInstance.styles = New List(Of Object)(3)
						outerInstance.styles.Add(t)
						outerInstance.styles.Add(a.getAttribute(HTML.Attribute.TYPE))
						outerInstance.inStyle = True
					End If
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.inStyle = False
				End Sub

				Friend Overridable Function isEmpty(ByVal t As HTML.Tag) As Boolean
					Return False
				End Function
			End Class


			Public Class PreAction
				Inherits BlockAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					outerInstance.inPre = True
					outerInstance.blockOpen(t, attr)
					attr.addAttribute(CSS.Attribute.WHITE_SPACE, "pre")
					outerInstance.blockOpen(HTML.Tag.IMPLIED, attr)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.blockClose(HTML.Tag.IMPLIED)
					' set inPre to false after closing, so that if a newline
					' is added it won't generate a blockOpen.
					outerInstance.inPre = False
					outerInstance.blockClose(t)
				End Sub
			End Class

			Public Class CharacterAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					outerInstance.pushCharacterStyle()
					If Not outerInstance.___foundInsertTag Then
						' Note that the third argument should really be based off
						' inParagraph and impliedP. If we're wrong (that is
						' insertTagDepthDelta shouldn't be changed), we'll end up
						' removing an extra EndSpec, which won't matter anyway.
						Dim insert As Boolean = outerInstance.canInsertTag(t, attr, False)
						If outerInstance.___foundInsertTag Then
							If Not outerInstance.inParagraph Then
									outerInstance.impliedP = True
									outerInstance.inParagraph = outerInstance.impliedP
							End If
						End If
						If Not insert Then Return
					End If
					If attr.isDefined(IMPLIED) Then attr.removeAttribute(IMPLIED)
					outerInstance.charAttr.addAttribute(t, attr.copyAttributes())
					If outerInstance.styleAttributes IsNot Nothing Then outerInstance.charAttr.addAttributes(outerInstance.styleAttributes)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.popCharacterStyle()
				End Sub
			End Class

			''' <summary>
			''' Provides conversion of HTML tag/attribute
			''' mappings that have a corresponding StyleConstants
			''' and CSS mapping.  The conversion is to CSS attributes.
			''' </summary>
			Friend Class ConvertAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					outerInstance.pushCharacterStyle()
					If Not outerInstance.___foundInsertTag Then
						' Note that the third argument should really be based off
						' inParagraph and impliedP. If we're wrong (that is
						' insertTagDepthDelta shouldn't be changed), we'll end up
						' removing an extra EndSpec, which won't matter anyway.
						Dim insert As Boolean = outerInstance.canInsertTag(t, attr, False)
						If outerInstance.___foundInsertTag Then
							If Not outerInstance.inParagraph Then
									outerInstance.impliedP = True
									outerInstance.inParagraph = outerInstance.impliedP
							End If
						End If
						If Not insert Then Return
					End If
					If attr.isDefined(IMPLIED) Then attr.removeAttribute(IMPLIED)
					If outerInstance.styleAttributes IsNot Nothing Then outerInstance.charAttr.addAttributes(outerInstance.styleAttributes)
					' We also need to add attr, otherwise we lose custom
					' attributes, including class/id for style lookups, and
					' further confuse style lookup (doesn't have tag).
					outerInstance.charAttr.addAttribute(t, attr.copyAttributes())
					Dim sheet As StyleSheet = styleSheet
					If t Is HTML.Tag.B Then
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.FONT_WEIGHT, "bold")
					ElseIf t Is HTML.Tag.I Then
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.FONT_STYLE, "italic")
					ElseIf t Is HTML.Tag.U Then
						Dim v As Object = outerInstance.charAttr.getAttribute(CSS.Attribute.TEXT_DECORATION)
						Dim value As String = "underline"
						value = If(v IsNot Nothing, value & "," & v.ToString(), value)
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.TEXT_DECORATION, value)
					ElseIf t Is HTML.Tag.STRIKE Then
						Dim v As Object = outerInstance.charAttr.getAttribute(CSS.Attribute.TEXT_DECORATION)
						Dim value As String = "line-through"
						value = If(v IsNot Nothing, value & "," & v.ToString(), value)
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.TEXT_DECORATION, value)
					ElseIf t Is HTML.Tag.SUP Then
						Dim v As Object = outerInstance.charAttr.getAttribute(CSS.Attribute.VERTICAL_ALIGN)
						Dim value As String = "sup"
						value = If(v IsNot Nothing, value & "," & v.ToString(), value)
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.VERTICAL_ALIGN, value)
					ElseIf t Is HTML.Tag.SUB Then
						Dim v As Object = outerInstance.charAttr.getAttribute(CSS.Attribute.VERTICAL_ALIGN)
						Dim value As String = "sub"
						value = If(v IsNot Nothing, value & "," & v.ToString(), value)
						sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.VERTICAL_ALIGN, value)
					ElseIf t Is HTML.Tag.FONT Then
						Dim color As String = CStr(attr.getAttribute(HTML.Attribute.COLOR))
						If color IsNot Nothing Then sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.COLOR, color)
						Dim face As String = CStr(attr.getAttribute(HTML.Attribute.FACE))
						If face IsNot Nothing Then sheet.addCSSAttribute(outerInstance.charAttr, CSS.Attribute.FONT_FAMILY, face)
						Dim size As String = CStr(attr.getAttribute(HTML.Attribute.SIZE))
						If size IsNot Nothing Then sheet.addCSSAttributeFromHTML(outerInstance.charAttr, CSS.Attribute.FONT_SIZE, size)
					End If
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.popCharacterStyle()
				End Sub

			End Class

			Friend Class AnchorAction
				Inherits CharacterAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					' set flag to catch empty anchors
					outerInstance.emptyAnchor = True
					MyBase.start(t, attr)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					If outerInstance.emptyAnchor Then
						' if the anchor was empty it was probably a
						' named anchor point and we don't want to throw
						' it away.
						Dim one As Char() = New Char(0){}
						one(0) = ControlChars.Lf
						outerInstance.addContent(one, 0, 1)
					End If
					MyBase.end(t)
				End Sub
			End Class

			Friend Class TitleAction
				Inherits HiddenAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					outerInstance.inTitle = True
					MyBase.start(t, attr)
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					outerInstance.inTitle = False
					MyBase.end(t)
				End Sub

				Friend Overrides Function isEmpty(ByVal t As HTML.Tag) As Boolean
					Return False
				End Function
			End Class


			Friend Class BaseAction
				Inherits TagAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					Dim href As String = CStr(attr.getAttribute(HTML.Attribute.HREF))
					If href IsNot Nothing Then
						Try
							Dim newBase As New java.net.URL(base, href)
							base = newBase
							hasBaseTag = True
						Catch ex As java.net.MalformedURLException
						End Try
					End If
					baseTarget = CStr(attr.getAttribute(HTML.Attribute.TARGET))
				End Sub
			End Class

			Friend Class ObjectAction
				Inherits SpecialAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
					If t Is HTML.Tag.PARAM Then
						addParameter(a)
					Else
						MyBase.start(t, a)
					End If
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					If t IsNot HTML.Tag.PARAM Then MyBase.end(t)
				End Sub

				Friend Overridable Sub addParameter(ByVal a As AttributeSet)
					Dim name As String = CStr(a.getAttribute(HTML.Attribute.NAME))
					Dim value As String = CStr(a.getAttribute(HTML.Attribute.VALUE))
					If (name IsNot Nothing) AndAlso (value IsNot Nothing) Then
						Dim objSpec As ElementSpec = outerInstance.parseBuffer(outerInstance.parseBuffer.Count - 1)
						Dim objAttr As MutableAttributeSet = CType(objSpec.attributes, MutableAttributeSet)
						objAttr.addAttribute(name, value)
					End If
				End Sub
			End Class

			''' <summary>
			''' Action to support forms by building all of the elements
			''' used to represent form controls.  This will process
			''' the &lt;INPUT&gt;, &lt;TEXTAREA&gt;, &lt;SELECT&gt;,
			''' and &lt;OPTION&gt; tags.  The element created by
			''' this action is expected to have the attribute
			''' <code>StyleConstants.ModelAttribute</code> set to
			''' the model that holds the state for the form control.
			''' This enables multiple views, and allows document to
			''' be iterated over picking up the data of the form.
			''' The following are the model assignments for the
			''' various type of form elements.
			''' <table summary="model assignments for the various types of form elements">
			''' <tr>
			'''   <th>Element Type
			'''   <th>Model Type
			''' <tr>
			'''   <td>input, type button
			'''   <td><seealso cref="DefaultButtonModel"/>
			''' <tr>
			'''   <td>input, type checkbox
			'''   <td><seealso cref="javax.swing.JToggleButton.ToggleButtonModel"/>
			''' <tr>
			'''   <td>input, type image
			'''   <td><seealso cref="DefaultButtonModel"/>
			''' <tr>
			'''   <td>input, type password
			'''   <td><seealso cref="PlainDocument"/>
			''' <tr>
			'''   <td>input, type radio
			'''   <td><seealso cref="javax.swing.JToggleButton.ToggleButtonModel"/>
			''' <tr>
			'''   <td>input, type reset
			'''   <td><seealso cref="DefaultButtonModel"/>
			''' <tr>
			'''   <td>input, type submit
			'''   <td><seealso cref="DefaultButtonModel"/>
			''' <tr>
			'''   <td>input, type text or type is null.
			'''   <td><seealso cref="PlainDocument"/>
			''' <tr>
			'''   <td>select
			'''   <td><seealso cref="DefaultComboBoxModel"/> or an <seealso cref="DefaultListModel"/>, with an item type of Option
			''' <tr>
			'''   <td>textarea
			'''   <td><seealso cref="PlainDocument"/>
			''' </table>
			''' 
			''' </summary>
			Public Class FormAction
				Inherits SpecialAction

				Private ReadOnly outerInstance As HTMLDocument.HTMLReader

				Public Sub New(ByVal outerInstance As HTMLDocument.HTMLReader)
					Me.outerInstance = outerInstance
				End Sub


				Public Overrides Sub start(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
					If t Is HTML.Tag.INPUT Then
						Dim type As String = CStr(attr.getAttribute(HTML.Attribute.TYPE))
	'                    
	'                     * if type is not defined the default is
	'                     * assumed to be text.
	'                     
						If type Is Nothing Then
							type = "text"
							attr.addAttribute(HTML.Attribute.TYPE, "text")
						End If
						modeldel(type, attr)
					ElseIf t Is HTML.Tag.TEXTAREA Then
						outerInstance.inTextArea = True
						outerInstance.textAreaDocument = New TextAreaDocument
						attr.addAttribute(StyleConstants.ModelAttribute, outerInstance.textAreaDocument)
					ElseIf t Is HTML.Tag.SELECT Then
						Dim size As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.SIZE, 1)
						Dim multiple As Boolean = attr.getAttribute(HTML.Attribute.MULTIPLE) IsNot Nothing
						If (size > 1) OrElse multiple Then
							Dim m As New OptionListModel(Of [Option])
							If multiple Then m.selectionMode = ListSelectionModel.MULTIPLE_INTERVAL_SELECTION
							selectModel = m
						Else
							selectModel = New OptionComboBoxModel(Of [Option])
						End If
						attr.addAttribute(StyleConstants.ModelAttribute, selectModel)

					End If

					' build the element, unless this is an option.
					If t Is HTML.Tag.OPTION Then
						outerInstance.option = New [Option](attr)

						If TypeOf selectModel Is OptionListModel Then
							Dim m As OptionListModel(Of [Option]) = CType(selectModel, OptionListModel(Of [Option]))
							m.addElement(outerInstance.option)
							If outerInstance.option.selected Then
								m.addSelectionInterval(optionCount, optionCount)
								m.initialSelection = optionCount
							End If
						ElseIf TypeOf selectModel Is OptionComboBoxModel Then
							Dim m As OptionComboBoxModel(Of [Option]) = CType(selectModel, OptionComboBoxModel(Of [Option]))
							m.addElement(outerInstance.option)
							If outerInstance.option.selected Then
								m.selectedItem = outerInstance.option
								m.initialSelection = outerInstance.option
							End If
						End If
						optionCount += 1
					Else
						MyBase.start(t, attr)
					End If
				End Sub

				Public Overrides Sub [end](ByVal t As HTML.Tag)
					If t Is HTML.Tag.OPTION Then
						outerInstance.option = Nothing
					Else
						If t Is HTML.Tag.SELECT Then
							selectModel = Nothing
							optionCount = 0
						ElseIf t Is HTML.Tag.TEXTAREA Then
							outerInstance.inTextArea = False

	'                         Now that the textarea has ended,
	'                         * store the entire initial text
	'                         * of the text area.  This will
	'                         * enable us to restore the initial
	'                         * state if a reset is requested.
	'                         
							outerInstance.textAreaDocument.storeInitialText()
						End If
						MyBase.end(t)
					End If
				End Sub

				Friend Overridable Sub setModel(ByVal type As String, ByVal attr As MutableAttributeSet)
					If type.Equals("submit") OrElse type.Equals("reset") OrElse type.Equals("image") Then

						' button model
						attr.addAttribute(StyleConstants.ModelAttribute, New DefaultButtonModel)
					ElseIf type.Equals("text") OrElse type.Equals("password") Then
						' plain text model
						Dim maxLength As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.MAXLENGTH, -1)
						Dim doc As Document

						If maxLength > 0 Then
							doc = New FixedLengthDocument(maxLength)
						Else
							doc = New PlainDocument
						End If
						Dim value As String = CStr(attr.getAttribute(HTML.Attribute.VALUE))
						Try
							doc.insertString(0, value, Nothing)
						Catch e As BadLocationException
						End Try
						attr.addAttribute(StyleConstants.ModelAttribute, doc)
					ElseIf type.Equals("file") Then
						' plain text model
						attr.addAttribute(StyleConstants.ModelAttribute, New PlainDocument)
					ElseIf type.Equals("checkbox") OrElse type.Equals("radio") Then
						Dim ___model As New JToggleButton.ToggleButtonModel
						If type.Equals("radio") Then
							Dim name As String = CStr(attr.getAttribute(HTML.Attribute.NAME))
							If radioButtonGroupsMap Is Nothing Then 'fix for 4772743 radioButtonGroupsMap = New Dictionary(Of String, ButtonGroup)
							Dim radioButtonGroup As ButtonGroup = radioButtonGroupsMap.get(name)
							If radioButtonGroup Is Nothing Then
								radioButtonGroup = New ButtonGroup
								radioButtonGroupsMap.put(name,radioButtonGroup)
							End If
							___model.group = radioButtonGroup
						End If
						Dim checked As Boolean = (attr.getAttribute(HTML.Attribute.CHECKED) IsNot Nothing)
						___model.selected = checked
						attr.addAttribute(StyleConstants.ModelAttribute, ___model)
					End If
				End Sub

				''' <summary>
				''' If a &lt;SELECT&gt; tag is being processed, this
				''' model will be a reference to the model being filled
				''' with the &lt;OPTION&gt; elements (which produce
				''' objects of type <code>Option</code>.
				''' </summary>
				Friend selectModel As Object
				Friend optionCount As Integer
			End Class


			' --- utility methods used by the reader ------------------

			''' <summary>
			''' Pushes the current character style on a stack in preparation
			''' for forming a new nested character style.
			''' </summary>
			Protected Friend Overridable Sub pushCharacterStyle()
				charAttrStack.Push(charAttr.copyAttributes())
			End Sub

			''' <summary>
			''' Pops a previously pushed character style off the stack
			''' to return to a previous style.
			''' </summary>
			Protected Friend Overridable Sub popCharacterStyle()
				If charAttrStack.Count > 0 Then
					charAttr = CType(charAttrStack.Peek(), MutableAttributeSet)
					charAttrStack.Pop()
				End If
			End Sub

			''' <summary>
			''' Adds the given content to the textarea document.
			''' This method gets called when we are in a textarea
			''' context.  Therefore all text that is seen belongs
			''' to the text area and is hence added to the
			''' TextAreaDocument associated with the text area.
			''' </summary>
			Protected Friend Overridable Sub textAreaContent(ByVal data As Char())
				Try
					textAreaDocument.insertString(textAreaDocument.length, New String(data), Nothing)
				Catch e As BadLocationException
					' Should do something reasonable
				End Try
			End Sub

			''' <summary>
			''' Adds the given content that was encountered in a
			''' PRE element.  This synthesizes lines to hold the
			''' runs of text, and makes calls to addContent to
			''' actually add the text.
			''' </summary>
			Protected Friend Overridable Sub preContent(ByVal data As Char())
				Dim last As Integer = 0
				For i As Integer = 0 To data.Length - 1
					If data(i) = ControlChars.Lf Then
						addContent(data, last, i - last + 1)
						blockClose(HTML.Tag.IMPLIED)
						Dim a As MutableAttributeSet = New SimpleAttributeSet
						a.addAttribute(CSS.Attribute.WHITE_SPACE, "pre")
						blockOpen(HTML.Tag.IMPLIED, a)
						last = i + 1
					End If
				Next i
				If last < data.Length Then addContent(data, last, data.Length - last)
			End Sub

			''' <summary>
			''' Adds an instruction to the parse buffer to create a
			''' block element with the given attributes.
			''' </summary>
			Protected Friend Overridable Sub blockOpen(ByVal t As HTML.Tag, ByVal attr As MutableAttributeSet)
				If impliedP Then blockClose(HTML.Tag.IMPLIED)

				inBlock += 1

				If Not canInsertTag(t, attr, True) Then Return
				If attr.isDefined(IMPLIED) Then attr.removeAttribute(IMPLIED)
				lastWasNewline = False
				attr.addAttribute(StyleConstants.NameAttribute, t)
				Dim es As New ElementSpec(attr.copyAttributes(), ElementSpec.StartTagType)
				parseBuffer.Add(es)
			End Sub

			''' <summary>
			''' Adds an instruction to the parse buffer to close out
			''' a block element of the given type.
			''' </summary>
			Protected Friend Overridable Sub blockClose(ByVal t As HTML.Tag)
				inBlock -= 1

				If Not ___foundInsertTag Then Return

				' Add a new line, if the last character wasn't one. This is
				' needed for proper positioning of the cursor. addContent
				' with true will force an implied paragraph to be generated if
				' there isn't one. This may result in a rather bogus structure
				' (perhaps a table with a child pargraph), but the paragraph
				' is needed for proper positioning and display.
				If Not lastWasNewline Then
					pushCharacterStyle()
					charAttr.addAttribute(IMPLIED_CR, Boolean.TRUE)
					addContent(NEWLINE, 0, 1, True)
					popCharacterStyle()
					lastWasNewline = True
				End If

				If impliedP Then
					impliedP = False
					inParagraph = False
					If t IsNot HTML.Tag.IMPLIED Then blockClose(HTML.Tag.IMPLIED)
				End If
				' an open/close with no content will be removed, so we
				' add a space of content to keep the element being formed.
				Dim prev As ElementSpec = If(parseBuffer.Count > 0, parseBuffer(parseBuffer.Count - 1), Nothing)
				If prev IsNot Nothing AndAlso prev.type = ElementSpec.StartTagType Then
					Dim one As Char() = New Char(0){}
					one(0) = " "c
					addContent(one, 0, 1)
				End If
				Dim es As New ElementSpec(Nothing, ElementSpec.EndTagType)
				parseBuffer.Add(es)
			End Sub

			''' <summary>
			''' Adds some text with the current character attributes.
			''' </summary>
			''' <param name="data"> the content to add </param>
			''' <param name="offs"> the initial offset </param>
			''' <param name="length"> the length </param>
			Protected Friend Overridable Sub addContent(ByVal data As Char(), ByVal offs As Integer, ByVal length As Integer)
				addContent(data, offs, length, True)
			End Sub

			''' <summary>
			''' Adds some text with the current character attributes.
			''' </summary>
			''' <param name="data"> the content to add </param>
			''' <param name="offs"> the initial offset </param>
			''' <param name="length"> the length </param>
			''' <param name="generateImpliedPIfNecessary"> whether to generate implied
			''' paragraphs </param>
			Protected Friend Overridable Sub addContent(ByVal data As Char(), ByVal offs As Integer, ByVal length As Integer, ByVal generateImpliedPIfNecessary As Boolean)
				If Not ___foundInsertTag Then Return

				If generateImpliedPIfNecessary AndAlso ((Not inParagraph)) AndAlso ((Not inPre)) Then
					blockOpen(HTML.Tag.IMPLIED, New SimpleAttributeSet)
					inParagraph = True
					impliedP = True
				End If
				emptyAnchor = False
				charAttr.addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
				Dim a As AttributeSet = charAttr.copyAttributes()
				Dim es As New ElementSpec(a, ElementSpec.ContentType, data, offs, length)
				parseBuffer.Add(es)

				If parseBuffer.Count > threshold Then
					If threshold <= MaxThreshold Then threshold *= StepThreshold
					Try
						flushBuffer(False)
					Catch ble As BadLocationException
					End Try
				End If
				If length > 0 Then lastWasNewline = (data(offs + length - 1) = ControlChars.Lf)
			End Sub

			''' <summary>
			''' Adds content that is basically specified entirely
			''' in the attribute set.
			''' </summary>
			Protected Friend Overridable Sub addSpecialElement(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet)
				If (t IsNot HTML.Tag.FRAME) AndAlso ((Not inParagraph)) AndAlso ((Not inPre)) Then
					nextTagAfterPImplied = t
					blockOpen(HTML.Tag.IMPLIED, New SimpleAttributeSet)
					nextTagAfterPImplied = Nothing
					inParagraph = True
					impliedP = True
				End If
				If Not canInsertTag(t, a, t.block) Then Return
				If a.isDefined(IMPLIED) Then a.removeAttribute(IMPLIED)
				emptyAnchor = False
				a.addAttributes(charAttr)
				a.addAttribute(StyleConstants.NameAttribute, t)
				Dim one As Char() = New Char(0){}
				one(0) = " "c
				Dim es As New ElementSpec(a.copyAttributes(), ElementSpec.ContentType, one, 0, 1)
				parseBuffer.Add(es)
				' Set this to avoid generating a newline for frames, frames
				' shouldn't have any content, and shouldn't need a newline.
				If t Is HTML.Tag.FRAME Then lastWasNewline = True
			End Sub

			''' <summary>
			''' Flushes the current parse buffer into the document. </summary>
			''' <param name="endOfStream"> true if there is no more content to parser </param>
			Friend Overridable Sub flushBuffer(ByVal endOfStream As Boolean)
				Dim oldLength As Integer = outerInstance.length
				Dim size As Integer = parseBuffer.Count
				If endOfStream AndAlso (insertTag IsNot Nothing OrElse insertAfterImplied) AndAlso size > 0 Then
					adjustEndSpecsForPartialInsert()
					size = parseBuffer.Count
				End If
				Dim spec As ElementSpec() = New ElementSpec(size - 1){}
				parseBuffer.CopyTo(spec)

				If oldLength = 0 AndAlso (insertTag Is Nothing AndAlso (Not insertAfterImplied)) Then
					outerInstance.create(spec)
				Else
					outerInstance.insert(offset, spec)
				End If
				parseBuffer.Clear()
				offset += outerInstance.length - oldLength
				flushCount += 1
			End Sub

			''' <summary>
			''' This will be invoked for the last flush, if <code>insertTag</code>
			''' is non null.
			''' </summary>
			Private Sub adjustEndSpecsForPartialInsert()
				Dim size As Integer = parseBuffer.Count
				If insertTagDepthDelta < 0 Then
					' When inserting via an insertTag, the depths (of the tree
					' being read in, and existing hierarchy) may not match up.
					' This attemps to clean it up.
					Dim removeCounter As Integer = insertTagDepthDelta
					Do While removeCounter < 0 AndAlso size >= 0 AndAlso parseBuffer(size - 1).type = ElementSpec.EndTagType
						size -= 1
						parseBuffer.RemoveAt(size)
						removeCounter += 1
					Loop
				End If
				If flushCount = 0 AndAlso ((Not insertAfterImplied) OrElse (Not wantsTrailingNewline)) Then
					' If this starts with content (or popDepth > 0 &&
					' pushDepth > 0) and ends with EndTagTypes, make sure
					' the last content isn't a \n, otherwise will end up with
					' an extra \n in the middle of content.
					Dim index As Integer = 0
					If pushDepth > 0 Then
						If parseBuffer(0).type = ElementSpec.ContentType Then index += 1
					End If
					index += (popDepth + pushDepth)
					Dim cCount As Integer = 0
					Dim cStart As Integer = index
					Do While index < size AndAlso parseBuffer(index).type = ElementSpec.ContentType
						index += 1
						cCount += 1
					Loop
					If cCount > 1 Then
						Do While index < size AndAlso parseBuffer(index).type = ElementSpec.EndTagType
							index += 1
						Loop
						If index = size Then
							Dim lastText As Char() = parseBuffer(cStart + cCount - 1).array
							If lastText.Length = 1 AndAlso lastText(0) = NEWLINE(0) Then
								index = cStart + cCount - 1
								Do While size > index
									size -= 1
									parseBuffer.RemoveAt(size)
								Loop
							End If
						End If
					End If
				End If
				If wantsTrailingNewline Then
					' Make sure there is in fact a newline
					For counter As Integer = parseBuffer.Count - 1 To 0 Step -1
						Dim spec As ElementSpec = parseBuffer(counter)
						If spec.type = ElementSpec.ContentType Then
							If spec.array(spec.length - 1) <> ControlChars.Lf Then
								Dim attrs As New SimpleAttributeSet

								attrs.addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
								parseBuffer.Insert(counter + 1, New ElementSpec(attrs, ElementSpec.ContentType, NEWLINE, 0, 1))
							End If
							Exit For
						End If
					Next counter
				End If
			End Sub

			''' <summary>
			''' Adds the CSS rules in <code>rules</code>.
			''' </summary>
			Friend Overridable Sub addCSSRules(ByVal rules As String)
				Dim ss As StyleSheet = outerInstance.styleSheet
				ss.addRule(rules)
			End Sub

			''' <summary>
			''' Adds the CSS stylesheet at <code>href</code> to the known list
			''' of stylesheets.
			''' </summary>
			Friend Overridable Sub linkCSSStyleSheet(ByVal href As String)
				Dim url As java.net.URL
				Try
					url = New java.net.URL(outerInstance.base, href)
				Catch mfe As java.net.MalformedURLException
					Try
						url = New java.net.URL(href)
					Catch mfe2 As java.net.MalformedURLException
						url = Nothing
					End Try
				End Try
				If url IsNot Nothing Then outerInstance.styleSheet.importStyleSheet(url)
			End Sub

			''' <summary>
			''' Returns true if can insert starting at <code>t</code>. This
			''' will return false if the insert tag is set, and hasn't been found
			''' yet.
			''' </summary>
			Private Function canInsertTag(ByVal t As HTML.Tag, ByVal attr As AttributeSet, ByVal isBlockTag As Boolean) As Boolean
				If Not ___foundInsertTag Then
					Dim needPImplied As Boolean = ((t Is HTML.Tag.IMPLIED) AndAlso ((Not inParagraph)) AndAlso ((Not inPre)))
					If needPImplied AndAlso (nextTagAfterPImplied IsNot Nothing) Then

	'                    
	'                     * If insertTag == null then just proceed to
	'                     * foundInsertTag() call below and return true.
	'                     
						If insertTag IsNot Nothing Then
							Dim nextTagIsInsertTag As Boolean = isInsertTag(nextTagAfterPImplied)
							If ((Not nextTagIsInsertTag)) OrElse ((Not insertInsertTag)) Then Return False
						End If
	'                    
	'                     *  Proceed to foundInsertTag() call...
	'                     
					 ElseIf (insertTag IsNot Nothing AndAlso (Not isInsertTag(t))) OrElse (insertAfterImplied AndAlso (attr Is Nothing OrElse attr.isDefined(IMPLIED) OrElse t Is HTML.Tag.IMPLIED)) Then
						Return False
					 End If

					' Allow the insert if t matches the insert tag, or
					' insertAfterImplied is true and the element is implied.
					foundInsertTag(isBlockTag)
					If Not insertInsertTag Then Return False
				End If
				Return True
			End Function

			Private Function isInsertTag(ByVal tag As HTML.Tag) As Boolean
				Return (insertTag Is tag)
			End Function

			Private Sub foundInsertTag(ByVal isBlockTag As Boolean)
				___foundInsertTag = True
				If (Not insertAfterImplied) AndAlso (popDepth > 0 OrElse pushDepth > 0) Then
					Try
						If offset = 0 OrElse (Not outerInstance.getText(offset - 1, 1).Equals(vbLf)) Then
							' Need to insert a newline.
							Dim newAttrs As AttributeSet = Nothing
							Dim joinP As Boolean = True

							If offset <> 0 Then
								' Determine if we can use JoinPrevious, we can't
								' if the Element has some attributes that are
								' not meant to be duplicated.
								Dim charElement As Element = outerInstance.getCharacterElement(offset - 1)
								Dim attrs As AttributeSet = charElement.attributes

								If attrs.isDefined(StyleConstants.ComposedTextAttribute) Then
									joinP = False
								Else
									Dim name As Object = attrs.getAttribute(StyleConstants.NameAttribute)
									If TypeOf name Is HTML.Tag Then
										Dim tag As HTML.Tag = CType(name, HTML.Tag)
										If tag Is HTML.Tag.IMG OrElse tag Is HTML.Tag.HR OrElse tag Is HTML.Tag.COMMENT OrElse (TypeOf tag Is HTML.UnknownTag) Then joinP = False
									End If
								End If
							End If
							If Not joinP Then
								' If not joining with the previous element, be
								' sure and set the name (otherwise it will be
								' inherited).
								newAttrs = New SimpleAttributeSet
								CType(newAttrs, SimpleAttributeSet).addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
							End If
							Dim es As New ElementSpec(newAttrs, ElementSpec.ContentType, NEWLINE, 0, NEWLINE.Length)
							If joinP Then es.direction = ElementSpec.JoinPreviousDirection
							parseBuffer.Add(es)
						End If
					Catch ble As BadLocationException
					End Try
				End If
				' pops
				For counter As Integer = 0 To popDepth - 1
					parseBuffer.Add(New ElementSpec(Nothing, ElementSpec.EndTagType))
				Next counter
				' pushes
				For counter As Integer = 0 To pushDepth - 1
					Dim es As New ElementSpec(Nothing, ElementSpec.StartTagType)
					es.direction = ElementSpec.JoinNextDirection
					parseBuffer.Add(es)
				Next counter
				insertTagDepthDelta = depthTo(Math.Max(0, offset - 1)) - popDepth + pushDepth - inBlock
				If isBlockTag Then
					' A start spec will be added (for this tag), so we account
					' for it here.
					insertTagDepthDelta += 1
				Else
					' An implied paragraph close (end spec) is going to be added,
					' so we account for it here.
					insertTagDepthDelta -= 1
					inParagraph = True
					lastWasNewline = False
				End If
			End Sub

			''' <summary>
			''' This is set to true when and end is invoked for {@literal <html>}.
			''' </summary>
			Private receivedEndHTML As Boolean
			''' <summary>
			''' Number of times <code>flushBuffer</code> has been invoked. </summary>
			Private flushCount As Integer
			''' <summary>
			''' If true, behavior is similar to insertTag, but instead of
			''' waiting for insertTag will wait for first Element without
			''' an 'implied' attribute and begin inserting then. 
			''' </summary>
			Private insertAfterImplied As Boolean
			''' <summary>
			''' This is only used if insertAfterImplied is true. If false, only
			''' inserting content, and there is a trailing newline it is removed. 
			''' </summary>
			Private wantsTrailingNewline As Boolean
			Friend threshold As Integer
			Friend offset As Integer
			Friend inParagraph As Boolean = False
			Friend impliedP As Boolean = False
			Friend inPre As Boolean = False
			Friend inTextArea As Boolean = False
			Friend textAreaDocument As TextAreaDocument = Nothing
			Friend inTitle As Boolean = False
			Friend lastWasNewline As Boolean = True
			Friend emptyAnchor As Boolean
			''' <summary>
			''' True if (!emptyDocument &amp;&amp; insertTag == null), this is used so
			''' much it is cached. 
			''' </summary>
			Friend midInsert As Boolean
			''' <summary>
			''' True when the body has been encountered. </summary>
			Friend inBody As Boolean
			''' <summary>
			''' If non null, gives parent Tag that insert is to happen at. </summary>
			Friend insertTag As HTML.Tag
			''' <summary>
			''' If true, the insertTag is inserted, otherwise elements after
			''' the insertTag is found are inserted. 
			''' </summary>
			Friend insertInsertTag As Boolean
			''' <summary>
			''' Set to true when insertTag has been found. </summary>
			Friend ___foundInsertTag As Boolean
			''' <summary>
			''' When foundInsertTag is set to true, this will be updated to
			''' reflect the delta between the two structures. That is, it
			''' will be the depth the inserts are happening at minus the
			''' depth of the tags being passed in. A value of 0 (the common
			''' case) indicates the structures match, a value greater than 0 indicates
			''' the insert is happening at a deeper depth than the stream is
			''' parsing, and a value less than 0 indicates the insert is happening earlier
			''' in the tree that the parser thinks and that we will need to remove
			''' EndTagType specs in the flushBuffer method.
			''' </summary>
			Friend insertTagDepthDelta As Integer
			''' <summary>
			''' How many parents to ascend before insert new elements. </summary>
			Friend popDepth As Integer
			''' <summary>
			''' How many parents to descend (relative to popDepth) before
			''' inserting. 
			''' </summary>
			Friend pushDepth As Integer
			''' <summary>
			''' Last Map that was encountered. </summary>
			Friend lastMap As Map
			''' <summary>
			''' Set to true when a style element is encountered. </summary>
			Friend inStyle As Boolean = False
			''' <summary>
			''' Name of style to use. Obtained from Meta tag. </summary>
			Friend defaultStyle As String
			''' <summary>
			''' Vector describing styles that should be include. Will consist
			''' of a bunch of HTML.Tags, which will either be:
			''' <p>LINK: in which case it is followed by an AttributeSet
			''' <p>STYLE: in which case the following element is a String
			''' indicating the type (may be null), and the elements following
			''' it until the next HTML.Tag are the rules as Strings.
			''' </summary>
			Friend styles As List(Of Object)
			''' <summary>
			''' True if inside the head tag. </summary>
			Friend inHead As Boolean = False
			''' <summary>
			''' Set to true if the style language is text/css. Since this is
			''' used alot, it is cached. 
			''' </summary>
			Friend isStyleCSS As Boolean
			''' <summary>
			''' True if inserting into an empty document. </summary>
			Friend emptyDocument As Boolean
			''' <summary>
			''' Attributes from a style Attribute. </summary>
			Friend styleAttributes As AttributeSet

			''' <summary>
			''' Current option, if in an option element (needed to
			''' load the label.
			''' </summary>
			Friend [option] As [Option]

			Protected Friend parseBuffer As New List(Of ElementSpec)
			Protected Friend charAttr As MutableAttributeSet = New TaggedAttributeSet
			Friend charAttrStack As New Stack(Of AttributeSet)
			Friend tagMap As Dictionary(Of HTML.Tag, TagAction)
			Friend inBlock As Integer = 0

			''' <summary>
			''' This attribute is sometimes used to refer to next tag
			''' to be handled after p-implied when the latter is
			''' the current tag which is being handled.
			''' </summary>
			Private nextTagAfterPImplied As HTML.Tag = Nothing
		End Class


		''' <summary>
		''' Used by StyleSheet to determine when to avoid removing HTML.Tags
		''' matching StyleConstants.
		''' </summary>
		Friend Class TaggedAttributeSet
			Inherits SimpleAttributeSet

			Friend Sub New()
				MyBase.New()
			End Sub
		End Class


		''' <summary>
		''' An element that represents a chunk of text that has
		''' a set of HTML character level attributes assigned to
		''' it.
		''' </summary>
		Public Class RunElement
			Inherits LeafElement

			Private ReadOnly outerInstance As HTMLDocument


			''' <summary>
			''' Constructs an element that represents content within the
			''' document (has no children).
			''' </summary>
			''' <param name="parent">  the parent element </param>
			''' <param name="a">       the element attributes </param>
			''' <param name="offs0">   the start offset (must be at least 0) </param>
			''' <param name="offs1">   the end offset (must be at least offs0)
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As HTMLDocument, ByVal parent As Element, ByVal a As AttributeSet, ByVal offs0 As Integer, ByVal offs1 As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(parent, a, offs0, offs1)
			End Sub

			''' <summary>
			''' Gets the name of the element.
			''' </summary>
			''' <returns> the name, null if none </returns>
			Public Overridable Property name As String
				Get
					Dim o As Object = getAttribute(StyleConstants.NameAttribute)
					If o IsNot Nothing Then Return o.ToString()
					Return MyBase.name
				End Get
			End Property

			''' <summary>
			''' Gets the resolving parent.  HTML attributes are not inherited
			''' at the model level so we override this to return null.
			''' </summary>
			''' <returns> null, there are none </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
			Public Overridable Property resolveParent As AttributeSet
				Get
					Return Nothing
				End Get
			End Property
		End Class

		''' <summary>
		''' An element that represents a structural <em>block</em> of
		''' HTML.
		''' </summary>
		Public Class BlockElement
			Inherits BranchElement

			Private ReadOnly outerInstance As HTMLDocument


			''' <summary>
			''' Constructs a composite element that initially contains
			''' no children.
			''' </summary>
			''' <param name="parent">  the parent element </param>
			''' <param name="a">       the attributes for the element
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As HTMLDocument, ByVal parent As Element, ByVal a As AttributeSet)
					Me.outerInstance = outerInstance
				MyBase.New(parent, a)
			End Sub

			''' <summary>
			''' Gets the name of the element.
			''' </summary>
			''' <returns> the name, null if none </returns>
			Public Overridable Property name As String
				Get
					Dim o As Object = getAttribute(StyleConstants.NameAttribute)
					If o IsNot Nothing Then Return o.ToString()
					Return MyBase.name
				End Get
			End Property

			''' <summary>
			''' Gets the resolving parent.  HTML attributes are not inherited
			''' at the model level so we override this to return null.
			''' </summary>
			''' <returns> null, there are none </returns>
			''' <seealso cref= AttributeSet#getResolveParent </seealso>
			Public Overridable Property resolveParent As AttributeSet
				Get
					Return Nothing
				End Get
			End Property

		End Class


		''' <summary>
		''' Document that allows you to set the maximum length of the text.
		''' </summary>
		Private Class FixedLengthDocument
			Inherits PlainDocument

			Private maxLength As Integer

			Public Sub New(ByVal maxLength As Integer)
				Me.maxLength = maxLength
			End Sub

			Public Overrides Sub insertString(ByVal offset As Integer, ByVal str As String, ByVal a As AttributeSet)
				If str IsNot Nothing AndAlso str.Length + length <= maxLength Then MyBase.insertString(offset, str, a)
			End Sub
		End Class
	End Class

End Namespace