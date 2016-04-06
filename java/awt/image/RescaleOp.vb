Imports Microsoft.VisualBasic
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
	''' This class performs a pixel-by-pixel rescaling of the data in the
	''' source image by multiplying the sample values for each pixel by a scale
	''' factor and then adding an offset. The scaled sample values are clipped
	''' to the minimum/maximum representable in the destination image.
	''' <p>
	''' The pseudo code for the rescaling operation is as follows:
	''' <pre>
	''' for each pixel from Source object {
	'''    for each band/component of the pixel {
	'''        dstElement = (srcElement*scaleFactor) + offset
	'''    }
	''' }
	''' </pre>
	''' <p>
	''' For Rasters, rescaling operates on bands.  The number of
	''' sets of scaling constants may be one, in which case the same constants
	''' are applied to all bands, or it must equal the number of Source
	''' Raster bands.
	''' <p>
	''' For BufferedImages, rescaling operates on color and alpha components.
	''' The number of sets of scaling constants may be one, in which case the
	''' same constants are applied to all color (but not alpha) components.
	''' Otherwise, the  number of sets of scaling constants may
	''' equal the number of Source color components, in which case no
	''' rescaling of the alpha component (if present) is performed.
	''' If neither of these cases apply, the number of sets of scaling constants
	''' must equal the number of Source color components plus alpha components,
	''' in which case all color and alpha components are rescaled.
	''' <p>
	''' BufferedImage sources with premultiplied alpha data are treated in the same
	''' manner as non-premultiplied images for purposes of rescaling.  That is,
	''' the rescaling is done per band on the raw data of the BufferedImage source
	''' without regard to whether the data is premultiplied.  If a color conversion
	''' is required to the destination ColorModel, the premultiplied state of
	''' both source and destination will be taken into account for this step.
	''' <p>
	''' Images with an IndexColorModel cannot be rescaled.
	''' <p>
	''' If a RenderingHints object is specified in the constructor, the
	''' color rendering hint and the dithering hint may be used when color
	''' conversion is required.
	''' <p>
	''' Note that in-place operation is allowed (i.e. the source and destination can
	''' be the same object). </summary>
	''' <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>
	Public Class RescaleOp
		Implements BufferedImageOp, RasterOp

		Friend scaleFactors As Single()
		Friend offsets As Single()
		Friend length As Integer = 0
		Friend hints As java.awt.RenderingHints

		Private srcNbits As Integer
		Private dstNbits As Integer


		''' <summary>
		''' Constructs a new RescaleOp with the desired scale factors
		''' and offsets.  The length of the scaleFactor and offset arrays
		''' must meet the restrictions stated in the class comments above.
		''' The RenderingHints argument may be null. </summary>
		''' <param name="scaleFactors"> the specified scale factors </param>
		''' <param name="offsets"> the specified offsets </param>
		''' <param name="hints"> the specified <code>RenderingHints</code>, or
		'''        <code>null</code> </param>
		Public Sub New(  scaleFactors As Single(),   offsets As Single(),   hints As java.awt.RenderingHints)
			length = scaleFactors.Length
			If length > offsets.Length Then length = offsets.Length

			Me.scaleFactors = New Single(length - 1){}
			Me.offsets = New Single(length - 1){}
			For i As Integer = 0 To length - 1
				Me.scaleFactors(i) = scaleFactors(i)
				Me.offsets(i) = offsets(i)
			Next i
			Me.hints = hints
		End Sub

		''' <summary>
		''' Constructs a new RescaleOp with the desired scale factor
		''' and offset.  The scaleFactor and offset will be applied to
		''' all bands in a source Raster and to all color (but not alpha)
		''' components in a BufferedImage.
		''' The RenderingHints argument may be null. </summary>
		''' <param name="scaleFactor"> the specified scale factor </param>
		''' <param name="offset"> the specified offset </param>
		''' <param name="hints"> the specified <code>RenderingHints</code>, or
		'''        <code>null</code> </param>
		Public Sub New(  scaleFactor As Single,   offset As Single,   hints As java.awt.RenderingHints)
			length = 1
			Me.scaleFactors = New Single(0){}
			Me.offsets = New Single(0){}
			Me.scaleFactors(0) = scaleFactor
			Me.offsets(0) = offset
			Me.hints = hints
		End Sub

		''' <summary>
		''' Returns the scale factors in the given array. The array is also
		''' returned for convenience.  If scaleFactors is null, a new array
		''' will be allocated. </summary>
		''' <param name="scaleFactors"> the array to contain the scale factors of
		'''        this <code>RescaleOp</code> </param>
		''' <returns> the scale factors of this <code>RescaleOp</code>. </returns>
		Public Function getScaleFactors(  scaleFactors As Single()) As Single()
			If scaleFactors Is Nothing Then Return CType(Me.scaleFactors.clone(), Single())
			Array.Copy(Me.scaleFactors, 0, scaleFactors, 0, System.Math.Min(Me.scaleFactors.Length, scaleFactors.Length))
			Return scaleFactors
		End Function

		''' <summary>
		''' Returns the offsets in the given array. The array is also returned
		''' for convenience.  If offsets is null, a new array
		''' will be allocated. </summary>
		''' <param name="offsets"> the array to contain the offsets of
		'''        this <code>RescaleOp</code> </param>
		''' <returns> the offsets of this <code>RescaleOp</code>. </returns>
		Public Function getOffsets(  offsets As Single()) As Single()
			If offsets Is Nothing Then Return CType(Me.offsets.clone(), Single())

			Array.Copy(Me.offsets, 0, offsets, 0, System.Math.Min(Me.offsets.Length, offsets.Length))
			Return offsets
		End Function

		''' <summary>
		''' Returns the number of scaling factors and offsets used in this
		''' RescaleOp. </summary>
		''' <returns> the number of scaling factors and offsets of this
		'''         <code>RescaleOp</code>. </returns>
		Public Property numFactors As Integer
			Get
				Return length
			End Get
		End Property


		''' <summary>
		''' Creates a ByteLookupTable to implement the rescale.
		''' The table may have either a SHORT or BYTE input. </summary>
		''' <param name="nElems">    Number of elements the table is to have.
		'''                  This will generally be 256 for byte and
		'''                  65536 for  java.lang.[Short]. </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private ByteLookupTable createByteLut(float scale() , float off(), int nBands, int nElems)

			Dim lutData As SByte()() = RectangularArrays.ReturnRectangularSByteArray(scale.length, nElems)

			For band As Integer = 0 To scale.length - 1
				Dim bandScale As Single = scale(band)
				Dim bandOff As Single = off(band)
				Dim bandLutData As SByte() = lutData(band)
				For i As Integer = 0 To nElems - 1
					Dim val As Integer = CInt(Fix(i*bandScale + bandOff))
					If (val And &Hffffff00L) <> 0 Then
						If val < 0 Then
							val = 0
						Else
							val = 255
						End If
					End If
					bandLutData(i) = CByte(val)
				Next i

			Next band

			Return New ByteLookupTable(0, lutData)

		''' <summary>
		''' Creates a ShortLookupTable to implement the rescale.
		''' The table may have either a SHORT or BYTE input. </summary>
		''' <param name="nElems">    Number of elements the table is to have.
		'''                  This will generally be 256 for byte and
		'''                  65536 for  java.lang.[Short]. </param>
		private ShortLookupTable createShortLut(Single scale() , Single off(), Integer nBands, Integer nElems)

			Dim lutData As Short()() = RectangularArrays.ReturnRectangularShortArray(scale.length, nElems)

			For band As Integer = 0 To scale.length - 1
				Dim bandScale As Single = scale(band)
				Dim bandOff As Single = off(band)
				Dim bandLutData As Short() = lutData(band)
				For i As Integer = 0 To nElems - 1
					Dim val As Integer = CInt(Fix(i*bandScale + bandOff))
					If (val And &Hffff0000L) <> 0 Then
						If val < 0 Then
							val = 0
						Else
							val = 65535
						End If
					End If
					bandLutData(i) = CShort(val)
				Next i
			Next band

			Return New ShortLookupTable(0, lutData)


		''' <summary>
		''' Determines if the rescale can be performed as a lookup.
		''' The dst must be a byte or short type.
		''' The src must be less than 16 bits.
		''' All source band sizes must be the same and all dst band sizes
		''' must be the same.
		''' </summary>
		private Boolean canUseLookup(Raster src, Raster dst)

			'
			' Check that the src datatype is either a BYTE or SHORT
			'
			Dim datatype As Integer = src.dataBuffer.dataType
			If datatype <> DataBuffer.TYPE_BYTE AndAlso datatype <> DataBuffer.TYPE_USHORT Then Return False

			'
			' Check dst sample sizes. All must be 8 or 16 bits.
			'
			Dim dstSM As SampleModel = dst.sampleModel
			dstNbits = dstSM.getSampleSize(0)

			If Not(dstNbits = 8 OrElse dstNbits = 16) Then Return False
			For i As Integer = 1 To src.numBands - 1
				Dim bandSize As Integer = dstSM.getSampleSize(i)
				If bandSize <> dstNbits Then Return False
			Next i

			'
			' Check src sample sizes. All must be the same size
			'
			Dim srcSM As SampleModel = src.sampleModel
			srcNbits = srcSM.getSampleSize(0)
			If srcNbits > 16 Then Return False
			For i As Integer = 1 To src.numBands - 1
				Dim bandSize As Integer = srcSM.getSampleSize(i)
				If bandSize <> srcNbits Then Return False
			Next i

			Return True

		''' <summary>
		''' Rescales the source BufferedImage.
		''' If the color model in the source image is not the same as that
		''' in the destination image, the pixels will be converted
		''' in the destination.  If the destination image is null,
		''' a BufferedImage will be created with the source ColorModel.
		''' An IllegalArgumentException may be thrown if the number of
		''' scaling factors/offsets in this object does not meet the
		''' restrictions stated in the class comments above, or if the
		''' source image has an IndexColorModel. </summary>
		''' <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		''' <param name="dst"> the destination for the filtering operation
		'''            or <code>null</code> </param>
		''' <returns> the filtered <code>BufferedImage</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if the <code>ColorModel</code>
		'''         of <code>src</code> is an <code>IndexColorModel</code>,
		'''         or if the number of scaling factors and offsets in this
		'''         <code>RescaleOp</code> do not meet the requirements
		'''         stated in the class comments. </exception>
		public final BufferedImage filter(BufferedImage src, BufferedImage dst)
			Dim srcCM As ColorModel = src.colorModel
			Dim dstCM As ColorModel
			Dim numBands As Integer = srcCM.numColorComponents


			If TypeOf srcCM Is IndexColorModel Then Throw New IllegalArgumentException("Rescaling cannot be " & "performed on an indexed image")
			If length <> 1 AndAlso length <> numBands AndAlso length <> srcCM.numComponents Then Throw New IllegalArgumentException("Number of scaling constants " & "does not equal the number of" & " of color or color/alpha " & " components")

			Dim needToConvert As Boolean = False

			' Include alpha
			If length > numBands AndAlso srcCM.hasAlpha() Then length = numBands+1

			Dim width As Integer = src.width
			Dim height As Integer = src.height

			If dst Is Nothing Then
				dst = createCompatibleDestImage(src, Nothing)
				dstCM = srcCM
			Else
				If width <> dst.width Then Throw New IllegalArgumentException("Src width (" & width & ") not equal to dst width (" & dst.width & ")")
				If height <> dst.height Then Throw New IllegalArgumentException("Src height (" & height & ") not equal to dst height (" & dst.height & ")")

				dstCM = dst.colorModel
				If srcCM.colorSpace.type <> dstCM.colorSpace.type Then
					needToConvert = True
					dst = createCompatibleDestImage(src, Nothing)
				End If

			End If

			Dim origDst As BufferedImage = dst

			'
			' Try to use a native BI rescale operation first
			'
			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then
				'
				' Native BI rescale failed - convert to rasters
				'
				Dim srcRaster As WritableRaster = src.raster
				Dim dstRaster As WritableRaster = dst.raster

				If srcCM.hasAlpha() Then
					If numBands-1 = length OrElse length = 1 Then
						Dim minx As Integer = srcRaster.minX
						Dim miny As Integer = srcRaster.minY
						Dim bands As Integer() = New Integer(numBands-2){}
						For i As Integer = 0 To numBands-2
							bands(i) = i
						Next i
						srcRaster = srcRaster.createWritableChild(minx, miny, srcRaster.width, srcRaster.height, minx, miny, bands)
					End If
				End If
				If dstCM.hasAlpha() Then
					Dim dstNumBands As Integer = dstRaster.numBands
					If dstNumBands-1 = length OrElse length = 1 Then
						Dim minx As Integer = dstRaster.minX
						Dim miny As Integer = dstRaster.minY
						Dim bands As Integer() = New Integer(numBands-2){}
						For i As Integer = 0 To numBands-2
							bands(i) = i
						Next i
						dstRaster = dstRaster.createWritableChild(minx, miny, dstRaster.width, dstRaster.height, minx, miny, bands)
					End If
				End If

				'
				' Call the raster filter method
				'
				filter(srcRaster, dstRaster)

			End If

			If needToConvert Then
				' ColorModels are not the same
				Dim ccop As New ColorConvertOp(hints)
				ccop.filter(dst, origDst)
			End If

			Return origDst

		''' <summary>
		''' Rescales the pixel data in the source Raster.
		''' If the destination Raster is null, a new Raster will be created.
		''' The source and destination must have the same number of bands.
		''' Otherwise, an IllegalArgumentException is thrown.
		''' Note that the number of scaling factors/offsets in this object must
		''' meet the restrictions stated in the class comments above.
		''' Otherwise, an IllegalArgumentException is thrown. </summary>
		''' <param name="src"> the <code>Raster</code> to be filtered </param>
		''' <param name="dst"> the destination for the filtering operation
		'''            or <code>null</code> </param>
		''' <returns> the filtered <code>WritableRaster</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>src</code> and
		'''         <code>dst</code> do not have the same number of bands,
		'''         or if the number of scaling factors and offsets in this
		'''         <code>RescaleOp</code> do not meet the requirements
		'''         stated in the class comments. </exception>
		public final WritableRaster filter(Raster src, WritableRaster dst)
			Dim numBands As Integer = src.numBands
			Dim width As Integer = src.width
			Dim height As Integer = src.height
			Dim srcPix As Integer() = Nothing
			Dim [step] As Integer = 0
			Dim tidx As Integer = 0

			' Create a new destination Raster, if needed
			If dst Is Nothing Then
				dst = createCompatibleDestRaster(src)
			ElseIf height <> dst.height OrElse width <> dst.width Then
				Throw New IllegalArgumentException("Width or height of Rasters do not " & "match")
			ElseIf numBands <> dst.numBands Then
				' Make sure that the number of bands are equal
				Throw New IllegalArgumentException("Number of bands in src " & numBands & " does not equal number of bands in dest " & dst.numBands)
			End If
			' Make sure that the arrays match
			' Make sure that the low/high/constant arrays match
			If length <> 1 AndAlso length <> src.numBands Then Throw New IllegalArgumentException("Number of scaling constants " & "does not equal the number of" & " of bands in the src raster")


			'
			' Try for a native raster rescale first
			'
			If sun.awt.image.ImagingLib.filter(Me, src, dst) IsNot Nothing Then Return dst

			'
			' Native raster rescale failed.
			' Try to see if a lookup operation can be used
			'
			If canUseLookup(src, dst) Then
				Dim srcNgray As Integer = (1 << srcNbits)
				Dim dstNgray As Integer = (1 << dstNbits)

				If dstNgray = 256 Then
					Dim lut As ByteLookupTable = createByteLut(scaleFactors, offsets, numBands, srcNgray)
					Dim op As New LookupOp(lut, hints)
					op.filter(src, dst)
				Else
					Dim lut As ShortLookupTable = createShortLut(scaleFactors, offsets, numBands, srcNgray)
					Dim op As New LookupOp(lut, hints)
					op.filter(src, dst)
				End If
			Else
				'
				' Fall back to the slow code
				'
				If length > 1 Then [step] = 1

				Dim sminX As Integer = src.minX
				Dim sY As Integer = src.minY
				Dim dminX As Integer = dst.minX
				Dim dY As Integer = dst.minY
				Dim sX As Integer
				Dim dX As Integer

				'
				'  Determine bits per band to determine maxval for clamps.
				'  The min is assumed to be zero.
				'  REMIND: This must change if we ever support signed data types.
				'
				Dim nbits As Integer
				Dim dstMax As Integer() = New Integer(numBands - 1){}
				Dim dstMask As Integer() = New Integer(numBands - 1){}
				Dim dstSM As SampleModel = dst.sampleModel
				For z As Integer = 0 To numBands - 1
					nbits = dstSM.getSampleSize(z)
					dstMax(z) = (1 << nbits) - 1
					dstMask(z) = Not(dstMax(z))
				Next z

				Dim val As Integer
				Dim y As Integer=0
				Do While y < height
					dX = dminX
					sX = sminX
					Dim x As Integer = 0
					Do While x < width
						' Get data for all bands at this x,y position
						srcPix = src.getPixel(sX, sY, srcPix)
						tidx = 0
						Dim z As Integer=0
						Do While z<numBands
							val = CInt(Fix(srcPix(z)*scaleFactors(tidx) + offsets(tidx)))
							' Clamp
							If (val And dstMask(z)) <> 0 Then
								If val < 0 Then
									val = 0
								Else
									val = dstMax(z)
								End If
							End If
							srcPix(z) = val

							z += 1
							tidx += [step]
						Loop

						' Put it back for all bands
						dst.pixelxel(dX, dY, srcPix)
						x += 1
						sX += 1
						dX += 1
					Loop
					y += 1
					sY += 1
					dY += 1
				Loop
			End If
			Return dst

		''' <summary>
		''' Returns the bounding box of the rescaled destination image.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change.
		''' </summary>
		public final java.awt.geom.Rectangle2D getBounds2D(BufferedImage src)
			 Return getBounds2D(src.raster)

		''' <summary>
		''' Returns the bounding box of the rescaled destination Raster.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change. </summary>
		''' <param name="src"> the rescaled destination <code>Raster</code> </param>
		''' <returns> the bounds of the specified <code>Raster</code>. </returns>
		public final java.awt.geom.Rectangle2D getBounds2D(Raster src)
			Return src.bounds

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number of
		''' bands. </summary>
		''' <param name="src">       Source image for the filter operation. </param>
		''' <param name="destCM">    ColorModel of the destination.  If null, the
		'''                  ColorModel of the source will be used. </param>
		''' <returns> the zeroed-destination image. </returns>
		public BufferedImage createCompatibleDestImage(BufferedImage src, ColorModel destCM)
			Dim image_Renamed As BufferedImage
			If destCM Is Nothing Then
				Dim cm As ColorModel = src.colorModel
				image_Renamed = New BufferedImage(cm, src.raster.createCompatibleWritableRaster(), cm.alphaPremultiplied, Nothing)
			Else
				Dim w As Integer = src.width
				Dim h As Integer = src.height
				image_Renamed = New BufferedImage(destCM, destCM.createCompatibleWritableRaster(w, h), destCM.alphaPremultiplied, Nothing)
			End If

			Return image_Renamed

		''' <summary>
		''' Creates a zeroed-destination <code>Raster</code> with the correct
		''' size and number of bands, given this source. </summary>
		''' <param name="src">       the source <code>Raster</code> </param>
		''' <returns> the zeroed-destination <code>Raster</code>. </returns>
		public WritableRaster createCompatibleDestRaster(Raster src)
			Return src.createCompatibleWritableRaster(src.width, src.height)

		''' <summary>
		''' Returns the location of the destination point given a
		''' point in the source.  If dstPt is non-null, it will
		''' be used to hold the return value.  Since this is not a geometric
		''' operation, the srcPt will equal the dstPt. </summary>
		''' <param name="srcPt"> a point in the source image </param>
		''' <param name="dstPt"> the destination point or <code>null</code> </param>
		''' <returns> the location of the destination point. </returns>
		public final java.awt.geom.Point2D getPoint2D(java.awt.geom.Point2D srcPt, java.awt.geom.Point2D dstPt)
			If dstPt Is Nothing Then dstPt = New java.awt.geom.Point2D.Float
			dstPt.locationion(srcPt.x, srcPt.y)
			Return dstPt

		''' <summary>
		''' Returns the rendering hints for this op. </summary>
		''' <returns> the rendering hints of this <code>RescaleOp</code>. </returns>
		public final java.awt.RenderingHints renderingHints
			Return hints
	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularSByteArray(  Size1 As Integer,   Size2 As Integer) As SByte()()
        Dim Array As SByte()() = New SByte(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New SByte(Size2 - 1) {}
        Next Array1
        Return Array
    End Function

    Friend Shared Function ReturnRectangularShortArray(  Size1 As Integer,   Size2 As Integer) As Short()()
        Dim Array As Short()() = New Short(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Short(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class