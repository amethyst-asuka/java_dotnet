Imports System
Imports System.Collections.Generic
Imports javax.swing

'
' * Copyright (c) 1998, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' Implements the bumps used throughout the Metal Look and Feel.
	''' 
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>


	Friend Class MetalBumps
		Implements Icon

		Friend Shared ReadOnly ALPHA As New Color(0, 0, 0, 0)

		Protected Friend xBumps As Integer
		Protected Friend yBumps As Integer
		Protected Friend topColor As Color
		Protected Friend shadowColor As Color
		Protected Friend backColor As Color

		Private Shared ReadOnly METAL_BUMPS As New Object
		Protected Friend buffer As BumpBuffer

		''' <summary>
		''' Creates MetalBumps of the specified size with the specified colors.
		''' If <code>newBackColor</code> is null, the background will be
		''' transparent.
		''' </summary>
		Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal newTopColor As Color, ByVal newShadowColor As Color, ByVal newBackColor As Color)
			bumpArearea(width, height)
			bumpColorsors(newTopColor, newShadowColor, newBackColor)
		End Sub

		Private Shared Function createBuffer(ByVal gc As GraphicsConfiguration, ByVal topColor As Color, ByVal shadowColor As Color, ByVal backColor As Color) As BumpBuffer
			Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim buffers As IList(Of BumpBuffer) = CType(context.get(METAL_BUMPS), IList(Of BumpBuffer))
			If buffers Is Nothing Then
				buffers = New List(Of BumpBuffer)
				context.put(METAL_BUMPS, buffers)
			End If
			For Each buffer As BumpBuffer In buffers
				If buffer.hasSameConfiguration(gc, topColor, shadowColor, backColor) Then Return buffer
			Next buffer
			Dim buffer As New BumpBuffer(gc, topColor, shadowColor, backColor)
			buffers.Add(buffer)
			Return buffer
		End Function

		Public Overridable Property bumpArea As Dimension
			Set(ByVal bumpArea As Dimension)
				bumpArearea(bumpArea.width, bumpArea.height)
			End Set
		End Property

		Public Overridable Sub setBumpArea(ByVal width As Integer, ByVal height As Integer)
			xBumps = width \ 2
			yBumps = height \ 2
		End Sub

		Public Overridable Sub setBumpColors(ByVal newTopColor As Color, ByVal newShadowColor As Color, ByVal newBackColor As Color)
			topColor = newTopColor
			shadowColor = newShadowColor
			If newBackColor Is Nothing Then
				backColor = ALPHA
			Else
				backColor = newBackColor
			End If
		End Sub

		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim gc As GraphicsConfiguration = If(TypeOf g Is Graphics2D, CType(g, Graphics2D).deviceConfiguration, Nothing)

			If (buffer Is Nothing) OrElse (Not buffer.hasSameConfiguration(gc, topColor, shadowColor, backColor)) Then buffer = createBuffer(gc, topColor, shadowColor, backColor)

			Dim bufferWidth As Integer = BumpBuffer.IMAGE_SIZE
			Dim bufferHeight As Integer = BumpBuffer.IMAGE_SIZE
			Dim ___iconWidth As Integer = iconWidth
			Dim ___iconHeight As Integer = iconHeight
			Dim x2 As Integer = x + ___iconWidth
			Dim y2 As Integer = y + ___iconHeight
			Dim savex As Integer = x

			Do While y < y2
				Dim h As Integer = Math.Min(y2 - y, bufferHeight)
				For x = savex To x2 - 1 Step bufferWidth
					Dim w As Integer = Math.Min(x2 - x, bufferWidth)
					g.drawImage(buffer.image, x, y, x+w, y+h, 0, 0, w, h, Nothing)
				Next x
				y += bufferHeight
			Loop
		End Sub

		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return xBumps * 2
			End Get
		End Property

		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return yBumps * 2
			End Get
		End Property
	End Class


	Friend Class BumpBuffer

		Friend Const IMAGE_SIZE As Integer = 64

		<NonSerialized> _
		Friend image As Image
		Friend topColor As Color
		Friend shadowColor As Color
		Friend backColor As Color
		Private gc As GraphicsConfiguration

		Public Sub New(ByVal gc As GraphicsConfiguration, ByVal aTopColor As Color, ByVal aShadowColor As Color, ByVal aBackColor As Color)
			Me.gc = gc
			topColor = aTopColor
			shadowColor = aShadowColor
			backColor = aBackColor
			createImage()
			fillBumpBuffer()
		End Sub

		Public Overridable Function hasSameConfiguration(ByVal gc As GraphicsConfiguration, ByVal aTopColor As Color, ByVal aShadowColor As Color, ByVal aBackColor As Color) As Boolean
			If Me.gc IsNot Nothing Then
				If Not Me.gc.Equals(gc) Then Return False
			ElseIf gc IsNot Nothing Then
				Return False
			End If
			Return topColor.Equals(aTopColor) AndAlso shadowColor.Equals(aShadowColor) AndAlso backColor.Equals(aBackColor)
		End Function

		''' <summary>
		''' Returns the Image containing the bumps appropriate for the passed in
		''' <code>GraphicsConfiguration</code>.
		''' </summary>
		Public Overridable Property image As Image
			Get
				Return image
			End Get
		End Property

		''' <summary>
		''' Paints the bumps into the current image.
		''' </summary>
		Private Sub fillBumpBuffer()
			Dim g As Graphics = image.graphics

			g.color = backColor
			g.fillRect(0, 0, IMAGE_SIZE, IMAGE_SIZE)

			g.color = topColor
			For x As Integer = 0 To IMAGE_SIZE - 1 Step 4
				For y As Integer = 0 To IMAGE_SIZE - 1 Step 4
					g.drawLine(x, y, x, y)
					g.drawLine(x+2, y+2, x+2, y+2)
				Next y
			Next x

			g.color = shadowColor
			For x As Integer = 0 To IMAGE_SIZE - 1 Step 4
				For y As Integer = 0 To IMAGE_SIZE - 1 Step 4
					g.drawLine(x+1, y+1, x+1, y+1)
					g.drawLine(x+3, y+3, x+3, y+3)
				Next y
			Next x
			g.Dispose()
		End Sub

		''' <summary>
		''' Creates the image appropriate for the passed in
		''' <code>GraphicsConfiguration</code>, which may be null.
		''' </summary>
		Private Sub createImage()
			If gc IsNot Nothing Then
				image = gc.createCompatibleImage(IMAGE_SIZE, IMAGE_SIZE,If(backColor IsNot MetalBumps.ALPHA, Transparency.OPAQUE, Transparency.BITMASK))
			Else
				Dim cmap As Integer() = { backColor.rGB, topColor.rGB, shadowColor.rGB }
				Dim icm As New IndexColorModel(8, 3, cmap, 0, False,If(backColor Is MetalBumps.ALPHA, 0, -1), DataBuffer.TYPE_BYTE)
				image = New BufferedImage(IMAGE_SIZE, IMAGE_SIZE, BufferedImage.TYPE_BYTE_INDEXED, icm)
			End If
		End Sub
	End Class

End Namespace