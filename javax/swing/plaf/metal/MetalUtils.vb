Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports javax.swing.plaf
Imports javax.swing

'
' * Copyright (c) 1998, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This is a dumping ground for random stuff we want to use in several places.
	''' 
	''' @author Steve Wilson
	''' </summary>

	Friend Class MetalUtils

		Friend Shared Sub drawFlush3DBorder(ByVal g As Graphics, ByVal r As Rectangle)
			drawFlush3DBorder(g, r.x, r.y, r.width, r.height)
		End Sub

		''' <summary>
		''' This draws the "Flush 3D Border" which is used throughout the Metal L&F
		''' </summary>
		Friend Shared Sub drawFlush3DBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			g.translate(x, y)
			g.color = MetalLookAndFeel.controlDarkShadow
			g.drawRect(0, 0, w-2, h-2)
			g.color = MetalLookAndFeel.controlHighlight
			g.drawRect(1, 1, w-2, h-2)
			g.color = MetalLookAndFeel.control
			g.drawLine(0, h-1, 1, h-2)
			g.drawLine(w-1, 0, w-2, 1)
			g.translate(-x, -y)
		End Sub

		''' <summary>
		''' This draws a variant "Flush 3D Border"
		''' It is used for things like pressed buttons.
		''' </summary>
		Friend Shared Sub drawPressed3DBorder(ByVal g As Graphics, ByVal r As Rectangle)
			drawPressed3DBorder(g, r.x, r.y, r.width, r.height)
		End Sub

		Friend Shared Sub drawDisabledBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			g.translate(x, y)
			g.color = MetalLookAndFeel.controlShadow
			g.drawRect(0, 0, w-1, h-1)
			g.translate(-x, -y)
		End Sub

		''' <summary>
		''' This draws a variant "Flush 3D Border"
		''' It is used for things like pressed buttons.
		''' </summary>
		Friend Shared Sub drawPressed3DBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			g.translate(x, y)

			drawFlush3DBorder(g, 0, 0, w, h)

			g.color = MetalLookAndFeel.controlShadow
			g.drawLine(1, 1, 1, h-2)
			g.drawLine(1, 1, w-2, 1)
			g.translate(-x, -y)
		End Sub

		''' <summary>
		''' This draws a variant "Flush 3D Border"
		''' It is used for things like active toggle buttons.
		''' This is used rarely.
		''' </summary>
		Friend Shared Sub drawDark3DBorder(ByVal g As Graphics, ByVal r As Rectangle)
			drawDark3DBorder(g, r.x, r.y, r.width, r.height)
		End Sub

		''' <summary>
		''' This draws a variant "Flush 3D Border"
		''' It is used for things like active toggle buttons.
		''' This is used rarely.
		''' </summary>
		Friend Shared Sub drawDark3DBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			g.translate(x, y)

			drawFlush3DBorder(g, 0, 0, w, h)

			g.color = MetalLookAndFeel.control
			g.drawLine(1, 1, 1, h-2)
			g.drawLine(1, 1, w-2, 1)
			g.color = MetalLookAndFeel.controlShadow
			g.drawLine(1, h-2, 1, h-2)
			g.drawLine(w-2, 1, w-2, 1)
			g.translate(-x, -y)
		End Sub

		Friend Shared Sub drawButtonBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal active As Boolean)
			If active Then
				drawActiveButtonBorder(g, x, y, w, h)
			Else
				drawFlush3DBorder(g, x, y, w, h)
			End If
		End Sub

		Friend Shared Sub drawActiveButtonBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			drawFlush3DBorder(g, x, y, w, h)
			g.color = MetalLookAndFeel.primaryControl
			g.drawLine(x+1, y+1, x+1, h-3)
			g.drawLine(x+1, y+1, w-3, x+1)
			g.color = MetalLookAndFeel.primaryControlDarkShadow
			g.drawLine(x+2, h-2, w-2, h-2)
			g.drawLine(w-2, y+2, w-2, h-2)
		End Sub

		Friend Shared Sub drawDefaultButtonBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal active As Boolean)
			drawButtonBorder(g, x+1, y+1, w-1, h-1, active)
			g.translate(x, y)
			g.color = MetalLookAndFeel.controlDarkShadow
			g.drawRect(0, 0, w-3, h-3)
			g.drawLine(w-2, 0, w-2, 0)
			g.drawLine(0, h-2, 0, h-2)
			g.translate(-x, -y)
		End Sub

		Friend Shared Sub drawDefaultButtonPressedBorder(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			drawPressed3DBorder(g, x + 1, y + 1, w - 1, h - 1)
			g.translate(x, y)
			g.color = MetalLookAndFeel.controlDarkShadow
			g.drawRect(0, 0, w - 3, h - 3)
			g.drawLine(w - 2, 0, w - 2, 0)
			g.drawLine(0, h - 2, 0, h - 2)
			g.color = MetalLookAndFeel.control
			g.drawLine(w - 1, 0, w - 1, 0)
			g.drawLine(0, h - 1, 0, h - 1)
			g.translate(-x, -y)
		End Sub

	'    
	'     * Convenience function for determining ComponentOrientation.  Helps us
	'     * avoid having Munge directives throughout the code.
	'     
		Friend Shared Function isLeftToRight(ByVal c As Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function

		Friend Shared Function getInt(ByVal key As Object, ByVal defaultValue As Integer) As Integer
			Dim value As Object = UIManager.get(key)

			If TypeOf value Is Integer? Then Return CInt(Fix(value))
			If TypeOf value Is String Then
				Try
					Return Convert.ToInt32(CStr(value))
				Catch nfe As NumberFormatException
				End Try
			End If
			Return defaultValue
		End Function

		'
		' Ocean specific stuff.
		'
		''' <summary>
		''' Draws a radial type gradient. The gradient will be drawn vertically if
		''' <code>vertical</code> is true, otherwise horizontally.
		''' The UIManager key consists of five values:
		''' r1 r2 c1 c2 c3. The gradient is broken down into four chunks drawn
		''' in order from the origin.
		''' <ol>
		''' <li>Gradient r1 % of the size from c1 to c2
		''' <li>Rectangle r2 % of the size in c2.
		''' <li>Gradient r1 % of the size from c2 to c1
		''' <li>The remaining size will be filled with a gradient from c1 to c3.
		''' </ol>
		''' </summary>
		''' <param name="c"> Component rendering to </param>
		''' <param name="g"> Graphics to draw to. </param>
		''' <param name="key"> UIManager key used to look up gradient values. </param>
		''' <param name="x"> X coordinate to draw from </param>
		''' <param name="y"> Y coordinate to draw from </param>
		''' <param name="w"> Width to draw to </param>
		''' <param name="h"> Height to draw to </param>
		''' <param name="vertical"> Direction of the gradient </param>
		''' <returns> true if <code>key</code> exists, otherwise false. </returns>
		Friend Shared Function drawGradient(ByVal c As Component, ByVal g As Graphics, ByVal key As String, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal vertical As Boolean) As Boolean
			Dim gradient As IList = CType(UIManager.get(key), IList)
			If gradient Is Nothing OrElse Not(TypeOf g Is Graphics2D) Then Return False

			If w <= 0 OrElse h <= 0 Then Return True

			GradientPainter.INSTANCE.paint(c, CType(g, Graphics2D), gradient, x, y, w, h, vertical)
			Return True
		End Function


		Private Class GradientPainter
			Inherits sun.swing.CachedPainter

			''' <summary>
			''' Instance used for painting.  This is the only instance that is
			''' ever created.
			''' </summary>
			Public Shared ReadOnly INSTANCE As New GradientPainter(8)

			' Size of images to create. For vertical gradients this is the width,
			' otherwise it's the height.
			Private Const IMAGE_SIZE As Integer = 64

			''' <summary>
			''' This is the actual width we're painting in, or last painted to.
			''' </summary>
			Private w As Integer
			''' <summary>
			''' This is the actual height we're painting in, or last painted to
			''' </summary>
			Private h As Integer


			Friend Sub New(ByVal count As Integer)
				MyBase.New(count)
			End Sub

			Public Overridable Sub paint(ByVal c As Component, ByVal g As Graphics2D, ByVal gradient As IList, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isVertical As Boolean)
				Dim imageWidth As Integer
				Dim imageHeight As Integer
				If isVertical Then
					imageWidth = IMAGE_SIZE
					imageHeight = h
				Else
					imageWidth = w
					imageHeight = IMAGE_SIZE
				End If
				SyncLock c.treeLock
					Me.w = w
					Me.h = h
					paint(c, g, x, y, imageWidth, imageHeight, gradient, isVertical)
				End SyncLock
			End Sub

			Protected Friend Overridable Sub paintToImage(ByVal c As Component, ByVal image As Image, ByVal g As Graphics, ByVal w As Integer, ByVal h As Integer, ByVal args As Object())
				Dim g2 As Graphics2D = CType(g, Graphics2D)
				Dim ___gradient As IList = CType(args(0), IList)
				Dim isVertical As Boolean = CBool(args(1))
				' Render to the VolatileImage
				If isVertical Then
					drawVerticalGradient(g2, CType(___gradient(0), Number), CType(___gradient(1), Number), CType(___gradient(2), Color), CType(___gradient(3), Color), CType(___gradient(4), Color), w, h)
				Else
					drawHorizontalGradient(g2, CType(___gradient(0), Number), CType(___gradient(1), Number), CType(___gradient(2), Color), CType(___gradient(3), Color), CType(___gradient(4), Color), w, h)
				End If
			End Sub

			Protected Friend Overridable Sub paintImage(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal imageW As Integer, ByVal imageH As Integer, ByVal image As Image, ByVal args As Object())
				Dim isVertical As Boolean = CBool(args(1))
				' Render to the screen
				g.translate(x, y)
				If isVertical Then
					For counter As Integer = 0 To w - 1 Step IMAGE_SIZE
						Dim tileSize As Integer = Math.Min(IMAGE_SIZE, w - counter)
						g.drawImage(image, counter, 0, counter + tileSize, h, 0, 0, tileSize, h, Nothing)
					Next counter
				Else
					For counter As Integer = 0 To h - 1 Step IMAGE_SIZE
						Dim tileSize As Integer = Math.Min(IMAGE_SIZE, h - counter)
						g.drawImage(image, 0, counter, w, counter + tileSize, 0, 0, w, tileSize, Nothing)
					Next counter
				End If
				g.translate(-x, -y)
			End Sub

			Private Sub drawVerticalGradient(ByVal g As Graphics2D, ByVal ratio1 As Single, ByVal ratio2 As Single, ByVal c1 As Color, ByVal c2 As Color, ByVal c3 As Color, ByVal w As Integer, ByVal h As Integer)
				Dim mid As Integer = CInt(Fix(ratio1 * h))
				Dim mid2 As Integer = CInt(Fix(ratio2 * h))
				If mid > 0 Then
					g.paint = getGradient(CSng(0), CSng(0), c1, CSng(0), CSng(mid), c2)
					g.fillRect(0, 0, w, mid)
				End If
				If mid2 > 0 Then
					g.color = c2
					g.fillRect(0, mid, w, mid2)
				End If
				If mid > 0 Then
					g.paint = getGradient(CSng(0), CSng(mid) + mid2, c2, CSng(0), CSng(mid) * 2 + mid2, c1)
					g.fillRect(0, mid + mid2, w, mid)
				End If
				If h - mid * 2 - mid2 > 0 Then
					g.paint = getGradient(CSng(0), CSng(mid) * 2 + mid2, c1, CSng(0), CSng(h), c3)
					g.fillRect(0, mid * 2 + mid2, w, h - mid * 2 - mid2)
				End If
			End Sub

			Private Sub drawHorizontalGradient(ByVal g As Graphics2D, ByVal ratio1 As Single, ByVal ratio2 As Single, ByVal c1 As Color, ByVal c2 As Color, ByVal c3 As Color, ByVal w As Integer, ByVal h As Integer)
				Dim mid As Integer = CInt(Fix(ratio1 * w))
				Dim mid2 As Integer = CInt(Fix(ratio2 * w))
				If mid > 0 Then
					g.paint = getGradient(CSng(0), CSng(0), c1, CSng(mid), CSng(0), c2)
					g.fillRect(0, 0, mid, h)
				End If
				If mid2 > 0 Then
					g.color = c2
					g.fillRect(mid, 0, mid2, h)
				End If
				If mid > 0 Then
					g.paint = getGradient(CSng(mid) + mid2, CSng(0), c2, CSng(mid) * 2 + mid2, CSng(0), c1)
					g.fillRect(mid + mid2, 0, mid, h)
				End If
				If w - mid * 2 - mid2 > 0 Then
					g.paint = getGradient(CSng(mid) * 2 + mid2, CSng(0), c1, w, CSng(0), c3)
					g.fillRect(mid * 2 + mid2, 0, w - mid * 2 - mid2, h)
				End If
			End Sub

			Private Function getGradient(ByVal x1 As Single, ByVal y1 As Single, ByVal c1 As Color, ByVal x2 As Single, ByVal y2 As Single, ByVal c2 As Color) As GradientPaint
				Return New GradientPaint(x1, y1, c1, x2, y2, c2, True)
			End Function
		End Class


		''' <summary>
		''' Returns true if the specified widget is in a toolbar.
		''' </summary>
		Friend Shared Function isToolBarButton(ByVal c As JComponent) As Boolean
			Return (TypeOf c.parent Is JToolBar)
		End Function

		Friend Shared Function getOceanToolBarIcon(ByVal i As Image) As Icon
			Dim prod As ImageProducer = New FilteredImageSource(i.source, New OceanToolBarImageFilter)
			Return New sun.swing.ImageIconUIResource(Toolkit.defaultToolkit.createImage(prod))
		End Function

		Friend Shared Function getOceanDisabledButtonIcon(ByVal image As Image) As Icon
			Dim range As Object() = CType(UIManager.get("Button.disabledGrayRange"), Object())
			Dim min As Integer = 180
			Dim max As Integer = 215
			If range IsNot Nothing Then
				min = CInt(Fix(range(0)))
				max = CInt(Fix(range(1)))
			End If
			Dim prod As ImageProducer = New FilteredImageSource(image.source, New OceanDisabledButtonImageFilter(min, max))
			Return New sun.swing.ImageIconUIResource(Toolkit.defaultToolkit.createImage(prod))
		End Function




		''' <summary>
		''' Used to create a disabled Icon with the ocean look.
		''' </summary>
		Private Class OceanDisabledButtonImageFilter
			Inherits RGBImageFilter

			Private min As Single
			Private factor As Single

			Friend Sub New(ByVal min As Integer, ByVal max As Integer)
				canFilterIndexColorModel = True
				Me.min = CSng(min)
				Me.factor = (max - min) / 255f
			End Sub

			Public Overridable Function filterRGB(ByVal x As Integer, ByVal y As Integer, ByVal rgb As Integer) As Integer
				' Coefficients are from the sRGB color space:
				Dim gray As Integer = Math.Min(255, CInt(Fix(((0.2125f * ((rgb >> 16) And &HFF)) + (0.7154f * ((rgb >> 8) And &HFF)) + (0.0721f * (rgb And &HFF)) +.5f) * factor + min)))

				Return (rgb And &Hff000000L) Or (gray << 16) Or (gray << 8) Or (gray << 0)
			End Function
		End Class


		''' <summary>
		''' Used to create the rollover icons with the ocean look.
		''' </summary>
		Private Class OceanToolBarImageFilter
			Inherits RGBImageFilter

			Friend Sub New()
				canFilterIndexColorModel = True
			End Sub

			Public Overridable Function filterRGB(ByVal x As Integer, ByVal y As Integer, ByVal rgb As Integer) As Integer
				Dim r As Integer = ((rgb >> 16) And &Hff)
				Dim g As Integer = ((rgb >> 8) And &Hff)
				Dim b As Integer = (rgb And &Hff)
				Dim gray As Integer = Math.Max(Math.Max(r, g), b)
				Return (rgb And &Hff000000L) Or (gray << 16) Or (gray << 8) Or (gray << 0)
			End Function
		End Class
	End Class

End Namespace