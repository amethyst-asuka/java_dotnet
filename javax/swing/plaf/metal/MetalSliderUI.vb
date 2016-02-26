Imports Microsoft.VisualBasic
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal



	''' <summary>
	''' A Java L&amp;F implementation of SliderUI.
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
	''' @author Tom Santos
	''' </summary>
	Public Class MetalSliderUI
		Inherits javax.swing.plaf.basic.BasicSliderUI

		Protected Friend ReadOnly TICK_BUFFER As Integer = 4
		Protected Friend filledSlider As Boolean = False
		' NOTE: these next five variables are currently unused.
		Protected Friend Shared thumbColor As java.awt.Color
		Protected Friend Shared highlightColor As java.awt.Color
		Protected Friend Shared darkShadowColor As java.awt.Color
		Protected Friend Shared trackWidth As Integer
		Protected Friend Shared tickLength As Integer
		Private safeLength As Integer

	   ''' <summary>
	   ''' A default horizontal thumb <code>Icon</code>. This field might not be
	   ''' used. To change the <code>Icon</code> used by this delegate directly set it
	   ''' using the <code>Slider.horizontalThumbIcon</code> UIManager property.
	   ''' </summary>
		Protected Friend Shared horizThumbIcon As Icon

	   ''' <summary>
	   ''' A default vertical thumb <code>Icon</code>. This field might not be
	   ''' used. To change the <code>Icon</code> used by this delegate directly set it
	   ''' using the <code>Slider.verticalThumbIcon</code> UIManager property.
	   ''' </summary>
		Protected Friend Shared vertThumbIcon As Icon

		Private Shared SAFE_HORIZ_THUMB_ICON As Icon
		Private Shared SAFE_VERT_THUMB_ICON As Icon


		Protected Friend ReadOnly SLIDER_FILL As String = "JSlider.isFilled"

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalSliderUI
		End Function

		Public Sub New()
			MyBase.New(Nothing)
		End Sub

		Private Property Shared horizThumbIcon As Icon
			Get
				If System.securityManager IsNot Nothing Then
					Return SAFE_HORIZ_THUMB_ICON
				Else
					Return horizThumbIcon
				End If
			End Get
		End Property

		Private Property Shared vertThumbIcon As Icon
			Get
				If System.securityManager IsNot Nothing Then
					Return SAFE_VERT_THUMB_ICON
				Else
					Return vertThumbIcon
				End If
			End Get
		End Property

		Public Overrides Sub installUI(ByVal c As JComponent)
			trackWidth = CInt(Fix(UIManager.get("Slider.trackWidth")))
				safeLength = CInt(Fix(UIManager.get("Slider.majorTickLength")))
				tickLength = safeLength
				SAFE_HORIZ_THUMB_ICON = UIManager.getIcon("Slider.horizontalThumbIcon")
				horizThumbIcon = SAFE_HORIZ_THUMB_ICON
				SAFE_VERT_THUMB_ICON = UIManager.getIcon("Slider.verticalThumbIcon")
				vertThumbIcon = SAFE_VERT_THUMB_ICON

			MyBase.installUI(c)

			thumbColor = UIManager.getColor("Slider.thumb")
			highlightColor = UIManager.getColor("Slider.highlight")
			darkShadowColor = UIManager.getColor("Slider.darkShadow")

			scrollListener.scrollByBlock = False

			prepareFilledSliderField()
		End Sub

		Protected Friend Overrides Function createPropertyChangeListener(ByVal slider As JSlider) As PropertyChangeListener
			Return New MetalPropertyListener(Me)
		End Function

		Protected Friend Class MetalPropertyListener
			Inherits javax.swing.plaf.basic.BasicSliderUI.PropertyChangeHandler

			Private ReadOnly outerInstance As MetalSliderUI

			Public Sub New(ByVal outerInstance As MetalSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent) ' listen for slider fill
				MyBase.propertyChange(e)

				If e.propertyName.Equals(outerInstance.SLIDER_FILL) Then outerInstance.prepareFilledSliderField()
			End Sub
		End Class

		Private Sub prepareFilledSliderField()
			' Use true for Ocean theme
			filledSlider = MetalLookAndFeel.usingOcean()

			Dim sliderFillProp As Object = slider.getClientProperty(SLIDER_FILL)

			If sliderFillProp IsNot Nothing Then filledSlider = CBool(sliderFillProp)
		End Sub

		Public Overridable Sub paintThumb(ByVal g As java.awt.Graphics)
			Dim knobBounds As java.awt.Rectangle = thumbRect

			g.translate(knobBounds.x, knobBounds.y)

			If slider.orientation = JSlider.HORIZONTAL Then
				horizThumbIcon.paintIcon(slider, g, 0, 0)
			Else
				vertThumbIcon.paintIcon(slider, g, 0, 0)
			End If

			g.translate(-knobBounds.x, -knobBounds.y)
		End Sub

		''' <summary>
		''' Returns a rectangle enclosing the track that will be painted.
		''' </summary>
		Private Property paintTrackRect As java.awt.Rectangle
			Get
				Dim trackLeft As Integer = 0, trackRight As Integer, trackTop As Integer = 0, trackBottom As Integer
				If slider.orientation = JSlider.HORIZONTAL Then
					trackBottom = (trackRect.height - 1) - thumbOverhang
					trackTop = trackBottom - (trackWidth - 1)
					trackRight = trackRect.width - 1
				Else
					If MetalUtils.isLeftToRight(slider) Then
						trackLeft = (trackRect.width - thumbOverhang) - trackWidth
						trackRight = (trackRect.width - thumbOverhang) - 1
					Else
						trackLeft = thumbOverhang
						trackRight = thumbOverhang + trackWidth - 1
					End If
					trackBottom = trackRect.height - 1
				End If
				Return New java.awt.Rectangle(trackRect.x + trackLeft, trackRect.y + trackTop, trackRight - trackLeft, trackBottom - trackTop)
			End Get
		End Property

		Public Overridable Sub paintTrack(ByVal g As java.awt.Graphics)
			If MetalLookAndFeel.usingOcean() Then
				oceanPaintTrack(g)
				Return
			End If
			Dim trackColor As java.awt.Color = If((Not slider.enabled), MetalLookAndFeel.controlShadow, slider.foreground)

			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(slider)

			g.translate(trackRect.x, trackRect.y)

			Dim trackLeft As Integer = 0
			Dim trackTop As Integer = 0
			Dim trackRight As Integer
			Dim trackBottom As Integer

			' Draw the track
			If slider.orientation = JSlider.HORIZONTAL Then
				trackBottom = (trackRect.height - 1) - thumbOverhang
				trackTop = trackBottom - (trackWidth - 1)
				trackRight = trackRect.width - 1
			Else
				If leftToRight Then
					trackLeft = (trackRect.width - thumbOverhang) - trackWidth
					trackRight = (trackRect.width - thumbOverhang) - 1
				Else
					trackLeft = thumbOverhang
					trackRight = thumbOverhang + trackWidth - 1
				End If
				trackBottom = trackRect.height - 1
			End If

			If slider.enabled Then
				g.color = MetalLookAndFeel.controlDarkShadow
				g.drawRect(trackLeft, trackTop, (trackRight - trackLeft) - 1, (trackBottom - trackTop) - 1)

				g.color = MetalLookAndFeel.controlHighlight
				g.drawLine(trackLeft + 1, trackBottom, trackRight, trackBottom)
				g.drawLine(trackRight, trackTop + 1, trackRight, trackBottom)

				g.color = MetalLookAndFeel.controlShadow
				g.drawLine(trackLeft + 1, trackTop + 1, trackRight - 2, trackTop + 1)
				g.drawLine(trackLeft + 1, trackTop + 1, trackLeft + 1, trackBottom - 2)
			Else
				g.color = MetalLookAndFeel.controlShadow
				g.drawRect(trackLeft, trackTop, (trackRight - trackLeft) - 1, (trackBottom - trackTop) - 1)
			End If

			' Draw the fill
			If filledSlider Then
				Dim middleOfThumb As Integer
				Dim fillTop As Integer
				Dim fillLeft As Integer
				Dim fillBottom As Integer
				Dim fillRight As Integer

				If slider.orientation = JSlider.HORIZONTAL Then
					middleOfThumb = thumbRect.x + (thumbRect.width / 2)
					middleOfThumb -= trackRect.x ' To compensate for the g.translate()
					fillTop = If((Not slider.enabled), trackTop, trackTop + 1)
					fillBottom = If((Not slider.enabled), trackBottom - 1, trackBottom - 2)

					If Not drawInverted() Then
						fillLeft = If((Not slider.enabled), trackLeft, trackLeft + 1)
						fillRight = middleOfThumb
					Else
						fillLeft = middleOfThumb
						fillRight = If((Not slider.enabled), trackRight - 1, trackRight - 2)
					End If
				Else
					middleOfThumb = thumbRect.y + (thumbRect.height / 2)
					middleOfThumb -= trackRect.y ' To compensate for the g.translate()
					fillLeft = If((Not slider.enabled), trackLeft, trackLeft + 1)
					fillRight = If((Not slider.enabled), trackRight - 1, trackRight - 2)

					If Not drawInverted() Then
						fillTop = middleOfThumb
						fillBottom = If((Not slider.enabled), trackBottom - 1, trackBottom - 2)
					Else
						fillTop = If((Not slider.enabled), trackTop, trackTop + 1)
						fillBottom = middleOfThumb
					End If
				End If

				If slider.enabled Then
					g.color = slider.background
					g.drawLine(fillLeft, fillTop, fillRight, fillTop)
					g.drawLine(fillLeft, fillTop, fillLeft, fillBottom)

					g.color = MetalLookAndFeel.controlShadow
					g.fillRect(fillLeft + 1, fillTop + 1, fillRight - fillLeft, fillBottom - fillTop)
				Else
					g.color = MetalLookAndFeel.controlShadow
					g.fillRect(fillLeft, fillTop, fillRight - fillLeft, fillBottom - fillTop)
				End If
			End If

			g.translate(-trackRect.x, -trackRect.y)
		End Sub

		Private Sub oceanPaintTrack(ByVal g As java.awt.Graphics)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(slider)
			Dim drawInverted As Boolean = drawInverted()
			Dim sliderAltTrackColor As java.awt.Color = CType(UIManager.get("Slider.altTrackColor"), java.awt.Color)

			' Translate to the origin of the painting rectangle
			Dim paintRect As java.awt.Rectangle = paintTrackRect
			g.translate(paintRect.x, paintRect.y)

			' Width and height of the painting rectangle.
			Dim w As Integer = paintRect.width
			Dim h As Integer = paintRect.height

			If slider.orientation = JSlider.HORIZONTAL Then
				Dim middleOfThumb As Integer = thumbRect.x + thumbRect.width / 2 - paintRect.x

				If slider.enabled Then
					Dim fillMinX As Integer
					Dim fillMaxX As Integer

					If middleOfThumb > 0 Then
						g.color = If(drawInverted, MetalLookAndFeel.controlDarkShadow, MetalLookAndFeel.primaryControlDarkShadow)

						g.drawRect(0, 0, middleOfThumb - 1, h - 1)
					End If

					If middleOfThumb < w Then
						g.color = If(drawInverted, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.controlDarkShadow)

						g.drawRect(middleOfThumb, 0, w - middleOfThumb - 1, h - 1)
					End If

					If filledSlider Then
						g.color = MetalLookAndFeel.primaryControlShadow
						If drawInverted Then
							fillMinX = middleOfThumb
							fillMaxX = w - 2
							g.drawLine(1, 1, middleOfThumb, 1)
						Else
							fillMinX = 1
							fillMaxX = middleOfThumb
							g.drawLine(middleOfThumb, 1, w - 1, 1)
						End If
						If h = 6 Then
							g.color = MetalLookAndFeel.white
							g.drawLine(fillMinX, 1, fillMaxX, 1)
							g.color = sliderAltTrackColor
							g.drawLine(fillMinX, 2, fillMaxX, 2)
							g.color = MetalLookAndFeel.controlShadow
							g.drawLine(fillMinX, 3, fillMaxX, 3)
							g.color = MetalLookAndFeel.primaryControlShadow
							g.drawLine(fillMinX, 4, fillMaxX, 4)
						End If
					End If
				Else
					g.color = MetalLookAndFeel.controlShadow

					If middleOfThumb > 0 Then
						If (Not drawInverted) AndAlso filledSlider Then
							g.fillRect(0, 0, middleOfThumb - 1, h - 1)
						Else
							g.drawRect(0, 0, middleOfThumb - 1, h - 1)
						End If
					End If

					If middleOfThumb < w Then
						If drawInverted AndAlso filledSlider Then
							g.fillRect(middleOfThumb, 0, w - middleOfThumb - 1, h - 1)
						Else
							g.drawRect(middleOfThumb, 0, w - middleOfThumb - 1, h - 1)
						End If
					End If
				End If
			Else
				Dim middleOfThumb As Integer = thumbRect.y + (thumbRect.height / 2) - paintRect.y

				If slider.enabled Then
					Dim fillMinY As Integer
					Dim fillMaxY As Integer

					If middleOfThumb > 0 Then
						g.color = If(drawInverted, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.controlDarkShadow)

						g.drawRect(0, 0, w - 1, middleOfThumb - 1)
					End If

					If middleOfThumb < h Then
						g.color = If(drawInverted, MetalLookAndFeel.controlDarkShadow, MetalLookAndFeel.primaryControlDarkShadow)

						g.drawRect(0, middleOfThumb, w - 1, h - middleOfThumb - 1)
					End If

					If filledSlider Then
						g.color = MetalLookAndFeel.primaryControlShadow
						If drawInverted() Then
							fillMinY = 1
							fillMaxY = middleOfThumb
							If leftToRight Then
								g.drawLine(1, middleOfThumb, 1, h - 1)
							Else
								g.drawLine(w - 2, middleOfThumb, w - 2, h - 1)
							End If
						Else
							fillMinY = middleOfThumb
							fillMaxY = h - 2
							If leftToRight Then
								g.drawLine(1, 1, 1, middleOfThumb)
							Else
								g.drawLine(w - 2, 1, w - 2, middleOfThumb)
							End If
						End If
						If w = 6 Then
							g.color = If(leftToRight, MetalLookAndFeel.white, MetalLookAndFeel.primaryControlShadow)
							g.drawLine(1, fillMinY, 1, fillMaxY)
							g.color = If(leftToRight, sliderAltTrackColor, MetalLookAndFeel.controlShadow)
							g.drawLine(2, fillMinY, 2, fillMaxY)
							g.color = If(leftToRight, MetalLookAndFeel.controlShadow, sliderAltTrackColor)
							g.drawLine(3, fillMinY, 3, fillMaxY)
							g.color = If(leftToRight, MetalLookAndFeel.primaryControlShadow, MetalLookAndFeel.white)
							g.drawLine(4, fillMinY, 4, fillMaxY)
						End If
					End If
				Else
					g.color = MetalLookAndFeel.controlShadow

					If middleOfThumb > 0 Then
						If drawInverted AndAlso filledSlider Then
							g.fillRect(0, 0, w - 1, middleOfThumb - 1)
						Else
							g.drawRect(0, 0, w - 1, middleOfThumb - 1)
						End If
					End If

					If middleOfThumb < h Then
						If (Not drawInverted) AndAlso filledSlider Then
							g.fillRect(0, middleOfThumb, w - 1, h - middleOfThumb - 1)
						Else
							g.drawRect(0, middleOfThumb, w - 1, h - middleOfThumb - 1)
						End If
					End If
				End If
			End If

			g.translate(-paintRect.x, -paintRect.y)
		End Sub

		Public Overridable Sub paintFocus(ByVal g As java.awt.Graphics)
		End Sub

		Protected Friend Property Overrides thumbSize As java.awt.Dimension
			Get
				Dim size As New java.awt.Dimension
    
				If slider.orientation = JSlider.VERTICAL Then
					size.width = vertThumbIcon.iconWidth
					size.height = vertThumbIcon.iconHeight
				Else
					size.width = horizThumbIcon.iconWidth
					size.height = horizThumbIcon.iconHeight
				End If
    
				Return size
			End Get
		End Property

		''' <summary>
		''' Gets the height of the tick area for horizontal sliders and the width of the
		''' tick area for vertical sliders.  BasicSliderUI uses the returned value to
		''' determine the tick area rectangle.
		''' </summary>
		Public Property Overrides tickLength As Integer
			Get
				Return If(slider.orientation = JSlider.HORIZONTAL, safeLength + TICK_BUFFER + 1, safeLength + TICK_BUFFER + 3)
			End Get
		End Property

		''' <summary>
		''' Returns the shorter dimension of the track.
		''' </summary>
		Protected Friend Overridable Property trackWidth As Integer
			Get
				' This strange calculation is here to keep the
				' track in proportion to the thumb.
				Const kIdealTrackWidth As Double = 7.0
				Const kIdealThumbHeight As Double = 16.0
				Dim kWidthScalar As Double = kIdealTrackWidth / kIdealThumbHeight
    
				If slider.orientation = JSlider.HORIZONTAL Then
					Return CInt(Fix(kWidthScalar * thumbRect.height))
				Else
					Return CInt(Fix(kWidthScalar * thumbRect.width))
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the longer dimension of the slide bar.  (The slide bar is only the
		''' part that runs directly under the thumb)
		''' </summary>
		Protected Friend Overridable Property trackLength As Integer
			Get
				If slider.orientation = JSlider.HORIZONTAL Then Return trackRect.width
				Return trackRect.height
			End Get
		End Property

		''' <summary>
		''' Returns the amount that the thumb goes past the slide bar.
		''' </summary>
		Protected Friend Overridable Property thumbOverhang As Integer
			Get
				Return CInt(Fix(thumbSize.height-trackWidth))\2
			End Get
		End Property

		Protected Friend Overrides Sub scrollDueToClickInTrack(ByVal dir As Integer)
			scrollByUnit(dir)
		End Sub

		Protected Friend Overridable Sub paintMinorTickForHorizSlider(ByVal g As java.awt.Graphics, ByVal tickBounds As java.awt.Rectangle, ByVal x As Integer)
			g.color = If(slider.enabled, slider.foreground, MetalLookAndFeel.controlShadow)
			g.drawLine(x, TICK_BUFFER, x, TICK_BUFFER + (safeLength \ 2))
		End Sub

		Protected Friend Overridable Sub paintMajorTickForHorizSlider(ByVal g As java.awt.Graphics, ByVal tickBounds As java.awt.Rectangle, ByVal x As Integer)
			g.color = If(slider.enabled, slider.foreground, MetalLookAndFeel.controlShadow)
			g.drawLine(x, TICK_BUFFER, x, TICK_BUFFER + (safeLength - 1))
		End Sub

		Protected Friend Overridable Sub paintMinorTickForVertSlider(ByVal g As java.awt.Graphics, ByVal tickBounds As java.awt.Rectangle, ByVal y As Integer)
			g.color = If(slider.enabled, slider.foreground, MetalLookAndFeel.controlShadow)

			If MetalUtils.isLeftToRight(slider) Then
				g.drawLine(TICK_BUFFER, y, TICK_BUFFER + (safeLength \ 2), y)
			Else
				g.drawLine(0, y, safeLength\2, y)
			End If
		End Sub

		Protected Friend Overridable Sub paintMajorTickForVertSlider(ByVal g As java.awt.Graphics, ByVal tickBounds As java.awt.Rectangle, ByVal y As Integer)
			g.color = If(slider.enabled, slider.foreground, MetalLookAndFeel.controlShadow)

			If MetalUtils.isLeftToRight(slider) Then
				g.drawLine(TICK_BUFFER, y, TICK_BUFFER + safeLength, y)
			Else
				g.drawLine(0, y, safeLength, y)
			End If
		End Sub
	End Class

End Namespace