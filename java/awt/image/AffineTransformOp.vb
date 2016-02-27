Imports Microsoft.VisualBasic
Imports System

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
	''' This class uses an affine transform to perform a linear mapping from
	''' 2D coordinates in the source image or <CODE>Raster</CODE> to 2D coordinates
	''' in the destination image or <CODE>Raster</CODE>.
	''' The type of interpolation that is used is specified through a constructor,
	''' either by a <CODE>RenderingHints</CODE> object or by one of the integer
	''' interpolation types defined in this class.
	''' <p>
	''' If a <CODE>RenderingHints</CODE> object is specified in the constructor, the
	''' interpolation hint and the rendering quality hint are used to set
	''' the interpolation type for this operation.  The color rendering hint
	''' and the dithering hint can be used when color conversion is required.
	''' <p>
	''' Note that the following constraints have to be met:
	''' <ul>
	''' <li>The source and destination must be different.
	''' <li>For <CODE>Raster</CODE> objects, the number of bands in the source must
	''' be equal to the number of bands in the destination.
	''' </ul> </summary>
	''' <seealso cref= AffineTransform </seealso>
	''' <seealso cref= BufferedImageFilter </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_INTERPOLATION </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	Public Class AffineTransformOp
		Implements BufferedImageOp, RasterOp

		Private xform As java.awt.geom.AffineTransform
		Friend hints As java.awt.RenderingHints

		''' <summary>
		''' Nearest-neighbor interpolation type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_NEAREST_NEIGHBOR As Integer = 1

		''' <summary>
		''' Bilinear interpolation type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_BILINEAR As Integer = 2

		''' <summary>
		''' Bicubic interpolation type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TYPE_BICUBIC As Integer = 3

		Friend interpolationType As Integer = TYPE_NEAREST_NEIGHBOR

		''' <summary>
		''' Constructs an <CODE>AffineTransformOp</CODE> given an affine transform.
		''' The interpolation type is determined from the
		''' <CODE>RenderingHints</CODE> object.  If the interpolation hint is
		''' defined, it will be used. Otherwise, if the rendering quality hint is
		''' defined, the interpolation type is determined from its value.  If no
		''' hints are specified (<CODE>hints</CODE> is null),
		''' the interpolation type is {@link #TYPE_NEAREST_NEIGHBOR
		''' TYPE_NEAREST_NEIGHBOR}.
		''' </summary>
		''' <param name="xform"> The <CODE>AffineTransform</CODE> to use for the
		''' operation.
		''' </param>
		''' <param name="hints"> The <CODE>RenderingHints</CODE> object used to specify
		''' the interpolation type for the operation.
		''' </param>
		''' <exception cref="ImagingOpException"> if the transform is non-invertible. </exception>
		''' <seealso cref= java.awt.RenderingHints#KEY_INTERPOLATION </seealso>
		''' <seealso cref= java.awt.RenderingHints#KEY_RENDERING </seealso>
		Public Sub New(ByVal xform As java.awt.geom.AffineTransform, ByVal hints As java.awt.RenderingHints)
			validateTransform(xform)
			Me.xform = CType(xform.clone(), java.awt.geom.AffineTransform)
			Me.hints = hints

			If hints IsNot Nothing Then
				Dim value As Object = hints.get(hints.KEY_INTERPOLATION)
				If value Is Nothing Then
					value = hints.get(hints.KEY_RENDERING)
					If value Is hints.VALUE_RENDER_SPEED Then
						interpolationType = TYPE_NEAREST_NEIGHBOR
					ElseIf value Is hints.VALUE_RENDER_QUALITY Then
						interpolationType = TYPE_BILINEAR
					End If
				ElseIf value Is hints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR Then
					interpolationType = TYPE_NEAREST_NEIGHBOR
				ElseIf value Is hints.VALUE_INTERPOLATION_BILINEAR Then
					interpolationType = TYPE_BILINEAR
				ElseIf value Is hints.VALUE_INTERPOLATION_BICUBIC Then
					interpolationType = TYPE_BICUBIC
				End If
			Else
				interpolationType = TYPE_NEAREST_NEIGHBOR
			End If
		End Sub

		''' <summary>
		''' Constructs an <CODE>AffineTransformOp</CODE> given an affine transform
		''' and the interpolation type.
		''' </summary>
		''' <param name="xform"> The <CODE>AffineTransform</CODE> to use for the operation. </param>
		''' <param name="interpolationType"> One of the integer
		''' interpolation type constants defined by this class:
		''' <seealso cref="#TYPE_NEAREST_NEIGHBOR TYPE_NEAREST_NEIGHBOR"/>,
		''' <seealso cref="#TYPE_BILINEAR TYPE_BILINEAR"/>,
		''' <seealso cref="#TYPE_BICUBIC TYPE_BICUBIC"/>. </param>
		''' <exception cref="ImagingOpException"> if the transform is non-invertible. </exception>
		Public Sub New(ByVal xform As java.awt.geom.AffineTransform, ByVal interpolationType As Integer)
			validateTransform(xform)
			Me.xform = CType(xform.clone(), java.awt.geom.AffineTransform)
			Select Case interpolationType
				Case TYPE_NEAREST_NEIGHBOR, TYPE_BILINEAR, TYPE_BICUBIC
			Case Else
				Throw New IllegalArgumentException("Unknown interpolation type: " & interpolationType)
			End Select
			Me.interpolationType = interpolationType
		End Sub

		''' <summary>
		''' Returns the interpolation type used by this op. </summary>
		''' <returns> the interpolation type. </returns>
		''' <seealso cref= #TYPE_NEAREST_NEIGHBOR </seealso>
		''' <seealso cref= #TYPE_BILINEAR </seealso>
		''' <seealso cref= #TYPE_BICUBIC </seealso>
		Public Property interpolationType As Integer
			Get
				Return interpolationType
			End Get
		End Property

		''' <summary>
		''' Transforms the source <CODE>BufferedImage</CODE> and stores the results
		''' in the destination <CODE>BufferedImage</CODE>.
		''' If the color models for the two images do not match, a color
		''' conversion into the destination color model is performed.
		''' If the destination image is null,
		''' a <CODE>BufferedImage</CODE> is created with the source
		''' <CODE>ColorModel</CODE>.
		''' <p>
		''' The coordinates of the rectangle returned by
		''' <code>getBounds2D(BufferedImage)</code>
		''' are not necessarily the same as the coordinates of the
		''' <code>BufferedImage</code> returned by this method.  If the
		''' upper-left corner coordinates of the rectangle are
		''' negative then this part of the rectangle is not drawn.  If the
		''' upper-left corner coordinates of the  rectangle are positive
		''' then the filtered image is drawn at that position in the
		''' destination <code>BufferedImage</code>.
		''' <p>
		''' An <CODE>IllegalArgumentException</CODE> is thrown if the source is
		''' the same as the destination.
		''' </summary>
		''' <param name="src"> The <CODE>BufferedImage</CODE> to transform. </param>
		''' <param name="dst"> The <CODE>BufferedImage</CODE> in which to store the results
		''' of the transformation.
		''' </param>
		''' <returns> The filtered <CODE>BufferedImage</CODE>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>src</code> and
		'''         <code>dst</code> are the same </exception>
		''' <exception cref="ImagingOpException"> if the image cannot be transformed
		'''         because of a data-processing error that might be
		'''         caused by an invalid image format, tile format, or
		'''         image-processing operation, or any other unsupported
		'''         operation. </exception>
		Public Function filter(ByVal src As BufferedImage, ByVal dst As BufferedImage) As BufferedImage Implements BufferedImageOp.filter

			If src Is Nothing Then Throw New NullPointerException("src image is null")
			If src Is dst Then Throw New IllegalArgumentException("src image cannot be the " & "same as the dst image")

			Dim needToConvert As Boolean = False
			Dim srcCM As ColorModel = src.colorModel
			Dim dstCM As ColorModel
			Dim origDst As BufferedImage = dst

			If dst Is Nothing Then
				dst = createCompatibleDestImage(src, Nothing)
				dstCM = srcCM
				origDst = dst
			Else
				dstCM = dst.colorModel
				If srcCM.colorSpace.type <> dstCM.colorSpace.type Then
					Dim type As Integer = xform.type
					Dim needTrans As Boolean = ((type And (xform.TYPE_MASK_ROTATION Or xform.TYPE_GENERAL_TRANSFORM)) <> 0)
					If (Not needTrans) AndAlso type <> xform.TYPE_TRANSLATION AndAlso type <> xform.TYPE_IDENTITY Then
						Dim mtx As Double() = New Double(3){}
						xform.getMatrix(mtx)
						' Check out the matrix.  A non-integral scale will force ARGB
						' since the edge conditions can't be guaranteed.
						needTrans = (mtx(0) <> CInt(Fix(mtx(0))) OrElse mtx(3) <> CInt(Fix(mtx(3))))
					End If

					If needTrans AndAlso srcCM.transparency = java.awt.Transparency.OPAQUE Then
						' Need to convert first
						Dim ccop As New ColorConvertOp(hints)
						Dim tmpSrc As BufferedImage = Nothing
						Dim sw As Integer = src.width
						Dim sh As Integer = src.height
						If dstCM.transparency = java.awt.Transparency.OPAQUE Then
							tmpSrc = New BufferedImage(sw, sh, BufferedImage.TYPE_INT_ARGB)
						Else
							Dim r As WritableRaster = dstCM.createCompatibleWritableRaster(sw, sh)
							tmpSrc = New BufferedImage(dstCM, r, dstCM.alphaPremultiplied, Nothing)
						End If
						src = ccop.filter(src, tmpSrc)
					Else
						needToConvert = True
						dst = createCompatibleDestImage(src, Nothing)
					End If
				End If

			End If

			If interpolationType <> TYPE_NEAREST_NEIGHBOR AndAlso TypeOf dst.colorModel Is IndexColorModel Then dst = New BufferedImage(dst.width, dst.height, BufferedImage.TYPE_INT_ARGB)
			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then Throw New ImagingOpException("Unable to transform src image")

			If needToConvert Then
				Dim ccop As New ColorConvertOp(hints)
				ccop.filter(dst, origDst)
			ElseIf origDst IsNot dst Then
				Dim g As java.awt.Graphics2D = origDst.createGraphics()
				Try
					g.composite = java.awt.AlphaComposite.Src
					g.drawImage(dst, 0, 0, Nothing)
				Finally
					g.Dispose()
				End Try
			End If

			Return origDst
		End Function

		''' <summary>
		''' Transforms the source <CODE>Raster</CODE> and stores the results in
		''' the destination <CODE>Raster</CODE>.  This operation performs the
		''' transform band by band.
		''' <p>
		''' If the destination <CODE>Raster</CODE> is null, a new
		''' <CODE>Raster</CODE> is created.
		''' An <CODE>IllegalArgumentException</CODE> may be thrown if the source is
		''' the same as the destination or if the number of bands in
		''' the source is not equal to the number of bands in the
		''' destination.
		''' <p>
		''' The coordinates of the rectangle returned by
		''' <code>getBounds2D(Raster)</code>
		''' are not necessarily the same as the coordinates of the
		''' <code>WritableRaster</code> returned by this method.  If the
		''' upper-left corner coordinates of rectangle are negative then
		''' this part of the rectangle is not drawn.  If the coordinates
		''' of the rectangle are positive then the filtered image is drawn at
		''' that position in the destination <code>Raster</code>.
		''' <p> </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to transform. </param>
		''' <param name="dst"> The <CODE>Raster</CODE> in which to store the results of the
		''' transformation.
		''' </param>
		''' <returns> The transformed <CODE>Raster</CODE>.
		''' </returns>
		''' <exception cref="ImagingOpException"> if the raster cannot be transformed
		'''         because of a data-processing error that might be
		'''         caused by an invalid image format, tile format, or
		'''         image-processing operation, or any other unsupported
		'''         operation. </exception>
		Public Function filter(ByVal src As Raster, ByVal dst As WritableRaster) As WritableRaster Implements RasterOp.filter
			If src Is Nothing Then Throw New NullPointerException("src image is null")
			If dst Is Nothing Then dst = createCompatibleDestRaster(src)
			If src Is dst Then Throw New IllegalArgumentException("src image cannot be the " & "same as the dst image")
			If src.numBands <> dst.numBands Then Throw New IllegalArgumentException("Number of src bands (" & src.numBands & ") does not match number of " & " dst bands (" & dst.numBands & ")")

			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then Throw New ImagingOpException("Unable to transform src image")
			Return dst
		End Function

		''' <summary>
		''' Returns the bounding box of the transformed destination.  The
		''' rectangle returned is the actual bounding box of the
		''' transformed points.  The coordinates of the upper-left corner
		''' of the returned rectangle might not be (0,&nbsp;0).
		''' </summary>
		''' <param name="src"> The <CODE>BufferedImage</CODE> to be transformed.
		''' </param>
		''' <returns> The <CODE>Rectangle2D</CODE> representing the destination's
		''' bounding box. </returns>
		Public Function getBounds2D(ByVal src As BufferedImage) As java.awt.geom.Rectangle2D Implements BufferedImageOp.getBounds2D
			Return getBounds2D(src.raster)
		End Function

		''' <summary>
		''' Returns the bounding box of the transformed destination.  The
		''' rectangle returned will be the actual bounding box of the
		''' transformed points.  The coordinates of the upper-left corner
		''' of the returned rectangle might not be (0,&nbsp;0).
		''' </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to be transformed.
		''' </param>
		''' <returns> The <CODE>Rectangle2D</CODE> representing the destination's
		''' bounding box. </returns>
		Public Function getBounds2D(ByVal src As Raster) As java.awt.geom.Rectangle2D Implements RasterOp.getBounds2D
			Dim w As Integer = src.width
			Dim h As Integer = src.height

			' Get the bounding box of the src and transform the corners
			Dim pts As Single() = {0, 0, w, 0, w, h, 0, h}
			xform.transform(pts, 0, pts, 0, 4)

			' Get the min, max of the dst
			Dim fmaxX As Single = pts(0)
			Dim fmaxY As Single = pts(1)
			Dim fminX As Single = pts(0)
			Dim fminY As Single = pts(1)
			For i As Integer = 2 To 7 Step 2
				If pts(i) > fmaxX Then
					fmaxX = pts(i)
				ElseIf pts(i) < fminX Then
					fminX = pts(i)
				End If
				If pts(i+1) > fmaxY Then
					fmaxY = pts(i+1)
				ElseIf pts(i+1) < fminY Then
					fminY = pts(i+1)
				End If
			Next i

			Return New java.awt.geom.Rectangle2D.Float(fminX, fminY, fmaxX-fminX, fmaxY-fminY)
		End Function

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number of
		''' bands.  A <CODE>RasterFormatException</CODE> may be thrown if the
		''' transformed width or height is equal to 0.
		''' <p>
		''' If <CODE>destCM</CODE> is null,
		''' an appropriate <CODE>ColorModel</CODE> is used; this
		''' <CODE>ColorModel</CODE> may have
		''' an alpha channel even if the source <CODE>ColorModel</CODE> is opaque.
		''' </summary>
		''' <param name="src">  The <CODE>BufferedImage</CODE> to be transformed. </param>
		''' <param name="destCM">  <CODE>ColorModel</CODE> of the destination.  If null,
		''' an appropriate <CODE>ColorModel</CODE> is used.
		''' </param>
		''' <returns> The zeroed destination image. </returns>
		Public Overridable Function createCompatibleDestImage(ByVal src As BufferedImage, ByVal destCM As ColorModel) As BufferedImage Implements BufferedImageOp.createCompatibleDestImage
			Dim image_Renamed As BufferedImage
			Dim r As java.awt.Rectangle = getBounds2D(src).bounds

			' If r.x (or r.y) is < 0, then we want to only create an image
			' that is in the positive range.
			' If r.x (or r.y) is > 0, then we need to create an image that
			' includes the translation.
			Dim w As Integer = r.x + r.width
			Dim h As Integer = r.y + r.height
			If w <= 0 Then Throw New RasterFormatException("Transformed width (" & w & ") is less than or equal to 0.")
			If h <= 0 Then Throw New RasterFormatException("Transformed height (" & h & ") is less than or equal to 0.")

			If destCM Is Nothing Then
				Dim cm As ColorModel = src.colorModel
				If interpolationType <> TYPE_NEAREST_NEIGHBOR AndAlso (TypeOf cm Is IndexColorModel OrElse cm.transparency = java.awt.Transparency.OPAQUE) Then
					image_Renamed = New BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB)
				Else
					image_Renamed = New BufferedImage(cm, src.raster.createCompatibleWritableRaster(w,h), cm.alphaPremultiplied, Nothing)
				End If
			Else
				image_Renamed = New BufferedImage(destCM, destCM.createCompatibleWritableRaster(w,h), destCM.alphaPremultiplied, Nothing)
			End If

			Return image_Renamed
		End Function

		''' <summary>
		''' Creates a zeroed destination <CODE>Raster</CODE> with the correct size
		''' and number of bands.  A <CODE>RasterFormatException</CODE> may be thrown
		''' if the transformed width or height is equal to 0.
		''' </summary>
		''' <param name="src"> The <CODE>Raster</CODE> to be transformed.
		''' </param>
		''' <returns> The zeroed destination <CODE>Raster</CODE>. </returns>
		Public Overridable Function createCompatibleDestRaster(ByVal src As Raster) As WritableRaster Implements RasterOp.createCompatibleDestRaster
			Dim r As java.awt.geom.Rectangle2D = getBounds2D(src)

			Return src.createCompatibleWritableRaster(CInt(Fix(r.x)), CInt(Fix(r.y)), CInt(Fix(r.width)), CInt(Fix(r.height)))
		End Function

		''' <summary>
		''' Returns the location of the corresponding destination point given a
		''' point in the source.  If <CODE>dstPt</CODE> is specified, it
		''' is used to hold the return value.
		''' </summary>
		''' <param name="srcPt"> The <code>Point2D</code> that represents the source
		'''              point. </param>
		''' <param name="dstPt"> The <CODE>Point2D</CODE> in which to store the result.
		''' </param>
		''' <returns> The <CODE>Point2D</CODE> in the destination that corresponds to
		''' the specified point in the source. </returns>
		Public Function getPoint2D(ByVal srcPt As java.awt.geom.Point2D, ByVal dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D Implements BufferedImageOp.getPoint2D, RasterOp.getPoint2D
			Return xform.transform(srcPt, dstPt)
		End Function

		''' <summary>
		''' Returns the affine transform used by this transform operation.
		''' </summary>
		''' <returns> The <CODE>AffineTransform</CODE> associated with this op. </returns>
		Public Property transform As java.awt.geom.AffineTransform
			Get
				Return CType(xform.clone(), java.awt.geom.AffineTransform)
			End Get
		End Property

		''' <summary>
		''' Returns the rendering hints used by this transform operation.
		''' </summary>
		''' <returns> The <CODE>RenderingHints</CODE> object associated with this op. </returns>
		Public Property renderingHints As java.awt.RenderingHints Implements BufferedImageOp.getRenderingHints, RasterOp.getRenderingHints
			Get
				If hints Is Nothing Then
					Dim val As Object
					Select Case interpolationType
					Case TYPE_NEAREST_NEIGHBOR
						val = java.awt.RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR
					Case TYPE_BILINEAR
						val = java.awt.RenderingHints.VALUE_INTERPOLATION_BILINEAR
					Case TYPE_BICUBIC
						val = java.awt.RenderingHints.VALUE_INTERPOLATION_BICUBIC
					Case Else
						' Should never get here
						Throw New InternalError("Unknown interpolation type " & interpolationType)
    
					End Select
					hints = New java.awt.RenderingHints(java.awt.RenderingHints.KEY_INTERPOLATION, val)
				End If
    
				Return hints
			End Get
		End Property

		' We need to be able to invert the transform if we want to
		' transform the image.  If the determinant of the matrix is 0,
		' then we can't invert the transform.
		Friend Overridable Sub validateTransform(ByVal xform As java.awt.geom.AffineTransform)
			If System.Math.Abs(xform.determinant) <= java.lang.[Double].MIN_VALUE Then Throw New ImagingOpException("Unable to invert transform " & xform)
		End Sub
	End Class

End Namespace