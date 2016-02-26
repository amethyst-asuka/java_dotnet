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
	''' <seealso cref="JSlider"/>.
	''' 
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthSliderUI
		Inherits javax.swing.plaf.basic.BasicSliderUI
		Implements PropertyChangeListener, SynthUI

		Private valueRect As New java.awt.Rectangle
		Private paintValue As Boolean

		''' <summary>
		''' When a JSlider is used as a renderer in a JTable, its layout is not
		''' being recomputed even though the size is changing. Even though there
		''' is a ComponentListener installed, it is not being notified. As such,
		''' at times when being asked to paint the layout should first be redone.
		''' At the end of the layout method we set this lastSize variable, which
		''' represents the size of the slider the last time it was layed out.
		''' 
		''' In the paint method we then check to see that this is accurate, that
		''' the slider has not changed sizes since being last layed out. If necessary
		''' we recompute the layout.
		''' </summary>
		Private lastSize As java.awt.Dimension

		Private trackHeight As Integer
		Private trackBorder As Integer
		Private thumbWidth As Integer
		Private thumbHeight As Integer

		Private style As SynthStyle
		Private sliderTrackStyle As SynthStyle
		Private sliderThumbStyle As SynthStyle

		''' <summary>
		''' Used to determine the color to paint the thumb. </summary>
		<NonSerialized> _
		Private thumbActive As Boolean 'happens on rollover, and when pressed
		<NonSerialized> _
		Private thumbPressed As Boolean 'happens when mouse was depressed while over thumb

		'/////////////////////////////////////////////////
		' ComponentUI Interface Implementation methods
		'/////////////////////////////////////////////////
		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthSliderUI(CType(c, JSlider))
		End Function

		Protected Friend Sub New(ByVal c As JSlider)
			MyBase.New(c)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults(ByVal slider As JSlider)
			updateStyle(slider)
		End Sub

		''' <summary>
		''' Uninstalls default setting. This method is called when a
		''' {@code LookAndFeel} is uninstalled.
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults(ByVal slider As JSlider)
			Dim ___context As SynthContext = getContext(slider, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			___context = getContext(slider, Region.SLIDER_TRACK, ENABLED)
			sliderTrackStyle.uninstallDefaults(___context)
			___context.Dispose()
			sliderTrackStyle = Nothing

			___context = getContext(slider, Region.SLIDER_THUMB, ENABLED)
			sliderThumbStyle.uninstallDefaults(___context)
			___context.Dispose()
			sliderThumbStyle = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners(ByVal slider As JSlider)
			MyBase.installListeners(slider)
			slider.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners(ByVal slider As JSlider)
			slider.removePropertyChangeListener(Me)
			MyBase.uninstallListeners(slider)
		End Sub

		Private Sub updateStyle(ByVal c As JSlider)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)

			If style IsNot oldStyle Then
				thumbWidth = style.getInt(___context, "Slider.thumbWidth", 30)

				thumbHeight = style.getInt(___context, "Slider.thumbHeight", 14)

				' handle scaling for sizeVarients for special case components. The
				' key "JComponent.sizeVariant" scales for large/small/mini
				' components are based on Apples LAF
				Dim scaleKey As String = CStr(slider.getClientProperty("JComponent.sizeVariant"))
				If scaleKey IsNot Nothing Then
					If "large".Equals(scaleKey) Then
						thumbWidth *= 1.15
						thumbHeight *= 1.15
					ElseIf "small".Equals(scaleKey) Then
						thumbWidth *= 0.857
						thumbHeight *= 0.857
					ElseIf "mini".Equals(scaleKey) Then
						thumbWidth *= 0.784
						thumbHeight *= 0.784
					End If
				End If

				trackBorder = style.getInt(___context, "Slider.trackBorder", 1)

				trackHeight = thumbHeight + trackBorder * 2

				paintValue = style.getBoolean(___context, "Slider.paintValue", True)
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions(c)
					installKeyboardActions(c)
				End If
			End If
			___context.Dispose()

			___context = getContext(c, Region.SLIDER_TRACK, ENABLED)
			sliderTrackStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()

			___context = getContext(c, Region.SLIDER_THUMB, ENABLED)
			sliderThumbStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createTrackListener(ByVal s As JSlider) As TrackListener
			Return New SynthTrackListener(Me)
		End Function

		Private Sub updateThumbState(ByVal x As Integer, ByVal y As Integer)
			thumbActive = thumbRect.contains(x, y)
		End Sub

		Private Sub updateThumbState(ByVal x As Integer, ByVal y As Integer, ByVal pressed As Boolean)
			updateThumbState(x, y)
			thumbPressed = pressed
		End Sub

		Private Property thumbActive As Boolean
			Set(ByVal active As Boolean)
				If thumbActive <> active Then
					thumbActive = active
					slider.repaint(thumbRect)
				End If
			End Set
		End Property

		Private Property thumbPressed As Boolean
			Set(ByVal pressed As Boolean)
				If thumbPressed <> pressed Then
					thumbPressed = pressed
					slider.repaint(thumbRect)
				End If
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			If c Is Nothing Then Throw New NullPointerException("Component must be non-null")
			If width < 0 OrElse height < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")
			If slider.paintLabels AndAlso labelsHaveSameBaselines() Then
				' Get the insets for the track.
				Dim trackInsets As New java.awt.Insets(0, 0, 0, 0)
				Dim trackContext As SynthContext = getContext(slider, Region.SLIDER_TRACK)
				style.getInsets(trackContext, trackInsets)
				trackContext.Dispose()
				If slider.orientation = JSlider.HORIZONTAL Then
					Dim valueHeight As Integer = 0
					If paintValue Then
						Dim ___context As SynthContext = getContext(slider)
						valueHeight = ___context.style.getGraphicsUtils(___context).getMaximumCharHeight(___context)
						___context.Dispose()
					End If
					Dim tickHeight As Integer = 0
					If slider.paintTicks Then tickHeight = tickLength
					Dim labelHeight As Integer = heightOfTallestLabel
					Dim contentHeight As Integer = valueHeight + trackHeight + trackInsets.top + trackInsets.bottom + tickHeight + labelHeight + 4
					Dim centerY As Integer = height \ 2 - contentHeight \ 2
					centerY += valueHeight + 2
					centerY += trackHeight + trackInsets.top + trackInsets.bottom
					centerY += tickHeight + 2
					Dim label As JComponent = CType(slider.labelTable.elements().nextElement(), JComponent)
					Dim pref As java.awt.Dimension = label.preferredSize
					Return centerY + label.getBaseline(pref.width, pref.height)
				Else ' VERTICAL
					Dim value As Integer? = If(slider.inverted, lowestValue, highestValue)
					If value IsNot Nothing Then
						Dim valueY As Integer = insetCache.top
						Dim valueHeight As Integer = 0
						If paintValue Then
							Dim ___context As SynthContext = getContext(slider)
							valueHeight = ___context.style.getGraphicsUtils(___context).getMaximumCharHeight(___context)
							___context.Dispose()
						End If
						Dim contentHeight As Integer = height - insetCache.top - insetCache.bottom
						Dim trackY As Integer = valueY + valueHeight
						Dim trackHeight As Integer = contentHeight - valueHeight
						Dim yPosition As Integer = yPositionForValue(value, trackY, trackHeight)
						Dim label As JComponent = CType(slider.labelTable.get(value), JComponent)
						Dim pref As java.awt.Dimension = label.preferredSize
						Return yPosition - pref.height / 2 + label.getBaseline(pref.width, pref.height)
					End If
				End If
			End If
			Return -1
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			recalculateIfInsetsChanged()
			Dim d As New java.awt.Dimension(contentRect.width, contentRect.height)
			If slider.orientation = JSlider.VERTICAL Then
				d.height = 200
			Else
				d.width = 200
			End If
			Dim i As java.awt.Insets = slider.insets
			d.width += i.left + i.right
			d.height += i.top + i.bottom
			Return d
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			recalculateIfInsetsChanged()
			Dim d As New java.awt.Dimension(contentRect.width, contentRect.height)
			If slider.orientation = JSlider.VERTICAL Then
				d.height = thumbRect.height + insetCache.top + insetCache.bottom
			Else
				d.width = thumbRect.width + insetCache.left + insetCache.right
			End If
			Return d
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub calculateGeometry()
			calculateThumbSize()
			layout()
			calculateThumbLocation()
		End Sub

		''' <summary>
		''' Lays out the slider.
		''' </summary>
		Protected Friend Overridable Sub layout()
			Dim ___context As SynthContext = getContext(slider)
			Dim synthGraphics As SynthGraphicsUtils = style.getGraphicsUtils(___context)

			' Get the insets for the track.
			Dim trackInsets As New java.awt.Insets(0, 0, 0, 0)
			Dim trackContext As SynthContext = getContext(slider, Region.SLIDER_TRACK)
			style.getInsets(trackContext, trackInsets)
			trackContext.Dispose()

			If slider.orientation = JSlider.HORIZONTAL Then
				' Calculate the height of all the subcomponents so we can center
				' them.
				valueRect.height = 0
				If paintValue Then valueRect.height = synthGraphics.getMaximumCharHeight(___context)

				trackRect.height = trackHeight

				tickRect.height = 0
				If slider.paintTicks Then tickRect.height = tickLength

				labelRect.height = 0
				If slider.paintLabels Then labelRect.height = heightOfTallestLabel

				contentRect.height = valueRect.height + trackRect.height + trackInsets.top + trackInsets.bottom + tickRect.height + labelRect.height + 4
				contentRect.width = slider.width - insetCache.left - insetCache.right

				' Check if any of the labels will paint out of bounds.
				Dim pad As Integer = 0
				If slider.paintLabels Then
					' Calculate the track rectangle.  It is necessary for
					' xPositionForValue to return correct values.
					trackRect.x = insetCache.left
					trackRect.width = contentRect.width

					Dim dictionary As java.util.Dictionary = slider.labelTable
					If dictionary IsNot Nothing Then
						Dim minValue As Integer = slider.minimum
						Dim maxValue As Integer = slider.maximum

						' Iterate through the keys in the dictionary and find the
						' first and last labels indices that fall within the
						' slider range.
						Dim firstLblIdx As Integer = Integer.MaxValue
						Dim lastLblIdx As Integer = Integer.MinValue
						Dim keys As System.Collections.IEnumerator = dictionary.keys()
						Do While keys.hasMoreElements()
							Dim keyInt As Integer = CInt(Fix(keys.nextElement()))
							If keyInt >= minValue AndAlso keyInt < firstLblIdx Then firstLblIdx = keyInt
							If keyInt <= maxValue AndAlso keyInt > lastLblIdx Then lastLblIdx = keyInt
						Loop
						' Calculate the pad necessary for the labels at the first
						' and last visible indices.
						pad = getPadForLabel(firstLblIdx)
						pad = Math.Max(pad, getPadForLabel(lastLblIdx))
					End If
				End If
				' Calculate the painting rectangles for each of the different
				' slider areas.
					labelRect.x = (insetCache.left + pad)
						tickRect.x = labelRect.x
							trackRect.x = tickRect.x
							valueRect.x = trackRect.x
					labelRect.width = (contentRect.width - (pad * 2))
						tickRect.width = labelRect.width
							trackRect.width = tickRect.width
							valueRect.width = trackRect.width

				Dim centerY As Integer = slider.height \ 2 - contentRect.height / 2

				valueRect.y = centerY
				centerY += valueRect.height + 2

				trackRect.y = centerY + trackInsets.top
				centerY += trackRect.height + trackInsets.top + trackInsets.bottom

				tickRect.y = centerY
				centerY += tickRect.height + 2

				labelRect.y = centerY
				centerY += labelRect.height
			Else
				' Calculate the width of all the subcomponents so we can center
				' them.
				trackRect.width = trackHeight

				tickRect.width = 0
				If slider.paintTicks Then tickRect.width = tickLength

				labelRect.width = 0
				If slider.paintLabels Then labelRect.width = widthOfWidestLabel

				valueRect.y = insetCache.top
				valueRect.height = 0
				If paintValue Then valueRect.height = synthGraphics.getMaximumCharHeight(___context)

				' Get the max width of the min or max value of the slider.
				Dim fm As java.awt.FontMetrics = slider.getFontMetrics(slider.font)
				valueRect.width = Math.Max(synthGraphics.computeStringWidth(___context, slider.font, fm, "" & slider.maximum), synthGraphics.computeStringWidth(___context, slider.font, fm, "" & slider.minimum))

				Dim l As Integer = valueRect.width / 2
				Dim w1 As Integer = trackInsets.left + trackRect.width / 2
				Dim w2 As Integer = trackRect.width / 2 + trackInsets.right + tickRect.width + labelRect.width
				contentRect.width = Math.Max(w1, l) + Math.Max(w2, l) + 2 + insetCache.left + insetCache.right
				contentRect.height = slider.height - insetCache.top - insetCache.bottom

				' Layout the components.
					labelRect.y = valueRect.y + valueRect.height
						tickRect.y = labelRect.y
						trackRect.y = tickRect.y
					labelRect.height = contentRect.height - valueRect.height
						tickRect.height = labelRect.height
						trackRect.height = tickRect.height

				Dim startX As Integer = slider.width \ 2 - contentRect.width / 2
				If SynthLookAndFeel.isLeftToRight(slider) Then
					If l > w1 Then startX += (l - w1)
					trackRect.x = startX + trackInsets.left

					startX += trackInsets.left + trackRect.width + trackInsets.right
					tickRect.x = startX
					labelRect.x = startX + tickRect.width + 2
				Else
					If l > w2 Then startX += (l - w2)
					labelRect.x = startX

					startX += labelRect.width + 2
					tickRect.x = startX
					trackRect.x = startX + tickRect.width + trackInsets.left
				End If
			End If
			___context.Dispose()
			lastSize = slider.size
		End Sub

		''' <summary>
		''' Calculates the pad for the label at the specified index.
		''' </summary>
		''' <param name="i"> index of the label to calculate pad for. </param>
		''' <returns> padding required to keep label visible. </returns>
		Private Function getPadForLabel(ByVal i As Integer) As Integer
			Dim pad As Integer = 0

			Dim c As JComponent = CType(slider.labelTable.get(i), JComponent)
			If c IsNot Nothing Then
				Dim centerX As Integer = xPositionForValue(i)
				Dim cHalfWidth As Integer = c.preferredSize.width / 2
				If centerX - cHalfWidth < insetCache.left Then pad = Math.Max(pad, insetCache.left - (centerX - cHalfWidth))

				If centerX + cHalfWidth > slider.width - insetCache.right Then pad = Math.Max(pad, (centerX + cHalfWidth) - (slider.width - insetCache.right))
			End If
			Return pad
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub calculateThumbLocation()
			MyBase.calculateThumbLocation()
			If slider.orientation = JSlider.HORIZONTAL Then
				thumbRect.y += trackBorder
			Else
				thumbRect.x += trackBorder
			End If
			Dim mousePosition As java.awt.Point = slider.mousePosition
			If mousePosition IsNot Nothing Then updateThumbState(mousePosition.x, mousePosition.y)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub setThumbLocation(ByVal x As Integer, ByVal y As Integer)
			MyBase.thumbLocationion(x, y)
			' Value rect is tied to the thumb location.  We need to repaint when
			' the thumb repaints.
			slider.repaint(valueRect.x, valueRect.y, valueRect.width, valueRect.height)
			thumbActive = False
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function xPositionForValue(ByVal value As Integer) As Integer
			Dim min As Integer = slider.minimum
			Dim max As Integer = slider.maximum
			Dim trackLeft As Integer = trackRect.x + thumbRect.width / 2 + trackBorder
			Dim trackRight As Integer = trackRect.x + trackRect.width - thumbRect.width / 2 - trackBorder
			Dim trackLength As Integer = trackRight - trackLeft
			Dim valueRange As Double = CDbl(max) - CDbl(min)
			Dim pixelsPerValue As Double = CDbl(trackLength) / valueRange
			Dim xPosition As Integer

			If Not drawInverted() Then
				xPosition = trackLeft
				xPosition += Math.Round(pixelsPerValue * (CDbl(value) - min))
			Else
				xPosition = trackRight
				xPosition -= Math.Round(pixelsPerValue * (CDbl(value) - min))
			End If

			xPosition = Math.Max(trackLeft, xPosition)
			xPosition = Math.Min(trackRight, xPosition)

			Return xPosition
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function yPositionForValue(ByVal value As Integer, ByVal trackY As Integer, ByVal trackHeight As Integer) As Integer
			Dim min As Integer = slider.minimum
			Dim max As Integer = slider.maximum
			Dim trackTop As Integer = trackY + thumbRect.height / 2 + trackBorder
			Dim trackBottom As Integer = trackY + trackHeight - thumbRect.height / 2 - trackBorder
			Dim trackLength As Integer = trackBottom - trackTop
			Dim valueRange As Double = CDbl(max) - CDbl(min)
			Dim pixelsPerValue As Double = CDbl(trackLength) / valueRange
			Dim yPosition As Integer

			If Not drawInverted() Then
				yPosition = trackTop
				yPosition += Math.Round(pixelsPerValue * (CDbl(max) - value))
			Else
				yPosition = trackTop
				yPosition += Math.Round(pixelsPerValue * (CDbl(value) - min))
			End If

			yPosition = Math.Max(trackTop, yPosition)
			yPosition = Math.Min(trackBottom, yPosition)

			Return yPosition
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function valueForYPosition(ByVal yPos As Integer) As Integer
			Dim value As Integer
			Dim minValue As Integer = slider.minimum
			Dim maxValue As Integer = slider.maximum
			Dim trackTop As Integer = trackRect.y + thumbRect.height / 2 + trackBorder
			Dim trackBottom As Integer = trackRect.y + trackRect.height - thumbRect.height / 2 - trackBorder
			Dim trackLength As Integer = trackBottom - trackTop

			If yPos <= trackTop Then
				value = If(drawInverted(), minValue, maxValue)
			ElseIf yPos >= trackBottom Then
				value = If(drawInverted(), maxValue, minValue)
			Else
				Dim distanceFromTrackTop As Integer = yPos - trackTop
				Dim valueRange As Double = CDbl(maxValue) - CDbl(minValue)
				Dim valuePerPixel As Double = valueRange / CDbl(trackLength)
				Dim valueFromTrackTop As Integer = CInt(Fix(Math.Round(distanceFromTrackTop * valuePerPixel)))
				value = If(drawInverted(), minValue + valueFromTrackTop, maxValue - valueFromTrackTop)
			End If
			Return value
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function valueForXPosition(ByVal xPos As Integer) As Integer
			Dim value As Integer
			Dim minValue As Integer = slider.minimum
			Dim maxValue As Integer = slider.maximum
			Dim trackLeft As Integer = trackRect.x + thumbRect.width / 2 + trackBorder
			Dim trackRight As Integer = trackRect.x + trackRect.width - thumbRect.width / 2 - trackBorder
			Dim trackLength As Integer = trackRight - trackLeft

			If xPos <= trackLeft Then
				value = If(drawInverted(), maxValue, minValue)
			ElseIf xPos >= trackRight Then
				value = If(drawInverted(), minValue, maxValue)
			Else
				Dim distanceFromTrackLeft As Integer = xPos - trackLeft
				Dim valueRange As Double = CDbl(maxValue) - CDbl(minValue)
				Dim valuePerPixel As Double = valueRange / CDbl(trackLength)
				Dim valueFromTrackLeft As Integer = CInt(Fix(Math.Round(distanceFromTrackLeft * valuePerPixel)))
				value = If(drawInverted(), maxValue - valueFromTrackLeft, minValue + valueFromTrackLeft)
			End If
			Return value
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides thumbSize As java.awt.Dimension
			Get
				Dim size As New java.awt.Dimension
    
				If slider.orientation = JSlider.VERTICAL Then
					size.width = thumbHeight
					size.height = thumbWidth
				Else
					size.width = thumbWidth
					size.height = thumbHeight
				End If
				Return size
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub recalculateIfInsetsChanged()
			Dim ___context As SynthContext = getContext(slider)
			Dim newInsets As java.awt.Insets = style.getInsets(___context, Nothing)
			Dim compInsets As java.awt.Insets = slider.insets
			newInsets.left += compInsets.left
			newInsets.right += compInsets.right
			newInsets.top += compInsets.top
			newInsets.bottom += compInsets.bottom
			If Not newInsets.Equals(insetCache) Then
				insetCache = newInsets
				calculateGeometry()
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

		Private Function getContext(ByVal c As JComponent, ByVal subregion As Region) As SynthContext
			Return getContext(c, subregion, getComponentState(c, subregion))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal subregion As Region, ByVal state As Integer) As SynthContext
			Dim style As SynthStyle = Nothing

			If subregion Is Region.SLIDER_TRACK Then
				style = sliderTrackStyle
			ElseIf subregion Is Region.SLIDER_THUMB Then
				style = sliderThumbStyle
			End If
			Return SynthContext.getContext(c, subregion, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent, ByVal ___region As Region) As Integer
			If ___region Is Region.SLIDER_THUMB AndAlso thumbActive AndAlso c.enabled Then
				Dim state As Integer = If(thumbPressed, PRESSED, MOUSE_OVER)
				If c.focusOwner Then state = state Or FOCUSED
				Return state
			End If
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
		Public Overrides Sub update(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)
			SynthLookAndFeel.update(___context, g)
			___context.painter.paintSliderBackground(___context, g, 0, 0, c.width, c.height, slider.orientation)
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
			recalculateIfInsetsChanged()
			recalculateIfOrientationChanged()
			Dim clip As java.awt.Rectangle = g.clipBounds

			If lastSize Is Nothing OrElse (Not lastSize.Equals(slider.size)) Then calculateGeometry()

			If paintValue Then
				Dim fm As java.awt.FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(slider, g)
				Dim labelWidth As Integer = context.style.getGraphicsUtils(context).computeStringWidth(context, g.font, fm, "" & slider.value)
				valueRect.x = thumbRect.x + (thumbRect.width - labelWidth) / 2

				' For horizontal sliders, make sure value is not painted
				' outside slider bounds.
				If slider.orientation = JSlider.HORIZONTAL Then
					If valueRect.x + labelWidth > insetCache.left + contentRect.width Then valueRect.x = (insetCache.left + contentRect.width) - labelWidth
					valueRect.x = Math.Max(valueRect.x, 0)
				End If

				g.color = context.style.getColor(context, ColorType.TEXT_FOREGROUND)
				context.style.getGraphicsUtils(context).paintText(context, g, "" & slider.value, valueRect.x, valueRect.y, -1)
			End If

			If slider.paintTrack AndAlso clip.intersects(trackRect) Then
				Dim subcontext As SynthContext = getContext(slider, Region.SLIDER_TRACK)
				paintTrack(subcontext, g, trackRect)
				subcontext.Dispose()
			End If

			If clip.intersects(thumbRect) Then
				Dim subcontext As SynthContext = getContext(slider, Region.SLIDER_THUMB)
				paintThumb(subcontext, g, thumbRect)
				subcontext.Dispose()
			End If

			If slider.paintTicks AndAlso clip.intersects(tickRect) Then paintTicks(g)

			If slider.paintLabels AndAlso clip.intersects(labelRect) Then paintLabels(g)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			context.painter.paintSliderBorder(context, g, x, y, w, h, slider.orientation)
		End Sub

		''' <summary>
		''' Paints the slider thumb.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="thumbBounds"> bounding box for the thumb </param>
		Protected Friend Overridable Sub paintThumb(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal thumbBounds As java.awt.Rectangle)
			Dim orientation As Integer = slider.orientation
			SynthLookAndFeel.updateSubregion(context, g, thumbBounds)
			context.painter.paintSliderThumbBackground(context, g, thumbBounds.x, thumbBounds.y, thumbBounds.width, thumbBounds.height, orientation)
			context.painter.paintSliderThumbBorder(context, g, thumbBounds.x, thumbBounds.y, thumbBounds.width, thumbBounds.height, orientation)
		End Sub

		''' <summary>
		''' Paints the slider track.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="trackBounds"> bounding box for the track </param>
		Protected Friend Overridable Sub paintTrack(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal trackBounds As java.awt.Rectangle)
			Dim orientation As Integer = slider.orientation
			SynthLookAndFeel.updateSubregion(context, g, trackBounds)
			context.painter.paintSliderTrackBackground(context, g, trackBounds.x, trackBounds.y, trackBounds.width, trackBounds.height, orientation)
			context.painter.paintSliderTrackBorder(context, g, trackBounds.x, trackBounds.y, trackBounds.width, trackBounds.height, orientation)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JSlider))
		End Sub

		'////////////////////////////////////////////////
		'/ Track Listener Class
		'////////////////////////////////////////////////
		''' <summary>
		''' Track mouse movements.
		''' </summary>
		Private Class SynthTrackListener
			Inherits TrackListener

			Private ReadOnly outerInstance As SynthSliderUI

			Public Sub New(ByVal outerInstance As SynthSliderUI)
				Me.outerInstance = outerInstance
			End Sub


			Public Overrides Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.thumbActive = False
			End Sub

			Public Overrides Sub mousePressed(ByVal e As MouseEvent)
				MyBase.mousePressed(e)
				outerInstance.thumbPressed = outerInstance.thumbRect.contains(e.x, e.y)
			End Sub

			Public Overrides Sub mouseReleased(ByVal e As MouseEvent)
				MyBase.mouseReleased(e)
				outerInstance.updateThumbState(e.x, e.y, False)
			End Sub

			Public Overrides Sub mouseDragged(ByVal e As MouseEvent)
				Dim thumbMiddle As Integer

				If Not outerInstance.slider.enabled Then Return

				currentMouseX = e.x
				currentMouseY = e.y

				If Not outerInstance.dragging Then Return

				outerInstance.slider.valueIsAdjusting = True

				Select Case outerInstance.slider.orientation
				Case JSlider.VERTICAL
					Dim halfThumbHeight As Integer = outerInstance.thumbRect.height / 2
					Dim thumbTop As Integer = e.y - offset
					Dim trackTop As Integer = outerInstance.trackRect.y
					Dim trackBottom As Integer = outerInstance.trackRect.y + outerInstance.trackRect.height - halfThumbHeight - outerInstance.trackBorder
					Dim vMax As Integer = outerInstance.yPositionForValue(outerInstance.slider.maximum - outerInstance.slider.extent)

					If outerInstance.drawInverted() Then
						trackBottom = vMax
						trackTop = trackTop + halfThumbHeight
					Else
						trackTop = vMax
					End If
					thumbTop = Math.Max(thumbTop, trackTop - halfThumbHeight)
					thumbTop = Math.Min(thumbTop, trackBottom - halfThumbHeight)

					outerInstance.thumbLocationion(outerInstance.thumbRect.x, thumbTop)

					thumbMiddle = thumbTop + halfThumbHeight
					outerInstance.slider.value = outerInstance.valueForYPosition(thumbMiddle)
				Case JSlider.HORIZONTAL
					Dim halfThumbWidth As Integer = outerInstance.thumbRect.width / 2
					Dim thumbLeft As Integer = e.x - offset
					Dim trackLeft As Integer = outerInstance.trackRect.x + halfThumbWidth + outerInstance.trackBorder
					Dim trackRight As Integer = outerInstance.trackRect.x + outerInstance.trackRect.width - halfThumbWidth - outerInstance.trackBorder
					Dim hMax As Integer = outerInstance.xPositionForValue(outerInstance.slider.maximum - outerInstance.slider.extent)

					If outerInstance.drawInverted() Then
						trackLeft = hMax
					Else
						trackRight = hMax
					End If
					thumbLeft = Math.Max(thumbLeft, trackLeft - halfThumbWidth)
					thumbLeft = Math.Min(thumbLeft, trackRight - halfThumbWidth)

					outerInstance.thumbLocationion(thumbLeft, outerInstance.thumbRect.y)

					thumbMiddle = thumbLeft + halfThumbWidth
					outerInstance.slider.value = outerInstance.valueForXPosition(thumbMiddle)
				Case Else
					Return
				End Select

				If outerInstance.slider.valueIsAdjusting Then outerInstance.thumbActive = True
			End Sub

			Public Overrides Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.updateThumbState(e.x, e.y)
			End Sub
		End Class
	End Class

End Namespace