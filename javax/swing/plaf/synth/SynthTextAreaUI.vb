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
	''' Provides the look and feel for a plain text editor in the
	''' Synth look and feel. In this implementation the default UI
	''' is extended to act as a simple view factory.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Shannon Hickey
	''' @since 1.7
	''' </summary>
	Public Class SynthTextAreaUI
		Inherits javax.swing.plaf.basic.BasicTextAreaUI
		Implements SynthUI

		Private handler As New Handler(Me)
		Private style As SynthStyle

		''' <summary>
		''' Creates a UI object for a JTextArea.
		''' </summary>
		''' <param name="ta"> a text area </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal ta As JComponent) As ComponentUI
			Return New SynthTextAreaUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			' Installs the text cursor on the component
			MyBase.installDefaults()
			updateStyle(component)
			component.addFocusListener(handler)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(component, ENABLED)

			component.putClientProperty("caretAspectRatio", Nothing)
			component.removeFocusListener(handler)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
			MyBase.uninstallDefaults()
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
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
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
			___context.painter.paintTextAreaBackground(___context, g, 0, 0, c.width, c.height)
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
		''' 
		''' Overridden to do nothing.
		''' </summary>
		Protected Friend Overrides Sub paintBackground(ByVal g As Graphics)
			' Overriden to do nothing, all our painting is done from update/paint.
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintTextAreaBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to rebuild the View when the
		''' <em>WrapLine</em> or the <em>WrapStyleWord</em> property changes.
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JTextComponent))
			MyBase.propertyChange(evt)
		End Sub

		Private NotInheritable Class Handler
			Implements java.awt.event.FocusListener

			Private ReadOnly outerInstance As SynthTextAreaUI

			Public Sub New(ByVal outerInstance As SynthTextAreaUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub focusGained(ByVal e As java.awt.event.FocusEvent)
				outerInstance.component.repaint()
			End Sub

			Public Sub focusLost(ByVal e As java.awt.event.FocusEvent)
				outerInstance.component.repaint()
			End Sub
		End Class
	End Class

End Namespace