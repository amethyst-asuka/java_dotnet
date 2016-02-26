Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' BasicToggleButton implementation
	''' <p>
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicToggleButtonUI
		Inherits BasicButtonUI

		Private Shared ReadOnly BASIC_TOGGLE_BUTTON_UI_KEY As New Object

		Private Shared ReadOnly propertyPrefix As String = "ToggleButton" & "."

		' ********************************
		'          Create PLAF
		' ********************************
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim toggleButtonUI As BasicToggleButtonUI = CType(appContext.get(BASIC_TOGGLE_BUTTON_UI_KEY), BasicToggleButtonUI)
			If toggleButtonUI Is Nothing Then
				toggleButtonUI = New BasicToggleButtonUI
				appContext.put(BASIC_TOGGLE_BUTTON_UI_KEY, toggleButtonUI)
			End If
			Return toggleButtonUI
		End Function

		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return propertyPrefix
			End Get
		End Property


		' ********************************
		'          Paint Methods
		' ********************************
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model

			Dim size As Dimension = b.size
			Dim fm As FontMetrics = g.fontMetrics

			Dim i As Insets = c.insets

			Dim viewRect As New Rectangle(size)

			viewRect.x += i.left
			viewRect.y += i.top
			viewRect.width -= (i.right + viewRect.x)
			viewRect.height -= (i.bottom + viewRect.y)

			Dim iconRect As New Rectangle
			Dim textRect As New Rectangle

			Dim f As Font = c.font
			g.font = f

			' layout the text and icon
			Dim text As String = SwingUtilities.layoutCompoundLabel(c, fm, b.text, b.icon, b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, viewRect, iconRect, textRect,If(b.text Is Nothing, 0, b.iconTextGap))

			g.color = b.background

			If model.armed AndAlso model.pressed OrElse model.selected Then paintButtonPressed(g,b)

			' Paint the Icon
			If b.icon IsNot Nothing Then paintIcon(g, b, iconRect)

			' Draw the Text
			If text IsNot Nothing AndAlso (Not text.Equals("")) Then
				Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
				If v IsNot Nothing Then
				   v.paint(g, textRect)
				Else
				   paintText(g, b, textRect, text)
				End If
			End If

			' draw the dashed focus line.
			If b.focusPainted AndAlso b.hasFocus() Then paintFocus(g, b, viewRect, textRect, iconRect)
		End Sub

		Protected Friend Overridable Sub paintIcon(ByVal g As Graphics, ByVal b As AbstractButton, ByVal iconRect As Rectangle)
			Dim model As ButtonModel = b.model
			Dim icon As Icon = Nothing

			If Not model.enabled Then
				If model.selected Then
				   icon = b.disabledSelectedIcon
				Else
				   icon = b.disabledIcon
				End If
			ElseIf model.pressed AndAlso model.armed Then
				icon = b.pressedIcon
				If icon Is Nothing Then icon = b.selectedIcon
			ElseIf model.selected Then
				If b.rolloverEnabled AndAlso model.rollover Then
					icon = b.rolloverSelectedIcon
					If icon Is Nothing Then icon = b.selectedIcon
				Else
					icon = b.selectedIcon
				End If
			ElseIf b.rolloverEnabled AndAlso model.rollover Then
				icon = b.rolloverIcon
			End If

			If icon Is Nothing Then icon = b.icon

			icon.paintIcon(b, g, iconRect.x, iconRect.y)
		End Sub

		''' <summary>
		''' Overriden so that the text will not be rendered as shifted for
		''' Toggle buttons and subclasses.
		''' </summary>
		Protected Friend Property Overrides textShiftOffset As Integer
			Get
				Return 0
			End Get
		End Property

	End Class

End Namespace