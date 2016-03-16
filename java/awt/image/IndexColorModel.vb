Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>IndexColorModel</code> class is a <code>ColorModel</code>
	''' class that works with pixel values consisting of a
	''' single sample that is an index into a fixed colormap in the default
	''' sRGB color space.  The colormap specifies red, green, blue, and
	''' optional alpha components corresponding to each index.  All components
	''' are represented in the colormap as 8-bit unsigned integral values.
	''' Some constructors allow the caller to specify "holes" in the colormap
	''' by indicating which colormap entries are valid and which represent
	''' unusable colors via the bits set in a <code>BigInteger</code> object.
	''' This color model is similar to an X11 PseudoColor visual.
	''' <p>
	''' Some constructors provide a means to specify an alpha component
	''' for each pixel in the colormap, while others either provide no
	''' such means or, in some cases, a flag to indicate whether the
	''' colormap data contains alpha values.  If no alpha is supplied to
	''' the constructor, an opaque alpha component (alpha = 1.0) is
	''' assumed for each entry.
	''' An optional transparent pixel value can be supplied that indicates a
	''' pixel to be made completely transparent, regardless of any alpha
	''' component supplied or assumed for that pixel value.
	''' Note that the color components in the colormap of an
	''' <code>IndexColorModel</code> objects are never pre-multiplied with
	''' the alpha components.
	''' <p>
	''' <a name="transparency">
	''' The transparency of an <code>IndexColorModel</code> object is
	''' determined by examining the alpha components of the colors in the
	''' colormap and choosing the most specific value after considering
	''' the optional alpha values and any transparent index specified.
	''' The transparency value is <code>Transparency.OPAQUE</code>
	''' only if all valid colors in
	''' the colormap are opaque and there is no valid transparent pixel.
	''' If all valid colors
	''' in the colormap are either completely opaque (alpha = 1.0) or
	''' completely transparent (alpha = 0.0), which typically occurs when
	''' a valid transparent pixel is specified,
	''' the value is <code>Transparency.BITMASK</code>.
	''' Otherwise, the value is <code>Transparency.TRANSLUCENT</code>, indicating
	''' that some valid color has an alpha component that is
	''' neither completely transparent nor completely opaque
	''' (0.0 &lt; alpha &lt; 1.0).
	''' </a>
	''' 
	''' <p>
	''' If an <code>IndexColorModel</code> object has
	''' a transparency value of <code>Transparency.OPAQUE</code>,
	''' then the <code>hasAlpha</code>
	''' and <code>getNumComponents</code> methods
	''' (both inherited from <code>ColorModel</code>)
	''' return false and 3, respectively.
	''' For any other transparency value,
	''' <code>hasAlpha</code> returns true
	''' and <code>getNumComponents</code> returns 4.
	''' 
	''' <p>
	''' <a name="index_values">
	''' The values used to index into the colormap are taken from the least
	''' significant <em>n</em> bits of pixel representations where
	''' <em>n</em> is based on the pixel size specified in the constructor.
	''' For pixel sizes smaller than 8 bits, <em>n</em> is rounded up to a
	''' power of two (3 becomes 4 and 5,6,7 become 8).
	''' For pixel sizes between 8 and 16 bits, <em>n</em> is equal to the
	''' pixel size.
	''' Pixel sizes larger than 16 bits are not supported by this class.
	''' Higher order bits beyond <em>n</em> are ignored in pixel representations.
	''' Index values greater than or equal to the map size, but less than
	''' 2<sup><em>n</em></sup>, are undefined and return 0 for all color and
	''' alpha components.
	''' </a>
	''' <p>
	''' For those methods that use a primitive array pixel representation of
	''' type <code>transferType</code>, the array length is always one.
	''' The transfer types supported are <code>DataBuffer.TYPE_BYTE</code> and
	''' <code>DataBuffer.TYPE_USHORT</code>.  A single int pixel
	''' representation is valid for all objects of this [Class], since it is
	''' always possible to represent pixel values used with this class in a
	''' single int.  Therefore, methods that use this representation do
	''' not throw an <code>IllegalArgumentException</code> due to an invalid
	''' pixel value.
	''' <p>
	''' Many of the methods in this class are final.  The reason for
	''' this is that the underlying native graphics code makes assumptions
	''' about the layout and operation of this class and those assumptions
	''' are reflected in the implementations of the methods here that are
	''' marked final.  You can subclass this class for other reasons, but
	''' you cannot override or modify the behaviour of those methods.
	''' </summary>
	''' <seealso cref= ColorModel </seealso>
	''' <seealso cref= ColorSpace </seealso>
	''' <seealso cref= DataBuffer
	'''  </seealso>
	Public Class IndexColorModel
		Inherits ColorModel

		Private rgb As Integer()
		Private map_size As Integer
		Private pixel_mask As Integer
		Private transparent_index As Integer = -1
		Private allgrayopaque As Boolean
		Private validBits As System.Numerics.BigInteger

		Private colorData As sun.awt.image.BufImgSurfaceData.ICMColorData = Nothing

		Private Shared opaqueBits As Integer() = {8, 8, 8}
		Private Shared alphaBits As Integer() = {8, 8, 8, 8}

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub
		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from the specified
		''' arrays of red, green, and blue components.  Pixels described
		''' by this color model all have alpha components of 255
		''' unnormalized (1.0&nbsp;normalized), which means they
		''' are fully opaque.  All of the arrays specifying the color
		''' components must have at least the specified number of entries.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' Since there is no alpha information in any of the arguments
		''' to this constructor, the transparency value is always
		''' <code>Transparency.OPAQUE</code>.
		''' The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		''' or <code>DataBuffer.TYPE_USHORT</code> that can hold a single pixel. </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="r">         the array of red color components </param>
		''' <param name="g">         the array of green color components </param>
		''' <param name="b">         the array of blue color components </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		'''         than 1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less
		'''         than 1 </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public IndexColorModel(int bits, int size, byte r() , byte g(), byte b())
			MyBase(bits, opaqueBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), False, False, OPAQUE, ColorModel.getDefaultTransferType(bits))
			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			rGBsGBs(size, r, g, b, Nothing)
			calculatePixelMask()

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from the given arrays
		''' of red, green, and blue components.  Pixels described by this color
		''' model all have alpha components of 255 unnormalized
		''' (1.0&nbsp;normalized), which means they are fully opaque, except
		''' for the indicated pixel to be made transparent.  All of the arrays
		''' specifying the color components must have at least the specified
		''' number of entries.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be <code>Transparency.OPAQUE</code> or
		''' <code>Transparency.BITMASK</code> depending on the arguments, as
		''' specified in the <a href="#transparency">class description</a> above.
		''' The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		''' or <code>DataBuffer.TYPE_USHORT</code> that can hold a
		''' single pixel. </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="r">         the array of red color components </param>
		''' <param name="g">         the array of green color components </param>
		''' <param name="b">         the array of blue color components </param>
		''' <param name="trans">     the index of the transparent pixel </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		'''          1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less than
		'''          1 </exception>
		public IndexColorModel(Integer bits, Integer size, SByte r() , SByte g(), SByte b(), Integer trans)
			MyBase(bits, opaqueBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), False, False, OPAQUE, ColorModel.getDefaultTransferType(bits))
			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			rGBsGBs(size, r, g, b, Nothing)
			transparentPixel = trans
			calculatePixelMask()

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from the given
		''' arrays of red, green, blue and alpha components.  All of the
		''' arrays specifying the components must have at least the specified
		''' number of entries.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be any of <code>Transparency.OPAQUE</code>,
		''' <code>Transparency.BITMASK</code>,
		''' or <code>Transparency.TRANSLUCENT</code>
		''' depending on the arguments, as specified
		''' in the <a href="#transparency">class description</a> above.
		''' The transfer type is the smallest of <code>DataBuffer.TYPE_BYTE</code>
		''' or <code>DataBuffer.TYPE_USHORT</code> that can hold a single pixel. </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="r">         the array of red color components </param>
		''' <param name="g">         the array of green color components </param>
		''' <param name="b">         the array of blue color components </param>
		''' <param name="a">         the array of alpha value components </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		'''           than 1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less
		'''           than 1 </exception>
		public IndexColorModel(Integer bits, Integer size, SByte r() , SByte g(), SByte b(), SByte a())
			MyBase(bits, alphaBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), True, False, TRANSLUCENT, ColorModel.getDefaultTransferType(bits))
			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			rGBsGBs(size, r, g, b, a)
			calculatePixelMask()

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from a single
		''' array of interleaved red, green, blue and optional alpha
		''' components.  The array must have enough values in it to
		''' fill all of the needed component arrays of the specified
		''' size.  The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be any of <code>Transparency.OPAQUE</code>,
		''' <code>Transparency.BITMASK</code>,
		''' or <code>Transparency.TRANSLUCENT</code>
		''' depending on the arguments, as specified
		''' in the <a href="#transparency">class description</a> above.
		''' The transfer type is the smallest of
		''' <code>DataBuffer.TYPE_BYTE</code> or <code>DataBuffer.TYPE_USHORT</code>
		''' that can hold a single pixel.
		''' </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="cmap">      the array of color components </param>
		''' <param name="start">     the starting offset of the first color component </param>
		''' <param name="hasalpha">  indicates whether alpha values are contained in
		'''                  the <code>cmap</code> array </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		'''           than 1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less
		'''           than 1 </exception>
		public IndexColorModel(Integer bits, Integer size, SByte cmap() , Integer start, Boolean hasalpha)
			Me(bits, size, cmap, start, hasalpha, -1)
			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from a single array of
		''' interleaved red, green, blue and optional alpha components.  The
		''' specified transparent index represents a pixel that is made
		''' entirely transparent regardless of any alpha value specified
		''' for it.  The array must have enough values in it to fill all
		''' of the needed component arrays of the specified size.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be any of <code>Transparency.OPAQUE</code>,
		''' <code>Transparency.BITMASK</code>,
		''' or <code>Transparency.TRANSLUCENT</code>
		''' depending on the arguments, as specified
		''' in the <a href="#transparency">class description</a> above.
		''' The transfer type is the smallest of
		''' <code>DataBuffer.TYPE_BYTE</code> or <code>DataBuffer.TYPE_USHORT</code>
		''' that can hold a single pixel. </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="cmap">      the array of color components </param>
		''' <param name="start">     the starting offset of the first color component </param>
		''' <param name="hasalpha">  indicates whether alpha values are contained in
		'''                  the <code>cmap</code> array </param>
		''' <param name="trans">     the index of the fully transparent pixel </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		'''               1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less than
		'''               1 </exception>
		public IndexColorModel(Integer bits, Integer size, SByte cmap() , Integer start, Boolean hasalpha, Integer trans)
			' REMIND: This assumes the ordering: RGB[A]
			MyBase(bits, opaqueBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), False, False, OPAQUE, ColorModel.getDefaultTransferType(bits))

			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			If size < 1 Then Throw New IllegalArgumentException("Map size (" & size & ") must be >= 1")
			map_size = size
			rgb = New Integer(calcRealMapSize(bits, size) - 1){}
			Dim j As Integer = start
			Dim alpha_Renamed As Integer = &Hff
			Dim allgray As Boolean = True
			Dim transparency_Renamed As Integer = OPAQUE
			For i As Integer = 0 To size - 1
				Dim r As Integer = cmap(j) And &Hff
				j += 1
				Dim g As Integer = cmap(j) And &Hff
				j += 1
				Dim b As Integer = cmap(j) And &Hff
				j += 1
				allgray = allgray AndAlso (r = g) AndAlso (g = b)
				If hasalpha Then
					alpha_Renamed = cmap(j) And &Hff
					j += 1
					If alpha_Renamed <> &Hff Then
						If alpha_Renamed = &H0 Then
							If transparency_Renamed = OPAQUE Then transparency_Renamed = BITMASK
							If transparent_index < 0 Then transparent_index = i
						Else
							transparency_Renamed = TRANSLUCENT
						End If
						allgray = False
					End If
				End If
				rgb(i) = (alpha_Renamed << 24) Or (r << 16) Or (g << 8) Or b
			Next i
			Me.allgrayopaque = allgray
			transparency = transparency_Renamed
			transparentPixel = trans
			calculatePixelMask()

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from an array of
		''' ints where each int is comprised of red, green, blue, and
		''' optional alpha components in the default RGB color model format.
		''' The specified transparent index represents a pixel that is made
		''' entirely transparent regardless of any alpha value specified
		''' for it.  The array must have enough values in it to fill all
		''' of the needed component arrays of the specified size.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be any of <code>Transparency.OPAQUE</code>,
		''' <code>Transparency.BITMASK</code>,
		''' or <code>Transparency.TRANSLUCENT</code>
		''' depending on the arguments, as specified
		''' in the <a href="#transparency">class description</a> above. </summary>
		''' <param name="bits">      the number of bits each pixel occupies </param>
		''' <param name="size">      the size of the color component arrays </param>
		''' <param name="cmap">      the array of color components </param>
		''' <param name="start">     the starting offset of the first color component </param>
		''' <param name="hasalpha">  indicates whether alpha values are contained in
		'''                  the <code>cmap</code> array </param>
		''' <param name="trans">     the index of the fully transparent pixel </param>
		''' <param name="transferType"> the data type of the array used to represent
		'''           pixel values.  The data type must be either
		'''           <code>DataBuffer.TYPE_BYTE</code> or
		'''           <code>DataBuffer.TYPE_USHORT</code>. </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		'''           than 1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less
		'''           than 1 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>transferType</code> is not
		'''           one of <code>DataBuffer.TYPE_BYTE</code> or
		'''           <code>DataBuffer.TYPE_USHORT</code> </exception>
		public IndexColorModel(Integer bits, Integer size, Integer cmap() , Integer start, Boolean hasalpha, Integer trans, Integer transferType)
			' REMIND: This assumes the ordering: RGB[A]
			MyBase(bits, opaqueBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), False, False, OPAQUE, transferType)

			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			If size < 1 Then Throw New IllegalArgumentException("Map size (" & size & ") must be >= 1")
			If (transferType <> DataBuffer.TYPE_BYTE) AndAlso (transferType <> DataBuffer.TYPE_USHORT) Then Throw New IllegalArgumentException("transferType must be either" & "DataBuffer.TYPE_BYTE or DataBuffer.TYPE_USHORT")

			rGBsGBs(size, cmap, start, hasalpha)
			transparentPixel = trans
			calculatePixelMask()

		''' <summary>
		''' Constructs an <code>IndexColorModel</code> from an
		''' <code>int</code> array where each <code>int</code> is
		''' comprised of red, green, blue, and alpha
		''' components in the default RGB color model format.
		''' The array must have enough values in it to fill all
		''' of the needed component arrays of the specified size.
		''' The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value may be any of <code>Transparency.OPAQUE</code>,
		''' <code>Transparency.BITMASK</code>,
		''' or <code>Transparency.TRANSLUCENT</code>
		''' depending on the arguments, as specified
		''' in the <a href="#transparency">class description</a> above.
		''' The transfer type must be one of <code>DataBuffer.TYPE_BYTE</code>
		''' <code>DataBuffer.TYPE_USHORT</code>.
		''' The <code>BigInteger</code> object specifies the valid/invalid pixels
		''' in the <code>cmap</code> array.  A pixel is valid if the
		''' <code>BigInteger</code> value at that index is set, and is invalid
		''' if the <code>BigInteger</code> bit  at that index is not set. </summary>
		''' <param name="bits"> the number of bits each pixel occupies </param>
		''' <param name="size"> the size of the color component array </param>
		''' <param name="cmap"> the array of color components </param>
		''' <param name="start"> the starting offset of the first color component </param>
		''' <param name="transferType"> the specified data type </param>
		''' <param name="validBits"> a <code>BigInteger</code> object.  If a bit is
		'''    set in the BigInteger, the pixel at that index is valid.
		'''    If a bit is not set, the pixel at that index
		'''    is considered invalid.  If null, all pixels are valid.
		'''    Only bits from 0 to the map size are considered. </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less
		'''           than 1 or greater than 16 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>size</code> is less
		'''           than 1 </exception>
		''' <exception cref="IllegalArgumentException"> if <code>transferType</code> is not
		'''           one of <code>DataBuffer.TYPE_BYTE</code> or
		'''           <code>DataBuffer.TYPE_USHORT</code>
		''' 
		''' @since 1.3 </exception>
		public IndexColorModel(Integer bits, Integer size, Integer cmap() , Integer start, Integer transferType, System.Numerics.BigInteger validBits)
			MyBase(bits, alphaBits, java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), True, False, TRANSLUCENT, transferType)

			If bits < 1 OrElse bits > 16 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 16.")
			If size < 1 Then Throw New IllegalArgumentException("Map size (" & size & ") must be >= 1")
			If (transferType <> DataBuffer.TYPE_BYTE) AndAlso (transferType <> DataBuffer.TYPE_USHORT) Then Throw New IllegalArgumentException("transferType must be either" & "DataBuffer.TYPE_BYTE or DataBuffer.TYPE_USHORT")

			If validBits IsNot Nothing Then
				' Check to see if it is all valid
				For i As Integer = 0 To size - 1
					If Not validBits.testBit(i) Then
						Me.validBits = validBits
						Exit For
					End If
				Next i
			End If

			rGBsGBs(size, cmap, start, True)
			calculatePixelMask()

		private  Sub  rGBsGBs(Integer size, SByte r() , SByte g(), SByte b(), SByte a())
			If size < 1 Then Throw New IllegalArgumentException("Map size (" & size & ") must be >= 1")
			map_size = size
			rgb = New Integer(calcRealMapSize(pixel_bits, size) - 1){}
			Dim alpha_Renamed As Integer = &Hff
			Dim transparency_Renamed As Integer = OPAQUE
			Dim allgray As Boolean = True
			For i As Integer = 0 To size - 1
				Dim rc As Integer = r(i) And &Hff
				Dim gc As Integer = g(i) And &Hff
				Dim bc As Integer = b(i) And &Hff
				allgray = allgray AndAlso (rc = gc) AndAlso (gc = bc)
				If a IsNot Nothing Then
					alpha_Renamed = a(i) And &Hff
					If alpha_Renamed <> &Hff Then
						If alpha_Renamed = &H0 Then
							If transparency_Renamed = OPAQUE Then transparency_Renamed = BITMASK
							If transparent_index < 0 Then transparent_index = i
						Else
							transparency_Renamed = TRANSLUCENT
						End If
						allgray = False
					End If
				End If
				rgb(i) = (alpha_Renamed << 24) Or (rc << 16) Or (gc << 8) Or bc
			Next i
			Me.allgrayopaque = allgray
			transparency = transparency_Renamed

		private  Sub  rGBsGBs(Integer size, Integer cmap() , Integer start, Boolean hasalpha)
			map_size = size
			rgb = New Integer(calcRealMapSize(pixel_bits, size) - 1){}
			Dim j As Integer = start
			Dim transparency_Renamed As Integer = OPAQUE
			Dim allgray As Boolean = True
			Dim validBits As System.Numerics.BigInteger = Me.validBits
			Dim i As Integer = 0
			Do While i < size
				If validBits IsNot Nothing AndAlso (Not validBits.testBit(i)) Then
					i += 1
				j += 1
					Continue Do
				End If
				Dim cmaprgb As Integer = cmap(j)
				Dim r As Integer = (cmaprgb >> 16) And &Hff
				Dim g As Integer = (cmaprgb >> 8) And &Hff
				Dim b As Integer = (cmaprgb) And &Hff
				allgray = allgray AndAlso (r = g) AndAlso (g = b)
				If hasalpha Then
					Dim alpha_Renamed As Integer = CInt(CUInt(cmaprgb) >> 24)
					If alpha_Renamed <> &Hff Then
						If alpha_Renamed = &H0 Then
							If transparency_Renamed = OPAQUE Then transparency_Renamed = BITMASK
							If transparent_index < 0 Then transparent_index = i
						Else
							transparency_Renamed = TRANSLUCENT
						End If
						allgray = False
					End If
				Else
					cmaprgb = cmaprgb Or &Hff000000L
				End If
				rgb(i) = cmaprgb
				i += 1
				j += 1
			Loop
			Me.allgrayopaque = allgray
			transparency = transparency_Renamed

		private Integer calcRealMapSize(Integer bits, Integer size)
			Dim newSize As Integer = System.Math.Max(1 << bits, size)
			Return System.Math.Max(newSize, 256)

		private System.Numerics.BigInteger allValid
			Dim numbytes As Integer = (map_size+7)\8
			Dim valid_Renamed As SByte() = New SByte(numbytes - 1){}
			Arrays.fill(valid_Renamed, CByte(&Hff))
			valid_Renamed(0) = CByte(&CInt(CUInt(Hff) >> (numbytes*8 - map_size)))

			Return New System.Numerics.BigInteger(1, valid_Renamed)

		''' <summary>
		''' Returns the transparency.  Returns either OPAQUE, BITMASK,
		''' or TRANSLUCENT </summary>
		''' <returns> the transparency of this <code>IndexColorModel</code> </returns>
		''' <seealso cref= Transparency#OPAQUE </seealso>
		''' <seealso cref= Transparency#BITMASK </seealso>
		''' <seealso cref= Transparency#TRANSLUCENT </seealso>
		public Integer transparency
			Return transparency

		''' <summary>
		''' Returns an array of the number of bits for each color/alpha component.
		''' The array contains the color components in the order red, green,
		''' blue, followed by the alpha component, if present. </summary>
		''' <returns> an array containing the number of bits of each color
		'''         and alpha component of this <code>IndexColorModel</code> </returns>
		public Integer() componentSize
			If nBits Is Nothing Then
				If supportsAlpha Then
					nBits = New Integer(3){}
					nBits(3) = 8
				Else
					nBits = New Integer(2){}
				End If
					nBits(2) = 8
						nBits(1) = nBits(2)
						nBits(0) = nBits(1)
			End If
			Return nBits.clone()

		''' <summary>
		''' Returns the size of the color/alpha component arrays in this
		''' <code>IndexColorModel</code>. </summary>
		''' <returns> the size of the color and alpha component arrays. </returns>
		public final Integer mapSize
			Return map_size

		''' <summary>
		''' Returns the index of a transparent pixel in this
		''' <code>IndexColorModel</code> or -1 if there is no pixel
		''' with an alpha value of 0.  If a transparent pixel was
		''' explicitly specified in one of the constructors by its
		''' index, then that index will be preferred, otherwise,
		''' the index of any pixel which happens to be fully transparent
		''' may be returned. </summary>
		''' <returns> the index of a transparent pixel in this
		'''         <code>IndexColorModel</code> object, or -1 if there
		'''         is no such pixel </returns>
		public final Integer transparentPixel
			Return transparent_index

		''' <summary>
		''' Copies the array of red color components into the specified array.
		''' Only the initial entries of the array as specified by
		''' <seealso cref="#getMapSize() getMapSize"/> are written. </summary>
		''' <param name="r"> the specified array into which the elements of the
		'''      array of red color components are copied </param>
		public final  Sub  getReds(SByte r())
			For i As Integer = 0 To map_size - 1
				r(i) = CByte(rgb(i) >> 16)
			Next i

		''' <summary>
		''' Copies the array of green color components into the specified array.
		''' Only the initial entries of the array as specified by
		''' <code>getMapSize</code> are written. </summary>
		''' <param name="g"> the specified array into which the elements of the
		'''      array of green color components are copied </param>
		public final  Sub  getGreens(SByte g())
			For i As Integer = 0 To map_size - 1
				g(i) = CByte(rgb(i) >> 8)
			Next i

		''' <summary>
		''' Copies the array of blue color components into the specified array.
		''' Only the initial entries of the array as specified by
		''' <code>getMapSize</code> are written. </summary>
		''' <param name="b"> the specified array into which the elements of the
		'''      array of blue color components are copied </param>
		public final  Sub  getBlues(SByte b())
			For i As Integer = 0 To map_size - 1
				b(i) = CByte(rgb(i))
			Next i

		''' <summary>
		''' Copies the array of alpha transparency components into the
		''' specified array.  Only the initial entries of the array as specified
		''' by <code>getMapSize</code> are written. </summary>
		''' <param name="a"> the specified array into which the elements of the
		'''      array of alpha components are copied </param>
		public final  Sub  getAlphas(SByte a())
			For i As Integer = 0 To map_size - 1
				a(i) = CByte(rgb(i) >> 24)
			Next i

		''' <summary>
		''' Converts data for each index from the color and alpha component
		''' arrays to an int in the default RGB ColorModel format and copies
		''' the resulting 32-bit ARGB values into the specified array.  Only
		''' the initial entries of the array as specified by
		''' <code>getMapSize</code> are
		''' written. </summary>
		''' <param name="rgb"> the specified array into which the converted ARGB
		'''        values from this array of color and alpha components
		'''        are copied. </param>
		public final  Sub  getRGBs(Integer rgb())
			Array.Copy(Me.rgb, 0, rgb, 0, map_size)

		private  Sub  transparentPixelxel(Integer trans)
			If trans >= 0 AndAlso trans < map_size Then
				rgb(trans) = rgb(trans) And &Hffffff
				transparent_index = trans
				allgrayopaque = False
				If Me.transparency = OPAQUE Then transparency = BITMASK
			End If

		private  Sub  transparencyncy(Integer transparency)
			If Me.transparency <> transparency Then
				Me.transparency = transparency
				If transparency = OPAQUE Then
					supportsAlpha = False
					numComponents = 3
					nBits = opaqueBits
				Else
					supportsAlpha = True
					numComponents = 4
					nBits = alphaBits
				End If
			End If

		''' <summary>
		''' This method is called from the constructors to set the pixel_mask
		''' value, which is based on the value of pixel_bits.  The pixel_mask
		''' value is used to mask off the pixel parameters for methods such
		''' as getRed(), getGreen(), getBlue(), getAlpha(), and getRGB().
		''' </summary>
		private final  Sub  calculatePixelMask()
			' Note that we adjust the mask so that our masking behavior here
			' is consistent with that of our native rendering loops.
			Dim maskbits As Integer = pixel_bits
			If maskbits = 3 Then
				maskbits = 4
			ElseIf maskbits > 4 AndAlso maskbits < 8 Then
				maskbits = 8
			End If
			pixel_mask = (1 << maskbits) - 1

		''' <summary>
		''' Returns the red color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		''' is specified as an int.
		''' Only the lower <em>n</em> bits of the pixel value, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' calculate the returned value.
		''' The returned value is a non pre-multiplied value. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the value of the red color component for the specified pixel </returns>
		public final Integer getRed(Integer pixel)
			Return (rgb(pixel And pixel_mask) >> 16) And &Hff

		''' <summary>
		''' Returns the green color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		''' is specified as an int.
		''' Only the lower <em>n</em> bits of the pixel value, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' calculate the returned value.
		''' The returned value is a non pre-multiplied value. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the value of the green color component for the specified pixel </returns>
		public final Integer getGreen(Integer pixel)
			Return (rgb(pixel And pixel_mask) >> 8) And &Hff

		''' <summary>
		''' Returns the blue color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  The pixel value
		''' is specified as an int.
		''' Only the lower <em>n</em> bits of the pixel value, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' calculate the returned value.
		''' The returned value is a non pre-multiplied value. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the value of the blue color component for the specified pixel </returns>
		public final Integer getBlue(Integer pixel)
			Return rgb(pixel And pixel_mask) And &Hff

		''' <summary>
		''' Returns the alpha component for the specified pixel, scaled
		''' from 0 to 255.  The pixel value is specified as an int.
		''' Only the lower <em>n</em> bits of the pixel value, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' calculate the returned value. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the value of the alpha component for the specified pixel </returns>
		public final Integer getAlpha(Integer pixel)
			Return (rgb(pixel And pixel_mask) >> 24) And &Hff

		''' <summary>
		''' Returns the color/alpha components of the pixel in the default
		''' RGB color model format.  The pixel value is specified as an int.
		''' Only the lower <em>n</em> bits of the pixel value, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' calculate the returned value.
		''' The returned value is in a non pre-multiplied format. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the color and alpha components of the specified pixel </returns>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		public final Integer getRGB(Integer pixel)
			Return rgb(pixel And pixel_mask)

		private static final Integer CACHESIZE = 40
		private Integer lookupcache() = New Integer(CACHESIZE - 1){}

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' ColorModel, given an integer pixel representation in the
		''' default RGB color model.  This array can then be passed to the
		''' <seealso cref="WritableRaster#setDataElements(int, int, java.lang.Object) setDataElements"/>
		''' method of a <seealso cref="WritableRaster"/> object.  If the pixel variable is
		''' <code>null</code>, a new array is allocated.  If <code>pixel</code>
		''' is not <code>null</code>, it must be
		''' a primitive array of type <code>transferType</code>; otherwise, a
		''' <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>pixel</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.  The pixel array is returned.
		''' <p>
		''' Since <code>IndexColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>.
		''' </summary>
		''' <param name="rgb"> the integer pixel representation in the default RGB
		''' color model </param>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> an array representation of the specified pixel in this
		'''  <code>IndexColorModel</code>. </returns>
		''' <exception cref="ClassCastException"> if <code>pixel</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>pixel</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		'''         is invalid </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		Public Function getDataElements(rgb As Integer, pixel As Object) As Object
            Dim red_Renamed As Integer = (rgb >> 16) And &HFF
            Dim green_Renamed As Integer = (rgb >> 8) And &HFF
            Dim blue_Renamed As Integer = rgb And &HFF
            Dim alpha_Renamed As Integer = (CInt(CUInt(rgb) >> 24))
            Dim pix As Integer = 0

            ' Note that pixels are stored at lookupcache[2*i]
            ' and the rgb that was searched is stored at
            ' lookupcache[2*i+1].  Also, the pixel is first
            ' inverted using the unary complement operator
            ' before storing in the cache so it can never be 0.
            For i As Integer = CACHESIZE - 2 To 0 Step -2
                pix = lookupcache(i)
                If pix = 0 Then Exit For
                If rgb = lookupcache(i + 1) Then Return installpixel(pixel, (Not pix))
            Next i

            If allgrayopaque Then
                ' IndexColorModel objects are all tagged as
                ' non-premultiplied so ignore the alpha value
                ' of the incoming color, convert the
                ' non-premultiplied color components to a
                ' grayscale value and search for the closest
                ' gray value in the palette.  Since all colors
                ' in the palette are gray, we only need compare
                ' to one of the color components for a match
                ' using a simple linear distance formula.

                Dim minDist As Integer = 256
                Dim d As Integer
                Dim gray As Integer = CInt(red_Renamed * 77 + green_Renamed * 150 + blue_Renamed * 29 + 128) \ 256

                For i As Integer = 0 To map_size - 1
                    If Me.rgb(i) = &H0 Then Continue For
                    d = (Me.rgb(i) And &HFF) - gray
                    If d < 0 Then d = -d
                    If d < minDist Then
                        pix = i
                        If d = 0 Then Exit For
                        minDist = d
                    End If
                Next i
            ElseIf transparency = OPAQUE Then
                ' IndexColorModel objects are all tagged as
                ' non-premultiplied so ignore the alpha value
                ' of the incoming color and search for closest
                ' color match independently using a 3 component
                ' Euclidean distance formula.
                ' For opaque colormaps, palette entries are 0
                ' iff they are an invalid color and should be
                ' ignored during color searches.
                ' As an optimization, exact color searches are
                ' likely to be fairly common in opaque colormaps
                ' so first we will do a quick search for an
                ' exact match.

                Dim smallestError As Integer = java.lang.[Integer].Max_Value
                Dim lut As Integer() = Me.rgb
                Dim lutrgb As Integer
                For i As Integer = 0 To map_size - 1
                    lutrgb = lut(i)
                    If lutrgb = rgb AndAlso lutrgb <> 0 Then
                        pix = i
                        smallestError = 0
                        Exit For
                    End If
                Next i

                If smallestError <> 0 Then
                    For i As Integer = 0 To map_size - 1
                        lutrgb = lut(i)
                        If lutrgb = 0 Then Continue For

                        Dim tmp As Integer = ((lutrgb >> 16) And &HFF) - red_Renamed
                        Dim currentError As Integer = tmp * tmp
                        If currentError < smallestError Then
                            tmp = ((lutrgb >> 8) And &HFF) - green_Renamed
                            currentError += tmp * tmp
                            If currentError < smallestError Then
                                tmp = (lutrgb And &HFF) - blue_Renamed
                                currentError += tmp * tmp
                                If currentError < smallestError Then
                                    pix = i
                                    smallestError = currentError
                                End If
                            End If
                        End If
                    Next i
                End If
            ElseIf alpha_Renamed = 0 AndAlso transparent_index >= 0 Then
                ' Special case - transparent color maps to the
                ' specified transparent pixel, if there is one

                pix = transparent_index
            Else
                ' IndexColorModel objects are all tagged as
                ' non-premultiplied so use non-premultiplied
                ' color components in the distance calculations.
                ' Look for closest match using a 4 component
                ' Euclidean distance formula.

                Dim smallestError As Integer = java.lang.[Integer].Max_Value
                Dim lut As Integer() = Me.rgb
                For i As Integer = 0 To map_size - 1
                    Dim lutrgb As Integer = lut(i)
                    If lutrgb = rgb Then
                        If validBits IsNot Nothing AndAlso (Not validBits.testBit(i)) Then Continue For
                        pix = i
                        Exit For
                    End If

                    Dim tmp As Integer = ((lutrgb >> 16) And &HFF) - red_Renamed
                    Dim currentError As Integer = tmp * tmp
                    If currentError < smallestError Then
                        tmp = ((lutrgb >> 8) And &HFF) - green_Renamed
                        currentError += tmp * tmp
                        If currentError < smallestError Then
                            tmp = (lutrgb And &HFF) - blue_Renamed
                            currentError += tmp * tmp
                            If currentError < smallestError Then
                                tmp = (CInt(CUInt(lutrgb) >> 24)) - alpha_Renamed
                                currentError += tmp * tmp
                                If currentError < smallestError AndAlso (validBits Is Nothing OrElse validBits.testBit(i)) Then
                                    pix = i
                                    smallestError = currentError
                                End If
                            End If
                        End If
                    End If
                Next i
            End If
            Array.Copy(lookupcache, 2, lookupcache, 0, CACHESIZE - 2)
            lookupcache(CACHESIZE - 1) = rgb
            lookupcache(CACHESIZE - 2) = Not pix
            Return installpixel(pixel, pix)
        End Function
        Private Function installpixel(pixel As Object, pix As Integer) As Object
            Select Case transferType
                Case DataBuffer.TYPE_INT
                    Dim intObj As Integer()
                    If pixel Is Nothing Then
                        intObj = New Integer(0) {}
                        pixel = intObj
                    Else
                        intObj = CType(pixel, Integer())
                    End If
                    intObj(0) = pix
                Case DataBuffer.TYPE_BYTE
                    Dim byteObj As SByte()
                    If pixel Is Nothing Then
                        byteObj = New SByte(0) {}
                        pixel = byteObj
                    Else
                        byteObj = CType(pixel, SByte())
                    End If
                    byteObj(0) = CByte(pix)
                Case DataBuffer.TYPE_USHORT
                    Dim shortObj As Short()
                    If pixel Is Nothing Then
                        shortObj = New Short(0) {}
                        pixel = shortObj
                    Else
                        shortObj = CType(pixel, Short())
                    End If
                    shortObj(0) = CShort(Fix(pix))
                Case Else
                    Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
            End Select
            Return pixel
        End Function
        ''' <summary>
        ''' Returns an array of unnormalized color/alpha components for a
        ''' specified pixel in this <code>ColorModel</code>.  The pixel value
        ''' is specified as an int.  If the <code>components</code> array is <code>null</code>,
        ''' a new array is allocated that contains
        ''' <code>offset + getNumComponents()</code> elements.
        ''' The <code>components</code> array is returned,
        ''' with the alpha component included
        ''' only if <code>hasAlpha</code> returns true.
        ''' Color/alpha components are stored in the <code>components</code> array starting
        ''' at <code>offset</code> even if the array is allocated by this method.
        ''' An <code>ArrayIndexOutOfBoundsException</code>
        ''' is thrown if  the <code>components</code> array is not <code>null</code> and is
        ''' not large enough to hold all the color and alpha components
        ''' starting at <code>offset</code>. </summary>
        ''' <param name="pixel"> the specified pixel </param>
        ''' <param name="components"> the array to receive the color and alpha
        ''' components of the specified pixel </param>
        ''' <param name="offset"> the offset into the <code>components</code> array at
        ''' which to start storing the color and alpha components </param>
        ''' <returns> an array containing the color and alpha components of the
        ''' specified pixel starting at the specified offset. </returns>
        ''' <seealso cref= ColorModel#hasAlpha </seealso>
        ''' <seealso cref= ColorModel#getNumComponents </seealso>
        Public Function getComponents(pixel As Integer, components As Integer(), offset As Integer) As Integer()
            If components Is Nothing Then components = New Integer(offset + numComponents - 1) {}

            ' REMIND: Needs to change if different color space
            components(offset + 0) = getRed(pixel)
            components(offset + 1) = getGreen(pixel)
            components(offset + 2) = getBlue(pixel)
            If supportsAlpha AndAlso (components.Length - offset) > 3 Then components(offset + 3) = getAlpha(pixel)

            Return components
        End Function
        ''' <summary>
        ''' Returns an array of unnormalized color/alpha components for
        ''' a specified pixel in this <code>ColorModel</code>.  The pixel
        ''' value is specified by an array of data elements of type
        ''' <code>transferType</code> passed in as an object reference.
        ''' If <code>pixel</code> is not a primitive array of type
        ''' <code>transferType</code>, a <code>ClassCastException</code>
        ''' is thrown.  An <code>ArrayIndexOutOfBoundsException</code>
        ''' is thrown if <code>pixel</code> is not large enough to hold
        ''' a pixel value for this <code>ColorModel</code>.  If the
        ''' <code>components</code> array is <code>null</code>, a new array
        ''' is allocated that contains
        ''' <code>offset + getNumComponents()</code> elements.
        ''' The <code>components</code> array is returned,
        ''' with the alpha component included
        ''' only if <code>hasAlpha</code> returns true.
        ''' Color/alpha components are stored in the <code>components</code>
        ''' array starting at <code>offset</code> even if the array is
        ''' allocated by this method.  An
        ''' <code>ArrayIndexOutOfBoundsException</code> is also
        ''' thrown if  the <code>components</code> array is not
        ''' <code>null</code> and is not large enough to hold all the color
        ''' and alpha components starting at <code>offset</code>.
        ''' <p>
        ''' Since <code>IndexColorModel</code> can be subclassed, subclasses
        ''' inherit the implementation of this method and if they don't
        ''' override it then they throw an exception if they use an
        ''' unsupported <code>transferType</code>.
        ''' </summary>
        ''' <param name="pixel"> the specified pixel </param>
        ''' <param name="components"> an array that receives the color and alpha
        ''' components of the specified pixel </param>
        ''' <param name="offset"> the index into the <code>components</code> array at
        ''' which to begin storing the color and alpha components of the
        ''' specified pixel </param>
        ''' <returns> an array containing the color and alpha components of the
        ''' specified pixel starting at the specified offset. </returns>
        ''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>pixel</code>
        '''            is not large enough to hold a pixel value for this
        '''            <code>ColorModel</code> or if the
        '''            <code>components</code> array is not <code>null</code>
        '''            and is not large enough to hold all the color
        '''            and alpha components starting at <code>offset</code> </exception>
        ''' <exception cref="ClassCastException"> if <code>pixel</code> is not a
        '''            primitive array of type <code>transferType</code> </exception>
        ''' <exception cref="UnsupportedOperationException"> if <code>transferType</code>
        '''         is not one of the supported transfer types </exception>
        ''' <seealso cref= ColorModel#hasAlpha </seealso>
        ''' <seealso cref= ColorModel#getNumComponents </seealso>
        Public Integer() getComponents(Object pixel, Integer() components, Integer offset)
			Dim intpixel As Integer
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(pixel, SByte())
				   intpixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(pixel, Short())
				   intpixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(pixel, Integer())
				   intpixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getComponents(intpixel, components, offset)

		''' <summary>
		''' Returns a pixel value represented as an int in this
		''' <code>ColorModel</code> given an array of unnormalized
		''' color/alpha components.  An
		''' <code>ArrayIndexOutOfBoundsException</code>
		''' is thrown if the <code>components</code> array is not large
		''' enough to hold all of the color and alpha components starting
		''' at <code>offset</code>.  Since
		''' <code>ColorModel</code> can be subclassed, subclasses inherit the
		''' implementation of this method and if they don't override it then
		''' they throw an exception if they use an unsupported transferType. </summary>
		''' <param name="components"> an array of unnormalized color and alpha
		''' components </param>
		''' <param name="offset"> the index into <code>components</code> at which to
		''' begin retrieving the color and alpha components </param>
		''' <returns> an <code>int</code> pixel value in this
		''' <code>ColorModel</code> corresponding to the specified components. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  the <code>components</code> array is not large enough to
		'''  hold all of the color and alpha components starting at
		'''  <code>offset</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		'''         is invalid </exception>
		public Integer getDataElement(Integer() components, Integer offset)
			Dim rgb_Renamed As Integer = (components(offset+0)<<16) Or (components(offset+1)<<8) Or (components(offset+2))
			If supportsAlpha Then
				rgb_Renamed = rgb_Renamed Or (components(offset+3)<<24)
			Else
				rgb_Renamed = rgb_Renamed Or &Hff000000L
			End If
			Dim inData As Object = getDataElements(rgb_Renamed, Nothing)
			Dim pixel As Integer
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0)
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return pixel

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <code>ColorModel</code> given an array of unnormalized color/alpha
		''' components.  This array can then be passed to the
		''' <code>setDataElements</code> method of a <code>WritableRaster</code>
		''' object.  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if the
		''' <code>components</code> array is not large enough to hold all of the
		''' color and alpha components starting at <code>offset</code>.
		''' If the pixel variable is <code>null</code>, a new array
		''' is allocated.  If <code>pixel</code> is not <code>null</code>,
		''' it must be a primitive array of type <code>transferType</code>;
		''' otherwise, a <code>ClassCastException</code> is thrown.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if pixel
		''' is not large enough to hold a pixel value for this
		''' <code>ColorModel</code>.
		''' <p>
		''' Since <code>IndexColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>
		''' </summary>
		''' <param name="components"> an array of unnormalized color and alpha
		''' components </param>
		''' <param name="offset"> the index into <code>components</code> at which to
		''' begin retrieving color and alpha components </param>
		''' <param name="pixel"> the <code>Object</code> representing an array of color
		''' and alpha components </param>
		''' <returns> an <code>Object</code> representing an array of color and
		''' alpha components. </returns>
		''' <exception cref="ClassCastException"> if <code>pixel</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>pixel</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> or the <code>components</code>
		'''  array is not large enough to hold all of the color and alpha
		'''  components starting at <code>offset</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if <code>transferType</code>
		'''         is not one of the supported transfer types </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		public Object getDataElements(Integer() components, Integer offset, Object pixel)
			Dim rgb_Renamed As Integer = (components(offset+0)<<16) Or (components(offset+1)<<8) Or (components(offset+2))
			If supportsAlpha Then
				rgb_Renamed = rgb_Renamed Or (components(offset+3)<<24)
			Else
				rgb_Renamed = rgb_Renamed And &Hff000000L
			End If
			Return getDataElements(rgb_Renamed, pixel)

		''' <summary>
		''' Creates a <code>WritableRaster</code> with the specified width
		''' and height that has a data layout (<code>SampleModel</code>)
		''' compatible with this <code>ColorModel</code>.  This method
		''' only works for color models with 16 or fewer bits per pixel.
		''' <p>
		''' Since <code>IndexColorModel</code> can be subclassed, any
		''' subclass that supports greater than 16 bits per pixel must
		''' override this method.
		''' </summary>
		''' <param name="w"> the width to apply to the new <code>WritableRaster</code> </param>
		''' <param name="h"> the height to apply to the new <code>WritableRaster</code> </param>
		''' <returns> a <code>WritableRaster</code> object with the specified
		''' width and height. </returns>
		''' <exception cref="UnsupportedOperationException"> if the number of bits in a
		'''         pixel is greater than 16 </exception>
		''' <seealso cref= WritableRaster </seealso>
		''' <seealso cref= SampleModel </seealso>
		public WritableRaster createCompatibleWritableRaster(Integer w, Integer h)
			Dim raster_Renamed As WritableRaster

			If pixel_bits = 1 OrElse pixel_bits = 2 OrElse pixel_bits = 4 Then
				' TYPE_BINARY
				raster_Renamed = Raster.createPackedRaster(DataBuffer.TYPE_BYTE, w, h, 1, pixel_bits, Nothing)
			ElseIf pixel_bits <= 8 Then
				raster_Renamed = Raster.createInterleavedRaster(DataBuffer.TYPE_BYTE, w,h,1,Nothing)
			ElseIf pixel_bits <= 16 Then
				raster_Renamed = Raster.createInterleavedRaster(DataBuffer.TYPE_USHORT, w,h,1,Nothing)
			Else
				Throw New UnsupportedOperationException("This method is not supported " & " for pixel bits > 16.")
			End If
			Return raster_Renamed

		''' <summary>
		''' Returns <code>true</code> if <code>raster</code> is compatible
		''' with this <code>ColorModel</code> or <code>false</code> if it
		''' is not compatible with this <code>ColorModel</code>. </summary>
		''' <param name="raster"> the <seealso cref="Raster"/> object to test for compatibility </param>
		''' <returns> <code>true</code> if <code>raster</code> is compatible
		''' with this <code>ColorModel</code>; <code>false</code> otherwise.
		'''   </returns>
		public Boolean isCompatibleRaster(Raster raster)

			Dim size As Integer = raster.sampleModel.getSampleSize(0)
			Return ((raster.transferType = transferType) AndAlso (raster.numBands = 1) AndAlso ((1 << size) >= map_size))

		''' <summary>
		''' Creates a <code>SampleModel</code> with the specified
		''' width and height that has a data layout compatible with
		''' this <code>ColorModel</code>. </summary>
		''' <param name="w"> the width to apply to the new <code>SampleModel</code> </param>
		''' <param name="h"> the height to apply to the new <code>SampleModel</code> </param>
		''' <returns> a <code>SampleModel</code> object with the specified
		''' width and height. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <seealso cref= SampleModel </seealso>
		public SampleModel createCompatibleSampleModel(Integer w, Integer h)
			Dim [off] As Integer() = New Integer(0){}
			[off](0) = 0
			If pixel_bits = 1 OrElse pixel_bits = 2 OrElse pixel_bits = 4 Then
				Return New MultiPixelPackedSampleModel(transferType, w, h, pixel_bits)
			Else
				Return New ComponentSampleModel(transferType, w, h, 1, w, [off])
			End If

		''' <summary>
		''' Checks if the specified <code>SampleModel</code> is compatible
		''' with this <code>ColorModel</code>.  If <code>sm</code> is
		''' <code>null</code>, this method returns <code>false</code>. </summary>
		''' <param name="sm"> the specified <code>SampleModel</code>,
		'''           or <code>null</code> </param>
		''' <returns> <code>true</code> if the specified <code>SampleModel</code>
		''' is compatible with this <code>ColorModel</code>; <code>false</code>
		''' otherwise. </returns>
		''' <seealso cref= SampleModel </seealso>
		public Boolean isCompatibleSampleModel(SampleModel sm)
			' fix 4238629
			If Not(TypeOf sm Is ComponentSampleModel) AndAlso Not(TypeOf sm Is MultiPixelPackedSampleModel) Then Return False

			' Transfer type must be the same
			If sm.transferType <> transferType Then Return False

			If sm.numBands <> 1 Then Return False

			Return True

		''' <summary>
		''' Returns a new <code>BufferedImage</code> of TYPE_INT_ARGB or
		''' TYPE_INT_RGB that has a <code>Raster</code> with pixel data
		''' computed by expanding the indices in the source <code>Raster</code>
		''' using the color/alpha component arrays of this <code>ColorModel</code>.
		''' Only the lower <em>n</em> bits of each index value in the source
		''' <code>Raster</code>, as specified in the
		''' <a href="#index_values">class description</a> above, are used to
		''' compute the color/alpha values in the returned image.
		''' If <code>forceARGB</code> is <code>true</code>, a TYPE_INT_ARGB image is
		''' returned regardless of whether or not this <code>ColorModel</code>
		''' has an alpha component array or a transparent pixel. </summary>
		''' <param name="raster"> the specified <code>Raster</code> </param>
		''' <param name="forceARGB"> if <code>true</code>, the returned
		'''     <code>BufferedImage</code> is TYPE_INT_ARGB; otherwise it is
		'''     TYPE_INT_RGB </param>
		''' <returns> a <code>BufferedImage</code> created with the specified
		'''     <code>Raster</code> </returns>
		''' <exception cref="IllegalArgumentException"> if the raster argument is not
		'''           compatible with this IndexColorModel </exception>
		public BufferedImage convertToIntDiscrete(Raster raster, Boolean forceARGB)
			Dim cm As ColorModel

			If Not isCompatibleRaster(raster) Then Throw New IllegalArgumentException("This raster is not compatible" & "with this IndexColorModel.")
			If forceARGB OrElse transparency = TRANSLUCENT Then
				cm = ColorModel.rGBdefault
			ElseIf transparency = BITMASK Then
				cm = New DirectColorModel(25, &Hff0000, &Hff00, &Hff, &H1000000)
			Else
				cm = New DirectColorModel(24, &Hff0000, &Hff00, &Hff)
			End If

			Dim w As Integer = raster.width
			Dim h As Integer = raster.height
			Dim discreteRaster As WritableRaster = cm.createCompatibleWritableRaster(w, h)
			Dim obj As Object = Nothing
			Dim data As Integer() = Nothing

			Dim rX As Integer = raster.minX
			Dim rY As Integer = raster.minY

			Dim y As Integer=0
			Do While y < h
				obj = raster.getDataElements(rX, rY, w, 1, obj)
				If TypeOf obj Is Integer() Then
					data = CType(obj, Integer())
				Else
					data = DataBuffer.toIntArray(obj)
				End If
				For x As Integer = 0 To w - 1
					data(x) = rgb(data(x) And pixel_mask)
				Next x
				discreteRaster.dataElementsnts(0, y, w, 1, data)
				y += 1
				rY += 1
			Loop

			Return New BufferedImage(cm, discreteRaster, False, Nothing)

		''' <summary>
		''' Returns whether or not the pixel is valid. </summary>
		''' <param name="pixel"> the specified pixel value </param>
		''' <returns> <code>true</code> if <code>pixel</code>
		''' is valid; <code>false</code> otherwise.
		''' @since 1.3 </returns>
		public Boolean isValid(Integer pixel)
			Return ((pixel >= 0 AndAlso pixel < map_size) AndAlso (validBits Is Nothing OrElse validBits.testBit(pixel)))

		''' <summary>
		''' Returns whether or not all of the pixels are valid. </summary>
		''' <returns> <code>true</code> if all pixels are valid;
		''' <code>false</code> otherwise.
		''' @since 1.3 </returns>
		public Boolean valid
			Return (validBits Is Nothing)

        ''' <summary>
        ''' Returns a <code>BigInteger</code> that indicates the valid/invalid
        ''' pixels in the colormap.  A bit is valid if the
        ''' <code>BigInteger</code> value at that index is set, and is invalid
        ''' if the <code>BigInteger</code> value at that index is not set.
        ''' The only valid ranges to query in the <code>BigInteger</code> are
        ''' between 0 and the map size. </summary>
        ''' <returns> a <code>BigInteger</code> indicating the valid/invalid pixels.
        ''' @since 1.3 </returns>
        Public Function validPixels() As System.Numerics.BigInteger
            If validBits Is Nothing Then
                Return allValid
            Else
                Return validBits
            End If
        End Function
        ''' <summary>
        ''' Disposes of system resources associated with this
        ''' <code>ColorModel</code> once this <code>ColorModel</code> is no
        ''' longer referenced.
        ''' </summary>
        Public Sub Finalize()
        End Sub
        ''' <summary>
        ''' Returns the <code>String</code> representation of the contents of
        ''' this <code>ColorModel</code>object. </summary>
        ''' <returns> a <code>String</code> representing the contents of this
        ''' <code>ColorModel</code> object. </returns>
        Public Function ToString() As String
            Return New String("IndexColorModel: #pixelBits = " & pixel_bits & " numComponents = " & numComponents & " color space = " & colorSpace & " transparency = " & transparency & " transIndex   = " & transparent_index & " has alpha = " & supportsAlpha & " isAlphaPre = " & isAlphaPremultiplied_Renamed)
        End Function
    End Class

End Namespace