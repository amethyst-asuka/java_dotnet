Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	'''  This class represents image data which is stored in a pixel interleaved
	'''  fashion and for
	'''  which each sample of a pixel occupies one data element of the DataBuffer.
	'''  It subclasses ComponentSampleModel but provides a more efficient
	'''  implementation for accessing pixel interleaved image data than is provided
	'''  by ComponentSampleModel.  This class
	'''  stores sample data for all bands in a single bank of the
	'''  DataBuffer. Accessor methods are provided so that image data can be
	'''  manipulated directly. Pixel stride is the number of
	'''  data array elements between two samples for the same band on the same
	'''  scanline. Scanline stride is the number of data array elements between
	'''  a given sample and the corresponding sample in the same column of the next
	'''  scanline.  Band offsets denote the number
	'''  of data array elements from the first data array element of the bank
	'''  of the DataBuffer holding each band to the first sample of the band.
	'''  The bands are numbered from 0 to N-1.
	'''  Bank indices denote the correspondence between a bank of the data buffer
	'''  and a band of image data.
	'''  This class supports
	'''  <seealso cref="DataBuffer#TYPE_BYTE TYPE_BYTE"/>,
	'''  <seealso cref="DataBuffer#TYPE_USHORT TYPE_USHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_SHORT TYPE_SHORT"/>,
	'''  <seealso cref="DataBuffer#TYPE_INT TYPE_INT"/>,
	'''  <seealso cref="DataBuffer#TYPE_FLOAT TYPE_FLOAT"/> and
	'''  <seealso cref="DataBuffer#TYPE_DOUBLE TYPE_DOUBLE"/> datatypes.
	''' </summary>

	Public Class PixelInterleavedSampleModel
		Inherits ComponentSampleModel

		''' <summary>
		''' Constructs a PixelInterleavedSampleModel with the specified parameters.
		''' The number of bands will be given by the length of the bandOffsets
		''' array. </summary>
		''' <param name="dataType">  The data type for storing samples. </param>
		''' <param name="w">         The width (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="h">         The height (in pixels) of the region of
		'''                  image data described. </param>
		''' <param name="pixelStride"> The pixel stride of the image data. </param>
		''' <param name="scanlineStride"> The line stride of the image data. </param>
		''' <param name="bandOffsets"> The offsets of all bands. </param>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <exception cref="IllegalArgumentException"> if any offset between bands is
		'''         greater than the scanline stride </exception>
		''' <exception cref="IllegalArgumentException"> if the product of
		'''         <code>pixelStride</code> and <code>w</code> is greater
		'''         than <code>scanlineStride</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>pixelStride</code> is
		'''         less than any offset between bands </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is not
		'''         one of the supported data types </exception>
		Public Sub New(ByVal dataType As Integer, ByVal w As Integer, ByVal h As Integer, ByVal pixelStride As Integer, ByVal scanlineStride As Integer, ByVal bandOffsets As Integer())
			MyBase.New(dataType, w, h, pixelStride, scanlineStride, bandOffsets)
			Dim minBandOff As Integer=Me.bandOffsets(0)
			Dim maxBandOff As Integer=Me.bandOffsets(0)
			For i As Integer = 1 To Me.bandOffsets.Length - 1
				minBandOff = System.Math.Min(minBandOff,Me.bandOffsets(i))
				maxBandOff = System.Math.Max(maxBandOff,Me.bandOffsets(i))
			Next i
			maxBandOff -= minBandOff
			If maxBandOff > scanlineStride Then Throw New IllegalArgumentException("Offsets between bands must be" & " less than the scanline " & " stride")
			If pixelStride*w > scanlineStride Then Throw New IllegalArgumentException("Pixel stride times width " & "must be less than or " & "equal to the scanline " & "stride")
			If pixelStride < maxBandOff Then Throw New IllegalArgumentException("Pixel stride must be greater" & " than or equal to the offsets" & " between bands")
		End Sub

		''' <summary>
		''' Creates a new PixelInterleavedSampleModel with the specified
		''' width and height.  The new PixelInterleavedSampleModel will have the
		''' same number of bands, storage data type, and pixel stride
		''' as this PixelInterleavedSampleModel.  The band offsets may be
		''' compressed such that the minimum of all of the band offsets is zero. </summary>
		''' <param name="w"> the width of the resulting <code>SampleModel</code> </param>
		''' <param name="h"> the height of the resulting <code>SampleModel</code> </param>
		''' <returns> a new <code>SampleModel</code> with the specified width
		'''         and height. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		Public Overrides Function createCompatibleSampleModel(ByVal w As Integer, ByVal h As Integer) As SampleModel
			Dim minBandoff As Integer=bandOffsets(0)
			Dim numBands_Renamed As Integer = bandOffsets.Length
			For i As Integer = 1 To numBands_Renamed - 1
				If bandOffsets(i) < minBandoff Then minBandoff = bandOffsets(i)
			Next i
			Dim bandOff As Integer()
			If minBandoff > 0 Then
				bandOff = New Integer(numBands_Renamed - 1){}
				For i As Integer = 0 To numBands_Renamed - 1
					bandOff(i) = bandOffsets(i) - minBandoff
				Next i
			Else
				bandOff = bandOffsets
			End If
			Return New PixelInterleavedSampleModel(dataType, w, h, pixelStride, pixelStride*w, bandOff)
		End Function

		''' <summary>
		''' Creates a new PixelInterleavedSampleModel with a subset of the
		''' bands of this PixelInterleavedSampleModel.  The new
		''' PixelInterleavedSampleModel can be used with any DataBuffer that the
		''' existing PixelInterleavedSampleModel can be used with.  The new
		''' PixelInterleavedSampleModel/DataBuffer combination will represent
		''' an image with a subset of the bands of the original
		''' PixelInterleavedSampleModel/DataBuffer combination.
		''' </summary>
		Public Overrides Function createSubsetSampleModel(ByVal bands As Integer()) As SampleModel
			Dim newBandOffsets As Integer() = New Integer(bands.Length - 1){}
			For i As Integer = 0 To bands.Length - 1
				newBandOffsets(i) = bandOffsets(bands(i))
			Next i
			Return New PixelInterleavedSampleModel(Me.dataType, width, height, Me.pixelStride, scanlineStride, newBandOffsets)
		End Function

		' Differentiate hash code from other ComponentSampleModel subclasses
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode() Xor &H1
		End Function
	End Class

End Namespace