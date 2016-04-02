Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The PixelGrabber class implements an ImageConsumer which can be attached
	''' to an Image or ImageProducer object to retrieve a subset of the pixels
	''' in that image.  Here is an example:
	''' <pre>{@code
	''' 
	''' public  Sub  handlesinglepixel(int x, int y, int pixel) {
	'''      int alpha = (pixel >> 24) & 0xff;
	'''      int red   = (pixel >> 16) & 0xff;
	'''      int green = (pixel >>  8) & 0xff;
	'''      int blue  = (pixel      ) & 0xff;
	'''      // Deal with the pixel as necessary...
	''' }
	''' 
	''' public  Sub  handlepixels(Image img, int x, int y, int w, int h) {
	'''      int[] pixels = new int[w * h];
	'''      PixelGrabber pg = new PixelGrabber(img, x, y, w, h, pixels, 0, w);
	'''      try {
	'''          pg.grabPixels();
	'''      } catch (InterruptedException e) {
	'''          System.err.println("interrupted waiting for pixels!");
	'''          return;
	'''      }
	'''      if ((pg.getStatus() & ImageObserver.ABORT) != 0) {
	'''          System.err.println("image fetch aborted or errored");
	'''          return;
	'''      }
	'''      for (int j = 0; j < h; j++) {
	'''          for (int i = 0; i < w; i++) {
	'''              handlesinglepixel(x+i, y+j, pixels[j * w + i]);
	'''          }
	'''      }
	''' }
	''' 
	''' }</pre>
	''' </summary>
	''' <seealso cref= ColorModel#getRGBdefault
	''' 
	''' @author      Jim Graham </seealso>
	Public Class PixelGrabber
		Implements java.awt.image.ImageConsumer

		Friend producer As java.awt.image.ImageProducer

		Friend dstX As Integer
		Friend dstY As Integer
		Friend dstW As Integer
		Friend dstH As Integer

		Friend imageModel As java.awt.image.ColorModel
		Friend bytePixels As SByte()
		Friend intPixels As Integer()
		Friend dstOff As Integer
		Friend dstScan As Integer

		Private grabbing As Boolean
		Private flags As Integer

		Private Shared ReadOnly GRABBEDBITS As Integer = (ImageObserver.FRAMEBITS Or ImageObserver.ALLBITS)
		Private Shared ReadOnly DONEBITS As Integer = (GRABBEDBITS Or ImageObserver.ERROR)

		''' <summary>
		''' Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		''' section of pixels from the specified image into the given array.
		''' The pixels are stored into the array in the default RGB ColorModel.
		''' The RGB data for pixel (i, j) where (i, j) is inside the rectangle
		''' (x, y, w, h) is stored in the array at
		''' <tt>pix[(j - y) * scansize + (i - x) + off]</tt>. </summary>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <param name="img"> the image to retrieve pixels from </param>
		''' <param name="x"> the x coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image, relative to the default
		''' (unscaled) size of the image </param>
		''' <param name="y"> the y coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image </param>
		''' <param name="w"> the width of the rectangle of pixels to retrieve </param>
		''' <param name="h"> the height of the rectangle of pixels to retrieve </param>
		''' <param name="pix"> the array of integers which are to be used to hold the
		''' RGB pixels retrieved from the image </param>
		''' <param name="off"> the offset into the array of where to store the first pixel </param>
		''' <param name="scansize"> the distance from one row of pixels to the next in
		''' the array </param>
		Public Sub New(ByVal img As java.awt.Image, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pix As Integer(), ByVal [off] As Integer, ByVal scansize As Integer)
			Me.New(img.source, x, y, w, h, pix, [off], scansize)
		End Sub

		''' <summary>
		''' Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		''' section of pixels from the image produced by the specified
		''' ImageProducer into the given array.
		''' The pixels are stored into the array in the default RGB ColorModel.
		''' The RGB data for pixel (i, j) where (i, j) is inside the rectangle
		''' (x, y, w, h) is stored in the array at
		''' <tt>pix[(j - y) * scansize + (i - x) + off]</tt>. </summary>
		''' <param name="ip"> the <code>ImageProducer</code> that produces the
		''' image from which to retrieve pixels </param>
		''' <param name="x"> the x coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image, relative to the default
		''' (unscaled) size of the image </param>
		''' <param name="y"> the y coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image </param>
		''' <param name="w"> the width of the rectangle of pixels to retrieve </param>
		''' <param name="h"> the height of the rectangle of pixels to retrieve </param>
		''' <param name="pix"> the array of integers which are to be used to hold the
		''' RGB pixels retrieved from the image </param>
		''' <param name="off"> the offset into the array of where to store the first pixel </param>
		''' <param name="scansize"> the distance from one row of pixels to the next in
		''' the array </param>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		Public Sub New(ByVal ip As java.awt.image.ImageProducer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pix As Integer(), ByVal [off] As Integer, ByVal scansize As Integer)
			producer = ip
			dstX = x
			dstY = y
			dstW = w
			dstH = h
			dstOff = [off]
			dstScan = scansize
			intPixels = pix
			imageModel = java.awt.image.ColorModel.rGBdefault
		End Sub

		''' <summary>
		''' Create a PixelGrabber object to grab the (x, y, w, h) rectangular
		''' section of pixels from the specified image.  The pixels are
		''' accumulated in the original ColorModel if the same ColorModel
		''' is used for every call to setPixels, otherwise the pixels are
		''' accumulated in the default RGB ColorModel.  If the forceRGB
		''' parameter is true, then the pixels will be accumulated in the
		''' default RGB ColorModel anyway.  A buffer is allocated by the
		''' PixelGrabber to hold the pixels in either case.  If {@code (w < 0)} or
		''' {@code (h < 0)}, then they will default to the remaining width and
		''' height of the source data when that information is delivered. </summary>
		''' <param name="img"> the image to retrieve the image data from </param>
		''' <param name="x"> the x coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image, relative to the default
		''' (unscaled) size of the image </param>
		''' <param name="y"> the y coordinate of the upper left corner of the rectangle
		''' of pixels to retrieve from the image </param>
		''' <param name="w"> the width of the rectangle of pixels to retrieve </param>
		''' <param name="h"> the height of the rectangle of pixels to retrieve </param>
		''' <param name="forceRGB"> true if the pixels should always be converted to
		''' the default RGB ColorModel </param>
		Public Sub New(ByVal img As java.awt.Image, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal forceRGB As Boolean)
			producer = img.source
			dstX = x
			dstY = y
			dstW = w
			dstH = h
			If forceRGB Then imageModel = java.awt.image.ColorModel.rGBdefault
		End Sub

		''' <summary>
		''' Request the PixelGrabber to start fetching the pixels.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub startGrabbing()
			If (flags And DONEBITS) <> 0 Then Return
			If Not grabbing Then
				grabbing = True
				flags = flags And Not(ImageObserver.ABORT)
				producer.startProduction(Me)
			End If
		End Sub

		''' <summary>
		''' Request the PixelGrabber to abort the image fetch.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub abortGrabbing()
			imageComplete(IMAGEABORTED)
		End Sub

		''' <summary>
		''' Request the Image or ImageProducer to start delivering pixels and
		''' wait for all of the pixels in the rectangle of interest to be
		''' delivered. </summary>
		''' <returns> true if the pixels were successfully grabbed, false on
		''' abort, error or timeout </returns>
		''' <exception cref="InterruptedException">
		'''            Another thread has interrupted this thread. </exception>
		Public Overridable Function grabPixels() As Boolean
			Return grabPixels(0)
		End Function

		''' <summary>
		''' Request the Image or ImageProducer to start delivering pixels and
		''' wait for all of the pixels in the rectangle of interest to be
		''' delivered or until the specified timeout has elapsed.  This method
		''' behaves in the following ways, depending on the value of
		''' <code>ms</code>:
		''' <ul>
		''' <li> If {@code ms == 0}, waits until all pixels are delivered
		''' <li> If {@code ms > 0}, waits until all pixels are delivered
		''' as timeout expires.
		''' <li> If {@code ms < 0}, returns <code>true</code> if all pixels
		''' are grabbed, <code>false</code> otherwise and does not wait.
		''' </ul> </summary>
		''' <param name="ms"> the number of milliseconds to wait for the image pixels
		''' to arrive before timing out </param>
		''' <returns> true if the pixels were successfully grabbed, false on
		''' abort, error or timeout </returns>
		''' <exception cref="InterruptedException">
		'''            Another thread has interrupted this thread. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function grabPixels(ByVal ms As Long) As Boolean
			If (flags And DONEBITS) <> 0 Then Return (flags And GRABBEDBITS) <> 0
			Dim [end] As Long = ms + System.currentTimeMillis()
			If Not grabbing Then
				grabbing = True
				flags = flags And Not(ImageObserver.ABORT)
				producer.startProduction(Me)
			End If
			Do While grabbing
				Dim timeout As Long
				If ms = 0 Then
					timeout = 0
				Else
					timeout = [end] - System.currentTimeMillis()
					If timeout <= 0 Then Exit Do
				End If
				wait(timeout)
			Loop
			Return (flags And GRABBEDBITS) <> 0
		End Function

		''' <summary>
		''' Return the status of the pixels.  The ImageObserver flags
		''' representing the available pixel information are returned. </summary>
		''' <returns> the bitwise OR of all relevant ImageObserver flags </returns>
		''' <seealso cref= ImageObserver </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property status As Integer
			Get
				Return flags
			End Get
		End Property

		''' <summary>
		''' Get the width of the pixel buffer (after adjusting for image width).
		''' If no width was specified for the rectangle of pixels to grab then
		''' then this information will only be available after the image has
		''' delivered the dimensions. </summary>
		''' <returns> the final width used for the pixel buffer or -1 if the width
		''' is not yet known </returns>
		''' <seealso cref= #getStatus </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property width As Integer
			Get
				Return If(dstW < 0, -1, dstW)
			End Get
		End Property

		''' <summary>
		''' Get the height of the pixel buffer (after adjusting for image height).
		''' If no width was specified for the rectangle of pixels to grab then
		''' then this information will only be available after the image has
		''' delivered the dimensions. </summary>
		''' <returns> the final height used for the pixel buffer or -1 if the height
		''' is not yet known </returns>
		''' <seealso cref= #getStatus </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property height As Integer
			Get
				Return If(dstH < 0, -1, dstH)
			End Get
		End Property

		''' <summary>
		''' Get the pixel buffer.  If the PixelGrabber was not constructed
		''' with an explicit pixel buffer to hold the pixels then this method
		''' will return null until the size and format of the image data is
		''' known.
		''' Since the PixelGrabber may fall back on accumulating the data
		''' in the default RGB ColorModel at any time if the source image
		''' uses more than one ColorModel to deliver the data, the array
		''' object returned by this method may change over time until the
		''' image grab is complete. </summary>
		''' <returns> either a byte array or an int array </returns>
		''' <seealso cref= #getStatus </seealso>
		''' <seealso cref= #setPixels(int, int, int, int, ColorModel, byte[], int, int) </seealso>
		''' <seealso cref= #setPixels(int, int, int, int, ColorModel, int[], int, int) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property pixels As Object
			Get
				Return If(bytePixels Is Nothing, (CObj(intPixels)), (CObj(bytePixels)))
			End Get
		End Property

		''' <summary>
		''' Get the ColorModel for the pixels stored in the array.  If the
		''' PixelGrabber was constructed with an explicit pixel buffer then
		''' this method will always return the default RGB ColorModel,
		''' otherwise it may return null until the ColorModel used by the
		''' ImageProducer is known.
		''' Since the PixelGrabber may fall back on accumulating the data
		''' in the default RGB ColorModel at any time if the source image
		''' uses more than one ColorModel to deliver the data, the ColorModel
		''' object returned by this method may change over time until the
		''' image grab is complete and may not reflect any of the ColorModel
		''' objects that was used by the ImageProducer to deliver the pixels. </summary>
		''' <returns> the ColorModel object used for storing the pixels </returns>
		''' <seealso cref= #getStatus </seealso>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #setColorModel(ColorModel) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getColorModel() As java.awt.image.ColorModel 'JavaToDotNetTempPropertyGetcolorModel
		Public Overridable Property colorModel As java.awt.image.ColorModel
			Get
				Return imageModel
			End Get
			Set(ByVal model As java.awt.image.ColorModel)
		End Property

		''' <summary>
		''' The setDimensions method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="width"> the width of the dimension </param>
		''' <param name="height"> the height of the dimension </param>
		Public Overridable Sub setDimensions(ByVal width As Integer, ByVal height As Integer)
			If dstW < 0 Then dstW = width - dstX
			If dstH < 0 Then dstH = height - dstY
			If dstW <= 0 OrElse dstH <= 0 Then
				imageComplete(STATICIMAGEDONE)
			ElseIf intPixels Is Nothing AndAlso imageModel Is java.awt.image.ColorModel.rGBdefault Then
				intPixels = New Integer(dstW * dstH - 1){}
				dstScan = dstW
				dstOff = 0
			End If
			flags = flags Or (ImageObserver.WIDTH Or ImageObserver.HEIGHT)
		End Sub

		''' <summary>
		''' The setHints method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="hints"> a set of hints used to process the pixels </param>
		Public Overridable Property hints As Integer
			Set(ByVal hints As Integer)
				Return
			End Set
		End Property

		''' <summary>
		''' The setProperties method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="props"> the list of properties </param>
		Public Overridable Property properties(Of T1) As Dictionary(Of T1)
			Set(ByVal props As Dictionary(Of T1))
				Return
			End Set
		End Property

			Return
		End Sub

		Private Sub convertToRGB()
			Dim size As Integer = dstW * dstH
			Dim newpixels As Integer() = New Integer(size - 1){}
			If bytePixels IsNot Nothing Then
				For i As Integer = 0 To size - 1
					newpixels(i) = imageModel.getRGB(bytePixels(i) And &Hff)
				Next i
			ElseIf intPixels IsNot Nothing Then
				For i As Integer = 0 To size - 1
					newpixels(i) = imageModel.getRGB(intPixels(i))
				Next i
			End If
			bytePixels = Nothing
			intPixels = newpixels
			dstScan = dstW
			dstOff = 0
			imageModel = java.awt.image.ColorModel.rGBdefault
		End Sub

		''' <summary>
		''' The setPixels method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="srcX"> the X coordinate of the upper-left corner
		'''        of the area of pixels to be set </param>
		''' <param name="srcY"> the Y coordinate of the upper-left corner
		'''        of the area of pixels to be set </param>
		''' <param name="srcW"> the width of the area of pixels </param>
		''' <param name="srcH"> the height of the area of pixels </param>
		''' <param name="model"> the specified <code>ColorModel</code> </param>
		''' <param name="pixels"> the array of pixels </param>
		''' <param name="srcOff"> the offset into the pixels array </param>
		''' <param name="srcScan"> the distance from one row of pixels to the next
		'''        in the pixels array </param>
		''' <seealso cref= #getPixels </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  setPixels(int srcX, int srcY, int srcW, int srcH, java.awt.image.ColorModel model, byte pixels() , int srcOff, int srcScan)
			If srcY < dstY Then
				Dim diff As Integer = dstY - srcY
				If diff >= srcH Then Return
				srcOff += srcScan * diff
				srcY += diff
				srcH -= diff
			End If
			If srcY + srcH > dstY + dstH Then
				srcH = (dstY + dstH) - srcY
				If srcH <= 0 Then Return
			End If
			If srcX < dstX Then
				Dim diff As Integer = dstX - srcX
				If diff >= srcW Then Return
				srcOff += diff
				srcX += diff
				srcW -= diff
			End If
			If srcX + srcW > dstX + dstW Then
				srcW = (dstX + dstW) - srcX
				If srcW <= 0 Then Return
			End If
			Dim dstPtr As Integer = dstOff + (srcY - dstY) * dstScan + (srcX - dstX)
			If intPixels Is Nothing Then
				If bytePixels Is Nothing Then
					bytePixels = New SByte(dstW * dstH - 1){}
					dstScan = dstW
					dstOff = 0
					imageModel = model
				ElseIf imageModel IsNot model Then
					convertToRGB()
				End If
				If bytePixels IsNot Nothing Then
					For h As Integer = srcH To 1 Step -1
						Array.Copy(pixels, srcOff, bytePixels, dstPtr, srcW)
						srcOff += srcScan
						dstPtr += dstScan
					Next h
				End If
			End If
			If intPixels IsNot Nothing Then
				Dim dstRem As Integer = dstScan - srcW
				Dim srcRem As Integer = srcScan - srcW
				For h As Integer = srcH To 1 Step -1
					For w As Integer = srcW To 1 Step -1
						intPixels(dstPtr) = model.getRGB(pixels(srcOff) And &Hff)
						srcOff += 1
						dstPtr += 1
					Next w
					srcOff += srcRem
					dstPtr += dstRem
				Next h
			End If
			flags = flags Or ImageObserver.SOMEBITS

		''' <summary>
		''' The setPixels method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="srcX"> the X coordinate of the upper-left corner
		'''        of the area of pixels to be set </param>
		''' <param name="srcY"> the Y coordinate of the upper-left corner
		'''        of the area of pixels to be set </param>
		''' <param name="srcW"> the width of the area of pixels </param>
		''' <param name="srcH"> the height of the area of pixels </param>
		''' <param name="model"> the specified <code>ColorModel</code> </param>
		''' <param name="pixels"> the array of pixels </param>
		''' <param name="srcOff"> the offset into the pixels array </param>
		''' <param name="srcScan"> the distance from one row of pixels to the next
		'''        in the pixels array </param>
		''' <seealso cref= #getPixels </seealso>
		public  Sub  pixelsels(Integer srcX, Integer srcY, Integer srcW, Integer srcH, java.awt.image.ColorModel model, Integer pixels() , Integer srcOff, Integer srcScan)
			If srcY < dstY Then
				Dim diff As Integer = dstY - srcY
				If diff >= srcH Then Return
				srcOff += srcScan * diff
				srcY += diff
				srcH -= diff
			End If
			If srcY + srcH > dstY + dstH Then
				srcH = (dstY + dstH) - srcY
				If srcH <= 0 Then Return
			End If
			If srcX < dstX Then
				Dim diff As Integer = dstX - srcX
				If diff >= srcW Then Return
				srcOff += diff
				srcX += diff
				srcW -= diff
			End If
			If srcX + srcW > dstX + dstW Then
				srcW = (dstX + dstW) - srcX
				If srcW <= 0 Then Return
			End If
			If intPixels Is Nothing Then
				If bytePixels Is Nothing Then
					intPixels = New Integer(dstW * dstH - 1){}
					dstScan = dstW
					dstOff = 0
					imageModel = model
				Else
					convertToRGB()
				End If
			End If
			Dim dstPtr As Integer = dstOff + (srcY - dstY) * dstScan + (srcX - dstX)
			If imageModel Is model Then
				For h As Integer = srcH To 1 Step -1
					Array.Copy(pixels, srcOff, intPixels, dstPtr, srcW)
					srcOff += srcScan
					dstPtr += dstScan
				Next h
			Else
				If imageModel IsNot java.awt.image.ColorModel.rGBdefault Then convertToRGB()
				Dim dstRem As Integer = dstScan - srcW
				Dim srcRem As Integer = srcScan - srcW
				For h As Integer = srcH To 1 Step -1
					For w As Integer = srcW To 1 Step -1
						intPixels(dstPtr) = model.getRGB(pixels(srcOff))
						srcOff += 1
						dstPtr += 1
					Next w
					srcOff += srcRem
					dstPtr += dstRem
				Next h
			End If
			flags = flags Or ImageObserver.SOMEBITS

		''' <summary>
		''' The imageComplete method is part of the ImageConsumer API which
		''' this class must implement to retrieve the pixels.
		''' <p>
		''' Note: This method is intended to be called by the ImageProducer
		''' of the Image whose pixels are being grabbed.  Developers using
		''' this class to retrieve pixels from an image should avoid calling
		''' this method directly since that operation could result in problems
		''' with retrieving the requested pixels. </summary>
		''' <param name="status"> the status of image loading </param>
		Public   Sub  imageComplete(Integer status)
			grabbing = False
			Select Case status
			Case Else
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case IMAGEERROR
				flags = flags Or ImageObserver.ERROR Or ImageObserver.ABORT
			Case IMAGEABORTED
				flags = flags Or ImageObserver.ABORT
			Case STATICIMAGEDONE
				flags = flags Or ImageObserver.ALLBITS
			Case SINGLEFRAMEDONE
				flags = flags Or ImageObserver.FRAMEBITS
			End Select
			producer.removeConsumer(Me)
			notifyAll()

		''' <summary>
		''' Returns the status of the pixels.  The ImageObserver flags
		''' representing the available pixel information are returned.
		''' This method and <seealso cref="#getStatus() getStatus"/> have the
		''' same implementation, but <code>getStatus</code> is the
		''' preferred method because it conforms to the convention of
		''' naming information-retrieval methods with the form
		''' "getXXX". </summary>
		''' <returns> the bitwise OR of all relevant ImageObserver flags </returns>
		''' <seealso cref= ImageObserver </seealso>
		''' <seealso cref= #getStatus() </seealso>
		Public  Integer status()
			Return flags
	End Class

End Namespace