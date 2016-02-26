Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports javax.swing
Imports javax.accessibility
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
	''' Basic UI implementation for JComboBox.
	''' <p>
	''' The combo box is a compound component which means that it is an aggregate of
	''' many simpler components. This class creates and manages the listeners
	''' on the combo box and the combo box model. These listeners update the user
	''' interface in response to changes in the properties and state of the combo box.
	''' <p>
	''' All event handling is handled by listener classes created with the
	''' <code>createxxxListener()</code> methods and internal classes.
	''' You can change the behavior of this class by overriding the
	''' <code>createxxxListener()</code> methods and supplying your own
	''' event listeners or subclassing from the ones supplied in this class.
	''' <p>
	''' For adding specific actions,
	''' overide <code>installKeyboardActions</code> to add actions in response to
	''' KeyStroke bindings. See the article <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/keybinding.html">How to Use Key Bindings</a>
	''' 
	''' @author Arnaud Weber
	''' @author Tom Santos
	''' @author Mark Davidson
	''' </summary>
	Public Class BasicComboBoxUI
		Inherits ComboBoxUI

		Protected Friend comboBox As JComboBox
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override.
		''' </summary>
		Protected Friend hasFocus As Boolean = False

		' Control the selection behavior of the JComboBox when it is used
		' in the JTable DefaultCellEditor.
		Private ___isTableCellEditor As Boolean = False
		Private Const IS_TABLE_CELL_EDITOR As String = "JComboBox.isTableCellEditor"

		' This list is for drawing the current item in the combo box.
		Protected Friend listBox As JList

		' Used to render the currently selected item in the combo box.
		' It doesn't have anything to do with the popup's rendering.
		Protected Friend currentValuePane As New CellRendererPane

		' The implementation of ComboPopup that is used to show the popup.
		Protected Friend popup As ComboPopup

		' The Component that the ComboBoxEditor uses for editing
		Protected Friend editor As Component

		' The arrow button that invokes the popup.
		Protected Friend arrowButton As JButton

		' Listeners that are attached to the JComboBox
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Override the listener construction method instead.
		''' </summary>
		''' <seealso cref= #createKeyListener </seealso>
		Protected Friend keyListener As KeyListener
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Override the listener construction method instead.
		''' </summary>
		''' <seealso cref= #createFocusListener </seealso>
		Protected Friend focusListener As FocusListener
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Override the listener construction method instead.
		''' </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Override the listener construction method instead.
		''' </summary>
		''' <seealso cref= #createItemListener </seealso>
		Protected Friend itemListener As ItemListener

		' Listeners that the ComboPopup produces.
		Protected Friend popupMouseListener As MouseListener
		Protected Friend popupMouseMotionListener As MouseMotionListener
		Protected Friend popupKeyListener As KeyListener

		Private mouseWheelListener As MouseWheelListener

		' This is used for knowing when to cache the minimum preferred size.
		' If the data in the list changes, the cached value get marked for recalc.
		' Added to the current JComboBox model
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Override the listener construction method instead.
		''' </summary>
		''' <seealso cref= #createListDataListener </seealso>
		Protected Friend listDataListener As ListDataListener

		''' <summary>
		''' Implements all the Listeners needed by this class, all existing
		''' listeners redirect to it.
		''' </summary>
		Private handler As Handler

		''' <summary>
		''' The time factor to treate the series of typed alphanumeric key
		''' as prefix for first letter navigation.
		''' </summary>
		Private timeFactor As Long = 1000L

		''' <summary>
		''' This is tricky, this variables is needed for DefaultKeySelectionManager
		''' to take into account time factor.
		''' </summary>
		Private lastTime As Long = 0L
		Private time As Long = 0L

		''' <summary>
		''' The default key selection manager
		''' </summary>
		Friend keySelectionManager As JComboBox.KeySelectionManager

		' Flag for recalculating the minimum preferred size.
		Protected Friend isMinimumSizeDirty As Boolean = True

		' Cached minimum preferred size.
		Protected Friend cachedMinimumSize As New Dimension(0, 0)

		' Flag for calculating the display size
		Private isDisplaySizeDirty As Boolean = True

		' Cached the size that the display needs to render the largest item
		Private cachedDisplaySize As New Dimension(0, 0)

		' Key used for lookup of the DefaultListCellRenderer in the AppContext.
		Private Shared ReadOnly COMBO_UI_LIST_CELL_RENDERER_KEY As Object = New StringBuilder("DefaultListCellRendererKey")

		Friend Shared ReadOnly HIDE_POPUP_KEY As New StringBuilder("HidePopupKey")

		''' <summary>
		''' Whether or not all cells have the same baseline.
		''' </summary>
		Private sameBaseline As Boolean

		''' <summary>
		''' Indicates whether or not the combo box button should be square.
		''' If square, then the width and height are equal, and are both set to
		''' the height of the combo minus appropriate insets.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend squareButton As Boolean = True

		''' <summary>
		''' If specified, these insets act as padding around the cell renderer when
		''' laying out and painting the "selected" item in the combo box. These
		''' insets add to those specified by the cell renderer.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend padding As Insets

		' Used for calculating the default size.
		Private Property Shared defaultListCellRenderer As ListCellRenderer
			Get
				Dim renderer As ListCellRenderer = CType(sun.awt.AppContext.appContext.get(COMBO_UI_LIST_CELL_RENDERER_KEY), ListCellRenderer)
    
				If renderer Is Nothing Then
					renderer = New DefaultListCellRenderer
					sun.awt.AppContext.appContext.put(COMBO_UI_LIST_CELL_RENDERER_KEY, New DefaultListCellRenderer)
				End If
				Return renderer
			End Get
		End Property

		''' <summary>
		''' Populates ComboBox's actions.
		''' </summary>
		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.HIDE))
			map.put(New Actions(Actions.PAGE_DOWN))
			map.put(New Actions(Actions.PAGE_UP))
			map.put(New Actions(Actions.HOME))
			map.put(New Actions(Actions.END))
			map.put(New Actions(Actions.DOWN))
			map.put(New Actions(Actions.DOWN_2))
			map.put(New Actions(Actions.TOGGLE))
			map.put(New Actions(Actions.TOGGLE_2))
			map.put(New Actions(Actions.UP))
			map.put(New Actions(Actions.UP_2))
			map.put(New Actions(Actions.ENTER))
		End Sub

		'========================
		' begin UI Initialization
		'

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicComboBoxUI
		End Function

		Public Overrides Sub installUI(ByVal c As JComponent)
			isMinimumSizeDirty = True

			comboBox = CType(c, JComboBox)
			installDefaults()
			popup = createPopup()
			listBox = popup.list

			' Is this combo box a cell editor?
			Dim inTable As Boolean? = CBool(c.getClientProperty(IS_TABLE_CELL_EDITOR))
			If inTable IsNot Nothing Then ___isTableCellEditor = If(inTable.Equals(Boolean.TRUE), True, False)

			If comboBox.renderer Is Nothing OrElse TypeOf comboBox.renderer Is UIResource Then comboBox.renderer = createRenderer()

			If comboBox.editor Is Nothing OrElse TypeOf comboBox.editor Is UIResource Then comboBox.editor = createEditor()

			installListeners()
			installComponents()

			comboBox.layout = createLayoutManager()

			comboBox.requestFocusEnabled = True

			installKeyboardActions()

			comboBox.putClientProperty("doNotCancelPopup", HIDE_POPUP_KEY)

			If keySelectionManager Is Nothing OrElse TypeOf keySelectionManager Is UIResource Then keySelectionManager = New DefaultKeySelectionManager(Me)
			comboBox.keySelectionManager = keySelectionManager
		End Sub

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			popupVisibleble(comboBox, False)
			popup.uninstallingUI()

			uninstallKeyboardActions()

			comboBox.layout = Nothing

			uninstallComponents()
			uninstallListeners()
			uninstallDefaults()

			If comboBox.renderer Is Nothing OrElse TypeOf comboBox.renderer Is UIResource Then comboBox.renderer = Nothing

			Dim comboBoxEditor As ComboBoxEditor = comboBox.editor
			If TypeOf comboBoxEditor Is UIResource Then
				If comboBoxEditor.editorComponent.hasFocus() Then comboBox.requestFocusInWindow()
				comboBox.editor = Nothing
			End If

			If TypeOf keySelectionManager Is UIResource Then comboBox.keySelectionManager = Nothing

			handler = Nothing
			keyListener = Nothing
			focusListener = Nothing
			listDataListener = Nothing
			propertyChangeListener = Nothing
			popup = Nothing
			listBox = Nothing
			comboBox = Nothing
		End Sub

		''' <summary>
		''' Installs the default colors, default font, default renderer, and default
		''' editor into the JComboBox.
		''' </summary>
		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(comboBox, "ComboBox.background", "ComboBox.foreground", "ComboBox.font")
			LookAndFeel.installBorder(comboBox, "ComboBox.border")
			LookAndFeel.installProperty(comboBox, "opaque", Boolean.TRUE)

			Dim l As Long? = CLng(Fix(UIManager.get("ComboBox.timeFactor")))
			timeFactor = If(l Is Nothing, 1000L, l)

			'NOTE: this needs to default to true if not specified
			Dim b As Boolean? = CBool(UIManager.get("ComboBox.squareButton"))
			squareButton = If(b Is Nothing, True, b)

			padding = UIManager.getInsets("ComboBox.padding")
		End Sub

		''' <summary>
		''' Creates and installs listeners for the combo box and its model.
		''' This method is called when the UI is installed.
		''' </summary>
		Protected Friend Overridable Sub installListeners()
			itemListener = createItemListener()
			If itemListener IsNot Nothing Then comboBox.addItemListener(itemListener)
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then comboBox.addPropertyChangeListener(propertyChangeListener)
			keyListener = createKeyListener()
			If keyListener IsNot Nothing Then comboBox.addKeyListener(keyListener)
			focusListener = createFocusListener()
			If focusListener IsNot Nothing Then comboBox.addFocusListener(focusListener)
			popupMouseListener = popup.mouseListener
			If popupMouseListener IsNot Nothing Then comboBox.addMouseListener(popupMouseListener)
			popupMouseMotionListener = popup.mouseMotionListener
			If popupMouseMotionListener IsNot Nothing Then comboBox.addMouseMotionListener(popupMouseMotionListener)
			popupKeyListener = popup.keyListener
			If popupKeyListener IsNot Nothing Then comboBox.addKeyListener(popupKeyListener)

			If comboBox.model IsNot Nothing Then
				listDataListener = createListDataListener()
				If listDataListener IsNot Nothing Then comboBox.model.addListDataListener(listDataListener)
			End If

			mouseWheelListener = createMouseWheelListener()
			If mouseWheelListener IsNot Nothing Then comboBox.addMouseWheelListener(mouseWheelListener)
		End Sub

		''' <summary>
		''' Uninstalls the default colors, default font, default renderer,
		''' and default editor from the combo box.
		''' </summary>
		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.installColorsAndFont(comboBox, "ComboBox.background", "ComboBox.foreground", "ComboBox.font")
			LookAndFeel.uninstallBorder(comboBox)
		End Sub

		''' <summary>
		''' Removes the installed listeners from the combo box and its model.
		''' The number and types of listeners removed and in this method should be
		''' the same that was added in <code>installListeners</code>
		''' </summary>
		Protected Friend Overridable Sub uninstallListeners()
			If keyListener IsNot Nothing Then comboBox.removeKeyListener(keyListener)
			If itemListener IsNot Nothing Then comboBox.removeItemListener(itemListener)
			If propertyChangeListener IsNot Nothing Then comboBox.removePropertyChangeListener(propertyChangeListener)
			If focusListener IsNot Nothing Then comboBox.removeFocusListener(focusListener)
			If popupMouseListener IsNot Nothing Then comboBox.removeMouseListener(popupMouseListener)
			If popupMouseMotionListener IsNot Nothing Then comboBox.removeMouseMotionListener(popupMouseMotionListener)
			If popupKeyListener IsNot Nothing Then comboBox.removeKeyListener(popupKeyListener)
			If comboBox.model IsNot Nothing Then
				If listDataListener IsNot Nothing Then comboBox.model.removeListDataListener(listDataListener)
			End If
			If mouseWheelListener IsNot Nothing Then comboBox.removeMouseWheelListener(mouseWheelListener)
		End Sub

		''' <summary>
		''' Creates the popup portion of the combo box.
		''' </summary>
		''' <returns> an instance of <code>ComboPopup</code> </returns>
		''' <seealso cref= ComboPopup </seealso>
		Protected Friend Overridable Function createPopup() As ComboPopup
			Return New BasicComboPopup(comboBox)
		End Function

		''' <summary>
		''' Creates a <code>KeyListener</code> which will be added to the
		''' combo box. If this method returns null then it will not be added
		''' to the combo box.
		''' </summary>
		''' <returns> an instance <code>KeyListener</code> or null </returns>
		Protected Friend Overridable Function createKeyListener() As KeyListener
			Return handler
		End Function

		''' <summary>
		''' Creates a <code>FocusListener</code> which will be added to the combo box.
		''' If this method returns null then it will not be added to the combo box.
		''' </summary>
		''' <returns> an instance of a <code>FocusListener</code> or null </returns>
		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function

		''' <summary>
		''' Creates a list data listener which will be added to the
		''' <code>ComboBoxModel</code>. If this method returns null then
		''' it will not be added to the combo box model.
		''' </summary>
		''' <returns> an instance of a <code>ListDataListener</code> or null </returns>
		Protected Friend Overridable Function createListDataListener() As ListDataListener
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
			Return Nothing
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
		''' Creates a layout manager for managing the components which make up the
		''' combo box.
		''' </summary>
		''' <returns> an instance of a layout manager </returns>
		Protected Friend Overridable Function createLayoutManager() As LayoutManager
			Return handler
		End Function

		''' <summary>
		''' Creates the default renderer that will be used in a non-editiable combo
		''' box. A default renderer will used only if a renderer has not been
		''' explicitly set with <code>setRenderer</code>.
		''' </summary>
		''' <returns> a <code>ListCellRender</code> used for the combo box </returns>
		''' <seealso cref= javax.swing.JComboBox#setRenderer </seealso>
		Protected Friend Overridable Function createRenderer() As ListCellRenderer
			Return New BasicComboBoxRenderer.UIResource
		End Function

		''' <summary>
		''' Creates the default editor that will be used in editable combo boxes.
		''' A default editor will be used only if an editor has not been
		''' explicitly set with <code>setEditor</code>.
		''' </summary>
		''' <returns> a <code>ComboBoxEditor</code> used for the combo box </returns>
		''' <seealso cref= javax.swing.JComboBox#setEditor </seealso>
		Protected Friend Overridable Function createEditor() As ComboBoxEditor
			Return New BasicComboBoxEditor.UIResource
		End Function

		''' <summary>
		''' Returns the shared listener.
		''' </summary>
		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Private Function createMouseWheelListener() As MouseWheelListener
			Return handler
		End Function

		'
		' end UI Initialization
		'======================


		'======================
		' begin Inner classes
		'

		''' <summary>
		''' This listener checks to see if the key event isn't a navigation key.  If
		''' it finds a key event that wasn't a navigation key it dispatches it to
		''' JComboBox.selectWithKeyChar() so that it can do type-ahead.
		''' 
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		Public Class KeyHandler
			Inherits KeyAdapter

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub keyPressed(ByVal e As KeyEvent)
				outerInstance.handler.keyPressed(e)
			End Sub
		End Class

		''' <summary>
		''' This listener hides the popup when the focus is lost.  It also repaints
		''' when focus is gained or lost.
		''' 
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		Public Class FocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for changes in the
		''' <code>ComboBoxModel</code>.
		''' <p>
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		''' <seealso cref= #createListDataListener </seealso>
		Public Class ListDataHandler
			Implements ListDataListener

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
				outerInstance.handler.contentsChanged(e)
			End Sub

			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
				outerInstance.handler.intervalAdded(e)
			End Sub

			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
				outerInstance.handler.intervalRemoved(e)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for changes to the selection in the
		''' combo box.
		''' <p>
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		''' <seealso cref= #createItemListener </seealso>
		Public Class ItemHandler
			Implements ItemListener

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			' This class used to implement behavior which is now redundant.
			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent)
			End Sub
		End Class

		''' <summary>
		''' This listener watches for bound properties that have changed in the
		''' combo box.
		''' <p>
		''' Subclasses which wish to listen to combo box property changes should
		''' call the superclass methods to ensure that the combo box ui correctly
		''' handles property changes.
		''' <p>
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class


		' Syncronizes the ToolTip text for the components within the combo box to be the
		' same value as the combo box ToolTip text.
		Private Sub updateToolTipTextForChildren()
			Dim children As Component() = comboBox.components
			For i As Integer = 0 To children.Length - 1
				If TypeOf children(i) Is JComponent Then CType(children(i), JComponent).toolTipText = comboBox.toolTipText
			Next i
		End Sub

		''' <summary>
		''' This layout manager handles the 'standard' layout of combo boxes.  It puts
		''' the arrow button to the right and the editor to the left.  If there is no
		''' editor it still keeps the arrow button to the right.
		''' 
		''' This public inner class should be treated as protected.
		''' Instantiate it only within subclasses of
		''' <code>BasicComboBoxUI</code>.
		''' </summary>
		Public Class ComboBoxLayoutManager
			Implements LayoutManager

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			End Sub

			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Return outerInstance.handler.preferredLayoutSize(parent)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Return outerInstance.handler.minimumLayoutSize(parent)
			End Function

			Public Overridable Sub layoutContainer(ByVal parent As Container)
				outerInstance.handler.layoutContainer(parent)
			End Sub
		End Class

		'
		' end Inner classes
		'====================


		'===============================
		' begin Sub-Component Management
		'

		''' <summary>
		''' Creates and initializes the components which make up the
		''' aggregate combo box. This method is called as part of the UI
		''' installation process.
		''' </summary>
		Protected Friend Overridable Sub installComponents()
			arrowButton = createArrowButton()

			If arrowButton IsNot Nothing Then
				comboBox.add(arrowButton)
				configureArrowButton()
			End If

			If comboBox.editable Then addEditor()

			comboBox.add(currentValuePane)
		End Sub

		''' <summary>
		''' The aggregate components which comprise the combo box are
		''' unregistered and uninitialized. This method is called as part of the
		''' UI uninstallation process.
		''' </summary>
		Protected Friend Overridable Sub uninstallComponents()
			If arrowButton IsNot Nothing Then unconfigureArrowButton()
			If editor IsNot Nothing Then unconfigureEditor()
			comboBox.removeAll() ' Just to be safe.
			arrowButton = Nothing
		End Sub

		''' <summary>
		''' This public method is implementation specific and should be private.
		''' do not call or override. To implement a specific editor create a
		''' custom <code>ComboBoxEditor</code>
		''' </summary>
		''' <seealso cref= #createEditor </seealso>
		''' <seealso cref= javax.swing.JComboBox#setEditor </seealso>
		''' <seealso cref= javax.swing.ComboBoxEditor </seealso>
		Public Overridable Sub addEditor()
			removeEditor()
			editor = comboBox.editor.editorComponent
			If editor IsNot Nothing Then
				configureEditor()
				comboBox.add(editor)
				If comboBox.focusOwner Then editor.requestFocusInWindow()
			End If
		End Sub

		''' <summary>
		''' This public method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		''' <seealso cref= #addEditor </seealso>
		Public Overridable Sub removeEditor()
			If editor IsNot Nothing Then
				unconfigureEditor()
				comboBox.remove(editor)
				editor = Nothing
			End If
		End Sub

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' do not call or override.
		''' </summary>
		''' <seealso cref= #addEditor </seealso>
		Protected Friend Overridable Sub configureEditor()
			' Should be in the same state as the combobox
			editor.enabled = comboBox.enabled

			editor.focusable = comboBox.focusable

			editor.font = comboBox.font

			If focusListener IsNot Nothing Then editor.addFocusListener(focusListener)

			editor.addFocusListener(handler)

			comboBox.editor.addActionListener(handler)

			If TypeOf editor Is JComponent Then
				CType(editor, JComponent).putClientProperty("doNotCancelPopup", HIDE_POPUP_KEY)
				CType(editor, JComponent).inheritsPopupMenu = True
			End If

			comboBox.configureEditor(comboBox.editor,comboBox.selectedItem)

			editor.addPropertyChangeListener(propertyChangeListener)
		End Sub

		''' <summary>
		''' This protected method is implementation specific and should be private.
		''' Do not call or override.
		''' </summary>
		''' <seealso cref= #addEditor </seealso>
		Protected Friend Overridable Sub unconfigureEditor()
			If focusListener IsNot Nothing Then editor.removeFocusListener(focusListener)

			editor.removePropertyChangeListener(propertyChangeListener)
			editor.removeFocusListener(handler)
			comboBox.editor.removeActionListener(handler)
		End Sub

		''' <summary>
		''' This public method is implementation specific and should be private. Do
		''' not call or override.
		''' </summary>
		''' <seealso cref= #createArrowButton </seealso>
		Public Overridable Sub configureArrowButton()
			If arrowButton IsNot Nothing Then
				arrowButton.enabled = comboBox.enabled
				arrowButton.focusable = comboBox.focusable
				arrowButton.requestFocusEnabled = False
				arrowButton.addMouseListener(popup.mouseListener)
				arrowButton.addMouseMotionListener(popup.mouseMotionListener)
				arrowButton.resetKeyboardActions()
				arrowButton.putClientProperty("doNotCancelPopup", HIDE_POPUP_KEY)
				arrowButton.inheritsPopupMenu = True
			End If
		End Sub

		''' <summary>
		''' This public method is implementation specific and should be private. Do
		''' not call or override.
		''' </summary>
		''' <seealso cref= #createArrowButton </seealso>
		Public Overridable Sub unconfigureArrowButton()
			If arrowButton IsNot Nothing Then
				arrowButton.removeMouseListener(popup.mouseListener)
				arrowButton.removeMouseMotionListener(popup.mouseMotionListener)
			End If
		End Sub

		''' <summary>
		''' Creates a button which will be used as the control to show or hide
		''' the popup portion of the combo box.
		''' </summary>
		''' <returns> a button which represents the popup control </returns>
		Protected Friend Overridable Function createArrowButton() As JButton
			Dim button As JButton = New BasicArrowButton(BasicArrowButton.SOUTH, UIManager.getColor("ComboBox.buttonBackground"), UIManager.getColor("ComboBox.buttonShadow"), UIManager.getColor("ComboBox.buttonDarkShadow"), UIManager.getColor("ComboBox.buttonHighlight"))
			button.name = "ComboBox.arrowButton"
			Return button
		End Function

		'
		' end Sub-Component Management
		'===============================


		'================================
		' begin ComboBoxUI Implementation
		'

		''' <summary>
		''' Tells if the popup is visible or not.
		''' </summary>
		Public Overridable Function isPopupVisible(ByVal c As JComboBox) As Boolean
			Return popup.visible
		End Function

		''' <summary>
		''' Hides the popup.
		''' </summary>
		Public Overridable Sub setPopupVisible(ByVal c As JComboBox, ByVal v As Boolean)
			If v Then
				popup.show()
			Else
				popup.hide()
			End If
		End Sub

		''' <summary>
		''' Determines if the JComboBox is focus traversable.  If the JComboBox is editable
		''' this returns false, otherwise it returns true.
		''' </summary>
		Public Overridable Function isFocusTraversable(ByVal c As JComboBox) As Boolean
			Return Not comboBox.editable
		End Function

		'
		' end ComboBoxUI Implementation
		'==============================


		'=================================
		' begin ComponentUI Implementation
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			hasFocus = comboBox.hasFocus()
			If Not comboBox.editable Then
				Dim r As Rectangle = rectangleForCurrentValue()
				paintCurrentValueBackground(g,r,hasFocus)
				paintCurrentValue(g,r,hasFocus)
			End If
		End Sub

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return getMinimumSize(c)
		End Function

		''' <summary>
		''' The minimum size is the size of the display area plus insets plus the button.
		''' </summary>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			If Not isMinimumSizeDirty Then Return New Dimension(cachedMinimumSize)
			Dim size As Dimension = displaySize
			Dim ___insets As Insets = insets
			'calculate the width and height of the button
			Dim buttonHeight As Integer = size.height
			Dim buttonWidth As Integer = If(squareButton, buttonHeight, arrowButton.preferredSize.width)
			'adjust the size based on the button width
			size.height += ___insets.top + ___insets.bottom
			size.width += ___insets.left + ___insets.right + buttonWidth

			cachedMinimumSize.sizeize(size.width, size.height)
			isMinimumSizeDirty = False

			Return New Dimension(size)
		End Function

		Public Overrides Function getMaximumSize(ByVal c As JComponent) As Dimension
			Return New Dimension(Short.MaxValue, Short.MaxValue)
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim ___baseline As Integer = -1
			' force sameBaseline to be updated.
			displaySize
			If sameBaseline Then
				Dim ___insets As Insets = c.insets
				height = height - ___insets.top - ___insets.bottom
				If Not comboBox.editable Then
					Dim renderer As ListCellRenderer = comboBox.renderer
					If renderer Is Nothing Then renderer = New DefaultListCellRenderer
					Dim value As Object = Nothing
					Dim prototypeValue As Object = comboBox.prototypeDisplayValue
					If prototypeValue IsNot Nothing Then
						value = prototypeValue
					ElseIf comboBox.model.size > 0 Then
						' Note, we're assuming the baseline is the same for all
						' cells, if not, this needs to loop through all.
						value = comboBox.model.getElementAt(0)
					End If
					Dim component As Component = renderer.getListCellRendererComponent(listBox, value, -1, False, False)
					If TypeOf component Is JLabel Then
						Dim label As JLabel = CType(component, JLabel)
						Dim text As String = label.text
						If (text Is Nothing) OrElse text.Length = 0 Then label.text = " "
					End If
					If TypeOf component Is JComponent Then component.font = comboBox.font
					___baseline = component.getBaseline(width, height)
				Else
					___baseline = editor.getBaseline(width, height)
				End If
				If ___baseline > 0 Then ___baseline += ___insets.top
			End If
			Return ___baseline
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overrides Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			' Force sameBaseline to be updated.
			displaySize
			If comboBox.editable Then
				Return editor.baselineResizeBehavior
			ElseIf sameBaseline Then
				Dim renderer As ListCellRenderer = comboBox.renderer
				If renderer Is Nothing Then renderer = New DefaultListCellRenderer
				Dim value As Object = Nothing
				Dim prototypeValue As Object = comboBox.prototypeDisplayValue
				If prototypeValue IsNot Nothing Then
					value = prototypeValue
				ElseIf comboBox.model.size > 0 Then
					' Note, we're assuming the baseline is the same for all
					' cells, if not, this needs to loop through all.
					value = comboBox.model.getElementAt(0)
				End If
				If value IsNot Nothing Then
					Dim component As Component = renderer.getListCellRendererComponent(listBox, value, -1, False, False)
					Return component.baselineResizeBehavior
				End If
			End If
			Return Component.BaselineResizeBehavior.OTHER
		End Function

		' This is currently hacky...
		Public Overrides Function getAccessibleChildrenCount(ByVal c As JComponent) As Integer
			If comboBox.editable Then
				Return 2
			Else
				Return 1
			End If
		End Function

		' This is currently hacky...
		Public Overrides Function getAccessibleChild(ByVal c As JComponent, ByVal i As Integer) As Accessible
			' 0 = the popup
			' 1 = the editor
			Select Case i
			Case 0
				If TypeOf popup Is Accessible Then
					Dim ac As AccessibleContext = CType(popup, Accessible).accessibleContext
					ac.accessibleParent = comboBox
					Return CType(popup, Accessible)
				End If
			Case 1
				If comboBox.editable AndAlso (TypeOf editor Is Accessible) Then
					Dim ac As AccessibleContext = CType(editor, Accessible).accessibleContext
					ac.accessibleParent = comboBox
					Return CType(editor, Accessible)
				End If
			End Select
			Return Nothing
		End Function

		'
		' end ComponentUI Implementation
		'===============================


		'======================
		' begin Utility Methods
		'

		''' <summary>
		''' Returns whether or not the supplied keyCode maps to a key that is used for
		''' navigation.  This is used for optimizing key input by only passing non-
		''' navigation keys to the type-ahead mechanism.  Subclasses should override this
		''' if they change the navigation keys.
		''' </summary>
		Protected Friend Overridable Function isNavigationKey(ByVal keyCode As Integer) As Boolean
			Return keyCode = KeyEvent.VK_UP OrElse keyCode = KeyEvent.VK_DOWN OrElse keyCode = KeyEvent.VK_KP_UP OrElse keyCode = KeyEvent.VK_KP_DOWN
		End Function

		Private Function isNavigationKey(ByVal keyCode As Integer, ByVal modifiers As Integer) As Boolean
			Dim ___inputMap As InputMap = comboBox.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			Dim key As KeyStroke = KeyStroke.getKeyStroke(keyCode, modifiers)

			If ___inputMap IsNot Nothing AndAlso ___inputMap.get(key) IsNot Nothing Then Return True
			Return False
		End Function

		''' <summary>
		''' Selects the next item in the list.  It won't change the selection if the
		''' currently selected item is already the last item.
		''' </summary>
		Protected Friend Overridable Sub selectNextPossibleValue()
			Dim si As Integer

			If comboBox.popupVisible Then
				si = listBox.selectedIndex
			Else
				si = comboBox.selectedIndex
			End If

			If si < comboBox.model.size - 1 Then
				listBox.selectedIndex = si + 1
				listBox.ensureIndexIsVisible(si + 1)
				If Not ___isTableCellEditor Then
					If Not(UIManager.getBoolean("ComboBox.noActionOnKeyNavigation") AndAlso comboBox.popupVisible) Then comboBox.selectedIndex = si+1
				End If
				comboBox.repaint()
			End If
		End Sub

		''' <summary>
		''' Selects the previous item in the list.  It won't change the selection if the
		''' currently selected item is already the first item.
		''' </summary>
		Protected Friend Overridable Sub selectPreviousPossibleValue()
			Dim si As Integer

			If comboBox.popupVisible Then
				si = listBox.selectedIndex
			Else
				si = comboBox.selectedIndex
			End If

			If si > 0 Then
				listBox.selectedIndex = si - 1
				listBox.ensureIndexIsVisible(si - 1)
				If Not ___isTableCellEditor Then
					If Not(UIManager.getBoolean("ComboBox.noActionOnKeyNavigation") AndAlso comboBox.popupVisible) Then comboBox.selectedIndex = si-1
				End If
				comboBox.repaint()
			End If
		End Sub

		''' <summary>
		''' Hides the popup if it is showing and shows the popup if it is hidden.
		''' </summary>
		Protected Friend Overridable Sub toggleOpenClose()
			popupVisibleble(comboBox, (Not isPopupVisible(comboBox)))
		End Sub

		''' <summary>
		''' Returns the area that is reserved for drawing the currently selected item.
		''' </summary>
		Protected Friend Overridable Function rectangleForCurrentValue() As Rectangle
			Dim width As Integer = comboBox.width
			Dim height As Integer = comboBox.height
			Dim ___insets As Insets = insets
			Dim buttonSize As Integer = height - (___insets.top + ___insets.bottom)
			If arrowButton IsNot Nothing Then buttonSize = arrowButton.width
			If BasicGraphicsUtils.isLeftToRight(comboBox) Then
				Return New Rectangle(___insets.left, ___insets.top, width - (___insets.left + ___insets.right + buttonSize), height - (___insets.top + ___insets.bottom))
			Else
				Return New Rectangle(___insets.left + buttonSize, ___insets.top, width - (___insets.left + ___insets.right + buttonSize), height - (___insets.top + ___insets.bottom))
			End If
		End Function

		''' <summary>
		''' Gets the insets from the JComboBox.
		''' </summary>
		Protected Friend Overridable Property insets As Insets
			Get
				Return comboBox.insets
			End Get
		End Property

		'
		' end Utility Methods
		'====================


		'===============================
		' begin Painting Utility Methods
		'

		''' <summary>
		''' Paints the currently selected item.
		''' </summary>
		Public Overridable Sub paintCurrentValue(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal hasFocus As Boolean)
			Dim renderer As ListCellRenderer = comboBox.renderer
			Dim c As Component

			If hasFocus AndAlso (Not isPopupVisible(comboBox)) Then
				c = renderer.getListCellRendererComponent(listBox, comboBox.selectedItem, -1, True, False)
			Else
				c = renderer.getListCellRendererComponent(listBox, comboBox.selectedItem, -1, False, False)
				c.background = UIManager.getColor("ComboBox.background")
			End If
			c.font = comboBox.font
			If hasFocus AndAlso (Not isPopupVisible(comboBox)) Then
				c.foreground = listBox.selectionForeground
				c.background = listBox.selectionBackground
			Else
				If comboBox.enabled Then
					c.foreground = comboBox.foreground
					c.background = comboBox.background
				Else
					c.foreground = sun.swing.DefaultLookup.getColor(comboBox, Me, "ComboBox.disabledForeground", Nothing)
					c.background = sun.swing.DefaultLookup.getColor(comboBox, Me, "ComboBox.disabledBackground", Nothing)
				End If
			End If

			' Fix for 4238829: should lay out the JPanel.
			Dim shouldValidate As Boolean = False
			If TypeOf c Is JPanel Then shouldValidate = True

			Dim x As Integer = bounds.x, y As Integer = bounds.y, w As Integer = bounds.width, h As Integer = bounds.height
			If padding IsNot Nothing Then
				x = bounds.x + padding.left
				y = bounds.y + padding.top
				w = bounds.width - (padding.left + padding.right)
				h = bounds.height - (padding.top + padding.bottom)
			End If

			currentValuePane.paintComponent(g,c,comboBox,x,y,w,h,shouldValidate)
		End Sub

		''' <summary>
		''' Paints the background of the currently selected item.
		''' </summary>
		Public Overridable Sub paintCurrentValueBackground(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal hasFocus As Boolean)
			Dim t As Color = g.color
			If comboBox.enabled Then
				g.color = sun.swing.DefaultLookup.getColor(comboBox, Me, "ComboBox.background", Nothing)
			Else
				g.color = sun.swing.DefaultLookup.getColor(comboBox, Me, "ComboBox.disabledBackground", Nothing)
			End If
			g.fillRect(bounds.x,bounds.y,bounds.width,bounds.height)
			g.color = t
		End Sub

		''' <summary>
		''' Repaint the currently selected item.
		''' </summary>
		Friend Overridable Sub repaintCurrentValue()
			Dim r As Rectangle = rectangleForCurrentValue()
			comboBox.repaint(r.x,r.y,r.width,r.height)
		End Sub

		'
		' end Painting Utility Methods
		'=============================


		'===============================
		' begin Size Utility Methods
		'

		''' <summary>
		''' Return the default size of an empty display area of the combo box using
		''' the current renderer and font.
		''' </summary>
		''' <returns> the size of an empty display area </returns>
		''' <seealso cref= #getDisplaySize </seealso>
		Protected Friend Overridable Property defaultSize As Dimension
			Get
				' Calculates the height and width using the default text renderer
				Dim d As Dimension = getSizeForComponent(defaultListCellRenderer.getListCellRendererComponent(listBox, " ", -1, False, False))
    
				Return New Dimension(d.width, d.height)
			End Get
		End Property

		''' <summary>
		''' Returns the calculated size of the display area. The display area is the
		''' portion of the combo box in which the selected item is displayed. This
		''' method will use the prototype display value if it has been set.
		''' <p>
		''' For combo boxes with a non trivial number of items, it is recommended to
		''' use a prototype display value to significantly speed up the display
		''' size calculation.
		''' </summary>
		''' <returns> the size of the display area calculated from the combo box items </returns>
		''' <seealso cref= javax.swing.JComboBox#setPrototypeDisplayValue </seealso>
		Protected Friend Overridable Property displaySize As Dimension
			Get
				If Not isDisplaySizeDirty Then Return New Dimension(cachedDisplaySize)
				Dim result As New Dimension
    
				Dim renderer As ListCellRenderer = comboBox.renderer
				If renderer Is Nothing Then renderer = New DefaultListCellRenderer
    
				sameBaseline = True
    
				Dim prototypeValue As Object = comboBox.prototypeDisplayValue
				If prototypeValue IsNot Nothing Then
					' Calculates the dimension based on the prototype value
					result = getSizeForComponent(renderer.getListCellRendererComponent(listBox, prototypeValue, -1, False, False))
				Else
					' Calculate the dimension by iterating over all the elements in the combo
					' box list.
					Dim model As ComboBoxModel = comboBox.model
					Dim modelSize As Integer = model.size
					Dim ___baseline As Integer = -1
					Dim d As Dimension
    
					Dim cpn As Component
    
					If modelSize > 0 Then
						For i As Integer = 0 To modelSize - 1
							' Calculates the maximum height and width based on the largest
							' element
							Dim value As Object = model.getElementAt(i)
							Dim c As Component = renderer.getListCellRendererComponent(listBox, value, -1, False, False)
							d = getSizeForComponent(c)
							If sameBaseline AndAlso value IsNot Nothing AndAlso (Not(TypeOf value Is String) OrElse (Not "".Equals(value))) Then
								Dim newBaseline As Integer = c.getBaseline(d.width, d.height)
								If newBaseline = -1 Then
									sameBaseline = False
								ElseIf ___baseline = -1 Then
									___baseline = newBaseline
								ElseIf ___baseline <> newBaseline Then
									sameBaseline = False
								End If
							End If
							result.width = Math.Max(result.width,d.width)
							result.height = Math.Max(result.height,d.height)
						Next i
					Else
						result = defaultSize
						If comboBox.editable Then result.width = 100
					End If
				End If
    
				If comboBox.editable Then
					Dim d As Dimension = editor.preferredSize
					result.width = Math.Max(result.width,d.width)
					result.height = Math.Max(result.height,d.height)
				End If
    
				' calculate in the padding
				If padding IsNot Nothing Then
					result.width += padding.left + padding.right
					result.height += padding.top + padding.bottom
				End If
    
				' Set the cached value
				cachedDisplaySize.sizeize(result.width, result.height)
				isDisplaySizeDirty = False
    
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns the size a component would have if used as a cell renderer.
		''' </summary>
		''' <param name="comp"> a {@code Component} to check </param>
		''' <returns> size of the component
		''' @since 1.7 </returns>
		Protected Friend Overridable Function getSizeForComponent(ByVal comp As Component) As Dimension
			' This has been refactored out in hopes that it may be investigated and
			' simplified for the next major release. adding/removing
			' the component to the currentValuePane and changing the font may be
			' redundant operations.
			currentValuePane.add(comp)
			comp.font = comboBox.font
			Dim d As Dimension = comp.preferredSize
			currentValuePane.remove(comp)
			Return d
		End Function


		'
		' end Size Utility Methods
		'=============================


		'=================================
		' begin Keyboard Action Management
		'

		''' <summary>
		''' Adds keyboard actions to the JComboBox.  Actions on enter and esc are already
		''' supplied.  Add more actions as you need them.
		''' </summary>
		Protected Friend Overridable Sub installKeyboardActions()
			Dim km As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			SwingUtilities.replaceUIInputMap(comboBox, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)


			LazyActionMap.installLazyActionMap(comboBox, GetType(BasicComboBoxUI), "ComboBox.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(sun.swing.DefaultLookup.get(comboBox, Me, "ComboBox.ancestorInputMap"), InputMap)
			Return Nothing
		End Function

		Friend Overridable Property tableCellEditor As Boolean
			Get
				Return ___isTableCellEditor
			End Get
		End Property

		''' <summary>
		''' Removes the focus InputMap and ActionMap.
		''' </summary>
		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(comboBox, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
			SwingUtilities.replaceUIActionMap(comboBox, Nothing)
		End Sub


		'
		' Actions
		'
		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const HIDE As String = "hidePopup"
			Private Const DOWN As String = "selectNext"
			Private Const DOWN_2 As String = "selectNext2"
			Private Const TOGGLE As String = "togglePopup"
			Private Const TOGGLE_2 As String = "spacePopup"
			Private Const UP As String = "selectPrevious"
			Private Const UP_2 As String = "selectPrevious2"
			Private Const ENTER As String = "enterPressed"
			Private Const PAGE_DOWN As String = "pageDownPassThrough"
			Private Const PAGE_UP As String = "pageUpPassThrough"
			Private Const HOME As String = "homePassThrough"
			Private Const [END] As String = "endPassThrough"

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim key As String = name
				Dim comboBox As JComboBox = CType(e.source, JComboBox)
				Dim ui As BasicComboBoxUI = CType(BasicLookAndFeel.getUIOfType(comboBox.uI, GetType(BasicComboBoxUI)), BasicComboBoxUI)
				If key = HIDE Then
					comboBox.firePopupMenuCanceled()
					comboBox.popupVisible = False
				ElseIf key = PAGE_DOWN OrElse key = PAGE_UP OrElse key = HOME OrElse key = [END] Then
					Dim index As Integer = getNextIndex(comboBox, key)
					If index >= 0 AndAlso index < comboBox.itemCount Then
						If UIManager.getBoolean("ComboBox.noActionOnKeyNavigation") AndAlso comboBox.popupVisible Then
							ui.listBox.selectedIndex = index
							ui.listBox.ensureIndexIsVisible(index)
							comboBox.repaint()
						Else
							comboBox.selectedIndex = index
						End If
					End If
				ElseIf key = DOWN Then
					If comboBox.showing Then
						If comboBox.popupVisible Then
							If ui IsNot Nothing Then ui.selectNextPossibleValue()
						Else
							comboBox.popupVisible = True
						End If
					End If
				ElseIf key = DOWN_2 Then
					' Special case in which pressing the arrow keys will not
					' make the popup appear - except for editable combo boxes
					' and combo boxes inside a table.
					If comboBox.showing Then
						If (comboBox.editable OrElse (ui IsNot Nothing AndAlso ui.tableCellEditor)) AndAlso (Not comboBox.popupVisible) Then
							comboBox.popupVisible = True
						Else
							If ui IsNot Nothing Then ui.selectNextPossibleValue()
						End If
					End If
				ElseIf key = TOGGLE OrElse key = TOGGLE_2 Then
					If ui IsNot Nothing AndAlso (key = TOGGLE OrElse (Not comboBox.editable)) Then
						If ui.tableCellEditor Then
							' Forces the selection of the list item if the
							' combo box is in a JTable.
							comboBox.selectedIndex = ui.popup.list.selectedIndex
						Else
							comboBox.popupVisible = (Not comboBox.popupVisible)
						End If
					End If
				ElseIf key = UP Then
					If ui IsNot Nothing Then
						If ui.isPopupVisible(comboBox) Then
							ui.selectPreviousPossibleValue()
						ElseIf sun.swing.DefaultLookup.getBoolean(comboBox, ui, "ComboBox.showPopupOnNavigation", False) Then
							ui.popupVisibleble(comboBox, True)
						End If
					End If
				ElseIf key = UP_2 Then
					 ' Special case in which pressing the arrow keys will not
					 ' make the popup appear - except for editable combo boxes.
					 If comboBox.showing AndAlso ui IsNot Nothing Then
						 If comboBox.editable AndAlso (Not comboBox.popupVisible) Then
							 comboBox.popupVisible = True
						 Else
							 ui.selectPreviousPossibleValue()
						 End If
					 End If

				ElseIf key = ENTER Then
					If comboBox.popupVisible Then
						' If ComboBox.noActionOnKeyNavigation is set,
						' forse selection of list item
						If UIManager.getBoolean("ComboBox.noActionOnKeyNavigation") Then
							Dim listItem As Object = ui.popup.list.selectedValue
							If listItem IsNot Nothing Then
								comboBox.editor.item = listItem
								comboBox.selectedItem = listItem
							End If
							comboBox.popupVisible = False
						Else
							' Forces the selection of the list item
							Dim isEnterSelectablePopup As Boolean = UIManager.getBoolean("ComboBox.isEnterSelectablePopup")
							If (Not comboBox.editable) OrElse isEnterSelectablePopup OrElse ui.isTableCellEditor Then
								Dim listItem As Object = ui.popup.list.selectedValue
								If listItem IsNot Nothing Then
									' Use the selected value from popup
									' to set the selected item in combo box,
									' but ensure before that JComboBox.actionPerformed()
									' won't use editor's value to set the selected item
									comboBox.editor.item = listItem
									comboBox.selectedItem = listItem
								End If
							End If
							comboBox.popupVisible = False
						End If
					Else
						' Hide combo box if it is a table cell editor
						If ui.isTableCellEditor AndAlso (Not comboBox.editable) Then comboBox.selectedItem = comboBox.selectedItem
						' Call the default button binding.
						' This is a pretty messy way of passing an event through
						' to the root pane.
						Dim root As JRootPane = SwingUtilities.getRootPane(comboBox)
						If root IsNot Nothing Then
							Dim im As InputMap = root.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
							Dim am As ActionMap = root.actionMap
							If im IsNot Nothing AndAlso am IsNot Nothing Then
								Dim obj As Object = im.get(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER,0))
								If obj IsNot Nothing Then
									Dim action As Action = am.get(obj)
									If action IsNot Nothing Then action.actionPerformed(New ActionEvent(root, e.iD, e.actionCommand, e.when, e.modifiers))
								End If
							End If
						End If
					End If
				End If
			End Sub

			Private Function getNextIndex(ByVal comboBox As JComboBox, ByVal key As String) As Integer
				Dim listHeight As Integer = comboBox.maximumRowCount

				Dim selectedIndex As Integer = comboBox.selectedIndex
				If UIManager.getBoolean("ComboBox.noActionOnKeyNavigation") AndAlso (TypeOf comboBox.uI Is BasicComboBoxUI) Then selectedIndex = CType(comboBox.uI, BasicComboBoxUI).listBox.selectedIndex

				If key = PAGE_UP Then
					Dim index As Integer = selectedIndex - listHeight
					Return (If(index < 0, 0, index))
				ElseIf key = PAGE_DOWN Then
					Dim index As Integer = selectedIndex + listHeight
					Dim max As Integer = comboBox.itemCount
					Return (If(index < max, index, max-1))
				ElseIf key = HOME Then
					Return 0
				ElseIf key = [END] Then
					Return comboBox.itemCount - 1
				End If
				Return comboBox.selectedIndex
			End Function

			Public Overridable Function isEnabled(ByVal c As Object) As Boolean
				If name = HIDE Then Return (c IsNot Nothing AndAlso CType(c, JComboBox).popupVisible)
				Return True
			End Function
		End Class
		'
		' end Keyboard Action Management
		'===============================


		'
		' Shared Handler, implements all listeners
		'
		Private Class Handler
			Implements ActionListener, FocusListener, KeyListener, LayoutManager, ListDataListener, java.beans.PropertyChangeListener, MouseWheelListener

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				If e.source Is outerInstance.editor Then
					' If the border of the editor changes then this can effect
					' the size of the editor which can cause the combo's size to
					' become invalid so we need to clear size caches
					If "border".Equals(propertyName) Then
						outerInstance.isMinimumSizeDirty = True
						outerInstance.isDisplaySizeDirty = True
						outerInstance.comboBox.revalidate()
					End If
				Else
					Dim comboBox As JComboBox = CType(e.source, JComboBox)
					If propertyName = "model" Then
						Dim newModel As ComboBoxModel = CType(e.newValue, ComboBoxModel)
						Dim oldModel As ComboBoxModel = CType(e.oldValue, ComboBoxModel)

						If oldModel IsNot Nothing AndAlso outerInstance.listDataListener IsNot Nothing Then oldModel.removeListDataListener(outerInstance.listDataListener)

						If newModel IsNot Nothing AndAlso outerInstance.listDataListener IsNot Nothing Then newModel.addListDataListener(outerInstance.listDataListener)

						If outerInstance.editor IsNot Nothing Then comboBox.configureEditor(comboBox.editor, comboBox.selectedItem)
						outerInstance.isMinimumSizeDirty = True
						outerInstance.isDisplaySizeDirty = True
						comboBox.revalidate()
						comboBox.repaint()
					ElseIf propertyName = "editor" AndAlso comboBox.editable Then
						outerInstance.addEditor()
						comboBox.revalidate()
					ElseIf propertyName = "editable" Then
						If comboBox.editable Then
							comboBox.requestFocusEnabled = False
							outerInstance.addEditor()
						Else
							comboBox.requestFocusEnabled = True
							outerInstance.removeEditor()
						End If
						outerInstance.updateToolTipTextForChildren()
						comboBox.revalidate()
					ElseIf propertyName = "enabled" Then
						Dim enabled As Boolean = comboBox.enabled
						If outerInstance.editor IsNot Nothing Then outerInstance.editor.enabled = enabled
						If outerInstance.arrowButton IsNot Nothing Then outerInstance.arrowButton.enabled = enabled
						comboBox.repaint()
					ElseIf propertyName = "focusable" Then
						Dim focusable As Boolean = comboBox.focusable
						If outerInstance.editor IsNot Nothing Then outerInstance.editor.focusable = focusable
						If outerInstance.arrowButton IsNot Nothing Then outerInstance.arrowButton.focusable = focusable
						comboBox.repaint()
					ElseIf propertyName = "maximumRowCount" Then
						If outerInstance.isPopupVisible(comboBox) Then
							outerInstance.popupVisibleble(comboBox, False)
							outerInstance.popupVisibleble(comboBox, True)
						End If
					ElseIf propertyName = "font" Then
						outerInstance.listBox.font = comboBox.font
						If outerInstance.editor IsNot Nothing Then outerInstance.editor.font = comboBox.font
						outerInstance.isMinimumSizeDirty = True
						outerInstance.isDisplaySizeDirty = True
						comboBox.validate()
					ElseIf propertyName = JComponent.TOOL_TIP_TEXT_KEY Then
						outerInstance.updateToolTipTextForChildren()
					ElseIf propertyName = BasicComboBoxUI.IS_TABLE_CELL_EDITOR Then
						Dim inTable As Boolean? = CBool(e.newValue)
						outerInstance.___isTableCellEditor = If(inTable.Equals(Boolean.TRUE), True, False)
					ElseIf propertyName = "prototypeDisplayValue" Then
						outerInstance.isMinimumSizeDirty = True
						outerInstance.isDisplaySizeDirty = True
						comboBox.revalidate()
					ElseIf propertyName = "renderer" Then
						outerInstance.isMinimumSizeDirty = True
						outerInstance.isDisplaySizeDirty = True
						comboBox.revalidate()
					End If
				End If
			End Sub


			'
			' KeyListener
			'

			' This listener checks to see if the key event isn't a navigation
			' key.  If it finds a key event that wasn't a navigation key it
			' dispatches it to JComboBox.selectWithKeyChar() so that it can do
			' type-ahead.
			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If outerInstance.isNavigationKey(e.keyCode, e.modifiers) Then
					outerInstance.lastTime = 0L
				ElseIf outerInstance.comboBox.enabled AndAlso outerInstance.comboBox.model.size<>0 AndAlso isTypeAheadKey(e) AndAlso e.keyChar <> KeyEvent.CHAR_UNDEFINED Then
					outerInstance.time = e.when
					If outerInstance.comboBox.selectWithKeyChar(e.keyChar) Then e.consume()
				End If
			End Sub

			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub

			Private Function isTypeAheadKey(ByVal e As KeyEvent) As Boolean
				Return (Not e.altDown) AndAlso Not BasicGraphicsUtils.isMenuShortcutKeyDown(e)
			End Function

			'
			' FocusListener
			'
			' NOTE: The class is added to both the Editor and ComboBox.
			' The combo box listener hides the popup when the focus is lost.
			' It also repaints when focus is gained or lost.

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				Dim comboBoxEditor As ComboBoxEditor = outerInstance.comboBox.editor

				If (comboBoxEditor IsNot Nothing) AndAlso (e.source Is comboBoxEditor.editorComponent) Then Return
				outerInstance.hasFocus = True
				outerInstance.comboBox.repaint()

				If outerInstance.comboBox.editable AndAlso outerInstance.editor IsNot Nothing Then outerInstance.editor.requestFocus()
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				Dim editor As ComboBoxEditor = outerInstance.comboBox.editor
				If (editor IsNot Nothing) AndAlso (e.source Is editor.editorComponent) Then
					Dim item As Object = editor.item

					Dim selectedItem As Object = outerInstance.comboBox.selectedItem
					If (Not e.temporary) AndAlso item IsNot Nothing AndAlso (Not item.Equals(If(selectedItem Is Nothing, "", selectedItem))) Then outerInstance.comboBox.actionPerformed(New ActionEvent(editor, 0, "", EventQueue.mostRecentEventTime, 0))
				End If

				outerInstance.hasFocus = False
				If Not e.temporary Then outerInstance.popupVisibleble(outerInstance.comboBox, False)
				outerInstance.comboBox.repaint()
			End Sub

			'
			' ListDataListener
			'

			' This listener watches for changes in the ComboBoxModel
			Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
				If Not(e.index0 = -1 AndAlso e.index1 = -1) Then
					outerInstance.isMinimumSizeDirty = True
					outerInstance.comboBox.revalidate()
				End If

				' set the editor with the selected item since this
				' is the event handler for a selected item change.
				If outerInstance.comboBox.editable AndAlso outerInstance.editor IsNot Nothing Then outerInstance.comboBox.configureEditor(outerInstance.comboBox.editor, outerInstance.comboBox.selectedItem)

				outerInstance.isDisplaySizeDirty = True
				outerInstance.comboBox.repaint()
			End Sub

			Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
				contentsChanged(e)
			End Sub

			Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
				contentsChanged(e)
			End Sub

			'
			' LayoutManager
			'

			' This layout manager handles the 'standard' layout of combo boxes.
			' It puts the arrow button to the right and the editor to the left.
			' If there is no editor it still keeps the arrow button to the right.
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			End Sub

			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Return parent.preferredSize
			End Function

			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Return parent.minimumSize
			End Function

			Public Overridable Sub layoutContainer(ByVal parent As Container)
				Dim cb As JComboBox = CType(parent, JComboBox)
				Dim width As Integer = cb.width
				Dim height As Integer = cb.height

				Dim insets As Insets = outerInstance.insets
				Dim buttonHeight As Integer = height - (insets.top + insets.bottom)
				Dim buttonWidth As Integer = buttonHeight
				If outerInstance.arrowButton IsNot Nothing Then
					Dim arrowInsets As Insets = outerInstance.arrowButton.insets
					buttonWidth = If(outerInstance.squareButton, buttonHeight, outerInstance.arrowButton.preferredSize.width + arrowInsets.left + arrowInsets.right)
				End If
				Dim cvb As Rectangle

				If outerInstance.arrowButton IsNot Nothing Then
					If BasicGraphicsUtils.isLeftToRight(cb) Then
						outerInstance.arrowButton.boundsnds(width - (insets.right + buttonWidth), insets.top, buttonWidth, buttonHeight)
					Else
						outerInstance.arrowButton.boundsnds(insets.left, insets.top, buttonWidth, buttonHeight)
					End If
				End If
				If outerInstance.editor IsNot Nothing Then
					cvb = outerInstance.rectangleForCurrentValue()
					outerInstance.editor.bounds = cvb
				End If
			End Sub

			'
			' ActionListener
			'
			' Fix for 4515752: Forward the Enter pressed on the
			' editable combo box to the default button

			' Note: This could depend on event ordering. The first ActionEvent
			' from the editor may be handled by the JComboBox in which case, the
			' enterPressed action will always be invoked.
			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim item As Object = outerInstance.comboBox.editor.item
				If item IsNot Nothing Then
					If (Not outerInstance.comboBox.popupVisible) AndAlso (Not item.Equals(outerInstance.comboBox.selectedItem)) Then outerInstance.comboBox.selectedItem = outerInstance.comboBox.editor.item
					Dim am As ActionMap = outerInstance.comboBox.actionMap
					If am IsNot Nothing Then
						Dim action As Action = am.get("enterPressed")
						If action IsNot Nothing Then action.actionPerformed(New ActionEvent(outerInstance.comboBox, evt.iD, evt.actionCommand, evt.modifiers))
					End If
				End If
			End Sub

			Public Overridable Sub mouseWheelMoved(ByVal e As MouseWheelEvent)
				e.consume()
			End Sub
		End Class

		Friend Class DefaultKeySelectionManager
			Implements JComboBox.KeySelectionManager, UIResource

			Private ReadOnly outerInstance As BasicComboBoxUI

			Public Sub New(ByVal outerInstance As BasicComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			Private prefix As String = ""
			Private typedString As String = ""

			Public Overridable Function selectionForKey(ByVal aKey As Char, ByVal aModel As ComboBoxModel) As Integer Implements JComboBox.KeySelectionManager.selectionForKey
				If outerInstance.lastTime = 0L Then
					prefix = ""
					typedString = ""
				End If
				Dim startingFromSelection As Boolean = True

				Dim startIndex As Integer = outerInstance.comboBox.selectedIndex
				If outerInstance.time - outerInstance.lastTime < outerInstance.timeFactor Then
					typedString += aKey
					If (prefix.Length = 1) AndAlso (aKey = prefix.Chars(0)) Then
						' Subsequent same key presses move the keyboard focus to the next
						' object that starts with the same letter.
						startIndex += 1
					Else
						prefix = typedString
					End If
				Else
					startIndex += 1
					typedString = "" & AscW(aKey)
					prefix = typedString
				End If
				outerInstance.lastTime = outerInstance.time

				If startIndex < 0 OrElse startIndex >= aModel.size Then
					startingFromSelection = False
					startIndex = 0
				End If
				Dim index As Integer = outerInstance.listBox.getNextMatch(prefix, startIndex, Position.Bias.Forward)
				If index < 0 AndAlso startingFromSelection Then ' wrap index = outerInstance.listBox.getNextMatch(prefix, 0, Position.Bias.Forward)
				Return index
			End Function
		End Class

	End Class

End Namespace