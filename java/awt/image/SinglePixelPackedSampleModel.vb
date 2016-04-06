Imports Microsoft.VisualBasic
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
	'''  This class represents pixel data packed such that the N samples which make
	'''  up a single pixel are stored in a single data array element, and each data
	'''  data array element holds samples for only one pixel.
	'''  This class supports
	'''  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	'''  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/> data types.
	'''  All data array elements reside
	'''  in the first bank of a DataBuffer.  Accessor methods are provided so
	'''  that the image data can be manipulated directly. Scanline stride is the
	'''  number of data array elements between a given sample and the corresponding
	'''  sample in the same column of the next scanline. Bit masks are the masks
	'''  required to extract the samples representing the bands of the pixel.
	'''  Bit offsets are the offsets in bits into the data array
	'''  element of the samples representing the bands of the pixel.
	''' <p>
	''' The following code illustrates extracting the bits of the sample
	''' representing band <code>b</code> for pixel <code>x,y</code>
	''' from DataBuffer <code>data</code>:
	''' <pre>{@code
	'''      int sample = data.getElem(y * scanlineStride + x);
	'''      sample = (sample & bitMasks[b]) >>> bitOffsets[b];
	''' }</pre>
	''' </summary>

	Public Class SinglePixelPackedSampleModel
		Inherits SampleModel

		''' <summary>
		''' Bit masks for all bands of the image data. </summary>
		Private bitMasks As Integer()

		''' <summary>
		''' Bit Offsets for all bands of the image data. </summary>
		Private bitOffsets As Integer()

		''' <summary>
		''' Bit sizes for all the bands of the image data. </summary>
		Private bitSizes As Integer()

		''' <summary>
		''' Maximum bit size. </summary>
		Private maxBitSize As Integer

		''' <summary>
		''' Line stride of the region of image data described by this
		'''  SinglePixelPackedSampleModel.
		''' </summary>
		Private scanlineStride As Integer

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Constructs a SinglePixelPackedSampleModel with bitMasks.length bands.
		''' Each sample is stored in a data array element in the position of
		''' its corresponding bit mask.  Each bit mask must be contiguous and
		''' masks must not overlap. Bit masks exceeding data type capacity are
		''' truncated. </summary>
		''' <param name="dataType">  The data type for storing samples. </param>
		''' <param name="w">         The width (in pixels) of the region of the
		'''                  image data described. </param>
		''' <param name="h">         The height (in pixels) of the region of the
		'''                  image data described. </param>
		''' <param name="bitMasks">  The bit masks for all bands. </param>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         either <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>, or
		'''         <code>DataBuffer.TYPE_INT</code> </exception>
		Public Sub New(  dataType As Integer,   w As Integer,   h As Integer,   bitMasks As Integer())
			Me.New(dataType, w, h, w, bitMasks)
			If dataType <> DataBuffer.TYPE_BYTE AndAlso dataType <> DataBuffer.TYPE_USHORT AndAlso dataType <> DataBuffer.TYPE_INT Then Throw New IllegalArgumentException("Unsupported data type " & dataType)
		End Sub

		''' <summary>
		''' Constructs a SinglePixelPackedSampleModel with bitMasks.length bands
		''' and a scanline stride equal to scanlineStride data array elements.
		''' Each sample is stored in a data array element in the position of
		''' its corresponding bit mask.  Each bit mask must be contiguous and
		''' masks must not overlap. Bit masks exceeding data type capacity are
		''' truncated. </summary>
		''' <param name="dataType">  The data type for storing samples. </param>
		''' <param name="w">         The width (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="h">         The height (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="scanlineStride"> The line stride of the image data. </param>
		''' <param name="bitMasks"> The bit masks for all bands. </param>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if any mask in
		'''         <code>bitMask</code> is not contiguous </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         either <code>DataBuffer.TYPE_BYTE</code>,
		'''         <code>DataBuffer.TYPE_USHORT</code>, or
		'''         <code>DataBuffer.TYPE_INT</code> </exception>
		Public Sub New(  dataType As Integer,   w As Integer,   h As Integer,   scanlineStride As Integer,   bitMasks As Integer())
			MyBase.New(dataType, w, h, bitMasks.Length)
			If dataType <> DataBuffer.TYPE_BYTE AndAlso dataType <> DataBuffer.TYPE_USHORT AndAlso dataType <> DataBuffer.TYPE_INT Then Throw New IllegalArgumentException("Unsupported data type " & dataType)
			Me.dataType = dataType
			Me.bitMasks = CType(bitMasks.clone(), Integer())
			Me.scanlineStride = scanlineStride

			Me.bitOffsets = New Integer(numBands - 1){}
			Me.bitSizes = New Integer(numBands - 1){}

			Dim maxMask As Integer = CInt(Fix((1L << DataBuffer.getDataTypeSize(dataType)) - 1))

			Me.maxBitSize = 0
			For i As Integer = 0 To numBands - 1
				Dim bitOffset As Integer = 0, bitSize As Integer = 0, mask As Integer
				Me.bitMasks(i) = Me.bitMasks(i) And maxMask
				mask = Me.bitMasks(i)
				If mask <> 0 Then
					Do While (mask And 1) = 0
						mask = CInt(CUInt(mask) >> 1)
						bitOffset += 1
					Loop
					Do While (mask And 1) = 1
						mask = CInt(CUInt(mask) >> 1)
						bitSize += 1
					Loop
					If mask <> 0 Then Throw New IllegalArgumentException("Mask " & bitMasks(i) & " must be contiguous")
				End If
				bitOffsets(i) = bitOffset
				bitSizes(i) = bitSize
				If bitSize > maxBitSize Then maxBitSize = bitSize
			Next i
		End Sub

		''' <summary>
		''' Returns the number of data elements needed to transfer one pixel
		''' via the getDataElements and setDataElements methods.
		''' For a SinglePixelPackedSampleModel, this is one.
		''' </summary>
		Public  Overrides ReadOnly Property  numDataElements As Integer
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' Returns the size of the buffer (in data array elements)
		''' needed for a data buffer that matches this
		''' SinglePixelPackedSampleModel.
		''' </summary>
		Private Property bufferSize As Long
			Get
			  Dim size As Long = scanlineStride * (height-1) + width
			  Return size
			End Get
		End Property

		''' <summary>
		''' Creates a new SinglePixelPackedSampleModel with the specified
		''' width and height.  The new SinglePixelPackedSampleModel will have the
		''' same storage data type and bit masks as this
		''' SinglePixelPackedSampleModel. </summary>
		''' <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		''' <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		''' <returns> a <code>SinglePixelPackedSampleModel</code> with the
		'''         specified width and height. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		Public Overrides Function createCompatibleSampleModel(  w As Integer,   h As Integer) As SampleModel
		  Dim sampleModel_Renamed As SampleModel = New SinglePixelPackedSampleModel(dataType, w, h, bitMasks)
		  Return sampleModel_Renamed
		End Function

		''' <summary>
		''' Creates a DataBuffer that corresponds to this
		''' SinglePixelPackedSampleModel.  The DataBuffer's data type and size
		''' will be consistent with this SinglePixelPackedSampleModel.  The
		''' DataBuffer will have a single bank.
		''' </summary>
		Public Overrides Function createDataBuffer() As DataBuffer
			Dim dataBuffer_Renamed As DataBuffer = Nothing

			Dim size As Integer = CInt(bufferSize)
			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				dataBuffer_Renamed = New DataBufferByte(size)
			Case DataBuffer.TYPE_USHORT
				dataBuffer_Renamed = New DataBufferUShort(size)
			Case DataBuffer.TYPE_INT
				dataBuffer_Renamed = New DataBufferInt(size)
			End Select
			Return dataBuffer_Renamed
		End Function

		''' <summary>
		''' Returns the number of bits per sample for all bands. </summary>
		Public  Overrides ReadOnly Property  sampleSize As Integer()
			Get
				Return bitSizes.clone()
			End Get
		End Property

		''' <summary>
		''' Returns the number of bits per sample for the specified band. </summary>
		Public Overrides Function getSampleSize(  band As Integer) As Integer
			Return bitSizes(band)
		End Function

		''' <summary>
		''' Returns the offset (in data array elements) of pixel (x,y).
		'''  The data element containing pixel <code>x,y</code>
		'''  can be retrieved from a DataBuffer <code>data</code> with a
		'''  SinglePixelPackedSampleModel <code>sppsm</code> as:
		''' <pre>
		'''        data.getElem(sppsm.getOffset(x, y));
		''' </pre> </summary>
		''' <param name="x"> the X coordinate of the specified pixel </param>
		''' <param name="y"> the Y coordinate of the specified pixel </param>
		''' <returns> the offset of the specified pixel. </returns>
		Public Overridable Function getOffset(  x As Integer,   y As Integer) As Integer
			Dim offset_Renamed As Integer = y * scanlineStride + x
			Return offset_Renamed
		End Function

		''' <summary>
		''' Returns the bit offsets into the data array element representing
		'''  a pixel for all bands. </summary>
		'''  <returns> the bit offsets representing a pixel for all bands. </returns>
		Public Overridable Property bitOffsets As Integer()
			Get
			  Return CType(bitOffsets.clone(), Integer())
			End Get
		End Property

		''' <summary>
		''' Returns the bit masks for all bands. </summary>
		'''  <returns> the bit masks for all bands. </returns>
		Public Overridable Property bitMasks As Integer()
			Get
			  Return CType(bitMasks.clone(), Integer())
			End Get
		End Property

		''' <summary>
		''' Returns the scanline stride of this SinglePixelPackedSampleModel. </summary>
		'''  <returns> the scanline stride of this
		'''          <code>SinglePixelPackedSampleModel</code>. </returns>
		Public Overridable Property scanlineStride As Integer
			Get
			  Return scanlineStride
			End Get
		End Property

		''' <summary>
		''' This creates a new SinglePixelPackedSampleModel with a subset of the
		''' bands of this SinglePixelPackedSampleModel.  The new
		''' SinglePixelPackedSampleModel can be used with any DataBuffer that the
		''' existing SinglePixelPackedSampleModel can be used with.  The new
		''' SinglePixelPackedSampleModel/DataBuffer combination will represent
		''' an image with a subset of the bands of the original
		''' SinglePixelPackedSampleModel/DataBuffer combination. </summary>
		''' <exception cref="RasterFormatException"> if the length of the bands argument is
		'''                                  greater than the number of bands in
		'''                                  the sample model. </exception>
		Public Overrides Function createSubsetSampleModel(  bands As Integer()) As SampleModel
			If bands.Length > numBands Then Throw New RasterFormatException("There are only " & numBands & " bands")
			Dim newBitMasks As Integer() = New Integer(bands.Length - 1){}
			For i As Integer = 0 To bands.Length - 1
				newBitMasks(i) = bitMasks(bands(i))
			Next i

			Return New SinglePixelPackedSampleModel(Me.dataType, width, height, Me.scanlineStride, newBitMasks)
		End Function

		''' <summary>
		''' Returns data for a single pixel in a primitive array of type
		''' TransferType.  For a SinglePixelPackedSampleModel, the array will
		''' have one element, and the type will be the same as the storage
		''' data type.  Generally, obj
		''' should be passed in as null, so that the Object will be created
		''' automatically and will be of the right primitive data type.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SinglePixelPackedSampleModel <code>sppsm1</code>, to
		''' DataBuffer <code>db2</code>, whose storage layout is described by
		''' SinglePixelPackedSampleModel <code>sppsm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixel/setPixel.
		''' <pre>
		'''       SinglePixelPackedSampleModel sppsm1, sppsm2;
		'''       DataBufferInt db1, db2;
		'''       sppsm2.setDataElements(x, y, sppsm1.getDataElements(x, y, null,
		'''                              db1), db2);
		''' </pre>
		''' Using getDataElements/setDataElements to transfer between two
		''' DataBuffer/SampleModel pairs is legitimate if the SampleModels have
		''' the same number of bands, corresponding bands have the same number of
		''' bits per sample, and the TransferTypes are the same.
		''' <p>
		''' If obj is non-null, it should be a primitive array of type TransferType.
		''' Otherwise, a ClassCastException is thrown.  An
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds, or if obj is non-null and is not large enough to hold
		''' the pixel data. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="obj">       If non-null, a primitive array in which to return
		'''                  the pixel data. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the data for the specified pixel. </returns>
		''' <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		Public Overrides Function getDataElements(  x As Integer,   y As Integer,   obj As Object,   data As DataBuffer) As Object
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim bdata As SByte()

				If obj Is Nothing Then
					bdata = New SByte(0){}
				Else
					bdata = CType(obj, SByte())
				End If

				bdata(0) = CByte(data.getElem(y * scanlineStride + x))

				obj = CObj(bdata)

			Case DataBuffer.TYPE_USHORT

				Dim sdata As Short()

				If obj Is Nothing Then
					sdata = New Short(0){}
				Else
					sdata = CType(obj, Short())
				End If

				sdata(0) = CShort(data.getElem(y * scanlineStride + x))

				obj = CObj(sdata)

			Case DataBuffer.TYPE_INT

				Dim idata As Integer()

				If obj Is Nothing Then
					idata = New Integer(0){}
				Else
					idata = CType(obj, Integer())
				End If

				idata(0) = data.getElem(y * scanlineStride + x)

				obj = CObj(idata)
			End Select

			Return obj
		End Function

		''' <summary>
		''' Returns all samples in for the specified pixel in an int array.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> all samples for the specified pixel. </returns>
		''' <seealso cref= #setPixel(int, int, int[], DataBuffer) </seealso>
		Public Overrides Function getPixel(  x As Integer,   y As Integer,   iArray As Integer(),   data As DataBuffer) As Integer()
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()
			If iArray Is Nothing Then
				pixels_Renamed = New Integer (numBands - 1){}
			Else
				pixels_Renamed = iArray
			End If

			Dim value As Integer = data.getElem(y * scanlineStride + x)
			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = CInt(CUInt((value And bitMasks(i))) >> bitOffsets(i))
			Next i
			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns all samples for the specified rectangle of pixels in
		''' an int array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="iArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> all samples for the specified region of pixels. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer) </seealso>
		Public Overrides Function getPixels(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   iArray As Integer(),   data As DataBuffer) As Integer()
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()
			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer (w*h*numBands - 1){}
			End If
			Dim lineOffset As Integer = y*scanlineStride + x
			Dim dstOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   For j As Integer = 0 To w - 1
				  Dim value As Integer = data.getElem(lineOffset+j)
				  For k As Integer = 0 To numBands - 1
					  pixels_Renamed(dstOffset) = (CInt(CUInt((value And bitMasks(k))) >> bitOffsets(k)))
					  dstOffset += 1
				  Next k
			   Next j
			   lineOffset += scanlineStride
			Next i
			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns as int the sample in a specified band for the pixel
		''' located at (x,y).
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the sample in a specified band for the specified
		'''         pixel. </returns>
		''' <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		Public Overrides Function getSample(  x As Integer,   y As Integer,   b As Integer,   data As DataBuffer) As Integer
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim sample_Renamed As Integer = data.getElem(y * scanlineStride + x)
			Return (CInt(CUInt((sample_Renamed And bitMasks(b))) >> bitOffsets(b)))
		End Function

		''' <summary>
		''' Returns the samples for a specified band for the specified rectangle
		''' of pixels in an int array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="iArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified band for the specified
		'''         region of pixels. </returns>
		''' <seealso cref= #setSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		Public Overrides Function getSamples(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   b As Integer,   iArray As Integer(),   data As DataBuffer) As Integer()
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x + w > width) OrElse (y + h > height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim samples_Renamed As Integer()
			If iArray IsNot Nothing Then
			   samples_Renamed = iArray
			Else
			   samples_Renamed = New Integer (w*h - 1){}
			End If
			Dim lineOffset As Integer = y*scanlineStride + x
			Dim dstOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   For j As Integer = 0 To w - 1
				  Dim value As Integer = data.getElem(lineOffset+j)
				  samples_Renamed(dstOffset) = (CInt(CUInt((value And bitMasks(b))) >> bitOffsets(b)))
				  dstOffset += 1
			   Next j
			   lineOffset += scanlineStride
			Next i
			Return samples_Renamed
		End Function

		''' <summary>
		''' Sets the data for a single pixel in the specified DataBuffer from a
		''' primitive array of type TransferType.  For a
		''' SinglePixelPackedSampleModel, only the first element of the array
		''' will hold valid data, and the type of the array must be the same as
		''' the storage data type of the SinglePixelPackedSampleModel.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SinglePixelPackedSampleModel <code>sppsm1</code>,
		''' to DataBuffer <code>db2</code>, whose storage layout is described by
		''' SinglePixelPackedSampleModel <code>sppsm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixel/setPixel.
		''' <pre>
		'''       SinglePixelPackedSampleModel sppsm1, sppsm2;
		'''       DataBufferInt db1, db2;
		'''       sppsm2.setDataElements(x, y, sppsm1.getDataElements(x, y, null,
		'''                              db1), db2);
		''' </pre>
		''' Using getDataElements/setDataElements to transfer between two
		''' DataBuffer/SampleModel pairs is legitimate if the SampleModels have
		''' the same number of bands, corresponding bands have the same number of
		''' bits per sample, and the TransferTypes are the same.
		''' <p>
		''' obj must be a primitive array of type TransferType.  Otherwise,
		''' a ClassCastException is thrown.  An
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds, or if obj is not large enough to hold the pixel data. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="obj">       A primitive array containing pixel data. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		Public Overrides Sub setDataElements(  x As Integer,   y As Integer,   obj As Object,   data As DataBuffer)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim barray As SByte() = CType(obj, SByte())
				data.elemlem(y*scanlineStride+x, (CInt(barray(0))) And &Hff)

			Case DataBuffer.TYPE_USHORT

				Dim sarray As Short() = CType(obj, Short())
				data.elemlem(y*scanlineStride+x, (CInt(sarray(0))) And &Hffff)

			Case DataBuffer.TYPE_INT

				Dim iarray As Integer() = CType(obj, Integer())
				data.elemlem(y*scanlineStride+x, iarray(0))
			End Select
		End Sub

		''' <summary>
		''' Sets a pixel in the DataBuffer using an int array of samples for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="iArray">    The input samples in an int array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  setPixel(int x, int y, int iArray() , DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim lineOffset As Integer = y * scanlineStride + x
			Dim value As Integer = data.getElem(lineOffset)
			For i As Integer = 0 To numBands - 1
				value = value And Not bitMasks(i)
				value = value Or ((iArray(i) << bitOffsets(i)) And bitMasks(i))
			Next i
			data.elemlem(lineOffset, value)

		''' <summary>
		''' Sets all samples for a rectangle of pixels from an int array containing
		''' one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="iArray">    The input samples in an int array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public  Sub  pixelsels(Integer x, Integer y, Integer w, Integer h, Integer iArray() , DataBuffer data)
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim lineOffset As Integer = y*scanlineStride + x
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   For j As Integer = 0 To w - 1
				   Dim value As Integer = data.getElem(lineOffset+j)
				   For k As Integer = 0 To numBands - 1
					   value = value And Not bitMasks(k)
					   Dim srcValue As Integer = iArray(srcOffset)
					   srcOffset += 1
					   value = value Or ((srcValue << bitOffsets(k)) And bitMasks(k))
				   Next k
				   data.elemlem(lineOffset+j, value)
			   Next j
			   lineOffset += scanlineStride
			Next i

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using an int for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="s">         The input sample as an int. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public  Sub  sampleple(Integer x, Integer y, Integer b, Integer s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim value As Integer = data.getElem(y*scanlineStride + x)
			value = value And Not bitMasks(b)
			value = value Or (s << bitOffsets(b)) And bitMasks(b)
			data.elemlem(y*scanlineStride + x,value)

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from an int array containing one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="iArray">    The input samples in an int array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public  Sub  samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray() , DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x + w > width) OrElse (y + h > height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim lineOffset As Integer = y*scanlineStride + x
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   For j As Integer = 0 To w - 1
				  Dim value As Integer = data.getElem(lineOffset+j)
				  value = value And Not bitMasks(b)
				  Dim sample_Renamed As Integer = iArray(srcOffset)
				  srcOffset += 1
				  value = value Or (CInt(sample_Renamed) << bitOffsets(b)) And bitMasks(b)
				  data.elemlem(lineOffset+j,value)
			   Next j
			   lineOffset += scanlineStride
			Next i

		public Boolean Equals(Object o)
			If (o Is Nothing) OrElse Not(TypeOf o Is SinglePixelPackedSampleModel) Then Return False

			Dim that As SinglePixelPackedSampleModel = CType(o, SinglePixelPackedSampleModel)
			Return Me.width = that.width AndAlso Me.height = that.height AndAlso Me.numBands = that.numBands AndAlso Me.dataType = that.dataType AndAlso java.util.Arrays.Equals(Me.bitMasks, that.bitMasks) AndAlso java.util.Arrays.Equals(Me.bitOffsets, that.bitOffsets) AndAlso java.util.Arrays.Equals(Me.bitSizes, that.bitSizes) AndAlso Me.maxBitSize = that.maxBitSize AndAlso Me.scanlineStride = that.scanlineStride

		' If we implement equals() we must also implement hashCode
		public Integer GetHashCode()
			Dim hash As Integer = 0
			hash = width
			hash <<= 8
			hash = hash Xor height
			hash <<= 8
			hash = hash Xor numBands
			hash <<= 8
			hash = hash Xor dataType
			hash <<= 8
			For i As Integer = 0 To bitMasks.Length - 1
				hash = hash Xor bitMasks(i)
				hash <<= 8
			Next i
			For i As Integer = 0 To bitOffsets.Length - 1
				hash = hash Xor bitOffsets(i)
				hash <<= 8
			Next i
			For i As Integer = 0 To bitSizes.Length - 1
				hash = hash Xor bitSizes(i)
				hash <<= 8
			Next i
			hash = hash Xor maxBitSize
			hash <<= 8
			hash = hash Xor scanlineStride
			Return hash
	End Class

End Namespace