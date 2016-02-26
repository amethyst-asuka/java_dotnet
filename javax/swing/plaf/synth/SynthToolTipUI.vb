Imports Microsoft.VisualBasic
Imports javax.swing

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
	''' <seealso cref="javax.swing.JToolTip"/>.
	''' 
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthToolTipUI
		Inherits javax.swing.plaf.basic.BasicToolTipUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthToolTipUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal c As JComponent)
			updateStyle(c)
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners(ByVal c As JComponent)
			c.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners(ByVal c As JComponent)
			c.removePropertyChangeListener(Me)
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
			Dim comp As JComponent = CType(c, JToolTip).component

			If comp IsNot Nothing AndAlso (Not comp.enabled) Then Return DISABLED
			Return SynthLookAndFeel.getComponentState(c)
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
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintToolTipBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintToolTipBorder(context, g, x, y, w, h)
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
			Dim tip As JToolTip = CType(context.component, JToolTip)

			Dim insets As Insets = tip.insets
			Dim v As javax.swing.text.View = CType(tip.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then
				Dim paintTextR As New Rectangle(insets.left, insets.top, tip.width - (insets.left + insets.right), tip.height - (insets.top + insets.bottom))
				v.paint(g, paintTextR)
			Else
				g.color = context.style.getColor(context, ColorType.TEXT_FOREGROUND)
				g.font = style.getFont(context)
				context.style.getGraphicsUtils(context).paintText(context, g, tip.tipText, insets.left, insets.top, -1)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim ___context As SynthContext = getContext(c)
			Dim insets As Insets = c.insets
			Dim prefSize As New Dimension(insets.left+insets.right, insets.top+insets.bottom)
			Dim text As String = CType(c, JToolTip).tipText

			If text IsNot Nothing Then
				Dim v As javax.swing.text.View = If(c IsNot Nothing, CType(c.getClientProperty("html"), javax.swing.text.View), Nothing)
				If v IsNot Nothing Then
					prefSize.width += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.X_AXIS)))
					prefSize.height += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
				Else
					Dim font As Font = ___context.style.getFont(___context)
					Dim fm As FontMetrics = c.getFontMetrics(font)
					prefSize.width += ___context.style.getGraphicsUtils(___context).computeStringWidth(___context, font, fm, text)
					prefSize.height += fm.height
				End If
			End If
			___context.Dispose()
			Return prefSize
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JToolTip))
			Dim name As String = e.propertyName
			If name.Equals("tiptext") OrElse "font".Equals(name) OrElse "foreground".Equals(name) Then
				' remove the old html view client property if one
				' existed, and install a new one if the text installed
				' into the JLabel is html source.
				Dim tip As JToolTip = (CType(e.source, JToolTip))
				Dim text As String = tip.tipText
				javax.swing.plaf.basic.BasicHTML.updateRenderer(tip, text)
			End If
		End Sub
	End Class

End Namespace