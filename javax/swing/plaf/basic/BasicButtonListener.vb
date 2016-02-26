Imports javax.swing
Imports javax.swing.event

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Button Listener
	''' 
	''' @author Jeff Dinkins
	''' @author Arnaud Weber (keyboard UI support)
	''' </summary>

	Public Class BasicButtonListener
		Implements MouseListener, MouseMotionListener, FocusListener, ChangeListener, PropertyChangeListener

		Private lastPressedTimestamp As Long = -1
		Private shouldDiscardRelease As Boolean = False

		''' <summary>
		''' Populates Buttons actions.
		''' </summary>
		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.PRESS))
			map.put(New Actions(Actions.RELEASE))
		End Sub


		Public Sub New(ByVal b As AbstractButton)
		End Sub

		Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
			Dim prop As String = e.propertyName
			If prop = AbstractButton.MNEMONIC_CHANGED_PROPERTY Then
				updateMnemonicBinding(CType(e.source, AbstractButton))
			ElseIf prop = AbstractButton.CONTENT_AREA_FILLED_CHANGED_PROPERTY Then
				checkOpacity(CType(e.source, AbstractButton))
			ElseIf prop = AbstractButton.TEXT_CHANGED_PROPERTY OrElse "font" = prop OrElse "foreground" = prop Then
				Dim b As AbstractButton = CType(e.source, AbstractButton)
				BasicHTML.updateRenderer(b, b.text)
			End If
		End Sub

		Protected Friend Overridable Sub checkOpacity(ByVal b As AbstractButton)
			b.opaque = b.contentAreaFilled
		End Sub

		''' <summary>
		''' Register default key actions: pressing space to "click" a
		''' button and registring the keyboard mnemonic (if any).
		''' </summary>
		Public Overridable Sub installKeyboardActions(ByVal c As JComponent)
			Dim b As AbstractButton = CType(c, AbstractButton)
			' Update the mnemonic binding.
			updateMnemonicBinding(b)

			LazyActionMap.installLazyActionMap(c, GetType(BasicButtonListener), "Button.actionMap")

			Dim km As InputMap = getInputMap(JComponent.WHEN_FOCUSED, c)

			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_FOCUSED, km)
		End Sub

		''' <summary>
		''' Unregister's default key actions
		''' </summary>
		Public Overridable Sub uninstallKeyboardActions(ByVal c As JComponent)
			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_FOCUSED, Nothing)
			SwingUtilities.replaceUIActionMap(c, Nothing)
		End Sub

		''' <summary>
		''' Returns the InputMap for condition <code>condition</code>. Called as
		''' part of <code>installKeyboardActions</code>.
		''' </summary>
		Friend Overridable Function getInputMap(ByVal condition As Integer, ByVal c As JComponent) As InputMap
			If condition = JComponent.WHEN_FOCUSED Then
				Dim ui As BasicButtonUI = CType(BasicLookAndFeel.getUIOfType(CType(c, AbstractButton).uI, GetType(BasicButtonUI)), BasicButtonUI)
				If ui IsNot Nothing Then Return CType(sun.swing.DefaultLookup.get(c, ui, ui.propertyPrefix & "focusInputMap"), InputMap)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Resets the binding for the mnemonic in the WHEN_IN_FOCUSED_WINDOW
		''' UI InputMap.
		''' </summary>
		Friend Overridable Sub updateMnemonicBinding(ByVal b As AbstractButton)
			Dim m As Integer = b.mnemonic
			If m <> 0 Then
				Dim map As InputMap = SwingUtilities.getUIInputMap(b, JComponent.WHEN_IN_FOCUSED_WINDOW)

				If map Is Nothing Then
					map = New javax.swing.plaf.ComponentInputMapUIResource(b)
					SwingUtilities.replaceUIInputMap(b, JComponent.WHEN_IN_FOCUSED_WINDOW, map)
				End If
				map.clear()
				map.put(KeyStroke.getKeyStroke(m, BasicLookAndFeel.focusAcceleratorKeyMask, False), "pressed")
				map.put(KeyStroke.getKeyStroke(m, BasicLookAndFeel.focusAcceleratorKeyMask, True), "released")
				map.put(KeyStroke.getKeyStroke(m, 0, True), "released")
			Else
				Dim map As InputMap = SwingUtilities.getUIInputMap(b, JComponent.WHEN_IN_FOCUSED_WINDOW)
				If map IsNot Nothing Then map.clear()
			End If
		End Sub

		Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
			Dim b As AbstractButton = CType(e.source, AbstractButton)
			b.repaint()
		End Sub

		Public Overridable Sub focusGained(ByVal e As FocusEvent)
			Dim b As AbstractButton = CType(e.source, AbstractButton)
			If TypeOf b Is JButton AndAlso CType(b, JButton).defaultCapable Then
				Dim root As JRootPane = b.rootPane
				If root IsNot Nothing Then
				   Dim ui As BasicButtonUI = CType(BasicLookAndFeel.getUIOfType(b.uI, GetType(BasicButtonUI)), BasicButtonUI)
				   If ui IsNot Nothing AndAlso sun.swing.DefaultLookup.getBoolean(b, ui, ui.propertyPrefix & "defaultButtonFollowsFocus", True) Then
					   root.putClientProperty("temporaryDefaultButton", b)
					   root.defaultButton = CType(b, JButton)
					   root.putClientProperty("temporaryDefaultButton", Nothing)
				   End If
				End If
			End If
			b.repaint()
		End Sub

		Public Overridable Sub focusLost(ByVal e As FocusEvent)
			Dim b As AbstractButton = CType(e.source, AbstractButton)
			Dim root As JRootPane = b.rootPane
			If root IsNot Nothing Then
			   Dim initialDefault As JButton = CType(root.getClientProperty("initialDefaultButton"), JButton)
			   If b IsNot initialDefault Then
				   Dim ui As BasicButtonUI = CType(BasicLookAndFeel.getUIOfType(b.uI, GetType(BasicButtonUI)), BasicButtonUI)
				   If ui IsNot Nothing AndAlso sun.swing.DefaultLookup.getBoolean(b, ui, ui.propertyPrefix & "defaultButtonFollowsFocus", True) Then root.defaultButton = initialDefault
			   End If
			End If

			Dim model As ButtonModel = b.model
			model.pressed = False
			model.armed = False
			b.repaint()
		End Sub

		Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
		End Sub


		Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
		End Sub

		Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
		End Sub

		Public Overridable Sub mousePressed(ByVal e As MouseEvent)
		   If SwingUtilities.isLeftMouseButton(e) Then
			  Dim b As AbstractButton = CType(e.source, AbstractButton)

			  If b.contains(e.x, e.y) Then
				  Dim multiClickThreshhold As Long = b.multiClickThreshhold
				  Dim lastTime As Long = lastPressedTimestamp
					  lastPressedTimestamp = e.when
					  Dim currentTime As Long = lastPressedTimestamp
				  If lastTime <> -1 AndAlso currentTime - lastTime < multiClickThreshhold Then
					  shouldDiscardRelease = True
					  Return
				  End If

				 Dim model As ButtonModel = b.model
				 If Not model.enabled Then Return
				 If Not model.armed Then model.armed = True
				 model.pressed = True
				 If (Not b.hasFocus()) AndAlso b.requestFocusEnabled Then b.requestFocus()
			  End If
		   End If
		End Sub

		Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			If SwingUtilities.isLeftMouseButton(e) Then
				' Support for multiClickThreshhold
				If shouldDiscardRelease Then
					shouldDiscardRelease = False
					Return
				End If
				Dim b As AbstractButton = CType(e.source, AbstractButton)
				Dim model As ButtonModel = b.model
				model.pressed = False
				model.armed = False
			End If
		End Sub

		Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			Dim b As AbstractButton = CType(e.source, AbstractButton)
			Dim model As ButtonModel = b.model
			If b.rolloverEnabled AndAlso (Not SwingUtilities.isLeftMouseButton(e)) Then model.rollover = True
			If model.pressed Then model.armed = True
		End Sub

		Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			Dim b As AbstractButton = CType(e.source, AbstractButton)
			Dim model As ButtonModel = b.model
			If b.rolloverEnabled Then model.rollover = False
			model.armed = False
		End Sub


		''' <summary>
		''' Actions for Buttons. Two types of action are supported:
		''' pressed: Moves the button to a pressed state
		''' released: Disarms the button.
		''' </summary>
		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const PRESS As String = "pressed"
			Private Const RELEASE As String = "released"

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim b As AbstractButton = CType(e.source, AbstractButton)
				Dim key As String = name
				If key = PRESS Then
					Dim model As ButtonModel = b.model
					model.armed = True
					model.pressed = True
					If Not b.hasFocus() Then b.requestFocus()
				ElseIf key = RELEASE Then
					Dim model As ButtonModel = b.model
					model.pressed = False
					model.armed = False
				End If
			End Sub

			Public Overridable Function isEnabled(ByVal sender As Object) As Boolean
				If sender IsNot Nothing AndAlso (TypeOf sender Is AbstractButton) AndAlso (Not CType(sender, AbstractButton).model.enabled) Then
					Return False
				Else
					Return True
				End If
			End Function
		End Class
	End Class

End Namespace