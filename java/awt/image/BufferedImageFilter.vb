Imports System

'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>BufferedImageFilter</code> class subclasses an
	''' <code>ImageFilter</code> to provide a simple means of
	''' using a single-source/single-destination image operator
	''' (<seealso cref="BufferedImageOp"/>) to filter a <code>BufferedImage</code>
	''' in the Image Producer/Consumer/Observer
	''' paradigm. Examples of these image operators are: <seealso cref="ConvolveOp"/>,
	''' <seealso cref="AffineTransformOp"/> and <seealso cref="LookupOp"/>.
	''' </summary>
	''' <seealso cref= ImageFilter </seealso>
	''' <seealso cref= BufferedImage </seealso>
	''' <seealso cref= BufferedImageOp </seealso>

	Public Class BufferedImageFilter
		Inherits java.awt.image.ImageFilter
		Implements Cloneable

		Friend bufferedImageOp As BufferedImageOp
		Friend model As ColorModel
		Friend width As Integer
		Friend height As Integer
		Friend bytePixels As SByte()
		Friend intPixels As Integer()

		''' <summary>
		''' Constructs a <code>BufferedImageFilter</code> with the
		''' specified single-source/single-destination operator. </summary>
		''' <param name="op"> the specified <code>BufferedImageOp</code> to
		'''           use to filter a <code>BufferedImage</code> </param>
		''' <exception cref="NullPointerException"> if op is null </exception>
		Public Sub New(ByVal op As BufferedImageOp)
			MyBase.New()
			If op Is Nothing Then Throw New NullPointerException("Operation cannot be null")
			bufferedImageOp = op
		End Sub

		''' <summary>
		''' Returns the <code>BufferedImageOp</code>. </summary>
		''' <returns> the operator of this <code>BufferedImageFilter</code>. </returns>
		Public Overridable Property bufferedImageOp As BufferedImageOp
			Get
				Return bufferedImageOp
			End Get
		End Property

		''' <summary>
		''' Filters the information provided in the
		''' <seealso cref="ImageConsumer#setDimensions(int, int) setDimensions "/> method
		''' of the <seealso cref="ImageConsumer"/> interface.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <seealso cref="ImageProducer"/> of the <code>Image</code> whose pixels are
		''' being filtered. Developers using this class to retrieve pixels from
		''' an image should avoid calling this method directly since that
		''' operation could result in problems with retrieving the requested
		''' pixels.
		''' <p> </summary>
		''' <param name="width"> the width to which to set the width of this
		'''        <code>BufferedImageFilter</code> </param>
		''' <param name="height"> the height to which to set the height of this
		'''        <code>BufferedImageFilter</code> </param>
		''' <seealso cref= ImageConsumer#setDimensions </seealso>
		Public Overrides Sub setDimensions(ByVal width As Integer, ByVal height As Integer)
			If width <= 0 OrElse height <= 0 Then
				imageComplete(STATICIMAGEDONE)
				Return
			End If
			Me.width = width
			Me.height = height
		End Sub

		''' <summary>
		''' Filters the information provided in the
		''' <seealso cref="ImageConsumer#setColorModel(ColorModel) setColorModel"/> method
		''' of the <code>ImageConsumer</code> interface.
		''' <p>
		''' If <code>model</code> is <code>null</code>, this
		''' method clears the current <code>ColorModel</code> of this
		''' <code>BufferedImageFilter</code>.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code>
		''' whose pixels are being filtered.  Developers using this
		''' class to retrieve pixels from an image
		''' should avoid calling this method directly since that
		''' operation could result in problems with retrieving the
		''' requested pixels. </summary>
		''' <param name="model"> the <seealso cref="ColorModel"/> to which to set the
		'''        <code>ColorModel</code> of this <code>BufferedImageFilter</code> </param>
		''' <seealso cref= ImageConsumer#setColorModel </seealso>
		Public Overrides Property colorModel As ColorModel
			Set(ByVal model As ColorModel)
				Me.model = model
			End Set
		End Property

		Private Sub convertToRGB()
			Dim size As Integer = width * height
			Dim newpixels As Integer() = New Integer(size - 1){}
			If bytePixels IsNot Nothing Then
				For i As Integer = 0 To size - 1
					newpixels(i) = Me.model.getRGB(bytePixels(i) And &Hff)
				Next i
			ElseIf intPixels IsNot Nothing Then
				For i As Integer = 0 To size - 1
					newpixels(i) = Me.model.getRGB(intPixels(i))
				Next i
			End If
			bytePixels = Nothing
			intPixels = newpixels
			Me.model = ColorModel.rGBdefault
		End Sub

		''' <summary>
		''' Filters the information provided in the <code>setPixels</code>
		''' method of the <code>ImageConsumer</code> interface which takes
		''' an array of bytes.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <exception cref="IllegalArgumentException"> if width or height are less than
		''' zero. </exception>
		''' <seealso cref= ImageConsumer#setPixels(int, int, int, int, ColorModel, byte[],
		'''                                int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void setPixels(int x, int y, int w, int h, ColorModel model, byte pixels() , int off, int scansize)
			' Fix 4184230
			If w < 0 OrElse h < 0 Then Throw New IllegalArgumentException("Width (" & w & ") and height (" & h & ") must be > 0")
			' Nothing to do
			If w = 0 OrElse h = 0 Then Return
			If y < 0 Then
				Dim diff As Integer = -y
				If diff >= h Then Return
				off += scansize * diff
				y += diff
				h -= diff
			End If
			If y + h > height Then
				h = height - y
				If h <= 0 Then Return
			End If
			If x < 0 Then
				Dim diff As Integer = -x
				If diff >= w Then Return
				off += diff
				x += diff
				w -= diff
			End If
			If x + w > width Then
				w = width - x
				If w <= 0 Then Return
			End If
			Dim dstPtr As Integer = y*width + x
			If intPixels Is Nothing Then
				If bytePixels Is Nothing Then
					bytePixels = New SByte(width*height - 1){}
					Me.model = model
				ElseIf Me.model IsNot model Then
					convertToRGB()
				End If
				If bytePixels IsNot Nothing Then
					For sh As Integer = h To 1 Step -1
						Array.Copy(pixels, off, bytePixels, dstPtr, w)
						off += scansize
						dstPtr += width
					Next sh
				End If
			End If
			If intPixels IsNot Nothing Then
				Dim dstRem As Integer = width - w
				Dim srcRem As Integer = scansize - w
				For sh As Integer = h To 1 Step -1
					For sw As Integer = w To 1 Step -1
						intPixels(dstPtr) = model.getRGB(pixels(off) And &Hff)
						off += 1
						dstPtr += 1
					Next sw
					off += srcRem
					dstPtr += dstRem
				Next sh
			End If
		''' <summary>
		''' Filters the information provided in the <code>setPixels</code>
		''' method of the <code>ImageConsumer</code> interface which takes
		''' an array of integers.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose
		''' pixels are being filtered.  Developers using this class to
		''' retrieve pixels from an image should avoid calling this method
		''' directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <exception cref="IllegalArgumentException"> if width or height are less than
		''' zero. </exception>
		''' <seealso cref= ImageConsumer#setPixels(int, int, int, int, ColorModel, int[],
		'''                                int, int) </seealso>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, ColorModel model, Integer pixels() , Integer off, Integer scansize)
			' Fix 4184230
			If w < 0 OrElse h < 0 Then Throw New IllegalArgumentException("Width (" & w & ") and height (" & h & ") must be > 0")
			' Nothing to do
			If w = 0 OrElse h = 0 Then Return
			If y < 0 Then
				Dim diff As Integer = -y
				If diff >= h Then Return
				off += scansize * diff
				y += diff
				h -= diff
			End If
			If y + h > height Then
				h = height - y
				If h <= 0 Then Return
			End If
			If x < 0 Then
				Dim diff As Integer = -x
				If diff >= w Then Return
				off += diff
				x += diff
				w -= diff
			End If
			If x + w > width Then
				w = width - x
				If w <= 0 Then Return
			End If

			If intPixels Is Nothing Then
				If bytePixels Is Nothing Then
					intPixels = New Integer(width * height - 1){}
					Me.model = model
				Else
					convertToRGB()
				End If
			End If
			Dim dstPtr As Integer = y*width + x
			If Me.model Is model Then
				For sh As Integer = h To 1 Step -1
					Array.Copy(pixels, off, intPixels, dstPtr, w)
					off += scansize
					dstPtr += width
				Next sh
			Else
				If Me.model IsNot ColorModel.rGBdefault Then convertToRGB()
				Dim dstRem As Integer = width - w
				Dim srcRem As Integer = scansize - w
				For sh As Integer = h To 1 Step -1
					For sw As Integer = w To 1 Step -1
						intPixels(dstPtr) = model.getRGB(pixels(off))
						off += 1
						dstPtr += 1
					Next sw
					off += srcRem
					dstPtr += dstRem
				Next sh
			End If

		''' <summary>
		''' Filters the information provided in the <code>imageComplete</code>
		''' method of the <code>ImageConsumer</code> interface.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="status"> the status of image loading </param>
		''' <exception cref="ImagingOpException"> if there was a problem calling the filter
		''' method of the <code>BufferedImageOp</code> associated with this
		''' instance. </exception>
		''' <seealso cref= ImageConsumer#imageComplete </seealso>
		public void imageComplete(Integer status)
			Dim wr As WritableRaster
			Select Case status
			Case IMAGEERROR, IMAGEABORTED
				' reinitialize the params
				model = Nothing
				width = -1
				height = -1
				intPixels = Nothing
				bytePixels = Nothing

			Case SINGLEFRAMEDONE, STATICIMAGEDONE
				If width <= 0 OrElse height <= 0 Then Exit Select
				If TypeOf model Is DirectColorModel Then
					If intPixels Is Nothing Then Exit Select
					wr = createDCMraster()
				ElseIf TypeOf model Is IndexColorModel Then
					Dim bandOffsets As Integer() = {0}
					If bytePixels Is Nothing Then Exit Select
					Dim db As New DataBufferByte(bytePixels, width*height)
					wr = Raster.createInterleavedRaster(db, width, height, width, 1, bandOffsets, Nothing)
				Else
					convertToRGB()
					If intPixels Is Nothing Then Exit Select
					wr = createDCMraster()
				End If
				Dim bi As New BufferedImage(model, wr, model.alphaPremultiplied, Nothing)
				bi = bufferedImageOp.filter(bi, Nothing)
				Dim r As WritableRaster = bi.raster
				Dim cm As ColorModel = bi.colorModel
				Dim w As Integer = r.width
				Dim h As Integer = r.height
				consumer.dimensionsons(w, h)
				consumer.colorModel = cm
				If TypeOf cm Is DirectColorModel Then
					Dim db As DataBufferInt = CType(r.dataBuffer, DataBufferInt)
					consumer.pixelsels(0, 0, w, h, cm, db.data, 0, w)
				ElseIf TypeOf cm Is IndexColorModel Then
					Dim db As DataBufferByte = CType(r.dataBuffer, DataBufferByte)
					consumer.pixelsels(0, 0, w, h, cm, db.data, 0, w)
				Else
					Throw New InternalError("Unknown color model " & cm)
				End If
			End Select
			consumer.imageComplete(status)

		private final WritableRaster createDCMraster()
			Dim wr As WritableRaster
			Dim dcm As DirectColorModel = CType(model, DirectColorModel)
			Dim hasAlpha As Boolean = model.hasAlpha()
			Dim bandMasks As Integer() = New Integer(3+(If(hasAlpha, 1, 0)) - 1){}
			bandMasks(0) = dcm.redMask
			bandMasks(1) = dcm.greenMask
			bandMasks(2) = dcm.blueMask
			If hasAlpha Then bandMasks(3) = dcm.alphaMask
			Dim db As New DataBufferInt(intPixels, width*height)
			wr = Raster.createPackedRaster(db, width, height, width, bandMasks, Nothing)
			Return wr

	End Class

End Namespace