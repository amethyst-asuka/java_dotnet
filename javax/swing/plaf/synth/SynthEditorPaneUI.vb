Imports javax.swing
Imports javax.swing.text
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
	''' <seealso cref="javax.swing.JEditorPane"/>.
	''' 
	''' @author  Shannon Hickey
	''' @since 1.7
	''' </summary>
	Public Class SynthEditorPaneUI
		Inherits javax.swing.plaf.basic.BasicEditorPaneUI
		Implements SynthUI

		Private style As SynthStyle
	'    
	'     * I would prefer to use UIResource instad of this.
	'     * Unfortunately Boolean is a final class
	'     
		Private localTrue As Boolean? = Boolean.TRUE

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthEditorPaneUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			' Installs the text cursor on the component
			MyBase.installDefaults()
			Dim c As JComponent = component
			Dim clientProperty As Object = c.getClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES)
			If clientProperty Is Nothing Then c.putClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES, localTrue)
			updateStyle(component)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(component, ENABLED)
			Dim c As JComponent = component
			c.putClientProperty("caretAspectRatio", Nothing)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			Dim clientProperty As Object = c.getClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES)
			If clientProperty Is localTrue Then c.putClientProperty(JEditorPane.HONOR_DISPLAY_PROPERTIES, Boolean.FALSE)
			MyBase.uninstallDefaults()
		End Sub

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to rebuild the ActionMap based upon an
		''' EditorKit change.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JTextComponent))
			MyBase.propertyChange(evt)
		End Sub

		Private Sub updateStyle(ByVal comp As JTextComponent)
			Dim ___context As SynthContext = getContext(comp, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)

			If style IsNot oldStyle Then
				SynthTextFieldUI.updateStyle(comp, ___context, propertyPrefix)

				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
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
			paintBackground(___context, g, c)
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
			MyBase.paint(g, component)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintBackground(ByVal g As Graphics)
			' Overriden to do nothing, all our painting is done from update/paint.
		End Sub

		Friend Overridable Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			context.painter.paintEditorPaneBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintEditorPaneBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace