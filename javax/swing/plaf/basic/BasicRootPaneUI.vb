Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Basic implementation of RootPaneUI, there is one shared between all
	''' JRootPane instances.
	''' 
	''' @author Scott Violet
	''' @since 1.3
	''' </summary>
	Public Class BasicRootPaneUI
		Inherits RootPaneUI
		Implements java.beans.PropertyChangeListener

		Private Shared rootPaneUI As RootPaneUI = New BasicRootPaneUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return rootPaneUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			installDefaults(CType(c, JRootPane))
			installComponents(CType(c, JRootPane))
			installListeners(CType(c, JRootPane))
			installKeyboardActions(CType(c, JRootPane))
		End Sub


		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults(CType(c, JRootPane))
			uninstallComponents(CType(c, JRootPane))
			uninstallListeners(CType(c, JRootPane))
			uninstallKeyboardActions(CType(c, JRootPane))
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal c As JRootPane)
			LookAndFeel.installProperty(c, "opaque", Boolean.FALSE)
		End Sub

		Protected Friend Overridable Sub installComponents(ByVal root As JRootPane)
		End Sub

		Protected Friend Overridable Sub installListeners(ByVal root As JRootPane)
			root.addPropertyChangeListener(Me)
		End Sub

		Protected Friend Overridable Sub installKeyboardActions(ByVal root As JRootPane)
			Dim km As InputMap = getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW, root)
			SwingUtilities.replaceUIInputMap(root, JComponent.WHEN_IN_FOCUSED_WINDOW, km)
			km = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, root)
			SwingUtilities.replaceUIInputMap(root, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)

			LazyActionMap.installLazyActionMap(root, GetType(BasicRootPaneUI), "RootPane.actionMap")
			updateDefaultButtonBindings(root)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal root As JRootPane)
		End Sub

		Protected Friend Overridable Sub uninstallComponents(ByVal root As JRootPane)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal root As JRootPane)
			root.removePropertyChangeListener(Me)
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions(ByVal root As JRootPane)
			SwingUtilities.replaceUIInputMap(root, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(root, Nothing)
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer, ByVal c As JComponent) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(sun.swing.DefaultLookup.get(c, Me, "RootPane.ancestorInputMap"), InputMap)

			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then Return createInputMap(condition, c)
			Return Nothing
		End Function

		Friend Overridable Function createInputMap(ByVal condition As Integer, ByVal c As JComponent) As ComponentInputMap
			Return New RootPaneInputMap(c)
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.PRESS))
			map.put(New Actions(Actions.RELEASE))
			map.put(New Actions(Actions.POST_POPUP))
		End Sub

		''' <summary>
		''' Invoked when the default button property has changed. This reloads
		''' the bindings from the defaults table with name
		''' <code>RootPane.defaultButtonWindowKeyBindings</code>.
		''' </summary>
		Friend Overridable Sub updateDefaultButtonBindings(ByVal root As JRootPane)
			Dim km As InputMap = SwingUtilities.getUIInputMap(root, JComponent.WHEN_IN_FOCUSED_WINDOW)
			Do While km IsNot Nothing AndAlso Not(TypeOf km Is RootPaneInputMap)
				km = km.parent
			Loop
			If km IsNot Nothing Then
				km.clear()
				If root.defaultButton IsNot Nothing Then
					Dim bindings As Object() = CType(sun.swing.DefaultLookup.get(root, Me, "RootPane.defaultButtonWindowKeyBindings"), Object())
					If bindings IsNot Nothing Then LookAndFeel.loadKeyBindings(km, bindings)
				End If
			End If
		End Sub

		''' <summary>
		''' Invoked when a property changes on the root pane. If the event
		''' indicates the <code>defaultButton</code> has changed, this will
		''' reinstall the keyboard actions.
		''' </summary>
		Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If e.propertyName.Equals("defaultButton") Then
				Dim rootpane As JRootPane = CType(e.source, JRootPane)
				updateDefaultButtonBindings(rootpane)
				If rootpane.getClientProperty("temporaryDefaultButton") Is Nothing Then rootpane.putClientProperty("initialDefaultButton", e.newValue)
			End If
		End Sub


		Friend Class Actions
			Inherits sun.swing.UIAction

			Public Const PRESS As String = "press"
			Public Const RELEASE As String = "release"
			Public Const POST_POPUP As String = "postPopup"

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As java.awt.event.ActionEvent)
				Dim root As JRootPane = CType(evt.source, JRootPane)
				Dim owner As JButton = root.defaultButton
				Dim key As String = name

				If key = POST_POPUP Then ' Action to post popup
					Dim c As java.awt.Component = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner

					If TypeOf c Is JComponent Then
						Dim src As JComponent = CType(c, JComponent)
						Dim jpm As JPopupMenu = src.componentPopupMenu
						If jpm IsNot Nothing Then
							Dim pt As java.awt.Point = src.getPopupLocation(Nothing)
							If pt Is Nothing Then
								Dim vis As java.awt.Rectangle = src.visibleRect
								pt = New java.awt.Point(vis.x+vis.width/2, vis.y+vis.height/2)
							End If
							jpm.show(c, pt.x, pt.y)
						End If
					End If
				ElseIf owner IsNot Nothing AndAlso SwingUtilities.getRootPane(owner) Is root Then
					If key = PRESS Then owner.doClick(20)
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				Dim key As String = name
				If key = POST_POPUP Then
					Dim elems As MenuElement() = MenuSelectionManager.defaultManager().selectedPath
					If elems IsNot Nothing AndAlso elems.Length <> 0 Then Return False

					Dim c As java.awt.Component = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
					If TypeOf c Is JComponent Then
						Dim src As JComponent = CType(c, JComponent)
						Return src.componentPopupMenu IsNot Nothing
					End If

					Return False
				End If

				If sender IsNot Nothing AndAlso TypeOf sender Is JRootPane Then
					Dim owner As JButton = CType(sender, JRootPane).defaultButton
					Return (owner IsNot Nothing AndAlso owner.model.enabled)
				End If
				Return True
			End Function
		End Class

		Private Class RootPaneInputMap
			Inherits ComponentInputMapUIResource

			Public Sub New(ByVal c As JComponent)
				MyBase.New(c)
			End Sub
		End Class
	End Class

End Namespace