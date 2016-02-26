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
	'''  This class represents image data which is stored in a band interleaved
	'''  fashion and for
	'''  which each sample of a pixel occupies one data element of the DataBuffer.
	'''  It subclasses ComponentSampleModel but provides a more efficient
	'''  implementation for accessing band interleaved image data than is provided
	'''  by ComponentSampleModel.  This class should typically be used when working
	'''  with images which store sample data for each band in a different bank of the
	'''  DataBuffer. Accessor methods are provided so that image data can be
	'''  manipulated directly. Pixel stride is the number of
	'''  data array elements between two samples for the same band on the same
	'''  scanline. The pixel stride for a BandedSampleModel is one.
	'''  Scanline stride is the number of data array elements between
	'''  a given sample and the corresponding sample in the same column of the next
	'''  scanline.  Band offsets denote the number
	'''  of data array elements from the first data array element of the bank
	'''  of the DataBuffer holding each band to the first sample of the band.
	'''  The bands are numbered from 0 to N-1.
	'''  Bank indices denote the correspondence between a bank of the data buffer
	'''  and a band of image data.  This class supports
	'''  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	'''  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	'''  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/>, and
	'''  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/> datatypes
	''' </summary>


	Public NotInheritable Class BandedSampleModel
		Inherits ComponentSampleModel

		''' <summary>
		''' Constructs a BandedSampleModel with the specified parameters.
		''' The pixel stride will be one data element.  The scanline stride
		''' will be the same as the width.  Each band will be stored in
		''' a separate bank and all band offsets will be zero. </summary>
		''' <param name="dataType">  The data type for storing samples. </param>
		''' <param name="w">         The width (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="h">         The height (in pixels) of the region of image
		'''                  data described. </param>
		''' <param name="numBands">  The number of bands for the image data. </param>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		Public Sub New(ByVal dataType As Integer, ByVal w As Integer, ByVal h As Integer, ByVal numBands As Integer)
			MyBase.New(dataType, w, h, 1, w, BandedSampleModel.createIndicesArray(numBands), BandedSampleModel.createOffsetArray(numBands))
		End Sub

		''' <summary>
		''' Constructs a BandedSampleModel with the specified parameters.
		''' The number of bands will be inferred from the lengths of the
		''' bandOffsets bankIndices arrays, which must be equal.  The pixel
		''' stride will be one data element. </summary>
		''' <param name="dataType">  The data type for storing samples. </param>
		''' <param name="w">         The width (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="h">         The height (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="scanlineStride"> The line stride of the of the image data. </param>
		''' <param name="bankIndices"> The bank index for each band. </param>
		''' <param name="bandOffsets"> The band offset for each band. </param>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public BandedSampleModel(int dataType, int w, int h, int scanlineStride, int bankIndices() , int bandOffsets())

			MyBase(dataType, w, h, 1,scanlineStride, bankIndices, bandOffsets)

		''' <summary>
		''' Creates a new BandedSampleModel with the specified
		''' width and height.  The new BandedSampleModel will have the same
		''' number of bands, storage data type, and bank indices
		''' as this BandedSampleModel.  The band offsets will be compressed
		''' such that the offset between bands will be w*pixelStride and
		''' the minimum of all of the band offsets is zero. </summary>
		''' <param name="w"> the width of the resulting <code>BandedSampleModel</code> </param>
		''' <param name="h"> the height of the resulting <code>BandedSampleModel</code> </param>
		''' <returns> a new <code>BandedSampleModel</code> with the specified
		'''         width and height. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> equals either
		'''         <code> [Integer].MAX_VALUE</code> or
		'''         <code>Integer.MIN_VALUE</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		public SampleModel createCompatibleSampleModel(Integer w, Integer h)
			Dim bandOffs As Integer()

			If numBanks = 1 Then
				bandOffs = orderBands(bandOffsets, w*h)
			Else
				bandOffs = New Integer(bandOffsets.Length - 1){}
			End If

			Dim sampleModel_Renamed As SampleModel = New BandedSampleModel(dataType, w, h, w, bankIndices, bandOffs)
			Return sampleModel_Renamed

		''' <summary>
		''' Creates a new BandedSampleModel with a subset of the bands of this
		''' BandedSampleModel.  The new BandedSampleModel can be
		''' used with any DataBuffer that the existing BandedSampleModel
		''' can be used with.  The new BandedSampleModel/DataBuffer
		''' combination will represent an image with a subset of the bands
		''' of the original BandedSampleModel/DataBuffer combination. </summary>
		''' <exception cref="RasterFormatException"> if the number of bands is greater than
		'''                               the number of banks in this sample model. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		public SampleModel createSubsetSampleModel(Integer bands())
			If bands.length > bankIndices.Length Then Throw New RasterFormatException("There are only " & bankIndices.Length & " bands")
			Dim newBankIndices As Integer() = New Integer(bands.length - 1){}
			Dim newBandOffsets As Integer() = New Integer(bands.length - 1){}

			For i As Integer = 0 To bands.length - 1
				newBankIndices(i) = bankIndices(bands(i))
				newBandOffsets(i) = bandOffsets(bands(i))
			Next i

			Return New BandedSampleModel(Me.dataType, width, height, Me.scanlineStride, newBankIndices, newBandOffsets)

		''' <summary>
		''' Creates a DataBuffer that corresponds to this BandedSampleModel,
		''' The DataBuffer's data type, number of banks, and size
		''' will be consistent with this BandedSampleModel. </summary>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported types. </exception>
		public DataBuffer createDataBuffer()
			Dim dataBuffer_Renamed As DataBuffer = Nothing

			Dim size As Integer = scanlineStride * height
			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				dataBuffer_Renamed = New DataBufferByte(size, numBanks)
			Case DataBuffer.TYPE_USHORT
				dataBuffer_Renamed = New DataBufferUShort(size, numBanks)
			Case DataBuffer.TYPE_SHORT
				dataBuffer_Renamed = New DataBufferShort(size, numBanks)
			Case DataBuffer.TYPE_INT
				dataBuffer_Renamed = New DataBufferInt(size, numBanks)
			Case DataBuffer.TYPE_FLOAT
				dataBuffer_Renamed = New DataBufferFloat(size, numBanks)
			Case DataBuffer.TYPE_DOUBLE
				dataBuffer_Renamed = New DataBufferDouble(size, numBanks)
			Case Else
				Throw New IllegalArgumentException("dataType is not one " & "of the supported types.")
			End Select

			Return dataBuffer_Renamed


        ''' <summary>
        ''' Returns data for a single pixel in a primitive array of type
        ''' TransferType.  For a BandedSampleModel, this will be the same
        ''' as the data type, and samples will be returned one per array
        ''' element.  Generally, obj
        ''' should be passed in as null, so that the Object will be created
        ''' automatically and will be of the right primitive data type.
        ''' <p>
        ''' The following code illustrates transferring data for one pixel from
        ''' DataBuffer <code>db1</code>, whose storage layout is described by
        ''' BandedSampleModel <code>bsm1</code>, to DataBuffer <code>db2</code>,
        ''' whose storage layout is described by
        ''' BandedSampleModel <code>bsm2</code>.
        ''' The transfer will generally be more efficient than using
        ''' getPixel/setPixel.
        ''' <pre>
        '''       BandedSampleModel bsm1, bsm2;
        '''       DataBufferInt db1, db2;
        '''       bsm2.setDataElements(x, y, bsm1.getDataElements(x, y, null, db1),
        '''                            db2);
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
        ''' <param name="x">         The X coordinate of the pixel location </param>
        ''' <param name="y">         The Y coordinate of the pixel location </param>
        ''' <param name="obj">       If non-null, a primitive array in which to return
        '''                  the pixel data. </param>
        ''' <param name="data">      The DataBuffer containing the image data. </param>
        ''' <returns> the data for the specified pixel. </returns>
        ''' <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
        Public Overrides Function getDataElements(x As Integer, y As Integer, obj As Object, data As DataBuffer) As Object
            If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
            Dim type As Integer = transferType
            Dim numDataElems As Integer = numDataElements
            Dim pixelOffset As Integer = y * scanlineStride + x

            Select Case type

                Case DataBuffer.TYPE_BYTE

                    Dim bdata As SByte()

                    If obj Is Nothing Then
                        bdata = New SByte(numDataElems - 1) {}
                    Else
                        bdata = CType(obj, SByte())
                    End If

                    For i As Integer = 0 To numDataElems - 1
                        bdata(i) = CByte(data.getElem(bankIndices(i), pixelOffset + bandOffsets(i)))
                    Next i

                    obj = CObj(bdata)

                Case DataBuffer.TYPE_USHORT, DataBuffer.TYPE_SHORT

                    Dim sdata As Short()

                    If obj Is Nothing Then
                        sdata = New Short(numDataElems - 1) {}
                    Else
                        sdata = CType(obj, Short())
                    End If

                    For i As Integer = 0 To numDataElems - 1
                        sdata(i) = CShort(Fix(data.getElem(bankIndices(i), pixelOffset + bandOffsets(i))))
                    Next i

                    obj = CObj(sdata)

                Case DataBuffer.TYPE_INT

                    Dim idata As Integer()

                    If obj Is Nothing Then
                        idata = New Integer(numDataElems - 1) {}
                    Else
                        idata = CType(obj, Integer())
                    End If

                    For i As Integer = 0 To numDataElems - 1
                        idata(i) = data.getElem(bankIndices(i), pixelOffset + bandOffsets(i))
                    Next i

                    obj = CObj(idata)

                Case DataBuffer.TYPE_FLOAT

                    Dim fdata As Single()

                    If obj Is Nothing Then
                        fdata = New Single(numDataElems - 1) {}
                    Else
                        fdata = CType(obj, Single())
                    End If

                    For i As Integer = 0 To numDataElems - 1
                        fdata(i) = data.getElemFloat(bankIndices(i), pixelOffset + bandOffsets(i))
                    Next i

                    obj = CObj(fdata)

                Case DataBuffer.TYPE_DOUBLE

                    Dim ddata As Double()

                    If obj Is Nothing Then
                        ddata = New Double(numDataElems - 1) {}
                    Else
                        ddata = CType(obj, Double())
                    End If

                    For i As Integer = 0 To numDataElems - 1
                        ddata(i) = data.getElemDouble(bankIndices(i), pixelOffset + bandOffsets(i))
                    Next i

                    obj = CObj(ddata)
            End Select

            Return obj
        End Function
        ''' <summary>
        ''' Returns all samples for the specified pixel in an int array.
        ''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
        ''' not in bounds. </summary>
        ''' <param name="x">         The X coordinate of the pixel location </param>
        ''' <param name="y">         The Y coordinate of the pixel location </param>
        ''' <param name="iArray">    If non-null, returns the samples in this array </param>
        ''' <param name="data">      The DataBuffer containing the image data </param>
        ''' <returns> the samples for the specified pixel. </returns>
        ''' <seealso cref= #setPixel(int, int, int[], DataBuffer) </seealso>
        Public Integer() getPixel(Integer x, Integer y, Integer iArray(), DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim pixels_Renamed As Integer()

			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer (numBands - 1){}
			End If

			Dim pixelOffset As Integer = y*scanlineStride + x
			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = data.getElem(bankIndices(i), pixelOffset + bandOffsets(i))
			Next i
			Return pixels_Renamed

		''' <summary>
		''' Returns all samples for the specified rectangle of pixels in
		''' an int array, one sample per data array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the samples for the pixels within the specified region. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public Integer() getPixels(Integer x, Integer y, Integer w, Integer h, Integer iArray(), DataBuffer data)
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()

			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer(w*h*numBands - 1){}
			End If

			For k As Integer = 0 To numBands - 1
				Dim lineOffset As Integer = y*scanlineStride + x + bandOffsets(k)
				Dim srcOffset As Integer = k
				Dim bank As Integer = bankIndices(k)

				For i As Integer = 0 To h - 1
					Dim pixelOffset As Integer = lineOffset
					For j As Integer = 0 To w - 1
						pixels_Renamed(srcOffset) = data.getElem(bank, pixelOffset)
						pixelOffset += 1
						srcOffset += numBands
					Next j
					lineOffset += scanlineStride
				Next i
			Next k
			Return pixels_Renamed

		''' <summary>
		''' Returns as int the sample in a specified band for the pixel
		''' located at (x,y).
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to return </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the sample in the specified band for the specified pixel. </returns>
		''' <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public Integer getSample(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim sample_Renamed As Integer = data.getElem(bankIndices(b), y*scanlineStride + x + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the sample in a specified band
		''' for the pixel located at (x,y) as a float.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to return </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> a float value that represents the sample in the specified
		''' band for the specified pixel. </returns>
		public Single getSampleFloat(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim sample_Renamed As Single = data.getElemFloat(bankIndices(b), y*scanlineStride + x + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the sample in a specified band
		''' for a pixel located at (x,y) as a double.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to return </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> a double value that represents the sample in the specified
		''' band for the specified pixel. </returns>
		public Double getSampleDouble(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim sample_Renamed As Double = data.getElemDouble(bankIndices(b), y*scanlineStride + x + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the samples in a specified band for the specified rectangle
		''' of pixels in an int array, one sample per data array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="b">         The band to return </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the samples in the specified band for the pixels within
		''' the specified region. </returns>
		''' <seealso cref= #setSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public Integer() getSamples(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray(), DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x + w > width) OrElse (y + h > height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim samples_Renamed As Integer()
			If iArray IsNot Nothing Then
			   samples_Renamed = iArray
			Else
			   samples_Renamed = New Integer (w*h - 1){}
			End If

			Dim lineOffset As Integer = y*scanlineStride + x + bandOffsets(b)
			Dim srcOffset As Integer = 0
			Dim bank As Integer = bankIndices(b)

			For i As Integer = 0 To h - 1
			   Dim sampleOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				   samples_Renamed(srcOffset) = data.getElem(bank, sampleOffset)
				   sampleOffset += 1
				   srcOffset += 1
			   Next j
			   lineOffset += scanlineStride
			Next i
			Return samples_Renamed

		''' <summary>
		''' Sets the data for a single pixel in the specified DataBuffer from a
		''' primitive array of type TransferType.  For a BandedSampleModel,
		''' this will be the same as the data type, and samples are transferred
		''' one per array element.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' BandedSampleModel <code>bsm1</code>, to DataBuffer <code>db2</code>,
		''' whose storage layout is described by
		''' BandedSampleModel <code>bsm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixel/setPixel.
		''' <pre>
		'''       BandedSampleModel bsm1, bsm2;
		'''       DataBufferInt db1, db2;
		'''       bsm2.setDataElements(x, y, bsm1.getDataElements(x, y, null, db1),
		'''                            db2);
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
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="obj">       If non-null, returns the primitive array in this
		'''                  object </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public void dataElementsnts(Integer x, Integer y, Object obj, DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim type As Integer = transferType
			Dim numDataElems As Integer = numDataElements
			Dim pixelOffset As Integer = y*scanlineStride + x

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim barray As SByte() = CType(obj, SByte())

				For i As Integer = 0 To numDataElems - 1
					data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), barray(i) And &Hff)
				Next i

			Case DataBuffer.TYPE_USHORT, DataBuffer.TYPE_SHORT

				Dim sarray As Short() = CType(obj, Short())

				For i As Integer = 0 To numDataElems - 1
					data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), sarray(i) And &Hffff)
				Next i

			Case DataBuffer.TYPE_INT

				Dim iarray As Integer() = CType(obj, Integer())

				For i As Integer = 0 To numDataElems - 1
					data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), iarray(i))
				Next i

			Case DataBuffer.TYPE_FLOAT

				Dim farray As Single() = CType(obj, Single())

				For i As Integer = 0 To numDataElems - 1
					data.elemFloatoat(bankIndices(i), pixelOffset + bandOffsets(i), farray(i))
				Next i

			Case DataBuffer.TYPE_DOUBLE

				Dim darray As Double() = CType(obj, Double())

				For i As Integer = 0 To numDataElems - 1
					data.elemDoubleble(bankIndices(i), pixelOffset + bandOffsets(i), darray(i))
				Next i

			End Select

		''' <summary>
		''' Sets a pixel in the DataBuffer using an int array of samples for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="iArray">    The input samples in an int array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		public void pixelxel(Integer x, Integer y, Integer iArray() , DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
		   Dim pixelOffset As Integer = y*scanlineStride + x
		   For i As Integer = 0 To numBands - 1
			   data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), iArray(i))
		   Next i

		''' <summary>
		''' Sets all samples for a rectangle of pixels from an int array containing
		''' one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="iArray">    The input samples in an int array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, Integer iArray() , DataBuffer data)
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			For k As Integer = 0 To numBands - 1
				Dim lineOffset As Integer = y*scanlineStride + x + bandOffsets(k)
				Dim srcOffset As Integer = k
				Dim bank As Integer = bankIndices(k)

				For i As Integer = 0 To h - 1
					Dim pixelOffset As Integer = lineOffset
					For j As Integer = 0 To w - 1
						data.elemlem(bank, pixelOffset, iArray(srcOffset))
						pixelOffset += 1
						srcOffset += numBands
					Next j
					lineOffset += scanlineStride
				Next i
			Next k

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using an int for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to set </param>
		''' <param name="s">         The input sample as an int </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public void sampleple(Integer x, Integer y, Integer b, Integer s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemlem(bankIndices(b), y*scanlineStride + x + bandOffsets(b), s)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a float for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to set </param>
		''' <param name="s">         The input sample as a float </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public void sampleple(Integer x, Integer y, Integer b, Single s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemFloatoat(bankIndices(b), y*scanlineStride + x + bandOffsets(b), s)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a double for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to set </param>
		''' <param name="s">         The input sample as a double </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public void sampleple(Integer x, Integer y, Integer b, Double s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemDoubleble(bankIndices(b), y*scanlineStride + x + bandOffsets(b), s)

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from an int array containing one sample per data array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="b">         The band to set </param>
		''' <param name="iArray">    The input sample array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public void samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray() , DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x + w > width) OrElse (y + h > height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim lineOffset As Integer = y*scanlineStride + x + bandOffsets(b)
			Dim srcOffset As Integer = 0
			Dim bank As Integer = bankIndices(b)

			For i As Integer = 0 To h - 1
			   Dim sampleOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				  data.elemlem(bank, sampleOffset, iArray(srcOffset))
				  srcOffset += 1
				  sampleOffset += 1
			   Next j
			   lineOffset += scanlineStride
			Next i

		private static Integer() createOffsetArray(Integer numBands)
			Dim bandOffsets_Renamed As Integer() = New Integer(numBands - 1){}
			For i As Integer = 0 To numBands - 1
				bandOffsets_Renamed(i) = 0
			Next i
			Return bandOffsets_Renamed

		private static Integer() createIndicesArray(Integer numBands)
			Dim bankIndices_Renamed As Integer() = New Integer(numBands - 1){}
			For i As Integer = 0 To numBands - 1
				bankIndices_Renamed(i) = i
			Next i
			Return bankIndices_Renamed

		' Differentiate hash code from other ComponentSampleModel subclasses
		public Integer GetHashCode()
			Return MyBase.GetHashCode() Xor &H2
	End Class

End Namespace