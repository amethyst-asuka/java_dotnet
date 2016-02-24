Imports System
Imports System.Collections
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
	''' This class is an implementation of the ImageProducer interface which
	''' uses an array to produce pixel values for an Image.  Here is an example
	''' which calculates a 100x100 image representing a fade from black to blue
	''' along the X axis and a fade from black to red along the Y axis:
	''' <pre>{@code
	''' 
	'''      int w = 100;
	'''      int h = 100;
	'''      int pix[] = new int[w * h];
	'''      int index = 0;
	'''      for (int y = 0; y < h; y++) {
	'''          int red = (y * 255) / (h - 1);
	'''          for (int x = 0; x < w; x++) {
	'''              int blue = (x * 255) / (w - 1);
	'''              pix[index++] = (255 << 24) | (red << 16) | blue;
	'''          }
	'''      }
	'''      Image img = createImage(new MemoryImageSource(w, h, pix, 0, w));
	''' 
	''' }</pre>
	''' The MemoryImageSource is also capable of managing a memory image which
	''' varies over time to allow animation or custom rendering.  Here is an
	''' example showing how to set up the animation source and signal changes
	''' in the data (adapted from the MemoryAnimationSourceDemo by Garth Dickie):
	''' <pre>{@code
	''' 
	'''      int pixels[];
	'''      MemoryImageSource source;
	''' 
	'''      public void init() {
	'''          int width = 50;
	'''          int height = 50;
	'''          int size = width * height;
	'''          pixels = new int[size];
	''' 
	'''          int value = getBackground().getRGB();
	'''          for (int i = 0; i < size; i++) {
	'''              pixels[i] = value;
	'''          }
	''' 
	'''          source = new MemoryImageSource(width, height, pixels, 0, width);
	'''          source.setAnimated(true);
	'''          image = createImage(source);
	'''      }
	''' 
	'''      public void run() {
	'''          Thread me = Thread.currentThread( );
	'''          me.setPriority(Thread.MIN_PRIORITY);
	''' 
	'''          while (true) {
	'''              try {
	'''                  Thread.sleep(10);
	'''              } catch( InterruptedException e ) {
	'''                  return;
	'''              }
	''' 
	'''              // Modify the values in the pixels array at (x, y, w, h)
	''' 
	'''              // Send the new data to the interested ImageConsumers
	'''              source.newPixels(x, y, w, h);
	'''          }
	'''      }
	''' 
	''' }</pre>
	''' </summary>
	''' <seealso cref= ImageProducer
	''' 
	''' @author      Jim Graham
	''' @author      Animation capabilities inspired by the
	'''              MemoryAnimationSource class written by Garth Dickie </seealso>
	Public Class MemoryImageSource
		Implements java.awt.image.ImageProducer

		Friend width As Integer
		Friend height As Integer
		Friend model As java.awt.image.ColorModel
		Friend pixels As Object
		Friend pixeloffset As Integer
		Friend pixelscan As Integer
		Friend properties As Hashtable
		Friend theConsumers As New ArrayList
		Friend animating As Boolean
		Friend fullbuffers As Boolean

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of bytes
		''' to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="cm"> the specified <code>ColorModel</code> </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		Public Sub New(ByVal w As Integer, ByVal h As Integer, ByVal cm As java.awt.image.ColorModel, ByVal pix As SByte(), ByVal [off] As Integer, ByVal scan As Integer)
			initialize(w, h, cm, CObj(pix), [off], scan, Nothing)
		End Sub

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of bytes
		''' to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="cm"> the specified <code>ColorModel</code> </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <param name="props"> a list of properties that the <code>ImageProducer</code>
		'''        uses to process an image </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		Public Sub New(Of T1)(ByVal w As Integer, ByVal h As Integer, ByVal cm As java.awt.image.ColorModel, ByVal pix As SByte(), ByVal [off] As Integer, ByVal scan As Integer, ByVal props As Dictionary(Of T1))
			initialize(w, h, cm, CObj(pix), [off], scan, props)
		End Sub

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of integers
		''' to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="cm"> the specified <code>ColorModel</code> </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		Public Sub New(ByVal w As Integer, ByVal h As Integer, ByVal cm As java.awt.image.ColorModel, ByVal pix As Integer(), ByVal [off] As Integer, ByVal scan As Integer)
			initialize(w, h, cm, CObj(pix), [off], scan, Nothing)
		End Sub

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of integers
		''' to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="cm"> the specified <code>ColorModel</code> </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <param name="props"> a list of properties that the <code>ImageProducer</code>
		'''        uses to process an image </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		Public Sub New(Of T1)(ByVal w As Integer, ByVal h As Integer, ByVal cm As java.awt.image.ColorModel, ByVal pix As Integer(), ByVal [off] As Integer, ByVal scan As Integer, ByVal props As Dictionary(Of T1))
			initialize(w, h, cm, CObj(pix), [off], scan, props)
		End Sub

		Private Sub initialize(ByVal w As Integer, ByVal h As Integer, ByVal cm As java.awt.image.ColorModel, ByVal pix As Object, ByVal [off] As Integer, ByVal scan As Integer, ByVal props As Hashtable)
			width = w
			height = h
			model = cm
			pixels = pix
			pixeloffset = [off]
			pixelscan = scan
			If props Is Nothing Then props = New Hashtable
			properties = props
		End Sub

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of integers
		''' in the default RGB ColorModel to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public MemoryImageSource(int w, int h, int pix() , int off, int scan)
			initialize(w, h, java.awt.image.ColorModel.rGBdefault, CObj(pix), off, scan, Nothing)

		''' <summary>
		''' Constructs an ImageProducer object which uses an array of integers
		''' in the default RGB ColorModel to produce data for an Image object. </summary>
		''' <param name="w"> the width of the rectangle of pixels </param>
		''' <param name="h"> the height of the rectangle of pixels </param>
		''' <param name="pix"> an array of pixels </param>
		''' <param name="off"> the offset into the array of where to store the
		'''        first pixel </param>
		''' <param name="scan"> the distance from one row of pixels to the next in
		'''        the array </param>
		''' <param name="props"> a list of properties that the <code>ImageProducer</code>
		'''        uses to process an image </param>
		''' <seealso cref= java.awt.Component#createImage </seealso>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public MemoryImageSource(Integer w, Integer h, Integer pix() , Integer off, Integer scan, Dictionary(Of ?, ?) props)
			initialize(w, h, java.awt.image.ColorModel.rGBdefault, CObj(pix), off, scan, props)

		''' <summary>
		''' Adds an ImageConsumer to the list of consumers interested in
		''' data for this image. </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code> </param>
		''' <exception cref="NullPointerException"> if the specified
		'''           <code>ImageConsumer</code> is null </exception>
		''' <seealso cref= ImageConsumer </seealso>
		public synchronized void addConsumer(java.awt.image.ImageConsumer ic)
			If theConsumers.Contains(ic) Then Return
			theConsumers.Add(ic)
			Try
				initConsumer(ic)
				sendPixels(ic, 0, 0, width, height)
				If isConsumer(ic) Then
					ic.imageComplete(If(animating, java.awt.image.ImageConsumer.SINGLEFRAMEDONE, java.awt.image.ImageConsumer.STATICIMAGEDONE))
					If (Not animating) AndAlso isConsumer(ic) Then
						ic.imageComplete(java.awt.image.ImageConsumer.IMAGEERROR)
						removeConsumer(ic)
					End If
				End If
			Catch e As Exception
				If isConsumer(ic) Then ic.imageComplete(java.awt.image.ImageConsumer.IMAGEERROR)
			End Try

		''' <summary>
		''' Determines if an ImageConsumer is on the list of consumers currently
		''' interested in data for this image. </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code> </param>
		''' <returns> <code>true</code> if the <code>ImageConsumer</code>
		''' is on the list; <code>false</code> otherwise. </returns>
		''' <seealso cref= ImageConsumer </seealso>
		public synchronized Boolean isConsumer(java.awt.image.ImageConsumer ic)
			Return theConsumers.Contains(ic)

		''' <summary>
		''' Removes an ImageConsumer from the list of consumers interested in
		''' data for this image. </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code> </param>
		''' <seealso cref= ImageConsumer </seealso>
		public synchronized void removeConsumer(java.awt.image.ImageConsumer ic)
			theConsumers.Remove(ic)

		''' <summary>
		''' Adds an ImageConsumer to the list of consumers interested in
		''' data for this image and immediately starts delivery of the
		''' image data through the ImageConsumer interface. </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code>
		''' image data through the ImageConsumer interface. </param>
		''' <seealso cref= ImageConsumer </seealso>
		public void startProduction(java.awt.image.ImageConsumer ic)
			addConsumer(ic)

		''' <summary>
		''' Requests that a given ImageConsumer have the image data delivered
		''' one more time in top-down, left-right order. </summary>
		''' <param name="ic"> the specified <code>ImageConsumer</code> </param>
		''' <seealso cref= ImageConsumer </seealso>
		public void requestTopDownLeftRightResend(java.awt.image.ImageConsumer ic)
			' Ignored.  The data is either single frame and already in TDLR
			' format or it is multi-frame and TDLR resends aren't critical.

		''' <summary>
		''' Changes this memory image into a multi-frame animation or a
		''' single-frame static image depending on the animated parameter.
		''' <p>This method should be called immediately after the
		''' MemoryImageSource is constructed and before an image is
		''' created with it to ensure that all ImageConsumers will
		''' receive the correct multi-frame data.  If an ImageConsumer
		''' is added to this ImageProducer before this flag is set then
		''' that ImageConsumer will see only a snapshot of the pixel
		''' data that was available when it connected. </summary>
		''' <param name="animated"> <code>true</code> if the image is a
		'''       multi-frame animation </param>
		public synchronized void animatedted(Boolean animated)
			Me.animating = animated
			If Not animating Then
				Dim enum_ As System.Collections.IEnumerator = theConsumers.elements()
				Do While enum_.hasMoreElements()
					Dim ic As java.awt.image.ImageConsumer = CType(enum_.nextElement(), java.awt.image.ImageConsumer)
					ic.imageComplete(java.awt.image.ImageConsumer.STATICIMAGEDONE)
					If isConsumer(ic) Then ic.imageComplete(java.awt.image.ImageConsumer.IMAGEERROR)
				Loop
				theConsumers.Clear()
			End If

		''' <summary>
		''' Specifies whether this animated memory image should always be
		''' updated by sending the complete buffer of pixels whenever
		''' there is a change.
		''' This flag is ignored if the animation flag is not turned on
		''' through the setAnimated() method.
		''' <p>This method should be called immediately after the
		''' MemoryImageSource is constructed and before an image is
		''' created with it to ensure that all ImageConsumers will
		''' receive the correct pixel delivery hints. </summary>
		''' <param name="fullbuffers"> <code>true</code> if the complete pixel
		'''             buffer should always
		''' be sent </param>
		''' <seealso cref= #setAnimated </seealso>
		public synchronized void fullBufferUpdatestes(Boolean fullbuffers)
			If Me.fullbuffers = fullbuffers Then Return
			Me.fullbuffers = fullbuffers
			If animating Then
				Dim enum_ As System.Collections.IEnumerator = theConsumers.elements()
				Do While enum_.hasMoreElements()
					Dim ic As java.awt.image.ImageConsumer = CType(enum_.nextElement(), java.awt.image.ImageConsumer)
					ic.hints = If(fullbuffers, (java.awt.image.ImageConsumer.TOPDOWNLEFTRIGHT Or java.awt.image.ImageConsumer.COMPLETESCANLINES), java.awt.image.ImageConsumer.RANDOMPIXELORDER)
				Loop
			End If

		''' <summary>
		''' Sends a whole new buffer of pixels to any ImageConsumers that
		''' are currently interested in the data for this image and notify
		''' them that an animation frame is complete.
		''' This method only has effect if the animation flag has been
		''' turned on through the setAnimated() method. </summary>
		''' <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		''' <seealso cref= ImageConsumer </seealso>
		''' <seealso cref= #setAnimated </seealso>
		public void newPixels()
			newPixels(0, 0, width, height, True)

		''' <summary>
		''' Sends a rectangular region of the buffer of pixels to any
		''' ImageConsumers that are currently interested in the data for
		''' this image and notify them that an animation frame is complete.
		''' This method only has effect if the animation flag has been
		''' turned on through the setAnimated() method.
		''' If the full buffer update flag was turned on with the
		''' setFullBufferUpdates() method then the rectangle parameters
		''' will be ignored and the entire buffer will always be sent. </summary>
		''' <param name="x"> the x coordinate of the upper left corner of the rectangle
		''' of pixels to be sent </param>
		''' <param name="y"> the y coordinate of the upper left corner of the rectangle
		''' of pixels to be sent </param>
		''' <param name="w"> the width of the rectangle of pixels to be sent </param>
		''' <param name="h"> the height of the rectangle of pixels to be sent </param>
		''' <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		''' <seealso cref= ImageConsumer </seealso>
		''' <seealso cref= #setAnimated </seealso>
		''' <seealso cref= #setFullBufferUpdates </seealso>
		public synchronized void newPixels(Integer x, Integer y, Integer w, Integer h)
			newPixels(x, y, w, h, True)

		''' <summary>
		''' Sends a rectangular region of the buffer of pixels to any
		''' ImageConsumers that are currently interested in the data for
		''' this image.
		''' If the framenotify parameter is true then the consumers are
		''' also notified that an animation frame is complete.
		''' This method only has effect if the animation flag has been
		''' turned on through the setAnimated() method.
		''' If the full buffer update flag was turned on with the
		''' setFullBufferUpdates() method then the rectangle parameters
		''' will be ignored and the entire buffer will always be sent. </summary>
		''' <param name="x"> the x coordinate of the upper left corner of the rectangle
		''' of pixels to be sent </param>
		''' <param name="y"> the y coordinate of the upper left corner of the rectangle
		''' of pixels to be sent </param>
		''' <param name="w"> the width of the rectangle of pixels to be sent </param>
		''' <param name="h"> the height of the rectangle of pixels to be sent </param>
		''' <param name="framenotify"> <code>true</code> if the consumers should be sent a
		''' <seealso cref="ImageConsumer#SINGLEFRAMEDONE SINGLEFRAMEDONE"/> notification </param>
		''' <seealso cref= ImageConsumer </seealso>
		''' <seealso cref= #setAnimated </seealso>
		''' <seealso cref= #setFullBufferUpdates </seealso>
		public synchronized void newPixels(Integer x, Integer y, Integer w, Integer h, Boolean framenotify)
			If animating Then
				If fullbuffers Then
						y = 0
						x = y
					w = width
					h = height
				Else
					If x < 0 Then
						w += x
						x = 0
					End If
					If x + w > width Then w = width - x
					If y < 0 Then
						h += y
						y = 0
					End If
					If y + h > height Then h = height - y
				End If
				If (w <= 0 OrElse h <= 0) AndAlso (Not framenotify) Then Return
				Dim enum_ As System.Collections.IEnumerator = theConsumers.elements()
				Do While enum_.hasMoreElements()
					Dim ic As java.awt.image.ImageConsumer = CType(enum_.nextElement(), java.awt.image.ImageConsumer)
					If w > 0 AndAlso h > 0 Then sendPixels(ic, x, y, w, h)
					If framenotify AndAlso isConsumer(ic) Then ic.imageComplete(java.awt.image.ImageConsumer.SINGLEFRAMEDONE)
				Loop
			End If

		''' <summary>
		''' Changes to a new byte array to hold the pixels for this image.
		''' If the animation flag has been turned on through the setAnimated()
		''' method, then the new pixels will be immediately delivered to any
		''' ImageConsumers that are currently interested in the data for
		''' this image. </summary>
		''' <param name="newpix"> the new pixel array </param>
		''' <param name="newmodel"> the specified <code>ColorModel</code> </param>
		''' <param name="offset"> the offset into the array </param>
		''' <param name="scansize"> the distance from one row of pixels to the next in
		''' the array </param>
		''' <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		''' <seealso cref= #setAnimated </seealso>
		public synchronized void newPixels(SByte() newpix, java.awt.image.ColorModel newmodel, Integer offset, Integer scansize)
			Me.pixels = newpix
			Me.model = newmodel
			Me.pixeloffset = offset
			Me.pixelscan = scansize
			newPixels()

		''' <summary>
		''' Changes to a new int array to hold the pixels for this image.
		''' If the animation flag has been turned on through the setAnimated()
		''' method, then the new pixels will be immediately delivered to any
		''' ImageConsumers that are currently interested in the data for
		''' this image. </summary>
		''' <param name="newpix"> the new pixel array </param>
		''' <param name="newmodel"> the specified <code>ColorModel</code> </param>
		''' <param name="offset"> the offset into the array </param>
		''' <param name="scansize"> the distance from one row of pixels to the next in
		''' the array </param>
		''' <seealso cref= #newPixels(int, int, int, int, boolean) </seealso>
		''' <seealso cref= #setAnimated </seealso>
		public synchronized void newPixels(Integer() newpix, java.awt.image.ColorModel newmodel, Integer offset, Integer scansize)
			Me.pixels = newpix
			Me.model = newmodel
			Me.pixeloffset = offset
			Me.pixelscan = scansize
			newPixels()

		private void initConsumer(java.awt.image.ImageConsumer ic)
			If isConsumer(ic) Then ic.dimensionsons(width, height)
			If isConsumer(ic) Then ic.properties = properties
			If isConsumer(ic) Then ic.colorModel = model
			If isConsumer(ic) Then ic.hints = If(animating, (If(fullbuffers, (java.awt.image.ImageConsumer.TOPDOWNLEFTRIGHT Or java.awt.image.ImageConsumer.COMPLETESCANLINES), java.awt.image.ImageConsumer.RANDOMPIXELORDER)), (java.awt.image.ImageConsumer.TOPDOWNLEFTRIGHT Or java.awt.image.ImageConsumer.COMPLETESCANLINES Or java.awt.image.ImageConsumer.SINGLEPASS Or java.awt.image.ImageConsumer.SINGLEFRAME))

		private void sendPixels(java.awt.image.ImageConsumer ic, Integer x, Integer y, Integer w, Integer h)
			Dim [off] As Integer = pixeloffset + pixelscan * y + x
			If isConsumer(ic) Then
				If TypeOf pixels Is SByte() Then
					ic.pixelsels(x, y, w, h, model, (CType(pixels, SByte())), [off], pixelscan)
				Else
					ic.pixelsels(x, y, w, h, model, (CType(pixels, Integer())), [off], pixelscan)
				End If
			End If
	End Class

End Namespace