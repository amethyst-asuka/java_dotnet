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
	''' <seealso cref="javax.swing.JScrollBar"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthScrollBarUI
		Inherits BasicScrollBarUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private thumbStyle As SynthStyle
		Private trackStyle As SynthStyle

		Private validMinimumThumbSize As Boolean

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthScrollBarUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			trackHighlight = NO_HIGHLIGHT
			If scrollbar.layout Is Nothing OrElse (TypeOf scrollbar.layout Is UIResource) Then scrollbar.layout = Me
			configureScrollBarColors()
			updateStyle(scrollbar)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub configureScrollBarColors()
		End Sub

		Private Sub updateStyle(ByVal c As JScrollBar)
			Dim oldStyle As SynthStyle = style
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				scrollBarWidth = style.getInt(___context,"ScrollBar.thumbHeight", 14)
				minimumThumbSize = CType(style.get(___context, "ScrollBar.minimumThumbSize"), Dimension)
				If minimumThumbSize Is Nothing Then
					minimumThumbSize = New Dimension
					validMinimumThumbSize = False
				Else
					validMinimumThumbSize = True
				End If
				maximumThumbSize = CType(style.get(___context, "ScrollBar.maximumThumbSize"), Dimension)
				If maximumThumbSize Is Nothing Then maximumThumbSize = New Dimension(4096, 4097)

				incrGap = style.getInt(___context, "ScrollBar.incrementButtonGap", 0)
				decrGap = style.getInt(___context, "ScrollBar.decrementButtonGap", 0)

				' handle scaling for sizeVarients for special case components. The
				' key "JComponent.sizeVariant" scales for large/small/mini
				' components are based on Apples LAF
				Dim scaleKey As String = CStr(scrollbar.getClientProperty("JComponent.sizeVariant"))
				If scaleKey IsNot Nothing Then
					If "large".Equals(scaleKey) Then
						scrollBarWidth *= 1.15
						incrGap *= 1.15
						decrGap *= 1.15
					ElseIf "small".Equals(scaleKey) Then
						scrollBarWidth *= 0.857
						incrGap *= 0.857
						decrGap *= 0.857
					ElseIf "mini".Equals(scaleKey) Then
						scrollBarWidth *= 0.714
						incrGap *= 0.714
						decrGap *= 0.714
					End If
				End If

				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()

			___context = getContext(c, Region.SCROLL_BAR_TRACK, ENABLED)
			trackStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()

			___context = getContext(c, Region.SCROLL_BAR_THUMB, ENABLED)
			thumbStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			scrollbar.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			scrollbar.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(scrollbar, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			___context = getContext(scrollbar, Region.SCROLL_BAR_TRACK, ENABLED)
			trackStyle.uninstallDefaults(___context)
			___context.Dispose()
			trackStyle = Nothing

			___context = getContext(scrollbar, Region.SCROLL_BAR_THUMB, ENABLED)
			thumbStyle.uninstallDefaults(___context)
			___context.Dispose()
			thumbStyle = Nothing

			MyBase.uninstallDefaults()
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

		Private Function getContext(ByVal c As JComponent, ByVal ___region As Region) As SynthContext
			Return getContext(c, ___region, getComponentState(c, ___region))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal ___region As Region, ByVal state As Integer) As SynthContext
			Dim style As SynthStyle = trackStyle

			If ___region Is Region.SCROLL_BAR_THUMB Then style = thumbStyle
			Return SynthContext.getContext(c, ___region, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent, ByVal ___region As Region) As Integer
			If ___region Is Region.SCROLL_BAR_THUMB AndAlso thumbRollover AndAlso c.enabled Then Return MOUSE_OVER
			Return SynthLookAndFeel.getComponentState(c)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides supportsAbsolutePositioning As Boolean
			Get
				Dim ___context As SynthContext = getContext(scrollbar)
				Dim value As Boolean = style.getBoolean(___context, "ScrollBar.allowsAbsolutePositioning", False)
				___context.Dispose()
				Return value
			End Get
		End Property

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
			___context.painter.paintScrollBarBackground(___context, g, 0, 0, c.width, c.height, scrollbar.orientation)
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
			Dim subcontext As SynthContext = getContext(scrollbar, Region.SCROLL_BAR_TRACK)
			paintTrack(subcontext, g, trackBounds)
			subcontext.Dispose()

			subcontext = getContext(scrollbar, Region.SCROLL_BAR_THUMB)
			paintThumb(subcontext, g, thumbBounds)
			subcontext.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintScrollBarBorder(context, g, x, y, w, h, scrollbar.orientation)
		End Sub

		''' <summary>
		''' Paints the scrollbar track.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="trackBounds"> bounding box for the track </param>
		Protected Friend Overridable Sub paintTrack(ByVal context As SynthContext, ByVal g As Graphics, ByVal trackBounds As Rectangle)
			SynthLookAndFeel.updateSubregion(context, g, trackBounds)
			context.painter.paintScrollBarTrackBackground(context, g, trackBounds.x, trackBounds.y, trackBounds.width, trackBounds.height, scrollbar.orientation)
			context.painter.paintScrollBarTrackBorder(context, g, trackBounds.x, trackBounds.y, trackBounds.width, trackBounds.height, scrollbar.orientation)
		End Sub

		''' <summary>
		''' Paints the scrollbar thumb.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="thumbBounds"> bounding box for the thumb </param>
		Protected Friend Overridable Sub paintThumb(ByVal context As SynthContext, ByVal g As Graphics, ByVal thumbBounds As Rectangle)
			SynthLookAndFeel.updateSubregion(context, g, thumbBounds)
			Dim orientation As Integer = scrollbar.orientation
			context.painter.paintScrollBarThumbBackground(context, g, thumbBounds.x, thumbBounds.y, thumbBounds.width, thumbBounds.height, orientation)
			context.painter.paintScrollBarThumbBorder(context, g, thumbBounds.x, thumbBounds.y, thumbBounds.width, thumbBounds.height, orientation)
		End Sub

		''' <summary>
		''' A vertical scrollbar's preferred width is the maximum of
		''' preferred widths of the (non <code>null</code>)
		''' increment/decrement buttons,
		''' and the minimum width of the thumb. The preferred height is the
		''' sum of the preferred heights of the same parts.  The basis for
		''' the preferred size of a horizontal scrollbar is similar.
		''' <p>
		''' The <code>preferredSize</code> is only computed once, subsequent
		''' calls to this method just return a cached size.
		''' </summary>
		''' <param name="c"> the <code>JScrollBar</code> that's delegating this method to us </param>
		''' <returns> the preferred size of a Basic JScrollBar </returns>
		''' <seealso cref= #getMaximumSize </seealso>
		''' <seealso cref= #getMinimumSize </seealso>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim insets As Insets = c.insets
			Return If(scrollbar.orientation = JScrollBar.VERTICAL, New Dimension(scrollBarWidth + insets.left + insets.right, 48), New Dimension(48, scrollBarWidth + insets.top + insets.bottom))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides minimumThumbSize As Dimension
			Get
				If Not validMinimumThumbSize Then
					If scrollbar.orientation = JScrollBar.VERTICAL Then
						minimumThumbSize.width = scrollBarWidth
						minimumThumbSize.height = 7
					Else
						minimumThumbSize.width = 7
						minimumThumbSize.height = scrollBarWidth
					End If
				End If
				Return minimumThumbSize
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createDecreaseButton(ByVal orientation As Integer) As JButton
			Dim synthArrowButton As SynthArrowButton = New SynthArrowButtonAnonymousInnerClassHelper
			synthArrowButton.name = "ScrollBar.button"
			Return synthArrowButton
		End Function

		Private Class SynthArrowButtonAnonymousInnerClassHelper
			Inherits SynthArrowButton

			Public Overrides Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
				If outerInstance.decrGap < 0 Then 'there is an overlap between the track and button
					Dim width As Integer = width
					Dim height As Integer = height
					If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL Then
						'adjust the height by decrGap
						'Note: decrGap is negative!
						height += outerInstance.decrGap
					Else
						'adjust the width by decrGap
						'Note: decrGap is negative!
						width += outerInstance.decrGap
					End If
					Return (x >= 0) AndAlso (x < width) AndAlso (y >= 0) AndAlso (y < height)
				End If
				Return MyBase.contains(x, y)
			End Function
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createIncreaseButton(ByVal orientation As Integer) As JButton
			Dim synthArrowButton As SynthArrowButton = New SynthArrowButtonAnonymousInnerClassHelper2
			synthArrowButton.name = "ScrollBar.button"
			Return synthArrowButton
		End Function

		Private Class SynthArrowButtonAnonymousInnerClassHelper2
			Inherits SynthArrowButton

			Public Overrides Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
				If outerInstance.incrGap < 0 Then 'there is an overlap between the track and button
					Dim width As Integer = width
					Dim height As Integer = height
					If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL Then
						'adjust the height and y by incrGap
						'Note: incrGap is negative!
						height += outerInstance.incrGap
						y += outerInstance.incrGap
					Else
						'adjust the width and x by incrGap
						'Note: incrGap is negative!
						width += outerInstance.incrGap
						x += outerInstance.incrGap
					End If
					Return (x >= 0) AndAlso (x < width) AndAlso (y >= 0) AndAlso (y < height)
				End If
				Return MyBase.contains(x, y)
			End Function
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Property thumbRollover As Boolean
			Set(ByVal active As Boolean)
				If thumbRollover <> active Then
					scrollbar.repaint(thumbBounds)
					MyBase.thumbRollover = active
				End If
			End Set
		End Property

		Private Sub updateButtonDirections()
			Dim orient As Integer = scrollbar.orientation
			If scrollbar.componentOrientation.leftToRight Then
				CType(incrButton, SynthArrowButton).direction = If(orient = HORIZONTAL, EAST, SOUTH)
				CType(decrButton, SynthArrowButton).direction = If(orient = HORIZONTAL, WEST, NORTH)
			Else
				CType(incrButton, SynthArrowButton).direction = If(orient = HORIZONTAL, WEST, SOUTH)
				CType(decrButton, SynthArrowButton).direction = If(orient = HORIZONTAL, EAST, NORTH)
			End If
		End Sub

		'
		' PropertyChangeListener
		'
		Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
			Dim propertyName As String = e.propertyName

			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JScrollBar))

			If "orientation" = propertyName Then
				updateButtonDirections()
			ElseIf "componentOrientation" = propertyName Then
				updateButtonDirections()
			End If
		End Sub
	End Class

End Namespace