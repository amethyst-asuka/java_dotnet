Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' RadioButtonUI implementation for BasicRadioButtonUI
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicRadioButtonUI
		Inherits BasicToggleButtonUI

		Private Shared ReadOnly BASIC_RADIO_BUTTON_UI_KEY As New Object

		''' <summary>
		''' The icon.
		''' </summary>
		Protected Friend icon As Icon

		Private defaults_initialized As Boolean = False

		Private Shared ReadOnly propertyPrefix As String = "RadioButton" & "."

		Private keyListener As KeyListener = Nothing

		' ********************************
		'        Create PLAF
		' ********************************

		''' <summary>
		''' Returns an instance of {@code BasicRadioButtonUI}.
		''' </summary>
		''' <param name="b"> a component </param>
		''' <returns> an instance of {@code BasicRadioButtonUI} </returns>
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim radioButtonUI As BasicRadioButtonUI = CType(appContext.get(BASIC_RADIO_BUTTON_UI_KEY), BasicRadioButtonUI)
			If radioButtonUI Is Nothing Then
				radioButtonUI = New BasicRadioButtonUI
				appContext.put(BASIC_RADIO_BUTTON_UI_KEY, radioButtonUI)
			End If
			Return radioButtonUI
		End Function

		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return propertyPrefix
			End Get
		End Property

		' ********************************
		'        Install PLAF
		' ********************************
		Protected Friend Overrides Sub installDefaults(ByVal b As AbstractButton)
			MyBase.installDefaults(b)
			If Not defaults_initialized Then
				icon = UIManager.getIcon(propertyPrefix & "icon")
				defaults_initialized = True
			End If
		End Sub

		' ********************************
		'        Uninstall PLAF
		' ********************************
		Protected Friend Overrides Sub uninstallDefaults(ByVal b As AbstractButton)
			MyBase.uninstallDefaults(b)
			defaults_initialized = False
		End Sub

		''' <summary>
		''' Returns the default icon.
		''' </summary>
		''' <returns> the default icon </returns>
		Public Overridable Property defaultIcon As Icon
			Get
				Return icon
			End Get
		End Property

		' ********************************
		'        Install Listeners
		' ********************************
		Protected Friend Overrides Sub installListeners(ByVal button As AbstractButton)
			MyBase.installListeners(button)

			' Only for JRadioButton
			If Not(TypeOf button Is JRadioButton) Then Return

			keyListener = createKeyListener()
			button.addKeyListener(keyListener)

			' Need to get traversal key event
			button.focusTraversalKeysEnabled = False

			' Map actions to the arrow keys
			button.actionMap.put("Previous", New SelectPreviousBtn(Me))
			button.actionMap.put("Next", New SelectNextBtn(Me))

			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(KeyStroke.getKeyStroke("UP"), "Previous")
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(KeyStroke.getKeyStroke("DOWN"), "Next")
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(KeyStroke.getKeyStroke("LEFT"), "Previous")
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(KeyStroke.getKeyStroke("RIGHT"), "Next")
		End Sub

		' ********************************
		'        UnInstall Listeners
		' ********************************
		Protected Friend Overrides Sub uninstallListeners(ByVal button As AbstractButton)
			MyBase.uninstallListeners(button)

			' Only for JRadioButton
			If Not(TypeOf button Is JRadioButton) Then Return

			' Unmap actions from the arrow keys
			button.actionMap.remove("Previous")
			button.actionMap.remove("Next")
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).remove(KeyStroke.getKeyStroke("UP"))
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).remove(KeyStroke.getKeyStroke("DOWN"))
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).remove(KeyStroke.getKeyStroke("LEFT"))
			button.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).remove(KeyStroke.getKeyStroke("RIGHT"))

			If keyListener IsNot Nothing Then
				button.removeKeyListener(keyListener)
				keyListener = Nothing
			End If
		End Sub

	'     These Dimensions/Rectangles are allocated once for all
	'     * RadioButtonUI.paint() calls.  Re-using rectangles
	'     * rather than allocating them in each paint call substantially
	'     * reduced the time it took paint to run.  Obviously, this
	'     * method can't be re-entered.
	'     
		Private Shared size As New Dimension
		Private Shared viewRect As New Rectangle
		Private Shared iconRect As New Rectangle
		Private Shared textRect As New Rectangle

		''' <summary>
		''' paint the radio button
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model

			Dim f As Font = c.font
			g.font = f
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g, f)

			Dim i As Insets = c.insets
			size = b.getSize(size)
			viewRect.x = i.left
			viewRect.y = i.top
			viewRect.width = size.width - (i.right + viewRect.x)
			viewRect.height = size.height - (i.bottom + viewRect.y)
				iconRect.height = 0
					iconRect.width = iconRect.height
						iconRect.y = iconRect.width
						iconRect.x = iconRect.y
				textRect.height = 0
					textRect.width = textRect.height
						textRect.y = textRect.width
						textRect.x = textRect.y

			Dim altIcon As Icon = b.icon
			Dim selectedIcon As Icon = Nothing
			Dim disabledIcon As Icon = Nothing

			Dim text As String = SwingUtilities.layoutCompoundLabel(c, fm, b.text,If(altIcon IsNot Nothing, altIcon, defaultIcon), b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, viewRect, iconRect, textRect,If(b.text Is Nothing, 0, b.iconTextGap))

			' fill background
			If c.opaque Then
				g.color = b.background
				g.fillRect(0,0, size.width, size.height)
			End If


			' Paint the radio button
			If altIcon IsNot Nothing Then

				If Not model.enabled Then
					If model.selected Then
					   altIcon = b.disabledSelectedIcon
					Else
					   altIcon = b.disabledIcon
					End If
				ElseIf model.pressed AndAlso model.armed Then
					altIcon = b.pressedIcon
					If altIcon Is Nothing Then altIcon = b.selectedIcon
				ElseIf model.selected Then
					If b.rolloverEnabled AndAlso model.rollover Then
							altIcon = b.rolloverSelectedIcon
							If altIcon Is Nothing Then altIcon = b.selectedIcon
					Else
							altIcon = b.selectedIcon
					End If
				ElseIf b.rolloverEnabled AndAlso model.rollover Then
					altIcon = b.rolloverIcon
				End If

				If altIcon Is Nothing Then altIcon = b.icon

				altIcon.paintIcon(c, g, iconRect.x, iconRect.y)

			Else
				defaultIcon.paintIcon(c, g, iconRect.x, iconRect.y)
			End If


			' Draw the Text
			If text IsNot Nothing Then
				Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
				If v IsNot Nothing Then
					v.paint(g, textRect)
				Else
					paintText(g, b, textRect, text)
				End If
				If b.hasFocus() AndAlso b.focusPainted AndAlso textRect.width > 0 AndAlso textRect.height > 0 Then paintFocus(g, textRect, size)
			End If
		End Sub

		''' <summary>
		''' Paints focused radio button.
		''' </summary>
		''' <param name="g"> an instance of {@code Graphics} </param>
		''' <param name="textRect"> bounds </param>
		''' <param name="size"> the size of radio button </param>
		Protected Friend Overridable Sub paintFocus(ByVal g As Graphics, ByVal textRect As Rectangle, ByVal size As Dimension)
		End Sub


	'     These Insets/Rectangles are allocated once for all
	'     * RadioButtonUI.getPreferredSize() calls.  Re-using rectangles
	'     * rather than allocating them in each call substantially
	'     * reduced the time it took getPreferredSize() to run.  Obviously,
	'     * this method can't be re-entered.
	'     
		Private Shared prefViewRect As New Rectangle
		Private Shared prefIconRect As New Rectangle
		Private Shared prefTextRect As New Rectangle
		Private Shared prefInsets As New Insets(0, 0, 0, 0)

		''' <summary>
		''' The preferred size of the radio button
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			If c.componentCount > 0 Then Return Nothing

			Dim b As AbstractButton = CType(c, AbstractButton)

			Dim text As String = b.text

			Dim buttonIcon As Icon = b.icon
			If buttonIcon Is Nothing Then buttonIcon = defaultIcon

			Dim font As Font = b.font
			Dim fm As FontMetrics = b.getFontMetrics(font)

				prefViewRect.y = 0
				prefViewRect.x = prefViewRect.y
			prefViewRect.width = Short.MaxValue
			prefViewRect.height = Short.MaxValue
				prefIconRect.height = 0
					prefIconRect.width = prefIconRect.height
						prefIconRect.y = prefIconRect.width
						prefIconRect.x = prefIconRect.y
				prefTextRect.height = 0
					prefTextRect.width = prefTextRect.height
						prefTextRect.y = prefTextRect.width
						prefTextRect.x = prefTextRect.y

			SwingUtilities.layoutCompoundLabel(c, fm, text, buttonIcon, b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, prefViewRect, prefIconRect, prefTextRect,If(text Is Nothing, 0, b.iconTextGap))

			' find the union of the icon and text rects (from Rectangle.java)
			Dim x1 As Integer = Math.Min(prefIconRect.x, prefTextRect.x)
			Dim x2 As Integer = Math.Max(prefIconRect.x + prefIconRect.width, prefTextRect.x + prefTextRect.width)
			Dim y1 As Integer = Math.Min(prefIconRect.y, prefTextRect.y)
			Dim y2 As Integer = Math.Max(prefIconRect.y + prefIconRect.height, prefTextRect.y + prefTextRect.height)
			Dim width As Integer = x2 - x1
			Dim height As Integer = y2 - y1

			prefInsets = b.getInsets(prefInsets)
			width += prefInsets.left + prefInsets.right
			height += prefInsets.top + prefInsets.bottom
			Return New Dimension(width, height)
		End Function

		'///////////////////////// Private functions ////////////////////////
		''' <summary>
		''' Creates the key listener to handle tab navigation in JRadioButton Group.
		''' </summary>
		Private Function createKeyListener() As KeyListener
			 If keyListener Is Nothing Then keyListener = New KeyHandler(Me)
			Return keyListener
		End Function


		Private Function isValidRadioButtonObj(ByVal obj As Object) As Boolean
			Return ((TypeOf obj Is JRadioButton) AndAlso CType(obj, JRadioButton).visible AndAlso CType(obj, JRadioButton).enabled)
		End Function

		''' <summary>
		''' Select radio button based on "Previous" or "Next" operation
		''' </summary>
		''' <param name="event">, the event object. </param>
		''' <param name="next">, indicate if it's next one </param>
		Private Sub selectRadioButton(ByVal [event] As ActionEvent, ByVal [next] As Boolean)
			' Get the source of the event.
			Dim eventSrc As Object = [event].source

			' Check whether the source is JRadioButton, it so, whether it is visible
			If Not isValidRadioButtonObj(eventSrc) Then Return

			Dim btnGroupInfo As New ButtonGroupInfo(Me, CType(eventSrc, JRadioButton))
			btnGroupInfo.selectNewButton([next])
		End Sub

		'///////////////////////// Inner Classes ////////////////////////
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class SelectPreviousBtn
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicRadioButtonUI

			Public Sub New(ByVal outerInstance As BasicRadioButtonUI)
					Me.outerInstance = outerInstance
				MyBase.New("Previous")
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
			   outerInstance.selectRadioButton(e, False)
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class SelectNextBtn
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicRadioButtonUI

			Public Sub New(ByVal outerInstance As BasicRadioButtonUI)
					Me.outerInstance = outerInstance
				MyBase.New("Next")
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.selectRadioButton(e, True)
			End Sub
		End Class

		''' <summary>
		''' ButtonGroupInfo, used to get related info in button group
		''' for given radio button
		''' </summary>
		Private Class ButtonGroupInfo
			Private ReadOnly outerInstance As BasicRadioButtonUI


			Friend activeBtn As JRadioButton = Nothing

			Friend firstBtn As JRadioButton = Nothing
			Friend lastBtn As JRadioButton = Nothing

			Friend previousBtn As JRadioButton = Nothing
			Friend nextBtn As JRadioButton = Nothing

			Friend btnsInGroup As HashSet(Of JRadioButton) = Nothing

			Friend srcFound As Boolean = False
			Public Sub New(ByVal outerInstance As BasicRadioButtonUI, ByVal btn As JRadioButton)
					Me.outerInstance = outerInstance
				activeBtn = btn
				btnsInGroup = New HashSet(Of JRadioButton)
			End Sub

			' Check if given object is in the button group
			Friend Overridable Function containsInGroup(ByVal obj As Object) As Boolean
			   Return btnsInGroup.Contains(obj)
			End Function

			' Check if the next object to gain focus belongs
			' to the button group or not
			Friend Overridable Function getFocusTransferBaseComponent(ByVal [next] As Boolean) As Component
				Dim focusBaseComp As Component = activeBtn
				Dim container As Container = focusBaseComp.focusCycleRootAncestor
				If container IsNot Nothing Then
					Dim policy As FocusTraversalPolicy = container.focusTraversalPolicy
					Dim comp As Component = If([next], policy.getComponentAfter(container, activeBtn), policy.getComponentBefore(container, activeBtn))

					' If next component in the button group, use last/first button as base focus
					' otherwise, use the activeBtn as the base focus
					If containsInGroup(comp) Then focusBaseComp = If([next], lastBtn, firstBtn)
				End If

				Return focusBaseComp
			End Function

			Friend Overridable Property buttonGroupInfo As Boolean
				Get
					If activeBtn Is Nothing Then Return False
    
					btnsInGroup.Clear()
    
					' Get the button model from the source.
					Dim model As ButtonModel = activeBtn.model
					If Not(TypeOf model Is DefaultButtonModel) Then Return False
    
					' If the button model is DefaultButtonModel, and use it, otherwise return.
					Dim bm As DefaultButtonModel = CType(model, DefaultButtonModel)
    
					' get the ButtonGroup of the button from the button model
					Dim group As ButtonGroup = bm.group
					If group Is Nothing Then Return False
    
					' Get all the buttons in the group
					Dim e As System.Collections.IEnumerator(Of AbstractButton) = group.elements
					If e Is Nothing Then Return False
    
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Do While e.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim curElement As AbstractButton = e.nextElement()
						If Not outerInstance.isValidRadioButtonObj(curElement) Then Continue Do
    
						btnsInGroup.Add(CType(curElement, JRadioButton))
    
						' If firstBtn is not set yet, curElement is that first button
						If Nothing Is firstBtn Then firstBtn = CType(curElement, JRadioButton)
    
						If activeBtn Is curElement Then
							srcFound = True
						ElseIf Not srcFound Then
							' The source has not been yet found and the current element
							' is the last previousBtn
							previousBtn = CType(curElement, JRadioButton)
						ElseIf nextBtn Is Nothing Then
							' The source has been found and the current element
							' is the next valid button of the list
							nextBtn = CType(curElement, JRadioButton)
						End If
    
						' Set new last "valid" JRadioButton of the list
						lastBtn = CType(curElement, JRadioButton)
					Loop
    
					Return True
				End Get
			End Property

			''' <summary>
			''' Find the new radio button that focus needs to be
			''' moved to in the group, select the button
			''' </summary>
			''' <param name="next">, indicate if it's arrow up/left or down/right </param>
			Friend Overridable Sub selectNewButton(ByVal [next] As Boolean)
				If Not buttonGroupInfo Then Return

				If srcFound Then
					Dim newSelectedBtn As JRadioButton = Nothing
					If [next] Then
						' Select Next button. Cycle to the first button if the source
						' button is the last of the group.
						newSelectedBtn = If(Nothing Is nextBtn, firstBtn, nextBtn)
					Else
						' Select previous button. Cycle to the last button if the source
						' button is the first button of the group.
						newSelectedBtn = If(Nothing Is previousBtn, lastBtn, previousBtn)
					End If
					If newSelectedBtn IsNot Nothing AndAlso (newSelectedBtn IsNot activeBtn) Then
						newSelectedBtn.requestFocusInWindow()
						newSelectedBtn.selected = True
					End If
				End If
			End Sub

			''' <summary>
			''' Find the button group the passed in JRadioButton belongs to, and
			''' move focus to next component of the last button in the group
			''' or previous component of first button
			''' </summary>
			''' <param name="next">, indicate if jump to next component or previous </param>
			Friend Overridable Sub jumpToNextComponent(ByVal [next] As Boolean)
				If Not buttonGroupInfo Then
					' In case the button does not belong to any group, it needs
					' to be treated as a component
					If activeBtn IsNot Nothing Then
						lastBtn = activeBtn
						firstBtn = activeBtn
					Else
						Return
					End If
				End If

				' Update the component we will use as base to transfer
				' focus from
				Dim compTransferFocusFrom As JComponent = activeBtn

				' If next component in the parent window is not in
				' the button group, current active button will be
				' base, otherwise, the base will be first or last
				' button in the button group
				Dim focusBase As Component = getFocusTransferBaseComponent([next])
				If focusBase IsNot Nothing Then
					If [next] Then
						KeyboardFocusManager.currentKeyboardFocusManager.focusNextComponent(focusBase)
					Else
						KeyboardFocusManager.currentKeyboardFocusManager.focusPreviousComponent(focusBase)
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' Radiobutton KeyListener
		''' </summary>
		Private Class KeyHandler
			Implements KeyListener

			Private ReadOnly outerInstance As BasicRadioButtonUI

			Public Sub New(ByVal outerInstance As BasicRadioButtonUI)
				Me.outerInstance = outerInstance
			End Sub


			' This listener checks if the key event is a KeyEvent.VK_TAB
			' or shift + KeyEvent.VK_TAB event on a radio button, consume the event
			' if so and move the focus to next/previous component
			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If e.keyCode = KeyEvent.VK_TAB Then
					 ' Get the source of the event.
					Dim eventSrc As Object = e.source

					' Check whether the source is a visible and enabled JRadioButton
					If outerInstance.isValidRadioButtonObj(eventSrc) Then
						e.consume()
						Dim btnGroupInfo As New ButtonGroupInfo(CType(eventSrc, JRadioButton))
						btnGroupInfo.jumpToNextComponent((Not e.shiftDown))
					End If
				End If
			End Sub

			Public Overridable Sub keyReleased(ByVal e As KeyEvent)
			End Sub

			Public Overridable Sub keyTyped(ByVal e As KeyEvent)
			End Sub
		End Class
	End Class

End Namespace