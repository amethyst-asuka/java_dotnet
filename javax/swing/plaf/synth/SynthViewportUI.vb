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
	''' <seealso cref="javax.swing.JViewport"/>.
	''' 
	''' @since 1.7
	''' </summary>
	Public Class SynthViewportUI
		Inherits ViewportUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthViewportUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			installDefaults(c)
			installListeners(c)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			uninstallListeners(c)
			uninstallDefaults(c)
		End Sub

		''' <summary>
		''' Installs defaults for a viewport.
		''' </summary>
		''' <param name="c"> a {@code JViewport} object </param>
		Protected Friend Overridable Sub installDefaults(ByVal c As JComponent)
			updateStyle(c)
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)

			' Note: JViewport is special cased as it does not allow for
			' a border to be set. JViewport.setBorder is overriden to throw
			' an IllegalArgumentException. Refer to SynthScrollPaneUI for
			' details of this.
			Dim newStyle As SynthStyle = SynthLookAndFeel.getStyle(___context.component, ___context.region)
			Dim oldStyle As SynthStyle = ___context.style

			If newStyle IsNot oldStyle Then
				If oldStyle IsNot Nothing Then oldStyle.uninstallDefaults(___context)
				___context.style = newStyle
				newStyle.installDefaults(___context)
			End If
			Me.style = newStyle
			___context.Dispose()
		End Sub

		''' <summary>
		''' Installs listeners into the viewport.
		''' </summary>
		''' <param name="c"> a {@code JViewport} object </param>
		Protected Friend Overridable Sub installListeners(ByVal c As JComponent)
			c.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Uninstalls listeners from the viewport.
		''' </summary>
		''' <param name="c"> a {@code JViewport} object </param>
		Protected Friend Overridable Sub uninstallListeners(ByVal c As JComponent)
			c.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Uninstalls defaults from a viewport.
		''' </summary>
		''' <param name="c"> a {@code JViewport} object </param>
		Protected Friend Overridable Sub uninstallDefaults(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getRegion(ByVal c As JComponent) As Region
			Return SynthLookAndFeel.getRegion(c)
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
			___context.painter.paintViewportBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the border. The method is never called,
		''' because the {@code JViewport} class does not support a border.
		''' This implementation does nothing.
		''' </summary>
		''' <param name="context"> a component context </param>
		''' <param name="g"> the {@code Graphics} to paint on </param>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="w"> width of the border </param>
		''' <param name="h"> height of the border </param>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
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
		''' Paints the specified component. This implementation does nothing.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JComponent))
		End Sub
	End Class

End Namespace