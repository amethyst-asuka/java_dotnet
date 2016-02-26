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
	''' <seealso cref="javax.swing.JOptionPane"/>.
	''' 
	''' @author James Gosling
	''' @author Scott Violet
	''' @author Amy Fowler
	''' @since 1.7
	''' </summary>
	Public Class SynthOptionPaneUI
		Inherits BasicOptionPaneUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New SynthOptionPaneUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(optionPane)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			optionPane.addPropertyChangeListener(Me)
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				minimumSize = CType(style.get(___context, "OptionPane.minimumSize"), Dimension)
				If minimumSize Is Nothing Then minimumSize = New Dimension(262, 90)
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
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(optionPane, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			optionPane.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installComponents()
			optionPane.add(createMessageArea())

			Dim separator As Container = createSeparator()
			If separator IsNot Nothing Then
				optionPane.add(separator)
				Dim ___context As SynthContext = getContext(optionPane, ENABLED)
				optionPane.add(Box.createVerticalStrut(___context.style.getInt(___context, "OptionPane.separatorPadding", 6)))
				___context.Dispose()
			End If
			optionPane.add(createButtonArea())
			optionPane.applyComponentOrientation(optionPane.componentOrientation)
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
			___context.painter.paintOptionPaneBackground(___context, g, 0, 0, c.width, c.height)
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
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintOptionPaneBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JOptionPane))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides sizeButtonsToSameWidth As Boolean
			Get
				Return sun.swing.DefaultLookup.getBoolean(optionPane, Me, "OptionPane.sameSizeButtons", True)
			End Get
		End Property

		''' <summary>
		''' Called from <seealso cref="#installComponents"/> to create a {@code Container}
		''' containing the body of the message. The icon is the created by calling
		''' <seealso cref="#addIcon"/>.
		''' </summary>
		Protected Friend Overrides Function createMessageArea() As Container
			Dim top As New JPanel
			top.name = "OptionPane.messageArea"
			top.layout = New BorderLayout

			' Fill the body. 
			Dim body As Container = New JPanel(New GridBagLayout)
			Dim realBody As Container = New JPanel(New BorderLayout)

			body.name = "OptionPane.body"
			realBody.name = "OptionPane.realBody"

			If icon IsNot Nothing Then
				Dim sep As New JPanel
				sep.name = "OptionPane.separator"
				sep.preferredSize = New Dimension(15, 1)
				realBody.add(sep, BorderLayout.BEFORE_LINE_BEGINS)
			End If
			realBody.add(body, BorderLayout.CENTER)

			Dim cons As New GridBagConstraints
				cons.gridy = 0
				cons.gridx = cons.gridy
			cons.gridwidth = GridBagConstraints.REMAINDER
			cons.gridheight = 1

			Dim ___context As SynthContext = getContext(optionPane, ENABLED)
			cons.anchor = ___context.style.getInt(___context, "OptionPane.messageAnchor", GridBagConstraints.CENTER)
			___context.Dispose()

			cons.insets = New Insets(0,0,3,0)

			addMessageComponents(body, cons, message, maxCharactersPerLineCount, False)
			top.add(realBody, BorderLayout.CENTER)

			addIcon(top)
			Return top
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createSeparator() As Container
			Dim separator As New JSeparator(SwingConstants.HORIZONTAL)

			separator.name = "OptionPane.separator"
			Return separator
		End Function
	End Class

End Namespace