Imports Microsoft.VisualBasic
Imports System

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

Namespace java.awt.image


	''' <summary>
	''' A <CODE>ColorModel</CODE> class that works with pixel values that
	''' represent color and alpha information as separate samples and that
	''' store each sample in a separate data element.  This class can be
	''' used with an arbitrary <CODE>ColorSpace</CODE>.  The number of
	''' color samples in the pixel values must be same as the number of
	''' color components in the <CODE>ColorSpace</CODE>. There may be a
	''' single alpha sample.
	''' <p>
	''' For those methods that use
	''' a primitive array pixel representation of type <CODE>transferType</CODE>,
	''' the array length is the same as the number of color and alpha samples.
	''' Color samples are stored first in the array followed by the alpha
	''' sample, if present.  The order of the color samples is specified
	''' by the <CODE>ColorSpace</CODE>.  Typically, this order reflects the
	''' name of the color space type. For example, for <CODE>TYPE_RGB</CODE>,
	''' index 0 corresponds to red, index 1 to green, and index 2 to blue.
	''' <p>
	''' The translation from pixel sample values to color/alpha components for
	''' display or processing purposes is based on a one-to-one correspondence of
	''' samples to components.
	''' Depending on the transfer type used to create an instance of
	''' <code>ComponentColorModel</code>, the pixel sample values
	''' represented by that instance may be signed or unsigned and may
	''' be of integral type or float or double (see below for details).
	''' The translation from sample values to normalized color/alpha components
	''' must follow certain rules.  For float and double samples, the translation
	''' is an identity, i.e. normalized component values are equal to the
	''' corresponding sample values.  For integral samples, the translation
	''' should be only a simple scale and offset, where the scale and offset
	''' constants may be different for each component.  The result of
	''' applying the scale and offset constants is a set of color/alpha
	''' component values, which are guaranteed to fall within a certain
	''' range.  Typically, the range for a color component will be the range
	''' defined by the <code>getMinValue</code> and <code>getMaxValue</code>
	''' methods of the <code>ColorSpace</code> class.  The range for an
	''' alpha component should be 0.0 to 1.0.
	''' <p>
	''' Instances of <code>ComponentColorModel</code> created with transfer types
	''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
	''' and <CODE>DataBuffer.TYPE_INT</CODE> have pixel sample values which
	''' are treated as unsigned integral values.
	''' The number of bits in a color or alpha sample of a pixel value might not
	''' be the same as the number of bits for the corresponding color or alpha
	''' sample passed to the
	''' <code>ComponentColorModel(ColorSpace, int[], boolean, boolean, int, int)</code>
	''' constructor.  In
	''' that case, this class assumes that the least significant n bits of a sample
	''' value hold the component value, where n is the number of significant bits
	''' for the component passed to the constructor.  It also assumes that
	''' any higher-order bits in a sample value are zero.  Thus, sample values
	''' range from 0 to 2<sup>n</sup> - 1.  This class maps these sample values
	''' to normalized color component values such that 0 maps to the value
	''' obtained from the <code>ColorSpace's</code> <code>getMinValue</code>
	''' method for each component and 2<sup>n</sup> - 1 maps to the value
	''' obtained from <code>getMaxValue</code>.  To create a
	''' <code>ComponentColorModel</code> with a different color sample mapping
	''' requires subclassing this class and overriding the
	''' <code>getNormalizedComponents(Object, float[], int)</code> method.
	''' The mapping for an alpha sample always maps 0 to 0.0 and
	''' 2<sup>n</sup> - 1 to 1.0.
	''' <p>
	''' For instances with unsigned sample values,
	''' the unnormalized color/alpha component representation is only
	''' supported if two conditions hold.  First, sample value value 0 must
	''' map to normalized component value 0.0 and sample value 2<sup>n</sup> - 1
	''' to 1.0.  Second the min/max range of all color components of the
	''' <code>ColorSpace</code> must be 0.0 to 1.0.  In this case, the
	''' component representation is the n least
	''' significant bits of the corresponding sample.  Thus each component is
	''' an unsigned integral value between 0 and 2<sup>n</sup> - 1, where
	''' n is the number of significant bits for a particular component.
	''' If these conditions are not met, any method taking an unnormalized
	''' component argument will throw an <code>IllegalArgumentException</code>.
	''' <p>
	''' Instances of <code>ComponentColorModel</code> created with transfer types
	''' <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>, and
	''' <CODE>DataBuffer.TYPE_DOUBLE</CODE> have pixel sample values which
	''' are treated as signed short, float, or double values.
	''' Such instances do not support the unnormalized color/alpha component
	''' representation, so any methods taking such a representation as an argument
	''' will throw an <code>IllegalArgumentException</code> when called on one
	''' of these instances.  The normalized component values of instances
	''' of this class have a range which depends on the transfer
	''' type as follows: for float samples, the full range of the float data
	''' type; for double samples, the full range of the float data type
	''' (resulting from casting double to float); for short samples,
	''' from approximately -maxVal to +maxVal, where maxVal is the per
	''' component maximum value for the <code>ColorSpace</code>
	''' (-32767 maps to -maxVal, 0 maps to 0.0, and 32767 maps
	''' to +maxVal).  A subclass may override the scaling for short sample
	''' values to normalized component values by overriding the
	''' <code>getNormalizedComponents(Object, float[], int)</code> method.
	''' For float and double samples, the normalized component values are
	''' taken to be equal to the corresponding sample values, and subclasses
	''' should not attempt to add any non-identity scaling for these transfer
	''' types.
	''' <p>
	''' Instances of <code>ComponentColorModel</code> created with transfer types
	''' <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>, and
	''' <CODE>DataBuffer.TYPE_DOUBLE</CODE>
	''' use all the bits of all sample values.  Thus all color/alpha components
	''' have 16 bits when using <CODE>DataBuffer.TYPE_SHORT</CODE>, 32 bits when
	''' using <CODE>DataBuffer.TYPE_FLOAT</CODE>, and 64 bits when using
	''' <CODE>DataBuffer.TYPE_DOUBLE</CODE>.  When the
	''' <code>ComponentColorModel(ColorSpace, int[], boolean, boolean, int, int)</code>
	''' form of constructor is used with one of these transfer types, the
	''' bits array argument is ignored.
	''' <p>
	''' It is possible to have color/alpha sample values
	''' which cannot be reasonably interpreted as component values for rendering.
	''' This can happen when <code>ComponentColorModel</code> is subclassed to
	''' override the mapping of unsigned sample values to normalized color
	''' component values or when signed sample values outside a certain range
	''' are used.  (As an example, specifying an alpha component as a signed
	''' short value outside the range 0 to 32767, normalized range 0.0 to 1.0, can
	''' lead to unexpected results.) It is the
	''' responsibility of applications to appropriately scale pixel data before
	''' rendering such that color components fall within the normalized range
	''' of the <code>ColorSpace</code> (obtained using the <code>getMinValue</code>
	''' and <code>getMaxValue</code> methods of the <code>ColorSpace</code> [Class])
	''' and the alpha component is between 0.0 and 1.0.  If color or alpha
	''' component values fall outside these ranges, rendering results are
	''' indeterminate.
	''' <p>
	''' Methods that use a single int pixel representation throw
	''' an <CODE>IllegalArgumentException</CODE>, unless the number of components
	''' for the <CODE>ComponentColorModel</CODE> is one and the component
	''' value is unsigned -- in other words,  a single color component using
	''' a transfer type of <CODE>DataBuffer.TYPE_BYTE</CODE>,
	''' <CODE>DataBuffer.TYPE_USHORT</CODE>, or <CODE>DataBuffer.TYPE_INT</CODE>
	''' and no alpha.
	''' <p>
	''' A <CODE>ComponentColorModel</CODE> can be used in conjunction with a
	''' <CODE>ComponentSampleModel</CODE>, a <CODE>BandedSampleModel</CODE>,
	''' or a <CODE>PixelInterleavedSampleModel</CODE> to construct a
	''' <CODE>BufferedImage</CODE>.
	''' </summary>
	''' <seealso cref= ColorModel </seealso>
	''' <seealso cref= ColorSpace </seealso>
	''' <seealso cref= ComponentSampleModel </seealso>
	''' <seealso cref= BandedSampleModel </seealso>
	''' <seealso cref= PixelInterleavedSampleModel </seealso>
	''' <seealso cref= BufferedImage
	'''  </seealso>
	Public Class ComponentColorModel
		Inherits ColorModel

		''' <summary>
		''' <code>signed</code>  is <code>true</code> for <code>short</code>,
		''' <code>float</code>, and <code>double</code> transfer types; it
		''' is <code>false</code> for <code>byte</code>, <code>ushort</code>,
		''' and <code>int</code> transfer types.
		''' </summary>
		Private signed As Boolean ' true for transfer types short, float, double
								' false for byte, ushort, int
		Private is_sRGB_stdScale As Boolean
		Private is_LinearRGB_stdScale As Boolean
		Private is_LinearGray_stdScale As Boolean
		Private is_ICCGray_stdScale As Boolean
		Private tosRGB8LUT As SByte()
		Private fromsRGB8LUT8 As SByte()
		Private fromsRGB8LUT16 As Short()
		Private fromLinearGray16ToOtherGray8LUT As SByte()
		Private fromLinearGray16ToOtherGray16LUT As Short()
		Private needScaleInit As Boolean
		Private noUnnorm As Boolean
		Private nonStdScale As Boolean
		Private min As Single()
		Private diffMinMax As Single()
		Private compOffset As Single()
		Private compScale As Single()

		''' <summary>
		''' Constructs a <CODE>ComponentColorModel</CODE> from the specified
		''' parameters. Color components will be in the specified
		''' <CODE>ColorSpace</CODE>.  The supported transfer types are
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>,
		''' <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		''' and <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		''' If not null, the <CODE>bits</CODE> array specifies the
		''' number of significant bits per color and alpha component and its
		''' length should be at least the number of components in the
		''' <CODE>ColorSpace</CODE> if there is no alpha
		''' information in the pixel values, or one more than this number if
		''' there is alpha information.  When the <CODE>transferType</CODE> is
		''' <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		''' or <CODE>DataBuffer.TYPE_DOUBLE</CODE> the <CODE>bits</CODE> array
		''' argument is ignored.  <CODE>hasAlpha</CODE> indicates whether alpha
		''' information is present.  If <CODE>hasAlpha</CODE> is true, then
		''' the boolean <CODE>isAlphaPremultiplied</CODE>
		''' specifies how to interpret color and alpha samples in pixel values.
		''' If the boolean is true, color samples are assumed to have been
		''' multiplied by the alpha sample. The <CODE>transparency</CODE>
		''' specifies what alpha values can be represented by this color model.
		''' The acceptable <code>transparency</code> values are
		''' <CODE>OPAQUE</CODE>, <CODE>BITMASK</CODE> or <CODE>TRANSLUCENT</CODE>.
		''' The <CODE>transferType</CODE> is the type of primitive array used
		''' to represent pixel values.
		''' </summary>
		''' <param name="colorSpace">       The <CODE>ColorSpace</CODE> associated
		'''                         with this color model. </param>
		''' <param name="bits">             The number of significant bits per component.
		'''                         May be null, in which case all bits of all
		'''                         component samples will be significant.
		'''                         Ignored if transferType is one of
		'''                         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		'''                         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		'''                         <CODE>DataBuffer.TYPE_DOUBLE</CODE>,
		'''                         in which case all bits of all component
		'''                         samples will be significant. </param>
		''' <param name="hasAlpha">         If true, this color model supports alpha. </param>
		''' <param name="isAlphaPremultiplied"> If true, alpha is premultiplied. </param>
		''' <param name="transparency">     Specifies what alpha values can be represented
		'''                         by this color model. </param>
		''' <param name="transferType">     Specifies the type of primitive array used to
		'''                         represent pixel values.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If the <CODE>bits</CODE> array
		'''         argument is not null, its length is less than the number of
		'''         color and alpha components, and transferType is one of
		'''         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		'''         <CODE>DataBuffer.TYPE_USHORT</CODE>, or
		'''         <CODE>DataBuffer.TYPE_INT</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If transferType is not one of
		'''         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		'''         <CODE>DataBuffer.TYPE_USHORT</CODE>,
		'''         <CODE>DataBuffer.TYPE_INT</CODE>,
		'''         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		'''         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		'''         <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		''' </exception>
		''' <seealso cref= ColorSpace </seealso>
		''' <seealso cref= java.awt.Transparency </seealso>
		Public Sub New(  colorSpace As java.awt.color.ColorSpace,   bits As Integer(),   hasAlpha As Boolean,   isAlphaPremultiplied As Boolean,   transparency As Integer,   transferType As Integer)
			MyBase.New(bitsHelper(transferType, colorSpace, hasAlpha), bitsArrayHelper(bits, transferType, colorSpace, hasAlpha), colorSpace, hasAlpha, isAlphaPremultiplied, transparency, transferType)
			Select Case transferType
				Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT
					signed = False
					needScaleInit = True
				Case DataBuffer.TYPE_SHORT
					signed = True
					needScaleInit = True
				Case DataBuffer.TYPE_FLOAT, DataBuffer.TYPE_DOUBLE
					signed = True
					needScaleInit = False
					noUnnorm = True
					nonStdScale = False
				Case Else
					Throw New IllegalArgumentException("This constructor is not " & "compatible with transferType " & transferType)
			End Select
			setupLUTs()
		End Sub

		''' <summary>
		''' Constructs a <CODE>ComponentColorModel</CODE> from the specified
		''' parameters. Color components will be in the specified
		''' <CODE>ColorSpace</CODE>.  The supported transfer types are
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>,
		''' <CODE>DataBuffer.TYPE_SHORT</CODE>, <CODE>DataBuffer.TYPE_FLOAT</CODE>,
		''' and <CODE>DataBuffer.TYPE_DOUBLE</CODE>.  The number of significant
		''' bits per color and alpha component will be 8, 16, 32, 16, 32,  or 64,
		''' respectively.  The number of color components will be the
		''' number of components in the <CODE>ColorSpace</CODE>.  There will be
		''' an alpha component if <CODE>hasAlpha</CODE> is <CODE>true</CODE>.
		''' If <CODE>hasAlpha</CODE> is true, then
		''' the boolean <CODE>isAlphaPremultiplied</CODE>
		''' specifies how to interpret color and alpha samples in pixel values.
		''' If the boolean is true, color samples are assumed to have been
		''' multiplied by the alpha sample. The <CODE>transparency</CODE>
		''' specifies what alpha values can be represented by this color model.
		''' The acceptable <code>transparency</code> values are
		''' <CODE>OPAQUE</CODE>, <CODE>BITMASK</CODE> or <CODE>TRANSLUCENT</CODE>.
		''' The <CODE>transferType</CODE> is the type of primitive array used
		''' to represent pixel values.
		''' </summary>
		''' <param name="colorSpace">       The <CODE>ColorSpace</CODE> associated
		'''                         with this color model. </param>
		''' <param name="hasAlpha">         If true, this color model supports alpha. </param>
		''' <param name="isAlphaPremultiplied"> If true, alpha is premultiplied. </param>
		''' <param name="transparency">     Specifies what alpha values can be represented
		'''                         by this color model. </param>
		''' <param name="transferType">     Specifies the type of primitive array used to
		'''                         represent pixel values.
		''' </param>
		''' <exception cref="IllegalArgumentException"> If transferType is not one of
		'''         <CODE>DataBuffer.TYPE_BYTE</CODE>,
		'''         <CODE>DataBuffer.TYPE_USHORT</CODE>,
		'''         <CODE>DataBuffer.TYPE_INT</CODE>,
		'''         <CODE>DataBuffer.TYPE_SHORT</CODE>,
		'''         <CODE>DataBuffer.TYPE_FLOAT</CODE>, or
		'''         <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		''' </exception>
		''' <seealso cref= ColorSpace </seealso>
		''' <seealso cref= java.awt.Transparency
		''' @since 1.4 </seealso>
		Public Sub New(  colorSpace As java.awt.color.ColorSpace,   hasAlpha As Boolean,   isAlphaPremultiplied As Boolean,   transparency As Integer,   transferType As Integer)
			Me.New(colorSpace, Nothing, hasAlpha, isAlphaPremultiplied, transparency, transferType)
		End Sub

		Private Shared Function bitsHelper(  transferType As Integer,   colorSpace As java.awt.color.ColorSpace,   hasAlpha As Boolean) As Integer
			Dim numBits As Integer = DataBuffer.getDataTypeSize(transferType)
			Dim numComponents_Renamed As Integer = colorSpace.numComponents
			If hasAlpha Then numComponents_Renamed += 1
			Return numBits * numComponents_Renamed
		End Function

		Private Shared Function bitsArrayHelper(  origBits As Integer(),   transferType As Integer,   colorSpace As java.awt.color.ColorSpace,   hasAlpha As Boolean) As Integer()
			Select Case transferType
				Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT, DataBuffer.TYPE_INT
					If origBits IsNot Nothing Then Return origBits
				Case Else
			End Select
			Dim numBits As Integer = DataBuffer.getDataTypeSize(transferType)
			Dim numComponents_Renamed As Integer = colorSpace.numComponents
			If hasAlpha Then numComponents_Renamed += 1
			Dim bits As Integer() = New Integer(numComponents_Renamed - 1){}
			For i As Integer = 0 To numComponents_Renamed - 1
				bits(i) = numBits
			Next i
			Return bits
		End Function

		Private Sub setupLUTs()
			' REMIND: there is potential to accelerate sRGB, LinearRGB,
			' LinearGray, ICCGray, and non-ICC Gray spaces with non-standard
			' scaling, if that becomes important
			'
			' NOTE: The is_xxx_stdScale and nonStdScale booleans are provisionally
			' set here when this method is called at construction time.  These
			' variables may be set again when initScale is called later.
			' When setupLUTs returns, nonStdScale is true if (the transferType
			' is not float or double) AND (some minimum ColorSpace component
			' value is not 0.0 OR some maximum ColorSpace component value
			' is not 1.0).  This is correct for the calls to
			' getNormalizedComponents(Object, float[], int) from initScale().
			' initScale() may change the value nonStdScale based on the
			' return value of getNormalizedComponents() - this will only
			' happen if getNormalizedComponents() has been overridden by a
			' subclass to make the mapping of min/max pixel sample values
			' something different from min/max color component values.
			If is_sRGB Then
				is_sRGB_stdScale = True
				nonStdScale = False
			ElseIf ColorModel.isLinearRGBspace(colorSpace) Then
				' Note that the built-in Linear RGB space has a normalized
				' range of 0.0 - 1.0 for each coordinate.  Usage of these
				' LUTs makes that assumption.
				is_LinearRGB_stdScale = True
				nonStdScale = False
				If transferType = DataBuffer.TYPE_BYTE Then
					tosRGB8LUT = ColorModel.linearRGB8TosRGB8LUT
					fromsRGB8LUT8 = ColorModel.getsRGB8ToLinearRGB8LUT()
				Else
					tosRGB8LUT = ColorModel.linearRGB16TosRGB8LUT
					fromsRGB8LUT16 = ColorModel.getsRGB8ToLinearRGB16LUT()
				End If
			ElseIf (colorSpaceType = java.awt.color.ColorSpace.TYPE_GRAY) AndAlso (TypeOf colorSpace Is java.awt.color.ICC_ColorSpace) AndAlso (colorSpace.getMinValue(0) = 0.0f) AndAlso (colorSpace.getMaxValue(0) = 1.0f) Then
				' Note that a normalized range of 0.0 - 1.0 for the gray
				' component is required, because usage of these LUTs makes
				' that assumption.
				Dim ics As java.awt.color.ICC_ColorSpace = CType(colorSpace, java.awt.color.ICC_ColorSpace)
				is_ICCGray_stdScale = True
				nonStdScale = False
				fromsRGB8LUT16 = ColorModel.getsRGB8ToLinearRGB16LUT()
				If ColorModel.isLinearGRAYspace(ics) Then
					is_LinearGray_stdScale = True
					If transferType = DataBuffer.TYPE_BYTE Then
						tosRGB8LUT = ColorModel.getGray8TosRGB8LUT(ics)
					Else
						tosRGB8LUT = ColorModel.getGray16TosRGB8LUT(ics)
					End If
				Else
					If transferType = DataBuffer.TYPE_BYTE Then
						tosRGB8LUT = ColorModel.getGray8TosRGB8LUT(ics)
						fromLinearGray16ToOtherGray8LUT = ColorModel.getLinearGray16ToOtherGray8LUT(ics)
					Else
						tosRGB8LUT = ColorModel.getGray16TosRGB8LUT(ics)
						fromLinearGray16ToOtherGray16LUT = ColorModel.getLinearGray16ToOtherGray16LUT(ics)
					End If
				End If
			ElseIf needScaleInit Then
				' if transferType is byte, ushort, int, or short and we
				' don't already know the ColorSpace has minVlaue == 0.0f and
				' maxValue == 1.0f for all components, we need to check that
				' now and setup the min[] and diffMinMax[] arrays if necessary.
				nonStdScale = False
				For i As Integer = 0 To numColorComponents - 1
					If (colorSpace.getMinValue(i) <> 0.0f) OrElse (colorSpace.getMaxValue(i) <> 1.0f) Then
						nonStdScale = True
						Exit For
					End If
				Next i
				If nonStdScale Then
					min = New Single(numColorComponents - 1){}
					diffMinMax = New Single(numColorComponents - 1){}
					For i As Integer = 0 To numColorComponents - 1
						min(i) = colorSpace.getMinValue(i)
						diffMinMax(i) = colorSpace.getMaxValue(i) - min(i)
					Next i
				End If
			End If
		End Sub

		Private Sub initScale()
			' This method is called the first time any method which uses
			' pixel sample value to color component value scaling information
			' is called if the transferType supports non-standard scaling
			' as defined above (byte, ushort, int, and short), unless the
			' method is getNormalizedComponents(Object, float[], int) (that
			' method must be overridden to use non-standard scaling).  This
			' method also sets up the noUnnorm boolean variable for these
			' transferTypes.  After this method is called, the nonStdScale
			' variable will be true if getNormalizedComponents() maps a
			' sample value of 0 to anything other than 0.0f OR maps a
			' sample value of 2^^n - 1 (2^^15 - 1 for short transferType)
			' to anything other than 1.0f.  Note that this can be independent
			' of the colorSpace min/max component values, if the
			' getNormalizedComponents() method has been overridden for some
			' reason, e.g. to provide greater dynamic range in the sample
			' values than in the color component values.  Unfortunately,
			' this method can't be called at construction time, since a
			' subclass may still have uninitialized state that would cause
			' getNormalizedComponents() to return an incorrect result.
			needScaleInit = False ' only needs to called once
			If nonStdScale OrElse signed Then
				' The unnormalized form is only supported for unsigned
				' transferTypes and when the ColorSpace min/max values
				' are 0.0/1.0.  When this method is called nonStdScale is
				' true if the latter condition does not hold.  In addition,
				' the unnormalized form requires that the full range of
				' the pixel sample values map to the full 0.0 - 1.0 range
				' of color component values.  That condition is checked
				' later in this method.
				noUnnorm = True
			Else
				noUnnorm = False
			End If
			Dim lowVal, highVal As Single()
			Select Case transferType
			Case DataBuffer.TYPE_BYTE
					Dim bpixel As SByte() = New SByte(numComponents - 1){}
					For i As Integer = 0 To numColorComponents - 1
						bpixel(i) = 0
					Next i
					If supportsAlpha Then bpixel(numColorComponents) = CByte((1 << nBits(numColorComponents)) - 1)
					lowVal = getNormalizedComponents(bpixel, Nothing, 0)
					For i As Integer = 0 To numColorComponents - 1
						bpixel(i) = CByte((1 << nBits(i)) - 1)
					Next i
					highVal = getNormalizedComponents(bpixel, Nothing, 0)
			Case DataBuffer.TYPE_USHORT
					Dim uspixel As Short() = New Short(numComponents - 1){}
					For i As Integer = 0 To numColorComponents - 1
						uspixel(i) = 0
					Next i
					If supportsAlpha Then uspixel(numColorComponents) = CShort(Fix((1 << nBits(numColorComponents)) - 1))
					lowVal = getNormalizedComponents(uspixel, Nothing, 0)
					For i As Integer = 0 To numColorComponents - 1
						uspixel(i) = CShort(Fix((1 << nBits(i)) - 1))
					Next i
					highVal = getNormalizedComponents(uspixel, Nothing, 0)
			Case DataBuffer.TYPE_INT
					Dim ipixel As Integer() = New Integer(numComponents - 1){}
					For i As Integer = 0 To numColorComponents - 1
						ipixel(i) = 0
					Next i
					If supportsAlpha Then ipixel(numColorComponents) = ((1 << nBits(numColorComponents)) - 1)
					lowVal = getNormalizedComponents(ipixel, Nothing, 0)
					For i As Integer = 0 To numColorComponents - 1
						ipixel(i) = ((1 << nBits(i)) - 1)
					Next i
					highVal = getNormalizedComponents(ipixel, Nothing, 0)
			Case DataBuffer.TYPE_SHORT
					Dim spixel As Short() = New Short(numComponents - 1){}
					For i As Integer = 0 To numColorComponents - 1
						spixel(i) = 0
					Next i
					If supportsAlpha Then spixel(numColorComponents) = 32767
					lowVal = getNormalizedComponents(spixel, Nothing, 0)
					For i As Integer = 0 To numColorComponents - 1
						spixel(i) = 32767
					Next i
					highVal = getNormalizedComponents(spixel, Nothing, 0)
			Case Else
					highVal = Nothing
					lowVal = highVal
			End Select
			nonStdScale = False
			For i As Integer = 0 To numColorComponents - 1
				If (lowVal(i) <> 0.0f) OrElse (highVal(i) <> 1.0f) Then
					nonStdScale = True
					Exit For
				End If
			Next i
			If nonStdScale Then
				noUnnorm = True
				is_sRGB_stdScale = False
				is_LinearRGB_stdScale = False
				is_LinearGray_stdScale = False
				is_ICCGray_stdScale = False
				compOffset = New Single(numColorComponents - 1){}
				compScale = New Single(numColorComponents - 1){}
				For i As Integer = 0 To numColorComponents - 1
					compOffset(i) = lowVal(i)
					compScale(i) = 1.0f / (highVal(i) - lowVal(i))
				Next i
			End If
		End Sub

		Private Function getRGBComponent(  pixel As Integer,   idx As Integer) As Integer
			If numComponents > 1 Then Throw New IllegalArgumentException("More than one component per pixel")
			If signed Then Throw New IllegalArgumentException("Component value is signed")
			If needScaleInit Then initScale()
			' Since there is only 1 component, there is no alpha

			' Normalize the pixel in order to convert it
			Dim opixel As Object = Nothing
			Select Case transferType
			Case DataBuffer.TYPE_BYTE
					Dim bpixel As SByte() = { CByte(pixel) }
					opixel = bpixel
			Case DataBuffer.TYPE_USHORT
					Dim spixel As Short() = { CShort(pixel) }
					opixel = spixel
			Case DataBuffer.TYPE_INT
					Dim ipixel As Integer() = { pixel }
					opixel = ipixel
			End Select
			Dim norm As Single() = getNormalizedComponents(opixel, Nothing, 0)
			Dim rgb_Renamed As Single() = colorSpace.toRGB(norm)

			Return CInt(Fix(rgb_Renamed(idx) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the red color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		''' is done if necessary.  The pixel value is specified as an int.
		''' The returned value will be a non pre-multiplied value.
		''' If the alpha is premultiplied, this method divides
		''' it out before returning the value (if the alpha value is 0,
		''' the red value will be 0).
		''' </summary>
		''' <param name="pixel"> The pixel from which you want to get the red color component.
		''' </param>
		''' <returns> The red color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than
		''' one component in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the component value for this
		''' <CODE>ColorModel</CODE> is signed </exception>
		Public Overrides Function getRed(  pixel As Integer) As Integer
			Return getRGBComponent(pixel, 0)
		End Function

		''' <summary>
		''' Returns the green color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		''' is done if necessary.  The pixel value is specified as an int.
		''' The returned value will be a non
		''' pre-multiplied value. If the alpha is premultiplied, this method
		''' divides it out before returning the value (if the alpha value is 0,
		''' the green value will be 0).
		''' </summary>
		''' <param name="pixel"> The pixel from which you want to get the green color component.
		''' </param>
		''' <returns> The green color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than
		''' one component in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the component value for this
		''' <CODE>ColorModel</CODE> is signed </exception>
		Public Overrides Function getGreen(  pixel As Integer) As Integer
			Return getRGBComponent(pixel, 1)
		End Function

		''' <summary>
		''' Returns the blue color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		''' is done if necessary.  The pixel value is specified as an int.
		''' The returned value will be a non
		''' pre-multiplied value. If the alpha is premultiplied, this method
		''' divides it out before returning the value (if the alpha value is 0,
		''' the blue value will be 0).
		''' </summary>
		''' <param name="pixel"> The pixel from which you want to get the blue color component.
		''' </param>
		''' <returns> The blue color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than
		''' one component in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the component value for this
		''' <CODE>ColorModel</CODE> is signed </exception>
		Public Overrides Function getBlue(  pixel As Integer) As Integer
			Return getRGBComponent(pixel, 2)
		End Function

		''' <summary>
		''' Returns the alpha component for the specified pixel, scaled
		''' from 0 to 255.   The pixel value is specified as an int.
		''' </summary>
		''' <param name="pixel"> The pixel from which you want to get the alpha component.
		''' </param>
		''' <returns> The alpha component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than
		''' one component in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the component value for this
		''' <CODE>ColorModel</CODE> is signed </exception>
		Public Overrides Function getAlpha(  pixel As Integer) As Integer
			If supportsAlpha = False Then Return 255
			If numComponents > 1 Then Throw New IllegalArgumentException("More than one component per pixel")
			If signed Then Throw New IllegalArgumentException("Component value is signed")

			Return CInt(Fix(((CSng(pixel)) / ((1<<nBits(0))-1)) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the color/alpha components of the pixel in the default
		''' RGB color model format.  A color conversion is done if necessary.
		''' The returned value will be in a non pre-multiplied format. If
		''' the alpha is premultiplied, this method divides it out of the
		''' color components (if the alpha value is 0, the color values will be 0).
		''' </summary>
		''' <param name="pixel"> The pixel from which you want to get the color/alpha components.
		''' </param>
		''' <returns> The color/alpha components for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than
		''' one component in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the component value for this
		''' <CODE>ColorModel</CODE> is signed </exception>
		Public Overrides Function getRGB(  pixel As Integer) As Integer
			If numComponents > 1 Then Throw New IllegalArgumentException("More than one component per pixel")
			If signed Then Throw New IllegalArgumentException("Component value is signed")

			Return (getAlpha(pixel) << 24) Or (getRed(pixel) << 16) Or (getGreen(pixel) << 8) Or (getBlue(pixel) << 0)
		End Function

		Private Function extractComponent(  inData As Object,   idx As Integer,   precision As Integer) As Integer
			' Extract component idx from inData.  The precision argument
			' should be either 8 or 16.  If it's 8, this method will return
			' an 8-bit value.  If it's 16, this method will return a 16-bit
			' value for transferTypes other than TYPE_BYTE.  For TYPE_BYTE,
			' an 8-bit value will be returned.

			' This method maps the input value corresponding to a
			' normalized ColorSpace component value of 0.0 to 0, and the
			' input value corresponding to a normalized ColorSpace
			' component value of 1.0 to 2^n - 1 (where n is 8 or 16), so
			' it is appropriate only for ColorSpaces with min/max component
			' values of 0.0/1.0.  This will be true for sRGB, the built-in
			' Linear RGB and Linear Gray spaces, and any other ICC grayscale
			' spaces for which we have precomputed LUTs.

			Dim needAlpha As Boolean = (supportsAlpha AndAlso isAlphaPremultiplied_Renamed)
			Dim alp As Integer = 0
			Dim comp As Integer
			Dim mask As Integer = (1 << nBits(idx)) - 1

			Select Case transferType
				' Note: we do no clamping of the pixel data here - we
				' assume that the data is scaled properly
				Case DataBuffer.TYPE_SHORT
					Dim sdata As Short() = CType(inData, Short())
					Dim scalefactor As Single = CSng((1 << precision) - 1)
					If needAlpha Then
						Dim s As Short = sdata(numColorComponents)
						If s <> CShort(0) Then
							Return CInt(Fix(((CSng(sdata(idx))) / (CSng(s))) * scalefactor + 0.5f))
						Else
							Return 0
						End If
					Else
						Return CInt(Fix((sdata(idx) / 32767.0f) * scalefactor + 0.5f))
					End If
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataBuffer.TYPE_FLOAT
					Dim fdata As Single() = CType(inData, Single())
					Dim scalefactor As Single = CSng((1 << precision) - 1)
					If needAlpha Then
						Dim f As Single = fdata(numColorComponents)
						If f <> 0.0f Then
							Return CInt(Fix(((fdata(idx) / f) * scalefactor) + 0.5f))
						Else
							Return 0
						End If
					Else
						Return CInt(Fix(fdata(idx) * scalefactor + 0.5f))
					End If
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataBuffer.TYPE_DOUBLE
					Dim ddata As Double() = CType(inData, Double())
					Dim scalefactor As Double = CDbl((1 << precision) - 1)
					If needAlpha Then
						Dim d As Double = ddata(numColorComponents)
						If d <> 0.0 Then
							Return CInt(Fix(((ddata(idx) / d) * scalefactor) + 0.5))
						Else
							Return 0
						End If
					Else
						Return CInt(Fix(ddata(idx) * scalefactor + 0.5))
					End If
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   comp = bdata(idx) And mask
				   precision = 8
				   If needAlpha Then alp = bdata(numColorComponents) And mask
				Case DataBuffer.TYPE_USHORT
				   Dim usdata As Short() = CType(inData, Short())
				   comp = usdata(idx) And mask
				   If needAlpha Then alp = usdata(numColorComponents) And mask
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   comp = idata(idx)
				   If needAlpha Then alp = idata(numColorComponents)
				Case Else
				   Throw New UnsupportedOperationException("This method has not " & "been implemented for transferType " & transferType)
			End Select
			If needAlpha Then
				If alp <> 0 Then
					Dim scalefactor As Single = CSng((1 << precision) - 1)
					Dim fcomp As Single = (CSng(comp)) / (CSng(mask))
					Dim invalp As Single = (CSng((1<<nBits(numColorComponents)) - 1)) / (CSng(alp))
					Return CInt(Fix(fcomp * invalp * scalefactor + 0.5f))
				Else
					Return 0
				End If
			Else
				If nBits(idx) <> precision Then
					Dim scalefactor As Single = CSng((1 << precision) - 1)
					Dim fcomp As Single = (CSng(comp)) / (CSng(mask))
					Return CInt(Fix(fcomp * scalefactor + 0.5f))
				End If
				Return comp
			End If
		End Function

		Private Function getRGBComponent(  inData As Object,   idx As Integer) As Integer
			If needScaleInit Then initScale()
			If is_sRGB_stdScale Then
				Return extractComponent(inData, idx, 8)
			ElseIf is_LinearRGB_stdScale Then
				Dim lutidx As Integer = extractComponent(inData, idx, 16)
				Return tosRGB8LUT(lutidx) And &Hff
			ElseIf is_ICCGray_stdScale Then
				Dim lutidx As Integer = extractComponent(inData, 0, 16)
				Return tosRGB8LUT(lutidx) And &Hff
			End If

			' Not CS_sRGB, CS_LINEAR_RGB, or any TYPE_GRAY ICC_ColorSpace
			Dim norm As Single() = getNormalizedComponents(inData, Nothing, 0)
			' Note that getNormalizedComponents returns non-premultiplied values
			Dim rgb_Renamed As Single() = colorSpace.toRGB(norm)
			Return CInt(Fix(rgb_Renamed(idx) * 255.0f + 0.5f))
		End Function

		''' <summary>
		''' Returns the red color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB ColorSpace, sRGB.  A color conversion
		''' is done if necessary.  The <CODE>pixel</CODE> value is specified by an array
		''' of data elements of type <CODE>transferType</CODE> passed in as an object
		''' reference. The returned value will be a non pre-multiplied value. If the
		''' alpha is premultiplied, this method divides it out before returning
		''' the value (if the alpha value is 0, the red value will be 0). Since
		''' <code>ComponentColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't override
		''' it then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> The pixel from which you want to get the red color component,
		''' specified by an array of data elements of type <CODE>transferType</CODE>.
		''' </param>
		''' <returns> The red color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		''' of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		''' large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		Public Overrides Function getRed(  inData As Object) As Integer
			Return getRGBComponent(inData, 0)
		End Function


		''' <summary>
		''' Returns the green color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <CODE>ColorSpace</CODE>, sRGB.
		''' A color conversion is done if necessary.  The <CODE>pixel</CODE> value
		''' is specified by an array of data elements of type <CODE>transferType</CODE>
		''' passed in as an object reference. The returned value is a non pre-multiplied
		''' value. If the alpha is premultiplied, this method divides it out before
		''' returning the value (if the alpha value is 0, the green value will be 0).
		''' Since <code>ComponentColorModel</code> can be subclassed,
		''' subclasses inherit the implementation of this method and if they
		''' don't override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> The pixel from which you want to get the green color component,
		''' specified by an array of data elements of type <CODE>transferType</CODE>.
		''' </param>
		''' <returns> The green color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		''' of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		''' large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		Public Overrides Function getGreen(  inData As Object) As Integer
			Return getRGBComponent(inData, 1)
		End Function


		''' <summary>
		''' Returns the blue color component for the specified pixel, scaled
		''' from 0 to 255 in the default RGB <CODE>ColorSpace</CODE>, sRGB.
		''' A color conversion is done if necessary.  The <CODE>pixel</CODE> value is
		''' specified by an array of data elements of type <CODE>transferType</CODE>
		''' passed in as an object reference. The returned value is a non pre-multiplied
		''' value. If the alpha is premultiplied, this method divides it out before
		''' returning the value (if the alpha value is 0, the blue value will be 0).
		''' Since <code>ComponentColorModel</code> can be subclassed,
		''' subclasses inherit the implementation of this method and if they
		''' don't override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> The pixel from which you want to get the blue color component,
		''' specified by an array of data elements of type <CODE>transferType</CODE>.
		''' </param>
		''' <returns> The blue color component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		''' of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		''' large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		Public Overrides Function getBlue(  inData As Object) As Integer
			Return getRGBComponent(inData, 2)
		End Function

		''' <summary>
		''' Returns the alpha component for the specified pixel, scaled from
		''' 0 to 255.  The pixel value is specified by an array of data
		''' elements of type <CODE>transferType</CODE> passed in as an
		''' object reference.  Since <code>ComponentColorModel</code> can be
		''' subclassed, subclasses inherit the
		''' implementation of this method and if they don't override it then
		''' they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> The pixel from which you want to get the alpha component,
		''' specified by an array of data elements of type <CODE>transferType</CODE>.
		''' </param>
		''' <returns> The alpha component for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		''' of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		''' large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		Public Overrides Function getAlpha(  inData As Object) As Integer
			If supportsAlpha = False Then Return 255

			Dim alpha_Renamed As Integer = 0
			Dim aIdx As Integer = numColorComponents
			Dim mask As Integer = (1 << nBits(aIdx)) - 1

			Select Case transferType
				Case DataBuffer.TYPE_SHORT
					Dim sdata As Short() = CType(inData, Short())
					alpha_Renamed = CInt(Fix((sdata(aIdx) / 32767.0f) * 255.0f + 0.5f))
					Return alpha_Renamed
				Case DataBuffer.TYPE_FLOAT
					Dim fdata As Single() = CType(inData, Single())
					alpha_Renamed = CInt(Fix(fdata(aIdx) * 255.0f + 0.5f))
					Return alpha_Renamed
				Case DataBuffer.TYPE_DOUBLE
					Dim ddata As Double() = CType(inData, Double())
					alpha_Renamed = CInt(Fix(ddata(aIdx) * 255.0 + 0.5))
					Return alpha_Renamed
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte() = CType(inData, SByte())
				   alpha_Renamed = bdata(aIdx) And mask
				Case DataBuffer.TYPE_USHORT
				   Dim usdata As Short() = CType(inData, Short())
				   alpha_Renamed = usdata(aIdx) And mask
				Case DataBuffer.TYPE_INT
				   Dim idata As Integer() = CType(inData, Integer())
				   alpha_Renamed = idata(aIdx)
				Case Else
				   Throw New UnsupportedOperationException("This method has not " & "been implemented for transferType " & transferType)
			End Select

			If nBits(aIdx) = 8 Then
				Return alpha_Renamed
			Else
				Return CInt(Fix(((CSng(alpha_Renamed)) / (CSng((1 << nBits(aIdx)) - 1))) * 255.0f + 0.5f))
			End If
		End Function

		''' <summary>
		''' Returns the color/alpha components for the specified pixel in the
		''' default RGB color model format.  A color conversion is done if
		''' necessary.  The pixel value is specified by an
		''' array of data elements of type <CODE>transferType</CODE> passed
		''' in as an object reference.
		''' The returned value is in a non pre-multiplied format. If
		''' the alpha is premultiplied, this method divides it out of the
		''' color components (if the alpha value is 0, the color values will be 0).
		''' Since <code>ComponentColorModel</code> can be subclassed,
		''' subclasses inherit the implementation of this method and if they
		''' don't override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>.
		''' </summary>
		''' <param name="inData"> The pixel from which you want to get the color/alpha components,
		''' specified by an array of data elements of type <CODE>transferType</CODE>.
		''' </param>
		''' <returns> The color/alpha components for the specified pixel, as an int.
		''' </returns>
		''' <exception cref="ClassCastException"> If <CODE>inData</CODE> is not a primitive array
		''' of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <CODE>inData</CODE> is not
		''' large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		''' <seealso cref= ColorModel#getRGBdefault </seealso>
		Public Overrides Function getRGB(  inData As Object) As Integer
			If needScaleInit Then initScale()
			If is_sRGB_stdScale OrElse is_LinearRGB_stdScale Then
				Return (getAlpha(inData) << 24) Or (getRed(inData) << 16) Or (getGreen(inData) << 8) Or (getBlue(inData))
			ElseIf colorSpaceType = java.awt.color.ColorSpace.TYPE_GRAY Then
				Dim gray As Integer = getRed(inData) ' Red sRGB component should equal
										   ' green and blue components
				Return (getAlpha(inData) << 24) Or (gray << 16) Or (gray << 8) Or gray
			End If
			Dim norm As Single() = getNormalizedComponents(inData, Nothing, 0)
			' Note that getNormalizedComponents returns non-premult values
			Dim rgb_Renamed As Single() = colorSpace.toRGB(norm)
			Return (getAlpha(inData) << 24) Or ((CInt(Fix(rgb_Renamed(0) * 255.0f + 0.5f))) << 16) Or ((CInt(Fix(rgb_Renamed(1) * 255.0f + 0.5f))) << 8) Or ((CInt(Fix(rgb_Renamed(2) * 255.0f + 0.5f))) << 0)
		End Function

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <CODE>ColorModel</CODE>, given an integer pixel representation
		''' in the default RGB color model.
		''' This array can then be passed to the <CODE>setDataElements</CODE>
		''' method of a <CODE>WritableRaster</CODE> object.  If the
		''' <CODE>pixel</CODE>
		''' parameter is null, a new array is allocated.  Since
		''' <code>ComponentColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then
		''' they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="rgb"> the integer representation of the pixel in the RGB
		'''            color model </param>
		''' <param name="pixel"> the specified pixel </param>
		''' <returns> The data element array representation of a pixel
		''' in this <CODE>ColorModel</CODE>. </returns>
		''' <exception cref="ClassCastException"> If <CODE>pixel</CODE> is not null and
		''' is not a primitive array of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If <CODE>pixel</CODE> is
		''' not large enough to hold a pixel value for this
		''' <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>.
		''' </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		Public Overrides Function getDataElements(  rgb As Integer,   pixel As Object) As Object
			' REMIND: Use rendering hints?

			Dim red_Renamed, grn, blu, alp As Integer
			red_Renamed = (rgb>>16) And &Hff
			grn = (rgb>>8) And &Hff
			blu = rgb And &Hff

			If needScaleInit Then initScale()
			If signed Then
				' Handle SHORT, FLOAT, & DOUBLE here

				Select Case transferType
				Case DataBuffer.TYPE_SHORT
						Dim sdata As Short()
						If pixel Is Nothing Then
							sdata = New Short(numComponents - 1){}
						Else
							sdata = CType(pixel, Short())
						End If
						Dim factor As Single
						If is_sRGB_stdScale OrElse is_LinearRGB_stdScale Then
							factor = 32767.0f / 255.0f
							If is_LinearRGB_stdScale Then
								red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
								grn = fromsRGB8LUT16(grn) And &Hffff
								blu = fromsRGB8LUT16(blu) And &Hffff
								factor = 32767.0f / 65535.0f
							End If
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								sdata(3) = CShort(Fix(alp * (32767.0f / 255.0f) + 0.5f))
								If isAlphaPremultiplied_Renamed Then factor = alp * factor * (1.0f / 255.0f)
							End If
							sdata(0) = CShort(Fix(red_Renamed * factor + 0.5f))
							sdata(1) = CShort(Fix(grn * factor + 0.5f))
							sdata(2) = CShort(Fix(blu * factor + 0.5f))
						ElseIf is_LinearGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							Dim gray As Single = ((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f
							factor = 32767.0f
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								sdata(1) = CShort(Fix(alp * (32767.0f / 255.0f) + 0.5f))
								If isAlphaPremultiplied_Renamed Then factor = alp * factor * (1.0f / 255.0f)
							End If
							sdata(0) = CShort(Fix(gray * factor + 0.5f))
						ElseIf is_ICCGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							Dim gray As Integer = CInt(Fix((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu) + 0.5f))
							gray = fromLinearGray16ToOtherGray16LUT(gray) And &Hffff
							factor = 32767.0f / 65535.0f
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								sdata(1) = CShort(Fix(alp * (32767.0f / 255.0f) + 0.5f))
								If isAlphaPremultiplied_Renamed Then factor = alp * factor * (1.0f / 255.0f)
							End If
							sdata(0) = CShort(Fix(gray * factor + 0.5f))
						Else
							factor = 1.0f / 255.0f
							Dim norm As Single() = New Single(2){}
							norm(0) = red_Renamed * factor
							norm(1) = grn * factor
							norm(2) = blu * factor
							norm = colorSpace.fromRGB(norm)
							If nonStdScale Then
								For i As Integer = 0 To numColorComponents - 1
									norm(i) = (norm(i) - compOffset(i)) * compScale(i)
									' REMIND: need to analyze whether this
									' clamping is necessary
									If norm(i) < 0.0f Then norm(i) = 0.0f
									If norm(i) > 1.0f Then norm(i) = 1.0f
								Next i
							End If
							factor = 32767.0f
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								sdata(numColorComponents) = CShort(Fix(alp * (32767.0f / 255.0f) + 0.5f))
								If isAlphaPremultiplied_Renamed Then factor *= alp * (1.0f / 255.0f)
							End If
							For i As Integer = 0 To numColorComponents - 1
								sdata(i) = CShort(Fix(norm(i) * factor + 0.5f))
							Next i
						End If
						Return sdata
				Case DataBuffer.TYPE_FLOAT
						Dim fdata As Single()
						If pixel Is Nothing Then
							fdata = New Single(numComponents - 1){}
						Else
							fdata = CType(pixel, Single())
						End If
						Dim factor As Single
						If is_sRGB_stdScale OrElse is_LinearRGB_stdScale Then
							If is_LinearRGB_stdScale Then
								red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
								grn = fromsRGB8LUT16(grn) And &Hffff
								blu = fromsRGB8LUT16(blu) And &Hffff
								factor = 1.0f / 65535.0f
							Else
								factor = 1.0f / 255.0f
							End If
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								fdata(3) = alp * (1.0f / 255.0f)
								If isAlphaPremultiplied_Renamed Then factor *= fdata(3)
							End If
							fdata(0) = red_Renamed * factor
							fdata(1) = grn * factor
							fdata(2) = blu * factor
						ElseIf is_LinearGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							fdata(0) = ((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								fdata(1) = alp * (1.0f / 255.0f)
								If isAlphaPremultiplied_Renamed Then fdata(0) *= fdata(1)
							End If
						ElseIf is_ICCGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							Dim gray As Integer = CInt(Fix((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu) + 0.5f))
							fdata(0) = (fromLinearGray16ToOtherGray16LUT(gray) And &Hffff) / 65535.0f
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								fdata(1) = alp * (1.0f / 255.0f)
								If isAlphaPremultiplied_Renamed Then fdata(0) *= fdata(1)
							End If
						Else
							Dim norm As Single() = New Single(2){}
							factor = 1.0f / 255.0f
							norm(0) = red_Renamed * factor
							norm(1) = grn * factor
							norm(2) = blu * factor
							norm = colorSpace.fromRGB(norm)
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								fdata(numColorComponents) = alp * factor
								If isAlphaPremultiplied_Renamed Then
									factor *= alp
									For i As Integer = 0 To numColorComponents - 1
										norm(i) *= factor
									Next i
								End If
							End If
							For i As Integer = 0 To numColorComponents - 1
								fdata(i) = norm(i)
							Next i
						End If
						Return fdata
				Case DataBuffer.TYPE_DOUBLE
						Dim ddata As Double()
						If pixel Is Nothing Then
							ddata = New Double(numComponents - 1){}
						Else
							ddata = CType(pixel, Double())
						End If
						If is_sRGB_stdScale OrElse is_LinearRGB_stdScale Then
							Dim factor As Double
							If is_LinearRGB_stdScale Then
								red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
								grn = fromsRGB8LUT16(grn) And &Hffff
								blu = fromsRGB8LUT16(blu) And &Hffff
								factor = 1.0 / 65535.0
							Else
								factor = 1.0 / 255.0
							End If
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								ddata(3) = alp * (1.0 / 255.0)
								If isAlphaPremultiplied_Renamed Then factor *= ddata(3)
							End If
							ddata(0) = red_Renamed * factor
							ddata(1) = grn * factor
							ddata(2) = blu * factor
						ElseIf is_LinearGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							ddata(0) = ((0.2125 * red_Renamed) + (0.7154 * grn) + (0.0721 * blu)) / 65535.0
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								ddata(1) = alp * (1.0 / 255.0)
								If isAlphaPremultiplied_Renamed Then ddata(0) *= ddata(1)
							End If
						ElseIf is_ICCGray_stdScale Then
							red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
							grn = fromsRGB8LUT16(grn) And &Hffff
							blu = fromsRGB8LUT16(blu) And &Hffff
							Dim gray As Integer = CInt(Fix((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu) + 0.5f))
							ddata(0) = (fromLinearGray16ToOtherGray16LUT(gray) And &Hffff) / 65535.0
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								ddata(1) = alp * (1.0 / 255.0)
								If isAlphaPremultiplied_Renamed Then ddata(0) *= ddata(1)
							End If
						Else
							Dim factor As Single = 1.0f / 255.0f
							Dim norm As Single() = New Single(2){}
							norm(0) = red_Renamed * factor
							norm(1) = grn * factor
							norm(2) = blu * factor
							norm = colorSpace.fromRGB(norm)
							If supportsAlpha Then
								alp = (rgb>>24) And &Hff
								ddata(numColorComponents) = alp * (1.0 / 255.0)
								If isAlphaPremultiplied_Renamed Then
									factor *= alp
									For i As Integer = 0 To numColorComponents - 1
										norm(i) *= factor
									Next i
								End If
							End If
							For i As Integer = 0 To numColorComponents - 1
								ddata(i) = norm(i)
							Next i
						End If
						Return ddata
				End Select
			End If

			' Handle BYTE, USHORT, & INT here
			'REMIND: maybe more efficient not to use int array for
			'DataBuffer.TYPE_USHORT and DataBuffer.TYPE_INT
			Dim intpixel As Integer()
			If transferType = DataBuffer.TYPE_INT AndAlso pixel IsNot Nothing Then
			   intpixel = CType(pixel, Integer())
			Else
				intpixel = New Integer(numComponents - 1){}
			End If

			If is_sRGB_stdScale OrElse is_LinearRGB_stdScale Then
				Dim precision As Integer
				Dim factor As Single
				If is_LinearRGB_stdScale Then
					If transferType = DataBuffer.TYPE_BYTE Then
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
					If nBits(3) = 8 Then
						intpixel(3) = alp
					Else
						intpixel(3) = CInt(Fix(alp * (1.0f / 255.0f) * ((1<<nBits(3)) - 1) + 0.5f))
					End If
					If isAlphaPremultiplied_Renamed Then
						factor *= (alp * (1.0f / 255.0f))
						precision = -1 ' force component calculations below
					End If
				End If
				If nBits(0) = precision Then
					intpixel(0) = red_Renamed
				Else
					intpixel(0) = CInt(Fix(red_Renamed * factor * ((1<<nBits(0)) - 1) + 0.5f))
				End If
				If nBits(1) = precision Then
					intpixel(1) = CInt(grn)
				Else
					intpixel(1) = CInt(Fix(grn * factor * ((1<<nBits(1)) - 1) + 0.5f))
				End If
				If nBits(2) = precision Then
					intpixel(2) = CInt(blu)
				Else
					intpixel(2) = CInt(Fix(blu * factor * ((1<<nBits(2)) - 1) + 0.5f))
				End If
			ElseIf is_LinearGray_stdScale Then
				red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
				grn = fromsRGB8LUT16(grn) And &Hffff
				blu = fromsRGB8LUT16(blu) And &Hffff
				Dim gray As Single = ((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu)) / 65535.0f
				If supportsAlpha Then
					alp = (rgb>>24) And &Hff
					If nBits(1) = 8 Then
						intpixel(1) = alp
					Else
						intpixel(1) = CInt(Fix(alp * (1.0f / 255.0f) * ((1 << nBits(1)) - 1) + 0.5f))
					End If
					If isAlphaPremultiplied_Renamed Then gray *= (alp * (1.0f / 255.0f))
				End If
				intpixel(0) = CInt(Fix(gray * ((1 << nBits(0)) - 1) + 0.5f))
			ElseIf is_ICCGray_stdScale Then
				red_Renamed = fromsRGB8LUT16(red_Renamed) And &Hffff
				grn = fromsRGB8LUT16(grn) And &Hffff
				blu = fromsRGB8LUT16(blu) And &Hffff
				Dim gray16 As Integer = CInt(Fix((0.2125f * red_Renamed) + (0.7154f * grn) + (0.0721f * blu) + 0.5f))
				Dim gray As Single = (fromLinearGray16ToOtherGray16LUT(gray16) And &Hffff) / 65535.0f
				If supportsAlpha Then
					alp = (rgb>>24) And &Hff
					If nBits(1) = 8 Then
						intpixel(1) = alp
					Else
						intpixel(1) = CInt(Fix(alp * (1.0f / 255.0f) * ((1 << nBits(1)) - 1) + 0.5f))
					End If
					If isAlphaPremultiplied_Renamed Then gray *= (alp * (1.0f / 255.0f))
				End If
				intpixel(0) = CInt(Fix(gray * ((1 << nBits(0)) - 1) + 0.5f))
			Else
				' Need to convert the color
				Dim norm As Single() = New Single(2){}
				Dim factor As Single = 1.0f / 255.0f
				norm(0) = red_Renamed * factor
				norm(1) = grn * factor
				norm(2) = blu * factor
				norm = colorSpace.fromRGB(norm)
				If nonStdScale Then
					For i As Integer = 0 To numColorComponents - 1
						norm(i) = (norm(i) - compOffset(i)) * compScale(i)
						' REMIND: need to analyze whether this
						' clamping is necessary
						If norm(i) < 0.0f Then norm(i) = 0.0f
						If norm(i) > 1.0f Then norm(i) = 1.0f
					Next i
				End If
				If supportsAlpha Then
					alp = (rgb>>24) And &Hff
					If nBits(numColorComponents) = 8 Then
						intpixel(numColorComponents) = alp
					Else
						intpixel(numColorComponents) = CInt(Fix(alp * factor * ((1<<nBits(numColorComponents)) - 1) + 0.5f))
					End If
					If isAlphaPremultiplied_Renamed Then
						factor *= alp
						For i As Integer = 0 To numColorComponents - 1
							norm(i) *= factor
						Next i
					End If
				End If
				For i As Integer = 0 To numColorComponents - 1
					intpixel(i) = CInt(Fix(norm(i) * ((1<<nBits(i)) - 1) + 0.5f))
				Next i
			End If

			Select Case transferType
				Case DataBuffer.TYPE_BYTE
				   Dim bdata As SByte()
				   If pixel Is Nothing Then
					   bdata = New SByte(numComponents - 1){}
				   Else
					   bdata = CType(pixel, SByte())
				   End If
				   For i As Integer = 0 To numComponents - 1
					   bdata(i) = CByte(&Hff And intpixel(i))
				   Next i
				   Return bdata
				Case DataBuffer.TYPE_USHORT
				   Dim sdata As Short()
				   If pixel Is Nothing Then
					   sdata = New Short(numComponents - 1){}
				   Else
					   sdata = CType(pixel, Short())
				   End If
				   For i As Integer = 0 To numComponents - 1
					   sdata(i) = CShort(Fix(intpixel(i) And &Hffff))
				   Next i
				   Return sdata
				Case DataBuffer.TYPE_INT
					If maxBits > 23 Then
						' fix 4412670 - for components of 24 or more bits
						' some calculations done above with float precision
						' may lose enough precision that the integer result
						' overflows nBits, so we need to clamp.
						For i As Integer = 0 To numComponents - 1
							If intpixel(i) > ((1<<nBits(i)) - 1) Then intpixel(i) = (1<<nBits(i)) - 1
						Next i
					End If
					Return intpixel
			End Select
			Throw New IllegalArgumentException("This method has not been " & "implemented for transferType " & transferType)
		End Function

	   ''' <summary>
	   ''' Returns an array of unnormalized color/alpha components given a pixel
	   ''' in this <CODE>ColorModel</CODE>.
	   ''' An IllegalArgumentException is thrown if the component value for this
	   ''' <CODE>ColorModel</CODE> is not conveniently representable in the
	   ''' unnormalized form.  Color/alpha components are stored
	   ''' in the <CODE>components</CODE> array starting at <CODE>offset</CODE>
	   ''' (even if the array is allocated by this method).
	   ''' </summary>
	   ''' <param name="pixel"> The pixel value specified as an  java.lang.[Integer]. </param>
	   ''' <param name="components"> An integer array in which to store the unnormalized
	   ''' color/alpha components. If the <CODE>components</CODE> array is null,
	   ''' a new array is allocated. </param>
	   ''' <param name="offset"> An offset into the <CODE>components</CODE> array.
	   ''' </param>
	   ''' <returns> The components array.
	   ''' </returns>
	   ''' <exception cref="IllegalArgumentException"> If there is more than one
	   ''' component in this <CODE>ColorModel</CODE>. </exception>
	   ''' <exception cref="IllegalArgumentException"> If this
	   ''' <CODE>ColorModel</CODE> does not support the unnormalized form </exception>
	   ''' <exception cref="ArrayIndexOutOfBoundsException"> If the <CODE>components</CODE>
	   ''' array is not null and is not large enough to hold all the color and
	   ''' alpha components (starting at offset). </exception>
		Public Overrides Function getComponents(  pixel As Integer,   components As Integer(),   offset As Integer) As Integer()
			If numComponents > 1 Then Throw New IllegalArgumentException("More than one component per pixel")
			If needScaleInit Then initScale()
			If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
			If components Is Nothing Then components = New Integer(offset){}

			components(offset+0) = (pixel And ((1<<nBits(0)) - 1))
			Return components
		End Function

		''' <summary>
		''' Returns an array of unnormalized color/alpha components given a pixel
		''' in this <CODE>ColorModel</CODE>.  The pixel value is specified by an
		''' array of data elements of type <CODE>transferType</CODE> passed in as
		''' an object reference.
		''' An IllegalArgumentException is thrown if the component values for this
		''' <CODE>ColorModel</CODE> are not conveniently representable in the
		''' unnormalized form.
		''' Color/alpha components are stored in the <CODE>components</CODE> array
		''' starting at  <CODE>offset</CODE> (even if the array is allocated by
		''' this method).  Since <code>ComponentColorModel</code> can be
		''' subclassed, subclasses inherit the
		''' implementation of this method and if they don't override it then
		''' this method might throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <param name="pixel"> A pixel value specified by an array of data elements of
		''' type <CODE>transferType</CODE>. </param>
		''' <param name="components"> An integer array in which to store the unnormalized
		''' color/alpha components. If the <CODE>components</CODE> array is null,
		''' a new array is allocated. </param>
		''' <param name="offset"> An offset into the <CODE>components</CODE> array.
		''' </param>
		''' <returns> The <CODE>components</CODE> array.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If this
		''' <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		''' <exception cref="UnsupportedOperationException"> in some cases iff the
		''' transfer type of this <CODE>ComponentColorModel</CODE>
		''' is not one of the following transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' or <CODE>DataBuffer.TYPE_INT</CODE>. </exception>
		''' <exception cref="ClassCastException"> If <CODE>pixel</CODE> is not a primitive
		''' array of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If the <CODE>components</CODE> array is
		''' not null and is not large enough to hold all the color and alpha
		''' components (starting at offset), or if <CODE>pixel</CODE> is not large
		''' enough to hold a pixel value for this ColorModel. </exception>
		Public Overrides Function getComponents(  pixel As Object,   components As Integer(),   offset As Integer) As Integer()
			Dim intpixel As Integer()
			If needScaleInit Then initScale()
			If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
			If TypeOf pixel Is Integer() Then
				intpixel = CType(pixel, Integer())
			Else
				intpixel = DataBuffer.toIntArray(pixel)
				If intpixel Is Nothing Then Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End If
			If intpixel.Length < numComponents Then Throw New IllegalArgumentException("Length of pixel array < number of components in model")
			If components Is Nothing Then
				components = New Integer(offset+numComponents - 1){}
			ElseIf (components.Length-offset) < numComponents Then
				Throw New IllegalArgumentException("Length of components array < number of components in model")
			End If
			Array.Copy(intpixel, 0, components, offset, numComponents)

			Return components
		End Function

		''' <summary>
		''' Returns an array of all of the color/alpha components in unnormalized
		''' form, given a normalized component array.  Unnormalized components
		''' are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		''' n is the number of bits for a particular component.  Normalized
		''' components are float values between a per component minimum and
		''' maximum specified by the <code>ColorSpace</code> object for this
		''' <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		''' will be thrown if color component values for this
		''' <code>ColorModel</code> are not conveniently representable in the
		''' unnormalized form.  If the
		''' <code>components</code> array is <code>null</code>, a new array
		''' will be allocated.  The <code>components</code> array will
		''' be returned.  Color/alpha components are stored in the
		''' <code>components</code> array starting at <code>offset</code> (even
		''' if the array is allocated by this method). An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if the
		''' <code>components</code> array is not <code>null</code> and is not
		''' large enough to hold all the color and alpha
		''' components (starting at <code>offset</code>).  An
		''' <code>IllegalArgumentException</code> is thrown if the
		''' <code>normComponents</code> array is not large enough to hold
		''' all the color and alpha components starting at
		''' <code>normOffset</code>. </summary>
		''' <param name="normComponents"> an array containing normalized components </param>
		''' <param name="normOffset"> the offset into the <code>normComponents</code>
		''' array at which to start retrieving normalized components </param>
		''' <param name="components"> an array that receives the components from
		''' <code>normComponents</code> </param>
		''' <param name="offset"> the index into <code>components</code> at which to
		''' begin storing normalized components from
		''' <code>normComponents</code> </param>
		''' <returns> an array containing unnormalized color and alpha
		''' components. </returns>
		''' <exception cref="IllegalArgumentException"> If this
		''' <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		''' <exception cref="IllegalArgumentException"> if the length of
		'''          <code>normComponents</code> minus <code>normOffset</code>
		'''          is less than <code>numComponents</code> </exception>
		Public Overrides Function getUnnormalizedComponents(  normComponents As Single(),   normOffset As Integer,   components As Integer(),   offset As Integer) As Integer()
			If needScaleInit Then initScale()
			If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
			Return MyBase.getUnnormalizedComponents(normComponents, normOffset, components, offset)
		End Function

		''' <summary>
		''' Returns an array of all of the color/alpha components in normalized
		''' form, given an unnormalized component array.  Unnormalized components
		''' are unsigned integral values between 0 and 2<sup>n</sup> - 1, where
		''' n is the number of bits for a particular component.  Normalized
		''' components are float values between a per component minimum and
		''' maximum specified by the <code>ColorSpace</code> object for this
		''' <code>ColorModel</code>.  An <code>IllegalArgumentException</code>
		''' will be thrown if color component values for this
		''' <code>ColorModel</code> are not conveniently representable in the
		''' unnormalized form.  If the
		''' <code>normComponents</code> array is <code>null</code>, a new array
		''' will be allocated.  The <code>normComponents</code> array
		''' will be returned.  Color/alpha components are stored in the
		''' <code>normComponents</code> array starting at
		''' <code>normOffset</code> (even if the array is allocated by this
		''' method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		''' if the <code>normComponents</code> array is not <code>null</code>
		''' and is not large enough to hold all the color and alpha components
		''' (starting at <code>normOffset</code>).  An
		''' <code>IllegalArgumentException</code> is thrown if the
		''' <code>components</code> array is not large enough to hold all the
		''' color and alpha components starting at <code>offset</code>. </summary>
		''' <param name="components"> an array containing unnormalized components </param>
		''' <param name="offset"> the offset into the <code>components</code> array at
		''' which to start retrieving unnormalized components </param>
		''' <param name="normComponents"> an array that receives the normalized components </param>
		''' <param name="normOffset"> the index into <code>normComponents</code> at
		''' which to begin storing normalized components </param>
		''' <returns> an array containing normalized color and alpha
		''' components. </returns>
		''' <exception cref="IllegalArgumentException"> If this
		''' <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		Public Overrides Function getNormalizedComponents(  components As Integer(),   offset As Integer,   normComponents As Single(),   normOffset As Integer) As Single()
			If needScaleInit Then initScale()
			If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
			Return MyBase.getNormalizedComponents(components, offset, normComponents, normOffset)
		End Function

		''' <summary>
		''' Returns a pixel value represented as an int in this <CODE>ColorModel</CODE>,
		''' given an array of unnormalized color/alpha components.
		''' </summary>
		''' <param name="components"> An array of unnormalized color/alpha components. </param>
		''' <param name="offset"> An offset into the <CODE>components</CODE> array.
		''' </param>
		''' <returns> A pixel value represented as an int.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If there is more than one component
		''' in this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If this
		''' <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		Public Overrides Function getDataElement(  components As Integer(),   offset As Integer) As Integer
			If needScaleInit Then initScale()
			If numComponents = 1 Then
				If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
				Return components(offset+0)
			End If
			Throw New IllegalArgumentException("This model returns " & numComponents & " elements in the pixel array.")
		End Function

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <CODE>ColorModel</CODE>, given an array of unnormalized color/alpha
		''' components. This array can then be passed to the <CODE>setDataElements</CODE>
		''' method of a <CODE>WritableRaster</CODE> object.
		''' </summary>
		''' <param name="components"> An array of unnormalized color/alpha components. </param>
		''' <param name="offset"> The integer offset into the <CODE>components</CODE> array. </param>
		''' <param name="obj"> The object in which to store the data element array
		''' representation of the pixel. If <CODE>obj</CODE> variable is null,
		''' a new array is allocated.  If <CODE>obj</CODE> is not null, it must
		''' be a primitive array of type <CODE>transferType</CODE>. An
		''' <CODE>ArrayIndexOutOfBoundsException</CODE> is thrown if
		''' <CODE>obj</CODE> is not large enough to hold a pixel value
		''' for this <CODE>ColorModel</CODE>.  Since
		''' <code>ComponentColorModel</code> can be subclassed, subclasses
		''' inherit the implementation of this method and if they don't
		''' override it then they throw an exception if they use an
		''' unsupported <code>transferType</code>.
		''' </param>
		''' <returns> The data element array representation of a pixel
		''' in this <CODE>ColorModel</CODE>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> If the components array
		''' is not large enough to hold all the color and alpha components
		''' (starting at offset). </exception>
		''' <exception cref="ClassCastException"> If <CODE>obj</CODE> is not null and is not a
		''' primitive  array of type <CODE>transferType</CODE>. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> If <CODE>obj</CODE> is not large
		''' enough to hold a pixel value for this <CODE>ColorModel</CODE>. </exception>
		''' <exception cref="IllegalArgumentException"> If this
		''' <CODE>ComponentColorModel</CODE> does not support the unnormalized form </exception>
		''' <exception cref="UnsupportedOperationException"> If the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the following transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' or <CODE>DataBuffer.TYPE_INT</CODE>.
		''' </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements </seealso>
		Public Overrides Function getDataElements(  components As Integer(),   offset As Integer,   obj As Object) As Object
			If needScaleInit Then initScale()
			If noUnnorm Then Throw New IllegalArgumentException("This ColorModel does not support the unnormalized form")
			If (components.Length-offset) < numComponents Then Throw New IllegalArgumentException("Component array too small" & " (should be " & numComponents)
			Select Case transferType
			Case DataBuffer.TYPE_INT
					Dim pixel As Integer()
					If obj Is Nothing Then
						pixel = New Integer(numComponents - 1){}
					Else
						pixel = CType(obj, Integer())
					End If
					Array.Copy(components, offset, pixel, 0, numComponents)
					Return pixel

			Case DataBuffer.TYPE_BYTE
					Dim pixel As SByte()
					If obj Is Nothing Then
						pixel = New SByte(numComponents - 1){}
					Else
						pixel = CType(obj, SByte())
					End If
					For i As Integer = 0 To numComponents - 1
						pixel(i) = CByte(components(offset+i) And &Hff)
					Next i
					Return pixel

			Case DataBuffer.TYPE_USHORT
					Dim pixel As Short()
					If obj Is Nothing Then
						pixel = New Short(numComponents - 1){}
					Else
						pixel = CType(obj, Short())
					End If
					For i As Integer = 0 To numComponents - 1
						pixel(i) = CShort(Fix(components(offset+i) And &Hffff))
					Next i
					Return pixel

			Case Else
				Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
		End Function

		''' <summary>
		''' Returns a pixel value represented as an <code>int</code> in this
		''' <code>ColorModel</code>, given an array of normalized color/alpha
		''' components.  This method will throw an
		''' <code>IllegalArgumentException</code> if pixel values for this
		''' <code>ColorModel</code> are not conveniently representable as a
		''' single <code>int</code>.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if  the
		''' <code>normComponents</code> array is not large enough to hold all the
		''' color and alpha components (starting at <code>normOffset</code>). </summary>
		''' <param name="normComponents"> an array of normalized color and alpha
		''' components </param>
		''' <param name="normOffset"> the index into <code>normComponents</code> at which to
		''' begin retrieving the color and alpha components </param>
		''' <returns> an <code>int</code> pixel value in this
		''' <code>ColorModel</code> corresponding to the specified components. </returns>
		''' <exception cref="IllegalArgumentException"> if
		'''  pixel values for this <code>ColorModel</code> are not
		'''  conveniently representable as a single <code>int</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  the <code>normComponents</code> array is not large enough to
		'''  hold all of the color and alpha components starting at
		'''  <code>normOffset</code>
		''' @since 1.4 </exception>
		Public Overrides Function getDataElement(  normComponents As Single(),   normOffset As Integer) As Integer
			If numComponents > 1 Then Throw New IllegalArgumentException("More than one component per pixel")
			If signed Then Throw New IllegalArgumentException("Component value is signed")
			If needScaleInit Then initScale()
			Dim pixel As Object = getDataElements(normComponents, normOffset, Nothing)
			Select Case transferType
			Case DataBuffer.TYPE_BYTE
					Dim bpixel As SByte() = CType(pixel, SByte())
					Return bpixel(0) And &Hff
			Case DataBuffer.TYPE_USHORT
					Dim uspixel As Short() = CType(pixel, Short())
					Return uspixel(0) And &Hffff
			Case DataBuffer.TYPE_INT
					Dim ipixel As Integer() = CType(pixel, Integer())
					Return ipixel(0)
			Case Else
				Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
		End Function

		''' <summary>
		''' Returns a data element array representation of a pixel in this
		''' <code>ColorModel</code>, given an array of normalized color/alpha
		''' components.  This array can then be passed to the
		''' <code>setDataElements</code> method of a <code>WritableRaster</code>
		''' object.  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		''' if the <code>normComponents</code> array is not large enough to hold
		''' all the color and alpha components (starting at
		''' <code>normOffset</code>).  If the <code>obj</code> variable is
		''' <code>null</code>, a new array will be allocated.  If
		''' <code>obj</code> is not <code>null</code>, it must be a primitive
		''' array of type transferType; otherwise, a
		''' <code>ClassCastException</code> is thrown.  An
		''' <code>ArrayIndexOutOfBoundsException</code> is thrown if
		''' <code>obj</code> is not large enough to hold a pixel value for this
		''' <code>ColorModel</code>. </summary>
		''' <param name="normComponents"> an array of normalized color and alpha
		''' components </param>
		''' <param name="normOffset"> the index into <code>normComponents</code> at which to
		''' begin retrieving color and alpha components </param>
		''' <param name="obj"> a primitive data array to hold the returned pixel </param>
		''' <returns> an <code>Object</code> which is a primitive data array
		''' representation of a pixel </returns>
		''' <exception cref="ClassCastException"> if <code>obj</code>
		'''  is not a primitive array of type <code>transferType</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''  <code>obj</code> is not large enough to hold a pixel value
		'''  for this <code>ColorModel</code> or the <code>normComponents</code>
		'''  array is not large enough to hold all of the color and alpha
		'''  components starting at <code>normOffset</code> </exception>
		''' <seealso cref= WritableRaster#setDataElements </seealso>
		''' <seealso cref= SampleModel#setDataElements
		''' @since 1.4 </seealso>
		Public Overrides Function getDataElements(  normComponents As Single(),   normOffset As Integer,   obj As Object) As Object
			Dim needAlpha As Boolean = supportsAlpha AndAlso isAlphaPremultiplied_Renamed
			Dim stdNormComponents As Single()
			If needScaleInit Then initScale()
			If nonStdScale Then
				stdNormComponents = New Single(numComponents - 1){}
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numColorComponents
					stdNormComponents(c) = (normComponents(nc) - compOffset(c)) * compScale(c)
					' REMIND: need to analyze whether this
					' clamping is necessary
					If stdNormComponents(c) < 0.0f Then stdNormComponents(c) = 0.0f
					If stdNormComponents(c) > 1.0f Then stdNormComponents(c) = 1.0f
					c += 1
					nc += 1
				Loop
				If supportsAlpha Then stdNormComponents(numColorComponents) = normComponents(numColorComponents + normOffset)
				normOffset = 0
			Else
				stdNormComponents = normComponents
			End If
			Select Case transferType
			Case DataBuffer.TYPE_BYTE
				Dim bpixel As SByte()
				If obj Is Nothing Then
					bpixel = New SByte(numComponents - 1){}
				Else
					bpixel = CType(obj, SByte())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Single = stdNormComponents(numColorComponents + normOffset)
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						bpixel(c) = CByte((stdNormComponents(nc) * alpha_Renamed) * (CSng((1 << nBits(c)) - 1)) + 0.5f)
						c += 1
						nc += 1
					Loop
					bpixel(numColorComponents) = CByte(alpha_Renamed * (CSng((1 << nBits(numColorComponents)) - 1)) + 0.5f)
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						bpixel(c) = CByte(stdNormComponents(nc) * (CSng((1 << nBits(c)) - 1)) + 0.5f)
						c += 1
						nc += 1
					Loop
				End If
				Return bpixel
			Case DataBuffer.TYPE_USHORT
				Dim uspixel As Short()
				If obj Is Nothing Then
					uspixel = New Short(numComponents - 1){}
				Else
					uspixel = CType(obj, Short())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Single = stdNormComponents(numColorComponents + normOffset)
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						uspixel(c) = CShort(Fix((stdNormComponents(nc) * alpha_Renamed) * (CSng((1 << nBits(c)) - 1)) + 0.5f))
						c += 1
						nc += 1
					Loop
					uspixel(numColorComponents) = CShort(Fix(alpha_Renamed * (CSng((1 << nBits(numColorComponents)) - 1)) + 0.5f))
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						uspixel(c) = CShort(Fix(stdNormComponents(nc) * (CSng((1 << nBits(c)) - 1)) + 0.5f))
						c += 1
						nc += 1
					Loop
				End If
				Return uspixel
			Case DataBuffer.TYPE_INT
				Dim ipixel As Integer()
				If obj Is Nothing Then
					ipixel = New Integer(numComponents - 1){}
				Else
					ipixel = CType(obj, Integer())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Single = stdNormComponents(numColorComponents + normOffset)
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						ipixel(c) = CInt(Fix((stdNormComponents(nc) * alpha_Renamed) * (CSng((1 << nBits(c)) - 1)) + 0.5f))
						c += 1
						nc += 1
					Loop
					ipixel(numColorComponents) = CInt(Fix(alpha_Renamed * (CSng((1 << nBits(numColorComponents)) - 1)) + 0.5f))
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						ipixel(c) = CInt(Fix(stdNormComponents(nc) * (CSng((1 << nBits(c)) - 1)) + 0.5f))
						c += 1
						nc += 1
					Loop
				End If
				Return ipixel
			Case DataBuffer.TYPE_SHORT
				Dim spixel As Short()
				If obj Is Nothing Then
					spixel = New Short(numComponents - 1){}
				Else
					spixel = CType(obj, Short())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Single = stdNormComponents(numColorComponents + normOffset)
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						spixel(c) = CShort(Fix(stdNormComponents(nc) * alpha_Renamed * 32767.0f + 0.5f))
						c += 1
						nc += 1
					Loop
					spixel(numColorComponents) = CShort(Fix(alpha_Renamed * 32767.0f + 0.5f))
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						spixel(c) = CShort(Fix(stdNormComponents(nc) * 32767.0f + 0.5f))
						c += 1
						nc += 1
					Loop
				End If
				Return spixel
			Case DataBuffer.TYPE_FLOAT
				Dim fpixel As Single()
				If obj Is Nothing Then
					fpixel = New Single(numComponents - 1){}
				Else
					fpixel = CType(obj, Single())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Single = normComponents(numColorComponents + normOffset)
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						fpixel(c) = normComponents(nc) * alpha_Renamed
						c += 1
						nc += 1
					Loop
					fpixel(numColorComponents) = alpha_Renamed
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						fpixel(c) = normComponents(nc)
						c += 1
						nc += 1
					Loop
				End If
				Return fpixel
			Case DataBuffer.TYPE_DOUBLE
				Dim dpixel As Double()
				If obj Is Nothing Then
					dpixel = New Double(numComponents - 1){}
				Else
					dpixel = CType(obj, Double())
				End If
				If needAlpha Then
					Dim alpha_Renamed As Double = CDbl(normComponents(numColorComponents + normOffset))
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numColorComponents
						dpixel(c) = normComponents(nc) * alpha_Renamed
						c += 1
						nc += 1
					Loop
					dpixel(numColorComponents) = alpha_Renamed
				Else
					Dim c As Integer = 0
					Dim nc As Integer = normOffset
					Do While c < numComponents
						dpixel(c) = CDbl(normComponents(nc))
						c += 1
						nc += 1
					Loop
				End If
				Return dpixel
			Case Else
				Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select
		End Function

		''' <summary>
		''' Returns an array of all of the color/alpha components in normalized
		''' form, given a pixel in this <code>ColorModel</code>.  The pixel
		''' value is specified by an array of data elements of type transferType
		''' passed in as an object reference.  If pixel is not a primitive array
		''' of type transferType, a <code>ClassCastException</code> is thrown.
		''' An <code>ArrayIndexOutOfBoundsException</code> is thrown if
		''' <code>pixel</code> is not large enough to hold a pixel value for this
		''' <code>ColorModel</code>.
		''' Normalized components are float values between a per component minimum
		''' and maximum specified by the <code>ColorSpace</code> object for this
		''' <code>ColorModel</code>.  If the
		''' <code>normComponents</code> array is <code>null</code>, a new array
		''' will be allocated.  The <code>normComponents</code> array
		''' will be returned.  Color/alpha components are stored in the
		''' <code>normComponents</code> array starting at
		''' <code>normOffset</code> (even if the array is allocated by this
		''' method).  An <code>ArrayIndexOutOfBoundsException</code> is thrown
		''' if the <code>normComponents</code> array is not <code>null</code>
		''' and is not large enough to hold all the color and alpha components
		''' (starting at <code>normOffset</code>).
		''' <p>
		''' This method must be overridden by a subclass if that subclass
		''' is designed to translate pixel sample values to color component values
		''' in a non-default way.  The default translations implemented by this
		''' class is described in the class comments.  Any subclass implementing
		''' a non-default translation must follow the constraints on allowable
		''' translations defined there. </summary>
		''' <param name="pixel"> the specified pixel </param>
		''' <param name="normComponents"> an array to receive the normalized components </param>
		''' <param name="normOffset"> the offset into the <code>normComponents</code>
		''' array at which to start storing normalized components </param>
		''' <returns> an array containing normalized color and alpha
		''' components. </returns>
		''' <exception cref="ClassCastException"> if <code>pixel</code> is not a primitive
		'''          array of type transferType </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''          <code>normComponents</code> is not large enough to hold all
		'''          color and alpha components starting at <code>normOffset</code> </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if
		'''          <code>pixel</code> is not large enough to hold a pixel
		'''          value for this <code>ColorModel</code>.
		''' @since 1.4 </exception>
		Public Overrides Function getNormalizedComponents(  pixel As Object,   normComponents As Single(),   normOffset As Integer) As Single()
			If normComponents Is Nothing Then normComponents = New Single(numComponents+normOffset - 1){}
			Select Case transferType
			Case DataBuffer.TYPE_BYTE
				Dim bpixel As SByte() = CType(pixel, SByte())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = (CSng(bpixel(c) And &Hff)) / (CSng((1 << nBits(c)) - 1))
					c += 1
					nc += 1
				Loop
			Case DataBuffer.TYPE_USHORT
				Dim uspixel As Short() = CType(pixel, Short())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = (CSng(uspixel(c) And &Hffff)) / (CSng((1 << nBits(c)) - 1))
					c += 1
					nc += 1
				Loop
			Case DataBuffer.TYPE_INT
				Dim ipixel As Integer() = CType(pixel, Integer())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = (CSng(ipixel(c))) / (CSng((1 << nBits(c)) - 1))
					c += 1
					nc += 1
				Loop
			Case DataBuffer.TYPE_SHORT
				Dim spixel As Short() = CType(pixel, Short())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = (CSng(spixel(c))) / 32767.0f
					c += 1
					nc += 1
				Loop
			Case DataBuffer.TYPE_FLOAT
				Dim fpixel As Single() = CType(pixel, Single())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = fpixel(c)
					c += 1
					nc += 1
				Loop
			Case DataBuffer.TYPE_DOUBLE
				Dim dpixel As Double() = CType(pixel, Double())
				Dim c As Integer = 0
				Dim nc As Integer = normOffset
				Do While c < numComponents
					normComponents(nc) = CSng(dpixel(c))
					c += 1
					nc += 1
				Loop
			Case Else
				Throw New UnsupportedOperationException("This method has not been " & "implemented for transferType " & transferType)
			End Select

			If supportsAlpha AndAlso isAlphaPremultiplied_Renamed Then
				Dim alpha_Renamed As Single = normComponents(numColorComponents + normOffset)
				If alpha_Renamed <> 0.0f Then
					Dim invAlpha As Single = 1.0f / alpha_Renamed
					For c As Integer = normOffset To numColorComponents + normOffset - 1
						normComponents(c) *= invAlpha
					Next c
				End If
			End If
			If min IsNot Nothing Then
				' Normally (i.e. when this class is not subclassed to override
				' this method), the test (min != null) will be equivalent to
				' the test (nonStdScale).  However, there is an unlikely, but
				' possible case, in which this method is overridden, nonStdScale
				' is set true by initScale(), the subclass method for some
				' reason calls this superclass method, but the min and
				' diffMinMax arrays were never initialized by setupLUTs().  In
				' that case, the right thing to do is follow the intended
				' semantics of this method, and rescale the color components
				' only if the ColorSpace min/max were detected to be other
				' than 0.0/1.0 by setupLUTs().  Note that this implies the
				' transferType is byte, ushort, int, or short - i.e. components
				' derived from float and double pixel data are never rescaled.
				For c As Integer = 0 To numColorComponents - 1
					normComponents(c + normOffset) = min(c) + diffMinMax(c) * normComponents(c + normOffset)
				Next c
			End If
			Return normComponents
		End Function

		''' <summary>
		''' Forces the raster data to match the state specified in the
		''' <CODE>isAlphaPremultiplied</CODE> variable, assuming the data
		''' is currently correctly described by this <CODE>ColorModel</CODE>.
		''' It may multiply or divide the color raster data by alpha, or
		''' do nothing if the data is in the correct state.  If the data needs
		''' to be coerced, this method also returns an instance of
		''' this <CODE>ColorModel</CODE> with
		''' the <CODE>isAlphaPremultiplied</CODE> flag set appropriately.
		''' Since <code>ColorModel</code> can be subclassed, subclasses inherit
		''' the implementation of this method and if they don't override it
		''' then they throw an exception if they use an unsupported
		''' <code>transferType</code>.
		''' </summary>
		''' <exception cref="NullPointerException"> if <code>raster</code> is
		''' <code>null</code> and data coercion is required. </exception>
		''' <exception cref="UnsupportedOperationException"> if the transfer type of
		''' this <CODE>ComponentColorModel</CODE>
		''' is not one of the supported transfer types:
		''' <CODE>DataBuffer.TYPE_BYTE</CODE>, <CODE>DataBuffer.TYPE_USHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_INT</CODE>, <CODE>DataBuffer.TYPE_SHORT</CODE>,
		''' <CODE>DataBuffer.TYPE_FLOAT</CODE>, or <CODE>DataBuffer.TYPE_DOUBLE</CODE>. </exception>
		Public Overrides Function coerceData(  raster_Renamed As WritableRaster,   isAlphaPremultiplied As Boolean) As ColorModel
			If (supportsAlpha = False) OrElse (Me.isAlphaPremultiplied_Renamed = isAlphaPremultiplied) Then Return Me

			Dim w As Integer = raster_Renamed.width
			Dim h As Integer = raster_Renamed.height
			Dim aIdx As Integer = raster_Renamed.numBands - 1
			Dim normAlpha As Single
			Dim rminX As Integer = raster_Renamed.minX
			Dim rY As Integer = raster_Renamed.minY
			Dim rX As Integer
			If isAlphaPremultiplied Then
				Select Case transferType
					Case DataBuffer.TYPE_BYTE
						Dim pixel As SByte() = Nothing
						Dim zpixel As SByte() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), SByte())
								normAlpha = (pixel(aIdx) And &Hff) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CByte((pixel(c) And &Hff) * normAlpha + 0.5f)
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New SByte(numComponents - 1){}
										Arrays.fill(zpixel, CByte(0))
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_USHORT
						Dim pixel As Short() = Nothing
						Dim zpixel As Short() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Short())
								normAlpha = (pixel(aIdx) And &Hffff) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CShort(Fix((pixel(c) And &Hffff) * normAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Short(numComponents - 1){}
										Arrays.fill(zpixel, CShort(0))
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_INT
						Dim pixel As Integer() = Nothing
						Dim zpixel As Integer() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Integer())
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * normAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Integer(numComponents - 1){}
										Arrays.fill(zpixel, 0)
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_SHORT
						Dim pixel As Short() = Nothing
						Dim zpixel As Short() = Nothing
						Dim alphaScale As Single = 1.0f / 32767.0f
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Short())
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CShort(Fix(pixel(c) * normAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Short(numComponents - 1){}
										Arrays.fill(zpixel, CShort(0))
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_FLOAT
						Dim pixel As Single() = Nothing
						Dim zpixel As Single() = Nothing
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Single())
								normAlpha = pixel(aIdx)
								If normAlpha <> 0.0f Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) *= normAlpha
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Single(numComponents - 1){}
										Arrays.fill(zpixel, 0.0f)
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_DOUBLE
						Dim pixel As Double() = Nothing
						Dim zpixel As Double() = Nothing
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Double())
								Dim dnormAlpha As Double = pixel(aIdx)
								If dnormAlpha <> 0.0 Then
									For c As Integer = 0 To aIdx - 1
										pixel(c) *= dnormAlpha
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								Else
									If zpixel Is Nothing Then
										zpixel = New Double(numComponents - 1){}
										Arrays.fill(zpixel, 0.0)
									End If
									raster_Renamed.dataElementsnts(rX, rY, zpixel)
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
						Dim pixel As SByte() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), SByte())
								normAlpha = (pixel(aIdx) And &Hff) * alphaScale
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CByte((pixel(c) And &Hff) * invAlpha + 0.5f)
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_USHORT
						Dim pixel As Short() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Short())
								normAlpha = (pixel(aIdx) And &Hffff) * alphaScale
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CShort(Fix((pixel(c) And &Hffff) * invAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_INT
						Dim pixel As Integer() = Nothing
						Dim alphaScale As Single = 1.0f / (CSng((1<<nBits(aIdx)) - 1))
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Integer())
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CInt(Fix(pixel(c) * invAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_SHORT
						Dim pixel As Short() = Nothing
						Dim alphaScale As Single = 1.0f / 32767.0f
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Short())
								normAlpha = pixel(aIdx) * alphaScale
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) = CShort(Fix(pixel(c) * invAlpha + 0.5f))
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_FLOAT
						Dim pixel As Single() = Nothing
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Single())
								normAlpha = pixel(aIdx)
								If normAlpha <> 0.0f Then
									Dim invAlpha As Single = 1.0f / normAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) *= invAlpha
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
								End If
								x += 1
								rX += 1
							Loop
							y += 1
							rY += 1
						Loop
					Case DataBuffer.TYPE_DOUBLE
						Dim pixel As Double() = Nothing
						Dim y As Integer = 0
						Do While y < h
							rX = rminX
							Dim x As Integer = 0
							Do While x < w
								pixel = CType(raster_Renamed.getDataElements(rX, rY, pixel), Double())
								Dim dnormAlpha As Double = pixel(aIdx)
								If dnormAlpha <> 0.0 Then
									Dim invAlpha As Double = 1.0 / dnormAlpha
									For c As Integer = 0 To aIdx - 1
										pixel(c) *= invAlpha
									Next c
									raster_Renamed.dataElementsnts(rX, rY, pixel)
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
			If Not signed Then
				Return New ComponentColorModel(colorSpace, nBits, supportsAlpha, isAlphaPremultiplied, transparency, transferType)
			Else
				Return New ComponentColorModel(colorSpace, supportsAlpha, isAlphaPremultiplied, transparency, transferType)
			End If

		End Function

		''' <summary>
		''' Returns true if <CODE>raster</CODE> is compatible with this
		''' <CODE>ColorModel</CODE>; false if it is not.
		''' </summary>
		''' <param name="raster"> The <CODE>Raster</CODE> object to test for compatibility.
		''' </param>
		''' <returns> <CODE>true</CODE> if <CODE>raster</CODE> is compatible with this
		''' <CODE>ColorModel</CODE>, <CODE>false</CODE> if it is not. </returns>
		Public Overrides Function isCompatibleRaster(  raster_Renamed As Raster) As Boolean

			Dim sm As SampleModel = raster_Renamed.sampleModel

			If TypeOf sm Is ComponentSampleModel Then
				If sm.numBands <> numComponents Then Return False
				For i As Integer = 0 To nBits.Length - 1
					If sm.getSampleSize(i) < nBits(i) Then Return False
				Next i
				Return (raster_Renamed.transferType = transferType)
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Creates a <CODE>WritableRaster</CODE> with the specified width and height,
		''' that  has a data layout (<CODE>SampleModel</CODE>) compatible with
		''' this <CODE>ColorModel</CODE>.
		''' </summary>
		''' <param name="w"> The width of the <CODE>WritableRaster</CODE> you want to create. </param>
		''' <param name="h"> The height of the <CODE>WritableRaster</CODE> you want to create.
		''' </param>
		''' <returns> A <CODE>WritableRaster</CODE> that is compatible with
		''' this <CODE>ColorModel</CODE>. </returns>
		''' <seealso cref= WritableRaster </seealso>
		''' <seealso cref= SampleModel </seealso>
		Public Overrides Function createCompatibleWritableRaster(  w As Integer,   h As Integer) As WritableRaster
			Dim dataSize As Integer = w*h*numComponents
			Dim raster_Renamed As WritableRaster = Nothing

			Select Case transferType
			Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT
				raster_Renamed = Raster.createInterleavedRaster(transferType, w, h, numComponents, Nothing)
			Case Else
				Dim sm As SampleModel = createCompatibleSampleModel(w, h)
				Dim db As DataBuffer = sm.createDataBuffer()
				raster_Renamed = Raster.createWritableRaster(sm, db, Nothing)
			End Select

			Return raster_Renamed
		End Function

		''' <summary>
		''' Creates a <CODE>SampleModel</CODE> with the specified width and height,
		''' that  has a data layout compatible with this <CODE>ColorModel</CODE>.
		''' </summary>
		''' <param name="w"> The width of the <CODE>SampleModel</CODE> you want to create. </param>
		''' <param name="h"> The height of the <CODE>SampleModel</CODE> you want to create.
		''' </param>
		''' <returns> A <CODE>SampleModel</CODE> that is compatible with this
		''' <CODE>ColorModel</CODE>.
		''' </returns>
		''' <seealso cref= SampleModel </seealso>
		Public Overrides Function createCompatibleSampleModel(  w As Integer,   h As Integer) As SampleModel
			Dim bandOffsets As Integer() = New Integer(numComponents - 1){}
			For i As Integer = 0 To numComponents - 1
				bandOffsets(i) = i
			Next i
			Select Case transferType
			Case DataBuffer.TYPE_BYTE, DataBuffer.TYPE_USHORT
				Return New PixelInterleavedSampleModel(transferType, w, h, numComponents, w*numComponents, bandOffsets)
			Case Else
				Return New ComponentSampleModel(transferType, w, h, numComponents, w*numComponents, bandOffsets)
			End Select
		End Function

		''' <summary>
		''' Checks whether or not the specified <CODE>SampleModel</CODE>
		''' is compatible with this <CODE>ColorModel</CODE>.
		''' </summary>
		''' <param name="sm"> The <CODE>SampleModel</CODE> to test for compatibility.
		''' </param>
		''' <returns> <CODE>true</CODE> if the <CODE>SampleModel</CODE> is
		''' compatible with this <CODE>ColorModel</CODE>, <CODE>false</CODE>
		''' if it is not.
		''' </returns>
		''' <seealso cref= SampleModel </seealso>
		Public Overrides Function isCompatibleSampleModel(  sm As SampleModel) As Boolean
			If Not(TypeOf sm Is ComponentSampleModel) Then Return False

			' Must have the same number of components
			If numComponents <> sm.numBands Then Return False

			If sm.transferType <> transferType Then Return False

			Return True
		End Function

		''' <summary>
		''' Returns a <CODE>Raster</CODE> representing the alpha channel of an image,
		''' extracted from the input <CODE>Raster</CODE>.
		''' This method assumes that <CODE>Raster</CODE> objects associated with
		''' this <CODE>ColorModel</CODE> store the alpha band, if present, as
		''' the last band of image data. Returns null if there is no separate spatial
		''' alpha channel associated with this <CODE>ColorModel</CODE>.
		''' This method creates a new <CODE>Raster</CODE>, but will share the data
		''' array.
		''' </summary>
		''' <param name="raster"> The <CODE>WritableRaster</CODE> from which to extract the
		''' alpha  channel.
		''' </param>
		''' <returns> A <CODE>WritableRaster</CODE> containing the image's alpha channel.
		'''  </returns>
		Public Overrides Function getAlphaRaster(  raster_Renamed As WritableRaster) As WritableRaster
			If hasAlpha() = False Then Return Nothing

			Dim x As Integer = raster_Renamed.minX
			Dim y As Integer = raster_Renamed.minY
			Dim band As Integer() = New Integer(0){}
			band(0) = raster_Renamed.numBands - 1
			Return raster_Renamed.createWritableChild(x, y, raster_Renamed.width, raster_Renamed.height, x, y, band)
		End Function

		''' <summary>
		''' Compares this color model with another for equality.
		''' </summary>
		''' <param name="obj"> The object to compare with this color model. </param>
		''' <returns> <CODE>true</CODE> if the color model objects are equal,
		''' <CODE>false</CODE> if they are not. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Not MyBase.Equals(obj) Then Return False

			If obj.GetType() IsNot Me.GetType() Then Return False

			Return True
		End Function

	End Class

End Namespace