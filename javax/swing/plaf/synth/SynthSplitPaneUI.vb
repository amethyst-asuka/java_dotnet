Imports System
Imports System.Collections.Generic
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
	''' <seealso cref="javax.swing.JSplitPane"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthSplitPaneUI
		Inherits BasicSplitPaneUI
		Implements PropertyChangeListener, SynthUI

		''' <summary>
		''' Keys to use for forward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private Shared managingFocusForwardTraversalKeys As [Set](Of KeyStroke)

		''' <summary>
		''' Keys to use for backward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private Shared managingFocusBackwardTraversalKeys As [Set](Of KeyStroke)

		''' <summary>
		''' Style for the JSplitPane.
		''' </summary>
		Private style As SynthStyle
		''' <summary>
		''' Style for the divider.
		''' </summary>
		Private dividerStyle As SynthStyle


		''' <summary>
		''' Creates a new SynthSplitPaneUI instance
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New SynthSplitPaneUI
		End Function

		''' <summary>
		''' Installs the UI defaults.
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(splitPane)

			orientation = splitPane.orientation
			continuousLayout = splitPane.continuousLayout

			resetLayoutManager()

	'         Install the nonContinuousLayoutDivider here to avoid having to
	'        add/remove everything later. 
			If nonContinuousLayoutDivider Is Nothing Then
				nonContinuousLayoutDividerder(createDefaultNonContinuousLayoutDivider(), True)
			Else
				nonContinuousLayoutDividerder(nonContinuousLayoutDivider, True)
			End If

			' focus forward traversal key
			If managingFocusForwardTraversalKeys Is Nothing Then
				managingFocusForwardTraversalKeys = New HashSet(Of KeyStroke)
				managingFocusForwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, 0))
			End If
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, managingFocusForwardTraversalKeys)
			' focus backward traversal key
			If managingFocusBackwardTraversalKeys Is Nothing Then
				managingFocusBackwardTraversalKeys = New HashSet(Of KeyStroke)
				managingFocusBackwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.SHIFT_MASK))
			End If
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, managingFocusBackwardTraversalKeys)
		End Sub

		Private Sub updateStyle(ByVal splitPane As JSplitPane)
			Dim ___context As SynthContext = getContext(splitPane, Region.SPLIT_PANE_DIVIDER, ENABLED)
			Dim oldDividerStyle As SynthStyle = dividerStyle
			dividerStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()

			___context = getContext(splitPane, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)

			If style IsNot oldStyle Then
				Dim value As Object = style.get(___context, "SplitPane.size")
				If value Is Nothing Then value = Convert.ToInt32(6)
				LookAndFeel.installProperty(splitPane, "dividerSize", value)

				value = style.get(___context, "SplitPane.oneTouchExpandable")
				If value IsNot Nothing Then LookAndFeel.installProperty(splitPane, "oneTouchExpandable", value)

				If divider IsNot Nothing Then
					splitPane.remove(divider)
					divider.dividerSize = splitPane.dividerSize
				End If
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			If style IsNot oldStyle OrElse dividerStyle IsNot oldDividerStyle Then
				' Only way to force BasicSplitPaneDivider to reread the
				' necessary properties.
				If divider IsNot Nothing Then splitPane.remove(divider)
				divider = createDefaultDivider()
				divider.basicSplitPaneUI = Me
				splitPane.add(divider, JSplitPane.DIVIDER)
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' Installs the event listeners for the UI.
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			splitPane.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' Uninstalls the UI defaults.
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(splitPane, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			___context = getContext(splitPane, Region.SPLIT_PANE_DIVIDER, ENABLED)
			dividerStyle.uninstallDefaults(___context)
			___context.Dispose()
			dividerStyle = Nothing

			MyBase.uninstallDefaults()
		End Sub


		''' <summary>
		''' Uninstalls the event listeners from the UI.
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			splitPane.removePropertyChangeListener(Me)
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

		Friend Overridable Function getContext(ByVal c As JComponent, ByVal ___region As Region) As SynthContext
			Return getContext(c, ___region, getComponentState(c, ___region))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal ___region As Region, ByVal state As Integer) As SynthContext
			If ___region Is Region.SPLIT_PANE_DIVIDER Then Return SynthContext.getContext(c, ___region, dividerStyle, state)
			Return SynthContext.getContext(c, ___region, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent, ByVal subregion As Region) As Integer
			Dim state As Integer = SynthLookAndFeel.getComponentState(c)

			If divider.mouseOver Then state = state Or MOUSE_OVER
			Return state
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JSplitPane))
		End Sub

		''' <summary>
		''' Creates the default divider.
		''' </summary>
		Public Overrides Function createDefaultDivider() As BasicSplitPaneDivider
			Dim ___divider As New SynthSplitPaneDivider(Me)

			___divider.dividerSize = splitPane.dividerSize
			Return ___divider
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createDefaultNonContinuousLayoutDivider() As Component
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New Canvas()
	'		{
	'			public void paint(Graphics g)
	'			{
	'				paintDragDivider(g, 0, 0, getWidth(), getHeight());
	'			}
	'		};
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
			___context.painter.paintSplitPaneBackground(___context, g, 0, 0, c.width, c.height)
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
			' This is done to update package private variables in
			' BasicSplitPaneUI
			MyBase.paint(g, splitPane)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintSplitPaneBorder(context, g, x, y, w, h)
		End Sub

		Private Sub paintDragDivider(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim ___context As SynthContext = getContext(splitPane,Region.SPLIT_PANE_DIVIDER)
			___context.componentState = ((___context.componentState Or MOUSE_OVER) Xor MOUSE_OVER) Or PRESSED
			Dim oldClip As Shape = g.clip
			g.clipRect(x, y, w, h)
			___context.painter.paintSplitPaneDragDivider(___context, g, x, y, w, h, splitPane.orientation)
			g.clip = oldClip
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub finishedPaintingChildren(ByVal jc As JSplitPane, ByVal g As Graphics)
			If jc Is splitPane AndAlso lastDragLocation <> -1 AndAlso (Not continuousLayout) AndAlso (Not draggingHW) Then
				If jc.orientation = JSplitPane.HORIZONTAL_SPLIT Then
					paintDragDivider(g, lastDragLocation, 0, dividerSize - 1, splitPane.height - 1)
				Else
					paintDragDivider(g, 0, lastDragLocation, splitPane.width - 1, dividerSize - 1)
				End If
			End If
		End Sub
	End Class

End Namespace