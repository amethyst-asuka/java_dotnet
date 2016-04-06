'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements a convolution from the source
	''' to the destination.
	''' Convolution using a convolution kernel is a spatial operation that
	''' computes the output pixel from an input pixel by multiplying the kernel
	''' with the surround of the input pixel.
	''' This allows the output pixel to be affected by the immediate neighborhood
	''' in a way that can be mathematically specified with a kernel.
	''' <p>
	''' This class operates with BufferedImage data in which color components are
	''' premultiplied with the alpha component.  If the Source BufferedImage has
	''' an alpha component, and the color components are not premultiplied with
	''' the alpha component, then the data are premultiplied before being
	''' convolved.  If the Destination has color components which are not
	''' premultiplied, then alpha is divided out before storing into the
	''' Destination (if alpha is 0, the color components are set to 0).  If the
	''' Destination has no alpha component, then the resulting alpha is discarded
	''' after first dividing it out of the color components.
	''' <p>
	''' Rasters are treated as having no alpha channel.  If the above treatment
	''' of the alpha channel in BufferedImages is not desired, it may be avoided
	''' by getting the Raster of a source BufferedImage and using the filter method
	''' of this class which works with Rasters.
	''' <p>
	''' If a RenderingHints object is specified in the constructor, the
	''' color rendering hint and the dithering hint may be used when color
	''' conversion is required.
	''' <p>
	''' Note that the Source and the Destination may not be the same object. </summary>
	''' <seealso cref= Kernel </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	Public Class ConvolveOp
		Implements BufferedImageOp, RasterOp

		Friend kernel As Kernel
		Friend edgeHint As Integer
		Friend hints As java.awt.RenderingHints
		''' <summary>
		''' Edge condition constants.
		''' </summary>

		''' <summary>
		''' Pixels at the edge of the destination image are set to zero.  This
		''' is the default.
		''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const EDGE_ZERO_FILL As Integer = 0

		''' <summary>
		''' Pixels at the edge of the source image are copied to
		''' the corresponding pixels in the destination without modification.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const EDGE_NO_OP As Integer = 1

		''' <summary>
		''' Constructs a ConvolveOp given a Kernel, an edge condition, and a
		''' RenderingHints object (which may be null). </summary>
		''' <param name="kernel"> the specified <code>Kernel</code> </param>
		''' <param name="edgeCondition"> the specified edge condition </param>
		''' <param name="hints"> the specified <code>RenderingHints</code> object </param>
		''' <seealso cref= Kernel </seealso>
		''' <seealso cref= #EDGE_NO_OP </seealso>
		''' <seealso cref= #EDGE_ZERO_FILL </seealso>
		''' <seealso cref= java.awt.RenderingHints </seealso>
		Public Sub New(  kernel_Renamed As Kernel,   edgeCondition As Integer,   hints As java.awt.RenderingHints)
			Me.kernel = kernel_Renamed
			Me.edgeHint = edgeCondition
			Me.hints = hints
		End Sub

		''' <summary>
		''' Constructs a ConvolveOp given a Kernel.  The edge condition
		''' will be EDGE_ZERO_FILL. </summary>
		''' <param name="kernel"> the specified <code>Kernel</code> </param>
		''' <seealso cref= Kernel </seealso>
		''' <seealso cref= #EDGE_ZERO_FILL </seealso>
		Public Sub New(  kernel_Renamed As Kernel)
			Me.kernel = kernel_Renamed
			Me.edgeHint = EDGE_ZERO_FILL
		End Sub

		''' <summary>
		''' Returns the edge condition. </summary>
		''' <returns> the edge condition of this <code>ConvolveOp</code>. </returns>
		''' <seealso cref= #EDGE_NO_OP </seealso>
		''' <seealso cref= #EDGE_ZERO_FILL </seealso>
		Public Overridable Property edgeCondition As Integer
			Get
				Return edgeHint
			End Get
		End Property

		''' <summary>
		''' Returns the Kernel. </summary>
		''' <returns> the <code>Kernel</code> of this <code>ConvolveOp</code>. </returns>
		Public Property kernel As Kernel
			Get
				Return CType(kernel.clone(), Kernel)
			End Get
		End Property

		''' <summary>
		''' Performs a convolution on BufferedImages.  Each component of the
		''' source image will be convolved (including the alpha component, if
		''' present).
		''' If the color model in the source image is not the same as that
		''' in the destination image, the pixels will be converted
		''' in the destination.  If the destination image is null,
		''' a BufferedImage will be created with the source ColorModel.
		''' The IllegalArgumentException may be thrown if the source is the
		''' same as the destination. </summary>
		''' <param name="src"> the source <code>BufferedImage</code> to filter </param>
		''' <param name="dst"> the destination <code>BufferedImage</code> for the
		'''        filtered <code>src</code> </param>
		''' <returns> the filtered <code>BufferedImage</code> </returns>
		''' <exception cref="NullPointerException"> if <code>src</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>src</code> equals
		'''         <code>dst</code> </exception>
		''' <exception cref="ImagingOpException"> if <code>src</code> cannot be filtered </exception>
		Public Function filter(  src As BufferedImage,   dst As BufferedImage) As BufferedImage Implements BufferedImageOp.filter
			If src Is Nothing Then Throw New NullPointerException("src image is null")
			If src Is dst Then Throw New IllegalArgumentException("src image cannot be the " & "same as the dst image")

			Dim needToConvert As Boolean = False
			Dim srcCM As ColorModel = src.colorModel
			Dim dstCM As ColorModel
			Dim origDst As BufferedImage = dst

			' Can't convolve an IndexColorModel.  Need to expand it
			If TypeOf srcCM Is IndexColorModel Then
				Dim icm As IndexColorModel = CType(srcCM, IndexColorModel)
				src = icm.convertToIntDiscrete(src.raster, False)
				srcCM = src.colorModel
			End If

			If dst Is Nothing Then
				dst = createCompatibleDestImage(src, Nothing)
				dstCM = srcCM
				origDst = dst
			Else
				dstCM = dst.colorModel
				If srcCM.colorSpace.type <> dstCM.colorSpace.type Then
					needToConvert = True
					dst = createCompatibleDestImage(src, Nothing)
					dstCM = dst.colorModel
				ElseIf TypeOf dstCM Is IndexColorModel Then
					dst = createCompatibleDestImage(src, Nothing)
					dstCM = dst.colorModel
				End If
			End If

			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then Throw New ImagingOpException("Unable to convolve src image")

			If needToConvert Then
				Dim ccop As New ColorConvertOp(hints)
				ccop.filter(dst, origDst)
			ElseIf origDst IsNot dst Then
				Dim g As java.awt.Graphics2D = origDst.createGraphics()
				Try
					g.drawImage(dst, 0, 0, Nothing)
				Finally
					g.Dispose()
				End Try
			End If

			Return origDst
		End Function

		''' <summary>
		''' Performs a convolution on Rasters.  Each band of the source Raster
		''' will be convolved.
		''' The source and destination must have the same number of bands.
		''' If the destination Raster is null, a new Raster will be created.
		''' The IllegalArgumentException may be thrown if the source is
		''' the same as the destination. </summary>
		''' <param name="src"> the source <code>Raster</code> to filter </param>
		''' <param name="dst"> the destination <code>WritableRaster</code> for the
		'''        filtered <code>src</code> </param>
		''' <returns> the filtered <code>WritableRaster</code> </returns>
		''' <exception cref="NullPointerException"> if <code>src</code> is <code>null</code> </exception>
		''' <exception cref="ImagingOpException"> if <code>src</code> and <code>dst</code>
		'''         do not have the same number of bands </exception>
		''' <exception cref="ImagingOpException"> if <code>src</code> cannot be filtered </exception>
		''' <exception cref="IllegalArgumentException"> if <code>src</code> equals
		'''         <code>dst</code> </exception>
		Public Function filter(  src As Raster,   dst As WritableRaster) As WritableRaster Implements RasterOp.filter
			If dst Is Nothing Then
				dst = createCompatibleDestRaster(src)
			ElseIf src Is dst Then
				Throw New IllegalArgumentException("src image cannot be the " & "same as the dst image")
			ElseIf src.numBands <> dst.numBands Then
				Throw New ImagingOpException("Different number of bands in src " & " and dst Rasters")
			End If

			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then Throw New ImagingOpException("Unable to convolve src image")

			Return dst
		End Function

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number
		''' of bands.  If destCM is null, an appropriate ColorModel will be used. </summary>
		''' <param name="src">       Source image for the filter operation. </param>
		''' <param name="destCM">    ColorModel of the destination.  Can be null. </param>
		''' <returns> a destination <code>BufferedImage</code> with the correct
		'''         size and number of bands. </returns>
		Public Overridable Function createCompatibleDestImage(  src As BufferedImage,   destCM As ColorModel) As BufferedImage Implements BufferedImageOp.createCompatibleDestImage
			Dim image_Renamed As BufferedImage

			Dim w As Integer = src.width
			Dim h As Integer = src.height

			Dim wr As WritableRaster = Nothing

			If destCM Is Nothing Then
				destCM = src.colorModel
				' Not much support for ICM
				If TypeOf destCM Is IndexColorModel Then
					destCM = ColorModel.rGBdefault
				Else
	'                 Create destination image as similar to the source
	'                 *  as it possible...
	'                 
					wr = src.data.createCompatibleWritableRaster(w, h)
				End If
			End If

			If wr Is Nothing Then wr = destCM.createCompatibleWritableRaster(w, h)

			image_Renamed = New BufferedImage(destCM, wr, destCM.alphaPremultiplied, Nothing)

			Return image_Renamed
		End Function

		''' <summary>
		''' Creates a zeroed destination Raster with the correct size and number
		''' of bands, given this source.
		''' </summary>
		Public Overridable Function createCompatibleDestRaster(  src As Raster) As WritableRaster Implements RasterOp.createCompatibleDestRaster
			Return src.createCompatibleWritableRaster()
		End Function

		''' <summary>
		''' Returns the bounding box of the filtered destination image.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change.
		''' </summary>
		Public Function getBounds2D(  src As BufferedImage) As java.awt.geom.Rectangle2D Implements BufferedImageOp.getBounds2D
			Return getBounds2D(src.raster)
		End Function

		''' <summary>
		''' Returns the bounding box of the filtered destination Raster.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change.
		''' </summary>
		Public Function getBounds2D(  src As Raster) As java.awt.geom.Rectangle2D Implements RasterOp.getBounds2D
			Return src.bounds
		End Function

		''' <summary>
		''' Returns the location of the destination point given a
		''' point in the source.  If dstPt is non-null, it will
		''' be used to hold the return value.  Since this is not a geometric
		''' operation, the srcPt will equal the dstPt.
		''' </summary>
		Public Function getPoint2D(  srcPt As java.awt.geom.Point2D,   dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D Implements BufferedImageOp.getPoint2D, RasterOp.getPoint2D
			If dstPt Is Nothing Then dstPt = New java.awt.geom.Point2D.Float
			dstPt.locationion(srcPt.x, srcPt.y)

			Return dstPt
		End Function

		''' <summary>
		''' Returns the rendering hints for this op.
		''' </summary>
		Public Property renderingHints As java.awt.RenderingHints Implements BufferedImageOp.getRenderingHints, RasterOp.getRenderingHints
			Get
				Return hints
			End Get
		End Property
	End Class

End Namespace