Imports Microsoft.VisualBasic
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
	''' EffectUtils
	''' 
	''' @author Created by Jasper Potts (Jun 18, 2007)
	''' </summary>
	Friend Class EffectUtils

		''' <summary>
		''' Clear a transparent image to 100% transparent
		''' </summary>
		''' <param name="img"> The image to clear </param>
		Friend Shared Sub clearImage(ByVal img As java.awt.image.BufferedImage)
			Dim g2 As java.awt.Graphics2D = img.createGraphics()
			g2.composite = java.awt.AlphaComposite.Clear
			g2.fillRect(0, 0, img.width, img.height)
			g2.Dispose()
		End Sub

		' =================================================================================================================
		' Blur

		''' <summary>
		''' Apply Gaussian Blur to Image
		''' </summary>
		''' <param name="src">    The image tp </param>
		''' <param name="dst">    The destination image to draw blured src image into, null if you want a new one created </param>
		''' <param name="radius"> The blur kernel radius </param>
		''' <returns> The blured image </returns>
		Friend Shared Function gaussianBlur(ByVal src As java.awt.image.BufferedImage, ByVal dst As java.awt.image.BufferedImage, ByVal radius As Integer) As java.awt.image.BufferedImage
			Dim width As Integer = src.width
			Dim height As Integer = src.height
			If dst Is Nothing OrElse dst.width <> width OrElse dst.height <> height OrElse src.type <> dst.type Then dst = createColorModelCompatibleImage(src)
			Dim kernel As Single() = createGaussianKernel(radius)
			If src.type = java.awt.image.BufferedImage.TYPE_INT_ARGB Then
				Dim srcPixels As Integer() = New Integer(width * height - 1){}
				Dim dstPixels As Integer() = New Integer(width * height - 1){}
				getPixels(src, 0, 0, width, height, srcPixels)
				' horizontal pass
				blur(srcPixels, dstPixels, width, height, kernel, radius)
				' vertical pass
				'noinspection SuspiciousNameCombination
				blur(dstPixels, srcPixels, height, width, kernel, radius)
				' the result is now stored in srcPixels due to the 2nd pass
				pixelsels(dst, 0, 0, width, height, srcPixels)
			ElseIf src.type = java.awt.image.BufferedImage.TYPE_BYTE_GRAY Then
				Dim srcPixels As SByte() = New SByte(width * height - 1){}
				Dim dstPixels As SByte() = New SByte(width * height - 1){}
				getPixels(src, 0, 0, width, height, srcPixels)
				' horizontal pass
				blur(srcPixels, dstPixels, width, height, kernel, radius)
				' vertical pass
				'noinspection SuspiciousNameCombination
				blur(dstPixels, srcPixels, height, width, kernel, radius)
				' the result is now stored in srcPixels due to the 2nd pass
				pixelsels(dst, 0, 0, width, height, srcPixels)
			Else
				Throw New System.ArgumentException("EffectUtils.gaussianBlur() src image is not a supported type, type=[" & src.type & "]")
			End If
			Return dst
		End Function

		''' <summary>
		''' <p>Blurs the source pixels into the destination pixels. The force of the blur is specified by the radius which
		''' must be greater than 0.</p> <p>The source and destination pixels arrays are expected to be in the INT_ARGB
		''' format.</p> <p>After this method is executed, dstPixels contains a transposed and filtered copy of
		''' srcPixels.</p>
		''' </summary>
		''' <param name="srcPixels"> the source pixels </param>
		''' <param name="dstPixels"> the destination pixels </param>
		''' <param name="width">     the width of the source picture </param>
		''' <param name="height">    the height of the source picture </param>
		''' <param name="kernel">    the kernel of the blur effect </param>
		''' <param name="radius">    the radius of the blur effect </param>
		Private Shared Sub blur(ByVal srcPixels As Integer(), ByVal dstPixels As Integer(), ByVal width As Integer, ByVal height As Integer, ByVal kernel As Single(), ByVal radius As Integer)
			Dim a As Single
			Dim r As Single
			Dim g As Single
			Dim b As Single

			Dim ca As Integer
			Dim cr As Integer
			Dim cg As Integer
			Dim cb As Integer

			For y As Integer = 0 To height - 1
				Dim index As Integer = y
				Dim offset As Integer = y * width

				For x As Integer = 0 To width - 1
						b = 0.0f
							g = b
								r = g
								a = r

					For i As Integer = -radius To radius
						Dim subOffset As Integer = x + i
						If subOffset < 0 OrElse subOffset >= width Then subOffset = (x + width) Mod width

						Dim pixel As Integer = srcPixels(offset + subOffset)
						Dim blurFactor As Single = kernel(radius + i)

						a += blurFactor * ((pixel >> 24) And &HFF)
						r += blurFactor * ((pixel >> 16) And &HFF)
						g += blurFactor * ((pixel >> 8) And &HFF)
						b += blurFactor * ((pixel) And &HFF)
					Next i

					ca = CInt(Fix(a + 0.5f))
					cr = CInt(Fix(r + 0.5f))
					cg = CInt(Fix(g + 0.5f))
					cb = CInt(Fix(b + 0.5f))

					dstPixels(index) = ((If(ca > 255, 255, ca)) << 24) Or ((If(cr > 255, 255, cr)) << 16) Or ((If(cg > 255, 255, cg)) << 8) Or (If(cb > 255, 255, cb))
					index += height
				Next x
			Next y
		End Sub

		''' <summary>
		''' <p>Blurs the source pixels into the destination pixels. The force of the blur is specified by the radius which
		''' must be greater than 0.</p> <p>The source and destination pixels arrays are expected to be in the BYTE_GREY
		''' format.</p> <p>After this method is executed, dstPixels contains a transposed and filtered copy of
		''' srcPixels.</p>
		''' </summary>
		''' <param name="srcPixels"> the source pixels </param>
		''' <param name="dstPixels"> the destination pixels </param>
		''' <param name="width">     the width of the source picture </param>
		''' <param name="height">    the height of the source picture </param>
		''' <param name="kernel">    the kernel of the blur effect </param>
		''' <param name="radius">    the radius of the blur effect </param>
		Friend Shared Sub blur(ByVal srcPixels As SByte(), ByVal dstPixels As SByte(), ByVal width As Integer, ByVal height As Integer, ByVal kernel As Single(), ByVal radius As Integer)
			Dim p As Single
			Dim cp As Integer
			For y As Integer = 0 To height - 1
				Dim index As Integer = y
				Dim offset As Integer = y * width
				For x As Integer = 0 To width - 1
					p = 0.0f
					For i As Integer = -radius To radius
						Dim subOffset As Integer = x + i
	'                    if (subOffset < 0) subOffset = 0;
	'                    if (subOffset >= width) subOffset = width-1;
						If subOffset < 0 OrElse subOffset >= width Then subOffset = (x + width) Mod width
						Dim pixel As Integer = srcPixels(offset + subOffset) And &HFF
						Dim blurFactor As Single = kernel(radius + i)
						p += blurFactor * pixel
					Next i
					cp = CInt(Fix(p + 0.5f))
					dstPixels(index) = CByte(If(cp > 255, 255, cp))
					index += height
				Next x
			Next y
		End Sub

		Friend Shared Function createGaussianKernel(ByVal radius As Integer) As Single()
			If radius < 1 Then Throw New System.ArgumentException("Radius must be >= 1")

			Dim data As Single() = New Single(radius * 2){}

			Dim sigma As Single = radius / 3.0f
			Dim twoSigmaSquare As Single = 2.0f * sigma * sigma
			Dim sigmaRoot As Single = CSng(Math.Sqrt(twoSigmaSquare * Math.PI))
			Dim total As Single = 0.0f

			For i As Integer = -radius To radius
				Dim distance As Single = i * i
				Dim index As Integer = i + radius
				data(index) = CSng(Math.Exp(-distance / twoSigmaSquare)) / sigmaRoot
				total += data(index)
			Next i

			For i As Integer = 0 To data.Length - 1
				data(i) /= total
			Next i

			Return data
		End Function

		' =================================================================================================================
		' Get/Set Pixels helper methods

		''' <summary>
		''' <p>Returns an array of pixels, stored as integers, from a <code>BufferedImage</code>. The pixels are grabbed from
		''' a rectangular area defined by a location and two dimensions. Calling this method on an image of type different
		''' from <code>BufferedImage.TYPE_INT_ARGB</code> and <code>BufferedImage.TYPE_INT_RGB</code> will unmanage the
		''' image.</p>
		''' </summary>
		''' <param name="img">    the source image </param>
		''' <param name="x">      the x location at which to start grabbing pixels </param>
		''' <param name="y">      the y location at which to start grabbing pixels </param>
		''' <param name="w">      the width of the rectangle of pixels to grab </param>
		''' <param name="h">      the height of the rectangle of pixels to grab </param>
		''' <param name="pixels"> a pre-allocated array of pixels of size w*h; can be null </param>
		''' <returns> <code>pixels</code> if non-null, a new array of integers otherwise </returns>
		''' <exception cref="IllegalArgumentException"> is <code>pixels</code> is non-null and of length &lt; w*h </exception>
		Friend Shared Function getPixels(ByVal img As java.awt.image.BufferedImage, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixels As SByte()) As SByte()
			If w = 0 OrElse h = 0 Then Return New SByte(){}

			If pixels Is Nothing Then
				pixels = New SByte(w * h - 1){}
			ElseIf pixels.Length < w * h Then
				Throw New System.ArgumentException("pixels array must have a length >= w*h")
			End If

			Dim imageType As Integer = img.type
			If imageType = java.awt.image.BufferedImage.TYPE_BYTE_GRAY Then
				Dim raster As java.awt.image.Raster = img.raster
				Return CType(raster.getDataElements(x, y, w, h, pixels), SByte())
			Else
				Throw New System.ArgumentException("Only type BYTE_GRAY is supported")
			End If
		End Function

		''' <summary>
		''' <p>Writes a rectangular area of pixels in the destination <code>BufferedImage</code>. Calling this method on an
		''' image of type different from <code>BufferedImage.TYPE_INT_ARGB</code> and <code>BufferedImage.TYPE_INT_RGB</code>
		''' will unmanage the image.</p>
		''' </summary>
		''' <param name="img">    the destination image </param>
		''' <param name="x">      the x location at which to start storing pixels </param>
		''' <param name="y">      the y location at which to start storing pixels </param>
		''' <param name="w">      the width of the rectangle of pixels to store </param>
		''' <param name="h">      the height of the rectangle of pixels to store </param>
		''' <param name="pixels"> an array of pixels, stored as integers </param>
		''' <exception cref="IllegalArgumentException"> is <code>pixels</code> is non-null and of length &lt; w*h </exception>
		Friend Shared Sub setPixels(ByVal img As java.awt.image.BufferedImage, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixels As SByte())
			If pixels Is Nothing OrElse w = 0 OrElse h = 0 Then
				Return
			ElseIf pixels.Length < w * h Then
				Throw New System.ArgumentException("pixels array must have a length >= w*h")
			End If
			Dim imageType As Integer = img.type
			If imageType = java.awt.image.BufferedImage.TYPE_BYTE_GRAY Then
				Dim raster As java.awt.image.WritableRaster = img.raster
				raster.dataElementsnts(x, y, w, h, pixels)
			Else
				Throw New System.ArgumentException("Only type BYTE_GRAY is supported")
			End If
		End Sub

		''' <summary>
		''' <p>Returns an array of pixels, stored as integers, from a
		''' <code>BufferedImage</code>. The pixels are grabbed from a rectangular
		''' area defined by a location and two dimensions. Calling this method on
		''' an image of type different from <code>BufferedImage.TYPE_INT_ARGB</code>
		''' and <code>BufferedImage.TYPE_INT_RGB</code> will unmanage the image.</p>
		''' </summary>
		''' <param name="img"> the source image </param>
		''' <param name="x"> the x location at which to start grabbing pixels </param>
		''' <param name="y"> the y location at which to start grabbing pixels </param>
		''' <param name="w"> the width of the rectangle of pixels to grab </param>
		''' <param name="h"> the height of the rectangle of pixels to grab </param>
		''' <param name="pixels"> a pre-allocated array of pixels of size w*h; can be null </param>
		''' <returns> <code>pixels</code> if non-null, a new array of integers
		'''   otherwise </returns>
		''' <exception cref="IllegalArgumentException"> is <code>pixels</code> is non-null and
		'''   of length &lt; w*h </exception>
		Public Shared Function getPixels(ByVal img As java.awt.image.BufferedImage, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixels As Integer()) As Integer()
			If w = 0 OrElse h = 0 Then Return New Integer(){}

			If pixels Is Nothing Then
				pixels = New Integer(w * h - 1){}
			ElseIf pixels.Length < w * h Then
				Throw New System.ArgumentException("pixels array must have a length" & " >= w*h")
			End If

			Dim imageType As Integer = img.type
			If imageType = java.awt.image.BufferedImage.TYPE_INT_ARGB OrElse imageType = java.awt.image.BufferedImage.TYPE_INT_RGB Then
				Dim raster As java.awt.image.Raster = img.raster
				Return CType(raster.getDataElements(x, y, w, h, pixels), Integer())
			End If

			' Unmanages the image
			Return img.getRGB(x, y, w, h, pixels, 0, w)
		End Function

		''' <summary>
		''' <p>Writes a rectangular area of pixels in the destination
		''' <code>BufferedImage</code>. Calling this method on
		''' an image of type different from <code>BufferedImage.TYPE_INT_ARGB</code>
		''' and <code>BufferedImage.TYPE_INT_RGB</code> will unmanage the image.</p>
		''' </summary>
		''' <param name="img"> the destination image </param>
		''' <param name="x"> the x location at which to start storing pixels </param>
		''' <param name="y"> the y location at which to start storing pixels </param>
		''' <param name="w"> the width of the rectangle of pixels to store </param>
		''' <param name="h"> the height of the rectangle of pixels to store </param>
		''' <param name="pixels"> an array of pixels, stored as integers </param>
		''' <exception cref="IllegalArgumentException"> is <code>pixels</code> is non-null and
		'''   of length &lt; w*h </exception>
		Public Shared Sub setPixels(ByVal img As java.awt.image.BufferedImage, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixels As Integer())
			If pixels Is Nothing OrElse w = 0 OrElse h = 0 Then
				Return
			ElseIf pixels.Length < w * h Then
				Throw New System.ArgumentException("pixels array must have a length" & " >= w*h")
			End If

			Dim imageType As Integer = img.type
			If imageType = java.awt.image.BufferedImage.TYPE_INT_ARGB OrElse imageType = java.awt.image.BufferedImage.TYPE_INT_RGB Then
				Dim raster As java.awt.image.WritableRaster = img.raster
				raster.dataElementsnts(x, y, w, h, pixels)
			Else
				' Unmanages the image
				img.rGBRGB(x, y, w, h, pixels, 0, w)
			End If
		End Sub

		''' <summary>
		''' <p>Returns a new <code>BufferedImage</code> using the same color model
		''' as the image passed as a parameter. The returned image is only compatible
		''' with the image passed as a parameter. This does not mean the returned
		''' image is compatible with the hardware.</p>
		''' </summary>
		''' <param name="image"> the reference image from which the color model of the new
		'''   image is obtained </param>
		''' <returns> a new <code>BufferedImage</code>, compatible with the color model
		'''   of <code>image</code> </returns>
		Public Shared Function createColorModelCompatibleImage(ByVal image As java.awt.image.BufferedImage) As java.awt.image.BufferedImage
			Dim cm As java.awt.image.ColorModel = image.colorModel
			Return New java.awt.image.BufferedImage(cm, cm.createCompatibleWritableRaster(image.width, image.height), cm.alphaPremultiplied, Nothing)
		End Function

		''' <summary>
		''' <p>Returns a new translucent compatible image of the specified width and
		''' height. That is, the returned <code>BufferedImage</code> is compatible with
		''' the graphics hardware. If the method is called in a headless
		''' environment, then the returned BufferedImage will be compatible with
		''' the source image.</p>
		''' </summary>
		''' <param name="width"> the width of the new image </param>
		''' <param name="height"> the height of the new image </param>
		''' <returns> a new translucent compatible <code>BufferedImage</code> of the
		'''   specified width and height </returns>
		Public Shared Function createCompatibleTranslucentImage(ByVal width As Integer, ByVal height As Integer) As java.awt.image.BufferedImage
			Return If(headless, New java.awt.image.BufferedImage(width, height, java.awt.image.BufferedImage.TYPE_INT_ARGB), graphicsConfiguration.createCompatibleImage(width, height, java.awt.Transparency.TRANSLUCENT))
		End Function

		Private Property Shared headless As Boolean
			Get
				Return java.awt.GraphicsEnvironment.headless
			End Get
		End Property

		' Returns the graphics configuration for the primary screen
		Private Property Shared graphicsConfiguration As java.awt.GraphicsConfiguration
			Get
				Return java.awt.GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice.defaultConfiguration
			End Get
		End Property

	End Class

End Namespace