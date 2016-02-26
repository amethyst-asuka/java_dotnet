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
	''' <seealso cref="javax.swing.JPanel"/>.
	''' 
	''' @author Steve Wilson
	''' @since 1.7
	''' </summary>
	Public Class SynthPanelUI
		Inherits javax.swing.plaf.basic.BasicPanelUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthPanelUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub installUI(ByVal c As JComponent)
			Dim p As JPanel = CType(c, JPanel)

			MyBase.installUI(c)
			installListeners(p)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			Dim p As JPanel = CType(c, JPanel)

			uninstallListeners(p)
			MyBase.uninstallUI(c)
		End Sub

		''' <summary>
		''' Installs listeners into the panel.
		''' </summary>
		''' <param name="p"> the {@code JPanel} object </param>
		Protected Friend Overridable Sub installListeners(ByVal p As JPanel)
			p.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Uninstalls listeners from the panel.
		''' </summary>
		''' <param name="p"> the {@code JPanel} object </param>
		Protected Friend Overridable Sub uninstallListeners(ByVal p As JPanel)
			p.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal p As JPanel)
			updateStyle(p)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal p As JPanel)
			Dim ___context As SynthContext = getContext(p, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		Private Sub updateStyle(ByVal c As JPanel)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
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
			___context.painter.paintPanelBackground(___context, g, 0, 0, c.width, c.height)
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
		''' Paints the specified component. This implementation does nothing.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			' do actual painting
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintPanelBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal pce As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(pce) Then updateStyle(CType(pce.source, JPanel))
		End Sub
	End Class

End Namespace