Imports System
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf.basic

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
	''' Metal's split pane divider
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
	''' @author Steve Wilson
	''' @author Ralph kar
	''' </summary>
	Friend Class MetalSplitPaneDivider
		Inherits BasicSplitPaneDivider

		Private bumps As New MetalBumps(10, 10, MetalLookAndFeel.controlHighlight, MetalLookAndFeel.controlDarkShadow, MetalLookAndFeel.control)

		Private focusBumps As New MetalBumps(10, 10, MetalLookAndFeel.primaryControlHighlight, MetalLookAndFeel.primaryControlDarkShadow, UIManager.getColor("SplitPane.dividerFocusColor"))

		Private inset As Integer = 2

		Private controlColor As Color = MetalLookAndFeel.control
		Private primaryControlColor As Color = UIManager.getColor("SplitPane.dividerFocusColor")

		Public Sub New(ByVal ui As BasicSplitPaneUI)
			MyBase.New(ui)
		End Sub

		Public Overrides Sub paint(ByVal g As Graphics)
			Dim usedBumps As MetalBumps
			If splitPane.hasFocus() Then
				usedBumps = focusBumps
				g.color = primaryControlColor
			Else
				usedBumps = bumps
				g.color = controlColor
			End If
			Dim clip As Rectangle = g.clipBounds
			Dim ___insets As Insets = insets
			g.fillRect(clip.x, clip.y, clip.width, clip.height)
			Dim size As Dimension = size
			size.width -= inset * 2
			size.height -= inset * 2
			Dim drawX As Integer = inset
			Dim drawY As Integer = inset
			If ___insets IsNot Nothing Then
				size.width -= (___insets.left + ___insets.right)
				size.height -= (___insets.top + ___insets.bottom)
				drawX += ___insets.left
				drawY += ___insets.top
			End If
			usedBumps.bumpArea = size
			usedBumps.paintIcon(Me, g, drawX, drawY)
			MyBase.paint(g)
		End Sub

		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the left component in the metal split pane.
		''' </summary>
		Protected Friend Overrides Function createLeftOneTouchButton() As JButton
			Dim b As JButton = New JButtonAnonymousInnerClassHelper
			b.requestFocusEnabled = False
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			maybeMakeButtonOpaque(b)
			Return b
		End Function

		Private Class JButtonAnonymousInnerClassHelper
			Inherits JButton

					' Sprite buffer for the arrow image of the left button
			Friend buffer As Integer()() = { New Integer() { 0, 0, 0, 2, 2, 0, 0, 0, 0 }, New Integer() { 0, 0, 2, 1, 1, 1, 0, 0, 0 }, New Integer() { 0, 2, 1, 1, 1, 1, 1, 0, 0 }, New Integer() { 2, 1, 1, 1, 1, 1, 1, 1, 0 }, New Integer() { 0, 3, 3, 3, 3, 3, 3, 3, 3 } }

			Public Overrides Property border As Border
				Set(ByVal b As Border)
				End Set
			End Property

			Public Overrides Sub paint(ByVal g As Graphics)
				Dim splitPane As JSplitPane = outerInstance.splitPaneFromSuper
				If splitPane IsNot Nothing Then
					Dim oneTouchSize As Integer = outerInstance.oneTouchSizeFromSuper
					Dim orientation As Integer = outerInstance.orientationFromSuper
					Dim blockSize As Integer = Math.Min(outerInstance.dividerSize, oneTouchSize)

					' Initialize the color array
					Dim colors As Color() = { Me.background, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.primaryControlHighlight}

					' Fill the background first ...
					g.color = Me.background
					If opaque Then g.fillRect(0, 0, Me.width, Me.height)

					' ... then draw the arrow.
					If model.pressed Then colors(1) = colors(2)
					If orientation = JSplitPane.VERTICAL_SPLIT Then
							' Draw the image for a vertical split
							For i As Integer = 1 To buffer(0).length
									For j As Integer = 1 To blockSize - 1
											If buffer(j-1)(i-1) = 0 Then
													Continue For
											Else
												g.color = colors(buffer(j-1)(i-1))
											End If
											g.drawLine(i, j, i, j)
									Next j
							Next i
					Else
							' Draw the image for a horizontal split
							' by simply swaping the i and j axis.
							' Except the drawLine() call this code is
							' identical to the code block above. This was done
							' in order to remove the additional orientation
							' check for each pixel.
							For i As Integer = 1 To buffer(0).length
									For j As Integer = 1 To blockSize - 1
											If buffer(j-1)(i-1) = 0 Then
													' Nothing needs
													' to be drawn
													Continue For
											Else
													' Set the color from the
													' color map
													g.color = colors(buffer(j-1)(i-1))
											End If
											' Draw a pixel
											g.drawLine(j, i, j, i)
									Next j
							Next i
					End If
				End If
			End Sub

			' Don't want the button to participate in focus traversable.
			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' If necessary <code>c</code> is made opaque.
		''' </summary>
		Private Sub maybeMakeButtonOpaque(ByVal c As JComponent)
			Dim opaque As Object = UIManager.get("SplitPane.oneTouchButtonsOpaque")
			If opaque IsNot Nothing Then c.opaque = CBool(opaque)
		End Sub

		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the right component in the metal split pane.
		''' </summary>
		Protected Friend Overrides Function createRightOneTouchButton() As JButton
			Dim b As JButton = New JButtonAnonymousInnerClassHelper2
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			b.requestFocusEnabled = False
			maybeMakeButtonOpaque(b)
			Return b
		End Function

		Private Class JButtonAnonymousInnerClassHelper2
			Inherits JButton

					' Sprite buffer for the arrow image of the right button
			Friend buffer As Integer()() = { New Integer() { 2, 2, 2, 2, 2, 2, 2, 2 }, New Integer() { 0, 1, 1, 1, 1, 1, 1, 3 }, New Integer() { 0, 0, 1, 1, 1, 1, 3, 0 }, New Integer() { 0, 0, 0, 1, 1, 3, 0, 0 }, New Integer() { 0, 0, 0, 0, 3, 0, 0, 0 } }

			Public Overrides Property border As Border
				Set(ByVal border As Border)
				End Set
			End Property

			Public Overrides Sub paint(ByVal g As Graphics)
				Dim splitPane As JSplitPane = outerInstance.splitPaneFromSuper
				If splitPane IsNot Nothing Then
					Dim oneTouchSize As Integer = outerInstance.oneTouchSizeFromSuper
					Dim orientation As Integer = outerInstance.orientationFromSuper
					Dim blockSize As Integer = Math.Min(outerInstance.dividerSize, oneTouchSize)

					' Initialize the color array
					Dim colors As Color() = { Me.background, MetalLookAndFeel.primaryControlDarkShadow, MetalLookAndFeel.primaryControlInfo, MetalLookAndFeel.primaryControlHighlight}

					' Fill the background first ...
					g.color = Me.background
					If opaque Then g.fillRect(0, 0, Me.width, Me.height)

					' ... then draw the arrow.
					If model.pressed Then colors(1) = colors(2)
					If orientation = JSplitPane.VERTICAL_SPLIT Then
							' Draw the image for a vertical split
							For i As Integer = 1 To buffer(0).length
									For j As Integer = 1 To blockSize - 1
											If buffer(j-1)(i-1) = 0 Then
													Continue For
											Else
												g.color = colors(buffer(j-1)(i-1))
											End If
											g.drawLine(i, j, i, j)
									Next j
							Next i
					Else
							' Draw the image for a horizontal split
							' by simply swaping the i and j axis.
							' Except the drawLine() call this code is
							' identical to the code block above. This was done
							' in order to remove the additional orientation
							' check for each pixel.
							For i As Integer = 1 To buffer(0).length
									For j As Integer = 1 To blockSize - 1
											If buffer(j-1)(i-1) = 0 Then
													' Nothing needs
													' to be drawn
													Continue For
											Else
													' Set the color from the
													' color map
													g.color = colors(buffer(j-1)(i-1))
											End If
											' Draw a pixel
											g.drawLine(j, i, j, i)
									Next j
							Next i
					End If
				End If
			End Sub

			' Don't want the button to participate in focus traversable.
			Public Overridable Property focusTraversable As Boolean
				Get
					Return False
				End Get
			End Property
		End Class

		''' <summary>
		''' Used to layout a MetalSplitPaneDivider. Layout for the divider
		''' involves appropriately moving the left/right buttons around.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of MetalSplitPaneDivider.
		''' </summary>
		Public Class MetalDividerLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As MetalSplitPaneDivider

			Public Sub New(ByVal outerInstance As MetalSplitPaneDivider)
				Me.outerInstance = outerInstance
			End Sub


			' NOTE NOTE NOTE NOTE NOTE
			' This class is no longer used, the functionality has
			' been rolled into BasicSplitPaneDivider.DividerLayout as a
			' defaults property

			Public Overridable Sub layoutContainer(ByVal c As Container)
				Dim leftButton As JButton = outerInstance.leftButtonFromSuper
				Dim rightButton As JButton = outerInstance.rightButtonFromSuper
				Dim splitPane As JSplitPane = outerInstance.splitPaneFromSuper
				Dim orientation As Integer = outerInstance.orientationFromSuper
				Dim oneTouchSize As Integer = outerInstance.oneTouchSizeFromSuper
				Dim oneTouchOffset As Integer = outerInstance.oneTouchOffsetFromSuper
				Dim insets As Insets = outerInstance.insets

				' This layout differs from the one used in BasicSplitPaneDivider.
				' It does not center justify the oneTouchExpadable buttons.
				' This was necessary in order to meet the spec of the Metal
				' splitpane divider.
				If leftButton IsNot Nothing AndAlso rightButton IsNot Nothing AndAlso c Is MetalSplitPaneDivider.this Then
					If splitPane.oneTouchExpandable Then
						If orientation = JSplitPane.VERTICAL_SPLIT Then
							Dim extraY As Integer = If(insets IsNot Nothing, insets.top, 0)
							Dim blockSize As Integer = outerInstance.dividerSize

							If insets IsNot Nothing Then blockSize -= (insets.top + insets.bottom)
							blockSize = Math.Min(blockSize, oneTouchSize)
							leftButton.boundsnds(oneTouchOffset, extraY, blockSize * 2, blockSize)
							rightButton.boundsnds(oneTouchOffset + oneTouchSize * 2, extraY, blockSize * 2, blockSize)
						Else
							Dim blockSize As Integer = outerInstance.dividerSize
							Dim extraX As Integer = If(insets IsNot Nothing, insets.left, 0)

							If insets IsNot Nothing Then blockSize -= (insets.left + insets.right)
							blockSize = Math.Min(blockSize, oneTouchSize)
							leftButton.boundsnds(extraX, oneTouchOffset, blockSize, blockSize * 2)
							rightButton.boundsnds(extraX, oneTouchOffset + oneTouchSize * 2, blockSize, blockSize * 2)
						End If
					Else
						leftButton.boundsnds(-5, -5, 1, 1)
						rightButton.boundsnds(-5, -5, 1, 1)
					End If
				End If
			End Sub

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				Return New Dimension(0,0)
			End Function

			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return New Dimension(0, 0)
			End Function

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal [string] As String, ByVal c As Component)
			End Sub
		End Class

	'    
	'     * The following methods only exist in order to be able to access protected
	'     * members in the superclass, because these are otherwise not available
	'     * in any inner class.
	'     

		Friend Overridable Property oneTouchSizeFromSuper As Integer
			Get
				Return MyBase.ONE_TOUCH_SIZE
			End Get
		End Property

		Friend Overridable Property oneTouchOffsetFromSuper As Integer
			Get
				Return MyBase.ONE_TOUCH_OFFSET
			End Get
		End Property

		Friend Overridable Property orientationFromSuper As Integer
			Get
				Return MyBase.orientation
			End Get
		End Property

		Friend Overridable Property splitPaneFromSuper As JSplitPane
			Get
				Return MyBase.splitPane
			End Get
		End Property

		Friend Overridable Property leftButtonFromSuper As JButton
			Get
				Return MyBase.leftButton
			End Get
		End Property

		Friend Overridable Property rightButtonFromSuper As JButton
			Get
				Return MyBase.rightButton
			End Get
		End Property
	End Class

End Namespace