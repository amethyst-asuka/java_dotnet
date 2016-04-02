Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

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

' ****************************************************************
' ******************************************************************
' ******************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997
' *** As  an unpublished  work pursuant to Title 17 of the United
' *** States Code.  All rights reserved.
' ******************************************************************
' ******************************************************************
' *****************************************************************


Namespace java.awt.image


	''' <summary>
	''' A class representing a rectangular array of pixels.  A Raster
	''' encapsulates a DataBuffer that stores the sample values and a
	''' SampleModel that describes how to locate a given sample value in a
	''' DataBuffer.
	''' <p>
	''' A Raster defines values for pixels occupying a particular
	''' rectangular area of the plane, not necessarily including (0, 0).
	''' The rectangle, known as the Raster's bounding rectangle and
	''' available by means of the getBounds method, is defined by minX,
	''' minY, width, and height values.  The minX and minY values define
	''' the coordinate of the upper left corner of the Raster.  References
	''' to pixels outside of the bounding rectangle may result in an
	''' exception being thrown, or may result in references to unintended
	''' elements of the Raster's associated DataBuffer.  It is the user's
	''' responsibility to avoid accessing such pixels.
	''' <p>
	''' A SampleModel describes how samples of a Raster
	''' are stored in the primitive array elements of a DataBuffer.
	''' Samples may be stored one per data element, as in a
	''' PixelInterleavedSampleModel or BandedSampleModel, or packed several to
	''' an element, as in a SinglePixelPackedSampleModel or
	''' MultiPixelPackedSampleModel.  The SampleModel is also
	''' controls whether samples are sign extended, allowing unsigned
	''' data to be stored in signed Java data types such as byte, short, and
	''' int.
	''' <p>
	''' Although a Raster may live anywhere in the plane, a SampleModel
	''' makes use of a simple coordinate system that starts at (0, 0).  A
	''' Raster therefore contains a translation factor that allows pixel
	''' locations to be mapped between the Raster's coordinate system and
	''' that of the SampleModel.  The translation from the SampleModel
	''' coordinate system to that of the Raster may be obtained by the
	''' getSampleModelTranslateX and getSampleModelTranslateY methods.
	''' <p>
	''' A Raster may share a DataBuffer with another Raster either by
	''' explicit construction or by the use of the createChild and
	''' createTranslatedChild methods.  Rasters created by these methods
	''' can return a reference to the Raster they were created from by
	''' means of the getParent method.  For a Raster that was not
	''' constructed by means of a call to createTranslatedChild or
	''' createChild, getParent will return null.
	''' <p>
	''' The createTranslatedChild method returns a new Raster that
	''' shares all of the data of the current Raster, but occupies a
	''' bounding rectangle of the same width and height but with a
	''' different starting point.  For example, if the parent Raster
	''' occupied the region (10, 10) to (100, 100), and the translated
	''' Raster was defined to start at (50, 50), then pixel (20, 20) of the
	''' parent and pixel (60, 60) of the child occupy the same location in
	''' the DataBuffer shared by the two Rasters.  In the first case, (-10,
	''' -10) should be added to a pixel coordinate to obtain the
	''' corresponding SampleModel coordinate, and in the second case (-50,
	''' -50) should be added.
	''' <p>
	''' The translation between a parent and child Raster may be
	''' determined by subtracting the child's sampleModelTranslateX and
	''' sampleModelTranslateY values from those of the parent.
	''' <p>
	''' The createChild method may be used to create a new Raster
	''' occupying only a subset of its parent's bounding rectangle
	''' (with the same or a translated coordinate system) or
	''' with a subset of the bands of its parent.
	''' <p>
	''' All constructors are protected.  The correct way to create a
	''' Raster is to use one of the static create methods defined in this
	''' class.  These methods create instances of Raster that use the
	''' standard Interleaved, Banded, and Packed SampleModels and that may
	''' be processed more efficiently than a Raster created by combining
	''' an externally generated SampleModel and DataBuffer. </summary>
	''' <seealso cref= java.awt.image.DataBuffer </seealso>
	''' <seealso cref= java.awt.image.SampleModel </seealso>
	''' <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	''' <seealso cref= java.awt.image.BandedSampleModel </seealso>
	''' <seealso cref= java.awt.image.SinglePixelPackedSampleModel </seealso>
	''' <seealso cref= java.awt.image.MultiPixelPackedSampleModel </seealso>
	Public Class Raster

		''' <summary>
		''' The SampleModel that describes how pixels from this Raster
		''' are stored in the DataBuffer.
		''' </summary>
		Protected Friend sampleModel As SampleModel

		''' <summary>
		''' The DataBuffer that stores the image data. </summary>
		Protected Friend dataBuffer_Renamed As DataBuffer

		''' <summary>
		''' The X coordinate of the upper-left pixel of this Raster. </summary>
		Protected Friend minX As Integer

		''' <summary>
		''' The Y coordinate of the upper-left pixel of this Raster. </summary>
		Protected Friend minY As Integer

		''' <summary>
		''' The width of this Raster. </summary>
		Protected Friend width As Integer

		''' <summary>
		''' The height of this Raster. </summary>
		Protected Friend height As Integer

		''' <summary>
		''' The X translation from the coordinate space of the
		''' Raster's SampleModel to that of the Raster.
		''' </summary>
		Protected Friend sampleModelTranslateX As Integer

		''' <summary>
		''' The Y translation from the coordinate space of the
		''' Raster's SampleModel to that of the Raster.
		''' </summary>
		Protected Friend sampleModelTranslateY As Integer

		''' <summary>
		''' The number of bands in the Raster. </summary>
		Protected Friend numBands As Integer

		''' <summary>
		''' The number of DataBuffer data elements per pixel. </summary>
		Protected Friend numDataElements As Integer

		''' <summary>
		''' The parent of this Raster, or null. </summary>
		Protected Friend parent As Raster

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Creates a Raster based on a PixelInterleavedSampleModel with the
		''' specified data type, width, height, and number of bands.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used.
		''' The dataType parameter should be one of the enumerated values
		''' defined in the DataBuffer class.
		''' 
		''' <p> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		''' Rasters are not supported.  To create a 1-band Raster of type
		''' <code>DataBuffer.TYPE_INT</code>, use
		''' Raster.createPackedRaster().
		''' <p> The only dataTypes supported currently are TYPE_BYTE
		''' and TYPE_U java.lang.[Short]. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="bands">     the number of bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height and number of bands. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		Public Shared Function createInterleavedRaster(ByVal dataType As Integer, ByVal w As Integer, ByVal h As Integer, ByVal bands As Integer, ByVal location As java.awt.Point) As WritableRaster
			Dim bandOffsets As Integer() = New Integer(bands - 1){}
			For i As Integer = 0 To bands - 1
				bandOffsets(i) = i
			Next i
			Return createInterleavedRaster(dataType, w, h, w*bands, bands, bandOffsets, location)
		End Function

		''' <summary>
		''' Creates a Raster based on a PixelInterleavedSampleModel with the
		''' specified data type, width, height, scanline stride, pixel
		''' stride, and band offsets.  The number of bands is inferred from
		''' bandOffsets.length.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used.
		''' The dataType parameter should be one of the enumerated values
		''' defined in the DataBuffer class.
		''' 
		''' <p> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		''' Rasters are not supported.  To create a 1-band Raster of type
		''' <code>DataBuffer.TYPE_INT</code>, use
		''' Raster.createPackedRaster().
		''' <p> The only dataTypes supported currently are TYPE_BYTE
		''' and TYPE_U java.lang.[Short]. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="pixelStride"> the pixel stride of the image data </param>
		''' <param name="bandOffsets"> the offsets of all bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height, scanline stride, pixel stride and band
		'''         offsets. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>, or
		'''         <code>DataBuffer.TYPE_USHORT</code>. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		Public Shared WritableRaster createInterleavedRaster(int dataType, int w, int h, int scanlineStride, int pixelStride, int bandOffsets() , java.awt.Point location)
			Dim d As DataBuffer

			Dim size As Integer = scanlineStride * (h - 1) + pixelStride * w ' last scan -  fisrt (h - 1) scans

			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				d = New DataBufferByte(size)

			Case DataBuffer.TYPE_USHORT
				d = New DataBufferUShort(size)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

			Return createInterleavedRaster(d, w, h, scanlineStride, pixelStride, bandOffsets, location)

		''' <summary>
		''' Creates a Raster based on a BandedSampleModel with the
		''' specified data type, width, height, and number of bands.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used.
		''' The dataType parameter should be one of the enumerated values
		''' defined in the DataBuffer class.
		''' 
		''' <p> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		''' and TYPE_INT. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="bands">     the number of bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height and number of bands. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>bands</code>
		'''         is less than 1 </exception>
		Public Shared WritableRaster createBandedRaster(Integer dataType, Integer w, Integer h, Integer bands, java.awt.Point location)
			If bands < 1 Then Throw New ArrayIndexOutOfBoundsException("Number of bands (" & bands & ") must" & " be greater than 0")
			Dim bankIndices As Integer() = New Integer(bands - 1){}
			Dim bandOffsets As Integer() = New Integer(bands - 1){}
			For i As Integer = 0 To bands - 1
				bankIndices(i) = i
				bandOffsets(i) = 0
			Next i

			Return createBandedRaster(dataType, w, h, w, bankIndices, bandOffsets, location)

		''' <summary>
		''' Creates a Raster based on a BandedSampleModel with the
		''' specified data type, width, height, scanline stride, bank
		''' indices and band offsets.  The number of bands is inferred from
		''' bankIndices.length and bandOffsets.length, which must be the
		''' same.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  The dataType parameter should be one of the
		''' enumerated values defined in the DataBuffer class.
		''' 
		''' <p> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		''' and TYPE_INT. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="bankIndices"> the bank indices for each band </param>
		''' <param name="bandOffsets"> the offsets of all bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height, scanline stride, bank indices and band
		'''         offsets. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>bankIndices</code>
		'''         or <code>bandOffsets</code> is <code>null</code> </exception>
		Public Shared WritableRaster createBandedRaster(Integer dataType, Integer w, Integer h, Integer scanlineStride, Integer bankIndices() , Integer bandOffsets(), java.awt.Point location)
			Dim d As DataBuffer
			Dim bands As Integer = bandOffsets.length

			If bankIndices Is Nothing Then Throw New ArrayIndexOutOfBoundsException("Bank indices array is null")
			If bandOffsets Is Nothing Then Throw New ArrayIndexOutOfBoundsException("Band offsets array is null")

			' Figure out the #banks and the largest band offset
			Dim maxBank As Integer = bankIndices(0)
			Dim maxBandOff As Integer = bandOffsets(0)
			For i As Integer = 1 To bands - 1
				If bankIndices(i) > maxBank Then maxBank = bankIndices(i)
				If bandOffsets(i) > maxBandOff Then maxBandOff = bandOffsets(i)
			Next i
			Dim banks As Integer = maxBank + 1
			Dim size As Integer = maxBandOff + scanlineStride * (h - 1) + w ' last scan -  fisrt (h - 1) scans

			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				d = New DataBufferByte(size, banks)

			Case DataBuffer.TYPE_USHORT
				d = New DataBufferUShort(size, banks)

			Case DataBuffer.TYPE_INT
				d = New DataBufferInt(size, banks)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

			Return createBandedRaster(d, w, h, scanlineStride, bankIndices, bandOffsets, location)

		''' <summary>
		''' Creates a Raster based on a SinglePixelPackedSampleModel with
		''' the specified data type, width, height, and band masks.
		''' The number of bands is inferred from bandMasks.length.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used.
		''' The dataType parameter should be one of the enumerated values
		''' defined in the DataBuffer class.
		''' 
		''' <p> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		''' and TYPE_INT. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="bandMasks"> an array containing an entry for each band </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height, and band masks. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		Public Shared WritableRaster createPackedRaster(Integer dataType, Integer w, Integer h, Integer bandMasks() , java.awt.Point location)
			Dim d As DataBuffer

			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				d = New DataBufferByte(w*h)

			Case DataBuffer.TYPE_USHORT
				d = New DataBufferUShort(w*h)

			Case DataBuffer.TYPE_INT
				d = New DataBufferInt(w*h)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

			Return createPackedRaster(d, w, h, w, bandMasks, location)

		''' <summary>
		''' Creates a Raster based on a packed SampleModel with the
		''' specified data type, width, height, number of bands, and bits
		''' per band.  If the number of bands is one, the SampleModel will
		''' be a MultiPixelPackedSampleModel.
		''' 
		''' <p> If the number of bands is more than one, the SampleModel
		''' will be a SinglePixelPackedSampleModel, with each band having
		''' bitsPerBand bits.  In either case, the requirements on dataType
		''' and bitsPerBand imposed by the corresponding SampleModel must
		''' be met.
		''' 
		''' <p> The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used.
		''' The dataType parameter should be one of the enumerated values
		''' defined in the DataBuffer class.
		''' 
		''' <p> The only dataTypes supported currently are TYPE_BYTE, TYPE_USHORT,
		''' and TYPE_INT. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="bands">     the number of bands </param>
		''' <param name="bitsPerBand"> the number of bits per band </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified data type,
		'''         width, height, number of bands, and bits per band. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if the product of
		'''         <code>bitsPerBand</code> and <code>bands</code> is
		'''         greater than the number of bits held by
		'''         <code>dataType</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bitsPerBand</code> or
		'''         <code>bands</code> is not greater than zero </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		Public Shared WritableRaster createPackedRaster(Integer dataType, Integer w, Integer h, Integer bands, Integer bitsPerBand, java.awt.Point location)
			Dim d As DataBuffer

			If bands <= 0 Then Throw New IllegalArgumentException("Number of bands (" & bands & ") must be greater than 0")

			If bitsPerBand <= 0 Then Throw New IllegalArgumentException("Bits per band (" & bitsPerBand & ") must be greater than 0")

			If bands <> 1 Then
				Dim masks As Integer() = New Integer(bands - 1){}
				Dim mask As Integer = (1 << bitsPerBand) - 1
				Dim shift As Integer = (bands-1)*bitsPerBand

				' Make sure the total mask size will fit in the data type 
				If shift+bitsPerBand > DataBuffer.getDataTypeSize(dataType) Then Throw New IllegalArgumentException("bitsPerBand(" & bitsPerBand & ") * bands is " & " greater than data type " & "size.")
				Select Case dataType
				Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT
				Case Else
					Throw New IllegalArgumentException("Unsupported data type " & dataType)
				End Select

				For i As Integer = 0 To bands - 1
					masks(i) = mask << shift
					shift = shift - bitsPerBand
				Next i

				Return createPackedRaster(dataType, w, h, masks, location)
			Else
				Dim fw As Double = w
				Select Case dataType
				Case DataBuffer.TYPE_BYTE
					d = New DataBufferByte(CInt(Fix (System.Math.Ceiling(fw/(8/bitsPerBand))))*h)

				Case DataBuffer.TYPE_USHORT
					d = New DataBufferUShort(CInt(Fix (System.Math.Ceiling(fw/(16/bitsPerBand))))*h)

				Case DataBuffer.TYPE_INT
					d = New DataBufferInt(CInt(Fix (System.Math.Ceiling(fw/(32/bitsPerBand))))*h)

				Case Else
					Throw New IllegalArgumentException("Unsupported data type " & dataType)
				End Select

				Return createPackedRaster(d, w, h, bitsPerBand, location)
			End If

		''' <summary>
		''' Creates a Raster based on a PixelInterleavedSampleModel with the
		''' specified DataBuffer, width, height, scanline stride, pixel
		''' stride, and band offsets.  The number of bands is inferred from
		''' bandOffsets.length.  The upper left corner of the Raster
		''' is given by the location argument.  If location is null, (0, 0)
		''' will be used.
		''' <p> Note that interleaved <code>DataBuffer.TYPE_INT</code>
		''' Rasters are not supported.  To create a 1-band Raster of type
		''' <code>DataBuffer.TYPE_INT</code>, use
		''' Raster.createPackedRaster(). </summary>
		''' <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		'''        image data </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="pixelStride"> the pixel stride of the image data </param>
		''' <param name="bandOffsets"> the offsets of all bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified
		'''         <code>DataBuffer</code>, width, height, scanline stride,
		'''         pixel stride and band offsets. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code> </exception>
		''' <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		'''         than one bank. </exception>
		''' <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		Public Shared WritableRaster createInterleavedRaster(DataBuffer dataBuffer_Renamed, Integer w, Integer h, Integer scanlineStride, Integer pixelStride, Integer bandOffsets() , java.awt.Point location)
			If dataBuffer_Renamed Is Nothing Then Throw New NullPointerException("DataBuffer cannot be null")
			If location Is Nothing Then location = New java.awt.Point(0, 0)
			Dim dataType As Integer = dataBuffer_Renamed.dataType

			Dim csm As New PixelInterleavedSampleModel(dataType, w, h, pixelStride, scanlineStride, bandOffsets)
			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				Return New sun.awt.image.ByteInterleavedRaster(csm, dataBuffer_Renamed, location)

			Case DataBuffer.TYPE_USHORT
				Return New sun.awt.image.ShortInterleavedRaster(csm, dataBuffer_Renamed, location)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

		''' <summary>
		''' Creates a Raster based on a BandedSampleModel with the
		''' specified DataBuffer, width, height, scanline stride, bank
		''' indices, and band offsets.  The number of bands is inferred
		''' from bankIndices.length and bandOffsets.length, which must be
		''' the same.  The upper left corner of the Raster is given by the
		''' location argument.  If location is null, (0, 0) will be used. </summary>
		''' <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		'''        image data </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="bankIndices"> the bank indices for each band </param>
		''' <param name="bandOffsets"> the offsets of all bands </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified
		'''         <code>DataBuffer</code>, width, height, scanline stride,
		'''         bank indices and band offsets. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		''' <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		Public Shared WritableRaster createBandedRaster(DataBuffer dataBuffer_Renamed, Integer w, Integer h, Integer scanlineStride, Integer bankIndices() , Integer bandOffsets(), java.awt.Point location)
			If dataBuffer_Renamed Is Nothing Then Throw New NullPointerException("DataBuffer cannot be null")
			If location Is Nothing Then location = New java.awt.Point(0,0)
			Dim dataType As Integer = dataBuffer_Renamed.dataType

			Dim bands As Integer = bankIndices.length
			If bandOffsets.length <> bands Then Throw New IllegalArgumentException("bankIndices.length != bandOffsets.length")

			Dim bsm As New BandedSampleModel(dataType, w, h, scanlineStride, bankIndices, bandOffsets)

			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				Return New sun.awt.image.ByteBandedRaster(bsm, dataBuffer_Renamed, location)

			Case DataBuffer.TYPE_USHORT
				Return New sun.awt.image.ShortBandedRaster(bsm, dataBuffer_Renamed, location)

			Case DataBuffer.TYPE_INT
				Return New sun.awt.image.SunWritableRaster(bsm, dataBuffer_Renamed, location)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

		''' <summary>
		''' Creates a Raster based on a SinglePixelPackedSampleModel with
		''' the specified DataBuffer, width, height, scanline stride, and
		''' band masks.  The number of bands is inferred from bandMasks.length.
		''' The upper left corner of the Raster is given by
		''' the location argument.  If location is null, (0, 0) will be used. </summary>
		''' <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		'''        image data </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="bandMasks"> an array containing an entry for each band </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified
		'''         <code>DataBuffer</code>, width, height, scanline stride,
		'''         and band masks. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		''' <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		'''         than one bank. </exception>
		''' <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		Public Shared WritableRaster createPackedRaster(DataBuffer dataBuffer_Renamed, Integer w, Integer h, Integer scanlineStride, Integer bandMasks() , java.awt.Point location)
			If dataBuffer_Renamed Is Nothing Then Throw New NullPointerException("DataBuffer cannot be null")
			If location Is Nothing Then location = New java.awt.Point(0,0)
			Dim dataType As Integer = dataBuffer_Renamed.dataType

			Dim sppsm As New SinglePixelPackedSampleModel(dataType, w, h, scanlineStride, bandMasks)

			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				Return New sun.awt.image.ByteInterleavedRaster(sppsm, dataBuffer_Renamed, location)

			Case DataBuffer.TYPE_USHORT
				Return New sun.awt.image.ShortInterleavedRaster(sppsm, dataBuffer_Renamed, location)

			Case DataBuffer.TYPE_INT
				Return New sun.awt.image.IntegerInterleavedRaster(sppsm, dataBuffer_Renamed, location)

			Case Else
				Throw New IllegalArgumentException("Unsupported data type " & dataType)
			End Select

		''' <summary>
		''' Creates a Raster based on a MultiPixelPackedSampleModel with the
		''' specified DataBuffer, width, height, and bits per pixel.  The upper
		''' left corner of the Raster is given by the location argument.  If
		''' location is null, (0, 0) will be used. </summary>
		''' <param name="dataBuffer"> the <code>DataBuffer</code> that contains the
		'''        image data </param>
		''' <param name="w">         the width in pixels of the image data </param>
		''' <param name="h">         the height in pixels of the image data </param>
		''' <param name="bitsPerPixel"> the number of bits for each pixel </param>
		''' <param name="location">  the upper-left corner of the <code>Raster</code> </param>
		''' <returns> a WritableRaster object with the specified
		'''         <code>DataBuffer</code>, width, height, and
		'''         bits per pixel. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>location.x + w</code> or
		'''         <code>location.y + h</code> results in integer
		'''         overflow </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types, which are
		'''         <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>
		'''         or <code>DataBuffer.TYPE_INT</code> </exception>
		''' <exception cref="RasterFormatException"> if <code>dataBuffer</code> has more
		'''         than one bank. </exception>
		''' <exception cref="NullPointerException"> if <code>dataBuffer</code> is null </exception>
		Public Shared WritableRaster createPackedRaster(DataBuffer dataBuffer_Renamed, Integer w, Integer h, Integer bitsPerPixel, java.awt.Point location)
			If dataBuffer_Renamed Is Nothing Then Throw New NullPointerException("DataBuffer cannot be null")
			If location Is Nothing Then location = New java.awt.Point(0,0)
			Dim dataType As Integer = dataBuffer_Renamed.dataType

			If dataType <> DataBuffer.TYPE_BYTE AndAlso dataType <> DataBuffer.TYPE_USHORT AndAlso dataType <> DataBuffer.TYPE_INT Then Throw New IllegalArgumentException("Unsupported data type " & dataType)

			If dataBuffer_Renamed.numBanks <> 1 Then Throw New RasterFormatException("DataBuffer for packed Rasters" & " must only have 1 bank.")

			Dim mppsm As New MultiPixelPackedSampleModel(dataType, w, h, bitsPerPixel)

			If dataType = DataBuffer.TYPE_BYTE AndAlso (bitsPerPixel = 1 OrElse bitsPerPixel = 2 OrElse bitsPerPixel = 4) Then
				Return New sun.awt.image.BytePackedRaster(mppsm, dataBuffer_Renamed, location)
			Else
				Return New sun.awt.image.SunWritableRaster(mppsm, dataBuffer_Renamed, location)
			End If


		''' <summary>
		'''  Creates a Raster with the specified SampleModel and DataBuffer.
		'''  The upper left corner of the Raster is given by the location argument.
		'''  If location is null, (0, 0) will be used. </summary>
		'''  <param name="sm"> the specified <code>SampleModel</code> </param>
		'''  <param name="db"> the specified <code>DataBuffer</code> </param>
		'''  <param name="location"> the upper-left corner of the <code>Raster</code> </param>
		'''  <returns> a <code>Raster</code> with the specified
		'''          <code>SampleModel</code>, <code>DataBuffer</code>, and
		'''          location. </returns>
		''' <exception cref="RasterFormatException"> if computing either
		'''         <code>location.x + sm.getWidth()</code> or
		'''         <code>location.y + sm.getHeight()</code> results in integer
		'''         overflow </exception>
		''' <exception cref="RasterFormatException"> if <code>db</code> has more
		'''         than one bank and <code>sm</code> is a
		'''         PixelInterleavedSampleModel, SinglePixelPackedSampleModel,
		'''         or MultiPixelPackedSampleModel. </exception>
		'''  <exception cref="NullPointerException"> if either SampleModel or DataBuffer is
		'''          null </exception>
		Public Shared Raster createRaster(SampleModel sm, DataBuffer db, java.awt.Point location)
			If (sm Is Nothing) OrElse (db Is Nothing) Then Throw New NullPointerException("SampleModel and DataBuffer cannot be null")

			If location Is Nothing Then location = New java.awt.Point(0,0)
			Dim dataType As Integer = sm.dataType

			If TypeOf sm Is PixelInterleavedSampleModel Then
				Select Case dataType
					Case DataBuffer.TYPE_BYTE
						Return New sun.awt.image.ByteInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_USHORT
						Return New sun.awt.image.ShortInterleavedRaster(sm, db, location)
				End Select
			ElseIf TypeOf sm Is SinglePixelPackedSampleModel Then
				Select Case dataType
					Case DataBuffer.TYPE_BYTE
						Return New sun.awt.image.ByteInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_USHORT
						Return New sun.awt.image.ShortInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_INT
						Return New sun.awt.image.IntegerInterleavedRaster(sm, db, location)
				End Select
			ElseIf TypeOf sm Is MultiPixelPackedSampleModel AndAlso dataType = DataBuffer.TYPE_BYTE AndAlso sm.getSampleSize(0) < 8 Then
				Return New sun.awt.image.BytePackedRaster(sm, db, location)
			End If

			' we couldn't do anything special - do the generic thing

			Return New Raster(sm,db,location)

		''' <summary>
		'''  Creates a WritableRaster with the specified SampleModel.
		'''  The upper left corner of the Raster is given by the location argument.
		'''  If location is null, (0, 0) will be used. </summary>
		'''  <param name="sm"> the specified <code>SampleModel</code> </param>
		'''  <param name="location"> the upper-left corner of the
		'''         <code>WritableRaster</code> </param>
		'''  <returns> a <code>WritableRaster</code> with the specified
		'''          <code>SampleModel</code> and location. </returns>
		'''  <exception cref="RasterFormatException"> if computing either
		'''          <code>location.x + sm.getWidth()</code> or
		'''          <code>location.y + sm.getHeight()</code> results in integer
		'''          overflow </exception>
		Public Shared WritableRaster createWritableRaster(SampleModel sm, java.awt.Point location)
			If location Is Nothing Then location = New java.awt.Point(0,0)

			Return createWritableRaster(sm, sm.createDataBuffer(), location)

		''' <summary>
		'''  Creates a WritableRaster with the specified SampleModel and DataBuffer.
		'''  The upper left corner of the Raster is given by the location argument.
		'''  If location is null, (0, 0) will be used. </summary>
		'''  <param name="sm"> the specified <code>SampleModel</code> </param>
		'''  <param name="db"> the specified <code>DataBuffer</code> </param>
		'''  <param name="location"> the upper-left corner of the
		'''         <code>WritableRaster</code> </param>
		'''  <returns> a <code>WritableRaster</code> with the specified
		'''          <code>SampleModel</code>, <code>DataBuffer</code>, and
		'''          location. </returns>
		''' <exception cref="RasterFormatException"> if computing either
		'''         <code>location.x + sm.getWidth()</code> or
		'''         <code>location.y + sm.getHeight()</code> results in integer
		'''         overflow </exception>
		''' <exception cref="RasterFormatException"> if <code>db</code> has more
		'''         than one bank and <code>sm</code> is a
		'''         PixelInterleavedSampleModel, SinglePixelPackedSampleModel,
		'''         or MultiPixelPackedSampleModel. </exception>
		''' <exception cref="NullPointerException"> if either SampleModel or DataBuffer is null </exception>
		Public Shared WritableRaster createWritableRaster(SampleModel sm, DataBuffer db, java.awt.Point location)
			If (sm Is Nothing) OrElse (db Is Nothing) Then Throw New NullPointerException("SampleModel and DataBuffer cannot be null")
			If location Is Nothing Then location = New java.awt.Point(0,0)

			Dim dataType As Integer = sm.dataType

			If TypeOf sm Is PixelInterleavedSampleModel Then
				Select Case dataType
					Case DataBuffer.TYPE_BYTE
						Return New sun.awt.image.ByteInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_USHORT
						Return New sun.awt.image.ShortInterleavedRaster(sm, db, location)
				End Select
			ElseIf TypeOf sm Is SinglePixelPackedSampleModel Then
				Select Case dataType
					Case DataBuffer.TYPE_BYTE
						Return New sun.awt.image.ByteInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_USHORT
						Return New sun.awt.image.ShortInterleavedRaster(sm, db, location)

					Case DataBuffer.TYPE_INT
						Return New sun.awt.image.IntegerInterleavedRaster(sm, db, location)
				End Select
			ElseIf TypeOf sm Is MultiPixelPackedSampleModel AndAlso dataType = DataBuffer.TYPE_BYTE AndAlso sm.getSampleSize(0) < 8 Then
				Return New sun.awt.image.BytePackedRaster(sm, db, location)
			End If

			' we couldn't do anything special - do the generic thing

			Return New sun.awt.image.SunWritableRaster(sm,db,location)

		''' <summary>
		'''  Constructs a Raster with the given SampleModel.  The Raster's
		'''  upper left corner is origin and it is the same size as the
		'''  SampleModel.  A DataBuffer large enough to describe the
		'''  Raster is automatically created. </summary>
		'''  <param name="sampleModel">     The SampleModel that specifies the layout </param>
		'''  <param name="origin">          The Point that specified the origin </param>
		'''  <exception cref="RasterFormatException"> if computing either
		'''          <code>origin.x + sampleModel.getWidth()</code> or
		'''          <code>origin.y + sampleModel.getHeight()</code> results in
		'''          integer overflow </exception>
		'''  <exception cref="NullPointerException"> either <code>sampleModel</code> or
		'''          <code>origin</code> is null </exception>
		protected Raster(SampleModel sampleModel, java.awt.Point origin)
			Me(sampleModel, sampleModel.createDataBuffer(), New java.awt.Rectangle(origin.x, origin.y, sampleModel.width, sampleModel.height), origin, Nothing)

		''' <summary>
		'''  Constructs a Raster with the given SampleModel and DataBuffer.
		'''  The Raster's upper left corner is origin and it is the same size
		'''  as the SampleModel.  The DataBuffer is not initialized and must
		'''  be compatible with SampleModel. </summary>
		'''  <param name="sampleModel">     The SampleModel that specifies the layout </param>
		'''  <param name="dataBuffer">      The DataBuffer that contains the image data </param>
		'''  <param name="origin">          The Point that specifies the origin </param>
		'''  <exception cref="RasterFormatException"> if computing either
		'''          <code>origin.x + sampleModel.getWidth()</code> or
		'''          <code>origin.y + sampleModel.getHeight()</code> results in
		'''          integer overflow </exception>
		'''  <exception cref="NullPointerException"> either <code>sampleModel</code> or
		'''          <code>origin</code> is null </exception>
		protected Raster(SampleModel sampleModel, DataBuffer dataBuffer_Renamed, java.awt.Point origin)
			Me(sampleModel, dataBuffer_Renamed, New java.awt.Rectangle(origin.x, origin.y, sampleModel.width, sampleModel.height), origin, Nothing)

		''' <summary>
		''' Constructs a Raster with the given SampleModel, DataBuffer, and
		''' parent.  aRegion specifies the bounding rectangle of the new
		''' Raster.  When translated into the base Raster's coordinate
		''' system, aRegion must be contained by the base Raster.
		''' (The base Raster is the Raster's ancestor which has no parent.)
		''' sampleModelTranslate specifies the sampleModelTranslateX and
		''' sampleModelTranslateY values of the new Raster.
		''' 
		''' Note that this constructor should generally be called by other
		''' constructors or create methods, it should not be used directly. </summary>
		''' <param name="sampleModel">     The SampleModel that specifies the layout </param>
		''' <param name="dataBuffer">      The DataBuffer that contains the image data </param>
		''' <param name="aRegion">         The Rectangle that specifies the image area </param>
		''' <param name="sampleModelTranslate">  The Point that specifies the translation
		'''                        from SampleModel to Raster coordinates </param>
		''' <param name="parent">          The parent (if any) of this raster </param>
		''' <exception cref="NullPointerException"> if any of <code>sampleModel</code>,
		'''         <code>dataBuffer</code>, <code>aRegion</code> or
		'''         <code>sampleModelTranslate</code> is null </exception>
		''' <exception cref="RasterFormatException"> if <code>aRegion</code> has width
		'''         or height less than or equal to zero, or computing either
		'''         <code>aRegion.x + aRegion.width</code> or
		'''         <code>aRegion.y + aRegion.height</code> results in integer
		'''         overflow </exception>
		protected Raster(SampleModel sampleModel, DataBuffer dataBuffer_Renamed, java.awt.Rectangle aRegion, java.awt.Point sampleModelTranslate, Raster parent)

			If (sampleModel Is Nothing) OrElse (dataBuffer_Renamed Is Nothing) OrElse (aRegion Is Nothing) OrElse (sampleModelTranslate Is Nothing) Then Throw New NullPointerException("SampleModel, dataBuffer, aRegion and " & "sampleModelTranslate cannot be null")
		   Me.sampleModel = sampleModel
		   Me.dataBuffer_Renamed = dataBuffer_Renamed
		   minX = aRegion.x
		   minY = aRegion.y
		   width = aRegion.width
		   height = aRegion.height
		   If width <= 0 OrElse height <= 0 Then Throw New RasterFormatException("negative or zero " & (If(width <= 0, "width", "height")))
		   If (minX + width) < minX Then Throw New RasterFormatException("overflow condition for X coordinates of Raster")
		   If (minY + height) < minY Then Throw New RasterFormatException("overflow condition for Y coordinates of Raster")

		   sampleModelTranslateX = sampleModelTranslate.x
		   sampleModelTranslateY = sampleModelTranslate.y

		   numBands = sampleModel.numBands
		   numDataElements = sampleModel.numDataElements
		   Me.parent = parent


		''' <summary>
		''' Returns the parent Raster (if any) of this Raster or null. </summary>
		''' <returns> the parent Raster or <code>null</code>. </returns>
		public Raster parent
			Return parent

		''' <summary>
		''' Returns the X translation from the coordinate system of the
		''' SampleModel to that of the Raster.  To convert a pixel's X
		''' coordinate from the Raster coordinate system to the SampleModel
		''' coordinate system, this value must be subtracted. </summary>
		''' <returns> the X translation from the coordinate space of the
		'''         Raster's SampleModel to that of the Raster. </returns>
		public final Integer sampleModelTranslateX
			Return sampleModelTranslateX

		''' <summary>
		''' Returns the Y translation from the coordinate system of the
		''' SampleModel to that of the Raster.  To convert a pixel's Y
		''' coordinate from the Raster coordinate system to the SampleModel
		''' coordinate system, this value must be subtracted. </summary>
		''' <returns> the Y translation from the coordinate space of the
		'''         Raster's SampleModel to that of the Raster. </returns>
		public final Integer sampleModelTranslateY
			Return sampleModelTranslateY

		''' <summary>
		''' Create a compatible WritableRaster the same size as this Raster with
		''' the same SampleModel and a new initialized DataBuffer. </summary>
		''' <returns> a compatible <code>WritableRaster</code> with the same sample
		'''         model and a new data buffer. </returns>
		public WritableRaster createCompatibleWritableRaster()
			Return New sun.awt.image.SunWritableRaster(sampleModel, New java.awt.Point(0,0))

		''' <summary>
		''' Create a compatible WritableRaster with the specified size, a new
		''' SampleModel, and a new initialized DataBuffer. </summary>
		''' <param name="w"> the specified width of the new <code>WritableRaster</code> </param>
		''' <param name="h"> the specified height of the new <code>WritableRaster</code> </param>
		''' <returns> a compatible <code>WritableRaster</code> with the specified
		'''         size and a new sample model and data buffer. </returns>
		''' <exception cref="RasterFormatException"> if the width or height is less than
		'''                               or equal to zero. </exception>
		public WritableRaster createCompatibleWritableRaster(Integer w, Integer h)
			If w <= 0 OrElse h <=0 Then Throw New RasterFormatException("negative " & (If(w <= 0, "width", "height")))

			Dim sm As SampleModel = sampleModel.createCompatibleSampleModel(w,h)

			Return New sun.awt.image.SunWritableRaster(sm, New java.awt.Point(0,0))

		''' <summary>
		''' Create a compatible WritableRaster with location (minX, minY)
		''' and size (width, height) specified by rect, a
		''' new SampleModel, and a new initialized DataBuffer. </summary>
		''' <param name="rect"> a <code>Rectangle</code> that specifies the size and
		'''        location of the <code>WritableRaster</code> </param>
		''' <returns> a compatible <code>WritableRaster</code> with the specified
		'''         size and location and a new sample model and data buffer. </returns>
		''' <exception cref="RasterFormatException"> if <code>rect</code> has width
		'''         or height less than or equal to zero, or computing either
		'''         <code>rect.x + rect.width</code> or
		'''         <code>rect.y + rect.height</code> results in integer
		'''         overflow </exception>
		''' <exception cref="NullPointerException"> if <code>rect</code> is null </exception>
		public WritableRaster createCompatibleWritableRaster(java.awt.Rectangle rect)
			If rect Is Nothing Then Throw New NullPointerException("Rect cannot be null")
			Return createCompatibleWritableRaster(rect.x, rect.y, rect.width, rect.height)

		''' <summary>
		''' Create a compatible WritableRaster with the specified
		''' location (minX, minY) and size (width, height), a
		''' new SampleModel, and a new initialized DataBuffer. </summary>
		''' <param name="x"> the X coordinate of the upper-left corner of
		'''        the <code>WritableRaster</code> </param>
		''' <param name="y"> the Y coordinate of the upper-left corner of
		'''        the <code>WritableRaster</code> </param>
		''' <param name="w"> the specified width of the <code>WritableRaster</code> </param>
		''' <param name="h"> the specified height of the <code>WritableRaster</code> </param>
		''' <returns> a compatible <code>WritableRaster</code> with the specified
		'''         size and location and a new sample model and data buffer. </returns>
		''' <exception cref="RasterFormatException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero, or computing either
		'''         <code>x + w</code> or
		'''         <code>y + h</code> results in integer
		'''         overflow </exception>
		public WritableRaster createCompatibleWritableRaster(Integer x, Integer y, Integer w, Integer h)
			Dim ret As WritableRaster = createCompatibleWritableRaster(w, h)
			Return ret.createWritableChild(0,0,w,h,x,y,Nothing)

		''' <summary>
		''' Create a Raster with the same size, SampleModel and DataBuffer
		''' as this one, but with a different location.  The new Raster
		''' will possess a reference to the current Raster, accessible
		''' through its getParent() method.
		''' </summary>
		''' <param name="childMinX"> the X coordinate of the upper-left
		'''        corner of the new <code>Raster</code> </param>
		''' <param name="childMinY"> the Y coordinate of the upper-left
		'''        corner of the new <code>Raster</code> </param>
		''' <returns> a new <code>Raster</code> with the same size, SampleModel,
		'''         and DataBuffer as this <code>Raster</code>, but with the
		'''         specified location. </returns>
		''' <exception cref="RasterFormatException"> if  computing either
		'''         <code>childMinX + this.getWidth()</code> or
		'''         <code>childMinY + this.getHeight()</code> results in integer
		'''         overflow </exception>
		public Raster createTranslatedChild(Integer childMinX, Integer childMinY)
			Return createChild(minX,minY,width,height, childMinX,childMinY,Nothing)

		''' <summary>
		''' Returns a new Raster which shares all or part of this Raster's
		''' DataBuffer.  The new Raster will possess a reference to the
		''' current Raster, accessible through its getParent() method.
		''' 
		''' <p> The parentX, parentY, width and height parameters
		''' form a Rectangle in this Raster's coordinate space,
		''' indicating the area of pixels to be shared.  An error will
		''' be thrown if this Rectangle is not contained with the bounds
		''' of the current Raster.
		''' 
		''' <p> The new Raster may additionally be translated to a
		''' different coordinate system for the plane than that used by the current
		''' Raster.  The childMinX and childMinY parameters give the new
		''' (x, y) coordinate of the upper-left pixel of the returned
		''' Raster; the coordinate (childMinX, childMinY) in the new Raster
		''' will map to the same pixel as the coordinate (parentX, parentY)
		''' in the current Raster.
		''' 
		''' <p> The new Raster may be defined to contain only a subset of
		''' the bands of the current Raster, possibly reordered, by means
		''' of the bandList parameter.  If bandList is null, it is taken to
		''' include all of the bands of the current Raster in their current
		''' order.
		''' 
		''' <p> To create a new Raster that contains a subregion of the current
		''' Raster, but shares its coordinate system and bands,
		''' this method should be called with childMinX equal to parentX,
		''' childMinY equal to parentY, and bandList equal to null.
		''' </summary>
		''' <param name="parentX"> The X coordinate of the upper-left corner
		'''        in this Raster's coordinates </param>
		''' <param name="parentY"> The Y coordinate of the upper-left corner
		'''        in this Raster's coordinates </param>
		''' <param name="width">      Width of the region starting at (parentX, parentY) </param>
		''' <param name="height">     Height of the region starting at (parentX, parentY). </param>
		''' <param name="childMinX"> The X coordinate of the upper-left corner
		'''                   of the returned Raster </param>
		''' <param name="childMinY"> The Y coordinate of the upper-left corner
		'''                   of the returned Raster </param>
		''' <param name="bandList">   Array of band indices, or null to use all bands </param>
		''' <returns> a new <code>Raster</code>. </returns>
		''' <exception cref="RasterFormatException"> if the specified subregion is outside
		'''                               of the raster bounds. </exception>
		''' <exception cref="RasterFormatException"> if <code>width</code> or
		'''         <code>height</code>
		'''         is less than or equal to zero, or computing any of
		'''         <code>parentX + width</code>, <code>parentY + height</code>,
		'''         <code>childMinX + width</code>, or
		'''         <code>childMinY + height</code> results in integer
		'''         overflow </exception>
		public Raster createChild(Integer parentX, Integer parentY, Integer width, Integer height, Integer childMinX, Integer childMinY, Integer bandList())
			If parentX < Me.minX Then Throw New RasterFormatException("parentX lies outside raster")
			If parentY < Me.minY Then Throw New RasterFormatException("parentY lies outside raster")
			If (parentX + width < parentX) OrElse (parentX + width > Me.width + Me.minX) Then Throw New RasterFormatException("(parentX + width) is outside raster")
			If (parentY + height < parentY) OrElse (parentY + height > Me.height + Me.minY) Then Throw New RasterFormatException("(parentY + height) is outside raster")

			Dim subSampleModel As SampleModel
			' Note: the SampleModel for the child Raster should have the same
			' width and height as that for the parent, since it represents
			' the physical layout of the pixel data.  The child Raster's width
			' and height represent a "virtual" view of the pixel data, so
			' they may be different than those of the SampleModel.
			If bandList Is Nothing Then
				subSampleModel = sampleModel
			Else
				subSampleModel = sampleModel.createSubsetSampleModel(bandList)
			End If

			Dim deltaX As Integer = childMinX - parentX
			Dim deltaY As Integer = childMinY - parentY

			Return New Raster(subSampleModel, dataBuffer, New java.awt.Rectangle(childMinX, childMinY, width, height), New java.awt.Point(sampleModelTranslateX + deltaX, sampleModelTranslateY + deltaY), Me)

		''' <summary>
		''' Returns the bounding Rectangle of this Raster. This function returns
		''' the same information as getMinX/MinY/Width/Height. </summary>
		''' <returns> the bounding box of this <code>Raster</code>. </returns>
		public java.awt.Rectangle bounds
			Return New java.awt.Rectangle(minX, minY, width, height)

		''' <summary>
		''' Returns the minimum valid X coordinate of the Raster. </summary>
		'''  <returns> the minimum x coordinate of this <code>Raster</code>. </returns>
		public final Integer minX
			Return minX

		''' <summary>
		''' Returns the minimum valid Y coordinate of the Raster. </summary>
		'''  <returns> the minimum y coordinate of this <code>Raster</code>. </returns>
		public final Integer minY
			Return minY

		''' <summary>
		''' Returns the width in pixels of the Raster. </summary>
		'''  <returns> the width of this <code>Raster</code>. </returns>
		public final Integer width
			Return width

		''' <summary>
		''' Returns the height in pixels of the Raster. </summary>
		'''  <returns> the height of this <code>Raster</code>. </returns>
		public final Integer height
			Return height

		''' <summary>
		''' Returns the number of bands (samples per pixel) in this Raster. </summary>
		'''  <returns> the number of bands of this <code>Raster</code>. </returns>
		public final Integer numBands
			Return numBands

		''' <summary>
		'''  Returns the number of data elements needed to transfer one pixel
		'''  via the getDataElements and setDataElements methods.  When pixels
		'''  are transferred via these methods, they may be transferred in a
		'''  packed or unpacked format, depending on the implementation of the
		'''  underlying SampleModel.  Using these methods, pixels are transferred
		'''  as an array of getNumDataElements() elements of a primitive type given
		'''  by getTransferType().  The TransferType may or may not be the same
		'''  as the storage data type of the DataBuffer. </summary>
		'''  <returns> the number of data elements. </returns>
		public final Integer numDataElements
			Return sampleModel.numDataElements

		''' <summary>
		'''  Returns the TransferType used to transfer pixels via the
		'''  getDataElements and setDataElements methods.  When pixels
		'''  are transferred via these methods, they may be transferred in a
		'''  packed or unpacked format, depending on the implementation of the
		'''  underlying SampleModel.  Using these methods, pixels are transferred
		'''  as an array of getNumDataElements() elements of a primitive type given
		'''  by getTransferType().  The TransferType may or may not be the same
		'''  as the storage data type of the DataBuffer.  The TransferType will
		'''  be one of the types defined in DataBuffer. </summary>
		'''  <returns> this transfer type. </returns>
		public final Integer transferType
			Return sampleModel.transferType

		''' <summary>
		''' Returns the DataBuffer associated with this Raster. </summary>
		'''  <returns> the <code>DataBuffer</code> of this <code>Raster</code>. </returns>
		public DataBuffer dataBuffer
			Return dataBuffer_Renamed

		''' <summary>
		''' Returns the SampleModel that describes the layout of the image data. </summary>
		'''  <returns> the <code>SampleModel</code> of this <code>Raster</code>. </returns>
		public SampleModel sampleModel
			Return sampleModel

		''' <summary>
		''' Returns data for a single pixel in a primitive array of type
		''' TransferType.  For image data supported by the Java 2D(tm) API,
		''' this will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		''' or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		''' thus increasing efficiency for data transfers.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed.
		''' A ClassCastException will be thrown if the input object is non null
		''' and references anything other than an array of TransferType. </summary>
		''' <seealso cref= java.awt.image.SampleModel#getDataElements(int, int, Object, DataBuffer) </seealso>
		''' <param name="x">        The X coordinate of the pixel location </param>
		''' <param name="y">        The Y coordinate of the pixel location </param>
		''' <param name="outData">  An object reference to an array of type defined by
		'''                 getTransferType() and length getNumDataElements().
		'''                 If null, an array of appropriate type and size will be
		'''                 allocated </param>
		''' <returns>         An object reference to an array of type defined by
		'''                 getTransferType() with the requested pixel data.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if outData is too small to hold the output. </exception>
		public Object getDataElements(Integer x, Integer y, Object outData)
			Return sampleModel.getDataElements(x - sampleModelTranslateX, y - sampleModelTranslateY, outData, dataBuffer_Renamed)

		''' <summary>
		''' Returns the pixel data for the specified rectangle of pixels in a
		''' primitive array of type TransferType.
		''' For image data supported by the Java 2D API, this
		''' will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		''' or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		''' thus increasing efficiency for data transfers.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed.
		''' A ClassCastException will be thrown if the input object is non null
		''' and references anything other than an array of TransferType. </summary>
		''' <seealso cref= java.awt.image.SampleModel#getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		''' <param name="x">    The X coordinate of the upper-left pixel location </param>
		''' <param name="y">    The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">    Width of the pixel rectangle </param>
		''' <param name="h">   Height of the pixel rectangle </param>
		''' <param name="outData">  An object reference to an array of type defined by
		'''                 getTransferType() and length w*h*getNumDataElements().
		'''                 If null, an array of appropriate type and size will be
		'''                 allocated. </param>
		''' <returns>         An object reference to an array of type defined by
		'''                 getTransferType() with the requested pixel data.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if outData is too small to hold the output. </exception>
		public Object getDataElements(Integer x, Integer y, Integer w, Integer h, Object outData)
			Return sampleModel.getDataElements(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, outData, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples in an array of int for the specified pixel.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x"> The X coordinate of the pixel location </param>
		''' <param name="y"> The Y coordinate of the pixel location </param>
		''' <param name="iArray"> An optionally preallocated int array </param>
		''' <returns> the samples for the specified pixel.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if iArray is too small to hold the output. </exception>
		public Integer() getPixel(Integer x, Integer y, Integer iArray())
			Return sampleModel.getPixel(x - sampleModelTranslateX, y - sampleModelTranslateY, iArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples in an array of float for the
		''' specified pixel.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x"> The X coordinate of the pixel location </param>
		''' <param name="y"> The Y coordinate of the pixel location </param>
		''' <param name="fArray"> An optionally preallocated float array </param>
		''' <returns> the samples for the specified pixel.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if fArray is too small to hold the output. </exception>
		public Single() getPixel(Integer x, Integer y, Single fArray())
			Return sampleModel.getPixel(x - sampleModelTranslateX, y - sampleModelTranslateY, fArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples in an array of double for the specified pixel.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x"> The X coordinate of the pixel location </param>
		''' <param name="y"> The Y coordinate of the pixel location </param>
		''' <param name="dArray"> An optionally preallocated double array </param>
		''' <returns> the samples for the specified pixel.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if dArray is too small to hold the output. </exception>
		public Double() getPixel(Integer x, Integer y, Double dArray())
			Return sampleModel.getPixel(x - sampleModelTranslateX, y - sampleModelTranslateY, dArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns an int array containing all samples for a rectangle of pixels,
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">      The X coordinate of the upper-left pixel location </param>
		''' <param name="y">      The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">      Width of the pixel rectangle </param>
		''' <param name="h">      Height of the pixel rectangle </param>
		''' <param name="iArray"> An optionally pre-allocated int array </param>
		''' <returns> the samples for the specified rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if iArray is too small to hold the output. </exception>
		public Integer() getPixels(Integer x, Integer y, Integer w, Integer h, Integer iArray())
			Return sampleModel.getPixels(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, iArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns a float array containing all samples for a rectangle of pixels,
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location </param>
		''' <param name="y">        The Y coordinate of the pixel location </param>
		''' <param name="w">        Width of the pixel rectangle </param>
		''' <param name="h">        Height of the pixel rectangle </param>
		''' <param name="fArray">   An optionally pre-allocated float array </param>
		''' <returns> the samples for the specified rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if fArray is too small to hold the output. </exception>
		public Single() getPixels(Integer x, Integer y, Integer w, Integer h, Single fArray())
			Return sampleModel.getPixels(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, fArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns a double array containing all samples for a rectangle of pixels,
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper-left pixel location </param>
		''' <param name="y">        The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">        Width of the pixel rectangle </param>
		''' <param name="h">        Height of the pixel rectangle </param>
		''' <param name="dArray">   An optionally pre-allocated double array </param>
		''' <returns> the samples for the specified rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if dArray is too small to hold the output. </exception>
		public Double() getPixels(Integer x, Integer y, Integer w, Integer h, Double dArray())
			Return sampleModel.getPixels(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, dArray, dataBuffer_Renamed)


		''' <summary>
		''' Returns the sample in a specified band for the pixel located
		''' at (x,y) as an int.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location </param>
		''' <param name="y">        The Y coordinate of the pixel location </param>
		''' <param name="b">        The band to return </param>
		''' <returns> the sample in the specified band for the pixel at the
		'''         specified coordinate.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public Integer getSample(Integer x, Integer y, Integer b)
			Return sampleModel.getSample(x - sampleModelTranslateX, y - sampleModelTranslateY, b, dataBuffer_Renamed)

		''' <summary>
		''' Returns the sample in a specified band
		''' for the pixel located at (x,y) as a float.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location </param>
		''' <param name="y">        The Y coordinate of the pixel location </param>
		''' <param name="b">        The band to return </param>
		''' <returns> the sample in the specified band for the pixel at the
		'''         specified coordinate.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public Single getSampleFloat(Integer x, Integer y, Integer b)
			Return sampleModel.getSampleFloat(x - sampleModelTranslateX, y - sampleModelTranslateY, b, dataBuffer_Renamed)

		''' <summary>
		''' Returns the sample in a specified band
		''' for a pixel located at (x,y) as a java.lang.[Double].
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location </param>
		''' <param name="y">        The Y coordinate of the pixel location </param>
		''' <param name="b">        The band to return </param>
		''' <returns> the sample in the specified band for the pixel at the
		'''         specified coordinate.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public Double getSampleDouble(Integer x, Integer y, Integer b)
			Return sampleModel.getSampleDouble(x - sampleModelTranslateX, y - sampleModelTranslateY, b, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples for a specified band for the specified rectangle
		''' of pixels in an int array, one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper-left pixel location </param>
		''' <param name="y">        The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">        Width of the pixel rectangle </param>
		''' <param name="h">        Height of the pixel rectangle </param>
		''' <param name="b">        The band to return </param>
		''' <param name="iArray">   An optionally pre-allocated int array </param>
		''' <returns> the samples for the specified band for the specified
		'''         rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if iArray is too small to
		''' hold the output. </exception>
		public Integer() getSamples(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray())
			Return sampleModel.getSamples(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, b, iArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples for a specified band for the specified rectangle
		''' of pixels in a float array, one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper-left pixel location </param>
		''' <param name="y">        The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">        Width of the pixel rectangle </param>
		''' <param name="h">        Height of the pixel rectangle </param>
		''' <param name="b">        The band to return </param>
		''' <param name="fArray">   An optionally pre-allocated float array </param>
		''' <returns> the samples for the specified band for the specified
		'''         rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if fArray is too small to
		''' hold the output. </exception>
		public Single() getSamples(Integer x, Integer y, Integer w, Integer h, Integer b, Single fArray())
			Return sampleModel.getSamples(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, b, fArray, dataBuffer_Renamed)

		''' <summary>
		''' Returns the samples for a specified band for a specified rectangle
		''' of pixels in a double array, one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown
		''' if the coordinates are not in bounds.  However, explicit bounds
		''' checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper-left pixel location </param>
		''' <param name="y">        The Y coordinate of the upper-left pixel location </param>
		''' <param name="w">        Width of the pixel rectangle </param>
		''' <param name="h">        Height of the pixel rectangle </param>
		''' <param name="b">        The band to return </param>
		''' <param name="dArray">   An optionally pre-allocated double array </param>
		''' <returns> the samples for the specified band for the specified
		'''         rectangle of pixels.
		''' </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if dArray is too small to
		''' hold the output. </exception>
		public Double() getSamples(Integer x, Integer y, Integer w, Integer h, Integer b, Double dArray())
			 Return sampleModel.getSamples(x - sampleModelTranslateX, y - sampleModelTranslateY, w, h, b, dArray, dataBuffer_Renamed)

	End Class

End Namespace