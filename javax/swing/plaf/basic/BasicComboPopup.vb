Imports System
Imports javax.swing
Imports javax.swing.event

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

Namespace javax.swing.plaf.basic



	''' <summary>
	''' This is a basic implementation of the <code>ComboPopup</code> interface.
	''' 
	''' This class represents the ui for the popup portion of the combo box.
	''' <p>
	''' All event handling is handled by listener classes created with the
	''' <code>createxxxListener()</code> methods and internal classes.
	''' You can change the behavior of this class by overriding the
	''' <code>createxxxListener()</code> methods and supplying your own
	''' event listeners or subclassing from the ones supplied in this class.
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
	''' @author Tom Santos
	''' @author Mark Davidson
	''' </summary>
	Public Class BasicComboPopup
		Inherits JPopupMenu
		Implements ComboPopup

		' An empty ListMode, this is used when the UI changes to allow
		' the JList to be gc'ed.
		<Serializable> _
		Private Class EmptyListModelClass
			Implements ListModel(Of Object)

			Public Overridable Property size As Integer Implements ListModel(Of Object).getSize
				Get
					Return 0
				End Get
			End Property
			Public Overridable Function getElementAt(ByVal index As Integer) As Object
				Return Nothing
			End Function
			Public Overridable Sub addListDataListener(ByVal l As ListDataListener)
			End Sub
			Public Overridable Sub removeListDataListener(ByVal l As ListDataListener)
			End Sub
		End Class

		Friend Shared ReadOnly EmptyListModel As ListModel = New EmptyListModelClass

		Private Shared LIST_BORDER As javax.swing.border.Border = New javax.swing.border.LineBorder(Color.BLACK, 1)

		Protected Friend comboBox As JComboBox
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #getList </seealso>
		''' <seealso cref= #createList </seealso>
		Protected Friend list As JList
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead
		''' </summary>
		''' <seealso cref= #createScroller </seealso>
		Protected Friend scroller As JScrollPane

		''' <summary>
		''' As of Java 2 platform v1.4 this previously undocumented field is no
		''' longer used.
		''' </summary>
		Protected Friend valueIsAdjusting As Boolean = False

		' Listeners that are required by the ComboPopup interface

		''' <summary>
		''' Implementation of all the listener classes.
		''' </summary>
		Private handler As Handler

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor or create methods instead.
		''' </summary>
		''' <seealso cref= #getMouseMotionListener </seealso>
		''' <seealso cref= #createMouseMotionListener </seealso>
		Protected Friend mouseMotionListener As MouseMotionListener
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor or create methods instead.
		''' </summary>
		''' <seealso cref= #getMouseListener </seealso>
		''' <seealso cref= #createMouseListener </seealso>
		Protected Friend mouseListener As MouseListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor or create methods instead.
		''' </summary>
		''' <seealso cref= #getKeyListener </seealso>
		''' <seealso cref= #createKeyListener </seealso>
		Protected Friend keyListener As KeyListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead.
		''' </summary>
		''' <seealso cref= #createListSelectionListener </seealso>
		Protected Friend listSelectionListener As ListSelectionListener

		' Listeners that are attached to the list
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead.
		''' </summary>
		''' <seealso cref= #createListMouseListener </seealso>
		Protected Friend listMouseListener As MouseListener
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead
		''' </summary>
		''' <seealso cref= #createListMouseMotionListener </seealso>
		Protected Friend listMouseMotionListener As MouseMotionListener

		' Added to the combo box for bound properties
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead
		''' </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener

		' Added to the combo box model
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead
		''' </summary>
		''' <seealso cref= #createListDataListener </seealso>
		Protected Friend listDataListener As ListDataListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the create method instead
		''' </summary>
		''' <seealso cref= #createItemListener </seealso>
		Protected Friend itemListener As ItemListener

		Private scrollerMouseWheelListener As MouseWheelListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override.
		''' </summary>
		Protected Friend autoscrollTimer As Timer
		Protected Friend hasEntered As Boolean = False
		Protected Friend isAutoScrolling As Boolean = False
		Protected Friend scrollDirection As Integer = SCROLL_UP

		Protected Friend Const SCROLL_UP As Integer = 0
		Protected Friend Const SCROLL_DOWN As Integer = 1


		'========================================
		' begin ComboPopup method implementations
		'

		''' <summary>
		''' Implementation of ComboPopup.show().
		''' </summary>
		Public Overridable Sub show() Implements ComboPopup.show
			comboBox.firePopupMenuWillBecomeVisible()
			listSelection = comboBox.selectedIndex
			Dim ___location As Point = popupLocation
			show(comboBox, ___location.x, ___location.y)
		End Sub


		''' <summary>
		''' Implementation of ComboPopup.hide().
		''' </summary>
		Public Overrides Sub hide() Implements ComboPopup.hide
			Dim manager As MenuSelectionManager = MenuSelectionManager.defaultManager()
			Dim selection As MenuElement() = manager.selectedPath
			For i As Integer = 0 To selection.Length - 1
				If selection(i) Is Me Then
					manager.clearSelectedPath()
					Exit For
				End If
			Next i
			If selection.Length > 0 Then comboBox.repaint()
		End Sub

		''' <summary>
		''' Implementation of ComboPopup.getList().
		''' </summary>
		Public Overridable Property list As JList
			Get
				Return list
			End Get
		End Property

		''' <summary>
		''' Implementation of ComboPopup.getMouseListener().
		''' </summary>
		''' <returns> a <code>MouseListener</code> or null </returns>
		''' <seealso cref= ComboPopup#getMouseListener </seealso>
		Public Overridable Property mouseListener As MouseListener
			Get
				If mouseListener Is Nothing Then mouseListener = createMouseListener()
				Return mouseListener
			End Get
		End Property

		''' <summary>
		''' Implementation of ComboPopup.getMouseMotionListener().
		''' </summary>
		''' <returns> a <code>MouseMotionListener</code> or null </returns>
		''' <seealso cref= ComboPopup#getMouseMotionListener </seealso>
		Public Overridable Property mouseMotionListener As MouseMotionListener
			Get
				If mouseMotionListener Is Nothing Then mouseMotionListener = createMouseMotionListener()
				Return mouseMotionListener
			End Get
		End Property

		''' <summary>
		''' Implementation of ComboPopup.getKeyListener().
		''' </summary>
		''' <returns> a <code>KeyListener</code> or null </returns>
		''' <seealso cref= ComboPopup#getKeyListener </seealso>
		Public Overridable Property keyListener As KeyListener
			Get
				If keyListener Is Nothing Then keyListener = createKeyListener()
				Return keyListener
			End Get
		End Property

		''' <summary>
		''' Called when the UI is uninstalling.  Since this popup isn't in the component
		''' tree, it won't get it's uninstallUI() called.  It removes the listeners that
		''' were added in addComboBoxListeners().
		''' </summary>
		Public Overridable Sub uninstallingUI() Implements ComboPopup.uninstallingUI
			If propertyChangeListener IsNot Nothing Then comboBox.removePropertyChangeListener(propertyChangeListener)
			If itemListener IsNot Nothing Then comboBox.removeItemListener(itemListener)
			uninstallComboBoxModelListeners(comboBox.model)
			uninstallKeyboardActions()
			uninstallListListeners()
			uninstallScrollerListeners()
			' We do this, otherwise the listener the ui installs on
			' the model (the combobox model in this case) will keep a
			' reference to the list, causing the list (and us) to never get gced.
			list.model = EmptyListModel
		End Sub

		'
		' end ComboPopup method implementations
		'======================================

		''' <summary>
		''' Removes the listeners from the combo box model
		''' </summary>
		''' <param name="model"> The combo box model to install listeners </param>
		''' <seealso cref= #installComboBoxModelListeners </seealso>
		Protected Friend Overridable Sub uninstallComboBoxModelListeners(ByVal model As ComboBoxModel)
			If model IsNot Nothing AndAlso listDataListener IsNot Nothing Then model.removeListDataListener(listDataListener)
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			' XXX - shouldn't call this method
	'        comboBox.unregisterKeyboardAction( KeyStroke.getKeyStroke( KeyEvent.VK_ENTER, 0 ) );
		End Sub



		'===================================================================
		' begin Initialization routines
		'
		Public Sub New(ByVal combo As JComboBox)
			MyBase.New()
			name = "ComboPopup.popup"
			comboBox = combo

			lightWeightPopupEnabled = comboBox.lightWeightPopupEnabled

			' UI construction of the popup.
			list = createList()
			list.name = "ComboBox.list"
			configureList()
			scroller = createScroller()
			scroller.name = "ComboBox.scrollPane"
			configureScroller()
			configurePopup()

			installComboBoxListeners()
			installKeyboardActions()
		End Sub

		' Overriden PopupMenuListener notification methods to inform combo box
		' PopupMenuListeners.

		Protected Friend Overrides Sub firePopupMenuWillBecomeVisible()
			MyBase.firePopupMenuWillBecomeVisible()
			' comboBox.firePopupMenuWillBecomeVisible() is called from BasicComboPopup.show() method
			' to let the user change the popup menu from the PopupMenuListener.popupMenuWillBecomeVisible()
		End Sub

		Protected Friend Overrides Sub firePopupMenuWillBecomeInvisible()
			MyBase.firePopupMenuWillBecomeInvisible()
			comboBox.firePopupMenuWillBecomeInvisible()
		End Sub

		Protected Friend Overrides Sub firePopupMenuCanceled()
			MyBase.firePopupMenuCanceled()
			comboBox.firePopupMenuCanceled()
		End Sub

		''' <summary>
		''' Creates a listener
		''' that will watch for mouse-press and release events on the combo box.
		''' 
		''' <strong>Warning:</strong>
		''' When overriding this method, make sure to maintain the existing
		''' behavior.
		''' </summary>
		''' <returns> a <code>MouseListener</code> which will be added to
		''' the combo box or null </returns>
		Protected Friend Overridable Function createMouseListener() As MouseListener
			Return handler
		End Function

		''' <summary>
		''' Creates the mouse motion listener which will be added to the combo
		''' box.
		''' 
		''' <strong>Warning:</strong>
		''' When overriding this method, make sure to maintain the existing
		''' behavior.
		''' </summary>
		''' <returns> a <code>MouseMotionListener</code> which will be added to
		'''         the combo box or null </returns>
		Protected Friend Overridable Function createMouseMotionListener() As MouseMotionListener
			Return handler
		End Function

		''' <summary>
		''' Creates the key listener that will be added to the combo box. If
		''' this method returns null then it will not be added to the combo box.
		''' </summary>
		''' <returns> a <code>KeyListener</code> or null </returns>
		Protected Friend Overridable Function createKeyListener() As KeyListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates a list selection listener that watches for selection changes in
		''' the popup's list.  If this method returns null then it will not
		''' be added to the popup list.
		''' </summary>
		''' <returns> an instance of a <code>ListSelectionListener</code> or null </returns>
		Protected Friend Overridable Function createListSelectionListener() As ListSelectionListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates a list data listener which will be added to the
		''' <code>ComboBoxModel</code>. If this method returns null then
		''' it will not be added to the combo box model.
		''' </summary>
		''' <returns> an instance of a <code>ListDataListener</code> or null </returns>
		Protected Friend Overridable Function createListDataListener() As ListDataListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates a mouse listener that watches for mouse events in
		''' the popup's list. If this method returns null then it will
		''' not be added to the combo box.
		''' </summary>
		''' <returns> an instance of a <code>MouseListener</code> or null </returns>
		Protected Friend Overridable Function createListMouseListener() As MouseListener
			Return handler
		End Function

		''' <summary>
		''' Creates a mouse motion listener that watches for mouse motion
		''' events in the popup's list. If this method returns null then it will
		''' not be added to the combo box.
		''' </summary>
		''' <returns> an instance of a <code>MouseMotionListener</code> or null </returns>
		Protected Friend Overridable Function createListMouseMotionListener() As MouseMotionListener
			Return handler
		End Function

		''' <summary>
		''' Creates a <code>PropertyChangeListener</code> which will be added to
		''' the combo box. If this method returns null then it will not
		''' be added to the combo box.
		''' </summary>
		''' <returns> an instance of a <code>PropertyChangeListener</code> or null </returns>
		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		''' <summary>
		''' Creates an <code>ItemListener</code> which will be added to the
		''' combo box. If this method returns null then it will not
		''' be added to the combo box.
		''' <p>
		''' Subclasses may override this method to return instances of their own
		''' ItemEvent handlers.
		''' </summary>
		''' <returns> an instance of an <code>ItemListener</code> or null </returns>
		Protected Friend Overridable Function createItemListener() As ItemListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		''' <summary>
		''' Creates the JList used in the popup to display
		''' the items in the combo box model. This method is called when the UI class
		''' is created.
		''' </summary>
		''' <returns> a <code>JList</code> used to display the combo box items </returns>
		Protected Friend Overridable Function createList() As JList
			Return New JListAnonymousInnerClassHelper(Of E)
		End Function

		Private Class JListAnonymousInnerClassHelper(Of E)
			Inherits JList(Of E)

			Public Overridable Sub processMouseEvent(ByVal e As MouseEvent)
				If BasicGraphicsUtils.isMenuShortcutKeyDown(e) Then
					' Fix for 4234053. Filter out the Control Key from the list.
					' ie., don't allow CTRL key deselection.
					Dim toolkit As Toolkit = Toolkit.defaultToolkit
					e = New MouseEvent(CType(e.source, Component), e.iD, e.when, e.modifiers Xor toolkit.menuShortcutKeyMask, e.x, e.y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, MouseEvent.NOBUTTON)
				End If
				MyBase.processMouseEvent(e)
			End Sub
		End Class

		''' <summary>
		''' Configures the list which is used to hold the combo box items in the
		''' popup. This method is called when the UI class
		''' is created.
		''' </summary>
		''' <seealso cref= #createList </seealso>
		Protected Friend Overridable Sub configureList()
			list.font = comboBox.font
			list.foreground = comboBox.foreground
			list.background = comboBox.background
			list.selectionForeground = UIManager.getColor("ComboBox.selectionForeground")
			list.selectionBackground = UIManager.getColor("ComboBox.selectionBackground")
			list.border = Nothing
			list.cellRenderer = comboBox.renderer
			list.focusable = False
			list.selectionMode = ListSelectionModel.SINGLE_SELECTION
			listSelection = comboBox.selectedIndex
			installListListeners()
		End Sub

		''' <summary>
		''' Adds the listeners to the list control.
		''' </summary>
		Protected Friend Overridable Sub installListListeners()
			listMouseListener = createListMouseListener()
			If listMouseListener IsNot Nothing Then list.addMouseListener(listMouseListener)
			listMouseMotionListener = createListMouseMotionListener()
			If listMouseMotionListener IsNot Nothing Then list.addMouseMotionListener(listMouseMotionListener)
			listSelectionListener = createListSelectionListener()
			If listSelectionListener IsNot Nothing Then list.addListSelectionListener(listSelectionListener)
		End Sub

		Friend Overridable Sub uninstallListListeners()
			If listMouseListener IsNot Nothing Then
				list.removeMouseListener(listMouseListener)
				listMouseListener = Nothing
			End If
			If listMouseMotionListener IsNot Nothing Then
				list.removeMouseMotionListener(listMouseMotionListener)
				listMouseMotionListener = Nothing
			End If
			If listSelectionListener IsNot Nothing Then
				list.removeListSelectionListener(listSelectionListener)
				listSelectionListener = Nothing
			End If
			handler = Nothing
		End Sub

		''' <summary>
		''' Creates the scroll pane which houses the scrollable list.
		''' </summary>
		Protected Friend Overridable Function createScroller() As JScrollPane
			Dim sp As New JScrollPane(list, ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER)
			sp.horizontalScrollBar = Nothing
			Return sp
		End Function

		''' <summary>
		''' Configures the scrollable portion which holds the list within
		''' the combo box popup. This method is called when the UI class
		''' is created.
		''' </summary>
		Protected Friend Overridable Sub configureScroller()
			scroller.focusable = False
			scroller.verticalScrollBar.focusable = False
			scroller.border = Nothing
			installScrollerListeners()
		End Sub

		''' <summary>
		''' Configures the popup portion of the combo box. This method is called
		''' when the UI class is created.
		''' </summary>
		Protected Friend Overridable Sub configurePopup()
			layout = New BoxLayout(Me, BoxLayout.Y_AXIS)
			borderPainted = True
			border = LIST_BORDER
			opaque = False
			add(scroller)
			doubleBuffered = True
			focusable = False
		End Sub

		Private Sub installScrollerListeners()
			scrollerMouseWheelListener = handler
			If scrollerMouseWheelListener IsNot Nothing Then scroller.addMouseWheelListener(scrollerMouseWheelListener)
		End Sub

		Private Sub uninstallScrollerListeners()
			If scrollerMouseWheelListener IsNot Nothing Then
				scroller.removeMouseWheelListener(scrollerMouseWheelListener)
				scrollerMouseWheelListener = Nothing
			End If
		End Sub

		''' <summary>
		''' This method adds the necessary listeners to the JComboBox.
		''' </summary>
		Protected Friend Overridable Sub installComboBoxListeners()
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then comboBox.addPropertyChangeListener(propertyChangeListener)
			itemListener = createItemListener()
			If itemListener IsNot Nothing Then comboBox.addItemListener(itemListener)
			installComboBoxModelListeners(comboBox.model)
		End Sub

		''' <summary>
		''' Installs the listeners on the combo box model. Any listeners installed
		''' on the combo box model should be removed in
		''' <code>uninstallComboBoxModelListeners</code>.
		''' </summary>
		''' <param name="model"> The combo box model to install listeners </param>
		''' <seealso cref= #uninstallComboBoxModelListeners </seealso>
		Protected Friend Overridable Sub installComboBoxModelListeners(ByVal model As ComboBoxModel)
			listDataListener = createListDataListener()
			If model IsNot Nothing AndAlso listDataListener IsNot Nothing Then model.addListDataListener(listDataListener)
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()

	'         XXX - shouldn't call this method. take it out for testing.
	'        ActionListener action = new ActionListener() {
	'            public void actionPerformed(ActionEvent e){
	'            }
	'        };
	'
	'        comboBox.registerKeyboardAction( action,
	'                                         KeyStroke.getKeyStroke( KeyEvent.VK_ENTER, 0 ),
	'                                         JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT ); 

		End Sub

		'
		' end Initialization routines
		'=================================================================


		'===================================================================
		' begin Event Listenters
		'

		''' <summary>
		''' A listener to be registered upon the combo box
		''' (<em>not</em> its popup menu)
		''' to handle mouse events
		''' that affect the state of the popup menu.
		''' The main purpose of this listener is to make the popup menu
		''' appear and disappear.
		''' This listener also helps
		''' with click-and-drag scenarios by setting the selection if the mouse was
		''' released over the list during a drag.
		''' 
		''' <p>
		''' <strong>Warning:</strong>
		''' We recommend that you <em>not</em>
		''' create subclasses of this class.
		''' If you absolutely must create a subclass,
		''' be sure to invoke the superclass
		''' version of each method.
		''' </summary>
		''' <seealso cref= BasicComboPopup#createMouseListener </seealso>
		Protected Friend Class InvocationMouseHandler
			Inherits MouseAdapter

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Responds to mouse-pressed events on the combo box.
			''' </summary>
			''' <param name="e"> the mouse-press event to be handled </param>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			''' <summary>
			''' Responds to the user terminating
			''' a click or drag that began on the combo box.
			''' </summary>
			''' <param name="e"> the mouse-release event to be handled </param>
			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for dragging and updates the current selection in the
		''' list if it is dragging over the list.
		''' </summary>
		Protected Friend Class InvocationMouseMotionHandler
			Inherits MouseMotionAdapter

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub
		End Class

		''' <summary>
		''' As of Java 2 platform v 1.4, this class is now obsolete and is only included for
		''' backwards API compatibility. Do not instantiate or subclass.
		''' <p>
		''' All the functionality of this class has been included in
		''' BasicComboBoxUI ActionMap/InputMap methods.
		''' </summary>
		Public Class InvocationKeyHandler
			Inherits KeyAdapter

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub
		End Class

		''' <summary>
		''' As of Java 2 platform v 1.4, this class is now obsolete, doesn't do anything, and
		''' is only included for backwards API compatibility. Do not call or
		''' override.
		''' </summary>
		Protected Friend Class ListSelectionHandler
			Implements ListSelectionListener

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
			End Sub
		End Class

		''' <summary>
		''' As of 1.4, this class is now obsolete, doesn't do anything, and
		''' is only included for backwards API compatibility. Do not call or
		''' override.
		''' <p>
		''' The functionality has been migrated into <code>ItemHandler</code>.
		''' </summary>
		''' <seealso cref= #createItemListener </seealso>
		Public Class ListDataHandler
			Implements ListDataListener

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
			End Sub

			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
			End Sub

			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
			End Sub
		End Class

		''' <summary>
		''' This listener hides the popup when the mouse is released in the list.
		''' </summary>
		Protected Friend Class ListMouseHandler
			Inherits MouseAdapter

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
			End Sub
			Public Overridable Sub mouseReleased(ByVal anEvent As MouseEvent)
				outerInstance.handler.mouseReleased(anEvent)
			End Sub
		End Class

		''' <summary>
		''' This listener changes the selected item as you move the mouse over the list.
		''' The selection change is not committed to the model, this is for user feedback only.
		''' </summary>
		Protected Friend Class ListMouseMotionHandler
			Inherits MouseMotionAdapter

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mouseMoved(ByVal anEvent As MouseEvent)
				outerInstance.handler.mouseMoved(anEvent)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for changes to the selection in the
		''' combo box.
		''' </summary>
		Protected Friend Class ItemHandler
			Implements ItemListener

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent)
				outerInstance.handler.itemStateChanged(e)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for bound properties that have changed in the
		''' combo box.
		''' <p>
		''' Subclasses which wish to listen to combo box property changes should
		''' call the superclass methods to ensure that the combo popup correctly
		''' handles property changes.
		''' </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class


		Private Class AutoScrollActionHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicComboPopup

			Private direction As Integer

			Friend Sub New(ByVal outerInstance As BasicComboPopup, ByVal direction As Integer)
					Me.outerInstance = outerInstance
				Me.direction = direction
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If direction = SCROLL_UP Then
					outerInstance.autoScrollUp()
				Else
					outerInstance.autoScrollDown()
				End If
			End Sub
		End Class


		<Serializable> _
		Private Class Handler
			Implements ItemListener, MouseListener, MouseMotionListener, MouseWheelListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicComboPopup

			Public Sub New(ByVal outerInstance As BasicComboPopup)
				Me.outerInstance = outerInstance
			End Sub

			'
			' MouseListener
			' NOTE: this is added to both the JList and JComboBox
			'
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If e.source Is outerInstance.list Then Return
				If (Not SwingUtilities.isLeftMouseButton(e)) OrElse (Not outerInstance.comboBox.enabled) Then Return

				If outerInstance.comboBox.editable Then
					Dim comp As Component = outerInstance.comboBox.editor.editorComponent
					If (Not(TypeOf comp Is JComponent)) OrElse CType(comp, JComponent).requestFocusEnabled Then comp.requestFocus()
				ElseIf outerInstance.comboBox.requestFocusEnabled Then
					outerInstance.comboBox.requestFocus()
				End If
				outerInstance.togglePopup()
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If e.source Is outerInstance.list Then
					If outerInstance.list.model.size > 0 Then
						' JList mouse listener
						If outerInstance.comboBox.selectedIndex = outerInstance.list.selectedIndex Then outerInstance.comboBox.editor.item = outerInstance.list.selectedValue
						outerInstance.comboBox.selectedIndex = outerInstance.list.selectedIndex
					End If
					outerInstance.comboBox.popupVisible = False
					' workaround for cancelling an edited item (bug 4530953)
					If outerInstance.comboBox.editable AndAlso outerInstance.comboBox.editor IsNot Nothing Then outerInstance.comboBox.configureEditor(outerInstance.comboBox.editor, outerInstance.comboBox.selectedItem)
					Return
				End If
				' JComboBox mouse listener
				Dim source As Component = CType(e.source, Component)
				Dim size As Dimension = source.size
				Dim bounds As New Rectangle(0, 0, size.width - 1, size.height - 1)
				If Not bounds.contains(e.point) Then
					Dim newEvent As MouseEvent = outerInstance.convertMouseEvent(e)
					Dim location As Point = newEvent.point
					Dim r As New Rectangle
					outerInstance.list.computeVisibleRect(r)
					If r.contains(location) Then
						If outerInstance.comboBox.selectedIndex = outerInstance.list.selectedIndex Then outerInstance.comboBox.editor.item = outerInstance.list.selectedValue
						outerInstance.comboBox.selectedIndex = outerInstance.list.selectedIndex
					End If
					outerInstance.comboBox.popupVisible = False
				End If
				outerInstance.hasEntered = False
				outerInstance.stopAutoScrolling()
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			'
			' MouseMotionListener:
			' NOTE: this is added to both the List and ComboBox
			'
			Public Overridable Sub mouseMoved(ByVal anEvent As MouseEvent)
				If anEvent.source Is outerInstance.list Then
					Dim location As Point = anEvent.point
					Dim r As New Rectangle
					outerInstance.list.computeVisibleRect(r)
					If r.contains(location) Then outerInstance.updateListBoxSelectionForEvent(anEvent, False)
				End If
			End Sub

			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If e.source Is outerInstance.list Then Return
				If outerInstance.visible Then
					Dim newEvent As MouseEvent = outerInstance.convertMouseEvent(e)
					Dim r As New Rectangle
					outerInstance.list.computeVisibleRect(r)

					If newEvent.point.y >= r.y AndAlso newEvent.point.y <= r.y + r.height - 1 Then
						outerInstance.hasEntered = True
						If outerInstance.isAutoScrolling Then outerInstance.stopAutoScrolling()
						Dim location As Point = newEvent.point
						If r.contains(location) Then outerInstance.updateListBoxSelectionForEvent(newEvent, False)
					Else
						If outerInstance.hasEntered Then
							Dim directionToScroll As Integer = If(newEvent.point.y < r.y, SCROLL_UP, SCROLL_DOWN)
							If outerInstance.isAutoScrolling AndAlso outerInstance.scrollDirection <> directionToScroll Then
								outerInstance.stopAutoScrolling()
								outerInstance.startAutoScrolling(directionToScroll)
							ElseIf Not outerInstance.isAutoScrolling Then
								outerInstance.startAutoScrolling(directionToScroll)
							End If
						Else
							If e.point.y < 0 Then
								outerInstance.hasEntered = True
								outerInstance.startAutoScrolling(SCROLL_UP)
							End If
						End If
					End If
				End If
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim comboBox As JComboBox = CType(e.source, JComboBox)
				Dim propertyName As String = e.propertyName

				If propertyName = "model" Then
					Dim oldModel As ComboBoxModel = CType(e.oldValue, ComboBoxModel)
					Dim newModel As ComboBoxModel = CType(e.newValue, ComboBoxModel)
					outerInstance.uninstallComboBoxModelListeners(oldModel)
					outerInstance.installComboBoxModelListeners(newModel)

					outerInstance.list.model = newModel

					If outerInstance.visible Then outerInstance.hide()
				ElseIf propertyName = "renderer" Then
					outerInstance.list.cellRenderer = comboBox.renderer
					If outerInstance.visible Then outerInstance.hide()
				ElseIf propertyName = "componentOrientation" Then
					' Pass along the new component orientation
					' to the list and the scroller

					Dim o As ComponentOrientation =CType(e.newValue, ComponentOrientation)

					Dim list As JList = outerInstance.list
					If list IsNot Nothing AndAlso list.componentOrientation IsNot o Then list.componentOrientation = o

					If outerInstance.scroller IsNot Nothing AndAlso outerInstance.scroller.componentOrientation IsNot o Then outerInstance.scroller.componentOrientation = o

					If o IsNot componentOrientation Then componentOrientation = o
				ElseIf propertyName = "lightWeightPopupEnabled" Then
					outerInstance.lightWeightPopupEnabled = comboBox.lightWeightPopupEnabled
				End If
			End Sub

			'
			' ItemListener
			'
			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent)
				If e.stateChange = ItemEvent.SELECTED Then
					Dim comboBox As JComboBox = CType(e.source, JComboBox)
					outerInstance.listSelection = comboBox.selectedIndex
				End If
			End Sub

			'
			' MouseWheelListener
			'
			Public Overridable Sub mouseWheelMoved(ByVal e As MouseWheelEvent)
				e.consume()
			End Sub
		End Class

		'
		' end Event Listeners
		'=================================================================


		''' <summary>
		''' Overridden to unconditionally return false.
		''' </summary>
		Public Overridable Property focusTraversable As Boolean
			Get
				Return False
			End Get
		End Property

		'===================================================================
		' begin Autoscroll methods
		'

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		Protected Friend Overridable Sub startAutoScrolling(ByVal direction As Integer)
			' XXX - should be a private method within InvocationMouseMotionHandler
			' if possible.
			If isAutoScrolling Then autoscrollTimer.stop()

			isAutoScrolling = True

			If direction = SCROLL_UP Then
				scrollDirection = SCROLL_UP
				Dim convertedPoint As Point = SwingUtilities.convertPoint(scroller, New Point(1, 1), list)
				Dim top As Integer = list.locationToIndex(convertedPoint)
				list.selectedIndex = top

				autoscrollTimer = New Timer(100, New AutoScrollActionHandler(Me, SCROLL_UP))
			ElseIf direction = SCROLL_DOWN Then
				scrollDirection = SCROLL_DOWN
				Dim ___size As Dimension = scroller.size
				Dim convertedPoint As Point = SwingUtilities.convertPoint(scroller, New Point(1, (___size.height - 1) - 2), list)
				Dim bottom As Integer = list.locationToIndex(convertedPoint)
				list.selectedIndex = bottom

				autoscrollTimer = New Timer(100, New AutoScrollActionHandler(Me, SCROLL_DOWN))
			End If
			autoscrollTimer.start()
		End Sub

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		Protected Friend Overridable Sub stopAutoScrolling()
			isAutoScrolling = False

			If autoscrollTimer IsNot Nothing Then
				autoscrollTimer.stop()
				autoscrollTimer = Nothing
			End If
		End Sub

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		Protected Friend Overridable Sub autoScrollUp()
			Dim index As Integer = list.selectedIndex
			If index > 0 Then
				list.selectedIndex = index - 1
				list.ensureIndexIsVisible(index - 1)
			End If
		End Sub

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		Protected Friend Overridable Sub autoScrollDown()
			Dim index As Integer = list.selectedIndex
			Dim lastItem As Integer = list.model.size - 1
			If index < lastItem Then
				list.selectedIndex = index + 1
				list.ensureIndexIsVisible(index + 1)
			End If
		End Sub

		'
		' end Autoscroll methods
		'=================================================================


		'===================================================================
		' begin Utility methods
		'

		''' <summary>
		''' Gets the AccessibleContext associated with this BasicComboPopup.
		''' The AccessibleContext will have its parent set to the ComboBox.
		''' </summary>
		''' <returns> an AccessibleContext for the BasicComboPopup
		''' @since 1.5 </returns>
		Public Property Overrides accessibleContext As javax.accessibility.AccessibleContext
			Get
				Dim context As javax.accessibility.AccessibleContext = MyBase.accessibleContext
				context.accessibleParent = comboBox
				Return context
			End Get
		End Property


		''' <summary>
		''' This is is a utility method that helps event handlers figure out where to
		''' send the focus when the popup is brought up.  The standard implementation
		''' delegates the focus to the editor (if the combo box is editable) or to
		''' the JComboBox if it is not editable.
		''' </summary>
		Protected Friend Overridable Sub delegateFocus(ByVal e As MouseEvent)
			If comboBox.editable Then
				Dim comp As Component = comboBox.editor.editorComponent
				If (Not(TypeOf comp Is JComponent)) OrElse CType(comp, JComponent).requestFocusEnabled Then comp.requestFocus()
			ElseIf comboBox.requestFocusEnabled Then
				comboBox.requestFocus()
			End If
		End Sub

		''' <summary>
		''' Makes the popup visible if it is hidden and makes it hidden if it is
		''' visible.
		''' </summary>
		Protected Friend Overridable Sub togglePopup()
			If visible Then
				hide()
			Else
				show()
			End If
		End Sub

		''' <summary>
		''' Sets the list selection index to the selectedIndex. This
		''' method is used to synchronize the list selection with the
		''' combo box selection.
		''' </summary>
		''' <param name="selectedIndex"> the index to set the list </param>
		Private Property listSelection As Integer
			Set(ByVal selectedIndex As Integer)
				If selectedIndex = -1 Then
					list.clearSelection()
				Else
					list.selectedIndex = selectedIndex
					list.ensureIndexIsVisible(selectedIndex)
				End If
			End Set
		End Property

		Protected Friend Overridable Function convertMouseEvent(ByVal e As MouseEvent) As MouseEvent
			Dim convertedPoint As Point = SwingUtilities.convertPoint(CType(e.source, Component), e.point, list)
			Dim newEvent As New MouseEvent(CType(e.source, Component), e.iD, e.when, e.modifiers, convertedPoint.x, convertedPoint.y, e.xOnScreen, e.yOnScreen, e.clickCount, e.popupTrigger, MouseEvent.NOBUTTON)
			Return newEvent
		End Function


		''' <summary>
		''' Retrieves the height of the popup based on the current
		''' ListCellRenderer and the maximum row count.
		''' </summary>
		Protected Friend Overridable Function getPopupHeightForRowCount(ByVal maxRowCount As Integer) As Integer
			' Set the cached value of the minimum row count
			Dim minRowCount As Integer = Math.Min(maxRowCount, comboBox.itemCount)
			Dim ___height As Integer = 0
			Dim renderer As ListCellRenderer = list.cellRenderer
			Dim value As Object = Nothing

			For i As Integer = 0 To minRowCount - 1
				value = list.model.getElementAt(i)
				Dim c As Component = renderer.getListCellRendererComponent(list, value, i, False, False)
				___height += c.preferredSize.height
			Next i

			If ___height = 0 Then ___height = comboBox.height

			Dim ___border As javax.swing.border.Border = scroller.viewportBorder
			If ___border IsNot Nothing Then
				Dim ___insets As Insets = ___border.getBorderInsets(Nothing)
				___height += ___insets.top + ___insets.bottom
			End If

			___border = scroller.border
			If ___border IsNot Nothing Then
				Dim ___insets As Insets = ___border.getBorderInsets(Nothing)
				___height += ___insets.top + ___insets.bottom
			End If

			Return ___height
		End Function

		''' <summary>
		''' Calculate the placement and size of the popup portion of the combo box based
		''' on the combo box location and the enclosing screen bounds. If
		''' no transformations are required, then the returned rectangle will
		''' have the same values as the parameters.
		''' </summary>
		''' <param name="px"> starting x location </param>
		''' <param name="py"> starting y location </param>
		''' <param name="pw"> starting width </param>
		''' <param name="ph"> starting height </param>
		''' <returns> a rectangle which represents the placement and size of the popup </returns>
		Protected Friend Overridable Function computePopupBounds(ByVal px As Integer, ByVal py As Integer, ByVal pw As Integer, ByVal ph As Integer) As Rectangle
			Dim toolkit As Toolkit = Toolkit.defaultToolkit
			Dim screenBounds As Rectangle

			' Calculate the desktop dimensions relative to the combo box.
			Dim gc As GraphicsConfiguration = comboBox.graphicsConfiguration
			Dim p As New Point
			SwingUtilities.convertPointFromScreen(p, comboBox)
			If gc IsNot Nothing Then
				Dim screenInsets As Insets = toolkit.getScreenInsets(gc)
				screenBounds = gc.bounds
				screenBounds.width -= (screenInsets.left + screenInsets.right)
				screenBounds.height -= (screenInsets.top + screenInsets.bottom)
				screenBounds.x += (p.x + screenInsets.left)
				screenBounds.y += (p.y + screenInsets.top)
			Else
				screenBounds = New Rectangle(p, toolkit.screenSize)
			End If

			Dim rect As New Rectangle(px,py,pw,ph)
			If py+ph > screenBounds.y+screenBounds.height AndAlso ph < screenBounds.height Then rect.y = -rect.height
			Return rect
		End Function

		''' <summary>
		''' Calculates the upper left location of the Popup.
		''' </summary>
		Private Property popupLocation As Point
			Get
				Dim ___popupSize As Dimension = comboBox.size
				Dim ___insets As Insets = insets
    
				' reduce the width of the scrollpane by the insets so that the popup
				' is the same width as the combo box.
				___popupSize.sizeize(___popupSize.width - (___insets.right + ___insets.left), getPopupHeightForRowCount(comboBox.maximumRowCount))
				Dim popupBounds As Rectangle = computePopupBounds(0, comboBox.bounds.height, ___popupSize.width, ___popupSize.height)
				Dim scrollSize As Dimension = popupBounds.size
				Dim ___popupLocation As Point = popupBounds.location
    
				scroller.maximumSize = scrollSize
				scroller.preferredSize = scrollSize
				scroller.minimumSize = scrollSize
    
				list.revalidate()
    
				Return ___popupLocation
			End Get
		End Property

		''' <summary>
		''' A utility method used by the event listeners.  Given a mouse event, it changes
		''' the list selection to the list item below the mouse.
		''' </summary>
		Protected Friend Overridable Sub updateListBoxSelectionForEvent(ByVal anEvent As MouseEvent, ByVal shouldScroll As Boolean)
			' XXX - only seems to be called from this class. shouldScroll flag is
			' never true
			Dim ___location As Point = anEvent.point
			If list Is Nothing Then Return
			Dim index As Integer = list.locationToIndex(___location)
			If index = -1 Then
				If ___location.y < 0 Then
					index = 0
				Else
					index = comboBox.model.size - 1
				End If
			End If
			If list.selectedIndex <> index Then
				list.selectedIndex = index
				If shouldScroll Then list.ensureIndexIsVisible(index)
			End If
		End Sub

		'
		' end Utility methods
		'=================================================================
	End Class

End Namespace