Imports System
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
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
	''' A Windows L&amp;F implementation of PopupMenuUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' </summary>
	Public Class BasicPopupMenuUI
		Inherits PopupMenuUI

		Friend Shared ReadOnly MOUSE_GRABBER_KEY As New StringBuilder("javax.swing.plaf.basic.BasicPopupMenuUI.MouseGrabber")
		Friend Shared ReadOnly MENU_KEYBOARD_HELPER_KEY As New StringBuilder("javax.swing.plaf.basic.BasicPopupMenuUI.MenuKeyboardHelper")

		Protected Friend popupMenu As JPopupMenu = Nothing
		<NonSerialized> _
		Private popupMenuListener As PopupMenuListener = Nothing
		Private menuKeyListener As MenuKeyListener = Nothing

		Private Shared checkedUnpostPopup As Boolean
		Private Shared unpostPopup As Boolean

		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New BasicPopupMenuUI
		End Function

		Public Sub New()
			BasicLookAndFeel.needsEventHelper = True
			Dim laf As LookAndFeel = UIManager.lookAndFeel
			If TypeOf laf Is BasicLookAndFeel Then CType(laf, BasicLookAndFeel).installAWTEventListener()
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			popupMenu = CType(c, JPopupMenu)

			installDefaults()
			installListeners()
			installKeyboardActions()
		End Sub

		Public Overridable Sub installDefaults()
			If popupMenu.layout Is Nothing OrElse TypeOf popupMenu.layout Is UIResource Then popupMenu.layout = New DefaultMenuLayout(popupMenu, BoxLayout.Y_AXIS)

			LookAndFeel.installProperty(popupMenu, "opaque", Boolean.TRUE)
			LookAndFeel.installBorder(popupMenu, "PopupMenu.border")
			LookAndFeel.installColorsAndFont(popupMenu, "PopupMenu.background", "PopupMenu.foreground", "PopupMenu.font")
		End Sub

		Protected Friend Overridable Sub installListeners()
			If popupMenuListener Is Nothing Then popupMenuListener = New BasicPopupMenuListener(Me)
			popupMenu.addPopupMenuListener(popupMenuListener)

			If menuKeyListener Is Nothing Then menuKeyListener = New BasicMenuKeyListener(Me)
			popupMenu.addMenuKeyListener(menuKeyListener)

			Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
			SyncLock MOUSE_GRABBER_KEY
				Dim mouseGrabber As MouseGrabber = CType(context.get(MOUSE_GRABBER_KEY), MouseGrabber)
				If mouseGrabber Is Nothing Then
					mouseGrabber = New MouseGrabber
					context.put(MOUSE_GRABBER_KEY, mouseGrabber)
				End If
			End SyncLock
			SyncLock MENU_KEYBOARD_HELPER_KEY
				Dim helper As MenuKeyboardHelper = CType(context.get(MENU_KEYBOARD_HELPER_KEY), MenuKeyboardHelper)
				If helper Is Nothing Then
					helper = New MenuKeyboardHelper
					context.put(MENU_KEYBOARD_HELPER_KEY, helper)
					Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
					msm.addChangeListener(helper)
				End If
			End SyncLock
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
		End Sub

		Friend Shared Function getInputMap(ByVal popup As JPopupMenu, ByVal c As JComponent) As InputMap
			Dim windowInputMap As InputMap = Nothing
			Dim bindings As Object() = CType(UIManager.get("PopupMenu.selectedWindowInputMapBindings"), Object())
			If bindings IsNot Nothing Then
				windowInputMap = LookAndFeel.makeComponentInputMap(c, bindings)
				If Not popup.componentOrientation.leftToRight Then
					Dim km As Object() = CType(UIManager.get("PopupMenu.selectedWindowInputMapBindings.RightToLeft"), Object())
					If km IsNot Nothing Then
						Dim rightToLeftInputMap As InputMap = LookAndFeel.makeComponentInputMap(c, km)
						rightToLeftInputMap.parent = windowInputMap
						windowInputMap = rightToLeftInputMap
					End If
				End If
			End If
			Return windowInputMap
		End Function

		Friend Property Shared actionMap As ActionMap
			Get
				Return LazyActionMap.getActionMap(GetType(BasicPopupMenuUI), "PopupMenu.actionMap")
			End Get
		End Property

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.CANCEL))
			map.put(New Actions(Actions.SELECT_NEXT))
			map.put(New Actions(Actions.SELECT_PREVIOUS))
			map.put(New Actions(Actions.SELECT_PARENT))
			map.put(New Actions(Actions.SELECT_CHILD))
			map.put(New Actions(Actions.RETURN))
			BasicLookAndFeel.installAudioActionMap(map)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallListeners()
			uninstallKeyboardActions()

			popupMenu = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(popupMenu)
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If popupMenuListener IsNot Nothing Then popupMenu.removePopupMenuListener(popupMenuListener)
			If menuKeyListener IsNot Nothing Then popupMenu.removeMenuKeyListener(menuKeyListener)
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(popupMenu, Nothing)
			SwingUtilities.replaceUIInputMap(popupMenu, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
		End Sub

		Friend Property Shared firstPopup As MenuElement
			Get
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim p As MenuElement() = msm.selectedPath
				Dim [me] As MenuElement = Nothing
    
				Dim i As Integer = 0
				Do While [me] Is Nothing AndAlso i < p.Length
					If TypeOf p(i) Is JPopupMenu Then [me] = p(i)
					i += 1
				Loop
    
				Return [me]
			End Get
		End Property

		Friend Property Shared lastPopup As JPopupMenu
			Get
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim p As MenuElement() = msm.selectedPath
				Dim ___popup As JPopupMenu = Nothing
    
				Dim i As Integer = p.Length - 1
				Do While ___popup Is Nothing AndAlso i >= 0
					If TypeOf p(i) Is JPopupMenu Then ___popup = CType(p(i), JPopupMenu)
					i -= 1
				Loop
				Return ___popup
			End Get
		End Property

		Friend Property Shared popups As IList(Of JPopupMenu)
			Get
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim p As MenuElement() = msm.selectedPath
    
				Dim list As IList(Of JPopupMenu) = New List(Of JPopupMenu)(p.Length)
				For Each element As MenuElement In p
					If TypeOf element Is JPopupMenu Then list.Add(CType(element, JPopupMenu))
				Next element
				Return list
			End Get
		End Property

		Public Overridable Function isPopupTrigger(ByVal e As MouseEvent) As Boolean
			Return ((e.iD=MouseEvent.MOUSE_RELEASED) AndAlso ((e.modifiers And MouseEvent.BUTTON3_MASK)<>0))
		End Function

		Private Shared Function checkInvokerEqual(ByVal present As MenuElement, ByVal last As MenuElement) As Boolean
			Dim invokerPresent As java.awt.Component = present.component
			Dim invokerLast As java.awt.Component = last.component

			If TypeOf invokerPresent Is JPopupMenu Then invokerPresent = CType(invokerPresent, JPopupMenu).invoker
			If TypeOf invokerLast Is JPopupMenu Then invokerLast = CType(invokerLast, JPopupMenu).invoker
			Return (invokerPresent Is invokerLast)
		End Function


		''' <summary>
		''' This Listener fires the Action that provides the correct auditory
		''' feedback.
		''' 
		''' @since 1.4
		''' </summary>
		Private Class BasicPopupMenuListener
			Implements PopupMenuListener

			Private ReadOnly outerInstance As BasicPopupMenuUI

			Public Sub New(ByVal outerInstance As BasicPopupMenuUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub popupMenuCanceled(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuCanceled
			End Sub

			Public Overridable Sub popupMenuWillBecomeInvisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeInvisible
			End Sub

			Public Overridable Sub popupMenuWillBecomeVisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeVisible
				BasicLookAndFeel.playSound(CType(e.source, JPopupMenu), "PopupMenu.popupSound")
			End Sub
		End Class

		''' <summary>
		''' Handles mnemonic for children JMenuItems.
		''' @since 1.5
		''' </summary>
		Private Class BasicMenuKeyListener
			Implements MenuKeyListener

			Private ReadOnly outerInstance As BasicPopupMenuUI

			Public Sub New(ByVal outerInstance As BasicPopupMenuUI)
				Me.outerInstance = outerInstance
			End Sub

			Friend menuToOpen As MenuElement = Nothing

			Public Overridable Sub menuKeyTyped(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyTyped
				If menuToOpen IsNot Nothing Then
					' we have a submenu to open
					Dim subpopup As JPopupMenu = CType(menuToOpen, JMenu).popupMenu
					Dim subitem As MenuElement = findEnabledChild(subpopup.subElements, -1, True)

					Dim lst As New List(Of MenuElement)(e.path)
					lst.Add(menuToOpen)
					lst.Add(subpopup)
					If subitem IsNot Nothing Then lst.Add(subitem)
					Dim newPath As MenuElement() = New MenuElement(){}
					newPath = lst.ToArray(newPath)
					MenuSelectionManager.defaultManager().selectedPath = newPath
					e.consume()
				End If
				menuToOpen = Nothing
			End Sub

			Public Overridable Sub menuKeyPressed(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyPressed
				Dim keyChar As Char = e.keyChar

				' Handle the case for Escape or Enter...
				If Not Char.IsLetterOrDigit(keyChar) Then Return

				Dim manager As MenuSelectionManager = e.menuSelectionManager
				Dim path As MenuElement() = e.path
				Dim items As MenuElement() = outerInstance.popupMenu.subElements
				Dim currentIndex As Integer = -1
				Dim matches As Integer = 0
				Dim firstMatch As Integer = -1
				Dim indexes As Integer() = Nothing

				For j As Integer = 0 To items.Length - 1
					If Not(TypeOf items(j) Is JMenuItem) Then Continue For
					Dim item As JMenuItem = CType(items(j), JMenuItem)
					Dim mnemonic As Integer = item.mnemonic
					If item.enabled AndAlso item.visible AndAlso lower(keyChar) = lower(mnemonic) Then
						If matches = 0 Then
							firstMatch = j
							matches += 1
						Else
							If indexes Is Nothing Then
								indexes = New Integer(items.Length - 1){}
								indexes(0) = firstMatch
							End If
							indexes(matches) = j
							matches += 1
						End If
					End If
					If item.armed OrElse item.selected Then currentIndex = matches - 1
				Next j

				If matches = 0 Then
					' no op
				ElseIf matches = 1 Then
					' Invoke the menu action
					Dim item As JMenuItem = CType(items(firstMatch), JMenuItem)
					If TypeOf item Is JMenu Then
						' submenus are handled in menuKeyTyped
						menuToOpen = item
					ElseIf item.enabled Then
						' we have a menu item
						manager.clearSelectedPath()
						item.doClick()
					End If
					e.consume()
				Else
					' Select the menu item with the matching mnemonic. If
					' the same mnemonic has been invoked then select the next
					' menu item in the cycle.
					Dim newItem As MenuElement

					newItem = items(indexes((currentIndex + 1) Mod matches))

					Dim newPath As MenuElement() = New MenuElement(path.Length){}
					Array.Copy(path, 0, newPath, 0, path.Length)
					newPath(path.Length) = newItem
					manager.selectedPath = newPath
					e.consume()
				End If
			End Sub

			Public Overridable Sub menuKeyReleased(ByVal e As MenuKeyEvent) Implements MenuKeyListener.menuKeyReleased
			End Sub

			Private Function lower(ByVal keyChar As Char) As Char
				Return Char.ToLower(keyChar)
			End Function

			Private Function lower(ByVal mnemonic As Integer) As Char
				Return Char.ToLower(ChrW(mnemonic))
			End Function
		End Class

		Private Class Actions
			Inherits sun.swing.UIAction

			' Types of actions
			Private Const ___CANCEL As String = "cancel"
			Private Const SELECT_NEXT As String = "selectNext"
			Private Const SELECT_PREVIOUS As String = "selectPrevious"
			Private Const SELECT_PARENT As String = "selectParent"
			Private Const SELECT_CHILD As String = "selectChild"
			Private Const [RETURN] As String = "return"

			' Used for next/previous actions
			Private Const FORWARD As Boolean = True
			Private Const BACKWARD As Boolean = False

			' Used for parent/child actions
			Private Const PARENT As Boolean = False
			Private Const CHILD As Boolean = True


			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim key As String = name
				If key = ___CANCEL Then
					cancel()
				ElseIf key = SELECT_NEXT Then
					selectItem(FORWARD)
				ElseIf key = SELECT_PREVIOUS Then
					selectItem(BACKWARD)
				ElseIf key = SELECT_PARENT Then
					selectParentChild(PARENT)
				ElseIf key = SELECT_CHILD Then
					selectParentChild(CHILD)
				ElseIf key = [RETURN] Then
					doReturn()
				End If
			End Sub

			Private Sub doReturn()
				Dim fmgr As java.awt.KeyboardFocusManager = java.awt.KeyboardFocusManager.currentKeyboardFocusManager
				Dim focusOwner As java.awt.Component = fmgr.focusOwner
				If focusOwner IsNot Nothing AndAlso Not(TypeOf focusOwner Is JRootPane) Then Return

				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim path As MenuElement() = msm.selectedPath
				Dim lastElement As MenuElement
				If path.Length > 0 Then
					lastElement = path(path.Length-1)
					If TypeOf lastElement Is JMenu Then
						Dim newPath As MenuElement() = New MenuElement(path.Length){}
						Array.Copy(path,0,newPath,0,path.Length)
						newPath(path.Length) = CType(lastElement, JMenu).popupMenu
						msm.selectedPath = newPath
					ElseIf TypeOf lastElement Is JMenuItem Then
						Dim mi As JMenuItem = CType(lastElement, JMenuItem)

						If TypeOf mi.uI Is BasicMenuItemUI Then
							CType(mi.uI, BasicMenuItemUI).doClick(msm)
						Else
							msm.clearSelectedPath()
							mi.doClick(0)
						End If
					End If
				End If
			End Sub
			Private Sub selectParentChild(ByVal direction As Boolean)
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim path As MenuElement() = msm.selectedPath
				Dim len As Integer = path.Length

				If direction = PARENT Then
					' selecting parent
					Dim popupIndex As Integer = len-1

					popupIndex -= 1
					If len > 2 AndAlso (TypeOf path(popupIndex) Is JPopupMenu OrElse TypeOf path(popupIndex) Is JPopupMenu) AndAlso (Not CType(path(popupIndex-1), JMenu).topLevelMenu) Then
						' check if we have an open submenu. A submenu item may or
						' may not be selected, so submenu popup can be either the
						' last or next to the last item.

						' we have a submenu, just close it
						Dim newPath As MenuElement() = New MenuElement(popupIndex - 1){}
						Array.Copy(path, 0, newPath, 0, popupIndex)
						msm.selectedPath = newPath
						Return
					End If
				Else
					' selecting child
					If len > 0 AndAlso TypeOf path(len-1) Is JMenu AndAlso (Not CType(path(len-1), JMenu).topLevelMenu) Then

						' we have a submenu, open it
						Dim menu As JMenu = CType(path(len-1), JMenu)
						Dim popup As JPopupMenu = menu.popupMenu
						Dim subs As MenuElement() = popup.subElements
						Dim item As MenuElement = findEnabledChild(subs, -1, True)
						Dim newPath As MenuElement()

						If item Is Nothing Then
							newPath = New MenuElement(len){}
						Else
							newPath = New MenuElement(len+2 - 1){}
							newPath(len+1) = item
						End If
						Array.Copy(path, 0, newPath, 0, len)
						newPath(len) = popup
						msm.selectedPath = newPath
						Return
					End If
				End If

				' check if we have a toplevel menu selected.
				' If this is the case, we select another toplevel menu
				If len > 1 AndAlso TypeOf path(0) Is JMenuBar Then
					Dim currentMenu As MenuElement = path(1)
					Dim nextMenu As MenuElement = findEnabledChild(path(0).subElements, currentMenu, direction)

					If nextMenu IsNot Nothing AndAlso nextMenu IsNot currentMenu Then
						Dim newSelection As MenuElement()
						If len = 2 Then
							' menu is selected but its popup not shown
							newSelection = New MenuElement(1){}
							newSelection(0) = path(0)
							newSelection(1) = nextMenu
						Else
							' menu is selected and its popup is shown
							newSelection = New MenuElement(2){}
							newSelection(0) = path(0)
							newSelection(1) = nextMenu
							newSelection(2) = CType(nextMenu, JMenu).popupMenu
						End If
						msm.selectedPath = newSelection
					End If
				End If
			End Sub

			Private Sub selectItem(ByVal direction As Boolean)
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim path As MenuElement() = msm.selectedPath
				If path.Length = 0 Then Return
				Dim len As Integer = path.Length
				If len = 1 AndAlso TypeOf path(0) Is JPopupMenu Then

					Dim popup As JPopupMenu = CType(path(0), JPopupMenu)
					Dim newPath As MenuElement() = New MenuElement(1){}
					newPath(0) = popup
					newPath(1) = findEnabledChild(popup.subElements, -1, direction)
					msm.selectedPath = newPath
				ElseIf len = 2 AndAlso TypeOf path(0) Is JMenuBar AndAlso TypeOf path(1) Is JMenu Then

					' a toplevel menu is selected, but its popup not shown.
					' Show the popup and select the first item
					Dim popup As JPopupMenu = CType(path(1), JMenu).popupMenu
					Dim [next] As MenuElement = findEnabledChild(popup.subElements, -1, FORWARD)
					Dim newPath As MenuElement()

					If [next] IsNot Nothing Then
						' an enabled item found -- include it in newPath
						newPath = New MenuElement(3){}
						newPath(3) = [next]
					Else
						' menu has no enabled items -- still must show the popup
						newPath = New MenuElement(2){}
					End If
					Array.Copy(path, 0, newPath, 0, 2)
					newPath(2) = popup
					msm.selectedPath = newPath

				ElseIf TypeOf path(len-1) Is JPopupMenu AndAlso TypeOf path(len-2) Is JMenu Then

					' a menu (not necessarily toplevel) is open and its popup
					' shown. Select the appropriate menu item
					Dim menu As JMenu = CType(path(len-2), JMenu)
					Dim popup As JPopupMenu = menu.popupMenu
					Dim [next] As MenuElement = findEnabledChild(popup.subElements, -1, direction)

					If [next] IsNot Nothing Then
						Dim newPath As MenuElement() = New MenuElement(len){}
						Array.Copy(path, 0, newPath, 0, len)
						newPath(len) = [next]
						msm.selectedPath = newPath
					Else
						' all items in the popup are disabled.
						' We're going to find the parent popup menu and select
						' its next item. If there's no parent popup menu (i.e.
						' current menu is toplevel), do nothing
						If len > 2 AndAlso TypeOf path(len-3) Is JPopupMenu Then
							popup = (CType(path(len-3), JPopupMenu))
							[next] = findEnabledChild(popup.subElements, menu, direction)

							If [next] IsNot Nothing AndAlso [next] IsNot menu Then
								Dim newPath As MenuElement() = New MenuElement(len-2){}
								Array.Copy(path, 0, newPath, 0, len-2)
								newPath(len-2) = [next]
								msm.selectedPath = newPath
							End If
						End If
					End If

				Else
					' just select the next item, no path expansion needed
					Dim subs As MenuElement() = path(len-2).subElements
					Dim nextChild As MenuElement = findEnabledChild(subs, path(len-1), direction)
					If nextChild Is Nothing Then nextChild = findEnabledChild(subs, -1, direction)
					If nextChild IsNot Nothing Then
						path(len-1) = nextChild
						msm.selectedPath = path
					End If
				End If
			End Sub

			Private Sub cancel()
				' 4234793: This action should call JPopupMenu.firePopupMenuCanceled but it's
				' a protected method. The real solution could be to make
				' firePopupMenuCanceled public and call it directly.
				Dim lastPopup As JPopupMenu = lastPopup
				If lastPopup IsNot Nothing Then lastPopup.putClientProperty("JPopupMenu.firePopupMenuCanceled", Boolean.TRUE)
				Dim mode As String = UIManager.getString("Menu.cancelMode")
				If "hideMenuTree".Equals(mode) Then
					MenuSelectionManager.defaultManager().clearSelectedPath()
				Else
					shortenSelectedPath()
				End If
			End Sub

			Private Sub shortenSelectedPath()
				Dim path As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
				If path.Length <= 2 Then
					MenuSelectionManager.defaultManager().clearSelectedPath()
					Return
				End If
				' unselect MenuItem and its Popup by default
				Dim value As Integer = 2
				Dim lastElement As MenuElement = path(path.Length - 1)
				Dim lastPopup As JPopupMenu = lastPopup
				If lastElement Is lastPopup Then
					Dim previousElement As MenuElement = path(path.Length - 2)
					If TypeOf previousElement Is JMenu Then
						Dim lastMenu As JMenu = CType(previousElement, JMenu)
						If lastMenu.enabled AndAlso lastPopup.componentCount > 0 Then
							' unselect the last visible popup only
							value = 1
						Else
							' unselect invisible popup and two visible elements
							value = 3
						End If
					End If
				End If
				If path.Length - value <= 2 AndAlso (Not UIManager.getBoolean("Menu.preserveTopLevelSelection")) Then value = path.Length
				Dim newPath As MenuElement() = New MenuElement(path.Length - value - 1){}
				Array.Copy(path, 0, newPath, 0, path.Length - value)
				MenuSelectionManager.defaultManager().selectedPath = newPath
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private static MenuElement nextEnabledChild(MenuElement e() , int fromIndex, int toIndex)
			For i As Integer = fromIndex To toIndex
				If e(i) IsNot Nothing Then
					Dim comp As java.awt.Component = e(i).component
					If comp IsNot Nothing AndAlso (comp.enabled OrElse UIManager.getBoolean("MenuItem.disabledAreNavigable")) AndAlso comp.visible Then Return e(i)
				End If
			Next i
			Return Nothing

		private static MenuElement previousEnabledChild(MenuElement e() , Integer fromIndex, Integer toIndex)
			For i As Integer = fromIndex To toIndex Step -1
				If e(i) IsNot Nothing Then
					Dim comp As java.awt.Component = e(i).component
					If comp IsNot Nothing AndAlso (comp.enabled OrElse UIManager.getBoolean("MenuItem.disabledAreNavigable")) AndAlso comp.visible Then Return e(i)
				End If
			Next i
			Return Nothing

		static MenuElement findEnabledChild(MenuElement e() , Integer fromIndex, Boolean forward)
			Dim result As MenuElement
			If forward Then
				result = nextEnabledChild(e, fromIndex+1, e.length-1)
				If result Is Nothing Then result = nextEnabledChild(e, 0, fromIndex-1)
			Else
				result = previousEnabledChild(e, fromIndex-1, 0)
				If result Is Nothing Then result = previousEnabledChild(e, e.length-1, fromIndex+1)
			End If
			Return result

		static MenuElement findEnabledChild(MenuElement e() , MenuElement elem, Boolean forward)
			For i As Integer = 0 To e.length - 1
				If e(i) = elem Then Return findEnabledChild(e, i, forward)
			Next i
			Return Nothing

		static class MouseGrabber implements ChangeListener, AWTEventListener, ComponentListener, WindowListener

			Dim grabbedWindow As java.awt.Window
			Dim lastPathSelected As MenuElement()

			public MouseGrabber()
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				msm.addChangeListener(Me)
				Me.lastPathSelected = msm.selectedPath
				If Me.lastPathSelected.length <> 0 Then grabWindow(Me.lastPathSelected)

			void uninstall()
				SyncLock MOUSE_GRABBER_KEY
					MenuSelectionManager.defaultManager().removeChangeListener(Me)
					ungrabWindow()
					sun.awt.AppContext.appContext.remove(MOUSE_GRABBER_KEY)
				End SyncLock

			void grabWindow(MenuElement() newPath)
				' A grab needs to be added
				Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'			{
	'					public Object run()
	'					{
	'						tk.addAWTEventListener(MouseGrabber.this, AWTEvent.MOUSE_EVENT_MASK | AWTEvent.MOUSE_MOTION_EVENT_MASK | AWTEvent.MOUSE_WHEEL_EVENT_MASK | AWTEvent.WINDOW_EVENT_MASK | sun.awt.SunToolkit.GRAB_EVENT_MASK);
	'						Return Nothing;
	'					}
	'				}
			   )

				Dim invoker As java.awt.Component = newPath(0).component
				If TypeOf invoker Is JPopupMenu Then invoker = CType(invoker, JPopupMenu).invoker
				grabbedWindow = If(TypeOf invoker Is java.awt.Window, CType(invoker, java.awt.Window), SwingUtilities.getWindowAncestor(invoker))
				If grabbedWindow IsNot Nothing Then
					If TypeOf tk Is sun.awt.SunToolkit Then
						CType(tk, sun.awt.SunToolkit).grab(grabbedWindow)
					Else
						grabbedWindow.addComponentListener(Me)
						grabbedWindow.addWindowListener(Me)
					End If
				End If

			void ungrabWindow()
				Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
				' The grab should be removed
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				 java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Object>()
	'			 {
	'					public Object run()
	'					{
	'						tk.removeAWTEventListener(MouseGrabber.this);
	'						Return Nothing;
	'					}
	'				}
			   )
				realUngrabWindow()

			void realUngrabWindow()
				Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
				If grabbedWindow IsNot Nothing Then
					If TypeOf tk Is sun.awt.SunToolkit Then
						CType(tk, sun.awt.SunToolkit).ungrab(grabbedWindow)
					Else
						grabbedWindow.removeComponentListener(Me)
						grabbedWindow.removeWindowListener(Me)
					End If
					grabbedWindow = Nothing
				End If

			public void stateChanged(ChangeEvent e)
				Dim msm As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim p As MenuElement() = msm.selectedPath

				If lastPathSelected.Length = 0 AndAlso p.Length <> 0 Then grabWindow(p)

				If lastPathSelected.Length <> 0 AndAlso p.Length = 0 Then ungrabWindow()

				lastPathSelected = p

			public void eventDispatched(java.awt.AWTEvent ev)
				If TypeOf ev Is sun.awt.UngrabEvent Then
					' Popup should be canceled in case of ungrab event
					cancelPopupMenu()
					Return
				End If
				If Not(TypeOf ev Is MouseEvent) Then Return
				Dim [me] As MouseEvent = CType(ev, MouseEvent)
				Dim src As java.awt.Component = [me].component
				Select Case [me].iD
				Case MouseEvent.MOUSE_PRESSED
					If isInPopup(src) OrElse (TypeOf src Is JMenu AndAlso CType(src, JMenu).selected) Then Return
					If Not(TypeOf src Is JComponent) OrElse Not(CType(src, JComponent).getClientProperty("doNotCancelPopup") Is BasicComboBoxUI.HIDE_POPUP_KEY) Then
						' Cancel popup only if this property was not set.
						' If this property is set to TRUE component wants
						' to deal with this event by himself.
						cancelPopupMenu()
						' Ask UIManager about should we consume event that closes
						' popup. This made to match native apps behaviour.
						Dim consumeEvent As Boolean = UIManager.getBoolean("PopupMenu.consumeEventOnClose")
						' Consume the event so that normal processing stops.
						If consumeEvent AndAlso Not(TypeOf src Is MenuElement) Then [me].consume()
					End If

				Case MouseEvent.MOUSE_RELEASED
					If Not(TypeOf src Is MenuElement) Then
						' Do not forward event to MSM, let component handle it
						If isInPopup(src) Then Exit Select
					End If
					If TypeOf src Is JMenu OrElse Not(TypeOf src Is JMenuItem) Then MenuSelectionManager.defaultManager().processMouseEvent([me])
				Case MouseEvent.MOUSE_DRAGGED
					If Not(TypeOf src Is MenuElement) Then
						' For the MOUSE_DRAGGED event the src is
						' the Component in which mouse button was pressed.
						' If the src is in popupMenu,
						' do not forward event to MSM, let component handle it.
						If isInPopup(src) Then Exit Select
					End If
					MenuSelectionManager.defaultManager().processMouseEvent([me])
				Case MouseEvent.MOUSE_WHEEL
					If isInPopup(src) OrElse ((TypeOf src Is JComboBox) AndAlso CType(src, JComboBox).popupVisible) Then Return
					cancelPopupMenu()
				End Select

			Boolean isInPopup(java.awt.Component src)
				Dim c As java.awt.Component=src
				Do While c IsNot Nothing
					If TypeOf c Is java.applet.Applet OrElse TypeOf c Is java.awt.Window Then
						Exit Do
					ElseIf TypeOf c Is JPopupMenu Then
						Return True
					End If
					c=c.parent
				Loop
				Return False

			void cancelPopupMenu()
				' We should ungrab window if a user code throws
				' an unexpected runtime exception. See 6495920.
				Try
					' 4234793: This action should call firePopupMenuCanceled but it's
					' a protected method. The real solution could be to make
					' firePopupMenuCanceled public and call it directly.
					Dim ___popups As IList(Of JPopupMenu) = popups
					For Each ___popup As JPopupMenu In ___popups
						___popup.putClientProperty("JPopupMenu.firePopupMenuCanceled", Boolean.TRUE)
					Next ___popup
					MenuSelectionManager.defaultManager().clearSelectedPath()
				Catch ex As Exception
					realUngrabWindow()
					Throw ex
				Catch err As Exception
					realUngrabWindow()
					Throw err
				End Try

			public void componentResized(ComponentEvent e)
				cancelPopupMenu()
			public void componentMoved(ComponentEvent e)
				cancelPopupMenu()
			public void componentShown(ComponentEvent e)
				cancelPopupMenu()
			public void componentHidden(ComponentEvent e)
				cancelPopupMenu()
			public void windowClosing(WindowEvent e)
				cancelPopupMenu()
			public void windowClosed(WindowEvent e)
				cancelPopupMenu()
			public void windowIconified(WindowEvent e)
				cancelPopupMenu()
			public void windowDeactivated(WindowEvent e)
				cancelPopupMenu()
			public void windowOpened(WindowEvent e)
			public void windowDeiconified(WindowEvent e)
			public void windowActivated(WindowEvent e)

		''' <summary>
		''' This helper is added to MenuSelectionManager as a ChangeListener to
		''' listen to menu selection changes. When a menu is activated, it passes
		''' focus to its parent JRootPane, and installs an ActionMap/InputMap pair
		''' on that JRootPane. Those maps are necessary in order for menu
		''' navigation to work. When menu is being deactivated, it restores focus
		''' to the component that has had it before menu activation, and uninstalls
		''' the maps.
		''' This helper is also installed as a KeyListener on root pane when menu
		''' is active. It forwards key events to MenuSelectionManager for mnemonic
		''' keys handling.
		''' </summary>
		static class MenuKeyboardHelper implements ChangeListener, KeyListener

			private java.awt.Component lastFocused = Nothing
			private MenuElement() lastPathSelected = New MenuElement(){}
			private JPopupMenu lastPopup

			private JRootPane invokerRootPane
			private ActionMap menuActionMap = actionMap
			private InputMap menuInputMap
			private Boolean focusTraversalKeysEnabled

	'        
	'         * Fix for 4213634
	'         * If this is false, KEY_TYPED and KEY_RELEASED events are NOT
	'         * processed. This is needed to avoid activating a menuitem when
	'         * the menu and menuitem share the same mnemonic.
	'         
			private Boolean receivedKeyPressed = False

			void removeItems()
				If lastFocused IsNot Nothing Then
					If Not lastFocused.requestFocusInWindow() Then
						' Workarounr for 4810575.
						' If lastFocused is not in currently focused window
						' requestFocusInWindow will fail. In this case we must
						' request focus by requestFocus() if it was not
						' transferred from our popup.
						Dim cfw As java.awt.Window = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusedWindow
						If cfw IsNot Nothing AndAlso "###focusableSwingPopup###".Equals(cfw.name) Then lastFocused.requestFocus()

					End If
					lastFocused = Nothing
				End If
				If invokerRootPane IsNot Nothing Then
					invokerRootPane.removeKeyListener(Me)
					invokerRootPane.focusTraversalKeysEnabled = focusTraversalKeysEnabled
					removeUIInputMap(invokerRootPane, menuInputMap)
					removeUIActionMap(invokerRootPane, menuActionMap)
					invokerRootPane = Nothing
				End If
				receivedKeyPressed = False

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			private FocusListener rootPaneFocusListener = New FocusAdapter()
	'		{
	'				public void focusGained(FocusEvent ev)
	'				{
	'					Component opposite = ev.getOppositeComponent();
	'					if (opposite != Nothing)
	'					{
	'						lastFocused = opposite;
	'					}
	'					ev.getComponent().removeFocusListener(Me);
	'				}
	'			};

			''' <summary>
			''' Return the last JPopupMenu in <code>path</code>,
			''' or <code>null</code> if none found
			''' </summary>
			JPopupMenu getActivePopup(MenuElement() path)
				For i As Integer = path.length-1 To 0 Step -1
					Dim elem As MenuElement = path(i)
					If TypeOf elem Is JPopupMenu Then Return CType(elem, JPopupMenu)
				Next i
				Return Nothing

			void addUIInputMap(JComponent c, InputMap map)
				Dim lastNonUI As InputMap = Nothing
				Dim parent As InputMap = c.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)

				Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is UIResource)
					lastNonUI = parent
					parent = parent.parent
				Loop

				If lastNonUI Is Nothing Then
					c.inputMapMap(JComponent.WHEN_IN_FOCUSED_WINDOW, map)
				Else
					lastNonUI.parent = map
				End If
				map.parent = parent

			void addUIActionMap(JComponent c, ActionMap map)
				Dim lastNonUI As ActionMap = Nothing
				Dim parent As ActionMap = c.actionMap

				Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is UIResource)
					lastNonUI = parent
					parent = parent.parent
				Loop

				If lastNonUI Is Nothing Then
					c.actionMap = map
				Else
					lastNonUI.parent = map
				End If
				map.parent = parent

			void removeUIInputMap(JComponent c, InputMap map)
				Dim im As InputMap = Nothing
				Dim parent As InputMap = c.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)

				Do While parent IsNot Nothing
					If parent Is map Then
						If im Is Nothing Then
							c.inputMapMap(JComponent.WHEN_IN_FOCUSED_WINDOW, map.parent)
						Else
							im.parent = map.parent
						End If
						Exit Do
					End If
					im = parent
					parent = parent.parent
				Loop

			void removeUIActionMap(JComponent c, ActionMap map)
				Dim im As ActionMap = Nothing
				Dim parent As ActionMap = c.actionMap

				Do While parent IsNot Nothing
					If parent Is map Then
						If im Is Nothing Then
							c.actionMap = map.parent
						Else
							im.parent = map.parent
						End If
						Exit Do
					End If
					im = parent
					parent = parent.parent
				Loop

			public void stateChanged(ChangeEvent ev)
				If Not(TypeOf UIManager.lookAndFeel Is BasicLookAndFeel) Then
					uninstall()
					Return
				End If
				Dim msm As MenuSelectionManager = CType(ev.source, MenuSelectionManager)
				Dim p As MenuElement() = msm.selectedPath
				Dim ___popup As JPopupMenu = getActivePopup(p)
				If ___popup IsNot Nothing AndAlso (Not ___popup.focusable) Then Return

				If lastPathSelected.length <> 0 AndAlso p.Length <> 0 Then
					If Not checkInvokerEqual(p(0),lastPathSelected(0)) Then
						removeItems()
						lastPathSelected = New MenuElement(){}
					End If
				End If

				If lastPathSelected.length = 0 AndAlso p.Length > 0 Then
					' menu posted
					Dim invoker As JComponent

					If ___popup Is Nothing Then
						If p.Length = 2 AndAlso TypeOf p(0) Is JMenuBar AndAlso TypeOf p(1) Is JMenu Then
							' a menu has been selected but not open
							invoker = CType(p(1), JComponent)
							___popup = CType(invoker, JMenu).popupMenu
						Else
							Return
						End If
					Else
						Dim c As java.awt.Component = ___popup.invoker
						If TypeOf c Is JFrame Then
							invoker = CType(c, JFrame).rootPane
						ElseIf TypeOf c Is JDialog Then
							invoker = CType(c, JDialog).rootPane
						ElseIf TypeOf c Is JApplet Then
							invoker = CType(c, JApplet).rootPane
						Else
							Do While Not(TypeOf c Is JComponent)
								If c Is Nothing Then Return
								c = c.parent
							Loop
							invoker = CType(c, JComponent)
						End If
					End If

					' remember current focus owner
					lastFocused = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner

					' request focus on root pane and install keybindings
					' used for menu navigation
					invokerRootPane = SwingUtilities.getRootPane(invoker)
					If invokerRootPane IsNot Nothing Then
						invokerRootPane.addFocusListener(rootPaneFocusListener)
						invokerRootPane.requestFocus(True)
						invokerRootPane.addKeyListener(Me)
						focusTraversalKeysEnabled = invokerRootPane.focusTraversalKeysEnabled
						invokerRootPane.focusTraversalKeysEnabled = False

						menuInputMap = getInputMap(___popup, invokerRootPane)
						addUIInputMap(invokerRootPane, menuInputMap)
						addUIActionMap(invokerRootPane, menuActionMap)
					End If
				ElseIf lastPathSelected.length <> 0 AndAlso p.Length = 0 Then
					' menu hidden -- return focus to where it had been before
					' and uninstall menu keybindings
					   removeItems()
				Else
					If ___popup IsNot lastPopup Then receivedKeyPressed = False
				End If

				' Remember the last path selected
				lastPathSelected = p
				lastPopup = ___popup

			public void keyPressed(KeyEvent ev)
				receivedKeyPressed = True
				MenuSelectionManager.defaultManager().processKeyEvent(ev)

			public void keyReleased(KeyEvent ev)
				If receivedKeyPressed Then
					receivedKeyPressed = False
					MenuSelectionManager.defaultManager().processKeyEvent(ev)
				End If

			public void keyTyped(KeyEvent ev)
				If receivedKeyPressed Then MenuSelectionManager.defaultManager().processKeyEvent(ev)

			void uninstall()
				SyncLock MENU_KEYBOARD_HELPER_KEY
					MenuSelectionManager.defaultManager().removeChangeListener(Me)
					sun.awt.AppContext.appContext.remove(MENU_KEYBOARD_HELPER_KEY)
				End SyncLock
	End Class

End Namespace