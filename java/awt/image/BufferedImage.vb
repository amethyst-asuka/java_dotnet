Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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



	''' 
	''' <summary>
	''' The <code>BufferedImage</code> subclass describes an {@link
	''' java.awt.Image Image} with an accessible buffer of image data.
	''' A <code>BufferedImage</code> is comprised of a <seealso cref="ColorModel"/> and a
	''' <seealso cref="Raster"/> of image data.
	''' The number and types of bands in the <seealso cref="SampleModel"/> of the
	''' <code>Raster</code> must match the number and types required by the
	''' <code>ColorModel</code> to represent its color and alpha components.
	''' All <code>BufferedImage</code> objects have an upper left corner
	''' coordinate of (0,&nbsp;0).  Any <code>Raster</code> used to construct a
	''' <code>BufferedImage</code> must therefore have minX=0 and minY=0.
	''' 
	''' <p>
	''' This class relies on the data fetching and setting methods
	''' of <code>Raster</code>,
	''' and on the color characterization methods of <code>ColorModel</code>.
	''' </summary>
	''' <seealso cref= ColorModel </seealso>
	''' <seealso cref= Raster </seealso>
	''' <seealso cref= WritableRaster </seealso>
	Public Class BufferedImage
		Inherits java.awt.Image
		Implements WritableRenderedImage, java.awt.Transparency

		Private imageType As Integer = TYPE_CUSTOM
		Private colorModel As ColorModel
		Private ReadOnly raster As WritableRaster
		Private osis As sun.awt.image.OffScreenImageSource
		Private properties As Dictionary(Of String, Object)

		''' <summary>
		''' Image Type Constants
		''' </summary>

		''' <summary>
		''' Image type is not recognized so it must be a customized
		''' image.  This type is only used as a return value for the getType()
		''' method.
		''' </summary>
		Public Const TYPE_CUSTOM As Integer = 0

		''' <summary>
		''' Represents an image with 8-bit RGB color components packed into
		''' integer pixels.  The image has a <seealso cref="DirectColorModel"/> without
		''' alpha.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_INT_RGB As Integer = 1

		''' <summary>
		''' Represents an image with 8-bit RGBA color components packed into
		''' integer pixels.  The image has a <code>DirectColorModel</code>
		''' with alpha. The color data in this image is considered not to be
		''' premultiplied with alpha.  When this type is used as the
		''' <code>imageType</code> argument to a <code>BufferedImage</code>
		''' constructor, the created image is consistent with images
		''' created in the JDK1.1 and earlier releases.
		''' </summary>
		Public Const TYPE_INT_ARGB As Integer = 2

		''' <summary>
		''' Represents an image with 8-bit RGBA color components packed into
		''' integer pixels.  The image has a <code>DirectColorModel</code>
		''' with alpha.  The color data in this image is considered to be
		''' premultiplied with alpha.
		''' </summary>
		Public Const TYPE_INT_ARGB_PRE As Integer = 3

		''' <summary>
		''' Represents an image with 8-bit RGB color components, corresponding
		''' to a Windows- or Solaris- style BGR color model, with the colors
		''' Blue, Green, and Red packed into integer pixels.  There is no alpha.
		''' The image has a <seealso cref="DirectColorModel"/>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_INT_BGR As Integer = 4

		''' <summary>
		''' Represents an image with 8-bit RGB color components, corresponding
		''' to a Windows-style BGR color model) with the colors Blue, Green,
		''' and Red stored in 3 bytes.  There is no alpha.  The image has a
		''' <code>ComponentColorModel</code>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_3BYTE_BGR As Integer = 5

		''' <summary>
		''' Represents an image with 8-bit RGBA color components with the colors
		''' Blue, Green, and Red stored in 3 bytes and 1 byte of alpha.  The
		''' image has a <code>ComponentColorModel</code> with alpha.  The
		''' color data in this image is considered not to be premultiplied with
		''' alpha.  The byte data is interleaved in a single
		''' byte array in the order A, B, G, R
		''' from lower to higher byte addresses within each pixel.
		''' </summary>
		Public Const TYPE_4BYTE_ABGR As Integer = 6

		''' <summary>
		''' Represents an image with 8-bit RGBA color components with the colors
		''' Blue, Green, and Red stored in 3 bytes and 1 byte of alpha.  The
		''' image has a <code>ComponentColorModel</code> with alpha. The color
		''' data in this image is considered to be premultiplied with alpha.
		''' The byte data is interleaved in a single byte array in the order
		''' A, B, G, R from lower to higher byte addresses within each pixel.
		''' </summary>
		Public Const TYPE_4BYTE_ABGR_PRE As Integer = 7

		''' <summary>
		''' Represents an image with 5-6-5 RGB color components (5-bits red,
		''' 6-bits green, 5-bits blue) with no alpha.  This image has
		''' a <code>DirectColorModel</code>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_USHORT_565_RGB As Integer = 8

		''' <summary>
		''' Represents an image with 5-5-5 RGB color components (5-bits red,
		''' 5-bits green, 5-bits blue) with no alpha.  This image has
		''' a <code>DirectColorModel</code>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_USHORT_555_RGB As Integer = 9

		''' <summary>
		''' Represents a unsigned byte grayscale image, non-indexed.  This
		''' image has a <code>ComponentColorModel</code> with a CS_GRAY
		''' <seealso cref="ColorSpace"/>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_BYTE_GRAY As Integer = 10

		''' <summary>
		''' Represents an unsigned short grayscale image, non-indexed).  This
		''' image has a <code>ComponentColorModel</code> with a CS_GRAY
		''' <code>ColorSpace</code>.
		''' When data with non-opaque alpha is stored
		''' in an image of this type,
		''' the color data must be adjusted to a non-premultiplied form
		''' and the alpha discarded,
		''' as described in the
		''' <seealso cref="java.awt.AlphaComposite"/> documentation.
		''' </summary>
		Public Const TYPE_USHORT_GRAY As Integer = 11

		''' <summary>
		''' Represents an opaque byte-packed 1, 2, or 4 bit image.  The
		''' image has an <seealso cref="IndexColorModel"/> without alpha.  When this
		''' type is used as the <code>imageType</code> argument to the
		''' <code>BufferedImage</code> constructor that takes an
		''' <code>imageType</code> argument but no <code>ColorModel</code>
		''' argument, a 1-bit image is created with an
		''' <code>IndexColorModel</code> with two colors in the default
		''' sRGB <code>ColorSpace</code>: {0,&nbsp;0,&nbsp;0} and
		''' {255,&nbsp;255,&nbsp;255}.
		''' 
		''' <p> Images with 2 or 4 bits per pixel may be constructed via
		''' the <code>BufferedImage</code> constructor that takes a
		''' <code>ColorModel</code> argument by supplying a
		''' <code>ColorModel</code> with an appropriate map size.
		''' 
		''' <p> Images with 8 bits per pixel should use the image types
		''' <code>TYPE_BYTE_INDEXED</code> or <code>TYPE_BYTE_GRAY</code>
		''' depending on their <code>ColorModel</code>.
		''' 
		''' <p> When color data is stored in an image of this type,
		''' the closest color in the colormap is determined
		''' by the <code>IndexColorModel</code> and the resulting index is stored.
		''' Approximation and loss of alpha or color components
		''' can result, depending on the colors in the
		''' <code>IndexColorModel</code> colormap.
		''' </summary>
		Public Const TYPE_BYTE_BINARY As Integer = 12

		''' <summary>
		''' Represents an indexed byte image.  When this type is used as the
		''' <code>imageType</code> argument to the <code>BufferedImage</code>
		''' constructor that takes an <code>imageType</code> argument
		''' but no <code>ColorModel</code> argument, an
		''' <code>IndexColorModel</code> is created with
		''' a 256-color 6/6/6 color cube palette with the rest of the colors
		''' from 216-255 populated by grayscale values in the
		''' default sRGB ColorSpace.
		''' 
		''' <p> When color data is stored in an image of this type,
		''' the closest color in the colormap is determined
		''' by the <code>IndexColorModel</code> and the resulting index is stored.
		''' Approximation and loss of alpha or color components
		''' can result, depending on the colors in the
		''' <code>IndexColorModel</code> colormap.
		''' </summary>
		Public Const TYPE_BYTE_INDEXED As Integer = 13

		Private Const DCM_RED_MASK As Integer = &Hff0000
		Private Const DCM_GREEN_MASK As Integer = &Hff00
		Private Const DCM_BLUE_MASK As Integer = &Hff
		Private Const DCM_ALPHA_MASK As Integer = &Hff000000L
		Private Const DCM_565_RED_MASK As Integer = &Hf800
		Private Const DCM_565_GRN_MASK As Integer = &H7E0
		Private Const DCM_565_BLU_MASK As Integer = &H1F
		Private Const DCM_555_RED_MASK As Integer = &H7C00
		Private Const DCM_555_GRN_MASK As Integer = &H3E0
		Private Const DCM_555_BLU_MASK As Integer = &H1F
		Private Const DCM_BGR_RED_MASK As Integer = &Hff
		Private Const DCM_BGR_GRN_MASK As Integer = &Hff00
		Private Const DCM_BGR_BLU_MASK As Integer = &Hff0000


'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Constructs a <code>BufferedImage</code> of one of the predefined
		''' image types.  The <code>ColorSpace</code> for the image is the
		''' default sRGB space. </summary>
		''' <param name="width">     width of the created image </param>
		''' <param name="height">    height of the created image </param>
		''' <param name="imageType"> type of the created image </param>
		''' <seealso cref= ColorSpace </seealso>
		''' <seealso cref= #TYPE_INT_RGB </seealso>
		''' <seealso cref= #TYPE_INT_ARGB </seealso>
		''' <seealso cref= #TYPE_INT_ARGB_PRE </seealso>
		''' <seealso cref= #TYPE_INT_BGR </seealso>
		''' <seealso cref= #TYPE_3BYTE_BGR </seealso>
		''' <seealso cref= #TYPE_4BYTE_ABGR </seealso>
		''' <seealso cref= #TYPE_4BYTE_ABGR_PRE </seealso>
		''' <seealso cref= #TYPE_BYTE_GRAY </seealso>
		''' <seealso cref= #TYPE_USHORT_GRAY </seealso>
		''' <seealso cref= #TYPE_BYTE_BINARY </seealso>
		''' <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		''' <seealso cref= #TYPE_USHORT_565_RGB </seealso>
		''' <seealso cref= #TYPE_USHORT_555_RGB </seealso>
		Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal imageType As Integer)
			Select Case imageType
			Case TYPE_INT_RGB
					colorModel = New DirectColorModel(24, &Hff0000, &Hff00, &Hff, &H0) ' Alpha -  Blue -  Green -  Red
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_INT_ARGB
					colorModel = ColorModel.rGBdefault

					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_INT_ARGB_PRE
					colorModel = New DirectColorModel(java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), 32, &Hff0000, &Hff00, &Hff, &Hff000000L, True, DataBuffer.TYPE_INT) ' Alpha Premultiplied -  Alpha -  Blue -  Green -  Red
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_INT_BGR
					colorModel = New DirectColorModel(24, &Hff, &Hff00, &Hff0000) ' Blue -  Green -  Red
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_3BYTE_BGR
					Dim cs As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB)
					Dim nBits As Integer() = {8, 8, 8}
					Dim bOffs As Integer() = {2, 1, 0}
					colorModel = New ComponentColorModel(cs, nBits, False, False, java.awt.Transparency.OPAQUE, DataBuffer.TYPE_BYTE)
					raster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width*3, 3, bOffs, Nothing)

			Case TYPE_4BYTE_ABGR
					Dim cs As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB)
					Dim nBits As Integer() = {8, 8, 8, 8}
					Dim bOffs As Integer() = {3, 2, 1, 0}
					colorModel = New ComponentColorModel(cs, nBits, True, False, java.awt.Transparency.TRANSLUCENT, DataBuffer.TYPE_BYTE)
					raster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width*4, 4, bOffs, Nothing)

			Case TYPE_4BYTE_ABGR_PRE
					Dim cs As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB)
					Dim nBits As Integer() = {8, 8, 8, 8}
					Dim bOffs As Integer() = {3, 2, 1, 0}
					colorModel = New ComponentColorModel(cs, nBits, True, True, java.awt.Transparency.TRANSLUCENT, DataBuffer.TYPE_BYTE)
					raster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, width*4, 4, bOffs, Nothing)

			Case TYPE_BYTE_GRAY
					Dim cs As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_GRAY)
					Dim nBits As Integer() = {8}
					colorModel = New ComponentColorModel(cs, nBits, False, True, java.awt.Transparency.OPAQUE, DataBuffer.TYPE_BYTE)
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_USHORT_GRAY
					Dim cs As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_GRAY)
					Dim nBits As Integer() = {16}
					colorModel = New ComponentColorModel(cs, nBits, False, True, java.awt.Transparency.OPAQUE, DataBuffer.TYPE_USHORT)
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_BYTE_BINARY
					Dim arr As SByte() = {CByte(0), CByte(&Hff)}

					colorModel = New IndexColorModel(1, 2, arr, arr, arr)
					raster = Raster.createPackedRaster(DataBuffer.TYPE_BYTE, width, height, 1, 1, Nothing)

			Case TYPE_BYTE_INDEXED
					' Create a 6x6x6 color cube
					Dim cmap As Integer() = New Integer(255){}
					Dim i As Integer=0
					For r As Integer = 0 To 255 Step 51
						For g As Integer = 0 To 255 Step 51
							For b As Integer = 0 To 255 Step 51
								cmap(i) = (r<<16) Or (g<<8) Or b
								i += 1
							Next b
						Next g
					Next r
					' And populate the rest of the cmap with gray values
					Dim grayIncr As Integer = 256\(256-i)

					' The gray ramp will be between 18 and 252
					Dim gray As Integer = grayIncr*3
					Do While i < 256
						cmap(i) = (gray<<16) Or (gray<<8) Or gray
						gray += grayIncr
						i += 1
					Loop

					colorModel = New IndexColorModel(8, 256, cmap, 0, False, -1, DataBuffer.TYPE_BYTE)
					raster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, 1, Nothing)

			Case TYPE_USHORT_565_RGB
					colorModel = New DirectColorModel(16, DCM_565_RED_MASK, DCM_565_GRN_MASK, DCM_565_BLU_MASK)
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case TYPE_USHORT_555_RGB
					colorModel = New DirectColorModel(15, DCM_555_RED_MASK, DCM_555_GRN_MASK, DCM_555_BLU_MASK)
					raster = colorModel.createCompatibleWritableRaster(width, height)

			Case Else
				Throw New IllegalArgumentException("Unknown image type " & imageType)
			End Select

			Me.imageType = imageType
		End Sub

		''' <summary>
		''' Constructs a <code>BufferedImage</code> of one of the predefined
		''' image types:
		''' TYPE_BYTE_BINARY or TYPE_BYTE_INDEXED.
		''' 
		''' <p> If the image type is TYPE_BYTE_BINARY, the number of
		''' entries in the color model is used to determine whether the
		''' image should have 1, 2, or 4 bits per pixel.  If the color model
		''' has 1 or 2 entries, the image will have 1 bit per pixel.  If it
		''' has 3 or 4 entries, the image with have 2 bits per pixel.  If
		''' it has between 5 and 16 entries, the image will have 4 bits per
		''' pixel.  Otherwise, an IllegalArgumentException will be thrown.
		''' </summary>
		''' <param name="width">     width of the created image </param>
		''' <param name="height">    height of the created image </param>
		''' <param name="imageType"> type of the created image </param>
		''' <param name="cm">        <code>IndexColorModel</code> of the created image </param>
		''' <exception cref="IllegalArgumentException">   if the imageType is not
		''' TYPE_BYTE_BINARY or TYPE_BYTE_INDEXED or if the imageType is
		''' TYPE_BYTE_BINARY and the color map has more than 16 entries. </exception>
		''' <seealso cref= #TYPE_BYTE_BINARY </seealso>
		''' <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal imageType As Integer, ByVal cm As IndexColorModel)
			If cm.hasAlpha() AndAlso cm.alphaPremultiplied Then Throw New IllegalArgumentException("This image types do not have " & "premultiplied alpha.")

			Select Case imageType
			Case TYPE_BYTE_BINARY
				Dim bits As Integer ' Will be set below
				Dim mapSize As Integer = cm.mapSize
				If mapSize <= 2 Then
					bits = 1
				ElseIf mapSize <= 4 Then
					bits = 2
				ElseIf mapSize <= 16 Then
					bits = 4
				Else
					Throw New IllegalArgumentException("Color map for TYPE_BYTE_BINARY " & "must have no more than 16 entries")
				End If
				raster = Raster.createPackedRaster(DataBuffer.TYPE_BYTE, width, height, 1, bits, Nothing)

			Case TYPE_BYTE_INDEXED
				raster = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, width, height, 1, Nothing)
			Case Else
				Throw New IllegalArgumentException("Invalid image type (" & imageType & ").  Image type must" & " be either TYPE_BYTE_BINARY or " & " TYPE_BYTE_INDEXED")
			End Select

			If Not cm.isCompatibleRaster(raster) Then Throw New IllegalArgumentException("Incompatible image type and IndexColorModel")

			colorModel = cm
			Me.imageType = imageType
		End Sub

		''' <summary>
		''' Constructs a new <code>BufferedImage</code> with a specified
		''' <code>ColorModel</code> and <code>Raster</code>.  If the number and
		''' types of bands in the <code>SampleModel</code> of the
		''' <code>Raster</code> do not match the number and types required by
		''' the <code>ColorModel</code> to represent its color and alpha
		''' components, a <seealso cref="RasterFormatException"/> is thrown.  This
		''' method can multiply or divide the color <code>Raster</code> data by
		''' alpha to match the <code>alphaPremultiplied</code> state
		''' in the <code>ColorModel</code>.  Properties for this
		''' <code>BufferedImage</code> can be established by passing
		''' in a <seealso cref="Hashtable"/> of <code>String</code>/<code>Object</code>
		''' pairs. </summary>
		''' <param name="cm"> <code>ColorModel</code> for the new image </param>
		''' <param name="raster">     <code>Raster</code> for the image data </param>
		''' <param name="isRasterPremultiplied">   if <code>true</code>, the data in
		'''                  the raster has been premultiplied with alpha. </param>
		''' <param name="properties"> <code>Hashtable</code> of
		'''                  <code>String</code>/<code>Object</code> pairs. </param>
		''' <exception cref="RasterFormatException"> if the number and
		''' types of bands in the <code>SampleModel</code> of the
		''' <code>Raster</code> do not match the number and types required by
		''' the <code>ColorModel</code> to represent its color and alpha
		''' components. </exception>
		''' <exception cref="IllegalArgumentException"> if
		'''          <code>raster</code> is incompatible with <code>cm</code> </exception>
		''' <seealso cref= ColorModel </seealso>
		''' <seealso cref= Raster </seealso>
		''' <seealso cref= WritableRaster </seealso>


	'
	' *
	' *  FOR NOW THE CODE WHICH DEFINES THE RASTER TYPE IS DUPLICATED BY DVF
	' *  SEE THE METHOD DEFINERASTERTYPE @ RASTEROUTPUTMANAGER
	' *
	' 
		Public Sub New(Of T1)(ByVal cm As ColorModel, ByVal raster_Renamed As WritableRaster, ByVal isRasterPremultiplied As Boolean, ByVal properties As Dictionary(Of T1))

			If Not cm.isCompatibleRaster(raster_Renamed) Then Throw New IllegalArgumentException("Raster " & raster_Renamed & " is incompatible with ColorModel " & cm)

			If (raster_Renamed.minX <> 0) OrElse (raster_Renamed.minY <> 0) Then Throw New IllegalArgumentException("Raster " & raster_Renamed & " has minX or minY not equal to zero: " & raster_Renamed.minX & " " & raster_Renamed.minY)

			colorModel = cm
			Me.raster = raster_Renamed
			If properties IsNot Nothing AndAlso properties.Count > 0 Then
				Me.properties = New Dictionary(Of )
				For Each key As Object In properties.Keys
					If TypeOf key Is String Then Me.properties(CStr(key)) = properties(key)
				Next key
			End If
			Dim numBands As Integer = raster_Renamed.numBands
			Dim isAlphaPre As Boolean = cm.alphaPremultiplied
			Dim isStandard As Boolean = isStandard(cm, raster_Renamed)
			Dim cs As java.awt.color.ColorSpace

			' Force the raster data alpha state to match the premultiplied
			' state in the color model
			coerceData(isRasterPremultiplied)

			Dim sm As SampleModel = raster_Renamed.sampleModel
			cs = cm.colorSpace
			Dim csType As Integer = cs.type
			If csType <> java.awt.color.ColorSpace.TYPE_RGB Then
				If csType = java.awt.color.ColorSpace.TYPE_GRAY AndAlso isStandard AndAlso TypeOf cm Is ComponentColorModel Then
					' Check if this might be a child raster (fix for bug 4240596)
					If TypeOf sm Is ComponentSampleModel AndAlso CType(sm, ComponentSampleModel).pixelStride <> numBands Then
						imageType = TYPE_CUSTOM
					ElseIf TypeOf raster_Renamed Is sun.awt.image.ByteComponentRaster AndAlso raster_Renamed.numBands = 1 AndAlso cm.getComponentSize(0) = 8 AndAlso CType(raster_Renamed, sun.awt.image.ByteComponentRaster).pixelStride = 1 Then
						imageType = TYPE_BYTE_GRAY
					ElseIf TypeOf raster_Renamed Is sun.awt.image.ShortComponentRaster AndAlso raster_Renamed.numBands = 1 AndAlso cm.getComponentSize(0) = 16 AndAlso CType(raster_Renamed, sun.awt.image.ShortComponentRaster).pixelStride = 1 Then
						imageType = TYPE_USHORT_GRAY
					End If
				Else
					imageType = TYPE_CUSTOM
				End If
				Return
			End If

			If (TypeOf raster_Renamed Is sun.awt.image.IntegerComponentRaster) AndAlso (numBands = 3 OrElse numBands = 4) Then
				Dim iraster As sun.awt.image.IntegerComponentRaster = CType(raster_Renamed, sun.awt.image.IntegerComponentRaster)
				' Check if the raster params and the color model
				' are correct
				Dim pixSize As Integer = cm.pixelSize
				If iraster.pixelStride = 1 AndAlso isStandard AndAlso TypeOf cm Is DirectColorModel AndAlso (pixSize = 32 OrElse pixSize = 24) Then
					' Now check on the DirectColorModel params
					Dim dcm As DirectColorModel = CType(cm, DirectColorModel)
					Dim rmask As Integer = dcm.redMask
					Dim gmask As Integer = dcm.greenMask
					Dim bmask As Integer = dcm.blueMask
					If rmask = DCM_RED_MASK AndAlso gmask = DCM_GREEN_MASK AndAlso bmask = DCM_BLUE_MASK Then
						If dcm.alphaMask = DCM_ALPHA_MASK Then
							imageType = (If(isAlphaPre, TYPE_INT_ARGB_PRE, TYPE_INT_ARGB))
						Else
							' No Alpha
							If Not dcm.hasAlpha() Then imageType = TYPE_INT_RGB
						End If ' if (dcm.getRedMask() == DCM_RED_MASK &&
					ElseIf rmask = DCM_BGR_RED_MASK AndAlso gmask = DCM_BGR_GRN_MASK AndAlso bmask = DCM_BGR_BLU_MASK Then
						If Not dcm.hasAlpha() Then imageType = TYPE_INT_BGR
					End If ' if (rmask == DCM_BGR_RED_MASK &&
				End If ' if (iraster.getPixelStride() == 1 ' ((raster instanceof IntegerComponentRaster) &&
			ElseIf (TypeOf cm Is IndexColorModel) AndAlso (numBands = 1) AndAlso isStandard AndAlso ((Not cm.hasAlpha()) OrElse (Not isAlphaPre)) Then
				Dim icm As IndexColorModel = CType(cm, IndexColorModel)
				Dim pixSize As Integer = icm.pixelSize

				If TypeOf raster_Renamed Is sun.awt.image.BytePackedRaster Then
					imageType = TYPE_BYTE_BINARY ' if (raster instanceof BytePackedRaster)
				ElseIf TypeOf raster_Renamed Is sun.awt.image.ByteComponentRaster Then
					Dim braster As sun.awt.image.ByteComponentRaster = CType(raster_Renamed, sun.awt.image.ByteComponentRaster)
					If braster.pixelStride = 1 AndAlso pixSize <= 8 Then imageType = TYPE_BYTE_INDEXED
				End If ' else if (cm instanceof IndexColorModel) && (numBands == 1))
			ElseIf (TypeOf raster_Renamed Is sun.awt.image.ShortComponentRaster) AndAlso (TypeOf cm Is DirectColorModel) AndAlso isStandard AndAlso (numBands = 3) AndAlso (Not cm.hasAlpha()) Then
				Dim dcm As DirectColorModel = CType(cm, DirectColorModel)
				If dcm.redMask = DCM_565_RED_MASK Then
					If dcm.greenMask = DCM_565_GRN_MASK AndAlso dcm.blueMask = DCM_565_BLU_MASK Then imageType = TYPE_USHORT_565_RGB
				ElseIf dcm.redMask = DCM_555_RED_MASK Then
					If dcm.greenMask = DCM_555_GRN_MASK AndAlso dcm.blueMask = DCM_555_BLU_MASK Then imageType = TYPE_USHORT_555_RGB
				End If ' else if ((cm instanceof IndexColorModel) && (numBands == 1))
			ElseIf (TypeOf raster_Renamed Is sun.awt.image.ByteComponentRaster) AndAlso (TypeOf cm Is ComponentColorModel) AndAlso isStandard AndAlso (TypeOf raster_Renamed.sampleModel Is PixelInterleavedSampleModel) AndAlso (numBands = 3 OrElse numBands = 4) Then
				Dim ccm As ComponentColorModel = CType(cm, ComponentColorModel)
				Dim csm As PixelInterleavedSampleModel = CType(raster_Renamed.sampleModel, PixelInterleavedSampleModel)
				Dim braster As sun.awt.image.ByteComponentRaster = CType(raster_Renamed, sun.awt.image.ByteComponentRaster)
				Dim offs As Integer() = csm.bandOffsets
				If ccm.numComponents <> numBands Then Throw New RasterFormatException("Number of components in " & "ColorModel (" & ccm.numComponents & ") does not match # in " & " Raster (" & numBands & ")")
				Dim nBits As Integer() = ccm.componentSize
				Dim is8bit As Boolean = True
				For i As Integer = 0 To numBands - 1
					If nBits(i) <> 8 Then
						is8bit = False
						Exit For
					End If
				Next i
				If is8bit AndAlso braster.pixelStride = numBands AndAlso offs(0) = numBands-1 AndAlso offs(1) = numBands-2 AndAlso offs(2) = numBands-3 Then
					If numBands = 3 AndAlso (Not ccm.hasAlpha()) Then
						imageType = TYPE_3BYTE_BGR
					ElseIf offs(3) = 0 AndAlso ccm.hasAlpha() Then
						imageType = (If(isAlphaPre, TYPE_4BYTE_ABGR_PRE, TYPE_4BYTE_ABGR))
					End If
				End If
			End If ' else if ((raster instanceof ByteComponentRaster) &&
		End Sub

		Private Shared Function isStandard(ByVal cm As ColorModel, ByVal wr As WritableRaster) As Boolean
			Dim cmClass As  [Class] = cm.GetType()
			Dim wrClass As  [Class] = wr.GetType()
			Dim smClass As  [Class] = wr.sampleModel.GetType()

			Dim checkClassLoadersAction As java.security.PrivilegedAction(Of Boolean?) = New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Return java.security.AccessController.doPrivileged(checkClassLoadersAction)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As Boolean?
				Dim std As  [Class]Loader = GetType(System).classLoader

				Return (cmClass.classLoader Is std) AndAlso (smClass.classLoader Is std) AndAlso (wrClass.classLoader Is std)
			End Function
		End Class

		''' <summary>
		''' Returns the image type.  If it is not one of the known types,
		''' TYPE_CUSTOM is returned. </summary>
		''' <returns> the image type of this <code>BufferedImage</code>. </returns>
		''' <seealso cref= #TYPE_INT_RGB </seealso>
		''' <seealso cref= #TYPE_INT_ARGB </seealso>
		''' <seealso cref= #TYPE_INT_ARGB_PRE </seealso>
		''' <seealso cref= #TYPE_INT_BGR </seealso>
		''' <seealso cref= #TYPE_3BYTE_BGR </seealso>
		''' <seealso cref= #TYPE_4BYTE_ABGR </seealso>
		''' <seealso cref= #TYPE_4BYTE_ABGR_PRE </seealso>
		''' <seealso cref= #TYPE_BYTE_GRAY </seealso>
		''' <seealso cref= #TYPE_BYTE_BINARY </seealso>
		''' <seealso cref= #TYPE_BYTE_INDEXED </seealso>
		''' <seealso cref= #TYPE_USHORT_GRAY </seealso>
		''' <seealso cref= #TYPE_USHORT_565_RGB </seealso>
		''' <seealso cref= #TYPE_USHORT_555_RGB </seealso>
		''' <seealso cref= #TYPE_CUSTOM </seealso>
		Public Overridable Property type As Integer
			Get
				Return imageType
			End Get
		End Property

		''' <summary>
		''' Returns the <code>ColorModel</code>. </summary>
		''' <returns> the <code>ColorModel</code> of this
		'''  <code>BufferedImage</code>. </returns>
		Public Overridable Property colorModel As ColorModel Implements RenderedImage.getColorModel
			Get
				Return colorModel
			End Get
		End Property

		''' <summary>
		''' Returns the <seealso cref="WritableRaster"/>. </summary>
		''' <returns> the <code>WriteableRaster</code> of this
		'''  <code>BufferedImage</code>. </returns>
		Public Overridable Property raster As WritableRaster
			Get
				Return raster
			End Get
		End Property


		''' <summary>
		''' Returns a <code>WritableRaster</code> representing the alpha
		''' channel for <code>BufferedImage</code> objects
		''' with <code>ColorModel</code> objects that support a separate
		''' spatial alpha channel, such as <code>ComponentColorModel</code> and
		''' <code>DirectColorModel</code>.  Returns <code>null</code> if there
		''' is no alpha channel associated with the <code>ColorModel</code> in
		''' this image.  This method assumes that for all
		''' <code>ColorModel</code> objects other than
		''' <code>IndexColorModel</code>, if the <code>ColorModel</code>
		''' supports alpha, there is a separate alpha channel
		''' which is stored as the last band of image data.
		''' If the image uses an <code>IndexColorModel</code> that
		''' has alpha in the lookup table, this method returns
		''' <code>null</code> since there is no spatially discrete alpha
		''' channel.  This method creates a new
		''' <code>WritableRaster</code>, but shares the data array. </summary>
		''' <returns> a <code>WritableRaster</code> or <code>null</code> if this
		'''          <code>BufferedImage</code> has no alpha channel associated
		'''          with its <code>ColorModel</code>. </returns>
		Public Overridable Property alphaRaster As WritableRaster
			Get
				Return colorModel.getAlphaRaster(raster)
			End Get
		End Property

		''' <summary>
		''' Returns an integer pixel in the default RGB color model
		''' (TYPE_INT_ARGB) and default sRGB colorspace.  Color
		''' conversion takes place if this default model does not match
		''' the image <code>ColorModel</code>.  There are only 8-bits of
		''' precision for each color component in the returned data when using
		''' this method.
		''' 
		''' <p>
		''' 
		''' An <code>ArrayOutOfBoundsException</code> may be thrown
		''' if the coordinates are not in bounds.
		''' However, explicit bounds checking is not guaranteed.
		''' </summary>
		''' <param name="x"> the X coordinate of the pixel from which to get
		'''          the pixel in the default RGB color model and sRGB
		'''          color space </param>
		''' <param name="y"> the Y coordinate of the pixel from which to get
		'''          the pixel in the default RGB color model and sRGB
		'''          color space </param>
		''' <returns> an integer pixel in the default RGB color model and
		'''          default sRGB colorspace. </returns>
		''' <seealso cref= #setRGB(int, int, int) </seealso>
		''' <seealso cref= #setRGB(int, int, int, int, int[], int, int) </seealso>
		Public Overridable Function getRGB(ByVal x As Integer, ByVal y As Integer) As Integer
			Return colorModel.getRGB(raster.getDataElements(x, y, Nothing))
		End Function

		''' <summary>
		''' Returns an array of integer pixels in the default RGB color model
		''' (TYPE_INT_ARGB) and default sRGB color space,
		''' from a portion of the image data.  Color conversion takes
		''' place if the default model does not match the image
		''' <code>ColorModel</code>.  There are only 8-bits of precision for
		''' each color component in the returned data when
		''' using this method.  With a specified coordinate (x,&nbsp;y) in the
		''' image, the ARGB pixel can be accessed in this way:
		''' 
		''' <pre>
		'''    pixel   = rgbArray[offset + (y-startY)*scansize + (x-startX)]; </pre>
		''' 
		''' <p>
		''' 
		''' An <code>ArrayOutOfBoundsException</code> may be thrown
		''' if the region is not in bounds.
		''' However, explicit bounds checking is not guaranteed.
		''' </summary>
		''' <param name="startX">      the starting X coordinate </param>
		''' <param name="startY">      the starting Y coordinate </param>
		''' <param name="w">           width of region </param>
		''' <param name="h">           height of region </param>
		''' <param name="rgbArray">    if not <code>null</code>, the rgb pixels are
		'''          written here </param>
		''' <param name="offset">      offset into the <code>rgbArray</code> </param>
		''' <param name="scansize">    scanline stride for the <code>rgbArray</code> </param>
		''' <returns>            array of RGB pixels. </returns>
		''' <seealso cref= #setRGB(int, int, int) </seealso>
		''' <seealso cref= #setRGB(int, int, int, int, int[], int, int) </seealso>
		Public Overridable Function getRGB(ByVal startX As Integer, ByVal startY As Integer, ByVal w As Integer, ByVal h As Integer, ByVal rgbArray As Integer(), ByVal offset As Integer, ByVal scansize As Integer) As Integer()
			Dim yoff As Integer = offset
			Dim [off] As Integer
			Dim data_Renamed As Object
			Dim nbands As Integer = raster.numBands
			Dim dataType As Integer = raster.dataBuffer.dataType
			Select Case dataType
			Case DataBuffer.TYPE_BYTE
				data_Renamed = New SByte(nbands - 1){}
			Case DataBuffer.TYPE_USHORT
				data_Renamed = New Short(nbands - 1){}
			Case DataBuffer.TYPE_INT
				data_Renamed = New Integer(nbands - 1){}
			Case DataBuffer.TYPE_FLOAT
				data_Renamed = New Single(nbands - 1){}
			Case DataBuffer.TYPE_DOUBLE
				data_Renamed = New Double(nbands - 1){}
			Case Else
				Throw New IllegalArgumentException("Unknown data buffer type: " & dataType)
			End Select

			If rgbArray Is Nothing Then rgbArray = New Integer(offset+h*scansize - 1){}

			Dim y As Integer = startY
			Do While y < startY+h
				[off] = yoff
				For x As Integer = startX To startX+w - 1
					rgbArray([off]) = colorModel.getRGB(raster.getDataElements(x, y, data_Renamed))
					[off] += 1
				Next x
				y += 1
				yoff+=scansize
			Loop

			Return rgbArray
		End Function


		''' <summary>
		''' Sets a pixel in this <code>BufferedImage</code> to the specified
		''' RGB value. The pixel is assumed to be in the default RGB color
		''' model, TYPE_INT_ARGB, and default sRGB color space.  For images
		''' with an <code>IndexColorModel</code>, the index with the nearest
		''' color is chosen.
		''' 
		''' <p>
		''' 
		''' An <code>ArrayOutOfBoundsException</code> may be thrown
		''' if the coordinates are not in bounds.
		''' However, explicit bounds checking is not guaranteed.
		''' </summary>
		''' <param name="x"> the X coordinate of the pixel to set </param>
		''' <param name="y"> the Y coordinate of the pixel to set </param>
		''' <param name="rgb"> the RGB value </param>
		''' <seealso cref= #getRGB(int, int) </seealso>
		''' <seealso cref= #getRGB(int, int, int, int, int[], int, int) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub setRGB(ByVal x As Integer, ByVal y As Integer, ByVal rgb As Integer)
			raster.dataElementsnts(x, y, colorModel.getDataElements(rgb, Nothing))
		End Sub

		''' <summary>
		''' Sets an array of integer pixels in the default RGB color model
		''' (TYPE_INT_ARGB) and default sRGB color space,
		''' into a portion of the image data.  Color conversion takes place
		''' if the default model does not match the image
		''' <code>ColorModel</code>.  There are only 8-bits of precision for
		''' each color component in the returned data when
		''' using this method.  With a specified coordinate (x,&nbsp;y) in the
		''' this image, the ARGB pixel can be accessed in this way:
		''' <pre>
		'''    pixel   = rgbArray[offset + (y-startY)*scansize + (x-startX)];
		''' </pre>
		''' WARNING: No dithering takes place.
		''' 
		''' <p>
		''' 
		''' An <code>ArrayOutOfBoundsException</code> may be thrown
		''' if the region is not in bounds.
		''' However, explicit bounds checking is not guaranteed.
		''' </summary>
		''' <param name="startX">      the starting X coordinate </param>
		''' <param name="startY">      the starting Y coordinate </param>
		''' <param name="w">           width of the region </param>
		''' <param name="h">           height of the region </param>
		''' <param name="rgbArray">    the rgb pixels </param>
		''' <param name="offset">      offset into the <code>rgbArray</code> </param>
		''' <param name="scansize">    scanline stride for the <code>rgbArray</code> </param>
		''' <seealso cref= #getRGB(int, int) </seealso>
		''' <seealso cref= #getRGB(int, int, int, int, int[], int, int) </seealso>
		Public Overridable Sub setRGB(ByVal startX As Integer, ByVal startY As Integer, ByVal w As Integer, ByVal h As Integer, ByVal rgbArray As Integer(), ByVal offset As Integer, ByVal scansize As Integer)
			Dim yoff As Integer = offset
			Dim [off] As Integer
			Dim pixel As Object = Nothing

			Dim y As Integer = startY
			Do While y < startY+h
				[off] = yoff
				For x As Integer = startX To startX+w - 1
					pixel = colorModel.getDataElements(rgbArray([off]), pixel)
					[off] += 1
					raster.dataElementsnts(x, y, pixel)
				Next x
				y += 1
				yoff+=scansize
			Loop
		End Sub


		''' <summary>
		''' Returns the width of the <code>BufferedImage</code>. </summary>
		''' <returns> the width of this <code>BufferedImage</code> </returns>
		Public Overridable Property width As Integer Implements RenderedImage.getWidth
			Get
				Return raster.width
			End Get
		End Property

		''' <summary>
		''' Returns the height of the <code>BufferedImage</code>. </summary>
		''' <returns> the height of this <code>BufferedImage</code> </returns>
		Public Overridable Property height As Integer Implements RenderedImage.getHeight
			Get
				Return raster.height
			End Get
		End Property

		''' <summary>
		''' Returns the width of the <code>BufferedImage</code>. </summary>
		''' <param name="observer"> ignored </param>
		''' <returns> the width of this <code>BufferedImage</code> </returns>
		Public Overridable Function getWidth(ByVal observer As ImageObserver) As Integer
			Return raster.width
		End Function

		''' <summary>
		''' Returns the height of the <code>BufferedImage</code>. </summary>
		''' <param name="observer"> ignored </param>
		''' <returns> the height of this <code>BufferedImage</code> </returns>
		Public Overridable Function getHeight(ByVal observer As ImageObserver) As Integer
			Return raster.height
		End Function

		''' <summary>
		''' Returns the object that produces the pixels for the image. </summary>
		''' <returns> the <seealso cref="ImageProducer"/> that is used to produce the
		''' pixels for this image. </returns>
		''' <seealso cref= ImageProducer </seealso>
		Public Property Overrides source As ImageProducer
			Get
				If osis Is Nothing Then
					If properties Is Nothing Then properties = New Hashtable
					osis = New sun.awt.image.OffScreenImageSource(Me, properties)
				End If
				Return osis
			End Get
		End Property


		''' <summary>
		''' Returns a property of the image by name.  Individual property names
		''' are defined by the various image formats.  If a property is not
		''' defined for a particular image, this method returns the
		''' <code>UndefinedProperty</code> field.  If the properties
		''' for this image are not yet known, then this method returns
		''' <code>null</code> and the <code>ImageObserver</code> object is
		''' notified later.  The property name "comment" should be used to
		''' store an optional comment that can be presented to the user as a
		''' description of the image, its source, or its author. </summary>
		''' <param name="name"> the property name </param>
		''' <param name="observer"> the <code>ImageObserver</code> that receives
		'''  notification regarding image information </param>
		''' <returns> an <seealso cref="Object"/> that is the property referred to by the
		'''          specified <code>name</code> or <code>null</code> if the
		'''          properties of this image are not yet known. </returns>
		''' <exception cref="NullPointerException"> if the property name is null. </exception>
		''' <seealso cref= ImageObserver </seealso>
		''' <seealso cref= java.awt.Image#UndefinedProperty </seealso>
		Public Overridable Function getProperty(ByVal name As String, ByVal observer As ImageObserver) As Object
			Return getProperty(name)
		End Function

		''' <summary>
		''' Returns a property of the image by name. </summary>
		''' <param name="name"> the property name </param>
		''' <returns> an <code>Object</code> that is the property referred to by
		'''          the specified <code>name</code>. </returns>
		''' <exception cref="NullPointerException"> if the property name is null. </exception>
		Public Overridable Function getProperty(ByVal name As String) As Object Implements RenderedImage.getProperty
			If name Is Nothing Then Throw New NullPointerException("null property name is not allowed")
			If properties Is Nothing Then Return java.awt.Image.UndefinedProperty
			Dim o As Object = properties(name)
			If o Is Nothing Then o = java.awt.Image.UndefinedProperty
			Return o
		End Function

		''' <summary>
		''' This method returns a <seealso cref="Graphics2D"/>, but is here
		''' for backwards compatibility.  <seealso cref="#createGraphics() createGraphics"/> is more
		''' convenient, since it is declared to return a
		''' <code>Graphics2D</code>. </summary>
		''' <returns> a <code>Graphics2D</code>, which can be used to draw into
		'''          this image. </returns>
		Public Property Overrides graphics As java.awt.Graphics
			Get
				Return createGraphics()
			End Get
		End Property

		''' <summary>
		''' Creates a <code>Graphics2D</code>, which can be used to draw into
		''' this <code>BufferedImage</code>. </summary>
		''' <returns> a <code>Graphics2D</code>, used for drawing into this
		'''          image. </returns>
		Public Overridable Function createGraphics() As java.awt.Graphics2D
			Dim env As java.awt.GraphicsEnvironment = java.awt.GraphicsEnvironment.localGraphicsEnvironment
			Return env.createGraphics(Me)
		End Function

		''' <summary>
		''' Returns a subimage defined by a specified rectangular region.
		''' The returned <code>BufferedImage</code> shares the same
		''' data array as the original image. </summary>
		''' <param name="x"> the X coordinate of the upper-left corner of the
		'''          specified rectangular region </param>
		''' <param name="y"> the Y coordinate of the upper-left corner of the
		'''          specified rectangular region </param>
		''' <param name="w"> the width of the specified rectangular region </param>
		''' <param name="h"> the height of the specified rectangular region </param>
		''' <returns> a <code>BufferedImage</code> that is the subimage of this
		'''          <code>BufferedImage</code>. </returns>
		''' <exception cref="RasterFormatException"> if the specified
		''' area is not contained within this <code>BufferedImage</code>. </exception>
		Public Overridable Function getSubimage(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As BufferedImage
			Return New BufferedImage(colorModel, raster.createWritableChild(x, y, w, h, 0, 0, Nothing), colorModel.alphaPremultiplied, properties)
		End Function

		''' <summary>
		''' Returns whether or not the alpha has been premultiplied.  It
		''' returns <code>false</code> if there is no alpha. </summary>
		''' <returns> <code>true</code> if the alpha has been premultiplied;
		'''          <code>false</code> otherwise. </returns>
		Public Overridable Property alphaPremultiplied As Boolean
			Get
				Return colorModel.alphaPremultiplied
			End Get
		End Property

		''' <summary>
		''' Forces the data to match the state specified in the
		''' <code>isAlphaPremultiplied</code> variable.  It may multiply or
		''' divide the color raster data by alpha, or do nothing if the data is
		''' in the correct state. </summary>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the alpha has been
		'''          premultiplied; <code>false</code> otherwise. </param>
		Public Overridable Sub coerceData(ByVal isAlphaPremultiplied As Boolean)
			If colorModel.hasAlpha() AndAlso colorModel.alphaPremultiplied <> isAlphaPremultiplied Then colorModel = colorModel.coerceData(raster, isAlphaPremultiplied)
		End Sub

		''' <summary>
		''' Returns a <code>String</code> representation of this
		''' <code>BufferedImage</code> object and its values. </summary>
		''' <returns> a <code>String</code> representing this
		'''          <code>BufferedImage</code>. </returns>
		Public Overrides Function ToString() As String
			Return "BufferedImage@" &  java.lang.[Integer].toHexString(GetHashCode()) & ": type = " & imageType & " " & colorModel & " " & raster
		End Function

		''' <summary>
		''' Returns a <seealso cref="Vector"/> of <seealso cref="RenderedImage"/> objects that are
		''' the immediate sources, not the sources of these immediate sources,
		''' of image data for this <code>BufferedImage</code>.  This
		''' method returns <code>null</code> if the <code>BufferedImage</code>
		''' has no information about its immediate sources.  It returns an
		''' empty <code>Vector</code> if the <code>BufferedImage</code> has no
		''' immediate sources. </summary>
		''' <returns> a <code>Vector</code> containing immediate sources of
		'''          this <code>BufferedImage</code> object's image date, or
		'''          <code>null</code> if this <code>BufferedImage</code> has
		'''          no information about its immediate sources, or an empty
		'''          <code>Vector</code> if this <code>BufferedImage</code>
		'''          has no immediate sources. </returns>
		Public Overridable Property sources As List(Of RenderedImage) Implements RenderedImage.getSources
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an array of names recognized by
		''' <seealso cref="#getProperty(String) getProperty(String)"/>
		''' or <code>null</code>, if no property names are recognized. </summary>
		''' <returns> a <code>String</code> array containing all of the property
		'''          names that <code>getProperty(String)</code> recognizes;
		'''          or <code>null</code> if no property names are recognized. </returns>
		Public Overridable Property propertyNames As String() Implements RenderedImage.getPropertyNames
			Get
				If properties Is Nothing OrElse properties.Count = 0 Then Return Nothing
				Dim keys As Dictionary(Of String, Object).KeyCollection = properties.Keys
				Return keys.ToArray(New String(keys.size() - 1){})
			End Get
		End Property

		''' <summary>
		''' Returns the minimum x coordinate of this
		''' <code>BufferedImage</code>.  This is always zero. </summary>
		''' <returns> the minimum x coordinate of this
		'''          <code>BufferedImage</code>. </returns>
		Public Overridable Property minX As Integer Implements RenderedImage.getMinX
			Get
				Return raster.minX
			End Get
		End Property

		''' <summary>
		''' Returns the minimum y coordinate of this
		''' <code>BufferedImage</code>.  This is always zero. </summary>
		''' <returns> the minimum y coordinate of this
		'''          <code>BufferedImage</code>. </returns>
		Public Overridable Property minY As Integer Implements RenderedImage.getMinY
			Get
				Return raster.minY
			End Get
		End Property

		''' <summary>
		''' Returns the <code>SampleModel</code> associated with this
		''' <code>BufferedImage</code>. </summary>
		''' <returns> the <code>SampleModel</code> of this
		'''          <code>BufferedImage</code>. </returns>
		Public Overridable Property sampleModel As SampleModel Implements RenderedImage.getSampleModel
			Get
				Return raster.sampleModel
			End Get
		End Property

		''' <summary>
		''' Returns the number of tiles in the x direction.
		''' This is always one. </summary>
		''' <returns> the number of tiles in the x direction. </returns>
		Public Overridable Property numXTiles As Integer Implements RenderedImage.getNumXTiles
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' Returns the number of tiles in the y direction.
		''' This is always one. </summary>
		''' <returns> the number of tiles in the y direction. </returns>
		Public Overridable Property numYTiles As Integer Implements RenderedImage.getNumYTiles
			Get
				Return 1
			End Get
		End Property

		''' <summary>
		''' Returns the minimum tile index in the x direction.
		''' This is always zero. </summary>
		''' <returns> the minimum tile index in the x direction. </returns>
		Public Overridable Property minTileX As Integer Implements RenderedImage.getMinTileX
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' Returns the minimum tile index in the y direction.
		''' This is always zero. </summary>
		''' <returns> the minimum tile index in the y direction. </returns>
		Public Overridable Property minTileY As Integer Implements RenderedImage.getMinTileY
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' Returns the tile width in pixels. </summary>
		''' <returns> the tile width in pixels. </returns>
		Public Overridable Property tileWidth As Integer Implements RenderedImage.getTileWidth
			Get
			   Return raster.width
			End Get
		End Property

		''' <summary>
		''' Returns the tile height in pixels. </summary>
		''' <returns> the tile height in pixels. </returns>
		Public Overridable Property tileHeight As Integer Implements RenderedImage.getTileHeight
			Get
			   Return raster.height
			End Get
		End Property

		''' <summary>
		''' Returns the x offset of the tile grid relative to the origin,
		''' For example, the x coordinate of the location of tile
		''' (0,&nbsp;0).  This is always zero. </summary>
		''' <returns> the x offset of the tile grid. </returns>
		Public Overridable Property tileGridXOffset As Integer Implements RenderedImage.getTileGridXOffset
			Get
				Return raster.sampleModelTranslateX
			End Get
		End Property

		''' <summary>
		''' Returns the y offset of the tile grid relative to the origin,
		''' For example, the y coordinate of the location of tile
		''' (0,&nbsp;0).  This is always zero. </summary>
		''' <returns> the y offset of the tile grid. </returns>
		Public Overridable Property tileGridYOffset As Integer Implements RenderedImage.getTileGridYOffset
			Get
				Return raster.sampleModelTranslateY
			End Get
		End Property

		''' <summary>
		''' Returns tile (<code>tileX</code>,&nbsp;<code>tileY</code>).  Note
		''' that <code>tileX</code> and <code>tileY</code> are indices
		''' into the tile array, not pixel locations.  The <code>Raster</code>
		''' that is returned is live, which means that it is updated if the
		''' image is changed. </summary>
		''' <param name="tileX"> the x index of the requested tile in the tile array </param>
		''' <param name="tileY"> the y index of the requested tile in the tile array </param>
		''' <returns> a <code>Raster</code> that is the tile defined by the
		'''          arguments <code>tileX</code> and <code>tileY</code>. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if both
		'''          <code>tileX</code> and <code>tileY</code> are not
		'''          equal to 0 </exception>
		Public Overridable Function getTile(ByVal tileX As Integer, ByVal tileY As Integer) As Raster Implements RenderedImage.getTile
			If tileX = 0 AndAlso tileY = 0 Then Return raster
			Throw New ArrayIndexOutOfBoundsException("BufferedImages only have" & " one tile with index 0,0")
		End Function

		''' <summary>
		''' Returns the image as one large tile.  The <code>Raster</code>
		''' returned is a copy of the image data is not updated if the
		''' image is changed. </summary>
		''' <returns> a <code>Raster</code> that is a copy of the image data. </returns>
		''' <seealso cref= #setData(Raster) </seealso>
		Public Overridable Property data As Raster Implements RenderedImage.getData
			Get
    
				' REMIND : this allocates a whole new tile if raster is a
				' subtile.  (It only copies in the requested area)
				' We should do something smarter.
				Dim width_Renamed As Integer = raster.width
				Dim height_Renamed As Integer = raster.height
				Dim startX As Integer = raster.minX
				Dim startY As Integer = raster.minY
				Dim wr As WritableRaster = Raster.createWritableRaster(raster.sampleModel, New java.awt.Point(raster.sampleModelTranslateX, raster.sampleModelTranslateY))
    
				Dim tdata As Object = Nothing
    
				For i As Integer = startY To startY+height_Renamed - 1
					tdata = raster.getDataElements(startX,i,width_Renamed,1,tdata)
					wr.dataElementsnts(startX,i,width_Renamed,1, tdata)
				Next i
				Return wr
			End Get
			Set(ByVal r As Raster)
				Dim width_Renamed As Integer = r.width
				Dim height_Renamed As Integer = r.height
				Dim startX As Integer = r.minX
				Dim startY As Integer = r.minY
    
				Dim tdata As Integer() = Nothing
    
				' Clip to the current Raster
				Dim rclip As New java.awt.Rectangle(startX, startY, width_Renamed, height_Renamed)
				Dim bclip As New java.awt.Rectangle(0, 0, raster.width, raster.height)
				Dim intersect As java.awt.Rectangle = rclip.intersection(bclip)
				If intersect.empty Then Return
				width_Renamed = intersect.width
				height_Renamed = intersect.height
				startX = intersect.x
				startY = intersect.y
    
				' remind use get/setDataElements for speed if Rasters are
				' compatible
				For i As Integer = startY To startY+height_Renamed - 1
					tdata = r.getPixels(startX,i,width_Renamed,1,tdata)
					raster.pixelsels(startX,i,width_Renamed,1, tdata)
				Next i
			End Set
		End Property

		''' <summary>
		''' Computes and returns an arbitrary region of the
		''' <code>BufferedImage</code>.  The <code>Raster</code> returned is a
		''' copy of the image data and is not updated if the image is
		''' changed. </summary>
		''' <param name="rect"> the region of the <code>BufferedImage</code> to be
		''' returned. </param>
		''' <returns> a <code>Raster</code> that is a copy of the image data of
		'''          the specified region of the <code>BufferedImage</code> </returns>
		''' <seealso cref= #setData(Raster) </seealso>
		Public Overridable Function getData(ByVal rect As java.awt.Rectangle) As Raster Implements RenderedImage.getData
			Dim sm As SampleModel = raster.sampleModel
			Dim nsm As SampleModel = sm.createCompatibleSampleModel(rect.width, rect.height)
			Dim wr As WritableRaster = Raster.createWritableRaster(nsm, rect.location)
			Dim width_Renamed As Integer = rect.width
			Dim height_Renamed As Integer = rect.height
			Dim startX As Integer = rect.x
			Dim startY As Integer = rect.y

			Dim tdata As Object = Nothing

			For i As Integer = startY To startY+height_Renamed - 1
				tdata = raster.getDataElements(startX,i,width_Renamed,1,tdata)
				wr.dataElementsnts(startX,i,width_Renamed,1, tdata)
			Next i
			Return wr
		End Function

		''' <summary>
		''' Computes an arbitrary rectangular region of the
		''' <code>BufferedImage</code> and copies it into a specified
		''' <code>WritableRaster</code>.  The region to be computed is
		''' determined from the bounds of the specified
		''' <code>WritableRaster</code>.  The specified
		''' <code>WritableRaster</code> must have a
		''' <code>SampleModel</code> that is compatible with this image.  If
		''' <code>outRaster</code> is <code>null</code>,
		''' an appropriate <code>WritableRaster</code> is created. </summary>
		''' <param name="outRaster"> a <code>WritableRaster</code> to hold the returned
		'''          part of the image, or <code>null</code> </param>
		''' <returns> a reference to the supplied or created
		'''          <code>WritableRaster</code>. </returns>
		Public Overridable Function copyData(ByVal outRaster As WritableRaster) As WritableRaster Implements RenderedImage.copyData
			If outRaster Is Nothing Then Return CType(data, WritableRaster)
			Dim width_Renamed As Integer = outRaster.width
			Dim height_Renamed As Integer = outRaster.height
			Dim startX As Integer = outRaster.minX
			Dim startY As Integer = outRaster.minY

			Dim tdata As Object = Nothing

			For i As Integer = startY To startY+height_Renamed - 1
				tdata = raster.getDataElements(startX,i,width_Renamed,1,tdata)
				outRaster.dataElementsnts(startX,i,width_Renamed,1, tdata)
			Next i

			Return outRaster
		End Function



	  ''' <summary>
	  ''' Adds a tile observer.  If the observer is already present,
	  ''' it receives multiple notifications. </summary>
	  ''' <param name="to"> the specified <seealso cref="TileObserver"/> </param>
		Public Overridable Sub addTileObserver(ByVal [to] As TileObserver) Implements WritableRenderedImage.addTileObserver
		End Sub

	  ''' <summary>
	  ''' Removes a tile observer.  If the observer was not registered,
	  ''' nothing happens.  If the observer was registered for multiple
	  ''' notifications, it is now registered for one fewer notification. </summary>
	  ''' <param name="to"> the specified <code>TileObserver</code>. </param>
		Public Overridable Sub removeTileObserver(ByVal [to] As TileObserver) Implements WritableRenderedImage.removeTileObserver
		End Sub

		''' <summary>
		''' Returns whether or not a tile is currently checked out for writing. </summary>
		''' <param name="tileX"> the x index of the tile. </param>
		''' <param name="tileY"> the y index of the tile. </param>
		''' <returns> <code>true</code> if the tile specified by the specified
		'''          indices is checked out for writing; <code>false</code>
		'''          otherwise. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if both
		'''          <code>tileX</code> and <code>tileY</code> are not equal
		'''          to 0 </exception>
		Public Overridable Function isTileWritable(ByVal tileX As Integer, ByVal tileY As Integer) As Boolean Implements WritableRenderedImage.isTileWritable
			If tileX = 0 AndAlso tileY = 0 Then Return True
			Throw New IllegalArgumentException("Only 1 tile in image")
		End Function

		''' <summary>
		''' Returns an array of <seealso cref="Point"/> objects indicating which tiles
		''' are checked out for writing.  Returns <code>null</code> if none are
		''' checked out. </summary>
		''' <returns> a <code>Point</code> array that indicates the tiles that
		'''          are checked out for writing, or <code>null</code> if no
		'''          tiles are checked out for writing. </returns>
		Public Overridable Property writableTileIndices As java.awt.Point() Implements WritableRenderedImage.getWritableTileIndices
			Get
				Dim p As java.awt.Point() = New java.awt.Point(0){}
				p(0) = New java.awt.Point(0, 0)
    
				Return p
			End Get
		End Property

		''' <summary>
		''' Returns whether or not any tile is checked out for writing.
		''' Semantically equivalent to
		''' <pre>
		''' (getWritableTileIndices() != null).
		''' </pre> </summary>
		''' <returns> <code>true</code> if any tile is checked out for writing;
		'''          <code>false</code> otherwise. </returns>
		Public Overridable Function hasTileWriters() As Boolean Implements WritableRenderedImage.hasTileWriters
			Return True
		End Function

	  ''' <summary>
	  ''' Checks out a tile for writing.  All registered
	  ''' <code>TileObservers</code> are notified when a tile goes from having
	  ''' no writers to having one writer. </summary>
	  ''' <param name="tileX"> the x index of the tile </param>
	  ''' <param name="tileY"> the y index of the tile </param>
	  ''' <returns> a <code>WritableRaster</code> that is the tile, indicated by
	  '''            the specified indices, to be checked out for writing. </returns>
		Public Overridable Function getWritableTile(ByVal tileX As Integer, ByVal tileY As Integer) As WritableRaster Implements WritableRenderedImage.getWritableTile
			Return raster
		End Function

	  ''' <summary>
	  ''' Relinquishes permission to write to a tile.  If the caller
	  ''' continues to write to the tile, the results are undefined.
	  ''' Calls to this method should only appear in matching pairs
	  ''' with calls to <seealso cref="#getWritableTile(int, int) getWritableTile(int, int)"/>.  Any other leads
	  ''' to undefined results.  All registered <code>TileObservers</code>
	  ''' are notified when a tile goes from having one writer to having no
	  ''' writers. </summary>
	  ''' <param name="tileX"> the x index of the tile </param>
	  ''' <param name="tileY"> the y index of the tile </param>
		Public Overridable Sub releaseWritableTile(ByVal tileX As Integer, ByVal tileY As Integer) Implements WritableRenderedImage.releaseWritableTile
		End Sub

		''' <summary>
		''' Returns the transparency.  Returns either OPAQUE, BITMASK,
		''' or TRANSLUCENT. </summary>
		''' <returns> the transparency of this <code>BufferedImage</code>. </returns>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT
		''' @since 1.5 </seealso>
		Public Overridable Property transparency As Integer
			Get
				Return colorModel.transparency
			End Get
		End Property
	End Class

End Namespace