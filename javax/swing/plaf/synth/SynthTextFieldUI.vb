Imports Microsoft.VisualBasic
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
	''' Provides the Synth L&amp;F UI delegate for <seealso cref="javax.swing.JTextField"/>.
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
	Public Class SynthTextFieldUI
		Inherits javax.swing.plaf.basic.BasicTextFieldUI
		Implements SynthUI

		Private handler As New Handler(Me)
		Private style As SynthStyle

		''' <summary>
		''' Creates a UI for a JTextField.
		''' </summary>
		''' <param name="c"> the text field </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthTextFieldUI
		End Function

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

		Friend Shared Sub updateStyle(ByVal comp As JTextComponent, ByVal context As SynthContext, ByVal prefix As String)
			Dim style As SynthStyle = context.style

			Dim color As Color = comp.caretColor
			If color Is Nothing OrElse TypeOf color Is UIResource Then comp.caretColor = CType(style.get(context, prefix & ".caretForeground"), Color)

			Dim fg As Color = comp.foreground
			If fg Is Nothing OrElse TypeOf fg Is UIResource Then
				fg = style.getColorForState(context, ColorType.TEXT_FOREGROUND)
				If fg IsNot Nothing Then comp.foreground = fg
			End If

			Dim ar As Object = style.get(context, prefix & ".caretAspectRatio")
			If TypeOf ar Is Number Then comp.putClientProperty("caretAspectRatio", ar)

			context.componentState = SELECTED Or FOCUSED

			Dim s As Color = comp.selectionColor
			If s Is Nothing OrElse TypeOf s Is UIResource Then comp.selectionColor = style.getColor(context, ColorType.TEXT_BACKGROUND)

			Dim sfg As Color = comp.selectedTextColor
			If sfg Is Nothing OrElse TypeOf sfg Is UIResource Then comp.selectedTextColor = style.getColor(context, ColorType.TEXT_FOREGROUND)

			context.componentState = DISABLED

			Dim dfg As Color = comp.disabledTextColor
			If dfg Is Nothing OrElse TypeOf dfg Is UIResource Then comp.disabledTextColor = style.getColor(context, ColorType.TEXT_FOREGROUND)

			Dim margin As Insets = comp.margin
			If margin Is Nothing OrElse TypeOf margin Is UIResource Then
				margin = CType(style.get(context, prefix & ".margin"), Insets)

				If margin Is Nothing Then margin = SynthLookAndFeel.EMPTY_UIRESOURCE_INSETS
				comp.margin = margin
			End If

			Dim caret As Caret = comp.caret
			If TypeOf caret Is UIResource Then
				Dim o As Object = style.get(context, prefix & ".caretBlinkRate")
				If o IsNot Nothing AndAlso TypeOf o Is Integer? Then
					Dim rate As Integer? = CInt(Fix(o))
					caret.blinkRate = rate
				End If
			End If
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
			paintBackground(___context, g, c)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' <p>This is routed to the <seealso cref="#paintSafely"/> method under
		''' the guarantee that the model does not change from the view of this
		''' thread while it is rendering (if the associated model is
		''' derived from {@code AbstractDocument}).  This enables the
		''' model to potentially be updated asynchronously.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			MyBase.paint(g, component)
		End Sub

		Friend Overridable Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			context.painter.paintTextFieldBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintTextFieldBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' Overridden to do nothing.
		''' </summary>
		Protected Friend Overrides Sub paintBackground(ByVal g As Graphics)
			' Overriden to do nothing, all our painting is done from update/paint.
		End Sub

		''' <summary>
		''' This method gets called when a bound property is changed
		''' on the associated JTextComponent.  This is a hook
		''' which UI implementations may change to reflect how the
		''' UI displays bound properties of JTextComponent subclasses.
		''' This is implemented to do nothing (i.e. the response to
		''' properties in JTextComponent itself are handled prior
		''' to calling this method).
		''' </summary>
		''' <param name="evt"> the property change event </param>
		Protected Friend Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JTextComponent))
			MyBase.propertyChange(evt)
		End Sub

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

		Private NotInheritable Class Handler
			Implements java.awt.event.FocusListener

			Private ReadOnly outerInstance As SynthTextFieldUI

			Public Sub New(ByVal outerInstance As SynthTextFieldUI)
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