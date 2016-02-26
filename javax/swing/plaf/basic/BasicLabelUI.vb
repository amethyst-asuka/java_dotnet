Imports System
Imports javax.swing
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
	''' A Windows L&amp;F implementation of LabelUI.  This implementation
	''' is completely static, i.e. there's only one UIView implementation
	''' that's shared by all JLabel objects.
	''' 
	''' @author Hans Muller
	''' </summary>
	Public Class BasicLabelUI
		Inherits LabelUI
		Implements java.beans.PropertyChangeListener

	   ''' <summary>
	   ''' The default <code>BasicLabelUI</code> instance. This field might
	   ''' not be used. To change the default instance use a subclass which
	   ''' overrides the <code>createUI</code> method, and place that class
	   ''' name in defaults table under the key "LabelUI".
	   ''' </summary>
		Protected Friend Shared labelUI As New BasicLabelUI
		Private Shared ReadOnly BASIC_LABEL_UI_KEY As New Object

		Private paintIconR As New java.awt.Rectangle
		Private paintTextR As New java.awt.Rectangle

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.PRESS))
			map.put(New Actions(Actions.RELEASE))
		End Sub

		''' <summary>
		''' Forwards the call to SwingUtilities.layoutCompoundLabel().
		''' This method is here so that a subclass could do Label specific
		''' layout and to shorten the method name a little.
		''' </summary>
		''' <seealso cref= SwingUtilities#layoutCompoundLabel </seealso>
		Protected Friend Overridable Function layoutCL(ByVal label As JLabel, ByVal fontMetrics As java.awt.FontMetrics, ByVal text As String, ByVal icon As Icon, ByVal viewR As java.awt.Rectangle, ByVal iconR As java.awt.Rectangle, ByVal textR As java.awt.Rectangle) As String
			Return SwingUtilities.layoutCompoundLabel(CType(label, JComponent), fontMetrics, text, icon, label.verticalAlignment, label.horizontalAlignment, label.verticalTextPosition, label.horizontalTextPosition, viewR, iconR, textR, label.iconTextGap)
		End Function

		''' <summary>
		''' Paint clippedText at textX, textY with the labels foreground color.
		''' </summary>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #paintDisabledText </seealso>
		Protected Friend Overridable Sub paintEnabledText(ByVal l As JLabel, ByVal g As java.awt.Graphics, ByVal s As String, ByVal textX As Integer, ByVal textY As Integer)
			Dim mnemIndex As Integer = l.displayedMnemonicIndex
			g.color = l.foreground
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(l, g, s, mnemIndex, textX, textY)
		End Sub


		''' <summary>
		''' Paint clippedText at textX, textY with background.lighter() and then
		''' shifted down and to the right by one pixel with background.darker().
		''' </summary>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #paintEnabledText </seealso>
		Protected Friend Overridable Sub paintDisabledText(ByVal l As JLabel, ByVal g As java.awt.Graphics, ByVal s As String, ByVal textX As Integer, ByVal textY As Integer)
			Dim accChar As Integer = l.displayedMnemonicIndex
			Dim background As java.awt.Color = l.background
			g.color = background.brighter()
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(l, g, s, accChar, textX + 1, textY + 1)
			g.color = background.darker()
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(l, g, s, accChar, textX, textY)
		End Sub

		''' <summary>
		''' Paints the label text with the foreground color, if the label is opaque
		''' then paints the entire background with the background color. The Label
		''' text is drawn by <seealso cref="#paintEnabledText"/> or <seealso cref="#paintDisabledText"/>.
		''' The locations of the label parts are computed by <seealso cref="#layoutCL"/>.
		''' </summary>
		''' <seealso cref= #paintEnabledText </seealso>
		''' <seealso cref= #paintDisabledText </seealso>
		''' <seealso cref= #layoutCL </seealso>
		Public Overridable Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim label As JLabel = CType(c, JLabel)
			Dim text As String = label.text
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)

			If (icon Is Nothing) AndAlso (text Is Nothing) Then Return

			Dim fm As java.awt.FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(label, g)
			Dim clippedText As String = layout(label, fm, c.width, c.height)

			If icon IsNot Nothing Then icon.paintIcon(c, g, paintIconR.x, paintIconR.y)

			If text IsNot Nothing Then
				Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
				If v IsNot Nothing Then
					v.paint(g, paintTextR)
				Else
					Dim textX As Integer = paintTextR.x
					Dim textY As Integer = paintTextR.y + fm.ascent

					If label.enabled Then
						paintEnabledText(label, g, clippedText, textX, textY)
					Else
						paintDisabledText(label, g, clippedText, textX, textY)
					End If
				End If
			End If
		End Sub

		Private Function layout(ByVal label As JLabel, ByVal fm As java.awt.FontMetrics, ByVal width As Integer, ByVal height As Integer) As String
			Dim insets As java.awt.Insets = label.getInsets(Nothing)
			Dim text As String = label.text
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)
			Dim paintViewR As New java.awt.Rectangle
			paintViewR.x = insets.left
			paintViewR.y = insets.top
			paintViewR.width = width - (insets.left + insets.right)
			paintViewR.height = height - (insets.top + insets.bottom)
				paintIconR.height = 0
					paintIconR.width = paintIconR.height
						paintIconR.y = paintIconR.width
						paintIconR.x = paintIconR.y
				paintTextR.height = 0
					paintTextR.width = paintTextR.height
						paintTextR.y = paintTextR.width
						paintTextR.x = paintTextR.y
			Return layoutCL(label, fm, text, icon, paintViewR, paintIconR, paintTextR)
		End Function

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Dim label As JLabel = CType(c, JLabel)
			Dim text As String = label.text
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)
			Dim insets As java.awt.Insets = label.getInsets(Nothing)
			Dim font As java.awt.Font = label.font

			Dim dx As Integer = insets.left + insets.right
			Dim dy As Integer = insets.top + insets.bottom

			If (icon Is Nothing) AndAlso ((text Is Nothing) OrElse ((text IsNot Nothing) AndAlso (font Is Nothing))) Then
				Return New java.awt.Dimension(dx, dy)
			ElseIf (text Is Nothing) OrElse ((icon IsNot Nothing) AndAlso (font Is Nothing)) Then
				Return New java.awt.Dimension(icon.iconWidth + dx, icon.iconHeight + dy)
			Else
				Dim fm As java.awt.FontMetrics = label.getFontMetrics(font)
				Dim iconR As New java.awt.Rectangle
				Dim textR As New java.awt.Rectangle
				Dim viewR As New java.awt.Rectangle

					iconR.height = 0
						iconR.width = iconR.height
							iconR.y = iconR.width
							iconR.x = iconR.y
					textR.height = 0
						textR.width = textR.height
							textR.y = textR.width
							textR.x = textR.y
				viewR.x = dx
				viewR.y = dy
					viewR.height = Short.MaxValue
					viewR.width = viewR.height

				layoutCL(label, fm, text, icon, viewR, iconR, textR)
				Dim x1 As Integer = Math.Min(iconR.x, textR.x)
				Dim x2 As Integer = Math.Max(iconR.x + iconR.width, textR.x + textR.width)
				Dim y1 As Integer = Math.Min(iconR.y, textR.y)
				Dim y2 As Integer = Math.Max(iconR.y + iconR.height, textR.y + textR.height)
				Dim rv As New java.awt.Dimension(x2 - x1, y2 - y1)

				rv.width += dx
				rv.height += dy
				Return rv
			End If
		End Function


		''' <returns> getPreferredSize(c) </returns>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Dim d As java.awt.Dimension = getPreferredSize(c)
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then d.width -= v.getPreferredSpan(javax.swing.text.View.X_AXIS) - v.getMinimumSpan(javax.swing.text.View.X_AXIS)
			Return d
		End Function

		''' <returns> getPreferredSize(c) </returns>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Dim d As java.awt.Dimension = getPreferredSize(c)
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
			Dim label As JLabel = CType(c, JLabel)
			Dim text As String = label.text
			If text Is Nothing OrElse "".Equals(text) OrElse label.font Is Nothing Then Return -1
			Dim fm As java.awt.FontMetrics = label.getFontMetrics(label.font)
			layout(label, fm, width, height)
			Return BasicHTML.getBaseline(label, paintTextR.y, fm.ascent, paintTextR.width, paintTextR.height)
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As java.awt.Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			If c.getClientProperty(BasicHTML.propertyKey) IsNot Nothing Then Return java.awt.Component.BaselineResizeBehavior.OTHER
			Select Case CType(c, JLabel).verticalAlignment
			Case JLabel.TOP
				Return java.awt.Component.BaselineResizeBehavior.CONSTANT_ASCENT
			Case JLabel.BOTTOM
				Return java.awt.Component.BaselineResizeBehavior.CONSTANT_DESCENT
			Case JLabel.CENTER
				Return java.awt.Component.BaselineResizeBehavior.CENTER_OFFSET
			End Select
			Return java.awt.Component.BaselineResizeBehavior.OTHER
		End Function


		Public Overridable Sub installUI(ByVal c As JComponent)
			installDefaults(CType(c, JLabel))
			installComponents(CType(c, JLabel))
			installListeners(CType(c, JLabel))
			installKeyboardActions(CType(c, JLabel))
		End Sub


		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults(CType(c, JLabel))
			uninstallComponents(CType(c, JLabel))
			uninstallListeners(CType(c, JLabel))
			uninstallKeyboardActions(CType(c, JLabel))
		End Sub

		 Protected Friend Overridable Sub installDefaults(ByVal c As JLabel)
			 LookAndFeel.installColorsAndFont(c, "Label.background", "Label.foreground", "Label.font")
			 LookAndFeel.installProperty(c, "opaque", Boolean.FALSE)
		 End Sub

		Protected Friend Overridable Sub installListeners(ByVal c As JLabel)
			c.addPropertyChangeListener(Me)
		End Sub

		Protected Friend Overridable Sub installComponents(ByVal c As JLabel)
			BasicHTML.updateRenderer(c, c.text)
			c.inheritsPopupMenu = True
		End Sub

		Protected Friend Overridable Sub installKeyboardActions(ByVal l As JLabel)
			Dim dka As Integer = l.displayedMnemonic
			Dim lf As java.awt.Component = l.labelFor
			If (dka <> 0) AndAlso (lf IsNot Nothing) Then
				LazyActionMap.installLazyActionMap(l, GetType(BasicLabelUI), "Label.actionMap")
				Dim inputMap As InputMap = SwingUtilities.getUIInputMap(l, JComponent.WHEN_IN_FOCUSED_WINDOW)
				If inputMap Is Nothing Then
					inputMap = New ComponentInputMapUIResource(l)
					SwingUtilities.replaceUIInputMap(l, JComponent.WHEN_IN_FOCUSED_WINDOW, inputMap)
				End If
				inputMap.clear()
				inputMap.put(KeyStroke.getKeyStroke(dka, BasicLookAndFeel.focusAcceleratorKeyMask, False), "press")
			Else
				Dim inputMap As InputMap = SwingUtilities.getUIInputMap(l, JComponent.WHEN_IN_FOCUSED_WINDOW)
				If inputMap IsNot Nothing Then inputMap.clear()
			End If
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal c As JLabel)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal c As JLabel)
			c.removePropertyChangeListener(Me)
		End Sub

		Protected Friend Overridable Sub uninstallComponents(ByVal c As JLabel)
			BasicHTML.updateRenderer(c, "")
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions(ByVal c As JLabel)
			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_FOCUSED, Nothing)
			SwingUtilities.replaceUIInputMap(c, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(c, Nothing)
		End Sub

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			If System.securityManager IsNot Nothing Then
				Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim safeBasicLabelUI As BasicLabelUI = CType(appContext.get(BASIC_LABEL_UI_KEY), BasicLabelUI)
				If safeBasicLabelUI Is Nothing Then
					safeBasicLabelUI = New BasicLabelUI
					appContext.put(BASIC_LABEL_UI_KEY, safeBasicLabelUI)
				End If
				Return safeBasicLabelUI
			End If
			Return labelUI
		End Function

		Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			Dim name As String = e.propertyName
			If name = "text" OrElse "font" = name OrElse "foreground" = name Then
				' remove the old html view client property if one
				' existed, and install a new one if the text installed
				' into the JLabel is html source.
				Dim lbl As JLabel = (CType(e.source, JLabel))
				Dim text As String = lbl.text
				BasicHTML.updateRenderer(lbl, text)
			ElseIf name = "labelFor" OrElse name = "displayedMnemonic" Then
				installKeyboardActions(CType(e.source, JLabel))
			End If
		End Sub

		' When the accelerator is pressed, temporarily make the JLabel
		' focusTraversable by registering a WHEN_FOCUSED action for the
		' release of the accelerator.  Then give it focus so it can
		' prevent unwanted keyTyped events from getting to other components.
		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const PRESS As String = "press"
			Private Const RELEASE As String = "release"

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim label As JLabel = CType(e.source, JLabel)
				Dim key As String = name
				If key = PRESS Then
					doPress(label)
				ElseIf key = RELEASE Then
					doRelease(label)
				End If
			End Sub

			Private Sub doPress(ByVal label As JLabel)
				Dim labelFor As java.awt.Component = label.labelFor
				If labelFor IsNot Nothing AndAlso labelFor.enabled Then
					Dim inputMap As InputMap = SwingUtilities.getUIInputMap(label, JComponent.WHEN_FOCUSED)
					If inputMap Is Nothing Then
						inputMap = New InputMapUIResource
						SwingUtilities.replaceUIInputMap(label, JComponent.WHEN_FOCUSED, inputMap)
					End If
					Dim dka As Integer = label.displayedMnemonic
					inputMap.put(KeyStroke.getKeyStroke(dka, BasicLookAndFeel.focusAcceleratorKeyMask, True), RELEASE)
					' Need this when the sticky keys are enabled
					inputMap.put(KeyStroke.getKeyStroke(dka, 0, True), RELEASE)
					' Need this if ALT is released before the accelerator
					inputMap.put(KeyStroke.getKeyStroke(java.awt.event.KeyEvent.VK_ALT, 0, True), RELEASE)
					label.requestFocus()
				End If
			End Sub

			Private Sub doRelease(ByVal label As JLabel)
				Dim labelFor As java.awt.Component = label.labelFor
				If labelFor IsNot Nothing AndAlso labelFor.enabled Then
					Dim inputMap As InputMap = SwingUtilities.getUIInputMap(label, JComponent.WHEN_FOCUSED)
					If inputMap IsNot Nothing Then
						' inputMap should never be null.
						Dim dka As Integer = label.displayedMnemonic
						inputMap.remove(KeyStroke.getKeyStroke(dka, BasicLookAndFeel.focusAcceleratorKeyMask, True))
						inputMap.remove(KeyStroke.getKeyStroke(dka, 0, True))
						inputMap.remove(KeyStroke.getKeyStroke(java.awt.event.KeyEvent.VK_ALT, 0, True))
					End If
					If TypeOf labelFor Is java.awt.Container AndAlso CType(labelFor, java.awt.Container).focusCycleRoot Then
						labelFor.requestFocus()
					Else
						sun.swing.SwingUtilities2.compositeRequestFocus(labelFor)
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace