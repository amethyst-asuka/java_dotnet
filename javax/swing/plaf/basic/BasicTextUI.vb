Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.text
Imports javax.swing.event

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
Namespace javax.swing.plaf.basic


	''' <summary>
	''' <p>
	''' Basis of a text components look-and-feel.  This provides the
	''' basic editor view and controller services that may be useful
	''' when creating a look-and-feel for an extension of
	''' <code>JTextComponent</code>.
	''' <p>
	''' Most state is held in the associated <code>JTextComponent</code>
	''' as bound properties, and the UI installs default values for the
	''' various properties.  This default will install something for
	''' all of the properties.  Typically, a LAF implementation will
	''' do more however.  At a minimum, a LAF would generally install
	''' key bindings.
	''' <p>
	''' This class also provides some concurrency support if the
	''' <code>Document</code> associated with the JTextComponent is a subclass of
	''' <code>AbstractDocument</code>.  Access to the View (or View hierarchy) is
	''' serialized between any thread mutating the model and the Swing
	''' event thread (which is expected to render, do model/view coordinate
	''' translation, etc).  <em>Any access to the root view should first
	''' acquire a read-lock on the AbstractDocument and release that lock
	''' in a finally block.</em>
	''' <p>
	''' An important method to define is the <seealso cref="#getPropertyPrefix"/> method
	''' which is used as the basis of the keys used to fetch defaults
	''' from the UIManager.  The string should reflect the type of
	''' TextUI (eg. TextField, TextArea, etc) without the particular
	''' LAF part of the name (eg Metal, Motif, etc).
	''' <p>
	''' To build a view of the model, one of the following strategies
	''' can be employed.
	''' <ol>
	''' <li>
	''' One strategy is to simply redefine the
	''' ViewFactory interface in the UI.  By default, this UI itself acts
	''' as the factory for View implementations.  This is useful
	''' for simple factories.  To do this reimplement the
	''' <seealso cref="#create"/> method.
	''' <li>
	''' A common strategy for creating more complex types of documents
	''' is to have the EditorKit implementation return a factory.  Since
	''' the EditorKit ties all of the pieces necessary to maintain a type
	''' of document, the factory is typically an important part of that
	''' and should be produced by the EditorKit implementation.
	''' </ol>
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
	''' @author Timothy Prinzing
	''' @author Shannon Hickey (drag and drop)
	''' </summary>
	Public MustInherit Class BasicTextUI
		Inherits TextUI
		Implements ViewFactory

		''' <summary>
		''' Creates a new UI.
		''' </summary>
		Public Sub New()
			painted = False
		End Sub

		''' <summary>
		''' Creates the object to use for a caret.  By default an
		''' instance of BasicCaret is created.  This method
		''' can be redefined to provide something else that implements
		''' the InputPosition interface or a subclass of JCaret.
		''' </summary>
		''' <returns> the caret object </returns>
		Protected Friend Overridable Function createCaret() As Caret
			Return New BasicCaret
		End Function

		''' <summary>
		''' Creates the object to use for adding highlights.  By default
		''' an instance of BasicHighlighter is created.  This method
		''' can be redefined to provide something else that implements
		''' the Highlighter interface or a subclass of DefaultHighlighter.
		''' </summary>
		''' <returns> the highlighter </returns>
		Protected Friend Overridable Function createHighlighter() As Highlighter
			Return New BasicHighlighter
		End Function

		''' <summary>
		''' Fetches the name of the keymap that will be installed/used
		''' by default for this UI. This is implemented to create a
		''' name based upon the classname.  The name is the the name
		''' of the class with the package prefix removed.
		''' </summary>
		''' <returns> the name </returns>
		Protected Friend Overridable Property keymapName As String
			Get
				Dim nm As String = Me.GetType().name
				Dim index As Integer = nm.LastIndexOf("."c)
				If index >= 0 Then nm = nm.Substring(index+1, nm.Length - (index+1))
				Return nm
			End Get
		End Property

		''' <summary>
		''' Creates the keymap to use for the text component, and installs
		''' any necessary bindings into it.  By default, the keymap is
		''' shared between all instances of this type of TextUI. The
		''' keymap has the name defined by the getKeymapName method.  If the
		''' keymap is not found, then DEFAULT_KEYMAP from JTextComponent is used.
		''' <p>
		''' The set of bindings used to create the keymap is fetched
		''' from the UIManager using a key formed by combining the
		''' <seealso cref="#getPropertyPrefix"/> method
		''' and the string <code>.keyBindings</code>.  The type is expected
		''' to be <code>JTextComponent.KeyBinding[]</code>.
		''' </summary>
		''' <returns> the keymap </returns>
		''' <seealso cref= #getKeymapName </seealso>
		''' <seealso cref= javax.swing.text.JTextComponent </seealso>
		Protected Friend Overridable Function createKeymap() As Keymap
			Dim nm As String = keymapName
			Dim map As Keymap = JTextComponent.getKeymap(nm)
			If map Is Nothing Then
				Dim parent As Keymap = JTextComponent.getKeymap(JTextComponent.DEFAULT_KEYMAP)
				map = JTextComponent.addKeymap(nm, parent)
				Dim prefix As String = propertyPrefix
				Dim o As Object = sun.swing.DefaultLookup.get(editor, Me, prefix & ".keyBindings")
				If (o IsNot Nothing) AndAlso (TypeOf o Is JTextComponent.KeyBinding()) Then
					Dim bindings As JTextComponent.KeyBinding() = CType(o, JTextComponent.KeyBinding())
					JTextComponent.loadKeymap(map, bindings, component.actions)
				End If
			End If
			Return map
		End Function

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to do nothing (i.e. the response to
		''' properties in JTextComponent itself are handled prior
		''' to calling this method).
		''' 
		''' This implementation updates the background of the text
		''' component if the editable and/or enabled state changes.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
			If evt.propertyName.Equals("editable") OrElse evt.propertyName.Equals("enabled") Then updateBackground(CType(evt.source, JTextComponent))
		End Sub

		''' <summary>
		''' Updates the background of the text component based on whether the
		''' text component is editable and/or enabled.
		''' </summary>
		''' <param name="c"> the JTextComponent that needs its background color updated </param>
		Private Sub updateBackground(ByVal c As JTextComponent)
			' This is a temporary workaround.
			' This code does not correctly deal with Synth (Synth doesn't use
			' properties like this), nor does it deal with the situation where
			' the developer grabs the color from a JLabel and sets it as
			' the background for a JTextArea in all look and feels. The problem
			' scenario results if the Color obtained for the Label and TextArea
			' is ==, which is the case for the windows look and feel.
			' Until an appropriate solution is found, the code is being
			' reverted to what it was before the original fix.
			If TypeOf Me Is javax.swing.plaf.synth.SynthUI OrElse (TypeOf c Is JTextArea) Then Return
			Dim background As Color = c.background
			If TypeOf background Is javax.swing.plaf.UIResource Then
				Dim prefix As String = propertyPrefix

				Dim disabledBG As Color = sun.swing.DefaultLookup.getColor(c, Me, prefix & ".disabledBackground", Nothing)
				Dim inactiveBG As Color = sun.swing.DefaultLookup.getColor(c, Me, prefix & ".inactiveBackground", Nothing)
				Dim bg As Color = sun.swing.DefaultLookup.getColor(c, Me, prefix & ".background", Nothing)

	'             In an ideal situation, the following check would not be necessary
	'             * and we would replace the color any time the previous color was a
	'             * UIResouce. However, it turns out that there is existing code that
	'             * uses the following inadvisable pattern to turn a text area into
	'             * what appears to be a multi-line label:
	'             *
	'             * JLabel label = new JLabel();
	'             * JTextArea area = new JTextArea();
	'             * area.setBackground(label.getBackground());
	'             * area.setEditable(false);
	'             *
	'             * JLabel's default background is a UIResource. As such, just
	'             * checking for UIResource would have us always changing the
	'             * background away from what the developer wanted.
	'             *
	'             * Therefore, for JTextArea/JEditorPane, we'll additionally check
	'             * that the color we're about to replace matches one that was
	'             * installed by us from the UIDefaults.
	'             
				If (TypeOf c Is JTextArea OrElse TypeOf c Is JEditorPane) AndAlso background IsNot disabledBG AndAlso background IsNot inactiveBG AndAlso background IsNot bg Then Return

				Dim newColor As Color = Nothing
				If Not c.enabled Then newColor = disabledBG
				If newColor Is Nothing AndAlso (Not c.editable) Then newColor = inactiveBG
				If newColor Is Nothing Then newColor = bg
				If newColor IsNot Nothing AndAlso newColor IsNot background Then c.background = newColor
			End If
		End Sub

		''' <summary>
		''' Gets the name used as a key to look up properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name </returns>
		Protected Friend MustOverride ReadOnly Property propertyPrefix As String

		''' <summary>
		''' Initializes component properties, such as font, foreground,
		''' background, caret color, selection color, selected text color,
		''' disabled text color, and border color.  The font, foreground, and
		''' background properties are only set if their current value is either null
		''' or a UIResource, other properties are set if the current
		''' value is null.
		''' </summary>
		''' <seealso cref= #uninstallDefaults </seealso>
		''' <seealso cref= #installUI </seealso>
		Protected Friend Overridable Sub installDefaults()
			Dim prefix As String = propertyPrefix
			Dim f As Font = editor.font
			If (f Is Nothing) OrElse (TypeOf f Is javax.swing.plaf.UIResource) Then editor.font = UIManager.getFont(prefix & ".font")

			Dim bg As Color = editor.background
			If (bg Is Nothing) OrElse (TypeOf bg Is javax.swing.plaf.UIResource) Then editor.background = UIManager.getColor(prefix & ".background")

			Dim fg As Color = editor.foreground
			If (fg Is Nothing) OrElse (TypeOf fg Is javax.swing.plaf.UIResource) Then editor.foreground = UIManager.getColor(prefix & ".foreground")

			Dim color As Color = editor.caretColor
			If (color Is Nothing) OrElse (TypeOf color Is javax.swing.plaf.UIResource) Then editor.caretColor = UIManager.getColor(prefix & ".caretForeground")

			Dim s As Color = editor.selectionColor
			If (s Is Nothing) OrElse (TypeOf s Is javax.swing.plaf.UIResource) Then editor.selectionColor = UIManager.getColor(prefix & ".selectionBackground")

			Dim sfg As Color = editor.selectedTextColor
			If (sfg Is Nothing) OrElse (TypeOf sfg Is javax.swing.plaf.UIResource) Then editor.selectedTextColor = UIManager.getColor(prefix & ".selectionForeground")

			Dim dfg As Color = editor.disabledTextColor
			If (dfg Is Nothing) OrElse (TypeOf dfg Is javax.swing.plaf.UIResource) Then editor.disabledTextColor = UIManager.getColor(prefix & ".inactiveForeground")

			Dim b As javax.swing.border.Border = editor.border
			If (b Is Nothing) OrElse (TypeOf b Is javax.swing.plaf.UIResource) Then editor.border = UIManager.getBorder(prefix & ".border")

			Dim margin As Insets = editor.margin
			If margin Is Nothing OrElse TypeOf margin Is javax.swing.plaf.UIResource Then editor.margin = UIManager.getInsets(prefix & ".margin")

			updateCursor()
		End Sub

		Private Sub installDefaults2()
			editor.addMouseListener(dragListener)
			editor.addMouseMotionListener(dragListener)

			Dim prefix As String = propertyPrefix

			Dim caret As Caret = editor.caret
			If caret Is Nothing OrElse TypeOf caret Is javax.swing.plaf.UIResource Then
				caret = createCaret()
				editor.caret = caret

				Dim rate As Integer = sun.swing.DefaultLookup.getInt(component, Me, prefix & ".caretBlinkRate", 500)
				caret.blinkRate = rate
			End If

			Dim highlighter As Highlighter = editor.highlighter
			If highlighter Is Nothing OrElse TypeOf highlighter Is javax.swing.plaf.UIResource Then editor.highlighter = createHighlighter()

			Dim th As TransferHandler = editor.transferHandler
			If th Is Nothing OrElse TypeOf th Is javax.swing.plaf.UIResource Then editor.transferHandler = transferHandler
		End Sub

		''' <summary>
		''' Sets the component properties that have not been explicitly overridden
		''' to {@code null}.  A property is considered overridden if its current
		''' value is not a {@code UIResource}.
		''' </summary>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #uninstallUI </seealso>
		Protected Friend Overridable Sub uninstallDefaults()
			editor.removeMouseListener(dragListener)
			editor.removeMouseMotionListener(dragListener)

			If TypeOf editor.caretColor Is javax.swing.plaf.UIResource Then editor.caretColor = Nothing

			If TypeOf editor.selectionColor Is javax.swing.plaf.UIResource Then editor.selectionColor = Nothing

			If TypeOf editor.disabledTextColor Is javax.swing.plaf.UIResource Then editor.disabledTextColor = Nothing

			If TypeOf editor.selectedTextColor Is javax.swing.plaf.UIResource Then editor.selectedTextColor = Nothing

			If TypeOf editor.border Is javax.swing.plaf.UIResource Then editor.border = Nothing

			If TypeOf editor.margin Is javax.swing.plaf.UIResource Then editor.margin = Nothing

			If TypeOf editor.caret Is javax.swing.plaf.UIResource Then editor.caret = Nothing

			If TypeOf editor.highlighter Is javax.swing.plaf.UIResource Then editor.highlighter = Nothing

			If TypeOf editor.transferHandler Is javax.swing.plaf.UIResource Then editor.transferHandler = Nothing

			If TypeOf editor.cursor Is javax.swing.plaf.UIResource Then editor.cursor = Nothing
		End Sub

		''' <summary>
		''' Installs listeners for the UI.
		''' </summary>
		Protected Friend Overridable Sub installListeners()
		End Sub

		''' <summary>
		''' Uninstalls listeners for the UI.
		''' </summary>
		Protected Friend Overridable Sub uninstallListeners()
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			' backward compatibility support... keymaps for the UI
			' are now installed in the more friendly input map.
			editor.keymap = createKeymap()

			Dim km As InputMap = inputMap
			If km IsNot Nothing Then SwingUtilities.replaceUIInputMap(editor, JComponent.WHEN_FOCUSED, km)

			Dim map As ActionMap = actionMap
			If map IsNot Nothing Then SwingUtilities.replaceUIActionMap(editor, map)

			updateFocusAcceleratorBinding(False)
		End Sub

		''' <summary>
		''' Get the InputMap to use for the UI.
		''' </summary>
		Friend Overridable Property inputMap As InputMap
			Get
				Dim map As InputMap = New InputMapUIResource
    
				Dim [shared] As InputMap = CType(sun.swing.DefaultLookup.get(editor, Me, propertyPrefix & ".focusInputMap"), InputMap)
				If [shared] IsNot Nothing Then map.parent = [shared]
				Return map
			End Get
		End Property

		''' <summary>
		''' Invoked when the focus accelerator changes, this will update the
		''' key bindings as necessary.
		''' </summary>
		Friend Overridable Sub updateFocusAcceleratorBinding(ByVal changed As Boolean)
			Dim accelerator As Char = editor.focusAccelerator

			If changed OrElse accelerator <> ControlChars.NullChar Then
				Dim km As InputMap = SwingUtilities.getUIInputMap(editor, JComponent.WHEN_IN_FOCUSED_WINDOW)

				If km Is Nothing AndAlso accelerator <> ControlChars.NullChar Then
					km = New ComponentInputMapUIResource(editor)
					SwingUtilities.replaceUIInputMap(editor, JComponent.WHEN_IN_FOCUSED_WINDOW, km)
					Dim am As ActionMap = actionMap
					SwingUtilities.replaceUIActionMap(editor, am)
				End If
				If km IsNot Nothing Then
					km.clear()
					If accelerator <> ControlChars.NullChar Then km.put(KeyStroke.getKeyStroke(accelerator, BasicLookAndFeel.focusAcceleratorKeyMask), "requestFocus")
				End If
			End If
		End Sub


		''' <summary>
		''' Invoked when editable property is changed.
		''' 
		''' removing 'TAB' and 'SHIFT-TAB' from traversalKeysSet in case
		''' editor is editable
		''' adding 'TAB' and 'SHIFT-TAB' to traversalKeysSet in case
		''' editor is non editable
		''' </summary>

		Friend Overridable Sub updateFocusTraversalKeys()
	'        
	'         * Fix for 4514331 Non-editable JTextArea and similar
	'         * should allow Tab to keyboard - accessibility
	'         
			Dim ___editorKit As EditorKit = getEditorKit(editor)
			If ___editorKit IsNot Nothing AndAlso TypeOf ___editorKit Is DefaultEditorKit Then
				Dim storedForwardTraversalKeys As [Set](Of AWTKeyStroke) = editor.getFocusTraversalKeys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS)
				Dim storedBackwardTraversalKeys As [Set](Of AWTKeyStroke) = editor.getFocusTraversalKeys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS)
				Dim forwardTraversalKeys As [Set](Of AWTKeyStroke) = New HashSet(Of AWTKeyStroke)(storedForwardTraversalKeys)
				Dim backwardTraversalKeys As [Set](Of AWTKeyStroke) = New HashSet(Of AWTKeyStroke)(storedBackwardTraversalKeys)
				If editor.editable Then
					forwardTraversalKeys.remove(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, 0))
					backwardTraversalKeys.remove(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.SHIFT_MASK))
				Else
					forwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, 0))
					backwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.SHIFT_MASK))
				End If
				LookAndFeel.installProperty(editor, "focusTraversalKeysForward", forwardTraversalKeys)
				LookAndFeel.installProperty(editor, "focusTraversalKeysBackward", backwardTraversalKeys)
			End If

		End Sub

		''' <summary>
		''' As needed updates cursor for the target editor.
		''' </summary>
		Private Sub updateCursor()
			If ((Not editor.cursorSet)) OrElse TypeOf editor.cursor Is javax.swing.plaf.UIResource Then
				Dim cursor As Cursor = If(editor.editable, textCursor, Nothing)
				editor.cursor = cursor
			End If
		End Sub

		''' <summary>
		''' Returns the <code>TransferHandler</code> that will be installed if
		''' their isn't one installed on the <code>JTextComponent</code>.
		''' </summary>
		Friend Overridable Property transferHandler As TransferHandler
			Get
				Return defaultTransferHandler
			End Get
		End Property

		''' <summary>
		''' Fetch an action map to use.
		''' </summary>
		Friend Overridable Property actionMap As ActionMap
			Get
				Dim mapName As String = propertyPrefix & ".actionMap"
				Dim map As ActionMap = CType(UIManager.get(mapName), ActionMap)
    
				If map Is Nothing Then
					map = createActionMap()
					If map IsNot Nothing Then UIManager.lookAndFeelDefaults(mapName) = map
				End If
				Dim componentMap As ActionMap = New ActionMapUIResource
				componentMap.put("requestFocus", New FocusAction(Me))
		'        
		'         * fix for bug 4515750
		'         * JTextField & non-editable JTextArea bind return key - default btn not accessible
		'         *
		'         * Wrap the return action so that it is only enabled when the
		'         * component is editable. This allows the default button to be
		'         * processed when the text component has focus and isn't editable.
		'         *
		'         
				If TypeOf getEditorKit(editor) Is DefaultEditorKit Then
					If map IsNot Nothing Then
						Dim obj As Object = map.get(DefaultEditorKit.insertBreakAction)
						If obj IsNot Nothing AndAlso TypeOf obj Is DefaultEditorKit.InsertBreakAction Then
							Dim action As Action = New TextActionWrapper(Me, CType(obj, TextAction))
							componentMap.put(action.getValue(Action.NAME),action)
						End If
					End If
				End If
				If map IsNot Nothing Then componentMap.parent = map
				Return componentMap
			End Get
		End Property

		''' <summary>
		''' Create a default action map.  This is basically the
		''' set of actions found exported by the component.
		''' </summary>
		Friend Overridable Function createActionMap() As ActionMap
			Dim map As ActionMap = New ActionMapUIResource
			Dim actions As Action() = editor.actions
			'System.out.println("building map for UI: " + getPropertyPrefix());
			Dim n As Integer = actions.Length
			For i As Integer = 0 To n - 1
				Dim a As Action = actions(i)
				map.put(a.getValue(Action.NAME), a)
				'System.out.println("  " + a.getValue(Action.NAME));
			Next i
			map.put(TransferHandler.cutAction.getValue(Action.NAME), TransferHandler.cutAction)
			map.put(TransferHandler.copyAction.getValue(Action.NAME), TransferHandler.copyAction)
			map.put(TransferHandler.pasteAction.getValue(Action.NAME), TransferHandler.pasteAction)
			Return map
		End Function

		Protected Friend Overridable Sub uninstallKeyboardActions()
			editor.keymap = Nothing
			SwingUtilities.replaceUIInputMap(editor, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(editor, Nothing)
		End Sub

		''' <summary>
		''' Paints a background for the view.  This will only be
		''' called if isOpaque() on the associated component is
		''' true.  The default is to paint the background color
		''' of the component.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		Protected Friend Overridable Sub paintBackground(ByVal g As Graphics)
			g.color = editor.background
			g.fillRect(0, 0, editor.width, editor.height)
		End Sub

		''' <summary>
		''' Fetches the text component associated with this
		''' UI implementation.  This will be null until
		''' the ui has been installed.
		''' </summary>
		''' <returns> the editor component </returns>
		Protected Friend Property component As JTextComponent
			Get
				Return editor
			End Get
		End Property

		''' <summary>
		''' Flags model changes.
		''' This is called whenever the model has changed.
		''' It is implemented to rebuild the view hierarchy
		''' to represent the default root element of the
		''' associated model.
		''' </summary>
		Protected Friend Overridable Sub modelChanged()
			' create a view hierarchy
			Dim f As ViewFactory = rootView.viewFactory
			Dim doc As Document = editor.document
			Dim elem As Element = doc.defaultRootElement
			view = f.create(elem)
		End Sub

		''' <summary>
		''' Sets the current root of the view hierarchy and calls invalidate().
		''' If there were any child components, they will be removed (i.e.
		''' there are assumed to have come from components embedded in views).
		''' </summary>
		''' <param name="v"> the root view </param>
		Protected Friend Property view As View
			Set(ByVal v As View)
				rootView.view = v
				painted = False
				editor.revalidate()
				editor.repaint()
			End Set
		End Property

		''' <summary>
		''' Paints the interface safely with a guarantee that
		''' the model won't change from the view of this thread.
		''' This does the following things, rendering from
		''' back to front.
		''' <ol>
		''' <li>
		''' If the component is marked as opaque, the background
		''' is painted in the current background color of the
		''' component.
		''' <li>
		''' The highlights (if any) are painted.
		''' <li>
		''' The view hierarchy is painted.
		''' <li>
		''' The caret is painted.
		''' </ol>
		''' </summary>
		''' <param name="g"> the graphics context </param>
		Protected Friend Overridable Sub paintSafely(ByVal g As Graphics)
			painted = True
			Dim highlighter As Highlighter = editor.highlighter
			Dim caret As Caret = editor.caret

			' paint the background
			If editor.opaque Then paintBackground(g)

			' paint the highlights
			If highlighter IsNot Nothing Then highlighter.paint(g)

			' paint the view hierarchy
			Dim alloc As Rectangle = visibleEditorRect
			If alloc IsNot Nothing Then rootView.paint(g, alloc)

			' paint the caret
			If caret IsNot Nothing Then caret.paint(g)

			If dropCaret IsNot Nothing Then dropCaret.paint(g)
		End Sub

		' --- ComponentUI methods --------------------------------------------

		''' <summary>
		''' Installs the UI for a component.  This does the following
		''' things.
		''' <ol>
		''' <li>
		''' Sets the associated component to opaque if the opaque property
		''' has not already been set by the client program. This will cause the
		''' component's background color to be painted.
		''' <li>
		''' Installs the default caret and highlighter into the
		''' associated component. These properties are only set if their
		''' current value is either {@code null} or an instance of
		''' <seealso cref="UIResource"/>.
		''' <li>
		''' Attaches to the editor and model.  If there is no
		''' model, a default one is created.
		''' <li>
		''' Creates the view factory and the view hierarchy used
		''' to represent the model.
		''' </ol>
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <seealso cref= ComponentUI#installUI </seealso>
		Public Overridable Sub installUI(ByVal c As JComponent)
			If TypeOf c Is JTextComponent Then
				editor = CType(c, JTextComponent)

				' common case is background painted... this can
				' easily be changed by subclasses or from outside
				' of the component.
				LookAndFeel.installProperty(editor, "opaque", Boolean.TRUE)
				LookAndFeel.installProperty(editor, "autoscrolls", Boolean.TRUE)

				' install defaults
				installDefaults()
				installDefaults2()

				' attach to the model and editor
				editor.addPropertyChangeListener(updateHandler)
				Dim doc As Document = editor.document
				If doc Is Nothing Then
					' no model, create a default one.  This will
					' fire a notification to the updateHandler
					' which takes care of the rest.
					editor.document = getEditorKit(editor).createDefaultDocument()
				Else
					doc.addDocumentListener(updateHandler)
					modelChanged()
				End If

				' install keymap
				installListeners()
				installKeyboardActions()

				Dim oldLayout As LayoutManager = editor.layout
				If (oldLayout Is Nothing) OrElse (TypeOf oldLayout Is javax.swing.plaf.UIResource) Then editor.layout = updateHandler

				updateBackground(editor)
			Else
				Throw New Exception("TextUI needs JTextComponent")
			End If
		End Sub

		''' <summary>
		''' Deinstalls the UI for a component.  This removes the listeners,
		''' uninstalls the highlighter, removes views, and nulls out the keymap.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <seealso cref= ComponentUI#uninstallUI </seealso>
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			' detach from the model
			editor.removePropertyChangeListener(updateHandler)
			editor.document.removeDocumentListener(updateHandler)

			' view part
			painted = False
			uninstallDefaults()
			rootView.view = Nothing
			c.removeAll()
			Dim lm As LayoutManager = c.layout
			If TypeOf lm Is javax.swing.plaf.UIResource Then c.layout = Nothing

			' controller part
			uninstallKeyboardActions()
			uninstallListeners()

			editor = Nothing
		End Sub

		''' <summary>
		''' Superclass paints background in an uncontrollable way
		''' (i.e. one might want an image tiled into the background).
		''' To prevent this from happening twice, this method is
		''' reimplemented to simply paint.
		''' <p>
		''' <em>NOTE:</em> NOTE: Superclass is also not thread-safe in its
		''' rendering of the background, although that is not an issue with the
		''' default rendering.
		''' </summary>
		Public Overridable Sub update(ByVal g As Graphics, ByVal c As JComponent)
			paint(g, c)
		End Sub

		''' <summary>
		''' Paints the interface.  This is routed to the
		''' paintSafely method under the guarantee that
		''' the model won't change from the view of this thread
		''' while it's rendering (if the associated model is
		''' derived from AbstractDocument).  This enables the
		''' model to potentially be updated asynchronously.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="c"> the editor component </param>
		Public Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			If (rootView.viewCount > 0) AndAlso (rootView.getView(0) IsNot Nothing) Then
				Dim doc As Document = editor.document
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
				Try
					paintSafely(g)
				Finally
					If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
				End Try
			End If
		End Sub

		''' <summary>
		''' Gets the preferred size for the editor component.  If the component
		''' has been given a size prior to receiving this request, it will
		''' set the size of the view hierarchy to reflect the size of the component
		''' before requesting the preferred size of the view hierarchy.  This
		''' allows formatted views to format to the current component size before
		''' answering the request.  Other views don't care about currently formatted
		''' size and give the same answer either way.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <returns> the size </returns>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim doc As Document = editor.document
			Dim i As Insets = c.insets
			Dim d As Dimension = c.size

			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				If (d.width > (i.left + i.right)) AndAlso (d.height > (i.top + i.bottom)) Then
					rootView.sizeize(d.width - i.left - i.right, d.height - i.top - i.bottom)
				ElseIf d.width = 0 AndAlso d.height = 0 Then
					' Probably haven't been layed out yet, force some sort of
					' initial sizing.
					rootView.sizeize(Integer.MaxValue, Integer.MaxValue)
				End If
				d.width = CInt(Fix(Math.Min(CLng(Fix(rootView.getPreferredSpan(View.X_AXIS))) + CLng(Fix(i.left)) + CLng(Fix(i.right)), Integer.MaxValue)))
				d.height = CInt(Fix(Math.Min(CLng(Fix(rootView.getPreferredSpan(View.Y_AXIS))) + CLng(Fix(i.top)) + CLng(Fix(i.bottom)), Integer.MaxValue)))
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return d
		End Function

		''' <summary>
		''' Gets the minimum size for the editor component.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <returns> the size </returns>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim doc As Document = editor.document
			Dim i As Insets = c.insets
			Dim d As New Dimension
			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				d.width = CInt(Fix(rootView.getMinimumSpan(View.X_AXIS))) + i.left + i.right
				d.height = CInt(Fix(rootView.getMinimumSpan(View.Y_AXIS))) + i.top + i.bottom
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return d
		End Function

		''' <summary>
		''' Gets the maximum size for the editor component.
		''' </summary>
		''' <param name="c"> the editor component </param>
		''' <returns> the size </returns>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim doc As Document = editor.document
			Dim i As Insets = c.insets
			Dim d As New Dimension
			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				d.width = CInt(Fix(Math.Min(CLng(Fix(rootView.getMaximumSpan(View.X_AXIS))) + CLng(Fix(i.left)) + CLng(Fix(i.right)), Integer.MaxValue)))
				d.height = CInt(Fix(Math.Min(CLng(Fix(rootView.getMaximumSpan(View.Y_AXIS))) + CLng(Fix(i.top)) + CLng(Fix(i.bottom)), Integer.MaxValue)))
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return d
		End Function

		' ---- TextUI methods -------------------------------------------


		''' <summary>
		''' Gets the allocation to give the root View.  Due
		''' to an unfortunate set of historical events this
		''' method is inappropriately named.  The Rectangle
		''' returned has nothing to do with visibility.
		''' The component must have a non-zero positive size for
		''' this translation to be computed.
		''' </summary>
		''' <returns> the bounding box for the root view </returns>
		Protected Friend Overridable Property visibleEditorRect As Rectangle
			Get
				Dim alloc As Rectangle = editor.bounds
				If (alloc.width > 0) AndAlso (alloc.height > 0) Then
						alloc.y = 0
						alloc.x = alloc.y
					Dim insets As Insets = editor.insets
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
		''' Converts the given location in the model to a place in
		''' the view coordinate system.
		''' The component must have a non-zero positive size for
		''' this translation to be computed.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <param name="pos"> the local location in the model to translate &gt;= 0 </param>
		''' <returns> the coordinates as a rectangle, null if the model is not painted </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= TextUI#modelToView </seealso>
		Public Overrides Function modelToView(ByVal tc As JTextComponent, ByVal pos As Integer) As Rectangle
			Return modelToView(tc, pos, Position.Bias.Forward)
		End Function

		''' <summary>
		''' Converts the given location in the model to a place in
		''' the view coordinate system.
		''' The component must have a non-zero positive size for
		''' this translation to be computed.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <param name="pos"> the local location in the model to translate &gt;= 0 </param>
		''' <returns> the coordinates as a rectangle, null if the model is not painted </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= TextUI#modelToView </seealso>
		Public Overrides Function modelToView(ByVal tc As JTextComponent, ByVal pos As Integer, ByVal bias As Position.Bias) As Rectangle
			Dim doc As Document = editor.document
			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				Dim alloc As Rectangle = visibleEditorRect
				If alloc IsNot Nothing Then
					rootView.sizeize(alloc.width, alloc.height)
					Dim s As Shape = rootView.modelToView(pos, alloc, bias)
					If s IsNot Nothing Then Return s.bounds
				End If
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return Nothing
		End Function

		''' <summary>
		''' Converts the given place in the view coordinate system
		''' to the nearest representative location in the model.
		''' The component must have a non-zero positive size for
		''' this translation to be computed.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <param name="pt"> the location in the view to translate.  This
		'''  should be in the same coordinate system as the mouse events. </param>
		''' <returns> the offset from the start of the document &gt;= 0,
		'''   -1 if not painted </returns>
		''' <seealso cref= TextUI#viewToModel </seealso>
		Public Overridable Function viewToModel(ByVal tc As JTextComponent, ByVal pt As Point) As Integer
			Return viewToModel(tc, pt, discardBias)
		End Function

		''' <summary>
		''' Converts the given place in the view coordinate system
		''' to the nearest representative location in the model.
		''' The component must have a non-zero positive size for
		''' this translation to be computed.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <param name="pt"> the location in the view to translate.  This
		'''  should be in the same coordinate system as the mouse events. </param>
		''' <returns> the offset from the start of the document &gt;= 0,
		'''   -1 if the component doesn't yet have a positive size. </returns>
		''' <seealso cref= TextUI#viewToModel </seealso>
		Public Overridable Function viewToModel(ByVal tc As JTextComponent, ByVal pt As Point, ByVal biasReturn As Position.Bias()) As Integer
			Dim offs As Integer = -1
			Dim doc As Document = editor.document
			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				Dim alloc As Rectangle = visibleEditorRect
				If alloc IsNot Nothing Then
					rootView.sizeize(alloc.width, alloc.height)
					offs = rootView.viewToModel(pt.x, pt.y, alloc, biasReturn)
				End If
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return offs
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getNextVisualPositionFrom(ByVal t As JTextComponent, ByVal pos As Integer, ByVal b As Position.Bias, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Dim doc As Document = editor.document
			If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
			Try
				If painted Then
					Dim alloc As Rectangle = visibleEditorRect
					If alloc IsNot Nothing Then rootView.sizeize(alloc.width, alloc.height)
					Return rootView.getNextVisualPositionFrom(pos, b, alloc, direction, biasRet)
				End If
			Finally
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
			Return -1
		End Function

		''' <summary>
		''' Causes the portion of the view responsible for the
		''' given part of the model to be repainted.  Does nothing if
		''' the view is not currently painted.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		''' <seealso cref= TextUI#damageRange </seealso>
		Public Overrides Sub damageRange(ByVal tc As JTextComponent, ByVal p0 As Integer, ByVal p1 As Integer)
			damageRange(tc, p0, p1, Position.Bias.Forward, Position.Bias.Backward)
		End Sub

		''' <summary>
		''' Causes the portion of the view responsible for the
		''' given part of the model to be repainted.
		''' </summary>
		''' <param name="p0"> the beginning of the range &gt;= 0 </param>
		''' <param name="p1"> the end of the range &gt;= p0 </param>
		Public Overrides Sub damageRange(ByVal t As JTextComponent, ByVal p0 As Integer, ByVal p1 As Integer, ByVal p0Bias As Position.Bias, ByVal p1Bias As Position.Bias)
			If painted Then
				Dim alloc As Rectangle = visibleEditorRect
				If alloc IsNot Nothing Then
					Dim doc As Document = t.document
					If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
					Try
						rootView.sizeize(alloc.width, alloc.height)
						Dim toDamage As Shape = rootView.modelToView(p0, p0Bias, p1, p1Bias, alloc)
						Dim rect As Rectangle = If(TypeOf toDamage Is Rectangle, CType(toDamage, Rectangle), toDamage.bounds)
						editor.repaint(rect.x, rect.y, rect.width, rect.height)
					Catch e As BadLocationException
					Finally
						If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
					End Try
				End If
			End If
		End Sub

		''' <summary>
		''' Fetches the EditorKit for the UI.
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <returns> the editor capabilities </returns>
		''' <seealso cref= TextUI#getEditorKit </seealso>
		Public Overrides Function getEditorKit(ByVal tc As JTextComponent) As EditorKit
			Return defaultKit
		End Function

		''' <summary>
		''' Fetches a View with the allocation of the associated
		''' text component (i.e. the root of the hierarchy) that
		''' can be traversed to determine how the model is being
		''' represented spatially.
		''' <p>
		''' <font color=red><b>NOTE:</b>The View hierarchy can
		''' be traversed from the root view, and other things
		''' can be done as well.  Things done in this way cannot
		''' be protected like simple method calls through the TextUI.
		''' Therefore, proper operation in the presence of concurrency
		''' must be arranged by any logic that calls this method!
		''' </font>
		''' </summary>
		''' <param name="tc"> the text component for which this UI is installed </param>
		''' <returns> the view </returns>
		''' <seealso cref= TextUI#getRootView </seealso>
		Public Overrides Function getRootView(ByVal tc As JTextComponent) As View
			Return rootView
		End Function


		''' <summary>
		''' Returns the string to be used as the tooltip at the passed in location.
		''' This forwards the method onto the root View.
		''' </summary>
		''' <seealso cref= javax.swing.text.JTextComponent#getToolTipText </seealso>
		''' <seealso cref= javax.swing.text.View#getToolTipText
		''' @since 1.4 </seealso>
		Public Overridable Function getToolTipText(ByVal t As JTextComponent, ByVal pt As Point) As String
			If Not painted Then Return Nothing
			Dim doc As Document = editor.document
			Dim tt As String = Nothing
			Dim alloc As Rectangle = visibleEditorRect

			If alloc IsNot Nothing Then
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
				Try
					tt = rootView.getToolTipText(pt.x, pt.y, alloc)
				Finally
					If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
				End Try
			End If
			Return tt
		End Function

		' --- ViewFactory methods ------------------------------

		''' <summary>
		''' Creates a view for an element.
		''' If a subclass wishes to directly implement the factory
		''' producing the view(s), it should reimplement this
		''' method.  By default it simply returns null indicating
		''' it is unable to represent the element.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the view </returns>
		Public Overridable Function create(ByVal elem As Element) As View Implements ViewFactory.create
			Return Nothing
		End Function

		''' <summary>
		''' Creates a view for an element.
		''' If a subclass wishes to directly implement the factory
		''' producing the view(s), it should reimplement this
		''' method.  By default it simply returns null indicating
		''' it is unable to represent the part of the element.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <param name="p0"> the starting offset &gt;= 0 </param>
		''' <param name="p1"> the ending offset &gt;= p0 </param>
		''' <returns> the view </returns>
		Public Overridable Function create(ByVal elem As Element, ByVal p0 As Integer, ByVal p1 As Integer) As View
			Return Nothing
		End Function

		Public Class BasicCaret
			Inherits DefaultCaret
			Implements javax.swing.plaf.UIResource

		End Class

		Public Class BasicHighlighter
			Inherits DefaultHighlighter
			Implements javax.swing.plaf.UIResource

		End Class

		Friend Class BasicCursor
			Inherits Cursor
			Implements javax.swing.plaf.UIResource

			Friend Sub New(ByVal type As Integer)
				MyBase.New(type)
			End Sub

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub
		End Class

		Private Shared textCursor As New BasicCursor(Cursor.TEXT_CURSOR)
		' ----- member variables ---------------------------------------

		Private Shared ReadOnly defaultKit As EditorKit = New DefaultEditorKit
		<NonSerialized> _
		Friend editor As JTextComponent
		<NonSerialized> _
		Friend painted As Boolean
		<NonSerialized> _
		Friend rootView As New RootView(Me)
		<NonSerialized> _
		Friend updateHandler As New UpdateHandler(Me)
		Private Shared ReadOnly defaultTransferHandler As TransferHandler = New TextTransferHandler
		Private ReadOnly dragListener As DragListener = dragListener
		Private Shared ReadOnly discardBias As Position.Bias() = New Position.Bias(0){}
		Private dropCaret As DefaultCaret

		''' <summary>
		''' Root view that acts as a gateway between the component
		''' and the View hierarchy.
		''' </summary>
		Friend Class RootView
			Inherits View

			Private ReadOnly outerInstance As BasicTextUI


			Friend Sub New(ByVal outerInstance As BasicTextUI)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing)
			End Sub

			Friend Overridable Property view As View
				Set(ByVal v As View)
					Dim oldView As View = view
					view = Nothing
					If oldView IsNot Nothing Then oldView.parent = Nothing
					If v IsNot Nothing Then v.parent = Me
					view = v
				End Set
			End Property

			''' <summary>
			''' Fetches the attributes to use when rendering.  At the root
			''' level there are no attributes.  If an attribute is resolved
			''' up the view hierarchy this is the end of the line.
			''' </summary>
			Public Property Overrides attributes As AttributeSet
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Determines the preferred span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
				If view IsNot Nothing Then Return view.getPreferredSpan(axis)
				Return 10
			End Function

			''' <summary>
			''' Determines the minimum span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
				If view IsNot Nothing Then Return view.getMinimumSpan(axis)
				Return 10
			End Function

			''' <summary>
			''' Determines the maximum span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
				Return Integer.MaxValue
			End Function

			''' <summary>
			''' Specifies that a preference has changed.
			''' Child views can call this on the parent to indicate that
			''' the preference has changed.  The root view routes this to
			''' invalidate on the hosting component.
			''' <p>
			''' This can be called on a different thread from the
			''' event dispatching thread and is basically unsafe to
			''' propagate into the component.  To make this safe,
			''' the operation is transferred over to the event dispatching
			''' thread for completion.  It is a design goal that all view
			''' methods be safe to call without concern for concurrency,
			''' and this behavior helps make that true.
			''' </summary>
			''' <param name="child"> the child view </param>
			''' <param name="width"> true if the width preference has changed </param>
			''' <param name="height"> true if the height preference has changed </param>
			Public Overrides Sub preferenceChanged(ByVal child As View, ByVal width As Boolean, ByVal height As Boolean)
				outerInstance.editor.revalidate()
			End Sub

			''' <summary>
			''' Determines the desired alignment for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the desired alignment, where 0.0 indicates the origin
			'''     and 1.0 the full span away from the origin </returns>
			Public Overrides Function getAlignment(ByVal axis As Integer) As Single
				If view IsNot Nothing Then Return view.getAlignment(axis)
				Return 0
			End Function

			''' <summary>
			''' Renders the view.
			''' </summary>
			''' <param name="g"> the graphics context </param>
			''' <param name="allocation"> the region to render into </param>
			Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
				If view IsNot Nothing Then
					Dim alloc As Rectangle = If(TypeOf allocation Is Rectangle, CType(allocation, Rectangle), allocation.bounds)
					sizeize(alloc.width, alloc.height)
					view.paint(g, allocation)
				End If
			End Sub

			''' <summary>
			''' Sets the view parent.
			''' </summary>
			''' <param name="parent"> the parent view </param>
			Public Overrides Property parent As View
				Set(ByVal parent As View)
					Throw New Exception("Can't set parent on root view")
				End Set
			End Property

			''' <summary>
			''' Returns the number of views in this view.  Since
			''' this view simply wraps the root of the view hierarchy
			''' it has exactly one child.
			''' </summary>
			''' <returns> the number of views </returns>
			''' <seealso cref= #getView </seealso>
			Public Property Overrides viewCount As Integer
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Gets the n-th view in this container.
			''' </summary>
			''' <param name="n"> the number of the view to get </param>
			''' <returns> the view </returns>
			Public Overrides Function getView(ByVal n As Integer) As View
				Return view
			End Function

			''' <summary>
			''' Returns the child view index representing the given position in
			''' the model.  This is implemented to return the index of the only
			''' child.
			''' </summary>
			''' <param name="pos"> the position &gt;= 0 </param>
			''' <returns>  index of the view representing the given position, or
			'''   -1 if no view represents that position
			''' @since 1.3 </returns>
			Public Overrides Function getViewIndex(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
				Return 0
			End Function

			''' <summary>
			''' Fetches the allocation for the given child view.
			''' This enables finding out where various views
			''' are located, without assuming the views store
			''' their location.  This returns the given allocation
			''' since this view simply acts as a gateway between
			''' the view hierarchy and the associated component.
			''' </summary>
			''' <param name="index"> the index of the child </param>
			''' <param name="a">  the allocation to this view. </param>
			''' <returns> the allocation to the child </returns>
			Public Overrides Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
				Return a
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="pos"> the position to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position </returns>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				If view IsNot Nothing Then Return view.modelToView(pos, a, b)
				Return Nothing
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="p0"> the position to convert &gt;= 0 </param>
			''' <param name="b0"> the bias toward the previous character or the
			'''  next character represented by p0, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="p1"> the position to convert &gt;= 0 </param>
			''' <param name="b1"> the bias toward the previous character or the
			'''  next character represented by p1, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position is returned </returns>
			''' <exception cref="BadLocationException">  if the given position does
			'''   not represent a valid location in the associated document </exception>
			''' <exception cref="IllegalArgumentException"> for an invalid bias argument </exception>
			''' <seealso cref= View#viewToModel </seealso>
			Public Overrides Function modelToView(ByVal p0 As Integer, ByVal b0 As Position.Bias, ByVal p1 As Integer, ByVal b1 As Position.Bias, ByVal a As Shape) As Shape
				If view IsNot Nothing Then Return view.modelToView(p0, b0, p1, b1, a)
				Return Nothing
			End Function

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.
			''' </summary>
			''' <param name="x"> x coordinate of the view location to convert </param>
			''' <param name="y"> y coordinate of the view location to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the location within the model that best represents the
			'''    given point in the view </returns>
			Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
				If view IsNot Nothing Then
					Dim retValue As Integer = view.viewToModel(x, y, a, bias)
					Return retValue
				End If
				Return -1
			End Function

			''' <summary>
			''' Provides a way to determine the next visually represented model
			''' location that one might place a caret.  Some views may not be visible,
			''' they might not be in the same order found in the model, or they just
			''' might not allow access to some of the locations in the model.
			''' This method enables specifying a position to convert
			''' within the range of &gt;=0.  If the value is -1, a position
			''' will be calculated automatically.  If the value &lt; -1,
			''' the {@code BadLocationException} will be thrown.
			''' </summary>
			''' <param name="pos"> the position to convert &gt;= 0 </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <param name="direction"> the direction from the current position that can
			'''  be thought of as the arrow keys typically found on a keyboard.
			'''  This may be SwingConstants.WEST, SwingConstants.EAST,
			'''  SwingConstants.NORTH, or SwingConstants.SOUTH. </param>
			''' <returns> the location within the model that best represents the next
			'''  location visual position. </returns>
			''' <exception cref="BadLocationException"> the given position is not a valid
			'''                                 position within the document </exception>
			''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
			Public Overrides Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
				If pos < -1 Then Throw New BadLocationException("invalid position", pos)
				If view IsNot Nothing Then
					Dim nextPos As Integer = view.getNextVisualPositionFrom(pos, b, a, direction, biasRet)
					If nextPos <> -1 Then
						pos = nextPos
					Else
						biasRet(0) = b
					End If
				End If
				Return pos
			End Function

			''' <summary>
			''' Gives notification that something was inserted into the document
			''' in a location that this view is responsible for.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			Public Overrides Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If view IsNot Nothing Then view.insertUpdate(e, a, f)
			End Sub

			''' <summary>
			''' Gives notification that something was removed from the document
			''' in a location that this view is responsible for.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			Public Overrides Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If view IsNot Nothing Then view.removeUpdate(e, a, f)
			End Sub

			''' <summary>
			''' Gives notification from the document that attributes were changed
			''' in a location that this view is responsible for.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				If view IsNot Nothing Then view.changedUpdate(e, a, f)
			End Sub

			''' <summary>
			''' Returns the document model underlying the view.
			''' </summary>
			''' <returns> the model </returns>
			Public Property Overrides document As Document
				Get
					Return outerInstance.editor.document
				End Get
			End Property

			''' <summary>
			''' Returns the starting offset into the model for this view.
			''' </summary>
			''' <returns> the starting offset </returns>
			Public Property Overrides startOffset As Integer
				Get
					If view IsNot Nothing Then Return view.startOffset
					Return element.startOffset
				End Get
			End Property

			''' <summary>
			''' Returns the ending offset into the model for this view.
			''' </summary>
			''' <returns> the ending offset </returns>
			Public Property Overrides endOffset As Integer
				Get
					If view IsNot Nothing Then Return view.endOffset
					Return element.endOffset
				End Get
			End Property

			''' <summary>
			''' Gets the element that this view is mapped to.
			''' </summary>
			''' <returns> the view </returns>
			Public Property Overrides element As Element
				Get
					If view IsNot Nothing Then Return view.element
					Return outerInstance.editor.document.defaultRootElement
				End Get
			End Property

			''' <summary>
			''' Breaks this view on the given axis at the given length.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <param name="len"> specifies where a break is desired in the span </param>
			''' <param name="the"> current allocation of the view </param>
			''' <returns> the fragment of the view that represents the given span
			'''   if the view can be broken, otherwise null </returns>
			Public Overridable Function breakView(ByVal axis As Integer, ByVal len As Single, ByVal a As Shape) As View
				Throw New Exception("Can't break root view")
			End Function

			''' <summary>
			''' Determines the resizability of the view along the
			''' given axis.  A value of 0 or less is not resizable.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the weight </returns>
			Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
				If view IsNot Nothing Then Return view.getResizeWeight(axis)
				Return 0
			End Function

			''' <summary>
			''' Sets the view size.
			''' </summary>
			''' <param name="width"> the width </param>
			''' <param name="height"> the height </param>
			Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
				If view IsNot Nothing Then view.sizeize(width, height)
			End Sub

			''' <summary>
			''' Fetches the container hosting the view.  This is useful for
			''' things like scheduling a repaint, finding out the host
			''' components font, etc.  The default implementation
			''' of this is to forward the query to the parent view.
			''' </summary>
			''' <returns> the container </returns>
			Public Property Overrides container As Container
				Get
					Return outerInstance.editor
				End Get
			End Property

			''' <summary>
			''' Fetches the factory to be used for building the
			''' various view fragments that make up the view that
			''' represents the model.  This is what determines
			''' how the model will be represented.  This is implemented
			''' to fetch the factory provided by the associated
			''' EditorKit unless that is null, in which case this
			''' simply returns the BasicTextUI itself which allows
			''' subclasses to implement a simple factory directly without
			''' creating extra objects.
			''' </summary>
			''' <returns> the factory </returns>
			Public Property Overrides viewFactory As ViewFactory
				Get
					Dim kit As EditorKit = outerInstance.getEditorKit(outerInstance.editor)
					Dim f As ViewFactory = kit.viewFactory
					If f IsNot Nothing Then Return f
					Return BasicTextUI.this
				End Get
			End Property

			Private view As View

		End Class

		''' <summary>
		''' Handles updates from various places.  If the model is changed,
		''' this class unregisters as a listener to the old model and
		''' registers with the new model.  If the document model changes,
		''' the change is forwarded to the root view.  If the focus
		''' accelerator changes, a new keystroke is registered to request
		''' focus.
		''' </summary>
		Friend Class UpdateHandler
			Implements PropertyChangeListener, DocumentListener, LayoutManager2, javax.swing.plaf.UIResource

			Private ReadOnly outerInstance As BasicTextUI

			Public Sub New(ByVal outerInstance As BasicTextUI)
				Me.outerInstance = outerInstance
			End Sub


			' --- PropertyChangeListener methods -----------------------

			''' <summary>
			''' This method gets called when a bound property is changed.
			''' We are looking for document changes on the editor.
			''' </summary>
			Public Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim oldValue As Object = evt.oldValue
				Dim newValue As Object = evt.newValue
				Dim propertyName As String = evt.propertyName
				If (TypeOf oldValue Is Document) OrElse (TypeOf newValue Is Document) Then
					If oldValue IsNot Nothing Then
						CType(oldValue, Document).removeDocumentListener(Me)
						i18nView = False
					End If
					If newValue IsNot Nothing Then
						CType(newValue, Document).addDocumentListener(Me)
						If "document" = propertyName Then
							outerInstance.view = Nothing
							outerInstance.propertyChange(evt)
							outerInstance.modelChanged()
							Return
						End If
					End If
					outerInstance.modelChanged()
				End If
				If "focusAccelerator" = propertyName Then
					outerInstance.updateFocusAcceleratorBinding(True)
				ElseIf "componentOrientation" = propertyName Then
					' Changes in ComponentOrientation require the views to be
					' rebuilt.
					outerInstance.modelChanged()
				ElseIf "font" = propertyName Then
					outerInstance.modelChanged()
				ElseIf "dropLocation" = propertyName Then
					dropIndexChanged()
				ElseIf "editable" = propertyName Then
					outerInstance.updateCursor()
					outerInstance.modelChanged()
				End If
				outerInstance.propertyChange(evt)
			End Sub

			Private Sub dropIndexChanged()
				If outerInstance.editor.dropMode = DropMode.USE_SELECTION Then Return

				Dim dropLocation As JTextComponent.DropLocation = outerInstance.editor.dropLocation

				If dropLocation Is Nothing Then
					If outerInstance.dropCaret IsNot Nothing Then
						outerInstance.dropCaret.deinstall(outerInstance.editor)
						outerInstance.editor.repaint(outerInstance.dropCaret)
						outerInstance.dropCaret = Nothing
					End If
				Else
					If outerInstance.dropCaret Is Nothing Then
						outerInstance.dropCaret = New BasicCaret
						outerInstance.dropCaret.install(outerInstance.editor)
						outerInstance.dropCaret.visible = True
					End If

					outerInstance.dropCaret.dotDot(dropLocation.index, dropLocation.bias)
				End If
			End Sub

			' --- DocumentListener methods -----------------------

			''' <summary>
			''' The insert notification.  Gets sent to the root of the view structure
			''' that represents the portion of the model being represented by the
			''' editor.  The factory is added as an argument to the update so that
			''' the views can update themselves in a dynamic (not hardcoded) way.
			''' </summary>
			''' <param name="e">  The change notification from the currently associated
			'''  document. </param>
			''' <seealso cref= DocumentListener#insertUpdate </seealso>
			Public Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
				Dim doc As Document = e.document
				Dim o As Object = doc.getProperty("i18n")
				If TypeOf o Is Boolean? Then
					Dim i18nFlag As Boolean? = CBool(o)
					If i18nFlag <> i18nView Then
						' i18n flag changed, rebuild the view
						i18nView = i18nFlag
						outerInstance.modelChanged()
						Return
					End If
				End If

				' normal insert update
				Dim alloc As Rectangle = If(outerInstance.painted, outerInstance.visibleEditorRect, Nothing)
				outerInstance.rootView.insertUpdate(e, alloc, outerInstance.rootView.viewFactory)
			End Sub

			''' <summary>
			''' The remove notification.  Gets sent to the root of the view structure
			''' that represents the portion of the model being represented by the
			''' editor.  The factory is added as an argument to the update so that
			''' the views can update themselves in a dynamic (not hardcoded) way.
			''' </summary>
			''' <param name="e">  The change notification from the currently associated
			'''  document. </param>
			''' <seealso cref= DocumentListener#removeUpdate </seealso>
			Public Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
				Dim alloc As Rectangle = If(outerInstance.painted, outerInstance.visibleEditorRect, Nothing)
				outerInstance.rootView.removeUpdate(e, alloc, outerInstance.rootView.viewFactory)
			End Sub

			''' <summary>
			''' The change notification.  Gets sent to the root of the view structure
			''' that represents the portion of the model being represented by the
			''' editor.  The factory is added as an argument to the update so that
			''' the views can update themselves in a dynamic (not hardcoded) way.
			''' </summary>
			''' <param name="e">  The change notification from the currently associated
			'''  document. </param>
			''' <seealso cref= DocumentListener#changedUpdate(DocumentEvent) </seealso>
			Public Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
				Dim alloc As Rectangle = If(outerInstance.painted, outerInstance.visibleEditorRect, Nothing)
				outerInstance.rootView.changedUpdate(e, alloc, outerInstance.rootView.viewFactory)
			End Sub

			' --- LayoutManager2 methods --------------------------------

			''' <summary>
			''' Adds the specified component with the specified name to
			''' the layout. </summary>
			''' <param name="name"> the component name </param>
			''' <param name="comp"> the component to be added </param>
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
				' not supported
			End Sub

			''' <summary>
			''' Removes the specified component from the layout. </summary>
			''' <param name="comp"> the component to be removed </param>
			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
				If constraints IsNot Nothing Then constraints.Remove(comp)
			End Sub

			''' <summary>
			''' Calculates the preferred size dimensions for the specified
			''' panel given the components in the specified parent container. </summary>
			''' <param name="parent"> the component to be laid out
			''' </param>
			''' <seealso cref= #minimumLayoutSize </seealso>
			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				' should not be called (JComponent uses UI instead)
				Return Nothing
			End Function

			''' <summary>
			''' Calculates the minimum size dimensions for the specified
			''' panel given the components in the specified parent container. </summary>
			''' <param name="parent"> the component to be laid out </param>
			''' <seealso cref= #preferredLayoutSize </seealso>
			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				' should not be called (JComponent uses UI instead)
				Return Nothing
			End Function

			''' <summary>
			''' Lays out the container in the specified panel.  This is
			''' implemented to position all components that were added
			''' with a View object as a constraint.  The current allocation
			''' of the associated View is used as the location of the
			''' component.
			''' <p>
			''' A read-lock is acquired on the document to prevent the
			''' view tree from being modified while the layout process
			''' is active.
			''' </summary>
			''' <param name="parent"> the component which needs to be laid out </param>
			Public Overridable Sub layoutContainer(ByVal parent As Container)
				If (constraints IsNot Nothing) AndAlso ( constraints.Count > 0) Then
					Dim alloc As Rectangle = outerInstance.visibleEditorRect
					If alloc IsNot Nothing Then
						Dim doc As Document = outerInstance.editor.document
						If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
						Try
							outerInstance.rootView.sizeize(alloc.width, alloc.height)
							Dim components As System.Collections.IEnumerator(Of Component) = constraints.Keys.GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While components.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								Dim comp As Component = components.nextElement()
								Dim v As View = CType(constraints(comp), View)
								Dim ca As Shape = calculateViewPosition(alloc, v)
								If ca IsNot Nothing Then
									Dim compAlloc As Rectangle = If(TypeOf ca Is Rectangle, CType(ca, Rectangle), ca.bounds)
									comp.bounds = compAlloc
								End If
							Loop
						Finally
							If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
						End Try
					End If
				End If
			End Sub

			''' <summary>
			''' Find the Shape representing the given view.
			''' </summary>
			Friend Overridable Function calculateViewPosition(ByVal alloc As Shape, ByVal v As View) As Shape
				Dim pos As Integer = v.startOffset
				Dim child As View = Nothing
				Dim parent As View = outerInstance.rootView
				Do While (parent IsNot Nothing) AndAlso (parent IsNot v)
					Dim index As Integer = parent.getViewIndex(pos, Position.Bias.Forward)
					alloc = parent.getChildAllocation(index, alloc)
					child = parent.getView(index)
					parent = child
				Loop
				Return If(child IsNot Nothing, alloc, Nothing)
			End Function

			''' <summary>
			''' Adds the specified component to the layout, using the specified
			''' constraint object.  We only store those components that were added
			''' with a constraint that is of type View.
			''' </summary>
			''' <param name="comp"> the component to be added </param>
			''' <param name="constraint">  where/how the component is added to the layout. </param>
			Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraint As Object)
				If TypeOf constraint Is View Then
					If constraints Is Nothing Then constraints = New Dictionary(Of Component, Object)(7)
					constraints(comp) = constraint
				End If
			End Sub

			''' <summary>
			''' Returns the maximum size of this component. </summary>
			''' <seealso cref= java.awt.Component#getMinimumSize() </seealso>
			''' <seealso cref= java.awt.Component#getPreferredSize() </seealso>
			''' <seealso cref= LayoutManager </seealso>
			Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
				' should not be called (JComponent uses UI instead)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the alignment along the x axis.  This specifies how
			''' the component would like to be aligned relative to other
			''' components.  The value should be a number between 0 and 1
			''' where 0 represents alignment along the origin, 1 is aligned
			''' the furthest away from the origin, 0.5 is centered, etc.
			''' </summary>
			Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
				Return 0.5f
			End Function

			''' <summary>
			''' Returns the alignment along the y axis.  This specifies how
			''' the component would like to be aligned relative to other
			''' components.  The value should be a number between 0 and 1
			''' where 0 represents alignment along the origin, 1 is aligned
			''' the furthest away from the origin, 0.5 is centered, etc.
			''' </summary>
			Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
				Return 0.5f
			End Function

			''' <summary>
			''' Invalidates the layout, indicating that if the layout manager
			''' has cached information it should be discarded.
			''' </summary>
			Public Overridable Sub invalidateLayout(ByVal target As Container)
			End Sub

			''' <summary>
			''' The "layout constraints" for the LayoutManager2 implementation.
			''' These are View objects for those components that are represented
			''' by a View in the View tree.
			''' </summary>
			Private constraints As Dictionary(Of Component, Object)

			Private i18nView As Boolean = False
		End Class

		''' <summary>
		''' Wrapper for text actions to return isEnabled false in case editor is non editable
		''' </summary>
		Friend Class TextActionWrapper
			Inherits TextAction

			Private ReadOnly outerInstance As BasicTextUI

			Public Sub New(ByVal outerInstance As BasicTextUI, ByVal action As TextAction)
					Me.outerInstance = outerInstance
				MyBase.New(CStr(action.getValue(Action.NAME)))
				Me.action = action
			End Sub
			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				action.actionPerformed(e)
			End Sub
			Public Property Overrides enabled As Boolean
				Get
					Return If(outerInstance.editor Is Nothing OrElse outerInstance.editor.editable, action.enabled, False)
				End Get
			End Property
			Friend action As TextAction = Nothing
		End Class


		''' <summary>
		''' Registered in the ActionMap.
		''' </summary>
		Friend Class FocusAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicTextUI

			Public Sub New(ByVal outerInstance As BasicTextUI)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.editor.requestFocus()
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Return outerInstance.editor.editable
				End Get
			End Property
		End Class

		Private Property Shared dragListener As DragListener
			Get
				SyncLock GetType(DragListener)
					Dim listener As DragListener = CType(sun.awt.AppContext.appContext.get(GetType(DragListener)), DragListener)
    
					If listener Is Nothing Then
						listener = New DragListener
						sun.awt.AppContext.appContext.put(GetType(DragListener), listener)
					End If
    
					Return listener
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Listens for mouse events for the purposes of detecting drag gestures.
		''' BasicTextUI will maintain one of these per AppContext.
		''' </summary>
		Friend Class DragListener
			Inherits MouseInputAdapter
			Implements javax.swing.plaf.basic.DragRecognitionSupport.BeforeDrag

			Private dragStarted As Boolean

			Public Overridable Sub dragStarting(ByVal [me] As MouseEvent)
				dragStarted = True
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				Dim c As JTextComponent = CType(e.source, JTextComponent)
				If c.dragEnabled Then
					dragStarted = False
					If isDragPossible(e) AndAlso DragRecognitionSupport.mousePressed(e) Then e.consume()
				End If
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				Dim c As JTextComponent = CType(e.source, JTextComponent)
				If c.dragEnabled Then
					If dragStarted Then e.consume()

					DragRecognitionSupport.mouseReleased(e)
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				Dim c As JTextComponent = CType(e.source, JTextComponent)
				If c.dragEnabled Then
					If dragStarted OrElse DragRecognitionSupport.mouseDragged(e, Me) Then e.consume()
				End If
			End Sub

			''' <summary>
			''' Determines if the following are true:
			''' <ul>
			''' <li>the component is enabled
			''' <li>the press event is located over a selection
			''' </ul>
			''' </summary>
			Protected Friend Overridable Function isDragPossible(ByVal e As MouseEvent) As Boolean
				Dim c As JTextComponent = CType(e.source, JTextComponent)
				If c.enabled Then
					Dim caret As Caret = c.caret
					Dim dot As Integer = caret.dot
					Dim mark As Integer = caret.mark
					If dot <> mark Then
						Dim p As New Point(e.x, e.y)
						Dim pos As Integer = c.viewToModel(p)

						Dim p0 As Integer = Math.Min(dot, mark)
						Dim p1 As Integer = Math.Max(dot, mark)
						If (pos >= p0) AndAlso (pos < p1) Then Return True
					End If
				End If
				Return False
			End Function
		End Class

		Friend Class TextTransferHandler
			Inherits TransferHandler
			Implements javax.swing.plaf.UIResource

			Private exportComp As JTextComponent
			Private shouldRemove As Boolean
			Private p0 As Integer
			Private p1 As Integer

			''' <summary>
			''' Whether or not this is a drop using
			''' <code>DropMode.INSERT</code>.
			''' </summary>
			Private modeBetween As Boolean = False

			''' <summary>
			''' Whether or not this is a drop.
			''' </summary>
			Private isDrop As Boolean = False

			''' <summary>
			''' The drop action.
			''' </summary>
			Private dropAction As Integer = MOVE

			''' <summary>
			''' The drop bias.
			''' </summary>
			Private dropBias As Position.Bias

			''' <summary>
			''' Try to find a flavor that can be used to import a Transferable.
			''' The set of usable flavors are tried in the following order:
			''' <ol>
			'''     <li>First, an attempt is made to find a flavor matching the content type
			'''         of the EditorKit for the component.
			'''     <li>Second, an attempt to find a text/plain flavor is made.
			'''     <li>Third, an attempt to find a flavor representing a String reference
			'''         in the same VM is made.
			'''     <li>Lastly, DataFlavor.stringFlavor is searched for.
			''' </ol>
			''' </summary>
			Protected Friend Overridable Function getImportFlavor(ByVal flavors As DataFlavor(), ByVal c As JTextComponent) As DataFlavor
				Dim plainFlavor As DataFlavor = Nothing
				Dim refFlavor As DataFlavor = Nothing
				Dim stringFlavor As DataFlavor = Nothing

				If TypeOf c Is JEditorPane Then
					For i As Integer = 0 To flavors.Length - 1
						Dim mime As String = flavors(i).mimeType
						If mime.StartsWith(CType(c, JEditorPane).editorKit.contentType) Then
							Return flavors(i)
						ElseIf plainFlavor Is Nothing AndAlso mime.StartsWith("text/plain") Then
							plainFlavor = flavors(i)
						ElseIf refFlavor Is Nothing AndAlso mime.StartsWith("application/x-java-jvm-local-objectref") AndAlso flavors(i).representationClass = GetType(String) Then
							refFlavor = flavors(i)
						ElseIf stringFlavor Is Nothing AndAlso flavors(i).Equals(DataFlavor.stringFlavor) Then
							stringFlavor = flavors(i)
						End If
					Next i
					If plainFlavor IsNot Nothing Then
						Return plainFlavor
					ElseIf refFlavor IsNot Nothing Then
						Return refFlavor
					ElseIf stringFlavor IsNot Nothing Then
						Return stringFlavor
					End If
					Return Nothing
				End If


				For i As Integer = 0 To flavors.Length - 1
					Dim mime As String = flavors(i).mimeType
					If mime.StartsWith("text/plain") Then
						Return flavors(i)
					ElseIf refFlavor Is Nothing AndAlso mime.StartsWith("application/x-java-jvm-local-objectref") AndAlso flavors(i).representationClass = GetType(String) Then
						refFlavor = flavors(i)
					ElseIf stringFlavor Is Nothing AndAlso flavors(i).Equals(DataFlavor.stringFlavor) Then
						stringFlavor = flavors(i)
					End If
				Next i
				If refFlavor IsNot Nothing Then
					Return refFlavor
				ElseIf stringFlavor IsNot Nothing Then
					Return stringFlavor
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Import the given stream data into the text component.
			''' </summary>
			Protected Friend Overridable Sub handleReaderImport(ByVal [in] As Reader, ByVal c As JTextComponent, ByVal useRead As Boolean)
				If useRead Then
					Dim startPosition As Integer = c.selectionStart
					Dim endPosition As Integer = c.selectionEnd
					Dim length As Integer = endPosition - startPosition
					Dim kit As EditorKit = c.uI.getEditorKit(c)
					Dim doc As Document = c.document
					If length > 0 Then doc.remove(startPosition, length)
					kit.read([in], doc, startPosition)
				Else
					Dim buff As Char() = New Char(1023){}
					Dim nch As Integer
					Dim lastWasCR As Boolean = False
					Dim last As Integer
					Dim sbuff As StringBuilder = Nothing

					' Read in a block at a time, mapping \r\n to \n, as well as single
					' \r to \n.
					nch = [in].read(buff, 0, buff.Length)
					Do While nch <> -1
						If sbuff Is Nothing Then sbuff = New StringBuilder(nch)
						last = 0
						For counter As Integer = 0 To nch - 1
							Select Case buff(counter)
							Case ControlChars.Cr
								If lastWasCR Then
									If counter = 0 Then
										sbuff.Append(ControlChars.Lf)
									Else
										buff(counter - 1) = ControlChars.Lf
									End If
								Else
									lastWasCR = True
								End If
							Case ControlChars.Lf
								If lastWasCR Then
									If counter > (last + 1) Then sbuff.Append(buff, last, counter - last - 1)
									' else nothing to do, can skip \r, next write will
									' write \n
									lastWasCR = False
									last = counter
								End If
							Case Else
								If lastWasCR Then
									If counter = 0 Then
										sbuff.Append(ControlChars.Lf)
									Else
										buff(counter - 1) = ControlChars.Lf
									End If
									lastWasCR = False
								End If
							End Select
						Next counter
						If last < nch Then
							If lastWasCR Then
								If last < (nch - 1) Then sbuff.Append(buff, last, nch - last - 1)
							Else
								sbuff.Append(buff, last, nch - last)
							End If
						End If
						nch = [in].read(buff, 0, buff.Length)
					Loop
					If lastWasCR Then sbuff.Append(ControlChars.Lf)
					c.replaceSelection(If(sbuff IsNot Nothing, sbuff.ToString(), ""))
				End If
			End Sub

			' --- TransferHandler methods ------------------------------------

			''' <summary>
			''' This is the type of transfer actions supported by the source.  Some models are
			''' not mutable, so a transfer operation of COPY only should
			''' be advertised in that case.
			''' </summary>
			''' <param name="c">  The component holding the data to be transfered.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <returns>  This is implemented to return NONE if the component is a JPasswordField
			'''  since exporting data via user gestures is not allowed.  If the text component is
			'''  editable, COPY_OR_MOVE is returned, otherwise just COPY is allowed. </returns>
			Public Overrides Function getSourceActions(ByVal c As JComponent) As Integer
				If TypeOf c Is JPasswordField AndAlso c.getClientProperty("JPasswordField.cutCopyAllowed") IsNot Boolean.TRUE Then Return NONE

				Return If(CType(c, JTextComponent).editable, COPY_OR_MOVE, COPY)
			End Function

			''' <summary>
			''' Create a Transferable to use as the source for a data transfer.
			''' </summary>
			''' <param name="comp">  The component holding the data to be transfered.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <returns>  The representation of the data to be transfered.
			'''  </returns>
			Protected Friend Overrides Function createTransferable(ByVal comp As JComponent) As Transferable
				exportComp = CType(comp, JTextComponent)
				shouldRemove = True
				p0 = exportComp.selectionStart
				p1 = exportComp.selectionEnd
				Return If(p0 <> p1, (New TextTransferable(exportComp, p0, p1)), Nothing)
			End Function

			''' <summary>
			''' This method is called after data has been exported.  This method should remove
			''' the data that was transfered if the action was MOVE.
			''' </summary>
			''' <param name="source"> The component that was the source of the data. </param>
			''' <param name="data">   The data that was transferred or possibly null
			'''               if the action is <code>NONE</code>. </param>
			''' <param name="action"> The actual action that was performed. </param>
			Protected Friend Overrides Sub exportDone(ByVal source As JComponent, ByVal data As Transferable, ByVal action As Integer)
				' only remove the text if shouldRemove has not been set to
				' false by importData and only if the action is a move
				If shouldRemove AndAlso action = MOVE Then
					Dim t As TextTransferable = CType(data, TextTransferable)
					t.removeText()
				End If

				exportComp = Nothing
			End Sub

			Public Overrides Function importData(ByVal support As TransferSupport) As Boolean
				isDrop = support.drop

				If isDrop Then
					modeBetween = CType(support.component, JTextComponent).dropMode = DropMode.INSERT

					dropBias = CType(support.dropLocation, JTextComponent.DropLocation).bias

					dropAction = support.dropAction
				End If

				Try
					Return MyBase.importData(support)
				Finally
					isDrop = False
					modeBetween = False
					dropBias = Nothing
					dropAction = MOVE
				End Try
			End Function

			''' <summary>
			''' This method causes a transfer to a component from a clipboard or a
			''' DND drop operation.  The Transferable represents the data to be
			''' imported into the component.
			''' </summary>
			''' <param name="comp">  The component to receive the transfer.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <param name="t">     The data to import </param>
			''' <returns>  true if the data was inserted into the component, false otherwise. </returns>
			Public Overrides Function importData(ByVal comp As JComponent, ByVal t As Transferable) As Boolean
				Dim c As JTextComponent = CType(comp, JTextComponent)

				Dim pos As Integer = If(modeBetween, c.dropLocation.index, c.caretPosition)

				' if we are importing to the same component that we exported from
				' then don't actually do anything if the drop location is inside
				' the drag location and set shouldRemove to false so that exportDone
				' knows not to remove any data
				If dropAction = MOVE AndAlso c Is exportComp AndAlso pos >= p0 AndAlso pos <= p1 Then
					shouldRemove = False
					Return True
				End If

				Dim imported As Boolean = False
				Dim ___importFlavor As DataFlavor = getImportFlavor(t.transferDataFlavors, c)
				If ___importFlavor IsNot Nothing Then
					Try
						Dim useRead As Boolean = False
						If TypeOf comp Is JEditorPane Then
							Dim ep As JEditorPane = CType(comp, JEditorPane)
							If (Not ep.contentType.StartsWith("text/plain")) AndAlso ___importFlavor.mimeType.StartsWith(ep.contentType) Then useRead = True
						End If
						Dim ic As java.awt.im.InputContext = c.inputContext
						If ic IsNot Nothing Then ic.endComposition()
						Dim r As Reader = ___importFlavor.getReaderForText(t)

						If modeBetween Then
							Dim caret As Caret = c.caret
							If TypeOf caret Is DefaultCaret Then
								CType(caret, DefaultCaret).dotDot(pos, dropBias)
							Else
								c.caretPosition = pos
							End If
						End If

						handleReaderImport(r, c, useRead)

						If isDrop Then
							c.requestFocus()
							Dim caret As Caret = c.caret
							If TypeOf caret Is DefaultCaret Then
								Dim newPos As Integer = caret.dot
								Dim newBias As Position.Bias = CType(caret, DefaultCaret).dotBias

								CType(caret, DefaultCaret).dotDot(pos, dropBias)
								CType(caret, DefaultCaret).moveDot(newPos, newBias)
							Else
								c.select(pos, c.caretPosition)
							End If
						End If

						imported = True
					Catch ufe As UnsupportedFlavorException
					Catch ble As BadLocationException
					Catch ioe As IOException
					End Try
				End If
				Return imported
			End Function

			''' <summary>
			''' This method indicates if a component would accept an import of the given
			''' set of data flavors prior to actually attempting to import it.
			''' </summary>
			''' <param name="comp">  The component to receive the transfer.  This
			'''  argument is provided to enable sharing of TransferHandlers by
			'''  multiple components. </param>
			''' <param name="flavors">  The data formats available </param>
			''' <returns>  true if the data can be inserted into the component, false otherwise. </returns>
			Public Overrides Function canImport(ByVal comp As JComponent, ByVal flavors As DataFlavor()) As Boolean
				Dim c As JTextComponent = CType(comp, JTextComponent)
				If Not(c.editable AndAlso c.enabled) Then Return False
				Return (getImportFlavor(flavors, c) IsNot Nothing)
			End Function

			''' <summary>
			''' A possible implementation of the Transferable interface
			''' for text components.  For a JEditorPane with a rich set
			''' of EditorKit implementations, conversions could be made
			''' giving a wider set of formats.  This is implemented to
			''' offer up only the active content type and text/plain
			''' (if that is not the active format) since that can be
			''' extracted from other formats.
			''' </summary>
			Friend Class TextTransferable
				Inherits BasicTransferable

				Friend Sub New(ByVal c As JTextComponent, ByVal start As Integer, ByVal [end] As Integer)
					MyBase.New(Nothing, Nothing)

					Me.c = c

					Dim doc As Document = c.document

					Try
						p0 = doc.createPosition(start)
						p1 = doc.createPosition([end])

						plainData = c.selectedText

						If TypeOf c Is JEditorPane Then
							Dim ep As JEditorPane = CType(c, JEditorPane)

							mimeType = ep.contentType

							If mimeType.StartsWith("text/plain") Then Return

							Dim sw As New StringWriter(p1.offset - p0.offset)
							ep.editorKit.write(sw, doc, p0.offset, p1.offset - p0.offset)

							If mimeType.StartsWith("text/html") Then
								htmlData = sw.ToString()
							Else
								richText = sw.ToString()
							End If
						End If
					Catch ble As BadLocationException
					Catch ioe As IOException
					End Try
				End Sub

				Friend Overridable Sub removeText()
					If (p0 IsNot Nothing) AndAlso (p1 IsNot Nothing) AndAlso (p0.offset <> p1.offset) Then
						Try
							Dim doc As Document = c.document
							doc.remove(p0.offset, p1.offset - p0.offset)
						Catch e As BadLocationException
						End Try
					End If
				End Sub

				' ---- EditorKit other than plain or HTML text -----------------------

				''' <summary>
				''' If the EditorKit is not for text/plain or text/html, that format
				''' is supported through the "richer flavors" part of BasicTransferable.
				''' </summary>
				Protected Friend Property Overrides richerFlavors As DataFlavor()
					Get
						If richText Is Nothing Then Return Nothing
    
						Try
							Dim flavors As DataFlavor() = New DataFlavor(2){}
							flavors(0) = New DataFlavor(mimeType & ";class=java.lang.String")
							flavors(1) = New DataFlavor(mimeType & ";class=java.io.Reader")
							flavors(2) = New DataFlavor(mimeType & ";class=java.io.InputStream;charset=unicode")
							Return flavors
						Catch cle As ClassNotFoundException
							' fall through to unsupported (should not happen)
						End Try
    
						Return Nothing
					End Get
				End Property

				''' <summary>
				''' The only richer format supported is the file list flavor
				''' </summary>
				Protected Friend Overrides Function getRicherData(ByVal flavor As DataFlavor) As Object
					If richText Is Nothing Then Return Nothing

					If GetType(String).Equals(flavor.representationClass) Then
						Return richText
					ElseIf GetType(Reader).Equals(flavor.representationClass) Then
						Return New StringReader(richText)
					ElseIf GetType(InputStream).Equals(flavor.representationClass) Then
						Return New StringBufferInputStream(richText)
					End If
					Throw New UnsupportedFlavorException(flavor)
				End Function

				Friend p0 As Position
				Friend p1 As Position
				Friend mimeType As String
				Friend richText As String
				Friend c As JTextComponent
			End Class

		End Class

	End Class

End Namespace