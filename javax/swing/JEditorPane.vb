Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports javax.swing.plaf
Imports javax.swing.text
Imports javax.swing.event
Imports javax.swing.text.html
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
Namespace javax.swing



	''' <summary>
	''' A text component to edit various kinds of content.
	''' You can find how-to information and examples of using editor panes in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/text.html">Using Text Components</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' 
	''' <p>
	''' This component uses implementations of the
	''' <code>EditorKit</code> to accomplish its behavior. It effectively
	''' morphs into the proper kind of text editor for the kind
	''' of content it is given.  The content type that editor is bound
	''' to at any given time is determined by the <code>EditorKit</code> currently
	''' installed.  If the content is set to a new URL, its type is used
	''' to determine the <code>EditorKit</code> that should be used to
	''' load the content.
	''' <p>
	''' By default, the following types of content are known:
	''' <dl>
	''' <dt><b>text/plain</b>
	''' <dd>Plain text, which is the default the type given isn't
	''' recognized.  The kit used in this case is an extension of
	''' <code>DefaultEditorKit</code> that produces a wrapped plain text view.
	''' <dt><b>text/html</b>
	''' <dd>HTML text.  The kit used in this case is the class
	''' <code>javax.swing.text.html.HTMLEditorKit</code>
	''' which provides HTML 3.2 support.
	''' <dt><b>text/rtf</b>
	''' <dd>RTF text.  The kit used in this case is the class
	''' <code>javax.swing.text.rtf.RTFEditorKit</code>
	''' which provides a limited support of the Rich Text Format.
	''' </dl>
	''' <p>
	''' There are several ways to load content into this component.
	''' <ol>
	''' <li>
	''' The <seealso cref="#setText setText"/> method can be used to initialize
	''' the component from a string.  In this case the current
	''' <code>EditorKit</code> will be used, and the content type will be
	''' expected to be of this type.
	''' <li>
	''' The <seealso cref="#read read"/> method can be used to initialize the
	''' component from a <code>Reader</code>.  Note that if the content type is HTML,
	''' relative references (e.g. for things like images) can't be resolved
	''' unless the &lt;base&gt; tag is used or the <em>Base</em> property
	''' on <code>HTMLDocument</code> is set.
	''' In this case the current <code>EditorKit</code> will be used,
	''' and the content type will be expected to be of this type.
	''' <li>
	''' The <seealso cref="#setPage setPage"/> method can be used to initialize
	''' the component from a URL.  In this case, the content type will be
	''' determined from the URL, and the registered <code>EditorKit</code>
	''' for that content type will be set.
	''' </ol>
	''' <p>
	''' Some kinds of content may provide hyperlink support by generating
	''' hyperlink events.  The HTML <code>EditorKit</code> will generate
	''' hyperlink events if the <code>JEditorPane</code> is <em>not editable</em>
	''' (<code>JEditorPane.setEditable(false);</code> has been called).
	''' If HTML frames are embedded in the document, the typical response would be
	''' to change a portion of the current document.  The following code
	''' fragment is a possible hyperlink listener implementation, that treats
	''' HTML frame events specially, and simply displays any other activated
	''' hyperlinks.
	''' <pre>
	''' 
	''' &nbsp;    class Hyperactive implements HyperlinkListener {
	''' &nbsp;
	''' &nbsp;        public void hyperlinkUpdate(HyperlinkEvent e) {
	''' &nbsp;            if (e.getEventType() == HyperlinkEvent.EventType.ACTIVATED) {
	''' &nbsp;                JEditorPane pane = (JEditorPane) e.getSource();
	''' &nbsp;                if (e instanceof HTMLFrameHyperlinkEvent) {
	''' &nbsp;                    HTMLFrameHyperlinkEvent  evt = (HTMLFrameHyperlinkEvent)e;
	''' &nbsp;                    HTMLDocument doc = (HTMLDocument)pane.getDocument();
	''' &nbsp;                    doc.processHTMLFrameHyperlinkEvent(evt);
	''' &nbsp;                } else {
	''' &nbsp;                    try {
	''' &nbsp;                        pane.setPage(e.getURL());
	''' &nbsp;                    } catch (Throwable t) {
	''' &nbsp;                        t.printStackTrace();
	''' &nbsp;                    }
	''' &nbsp;                }
	''' &nbsp;            }
	''' &nbsp;        }
	''' &nbsp;    }
	''' 
	''' </pre>
	''' <p>
	''' For information on customizing how <b>text/html</b> is rendered please see
	''' <seealso cref="#W3C_LENGTH_UNITS"/> and <seealso cref="#HONOR_DISPLAY_PROPERTIES"/>
	''' <p>
	''' Culturally dependent information in some documents is handled through
	''' a mechanism called character encoding.  Character encoding is an
	''' unambiguous mapping of the members of a character set (letters, ideographs,
	''' digits, symbols, or control functions) to specific numeric code values. It
	''' represents the way the file is stored. Example character encodings are
	''' ISO-8859-1, ISO-8859-5, Shift-jis, Euc-jp, and UTF-8. When the file is
	''' passed to an user agent (<code>JEditorPane</code>) it is converted to
	''' the document character set (ISO-10646 aka Unicode).
	''' <p>
	''' There are multiple ways to get a character set mapping to happen
	''' with <code>JEditorPane</code>.
	''' <ol>
	''' <li>
	''' One way is to specify the character set as a parameter of the MIME
	''' type.  This will be established by a call to the
	''' <seealso cref="#setContentType setContentType"/> method.  If the content
	''' is loaded by the <seealso cref="#setPage setPage"/> method the content
	''' type will have been set according to the specification of the URL.
	''' It the file is loaded directly, the content type would be expected to
	''' have been set prior to loading.
	''' <li>
	''' Another way the character set can be specified is in the document itself.
	''' This requires reading the document prior to determining the character set
	''' that is desired.  To handle this, it is expected that the
	''' <code>EditorKit</code>.read operation throw a
	''' <code>ChangedCharSetException</code> which will
	''' be caught.  The read is then restarted with a new Reader that uses
	''' the character set specified in the <code>ChangedCharSetException</code>
	''' (which is an <code>IOException</code>).
	''' </ol>
	''' <p>
	''' <dl>
	''' <dt><b><font size=+1>Newlines</font></b>
	''' <dd>
	''' For a discussion on how newlines are handled, see
	''' <a href="text/DefaultEditorKit.html">DefaultEditorKit</a>.
	''' </dl>
	''' 
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A text component to edit various types of content.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class JEditorPane
		Inherits JTextComponent

		''' <summary>
		''' Creates a new <code>JEditorPane</code>.
		''' The document model is set to <code>null</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()
			focusCycleRoot = True
			focusTraversalPolicyicy(New LayoutFocusTraversalPolicyAnonymousInnerClassHelper
			LookAndFeel.installProperty(Me, "focusTraversalKeysForward", JComponent.managingFocusForwardTraversalKeys)
			LookAndFeel.installProperty(Me, "focusTraversalKeysBackward", JComponent.managingFocusBackwardTraversalKeys)
		End Sub

		Private Class LayoutFocusTraversalPolicyAnonymousInnerClassHelper
			Inherits LayoutFocusTraversalPolicy

			Public Overridable Function getComponentAfter(ByVal focusCycleRoot As Container, ByVal aComponent As Component) As Component
				If focusCycleRoot IsNot JEditorPane.this OrElse ((Not outerInstance.editable) AndAlso componentCount > 0) Then
					Return MyBase.getComponentAfter(focusCycleRoot, aComponent)
				Else
					Dim rootAncestor As Container = focusCycleRootAncestor
					Return If(rootAncestor IsNot Nothing, rootAncestor.focusTraversalPolicy.getComponentAfter(rootAncestor, JEditorPane.this), Nothing)
				End If
			End Function
			Public Overridable Function getComponentBefore(ByVal focusCycleRoot As Container, ByVal aComponent As Component) As Component
				If focusCycleRoot IsNot JEditorPane.this OrElse ((Not outerInstance.editable) AndAlso componentCount > 0) Then
					Return MyBase.getComponentBefore(focusCycleRoot, aComponent)
				Else
					Dim rootAncestor As Container = focusCycleRootAncestor
					Return If(rootAncestor IsNot Nothing, rootAncestor.focusTraversalPolicy.getComponentBefore(rootAncestor, JEditorPane.this), Nothing)
				End If
			End Function
			Public Overridable Function getDefaultComponent(ByVal focusCycleRoot As Container) As Component
				Return If(focusCycleRoot IsNot JEditorPane.this OrElse ((Not outerInstance.editable) AndAlso componentCount > 0), MyBase.getDefaultComponent(focusCycleRoot), Nothing)
			End Function
			Protected Friend Overridable Function accept(ByVal aComponent As Component) As Boolean
				Return If(aComponent IsNot JEditorPane.this, MyBase.accept(aComponent), False)
			End Function
		End Class

		''' <summary>
		''' Creates a <code>JEditorPane</code> based on a specified URL for input.
		''' </summary>
		''' <param name="initialPage"> the URL </param>
		''' <exception cref="IOException"> if the URL is <code>null</code>
		'''          or cannot be accessed </exception>
		Public Sub New(ByVal initialPage As URL)
			Me.New()
			page = initialPage
		End Sub

		''' <summary>
		''' Creates a <code>JEditorPane</code> based on a string containing
		''' a URL specification.
		''' </summary>
		''' <param name="url"> the URL </param>
		''' <exception cref="IOException"> if the URL is <code>null</code> or
		'''          cannot be accessed </exception>
		Public Sub New(ByVal url As String)
			Me.New()
			page = url
		End Sub

		''' <summary>
		''' Creates a <code>JEditorPane</code> that has been initialized
		''' to the given text.  This is a convenience constructor that calls the
		''' <code>setContentType</code> and <code>setText</code> methods.
		''' </summary>
		''' <param name="type"> mime type of the given text </param>
		''' <param name="text"> the text to initialize with; may be <code>null</code> </param>
		''' <exception cref="NullPointerException"> if the <code>type</code> parameter
		'''          is <code>null</code> </exception>
		Public Sub New(ByVal type As String, ByVal text As String)
			Me.New()
			contentType = type
			text = text
		End Sub

		''' <summary>
		''' Adds a hyperlink listener for notification of any changes, for example
		''' when a link is selected and entered.
		''' </summary>
		''' <param name="listener"> the listener </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addHyperlinkListener(ByVal listener As HyperlinkListener)
			listenerList.add(GetType(HyperlinkListener), listener)
		End Sub

		''' <summary>
		''' Removes a hyperlink listener.
		''' </summary>
		''' <param name="listener"> the listener </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeHyperlinkListener(ByVal listener As HyperlinkListener)
			listenerList.remove(GetType(HyperlinkListener), listener)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>HyperLinkListener</code>s added
		''' to this JEditorPane with addHyperlinkListener().
		''' </summary>
		''' <returns> all of the <code>HyperLinkListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property hyperlinkListeners As HyperlinkListener()
			Get
				Return listenerList.getListeners(GetType(javax.swing.event.HyperlinkListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  This is normally called
		''' by the currently installed <code>EditorKit</code> if a content type
		''' that supports hyperlinks is currently active and there
		''' was activity with a link.  The listener list is processed
		''' last to first.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireHyperlinkUpdate(ByVal e As HyperlinkEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(HyperlinkListener) Then CType(___listeners(i+1), HyperlinkListener).hyperlinkUpdate(e)
			Next i
		End Sub


		''' <summary>
		''' Sets the current URL being displayed.  The content type of the
		''' pane is set, and if the editor kit for the pane is
		''' non-<code>null</code>, then
		''' a new default document is created and the URL is read into it.
		''' If the URL contains and reference location, the location will
		''' be scrolled to by calling the <code>scrollToReference</code>
		''' method. If the desired URL is the one currently being displayed,
		''' the document will not be reloaded. To force a document
		''' reload it is necessary to clear the stream description property
		''' of the document. The following code shows how this can be done:
		''' 
		''' <pre>
		'''   Document doc = jEditorPane.getDocument();
		'''   doc.putProperty(Document.StreamDescriptionProperty, null);
		''' </pre>
		''' 
		''' If the desired URL is not the one currently being
		''' displayed, the <code>getStream</code> method is called to
		''' give subclasses control over the stream provided.
		''' <p>
		''' This may load either synchronously or asynchronously
		''' depending upon the document returned by the <code>EditorKit</code>.
		''' If the <code>Document</code> is of type
		''' <code>AbstractDocument</code> and has a value returned by
		''' <code>AbstractDocument.getAsynchronousLoadPriority</code>
		''' that is greater than or equal to zero, the page will be
		''' loaded on a separate thread using that priority.
		''' <p>
		''' If the document is loaded synchronously, it will be
		''' filled in with the stream prior to being installed into
		''' the editor with a call to <code>setDocument</code>, which
		''' is bound and will fire a property change event.  If an
		''' <code>IOException</code> is thrown the partially loaded
		''' document will
		''' be discarded and neither the document or page property
		''' change events will be fired.  If the document is
		''' successfully loaded and installed, a view will be
		''' built for it by the UI which will then be scrolled if
		''' necessary, and then the page property change event
		''' will be fired.
		''' <p>
		''' If the document is loaded asynchronously, the document
		''' will be installed into the editor immediately using a
		''' call to <code>setDocument</code> which will fire a
		''' document property change event, then a thread will be
		''' created which will begin doing the actual loading.
		''' In this case, the page property change event will not be
		''' fired by the call to this method directly, but rather will be
		''' fired when the thread doing the loading has finished.
		''' It will also be fired on the event-dispatch thread.
		''' Since the calling thread can not throw an <code>IOException</code>
		''' in the event of failure on the other thread, the page
		''' property change event will be fired when the other
		''' thread is done whether the load was successful or not.
		''' </summary>
		''' <param name="page"> the URL of the page </param>
		''' <exception cref="IOException"> for a <code>null</code> or invalid
		'''          page specification, or exception from the stream being read </exception>
		''' <seealso cref= #getPage
		''' @beaninfo
		'''  description: the URL used to set content
		'''        bound: true
		'''       expert: true </seealso>
		Public Overridable Property page As URL
			Set(ByVal page As URL)
				If page Is Nothing Then Throw New IOException("invalid url")
				Dim loaded As URL = page
    
    
				' reset scrollbar
				If (Not page.Equals(loaded)) AndAlso page.ref Is Nothing Then scrollRectToVisible(New Rectangle(0,0,1,1))
				Dim reloaded As Boolean = False
				Dim ___postData As Object = postData
				If (loaded Is Nothing) OrElse (Not loaded.sameFile(page)) OrElse (___postData IsNot Nothing) Then
					' different url or POST method, load the new content
    
					Dim p As Integer = getAsynchronousLoadPriority(document)
					If p < 0 Then
						' open stream synchronously
						Dim [in] As InputStream = getStream(page)
						If kit IsNot Nothing Then
							Dim doc As Document = initializeModel(kit, page)
    
							' At this point, one could either load up the model with no
							' view notifications slowing it down (i.e. best synchronous
							' behavior) or set the model and start to feed it on a separate
							' thread (best asynchronous behavior).
							p = getAsynchronousLoadPriority(doc)
							If p >= 0 Then
								' load asynchronously
								document = doc
								SyncLock Me
									pageLoader = New PageLoader(Me, doc, [in], loaded, page)
									pageLoader.execute()
								End SyncLock
								Return
							End If
							read([in], doc)
							document = doc
							reloaded = True
						End If
					Else
						' we may need to cancel background loading
						If pageLoader IsNot Nothing Then pageLoader.cancel(True)
    
						' Do everything in a background thread.
						' Model initialization is deferred to that thread, too.
						pageLoader = New PageLoader(Me, Nothing, Nothing, loaded, page)
						pageLoader.execute()
						Return
					End If
				End If
				Dim reference As String = page.ref
				If reference IsNot Nothing Then
					If Not reloaded Then
						scrollToReference(reference)
					Else
						' Have to scroll after painted.
	'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
	'					SwingUtilities.invokeLater(New Runnable()
		'				{
		'					public void run()
		'					{
		'						scrollToReference(reference);
		'					}
		'				});
					End If
					document.putProperty(Document.StreamDescriptionProperty, page)
				End If
				firePropertyChange("page", loaded, page)
			End Set
		End Property

		''' <summary>
		''' Create model and initialize document properties from page properties.
		''' </summary>
		Private Function initializeModel(ByVal kit As EditorKit, ByVal page As URL) As Document
			Dim doc As Document = kit.createDefaultDocument()
			If pageProperties IsNot Nothing Then
				' transfer properties discovered in stream to the
				' document property collection.
				Dim e As System.Collections.IEnumerator(Of String) = pageProperties.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim key As String = e.nextElement()
					doc.putProperty(key, pageProperties(key))
				Loop
				pageProperties.Clear()
			End If
			If doc.getProperty(Document.StreamDescriptionProperty) Is Nothing Then doc.putProperty(Document.StreamDescriptionProperty, page)
			Return doc
		End Function

		''' <summary>
		''' Return load priority for the document or -1 if priority not supported.
		''' </summary>
		Private Function getAsynchronousLoadPriority(ByVal doc As Document) As Integer
			Return (If(TypeOf doc Is AbstractDocument, CType(doc, AbstractDocument).asynchronousLoadPriority, -1))
		End Function

		''' <summary>
		''' This method initializes from a stream.  If the kit is
		''' set to be of type <code>HTMLEditorKit</code>, and the
		''' <code>desc</code> parameter is an <code>HTMLDocument</code>,
		''' then it invokes the <code>HTMLEditorKit</code> to initiate
		''' the read. Otherwise it calls the superclass
		''' method which loads the model as plain text.
		''' </summary>
		''' <param name="in"> the stream from which to read </param>
		''' <param name="desc"> an object describing the stream </param>
		''' <exception cref="IOException"> as thrown by the stream being
		'''          used to initialize </exception>
		''' <seealso cref= JTextComponent#read </seealso>
		''' <seealso cref= #setDocument </seealso>
		Public Overridable Sub read(ByVal [in] As InputStream, ByVal desc As Object)

			If TypeOf desc Is HTMLDocument AndAlso TypeOf kit Is HTMLEditorKit Then
				Dim hdoc As HTMLDocument = CType(desc, HTMLDocument)
				document = hdoc
				read([in], hdoc)
			Else
				Dim charset As String = CStr(getClientProperty("charset"))
				Dim r As Reader = If(charset IsNot Nothing, New InputStreamReader([in], charset), New InputStreamReader([in]))
				MyBase.read(r, desc)
			End If
		End Sub


		''' <summary>
		''' This method invokes the <code>EditorKit</code> to initiate a
		''' read.  In the case where a <code>ChangedCharSetException</code>
		''' is thrown this exception will contain the new CharSet.
		''' Therefore the <code>read</code> operation
		''' is then restarted after building a new Reader with the new charset.
		''' </summary>
		''' <param name="in"> the inputstream to use </param>
		''' <param name="doc"> the document to load
		'''  </param>
		Friend Overridable Sub read(ByVal [in] As InputStream, ByVal doc As Document)
			If Not Boolean.TRUE.Equals(doc.getProperty("IgnoreCharsetDirective")) Then
				Const READ_LIMIT As Integer = 1024 * 10
				[in] = New BufferedInputStream([in], READ_LIMIT)
				[in].mark(READ_LIMIT)
			End If
			Try
				Dim charset As String = CStr(getClientProperty("charset"))
				Dim r As Reader = If(charset IsNot Nothing, New InputStreamReader([in], charset), New InputStreamReader([in]))
				kit.read(r, doc, 0)
			Catch e As BadLocationException
				Throw New IOException(e.Message)
			Catch changedCharSetException As ChangedCharSetException
				Dim charSetSpec As String = changedCharSetException.charSetSpec
				If changedCharSetException.keyEqualsCharSet() Then
					putClientProperty("charset", charSetSpec)
				Else
					charsetFromContentTypeParameters = charSetSpec
				End If
				Try
					[in].reset()
				Catch exception As IOException
					'mark was invalidated
					[in].close()
					Dim url As URL = CType(doc.getProperty(Document.StreamDescriptionProperty), URL)
					If url IsNot Nothing Then
						Dim conn As URLConnection = url.openConnection()
						[in] = conn.inputStream
					Else
						'there is nothing we can do to recover stream
						Throw changedCharSetException
					End If
				End Try
				Try
					doc.remove(0, doc.length)
				Catch e As BadLocationException
				End Try
				doc.putProperty("IgnoreCharsetDirective", Convert.ToBoolean(True))
				read([in], doc)
			End Try
		End Sub


		''' <summary>
		''' Loads a stream into the text document model.
		''' </summary>
		Friend Class PageLoader
			Inherits SwingWorker(Of URL, Object)

			Private ReadOnly outerInstance As JEditorPane


			''' <summary>
			''' Construct an asynchronous page loader.
			''' </summary>
			Friend Sub New(ByVal outerInstance As JEditorPane, ByVal doc As Document, ByVal [in] As InputStream, ByVal old As URL, ByVal page As URL)
					Me.outerInstance = outerInstance
				Me.in = [in]
				Me.old = old
				Me.page = page
				Me.doc = doc
			End Sub

			''' <summary>
			''' Try to load the document, then scroll the view
			''' to the reference (if specified).  When done, fire
			''' a page property change event.
			''' </summary>
			Protected Friend Overridable Function doInBackground() As URL
				Dim pageLoaded As Boolean = False
				Try
					If [in] Is Nothing Then
						[in] = outerInstance.getStream(page)
						If outerInstance.kit Is Nothing Then
							' We received document of unknown content type.
							UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
							Return old
						End If
					End If

					If doc Is Nothing Then
						Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'							SwingUtilities.invokeAndWait(New Runnable()
	'						{
	'							public void run()
	'							{
	'								doc = initializeModel(kit, page);
	'								setDocument(doc);
	'							}
	'						});
						Catch ex As InvocationTargetException
							UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
							Return old
						Catch ex As InterruptedException
							UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
							Return old
						End Try
					End If

					outerInstance.read([in], doc)
					Dim page As URL = CType(doc.getProperty(Document.StreamDescriptionProperty), URL)
					Dim reference As String = page.ref
					If reference IsNot Nothing Then SwingUtilities.invokeLater(callScrollToReference)
					pageLoaded = True
				Catch ioe As IOException
					UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
				Finally
					If pageLoaded Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						SwingUtilities.invokeLater(New Runnable()
	'					{
	'						public void run()
	'						{
	'							outerInstance.firePropertyChange("page", old, page);
	'						}
	'					});
					End If
					Return (If(pageLoaded, page, old))
				End Try
			End Function

			''' <summary>
			''' The stream to load the document with
			''' </summary>
			Friend [in] As InputStream

			''' <summary>
			''' URL of the old page that was replaced (for the property change event)
			''' </summary>
			Friend old As URL

			''' <summary>
			''' URL of the page being loaded (for the property change event)
			''' </summary>
			Friend page As URL

			''' <summary>
			''' The Document instance to load into. This is cached in case a
			''' new Document is created between the time the thread this is created
			''' and run.
			''' </summary>
			Friend doc As Document
		End Class

		''' <summary>
		''' Fetches a stream for the given URL, which is about to
		''' be loaded by the <code>setPage</code> method.  By
		''' default, this simply opens the URL and returns the
		''' stream.  This can be reimplemented to do useful things
		''' like fetch the stream from a cache, monitor the progress
		''' of the stream, etc.
		''' <p>
		''' This method is expected to have the the side effect of
		''' establishing the content type, and therefore setting the
		''' appropriate <code>EditorKit</code> to use for loading the stream.
		''' <p>
		''' If this the stream was an http connection, redirects
		''' will be followed and the resulting URL will be set as
		''' the <code>Document.StreamDescriptionProperty</code> so that relative
		''' URL's can be properly resolved.
		''' </summary>
		''' <param name="page">  the URL of the page </param>
		Protected Friend Overridable Function getStream(ByVal page As URL) As InputStream
			Dim conn As URLConnection = page.openConnection()
			If TypeOf conn Is HttpURLConnection Then
				Dim hconn As HttpURLConnection = CType(conn, HttpURLConnection)
				hconn.instanceFollowRedirects = False
				Dim ___postData As Object = postData
				If ___postData IsNot Nothing Then handlePostData(hconn, ___postData)
				Dim response As Integer = hconn.responseCode
				Dim redirect As Boolean = (response >= 300 AndAlso response <= 399)

	'            
	'             * In the case of a redirect, we want to actually change the URL
	'             * that was input to the new, redirected URL
	'             
				If redirect Then
					Dim loc As String = conn.getHeaderField("Location")
					If loc.StartsWith("http", 0) Then
						page = New URL(loc)
					Else
						page = New URL(page, loc)
					End If
					Return getStream(page)
				End If
			End If

			' Connection properties handler should be forced to run on EDT,
			' as it instantiates the EditorKit.
			If SwingUtilities.eventDispatchThread Then
				handleConnectionProperties(conn)
			Else
				Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					SwingUtilities.invokeAndWait(New Runnable()
	'				{
	'					public void run()
	'					{
	'						handleConnectionProperties(conn);
	'					}
	'				});
				Catch e As InterruptedException
					Throw New Exception(e)
				Catch e As InvocationTargetException
					Throw New Exception(e)
				End Try
			End If
			Return conn.inputStream
		End Function

		''' <summary>
		''' Handle URL connection properties (most notably, content type).
		''' </summary>
		Private Sub handleConnectionProperties(ByVal conn As URLConnection)
			If pageProperties Is Nothing Then pageProperties = New Dictionary(Of String, Object)
			Dim type As String = conn.contentType
			If type IsNot Nothing Then
				contentType = type
				pageProperties("content-type") = type
			End If
			pageProperties(Document.StreamDescriptionProperty) = conn.uRL
			Dim enc As String = conn.contentEncoding
			If enc IsNot Nothing Then pageProperties("content-encoding") = enc
		End Sub

		Private Property postData As Object
			Get
				Return document.getProperty(PostDataProperty)
			End Get
		End Property

		Private Sub handlePostData(ByVal conn As HttpURLConnection, ByVal postData As Object)
			conn.doOutput = True
			Dim os As DataOutputStream = Nothing
			Try
				conn.requestPropertyrty("Content-Type", "application/x-www-form-urlencoded")
				os = New DataOutputStream(conn.outputStream)
				os.writeBytes(CStr(postData))
			Finally
				If os IsNot Nothing Then os.close()
			End Try
		End Sub


		''' <summary>
		''' Scrolls the view to the given reference location
		''' (that is, the value returned by the <code>UL.getRef</code>
		''' method for the URL being displayed).  By default, this
		''' method only knows how to locate a reference in an
		''' HTMLDocument.  The implementation calls the
		''' <code>scrollRectToVisible</code> method to
		''' accomplish the actual scrolling.  If scrolling to a
		''' reference location is needed for document types other
		''' than HTML, this method should be reimplemented.
		''' This method will have no effect if the component
		''' is not visible.
		''' </summary>
		''' <param name="reference"> the named location to scroll to </param>
		Public Overridable Sub scrollToReference(ByVal reference As String)
			Dim d As Document = document
			If TypeOf d Is HTMLDocument Then
				Dim doc As HTMLDocument = CType(d, HTMLDocument)
				Dim iter As HTMLDocument.Iterator = doc.getIterator(HTML.Tag.A)
				Do While iter.valid
					Dim a As AttributeSet = iter.attributes
					Dim nm As String = CStr(a.getAttribute(HTML.Attribute.NAME))
					If (nm IsNot Nothing) AndAlso nm.Equals(reference) Then
						' found a matching reference in the document.
						Try
							Dim pos As Integer = iter.startOffset
							Dim r As Rectangle = modelToView(pos)
							If r IsNot Nothing Then
								' the view is visible, scroll it to the
								' center of the current visible area.
								Dim vis As Rectangle = visibleRect
								'r.y -= (vis.height / 2);
								r.height = vis.height
								scrollRectToVisible(r)
								caretPosition = pos
							End If
						Catch ble As BadLocationException
							UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
						End Try
					End If
					iter.next()
				Loop
			End If
		End Sub

		''' <summary>
		''' Gets the current URL being displayed.  If a URL was
		''' not specified in the creation of the document, this
		''' will return <code>null</code>, and relative URL's will not be
		''' resolved.
		''' </summary>
		''' <returns> the URL, or <code>null</code> if none </returns>
		Public Overridable Property page As URL
			Get
				Return CType(document.getProperty(Document.StreamDescriptionProperty), URL)
			End Get
			Set(ByVal url As String)
				If url Is Nothing Then Throw New IOException("invalid url")
				Dim ___page As New URL(url)
				page = ___page
			End Set
		End Property


		''' <summary>
		''' Gets the class ID for the UI.
		''' </summary>
		''' <returns> the string "EditorPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Creates the default editor kit (<code>PlainEditorKit</code>) for when
		''' the component is first created.
		''' </summary>
		''' <returns> the editor kit </returns>
		Protected Friend Overridable Function createDefaultEditorKit() As EditorKit
			Return New PlainEditorKit
		End Function

		''' <summary>
		''' Fetches the currently installed kit for handling content.
		''' <code>createDefaultEditorKit</code> is called to set up a default
		''' if necessary.
		''' </summary>
		''' <returns> the editor kit </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getEditorKit() As EditorKit 'JavaToDotNetTempPropertyGeteditorKit
		Public Overridable Property editorKit As EditorKit
			Get
				If kit Is Nothing Then
					kit = createDefaultEditorKit()
					isUserSetEditorKit = False
				End If
				Return kit
			End Get
			Set(ByVal kit As EditorKit)
		End Property

		''' <summary>
		''' Gets the type of content that this editor
		''' is currently set to deal with.  This is
		''' defined to be the type associated with the
		''' currently installed <code>EditorKit</code>.
		''' </summary>
		''' <returns> the content type, <code>null</code> if no editor kit set </returns>
		Public Property contentType As String
			Get
				Return If(kit IsNot Nothing, kit.contentType, Nothing)
			End Get
			Set(ByVal type As String)
				' The type could have optional info is part of it,
				' for example some charset info.  We need to strip that
				' of and save it.
				Dim parm As Integer = type.IndexOf(";")
				If parm > -1 Then
					' Save the paramList.
					Dim paramList As String = type.Substring(parm)
					' update the content type string.
					type = type.Substring(0, parm).Trim()
					If type.ToLower().StartsWith("text/") Then charsetFromContentTypeParameters = paramList
				End If
				If (kit Is Nothing) OrElse ((Not type.Equals(kit.contentType))) OrElse (Not isUserSetEditorKit) Then
					Dim k As EditorKit = getEditorKitForContentType(type)
					If k IsNot Nothing AndAlso k IsNot kit Then
						editorKit = k
						isUserSetEditorKit = False
					End If
				End If
    
			End Set
		End Property


		''' <summary>
		''' This method gets the charset information specified as part
		''' of the content type in the http header information.
		''' </summary>
		Private Property charsetFromContentTypeParameters As String
			Set(ByVal paramlist As String)
				Dim charset As String
				Try
					' paramlist is handed to us with a leading ';', strip it.
					Dim semi As Integer = paramlist.IndexOf(";"c)
					If semi > -1 AndAlso semi < paramlist.Length-1 Then paramlist = paramlist.Substring(semi + 1)
    
					If paramlist.Length > 0 Then
						' parse the paramlist into attr-value pairs & get the
						' charset pair's value
						Dim hdrParser As New HeaderParser(paramlist)
						charset = hdrParser.findValue("charset")
						If charset IsNot Nothing Then putClientProperty("charset", charset)
					End If
				Catch e As System.IndexOutOfRangeException
					' malformed parameter list, use charset we have
				Catch e As NullPointerException
					' malformed parameter list, use charset we have
				Catch e As Exception
					' malformed parameter list, use charset we have; but complain
					Console.Error.WriteLine("JEditorPane.getCharsetFromContentTypeParameters failed on: " & paramlist)
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End Set
		End Property


			Dim old As EditorKit = Me.kit
			isUserSetEditorKit = True
			If old IsNot Nothing Then old.deinstall(Me)
			Me.kit = kit
			If Me.kit IsNot Nothing Then
				Me.kit.install(Me)
				document = Me.kit.createDefaultDocument()
			End If
			firePropertyChange("editorKit", old, kit)
		End Sub

		''' <summary>
		''' Fetches the editor kit to use for the given type
		''' of content.  This is called when a type is requested
		''' that doesn't match the currently installed type.
		''' If the component doesn't have an <code>EditorKit</code> registered
		''' for the given type, it will try to create an
		''' <code>EditorKit</code> from the default <code>EditorKit</code> registry.
		''' If that fails, a <code>PlainEditorKit</code> is used on the
		''' assumption that all text documents can be represented
		''' as plain text.
		''' <p>
		''' This method can be reimplemented to use some
		''' other kind of type registry.  This can
		''' be reimplemented to use the Java Activation
		''' Framework, for example.
		''' </summary>
		''' <param name="type"> the non-<code>null</code> content type </param>
		''' <returns> the editor kit </returns>
		Public Overridable Function getEditorKitForContentType(ByVal type As String) As EditorKit
			If typeHandlers Is Nothing Then typeHandlers = New Dictionary(Of String, EditorKit)(3)
			Dim k As EditorKit = typeHandlers(type)
			If k Is Nothing Then
				k = createEditorKitForContentType(type)
				If k IsNot Nothing Then editorKitForContentTypeype(type, k)
			End If
			If k Is Nothing Then k = createDefaultEditorKit()
			Return k
		End Function

		''' <summary>
		''' Directly sets the editor kit to use for the given type.  A
		''' look-and-feel implementation might use this in conjunction
		''' with <code>createEditorKitForContentType</code> to install handlers for
		''' content types with a look-and-feel bias.
		''' </summary>
		''' <param name="type"> the non-<code>null</code> content type </param>
		''' <param name="k"> the editor kit to be set </param>
		Public Overridable Sub setEditorKitForContentType(ByVal type As String, ByVal k As EditorKit)
			If typeHandlers Is Nothing Then typeHandlers = New Dictionary(Of String, EditorKit)(3)
			typeHandlers(type) = k
		End Sub

		''' <summary>
		''' Replaces the currently selected content with new content
		''' represented by the given string.  If there is no selection
		''' this amounts to an insert of the given text.  If there
		''' is no replacement text (i.e. the content string is empty
		''' or <code>null</code>) this amounts to a removal of the
		''' current selection.  The replacement text will have the
		''' attributes currently defined for input.  If the component is not
		''' editable, beep and return.
		''' </summary>
		''' <param name="content">  the content to replace the selection with.  This
		'''   value can be <code>null</code> </param>
		Public Overrides Sub replaceSelection(ByVal content As String)
			If Not editable Then
				UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
				Return
			End If
			Dim kit As EditorKit = editorKit
			If TypeOf kit Is StyledEditorKit Then
				Try
					Dim doc As Document = document
					Dim ___caret As Caret = caret
					Dim composedTextSaved As Boolean = saveComposedText(___caret.dot)
					Dim p0 As Integer = Math.Min(___caret.dot, ___caret.mark)
					Dim p1 As Integer = Math.Max(___caret.dot, ___caret.mark)
					If TypeOf doc Is AbstractDocument Then
						CType(doc, AbstractDocument).replace(p0, p1 - p0, content, CType(kit, StyledEditorKit).inputAttributes)
					Else
						If p0 <> p1 Then doc.remove(p0, p1 - p0)
						If content IsNot Nothing AndAlso content.Length > 0 Then doc.insertString(p0, content, CType(kit, StyledEditorKit).inputAttributes)
					End If
					If composedTextSaved Then restoreComposedText()
				Catch e As BadLocationException
					UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
				End Try
			Else
				MyBase.replaceSelection(content)
			End If
		End Sub

		''' <summary>
		''' Creates a handler for the given type from the default registry
		''' of editor kits.  The registry is created if necessary.  If the
		''' registered class has not yet been loaded, an attempt
		''' is made to dynamically load the prototype of the kit for the
		''' given type.  If the type was registered with a <code>ClassLoader</code>,
		''' that <code>ClassLoader</code> will be used to load the prototype.
		''' If there was no registered <code>ClassLoader</code>,
		''' <code>Class.forName</code> will be used to load the prototype.
		''' <p>
		''' Once a prototype <code>EditorKit</code> instance is successfully
		''' located, it is cloned and the clone is returned.
		''' </summary>
		''' <param name="type"> the content type </param>
		''' <returns> the editor kit, or <code>null</code> if there is nothing
		'''   registered for the given type </returns>
		Public Shared Function createEditorKitForContentType(ByVal type As String) As EditorKit
			Dim kitRegistry As Dictionary(Of String, EditorKit) = kitRegisty
			Dim k As EditorKit = kitRegistry(type)
			If k Is Nothing Then
				' try to dynamically load the support
				Dim classname As String = kitTypeRegistry(type)
				Dim loader As ClassLoader = kitLoaderRegistry(type)
				Try
					Dim c As Type
					If loader IsNot Nothing Then
						c = loader.loadClass(classname)
					Else
						' Will only happen if developer has invoked
						' registerEditorKitForContentType(type, class, null).
						c = Type.GetType(classname, True, Thread.CurrentThread.contextClassLoader)
					End If
					k = CType(c.newInstance(), EditorKit)
					kitRegistry(type) = k
				Catch e As Exception
					k = Nothing
				End Try
			End If

			' create a copy of the prototype or null if there
			' is no prototype.
			If k IsNot Nothing Then Return CType(k.clone(), EditorKit)
			Return Nothing
		End Function

		''' <summary>
		''' Establishes the default bindings of <code>type</code> to
		''' <code>classname</code>.
		''' The class will be dynamically loaded later when actually
		''' needed, and can be safely changed before attempted uses
		''' to avoid loading unwanted classes.  The prototype
		''' <code>EditorKit</code> will be loaded with <code>Class.forName</code>
		''' when registered with this method.
		''' </summary>
		''' <param name="type"> the non-<code>null</code> content type </param>
		''' <param name="classname"> the class to load later </param>
		Public Shared Sub registerEditorKitForContentType(ByVal type As String, ByVal classname As String)
			registerEditorKitForContentType(type, classname,Thread.CurrentThread.contextClassLoader)
		End Sub

		''' <summary>
		''' Establishes the default bindings of <code>type</code> to
		''' <code>classname</code>.
		''' The class will be dynamically loaded later when actually
		''' needed using the given <code>ClassLoader</code>,
		''' and can be safely changed
		''' before attempted uses to avoid loading unwanted classes.
		''' </summary>
		''' <param name="type"> the non-<code>null</code> content type </param>
		''' <param name="classname"> the class to load later </param>
		''' <param name="loader"> the <code>ClassLoader</code> to use to load the name </param>
		Public Shared Sub registerEditorKitForContentType(ByVal type As String, ByVal classname As String, ByVal loader As ClassLoader)
			kitTypeRegistry(type) = classname
			kitLoaderRegistry(type) = loader
			kitRegisty.Remove(type)
		End Sub

		''' <summary>
		''' Returns the currently registered <code>EditorKit</code>
		''' class name for the type <code>type</code>.
		''' </summary>
		''' <param name="type">  the non-<code>null</code> content type
		''' 
		''' @since 1.3 </param>
		Public Shared Function getEditorKitClassNameForContentType(ByVal type As String) As String
			Return kitTypeRegistry(type)
		End Function

		Private Property Shared kitTypeRegistry As Dictionary(Of String, String)
			Get
				loadDefaultKitsIfNecessary()
				Return CType(SwingUtilities.appContextGet(kitTypeRegistryKey), Hashtable)
			End Get
		End Property

		Private Property Shared kitLoaderRegistry As Dictionary(Of String, ClassLoader)
			Get
				loadDefaultKitsIfNecessary()
				Return CType(SwingUtilities.appContextGet(kitLoaderRegistryKey), Hashtable)
			End Get
		End Property

		Private Property Shared kitRegisty As Dictionary(Of String, EditorKit)
			Get
				Dim ht As Hashtable = CType(SwingUtilities.appContextGet(kitRegistryKey), Hashtable)
				If ht Is Nothing Then
					ht = New Hashtable(3)
					SwingUtilities.appContextPut(kitRegistryKey, ht)
				End If
				Return ht
			End Get
		End Property

		''' <summary>
		''' This is invoked every time the registries are accessed. Loading
		''' is done this way instead of via a static as the static is only
		''' called once when running in plugin resulting in the entries only
		''' appearing in the first applet.
		''' </summary>
		Private Shared Sub loadDefaultKitsIfNecessary()
			If SwingUtilities.appContextGet(kitTypeRegistryKey) Is Nothing Then
				SyncLock defaultEditorKitMap
					If defaultEditorKitMap.size() = 0 Then
						defaultEditorKitMap.put("text/plain", "javax.swing.JEditorPane$PlainEditorKit")
						defaultEditorKitMap.put("text/html", "javax.swing.text.html.HTMLEditorKit")
						defaultEditorKitMap.put("text/rtf", "javax.swing.text.rtf.RTFEditorKit")
						defaultEditorKitMap.put("application/rtf", "javax.swing.text.rtf.RTFEditorKit")
					End If
				End SyncLock
				Dim ht As New Hashtable
				SwingUtilities.appContextPut(kitTypeRegistryKey, ht)
				ht = New Hashtable
				SwingUtilities.appContextPut(kitLoaderRegistryKey, ht)
				For Each key As String In defaultEditorKitMap.Keys
					registerEditorKitForContentType(key,defaultEditorKitMap.get(key))
				Next key

			End If
		End Sub

		' --- java.awt.Component methods --------------------------

		''' <summary>
		''' Returns the preferred size for the <code>JEditorPane</code>.
		''' The preferred size for <code>JEditorPane</code> is slightly altered
		''' from the preferred size of the superclass.  If the size
		''' of the viewport has become smaller than the minimum size
		''' of the component, the scrollable definition for tracking
		''' width or height will turn to false.  The default viewport
		''' layout will give the preferred size, and that is not desired
		''' in the case where the scrollable is tracking.  In that case
		''' the <em>normal</em> preferred size is adjusted to the
		''' minimum size.  This allows things like HTML tables to
		''' shrink down to their minimum size and then be laid out at
		''' their minimum size, refusing to shrink any further.
		''' </summary>
		''' <returns> a <code>Dimension</code> containing the preferred size </returns>
		Public Property Overrides preferredSize As Dimension
			Get
				Dim d As Dimension = MyBase.preferredSize
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then
					Dim port As JViewport = CType(parent, JViewport)
					Dim ___ui As TextUI = uI
					Dim prefWidth As Integer = d.width
					Dim prefHeight As Integer = d.height
					If Not scrollableTracksViewportWidth Then
						Dim w As Integer = port.width
						Dim min As Dimension = ___ui.getMinimumSize(Me)
						If w <> 0 AndAlso w < min.width Then prefWidth = min.width
					End If
					If Not scrollableTracksViewportHeight Then
						Dim h As Integer = port.height
						Dim min As Dimension = ___ui.getMinimumSize(Me)
						If h <> 0 AndAlso h < min.height Then prefHeight = min.height
					End If
					If prefWidth <> d.width OrElse prefHeight <> d.height Then d = New Dimension(prefWidth, prefHeight)
				End If
				Return d
			End Get
		End Property

		' --- JTextComponent methods -----------------------------

		''' <summary>
		''' Sets the text of this <code>TextComponent</code> to the specified
		''' content,
		''' which is expected to be in the format of the content type of
		''' this editor.  For example, if the type is set to <code>text/html</code>
		''' the string should be specified in terms of HTML.
		''' <p>
		''' This is implemented to remove the contents of the current document,
		''' and replace them by parsing the given string using the current
		''' <code>EditorKit</code>.  This gives the semantics of the
		''' superclass by not changing
		''' out the model, while supporting the content type currently set on
		''' this component.  The assumption is that the previous content is
		''' relatively
		''' small, and that the previous content doesn't have side effects.
		''' Both of those assumptions can be violated and cause undesirable results.
		''' To avoid this, create a new document,
		''' <code>getEditorKit().createDefaultDocument()</code>, and replace the
		''' existing <code>Document</code> with the new one. You are then assured the
		''' previous <code>Document</code> won't have any lingering state.
		''' <ol>
		''' <li>
		''' Leaving the existing model in place means that the old view will be
		''' torn down, and a new view created, where replacing the document would
		''' avoid the tear down of the old view.
		''' <li>
		''' Some formats (such as HTML) can install things into the document that
		''' can influence future contents.  HTML can have style information embedded
		''' that would influence the next content installed unexpectedly.
		''' </ol>
		''' <p>
		''' An alternative way to load this component with a string would be to
		''' create a StringReader and call the read method.  In this case the model
		''' would be replaced after it was initialized with the contents of the
		''' string.
		''' </summary>
		''' <param name="t"> the new text to be set; if <code>null</code> the old
		'''    text will be deleted </param>
		''' <seealso cref= #getText
		''' @beaninfo
		''' description: the text of this component </seealso>
		Public Overrides Property text As String
			Set(ByVal t As String)
				Try
					Dim doc As Document = document
					doc.remove(0, doc.length)
					If t Is Nothing OrElse t.Equals("") Then Return
					Dim r As Reader = New StringReader(t)
					Dim kit As EditorKit = editorKit
					kit.read(r, doc, 0)
				Catch ioe As IOException
					UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
				Catch ble As BadLocationException
					UIManager.lookAndFeel.provideErrorFeedback(JEditorPane.this)
				End Try
			End Set
			Get
				Dim txt As String
				Try
					Dim buf As New StringWriter
					write(buf)
					txt = buf.ToString()
				Catch ioe As IOException
					txt = Nothing
				End Try
				Return txt
			End Get
		End Property


		' --- Scrollable  ----------------------------------------

		''' <summary>
		''' Returns true if a viewport should always force the width of this
		''' <code>Scrollable</code> to match the width of the viewport.
		''' </summary>
		''' <returns> true if a viewport should force the Scrollables width to
		''' match its own, false otherwise </returns>
		Public Property Overrides scrollableTracksViewportWidth As Boolean
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then
					Dim port As JViewport = CType(parent, JViewport)
					Dim ___ui As TextUI = uI
					Dim w As Integer = port.width
					Dim min As Dimension = ___ui.getMinimumSize(Me)
					Dim max As Dimension = ___ui.getMaximumSize(Me)
					If (w >= min.width) AndAlso (w <= max.width) Then Return True
				End If
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns true if a viewport should always force the height of this
		''' <code>Scrollable</code> to match the height of the viewport.
		''' </summary>
		''' <returns> true if a viewport should force the
		'''          <code>Scrollable</code>'s height to match its own,
		'''          false otherwise </returns>
		Public Property Overrides scrollableTracksViewportHeight As Boolean
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then
					Dim port As JViewport = CType(parent, JViewport)
					Dim ___ui As TextUI = uI
					Dim h As Integer = port.height
					Dim min As Dimension = ___ui.getMinimumSize(Me)
					If h >= min.height Then
						Dim max As Dimension = ___ui.getMaximumSize(Me)
						If h <= max.height Then Return True
					End If
				End If
				Return False
			End Get
		End Property

		' --- Serialization ------------------------------------

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		' --- variables ---------------------------------------

		Private pageLoader As SwingWorker(Of URL, Object)

		''' <summary>
		''' Current content binding of the editor.
		''' </summary>
		Private kit As EditorKit
		Private isUserSetEditorKit As Boolean

		Private pageProperties As Dictionary(Of String, Object)

		''' <summary>
		''' Should be kept in sync with javax.swing.text.html.FormView counterpart. </summary>
		Friend Const PostDataProperty As String = "javax.swing.JEditorPane.postdata"

		''' <summary>
		''' Table of registered type handlers for this editor.
		''' </summary>
		Private typeHandlers As Dictionary(Of String, EditorKit)

	'    
	'     * Private AppContext keys for this class's static variables.
	'     
		Private Shared ReadOnly kitRegistryKey As Object = New StringBuilder("JEditorPane.kitRegistry")
		Private Shared ReadOnly kitTypeRegistryKey As Object = New StringBuilder("JEditorPane.kitTypeRegistry")
		Private Shared ReadOnly kitLoaderRegistryKey As Object = New StringBuilder("JEditorPane.kitLoaderRegistry")

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "EditorPaneUI"


		''' <summary>
		''' Key for a client property used to indicate whether
		''' <a href="http://www.w3.org/TR/CSS21/syndata.html#length-units">
		''' w3c compliant</a> length units are used for html rendering.
		''' <p>
		''' By default this is not enabled; to enable
		''' it set the client <seealso cref="#putClientProperty property"/> with this name
		''' to <code>Boolean.TRUE</code>.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const W3C_LENGTH_UNITS As String = "JEditorPane.w3cLengthUnits"

		''' <summary>
		''' Key for a client property used to indicate whether
		''' the default font and foreground color from the component are
		''' used if a font or foreground color is not specified in the styled
		''' text.
		''' <p>
		''' The default varies based on the look and feel;
		''' to enable it set the client <seealso cref="#putClientProperty property"/> with
		''' this name to <code>Boolean.TRUE</code>.
		''' 
		''' @since 1.5
		''' </summary>
		Public Const HONOR_DISPLAY_PROPERTIES As String = "JEditorPane.honorDisplayProperties"

		Friend Shared ReadOnly defaultEditorKitMap As Map(Of String, String) = New Dictionary(Of String, String)(0)

		''' <summary>
		''' Returns a string representation of this <code>JEditorPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JEditorPane</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim kitString As String = (If(kit IsNot Nothing, kit.ToString(), ""))
			Dim typeHandlersString As String = (If(typeHandlers IsNot Nothing, typeHandlers.ToString(), ""))

			Return MyBase.paramString() & ",kit=" & kitString & ",typeHandlers=" & typeHandlersString
		End Function


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this JEditorPane.
		''' For editor panes, the AccessibleContext takes the form of an
		''' AccessibleJEditorPane.
		''' A new AccessibleJEditorPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJEditorPane that serves as the
		'''         AccessibleContext of this JEditorPane </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If TypeOf editorKit Is HTMLEditorKit Then
					If accessibleContext Is Nothing OrElse accessibleContext.GetType() IsNot GetType(AccessibleJEditorPaneHTML) Then accessibleContext = New AccessibleJEditorPaneHTML(Me)
				ElseIf accessibleContext Is Nothing OrElse accessibleContext.GetType() IsNot GetType(AccessibleJEditorPane) Then
					accessibleContext = New AccessibleJEditorPane(Me)
				End If
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JEditorPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to editor pane user-interface
		''' elements.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		Protected Friend Class AccessibleJEditorPane
			Inherits AccessibleJTextComponent

			Private ReadOnly outerInstance As JEditorPane

			Public Sub New(ByVal outerInstance As JEditorPane)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the accessibleDescription property of this object.  If this
			''' property isn't set, returns the content type of this
			''' <code>JEditorPane</code> instead (e.g. "plain/text", "html/text").
			''' </summary>
			''' <returns> the localized description of the object; <code>null</code>
			'''      if this object does not have a description
			''' </returns>
			''' <seealso cref= #setAccessibleName </seealso>
			Public Overridable Property accessibleDescription As String
				Get
					Dim description As String = accessibleDescription
    
					' fallback to client property
					If description Is Nothing Then description = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY))
					If description Is Nothing Then description = outerInstance.contentType
					Return description
				End Get
			End Property

			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet describing the states
			''' of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					states.add(AccessibleState.MULTI_LINE)
					Return states
				End Get
			End Property
		End Class

		''' <summary>
		''' This class provides support for <code>AccessibleHypertext</code>,
		''' and is used in instances where the <code>EditorKit</code>
		''' installed in this <code>JEditorPane</code> is an instance of
		''' <code>HTMLEditorKit</code>.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		Protected Friend Class AccessibleJEditorPaneHTML
			Inherits AccessibleJEditorPane

			Private ReadOnly outerInstance As JEditorPane


			Private ___accessibleContext As AccessibleContext

			Public Overridable Property accessibleText As AccessibleText
				Get
					Return New JEditorPaneAccessibleHypertextSupport
				End Get
			End Property

			Protected Friend Sub New(ByVal outerInstance As JEditorPane)
					Me.outerInstance = outerInstance
				Dim kit As HTMLEditorKit = CType(JEditorPane.this.editorKit, HTMLEditorKit)
				___accessibleContext = kit.accessibleContext
			End Sub

			''' <summary>
			''' Returns the number of accessible children of the object.
			''' </summary>
			''' <returns> the number of accessible children of the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					If ___accessibleContext IsNot Nothing Then
						Return ___accessibleContext.accessibleChildrenCount
					Else
						Return 0
					End If
				End Get
			End Property

			''' <summary>
			''' Returns the specified Accessible child of the object.  The Accessible
			''' children of an Accessible object are zero-based, so the first child
			''' of an Accessible child is at index 0, the second child is at index 1,
			''' and so on.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				If ___accessibleContext IsNot Nothing Then
					Return ___accessibleContext.getAccessibleChild(i)
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the Accessible child, if one exists, contained at the local
			''' coordinate Point.
			''' </summary>
			''' <param name="p"> The point relative to the coordinate system of this object. </param>
			''' <returns> the Accessible, if it exists, at the specified location;
			''' otherwise null </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				If ___accessibleContext IsNot Nothing AndAlso p IsNot Nothing Then
					Try
						Dim acomp As AccessibleComponent = ___accessibleContext.accessibleComponent
						If acomp IsNot Nothing Then
							Return acomp.getAccessibleAt(p)
						Else
							Return Nothing
						End If
					Catch e As IllegalComponentStateException
						Return Nothing
					End Try
				Else
					Return Nothing
				End If
			End Function
		End Class

		''' <summary>
		''' What's returned by
		''' <code>AccessibleJEditorPaneHTML.getAccessibleText</code>.
		''' 
		''' Provides support for <code>AccessibleHypertext</code> in case
		''' there is an HTML document being displayed in this
		''' <code>JEditorPane</code>.
		''' 
		''' </summary>
		Protected Friend Class JEditorPaneAccessibleHypertextSupport
			Inherits AccessibleJEditorPane
			Implements AccessibleHypertext

			Private ReadOnly outerInstance As JEditorPane


			Public Class HTMLLink
				Inherits AccessibleHyperlink

				Private ReadOnly outerInstance As JEditorPane.JEditorPaneAccessibleHypertextSupport

				Friend element As Element

				Public Sub New(ByVal outerInstance As JEditorPane.JEditorPaneAccessibleHypertextSupport, ByVal e As Element)
						Me.outerInstance = outerInstance
					element = e
				End Sub

				''' <summary>
				''' Since the document a link is associated with may have
				''' changed, this method returns whether this Link is valid
				''' anymore (with respect to the document it references).
				''' </summary>
				''' <returns> a flag indicating whether this link is still valid with
				'''         respect to the AccessibleHypertext it belongs to </returns>
				Public Property Overrides valid As Boolean
					Get
						Return outerInstance.linksValid
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible actions available in this Link
				''' If there are more than one, the first one is NOT considered the
				''' "default" action of this LINK object (e.g. in an HTML imagemap).
				''' In general, links will have only one AccessibleAction in them.
				''' </summary>
				''' <returns> the zero-based number of Actions in this object </returns>
				Public Property Overrides accessibleActionCount As Integer
					Get
						Return 1
					End Get
				End Property

				''' <summary>
				''' Perform the specified Action on the object
				''' </summary>
				''' <param name="i"> zero-based index of actions </param>
				''' <returns> true if the the action was performed; else false. </returns>
				''' <seealso cref= #getAccessibleActionCount </seealso>
				Public Overrides Function doAccessibleAction(ByVal i As Integer) As Boolean
					If i = 0 AndAlso valid = True Then
						Dim u As URL = CType(getAccessibleActionObject(i), URL)
						If u IsNot Nothing Then
							Dim linkEvent As New HyperlinkEvent(JEditorPane.this, HyperlinkEvent.EventType.ACTIVATED, u)
							outerInstance.fireHyperlinkUpdate(linkEvent)
							Return True
						End If
					End If
					Return False ' link invalid or i != 0
				End Function

				''' <summary>
				''' Return a String description of this particular
				''' link action.  The string returned is the text
				''' within the document associated with the element
				''' which contains this link.
				''' </summary>
				''' <param name="i"> zero-based index of the actions </param>
				''' <returns> a String description of the action </returns>
				''' <seealso cref= #getAccessibleActionCount </seealso>
				Public Overrides Function getAccessibleActionDescription(ByVal i As Integer) As String
					If i = 0 AndAlso valid = True Then
						Dim d As Document = outerInstance.document
						If d IsNot Nothing Then
							Try
								Return d.getText(startIndex, endIndex - startIndex)
							Catch exception As BadLocationException
								Return Nothing
							End Try
						End If
					End If
					Return Nothing
				End Function

				''' <summary>
				''' Returns a URL object that represents the link.
				''' </summary>
				''' <param name="i"> zero-based index of the actions </param>
				''' <returns> an URL representing the HTML link itself </returns>
				''' <seealso cref= #getAccessibleActionCount </seealso>
				Public Overrides Function getAccessibleActionObject(ByVal i As Integer) As Object
					If i = 0 AndAlso valid = True Then
						Dim [as] As AttributeSet = element.attributes
						Dim anchor As AttributeSet = CType([as].getAttribute(HTML.Tag.A), AttributeSet)
						Dim href As String = If(anchor IsNot Nothing, CStr(anchor.getAttribute(HTML.Attribute.HREF)), Nothing)
						If href IsNot Nothing Then
							Dim u As URL
							Try
								u = New URL(outerInstance.page, href)
							Catch m As MalformedURLException
								u = Nothing
							End Try
							Return u
						End If
					End If
					Return Nothing ' link invalid or i != 0
				End Function

				''' <summary>
				''' Return an object that represents the link anchor,
				''' as appropriate for that link.  E.g. from HTML:
				'''   <a href="http://www.sun.com/access">Accessibility</a>
				''' this method would return a String containing the text:
				''' 'Accessibility'.
				''' 
				''' Similarly, from this HTML:
				'''   &lt;a HREF="#top"&gt;&lt;img src="top-hat.gif" alt="top hat"&gt;&lt;/a&gt;
				''' this might return the object ImageIcon("top-hat.gif", "top hat");
				''' </summary>
				''' <param name="i"> zero-based index of the actions </param>
				''' <returns> an Object representing the hypertext anchor </returns>
				''' <seealso cref= #getAccessibleActionCount </seealso>
				Public Overrides Function getAccessibleActionAnchor(ByVal i As Integer) As Object
					Return getAccessibleActionDescription(i)
				End Function


				''' <summary>
				''' Get the index with the hypertext document at which this
				''' link begins
				''' </summary>
				''' <returns> index of start of link </returns>
				Public Property Overrides startIndex As Integer
					Get
						Return element.startOffset
					End Get
				End Property

				''' <summary>
				''' Get the index with the hypertext document at which this
				''' link ends
				''' </summary>
				''' <returns> index of end of link </returns>
				Public Property Overrides endIndex As Integer
					Get
						Return element.endOffset
					End Get
				End Property
			End Class

			Private Class LinkVector
				Inherits List(Of HTMLLink)

				Private ReadOnly outerInstance As JEditorPane.JEditorPaneAccessibleHypertextSupport

				Public Sub New(ByVal outerInstance As JEditorPane.JEditorPaneAccessibleHypertextSupport)
					Me.outerInstance = outerInstance
				End Sub

				Public Overridable Function baseElementIndex(ByVal e As Element) As Integer
					Dim l As HTMLLink
					For i As Integer = 0 To elementCount - 1
						l = elementAt(i)
						If l.element Is e Then Return i
					Next i
					Return -1
				End Function
			End Class

			Friend hyperlinks As LinkVector
			Friend linksValid As Boolean = False

			''' <summary>
			''' Build the private table mapping links to locations in the text
			''' </summary>
			Private Sub buildLinkTable()
				hyperlinks.Clear()
				Dim d As Document = outerInstance.document
				If d IsNot Nothing Then
					Dim ei As New ElementIterator(d)
					Dim e As Element
					Dim [as] As AttributeSet
					Dim anchor As AttributeSet
					Dim href As String
					e = ei.next()
					Do While e IsNot Nothing
						If e.leaf Then
							[as] = e.attributes
						anchor = CType([as].getAttribute(HTML.Tag.A), AttributeSet)
						href = If(anchor IsNot Nothing, CStr(anchor.getAttribute(HTML.Attribute.HREF)), Nothing)
							If href IsNot Nothing Then hyperlinks.Add(New HTMLLink(Me, e))
						End If
						e = ei.next()
					Loop
				End If
				linksValid = True
			End Sub

			''' <summary>
			''' Make one of these puppies
			''' </summary>
			Public Sub New(ByVal outerInstance As JEditorPane)
					Me.outerInstance = outerInstance
				hyperlinks = New LinkVector(Me)
				Dim d As Document = outerInstance.document
				If d IsNot Nothing Then d.addDocumentListener(New DocumentListenerAnonymousInnerClassHelper
			End Sub

			Private Class DocumentListenerAnonymousInnerClassHelper
				Implements DocumentListener

				Public Overridable Sub changedUpdate(ByVal theEvent As DocumentEvent) Implements DocumentListener.changedUpdate
					outerInstance.linksValid = False
				End Sub
				Public Overridable Sub insertUpdate(ByVal theEvent As DocumentEvent) Implements DocumentListener.insertUpdate
					outerInstance.linksValid = False
				End Sub
				Public Overridable Sub removeUpdate(ByVal theEvent As DocumentEvent) Implements DocumentListener.removeUpdate
					outerInstance.linksValid = False
				End Sub
			End Class

			''' <summary>
			''' Returns the number of links within this hypertext doc.
			''' </summary>
			''' <returns> number of links in this hypertext doc. </returns>
			Public Overridable Property linkCount As Integer Implements AccessibleHypertext.getLinkCount
				Get
					If linksValid = False Then buildLinkTable()
					Return hyperlinks.Count
				End Get
			End Property

			''' <summary>
			''' Returns the index into an array of hyperlinks that
			''' is associated with this character index, or -1 if there
			''' is no hyperlink associated with this index.
			''' </summary>
			''' <param name="charIndex"> index within the text </param>
			''' <returns> index into the set of hyperlinks for this hypertext doc. </returns>
			Public Overridable Function getLinkIndex(ByVal charIndex As Integer) As Integer Implements AccessibleHypertext.getLinkIndex
				If linksValid = False Then buildLinkTable()
				Dim e As Element = Nothing
				Dim doc As Document = outerInstance.document
				If doc IsNot Nothing Then
					e = doc.defaultRootElement
					Do While Not e.leaf
						Dim index As Integer = e.getElementIndex(charIndex)
						e = e.getElement(index)
					Loop
				End If

				' don't need to verify that it's an HREF element; if
				' not, then it won't be in the hyperlinks Vector, and
				' so indexOf will return -1 in any case
				Return hyperlinks.baseElementIndex(e)
			End Function

			''' <summary>
			''' Returns the index into an array of hyperlinks that
			''' index.  If there is no hyperlink at this index, it returns
			''' null.
			''' </summary>
			''' <param name="linkIndex"> into the set of hyperlinks for this hypertext doc. </param>
			''' <returns> string representation of the hyperlink </returns>
			Public Overridable Function getLink(ByVal linkIndex As Integer) As AccessibleHyperlink Implements AccessibleHypertext.getLink
				If linksValid = False Then buildLinkTable()
				If linkIndex >= 0 AndAlso linkIndex < hyperlinks.Count Then
					Return hyperlinks(linkIndex)
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the contiguous text within the document that
			''' is associated with this hyperlink.
			''' </summary>
			''' <param name="linkIndex"> into the set of hyperlinks for this hypertext doc. </param>
			''' <returns> the contiguous text sharing the link at this index </returns>
			Public Overridable Function getLinkText(ByVal linkIndex As Integer) As String
				If linksValid = False Then buildLinkTable()
				Dim e As Element = CType(hyperlinks(linkIndex), Element)
				If e IsNot Nothing Then
					Dim d As Document = outerInstance.document
					If d IsNot Nothing Then
						Try
							Return d.getText(e.startOffset, e.endOffset - e.startOffset)
						Catch exception As BadLocationException
							Return Nothing
						End Try
					End If
				End If
				Return Nothing
			End Function
		End Class

		Friend Class PlainEditorKit
			Inherits DefaultEditorKit
			Implements ViewFactory

			''' <summary>
			''' Fetches a factory that is suitable for producing
			''' views of any models that are produced by this
			''' kit.  The default is to have the UI produce the
			''' factory, so this method has no implementation.
			''' </summary>
			''' <returns> the view factory </returns>
			Public Property Overrides viewFactory As ViewFactory
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Creates a view from the given structural element of a
			''' document.
			''' </summary>
			''' <param name="elem">  the piece of the document to build a view of </param>
			''' <returns> the view </returns>
			''' <seealso cref= View </seealso>
			Public Overridable Function create(ByVal elem As Element) As View Implements ViewFactory.create
				Dim doc As Document = elem.document
				Dim i18nFlag As Object = doc.getProperty("i18n") 'AbstractDocument.I18NProperty
				If (i18nFlag IsNot Nothing) AndAlso i18nFlag.Equals(Boolean.TRUE) Then
					' build a view that support bidi
					Return createI18N(elem)
				Else
					Return New WrappedPlainView(elem)
				End If
			End Function

			Friend Overridable Function createI18N(ByVal elem As Element) As View
				Dim kind As String = elem.name
				If kind IsNot Nothing Then
					If kind.Equals(AbstractDocument.ContentElementName) Then
						Return New PlainParagraph(elem)
					ElseIf kind.Equals(AbstractDocument.ParagraphElementName) Then
						Return New BoxView(elem, View.Y_AXIS)
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Paragraph for representing plain-text lines that support
			''' bidirectional text.
			''' </summary>
			Friend Class PlainParagraph
				Inherits javax.swing.text.ParagraphView

				Friend Sub New(ByVal elem As Element)
					MyBase.New(elem)
					layoutPool = New LogicalView(elem)
					layoutPool.parent = Me
				End Sub

				Protected Friend Overrides Sub setPropertiesFromAttributes()
					Dim c As Component = container
					If (c IsNot Nothing) AndAlso ((Not c.componentOrientation.leftToRight)) Then
						justification = StyleConstants.ALIGN_RIGHT
					Else
						justification = StyleConstants.ALIGN_LEFT
					End If
				End Sub

				''' <summary>
				''' Fetch the constraining span to flow against for
				''' the given child index.
				''' </summary>
				Public Overrides Function getFlowSpan(ByVal index As Integer) As Integer
					Dim c As Component = container
					If TypeOf c Is JTextArea Then
						Dim area As JTextArea = CType(c, JTextArea)
						If Not area.lineWrap Then Return Integer.MaxValue
					End If
					Return MyBase.getFlowSpan(index)
				End Function

				Protected Friend Overridable Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As SizeRequirements) As SizeRequirements
					Dim req As SizeRequirements = MyBase.calculateMinorAxisRequirements(axis, r)
					Dim c As Component = container
					If TypeOf c Is JTextArea Then
						Dim area As JTextArea = CType(c, JTextArea)
						If Not area.lineWrap Then req.minimum = req.preferred
					End If
					Return req
				End Function

				''' <summary>
				''' This class can be used to represent a logical view for
				''' a flow.  It keeps the children updated to reflect the state
				''' of the model, gives the logical child views access to the
				''' view hierarchy, and calculates a preferred span.  It doesn't
				''' do any rendering, layout, or model/view translation.
				''' </summary>
				Friend Class LogicalView
					Inherits CompositeView

					Friend Sub New(ByVal elem As Element)
						MyBase.New(elem)
					End Sub

					Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
						Dim elem As Element = element
						If elem.elementCount > 0 Then Return elem.getElementIndex(pos)
						Return 0
					End Function

					Protected Friend Overrides Function updateChildren(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal f As ViewFactory) As Boolean
						Return False
					End Function

					Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
						Dim elem As Element = element
						If elem.elementCount > 0 Then
							MyBase.loadChildren(f)
						Else
							Dim v As View = New GlyphView(elem)
							append(v)
						End If
					End Sub

					Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
						If viewCount <> 1 Then Throw New Exception("One child view is assumed.")

						Dim v As View = getView(0)
						'((GlyphView)v).setGlyphPainter(null);
						Return v.getPreferredSpan(axis)
					End Function

					''' <summary>
					''' Forward the DocumentEvent to the given child view.  This
					''' is implemented to reparent the child to the logical view
					''' (the children may have been parented by a row in the flow
					''' if they fit without breaking) and then execute the
					''' superclass behavior.
					''' </summary>
					''' <param name="v"> the child view to forward the event to. </param>
					''' <param name="e"> the change information from the associated document </param>
					''' <param name="a"> the current allocation of the view </param>
					''' <param name="f"> the factory to use to rebuild if the view has
					'''          children </param>
					''' <seealso cref= #forwardUpdate
					''' @since 1.3 </seealso>
					Protected Friend Overrides Sub forwardUpdateToView(ByVal v As View, ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
						v.parent = Me
						MyBase.forwardUpdateToView(v, e, a, f)
					End Sub

					' The following methods don't do anything useful, they
					' simply keep the class from being abstract.

					Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
					End Sub

					Protected Friend Overrides Function isBefore(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
						Return False
					End Function

					Protected Friend Overrides Function isAfter(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
						Return False
					End Function

					Protected Friend Overrides Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View
						Return Nothing
					End Function

					Protected Friend Overrides Sub childAllocation(ByVal index As Integer, ByVal a As Rectangle)
					End Sub
				End Class
			End Class
		End Class

	' This is useful for the nightmare of parsing multi-part HTTP/RFC822 headers
	' * sensibly:
	' * From a String like: 'timeout=15, max=5'
	' * create an array of Strings:
	' * { {"timeout", "15"},
	' *   {"max", "5"}
	' * }
	' * From one like: 'Basic Realm="FuzzFace" Foo="Biz Bar Baz"'
	' * create one like (no quotes in literal):
	' * { {"basic", null},
	' *   {"realm", "FuzzFace"}
	' *   {"foo", "Biz Bar Baz"}
	' * }
	' * keys are converted to lower case, vals are left as is....
	' *
	' * author Dave Brown
	' 


	Friend Class HeaderParser

		' table of key/val pairs - maxes out at 10!!!!
		Friend raw As String
		Friend tab As String()()

		Public Sub New(ByVal raw As String)
			Me.raw = raw
			tab = RectangularArrays.ReturnRectangularStringArray(10, 2)
			parse()
		End Sub

		Private Sub parse()

			If raw IsNot Nothing Then
				raw = raw.Trim()
				Dim ca As Char() = raw.ToCharArray()
				Dim beg As Integer = 0, [end] As Integer = 0, i As Integer = 0
				Dim inKey As Boolean = True
				Dim inQuote As Boolean = False
				Dim len As Integer = ca.Length
				Do While [end] < len
					Dim c As Char = ca([end])
					If c = "="c Then ' end of a key
						tab(i)(0) = (New String(ca, beg, [end]-beg)).ToLower()
						inKey = False
						[end] += 1
						beg = [end]
					ElseIf c = """"c Then
						If inQuote Then
							tab(i)(1)= New String(ca, beg, [end]-beg)
							i += 1
							inQuote=False
							Do
								[end] += 1
							Loop While [end] < len AndAlso (ca([end]) = " "c OrElse ca([end]) = ","c)
							inKey=True
							beg=[end]
						Else
							inQuote=True
							[end] += 1
							beg=[end]
						End If ' end key/val, of whatever we're in
					ElseIf c = " "c OrElse c = ","c Then
						If inQuote Then
							[end] += 1
							Continue Do
						ElseIf inKey Then
							tab(i)(0) = (New String(ca, beg, [end]-beg)).ToLower()
							i += 1
						Else
							tab(i)(1) = (New String(ca, beg, [end]-beg))
							i += 1
						End If
						Do While [end] < len AndAlso (ca([end]) = " "c OrElse ca([end]) = ","c)
							[end] += 1
						Loop
						inKey = True
						beg = [end]
					Else
						[end] += 1
					End If
				Loop
				' get last key/val, if any
				[end] -= 1
				If [end] > beg Then
					If Not inKey Then
						If ca([end]) = """"c Then
							tab(i)(1) = (New String(ca, beg, [end]-beg))
							i += 1
						Else
							tab(i)(1) = (New String(ca, beg, [end]-beg+1))
							i += 1
						End If
					Else
						tab(i)(0) = (New String(ca, beg, [end]-beg+1)).ToLower()
					End If
				ElseIf [end] = beg Then
					If Not inKey Then
						If ca([end]) = """"c Then
							tab(i)(1) = Convert.ToString(ca([end]-1))
							i += 1
						Else
							tab(i)(1) = Convert.ToString(ca([end]))
							i += 1
						End If
					Else
						tab(i)(0) = Convert.ToString(ca([end])).ToLower()
					End If
				End If
			End If

		End Sub

		Public Overridable Function findKey(ByVal i As Integer) As String
			If i < 0 OrElse i > 10 Then Return Nothing
			Return tab(i)(0)
		End Function

		Public Overridable Function findValue(ByVal i As Integer) As String
			If i < 0 OrElse i > 10 Then Return Nothing
			Return tab(i)(1)
		End Function

		Public Overridable Function findValue(ByVal key As String) As String
			Return findValue(key, Nothing)
		End Function

		Public Overridable Function findValue(ByVal k As String, ByVal [Default] As String) As String
			If k Is Nothing Then Return [Default]
			k = k.ToLower()
			For i As Integer = 0 To 9
				If tab(i)(0) Is Nothing Then
					Return [Default]
				ElseIf k.Equals(tab(i)(0)) Then
					Return tab(i)(1)
				End If
			Next i
			Return [Default]
		End Function

		Public Overridable Function findInt(ByVal k As String, ByVal [Default] As Integer) As Integer
			Try
				Return Convert.ToInt32(findValue(k, Convert.ToString([Default])))
			Catch t As Exception
				Return [Default]
			End Try
		End Function
	End Class

	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularStringArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As String()()
        Dim Array As String()() = New String(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New String(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class