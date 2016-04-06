'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface describes single-input/single-output
	''' operations performed on <CODE>BufferedImage</CODE> objects.
	''' It is implemented by <CODE>AffineTransformOp</CODE>,
	''' <CODE>ConvolveOp</CODE>, <CODE>ColorConvertOp</CODE>, <CODE>RescaleOp</CODE>,
	''' and <CODE>LookupOp</CODE>.  These objects can be passed into
	''' a <CODE>BufferedImageFilter</CODE> to operate on a
	''' <CODE>BufferedImage</CODE> in the
	''' ImageProducer-ImageFilter-ImageConsumer paradigm.
	''' <p>
	''' Classes that implement this
	''' interface must specify whether or not they allow in-place filtering--
	''' filter operations where the source object is equal to the destination
	''' object.
	''' <p>
	''' This interface cannot be used to describe more sophisticated operations
	''' such as those that take multiple sources. Note that this restriction also
	''' means that the values of the destination pixels prior to the operation are
	''' not used as input to the filter operation.
	''' </summary>
	''' <seealso cref= BufferedImage </seealso>
	''' <seealso cref= BufferedImageFilter </seealso>
	''' <seealso cref= AffineTransformOp </seealso>
	''' <seealso cref= BandCombineOp </seealso>
	''' <seealso cref= ColorConvertOp </seealso>
	''' <seealso cref= ConvolveOp </seealso>
	''' <seealso cref= LookupOp </seealso>
	''' <seealso cref= RescaleOp </seealso>
	Public Interface BufferedImageOp
		''' <summary>
		''' Performs a single-input/single-output operation on a
		''' <CODE>BufferedImage</CODE>.
		''' If the color models for the two images do not match, a color
		''' conversion into the destination color model is performed.
		''' If the destination image is null,
		''' a <CODE>BufferedImage</CODE> with an appropriate <CODE>ColorModel</CODE>
		''' is created.
		''' <p>
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the source
		''' and/or destination image is incompatible with the types of images       $
		''' allowed by the class implementing this filter.
		''' </summary>
		''' <param name="src"> The <CODE>BufferedImage</CODE> to be filtered </param>
		''' <param name="dest"> The <CODE>BufferedImage</CODE> in which to store the results$
		''' </param>
		''' <returns> The filtered <CODE>BufferedImage</CODE>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the source and/or destination
		''' image is not compatible with the types of images allowed by the class
		''' implementing this filter. </exception>
		Function filter(  src As BufferedImage,   dest As BufferedImage) As BufferedImage

		''' <summary>
		''' Returns the bounding box of the filtered destination image.
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the source
		''' image is incompatible with the types of images allowed
		''' by the class implementing this filter.
		''' </summary>
		''' <param name="src"> The <CODE>BufferedImage</CODE> to be filtered
		''' </param>
		''' <returns> The <CODE>Rectangle2D</CODE> representing the destination
		''' image's bounding box. </returns>
		Function getBounds2D(  src As BufferedImage) As java.awt.geom.Rectangle2D

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number of
		''' bands.
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the source
		''' image is incompatible with the types of images allowed
		''' by the class implementing this filter.
		''' </summary>
		''' <param name="src"> The <CODE>BufferedImage</CODE> to be filtered </param>
		''' <param name="destCM"> <CODE>ColorModel</CODE> of the destination.  If null,
		''' the <CODE>ColorModel</CODE> of the source is used.
		''' </param>
		''' <returns> The zeroed destination image. </returns>
		Function createCompatibleDestImage(  src As BufferedImage,   destCM As ColorModel) As BufferedImage

		''' <summary>
		''' Returns the location of the corresponding destination point given a
		''' point in the source image.  If <CODE>dstPt</CODE> is specified, it
		''' is used to hold the return value. </summary>
		''' <param name="srcPt"> the <code>Point2D</code> that represents the point in
		''' the source image </param>
		''' <param name="dstPt"> The <CODE>Point2D</CODE> in which to store the result
		''' </param>
		''' <returns> The <CODE>Point2D</CODE> in the destination image that
		''' corresponds to the specified point in the source image. </returns>
		Function getPoint2D(  srcPt As java.awt.geom.Point2D,   dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D

		''' <summary>
		''' Returns the rendering hints for this operation.
		''' </summary>
		''' <returns> The <CODE>RenderingHints</CODE> object for this
		''' <CODE>BufferedImageOp</CODE>.  Returns
		''' null if no hints have been set. </returns>
		ReadOnly Property renderingHints As java.awt.RenderingHints
	End Interface

End Namespace