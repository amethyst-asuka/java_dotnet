Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.accessibility
Imports javax.print.attribute

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
Namespace javax.swing.text













	''' <summary>
	''' <code>JTextComponent</code> is the base class for swing text
	''' components.  It tries to be compatible with the
	''' <code>java.awt.TextComponent</code> class
	''' where it can reasonably do so.  Also provided are other services
	''' for additional flexibility (beyond the pluggable UI and bean
	''' support).
	''' You can find information on how to use the functionality
	''' this class provides in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/generaltext.html">General Rules for Using Text Components</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' 
	''' <dl>
	''' <dt><b><font size=+1>Caret Changes</font></b>
	''' <dd>
	''' The caret is a pluggable object in swing text components.
	''' Notification of changes to the caret position and the selection
	''' are sent to implementations of the <code>CaretListener</code>
	''' interface that have been registered with the text component.
	''' The UI will install a default caret unless a customized caret
	''' has been set. <br>
	''' By default the caret tracks all the document changes
	''' performed on the Event Dispatching Thread and updates it's position
	''' accordingly if an insertion occurs before or at the caret position
	''' or a removal occurs before the caret position. <code>DefaultCaret</code>
	''' tries to make itself visible which may lead to scrolling
	''' of a text component within <code>JScrollPane</code>. The default caret
	''' behavior can be changed by the <seealso cref="DefaultCaret#setUpdatePolicy"/> method.
	''' <br>
	''' <b>Note</b>: Non-editable text components also have a caret though
	''' it may not be painted.
	''' 
	''' <dt><b><font size=+1>Commands</font></b>
	''' <dd>
	''' Text components provide a number of commands that can be used
	''' to manipulate the component.  This is essentially the way that
	''' the component expresses its capabilities.  These are expressed
	''' in terms of the swing <code>Action</code> interface,
	''' using the <code>TextAction</code> implementation.
	''' The set of commands supported by the text component can be
	''' found with the <seealso cref="#getActions"/> method.  These actions
	''' can be bound to key events, fired from buttons, etc.
	''' 
	''' <dt><b><font size=+1>Text Input</font></b>
	''' <dd>
	''' The text components support flexible and internationalized text input, using
	''' keymaps and the input method framework, while maintaining compatibility with
	''' the AWT listener model.
	''' <p>
	''' A <seealso cref="javax.swing.text.Keymap"/> lets an application bind key
	''' strokes to actions.
	''' In order to allow keymaps to be shared across multiple text components, they
	''' can use actions that extend <code>TextAction</code>.
	''' <code>TextAction</code> can determine which <code>JTextComponent</code>
	''' most recently has or had focus and therefore is the subject of
	''' the action (In the case that the <code>ActionEvent</code>
	''' sent to the action doesn't contain the target text component as its source).
	''' <p>
	''' The <a href="../../../../technotes/guides/imf/spec.html">input method framework</a>
	''' lets text components interact with input methods, separate software
	''' components that preprocess events to let users enter thousands of
	''' different characters using keyboards with far fewer keys.
	''' <code>JTextComponent</code> is an <em>active client</em> of
	''' the framework, so it implements the preferred user interface for interacting
	''' with input methods. As a consequence, some key events do not reach the text
	''' component because they are handled by an input method, and some text input
	''' reaches the text component as committed text within an {@link
	''' java.awt.event.InputMethodEvent} instead of as a key event.
	''' The complete text input is the combination of the characters in
	''' <code>keyTyped</code> key events and committed text in input method events.
	''' <p>
	''' The AWT listener model lets applications attach event listeners to
	''' components in order to bind events to actions. Swing encourages the
	''' use of keymaps instead of listeners, but maintains compatibility
	''' with listeners by giving the listeners a chance to steal an event
	''' by consuming it.
	''' <p>
	''' Keyboard event and input method events are handled in the following stages,
	''' with each stage capable of consuming the event:
	''' 
	''' <table border=1 summary="Stages of keyboard and input method event handling">
	''' <tr>
	''' <th id="stage"><p style="text-align:left">Stage</p></th>
	''' <th id="ke"><p style="text-align:left">KeyEvent</p></th>
	''' <th id="ime"><p style="text-align:left">InputMethodEvent</p></th></tr>
	''' <tr><td headers="stage">1.   </td>
	'''     <td headers="ke">input methods </td>
	'''     <td headers="ime">(generated here)</td></tr>
	''' <tr><td headers="stage">2.   </td>
	'''     <td headers="ke">focus manager </td>
	'''     <td headers="ime"></td>
	''' </tr>
	''' <tr>
	'''     <td headers="stage">3.   </td>
	'''     <td headers="ke">registered key listeners</td>
	'''     <td headers="ime">registered input method listeners</tr>
	''' <tr>
	'''     <td headers="stage">4.   </td>
	'''     <td headers="ke"></td>
	'''     <td headers="ime">input method handling in JTextComponent</tr>
	''' <tr>
	'''     <td headers="stage">5.   </td><td headers="ke ime" colspan=2>keymap handling using the current keymap</td></tr>
	''' <tr><td headers="stage">6.   </td><td headers="ke">keyboard handling in JComponent (e.g. accelerators, component navigation, etc.)</td>
	'''     <td headers="ime"></td></tr>
	''' </table>
	''' 
	''' <p>
	''' To maintain compatibility with applications that listen to key
	''' events but are not aware of input method events, the input
	''' method handling in stage 4 provides a compatibility mode for
	''' components that do not process input method events. For these
	''' components, the committed text is converted to keyTyped key events
	''' and processed in the key event pipeline starting at stage 3
	''' instead of in the input method event pipeline.
	''' <p>
	''' By default the component will create a keymap (named <b>DEFAULT_KEYMAP</b>)
	''' that is shared by all JTextComponent instances as the default keymap.
	''' Typically a look-and-feel implementation will install a different keymap
	''' that resolves to the default keymap for those bindings not found in the
	''' different keymap. The minimal bindings include:
	''' <ul>
	''' <li>inserting content into the editor for the
	'''  printable keys.
	''' <li>removing content with the backspace and del
	'''  keys.
	''' <li>caret movement forward and backward
	''' </ul>
	''' 
	''' <dt><b><font size=+1>Model/View Split</font></b>
	''' <dd>
	''' The text components have a model-view split.  A text component pulls
	''' together the objects used to represent the model, view, and controller.
	''' The text document model may be shared by other views which act as observers
	''' of the model (e.g. a document may be shared by multiple components).
	''' 
	''' <p style="text-align:center"><img src="doc-files/editor.gif" alt="Diagram showing interaction between Controller, Document, events, and ViewFactory"
	'''                  HEIGHT=358 WIDTH=587></p>
	''' 
	''' <p>
	''' The model is defined by the <seealso cref="Document"/> interface.
	''' This is intended to provide a flexible text storage mechanism
	''' that tracks change during edits and can be extended to more sophisticated
	''' models.  The model interfaces are meant to capture the capabilities of
	''' expression given by SGML, a system used to express a wide variety of
	''' content.
	''' Each modification to the document causes notification of the
	''' details of the change to be sent to all observers in the form of a
	''' <seealso cref="DocumentEvent"/> which allows the views to stay up to date with the model.
	''' This event is sent to observers that have implemented the
	''' <seealso cref="DocumentListener"/>
	''' interface and registered interest with the model being observed.
	''' 
	''' <dt><b><font size=+1>Location Information</font></b>
	''' <dd>
	''' The capability of determining the location of text in
	''' the view is provided.  There are two methods, <seealso cref="#modelToView"/>
	''' and <seealso cref="#viewToModel"/> for determining this information.
	''' 
	''' <dt><b><font size=+1>Undo/Redo support</font></b>
	''' <dd>
	''' Support for an edit history mechanism is provided to allow
	''' undo/redo operations.  The text component does not itself
	''' provide the history buffer by default, but does provide
	''' the <code>UndoableEdit</code> records that can be used in conjunction
	''' with a history buffer to provide the undo/redo support.
	''' The support is provided by the Document model, which allows
	''' one to attach UndoableEditListener implementations.
	''' 
	''' <dt><b><font size=+1>Thread Safety</font></b>
	''' <dd>
	''' The swing text components provide some support of thread
	''' safe operations.  Because of the high level of configurability
	''' of the text components, it is possible to circumvent the
	''' protection provided.  The protection primarily comes from
	''' the model, so the documentation of <code>AbstractDocument</code>
	''' describes the assumptions of the protection provided.
	''' The methods that are safe to call asynchronously are marked
	''' with comments.
	''' 
	''' <dt><b><font size=+1>Newlines</font></b>
	''' <dd>
	''' For a discussion on how newlines are handled, see
	''' <a href="DefaultEditorKit.html">DefaultEditorKit</a>.
	''' 
	''' 
	''' <dt><b><font size=+1>Printing support</font></b>
	''' <dd>
	''' Several <seealso cref="#print print"/> methods are provided for basic
	''' document printing.  If more advanced printing is needed, use the
	''' <seealso cref="#getPrintable"/> method.
	''' </dl>
	''' 
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
	'''     attribute: isContainer false
	''' 
	''' @author  Timothy Prinzing
	''' @author Igor Kushnirskiy (printing support) </summary>
	''' <seealso cref= Document </seealso>
	''' <seealso cref= DocumentEvent </seealso>
	''' <seealso cref= DocumentListener </seealso>
	''' <seealso cref= Caret </seealso>
	''' <seealso cref= CaretEvent </seealso>
	''' <seealso cref= CaretListener </seealso>
	''' <seealso cref= TextUI </seealso>
	''' <seealso cref= View </seealso>
	''' <seealso cref= ViewFactory </seealso>
	Public MustInherit Class JTextComponent
		Inherits JComponent
		Implements Scrollable, Accessible

		''' <summary>
		''' Creates a new <code>JTextComponent</code>.
		''' Listeners for caret events are established, and the pluggable
		''' UI installed.  The component is marked as editable.  No layout manager
		''' is used, because layout is managed by the view subsystem of text.
		''' The document model is set to <code>null</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()
			' enable InputMethodEvent for on-the-spot pre-editing
			enableEvents(AWTEvent.KEY_EVENT_MASK Or AWTEvent.INPUT_METHOD_EVENT_MASK)
			caretEvent = New MutableCaretEvent(Me)
			addMouseListener(caretEvent)
			addFocusListener(caretEvent)
			editable = True
			dragEnabled = False
			layout = Nothing ' layout is managed by View hierarchy
			updateUI()
		End Sub

		''' <summary>
		''' Fetches the user-interface factory for this text-oriented editor.
		''' </summary>
		''' <returns> the factory </returns>
		Public Overridable Property uI As TextUI
			Get
				Return CType(ui, TextUI)
			End Get
			Set(ByVal ui As TextUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Reloads the pluggable UI.  The key used to fetch the
		''' new interface is <code>getUIClassID()</code>.  The type of
		''' the UI is <code>TextUI</code>.  <code>invalidate</code>
		''' is called after setting the UI.
		''' </summary>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), TextUI)
			invalidate()
		End Sub

		''' <summary>
		''' Adds a caret listener for notification of any changes
		''' to the caret.
		''' </summary>
		''' <param name="listener"> the listener to be added </param>
		''' <seealso cref= javax.swing.event.CaretEvent </seealso>
		Public Overridable Sub addCaretListener(ByVal listener As CaretListener)
			listenerList.add(GetType(CaretListener), listener)
		End Sub

		''' <summary>
		''' Removes a caret listener.
		''' </summary>
		''' <param name="listener"> the listener to be removed </param>
		''' <seealso cref= javax.swing.event.CaretEvent </seealso>
		Public Overridable Sub removeCaretListener(ByVal listener As CaretListener)
			listenerList.remove(GetType(CaretListener), listener)
		End Sub

		''' <summary>
		''' Returns an array of all the caret listeners
		''' registered on this text component.
		''' </summary>
		''' <returns> all of this component's <code>CaretListener</code>s
		'''         or an empty
		'''         array if no caret listeners are currently registered
		''' </returns>
		''' <seealso cref= #addCaretListener </seealso>
		''' <seealso cref= #removeCaretListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property caretListeners As CaretListener()
			Get
				Return listenerList.getListeners(GetType(CaretListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method.  The listener list is processed in a
		''' last-to-first manner.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireCaretUpdate(ByVal e As CaretEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(CaretListener) Then CType(___listeners(i+1), CaretListener).caretUpdate(e)
			Next i
		End Sub

		''' <summary>
		''' Associates the editor with a text document.
		''' The currently registered factory is used to build a view for
		''' the document, which gets displayed by the editor after revalidation.
		''' A PropertyChange event ("document") is propagated to each listener.
		''' </summary>
		''' <param name="doc">  the document to display/edit </param>
		''' <seealso cref= #getDocument
		''' @beaninfo
		'''  description: the text document model
		'''        bound: true
		'''       expert: true </seealso>
		Public Overridable Property document As Document
			Set(ByVal doc As Document)
				Dim old As Document = model
    
		'        
		'         * acquire a read lock on the old model to prevent notification of
		'         * mutations while we disconnecting the old model.
		'         
				Try
					If TypeOf old Is AbstractDocument Then CType(old, AbstractDocument).readLock()
					If accessibleContext IsNot Nothing Then model.removeDocumentListener((CType(accessibleContext, AccessibleJTextComponent)))
					If inputMethodRequestsHandler IsNot Nothing Then model.removeDocumentListener(CType(inputMethodRequestsHandler, DocumentListener))
					model = doc
    
					' Set the document's run direction property to match the
					' component's ComponentOrientation property.
					Dim runDir As Boolean? = If(componentOrientation.leftToRight, java.awt.font.TextAttribute.RUN_DIRECTION_LTR, java.awt.font.TextAttribute.RUN_DIRECTION_RTL)
					If runDir IsNot doc.getProperty(java.awt.font.TextAttribute.RUN_DIRECTION) Then doc.putProperty(java.awt.font.TextAttribute.RUN_DIRECTION, runDir)
					firePropertyChange("document", old, doc)
				Finally
					If TypeOf old Is AbstractDocument Then CType(old, AbstractDocument).readUnlock()
				End Try
    
				revalidate()
				repaint()
				If accessibleContext IsNot Nothing Then model.addDocumentListener((CType(accessibleContext, AccessibleJTextComponent)))
				If inputMethodRequestsHandler IsNot Nothing Then model.addDocumentListener(CType(inputMethodRequestsHandler, DocumentListener))
			End Set
			Get
				Return model
			End Get
		End Property


		' Override of Component.setComponentOrientation
		Public Overridable Property componentOrientation As ComponentOrientation
			Set(ByVal o As ComponentOrientation)
				' Set the document's run direction property to match the
				' ComponentOrientation property.
				Dim doc As Document = document
				If doc IsNot Nothing Then
					Dim runDir As Boolean? = If(o.leftToRight, java.awt.font.TextAttribute.RUN_DIRECTION_LTR, java.awt.font.TextAttribute.RUN_DIRECTION_RTL)
					doc.putProperty(java.awt.font.TextAttribute.RUN_DIRECTION, runDir)
				End If
				MyBase.componentOrientation = o
			End Set
		End Property

		''' <summary>
		''' Fetches the command list for the editor.  This is
		''' the list of commands supported by the plugged-in UI
		''' augmented by the collection of commands that the
		''' editor itself supports.  These are useful for binding
		''' to events, such as in a keymap.
		''' </summary>
		''' <returns> the command list </returns>
		Public Overridable Property actions As Action()
			Get
				Return uI.getEditorKit(Me).actions
			End Get
		End Property

		''' <summary>
		''' Sets margin space between the text component's border
		''' and its text.  The text component's default <code>Border</code>
		''' object will use this value to create the proper margin.
		''' However, if a non-default border is set on the text component,
		''' it is that <code>Border</code> object's responsibility to create the
		''' appropriate margin space (else this property will effectively
		''' be ignored).  This causes a redraw of the component.
		''' A PropertyChange event ("margin") is sent to all listeners.
		''' </summary>
		''' <param name="m"> the space between the border and the text
		''' @beaninfo
		'''  description: desired space between the border and text area
		'''        bound: true </param>
		Public Overridable Property margin As Insets
			Set(ByVal m As Insets)
				Dim old As Insets = margin
				margin = m
				firePropertyChange("margin", old, m)
				invalidate()
			End Set
			Get
				Return margin
			End Get
		End Property


		''' <summary>
		''' Sets the <code>NavigationFilter</code>. <code>NavigationFilter</code>
		''' is used by <code>DefaultCaret</code> and the default cursor movement
		''' actions as a way to restrict the cursor movement.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Property navigationFilter As NavigationFilter
			Set(ByVal filter As NavigationFilter)
				navigationFilter = filter
			End Set
			Get
				Return navigationFilter
			End Get
		End Property


		''' <summary>
		''' Fetches the caret that allows text-oriented navigation over
		''' the view.
		''' </summary>
		''' <returns> the caret </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property caret As Caret
			Get
				Return caret
			End Get
			Set(ByVal c As Caret)
				If caret IsNot Nothing Then
					caret.removeChangeListener(caretEvent)
					caret.deinstall(Me)
				End If
				Dim old As Caret = caret
				caret = c
				If caret IsNot Nothing Then
					caret.install(Me)
					caret.addChangeListener(caretEvent)
				End If
				firePropertyChange("caret", old, caret)
			End Set
		End Property


		''' <summary>
		''' Fetches the object responsible for making highlights.
		''' </summary>
		''' <returns> the highlighter </returns>
		Public Overridable Property highlighter As Highlighter
			Get
				Return highlighter
			End Get
			Set(ByVal h As Highlighter)
				If highlighter IsNot Nothing Then highlighter.deinstall(Me)
				Dim old As Highlighter = highlighter
				highlighter = h
				If highlighter IsNot Nothing Then highlighter.install(Me)
				firePropertyChange("highlighter", old, h)
			End Set
		End Property


		''' <summary>
		''' Sets the keymap to use for binding events to
		''' actions.  Setting to <code>null</code> effectively disables
		''' keyboard input.
		''' A PropertyChange event ("keymap") is fired when a new keymap
		''' is installed.
		''' </summary>
		''' <param name="map"> the keymap </param>
		''' <seealso cref= #getKeymap
		''' @beaninfo
		'''  description: set of key event to action bindings to use
		'''        bound: true </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setKeymap(ByVal map As Keymap) 'JavaToDotNetTempPropertySetkeymap
		Public Overridable Property keymap As Keymap
			Set(ByVal map As Keymap)
				Dim old As Keymap = keymap
				keymap = map
				firePropertyChange("keymap", old, keymap)
				updateInputMap(old, map)
			End Set
			Get
		End Property

		''' <summary>
		''' Turns on or off automatic drag handling. In order to enable automatic
		''' drag handling, this property should be set to {@code true}, and the
		''' component's {@code TransferHandler} needs to be {@code non-null}.
		''' The default value of the {@code dragEnabled} property is {@code false}.
		''' <p>
		''' The job of honoring this property, and recognizing a user drag gesture,
		''' lies with the look and feel implementation, and in particular, the component's
		''' {@code TextUI}. When automatic drag handling is enabled, most look and
		''' feels (including those that subclass {@code BasicLookAndFeel}) begin a
		''' drag and drop operation whenever the user presses the mouse button over
		''' a selection and then moves the mouse a few pixels. Setting this property to
		''' {@code true} can therefore have a subtle effect on how selections behave.
		''' <p>
		''' If a look and feel is used that ignores this property, you can still
		''' begin a drag and drop operation by calling {@code exportAsDrag} on the
		''' component's {@code TransferHandler}.
		''' </summary>
		''' <param name="b"> whether or not to enable automatic drag handling </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.4
		''' 
		''' @beaninfo
		'''  description: determines whether automatic drag handling is enabled
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso GraphicsEnvironment.headless Then Throw New HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the drop mode for this component. For backward compatibility,
		''' the default for this property is <code>DropMode.USE_SELECTION</code>.
		''' Usage of <code>DropMode.INSERT</code> is recommended, however,
		''' for an improved user experience. It offers similar behavior of dropping
		''' between text locations, but does so without affecting the actual text
		''' selection and caret location.
		''' <p>
		''' <code>JTextComponents</code> support the following drop modes:
		''' <ul>
		'''    <li><code>DropMode.USE_SELECTION</code></li>
		'''    <li><code>DropMode.INSERT</code></li>
		''' </ul>
		''' <p>
		''' The drop mode is only meaningful if this component has a
		''' <code>TransferHandler</code> that accepts drops.
		''' </summary>
		''' <param name="dropMode"> the drop mode to use </param>
		''' <exception cref="IllegalArgumentException"> if the drop mode is unsupported
		'''         or <code>null</code> </exception>
		''' <seealso cref= #getDropMode </seealso>
		''' <seealso cref= #getDropLocation </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= javax.swing.TransferHandler
		''' @since 1.6 </seealso>
		Public Property dropMode As DropMode
			Set(ByVal dropMode As DropMode)
				If dropMode IsNot Nothing Then
					Select Case dropMode
						Case DropMode.USE_SELECTION, INSERT
							Me.dropMode = dropMode
							Return
					End Select
				End If
    
				Throw New System.ArgumentException(dropMode & ": Unsupported drop mode for text")
			End Set
			Get
				Return dropMode
			End Get
		End Property


		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.swing.SwingAccessor.setJTextComponentAccessor(New sun.swing.SwingAccessor.JTextComponentAccessor()
	'		{
	'				public TransferHandler.DropLocation dropLocationForPoint(JTextComponent textComp, Point p)
	'				{
	'					Return textComp.dropLocationForPoint(p);
	'				}
	'				public Object setDropLocation(JTextComponent textComp, TransferHandler.DropLocation location, Object state, boolean forDrop)
	'				{
	'					Return textComp.setDropLocation(location, state, forDrop);
	'				}
	'			});
		End Sub


		''' <summary>
		''' Calculates a drop location in this component, representing where a
		''' drop at the given point should insert data.
		''' <p>
		''' Note: This method is meant to override
		''' <code>JComponent.dropLocationForPoint()</code>, which is package-private
		''' in javax.swing. <code>TransferHandler</code> will detect text components
		''' and call this method instead via reflection. It's name should therefore
		''' not be changed.
		''' </summary>
		''' <param name="p"> the point to calculate a drop location for </param>
		''' <returns> the drop location, or <code>null</code> </returns>
		Friend Overrides Function dropLocationForPoint(ByVal p As Point) As DropLocation
			Dim bias As Position.Bias() = New Position.Bias(0){}
			Dim index As Integer = uI.viewToModel(Me, p, bias)

			' viewToModel currently returns null for some HTML content
			' when the point is within the component's top inset
			If bias(0) Is Nothing Then bias(0) = Position.Bias.Forward

			Return New DropLocation(p, index, bias(0))
		End Function

		''' <summary>
		''' Called to set or clear the drop location during a DnD operation.
		''' In some cases, the component may need to use it's internal selection
		''' temporarily to indicate the drop location. To help facilitate this,
		''' this method returns and accepts as a parameter a state object.
		''' This state object can be used to store, and later restore, the selection
		''' state. Whatever this method returns will be passed back to it in
		''' future calls, as the state parameter. If it wants the DnD system to
		''' continue storing the same state, it must pass it back every time.
		''' Here's how this is used:
		''' <p>
		''' Let's say that on the first call to this method the component decides
		''' to save some state (because it is about to use the selection to show
		''' a drop index). It can return a state object to the caller encapsulating
		''' any saved selection state. On a second call, let's say the drop location
		''' is being changed to something else. The component doesn't need to
		''' restore anything yet, so it simply passes back the same state object
		''' to have the DnD system continue storing it. Finally, let's say this
		''' method is messaged with <code>null</code>. This means DnD
		''' is finished with this component for now, meaning it should restore
		''' state. At this point, it can use the state parameter to restore
		''' said state, and of course return <code>null</code> since there's
		''' no longer anything to store.
		''' <p>
		''' Note: This method is meant to override
		''' <code>JComponent.setDropLocation()</code>, which is package-private
		''' in javax.swing. <code>TransferHandler</code> will detect text components
		''' and call this method instead via reflection. It's name should therefore
		''' not be changed.
		''' </summary>
		''' <param name="location"> the drop location (as calculated by
		'''        <code>dropLocationForPoint</code>) or <code>null</code>
		'''        if there's no longer a valid drop location </param>
		''' <param name="state"> the state object saved earlier for this component,
		'''        or <code>null</code> </param>
		''' <param name="forDrop"> whether or not the method is being called because an
		'''        actual drop occurred </param>
		''' <returns> any saved state for this component, or <code>null</code> if none </returns>
		Friend Overrides Function setDropLocation(ByVal location As TransferHandler.DropLocation, ByVal state As Object, ByVal forDrop As Boolean) As Object

			Dim retVal As Object = Nothing
			Dim textLocation As DropLocation = CType(location, DropLocation)

			If dropMode = DropMode.USE_SELECTION Then
				If textLocation Is Nothing Then
					If state IsNot Nothing Then
	'                    
	'                     * This object represents the state saved earlier.
	'                     *     If the caret is a DefaultCaret it will be
	'                     *     an Object array containing, in order:
	'                     *         - the saved caret mark (Integer)
	'                     *         - the saved caret dot (Integer)
	'                     *         - the saved caret visibility (Boolean)
	'                     *         - the saved mark bias (Position.Bias)
	'                     *         - the saved dot bias (Position.Bias)
	'                     *     If the caret is not a DefaultCaret it will
	'                     *     be similar, but will not contain the dot
	'                     *     or mark bias.
	'                     
						Dim vals As Object() = CType(state, Object())

						If Not forDrop Then
							If TypeOf caret Is DefaultCaret Then
								CType(caret, DefaultCaret).dotDot(CInt(Fix(vals(0))), CType(vals(3), Position.Bias))
								CType(caret, DefaultCaret).moveDot(CInt(Fix(vals(1))), CType(vals(4), Position.Bias))
							Else
								caret.dot = CInt(Fix(vals(0)))
								caret.moveDot(CInt(Fix(vals(1))))
							End If
						End If

						caret.visible = CBool(vals(2))
					End If
				Else
					If dropLocation Is Nothing Then
						Dim ___visible As Boolean

						If TypeOf caret Is DefaultCaret Then
							Dim dc As DefaultCaret = CType(caret, DefaultCaret)
							___visible = dc.active
							retVal = New Object() {Convert.ToInt32(dc.mark), Convert.ToInt32(dc.dot), Convert.ToBoolean(___visible), dc.markBias, dc.dotBias}
						Else
							___visible = caret.visible
							retVal = New Object() {Convert.ToInt32(caret.mark), Convert.ToInt32(caret.dot), Convert.ToBoolean(___visible)}
						End If

						caret.visible = True
					Else
						retVal = state
					End If

					If TypeOf caret Is DefaultCaret Then
						CType(caret, DefaultCaret).dotDot(textLocation.index, textLocation.bias)
					Else
						caret.dot = textLocation.index
					End If
				End If
			Else
				If textLocation Is Nothing Then
					If state IsNot Nothing Then caret.visible = CBool(state)
				Else
					If dropLocation Is Nothing Then
						Dim ___visible As Boolean = If(TypeOf caret Is DefaultCaret, CType(caret, DefaultCaret).active, caret.visible)
						retVal = Convert.ToBoolean(___visible)
						caret.visible = False
					Else
						retVal = state
					End If
				End If
			End If

			Dim old As DropLocation = dropLocation
			dropLocation = textLocation
			firePropertyChange("dropLocation", old, dropLocation)

			Return retVal
		End Function

		''' <summary>
		''' Returns the location that this component should visually indicate
		''' as the drop location during a DnD operation over the component,
		''' or {@code null} if no location is to currently be shown.
		''' <p>
		''' This method is not meant for querying the drop location
		''' from a {@code TransferHandler}, as the drop location is only
		''' set after the {@code TransferHandler}'s <code>canImport</code>
		''' has returned and has allowed for the location to be shown.
		''' <p>
		''' When this property changes, a property change event with
		''' name "dropLocation" is fired by the component.
		''' </summary>
		''' <returns> the drop location </returns>
		''' <seealso cref= #setDropMode </seealso>
		''' <seealso cref= TransferHandler#canImport(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public Property dropLocation As DropLocation
			Get
				Return dropLocation
			End Get
		End Property


		''' <summary>
		''' Updates the <code>InputMap</code>s in response to a
		''' <code>Keymap</code> change. </summary>
		''' <param name="oldKm">  the old <code>Keymap</code> </param>
		''' <param name="newKm">  the new <code>Keymap</code> </param>
		Friend Overridable Sub updateInputMap(ByVal oldKm As Keymap, ByVal newKm As Keymap)
			' Locate the current KeymapWrapper.
			Dim km As InputMap = getInputMap(JComponent.WHEN_FOCUSED)
			Dim last As InputMap = km
			Do While km IsNot Nothing AndAlso Not(TypeOf km Is KeymapWrapper)
				last = km
				km = km.parent
			Loop
			If km IsNot Nothing Then
				' Found it, tweak the InputMap that points to it, as well
				' as anything it points to.
				If newKm Is Nothing Then
					If last IsNot km Then
						last.parent = km.parent
					Else
						last.parent = Nothing
					End If
				Else
					Dim ___newKM As InputMap = New KeymapWrapper(newKm)
					last.parent = ___newKM
					If last IsNot km Then ___newKM.parent = km.parent
				End If
			ElseIf newKm IsNot Nothing Then
				km = getInputMap(JComponent.WHEN_FOCUSED)
				If km IsNot Nothing Then
					' Couldn't find it.
					' Set the parent of WHEN_FOCUSED InputMap to be the new one.
					Dim ___newKM As InputMap = New KeymapWrapper(newKm)
					___newKM.parent = km.parent
					km.parent = ___newKM
				End If
			End If

			' Do the same thing with the ActionMap
			Dim am As ActionMap = actionMap
			Dim lastAM As ActionMap = am
			Do While am IsNot Nothing AndAlso Not(TypeOf am Is KeymapActionMap)
				lastAM = am
				am = am.parent
			Loop
			If am IsNot Nothing Then
				' Found it, tweak the Actionap that points to it, as well
				' as anything it points to.
				If newKm Is Nothing Then
					If lastAM IsNot am Then
						lastAM.parent = am.parent
					Else
						lastAM.parent = Nothing
					End If
				Else
					Dim newAM As ActionMap = New KeymapActionMap(newKm)
					lastAM.parent = newAM
					If lastAM IsNot am Then newAM.parent = am.parent
				End If
			ElseIf newKm IsNot Nothing Then
				am = actionMap
				If am IsNot Nothing Then
					' Couldn't find it.
					' Set the parent of ActionMap to be the new one.
					Dim newAM As ActionMap = New KeymapActionMap(newKm)
					newAM.parent = am.parent
					am.parent = newAM
				End If
			End If
		End Sub

			Return keymap
		End Function

		''' <summary>
		''' Adds a new keymap into the keymap hierarchy.  Keymap bindings
		''' resolve from bottom up so an attribute specified in a child
		''' will override an attribute specified in the parent.
		''' </summary>
		''' <param name="nm">   the name of the keymap (must be unique within the
		'''   collection of named keymaps in the document); the name may
		'''   be <code>null</code> if the keymap is unnamed,
		'''   but the caller is responsible for managing the reference
		'''   returned as an unnamed keymap can't
		'''   be fetched by name </param>
		''' <param name="parent"> the parent keymap; this may be <code>null</code> if
		'''   unspecified bindings need not be resolved in some other keymap </param>
		''' <returns> the keymap </returns>
		Public Shared Function addKeymap(ByVal nm As String, ByVal parent As Keymap) As Keymap
			Dim map As Keymap = New DefaultKeymap(nm, parent)
			If nm IsNot Nothing Then keymapTable(nm) = map
			Return map
		End Function

		''' <summary>
		''' Removes a named keymap previously added to the document.  Keymaps
		''' with <code>null</code> names may not be removed in this way.
		''' </summary>
		''' <param name="nm">  the name of the keymap to remove </param>
		''' <returns> the keymap that was removed </returns>
		Public Shared Function removeKeymap(ByVal nm As String) As Keymap
			Return keymapTable.Remove(nm)
		End Function

		''' <summary>
		''' Fetches a named keymap previously added to the document.
		''' This does not work with <code>null</code>-named keymaps.
		''' </summary>
		''' <param name="nm">  the name of the keymap </param>
		''' <returns> the keymap </returns>
		Public Shared Function getKeymap(ByVal nm As String) As Keymap
			Return keymapTable(nm)
		End Function

		Private Property Shared keymapTable As Dictionary(Of String, Keymap)
			Get
				SyncLock KEYMAP_TABLE
					Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
					Dim ___keymapTable As Dictionary(Of String, Keymap) = CType(appContext.get(KEYMAP_TABLE), Dictionary(Of String, Keymap))
					If ___keymapTable Is Nothing Then
						___keymapTable = New Dictionary(Of String, Keymap)(17)
						appContext.put(KEYMAP_TABLE, ___keymapTable)
						'initialize default keymap
						Dim binding As Keymap = addKeymap(DEFAULT_KEYMAP, Nothing)
						binding.defaultAction = New DefaultEditorKit.DefaultKeyTypedAction
					End If
					Return ___keymapTable
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Binding record for creating key bindings.
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
		Public Class KeyBinding

			''' <summary>
			''' The key.
			''' </summary>
			Public key As KeyStroke

			''' <summary>
			''' The name of the action for the key.
			''' </summary>
			Public actionName As String

			''' <summary>
			''' Creates a new key binding.
			''' </summary>
			''' <param name="key"> the key </param>
			''' <param name="actionName"> the name of the action for the key </param>
			Public Sub New(ByVal key As KeyStroke, ByVal actionName As String)
				Me.key = key
				Me.actionName = actionName
			End Sub
		End Class

		''' <summary>
		''' <p>
		''' Loads a keymap with a bunch of
		''' bindings.  This can be used to take a static table of
		''' definitions and load them into some keymap.  The following
		''' example illustrates an example of binding some keys to
		''' the cut, copy, and paste actions associated with a
		''' JTextComponent.  A code fragment to accomplish
		''' this might look as follows:
		''' <pre><code>
		''' 
		'''   static final JTextComponent.KeyBinding[] defaultBindings = {
		'''     new JTextComponent.KeyBinding(
		'''       KeyStroke.getKeyStroke(KeyEvent.VK_C, InputEvent.CTRL_MASK),
		'''       DefaultEditorKit.copyAction),
		'''     new JTextComponent.KeyBinding(
		'''       KeyStroke.getKeyStroke(KeyEvent.VK_V, InputEvent.CTRL_MASK),
		'''       DefaultEditorKit.pasteAction),
		'''     new JTextComponent.KeyBinding(
		'''       KeyStroke.getKeyStroke(KeyEvent.VK_X, InputEvent.CTRL_MASK),
		'''       DefaultEditorKit.cutAction),
		'''   };
		''' 
		'''   JTextComponent c = new JTextPane();
		'''   Keymap k = c.getKeymap();
		'''   JTextComponent.loadKeymap(k, defaultBindings, c.getActions());
		''' 
		''' </code></pre>
		''' The sets of bindings and actions may be empty but must be
		''' non-<code>null</code>.
		''' </summary>
		''' <param name="map"> the keymap </param>
		''' <param name="bindings"> the bindings </param>
		''' <param name="actions"> the set of actions </param>
		Public Shared Sub loadKeymap(ByVal map As Keymap, ByVal bindings As KeyBinding(), ByVal actions As Action())
			Dim h As New Dictionary(Of String, Action)
			For Each a As Action In actions
				Dim value As String = CStr(a.getValue(Action.NAME))
				h((If(value IsNot Nothing, value, ""))) = a
			Next a
			For Each binding As KeyBinding In bindings
				Dim a As Action = h(binding.actionName)
				If a IsNot Nothing Then map.addActionForKeyStroke(binding.key, a)
			Next binding
		End Sub

		''' <summary>
		''' Fetches the current color used to render the
		''' caret.
		''' </summary>
		''' <returns> the color </returns>
		Public Overridable Property caretColor As Color
			Get
				Return caretColor
			End Get
			Set(ByVal c As Color)
				Dim old As Color = caretColor
				caretColor = c
				firePropertyChange("caretColor", old, caretColor)
			End Set
		End Property


		''' <summary>
		''' Fetches the current color used to render the
		''' selection.
		''' </summary>
		''' <returns> the color </returns>
		Public Overridable Property selectionColor As Color
			Get
				Return selectionColor
			End Get
			Set(ByVal c As Color)
				Dim old As Color = selectionColor
				selectionColor = c
				firePropertyChange("selectionColor", old, selectionColor)
			End Set
		End Property


		''' <summary>
		''' Fetches the current color used to render the
		''' selected text.
		''' </summary>
		''' <returns> the color </returns>
		Public Overridable Property selectedTextColor As Color
			Get
				Return selectedTextColor
			End Get
			Set(ByVal c As Color)
				Dim old As Color = selectedTextColor
				selectedTextColor = c
				firePropertyChange("selectedTextColor", old, selectedTextColor)
			End Set
		End Property


		''' <summary>
		''' Fetches the current color used to render the
		''' disabled text.
		''' </summary>
		''' <returns> the color </returns>
		Public Overridable Property disabledTextColor As Color
			Get
				Return disabledTextColor
			End Get
			Set(ByVal c As Color)
				Dim old As Color = disabledTextColor
				disabledTextColor = c
				firePropertyChange("disabledTextColor", old, disabledTextColor)
			End Set
		End Property


		''' <summary>
		''' Replaces the currently selected content with new content
		''' represented by the given string.  If there is no selection
		''' this amounts to an insert of the given text.  If there
		''' is no replacement text this amounts to a removal of the
		''' current selection.
		''' <p>
		''' This is the method that is used by the default implementation
		''' of the action for inserting content that gets bound to the
		''' keymap actions.
		''' </summary>
		''' <param name="content">  the content to replace the selection with </param>
		Public Overridable Sub replaceSelection(ByVal content As String)
			Dim doc As Document = document
			If doc IsNot Nothing Then
				Try
					Dim composedTextSaved As Boolean = saveComposedText(caret.dot)
					Dim p0 As Integer = Math.Min(caret.dot, caret.mark)
					Dim p1 As Integer = Math.Max(caret.dot, caret.mark)
					If TypeOf doc Is AbstractDocument Then
						CType(doc, AbstractDocument).replace(p0, p1 - p0, content,Nothing)
					Else
						If p0 <> p1 Then doc.remove(p0, p1 - p0)
						If content IsNot Nothing AndAlso content.Length > 0 Then doc.insertString(p0, content, Nothing)
					End If
					If composedTextSaved Then restoreComposedText()
				Catch e As BadLocationException
					UIManager.lookAndFeel.provideErrorFeedback(JTextComponent.this)
				End Try
			End If
		End Sub

		''' <summary>
		''' Fetches a portion of the text represented by the
		''' component.  Returns an empty string if length is 0.
		''' </summary>
		''' <param name="offs"> the offset &ge; 0 </param>
		''' <param name="len"> the length &ge; 0 </param>
		''' <returns> the text </returns>
		''' <exception cref="BadLocationException"> if the offset or length are invalid </exception>
		Public Overridable Function getText(ByVal offs As Integer, ByVal len As Integer) As String
			Return document.getText(offs, len)
		End Function

		''' <summary>
		''' Converts the given location in the model to a place in
		''' the view coordinate system.
		''' The component must have a positive size for
		''' this translation to be computed (i.e. layout cannot
		''' be computed until the component has been sized).  The
		''' component does not have to be visible or painted.
		''' </summary>
		''' <param name="pos"> the position &ge; 0 </param>
		''' <returns> the coordinates as a rectangle, with (r.x, r.y) as the location
		'''   in the coordinate system, or null if the component does
		'''   not yet have a positive size. </returns>
		''' <exception cref="BadLocationException"> if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= TextUI#modelToView </seealso>
		Public Overridable Function modelToView(ByVal pos As Integer) As Rectangle
			Return uI.modelToView(Me, pos)
		End Function

		''' <summary>
		''' Converts the given place in the view coordinate system
		''' to the nearest representative location in the model.
		''' The component must have a positive size for
		''' this translation to be computed (i.e. layout cannot
		''' be computed until the component has been sized).  The
		''' component does not have to be visible or painted.
		''' </summary>
		''' <param name="pt"> the location in the view to translate </param>
		''' <returns> the offset &ge; 0 from the start of the document,
		'''   or -1 if the component does not yet have a positive
		'''   size. </returns>
		''' <seealso cref= TextUI#viewToModel </seealso>
		Public Overridable Function viewToModel(ByVal pt As Point) As Integer
			Return uI.viewToModel(Me, pt)
		End Function

		''' <summary>
		''' Transfers the currently selected range in the associated
		''' text model to the system clipboard, removing the contents
		''' from the model.  The current selection is reset.  Does nothing
		''' for <code>null</code> selections.
		''' </summary>
		''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		''' <seealso cref= java.awt.datatransfer.Clipboard </seealso>
		Public Overridable Sub cut()
			If editable AndAlso enabled Then invokeAction("cut", TransferHandler.cutAction)
		End Sub

		''' <summary>
		''' Transfers the currently selected range in the associated
		''' text model to the system clipboard, leaving the contents
		''' in the text model.  The current selection remains intact.
		''' Does nothing for <code>null</code> selections.
		''' </summary>
		''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		''' <seealso cref= java.awt.datatransfer.Clipboard </seealso>
		Public Overridable Sub copy()
			invokeAction("copy", TransferHandler.copyAction)
		End Sub

		''' <summary>
		''' Transfers the contents of the system clipboard into the
		''' associated text model.  If there is a selection in the
		''' associated view, it is replaced with the contents of the
		''' clipboard.  If there is no selection, the clipboard contents
		''' are inserted in front of the current insert position in
		''' the associated view.  If the clipboard is empty, does nothing.
		''' </summary>
		''' <seealso cref= #replaceSelection </seealso>
		''' <seealso cref= java.awt.Toolkit#getSystemClipboard </seealso>
		''' <seealso cref= java.awt.datatransfer.Clipboard </seealso>
		Public Overridable Sub paste()
			If editable AndAlso enabled Then invokeAction("paste", TransferHandler.pasteAction)
		End Sub

		''' <summary>
		''' This is a convenience method that is only useful for
		''' <code>cut</code>, <code>copy</code> and <code>paste</code>.  If
		''' an <code>Action</code> with the name <code>name</code> does not
		''' exist in the <code>ActionMap</code>, this will attempt to install a
		''' <code>TransferHandler</code> and then use <code>altAction</code>.
		''' </summary>
		Private Sub invokeAction(ByVal name As String, ByVal altAction As Action)
			Dim map As ActionMap = actionMap
			Dim action As Action = Nothing

			If map IsNot Nothing Then action = map.get(name)
			If action Is Nothing Then
				installDefaultTransferHandlerIfNecessary()
				action = altAction
			End If
			action.actionPerformed(New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, CStr(action.getValue(Action.NAME)), EventQueue.mostRecentEventTime, currentEventModifiers))
		End Sub

		''' <summary>
		''' If the current <code>TransferHandler</code> is null, this will
		''' install a new one.
		''' </summary>
		Private Sub installDefaultTransferHandlerIfNecessary()
			If transferHandler Is Nothing Then
				If defaultTransferHandler Is Nothing Then defaultTransferHandler = New DefaultTransferHandler
				transferHandler = defaultTransferHandler
			End If
		End Sub

		''' <summary>
		''' Moves the caret to a new position, leaving behind a mark
		''' defined by the last time <code>setCaretPosition</code> was
		''' called.  This forms a selection.
		''' If the document is <code>null</code>, does nothing. The position
		''' must be between 0 and the length of the component's text or else
		''' an exception is thrown.
		''' </summary>
		''' <param name="pos"> the position </param>
		''' <exception cref="IllegalArgumentException"> if the value supplied
		'''               for <code>position</code> is less than zero or greater
		'''               than the component's text length </exception>
		''' <seealso cref= #setCaretPosition </seealso>
		Public Overridable Sub moveCaretPosition(ByVal pos As Integer)
			Dim doc As Document = document
			If doc IsNot Nothing Then
				If pos > doc.length OrElse pos < 0 Then Throw New System.ArgumentException("bad position: " & pos)
				caret.moveDot(pos)
			End If
		End Sub

		''' <summary>
		''' The bound property name for the focus accelerator.
		''' </summary>
		Public Const FOCUS_ACCELERATOR_KEY As String = "focusAcceleratorKey"

		''' <summary>
		''' Sets the key accelerator that will cause the receiving text
		''' component to get the focus.  The accelerator will be the
		''' key combination of the platform-specific modifier key and
		''' the character given (converted to upper case).  For example,
		''' the ALT key is used as a modifier on Windows and the CTRL+ALT
		''' combination is used on Mac.  By default, there is no focus
		''' accelerator key.  Any previous key accelerator setting will be
		''' superseded.  A '\0' key setting will be registered, and has the
		''' effect of turning off the focus accelerator.  When the new key
		''' is set, a PropertyChange event (FOCUS_ACCELERATOR_KEY) will be fired.
		''' </summary>
		''' <param name="aKey"> the key </param>
		''' <seealso cref= #getFocusAccelerator
		''' @beaninfo
		'''  description: accelerator character used to grab focus
		'''        bound: true </seealso>
		Public Overridable Property focusAccelerator As Char
			Set(ByVal aKey As Char)
				aKey = Char.ToUpper(aKey)
				Dim old As Char = focusAccelerator
				focusAccelerator = aKey
				' Fix for 4341002: value of FOCUS_ACCELERATOR_KEY is wrong.
				' So we fire both FOCUS_ACCELERATOR_KEY, for compatibility,
				' and the correct event here.
				firePropertyChange(FOCUS_ACCELERATOR_KEY, old, focusAccelerator)
				firePropertyChange("focusAccelerator", old, focusAccelerator)
			End Set
			Get
				Return focusAccelerator
			End Get
		End Property


		''' <summary>
		''' Initializes from a stream.  This creates a
		''' model of the type appropriate for the component
		''' and initializes the model from the stream.
		''' By default this will load the model as plain
		''' text.  Previous contents of the model are discarded.
		''' </summary>
		''' <param name="in"> the stream to read from </param>
		''' <param name="desc"> an object describing the stream; this
		'''   might be a string, a File, a URL, etc.  Some kinds
		'''   of documents (such as html for example) might be
		'''   able to make use of this information; if non-<code>null</code>,
		'''   it is added as a property of the document </param>
		''' <exception cref="IOException"> as thrown by the stream being
		'''  used to initialize </exception>
		''' <seealso cref= EditorKit#createDefaultDocument </seealso>
		''' <seealso cref= #setDocument </seealso>
		''' <seealso cref= PlainDocument </seealso>
		Public Overridable Sub read(ByVal [in] As Reader, ByVal desc As Object)
			Dim kit As EditorKit = uI.getEditorKit(Me)
			Dim doc As Document = kit.createDefaultDocument()
			If desc IsNot Nothing Then doc.putProperty(Document.StreamDescriptionProperty, desc)
			Try
				kit.read([in], doc, 0)
				document = doc
			Catch e As BadLocationException
				Throw New IOException(e.Message)
			End Try
		End Sub

		''' <summary>
		''' Stores the contents of the model into the given
		''' stream.  By default this will store the model as plain
		''' text.
		''' </summary>
		''' <param name="out"> the output stream </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Public Overridable Sub write(ByVal out As Writer)
			Dim doc As Document = document
			Try
				uI.getEditorKit(Me).write(out, doc, 0, doc.length)
			Catch e As BadLocationException
				Throw New IOException(e.Message)
			End Try
		End Sub

		Public Overrides Sub removeNotify()
			MyBase.removeNotify()
			If focusedComponent Is Me Then sun.awt.AppContext.appContext.remove(FOCUSED_COMPONENT)
		End Sub

		' --- java.awt.TextComponent methods ------------------------

		''' <summary>
		''' Sets the position of the text insertion caret for the
		''' <code>TextComponent</code>.  Note that the caret tracks change,
		''' so this may move if the underlying text of the component is changed.
		''' If the document is <code>null</code>, does nothing. The position
		''' must be between 0 and the length of the component's text or else
		''' an exception is thrown.
		''' </summary>
		''' <param name="position"> the position </param>
		''' <exception cref="IllegalArgumentException"> if the value supplied
		'''               for <code>position</code> is less than zero or greater
		'''               than the component's text length
		''' @beaninfo
		''' description: the caret position </exception>
		Public Overridable Property caretPosition As Integer
			Set(ByVal position As Integer)
				Dim doc As Document = document
				If doc IsNot Nothing Then
					If position > doc.length OrElse position < 0 Then Throw New System.ArgumentException("bad position: " & position)
					caret.dot = position
				End If
			End Set
			Get
				Return caret.dot
			End Get
		End Property

		''' <summary>
		''' Returns the position of the text insertion caret for the
		''' text component.
		''' </summary>
		''' <returns> the position of the text insertion caret for the
		'''  text component &ge; 0 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Sets the text of this <code>TextComponent</code>
		''' to the specified text.  If the text is <code>null</code>
		''' or empty, has the effect of simply deleting the old text.
		''' When text has been inserted, the resulting caret location
		''' is determined by the implementation of the caret class.
		''' 
		''' <p>
		''' Note that text is not a bound property, so no <code>PropertyChangeEvent
		''' </code> is fired when it changes. To listen for changes to the text,
		''' use <code>DocumentListener</code>.
		''' </summary>
		''' <param name="t"> the new text to be set </param>
		''' <seealso cref= #getText </seealso>
		''' <seealso cref= DefaultCaret
		''' @beaninfo
		''' description: the text of this component </seealso>
		Public Overridable Property text As String
			Set(ByVal t As String)
				Try
					Dim doc As Document = document
					If TypeOf doc Is AbstractDocument Then
						CType(doc, AbstractDocument).replace(0, doc.length, t,Nothing)
					Else
						doc.remove(0, doc.length)
						doc.insertString(0, t, Nothing)
					End If
				Catch e As BadLocationException
					UIManager.lookAndFeel.provideErrorFeedback(JTextComponent.this)
				End Try
			End Set
			Get
				Dim doc As Document = document
				Dim txt As String
				Try
					txt = doc.getText(0, doc.length)
				Catch e As BadLocationException
					txt = Nothing
				End Try
				Return txt
			End Get
		End Property


		''' <summary>
		''' Returns the selected text contained in this
		''' <code>TextComponent</code>.  If the selection is
		''' <code>null</code> or the document empty, returns <code>null</code>.
		''' </summary>
		''' <returns> the text </returns>
		''' <exception cref="IllegalArgumentException"> if the selection doesn't
		'''  have a valid mapping into the document for some reason </exception>
		''' <seealso cref= #setText </seealso>
		Public Overridable Property selectedText As String
			Get
				Dim txt As String = Nothing
				Dim p0 As Integer = Math.Min(caret.dot, caret.mark)
				Dim p1 As Integer = Math.Max(caret.dot, caret.mark)
				If p0 <> p1 Then
					Try
						Dim doc As Document = document
						txt = doc.getText(p0, p1 - p0)
					Catch e As BadLocationException
						Throw New System.ArgumentException(e.Message)
					End Try
				End If
				Return txt
			End Get
		End Property

		''' <summary>
		''' Returns the boolean indicating whether this
		''' <code>TextComponent</code> is editable or not.
		''' </summary>
		''' <returns> the boolean value </returns>
		''' <seealso cref= #setEditable </seealso>
		Public Overridable Property editable As Boolean
			Get
				Return editable
			End Get
			Set(ByVal b As Boolean)
				If b <> editable Then
					Dim oldVal As Boolean = editable
					editable = b
					enableInputMethods(editable)
					firePropertyChange("editable", Convert.ToBoolean(oldVal), Convert.ToBoolean(editable))
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the selected text's start position.  Return 0 for an
		''' empty document, or the value of dot if no selection.
		''' </summary>
		''' <returns> the start position &ge; 0 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property selectionStart As Integer
			Get
				Dim start As Integer = Math.Min(caret.dot, caret.mark)
				Return start
			End Get
			Set(ByVal selectionStart As Integer)
		'         Route through select method to enforce consistent policy
		'         * between selectionStart and selectionEnd.
		'         
				[select](selectionStart, selectionEnd)
			End Set
		End Property


		''' <summary>
		''' Returns the selected text's end position.  Return 0 if the document
		''' is empty, or the value of dot if there is no selection.
		''' </summary>
		''' <returns> the end position &ge; 0 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property selectionEnd As Integer
			Get
				Dim [end] As Integer = Math.Max(caret.dot, caret.mark)
				Return [end]
			End Get
			Set(ByVal selectionEnd As Integer)
		'         Route through select method to enforce consistent policy
		'         * between selectionStart and selectionEnd.
		'         
				[select](selectionStart, selectionEnd)
			End Set
		End Property


		''' <summary>
		''' Selects the text between the specified start and end positions.
		''' <p>
		''' This method sets the start and end positions of the
		''' selected text, enforcing the restriction that the start position
		''' must be greater than or equal to zero.  The end position must be
		''' greater than or equal to the start position, and less than or
		''' equal to the length of the text component's text.
		''' <p>
		''' If the caller supplies values that are inconsistent or out of
		''' bounds, the method enforces these constraints silently, and
		''' without failure. Specifically, if the start position or end
		''' position is greater than the length of the text, it is reset to
		''' equal the text length. If the start position is less than zero,
		''' it is reset to zero, and if the end position is less than the
		''' start position, it is reset to the start position.
		''' <p>
		''' This call is provided for backward compatibility.
		''' It is routed to a call to <code>setCaretPosition</code>
		''' followed by a call to <code>moveCaretPosition</code>.
		''' The preferred way to manage selection is by calling
		''' those methods directly.
		''' </summary>
		''' <param name="selectionStart"> the start position of the text </param>
		''' <param name="selectionEnd"> the end position of the text </param>
		''' <seealso cref= #setCaretPosition </seealso>
		''' <seealso cref= #moveCaretPosition </seealso>
		Public Overridable Sub [select](ByVal selectionStart As Integer, ByVal selectionEnd As Integer)
			' argument adjustment done by java.awt.TextComponent
			Dim docLength As Integer = document.length

			If selectionStart < 0 Then selectionStart = 0
			If selectionStart > docLength Then selectionStart = docLength
			If selectionEnd > docLength Then selectionEnd = docLength
			If selectionEnd < selectionStart Then selectionEnd = selectionStart

			caretPosition = selectionStart
			moveCaretPosition(selectionEnd)
		End Sub

		''' <summary>
		''' Selects all the text in the <code>TextComponent</code>.
		''' Does nothing on a <code>null</code> or empty document.
		''' </summary>
		Public Overridable Sub selectAll()
			Dim doc As Document = document
			If doc IsNot Nothing Then
				caretPosition = 0
				moveCaretPosition(doc.length)
			End If
		End Sub

		' --- Tooltip Methods ---------------------------------------------

		''' <summary>
		''' Returns the string to be used as the tooltip for <code>event</code>.
		''' This will return one of:
		''' <ol>
		'''  <li>If <code>setToolTipText</code> has been invoked with a
		'''      non-<code>null</code>
		'''      value, it will be returned, otherwise
		'''  <li>The value from invoking <code>getToolTipText</code> on
		'''      the UI will be returned.
		''' </ol>
		''' By default <code>JTextComponent</code> does not register
		''' itself with the <code>ToolTipManager</code>.
		''' This means that tooltips will NOT be shown from the
		''' <code>TextUI</code> unless <code>registerComponent</code> has
		''' been invoked on the <code>ToolTipManager</code>.
		''' </summary>
		''' <param name="event"> the event in question </param>
		''' <returns> the string to be used as the tooltip for <code>event</code> </returns>
		''' <seealso cref= javax.swing.JComponent#setToolTipText </seealso>
		''' <seealso cref= javax.swing.plaf.TextUI#getToolTipText </seealso>
		''' <seealso cref= javax.swing.ToolTipManager#registerComponent </seealso>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			Dim retValue As String = MyBase.getToolTipText([event])

			If retValue Is Nothing Then
				Dim ___ui As TextUI = uI
				If ___ui IsNot Nothing Then retValue = ___ui.getToolTipText(Me, New Point([event].x, [event].y))
			End If
			Return retValue
		End Function

		' --- Scrollable methods ---------------------------------------------

		''' <summary>
		''' Returns the preferred size of the viewport for a view component.
		''' This is implemented to do the default behavior of returning
		''' the preferred size of the component.
		''' </summary>
		''' <returns> the <code>preferredSize</code> of a <code>JViewport</code>
		''' whose view is this <code>Scrollable</code> </returns>
		Public Overridable Property preferredScrollableViewportSize As Dimension
			Get
				Return preferredSize
			End Get
		End Property


		''' <summary>
		''' Components that display logical rows or columns should compute
		''' the scroll increment that will completely expose one new row
		''' or column, depending on the value of orientation.  Ideally,
		''' components should handle a partially exposed row or column by
		''' returning the distance required to completely expose the item.
		''' <p>
		''' The default implementation of this is to simply return 10% of
		''' the visible area.  Subclasses are likely to be able to provide
		''' a much more reasonable value.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> either <code>SwingConstants.VERTICAL</code> or
		'''   <code>SwingConstants.HORIZONTAL</code> </param>
		''' <param name="direction"> less than zero to scroll up/left, greater than
		'''   zero for down/right </param>
		''' <returns> the "unit" increment for scrolling in the specified direction </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid orientation </exception>
		''' <seealso cref= JScrollBar#setUnitIncrement </seealso>
		Public Overridable Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			Select Case orientation
			Case SwingConstants.VERTICAL
				Return visibleRect.height / 10
			Case SwingConstants.HORIZONTAL
				Return visibleRect.width / 10
			Case Else
				Throw New System.ArgumentException("Invalid orientation: " & orientation)
			End Select
		End Function


		''' <summary>
		''' Components that display logical rows or columns should compute
		''' the scroll increment that will completely expose one block
		''' of rows or columns, depending on the value of orientation.
		''' <p>
		''' The default implementation of this is to simply return the visible
		''' area.  Subclasses will likely be able to provide a much more
		''' reasonable value.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> either <code>SwingConstants.VERTICAL</code> or
		'''   <code>SwingConstants.HORIZONTAL</code> </param>
		''' <param name="direction"> less than zero to scroll up/left, greater than zero
		'''  for down/right </param>
		''' <returns> the "block" increment for scrolling in the specified direction </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid orientation </exception>
		''' <seealso cref= JScrollBar#setBlockIncrement </seealso>
		Public Overridable Function getScrollableBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			Select Case orientation
			Case SwingConstants.VERTICAL
				Return visibleRect.height
			Case SwingConstants.HORIZONTAL
				Return visibleRect.width
			Case Else
				Throw New System.ArgumentException("Invalid orientation: " & orientation)
			End Select
		End Function


		''' <summary>
		''' Returns true if a viewport should always force the width of this
		''' <code>Scrollable</code> to match the width of the viewport.
		''' For example a normal text view that supported line wrapping
		''' would return true here, since it would be undesirable for
		''' wrapped lines to disappear beyond the right
		''' edge of the viewport.  Note that returning true for a
		''' <code>Scrollable</code> whose ancestor is a <code>JScrollPane</code>
		''' effectively disables horizontal scrolling.
		''' <p>
		''' Scrolling containers, like <code>JViewport</code>,
		''' will use this method each time they are validated.
		''' </summary>
		''' <returns> true if a viewport should force the <code>Scrollable</code>s
		'''   width to match its own </returns>
		Public Overridable Property scrollableTracksViewportWidth As Boolean Implements Scrollable.getScrollableTracksViewportWidth
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.width > preferredSize.width
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns true if a viewport should always force the height of this
		''' <code>Scrollable</code> to match the height of the viewport.
		''' For example a columnar text view that flowed text in left to
		''' right columns could effectively disable vertical scrolling by
		''' returning true here.
		''' <p>
		''' Scrolling containers, like <code>JViewport</code>,
		''' will use this method each time they are validated.
		''' </summary>
		''' <returns> true if a viewport should force the Scrollables height
		'''   to match its own </returns>
		Public Overridable Property scrollableTracksViewportHeight As Boolean Implements Scrollable.getScrollableTracksViewportHeight
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				If TypeOf parent Is JViewport Then Return parent.height > preferredSize.height
				Return False
			End Get
		End Property


	'////////////////
	' Printing Support
	'////////////////

		''' <summary>
		''' A convenience print method that displays a print dialog, and then
		''' prints this {@code JTextComponent} in <i>interactive</i> mode with no
		''' header or footer text. Note: this method
		''' blocks until printing is done.
		''' <p>
		''' Note: In <i>headless</i> mode, no dialogs will be shown.
		''' 
		''' <p> This method calls the full featured
		''' {@link #print(MessageFormat, MessageFormat, boolean, PrintService, PrintRequestAttributeSet, boolean)
		''' print} method to perform printing. </summary>
		''' <returns> {@code true}, unless printing is canceled by the user </returns>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''         to be aborted </exception>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request
		''' </exception>
		''' <seealso cref= #print(MessageFormat, MessageFormat, boolean, PrintService, PrintRequestAttributeSet, boolean)
		''' 
		''' @since 1.6 </seealso>

		Public Overridable Function print() As Boolean
			Return print(Nothing, Nothing, True, Nothing, Nothing, True)
		End Function

		''' <summary>
		''' A convenience print method that displays a print dialog, and then
		''' prints this {@code JTextComponent} in <i>interactive</i> mode with
		''' the specified header and footer text. Note: this method
		''' blocks until printing is done.
		''' <p>
		''' Note: In <i>headless</i> mode, no dialogs will be shown.
		''' 
		''' <p> This method calls the full featured
		''' {@link #print(MessageFormat, MessageFormat, boolean, PrintService, PrintRequestAttributeSet, boolean)
		''' print} method to perform printing. </summary>
		''' <param name="headerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the header, or {@code null} for no header </param>
		''' <param name="footerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the footer, or {@code null} for no footer </param>
		''' <returns> {@code true}, unless printing is canceled by the user </returns>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''         to be aborted </exception>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request
		''' </exception>
		''' <seealso cref= #print(MessageFormat, MessageFormat, boolean, PrintService, PrintRequestAttributeSet, boolean) </seealso>
		''' <seealso cref= java.text.MessageFormat
		''' @since 1.6 </seealso>
		Public Overridable Function print(ByVal headerFormat As MessageFormat, ByVal footerFormat As MessageFormat) As Boolean
			Return print(headerFormat, footerFormat, True, Nothing, Nothing, True)
		End Function

		''' <summary>
		''' Prints the content of this {@code JTextComponent}. Note: this method
		''' blocks until printing is done.
		''' 
		''' <p>
		''' Page header and footer text can be added to the output by providing
		''' {@code MessageFormat} arguments. The printing code requests
		''' {@code Strings} from the formats, providing a single item which may be
		''' included in the formatted string: an {@code Integer} representing the
		''' current page number.
		''' 
		''' <p>
		''' {@code showPrintDialog boolean} parameter allows you to specify whether
		''' a print dialog is displayed to the user. When it is, the user
		''' may use the dialog to change printing attributes or even cancel the
		''' print.
		''' 
		''' <p>
		''' {@code service} allows you to provide the initial
		''' {@code PrintService} for the print dialog, or to specify
		''' {@code PrintService} to print to when the dialog is not shown.
		''' 
		''' <p>
		''' {@code attributes} can be used to provide the
		''' initial values for the print dialog, or to supply any needed
		''' attributes when the dialog is not shown. {@code attributes} can
		''' be used to control how the job will print, for example
		''' <i>duplex</i> or <i>single-sided</i>.
		''' 
		''' <p>
		''' {@code interactive boolean} parameter allows you to specify
		''' whether to perform printing in <i>interactive</i>
		''' mode. If {@code true}, a progress dialog, with an abort option,
		''' is displayed for the duration of printing.  This dialog is
		''' <i>modal</i> when {@code print} is invoked on the <i>Event Dispatch
		''' Thread</i> and <i>non-modal</i> otherwise. <b>Warning</b>:
		''' calling this method on the <i>Event Dispatch Thread</i> with {@code
		''' interactive false} blocks <i>all</i> events, including repaints, from
		''' being processed until printing is complete. It is only
		''' recommended when printing from an application with no
		''' visible GUI.
		''' 
		''' <p>
		''' Note: In <i>headless</i> mode, {@code showPrintDialog} and
		''' {@code interactive} parameters are ignored and no dialogs are
		''' shown.
		''' 
		''' <p>
		''' This method ensures the {@code document} is not mutated during printing.
		''' To indicate it visually, {@code setEnabled(false)} is set for the
		''' duration of printing.
		''' 
		''' <p>
		''' This method uses <seealso cref="#getPrintable"/> to render document content.
		''' 
		''' <p>
		''' This method is thread-safe, although most Swing methods are not. Please
		''' see <A
		''' HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">
		''' Concurrency in Swing</A> for more information.
		''' 
		''' <p>
		''' <b>Sample Usage</b>. This code snippet shows a cross-platform print
		''' dialog and then prints the {@code JTextComponent} in <i>interactive</i> mode
		''' unless the user cancels the dialog:
		''' 
		''' <pre>
		''' textComponent.print(new MessageFormat(&quot;My text component header&quot;),
		'''     new MessageFormat(&quot;Footer. Page - {0}&quot;), true, null, null, true);
		''' </pre>
		''' <p>
		''' Executing this code off the <i>Event Dispatch Thread</i>
		''' performs printing on the <i>background</i>.
		''' The following pattern might be used for <i>background</i>
		''' printing:
		''' <pre>
		'''     FutureTask&lt;Boolean&gt; future =
		'''         new FutureTask&lt;Boolean&gt;(
		'''             new Callable&lt;Boolean&gt;() {
		'''                 public Boolean call() {
		'''                     return textComponent.print(.....);
		'''                 }
		'''             });
		'''     executor.execute(future);
		''' </pre>
		''' </summary>
		''' <param name="headerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the header, or {@code null} for no header </param>
		''' <param name="footerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the footer, or {@code null} for no footer </param>
		''' <param name="showPrintDialog"> {@code true} to display a print dialog,
		'''        {@code false} otherwise </param>
		''' <param name="service"> initial {@code PrintService}, or {@code null} for the
		'''        default </param>
		''' <param name="attributes"> the job attributes to be applied to the print job, or
		'''        {@code null} for none </param>
		''' <param name="interactive"> whether to print in an interactive mode </param>
		''' <returns> {@code true}, unless printing is canceled by the user </returns>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''         to be aborted </exception>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request
		''' </exception>
		''' <seealso cref= #getPrintable </seealso>
		''' <seealso cref= java.text.MessageFormat </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= java.util.concurrent.FutureTask
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Function print(ByVal headerFormat As MessageFormat, ByVal footerFormat As MessageFormat, ByVal showPrintDialog As Boolean, ByVal service As javax.print.PrintService, ByVal attributes As javax.print.attribute.PrintRequestAttributeSet, ByVal interactive As Boolean) As Boolean

			Dim job As PrinterJob = PrinterJob.printerJob
			Dim ___printable As java.awt.print.Printable
			Dim printingStatus As sun.swing.PrintingStatus
			Dim isHeadless As Boolean = GraphicsEnvironment.headless
			Dim isEventDispatchThread As Boolean = SwingUtilities.eventDispatchThread
			Dim textPrintable As java.awt.print.Printable = getPrintable(headerFormat, footerFormat)
			If interactive AndAlso (Not isHeadless) Then
				printingStatus = sun.swing.PrintingStatus.createPrintingStatus(Me, job)
				___printable = printingStatus.createNotificationPrintable(textPrintable)
			Else
				printingStatus = Nothing
				___printable = textPrintable
			End If

			If service IsNot Nothing Then job.printService = service

			job.printable = ___printable

			Dim attr As javax.print.attribute.PrintRequestAttributeSet = If(attributes Is Nothing, New HashPrintRequestAttributeSet, attributes)

			If showPrintDialog AndAlso (Not isHeadless) AndAlso (Not job.printDialog(attr)) Then Return False

	'        
	'         * there are three cases for printing:
	'         * 1. print non interactively (! interactive || isHeadless)
	'         * 2. print interactively off EDT
	'         * 3. print interactively on EDT
	'         *
	'         * 1 and 2 prints on the current thread (3 prints on another thread)
	'         * 2 and 3 deal with PrintingStatusDialog
	'         
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			final Callable<Object> doPrint = New Callable<Object>()
	'		{
	'				public Object call() throws Exception
	'				{
	'					try
	'					{
	'						job.print(attr);
	'					}
	'					finally
	'					{
	'						if (printingStatus != Nothing)
	'						{
	'							printingStatus.dispose();
	'						}
	'					}
	'					Return Nothing;
	'				}
	'			};

			Dim futurePrinting As New FutureTask(Of Object)(doPrint)

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			final Runnable runnablePrinting = New Runnable()
	'		{
	'				public void run()
	'				{
	'					'disable component
	'					boolean wasEnabled = False;
	'					if (isEventDispatchThread)
	'					{
	'						if (isEnabled())
	'						{
	'							wasEnabled = True;
	'							setEnabled(False);
	'						}
	'					}
	'					else
	'					{
	'						try
	'						{
	'							wasEnabled = SwingUtilities2.submit(New Callable<java.lang.Boolean>()
	'							{
	'									public java.lang.Boolean call() throws Exception
	'									{
	'										boolean rv = isEnabled();
	'										if (rv)
	'										{
	'											setEnabled(False);
	'										}
	'										Return rv;
	'									}
	'								}).get();
	'						}
	'						catch (InterruptedException e)
	'						{
	'							throw New RuntimeException(e);
	'						}
	'						catch (ExecutionException e)
	'						{
	'							Throwable cause = e.getCause();
	'							if (cause instanceof Error)
	'							{
	'								throw (Error) cause;
	'							}
	'							if (cause instanceof RuntimeException)
	'							{
	'								throw (RuntimeException) cause;
	'							}
	'							throw New AssertionError(cause);
	'						}
	'					}
	'
	'					getDocument().render(futurePrinting);
	'
	'					'enable component
	'					if (wasEnabled)
	'					{
	'						if (isEventDispatchThread)
	'						{
	'							setEnabled(True);
	'						}
	'						else
	'						{
	'							try
	'							{
	'								SwingUtilities2.submit(New Runnable()
	'								{
	'										public void run()
	'										{
	'											setEnabled(True);
	'										}
	'									}, Nothing).get();
	'							}
	'							catch (InterruptedException e)
	'							{
	'								throw New RuntimeException(e);
	'							}
	'							catch (ExecutionException e)
	'							{
	'								Throwable cause = e.getCause();
	'								if (cause instanceof Error)
	'								{
	'									throw (Error) cause;
	'								}
	'								if (cause instanceof RuntimeException)
	'								{
	'									throw (RuntimeException) cause;
	'								}
	'								throw New AssertionError(cause);
	'							}
	'						}
	'					}
	'				}
	'			};

			If (Not interactive) OrElse isHeadless Then
				runnablePrinting.run()
			Else
				If isEventDispatchThread Then
					CType(New Thread(runnablePrinting), Thread).Start()
					printingStatus.showModal(True)
				Else
					printingStatus.showModal(False)
					runnablePrinting.run()
				End If
			End If

			'the printing is done successfully or otherwise.
			'dialog is hidden if needed.
			Try
				futurePrinting.get()
			Catch e As InterruptedException
				Throw New Exception(e)
			Catch e As ExecutionException
				Dim cause As Exception = e.InnerException
				If TypeOf cause Is PrinterAbortException Then
					If printingStatus IsNot Nothing AndAlso printingStatus.aborted Then
						Return False
					Else
						Throw CType(cause, PrinterAbortException)
					End If
				ElseIf TypeOf cause Is java.awt.print.PrinterException Then
					Throw CType(cause, java.awt.print.PrinterException)
				ElseIf TypeOf cause Is Exception Then
					Throw CType(cause, Exception)
				ElseIf TypeOf cause Is Exception Then
					Throw CType(cause, [Error])
				Else
					Throw New AssertionError(cause)
				End If
			End Try
			Return True
		End Function


		''' <summary>
		''' Returns a {@code Printable} to use for printing the content of this
		''' {@code JTextComponent}. The returned {@code Printable} prints
		''' the document as it looks on the screen except being reformatted
		''' to fit the paper.
		''' The returned {@code Printable} can be wrapped inside another
		''' {@code Printable} in order to create complex reports and
		''' documents.
		''' 
		''' 
		''' <p>
		''' The returned {@code Printable} shares the {@code document} with this
		''' {@code JTextComponent}. It is the responsibility of the developer to
		''' ensure that the {@code document} is not mutated while this {@code Printable}
		''' is used. Printing behavior is undefined when the {@code document} is
		''' mutated during printing.
		''' 
		''' <p>
		''' Page header and footer text can be added to the output by providing
		''' {@code MessageFormat} arguments. The printing code requests
		''' {@code Strings} from the formats, providing a single item which may be
		''' included in the formatted string: an {@code Integer} representing the
		''' current page number.
		''' 
		''' <p>
		''' The returned {@code Printable} when printed, formats the
		''' document content appropriately for the page size. For correct
		''' line wrapping the {@code imageable width} of all pages must be the
		''' same. See <seealso cref="java.awt.print.PageFormat#getImageableWidth"/>.
		''' 
		''' <p>
		''' This method is thread-safe, although most Swing methods are not. Please
		''' see <A
		''' HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">
		''' Concurrency in Swing</A> for more information.
		''' 
		''' <p>
		''' The returned {@code Printable} can be printed on any thread.
		''' 
		''' <p>
		''' This implementation returned {@code Printable} performs all painting on
		''' the <i>Event Dispatch Thread</i>, regardless of what thread it is
		''' used on.
		''' </summary>
		''' <param name="headerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the header, or {@code null} for no header </param>
		''' <param name="footerFormat"> the text, in {@code MessageFormat}, to be
		'''        used as the footer, or {@code null} for no footer </param>
		''' <returns> a {@code Printable} for use in printing content of this
		'''         {@code JTextComponent}
		''' 
		''' </returns>
		''' <seealso cref= java.awt.print.Printable </seealso>
		''' <seealso cref= java.awt.print.PageFormat </seealso>
		''' <seealso cref= javax.swing.text.Document#render(java.lang.Runnable)
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Function getPrintable(ByVal headerFormat As MessageFormat, ByVal footerFormat As MessageFormat) As java.awt.print.Printable
			Return sun.swing.text.TextComponentPrintable.getPrintable(Me, headerFormat, footerFormat)
		End Function


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JTextComponent</code>. For text components,
		''' the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJTextComponent</code>.
		''' A new <code>AccessibleJTextComponent</code> instance
		''' is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJTextComponent</code> that serves as the
		'''         <code>AccessibleContext</code> of this
		'''         <code>JTextComponent</code> </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTextComponent(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTextComponent</code> class.  It provides an implementation of
		''' the Java Accessibility API appropriate to menu user-interface elements.
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
		Public Class AccessibleJTextComponent
			Inherits AccessibleJComponent
			Implements AccessibleText, CaretListener, DocumentListener, AccessibleAction, AccessibleEditableText, AccessibleExtendedText

			Private ReadOnly outerInstance As JTextComponent


			Friend caretPos As Integer
			Friend oldLocationOnScreen As Point

			''' <summary>
			''' Constructs an AccessibleJTextComponent.  Adds a listener to track
			''' caret change.
			''' </summary>
			Public Sub New(ByVal outerInstance As JTextComponent)
					Me.outerInstance = outerInstance
				Dim doc As Document = outerInstance.document
				If doc IsNot Nothing Then doc.addDocumentListener(Me)
				outerInstance.addCaretListener(Me)
				caretPos = caretPosition

				Try
					oldLocationOnScreen = locationOnScreen
				Catch iae As IllegalComponentStateException
				End Try

				' Fire a ACCESSIBLE_VISIBLE_DATA_PROPERTY PropertyChangeEvent
				' when the text component moves (e.g., when scrolling).
				' Using an anonymous class since making AccessibleJTextComponent
				' implement ComponentListener would be an API change.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				outerInstance.addComponentListener(New ComponentAdapter()
	'			{
	'
	'				public void componentMoved(ComponentEvent e)
	'				{
	'					try
	'					{
	'						Point newLocationOnScreen = getLocationOnScreen();
	'						firePropertyChange(ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldLocationOnScreen, newLocationOnScreen);
	'
	'						oldLocationOnScreen = newLocationOnScreen;
	'					}
	'					catch (IllegalComponentStateException iae)
	'					{
	'					}
	'				}
	'			});
			End Sub

			''' <summary>
			''' Handles caret updates (fire appropriate property change event,
			''' which are AccessibleContext.ACCESSIBLE_CARET_PROPERTY and
			''' AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY).
			''' This keeps track of the dot position internally.  When the caret
			''' moves, the internal position is updated after firing the event.
			''' </summary>
			''' <param name="e"> the CaretEvent </param>
			Public Overridable Sub caretUpdate(ByVal e As CaretEvent) Implements CaretListener.caretUpdate
				Dim dot As Integer = e.dot
				Dim mark As Integer = e.mark
				If caretPos <> dot Then
					' the caret moved
					outerInstance.firePropertyChange(ACCESSIBLE_CARET_PROPERTY, New Integer?(caretPos), New Integer?(dot))
					caretPos = dot

					Try
						oldLocationOnScreen = locationOnScreen
					Catch iae As IllegalComponentStateException
					End Try
				End If
				If mark <> dot Then outerInstance.firePropertyChange(ACCESSIBLE_SELECTION_PROPERTY, Nothing, selectedText)
			End Sub

			' DocumentListener methods

			''' <summary>
			''' Handles document insert (fire appropriate property change event
			''' which is AccessibleContext.ACCESSIBLE_TEXT_PROPERTY).
			''' This tracks the changed offset via the event.
			''' </summary>
			''' <param name="e"> the DocumentEvent </param>
			Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
				Dim pos As Integer? = New Integer?(e.offset)
				If SwingUtilities.eventDispatchThread Then
					outerInstance.firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos)
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Runnable doFire = New Runnable()
	'				{
	'					public void run()
	'					{
	'						firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos);
	'					}
	'				};
					SwingUtilities.invokeLater(doFire)
				End If
			End Sub

			''' <summary>
			''' Handles document remove (fire appropriate property change event,
			''' which is AccessibleContext.ACCESSIBLE_TEXT_PROPERTY).
			''' This tracks the changed offset via the event.
			''' </summary>
			''' <param name="e"> the DocumentEvent </param>
			Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
				Dim pos As Integer? = New Integer?(e.offset)
				If SwingUtilities.eventDispatchThread Then
					outerInstance.firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos)
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Runnable doFire = New Runnable()
	'				{
	'					public void run()
	'					{
	'						firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos);
	'					}
	'				};
					SwingUtilities.invokeLater(doFire)
				End If
			End Sub

			''' <summary>
			''' Handles document remove (fire appropriate property change event,
			''' which is AccessibleContext.ACCESSIBLE_TEXT_PROPERTY).
			''' This tracks the changed offset via the event.
			''' </summary>
			''' <param name="e"> the DocumentEvent </param>
			Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
				Dim pos As Integer? = New Integer?(e.offset)
				If SwingUtilities.eventDispatchThread Then
					outerInstance.firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos)
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Runnable doFire = New Runnable()
	'				{
	'					public void run()
	'					{
	'						firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, pos);
	'					}
	'				};
					SwingUtilities.invokeLater(doFire)
				End If
			End Sub

			''' <summary>
			''' Gets the state set of the JTextComponent.
			''' The AccessibleStateSet of an object is composed of a set of
			''' unique AccessibleState's.  A change in the AccessibleStateSet
			''' of an object will cause a PropertyChangeEvent to be fired
			''' for the AccessibleContext.ACCESSIBLE_STATE_PROPERTY property.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the
			''' current state set of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState </seealso>
			''' <seealso cref= #addPropertyChangeListener </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.editable Then states.add(AccessibleState.EDITABLE)
					Return states
				End Get
			End Property


			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object (AccessibleRole.TEXT) </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TEXT
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleText associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleText interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleText As AccessibleText
				Get
					Return Me
				End Get
			End Property


			' --- interface AccessibleText methods ------------------------

			''' <summary>
			''' Many of these methods are just convenience methods; they
			''' just call the equivalent on the parent
			''' </summary>

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p. </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer Implements AccessibleText.getIndexAtPoint
				If p Is Nothing Then Return -1
				Return outerInstance.viewToModel(p)
			End Function

				''' <summary>
				''' Gets the editor's drawing rectangle.  Stolen
				''' from the unfortunately named
				''' BasicTextUI.getVisibleEditorRect()
				''' </summary>
				''' <returns> the bounding box for the root view </returns>
				Friend Overridable Property rootEditorRect As Rectangle
					Get
						Dim alloc As Rectangle = outerInstance.bounds
						If (alloc.width > 0) AndAlso (alloc.height > 0) Then
									alloc.y = 0
									alloc.x = alloc.y
								Dim insets As Insets = outerInstance.insets
								alloc.x += insets.left
								alloc.y += insets.top
								alloc.width -= insets.left + insets.right
								alloc.height -= insets.top + insets.bottom
								Return alloc
						End If
						Return Nothing
					End Get
				End Property

			''' <summary>
			''' Determines the bounding box of the character at the given
			''' index into the string.  The bounds are returned in local
			''' coordinates.  If the index is invalid a null rectangle
			''' is returned.
			''' 
			''' The screen coordinates returned are "unscrolled coordinates"
			''' if the JTextComponent is contained in a JScrollPane in which
			''' case the resulting rectangle should be composed with the parent
			''' coordinates.  A good algorithm to use is:
			''' <pre>
			''' Accessible a:
			''' AccessibleText at = a.getAccessibleText();
			''' AccessibleComponent ac = a.getAccessibleComponent();
			''' Rectangle r = at.getCharacterBounds();
			''' Point p = ac.getLocation();
			''' r.x += p.x;
			''' r.y += p.y;
			''' </pre>
			''' 
			''' Note: the JTextComponent must have a valid size (e.g. have
			''' been added to a parent container whose ancestor container
			''' is a valid top-level window) for this method to be able
			''' to return a meaningful (non-null) value.
			''' </summary>
			''' <param name="i"> the index into the String &ge; 0 </param>
			''' <returns> the screen coordinates of the character's bounding box </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle Implements AccessibleText.getCharacterBounds
				If i < 0 OrElse i > outerInstance.model.length-1 Then Return Nothing
				Dim ui As TextUI = outerInstance.uI
				If ui Is Nothing Then Return Nothing
				Dim rect As Rectangle = Nothing
				Dim alloc As Rectangle = rootEditorRect
				If alloc Is Nothing Then Return Nothing
				If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
				Try
					Dim rootView As View = ui.getRootView(JTextComponent.this)
					If rootView IsNot Nothing Then
						rootView.sizeize(alloc.width, alloc.height)

						Dim bounds As Shape = rootView.modelToView(i, Position.Bias.Forward, i+1, Position.Bias.Backward, alloc)

						rect = If(TypeOf bounds Is Rectangle, CType(bounds, Rectangle), bounds.bounds)

					End If
				Catch e As BadLocationException
				Finally
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
				End Try
				Return rect
			End Function

			''' <summary>
			''' Returns the number of characters (valid indices)
			''' </summary>
			''' <returns> the number of characters &ge; 0 </returns>
			Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
				Get
					Return outerInstance.model.length
				End Get
			End Property

			''' <summary>
			''' Returns the zero-based offset of the caret.
			''' 
			''' Note: The character to the right of the caret will have the
			''' same index value as the offset (the caret is between
			''' two characters).
			''' </summary>
			''' <returns> the zero-based offset of the caret. </returns>
			Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
				Get
					Return outerInstance.caretPosition
				End Get
			End Property

			''' <summary>
			''' Returns the AttributeSet for a given character (at a given index).
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
				Dim e As Element = Nothing
				If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
				Try
					e = outerInstance.model.defaultRootElement
					Do While Not e.leaf
						Dim index As Integer = e.getElementIndex(i)
						e = e.getElement(index)
					Loop
				Finally
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
				End Try
				Return e.attributes
			End Function


			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' Return 0 if the text is empty, or the caret position
			''' if no selection.
			''' </summary>
			''' <returns> the index into the text of the start of the selection &ge; 0 </returns>
			Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
				Get
					Return outerInstance.selectionStart
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' Return 0 if the text is empty, or the caret position
			''' if no selection.
			''' </summary>
			''' <returns> the index into the text of the end of the selection &ge; 0 </returns>
			Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
				Get
					Return outerInstance.selectionEnd
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the text, null if no selection </returns>
			Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
				Get
					Return outerInstance.selectedText
				End Get
			End Property

		   ''' <summary>
		   ''' IndexedSegment extends Segment adding the offset into the
		   ''' the model the <code>Segment</code> was asked for.
		   ''' </summary>
			Private Class IndexedSegment
				Inherits Segment

				Private ReadOnly outerInstance As JTextComponent.AccessibleJTextComponent

				Public Sub New(ByVal outerInstance As JTextComponent.AccessibleJTextComponent)
					Me.outerInstance = outerInstance
				End Sub

				''' <summary>
				''' Offset into the model that the position represents.
				''' </summary>
				Public modelOffset As Integer
			End Class


			' TIGER - 4170173
			''' <summary>
			''' Returns the String at a given index. Whitespace
			''' between words is treated as a word.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence.
			'''  </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
				Return getAtIndex(part, index, 0)
			End Function


			''' <summary>
			''' Returns the String after a given index. Whitespace
			''' between words is treated as a word.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence. </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
				Return getAtIndex(part, index, 1)
			End Function


			''' <summary>
			''' Returns the String before a given index. Whitespace
			''' between words is treated a word.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence. </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
				Return getAtIndex(part, index, -1)
			End Function


			''' <summary>
			''' Gets the word, sentence, or character at <code>index</code>.
			''' If <code>direction</code> is non-null this will find the
			''' next/previous word/sentence/character.
			''' </summary>
			Private Function getAtIndex(ByVal part As Integer, ByVal index As Integer, ByVal direction As Integer) As String
				If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
				Try
					If index < 0 OrElse index >= outerInstance.model.length Then Return Nothing
					Select Case part
					Case AccessibleText.CHARACTER
						If index + direction < outerInstance.model.length AndAlso index + direction >= 0 Then Return outerInstance.model.getText(index + direction, 1)


					Case AccessibleText.WORD, AccessibleText.SENTENCE
						Dim seg As IndexedSegment = getSegmentAt(part, index)
						If seg IsNot Nothing Then
							If direction <> 0 Then
								Dim [next] As Integer


								If direction < 0 Then
									[next] = seg.modelOffset - 1
								Else
									[next] = seg.modelOffset + direction * seg.count
								End If
								If [next] >= 0 AndAlso [next] <= outerInstance.model.length Then
									seg = getSegmentAt(part, [next])
								Else
									seg = Nothing
								End If
							End If
							If seg IsNot Nothing Then Return New String(seg.array, seg.offset, seg.count)
						End If


					Case Else
					End Select
				Catch e As BadLocationException
				Finally
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
				End Try
				Return Nothing
			End Function


	'        
	'         * Returns the paragraph element for the specified index.
	'         
			Private Function getParagraphElement(ByVal index As Integer) As Element
				If TypeOf outerInstance.model Is PlainDocument Then
					Dim sdoc As PlainDocument = CType(outerInstance.model, PlainDocument)
					Return sdoc.getParagraphElement(index)
				ElseIf TypeOf outerInstance.model Is StyledDocument Then
					Dim sdoc As StyledDocument = CType(outerInstance.model, StyledDocument)
					Return sdoc.getParagraphElement(index)
				Else
					Dim para As Element
					para = outerInstance.model.defaultRootElement
					Do While Not para.leaf
						Dim pos As Integer = para.getElementIndex(index)
						para = para.getElement(pos)
					Loop
					If para Is Nothing Then Return Nothing
					Return para.parentElement
				End If
			End Function

	'        
	'         * Returns a <code>Segment</code> containing the paragraph text
	'         * at <code>index</code>, or null if <code>index</code> isn't
	'         * valid.
	'         
			Private Function getParagraphElementText(ByVal index As Integer) As IndexedSegment
				Dim para As Element = getParagraphElement(index)


				If para IsNot Nothing Then
					Dim segment As New IndexedSegment(Me)
					Try
						Dim length As Integer = para.endOffset - para.startOffset
						outerInstance.model.getText(para.startOffset, length, segment)
					Catch e As BadLocationException
						Return Nothing
					End Try
					segment.modelOffset = para.startOffset
					Return segment
				End If
				Return Nothing
			End Function


			''' <summary>
			''' Returns the Segment at <code>index</code> representing either
			''' the paragraph or sentence as identified by <code>part</code>, or
			''' null if a valid paragraph/sentence can't be found. The offset
			''' will point to the start of the word/sentence in the array, and
			''' the modelOffset will point to the location of the word/sentence
			''' in the model.
			''' </summary>
			Private Function getSegmentAt(ByVal part As Integer, ByVal index As Integer) As IndexedSegment
				Dim seg As IndexedSegment = getParagraphElementText(index)
				If seg Is Nothing Then Return Nothing
				Dim [iterator] As BreakIterator
				Select Case part
				Case AccessibleText.WORD
					[iterator] = BreakIterator.getWordInstance(locale)
				Case AccessibleText.SENTENCE
					[iterator] = BreakIterator.getSentenceInstance(locale)
				Case Else
					Return Nothing
				End Select
				seg.first()
				[iterator].text = seg
				Dim [end] As Integer = [iterator].following(index - seg.modelOffset + seg.offset)
				If [end] = BreakIterator.DONE Then Return Nothing
				If [end] > seg.offset + seg.count Then Return Nothing
				Dim begin As Integer = [iterator].previous()
				If begin = BreakIterator.DONE OrElse begin >= seg.offset + seg.count Then Return Nothing
				seg.modelOffset = seg.modelOffset + begin - seg.offset
				seg.offset = begin
				seg.count = [end] - begin
				Return seg
			End Function

			' begin AccessibleEditableText methods -----

			''' <summary>
			''' Returns the AccessibleEditableText interface for
			''' this text component.
			''' </summary>
			''' <returns> the AccessibleEditableText interface
			''' @since 1.4 </returns>
			Public Overridable Property accessibleEditableText As AccessibleEditableText
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Sets the text contents to the specified string.
			''' </summary>
			''' <param name="s"> the string to set the text contents
			''' @since 1.4 </param>
			Public Overridable Property textContents Implements AccessibleEditableText.setTextContents As String
				Set(ByVal s As String)
					outerInstance.text = s
				End Set
			End Property

			''' <summary>
			''' Inserts the specified string at the given index
			''' </summary>
			''' <param name="index"> the index in the text where the string will
			''' be inserted </param>
			''' <param name="s"> the string to insert in the text
			''' @since 1.4 </param>
			Public Overridable Sub insertTextAtIndex(ByVal index As Integer, ByVal s As String) Implements AccessibleEditableText.insertTextAtIndex
				Dim doc As Document = outerInstance.document
				If doc IsNot Nothing Then
					Try
						If s IsNot Nothing AndAlso s.Length > 0 Then
							Dim composedTextSaved As Boolean = outerInstance.saveComposedText(index)
							doc.insertString(index, s, Nothing)
							If composedTextSaved Then outerInstance.restoreComposedText()
						End If
					Catch e As BadLocationException
						UIManager.lookAndFeel.provideErrorFeedback(JTextComponent.this)
					End Try
				End If
			End Sub

			''' <summary>
			''' Returns the text string between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <returns> the text string between the indices
			''' @since 1.4 </returns>
			Public Overridable Function getTextRange(ByVal startIndex As Integer, ByVal endIndex As Integer) As String Implements AccessibleEditableText.getTextRange, AccessibleExtendedText.getTextRange
				Dim txt As String = Nothing
				Dim p0 As Integer = Math.Min(startIndex, endIndex)
				Dim p1 As Integer = Math.Max(startIndex, endIndex)
				If p0 <> p1 Then
					Try
						Dim doc As Document = outerInstance.document
						txt = doc.getText(p0, p1 - p0)
					Catch e As BadLocationException
						Throw New System.ArgumentException(e.Message)
					End Try
				End If
				Return txt
			End Function

			''' <summary>
			''' Deletes the text between two indices
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text
			''' @since 1.4 </param>
			Public Overridable Sub delete(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.delete
				If outerInstance.editable AndAlso enabled Then
					Try
						Dim p0 As Integer = Math.Min(startIndex, endIndex)
						Dim p1 As Integer = Math.Max(startIndex, endIndex)
						If p0 <> p1 Then
							Dim doc As Document = outerInstance.document
							doc.remove(p0, p1 - p0)
						End If
					Catch e As BadLocationException
					End Try
				Else
					UIManager.lookAndFeel.provideErrorFeedback(JTextComponent.this)
				End If
			End Sub

			''' <summary>
			''' Cuts the text between two indices into the system clipboard.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text
			''' @since 1.4 </param>
			Public Overridable Sub cut(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.cut
				selectText(startIndex, endIndex)
				outerInstance.cut()
			End Sub

			''' <summary>
			''' Pastes the text from the system clipboard into the text
			''' starting at the specified index.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text
			''' @since 1.4 </param>
			Public Overridable Sub paste(ByVal startIndex As Integer) Implements AccessibleEditableText.paste
				outerInstance.caretPosition = startIndex
				outerInstance.paste()
			End Sub

			''' <summary>
			''' Replaces the text between two indices with the specified
			''' string.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <param name="s"> the string to replace the text between two indices
			''' @since 1.4 </param>
			Public Overridable Sub replaceText(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal s As String) Implements AccessibleEditableText.replaceText
				selectText(startIndex, endIndex)
				outerInstance.replaceSelection(s)
			End Sub

			''' <summary>
			''' Selects the text between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text
			''' @since 1.4 </param>
			Public Overridable Sub selectText(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.selectText
				outerInstance.select(startIndex, endIndex)
			End Sub

			''' <summary>
			''' Sets attributes for the text between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <param name="as"> the attribute set </param>
			''' <seealso cref= AttributeSet
			''' @since 1.4 </seealso>
			Public Overridable Sub setAttributes(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal [as] As AttributeSet) Implements AccessibleEditableText.setAttributes

				' Fixes bug 4487492
				Dim doc As Document = outerInstance.document
				If doc IsNot Nothing AndAlso TypeOf doc Is StyledDocument Then
					Dim sDoc As StyledDocument = CType(doc, StyledDocument)
					Dim offset As Integer = startIndex
					Dim length As Integer = endIndex - startIndex
					sDoc.characterAttributestes(offset, length, [as], True)
				End If
			End Sub

			' ----- end AccessibleEditableText methods


			' ----- begin AccessibleExtendedText methods

	' Probably should replace the helper method getAtIndex() to return
	' instead an AccessibleTextSequence also for LINE & ATTRIBUTE_RUN
	' and then make the AccessibleText methods get[At|After|Before]Point
	' call this new method instead and return only the string portion

			''' <summary>
			''' Returns the AccessibleTextSequence at a given <code>index</code>.
			''' If <code>direction</code> is non-null this will find the
			''' next/previous word/sentence/character.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or
			''' <code>ATTRIBUTE_RUN</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <param name="direction"> is either -1, 0, or 1 </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text
			''' if <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned.
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Private Function getSequenceAtIndex(ByVal part As Integer, ByVal index As Integer, ByVal direction As Integer) As AccessibleTextSequence
				If index < 0 OrElse index >= outerInstance.model.length Then Return Nothing
				If direction < -1 OrElse direction > 1 Then Return Nothing ' direction must be 1, 0, or -1

				Select Case part
				Case AccessibleText.CHARACTER
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
					Dim charSequence As AccessibleTextSequence = Nothing
					Try
						If index + direction < outerInstance.model.length AndAlso index + direction >= 0 Then charSequence = New AccessibleTextSequence(index + direction, index + direction + 1, outerInstance.model.getText(index + direction, 1))

					Catch e As BadLocationException
						' we are intentionally silent; our contract says we return
						' null if there is any failure in this method
					Finally
						If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
					End Try
					Return charSequence

				Case AccessibleText.WORD, AccessibleText.SENTENCE
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
					Dim rangeSequence As AccessibleTextSequence = Nothing
					Try
						Dim seg As IndexedSegment = getSegmentAt(part, index)
						If seg IsNot Nothing Then
							If direction <> 0 Then
								Dim [next] As Integer

								If direction < 0 Then
									[next] = seg.modelOffset - 1
								Else
									[next] = seg.modelOffset + seg.count
								End If
								If [next] >= 0 AndAlso [next] <= outerInstance.model.length Then
									seg = getSegmentAt(part, [next])
								Else
									seg = Nothing
								End If
							End If
							If seg IsNot Nothing AndAlso (seg.offset + seg.count) <= outerInstance.model.length Then
								rangeSequence = New AccessibleTextSequence(seg.offset, seg.offset + seg.count, New String(seg.array, seg.offset, seg.count))
							End If ' else we leave rangeSequence set to null
						End If
					Catch e As BadLocationException
						' we are intentionally silent; our contract says we return
						' null if there is any failure in this method
					Finally
						If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
					End Try
					Return rangeSequence

				Case AccessibleExtendedText.LINE
					Dim lineSequence As AccessibleTextSequence = Nothing
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
					Try
						Dim startIndex As Integer = Utilities.getRowStart(JTextComponent.this, index)
						Dim endIndex As Integer = Utilities.getRowEnd(JTextComponent.this, index)
						If startIndex >= 0 AndAlso endIndex >= startIndex Then
							If direction = 0 Then
								lineSequence = New AccessibleTextSequence(startIndex, endIndex, outerInstance.model.getText(startIndex, endIndex - startIndex + 1))
							ElseIf direction = -1 AndAlso startIndex > 0 Then
								endIndex = Utilities.getRowEnd(JTextComponent.this, startIndex - 1)
								startIndex = Utilities.getRowStart(JTextComponent.this, startIndex - 1)
								If startIndex >= 0 AndAlso endIndex >= startIndex Then lineSequence = New AccessibleTextSequence(startIndex, endIndex, outerInstance.model.getText(startIndex, endIndex - startIndex + 1))
							ElseIf direction = 1 AndAlso endIndex < outerInstance.model.length Then
								startIndex = Utilities.getRowStart(JTextComponent.this, endIndex + 1)
								endIndex = Utilities.getRowEnd(JTextComponent.this, endIndex + 1)
								If startIndex >= 0 AndAlso endIndex >= startIndex Then lineSequence = New AccessibleTextSequence(startIndex, endIndex, outerInstance.model.getText(startIndex, endIndex - startIndex + 1))
							End If
							' already validated 'direction' above...
						End If
					Catch e As BadLocationException
						' we are intentionally silent; our contract says we return
						' null if there is any failure in this method
					Finally
						If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
					End Try
					Return lineSequence

				Case AccessibleExtendedText.ATTRIBUTE_RUN
					' assumptions: (1) that all characters in a single element
					' share the same attribute set; (2) that adjacent elements
					' *may* share the same attribute set

					Dim attributeRunStartIndex, attributeRunEndIndex As Integer
					Dim runText As String = Nothing
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()

					Try
							attributeRunEndIndex = Integer.MinValue
							attributeRunStartIndex = attributeRunEndIndex
						Dim tempIndex As Integer = index
						Select Case direction
						Case -1
							' going backwards, so find left edge of this run -
							' that'll be the end of the previous run
							' (off-by-one counting)
							attributeRunEndIndex = getRunEdge(index, direction)
							' now set ourselves up to find the left edge of the
							' prev. run
							tempIndex = attributeRunEndIndex - 1
						Case 1
							' going forward, so find right edge of this run -
							' that'll be the start of the next run
							' (off-by-one counting)
							attributeRunStartIndex = getRunEdge(index, direction)
							' now set ourselves up to find the right edge of the
							' next run
							tempIndex = attributeRunStartIndex
						Case 0
							' interested in the current run, so nothing special to
							' set up in advance...
						Case Else
							' only those three values of direction allowed...
							Throw New AssertionError(direction)
						End Select

						' set the unset edge; if neither set then we're getting
						' both edges of the current run around our 'index'
						attributeRunStartIndex = If(attributeRunStartIndex <> Integer.MinValue, attributeRunStartIndex, getRunEdge(tempIndex, -1))
						attributeRunEndIndex = If(attributeRunEndIndex <> Integer.MinValue, attributeRunEndIndex, getRunEdge(tempIndex, 1))

						runText = outerInstance.model.getText(attributeRunStartIndex, attributeRunEndIndex - attributeRunStartIndex)
					Catch e As BadLocationException
						' we are intentionally silent; our contract says we return
						' null if there is any failure in this method
						Return Nothing
					Finally
						If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
					End Try
					Return New AccessibleTextSequence(attributeRunStartIndex, attributeRunEndIndex, runText)

				Case Else
				End Select
				Return Nothing
			End Function


			''' <summary>
			''' Starting at text position <code>index</code>, and going in
			''' <code>direction</code>, return the edge of run that shares the
			''' same <code>AttributeSet</code> and parent element as those at
			''' <code>index</code>.
			''' 
			''' Note: we assume the document is already locked...
			''' </summary>
			Private Function getRunEdge(ByVal index As Integer, ByVal direction As Integer) As Integer
				If index < 0 OrElse index >= outerInstance.model.length Then Throw New BadLocationException("Location out of bounds", index)
				' locate the Element at index
				Dim indexElement As Element
				' locate the Element at our index/offset
				Dim elementIndex As Integer = -1 ' test for initialization
				indexElement = outerInstance.model.defaultRootElement
				Do While Not indexElement.leaf
					elementIndex = indexElement.getElementIndex(index)
					indexElement = indexElement.getElement(elementIndex)
				Loop
				If elementIndex = -1 Then Throw New AssertionError(index)
				' cache the AttributeSet and parentElement atindex
				Dim indexAS As AttributeSet = indexElement.attributes
				Dim parent As Element = indexElement.parentElement

				' find the first Element before/after ours w/the same AttributeSet
				' if we are already at edge of the first element in our parent
				' then return that edge
				Dim edgeElement As Element
				Select Case direction
				Case -1, 1
					Dim edgeElementIndex As Integer = elementIndex
					Dim elementCount As Integer = parent.elementCount
					Do While (edgeElementIndex + direction) > 0 AndAlso ((edgeElementIndex + direction) < elementCount) AndAlso parent.getElement(edgeElementIndex + direction).attributes.isEqual(indexAS)
						edgeElementIndex += direction
					Loop
					edgeElement = parent.getElement(edgeElementIndex)
				Case Else
					Throw New AssertionError(direction)
				End Select
				Select Case direction
				Case -1
					Return edgeElement.startOffset
				Case 1
					Return edgeElement.endOffset
				Case Else
					' we already caught this case earlier; this is to satisfy
					' the compiler...
					Return Integer.MinValue
				End Select
			End Function

			' getTextRange() not needed; defined in AccessibleEditableText

			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> at a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or
			''' <code>ATTRIBUTE_RUN</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text if
			''' <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceAt(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence Implements AccessibleExtendedText.getTextSequenceAt
				Return getSequenceAtIndex(part, index, 0)
			End Function

			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> after a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or
			''' <code>ATTRIBUTE_RUN</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text
			''' if <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceAfter(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence Implements AccessibleExtendedText.getTextSequenceAfter
				Return getSequenceAtIndex(part, index, 1)
			End Function

			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> before a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or
			''' <code>ATTRIBUTE_RUN</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text
			''' if <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceBefore(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence Implements AccessibleExtendedText.getTextSequenceBefore
				Return getSequenceAtIndex(part, index, -1)
			End Function

			''' <summary>
			''' Returns the <code>Rectangle</code> enclosing the text between
			''' two indicies.
			''' </summary>
			''' <param name="startIndex"> the start index in the text </param>
			''' <param name="endIndex"> the end index in the text </param>
			''' <returns> the bounding rectangle of the text if the indices are valid.
			''' Otherwise, <code>null</code> is returned
			''' 
			''' @since 1.6 </returns>
			Public Overridable Function getTextBounds(ByVal startIndex As Integer, ByVal endIndex As Integer) As Rectangle Implements AccessibleExtendedText.getTextBounds
				If startIndex < 0 OrElse startIndex > outerInstance.model.length-1 OrElse endIndex < 0 OrElse endIndex > outerInstance.model.length-1 OrElse startIndex > endIndex Then Return Nothing
				Dim ui As TextUI = outerInstance.uI
				If ui Is Nothing Then Return Nothing
				Dim rect As Rectangle = Nothing
				Dim alloc As Rectangle = rootEditorRect
				If alloc Is Nothing Then Return Nothing
				If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readLock()
				Try
					Dim rootView As View = ui.getRootView(JTextComponent.this)
					If rootView IsNot Nothing Then
						Dim bounds As Shape = rootView.modelToView(startIndex, Position.Bias.Forward, endIndex, Position.Bias.Backward, alloc)

						rect = If(TypeOf bounds Is Rectangle, CType(bounds, Rectangle), bounds.bounds)

					End If
				Catch e As BadLocationException
				Finally
					If TypeOf outerInstance.model Is AbstractDocument Then CType(outerInstance.model, AbstractDocument).readUnlock()
				End Try
				Return rect
			End Function

			' ----- end AccessibleExtendedText methods


			' --- interface AccessibleAction methods ------------------------

			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible actions available in this object
			''' If there are more than one, the first one is considered the
			''' "default" action of the object.
			''' </summary>
			''' <returns> the zero-based number of Actions in this object
			''' @since 1.4 </returns>
			Public Overridable Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount
				Get
					Dim actions As Action() = outerInstance.actions
					Return actions.Length
				End Get
			End Property

			''' <summary>
			''' Returns a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> a String description of the action </returns>
			''' <seealso cref= #getAccessibleActionCount
			''' @since 1.4 </seealso>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription
				Dim actions As Action() = outerInstance.actions
				If i < 0 OrElse i >= actions.Length Then Return Nothing
				Return CStr(actions(i).getValue(Action.NAME))
			End Function

			''' <summary>
			''' Performs the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			''' <seealso cref= #getAccessibleActionCount
			''' @since 1.4 </seealso>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction
				Dim actions As Action() = outerInstance.actions
				If i < 0 OrElse i >= actions.Length Then Return False
				Dim ae As New ActionEvent(JTextComponent.this, ActionEvent.ACTION_PERFORMED, Nothing, EventQueue.mostRecentEventTime, outerInstance.currentEventModifiers)
				actions(i).actionPerformed(ae)
				Return True
			End Function

			' ----- end AccessibleAction methods


		End Class


		' --- serialization ---------------------------------------------

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			caretEvent = New MutableCaretEvent(Me)
			addMouseListener(caretEvent)
			addFocusListener(caretEvent)
		End Sub

		' --- member variables ----------------------------------

		''' <summary>
		''' The document model.
		''' </summary>
		Private model As Document

		''' <summary>
		''' The caret used to display the insert position
		''' and navigate throughout the document.
		''' 
		''' PENDING(prinz)
		''' This should be serializable, default installed
		''' by UI.
		''' </summary>
		<NonSerialized> _
		Private caret As Caret

		''' <summary>
		''' Object responsible for restricting the cursor navigation.
		''' </summary>
		Private navigationFilter As NavigationFilter

		''' <summary>
		''' The object responsible for managing highlights.
		''' 
		''' PENDING(prinz)
		''' This should be serializable, default installed
		''' by UI.
		''' </summary>
		<NonSerialized> _
		Private highlighter As Highlighter

		''' <summary>
		''' The current key bindings in effect.
		''' 
		''' PENDING(prinz)
		''' This should be serializable, default installed
		''' by UI.
		''' </summary>
		<NonSerialized> _
		Private keymap As Keymap

		<NonSerialized> _
		Private caretEvent As MutableCaretEvent
		Private caretColor As Color
		Private selectionColor As Color
		Private selectedTextColor As Color
		Private disabledTextColor As Color
		Private editable As Boolean
		Private margin As Insets
		Private focusAccelerator As Char
		Private dragEnabled As Boolean

		''' <summary>
		''' The drop mode for this component.
		''' </summary>
		Private dropMode As DropMode = DropMode.USE_SELECTION

		''' <summary>
		''' The drop location.
		''' </summary>
		<NonSerialized> _
		Private dropLocation As DropLocation

		''' <summary>
		''' Represents a drop location for <code>JTextComponent</code>s.
		''' </summary>
		''' <seealso cref= #getDropLocation
		''' @since 1.6 </seealso>
		Public NotInheritable Class DropLocation
			Inherits TransferHandler.DropLocation

			Private ReadOnly index As Integer
			Private ReadOnly bias As Position.Bias

			Private Sub New(ByVal p As Point, ByVal index As Integer, ByVal bias As Position.Bias)
				MyBase.New(p)
				Me.index = index
				Me.bias = bias
			End Sub

			''' <summary>
			''' Returns the index where dropped data should be inserted into the
			''' associated component. This index represents a position between
			''' characters, as would be interpreted by a caret.
			''' </summary>
			''' <returns> the drop index </returns>
			Public Property index As Integer
				Get
					Return index
				End Get
			End Property

			''' <summary>
			''' Returns the bias for the drop index.
			''' </summary>
			''' <returns> the drop bias </returns>
			Public Property bias As Position.Bias
				Get
					Return bias
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this drop location.
			''' This method is intended to be used for debugging purposes,
			''' and the content and format of the returned string may vary
			''' between implementations.
			''' </summary>
			''' <returns> a string representation of this drop location </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[dropPoint=" & dropPoint & "," & "index=" & index & "," & "bias=" & bias & "]"
			End Function
		End Class

		''' <summary>
		''' TransferHandler used if one hasn't been supplied by the UI.
		''' </summary>
		Private Shared defaultTransferHandler As DefaultTransferHandler

		''' <summary>
		''' Maps from class name to Boolean indicating if
		''' <code>processInputMethodEvent</code> has been overriden.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private static com.sun.beans.util.Cache<Class,java.lang.Boolean> METHOD_OVERRIDDEN = New com.sun.beans.util.Cache<Class,java.lang.Boolean>(com.sun.beans.util.Cache.Kind.WEAK, com.sun.beans.util.Cache.Kind.STRONG)
	'	{
	'		''' <summary>
	'		''' Returns {@code true} if the specified {@code type} extends <seealso cref="JTextComponent"/>
	'		''' and the <seealso cref="JTextComponent#processInputMethodEvent"/> method is overridden.
	'		''' </summary>
	'		@Override public java.lang.Boolean create(final Class type)
	'		{
	'			if (JTextComponent.class == type)
	'			{
	'				Return Boolean.FALSE;
	'			}
	'			if (get(type.getSuperclass()))
	'			{
	'				Return Boolean.TRUE;
	'			}
	'			Return AccessController.doPrivileged(New PrivilegedAction<java.lang.Boolean>()
	'			{
	'						public java.lang.Boolean run()
	'						{
	'							try
	'							{
	'								type.getDeclaredMethod("processInputMethodEvent", InputMethodEvent.class);
	'								Return Boolean.TRUE;
	'							}
	'							catch (NoSuchMethodException exception)
	'							{
	'								Return Boolean.FALSE;
	'							}
	'						}
	'					});
	'		}
	'	};

		''' <summary>
		''' Returns a string representation of this <code>JTextComponent</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' <P>
		''' Overriding <code>paramString</code> to provide information about the
		''' specific new aspects of the JFC components.
		''' </summary>
		''' <returns>  a string representation of this <code>JTextComponent</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim editableString As String = (If(editable, "true", "false"))
			Dim caretColorString As String = (If(caretColor IsNot Nothing, caretColor.ToString(), ""))
			Dim selectionColorString As String = (If(selectionColor IsNot Nothing, selectionColor.ToString(), ""))
			Dim selectedTextColorString As String = (If(selectedTextColor IsNot Nothing, selectedTextColor.ToString(), ""))
			Dim disabledTextColorString As String = (If(disabledTextColor IsNot Nothing, disabledTextColor.ToString(), ""))
			Dim marginString As String = (If(margin IsNot Nothing, margin.ToString(), ""))

			Return MyBase.paramString() & ",caretColor=" & caretColorString & ",disabledTextColor=" & disabledTextColorString & ",editable=" & editableString & ",margin=" & marginString & ",selectedTextColor=" & selectedTextColorString & ",selectionColor=" & selectionColorString
		End Function


		''' <summary>
		''' A Simple TransferHandler that exports the data as a String, and
		''' imports the data from the String clipboard.  This is only used
		''' if the UI hasn't supplied one, which would only happen if someone
		''' hasn't subclassed Basic.
		''' </summary>
		Friend Class DefaultTransferHandler
			Inherits TransferHandler
			Implements UIResource

			Public Overrides Sub exportToClipboard(ByVal comp As JComponent, ByVal clipboard As Clipboard, ByVal action As Integer)
				If TypeOf comp Is JTextComponent Then
					Dim text As JTextComponent = CType(comp, JTextComponent)
					Dim p0 As Integer = text.selectionStart
					Dim p1 As Integer = text.selectionEnd
					If p0 <> p1 Then
						Try
							Dim doc As Document = text.document
							Dim srcData As String = doc.getText(p0, p1 - p0)
							Dim contents As New StringSelection(srcData)

							' this may throw an IllegalStateException,
							' but it will be caught and handled in the
							' action that invoked this method
							clipboard.contentsnts(contents, Nothing)

							If action = TransferHandler.MOVE Then doc.remove(p0, p1 - p0)
						Catch ble As BadLocationException
						End Try
					End If
				End If
			End Sub
			Public Overrides Function importData(ByVal comp As JComponent, ByVal t As Transferable) As Boolean
				If TypeOf comp Is JTextComponent Then
					Dim ___flavor As DataFlavor = getFlavor(t.transferDataFlavors)

					If ___flavor IsNot Nothing Then
						Dim ic As java.awt.im.InputContext = comp.inputContext
						If ic IsNot Nothing Then ic.endComposition()
						Try
							Dim data As String = CStr(t.getTransferData(___flavor))

							CType(comp, JTextComponent).replaceSelection(data)
							Return True
						Catch ufe As UnsupportedFlavorException
						Catch ioe As IOException
						End Try
					End If
				End If
				Return False
			End Function
			Public Overrides Function canImport(ByVal comp As JComponent, ByVal transferFlavors As DataFlavor()) As Boolean
				Dim c As JTextComponent = CType(comp, JTextComponent)
				If Not(c.editable AndAlso c.enabled) Then Return False
				Return (getFlavor(transferFlavors) IsNot Nothing)
			End Function
			Public Overrides Function getSourceActions(ByVal c As JComponent) As Integer
				Return NONE
			End Function
			Private Function getFlavor(ByVal flavors As DataFlavor()) As DataFlavor
				If flavors IsNot Nothing Then
					For Each ___flavor As DataFlavor In flavors
						If ___flavor.Equals(DataFlavor.stringFlavor) Then Return ___flavor
					Next ___flavor
				End If
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Returns the JTextComponent that most recently had focus. The returned
		''' value may currently have focus.
		''' </summary>
		Shared focusedComponent As JTextComponent
			Get
				Return CType(sun.awt.AppContext.appContext.get(FOCUSED_COMPONENT), JTextComponent)
			End Get
		End Property

		Private Property currentEventModifiers As Integer
			Get
				Dim modifiers As Integer = 0
				Dim currentEvent As AWTEvent = EventQueue.currentEvent
				If TypeOf currentEvent Is InputEvent Then
					modifiers = CType(currentEvent, InputEvent).modifiers
				ElseIf TypeOf currentEvent Is ActionEvent Then
					modifiers = CType(currentEvent, ActionEvent).modifiers
				End If
				Return modifiers
			End Get
		End Property

		Private Shared ReadOnly KEYMAP_TABLE As Object = New StringBuilder("JTextComponent_KeymapTable")

		'
		' member variables used for on-the-spot input method
		' editing style support
		'
		<NonSerialized> _
		Private inputMethodRequestsHandler As java.awt.im.InputMethodRequests
		Private composedTextAttribute As SimpleAttributeSet
		Private composedTextContent As String
		Private composedTextStart As Position
		Private composedTextEnd As Position
		Private latestCommittedTextStart As Position
		Private latestCommittedTextEnd As Position
		Private composedTextCaret As ComposedTextCaret
		<NonSerialized> _
		Private originalCaret As Caret
		''' <summary>
		''' Set to true after the check for the override of processInputMethodEvent
		''' has been checked.
		''' </summary>
		Private checkedInputOverride As Boolean
		Private needToSendKeyTypedEvent As Boolean

		Friend Class DefaultKeymap
			Implements Keymap

			Friend Sub New(ByVal nm As String, ByVal parent As Keymap)
				Me.nm = nm
				Me.parent = parent
				bindings = New Dictionary(Of KeyStroke, Action)
			End Sub

			''' <summary>
			''' Fetch the default action to fire if a
			''' key is typed (ie a KEY_TYPED KeyEvent is received)
			''' and there is no binding for it.  Typically this
			''' would be some action that inserts text so that
			''' the keymap doesn't require an action for each
			''' possible key.
			''' </summary>
			Public Overridable Property defaultAction As Action
				Get
					If defaultAction IsNot Nothing Then Return defaultAction
					Return If(parent IsNot Nothing, parent.defaultAction, Nothing)
				End Get
				Set(ByVal a As Action)
					defaultAction = a
				End Set
			End Property


			Public Overridable Property name As String Implements Keymap.getName
				Get
					Return nm
				End Get
			End Property

			Public Overridable Function getAction(ByVal key As KeyStroke) As Action
				Dim a As Action = bindings(key)
				If (a Is Nothing) AndAlso (parent IsNot Nothing) Then a = parent.getAction(key)
				Return a
			End Function

			Public Overridable Property boundKeyStrokes As KeyStroke()
				Get
					Dim keys As KeyStroke() = New KeyStroke(bindings.Count - 1){}
					Dim i As Integer = 0
					Dim e As System.Collections.IEnumerator(Of KeyStroke) = bindings.Keys.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						keys(i) = e.nextElement()
						i += 1
					Loop
					Return keys
				End Get
			End Property

			Public Overridable Property boundActions As Action()
				Get
					Dim actions As Action() = New Action(bindings.Count - 1){}
					Dim i As Integer = 0
					Dim e As System.Collections.IEnumerator(Of Action) = bindings.Values.GetEnumerator()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						actions(i) = e.nextElement()
						i += 1
					Loop
					Return actions
				End Get
			End Property

			Public Overridable Function getKeyStrokesForAction(ByVal a As Action) As KeyStroke()
				If a Is Nothing Then Return Nothing
				Dim retValue As KeyStroke() = Nothing
				' Determine local bindings first.
				Dim keyStrokes As List(Of KeyStroke) = Nothing
				Dim keys As System.Collections.IEnumerator(Of KeyStroke) = bindings.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim key As KeyStroke = keys.nextElement()
					If bindings(key) Is a Then
						If keyStrokes Is Nothing Then keyStrokes = New List(Of KeyStroke)
						keyStrokes.Add(key)
					End If
				Loop
				' See if the parent has any.
				If parent IsNot Nothing Then
					Dim pStrokes As KeyStroke() = parent.getKeyStrokesForAction(a)
					If pStrokes IsNot Nothing Then
						' Remove any bindings defined in the parent that
						' are locally defined.
						Dim rCount As Integer = 0
						For counter As Integer = pStrokes.Length - 1 To 0 Step -1
							If isLocallyDefined(pStrokes(counter)) Then
								pStrokes(counter) = Nothing
								rCount += 1
							End If
						Next counter
						If rCount > 0 AndAlso rCount < pStrokes.Length Then
							If keyStrokes Is Nothing Then keyStrokes = New List(Of KeyStroke)
							For counter As Integer = pStrokes.Length - 1 To 0 Step -1
								If pStrokes(counter) IsNot Nothing Then keyStrokes.Add(pStrokes(counter))
							Next counter
						ElseIf rCount = 0 Then
							If keyStrokes Is Nothing Then
								retValue = pStrokes
							Else
								retValue = New KeyStroke(keyStrokes.Count + pStrokes.Length - 1){}
								keyStrokes.CopyTo(retValue)
								Array.Copy(pStrokes, 0, retValue, keyStrokes.Count, pStrokes.Length)
								keyStrokes = Nothing
							End If
						End If
					End If
				End If
				If keyStrokes IsNot Nothing Then
					retValue = New KeyStroke(keyStrokes.Count - 1){}
					keyStrokes.CopyTo(retValue)
				End If
				Return retValue
			End Function

			Public Overridable Function isLocallyDefined(ByVal key As KeyStroke) As Boolean
				Return bindings.ContainsKey(key)
			End Function

			Public Overridable Sub addActionForKeyStroke(ByVal key As KeyStroke, ByVal a As Action)
				bindings(key) = a
			End Sub

			Public Overridable Sub removeKeyStrokeBinding(ByVal key As KeyStroke)
				bindings.Remove(key)
			End Sub

			Public Overridable Sub removeBindings() Implements Keymap.removeBindings
				bindings.Clear()
			End Sub

			Public Overridable Property resolveParent As Keymap Implements Keymap.getResolveParent
				Get
					Return parent
				End Get
				Set(ByVal parent As Keymap)
					Me.parent = parent
				End Set
			End Property


			''' <summary>
			''' String representation of the keymap... potentially
			''' a very long string.
			''' </summary>
			Public Overrides Function ToString() As String
				Return "Keymap[" & nm & "]" & bindings
			End Function

			Friend nm As String
			Friend parent As Keymap
			Friend bindings As Dictionary(Of KeyStroke, Action)
			Friend defaultAction As Action
		End Class


		''' <summary>
		''' KeymapWrapper wraps a Keymap inside an InputMap. For KeymapWrapper
		''' to be useful it must be used with a KeymapActionMap.
		''' KeymapWrapper for the most part, is an InputMap with two parents.
		''' The first parent visited is ALWAYS the Keymap, with the second
		''' parent being the parent inherited from InputMap. If
		''' <code>keymap.getAction</code> returns null, implying the Keymap
		''' does not have a binding for the KeyStroke,
		''' the parent is then visited. If the Keymap has a binding, the
		''' Action is returned, if not and the KeyStroke represents a
		''' KeyTyped event and the Keymap has a defaultAction,
		''' <code>DefaultActionKey</code> is returned.
		''' <p>KeymapActionMap is then able to transate the object passed in
		''' to either message the Keymap, or message its default implementation.
		''' </summary>
		Friend Class KeymapWrapper
			Inherits InputMap

			Friend Shared ReadOnly DefaultActionKey As New Object

			Private keymap As Keymap

			Friend Sub New(ByVal keymap As Keymap)
				Me.keymap = keymap
			End Sub

			Public Overrides Function keys() As KeyStroke()
				Dim sKeys As KeyStroke() = MyBase.keys()
				Dim keymapKeys As KeyStroke() = keymap.boundKeyStrokes
				Dim sCount As Integer = If(sKeys Is Nothing, 0, sKeys.Length)
				Dim keymapCount As Integer = If(keymapKeys Is Nothing, 0, keymapKeys.Length)
				If sCount = 0 Then Return keymapKeys
				If keymapCount = 0 Then Return sKeys
				Dim retValue As KeyStroke() = New KeyStroke(sCount + keymapCount - 1){}
				' There may be some duplication here...
				Array.Copy(sKeys, 0, retValue, 0, sCount)
				Array.Copy(keymapKeys, 0, retValue, sCount, keymapCount)
				Return retValue
			End Function

			Public Overrides Function size() As Integer
				' There may be some duplication here...
				Dim keymapStrokes As KeyStroke() = keymap.boundKeyStrokes
				Dim keymapCount As Integer = If(keymapStrokes Is Nothing, 0, keymapStrokes.Length)
				Return MyBase.size() + keymapCount
			End Function

			Public Overrides Function [get](ByVal ___keyStroke As KeyStroke) As Object
				Dim retValue As Object = keymap.getAction(___keyStroke)
				If retValue Is Nothing Then
					retValue = MyBase.get(___keyStroke)
					If retValue Is Nothing AndAlso ___keyStroke.keyChar <> KeyEvent.CHAR_UNDEFINED AndAlso keymap.defaultAction IsNot Nothing Then retValue = DefaultActionKey
				End If
				Return retValue
			End Function
		End Class


		''' <summary>
		''' Wraps a Keymap inside an ActionMap. This is used with
		''' a KeymapWrapper. If <code>get</code> is passed in
		''' <code>KeymapWrapper.DefaultActionKey</code>, the default action is
		''' returned, otherwise if the key is an Action, it is returned.
		''' </summary>
		Friend Class KeymapActionMap
			Inherits ActionMap

			Private keymap As Keymap

			Friend Sub New(ByVal keymap As Keymap)
				Me.keymap = keymap
			End Sub

			Public Overrides Function keys() As Object()
				Dim sKeys As Object() = MyBase.keys()
				Dim keymapKeys As Object() = keymap.boundActions
				Dim sCount As Integer = If(sKeys Is Nothing, 0, sKeys.Length)
				Dim keymapCount As Integer = If(keymapKeys Is Nothing, 0, keymapKeys.Length)
				Dim hasDefault As Boolean = (keymap.defaultAction IsNot Nothing)
				If hasDefault Then keymapCount += 1
				If sCount = 0 Then
					If hasDefault Then
						Dim retValue As Object() = New Object(keymapCount - 1){}
						If keymapCount > 1 Then Array.Copy(keymapKeys, 0, retValue, 0, keymapCount - 1)
						retValue(keymapCount - 1) = KeymapWrapper.DefaultActionKey
						Return retValue
					End If
					Return keymapKeys
				End If
				If keymapCount = 0 Then Return sKeys
				Dim retValue As Object() = New Object(sCount + keymapCount - 1){}
				' There may be some duplication here...
				Array.Copy(sKeys, 0, retValue, 0, sCount)
				If hasDefault Then
					If keymapCount > 1 Then Array.Copy(keymapKeys, 0, retValue, sCount, keymapCount - 1)
					retValue(sCount + keymapCount - 1) = KeymapWrapper.DefaultActionKey
				Else
					Array.Copy(keymapKeys, 0, retValue, sCount, keymapCount)
				End If
				Return retValue
			End Function

			Public Overrides Function size() As Integer
				' There may be some duplication here...
				Dim actions As Object() = keymap.boundActions
				Dim keymapCount As Integer = If(actions Is Nothing, 0, actions.Length)
				If keymap.defaultAction IsNot Nothing Then keymapCount += 1
				Return MyBase.size() + keymapCount
			End Function

			Public Overrides Function [get](ByVal key As Object) As Action
				Dim retValue As Action = MyBase.get(key)
				If retValue Is Nothing Then
					' Try the Keymap.
					If key Is KeymapWrapper.DefaultActionKey Then
						retValue = keymap.defaultAction
					ElseIf TypeOf key Is Action Then
						' This is a little iffy, technically an Action is
						' a valid Key. We're assuming the Action came from
						' the InputMap though.
						retValue = CType(key, Action)
					End If
				End If
				Return retValue
			End Function
		End Class

		Private Shared ReadOnly FOCUSED_COMPONENT As Object = New StringBuilder("JTextComponent_FocusedComponent")

		''' <summary>
		''' The default keymap that will be shared by all
		''' <code>JTextComponent</code> instances unless they
		''' have had a different keymap set.
		''' </summary>
		Public Const DEFAULT_KEYMAP As String = "default"

		''' <summary>
		''' Event to use when firing a notification of change to caret
		''' position.  This is mutable so that the event can be reused
		''' since caret events can be fairly high in bandwidth.
		''' </summary>
		Friend Class MutableCaretEvent
			Inherits CaretEvent
			Implements ChangeListener, FocusListener, MouseListener

			Friend Sub New(ByVal c As JTextComponent)
				MyBase.New(c)
			End Sub

			Friend Sub fire()
				Dim c As JTextComponent = CType(source, JTextComponent)
				If c IsNot Nothing Then
					Dim caret As Caret = c.caret
					dot = caret.dot
					mark = caret.mark
					c.fireCaretUpdate(Me)
				End If
			End Sub

			Public NotOverridable Overrides Function ToString() As String
				Return "dot=" & dot & "," & "mark=" & mark
			End Function

			' --- CaretEvent methods -----------------------

			Public Property NotOverridable Overrides dot As Integer
				Get
					Return dot
				End Get
			End Property

			Public Property NotOverridable Overrides mark As Integer
				Get
					Return mark
				End Get
			End Property

			' --- ChangeListener methods -------------------

			Public Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If Not dragActive Then fire()
			End Sub

			' --- FocusListener methods -----------------------------------
			Public Overridable Sub focusGained(ByVal fe As FocusEvent)
				sun.awt.AppContext.appContext.put(FOCUSED_COMPONENT, fe.source)
			End Sub

			Public Overridable Sub focusLost(ByVal fe As FocusEvent)
			End Sub

			' --- MouseListener methods -----------------------------------

			''' <summary>
			''' Requests focus on the associated
			''' text component, and try to set the cursor position.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			''' <seealso cref= MouseListener#mousePressed </seealso>
			Public Sub mousePressed(ByVal e As MouseEvent)
				dragActive = True
			End Sub

			''' <summary>
			''' Called when the mouse is released.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			''' <seealso cref= MouseListener#mouseReleased </seealso>
			Public Sub mouseReleased(ByVal e As MouseEvent)
				dragActive = False
				fire()
			End Sub

			Public Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			Private dragActive As Boolean
			Private dot As Integer
			Private mark As Integer
		End Class

		'
		' Process any input method events that the component itself
		' recognizes. The default on-the-spot handling for input method
		' composed(uncommitted) text is done here after all input
		' method listeners get called for stealing the events.
		'
		Protected Friend Overridable Sub processInputMethodEvent(ByVal e As InputMethodEvent)
			' let listeners handle the events
			MyBase.processInputMethodEvent(e)

			If Not e.consumed Then
				If Not editable Then
					Return
				Else
					Select Case e.iD
					Case InputMethodEvent.INPUT_METHOD_TEXT_CHANGED
						replaceInputMethodText(e)

						' fall through

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case InputMethodEvent.CARET_POSITION_CHANGED
						inputMethodCaretPosition = e
					End Select
				End If

				e.consume()
			End If
		End Sub

		'
		' Overrides this method to become an active input method client.
		'
		Public Overridable Property inputMethodRequests As java.awt.im.InputMethodRequests
			Get
				If inputMethodRequestsHandler Is Nothing Then
					inputMethodRequestsHandler = New InputMethodRequestsHandler(Me)
					Dim doc As Document = document
					If doc IsNot Nothing Then doc.addDocumentListener(CType(inputMethodRequestsHandler, DocumentListener))
				End If
    
				Return inputMethodRequestsHandler
			End Get
		End Property

		'
		' Overrides this method to watch the listener installed.
		'
		Public Overridable Sub addInputMethodListener(ByVal l As InputMethodListener)
			MyBase.addInputMethodListener(l)
			If l IsNot Nothing Then
				needToSendKeyTypedEvent = False
				checkedInputOverride = True
			End If
		End Sub


		'
		' Default implementation of the InputMethodRequests interface.
		'
		Friend Class InputMethodRequestsHandler
			Implements java.awt.im.InputMethodRequests, DocumentListener

			Private ReadOnly outerInstance As JTextComponent

			Public Sub New(ByVal outerInstance As JTextComponent)
				Me.outerInstance = outerInstance
			End Sub


			' --- InputMethodRequests methods ---

			Public Overridable Function cancelLatestCommittedText(ByVal attributes As java.text.AttributedCharacterIterator.Attribute()) As AttributedCharacterIterator
				Dim doc As Document = outerInstance.document
				If (doc IsNot Nothing) AndAlso (outerInstance.latestCommittedTextStart IsNot Nothing) AndAlso ((Not outerInstance.latestCommittedTextStart.Equals(outerInstance.latestCommittedTextEnd))) Then
					Try
						Dim startIndex As Integer = outerInstance.latestCommittedTextStart.offset
						Dim endIndex As Integer = outerInstance.latestCommittedTextEnd.offset
						Dim latestCommittedText As String = doc.getText(startIndex, endIndex - startIndex)
						doc.remove(startIndex, endIndex - startIndex)
						Return (New AttributedString(latestCommittedText)).iterator
					Catch ble As BadLocationException
					End Try
				End If
				Return Nothing
			End Function

			Public Overridable Function getCommittedText(ByVal beginIndex As Integer, ByVal endIndex As Integer, ByVal attributes As java.text.AttributedCharacterIterator.Attribute()) As AttributedCharacterIterator
				Dim composedStartIndex As Integer = 0
				Dim composedEndIndex As Integer = 0
				If outerInstance.composedTextExists() Then
					composedStartIndex = outerInstance.composedTextStart.offset
					composedEndIndex = outerInstance.composedTextEnd.offset
				End If

				Dim committed As String
				Try
					If beginIndex < composedStartIndex Then
						If endIndex <= composedStartIndex Then
							committed = outerInstance.getText(beginIndex, endIndex - beginIndex)
						Else
							Dim firstPartLength As Integer = composedStartIndex - beginIndex
							committed = outerInstance.getText(beginIndex, firstPartLength) + outerInstance.getText(composedEndIndex, endIndex - beginIndex - firstPartLength)
						End If
					Else
						committed = outerInstance.getText(beginIndex + (composedEndIndex - composedStartIndex), endIndex - beginIndex)
					End If
				Catch ble As BadLocationException
					Throw New System.ArgumentException("Invalid range")
				End Try
				Return (New AttributedString(committed)).iterator
			End Function

			Public Overridable Property committedTextLength As Integer
				Get
					Dim doc As Document = outerInstance.document
					Dim length As Integer = 0
					If doc IsNot Nothing Then
						length = doc.length
						If outerInstance.composedTextContent IsNot Nothing Then
							If outerInstance.composedTextEnd Is Nothing OrElse outerInstance.composedTextStart Is Nothing Then
		'                        
		'                         * fix for : 6355666
		'                         * this is the case when this method is invoked
		'                         * from DocumentListener. At this point
		'                         * composedTextEnd and composedTextStart are
		'                         * not defined yet.
		'                         
								length -= outerInstance.composedTextContent.Length
							Else
								length -= outerInstance.composedTextEnd.offset - outerInstance.composedTextStart.offset
							End If
						End If
					End If
					Return length
				End Get
			End Property

			Public Overridable Property insertPositionOffset As Integer
				Get
					Dim composedStartIndex As Integer = 0
					Dim composedEndIndex As Integer = 0
					If outerInstance.composedTextExists() Then
						composedStartIndex = outerInstance.composedTextStart.offset
						composedEndIndex = outerInstance.composedTextEnd.offset
					End If
					Dim caretIndex As Integer = outerInstance.caretPosition
    
					If caretIndex < composedStartIndex Then
						Return caretIndex
					ElseIf caretIndex < composedEndIndex Then
						Return composedStartIndex
					Else
						Return caretIndex - (composedEndIndex - composedStartIndex)
					End If
				End Get
			End Property

			Public Overridable Function getLocationOffset(ByVal x As Integer, ByVal y As Integer) As java.awt.font.TextHitInfo
				If outerInstance.composedTextAttribute Is Nothing Then
					Return Nothing
				Else
					Dim p As Point = locationOnScreen
					p.x = x - p.x
					p.y = y - p.y
					Dim pos As Integer = outerInstance.viewToModel(p)
					If (pos >= outerInstance.composedTextStart.offset) AndAlso (pos <= outerInstance.composedTextEnd.offset) Then
						Return java.awt.font.TextHitInfo.leading(pos - outerInstance.composedTextStart.offset)
					Else
						Return Nothing
					End If
				End If
			End Function

			Public Overridable Function getTextLocation(ByVal offset As java.awt.font.TextHitInfo) As Rectangle
				Dim r As Rectangle

				Try
					r = outerInstance.modelToView(outerInstance.caretPosition)
					If r IsNot Nothing Then
						Dim p As Point = locationOnScreen
						r.translate(p.x, p.y)
					End If
				Catch ble As BadLocationException
					r = Nothing
				End Try

				If r Is Nothing Then r = New Rectangle

				Return r
			End Function

			Public Overridable Function getSelectedText(ByVal attributes As java.text.AttributedCharacterIterator.Attribute()) As AttributedCharacterIterator
				Dim selection As String = outerInstance.selectedText
				If selection IsNot Nothing Then
					Return (New AttributedString(selection)).iterator
				Else
					Return Nothing
				End If
			End Function

			' --- DocumentListener methods ---

			Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
					outerInstance.latestCommittedTextEnd = Nothing
					outerInstance.latestCommittedTextStart = outerInstance.latestCommittedTextEnd
			End Sub

			Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
					outerInstance.latestCommittedTextEnd = Nothing
					outerInstance.latestCommittedTextStart = outerInstance.latestCommittedTextEnd
			End Sub

			Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
					outerInstance.latestCommittedTextEnd = Nothing
					outerInstance.latestCommittedTextStart = outerInstance.latestCommittedTextEnd
			End Sub
		End Class

		'
		' Replaces the current input method (composed) text according to
		' the passed input method event. This method also inserts the
		' committed text into the document.
		'
		Private Sub replaceInputMethodText(ByVal e As InputMethodEvent)
			Dim commitCount As Integer = e.committedCharacterCount
			Dim ___text As AttributedCharacterIterator = e.text
			Dim composedTextIndex As Integer

			' old composed text deletion
			Dim doc As Document = document
			If composedTextExists() Then
				Try
					doc.remove(composedTextStart.offset, composedTextEnd.offset - composedTextStart.offset)
				Catch ble As BadLocationException
				End Try
					composedTextEnd = Nothing
					composedTextStart = composedTextEnd
				composedTextAttribute = Nothing
				composedTextContent = Nothing
			End If

			If ___text IsNot Nothing Then
				___text.first()
				Dim committedTextStartIndex As Integer = 0
				Dim committedTextEndIndex As Integer = 0

				' committed text insertion
				If commitCount > 0 Then
					' Remember latest committed text start index
					committedTextStartIndex = caret.dot

					' Need to generate KeyTyped events for the committed text for components
					' that are not aware they are active input method clients.
					If shouldSynthensizeKeyEvents() Then
						Dim c As Char = ___text.current()
						Do While commitCount > 0
							Dim ke As New KeyEvent(Me, KeyEvent.KEY_TYPED, EventQueue.mostRecentEventTime, 0, KeyEvent.VK_UNDEFINED, c)
							processKeyEvent(ke)
							c = ___text.next()
							commitCount -= 1
						Loop
					Else
						Dim strBuf As New StringBuilder
						Dim c As Char = ___text.current()
						Do While commitCount > 0
							strBuf.Append(c)
							c = ___text.next()
							commitCount -= 1
						Loop

						' map it to an ActionEvent
						mapCommittedTextToAction(strBuf.ToString())
					End If

					' Remember latest committed text end index
					committedTextEndIndex = caret.dot
				End If

				' new composed text insertion
				composedTextIndex = ___text.index
				If composedTextIndex < ___text.endIndex Then
					createComposedTextAttribute(composedTextIndex, ___text)
					Try
						replaceSelection(Nothing)
						doc.insertString(caret.dot, composedTextContent, composedTextAttribute)
						composedTextStart = doc.createPosition(caret.dot - composedTextContent.Length)
						composedTextEnd = doc.createPosition(caret.dot)
					Catch ble As BadLocationException
							composedTextEnd = Nothing
							composedTextStart = composedTextEnd
						composedTextAttribute = Nothing
						composedTextContent = Nothing
					End Try
				End If

				' Save the latest committed text information
				If committedTextStartIndex <> committedTextEndIndex Then
					Try
						latestCommittedTextStart = doc.createPosition(committedTextStartIndex)
						latestCommittedTextEnd = doc.createPosition(committedTextEndIndex)
					Catch ble As BadLocationException
							latestCommittedTextEnd = Nothing
							latestCommittedTextStart = latestCommittedTextEnd
					End Try
				Else
						latestCommittedTextEnd = Nothing
						latestCommittedTextStart = latestCommittedTextEnd
				End If
			End If
		End Sub

		Private Sub createComposedTextAttribute(ByVal composedIndex As Integer, ByVal text As AttributedCharacterIterator)
			Dim doc As Document = document
			Dim strBuf As New StringBuilder

			' create attributed string with no attributes
			Dim c As Char = text.indexdex(composedIndex)
			Do While c <> CharacterIterator.DONE
				strBuf.Append(c)
				c = text.next()
			Loop

			composedTextContent = strBuf.ToString()
			composedTextAttribute = New SimpleAttributeSet
			composedTextAttribute.addAttribute(StyleConstants.ComposedTextAttribute, New AttributedString(text, composedIndex, text.endIndex))
		End Sub

		''' <summary>
		''' Saves composed text around the specified position.
		''' 
		''' The composed text (if any) around the specified position is saved
		''' in a backing store and removed from the document.
		''' </summary>
		''' <param name="pos">  document position to identify the composed text location </param>
		''' <returns>  {@code true} if the composed text exists and is saved,
		'''          {@code false} otherwise </returns>
		''' <seealso cref= #restoreComposedText
		''' @since 1.7 </seealso>
		Protected Friend Overridable Function saveComposedText(ByVal pos As Integer) As Boolean
			If composedTextExists() Then
				Dim start As Integer = composedTextStart.offset
				Dim len As Integer = composedTextEnd.offset - composedTextStart.offset
				If pos >= start AndAlso pos <= start + len Then
					Try
						document.remove(start, len)
						Return True
					Catch ble As BadLocationException
					End Try
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Restores composed text previously saved by {@code saveComposedText}.
		''' 
		''' The saved composed text is inserted back into the document. This method
		''' should be invoked only if {@code saveComposedText} returns {@code true}.
		''' </summary>
		''' <seealso cref= #saveComposedText
		''' @since 1.7 </seealso>
		Protected Friend Overridable Sub restoreComposedText()
			Dim doc As Document = document
			Try
				doc.insertString(caret.dot, composedTextContent, composedTextAttribute)
				composedTextStart = doc.createPosition(caret.dot - composedTextContent.Length)
				composedTextEnd = doc.createPosition(caret.dot)
			Catch ble As BadLocationException
			End Try
		End Sub

		'
		' Map committed text to an ActionEvent. If the committed text length is 1,
		' treat it as a KeyStroke, otherwise or there is no KeyStroke defined,
		' treat it just as a default action.
		'
		Private Sub mapCommittedTextToAction(ByVal committedText As String)
			Dim binding As Keymap = keymap
			If binding IsNot Nothing Then
				Dim a As Action = Nothing
				If committedText.Length = 1 Then
					Dim k As KeyStroke = KeyStroke.getKeyStroke(committedText.Chars(0))
					a = binding.getAction(k)
				End If

				If a Is Nothing Then a = binding.defaultAction

				If a IsNot Nothing Then
					Dim ae As New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, committedText, EventQueue.mostRecentEventTime, currentEventModifiers)
					a.actionPerformed(ae)
				End If
			End If
		End Sub

		'
		' Sets the caret position according to the passed input method
		' event. Also, sets/resets composed text caret appropriately.
		'
		Private Property inputMethodCaretPosition As InputMethodEvent
			Set(ByVal e As InputMethodEvent)
				Dim dot As Integer
    
				If composedTextExists() Then
					dot = composedTextStart.offset
					If Not(TypeOf caret Is ComposedTextCaret) Then
						If composedTextCaret Is Nothing Then composedTextCaret = New ComposedTextCaret(Me)
						originalCaret = caret
						' Sets composed text caret
						exchangeCaret(originalCaret, composedTextCaret)
					End If
    
					Dim caretPos As java.awt.font.TextHitInfo = e.caret
					If caretPos IsNot Nothing Then
						Dim index As Integer = caretPos.insertionIndex
						dot += index
						If index = 0 Then
							' Scroll the component if needed so that the composed text
							' becomes visible.
							Try
								Dim d As Rectangle = modelToView(dot)
								Dim [end] As Rectangle = modelToView(composedTextEnd.offset)
								Dim b As Rectangle = bounds
								d.x += Math.Min([end].x - d.x, b.width)
								scrollRectToVisible(d)
							Catch ble As BadLocationException
							End Try
						End If
					End If
					caret.dot = dot
				ElseIf TypeOf caret Is ComposedTextCaret Then
					dot = caret.dot
					' Restores original caret
					exchangeCaret(caret, originalCaret)
					caret.dot = dot
				End If
			End Set
		End Property

		Private Sub exchangeCaret(ByVal oldCaret As Caret, ByVal newCaret As Caret)
			Dim blinkRate As Integer = oldCaret.blinkRate
			caret = newCaret
			caret.blinkRate = blinkRate
			caret.visible = hasFocus()
		End Sub

		''' <summary>
		''' Returns true if KeyEvents should be synthesized from an InputEvent.
		''' </summary>
		Private Function shouldSynthensizeKeyEvents() As Boolean
			If Not checkedInputOverride Then
				' Checks whether the client code overrides processInputMethodEvent.
				' If it is overridden, need not to generate KeyTyped events for committed text.
				' If it's not, behave as an passive input method client.
				needToSendKeyTypedEvent = Not METHOD_OVERRIDDEN.get(Me.GetType())
				checkedInputOverride = True
			End If
			Return needToSendKeyTypedEvent
		End Function

		'
		' Checks whether a composed text in this text component
		'
		Friend Overridable Function composedTextExists() As Boolean
			Return (composedTextStart IsNot Nothing)
		End Function

		'
		' Caret implementation for editing the composed text.
		'
		<Serializable> _
		Friend Class ComposedTextCaret
			Inherits DefaultCaret

			Private ReadOnly outerInstance As JTextComponent

			Public Sub New(ByVal outerInstance As JTextComponent)
				Me.outerInstance = outerInstance
			End Sub

			Friend bg As Color

			'
			' Get the background color of the component
			'
			Public Overrides Sub install(ByVal c As JTextComponent)
				MyBase.install(c)

				Dim doc As Document = c.document
				If TypeOf doc Is StyledDocument Then
					Dim sDoc As StyledDocument = CType(doc, StyledDocument)
					Dim elem As Element = sDoc.getCharacterElement(c.composedTextStart.offset)
					Dim attr As AttributeSet = elem.attributes
					bg = sDoc.getBackground(attr)
				End If

				If bg Is Nothing Then bg = c.background
			End Sub

			'
			' Draw caret in XOR mode.
			'
			Public Overrides Sub paint(ByVal g As Graphics)
				If visible Then
					Try
						Dim r As Rectangle = component.modelToView(dot)
						g.xORMode = bg
						g.drawLine(r.x, r.y, r.x, r.y + r.height - 1)
						g.paintModeode()
					Catch e As BadLocationException
						' can't render I guess
						'System.err.println("Can't render cursor");
					End Try
				End If
			End Sub

			'
			' If some area other than the composed text is clicked by mouse,
			' issue endComposition() to force commit the composed text.
			'
			Protected Friend Overrides Sub positionCaret(ByVal [me] As MouseEvent)
				Dim host As JTextComponent = component
				Dim pt As New Point([me].x, [me].y)
				Dim offset As Integer = host.viewToModel(pt)
				Dim composedStartIndex As Integer = host.composedTextStart.offset
				If (offset < composedStartIndex) OrElse (offset > outerInstance.composedTextEnd.offset) Then
					Try
						' Issue endComposition
						Dim newPos As Position = host.document.createPosition(offset)
						host.inputContext.endComposition()

						' Post a caret positioning runnable to assure that the positioning
						' occurs *after* committing the composed text.
						EventQueue.invokeLater(New DoSetCaretPosition(host, newPos))
					Catch ble As BadLocationException
						Console.Error.WriteLine(ble)
					End Try
				Else
					' Normal processing
					MyBase.positionCaret([me])
				End If
			End Sub
		End Class

		'
		' Runnable class for invokeLater() to set caret position later.
		'
		Private Class DoSetCaretPosition
			Implements Runnable

			Private ReadOnly outerInstance As JTextComponent

			Friend host As JTextComponent
			Friend newPos As Position

			Friend Sub New(ByVal outerInstance As JTextComponent, ByVal host As JTextComponent, ByVal newPos As Position)
					Me.outerInstance = outerInstance
				Me.host = host
				Me.newPos = newPos
			End Sub

			Public Overridable Sub run()
				host.caretPosition = newPos.offset
			End Sub
		End Class
	End Class

End Namespace