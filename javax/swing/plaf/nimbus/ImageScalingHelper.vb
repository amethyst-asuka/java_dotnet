Imports System

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' ImageScalingHelper
	''' 
	''' @author Created by Jasper Potts (Aug 8, 2007)
	''' </summary>
	Friend Class ImageScalingHelper

		''' <summary>
		''' Enumeration for the types of painting this class can handle. </summary>
		Friend Enum PaintType
			''' <summary>
			''' Painting type indicating the image should be centered in the space provided.  When used the <code>mask</code>
			''' is ignored.
			''' </summary>
			CENTER

			''' <summary>
			''' Painting type indicating the image should be tiled across the specified width and height.  When used the
			''' <code>mask</code> is ignored.
			''' </summary>
			TILE

			''' <summary>
			''' Painting type indicating the image should be split into nine regions with the top, left, bottom and right
			''' areas stretched.
			''' </summary>
			PAINT9_STRETCH

			''' <summary>
			''' Painting type indicating the image should be split into nine regions with the top, left, bottom and right
			''' areas tiled.
			''' </summary>
			PAINT9_TILE
		End Enum



		Private Shared ReadOnly EMPTY_INSETS As New java.awt.Insets(0, 0, 0, 0)

		Friend Const PAINT_TOP_LEFT As Integer = 1
		Friend Const PAINT_TOP As Integer = 2
		Friend Const PAINT_TOP_RIGHT As Integer = 4
		Friend Const PAINT_LEFT As Integer = 8
		Friend Const PAINT_CENTER As Integer = 16
		Friend Const PAINT_RIGHT As Integer = 32
		Friend Const PAINT_BOTTOM_RIGHT As Integer = 64
		Friend Const PAINT_BOTTOM As Integer = 128
		Friend Const PAINT_BOTTOM_LEFT As Integer = 256
		''' <summary>
		''' Specifies that all regions should be painted.  If this is set any other regions specified will not be painted.
		''' For example PAINT_ALL | PAINT_CENTER will paint all but the center.
		''' </summary>
		Friend Const PAINT_ALL As Integer = 512

		''' <summary>
		''' Paints using the algorightm specified by <code>paintType</code>.
		''' </summary>
		''' <param name="g">         Graphics to render to </param>
		''' <param name="x">         X-coordinate </param>
		''' <param name="y">         Y-coordinate </param>
		''' <param name="w">         Width to render to </param>
		''' <param name="h">         Height to render to </param>
		''' <param name="image">     Image to render from, if <code>null</code> this method will do nothing </param>
		''' <param name="sInsets">   Insets specifying the portion of the image that will be stretched or tiled, if <code>null</code>
		'''                  empty <code>Insets</code> will be used. </param>
		''' <param name="dInsets">   Destination insets specifying the portion of the image will be stretched or tiled, if
		'''                  <code>null</code> empty <code>Insets</code> will be used. </param>
		''' <param name="paintType"> Specifies what type of algorithm to use in painting </param>
		''' <param name="mask">      Specifies portion of image to render, if <code>PAINT_ALL</code> is specified, any other regions
		'''                  specified will not be painted, for example PAINT_ALL | PAINT_CENTER paints everything but the
		'''                  center. </param>
		Public Shared Sub paint(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal image As java.awt.Image, ByVal sInsets As java.awt.Insets, ByVal dInsets As java.awt.Insets, ByVal paintType As PaintType, ByVal mask As Integer)
			If image Is Nothing OrElse image.getWidth(Nothing) <= 0 OrElse image.getHeight(Nothing) <= 0 Then Return
			If sInsets Is Nothing Then sInsets = EMPTY_INSETS
			If dInsets Is Nothing Then dInsets = EMPTY_INSETS
			Dim iw As Integer = image.getWidth(Nothing)
			Dim ih As Integer = image.getHeight(Nothing)

			If paintType = PaintType.CENTER Then
				' Center the image
				g.drawImage(image, x + (w - iw) \ 2, y + (h - ih) \ 2, Nothing)
			ElseIf paintType = PaintType.TILE Then
				' Tile the image
				Dim lastIY As Integer = 0
				Dim yCounter As Integer = y
				Dim maxY As Integer = y + h
				Do While yCounter < maxY
					Dim lastIX As Integer = 0
					Dim xCounter As Integer = x
					Dim maxX As Integer = x + w
					Do While xCounter < maxX
						Dim dx2 As Integer = Math.Min(maxX, xCounter + iw - lastIX)
						Dim dy2 As Integer = Math.Min(maxY, yCounter + ih - lastIY)
						g.drawImage(image, xCounter, yCounter, dx2, dy2, lastIX, lastIY, lastIX + dx2 - xCounter, lastIY + dy2 - yCounter, Nothing)
						xCounter += (iw - lastIX)
						lastIX = 0
					Loop
					yCounter += (ih - lastIY)
					lastIY = 0
				Loop
			Else
				Dim st As Integer = sInsets.top
				Dim sl As Integer = sInsets.left
				Dim sb As Integer = sInsets.bottom
				Dim sr As Integer = sInsets.right

				Dim dt As Integer = dInsets.top
				Dim dl As Integer = dInsets.left
				Dim db As Integer = dInsets.bottom
				Dim dr As Integer = dInsets.right

				' Constrain the insets to the size of the image
				If st + sb > ih Then
						st = Math.Max(0, ih \ 2)
							sb = st
								dt = sb
								db = dt
				End If
				If sl + sr > iw Then
						sr = Math.Max(0, iw \ 2)
							sl = sr
								dr = sl
								dl = dr
				End If

				' Constrain the insets to the size of the region we're painting
				' in.
				If dt + db > h Then
						db = Math.Max(0, h \ 2 - 1)
						dt = db
				End If
				If dl + dr > w Then
						dr = Math.Max(0, w \ 2 - 1)
						dl = dr
				End If

				Dim stretch As Boolean = (paintType = PaintType.PAINT9_STRETCH)
				If (mask And PAINT_ALL) <> 0 Then mask = (PAINT_ALL - 1) And Not mask

				If (mask And PAINT_LEFT) <> 0 Then drawChunk(image, g, stretch, x, y + dt, x + dl, y + h - db, 0, st, sl, ih - sb, False)
				If (mask And PAINT_TOP_LEFT) <> 0 Then drawImage(image, g, x, y, x + dl, y + dt, 0, 0, sl, st)
				If (mask And PAINT_TOP) <> 0 Then drawChunk(image, g, stretch, x + dl, y, x + w - dr, y + dt, sl, 0, iw - sr, st, True)
				If (mask And PAINT_TOP_RIGHT) <> 0 Then drawImage(image, g, x + w - dr, y, x + w, y + dt, iw - sr, 0, iw, st)
				If (mask And PAINT_RIGHT) <> 0 Then drawChunk(image, g, stretch, x + w - dr, y + dt, x + w, y + h - db, iw - sr, st, iw, ih - sb, False)
				If (mask And PAINT_BOTTOM_RIGHT) <> 0 Then drawImage(image, g, x + w - dr, y + h - db, x + w, y + h, iw - sr, ih - sb, iw, ih)
				If (mask And PAINT_BOTTOM) <> 0 Then drawChunk(image, g, stretch, x + dl, y + h - db, x + w - dr, y + h, sl, ih - sb, iw - sr, ih, True)
				If (mask And PAINT_BOTTOM_LEFT) <> 0 Then drawImage(image, g, x, y + h - db, x + dl, y + h, 0, ih - sb, sl, ih)
				If (mask And PAINT_CENTER) <> 0 Then drawImage(image, g, x + dl, y + dt, x + w - dr, y + h - db, sl, st, iw - sr, ih - sb)
			End If
		End Sub

		''' <summary>
		''' Draws a portion of an image, stretched or tiled.
		''' </summary>
		''' <param name="image"> Image to render. </param>
		''' <param name="g"> Graphics to render to </param>
		''' <param name="stretch"> Whether the image should be stretched or timed in the
		'''                provided space. </param>
		''' <param name="dx1"> X origin to draw to </param>
		''' <param name="dy1"> Y origin to draw to </param>
		''' <param name="dx2"> End x location to draw to </param>
		''' <param name="dy2"> End y location to draw to </param>
		''' <param name="sx1"> X origin to draw from </param>
		''' <param name="sy1"> Y origin to draw from </param>
		''' <param name="sx2"> Max x location to draw from </param>
		''' <param name="sy2"> Max y location to draw from </param>
		''' <param name="xDirection"> Used if the image is not stretched. If true it
		'''        indicates the image should be tiled along the x axis. </param>
		Private Shared Sub drawChunk(ByVal image As java.awt.Image, ByVal g As java.awt.Graphics, ByVal stretch As Boolean, ByVal dx1 As Integer, ByVal dy1 As Integer, ByVal dx2 As Integer, ByVal dy2 As Integer, ByVal sx1 As Integer, ByVal sy1 As Integer, ByVal sx2 As Integer, ByVal sy2 As Integer, ByVal xDirection As Boolean)
			If dx2 - dx1 <= 0 OrElse dy2 - dy1 <= 0 OrElse sx2 - sx1 <= 0 OrElse sy2 - sy1 <= 0 Then Return
			If stretch Then
				g.drawImage(image, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, Nothing)
			Else
				Dim xSize As Integer = sx2 - sx1
				Dim ySize As Integer = sy2 - sy1
				Dim deltaX As Integer
				Dim deltaY As Integer

				If xDirection Then
					deltaX = xSize
					deltaY = 0
				Else
					deltaX = 0
					deltaY = ySize
				End If
				Do While dx1 < dx2 AndAlso dy1 < dy2
					Dim newDX2 As Integer = Math.Min(dx2, dx1 + xSize)
					Dim newDY2 As Integer = Math.Min(dy2, dy1 + ySize)

					g.drawImage(image, dx1, dy1, newDX2, newDY2, sx1, sy1, sx1 + newDX2 - dx1, sy1 + newDY2 - dy1, Nothing)
					dx1 += deltaX
					dy1 += deltaY
				Loop
			End If
		End Sub

		Private Shared Sub drawImage(ByVal image As java.awt.Image, ByVal g As java.awt.Graphics, ByVal dx1 As Integer, ByVal dy1 As Integer, ByVal dx2 As Integer, ByVal dy2 As Integer, ByVal sx1 As Integer, ByVal sy1 As Integer, ByVal sx2 As Integer, ByVal sy2 As Integer)
			' PENDING: is this necessary, will G2D do it for me?
			If dx2 - dx1 <= 0 OrElse dy2 - dy1 <= 0 OrElse sx2 - sx1 <= 0 OrElse sy2 - sy1 <= 0 Then Return
			g.drawImage(image, dx1, dy1, dx2, dy2, sx1, sy1, sx2, sy2, Nothing)
		End Sub


	End Class

End Namespace