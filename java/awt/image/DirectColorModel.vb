Imports Microsoft.VisualBasic

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
	''' The <code>DirectColorModel</code> class is a <code>ColorModel</code>
	''' class that works with pixel values that represent RGB
	''' color and alpha information as separate samples and that pack all
	''' samples for a single pixel into a single int, short, or byte quantity.
	''' This class can be used only with ColorSpaces of type ColorSpace.TYPE_RGB.
	''' In addition, for each component of the ColorSpace, the minimum
	''' normalized component value obtained via the <code>getMinValue()</code>
	''' method of ColorSpace must be 0.0, and the maximum value obtained via
	''' the <code>getMaxValue()</code> method must be 1.0 (these min/max
	''' values are typical for RGB spaces).
	''' There must be three color samples in the pixel values and there can
	''' be a single alpha sample.  For those methods that use a primitive array
	''' pixel representation of type <code>transferType</code>, the array
	''' length is always one.  The transfer
	''' types supported are DataBuffer.TYPE_BYTE,
	''' DataBuffer.TYPE_USHORT, and DataBuffer.TYPE_INT.
	''' Color and alpha samples are stored in the single
	''' element of the array in bits indicated by bit masks.  Each bit mask
	''' must be contiguous and masks must not overlap.  The same masks apply to
	''' the single int pixel representation used by other methods.  The
	''' correspondence of masks and color/alpha samples is as follows:
	''' <ul>
	''' <li> Masks are identified by indices running from 0 through 2
	''' if no alpha is present, or 3 if an alpha is present.
	''' <li> The first three indices refer to color samples;
	''' index 0 corresponds to red, index 1 to green, and index 2 to blue.
	''' <li> Index 3 corresponds to the alpha sample, if present.
	''' </ul>
	''' <p>
	''' The translation from pixel values to color/alpha components for
	''' display or processing purposes is a one-to-one correspondence of
	''' samples to components.  A <code>DirectColorModel</code> is
	''' typically used with image data which uses masks to define packed
	''' samples.  For example, a <code>DirectColorModel</code> can be used in
	''' conjunction with a <code>SinglePixelPackedSampleModel</code> to
	''' construct a <seealso cref="BufferedImage"/>.  Normally the masks used by the
	''' <seealso cref="SampleModel"/> and the <code>ColorModel</code> would be the
	''' same.  However, if they are different, the color interpretation
	''' of pixel data will be done according to the masks of the
	''' <code>ColorModel</code>.
	''' <p>
	''' A single int pixel representation is valid for all objects of this
	''' [Class], since it is always possible to represent pixel values used with
	''' this class in a single int.  Therefore, methods which use this
	''' representation will not throw an <code>IllegalArgumentException</code>
	''' due to an invalid pixel value.
	''' <p>
	''' This color model is similar to an X11 TrueColor visual.
	''' The default RGB ColorModel specified by the
	''' <seealso cref="ColorModel#getRGBdefault() getRGBdefault"/> method is a
	''' <code>DirectColorModel</code> with the following parameters:
	''' <pre>
	''' Number of bits:        32
	''' Red mask:              0x00ff0000
	''' Green mask:            0x0000ff00
	''' Blue mask:             0x000000ff
	''' Alpha mask:            0xff000000
	''' Color space:           sRGB
	''' isAlphaPremultiplied:  False
	''' Transparency:          Transparency.TRANSLUCENT
	''' transferType:          DataBuffer.TYPE_INT
	''' </pre>
	''' <p>
	''' Many of the methods in this class are final. This is because the
	''' underlying native graphics code makes assumptions about the layout
	''' and operation of this class and those assumptions are reflected in
	''' the implementations of the methods here that are marked final.  You
	''' can subclass this class for other reasons, but you cannot override
	''' or modify the behavior of those methods.
	''' </summary>
	''' <seealso cref= ColorModel </seealso>
	''' <seealso cref= ColorSpace </seealso>
	''' <seealso cref= SinglePixelPackedSampleModel </seealso>
	''' <seealso cref= BufferedImage </seealso>
	''' <seealso cref= ColorModel#getRGBdefault
	'''  </seealso>
	Public Class DirectColorModel
		Inherits PackedColorModel

		Private red_mask As Integer
		Private green_mask As Integer
		Private blue_mask As Integer
		Private alpha_mask As Integer
		Private red_offset As Integer
		Private green_offset As Integer
		Private blue_offset As Integer
		Private alpha_offset As Integer
		Private red_scale As Integer
		Private green_scale As Integer
		Private blue_scale As Integer
		Private alpha_scale As Integer
		Private is_LinearRGB As Boolean
		Private lRGBprecision As Integer
		Private tosRGB8LUT As SByte()
		Private fromsRGB8LUT8 As SByte()
		Private fromsRGB8LUT16 As Short()

		''' <summary>
		''' Constructs a <code>DirectColorModel</code> from the specified masks
		''' that indicate which bits in an <code>int</code> pixel representation
		''' contain the red, green and blue color samples.  As pixel values do not
		''' contain alpha information, all pixels are treated as opaque, which
		''' means that alpha&nbsp;=&nbsp;1.0.  All of the bits
		''' in each mask must be contiguous and fit in the specified number
		''' of least significant bits of an <code>int</code> pixel representation.
		'''  The <code>ColorSpace</code> is the default sRGB space. The
		''' transparency value is Transparency.OPAQUE.  The transfer type
		''' is the smallest of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' or DataBuffer.TYPE_INT that can hold a single pixel. </summary>
		''' <param name="bits"> the number of bits in the pixel values; for example,
		'''         the sum of the number of bits in the masks. </param>
		''' <param name="rmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the red component </param>
		''' <param name="gmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the green component </param>
		''' <param name="bmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the blue component
		'''  </param>
		Public Sub New(ByVal bits As Integer, ByVal rmask As Integer, ByVal gmask As Integer, ByVal bmask As Integer)
			Me.New(bits, rmask, gmask, bmask, 0)
		End Sub

		''' <summary>
		''' Constructs a <code>DirectColorModel</code> from the specified masks
		''' that indicate which bits in an <code>int</code> pixel representation
		''' contain the red, green and blue color samples and the alpha sample,
		''' if present.  If <code>amask</code> is 0, pixel values do not contain
		''' alpha information and all pixels are treated as opaque, which means
		''' that alpha&nbsp;=&nbsp;1.0.  All of the bits in each mask must
		''' be contiguous and fit in the specified number of least significant bits
		''' of an <code>int</code> pixel representation.  Alpha, if present, is not
		''' premultiplied.  The <code>ColorSpace</code> is the default sRGB space.
		''' The transparency value is Transparency.OPAQUE if no alpha is
		''' present, or Transparency.TRANSLUCENT otherwise.  The transfer type
		''' is the smallest of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT,
		''' or DataBuffer.TYPE_INT that can hold a single pixel. </summary>
		''' <param name="bits"> the number of bits in the pixel values; for example,
		'''         the sum of the number of bits in the masks. </param>
		''' <param name="rmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the red component </param>
		''' <param name="gmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the green component </param>
		''' <param name="bmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the blue component </param>
		''' <param name="amask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the alpha component </param>
		Public Sub New(ByVal bits As Integer, ByVal rmask As Integer, ByVal gmask As Integer, ByVal bmask As Integer, ByVal amask As Integer)
			MyBase.New(java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), bits, rmask, gmask, bmask, amask, False,If(amask = 0, java.awt.Transparency.OPAQUE, java.awt.Transparency.TRANSLUCENT), ColorModel.getDefaultTransferType(bits))
			fieldslds()
		End Sub

		''' <summary>
		''' Constructs a <code>DirectColorModel</code> from the specified
		''' parameters.  Color components are in the specified
		''' <code>ColorSpace</code>, which must be of type ColorSpace.TYPE_RGB
		''' and have minimum normalized component values which are all 0.0
		''' and maximum values which are all 1.0.
		''' The masks specify which bits in an <code>int</code> pixel
		''' representation contain the red, green and blue color samples and
		''' the alpha sample, if present.  If <code>amask</code> is 0, pixel
		''' values do not contain alpha information and all pixels are treated
		''' as opaque, which means that alpha&nbsp;=&nbsp;1.0.  All of the
		''' bits in each mask must be contiguous and fit in the specified number
		''' of least significant bits of an <code>int</code> pixel
		''' representation.  If there is alpha, the <code>boolean</code>
		''' <code>isAlphaPremultiplied</code> specifies how to interpret
		''' color and alpha samples in pixel values.  If the <code>boolean</code>
		''' is <code>true</code>, color samples are assumed to have been
		''' multiplied by the alpha sample.  The transparency value is
		''' Transparency.OPAQUE, if no alpha is present, or
		''' Transparency.TRANSLUCENT otherwise.  The transfer type
		''' is the type of primitive array used to represent pixel values and
		''' must be one of DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, or
		''' DataBuffer.TYPE_INT. </summary>
		''' <param name="space"> the specified <code>ColorSpace</code> </param>
		''' <param name="bits"> the number of bits in the pixel values; for example,
		'''         the sum of the number of bits in the masks. </param>
		''' <param name="rmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the red component </param>
		''' <param name="gmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the green component </param>
		''' <param name="bmask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the blue component </param>
		''' <param name="amask"> specifies a mask indicating which bits in an
		'''         integer pixel contain the alpha component </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if color samples are
		'''        premultiplied by the alpha sample; <code>false</code> otherwise </param>
		''' <param name="transferType"> the type of array used to represent pixel values </param>
		''' <exception cref="IllegalArgumentException"> if <code>space</code> is not a
		'''         TYPE_RGB space or if the min/max normalized component
		'''         values are not 0.0/1.0. </exception>
		Public Sub New(ByVal space As java.awt.color.ColorSpace, ByVal bits As Integer, ByVal rmask As Integer, ByVal gmask As Integer, ByVal bmask As Integer, ByVal amask As Integer, ByVal isAlphaPremultiplied As Boolean, ByVal transferType As Integer)
			MyBase.New(space, bits, rmask, gmask, bmask, amask, isAlphaPremultiplied,If(amask = 0, java.awt.Transparency.OPAQUE, java.awt.Transparency.TRANSLUCENT), transferType)
			If ColorModel.isLinearRGBspace(colorSpace) Then
				is_LinearRGB = True
				If maxBits <= 8 Then
					lRGBprecision = 8
					tosRGB8LUT = ColorModel.linearRGB8TosRGB8LUT
					fromsRGB8LUT8 = ColorModel.getsRGB8ToLinearRGB8LUT()
				Else
					lRGBprecision = 16
					tosRGB8LUT = ColorModel.linearRGB16TosRGB8LUT
					fromsRGB8LUT16 = ColorModel.getsRGB8ToLinearRGB16LUT()
				End If
			ElseIf Not is_sRGB Then
				For i As Integer = 0 To 2
					' super constructor checks that space is TYPE_RGB
					' check here that min/max are all 0.0/1.0
					If (space.getMinValue(i) <> 0.0f) OrElse (space.getMaxValue(i) <> 1.0f) Then Throw New IllegalArgumentException("Illegal min/max RGB component value")
				Next i
			End If
			fieldslds()
		End Sub

		''' <summary>
		''' Returns the mask indicating which bits in an <code>int</code> pixel
		''' representation contain the red color component. </summary>
		''' <returns> the mask, which indicates which bits of the <code>int</code>
		'''         pixel representation contain the red color sample. </returns>
		Public Property redMask As Integer
			Get
				Return maskArray(0)
			End Get
		End Property

		''' <summary>
		''' Returns the mask indicating which bits in an <code>int</code> pixel
		''' representation contain the green color component. </summary>
		''' <returns> the mask, which indicates which bits of the <code>int</code>
		'''         pixel representation contain the green color sample. </returns>
		Public Property greenMask As Integer
			Get
				Return maskArray(1)
			End Get
		End Property

		''' <summary>
		''' Returns the mask indicating which bits in an <code>int</code> pixel
		''' representation contain the blue color component. </summary>
		''' <returns> the mask, which indicates which bits of the <code>int</code>
		'''         pixel representation contain the blue color sample. </returns>
		Public Property blueMask As Integer
			Get
				Return maskArray(2)
			End Get
		End Property

		''' <summary>
		''' Returns the mask indicating which bits in an <code>int</code> pixel
		''' representation contain the alpha component. </summary>
		''' <returns> the mask, which indicates which bits of the <code>int</code>
		'''         pixel representation contain the alpha sample. </returns>
		Public Property alphaMask As Integer
			Get
				If supportsAlpha Then
					Return maskArray(3)
				Else
					Return 0
				End If
			End Get
		End Property


	'    
	'     * Given an int pixel in this ColorModel's ColorSpace, converts
	'     * it to the default sRGB ColorSpace and returns the R, G, and B
	'     * components as float values between 0.0 and 1.0.
	'     
		Private Function getDefaultRGBComponents(ByVal pixel As Integer) As Single()
			Dim components_Renamed As Integer() = getComponents(pixel, Nothing, 0)
			Dim norm As Single() = getNormalizedComponents(components_Renamed, 0, Nothing, 0)
			' Note that getNormalizedComponents returns non-premultiplied values
			Return colorSpace.toRGB(norm)
		End Function


		Private Function getsRGBComponentFromsRGB(ByVal pixel As Integer, ByVal idx As Integer) As Integer
			Dim c As Integer = (CInt(CUInt((pixel And maskArray(idx))) >> maskOffsets(idx)))
			If isAlphaPremultiplied_Renamed Then
				Dim a As Integer = (CInt(CUInt((pixel And maskArray(3))) >> maskOffsets(3)))
				c = If(a = 0, 0, CInt(Fix(((c * scaleFactors(idx)) * 255.0f / (a * scaleFactors(3))) + 0.5f)))
			ElseIf scaleFactors(idx) <> 1.0f Then
				c = CInt(Fix((c * scaleFactors(idx)) + 0.5f))
			End If
			Return c
		End Function


		Private Function getsRGBComponentFromLinearRGB(ByVal pixel As Integer, ByVal idx As Integer) As Integer
			Dim c As Integer = (CInt(CUInt((pixel And maskArray(idx))) >> maskOffsets(idx)))
			If isAlphaPremultiplied_Renamed Then
				Dim factor As Single = CSng((1 << lRGBprecision) - 1)
				Dim a As Integer = (CInt(CUInt((pixel And maskArray(3))) >> maskOffsets(3)))
				c = If(a = 0, 0, CInt(Fix(((c * scaleFactors(idx)) * factor / (a * scaleFactors(3))) + 0.5f)))
			ElseIf nBits(idx) <> lRGBprecision Then
				If lRGBprecision = 16 Then
					c = CInt(Fix((c * scaleFactors(idx) * 257.0f) + 0.5f))
				Else
					c = CInt(Fix((c * scaleFactors(idx)) + 0.5f))
				End If
			End If
			' now range of c is 0-255 or 0-65535, depending on lRGBprecision
			Return tosRGB8LUT(c) And &Hff
		End Function


		''' <summary>
		''' Returns the red color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' as an <code>int</code>.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the red value
		''' is 0. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the red color component for the specified pixel, from
		'''         0 to 255 in the sRGB <code>ColorSpace</code>. </returns>
		Public NotOverridable Overrides Function getRed(ByVal pixel As Integer) As Integer
			If is_sRGB Then
				Return getsRGBComponentFromsRGB(pixel, 0)
			ElseIf is_LinearRGB Then
				Return getsRGBComponentFromLinearRGB(pixel, 0)
			End If
			Dim rgb_Renamed As Single() = getDefaultRGBComponents(pixel)
			Return CInt(Fix(rgb_Renamed(0) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the green color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' as an <code>int</code>.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the green value
		''' is 0. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the green color component for the specified pixel, from
		'''         0 to 255 in the sRGB <code>ColorSpace</code>. </returns>
		Public NotOverridable Overrides Function getGreen(ByVal pixel As Integer) As Integer
			If is_sRGB Then
				Return getsRGBComponentFromsRGB(pixel, 1)
			ElseIf is_LinearRGB Then
				Return getsRGBComponentFromLinearRGB(pixel, 1)
			End If
			Dim rgb_Renamed As Single() = getDefaultRGBComponents(pixel)
			Return CInt(Fix(rgb_Renamed(1) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the blue color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' as an <code>int</code>.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the blue value
		''' is 0. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the blue color component for the specified pixel, from
		'''         0 to 255 in the sRGB <code>ColorSpace</code>. </returns>
		Public NotOverridable Overrides Function getBlue(ByVal pixel As Integer) As Integer
			If is_sRGB Then
				Return getsRGBComponentFromsRGB(pixel, 2)
			ElseIf is_LinearRGB Then
				Return getsRGBComponentFromLinearRGB(pixel, 2)
			End If
			Dim rgb_Renamed As Single() = getDefaultRGBComponents(pixel)
			Return CInt(Fix(rgb_Renamed(2) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the alpha component for the specified pixel, scaled
		''' from 0 to 255.  The pixel value is specified as an <code>int</code>. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the value of the alpha component of <code>pixel</code>
		'''         from 0 to 255. </returns>
		Public NotOverridable Overrides Function getAlpha(ByVal pixel As Integer) As Integer
			If Not supportsAlpha Then Return 255
			Dim a As Integer = (CInt(CUInt((pixel And maskArray(3))) >> maskOffsets(3)))
			If scaleFactors(3) <> 1.0f Then a = CInt(Fix(a * scaleFactors(3) + 0.5f))
			Return a
		End Function

		''' <summary>
		''' Returns the color/alpha components of the pixel in the default
		''' RGB color model format.  A color conversion is done if necessary.
		''' The pixel value is specified as an <code>int</code>.
		''' The returned value is in a non pre-multiplied format.  Thus, if
		''' the alpha is premultiplied, this method divides it out of the
		''' color components.  If the alpha value is 0, for example, the color
		''' values are each 0. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> the RGB value of the color/alpha components of the specified
		'''         pixel. </returns>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		Public NotOverridable Overrides Function getRGB(ByVal pixel As Integer) As Integer
			If is_sRGB OrElse is_LinearRGB Then Return (getAlpha(pixel) << 24) Or (getRed(pixel) << 16) Or (getGreen(pixel) << 8) Or (getBlue(pixel) << 0)
			Dim rgb_Renamed As Single() = getDefaultRGBComponents(pixel)
			Return (getAlpha(pixel) << 24) Or ((CInt(Fix(rgb_Renamed(0) * 255.0f + 0.5f))) << 16) Or ((CInt(Fix(rgb_Renamed(1) * 255.0f + 0.5f))) << 8) Or ((CInt(Fix(rgb_Renamed(2) * 255.0f + 0.5f))) << 0)
		End Function

		''' <summary>
		''' Returns the red color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' by an array of data elements of type <code>transferType</code> passed
		''' in as an object reference.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the red value
		''' is 0.
		''' If <code>inData</code> is not a primitive array of type
		''' <code>transferType</code>, a <code>ClassCastException</code> is
		''' thrown.  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>inData</code> is not large enough to hold a
		''' pixel value for this <code>ColorModel</code>.  Since
		''' <code>DirectColorModel</code> can be subclassed, subclasses inherit
		''' the implementation of this method and if they don't override it
		''' then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' An <code>UnsupportedOperationException</code> is thrown if this
		''' <code>transferType</code> is not supported by this
		''' <code>ColorModel</code>. </summary>
		''' <param name="inData"> the array containing the pixel value </param>
		''' <returns> the value of the red component of the specified pixel. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>inData</code> is not
		'''         large enough to hold a pixel value for this color model </exception>
		''' <exception cref="ClassCastException"> if <code>inData</code> is not a
		'''         primitive array of type <code>transferType</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this <code>transferType</code>
		'''         is not supported by this color model </exception>
		Public Overrides Function getRed(ByVal inData As Object) As Integer
			Dim pixel As Integer=0
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getRed(pixel)
		End Function


		''' <summary>
		''' Returns the green color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' by an array of data elements of type <code>transferType</code> passed
		''' in as an object reference.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the green value
		''' is 0.  If <code>inData</code> is not a primitive array of type
		''' <code>transferType</code>, a <code>ClassCastException</code> is thrown.
		'''  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>inData</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.  Since
		''' <code>DirectColorModel</code> can be subclassed, subclasses inherit
		''' the implementation of this method and if they don't override it
		''' then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' An <code>UnsupportedOperationException</code> is
		''' thrown if this <code>transferType</code> is not supported by this
		''' <code>ColorModel</code>. </summary>
		''' <param name="inData"> the array containing the pixel value </param>
		''' <returns> the value of the green component of the specified pixel. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>inData</code> is not
		'''         large enough to hold a pixel value for this color model </exception>
		''' <exception cref="ClassCastException"> if <code>inData</code> is not a
		'''         primitive array of type <code>transferType</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this <code>transferType</code>
		'''         is not supported by this color model </exception>
		Public Overrides Function getGreen(ByVal inData As Object) As Integer
			Dim pixel As Integer=0
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getGreen(pixel)
		End Function


		''' <summary>
		''' Returns the blue color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <code>ColorSpace</code>, sRGB.  A
		''' color conversion is done if necessary.  The pixel value is specified
		''' by an array of data elements of type <code>transferType</code> passed
		''' in as an object reference.
		''' The returned value is a non pre-multiplied value.  Thus, if the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value.  If the alpha value is 0, for example, the blue value
		''' is 0.  If <code>inData</code> is not a primitive array of type
		''' <code>transferType</code>, a <code>ClassCastException</code> is thrown.
		'''  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>inData</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.  Since
		''' <code>DirectColorModel</code> can be subclassed, subclasses inherit
		''' the implementation of this method and if they don't override it
		''' then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' An <code>UnsupportedOperationException</code> is
		''' thrown if this <code>transferType</code> is not supported by this
		''' <code>ColorModel</code>. </summary>
		''' <param name="inData"> the array containing the pixel value </param>
		''' <returns> the value of the blue component of the specified pixel. </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>inData</code> is not
		'''         large enough to hold a pixel value for this color model </exception>
		''' <exception cref="ClassCastException"> if <code>inData</code> is not a
		'''         primitive array of type <code>transferType</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this <code>transferType</code>
		'''         is not supported by this color model </exception>
		Public Overrides Function getBlue(ByVal inData As Object) As Integer
			Dim pixel As Integer=0
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getBlue(pixel)
		End Function

		''' <summary>
		''' Returns the alpha component for the specified pixel, scaled
		''' from 0 to 255.  The pixel value is specified by an array of data
		''' elements of type <code>transferType</code> passed in as an object
		''' reference.
		''' If <code>inData</code> is not a primitive array of type
		''' <code>transferType</code>, a <code>ClassCastException</code> is
		''' thrown.  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>inData</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.  Since
		''' <code>DirectColorModel</code> can be subclassed, subclasses inherit
		''' the implementation of this method and if they don't override it
		''' then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' If this <code>transferType</code> is not supported, an
		''' <code>UnsupportedOperationException</code> is thrown. </summary>
		''' <param name="inData"> the specified pixel </param>
		''' <returns> the alpha component of the specified pixel, scaled from
		'''         0 to 255. </returns>
		''' <exception cref="ClassCastException"> if <code>inData</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>inData</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this
		'''  <code>tranferType</code> is not supported by this
		'''  <code>ColorModel</code> </exception>
		Public Overrides Function getAlpha(ByVal inData As Object) As Integer
			Dim pixel As Integer=0
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getAlpha(pixel)
		End Function

		''' <summary>
		''' Returns the color/alpha components for the specified pixel in the
		''' default RGB color model format.  A color conversion is done if
		''' necessary.  The pixel value is specified by an array of data
		''' elements of type <code>transferType</code> passed in as an object
		''' reference.  If <code>inData</code> is not a primitive array of type
		''' <code>transferType</code>, a <code>ClassCastException</code> is
		''' thrown.  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>inData</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.
		''' The returned value is in a non pre-multiplied format.  Thus, if
		''' the alpha is premultiplied, this method divides it out of the
		''' color components.  If the alpha value is 0, for example, the color
		''' values is 0.  Since <code>DirectColorModel</code> can be
		''' subclassed, subclasses inherit the implementation of this method
		''' and if they don't override it then
		''' they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> the specified pixel </param>
		''' <returns> the color and alpha components of the specified pixel. </returns>
		''' <exception cref="UnsupportedOperationException"> if this
		'''            <code>transferType</code> is not supported by this
		'''            <code>ColorModel</code> </exception>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		Public Overrides Function getRGB(ByVal inData As Object) As Integer
			Dim pixel As Integer=0
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   pixel = bdata(0) And &Hff
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short() = CType(inData, Short())
				   pixel = sdata(0) And &Hffff
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   pixel = idata(0)
				Case Else
				   Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
			Return getRGB(pixel)
		End Function

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <code>ColorModel</code>, given an integer pixel representation in the
		''' default RGB color model.
		''' This array can then be passed to the <code>setDataElements</code>
		''' method of a <code>WritableRaster</code> object.  If the pixel variable
		''' is <code>null</code>, a new array is allocated.  If <code>pixel</code>
		''' is not <code>null</code>, it must be a primitive array of type
		''' <code>transferType</code>; otherwise, a
		''' <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>pixel</code> is not large enough to hold a pixel
		''' value for this <code>ColorModel</code>.  The pixel array is returned.
		''' Since <code>DirectColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="rgb"> the integer pixel representation in the default RGB
		'''            color model </param>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> an array representation of the specified pixel in this
		'''         <code>ColorModel</code> </returns>
		''' <exception cref="ClassCastException"> if <code>pixel</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>pixel</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this
		'''  <code>transferType</code> is not supported by this
		'''  <code>ColorModel</code> </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		Public Overrides Function getDataElements(ByVal rgb As Integer, ByVal pixel As Object) As Object
			'REMIND: maybe more efficient not to use int array for
			'DataBuffer.TYPE_USHORT and DataBuffer.TYPE_INT
			Dim intpixel As Integer() = Nothing
			If transferType = DataBuffer.TYPE_INT AndAlso pixel IsNot Nothing Then
				intpixel = CType(pixel, Integer())
				intpixel(0) = 0
			Else
				intpixel = New Integer(0){}
			End If

			Dim defaultCM As ColorModel = ColorModel.rGBdefault
			If Me Is defaultCM OrElse Equals(defaultCM) Then
				intpixel(0) = rgb
				Return intpixel
			End If

			Dim red_Renamed, grn, blu, alp As Integer
			red_Renamed = (rgb>>16) And &Hff
			grn = (rgb>>8) And &Hff
			blu = rgb And &Hff
			If is_sRGB OrElse is_LinearRGB Then
				Dim precision As Integer
				Dim factor As Single
				If is_LinearRGB Then
					If lRGBprecision = 8 Then
						red_Renamed = fromsRGB8LUT8(red_Renamed) And &Hff
						grn = fromsRGB8LUT8(grn) And &Hff
						blu = fromsRGB8LUT8(blu) And &Hff
						precision = 8
						factor = 1.0f / 255.0f
					Else
						red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
						grn = fromsRGB8LUT16(grn) And &Hffff
						blu = fromsRGB8LUT16(blu) And &Hffff
						precision = 16
						factor = 1.0f / 65535.0f
					End If
				Else
					precision = 8
					factor = 1.0f / 255.0f
				End If
				If supportsAlpha Then
					alp = (rgb>>24) And &Hff
					If isAlphaPremultiplied_Renamed Then
						factor *= (alp * (1.0f / 255.0f))
						precision = -1 ' force component calculations below
					End If
					If nBits(3) <> 8 Then
						alp = CInt(Fix((alp * (1.0f / 255.0f) * ((1<<nBits(3)) - 1)) + 0.5f))
						If alp > ((1<<nBits(3)) - 1) Then alp = (1<<nBits(3)) - 1
					End If
					intpixel(0) = alp << maskOffsets(3)
				End If
				If nBits(0) <> precision Then red_Renamed = CInt(Fix((red_Renamed * factor * ((1<<nBits(0)) - 1)) + 0.5f))
				If nBits(1) <> precision Then grn = CInt(Fix((grn * factor * ((1<<nBits(1)) - 1)) + 0.5f))
				If nBits(2) <> precision Then blu = CInt(Fix((blu * factor * ((1<<nBits(2)) - 1)) + 0.5f))
			Else
				' Need to convert the color
				Dim norm As Single() = New Single(2){}
				Dim factor As Single = 1.0f / 255.0f
				norm(0) = red_Renamed * factor
				norm(1) = grn * factor
				norm(2) = blu * factor
				norm = colorSpace.fromRGB(norm)
				If supportsAlpha Then
					alp = (rgb>>24) And &Hff
					If isAlphaPremultiplied_Renamed Then
						factor *= alp
						For i As Integer = 0 To 2
							norm(i) *= factor
						Next i
					End If
					If nBits(3) <> 8 Then
						alp = CInt(Fix((alp * (1.0f / 255.0f) * ((1<<nBits(3)) - 1)) + 0.5f))
						If alp > ((1<<nBits(3)) - 1) Then alp = (1<<nBits(3)) - 1
					End If
					intpixel(0) = alp << maskOffsets(3)
				End If
				red_Renamed = CInt(Fix((norm(0) * ((1<<nBits(0)) - 1)) + 0.5f))
				grn = CInt(Fix((norm(1) * ((1<<nBits(1)) - 1)) + 0.5f))
				blu = CInt(Fix((norm(2) * ((1<<nBits(2)) - 1)) + 0.5f))
			End If

			If maxBits > 23 Then
				' fix 4412670 - for components of 24 or more bits
				' some calculations done above with float precision
				' may lose enough precision that the integer result
				' overflows nBits, so we need to clamp.
				If red_Renamed > ((1<<nBits(0)) - 1) Then red_Renamed = (1<<nBits(0)) - 1
				If grn > ((1<<nBits(1)) - 1) Then grn = (1<<nBits(1)) - 1
				If blu > ((1<<nBits(2)) - 1) Then blu = (1<<nBits(2)) - 1
			End If

			intpixel(0) = intpixel(0) Or (red_Renamed << maskOffsets(0)) Or (grn << maskOffsets(1)) Or (blu << maskOffsets(2))

			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte()
				   If pixel Is Nothing Then
					   bdata = New SByte(0){}
				   Else
					   bdata = CType(pixel, SByte())
				   End If
				   bdata(0) = CByte(&Hff And intpixel(0))
				   Return bdata
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short()
				   If pixel Is Nothing Then
					   sdata = New Short(0){}
				   Else
					   sdata = CType(pixel, Short())
				   End If
				   sdata(0) = CShort(Fix(intpixel(0) And &Hffff))
				   Return sdata
				Case DataBuffer.TYPE_INT
				   Return intpixel
			End Select
			Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)

		End Function

		''' <summary>
		''' Returns an array of unnormalized color/alpha components given a pixel
		''' in this <code>ColorModel</code>.  The pixel value is specified as an
		''' <code>int</code>.  If the <code>components</code> array is
		''' <code>null</code>, a new array is allocated.  The
		''' <code>components</code> array is returned.  Color/alpha components are
		''' stored in the <code>components</code> array starting at
		''' <code>offset</code>, even if the array is allocated by this method.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' <code>components</code> array is not <code>null</code> and is not large
		''' enough to hold all the color and alpha components, starting at
		''' <code>offset</code>. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <param name="components"> the array to receive the color and alpha
		''' components of the specified pixel </param>
		''' <param name="offset"> the offset into the <code>components</code> array at
		''' which to start storing the color and alpha components </param>
		''' <returns> an array containing the color and alpha components of the
		''' specified pixel starting at the specified offset. </returns>
		Public NotOverridable Overrides Function getComponents(ByVal pixel As Integer, ByVal components As Integer(), ByVal offset As Integer) As Integer()
			If components Is Nothing Then components = New Integer(offset+numComponents - 1){}

			For i As Integer = 0 To numComponents - 1
				components(offset+i) = CInt(CUInt((pixel And maskArray(i))) >> maskOffsets(i))
			Next i

			Return components
		End Function

		''' <summary>
		''' Returns an array of unnormalized color/alpha components given a pixel
		''' in this <code>ColorModel</code>.  The pixel value is specified by an
		''' array of data elements of type <code>transferType</code> passed in as
		''' an object reference.  If <code>pixel</code> is not a primitive array
		''' of type <code>transferType</code>, a <code>ClassCastException</code>
		''' is thrown.  An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if <code>pixel</code> is not large enough to hold a
		''' pixel value for this <code>ColorModel</code>.  If the
		''' <code>components</code> array is <code>null</code>, a new
		''' array is allocated.  The <code>components</code> array is returned.
		''' Color/alpha components are stored in the <code>components</code> array
		''' starting at <code>offset</code>, even if the array is allocated by
		''' this method.  An <code>ArrayIndexOutOfBoundsException</code>
		''' is thrown if the <code>components</code> array is not
		''' <code>null</code> and is not large enough to hold all the color and
		''' alpha components, starting at <code>offset</code>.
		''' Since <code>DirectColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an unsupported
		''' <code>transferType</code>. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <param name="components"> the array to receive the color and alpha
		'''        components of the specified pixel </param>
		''' <param name="offset"> the offset into the <code>components</code> array at
		'''        which to start storing the color and alpha components </param>
		''' <returns> an array containing the color and alpha components of the
		''' specified pixel starting at the specified offset. </returns>
		''' <exception cref="ClassCastException"> if <code>pixel</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>pixel</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code>, or if <code>components</code>
		'''  is not <code>null</code> and is not large enough to hold all the
		'''  color and alpha components, starting at <code>offset</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this
		'''            <code>transferType</code> is not supported by this
		'''            color model </exception>
		Public NotOverridable Overrides Function getComponents(ByVal pixel As Object, ByVal components As Integer(), ByVal offset As Integer) As Integer()
			Dim intpixel As Integer=0
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
		End Function

		''' <summary>
		''' Creates a <code>WritableRaster</code> with the specified width and
		''' height that has a data layout (<code>SampleModel</code>) compatible
		''' with this <code>ColorModel</code>. </summary>
		''' <param name="w"> the width to apply to the new <code>WritableRaster</code> </param>
		''' <param name="h"> the height to apply to the new <code>WritableRaster</code> </param>
		''' <returns> a <code>WritableRaster</code> object with the specified
		''' width and height. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>w</code> or <code>h</code>
		'''         is less than or equal to zero </exception>
		''' <seealso cref= WritableRaster </seealso>
		''' <seealso cref= SampleModel </seealso>
		Public NotOverridable Overrides Function createCompatibleWritableRaster(ByVal w As Integer, ByVal h As Integer) As WritableRaster
			If (w <= 0) OrElse (h <= 0) Then Throw New IllegalArgumentException("Width (" & w & ") and height (" & h & ") cannot be <= 0")
			Dim bandmasks As Integer()
			If supportsAlpha Then
				bandmasks = New Integer(3){}
				bandmasks(3) = alpha_mask
			Else
				bandmasks = New Integer(2){}
			End If
			bandmasks(0) = red_mask
			bandmasks(1) = green_mask
			bandmasks(2) = blue_mask

			If pixel_bits > 16 Then
				Return Raster.createPackedRaster(DataBuffer.TYPE_INT, w,h,bandmasks,Nothing)
			ElseIf pixel_bits > 8 Then
				Return Raster.createPackedRaster(DataBuffer.TYPE_USHORT, w,h,bandmasks,Nothing)
			Else
				Return Raster.createPackedRaster(DataBuffer.TYPE_BYTE, w,h,bandmasks,Nothing)
			End If
		End Function

		''' <summary>
		''' Returns a pixel value represented as an <code>int</code> in this
		''' <code>ColorModel</code>, given an array of unnormalized color/alpha
		''' components.   An <code>ArrayIndexOutOfBoundsException</code> is
		''' thrown if the <code>components</code> array is
		''' not large enough to hold all the color and alpha components, starting
		''' at <code>offset</code>. </summary>
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
		Public Overrides Function getDataElement(ByVal components As Integer(), ByVal offset As Integer) As Integer
			Dim pixel As Integer = 0
			For i As Integer = 0 To numComponents - 1
				pixel = pixel Or ((components(offset+i)<<maskOffsets(i)) And maskArray(i))
			Next i
			Return pixel
		End Function

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <code>ColorModel</code>, given an array of unnormalized color/alpha
		''' components.
		''' This array can then be passed to the <code>setDataElements</code>
		''' method of a <code>WritableRaster</code> object.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' <code>components</code> array
		''' is not large enough to hold all the color and alpha components,
		''' starting at offset.  If the <code>obj</code> variable is
		''' <code>null</code>, a new array is allocated.  If <code>obj</code> is
		''' not <code>null</code>, it must be a primitive array
		''' of type <code>transferType</code>; otherwise, a
		''' <code>ClassCastException</code> is thrown.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if
		''' <code>obj</code> is not large enough to hold a pixel value for this
		''' <code>ColorModel</code>.
		''' Since <code>DirectColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an unsupported
		''' <code>transferType</code>. </summary>
		''' <param name="components"> an array of unnormalized color and alpha
		''' components </param>
		''' <param name="offset"> the index into <code>components</code> at which to
		''' begin retrieving color and alpha components </param>
		''' <param name="obj"> the <code>Object</code> representing an array of color
		''' and alpha components </param>
		''' <returns> an <code>Object</code> representing an array of color and
		''' alpha components. </returns>
		''' <exception cref="ClassCastException"> if <code>obj</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>obj</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> or the <code>components</code>
		'''  array is not large enough to hold all of the color and alpha
		'''  components starting at <code>offset</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if this
		'''            <code>transferType</code> is not supported by this
		'''            color model </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		Public Overrides Function getDataElements(ByVal components As Integer(), ByVal offset As Integer, ByVal obj As Object) As Object
			Dim pixel As Integer = 0
			For i As Integer = 0 To numComponents - 1
				pixel = pixel Or ((components(offset+i)<<maskOffsets(i)) And maskArray(i))
			Next i
			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   If TypeOf obj Is SByte() Then
					   Dim bdata As SByte() = CType(obj, SByte())
					   bdata(0) = CByte(pixel And &Hff)
					   Return bdata
				   Else
					   Dim bdata As SByte() = {CByte(pixel And &Hff)}
					   Return bdata
				   End If
				Case DataBuffer.TYPE_USHORT
				   If TypeOf obj Is Short() Then
					   Dim sdata As Short() = CType(obj, Short())
					   sdata(0) = CShort(Fix(pixel And &Hffff))
					   Return sdata
				   Else
					   Dim sdata As Short() = {CShort(Fix(pixel And &Hffff))}
					   Return sdata
				   End If
				Case DataBuffer.TYPE_INT
				   If TypeOf obj Is Integer() Then
					   Dim idata As Integer() = CType(obj, Integer())
					   idata(0) = pixel
					   Return idata
				   Else
					   Dim idata As Integer() = {pixel}
					   Return idata
				   End If
				Case Else
				   Throw New ClassCastException("This method has not been " & "implemented for transferType " & transferType)
			End Select
		End Function

		''' <summary>
		''' Forces the raster data to match the state specified in the
		''' <code>isAlphaPremultiplied</code> variable, assuming the data is
		''' currently correctly described by this <code>ColorModel</code>.  It
		''' may multiply or divide the color raster data by alpha, or do
		''' nothing if the data is in the correct state.  If the data needs to
		''' be coerced, this method will also return an instance of this
		''' <code>ColorModel</code> with the <code>isAlphaPremultiplied</code>
		''' flag set appropriately.  This method will throw a
		''' <code>UnsupportedOperationException</code> if this transferType is
		''' not supported by this <code>ColorModel</code>.  Since
		''' <code>ColorModel</code> can be subclassed, subclasses inherit the
		''' implementation of this method and if they don't override it then
		''' they throw an exception if they use an unsupported transferType.
		''' </summary>
		''' <param name="raster"> the <code>WritableRaster</code> data </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the alpha is
		''' premultiplied; <code>false</code> otherwise </param>
		''' <returns> a <code>ColorModel</code> object that represents the
		''' coerced data. </returns>
		''' <exception cref="UnsupportedOperationException"> if this
		'''            <code>transferType</code> is not supported by this
		'''            color model </exception>
		Public NotOverridable Overrides Function coerceData(ByVal raster_Renamed As WritableRaster, ByVal isAlphaPremultiplied As Boolean) As ColorModel
			If (Not supportsAlpha) OrElse Me.alphaPremultiplied = isAlphaPremultiplied Then Return Me

			Dim w As Integer = raster_Renamed.width
			Dim h As Integer = raster_Renamed.height
			Dim aIdx As Integer = numColorComponents
			Dim normAlpha As Single
			Dim alphaScale As Single = 1.0f / (CSng((1 << nBits(aIdx)) - 1))

			Dim rminX As Integer = raster_Renamed.minX
			Dim rY As Integer = raster_Renamed.minY
			Dim rX As Integer
			Dim pixel As Integer() = Nothing
			Dim zpixel As Integer() = Nothing

			If isAlphaPremultiplied Then
				' Must mean that we are currently not premultiplied so
				' multiply by alpha
				Select Case transferType
					Case DataBuffer.TYPE_BYTE
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * normAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Integer(numComponents - 1){}
										Arrays.fill(zpixel, 0)
									End If
									raster_Renamed.pixelxel(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_USHORT
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * normAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Integer(numComponents - 1){}
										Arrays.fill(zpixel, 0)
									End If
									raster_Renamed.pixelxel(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_INT
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * normAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Integer(numComponents - 1){}
										Arrays.fill(zpixel, 0)
									End If
									raster_Renamed.pixelxel(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case Else
						Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
				End Select
			Else
				' We are premultiplied and want to divide it out
				Select Case transferType
					Case DataBuffer.TYPE_BYTE
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * invAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_USHORT
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0 Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * invAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_INT
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = raster_Renamed.getPixel(rX, rY, pixel)
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0 Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * invAlpha + 0.5f))
									Next c
									raster_Renamed.pixelxel(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case Else
						Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
				End Select
			End If

			' Return a new color model
			Return New DirectColorModel(colorSpace, pixel_bits, maskArray(0), maskArray(1), maskArray(2), maskArray(3), isAlphaPremultiplied, transferType)

		End Function

		''' <summary>
		''' Returns <code>true</code> if <code>raster</code> is compatible
		''' with this <code>ColorModel</code> and <code>false</code> if it is
		''' not. </summary>
		''' <param name="raster"> the <seealso cref="Raster"/> object to test for compatibility </param>
		''' <returns> <code>true</code> if <code>raster</code> is compatible
		''' with this <code>ColorModel</code>; <code>false</code> otherwise. </returns>
		Public Overrides Function isCompatibleRaster(ByVal raster_Renamed As Raster) As Boolean
			Dim sm As SampleModel = raster_Renamed.sampleModel
			Dim spsm As SinglePixelPackedSampleModel
			If TypeOf sm Is SinglePixelPackedSampleModel Then
				spsm = CType(sm, SinglePixelPackedSampleModel)
			Else
				Return False
			End If
			If spsm.numBands <> numComponents Then Return False

			Dim bitMasks As Integer() = spsm.bitMasks
			For i As Integer = 0 To numComponents - 1
				If bitMasks(i) <> maskArray(i) Then Return False
			Next i

			Return (raster_Renamed.transferType = transferType)
		End Function

		Private Sub setFields()
			' Set the private fields
			' REMIND: Get rid of these from the native code
			red_mask = maskArray(0)
			red_offset = maskOffsets(0)
			green_mask = maskArray(1)
			green_offset = maskOffsets(1)
			blue_mask = maskArray(2)
			blue_offset = maskOffsets(2)
			If nBits(0) < 8 Then red_scale = (1 << nBits(0)) - 1
			If nBits(1) < 8 Then green_scale = (1 << nBits(1)) - 1
			If nBits(2) < 8 Then blue_scale = (1 << nBits(2)) - 1
			If supportsAlpha Then
				alpha_mask = maskArray(3)
				alpha_offset = maskOffsets(3)
				If nBits(3) < 8 Then alpha_scale = (1 << nBits(3)) - 1
			End If
		End Sub

		''' <summary>
		''' Returns a <code>String</code> that represents this
		''' <code>DirectColorModel</code>. </summary>
		''' <returns> a <code>String</code> representing this
		''' <code>DirectColorModel</code>. </returns>
		Public Overrides Function ToString() As String
			Return New String("DirectColorModel: rmask=" &  java.lang.[Integer].toHexString(red_mask) & " gmask=" &  java.lang.[Integer].toHexString(green_mask) & " bmask=" &  java.lang.[Integer].toHexString(blue_mask) & " amask=" &  java.lang.[Integer].toHexString(alpha_mask))
		End Function
	End Class

End Namespace