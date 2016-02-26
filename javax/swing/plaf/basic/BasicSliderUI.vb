Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A Basic L&amp;F implementation of SliderUI.
	''' 
	''' @author Tom Santos
	''' </summary>
	Public Class BasicSliderUI
		Inherits SliderUI

		' Old actions forward to an instance of this.
		Private Shared ReadOnly SHARED_ACTION As New Actions

		Public Const POSITIVE_SCROLL As Integer = +1
		Public Const NEGATIVE_SCROLL As Integer = -1
		Public Const MIN_SCROLL As Integer = -2
		Public Const MAX_SCROLL As Integer = +2

		Protected Friend scrollTimer As Timer
		Protected Friend slider As JSlider

		Protected Friend focusInsets As Insets = Nothing
		Protected Friend insetCache As Insets = Nothing
		Protected Friend leftToRightCache As Boolean = True
		Protected Friend focusRect As Rectangle = Nothing
		Protected Friend contentRect As Rectangle = Nothing
		Protected Friend labelRect As Rectangle = Nothing
		Protected Friend tickRect As Rectangle = Nothing
		Protected Friend trackRect As Rectangle = Nothing
		Protected Friend thumbRect As Rectangle = Nothing

		Protected Friend trackBuffer As Integer = 0 ' The distance that the track is from the side of the control

		<NonSerialized> _
		Private ___isDragging As Boolean

		Protected Friend trackListener As TrackListener
		Protected Friend changeListener As ChangeListener
		Protected Friend componentListener As ComponentListener
		Protected Friend focusListener As FocusListener
		Protected Friend scrollListener As ScrollListener
		Protected Friend propertyChangeListener As PropertyChangeListener
		Private handler As Handler
		Private lastValue As Integer

		' Colors
		Private shadowColor As Color
		Private highlightColor As Color
		Private focusColor As Color

		''' <summary>
		''' Whther or not sameLabelBaselines is up to date.
		''' </summary>
		Private checkedLabelBaselines As Boolean
		''' <summary>
		''' Whether or not all the entries in the labeltable have the same
		''' baseline.
		''' </summary>
		Private sameLabelBaselines As Boolean


		Protected Friend Overridable Property shadowColor As Color
			Get
				Return shadowColor
			End Get
		End Property

		Protected Friend Overridable Property highlightColor As Color
			Get
				Return highlightColor
			End Get
		End Property

		Protected Friend Overridable Property focusColor As Color
			Get
				Return focusColor
			End Get
		End Property

		''' <summary>
		''' Returns true if the user is dragging the slider.
		''' </summary>
		''' <returns> true if the user is dragging the slider
		''' @since 1.5 </returns>
		Protected Friend Overridable Property dragging As Boolean
			Get
				Return ___isDragging
			End Get
		End Property

		'///////////////////////////////////////////////////////////////////////////
		' ComponentUI Interface Implementation methods
		'///////////////////////////////////////////////////////////////////////////
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Return New BasicSliderUI(CType(b, JSlider))
		End Function

		Public Sub New(ByVal b As JSlider)
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			slider = CType(c, JSlider)

			checkedLabelBaselines = False

			slider.enabled = slider.enabled
			LookAndFeel.installProperty(slider, "opaque", Boolean.TRUE)

			___isDragging = False
			trackListener = createTrackListener(slider)
			changeListener = createChangeListener(slider)
			componentListener = createComponentListener(slider)
			focusListener = createFocusListener(slider)
			scrollListener = createScrollListener(slider)
			propertyChangeListener = createPropertyChangeListener(slider)

			installDefaults(slider)
			installListeners(slider)
			installKeyboardActions(slider)

			scrollTimer = New Timer(100, scrollListener)
			scrollTimer.initialDelay = 300

			insetCache = slider.insets
			leftToRightCache = BasicGraphicsUtils.isLeftToRight(slider)
			focusRect = New Rectangle
			contentRect = New Rectangle
			labelRect = New Rectangle
			tickRect = New Rectangle
			trackRect = New Rectangle
			thumbRect = New Rectangle
			lastValue = slider.value

			calculateGeometry() ' This figures out where the labels, ticks, track, and thumb are.
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			If c IsNot slider Then Throw New IllegalComponentStateException(Me & " was asked to deinstall() " & c & " when it only knows about " & slider & ".")

			scrollTimer.stop()
			scrollTimer = Nothing

			uninstallDefaults(slider)
			uninstallListeners(slider)
			uninstallKeyboardActions(slider)

			insetCache = Nothing
			leftToRightCache = True
			focusRect = Nothing
			contentRect = Nothing
			labelRect = Nothing
			tickRect = Nothing
			trackRect = Nothing
			thumbRect = Nothing
			trackListener = Nothing
			changeListener = Nothing
			componentListener = Nothing
			focusListener = Nothing
			scrollListener = Nothing
			propertyChangeListener = Nothing
			slider = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal slider As JSlider)
			LookAndFeel.installBorder(slider, "Slider.border")
			LookAndFeel.installColorsAndFont(slider, "Slider.background", "Slider.foreground", "Slider.font")
			highlightColor = UIManager.getColor("Slider.highlight")

			shadowColor = UIManager.getColor("Slider.shadow")
			focusColor = UIManager.getColor("Slider.focus")

			focusInsets = CType(UIManager.get("Slider.focusInsets"), Insets)
			' use default if missing so that BasicSliderUI can be used in other
			' LAFs like Nimbus
			If focusInsets Is Nothing Then focusInsets = New InsetsUIResource(2,2,2,2)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal slider As JSlider)
			LookAndFeel.uninstallBorder(slider)

			focusInsets = Nothing
		End Sub

		Protected Friend Overridable Function createTrackListener(ByVal slider As JSlider) As TrackListener
			Return New TrackListener(Me)
		End Function

		Protected Friend Overridable Function createChangeListener(ByVal slider As JSlider) As ChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createComponentListener(ByVal slider As JSlider) As ComponentListener
			Return handler
		End Function

		Protected Friend Overridable Function createFocusListener(ByVal slider As JSlider) As FocusListener
			Return handler
		End Function

		Protected Friend Overridable Function createScrollListener(ByVal slider As JSlider) As ScrollListener
			Return New ScrollListener(Me)
		End Function

		Protected Friend Overridable Function createPropertyChangeListener(ByVal slider As JSlider) As PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Sub installListeners(ByVal slider As JSlider)
			slider.addMouseListener(trackListener)
			slider.addMouseMotionListener(trackListener)
			slider.addFocusListener(focusListener)
			slider.addComponentListener(componentListener)
			slider.addPropertyChangeListener(propertyChangeListener)
			slider.model.addChangeListener(changeListener)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal slider As JSlider)
			slider.removeMouseListener(trackListener)
			slider.removeMouseMotionListener(trackListener)
			slider.removeFocusListener(focusListener)
			slider.removeComponentListener(componentListener)
			slider.removePropertyChangeListener(propertyChangeListener)
			slider.model.removeChangeListener(changeListener)
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub installKeyboardActions(ByVal slider As JSlider)
			Dim km As InputMap = getInputMap(JComponent.WHEN_FOCUSED, slider)
			SwingUtilities.replaceUIInputMap(slider, JComponent.WHEN_FOCUSED, km)
			LazyActionMap.installLazyActionMap(slider, GetType(BasicSliderUI), "Slider.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer, ByVal slider As JSlider) As InputMap
			If condition = JComponent.WHEN_FOCUSED Then
				Dim keyMap As InputMap = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.focusInputMap"), InputMap)
				Dim rtlKeyMap As InputMap

				rtlKeyMap = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.focusInputMap.RightToLeft"), InputMap)
				If slider.componentOrientation.leftToRight OrElse (rtlKeyMap Is Nothing) Then
					Return keyMap
				Else
					rtlKeyMap.parent = keyMap
					Return rtlKeyMap
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Populates ComboBox's actions.
		''' </summary>
		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.POSITIVE_UNIT_INCREMENT))
			map.put(New Actions(Actions.POSITIVE_BLOCK_INCREMENT))
			map.put(New Actions(Actions.NEGATIVE_UNIT_INCREMENT))
			map.put(New Actions(Actions.NEGATIVE_BLOCK_INCREMENT))
			map.put(New Actions(Actions.MIN_SCROLL_INCREMENT))
			map.put(New Actions(Actions.MAX_SCROLL_INCREMENT))
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions(ByVal slider As JSlider)
			SwingUtilities.replaceUIActionMap(slider, Nothing)
			SwingUtilities.replaceUIInputMap(slider, JComponent.WHEN_FOCUSED, Nothing)
		End Sub


		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			If slider.paintLabels AndAlso labelsHaveSameBaselines() Then
				Dim metrics As FontMetrics = slider.getFontMetrics(slider.font)
				Dim insets As Insets = slider.insets
				Dim ___thumbSize As Dimension = thumbSize
				If slider.orientation = JSlider.HORIZONTAL Then
					Dim ___tickLength As Integer = tickLength
					Dim contentHeight As Integer = height - insets.top - insets.bottom - focusInsets.top - focusInsets.bottom
					Dim thumbHeight As Integer = ___thumbSize.height
					Dim centerSpacing As Integer = thumbHeight
					If slider.paintTicks Then centerSpacing += ___tickLength
					' Assume uniform labels.
					centerSpacing += heightOfTallestLabel
					Dim trackY As Integer = insets.top + focusInsets.top + (contentHeight - centerSpacing - 1) \ 2
					Dim trackHeight As Integer = thumbHeight
					Dim tickY As Integer = trackY + trackHeight
					Dim tickHeight As Integer = ___tickLength
					If Not slider.paintTicks Then tickHeight = 0
					Dim labelY As Integer = tickY + tickHeight
					Return labelY + metrics.ascent
				Else ' vertical
					Dim inverted As Boolean = slider.inverted
					Dim value As Integer? = If(inverted, lowestValue, highestValue)
					If value IsNot Nothing Then
						Dim thumbHeight As Integer = ___thumbSize.height
						Dim trackBuffer As Integer = Math.Max(metrics.height / 2, thumbHeight \ 2)
						Dim contentY As Integer = focusInsets.top + insets.top
						Dim trackY As Integer = contentY + trackBuffer
						Dim trackHeight As Integer = height - focusInsets.top - focusInsets.bottom - insets.top - insets.bottom - trackBuffer - trackBuffer
						Dim yPosition As Integer = yPositionForValue(value, trackY, trackHeight)
						Return yPosition - metrics.height / 2 + metrics.ascent
					End If
				End If
			End If
			Return 0
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			' NOTE: BasicSpinner really provides for CENTER_OFFSET, but
			' the default min/pref size is smaller than it should be
			' so that getBaseline() doesn't implement the contract
			' for CENTER_OFFSET as defined in Component.
			Return Component.BaselineResizeBehavior.OTHER
		End Function

		''' <summary>
		''' Returns true if all the labels from the label table have the same
		''' baseline.
		''' </summary>
		''' <returns> true if all the labels from the label table have the
		'''         same baseline
		''' @since 1.6 </returns>
		Protected Friend Overridable Function labelsHaveSameBaselines() As Boolean
			If Not checkedLabelBaselines Then
				checkedLabelBaselines = True
				Dim dictionary As java.util.Dictionary = slider.labelTable
				If dictionary IsNot Nothing Then
					sameLabelBaselines = True
					Dim elements As System.Collections.IEnumerator = dictionary.elements()
					Dim ___baseline As Integer = -1
					Do While elements.hasMoreElements()
						Dim label As JComponent = CType(elements.nextElement(), JComponent)
						Dim pref As Dimension = label.preferredSize
						Dim labelBaseline As Integer = label.getBaseline(pref.width, pref.height)
						If labelBaseline >= 0 Then
							If ___baseline = -1 Then
								___baseline = labelBaseline
							ElseIf ___baseline <> labelBaseline Then
								sameLabelBaselines = False
								Exit Do
							End If
						Else
							sameLabelBaselines = False
							Exit Do
						End If
					Loop
				Else
					sameLabelBaselines = False
				End If
			End If
			Return sameLabelBaselines
		End Function

		Public Overridable Property preferredHorizontalSize As Dimension
			Get
				Dim horizDim As Dimension = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.horizontalSize"), Dimension)
				If horizDim Is Nothing Then horizDim = New Dimension(200, 21)
				Return horizDim
			End Get
		End Property

		Public Overridable Property preferredVerticalSize As Dimension
			Get
				Dim vertDim As Dimension = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.verticalSize"), Dimension)
				If vertDim Is Nothing Then vertDim = New Dimension(21, 200)
				Return vertDim
			End Get
		End Property

		Public Overridable Property minimumHorizontalSize As Dimension
			Get
				Dim minHorizDim As Dimension = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.minimumHorizontalSize"), Dimension)
				If minHorizDim Is Nothing Then minHorizDim = New Dimension(36, 21)
				Return minHorizDim
			End Get
		End Property

		Public Overridable Property minimumVerticalSize As Dimension
			Get
				Dim minVertDim As Dimension = CType(sun.swing.DefaultLookup.get(slider, Me, "Slider.minimumVerticalSize"), Dimension)
				If minVertDim Is Nothing Then minVertDim = New Dimension(21, 36)
				Return minVertDim
			End Get
		End Property

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			recalculateIfInsetsChanged()
			Dim d As Dimension
			If slider.orientation = JSlider.VERTICAL Then
				d = New Dimension(preferredVerticalSize)
				d.width = insetCache.left + insetCache.right
				d.width += focusInsets.left + focusInsets.right
				d.width += trackRect.width + tickRect.width + labelRect.width
			Else
				d = New Dimension(preferredHorizontalSize)
				d.height = insetCache.top + insetCache.bottom
				d.height += focusInsets.top + focusInsets.bottom
				d.height += trackRect.height + tickRect.height + labelRect.height
			End If

			Return d
		End Function

		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			recalculateIfInsetsChanged()
			Dim d As Dimension

			If slider.orientation = JSlider.VERTICAL Then
				d = New Dimension(minimumVerticalSize)
				d.width = insetCache.left + insetCache.right
				d.width += focusInsets.left + focusInsets.right
				d.width += trackRect.width + tickRect.width + labelRect.width
			Else
				d = New Dimension(minimumHorizontalSize)
				d.height = insetCache.top + insetCache.bottom
				d.height += focusInsets.top + focusInsets.bottom
				d.height += trackRect.height + tickRect.height + labelRect.height
			End If

			Return d
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = getPreferredSize(c)
			If slider.orientation = JSlider.VERTICAL Then
				d.height = Short.MaxValue
			Else
				d.width = Short.MaxValue
			End If

			Return d
		End Function

		Protected Friend Overridable Sub calculateGeometry()
			calculateFocusRect()
			calculateContentRect()
			calculateThumbSize()
			calculateTrackBuffer()
			calculateTrackRect()
			calculateTickRect()
			calculateLabelRect()
			calculateThumbLocation()
		End Sub

		Protected Friend Overridable Sub calculateFocusRect()
			focusRect.x = insetCache.left
			focusRect.y = insetCache.top
			focusRect.width = slider.width - (insetCache.left + insetCache.right)
			focusRect.height = slider.height - (insetCache.top + insetCache.bottom)
		End Sub

		Protected Friend Overridable Sub calculateThumbSize()
			Dim size As Dimension = thumbSize
			thumbRect.sizeize(size.width, size.height)
		End Sub

		Protected Friend Overridable Sub calculateContentRect()
			contentRect.x = focusRect.x + focusInsets.left
			contentRect.y = focusRect.y + focusInsets.top
			contentRect.width = focusRect.width - (focusInsets.left + focusInsets.right)
			contentRect.height = focusRect.height - (focusInsets.top + focusInsets.bottom)
		End Sub

		Private Property tickSpacing As Integer
			Get
				Dim majorTickSpacing As Integer = slider.majorTickSpacing
				Dim minorTickSpacing As Integer = slider.minorTickSpacing
    
				Dim result As Integer
    
				If minorTickSpacing > 0 Then
					result = minorTickSpacing
				ElseIf majorTickSpacing > 0 Then
					result = majorTickSpacing
				Else
					result = 0
				End If
    
				Return result
			End Get
		End Property

		Protected Friend Overridable Sub calculateThumbLocation()
			If slider.snapToTicks Then
				Dim sliderValue As Integer = slider.value
				Dim snappedValue As Integer = sliderValue
				Dim ___tickSpacing As Integer = tickSpacing

				If ___tickSpacing <> 0 Then
					' If it's not on a tick, change the value
					If (sliderValue - slider.minimum) Mod ___tickSpacing <> 0 Then
						Dim temp As Single = CSng(sliderValue - slider.minimum) / CSng(___tickSpacing)
						Dim whichTick As Integer = Math.Round(temp)

						' This is the fix for the bug #6401380
						If temp - CInt(Fix(temp)) =.5 AndAlso sliderValue < lastValue Then whichTick -= 1
						snappedValue = slider.minimum + (whichTick * ___tickSpacing)
					End If

					If snappedValue <> sliderValue Then slider.value = snappedValue
				End If
			End If

			If slider.orientation = JSlider.HORIZONTAL Then
				Dim valuePosition As Integer = xPositionForValue(slider.value)

				thumbRect.x = valuePosition - (thumbRect.width / 2)
				thumbRect.y = trackRect.y
			Else
				Dim valuePosition As Integer = yPositionForValue(slider.value)

				thumbRect.x = trackRect.x
				thumbRect.y = valuePosition - (thumbRect.height / 2)
			End If
		End Sub

		Protected Friend Overridable Sub calculateTrackBuffer()
			If slider.paintLabels AndAlso slider.labelTable IsNot Nothing Then
				Dim highLabel As Component = highestValueLabel
				Dim lowLabel As Component = lowestValueLabel

				If slider.orientation = JSlider.HORIZONTAL Then
					trackBuffer = Math.Max(highLabel.bounds.width, lowLabel.bounds.width) / 2
					trackBuffer = Math.Max(trackBuffer, thumbRect.width / 2)
				Else
					trackBuffer = Math.Max(highLabel.bounds.height, lowLabel.bounds.height) / 2
					trackBuffer = Math.Max(trackBuffer, thumbRect.height / 2)
				End If
			Else
				If slider.orientation = JSlider.HORIZONTAL Then
					trackBuffer = thumbRect.width / 2
				Else
					trackBuffer = thumbRect.height / 2
				End If
			End If
		End Sub


		Protected Friend Overridable Sub calculateTrackRect()
			Dim centerSpacing As Integer ' used to center sliders added using BorderLayout.CENTER (bug 4275631)
			If slider.orientation = JSlider.HORIZONTAL Then
				centerSpacing = thumbRect.height
				If slider.paintTicks Then centerSpacing += tickLength
				If slider.paintLabels Then centerSpacing += heightOfTallestLabel
				trackRect.x = contentRect.x + trackBuffer
				trackRect.y = contentRect.y + (contentRect.height - centerSpacing - 1)/2
				trackRect.width = contentRect.width - (trackBuffer * 2)
				trackRect.height = thumbRect.height
			Else
				centerSpacing = thumbRect.width
				If BasicGraphicsUtils.isLeftToRight(slider) Then
					If slider.paintTicks Then centerSpacing += tickLength
					If slider.paintLabels Then centerSpacing += widthOfWidestLabel
				Else
					If slider.paintTicks Then centerSpacing -= tickLength
					If slider.paintLabels Then centerSpacing -= widthOfWidestLabel
				End If
				trackRect.x = contentRect.x + (contentRect.width - centerSpacing - 1)/2
				trackRect.y = contentRect.y + trackBuffer
				trackRect.width = thumbRect.width
				trackRect.height = contentRect.height - (trackBuffer * 2)
			End If

		End Sub

		''' <summary>
		''' Gets the height of the tick area for horizontal sliders and the width of the
		''' tick area for vertical sliders.  BasicSliderUI uses the returned value to
		''' determine the tick area rectangle.  If you want to give your ticks some room,
		''' make this larger than you need and paint your ticks away from the sides in paintTicks().
		''' </summary>
		Protected Friend Overridable Property tickLength As Integer
			Get
				Return 8
			End Get
		End Property

		Protected Friend Overridable Sub calculateTickRect()
			If slider.orientation = JSlider.HORIZONTAL Then
				tickRect.x = trackRect.x
				tickRect.y = trackRect.y + trackRect.height
				tickRect.width = trackRect.width
				tickRect.height = If(slider.paintTicks, tickLength, 0)
			Else
				tickRect.width = If(slider.paintTicks, tickLength, 0)
				If BasicGraphicsUtils.isLeftToRight(slider) Then
					tickRect.x = trackRect.x + trackRect.width
				Else
					tickRect.x = trackRect.x - tickRect.width
				End If
				tickRect.y = trackRect.y
				tickRect.height = trackRect.height
			End If
		End Sub

		Protected Friend Overridable Sub calculateLabelRect()
			If slider.paintLabels Then
				If slider.orientation = JSlider.HORIZONTAL Then
					labelRect.x = tickRect.x - trackBuffer
					labelRect.y = tickRect.y + tickRect.height
					labelRect.width = tickRect.width + (trackBuffer * 2)
					labelRect.height = heightOfTallestLabel
				Else
					If BasicGraphicsUtils.isLeftToRight(slider) Then
						labelRect.x = tickRect.x + tickRect.width
						labelRect.width = widthOfWidestLabel
					Else
						labelRect.width = widthOfWidestLabel
						labelRect.x = tickRect.x - labelRect.width
					End If
					labelRect.y = tickRect.y - trackBuffer
					labelRect.height = tickRect.height + (trackBuffer * 2)
				End If
			Else
				If slider.orientation = JSlider.HORIZONTAL Then
					labelRect.x = tickRect.x
					labelRect.y = tickRect.y + tickRect.height
					labelRect.width = tickRect.width
					labelRect.height = 0
				Else
					If BasicGraphicsUtils.isLeftToRight(slider) Then
						labelRect.x = tickRect.x + tickRect.width
					Else
						labelRect.x = tickRect.x
					End If
					labelRect.y = tickRect.y
					labelRect.width = 0
					labelRect.height = tickRect.height
				End If
			End If
		End Sub

		Protected Friend Overridable Property thumbSize As Dimension
			Get
				Dim size As New Dimension
    
				If slider.orientation = JSlider.VERTICAL Then
					size.width = 20
					size.height = 11
				Else
					size.width = 11
					size.height = 20
				End If
    
				Return size
			End Get
		End Property

		Public Class PropertyChangeHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
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

		Protected Friend Overridable Property widthOfWidestLabel As Integer
			Get
				Dim dictionary As java.util.Dictionary = slider.labelTable
				Dim widest As Integer = 0
				If dictionary IsNot Nothing Then
					Dim keys As System.Collections.IEnumerator = dictionary.keys()
					Do While keys.hasMoreElements()
						Dim label As JComponent = CType(dictionary.get(keys.nextElement()), JComponent)
						widest = Math.Max(label.preferredSize.width, widest)
					Loop
				End If
				Return widest
			End Get
		End Property

		Protected Friend Overridable Property heightOfTallestLabel As Integer
			Get
				Dim dictionary As java.util.Dictionary = slider.labelTable
				Dim tallest As Integer = 0
				If dictionary IsNot Nothing Then
					Dim keys As System.Collections.IEnumerator = dictionary.keys()
					Do While keys.hasMoreElements()
						Dim label As JComponent = CType(dictionary.get(keys.nextElement()), JComponent)
						tallest = Math.Max(label.preferredSize.height, tallest)
					Loop
				End If
				Return tallest
			End Get
		End Property

		Protected Friend Overridable Property widthOfHighValueLabel As Integer
			Get
				Dim label As Component = highestValueLabel
				Dim width As Integer = 0
    
				If label IsNot Nothing Then width = label.preferredSize.width
    
				Return width
			End Get
		End Property

		Protected Friend Overridable Property widthOfLowValueLabel As Integer
			Get
				Dim label As Component = lowestValueLabel
				Dim width As Integer = 0
    
				If label IsNot Nothing Then width = label.preferredSize.width
    
				Return width
			End Get
		End Property

		Protected Friend Overridable Property heightOfHighValueLabel As Integer
			Get
				Dim label As Component = highestValueLabel
				Dim height As Integer = 0
    
				If label IsNot Nothing Then height = label.preferredSize.height
    
				Return height
			End Get
		End Property

		Protected Friend Overridable Property heightOfLowValueLabel As Integer
			Get
				Dim label As Component = lowestValueLabel
				Dim height As Integer = 0
    
				If label IsNot Nothing Then height = label.preferredSize.height
    
				Return height
			End Get
		End Property

		Protected Friend Overridable Function drawInverted() As Boolean
			If slider.orientation=JSlider.HORIZONTAL Then
				If BasicGraphicsUtils.isLeftToRight(slider) Then
					Return slider.inverted
				Else
					Return Not slider.inverted
				End If
			Else
				Return slider.inverted
			End If
		End Function

		''' <summary>
		''' Returns the biggest value that has an entry in the label table.
		''' </summary>
		''' <returns> biggest value that has an entry in the label table, or
		'''         null.
		''' @since 1.6 </returns>
		Protected Friend Overridable Property highestValue As Integer?
			Get
				Dim dictionary As java.util.Dictionary = slider.labelTable
    
				If dictionary Is Nothing Then Return Nothing
    
				Dim keys As System.Collections.IEnumerator = dictionary.keys()
    
				Dim max As Integer? = Nothing
    
				Do While keys.hasMoreElements()
					Dim i As Integer? = CInt(Fix(keys.nextElement()))
    
					If max Is Nothing OrElse i > max Then max = i
				Loop
    
				Return max
			End Get
		End Property

		''' <summary>
		''' Returns the smallest value that has an entry in the label table.
		''' </summary>
		''' <returns> smallest value that has an entry in the label table, or
		'''         null.
		''' @since 1.6 </returns>
		Protected Friend Overridable Property lowestValue As Integer?
			Get
				Dim dictionary As java.util.Dictionary = slider.labelTable
    
				If dictionary Is Nothing Then Return Nothing
    
				Dim keys As System.Collections.IEnumerator = dictionary.keys()
    
				Dim min As Integer? = Nothing
    
				Do While keys.hasMoreElements()
					Dim i As Integer? = CInt(Fix(keys.nextElement()))
    
					If min Is Nothing OrElse i < min Then min = i
				Loop
    
				Return min
			End Get
		End Property


		''' <summary>
		''' Returns the label that corresponds to the highest slider value in the label table. </summary>
		''' <seealso cref= JSlider#setLabelTable </seealso>
		Protected Friend Overridable Property lowestValueLabel As Component
			Get
				Dim min As Integer? = lowestValue
				If min IsNot Nothing Then Return CType(slider.labelTable.get(min), Component)
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the label that corresponds to the lowest slider value in the label table. </summary>
		''' <seealso cref= JSlider#setLabelTable </seealso>
		Protected Friend Overridable Property highestValueLabel As Component
			Get
				Dim max As Integer? = highestValue
				If max IsNot Nothing Then Return CType(slider.labelTable.get(max), Component)
				Return Nothing
			End Get
		End Property

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			recalculateIfInsetsChanged()
			recalculateIfOrientationChanged()
			Dim clip As Rectangle = g.clipBounds

			If (Not clip.intersects(trackRect)) AndAlso slider.paintTrack Then calculateGeometry()

			If slider.paintTrack AndAlso clip.intersects(trackRect) Then paintTrack(g)
			If slider.paintTicks AndAlso clip.intersects(tickRect) Then paintTicks(g)
			If slider.paintLabels AndAlso clip.intersects(labelRect) Then paintLabels(g)
			If slider.hasFocus() AndAlso clip.intersects(focusRect) Then paintFocus(g)
			If clip.intersects(thumbRect) Then paintThumb(g)
		End Sub

		Protected Friend Overridable Sub recalculateIfInsetsChanged()
			Dim newInsets As Insets = slider.insets
			If Not newInsets.Equals(insetCache) Then
				insetCache = newInsets
				calculateGeometry()
			End If
		End Sub

		Protected Friend Overridable Sub recalculateIfOrientationChanged()
			Dim ltr As Boolean = BasicGraphicsUtils.isLeftToRight(slider)
			If ltr<>leftToRightCache Then
				leftToRightCache = ltr
				calculateGeometry()
			End If
		End Sub

		Public Overridable Sub paintFocus(ByVal g As Graphics)
			g.color = focusColor

			BasicGraphicsUtils.drawDashedRect(g, focusRect.x, focusRect.y, focusRect.width, focusRect.height)
		End Sub

		Public Overridable Sub paintTrack(ByVal g As Graphics)

			Dim trackBounds As Rectangle = trackRect

			If slider.orientation = JSlider.HORIZONTAL Then
				Dim cy As Integer = (trackBounds.height / 2) - 2
				Dim cw As Integer = trackBounds.width

				g.translate(trackBounds.x, trackBounds.y + cy)

				g.color = shadowColor
				g.drawLine(0, 0, cw - 1, 0)
				g.drawLine(0, 1, 0, 2)
				g.color = highlightColor
				g.drawLine(0, 3, cw, 3)
				g.drawLine(cw, 0, cw, 3)
				g.color = Color.black
				g.drawLine(1, 1, cw-2, 1)

				g.translate(-trackBounds.x, -(trackBounds.y + cy))
			Else
				Dim cx As Integer = (trackBounds.width / 2) - 2
				Dim ch As Integer = trackBounds.height

				g.translate(trackBounds.x + cx, trackBounds.y)

				g.color = shadowColor
				g.drawLine(0, 0, 0, ch - 1)
				g.drawLine(1, 0, 2, 0)
				g.color = highlightColor
				g.drawLine(3, 0, 3, ch)
				g.drawLine(0, ch, 3, ch)
				g.color = Color.black
				g.drawLine(1, 1, 1, ch-2)

				g.translate(-(trackBounds.x + cx), -trackBounds.y)
			End If
		End Sub

		Public Overridable Sub paintTicks(ByVal g As Graphics)
			Dim tickBounds As Rectangle = tickRect

			g.color = sun.swing.DefaultLookup.getColor(slider, Me, "Slider.tickColor", Color.black)

			If slider.orientation = JSlider.HORIZONTAL Then
				g.translate(0, tickBounds.y)

				If slider.minorTickSpacing > 0 Then
					Dim value As Integer = slider.minimum

					Do While value <= slider.maximum
						Dim xPos As Integer = xPositionForValue(value)
						paintMinorTickForHorizSlider(g, tickBounds, xPos)

						' Overflow checking
						If Integer.MaxValue - slider.minorTickSpacing < value Then Exit Do

						value += slider.minorTickSpacing
					Loop
				End If

				If slider.majorTickSpacing > 0 Then
					Dim value As Integer = slider.minimum

					Do While value <= slider.maximum
						Dim xPos As Integer = xPositionForValue(value)
						paintMajorTickForHorizSlider(g, tickBounds, xPos)

						' Overflow checking
						If Integer.MaxValue - slider.majorTickSpacing < value Then Exit Do

						value += slider.majorTickSpacing
					Loop
				End If

				g.translate(0, -tickBounds.y)
			Else
				g.translate(tickBounds.x, 0)

				If slider.minorTickSpacing > 0 Then
					Dim offset As Integer = 0
					If Not BasicGraphicsUtils.isLeftToRight(slider) Then
						offset = tickBounds.width - tickBounds.width / 2
						g.translate(offset, 0)
					End If

					Dim value As Integer = slider.minimum

					Do While value <= slider.maximum
						Dim yPos As Integer = yPositionForValue(value)
						paintMinorTickForVertSlider(g, tickBounds, yPos)

						' Overflow checking
						If Integer.MaxValue - slider.minorTickSpacing < value Then Exit Do

						value += slider.minorTickSpacing
					Loop

					If Not BasicGraphicsUtils.isLeftToRight(slider) Then g.translate(-offset, 0)
				End If

				If slider.majorTickSpacing > 0 Then
					If Not BasicGraphicsUtils.isLeftToRight(slider) Then g.translate(2, 0)

					Dim value As Integer = slider.minimum

					Do While value <= slider.maximum
						Dim yPos As Integer = yPositionForValue(value)
						paintMajorTickForVertSlider(g, tickBounds, yPos)

						' Overflow checking
						If Integer.MaxValue - slider.majorTickSpacing < value Then Exit Do

						value += slider.majorTickSpacing
					Loop

					If Not BasicGraphicsUtils.isLeftToRight(slider) Then g.translate(-2, 0)
				End If
				g.translate(-tickBounds.x, 0)
			End If
		End Sub

		Protected Friend Overridable Sub paintMinorTickForHorizSlider(ByVal g As Graphics, ByVal tickBounds As Rectangle, ByVal x As Integer)
			g.drawLine(x, 0, x, tickBounds.height / 2 - 1)
		End Sub

		Protected Friend Overridable Sub paintMajorTickForHorizSlider(ByVal g As Graphics, ByVal tickBounds As Rectangle, ByVal x As Integer)
			g.drawLine(x, 0, x, tickBounds.height - 2)
		End Sub

		Protected Friend Overridable Sub paintMinorTickForVertSlider(ByVal g As Graphics, ByVal tickBounds As Rectangle, ByVal y As Integer)
			g.drawLine(0, y, tickBounds.width / 2 - 1, y)
		End Sub

		Protected Friend Overridable Sub paintMajorTickForVertSlider(ByVal g As Graphics, ByVal tickBounds As Rectangle, ByVal y As Integer)
			g.drawLine(0, y, tickBounds.width - 2, y)
		End Sub

		Public Overridable Sub paintLabels(ByVal g As Graphics)
			Dim labelBounds As Rectangle = labelRect

			Dim dictionary As java.util.Dictionary = slider.labelTable
			If dictionary IsNot Nothing Then
				Dim keys As System.Collections.IEnumerator = dictionary.keys()
				Dim minValue As Integer = slider.minimum
				Dim maxValue As Integer = slider.maximum
				Dim enabled As Boolean = slider.enabled
				Do While keys.hasMoreElements()
					Dim key As Integer? = CInt(Fix(keys.nextElement()))
					Dim value As Integer = key
					If value >= minValue AndAlso value <= maxValue Then
						Dim label As JComponent = CType(dictionary.get(key), JComponent)
						label.enabled = enabled

						If TypeOf label Is JLabel Then
							Dim icon As Icon = If(label.enabled, CType(label, JLabel).icon, CType(label, JLabel).disabledIcon)

							If TypeOf icon Is ImageIcon Then Toolkit.defaultToolkit.checkImage(CType(icon, ImageIcon).image, -1, -1, slider)
						End If

						If slider.orientation = JSlider.HORIZONTAL Then
							g.translate(0, labelBounds.y)
							paintHorizontalLabel(g, value, label)
							g.translate(0, -labelBounds.y)
						Else
							Dim offset As Integer = 0
							If Not BasicGraphicsUtils.isLeftToRight(slider) Then offset = labelBounds.width - label.preferredSize.width
							g.translate(labelBounds.x + offset, 0)
							paintVerticalLabel(g, value, label)
							g.translate(-labelBounds.x - offset, 0)
						End If
					End If
				Loop
			End If

		End Sub

		''' <summary>
		''' Called for every label in the label table.  Used to draw the labels for horizontal sliders.
		''' The graphics have been translated to labelRect.y already. </summary>
		''' <seealso cref= JSlider#setLabelTable </seealso>
		Protected Friend Overridable Sub paintHorizontalLabel(ByVal g As Graphics, ByVal value As Integer, ByVal label As Component)
			Dim labelCenter As Integer = xPositionForValue(value)
			Dim labelLeft As Integer = labelCenter - (label.preferredSize.width / 2)
			g.translate(labelLeft, 0)
			label.paint(g)
			g.translate(-labelLeft, 0)
		End Sub

		''' <summary>
		''' Called for every label in the label table.  Used to draw the labels for vertical sliders.
		''' The graphics have been translated to labelRect.x already. </summary>
		''' <seealso cref= JSlider#setLabelTable </seealso>
		Protected Friend Overridable Sub paintVerticalLabel(ByVal g As Graphics, ByVal value As Integer, ByVal label As Component)
			Dim labelCenter As Integer = yPositionForValue(value)
			Dim labelTop As Integer = labelCenter - (label.preferredSize.height / 2)
			g.translate(0, labelTop)
			label.paint(g)
			g.translate(0, -labelTop)
		End Sub

		Public Overridable Sub paintThumb(ByVal g As Graphics)
			Dim knobBounds As Rectangle = thumbRect
			Dim w As Integer = knobBounds.width
			Dim h As Integer = knobBounds.height

			g.translate(knobBounds.x, knobBounds.y)

			If slider.enabled Then
				g.color = slider.background
			Else
				g.color = slider.background.darker()
			End If

			Dim paintThumbArrowShape As Boolean? = CBool(slider.getClientProperty("Slider.paintThumbArrowShape"))

			If ((Not slider.paintTicks) AndAlso paintThumbArrowShape Is Nothing) OrElse paintThumbArrowShape Is Boolean.FALSE Then

				' "plain" version
				g.fillRect(0, 0, w, h)

				g.color = Color.black
				g.drawLine(0, h-1, w-1, h-1)
				g.drawLine(w-1, 0, w-1, h-1)

				g.color = highlightColor
				g.drawLine(0, 0, 0, h-2)
				g.drawLine(1, 0, w-2, 0)

				g.color = shadowColor
				g.drawLine(1, h-2, w-2, h-2)
				g.drawLine(w-2, 1, w-2, h-3)
			ElseIf slider.orientation = JSlider.HORIZONTAL Then
				Dim cw As Integer = w \ 2
				g.fillRect(1, 1, w-3, h-1-cw)
				Dim p As New Polygon
				p.addPoint(1, h-cw)
				p.addPoint(cw-1, h-1)
				p.addPoint(w-2, h-1-cw)
				g.fillPolygon(p)

				g.color = highlightColor
				g.drawLine(0, 0, w-2, 0)
				g.drawLine(0, 1, 0, h-1-cw)
				g.drawLine(0, h-cw, cw-1, h-1)

				g.color = Color.black
				g.drawLine(w-1, 0, w-1, h-2-cw)
				g.drawLine(w-1, h-1-cw, w-1-cw, h-1)

				g.color = shadowColor
				g.drawLine(w-2, 1, w-2, h-2-cw)
				g.drawLine(w-2, h-1-cw, w-1-cw, h-2)
			Else ' vertical
				Dim cw As Integer = h \ 2
				If BasicGraphicsUtils.isLeftToRight(slider) Then
					  g.fillRect(1, 1, w-1-cw, h-3)
					  Dim p As New Polygon
					  p.addPoint(w-cw-1, 0)
					  p.addPoint(w-1, cw)
					  p.addPoint(w-1-cw, h-2)
					  g.fillPolygon(p)

					  g.color = highlightColor
					  g.drawLine(0, 0, 0, h - 2) ' left
					  g.drawLine(1, 0, w-1-cw, 0) ' top
					  g.drawLine(w-cw-1, 0, w-1, cw) ' top slant

					  g.color = Color.black
					  g.drawLine(0, h-1, w-2-cw, h-1) ' bottom
					  g.drawLine(w-1-cw, h-1, w-1, h-1-cw) ' bottom slant

					  g.color = shadowColor
					  g.drawLine(1, h-2, w-2-cw, h-2) ' bottom
					  g.drawLine(w-1-cw, h-2, w-2, h-cw-1) ' bottom slant
				Else
					  g.fillRect(5, 1, w-1-cw, h-3)
					  Dim p As New Polygon
					  p.addPoint(cw, 0)
					  p.addPoint(0, cw)
					  p.addPoint(cw, h-2)
					  g.fillPolygon(p)

					  g.color = highlightColor
					  g.drawLine(cw-1, 0, w-2, 0) ' top
					  g.drawLine(0, cw, cw, 0) ' top slant

					  g.color = Color.black
					  g.drawLine(0, h-1-cw, cw, h-1) ' bottom slant
					  g.drawLine(cw, h-1, w-1, h-1) ' bottom

					  g.color = shadowColor
					  g.drawLine(cw, h-2, w-2, h-2) ' bottom
					  g.drawLine(w-1, 1, w-1, h-2) ' right
				End If
			End If

			g.translate(-knobBounds.x, -knobBounds.y)
		End Sub

		' Used exclusively by setThumbLocation()
		Private Shared unionRect As New Rectangle

		Public Overridable Sub setThumbLocation(ByVal x As Integer, ByVal y As Integer)
			unionRect.bounds = thumbRect

			thumbRect.locationion(x, y)

			SwingUtilities.computeUnion(thumbRect.x, thumbRect.y, thumbRect.width, thumbRect.height, unionRect)
			slider.repaint(unionRect.x, unionRect.y, unionRect.width, unionRect.height)
		End Sub

		Public Overridable Sub scrollByBlock(ByVal direction As Integer)
			SyncLock slider
				Dim blockIncrement As Integer = (slider.maximum - slider.minimum) \ 10
				If blockIncrement = 0 Then blockIncrement = 1

				If slider.snapToTicks Then
					Dim ___tickSpacing As Integer = tickSpacing

					If blockIncrement < ___tickSpacing Then blockIncrement = ___tickSpacing
				End If

				Dim delta As Integer = blockIncrement * (If(direction > 0, POSITIVE_SCROLL, NEGATIVE_SCROLL))
				slider.value = slider.value + delta
			End SyncLock
		End Sub

		Public Overridable Sub scrollByUnit(ByVal direction As Integer)
			SyncLock slider
				Dim delta As Integer = (If(direction > 0, POSITIVE_SCROLL, NEGATIVE_SCROLL))

				If slider.snapToTicks Then delta *= tickSpacing

				slider.value = slider.value + delta
			End SyncLock
		End Sub

		''' <summary>
		''' This function is called when a mousePressed was detected in the track, not
		''' in the thumb.  The default behavior is to scroll by block.  You can
		'''  override this method to stop it from scrolling or to add additional behavior.
		''' </summary>
		Protected Friend Overridable Sub scrollDueToClickInTrack(ByVal dir As Integer)
			scrollByBlock(dir)
		End Sub

		Protected Friend Overridable Function xPositionForValue(ByVal value As Integer) As Integer
			Dim min As Integer = slider.minimum
			Dim max As Integer = slider.maximum
			Dim trackLength As Integer = trackRect.width
			Dim valueRange As Double = CDbl(max) - CDbl(min)
			Dim pixelsPerValue As Double = CDbl(trackLength) / valueRange
			Dim trackLeft As Integer = trackRect.x
			Dim trackRight As Integer = trackRect.x + (trackRect.width - 1)
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

		Protected Friend Overridable Function yPositionForValue(ByVal value As Integer) As Integer
			Return yPositionForValue(value, trackRect.y, trackRect.height)
		End Function

		''' <summary>
		''' Returns the y location for the specified value.  No checking is
		''' done on the arguments.  In particular if <code>trackHeight</code> is
		''' negative undefined results may occur.
		''' </summary>
		''' <param name="value"> the slider value to get the location for </param>
		''' <param name="trackY"> y-origin of the track </param>
		''' <param name="trackHeight"> the height of the track
		''' @since 1.6 </param>
		Protected Friend Overridable Function yPositionForValue(ByVal value As Integer, ByVal trackY As Integer, ByVal trackHeight As Integer) As Integer
			Dim min As Integer = slider.minimum
			Dim max As Integer = slider.maximum
			Dim valueRange As Double = CDbl(max) - CDbl(min)
			Dim pixelsPerValue As Double = CDbl(trackHeight) / valueRange
			Dim trackBottom As Integer = trackY + (trackHeight - 1)
			Dim yPosition As Integer

			If Not drawInverted() Then
				yPosition = trackY
				yPosition += Math.Round(pixelsPerValue * (CDbl(max) - value))
			Else
				yPosition = trackY
				yPosition += Math.Round(pixelsPerValue * (CDbl(value) - min))
			End If

			yPosition = Math.Max(trackY, yPosition)
			yPosition = Math.Min(trackBottom, yPosition)

			Return yPosition
		End Function

		''' <summary>
		''' Returns the value at the y position. If {@code yPos} is beyond the
		''' track at the the bottom or the top, this method sets the value to either
		''' the minimum or maximum value of the slider, depending on if the slider
		''' is inverted or not.
		''' </summary>
		Public Overridable Function valueForYPosition(ByVal yPos As Integer) As Integer
			Dim value As Integer
			Dim minValue As Integer = slider.minimum
			Dim maxValue As Integer = slider.maximum
			Dim trackLength As Integer = trackRect.height
			Dim trackTop As Integer = trackRect.y
			Dim trackBottom As Integer = trackRect.y + (trackRect.height - 1)

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
		''' Returns the value at the x position.  If {@code xPos} is beyond the
		''' track at the left or the right, this method sets the value to either the
		''' minimum or maximum value of the slider, depending on if the slider is
		''' inverted or not.
		''' </summary>
		Public Overridable Function valueForXPosition(ByVal xPos As Integer) As Integer
			Dim value As Integer
			Dim minValue As Integer = slider.minimum
			Dim maxValue As Integer = slider.maximum
			Dim trackLength As Integer = trackRect.width
			Dim trackLeft As Integer = trackRect.x
			Dim trackRight As Integer = trackRect.x + (trackRect.width - 1)

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


		Private Class Handler
			Implements ChangeListener, ComponentListener, FocusListener, PropertyChangeListener

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			' Change Handler
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If Not outerInstance.___isDragging Then
					outerInstance.calculateThumbLocation()
					outerInstance.slider.repaint()
				End If
				outerInstance.lastValue = outerInstance.slider.value
			End Sub

			' Component Handler
			Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
			End Sub
			Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
			End Sub
			Public Overridable Sub componentResized(ByVal e As ComponentEvent)
				outerInstance.calculateGeometry()
				outerInstance.slider.repaint()
			End Sub
			Public Overridable Sub componentShown(ByVal e As ComponentEvent)
			End Sub

			' Focus Handler
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.slider.repaint()
			End Sub
			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.slider.repaint()
			End Sub

			' Property Change Handler
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				If propertyName = "orientation" OrElse propertyName = "inverted" OrElse propertyName = "labelTable" OrElse propertyName = "majorTickSpacing" OrElse propertyName = "minorTickSpacing" OrElse propertyName = "paintTicks" OrElse propertyName = "paintTrack" OrElse propertyName = "font" OrElse propertyName = "paintLabels" OrElse propertyName = "Slider.paintThumbArrowShape" Then
					outerInstance.checkedLabelBaselines = False
					outerInstance.calculateGeometry()
					outerInstance.slider.repaint()
				ElseIf propertyName = "componentOrientation" Then
					outerInstance.calculateGeometry()
					outerInstance.slider.repaint()
					Dim km As InputMap = outerInstance.getInputMap(JComponent.WHEN_FOCUSED, outerInstance.slider)
					SwingUtilities.replaceUIInputMap(outerInstance.slider, JComponent.WHEN_FOCUSED, km)
				ElseIf propertyName = "model" Then
					CType(e.oldValue, BoundedRangeModel).removeChangeListener(outerInstance.changeListener)
					CType(e.newValue, BoundedRangeModel).addChangeListener(outerInstance.changeListener)
					outerInstance.calculateThumbLocation()
					outerInstance.slider.repaint()
				End If
			End Sub
		End Class

		'///////////////////////////////////////////////////////////////////////
		'/ Model Listener Class
		'///////////////////////////////////////////////////////////////////////
		''' <summary>
		''' Data model listener.
		''' 
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class ChangeHandler
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class

		'///////////////////////////////////////////////////////////////////////
		'/ Track Listener Class
		'///////////////////////////////////////////////////////////////////////
		''' <summary>
		''' Track mouse movements.
		''' 
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class TrackListener
			Inherits MouseInputAdapter

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			<NonSerialized> _
			Protected Friend offset As Integer
			<NonSerialized> _
			Protected Friend currentMouseX, currentMouseY As Integer

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				If Not outerInstance.slider.enabled Then Return

				offset = 0
				outerInstance.scrollTimer.stop()

				outerInstance.___isDragging = False
				outerInstance.slider.valueIsAdjusting = False
				outerInstance.slider.repaint()
			End Sub

			''' <summary>
			''' If the mouse is pressed above the "thumb" component
			''' then reduce the scrollbars value by one page ("page up"),
			''' otherwise increase it by one page.  If there is no
			''' thumb then page up if the mouse is in the upper half
			''' of the track.
			''' </summary>
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If Not outerInstance.slider.enabled Then Return

				' We should recalculate geometry just before
				' calculation of the thumb movement direction.
				' It is important for the case, when JSlider
				' is a cell editor in JTable. See 6348946.
				outerInstance.calculateGeometry()

				currentMouseX = e.x
				currentMouseY = e.y

				If outerInstance.slider.requestFocusEnabled Then outerInstance.slider.requestFocus()

				' Clicked in the Thumb area?
				If outerInstance.thumbRect.contains(currentMouseX, currentMouseY) Then
					If UIManager.getBoolean("Slider.onlyLeftMouseButtonDrag") AndAlso (Not SwingUtilities.isLeftMouseButton(e)) Then Return

					Select Case outerInstance.slider.orientation
					Case JSlider.VERTICAL
						offset = currentMouseY - outerInstance.thumbRect.y
					Case JSlider.HORIZONTAL
						offset = currentMouseX - outerInstance.thumbRect.x
					End Select
					outerInstance.___isDragging = True
					Return
				End If

				If Not SwingUtilities.isLeftMouseButton(e) Then Return

				outerInstance.___isDragging = False
				outerInstance.slider.valueIsAdjusting = True

				Dim sbSize As Dimension = outerInstance.slider.size
				Dim direction As Integer = POSITIVE_SCROLL

				Select Case outerInstance.slider.orientation
				Case JSlider.VERTICAL
					If outerInstance.thumbRect.empty Then
						Dim scrollbarCenter As Integer = sbSize.height / 2
						If Not outerInstance.drawInverted() Then
							direction = If(currentMouseY < scrollbarCenter, POSITIVE_SCROLL, NEGATIVE_SCROLL)
						Else
							direction = If(currentMouseY < scrollbarCenter, NEGATIVE_SCROLL, POSITIVE_SCROLL)
						End If
					Else
						Dim thumbY As Integer = outerInstance.thumbRect.y
						If Not outerInstance.drawInverted() Then
							direction = If(currentMouseY < thumbY, POSITIVE_SCROLL, NEGATIVE_SCROLL)
						Else
							direction = If(currentMouseY < thumbY, NEGATIVE_SCROLL, POSITIVE_SCROLL)
						End If
					End If
				Case JSlider.HORIZONTAL
					If outerInstance.thumbRect.empty Then
						Dim scrollbarCenter As Integer = sbSize.width / 2
						If Not outerInstance.drawInverted() Then
							direction = If(currentMouseX < scrollbarCenter, NEGATIVE_SCROLL, POSITIVE_SCROLL)
						Else
							direction = If(currentMouseX < scrollbarCenter, POSITIVE_SCROLL, NEGATIVE_SCROLL)
						End If
					Else
						Dim thumbX As Integer = outerInstance.thumbRect.x
						If Not outerInstance.drawInverted() Then
							direction = If(currentMouseX < thumbX, NEGATIVE_SCROLL, POSITIVE_SCROLL)
						Else
							direction = If(currentMouseX < thumbX, POSITIVE_SCROLL, NEGATIVE_SCROLL)
						End If
					End If
				End Select

				If shouldScroll(direction) Then outerInstance.scrollDueToClickInTrack(direction)
				If shouldScroll(direction) Then
					outerInstance.scrollTimer.stop()
					outerInstance.scrollListener.direction = direction
					outerInstance.scrollTimer.start()
				End If
			End Sub

			Public Overridable Function shouldScroll(ByVal direction As Integer) As Boolean
				Dim r As Rectangle = outerInstance.thumbRect
				If outerInstance.slider.orientation = JSlider.VERTICAL Then
					If If(outerInstance.drawInverted(), direction < 0, direction > 0) Then
						If r.y <= currentMouseY Then Return False
					ElseIf r.y + r.height >= currentMouseY Then
						Return False
					End If
				Else
					If If(outerInstance.drawInverted(), direction < 0, direction > 0) Then
						If r.x + r.width >= currentMouseX Then Return False
					ElseIf r.x <= currentMouseX Then
						Return False
					End If
				End If

				If direction > 0 AndAlso outerInstance.slider.value + outerInstance.slider.extent >= outerInstance.slider.maximum Then
					Return False
				ElseIf direction < 0 AndAlso outerInstance.slider.value <= outerInstance.slider.minimum Then
					Return False
				End If

				Return True
			End Function

			''' <summary>
			''' Set the models value to the position of the top/left
			''' of the thumb relative to the origin of the track.
			''' </summary>
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				Dim thumbMiddle As Integer

				If Not outerInstance.slider.enabled Then Return

				currentMouseX = e.x
				currentMouseY = e.y

				If Not outerInstance.___isDragging Then Return

				outerInstance.slider.valueIsAdjusting = True

				Select Case outerInstance.slider.orientation
				Case JSlider.VERTICAL
					Dim halfThumbHeight As Integer = outerInstance.thumbRect.height / 2
					Dim thumbTop As Integer = e.y - offset
					Dim trackTop As Integer = outerInstance.trackRect.y
					Dim trackBottom As Integer = outerInstance.trackRect.y + (outerInstance.trackRect.height - 1)
					Dim vMax As Integer = outerInstance.yPositionForValue(outerInstance.slider.maximum - outerInstance.slider.extent)

					If outerInstance.drawInverted() Then
						trackBottom = vMax
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
					Dim trackLeft As Integer = outerInstance.trackRect.x
					Dim trackRight As Integer = outerInstance.trackRect.x + (outerInstance.trackRect.width - 1)
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
				End Select
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			End Sub
		End Class

		''' <summary>
		''' Scroll-event listener.
		''' 
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class ScrollListener
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSliderUI

			' changed this class to public to avoid bogus IllegalAccessException
			' bug in InternetExplorer browser.  It was protected.  Work around
			' for 4109432
			Friend direction As Integer = POSITIVE_SCROLL
			Friend useBlockIncrement As Boolean

			Public Sub New(ByVal outerInstance As BasicSliderUI)
					Me.outerInstance = outerInstance
				direction = POSITIVE_SCROLL
				useBlockIncrement = True
			End Sub

			Public Sub New(ByVal outerInstance As BasicSliderUI, ByVal dir As Integer, ByVal block As Boolean)
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
				Else
					outerInstance.scrollByUnit(direction)
				End If
				If Not outerInstance.trackListener.shouldScroll(direction) Then CType(e.source, Timer).stop()
			End Sub
		End Class

		''' <summary>
		''' Listener for resizing events.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class ComponentHandler
			Inherits ComponentAdapter

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub componentResized(ByVal e As ComponentEvent)
				outerInstance.handler.componentResized(e)
			End Sub
		End Class

		''' <summary>
		''' Focus-change listener.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class FocusHandler
			Implements FocusListener

			Private ReadOnly outerInstance As BasicSliderUI

			Public Sub New(ByVal outerInstance As BasicSliderUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

		''' <summary>
		''' As of Java 2 platform v1.3 this undocumented class is no longer used.
		''' The recommended approach to creating bindings is to use a
		''' combination of an <code>ActionMap</code>, to contain the action,
		''' and an <code>InputMap</code> to contain the mapping from KeyStroke
		''' to action description. The InputMap is is usually described in the
		''' LookAndFeel tables.
		''' <p>
		''' Please refer to the key bindings specification for further details.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of <code>Foo</code>.
		''' </summary>
		Public Class ActionScroller
			Inherits AbstractAction

			Private ReadOnly outerInstance As BasicSliderUI

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Actions. If you need to add
			' new functionality add it to the Actions, but make sure this
			' class calls into the Actions.
			Friend dir As Integer
			Friend block As Boolean
			Friend slider As JSlider

			Public Sub New(ByVal outerInstance As BasicSliderUI, ByVal slider As JSlider, ByVal dir As Integer, ByVal block As Boolean)
					Me.outerInstance = outerInstance
				Me.dir = dir
				Me.block = block
				Me.slider = slider
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				SHARED_ACTION.scroll(slider, BasicSliderUI.this, dir, block)
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim b As Boolean = True
					If slider IsNot Nothing Then b = slider.enabled
					Return b
				End Get
			End Property

		End Class


		''' <summary>
		''' A static version of the above.
		''' </summary>
		Friend Class SharedActionScroller
			Inherits AbstractAction

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Actions. If you need to add
			' new functionality add it to the Actions, but make sure this
			' class calls into the Actions.
			Friend dir As Integer
			Friend block As Boolean

			Public Sub New(ByVal dir As Integer, ByVal block As Boolean)
				Me.dir = dir
				Me.block = block
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim slider As JSlider = CType(evt.source, JSlider)
				Dim ui As BasicSliderUI = CType(BasicLookAndFeel.getUIOfType(slider.uI, GetType(BasicSliderUI)), BasicSliderUI)
				If ui Is Nothing Then Return
				SHARED_ACTION.scroll(slider, ui, dir, block)
			End Sub
		End Class

		Private Class Actions
			Inherits sun.swing.UIAction

			Public Const POSITIVE_UNIT_INCREMENT As String = "positiveUnitIncrement"
			Public Const POSITIVE_BLOCK_INCREMENT As String = "positiveBlockIncrement"
			Public Const NEGATIVE_UNIT_INCREMENT As String = "negativeUnitIncrement"
			Public Const NEGATIVE_BLOCK_INCREMENT As String = "negativeBlockIncrement"
			Public Const MIN_SCROLL_INCREMENT As String = "minScroll"
			Public Const MAX_SCROLL_INCREMENT As String = "maxScroll"


			Friend Sub New()
				MyBase.New(Nothing)
			End Sub

			Public Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

			Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
				Dim slider As JSlider = CType(evt.source, JSlider)
				Dim ui As BasicSliderUI = CType(BasicLookAndFeel.getUIOfType(slider.uI, GetType(BasicSliderUI)), BasicSliderUI)
				Dim name As String = name

				If ui Is Nothing Then Return
				If POSITIVE_UNIT_INCREMENT = name Then
					scroll(slider, ui, POSITIVE_SCROLL, False)
				ElseIf NEGATIVE_UNIT_INCREMENT = name Then
					scroll(slider, ui, NEGATIVE_SCROLL, False)
				ElseIf POSITIVE_BLOCK_INCREMENT = name Then
					scroll(slider, ui, POSITIVE_SCROLL, True)
				ElseIf NEGATIVE_BLOCK_INCREMENT = name Then
					scroll(slider, ui, NEGATIVE_SCROLL, True)
				ElseIf MIN_SCROLL_INCREMENT = name Then
					scroll(slider, ui, MIN_SCROLL, False)
				ElseIf MAX_SCROLL_INCREMENT = name Then
					scroll(slider, ui, MAX_SCROLL, False)
				End If
			End Sub

			Private Sub scroll(ByVal slider As JSlider, ByVal ui As BasicSliderUI, ByVal direction As Integer, ByVal isBlock As Boolean)
				Dim invert As Boolean = slider.inverted

				If direction = NEGATIVE_SCROLL OrElse direction = POSITIVE_SCROLL Then
					If invert Then direction = If(direction = POSITIVE_SCROLL, NEGATIVE_SCROLL, POSITIVE_SCROLL)

					If isBlock Then
						ui.scrollByBlock(direction)
					Else
						ui.scrollByUnit(direction)
					End If ' MIN or MAX
				Else
					If invert Then direction = If(direction = MIN_SCROLL, MAX_SCROLL, MIN_SCROLL)

					slider.value = If(direction = MIN_SCROLL, slider.minimum, slider.maximum)
				End If
			End Sub
		End Class
	End Class

End Namespace