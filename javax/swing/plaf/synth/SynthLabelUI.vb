Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

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
	''' <seealso cref="javax.swing.JLabel"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthLabelUI
		Inherits BasicLabelUI
		Implements SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Returns the LabelUI implementation used for the skins look and feel.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthLabelUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal c As JLabel)
			updateStyle(c)
		End Sub

		Friend Overridable Sub updateStyle(ByVal c As JLabel)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal c As JLabel)
			Dim ___context As SynthContext = getContext(c, ENABLED)

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

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			Dim state As Integer = SynthLookAndFeel.getComponentState(c)
			If SynthLookAndFeel.selectedUI Is Me AndAlso state = SynthConstants.ENABLED Then state = SynthLookAndFeel.selectedUIState Or SynthConstants.ENABLED
			Return state
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException("Component must be non-null")
			If width < 0 OrElse height < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")
			Dim label As JLabel = CType(c, JLabel)
			Dim text As String = label.text
			If text Is Nothing OrElse "".Equals(text) Then Return -1
			Dim i As java.awt.Insets = label.insets
			Dim viewRect As New java.awt.Rectangle
			Dim textRect As New java.awt.Rectangle
			Dim iconRect As New java.awt.Rectangle
			viewRect.x = i.left
			viewRect.y = i.top
			viewRect.width = width - (i.right + viewRect.x)
			viewRect.height = height - (i.bottom + viewRect.y)

			' layout the text and icon
			Dim ___context As SynthContext = getContext(label)
			Dim fm As java.awt.FontMetrics = ___context.component.getFontMetrics(___context.style.getFont(___context))
			___context.style.getGraphicsUtils(___context).layoutText(___context, fm, label.text, label.icon, label.horizontalAlignment, label.verticalAlignment, label.horizontalTextPosition, label.verticalTextPosition, viewRect, iconRect, textRect, label.iconTextGap)
			Dim view As javax.swing.text.View = CType(label.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			Dim ___baseline As Integer
			If view IsNot Nothing Then
				___baseline = BasicHTML.getHTMLBaseline(view, textRect.width, textRect.height)
				If ___baseline >= 0 Then ___baseline += textRect.y
			Else
				___baseline = textRect.y + fm.ascent
			End If
			___context.Dispose()
			Return ___baseline
		End Function

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
		Public Overrides Sub update(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintLabelBackground(___context, g, 0, 0, c.width, c.height)
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
		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
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
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As java.awt.Graphics)
			Dim label As JLabel = CType(context.component, JLabel)
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)

			g.color = context.style.getColor(context, ColorType.TEXT_FOREGROUND)
			g.font = style.getFont(context)
			context.style.getGraphicsUtils(context).paintText(context, g, label.text, icon, label.horizontalAlignment, label.verticalAlignment, label.horizontalTextPosition, label.verticalTextPosition, label.iconTextGap, label.displayedMnemonicIndex, 0)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			context.painter.paintLabelBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Dim label As JLabel = CType(c, JLabel)
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)
			Dim ___context As SynthContext = getContext(c)
			Dim size As java.awt.Dimension = ___context.style.getGraphicsUtils(___context).getPreferredSize(___context, ___context.style.getFont(___context), label.text, icon, label.horizontalAlignment, label.verticalAlignment, label.horizontalTextPosition, label.verticalTextPosition, label.iconTextGap, label.displayedMnemonicIndex)

			___context.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Dim label As JLabel = CType(c, JLabel)
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)
			Dim ___context As SynthContext = getContext(c)
			Dim size As java.awt.Dimension = ___context.style.getGraphicsUtils(___context).getMinimumSize(___context, ___context.style.getFont(___context), label.text, icon, label.horizontalAlignment, label.verticalAlignment, label.horizontalTextPosition, label.verticalTextPosition, label.iconTextGap, label.displayedMnemonicIndex)

			___context.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Dim label As JLabel = CType(c, JLabel)
			Dim icon As Icon = If(label.enabled, label.icon, label.disabledIcon)
			Dim ___context As SynthContext = getContext(c)
			Dim size As java.awt.Dimension = ___context.style.getGraphicsUtils(___context).getMaximumSize(___context, ___context.style.getFont(___context), label.text, icon, label.horizontalAlignment, label.verticalAlignment, label.horizontalTextPosition, label.verticalTextPosition, label.iconTextGap, label.displayedMnemonicIndex)

			___context.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			MyBase.propertyChange(e)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JLabel))
		End Sub
	End Class

End Namespace