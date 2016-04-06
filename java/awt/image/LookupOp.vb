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
	''' This class implements a lookup operation from the source
	''' to the destination.  The LookupTable object may contain a single array
	''' or multiple arrays, subject to the restrictions below.
	''' <p>
	''' For Rasters, the lookup operates on bands.  The number of
	''' lookup arrays may be one, in which case the same array is
	''' applied to all bands, or it must equal the number of Source
	''' Raster bands.
	''' <p>
	''' For BufferedImages, the lookup operates on color and alpha components.
	''' The number of lookup arrays may be one, in which case the
	''' same array is applied to all color (but not alpha) components.
	''' Otherwise, the number of lookup arrays may
	''' equal the number of Source color components, in which case no
	''' lookup of the alpha component (if present) is performed.
	''' If neither of these cases apply, the number of lookup arrays
	''' must equal the number of Source color components plus alpha components,
	''' in which case lookup is performed for all color and alpha components.
	''' This allows non-uniform rescaling of multi-band BufferedImages.
	''' <p>
	''' BufferedImage sources with premultiplied alpha data are treated in the same
	''' manner as non-premultiplied images for purposes of the lookup.  That is,
	''' the lookup is done per band on the raw data of the BufferedImage source
	''' without regard to whether the data is premultiplied.  If a color conversion
	''' is required to the destination ColorModel, the premultiplied state of
	''' both source and destination will be taken into account for this step.
	''' <p>
	''' Images with an IndexColorModel cannot be used.
	''' <p>
	''' If a RenderingHints object is specified in the constructor, the
	''' color rendering hint and the dithering hint may be used when color
	''' conversion is required.
	''' <p>
	''' This class allows the Source to be the same as the Destination.
	''' </summary>
	''' <seealso cref= LookupTable </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_COLOR_RENDERING </seealso>
	''' <seealso cref= java.awt.RenderingHints#KEY_DITHERING </seealso>

	Public Class LookupOp
		Implements BufferedImageOp, RasterOp

		Private ltable As LookupTable
		Private numComponents As Integer
		Friend hints As java.awt.RenderingHints

		''' <summary>
		''' Constructs a <code>LookupOp</code> object given the lookup
		''' table and a <code>RenderingHints</code> object, which might
		''' be <code>null</code>. </summary>
		''' <param name="lookup"> the specified <code>LookupTable</code> </param>
		''' <param name="hints"> the specified <code>RenderingHints</code>,
		'''        or <code>null</code> </param>
		Public Sub New(  lookup As LookupTable,   hints As java.awt.RenderingHints)
			Me.ltable = lookup
			Me.hints = hints
			numComponents = ltable.numComponents
		End Sub

		''' <summary>
		''' Returns the <code>LookupTable</code>. </summary>
		''' <returns> the <code>LookupTable</code> of this
		'''         <code>LookupOp</code>. </returns>
		Public Property table As LookupTable
			Get
				Return ltable
			End Get
		End Property

		''' <summary>
		''' Performs a lookup operation on a <code>BufferedImage</code>.
		''' If the color model in the source image is not the same as that
		''' in the destination image, the pixels will be converted
		''' in the destination.  If the destination image is <code>null</code>,
		''' a <code>BufferedImage</code> will be created with an appropriate
		''' <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		''' might be thrown if the number of arrays in the
		''' <code>LookupTable</code> does not meet the restrictions
		''' stated in the class comment above, or if the source image
		''' has an <code>IndexColorModel</code>. </summary>
		''' <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		''' <param name="dst"> the <code>BufferedImage</code> in which to
		'''            store the results of the filter operation </param>
		''' <returns> the filtered <code>BufferedImage</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if the number of arrays in the
		'''         <code>LookupTable</code> does not meet the restrictions
		'''         described in the class comments, or if the source image
		'''         has an <code>IndexColorModel</code>. </exception>
		Public Function filter(  src As BufferedImage,   dst As BufferedImage) As BufferedImage Implements BufferedImageOp.filter
			Dim srcCM As ColorModel = src.colorModel
			Dim numBands As Integer = srcCM.numColorComponents
			Dim dstCM As ColorModel
			If TypeOf srcCM Is IndexColorModel Then Throw New IllegalArgumentException("LookupOp cannot be " & "performed on an indexed image")
			Dim numComponents As Integer = ltable.numComponents
			If numComponents <> 1 AndAlso numComponents <> srcCM.numComponents AndAlso numComponents <> srcCM.numColorComponents Then Throw New IllegalArgumentException("Number of arrays in the " & " lookup table (" & numComponents & " is not compatible with the " & " src image: " & src)


			Dim needToConvert As Boolean = False

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

			If sun.awt.image.ImagingLib.filter(Me, src, dst) Is Nothing Then
				' Do it the slow way
				Dim srcRaster As WritableRaster = src.raster
				Dim dstRaster As WritableRaster = dst.raster

				If srcCM.hasAlpha() Then
					If numBands-1 = numComponents OrElse numComponents = 1 Then
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
					If dstNumBands-1 = numComponents OrElse numComponents = 1 Then
						Dim minx As Integer = dstRaster.minX
						Dim miny As Integer = dstRaster.minY
						Dim bands As Integer() = New Integer(numBands-2){}
						For i As Integer = 0 To numBands-2
							bands(i) = i
						Next i
						dstRaster = dstRaster.createWritableChild(minx, miny, dstRaster.width, dstRaster.height, minx, miny, bands)
					End If
				End If

				filter(srcRaster, dstRaster)
			End If

			If needToConvert Then
				' ColorModels are not the same
				Dim ccop As New ColorConvertOp(hints)
				ccop.filter(dst, origDst)
			End If

			Return origDst
		End Function

		''' <summary>
		''' Performs a lookup operation on a <code>Raster</code>.
		''' If the destination <code>Raster</code> is <code>null</code>,
		''' a new <code>Raster</code> will be created.
		''' The <code>IllegalArgumentException</code> might be thrown
		''' if the source <code>Raster</code> and the destination
		''' <code>Raster</code> do not have the same
		''' number of bands or if the number of arrays in the
		''' <code>LookupTable</code> does not meet the
		''' restrictions stated in the class comment above. </summary>
		''' <param name="src"> the source <code>Raster</code> to filter </param>
		''' <param name="dst"> the destination <code>WritableRaster</code> for the
		'''            filtered <code>src</code> </param>
		''' <returns> the filtered <code>WritableRaster</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if the source and destinations
		'''         rasters do not have the same number of bands, or the
		'''         number of arrays in the <code>LookupTable</code> does
		'''         not meet the restrictions described in the class comments.
		'''  </exception>
		Public Function filter(  src As Raster,   dst As WritableRaster) As WritableRaster Implements RasterOp.filter
			Dim numBands As Integer = src.numBands
			Dim dstLength As Integer = dst.numBands
			Dim height As Integer = src.height
			Dim width As Integer = src.width
			Dim srcPix As Integer() = New Integer(numBands - 1){}

			' Create a new destination Raster, if needed

			If dst Is Nothing Then
				dst = createCompatibleDestRaster(src)
			ElseIf height <> dst.height OrElse width <> dst.width Then
				Throw New IllegalArgumentException("Width or height of Rasters do not " & "match")
			End If
			dstLength = dst.numBands

			If numBands <> dstLength Then Throw New IllegalArgumentException("Number of channels in the src (" & numBands & ") does not match number of channels" & " in the destination (" & dstLength & ")")
			Dim numComponents As Integer = ltable.numComponents
			If numComponents <> 1 AndAlso numComponents <> src.numBands Then Throw New IllegalArgumentException("Number of arrays in the " & " lookup table (" & numComponents & " is not compatible with the " & " src Raster: " & src)


			If sun.awt.image.ImagingLib.filter(Me, src, dst) IsNot Nothing Then Return dst

			' Optimize for cases we know about
			If TypeOf ltable Is ByteLookupTable Then
				byteFilter(CType(ltable, ByteLookupTable), src, dst, width, height, numBands)
			ElseIf TypeOf ltable Is ShortLookupTable Then
				shortFilter(CType(ltable, ShortLookupTable), src, dst, width, height, numBands)
			Else
				' Not one we recognize so do it slowly
				Dim sminX As Integer = src.minX
				Dim sY As Integer = src.minY
				Dim dminX As Integer = dst.minX
				Dim dY As Integer = dst.minY
				Dim y As Integer=0
				Do While y < height
					Dim sX As Integer = sminX
					Dim dX As Integer = dminX
					Dim x As Integer=0
					Do While x < width
						' Find data for all bands at this x,y position
						src.getPixel(sX, sY, srcPix)

						' Lookup the data for all bands at this x,y position
						ltable.lookupPixel(srcPix, srcPix)

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
		End Function

		''' <summary>
		''' Returns the bounding box of the filtered destination image.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change. </summary>
		''' <param name="src"> the <code>BufferedImage</code> to be filtered </param>
		''' <returns> the bounds of the filtered definition image. </returns>
		Public Function getBounds2D(  src As BufferedImage) As java.awt.geom.Rectangle2D Implements BufferedImageOp.getBounds2D
			Return getBounds2D(src.raster)
		End Function

		''' <summary>
		''' Returns the bounding box of the filtered destination Raster.  Since
		''' this is not a geometric operation, the bounding box does not
		''' change. </summary>
		''' <param name="src"> the <code>Raster</code> to be filtered </param>
		''' <returns> the bounds of the filtered definition <code>Raster</code>. </returns>
		Public Function getBounds2D(  src As Raster) As java.awt.geom.Rectangle2D Implements RasterOp.getBounds2D
			Return src.bounds

		End Function

		''' <summary>
		''' Creates a zeroed destination image with the correct size and number of
		''' bands.  If destCM is <code>null</code>, an appropriate
		''' <code>ColorModel</code> will be used. </summary>
		''' <param name="src">       Source image for the filter operation. </param>
		''' <param name="destCM">    the destination's <code>ColorModel</code>, which
		'''                  can be <code>null</code>. </param>
		''' <returns> a filtered destination <code>BufferedImage</code>. </returns>
		Public Overridable Function createCompatibleDestImage(  src As BufferedImage,   destCM As ColorModel) As BufferedImage Implements BufferedImageOp.createCompatibleDestImage
			Dim image_Renamed As BufferedImage
			Dim w As Integer = src.width
			Dim h As Integer = src.height
			Dim transferType As Integer = DataBuffer.TYPE_BYTE
			If destCM Is Nothing Then
				Dim cm As ColorModel = src.colorModel
				Dim raster_Renamed As Raster = src.raster
				If TypeOf cm Is ComponentColorModel Then
					Dim db As DataBuffer = raster_Renamed.dataBuffer
					Dim hasAlpha As Boolean = cm.hasAlpha()
					Dim isPre As Boolean = cm.alphaPremultiplied
					Dim trans As Integer = cm.transparency
					Dim nbits As Integer() = Nothing
					If TypeOf ltable Is ByteLookupTable Then
						If db.dataType = db.TYPE_USHORT Then
							' Dst raster should be of type byte
							If hasAlpha Then
								nbits = New Integer(1){}
								If trans = cm.BITMASK Then
									nbits(1) = 1
								Else
									nbits(1) = 8
								End If
							Else
								nbits = New Integer(0){}
							End If
							nbits(0) = 8
						End If
						' For byte, no need to change the cm
					ElseIf TypeOf ltable Is ShortLookupTable Then
						transferType = DataBuffer.TYPE_USHORT
						If db.dataType = db.TYPE_BYTE Then
							If hasAlpha Then
								nbits = New Integer(1){}
								If trans = cm.BITMASK Then
									nbits(1) = 1
								Else
									nbits(1) = 16
								End If
							Else
								nbits = New Integer(0){}
							End If
							nbits(0) = 16
						End If
					End If
					If nbits IsNot Nothing Then cm = New ComponentColorModel(cm.colorSpace, nbits, hasAlpha, isPre, trans, transferType)
				End If
				image_Renamed = New BufferedImage(cm, cm.createCompatibleWritableRaster(w, h), cm.alphaPremultiplied, Nothing)
			Else
				image_Renamed = New BufferedImage(destCM, destCM.createCompatibleWritableRaster(w, h), destCM.alphaPremultiplied, Nothing)
			End If

			Return image_Renamed
		End Function

		''' <summary>
		''' Creates a zeroed-destination <code>Raster</code> with the
		''' correct size and number of bands, given this source. </summary>
		''' <param name="src"> the <code>Raster</code> to be transformed </param>
		''' <returns> the zeroed-destination <code>Raster</code>. </returns>
		Public Overridable Function createCompatibleDestRaster(  src As Raster) As WritableRaster Implements RasterOp.createCompatibleDestRaster
			Return src.createCompatibleWritableRaster()
		End Function

		''' <summary>
		''' Returns the location of the destination point given a
		''' point in the source.  If <code>dstPt</code> is not
		''' <code>null</code>, it will be used to hold the return value.
		''' Since this is not a geometric operation, the <code>srcPt</code>
		''' will equal the <code>dstPt</code>. </summary>
		''' <param name="srcPt"> a <code>Point2D</code> that represents a point
		'''        in the source image </param>
		''' <param name="dstPt"> a <code>Point2D</code>that represents the location
		'''        in the destination </param>
		''' <returns> the <code>Point2D</code> in the destination that
		'''         corresponds to the specified point in the source. </returns>
		Public Function getPoint2D(  srcPt As java.awt.geom.Point2D,   dstPt As java.awt.geom.Point2D) As java.awt.geom.Point2D Implements BufferedImageOp.getPoint2D, RasterOp.getPoint2D
			If dstPt Is Nothing Then dstPt = New java.awt.geom.Point2D.Float
			dstPt.locationion(srcPt.x, srcPt.y)

			Return dstPt
		End Function

		''' <summary>
		''' Returns the rendering hints for this op. </summary>
		''' <returns> the <code>RenderingHints</code> object associated
		'''         with this op. </returns>
		Public Property renderingHints As java.awt.RenderingHints Implements BufferedImageOp.getRenderingHints, RasterOp.getRenderingHints
			Get
				Return hints
			End Get
		End Property

		Private Sub byteFilter(  lookup As ByteLookupTable,   src As Raster,   dst As WritableRaster,   width As Integer,   height As Integer,   numBands As Integer)
			Dim srcPix As Integer() = Nothing

			' Find the ref to the table and the offset
			Dim table_Renamed As SByte()() = lookup.table
			Dim offset As Integer = lookup.offset
			Dim tidx As Integer
			Dim [step] As Integer=1

			' Check if it is one lookup applied to all bands
			If table_Renamed.Length = 1 Then [step]=0

			Dim x As Integer
			Dim y As Integer
			Dim band As Integer
			Dim len As Integer = table_Renamed(0).Length

			' Loop through the data
			For y = 0 To height - 1
				tidx = 0
				band=0
				Do While band < numBands
					' Find data for this band, scanline
					srcPix = src.getSamples(0, y, width, 1, band, srcPix)

					For x = 0 To width - 1
						Dim index As Integer = srcPix(x)-offset
						If index < 0 OrElse index > len Then Throw New IllegalArgumentException("index (" & index & "(out of range: " & " srcPix[" & x & "]=" & srcPix(x) & " offset=" & offset)
						' Do the lookup
						srcPix(x) = table_Renamed(tidx)(index)
					Next x
					' Put it back
					dst.samplesles(0, y, width, 1, band, srcPix)
					band += 1
					tidx+=[step]
				Loop
			Next y
		End Sub

		Private Sub shortFilter(  lookup As ShortLookupTable,   src As Raster,   dst As WritableRaster,   width As Integer,   height As Integer,   numBands As Integer)
			Dim band As Integer
			Dim srcPix As Integer() = Nothing

			' Find the ref to the table and the offset
			Dim table_Renamed As Short()() = lookup.table
			Dim offset As Integer = lookup.offset
			Dim tidx As Integer
			Dim [step] As Integer=1

			' Check if it is one lookup applied to all bands
			If table_Renamed.Length = 1 Then [step]=0

			Dim x As Integer = 0
			Dim y As Integer = 0
			Dim index As Integer
			Dim maxShort As Integer = (1<<16)-1
			' Loop through the data
			For y = 0 To height - 1
				tidx = 0
				band=0
				Do While band < numBands
					' Find data for this band, scanline
					srcPix = src.getSamples(0, y, width, 1, band, srcPix)

					For x = 0 To width - 1
						index = srcPix(x)-offset
						If index < 0 OrElse index > maxShort Then Throw New IllegalArgumentException("index out of range " & index & " x is " & x & "srcPix[x]=" & srcPix(x) & " offset=" & offset)
						' Do the lookup
						srcPix(x) = table_Renamed(tidx)(index)
					Next x
					' Put it back
					dst.samplesles(0, y, width, 1, band, srcPix)
					band += 1
					tidx+=[step]
				Loop
			Next y
		End Sub
	End Class

End Namespace