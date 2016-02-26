Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf

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
	''' A default L&amp;F implementation of MenuBarUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' </summary>
	Public Class BasicMenuBarUI
		Inherits MenuBarUI

		Protected Friend menuBar As JMenuBar = Nothing
		Protected Friend containerListener As ContainerListener
		Protected Friend changeListener As ChangeListener
		Private handler As Handler

		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New BasicMenuBarUI
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.TAKE_FOCUS))
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			menuBar = CType(c, JMenuBar)

			installDefaults()
			installListeners()
			installKeyboardActions()

		End Sub

		Protected Friend Overridable Sub installDefaults()
			If menuBar.layout Is Nothing OrElse TypeOf menuBar.layout Is UIResource Then menuBar.layout = New DefaultMenuLayout(menuBar,BoxLayout.LINE_AXIS)

			LookAndFeel.installProperty(menuBar, "opaque", Boolean.TRUE)
			LookAndFeel.installBorder(menuBar,"MenuBar.border")
			LookAndFeel.installColorsAndFont(menuBar, "MenuBar.background", "MenuBar.foreground", "MenuBar.font")
		End Sub

		Protected Friend Overridable Sub installListeners()
			containerListener = createContainerListener()
			changeListener = createChangeListener()

			For i As Integer = 0 To menuBar.menuCount - 1
				Dim menu As JMenu = menuBar.getMenu(i)
				If menu IsNot Nothing Then menu.model.addChangeListener(changeListener)
			Next i
			menuBar.addContainerListener(containerListener)
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)

			SwingUtilities.replaceUIInputMap(menuBar, JComponent.WHEN_IN_FOCUSED_WINDOW, ___inputMap)

			LazyActionMap.installLazyActionMap(menuBar, GetType(BasicMenuBarUI), "MenuBar.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then
				Dim bindings As Object() = CType(sun.swing.DefaultLookup.get(menuBar, Me, "MenuBar.windowBindings"), Object())
				If bindings IsNot Nothing Then Return LookAndFeel.makeComponentInputMap(menuBar, bindings)
			End If
			Return Nothing
		End Function

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallListeners()
			uninstallKeyboardActions()

			menuBar = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			If menuBar IsNot Nothing Then LookAndFeel.uninstallBorder(menuBar)
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			menuBar.removeContainerListener(containerListener)

			For i As Integer = 0 To menuBar.menuCount - 1
				Dim menu As JMenu = menuBar.getMenu(i)
				If menu IsNot Nothing Then menu.model.removeChangeListener(changeListener)
			Next i

			containerListener = Nothing
			changeListener = Nothing
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(menuBar, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(menuBar, Nothing)
		End Sub

		Protected Friend Overridable Function createContainerListener() As ContainerListener
			Return handler
		End Function

		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property


		Public Overridable Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Return Nothing
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Return Nothing
		End Function

		Private Class Handler
			Implements ChangeListener, ContainerListener

			Private ReadOnly outerInstance As BasicMenuBarUI

			Public Sub New(ByVal outerInstance As BasicMenuBarUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' ChangeListener
			'
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim i, c As Integer
				i=0
				c = outerInstance.menuBar.menuCount
				Do While i < c
					Dim menu As JMenu = outerInstance.menuBar.getMenu(i)
					If menu IsNot Nothing AndAlso menu.selected Then
						outerInstance.menuBar.selectionModel.selectedIndex = i
						Exit Do
					End If
					i += 1
				Loop
			End Sub

			'
			' ContainerListener
			'
			Public Overridable Sub componentAdded(ByVal e As ContainerEvent)
				Dim c As java.awt.Component = e.child
				If TypeOf c Is JMenu Then CType(c, JMenu).model.addChangeListener(outerInstance.changeListener)
			End Sub
			Public Overridable Sub componentRemoved(ByVal e As ContainerEvent)
				Dim c As java.awt.Component = e.child
				If TypeOf c Is JMenu Then CType(c, JMenu).model.removeChangeListener(outerInstance.changeListener)
			End Sub
		End Class


		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const TAKE_FOCUS As String = "takeFocus"

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				' TAKE_FOCUS
				Dim menuBar As JMenuBar = CType(e.source, JMenuBar)
				Dim defaultManager As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim [me] As MenuElement()
				Dim subElements As MenuElement()
				Dim menu As JMenu = menuBar.getMenu(0)
				If menu IsNot Nothing Then
						[me] = New MenuElement(2){}
						[me](0) = CType(menuBar, MenuElement)
						[me](1) = CType(menu, MenuElement)
						[me](2) = CType(menu.popupMenu, MenuElement)
						defaultManager.selectedPath = [me]
				End If
			End Sub
		End Class
	End Class

End Namespace