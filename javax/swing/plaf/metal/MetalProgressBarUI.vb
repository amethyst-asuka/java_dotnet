Imports javax.swing
Imports javax.swing.plaf
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
	''' The Metal implementation of ProgressBarUI.
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
	''' @author Michael C. Albers
	''' </summary>
	Public Class MetalProgressBarUI
		Inherits BasicProgressBarUI

		Private innards As Rectangle
		Private ___box As Rectangle

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalProgressBarUI
		End Function

		''' <summary>
		''' Draws a bit of special highlighting on the progress bar.
		''' The core painting is deferred to the BasicProgressBar's
		''' <code>paintDeterminate</code> method.
		''' @since 1.4
		''' </summary>
		Public Overrides Sub paintDeterminate(ByVal g As Graphics, ByVal c As JComponent)
			MyBase.paintDeterminate(g,c)

			If Not(TypeOf g Is Graphics2D) Then Return

			If progressBar.borderPainted Then
				Dim b As Insets = progressBar.insets ' area for border
				Dim barRectWidth As Integer = progressBar.width - (b.left + b.right)
				Dim barRectHeight As Integer = progressBar.height - (b.top + b.bottom)
				Dim ___amountFull As Integer = getAmountFull(b, barRectWidth, barRectHeight)
				Dim isLeftToRight As Boolean = MetalUtils.isLeftToRight(c)
				Dim startX, startY, endX, endY As Integer

				' The progress bar border is painted according to a light source.
				' This light source is stationary and does not change when the
				' component orientation changes.
				startX = b.left
				startY = b.top
				endX = b.left + barRectWidth - 1
				endY = b.top + barRectHeight - 1

				Dim g2 As Graphics2D = CType(g, Graphics2D)
				g2.stroke = New BasicStroke(1.0f)

				If progressBar.orientation = JProgressBar.HORIZONTAL Then
					' Draw light line lengthwise across the progress bar.
					g2.color = MetalLookAndFeel.controlShadow
					g2.drawLine(startX, startY, endX, startY)

					If ___amountFull > 0 Then
						' Draw darker lengthwise line over filled area.
						g2.color = MetalLookAndFeel.primaryControlDarkShadow

						If isLeftToRight Then
							g2.drawLine(startX, startY, startX + ___amountFull - 1, startY)
						Else
							g2.drawLine(endX, startY, endX - ___amountFull + 1, startY)
							If progressBar.percentComplete <> 1.0f Then g2.color = MetalLookAndFeel.controlShadow
						End If
					End If
					' Draw a line across the width.  The color is determined by
					' the code above.
					g2.drawLine(startX, startY, startX, endY)
 ' VERTICAL
				Else
					' Draw light line lengthwise across the progress bar.
					g2.color = MetalLookAndFeel.controlShadow
					g2.drawLine(startX, startY, startX, endY)

					If ___amountFull > 0 Then
						' Draw darker lengthwise line over filled area.
						g2.color = MetalLookAndFeel.primaryControlDarkShadow
						g2.drawLine(startX, endY, startX, endY - ___amountFull + 1)
					End If
					' Draw a line across the width.  The color is determined by
					' the code above.
					g2.color = MetalLookAndFeel.controlShadow

					If progressBar.percentComplete = 1.0f Then g2.color = MetalLookAndFeel.primaryControlDarkShadow
					g2.drawLine(startX, startY, endX, startY)
				End If
			End If
		End Sub

		''' <summary>
		''' Draws a bit of special highlighting on the progress bar
		''' and bouncing box.
		''' The core painting is deferred to the BasicProgressBar's
		''' <code>paintIndeterminate</code> method.
		''' @since 1.4
		''' </summary>
		Public Overrides Sub paintIndeterminate(ByVal g As Graphics, ByVal c As JComponent)
			MyBase.paintIndeterminate(g, c)

			If (Not progressBar.borderPainted) OrElse (Not(TypeOf g Is Graphics2D)) Then Return

			Dim b As Insets = progressBar.insets ' area for border
			Dim barRectWidth As Integer = progressBar.width - (b.left + b.right)
			Dim barRectHeight As Integer = progressBar.height - (b.top + b.bottom)
			Dim ___amountFull As Integer = getAmountFull(b, barRectWidth, barRectHeight)
			Dim isLeftToRight As Boolean = MetalUtils.isLeftToRight(c)
			Dim startX, startY, endX, endY As Integer
			Dim ___box As Rectangle = Nothing
			___box = getBox(___box)

			' The progress bar border is painted according to a light source.
			' This light source is stationary and does not change when the
			' component orientation changes.
			startX = b.left
			startY = b.top
			endX = b.left + barRectWidth - 1
			endY = b.top + barRectHeight - 1

			Dim g2 As Graphics2D = CType(g, Graphics2D)
			g2.stroke = New BasicStroke(1.0f)

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				' Draw light line lengthwise across the progress bar.
				g2.color = MetalLookAndFeel.controlShadow
				g2.drawLine(startX, startY, endX, startY)
				g2.drawLine(startX, startY, startX, endY)

				' Draw darker lengthwise line over filled area.
				g2.color = MetalLookAndFeel.primaryControlDarkShadow
				g2.drawLine(___box.x, startY, ___box.x + ___box.width - 1, startY)
 ' VERTICAL
			Else
				' Draw light line lengthwise across the progress bar.
				g2.color = MetalLookAndFeel.controlShadow
				g2.drawLine(startX, startY, startX, endY)
				g2.drawLine(startX, startY, endX, startY)

				' Draw darker lengthwise line over filled area.
				g2.color = MetalLookAndFeel.primaryControlDarkShadow
				g2.drawLine(startX, ___box.y, startX, ___box.y + ___box.height - 1)
			End If
		End Sub
	End Class

End Namespace