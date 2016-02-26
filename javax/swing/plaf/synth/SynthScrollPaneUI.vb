Imports System.Diagnostics
Imports javax.swing
Imports javax.swing.border
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
	''' <seealso cref="javax.swing.JScrollPane"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthScrollPaneUI
		Inherits BasicScrollPaneUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private viewportViewHasFocus As Boolean = False
		Private viewportViewFocusHandler As ViewportViewFocusHandler

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New SynthScrollPaneUI
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
			___context.painter.paintScrollPaneBackground(___context, g, 0, 0, c.width, c.height)
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
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			Dim vpBorder As Border = scrollpane.viewportBorder
			If vpBorder IsNot Nothing Then
				Dim r As Rectangle = scrollpane.viewportBorderBounds
				vpBorder.paintBorder(scrollpane, g, r.x, r.y, r.width, r.height)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintScrollPaneBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal scrollpane As JScrollPane)
			updateStyle(scrollpane)
		End Sub

		Private Sub updateStyle(ByVal c As JScrollPane)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				Dim vpBorder As Border = scrollpane.viewportBorder
				If (vpBorder Is Nothing) OrElse (TypeOf vpBorder Is UIResource) Then scrollpane.viewportBorder = New ViewportBorder(Me, ___context)
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions(c)
					installKeyboardActions(c)
				End If
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners(ByVal c As JScrollPane)
			MyBase.installListeners(c)
			c.addPropertyChangeListener(Me)
			If UIManager.getBoolean("ScrollPane.useChildTextComponentFocus") Then
				viewportViewFocusHandler = New ViewportViewFocusHandler(Me)
				c.viewport.addContainerListener(viewportViewFocusHandler)
				Dim view As Component = c.viewport.view
				If TypeOf view Is javax.swing.text.JTextComponent Then view.addFocusListener(viewportViewFocusHandler)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal c As JScrollPane)
			Dim ___context As SynthContext = getContext(c, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()

			If TypeOf scrollpane.viewportBorder Is UIResource Then scrollpane.viewportBorder = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners(ByVal c As JComponent)
			MyBase.uninstallListeners(c)
			c.removePropertyChangeListener(Me)
			If viewportViewFocusHandler IsNot Nothing Then
				Dim viewport As JViewport = CType(c, JScrollPane).viewport
				viewport.removeContainerListener(viewportViewFocusHandler)
				If viewport.view IsNot Nothing Then viewport.view.removeFocusListener(viewportViewFocusHandler)
				viewportViewFocusHandler = Nothing
			End If
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
			Dim baseState As Integer = SynthLookAndFeel.getComponentState(c)
			If viewportViewFocusHandler IsNot Nothing AndAlso viewportViewHasFocus Then baseState = baseState Or FOCUSED
			Return baseState
		End Function

		Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(scrollpane)
		End Sub



		Private Class ViewportBorder
			Inherits AbstractBorder
			Implements UIResource

			Private ReadOnly outerInstance As SynthScrollPaneUI

			Private insets As Insets

			Friend Sub New(ByVal outerInstance As SynthScrollPaneUI, ByVal context As SynthContext)
					Me.outerInstance = outerInstance
				Me.insets = CType(context.style.get(context, "ScrollPane.viewportBorderInsets"), Insets)
				If Me.insets Is Nothing Then Me.insets = SynthLookAndFeel.EMPTY_UIRESOURCE_INSETS
			End Sub

			Public Overrides Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				Dim jc As JComponent = CType(c, JComponent)
				Dim context As SynthContext = outerInstance.getContext(jc)
				Dim style As SynthStyle = context.style
				If style Is Nothing Then
					Debug.Assert(False, "SynthBorder is being used outside after the " & " UI has been uninstalled")
					Return
				End If
				context.painter.paintViewportBorder(context, g, x, y, width, height)
				context.Dispose()
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As Component, ByVal insets As Insets) As Insets
				If insets Is Nothing Then Return New Insets(Me.insets.top, Me.insets.left, Me.insets.bottom, Me.insets.right)
				insets.top = Me.insets.top
				insets.bottom = Me.insets.bottom
				insets.left = Me.insets.left
				insets.right = Me.insets.left
				Return insets
			End Function

			Public Property Overrides borderOpaque As Boolean
				Get
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' Handle keeping track of the viewport's view's focus
		''' </summary>
		Private Class ViewportViewFocusHandler
			Implements java.awt.event.ContainerListener, java.awt.event.FocusListener

			Private ReadOnly outerInstance As SynthScrollPaneUI

			Public Sub New(ByVal outerInstance As SynthScrollPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub componentAdded(ByVal e As java.awt.event.ContainerEvent)
				If TypeOf e.child Is javax.swing.text.JTextComponent Then
					e.child.addFocusListener(Me)
					outerInstance.viewportViewHasFocus = e.child.focusOwner
					outerInstance.scrollpane.repaint()
				End If
			End Sub

			Public Overridable Sub componentRemoved(ByVal e As java.awt.event.ContainerEvent)
				If TypeOf e.child Is javax.swing.text.JTextComponent Then e.child.removeFocusListener(Me)
			End Sub

			Public Overridable Sub focusGained(ByVal e As java.awt.event.FocusEvent)
				outerInstance.viewportViewHasFocus = True
				outerInstance.scrollpane.repaint()
			End Sub

			Public Overridable Sub focusLost(ByVal e As java.awt.event.FocusEvent)
				outerInstance.viewportViewHasFocus = False
				outerInstance.scrollpane.repaint()
			End Sub
		End Class
	End Class

End Namespace