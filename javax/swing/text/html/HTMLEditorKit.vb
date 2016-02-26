Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports javax.swing.text
Imports javax.swing
Imports javax.swing.event
Imports javax.accessibility

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
	''' The Swing JEditorPane text component supports different kinds
	''' of content via a plug-in mechanism called an EditorKit.  Because
	''' HTML is a very popular format of content, some support is provided
	''' by default.  The default support is provided by this class, which
	''' supports HTML version 3.2 (with some extensions), and is migrating
	''' toward version 4.0.
	''' The &lt;applet&gt; tag is not supported, but some support is provided
	''' for the &lt;object&gt; tag.
	''' <p>
	''' There are several goals of the HTML EditorKit provided, that have
	''' an effect upon the way that HTML is modeled.  These
	''' have influenced its design in a substantial way.
	''' <dl>
	''' <dt>
	''' Support editing
	''' <dd>
	''' It might seem fairly obvious that a plug-in for JEditorPane
	''' should provide editing support, but that fact has several
	''' design considerations.  There are a substantial number of HTML
	''' documents that don't properly conform to an HTML specification.
	''' These must be normalized somewhat into a correct form if one
	''' is to edit them.  Additionally, users don't like to be presented
	''' with an excessive amount of structure editing, so using traditional
	''' text editing gestures is preferred over using the HTML structure
	''' exactly as defined in the HTML document.
	''' <p>
	''' The modeling of HTML is provided by the class <code>HTMLDocument</code>.
	''' Its documentation describes the details of how the HTML is modeled.
	''' The editing support leverages heavily off of the text package.
	''' 
	''' <dt>
	''' Extendable/Scalable
	''' <dd>
	''' To maximize the usefulness of this kit, a great deal of effort
	''' has gone into making it extendable.  These are some of the
	''' features.
	''' <ol>
	'''   <li>
	'''   The parser is replaceable.  The default parser is the Hot Java
	'''   parser which is DTD based.  A different DTD can be used, or an
	'''   entirely different parser can be used.  To change the parser,
	'''   reimplement the getParser method.  The default parser is
	'''   dynamically loaded when first asked for, so the class files
	'''   will never be loaded if an alternative parser is used.  The
	'''   default parser is in a separate package called parser below
	'''   this package.
	'''   <li>
	'''   The parser drives the ParserCallback, which is provided by
	'''   HTMLDocument.  To change the callback, subclass HTMLDocument
	'''   and reimplement the createDefaultDocument method to return
	'''   document that produces a different reader.  The reader controls
	'''   how the document is structured.  Although the Document provides
	'''   HTML support by default, there is nothing preventing support of
	'''   non-HTML tags that result in alternative element structures.
	'''   <li>
	'''   The default view of the models are provided as a hierarchy of
	'''   View implementations, so one can easily customize how a particular
	'''   element is displayed or add capabilities for new kinds of elements
	'''   by providing new View implementations.  The default set of views
	'''   are provided by the <code>HTMLFactory</code> class.  This can
	'''   be easily changed by subclassing or replacing the HTMLFactory
	'''   and reimplementing the getViewFactory method to return the alternative
	'''   factory.
	'''   <li>
	'''   The View implementations work primarily off of CSS attributes,
	'''   which are kept in the views.  This makes it possible to have
	'''   multiple views mapped over the same model that appear substantially
	'''   different.  This can be especially useful for printing.  For
	'''   most HTML attributes, the HTML attributes are converted to CSS
	'''   attributes for display.  This helps make the View implementations
	'''   more general purpose
	''' </ol>
	''' 
	''' <dt>
	''' Asynchronous Loading
	''' <dd>
	''' Larger documents involve a lot of parsing and take some time
	''' to load.  By default, this kit produces documents that will be
	''' loaded asynchronously if loaded using <code>JEditorPane.setPage</code>.
	''' This is controlled by a property on the document.  The method
	''' <seealso cref="#createDefaultDocument createDefaultDocument"/> can
	''' be overriden to change this.  The batching of work is done
	''' by the <code>HTMLDocument.HTMLReader</code> class.  The actual
	''' work is done by the <code>DefaultStyledDocument</code> and
	''' <code>AbstractDocument</code> classes in the text package.
	''' 
	''' <dt>
	''' Customization from current LAF
	''' <dd>
	''' HTML provides a well known set of features without exactly
	''' specifying the display characteristics.  Swing has a theme
	''' mechanism for its look-and-feel implementations.  It is desirable
	''' for the look-and-feel to feed display characteristics into the
	''' HTML views.  An user with poor vision for example would want
	''' high contrast and larger than typical fonts.
	''' <p>
	''' The support for this is provided by the <code>StyleSheet</code>
	''' class.  The presentation of the HTML can be heavily influenced
	''' by the setting of the StyleSheet property on the EditorKit.
	''' 
	''' <dt>
	''' Not lossy
	''' <dd>
	''' An EditorKit has the ability to be read and save documents.
	''' It is generally the most pleasing to users if there is no loss
	''' of data between the two operation.  The policy of the HTMLEditorKit
	''' will be to store things not recognized or not necessarily visible
	''' so they can be subsequently written out.  The model of the HTML document
	''' should therefore contain all information discovered while reading the
	''' document.  This is constrained in some ways by the need to support
	''' editing (i.e. incorrect documents sometimes must be normalized).
	''' The guiding principle is that information shouldn't be lost, but
	''' some might be synthesized to produce a more correct model or it might
	''' be rearranged.
	''' </dl>
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class HTMLEditorKit
		Inherits StyledEditorKit
		Implements Accessible

		Private theEditor As JEditorPane

		''' <summary>
		''' Constructs an HTMLEditorKit, creates a StyleContext,
		''' and loads the style sheet.
		''' </summary>
		Public Sub New()

		End Sub

		''' <summary>
		''' Get the MIME type of the data that this
		''' kit represents support for.  This kit supports
		''' the type <code>text/html</code>.
		''' </summary>
		''' <returns> the type </returns>
		Public Property Overrides contentType As String
			Get
				Return "text/html"
			End Get
		End Property

		''' <summary>
		''' Fetch a factory that is suitable for producing
		''' views of any models that are produced by this
		''' kit.
		''' </summary>
		''' <returns> the factory </returns>
		Public Property Overrides viewFactory As ViewFactory
			Get
				Return defaultFactory
			End Get
		End Property

		''' <summary>
		''' Create an uninitialized text storage model
		''' that is appropriate for this type of editor.
		''' </summary>
		''' <returns> the model </returns>
		Public Overrides Function createDefaultDocument() As Document
			Dim styles As StyleSheet = styleSheet
			Dim ss As New StyleSheet

			ss.addStyleSheet(styles)

			Dim doc As New HTMLDocument(ss)
			doc.parser = parser
			doc.asynchronousLoadPriority = 4
			doc.tokenThreshold = 100
			Return doc
		End Function

		''' <summary>
		''' Try to get an HTML parser from the document.  If no parser is set for
		''' the document, return the editor kit's default parser.  It is an error
		''' if no parser could be obtained from the editor kit.
		''' </summary>
		Private Function ensureParser(ByVal doc As HTMLDocument) As Parser
			Dim p As Parser = doc.parser
			If p Is Nothing Then p = parser
			If p Is Nothing Then Throw New IOException("Can't load parser")
			Return p
		End Function

		''' <summary>
		''' Inserts content from the given stream. If <code>doc</code> is
		''' an instance of HTMLDocument, this will read
		''' HTML 3.2 text. Inserting HTML into a non-empty document must be inside
		''' the body Element, if you do not insert into the body an exception will
		''' be thrown. When inserting into a non-empty document all tags outside
		''' of the body (head, title) will be dropped.
		''' </summary>
		''' <param name="in">  the stream to read from </param>
		''' <param name="doc"> the destination for the insertion </param>
		''' <param name="pos"> the location in the document to place the
		'''   content </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document </exception>
		''' <exception cref="RuntimeException"> (will eventually be a BadLocationException)
		'''            if pos is invalid </exception>
		Public Overrides Sub read(ByVal [in] As Reader, ByVal doc As Document, ByVal pos As Integer)

			If TypeOf doc Is HTMLDocument Then
				Dim hdoc As HTMLDocument = CType(doc, HTMLDocument)
				If pos > doc.length Then Throw New BadLocationException("Invalid location", pos)

				Dim p As Parser = ensureParser(hdoc)
				Dim receiver As ParserCallback = hdoc.getReader(pos)
				Dim ignoreCharset As Boolean? = CBool(doc.getProperty("IgnoreCharsetDirective"))
				p.parse([in], receiver,If(ignoreCharset Is Nothing, False, ignoreCharset))
				receiver.flush()
			Else
				MyBase.read([in], doc, pos)
			End If
		End Sub

		''' <summary>
		''' Inserts HTML into an existing document.
		''' </summary>
		''' <param name="doc">       the document to insert into </param>
		''' <param name="offset">    the offset to insert HTML at </param>
		''' <param name="popDepth">  the number of ElementSpec.EndTagTypes to generate before
		'''        inserting </param>
		''' <param name="pushDepth"> the number of ElementSpec.StartTagTypes with a direction
		'''        of ElementSpec.JoinNextDirection that should be generated
		'''        before inserting, but after the end tags have been generated </param>
		''' <param name="insertTag"> the first tag to start inserting into document </param>
		''' <exception cref="RuntimeException"> (will eventually be a BadLocationException)
		'''            if pos is invalid </exception>
		Public Overridable Sub insertHTML(ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal html As String, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal insertTag As HTML.Tag)
			If offset > doc.length Then Throw New BadLocationException("Invalid location", offset)

			Dim p As Parser = ensureParser(doc)
			Dim receiver As ParserCallback = doc.getReader(offset, popDepth, pushDepth, insertTag)
			Dim ignoreCharset As Boolean? = CBool(doc.getProperty("IgnoreCharsetDirective"))
			p.parse(New StringReader(html), receiver,If(ignoreCharset Is Nothing, False, ignoreCharset))
			receiver.flush()
		End Sub

		''' <summary>
		''' Write content from a document to the given stream
		''' in a format appropriate for this kind of content handler.
		''' </summary>
		''' <param name="out">  the stream to write to </param>
		''' <param name="doc">  the source for the write </param>
		''' <param name="pos">  the location in the document to fetch the
		'''   content </param>
		''' <param name="len">  the amount to write out </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document </exception>
		Public Overrides Sub write(ByVal out As Writer, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

			If TypeOf doc Is HTMLDocument Then
				Dim w As New HTMLWriter(out, CType(doc, HTMLDocument), pos, len)
				w.write()
			ElseIf TypeOf doc Is StyledDocument Then
				Dim w As New MinimalHTMLWriter(out, CType(doc, StyledDocument), pos, len)
				w.write()
			Else
				MyBase.write(out, doc, pos, len)
			End If
		End Sub

		''' <summary>
		''' Called when the kit is being installed into the
		''' a JEditorPane.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overridable Sub install(ByVal c As JEditorPane)
			c.addMouseListener(linkHandler)
			c.addMouseMotionListener(linkHandler)
			c.addCaretListener(nextLinkAction)
			MyBase.install(c)
			theEditor = c
		End Sub

		''' <summary>
		''' Called when the kit is being removed from the
		''' JEditorPane.  This is used to unregister any
		''' listeners that were attached.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overridable Sub deinstall(ByVal c As JEditorPane)
			c.removeMouseListener(linkHandler)
			c.removeMouseMotionListener(linkHandler)
			c.removeCaretListener(nextLinkAction)
			MyBase.deinstall(c)
			theEditor = Nothing
		End Sub

		''' <summary>
		''' Default Cascading Style Sheet file that sets
		''' up the tag views.
		''' </summary>
		Public Const DEFAULT_CSS As String = "default.css"

		''' <summary>
		''' Set the set of styles to be used to render the various
		''' HTML elements.  These styles are specified in terms of
		''' CSS specifications.  Each document produced by the kit
		''' will have a copy of the sheet which it can add the
		''' document specific styles to.  By default, the StyleSheet
		''' specified is shared by all HTMLEditorKit instances.
		''' This should be reimplemented to provide a finer granularity
		''' if desired.
		''' </summary>
		Public Overridable Property styleSheet As StyleSheet
			Set(ByVal s As StyleSheet)
				If s Is Nothing Then
					sun.awt.AppContext.appContext.remove(DEFAULT_STYLES_KEY)
				Else
					sun.awt.AppContext.appContext.put(DEFAULT_STYLES_KEY, s)
				End If
			End Set
			Get
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim defaultStyles As StyleSheet = CType(appContext.get(DEFAULT_STYLES_KEY), StyleSheet)
    
				If defaultStyles Is Nothing Then
					defaultStyles = New StyleSheet
					appContext.put(DEFAULT_STYLES_KEY, defaultStyles)
					Try
						Dim [is] As InputStream = HTMLEditorKit.getResourceAsStream(DEFAULT_CSS)
						Dim r As Reader = New BufferedReader(New InputStreamReader([is], "ISO-8859-1"))
						defaultStyles.loadRules(r, Nothing)
						r.close()
					Catch e As Exception
						' on error we simply have no styles... the html
						' will look mighty wrong but still function.
					End Try
				End If
				Return defaultStyles
			End Get
		End Property


		''' <summary>
		''' Fetch a resource relative to the HTMLEditorKit classfile.
		''' If this is called on 1.2 the loading will occur under the
		''' protection of a doPrivileged call to allow the HTMLEditorKit
		''' to function when used in an applet.
		''' </summary>
		''' <param name="name"> the name of the resource, relative to the
		'''  HTMLEditorKit class </param>
		''' <returns> a stream representing the resource </returns>
		Friend Shared Function getResourceAsStream(ByVal name As String) As InputStream
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<InputStream>()
	'		{
	'					public InputStream run()
	'					{
	'						Return HTMLEditorKit.class.getResourceAsStream(name);
	'					}
	'				});
		End Function

		''' <summary>
		''' Fetches the command list for the editor.  This is
		''' the list of commands supported by the superclass
		''' augmented by the collection of commands defined
		''' locally for style operations.
		''' </summary>
		''' <returns> the command list </returns>
		Public Property Overrides actions As Action()
			Get
				Return TextAction.augmentList(MyBase.actions, Me.defaultActions)
			End Get
		End Property

		''' <summary>
		''' Copies the key/values in <code>element</code>s AttributeSet into
		''' <code>set</code>. This does not copy component, icon, or element
		''' names attributes. Subclasses may wish to refine what is and what
		''' isn't copied here. But be sure to first remove all the attributes that
		''' are in <code>set</code>.<p>
		''' This is called anytime the caret moves over a different location.
		''' 
		''' </summary>
		Protected Friend Overrides Sub createInputAttributes(ByVal element As Element, ByVal [set] As MutableAttributeSet)
			[set].removeAttributes([set])
			[set].addAttributes(element.attributes)
			[set].removeAttribute(StyleConstants.ComposedTextAttribute)

			Dim o As Object = [set].getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim tag As HTML.Tag = CType(o, HTML.Tag)
				' PENDING: we need a better way to express what shouldn't be
				' copied when editing...
				If tag Is HTML.Tag.IMG Then
					' Remove the related image attributes, src, width, height
					[set].removeAttribute(HTML.Attribute.SRC)
					[set].removeAttribute(HTML.Attribute.HEIGHT)
					[set].removeAttribute(HTML.Attribute.WIDTH)
					[set].addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
				ElseIf tag Is HTML.Tag.HR OrElse tag Is HTML.Tag.BR Then
					' Don't copy HRs or BRs either.
					[set].addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
				ElseIf tag Is HTML.Tag.COMMENT Then
					' Don't copy COMMENTs either
					[set].addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
					[set].removeAttribute(HTML.Attribute.COMMENT)
				ElseIf tag Is HTML.Tag.INPUT Then
					' or INPUT either
					[set].addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
					[set].removeAttribute(HTML.Tag.INPUT)
				ElseIf TypeOf tag Is HTML.UnknownTag Then
					' Don't copy unknowns either:(
					[set].addAttribute(StyleConstants.NameAttribute, HTML.Tag.CONTENT)
					[set].removeAttribute(HTML.Attribute.ENDTAG)
				End If
			End If
		End Sub

		''' <summary>
		''' Gets the input attributes used for the styled
		''' editing actions.
		''' </summary>
		''' <returns> the attribute set </returns>
		Public Property Overrides inputAttributes As MutableAttributeSet
			Get
				If input Is Nothing Then input = styleSheet.addStyle(Nothing, Nothing)
				Return input
			End Get
		End Property

		''' <summary>
		''' Sets the default cursor.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property defaultCursor As Cursor
			Set(ByVal cursor As Cursor)
				defaultCursor = cursor
			End Set
			Get
				Return defaultCursor
			End Get
		End Property


		''' <summary>
		''' Sets the cursor to use over links.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property linkCursor As Cursor
			Set(ByVal cursor As Cursor)
				linkCursor = cursor
			End Set
			Get
				Return linkCursor
			End Get
		End Property


		''' <summary>
		''' Indicates whether an html form submission is processed automatically
		''' or only <code>FormSubmitEvent</code> is fired.
		''' </summary>
		''' <returns> true  if html form submission is processed automatically,
		'''         false otherwise.
		''' </returns>
		''' <seealso cref= #setAutoFormSubmission
		''' @since 1.5 </seealso>
		Public Overridable Property autoFormSubmission As Boolean
			Get
				Return ___isAutoFormSubmission
			End Get
			Set(ByVal isAuto As Boolean)
				___isAutoFormSubmission = isAuto
			End Set
		End Property


		''' <summary>
		''' Creates a copy of the editor kit.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overrides Function clone() As Object
			Dim o As HTMLEditorKit = CType(MyBase.clone(), HTMLEditorKit)
			If o IsNot Nothing Then
				o.input = Nothing
				o.linkHandler = New LinkController
			End If
			Return o
		End Function

		''' <summary>
		''' Fetch the parser to use for reading HTML streams.
		''' This can be reimplemented to provide a different
		''' parser.  The default implementation is loaded dynamically
		''' to avoid the overhead of loading the default parser if
		''' it's not used.  The default parser is the HotJava parser
		''' using an HTML 3.2 DTD.
		''' </summary>
		Protected Friend Overridable Property parser As Parser
			Get
				If defaultParser Is Nothing Then
					Try
						Dim c As Type = Type.GetType("javax.swing.text.html.parser.ParserDelegator")
						defaultParser = CType(c.newInstance(), Parser)
					Catch e As Exception
					End Try
				End If
				Return defaultParser
			End Get
		End Property

		' ----- Accessibility support -----
		Private ___accessibleContext As AccessibleContext

		''' <summary>
		''' returns the AccessibleContext associated with this editor kit
		''' </summary>
		''' <returns> the AccessibleContext associated with this editor kit
		''' @since 1.4 </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If theEditor Is Nothing Then Return Nothing
				If ___accessibleContext Is Nothing Then
					Dim a As New AccessibleHTML(theEditor)
					___accessibleContext = a.accessibleContext
				End If
				Return ___accessibleContext
			End Get
		End Property

		' --- variables ------------------------------------------

		Private Shared ReadOnly MoveCursor As Cursor = Cursor.getPredefinedCursor(Cursor.HAND_CURSOR)
		Private Shared ReadOnly DefaultCursor As Cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)

		''' <summary>
		''' Shared factory for creating HTML Views. </summary>
		Private Shared ReadOnly defaultFactory As ViewFactory = New HTMLFactory

		Friend input As MutableAttributeSet
		Private Shared ReadOnly DEFAULT_STYLES_KEY As New Object
		Private linkHandler As New LinkController
		Private Shared defaultParser As Parser = Nothing
		Private defaultCursor As Cursor = DefaultCursor
		Private linkCursor As Cursor = MoveCursor
		Private ___isAutoFormSubmission As Boolean = True

		''' <summary>
		''' Class to watch the associated component and fire
		''' hyperlink events on it when appropriate.
		''' </summary>
		<Serializable> _
		Public Class LinkController
			Inherits MouseAdapter
			Implements MouseMotionListener

			Private curElem As Element = Nothing
			''' <summary>
			''' If true, the current element (curElem) represents an image.
			''' </summary>
			Private curElemImage As Boolean = False
			Private href As String = Nothing
			''' <summary>
			''' This is used by viewToModel to avoid allocing a new array each
			''' time. 
			''' </summary>
			<NonSerialized> _
			Private bias As Position.Bias() = New Position.Bias(0){}
			''' <summary>
			''' Current offset.
			''' </summary>
			Private curOffset As Integer

			''' <summary>
			''' Called for a mouse click event.
			''' If the component is read-only (ie a browser) then
			''' the clicked event is used to drive an attempt to
			''' follow the reference specified by a link.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			''' <seealso cref= MouseListener#mouseClicked </seealso>
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				Dim editor As JEditorPane = CType(e.source, JEditorPane)

				If (Not editor.editable) AndAlso editor.enabled AndAlso SwingUtilities.isLeftMouseButton(e) Then
					Dim pt As New Point(e.x, e.y)
					Dim pos As Integer = editor.viewToModel(pt)
					If pos >= 0 Then activateLink(pos, editor, e)
				End If
			End Sub

			' ignore the drags
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
			End Sub

			' track the moving of the mouse.
			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				Dim editor As JEditorPane = CType(e.source, JEditorPane)
				If Not editor.enabled Then Return

				Dim kit As HTMLEditorKit = CType(editor.editorKit, HTMLEditorKit)
				Dim adjustCursor As Boolean = True
				Dim newCursor As Cursor = kit.defaultCursor
				If Not editor.editable Then
					Dim pt As New Point(e.x, e.y)
					Dim pos As Integer = editor.uI.viewToModel(editor, pt, bias)
					If bias(0) Is Position.Bias.Backward AndAlso pos > 0 Then pos -= 1
					If pos >= 0 AndAlso (TypeOf editor.document Is HTMLDocument) Then
						Dim hdoc As HTMLDocument = CType(editor.document, HTMLDocument)
						Dim elem As Element = hdoc.getCharacterElement(pos)
						If Not doesElementContainLocation(editor, elem, pos, e.x, e.y) Then elem = Nothing
						If curElem IsNot elem OrElse curElemImage Then
							Dim lastElem As Element = curElem
							curElem = elem
							Dim href As String = Nothing
							curElemImage = False
							If elem IsNot Nothing Then
								Dim a As AttributeSet = elem.attributes
								Dim anchor As AttributeSet = CType(a.getAttribute(HTML.Tag.A), AttributeSet)
								If anchor Is Nothing Then
									curElemImage = (a.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.IMG)
									If curElemImage Then href = getMapHREF(editor, hdoc, elem, a, pos, e.x, e.y)
								Else
									href = CStr(anchor.getAttribute(HTML.Attribute.HREF))
								End If
							End If

							If href <> Me.href Then
								' reference changed, fire event(s)
								fireEvents(editor, hdoc, href, lastElem, e)
								Me.href = href
								If href IsNot Nothing Then newCursor = kit.linkCursor
							Else
								adjustCursor = False
							End If
						Else
							adjustCursor = False
						End If
						curOffset = pos
					End If
				End If
				If adjustCursor AndAlso editor.cursor IsNot newCursor Then editor.cursor = newCursor
			End Sub

			''' <summary>
			''' Returns a string anchor if the passed in element has a
			''' USEMAP that contains the passed in location.
			''' </summary>
			Private Function getMapHREF(ByVal html As JEditorPane, ByVal hdoc As HTMLDocument, ByVal elem As Element, ByVal attr As AttributeSet, ByVal offset As Integer, ByVal x As Integer, ByVal y As Integer) As String
				Dim useMap As Object = attr.getAttribute(HTML.Attribute.USEMAP)
				If useMap IsNot Nothing AndAlso (TypeOf useMap Is String) Then
					Dim m As Map = hdoc.getMap(CStr(useMap))
					If m IsNot Nothing AndAlso offset < hdoc.length Then
						Dim bounds As Rectangle
						Dim ui As javax.swing.plaf.TextUI = html.uI
						Try
							Dim lBounds As Shape = ui.modelToView(html, offset, Position.Bias.Forward)
							Dim rBounds As Shape = ui.modelToView(html, offset + 1, Position.Bias.Backward)
							bounds = lBounds.bounds
							bounds.add(If(TypeOf rBounds Is Rectangle, CType(rBounds, Rectangle), rBounds.bounds))
						Catch ble As BadLocationException
							bounds = Nothing
						End Try
						If bounds IsNot Nothing Then
							Dim area As AttributeSet = m.getArea(x - bounds.x, y - bounds.y, bounds.width, bounds.height)
							If area IsNot Nothing Then Return CStr(area.getAttribute(HTML.Attribute.HREF))
						End If
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns true if the View representing <code>e</code> contains
			''' the location <code>x</code>, <code>y</code>. <code>offset</code>
			''' gives the offset into the Document to check for.
			''' </summary>
			Private Function doesElementContainLocation(ByVal editor As JEditorPane, ByVal e As Element, ByVal offset As Integer, ByVal x As Integer, ByVal y As Integer) As Boolean
				If e IsNot Nothing AndAlso offset > 0 AndAlso e.startOffset = offset Then
					Try
						Dim ui As javax.swing.plaf.TextUI = editor.uI
						Dim s1 As Shape = ui.modelToView(editor, offset, Position.Bias.Forward)
						If s1 Is Nothing Then Return False
						Dim r1 As Rectangle = If(TypeOf s1 Is Rectangle, CType(s1, Rectangle), s1.bounds)
						Dim s2 As Shape = ui.modelToView(editor, e.endOffset, Position.Bias.Backward)
						If s2 IsNot Nothing Then
							Dim r2 As Rectangle = If(TypeOf s2 Is Rectangle, CType(s2, Rectangle), s2.bounds)
							r1.add(r2)
						End If
						Return r1.contains(x, y)
					Catch ble As BadLocationException
					End Try
				End If
				Return True
			End Function

			''' <summary>
			''' Calls linkActivated on the associated JEditorPane
			''' if the given position represents a link.<p>This is implemented
			''' to forward to the method with the same name, but with the following
			''' args both == -1.
			''' </summary>
			''' <param name="pos"> the position </param>
			''' <param name="editor"> the editor pane </param>
			Protected Friend Overridable Sub activateLink(ByVal pos As Integer, ByVal editor As JEditorPane)
				activateLink(pos, editor, Nothing)
			End Sub

			''' <summary>
			''' Calls linkActivated on the associated JEditorPane
			''' if the given position represents a link. If this was the result
			''' of a mouse click, <code>x</code> and
			''' <code>y</code> will give the location of the mouse, otherwise
			''' they will be {@literal <} 0.
			''' </summary>
			''' <param name="pos"> the position </param>
			''' <param name="html"> the editor pane </param>
			Friend Overridable Sub activateLink(ByVal pos As Integer, ByVal html As JEditorPane, ByVal mouseEvent As MouseEvent)
				Dim doc As Document = html.document
				If TypeOf doc Is HTMLDocument Then
					Dim hdoc As HTMLDocument = CType(doc, HTMLDocument)
					Dim e As Element = hdoc.getCharacterElement(pos)
					Dim a As AttributeSet = e.attributes
					Dim anchor As AttributeSet = CType(a.getAttribute(HTML.Tag.A), AttributeSet)
					Dim linkEvent As HyperlinkEvent = Nothing
					Dim description As String
					Dim x As Integer = -1
					Dim y As Integer = -1

					If mouseEvent IsNot Nothing Then
						x = mouseEvent.x
						y = mouseEvent.y
					End If

					If anchor Is Nothing Then
						href = getMapHREF(html, hdoc, e, a, pos, x, y)
					Else
						href = CStr(anchor.getAttribute(HTML.Attribute.HREF))
					End If

					If href IsNot Nothing Then linkEvent = createHyperlinkEvent(html, hdoc, href, anchor, e, mouseEvent)
					If linkEvent IsNot Nothing Then html.fireHyperlinkUpdate(linkEvent)
				End If
			End Sub

			''' <summary>
			''' Creates and returns a new instance of HyperlinkEvent. If
			''' <code>hdoc</code> is a frame document a HTMLFrameHyperlinkEvent
			''' will be created.
			''' </summary>
			Friend Overridable Function createHyperlinkEvent(ByVal html As JEditorPane, ByVal hdoc As HTMLDocument, ByVal href As String, ByVal anchor As AttributeSet, ByVal element As Element, ByVal mouseEvent As MouseEvent) As HyperlinkEvent
				Dim u As java.net.URL
				Try
					Dim base As java.net.URL = hdoc.base
					u = New java.net.URL(base, href)
					' Following is a workaround for 1.2, in which
					' new URL("file://...", "#...") causes the filename to
					' be lost.
					If href IsNot Nothing AndAlso "file".Equals(u.protocol) AndAlso href.StartsWith("#") Then
						Dim baseFile As String = base.file
						Dim newFile As String = u.file
						If baseFile IsNot Nothing AndAlso newFile IsNot Nothing AndAlso (Not newFile.StartsWith(baseFile)) Then u = New java.net.URL(base, baseFile + href)
					End If
				Catch m As java.net.MalformedURLException
					u = Nothing
				End Try
				Dim linkEvent As HyperlinkEvent

				If Not hdoc.frameDocument Then
					linkEvent = New HyperlinkEvent(html, HyperlinkEvent.EventType.ACTIVATED, u, href, element, mouseEvent)
				Else
					Dim target As String = If(anchor IsNot Nothing, CStr(anchor.getAttribute(HTML.Attribute.TARGET)), Nothing)
					If (target Is Nothing) OrElse (target.Equals("")) Then target = hdoc.baseTarget
					If (target Is Nothing) OrElse (target.Equals("")) Then target = "_self"
						linkEvent = New HTMLFrameHyperlinkEvent(html, HyperlinkEvent.EventType.ACTIVATED, u, href, element, mouseEvent, target)
				End If
				Return linkEvent
			End Function

			Friend Overridable Sub fireEvents(ByVal editor As JEditorPane, ByVal doc As HTMLDocument, ByVal href As String, ByVal lastElem As Element, ByVal mouseEvent As MouseEvent)
				If Me.href IsNot Nothing Then
					' fire an exited event on the old link
					Dim u As java.net.URL
					Try
						u = New java.net.URL(doc.base, Me.href)
					Catch m As java.net.MalformedURLException
						u = Nothing
					End Try
					Dim [exit] As New HyperlinkEvent(editor, HyperlinkEvent.EventType.EXITED, u, Me.href, lastElem, mouseEvent)
					editor.fireHyperlinkUpdate([exit])
				End If
				If href IsNot Nothing Then
					' fire an entered event on the new link
					Dim u As java.net.URL
					Try
						u = New java.net.URL(doc.base, href)
					Catch m As java.net.MalformedURLException
						u = Nothing
					End Try
					Dim entered As New HyperlinkEvent(editor, HyperlinkEvent.EventType.ENTERED, u, href, curElem, mouseEvent)
					editor.fireHyperlinkUpdate(entered)
				End If
			End Sub
		End Class

		''' <summary>
		''' Interface to be supported by the parser.  This enables
		''' providing a different parser while reusing some of the
		''' implementation provided by this editor kit.
		''' </summary>
		Public MustInherit Class Parser
			''' <summary>
			''' Parse the given stream and drive the given callback
			''' with the results of the parse.  This method should
			''' be implemented to be thread-safe.
			''' </summary>
			Public MustOverride Sub parse(ByVal r As Reader, ByVal cb As ParserCallback, ByVal ignoreCharSet As Boolean)

		End Class

		''' <summary>
		''' The result of parsing drives these callback methods.
		''' The open and close actions should be balanced.  The
		''' <code>flush</code> method will be the last method
		''' called, to give the receiver a chance to flush any
		''' pending data into the document.
		''' <p>Refer to DocumentParser, the default parser used, for further
		''' information on the contents of the AttributeSets, the positions, and
		''' other info.
		''' </summary>
		''' <seealso cref= javax.swing.text.html.parser.DocumentParser </seealso>
		Public Class ParserCallback
			''' <summary>
			''' This is passed as an attribute in the attributeset to indicate
			''' the element is implied eg, the string '&lt;&gt;foo&lt;\t&gt;'
			''' contains an implied html element and an implied body element.
			''' 
			''' @since 1.3
			''' </summary>
			Public Const IMPLIED As Object = "_implied_"


			Public Overridable Sub flush()
			End Sub

			Public Overridable Sub handleText(ByVal data As Char(), ByVal pos As Integer)
			End Sub

			Public Overridable Sub handleComment(ByVal data As Char(), ByVal pos As Integer)
			End Sub

			Public Overridable Sub handleStartTag(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet, ByVal pos As Integer)
			End Sub

			Public Overridable Sub handleEndTag(ByVal t As HTML.Tag, ByVal pos As Integer)
			End Sub

			Public Overridable Sub handleSimpleTag(ByVal t As HTML.Tag, ByVal a As MutableAttributeSet, ByVal pos As Integer)
			End Sub

			Public Overridable Sub handleError(ByVal errorMsg As String, ByVal pos As Integer)
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
			End Sub
		End Class

		''' <summary>
		''' A factory to build views for HTML.  The following
		''' table describes what this factory will build by
		''' default.
		''' 
		''' <table summary="Describes the tag and view created by this factory by default">
		''' <tr>
		''' <th align=left>Tag<th align=left>View created
		''' </tr><tr>
		''' <td>HTML.Tag.CONTENT<td>InlineView
		''' </tr><tr>
		''' <td>HTML.Tag.IMPLIED<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.P<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H1<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H2<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H3<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H4<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H5<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.H6<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.DT<td>javax.swing.text.html.ParagraphView
		''' </tr><tr>
		''' <td>HTML.Tag.MENU<td>ListView
		''' </tr><tr>
		''' <td>HTML.Tag.DIR<td>ListView
		''' </tr><tr>
		''' <td>HTML.Tag.UL<td>ListView
		''' </tr><tr>
		''' <td>HTML.Tag.OL<td>ListView
		''' </tr><tr>
		''' <td>HTML.Tag.LI<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.DL<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.DD<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.BODY<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.HTML<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.CENTER<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.DIV<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.BLOCKQUOTE<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.PRE<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.BLOCKQUOTE<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.PRE<td>BlockView
		''' </tr><tr>
		''' <td>HTML.Tag.IMG<td>ImageView
		''' </tr><tr>
		''' <td>HTML.Tag.HR<td>HRuleView
		''' </tr><tr>
		''' <td>HTML.Tag.BR<td>BRView
		''' </tr><tr>
		''' <td>HTML.Tag.TABLE<td>javax.swing.text.html.TableView
		''' </tr><tr>
		''' <td>HTML.Tag.INPUT<td>FormView
		''' </tr><tr>
		''' <td>HTML.Tag.SELECT<td>FormView
		''' </tr><tr>
		''' <td>HTML.Tag.TEXTAREA<td>FormView
		''' </tr><tr>
		''' <td>HTML.Tag.OBJECT<td>ObjectView
		''' </tr><tr>
		''' <td>HTML.Tag.FRAMESET<td>FrameSetView
		''' </tr><tr>
		''' <td>HTML.Tag.FRAME<td>FrameView
		''' </tr>
		''' </table>
		''' </summary>
		Public Class HTMLFactory
			Implements ViewFactory

			''' <summary>
			''' Creates a view from an element.
			''' </summary>
			''' <param name="elem"> the element </param>
			''' <returns> the view </returns>
			Public Overridable Function create(ByVal elem As Element) As View Implements ViewFactory.create
				Dim attrs As AttributeSet = elem.attributes
				Dim elementName As Object = attrs.getAttribute(AbstractDocument.ElementNameAttribute)
				Dim o As Object = If(elementName IsNot Nothing, Nothing, attrs.getAttribute(StyleConstants.NameAttribute))
				If TypeOf o Is HTML.Tag Then
					Dim kind As HTML.Tag = CType(o, HTML.Tag)
					If kind Is HTML.Tag.CONTENT Then
						Return New InlineView(elem)
					ElseIf kind Is HTML.Tag.IMPLIED Then
						Dim ws As String = CStr(elem.attributes.getAttribute(CSS.Attribute.WHITE_SPACE))
						If (ws IsNot Nothing) AndAlso ws.Equals("pre") Then Return New LineView(elem)
						Return New javax.swing.text.html.ParagraphView(elem)
					ElseIf (kind Is HTML.Tag.P) OrElse (kind Is HTML.Tag.H1) OrElse (kind Is HTML.Tag.H2) OrElse (kind Is HTML.Tag.H3) OrElse (kind Is HTML.Tag.H4) OrElse (kind Is HTML.Tag.H5) OrElse (kind Is HTML.Tag.H6) OrElse (kind Is HTML.Tag.DT) Then
						' paragraph
						Return New javax.swing.text.html.ParagraphView(elem)
					ElseIf (kind Is HTML.Tag.MENU) OrElse (kind Is HTML.Tag.DIR) OrElse (kind Is HTML.Tag.UL) OrElse (kind Is HTML.Tag.OL) Then
						Return New ListView(elem)
					ElseIf kind Is HTML.Tag.BODY Then
						Return New BodyBlockView(elem)
					ElseIf kind Is HTML.Tag.HTML Then
						Return New BlockView(elem, View.Y_AXIS)
					ElseIf (kind Is HTML.Tag.LI) OrElse (kind Is HTML.Tag.CENTER) OrElse (kind Is HTML.Tag.DL) OrElse (kind Is HTML.Tag.DD) OrElse (kind Is HTML.Tag.DIV) OrElse (kind Is HTML.Tag.BLOCKQUOTE) OrElse (kind Is HTML.Tag.PRE) OrElse (kind Is HTML.Tag.FORM) Then
						' vertical box
						Return New BlockView(elem, View.Y_AXIS)
					ElseIf kind Is HTML.Tag.NOFRAMES Then
						Return New NoFramesView(elem, View.Y_AXIS)
					ElseIf kind Is HTML.Tag.IMG Then
						Return New ImageView(elem)
					ElseIf kind Is HTML.Tag.ISINDEX Then
						Return New IsindexView(elem)
					ElseIf kind Is HTML.Tag.HR Then
						Return New HRuleView(elem)
					ElseIf kind Is HTML.Tag.BR Then
						Return New BRView(elem)
					ElseIf kind Is HTML.Tag.TABLE Then
						Return New javax.swing.text.html.TableView(elem)
					ElseIf (kind Is HTML.Tag.INPUT) OrElse (kind Is HTML.Tag.SELECT) OrElse (kind Is HTML.Tag.TEXTAREA) Then
						Return New FormView(elem)
					ElseIf kind Is HTML.Tag.OBJECT Then
						Return New ObjectView(elem)
					ElseIf kind Is HTML.Tag.FRAMESET Then
						 If elem.attributes.isDefined(HTML.Attribute.ROWS) Then
							 Return New FrameSetView(elem, View.Y_AXIS)
						 ElseIf elem.attributes.isDefined(HTML.Attribute.COLS) Then
							 Return New FrameSetView(elem, View.X_AXIS)
						 End If
						 Throw New Exception("Can't build a" & kind & ", " & elem & ":" & "no ROWS or COLS defined.")
					ElseIf kind Is HTML.Tag.FRAME Then
						Return New FrameView(elem)
					ElseIf TypeOf kind Is HTML.UnknownTag Then
						Return New HiddenTagView(elem)
					ElseIf kind Is HTML.Tag.COMMENT Then
						Return New CommentView(elem)
					ElseIf kind Is HTML.Tag.HEAD Then
						' Make the head never visible, and never load its
						' children. For Cursor positioning,
						' getNextVisualPositionFrom is overriden to always return
						' the end offset of the element.
						Return New BlockViewAnonymousInnerClassHelper
					ElseIf (kind Is HTML.Tag.TITLE) OrElse (kind Is HTML.Tag.META) OrElse (kind Is HTML.Tag.LINK) OrElse (kind Is HTML.Tag.STYLE) OrElse (kind Is HTML.Tag.SCRIPT) OrElse (kind Is HTML.Tag.AREA) OrElse (kind Is HTML.Tag.MAP) OrElse (kind Is HTML.Tag.PARAM) OrElse (kind Is HTML.Tag.APPLET) Then
						Return New HiddenTagView(elem)
					End If
				End If
				' If we get here, it's either an element we don't know about
				' or something from StyledDocument that doesn't have a mapping to HTML.
				Dim nm As String = If(elementName IsNot Nothing, CStr(elementName), elem.name)
				If nm IsNot Nothing Then
					If nm.Equals(AbstractDocument.ContentElementName) Then
						Return New LabelView(elem)
					ElseIf nm.Equals(AbstractDocument.ParagraphElementName) Then
						Return New ParagraphView(elem)
					ElseIf nm.Equals(AbstractDocument.SectionElementName) Then
						Return New BoxView(elem, View.Y_AXIS)
					ElseIf nm.Equals(StyleConstants.ComponentElementName) Then
						Return New ComponentView(elem)
					ElseIf nm.Equals(StyleConstants.IconElementName) Then
						Return New IconView(elem)
					End If
				End If

				' default to text display
				Return New LabelView(elem)
			End Function

			Private Class BlockViewAnonymousInnerClassHelper
				Inherits BlockView

				Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
					Return 0
				End Function
				Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
					Return 0
				End Function
				Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
					Return 0
				End Function
				Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
				End Sub
				Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
					Return a
				End Function
				Public Overrides Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
					Return element.endOffset
				End Function
			End Class

			Friend Class BodyBlockView
				Inherits BlockView
				Implements ComponentListener

				Public Sub New(ByVal elem As Element)
					MyBase.New(elem,View.Y_AXIS)
				End Sub
				' reimplement major axis requirements to indicate that the
				' block is flexible for the body element... so that it can
				' be stretched to fill the background properly.
				Protected Friend Overridable Function calculateMajorAxisRequirements(ByVal axis As Integer, ByVal r As SizeRequirements) As SizeRequirements
					r = MyBase.calculateMajorAxisRequirements(axis, r)
					r.maximum = Integer.MaxValue
					Return r
				End Function

				Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
					Dim ___container As Container = container
					Dim parentContainer As Container
					parentContainer = ___container.parent
					If ___container IsNot Nothing AndAlso (TypeOf ___container Is javax.swing.JEditorPane) AndAlso parentContainer IsNot Nothing AndAlso (TypeOf parentContainer Is javax.swing.JViewport) Then
						Dim viewPort As JViewport = CType(parentContainer, JViewport)
						If cachedViewPort IsNot Nothing Then
							Dim cachedObject As JViewport = cachedViewPort.get()
							If cachedObject IsNot Nothing Then
								If cachedObject IsNot viewPort Then cachedObject.removeComponentListener(Me)
							Else
								cachedViewPort = Nothing
							End If
						End If
						If cachedViewPort Is Nothing Then
							viewPort.addComponentListener(Me)
							cachedViewPort = New WeakReference(Of JViewport)(viewPort)
						End If

						componentVisibleWidth = viewPort.extentSize.width
						If componentVisibleWidth > 0 Then
						Dim ___insets As Insets = ___container.insets
						viewVisibleWidth = componentVisibleWidth - ___insets.left - leftInset
						'try to use viewVisibleWidth if it is smaller than targetSpan
						targetSpan = Math.Min(targetSpan, viewVisibleWidth)
						End If
					Else
						If cachedViewPort IsNot Nothing Then
							Dim cachedObject As JViewport = cachedViewPort.get()
							If cachedObject IsNot Nothing Then cachedObject.removeComponentListener(Me)
							cachedViewPort = Nothing
						End If
					End If
					MyBase.layoutMinorAxis(targetSpan, axis, offsets, spans)
				End Sub

				Public Overrides Property parent As View
					Set(ByVal parent As View)
						'if parent == null unregister component listener
						If parent Is Nothing Then
							If cachedViewPort IsNot Nothing Then
								Dim cachedObject As Object
								cachedObject = cachedViewPort.get()
								If cachedObject IsNot Nothing Then CType(cachedObject, JComponent).removeComponentListener(Me)
								cachedViewPort = Nothing
							End If
						End If
						MyBase.parent = parent
					End Set
				End Property

				Public Overridable Sub componentResized(ByVal e As ComponentEvent)
					If Not(TypeOf e.source Is JViewport) Then Return
					Dim viewPort As JViewport = CType(e.source, JViewport)
					If componentVisibleWidth <> viewPort.extentSize.width Then
						Dim doc As Document = document
						If TypeOf doc Is AbstractDocument Then
							Dim ___document As AbstractDocument = CType(document, AbstractDocument)
							___document.readLock()
							Try
								layoutChanged(X_AXIS)
								preferenceChanged(Nothing, True, True)
							Finally
								___document.readUnlock()
							End Try

						End If
					End If
				End Sub
				Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
				End Sub
				Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
				End Sub
				Public Overridable Sub componentShown(ByVal e As ComponentEvent)
				End Sub
	'            
	'             * we keep weak reference to viewPort if and only if BodyBoxView is listening for ComponentEvents
	'             * only in that case cachedViewPort is not equal to null.
	'             * we need to keep this reference in order to remove BodyBoxView from viewPort listeners.
	'             *
	'             
				Private cachedViewPort As Reference(Of JViewport) = Nothing
				Private isListening As Boolean = False
				Private viewVisibleWidth As Integer = Integer.MAX_VALUE
				Private componentVisibleWidth As Integer = Integer.MAX_VALUE
			End Class

		End Class

		' --- Action implementations ------------------------------

	''' <summary>
	''' The bold action identifier
	''' </summary>
		Public Const BOLD_ACTION As String = "html-bold-action"
	''' <summary>
	''' The italic action identifier
	''' </summary>
		Public Const ITALIC_ACTION As String = "html-italic-action"
	''' <summary>
	''' The paragraph left indent action identifier
	''' </summary>
		Public Const PARA_INDENT_LEFT As String = "html-para-indent-left"
	''' <summary>
	''' The paragraph right indent action identifier
	''' </summary>
		Public Const PARA_INDENT_RIGHT As String = "html-para-indent-right"
	''' <summary>
	''' The  font size increase to next value action identifier
	''' </summary>
		Public Const FONT_CHANGE_BIGGER As String = "html-font-bigger"
	''' <summary>
	''' The font size decrease to next value action identifier
	''' </summary>
		Public Const FONT_CHANGE_SMALLER As String = "html-font-smaller"
	''' <summary>
	''' The Color choice action identifier
	'''     The color is passed as an argument
	''' </summary>
		Public Const COLOR_ACTION As String = "html-color-action"
	''' <summary>
	''' The logical style choice action identifier
	'''     The logical style is passed in as an argument
	''' </summary>
		Public Const LOGICAL_STYLE_ACTION As String = "html-logical-style-action"
		''' <summary>
		''' Align images at the top.
		''' </summary>
		Public Const IMG_ALIGN_TOP As String = "html-image-align-top"

		''' <summary>
		''' Align images in the middle.
		''' </summary>
		Public Const IMG_ALIGN_MIDDLE As String = "html-image-align-middle"

		''' <summary>
		''' Align images at the bottom.
		''' </summary>
		Public Const IMG_ALIGN_BOTTOM As String = "html-image-align-bottom"

		''' <summary>
		''' Align images at the border.
		''' </summary>
		Public Const IMG_BORDER As String = "html-image-border"


		''' <summary>
		''' HTML used when inserting tables. </summary>
		Private Const INSERT_TABLE_HTML As String = "<table border=1><tr><td></td></tr></table>"

		''' <summary>
		''' HTML used when inserting unordered lists. </summary>
		Private Const INSERT_UL_HTML As String = "<ul><li></li></ul>"

		''' <summary>
		''' HTML used when inserting ordered lists. </summary>
		Private Const INSERT_OL_HTML As String = "<ol><li></li></ol>"

		''' <summary>
		''' HTML used when inserting hr. </summary>
		Private Const INSERT_HR_HTML As String = "<hr>"

		''' <summary>
		''' HTML used when inserting pre. </summary>
		Private Const INSERT_PRE_HTML As String = "<pre></pre>"

		Private Shared ReadOnly nextLinkAction As New NavigateLinkAction("next-link-action")

		Private Shared ReadOnly previousLinkAction As New NavigateLinkAction("previous-link-action")

		Private Shared ReadOnly activateLinkAction As New ActivateLinkAction("activate-link-action")

		Private Shared ReadOnly defaultActions As Action() = { New InsertHTMLTextAction("InsertTable", INSERT_TABLE_HTML, HTML.Tag.BODY, HTML.Tag.TABLE), New InsertHTMLTextAction("InsertTableRow", INSERT_TABLE_HTML, HTML.Tag.TABLE, HTML.Tag.TR, HTML.Tag.BODY, HTML.Tag.TABLE), New InsertHTMLTextAction("InsertTableDataCell", INSERT_TABLE_HTML, HTML.Tag.TR, HTML.Tag.TD, HTML.Tag.BODY, HTML.Tag.TABLE), New InsertHTMLTextAction("InsertUnorderedList", INSERT_UL_HTML, HTML.Tag.BODY, HTML.Tag.UL), New InsertHTMLTextAction("InsertUnorderedListItem", INSERT_UL_HTML, HTML.Tag.UL, HTML.Tag.LI, HTML.Tag.BODY, HTML.Tag.UL), New InsertHTMLTextAction("InsertOrderedList", INSERT_OL_HTML, HTML.Tag.BODY, HTML.Tag.OL), New InsertHTMLTextAction("InsertOrderedListItem", INSERT_OL_HTML, HTML.Tag.OL, HTML.Tag.LI, HTML.Tag.BODY, HTML.Tag.OL), New InsertHRAction, New InsertHTMLTextAction("InsertPre", INSERT_PRE_HTML, HTML.Tag.BODY, HTML.Tag.PRE), nextLinkAction, previousLinkAction, activateLinkAction, New BeginAction(beginAction, False), New BeginAction(selectionBeginAction, True) }

		' link navigation support
		Private foundLink As Boolean = False
		Private prevHypertextOffset As Integer = -1
		Private linkNavigationTag As Object


		''' <summary>
		''' An abstract Action providing some convenience methods that may
		''' be useful in inserting HTML into an existing document.
		''' <p>NOTE: None of the convenience methods obtain a lock on the
		''' document. If you have another thread modifying the text these
		''' methods may have inconsistent behavior, or return the wrong thing.
		''' </summary>
		Public MustInherit Class HTMLTextAction
			Inherits StyledTextAction

			Public Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			''' <returns> HTMLDocument of <code>e</code>. </returns>
			Protected Friend Overridable Function getHTMLDocument(ByVal e As JEditorPane) As HTMLDocument
				Dim d As Document = e.document
				If TypeOf d Is HTMLDocument Then Return CType(d, HTMLDocument)
				Throw New System.ArgumentException("document must be HTMLDocument")
			End Function

			''' <returns> HTMLEditorKit for <code>e</code>. </returns>
			Protected Friend Overridable Function getHTMLEditorKit(ByVal e As JEditorPane) As HTMLEditorKit
				Dim k As EditorKit = e.editorKit
				If TypeOf k Is HTMLEditorKit Then Return CType(k, HTMLEditorKit)
				Throw New System.ArgumentException("EditorKit must be HTMLEditorKit")
			End Function

			''' <summary>
			''' Returns an array of the Elements that contain <code>offset</code>.
			''' The first elements corresponds to the root.
			''' </summary>
			Protected Friend Overridable Function getElementsAt(ByVal doc As HTMLDocument, ByVal offset As Integer) As Element()
				Return getElementsAt(doc.defaultRootElement, offset, 0)
			End Function

			''' <summary>
			''' Recursive method used by getElementsAt.
			''' </summary>
			Private Function getElementsAt(ByVal parent As Element, ByVal offset As Integer, ByVal depth As Integer) As Element()
				If parent.leaf Then
					Dim retValue As Element() = New Element(depth){}
					retValue(depth) = parent
					Return retValue
				End If
				Dim retValue As Element() = getElementsAt(parent.getElement(parent.getElementIndex(offset)), offset, depth + 1)
				retValue(depth) = parent
				Return retValue
			End Function

			''' <summary>
			''' Returns number of elements, starting at the deepest leaf, needed
			''' to get to an element representing <code>tag</code>. This will
			''' return -1 if no elements is found representing <code>tag</code>,
			''' or 0 if the parent of the leaf at <code>offset</code> represents
			''' <code>tag</code>.
			''' </summary>
			Protected Friend Overridable Function elementCountToTag(ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal tag As HTML.Tag) As Integer
				Dim depth As Integer = -1
				Dim e As Element = doc.getCharacterElement(offset)
				Do While e IsNot Nothing AndAlso e.attributes.getAttribute(StyleConstants.NameAttribute) IsNot tag
					e = e.parentElement
					depth += 1
				Loop
				If e Is Nothing Then Return -1
				Return depth
			End Function

			''' <summary>
			''' Returns the deepest element at <code>offset</code> matching
			''' <code>tag</code>.
			''' </summary>
			Protected Friend Overridable Function findElementMatchingTag(ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal tag As HTML.Tag) As Element
				Dim e As Element = doc.defaultRootElement
				Dim lastMatch As Element = Nothing
				Do While e IsNot Nothing
					If e.attributes.getAttribute(StyleConstants.NameAttribute) Is tag Then lastMatch = e
					e = e.getElement(e.getElementIndex(offset))
				Loop
				Return lastMatch
			End Function
		End Class


		''' <summary>
		''' InsertHTMLTextAction can be used to insert an arbitrary string of HTML
		''' into an existing HTML document. At least two HTML.Tags need to be
		''' supplied. The first Tag, parentTag, identifies the parent in
		''' the document to add the elements to. The second tag, addTag,
		''' identifies the first tag that should be added to the document as
		''' seen in the HTML string. One important thing to remember, is that
		''' the parser is going to generate all the appropriate tags, even if
		''' they aren't in the HTML string passed in.<p>
		''' For example, lets say you wanted to create an action to insert
		''' a table into the body. The parentTag would be HTML.Tag.BODY,
		''' addTag would be HTML.Tag.TABLE, and the string could be something
		''' like &lt;table&gt;&lt;tr&gt;&lt;td&gt;&lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;.
		''' <p>There is also an option to supply an alternate parentTag and
		''' addTag. These will be checked for if there is no parentTag at
		''' offset.
		''' </summary>
		Public Class InsertHTMLTextAction
			Inherits HTMLTextAction

			Public Sub New(ByVal name As String, ByVal html As String, ByVal parentTag As HTML.Tag, ByVal addTag As HTML.Tag)
				Me.New(name, html, parentTag, addTag, Nothing, Nothing)
			End Sub

			Public Sub New(ByVal name As String, ByVal html As String, ByVal parentTag As HTML.Tag, ByVal addTag As HTML.Tag, ByVal alternateParentTag As HTML.Tag, ByVal alternateAddTag As HTML.Tag)
				Me.New(name, html, parentTag, addTag, alternateParentTag, alternateAddTag, True)
			End Sub

			' public 
			Friend Sub New(ByVal name As String, ByVal html As String, ByVal parentTag As HTML.Tag, ByVal addTag As HTML.Tag, ByVal alternateParentTag As HTML.Tag, ByVal alternateAddTag As HTML.Tag, ByVal adjustSelection As Boolean)
				MyBase.New(name)
				Me.html = html
				Me.parentTag = parentTag
				Me.addTag = addTag
				Me.alternateParentTag = alternateParentTag
				Me.alternateAddTag = alternateAddTag
				Me.___adjustSelection = adjustSelection
			End Sub

			''' <summary>
			''' A cover for HTMLEditorKit.insertHTML. If an exception it
			''' thrown it is wrapped in a RuntimeException and thrown.
			''' </summary>
			Protected Friend Overridable Sub insertHTML(ByVal editor As JEditorPane, ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal html As String, ByVal popDepth As Integer, ByVal pushDepth As Integer, ByVal addTag As HTML.Tag)
				Try
					getHTMLEditorKit(editor).insertHTML(doc, offset, html, popDepth, pushDepth, addTag)
				Catch ioe As IOException
					Throw New Exception("Unable to insert: " & ioe)
				Catch ble As BadLocationException
					Throw New Exception("Unable to insert: " & ble)
				End Try
			End Sub

			''' <summary>
			''' This is invoked when inserting at a boundary. It determines
			''' the number of pops, and then the number of pushes that need
			''' to be performed, and then invokes insertHTML.
			''' @since 1.3
			''' </summary>
			Protected Friend Overridable Sub insertAtBoundary(ByVal editor As JEditorPane, ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal insertElement As Element, ByVal html As String, ByVal parentTag As HTML.Tag, ByVal addTag As HTML.Tag)
				insertAtBoundry(editor, doc, offset, insertElement, html, parentTag, addTag)
			End Sub

			''' <summary>
			''' This is invoked when inserting at a boundary. It determines
			''' the number of pops, and then the number of pushes that need
			''' to be performed, and then invokes insertHTML. </summary>
			''' @deprecated As of Java 2 platform v1.3, use insertAtBoundary 
			<Obsolete("As of Java 2 platform v1.3, use insertAtBoundary")> _
			Protected Friend Overridable Sub insertAtBoundry(ByVal editor As JEditorPane, ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal insertElement As Element, ByVal html As String, ByVal parentTag As HTML.Tag, ByVal addTag As HTML.Tag)
				' Find the common parent.
				Dim e As Element
				Dim commonParent As Element
				Dim isFirst As Boolean = (offset = 0)

				If offset > 0 OrElse insertElement Is Nothing Then
					e = doc.defaultRootElement
					Do While e IsNot Nothing AndAlso e.startOffset <> offset AndAlso Not e.leaf
						e = e.getElement(e.getElementIndex(offset))
					Loop
					commonParent = If(e IsNot Nothing, e.parentElement, Nothing)
				Else
					' If inserting at the origin, the common parent is the
					' insertElement.
					commonParent = insertElement
				End If
				If commonParent IsNot Nothing Then
					' Determine how many pops to do.
					Dim pops As Integer = 0
					Dim pushes As Integer = 0
					If isFirst AndAlso insertElement IsNot Nothing Then
						e = commonParent
						Do While e IsNot Nothing AndAlso Not e.leaf
							e = e.getElement(e.getElementIndex(offset))
							pops += 1
						Loop
					Else
						e = commonParent
						offset -= 1
						Do While e IsNot Nothing AndAlso Not e.leaf
							e = e.getElement(e.getElementIndex(offset))
							pops += 1
						Loop

						' And how many pushes
						e = commonParent
						offset += 1
						Do While e IsNot Nothing AndAlso e IsNot insertElement
							e = e.getElement(e.getElementIndex(offset))
							pushes += 1
						Loop
					End If
					pops = Math.Max(0, pops - 1)

					' And insert!
					insertHTML(editor, doc, offset, html, pops, pushes, addTag)
				End If
			End Sub

			''' <summary>
			''' If there is an Element with name <code>tag</code> at
			''' <code>offset</code>, this will invoke either insertAtBoundary
			''' or <code>insertHTML</code>. This returns true if there is
			''' a match, and one of the inserts is invoked.
			''' </summary>
			'protected
			Friend Overridable Function insertIntoTag(ByVal editor As JEditorPane, ByVal doc As HTMLDocument, ByVal offset As Integer, ByVal tag As HTML.Tag, ByVal addTag As HTML.Tag) As Boolean
				Dim e As Element = findElementMatchingTag(doc, offset, tag)
				If e IsNot Nothing AndAlso e.startOffset = offset Then
					insertAtBoundary(editor, doc, offset, e, html, tag, addTag)
					Return True
				ElseIf offset > 0 Then
					Dim depth As Integer = elementCountToTag(doc, offset - 1, tag)
					If depth <> -1 Then
						insertHTML(editor, doc, offset, html, depth, 0, addTag)
						Return True
					End If
				End If
				Return False
			End Function

			''' <summary>
			''' Called after an insertion to adjust the selection.
			''' </summary>
			' protected 
			Friend Overridable Sub adjustSelection(ByVal pane As JEditorPane, ByVal doc As HTMLDocument, ByVal startOffset As Integer, ByVal oldLength As Integer)
				Dim newLength As Integer = doc.length
				If newLength <> oldLength AndAlso startOffset < newLength Then
					If startOffset > 0 Then
						Dim text As String
						Try
							text = doc.getText(startOffset - 1, 1)
						Catch ble As BadLocationException
							text = Nothing
						End Try
						If text IsNot Nothing AndAlso text.Length > 0 AndAlso text.Chars(0) = ControlChars.Lf Then
							pane.select(startOffset, startOffset)
						Else
							pane.select(startOffset + 1, startOffset + 1)
						End If
					Else
						pane.select(1, 1)
					End If
				End If
			End Sub

			''' <summary>
			''' Inserts the HTML into the document.
			''' </summary>
			''' <param name="ae"> the event </param>
			Public Overridable Sub actionPerformed(ByVal ae As ActionEvent)
				Dim editor As JEditorPane = getEditor(ae)
				If editor IsNot Nothing Then
					Dim doc As HTMLDocument = getHTMLDocument(editor)
					Dim offset As Integer = editor.selectionStart
					Dim length As Integer = doc.length
					Dim inserted As Boolean
					' Try first choice
					If (Not insertIntoTag(editor, doc, offset, parentTag, addTag)) AndAlso alternateParentTag IsNot Nothing Then
						' Then alternate.
						inserted = insertIntoTag(editor, doc, offset, alternateParentTag, alternateAddTag)
					Else
						inserted = True
					End If
					If ___adjustSelection AndAlso inserted Then adjustSelection(editor, doc, offset, length)
				End If
			End Sub

			''' <summary>
			''' HTML to insert. </summary>
			Protected Friend html As String
			''' <summary>
			''' Tag to check for in the document. </summary>
			Protected Friend parentTag As HTML.Tag
			''' <summary>
			''' Tag in HTML to start adding tags from. </summary>
			Protected Friend addTag As HTML.Tag
			''' <summary>
			''' Alternate Tag to check for in the document if parentTag is
			''' not found. 
			''' </summary>
			Protected Friend alternateParentTag As HTML.Tag
			''' <summary>
			''' Alternate tag in HTML to start adding tags from if parentTag
			''' is not found and alternateParentTag is found. 
			''' </summary>
			Protected Friend alternateAddTag As HTML.Tag
			''' <summary>
			''' True indicates the selection should be adjusted after an insert. </summary>
			Friend ___adjustSelection As Boolean
		End Class


		''' <summary>
		''' InsertHRAction is special, at actionPerformed time it will determine
		''' the parent HTML.Tag based on the paragraph element at the selection
		''' start.
		''' </summary>
		Friend Class InsertHRAction
			Inherits InsertHTMLTextAction

			Friend Sub New()
				MyBase.New("InsertHR", "<hr>", Nothing, HTML.Tag.IMPLIED, Nothing, Nothing, False)
			End Sub

			''' <summary>
			''' Inserts the HTML into the document.
			''' </summary>
			''' <param name="ae"> the event </param>
			Public Overrides Sub actionPerformed(ByVal ae As ActionEvent)
				Dim editor As JEditorPane = getEditor(ae)
				If editor IsNot Nothing Then
					Dim doc As HTMLDocument = getHTMLDocument(editor)
					Dim offset As Integer = editor.selectionStart
					Dim paragraph As Element = doc.getParagraphElement(offset)
					If paragraph.parentElement IsNot Nothing Then
						parentTag = CType(paragraph.parentElement.attributes.getAttribute(StyleConstants.NameAttribute), HTML.Tag)
						MyBase.actionPerformed(ae)
					End If
				End If
			End Sub

		End Class

	'    
	'     * Returns the object in an AttributeSet matching a key
	'     
		Private Shared Function getAttrValue(ByVal attr As AttributeSet, ByVal key As HTML.Attribute) As Object
			Dim names As System.Collections.IEnumerator = attr.attributeNames
			Do While names.hasMoreElements()
				Dim nextKey As Object = names.nextElement()
				Dim nextVal As Object = attr.getAttribute(nextKey)
				If TypeOf nextVal Is AttributeSet Then
					Dim value As Object = getAttrValue(CType(nextVal, AttributeSet), key)
					If value IsNot Nothing Then Return value
				ElseIf nextKey Is key Then
					Return nextVal
				End If
			Loop
			Return Nothing
		End Function

	'    
	'     * Action to move the focus on the next or previous hypertext link
	'     * or object. TODO: This method relies on support from the
	'     * javax.accessibility package.  The text package should support
	'     * keyboard navigation of text elements directly.
	'     
		Friend Class NavigateLinkAction
			Inherits TextAction
			Implements CaretListener

			Private Shared ReadOnly focusPainter As New FocusHighlightPainter(Nothing)
			Private ReadOnly focusBack As Boolean

	'        
	'         * Create this action with the appropriate identifier.
	'         
			Public Sub New(ByVal actionName As String)
				MyBase.New(actionName)
				focusBack = "previous-link-action".Equals(actionName)
			End Sub

			''' <summary>
			''' Called when the caret position is updated.
			''' </summary>
			''' <param name="e"> the caret event </param>
			Public Overridable Sub caretUpdate(ByVal e As CaretEvent) Implements CaretListener.caretUpdate
				Dim src As Object = e.source
				If TypeOf src Is JTextComponent Then
					Dim comp As JTextComponent = CType(src, JTextComponent)
					Dim kit As HTMLEditorKit = getHTMLEditorKit(comp)
					If kit IsNot Nothing AndAlso kit.foundLink Then
						kit.foundLink = False
						' TODO: The AccessibleContext for the editor should register
						' as a listener for CaretEvents and forward the events to
						' assistive technologies listening for such events.
						comp.accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_HYPERTEXT_OFFSET, Convert.ToInt32(kit.prevHypertextOffset), Convert.ToInt32(e.dot))
					End If
				End If
			End Sub

	'        
	'         * The operation to perform when this action is triggered.
	'         
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim comp As JTextComponent = getTextComponent(e)
				If comp Is Nothing OrElse comp.editable Then Return

				Dim doc As Document = comp.document
				Dim kit As HTMLEditorKit = getHTMLEditorKit(comp)
				If doc Is Nothing OrElse kit Is Nothing Then Return

				' TODO: Should start successive iterations from the
				' current caret position.
				Dim ei As New ElementIterator(doc)
				Dim currentOffset As Integer = comp.caretPosition
				Dim prevStartOffset As Integer = -1
				Dim prevEndOffset As Integer = -1

				' highlight the next link or object after the current caret position
				Dim nextElement As Element
				nextElement = ei.next()
				Do While nextElement IsNot Nothing
					Dim name As String = nextElement.name
					Dim attr As AttributeSet = nextElement.attributes

					Dim href As Object = getAttrValue(attr, HTML.Attribute.HREF)
					If Not(name.Equals(HTML.Tag.OBJECT.ToString())) AndAlso href Is Nothing Then
						nextElement = ei.next()
						Continue Do
					End If

					Dim elementOffset As Integer = nextElement.startOffset
					If focusBack Then
						If elementOffset >= currentOffset AndAlso prevStartOffset >= 0 Then

							kit.foundLink = True
							comp.caretPosition = prevStartOffset
							moveCaretPosition(comp, kit, prevStartOffset, prevEndOffset)
							kit.prevHypertextOffset = prevStartOffset
							Return
						End If ' focus forward
					Else
						If elementOffset > currentOffset Then

							kit.foundLink = True
							comp.caretPosition = elementOffset
							moveCaretPosition(comp, kit, elementOffset, nextElement.endOffset)
							kit.prevHypertextOffset = elementOffset
							Return
						End If
					End If
					prevStartOffset = nextElement.startOffset
					prevEndOffset = nextElement.endOffset
					nextElement = ei.next()
				Loop
				If focusBack AndAlso prevStartOffset >= 0 Then
					kit.foundLink = True
					comp.caretPosition = prevStartOffset
					moveCaretPosition(comp, kit, prevStartOffset, prevEndOffset)
					kit.prevHypertextOffset = prevStartOffset
				End If
			End Sub

	'        
	'         * Moves the caret from mark to dot
	'         
			Private Sub moveCaretPosition(ByVal comp As JTextComponent, ByVal kit As HTMLEditorKit, ByVal mark As Integer, ByVal dot As Integer)
				Dim h As Highlighter = comp.highlighter
				If h IsNot Nothing Then
					Dim p0 As Integer = Math.Min(dot, mark)
					Dim p1 As Integer = Math.Max(dot, mark)
					Try
						If kit.linkNavigationTag IsNot Nothing Then
							h.changeHighlight(kit.linkNavigationTag, p0, p1)
						Else
							kit.linkNavigationTag = h.addHighlight(p0, p1, focusPainter)
						End If
					Catch e As BadLocationException
					End Try
				End If
			End Sub

			Private Function getHTMLEditorKit(ByVal comp As JTextComponent) As HTMLEditorKit
				If TypeOf comp Is JEditorPane Then
					Dim kit As EditorKit = CType(comp, JEditorPane).editorKit
					If TypeOf kit Is HTMLEditorKit Then Return CType(kit, HTMLEditorKit)
				End If
				Return Nothing
			End Function

			''' <summary>
			''' A highlight painter that draws a one-pixel border around
			''' the highlighted area.
			''' </summary>
			Friend Class FocusHighlightPainter
				Inherits DefaultHighlighter.DefaultHighlightPainter

				Friend Sub New(ByVal color As Color)
					MyBase.New(color)
				End Sub

				''' <summary>
				''' Paints a portion of a highlight.
				''' </summary>
				''' <param name="g"> the graphics context </param>
				''' <param name="offs0"> the starting model offset &ge; 0 </param>
				''' <param name="offs1"> the ending model offset &ge; offs1 </param>
				''' <param name="bounds"> the bounding box of the view, which is not
				'''        necessarily the region to paint. </param>
				''' <param name="c"> the editor </param>
				''' <param name="view"> View painting for </param>
				''' <returns> region in which drawing occurred </returns>
				Public Overridable Function paintLayer(ByVal g As Graphics, ByVal offs0 As Integer, ByVal offs1 As Integer, ByVal bounds As Shape, ByVal c As JTextComponent, ByVal ___view As View) As Shape

					Dim ___color As Color = color

					If ___color Is Nothing Then
						g.color = c.selectionColor
					Else
						g.color = ___color
					End If
					If offs0 = ___view.startOffset AndAlso offs1 = ___view.endOffset Then
						' Contained in view, can just use bounds.
						Dim alloc As Rectangle
						If TypeOf bounds Is Rectangle Then
							alloc = CType(bounds, Rectangle)
						Else
							alloc = bounds.bounds
						End If
						g.drawRect(alloc.x, alloc.y, alloc.width - 1, alloc.height)
						Return alloc
					Else
						' Should only render part of View.
						Try
							' --- determine locations ---
							Dim shape As Shape = ___view.modelToView(offs0, Position.Bias.Forward, offs1,Position.Bias.Backward, bounds)
							Dim r As Rectangle = If(TypeOf shape Is Rectangle, CType(shape, Rectangle), shape.bounds)
							g.drawRect(r.x, r.y, r.width - 1, r.height)
							Return r
						Catch e As BadLocationException
							' can't render
						End Try
					End If
					' Only if exception
					Return Nothing
				End Function
			End Class
		End Class

	'    
	'     * Action to activate the hypertext link that has focus.
	'     * TODO: This method relies on support from the
	'     * javax.accessibility package.  The text package should support
	'     * keyboard navigation of text elements directly.
	'     
		Friend Class ActivateLinkAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier.
			''' </summary>
			Public Sub New(ByVal actionName As String)
				MyBase.New(actionName)
			End Sub

	'        
	'         * activates the hyperlink at offset
	'         
			Private Sub activateLink(ByVal href As String, ByVal doc As HTMLDocument, ByVal editor As JEditorPane, ByVal offset As Integer)
				Try
					Dim page As java.net.URL = CType(doc.getProperty(Document.StreamDescriptionProperty), java.net.URL)
					Dim url As New java.net.URL(page, href)
					Dim linkEvent As New HyperlinkEvent(editor, HyperlinkEvent.EventType.ACTIVATED, url, url.toExternalForm(), doc.getCharacterElement(offset))
					editor.fireHyperlinkUpdate(linkEvent)
				Catch m As java.net.MalformedURLException
				End Try
			End Sub

	'        
	'         * Invokes default action on the object in an element
	'         
			Private Sub doObjectAction(ByVal editor As JEditorPane, ByVal elem As Element)
				Dim ___view As View = getView(editor, elem)
				If ___view IsNot Nothing AndAlso TypeOf ___view Is ObjectView Then
					Dim comp As Component = CType(___view, ObjectView).component
					If comp IsNot Nothing AndAlso TypeOf comp Is Accessible Then
						Dim ac As AccessibleContext = comp.accessibleContext
						If ac IsNot Nothing Then
							Dim aa As AccessibleAction = ac.accessibleAction
							If aa IsNot Nothing Then aa.doAccessibleAction(0)
						End If
					End If
				End If
			End Sub

	'        
	'         * Returns the root view for a document
	'         
			Private Function getRootView(ByVal editor As JEditorPane) As View
				Return editor.uI.getRootView(editor)
			End Function

	'        
	'         * Returns a view associated with an element
	'         
			Private Function getView(ByVal editor As JEditorPane, ByVal elem As Element) As View
				Dim lock As Object = lock(editor)
				Try
					Dim ___rootView As View = getRootView(editor)
					Dim start As Integer = elem.startOffset
					If ___rootView IsNot Nothing Then Return getView(___rootView, elem, start)
					Return Nothing
				Finally
					unlock(lock)
				End Try
			End Function

			Private Function getView(ByVal parent As View, ByVal elem As Element, ByVal start As Integer) As View
				If parent.element Is elem Then Return parent
				Dim index As Integer = parent.getViewIndex(start, Position.Bias.Forward)

				If index <> -1 AndAlso index < parent.viewCount Then Return getView(parent.getView(index), elem, start)
				Return Nothing
			End Function

	'        
	'         * If possible acquires a lock on the Document.  If a lock has been
	'         * obtained a key will be retured that should be passed to
	'         * <code>unlock</code>.
	'         
			Private Function lock(ByVal editor As JEditorPane) As Object
				Dim document As Document = editor.document

				If TypeOf document Is AbstractDocument Then
					CType(document, AbstractDocument).readLock()
					Return document
				End If
				Return Nothing
			End Function

	'        
	'         * Releases a lock previously obtained via <code>lock</code>.
	'         
			Private Sub unlock(ByVal key As Object)
				If key IsNot Nothing Then CType(key, AbstractDocument).readUnlock()
			End Sub

	'        
	'         * The operation to perform when this action is triggered.
	'         
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)

				Dim c As JTextComponent = getTextComponent(e)
				If c.editable OrElse Not(TypeOf c Is JEditorPane) Then Return
				Dim editor As JEditorPane = CType(c, JEditorPane)

				Dim d As Document = editor.document
				If d Is Nothing OrElse Not(TypeOf d Is HTMLDocument) Then Return
				Dim doc As HTMLDocument = CType(d, HTMLDocument)

				Dim ei As New ElementIterator(doc)
				Dim currentOffset As Integer = editor.caretPosition

				' invoke the next link or object action
				Dim urlString As String = Nothing
				Dim objString As String = Nothing
				Dim currentElement As Element
				currentElement = ei.next()
				Do While currentElement IsNot Nothing
					Dim name As String = currentElement.name
					Dim attr As AttributeSet = currentElement.attributes

					Dim href As Object = getAttrValue(attr, HTML.Attribute.HREF)
					If href IsNot Nothing Then
						If currentOffset >= currentElement.startOffset AndAlso currentOffset <= currentElement.endOffset Then

							activateLink(CStr(href), doc, editor, currentOffset)
							Return
						End If
					ElseIf name.Equals(HTML.Tag.OBJECT.ToString()) Then
						Dim obj As Object = getAttrValue(attr, HTML.Attribute.CLASSID)
						If obj IsNot Nothing Then
							If currentOffset >= currentElement.startOffset AndAlso currentOffset <= currentElement.endOffset Then

								doObjectAction(editor, currentElement)
								Return
							End If
						End If
					End If
					currentElement = ei.next()
				Loop
			End Sub
		End Class

		Private Shared Function getBodyElementStart(ByVal comp As JTextComponent) As Integer
			Dim rootElement As Element = comp.document.rootElements(0)
			For i As Integer = 0 To rootElement.elementCount - 1
				Dim currElement As Element = rootElement.getElement(i)
				If "body".Equals(currElement.name) Then Return currElement.startOffset
			Next i
			Return 0
		End Function

	'    
	'     * Move the caret to the beginning of the document.
	'     * @see DefaultEditorKit#beginAction
	'     * @see HTMLEditorKit#getActions
	'     

		Friend Class BeginAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				Dim bodyStart As Integer = getBodyElementStart(target)

				If target IsNot Nothing Then
					If [select] Then
						target.moveCaretPosition(bodyStart)
					Else
						target.caretPosition = bodyStart
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class
	End Class

End Namespace