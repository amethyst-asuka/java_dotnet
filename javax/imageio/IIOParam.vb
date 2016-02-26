'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio


	''' <summary>
	''' A superclass of all classes describing how streams should be
	''' decoded or encoded.  This class contains all the variables and
	''' methods that are shared by <code>ImageReadParam</code> and
	''' <code>ImageWriteParam</code>.
	''' 
	''' <p> This class provides mechanisms to specify a source region and a
	''' destination region.  When reading, the source is the stream and
	''' the in-memory image is the destination.  When writing, these are
	''' reversed.  In the case of writing, destination regions may be used
	''' only with a writer that supports pixel replacement.
	''' <p>
	''' Decimation subsampling may be specified for both readers
	''' and writers, using a movable subsampling grid.
	''' <p>
	''' Subsets of the source and destination bands may be selected.
	''' 
	''' </summary>
	Public MustInherit Class IIOParam

		''' <summary>
		''' The source region, on <code>null</code> if none is set.
		''' </summary>
		Protected Friend sourceRegion As java.awt.Rectangle = Nothing

		''' <summary>
		''' The decimation subsampling to be applied in the horizontal
		''' direction.  By default, the value is <code>1</code>.
		''' The value must not be negative or 0.
		''' </summary>
		Protected Friend sourceXSubsampling As Integer = 1

		''' <summary>
		''' The decimation subsampling to be applied in the vertical
		''' direction.  By default, the value is <code>1</code>.
		''' The value must not be negative or 0.
		''' </summary>
		Protected Friend sourceYSubsampling As Integer = 1

		''' <summary>
		''' A horizontal offset to be applied to the subsampling grid before
		''' subsampling.  The first pixel to be used will be offset this
		''' amount from the origin of the region, or of the image if no
		''' region is specified.
		''' </summary>
		Protected Friend subsamplingXOffset As Integer = 0

		''' <summary>
		''' A vertical offset to be applied to the subsampling grid before
		''' subsampling.  The first pixel to be used will be offset this
		''' amount from the origin of the region, or of the image if no
		''' region is specified.
		''' </summary>
		Protected Friend subsamplingYOffset As Integer = 0

		''' <summary>
		''' An array of <code>int</code>s indicating which source bands
		''' will be used, or <code>null</code>.  If <code>null</code>, the
		''' set of source bands to be used is as described in the comment
		''' for the <code>setSourceBands</code> method.  No value should
		''' be allowed to be negative.
		''' </summary>
		Protected Friend sourceBands As Integer() = Nothing

		''' <summary>
		''' An <code>ImageTypeSpecifier</code> to be used to generate a
		''' destination image when reading, or to set the output color type
		''' when writing.  If non has been set the value will be
		''' <code>null</code>.  By default, the value is <code>null</code>.
		''' </summary>
		Protected Friend destinationType As ImageTypeSpecifier = Nothing

		''' <summary>
		''' The offset in the destination where the upper-left decoded
		''' pixel should be placed.  By default, the value is (0, 0).
		''' </summary>
		Protected Friend destinationOffset As New java.awt.Point(0, 0)

		''' <summary>
		''' The default <code>IIOParamController</code> that will be
		''' used to provide settings for this <code>IIOParam</code>
		''' object when the <code>activateController</code> method
		''' is called.  This default should be set by subclasses
		''' that choose to provide their own default controller,
		''' usually a GUI, for setting parameters.
		''' </summary>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #activateController </seealso>
		Protected Friend defaultController As IIOParamController = Nothing

		''' <summary>
		''' The <code>IIOParamController</code> that will be
		''' used to provide settings for this <code>IIOParam</code>
		''' object when the <code>activateController</code> method
		''' is called.  This value overrides any default controller,
		''' even when null.
		''' </summary>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #setController(IIOParamController) </seealso>
		''' <seealso cref= #hasController() </seealso>
		''' <seealso cref= #activateController() </seealso>
		Protected Friend controller As IIOParamController = Nothing

		''' <summary>
		''' Protected constructor may be called only by subclasses.
		''' </summary>
		Protected Friend Sub New()
			controller = defaultController
		End Sub

		''' <summary>
		''' Sets the source region of interest.  The region of interest is
		''' described as a rectangle, with the upper-left corner of the
		''' source image as pixel (0, 0) and increasing values down and to
		''' the right.  The actual number of pixels used will depend on
		''' the subsampling factors set by <code>setSourceSubsampling</code>.
		''' If subsampling has been set such that this number is zero,
		''' an <code>IllegalStateException</code> will be thrown.
		''' 
		''' <p> The source region of interest specified by this method will
		''' be clipped as needed to fit within the source bounds, as well
		''' as the destination offsets, width, and height at the time of
		''' actual I/O.
		''' 
		''' <p> A value of <code>null</code> for <code>sourceRegion</code>
		''' will remove any region specification, causing the entire image
		''' to be used.
		''' </summary>
		''' <param name="sourceRegion"> a <code>Rectangle</code> specifying the
		''' source region of interest, or <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>sourceRegion</code> is non-<code>null</code> and either
		''' <code>sourceRegion.x</code> or <code>sourceRegion.y</code> is
		''' negative. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>sourceRegion</code> is non-<code>null</code> and either
		''' <code>sourceRegion.width</code> or
		''' <code>sourceRegion.height</code> is negative or 0. </exception>
		''' <exception cref="IllegalStateException"> if subsampling is such that
		''' this region will have a subsampled width or height of zero.
		''' </exception>
		''' <seealso cref= #getSourceRegion </seealso>
		''' <seealso cref= #setSourceSubsampling </seealso>
		''' <seealso cref= ImageReadParam#setDestinationOffset </seealso>
		''' <seealso cref= ImageReadParam#getDestinationOffset </seealso>
		Public Overridable Property sourceRegion As java.awt.Rectangle
			Set(ByVal sourceRegion As java.awt.Rectangle)
				If sourceRegion Is Nothing Then
					Me.sourceRegion = Nothing
					Return
				End If
    
				If sourceRegion.x < 0 Then Throw New System.ArgumentException("sourceRegion.x < 0!")
				If sourceRegion.y < 0 Then Throw New System.ArgumentException("sourceRegion.y < 0!")
				If sourceRegion.width <= 0 Then Throw New System.ArgumentException("sourceRegion.width <= 0!")
				If sourceRegion.height <= 0 Then Throw New System.ArgumentException("sourceRegion.height <= 0!")
    
				' Throw an IllegalStateException if region falls between subsamples
				If sourceRegion.width <= subsamplingXOffset Then Throw New IllegalStateException("sourceRegion.width <= subsamplingXOffset!")
				If sourceRegion.height <= subsamplingYOffset Then Throw New IllegalStateException("sourceRegion.height <= subsamplingYOffset!")
    
				Me.sourceRegion = CType(sourceRegion.clone(), java.awt.Rectangle)
			End Set
			Get
				If sourceRegion Is Nothing Then Return Nothing
				Return CType(sourceRegion.clone(), java.awt.Rectangle)
			End Get
		End Property


		''' <summary>
		''' Specifies a decimation subsampling to apply on I/O.  The
		''' <code>sourceXSubsampling</code> and
		''' <code>sourceYSubsampling</code> parameters specify the
		''' subsampling period (<i>i.e.</i>, the number of rows and columns
		''' to advance after every source pixel).  Specifically, a period of
		''' 1 will use every row or column; a period of 2 will use every
		''' other row or column.  The <code>subsamplingXOffset</code> and
		''' <code>subsamplingYOffset</code> parameters specify an offset
		''' from the region (or image) origin for the first subsampled pixel.
		''' Adjusting the origin of the subsample grid is useful for avoiding
		''' seams when subsampling a very large source image into destination
		''' regions that will be assembled into a complete subsampled image.
		''' Most users will want to simply leave these parameters at 0.
		''' 
		''' <p> The number of pixels and scanlines to be used are calculated
		''' as follows.
		''' <p>
		''' The number of subsampled pixels in a scanline is given by
		''' <p>
		''' <code>truncate[(width - subsamplingXOffset + sourceXSubsampling - 1)
		''' / sourceXSubsampling]</code>.
		''' <p>
		''' If the region is such that this width is zero, an
		''' <code>IllegalStateException</code> is thrown.
		''' <p>
		''' The number of scanlines to be used can be computed similarly.
		''' 
		''' <p>The ability to set the subsampling grid to start somewhere
		''' other than the source region origin is useful if the
		''' region is being used to create subsampled tiles of a large image,
		''' where the tile width and height are not multiples of the
		''' subsampling periods.  If the subsampling grid does not remain
		''' consistent from tile to tile, there will be artifacts at the tile
		''' boundaries.  By adjusting the subsampling grid offset for each
		''' tile to compensate, these artifacts can be avoided.  The tradeoff
		''' is that in order to avoid these artifacts, the tiles are not all
		''' the same size.  The grid offset to use in this case is given by:
		''' <br>
		''' grid offset = [period - (region offset modulo period)] modulo period)
		''' 
		''' <p> If either <code>sourceXSubsampling</code> or
		''' <code>sourceYSubsampling</code> is 0 or negative, an
		''' <code>IllegalArgumentException</code> will be thrown.
		''' 
		''' <p> If either <code>subsamplingXOffset</code> or
		''' <code>subsamplingYOffset</code> is negative or greater than or
		''' equal to the corresponding period, an
		''' <code>IllegalArgumentException</code> will be thrown.
		''' 
		''' <p> There is no <code>unsetSourceSubsampling</code> method;
		''' simply call <code>setSourceSubsampling(1, 1, 0, 0)</code> to
		''' restore default values.
		''' </summary>
		''' <param name="sourceXSubsampling"> the number of columns to advance
		''' between pixels. </param>
		''' <param name="sourceYSubsampling"> the number of rows to advance between
		''' pixels. </param>
		''' <param name="subsamplingXOffset"> the horizontal offset of the first subsample
		''' within the region, or within the image if no region is set. </param>
		''' <param name="subsamplingYOffset"> the horizontal offset of the first subsample
		''' within the region, or within the image if no region is set. </param>
		''' <exception cref="IllegalArgumentException"> if either period is
		''' negative or 0, or if either grid offset is negative or greater than
		''' the corresponding period. </exception>
		''' <exception cref="IllegalStateException"> if the source region is such that
		''' the subsampled output would contain no pixels. </exception>
		Public Overridable Sub setSourceSubsampling(ByVal sourceXSubsampling As Integer, ByVal sourceYSubsampling As Integer, ByVal subsamplingXOffset As Integer, ByVal subsamplingYOffset As Integer)
			If sourceXSubsampling <= 0 Then Throw New System.ArgumentException("sourceXSubsampling <= 0!")
			If sourceYSubsampling <= 0 Then Throw New System.ArgumentException("sourceYSubsampling <= 0!")
			If subsamplingXOffset < 0 OrElse subsamplingXOffset >= sourceXSubsampling Then Throw New System.ArgumentException("subsamplingXOffset out of range!")
			If subsamplingYOffset < 0 OrElse subsamplingYOffset >= sourceYSubsampling Then Throw New System.ArgumentException("subsamplingYOffset out of range!")

			' Throw an IllegalStateException if region falls between subsamples
			If sourceRegion IsNot Nothing Then
				If subsamplingXOffset >= sourceRegion.width OrElse subsamplingYOffset >= sourceRegion.height Then Throw New IllegalStateException("region contains no pixels!")
			End If

			Me.sourceXSubsampling = sourceXSubsampling
			Me.sourceYSubsampling = sourceYSubsampling
			Me.subsamplingXOffset = subsamplingXOffset
			Me.subsamplingYOffset = subsamplingYOffset
		End Sub

		''' <summary>
		''' Returns the number of source columns to advance for each pixel.
		''' 
		''' <p>If <code>setSourceSubsampling</code> has not been called, 1
		''' is returned (which is the correct value).
		''' </summary>
		''' <returns> the source subsampling X period.
		''' </returns>
		''' <seealso cref= #setSourceSubsampling </seealso>
		''' <seealso cref= #getSourceYSubsampling </seealso>
		Public Overridable Property sourceXSubsampling As Integer
			Get
				Return sourceXSubsampling
			End Get
		End Property

		''' <summary>
		''' Returns the number of rows to advance for each pixel.
		''' 
		''' <p>If <code>setSourceSubsampling</code> has not been called, 1
		''' is returned (which is the correct value).
		''' </summary>
		''' <returns> the source subsampling Y period.
		''' </returns>
		''' <seealso cref= #setSourceSubsampling </seealso>
		''' <seealso cref= #getSourceXSubsampling </seealso>
		Public Overridable Property sourceYSubsampling As Integer
			Get
				Return sourceYSubsampling
			End Get
		End Property

		''' <summary>
		''' Returns the horizontal offset of the subsampling grid.
		''' 
		''' <p>If <code>setSourceSubsampling</code> has not been called, 0
		''' is returned (which is the correct value).
		''' </summary>
		''' <returns> the source subsampling grid X offset.
		''' </returns>
		''' <seealso cref= #setSourceSubsampling </seealso>
		''' <seealso cref= #getSubsamplingYOffset </seealso>
		Public Overridable Property subsamplingXOffset As Integer
			Get
				Return subsamplingXOffset
			End Get
		End Property

		''' <summary>
		''' Returns the vertical offset of the subsampling grid.
		''' 
		''' <p>If <code>setSourceSubsampling</code> has not been called, 0
		''' is returned (which is the correct value).
		''' </summary>
		''' <returns> the source subsampling grid Y offset.
		''' </returns>
		''' <seealso cref= #setSourceSubsampling </seealso>
		''' <seealso cref= #getSubsamplingXOffset </seealso>
		Public Overridable Property subsamplingYOffset As Integer
			Get
				Return subsamplingYOffset
			End Get
		End Property

		''' <summary>
		''' Sets the indices of the source bands to be used.  Duplicate
		''' indices are not allowed.
		''' 
		''' <p> A <code>null</code> value indicates that all source bands
		''' will be used.
		''' 
		''' <p> At the time of reading, an
		''' <code>IllegalArgumentException</code> will be thrown by the
		''' reader or writer if a value larger than the largest available
		''' source band index has been specified or if the number of source
		''' bands and destination bands to be used differ.  The
		''' <code>ImageReader.checkReadParamBandSettings</code> method may
		''' be used to automate this test.
		''' 
		''' <p> Semantically, a copy is made of the array; changes to the
		''' array contents subsequent to this call have no effect on
		''' this <code>IIOParam</code>.
		''' </summary>
		''' <param name="sourceBands"> an array of integer band indices to be
		''' used.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>sourceBands</code>
		''' contains a negative or duplicate value.
		''' </exception>
		''' <seealso cref= #getSourceBands </seealso>
		''' <seealso cref= ImageReadParam#setDestinationBands </seealso>
		''' <seealso cref= ImageReader#checkReadParamBandSettings </seealso>
		Public Overridable Property sourceBands As Integer()
			Set(ByVal sourceBands As Integer())
				If sourceBands Is Nothing Then
					Me.sourceBands = Nothing
				Else
					Dim numBands As Integer = sourceBands.Length
					For i As Integer = 0 To numBands - 1
						Dim band As Integer = sourceBands(i)
						If band < 0 Then Throw New System.ArgumentException("Band value < 0!")
						For j As Integer = i + 1 To numBands - 1
							If band = sourceBands(j) Then Throw New System.ArgumentException("Duplicate band value!")
						Next j
    
					Next i
					Me.sourceBands = CType(sourceBands.clone(), Integer())
				End If
			End Set
			Get
				If sourceBands Is Nothing Then Return Nothing
				Return CType(sourceBands.clone(), Integer())
			End Get
		End Property


		''' <summary>
		''' Sets the desired image type for the destination image, using an
		''' <code>ImageTypeSpecifier</code>.
		''' 
		''' <p> When reading, if the layout of the destination has been set
		''' using this method, each call to an <code>ImageReader</code>
		''' <code>read</code> method will return a new
		''' <code>BufferedImage</code> using the format specified by the
		''' supplied type specifier.  As a side effect, any destination
		''' <code>BufferedImage</code> set by
		''' <code>ImageReadParam.setDestination(BufferedImage)</code> will
		''' no longer be set as the destination.  In other words, this
		''' method may be thought of as calling
		''' <code>setDestination((BufferedImage)null)</code>.
		''' 
		''' <p> When writing, the destination type maybe used to determine
		''' the color type of the image.  The <code>SampleModel</code>
		''' information will be ignored, and may be <code>null</code>.  For
		''' example, a 4-banded image could represent either CMYK or RGBA
		''' data.  If a destination type is set, its
		''' <code>ColorModel</code> will override any
		''' <code>ColorModel</code> on the image itself.  This is crucial
		''' when <code>setSourceBands</code> is used since the image's
		''' <code>ColorModel</code> will refer to the entire image rather
		''' than to the subset of bands being written.
		''' </summary>
		''' <param name="destinationType"> the <code>ImageTypeSpecifier</code> to
		''' be used to determine the destination layout and color type.
		''' </param>
		''' <seealso cref= #getDestinationType </seealso>
		Public Overridable Property destinationType As ImageTypeSpecifier
			Set(ByVal destinationType As ImageTypeSpecifier)
				Me.destinationType = destinationType
			End Set
			Get
				Return destinationType
			End Get
		End Property


		''' <summary>
		''' Specifies the offset in the destination image at which future
		''' decoded pixels are to be placed, when reading, or where a
		''' region will be written, when writing.
		''' 
		''' <p> When reading, the region to be written within the
		''' destination <code>BufferedImage</code> will start at this
		''' offset and have a width and height determined by the source
		''' region of interest, the subsampling parameters, and the
		''' destination bounds.
		''' 
		''' <p> Normal writes are not affected by this method, only writes
		''' performed using <code>ImageWriter.replacePixels</code>.  For
		''' such writes, the offset specified is within the output stream
		''' image whose pixels are being modified.
		''' 
		''' <p> There is no <code>unsetDestinationOffset</code> method;
		''' simply call <code>setDestinationOffset(new Point(0, 0))</code> to
		''' restore default values.
		''' </summary>
		''' <param name="destinationOffset"> the offset in the destination, as a
		''' <code>Point</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>destinationOffset</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getDestinationOffset </seealso>
		''' <seealso cref= ImageWriter#replacePixels </seealso>
		Public Overridable Property destinationOffset As java.awt.Point
			Set(ByVal destinationOffset As java.awt.Point)
				If destinationOffset Is Nothing Then Throw New System.ArgumentException("destinationOffset == null!")
				Me.destinationOffset = CType(destinationOffset.clone(), java.awt.Point)
			End Set
			Get
				Return CType(destinationOffset.clone(), java.awt.Point)
			End Get
		End Property


		''' <summary>
		''' Sets the <code>IIOParamController</code> to be used
		''' to provide settings for this <code>IIOParam</code>
		''' object when the <code>activateController</code> method
		''' is called, overriding any default controller.  If the
		''' argument is <code>null</code>, no controller will be
		''' used, including any default.  To restore the default, use
		''' <code>setController(getDefaultController())</code>.
		''' </summary>
		''' <param name="controller"> An appropriate
		''' <code>IIOParamController</code>, or <code>null</code>.
		''' </param>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #hasController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Property controller As IIOParamController
			Set(ByVal controller As IIOParamController)
				Me.controller = controller
			End Set
			Get
				Return controller
			End Get
		End Property


		''' <summary>
		''' Returns the default <code>IIOParamController</code>, if there
		''' is one, regardless of the currently installed controller.  If
		''' there is no default controller, returns <code>null</code>.
		''' </summary>
		''' <returns> the default <code>IIOParamController</code>, or
		''' <code>null</code>.
		''' </returns>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #setController(IIOParamController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #hasController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Property defaultController As IIOParamController
			Get
				Return defaultController
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if there is a controller installed
		''' for this <code>IIOParam</code> object.  This will return
		''' <code>true</code> if <code>getController</code> would not
		''' return <code>null</code>.
		''' </summary>
		''' <returns> <code>true</code> if a controller is installed.
		''' </returns>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #setController(IIOParamController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Function hasController() As Boolean
			Return (controller IsNot Nothing)
		End Function

		''' <summary>
		''' Activates the installed <code>IIOParamController</code> for
		''' this <code>IIOParam</code> object and returns the resulting
		''' value.  When this method returns <code>true</code>, all values
		''' for this <code>IIOParam</code> object will be ready for the
		''' next read or write operation.  If <code>false</code> is
		''' returned, no settings in this object will have been disturbed
		''' (<i>i.e.</i>, the user canceled the operation).
		''' 
		''' <p> Ordinarily, the controller will be a GUI providing a user
		''' interface for a subclass of <code>IIOParam</code> for a
		''' particular plug-in.  Controllers need not be GUIs, however.
		''' </summary>
		''' <returns> <code>true</code> if the controller completed normally.
		''' </returns>
		''' <exception cref="IllegalStateException"> if there is no controller
		''' currently installed.
		''' </exception>
		''' <seealso cref= IIOParamController </seealso>
		''' <seealso cref= #setController(IIOParamController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #hasController </seealso>
		Public Overridable Function activateController() As Boolean
			If Not hasController() Then Throw New IllegalStateException("hasController() == false!")
			Return controller.activate(Me)
		End Function
	End Class

End Namespace