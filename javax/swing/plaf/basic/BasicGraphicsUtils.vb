Imports Microsoft.VisualBasic
Imports javax.swing

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




	'
	' * @author Hans Muller
	' 

	Public Class BasicGraphicsUtils

		Private Shared ReadOnly GROOVE_INSETS As New java.awt.Insets(2, 2, 2, 2)
		Private Shared ReadOnly ETCHED_INSETS As New java.awt.Insets(2, 2, 2, 2)

		Public Shared Sub drawEtchedRect(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
			Dim oldColor As java.awt.Color = g.color ' Make no net change to g
			g.translate(x, y)

			g.color = shadow
			g.drawLine(0, 0, w-1, 0) ' outer border, top
			g.drawLine(0, 1, 0, h-2) ' outer border, left

			g.color = darkShadow
			g.drawLine(1, 1, w-3, 1) ' inner border, top
			g.drawLine(1, 2, 1, h-3) ' inner border, left

			g.color = lightHighlight
			g.drawLine(w-1, 0, w-1, h-1) ' outer border, bottom
			g.drawLine(0, h-1, w-1, h-1) ' outer border, right

			g.color = highlight
			g.drawLine(w-2, 1, w-2, h-3) ' inner border, right
			g.drawLine(1, h-2, w-2, h-2) ' inner border, bottom

			g.translate(-x, -y)
			g.color = oldColor
		End Sub


		''' <summary>
		''' Returns the amount of space taken up by a border drawn by
		''' <code>drawEtchedRect()</code>
		''' </summary>
		''' <returns>  the inset of an etched rect </returns>
		Public Property Shared etchedInsets As java.awt.Insets
			Get
				Return ETCHED_INSETS
			End Get
		End Property


		Public Shared Sub drawGroove(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal shadow As java.awt.Color, ByVal highlight As java.awt.Color)
			Dim oldColor As java.awt.Color = g.color ' Make no net change to g
			g.translate(x, y)

			g.color = shadow
			g.drawRect(0, 0, w-2, h-2)

			g.color = highlight
			g.drawLine(1, h-3, 1, 1)
			g.drawLine(1, 1, w-3, 1)

			g.drawLine(0, h-1, w-1, h-1)
			g.drawLine(w-1, h-1, w-1, 0)

			g.translate(-x, -y)
			g.color = oldColor
		End Sub

		''' <summary>
		''' Returns the amount of space taken up by a border drawn by
		''' <code>drawGroove()</code>
		''' </summary>
		''' <returns>  the inset of a groove border </returns>
		Public Property Shared grooveInsets As java.awt.Insets
			Get
				Return GROOVE_INSETS
			End Get
		End Property


		Public Shared Sub drawBezel(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isPressed As Boolean, ByVal isDefault As Boolean, ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
			Dim oldColor As java.awt.Color = g.color ' Make no net change to g
			g.translate(x, y)

			If isPressed AndAlso isDefault Then
				g.color = darkShadow
				g.drawRect(0, 0, w - 1, h - 1)
				g.color = shadow
				g.drawRect(1, 1, w - 3, h - 3)
			ElseIf isPressed Then
				drawLoweredBezel(g, x, y, w, h, shadow, darkShadow, highlight, lightHighlight)
			ElseIf isDefault Then
				g.color = darkShadow
				g.drawRect(0, 0, w-1, h-1)

				g.color = lightHighlight
				g.drawLine(1, 1, 1, h-3)
				g.drawLine(2, 1, w-3, 1)

				g.color = highlight
				g.drawLine(2, 2, 2, h-4)
				g.drawLine(3, 2, w-4, 2)

				g.color = shadow
				g.drawLine(2, h-3, w-3, h-3)
				g.drawLine(w-3, 2, w-3, h-4)

				g.color = darkShadow
				g.drawLine(1, h-2, w-2, h-2)
				g.drawLine(w-2, h-2, w-2, 1)
			Else
				g.color = lightHighlight
				g.drawLine(0, 0, 0, h-1)
				g.drawLine(1, 0, w-2, 0)

				g.color = highlight
				g.drawLine(1, 1, 1, h-3)
				g.drawLine(2, 1, w-3, 1)

				g.color = shadow
				g.drawLine(1, h-2, w-2, h-2)
				g.drawLine(w-2, 1, w-2, h-3)

				g.color = darkShadow
				g.drawLine(0, h-1, w-1, h-1)
				g.drawLine(w-1, h-1, w-1, 0)
			End If
			g.translate(-x, -y)
			g.color = oldColor
		End Sub

		Public Shared Sub drawLoweredBezel(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color, ByVal lightHighlight As java.awt.Color)
			g.color = darkShadow
			g.drawLine(0, 0, 0, h-1)
			g.drawLine(1, 0, w-2, 0)

			g.color = shadow
			g.drawLine(1, 1, 1, h-2)
			g.drawLine(1, 1, w-3, 1)

			g.color = lightHighlight
			g.drawLine(0, h-1, w-1, h-1)
			g.drawLine(w-1, h-1, w-1, 0)

			g.color = highlight
			g.drawLine(1, h-2, w-2, h-2)
			g.drawLine(w-2, h-2, w-2, 1)
		End Sub


		''' <summary>
		''' Draw a string with the graphics <code>g</code> at location (x,y)
		'''  just like <code>g.drawString</code> would.
		'''  The first occurrence of <code>underlineChar</code>
		'''  in text will be underlined. The matching algorithm is
		'''  not case sensitive.
		''' </summary>
		Public Shared Sub drawString(ByVal g As java.awt.Graphics, ByVal text As String, ByVal underlinedChar As Integer, ByVal x As Integer, ByVal y As Integer)
			Dim index As Integer=-1

			If underlinedChar <> ControlChars.NullChar Then
				Dim uc As Char = Char.ToUpper(ChrW(underlinedChar))
				Dim lc As Char = Char.ToLower(ChrW(underlinedChar))
				Dim uci As Integer = text.IndexOf(uc)
				Dim lci As Integer = text.IndexOf(lc)

				If uci = -1 Then
					index = lci
				ElseIf lci = -1 Then
					index = uci
				Else
					index = If(lci < uci, lci, uci)
				End If
			End If
			drawStringUnderlineCharAt(g, text, index, x, y)
		End Sub

		''' <summary>
		''' Draw a string with the graphics <code>g</code> at location
		''' (<code>x</code>, <code>y</code>)
		''' just like <code>g.drawString</code> would.
		''' The character at index <code>underlinedIndex</code>
		''' in text will be underlined. If <code>index</code> is beyond the
		''' bounds of <code>text</code> (including &lt; 0), nothing will be
		''' underlined.
		''' </summary>
		''' <param name="g"> Graphics to draw with </param>
		''' <param name="text"> String to draw </param>
		''' <param name="underlinedIndex"> Index of character in text to underline </param>
		''' <param name="x"> x coordinate to draw at </param>
		''' <param name="y"> y coordinate to draw at
		''' @since 1.4 </param>
		Public Shared Sub drawStringUnderlineCharAt(ByVal g As java.awt.Graphics, ByVal text As String, ByVal underlinedIndex As Integer, ByVal x As Integer, ByVal y As Integer)
			sun.swing.SwingUtilities2.drawStringUnderlineCharAt(Nothing, g, text, underlinedIndex, x, y)
		End Sub

		Public Shared Sub drawDashedRect(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim vx, vy As Integer

			' draw upper and lower horizontal dashes
			For vx = x To (x + width) - 1 Step 2
				g.fillRect(vx, y, 1, 1)
				g.fillRect(vx, y + height-1, 1, 1)
			Next vx

			' draw left and right vertical dashes
			For vy = y To (y + height) - 1 Step 2
				g.fillRect(x, vy, 1, 1)
				g.fillRect(x+width-1, vy, 1, 1)
			Next vy
		End Sub

		Public Shared Function getPreferredButtonSize(ByVal b As AbstractButton, ByVal textIconGap As Integer) As java.awt.Dimension
			If b.componentCount > 0 Then Return Nothing

			Dim icon As Icon = b.icon
			Dim text As String = b.text

			Dim font As java.awt.Font = b.font
			Dim fm As java.awt.FontMetrics = b.getFontMetrics(font)

			Dim iconR As New java.awt.Rectangle
			Dim textR As New java.awt.Rectangle
			Dim viewR As New java.awt.Rectangle(Short.MaxValue, Short.MaxValue)

			SwingUtilities.layoutCompoundLabel(b, fm, text, icon, b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, viewR, iconR, textR, (If(text Is Nothing, 0, textIconGap)))

	'         The preferred size of the button is the size of
	'         * the text and icon rectangles plus the buttons insets.
	'         

			Dim r As java.awt.Rectangle = iconR.union(textR)

			Dim insets As java.awt.Insets = b.insets
			r.width += insets.left + insets.right
			r.height += insets.top + insets.bottom

			Return r.size
		End Function

	'    
	'     * Convenience function for determining ComponentOrientation.  Helps us
	'     * avoid having Munge directives throughout the code.
	'     
		Friend Shared Function isLeftToRight(ByVal c As java.awt.Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function

		Friend Shared Function isMenuShortcutKeyDown(ByVal [event] As java.awt.event.InputEvent) As Boolean
			Return ([event].modifiers And java.awt.Toolkit.defaultToolkit.menuShortcutKeyMask) <> 0
		End Function
	End Class

End Namespace