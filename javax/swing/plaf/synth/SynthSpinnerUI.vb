Imports Microsoft.VisualBasic
Imports System
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
	''' <seealso cref="javax.swing.JSpinner"/>.
	''' 
	''' @author Hans Muller
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthSpinnerUI
		Inherits javax.swing.plaf.basic.BasicSpinnerUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle
		''' <summary>
		''' A FocusListener implementation which causes the entire spinner to be
		''' repainted whenever the editor component (typically a text field) becomes
		''' focused, or loses focus. This is necessary because since SynthSpinnerUI
		''' is composed of an editor and two buttons, it is necessary that all three
		''' components indicate that they are "focused" so that they can be drawn
		''' appropriately. The repaint is used to ensure that the buttons are drawn
		''' in the new focused or unfocused state, mirroring that of the editor.
		''' </summary>
		Private editorFocusHandler As New EditorFocusHandler(Me)

		''' <summary>
		''' Returns a new instance of SynthSpinnerUI.
		''' </summary>
		''' <param name="c"> the JSpinner (not used) </param>
		''' <seealso cref= ComponentUI#createUI </seealso>
		''' <returns> a new SynthSpinnerUI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthSpinnerUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			spinner.addPropertyChangeListener(Me)
			Dim editor As JComponent = spinner.editor
			If TypeOf editor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then tf.addFocusListener(editorFocusHandler)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			spinner.removePropertyChangeListener(Me)
			Dim editor As JComponent = spinner.editor
			If TypeOf editor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then tf.removeFocusListener(editorFocusHandler)
			End If
		End Sub

		''' <summary>
		''' Initializes the <code>JSpinner</code> <code>border</code>,
		''' <code>foreground</code>, and <code>background</code>, properties
		''' based on the corresponding "Spinner.*" properties from defaults table.
		''' The <code>JSpinners</code> layout is set to the value returned by
		''' <code>createLayout</code>.  This method is called by <code>installUI</code>.
		''' </summary>
		''' <seealso cref= #uninstallDefaults </seealso>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #createLayout </seealso>
		''' <seealso cref= LookAndFeel#installBorder </seealso>
		''' <seealso cref= LookAndFeel#installColors </seealso>
		Protected Friend Overrides Sub installDefaults()
			Dim layout As LayoutManager = spinner.layout

			If layout Is Nothing OrElse TypeOf layout Is UIResource Then spinner.layout = createLayout()
			updateStyle(spinner)
		End Sub


		Private Sub updateStyle(ByVal c As JSpinner)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				If oldStyle IsNot Nothing Then installKeyboardActions()
			End If
			___context.Dispose()
		End Sub


		''' <summary>
		''' Sets the <code>JSpinner's</code> layout manager to null.  This
		''' method is called by <code>uninstallUI</code>.
		''' </summary>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #uninstallUI </seealso>
		Protected Friend Overrides Sub uninstallDefaults()
			If TypeOf spinner.layout Is UIResource Then spinner.layout = Nothing

			Dim ___context As SynthContext = getContext(spinner, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createLayout() As LayoutManager
			Return New SpinnerLayout
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createPreviousButton() As Component
			Dim b As JButton = New SynthArrowButton(SwingConstants.SOUTH)
			b.name = "Spinner.previousButton"
			installPreviousButtonListeners(b)
			Return b
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createNextButton() As Component
			Dim b As JButton = New SynthArrowButton(SwingConstants.NORTH)
			b.name = "Spinner.nextButton"
			installNextButtonListeners(b)
			Return b
		End Function


		''' <summary>
		''' This method is called by installUI to get the editor component
		''' of the <code>JSpinner</code>.  By default it just returns
		''' <code>JSpinner.getEditor()</code>.  Subclasses can override
		''' <code>createEditor</code> to return a component that contains
		''' the spinner's editor or null, if they're going to handle adding
		''' the editor to the <code>JSpinner</code> in an
		''' <code>installUI</code> override.
		''' <p>
		''' Typically this method would be overridden to wrap the editor
		''' with a container with a custom border, since one can't assume
		''' that the editors border can be set directly.
		''' <p>
		''' The <code>replaceEditor</code> method is called when the spinners
		''' editor is changed with <code>JSpinner.setEditor</code>.  If you've
		''' overriden this method, then you'll probably want to override
		''' <code>replaceEditor</code> as well.
		''' </summary>
		''' <returns> the JSpinners editor JComponent, spinner.getEditor() by default </returns>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #replaceEditor </seealso>
		''' <seealso cref= JSpinner#getEditor </seealso>
		Protected Friend Overrides Function createEditor() As JComponent
			Dim editor As JComponent = spinner.editor
			editor.name = "Spinner.editor"
			updateEditorAlignment(editor)
			Return editor
		End Function


		''' <summary>
		''' Called by the <code>PropertyChangeListener</code> when the
		''' <code>JSpinner</code> editor property changes.  It's the responsibility
		''' of this method to remove the old editor and add the new one.  By
		''' default this operation is just:
		''' <pre>
		''' spinner.remove(oldEditor);
		''' spinner.add(newEditor, "Editor");
		''' </pre>
		''' The implementation of <code>replaceEditor</code> should be coordinated
		''' with the <code>createEditor</code> method.
		''' </summary>
		''' <seealso cref= #createEditor </seealso>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend Overrides Sub replaceEditor(ByVal oldEditor As JComponent, ByVal newEditor As JComponent)
			spinner.remove(oldEditor)
			spinner.add(newEditor, "Editor")
			If TypeOf oldEditor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(oldEditor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then tf.removeFocusListener(editorFocusHandler)
			End If
			If TypeOf newEditor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(newEditor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then tf.addFocusListener(editorFocusHandler)
			End If
		End Sub

		Private Sub updateEditorAlignment(ByVal editor As JComponent)
			If TypeOf editor Is JSpinner.DefaultEditor Then
				Dim ___context As SynthContext = getContext(spinner)
				Dim alignment As Integer? = CInt(Fix(___context.style.get(___context, "Spinner.editorAlignment")))
				Dim text As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				If alignment IsNot Nothing Then text.horizontalAlignment = alignment
				' copy across the sizeVariant property to the editor
				text.putClientProperty("JComponent.sizeVariant", spinner.getClientProperty("JComponent.sizeVariant"))
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
			___context.painter.paintSpinnerBackground(___context, g, 0, 0, c.width, c.height)
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
			context.painter.paintSpinnerBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' A simple layout manager for the editor and the next/previous buttons.
		''' See the SynthSpinnerUI javadoc for more information about exactly
		''' how the components are arranged.
		''' </summary>
		Private Class SpinnerLayout
			Implements LayoutManager, UIResource

			Private nextButton As Component = Nothing
			Private previousButton As Component = Nothing
			Private editor As Component = Nothing

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
				If "Next".Equals(name) Then
					nextButton = c
				ElseIf "Previous".Equals(name) Then
					previousButton = c
				ElseIf "Editor".Equals(name) Then
					editor = c
				End If
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
				If c Is nextButton Then
					nextButton = Nothing
				ElseIf c Is previousButton Then
					previousButton = Nothing
				ElseIf c Is editor Then
					editor = Nothing
				End If
			End Sub

			Private Function preferredSize(ByVal c As Component) As Dimension
				Return If(c Is Nothing, New Dimension(0, 0), c.preferredSize)
			End Function

			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Dim nextD As Dimension = preferredSize(nextButton)
				Dim previousD As Dimension = preferredSize(previousButton)
				Dim editorD As Dimension = preferredSize(editor)

	'             Force the editors height to be a multiple of 2
	'             
				editorD.height = ((editorD.height + 1) / 2) * 2

				Dim size As New Dimension(editorD.width, editorD.height)
				size.width += Math.Max(nextD.width, previousD.width)
				Dim insets As Insets = parent.insets
				size.width += insets.left + insets.right
				size.height += insets.top + insets.bottom
				Return size
			End Function

			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Return preferredLayoutSize(parent)
			End Function

			Private Sub setBounds(ByVal c As Component, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				If c IsNot Nothing Then c.boundsnds(x, y, width, height)
			End Sub

			Public Overridable Sub layoutContainer(ByVal parent As Container)
				Dim insets As Insets = parent.insets
				Dim availWidth As Integer = parent.width - (insets.left + insets.right)
				Dim availHeight As Integer = parent.height - (insets.top + insets.bottom)
				Dim nextD As Dimension = preferredSize(nextButton)
				Dim previousD As Dimension = preferredSize(previousButton)
				Dim nextHeight As Integer = availHeight \ 2
				Dim previousHeight As Integer = availHeight - nextHeight
				Dim buttonsWidth As Integer = Math.Max(nextD.width, previousD.width)
				Dim editorWidth As Integer = availWidth - buttonsWidth

	'             Deal with the spinners componentOrientation property.
	'             
				Dim editorX, buttonsX As Integer
				If parent.componentOrientation.leftToRight Then
					editorX = insets.left
					buttonsX = editorX + editorWidth
				Else
					buttonsX = insets.left
					editorX = buttonsX + buttonsWidth
				End If

				Dim previousY As Integer = insets.top + nextHeight
				boundsnds(editor, editorX, insets.top, editorWidth, availHeight)
				boundsnds(nextButton, buttonsX, insets.top, buttonsWidth, nextHeight)
				boundsnds(previousButton, buttonsX, previousY, buttonsWidth, previousHeight)
			End Sub
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			Dim spinner As JSpinner = CType(e.source, JSpinner)
			Dim spinnerUI As SpinnerUI = spinner.uI

			If TypeOf spinnerUI Is SynthSpinnerUI Then
				Dim ui As SynthSpinnerUI = CType(spinnerUI, SynthSpinnerUI)

				If SynthLookAndFeel.shouldUpdateStyle(e) Then ui.updateStyle(spinner)
			End If
		End Sub

		''' <summary>
		''' Listen to editor text field focus changes and repaint whole spinner </summary>
		Private Class EditorFocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As SynthSpinnerUI

			Public Sub New(ByVal outerInstance As SynthSpinnerUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Invoked when a editor text field gains the keyboard focus. </summary>
			Public Overrides Sub focusGained(ByVal e As FocusEvent)
				outerInstance.spinner.repaint()
			End Sub

			''' <summary>
			''' Invoked when a editor text field loses the keyboard focus. </summary>
			Public Overrides Sub focusLost(ByVal e As FocusEvent)
				outerInstance.spinner.repaint()
			End Sub
		End Class
	End Class

End Namespace