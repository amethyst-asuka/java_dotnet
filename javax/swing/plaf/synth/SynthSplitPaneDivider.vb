Imports javax.swing
Imports javax.swing.plaf.basic

'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Synth's SplitPaneDivider.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class SynthSplitPaneDivider
		Inherits BasicSplitPaneDivider

		Public Sub New(ByVal ui As BasicSplitPaneUI)
			MyBase.New(ui)
		End Sub

		Protected Friend Overrides Property mouseOver As Boolean
			Set(ByVal mouseOver As Boolean)
				If mouseOver <> mouseOver Then repaint()
				MyBase.mouseOver = mouseOver
			End Set
		End Property

		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			MyBase.propertyChange(e)
			If e.source Is splitPane Then
				If e.propertyName = JSplitPane.ORIENTATION_PROPERTY Then
					If TypeOf leftButton Is SynthArrowButton Then CType(leftButton, SynthArrowButton).direction = mapDirection(True)
					If TypeOf rightButton Is SynthArrowButton Then CType(rightButton, SynthArrowButton).direction = mapDirection(False)
				End If
			End If
		End Sub

		Public Overrides Sub paint(ByVal g As Graphics)
			Dim g2 As Graphics = g.create()

			Dim context As SynthContext = CType(splitPaneUI, SynthSplitPaneUI).getContext(splitPane, Region.SPLIT_PANE_DIVIDER)
			Dim bounds As Rectangle = bounds
				bounds.y = 0
				bounds.x = bounds.y
			SynthLookAndFeel.updateSubregion(context, g, bounds)
			context.painter.paintSplitPaneDividerBackground(context, g, 0, 0, bounds.width, bounds.height, splitPane.orientation)

			Dim foreground As SynthPainter = Nothing

			context.painter.paintSplitPaneDividerForeground(context, g, 0, 0, width, height, splitPane.orientation)
			context.Dispose()

			' super.paint(g2);
			For counter As Integer = 0 To componentCount - 1
				Dim child As Component = getComponent(counter)
				Dim childBounds As Rectangle = child.bounds
				Dim childG As Graphics = g.create(childBounds.x, childBounds.y, childBounds.width, childBounds.height)
				child.paint(childG)
				childG.Dispose()
			Next counter
			g2.Dispose()
		End Sub

		Private Function mapDirection(ByVal isLeft As Boolean) As Integer
			If isLeft Then
				If splitPane.orientation = JSplitPane.HORIZONTAL_SPLIT Then Return SwingConstants.WEST
				Return SwingConstants.NORTH
			End If
			If splitPane.orientation = JSplitPane.HORIZONTAL_SPLIT Then Return SwingConstants.EAST
			Return SwingConstants.SOUTH
		End Function


		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the left component in the split pane.
		''' </summary>
		Protected Friend Overrides Function createLeftOneTouchButton() As JButton
			Dim b As New SynthArrowButton(SwingConstants.NORTH)
			Dim oneTouchSize As Integer = lookupOneTouchSize()

			b.name = "SplitPaneDivider.leftOneTouchButton"
			b.minimumSize = New Dimension(oneTouchSize, oneTouchSize)
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			b.requestFocusEnabled = False
			b.direction = mapDirection(True)
			Return b
		End Function

		Private Function lookupOneTouchSize() As Integer
			Return sun.swing.DefaultLookup.getInt(splitPaneUI.splitPane, splitPaneUI, "SplitPaneDivider.oneTouchButtonSize", ONE_TOUCH_SIZE)
		End Function

		''' <summary>
		''' Creates and return an instance of JButton that can be used to
		''' collapse the right component in the split pane.
		''' </summary>
		Protected Friend Overrides Function createRightOneTouchButton() As JButton
			Dim b As New SynthArrowButton(SwingConstants.NORTH)
			Dim oneTouchSize As Integer = lookupOneTouchSize()

			b.name = "SplitPaneDivider.rightOneTouchButton"
			b.minimumSize = New Dimension(oneTouchSize, oneTouchSize)
			b.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			b.focusPainted = False
			b.borderPainted = False
			b.requestFocusEnabled = False
			b.direction = mapDirection(False)
			Return b
		End Function
	End Class

End Namespace