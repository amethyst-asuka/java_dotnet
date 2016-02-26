Imports System
Imports javax.swing
Imports javax.swing.border

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
	''' BasicButton implementation
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicButtonUI
		Inherits javax.swing.plaf.ButtonUI

		' Visual constants
		' NOTE: This is not used or set any where. Were we allowed to remove
		' fields, this would be removed.
		Protected Friend defaultTextIconGap As Integer

		' Amount to offset text, the value of this comes from
		' defaultTextShiftOffset once setTextShiftOffset has been invoked.
		Private shiftOffset As Integer = 0
		' Value that is set in shiftOffset once setTextShiftOffset has been
		' invoked. The value of this comes from the defaults table.
		Protected Friend defaultTextShiftOffset As Integer

		Private Shared ReadOnly propertyPrefix As String = "Button" & "."

		Private Shared ReadOnly BASIC_BUTTON_UI_KEY As New Object

		' ********************************
		'          Create PLAF
		' ********************************
		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim buttonUI As BasicButtonUI = CType(appContext.get(BASIC_BUTTON_UI_KEY), BasicButtonUI)
			If buttonUI Is Nothing Then
				buttonUI = New BasicButtonUI
				appContext.put(BASIC_BUTTON_UI_KEY, buttonUI)
			End If
			Return buttonUI
		End Function

		Protected Friend Overridable Property propertyPrefix As String
			Get
				Return propertyPrefix
			End Get
		End Property


		' ********************************
		'          Install PLAF
		' ********************************
		Public Overridable Sub installUI(ByVal c As JComponent)
			installDefaults(CType(c, AbstractButton))
			installListeners(CType(c, AbstractButton))
			installKeyboardActions(CType(c, AbstractButton))
			BasicHTML.updateRenderer(c, CType(c, AbstractButton).text)
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal b As AbstractButton)
			' load shared instance defaults
			Dim pp As String = propertyPrefix

			defaultTextShiftOffset = UIManager.getInt(pp & "textShiftOffset")

			' set the following defaults on the button
			If b.contentAreaFilled Then
				LookAndFeel.installProperty(b, "opaque", Boolean.TRUE)
			Else
				LookAndFeel.installProperty(b, "opaque", Boolean.FALSE)
			End If

			If b.margin Is Nothing OrElse (TypeOf b.margin Is javax.swing.plaf.UIResource) Then b.margin = UIManager.getInsets(pp & "margin")

			LookAndFeel.installColorsAndFont(b, pp & "background", pp & "foreground", pp & "font")
			LookAndFeel.installBorder(b, pp & "border")

			Dim rollover As Object = UIManager.get(pp & "rollover")
			If rollover IsNot Nothing Then LookAndFeel.installProperty(b, "rolloverEnabled", rollover)

			LookAndFeel.installProperty(b, "iconTextGap", Convert.ToInt32(4))
		End Sub

		Protected Friend Overridable Sub installListeners(ByVal b As AbstractButton)
			Dim listener As BasicButtonListener = createButtonListener(b)
			If listener IsNot Nothing Then
				b.addMouseListener(listener)
				b.addMouseMotionListener(listener)
				b.addFocusListener(listener)
				b.addPropertyChangeListener(listener)
				b.addChangeListener(listener)
			End If
		End Sub

		Protected Friend Overridable Sub installKeyboardActions(ByVal b As AbstractButton)
			Dim listener As BasicButtonListener = getButtonListener(b)

			If listener IsNot Nothing Then listener.installKeyboardActions(b)
		End Sub


		' ********************************
		'         Uninstall PLAF
		' ********************************
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallKeyboardActions(CType(c, AbstractButton))
			uninstallListeners(CType(c, AbstractButton))
			uninstallDefaults(CType(c, AbstractButton))
			BasicHTML.updateRenderer(c, "")
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions(ByVal b As AbstractButton)
			Dim listener As BasicButtonListener = getButtonListener(b)
			If listener IsNot Nothing Then listener.uninstallKeyboardActions(b)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal b As AbstractButton)
			Dim listener As BasicButtonListener = getButtonListener(b)
			If listener IsNot Nothing Then
				b.removeMouseListener(listener)
				b.removeMouseMotionListener(listener)
				b.removeFocusListener(listener)
				b.removeChangeListener(listener)
				b.removePropertyChangeListener(listener)
			End If
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal b As AbstractButton)
			LookAndFeel.uninstallBorder(b)
		End Sub

		' ********************************
		'        Create Listeners
		' ********************************
		Protected Friend Overridable Function createButtonListener(ByVal b As AbstractButton) As BasicButtonListener
			Return New BasicButtonListener(b)
		End Function

		Public Overridable Function getDefaultTextIconGap(ByVal b As AbstractButton) As Integer
			Return defaultTextIconGap
		End Function

	'     These rectangles/insets are allocated once for all
	'     * ButtonUI.paint() calls.  Re-using rectangles rather than
	'     * allocating them in each paint call substantially reduced the time
	'     * it took paint to run.  Obviously, this method can't be re-entered.
	'     
		Private Shared viewRect As New Rectangle
		Private Shared textRect As New Rectangle
		Private Shared iconRect As New Rectangle

		' ********************************
		'          Paint Methods
		' ********************************

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model

			Dim text As String = layout(b, sun.swing.SwingUtilities2.getFontMetrics(b, g), b.width, b.height)

			clearTextShiftOffset()

			' perform UI specific press action, e.g. Windows L&F shifts text
			If model.armed AndAlso model.pressed Then paintButtonPressed(g,b)

			' Paint the Icon
			If b.icon IsNot Nothing Then paintIcon(g,c,iconRect)

			If text IsNot Nothing AndAlso (Not text.Equals("")) Then
				Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
				If v IsNot Nothing Then
					v.paint(g, textRect)
				Else
					paintText(g, b, textRect, text)
				End If
			End If

			If b.focusPainted AndAlso b.hasFocus() Then paintFocus(g,b,viewRect,textRect,iconRect)
		End Sub

		Protected Friend Overridable Sub paintIcon(ByVal g As Graphics, ByVal c As JComponent, ByVal iconRect As Rectangle)
				Dim b As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = b.model
				Dim icon As Icon = b.icon
				Dim tmpIcon As Icon = Nothing

				If icon Is Nothing Then Return

				Dim selectedIcon As Icon = Nothing

				' the fallback icon should be based on the selected state 
				If model.selected Then
					selectedIcon = b.selectedIcon
					If selectedIcon IsNot Nothing Then icon = selectedIcon
				End If

				If Not model.enabled Then
					If model.selected Then
					   tmpIcon = b.disabledSelectedIcon
					   If tmpIcon Is Nothing Then tmpIcon = selectedIcon
					End If

					If tmpIcon Is Nothing Then tmpIcon = b.disabledIcon
				ElseIf model.pressed AndAlso model.armed Then
					tmpIcon = b.pressedIcon
					If tmpIcon IsNot Nothing Then clearTextShiftOffset()
				ElseIf b.rolloverEnabled AndAlso model.rollover Then
					If model.selected Then
					   tmpIcon = b.rolloverSelectedIcon
					   If tmpIcon Is Nothing Then tmpIcon = selectedIcon
					End If

					If tmpIcon Is Nothing Then tmpIcon = b.rolloverIcon
				End If

				If tmpIcon IsNot Nothing Then icon = tmpIcon

				If model.pressed AndAlso model.armed Then
					icon.paintIcon(c, g, iconRect.x + textShiftOffset, iconRect.y + textShiftOffset)
				Else
					icon.paintIcon(c, g, iconRect.x, iconRect.y)
				End If

		End Sub

		''' <summary>
		''' As of Java 2 platform v 1.4 this method should not be used or overriden.
		''' Use the paintText method which takes the AbstractButton argument.
		''' </summary>
		Protected Friend Overridable Sub paintText(ByVal g As Graphics, ByVal c As JComponent, ByVal textRect As Rectangle, ByVal text As String)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g)
			Dim mnemonicIndex As Integer = b.displayedMnemonicIndex

			' Draw the Text 
			If model.enabled Then
				''' <summary>
				'''* paint the text normally </summary>
				g.color = b.foreground
				sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c, g,text, mnemonicIndex, textRect.x + textShiftOffset, textRect.y + fm.ascent + textShiftOffset)
			Else
				''' <summary>
				'''* paint the text disabled ** </summary>
				g.color = b.background.brighter()
				sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c, g,text, mnemonicIndex, textRect.x, textRect.y + fm.ascent)
				g.color = b.background.darker()
				sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c, g,text, mnemonicIndex, textRect.x - 1, textRect.y + fm.ascent - 1)
			End If
		End Sub

		''' <summary>
		''' Method which renders the text of the current button.
		''' <p> </summary>
		''' <param name="g"> Graphics context </param>
		''' <param name="b"> Current button to render </param>
		''' <param name="textRect"> Bounding rectangle to render the text. </param>
		''' <param name="text"> String to render
		''' @since 1.4 </param>
		Protected Friend Overridable Sub paintText(ByVal g As Graphics, ByVal b As AbstractButton, ByVal textRect As Rectangle, ByVal text As String)
			paintText(g, CType(b, JComponent), textRect, text)
		End Sub

		' Method signature defined here overriden in subclasses.
		' Perhaps this class should be abstract?
		Protected Friend Overridable Sub paintFocus(ByVal g As Graphics, ByVal b As AbstractButton, ByVal viewRect As Rectangle, ByVal textRect As Rectangle, ByVal iconRect As Rectangle)
		End Sub



		Protected Friend Overridable Sub paintButtonPressed(ByVal g As Graphics, ByVal b As AbstractButton)
		End Sub

		Protected Friend Overridable Sub clearTextShiftOffset()
			Me.shiftOffset = 0
		End Sub

		Protected Friend Overridable Sub setTextShiftOffset()
			Me.shiftOffset = defaultTextShiftOffset
		End Sub

		Protected Friend Overridable Property textShiftOffset As Integer
			Get
				Return shiftOffset
			End Get
		End Property

		' ********************************
		'          Layout Methods
		' ********************************
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = getPreferredSize(c)
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then d.width -= v.getPreferredSpan(javax.swing.text.View.X_AXIS) - v.getMinimumSpan(javax.swing.text.View.X_AXIS)
			Return d
		End Function

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim b As AbstractButton = CType(c, AbstractButton)
			Return BasicGraphicsUtils.getPreferredButtonSize(b, b.iconTextGap)
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = getPreferredSize(c)
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then d.width += v.getMaximumSpan(javax.swing.text.View.X_AXIS) - v.getPreferredSpan(javax.swing.text.View.X_AXIS)
			Return d
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim text As String = b.text
			If text Is Nothing OrElse "".Equals(text) Then Return -1
			Dim fm As FontMetrics = b.getFontMetrics(b.font)
			layout(b, fm, width, height)
			Return BasicHTML.getBaseline(b, textRect.y, fm.ascent, textRect.width, textRect.height)
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			If c.getClientProperty(BasicHTML.propertyKey) IsNot Nothing Then Return Component.BaselineResizeBehavior.OTHER
			Select Case CType(c, AbstractButton).verticalAlignment
			Case AbstractButton.TOP
				Return Component.BaselineResizeBehavior.CONSTANT_ASCENT
			Case AbstractButton.BOTTOM
				Return Component.BaselineResizeBehavior.CONSTANT_DESCENT
			Case AbstractButton.CENTER
				Return Component.BaselineResizeBehavior.CENTER_OFFSET
			End Select
			Return Component.BaselineResizeBehavior.OTHER
		End Function

		Private Function layout(ByVal b As AbstractButton, ByVal fm As FontMetrics, ByVal width As Integer, ByVal height As Integer) As String
			Dim i As Insets = b.insets
			viewRect.x = i.left
			viewRect.y = i.top
			viewRect.width = width - (i.right + viewRect.x)
			viewRect.height = height - (i.bottom + viewRect.y)

				textRect.height = 0
					textRect.width = textRect.height
						textRect.y = textRect.width
						textRect.x = textRect.y
				iconRect.height = 0
					iconRect.width = iconRect.height
						iconRect.y = iconRect.width
						iconRect.x = iconRect.y

			' layout the text and icon
			Return SwingUtilities.layoutCompoundLabel(b, fm, b.text, b.icon, b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, viewRect, iconRect, textRect,If(b.text Is Nothing, 0, b.iconTextGap))
		End Function

		''' <summary>
		''' Returns the ButtonListener for the passed in Button, or null if one
		''' could not be found.
		''' </summary>
		Private Function getButtonListener(ByVal b As AbstractButton) As BasicButtonListener
			Dim listeners As MouseMotionListener() = b.mouseMotionListeners

			If listeners IsNot Nothing Then
				For Each listener As MouseMotionListener In listeners
					If TypeOf listener Is BasicButtonListener Then Return CType(listener, BasicButtonListener)
				Next listener
			End If
			Return Nothing
		End Function

	End Class

End Namespace