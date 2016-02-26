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
	''' <seealso cref="javax.swing.JSeparator"/>.
	''' 
	''' @author Shannon Hickey
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthSeparatorUI
		Inherits javax.swing.plaf.SeparatorUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthSeparatorUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub installUI(ByVal c As JComponent)
			installDefaults(CType(c, JSeparator))
			installListeners(CType(c, JSeparator))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			uninstallListeners(CType(c, JSeparator))
			uninstallDefaults(CType(c, JSeparator))
		End Sub

		''' <summary>
		''' Installs default setting. This method is called when a
		''' {@code LookAndFeel} is installed.
		''' </summary>
		Public Overridable Sub installDefaults(ByVal c As JSeparator)
			updateStyle(c)
		End Sub

		Private Sub updateStyle(ByVal sep As JSeparator)
			Dim ___context As SynthContext = getContext(sep, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)

			If style IsNot oldStyle Then
				If TypeOf sep Is JToolBar.Separator Then
					Dim size As java.awt.Dimension = CType(sep, JToolBar.Separator).separatorSize
					If size Is Nothing OrElse TypeOf size Is javax.swing.plaf.UIResource Then
						size = CType(style.get(___context, "ToolBar.separatorSize"), javax.swing.plaf.DimensionUIResource)
						If size Is Nothing Then size = New javax.swing.plaf.DimensionUIResource(10, 10)
						CType(sep, JToolBar.Separator).separatorSize = size
					End If
				End If
			End If

			___context.Dispose()
		End Sub

		''' <summary>
		''' Uninstalls default setting. This method is called when a
		''' {@code LookAndFeel} is uninstalled.
		''' </summary>
		Public Overridable Sub uninstallDefaults(ByVal c As JSeparator)
			Dim ___context As SynthContext = getContext(c, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' Installs listeners. This method is called when a
		''' {@code LookAndFeel} is installed.
		''' </summary>
		Public Overridable Sub installListeners(ByVal c As JSeparator)
			c.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Uninstalls listeners. This method is called when a
		''' {@code LookAndFeel} is uninstalled.
		''' </summary>
		Public Overridable Sub uninstallListeners(ByVal c As JSeparator)
			c.removePropertyChangeListener(Me)
		End Sub

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

			Dim separator As JSeparator = CType(___context.component, JSeparator)
			SynthLookAndFeel.update(___context, g)
			___context.painter.paintSeparatorBackground(___context, g, 0, 0, c.width, c.height, separator.orientation)
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
			Dim separator As JSeparator = CType(context.component, JSeparator)
			context.painter.paintSeparatorForeground(context, g, 0, 0, separator.width, separator.height, separator.orientation)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			Dim separator As JSeparator = CType(context.component, JSeparator)
			context.painter.paintSeparatorBorder(context, g, x, y, w, h, separator.orientation)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Dim ___context As SynthContext = getContext(c)

			Dim thickness As Integer = style.getInt(___context, "Separator.thickness", 2)
			Dim insets As java.awt.Insets = c.insets
			Dim size As java.awt.Dimension

			If CType(c, JSeparator).orientation = JSeparator.VERTICAL Then
				size = New java.awt.Dimension(insets.left + insets.right + thickness, insets.top + insets.bottom)
			Else
				size = New java.awt.Dimension(insets.left + insets.right, insets.top + insets.bottom + thickness)
			End If
			___context.Dispose()
			Return size
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Return getPreferredSize(c)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(Short.MaxValue, Short.MaxValue)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JSeparator))
		End Sub
	End Class

End Namespace