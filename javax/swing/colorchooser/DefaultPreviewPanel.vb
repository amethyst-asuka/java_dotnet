Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.text

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

Namespace javax.swing.colorchooser



	''' <summary>
	''' The standard preview panel for the color chooser.
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
	''' @author Steve Wilson </summary>
	''' <seealso cref= JColorChooser </seealso>
	Friend Class DefaultPreviewPanel
		Inherits JPanel

		Private squareSize As Integer = 25
		Private squareGap As Integer = 5
		Private innerGap As Integer = 5


		Private textGap As Integer = 5
		Private font As New Font(Font.DIALOG, Font.PLAIN, 12)
		Private sampleText As String

		Private swatchWidth As Integer = 50

		Private oldColor As Color = Nothing

		Private Property colorChooser As JColorChooser
			Get
				Return CType(SwingUtilities.getAncestorOfClass(GetType(JColorChooser), Me), JColorChooser)
			End Get
		End Property

		Public Property Overrides preferredSize As Dimension
			Get
				Dim host As JComponent = colorChooser
				If host Is Nothing Then host = Me
				Dim fm As FontMetrics = host.getFontMetrics(font)
    
				Dim ascent As Integer = fm.ascent
				Dim ___height As Integer = fm.height
				Dim ___width As Integer = sun.swing.SwingUtilities2.stringWidth(host, fm, sampleText)
    
				Dim ___y As Integer = ___height*3 + textGap*3
				Dim ___x As Integer = squareSize * 3 + squareGap*2 + swatchWidth + ___width + textGap*3
				Return New Dimension(___x,___y)
			End Get
		End Property

		Public Overrides Sub paintComponent(ByVal g As Graphics)
			If oldColor Is Nothing Then oldColor = foreground

			g.color = background
			g.fillRect(0,0,width,height)

			If Me.componentOrientation.leftToRight Then
				Dim squareWidth As Integer = paintSquares(g, 0)
				Dim textWidth As Integer = paintText(g, squareWidth)
				paintSwatch(g, squareWidth + textWidth)
			Else
				Dim swatchWidth As Integer = paintSwatch(g, 0)
				Dim textWidth As Integer = paintText(g, swatchWidth)
				paintSquares(g, swatchWidth + textWidth)

			End If
		End Sub

		Private Function paintSwatch(ByVal g As Graphics, ByVal offsetX As Integer) As Integer
			Dim swatchX As Integer = offsetX
			g.color = oldColor
			g.fillRect(swatchX, 0, swatchWidth, (squareSize) + (squareGap\2))
			g.color = foreground
			g.fillRect(swatchX, (squareSize) + (squareGap\2), swatchWidth, (squareSize) + (squareGap\2))
			Return (swatchX+swatchWidth)
		End Function

		Private Function paintText(ByVal g As Graphics, ByVal offsetX As Integer) As Integer
			g.font = font
			Dim host As JComponent = colorChooser
			If host Is Nothing Then host = Me
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(host, g)

			Dim ascent As Integer = fm.ascent
			Dim ___height As Integer = fm.height
			Dim ___width As Integer = sun.swing.SwingUtilities2.stringWidth(host, fm, sampleText)

			Dim textXOffset As Integer = offsetX + textGap

			Dim color As Color = foreground

			g.color = color

			sun.swing.SwingUtilities2.drawString(host, g, sampleText,textXOffset+(textGap\2), ascent+2)

			g.fillRect(textXOffset, (___height) + textGap, ___width + (textGap), ___height +2)

			g.color = Color.black
			sun.swing.SwingUtilities2.drawString(host, g, sampleText, textXOffset+(textGap\2), ___height+ascent+textGap+2)


			g.color = Color.white

			g.fillRect(textXOffset, (___height + textGap) * 2, ___width + (textGap), ___height +2)

			g.color = color
			sun.swing.SwingUtilities2.drawString(host, g, sampleText, textXOffset+(textGap\2), ((___height+textGap) * 2)+ascent+2)

			Return ___width + textGap*3

		End Function

		Private Function paintSquares(ByVal g As Graphics, ByVal offsetX As Integer) As Integer

			Dim squareXOffset As Integer = offsetX
			Dim color As Color = foreground

			g.color = Color.white
			g.fillRect(squareXOffset,0,squareSize,squareSize)
			g.color = color
			g.fillRect(squareXOffset+innerGap, innerGap, squareSize - (innerGap*2), squareSize - (innerGap*2))
			g.color = Color.white
			g.fillRect(squareXOffset+innerGap*2, innerGap*2, squareSize - (innerGap*4), squareSize - (innerGap*4))

			g.color = color
			g.fillRect(squareXOffset,squareSize+squareGap,squareSize,squareSize)

			g.translate(squareSize+squareGap, 0)
			g.color = Color.black
			g.fillRect(squareXOffset,0,squareSize,squareSize)
			g.color = color
			g.fillRect(squareXOffset+innerGap, innerGap, squareSize - (innerGap*2), squareSize - (innerGap*2))
			g.color = Color.white
			g.fillRect(squareXOffset+innerGap*2, innerGap*2, squareSize - (innerGap*4), squareSize - (innerGap*4))
			g.translate(-(squareSize+squareGap), 0)

			g.translate(squareSize+squareGap, squareSize+squareGap)
			g.color = Color.white
			g.fillRect(squareXOffset,0,squareSize,squareSize)
			g.color = color
			g.fillRect(squareXOffset+innerGap, innerGap, squareSize - (innerGap*2), squareSize - (innerGap*2))
			g.translate(-(squareSize+squareGap), -(squareSize+squareGap))



			g.translate((squareSize+squareGap)*2, 0)
			g.color = Color.white
			g.fillRect(squareXOffset,0,squareSize,squareSize)
			g.color = color
			g.fillRect(squareXOffset+innerGap, innerGap, squareSize - (innerGap*2), squareSize - (innerGap*2))
			g.color = Color.black
			g.fillRect(squareXOffset+innerGap*2, innerGap*2, squareSize - (innerGap*4), squareSize - (innerGap*4))
			g.translate(-((squareSize+squareGap)*2), 0)

			g.translate((squareSize+squareGap)*2, (squareSize+squareGap))
			g.color = Color.black
			g.fillRect(squareXOffset,0,squareSize,squareSize)
			g.color = color
			g.fillRect(squareXOffset+innerGap, innerGap, squareSize - (innerGap*2), squareSize - (innerGap*2))
			g.translate(-((squareSize+squareGap)*2), -(squareSize+squareGap))

			Return (squareSize*3+squareGap*2)

		End Function

		Private Property sampleText As String
			Get
				If Me.sampleText Is Nothing Then Me.sampleText = UIManager.getString("ColorChooser.sampleText", locale)
				Return Me.sampleText
			End Get
		End Property
	End Class

End Namespace