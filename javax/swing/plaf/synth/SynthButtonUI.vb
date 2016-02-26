Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JButton"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthButtonUI
		Inherits javax.swing.plaf.basic.BasicButtonUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthButtonUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal b As AbstractButton)
			updateStyle(b)

			LookAndFeel.installProperty(b, "rolloverEnabled", Boolean.TRUE)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners(ByVal b As AbstractButton)
			MyBase.installListeners(b)
			b.addPropertyChangeListener(Me)
		End Sub

		Friend Overridable Sub updateStyle(ByVal b As AbstractButton)
			Dim ___context As SynthContext = getContext(b, SynthConstants.ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				If b.margin Is Nothing OrElse (TypeOf b.margin Is UIResource) Then
					Dim margin As Insets = CType(style.get(___context,propertyPrefix & "margin"), Insets)

					If margin Is Nothing Then margin = SynthLookAndFeel.EMPTY_UIRESOURCE_INSETS
					b.margin = margin
				End If

				Dim value As Object = style.get(___context, propertyPrefix & "iconTextGap")
				If value IsNot Nothing Then LookAndFeel.installProperty(b, "iconTextGap", value)

				value = style.get(___context, propertyPrefix & "contentAreaFilled")
				LookAndFeel.installProperty(b, "contentAreaFilled",If(value IsNot Nothing, value, Boolean.TRUE))

				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions(b)
					installKeyboardActions(b)
				End If

			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners(ByVal b As AbstractButton)
			MyBase.uninstallListeners(b)
			b.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal b As AbstractButton)
			Dim ___context As SynthContext = getContext(b, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Friend Overridable Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		''' <summary>
		''' Returns the current state of the passed in <code>AbstractButton</code>.
		''' </summary>
		Private Function getComponentState(ByVal c As JComponent) As Integer
			Dim state As Integer = ENABLED

			If Not c.enabled Then state = DISABLED
			If SynthLookAndFeel.selectedUI Is Me Then Return SynthLookAndFeel.selectedUIState Or SynthConstants.ENABLED
			Dim button As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = button.model

			If model.pressed Then
				If model.armed Then
					state = PRESSED
				Else
					state = MOUSE_OVER
				End If
			End If
			If model.rollover Then state = state Or MOUSE_OVER
			If model.selected Then state = state Or SELECTED
			If c.focusOwner AndAlso button.focusPainted Then state = state Or FOCUSED
			If (TypeOf c Is JButton) AndAlso CType(c, JButton).defaultButton Then state = state Or DEFAULT
			Return state
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException("Component must be non-null")
			If width < 0 OrElse height < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim text As String = b.text
			If text Is Nothing OrElse "".Equals(text) Then Return -1
			Dim i As Insets = b.insets
			Dim viewRect As New Rectangle
			Dim textRect As New Rectangle
			Dim iconRect As New Rectangle
			viewRect.x = i.left
			viewRect.y = i.top
			viewRect.width = width - (i.right + viewRect.x)
			viewRect.height = height - (i.bottom + viewRect.y)

			' layout the text and icon
			Dim ___context As SynthContext = getContext(b)
			Dim fm As FontMetrics = ___context.component.getFontMetrics(___context.style.getFont(___context))
			___context.style.getGraphicsUtils(___context).layoutText(___context, fm, b.text, b.icon, b.horizontalAlignment, b.verticalAlignment, b.horizontalTextPosition, b.verticalTextPosition, viewRect, iconRect, textRect, b.iconTextGap)
			Dim view As javax.swing.text.View = CType(b.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), javax.swing.text.View)
			Dim ___baseline As Integer
			If view IsNot Nothing Then
				___baseline = javax.swing.plaf.basic.BasicHTML.getHTMLBaseline(view, textRect.width, textRect.height)
				If ___baseline >= 0 Then ___baseline += textRect.y
			Else
				___baseline = textRect.y + fm.ascent
			End If
			___context.Dispose()
			Return ___baseline
		End Function

		' ********************************
		'          Paint Methods
		' ********************************

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			paintBackground(___context, g, c)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			Dim b As AbstractButton = CType(context.component, AbstractButton)

			g.color = context.style.getColor(context, ColorType.TEXT_FOREGROUND)
			g.font = style.getFont(context)
			context.style.getGraphicsUtils(context).paintText(context, g, b.text, getIcon(b), b.horizontalAlignment, b.verticalAlignment, b.horizontalTextPosition, b.verticalTextPosition, b.iconTextGap, b.displayedMnemonicIndex, getTextShiftOffset(context))
		End Sub

		Friend Overridable Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			If CType(c, AbstractButton).contentAreaFilled Then context.painter.paintButtonBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintButtonBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Returns the default icon. This should not callback
		''' to the JComponent.
		''' </summary>
		''' <param name="b"> button the icon is associated with </param>
		''' <returns> default icon </returns>
		Protected Friend Overridable Function getDefaultIcon(ByVal b As AbstractButton) As Icon
			Dim ___context As SynthContext = getContext(b)
			Dim ___icon As Icon = ___context.style.getIcon(___context, propertyPrefix & "icon")
			___context.Dispose()
			Return ___icon
		End Function

		''' <summary>
		''' Returns the Icon to use for painting the button. The icon is chosen with
		''' respect to the current state of the button.
		''' </summary>
		''' <param name="b"> button the icon is associated with </param>
		''' <returns> an icon </returns>
		Protected Friend Overridable Function getIcon(ByVal b As AbstractButton) As Icon
			Dim ___icon As Icon = b.icon
			Dim model As ButtonModel = b.model

			If Not model.enabled Then
				___icon = getSynthDisabledIcon(b, ___icon)
			ElseIf model.pressed AndAlso model.armed Then
				___icon = getPressedIcon(b, getSelectedIcon(b, ___icon))
			ElseIf b.rolloverEnabled AndAlso model.rollover Then
				___icon = getRolloverIcon(b, getSelectedIcon(b, ___icon))
			ElseIf model.selected Then
				___icon = getSelectedIcon(b, ___icon)
			Else
				___icon = getEnabledIcon(b, ___icon)
			End If
			If ___icon Is Nothing Then Return getDefaultIcon(b)
			Return ___icon
		End Function

		''' <summary>
		''' This method will return the icon that should be used for a button.  We
		''' only want to use the synth icon defined by the style if the specific
		''' icon has not been defined for the button state and the backup icon is a
		''' UIResource (we set it, not the developer).
		''' </summary>
		''' <param name="b"> button </param>
		''' <param name="specificIcon"> icon returned from the button for the specific state </param>
		''' <param name="defaultIcon"> fallback icon </param>
		''' <param name="state"> the synth state of the button </param>
		Private Function getIcon(ByVal b As AbstractButton, ByVal specificIcon As Icon, ByVal defaultIcon As Icon, ByVal state As Integer) As Icon
			Dim ___icon As Icon = specificIcon
			If ___icon Is Nothing Then
				If TypeOf defaultIcon Is UIResource Then
					___icon = getSynthIcon(b, state)
					If ___icon Is Nothing Then ___icon = defaultIcon
				Else
					___icon = defaultIcon
				End If
			End If
			Return ___icon
		End Function

		Private Function getSynthIcon(ByVal b As AbstractButton, ByVal synthConstant As Integer) As Icon
			Return style.getIcon(getContext(b, synthConstant), propertyPrefix & "icon")
		End Function

		Private Function getEnabledIcon(ByVal b As AbstractButton, ByVal defaultIcon As Icon) As Icon
			If defaultIcon Is Nothing Then defaultIcon = getSynthIcon(b, SynthConstants.ENABLED)
			Return defaultIcon
		End Function

		Private Function getSelectedIcon(ByVal b As AbstractButton, ByVal defaultIcon As Icon) As Icon
			Return getIcon(b, b.selectedIcon, defaultIcon, SynthConstants.SELECTED)
		End Function

		Private Function getRolloverIcon(ByVal b As AbstractButton, ByVal defaultIcon As Icon) As Icon
			Dim model As ButtonModel = b.model
			Dim ___icon As Icon
			If model.selected Then
				___icon = getIcon(b, b.rolloverSelectedIcon, defaultIcon, SynthConstants.MOUSE_OVER Or SynthConstants.SELECTED)
			Else
				___icon = getIcon(b, b.rolloverIcon, defaultIcon, SynthConstants.MOUSE_OVER)
			End If
			Return ___icon
		End Function

		Private Function getPressedIcon(ByVal b As AbstractButton, ByVal defaultIcon As Icon) As Icon
			Return getIcon(b, b.pressedIcon, defaultIcon, SynthConstants.PRESSED)
		End Function

		Private Function getSynthDisabledIcon(ByVal b As AbstractButton, ByVal defaultIcon As Icon) As Icon
			Dim model As ButtonModel = b.model
			Dim ___icon As Icon
			If model.selected Then
				___icon = getIcon(b, b.disabledSelectedIcon, defaultIcon, SynthConstants.DISABLED Or SynthConstants.SELECTED)
			Else
				___icon = getIcon(b, b.disabledIcon, defaultIcon, SynthConstants.DISABLED)
			End If
			Return ___icon
		End Function

		''' <summary>
		''' Returns the amount to shift the text/icon when painting.
		''' </summary>
		Private Function getTextShiftOffset(ByVal state As SynthContext) As Integer
			Dim button As AbstractButton = CType(state.component, AbstractButton)
			Dim model As ButtonModel = button.model

			If model.armed AndAlso model.pressed AndAlso button.pressedIcon Is Nothing Then Return state.style.getInt(state, propertyPrefix & "textShiftOffset", 0)
			Return 0
		End Function

		' ********************************
		'          Layout Methods
		' ********************************

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As Dimension
			If c.componentCount > 0 AndAlso c.layout IsNot Nothing Then Return Nothing
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim ss As SynthContext = getContext(c)
			Dim size As Dimension = ss.style.getGraphicsUtils(ss).getMinimumSize(ss, ss.style.getFont(ss), b.text, getSizingIcon(b), b.horizontalAlignment, b.verticalAlignment, b.horizontalTextPosition, b.verticalTextPosition, b.iconTextGap, b.displayedMnemonicIndex)

			ss.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			If c.componentCount > 0 AndAlso c.layout IsNot Nothing Then Return Nothing
			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim ss As SynthContext = getContext(c)
			Dim size As Dimension = ss.style.getGraphicsUtils(ss).getPreferredSize(ss, ss.style.getFont(ss), b.text, getSizingIcon(b), b.horizontalAlignment, b.verticalAlignment, b.horizontalTextPosition, b.verticalTextPosition, b.iconTextGap, b.displayedMnemonicIndex)

			ss.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMaximumSize(ByVal c As JComponent) As Dimension
			If c.componentCount > 0 AndAlso c.layout IsNot Nothing Then Return Nothing

			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim ss As SynthContext = getContext(c)
			Dim size As Dimension = ss.style.getGraphicsUtils(ss).getMaximumSize(ss, ss.style.getFont(ss), b.text, getSizingIcon(b), b.horizontalAlignment, b.verticalAlignment, b.horizontalTextPosition, b.verticalTextPosition, b.iconTextGap, b.displayedMnemonicIndex)

			ss.Dispose()
			Return size
		End Function

		''' <summary>
		''' Returns the Icon used in calculating the
		''' preferred/minimum/maximum size.
		''' </summary>
		Protected Friend Overridable Function getSizingIcon(ByVal b As AbstractButton) As Icon
			Dim ___icon As Icon = getEnabledIcon(b, b.icon)
			If ___icon Is Nothing Then ___icon = getDefaultIcon(b)
			Return ___icon
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, AbstractButton))
		End Sub
	End Class

End Namespace