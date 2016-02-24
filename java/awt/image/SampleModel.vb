Imports Microsoft.VisualBasic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	'''  This abstract class defines an interface for extracting samples of pixels
	'''  in an image.  All image data is expressed as a collection of pixels.
	'''  Each pixel consists of a number of samples. A sample is a datum
	'''  for one band of an image and a band consists of all samples of a
	'''  particular type in an image.  For example, a pixel might contain
	'''  three samples representing its red, green and blue components.
	'''  There are three bands in the image containing this pixel.  One band
	'''  consists of all the red samples from all pixels in the
	'''  image.  The second band consists of all the green samples and
	'''  the remaining band consists of all of the blue samples.  The pixel
	'''  can be stored in various formats.  For example, all samples from
	'''  a particular band can be stored contiguously or all samples from a
	'''  single pixel can be stored contiguously.
	'''  <p>
	'''  Subclasses of SampleModel specify the types of samples they can
	'''  represent (e.g. unsigned 8-bit byte, signed 16-bit short, etc.)
	'''  and may specify how the samples are organized in memory.
	'''  In the Java 2D(tm) API, built-in image processing operators may
	'''  not operate on all possible sample types, but generally will work
	'''  for unsigned integral samples of 16 bits or less.  Some operators
	'''  support a wider variety of sample types.
	'''  <p>
	'''  A collection of pixels is represented as a Raster, which consists of
	'''  a DataBuffer and a SampleModel.  The SampleModel allows access to
	'''  samples in the DataBuffer and may provide low-level information that
	'''  a programmer can use to directly manipulate samples and pixels in the
	'''  DataBuffer.
	'''  <p>
	'''  This class is generally a fall back method for dealing with
	'''  images.  More efficient code will cast the SampleModel to the
	'''  appropriate subclass and extract the information needed to directly
	'''  manipulate pixels in the DataBuffer.
	''' </summary>
	'''  <seealso cref= java.awt.image.DataBuffer </seealso>
	'''  <seealso cref= java.awt.image.Raster </seealso>
	'''  <seealso cref= java.awt.image.ComponentSampleModel </seealso>
	'''  <seealso cref= java.awt.image.PixelInterleavedSampleModel </seealso>
	'''  <seealso cref= java.awt.image.BandedSampleModel </seealso>
	'''  <seealso cref= java.awt.image.MultiPixelPackedSampleModel </seealso>
	'''  <seealso cref= java.awt.image.SinglePixelPackedSampleModel </seealso>

	Public MustInherit Class SampleModel

		''' <summary>
		''' Width in pixels of the region of image data that this SampleModel
		'''  describes.
		''' </summary>
		Protected Friend width As Integer

		''' <summary>
		''' Height in pixels of the region of image data that this SampleModel
		'''  describes.
		''' </summary>
		Protected Friend height As Integer

		''' <summary>
		''' Number of bands of the image data that this SampleModel describes. </summary>
		Protected Friend numBands As Integer

		''' <summary>
		''' Data type of the DataBuffer storing the pixel data. </summary>
		'''  <seealso cref= java.awt.image.DataBuffer </seealso>
		Protected Friend dataType As Integer

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Constructs a SampleModel with the specified parameters. </summary>
		''' <param name="dataType">  The data type of the DataBuffer storing the pixel data. </param>
		''' <param name="w">         The width (in pixels) of the region of image data. </param>
		''' <param name="h">         The height (in pixels) of the region of image data. </param>
		''' <param name="numBands">  The number of bands of the image data. </param>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or <code>h</code>
		'''         is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if the product of <code>w</code>
		'''         and <code>h</code> is greater than
		'''         <code>Integer.MAX_VALUE</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		Public Sub New(ByVal dataType As Integer, ByVal w As Integer, ByVal h As Integer, ByVal numBands As Integer)
			Dim size As Long = CLng(w) * h
			If w <= 0 OrElse h <= 0 Then Throw New IllegalArgumentException("Width (" & w & ") and height (" & h & ") must be > 0")
			If size >= Integer.MaxValue Then Throw New IllegalArgumentException("Dimensions (width=" & w & " height=" & h & ") are too large")

			If dataType < DataBuffer.TYPE_BYTE OrElse (dataType > DataBuffer.TYPE_DOUBLE AndAlso dataType <> DataBuffer.TYPE_UNDEFINED) Then Throw New IllegalArgumentException("Unsupported dataType: " & dataType)

			If numBands <= 0 Then Throw New IllegalArgumentException("Number of bands must be > 0")

			Me.dataType = dataType
			Me.width = w
			Me.height = h
			Me.numBands = numBands
		End Sub

		''' <summary>
		''' Returns the width in pixels. </summary>
		'''  <returns> the width in pixels of the region of image data
		'''          that this <code>SampleModel</code> describes. </returns>
		Public Property width As Integer
			Get
				 Return width
			End Get
		End Property

		''' <summary>
		''' Returns the height in pixels. </summary>
		'''  <returns> the height in pixels of the region of image data
		'''          that this <code>SampleModel</code> describes. </returns>
		Public Property height As Integer
			Get
				 Return height
			End Get
		End Property

		''' <summary>
		''' Returns the total number of bands of image data. </summary>
		'''  <returns> the number of bands of image data that this
		'''          <code>SampleModel</code> describes. </returns>
		Public Property numBands As Integer
			Get
				 Return numBands
			End Get
		End Property

		''' <summary>
		''' Returns the number of data elements needed to transfer a pixel
		'''  via the getDataElements and setDataElements methods.  When pixels
		'''  are transferred via these methods, they may be transferred in a
		'''  packed or unpacked format, depending on the implementation of the
		'''  SampleModel.  Using these methods, pixels are transferred as an
		'''  array of getNumDataElements() elements of a primitive type given
		'''  by getTransferType().  The TransferType may or may not be the same
		'''  as the storage DataType. </summary>
		'''  <returns> the number of data elements. </returns>
		'''  <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #getTransferType </seealso>
		Public MustOverride ReadOnly Property numDataElements As Integer

		''' <summary>
		''' Returns the data type of the DataBuffer storing the pixel data. </summary>
		'''  <returns> the data type. </returns>
		Public Property dataType As Integer
			Get
				Return dataType
			End Get
		End Property

		''' <summary>
		''' Returns the TransferType used to transfer pixels via the
		'''  getDataElements and setDataElements methods.  When pixels
		'''  are transferred via these methods, they may be transferred in a
		'''  packed or unpacked format, depending on the implementation of the
		'''  SampleModel.  Using these methods, pixels are transferred as an
		'''  array of getNumDataElements() elements of a primitive type given
		'''  by getTransferType().  The TransferType may or may not be the same
		'''  as the storage DataType.  The TransferType will be one of the types
		'''  defined in DataBuffer. </summary>
		'''  <returns> the transfer type. </returns>
		'''  <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #setDataElements(int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		'''  <seealso cref= #getNumDataElements </seealso>
		'''  <seealso cref= java.awt.image.DataBuffer </seealso>
		Public Overridable Property transferType As Integer
			Get
				Return dataType
			End Get
		End Property

		''' <summary>
		''' Returns the samples for a specified pixel in an int array,
		''' one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location </param>
		''' <param name="y">         The Y coordinate of the pixel location </param>
		''' <param name="iArray">    If non-null, returns the samples in this array </param>
		''' <param name="data">      The DataBuffer containing the image data </param>
		''' <returns> the samples for the specified pixel. </returns>
		''' <seealso cref= #setPixel(int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if iArray is too small to hold the output. </exception>
		Public Overridable Function getPixel(ByVal x As Integer, ByVal y As Integer, ByVal iArray As Integer(), ByVal data As DataBuffer) As Integer()

			Dim pixels_Renamed As Integer()

			If iArray IsNot Nothing Then
				pixels_Renamed = iArray
			Else
				pixels_Renamed = New Integer(numBands - 1){}
			End If

			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = getSample(x, y, i, data)
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns data for a single pixel in a primitive array of type
		''' TransferType.  For image data supported by the Java 2D API, this
		''' will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		''' or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		''' thus increasing efficiency for data transfers. Generally, obj
		''' should be passed in as null, so that the Object will be created
		''' automatically and will be of the right primitive data type.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		''' storage layout is described by SampleModel <code>sm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixel/setPixel.
		''' <pre>
		'''       SampleModel sm1, sm2;
		'''       DataBuffer db1, db2;
		'''       sm2.setDataElements(x, y, sm1.getDataElements(x, y, null, db1), db2);
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
		''' <returns> the data elements for the specified pixel. </returns>
		''' <seealso cref= #getNumDataElements </seealso>
		''' <seealso cref= #getTransferType </seealso>
		''' <seealso cref= java.awt.image.DataBuffer </seealso>
		''' <seealso cref= #setDataElements(int, int, Object, DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if obj is too small to hold the output. </exception>
		Public MustOverride Function getDataElements(ByVal x As Integer, ByVal y As Integer, ByVal obj As Object, ByVal data As DataBuffer) As Object

		''' <summary>
		''' Returns the pixel data for the specified rectangle of pixels in a
		''' primitive array of type TransferType.
		''' For image data supported by the Java 2D API, this
		''' will be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_FLOAT,
		''' or DataBuffer.TYPE_DOUBLE.  Data may be returned in a packed format,
		''' thus increasing efficiency for data transfers. Generally, obj
		''' should be passed in as null, so that the Object will be created
		''' automatically and will be of the right primitive data type.
		''' <p>
		''' The following code illustrates transferring data for a rectangular
		''' region of pixels from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		''' storage layout is described by SampleModel <code>sm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixels/setPixels.
		''' <pre>
		'''       SampleModel sm1, sm2;
		'''       DataBuffer db1, db2;
		'''       sm2.setDataElements(x, y, w, h, sm1.getDataElements(x, y, w,
		'''                           h, null, db1), db2);
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
		''' <param name="x">         The minimum X coordinate of the pixel rectangle. </param>
		''' <param name="y">         The minimum Y coordinate of the pixel rectangle. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="obj">       If non-null, a primitive array in which to return
		'''                  the pixel data. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the data elements for the specified region of pixels. </returns>
		''' <seealso cref= #getNumDataElements </seealso>
		''' <seealso cref= #getTransferType </seealso>
		''' <seealso cref= #setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		''' <seealso cref= java.awt.image.DataBuffer
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if obj is too small to hold the output. </exception>
		Public Overridable Function getDataElements(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal obj As Object, ByVal data As DataBuffer) As Object

			Dim type As Integer = transferType
			Dim numDataElems As Integer = numDataElements
			Dim cnt As Integer = 0
			Dim o As Object = Nothing

			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim btemp As SByte()
				Dim bdata As SByte()

				If obj Is Nothing Then
					bdata = New SByte(numDataElems*w*h - 1){}
				Else
					bdata = CType(obj, SByte())
				End If

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						o = getDataElements(j, i, o, data)
						btemp = CType(o, SByte())
						For k As Integer = 0 To numDataElems - 1
							bdata(cnt) = btemp(k)
							cnt += 1
						Next k
					Next j
				Next i
				obj = CObj(bdata)

			Case DataBuffer.TYPE_USHORT, DataBuffer.TYPE_SHORT

				Dim sdata As Short()
				Dim stemp As Short()

				If obj Is Nothing Then
					sdata = New Short(numDataElems*w*h - 1){}
				Else
					sdata = CType(obj, Short())
				End If

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						o = getDataElements(j, i, o, data)
						stemp = CType(o, Short())
						For k As Integer = 0 To numDataElems - 1
							sdata(cnt) = stemp(k)
							cnt += 1
						Next k
					Next j
				Next i

				obj = CObj(sdata)

			Case DataBuffer.TYPE_INT

				Dim idata As Integer()
				Dim itemp As Integer()

				If obj Is Nothing Then
					idata = New Integer(numDataElems*w*h - 1){}
				Else
					idata = CType(obj, Integer())
				End If

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						o = getDataElements(j, i, o, data)
						itemp = CType(o, Integer())
						For k As Integer = 0 To numDataElems - 1
							idata(cnt) = itemp(k)
							cnt += 1
						Next k
					Next j
				Next i

				obj = CObj(idata)

			Case DataBuffer.TYPE_FLOAT

				Dim fdata As Single()
				Dim ftemp As Single()

				If obj Is Nothing Then
					fdata = New Single(numDataElems*w*h - 1){}
				Else
					fdata = CType(obj, Single())
				End If

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						o = getDataElements(j, i, o, data)
						ftemp = CType(o, Single())
						For k As Integer = 0 To numDataElems - 1
							fdata(cnt) = ftemp(k)
							cnt += 1
						Next k
					Next j
				Next i

				obj = CObj(fdata)

			Case DataBuffer.TYPE_DOUBLE

				Dim ddata As Double()
				Dim dtemp As Double()

				If obj Is Nothing Then
					ddata = New Double(numDataElems*w*h - 1){}
				Else
					ddata = CType(obj, Double())
				End If

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						o = getDataElements(j, i, o, data)
						dtemp = CType(o, Double())
						For k As Integer = 0 To numDataElems - 1
							ddata(cnt) = dtemp(k)
							cnt += 1
						Next k
					Next j
				Next i

				obj = CObj(ddata)
			End Select

			Return obj
		End Function

		''' <summary>
		''' Sets the data for a single pixel in the specified DataBuffer from a
		''' primitive array of type TransferType.  For image data supported by
		''' the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		''' DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		''' DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		''' may be in a packed format, thus increasing efficiency for data
		''' transfers.
		''' <p>
		''' The following code illustrates transferring data for one pixel from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		''' storage layout is described by SampleModel <code>sm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixel/setPixel.
		''' <pre>
		'''       SampleModel sm1, sm2;
		'''       DataBuffer db1, db2;
		'''       sm2.setDataElements(x, y, sm1.getDataElements(x, y, null, db1),
		'''                           db2);
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
		''' <seealso cref= #getNumDataElements </seealso>
		''' <seealso cref= #getTransferType </seealso>
		''' <seealso cref= #getDataElements(int, int, Object, DataBuffer) </seealso>
		''' <seealso cref= java.awt.image.DataBuffer
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if obj is too small to hold the input. </exception>
		Public MustOverride Sub setDataElements(ByVal x As Integer, ByVal y As Integer, ByVal obj As Object, ByVal data As DataBuffer)

		''' <summary>
		''' Sets the data for a rectangle of pixels in the specified DataBuffer
		''' from a primitive array of type TransferType.  For image data supported
		''' by the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		''' DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		''' DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		''' may be in a packed format, thus increasing efficiency for data
		''' transfers.
		''' <p>
		''' The following code illustrates transferring data for a rectangular
		''' region of pixels from
		''' DataBuffer <code>db1</code>, whose storage layout is described by
		''' SampleModel <code>sm1</code>, to DataBuffer <code>db2</code>, whose
		''' storage layout is described by SampleModel <code>sm2</code>.
		''' The transfer will generally be more efficient than using
		''' getPixels/setPixels.
		''' <pre>
		'''       SampleModel sm1, sm2;
		'''       DataBuffer db1, db2;
		'''       sm2.setDataElements(x, y, w, h, sm1.getDataElements(x, y, w, h,
		'''                           null, db1), db2);
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
		''' <param name="x">         The minimum X coordinate of the pixel rectangle. </param>
		''' <param name="y">         The minimum Y coordinate of the pixel rectangle. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="obj">       A primitive array containing pixel data. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getNumDataElements </seealso>
		''' <seealso cref= #getTransferType </seealso>
		''' <seealso cref= #getDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		''' <seealso cref= java.awt.image.DataBuffer
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if obj is too small to hold the input. </exception>
		Public Overridable Sub setDataElements(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal obj As Object, ByVal data As DataBuffer)

			Dim cnt As Integer = 0
			Dim o As Object = Nothing
			Dim type As Integer = transferType
			Dim numDataElems As Integer = numDataElements

			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			Select Case type

			Case DataBuffer.TYPE_BYTE

				Dim barray As SByte() = CType(obj, SByte())
				Dim btemp As SByte() = New SByte(numDataElems - 1){}

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						For k As Integer = 0 To numDataElems - 1
							btemp(k) = barray(cnt)
							cnt += 1
						Next k

						dataElementsnts(j, i, btemp, data)
					Next j
				Next i

			Case DataBuffer.TYPE_USHORT, DataBuffer.TYPE_SHORT

				Dim sarray As Short() = CType(obj, Short())
				Dim stemp As Short() = New Short(numDataElems - 1){}

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						For k As Integer = 0 To numDataElems - 1
							stemp(k) = sarray(cnt)
							cnt += 1
						Next k

						dataElementsnts(j, i, stemp, data)
					Next j
				Next i

			Case DataBuffer.TYPE_INT

				Dim iArray As Integer() = CType(obj, Integer())
				Dim itemp As Integer() = New Integer(numDataElems - 1){}

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						For k As Integer = 0 To numDataElems - 1
							itemp(k) = iArray(cnt)
							cnt += 1
						Next k

						dataElementsnts(j, i, itemp, data)
					Next j
				Next i

			Case DataBuffer.TYPE_FLOAT

				Dim fArray As Single() = CType(obj, Single())
				Dim ftemp As Single() = New Single(numDataElems - 1){}

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						For k As Integer = 0 To numDataElems - 1
							ftemp(k) = fArray(cnt)
							cnt += 1
						Next k

						dataElementsnts(j, i, ftemp, data)
					Next j
				Next i

			Case DataBuffer.TYPE_DOUBLE

				Dim dArray As Double() = CType(obj, Double())
				Dim dtemp As Double() = New Double(numDataElems - 1){}

				For i As Integer = y To y1 - 1
					For j As Integer = x To x1 - 1
						For k As Integer = 0 To numDataElems - 1
							dtemp(k) = dArray(cnt)
							cnt += 1
						Next k

						dataElementsnts(j, i, dtemp, data)
					Next j
				Next i
			End Select

		End Sub

		''' <summary>
		''' Returns the samples for the specified pixel in an array of float.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="fArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified pixel. </returns>
		''' <seealso cref= #setPixel(int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if fArray is too small to hold the output. </exception>
		Public Overridable Function getPixel(ByVal x As Integer, ByVal y As Integer, ByVal fArray As Single(), ByVal data As DataBuffer) As Single()

			Dim pixels_Renamed As Single()

			If fArray IsNot Nothing Then
				pixels_Renamed = fArray
			Else
				pixels_Renamed = New Single(numBands - 1){}
			End If

			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = getSampleFloat(x, y, i, data)
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns the samples for the specified pixel in an array of double.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="dArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified pixel. </returns>
		''' <seealso cref= #setPixel(int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if dArray is too small to hold the output. </exception>
		Public Overridable Function getPixel(ByVal x As Integer, ByVal y As Integer, ByVal dArray As Double(), ByVal data As DataBuffer) As Double()

			Dim pixels_Renamed As Double()

			If dArray IsNot Nothing Then
				pixels_Renamed = dArray
			Else
				pixels_Renamed = New Double(numBands - 1){}
			End If

			For i As Integer = 0 To numBands - 1
				pixels_Renamed(i) = getSampleDouble(x, y, i, data)
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns all samples for a rectangle of pixels in an
		''' int array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="iArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified region of pixels. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if iArray is too small to hold the output. </exception>
		Public Overridable Function getPixels(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal iArray As Integer(), ByVal data As DataBuffer) As Integer()

			Dim pixels_Renamed As Integer()
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			If iArray IsNot Nothing Then
				pixels_Renamed = iArray
			Else
				pixels_Renamed = New Integer(numBands * w * h - 1){}
			End If

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						pixels_Renamed(Offset) = getSample(j, i, k, data)
						Offset += 1
					Next k
				Next j
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns all samples for a rectangle of pixels in a float
		''' array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="fArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified region of pixels. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if fArray is too small to hold the output. </exception>
		Public Overridable Function getPixels(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal fArray As Single(), ByVal data As DataBuffer) As Single()

			Dim pixels_Renamed As Single()
			Dim Offset As Integer = 0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			If fArray IsNot Nothing Then
				pixels_Renamed = fArray
			Else
				pixels_Renamed = New Single(numBands * w * h - 1){}
			End If

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						pixels_Renamed(Offset) = getSampleFloat(j, i, k, data)
						Offset += 1
					Next k
				Next j
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns all samples for a rectangle of pixels in a double
		''' array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="dArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified region of pixels. </returns>
		''' <seealso cref= #setPixels(int, int, int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if dArray is too small to hold the output. </exception>
		Public Overridable Function getPixels(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal dArray As Double(), ByVal data As DataBuffer) As Double()
			Dim pixels_Renamed As Double()
			Dim Offset As Integer = 0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			If dArray IsNot Nothing Then
				pixels_Renamed = dArray
			Else
				pixels_Renamed = New Double(numBands * w * h - 1){}
			End If

			' Fix 4217412
			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						pixels_Renamed(Offset) = getSampleDouble(j, i, k, data)
						Offset += 1
					Next k
				Next j
			Next i

			Return pixels_Renamed
		End Function


		''' <summary>
		''' Returns the sample in a specified band for the pixel located
		''' at (x,y) as an int.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the sample in a specified band for the specified pixel. </returns>
		''' <seealso cref= #setSample(int, int, int, int, DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public MustOverride Function getSample(ByVal x As Integer, ByVal y As Integer, ByVal b As Integer, ByVal data As DataBuffer) As Integer


		''' <summary>
		''' Returns the sample in a specified band
		''' for the pixel located at (x,y) as a float.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the sample in a specified band for the specified pixel.
		''' </returns>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public Overridable Function getSampleFloat(ByVal x As Integer, ByVal y As Integer, ByVal b As Integer, ByVal data As DataBuffer) As Single

			Dim sample_Renamed As Single
			sample_Renamed = CSng(getSample(x, y, b, data))
			Return sample_Renamed
		End Function

		''' <summary>
		''' Returns the sample in a specified band
		''' for a pixel located at (x,y) as a double.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the sample in a specified band for the specified pixel.
		''' </returns>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public Overridable Function getSampleDouble(ByVal x As Integer, ByVal y As Integer, ByVal b As Integer, ByVal data As DataBuffer) As Double

			Dim sample_Renamed As Double

			sample_Renamed = CDbl(getSample(x, y, b, data))
			Return sample_Renamed
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
		''' <returns> the samples for the specified band for the specified region
		'''         of pixels. </returns>
		''' <seealso cref= #setSamples(int, int, int, int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if iArray is too small to
		''' hold the output. </exception>
		Public Overridable Function getSamples(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal b As Integer, ByVal iArray As Integer(), ByVal data As DataBuffer) As Integer()
			Dim pixels_Renamed As Integer()
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x1 < x OrElse x1 > width OrElse y < 0 OrElse y1 < y OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			If iArray IsNot Nothing Then
				pixels_Renamed = iArray
			Else
				pixels_Renamed = New Integer(w * h - 1){}
			End If

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					pixels_Renamed(Offset) = getSample(j, i, b, data)
					Offset += 1
				Next j
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns the samples for a specified band for the specified rectangle
		''' of pixels in a float array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="fArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified band for the specified region
		'''         of pixels. </returns>
		''' <seealso cref= #setSamples(int, int, int, int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if fArray is too small to
		''' hold the output. </exception>
		Public Overridable Function getSamples(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal b As Integer, ByVal fArray As Single(), ByVal data As DataBuffer) As Single()
			Dim pixels_Renamed As Single()
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x1 < x OrElse x1 > width OrElse y < 0 OrElse y1 < y OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates")

			If fArray IsNot Nothing Then
				pixels_Renamed = fArray
			Else
				pixels_Renamed = New Single(w * h - 1){}
			End If

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					pixels_Renamed(Offset) = getSampleFloat(j, i, b, data)
					Offset += 1
				Next j
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Returns the samples for a specified band for a specified rectangle
		''' of pixels in a double array, one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to return. </param>
		''' <param name="dArray">    If non-null, returns the samples in this array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <returns> the samples for the specified band for the specified region
		'''         of pixels. </returns>
		''' <seealso cref= #setSamples(int, int, int, int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if dArray is too small to
		''' hold the output. </exception>
		Public Overridable Function getSamples(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal b As Integer, ByVal dArray As Double(), ByVal data As DataBuffer) As Double()
			Dim pixels_Renamed As Double()
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x1 < x OrElse x1 > width OrElse y < 0 OrElse y1 < y OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates")

			If dArray IsNot Nothing Then
				pixels_Renamed = dArray
			Else
				pixels_Renamed = New Double(w * h - 1){}
			End If

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					pixels_Renamed(Offset) = getSampleDouble(j, i, b, data)
					Offset += 1
				Next j
			Next i

			Return pixels_Renamed
		End Function

		''' <summary>
		''' Sets a pixel in  the DataBuffer using an int array of samples for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="iArray">    The input samples in an int array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixel(int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if iArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if iArray is too small to hold the input. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void setPixel(int x, int y, int iArray() , DataBuffer data)

			For i As Integer = 0 To numBands - 1
				sampleple(x, y, i, iArray(i), data)
			Next i

		''' <summary>
		''' Sets a pixel in the DataBuffer using a float array of samples for input.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="fArray">    The input samples in a float array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixel(int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if fArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if fArray is too small to hold the input. </exception>
		public void pixelxel(Integer x, Integer y, Single fArray() , DataBuffer data)

			For i As Integer = 0 To numBands - 1
				sampleple(x, y, i, fArray(i), data)
			Next i

		''' <summary>
		''' Sets a pixel in the DataBuffer using a double array of samples
		''' for input. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="dArray">    The input samples in a double array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixel(int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if dArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if fArray is too small to hold the input. </exception>
		public void pixelxel(Integer x, Integer y, Double dArray() , DataBuffer data)

			For i As Integer = 0 To numBands - 1
				sampleple(x, y, i, dArray(i), data)
			Next i

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
		''' <seealso cref= #getPixels(int, int, int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if iArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if iArray is too small to hold the input. </exception>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, Integer iArray() , DataBuffer data)
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						sampleple(j, i, k, iArray(Offset), data)
						Offset += 1
					Next k
				Next j
			Next i

		''' <summary>
		''' Sets all samples for a rectangle of pixels from a float array containing
		''' one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="fArray">    The input samples in a float array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixels(int, int, int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if fArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if fArray is too small to hold the input. </exception>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, Single fArray() , DataBuffer data)
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						sampleple(j, i, k, fArray(Offset), data)
						Offset += 1
					Next k
				Next j
			Next i

		''' <summary>
		''' Sets all samples for a rectangle of pixels from a double array
		''' containing one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="dArray">    The input samples in a double array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getPixels(int, int, int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if dArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are
		''' not in bounds, or if dArray is too small to hold the input. </exception>
		public void pixelsels(Integer x, Integer y, Integer w, Integer h, Double dArray() , DataBuffer data)
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					For k As Integer = 0 To numBands - 1
						sampleple(j, i, k, dArray(Offset), data)
						Offset += 1
					Next k
				Next j
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
		''' <seealso cref= #getSample(int, int, int,  DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public abstract void sampleple(Integer x, Integer y, Integer b, Integer s, DataBuffer data)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a float for input.
		''' The default implementation of this method casts the input
		''' float sample to an int and then calls the
		''' <code>setSample(int, int, int, DataBuffer)</code> method using
		''' that int value.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="s">         The input sample as a float. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public void sampleple(Integer x, Integer y, Integer b, Single s, DataBuffer data)
			Dim sample_Renamed As Integer = CInt(Fix(s))

			sampleple(x, y, b, sample_Renamed, data)

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a double for input.
		''' The default implementation of this method casts the input
		''' double sample to an int and then calls the
		''' <code>setSample(int, int, int, DataBuffer)</code> method using
		''' that int value.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the pixel location. </param>
		''' <param name="y">         The Y coordinate of the pixel location. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="s">         The input sample as a double. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSample(int, int, int, DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		public void sampleple(Integer x, Integer y, Integer b, Double s, DataBuffer data)
			Dim sample_Renamed As Integer = CInt(Fix(s))

			sampleple(x, y, b, sample_Renamed, data)

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
		''' <seealso cref= #getSamples(int, int, int, int, int, int[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if iArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if iArray is too small to
		''' hold the input. </exception>
		public void samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Integer iArray() , DataBuffer data)

			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h
			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					sampleple(j, i, b, iArray(Offset), data)
					Offset += 1
				Next j
			Next i

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from a float array containing one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="fArray">    The input samples in a float array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSamples(int, int, int, int, int, float[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if fArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if fArray is too small to
		''' hold the input. </exception>
		public void samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Single fArray() , DataBuffer data)
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h

			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					sampleple(j, i, b, fArray(Offset), data)
					Offset += 1
				Next j
			Next i

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from a double array containing one sample per array element.
		''' ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds. </summary>
		''' <param name="x">         The X coordinate of the upper left pixel location. </param>
		''' <param name="y">         The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">         The width of the pixel rectangle. </param>
		''' <param name="h">         The height of the pixel rectangle. </param>
		''' <param name="b">         The band to set. </param>
		''' <param name="dArray">    The input samples in a double array. </param>
		''' <param name="data">      The DataBuffer containing the image data. </param>
		''' <seealso cref= #getSamples(int, int, int, int, int, double[], DataBuffer)
		''' </seealso>
		''' <exception cref="NullPointerException"> if dArray or data is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if dArray is too small to
		''' hold the input. </exception>
		public void samplesles(Integer x, Integer y, Integer w, Integer h, Integer b, Double dArray() , DataBuffer data)
			Dim Offset As Integer=0
			Dim x1 As Integer = x + w
			Dim y1 As Integer = y + h


			If x < 0 OrElse x >= width OrElse w > width OrElse x1 < 0 OrElse x1 > width OrElse y < 0 OrElse y >= height OrElse h > height OrElse y1 < 0 OrElse y1 > height Then Throw New ArrayIndexOutOfBoundsException("Invalid coordinates.")

			For i As Integer = y To y1 - 1
				For j As Integer = x To x1 - 1
					sampleple(j, i, b, dArray(Offset), data)
					Offset += 1
				Next j
			Next i

		''' <summary>
		'''  Creates a SampleModel which describes data in this SampleModel's
		'''  format, but with a different width and height. </summary>
		'''  <param name="w"> the width of the image data </param>
		'''  <param name="h"> the height of the image data </param>
		'''  <returns> a <code>SampleModel</code> describing the same image
		'''          data as this <code>SampleModel</code>, but with a
		'''          different size. </returns>
		public abstract SampleModel createCompatibleSampleModel(Integer w, Integer h)

		''' <summary>
		''' Creates a new SampleModel
		''' with a subset of the bands of this
		''' SampleModel. </summary>
		''' <param name="bands"> the subset of bands of this <code>SampleModel</code> </param>
		''' <returns> a <code>SampleModel</code> with a subset of bands of this
		'''         <code>SampleModel</code>. </returns>
		public abstract SampleModel createSubsetSampleModel(Integer bands())

		''' <summary>
		''' Creates a DataBuffer that corresponds to this SampleModel.
		''' The DataBuffer's width and height will match this SampleModel's. </summary>
		''' <returns> a <code>DataBuffer</code> corresponding to this
		'''         <code>SampleModel</code>. </returns>
		public abstract DataBuffer createDataBuffer()

		''' <summary>
		''' Returns the size in bits of samples for all bands. </summary>
		'''  <returns> the size of samples for all bands. </returns>
		public abstract Integer() sampleSize

		''' <summary>
		''' Returns the size in bits of samples for the specified band. </summary>
		'''  <param name="band"> the specified band </param>
		'''  <returns> the size of the samples of the specified band. </returns>
		public abstract Integer getSampleSize(Integer band)

	End Class

End Namespace