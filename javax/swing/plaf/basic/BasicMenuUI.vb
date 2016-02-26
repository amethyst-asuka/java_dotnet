Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.border

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
	''' A default L&amp;F implementation of MenuUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' </summary>
	Public Class BasicMenuUI
		Inherits BasicMenuItemUI

		Protected Friend changeListener As ChangeListener
		Protected Friend menuListener As MenuListener

		Private lastMnemonic As Integer = 0

		''' <summary>
		''' Uses as the parent of the windowInputMap when selected. </summary>
		Private selectedWindowInputMap As InputMap

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		Private Shared crossMenuMnemonic As Boolean = True

		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New BasicMenuUI
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			BasicMenuItemUI.loadActionMap(map)
			map.put(New Actions(Actions.SELECT, Nothing, True))
		End Sub


		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			updateDefaultBackgroundColor()
			CType(menuItem, JMenu).delay = 200
			crossMenuMnemonic = UIManager.getBoolean("Menu.crossMenuMnemonic")
		End Sub

		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "Menu"
			End Get
		End Property

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()

			If changeListener Is Nothing Then changeListener = createChangeListener(menuItem)

			If changeListener IsNot Nothing Then menuItem.addChangeListener(changeListener)

			If menuListener Is Nothing Then menuListener = createMenuListener(menuItem)

			If menuListener IsNot Nothing Then CType(menuItem, JMenu).addMenuListener(menuListener)
		End Sub

		Protected Friend Overrides Sub installKeyboardActions()
			MyBase.installKeyboardActions()
			updateMnemonicBinding()
		End Sub

		Friend Overrides Sub installLazyActionMap()
			LazyActionMap.installLazyActionMap(menuItem, GetType(BasicMenuUI), propertyPrefix & ".actionMap")
		End Sub

		Friend Overridable Sub updateMnemonicBinding()
			Dim mnemonic As Integer = menuItem.model.mnemonic
			Dim shortcutKeys As Integer() = CType(sun.swing.DefaultLookup.get(menuItem, Me, "Menu.shortcutKeys"), Integer())
			If shortcutKeys Is Nothing Then shortcutKeys = New Integer() {KeyEvent.ALT_MASK}
			If mnemonic = lastMnemonic Then Return
			Dim windowInputMap As InputMap = SwingUtilities.getUIInputMap(menuItem, JComponent.WHEN_IN_FOCUSED_WINDOW)
			If lastMnemonic <> 0 AndAlso windowInputMap IsNot Nothing Then
				For Each shortcutKey As Integer In shortcutKeys
					windowInputMap.remove(KeyStroke.getKeyStroke(lastMnemonic, shortcutKey, False))
				Next shortcutKey
			End If
			If mnemonic <> 0 Then
				If windowInputMap Is Nothing Then
					windowInputMap = createInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
					SwingUtilities.replaceUIInputMap(menuItem, JComponent.WHEN_IN_FOCUSED_WINDOW, windowInputMap)
				End If
				For Each shortcutKey As Integer In shortcutKeys
					windowInputMap.put(KeyStroke.getKeyStroke(mnemonic, shortcutKey, False), "selectMenu")
				Next shortcutKey
			End If
			lastMnemonic = mnemonic
		End Sub

		Protected Friend Overrides Sub uninstallKeyboardActions()
			MyBase.uninstallKeyboardActions()
			lastMnemonic = 0
		End Sub

		Protected Friend Overrides Function createMouseInputListener(ByVal c As JComponent) As MouseInputListener
			Return handler
		End Function

		Protected Friend Overridable Function createMenuListener(ByVal c As JComponent) As MenuListener
			Return Nothing
		End Function

		Protected Friend Overridable Function createChangeListener(ByVal c As JComponent) As ChangeListener
			Return Nothing
		End Function

		Protected Friend Overrides Function createPropertyChangeListener(ByVal c As JComponent) As PropertyChangeListener
			Return handler
		End Function

		Friend Property Overrides handler As BasicMenuItemUI.Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overrides Sub uninstallDefaults()
			menuItem.armed = False
			menuItem.selected = False
			menuItem.resetKeyboardActions()
			MyBase.uninstallDefaults()
		End Sub

		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()

			If changeListener IsNot Nothing Then menuItem.removeChangeListener(changeListener)

			If menuListener IsNot Nothing Then CType(menuItem, JMenu).removeMenuListener(menuListener)

			changeListener = Nothing
			menuListener = Nothing
			handler = Nothing
		End Sub

		Protected Friend Overrides Function createMenuDragMouseListener(ByVal c As JComponent) As MenuDragMouseListener
			Return handler
		End Function

		Protected Friend Overrides Function createMenuKeyListener(ByVal c As JComponent) As MenuKeyListener
			Return CType(handler, MenuKeyListener)
		End Function

		Public Overrides Function getMaximumSize(ByVal c As JComponent) As Dimension
			If CType(menuItem, JMenu).topLevelMenu = True Then
				Dim d As Dimension = c.preferredSize
				Return New Dimension(d.width, Short.MaxValue)
			End If
			Return Nothing
		End Function

		Protected Friend Overridable Sub setupPostTimer(ByVal menu As JMenu)
			Dim ___timer As New Timer(menu.delay, New Actions(Actions.SELECT, menu,False))
			___timer.repeats = False
			___timer.start()
		End Sub

		Private Shared Sub appendPath(ByVal path As MenuElement(), ByVal elem As MenuElement)
			Dim newPath As MenuElement() = New MenuElement(path.Length){}
			Array.Copy(path, 0, newPath, 0, path.Length)
			newPath(path.Length) = elem
			MenuSelectionManager.defaultManager().selectedPath = newPath
		End Sub

		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const [SELECT] As String = "selectMenu"

			' NOTE: This will be null if the action is registered in the
			' ActionMap. For the timer use it will be non-null.
			Private menu As JMenu
			Private force As Boolean=False

			Friend Sub New(ByVal key As String, ByVal menu As JMenu, ByVal shouldForce As Boolean)
				MyBase.New(key)
				Me.menu = menu
				Me.force = shouldForce
			End Sub

			Private Function getMenu(ByVal e As ActionEvent) As JMenu
				If TypeOf e.source Is JMenu Then Return CType(e.source, JMenu)
				Return menu
			End Function

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim ___menu As JMenu = getMenu(e)
				If Not crossMenuMnemonic Then
					Dim pm As JPopupMenu = BasicPopupMenuUI.lastPopup
					If pm IsNot Nothing AndAlso pm IsNot ___menu.parent Then Return
				End If

				Dim defaultManager As MenuSelectionManager = MenuSelectionManager.defaultManager()
				If force Then
					Dim cnt As Container = ___menu.parent
					If cnt IsNot Nothing AndAlso TypeOf cnt Is JMenuBar Then
						Dim [me] As MenuElement()
						Dim subElements As MenuElement()

						subElements = ___menu.popupMenu.subElements
						If subElements.Length > 0 Then
							[me] = New MenuElement(3){}
							[me](0) = CType(cnt, MenuElement)
							[me](1) = ___menu
							[me](2) = ___menu.popupMenu
							[me](3) = subElements(0)
						Else
							[me] = New MenuElement(2){}
							[me](0) = CType(cnt, MenuElement)
							[me](1) = ___menu
							[me](2) = ___menu.popupMenu
						End If
						defaultManager.selectedPath = [me]
					End If
				Else
					Dim path As MenuElement() = defaultManager.selectedPath
					If path.Length > 0 AndAlso path(path.Length-1) Is ___menu Then appendPath(path, ___menu.popupMenu)
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal c As Object) As Boolean
				If TypeOf c Is JMenu Then Return CType(c, JMenu).enabled
				Return True
			End Function
		End Class

	'    
	'     * Set the background color depending on whether this is a toplevel menu
	'     * in a menubar or a submenu of another menu.
	'     
		Private Sub updateDefaultBackgroundColor()
			If Not UIManager.getBoolean("Menu.useMenuBarBackgroundForTopLevel") Then Return
			Dim menu As JMenu = CType(menuItem, JMenu)
			If TypeOf menu.background Is UIResource Then
				If menu.topLevelMenu Then
					menu.background = UIManager.getColor("MenuBar.background")
				Else
					menu.background = UIManager.getColor(propertyPrefix & ".background")
				End If
			End If
		End Sub

		''' <summary>
		''' Instantiated and used by a menu item to handle the current menu selection
		''' from mouse events. A MouseInputHandler processes and forwards all mouse events
		''' to a shared instance of the MenuSelectionManager.
		''' <p>
		''' This class is protected so that it can be subclassed by other look and
		''' feels to implement their own mouse handling behavior. All overridden
		''' methods should call the parent methods so that the menu selection
		''' is correct.
		''' </summary>
		''' <seealso cref= javax.swing.MenuSelectionManager
		''' @since 1.4 </seealso>
		Protected Friend Class MouseInputHandler
			Implements MouseInputListener

			Private ReadOnly outerInstance As BasicMenuUI

			Public Sub New(ByVal outerInstance As BasicMenuUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				outerInstance.handler.mouseClicked(e)
			End Sub

			''' <summary>
			''' Invoked when the mouse has been clicked on the menu. This
			''' method clears or sets the selection path of the
			''' MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub

			''' <summary>
			''' Invoked when the mouse has been released on the menu. Delegates the
			''' mouse event to the MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.handler.mouseReleased(e)
			End Sub

			''' <summary>
			''' Invoked when the cursor enters the menu. This method sets the selected
			''' path for the MenuSelectionManager and handles the case
			''' in which a menu item is used to pop up an additional menu, as in a
			''' hierarchical menu system.
			''' </summary>
			''' <param name="e"> the mouse event; not used </param>
			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.handler.mouseEntered(e)
			End Sub
			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.handler.mouseExited(e)
			End Sub

			''' <summary>
			''' Invoked when a mouse button is pressed on the menu and then dragged.
			''' Delegates the mouse event to the MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			''' <seealso cref= java.awt.event.MouseMotionListener#mouseDragged </seealso>
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				outerInstance.handler.mouseDragged(e)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.handler.mouseMoved(e)
			End Sub
		End Class

		''' <summary>
		''' As of Java 2 platform 1.4, this previously undocumented class
		''' is now obsolete. KeyBindings are now managed by the popup menu.
		''' </summary>
		Public Class ChangeHandler
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicMenuUI

			Public menu As JMenu
			Public ui As BasicMenuUI
			Public isSelected As Boolean = False
			Public wasFocused As Component

			Public Sub New(ByVal outerInstance As BasicMenuUI, ByVal m As JMenu, ByVal ui As BasicMenuUI)
					Me.outerInstance = outerInstance
				menu = m
				Me.ui = ui
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
			End Sub
		End Class

		Private Class Handler
			Inherits BasicMenuItemUI.Handler
			Implements MenuKeyListener

			Private ReadOnly outerInstance As BasicMenuUI

			Public Sub New(ByVal outerInstance As BasicMenuUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				If e.propertyName = AbstractButton.MNEMONIC_CHANGED_PROPERTY Then
					outerInstance.updateMnemonicBinding()
				Else
					If e.propertyName.Equals("ancestor") Then outerInstance.updateDefaultBackgroundColor()
					MyBase.propertyChange(e)
				End If
			End Sub

			'
			' MouseInputListener
			'
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			''' <summary>
			''' Invoked when the mouse has been clicked on the menu. This
			''' method clears or sets the selection path of the
			''' MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				Dim menu As JMenu = CType(outerInstance.menuItem, JMenu)
				If Not menu.enabled Then Return

				Dim manager As MenuSelectionManager = MenuSelectionManager.defaultManager()
				If menu.topLevelMenu Then
					If menu.selected AndAlso menu.popupMenu.showing Then
						manager.clearSelectedPath()
					Else
						Dim cnt As Container = menu.parent
						If cnt IsNot Nothing AndAlso TypeOf cnt Is JMenuBar Then
							Dim [me] As MenuElement() = New MenuElement(1){}
							[me](0)=CType(cnt, MenuElement)
							[me](1)=menu
							manager.selectedPath = [me]
						End If
					End If
				End If

				Dim selectedPath As MenuElement() = manager.selectedPath
				If selectedPath.Length > 0 AndAlso selectedPath(selectedPath.Length-1) IsNot menu.popupMenu Then

					If menu.topLevelMenu OrElse menu.delay = 0 Then
						appendPath(selectedPath, menu.popupMenu)
					Else
						outerInstance.setupPostTimer(menu)
					End If
				End If
			End Sub

			''' <summary>
			''' Invoked when the mouse has been released on the menu. Delegates the
			''' mouse event to the MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				Dim menu As JMenu = CType(outerInstance.menuItem, JMenu)
				If Not menu.enabled Then Return
				Dim manager As MenuSelectionManager = MenuSelectionManager.defaultManager()
				manager.processMouseEvent(e)
				If Not e.consumed Then manager.clearSelectedPath()
			End Sub

			''' <summary>
			''' Invoked when the cursor enters the menu. This method sets the selected
			''' path for the MenuSelectionManager and handles the case
			''' in which a menu item is used to pop up an additional menu, as in a
			''' hierarchical menu system.
			''' </summary>
			''' <param name="e"> the mouse event; not used </param>
			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				Dim menu As JMenu = CType(outerInstance.menuItem, JMenu)
				' only disable the menu highlighting if it's disabled and the property isn't
				' true. This allows disabled rollovers to work in WinL&F
				If (Not menu.enabled) AndAlso (Not UIManager.getBoolean("MenuItem.disabledAreNavigable")) Then Return

				Dim manager As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim selectedPath As MenuElement() = manager.selectedPath
				If Not menu.topLevelMenu Then
					If Not(selectedPath.Length > 0 AndAlso selectedPath(selectedPath.Length-1) Is menu.popupMenu) Then
						If menu.delay = 0 Then
							appendPath(outerInstance.path, menu.popupMenu)
						Else
							manager.selectedPath = outerInstance.path
							outerInstance.setupPostTimer(menu)
						End If
					End If
				Else
					If selectedPath.Length > 0 AndAlso selectedPath(0) Is menu.parent Then
						Dim newPath As MenuElement() = New MenuElement(2){}
						' A top level menu's parent is by definition
						' a JMenuBar
						newPath(0) = CType(menu.parent, MenuElement)
						newPath(1) = menu
						If BasicPopupMenuUI.lastPopup IsNot Nothing Then newPath(2) = menu.popupMenu
						manager.selectedPath = newPath
					End If
				End If
			End Sub
			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			''' <summary>
			''' Invoked when a mouse button is pressed on the menu and then dragged.
			''' Delegates the mouse event to the MenuSelectionManager.
			''' </summary>
			''' <param name="e"> the mouse event </param>
			''' <seealso cref= java.awt.event.MouseMotionListener#mouseDragged </seealso>
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				Dim menu As JMenu = CType(outerInstance.menuItem, JMenu)
				If Not menu.enabled Then Return
				MenuSelectionManager.defaultManager().processMouseEvent(e)
			End Sub
			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub


			'
			' MenuDragHandler
			'
			Public Overridable Sub menuDragMouseEntered(ByVal e As MenuDragMouseEvent)
			End Sub
			Public Overridable Sub menuDragMouseDragged(ByVal e As MenuDragMouseEvent)
				If outerInstance.menuItem.enabled = False Then Return

				Dim manager As MenuSelectionManager = e.menuSelectionManager
				Dim path As MenuElement() = e.path

				Dim p As Point = e.point
				If p.x >= 0 AndAlso p.x < outerInstance.menuItem.width AndAlso p.y >= 0 AndAlso p.y < outerInstance.menuItem.height Then
					Dim menu As JMenu = CType(outerInstance.menuItem, JMenu)
					Dim selectedPath As MenuElement() = manager.selectedPath
					If Not(selectedPath.Length > 0 AndAlso selectedPath(selectedPath.Length-1) Is menu.popupMenu) Then
						If menu.topLevelMenu OrElse menu.delay = 0 OrElse e.iD = MouseEvent.MOUSE_DRAGGED Then
							appendPath(path, menu.popupMenu)
						Else
							manager.selectedPath = path
							outerInstance.setupPostTimer(menu)
						End If
					End If
				ElseIf e.iD = MouseEvent.MOUSE_RELEASED Then
					Dim comp As Component = manager.componentForPoint(e.component, e.point)
					If comp Is Nothing Then manager.clearSelectedPath()
				End If

			End Sub
			Public Overridable Sub menuDragMouseExited(ByVal e As MenuDragMouseEvent)
			End Sub
			Public Overridable Sub menuDragMouseReleased(ByVal e As MenuDragMouseEvent)
			End Sub

			'
			' MenuKeyListener
			'
			''' <summary>
			''' Open the Menu
			''' </summary>
			Public Overridable Sub menuKeyTyped(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyTyped
				If (Not crossMenuMnemonic) AndAlso BasicPopupMenuUI.lastPopup IsNot Nothing Then Return

				If BasicPopupMenuUI.popups.Count <> 0 Then Return

				Dim key As Char = Char.ToLower(CChar(outerInstance.menuItem.mnemonic))
				Dim path As MenuElement() = e.path
				If key = Char.ToLower(e.keyChar) Then
					Dim popupMenu As JPopupMenu = CType(outerInstance.menuItem, JMenu).popupMenu
					Dim newList As New ArrayList(java.util.Arrays.asList(path))
					newList.Add(popupMenu)
					Dim subs As MenuElement() = popupMenu.subElements
					Dim [sub] As MenuElement = BasicPopupMenuUI.findEnabledChild(subs, -1, True)
					If [sub] IsNot Nothing Then newList.Add([sub])
					Dim manager As MenuSelectionManager = e.menuSelectionManager
					Dim newPath As MenuElement() = New MenuElement(){}
					newPath = CType(newList.ToArray(newPath), MenuElement())
					manager.selectedPath = newPath
					e.consume()
				End If
			End Sub

			Public Overridable Sub menuKeyPressed(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyPressed
			End Sub
			Public Overridable Sub menuKeyReleased(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyReleased
			End Sub
		End Class
	End Class

End Namespace