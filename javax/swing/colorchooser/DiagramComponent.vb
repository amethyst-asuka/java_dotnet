Imports Microsoft.VisualBasic

'
' * Copyright (c) 2008, Oracle and/or its affiliates. All rights reserved.
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


	Friend NotInheritable Class DiagramComponent
		Inherits javax.swing.JComponent
		Implements java.awt.event.MouseListener, java.awt.event.MouseMotionListener

		Private ReadOnly panel As ColorPanel
		Private ReadOnly diagram As Boolean

		Private ReadOnly insets As New java.awt.Insets(0, 0, 0, 0)

		Private width As Integer
		Private height As Integer

		Private array As Integer()
		Private image As java.awt.image.BufferedImage

		Friend Sub New(ByVal panel As ColorPanel, ByVal diagram As Boolean)
			Me.panel = panel
			Me.diagram = diagram
			addMouseListener(Me)
			addMouseMotionListener(Me)
		End Sub

		Protected Friend Overrides Sub paintComponent(ByVal g As java.awt.Graphics)
			getInsets(Me.insets)
			Me.width = width - Me.insets.left - Me.insets.right
			Me.height = height - Me.insets.top - Me.insets.bottom

			Dim update As Boolean = (Me.image Is Nothing) OrElse (Me.width <> Me.image.width) OrElse (Me.height <> Me.image.height)
			If update Then
				Dim ___size As Integer = Me.width * Me.height
				If (Me.array Is Nothing) OrElse (Me.array.Length < ___size) Then Me.array = New Integer(___size - 1){}
				Me.image = New java.awt.image.BufferedImage(Me.width, Me.height, java.awt.image.BufferedImage.TYPE_INT_RGB)
			End If
				Dim dx As Single = 1.0f / CSng(Me.width - 1)
				Dim dy As Single = 1.0f / CSng(Me.height - 1)

				Dim offset As Integer = 0
				Dim ___y As Single = 0.0f
				Dim h As Integer = 0
				Do While h < Me.height
					If Me.diagram Then
						Dim ___x As Single = 0.0f
						Dim w As Integer = 0
						Do While w < Me.width
							Me.array(offset) = Me.panel.getColor(___x, ___y)
							w += 1
							___x += dx
							offset += 1
						Loop
					Else
						Dim color As Integer = Me.panel.getColor(___y)
						Dim w As Integer = 0
						Do While w < Me.width
							Me.array(offset) = color
							w += 1
							offset += 1
						Loop
					End If
					h += 1
					___y += dy
				Loop
			Me.image.rGBRGB(0, 0, Me.width, Me.height, Me.array, 0, Me.width)
			g.drawImage(Me.image, Me.insets.left, Me.insets.top, Me.width, Me.height, Me)
			If enabled Then
				Me.width -= 1
				Me.height -= 1
				g.xORMode = java.awt.Color.WHITE
				g.color = java.awt.Color.BLACK
				If Me.diagram Then
					Dim ___x As Integer = getValue(Me.panel.valueX, Me.insets.left, Me.width)
					Dim ___y As Integer = getValue(Me.panel.valueY, Me.insets.top, Me.height)
					g.drawLine(___x - 8, ___y, ___x + 8, ___y)
					g.drawLine(___x, ___y - 8, ___x, ___y + 8)
				Else
					Dim z As Integer = getValue(Me.panel.valueZ, Me.insets.top, Me.height)
					g.drawLine(Me.insets.left, z, Me.insets.left + Me.width, z)
				End If
				g.paintModeode()
			End If
		End Sub

		Public Sub mousePressed(ByVal [event] As java.awt.event.MouseEvent)
			mouseDragged([event])
		End Sub

		Public Sub mouseReleased(ByVal [event] As java.awt.event.MouseEvent)
		End Sub

		Public Sub mouseClicked(ByVal [event] As java.awt.event.MouseEvent)
		End Sub

		Public Sub mouseEntered(ByVal [event] As java.awt.event.MouseEvent)
		End Sub

		Public Sub mouseExited(ByVal [event] As java.awt.event.MouseEvent)
		End Sub

		Public Sub mouseMoved(ByVal [event] As java.awt.event.MouseEvent)
		End Sub

		Public Sub mouseDragged(ByVal [event] As java.awt.event.MouseEvent)
			If enabled Then
				Dim ___y As Single = getValue([event].y, Me.insets.top, Me.height)
				If Me.diagram Then
					Dim ___x As Single = getValue([event].x, Me.insets.left, Me.width)
					Me.panel.valuelue(___x, ___y)
				Else
					Me.panel.value = ___y
				End If
			End If
		End Sub

		Private Shared Function getValue(ByVal value As Single, ByVal min As Integer, ByVal max As Integer) As Integer
			Return min + CInt(Fix(value * CSng(max)))
		End Function

		Private Shared Function getValue(ByVal value As Integer, ByVal min As Integer, ByVal max As Integer) As Single
			If min < value Then
				value -= min
				Return If(value < max, CSng(value) / CSng(max), 1.0f)
			End If
			Return 0.0f
		End Function
	End Class

End Namespace