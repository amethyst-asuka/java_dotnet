Imports Microsoft.VisualBasic

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
	''' The <code>MultiPixelPackedSampleModel</code> class represents
	''' one-banded images and can pack multiple one-sample
	''' pixels into one data element.  Pixels are not allowed to span data elements.
	''' The data type can be DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
	''' or DataBuffer.TYPE_INT.  Each pixel must be a power of 2 number of bits
	''' and a power of 2 number of pixels must fit exactly in one data element.
	''' Pixel bit stride is equal to the number of bits per pixel.  Scanline
	''' stride is in data elements and the last several data elements might be
	''' padded with unused pixels.  Data bit offset is the offset in bits from
	''' the beginning of the <seealso cref="DataBuffer"/> to the first pixel and must be
	''' a multiple of pixel bit stride.
	''' <p>
	''' The following code illustrates extracting the bits for pixel
	''' <code>x,&nbsp;y</code> from <code>DataBuffer</code> <code>data</code>
	''' and storing the pixel data in data elements of type
	''' <code>dataType</code>:
	''' <pre>{@code
	'''      int dataElementSize = DataBuffer.getDataTypeSize(dataType);
	'''      int bitnum = dataBitOffset + x*pixelBitStride;
	'''      int element = data.getElem(y*scanlineStride + bitnum/dataElementSize);
	'''      int shift = dataElementSize - (bitnum & (dataElementSize-1))
	'''                  - pixelBitStride;
	'''      int pixel = (element >> shift) & ((1 << pixelBitStride) - 1);
	''' }</pre>
	''' </summary>

	Public Class MultiPixelPackedSampleModel
		Inherits SampleModel

		''' <summary>
		''' The number of bits from one pixel to the next. </summary>
		Friend pixelBitStride As Integer

		''' <summary>
		''' Bitmask that extracts the rightmost pixel of a data element. </summary>
		Friend bitMask As Integer

		''' <summary>
		''' The number of pixels that fit in a data element.  Also used
		''' as the number of bits per pixel.
		''' </summary>
		Friend pixelsPerDataElement As Integer

		''' <summary>
		''' The size of a data element in bits. </summary>
		Friend dataElementSize As Integer

		''' <summary>
		''' The bit offset into the data array where the first pixel begins.
		''' </summary>
		Friend dataBitOffset As Integer

		''' <summary>
		''' ScanlineStride of the data buffer described in data array elements. </summary>
		Friend scanlineStride As Integer

		''' <summary>
		''' Constructs a <code>MultiPixelPackedSampleModel</code> with the
		''' specified data type, width, height and number of bits per pixel. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width, in pixels, of the region of
		'''                  image data described </param>
		''' <param name="h">         the height, in pixels, of the region of
		'''                  image data described </param>
		''' <param name="numberOfBits"> the number of bits per pixel </param>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         either <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>, or
		'''         <code>DataBuffer.TYPE_INT</code> </exception>
		Public Sub New(  dataType As Integer,   w As Integer,   h As Integer,   numberOfBits As Integer)
			Me.New(dataType,w,h, numberOfBits, (w*numberOfBits+DataBuffer.getDataTypeSize(dataType)-1)\ DataBuffer.getDataTypeSize(dataType), 0)
			If dataType <> DataBuffer.TYPE_BYTE AndAlso dataType <> DataBuffer.TYPE_USHORT AndAlso dataType <> DataBuffer.TYPE_INT Then Throw New IllegalArgumentException("Unsupported data type " & dataType)
		End Sub

		''' <summary>
		''' Constructs a <code>MultiPixelPackedSampleModel</code> with
		''' specified data type, width, height, number of bits per pixel,
		''' scanline stride and data bit offset. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width, in pixels, of the region of
		'''                  image data described </param>
		''' <param name="h">         the height, in pixels, of the region of
		'''                  image data described </param>
		''' <param name="numberOfBits"> the number of bits per pixel </param>
		''' <param name="scanlineStride"> the line stride of the image data </param>
		''' <param name="dataBitOffset"> the data bit offset for the region of image
		'''                  data described </param>
		''' <exception cref="RasterFormatException"> if the number of bits per pixel
		'''                  is not a power of 2 or if a power of 2 number of
		'''                  pixels do not fit in one data element. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         either <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>, or
		'''         <code>DataBuffer.TYPE_INT</code> </exception>
		Public Sub New(  dataType As Integer,   w As Integer,   h As Integer,   numberOfBits As Integer,   scanlineStride As Integer,   dataBitOffset As Integer)
			MyBase.New(dataType, w, h, 1)
			If dataType <> DataBuffer.TYPE_BYTE AndAlso dataType <> DataBuffer.TYPE_USHORT AndAlso dataType <> DataBuffer.TYPE_INT Then Throw New IllegalArgumentException("Unsupported data type " & dataType)
			Me.dataType = dataType
			Me.pixelBitStride = numberOfBits
			Me.scanlineStride = scanlineStride
			Me.dataBitOffset = dataBitOffset
			Me.dataElementSize = DataBuffer.getDataTypeSize(dataType)
			Me.pixelsPerDataElement = dataElementSize\numberOfBits
			If pixelsPerDataElement*numberOfBits <> dataElementSize Then Throw New RasterFormatException("MultiPixelPackedSampleModel " & "does not allow pixels to " & "span data element boundaries")
			Me.bitMask = (1 << numberOfBits) - 1
		End Sub


		''' <summary>
		''' Creates a new <code>MultiPixelPackedSampleModel</code> with the
		''' specified width and height.  The new
		''' <code>MultiPixelPackedSampleModel</code> has the
		''' same storage data type and number of bits per pixel as this
		''' <code>MultiPixelPackedSampleModel</code>. </summary>
		''' <param name="w"> the specified width </param>
		''' <param name="h"> the specified height </param>
		''' <returns> a <seealso cref="SampleModel"/> with the specified width and height
		''' and with the same storage data type and number of bits per pixel
		''' as this <code>MultiPixelPackedSampleModel</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		Public Overrides Function createCompatibleSampleModel(  w As Integer,   h As Integer) As SampleModel
		  Dim sampleModel_Renamed As SampleModel = New MultiPixelPackedSampleModel(dataType, w, h, pixelBitStride)
		  Return sampleModel_Renamed
		End Function

		''' <summary>
		''' Creates a <code>DataBuffer</code> that corresponds to this
		''' <code>MultiPixelPackedSampleModel</code>.  The
		''' <code>DataBuffer</code> object's data type and size
		''' is consistent with this <code>MultiPixelPackedSampleModel</code>.
		''' The <code>DataBuffer</code> has a single bank. </summary>
		''' <returns> a <code>DataBuffer</code> with the same data type and
		''' size as this <code>MultiPixelPackedSampleModel</code>. </returns>
		Public Overrides Function createDataBuffer() As DataBuffer
			Dim dataBuffer_Renamed As DataBuffer = Nothing

			Dim size As Integer = CInt(scanlineStride)*height
			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				dataBuffer_Renamed = New DataBufferByte(size+(dataBitOffset+7)\8)
			Case DataBuffer.TYPE_USHORT
				dataBuffer_Renamed = New DataBufferUShort(size+(dataBitOffset+15)\16)
			Case DataBuffer.TYPE_INT
				dataBuffer_Renamed = New DataBufferInt(size+(dataBitOffset+31)\32)
			End Select
			Return dataBuffer_Renamed
		End Function

		''' <summary>
		''' Returns the number of data elements needed to transfer one pixel
		''' via the <seealso cref="#getDataElements"/> and <seealso cref="#setDataElements"/>
		''' methods.  For a <code>MultiPixelPackedSampleModel</code>, this is
		''' one. </summary>
		''' <returns> the number of data elements. </returns>
		Public  Overrides ReadOnly Property  numDataElements As Integer
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' Returns the number of bits per sample for all bands. </summary>
		''' <returns> the number of bits per sample. </returns>
		Public  Overrides ReadOnly Property  sampleSize As Integer()
			Get
				Dim sampleSize_Renamed As Integer() = {pixelBitStride}
				Return sampleSize_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the number of bits per sample for the specified band. </summary>
		''' <param name="band"> the specified band </param>
		''' <returns> the number of bits per sample for the specified band. </returns>
		Public Overrides Function getSampleSize(  band As Integer) As Integer
			Return pixelBitStride
		End Function

		''' <summary>
		''' Returns the offset of pixel (x,&nbsp;y) in data array elements. </summary>
		''' <param name="x"> the X coordinate of the specified pixel </param>
		''' <param name="y"> the Y coordinate of the specified pixel </param>
		''' <returns> the offset of the specified pixel. </returns>
		Public Overridable Function getOffset(  x As Integer,   y As Integer) As Integer
			Dim offset_Renamed As Integer = y * scanlineStride
			offset_Renamed += (x*pixelBitStride+dataBitOffset)\dataElementSize
			Return offset_Renamed
		End Function

		''' <summary>
		'''  Returns the offset, in bits, into the data element in which it is
		'''  stored for the <code>x</code>th pixel of a scanline.
		'''  This offset is the same for all scanlines. </summary>
		'''  <param name="x"> the specified pixel </param>
		'''  <returns> the bit offset of the specified pixel. </returns>
		Public Overridable Function getBitOffset(  x As Integer) As Integer
		   Return (x*pixelBitStride+dataBitOffset) Mod dataElementSize
		End Function

		''' <summary>
		''' Returns the scanline stride. </summary>
		''' <returns> the scanline stride of this
		''' <code>MultiPixelPackedSampleModel</code>. </returns>
		Public Overridable Property scanlineStride As Integer
			Get
				Return scanlineStride
			End Get
		End Property

		''' <summary>
		''' Returns the pixel bit stride in bits.  This value is the same as
		''' the number of bits per pixel. </summary>
		''' <returns> the <code>pixelBitStride</code> of this
		''' <code>MultiPixelPackedSampleModel</code>. </returns>
		Public Overridable Property pixelBitStride As Integer
			Get
				Return pixelBitStride
			End Get
		End Property

		''' <summary>
		''' Returns the data bit offset in bits. </summary>
		''' <returns> the <code>dataBitOffset</code> of this
		''' <code>MultiPixelPackedSampleModel</code>. </returns>
		Public Overridable Property dataBitOffset As Integer
			Get
				Return dataBitOffset
			End Get
		End Property

		''' <summary>
		'''  Returns the TransferType used to transfer pixels by way of the
		'''  <code>getDataElements</code> and <code>setDataElements</code>
		'''  methods. The TransferType might or might not be the same as the
		'''  storage DataType.  The TransferType is one of
		'''  DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		'''  or DataBuffer.TYPE_INT. </summary>
		'''  <returns> the transfertype. </returns>
		Public  Overrides ReadOnly Property  transferType As Integer
			Get
				If pixelBitStride > 16 Then
					Return DataBuffer.TYPE_INT
				ElseIf pixelBitStride > 8 Then
					Return DataBuffer.TYPE_USHORT
				Else
					Return DataBuffer.TYPE_BYTE
				End If
			End Get
		End Property

		''' <summary>
		''' Creates a new <code>MultiPixelPackedSampleModel</code> with a
		''' subset of the bands of this
		''' <code>MultiPixelPackedSampleModel</code>.  Since a
		''' <code>MultiPixelPackedSampleModel</code> only has one band, the
		''' bands argument must have a length of one and indicate the zeroth
		''' band. </summary>
		''' <param name="bands"> the specified bands </param>
		''' <returns> a new <code>SampleModel</code> with a subset of bands of
		''' this <code>MultiPixelPackedSampleModel</code>. </returns>
		''' <exception cref="RasterFormatException"> if the number of bands requested
		''' is not one. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		Public Overrides Function createSubsetSampleModel(  bands As Integer()) As SampleModel
			If bands IsNot Nothing Then
			   If bands.Length <> 1 Then Throw New RasterFormatException("MultiPixelPackedSampleModel has " & "only one band.")
			End If
			Dim sm As SampleModel = createCompatibleSampleModel(width, height)
			Return sm
		End Function

		''' <summary>
		''' Returns as <code>int</code> the sample in a specified band for the
		''' pixel located at (x,&nbsp;y).  An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x">         the X coordinate of the specified pixel </param>
		''' <param name="y">         the Y coordinate of the specified pixel </param>
		''' <param name="b">         the band to return, which is assumed to be 0 </param>
		''' <param name="data">      the <code>DataBuffer</code> containing the image
		'''                  data </param>
		''' <returns> the specified band containing the sample of the specified
		''' pixel. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the specified
		'''          coordinates are not in bounds. </exception>
		''' <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		Public Overrides Function getSample(  x As Integer,   y As Integer,   b As Integer,   data As DataBuffer) As Integer
			' 'b' must be 0
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) OrElse (b <> 0) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim bitnum As Integer = dataBitOffset + x*pixelBitStride
			Dim element As Integer = data.getElem(y*scanlineStride + bitnum\dataElementSize)
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			Return (element >> shift) And bitMask
		End Function

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at
		''' (x,&nbsp;y) in the <code>DataBuffer</code> using an
		''' <code>int</code> for input.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x"> the X coordinate of the specified pixel </param>
		''' <param name="y"> the Y coordinate of the specified pixel </param>
		''' <param name="b"> the band to return, which is assumed to be 0 </param>
		''' <param name="s"> the input sample as an <code>int</code> </param>
		''' <param name="data"> the <code>DataBuffer</code> where image data is stored </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds. </exception>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		Public Overrides Sub setSample(  x As Integer,   y As Integer,   b As Integer,   s As Integer,   data As DataBuffer)
			' 'b' must be 0
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) OrElse (b <> 0) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim bitnum As Integer = dataBitOffset + x * pixelBitStride
			Dim index As Integer = y * scanlineStride + (bitnum \ dataElementSize)
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			Dim element As Integer = data.getElem(index)
			element = element And Not(bitMask << shift)
			element = element Or (s And bitMask) << shift
			data.elemlem(index,element)
		End Sub

		''' <summary>
		''' Returns data for a single pixel in a primitive array of type
		''' TransferType.  For a <code>MultiPixelPackedSampleModel</code>,
		''' the array has one element, and the type is the smallest of
		''' DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, or DataBuffer.TYPE_INT
		''' that can hold a single pixel.  Generally, <code>obj</code>
		''' should be passed in as <code>null</code>, so that the
		''' <code>Object</code> is created automatically and is the
		''' correct primitive data type.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		''' described by <code>MultiPixelPackedSampleModel</code>
		''' <code>mppsm1</code>, to <code>DataBuffer</code> <code>db2</code>,
		''' whose storage layout is described by
		''' <code>MultiPixelPackedSampleModel</code> <code>mppsm2</code>.
		''' The transfer is generally more efficient than using
		''' <code>getPixel</code> or <code>setPixel</code>.
		''' <pre>
		'''       MultiPixelPackedSampleModel mppsm1, mppsm2;
		'''       DataBufferInt db1, db2;
		'''       mppsm2.setDataElements(x, y, mppsm1.getDataElements(x, y, null,
		'''                              db1), db2);
		''' </pre>
		''' Using <code>getDataElements</code> or <code>setDataElements</code>
		''' to transfer between two <code>DataBuffer/SampleModel</code> pairs
		''' is legitimate if the <code>SampleModels</code> have the same number
		''' of bands, corresponding bands have the same number of
		''' bits per sample, and the TransferTypes are the same.
		''' <p>
		''' If <code>obj</code> is not <code>null</code>, it should be a
		''' primitive array of type TransferType.  Otherwise, a
		''' <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' coordinates are not in bounds, or if <code>obj</code> is not
		''' <code>null</code> and is not large enough to hold the pixel data. </summary>
		''' <param name="x"> the X coordinate of the specified pixel </param>
		''' <param name="y"> the Y coordinate of the specified pixel </param>
		''' <param name="obj"> a primitive array in which to return the pixel data or
		'''          <code>null</code>. </param>
		''' <param name="data"> the <code>DataBuffer</code> containing the image data. </param>
		''' <returns> an <code>Object</code> containing data for the specified
		'''  pixel. </returns>
		''' <exception cref="ClassCastException"> if <code>obj</code> is not a
		'''  primitive array of type TransferType or is not <code>null</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if <code>obj</code> is not <code>null</code> or
		''' not large enough to hold the pixel data </exception>
		''' <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		Public Overrides Function getDataElements(  x As Integer,   y As Integer,   obj As Object,   data As DataBuffer) As Object
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType
			Dim bitnum As Integer = dataBitOffset + x*pixelBitStride
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			Dim element As Integer = 0

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim bdata As SByte()

				If obj Is Nothing Then
					bdata = New SByte(0){}
				Else
					bdata = CType(obj, SByte())
				End If

				element = data.getElem(y*scanlineStride + bitnum\dataElementSize)
				bdata(0) = CByte((element >> shift) And bitMask)

				obj = CObj(bdata)

			Case DataBuffer.TYPE_USHORT

				Dim sdata As Short()

				If obj Is Nothing Then
					sdata = New Short(0){}
				Else
					sdata = CType(obj, Short())
				End If

				element = data.getElem(y*scanlineStride + bitnum\dataElementSize)
				sdata(0) = CShort(Fix((element >> shift) And bitMask))

				obj = CObj(sdata)

			Case DataBuffer.TYPE_INT

				Dim idata As Integer()

				If obj Is Nothing Then
					idata = New Integer(0){}
				Else
					idata = CType(obj, Integer())
				End If

				element = data.getElem(y*scanlineStride + bitnum\dataElementSize)
				idata(0) = (element >> shift) And bitMask

				obj = CObj(idata)
			End Select

			Return obj
		End Function

		''' <summary>
		''' Returns the specified single band pixel in the first element
		''' of an <code>int</code> array.
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x"> the X coordinate of the specified pixel </param>
		''' <param name="y"> the Y coordinate of the specified pixel </param>
		''' <param name="iArray"> the array containing the pixel to be returned or
		'''  <code>null</code> </param>
		''' <param name="data"> the <code>DataBuffer</code> where image data is stored </param>
		''' <returns> an array containing the specified pixel. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates
		'''  are not in bounds </exception>
		''' <seealso cref= #setPixel(int, int, int[], DataBuffer) </seealso>
		Public Overrides Function getPixel(  x As Integer,   y As Integer,   iArray As Integer(),   data As DataBuffer) As Integer()
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()
			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer (numBands - 1){}
			End If
			Dim bitnum As Integer = dataBitOffset + x*pixelBitStride
			Dim element As Integer = data.getElem(y*scanlineStride + bitnum\dataElementSize)
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			pixels_Renamed(0) = (element >> shift) And bitMask
			Return pixels_Renamed
		End Function

		''' <summary>
		''' Sets the data for a single pixel in the specified
		''' <code>DataBuffer</code> from a primitive array of type
		''' TransferType.  For a <code>MultiPixelPackedSampleModel</code>,
		''' only the first element of the array holds valid data,
		''' and the type must be the smallest of
		''' DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, or DataBuffer.TYPE_INT
		''' that can hold a single pixel.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		''' described by <code>MultiPixelPackedSampleModel</code>
		''' <code>mppsm1</code>, to <code>DataBuffer</code> <code>db2</code>,
		''' whose storage layout is described by
		''' <code>MultiPixelPackedSampleModel</code> <code>mppsm2</code>.
		''' The transfer is generally more efficient than using
		''' <code>getPixel</code> or <code>setPixel</code>.
		''' <pre>
		'''       MultiPixelPackedSampleModel mppsm1, mppsm2;
		'''       DataBufferInt db1, db2;
		'''       mppsm2.setDataElements(x, y, mppsm1.getDataElements(x, y, null,
		'''                              db1), db2);
		''' </pre>
		''' Using <code>getDataElements</code> or <code>setDataElements</code> to
		''' transfer between two <code>DataBuffer/SampleModel</code> pairs is
		''' legitimate if the <code>SampleModel</code> objects have
		''' the same number of bands, corresponding bands have the same number of
		''' bits per sample, and the TransferTypes are the same.
		''' <p>
		''' <code>obj</code> must be a primitive array of type TransferType.
		''' Otherwise, a <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' coordinates are not in bounds, or if <code>obj</code> is not large
		''' enough to hold the pixel data. </summary>
		''' <param name="x"> the X coordinate of the pixel location </param>
		''' <param name="y"> the Y coordinate of the pixel location </param>
		''' <param name="obj"> a primitive array containing pixel data </param>
		''' <param name="data"> the <code>DataBuffer</code> containing the image data </param>
		''' <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		Public Overrides Sub setDataElements(  x As Integer,   y As Integer,   obj As Object,   data As DataBuffer)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType
			Dim bitnum As Integer = dataBitOffset + x * pixelBitStride
			Dim index As Integer = y * scanlineStride + (bitnum \ dataElementSize)
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			Dim element As Integer = data.getElem(index)
			element = element And Not(bitMask << shift)

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim barray As SByte() = CType(obj, SByte())
				element = element Or ((CInt(barray(0)) And &Hff) And bitMask) << shift
				data.elemlem(index, element)

			Case DataBuffer.TYPE_USHORT

				Dim sarray As Short() = CType(obj, Short())
				element = element Or ((CInt(sarray(0)) And &Hffff) And bitMask) << shift
				data.elemlem(index, element)

			Case DataBuffer.TYPE_INT

				Dim iarray As Integer() = CType(obj, Integer())
				element = element Or (iarray(0) And bitMask) << shift
				data.elemlem(index, element)
			End Select
		End Sub

		''' <summary>
		''' Sets a pixel in the <code>DataBuffer</code> using an
		''' <code>int</code> array for input.
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x"> the X coordinate of the pixel location </param>
		''' <param name="y"> the Y coordinate of the pixel location </param>
		''' <param name="iArray"> the input pixel in an <code>int</code> array </param>
		''' <param name="data"> the <code>DataBuffer</code> containing the image data </param>
		''' <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		Public Overrides Sub setPixel(  x As Integer,   y As Integer,   iArray As Integer(),   data As DataBuffer)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim bitnum As Integer = dataBitOffset + x * pixelBitStride
			Dim index As Integer = y * scanlineStride + (bitnum \ dataElementSize)
			Dim shift As Integer = dataElementSize - (bitnum And (dataElementSize-1)) - pixelBitStride
			Dim element As Integer = data.getElem(index)
			element = element And Not(bitMask << shift)
			element = element Or (iArray(0) And bitMask) << shift
			data.elemlem(index,element)
		End Sub

		Public Overrides Function Equals(  o As Object) As Boolean
			If (o Is Nothing) OrElse Not(TypeOf o Is MultiPixelPackedSampleModel) Then Return False

			Dim that As MultiPixelPackedSampleModel = CType(o, MultiPixelPackedSampleModel)
			Return Me.width = that.width AndAlso Me.height = that.height AndAlso Me.numBands = that.numBands AndAlso Me.dataType = that.dataType AndAlso Me.pixelBitStride = that.pixelBitStride AndAlso Me.bitMask = that.bitMask AndAlso Me.pixelsPerDataElement = that.pixelsPerDataElement AndAlso Me.dataElementSize = that.dataElementSize AndAlso Me.dataBitOffset = that.dataBitOffset AndAlso Me.scanlineStride = that.scanlineStride
		End Function

		' If we implement equals() we must also implement hashCode
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = 0
			hash = width
			hash <<= 8
			hash = hash Xor height
			hash <<= 8
			hash = hash Xor numBands
			hash <<= 8
			hash = hash Xor dataType
			hash <<= 8
			hash = hash Xor pixelBitStride
			hash <<= 8
			hash = hash Xor bitMask
			hash <<= 8
			hash = hash Xor pixelsPerDataElement
			hash <<= 8
			hash = hash Xor dataElementSize
			hash <<= 8
			hash = hash Xor dataBitOffset
			hash <<= 8
			hash = hash Xor scanlineStride
			Return hash
		End Function
	End Class

End Namespace