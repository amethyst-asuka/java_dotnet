Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' LoweredBorder - A recessed rounded inner shadowed border. Used as the
	''' standard Nimbus TitledBorder. This class is both a painter and a swing
	''' border.
	''' 
	''' @author Jasper Potts
	''' </summary>
	Friend Class LoweredBorder
		Inherits AbstractRegionPainter
		Implements javax.swing.border.Border

		Private Const IMG_SIZE As Integer = 30
		Private Const RADIUS As Integer = 13
		Private Shared ReadOnly INSETS As New java.awt.Insets(10,10,10,10)
		Private Shared ReadOnly PAINT_CONTEXT As New PaintContext(INSETS, New java.awt.Dimension(IMG_SIZE,IMG_SIZE),False, PaintContext.CacheMode.NINE_SQUARE_SCALE, Integer.MAX_VALUE, Integer.MAX_VALUE)

		' =========================================================================
		' Painter Methods

		Protected Friend Overrides Function getExtendedCacheKeys(ByVal c As javax.swing.JComponent) As Object()
			Return If(c IsNot Nothing, New Object() { c.background }, Nothing)
		End Function

		''' <summary>
		''' Actually performs the painting operation. Subclasses must implement this
		''' method. The graphics object passed may represent the actual surface being
		''' rendered to, or it may be an intermediate buffer. It has also been
		''' pre-translated. Simply render the component as if it were located at 0, 0
		''' and had a width of <code>width</code> and a height of
		''' <code>height</code>. For performance reasons, you may want to read the
		''' clip from the Graphics2D object and only render within that space.
		''' </summary>
		''' <param name="g">      The Graphics2D surface to paint to </param>
		''' <param name="c">      The JComponent related to the drawing event. For example,
		'''               if the region being rendered is Button, then <code>c</code>
		'''               will be a JButton. If the region being drawn is
		'''               ScrollBarSlider, then the component will be JScrollBar.
		'''               This value may be null. </param>
		''' <param name="width">  The width of the region to paint. Note that in the case of
		'''               painting the foreground, this value may differ from
		'''               c.getWidth(). </param>
		''' <param name="height"> The height of the region to paint. Note that in the case of
		'''               painting the foreground, this value may differ from
		'''               c.getHeight(). </param>
		Protected Friend Overridable Sub doPaint(ByVal g As java.awt.Graphics2D, ByVal c As javax.swing.JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())
			Dim color As java.awt.Color = If(c Is Nothing, java.awt.Color.BLACK, c.background)
			Dim img1 As New java.awt.image.BufferedImage(IMG_SIZE,IMG_SIZE, java.awt.image.BufferedImage.TYPE_INT_ARGB)
			Dim img2 As New java.awt.image.BufferedImage(IMG_SIZE,IMG_SIZE, java.awt.image.BufferedImage.TYPE_INT_ARGB)
			' draw shadow shape
			Dim g2 As java.awt.Graphics2D = CType(img1.graphics, java.awt.Graphics2D)
			g2.renderingHintint(java.awt.RenderingHints.KEY_ANTIALIASING, java.awt.RenderingHints.VALUE_ANTIALIAS_ON)
			g2.color = color
			g2.fillRoundRect(2,0,26,26,RADIUS,RADIUS)
			g2.Dispose()
			' draw shadow
			Dim ___effect As New InnerShadowEffect
			___effect.distance = 1
			___effect.size = 3
			___effect.color = getLighter(color, 2.1f)
			___effect.angle = 90
			___effect.applyEffect(img1,img2,IMG_SIZE,IMG_SIZE)
			' draw outline to img2
			g2 = CType(img2.graphics, java.awt.Graphics2D)
			g2.renderingHintint(java.awt.RenderingHints.KEY_ANTIALIASING, java.awt.RenderingHints.VALUE_ANTIALIAS_ON)
			g2.cliplip(0,28,IMG_SIZE,1)
			g2.color = getLighter(color, 0.90f)
			g2.drawRoundRect(2,1,25,25,RADIUS,RADIUS)
			g2.Dispose()
			' draw final image
			If width <> IMG_SIZE OrElse height <> IMG_SIZE Then
				ImageScalingHelper.paint(g,0,0,width,height,img2, INSETS, INSETS, ImageScalingHelper.PaintType.PAINT9_STRETCH, ImageScalingHelper.PAINT_ALL)
			Else
				g.drawImage(img2,0,0,c)
			End If
			img1 = Nothing
			img2 = Nothing
		End Sub

		''' <summary>
		''' <p>Gets the PaintContext for this painting operation. This method is
		''' called on every paint, and so should be fast and produce no garbage. The
		''' PaintContext contains information such as cache hints. It also contains
		''' data necessary for decoding points at runtime, such as the stretching
		''' insets, the canvas size at which the encoded points were defined, and
		''' whether the stretching insets are inverted.</p>
		''' <p/>
		''' <p> This method allows for subclasses to package the painting of
		''' different states with possibly different canvas sizes, etc, into one
		''' AbstractRegionPainter implementation.</p>
		''' </summary>
		''' <returns> a PaintContext associated with this paint operation. </returns>
		Protected Friend Property Overrides paintContext As PaintContext
			Get
				Return PAINT_CONTEXT
			End Get
		End Property

		' =========================================================================
		' Border Methods

		''' <summary>
		''' Returns the insets of the border.
		''' </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		Public Overridable Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets
			Return CType(INSETS.clone(), java.awt.Insets)
		End Function

		''' <summary>
		''' Returns whether or not the border is opaque.  If the border is opaque, it
		''' is responsible for filling in it's own background when painting.
		''' </summary>
		Public Overridable Property borderOpaque As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Paints the border for the specified component with the specified position
		''' and size.
		''' </summary>
		''' <param name="c">      the component for which this border is being painted </param>
		''' <param name="g">      the paint graphics </param>
		''' <param name="x">      the x position of the painted border </param>
		''' <param name="y">      the y position of the painted border </param>
		''' <param name="width">  the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overridable Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim comp As javax.swing.JComponent = If(TypeOf c Is javax.swing.JComponent, CType(c, javax.swing.JComponent), Nothing)
			If TypeOf g Is java.awt.Graphics2D Then
				Dim g2 As java.awt.Graphics2D = CType(g, java.awt.Graphics2D)
				g2.translate(x,y)
				paint(g2,comp, width, height)
				g2.translate(-x,-y)
			Else
				Dim img As New java.awt.image.BufferedImage(IMG_SIZE,IMG_SIZE, java.awt.image.BufferedImage.TYPE_INT_ARGB)
				Dim g2 As java.awt.Graphics2D = CType(img.graphics, java.awt.Graphics2D)
				paint(g2,comp, width, height)
				g2.Dispose()
				ImageScalingHelper.paint(g,x,y,width,height,img,INSETS, INSETS, ImageScalingHelper.PaintType.PAINT9_STRETCH, ImageScalingHelper.PAINT_ALL)
			End If
		End Sub

		Private Function getLighter(ByVal c As java.awt.Color, ByVal factor As Single) As java.awt.Color
			Return New java.awt.Color(Math.Min(CInt(Fix(c.red/factor)), 255), Math.Min(CInt(Fix(c.green/factor)), 255), Math.Min(CInt(Fix(c.blue/factor)), 255))
		End Function
	End Class


End Namespace