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
	''' A class describing how a stream is to be encoded.  Instances of
	''' this class or its subclasses are used to supply prescriptive
	''' "how-to" information to instances of <code>ImageWriter</code>.
	''' 
	''' <p> A plug-in for a specific image format may define a subclass of
	''' this class, and return objects of that class from the
	''' <code>getDefaultWriteParam</code> method of its
	''' <code>ImageWriter</code> implementation.  For example, the built-in
	''' JPEG writer plug-in will return instances of
	''' <code>javax.imageio.plugins.jpeg.JPEGImageWriteParam</code>.
	''' 
	''' <p> The region of the image to be written is determined by first
	''' intersecting the actual bounds of the image with the rectangle
	''' specified by <code>IIOParam.setSourceRegion</code>, if any.  If the
	''' resulting rectangle has a width or height of zero, the writer will
	''' throw an <code>IIOException</code>. If the intersection is
	''' non-empty, writing will commence with the first subsampled pixel
	''' and include additional pixels within the intersected bounds
	''' according to the horizontal and vertical subsampling factors
	''' specified by {@link IIOParam#setSourceSubsampling
	''' IIOParam.setSourceSubsampling}.
	''' 
	''' <p> Individual features such as tiling, progressive encoding, and
	''' compression may be set in one of four modes.
	''' <code>MODE_DISABLED</code> disables the features;
	''' <code>MODE_DEFAULT</code> enables the feature with
	''' writer-controlled parameter values; <code>MODE_EXPLICIT</code>
	''' enables the feature and allows the use of a <code>set</code> method
	''' to provide additional parameters; and
	''' <code>MODE_COPY_FROM_METADATA</code> copies relevant parameter
	''' values from the stream and image metadata objects passed to the
	''' writer.  The default for all features is
	''' <code>MODE_COPY_FROM_METADATA</code>.  Non-standard features
	''' supplied in subclasses are encouraged, but not required to use a
	''' similar scheme.
	''' 
	''' <p> Plug-in writers may extend the functionality of
	''' <code>ImageWriteParam</code> by providing a subclass that implements
	''' additional, plug-in specific interfaces.  It is up to the plug-in
	''' to document what interfaces are available and how they are to be
	''' used.  Writers will silently ignore any extended features of an
	''' <code>ImageWriteParam</code> subclass of which they are not aware.
	''' Also, they may ignore any optional features that they normally
	''' disable when creating their own <code>ImageWriteParam</code>
	''' instances via <code>getDefaultWriteParam</code>.
	''' 
	''' <p> Note that unless a query method exists for a capability, it must
	''' be supported by all <code>ImageWriter</code> implementations
	''' (<i>e.g.</i> progressive encoding is optional, but subsampling must be
	''' supported).
	''' 
	''' </summary>
	''' <seealso cref= ImageReadParam </seealso>
	Public Class ImageWriteParam
		Inherits IIOParam

		''' <summary>
		''' A constant value that may be passed into methods such as
		''' <code>setTilingMode</code>, <code>setProgressiveMode</code>,
		''' and <code>setCompressionMode</code> to disable a feature for
		''' future writes.  That is, when this mode is set the stream will
		''' <b>not</b> be tiled, progressive, or compressed, and the
		''' relevant accessor methods will throw an
		''' <code>IllegalStateException</code>.
		''' </summary>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		''' <seealso cref= #setTilingMode </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		''' <seealso cref= #setCompressionMode </seealso>
		''' <seealso cref= #getCompressionMode </seealso>
		Public Const MODE_DISABLED As Integer = 0

		''' <summary>
		''' A constant value that may be passed into methods such as
		''' <code>setTilingMode</code>,
		''' <code>setProgressiveMode</code>, and
		''' <code>setCompressionMode</code> to enable that feature for
		''' future writes.  That is, when this mode is enabled the stream
		''' will be tiled, progressive, or compressed according to a
		''' sensible default chosen internally by the writer in a plug-in
		''' dependent way, and the relevant accessor methods will
		''' throw an <code>IllegalStateException</code>.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		''' <seealso cref= #setTilingMode </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		''' <seealso cref= #setCompressionMode </seealso>
		''' <seealso cref= #getCompressionMode </seealso>
		Public Const MODE_DEFAULT As Integer = 1

		''' <summary>
		''' A constant value that may be passed into methods such as
		''' <code>setTilingMode</code> or <code>setCompressionMode</code>
		''' to enable a feature for future writes. That is, when this mode
		''' is set the stream will be tiled or compressed according to
		''' additional information supplied to the corresponding
		''' <code>set</code> methods in this class and retrievable from the
		''' corresponding <code>get</code> methods.  Note that this mode is
		''' not supported for progressive output.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		''' <seealso cref= #setTilingMode </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		''' <seealso cref= #setCompressionMode </seealso>
		''' <seealso cref= #getCompressionMode </seealso>
		Public Const MODE_EXPLICIT As Integer = 2

		''' <summary>
		''' A constant value that may be passed into methods such as
		''' <code>setTilingMode</code>, <code>setProgressiveMode</code>, or
		''' <code>setCompressionMode</code> to enable that feature for
		''' future writes.  That is, when this mode is enabled the stream
		''' will be tiled, progressive, or compressed based on the contents
		''' of stream and/or image metadata passed into the write
		''' operation, and any relevant accessor methods will throw an
		''' <code>IllegalStateException</code>.
		''' 
		''' <p> This is the default mode for all features, so that a read
		''' including metadata followed by a write including metadata will
		''' preserve as much information as possible.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		''' <seealso cref= #setTilingMode </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		''' <seealso cref= #setCompressionMode </seealso>
		''' <seealso cref= #getCompressionMode </seealso>
		Public Const MODE_COPY_FROM_METADATA As Integer = 3

		' If more modes are added, this should be updated.
		Private Const MAX_MODE As Integer = MODE_COPY_FROM_METADATA

		''' <summary>
		''' A <code>boolean</code> that is <code>true</code> if this
		''' <code>ImageWriteParam</code> allows tile width and tile height
		''' parameters to be set.  By default, the value is
		''' <code>false</code>.  Subclasses must set the value manually.
		''' 
		''' <p> Subclasses that do not support writing tiles should ensure
		''' that this value is set to <code>false</code>.
		''' </summary>
		Protected Friend ___canWriteTiles As Boolean = False

		''' <summary>
		''' The mode controlling tiling settings, which Must be
		''' set to one of the four <code>MODE_*</code> values.  The default
		''' is <code>MODE_COPY_FROM_METADATA</code>.
		''' 
		''' <p> Subclasses that do not writing tiles may ignore this value.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setTilingMode </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		Protected Friend tilingMode As Integer = MODE_COPY_FROM_METADATA

		''' <summary>
		''' An array of preferred tile size range pairs.  The default value
		''' is <code>null</code>, which indicates that there are no
		''' preferred sizes.  If the value is non-<code>null</code>, it
		''' must have an even length of at least two.
		''' 
		''' <p> Subclasses that do not support writing tiles may ignore
		''' this value.
		''' </summary>
		''' <seealso cref= #getPreferredTileSizes </seealso>
		Protected Friend preferredTileSizes As java.awt.Dimension() = Nothing

		''' <summary>
		''' A <code>boolean</code> that is <code>true</code> if tiling
		''' parameters have been specified.
		''' 
		''' <p> Subclasses that do not support writing tiles may ignore
		''' this value.
		''' </summary>
		Protected Friend tilingSet As Boolean = False

		''' <summary>
		''' The width of each tile if tiling has been set, or 0 otherwise.
		''' 
		''' <p> Subclasses that do not support tiling may ignore this
		''' value.
		''' </summary>
		Protected Friend tileWidth As Integer = 0

		''' <summary>
		''' The height of each tile if tiling has been set, or 0 otherwise.
		''' The initial value is <code>0</code>.
		''' 
		''' <p> Subclasses that do not support tiling may ignore this
		''' value.
		''' </summary>
		Protected Friend tileHeight As Integer = 0

		''' <summary>
		''' A <code>boolean</code> that is <code>true</code> if this
		''' <code>ImageWriteParam</code> allows tiling grid offset
		''' parameters to be set.  By default, the value is
		''' <code>false</code>.  Subclasses must set the value manually.
		''' 
		''' <p> Subclasses that do not support writing tiles, or that
		''' support writing but not offsetting tiles must ensure that this
		''' value is set to <code>false</code>.
		''' </summary>
		Protected Friend ___canOffsetTiles As Boolean = False

		''' <summary>
		''' The amount by which the tile grid origin should be offset
		''' horizontally from the image origin if tiling has been set,
		''' or 0 otherwise.  The initial value is <code>0</code>.
		''' 
		''' <p> Subclasses that do not support offsetting tiles may ignore
		''' this value.
		''' </summary>
		Protected Friend tileGridXOffset As Integer = 0

		''' <summary>
		''' The amount by which the tile grid origin should be offset
		''' vertically from the image origin if tiling has been set,
		''' or 0 otherwise.  The initial value is <code>0</code>.
		''' 
		''' <p> Subclasses that do not support offsetting tiles may ignore
		''' this value.
		''' </summary>
		Protected Friend tileGridYOffset As Integer = 0

		''' <summary>
		''' A <code>boolean</code> that is <code>true</code> if this
		''' <code>ImageWriteParam</code> allows images to be written as a
		''' progressive sequence of increasing quality passes.  By default,
		''' the value is <code>false</code>.  Subclasses must set the value
		''' manually.
		''' 
		''' <p> Subclasses that do not support progressive encoding must
		''' ensure that this value is set to <code>false</code>.
		''' </summary>
		Protected Friend ___canWriteProgressive As Boolean = False

		''' <summary>
		''' The mode controlling progressive encoding, which must be set to
		''' one of the four <code>MODE_*</code> values, except
		''' <code>MODE_EXPLICIT</code>.  The default is
		''' <code>MODE_COPY_FROM_METADATA</code>.
		''' 
		''' <p> Subclasses that do not support progressive encoding may
		''' ignore this value.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		Protected Friend progressiveMode As Integer = MODE_COPY_FROM_METADATA

		''' <summary>
		''' A <code>boolean</code> that is <code>true</code> if this writer
		''' can write images using compression. By default, the value is
		''' <code>false</code>.  Subclasses must set the value manually.
		''' 
		''' <p> Subclasses that do not support compression must ensure that
		''' this value is set to <code>false</code>.
		''' </summary>
		Protected Friend ___canWriteCompressed As Boolean = False

		''' <summary>
		''' The mode controlling compression settings, which must be set to
		''' one of the four <code>MODE_*</code> values.  The default is
		''' <code>MODE_COPY_FROM_METADATA</code>.
		''' 
		''' <p> Subclasses that do not support compression may ignore this
		''' value.
		''' </summary>
		''' <seealso cref= #MODE_DISABLED </seealso>
		''' <seealso cref= #MODE_EXPLICIT </seealso>
		''' <seealso cref= #MODE_COPY_FROM_METADATA </seealso>
		''' <seealso cref= #MODE_DEFAULT </seealso>
		''' <seealso cref= #setCompressionMode </seealso>
		''' <seealso cref= #getCompressionMode </seealso>
		Protected Friend compressionMode As Integer = MODE_COPY_FROM_METADATA

		''' <summary>
		''' An array of <code>String</code>s containing the names of the
		''' available compression types.  Subclasses must set the value
		''' manually.
		''' 
		''' <p> Subclasses that do not support compression may ignore this
		''' value.
		''' </summary>
		Protected Friend compressionTypes As String() = Nothing

		''' <summary>
		''' A <code>String</code> containing the name of the current
		''' compression type, or <code>null</code> if none is set.
		''' 
		''' <p> Subclasses that do not support compression may ignore this
		''' value.
		''' </summary>
		Protected Friend compressionType As String = Nothing

		''' <summary>
		''' A <code>float</code> containing the current compression quality
		''' setting.  The initial value is <code>1.0F</code>.
		''' 
		''' <p> Subclasses that do not support compression may ignore this
		''' value.
		''' </summary>
		Protected Friend compressionQuality As Single = 1.0F

		''' <summary>
		''' A <code>Locale</code> to be used to localize compression type
		''' names and quality descriptions, or <code>null</code> to use a
		''' default <code>Locale</code>.  Subclasses must set the value
		''' manually.
		''' </summary>
		Protected Friend locale As java.util.Locale = Nothing

		''' <summary>
		''' Constructs an empty <code>ImageWriteParam</code>.  It is up to
		''' the subclass to set up the instance variables properly.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ImageWriteParam</code> set to use a
		''' given <code>Locale</code>.
		''' </summary>
		''' <param name="locale"> a <code>Locale</code> to be used to localize
		''' compression type names and quality descriptions, or
		''' <code>null</code>. </param>
		Public Sub New(ByVal locale As java.util.Locale)
			Me.locale = locale
		End Sub

		' Return a deep copy of the array
		Private Shared Function clonePreferredTileSizes(ByVal sizes As java.awt.Dimension()) As java.awt.Dimension()
			If sizes Is Nothing Then Return Nothing
			Dim temp As java.awt.Dimension() = New java.awt.Dimension(sizes.Length - 1){}
			For i As Integer = 0 To sizes.Length - 1
				temp(i) = New java.awt.Dimension(sizes(i))
			Next i
			Return temp
		End Function

		''' <summary>
		''' Returns the currently set <code>Locale</code>, or
		''' <code>null</code> if only a default <code>Locale</code> is
		''' supported.
		''' </summary>
		''' <returns> the current <code>Locale</code>, or <code>null</code>. </returns>
		Public Overridable Property locale As java.util.Locale
			Get
				Return locale
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the writer can perform tiling
		''' while writing.  If this method returns <code>false</code>, then
		''' <code>setTiling</code> will throw an
		''' <code>UnsupportedOperationException</code>.
		''' </summary>
		''' <returns> <code>true</code> if the writer supports tiling.
		''' </returns>
		''' <seealso cref= #canOffsetTiles() </seealso>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		Public Overridable Function canWriteTiles() As Boolean
			Return ___canWriteTiles
		End Function

		''' <summary>
		''' Returns <code>true</code> if the writer can perform tiling with
		''' non-zero grid offsets while writing.  If this method returns
		''' <code>false</code>, then <code>setTiling</code> will throw an
		''' <code>UnsupportedOperationException</code> if the grid offset
		''' arguments are not both zero.  If <code>canWriteTiles</code>
		''' returns <code>false</code>, this method will return
		''' <code>false</code> as well.
		''' </summary>
		''' <returns> <code>true</code> if the writer supports non-zero tile
		''' offsets.
		''' </returns>
		''' <seealso cref= #canWriteTiles() </seealso>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		Public Overridable Function canOffsetTiles() As Boolean
			Return ___canOffsetTiles
		End Function

		''' <summary>
		''' Determines whether the image will be tiled in the output
		''' stream and, if it will, how the tiling parameters will be
		''' determined.  The modes are interpreted as follows:
		''' 
		''' <ul>
		''' 
		''' <li><code>MODE_DISABLED</code> - The image will not be tiled.
		''' <code>setTiling</code> will throw an
		''' <code>IllegalStateException</code>.
		''' 
		''' <li><code>MODE_DEFAULT</code> - The image will be tiled using
		''' default parameters.  <code>setTiling</code> will throw an
		''' <code>IllegalStateException</code>.
		''' 
		''' <li><code>MODE_EXPLICIT</code> - The image will be tiled
		''' according to parameters given in the <seealso cref="#setTiling setTiling"/>
		''' method.  Any previously set tiling parameters are discarded.
		''' 
		''' <li><code>MODE_COPY_FROM_METADATA</code> - The image will
		''' conform to the metadata object passed in to a write.
		''' <code>setTiling</code> will throw an
		''' <code>IllegalStateException</code>.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="mode"> The mode to use for tiling.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if
		''' <code>canWriteTiles</code> returns <code>false</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>mode</code> is not
		''' one of the modes listed above.
		''' </exception>
		''' <seealso cref= #setTiling </seealso>
		''' <seealso cref= #getTilingMode </seealso>
		Public Overridable Property tilingMode As Integer
			Set(ByVal mode As Integer)
				If canWriteTiles() = False Then Throw New System.NotSupportedException("Tiling not supported!")
				If mode < MODE_DISABLED OrElse mode > MAX_MODE Then Throw New System.ArgumentException("Illegal value for mode!")
				Me.tilingMode = mode
				If mode = MODE_EXPLICIT Then unsetTiling()
			End Set
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported")
				Return tilingMode
			End Get
		End Property


		''' <summary>
		''' Returns an array of <code>Dimension</code>s indicating the
		''' legal size ranges for tiles as they will be encoded in the
		''' output file or stream.  The returned array is a copy.
		''' 
		''' <p> The information is returned as a set of pairs; the first
		''' element of a pair contains an (inclusive) minimum width and
		''' height, and the second element contains an (inclusive) maximum
		''' width and height.  Together, each pair defines a valid range of
		''' sizes.  To specify a fixed size, use the same width and height
		''' for both elements.  To specify an arbitrary range, a value of
		''' <code>null</code> is used in place of an actual array of
		''' <code>Dimension</code>s.
		''' 
		''' <p> If no array is specified on the constructor, but tiling is
		''' allowed, then this method returns <code>null</code>.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does
		''' not support tiling.
		''' </exception>
		''' <returns> an array of <code>Dimension</code>s with an even length
		''' of at least two, or <code>null</code>. </returns>
		Public Overridable Property preferredTileSizes As java.awt.Dimension()
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported")
				Return clonePreferredTileSizes(preferredTileSizes)
			End Get
		End Property

		''' <summary>
		''' Specifies that the image should be tiled in the output stream.
		''' The <code>tileWidth</code> and <code>tileHeight</code>
		''' parameters specify the width and height of the tiles in the
		''' file.  If the tile width or height is greater than the width or
		''' height of the image, the image is not tiled in that dimension.
		''' 
		''' <p> If <code>canOffsetTiles</code> returns <code>false</code>,
		''' then the <code>tileGridXOffset</code> and
		''' <code>tileGridYOffset</code> parameters must be zero.
		''' </summary>
		''' <param name="tileWidth"> the width of each tile. </param>
		''' <param name="tileHeight"> the height of each tile. </param>
		''' <param name="tileGridXOffset"> the horizontal offset of the tile grid. </param>
		''' <param name="tileGridYOffset"> the vertical offset of the tile grid.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support grid offsets, and the grid offsets are not both zero. </exception>
		''' <exception cref="IllegalArgumentException"> if the tile size is not
		''' within one of the allowable ranges returned by
		''' <code>getPreferredTileSizes</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>tileWidth</code>
		''' or <code>tileHeight</code> is less than or equal to 0.
		''' </exception>
		''' <seealso cref= #canWriteTiles </seealso>
		''' <seealso cref= #canOffsetTiles </seealso>
		''' <seealso cref= #getTileWidth() </seealso>
		''' <seealso cref= #getTileHeight() </seealso>
		''' <seealso cref= #getTileGridXOffset() </seealso>
		''' <seealso cref= #getTileGridYOffset() </seealso>
		Public Overridable Sub setTiling(ByVal tileWidth As Integer, ByVal tileHeight As Integer, ByVal tileGridXOffset As Integer, ByVal tileGridYOffset As Integer)
			If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
			If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
			If tileWidth <= 0 OrElse tileHeight <= 0 Then Throw New System.ArgumentException("tile dimensions are non-positive!")
			Dim tilesOffset As Boolean = (tileGridXOffset <> 0) OrElse (tileGridYOffset <> 0)
			If (Not canOffsetTiles()) AndAlso tilesOffset Then Throw New System.NotSupportedException("Can't offset tiles!")
			If preferredTileSizes IsNot Nothing Then
				Dim ok As Boolean = True
				For i As Integer = 0 To preferredTileSizes.Length - 1 Step 2
					Dim min As java.awt.Dimension = preferredTileSizes(i)
					Dim max As java.awt.Dimension = preferredTileSizes(i+1)
					If (tileWidth < min.width) OrElse (tileWidth > max.width) OrElse (tileHeight < min.height) OrElse (tileHeight > max.height) Then
						ok = False
						Exit For
					End If
				Next i
				If Not ok Then Throw New System.ArgumentException("Illegal tile size!")
			End If

			Me.tilingSet = True
			Me.tileWidth = tileWidth
			Me.tileHeight = tileHeight
			Me.tileGridXOffset = tileGridXOffset
			Me.tileGridYOffset = tileGridYOffset
		End Sub

		''' <summary>
		''' Removes any previous tile grid parameters specified by calls to
		''' <code>setTiling</code>.
		''' 
		''' <p> The default implementation sets the instance variables
		''' <code>tileWidth</code>, <code>tileHeight</code>,
		''' <code>tileGridXOffset</code>, and
		''' <code>tileGridYOffset</code> to <code>0</code>.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>.
		''' </exception>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		Public Overridable Sub unsetTiling()
			If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
			If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
			Me.tilingSet = False
			Me.tileWidth = 0
			Me.tileHeight = 0
			Me.tileGridXOffset = 0
			Me.tileGridYOffset = 0
		End Sub

		''' <summary>
		''' Returns the width of each tile in an image as it will be
		''' written to the output stream.  If tiling parameters have not
		''' been set, an <code>IllegalStateException</code> is thrown.
		''' </summary>
		''' <returns> the tile width to be used for encoding.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the tiling parameters have
		''' not been set.
		''' </exception>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		''' <seealso cref= #getTileHeight() </seealso>
		Public Overridable Property tileWidth As Integer
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
				If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
				If Not tilingSet Then Throw New IllegalStateException("Tiling parameters not set!")
				Return tileWidth
			End Get
		End Property

		''' <summary>
		''' Returns the height of each tile in an image as it will be written to
		''' the output stream.  If tiling parameters have not
		''' been set, an <code>IllegalStateException</code> is thrown.
		''' </summary>
		''' <returns> the tile height to be used for encoding.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the tiling parameters have
		''' not been set.
		''' </exception>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		''' <seealso cref= #getTileWidth() </seealso>
		Public Overridable Property tileHeight As Integer
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
				If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
				If Not tilingSet Then Throw New IllegalStateException("Tiling parameters not set!")
				Return tileHeight
			End Get
		End Property

		''' <summary>
		''' Returns the horizontal tile grid offset of an image as it will
		''' be written to the output stream.  If tiling parameters have not
		''' been set, an <code>IllegalStateException</code> is thrown.
		''' </summary>
		''' <returns> the tile grid X offset to be used for encoding.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the tiling parameters have
		''' not been set.
		''' </exception>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		''' <seealso cref= #getTileGridYOffset() </seealso>
		Public Overridable Property tileGridXOffset As Integer
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
				If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
				If Not tilingSet Then Throw New IllegalStateException("Tiling parameters not set!")
				Return tileGridXOffset
			End Get
		End Property

		''' <summary>
		''' Returns the vertical tile grid offset of an image as it will
		''' be written to the output stream.  If tiling parameters have not
		''' been set, an <code>IllegalStateException</code> is thrown.
		''' </summary>
		''' <returns> the tile grid Y offset to be used for encoding.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support tiling. </exception>
		''' <exception cref="IllegalStateException"> if the tiling mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the tiling parameters have
		''' not been set.
		''' </exception>
		''' <seealso cref= #setTiling(int, int, int, int) </seealso>
		''' <seealso cref= #getTileGridXOffset() </seealso>
		Public Overridable Property tileGridYOffset As Integer
			Get
				If Not canWriteTiles() Then Throw New System.NotSupportedException("Tiling not supported!")
				If tilingMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Tiling mode not MODE_EXPLICIT!")
				If Not tilingSet Then Throw New IllegalStateException("Tiling parameters not set!")
				Return tileGridYOffset
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the writer can write out images
		''' as a series of passes of progressively increasing quality.
		''' </summary>
		''' <returns> <code>true</code> if the writer supports progressive
		''' encoding.
		''' </returns>
		''' <seealso cref= #setProgressiveMode </seealso>
		''' <seealso cref= #getProgressiveMode </seealso>
		Public Overridable Function canWriteProgressive() As Boolean
			Return ___canWriteProgressive
		End Function

		''' <summary>
		''' Specifies that the writer is to write the image out in a
		''' progressive mode such that the stream will contain a series of
		''' scans of increasing quality.  If progressive encoding is not
		''' supported, an <code>UnsupportedOperationException</code> will
		''' be thrown.
		''' 
		''' <p>  The mode argument determines how
		''' the progression parameters are chosen, and must be either
		''' <code>MODE_DISABLED</code>,
		''' <code>MODE_COPY_FROM_METADATA</code>, or
		''' <code>MODE_DEFAULT</code>.  Otherwise an
		''' <code>IllegalArgumentException</code> is thrown.
		''' 
		''' <p> The modes are interpreted as follows:
		''' 
		''' <ul>
		'''   <li><code>MODE_DISABLED</code> - No progression.  Use this to
		'''   turn off progression.
		''' 
		'''   <li><code>MODE_COPY_FROM_METADATA</code> - The output image
		'''   will use whatever progression parameters are found in the
		'''   metadata objects passed into the writer.
		''' 
		'''   <li><code>MODE_DEFAULT</code> - The image will be written
		'''   progressively, with parameters chosen by the writer.
		''' </ul>
		''' 
		''' <p> The default is <code>MODE_COPY_FROM_METADATA</code>.
		''' </summary>
		''' <param name="mode"> The mode for setting progression in the output
		''' stream.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support progressive encoding. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>mode</code> is not
		''' one of the modes listed above.
		''' </exception>
		''' <seealso cref= #getProgressiveMode </seealso>
		Public Overridable Property progressiveMode As Integer
			Set(ByVal mode As Integer)
				If Not canWriteProgressive() Then Throw New System.NotSupportedException("Progressive output not supported")
				If mode < MODE_DISABLED OrElse mode > MAX_MODE Then Throw New System.ArgumentException("Illegal value for mode!")
				If mode = MODE_EXPLICIT Then Throw New System.ArgumentException("MODE_EXPLICIT not supported for progressive output")
				Me.progressiveMode = mode
			End Set
			Get
				If Not canWriteProgressive() Then Throw New System.NotSupportedException("Progressive output not supported")
				Return progressiveMode
			End Get
		End Property


		''' <summary>
		''' Returns <code>true</code> if this writer supports compression.
		''' </summary>
		''' <returns> <code>true</code> if the writer supports compression. </returns>
		Public Overridable Function canWriteCompressed() As Boolean
			Return ___canWriteCompressed
		End Function

		''' <summary>
		''' Specifies whether compression is to be performed, and if so how
		''' compression parameters are to be determined.  The <code>mode</code>
		''' argument must be one of the four modes, interpreted as follows:
		''' 
		''' <ul>
		'''   <li><code>MODE_DISABLED</code> - If the mode is set to
		'''   <code>MODE_DISABLED</code>, methods that query or modify the
		'''   compression type or parameters will throw an
		'''   <code>IllegalStateException</code> (if compression is
		'''   normally supported by the plug-in). Some writers, such as JPEG,
		'''   do not normally offer uncompressed output. In this case, attempting
		'''   to set the mode to <code>MODE_DISABLED</code> will throw an
		'''   <code>UnsupportedOperationException</code> and the mode will not be
		'''   changed.
		''' 
		'''   <li><code>MODE_EXPLICIT</code> - Compress using the
		'''   compression type and quality settings specified in this
		'''   <code>ImageWriteParam</code>.  Any previously set compression
		'''   parameters are discarded.
		''' 
		'''   <li><code>MODE_COPY_FROM_METADATA</code> - Use whatever
		'''   compression parameters are specified in metadata objects
		'''   passed in to the writer.
		''' 
		'''   <li><code>MODE_DEFAULT</code> - Use default compression
		'''   parameters.
		''' </ul>
		''' 
		''' <p> The default is <code>MODE_COPY_FROM_METADATA</code>.
		''' </summary>
		''' <param name="mode"> The mode for setting compression in the output
		''' stream.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression, or does not support the requested mode. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>mode</code> is not
		''' one of the modes listed above.
		''' </exception>
		''' <seealso cref= #getCompressionMode </seealso>
		Public Overridable Property compressionMode As Integer
			Set(ByVal mode As Integer)
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If mode < MODE_DISABLED OrElse mode > MAX_MODE Then Throw New System.ArgumentException("Illegal value for mode!")
				Me.compressionMode = mode
				If mode = MODE_EXPLICIT Then unsetCompression()
			End Set
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				Return compressionMode
			End Get
		End Property


		''' <summary>
		''' Returns a list of available compression types, as an array or
		''' <code>String</code>s, or <code>null</code> if a compression
		''' type may not be chosen using these interfaces.  The array
		''' returned is a copy.
		''' 
		''' <p> If the writer only offers a single, mandatory form of
		''' compression, it is not necessary to provide any named
		''' compression types.  Named compression types should only be
		''' used where the user is able to make a meaningful choice
		''' between different schemes.
		''' 
		''' <p> The default implementation checks if compression is
		''' supported and throws an
		''' <code>UnsupportedOperationException</code> if not.  Otherwise,
		''' it returns a clone of the <code>compressionTypes</code>
		''' instance variable if it is non-<code>null</code>, or else
		''' returns <code>null</code>.
		''' </summary>
		''' <returns> an array of <code>String</code>s containing the
		''' (non-localized) names of available compression types, or
		''' <code>null</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		Public Overridable Property compressionTypes As String()
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported")
				If compressionTypes Is Nothing Then Return Nothing
				Return CType(compressionTypes.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Sets the compression type to one of the values indicated by
		''' <code>getCompressionTypes</code>.  If a value of
		''' <code>null</code> is passed in, any previous setting is
		''' removed.
		''' 
		''' <p> The default implementation checks whether compression is
		''' supported and the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, it calls
		''' <code>getCompressionTypes</code> and checks if
		''' <code>compressionType</code> is one of the legal values.  If it
		''' is, the <code>compressionType</code> instance variable is set.
		''' If <code>compressionType</code> is <code>null</code>, the
		''' instance variable is set without performing any checking.
		''' </summary>
		''' <param name="compressionType"> one of the <code>String</code>s returned
		''' by <code>getCompressionTypes</code>, or <code>null</code> to
		''' remove any previous setting.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="UnsupportedOperationException"> if there are no
		''' settable compression types. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>compressionType</code> is non-<code>null</code> but is not
		''' one of the values returned by <code>getCompressionTypes</code>.
		''' </exception>
		''' <seealso cref= #getCompressionTypes </seealso>
		''' <seealso cref= #getCompressionType </seealso>
		''' <seealso cref= #unsetCompression </seealso>
		Public Overridable Property compressionType As String
			Set(ByVal compressionType As String)
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				Dim legalTypes As String() = compressionTypes
				If legalTypes Is Nothing Then Throw New System.NotSupportedException("No settable compression types")
				If compressionType IsNot Nothing Then
					Dim found As Boolean = False
					If legalTypes IsNot Nothing Then
						For i As Integer = 0 To legalTypes.Length - 1
							If compressionType.Equals(legalTypes(i)) Then
								found = True
								Exit For
							End If
						Next i
					End If
					If Not found Then Throw New System.ArgumentException("Unknown compression type!")
				End If
				Me.compressionType = compressionType
			End Set
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				Return compressionType
			End Get
		End Property


		''' <summary>
		''' Removes any previous compression type and quality settings.
		''' 
		''' <p> The default implementation sets the instance variable
		''' <code>compressionType</code> to <code>null</code>, and the
		''' instance variable <code>compressionQuality</code> to
		''' <code>1.0F</code>.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the plug-in does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>.
		''' </exception>
		''' <seealso cref= #setCompressionType </seealso>
		''' <seealso cref= #setCompressionQuality </seealso>
		Public Overridable Sub unsetCompression()
			If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported")
			If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
			Me.compressionType = Nothing
			Me.compressionQuality = 1.0F
		End Sub

		''' <summary>
		''' Returns a localized version of the name of the current
		''' compression type, using the <code>Locale</code> returned by
		''' <code>getLocale</code>.
		''' 
		''' <p> The default implementation checks whether compression is
		''' supported and the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>compressionType</code> is <code>non-null</code> the value
		''' of <code>getCompressionType</code> is returned as a
		''' convenience.
		''' </summary>
		''' <returns> a <code>String</code> containing a localized version of
		''' the name of the current compression type.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if no compression type is set. </exception>
		Public Overridable Property localizedCompressionTypeName As String
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If compressionType Is Nothing Then Throw New IllegalStateException("No compression type set!")
				Return compressionType
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the current compression type
		''' provides lossless compression.  If a plug-in provides only
		''' one mandatory compression type, then this method may be
		''' called without calling <code>setCompressionType</code> first.
		''' 
		''' <p> If there are multiple compression types but none has
		''' been set, an <code>IllegalStateException</code> is thrown.
		''' 
		''' <p> The default implementation checks whether compression is
		''' supported and the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>getCompressionTypes()</code> is <code>null</code> or
		''' <code>getCompressionType()</code> is non-<code>null</code>
		''' <code>true</code> is returned as a convenience.
		''' </summary>
		''' <returns> <code>true</code> if the current compression type is
		''' lossless.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the set of legal
		''' compression types is non-<code>null</code> and the current
		''' compression type is <code>null</code>. </exception>
		Public Overridable Property compressionLossless As Boolean
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If (compressionTypes IsNot Nothing) AndAlso (compressionType Is Nothing) Then Throw New IllegalStateException("No compression type set!")
				Return True
			End Get
		End Property

		''' <summary>
		''' Sets the compression quality to a value between <code>0</code>
		''' and <code>1</code>.  Only a single compression quality setting
		''' is supported by default; writers can provide extended versions
		''' of <code>ImageWriteParam</code> that offer more control.  For
		''' lossy compression schemes, the compression quality should
		''' control the tradeoff between file size and image quality (for
		''' example, by choosing quantization tables when writing JPEG
		''' images).  For lossless schemes, the compression quality may be
		''' used to control the tradeoff between file size and time taken
		''' to perform the compression (for example, by optimizing row
		''' filters and setting the ZLIB compression level when writing
		''' PNG images).
		''' 
		''' <p> A compression quality setting of 0.0 is most generically
		''' interpreted as "high compression is important," while a setting of
		''' 1.0 is most generically interpreted as "high image quality is
		''' important."
		''' 
		''' <p> If there are multiple compression types but none has been
		''' set, an <code>IllegalStateException</code> is thrown.
		''' 
		''' <p> The default implementation checks that compression is
		''' supported, and that the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>getCompressionTypes()</code> returns <code>null</code> or
		''' <code>compressionType</code> is non-<code>null</code> it sets
		''' the <code>compressionQuality</code> instance variable.
		''' </summary>
		''' <param name="quality"> a <code>float</code> between <code>0</code>and
		''' <code>1</code> indicating the desired quality level.
		''' </param>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the set of legal
		''' compression types is non-<code>null</code> and the current
		''' compression type is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>quality</code> is
		''' not between <code>0</code>and <code>1</code>, inclusive.
		''' </exception>
		''' <seealso cref= #getCompressionQuality </seealso>
		Public Overridable Property compressionQuality As Single
			Set(ByVal quality As Single)
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If compressionTypes IsNot Nothing AndAlso compressionType Is Nothing Then Throw New IllegalStateException("No compression type set!")
				If quality < 0.0F OrElse quality > 1.0F Then Throw New System.ArgumentException("Quality out-of-bounds!")
				Me.compressionQuality = quality
			End Set
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If (compressionTypes IsNot Nothing) AndAlso (compressionType Is Nothing) Then Throw New IllegalStateException("No compression type set!")
				Return compressionQuality
			End Get
		End Property



		''' <summary>
		''' Returns a <code>float</code> indicating an estimate of the
		''' number of bits of output data for each bit of input image data
		''' at the given quality level.  The value will typically lie
		''' between <code>0</code> and <code>1</code>, with smaller values
		''' indicating more compression.  A special value of
		''' <code>-1.0F</code> is used to indicate that no estimate is
		''' available.
		''' 
		''' <p> If there are multiple compression types but none has been set,
		''' an <code>IllegalStateException</code> is thrown.
		''' 
		''' <p> The default implementation checks that compression is
		''' supported and the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>getCompressionTypes()</code> is <code>null</code> or
		''' <code>getCompressionType()</code> is non-<code>null</code>, and
		''' <code>quality</code> is within bounds, it returns
		''' <code>-1.0</code>.
		''' </summary>
		''' <param name="quality"> the quality setting whose bit rate is to be
		''' queried.
		''' </param>
		''' <returns> an estimate of the compressed bit rate, or
		''' <code>-1.0F</code> if no estimate is available.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the set of legal
		''' compression types is non-<code>null</code> and the current
		''' compression type is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>quality</code> is
		''' not between <code>0</code>and <code>1</code>, inclusive. </exception>
		Public Overridable Function getBitRate(ByVal quality As Single) As Single
			If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
			If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
			If (compressionTypes IsNot Nothing) AndAlso (compressionType Is Nothing) Then Throw New IllegalStateException("No compression type set!")
			If quality < 0.0F OrElse quality > 1.0F Then Throw New System.ArgumentException("Quality out-of-bounds!")
			Return -1.0F
		End Function

		''' <summary>
		''' Returns an array of <code>String</code>s that may be used along
		''' with <code>getCompressionQualityValues</code> as part of a user
		''' interface for setting or displaying the compression quality
		''' level.  The <code>String</code> with index <code>i</code>
		''' provides a description of the range of quality levels between
		''' <code>getCompressionQualityValues[i]</code> and
		''' <code>getCompressionQualityValues[i + 1]</code>.  Note that the
		''' length of the array returned from
		''' <code>getCompressionQualityValues</code> will always be one
		''' greater than that returned from
		''' <code>getCompressionQualityDescriptions</code>.
		''' 
		''' <p> As an example, the strings "Good", "Better", and "Best"
		''' could be associated with the ranges <code>[0, .33)</code>,
		''' <code>[.33, .66)</code>, and <code>[.66, 1.0]</code>.  In this
		''' case, <code>getCompressionQualityDescriptions</code> would
		''' return <code>{ "Good", "Better", "Best" }</code> and
		''' <code>getCompressionQualityValues</code> would return
		''' <code>{ 0.0F, .33F, .66F, 1.0F }</code>.
		''' 
		''' <p> If no descriptions are available, <code>null</code> is
		''' returned.  If <code>null</code> is returned from
		''' <code>getCompressionQualityValues</code>, this method must also
		''' return <code>null</code>.
		''' 
		''' <p> The descriptions should be localized for the
		''' <code>Locale</code> returned by <code>getLocale</code>, if it
		''' is non-<code>null</code>.
		''' 
		''' <p> If there are multiple compression types but none has been set,
		''' an <code>IllegalStateException</code> is thrown.
		''' 
		''' <p> The default implementation checks that compression is
		''' supported and that the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>getCompressionTypes()</code> is <code>null</code> or
		''' <code>getCompressionType()</code> is non-<code>null</code>, it
		''' returns <code>null</code>.
		''' </summary>
		''' <returns> an array of <code>String</code>s containing localized
		''' descriptions of the compression quality levels.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the set of legal
		''' compression types is non-<code>null</code> and the current
		''' compression type is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getCompressionQualityValues </seealso>
		Public Overridable Property compressionQualityDescriptions As String()
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If (compressionTypes IsNot Nothing) AndAlso (compressionType Is Nothing) Then Throw New IllegalStateException("No compression type set!")
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>float</code>s that may be used along
		''' with <code>getCompressionQualityDescriptions</code> as part of a user
		''' interface for setting or displaying the compression quality
		''' level.  See {@link #getCompressionQualityDescriptions
		''' getCompressionQualityDescriptions} for more information.
		''' 
		''' <p> If no descriptions are available, <code>null</code> is
		''' returned.  If <code>null</code> is returned from
		''' <code>getCompressionQualityDescriptions</code>, this method
		''' must also return <code>null</code>.
		''' 
		''' <p> If there are multiple compression types but none has been set,
		''' an <code>IllegalStateException</code> is thrown.
		''' 
		''' <p> The default implementation checks that compression is
		''' supported and that the compression mode is
		''' <code>MODE_EXPLICIT</code>.  If so, if
		''' <code>getCompressionTypes()</code> is <code>null</code> or
		''' <code>getCompressionType()</code> is non-<code>null</code>, it
		''' returns <code>null</code>.
		''' </summary>
		''' <returns> an array of <code>float</code>s indicating the
		''' boundaries between the compression quality levels as described
		''' by the <code>String</code>s from
		''' <code>getCompressionQualityDescriptions</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if the writer does not
		''' support compression. </exception>
		''' <exception cref="IllegalStateException"> if the compression mode is not
		''' <code>MODE_EXPLICIT</code>. </exception>
		''' <exception cref="IllegalStateException"> if the set of legal
		''' compression types is non-<code>null</code> and the current
		''' compression type is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getCompressionQualityDescriptions </seealso>
		Public Overridable Property compressionQualityValues As Single()
			Get
				If Not canWriteCompressed() Then Throw New System.NotSupportedException("Compression not supported.")
				If compressionMode <> MODE_EXPLICIT Then Throw New IllegalStateException("Compression mode not MODE_EXPLICIT!")
				If (compressionTypes IsNot Nothing) AndAlso (compressionType Is Nothing) Then Throw New IllegalStateException("No compression type set!")
				Return Nothing
			End Get
		End Property
	End Class

End Namespace