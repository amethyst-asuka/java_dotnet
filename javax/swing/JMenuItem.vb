Imports System
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
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
Namespace javax.swing




	''' <summary>
	''' An implementation of an item in a menu. A menu item is essentially a button
	''' sitting in a list. When the user selects the "button", the action
	''' associated with the menu item is performed. A <code>JMenuItem</code>
	''' contained in a <code>JPopupMenu</code> performs exactly that function.
	''' <p>
	''' Menu items can be configured, and to some degree controlled, by
	''' <code><a href="Action.html">Action</a></code>s.  Using an
	''' <code>Action</code> with a menu item has many benefits beyond directly
	''' configuring a menu item.  Refer to <a href="Action.html#buttonActions">
	''' Swing Components Supporting <code>Action</code></a> for more
	''' details, and you can find more information in <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/misc/action.html">How
	''' to Use Actions</a>, a section in <em>The Java Tutorial</em>.
	''' <p>
	''' For further documentation and for examples, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/menu.html">How to Use Menus</a>
	''' in <em>The Java Tutorial.</em>
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
	''' description: An item which can be selected in a menu.
	''' 
	''' @author Georges Saab
	''' @author David Karlton </summary>
	''' <seealso cref= JPopupMenu </seealso>
	''' <seealso cref= JMenu </seealso>
	''' <seealso cref= JCheckBoxMenuItem </seealso>
	''' <seealso cref= JRadioButtonMenuItem </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JMenuItem
		Inherits AbstractButton
		Implements Accessible, MenuElement

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "MenuItemUI"

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		Private isMouseDragged As Boolean = False

		''' <summary>
		''' Creates a <code>JMenuItem</code> with no set text or icon.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, CType(Nothing, Icon))
		End Sub

		''' <summary>
		''' Creates a <code>JMenuItem</code> with the specified icon.
		''' </summary>
		''' <param name="icon"> the icon of the <code>JMenuItem</code> </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon)
		End Sub

		''' <summary>
		''' Creates a <code>JMenuItem</code> with the specified text.
		''' </summary>
		''' <param name="text"> the text of the <code>JMenuItem</code> </param>
		Public Sub New(ByVal text As String)
			Me.New(text, CType(Nothing, Icon))
		End Sub

		''' <summary>
		''' Creates a menu item whose properties are taken from the
		''' specified <code>Action</code>.
		''' </summary>
		''' <param name="a"> the action of the <code>JMenuItem</code>
		''' @since 1.3 </param>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates a <code>JMenuItem</code> with the specified text and icon.
		''' </summary>
		''' <param name="text"> the text of the <code>JMenuItem</code> </param>
		''' <param name="icon"> the icon of the <code>JMenuItem</code> </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			model = New DefaultButtonModel
			init(text, icon)
			initFocusability()
		End Sub

		''' <summary>
		''' Creates a <code>JMenuItem</code> with the specified text and
		''' keyboard mnemonic.
		''' </summary>
		''' <param name="text"> the text of the <code>JMenuItem</code> </param>
		''' <param name="mnemonic"> the keyboard mnemonic for the <code>JMenuItem</code> </param>
		Public Sub New(ByVal text As String, ByVal mnemonic As Integer)
			model = New DefaultButtonModel
			init(text, Nothing)
			mnemonic = mnemonic
			initFocusability()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property model As ButtonModel
			Set(ByVal newModel As ButtonModel)
				MyBase.model = newModel
				If TypeOf newModel Is DefaultButtonModel Then CType(newModel, DefaultButtonModel).menuItem = True
			End Set
		End Property

		''' <summary>
		''' Inititalizes the focusability of the the <code>JMenuItem</code>.
		''' <code>JMenuItem</code>'s are focusable, but subclasses may
		''' want to be, this provides them the opportunity to override this
		''' and invoke something else, or nothing at all. Refer to
		''' <seealso cref="javax.swing.JMenu#initFocusability"/> for the motivation of
		''' this.
		''' </summary>
		Friend Overridable Sub initFocusability()
			focusable = False
		End Sub

		''' <summary>
		''' Initializes the menu item with the specified text and icon.
		''' </summary>
		''' <param name="text"> the text of the <code>JMenuItem</code> </param>
		''' <param name="icon"> the icon of the <code>JMenuItem</code> </param>
		Protected Friend Overrides Sub init(ByVal text As String, ByVal icon As Icon)
			If text IsNot Nothing Then text = text

			If icon IsNot Nothing Then icon = icon

			' Listen for Focus events
			addFocusListener(New MenuItemFocusListener)
			uIPropertyrty("borderPainted", Boolean.FALSE)
			focusPainted = False
			horizontalTextPosition = JButton.TRAILING
			horizontalAlignment = JButton.LEADING
			updateUI()
		End Sub

		<Serializable> _
		Private Class MenuItemFocusListener
			Implements FocusListener

			Public Overridable Sub focusGained(ByVal [event] As FocusEvent)
			End Sub
			Public Overridable Sub focusLost(ByVal [event] As FocusEvent)
				' When focus is lost, repaint if
				' the focus information is painted
				Dim mi As JMenuItem = CType([event].source, JMenuItem)
				If mi.focusPainted Then mi.repaint()
			End Sub
		End Class


		''' <summary>
		''' Sets the look and feel object that renders this component.
		''' </summary>
		''' <param name="ui">  the <code>JMenuItemUI</code> L&amp;F object </param>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The UI object that implements the Component's LookAndFeel. </seealso>
		Public Overridable Property uI As MenuItemUI
			Set(ByVal ui As MenuItemUI)
				MyBase.uI = ui
			End Set
		End Property

		''' <summary>
		''' Resets the UI property with a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), MenuItemUI)
		End Sub


		''' <summary>
		''' Returns the suffix used to construct the name of the L&amp;F class used to
		''' render this component.
		''' </summary>
		''' <returns> the string "MenuItemUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Identifies the menu item as "armed". If the mouse button is
		''' released while it is over this item, the menu's action event
		''' will fire. If the mouse button is released elsewhere, the
		''' event will not fire and the menu item will be disarmed.
		''' </summary>
		''' <param name="b"> true to arm the menu item so it can be selected
		''' @beaninfo
		'''    description: Mouse release will fire an action event
		'''         hidden: true </param>
		Public Overridable Property armed As Boolean
			Set(ByVal b As Boolean)
				Dim ___model As ButtonModel = model
    
				Dim oldValue As Boolean = ___model.armed
				If ___model.armed <> b Then ___model.armed = b
			End Set
			Get
				Dim ___model As ButtonModel = model
				Return ___model.armed
			End Get
		End Property


		''' <summary>
		''' Enables or disables the menu item.
		''' </summary>
		''' <param name="b">  true to enable the item
		''' @beaninfo
		'''    description: Does the component react to user interaction
		'''          bound: true
		'''      preferred: true </param>
		Public Overrides Property enabled As Boolean
			Set(ByVal b As Boolean)
				' Make sure we aren't armed!
				If (Not b) AndAlso (Not UIManager.getBoolean("MenuItem.disabledAreNavigable")) Then armed = False
				MyBase.enabled = b
			End Set
		End Property


		''' <summary>
		''' Returns true since <code>Menu</code>s, by definition,
		''' should always be on top of all other windows.  If the menu is
		''' in an internal frame false is returned due to the rollover effect
		''' for windows laf where the menu is not always on top.
		''' </summary>
		' package private
		Friend Overrides Function alwaysOnTop() As Boolean
			' Fix for bug #4482165
			If SwingUtilities.getAncestorOfClass(GetType(JInternalFrame), Me) IsNot Nothing Then Return False
			Return True
		End Function


	'     The keystroke which acts as the menu item's accelerator
	'     
		Private accelerator As KeyStroke

		''' <summary>
		''' Sets the key combination which invokes the menu item's
		''' action listeners without navigating the menu hierarchy. It is the
		''' UI's responsibility to install the correct action.  Note that
		''' when the keyboard accelerator is typed, it will work whether or
		''' not the menu is currently displayed.
		''' </summary>
		''' <param name="keyStroke"> the <code>KeyStroke</code> which will
		'''          serve as an accelerator
		''' @beaninfo
		'''     description: The keystroke combination which will invoke the
		'''                  JMenuItem's actionlisteners without navigating the
		'''                  menu hierarchy
		'''           bound: true
		'''       preferred: true </param>
		Public Overridable Property accelerator As KeyStroke
			Set(ByVal ___keyStroke As KeyStroke)
				Dim oldAccelerator As KeyStroke = accelerator
				Me.accelerator = ___keyStroke
				repaint()
				revalidate()
				firePropertyChange("accelerator", oldAccelerator, accelerator)
			End Set
			Get
				Return Me.accelerator
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overrides Sub configurePropertiesFromAction(ByVal a As Action)
			MyBase.configurePropertiesFromAction(a)
			configureAcceleratorFromAction(a)
		End Sub

		Friend Overrides Property iconFromAction As Action
			Set(ByVal a As Action)
				Dim ___icon As Icon = Nothing
				If a IsNot Nothing Then ___icon = CType(a.getValue(Action.SMALL_ICON), Icon)
				icon = ___icon
			End Set
		End Property

		Friend Overrides Sub largeIconChanged(ByVal a As Action)
		End Sub

		Friend Overrides Sub smallIconChanged(ByVal a As Action)
			iconFromAction = a
		End Sub

		Friend Overridable Sub configureAcceleratorFromAction(ByVal a As Action)
			Dim ks As KeyStroke = If(a Is Nothing, Nothing, CType(a.getValue(Action.ACCELERATOR_KEY), KeyStroke))
			accelerator = ks
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Protected Friend Overrides Sub actionPropertyChanged(ByVal action As Action, ByVal propertyName As String)
			If propertyName = Action.ACCELERATOR_KEY Then
				configureAcceleratorFromAction(action)
			Else
				MyBase.actionPropertyChanged(action, propertyName)
			End If
		End Sub

		''' <summary>
		''' Processes a mouse event forwarded from the
		''' <code>MenuSelectionManager</code> and changes the menu
		''' selection, if necessary, by using the
		''' <code>MenuSelectionManager</code>'s API.
		''' <p>
		''' Note: you do not have to forward the event to sub-components.
		''' This is done automatically by the <code>MenuSelectionManager</code>.
		''' </summary>
		''' <param name="e">   a <code>MouseEvent</code> </param>
		''' <param name="path">  the <code>MenuElement</code> path array </param>
		''' <param name="manager">   the <code>MenuSelectionManager</code> </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void processMouseEvent(MouseEvent e,MenuElement path() ,MenuSelectionManager manager)
			processMenuDragMouseEvent(New MenuDragMouseEvent(e.component, e.iD, e.when, e.modifiers, e.x, e.y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, path, manager))


		''' <summary>
		''' Processes a key event forwarded from the
		''' <code>MenuSelectionManager</code> and changes the menu selection,
		''' if necessary, by using <code>MenuSelectionManager</code>'s API.
		''' <p>
		''' Note: you do not have to forward the event to sub-components.
		''' This is done automatically by the <code>MenuSelectionManager</code>.
		''' </summary>
		''' <param name="e">  a <code>KeyEvent</code> </param>
		''' <param name="path"> the <code>MenuElement</code> path array </param>
		''' <param name="manager">   the <code>MenuSelectionManager</code> </param>
		public void processKeyEvent(KeyEvent e,MenuElement path() ,MenuSelectionManager manager)
			If DEBUG Then Console.WriteLine("in JMenuItem.processKeyEvent/3 for " & text & "  " & KeyStroke.getKeyStrokeForEvent(e))
			Dim mke As New MenuKeyEvent(e.component, e.iD, e.when, e.modifiers, e.keyCode, e.keyChar, path, manager)
			processMenuKeyEvent(mke)

			If mke.consumed Then e.consume()



		''' <summary>
		''' Handles mouse drag in a menu.
		''' </summary>
		''' <param name="e">  a <code>MenuDragMouseEvent</code> object </param>
		public void processMenuDragMouseEvent(MenuDragMouseEvent e)
			Select Case e.iD
			Case MouseEvent.MOUSE_ENTERED
				isMouseDragged = False
				fireMenuDragMouseEntered(e)
			Case MouseEvent.MOUSE_EXITED
				isMouseDragged = False
				fireMenuDragMouseExited(e)
			Case MouseEvent.MOUSE_DRAGGED
				isMouseDragged = True
				fireMenuDragMouseDragged(e)
			Case MouseEvent.MOUSE_RELEASED
				If isMouseDragged Then fireMenuDragMouseReleased(e)
			Case Else
			End Select

		''' <summary>
		''' Handles a keystroke in a menu.
		''' </summary>
		''' <param name="e">  a <code>MenuKeyEvent</code> object </param>
		public void processMenuKeyEvent(MenuKeyEvent e)
			If DEBUG Then Console.WriteLine("in JMenuItem.processMenuKeyEvent for " & text & "  " & KeyStroke.getKeyStrokeForEvent(e))
			Select Case e.iD
			Case KeyEvent.KEY_PRESSED
				fireMenuKeyPressed(e)
			Case KeyEvent.KEY_RELEASED
				fireMenuKeyReleased(e)
			Case KeyEvent.KEY_TYPED
				fireMenuKeyTyped(e)
			Case Else
			End Select

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuMouseDragEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuDragMouseEntered(MenuDragMouseEvent event)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuDragMouseListener) Then CType(___listeners(i+1), MenuDragMouseListener).menuDragMouseEntered(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuDragMouseEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuDragMouseExited(MenuDragMouseEvent event)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuDragMouseListener) Then CType(___listeners(i+1), MenuDragMouseListener).menuDragMouseExited(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuDragMouseEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuDragMouseDragged(MenuDragMouseEvent event)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuDragMouseListener) Then CType(___listeners(i+1), MenuDragMouseListener).menuDragMouseDragged(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuDragMouseEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuDragMouseReleased(MenuDragMouseEvent event)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuDragMouseListener) Then CType(___listeners(i+1), MenuDragMouseListener).menuDragMouseReleased(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuKeyPressed(MenuKeyEvent event)
			If DEBUG Then Console.WriteLine("in JMenuItem.fireMenuKeyPressed for " & text & "  " & KeyStroke.getKeyStrokeForEvent(event))
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyPressed(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuKeyReleased(MenuKeyEvent event)
			If DEBUG Then Console.WriteLine("in JMenuItem.fireMenuKeyReleased for " & text & "  " & KeyStroke.getKeyStrokeForEvent(event))
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyReleased(event)
			Next i

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="event"> a <code>MenuKeyEvent</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		protected void fireMenuKeyTyped(MenuKeyEvent event)
			If DEBUG Then Console.WriteLine("in JMenuItem.fireMenuKeyTyped for " & text & "  " & KeyStroke.getKeyStrokeForEvent(event))
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(MenuKeyListener) Then CType(___listeners(i+1), MenuKeyListener).menuKeyTyped(event)
			Next i

		''' <summary>
		''' Called by the <code>MenuSelectionManager</code> when the
		''' <code>MenuElement</code> is selected or unselected.
		''' </summary>
		''' <param name="isIncluded">  true if this menu item is on the part of the menu
		'''                    path that changed, false if this menu is part of the
		'''                    a menu path that changed, but this particular part of
		'''                    that path is still the same </param>
		''' <seealso cref= MenuSelectionManager#setSelectedPath(MenuElement[]) </seealso>
		public void menuSelectionChanged(Boolean isIncluded)
			armed = isIncluded

		''' <summary>
		''' This method returns an array containing the sub-menu
		''' components for this menu component.
		''' </summary>
		''' <returns> an array of <code>MenuElement</code>s </returns>
		public MenuElement() subElements
			Return New MenuElement(){}

		''' <summary>
		''' Returns the <code>java.awt.Component</code> used to paint
		''' this object. The returned component will be used to convert
		''' events and detect if an event is inside a menu component.
		''' </summary>
		''' <returns> the <code>Component</code> that paints this menu item </returns>
		public Component component
			Return Me

		''' <summary>
		''' Adds a <code>MenuDragMouseListener</code> to the menu item.
		''' </summary>
		''' <param name="l"> the <code>MenuDragMouseListener</code> to be added </param>
		public void addMenuDragMouseListener(MenuDragMouseListener l)
			listenerList.add(GetType(MenuDragMouseListener), l)

		''' <summary>
		''' Removes a <code>MenuDragMouseListener</code> from the menu item.
		''' </summary>
		''' <param name="l"> the <code>MenuDragMouseListener</code> to be removed </param>
		public void removeMenuDragMouseListener(MenuDragMouseListener l)
			listenerList.remove(GetType(MenuDragMouseListener), l)

		''' <summary>
		''' Returns an array of all the <code>MenuDragMouseListener</code>s added
		''' to this JMenuItem with addMenuDragMouseListener().
		''' </summary>
		''' <returns> all of the <code>MenuDragMouseListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		public MenuDragMouseListener() menuDragMouseListeners
			Return listenerList.getListeners(GetType(MenuDragMouseListener))

		''' <summary>
		''' Adds a <code>MenuKeyListener</code> to the menu item.
		''' </summary>
		''' <param name="l"> the <code>MenuKeyListener</code> to be added </param>
		public void addMenuKeyListener(MenuKeyListener l)
			listenerList.add(GetType(MenuKeyListener), l)

		''' <summary>
		''' Removes a <code>MenuKeyListener</code> from the menu item.
		''' </summary>
		''' <param name="l"> the <code>MenuKeyListener</code> to be removed </param>
		public void removeMenuKeyListener(MenuKeyListener l)
			listenerList.remove(GetType(MenuKeyListener), l)

		''' <summary>
		''' Returns an array of all the <code>MenuKeyListener</code>s added
		''' to this JMenuItem with addMenuKeyListener().
		''' </summary>
		''' <returns> all of the <code>MenuKeyListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		public MenuKeyListener() menuKeyListeners
			Return listenerList.getListeners(GetType(MenuKeyListener))

		''' <summary>
		''' See JComponent.readObject() for information about serialization
		''' in Swing.
		''' </summary>
		private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
			s.defaultReadObject()
			If uIClassID.Equals(uiClassID) Then updateUI()

		private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If


		''' <summary>
		''' Returns a string representation of this <code>JMenuItem</code>.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JMenuItem</code> </returns>
		protected String paramString()
			Return MyBase.paramString()

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Returns the <code>AccessibleContext</code> associated with this
		''' <code>JMenuItem</code>. For <code>JMenuItem</code>s,
		''' the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJMenuItem</code>.
		''' A new AccessibleJMenuItme instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJMenuItem</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JMenuItem</code> </returns>
		public AccessibleContext accessibleContext
			If accessibleContext Is Nothing Then accessibleContext = New AccessibleJMenuItem(Me)
			Return accessibleContext


		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JMenuItem</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu item user-interface
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		protected class AccessibleJMenuItem extends AccessibleAbstractButton implements ChangeListener

			private Boolean isArmed = False
			private Boolean hasFocus = False
			private Boolean isPressed = False
			private Boolean isSelected = False

			AccessibleJMenuItem()
				MyBase()
				outerInstance.addChangeListener(Me)

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			public AccessibleRole accessibleRole
				Return AccessibleRole.MENU_ITEM

			private void fireAccessibilityFocusedEvent(JMenuItem toCheck)
				Dim path As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
				If path.Length > 0 Then
					Dim menuItem As Object = path(path.Length - 1)
					If toCheck Is menuItem Then firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
				End If

			''' <summary>
			''' Supports the change listener interface and fires property changes.
			''' </summary>
			public void stateChanged(ChangeEvent e)
				firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))
				If outerInstance.model.armed Then
					If Not isArmed Then
						isArmed = True
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.ARMED)
						' Fix for 4848220 moved here to avoid major memory leak
						' Here we will fire the event in case of JMenuItem
						' See bug 4910323 for details [zav]
						fireAccessibilityFocusedEvent(JMenuItem.this)
					End If
				Else
					If isArmed Then
						isArmed = False
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.ARMED, Nothing)
					End If
				End If
				If outerInstance.focusOwner Then
					If Not hasFocus Then
						hasFocus = True
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
					End If
				Else
					If hasFocus Then
						hasFocus = False
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.FOCUSED, Nothing)
					End If
				End If
				If outerInstance.model.pressed Then
					If Not isPressed Then
						isPressed = True
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.PRESSED)
					End If
				Else
					If isPressed Then
						isPressed = False
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.PRESSED, Nothing)
					End If
				End If
				If outerInstance.model.selected Then
					If Not isSelected Then
						isSelected = True
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.CHECKED)

						' Fix for 4848220 moved here to avoid major memory leak
						' Here we will fire the event in case of JMenu
						' See bug 4910323 for details [zav]
						fireAccessibilityFocusedEvent(JMenuItem.this)
					End If
				Else
					If isSelected Then
						isSelected = False
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.CHECKED, Nothing)
					End If
				End If
 ' inner class AccessibleJMenuItem


	End Class

End Namespace