Imports Microsoft.VisualBasic

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

Namespace java.awt.image


	''' <summary>
	''' The <code>PackedColorModel</code> class is an abstract
	''' <seealso cref="ColorModel"/> class that works with pixel values which represent
	''' color and alpha information as separate samples and which pack all
	''' samples for a single pixel into a single int, short, or byte quantity.
	''' This class can be used with an arbitrary <seealso cref="ColorSpace"/>.  The number of
	''' color samples in the pixel values must be the same as the number of color
	''' components in the <code>ColorSpace</code>.  There can be a single alpha
	''' sample.  The array length is always 1 for those methods that use a
	''' primitive array pixel representation of type <code>transferType</code>.
	''' The transfer types supported are DataBuffer.TYPE_BYTE,
	''' DataBuffer.TYPE_USHORT, and DataBuffer.TYPE_INT.
	''' Color and alpha samples are stored in the single element of the array
	''' in bits indicated by bit masks.  Each bit mask must be contiguous and
	''' masks must not overlap.  The same masks apply to the single int
	''' pixel representation used by other methods.  The correspondence of
	''' masks and color/alpha samples is as follows:
	''' <ul>
	''' <li> Masks are identified by indices running from 0 through
	''' <seealso cref="ColorModel#getNumComponents() getNumComponents"/>&nbsp;-&nbsp;1.
	''' <li> The first
	''' <seealso cref="ColorModel#getNumColorComponents() getNumColorComponents"/>
	''' indices refer to color samples.
	''' <li> If an alpha sample is present, it corresponds the last index.
	''' <li> The order of the color indices is specified
	''' by the <code>ColorSpace</code>.  Typically, this reflects the name of
	''' the color space type (for example, TYPE_RGB), index 0
	''' corresponds to red, index 1 to green, and index 2 to blue.
	''' </ul>
	''' <p>
	''' The translation from pixel values to color/alpha components for
	''' display or processing purposes is a one-to-one correspondence of
	''' samples to components.
	''' A <code>PackedColorModel</code> is typically used with image data
	''' that uses masks to define packed samples.  For example, a
	''' <code>PackedColorModel</code> can be used in conjunction with a
	''' <seealso cref="SinglePixelPackedSampleModel"/> to construct a
	''' <seealso cref="BufferedImage"/>.  Normally the masks used by the
	''' <seealso cref="SampleModel"/> and the <code>ColorModel</code> would be the same.
	''' However, if they are different, the color interpretation of pixel data is
	''' done according to the masks of the <code>ColorModel</code>.
	''' <p>
	''' A single <code>int</code> pixel representation is valid for all objects
	''' of this class since it is always possible to represent pixel values
	''' used with this class in a single <code>int</code>.  Therefore, methods
	''' that use this representation do not throw an
	''' <code>IllegalArgumentException</code> due to an invalid pixel value.
	''' <p>
	''' A subclass of <code>PackedColorModel</code> is <seealso cref="DirectColorModel"/>,
	''' which is similar to an X11 TrueColor visual.
	''' </summary>
	''' <seealso cref= DirectColorModel </seealso>
	''' <seealso cref= SinglePixelPackedSampleModel </seealso>
	''' <seealso cref= BufferedImage </seealso>

	Public MustInherit Class PackedColorModel
		Inherits ColorModel

		Friend maskArray As Integer()
		Friend maskOffsets As Integer()
		Friend scaleFactors As Single()

		''' <summary>
		''' Constructs a <code>PackedColorModel</code> from a color mask array,
		''' which specifies which bits in an <code>int</code> pixel representation
		''' contain each of the color samples, and an alpha mask.  Color
		''' components are in the specified <code>ColorSpace</code>.  The length of
		''' <code>colorMaskArray</code> should be the number of components in
		''' the <code>ColorSpace</code>.  All of the bits in each mask
		''' must be contiguous and fit in the specified number of least significant
		''' bits of an <code>int</code> pixel representation.  If the
		''' <code>alphaMask</code> is 0, there is no alpha.  If there is alpha,
		''' the <code>boolean</code> <code>isAlphaPremultiplied</code> specifies
		''' how to interpret color and alpha samples in pixel values.  If the
		''' <code>boolean</code> is <code>true</code>, color samples are assumed
		''' to have been multiplied by the alpha sample.  The transparency,
		''' <code>trans</code>, specifies what alpha values can be represented
		''' by this color model.  The transfer type is the type of primitive
		''' array used to represent pixel values. </summary>
		''' <param name="space"> the specified <code>ColorSpace</code> </param>
		''' <param name="bits"> the number of bits in the pixel values </param>
		''' <param name="colorMaskArray"> array that specifies the masks representing
		'''         the bits of the pixel values that represent the color
		'''         components </param>
		''' <param name="alphaMask"> specifies the mask representing
		'''         the bits of the pixel values that represent the alpha
		'''         component </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		'''        premultiplied by the alpha sample; <code>false</code> otherwise </param>
		''' <param name="trans"> specifies the alpha value that can be represented by
		'''        this color model </param>
		''' <param name="transferType"> the type of array used to represent pixel values </param>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is less than
		'''         1 or greater than 32 </exception>
		Public Sub New(ByVal space As java.awt.color.ColorSpace, ByVal bits As Integer, ByVal colorMaskArray As Integer(), ByVal alphaMask As Integer, ByVal isAlphaPremultiplied As Boolean, ByVal trans As Integer, ByVal transferType As Integer)
			MyBase.New(bits, PackedColorModel.createBitsArray(colorMaskArray, alphaMask), space, (If(alphaMask = 0, False, True)), isAlphaPremultiplied, trans, transferType)
			If bits < 1 OrElse bits > 32 Then Throw New IllegalArgumentException("Number of bits must be between" & " 1 and 32.")
			maskArray = New Integer(numComponents - 1){}
			maskOffsets = New Integer(numComponents - 1){}
			scaleFactors = New Single(numComponents - 1){}

			For i As Integer = 0 To numColorComponents - 1
				' Get the mask offset and #bits
				DecomposeMask(colorMaskArray(i), i, space.getName(i))
			Next i
			If alphaMask <> 0 Then
				DecomposeMask(alphaMask, numColorComponents, "alpha")
				If nBits(numComponents-1) = 1 Then transparency = java.awt.Transparency.BITMASK
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>PackedColorModel</code> from the specified
		''' masks which indicate which bits in an <code>int</code> pixel
		''' representation contain the alpha, red, green and blue color samples.
		''' Color components are in the specified <code>ColorSpace</code>, which
		''' must be of type ColorSpace.TYPE_RGB.  All of the bits in each
		''' mask must be contiguous and fit in the specified number of
		''' least significant bits of an <code>int</code> pixel representation.  If
		''' <code>amask</code> is 0, there is no alpha.  If there is alpha,
		''' the <code>boolean</code> <code>isAlphaPremultiplied</code>
		''' specifies how to interpret color and alpha samples
		''' in pixel values.  If the <code>boolean</code> is <code>true</code>,
		''' color samples are assumed to have been multiplied by the alpha sample.
		''' The transparency, <code>trans</code>, specifies what alpha values
		''' can be represented by this color model.
		''' The transfer type is the type of primitive array used to represent
		''' pixel values. </summary>
		''' <param name="space"> the specified <code>ColorSpace</code> </param>
		''' <param name="bits"> the number of bits in the pixel values </param>
		''' <param name="rmask"> specifies the mask representing
		'''         the bits of the pixel values that represent the red
		'''         color component </param>
		''' <param name="gmask"> specifies the mask representing
		'''         the bits of the pixel values that represent the green
		'''         color component </param>
		''' <param name="bmask"> specifies the mask representing
		'''         the bits of the pixel values that represent
		'''         the blue color component </param>
		''' <param name="amask"> specifies the mask representing
		'''         the bits of the pixel values that represent
		'''         the alpha component </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		'''        premultiplied by the alpha sample; <code>false</code> otherwise </param>
		''' <param name="trans"> specifies the alpha value that can be represented by
		'''        this color model </param>
		''' <param name="transferType"> the type of array used to represent pixel values </param>
		''' <exception cref="IllegalArgumentException"> if <code>space</code> is not a
		'''         TYPE_RGB space </exception>
		''' <seealso cref= ColorSpace </seealso>
		Public Sub New(ByVal space As java.awt.color.ColorSpace, ByVal bits As Integer, ByVal rmask As Integer, ByVal gmask As Integer, ByVal bmask As Integer, ByVal amask As Integer, ByVal isAlphaPremultiplied As Boolean, ByVal trans As Integer, ByVal transferType As Integer)
			MyBase.New(bits, PackedColorModel.createBitsArray(rmask, gmask, bmask, amask), space, (If(amask = 0, False, True)), isAlphaPremultiplied, trans, transferType)

			If space.type <> java.awt.color.ColorSpace.TYPE_RGB Then Throw New IllegalArgumentException("ColorSpace must be TYPE_RGB.")
			maskArray = New Integer(numComponents - 1){}
			maskOffsets = New Integer(numComponents - 1){}
			scaleFactors = New Single(numComponents - 1){}

			DecomposeMask(rmask, 0, "red")

			DecomposeMask(gmask, 1, "green")

			DecomposeMask(bmask, 2, "blue")

			If amask <> 0 Then
				DecomposeMask(amask, 3, "alpha")
				If nBits(3) = 1 Then transparency = java.awt.Transparency.BITMASK
			End If
		End Sub

		''' <summary>
		''' Returns the mask indicating which bits in a pixel
		''' contain the specified color/alpha sample.  For color
		''' samples, <code>index</code> corresponds to the placement of color
		''' sample names in the color space.  Thus, an <code>index</code>
		''' equal to 0 for a CMYK ColorSpace would correspond to
		''' Cyan and an <code>index</code> equal to 1 would correspond to
		''' Magenta.  If there is alpha, the alpha <code>index</code> would be:
		''' <pre>
		'''      alphaIndex = numComponents() - 1;
		''' </pre> </summary>
		''' <param name="index"> the specified color or alpha sample </param>
		''' <returns> the mask, which indicates which bits of the <code>int</code>
		'''         pixel representation contain the color or alpha sample specified
		'''         by <code>index</code>. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>index</code> is
		'''         greater than the number of components minus 1 in this
		'''         <code>PackedColorModel</code> or if <code>index</code> is
		'''         less than zero </exception>
		Public Function getMask(ByVal index As Integer) As Integer
			Return maskArray(index)
		End Function

		''' <summary>
		''' Returns a mask array indicating which bits in a pixel
		''' contain the color and alpha samples. </summary>
		''' <returns> the mask array , which indicates which bits of the
		'''         <code>int</code> pixel
		'''         representation contain the color or alpha samples. </returns>
		Public Property masks As Integer()
			Get
				Return CType(maskArray.clone(), Integer())
			End Get
		End Property

	'    
	'     * A utility function to compute the mask offset and scalefactor,
	'     * store these and the mask in instance arrays, and verify that
	'     * the mask fits in the specified pixel size.
	'     
		Private Sub DecomposeMask(ByVal mask As Integer, ByVal idx As Integer, ByVal componentName As String)
			Dim [off] As Integer = 0
			Dim count As Integer = nBits(idx)

			' Store the mask
			maskArray(idx) = mask

			' Now find the shift
			If mask <> 0 Then
				Do While (mask And 1) = 0
					mask >>>= 1
					[off] += 1
				Loop
			End If

			If [off] + count > pixel_bits Then Throw New IllegalArgumentException(componentName & " mask " &  java.lang.[Integer].toHexString(maskArray(idx)) & " overflows pixel (expecting " & pixel_bits & " bits")

			maskOffsets(idx) = [off]
			If count = 0 Then
				' High enough to scale any 0-ff value down to 0.0, but not
				' high enough to get Infinity when scaling back to pixel bits
				scaleFactors(idx) = 256.0f
			Else
				scaleFactors(idx) = 255.0f / ((1 << count) - 1)
			End If

		End Sub

		''' <summary>
		''' Creates a <code>SampleModel</code> with the specified width and
		''' height that has a data layout compatible with this
		''' <code>ColorModel</code>. </summary>
		''' <param name="w"> the width (in pixels) of the region of the image data
		'''          described </param>
		''' <param name="h"> the height (in pixels) of the region of the image data
		'''          described </param>
		''' <returns> the newly created <code>SampleModel</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or
		'''         <code>h</code> is not greater than 0 </exception>
		''' <seealso cref= SampleModel </seealso>
		Public Overrides Function createCompatibleSampleModel(ByVal w As Integer, ByVal h As Integer) As SampleModel
			Return New SinglePixelPackedSampleModel(transferType, w, h, maskArray)
		End Function

		''' <summary>
		''' Checks if the specified <code>SampleModel</code> is compatible
		''' with this <code>ColorModel</code>.  If <code>sm</code> is
		''' <code>null</code>, this method returns <code>false</code>. </summary>
		''' <param name="sm"> the specified <code>SampleModel</code>,
		''' or <code>null</code> </param>
		''' <returns> <code>true</code> if the specified <code>SampleModel</code>
		'''         is compatible with this <code>ColorModel</code>;
		'''         <code>false</code> otherwise. </returns>
		''' <seealso cref= SampleModel </seealso>
		Public Overrides Function isCompatibleSampleModel(ByVal sm As SampleModel) As Boolean
			If Not(TypeOf sm Is SinglePixelPackedSampleModel) Then Return False

			' Must have the same number of components
			If numComponents <> sm.numBands Then Return False

			' Transfer type must be the same
			If sm.transferType <> transferType Then Return False

			Dim sppsm As SinglePixelPackedSampleModel = CType(sm, SinglePixelPackedSampleModel)
			' Now compare the specific masks
			Dim bitMasks As Integer() = sppsm.bitMasks
			If bitMasks.Length <> maskArray.Length Then Return False

	'         compare 'effective' masks only, i.e. only part of the mask
	'         * which fits the capacity of the transfer type.
	'         
			Dim maxMask As Integer = CInt(Fix((1L << DataBuffer.getDataTypeSize(transferType)) - 1))
			For i As Integer = 0 To bitMasks.Length - 1
				If (maxMask And bitMasks(i)) <> (maxMask And maskArray(i)) Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Returns a <seealso cref="WritableRaster"/> representing the alpha channel of
		''' an image, extracted from the input <code>WritableRaster</code>.
		''' This method assumes that <code>WritableRaster</code> objects
		''' associated with this <code>ColorModel</code> store the alpha band,
		''' if present, as the last band of image data.  Returns <code>null</code>
		''' if there is no separate spatial alpha channel associated with this
		''' <code>ColorModel</code>.  This method creates a new
		''' <code>WritableRaster</code>, but shares the data array. </summary>
		''' <param name="raster"> a <code>WritableRaster</code> containing an image </param>
		''' <returns> a <code>WritableRaster</code> that represents the alpha
		'''         channel of the image contained in <code>raster</code>. </returns>
		Public Overrides Function getAlphaRaster(ByVal raster_Renamed As WritableRaster) As WritableRaster
			If hasAlpha() = False Then Return Nothing

			Dim x As Integer = raster_Renamed.minX
			Dim y As Integer = raster_Renamed.minY
			Dim band As Integer() = New Integer(0){}
			band(0) = raster_Renamed.numBands - 1
			Return raster_Renamed.createWritableChild(x, y, raster_Renamed.width, raster_Renamed.height, x, y, band)
		End Function

		''' <summary>
		''' Tests if the specified <code>Object</code> is an instance
		''' of <code>PackedColorModel</code> and equals this
		''' <code>PackedColorModel</code>. </summary>
		''' <param name="obj"> the <code>Object</code> to test for equality </param>
		''' <returns> <code>true</code> if the specified <code>Object</code>
		''' is an instance of <code>PackedColorModel</code> and equals this
		''' <code>PackedColorModel</code>; <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is PackedColorModel) Then Return False

			If Not MyBase.Equals(obj) Then Return False

			Dim cm As PackedColorModel = CType(obj, PackedColorModel)
			Dim numC As Integer = cm.numComponents
			If numC <> numComponents Then Return False
			For i As Integer = 0 To numC - 1
				If maskArray(i) <> cm.getMask(i) Then Return False
			Next i
			Return True
		End Function

		Private Shared Function createBitsArray(ByVal colorMaskArray As Integer(), ByVal alphaMask As Integer) As Integer()
			Dim numColors As Integer = colorMaskArray.Length
			Dim numAlpha As Integer = (If(alphaMask = 0, 0, 1))
			Dim arr As Integer() = New Integer(numColors+numAlpha - 1){}
			For i As Integer = 0 To numColors - 1
				arr(i) = countBits(colorMaskArray(i))
				If arr(i) < 0 Then Throw New IllegalArgumentException("Noncontiguous color mask (" &  java.lang.[Integer].toHexString(colorMaskArray(i)) & "at index " & i)
			Next i
			If alphaMask <> 0 Then
				arr(numColors) = countBits(alphaMask)
				If arr(numColors) < 0 Then Throw New IllegalArgumentException("Noncontiguous alpha mask (" &  java.lang.[Integer].toHexString(alphaMask))
			End If
			Return arr
		End Function

		Private Shared Function createBitsArray(ByVal rmask As Integer, ByVal gmask As Integer, ByVal bmask As Integer, ByVal amask As Integer) As Integer()
			Dim arr As Integer() = New Integer(3 + (If(amask = 0, 0, 1)) - 1){}
			arr(0) = countBits(rmask)
			arr(1) = countBits(gmask)
			arr(2) = countBits(bmask)
			If arr(0) < 0 Then
				Throw New IllegalArgumentException("Noncontiguous red mask (" &  java.lang.[Integer].toHexString(rmask))
			ElseIf arr(1) < 0 Then
				Throw New IllegalArgumentException("Noncontiguous green mask (" &  java.lang.[Integer].toHexString(gmask))
			ElseIf arr(2) < 0 Then
				Throw New IllegalArgumentException("Noncontiguous blue mask (" &  java.lang.[Integer].toHexString(bmask))
			End If
			If amask <> 0 Then
				arr(3) = countBits(amask)
				If arr(3) < 0 Then Throw New IllegalArgumentException("Noncontiguous alpha mask (" &  java.lang.[Integer].toHexString(amask))
			End If
			Return arr
		End Function

		Private Shared Function countBits(ByVal mask As Integer) As Integer
			Dim count As Integer = 0
			If mask <> 0 Then
				Do While (mask And 1) = 0
					mask >>>= 1
				Loop
				Do While (mask And 1) = 1
					mask >>>= 1
					count += 1
				Loop
			End If
			If mask <> 0 Then Return -1
			Return count
		End Function

	End Class

End Namespace