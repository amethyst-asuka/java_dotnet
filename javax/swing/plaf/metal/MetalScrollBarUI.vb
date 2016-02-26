Imports Microsoft.VisualBasic
Imports System
import static sun.swing.SwingUtilities2.drawHLine
import static sun.swing.SwingUtilities2.drawRect
import static sun.swing.SwingUtilities2.drawVLine

'
' * Copyright (c) 1998, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' Implementation of ScrollBarUI for the Metal Look and Feel
	''' <p>
	''' 
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalScrollBarUI
		Inherits javax.swing.plaf.basic.BasicScrollBarUI

		Private Shared shadowColor As java.awt.Color
		Private Shared highlightColor As java.awt.Color
		Private Shared darkShadowColor As java.awt.Color
		Private Shared Shadows thumbColor As java.awt.Color
		Private Shared thumbShadow As java.awt.Color
		Private Shared Shadows thumbHighlightColor As java.awt.Color


		Protected Friend bumps As MetalBumps

		Protected Friend increaseButton As MetalScrollButton
		Protected Friend decreaseButton As MetalScrollButton

		Protected Friend Shadows scrollBarWidth As Integer

		Public Const FREE_STANDING_PROP As String = "JScrollBar.isFreeStanding"
		Protected Friend isFreeStanding As Boolean = True

		Public Shared Function createUI(ByVal c As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New MetalScrollBarUI
		End Function

		Protected Friend Overrides Sub installDefaults()
			scrollBarWidth = CInt(Fix(javax.swing.UIManager.get("ScrollBar.width")))
			MyBase.installDefaults()
			bumps = New MetalBumps(10, 10, thumbHighlightColor, thumbShadow, thumbColor)
		End Sub

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			CType(propertyChangeListener, ScrollBarListener).handlePropertyChange(scrollbar.getClientProperty(FREE_STANDING_PROP))
		End Sub

		Protected Friend Overrides Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return New ScrollBarListener(Me)
		End Function

		Protected Friend Overrides Sub configureScrollBarColors()
			MyBase.configureScrollBarColors()
			shadowColor = javax.swing.UIManager.getColor("ScrollBar.shadow")
			highlightColor = javax.swing.UIManager.getColor("ScrollBar.highlight")
			darkShadowColor = javax.swing.UIManager.getColor("ScrollBar.darkShadow")
			thumbColor = javax.swing.UIManager.getColor("ScrollBar.thumb")
			thumbShadow = javax.swing.UIManager.getColor("ScrollBar.thumbShadow")
			thumbHighlightColor = javax.swing.UIManager.getColor("ScrollBar.thumbHighlight")


		End Sub

		Public Overridable Function getPreferredSize(ByVal c As javax.swing.JComponent) As java.awt.Dimension
			If scrollbar.orientation = javax.swing.JScrollBar.VERTICAL Then
				Return New java.awt.Dimension(scrollBarWidth, scrollBarWidth * 3 + 10)
			Else ' Horizontal
				Return New java.awt.Dimension(scrollBarWidth * 3 + 10, scrollBarWidth)
			End If

		End Function

		''' <summary>
		''' Returns the view that represents the decrease view.
		''' </summary>
		Protected Friend Overrides Function createDecreaseButton(ByVal orientation As Integer) As javax.swing.JButton
			decreaseButton = New MetalScrollButton(orientation, scrollBarWidth, isFreeStanding)
			Return decreaseButton
		End Function

		''' <summary>
		''' Returns the view that represents the increase view. </summary>
		Protected Friend Overrides Function createIncreaseButton(ByVal orientation As Integer) As javax.swing.JButton
			increaseButton = New MetalScrollButton(orientation, scrollBarWidth, isFreeStanding)
			Return increaseButton
		End Function

		Protected Friend Overridable Sub paintTrack(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent, ByVal trackBounds As java.awt.Rectangle)
			g.translate(trackBounds.x, trackBounds.y)

			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(c)

			If scrollbar.orientation = javax.swing.JScrollBar.VERTICAL Then
				If Not isFreeStanding Then
					trackBounds.width += 2
					If Not leftToRight Then g.translate(-1, 0)
				End If

				If c.enabled Then
					g.color = darkShadowColor
					drawVLine(g, 0, 0, trackBounds.height - 1)
					drawVLine(g, trackBounds.width - 2, 0, trackBounds.height - 1)
					drawHLine(g, 2, trackBounds.width - 1, trackBounds.height - 1)
					drawHLine(g, 2, trackBounds.width - 2, 0)

					g.color = shadowColor
					'      g.setColor( Color.red);
					drawVLine(g, 1, 1, trackBounds.height - 2)
					drawHLine(g, 1, trackBounds.width - 3, 1)
					If scrollbar.value <> scrollbar.maximum Then ' thumb shadow
						Dim y As Integer = thumbRect.y + thumbRect.height - trackBounds.y
						drawHLine(g, 1, trackBounds.width - 1, y)
					End If
					g.color = highlightColor
					drawVLine(g, trackBounds.width - 1, 0, trackBounds.height - 1)
				Else
					MetalUtils.drawDisabledBorder(g, 0, 0, trackBounds.width, trackBounds.height)
				End If

				If Not isFreeStanding Then
					trackBounds.width -= 2
					If Not leftToRight Then g.translate(1, 0)
				End If
			Else ' HORIZONTAL
				If Not isFreeStanding Then trackBounds.height += 2

				If c.enabled Then
					g.color = darkShadowColor
					drawHLine(g, 0, trackBounds.width - 1, 0) ' top
					drawVLine(g, 0, 2, trackBounds.height - 2) ' left
					drawHLine(g, 0, trackBounds.width - 1, trackBounds.height - 2) ' bottom
					drawVLine(g, trackBounds.width - 1, 2, trackBounds.height - 1) ' right

					g.color = shadowColor
					'      g.setColor( Color.red);
					drawHLine(g, 1, trackBounds.width - 2, 1) ' top
					drawVLine(g, 1, 1, trackBounds.height - 3) ' left
					drawHLine(g, 0, trackBounds.width - 1, trackBounds.height - 1) ' bottom
					If scrollbar.value <> scrollbar.maximum Then ' thumb shadow
						Dim x As Integer = thumbRect.x + thumbRect.width - trackBounds.x
						drawVLine(g, x, 1, trackBounds.height-1)
					End If
				Else
					MetalUtils.drawDisabledBorder(g, 0, 0, trackBounds.width, trackBounds.height)
				End If

				If Not isFreeStanding Then trackBounds.height -= 2
			End If

			g.translate(-trackBounds.x, -trackBounds.y)
		End Sub

		Protected Friend Overridable Sub paintThumb(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent, ByVal thumbBounds As java.awt.Rectangle)
			If Not c.enabled Then Return

			If MetalLookAndFeel.usingOcean() Then
				oceanPaintThumb(g, c, thumbBounds)
				Return
			End If

			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(c)

			g.translate(thumbBounds.x, thumbBounds.y)

			If scrollbar.orientation = javax.swing.JScrollBar.VERTICAL Then
				If Not isFreeStanding Then
					thumbBounds.width += 2
					If Not leftToRight Then g.translate(-1, 0)
				End If

				g.color = thumbColor
				g.fillRect(0, 0, thumbBounds.width - 2, thumbBounds.height - 1)

				g.color = thumbShadow
				drawRect(g, 0, 0, thumbBounds.width - 2, thumbBounds.height - 1)

				g.color = thumbHighlightColor
				drawHLine(g, 1, thumbBounds.width - 3, 1)
				drawVLine(g, 1, 1, thumbBounds.height - 2)

				bumps.bumpArearea(thumbBounds.width - 6, thumbBounds.height - 7)
				bumps.paintIcon(c, g, 3, 4)

				If Not isFreeStanding Then
					thumbBounds.width -= 2
					If Not leftToRight Then g.translate(1, 0)
				End If
			Else ' HORIZONTAL
				If Not isFreeStanding Then thumbBounds.height += 2

				g.color = thumbColor
				g.fillRect(0, 0, thumbBounds.width - 1, thumbBounds.height - 2)

				g.color = thumbShadow
				drawRect(g, 0, 0, thumbBounds.width - 1, thumbBounds.height - 2)

				g.color = thumbHighlightColor
				drawHLine(g, 1, thumbBounds.width - 3, 1)
				drawVLine(g, 1, 1, thumbBounds.height - 3)

				bumps.bumpArearea(thumbBounds.width - 7, thumbBounds.height - 6)
				bumps.paintIcon(c, g, 4, 3)

				If Not isFreeStanding Then thumbBounds.height -= 2
			End If

			g.translate(-thumbBounds.x, -thumbBounds.y)
		End Sub

		Private Sub oceanPaintThumb(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent, ByVal thumbBounds As java.awt.Rectangle)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(c)

			g.translate(thumbBounds.x, thumbBounds.y)

			If scrollbar.orientation = javax.swing.JScrollBar.VERTICAL Then
				If Not isFreeStanding Then
					thumbBounds.width += 2
					If Not leftToRight Then g.translate(-1, 0)
				End If

				If thumbColor IsNot Nothing Then
					g.color = thumbColor
					g.fillRect(0, 0, thumbBounds.width - 2,thumbBounds.height - 1)
				End If

				g.color = thumbShadow
				drawRect(g, 0, 0, thumbBounds.width - 2, thumbBounds.height - 1)

				g.color = thumbHighlightColor
				drawHLine(g, 1, thumbBounds.width - 3, 1)
				drawVLine(g, 1, 1, thumbBounds.height - 2)

				MetalUtils.drawGradient(c, g, "ScrollBar.gradient", 2, 2, thumbBounds.width - 4, thumbBounds.height - 3, False)

				Dim gripSize As Integer = thumbBounds.width - 8
				If gripSize > 2 AndAlso thumbBounds.height >= 10 Then
					g.color = MetalLookAndFeel.primaryControlDarkShadow
					Dim gripY As Integer = thumbBounds.height / 2 - 2
					For counter As Integer = 0 To 5 Step 2
						g.fillRect(4, counter + gripY, gripSize, 1)
					Next counter

					g.color = MetalLookAndFeel.white
					gripY += 1
					For counter As Integer = 0 To 5 Step 2
						g.fillRect(5, counter + gripY, gripSize, 1)
					Next counter
				End If
				If Not isFreeStanding Then
					thumbBounds.width -= 2
					If Not leftToRight Then g.translate(1, 0)
				End If
			Else ' HORIZONTAL
				If Not isFreeStanding Then thumbBounds.height += 2

				If thumbColor IsNot Nothing Then
					g.color = thumbColor
					g.fillRect(0, 0, thumbBounds.width - 1,thumbBounds.height - 2)
				End If

				g.color = thumbShadow
				drawRect(g, 0, 0, thumbBounds.width - 1, thumbBounds.height - 2)

				g.color = thumbHighlightColor
				drawHLine(g, 1, thumbBounds.width - 2, 1)
				drawVLine(g, 1, 1, thumbBounds.height - 3)

				MetalUtils.drawGradient(c, g, "ScrollBar.gradient", 2, 2, thumbBounds.width - 3, thumbBounds.height - 4, True)

				Dim gripSize As Integer = thumbBounds.height - 8
				If gripSize > 2 AndAlso thumbBounds.width >= 10 Then
					g.color = MetalLookAndFeel.primaryControlDarkShadow
					Dim gripX As Integer = thumbBounds.width / 2 - 2
					For counter As Integer = 0 To 5 Step 2
						g.fillRect(gripX + counter, 4, 1, gripSize)
					Next counter

					g.color = MetalLookAndFeel.white
					gripX += 1
					For counter As Integer = 0 To 5 Step 2
						g.fillRect(gripX + counter, 5, 1, gripSize)
					Next counter
				End If

				If Not isFreeStanding Then thumbBounds.height -= 2
			End If

			g.translate(-thumbBounds.x, -thumbBounds.y)
		End Sub

		Protected Friend Property Overrides minimumThumbSize As java.awt.Dimension
			Get
				Return New java.awt.Dimension(scrollBarWidth, scrollBarWidth)
			End Get
		End Property

		''' <summary>
		''' This is overridden only to increase the invalid area.  This
		''' ensures that the "Shadow" below the thumb is invalidated
		''' </summary>
		Protected Friend Overrides Sub setThumbBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
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
			scrollbar.repaint(minX, minY, (maxX - minX)+1, (maxY - minY)+1)
		End Sub



		Friend Class ScrollBarListener
			Inherits javax.swing.plaf.basic.BasicScrollBarUI.PropertyChangeHandler

			Private ReadOnly outerInstance As MetalScrollBarUI

			Public Sub New(ByVal outerInstance As MetalScrollBarUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim name As String = e.propertyName
				If name.Equals(FREE_STANDING_PROP) Then
					handlePropertyChange(e.newValue)
				Else
					MyBase.propertyChange(e)
				End If
			End Sub

			Public Overridable Sub handlePropertyChange(ByVal newValue As Object)
				If newValue IsNot Nothing Then
					Dim temp As Boolean = CBool(newValue)
					Dim becameFlush As Boolean = temp = False AndAlso outerInstance.isFreeStanding = True
					Dim becameNormal As Boolean = temp = True AndAlso outerInstance.isFreeStanding = False

					outerInstance.isFreeStanding = temp

					If becameFlush Then
						toFlush()
					ElseIf becameNormal Then
						toFreeStanding()
					End If
				Else

					If Not outerInstance.isFreeStanding Then
						outerInstance.isFreeStanding = True
						toFreeStanding()
					End If

					' This commented-out block is used for testing flush scrollbars.
	'
	'                if ( isFreeStanding ) {
	'                    isFreeStanding = false;
	'                    toFlush();
	'                }
	'
				End If

				If outerInstance.increaseButton IsNot Nothing Then outerInstance.increaseButton.freeStanding = outerInstance.isFreeStanding
				If outerInstance.decreaseButton IsNot Nothing Then outerInstance.decreaseButton.freeStanding = outerInstance.isFreeStanding
			End Sub

			Protected Friend Overridable Sub toFlush()
				outerInstance.scrollBarWidth -= 2
			End Sub

			Protected Friend Overridable Sub toFreeStanding()
				outerInstance.scrollBarWidth += 2
			End Sub
		End Class ' end class ScrollBarListener
	End Class

End Namespace