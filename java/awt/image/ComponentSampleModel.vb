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
	'''  This class represents image data which is stored such that each sample
	'''  of a pixel occupies one data element of the DataBuffer.  It stores the
	'''  N samples which make up a pixel in N separate data array elements.
	'''  Different bands may be in different banks of the DataBuffer.
	'''  Accessor methods are provided so that image data can be manipulated
	'''  directly. This class can support different kinds of interleaving, e.g.
	'''  band interleaving, scanline interleaving, and pixel interleaving.
	'''  Pixel stride is the number of data array elements between two samples
	'''  for the same band on the same scanline. Scanline stride is the number
	'''  of data array elements between a given sample and the corresponding sample
	'''  in the same column of the next scanline.  Band offsets denote the number
	'''  of data array elements from the first data array element of the bank
	'''  of the DataBuffer holding each band to the first sample of the band.
	'''  The bands are numbered from 0 to N-1.  This class can represent image
	'''  data for which each sample is an unsigned integral number which can be
	'''  stored in 8, 16, or 32 bits (using <code>DataBuffer.TYPE_BYTE</code>,
	'''  <code>DataBuffer.TYPE_USHORT</code>, or <code>DataBuffer.TYPE_INT</code>,
	'''  respectively), data for which each sample is a signed integral number
	'''  which can be stored in 16 bits (using <code>DataBuffer.TYPE_SHORT</code>),
	'''  or data for which each sample is a signed float or double quantity
	'''  (using <code>DataBuffer.TYPE_FLOAT</code> or
	'''  <code>DataBuffer.TYPE_DOUBLE</code>, respectively).
	'''  All samples of a given ComponentSampleModel
	'''  are stored with the same precision.  All strides and offsets must be
	'''  non-negative.  This class supports
	'''  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	'''  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	'''  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/>,
	'''  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/>, </summary>
	'''  <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	'''  <seealso cref= java.awt.image.BandedSampleModel </seealso>

	Public Class ComponentSampleModel
		Inherits SampleModel

		''' <summary>
		''' Offsets for all bands in data array elements. </summary>
		Protected Friend bandOffsets As Integer()

		''' <summary>
		''' Index for each bank storing a band of image data. </summary>
		Protected Friend bankIndices As Integer()

		''' <summary>
		''' The number of bands in this
		''' <code>ComponentSampleModel</code>.
		''' </summary>
		Protected Friend Shadows numBands As Integer = 1

		''' <summary>
		''' The number of banks in this
		''' <code>ComponentSampleModel</code>.
		''' </summary>
		Protected Friend numBanks As Integer = 1

		''' <summary>
		'''  Line stride (in data array elements) of the region of image
		'''  data described by this ComponentSampleModel.
		''' </summary>
		Protected Friend scanlineStride As Integer

		''' <summary>
		''' Pixel stride (in data array elements) of the region of image
		'''  data described by this ComponentSampleModel.
		''' </summary>
		Protected Friend pixelStride As Integer

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Constructs a ComponentSampleModel with the specified parameters.
		''' The number of bands will be given by the length of the bandOffsets array.
		''' All bands will be stored in the first bank of the DataBuffer. </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width (in pixels) of the region of
		'''     image data described </param>
		''' <param name="h">         the height (in pixels) of the region of
		'''     image data described </param>
		''' <param name="pixelStride"> the pixel stride of the region of image
		'''     data described </param>
		''' <param name="scanlineStride"> the line stride of the region of image
		'''     data described </param>
		''' <param name="bandOffsets"> the offsets of all bands </param>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>pixelStride</code>
		'''         is less than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>scanlineStride</code>
		'''         is less than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>numBands</code>
		'''         is less than 1 </exception>
		''' <exception cref="IllegalArgumentException"> if the product of <code>w</code>
		'''         and <code>h</code> is greater than
		'''         <code> java.lang.[Integer].MAX_VALUE</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		Public Sub New(ByVal dataType As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixelStride As Integer, ByVal scanlineStride As Integer, ByVal bandOffsets As Integer())
			MyBase.New(dataType, w, h, bandOffsets.Length)
			Me.dataType = dataType
			Me.pixelStride = pixelStride
			Me.scanlineStride = scanlineStride
			Me.bandOffsets = CType(bandOffsets.clone(), Integer())
			numBands = Me.bandOffsets.Length
			If pixelStride < 0 Then Throw New IllegalArgumentException("Pixel stride must be >= 0")
			' TODO - bug 4296691 - remove this check
			If scanlineStride < 0 Then Throw New IllegalArgumentException("Scanline stride must be >= 0")
			If numBands < 1 Then Throw New IllegalArgumentException("Must have at least one band.")
			If (dataType < DataBuffer.TYPE_BYTE) OrElse (dataType > DataBuffer.TYPE_DOUBLE) Then Throw New IllegalArgumentException("Unsupported dataType.")
			bankIndices = New Integer(numBands - 1){}
			For i As Integer = 0 To numBands - 1
				bankIndices(i) = 0
			Next i
			verify()
		End Sub


		''' <summary>
		''' Constructs a ComponentSampleModel with the specified parameters.
		''' The number of bands will be given by the length of the bandOffsets array.
		''' Different bands may be stored in different banks of the DataBuffer.
		''' </summary>
		''' <param name="dataType">  the data type for storing samples </param>
		''' <param name="w">         the width (in pixels) of the region of
		'''     image data described </param>
		''' <param name="h">         the height (in pixels) of the region of
		'''     image data described </param>
		''' <param name="pixelStride"> the pixel stride of the region of image
		'''     data described </param>
		''' <param name="scanlineStride"> The line stride of the region of image
		'''     data described </param>
		''' <param name="bankIndices"> the bank indices of all bands </param>
		''' <param name="bandOffsets"> the band offsets of all bands </param>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>pixelStride</code>
		'''         is less than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>scanlineStride</code>
		'''         is less than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if the length of
		'''         <code>bankIndices</code> does not equal the length of
		'''         <code>bankOffsets</code> </exception>
		''' <exception cref="IllegalArgumentException"> if any of the bank indices
		'''         of <code>bandIndices</code> is less than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public ComponentSampleModel(int dataType, int w, int h, int pixelStride, int scanlineStride, int bankIndices() , int bandOffsets())
			MyBase(dataType, w, h, bandOffsets.Length)
			Me.dataType = dataType
			Me.pixelStride = pixelStride
			Me.scanlineStride = scanlineStride
			Me.bandOffsets = CType(bandOffsets.clone(), Integer())
			Me.bankIndices = CType(bankIndices.clone(), Integer())
			If pixelStride < 0 Then Throw New IllegalArgumentException("Pixel stride must be >= 0")
			' TODO - bug 4296691 - remove this check
			If scanlineStride < 0 Then Throw New IllegalArgumentException("Scanline stride must be >= 0")
			If (dataType < DataBuffer.TYPE_BYTE) OrElse (dataType > DataBuffer.TYPE_DOUBLE) Then Throw New IllegalArgumentException("Unsupported dataType.")
			Dim maxBank As Integer = Me.bankIndices(0)
			If maxBank < 0 Then Throw New IllegalArgumentException("Index of bank 0 is less than " & "0 (" & maxBank & ")")
			For i As Integer = 1 To Me.bankIndices.Length - 1
				If Me.bankIndices(i) > maxBank Then
					maxBank = Me.bankIndices(i)
				ElseIf Me.bankIndices(i) < 0 Then
					Throw New IllegalArgumentException("Index of bank " & i & " is less than 0 (" & maxBank & ")")
				End If
			Next i
			numBanks = maxBank+1
			numBands = Me.bandOffsets.Length
			If Me.bandOffsets.Length <> Me.bankIndices.Length Then Throw New IllegalArgumentException("Length of bandOffsets must " & "equal length of bankIndices.")
			verify()

		private  Sub  verify()
			Dim requiredSize As Integer = bufferSize

		''' <summary>
		''' Returns the size of the data buffer (in data elements) needed
		''' for a data buffer that matches this ComponentSampleModel.
		''' </summary>
		 private Integer bufferSize
			 Dim maxBandOff As Integer=bandOffsets(0)
			 For i As Integer = 1 To bandOffsets.Length - 1
				 maxBandOff = System.Math.Max(maxBandOff,bandOffsets(i))
			 Next i

			 If maxBandOff < 0 OrElse maxBandOff > ( java.lang.[Integer].Max_Value - 1) Then Throw New IllegalArgumentException("Invalid band offset")

			 If pixelStride < 0 OrElse pixelStride > ( java.lang.[Integer].Max_Value / width) Then Throw New IllegalArgumentException("Invalid pixel stride")

			 If scanlineStride < 0 OrElse scanlineStride > ( java.lang.[Integer].Max_Value / height) Then Throw New IllegalArgumentException("Invalid scanline stride")

			 Dim size As Integer = maxBandOff + 1

			 Dim val As Integer = pixelStride * (width - 1)

			 If val > ( java.lang.[Integer].Max_Value - size) Then Throw New IllegalArgumentException("Invalid pixel stride")

			 size += val

			 val = scanlineStride * (height - 1)

			 If val > ( java.lang.[Integer].Max_Value - size) Then Throw New IllegalArgumentException("Invalid scan stride")

			 size += val

			 Return size

		 ''' <summary>
		 ''' Preserves band ordering with new step factor...
		 ''' </summary>
		Integer () orderBands(Integer orig(), Integer step)
			Dim map As Integer() = New Integer(orig.length - 1){}
			Dim ret As Integer() = New Integer(orig.length - 1){}

			For i As Integer = 0 To map.Length - 1
				map(i) = i
			Next i

			For i As Integer = 0 To ret.Length - 1
				Dim index As Integer = i
				For j As Integer = i+1 To ret.Length - 1
					If orig(map(index)) > orig(map(j)) Then index = j
				Next j
				ret(map(index)) = i*step
				map(index) = map(i)
			Next i
			Return ret

		''' <summary>
		''' Creates a new <code>ComponentSampleModel</code> with the specified
		''' width and height.  The new <code>SampleModel</code> will have the same
		''' number of bands, storage data type, interleaving scheme, and
		''' pixel stride as this <code>SampleModel</code>. </summary>
		''' <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		''' <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		''' <returns> a new <code>ComponentSampleModel</code> with the specified size </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		public SampleModel createCompatibleSampleModel(Integer w, Integer h)
			Dim ret As SampleModel=Nothing
			Dim size As Long
			Dim minBandOff As Integer=bandOffsets(0)
			Dim maxBandOff As Integer=bandOffsets(0)
			For i As Integer = 1 To bandOffsets.Length - 1
				minBandOff = System.Math.Min(minBandOff,bandOffsets(i))
				maxBandOff = System.Math.Max(maxBandOff,bandOffsets(i))
			Next i
			maxBandOff -= minBandOff

			Dim bands As Integer = bandOffsets.Length
			Dim bandOff As Integer()
			Dim pStride As Integer = System.Math.Abs(pixelStride)
			Dim lStride As Integer = System.Math.Abs(scanlineStride)
			Dim bStride As Integer = System.Math.Abs(maxBandOff)

			If pStride > lStride Then
				If pStride > bStride Then
					If lStride > bStride Then ' pix > line > band
						bandOff = New Integer(bandOffsets.Length - 1){}
						For i As Integer = 0 To bands - 1
							bandOff(i) = bandOffsets(i)-minBandOff
						Next i
						lStride = bStride+1
						pStride = lStride*h ' pix > band > line
					Else
						bandOff = orderBands(bandOffsets,lStride*h)
						pStride = bands*lStride*h
					End If ' band > pix > line
				Else
					pStride = lStride*h
					bandOff = orderBands(bandOffsets,pStride*w)
				End If
			Else
				If pStride > bStride Then ' line > pix > band
					bandOff = New Integer(bandOffsets.Length - 1){}
					For i As Integer = 0 To bands - 1
						bandOff(i) = bandOffsets(i)-minBandOff
					Next i
					pStride = bStride+1
					lStride = pStride*w
				Else
					If lStride > bStride Then ' line > band > pix
						bandOff = orderBands(bandOffsets,pStride*w)
						lStride = bands*pStride*w ' band > line > pix
					Else
						lStride = pStride*w
						bandOff = orderBands(bandOffsets,lStride*h)
					End If
				End If
			End If

			' make sure we make room for negative offsets...
			Dim base As Integer = 0
			If scanlineStride < 0 Then
				base += lStride*h
				lStride *= -1
			End If
			If pixelStride < 0 Then
				base += pStride*w
				pStride *= -1
			End If

			For i As Integer = 0 To bands - 1
				bandOff(i) += base
			Next i
			Return New ComponentSampleModel(dataType, w, h, pStride, lStride, bankIndices, bandOff)

		''' <summary>
		''' Creates a new ComponentSampleModel with a subset of the bands
		''' of this ComponentSampleModel.  The new ComponentSampleModel can be
		''' used with any DataBuffer that the existing ComponentSampleModel
		''' can be used with.  The new ComponentSampleModel/DataBuffer
		''' combination will represent an image with a subset of the bands
		''' of the original ComponentSampleModel/DataBuffer combination. </summary>
		''' <param name="bands"> a subset of bands from this
		'''              <code>ComponentSampleModel</code> </param>
		''' <returns> a <code>ComponentSampleModel</code> created with a subset
		'''          of bands from this <code>ComponentSampleModel</code>. </returns>
		public SampleModel createSubsetSampleModel(Integer bands())
		   If bands.length > bankIndices.Length Then Throw New RasterFormatException("There are only " & bankIndices.Length & " bands")
			Dim newBankIndices As Integer() = New Integer(bands.length - 1){}
			Dim newBandOffsets As Integer() = New Integer(bands.length - 1){}

			For i As Integer = 0 To bands.length - 1
				newBankIndices(i) = bankIndices(bands(i))
				newBandOffsets(i) = bandOffsets(bands(i))
			Next i

			Return New ComponentSampleModel(Me.dataType, width, height, Me.pixelStride, Me.scanlineStride, newBankIndices, newBandOffsets)

		''' <summary>
		''' Creates a <code>DataBuffer</code> that corresponds to this
		''' <code>ComponentSampleModel</code>.
		''' The <code>DataBuffer</code> object's data type, number of banks,
		''' and size are be consistent with this <code>ComponentSampleModel</code>. </summary>
		''' <returns> a <code>DataBuffer</code> whose data type, number of banks
		'''         and size are consistent with this
		'''         <code>ComponentSampleModel</code>. </returns>
		public DataBuffer createDataBuffer()
			Dim dataBuffer_Renamed As DataBuffer = Nothing

			Dim size As Integer = bufferSize
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
			End Select

			Return dataBuffer_Renamed


		''' <summary>
		''' Gets the offset for the first band of pixel (x,y).
		'''  A sample of the first band can be retrieved from a
		''' <code>DataBuffer</code>
		'''  <code>data</code> with a <code>ComponentSampleModel</code>
		''' <code>csm</code> as
		''' <pre>
		'''        data.getElem(csm.getOffset(x, y));
		''' </pre> </summary>
		''' <param name="x"> the X location of the pixel </param>
		''' <param name="y"> the Y location of the pixel </param>
		''' <returns> the offset for the first band of the specified pixel. </returns>
		public Integer getOffset(Integer x, Integer y)
			Dim offset_Renamed As Integer = y*scanlineStride + x*pixelStride + bandOffsets(0)
			Return offset_Renamed

		''' <summary>
		''' Gets the offset for band b of pixel (x,y).
		'''  A sample of band <code>b</code> can be retrieved from a
		'''  <code>DataBuffer</code> <code>data</code>
		'''  with a <code>ComponentSampleModel</code> <code>csm</code> as
		''' <pre>
		'''       data.getElem(csm.getOffset(x, y, b));
		''' </pre> </summary>
		''' <param name="x"> the X location of the specified pixel </param>
		''' <param name="y"> the Y location of the specified pixel </param>
		''' <param name="b"> the specified band </param>
		''' <returns> the offset for the specified band of the specified pixel. </returns>
		public Integer getOffset(Integer x, Integer y, Integer b)
			Dim offset_Renamed As Integer = y*scanlineStride + x*pixelStride + bandOffsets(b)
			Return offset_Renamed

		''' <summary>
		''' Returns the number of bits per sample for all bands. </summary>
		'''  <returns> an array containing the number of bits per sample
		'''          for all bands, where each element in the array
		'''          represents a band. </returns>
		public final Integer() sampleSize
			Dim sampleSize_Renamed As Integer() = New Integer (numBands - 1){}
			Dim sizeInBits As Integer = getSampleSize(0)

			For i As Integer = 0 To numBands - 1
				sampleSize_Renamed(i) = sizeInBits
			Next i

			Return sampleSize_Renamed

		''' <summary>
		''' Returns the number of bits per sample for the specified band. </summary>
		'''  <param name="band"> the specified band </param>
		'''  <returns> the number of bits per sample for the specified band. </returns>
		public final Integer getSampleSize(Integer band)
			Return DataBuffer.getDataTypeSize(dataType)

		''' <summary>
		''' Returns the bank indices for all bands. </summary>
		'''  <returns> the bank indices for all bands. </returns>
		public final Integer () bankIndices
			Return CType(bankIndices.clone(), Integer())

		''' <summary>
		''' Returns the band offset for all bands. </summary>
		'''  <returns> the band offsets for all bands. </returns>
		public final Integer () bandOffsets
			Return CType(bandOffsets.clone(), Integer())

		''' <summary>
		''' Returns the scanline stride of this ComponentSampleModel. </summary>
		'''  <returns> the scanline stride of this <code>ComponentSampleModel</code>. </returns>
		public final Integer scanlineStride
			Return scanlineStride

		''' <summary>
		''' Returns the pixel stride of this ComponentSampleModel. </summary>
		'''  <returns> the pixel stride of this <code>ComponentSampleModel</code>. </returns>
		public final Integer pixelStride
			Return pixelStride

		''' <summary>
		''' Returns the number of data elements needed to transfer a pixel
		''' with the
		''' <seealso cref="#getDataElements(int, int, Object, DataBuffer) "/> and
		''' <seealso cref="#setDataElements(int, int, Object, DataBuffer) "/>
		''' methods.
		''' For a <code>ComponentSampleModel</code>, this is identical to the
		''' number of bands. </summary>
		''' <returns> the number of data elements needed to transfer a pixel with
		'''         the <code>getDataElements</code> and
		'''         <code>setDataElements</code> methods. </returns>
		''' <seealso cref= java.awt.image.SampleModel#getNumDataElements </seealso>
		''' <seealso cref= #getNumBands </seealso>
		public final Integer numDataElements
			Return numBands

		''' <summary>
		''' Returns data for a single pixel in a primitive array of type
		''' <code>TransferType</code>.  For a <code>ComponentSampleModel</code>,
		''' this is the same as the data type, and samples are returned
		''' one per array element.  Generally, <code>obj</code> should
		''' be passed in as <code>null</code>, so that the <code>Object</code>
		''' is created automatically and is the right primitive data type.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		''' described by <code>ComponentSampleModel</code> <code>csm1</code>,
		''' to <code>DataBuffer</code> <code>db2</code>, whose storage layout
		''' is described by <code>ComponentSampleModel</code> <code>csm2</code>.
		''' The transfer is usually more efficient than using
		''' <code>getPixel</code> and <code>setPixel</code>.
		''' <pre>
		'''       ComponentSampleModel csm1, csm2;
		'''       DataBufferInt db1, db2;
		'''       csm2.setDataElements(x, y,
		'''                            csm1.getDataElements(x, y, null, db1), db2);
		''' </pre>
		''' 
		''' Using <code>getDataElements</code> and <code>setDataElements</code>
		''' to transfer between two <code>DataBuffer/SampleModel</code>
		''' pairs is legitimate if the <code>SampleModel</code> objects have
		''' the same number of bands, corresponding bands have the same number of
		''' bits per sample, and the <code>TransferType</code>s are the same.
		''' <p>
		''' If <code>obj</code> is not <code>null</code>, it should be a
		''' primitive array of type <code>TransferType</code>.
		''' Otherwise, a <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		''' coordinates are not in bounds, or if <code>obj</code> is not
		''' <code>null</code> and is not large enough to hold
		''' the pixel data.
		''' </summary>
		''' <param name="x">         the X coordinate of the pixel location </param>
		''' <param name="y">         the Y coordinate of the pixel location </param>
		''' <param name="obj">       if non-<code>null</code>, a primitive array
		'''                  in which to return the pixel data </param>
		''' <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		''' <returns> the data of the specified pixel </returns>
		''' <seealso cref= #setDataElements(int, int, Object, DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if obj is too small to hold the output. </exception>
		public Object getDataElements(Integer x, Integer y, Object obj, DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType
			Dim numDataElems As Integer = numDataElements
			Dim pixelOffset As Integer = y*scanlineStride + x*pixelStride

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim bdata As SByte()

				If obj Is Nothing Then
					bdata = New SByte(numDataElems - 1){}
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
					sdata = New Short(numDataElems - 1){}
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
					idata = New Integer(numDataElems - 1){}
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
					fdata = New Single(numDataElems - 1){}
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
					ddata = New Double(numDataElems - 1){}
				Else
					ddata = CType(obj, Double())
				End If

				For i As Integer = 0 To numDataElems - 1
					ddata(i) = data.getElemDouble(bankIndices(i), pixelOffset + bandOffsets(i))
				Next i

				obj = CObj(ddata)
			End Select

			Return obj

		''' <summary>
		''' Returns all samples for the specified pixel in an int array,
		''' one sample per array element.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         the X coordinate of the pixel location </param>
		''' <param name="y">         the Y coordinate of the pixel location </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the samples of the specified pixel. </returns>
		''' <seealso cref= #setPixel(int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if iArray is too small to hold the output. </exception>
		public Integer() getPixel(Integer x, Integer y, Integer iArray(), DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()
			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer (numBands - 1){}
			End If
			Dim pixelOffset As Integer = y*scanlineStride + x*pixelStride
			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = data.getElem(bankIndices(i), pixelOffset + bandOffsets(i))
			Next i
			Return pixels_Renamed

		''' <summary>
		''' Returns all samples for the specified rectangle of pixels in
		''' an int array, one sample per array element.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the samples of the pixels within the specified region. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public Integer() getPixels(Integer x, Integer y, Integer w, Integer h, Integer iArray(), DataBuffer data)
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse y > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim pixels_Renamed As Integer()
			If iArray IsNot Nothing Then
			   pixels_Renamed = iArray
			Else
			   pixels_Renamed = New Integer (w*h*numBands - 1){}
			End If
			Dim lineOffset As Integer = y*scanlineStride + x*pixelStride
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   Dim pixelOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				  For k As Integer = 0 To numBands - 1
					 pixels_Renamed(srcOffset) = data.getElem(bankIndices(k), pixelOffset + bandOffsets(k))
					 srcOffset += 1
				  Next k
				  pixelOffset += pixelStride
			   Next j
			   lineOffset += scanlineStride
			Next i
			Return pixels_Renamed

		''' <summary>
		''' Returns as int the sample in a specified band for the pixel
		''' located at (x,y).
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         the X coordinate of the pixel location </param>
		''' <param name="y">         the Y coordinate of the pixel location </param>
		''' <param name="b">         the band to return </param>
		''' <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		''' <returns> the sample in a specified band for the specified pixel </returns>
		''' <seealso cref= #setSample(int, int, int, int, DataBuffer) </seealso>
		public Integer getSample(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim sample_Renamed As Integer = data.getElem(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the sample in a specified band
		''' for the pixel located at (x,y) as a float.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be
		''' thrown if the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to return </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> a float value representing the sample in the specified
		''' band for the specified pixel. </returns>
		public Single getSampleFloat(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim sample_Renamed As Single = data.getElemFloat(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the sample in a specified band
		''' for a pixel located at (x,y) as a java.lang.[Double].
		''' An <code>ArrayIndexOutOfBoundsException</code> might be
		''' thrown if the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to return </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> a double value representing the sample in the specified
		''' band for the specified pixel. </returns>
		public Double getSampleDouble(Integer x, Integer y, Integer b, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim sample_Renamed As Double = data.getElemDouble(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b))
			Return sample_Renamed

		''' <summary>
		''' Returns the samples in a specified band for the specified rectangle
		''' of pixels in an int array, one sample per data array element.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         the width of the pixel rectangle </param>
		''' <param name="h">         the height of the pixel rectangle </param>
		''' <param name="b">         the band to return </param>
		''' <param name="iArray">    if non-<code>null</code>, returns the samples
		'''                  in this array </param>
		''' <param name="data">      the <code>DataBuffer</code> containing the image data </param>
		''' <returns> the samples in the specified band of the specified pixel </returns>
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
			Dim lineOffset As Integer = y*scanlineStride + x*pixelStride + bandOffsets(b)
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   Dim sampleOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				  samples_Renamed(srcOffset) = data.getElem(bankIndices(b), sampleOffset)
				  srcOffset += 1
				  sampleOffset += pixelStride
			   Next j
			   lineOffset += scanlineStride
			Next i
			Return samples_Renamed

		''' <summary>
		''' Sets the data for a single pixel in the specified
		''' <code>DataBuffer</code> from a primitive array of type
		''' <code>TransferType</code>.  For a <code>ComponentSampleModel</code>,
		''' this is the same as the data type, and samples are transferred
		''' one per array element.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' <code>DataBuffer</code> <code>db1</code>, whose storage layout is
		''' described by <code>ComponentSampleModel</code> <code>csm1</code>,
		''' to <code>DataBuffer</code> <code>db2</code>, whose storage layout
		''' is described by <code>ComponentSampleModel</code> <code>csm2</code>.
		''' The transfer is usually more efficient than using
		''' <code>getPixel</code> and <code>setPixel</code>.
		''' <pre>
		'''       ComponentSampleModel csm1, csm2;
		'''       DataBufferInt db1, db2;
		'''       csm2.setDataElements(x, y, csm1.getDataElements(x, y, null, db1),
		'''                            db2);
		''' </pre>
		''' Using <code>getDataElements</code> and <code>setDataElements</code>
		''' to transfer between two <code>DataBuffer/SampleModel</code> pairs
		''' is legitimate if the <code>SampleModel</code> objects have
		''' the same number of bands, corresponding bands have the same number of
		''' bits per sample, and the <code>TransferType</code>s are the same.
		''' <p>
		''' A <code>ClassCastException</code> is thrown if <code>obj</code> is not
		''' a primitive array of type <code>TransferType</code>.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds, or if <code>obj</code> is not large
		''' enough to hold the pixel data. </summary>
		''' <param name="x">         the X coordinate of the pixel location </param>
		''' <param name="y">         the Y coordinate of the pixel location </param>
		''' <param name="obj">       a primitive array containing pixel data </param>
		''' <param name="data">      the DataBuffer containing the image data </param>
		''' <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		public  Sub  dataElementsnts(Integer x, Integer y, Object obj, DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim type As Integer = transferType
			Dim numDataElems As Integer = numDataElements
			Dim pixelOffset As Integer = y*scanlineStride + x*pixelStride

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim barray As SByte() = CType(obj, SByte())

				For i As Integer = 0 To numDataElems - 1
					data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), (CInt(barray(i))) And &Hff)
				Next i

			Case DataBuffer.TYPE_USHORT, DataBuffer.TYPE_SHORT

				Dim sarray As Short() = CType(obj, Short())

				For i As Integer = 0 To numDataElems - 1
					data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i), (CInt(sarray(i))) And &Hffff)
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
		''' Sets a pixel in the <code>DataBuffer</code> using an int array of
		''' samples for input.  An <code>ArrayIndexOutOfBoundsException</code>
		''' might be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="iArray">    The input samples in an int array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getPixel(int, int, int[], DataBuffer) </seealso>
		public  Sub  pixelxel(Integer x, Integer y, Integer iArray() , DataBuffer data)
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
		   Dim pixelOffset As Integer = y*scanlineStride + x*pixelStride
		   For i As Integer = 0 To numBands - 1
			   data.elemlem(bankIndices(i), pixelOffset + bandOffsets(i),iArray(i))
		   Next i

		''' <summary>
		''' Sets all samples for a rectangle of pixels from an int array containing
		''' one sample per array element.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="iArray">    The input samples in an int array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer) </seealso>
		public  Sub  pixelsels(Integer x, Integer y, Integer w, Integer h, Integer iArray() , DataBuffer data)
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim lineOffset As Integer = y*scanlineStride + x*pixelStride
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   Dim pixelOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				  For k As Integer = 0 To numBands - 1
					 data.elemlem(bankIndices(k), pixelOffset + bandOffsets(k), iArray(srcOffset))
					 srcOffset += 1
				  Next k
				  pixelOffset += pixelStride
			   Next j
			   lineOffset += scanlineStride
			Next i

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the <code>DataBuffer</code> using an int for input.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         the band to set </param>
		''' <param name="s">         the input sample as an int </param>
		''' <param name="data">      the DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public  Sub  sampleple(Integer x, Integer y, Integer b, Integer s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemlem(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b), s)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the <code>DataBuffer</code> using a float for input.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to set </param>
		''' <param name="s">         The input sample as a float </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public  Sub  sampleple(Integer x, Integer y, Integer b, Single s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemFloatoat(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b), s)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the <code>DataBuffer</code> using a double for input.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if
		''' the coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="b">         The band to set </param>
		''' <param name="s">         The input sample as a double </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer) </seealso>
		public  Sub  sampleple(Integer x, Integer y, Integer b, Double s, DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x >= width) OrElse (y >= height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			data.elemDoubleble(bankIndices(b), y*scanlineStride + x*pixelStride + bandOffsets(b), s)

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from an int array containing one sample per data array element.
		''' An <code>ArrayIndexOutOfBoundsException</code> might be thrown if the
		''' coordinates are not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location </param>
		''' <param name="w">         The width of the pixel rectangle </param>
		''' <param name="h">         The height of the pixel rectangle </param>
		''' <param name="b">         The band to set </param>
		''' <param name="iArray">    The input samples in an int array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer) </seealso>
		public  Sub  samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray() , DataBuffer data)
			' Bounds check for 'b' will be performed automatically
			If (x < 0) OrElse (y < 0) OrElse (x + w > width) OrElse (y + h > height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")
			Dim lineOffset As Integer = y*scanlineStride + x*pixelStride + bandOffsets(b)
			Dim srcOffset As Integer = 0

			For i As Integer = 0 To h - 1
			   Dim sampleOffset As Integer = lineOffset
			   For j As Integer = 0 To w - 1
				  data.elemlem(bankIndices(b), sampleOffset, iArray(srcOffset))
				  srcOffset += 1
				  sampleOffset += pixelStride
			   Next j
			   lineOffset += scanlineStride
			Next i

		public Boolean Equals(Object o)
			If (o Is Nothing) OrElse Not(TypeOf o Is ComponentSampleModel) Then Return False

			Dim that As ComponentSampleModel = CType(o, ComponentSampleModel)
			Return Me.width = that.width AndAlso Me.height = that.height AndAlso Me.numBands = that.numBands AndAlso Me.dataType = that.dataType AndAlso java.util.Arrays.Equals(Me.bandOffsets, that.bandOffsets) AndAlso java.util.Arrays.Equals(Me.bankIndices, that.bankIndices) AndAlso Me.numBands = that.numBands AndAlso Me.numBanks = that.numBanks AndAlso Me.scanlineStride = that.scanlineStride AndAlso Me.pixelStride = that.pixelStride

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
			For i As Integer = 0 To bandOffsets.Length - 1
				hash = hash Xor bandOffsets(i)
				hash <<= 8
			Next i
			For i As Integer = 0 To bankIndices.Length - 1
				hash = hash Xor bankIndices(i)
				hash <<= 8
			Next i
			hash = hash Xor numBands
			hash <<= 8
			hash = hash Xor numBanks
			hash <<= 8
			hash = hash Xor scanlineStride
			hash <<= 8
			hash = hash Xor pixelStride
			Return hash
	End Class

End Namespace