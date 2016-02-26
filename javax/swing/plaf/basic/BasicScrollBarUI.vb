Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
import static sun.swing.SwingUtilities2.drawHLine
import static sun.swing.SwingUtilities2.drawRect
import static sun.swing.SwingUtilities2.drawVLine

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.basic






	''' <summary>
	''' Implementation of ScrollBarUI for the Basic Look and Feel
	''' 
	''' @author Rich Schiavi
	''' @author David Kloba
	''' @author Hans Muller
	''' </summary>
	Public Class BasicScrollBarUI
		Inherits ScrollBarUI
		Implements LayoutManager, SwingConstants

		Private Const POSITIVE_SCROLL As Integer = 1
		Private Const NEGATIVE_SCROLL As Integer = -1

		Private Const MIN_SCROLL As Integer = 2
		Private Const MAX_SCROLL As Integer = 3

		' NOTE: DO NOT use this field directly, SynthScrollBarUI assumes you'll
		' call getMinimumThumbSize to access it.
		Protected Friend minimumThumbSize As Dimension
		Protected Friend maximumThumbSize As Dimension

		Protected Friend thumbHighlightColor As Color
		Protected Friend thumbLightShadowColor As Color
		Protected Friend thumbDarkShadowColor As Color
		Protected Friend thumbColor As Color
		Protected Friend trackColor As Color
		Protected Friend trackHighlightColor As Color

		Protected Friend scrollbar As JScrollBar
		Protected Friend incrButton As JButton
		Protected Friend decrButton As JButton
		Protected Friend isDragging As Boolean
		Protected Friend trackListener As TrackListener
		Protected Friend buttonListener As ArrowButtonListener
		Protected Friend modelListener As ModelListener

		Protected Friend thumbRect As Rectangle
		Protected Friend trackRect As Rectangle

		Protected Friend trackHighlight As Integer

		Protected Friend Const NO_HIGHLIGHT As Integer = 0
		Protected Friend Const DECREASE_HIGHLIGHT As Integer = 1
		Protected Friend Const INCREASE_HIGHLIGHT As Integer = 2

		Protected Friend scrollListener As ScrollListener
		Protected Friend propertyChangeListener As PropertyChangeListener
		Protected Friend scrollTimer As Timer

		Private Const scrollSpeedThrottle As Integer = 60 ' delay in milli seconds

		''' <summary>
		''' True indicates a middle click will absolutely position the
		''' scrollbar. 
		''' </summary>
		Private supportsAbsolutePositioning As Boolean

		''' <summary>
		''' Hint as to what width (when vertical) or height (when horizontal)
		''' should be.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend scrollBarWidth As Integer

		Private handler As Handler

		Private thumbActive As Boolean

		''' <summary>
		''' Determine whether scrollbar layout should use cached value or adjusted
		''' value returned by scrollbar's <code>getValue</code>.
		''' </summary>
		Private useCachedValue As Boolean = False
		''' <summary>
		''' The scrollbar value is cached to save real value if the view is adjusted.
		''' </summary>
		Private scrollBarValue As Integer

		''' <summary>
		''' Distance between the increment button and the track. This may be a negative
		''' number. If negative, then an overlap between the button and track will occur,
		''' which is useful for shaped buttons.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend incrGap As Integer

		''' <summary>
		''' Distance between the decrement button and the track. This may be a negative
		''' number. If negative, then an overlap between the button and track will occur,
		''' which is useful for shaped buttons.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend decrGap As Integer

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.POSITIVE_UNIT_INCREMENT))
			map.put(New Actions(Actions.POSITIVE_BLOCK_INCREMENT))
			map.put(New Actions(Actions.NEGATIVE_UNIT_INCREMENT))
			map.put(New Actions(Actions.NEGATIVE_BLOCK_INCREMENT))
			map.put(New Actions(Actions.MIN_SCROLL))
			map.put(New Actions(Actions.MAX_SCROLL))
		End Sub


		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicScrollBarUI
		End Function


		Protected Friend Overridable Sub configureScrollBarColors()
			LookAndFeel.installColors(scrollbar, "ScrollBar.background", "ScrollBar.foreground")
			thumbHighlightColor = UIManager.getColor("ScrollBar.thumbHighlight")
			thumbLightShadowColor = UIManager.getColor("ScrollBar.thumbShadow")
			thumbDarkShadowColor = UIManager.getColor("ScrollBar.thumbDarkShadow")
			thumbColor = UIManager.getColor("ScrollBar.thumb")
			trackColor = UIManager.getColor("ScrollBar.track")
			trackHighlightColor = UIManager.getColor("ScrollBar.trackHighlight")
		End Sub


		Public Overridable Sub installUI(ByVal c As JComponent)
			scrollbar = CType(c, JScrollBar)
			thumbRect = New Rectangle(0, 0, 0, 0)
			trackRect = New Rectangle(0, 0, 0, 0)
			installDefaults()
			installComponents()
			installListeners()
			installKeyboardActions()
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			scrollbar = CType(c, JScrollBar)
			uninstallListeners()
			uninstallDefaults()
			uninstallComponents()
			uninstallKeyboardActions()
			thumbRect = Nothing
			scrollbar = Nothing
			incrButton = Nothing
			decrButton = Nothing
		End Sub


		Protected Friend Overridable Sub installDefaults()
			scrollBarWidth = UIManager.getInt("ScrollBar.width")
			If scrollBarWidth <= 0 Then scrollBarWidth = 16
			minimumThumbSize = CType(UIManager.get("ScrollBar.minimumThumbSize"), Dimension)
			maximumThumbSize = CType(UIManager.get("ScrollBar.maximumThumbSize"), Dimension)

			Dim absB As Boolean? = CBool(UIManager.get("ScrollBar.allowsAbsolutePositioning"))
			supportsAbsolutePositioning = If(absB IsNot Nothing, absB, False)

			trackHighlight = NO_HIGHLIGHT
			If scrollbar.layout Is Nothing OrElse (TypeOf scrollbar.layout Is UIResource) Then scrollbar.layout = Me
			configureScrollBarColors()
			LookAndFeel.installBorder(scrollbar, "ScrollBar.border")
			LookAndFeel.installProperty(scrollbar, "opaque", Boolean.TRUE)

			scrollBarValue = scrollbar.value

			incrGap = UIManager.getInt("ScrollBar.incrementButtonGap")
			decrGap = UIManager.getInt("ScrollBar.decrementButtonGap")

			' TODO this can be removed when incrGap/decrGap become protected
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
					decrGap *= 0.714
				ElseIf "mini".Equals(scaleKey) Then
					scrollBarWidth *= 0.714
					incrGap *= 0.714
					decrGap *= 0.714
				End If
			End If
		End Sub


		Protected Friend Overridable Sub installComponents()
			Select Case scrollbar.orientation
			Case JScrollBar.VERTICAL
				incrButton = createIncreaseButton(SOUTH)
				decrButton = createDecreaseButton(NORTH)

			Case JScrollBar.HORIZONTAL
				If scrollbar.componentOrientation.leftToRight Then
					incrButton = createIncreaseButton(EAST)
					decrButton = createDecreaseButton(WEST)
				Else
					incrButton = createIncreaseButton(WEST)
					decrButton = createDecreaseButton(EAST)
				End If
			End Select
			scrollbar.add(incrButton)
			scrollbar.add(decrButton)
			' Force the children's enabled state to be updated.
			scrollbar.enabled = scrollbar.enabled
		End Sub

		Protected Friend Overridable Sub uninstallComponents()
			scrollbar.remove(incrButton)
			scrollbar.remove(decrButton)
		End Sub


		Protected Friend Overridable Sub installListeners()
			trackListener = createTrackListener()
			buttonListener = createArrowButtonListener()
			modelListener = createModelListener()
			propertyChangeListener = createPropertyChangeListener()

			scrollbar.addMouseListener(trackListener)
			scrollbar.addMouseMotionListener(trackListener)
			scrollbar.model.addChangeListener(modelListener)
			scrollbar.addPropertyChangeListener(propertyChangeListener)
			scrollbar.addFocusListener(handler)

			If incrButton IsNot Nothing Then incrButton.addMouseListener(buttonListener)
			If decrButton IsNot Nothing Then decrButton.addMouseListener(buttonListener)

			scrollListener = createScrollListener()
			scrollTimer = New Timer(scrollSpeedThrottle, scrollListener)
			scrollTimer.initialDelay = 300 ' default InitialDelay?
		End Sub


		Protected Friend Overridable Sub installKeyboardActions()
			LazyActionMap.installLazyActionMap(scrollbar, GetType(BasicScrollBarUI), "ScrollBar.actionMap")

			Dim ___inputMap As InputMap = getInputMap(JComponent.WHEN_FOCUSED)
			SwingUtilities.replaceUIInputMap(scrollbar, JComponent.WHEN_FOCUSED, ___inputMap)
			___inputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)
			SwingUtilities.replaceUIInputMap(scrollbar, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, ___inputMap)
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(scrollbar, JComponent.WHEN_FOCUSED, Nothing)
			SwingUtilities.replaceUIActionMap(scrollbar, Nothing)
		End Sub

		Private Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_FOCUSED Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(scrollbar, Me, "ScrollBar.focusInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(scrollbar, Me, "ScrollBar.focusInputMap.RightToLeft"), InputMap)
				If scrollbar.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			ElseIf condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(scrollbar, Me, "ScrollBar.ancestorInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(scrollbar, Me, "ScrollBar.ancestorInputMap.RightToLeft"), InputMap)
				If scrollbar.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function


		Protected Friend Overridable Sub uninstallListeners()
			scrollTimer.stop()
			scrollTimer = Nothing

			If decrButton IsNot Nothing Then decrButton.removeMouseListener(buttonListener)
			If incrButton IsNot Nothing Then incrButton.removeMouseListener(buttonListener)

			scrollbar.model.removeChangeListener(modelListener)
			scrollbar.removeMouseListener(trackListener)
			scrollbar.removeMouseMotionListener(trackListener)
			scrollbar.removePropertyChangeListener(propertyChangeListener)
			scrollbar.removeFocusListener(handler)
			handler = Nothing
		End Sub


		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(scrollbar)
			If scrollbar.layout Is Me Then scrollbar.layout = Nothing
		End Sub


		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Function createTrackListener() As TrackListener
			Return New TrackListener(Me)
		End Function

		Protected Friend Overridable Function createArrowButtonListener() As ArrowButtonListener
			Return New ArrowButtonListener(Me)
		End Function

		Protected Friend Overridable Function createModelListener() As ModelListener
			Return New ModelListener(Me)
		End Function

		Protected Friend Overridable Function createScrollListener() As ScrollListener
			Return New ScrollListener(Me)
		End Function

		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function

		Private Sub updateThumbState(ByVal x As Integer, ByVal y As Integer)
			Dim rect As Rectangle = thumbBounds

			thumbRollover = rect.contains(x, y)
		End Sub

		''' <summary>
		''' Sets whether or not the mouse is currently over the thumb.
		''' </summary>
		''' <param name="active"> True indicates the thumb is currently active.
		''' @since 1.5 </param>
		Protected Friend Overridable Property thumbRollover As Boolean
			Set(ByVal active As Boolean)
				If thumbActive <> active Then
					thumbActive = active
					scrollbar.repaint(thumbBounds)
				End If
			End Set
			Get
				Return thumbActive
			End Get
		End Property


		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			paintTrack(g, c, trackBounds)
			Dim ___thumbBounds As Rectangle = thumbBounds
			If ___thumbBounds.intersects(g.clipBounds) Then paintThumb(g, c, ___thumbBounds)
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
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return If(scrollbar.orientation = JScrollBar.VERTICAL, New Dimension(scrollBarWidth, 48), New Dimension(48, scrollBarWidth))
		End Function


		''' <param name="c"> The JScrollBar that's delegating this method to us. </param>
		''' <returns> new Dimension(Integer.MAX_VALUE, Integer.MAX_VALUE); </returns>
		''' <seealso cref= #getMinimumSize </seealso>
		''' <seealso cref= #getPreferredSize </seealso>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Return New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function

		Protected Friend Overridable Function createDecreaseButton(ByVal orientation As Integer) As JButton
			Return New BasicArrowButton(orientation, UIManager.getColor("ScrollBar.thumb"), UIManager.getColor("ScrollBar.thumbShadow"), UIManager.getColor("ScrollBar.thumbDarkShadow"), UIManager.getColor("ScrollBar.thumbHighlight"))
		End Function

		Protected Friend Overridable Function createIncreaseButton(ByVal orientation As Integer) As JButton
			Return New BasicArrowButton(orientation, UIManager.getColor("ScrollBar.thumb"), UIManager.getColor("ScrollBar.thumbShadow"), UIManager.getColor("ScrollBar.thumbDarkShadow"), UIManager.getColor("ScrollBar.thumbHighlight"))
		End Function


		Protected Friend Overridable Sub paintDecreaseHighlight(ByVal g As Graphics)
			Dim insets As Insets = scrollbar.insets
			Dim thumbR As Rectangle = thumbBounds
			g.color = trackHighlightColor

			If scrollbar.orientation = JScrollBar.VERTICAL Then
				'paint the distance between the start of the track and top of the thumb
				Dim x As Integer = insets.left
				Dim y As Integer = trackRect.y
				Dim w As Integer = scrollbar.width - (insets.left + insets.right)
				Dim h As Integer = thumbR.y - y
				g.fillRect(x, y, w, h)
			Else
				'if left-to-right, fill the area between the start of the track and
				'the left edge of the thumb. If right-to-left, fill the area between
				'the end of the thumb and end of the track.
				Dim x, w As Integer
				If scrollbar.componentOrientation.leftToRight Then
				   x = trackRect.x
					w = thumbR.x - x
				Else
					x = thumbR.x + thumbR.width
					w = trackRect.x + trackRect.width - x
				End If
				Dim y As Integer = insets.top
				Dim h As Integer = scrollbar.height - (insets.top + insets.bottom)
				g.fillRect(x, y, w, h)
			End If
		End Sub


		Protected Friend Overridable Sub paintIncreaseHighlight(ByVal g As Graphics)
			Dim insets As Insets = scrollbar.insets
			Dim thumbR As Rectangle = thumbBounds
			g.color = trackHighlightColor

			If scrollbar.orientation = JScrollBar.VERTICAL Then
				'fill the area between the bottom of the thumb and the end of the track.
				Dim x As Integer = insets.left
				Dim y As Integer = thumbR.y + thumbR.height
				Dim w As Integer = scrollbar.width - (insets.left + insets.right)
				Dim h As Integer = trackRect.y + trackRect.height - y
				g.fillRect(x, y, w, h)
			Else
				'if left-to-right, fill the area between the right of the thumb and the
				'end of the track. If right-to-left, then fill the area to the left of
				'the thumb and the start of the track.
				Dim x, w As Integer
				If scrollbar.componentOrientation.leftToRight Then
					x = thumbR.x + thumbR.width
					w = trackRect.x + trackRect.width - x
				Else
					x = trackRect.x
					w = thumbR.x - x
				End If
				Dim y As Integer = insets.top
				Dim h As Integer = scrollbar.height - (insets.top + insets.bottom)
				g.fillRect(x, y, w, h)
			End If
		End Sub


		Protected Friend Overridable Sub paintTrack(ByVal g As Graphics, ByVal c As JComponent, ByVal trackBounds As Rectangle)
			g.color = trackColor
			g.fillRect(trackBounds.x, trackBounds.y, trackBounds.width, trackBounds.height)

			If trackHighlight = DECREASE_HIGHLIGHT Then
				paintDecreaseHighlight(g)
			ElseIf trackHighlight = INCREASE_HIGHLIGHT Then
				paintIncreaseHighlight(g)
			End If
		End Sub


		Protected Friend Overridable Sub paintThumb(ByVal g As Graphics, ByVal c As JComponent, ByVal thumbBounds As Rectangle)
			If thumbBounds.empty OrElse (Not scrollbar.enabled) Then Return

			Dim w As Integer = thumbBounds.width
			Dim h As Integer = thumbBounds.height

			g.translate(thumbBounds.x, thumbBounds.y)

			g.color = thumbDarkShadowColor
			drawRect(g, 0, 0, w - 1, h - 1)
			g.color = thumbColor
			g.fillRect(0, 0, w - 1, h - 1)

			g.color = thumbHighlightColor
			drawVLine(g, 1, 1, h - 2)
			drawHLine(g, 2, w - 3, 1)

			g.color = thumbLightShadowColor
			drawHLine(g, 2, w - 2, h - 2)
			drawVLine(g, w - 2, 1, h - 3)

			g.translate(-thumbBounds.x, -thumbBounds.y)
		End Sub


		''' <summary>
		''' Returns the smallest acceptable size for the thumb.  If the scrollbar
		''' becomes so small that this size isn't available, the thumb will be
		''' hidden.
		''' <p>
		''' <b>Warning </b>: the value returned by this method should not be
		''' be modified, it's a shared static constant.
		''' </summary>
		''' <returns> The smallest acceptable size for the thumb. </returns>
		''' <seealso cref= #getMaximumThumbSize </seealso>
		Protected Friend Overridable Property minimumThumbSize As Dimension
			Get
				Return minimumThumbSize
			End Get
		End Property

		''' <summary>
		''' Returns the largest acceptable size for the thumb.  To create a fixed
		''' size thumb one make this method and <code>getMinimumThumbSize</code>
		''' return the same value.
		''' <p>
		''' <b>Warning </b>: the value returned by this method should not be
		''' be modified, it's a shared static constant.
		''' </summary>
		''' <returns> The largest acceptable size for the thumb. </returns>
		''' <seealso cref= #getMinimumThumbSize </seealso>
		Protected Friend Overridable Property maximumThumbSize As Dimension
			Get
				Return maximumThumbSize
			End Get
		End Property


	'    
	'     * LayoutManager Implementation
	'     

		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal child As Component)
		End Sub
		Public Overridable Sub removeLayoutComponent(ByVal child As Component)
		End Sub

		Public Overridable Function preferredLayoutSize(ByVal scrollbarContainer As Container) As Dimension
			Return getPreferredSize(CType(scrollbarContainer, JComponent))
		End Function

		Public Overridable Function minimumLayoutSize(ByVal scrollbarContainer As Container) As Dimension
			Return getMinimumSize(CType(scrollbarContainer, JComponent))
		End Function

		Private Function getValue(ByVal sb As JScrollBar) As Integer
			Return If(useCachedValue, scrollBarValue, sb.value)
		End Function

		Protected Friend Overridable Sub layoutVScrollbar(ByVal sb As JScrollBar)
			Dim sbSize As Dimension = sb.size
			Dim sbInsets As Insets = sb.insets

	'        
	'         * Width and left edge of the buttons and thumb.
	'         
			Dim itemW As Integer = sbSize.width - (sbInsets.left + sbInsets.right)
			Dim itemX As Integer = sbInsets.left

	'         Nominal locations of the buttons, assuming their preferred
	'         * size will fit.
	'         
			Dim squareButtons As Boolean = sun.swing.DefaultLookup.getBoolean(scrollbar, Me, "ScrollBar.squareButtons", False)
			Dim decrButtonH As Integer = If(squareButtons, itemW, decrButton.preferredSize.height)
			Dim decrButtonY As Integer = sbInsets.top

			Dim incrButtonH As Integer = If(squareButtons, itemW, incrButton.preferredSize.height)
			Dim incrButtonY As Integer = sbSize.height - (sbInsets.bottom + incrButtonH)

	'         The thumb must fit within the height left over after we
	'         * subtract the preferredSize of the buttons and the insets
	'         * and the gaps
	'         
			Dim sbInsetsH As Integer = sbInsets.top + sbInsets.bottom
			Dim sbButtonsH As Integer = decrButtonH + incrButtonH
			Dim gaps As Integer = decrGap + incrGap
			Dim trackH As Single = sbSize.height - (sbInsetsH + sbButtonsH) - gaps

	'         Compute the height and origin of the thumb.   The case
	'         * where the thumb is at the bottom edge is handled specially
	'         * to avoid numerical problems in computing thumbY.  Enforce
	'         * the thumbs min/max dimensions.  If the thumb doesn't
	'         * fit in the track (trackH) we'll hide it later.
	'         
			Dim min As Single = sb.minimum
			Dim extent As Single = sb.visibleAmount
			Dim range As Single = sb.maximum - min
			Dim ___value As Single = getValue(sb)

			Dim thumbH As Integer = If(range <= 0, maximumThumbSize.height, CInt(Fix(trackH * (extent / range))))
			thumbH = Math.Max(thumbH, minimumThumbSize.height)
			thumbH = Math.Min(thumbH, maximumThumbSize.height)

			Dim thumbY As Integer = incrButtonY - incrGap - thumbH
			If ___value < (sb.maximum - sb.visibleAmount) Then
				Dim thumbRange As Single = trackH - thumbH
				thumbY = CInt(Fix(0.5f + (thumbRange * ((___value - min) / (range - extent)))))
				thumbY += decrButtonY + decrButtonH + decrGap
			End If

	'         If the buttons don't fit, allocate half of the available
	'         * space to each and move the lower one (incrButton) down.
	'         
			Dim sbAvailButtonH As Integer = (sbSize.height - sbInsetsH)
			If sbAvailButtonH < sbButtonsH Then
					decrButtonH = sbAvailButtonH \ 2
					incrButtonH = decrButtonH
				incrButtonY = sbSize.height - (sbInsets.bottom + incrButtonH)
			End If
			decrButton.boundsnds(itemX, decrButtonY, itemW, decrButtonH)
			incrButton.boundsnds(itemX, incrButtonY, itemW, incrButtonH)

	'         Update the trackRect field.
	'         
			Dim itrackY As Integer = decrButtonY + decrButtonH + decrGap
			Dim itrackH As Integer = incrButtonY - incrGap - itrackY
			trackRect.boundsnds(itemX, itrackY, itemW, itrackH)

	'         If the thumb isn't going to fit, zero it's bounds.  Otherwise
	'         * make sure it fits between the buttons.  Note that setting the
	'         * thumbs bounds will cause a repaint.
	'         
			If thumbH >= CInt(Fix(trackH)) Then
				If UIManager.getBoolean("ScrollBar.alwaysShowThumb") Then
					' This is used primarily for GTK L&F, which expands the
					' thumb to fit the track when it would otherwise be hidden.
					thumbBoundsnds(itemX, itrackY, itemW, itrackH)
				Else
					' Other L&F's simply hide the thumb in this case.
					thumbBoundsnds(0, 0, 0, 0)
				End If
			Else
				If (thumbY + thumbH) > incrButtonY - incrGap Then thumbY = incrButtonY - incrGap - thumbH
				If thumbY < (decrButtonY + decrButtonH + decrGap) Then thumbY = decrButtonY + decrButtonH + decrGap + 1
				thumbBoundsnds(itemX, thumbY, itemW, thumbH)
			End If
		End Sub


		Protected Friend Overridable Sub layoutHScrollbar(ByVal sb As JScrollBar)
			Dim sbSize As Dimension = sb.size
			Dim sbInsets As Insets = sb.insets

	'         Height and top edge of the buttons and thumb.
	'         
			Dim itemH As Integer = sbSize.height - (sbInsets.top + sbInsets.bottom)
			Dim itemY As Integer = sbInsets.top

			Dim ltr As Boolean = sb.componentOrientation.leftToRight

	'         Nominal locations of the buttons, assuming their preferred
	'         * size will fit.
	'         
			Dim squareButtons As Boolean = sun.swing.DefaultLookup.getBoolean(scrollbar, Me, "ScrollBar.squareButtons", False)
			Dim leftButtonW As Integer = If(squareButtons, itemH, decrButton.preferredSize.width)
			Dim rightButtonW As Integer = If(squareButtons, itemH, incrButton.preferredSize.width)
			If Not ltr Then
				Dim temp As Integer = leftButtonW
				leftButtonW = rightButtonW
				rightButtonW = temp
			End If
			Dim leftButtonX As Integer = sbInsets.left
			Dim rightButtonX As Integer = sbSize.width - (sbInsets.right + rightButtonW)
			Dim leftGap As Integer = If(ltr, decrGap, incrGap)
			Dim rightGap As Integer = If(ltr, incrGap, decrGap)

	'         The thumb must fit within the width left over after we
	'         * subtract the preferredSize of the buttons and the insets
	'         * and the gaps
	'         
			Dim sbInsetsW As Integer = sbInsets.left + sbInsets.right
			Dim sbButtonsW As Integer = leftButtonW + rightButtonW
			Dim trackW As Single = sbSize.width - (sbInsetsW + sbButtonsW) - (leftGap + rightGap)

	'         Compute the width and origin of the thumb.  Enforce
	'         * the thumbs min/max dimensions.  The case where the thumb
	'         * is at the right edge is handled specially to avoid numerical
	'         * problems in computing thumbX.  If the thumb doesn't
	'         * fit in the track (trackH) we'll hide it later.
	'         
			Dim min As Single = sb.minimum
			Dim max As Single = sb.maximum
			Dim extent As Single = sb.visibleAmount
			Dim range As Single = max - min
			Dim ___value As Single = getValue(sb)

			Dim thumbW As Integer = If(range <= 0, maximumThumbSize.width, CInt(Fix(trackW * (extent / range))))
			thumbW = Math.Max(thumbW, minimumThumbSize.width)
			thumbW = Math.Min(thumbW, maximumThumbSize.width)

			Dim thumbX As Integer = If(ltr, rightButtonX - rightGap - thumbW, leftButtonX + leftButtonW + leftGap)
			If ___value < (max - sb.visibleAmount) Then
				Dim thumbRange As Single = trackW - thumbW
				If ltr Then
					thumbX = CInt(Fix(0.5f + (thumbRange * ((___value - min) / (range - extent)))))
				Else
					thumbX = CInt(Fix(0.5f + (thumbRange * ((max - extent - ___value) / (range - extent)))))
				End If
				thumbX += leftButtonX + leftButtonW + leftGap
			End If

	'         If the buttons don't fit, allocate half of the available
	'         * space to each and move the right one over.
	'         
			Dim sbAvailButtonW As Integer = (sbSize.width - sbInsetsW)
			If sbAvailButtonW < sbButtonsW Then
					leftButtonW = sbAvailButtonW \ 2
					rightButtonW = leftButtonW
				rightButtonX = sbSize.width - (sbInsets.right + rightButtonW + rightGap)
			End If

			(If(ltr, decrButton, incrButton)).boundsnds(leftButtonX, itemY, leftButtonW, itemH)
			(If(ltr, incrButton, decrButton)).boundsnds(rightButtonX, itemY, rightButtonW, itemH)

	'         Update the trackRect field.
	'         
			Dim itrackX As Integer = leftButtonX + leftButtonW + leftGap
			Dim itrackW As Integer = rightButtonX - rightGap - itrackX
			trackRect.boundsnds(itrackX, itemY, itrackW, itemH)

	'         Make sure the thumb fits between the buttons.  Note
	'         * that setting the thumbs bounds causes a repaint.
	'         
			If thumbW >= CInt(Fix(trackW)) Then
				If UIManager.getBoolean("ScrollBar.alwaysShowThumb") Then
					' This is used primarily for GTK L&F, which expands the
					' thumb to fit the track when it would otherwise be hidden.
					thumbBoundsnds(itrackX, itemY, itrackW, itemH)
				Else
					' Other L&F's simply hide the thumb in this case.
					thumbBoundsnds(0, 0, 0, 0)
				End If
			Else
				If thumbX + thumbW > rightButtonX - rightGap Then thumbX = rightButtonX - rightGap - thumbW
				If thumbX < leftButtonX + leftButtonW + leftGap Then thumbX = leftButtonX + leftButtonW + leftGap + 1
				thumbBoundsnds(thumbX, itemY, thumbW, itemH)
			End If
		End Sub

		Public Overridable Sub layoutContainer(ByVal scrollbarContainer As Container)
	'         If the user is dragging the value, we'll assume that the
	'         * scrollbars layout is OK modulo the thumb which is being
	'         * handled by the dragging code.
	'         
			If isDragging Then Return

			Dim scrollbar As JScrollBar = CType(scrollbarContainer, JScrollBar)
			Select Case scrollbar.orientation
			Case JScrollBar.VERTICAL
				layoutVScrollbar(scrollbar)

			Case JScrollBar.HORIZONTAL
				layoutHScrollbar(scrollbar)
			End Select
		End Sub


		''' <summary>
		''' Set the bounds of the thumb and force a repaint that includes
		''' the old thumbBounds and the new one.
		''' </summary>
		''' <seealso cref= #getThumbBounds </seealso>
		Protected Friend Overridable Sub setThumbBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
	'         If the thumbs bounds haven't changed, we're done.
	'         
			If (thumbRect.x = x) AndAlso (thumbRect.y = y) AndAlso (thumbRect.width = width) AndAlso (thumbRect.height = height) Then Return

	'         Update thumbRect, and repaint the union of x,y,w,h and
	'         * the old thumbRect.
	'         
			Dim minX As Integer = Math.Min(x, thumbRect.x)
			Dim minY As Integer = Math.Min(y, thumbRect.y)
			Dim maxX As Integer = Math.Max(x + width, thumbRect.x + thumbRect.width)
			Dim maxY As Integer = Math.Max(y + height, thumbRect.y + thumbRect.height)

			thumbRect.boundsnds(x, y, width, height)
			scrollbar.repaint(minX, minY, maxX - minX, maxY - minY)

			' Once there is API to determine the mouse location this will need
			' to be changed.
			thumbRollover = False
		End Sub


		''' <summary>
		''' Return the current size/location of the thumb.
		''' <p>
		''' <b>Warning </b>: the value returned by this method should not be
		''' be modified, it's a reference to the actual rectangle, not a copy.
		''' </summary>
		''' <returns> The current size/location of the thumb. </returns>
		''' <seealso cref= #setThumbBounds </seealso>
		Protected Friend Overridable Property thumbBounds As Rectangle
			Get
				Return thumbRect
			End Get
		End Property


		''' <summary>
		''' Returns the current bounds of the track, i.e. the space in between
		''' the increment and decrement buttons, less the insets.  The value
		''' returned by this method is updated each time the scrollbar is
		''' laid out (validated).
		''' <p>
		''' <b>Warning </b>: the value returned by this method should not be
		''' be modified, it's a reference to the actual rectangle, not a copy.
		''' </summary>
		''' <returns> the current bounds of the scrollbar track </returns>
		''' <seealso cref= #layoutContainer </seealso>
		Protected Friend Overridable Property trackBounds As Rectangle
			Get
				Return trackRect
			End Get
		End Property

	'    
	'     * Method for scrolling by a block increment.
	'     * Added for mouse wheel scrolling support, RFE 4202656.
	'     
		Friend Shared Sub scrollByBlock(ByVal scrollbar As JScrollBar, ByVal direction As Integer)
			' This method is called from BasicScrollPaneUI to implement wheel
			' scrolling, and also from scrollByBlock().
				Dim oldValue As Integer = scrollbar.value
				Dim blockIncrement As Integer = scrollbar.getBlockIncrement(direction)
				Dim delta As Integer = blockIncrement * (If(direction > 0, +1, -1))
				Dim newValue As Integer = oldValue + delta

				' Check for overflow.
				If delta > 0 AndAlso newValue < oldValue Then
					newValue = scrollbar.maximum
				ElseIf delta < 0 AndAlso newValue > oldValue Then
					newValue = scrollbar.minimum
				End If

				scrollbar.value = newValue
		End Sub

		Protected Friend Overridable Sub scrollByBlock(ByVal direction As Integer)
			scrollByBlock(scrollbar, direction)
				trackHighlight = If(direction > 0, INCREASE_HIGHLIGHT, DECREASE_HIGHLIGHT)
				Dim dirtyRect As Rectangle = trackBounds
				scrollbar.repaint(dirtyRect.x, dirtyRect.y, dirtyRect.width, dirtyRect.height)
		End Sub

	'    
	'     * Method for scrolling by a unit increment.
	'     * Added for mouse wheel scrolling support, RFE 4202656.
	'     *
	'     * If limitByBlock is set to true, the scrollbar will scroll at least 1
	'     * unit increment, but will not scroll farther than the block increment.
	'     * See BasicScrollPaneUI.Handler.mouseWheelMoved().
	'     
		Friend Shared Sub scrollByUnits(ByVal scrollbar As JScrollBar, ByVal direction As Integer, ByVal units As Integer, ByVal limitToBlock As Boolean)
			' This method is called from BasicScrollPaneUI to implement wheel
			' scrolling, as well as from scrollByUnit().
			Dim delta As Integer
			Dim limit As Integer = -1

			If limitToBlock Then
				If direction < 0 Then
					limit = scrollbar.value - scrollbar.getBlockIncrement(direction)
				Else
					limit = scrollbar.value + scrollbar.getBlockIncrement(direction)
				End If
			End If

			For i As Integer = 0 To units - 1
				If direction > 0 Then
					delta = scrollbar.getUnitIncrement(direction)
				Else
					delta = -scrollbar.getUnitIncrement(direction)
				End If

				Dim oldValue As Integer = scrollbar.value
				Dim newValue As Integer = oldValue + delta

				' Check for overflow.
				If delta > 0 AndAlso newValue < oldValue Then
					newValue = scrollbar.maximum
				ElseIf delta < 0 AndAlso newValue > oldValue Then
					newValue = scrollbar.minimum
				End If
				If oldValue = newValue Then Exit For

				If limitToBlock AndAlso i > 0 Then
					Debug.Assert(limit <> -1)
					If (direction < 0 AndAlso newValue < limit) OrElse (direction > 0 AndAlso newValue > limit) Then Exit For
				End If
				scrollbar.value = newValue
			Next i
		End Sub

		Protected Friend Overridable Sub scrollByUnit(ByVal direction As Integer)
			scrollByUnits(scrollbar, direction, 1, False)
		End Sub

		''' <summary>
		''' Indicates whether the user can absolutely position the thumb with
		''' a mouse gesture (usually the middle mouse button).
		''' </summary>
		''' <returns> true if a mouse gesture can absolutely position the thumb
		''' @since 1.5 </returns>
		Public Overridable Property supportsAbsolutePositioning As Boolean
			Get
				Return supportsAbsolutePositioning
			End Get
		End Property

		''' <summary>
		''' A listener to listen for model changes.
		''' 
		''' </summary>
		Protected Friend Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicScrollBarUI

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If Not outerInstance.useCachedValue Then outerInstance.scrollBarValue = outerInstance.scrollbar.value
				outerInstance.layoutContainer(outerInstance.scrollbar)
				outerInstance.useCachedValue = False
			End Sub
		End Class


		''' <summary>
		''' Track mouse drags.
		''' </summary>
		Protected Friend Class TrackListener
			Inherits MouseAdapter
			Implements MouseMotionListener

			Private ReadOnly outerInstance As BasicScrollBarUI

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			<NonSerialized> _
			Protected Friend offset As Integer
			<NonSerialized> _
			Protected Friend currentMouseX, currentMouseY As Integer
			<NonSerialized> _
			Private direction As Integer = +1

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If outerInstance.isDragging Then outerInstance.updateThumbState(e.x, e.y)
				If SwingUtilities.isRightMouseButton(e) OrElse ((Not outerInstance.supportsAbsolutePositioning) AndAlso SwingUtilities.isMiddleMouseButton(e)) Then Return
				If Not outerInstance.scrollbar.enabled Then Return

				Dim r As Rectangle = outerInstance.trackBounds
				outerInstance.scrollbar.repaint(r.x, r.y, r.width, r.height)

				outerInstance.trackHighlight = NO_HIGHLIGHT
				outerInstance.isDragging = False
				offset = 0
				outerInstance.scrollTimer.stop()
				outerInstance.useCachedValue = True
				outerInstance.scrollbar.valueIsAdjusting = False
			End Sub


			''' <summary>
			''' If the mouse is pressed above the "thumb" component
			''' then reduce the scrollbars value by one page ("page up"),
			''' otherwise increase it by one page.  If there is no
			''' thumb then page up if the mouse is in the upper half
			''' of the track.
			''' </summary>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If SwingUtilities.isRightMouseButton(e) OrElse ((Not outerInstance.supportsAbsolutePositioning) AndAlso SwingUtilities.isMiddleMouseButton(e)) Then Return
				If Not outerInstance.scrollbar.enabled Then Return

				If (Not outerInstance.scrollbar.hasFocus()) AndAlso outerInstance.scrollbar.requestFocusEnabled Then outerInstance.scrollbar.requestFocus()

				outerInstance.useCachedValue = True
				outerInstance.scrollbar.valueIsAdjusting = True

				currentMouseX = e.x
				currentMouseY = e.y

				' Clicked in the Thumb area?
				If outerInstance.thumbBounds.contains(currentMouseX, currentMouseY) Then
					Select Case outerInstance.scrollbar.orientation
					Case JScrollBar.VERTICAL
						offset = currentMouseY - outerInstance.thumbBounds.y
					Case JScrollBar.HORIZONTAL
						offset = currentMouseX - outerInstance.thumbBounds.x
					End Select
					outerInstance.isDragging = True
					Return
				ElseIf outerInstance.supportsAbsolutePositioning AndAlso SwingUtilities.isMiddleMouseButton(e) Then
					Select Case outerInstance.scrollbar.orientation
					Case JScrollBar.VERTICAL
						offset = outerInstance.thumbBounds.height / 2
					Case JScrollBar.HORIZONTAL
						offset = outerInstance.thumbBounds.width / 2
					End Select
					outerInstance.isDragging = True
					valueFrom = e
					Return
				End If
				outerInstance.isDragging = False

				Dim sbSize As Dimension = outerInstance.scrollbar.size
				direction = +1

				Select Case outerInstance.scrollbar.orientation
				Case JScrollBar.VERTICAL
					If outerInstance.thumbBounds.empty Then
						Dim scrollbarCenter As Integer = sbSize.height / 2
						direction = If(currentMouseY < scrollbarCenter, -1, +1)
					Else
						Dim thumbY As Integer = outerInstance.thumbBounds.y
						direction = If(currentMouseY < thumbY, -1, +1)
					End If
				Case JScrollBar.HORIZONTAL
					If outerInstance.thumbBounds.empty Then
						Dim scrollbarCenter As Integer = sbSize.width / 2
						direction = If(currentMouseX < scrollbarCenter, -1, +1)
					Else
						Dim thumbX As Integer = outerInstance.thumbBounds.x
						direction = If(currentMouseX < thumbX, -1, +1)
					End If
					If Not outerInstance.scrollbar.componentOrientation.leftToRight Then direction = -direction
				End Select
				outerInstance.scrollByBlock(direction)

				outerInstance.scrollTimer.stop()
				outerInstance.scrollListener.direction = direction
				outerInstance.scrollListener.scrollByBlock = True
				startScrollTimerIfNecessary()
			End Sub


			''' <summary>
			''' Set the models value to the position of the thumb's top of Vertical
			''' scrollbar, or the left/right of Horizontal scrollbar in
			''' left-to-right/right-to-left scrollbar relative to the origin of the
			''' track.
			''' </summary>
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				If SwingUtilities.isRightMouseButton(e) OrElse ((Not outerInstance.supportsAbsolutePositioning) AndAlso SwingUtilities.isMiddleMouseButton(e)) Then Return
				If (Not outerInstance.scrollbar.enabled) OrElse outerInstance.thumbBounds.empty Then Return
				If outerInstance.isDragging Then
					valueFrom = e
				Else
					currentMouseX = e.x
					currentMouseY = e.y
					outerInstance.updateThumbState(currentMouseX, currentMouseY)
					startScrollTimerIfNecessary()
				End If
			End Sub

			Private Property valueFrom As MouseEvent
				Set(ByVal e As MouseEvent)
					Dim active As Boolean = outerInstance.thumbRollover
					Dim model As BoundedRangeModel = outerInstance.scrollbar.model
					Dim thumbR As Rectangle = outerInstance.thumbBounds
					Dim trackLength As Single
					Dim thumbMin, thumbMax, thumbPos As Integer
    
					If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL Then
						thumbMin = outerInstance.trackRect.y
						thumbMax = outerInstance.trackRect.y + outerInstance.trackRect.height - thumbR.height
						thumbPos = Math.Min(thumbMax, Math.Max(thumbMin, (e.y - offset)))
						outerInstance.thumbBoundsnds(thumbR.x, thumbPos, thumbR.width, thumbR.height)
						trackLength = outerInstance.trackBounds.height
					Else
						thumbMin = outerInstance.trackRect.x
						thumbMax = outerInstance.trackRect.x + outerInstance.trackRect.width - thumbR.width
						thumbPos = Math.Min(thumbMax, Math.Max(thumbMin, (e.x - offset)))
						outerInstance.thumbBoundsnds(thumbPos, thumbR.y, thumbR.width, thumbR.height)
						trackLength = outerInstance.trackBounds.width
					End If
    
		'             Set the scrollbars value.  If the thumb has reached the end of
		'             * the scrollbar, then just set the value to its maximum.  Otherwise
		'             * compute the value as accurately as possible.
		'             
					If thumbPos = thumbMax Then
						If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL OrElse outerInstance.scrollbar.componentOrientation.leftToRight Then
							outerInstance.scrollbar.value = model.maximum - model.extent
						Else
							outerInstance.scrollbar.value = model.minimum
						End If
					Else
						Dim valueMax As Single = model.maximum - model.extent
						Dim valueRange As Single = valueMax - model.minimum
						Dim thumbValue As Single = thumbPos - thumbMin
						Dim thumbRange As Single = thumbMax - thumbMin
						Dim value As Integer
						If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL OrElse outerInstance.scrollbar.componentOrientation.leftToRight Then
							value = CInt(Fix(0.5 + ((thumbValue / thumbRange) * valueRange)))
						Else
							value = CInt(Fix(0.5 + (((thumbMax - thumbPos) / thumbRange) * valueRange)))
						End If
    
						outerInstance.useCachedValue = True
						outerInstance.scrollBarValue = value + model.minimum
						outerInstance.scrollbar.value = adjustValueIfNecessary(outerInstance.scrollBarValue)
					End If
					outerInstance.thumbRollover = active
				End Set
			End Property

			Private Function adjustValueIfNecessary(ByVal value As Integer) As Integer
				If TypeOf outerInstance.scrollbar.parent Is JScrollPane Then
					Dim scrollpane As JScrollPane = CType(outerInstance.scrollbar.parent, JScrollPane)
					Dim viewport As JViewport = scrollpane.viewport
					Dim view As Component = viewport.view
					If TypeOf view Is JList Then
						Dim list As JList = CType(view, JList)
						If sun.swing.DefaultLookup.getBoolean(list, list.uI, "List.lockToPositionOnScroll", False) Then
							Dim adjustedValue As Integer = value
							Dim mode As Integer = list.layoutOrientation
							Dim orientation As Integer = outerInstance.scrollbar.orientation
							If orientation = JScrollBar.VERTICAL AndAlso mode = JList.VERTICAL Then
								Dim index As Integer = list.locationToIndex(New Point(0, value))
								Dim rect As Rectangle = list.getCellBounds(index, index)
								If rect IsNot Nothing Then adjustedValue = rect.y
							End If
							If orientation = JScrollBar.HORIZONTAL AndAlso (mode = JList.VERTICAL_WRAP OrElse mode = JList.HORIZONTAL_WRAP) Then
								If scrollpane.componentOrientation.leftToRight Then
									Dim index As Integer = list.locationToIndex(New Point(value, 0))
									Dim rect As Rectangle = list.getCellBounds(index, index)
									If rect IsNot Nothing Then adjustedValue = rect.x
								Else
									Dim loc As New Point(value, 0)
									Dim extent As Integer = viewport.extentSize.width
									loc.x += extent - 1
									Dim index As Integer = list.locationToIndex(loc)
									Dim rect As Rectangle = list.getCellBounds(index, index)
									If rect IsNot Nothing Then adjustedValue = rect.x + rect.width - extent
								End If
							End If
							value = adjustedValue

						End If
					End If
				End If
				Return value
			End Function

			Private Sub startScrollTimerIfNecessary()
				If outerInstance.scrollTimer.running Then Return

				Dim tb As Rectangle = outerInstance.thumbBounds

				Select Case outerInstance.scrollbar.orientation
				Case JScrollBar.VERTICAL
					If direction > 0 Then
						If tb.y + tb.height < outerInstance.trackListener.currentMouseY Then outerInstance.scrollTimer.start()
					ElseIf tb.y > outerInstance.trackListener.currentMouseY Then
						outerInstance.scrollTimer.start()
					End If
				Case JScrollBar.HORIZONTAL
					If (direction > 0 AndAlso outerInstance.mouseAfterThumb) OrElse (direction < 0 AndAlso outerInstance.mouseBeforeThumb) Then outerInstance.scrollTimer.start()
				End Select
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				If Not outerInstance.isDragging Then outerInstance.updateThumbState(e.x, e.y)
			End Sub

			''' <summary>
			''' Invoked when the mouse exits the scrollbar.
			''' </summary>
			''' <param name="e"> MouseEvent further describing the event
			''' @since 1.5 </param>
			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				If Not outerInstance.isDragging Then outerInstance.thumbRollover = False
			End Sub
		End Class


		''' <summary>
		''' Listener for cursor keys.
		''' </summary>
		Protected Friend Class ArrowButtonListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As BasicScrollBarUI

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' Because we are handling both mousePressed and Actions
			' we need to make sure we don't fire under both conditions.
			' (keyfocus on scrollbars causes action without mousePress
			Friend handledEvent As Boolean

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If Not outerInstance.scrollbar.enabled Then Return
				' not an unmodified left mouse button
				'if(e.getModifiers() != InputEvent.BUTTON1_MASK) {return; }
				If Not SwingUtilities.isLeftMouseButton(e) Then Return

				Dim direction As Integer = If(e.source Is outerInstance.incrButton, 1, -1)

				outerInstance.scrollByUnit(direction)
				outerInstance.scrollTimer.stop()
				outerInstance.scrollListener.direction = direction
				outerInstance.scrollListener.scrollByBlock = False
				outerInstance.scrollTimer.start()

				handledEvent = True
				If (Not outerInstance.scrollbar.hasFocus()) AndAlso outerInstance.scrollbar.requestFocusEnabled Then outerInstance.scrollbar.requestFocus()
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				outerInstance.scrollTimer.stop()
				handledEvent = False
				outerInstance.scrollbar.valueIsAdjusting = False
			End Sub
		End Class


		''' <summary>
		''' Listener for scrolling events initiated in the
		''' <code>ScrollPane</code>.
		''' </summary>
		Protected Friend Class ScrollListener
			Implements ActionListener

			Private ReadOnly outerInstance As BasicScrollBarUI

			Friend direction As Integer = +1
			Friend useBlockIncrement As Boolean

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
					Me.outerInstance = outerInstance
				direction = +1
				useBlockIncrement = False
			End Sub

			Public Sub New(ByVal outerInstance As BasicScrollBarUI, ByVal dir As Integer, ByVal block As Boolean)
					Me.outerInstance = outerInstance
				direction = dir
				useBlockIncrement = block
			End Sub

			Public Overridable Property direction As Integer
				Set(ByVal direction As Integer)
					Me.direction = direction
				End Set
			End Property
			Public Overridable Property scrollByBlock As Boolean
				Set(ByVal block As Boolean)
					Me.useBlockIncrement = block
				End Set
			End Property

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If useBlockIncrement Then
					outerInstance.scrollByBlock(direction)
					' Stop scrolling if the thumb catches up with the mouse
					If outerInstance.scrollbar.orientation = JScrollBar.VERTICAL Then
						If direction > 0 Then
							If outerInstance.thumbBounds.y + outerInstance.thumbBounds.height >= outerInstance.trackListener.currentMouseY Then CType(e.source, Timer).stop()
						ElseIf outerInstance.thumbBounds.y <= outerInstance.trackListener.currentMouseY Then
							CType(e.source, Timer).stop()
						End If
					Else
						If (direction > 0 AndAlso (Not outerInstance.mouseAfterThumb)) OrElse (direction < 0 AndAlso (Not outerInstance.mouseBeforeThumb)) Then CType(e.source, Timer).stop()
					End If
				Else
					outerInstance.scrollByUnit(direction)
				End If

				If direction > 0 AndAlso outerInstance.scrollbar.value+outerInstance.scrollbar.visibleAmount >= outerInstance.scrollbar.maximum Then
					CType(e.source, Timer).stop()
				ElseIf direction < 0 AndAlso outerInstance.scrollbar.value <= outerInstance.scrollbar.minimum Then
					CType(e.source, Timer).stop()
				End If
			End Sub
		End Class

		Private Property mouseLeftOfThumb As Boolean
			Get
				Return trackListener.currentMouseX < thumbBounds.x
			End Get
		End Property

		Private Property mouseRightOfThumb As Boolean
			Get
				Dim tb As Rectangle = thumbBounds
				Return trackListener.currentMouseX > tb.x + tb.width
			End Get
		End Property

		Private Property mouseBeforeThumb As Boolean
			Get
				Return If(scrollbar.componentOrientation.leftToRight, mouseLeftOfThumb, mouseRightOfThumb)
			End Get
		End Property

		Private Property mouseAfterThumb As Boolean
			Get
				Return If(scrollbar.componentOrientation.leftToRight, mouseRightOfThumb, mouseLeftOfThumb)
			End Get
		End Property

		Private Sub updateButtonDirections()
			Dim orient As Integer = scrollbar.orientation
			If scrollbar.componentOrientation.leftToRight Then
				If TypeOf incrButton Is BasicArrowButton Then CType(incrButton, BasicArrowButton).direction = If(orient = HORIZONTAL, EAST, SOUTH)
				If TypeOf decrButton Is BasicArrowButton Then CType(decrButton, BasicArrowButton).direction = If(orient = HORIZONTAL, WEST, NORTH)
			Else
				If TypeOf incrButton Is BasicArrowButton Then CType(incrButton, BasicArrowButton).direction = If(orient = HORIZONTAL, WEST, SOUTH)
				If TypeOf decrButton Is BasicArrowButton Then CType(decrButton, BasicArrowButton).direction = If(orient = HORIZONTAL, EAST, NORTH)
			End If
		End Sub

		Public Class PropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicScrollBarUI

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class


		''' <summary>
		''' Used for scrolling the scrollbar.
		''' </summary>
		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const POSITIVE_UNIT_INCREMENT As String = "positiveUnitIncrement"
			Private Const POSITIVE_BLOCK_INCREMENT As String = "positiveBlockIncrement"
			Private Const NEGATIVE_UNIT_INCREMENT As String = "negativeUnitIncrement"
			Private Const NEGATIVE_BLOCK_INCREMENT As String = "negativeBlockIncrement"
			Private Const MIN_SCROLL As String = "minScroll"
			Private Const MAX_SCROLL As String = "maxScroll"

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim scrollBar As JScrollBar = CType(e.source, JScrollBar)
				Dim key As String = name
				If key = POSITIVE_UNIT_INCREMENT Then
					scroll(scrollBar, POSITIVE_SCROLL, False)
				ElseIf key = POSITIVE_BLOCK_INCREMENT Then
					scroll(scrollBar, POSITIVE_SCROLL, True)
				ElseIf key = NEGATIVE_UNIT_INCREMENT Then
					scroll(scrollBar, NEGATIVE_SCROLL, False)
				ElseIf key = NEGATIVE_BLOCK_INCREMENT Then
					scroll(scrollBar, NEGATIVE_SCROLL, True)
				ElseIf key = MIN_SCROLL Then
					scroll(scrollBar, BasicScrollBarUI.MIN_SCROLL, True)
				ElseIf key = MAX_SCROLL Then
					scroll(scrollBar, BasicScrollBarUI.MAX_SCROLL, True)
				End If
			End Sub
			Private Sub scroll(ByVal scrollBar As JScrollBar, ByVal dir As Integer, ByVal block As Boolean)

				If dir = NEGATIVE_SCROLL OrElse dir = POSITIVE_SCROLL Then
					Dim amount As Integer
					' Don't use the BasicScrollBarUI.scrollByXXX methods as we
					' don't want to use an invokeLater to reset the trackHighlight
					' via an invokeLater
					If block Then
						If dir = NEGATIVE_SCROLL Then
							amount = -1 * scrollBar.getBlockIncrement(-1)
						Else
							amount = scrollBar.getBlockIncrement(1)
						End If
					Else
						If dir = NEGATIVE_SCROLL Then
							amount = -1 * scrollBar.getUnitIncrement(-1)
						Else
							amount = scrollBar.getUnitIncrement(1)
						End If
					End If
					scrollBar.value = scrollBar.value + amount
				ElseIf dir = BasicScrollBarUI.MIN_SCROLL Then
					scrollBar.value = scrollBar.minimum
				ElseIf dir = BasicScrollBarUI.MAX_SCROLL Then
					scrollBar.value = scrollBar.maximum
				End If
			End Sub
		End Class


		'
		' EventHandler
		'
		Private Class Handler
			Implements FocusListener, PropertyChangeListener

			Private ReadOnly outerInstance As BasicScrollBarUI

			Public Sub New(ByVal outerInstance As BasicScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' FocusListener
			'
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.scrollbar.repaint()
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.scrollbar.repaint()
			End Sub


			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim propertyName As String = e.propertyName

				If "model" = propertyName Then
					Dim oldModel As BoundedRangeModel = CType(e.oldValue, BoundedRangeModel)
					Dim newModel As BoundedRangeModel = CType(e.newValue, BoundedRangeModel)
					oldModel.removeChangeListener(outerInstance.modelListener)
					newModel.addChangeListener(outerInstance.modelListener)
					outerInstance.scrollBarValue = outerInstance.scrollbar.value
					outerInstance.scrollbar.repaint()
					outerInstance.scrollbar.revalidate()
				ElseIf "orientation" = propertyName Then
					outerInstance.updateButtonDirections()
				ElseIf "componentOrientation" = propertyName Then
					outerInstance.updateButtonDirections()
					Dim inputMap As InputMap = outerInstance.getInputMap(JComponent.WHEN_FOCUSED)
					SwingUtilities.replaceUIInputMap(outerInstance.scrollbar, JComponent.WHEN_FOCUSED, inputMap)
				End If
			End Sub
		End Class
	End Class

End Namespace