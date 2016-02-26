Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.ClientPropertyKey
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
	''' The base class for all Swing components except top-level containers.
	''' To use a component that inherits from <code>JComponent</code>,
	''' you must place the component in a containment hierarchy
	''' whose root is a top-level Swing container.
	''' Top-level Swing containers --
	''' such as <code>JFrame</code>, <code>JDialog</code>,
	''' and <code>JApplet</code> --
	''' are specialized components
	''' that provide a place for other Swing components to paint themselves.
	''' For an explanation of containment hierarchies, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/toplevel.html">Swing Components and the Containment Hierarchy</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' The <code>JComponent</code> class provides:
	''' <ul>
	''' <li>The base class for both standard and custom components
	'''     that use the Swing architecture.
	''' <li>A "pluggable look and feel" (L&amp;F) that can be specified by the
	'''     programmer or (optionally) selected by the user at runtime.
	'''     The look and feel for each component is provided by a
	'''     <em>UI delegate</em> -- an object that descends from
	'''     <seealso cref="javax.swing.plaf.ComponentUI"/>.
	'''     See <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/lookandfeel/plaf.html">How
	'''     to Set the Look and Feel</a>
	'''     in <em>The Java Tutorial</em>
	'''     for more information.
	''' <li>Comprehensive keystroke handling.
	'''     See the document <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/misc/keybinding.html">How to Use Key Bindings</a>,
	'''     an article in <em>The Java Tutorial</em>,
	'''     for more information.
	''' <li>Support for tool tips --
	'''     short descriptions that pop up when the cursor lingers
	'''     over a component.
	'''     See <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/tooltip.html">How
	'''     to Use Tool Tips</a>
	'''     in <em>The Java Tutorial</em>
	'''     for more information.
	''' <li>Support for accessibility.
	'''     <code>JComponent</code> contains all of the methods in the
	'''     <code>Accessible</code> interface,
	'''     but it doesn't actually implement the interface.  That is the
	'''     responsibility of the individual classes
	'''     that extend <code>JComponent</code>.
	''' <li>Support for component-specific properties.
	'''     With the <seealso cref="#putClientProperty"/>
	'''     and <seealso cref="#getClientProperty"/> methods,
	'''     you can associate name-object pairs
	'''     with any object that descends from <code>JComponent</code>.
	''' <li>An infrastructure for painting
	'''     that includes double buffering and support for borders.
	'''     For more information see <a
	''' href="http://www.oracle.com/technetwork/java/painting-140037.html#swing">Painting</a> and
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/border.htmll">How
	'''     to Use Borders</a>,
	'''     both of which are sections in <em>The Java Tutorial</em>.
	''' </ul>
	''' For more information on these subjects, see the
	''' <a href="package-summary.html#package_description">Swing package description</a>
	''' and <em>The Java Tutorial</em> section
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/jcomponent.html">The JComponent Class</a>.
	''' <p>
	''' <code>JComponent</code> and its subclasses document default values
	''' for certain properties.  For example, <code>JTable</code> documents the
	''' default row height as 16.  Each <code>JComponent</code> subclass
	''' that has a <code>ComponentUI</code> will create the
	''' <code>ComponentUI</code> as part of its constructor.  In order
	''' to provide a particular look and feel each
	''' <code>ComponentUI</code> may set properties back on the
	''' <code>JComponent</code> that created it.  For example, a custom
	''' look and feel may require <code>JTable</code>s to have a row
	''' height of 24. The documented defaults are the value of a property
	''' BEFORE the <code>ComponentUI</code> has been installed.  If you
	''' need a specific value for a particular property you should
	''' explicitly set it.
	''' <p>
	''' In release 1.4, the focus subsystem was rearchitected.
	''' For more information, see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	''' How to Use the Focus Subsystem</a>,
	''' a section in <em>The Java Tutorial</em>.
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
	''' </summary>
	''' <seealso cref= KeyStroke </seealso>
	''' <seealso cref= Action </seealso>
	''' <seealso cref= #setBorder </seealso>
	''' <seealso cref= #registerKeyboardAction </seealso>
	''' <seealso cref= JOptionPane </seealso>
	''' <seealso cref= #setDebugGraphicsOptions </seealso>
	''' <seealso cref= #setToolTipText </seealso>
	''' <seealso cref= #setAutoscrolls
	''' 
	''' @author Hans Muller
	''' @author Arnaud Weber </seealso>
	<Serializable> _
	Public MustInherit Class JComponent
		Inherits Container
		Implements TransferHandler.HasGetTransferHandler

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #writeObject </seealso>
		Private Const uiClassID As String = "ComponentUI"

		''' <seealso cref= #readObject </seealso>
		Private Shared ReadOnly readObjectCallbacks As New Dictionary(Of java.io.ObjectInputStream, ReadObjectCallback)(1)

		''' <summary>
		''' Keys to use for forward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private Shared managingFocusForwardTraversalKeys As java.util.Set(Of KeyStroke)

		''' <summary>
		''' Keys to use for backward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private Shared managingFocusBackwardTraversalKeys As java.util.Set(Of KeyStroke)

		' Following are the possible return values from getObscuredState.
		Private Const NOT_OBSCURED As Integer = 0
		Private Const PARTIALLY_OBSCURED As Integer = 1
		Private Const COMPLETELY_OBSCURED As Integer = 2

		''' <summary>
		''' Set to true when DebugGraphics has been loaded.
		''' </summary>
		Friend Shared DEBUG_GRAPHICS_LOADED As Boolean

		''' <summary>
		''' Key used to look up a value from the AppContext to determine the
		''' JComponent the InputVerifier is running for. That is, if
		''' AppContext.get(INPUT_VERIFIER_SOURCE_KEY) returns non-null, it
		''' indicates the EDT is calling into the InputVerifier from the
		''' returned component.
		''' </summary>
		Private Shared ReadOnly INPUT_VERIFIER_SOURCE_KEY As Object = New StringBuilder("InputVerifierSourceKey")

	'     The following fields support set methods for the corresponding
	'     * java.awt.Component properties.
	'     
		Private isAlignmentXSet As Boolean
		Private alignmentX As Single
		Private isAlignmentYSet As Boolean
		Private alignmentY As Single

		''' <summary>
		''' Backing store for JComponent properties and listeners
		''' </summary>

		''' <summary>
		''' The look and feel delegate for this component. </summary>
		<NonSerialized> _
		Protected Friend ui As ComponentUI
		''' <summary>
		''' A list of event listeners for this component. </summary>
		Protected Friend listenerList As New EventListenerList

		<NonSerialized> _
		Private clientProperties As ArrayTable
		Private vetoableChangeSupport As java.beans.VetoableChangeSupport
		''' <summary>
		''' Whether or not autoscroll has been enabled.
		''' </summary>
		Private autoscrolls As Boolean
		Private border As Border
		Private flags As Integer

		' Input verifier for this component 
		Private inputVerifier As InputVerifier = Nothing

		Private verifyInputWhenFocusTarget As Boolean = True

		''' <summary>
		''' Set in <code>_paintImmediately</code>.
		''' Will indicate the child that initiated the painting operation.
		''' If <code>paintingChild</code> is opaque, no need to paint
		''' any child components after <code>paintingChild</code>.
		''' Test used in <code>paintChildren</code>.
		''' </summary>
		<NonSerialized> _
		Friend paintingChild As Component

		''' <summary>
		''' Constant used for <code>registerKeyboardAction</code> that
		''' means that the command should be invoked when
		''' the component has the focus.
		''' </summary>
		Public Const WHEN_FOCUSED As Integer = 0

		''' <summary>
		''' Constant used for <code>registerKeyboardAction</code> that
		''' means that the command should be invoked when the receiving
		''' component is an ancestor of the focused component or is
		''' itself the focused component.
		''' </summary>
		Public Const WHEN_ANCESTOR_OF_FOCUSED_COMPONENT As Integer = 1

		''' <summary>
		''' Constant used for <code>registerKeyboardAction</code> that
		''' means that the command should be invoked when
		''' the receiving component is in the window that has the focus
		''' or is itself the focused component.
		''' </summary>
		Public Const WHEN_IN_FOCUSED_WINDOW As Integer = 2

		''' <summary>
		''' Constant used by some of the APIs to mean that no condition is defined.
		''' </summary>
		Public Const UNDEFINED_CONDITION As Integer = -1

		''' <summary>
		''' The key used by <code>JComponent</code> to access keyboard bindings.
		''' </summary>
		Private Const KEYBOARD_BINDINGS_KEY As String = "_KeyboardBindings"

		''' <summary>
		''' An array of <code>KeyStroke</code>s used for
		''' <code>WHEN_IN_FOCUSED_WINDOW</code> are stashed
		''' in the client properties under this string.
		''' </summary>
		Private Const WHEN_IN_FOCUSED_WINDOW_BINDINGS As String = "_WhenInFocusedWindow"

		''' <summary>
		''' The comment to display when the cursor is over the component,
		''' also known as a "value tip", "flyover help", or "flyover label".
		''' </summary>
		Public Const TOOL_TIP_TEXT_KEY As String = "ToolTipText"

		Private Const NEXT_FOCUS As String = "nextFocus"

		''' <summary>
		''' <code>JPopupMenu</code> assigned to this component
		''' and all of its children
		''' </summary>
		Private popupMenu As JPopupMenu

		''' <summary>
		''' Private flags * </summary>
		Private Const IS_DOUBLE_BUFFERED As Integer = 0
		Private Const ANCESTOR_USING_BUFFER As Integer = 1
		Private Const IS_PAINTING_TILE As Integer = 2
		Private Const IS_OPAQUE As Integer = 3
		Private Const KEY_EVENTS_ENABLED As Integer = 4
		Private Const FOCUS_INPUTMAP_CREATED As Integer = 5
		Private Const ANCESTOR_INPUTMAP_CREATED As Integer = 6
		Private Const WIF_INPUTMAP_CREATED As Integer = 7
		Private Const ACTIONMAP_CREATED As Integer = 8
		Private Const CREATED_DOUBLE_BUFFER As Integer = 9
		' bit 10 is free
		Private Const IS_PRINTING As Integer = 11
		Private Const IS_PRINTING_ALL As Integer = 12
		Private Const IS_REPAINTING As Integer = 13
		''' <summary>
		''' Bits 14-21 are used to handle nested writeObject calls. * </summary>
		Private Const WRITE_OBJ_COUNTER_FIRST As Integer = 14
		Private Const RESERVED_1 As Integer = 15
		Private Const RESERVED_2 As Integer = 16
		Private Const RESERVED_3 As Integer = 17
		Private Const RESERVED_4 As Integer = 18
		Private Const RESERVED_5 As Integer = 19
		Private Const RESERVED_6 As Integer = 20
		Private Const WRITE_OBJ_COUNTER_LAST As Integer = 21

		Private Const REQUEST_FOCUS_DISABLED As Integer = 22
		Private Const INHERITS_POPUP_MENU As Integer = 23
		Private Const OPAQUE_SET As Integer = 24
		Private Const AUTOSCROLLS_SET As Integer = 25
		Private Const FOCUS_TRAVERSAL_KEYS_FORWARD_SET As Integer = 26
		Private Const FOCUS_TRAVERSAL_KEYS_BACKWARD_SET As Integer = 27

		<NonSerialized> _
		Private revalidateRunnableScheduled As New java.util.concurrent.atomic.AtomicBoolean(False)

		''' <summary>
		''' Temporary rectangles.
		''' </summary>
		Private Shared tempRectangles As IList(Of Rectangle) = New List(Of Rectangle)(11)

		''' <summary>
		''' Used for <code>WHEN_FOCUSED</code> bindings. </summary>
		Private focusInputMap As InputMap
		''' <summary>
		''' Used for <code>WHEN_ANCESTOR_OF_FOCUSED_COMPONENT</code> bindings. </summary>
		Private ancestorInputMap As InputMap
		''' <summary>
		''' Used for <code>WHEN_IN_FOCUSED_KEY</code> bindings. </summary>
		Private windowInputMap As ComponentInputMap

		''' <summary>
		''' ActionMap. </summary>
		Private actionMap As ActionMap

		''' <summary>
		''' Key used to store the default locale in an AppContext * </summary>
		Private Const defaultLocale As String = "JComponent.defaultLocale"

		Private Shared componentObtainingGraphicsFrom As Component
		Private Shared componentObtainingGraphicsFromLock As Object = New StringBuilder("componentObtainingGraphicsFrom")

		''' <summary>
		''' AA text hints.
		''' </summary>
		<NonSerialized> _
		Private aaTextInfo As Object

		Friend Shared Function safelyGetGraphics(ByVal c As Component) As Graphics
			Return safelyGetGraphics(c, SwingUtilities.getRoot(c))
		End Function

		Friend Shared Function safelyGetGraphics(ByVal c As Component, ByVal root As Component) As Graphics
			SyncLock componentObtainingGraphicsFromLock
				componentObtainingGraphicsFrom = root
				Dim g As Graphics = c.graphics
				componentObtainingGraphicsFrom = Nothing
				Return g
			End SyncLock
		End Function

		Friend Shared Sub getGraphicsInvoked(ByVal root As Component)
			If Not JComponent.isComponentObtainingGraphicsFrom(root) Then
				Dim ___rootPane As JRootPane = CType(root, RootPaneContainer).rootPane
				If ___rootPane IsNot Nothing Then ___rootPane.disableTrueDoubleBuffering()
			End If
		End Sub


		''' <summary>
		''' Returns true if {@code c} is the component the graphics is being
		''' requested of. This is intended for use when getGraphics is invoked.
		''' </summary>
		Private Shared Function isComponentObtainingGraphicsFrom(ByVal c As Component) As Boolean
			SyncLock componentObtainingGraphicsFromLock
				Return (componentObtainingGraphicsFrom Is c)
			End SyncLock
		End Function

		''' <summary>
		''' Returns the Set of <code>KeyStroke</code>s to use if the component
		''' is managing focus for forward focus traversal.
		''' </summary>
		Friend Property Shared managingFocusForwardTraversalKeys As java.util.Set(Of KeyStroke)
			Get
				SyncLock GetType(JComponent)
					If managingFocusForwardTraversalKeys Is Nothing Then
						managingFocusForwardTraversalKeys = New HashSet(Of KeyStroke)(1)
						managingFocusForwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.CTRL_MASK))
					End If
				End SyncLock
				Return managingFocusForwardTraversalKeys
			End Get
		End Property

		''' <summary>
		''' Returns the Set of <code>KeyStroke</code>s to use if the component
		''' is managing focus for backward focus traversal.
		''' </summary>
		Friend Property Shared managingFocusBackwardTraversalKeys As java.util.Set(Of KeyStroke)
			Get
				SyncLock GetType(JComponent)
					If managingFocusBackwardTraversalKeys Is Nothing Then
						managingFocusBackwardTraversalKeys = New HashSet(Of KeyStroke)(1)
						managingFocusBackwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.SHIFT_MASK Or InputEvent.CTRL_MASK))
					End If
				End SyncLock
				Return managingFocusBackwardTraversalKeys
			End Get
		End Property

		Private Shared Function fetchRectangle() As Rectangle
			SyncLock tempRectangles
				Dim rect As Rectangle
				Dim ___size As Integer = tempRectangles.Count
				If ___size > 0 Then
					rect = tempRectangles.Remove(___size - 1)
				Else
					rect = New Rectangle(0, 0, 0, 0)
				End If
				Return rect
			End SyncLock
		End Function

		Private Shared Sub recycleRectangle(ByVal rect As Rectangle)
			SyncLock tempRectangles
				tempRectangles.Add(rect)
			End SyncLock
		End Sub

		''' <summary>
		''' Sets whether or not <code>getComponentPopupMenu</code> should delegate
		''' to the parent if this component does not have a <code>JPopupMenu</code>
		''' assigned to it.
		''' <p>
		''' The default value for this is false, but some <code>JComponent</code>
		''' subclasses that are implemented as a number of <code>JComponent</code>s
		''' may set this to true.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="value"> whether or not the JPopupMenu is inherited </param>
		''' <seealso cref= #setComponentPopupMenu
		''' @beaninfo
		'''        bound: true
		'''  description: Whether or not the JPopupMenu is inherited
		''' @since 1.5 </seealso>
		Public Overridable Property inheritsPopupMenu As Boolean
			Set(ByVal value As Boolean)
				Dim oldValue As Boolean = getFlag(INHERITS_POPUP_MENU)
				flaglag(INHERITS_POPUP_MENU, value)
				firePropertyChange("inheritsPopupMenu", oldValue, value)
			End Set
			Get
				Return getFlag(INHERITS_POPUP_MENU)
			End Get
		End Property


		''' <summary>
		''' Sets the <code>JPopupMenu</code> for this <code>JComponent</code>.
		''' The UI is responsible for registering bindings and adding the necessary
		''' listeners such that the <code>JPopupMenu</code> will be shown at
		''' the appropriate time. When the <code>JPopupMenu</code> is shown
		''' depends upon the look and feel: some may show it on a mouse event,
		''' some may enable a key binding.
		''' <p>
		''' If <code>popup</code> is null, and <code>getInheritsPopupMenu</code>
		''' returns true, then <code>getComponentPopupMenu</code> will be delegated
		''' to the parent. This provides for a way to make all child components
		''' inherit the popupmenu of the parent.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="popup"> - the popup that will be assigned to this component
		'''                may be null </param>
		''' <seealso cref= #getComponentPopupMenu
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''  description: Popup to show
		''' @since 1.5 </seealso>
		Public Overridable Property componentPopupMenu As JPopupMenu
			Set(ByVal popup As JPopupMenu)
				If popup IsNot Nothing Then enableEvents(AWTEvent.MOUSE_EVENT_MASK)
				Dim oldPopup As JPopupMenu = Me.popupMenu
				Me.popupMenu = popup
				firePropertyChange("componentPopupMenu", oldPopup, popup)
			End Set
			Get
    
				If Not inheritsPopupMenu Then Return popupMenu
    
				If popupMenu Is Nothing Then
					' Search parents for its popup
					Dim parent As Container = parent
					Do While parent IsNot Nothing
						If TypeOf parent Is JComponent Then Return CType(parent, JComponent).componentPopupMenu
						If TypeOf parent Is Window OrElse TypeOf parent Is java.applet.Applet Then Exit Do
						parent = parent.parent
					Loop
					Return Nothing
				End If
    
				Return popupMenu
			End Get
		End Property


		''' <summary>
		''' Default <code>JComponent</code> constructor.  This constructor does
		''' very little initialization beyond calling the <code>Container</code>
		''' constructor.  For example, the initial layout manager is
		''' <code>null</code>. It does, however, set the component's locale
		''' property to the value returned by
		''' <code>JComponent.getDefaultLocale</code>.
		''' </summary>
		''' <seealso cref= #getDefaultLocale </seealso>
		Public Sub New()
			MyBase.New()
			' We enable key events on all JComponents so that accessibility
			' bindings will work everywhere. This is a partial fix to BugID
			' 4282211.
			enableEvents(AWTEvent.KEY_EVENT_MASK)
			If managingFocus Then
				LookAndFeel.installProperty(Me, "focusTraversalKeysForward", managingFocusForwardTraversalKeys)
				LookAndFeel.installProperty(Me, "focusTraversalKeysBackward", managingFocusBackwardTraversalKeys)
			End If

			MyBase.locale = JComponent.defaultLocale
		End Sub


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' <code>JComponent</code> subclasses must override this method
		''' like this:
		''' <pre>
		'''   public void updateUI() {
		'''      setUI((SliderUI)UIManager.getUI(this);
		'''   }
		'''  </pre>
		''' </summary>
		''' <seealso cref= #setUI </seealso>
		''' <seealso cref= UIManager#getLookAndFeel </seealso>
		''' <seealso cref= UIManager#getUI </seealso>
		Public Overridable Sub updateUI()
		End Sub


		''' <summary>
		''' Sets the look and feel delegate for this component.
		''' <code>JComponent</code> subclasses generally override this method
		''' to narrow the argument type. For example, in <code>JSlider</code>:
		''' <pre>
		''' public void setUI(SliderUI newUI) {
		'''     super.setUI(newUI);
		''' }
		'''  </pre>
		''' <p>
		''' Additionally <code>JComponent</code> subclasses must provide a
		''' <code>getUI</code> method that returns the correct type.  For example:
		''' <pre>
		''' public SliderUI getUI() {
		'''     return (SliderUI)ui;
		''' }
		''' </pre>
		''' </summary>
		''' <param name="newUI"> the new UI delegate </param>
		''' <seealso cref= #updateUI </seealso>
		''' <seealso cref= UIManager#getLookAndFeel </seealso>
		''' <seealso cref= UIManager#getUI
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The component's look and feel delegate. </seealso>
		Protected Friend Overridable Property uI As ComponentUI
			Set(ByVal newUI As ComponentUI)
		'         We do not check that the UI instance is different
		'         * before allowing the switch in order to enable the
		'         * same UI instance *with different default settings*
		'         * to be installed.
		'         
    
				uninstallUIAndProperties()
    
				' aaText shouldn't persist between look and feels, reset it.
				aaTextInfo = UIManager.defaults(sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY)
				Dim oldUI As ComponentUI = ui
				ui = newUI
				If ui IsNot Nothing Then ui.installUI(Me)
    
				firePropertyChange("UI", oldUI, newUI)
				revalidate()
				repaint()
			End Set
		End Property

		''' <summary>
		''' Uninstalls the UI, if any, and any client properties designated
		''' as being specific to the installed UI - instances of
		''' {@code UIClientPropertyKey}.
		''' </summary>
		Private Sub uninstallUIAndProperties()
			If ui IsNot Nothing Then
				ui.uninstallUI(Me)
				'clean UIClientPropertyKeys from client properties
				If clientProperties IsNot Nothing Then
					SyncLock clientProperties
						Dim clientPropertyKeys As Object() = clientProperties.getKeys(Nothing)
						If clientPropertyKeys IsNot Nothing Then
							For Each key As Object In clientPropertyKeys
								If TypeOf key Is sun.swing.UIClientPropertyKey Then putClientProperty(key, Nothing)
							Next key
						End If
					End SyncLock
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the <code>UIDefaults</code> key used to
		''' look up the name of the <code>swing.plaf.ComponentUI</code>
		''' class that defines the look and feel
		''' for this component.  Most applications will never need to
		''' call this method.  Subclasses of <code>JComponent</code> that support
		''' pluggable look and feel should override this method to
		''' return a <code>UIDefaults</code> key that maps to the
		''' <code>ComponentUI</code> subclass that defines their look and feel.
		''' </summary>
		''' <returns> the <code>UIDefaults</code> key for a
		'''          <code>ComponentUI</code> subclass </returns>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''      expert: true
		''' description: UIClassID </seealso>
		Public Overridable Property uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Returns the graphics object used to paint this component.
		''' If <code>DebugGraphics</code> is turned on we create a new
		''' <code>DebugGraphics</code> object if necessary.
		''' Otherwise we just configure the
		''' specified graphics object's foreground and font.
		''' </summary>
		''' <param name="g"> the original <code>Graphics</code> object </param>
		''' <returns> a <code>Graphics</code> object configured for this component </returns>
		Protected Friend Overridable Function getComponentGraphics(ByVal g As Graphics) As Graphics
			Dim ___componentGraphics As Graphics = g
			If ui IsNot Nothing AndAlso DEBUG_GRAPHICS_LOADED Then
				If (DebugGraphics.debugComponentCount() <> 0) AndAlso (shouldDebugGraphics() <> 0) AndAlso Not(TypeOf g Is DebugGraphics) Then ___componentGraphics = New DebugGraphics(g,Me)
			End If
			___componentGraphics.color = foreground
			___componentGraphics.font = font

			Return ___componentGraphics
		End Function


		''' <summary>
		''' Calls the UI delegate's paint method, if the UI delegate
		''' is non-<code>null</code>.  We pass the delegate a copy of the
		''' <code>Graphics</code> object to protect the rest of the
		''' paint code from irrevocable changes
		''' (for example, <code>Graphics.translate</code>).
		''' <p>
		''' If you override this in a subclass you should not make permanent
		''' changes to the passed in <code>Graphics</code>. For example, you
		''' should not alter the clip <code>Rectangle</code> or modify the
		''' transform. If you need to do these operations you may find it
		''' easier to create a new <code>Graphics</code> from the passed in
		''' <code>Graphics</code> and manipulate it. Further, if you do not
		''' invoker super's implementation you must honor the opaque property,
		''' that is
		''' if this component is opaque, you must completely fill in the background
		''' in a non-opaque color. If you do not honor the opaque property you
		''' will likely see visual artifacts.
		''' <p>
		''' The passed in <code>Graphics</code> object might
		''' have a transform other than the identify transform
		''' installed on it.  In this case, you might get
		''' unexpected results if you cumulatively apply
		''' another transform.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> object to protect </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= ComponentUI </seealso>
		Protected Friend Overridable Sub paintComponent(ByVal g As Graphics)
			If ui IsNot Nothing Then
				Dim scratchGraphics As Graphics = If(g Is Nothing, Nothing, g.create())
				Try
					ui.update(scratchGraphics, Me)
				Finally
					scratchGraphics.Dispose()
				End Try
			End If
		End Sub

		''' <summary>
		''' Paints this component's children.
		''' If <code>shouldUseBuffer</code> is true,
		''' no component ancestor has a buffer and
		''' the component children can use a buffer if they have one.
		''' Otherwise, one ancestor has a buffer currently in use and children
		''' should not use a buffer to paint. </summary>
		''' <param name="g">  the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= java.awt.Container#paint </seealso>
		Protected Friend Overridable Sub paintChildren(ByVal g As Graphics)
			Dim sg As Graphics = g

			SyncLock treeLock
				Dim i As Integer = componentCount - 1
				If i < 0 Then Return
				' If we are only to paint to a specific child, determine
				' its index.
				If paintingChild IsNot Nothing AndAlso (TypeOf paintingChild Is JComponent) AndAlso paintingChild.opaque Then
					Do While i >= 0
						If getComponent(i) Is paintingChild Then Exit Do
						i -= 1
					Loop
				End If
				Dim tmpRect As Rectangle = fetchRectangle()
				Dim checkSiblings As Boolean = ((Not optimizedDrawingEnabled) AndAlso checkIfChildObscuredBySibling())
				Dim clipBounds As Rectangle = Nothing
				If checkSiblings Then
					clipBounds = sg.clipBounds
					If clipBounds Is Nothing Then clipBounds = New Rectangle(0, 0, width, height)
				End If
				Dim printing As Boolean = getFlag(IS_PRINTING)
				Dim window As Window = SwingUtilities.getWindowAncestor(Me)
				Dim isWindowOpaque As Boolean = window Is Nothing OrElse window.opaque
				Do While i >= 0
					Dim comp As Component = getComponent(i)
					If comp Is Nothing Then
						i -= 1
						Continue Do
					End If

					Dim isJComponent As Boolean = TypeOf comp Is JComponent

					' Enable painting of heavyweights in non-opaque windows.
					' See 6884960
					If ((Not isWindowOpaque) OrElse isJComponent OrElse isLightweightComponent(comp)) AndAlso comp.visible Then
						Dim cr As Rectangle

						cr = comp.getBounds(tmpRect)

						Dim hitClip As Boolean = g.hitClip(cr.x, cr.y, cr.width, cr.height)

						If hitClip Then
							If checkSiblings AndAlso i > 0 Then
								Dim ___x As Integer = cr.x
								Dim ___y As Integer = cr.y
								Dim ___width As Integer = cr.width
								Dim ___height As Integer = cr.height
								SwingUtilities.computeIntersection(clipBounds.x, clipBounds.y, clipBounds.width, clipBounds.height, cr)

								If getObscuredState(i, cr.x, cr.y, cr.width, cr.height) = COMPLETELY_OBSCURED Then
									i -= 1
									Continue Do
								End If
								cr.x = ___x
								cr.y = ___y
								cr.width = ___width
								cr.height = ___height
							End If
							Dim cg As Graphics = sg.create(cr.x, cr.y, cr.width, cr.height)
							cg.color = comp.foreground
							cg.font = comp.font
							Dim shouldSetFlagBack As Boolean = False
							Try
								If isJComponent Then
									If getFlag(ANCESTOR_USING_BUFFER) Then
										CType(comp, JComponent).flaglag(ANCESTOR_USING_BUFFER,True)
										shouldSetFlagBack = True
									End If
									If getFlag(IS_PAINTING_TILE) Then
										CType(comp, JComponent).flaglag(IS_PAINTING_TILE,True)
										shouldSetFlagBack = True
									End If
									If Not printing Then
										comp.paint(cg)
									Else
										If Not getFlag(IS_PRINTING_ALL) Then
											comp.print(cg)
										Else
											comp.printAll(cg)
										End If
									End If
								Else
									' The component is either lightweight, or
									' heavyweight in a non-opaque window
									If Not printing Then
										comp.paint(cg)
									Else
										If Not getFlag(IS_PRINTING_ALL) Then
											comp.print(cg)
										Else
											comp.printAll(cg)
										End If
									End If
								End If
							Finally
								cg.Dispose()
								If shouldSetFlagBack Then
									CType(comp, JComponent).flaglag(ANCESTOR_USING_BUFFER,False)
									CType(comp, JComponent).flaglag(IS_PAINTING_TILE,False)
								End If
							End Try
						End If
					End If

					i -= 1
				Loop
				recycleRectangle(tmpRect)
			End SyncLock
		End Sub

		''' <summary>
		''' Paints the component's border.
		''' <p>
		''' If you override this in a subclass you should not make permanent
		''' changes to the passed in <code>Graphics</code>. For example, you
		''' should not alter the clip <code>Rectangle</code> or modify the
		''' transform. If you need to do these operations you may find it
		''' easier to create a new <code>Graphics</code> from the passed in
		''' <code>Graphics</code> and manipulate it.
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> context in which to paint
		''' </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #setBorder </seealso>
		Protected Friend Overridable Sub paintBorder(ByVal g As Graphics)
			Dim ___border As Border = border
			If ___border IsNot Nothing Then ___border.paintBorder(Me, g, 0, 0, width, height)
		End Sub


		''' <summary>
		''' Calls <code>paint</code>.  Doesn't clear the background but see
		''' <code>ComponentUI.update</code>, which is called by
		''' <code>paintComponent</code>.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #paintComponent </seealso>
		''' <seealso cref= javax.swing.plaf.ComponentUI </seealso>
		Public Overridable Sub update(ByVal g As Graphics)
			paint(g)
		End Sub


		''' <summary>
		''' Invoked by Swing to draw components.
		''' Applications should not invoke <code>paint</code> directly,
		''' but should instead use the <code>repaint</code> method to
		''' schedule the component for redrawing.
		''' <p>
		''' This method actually delegates the work of painting to three
		''' protected methods: <code>paintComponent</code>,
		''' <code>paintBorder</code>,
		''' and <code>paintChildren</code>.  They're called in the order
		''' listed to ensure that children appear on top of component itself.
		''' Generally speaking, the component and its children should not
		''' paint in the insets area allocated to the border. Subclasses can
		''' just override this method, as always.  A subclass that just
		''' wants to specialize the UI (look and feel) delegate's
		''' <code>paint</code> method should just override
		''' <code>paintComponent</code>.
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #paintComponent </seealso>
		''' <seealso cref= #paintBorder </seealso>
		''' <seealso cref= #paintChildren </seealso>
		''' <seealso cref= #getComponentGraphics </seealso>
		''' <seealso cref= #repaint </seealso>
		Public Overridable Sub paint(ByVal g As Graphics)
			Dim shouldClearPaintFlags As Boolean = False

			If (width <= 0) OrElse (height <= 0) Then Return

			Dim ___componentGraphics As Graphics = getComponentGraphics(g)
			Dim co As Graphics = ___componentGraphics.create()
			Try
				Dim ___repaintManager As RepaintManager = RepaintManager.currentManager(Me)
				Dim clipRect As Rectangle = co.clipBounds
				Dim clipX As Integer
				Dim clipY As Integer
				Dim clipW As Integer
				Dim clipH As Integer
				If clipRect Is Nothing Then
						clipY = 0
						clipX = clipY
					clipW = width
					clipH = height
				Else
					clipX = clipRect.x
					clipY = clipRect.y
					clipW = clipRect.width
					clipH = clipRect.height
				End If

				If clipW > width Then clipW = width
				If clipH > height Then clipH = height

				If parent IsNot Nothing AndAlso Not(TypeOf parent Is JComponent) Then
					adjustPaintFlags()
					shouldClearPaintFlags = True
				End If

				Dim bw, bh As Integer
				Dim printing As Boolean = getFlag(IS_PRINTING)
				If (Not printing) AndAlso ___repaintManager.doubleBufferingEnabled AndAlso (Not getFlag(ANCESTOR_USING_BUFFER)) AndAlso doubleBuffered AndAlso (getFlag(IS_REPAINTING) OrElse ___repaintManager.painting) Then
					___repaintManager.beginPaint()
					Try
						___repaintManager.paint(Me, Me, co, clipX, clipY, clipW, clipH)
					Finally
						___repaintManager.endPaint()
					End Try
				Else
					' Will ocassionaly happen in 1.2, especially when printing.
					If clipRect Is Nothing Then co.cliplip(clipX, clipY, clipW, clipH)

					If Not rectangleIsObscured(clipX,clipY,clipW,clipH) Then
						If Not printing Then
							paintComponent(co)
							paintBorder(co)
						Else
							printComponent(co)
							printBorder(co)
						End If
					End If
					If Not printing Then
						paintChildren(co)
					Else
						printChildren(co)
					End If
				End If
			Finally
				co.Dispose()
				If shouldClearPaintFlags Then
					flaglag(ANCESTOR_USING_BUFFER,False)
					flaglag(IS_PAINTING_TILE,False)
					flaglag(IS_PRINTING,False)
					flaglag(IS_PRINTING_ALL,False)
				End If
			End Try
		End Sub

		' paint forcing use of the double buffer.  This is used for historical
		' reasons: JViewport, when scrolling, previously directly invoked paint
		' while turning off double buffering at the RepaintManager level, this
		' codes simulates that.
		Friend Overridable Sub paintForceDoubleBuffered(ByVal g As Graphics)
			Dim rm As RepaintManager = RepaintManager.currentManager(Me)
			Dim clip As Rectangle = g.clipBounds
			rm.beginPaint()
			flaglag(IS_REPAINTING, True)
			Try
				rm.paint(Me, Me, g, clip.x, clip.y, clip.width, clip.height)
			Finally
				rm.endPaint()
				flaglag(IS_REPAINTING, False)
			End Try
		End Sub

		''' <summary>
		''' Returns true if this component, or any of its ancestors, are in
		''' the processing of painting.
		''' </summary>
		Friend Overridable Property painting As Boolean
			Get
				Dim component As Container = Me
				Do While component IsNot Nothing
					If TypeOf component Is JComponent AndAlso CType(component, JComponent).getFlag(ANCESTOR_USING_BUFFER) Then Return True
					component = component.parent
				Loop
				Return False
			End Get
		End Property

		Private Sub adjustPaintFlags()
			Dim jparent As JComponent
			Dim parent As Container
			parent = parent
			Do While parent IsNot Nothing
				If TypeOf parent Is JComponent Then
					jparent = CType(parent, JComponent)
					If jparent.getFlag(ANCESTOR_USING_BUFFER) Then flaglag(ANCESTOR_USING_BUFFER, True)
					If jparent.getFlag(IS_PAINTING_TILE) Then flaglag(IS_PAINTING_TILE, True)
					If jparent.getFlag(IS_PRINTING) Then flaglag(IS_PRINTING, True)
					If jparent.getFlag(IS_PRINTING_ALL) Then flaglag(IS_PRINTING_ALL, True)
					Exit Do
				End If
				parent = parent.parent
			Loop
		End Sub

		''' <summary>
		''' Invoke this method to print the component. This method invokes
		''' <code>print</code> on the component.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #print </seealso>
		''' <seealso cref= #printComponent </seealso>
		''' <seealso cref= #printBorder </seealso>
		''' <seealso cref= #printChildren </seealso>
		Public Overridable Sub printAll(ByVal g As Graphics)
			flaglag(IS_PRINTING_ALL, True)
			Try
				print(g)
			Finally
				flaglag(IS_PRINTING_ALL, False)
			End Try
		End Sub

		''' <summary>
		''' Invoke this method to print the component to the specified
		''' <code>Graphics</code>. This method will result in invocations
		''' of <code>printComponent</code>, <code>printBorder</code> and
		''' <code>printChildren</code>. It is recommended that you override
		''' one of the previously mentioned methods rather than this one if
		''' your intention is to customize the way printing looks. However,
		''' it can be useful to override this method should you want to prepare
		''' state before invoking the superclass behavior. As an example,
		''' if you wanted to change the component's background color before
		''' printing, you could do the following:
		''' <pre>
		'''     public void print(Graphics g) {
		'''         Color orig = getBackground();
		'''         setBackground(Color.WHITE);
		''' 
		'''         // wrap in try/finally so that we always restore the state
		'''         try {
		'''             super.print(g);
		'''         } finally {
		'''             setBackground(orig);
		'''         }
		'''     }
		''' </pre>
		''' <p>
		''' Alternatively, or for components that delegate painting to other objects,
		''' you can query during painting whether or not the component is in the
		''' midst of a print operation. The <code>isPaintingForPrint</code> method provides
		''' this ability and its return value will be changed by this method: to
		''' <code>true</code> immediately before rendering and to <code>false</code>
		''' immediately after. With each change a property change event is fired on
		''' this component with the name <code>"paintingForPrint"</code>.
		''' <p>
		''' This method sets the component's state such that the double buffer
		''' will not be used: painting will be done directly on the passed in
		''' <code>Graphics</code>.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #printComponent </seealso>
		''' <seealso cref= #printBorder </seealso>
		''' <seealso cref= #printChildren </seealso>
		''' <seealso cref= #isPaintingForPrint </seealso>
		Public Overridable Sub print(ByVal g As Graphics)
			flaglag(IS_PRINTING, True)
			firePropertyChange("paintingForPrint", False, True)
			Try
				paint(g)
			Finally
				flaglag(IS_PRINTING, False)
				firePropertyChange("paintingForPrint", True, False)
			End Try
		End Sub

		''' <summary>
		''' This is invoked during a printing operation. This is implemented to
		''' invoke <code>paintComponent</code> on the component. Override this
		''' if you wish to add special painting behavior when printing.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #print
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub printComponent(ByVal g As Graphics)
			paintComponent(g)
		End Sub

		''' <summary>
		''' Prints this component's children. This is implemented to invoke
		''' <code>paintChildren</code> on the component. Override this if you
		''' wish to print the children differently than painting.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #print
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub printChildren(ByVal g As Graphics)
			paintChildren(g)
		End Sub

		''' <summary>
		''' Prints the component's border. This is implemented to invoke
		''' <code>paintBorder</code> on the component. Override this if you
		''' wish to print the border differently that it is painted.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint </param>
		''' <seealso cref= #print
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub printBorder(ByVal g As Graphics)
			paintBorder(g)
		End Sub

		''' <summary>
		'''  Returns true if the component is currently painting a tile.
		'''  If this method returns true, paint will be called again for another
		'''  tile. This method returns false if you are not painting a tile or
		'''  if the last tile is painted.
		'''  Use this method to keep some state you might need between tiles.
		''' </summary>
		'''  <returns>  true if the component is currently painting a tile,
		'''          false otherwise </returns>
		Public Overridable Property paintingTile As Boolean
			Get
				Return getFlag(IS_PAINTING_TILE)
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the current painting operation on this
		''' component is part of a <code>print</code> operation. This method is
		''' useful when you want to customize what you print versus what you show
		''' on the screen.
		''' <p>
		''' You can detect changes in the value of this property by listening for
		''' property change events on this component with name
		''' <code>"paintingForPrint"</code>.
		''' <p>
		''' Note: This method provides complimentary functionality to that provided
		''' by other high level Swing printing APIs. However, it deals strictly with
		''' painting and should not be confused as providing information on higher
		''' level print processes. For example, a <seealso cref="javax.swing.JTable#print()"/>
		''' operation doesn't necessarily result in a continuous rendering of the
		''' full component, and the return value of this method can change multiple
		''' times during that operation. It is even possible for the component to be
		''' painted to the screen while the printing process is ongoing. In such a
		''' case, the return value of this method is <code>true</code> when, and only
		''' when, the table is being painted as part of the printing process.
		''' </summary>
		''' <returns> true if the current painting operation on this component
		'''         is part of a print operation </returns>
		''' <seealso cref= #print
		''' @since 1.6 </seealso>
		Public Property paintingForPrint As Boolean
			Get
				Return getFlag(IS_PRINTING)
			End Get
		End Property

		''' <summary>
		''' In release 1.4, the focus subsystem was rearchitected.
		''' For more information, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' <p>
		''' Changes this <code>JComponent</code>'s focus traversal keys to
		''' CTRL+TAB and CTRL+SHIFT+TAB. Also prevents
		''' <code>SortingFocusTraversalPolicy</code> from considering descendants
		''' of this JComponent when computing a focus traversal cycle.
		''' </summary>
		''' <seealso cref= java.awt.Component#setFocusTraversalKeys </seealso>
		''' <seealso cref= SortingFocusTraversalPolicy </seealso>
		''' @deprecated As of 1.4, replaced by
		'''   <code>Component.setFocusTraversalKeys(int, Set)</code> and
		'''   <code>Container.setFocusCycleRoot(boolean)</code>. 
		<Obsolete("As of 1.4, replaced by")> _
		Public Overridable Property managingFocus As Boolean
			Get
				Return False
			End Get
		End Property

		Private Sub registerNextFocusableComponent()
			registerNextFocusableComponent(nextFocusableComponent)
		End Sub

		Private Sub registerNextFocusableComponent(ByVal nextFocusableComponent As Component)
			If nextFocusableComponent Is Nothing Then Return

			Dim nearestRoot As Container = If(focusCycleRoot, Me, focusCycleRootAncestor)
			Dim policy As FocusTraversalPolicy = nearestRoot.focusTraversalPolicy
			If Not(TypeOf policy Is LegacyGlueFocusTraversalPolicy) Then
				policy = New LegacyGlueFocusTraversalPolicy(policy)
				nearestRoot.focusTraversalPolicy = policy
			End If
			CType(policy, LegacyGlueFocusTraversalPolicy).nextFocusableComponentent(Me, nextFocusableComponent)
		End Sub

		Private Sub deregisterNextFocusableComponent()
			Dim ___nextFocusableComponent As Component = nextFocusableComponent
			If ___nextFocusableComponent Is Nothing Then Return

			Dim nearestRoot As Container = If(focusCycleRoot, Me, focusCycleRootAncestor)
			If nearestRoot Is Nothing Then Return
			Dim policy As FocusTraversalPolicy = nearestRoot.focusTraversalPolicy
			If TypeOf policy Is LegacyGlueFocusTraversalPolicy Then CType(policy, LegacyGlueFocusTraversalPolicy).unsetNextFocusableComponent(Me, ___nextFocusableComponent)
		End Sub

		''' <summary>
		''' In release 1.4, the focus subsystem was rearchitected.
		''' For more information, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' <p>
		''' Overrides the default <code>FocusTraversalPolicy</code> for this
		''' <code>JComponent</code>'s focus traversal cycle by unconditionally
		''' setting the specified <code>Component</code> as the next
		''' <code>Component</code> in the cycle, and this <code>JComponent</code>
		''' as the specified <code>Component</code>'s previous
		''' <code>Component</code> in the cycle.
		''' </summary>
		''' <param name="aComponent"> the <code>Component</code> that should follow this
		'''        <code>JComponent</code> in the focus traversal cycle
		''' </param>
		''' <seealso cref= #getNextFocusableComponent </seealso>
		''' <seealso cref= java.awt.FocusTraversalPolicy </seealso>
		''' @deprecated As of 1.4, replaced by <code>FocusTraversalPolicy</code> 
		<Obsolete("As of 1.4, replaced by <code>FocusTraversalPolicy</code>")> _
		Public Overridable Property nextFocusableComponent As Component
			Set(ByVal aComponent As Component)
				Dim displayable As Boolean = displayable
				If displayable Then deregisterNextFocusableComponent()
				putClientProperty(NEXT_FOCUS, aComponent)
				If displayable Then registerNextFocusableComponent(aComponent)
			End Set
			Get
				Return CType(getClientProperty(NEXT_FOCUS), Component)
			End Get
		End Property


		''' <summary>
		''' Provides a hint as to whether or not this <code>JComponent</code>
		''' should get focus. This is only a hint, and it is up to consumers that
		''' are requesting focus to honor this property. This is typically honored
		''' for mouse operations, but not keyboard operations. For example, look
		''' and feels could verify this property is true before requesting focus
		''' during a mouse operation. This would often times be used if you did
		''' not want a mouse press on a <code>JComponent</code> to steal focus,
		''' but did want the <code>JComponent</code> to be traversable via the
		''' keyboard. If you do not want this <code>JComponent</code> focusable at
		''' all, use the <code>setFocusable</code> method instead.
		''' <p>
		''' Please see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>,
		''' for more information.
		''' </summary>
		''' <param name="requestFocusEnabled"> indicates whether you want this
		'''        <code>JComponent</code> to be focusable or not </param>
		''' <seealso cref= <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a> </seealso>
		''' <seealso cref= java.awt.Component#setFocusable </seealso>
		Public Overridable Property requestFocusEnabled As Boolean
			Set(ByVal requestFocusEnabled As Boolean)
				flaglag(REQUEST_FOCUS_DISABLED, (Not requestFocusEnabled))
			End Set
			Get
				Return Not getFlag(REQUEST_FOCUS_DISABLED)
			End Get
		End Property


		''' <summary>
		''' Requests that this <code>Component</code> gets the input focus.
		''' Refer to {@link java.awt.Component#requestFocus()
		''' Component.requestFocus()} for a complete description of
		''' this method.
		''' <p>
		''' Note that the use of this method is discouraged because
		''' its behavior is platform dependent. Instead we recommend the
		''' use of <seealso cref="#requestFocusInWindow() requestFocusInWindow()"/>.
		''' If you would like more information on focus, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' </summary>
		''' <seealso cref= java.awt.Component#requestFocusInWindow() </seealso>
		''' <seealso cref= java.awt.Component#requestFocusInWindow(boolean)
		''' @since 1.4 </seealso>
		Public Overridable Sub requestFocus()
			MyBase.requestFocus()
		End Sub

		''' <summary>
		''' Requests that this <code>Component</code> gets the input focus.
		''' Refer to {@link java.awt.Component#requestFocus(boolean)
		''' Component.requestFocus(boolean)} for a complete description of
		''' this method.
		''' <p>
		''' Note that the use of this method is discouraged because
		''' its behavior is platform dependent. Instead we recommend the
		''' use of {@link #requestFocusInWindow(boolean)
		''' requestFocusInWindow(boolean)}.
		''' If you would like more information on focus, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' </summary>
		''' <param name="temporary"> boolean indicating if the focus change is temporary </param>
		''' <returns> <code>false</code> if the focus change request is guaranteed to
		'''         fail; <code>true</code> if it is likely to succeed </returns>
		''' <seealso cref= java.awt.Component#requestFocusInWindow() </seealso>
		''' <seealso cref= java.awt.Component#requestFocusInWindow(boolean)
		''' @since 1.4 </seealso>
		Public Overridable Function requestFocus(ByVal temporary As Boolean) As Boolean
			Return MyBase.requestFocus(temporary)
		End Function

		''' <summary>
		''' Requests that this <code>Component</code> gets the input focus.
		''' Refer to {@link java.awt.Component#requestFocusInWindow()
		''' Component.requestFocusInWindow()} for a complete description of
		''' this method.
		''' <p>
		''' If you would like more information on focus, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' </summary>
		''' <returns> <code>false</code> if the focus change request is guaranteed to
		'''         fail; <code>true</code> if it is likely to succeed </returns>
		''' <seealso cref= java.awt.Component#requestFocusInWindow() </seealso>
		''' <seealso cref= java.awt.Component#requestFocusInWindow(boolean)
		''' @since 1.4 </seealso>
		Public Overridable Function requestFocusInWindow() As Boolean
			Return MyBase.requestFocusInWindow()
		End Function

		''' <summary>
		''' Requests that this <code>Component</code> gets the input focus.
		''' Refer to {@link java.awt.Component#requestFocusInWindow(boolean)
		''' Component.requestFocusInWindow(boolean)} for a complete description of
		''' this method.
		''' <p>
		''' If you would like more information on focus, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' </summary>
		''' <param name="temporary"> boolean indicating if the focus change is temporary </param>
		''' <returns> <code>false</code> if the focus change request is guaranteed to
		'''         fail; <code>true</code> if it is likely to succeed </returns>
		''' <seealso cref= java.awt.Component#requestFocusInWindow() </seealso>
		''' <seealso cref= java.awt.Component#requestFocusInWindow(boolean)
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function requestFocusInWindow(ByVal temporary As Boolean) As Boolean
			Return MyBase.requestFocusInWindow(temporary)
		End Function

		''' <summary>
		''' Requests that this Component get the input focus, and that this
		''' Component's top-level ancestor become the focused Window. This component
		''' must be displayable, visible, and focusable for the request to be
		''' granted.
		''' <p>
		''' This method is intended for use by focus implementations. Client code
		''' should not use this method; instead, it should use
		''' <code>requestFocusInWindow()</code>.
		''' </summary>
		''' <seealso cref= #requestFocusInWindow() </seealso>
		Public Overridable Sub grabFocus()
			requestFocus()
		End Sub

		''' <summary>
		''' Sets the value to indicate whether input verifier for the
		''' current focus owner will be called before this component requests
		''' focus. The default is true. Set to false on components such as a
		''' Cancel button or a scrollbar, which should activate even if the
		''' input in the current focus owner is not "passed" by the input
		''' verifier for that component.
		''' </summary>
		''' <param name="verifyInputWhenFocusTarget"> value for the
		'''        <code>verifyInputWhenFocusTarget</code> property </param>
		''' <seealso cref= InputVerifier </seealso>
		''' <seealso cref= #setInputVerifier </seealso>
		''' <seealso cref= #getInputVerifier </seealso>
		''' <seealso cref= #getVerifyInputWhenFocusTarget
		''' 
		''' @since 1.3
		''' @beaninfo
		'''       bound: true
		''' description: Whether the Component verifies input before accepting
		'''              focus. </seealso>
		Public Overridable Property verifyInputWhenFocusTarget As Boolean
			Set(ByVal verifyInputWhenFocusTarget As Boolean)
				Dim oldVerifyInputWhenFocusTarget As Boolean = Me.verifyInputWhenFocusTarget
				Me.verifyInputWhenFocusTarget = verifyInputWhenFocusTarget
				firePropertyChange("verifyInputWhenFocusTarget", oldVerifyInputWhenFocusTarget, verifyInputWhenFocusTarget)
			End Set
			Get
				Return verifyInputWhenFocusTarget
			End Get
		End Property



		''' <summary>
		''' Gets the <code>FontMetrics</code> for the specified <code>Font</code>.
		''' </summary>
		''' <param name="font"> the font for which font metrics is to be
		'''          obtained </param>
		''' <returns> the font metrics for <code>font</code> </returns>
		''' <exception cref="NullPointerException"> if <code>font</code> is null
		''' @since 1.5 </exception>
		Public Overridable Function getFontMetrics(ByVal font As Font) As FontMetrics
			Return sun.swing.SwingUtilities2.getFontMetrics(Me, font)
		End Function


		''' <summary>
		''' Sets the preferred size of this component.
		''' If <code>preferredSize</code> is <code>null</code>, the UI will
		''' be asked for the preferred size.
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The preferred size of the component.
		''' </summary>
		Public Overridable Property preferredSize As Dimension
			Set(ByVal preferredSize As Dimension)
				MyBase.preferredSize = preferredSize
			End Set
			Get
				If preferredSizeSet Then Return MyBase.preferredSize
				Dim ___size As Dimension = Nothing
				If ui IsNot Nothing Then ___size = ui.getPreferredSize(Me)
				Return If(___size IsNot Nothing, ___size, MyBase.preferredSize)
			End Get
		End Property


		''' <summary>
		''' If the <code>preferredSize</code> has been set to a
		''' non-<code>null</code> value just returns it.
		''' If the UI delegate's <code>getPreferredSize</code>
		''' method returns a non <code>null</code> value then return that;
		''' otherwise defer to the component's layout manager.
		''' </summary>
		''' <returns> the value of the <code>preferredSize</code> property </returns>
		''' <seealso cref= #setPreferredSize </seealso>
		''' <seealso cref= ComponentUI </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:


		''' <summary>
		''' Sets the maximum size of this component to a constant
		''' value.  Subsequent calls to <code>getMaximumSize</code> will always
		''' return this value; the component's UI will not be asked
		''' to compute it.  Setting the maximum size to <code>null</code>
		''' restores the default behavior.
		''' </summary>
		''' <param name="maximumSize"> a <code>Dimension</code> containing the
		'''          desired maximum allowable size </param>
		''' <seealso cref= #getMaximumSize
		''' @beaninfo
		'''       bound: true
		''' description: The maximum size of the component. </seealso>
		Public Overridable Property maximumSize As Dimension
			Set(ByVal maximumSize As Dimension)
				MyBase.maximumSize = maximumSize
			End Set
			Get
				If maximumSizeSet Then Return MyBase.maximumSize
				Dim ___size As Dimension = Nothing
				If ui IsNot Nothing Then ___size = ui.getMaximumSize(Me)
				Return If(___size IsNot Nothing, ___size, MyBase.maximumSize)
			End Get
		End Property


		''' <summary>
		''' If the maximum size has been set to a non-<code>null</code> value
		''' just returns it.  If the UI delegate's <code>getMaximumSize</code>
		''' method returns a non-<code>null</code> value then return that;
		''' otherwise defer to the component's layout manager.
		''' </summary>
		''' <returns> the value of the <code>maximumSize</code> property </returns>
		''' <seealso cref= #setMaximumSize </seealso>
		''' <seealso cref= ComponentUI </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:


		''' <summary>
		''' Sets the minimum size of this component to a constant
		''' value.  Subsequent calls to <code>getMinimumSize</code> will always
		''' return this value; the component's UI will not be asked
		''' to compute it.  Setting the minimum size to <code>null</code>
		''' restores the default behavior.
		''' </summary>
		''' <param name="minimumSize"> the new minimum size of this component </param>
		''' <seealso cref= #getMinimumSize
		''' @beaninfo
		'''       bound: true
		''' description: The minimum size of the component. </seealso>
		Public Overridable Property minimumSize As Dimension
			Set(ByVal minimumSize As Dimension)
				MyBase.minimumSize = minimumSize
			End Set
			Get
				If minimumSizeSet Then Return MyBase.minimumSize
				Dim ___size As Dimension = Nothing
				If ui IsNot Nothing Then ___size = ui.getMinimumSize(Me)
				Return If(___size IsNot Nothing, ___size, MyBase.minimumSize)
			End Get
		End Property

		''' <summary>
		''' If the minimum size has been set to a non-<code>null</code> value
		''' just returns it.  If the UI delegate's <code>getMinimumSize</code>
		''' method returns a non-<code>null</code> value then return that; otherwise
		''' defer to the component's layout manager.
		''' </summary>
		''' <returns> the value of the <code>minimumSize</code> property </returns>
		''' <seealso cref= #setMinimumSize </seealso>
		''' <seealso cref= ComponentUI </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Gives the UI delegate an opportunity to define the precise
		''' shape of this component for the sake of mouse processing.
		''' </summary>
		''' <returns> true if this component logically contains x,y </returns>
		''' <seealso cref= java.awt.Component#contains(int, int) </seealso>
		''' <seealso cref= ComponentUI </seealso>
		Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
			Return If(ui IsNot Nothing, ui.contains(Me, x, y), MyBase.contains(x, y))
		End Function

		''' <summary>
		''' Sets the border of this component.  The <code>Border</code> object is
		''' responsible for defining the insets for the component
		''' (overriding any insets set directly on the component) and
		''' for optionally rendering any border decorations within the
		''' bounds of those insets.  Borders should be used (rather
		''' than insets) for creating both decorative and non-decorative
		''' (such as margins and padding) regions for a swing component.
		''' Compound borders can be used to nest multiple borders within a
		''' single component.
		''' <p>
		''' Although technically you can set the border on any object
		''' that inherits from <code>JComponent</code>, the look and
		''' feel implementation of many standard Swing components
		''' doesn't work well with user-set borders.  In general,
		''' when you want to set a border on a standard Swing
		''' component other than <code>JPanel</code> or <code>JLabel</code>,
		''' we recommend that you put the component in a <code>JPanel</code>
		''' and set the border on the <code>JPanel</code>.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="border"> the border to be rendered for this component </param>
		''' <seealso cref= Border </seealso>
		''' <seealso cref= CompoundBorder
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The component's border. </seealso>
		Public Overridable Property border As Border
			Set(ByVal border As Border)
				Dim oldBorder As Border = Me.border
    
				Me.border = border
				firePropertyChange("border", oldBorder, border)
				If border IsNot oldBorder Then
					If border Is Nothing OrElse oldBorder Is Nothing OrElse Not(border.getBorderInsets(Me).Equals(oldBorder.getBorderInsets(Me))) Then revalidate()
					repaint()
				End If
			End Set
			Get
				Return border
			End Get
		End Property


		''' <summary>
		''' If a border has been set on this component, returns the
		''' border's insets; otherwise calls <code>super.getInsets</code>.
		''' </summary>
		''' <returns> the value of the insets property </returns>
		''' <seealso cref= #setBorder </seealso>
		Public Overridable Property insets As Insets
			Get
				If border IsNot Nothing Then Return border.getBorderInsets(Me)
				Return MyBase.insets
			End Get
		End Property

		''' <summary>
		''' Returns an <code>Insets</code> object containing this component's inset
		''' values.  The passed-in <code>Insets</code> object will be reused
		''' if possible.
		''' Calling methods cannot assume that the same object will be returned,
		''' however.  All existing values within this object are overwritten.
		''' If <code>insets</code> is null, this will allocate a new one.
		''' </summary>
		''' <param name="insets"> the <code>Insets</code> object, which can be reused </param>
		''' <returns> the <code>Insets</code> object </returns>
		''' <seealso cref= #getInsets
		''' @beaninfo
		'''   expert: true </seealso>
		Public Overridable Function getInsets(ByVal insets As Insets) As Insets
			If insets Is Nothing Then insets = New Insets(0, 0, 0, 0)
			If border IsNot Nothing Then
				If TypeOf border Is AbstractBorder Then
					Return CType(border, AbstractBorder).getBorderInsets(Me, insets)
				Else
					' Can't reuse border insets because the Border interface
					' can't be enhanced.
					Return border.getBorderInsets(Me)
				End If
			Else
				' super.getInsets() always returns an Insets object with
				' all of its value zeroed.  No need for a new object here.
					insets.bottom = 0
						insets.right = insets.bottom
							insets.top = insets.right
							insets.left = insets.top
				Return insets
			End If
		End Function

		''' <summary>
		''' Overrides <code>Container.getAlignmentY</code> to return
		''' the horizontal alignment.
		''' </summary>
		''' <returns> the value of the <code>alignmentY</code> property </returns>
		''' <seealso cref= #setAlignmentY </seealso>
		''' <seealso cref= java.awt.Component#getAlignmentY </seealso>
		Public Overridable Property alignmentY As Single
			Get
				If isAlignmentYSet Then Return alignmentY
				Return MyBase.alignmentY
			End Get
			Set(ByVal alignmentY As Single)
				Me.alignmentY = If(alignmentY > 1.0f, 1.0f, If(alignmentY < 0.0f, 0.0f, alignmentY))
				isAlignmentYSet = True
			End Set
		End Property



		''' <summary>
		''' Overrides <code>Container.getAlignmentX</code> to return
		''' the vertical alignment.
		''' </summary>
		''' <returns> the value of the <code>alignmentX</code> property </returns>
		''' <seealso cref= #setAlignmentX </seealso>
		''' <seealso cref= java.awt.Component#getAlignmentX </seealso>
		Public Overridable Property alignmentX As Single
			Get
				If isAlignmentXSet Then Return alignmentX
				Return MyBase.alignmentX
			End Get
			Set(ByVal alignmentX As Single)
				Me.alignmentX = If(alignmentX > 1.0f, 1.0f, If(alignmentX < 0.0f, 0.0f, alignmentX))
				isAlignmentXSet = True
			End Set
		End Property


		''' <summary>
		''' Sets the input verifier for this component.
		''' </summary>
		''' <param name="inputVerifier"> the new input verifier
		''' @since 1.3 </param>
		''' <seealso cref= InputVerifier
		''' @beaninfo
		'''       bound: true
		''' description: The component's input verifier. </seealso>
		Public Overridable Property inputVerifier As InputVerifier
			Set(ByVal inputVerifier As InputVerifier)
				Dim oldInputVerifier As InputVerifier = CType(getClientProperty(JComponent_INPUT_VERIFIER), InputVerifier)
				putClientProperty(JComponent_INPUT_VERIFIER, inputVerifier)
				firePropertyChange("inputVerifier", oldInputVerifier, inputVerifier)
			End Set
			Get
				Return CType(getClientProperty(JComponent_INPUT_VERIFIER), InputVerifier)
			End Get
		End Property


		''' <summary>
		''' Returns this component's graphics context, which lets you draw
		''' on a component. Use this method to get a <code>Graphics</code> object and
		''' then invoke operations on that object to draw on the component. </summary>
		''' <returns> this components graphics context </returns>
		Public Overridable Property graphics As Graphics
			Get
				If DEBUG_GRAPHICS_LOADED AndAlso shouldDebugGraphics() <> 0 Then
					Dim ___graphics As New DebugGraphics(MyBase.graphics, Me)
					Return ___graphics
				End If
				Return MyBase.graphics
			End Get
		End Property


		''' <summary>
		''' Enables or disables diagnostic information about every graphics
		''' operation performed within the component or one of its children.
		''' </summary>
		''' <param name="debugOptions">  determines how the component should display
		'''         the information;  one of the following options:
		''' <ul>
		''' <li>DebugGraphics.LOG_OPTION - causes a text message to be printed.
		''' <li>DebugGraphics.FLASH_OPTION - causes the drawing to flash several
		''' times.
		''' <li>DebugGraphics.BUFFERED_OPTION - creates an
		'''         <code>ExternalWindow</code> that displays the operations
		'''         performed on the View's offscreen buffer.
		''' <li>DebugGraphics.NONE_OPTION disables debugging.
		''' <li>A value of 0 causes no changes to the debugging options.
		''' </ul>
		''' <code>debugOptions</code> is bitwise OR'd into the current value
		'''  
		''' @beaninfo
		'''   preferred: true
		'''        enum: NONE_OPTION DebugGraphics.NONE_OPTION
		'''              LOG_OPTION DebugGraphics.LOG_OPTION
		'''              FLASH_OPTION DebugGraphics.FLASH_OPTION
		'''              BUFFERED_OPTION DebugGraphics.BUFFERED_OPTION
		''' description: Diagnostic options for graphics operations. </param>
		Public Overridable Property debugGraphicsOptions As Integer
			Set(ByVal debugOptions As Integer)
				DebugGraphics.debugOptionsons(Me, debugOptions)
			End Set
			Get
				Return DebugGraphics.getDebugOptions(Me)
			End Get
		End Property



		''' <summary>
		''' Returns true if debug information is enabled for this
		''' <code>JComponent</code> or one of its parents.
		''' </summary>
		Friend Overridable Function shouldDebugGraphics() As Integer
			Return DebugGraphics.shouldComponentDebug(Me)
		End Function

		''' <summary>
		''' This method is now obsolete, please use a combination of
		''' <code>getActionMap()</code> and <code>getInputMap()</code> for
		''' similar behavior. For example, to bind the <code>KeyStroke</code>
		''' <code>aKeyStroke</code> to the <code>Action</code> <code>anAction</code>
		''' now use:
		''' <pre>
		'''   component.getInputMap().put(aKeyStroke, aCommand);
		'''   component.getActionMap().put(aCommmand, anAction);
		''' </pre>
		''' The above assumes you want the binding to be applicable for
		''' <code>WHEN_FOCUSED</code>. To register bindings for other focus
		''' states use the <code>getInputMap</code> method that takes an integer.
		''' <p>
		''' Register a new keyboard action.
		''' <code>anAction</code> will be invoked if a key event matching
		''' <code>aKeyStroke</code> occurs and <code>aCondition</code> is verified.
		''' The <code>KeyStroke</code> object defines a
		''' particular combination of a keyboard key and one or more modifiers
		''' (alt, shift, ctrl, meta).
		''' <p>
		''' The <code>aCommand</code> will be set in the delivered event if
		''' specified.
		''' <p>
		''' The <code>aCondition</code> can be one of:
		''' <blockquote>
		''' <DL>
		''' <DT>WHEN_FOCUSED
		''' <DD>The action will be invoked only when the keystroke occurs
		'''     while the component has the focus.
		''' <DT>WHEN_IN_FOCUSED_WINDOW
		''' <DD>The action will be invoked when the keystroke occurs while
		'''     the component has the focus or if the component is in the
		'''     window that has the focus. Note that the component need not
		'''     be an immediate descendent of the window -- it can be
		'''     anywhere in the window's containment hierarchy. In other
		'''     words, whenever <em>any</em> component in the window has the focus,
		'''     the action registered with this component is invoked.
		''' <DT>WHEN_ANCESTOR_OF_FOCUSED_COMPONENT
		''' <DD>The action will be invoked when the keystroke occurs while the
		'''     component has the focus or if the component is an ancestor of
		'''     the component that has the focus.
		''' </DL>
		''' </blockquote>
		''' <p>
		''' The combination of keystrokes and conditions lets you define high
		''' level (semantic) action events for a specified keystroke+modifier
		''' combination (using the KeyStroke class) and direct to a parent or
		''' child of a component that has the focus, or to the component itself.
		''' In other words, in any hierarchical structure of components, an
		''' arbitrary key-combination can be immediately directed to the
		''' appropriate component in the hierarchy, and cause a specific method
		''' to be invoked (usually by way of adapter objects).
		''' <p>
		''' If an action has already been registered for the receiving
		''' container, with the same charCode and the same modifiers,
		''' <code>anAction</code> will replace the action.
		''' </summary>
		''' <param name="anAction">  the <code>Action</code> to be registered </param>
		''' <param name="aCommand">  the command to be set in the delivered event </param>
		''' <param name="aKeyStroke"> the <code>KeyStroke</code> to bind to the action </param>
		''' <param name="aCondition"> the condition that needs to be met, see above </param>
		''' <seealso cref= KeyStroke </seealso>
		Public Overridable Sub registerKeyboardAction(ByVal anAction As ActionListener, ByVal aCommand As String, ByVal aKeyStroke As KeyStroke, ByVal aCondition As Integer)

			Dim ___inputMap As InputMap = getInputMap(aCondition, True)

			If ___inputMap IsNot Nothing Then
				Dim ___actionMap As ActionMap = getActionMap(True)
				Dim action As New ActionStandin(Me, anAction, aCommand)
				___inputMap.put(aKeyStroke, action)
				If ___actionMap IsNot Nothing Then ___actionMap.put(action, action)
			End If
		End Sub

		''' <summary>
		''' Registers any bound <code>WHEN_IN_FOCUSED_WINDOW</code> actions with
		''' the <code>KeyboardManager</code>. If <code>onlyIfNew</code>
		''' is true only actions that haven't been registered are pushed
		''' to the <code>KeyboardManager</code>;
		''' otherwise all actions are pushed to the <code>KeyboardManager</code>.
		''' </summary>
		''' <param name="onlyIfNew">  if true, only actions that haven't been registered
		'''          are pushed to the <code>KeyboardManager</code> </param>
		Private Sub registerWithKeyboardManager(ByVal onlyIfNew As Boolean)
			Dim ___inputMap As InputMap = getInputMap(WHEN_IN_FOCUSED_WINDOW, False)
			Dim strokes As KeyStroke()
			Dim registered As Dictionary(Of KeyStroke, KeyStroke) = CType(getClientProperty(WHEN_IN_FOCUSED_WINDOW_BINDINGS), Dictionary(Of KeyStroke, KeyStroke))

			If ___inputMap IsNot Nothing Then
				' Push any new KeyStrokes to the KeyboardManager.
				strokes = ___inputMap.allKeys()
				If strokes IsNot Nothing Then
					For counter As Integer = strokes.Length - 1 To 0 Step -1
						If (Not onlyIfNew) OrElse registered Is Nothing OrElse registered(strokes(counter)) Is Nothing Then registerWithKeyboardManager(strokes(counter))
						If registered IsNot Nothing Then registered.Remove(strokes(counter))
					Next counter
				End If
			Else
				strokes = Nothing
			End If
			' Remove any old ones.
			If registered IsNot Nothing AndAlso registered.Count > 0 Then
				Dim keys As System.Collections.IEnumerator(Of KeyStroke) = registered.Keys.GetEnumerator()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ks As KeyStroke = keys.nextElement()
					unregisterWithKeyboardManager(ks)
				Loop
				registered.Clear()
			End If
			' Updated the registered Hashtable.
			If strokes IsNot Nothing AndAlso strokes.Length > 0 Then
				If registered Is Nothing Then
					registered = New Dictionary(Of KeyStroke, KeyStroke)(strokes.Length)
					putClientProperty(WHEN_IN_FOCUSED_WINDOW_BINDINGS, registered)
				End If
				For counter As Integer = strokes.Length - 1 To 0 Step -1
					registered(strokes(counter)) = strokes(counter)
				Next counter
			Else
				putClientProperty(WHEN_IN_FOCUSED_WINDOW_BINDINGS, Nothing)
			End If
		End Sub

		''' <summary>
		''' Unregisters all the previously registered
		''' <code>WHEN_IN_FOCUSED_WINDOW</code> <code>KeyStroke</code> bindings.
		''' </summary>
		Private Sub unregisterWithKeyboardManager()
			Dim registered As Dictionary(Of KeyStroke, KeyStroke) = CType(getClientProperty(WHEN_IN_FOCUSED_WINDOW_BINDINGS), Dictionary(Of KeyStroke, KeyStroke))

			If registered IsNot Nothing AndAlso registered.Count > 0 Then
				Dim keys As System.Collections.IEnumerator(Of KeyStroke) = registered.Keys.GetEnumerator()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While keys.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ks As KeyStroke = keys.nextElement()
					unregisterWithKeyboardManager(ks)
				Loop
			End If
			putClientProperty(WHEN_IN_FOCUSED_WINDOW_BINDINGS, Nothing)
		End Sub

		''' <summary>
		''' Invoked from <code>ComponentInputMap</code> when its bindings change.
		''' If <code>inputMap</code> is the current <code>windowInputMap</code>
		''' (or a parent of the window <code>InputMap</code>)
		''' the <code>KeyboardManager</code> is notified of the new bindings.
		''' </summary>
		''' <param name="inputMap"> the map containing the new bindings </param>
		Friend Overridable Sub componentInputMapChanged(ByVal inputMap As ComponentInputMap)
			Dim km As InputMap = getInputMap(WHEN_IN_FOCUSED_WINDOW, False)

			Do While km IsNot inputMap AndAlso km IsNot Nothing
				km = km.parent
			Loop
			If km IsNot Nothing Then registerWithKeyboardManager(False)
		End Sub

		Private Sub registerWithKeyboardManager(ByVal aKeyStroke As KeyStroke)
			KeyboardManager.currentManager.registerKeyStroke(aKeyStroke,Me)
		End Sub

		Private Sub unregisterWithKeyboardManager(ByVal aKeyStroke As KeyStroke)
			KeyboardManager.currentManager.unregisterKeyStroke(aKeyStroke, Me)
		End Sub

		''' <summary>
		''' This method is now obsolete, please use a combination of
		''' <code>getActionMap()</code> and <code>getInputMap()</code> for
		''' similar behavior.
		''' </summary>
		Public Overridable Sub registerKeyboardAction(ByVal anAction As ActionListener, ByVal aKeyStroke As KeyStroke, ByVal aCondition As Integer)
			registerKeyboardAction(anAction,Nothing,aKeyStroke,aCondition)
		End Sub

		''' <summary>
		''' This method is now obsolete. To unregister an existing binding
		''' you can either remove the binding from the
		''' <code>ActionMap/InputMap</code>, or place a dummy binding the
		''' <code>InputMap</code>. Removing the binding from the
		''' <code>InputMap</code> allows bindings in parent <code>InputMap</code>s
		''' to be active, whereas putting a dummy binding in the
		''' <code>InputMap</code> effectively disables
		''' the binding from ever happening.
		''' <p>
		''' Unregisters a keyboard action.
		''' This will remove the binding from the <code>ActionMap</code>
		''' (if it exists) as well as the <code>InputMap</code>s.
		''' </summary>
		Public Overridable Sub unregisterKeyboardAction(ByVal aKeyStroke As KeyStroke)
			Dim am As ActionMap = getActionMap(False)
			For counter As Integer = 0 To 2
				Dim km As InputMap = getInputMap(counter, False)
				If km IsNot Nothing Then
					Dim actionID As Object = km.get(aKeyStroke)

					If am IsNot Nothing AndAlso actionID IsNot Nothing Then am.remove(actionID)
					km.remove(aKeyStroke)
				End If
			Next counter
		End Sub

		''' <summary>
		''' Returns the <code>KeyStrokes</code> that will initiate
		''' registered actions.
		''' </summary>
		''' <returns> an array of <code>KeyStroke</code> objects </returns>
		''' <seealso cref= #registerKeyboardAction </seealso>
		Public Overridable Property registeredKeyStrokes As KeyStroke()
			Get
				Dim counts As Integer() = New Integer(2){}
				Dim strokes As KeyStroke()() = New KeyStroke(2)(){}
    
				For counter As Integer = 0 To 2
					Dim km As InputMap = getInputMap(counter, False)
					strokes(counter) = If(km IsNot Nothing, km.allKeys(), Nothing)
					counts(counter) = If(strokes(counter) IsNot Nothing, strokes(counter).Length, 0)
				Next counter
				Dim retValue As KeyStroke() = New KeyStroke(counts(0) + counts(1) + counts(2) - 1){}
				Dim counter As Integer = 0
				Dim last As Integer = 0
				Do While counter < 3
					If counts(counter) > 0 Then
						Array.Copy(strokes(counter), 0, retValue, last, counts(counter))
						last += counts(counter)
					End If
					counter += 1
				Loop
				Return retValue
			End Get
		End Property

		''' <summary>
		''' Returns the condition that determines whether a registered action
		''' occurs in response to the specified keystroke.
		''' <p>
		''' For Java 2 platform v1.3, a <code>KeyStroke</code> can be associated
		''' with more than one condition.
		''' For example, 'a' could be bound for the two
		''' conditions <code>WHEN_FOCUSED</code> and
		''' <code>WHEN_IN_FOCUSED_WINDOW</code> condition.
		''' </summary>
		''' <returns> the action-keystroke condition </returns>
		Public Overridable Function getConditionForKeyStroke(ByVal aKeyStroke As KeyStroke) As Integer
			For counter As Integer = 0 To 2
				Dim ___inputMap As InputMap = getInputMap(counter, False)
				If ___inputMap IsNot Nothing AndAlso ___inputMap.get(aKeyStroke) IsNot Nothing Then Return counter
			Next counter
			Return UNDEFINED_CONDITION
		End Function

		''' <summary>
		''' Returns the object that will perform the action registered for a
		''' given keystroke.
		''' </summary>
		''' <returns> the <code>ActionListener</code>
		'''          object invoked when the keystroke occurs </returns>
		Public Overridable Function getActionForKeyStroke(ByVal aKeyStroke As KeyStroke) As ActionListener
			Dim am As ActionMap = getActionMap(False)

			If am Is Nothing Then Return Nothing
			For counter As Integer = 0 To 2
				Dim ___inputMap As InputMap = getInputMap(counter, False)
				If ___inputMap IsNot Nothing Then
					Dim actionBinding As Object = ___inputMap.get(aKeyStroke)

					If actionBinding IsNot Nothing Then
						Dim action As Action = am.get(actionBinding)
						If TypeOf action Is ActionStandin Then Return CType(action, ActionStandin).actionListener
						Return action
					End If
				End If
			Next counter
			Return Nothing
		End Function

		''' <summary>
		''' Unregisters all the bindings in the first tier <code>InputMaps</code>
		''' and <code>ActionMap</code>. This has the effect of removing any
		''' local bindings, and allowing the bindings defined in parent
		''' <code>InputMap/ActionMaps</code>
		''' (the UI is usually defined in the second tier) to persist.
		''' </summary>
		Public Overridable Sub resetKeyboardActions()
			' Keys
			For counter As Integer = 0 To 2
				Dim ___inputMap As InputMap = getInputMap(counter, False)

				If ___inputMap IsNot Nothing Then ___inputMap.clear()
			Next counter

			' Actions
			Dim am As ActionMap = getActionMap(False)

			If am IsNot Nothing Then am.clear()
		End Sub

		''' <summary>
		''' Sets the <code>InputMap</code> to use under the condition
		''' <code>condition</code> to
		''' <code>map</code>. A <code>null</code> value implies you
		''' do not want any bindings to be used, even from the UI. This will
		''' not reinstall the UI <code>InputMap</code> (if there was one).
		''' <code>condition</code> has one of the following values:
		''' <ul>
		''' <li><code>WHEN_IN_FOCUSED_WINDOW</code>
		''' <li><code>WHEN_FOCUSED</code>
		''' <li><code>WHEN_ANCESTOR_OF_FOCUSED_COMPONENT</code>
		''' </ul>
		''' If <code>condition</code> is <code>WHEN_IN_FOCUSED_WINDOW</code>
		''' and <code>map</code> is not a <code>ComponentInputMap</code>, an
		''' <code>IllegalArgumentException</code> will be thrown.
		''' Similarly, if <code>condition</code> is not one of the values
		''' listed, an <code>IllegalArgumentException</code> will be thrown.
		''' </summary>
		''' <param name="condition"> one of the values listed above </param>
		''' <param name="map">  the <code>InputMap</code> to use for the given condition </param>
		''' <exception cref="IllegalArgumentException"> if <code>condition</code> is
		'''          <code>WHEN_IN_FOCUSED_WINDOW</code> and <code>map</code>
		'''          is not an instance of <code>ComponentInputMap</code>; or
		'''          if <code>condition</code> is not one of the legal values
		'''          specified above
		''' @since 1.3 </exception>
		Public Sub setInputMap(ByVal condition As Integer, ByVal map As InputMap)
			Select Case condition
			Case WHEN_IN_FOCUSED_WINDOW
				If map IsNot Nothing AndAlso Not(TypeOf map Is ComponentInputMap) Then Throw New System.ArgumentException("WHEN_IN_FOCUSED_WINDOW InputMaps must be of type ComponentInputMap")
				windowInputMap = CType(map, ComponentInputMap)
				flaglag(WIF_INPUTMAP_CREATED, True)
				registerWithKeyboardManager(False)
			Case WHEN_ANCESTOR_OF_FOCUSED_COMPONENT
				ancestorInputMap = map
				flaglag(ANCESTOR_INPUTMAP_CREATED, True)
			Case WHEN_FOCUSED
				focusInputMap = map
				flaglag(FOCUS_INPUTMAP_CREATED, True)
			Case Else
				Throw New System.ArgumentException("condition must be one of JComponent.WHEN_IN_FOCUSED_WINDOW, JComponent.WHEN_FOCUSED or JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT")
			End Select
		End Sub

		''' <summary>
		''' Returns the <code>InputMap</code> that is used during
		''' <code>condition</code>.
		''' </summary>
		''' <param name="condition"> one of WHEN_IN_FOCUSED_WINDOW, WHEN_FOCUSED,
		'''        WHEN_ANCESTOR_OF_FOCUSED_COMPONENT </param>
		''' <returns> the <code>InputMap</code> for the specified
		'''          <code>condition</code>
		''' @since 1.3 </returns>
		Public Function getInputMap(ByVal condition As Integer) As InputMap
			Return getInputMap(condition, True)
		End Function

		''' <summary>
		''' Returns the <code>InputMap</code> that is used when the
		''' component has focus.
		''' This is convenience method for <code>getInputMap(WHEN_FOCUSED)</code>.
		''' </summary>
		''' <returns> the <code>InputMap</code> used when the component has focus
		''' @since 1.3 </returns>
		Public Property inputMap As InputMap
			Get
				Return getInputMap(WHEN_FOCUSED, True)
			End Get
		End Property

		''' <summary>
		''' Sets the <code>ActionMap</code> to <code>am</code>. This does not set
		''' the parent of the <code>am</code> to be the <code>ActionMap</code>
		''' from the UI (if there was one), it is up to the caller to have done this.
		''' </summary>
		''' <param name="am">  the new <code>ActionMap</code>
		''' @since 1.3 </param>
		Public Property actionMap As ActionMap
			Set(ByVal am As ActionMap)
				actionMap = am
				flaglag(ACTIONMAP_CREATED, True)
			End Set
			Get
				Return getActionMap(True)
			End Get
		End Property


		''' <summary>
		''' Returns the <code>InputMap</code> to use for condition
		''' <code>condition</code>.  If the <code>InputMap</code> hasn't
		''' been created, and <code>create</code> is
		''' true, it will be created.
		''' </summary>
		''' <param name="condition"> one of the following values:
		''' <ul>
		''' <li>JComponent.FOCUS_INPUTMAP_CREATED
		''' <li>JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT
		''' <li>JComponent.WHEN_IN_FOCUSED_WINDOW
		''' </ul> </param>
		''' <param name="create"> if true, create the <code>InputMap</code> if it
		'''          is not already created </param>
		''' <returns> the <code>InputMap</code> for the given <code>condition</code>;
		'''          if <code>create</code> is false and the <code>InputMap</code>
		'''          hasn't been created, returns <code>null</code> </returns>
		''' <exception cref="IllegalArgumentException"> if <code>condition</code>
		'''          is not one of the legal values listed above </exception>
		Friend Function getInputMap(ByVal condition As Integer, ByVal create As Boolean) As InputMap
			Select Case condition
			Case WHEN_FOCUSED
				If getFlag(FOCUS_INPUTMAP_CREATED) Then Return focusInputMap
				' Hasn't been created yet.
				If create Then
					Dim km As New InputMap
					inputMapMap(condition, km)
					Return km
				End If
			Case WHEN_ANCESTOR_OF_FOCUSED_COMPONENT
				If getFlag(ANCESTOR_INPUTMAP_CREATED) Then Return ancestorInputMap
				' Hasn't been created yet.
				If create Then
					Dim km As New InputMap
					inputMapMap(condition, km)
					Return km
				End If
			Case WHEN_IN_FOCUSED_WINDOW
				If getFlag(WIF_INPUTMAP_CREATED) Then Return windowInputMap
				' Hasn't been created yet.
				If create Then
					Dim km As New ComponentInputMap(Me)
					inputMapMap(condition, km)
					Return km
				End If
			Case Else
				Throw New System.ArgumentException("condition must be one of JComponent.WHEN_IN_FOCUSED_WINDOW, JComponent.WHEN_FOCUSED or JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT")
			End Select
			Return Nothing
		End Function

		''' <summary>
		''' Finds and returns the appropriate <code>ActionMap</code>.
		''' </summary>
		''' <param name="create"> if true, create the <code>ActionMap</code> if it
		'''          is not already created </param>
		''' <returns> the <code>ActionMap</code> for this component; if the
		'''          <code>create</code> flag is false and there is no
		'''          current <code>ActionMap</code>, returns <code>null</code> </returns>
		Friend Function getActionMap(ByVal create As Boolean) As ActionMap
			If getFlag(ACTIONMAP_CREATED) Then Return actionMap
			' Hasn't been created.
			If create Then
				Dim am As New ActionMap
				actionMap = am
				Return am
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the baseline.  The baseline is measured from the top of
		''' the component.  This method is primarily meant for
		''' <code>LayoutManager</code>s to align components along their
		''' baseline.  A return value less than 0 indicates this component
		''' does not have a reasonable baseline and that
		''' <code>LayoutManager</code>s should not align this component on
		''' its baseline.
		''' <p>
		''' This method calls into the <code>ComponentUI</code> method of the
		''' same name.  If this component does not have a <code>ComponentUI</code>
		''' -1 will be returned.  If a value &gt;= 0 is
		''' returned, then the component has a valid baseline for any
		''' size &gt;= the minimum size and <code>getBaselineResizeBehavior</code>
		''' can be used to determine how the baseline changes with size.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= #getBaselineResizeBehavior </seealso>
		''' <seealso cref= java.awt.FontMetrics
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal width As Integer, ByVal height As Integer) As Integer
			' check size.
			MyBase.getBaseline(width, height)
			If ui IsNot Nothing Then Return ui.getBaseline(Me, width, height)
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.  This method is primarily meant for
		''' layout managers and GUI builders.
		''' <p>
		''' This method calls into the <code>ComponentUI</code> method of
		''' the same name.  If this component does not have a
		''' <code>ComponentUI</code>
		''' <code>BaselineResizeBehavior.OTHER</code> will be
		''' returned.  Subclasses should
		''' never return <code>null</code>; if the baseline can not be
		''' calculated return <code>BaselineResizeBehavior.OTHER</code>.  Callers
		''' should first ask for the baseline using
		''' <code>getBaseline</code> and if a value &gt;= 0 is returned use
		''' this method.  It is acceptable for this method to return a
		''' value other than <code>BaselineResizeBehavior.OTHER</code> even if
		''' <code>getBaseline</code> returns a value less than 0.
		''' </summary>
		''' <seealso cref= #getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Property baselineResizeBehavior As BaselineResizeBehavior
			Get
				If ui IsNot Nothing Then Return ui.getBaselineResizeBehavior(Me)
				Return BaselineResizeBehavior.OTHER
			End Get
		End Property

		''' <summary>
		''' In release 1.4, the focus subsystem was rearchitected.
		''' For more information, see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
		''' How to Use the Focus Subsystem</a>,
		''' a section in <em>The Java Tutorial</em>.
		''' <p>
		''' Requests focus on this <code>JComponent</code>'s
		''' <code>FocusTraversalPolicy</code>'s default <code>Component</code>.
		''' If this <code>JComponent</code> is a focus cycle root, then its
		''' <code>FocusTraversalPolicy</code> is used. Otherwise, the
		''' <code>FocusTraversalPolicy</code> of this <code>JComponent</code>'s
		''' focus-cycle-root ancestor is used.
		''' </summary>
		''' <seealso cref= java.awt.FocusTraversalPolicy#getDefaultComponent </seealso>
		''' @deprecated As of 1.4, replaced by
		''' <code>FocusTraversalPolicy.getDefaultComponent(Container).requestFocus()</code> 
		<Obsolete("As of 1.4, replaced by")> _
		Public Overridable Function requestDefaultFocus() As Boolean
			Dim nearestRoot As Container = If(focusCycleRoot, Me, focusCycleRootAncestor)
			If nearestRoot Is Nothing Then Return False
			Dim comp As Component = nearestRoot.focusTraversalPolicy.getDefaultComponent(nearestRoot)
			If comp IsNot Nothing Then
				comp.requestFocus()
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Makes the component visible or invisible.
		''' Overrides <code>Component.setVisible</code>.
		''' </summary>
		''' <param name="aFlag">  true to make the component visible; false to
		'''          make it invisible
		''' 
		''' @beaninfo
		'''    attribute: visualUpdate true </param>
		Public Overridable Property visible As Boolean
			Set(ByVal aFlag As Boolean)
				If aFlag <> visible Then
					MyBase.visible = aFlag
					If aFlag Then
						Dim parent As Container = parent
						If parent IsNot Nothing Then
							Dim r As Rectangle = bounds
							parent.repaint(r.x, r.y, r.width, r.height)
						End If
						revalidate()
					End If
				End If
			End Set
		End Property

		''' <summary>
		''' Sets whether or not this component is enabled.
		''' A component that is enabled may respond to user input,
		''' while a component that is not enabled cannot respond to
		''' user input.  Some components may alter their visual
		''' representation when they are disabled in order to
		''' provide feedback to the user that they cannot take input.
		''' <p>Note: Disabling a component does not disable its children.
		''' 
		''' <p>Note: Disabling a lightweight component does not prevent it from
		''' receiving MouseEvents.
		''' </summary>
		''' <param name="enabled"> true if this component should be enabled, false otherwise </param>
		''' <seealso cref= java.awt.Component#isEnabled </seealso>
		''' <seealso cref= java.awt.Component#isLightweight
		''' 
		''' @beaninfo
		'''    preferred: true
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The enabled state of the component. </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setEnabled(ByVal enabled As Boolean) 'JavaToDotNetTempPropertySetenabled
		Public Overridable Property enabled As Boolean
			Set(ByVal enabled As Boolean)
				Dim oldEnabled As Boolean = enabled
				MyBase.enabled = enabled
				firePropertyChange("enabled", oldEnabled, enabled)
				If enabled <> oldEnabled Then repaint()
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the foreground color of this component.  It is up to the
		''' look and feel to honor this property, some may choose to ignore
		''' it.
		''' </summary>
		''' <param name="fg">  the desired foreground <code>Color</code> </param>
		''' <seealso cref= java.awt.Component#getForeground
		''' 
		''' @beaninfo
		'''    preferred: true
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The foreground color of the component. </seealso>
		Public Overridable Property foreground As Color
			Set(ByVal fg As Color)
				Dim oldFg As Color = foreground
				MyBase.foreground = fg
				If If(oldFg IsNot Nothing, (Not oldFg.Equals(fg)), ((fg IsNot Nothing) AndAlso (Not fg.Equals(oldFg)))) Then repaint()
			End Set
		End Property

		''' <summary>
		''' Sets the background color of this component.  The background
		''' color is used only if the component is opaque, and only
		''' by subclasses of <code>JComponent</code> or
		''' <code>ComponentUI</code> implementations.  Direct subclasses of
		''' <code>JComponent</code> must override
		''' <code>paintComponent</code> to honor this property.
		''' <p>
		''' It is up to the look and feel to honor this property, some may
		''' choose to ignore it.
		''' </summary>
		''' <param name="bg"> the desired background <code>Color</code> </param>
		''' <seealso cref= java.awt.Component#getBackground </seealso>
		''' <seealso cref= #setOpaque
		''' 
		''' @beaninfo
		'''    preferred: true
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The background color of the component. </seealso>
		Public Overridable Property background As Color
			Set(ByVal bg As Color)
				Dim oldBg As Color = background
				MyBase.background = bg
				If If(oldBg IsNot Nothing, (Not oldBg.Equals(bg)), ((bg IsNot Nothing) AndAlso (Not bg.Equals(oldBg)))) Then repaint()
			End Set
		End Property

		''' <summary>
		''' Sets the font for this component.
		''' </summary>
		''' <param name="font"> the desired <code>Font</code> for this component </param>
		''' <seealso cref= java.awt.Component#getFont
		''' 
		''' @beaninfo
		'''    preferred: true
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The font for the component. </seealso>
		Public Overridable Property font As Font
			Set(ByVal font As Font)
				Dim oldFont As Font = font
				MyBase.font = font
				' font already bound in AWT1.2
				If font IsNot oldFont Then
					revalidate()
					repaint()
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the default locale used to initialize each JComponent's
		''' locale property upon creation.
		''' 
		''' The default locale has "AppContext" scope so that applets (and
		''' potentially multiple lightweight applications running in a single VM)
		''' can have their own setting. An applet can safely alter its default
		''' locale because it will have no affect on other applets (or the browser).
		''' </summary>
		''' <returns> the default <code>Locale</code>. </returns>
		''' <seealso cref= #setDefaultLocale </seealso>
		''' <seealso cref= java.awt.Component#getLocale </seealso>
		''' <seealso cref= #setLocale
		''' @since 1.4 </seealso>
		Public Property Shared defaultLocale As java.util.Locale
			Get
				Dim l As java.util.Locale = CType(SwingUtilities.appContextGet(defaultLocale), java.util.Locale)
				If l Is Nothing Then
					'REMIND(bcb) choosing the default value is more complicated
					'than this.
					l = java.util.Locale.default
					JComponent.defaultLocale = l
				End If
				Return l
			End Get
			Set(ByVal l As java.util.Locale)
				SwingUtilities.appContextPut(defaultLocale, l)
			End Set
		End Property




		''' <summary>
		''' Processes any key events that the component itself
		''' recognizes.  This is called after the focus
		''' manager and any interested listeners have been
		''' given a chance to steal away the event.  This
		''' method is called only if the event has not
		''' yet been consumed.  This method is called prior
		''' to the keyboard UI logic.
		''' <p>
		''' This method is implemented to do nothing.  Subclasses would
		''' normally override this method if they process some
		''' key events themselves.  If the event is processed,
		''' it should be consumed.
		''' </summary>
		Protected Friend Overridable Sub processComponentKeyEvent(ByVal e As KeyEvent)
		End Sub

		''' <summary>
		''' Overrides <code>processKeyEvent</code> to process events. * </summary>
		Protected Friend Overridable Sub processKeyEvent(ByVal e As KeyEvent)
		  Dim result As Boolean
		  Dim shouldProcessKey As Boolean

		  ' This gives the key event listeners a crack at the event
		  MyBase.processKeyEvent(e)

		  ' give the component itself a crack at the event
		  If Not e.consumed Then processComponentKeyEvent(e)

		  shouldProcessKey = KeyboardState.shouldProcess(e)

		  If e.consumed Then Return

		  If shouldProcessKey AndAlso processKeyBindings(e, e.iD = KeyEvent.KEY_PRESSED) Then e.consume()
		End Sub

		''' <summary>
		''' Invoked to process the key bindings for <code>ks</code> as the result
		''' of the <code>KeyEvent</code> <code>e</code>. This obtains
		''' the appropriate <code>InputMap</code>,
		''' gets the binding, gets the action from the <code>ActionMap</code>,
		''' and then (if the action is found and the component
		''' is enabled) invokes <code>notifyAction</code> to notify the action.
		''' </summary>
		''' <param name="ks">  the <code>KeyStroke</code> queried </param>
		''' <param name="e"> the <code>KeyEvent</code> </param>
		''' <param name="condition"> one of the following values:
		''' <ul>
		''' <li>JComponent.WHEN_FOCUSED
		''' <li>JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT
		''' <li>JComponent.WHEN_IN_FOCUSED_WINDOW
		''' </ul> </param>
		''' <param name="pressed"> true if the key is pressed </param>
		''' <returns> true if there was a binding to an action, and the action
		'''         was enabled
		''' 
		''' @since 1.3 </returns>
		Protected Friend Overridable Function processKeyBinding(ByVal ks As KeyStroke, ByVal e As KeyEvent, ByVal condition As Integer, ByVal pressed As Boolean) As Boolean
			Dim map As InputMap = getInputMap(condition, False)
			Dim am As ActionMap = getActionMap(False)

			If map IsNot Nothing AndAlso am IsNot Nothing AndAlso enabled Then
				Dim binding As Object = map.get(ks)
				Dim action As Action = If(binding Is Nothing, Nothing, am.get(binding))
				If action IsNot Nothing Then Return SwingUtilities.notifyAction(action, ks, e, Me, e.modifiers)
			End If
			Return False
		End Function

		''' <summary>
		''' This is invoked as the result of a <code>KeyEvent</code>
		''' that was not consumed by the <code>FocusManager</code>,
		''' <code>KeyListeners</code>, or the component. It will first try
		''' <code>WHEN_FOCUSED</code> bindings,
		''' then <code>WHEN_ANCESTOR_OF_FOCUSED_COMPONENT</code> bindings,
		''' and finally <code>WHEN_IN_FOCUSED_WINDOW</code> bindings.
		''' </summary>
		''' <param name="e"> the unconsumed <code>KeyEvent</code> </param>
		''' <param name="pressed"> true if the key is pressed </param>
		''' <returns> true if there is a key binding for <code>e</code> </returns>
		Friend Overridable Function processKeyBindings(ByVal e As KeyEvent, ByVal pressed As Boolean) As Boolean
		  If Not SwingUtilities.isValidKeyEventForKeyBindings(e) Then Return False
		  ' Get the KeyStroke
		  ' There may be two keystrokes associated with a low-level key event;
		  ' in this case a keystroke made of an extended key code has a priority.
		  Dim ks As KeyStroke
		  Dim ksE As KeyStroke = Nothing

		  If e.iD = KeyEvent.KEY_TYPED Then
			  ks = KeyStroke.getKeyStroke(e.keyChar)
		  Else
			  ks = KeyStroke.getKeyStroke(e.keyCode,e.modifiers, (If(pressed, False, True)))
			  If e.keyCode <> e.extendedKeyCode Then ksE = KeyStroke.getKeyStroke(e.extendedKeyCode,e.modifiers, (If(pressed, False, True)))
		  End If

		  ' Do we have a key binding for e?
		  ' If we have a binding by an extended code, use it.
		  ' If not, check for regular code binding.
		  If ksE IsNot Nothing AndAlso processKeyBinding(ksE, e, WHEN_FOCUSED, pressed) Then Return True
		  If processKeyBinding(ks, e, WHEN_FOCUSED, pressed) Then Return True

	'       We have no key binding. Let's try the path from our parent to the
	'       * window excluded. We store the path components so we can avoid
	'       * asking the same component twice.
	'       
		  Dim parent As Container = Me
		  Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is Window) AndAlso Not(TypeOf parent Is java.applet.Applet)
			  If TypeOf parent Is JComponent Then
				  If ksE IsNot Nothing AndAlso CType(parent, JComponent).processKeyBinding(ksE, e, WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, pressed) Then Return True
				  If CType(parent, JComponent).processKeyBinding(ks, e, WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, pressed) Then Return True
			  End If
			  ' This is done so that the children of a JInternalFrame are
			  ' given precedence for WHEN_IN_FOCUSED_WINDOW bindings before
			  ' other components WHEN_IN_FOCUSED_WINDOW bindings. This also gives
			  ' more precedence to the WHEN_IN_FOCUSED_WINDOW bindings of the
			  ' JInternalFrame's children vs the
			  ' WHEN_ANCESTOR_OF_FOCUSED_COMPONENT bindings of the parents.
			  ' maybe generalize from JInternalFrame (like isFocusCycleRoot).
			  If (TypeOf parent Is JInternalFrame) AndAlso JComponent.processKeyBindingsForAllComponents(e,parent,pressed) Then Return True
			  parent = parent.parent
		  Loop

	'       No components between the focused component and the window is
	'       * actually interested by the key event. Let's try the other
	'       * JComponent in this window.
	'       
		  If parent IsNot Nothing Then Return JComponent.processKeyBindingsForAllComponents(e,parent,pressed)
		  Return False
		End Function

		Friend Shared Function processKeyBindingsForAllComponents(ByVal e As KeyEvent, ByVal container As Container, ByVal pressed As Boolean) As Boolean
			Do
				If KeyboardManager.currentManager.fireKeyboardAction(e, pressed, container) Then Return True
				If TypeOf container Is Popup.HeavyWeightWindow Then
					container = CType(container, Window).owner
				Else
					Return False
				End If
			Loop
		End Function

		''' <summary>
		''' Registers the text to display in a tool tip.
		''' The text displays when the cursor lingers over the component.
		''' <p>
		''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tooltip.html">How to Use Tool Tips</a>
		''' in <em>The Java Tutorial</em>
		''' for further documentation.
		''' </summary>
		''' <param name="text">  the string to display; if the text is <code>null</code>,
		'''              the tool tip is turned off for this component </param>
		''' <seealso cref= #TOOL_TIP_TEXT_KEY
		''' @beaninfo
		'''   preferred: true
		''' description: The text to display in a tool tip. </seealso>
		Public Overridable Property toolTipText As String
			Set(ByVal text As String)
				Dim oldText As String = toolTipText
				putClientProperty(TOOL_TIP_TEXT_KEY, text)
				Dim ___toolTipManager As ToolTipManager = ToolTipManager.sharedInstance()
				If text IsNot Nothing Then
					If oldText Is Nothing Then ___toolTipManager.registerComponent(Me)
				Else
					___toolTipManager.unregisterComponent(Me)
				End If
			End Set
			Get
				Return CStr(getClientProperty(TOOL_TIP_TEXT_KEY))
			End Get
		End Property



		''' <summary>
		''' Returns the string to be used as the tooltip for <i>event</i>.
		''' By default this returns any string set using
		''' <code>setToolTipText</code>.  If a component provides
		''' more extensive API to support differing tooltips at different locations,
		''' this method should be overridden.
		''' </summary>
		Public Overridable Function getToolTipText(ByVal [event] As MouseEvent) As String
			Return toolTipText
		End Function

		''' <summary>
		''' Returns the tooltip location in this component's coordinate system.
		''' If <code>null</code> is returned, Swing will choose a location.
		''' The default implementation returns <code>null</code>.
		''' </summary>
		''' <param name="event">  the <code>MouseEvent</code> that caused the
		'''          <code>ToolTipManager</code> to show the tooltip </param>
		''' <returns> always returns <code>null</code> </returns>
		Public Overridable Function getToolTipLocation(ByVal [event] As MouseEvent) As Point
			Return Nothing
		End Function

		''' <summary>
		''' Returns the preferred location to display the popup menu in this
		''' component's coordinate system. It is up to the look and feel to
		''' honor this property, some may choose to ignore it.
		''' If {@code null}, the look and feel will choose a suitable location.
		''' </summary>
		''' <param name="event"> the {@code MouseEvent} that triggered the popup to be
		'''        shown, or {@code null} if the popup is not being shown as the
		'''        result of a mouse event </param>
		''' <returns> location to display the {@code JPopupMenu}, or {@code null}
		''' @since 1.5 </returns>
		Public Overridable Function getPopupLocation(ByVal [event] As MouseEvent) As Point
			Return Nothing
		End Function


		''' <summary>
		''' Returns the instance of <code>JToolTip</code> that should be used
		''' to display the tooltip.
		''' Components typically would not override this method,
		''' but it can be used to
		''' cause different tooltips to be displayed differently.
		''' </summary>
		''' <returns> the <code>JToolTip</code> used to display this toolTip </returns>
		Public Overridable Function createToolTip() As JToolTip
			Dim tip As New JToolTip
			tip.component = Me
			Return tip
		End Function

		''' <summary>
		''' Forwards the <code>scrollRectToVisible()</code> message to the
		''' <code>JComponent</code>'s parent. Components that can service
		''' the request, such as <code>JViewport</code>,
		''' override this method and perform the scrolling.
		''' </summary>
		''' <param name="aRect"> the visible <code>Rectangle</code> </param>
		''' <seealso cref= JViewport </seealso>
		Public Overridable Sub scrollRectToVisible(ByVal aRect As Rectangle)
			Dim parent As Container
			Dim dx As Integer = x, dy As Integer = y

			parent = parent
			Do While Not(parent Is Nothing) AndAlso Not(TypeOf parent Is JComponent) AndAlso Not(TypeOf parent Is CellRendererPane)
				 Dim ___bounds As Rectangle = parent.bounds

				 dx += ___bounds.x
				 dy += ___bounds.y
				parent = parent.parent
			Loop

			If Not(parent Is Nothing) AndAlso Not(TypeOf parent Is CellRendererPane) Then
				aRect.x += dx
				aRect.y += dy

				CType(parent, JComponent).scrollRectToVisible(aRect)
				aRect.x -= dx
				aRect.y -= dy
			End If
		End Sub

		''' <summary>
		''' Sets the <code>autoscrolls</code> property.
		''' If <code>true</code> mouse dragged events will be
		''' synthetically generated when the mouse is dragged
		''' outside of the component's bounds and mouse motion
		''' has paused (while the button continues to be held
		''' down). The synthetic events make it appear that the
		''' drag gesture has resumed in the direction established when
		''' the component's boundary was crossed.  Components that
		''' support autoscrolling must handle <code>mouseDragged</code>
		''' events by calling <code>scrollRectToVisible</code> with a
		''' rectangle that contains the mouse event's location.  All of
		''' the Swing components that support item selection and are
		''' typically displayed in a <code>JScrollPane</code>
		''' (<code>JTable</code>, <code>JList</code>, <code>JTree</code>,
		''' <code>JTextArea</code>, and <code>JEditorPane</code>)
		''' already handle mouse dragged events in this way.  To enable
		''' autoscrolling in any other component, add a mouse motion
		''' listener that calls <code>scrollRectToVisible</code>.
		''' For example, given a <code>JPanel</code>, <code>myPanel</code>:
		''' <pre>
		''' MouseMotionListener doScrollRectToVisible = new MouseMotionAdapter() {
		'''     public void mouseDragged(MouseEvent e) {
		'''        Rectangle r = new Rectangle(e.getX(), e.getY(), 1, 1);
		'''        ((JPanel)e.getSource()).scrollRectToVisible(r);
		'''    }
		''' };
		''' myPanel.addMouseMotionListener(doScrollRectToVisible);
		''' </pre>
		''' The default value of the <code>autoScrolls</code>
		''' property is <code>false</code>.
		''' </summary>
		''' <param name="autoscrolls"> if true, synthetic mouse dragged events
		'''   are generated when the mouse is dragged outside of a component's
		'''   bounds and the mouse button continues to be held down; otherwise
		'''   false </param>
		''' <seealso cref= #getAutoscrolls </seealso>
		''' <seealso cref= JViewport </seealso>
		''' <seealso cref= JScrollPane
		''' 
		''' @beaninfo
		'''      expert: true
		''' description: Determines if this component automatically scrolls its contents when dragged. </seealso>
		Public Overridable Property autoscrolls As Boolean
			Set(ByVal autoscrolls As Boolean)
				flaglag(AUTOSCROLLS_SET, True)
				If Me.autoscrolls <> autoscrolls Then
					Me.autoscrolls = autoscrolls
					If autoscrolls Then
						enableEvents(AWTEvent.MOUSE_EVENT_MASK)
						enableEvents(AWTEvent.MOUSE_MOTION_EVENT_MASK)
					Else
						Autoscroller.stop(Me)
					End If
				End If
			End Set
			Get
				Return autoscrolls
			End Get
		End Property


		''' <summary>
		''' Sets the {@code TransferHandler}, which provides support for transfer
		''' of data into and out of this component via cut/copy/paste and drag
		''' and drop. This may be {@code null} if the component does not support
		''' data transfer operations.
		''' <p>
		''' If the new {@code TransferHandler} is not {@code null}, this method
		''' also installs a <b>new</b> {@code DropTarget} on the component to
		''' activate drop handling through the {@code TransferHandler} and activate
		''' any built-in support (such as calculating and displaying potential drop
		''' locations). If you do not wish for this component to respond in any way
		''' to drops, you can disable drop support entirely either by removing the
		''' drop target ({@code setDropTarget(null)}) or by de-activating it
		''' ({@code getDropTaget().setActive(false)}).
		''' <p>
		''' If the new {@code TransferHandler} is {@code null}, this method removes
		''' the drop target.
		''' <p>
		''' Under two circumstances, this method does not modify the drop target:
		''' First, if the existing drop target on this component was explicitly
		''' set by the developer to a {@code non-null} value. Second, if the
		''' system property {@code suppressSwingDropSupport} is {@code true}. The
		''' default value for the system property is {@code false}.
		''' <p>
		''' Please see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
		''' How to Use Drag and Drop and Data Transfer</a>,
		''' a section in <em>The Java Tutorial</em>, for more information.
		''' </summary>
		''' <param name="newHandler"> the new {@code TransferHandler}
		''' </param>
		''' <seealso cref= TransferHandler </seealso>
		''' <seealso cref= #getTransferHandler
		''' @since 1.4
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''  description: Mechanism for transfer of data to and from the component </seealso>
		Public Overridable Property transferHandler As TransferHandler
			Set(ByVal newHandler As TransferHandler)
				Dim oldHandler As TransferHandler = CType(getClientProperty(JComponent_TRANSFER_HANDLER), TransferHandler)
				putClientProperty(JComponent_TRANSFER_HANDLER, newHandler)
    
				SwingUtilities.installSwingDropTargetAsNecessary(Me, newHandler)
				firePropertyChange("transferHandler", oldHandler, newHandler)
			End Set
			Get
				Return CType(getClientProperty(JComponent_TRANSFER_HANDLER), TransferHandler)
			End Get
		End Property


		''' <summary>
		''' Calculates a custom drop location for this type of component,
		''' representing where a drop at the given point should insert data.
		''' <code>null</code> is returned if this component doesn't calculate
		''' custom drop locations. In this case, <code>TransferHandler</code>
		''' will provide a default <code>DropLocation</code> containing just
		''' the point.
		''' </summary>
		''' <param name="p"> the point to calculate a drop location for </param>
		''' <returns> the drop location, or <code>null</code> </returns>
		Friend Overridable Function dropLocationForPoint(ByVal p As Point) As TransferHandler.DropLocation
			Return Nothing
		End Function

		''' <summary>
		''' Called to set or clear the drop location during a DnD operation.
		''' In some cases, the component may need to use its internal selection
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
		''' </summary>
		''' <param name="location"> the drop location (as calculated by
		'''        <code>dropLocationForPoint</code>) or <code>null</code>
		'''        if there's no longer a valid drop location </param>
		''' <param name="state"> the state object saved earlier for this component,
		'''        or <code>null</code> </param>
		''' <param name="forDrop"> whether or not the method is being called because an
		'''        actual drop occurred </param>
		''' <returns> any saved state for this component, or <code>null</code> if none </returns>
		Friend Overridable Function setDropLocation(ByVal location As TransferHandler.DropLocation, ByVal state As Object, ByVal forDrop As Boolean) As Object

			Return Nothing
		End Function

		''' <summary>
		''' Called to indicate to this component that DnD is done.
		''' Needed by <code>JTree</code>.
		''' </summary>
		Friend Overridable Sub dndDone()
		End Sub

		''' <summary>
		''' Processes mouse events occurring on this component by
		''' dispatching them to any registered
		''' <code>MouseListener</code> objects, refer to
		''' <seealso cref="java.awt.Component#processMouseEvent(MouseEvent)"/>
		''' for a complete description of this method.
		''' </summary>
		''' <param name="e"> the mouse event </param>
		''' <seealso cref=         java.awt.Component#processMouseEvent
		''' @since       1.5 </seealso>
		Protected Friend Overridable Sub processMouseEvent(ByVal e As MouseEvent)
			If autoscrolls AndAlso e.iD = MouseEvent.MOUSE_RELEASED Then Autoscroller.stop(Me)
			MyBase.processMouseEvent(e)
		End Sub

		''' <summary>
		''' Processes mouse motion events, such as MouseEvent.MOUSE_DRAGGED.
		''' </summary>
		''' <param name="e"> the <code>MouseEvent</code> </param>
		''' <seealso cref= MouseEvent </seealso>
		Protected Friend Overridable Sub processMouseMotionEvent(ByVal e As MouseEvent)
			Dim dispatch As Boolean = True
			If autoscrolls AndAlso e.iD = MouseEvent.MOUSE_DRAGGED Then
				' We don't want to do the drags when the mouse moves if we're
				' autoscrolling.  It makes it feel spastic.
				dispatch = Not Autoscroller.isRunning(Me)
				Autoscroller.processMouseDragged(e)
			End If
			If dispatch Then MyBase.processMouseMotionEvent(e)
		End Sub

		' Inner classes can't get at this method from a super class
		Friend Overridable Sub superProcessMouseMotionEvent(ByVal e As MouseEvent)
			MyBase.processMouseMotionEvent(e)
		End Sub

		''' <summary>
		''' This is invoked by the <code>RepaintManager</code> if
		''' <code>createImage</code> is called on the component.
		''' </summary>
		''' <param name="newValue"> true if the double buffer image was created from this component </param>
		Friend Overridable Property createdDoubleBuffer As Boolean
			Set(ByVal newValue As Boolean)
				flaglag(CREATED_DOUBLE_BUFFER, newValue)
			End Set
			Get
				Return getFlag(CREATED_DOUBLE_BUFFER)
			End Get
		End Property


		''' <summary>
		''' <code>ActionStandin</code> is used as a standin for
		''' <code>ActionListeners</code> that are
		''' added via <code>registerKeyboardAction</code>.
		''' </summary>
		Friend NotInheritable Class ActionStandin
			Implements Action

			Private ReadOnly outerInstance As JComponent

			Private ReadOnly actionListener As ActionListener
			Private ReadOnly command As String
			' This will be non-null if actionListener is an Action.
			Private ReadOnly action As Action

			Friend Sub New(ByVal outerInstance As JComponent, ByVal actionListener As ActionListener, ByVal command As String)
					Me.outerInstance = outerInstance
				Me.actionListener = actionListener
				If TypeOf actionListener Is Action Then
					Me.action = CType(actionListener, Action)
				Else
					Me.action = Nothing
				End If
				Me.command = command
			End Sub

			Public Function getValue(ByVal key As String) As Object Implements Action.getValue
				If key IsNot Nothing Then
					If key.Equals(Action.ACTION_COMMAND_KEY) Then Return command
					If action IsNot Nothing Then Return action.getValue(key)
					If key.Equals(NAME) Then Return "ActionStandin"
				End If
				Return Nothing
			End Function

				If actionListener Is Nothing Then Return False
				If action Is Nothing Then Return True
				Return action.enabled
			End Function

			Public Sub actionPerformed(ByVal ae As ActionEvent)
				If actionListener IsNot Nothing Then actionListener.actionPerformed(ae)
			End Sub

			' We don't allow any values to be added.
			Public Sub putValue(ByVal key As String, ByVal value As Object) Implements Action.putValue
			End Sub

			' Does nothing, our enabledness is determiend from our asociated
			' action.
			Public Property enabled Implements Action.setEnabled As Boolean
				Set(ByVal b As Boolean)
				End Set
			End Property

			Public Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			End Sub
			Public Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			End Sub
		End Class


		' This class is used by the KeyboardState class to provide a single
		' instance that can be stored in the AppContext.
		Friend NotInheritable Class IntVector
			Friend array As Integer() = Nothing
			Friend count As Integer = 0
			Friend capacity As Integer = 0

			Friend Function size() As Integer
				Return count
			End Function

			Friend Function elementAt(ByVal index As Integer) As Integer
				Return array(index)
			End Function

			Friend Sub addElement(ByVal value As Integer)
				If count = capacity Then
					capacity = (capacity + 2) * 2
					Dim newarray As Integer() = New Integer(capacity - 1){}
					If count > 0 Then Array.Copy(array, 0, newarray, 0, count)
					array = newarray
				End If
				array(count) = value
				count += 1
			End Sub

			Friend Sub setElementAt(ByVal value As Integer, ByVal index As Integer)
				array(index) = value
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Friend Class KeyboardState
			Private Shared ReadOnly keyCodesKey As Object = GetType(JComponent.KeyboardState)

			' Get the array of key codes from the AppContext.
			Friend Property Shared keyCodeArray As IntVector
				Get
					Dim iv As IntVector = CType(SwingUtilities.appContextGet(keyCodesKey), IntVector)
					If iv Is Nothing Then
						iv = New IntVector
						SwingUtilities.appContextPut(keyCodesKey, iv)
					End If
					Return iv
				End Get
			End Property

			Friend Shared Sub registerKeyPressed(ByVal keyCode As Integer)
				Dim kca As IntVector = keyCodeArray
				Dim count As Integer = kca.size()
				Dim i As Integer
				For i = 0 To count - 1
					If kca.elementAt(i) = -1 Then
						kca.elementAttAt(keyCode, i)
						Return
					End If
				Next i
				kca.addElement(keyCode)
			End Sub

			Friend Shared Sub registerKeyReleased(ByVal keyCode As Integer)
				Dim kca As IntVector = keyCodeArray
				Dim count As Integer = kca.size()
				Dim i As Integer
				For i = 0 To count - 1
					If kca.elementAt(i) = keyCode Then
						kca.elementAttAt(-1, i)
						Return
					End If
				Next i
			End Sub

			Friend Shared Function keyIsPressed(ByVal keyCode As Integer) As Boolean
				Dim kca As IntVector = keyCodeArray
				Dim count As Integer = kca.size()
				Dim i As Integer
				For i = 0 To count - 1
					If kca.elementAt(i) = keyCode Then Return True
				Next i
				Return False
			End Function

			''' <summary>
			''' Updates internal state of the KeyboardState and returns true
			''' if the event should be processed further.
			''' </summary>
			Friend Shared Function shouldProcess(ByVal e As KeyEvent) As Boolean
				Select Case e.iD
				Case KeyEvent.KEY_PRESSED
					If Not keyIsPressed(e.keyCode) Then registerKeyPressed(e.keyCode)
					Return True
				Case KeyEvent.KEY_RELEASED
					' We are forced to process VK_PRINTSCREEN separately because
					' the Windows doesn't generate the key pressed event for
					' printscreen and it block the processing of key release
					' event for printscreen.
					If keyIsPressed(e.keyCode) OrElse e.keyCode=KeyEvent.VK_PRINTSCREEN Then
						registerKeyReleased(e.keyCode)
						Return True
					End If
					Return False
				Case KeyEvent.KEY_TYPED
					Return True
				Case Else
					' Not a known KeyEvent type, bail.
					Return False
				End Select
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		static final sun.awt.RequestFocusController focusController = New sun.awt.RequestFocusController()
	'	{
	'			public boolean acceptRequestFocus(Component from, Component to, boolean temporary, boolean focusedWindowChangeAllowed, sun.awt.CausedFocusEvent.Cause cause)
	'			{
	'				if ((to == Nothing) || !(to instanceof JComponent))
	'				{
	'					Return True;
	'				}
	'
	'				if ((from == Nothing) || !(from instanceof JComponent))
	'				{
	'					Return True;
	'				}
	'
	'				JComponent target = (JComponent) to;
	'				if (!target.getVerifyInputWhenFocusTarget())
	'				{
	'					Return True;
	'				}
	'
	'				JComponent jFocusOwner = (JComponent)from;
	'				InputVerifier iv = jFocusOwner.getInputVerifier();
	'
	'				if (iv == Nothing)
	'				{
	'					Return True;
	'				}
	'				else
	'				{
	'					Object currentSource = SwingUtilities.appContextGet(INPUT_VERIFIER_SOURCE_KEY);
	'					if (currentSource == jFocusOwner)
	'					{
	'						' We're currently calling into the InputVerifier
	'						' for this component, so allow the focus change.
	'						Return True;
	'					}
	'					SwingUtilities.appContextPut(INPUT_VERIFIER_SOURCE_KEY, jFocusOwner);
	'					try
	'					{
	'						Return iv.shouldYieldFocus(jFocusOwner);
	'					}
	'					finally
	'					{
	'						if (currentSource != Nothing)
	'						{
	'							' We're already in the InputVerifier for
	'							' currentSource. By resetting the currentSource
	'							' we ensure that if the InputVerifier for
	'							' currentSource does a requestFocus, we don't
	'							' try and run the InputVerifier again.
	'							SwingUtilities.appContextPut(INPUT_VERIFIER_SOURCE_KEY, currentSource);
	'						}
	'						else
	'						{
	'							SwingUtilities.appContextRemove(INPUT_VERIFIER_SOURCE_KEY);
	'						}
	'					}
	'				}
	'			}
	'		};

	'    
	'     * --- Accessibility Support ---
	'     

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>java.awt.Component.setEnabled(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub enable()
			If enabled <> True Then
				MyBase.enable()
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.ENABLED)
			End If
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>java.awt.Component.setEnabled(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub disable()
			If enabled <> False Then
				MyBase.disable()
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.ENABLED, Nothing)
			End If
		End Sub

		''' <summary>
		''' Inner class of JComponent used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by component developers.
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
		Public MustInherit Class AccessibleJComponent
			Inherits AccessibleAWTContainer
			Implements AccessibleExtendedComponent

			Private ReadOnly outerInstance As JComponent

			''' <summary>
			''' Though the class is abstract, this should be called by
			''' all sub-classes.
			''' </summary>
			Protected Friend Sub New(ByVal outerInstance As JComponent)
					Me.outerInstance = outerInstance
				MyBase.New()
			End Sub

			''' <summary>
			''' Number of PropertyChangeListener objects registered. It's used
			''' to add/remove ContainerListener and FocusListener to track
			''' target JComponent's state
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			<NonSerialized> _
			Private propertyListenersCount As Integer = 0

			''' <summary>
			''' This field duplicates the function of the accessibleAWTFocusHandler field
			''' in java.awt.Component.AccessibleAWTComponent, so it has been deprecated.
			''' </summary>
			<Obsolete> _
			Protected Friend accessibleFocusHandler As FocusListener = Nothing

			''' <summary>
			''' Fire PropertyChange listener, if one is registered,
			''' when children added/removed.
			''' </summary>
			Protected Friend Class AccessibleContainerHandler
				Implements ContainerListener

				Private ReadOnly outerInstance As JComponent.AccessibleJComponent

				Public Sub New(ByVal outerInstance As JComponent.AccessibleJComponent)
					Me.outerInstance = outerInstance
				End Sub

				Public Overridable Sub componentAdded(ByVal e As ContainerEvent)
					Dim c As Component = e.child
					If c IsNot Nothing AndAlso TypeOf c Is Accessible Then outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, Nothing, c.accessibleContext)
				End Sub
				Public Overridable Sub componentRemoved(ByVal e As ContainerEvent)
					Dim c As Component = e.child
					If c IsNot Nothing AndAlso TypeOf c Is Accessible Then outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY, c.accessibleContext, Nothing)
				End Sub
			End Class

			''' <summary>
			''' Fire PropertyChange listener, if one is registered,
			''' when focus events happen
			''' @since 1.3
			''' </summary>
			Protected Friend Class AccessibleFocusHandler
				Implements FocusListener

				Private ReadOnly outerInstance As JComponent.AccessibleJComponent

				Public Sub New(ByVal outerInstance As JComponent.AccessibleJComponent)
					Me.outerInstance = outerInstance
				End Sub

			   Public Overridable Sub focusGained(ByVal [event] As FocusEvent)
				   If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
			   End Sub
				Public Overridable Sub focusLost(ByVal [event] As FocusEvent)
					If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.FOCUSED, Nothing)
				End Sub
			End Class ' inner class AccessibleFocusHandler


			''' <summary>
			''' Adds a PropertyChangeListener to the listener list.
			''' </summary>
			''' <param name="listener">  the PropertyChangeListener to be added </param>
			Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
				MyBase.addPropertyChangeListener(listener)
			End Sub

			''' <summary>
			''' Removes a PropertyChangeListener from the listener list.
			''' This removes a PropertyChangeListener that was registered
			''' for all properties.
			''' </summary>
			''' <param name="listener">  the PropertyChangeListener to be removed </param>
			Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
				MyBase.removePropertyChangeListener(listener)
			End Sub



			''' <summary>
			''' Recursively search through the border hierarchy (if it exists)
			''' for a TitledBorder with a non-null title.  This does a depth
			''' first search on first the inside borders then the outside borders.
			''' The assumption is that titles make really pretty inside borders
			''' but not very pretty outside borders in compound border situations.
			''' It's rather arbitrary, but hopefully decent UI programmers will
			''' not create multiple titled borders for the same component.
			''' </summary>
			Protected Friend Overridable Function getBorderTitle(ByVal b As Border) As String
				Dim s As String
				If TypeOf b Is TitledBorder Then
					Return CType(b, TitledBorder).title
				ElseIf TypeOf b Is CompoundBorder Then
					s = getBorderTitle(CType(b, CompoundBorder).insideBorder)
					If s Is Nothing Then s = getBorderTitle(CType(b, CompoundBorder).outsideBorder)
					Return s
				Else
					Return Nothing
				End If
			End Function

			' AccessibleContext methods
			'
			''' <summary>
			''' Gets the accessible name of this object.  This should almost never
			''' return java.awt.Component.getName(), as that generally isn't
			''' a localized name, and doesn't have meaning for the user.  If the
			''' object is fundamentally a text object (such as a menu item), the
			''' accessible name should be the text of the object (for example,
			''' "save").
			''' If the object has a tooltip, the tooltip text may also be an
			''' appropriate String to return.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			'''         object does not have a name </returns>
			''' <seealso cref= AccessibleContext#setAccessibleName </seealso>
			Public Overridable Property accessibleName As String
				Get
					Dim name As String = accessibleName
    
					' fallback to the client name property
					'
					If name Is Nothing Then name = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
    
					' fallback to the titled border if it exists
					'
					If name Is Nothing Then name = getBorderTitle(outerInstance.border)
    
					' fallback to the label labeling us if it exists
					'
					If name Is Nothing Then
						Dim o As Object = outerInstance.getClientProperty(JLabel.LABELED_BY_PROPERTY)
						If TypeOf o Is Accessible Then
							Dim ac As AccessibleContext = CType(o, Accessible).accessibleContext
							If ac IsNot Nothing Then name = ac.accessibleName
						End If
					End If
					Return name
				End Get
			End Property

			''' <summary>
			''' Gets the accessible description of this object.  This should be
			''' a concise, localized description of what this object is - what
			''' is its meaning to the user.  If the object has a tooltip, the
			''' tooltip text may be an appropriate string to return, assuming
			''' it contains a concise description of the object (instead of just
			''' the name of the object - for example a "Save" icon on a toolbar that
			''' had "save" as the tooltip text shouldn't return the tooltip
			''' text as the description, but something like "Saves the current
			''' text document" instead).
			''' </summary>
			''' <returns> the localized description of the object -- can be null if
			''' this object does not have a description </returns>
			''' <seealso cref= AccessibleContext#setAccessibleDescription </seealso>
			Public Overridable Property accessibleDescription As String
				Get
					Dim description As String = accessibleDescription
    
					' fallback to the client description property
					'
					If description Is Nothing Then description = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY))
    
					' fallback to the tool tip text if it exists
					'
					If description Is Nothing Then
						Try
							description = toolTipText
						Catch e As Exception
							' Just in case the subclass overrode the
							' getToolTipText method and actually
							' requires a MouseEvent.
							' [[[FIXME:  WDW - we probably should require this
							' method to take a MouseEvent and just pass it on
							' to getToolTipText.  The swing-feedback traffic
							' leads me to believe getToolTipText might change,
							' though, so I was hesitant to make this change at
							' this time.]]]
						End Try
					End If
    
					' fallback to the label labeling us if it exists
					'
					If description Is Nothing Then
						Dim o As Object = outerInstance.getClientProperty(JLabel.LABELED_BY_PROPERTY)
						If TypeOf o Is Accessible Then
							Dim ac As AccessibleContext = CType(o, Accessible).accessibleContext
							If ac IsNot Nothing Then description = ac.accessibleDescription
						End If
					End If
    
					Return description
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SWING_COMPONENT
				End Get
			End Property

			''' <summary>
			''' Gets the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.opaque Then states.add(AccessibleState.OPAQUE)
					Return states
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return MyBase.accessibleChildrenCount
				End Get
			End Property

			''' <summary>
			''' Returns the nth Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				Return MyBase.getAccessibleChild(i)
			End Function

			' ----- AccessibleExtendedComponent

			''' <summary>
			''' Returns the AccessibleExtendedComponent
			''' </summary>
			''' <returns> the AccessibleExtendedComponent </returns>
			Friend Overridable Property accessibleExtendedComponent As AccessibleExtendedComponent
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the tool tip text
			''' </summary>
			''' <returns> the tool tip text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property toolTipText As String Implements AccessibleExtendedComponent.getToolTipText
				Get
					Return outerInstance.toolTipText
				End Get
			End Property

			''' <summary>
			''' Returns the titled border text
			''' </summary>
			''' <returns> the titled border text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property titledBorderText As String Implements AccessibleExtendedComponent.getTitledBorderText
				Get
					Dim border As Border = outerInstance.border
					If TypeOf border Is TitledBorder Then
						Return CType(border, TitledBorder).title
					Else
						Return Nothing
					End If
				End Get
			End Property

			''' <summary>
			''' Returns key bindings associated with this object
			''' </summary>
			''' <returns> the key bindings, if supported, of the object;
			''' otherwise, null </returns>
			''' <seealso cref= AccessibleKeyBinding
			''' @since 1.4 </seealso>
			Public Overridable Property accessibleKeyBinding As AccessibleKeyBinding Implements AccessibleExtendedComponent.getAccessibleKeyBinding
				Get
					' Try to get the linked label's mnemonic if it exists
					Dim o As Object = outerInstance.getClientProperty(JLabel.LABELED_BY_PROPERTY)
					If TypeOf o Is Accessible Then
						Dim ac As AccessibleContext = CType(o, Accessible).accessibleContext
						If ac IsNot Nothing Then
							Dim comp As AccessibleComponent = ac.accessibleComponent
							If Not(TypeOf comp Is AccessibleExtendedComponent) Then Return Nothing
							Return CType(comp, AccessibleExtendedComponent).accessibleKeyBinding
						End If
					End If
					Return Nothing
				End Get
			End Property
		End Class ' inner class AccessibleJComponent


		''' <summary>
		''' Returns an <code>ArrayTable</code> used for
		''' key/value "client properties" for this component. If the
		''' <code>clientProperties</code> table doesn't exist, an empty one
		''' will be created.
		''' </summary>
		''' <returns> an ArrayTable </returns>
		''' <seealso cref= #putClientProperty </seealso>
		''' <seealso cref= #getClientProperty </seealso>
		Private Property clientProperties As ArrayTable
			Get
				If clientProperties Is Nothing Then clientProperties = New ArrayTable
				Return clientProperties
			End Get
		End Property


		''' <summary>
		''' Returns the value of the property with the specified key.  Only
		''' properties added with <code>putClientProperty</code> will return
		''' a non-<code>null</code> value.
		''' </summary>
		''' <param name="key"> the being queried </param>
		''' <returns> the value of this property or <code>null</code> </returns>
		''' <seealso cref= #putClientProperty </seealso>
		Public Function getClientProperty(ByVal key As Object) As Object
			If key Is sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY Then
				Return aaTextInfo
			ElseIf key Is sun.swing.SwingUtilities2.COMPONENT_UI_PROPERTY_KEY Then
				Return ui
			End If
			 If clientProperties Is Nothing Then
				Return Nothing
			Else
				SyncLock clientProperties
					Return clientProperties.get(key)
				End SyncLock
			End If
		End Function

		''' <summary>
		''' Adds an arbitrary key/value "client property" to this component.
		''' <p>
		''' The <code>get/putClientProperty</code> methods provide access to
		''' a small per-instance hashtable. Callers can use get/putClientProperty
		''' to annotate components that were created by another module.
		''' For example, a
		''' layout manager might store per child constraints this way. For example:
		''' <pre>
		''' componentA.putClientProperty("to the left of", componentB);
		''' </pre>
		''' If value is <code>null</code> this method will remove the property.
		''' Changes to client properties are reported with
		''' <code>PropertyChange</code> events.
		''' The name of the property (for the sake of PropertyChange
		''' events) is <code>key.toString()</code>.
		''' <p>
		''' The <code>clientProperty</code> dictionary is not intended to
		''' support large
		''' scale extensions to JComponent nor should be it considered an
		''' alternative to subclassing when designing a new component.
		''' </summary>
		''' <param name="key"> the new client property key </param>
		''' <param name="value"> the new client property value; if <code>null</code>
		'''          this method will remove the property </param>
		''' <seealso cref= #getClientProperty </seealso>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		Public Sub putClientProperty(ByVal key As Object, ByVal value As Object)
			If key Is sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY Then
				aaTextInfo = value
				Return
			End If
			If value Is Nothing AndAlso clientProperties Is Nothing Then Return
			Dim ___clientProperties As ArrayTable = clientProperties
			Dim oldValue As Object
			SyncLock ___clientProperties
				oldValue = ___clientProperties.get(key)
				If value IsNot Nothing Then
					___clientProperties.put(key, value)
				ElseIf oldValue IsNot Nothing Then
					___clientProperties.remove(key)
				Else
					' old == new == null
					Return
				End If
			End SyncLock
			clientPropertyChanged(key, oldValue, value)
			firePropertyChange(key.ToString(), oldValue, value)
		End Sub

		' Invoked from putClientProperty.  This is provided for subclasses
		' in Swing.
		Friend Overridable Sub clientPropertyChanged(ByVal key As Object, ByVal oldValue As Object, ByVal newValue As Object)
		End Sub


	'    
	'     * Sets the property with the specified name to the specified value if
	'     * the property has not already been set by the client program.
	'     * This method is used primarily to set UI defaults for properties
	'     * with primitive types, where the values cannot be marked with
	'     * UIResource.
	'     * @see LookAndFeel#installProperty
	'     * @param propertyName String containing the name of the property
	'     * @param value Object containing the property value
	'     
		Friend Overridable Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "opaque" Then
				If Not getFlag(OPAQUE_SET) Then
					opaque = CBool(value)
					flaglag(OPAQUE_SET, False)
				End If
			ElseIf propertyName = "autoscrolls" Then
				If Not getFlag(AUTOSCROLLS_SET) Then
					autoscrolls = CBool(value)
					flaglag(AUTOSCROLLS_SET, False)
				End If
			ElseIf propertyName = "focusTraversalKeysForward" Then
				If Not getFlag(FOCUS_TRAVERSAL_KEYS_FORWARD_SET) Then MyBase.focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, CType(value, java.util.Set(Of AWTKeyStroke)))
			ElseIf propertyName = "focusTraversalKeysBackward" Then
				If Not getFlag(FOCUS_TRAVERSAL_KEYS_BACKWARD_SET) Then MyBase.focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, CType(value, java.util.Set(Of AWTKeyStroke)))
			Else
				Throw New System.ArgumentException("property """ & propertyName & """ cannot be set using this method")
			End If
		End Sub


		''' <summary>
		''' Sets the focus traversal keys for a given traversal operation for this
		''' Component.
		''' Refer to
		''' <seealso cref="java.awt.Component#setFocusTraversalKeys"/>
		''' for a complete description of this method.
		''' <p>
		''' This method may throw a {@code ClassCastException} if any {@code Object}
		''' in {@code keystrokes} is not an {@code AWTKeyStroke}.
		''' </summary>
		''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''        KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		'''        KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS </param>
		''' <param name="keystrokes"> the Set of AWTKeyStroke for the specified operation </param>
		''' <seealso cref= java.awt.KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= java.awt.KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= java.awt.KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <exception cref="IllegalArgumentException"> if id is not one of
		'''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, or
		'''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or if keystrokes
		'''         contains null, or if any keystroke represents a KEY_TYPED event,
		'''         or if any keystroke already maps to another focus traversal
		'''         operation for this Component
		''' @since 1.5
		''' @beaninfo
		'''       bound: true </exception>
		Public Overridable Sub setFocusTraversalKeys(Of T1 As AWTKeyStroke)(ByVal id As Integer, ByVal keystrokes As java.util.Set(Of T1))
			If id = KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS Then
				flaglag(FOCUS_TRAVERSAL_KEYS_FORWARD_SET,True)
			ElseIf id = KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS Then
				flaglag(FOCUS_TRAVERSAL_KEYS_BACKWARD_SET,True)
			End If
			MyBase.focusTraversalKeyseys(id,keystrokes)
		End Sub

	'     --- Transitional java.awt.Component Support ---
	'     * The methods and fields in this section will migrate to
	'     * java.awt.Component in the next JDK release.
	'     

		''' <summary>
		''' Returns true if this component is lightweight, that is, if it doesn't
		''' have a native window system peer.
		''' </summary>
		''' <returns> true if this component is lightweight </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function isLightweightComponent(ByVal c As Component) As Boolean
			Return TypeOf c.peer Is java.awt.peer.LightweightPeer
		End Function


		''' @deprecated As of JDK 5,
		''' replaced by <code>Component.setBounds(int, int, int, int)</code>.
		''' <p>
		''' Moves and resizes this component.
		''' 
		''' <param name="x">  the new horizontal location </param>
		''' <param name="y">  the new vertical location </param>
		''' <param name="w">  the new width </param>
		''' <param name="h">  the new height </param>
		''' <seealso cref= java.awt.Component#setBounds </seealso>
		<Obsolete("As of JDK 5,")> _
		Public Overridable Sub reshape(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			MyBase.reshape(x, y, w, h)
		End Sub


		''' <summary>
		''' Stores the bounds of this component into "return value"
		''' <code>rv</code> and returns <code>rv</code>.
		''' If <code>rv</code> is <code>null</code> a new <code>Rectangle</code>
		''' is allocated.  This version of <code>getBounds</code> is useful
		''' if the caller wants to avoid allocating a new <code>Rectangle</code>
		''' object on the heap.
		''' </summary>
		''' <param name="rv"> the return value, modified to the component's bounds </param>
		''' <returns> <code>rv</code>; if <code>rv</code> is <code>null</code>
		'''          return a newly created <code>Rectangle</code> with this
		'''          component's bounds </returns>
		Public Overridable Function getBounds(ByVal rv As Rectangle) As Rectangle
			If rv Is Nothing Then
				Return New Rectangle(x, y, width, height)
			Else
				rv.boundsnds(x, y, width, height)
				Return rv
			End If
		End Function


		''' <summary>
		''' Stores the width/height of this component into "return value"
		''' <code>rv</code> and returns <code>rv</code>.
		''' If <code>rv</code> is <code>null</code> a new <code>Dimension</code>
		''' object is allocated.  This version of <code>getSize</code>
		''' is useful if the caller wants to avoid allocating a new
		''' <code>Dimension</code> object on the heap.
		''' </summary>
		''' <param name="rv"> the return value, modified to the component's size </param>
		''' <returns> <code>rv</code> </returns>
		Public Overridable Function getSize(ByVal rv As Dimension) As Dimension
			If rv Is Nothing Then
				Return New Dimension(width, height)
			Else
				rv.sizeize(width, height)
				Return rv
			End If
		End Function


		''' <summary>
		''' Stores the x,y origin of this component into "return value"
		''' <code>rv</code> and returns <code>rv</code>.
		''' If <code>rv</code> is <code>null</code> a new <code>Point</code>
		''' is allocated.  This version of <code>getLocation</code> is useful
		''' if the caller wants to avoid allocating a new <code>Point</code>
		''' object on the heap.
		''' </summary>
		''' <param name="rv"> the return value, modified to the component's location </param>
		''' <returns> <code>rv</code> </returns>
		Public Overridable Function getLocation(ByVal rv As Point) As Point
			If rv Is Nothing Then
				Return New Point(x, y)
			Else
				rv.locationion(x, y)
				Return rv
			End If
		End Function


		''' <summary>
		''' Returns the current x coordinate of the component's origin.
		''' This method is preferable to writing
		''' <code>component.getBounds().x</code>, or
		''' <code>component.getLocation().x</code> because it doesn't cause any
		''' heap allocations.
		''' </summary>
		''' <returns> the current x coordinate of the component's origin </returns>
		Public Overridable Property x As Integer
			Get
				Return MyBase.x
			End Get
		End Property


		''' <summary>
		''' Returns the current y coordinate of the component's origin.
		''' This method is preferable to writing
		''' <code>component.getBounds().y</code>, or
		''' <code>component.getLocation().y</code> because it doesn't cause any
		''' heap allocations.
		''' </summary>
		''' <returns> the current y coordinate of the component's origin </returns>
		Public Overridable Property y As Integer
			Get
				Return MyBase.y
			End Get
		End Property


		''' <summary>
		''' Returns the current width of this component.
		''' This method is preferable to writing
		''' <code>component.getBounds().width</code>, or
		''' <code>component.getSize().width</code> because it doesn't cause any
		''' heap allocations.
		''' </summary>
		''' <returns> the current width of this component </returns>
		Public Overridable Property width As Integer
			Get
				Return MyBase.width
			End Get
		End Property


		''' <summary>
		''' Returns the current height of this component.
		''' This method is preferable to writing
		''' <code>component.getBounds().height</code>, or
		''' <code>component.getSize().height</code> because it doesn't cause any
		''' heap allocations.
		''' </summary>
		''' <returns> the current height of this component </returns>
		Public Overridable Property height As Integer
			Get
				Return MyBase.height
			End Get
		End Property

		''' <summary>
		''' Returns true if this component is completely opaque.
		''' <p>
		''' An opaque component paints every pixel within its
		''' rectangular bounds. A non-opaque component paints only a subset of
		''' its pixels or none at all, allowing the pixels underneath it to
		''' "show through".  Therefore, a component that does not fully paint
		''' its pixels provides a degree of transparency.
		''' <p>
		''' Subclasses that guarantee to always completely paint their contents
		''' should override this method and return true.
		''' </summary>
		''' <returns> true if this component is completely opaque </returns>
		''' <seealso cref= #setOpaque </seealso>
		Public Overridable Property opaque As Boolean
			Get
				Return getFlag(IS_OPAQUE)
			End Get
			Set(ByVal isOpaque As Boolean)
				Dim oldValue As Boolean = getFlag(IS_OPAQUE)
				flaglag(IS_OPAQUE, isOpaque)
				flaglag(OPAQUE_SET, True)
				firePropertyChange("opaque", oldValue, isOpaque)
			End Set
		End Property



		''' <summary>
		''' If the specified rectangle is completely obscured by any of this
		''' component's opaque children then returns true.  Only direct children
		''' are considered, more distant descendants are ignored.  A
		''' <code>JComponent</code> is opaque if
		''' <code>JComponent.isOpaque()</code> returns true, other lightweight
		''' components are always considered transparent, and heavyweight components
		''' are always considered opaque.
		''' </summary>
		''' <param name="x">  x value of specified rectangle </param>
		''' <param name="y">  y value of specified rectangle </param>
		''' <param name="width">  width of specified rectangle </param>
		''' <param name="height"> height of specified rectangle </param>
		''' <returns> true if the specified rectangle is obscured by an opaque child </returns>
		Friend Overridable Function rectangleIsObscured(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Boolean
			Dim numChildren As Integer = componentCount

			For i As Integer = 0 To numChildren - 1
				Dim child As Component = getComponent(i)
				Dim cx, cy, cw, ch As Integer

				cx = child.x
				cy = child.y
				cw = child.width
				ch = child.height

				If x >= cx AndAlso (x + width) <= (cx + cw) AndAlso y >= cy AndAlso (y + height) <= (cy + ch) AndAlso child.visible Then

					If TypeOf child Is JComponent Then
	'                  System.out.println("A) checking opaque: " + ((JComponent)child).isOpaque() + "  " + child);
	'                  System.out.print("B) ");
	'                  Thread.dumpStack();
						Return child.opaque
					Else
						''' <summary>
						''' Sometimes a heavy weight can have a bound larger than its peer size
						'''  so we should always draw under heavy weights
						''' </summary>
						Return False
					End If
				End If
			Next i

			Return False
		End Function


		''' <summary>
		''' Returns the <code>Component</code>'s "visible rect rectangle" -  the
		''' intersection of the visible rectangles for the component <code>c</code>
		''' and all of its ancestors.  The return value is stored in
		''' <code>visibleRect</code>.
		''' </summary>
		''' <param name="c">  the component </param>
		''' <param name="visibleRect">  a <code>Rectangle</code> computed as the
		'''          intersection of all visible rectangles for the component
		'''          <code>c</code> and all of its ancestors -- this is the
		'''          return value for this method </param>
		''' <seealso cref= #getVisibleRect </seealso>
		Friend Shared Sub computeVisibleRect(ByVal c As Component, ByVal visibleRect As Rectangle)
			Dim p As Container = c.parent
			Dim ___bounds As Rectangle = c.bounds

			If p Is Nothing OrElse TypeOf p Is Window OrElse TypeOf p Is java.applet.Applet Then
				visibleRect.boundsnds(0, 0, ___bounds.width, ___bounds.height)
			Else
				computeVisibleRect(p, visibleRect)
				visibleRect.x -= ___bounds.x
				visibleRect.y -= ___bounds.y
				SwingUtilities.computeIntersection(0,0,___bounds.width,___bounds.height,visibleRect)
			End If
		End Sub


		''' <summary>
		''' Returns the <code>Component</code>'s "visible rect rectangle" -  the
		''' intersection of the visible rectangles for this component
		''' and all of its ancestors.  The return value is stored in
		''' <code>visibleRect</code>.
		''' </summary>
		''' <param name="visibleRect"> a <code>Rectangle</code> computed as the
		'''          intersection of all visible rectangles for this
		'''          component and all of its ancestors -- this is the return
		'''          value for this method </param>
		''' <seealso cref= #getVisibleRect </seealso>
		Public Overridable Sub computeVisibleRect(ByVal visibleRect As Rectangle)
			computeVisibleRect(Me, visibleRect)
		End Sub


		''' <summary>
		''' Returns the <code>Component</code>'s "visible rectangle" -  the
		''' intersection of this component's visible rectangle,
		''' <code>new Rectangle(0, 0, getWidth(), getHeight())</code>,
		''' and all of its ancestors' visible rectangles.
		''' </summary>
		''' <returns> the visible rectangle </returns>
		Public Overridable Property visibleRect As Rectangle
			Get
				Dim ___visibleRect As New Rectangle
    
				computeVisibleRect(___visibleRect)
				Return ___visibleRect
			End Get
		End Property

		''' <summary>
		''' Support for reporting bound property changes for boolean properties.
		''' This method can be called when a bound property has changed and it will
		''' send the appropriate PropertyChangeEvent to any registered
		''' PropertyChangeListeners.
		''' </summary>
		''' <param name="propertyName"> the property whose value has changed </param>
		''' <param name="oldValue"> the property's previous value </param>
		''' <param name="newValue"> the property's new value </param>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Boolean, ByVal newValue As Boolean)
			MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub


		''' <summary>
		''' Support for reporting bound property changes for integer properties.
		''' This method can be called when a bound property has changed and it will
		''' send the appropriate PropertyChangeEvent to any registered
		''' PropertyChangeListeners.
		''' </summary>
		''' <param name="propertyName"> the property whose value has changed </param>
		''' <param name="oldValue"> the property's previous value </param>
		''' <param name="newValue"> the property's new value </param>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Integer, ByVal newValue As Integer)
			MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		' XXX This method is implemented as a workaround to a JLS issue with ambiguous
		' methods. This should be removed once 4758654 is resolved.
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Char, ByVal newValue As Char)
			MyBase.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		''' <summary>
		''' Supports reporting constrained property changes.
		''' This method can be called when a constrained property has changed
		''' and it will send the appropriate <code>PropertyChangeEvent</code>
		''' to any registered <code>VetoableChangeListeners</code>.
		''' </summary>
		''' <param name="propertyName">  the name of the property that was listened on </param>
		''' <param name="oldValue">  the old value of the property </param>
		''' <param name="newValue">  the new value of the property </param>
		''' <exception cref="java.beans.PropertyVetoException"> when the attempt to set the
		'''          property is vetoed by the component </exception>
		Protected Friend Overridable Sub fireVetoableChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If vetoableChangeSupport Is Nothing Then Return
			vetoableChangeSupport.fireVetoableChange(propertyName, oldValue, newValue)
		End Sub


		''' <summary>
		''' Adds a <code>VetoableChangeListener</code> to the listener list.
		''' The listener is registered for all properties.
		''' </summary>
		''' <param name="listener">  the <code>VetoableChangeListener</code> to be added </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addVetoableChangeListener(ByVal listener As java.beans.VetoableChangeListener)
			If vetoableChangeSupport Is Nothing Then vetoableChangeSupport = New java.beans.VetoableChangeSupport(Me)
			vetoableChangeSupport.addVetoableChangeListener(listener)
		End Sub


		''' <summary>
		''' Removes a <code>VetoableChangeListener</code> from the listener list.
		''' This removes a <code>VetoableChangeListener</code> that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the <code>VetoableChangeListener</code> to be removed </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeVetoableChangeListener(ByVal listener As java.beans.VetoableChangeListener)
			If vetoableChangeSupport Is Nothing Then Return
			vetoableChangeSupport.removeVetoableChangeListener(listener)
		End Sub


		''' <summary>
		''' Returns an array of all the vetoable change listeners
		''' registered on this component.
		''' </summary>
		''' <returns> all of the component's <code>VetoableChangeListener</code>s
		'''         or an empty
		'''         array if no vetoable change listeners are currently registered
		''' </returns>
		''' <seealso cref= #addVetoableChangeListener </seealso>
		''' <seealso cref= #removeVetoableChangeListener
		''' 
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property vetoableChangeListeners As java.beans.VetoableChangeListener()
			Get
				If vetoableChangeSupport Is Nothing Then Return New java.beans.VetoableChangeListener(){}
				Return vetoableChangeSupport.vetoableChangeListeners
			End Get
		End Property


		''' <summary>
		''' Returns the top-level ancestor of this component (either the
		''' containing <code>Window</code> or <code>Applet</code>),
		''' or <code>null</code> if this component has not
		''' been added to any container.
		''' </summary>
		''' <returns> the top-level <code>Container</code> that this component is in,
		'''          or <code>null</code> if not in any container </returns>
		Public Overridable Property topLevelAncestor As Container
			Get
				Dim p As Container = Me
				Do While p IsNot Nothing
					If TypeOf p Is Window OrElse TypeOf p Is java.applet.Applet Then Return p
					p = p.parent
				Loop
				Return Nothing
			End Get
		End Property

		Private Property ancestorNotifier As AncestorNotifier
			Get
				Return CType(getClientProperty(JComponent_ANCESTOR_NOTIFIER), AncestorNotifier)
			End Get
		End Property

		''' <summary>
		''' Registers <code>listener</code> so that it will receive
		''' <code>AncestorEvents</code> when it or any of its ancestors
		''' move or are made visible or invisible.
		''' Events are also sent when the component or its ancestors are added
		''' or removed from the containment hierarchy.
		''' </summary>
		''' <param name="listener">  the <code>AncestorListener</code> to register </param>
		''' <seealso cref= AncestorEvent </seealso>
		Public Overridable Sub addAncestorListener(ByVal listener As AncestorListener)
			Dim ___ancestorNotifier As AncestorNotifier = ancestorNotifier
			If ___ancestorNotifier Is Nothing Then
				___ancestorNotifier = New AncestorNotifier(Me)
				putClientProperty(JComponent_ANCESTOR_NOTIFIER, ___ancestorNotifier)
			End If
			___ancestorNotifier.addAncestorListener(listener)
		End Sub

		''' <summary>
		''' Unregisters <code>listener</code> so that it will no longer receive
		''' <code>AncestorEvents</code>.
		''' </summary>
		''' <param name="listener">  the <code>AncestorListener</code> to be removed </param>
		''' <seealso cref= #addAncestorListener </seealso>
		Public Overridable Sub removeAncestorListener(ByVal listener As AncestorListener)
			Dim ___ancestorNotifier As AncestorNotifier = ancestorNotifier
			If ___ancestorNotifier Is Nothing Then Return
			___ancestorNotifier.removeAncestorListener(listener)
			If ___ancestorNotifier.listenerList.listenerList.Length = 0 Then
				___ancestorNotifier.removeAllListeners()
				putClientProperty(JComponent_ANCESTOR_NOTIFIER, Nothing)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the ancestor listeners
		''' registered on this component.
		''' </summary>
		''' <returns> all of the component's <code>AncestorListener</code>s
		'''         or an empty
		'''         array if no ancestor listeners are currently registered
		''' </returns>
		''' <seealso cref= #addAncestorListener </seealso>
		''' <seealso cref= #removeAncestorListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property ancestorListeners As AncestorListener()
			Get
				Dim ___ancestorNotifier As AncestorNotifier = ancestorNotifier
				If ___ancestorNotifier Is Nothing Then Return New AncestorListener(){}
				Return ___ancestorNotifier.ancestorListeners
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>JComponent</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,
		''' such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>JComponent</code> <code>c</code>
		''' for its mouse listeners with the following code:
		''' <pre>MouseListener[] mls = (MouseListener[])(c.getListeners(MouseListener.class));</pre>
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' 
		''' @since 1.3
		''' </exception>
		''' <seealso cref= #getVetoableChangeListeners </seealso>
		''' <seealso cref= #getAncestorListeners </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Dim result As T()
			If listenerType Is GetType(AncestorListener) Then
				' AncestorListeners are handled by the AncestorNotifier
				result = CType(ancestorListeners, T())
			ElseIf listenerType Is GetType(java.beans.VetoableChangeListener) Then
				' VetoableChangeListeners are handled by VetoableChangeSupport
				result = CType(vetoableChangeListeners, T())
			ElseIf listenerType Is GetType(java.beans.PropertyChangeListener) Then
				' PropertyChangeListeners are handled by PropertyChangeSupport
				result = CType(propertyChangeListeners, T())
			Else
				result = listenerList.getListeners(listenerType)
			End If

			If result.Length = 0 Then Return MyBase.getListeners(listenerType)
			Return result
		End Function

		''' <summary>
		''' Notifies this component that it now has a parent component.
		''' When this method is invoked, the chain of parent components is
		''' set up with <code>KeyboardAction</code> event listeners.
		''' This method is called by the toolkit internally and should
		''' not be called directly by programs.
		''' </summary>
		''' <seealso cref= #registerKeyboardAction </seealso>
		Public Overridable Sub addNotify()
			MyBase.addNotify()
			firePropertyChange("ancestor", Nothing, parent)

			registerWithKeyboardManager(False)
			registerNextFocusableComponent()
		End Sub


		''' <summary>
		''' Notifies this component that it no longer has a parent component.
		''' When this method is invoked, any <code>KeyboardAction</code>s
		''' set up in the the chain of parent components are removed.
		''' This method is called by the toolkit internally and should
		''' not be called directly by programs.
		''' </summary>
		''' <seealso cref= #registerKeyboardAction </seealso>
		Public Overridable Sub removeNotify()
			MyBase.removeNotify()
			' This isn't strictly correct.  The event shouldn't be
			' fired until *after* the parent is set to null.  But
			' we only get notified before that happens
			firePropertyChange("ancestor", parent, Nothing)

			unregisterWithKeyboardManager()
			deregisterNextFocusableComponent()

			If createdDoubleBuffer Then
				RepaintManager.currentManager(Me).resetDoubleBuffer()
				createdDoubleBuffer = False
			End If
			If autoscrolls Then Autoscroller.stop(Me)
		End Sub


		''' <summary>
		''' Adds the specified region to the dirty region list if the component
		''' is showing.  The component will be repainted after all of the
		''' currently pending events have been dispatched.
		''' </summary>
		''' <param name="tm">  this parameter is not used </param>
		''' <param name="x">  the x value of the dirty region </param>
		''' <param name="y">  the y value of the dirty region </param>
		''' <param name="width">  the width of the dirty region </param>
		''' <param name="height">  the height of the dirty region </param>
		''' <seealso cref= #isPaintingOrigin() </seealso>
		''' <seealso cref= java.awt.Component#isShowing </seealso>
		''' <seealso cref= RepaintManager#addDirtyRegion </seealso>
		Public Overridable Sub repaint(ByVal tm As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			RepaintManager.currentManager(sun.awt.SunToolkit.targetToAppContext(Me)).addDirtyRegion(Me, x, y, width, height)
		End Sub


		''' <summary>
		''' Adds the specified region to the dirty region list if the component
		''' is showing.  The component will be repainted after all of the
		''' currently pending events have been dispatched.
		''' </summary>
		''' <param name="r"> a <code>Rectangle</code> containing the dirty region </param>
		''' <seealso cref= #isPaintingOrigin() </seealso>
		''' <seealso cref= java.awt.Component#isShowing </seealso>
		''' <seealso cref= RepaintManager#addDirtyRegion </seealso>
		Public Overridable Sub repaint(ByVal r As Rectangle)
			repaint(0,r.x,r.y,r.width,r.height)
		End Sub


		''' <summary>
		''' Supports deferred automatic layout.
		''' <p>
		''' Calls <code>invalidate</code> and then adds this component's
		''' <code>validateRoot</code> to a list of components that need to be
		''' validated.  Validation will occur after all currently pending
		''' events have been dispatched.  In other words after this method
		''' is called,  the first validateRoot (if any) found when walking
		''' up the containment hierarchy of this component will be validated.
		''' By default, <code>JRootPane</code>, <code>JScrollPane</code>,
		''' and <code>JTextField</code> return true
		''' from <code>isValidateRoot</code>.
		''' <p>
		''' This method will automatically be called on this component
		''' when a property value changes such that size, location, or
		''' internal layout of this component has been affected.  This automatic
		''' updating differs from the AWT because programs generally no
		''' longer need to invoke <code>validate</code> to get the contents of the
		''' GUI to update.
		''' </summary>
		''' <seealso cref= java.awt.Component#invalidate </seealso>
		''' <seealso cref= java.awt.Container#validate </seealso>
		''' <seealso cref= #isValidateRoot </seealso>
		''' <seealso cref= RepaintManager#addInvalidComponent </seealso>
		Public Overridable Sub revalidate()
			If parent Is Nothing Then Return
			If sun.awt.SunToolkit.isDispatchThreadForAppContext(Me) Then
				invalidate()
				RepaintManager.currentManager(Me).addInvalidComponent(Me)
			Else
				' To avoid a flood of Runnables when constructing GUIs off
				' the EDT, a flag is maintained as to whether or not
				' a Runnable has been scheduled.
				If revalidateRunnableScheduled.getAndSet(True) Then Return
				sun.awt.SunToolkit.executeOnEventHandlerThread(Me, () -> { revalidateRunnableScheduled.set(False); revalidate(); })
			End If
		End Sub

		''' <summary>
		''' If this method returns true, <code>revalidate</code> calls by
		''' descendants of this component will cause the entire tree
		''' beginning with this root to be validated.
		''' Returns false by default.  <code>JScrollPane</code> overrides
		''' this method and returns true.
		''' </summary>
		''' <returns> always returns false </returns>
		''' <seealso cref= #revalidate </seealso>
		''' <seealso cref= java.awt.Component#invalidate </seealso>
		''' <seealso cref= java.awt.Container#validate </seealso>
		''' <seealso cref= java.awt.Container#isValidateRoot </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return False
			End Get
		End Property


		''' <summary>
		''' Returns true if this component tiles its children -- that is, if
		''' it can guarantee that the children will not overlap.  The
		''' repainting system is substantially more efficient in this
		''' common case.  <code>JComponent</code> subclasses that can't make this
		''' guarantee, such as <code>JLayeredPane</code>,
		''' should override this method to return false.
		''' </summary>
		''' <returns> always returns true </returns>
		Public Overridable Property optimizedDrawingEnabled As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if a paint triggered on a child component should cause
		''' painting to originate from this Component, or one of its ancestors.
		''' <p>
		''' Calling <seealso cref="#repaint"/> or <seealso cref="#paintImmediately(int, int, int, int)"/>
		''' on a Swing component will result in calling
		''' the <seealso cref="JComponent#paintImmediately(int, int, int, int)"/> method of
		''' the first ancestor which {@code isPaintingOrigin()} returns {@code true}, if there are any.
		''' <p>
		''' {@code JComponent} subclasses that need to be painted when any of their
		''' children are repainted should override this method to return {@code true}.
		''' </summary>
		''' <returns> always returns {@code false}
		''' </returns>
		''' <seealso cref= #paintImmediately(int, int, int, int) </seealso>
		Protected Friend Overridable Property paintingOrigin As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Paints the specified region in this component and all of its
		''' descendants that overlap the region, immediately.
		''' <p>
		''' It's rarely necessary to call this method.  In most cases it's
		''' more efficient to call repaint, which defers the actual painting
		''' and can collapse redundant requests into a single paint call.
		''' This method is useful if one needs to update the display while
		''' the current event is being dispatched.
		''' <p>
		''' This method is to be overridden when the dirty region needs to be changed
		''' for components that are painting origins.
		''' </summary>
		''' <param name="x">  the x value of the region to be painted </param>
		''' <param name="y">  the y value of the region to be painted </param>
		''' <param name="w">  the width of the region to be painted </param>
		''' <param name="h">  the height of the region to be painted </param>
		''' <seealso cref= #repaint </seealso>
		''' <seealso cref= #isPaintingOrigin() </seealso>
		Public Overridable Sub paintImmediately(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim c As Component = Me
			Dim parent As Component

			If Not showing Then Return

			Dim paintingOigin As JComponent = SwingUtilities.getPaintingOrigin(Me)
			If paintingOigin IsNot Nothing Then
				Dim rectangle As Rectangle = SwingUtilities.convertRectangle(c, New Rectangle(x, y, w, h), paintingOigin)
				paintingOigin.paintImmediately(rectangle.x, rectangle.y, rectangle.width, rectangle.height)
				Return
			End If

			Do While Not c.opaque
				parent = c.parent
				If parent IsNot Nothing Then
					x += c.x
					y += c.y
					c = parent
				Else
					Exit Do
				End If

				If Not(TypeOf c Is JComponent) Then Exit Do
			Loop
			If TypeOf c Is JComponent Then
				CType(c, JComponent)._paintImmediately(x,y,w,h)
			Else
				c.repaint(x,y,w,h)
			End If
		End Sub

		''' <summary>
		''' Paints the specified region now.
		''' </summary>
		''' <param name="r"> a <code>Rectangle</code> containing the region to be painted </param>
		Public Overridable Sub paintImmediately(ByVal r As Rectangle)
			paintImmediately(r.x,r.y,r.width,r.height)
		End Sub

		''' <summary>
		''' Returns whether this component should be guaranteed to be on top.
		''' For example, it would make no sense for <code>Menu</code>s to pop up
		''' under another component, so they would always return true.
		''' Most components will want to return false, hence that is the default.
		''' </summary>
		''' <returns> always returns false </returns>
		' package private
		Friend Overridable Function alwaysOnTop() As Boolean
			Return False
		End Function

		Friend Overridable Property paintingChild As Component
			Set(ByVal paintingChild As Component)
				Me.paintingChild = paintingChild
			End Set
		End Property

		Friend Overridable Sub _paintImmediately(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim g As Graphics
			Dim c As Container
			Dim b As Rectangle

			Dim tmpX, tmpY, tmpWidth, tmpHeight As Integer
			Dim offsetX As Integer=0, offsetY As Integer=0

			Dim hasBuffer As Boolean = False

			Dim bufferedComponent As JComponent = Nothing
			Dim paintingComponent As JComponent = Me

			Dim ___repaintManager As RepaintManager = RepaintManager.currentManager(Me)
			' parent Container's up to Window or Applet. First container is
			' the direct parent. Note that in testing it was faster to
			' alloc a new Vector vs keeping a stack of them around, and gc
			' seemed to have a minimal effect on this.
			Dim path As IList(Of Component) = New List(Of Component)(7)
			Dim pIndex As Integer = -1
			Dim pCount As Integer = 0

				tmpHeight = 0
					tmpWidth = tmpHeight
						tmpY = tmpWidth
						tmpX = tmpY

			Dim paintImmediatelyClip As Rectangle = fetchRectangle()
			paintImmediatelyClip.x = x
			paintImmediatelyClip.y = y
			paintImmediatelyClip.width = w
			paintImmediatelyClip.height = h


			' System.out.println("1) ************* in _paintImmediately for " + this);

			Dim ontop As Boolean = alwaysOnTop() AndAlso opaque
			If ontop Then
				SwingUtilities.computeIntersection(0, 0, width, height, paintImmediatelyClip)
				If paintImmediatelyClip.width = 0 Then
					recycleRectangle(paintImmediatelyClip)
					Return
				End If
			End If
			Dim child As Component
			c = Me
			child = Nothing
			Do While c IsNot Nothing AndAlso Not(TypeOf c Is Window) AndAlso Not(TypeOf c Is java.applet.Applet)
					Dim jc As JComponent = If(TypeOf c Is JComponent, CType(c, JComponent), Nothing)
					path.Add(c)
					If (Not ontop) AndAlso jc IsNot Nothing AndAlso (Not jc.optimizedDrawingEnabled) Then
						Dim resetPC As Boolean

						' Children of c may overlap, three possible cases for the
						' painting region:
						' . Completely obscured by an opaque sibling, in which
						'   case there is no need to paint.
						' . Partially obscured by a sibling: need to start
						'   painting from c.
						' . Otherwise we aren't obscured and thus don't need to
						'   start painting from parent.
						If c IsNot Me Then
							If jc.paintingOrigin Then
								resetPC = True
							Else
								Dim children As Component() = c.components
								Dim i As Integer = 0
								Do While i<children.Length
									If children(i) Is child Then Exit Do
									i += 1
								Loop
								Select Case jc.getObscuredState(i, paintImmediatelyClip.x, paintImmediatelyClip.y, paintImmediatelyClip.width, paintImmediatelyClip.height)
								Case NOT_OBSCURED
									resetPC = False
								Case COMPLETELY_OBSCURED
									recycleRectangle(paintImmediatelyClip)
									Return
								Case Else
									resetPC = True
								End Select
							End If
						Else
							resetPC = False
						End If

						If resetPC Then
							' Get rid of any buffer since we draw from here and
							' we might draw something larger
							paintingComponent = jc
							pIndex = pCount
								offsetY = 0
								offsetX = offsetY
							hasBuffer = False
						End If
					End If
					pCount += 1

					' look to see if the parent (and therefor this component)
					' is double buffered
					If ___repaintManager.doubleBufferingEnabled AndAlso jc IsNot Nothing AndAlso jc.doubleBuffered Then
						hasBuffer = True
						bufferedComponent = jc
					End If

					' if we aren't on top, include the parent's clip
					If Not ontop Then
						Dim bx As Integer = c.x
						Dim by As Integer = c.y
						tmpWidth = c.width
						tmpHeight = c.height
						SwingUtilities.computeIntersection(tmpX,tmpY,tmpWidth,tmpHeight,paintImmediatelyClip)
						paintImmediatelyClip.x += bx
						paintImmediatelyClip.y += by
						offsetX += bx
						offsetY += by
					End If
				child = c
				c = c.parent
			Loop

			' If the clip width or height is negative, don't bother painting
			If c Is Nothing OrElse c.peer Is Nothing OrElse paintImmediatelyClip.width <= 0 OrElse paintImmediatelyClip.height <= 0 Then
				recycleRectangle(paintImmediatelyClip)
				Return
			End If

			paintingComponent.flaglag(IS_REPAINTING, True)

			paintImmediatelyClip.x -= offsetX
			paintImmediatelyClip.y -= offsetY

			' Notify the Components that are going to be painted of the
			' child component to paint to.
			If paintingComponent IsNot Me Then
				Dim comp As Component
				Dim i As Integer = pIndex
				Do While i > 0
					comp = path(i)
					If TypeOf comp Is JComponent Then CType(comp, JComponent).paintingChild = path(i-1)
					i -= 1
				Loop
			End If
			Try
				g = safelyGetGraphics(paintingComponent, c)
				If g IsNot Nothing Then
					Try
						If hasBuffer Then
							Dim rm As RepaintManager = RepaintManager.currentManager(bufferedComponent)
							rm.beginPaint()
							Try
								rm.paint(paintingComponent, bufferedComponent, g, paintImmediatelyClip.x, paintImmediatelyClip.y, paintImmediatelyClip.width, paintImmediatelyClip.height)
							Finally
								rm.endPaint()
							End Try
						Else
							g.cliplip(paintImmediatelyClip.x, paintImmediatelyClip.y, paintImmediatelyClip.width, paintImmediatelyClip.height)
							paintingComponent.paint(g)
						End If
					Finally
						g.Dispose()
					End Try
				End If
			Finally
				' Reset the painting child for the parent components.
				If paintingComponent IsNot Me Then
					Dim comp As Component
					Dim i As Integer = pIndex
					Do While i > 0
						comp = path(i)
						If TypeOf comp Is JComponent Then CType(comp, JComponent).paintingChild = Nothing
						i -= 1
					Loop
				End If
				paintingComponent.flaglag(IS_REPAINTING, False)
			End Try
			recycleRectangle(paintImmediatelyClip)
		End Sub

		''' <summary>
		''' Paints to the specified graphics.  This does not set the clip and it
		''' does not adjust the Graphics in anyway, callers must do that first.
		''' This method is package-private for RepaintManager.PaintManager and
		''' its subclasses to call, it is NOT intended for general use outside
		''' of that.
		''' </summary>
		Friend Overridable Sub paintToOffscreen(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal maxX As Integer, ByVal maxY As Integer)
			Try
				flaglag(ANCESTOR_USING_BUFFER, True)
				If (y + h) < maxY OrElse (x + w) < maxX Then flaglag(IS_PAINTING_TILE, True)
				If getFlag(IS_REPAINTING) Then
					' Called from paintImmediately (RepaintManager) to fill
					' repaint request
					paint(g)
				Else
					' Called from paint() (AWT) to repair damage
					If Not rectangleIsObscured(x, y, w, h) Then
						paintComponent(g)
						paintBorder(g)
					End If
					paintChildren(g)
				End If
			Finally
				flaglag(ANCESTOR_USING_BUFFER, False)
				flaglag(IS_PAINTING_TILE, False)
			End Try
		End Sub

		''' <summary>
		''' Returns whether or not the region of the specified component is
		''' obscured by a sibling.
		''' </summary>
		''' <returns> NOT_OBSCURED if non of the siblings above the Component obscure
		'''         it, COMPLETELY_OBSCURED if one of the siblings completely
		'''         obscures the Component or PARTIALLY_OBSCURED if the Component is
		'''         only partially obscured. </returns>
		Private Function getObscuredState(ByVal compIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Integer
			Dim retValue As Integer = NOT_OBSCURED
			Dim tmpRect As Rectangle = fetchRectangle()

			For i As Integer = compIndex - 1 To 0 Step -1
				Dim sibling As Component = getComponent(i)
				If Not sibling.visible Then Continue For
				Dim siblingRect As Rectangle
				Dim ___opaque As Boolean
				If TypeOf sibling Is JComponent Then
					___opaque = sibling.opaque
					If Not ___opaque Then
						If retValue = PARTIALLY_OBSCURED Then Continue For
					End If
				Else
					___opaque = True
				End If
				siblingRect = sibling.getBounds(tmpRect)
				If ___opaque AndAlso x >= siblingRect.x AndAlso (x + width) <= (siblingRect.x + siblingRect.width) AndAlso y >= siblingRect.y AndAlso (y + height) <= (siblingRect.y + siblingRect.height) Then
					recycleRectangle(tmpRect)
					Return COMPLETELY_OBSCURED
				ElseIf retValue = NOT_OBSCURED AndAlso Not((x + width <= siblingRect.x) OrElse (y + height <= siblingRect.y) OrElse (x >= siblingRect.x + siblingRect.width) OrElse (y >= siblingRect.y + siblingRect.height)) Then
					retValue = PARTIALLY_OBSCURED
				End If
			Next i
			recycleRectangle(tmpRect)
			Return retValue
		End Function

		''' <summary>
		''' Returns true, which implies that before checking if a child should
		''' be painted it is first check that the child is not obscured by another
		''' sibling. This is only checked if <code>isOptimizedDrawingEnabled</code>
		''' returns false.
		''' </summary>
		''' <returns> always returns true </returns>
		Friend Overridable Function checkIfChildObscuredBySibling() As Boolean
			Return True
		End Function


		Private Sub setFlag(ByVal aFlag As Integer, ByVal aValue As Boolean)
			If aValue Then
				flags = flags Or (1 << aFlag)
			Else
				flags = flags And Not(1 << aFlag)
			End If
		End Sub
		Private Function getFlag(ByVal aFlag As Integer) As Boolean
			Dim mask As Integer = (1 << aFlag)
			Return ((flags And mask) = mask)
		End Function
		' These functions must be static so that they can be called from
		' subclasses inside the package, but whose inheritance hierarhcy includes
		' classes outside of the package below JComponent (e.g., JTextArea).
		Shared Sub setWriteObjCounter(ByVal comp As JComponent, ByVal count As SByte)
			comp.flags = (comp.flags And Not(&HFF << WRITE_OBJ_COUNTER_FIRST)) Or (count << WRITE_OBJ_COUNTER_FIRST)
		End Sub
		Shared Function getWriteObjCounter(ByVal comp As JComponent) As SByte
			Return CByte((comp.flags >> WRITE_OBJ_COUNTER_FIRST) And &HFF)
		End Function

		''' <summary>
		''' Buffering * </summary>

		''' <summary>
		'''  Sets whether this component should use a buffer to paint.
		'''  If set to true, all the drawing from this component will be done
		'''  in an offscreen painting buffer. The offscreen painting buffer will
		'''  the be copied onto the screen.
		'''  If a <code>Component</code> is buffered and one of its ancestor
		'''  is also buffered, the ancestor buffer will be used.
		''' </summary>
		'''  <param name="aFlag"> if true, set this component to be double buffered </param>
		Public Overridable Property doubleBuffered As Boolean
			Set(ByVal aFlag As Boolean)
				flaglag(IS_DOUBLE_BUFFERED,aFlag)
			End Set
			Get
				Return getFlag(IS_DOUBLE_BUFFERED)
			End Get
		End Property


		''' <summary>
		''' Returns the <code>JRootPane</code> ancestor for this component.
		''' </summary>
		''' <returns> the <code>JRootPane</code> that contains this component,
		'''          or <code>null</code> if no <code>JRootPane</code> is found </returns>
		Public Overridable Property rootPane As JRootPane
			Get
				Return SwingUtilities.getRootPane(Me)
			End Get
		End Property


		''' <summary>
		''' Serialization * </summary>

		''' <summary>
		''' This is called from Component by way of reflection. Do NOT change
		''' the name unless you change the code in Component as well.
		''' </summary>
		Friend Overridable Sub compWriteObjectNotify()
			Dim count As SByte = JComponent.getWriteObjCounter(Me)
			JComponent.writeObjCounterter(Me, CByte(count + 1))
			If count <> 0 Then Return

			uninstallUIAndProperties()

	'         JTableHeader is in a separate package, which prevents it from
	'         * being able to override this package-private method the way the
	'         * other components can.  We don't want to make this method protected
	'         * because it would introduce public-api for a less-than-desirable
	'         * serialization scheme, so we compromise with this 'instanceof' hack
	'         * for now.
	'         
			If toolTipText IsNot Nothing OrElse TypeOf Me Is javax.swing.table.JTableHeader Then ToolTipManager.sharedInstance().unregisterComponent(JComponent.this)
		End Sub

		''' <summary>
		''' This object is the <code>ObjectInputStream</code> callback
		''' that's called after a complete graph of objects (including at least
		''' one <code>JComponent</code>) has been read.
		'''  It sets the UI property of each Swing component
		''' that was read to the current default with <code>updateUI</code>.
		''' <p>
		''' As each  component is read in we keep track of the current set of
		''' root components here, in the roots vector.  Note that there's only one
		''' <code>ReadObjectCallback</code> per <code>ObjectInputStream</code>,
		''' they're stored in the static <code>readObjectCallbacks</code>
		''' hashtable.
		''' </summary>
		''' <seealso cref= java.io.ObjectInputStream#registerValidation </seealso>
		''' <seealso cref= SwingUtilities#updateComponentTreeUI </seealso>
		Private Class ReadObjectCallback
			Implements java.io.ObjectInputValidation

			Private ReadOnly outerInstance As JComponent

			Private ReadOnly roots As New List(Of JComponent)(1)
			Private ReadOnly inputStream As java.io.ObjectInputStream

			Friend Sub New(ByVal outerInstance As JComponent, ByVal s As java.io.ObjectInputStream)
					Me.outerInstance = outerInstance
				inputStream = s
				s.registerValidation(Me, 0)
			End Sub

			''' <summary>
			''' This is the method that's called after the entire graph
			''' of objects has been read in.  It initializes
			''' the UI property of all of the copmonents with
			''' <code>SwingUtilities.updateComponentTreeUI</code>.
			''' </summary>
			Public Overridable Sub validateObject()
				Try
					For Each root As JComponent In roots
						SwingUtilities.updateComponentTreeUI(root)
					Next root
				Finally
					readObjectCallbacks.Remove(inputStream)
				End Try
			End Sub

			''' <summary>
			''' If <code>c</code> isn't a descendant of a component we've already
			''' seen, then add it to the roots <code>Vector</code>.
			''' </summary>
			''' <param name="c"> the <code>JComponent</code> to add </param>
			Private Sub registerComponent(ByVal c As JComponent)
	'             If the Component c is a descendant of one of the
	'             * existing roots (or it IS an existing root), we're done.
	'             
				For Each root As JComponent In roots
					Dim p As Component = c
					Do While p IsNot Nothing
						If p Is root Then Return
						p = p.parent
					Loop
				Next root

	'             Otherwise: if Component c is an ancestor of any of the
	'             * existing roots then remove them and add c (the "new root")
	'             * to the roots vector.
	'             
				For i As Integer = 0 To roots.Count - 1
					Dim root As JComponent = roots(i)
					Dim p As Component = root.parent
					Do While p IsNot Nothing
						If p Is c Then
							roots.RemoveAt(i)
							i -= 1
							Exit Do
						End If
						p = p.parent
					Loop
				Next i

				roots.Add(c)
			End Sub
		End Class


		''' <summary>
		''' We use the <code>ObjectInputStream</code> "registerValidation"
		''' callback to update the UI for the entire tree of components
		''' after they've all been read in.
		''' </summary>
		''' <param name="s">  the <code>ObjectInputStream</code> from which to read </param>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()

	'         If there's no ReadObjectCallback for this stream yet, that is, if
	'         * this is the first call to JComponent.readObject() for this
	'         * graph of objects, then create a callback and stash it
	'         * in the readObjectCallbacks table.  Note that the ReadObjectCallback
	'         * constructor takes care of calling s.registerValidation().
	'         
			Dim cb As ReadObjectCallback = readObjectCallbacks(s)
			If cb Is Nothing Then
				Try
						cb = New ReadObjectCallback(Me, Me, s)
						readObjectCallbacks(s) = cb
				Catch e As Exception
					Throw New java.io.IOException(e.ToString())
				End Try
			End If
			cb.registerComponent(Me)

			' Read back the client properties.
			Dim cpCount As Integer = s.readInt()
			If cpCount > 0 Then
				clientProperties = New ArrayTable
				For counter As Integer = 0 To cpCount - 1
					clientProperties.put(s.readObject(), s.readObject())
				Next counter
			End If
			If toolTipText IsNot Nothing Then ToolTipManager.sharedInstance().registerComponent(Me)
			writeObjCounterter(Me, CByte(0))
			revalidateRunnableScheduled = New java.util.concurrent.atomic.AtomicBoolean(False)
		End Sub


		''' <summary>
		''' Before writing a <code>JComponent</code> to an
		''' <code>ObjectOutputStream</code> we temporarily uninstall its UI.
		''' This is tricky to do because we want to uninstall
		''' the UI before any of the <code>JComponent</code>'s children
		''' (or its <code>LayoutManager</code> etc.) are written,
		''' and we don't want to restore the UI until the most derived
		''' <code>JComponent</code> subclass has been been stored.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> in which to write </param>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
			ArrayTable.writeArrayTable(s, clientProperties)
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JComponent</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JComponent</code> </returns>
		Protected Friend Overridable Function paramString() As String
			Dim preferredSizeString As String = (If(preferredSizeSet, preferredSize.ToString(), ""))
			Dim minimumSizeString As String = (If(minimumSizeSet, minimumSize.ToString(), ""))
			Dim maximumSizeString As String = (If(maximumSizeSet, maximumSize.ToString(), ""))
			Dim borderString As String = (If(border Is Nothing, "", (If(border Is Me, "this", border.ToString()))))

			Return MyBase.paramString() & ",alignmentX=" & alignmentX & ",alignmentY=" & alignmentY & ",border=" & borderString & ",flags=" & flags & ",maximumSize=" & maximumSizeString & ",minimumSize=" & minimumSizeString & ",preferredSize=" & preferredSizeString ' should beef this up a bit
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		<Obsolete> _
		Public Overrides Sub hide()
			Dim showing As Boolean = showing
			MyBase.hide()
			If showing Then
				Dim parent As Container = parent
				If parent IsNot Nothing Then
					Dim r As Rectangle = bounds
					parent.repaint(r.x, r.y, r.width, r.height)
				End If
				revalidate()
			End If
		End Sub

	End Class

End Namespace