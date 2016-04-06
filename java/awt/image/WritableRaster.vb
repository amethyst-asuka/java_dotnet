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
	''' This class extends Raster to provide pixel writing capabilities.
	''' Refer to the class comment for Raster for descriptions of how
	''' a Raster stores pixels.
	''' 
	''' <p> The constructors of this class are protected.  To instantiate
	''' a WritableRaster, use one of the createWritableRaster factory methods
	''' in the Raster class.
	''' </summary>
	Public Class WritableRaster
		Inherits Raster

		''' <summary>
		'''  Constructs a WritableRaster with the given SampleModel.  The
		'''  WritableRaster's upper left corner is origin and it is the
		'''  same size as the  SampleModel.  A DataBuffer large enough to
		'''  describe the WritableRaster is automatically created. </summary>
		'''  <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		'''  <param name="origin">          The Point that specifies the origin. </param>
		'''  <exception cref="RasterFormatException"> if computing either
		'''          <code>origin.x + sampleModel.getWidth()</code> or
		'''          <code>origin.y + sampleModel.getHeight()</code> results
		'''          in integer overflow </exception>
		Protected Friend Sub New(  sampleModel_Renamed As SampleModel,   origin As java.awt.Point)
			Me.New(sampleModel_Renamed, sampleModel_Renamed.createDataBuffer(), New java.awt.Rectangle(origin.x, origin.y, sampleModel_Renamed.width, sampleModel_Renamed.height), origin, Nothing)
		End Sub

		''' <summary>
		'''  Constructs a WritableRaster with the given SampleModel and DataBuffer.
		'''  The WritableRaster's upper left corner is origin and it is the same
		'''  size as the SampleModel.  The DataBuffer is not initialized and must
		'''  be compatible with SampleModel. </summary>
		'''  <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		'''  <param name="dataBuffer">      The DataBuffer that contains the image data. </param>
		'''  <param name="origin">          The Point that specifies the origin. </param>
		'''  <exception cref="RasterFormatException"> if computing either
		'''          <code>origin.x + sampleModel.getWidth()</code> or
		'''          <code>origin.y + sampleModel.getHeight()</code> results
		'''          in integer overflow </exception>
		Protected Friend Sub New(  sampleModel_Renamed As SampleModel,   dataBuffer_Renamed As DataBuffer,   origin As java.awt.Point)
			Me.New(sampleModel_Renamed, dataBuffer_Renamed, New java.awt.Rectangle(origin.x, origin.y, sampleModel_Renamed.width, sampleModel_Renamed.height), origin, Nothing)
		End Sub

		''' <summary>
		''' Constructs a WritableRaster with the given SampleModel, DataBuffer,
		''' and parent.  aRegion specifies the bounding rectangle of the new
		''' Raster.  When translated into the base Raster's coordinate
		''' system, aRegion must be contained by the base Raster.
		''' (The base Raster is the Raster's ancestor which has no parent.)
		''' sampleModelTranslate specifies the sampleModelTranslateX and
		''' sampleModelTranslateY values of the new Raster.
		''' 
		''' Note that this constructor should generally be called by other
		''' constructors or create methods, it should not be used directly. </summary>
		''' <param name="sampleModel">     The SampleModel that specifies the layout. </param>
		''' <param name="dataBuffer">      The DataBuffer that contains the image data. </param>
		''' <param name="aRegion">         The Rectangle that specifies the image area. </param>
		''' <param name="sampleModelTranslate">  The Point that specifies the translation
		'''                        from SampleModel to Raster coordinates. </param>
		''' <param name="parent">          The parent (if any) of this raster. </param>
		''' <exception cref="RasterFormatException"> if <code>aRegion</code> has width
		'''         or height less than or equal to zero, or computing either
		'''         <code>aRegion.x + aRegion.width</code> or
		'''         <code>aRegion.y + aRegion.height</code> results in integer
		'''         overflow </exception>
		Protected Friend Sub New(  sampleModel_Renamed As SampleModel,   dataBuffer_Renamed As DataBuffer,   aRegion As java.awt.Rectangle,   sampleModelTranslate As java.awt.Point,   parent As WritableRaster)
			MyBase.New(sampleModel_Renamed,dataBuffer_Renamed,aRegion,sampleModelTranslate,parent)
		End Sub

		''' <summary>
		''' Returns the parent WritableRaster (if any) of this WritableRaster,
		'''  or else null. </summary>
		'''  <returns> the parent of this <code>WritableRaster</code>, or
		'''          <code>null</code>. </returns>
		Public Overridable Property writableParent As WritableRaster
			Get
				Return CType(parent, WritableRaster)
			End Get
		End Property

		''' <summary>
		''' Create a WritableRaster with the same size, SampleModel and DataBuffer
		''' as this one, but with a different location.  The new WritableRaster
		''' will possess a reference to the current WritableRaster, accessible
		''' through its getParent() and getWritableParent() methods.
		''' </summary>
		''' <param name="childMinX"> X coord of the upper left corner of the new Raster. </param>
		''' <param name="childMinY"> Y coord of the upper left corner of the new Raster. </param>
		''' <returns> a <code>WritableRaster</code> the same as this one except
		'''         for the specified location. </returns>
		''' <exception cref="RasterFormatException"> if  computing either
		'''         <code>childMinX + this.getWidth()</code> or
		'''         <code>childMinY + this.getHeight()</code> results in integer
		'''         overflow </exception>
		Public Overridable Function createWritableTranslatedChild(  childMinX As Integer,   childMinY As Integer) As WritableRaster
			Return createWritableChild(minX,minY,width,height, childMinX,childMinY,Nothing)
		End Function

		''' <summary>
		''' Returns a new WritableRaster which shares all or part of this
		''' WritableRaster's DataBuffer.  The new WritableRaster will
		''' possess a reference to the current WritableRaster, accessible
		''' through its getParent() and getWritableParent() methods.
		''' 
		''' <p> The parentX, parentY, width and height parameters form a
		''' Rectangle in this WritableRaster's coordinate space, indicating
		''' the area of pixels to be shared.  An error will be thrown if
		''' this Rectangle is not contained with the bounds of the current
		''' WritableRaster.
		''' 
		''' <p> The new WritableRaster may additionally be translated to a
		''' different coordinate system for the plane than that used by the current
		''' WritableRaster.  The childMinX and childMinY parameters give
		''' the new (x, y) coordinate of the upper-left pixel of the
		''' returned WritableRaster; the coordinate (childMinX, childMinY)
		''' in the new WritableRaster will map to the same pixel as the
		''' coordinate (parentX, parentY) in the current WritableRaster.
		''' 
		''' <p> The new WritableRaster may be defined to contain only a
		''' subset of the bands of the current WritableRaster, possibly
		''' reordered, by means of the bandList parameter.  If bandList is
		''' null, it is taken to include all of the bands of the current
		''' WritableRaster in their current order.
		''' 
		''' <p> To create a new WritableRaster that contains a subregion of
		''' the current WritableRaster, but shares its coordinate system
		''' and bands, this method should be called with childMinX equal to
		''' parentX, childMinY equal to parentY, and bandList equal to
		''' null.
		''' </summary>
		''' <param name="parentX">    X coordinate of the upper left corner in this
		'''                   WritableRaster's coordinates. </param>
		''' <param name="parentY">    Y coordinate of the upper left corner in this
		'''                   WritableRaster's coordinates. </param>
		''' <param name="w">          Width of the region starting at (parentX, parentY). </param>
		''' <param name="h">          Height of the region starting at (parentX, parentY). </param>
		''' <param name="childMinX">  X coordinate of the upper left corner of
		'''                   the returned WritableRaster. </param>
		''' <param name="childMinY">  Y coordinate of the upper left corner of
		'''                   the returned WritableRaster. </param>
		''' <param name="bandList">   Array of band indices, or null to use all bands. </param>
		''' <returns> a <code>WritableRaster</code> sharing all or part of the
		'''         <code>DataBuffer</code> of this <code>WritableRaster</code>. </returns>
		''' <exception cref="RasterFormatException"> if the subregion is outside of the
		'''                               raster bounds. </exception>
		''' <exception cref="RasterFormatException"> if <code>w</code> or
		'''         <code>h</code>
		'''         is less than or equal to zero, or computing any of
		'''         <code>parentX + w</code>, <code>parentY + h</code>,
		'''         <code>childMinX + w</code>, or
		'''         <code>childMinY + h</code> results in integer
		'''         overflow </exception>
		Public Overridable Function createWritableChild(  parentX As Integer,   parentY As Integer,   w As Integer,   h As Integer,   childMinX As Integer,   childMinY As Integer,   bandList As Integer()) As WritableRaster
			If parentX < Me.minX Then Throw New RasterFormatException("parentX lies outside raster")
			If parentY < Me.minY Then Throw New RasterFormatException("parentY lies outside raster")
			If (parentX+w < parentX) OrElse (parentX+w > Me.width + Me.minX) Then Throw New RasterFormatException("(parentX + width) is outside raster")
			If (parentY+h < parentY) OrElse (parentY+h > Me.height + Me.minY) Then Throw New RasterFormatException("(parentY + height) is outside raster")

			Dim sm As SampleModel
			' Note: the SampleModel for the child Raster should have the same
			' width and height as that for the parent, since it represents
			' the physical layout of the pixel data.  The child Raster's width
			' and height represent a "virtual" view of the pixel data, so
			' they may be different than those of the SampleModel.
			If bandList IsNot Nothing Then
				sm = sampleModel.createSubsetSampleModel(bandList)
			Else
				sm = sampleModel
			End If

			Dim deltaX As Integer = childMinX - parentX
			Dim deltaY As Integer = childMinY - parentY

			Return New WritableRaster(sm, dataBuffer, New java.awt.Rectangle(childMinX,childMinY, w, h), New java.awt.Point(sampleModelTranslateX+deltaX, sampleModelTranslateY+deltaY), Me)
		End Function

		''' <summary>
		''' Sets the data for a single pixel from a
		''' primitive array of type TransferType.  For image data supported by
		''' the Java 2D(tm) API, this will be one of DataBuffer.TYPE_BYTE,
		''' DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		''' DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		''' may be in a packed format, thus increasing efficiency for data
		''' transfers.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds, or if inData is not large enough to hold the pixel data.
		''' However, explicit bounds checking is not guaranteed.
		''' A ClassCastException will be thrown if the input object is not null
		''' and references anything other than an array of TransferType. </summary>
		''' <seealso cref= java.awt.image.SampleModel#setDataElements(int, int, Object, DataBuffer) </seealso>
		''' <param name="x">        The X coordinate of the pixel location. </param>
		''' <param name="y">        The Y coordinate of the pixel location. </param>
		''' <param name="inData">   An object reference to an array of type defined by
		'''                 getTransferType() and length getNumDataElements()
		'''                 containing the pixel data to place at x,y.
		''' </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if inData is too small to hold the input. </exception>
		Public Overridable Sub setDataElements(  x As Integer,   y As Integer,   inData As Object)
			sampleModel.dataElementsnts(x-sampleModelTranslateX, y-sampleModelTranslateY, inData, dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets the data for a rectangle of pixels from an input Raster.
		''' The input Raster must be compatible with this WritableRaster
		''' in that they must have the same number of bands, corresponding bands
		''' must have the same number of bits per sample, the TransferTypes
		''' and NumDataElements must be the same, and the packing used by
		''' the getDataElements/setDataElements must be identical.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location. </param>
		''' <param name="y">        The Y coordinate of the pixel location. </param>
		''' <param name="inRaster"> Raster containing data to place at x,y.
		''' </param>
		''' <exception cref="NullPointerException"> if inRaster is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds. </exception>
		Public Overridable Sub setDataElements(  x As Integer,   y As Integer,   inRaster As Raster)
			Dim dstOffX As Integer = x+inRaster.minX
			Dim dstOffY As Integer = y+inRaster.minY
			Dim width_Renamed As Integer = inRaster.width
			Dim height_Renamed As Integer = inRaster.height
			If (dstOffX < Me.minX) OrElse (dstOffY < Me.minY) OrElse (dstOffX + width_Renamed > Me.minX + Me.width) OrElse (dstOffY + height_Renamed > Me.minY + Me.height) Then Throw New ArrayIndexOutOfBoundsException("Coordinate out of bounds!")

			Dim srcOffX As Integer = inRaster.minX
			Dim srcOffY As Integer = inRaster.minY
			Dim tdata As Object = Nothing

			For startY As Integer = 0 To height_Renamed - 1
				tdata = inRaster.getDataElements(srcOffX, srcOffY+startY, width_Renamed, 1, tdata)
				dataElementsnts(dstOffX, dstOffY+startY, width_Renamed, 1, tdata)
			Next startY
		End Sub

		''' <summary>
		''' Sets the data for a rectangle of pixels from a
		''' primitive array of type TransferType.  For image data supported by
		''' the Java 2D API, this will be one of DataBuffer.TYPE_BYTE,
		''' DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT, DataBuffer.TYPE_SHORT,
		''' DataBuffer.TYPE_FLOAT, or DataBuffer.TYPE_DOUBLE.  Data in the array
		''' may be in a packed format, thus increasing efficiency for data
		''' transfers.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds, or if inData is not large enough to hold the pixel data.
		''' However, explicit bounds checking is not guaranteed.
		''' A ClassCastException will be thrown if the input object is not null
		''' and references anything other than an array of TransferType. </summary>
		''' <seealso cref= java.awt.image.SampleModel#setDataElements(int, int, int, int, Object, DataBuffer) </seealso>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="inData">   An object reference to an array of type defined by
		'''                 getTransferType() and length w*h*getNumDataElements()
		'''                 containing the pixel data to place between x,y and
		'''                 x+w-1, y+h-1.
		''' </param>
		''' <exception cref="NullPointerException"> if inData is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if inData is too small to hold the input. </exception>
		Public Overridable Sub setDataElements(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   inData As Object)
			sampleModel.dataElementsnts(x-sampleModelTranslateX, y-sampleModelTranslateY, w,h,inData,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Copies pixels from Raster srcRaster to this WritableRaster.  Each pixel
		''' in srcRaster is copied to the same x,y address in this raster, unless
		''' the address falls outside the bounds of this raster.  srcRaster
		''' must have the same number of bands as this WritableRaster.  The
		''' copy is a simple copy of source samples to the corresponding destination
		''' samples.
		''' <p>
		''' If all samples of both source and destination Rasters are of
		''' integral type and less than or equal to 32 bits in size, then calling
		''' this method is equivalent to executing the following code for all
		''' <code>x,y</code> addresses valid in both Rasters.
		''' <pre>{@code
		'''       Raster srcRaster;
		'''       WritableRaster dstRaster;
		'''       for (int b = 0; b < srcRaster.getNumBands(); b++) {
		'''           dstRaster.setSample(x, y, b, srcRaster.getSample(x, y, b));
		'''       }
		''' }</pre>
		''' Thus, when copying an integral type source to an integral type
		''' destination, if the source sample size is greater than the destination
		''' sample size for a particular band, the high order bits of the source
		''' sample are truncated.  If the source sample size is less than the
		''' destination size for a particular band, the high order bits of the
		''' destination are zero-extended or sign-extended depending on whether
		''' srcRaster's SampleModel treats the sample as a signed or unsigned
		''' quantity.
		''' <p>
		''' When copying a float or double source to an integral type destination,
		''' each source sample is cast to the destination type.  When copying an
		''' integral type source to a float or double destination, the source
		''' is first converted to a 32-bit int (if necessary), using the above
		''' rules for integral types, and then the int is cast to float or
		''' java.lang.[Double].
		''' <p> </summary>
		''' <param name="srcRaster">  The  Raster from which to copy pixels.
		''' </param>
		''' <exception cref="NullPointerException"> if srcRaster is null. </exception>
		Public Overridable Property rect As Raster
			Set(  srcRaster As Raster)
				rectect(0,0,srcRaster)
			End Set
		End Property

		''' <summary>
		''' Copies pixels from Raster srcRaster to this WritableRaster.
		''' For each (x, y) address in srcRaster, the corresponding pixel
		''' is copied to address (x+dx, y+dy) in this WritableRaster,
		''' unless (x+dx, y+dy) falls outside the bounds of this raster.
		''' srcRaster must have the same number of bands as this WritableRaster.
		''' The copy is a simple copy of source samples to the corresponding
		''' destination samples.  For details, see
		''' <seealso cref="WritableRaster#setRect(Raster)"/>.
		''' </summary>
		''' <param name="dx">        The X translation factor from src space to dst space
		'''                  of the copy. </param>
		''' <param name="dy">        The Y translation factor from src space to dst space
		'''                  of the copy. </param>
		''' <param name="srcRaster"> The Raster from which to copy pixels.
		''' </param>
		''' <exception cref="NullPointerException"> if srcRaster is null. </exception>
		Public Overridable Sub setRect(  dx As Integer,   dy As Integer,   srcRaster As Raster)
			Dim width_Renamed As Integer = srcRaster.width
			Dim height_Renamed As Integer = srcRaster.height
			Dim srcOffX As Integer = srcRaster.minX
			Dim srcOffY As Integer = srcRaster.minY
			Dim dstOffX As Integer = dx+srcOffX
			Dim dstOffY As Integer = dy+srcOffY

			' Clip to this raster
			If dstOffX < Me.minX Then
				Dim skipX As Integer = Me.minX - dstOffX
				width_Renamed -= skipX
				srcOffX += skipX
				dstOffX = Me.minX
			End If
			If dstOffY < Me.minY Then
				Dim skipY As Integer = Me.minY - dstOffY
				height_Renamed -= skipY
				srcOffY += skipY
				dstOffY = Me.minY
			End If
			If dstOffX+width_Renamed > Me.minX+Me.width Then width_Renamed = Me.minX + Me.width - dstOffX
			If dstOffY+height_Renamed > Me.minY+Me.height Then height_Renamed = Me.minY + Me.height - dstOffY

			If width_Renamed <= 0 OrElse height_Renamed <= 0 Then Return

			Select Case srcRaster.sampleModel.dataType
			Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_SHORT, DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT
				Dim iData As Integer() = Nothing
				For startY As Integer = 0 To height_Renamed - 1
					' Grab one scanline at a time
					iData = srcRaster.getPixels(srcOffX, srcOffY+startY, width_Renamed, 1, iData)
					pixelsels(dstOffX, dstOffY+startY, width_Renamed, 1, iData)
				Next startY

			Case DataBuffer.TYPE_FLOAT
				Dim fData As Single() = Nothing
				For startY As Integer = 0 To height_Renamed - 1
					fData = srcRaster.getPixels(srcOffX, srcOffY+startY, width_Renamed, 1, fData)
					pixelsels(dstOffX, dstOffY+startY, width_Renamed, 1, fData)
				Next startY

			Case DataBuffer.TYPE_DOUBLE
				Dim dData As Double() = Nothing
				For startY As Integer = 0 To height_Renamed - 1
					' Grab one scanline at a time
					dData = srcRaster.getPixels(srcOffX, srcOffY+startY, width_Renamed, 1, dData)
					pixelsels(dstOffX, dstOffY+startY, width_Renamed, 1, dData)
				Next startY
			End Select
		End Sub

		''' <summary>
		''' Sets a pixel in the DataBuffer using an int array of samples for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">      The X coordinate of the pixel location. </param>
		''' <param name="y">      The Y coordinate of the pixel location. </param>
		''' <param name="iArray"> The input samples in a int array.
		''' </param>
		''' <exception cref="NullPointerException"> if iArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if iArray is too small to hold the input. </exception>
		Public Overridable Sub setPixel(  x As Integer,   y As Integer,   iArray As Integer())
			sampleModel.pixelxel(x-sampleModelTranslateX,y-sampleModelTranslateY, iArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets a pixel in the DataBuffer using a float array of samples for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">      The X coordinate of the pixel location. </param>
		''' <param name="y">      The Y coordinate of the pixel location. </param>
		''' <param name="fArray"> The input samples in a float array.
		''' </param>
		''' <exception cref="NullPointerException"> if fArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if fArray is too small to hold the input. </exception>
		Public Overridable Sub setPixel(  x As Integer,   y As Integer,   fArray As Single())
			sampleModel.pixelxel(x-sampleModelTranslateX,y-sampleModelTranslateY, fArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets a pixel in the DataBuffer using a double array of samples for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">      The X coordinate of the pixel location. </param>
		''' <param name="y">      The Y coordinate of the pixel location. </param>
		''' <param name="dArray"> The input samples in a double array.
		''' </param>
		''' <exception cref="NullPointerException"> if dArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if dArray is too small to hold the input. </exception>
		Public Overridable Sub setPixel(  x As Integer,   y As Integer,   dArray As Double())
			sampleModel.pixelxel(x-sampleModelTranslateX,y-sampleModelTranslateY, dArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets all samples for a rectangle of pixels from an int array containing
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="iArray">   The input int pixel array.
		''' </param>
		''' <exception cref="NullPointerException"> if iArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if iArray is too small to hold the input. </exception>
		Public Overridable Sub setPixels(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   iArray As Integer())
			sampleModel.pixelsels(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,iArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets all samples for a rectangle of pixels from a float array containing
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="fArray">   The input float pixel array.
		''' </param>
		''' <exception cref="NullPointerException"> if fArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if fArray is too small to hold the input. </exception>
		Public Overridable Sub setPixels(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   fArray As Single())
			sampleModel.pixelsels(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,fArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets all samples for a rectangle of pixels from a double array containing
		''' one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="dArray">   The input double pixel array.
		''' </param>
		''' <exception cref="NullPointerException"> if dArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates are not
		''' in bounds, or if dArray is too small to hold the input. </exception>
		Public Overridable Sub setPixels(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   dArray As Double())
			sampleModel.pixelsels(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,dArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using an int for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location. </param>
		''' <param name="y">        The Y coordinate of the pixel location. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="s">        The input sample.
		''' </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public Overridable Sub setSample(  x As Integer,   y As Integer,   b As Integer,   s As Integer)
			sampleModel.sampleple(x-sampleModelTranslateX, y-sampleModelTranslateY, b, s, dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a float for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location. </param>
		''' <param name="y">        The Y coordinate of the pixel location. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="s">        The input sample as a float.
		''' </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public Overridable Sub setSample(  x As Integer,   y As Integer,   b As Integer,   s As Single)
			sampleModel.sampleple(x-sampleModelTranslateX,y-sampleModelTranslateY, b,s,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets a sample in the specified band for the pixel located at (x,y)
		''' in the DataBuffer using a double for input.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the pixel location. </param>
		''' <param name="y">        The Y coordinate of the pixel location. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="s">        The input sample as a java.lang.[Double].
		''' </param>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds. </exception>
		Public Overridable Sub setSample(  x As Integer,   y As Integer,   b As Integer,   s As Double)
			sampleModel.sampleple(x-sampleModelTranslateX,y-sampleModelTranslateY, b,s,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from an int array containing one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="iArray">   The input int sample array.
		''' </param>
		''' <exception cref="NullPointerException"> if iArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if iArray is too small to
		''' hold the input. </exception>
		Public Overridable Sub setSamples(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   b As Integer,   iArray As Integer())
			sampleModel.samplesles(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,b,iArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from a float array containing one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="fArray">   The input float sample array.
		''' </param>
		''' <exception cref="NullPointerException"> if fArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if fArray is too small to
		''' hold the input. </exception>
		Public Overridable Sub setSamples(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   b As Integer,   fArray As Single())
			sampleModel.samplesles(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,b,fArray,dataBuffer_Renamed)
		End Sub

		''' <summary>
		''' Sets the samples in the specified band for the specified rectangle
		''' of pixels from a double array containing one sample per array element.
		''' An ArrayIndexOutOfBoundsException may be thrown if the coordinates are
		''' not in bounds.
		''' However, explicit bounds checking is not guaranteed. </summary>
		''' <param name="x">        The X coordinate of the upper left pixel location. </param>
		''' <param name="y">        The Y coordinate of the upper left pixel location. </param>
		''' <param name="w">        Width of the pixel rectangle. </param>
		''' <param name="h">        Height of the pixel rectangle. </param>
		''' <param name="b">        The band to set. </param>
		''' <param name="dArray">   The input double sample array.
		''' </param>
		''' <exception cref="NullPointerException"> if dArray is null. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if the coordinates or
		''' the band index are not in bounds, or if dArray is too small to
		''' hold the input. </exception>
		Public Overridable Sub setSamples(  x As Integer,   y As Integer,   w As Integer,   h As Integer,   b As Integer,   dArray As Double())
			sampleModel.samplesles(x-sampleModelTranslateX,y-sampleModelTranslateY, w,h,b,dArray,dataBuffer_Renamed)
		End Sub

	End Class

End Namespace