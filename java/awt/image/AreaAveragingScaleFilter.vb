Imports System

'
' * Copyright (c) 1996, 2002, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image


	''' <summary>
	''' An ImageFilter class for scaling images using a simple area averaging
	''' algorithm that produces smoother results than the nearest neighbor
	''' algorithm.
	''' <p>This class extends the basic ImageFilter Class to scale an existing
	''' image and provide a source for a new image containing the resampled
	''' image.  The pixels in the source image are blended to produce pixels
	''' for an image of the specified size.  The blending process is analogous
	''' to scaling up the source image to a multiple of the destination size
	''' using pixel replication and then scaling it back down to the destination
	''' size by simply averaging all the pixels in the supersized image that
	''' fall within a given pixel of the destination image.  If the data from
	''' the source is not delivered in TopDownLeftRight order then the filter
	''' will back off to a simple pixel replication behavior and utilize the
	''' requestTopDownLeftRightResend() method to refilter the pixels in a
	''' better way at the end.
	''' <p>It is meant to be used in conjunction with a FilteredImageSource
	''' object to produce scaled versions of existing images.  Due to
	''' implementation dependencies, there may be differences in pixel values
	''' of an image filtered on different platforms.
	''' </summary>
	''' <seealso cref= FilteredImageSource </seealso>
	''' <seealso cref= ReplicateScaleFilter </seealso>
	''' <seealso cref= ImageFilter
	''' 
	''' @author      Jim Graham </seealso>
	Public Class AreaAveragingScaleFilter
		Inherits ReplicateScaleFilter

		Private Shared ReadOnly rgbmodel As java.awt.image.ColorModel = java.awt.image.ColorModel.rGBdefault
		Private Shared ReadOnly neededHints As Integer = (TOPDOWNLEFTRIGHT Or COMPLETESCANLINES)

		Private passthrough As Boolean
		Private reds As Single() : Private greens As Single() : Private blues As Single() : Private alphas As Single()
		Private savedy As Integer
		Private savedyrem As Integer

		''' <summary>
		''' Constructs an AreaAveragingScaleFilter that scales the pixels from
		''' its source Image as specified by the width and height parameters. </summary>
		''' <param name="width"> the target width to scale the image </param>
		''' <param name="height"> the target height to scale the image </param>
		Public Sub New(  width As Integer,   height As Integer)
			MyBase.New(width, height)
		End Sub

        ''' <summary>
        ''' Detect if the data is being delivered with the necessary hints
        ''' to allow the averaging algorithm to do its work.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code> whose
        ''' pixels are being filtered.  Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation. </summary>
        ''' <seealso cref= ImageConsumer#setHints </seealso>
        Public Overrides ReadOnly Property hints As Integer
            Set(  hints As Integer)
                passthrough = ((hints And neededHints) <> neededHints)
                MyBase.hints = hints
            End Set
        End Property

        Private Sub makeAccumBuffers()
			reds = New Single(destWidth - 1){}
			greens = New Single(destWidth - 1){}
			blues = New Single(destWidth - 1){}
			alphas = New Single(destWidth - 1){}
		End Sub

		Private Function calcRow() As Integer()
			Dim origmult As Single = (CSng(srcWidth)) * srcHeight
			If outpixbuf Is Nothing OrElse Not(TypeOf outpixbuf Is Integer()) Then outpixbuf = New Integer(destWidth - 1){}
			Dim outpix As Integer() = CType(outpixbuf, Integer())
			For x As Integer = 0 To destWidth - 1
				Dim mult As Single = origmult
				Dim a As Integer = System.Math.Round(alphas(x) / mult)
				If a <= 0 Then
					a = 0
				ElseIf a >= 255 Then
					a = 255
				Else
					' un-premultiply the components (by modifying mult here, we
					' are effectively doing the divide by mult and divide by
					' alpha in the same step)
					mult = alphas(x) / 255
				End If
				Dim r As Integer = System.Math.Round(reds(x) / mult)
				Dim g As Integer = System.Math.Round(greens(x) / mult)
				Dim b As Integer = System.Math.Round(blues(x) / mult)
				If r < 0 Then
					r = 0
				ElseIf r > 255 Then
					r = 255
				End If
				If g < 0 Then
					g = 0
				ElseIf g > 255 Then
					g = 255
				End If
				If b < 0 Then
					b = 0
				ElseIf b > 255 Then
					b = 255
				End If
				outpix(x) = (a << 24 Or r << 16 Or g << 8 Or b)
			Next x
			Return outpix
		End Function

		Private Sub accumPixels(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   model As java.awt.image.ColorModel,   pixels As Object,   [off] As Integer,   scansize As Integer)
			If reds Is Nothing Then makeAccumBuffers()
			Dim sy As Integer = y
			Dim syrem As Integer = destHeight
			Dim dy, dyrem As Integer
			If sy = 0 Then
				dy = 0
				dyrem = 0
			Else
				dy = savedy
				dyrem = savedyrem
			End If
			Do While sy < y + h
				Dim amty As Integer
				If dyrem = 0 Then
					For i As Integer = 0 To destWidth - 1
							blues(i) = 0f
								greens(i) = blues(i)
									reds(i) = greens(i)
									alphas(i) = reds(i)
					Next i
					dyrem = srcHeight
				End If
				If syrem < dyrem Then
					amty = syrem
				Else
					amty = dyrem
				End If
				Dim sx As Integer = 0
				Dim dx As Integer = 0
				Dim sxrem As Integer = 0
				Dim dxrem As Integer = srcWidth
				Dim a As Single = 0f, r As Single = 0f, g As Single = 0f, b As Single = 0f
				Do While sx < w
					If sxrem = 0 Then
						sxrem = destWidth
						Dim rgb As Integer
						If TypeOf pixels Is SByte() Then
							rgb = CType(pixels, SByte())([off] + sx) And &Hff
						Else
							rgb = CType(pixels, Integer())([off] + sx)
						End If
						' getRGB() always returns non-premultiplied components
						rgb = model.getRGB(rgb)
						a = CInt(CUInt(rgb) >> 24)
						r = (rgb >> 16) And &Hff
						g = (rgb >> 8) And &Hff
						b = rgb And &Hff
						' premultiply the components if necessary
						If a <> 255.0f Then
							Dim ascale As Single = a / 255.0f
							r *= ascale
							g *= ascale
							b *= ascale
						End If
					End If
					Dim amtx As Integer
					If sxrem < dxrem Then
						amtx = sxrem
					Else
						amtx = dxrem
					End If
					Dim mult As Single = (CSng(amtx)) * amty
					alphas(dx) += mult * a
					reds(dx) += mult * r
					greens(dx) += mult * g
					blues(dx) += mult * b
					sxrem -= amtx
					If sxrem = 0 Then sx += 1
					dxrem -= amtx
					If dxrem = 0 Then
						dx += 1
						dxrem = srcWidth
					End If
				Loop
				dyrem -= amty
				If dyrem = 0 Then
					Dim outpix As Integer() = calcRow()
					Do
						consumer.pixelsels(0, dy, destWidth, 1, rgbmodel, outpix, 0, destWidth)
						dy += 1
						syrem -= amty
					Loop While syrem >= amty AndAlso amty = srcHeight
				Else
					syrem -= amty
				End If
				If syrem = 0 Then
					syrem = destHeight
					sy += 1
					[off] += scansize
				End If
			Loop
			savedyrem = dyrem
			savedy = dy
		End Sub

        ''' <summary>
        ''' Combine the components for the delivered byte pixels into the
        ''' accumulation arrays and send on any averaged data for rows of
        ''' pixels that are complete.  If the correct hints were not
        ''' specified in the setHints call then relay the work to our
        ''' superclass which is capable of scaling pixels regardless of
        ''' the delivery hints.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code>
        ''' whose pixels are being filtered.  Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation. </summary>
        ''' <seealso cref= ReplicateScaleFilter </seealso>
        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Sub setPixels(x As Integer, y As Integer, w As Integer, h As Integer, model As java.awt.image.ColorModel, pixels() As Byte, off As Integer, scansize As Integer)
            If passthrough Then
                MyBase.pixelsels(x, y, w, h, model, pixels, off, scansize)
            Else
                accumPixels(x, y, w, h, model, pixels, off, scansize)
            End If
        End Sub

        ''' <summary>
        ''' Combine the components for the delivered int pixels into the
        ''' accumulation arrays and send on any averaged data for rows of
        ''' pixels that are complete.  If the correct hints were not
        ''' specified in the setHints call then relay the work to our
        ''' superclass which is capable of scaling pixels regardless of
        ''' the delivery hints.
        ''' <p>
        ''' Note: This method is intended to be called by the
        ''' <code>ImageProducer</code> of the <code>Image</code>
        ''' whose pixels are being filtered.  Developers using
        ''' this class to filter pixels from an image should avoid calling
        ''' this method directly since that operation could interfere
        ''' with the filtering operation. </summary>
        ''' <seealso cref= ReplicateScaleFilter </seealso>
        Public Sub pixelsels(x As Integer, y As Integer, w As Integer, h As Integer, model As java.awt.image.ColorModel, pixels() As Integer, off As Integer, scansize As Integer)
            If passthrough Then
                MyBase.pixelsels(x, y, w, h, model, pixels, off, scansize)
            Else
                accumPixels(x, y, w, h, model, pixels, off, scansize)
            End If
        End Sub
    End Class

End Namespace