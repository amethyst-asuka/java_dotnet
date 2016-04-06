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
	''' This class provides an easy way to create an ImageFilter which modifies
	''' the pixels of an image in the default RGB ColorModel.  It is meant to
	''' be used in conjunction with a FilteredImageSource object to produce
	''' filtered versions of existing images.  It is an abstract class that
	''' provides the calls needed to channel all of the pixel data through a
	''' single method which converts pixels one at a time in the default RGB
	''' ColorModel regardless of the ColorModel being used by the ImageProducer.
	''' The only method which needs to be defined to create a useable image
	''' filter is the filterRGB method.  Here is an example of a definition
	''' of a filter which swaps the red and blue components of an image:
	''' <pre>{@code
	''' 
	'''      class RedBlueSwapFilter extends RGBImageFilter {
	'''          public RedBlueSwapFilter() {
	'''              // The filter's operation does not depend on the
	'''              // pixel's location, so IndexColorModels can be
	'''              // filtered directly.
	'''              canFilterIndexColorModel = true;
	'''          }
	''' 
	'''          public int filterRGB(int x, int y, int rgb) {
	'''              return ((rgb & 0xff00ff00)
	'''                      | ((rgb & 0xff0000) >> 16)
	'''                      | ((rgb & 0xff) << 16));
	'''          }
	'''      }
	''' 
	''' }</pre>
	''' </summary>
	''' <seealso cref= FilteredImageSource </seealso>
	''' <seealso cref= ImageFilter </seealso>
	''' <seealso cref= ColorModel#getRGBdefault
	''' 
	''' @author      Jim Graham </seealso>
	Public MustInherit Class RGBImageFilter
		Inherits ImageFilter

		''' <summary>
		''' The <code>ColorModel</code> to be replaced by
		''' <code>newmodel</code> when the user calls
		''' <seealso cref="#substituteColorModel(ColorModel, ColorModel) substituteColorModel"/>.
		''' </summary>
		Protected Friend origmodel As java.awt.image.ColorModel

		''' <summary>
		''' The <code>ColorModel</code> with which to
		''' replace <code>origmodel</code> when the user calls
		''' <code>substituteColorModel</code>.
		''' </summary>
		Protected Friend newmodel As java.awt.image.ColorModel

		''' <summary>
		''' This boolean indicates whether or not it is acceptable to apply
		''' the color filtering of the filterRGB method to the color table
		''' entries of an IndexColorModel object in lieu of pixel by pixel
		''' filtering.  Subclasses should set this variable to true in their
		''' constructor if their filterRGB method does not depend on the
		''' coordinate of the pixel being filtered. </summary>
		''' <seealso cref= #substituteColorModel </seealso>
		''' <seealso cref= #filterRGB </seealso>
		''' <seealso cref= IndexColorModel </seealso>
		Protected Friend canFilterIndexColorModel As Boolean

		''' <summary>
		''' If the ColorModel is an IndexColorModel and the subclass has
		''' set the canFilterIndexColorModel flag to true, we substitute
		''' a filtered version of the color model here and wherever
		''' that original ColorModel object appears in the setPixels methods.
		''' If the ColorModel is not an IndexColorModel or is null, this method
		''' overrides the default ColorModel used by the ImageProducer and
		''' specifies the default RGB ColorModel instead.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation. </summary>
		''' <seealso cref= ImageConsumer </seealso>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		Public Overridable Property colorModel As java.awt.image.ColorModel
			Set(  model As java.awt.image.ColorModel)
				If canFilterIndexColorModel AndAlso (TypeOf model Is IndexColorModel) Then
					Dim newcm As java.awt.image.ColorModel = filterIndexColorModel(CType(model, IndexColorModel))
					substituteColorModel(model, newcm)
					consumer.colorModel = newcm
				Else
					consumer.colorModel = java.awt.image.ColorModel.rGBdefault
				End If
			End Set
		End Property

		''' <summary>
		''' Registers two ColorModel objects for substitution.  If the oldcm
		''' is encountered during any of the setPixels methods, the newcm
		''' is substituted and the pixels passed through
		''' untouched (but with the new ColorModel object). </summary>
		''' <param name="oldcm"> the ColorModel object to be replaced on the fly </param>
		''' <param name="newcm"> the ColorModel object to replace oldcm on the fly </param>
		Public Overridable Sub substituteColorModel(  oldcm As java.awt.image.ColorModel,   newcm As java.awt.image.ColorModel)
			origmodel = oldcm
			newmodel = newcm
		End Sub

		''' <summary>
		''' Filters an IndexColorModel object by running each entry in its
		''' color tables through the filterRGB function that RGBImageFilter
		''' subclasses must provide.  Uses coordinates of -1 to indicate that
		''' a color table entry is being filtered rather than an actual
		''' pixel value. </summary>
		''' <param name="icm"> the IndexColorModel object to be filtered </param>
		''' <exception cref="NullPointerException"> if <code>icm</code> is null </exception>
		''' <returns> a new IndexColorModel representing the filtered colors </returns>
		Public Overridable Function filterIndexColorModel(  icm As IndexColorModel) As IndexColorModel
			Dim mapsize As Integer = icm.mapSize
			Dim r As SByte() = New SByte(mapsize - 1){}
			Dim g As SByte() = New SByte(mapsize - 1){}
			Dim b As SByte() = New SByte(mapsize - 1){}
			Dim a As SByte() = New SByte(mapsize - 1){}
			icm.getReds(r)
			icm.getGreens(g)
			icm.getBlues(b)
			icm.getAlphas(a)
			Dim trans As Integer = icm.transparentPixel
			Dim needalpha As Boolean = False
			For i As Integer = 0 To mapsize - 1
				Dim rgb As Integer = filterRGB(-1, -1, icm.getRGB(i))
				a(i) = CByte(rgb >> 24)
				If a(i) <> (CByte(&Hff)) AndAlso i <> trans Then needalpha = True
				r(i) = CByte(rgb >> 16)
				g(i) = CByte(rgb >> 8)
				b(i) = CByte(rgb >> 0)
			Next i
			If needalpha Then
				Return New IndexColorModel(icm.pixelSize, mapsize, r, g, b, a)
			Else
				Return New IndexColorModel(icm.pixelSize, mapsize, r, g, b, trans)
			End If
		End Function

		''' <summary>
		''' Filters a buffer of pixels in the default RGB ColorModel by passing
		''' them one by one through the filterRGB method. </summary>
		''' <param name="x"> the X coordinate of the upper-left corner of the region
		'''          of pixels </param>
		''' <param name="y"> the Y coordinate of the upper-left corner of the region
		'''          of pixels </param>
		''' <param name="w"> the width of the region of pixels </param>
		''' <param name="h"> the height of the region of pixels </param>
		''' <param name="pixels"> the array of pixels </param>
		''' <param name="off"> the offset into the <code>pixels</code> array </param>
		''' <param name="scansize"> the distance from one row of pixels to the next
		'''        in the array </param>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #filterRGB </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  filterRGBPixels(int x, int y, int w, int h, int pixels() , int off, int scansize)
			Dim index As Integer = off
			For cy As Integer = 0 To h - 1
				For cx As Integer = 0 To w - 1
					pixels(index) = filterRGB(x + cx, y + cy, pixels(index))
					index += 1
				Next cx
				index += scansize - w
			Next cy
			consumer.pixelsels(x, y, w, h, java.awt.image.ColorModel.rGBdefault, pixels, off, scansize)

		''' <summary>
		''' If the ColorModel object is the same one that has already
		''' been converted, then simply passes the pixels through with the
		''' converted ColorModel. Otherwise converts the buffer of byte
		''' pixels to the default RGB ColorModel and passes the converted
		''' buffer to the filterRGBPixels method to be converted one by one.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation. </summary>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #filterRGBPixels </seealso>
		public  Sub  pixelsels(Integer x, Integer y, Integer w, Integer h, java.awt.image.ColorModel model, SByte pixels() , Integer off, Integer scansize)
			If model Is origmodel Then
				consumer.pixelsels(x, y, w, h, newmodel, pixels, off, scansize)
			Else
				Dim filteredpixels As Integer() = New Integer(w - 1){}
				Dim index As Integer = off
				For cy As Integer = 0 To h - 1
					For cx As Integer = 0 To w - 1
						filteredpixels(cx) = model.getRGB((pixels(index) And &Hff))
						index += 1
					Next cx
					index += scansize - w
					filterRGBPixels(x, y + cy, w, 1, filteredpixels, 0, w)
				Next cy
			End If

		''' <summary>
		''' If the ColorModel object is the same one that has already
		''' been converted, then simply passes the pixels through with the
		''' converted ColorModel, otherwise converts the buffer of integer
		''' pixels to the default RGB ColorModel and passes the converted
		''' buffer to the filterRGBPixels method to be converted one by one.
		''' Converts a buffer of integer pixels to the default RGB ColorModel
		''' and passes the converted buffer to the filterRGBPixels method.
		''' <p>
		''' Note: This method is intended to be called by the
		''' <code>ImageProducer</code> of the <code>Image</code> whose pixels
		''' are being filtered. Developers using
		''' this class to filter pixels from an image should avoid calling
		''' this method directly since that operation could interfere
		''' with the filtering operation. </summary>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #filterRGBPixels </seealso>
		public  Sub  pixelsels(Integer x, Integer y, Integer w, Integer h, java.awt.image.ColorModel model, Integer pixels() , Integer off, Integer scansize)
			If model Is origmodel Then
				consumer.pixelsels(x, y, w, h, newmodel, pixels, off, scansize)
			Else
				Dim filteredpixels As Integer() = New Integer(w - 1){}
				Dim index As Integer = off
				For cy As Integer = 0 To h - 1
					For cx As Integer = 0 To w - 1
						filteredpixels(cx) = model.getRGB(pixels(index))
						index += 1
					Next cx
					index += scansize - w
					filterRGBPixels(x, y + cy, w, 1, filteredpixels, 0, w)
				Next cy
			End If

		''' <summary>
		''' Subclasses must specify a method to convert a single input pixel
		''' in the default RGB ColorModel to a single output pixel. </summary>
		''' <param name="x"> the X coordinate of the pixel </param>
		''' <param name="y"> the Y coordinate of the pixel </param>
		''' <param name="rgb"> the integer pixel representation in the default RGB
		'''            color model </param>
		''' <returns> a filtered pixel in the default RGB color model. </returns>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		''' <seealso cref= #filterRGBPixels </seealso>
		Public MustOverride Integer filterRGB(Integer x, Integer y, Integer rgb)
	End Class

End Namespace