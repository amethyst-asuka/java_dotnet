Imports System
Imports System.Collections

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
	''' A class that allows the format of an image (in particular, its
	''' <code>SampleModel</code> and <code>ColorModel</code>) to be
	''' specified in a convenient manner.
	''' 
	''' </summary>
	Public Class ImageTypeSpecifier

		''' <summary>
		''' The <code>ColorModel</code> to be used as a prototype.
		''' </summary>
		Protected Friend colorModel As java.awt.image.ColorModel

		''' <summary>
		''' A <code>SampleModel</code> to be used as a prototype.
		''' </summary>
		Protected Friend sampleModel As java.awt.image.SampleModel

		''' <summary>
		''' Cached specifiers for all of the standard
		''' <code>BufferedImage</code> types.
		''' </summary>
		Private Shared BISpecifier As ImageTypeSpecifier()
		Private Shared sRGB As java.awt.color.ColorSpace
		' Initialize the standard specifiers
		Shared Sub New()
			sRGB = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB)

			BISpecifier = New ImageTypeSpecifier(java.awt.image.BufferedImage.TYPE_BYTE_INDEXED){}
		End Sub

		''' <summary>
		''' A constructor to be used by inner subclasses only.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ImageTypeSpecifier</code> directly
		''' from a <code>ColorModel</code> and a <code>SampleModel</code>.
		''' It is the caller's responsibility to supply compatible
		''' parameters.
		''' </summary>
		''' <param name="colorModel"> a <code>ColorModel</code>. </param>
		''' <param name="sampleModel"> a <code>SampleModel</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if either parameter is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>sampleModel</code>
		''' is not compatible with <code>colorModel</code>. </exception>
		Public Sub New(ByVal colorModel As java.awt.image.ColorModel, ByVal sampleModel As java.awt.image.SampleModel)
			If colorModel Is Nothing Then Throw New System.ArgumentException("colorModel == null!")
			If sampleModel Is Nothing Then Throw New System.ArgumentException("sampleModel == null!")
			If Not colorModel.isCompatibleSampleModel(sampleModel) Then Throw New System.ArgumentException("sampleModel is incompatible with colorModel!")
			Me.colorModel = colorModel
			Me.sampleModel = sampleModel
		End Sub

		''' <summary>
		''' Constructs an <code>ImageTypeSpecifier</code> from a
		''' <code>RenderedImage</code>.  If a <code>BufferedImage</code> is
		''' being used, one of the factory methods
		''' <code>createFromRenderedImage</code> or
		''' <code>createFromBufferedImageType</code> should be used instead in
		''' order to get a more accurate result.
		''' </summary>
		''' <param name="image"> a <code>RenderedImage</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the argument is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal image As java.awt.image.RenderedImage)
			If image Is Nothing Then Throw New System.ArgumentException("image == null!")
			colorModel = image.colorModel
			sampleModel = image.sampleModel
		End Sub

		' Packed

		Friend Class Packed
			Inherits ImageTypeSpecifier

			Friend colorSpace As java.awt.color.ColorSpace
			Friend redMask As Integer
			Friend greenMask As Integer
			Friend blueMask As Integer
			Friend alphaMask As Integer
			Friend transferType As Integer
			Friend isAlphaPremultiplied As Boolean

			Public Sub New(ByVal colorSpace As java.awt.color.ColorSpace, ByVal redMask As Integer, ByVal greenMask As Integer, ByVal blueMask As Integer, ByVal alphaMask As Integer, ByVal transferType As Integer, ByVal isAlphaPremultiplied As Boolean) ' 0 if no alpha
				If colorSpace Is Nothing Then Throw New System.ArgumentException("colorSpace == null!")
				If colorSpace.type <> java.awt.color.ColorSpace.TYPE_RGB Then Throw New System.ArgumentException("colorSpace is not of type TYPE_RGB!")
				If transferType <> java.awt.image.DataBuffer.TYPE_BYTE AndAlso transferType <> java.awt.image.DataBuffer.TYPE_USHORT AndAlso transferType <> java.awt.image.DataBuffer.TYPE_INT Then Throw New System.ArgumentException("Bad value for transferType!")
				If redMask = 0 AndAlso greenMask = 0 AndAlso blueMask = 0 AndAlso alphaMask = 0 Then Throw New System.ArgumentException("No mask has at least 1 bit set!")
				Me.colorSpace = colorSpace
				Me.redMask = redMask
				Me.greenMask = greenMask
				Me.blueMask = blueMask
				Me.alphaMask = alphaMask
				Me.transferType = transferType
				Me.isAlphaPremultiplied = isAlphaPremultiplied

				Dim bits As Integer = 32
				Me.colorModel = New java.awt.image.DirectColorModel(colorSpace, bits, redMask, greenMask, blueMask, alphaMask, isAlphaPremultiplied, transferType)
				Me.sampleModel = colorModel.createCompatibleSampleModel(1, 1)
			End Sub
		End Class

		''' <summary>
		''' Returns a specifier for a packed image format that will use a
		''' <code>DirectColorModel</code> and a packed
		''' <code>SampleModel</code> to store each pixel packed into in a
		''' single byte, short, or int.
		''' </summary>
		''' <param name="colorSpace"> the desired <code>ColorSpace</code>. </param>
		''' <param name="redMask"> a contiguous mask indicated the position of the
		''' red channel. </param>
		''' <param name="greenMask"> a contiguous mask indicated the position of the
		''' green channel. </param>
		''' <param name="blueMask"> a contiguous mask indicated the position of the
		''' blue channel. </param>
		''' <param name="alphaMask"> a contiguous mask indicated the position of the
		''' alpha channel. </param>
		''' <param name="transferType"> the desired <code>SampleModel</code> transfer type. </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the color channels
		''' will be premultipled by the alpha channel.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>colorSpace</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>colorSpace</code>
		''' is not of type <code>TYPE_RGB</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if no mask has at least 1
		''' bit set. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>transferType</code> if not one of
		''' <code>DataBuffer.TYPE_BYTE</code>,
		''' <code>DataBuffer.TYPE_USHORT</code>, or
		''' <code>DataBuffer.TYPE_INT</code>. </exception>
		Public Shared Function createPacked(ByVal colorSpace As java.awt.color.ColorSpace, ByVal redMask As Integer, ByVal greenMask As Integer, ByVal blueMask As Integer, ByVal alphaMask As Integer, ByVal transferType As Integer, ByVal isAlphaPremultiplied As Boolean) As ImageTypeSpecifier ' 0 if no alpha
			Return New ImageTypeSpecifier.Packed(colorSpace, redMask, greenMask, blueMask, alphaMask, transferType, isAlphaPremultiplied) ' 0 if no alpha
		End Function

		Friend Shared Function createComponentCM(ByVal colorSpace As java.awt.color.ColorSpace, ByVal numBands As Integer, ByVal dataType As Integer, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean) As java.awt.image.ColorModel
			Dim transparency As Integer = If(hasAlpha, java.awt.Transparency.TRANSLUCENT, java.awt.Transparency.OPAQUE)

			Dim numBits As Integer() = New Integer(numBands - 1){}
			Dim bits As Integer = java.awt.image.DataBuffer.getDataTypeSize(dataType)

			For i As Integer = 0 To numBands - 1
				numBits(i) = bits
			Next i

			Return New java.awt.image.ComponentColorModel(colorSpace, numBits, hasAlpha, isAlphaPremultiplied, transparency, dataType)
		End Function

		' Interleaved

		Friend Class Interleaved
			Inherits ImageTypeSpecifier

			Friend colorSpace As java.awt.color.ColorSpace
			Friend bandOffsets As Integer()
			Friend dataType As Integer
			Friend hasAlpha As Boolean
			Friend isAlphaPremultiplied As Boolean

			Public Sub New(ByVal colorSpace As java.awt.color.ColorSpace, ByVal bandOffsets As Integer(), ByVal dataType As Integer, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean)
				If colorSpace Is Nothing Then Throw New System.ArgumentException("colorSpace == null!")
				If bandOffsets Is Nothing Then Throw New System.ArgumentException("bandOffsets == null!")
				Dim ___numBands As Integer = colorSpace.numComponents + (If(hasAlpha, 1, 0))
				If bandOffsets.Length <> ___numBands Then Throw New System.ArgumentException("bandOffsets.length is wrong!")
				If dataType <> java.awt.image.DataBuffer.TYPE_BYTE AndAlso dataType <> java.awt.image.DataBuffer.TYPE_SHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_USHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_INT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_FLOAT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_DOUBLE Then Throw New System.ArgumentException("Bad value for dataType!")
				Me.colorSpace = colorSpace
				Me.bandOffsets = CType(bandOffsets.clone(), Integer())
				Me.dataType = dataType
				Me.hasAlpha = hasAlpha
				Me.isAlphaPremultiplied = isAlphaPremultiplied

				Me.colorModel = ImageTypeSpecifier.createComponentCM(colorSpace, bandOffsets.Length, dataType, hasAlpha, isAlphaPremultiplied)

				Dim minBandOffset As Integer = bandOffsets(0)
				Dim maxBandOffset As Integer = minBandOffset
				For i As Integer = 0 To bandOffsets.Length - 1
					Dim offset As Integer = bandOffsets(i)
					minBandOffset = Math.Min(offset, minBandOffset)
					maxBandOffset = Math.Max(offset, maxBandOffset)
				Next i
				Dim pixelStride As Integer = maxBandOffset - minBandOffset + 1

				Dim w As Integer = 1
				Dim h As Integer = 1
				Me.sampleModel = New java.awt.image.PixelInterleavedSampleModel(dataType, w, h, pixelStride, w*pixelStride, bandOffsets)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If (o Is Nothing) OrElse Not(TypeOf o Is ImageTypeSpecifier.Interleaved) Then Return False

				Dim that As ImageTypeSpecifier.Interleaved = CType(o, ImageTypeSpecifier.Interleaved)

				If (Not(Me.colorSpace.Equals(that.colorSpace))) OrElse (Me.dataType <> that.dataType) OrElse (Me.hasAlpha <> that.hasAlpha) OrElse (Me.isAlphaPremultiplied <> that.isAlphaPremultiplied) OrElse (Me.bandOffsets.Length <> that.bandOffsets.Length) Then Return False

				For i As Integer = 0 To bandOffsets.Length - 1
					If Me.bandOffsets(i) <> that.bandOffsets(i) Then Return False
				Next i

				Return True
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return (MyBase.GetHashCode() + (4 * bandOffsets.Length) + (25 * dataType) + (If(hasAlpha, 17, 18)))
			End Function
		End Class

		''' <summary>
		''' Returns a specifier for an interleaved image format that will
		''' use a <code>ComponentColorModel</code> and a
		''' <code>PixelInterleavedSampleModel</code> to store each pixel
		''' component in a separate byte, short, or int.
		''' </summary>
		''' <param name="colorSpace"> the desired <code>ColorSpace</code>. </param>
		''' <param name="bandOffsets"> an array of <code>int</code>s indicating the
		''' offsets for each band. </param>
		''' <param name="dataType"> the desired data type, as one of the enumerations
		''' from the <code>DataBuffer</code> class. </param>
		''' <param name="hasAlpha"> <code>true</code> if an alpha channel is desired. </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the color channels
		''' will be premultipled by the alpha channel.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>colorSpace</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bandOffsets</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the legal <code>DataBuffer.TYPE_*</code> constants. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>bandOffsets.length</code> does not equal the number of
		''' color space components, plus 1 if <code>hasAlpha</code> is
		''' <code>true</code>. </exception>
		Public Shared Function createInterleaved(ByVal colorSpace As java.awt.color.ColorSpace, ByVal bandOffsets As Integer(), ByVal dataType As Integer, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean) As ImageTypeSpecifier
			Return New ImageTypeSpecifier.Interleaved(colorSpace, bandOffsets, dataType, hasAlpha, isAlphaPremultiplied)
		End Function

		' Banded

		Friend Class Banded
			Inherits ImageTypeSpecifier

			Friend colorSpace As java.awt.color.ColorSpace
			Friend bankIndices As Integer()
			Friend bandOffsets As Integer()
			Friend dataType As Integer
			Friend hasAlpha As Boolean
			Friend isAlphaPremultiplied As Boolean

			Public Sub New(ByVal colorSpace As java.awt.color.ColorSpace, ByVal bankIndices As Integer(), ByVal bandOffsets As Integer(), ByVal dataType As Integer, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean)
				If colorSpace Is Nothing Then Throw New System.ArgumentException("colorSpace == null!")
				If bankIndices Is Nothing Then Throw New System.ArgumentException("bankIndices == null!")
				If bandOffsets Is Nothing Then Throw New System.ArgumentException("bandOffsets == null!")
				If bankIndices.Length <> bandOffsets.Length Then Throw New System.ArgumentException("bankIndices.length != bandOffsets.length!")
				If dataType <> java.awt.image.DataBuffer.TYPE_BYTE AndAlso dataType <> java.awt.image.DataBuffer.TYPE_SHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_USHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_INT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_FLOAT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_DOUBLE Then Throw New System.ArgumentException("Bad value for dataType!")
				Dim ___numBands As Integer = colorSpace.numComponents + (If(hasAlpha, 1, 0))
				If bandOffsets.Length <> ___numBands Then Throw New System.ArgumentException("bandOffsets.length is wrong!")

				Me.colorSpace = colorSpace
				Me.bankIndices = CType(bankIndices.clone(), Integer())
				Me.bandOffsets = CType(bandOffsets.clone(), Integer())
				Me.dataType = dataType
				Me.hasAlpha = hasAlpha
				Me.isAlphaPremultiplied = isAlphaPremultiplied

				Me.colorModel = ImageTypeSpecifier.createComponentCM(colorSpace, bankIndices.Length, dataType, hasAlpha, isAlphaPremultiplied)

				Dim w As Integer = 1
				Dim h As Integer = 1
				Me.sampleModel = New java.awt.image.BandedSampleModel(dataType, w, h, w, bankIndices, bandOffsets)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If (o Is Nothing) OrElse Not(TypeOf o Is ImageTypeSpecifier.Banded) Then Return False

				Dim that As ImageTypeSpecifier.Banded = CType(o, ImageTypeSpecifier.Banded)

				If (Not(Me.colorSpace.Equals(that.colorSpace))) OrElse (Me.dataType <> that.dataType) OrElse (Me.hasAlpha <> that.hasAlpha) OrElse (Me.isAlphaPremultiplied <> that.isAlphaPremultiplied) OrElse (Me.bankIndices.Length <> that.bankIndices.Length) OrElse (Me.bandOffsets.Length <> that.bandOffsets.Length) Then Return False

				For i As Integer = 0 To bankIndices.Length - 1
					If Me.bankIndices(i) <> that.bankIndices(i) Then Return False
				Next i

				For i As Integer = 0 To bandOffsets.Length - 1
					If Me.bandOffsets(i) <> that.bandOffsets(i) Then Return False
				Next i

				Return True
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return (MyBase.GetHashCode() + (3 * bandOffsets.Length) + (7 * bankIndices.Length) + (21 * dataType) + (If(hasAlpha, 19, 29)))
			End Function
		End Class

		''' <summary>
		''' Returns a specifier for a banded image format that will use a
		''' <code>ComponentColorModel</code> and a
		''' <code>BandedSampleModel</code> to store each channel in a
		''' separate array.
		''' </summary>
		''' <param name="colorSpace"> the desired <code>ColorSpace</code>. </param>
		''' <param name="bankIndices"> an array of <code>int</code>s indicating the
		''' bank in which each band will be stored. </param>
		''' <param name="bandOffsets"> an array of <code>int</code>s indicating the
		''' starting offset of each band within its bank. </param>
		''' <param name="dataType"> the desired data type, as one of the enumerations
		''' from the <code>DataBuffer</code> class. </param>
		''' <param name="hasAlpha"> <code>true</code> if an alpha channel is desired. </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the color channels
		''' will be premultipled by the alpha channel.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>colorSpace</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bankIndices</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bandOffsets</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the lengths of
		''' <code>bankIndices</code> and <code>bandOffsets</code> differ. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>bandOffsets.length</code> does not equal the number of
		''' color space components, plus 1 if <code>hasAlpha</code> is
		''' <code>true</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of the legal <code>DataBuffer.TYPE_*</code> constants. </exception>
		Public Shared Function createBanded(ByVal colorSpace As java.awt.color.ColorSpace, ByVal bankIndices As Integer(), ByVal bandOffsets As Integer(), ByVal dataType As Integer, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean) As ImageTypeSpecifier
			Return New ImageTypeSpecifier.Banded(colorSpace, bankIndices, bandOffsets, dataType, hasAlpha, isAlphaPremultiplied)
		End Function

		' Grayscale

		Friend Class Grayscale
			Inherits ImageTypeSpecifier

			Friend bits As Integer
			Friend dataType As Integer
			Friend isSigned As Boolean
			Friend hasAlpha As Boolean
			Friend isAlphaPremultiplied As Boolean

			Public Sub New(ByVal bits As Integer, ByVal dataType As Integer, ByVal isSigned As Boolean, ByVal hasAlpha As Boolean, ByVal isAlphaPremultiplied As Boolean)
				If bits <> 1 AndAlso bits <> 2 AndAlso bits <> 4 AndAlso bits <> 8 AndAlso bits <> 16 Then Throw New System.ArgumentException("Bad value for bits!")
				If dataType <> java.awt.image.DataBuffer.TYPE_BYTE AndAlso dataType <> java.awt.image.DataBuffer.TYPE_SHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_USHORT Then Throw New System.ArgumentException("Bad value for dataType!")
				If bits > 8 AndAlso dataType = java.awt.image.DataBuffer.TYPE_BYTE Then Throw New System.ArgumentException("Too many bits for dataType!")

				Me.bits = bits
				Me.dataType = dataType
				Me.isSigned = isSigned
				Me.hasAlpha = hasAlpha
				Me.isAlphaPremultiplied = isAlphaPremultiplied

				Dim colorSpace As java.awt.color.ColorSpace = java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_GRAY)

				If (bits = 8 AndAlso dataType = java.awt.image.DataBuffer.TYPE_BYTE) OrElse (bits = 16 AndAlso (dataType = java.awt.image.DataBuffer.TYPE_SHORT OrElse dataType = java.awt.image.DataBuffer.TYPE_USHORT)) Then
					' Use component color model & sample model

					Dim ___numBands As Integer = If(hasAlpha, 2, 1)
					Dim transparency As Integer = If(hasAlpha, java.awt.Transparency.TRANSLUCENT, java.awt.Transparency.OPAQUE)


					Dim nBits As Integer() = New Integer(___numBands - 1){}
					nBits(0) = bits
					If ___numBands = 2 Then nBits(1) = bits
					Me.colorModel = New java.awt.image.ComponentColorModel(colorSpace, nBits, hasAlpha, isAlphaPremultiplied, transparency, dataType)

					Dim bandOffsets As Integer() = New Integer(___numBands - 1){}
					bandOffsets(0) = 0
					If ___numBands = 2 Then bandOffsets(1) = 1

					Dim w As Integer = 1
					Dim h As Integer = 1
					Me.sampleModel = New java.awt.image.PixelInterleavedSampleModel(dataType, w, h, ___numBands, w*___numBands, bandOffsets)
				Else
					Dim numEntries As Integer = 1 << bits
					Dim arr As SByte() = New SByte(numEntries - 1){}
					For i As Integer = 0 To numEntries - 1
						arr(i) = CByte(i*255\(numEntries - 1))
					Next i
					Me.colorModel = New java.awt.image.IndexColorModel(bits, numEntries, arr, arr, arr)

					Me.sampleModel = New java.awt.image.MultiPixelPackedSampleModel(dataType, 1, 1, bits)
				End If
			End Sub
		End Class

		''' <summary>
		''' Returns a specifier for a grayscale image format that will pack
		''' pixels of the given bit depth into array elements of
		''' the specified data type.
		''' </summary>
		''' <param name="bits"> the number of bits per gray value (1, 2, 4, 8, or 16). </param>
		''' <param name="dataType"> the desired data type, as one of the enumerations
		''' from the <code>DataBuffer</code> class. </param>
		''' <param name="isSigned"> <code>true</code> if negative values are to
		''' be represented.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' not one of 1, 2, 4, 8, or 16. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of <code>DataBuffer.TYPE_BYTE</code>,
		''' <code>DataBuffer.TYPE_SHORT</code>, or
		''' <code>DataBuffer.TYPE_USHORT</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' larger than the bit size of the given <code>dataType</code>. </exception>
		Public Shared Function createGrayscale(ByVal bits As Integer, ByVal dataType As Integer, ByVal isSigned As Boolean) As ImageTypeSpecifier
			Return New ImageTypeSpecifier.Grayscale(bits, dataType, isSigned, False, False)
		End Function

		''' <summary>
		''' Returns a specifier for a grayscale plus alpha image format
		''' that will pack pixels of the given bit depth into array
		''' elements of the specified data type.
		''' </summary>
		''' <param name="bits"> the number of bits per gray value (1, 2, 4, 8, or 16). </param>
		''' <param name="dataType"> the desired data type, as one of the enumerations
		''' from the <code>DataBuffer</code> class. </param>
		''' <param name="isSigned"> <code>true</code> if negative values are to
		''' be represented. </param>
		''' <param name="isAlphaPremultiplied"> <code>true</code> if the luminance channel
		''' will be premultipled by the alpha channel.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' not one of 1, 2, 4, 8, or 16. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of <code>DataBuffer.TYPE_BYTE</code>,
		''' <code>DataBuffer.TYPE_SHORT</code>, or
		''' <code>DataBuffer.TYPE_USHORT</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' larger than the bit size of the given <code>dataType</code>. </exception>
		Public Shared Function createGrayscale(ByVal bits As Integer, ByVal dataType As Integer, ByVal isSigned As Boolean, ByVal isAlphaPremultiplied As Boolean) As ImageTypeSpecifier
			Return New ImageTypeSpecifier.Grayscale(bits, dataType, isSigned, True, isAlphaPremultiplied)
		End Function

		' Indexed

		Friend Class Indexed
			Inherits ImageTypeSpecifier

			Friend redLUT As SByte()
			Friend greenLUT As SByte()
			Friend blueLUT As SByte()
			Friend alphaLUT As SByte() = Nothing
			Friend bits As Integer
			Friend dataType As Integer

			Public Sub New(ByVal redLUT As SByte(), ByVal greenLUT As SByte(), ByVal blueLUT As SByte(), ByVal alphaLUT As SByte(), ByVal bits As Integer, ByVal dataType As Integer)
				If redLUT Is Nothing OrElse greenLUT Is Nothing OrElse blueLUT Is Nothing Then Throw New System.ArgumentException("LUT is null!")
				If bits <> 1 AndAlso bits <> 2 AndAlso bits <> 4 AndAlso bits <> 8 AndAlso bits <> 16 Then Throw New System.ArgumentException("Bad value for bits!")
				If dataType <> java.awt.image.DataBuffer.TYPE_BYTE AndAlso dataType <> java.awt.image.DataBuffer.TYPE_SHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_USHORT AndAlso dataType <> java.awt.image.DataBuffer.TYPE_INT Then Throw New System.ArgumentException("Bad value for dataType!")
				If (bits > 8 AndAlso dataType = java.awt.image.DataBuffer.TYPE_BYTE) OrElse (bits > 16 AndAlso dataType <> java.awt.image.DataBuffer.TYPE_INT) Then Throw New System.ArgumentException("Too many bits for dataType!")

				Dim len As Integer = 1 << bits
				If redLUT.Length <> len OrElse greenLUT.Length <> len OrElse blueLUT.Length <> len OrElse (alphaLUT IsNot Nothing AndAlso alphaLUT.Length <> len) Then Throw New System.ArgumentException("LUT has improper length!")
				Me.redLUT = CType(redLUT.clone(), SByte())
				Me.greenLUT = CType(greenLUT.clone(), SByte())
				Me.blueLUT = CType(blueLUT.clone(), SByte())
				If alphaLUT IsNot Nothing Then Me.alphaLUT = CType(alphaLUT.clone(), SByte())
				Me.bits = bits
				Me.dataType = dataType

				If alphaLUT Is Nothing Then
					Me.colorModel = New java.awt.image.IndexColorModel(bits, redLUT.Length, redLUT, greenLUT, blueLUT)
				Else
					Me.colorModel = New java.awt.image.IndexColorModel(bits, redLUT.Length, redLUT, greenLUT, blueLUT, alphaLUT)
				End If

				If (bits = 8 AndAlso dataType = java.awt.image.DataBuffer.TYPE_BYTE) OrElse (bits = 16 AndAlso (dataType = java.awt.image.DataBuffer.TYPE_SHORT OrElse dataType = java.awt.image.DataBuffer.TYPE_USHORT)) Then
					Dim bandOffsets As Integer() = { 0 }
					Me.sampleModel = New java.awt.image.PixelInterleavedSampleModel(dataType, 1, 1, 1, 1, bandOffsets)
				Else
					Me.sampleModel = New java.awt.image.MultiPixelPackedSampleModel(dataType, 1, 1, bits)
				End If
			End Sub
		End Class

		''' <summary>
		''' Returns a specifier for an indexed-color image format that will pack
		''' index values of the given bit depth into array elements of
		''' the specified data type.
		''' </summary>
		''' <param name="redLUT"> an array of <code>byte</code>s containing
		''' the red values for each index. </param>
		''' <param name="greenLUT"> an array of <code>byte</code>s containing * the
		'''  green values for each index. </param>
		''' <param name="blueLUT"> an array of <code>byte</code>s containing the
		''' blue values for each index. </param>
		''' <param name="alphaLUT"> an array of <code>byte</code>s containing the
		''' alpha values for each index, or <code>null</code> to create a
		''' fully opaque LUT. </param>
		''' <param name="bits"> the number of bits in each index. </param>
		''' <param name="dataType"> the desired output type, as one of the enumerations
		''' from the <code>DataBuffer</code> class.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>redLUT</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>greenLUT</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>blueLUT</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' not one of 1, 2, 4, 8, or 16. </exception>
		''' <exception cref="IllegalArgumentException"> if the
		''' non-<code>null</code> LUT parameters do not have lengths of
		''' exactly {@code 1 << bits}. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dataType</code> is
		''' not one of <code>DataBuffer.TYPE_BYTE</code>,
		''' <code>DataBuffer.TYPE_SHORT</code>,
		''' <code>DataBuffer.TYPE_USHORT</code>,
		''' or <code>DataBuffer.TYPE_INT</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>bits</code> is
		''' larger than the bit size of the given <code>dataType</code>. </exception>
		Public Shared Function createIndexed(ByVal redLUT As SByte(), ByVal greenLUT As SByte(), ByVal blueLUT As SByte(), ByVal alphaLUT As SByte(), ByVal bits As Integer, ByVal dataType As Integer) As ImageTypeSpecifier
			Return New ImageTypeSpecifier.Indexed(redLUT, greenLUT, blueLUT, alphaLUT, bits, dataType)
		End Function

		''' <summary>
		''' Returns an <code>ImageTypeSpecifier</code> that encodes
		''' one of the standard <code>BufferedImage</code> types
		''' (other than <code>TYPE_CUSTOM</code>).
		''' </summary>
		''' <param name="bufferedImageType"> an int representing one of the standard
		''' <code>BufferedImage</code> types.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>bufferedImageType</code> is not one of the standard
		''' types, or is equal to <code>TYPE_CUSTOM</code>.
		''' </exception>
		''' <seealso cref= java.awt.image.BufferedImage </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_ARGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_ARGB_PRE </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_BGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_3BYTE_BGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_4BYTE_ABGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_4BYTE_ABGR_PRE </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_565_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_555_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_GRAY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_GRAY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_BINARY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_INDEXED </seealso>
		Public Shared Function createFromBufferedImageType(ByVal bufferedImageType As Integer) As ImageTypeSpecifier
			If bufferedImageType >= java.awt.image.BufferedImage.TYPE_INT_RGB AndAlso bufferedImageType <= java.awt.image.BufferedImage.TYPE_BYTE_INDEXED Then
				Return getSpecifier(bufferedImageType)
			ElseIf bufferedImageType = java.awt.image.BufferedImage.TYPE_CUSTOM Then
				Throw New System.ArgumentException("Cannot create from TYPE_CUSTOM!")
			Else
				Throw New System.ArgumentException("Invalid BufferedImage type!")
			End If
		End Function

		''' <summary>
		''' Returns an <code>ImageTypeSpecifier</code> that encodes the
		''' layout of a <code>RenderedImage</code> (which may be a
		''' <code>BufferedImage</code>).
		''' </summary>
		''' <param name="image"> a <code>RenderedImage</code>.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code> with the desired
		''' characteristics.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>image</code> is
		''' <code>null</code>. </exception>
		Public Shared Function createFromRenderedImage(ByVal image As java.awt.image.RenderedImage) As ImageTypeSpecifier
			If image Is Nothing Then Throw New System.ArgumentException("image == null!")

			If TypeOf image Is java.awt.image.BufferedImage Then
				Dim ___bufferedImageType As Integer = CType(image, java.awt.image.BufferedImage).type
				If ___bufferedImageType <> java.awt.image.BufferedImage.TYPE_CUSTOM Then Return getSpecifier(___bufferedImageType)
			End If

			Return New ImageTypeSpecifier(image)
		End Function

		''' <summary>
		''' Returns an int containing one of the enumerated constant values
		''' describing image formats from <code>BufferedImage</code>.
		''' </summary>
		''' <returns> an <code>int</code> representing a
		''' <code>BufferedImage</code> type.
		''' </returns>
		''' <seealso cref= java.awt.image.BufferedImage </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_CUSTOM </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_ARGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_ARGB_PRE </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_INT_BGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_3BYTE_BGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_4BYTE_ABGR </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_4BYTE_ABGR_PRE </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_565_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_555_RGB </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_GRAY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_USHORT_GRAY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_BINARY </seealso>
		''' <seealso cref= java.awt.image.BufferedImage#TYPE_BYTE_INDEXED </seealso>
		Public Overridable Property bufferedImageType As Integer
			Get
				Dim bi As java.awt.image.BufferedImage = createBufferedImage(1, 1)
				Return bi.type
			End Get
		End Property

		''' <summary>
		''' Return the number of color components
		''' specified by this object.  This is the same value as returned by
		''' <code>ColorModel.getNumComponents</code>
		''' </summary>
		''' <returns> the number of components in the image. </returns>
		Public Overridable Property numComponents As Integer
			Get
				Return colorModel.numComponents
			End Get
		End Property

		''' <summary>
		''' Return the number of bands
		''' specified by this object.  This is the same value as returned by
		''' <code>SampleModel.getNumBands</code>
		''' </summary>
		''' <returns> the number of bands in the image. </returns>
		Public Overridable Property numBands As Integer
			Get
				Return sampleModel.numBands
			End Get
		End Property

		''' <summary>
		''' Return the number of bits used to represent samples of the given band.
		''' </summary>
		''' <param name="band"> the index of the band to be queried, as an
		''' int.
		''' </param>
		''' <returns> an int specifying a number of bits.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>band</code> is
		''' negative or greater than the largest band index. </exception>
		Public Overridable Function getBitsPerBand(ByVal band As Integer) As Integer
			If band < 0 Or band >= numBands Then Throw New System.ArgumentException("band out of range!")
			Return sampleModel.getSampleSize(band)
		End Function

		''' <summary>
		''' Returns a <code>SampleModel</code> based on the settings
		''' encapsulated within this object.  The width and height of the
		''' <code>SampleModel</code> will be set to arbitrary values.
		''' </summary>
		''' <returns> a <code>SampleModel</code> with arbitrary dimensions. </returns>
		Public Overridable Property sampleModel As java.awt.image.SampleModel
			Get
				Return sampleModel
			End Get
		End Property

		''' <summary>
		''' Returns a <code>SampleModel</code> based on the settings
		''' encapsulated within this object.  The width and height of the
		''' <code>SampleModel</code> will be set to the supplied values.
		''' </summary>
		''' <param name="width"> the desired width of the returned <code>SampleModel</code>. </param>
		''' <param name="height"> the desired height of the returned
		''' <code>SampleModel</code>.
		''' </param>
		''' <returns> a <code>SampleModel</code> with the given dimensions.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either <code>width</code> or
		''' <code>height</code> are negative or zero. </exception>
		''' <exception cref="IllegalArgumentException"> if the product of
		''' <code>width</code> and <code>height</code> is greater than
		''' <code>Integer.MAX_VALUE</code> </exception>
		Public Overridable Function getSampleModel(ByVal width As Integer, ByVal height As Integer) As java.awt.image.SampleModel
			If CLng(width)*height > Integer.MaxValue Then Throw New System.ArgumentException("width*height > Integer.MAX_VALUE!")
			Return sampleModel.createCompatibleSampleModel(width, height)
		End Function

		''' <summary>
		''' Returns the <code>ColorModel</code> specified by this object.
		''' </summary>
		''' <returns> a <code>ColorModel</code>. </returns>
		Public Overridable Property colorModel As java.awt.image.ColorModel
			Get
				Return colorModel
			End Get
		End Property

		''' <summary>
		''' Creates a <code>BufferedImage</code> with a given width and
		''' height according to the specification embodied in this object.
		''' </summary>
		''' <param name="width"> the desired width of the returned
		''' <code>BufferedImage</code>. </param>
		''' <param name="height"> the desired height of the returned
		''' <code>BufferedImage</code>.
		''' </param>
		''' <returns> a new <code>BufferedImage</code>
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if either <code>width</code> or
		''' <code>height</code> are negative or zero. </exception>
		''' <exception cref="IllegalArgumentException"> if the product of
		''' <code>width</code> and <code>height</code> is greater than
		''' <code>Integer.MAX_VALUE</code>, or if the number of array
		''' elements needed to store the image is greater than
		''' <code>Integer.MAX_VALUE</code>. </exception>
		Public Overridable Function createBufferedImage(ByVal width As Integer, ByVal height As Integer) As java.awt.image.BufferedImage
			Try
				Dim ___sampleModel As java.awt.image.SampleModel = getSampleModel(width, height)
				Dim raster As java.awt.image.WritableRaster = java.awt.image.Raster.createWritableRaster(___sampleModel, New java.awt.Point(0, 0))
				Return New java.awt.image.BufferedImage(colorModel, raster, colorModel.alphaPremultiplied, New Hashtable)
			Catch e As NegativeArraySizeException
				' Exception most likely thrown from a DataBuffer constructor
				Throw New System.ArgumentException("Array size > Integer.MAX_VALUE!")
			End Try
		End Function

		''' <summary>
		''' Returns <code>true</code> if the given <code>Object</code> is
		''' an <code>ImageTypeSpecifier</code> and has a
		''' <code>SampleModel</code> and <code>ColorModel</code> that are
		''' equal to those of this object.
		''' </summary>
		''' <param name="o"> the <code>Object</code> to be compared for equality.
		''' </param>
		''' <returns> <code>true</code> if the given object is an equivalent
		''' <code>ImageTypeSpecifier</code>. </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If (o Is Nothing) OrElse Not(TypeOf o Is ImageTypeSpecifier) Then Return False

			Dim that As ImageTypeSpecifier = CType(o, ImageTypeSpecifier)
			Return (colorModel.Equals(that.colorModel)) AndAlso (sampleModel.Equals(that.sampleModel))
		End Function

		''' <summary>
		''' Returns the hash code for this ImageTypeSpecifier.
		''' </summary>
		''' <returns> a hash code for this ImageTypeSpecifier </returns>
		Public Overrides Function GetHashCode() As Integer
			Return (9 * colorModel.GetHashCode()) + (14 * sampleModel.GetHashCode())
		End Function

		Private Shared Function getSpecifier(ByVal type As Integer) As ImageTypeSpecifier
			If BISpecifier(type) Is Nothing Then BISpecifier(type) = createSpecifier(type)
			Return BISpecifier(type)
		End Function

		Private Shared Function createSpecifier(ByVal type As Integer) As ImageTypeSpecifier
			Select Case type
			  Case java.awt.image.BufferedImage.TYPE_INT_RGB
				  Return createPacked(sRGB, &Hff0000, &Hff00, &Hff, &H0, java.awt.image.DataBuffer.TYPE_INT, False)

			  Case java.awt.image.BufferedImage.TYPE_INT_ARGB
				  Return createPacked(sRGB, &Hff0000, &Hff00, &Hff, &Hff000000L, java.awt.image.DataBuffer.TYPE_INT, False)

			  Case java.awt.image.BufferedImage.TYPE_INT_ARGB_PRE
				  Return createPacked(sRGB, &Hff0000, &Hff00, &Hff, &Hff000000L, java.awt.image.DataBuffer.TYPE_INT, True)

			  Case java.awt.image.BufferedImage.TYPE_INT_BGR
				  Return createPacked(sRGB, &Hff, &Hff00, &Hff0000, &H0, java.awt.image.DataBuffer.TYPE_INT, False)

			  Case java.awt.image.BufferedImage.TYPE_3BYTE_BGR
				  Return createInterleaved(sRGB, New Integer() { 2, 1, 0 }, java.awt.image.DataBuffer.TYPE_BYTE, False, False)

			  Case java.awt.image.BufferedImage.TYPE_4BYTE_ABGR
				  Return createInterleaved(sRGB, New Integer() { 3, 2, 1, 0 }, java.awt.image.DataBuffer.TYPE_BYTE, True, False)

			  Case java.awt.image.BufferedImage.TYPE_4BYTE_ABGR_PRE
				  Return createInterleaved(sRGB, New Integer() { 3, 2, 1, 0 }, java.awt.image.DataBuffer.TYPE_BYTE, True, True)

			  Case java.awt.image.BufferedImage.TYPE_USHORT_565_RGB
				  Return createPacked(sRGB, &HF800, &H7E0, &H1F, &H0, java.awt.image.DataBuffer.TYPE_USHORT, False)

			  Case java.awt.image.BufferedImage.TYPE_USHORT_555_RGB
				  Return createPacked(sRGB, &H7C00, &H3E0, &H1F, &H0, java.awt.image.DataBuffer.TYPE_USHORT, False)

			  Case java.awt.image.BufferedImage.TYPE_BYTE_GRAY
				Return createGrayscale(8, java.awt.image.DataBuffer.TYPE_BYTE, False)

			  Case java.awt.image.BufferedImage.TYPE_USHORT_GRAY
				Return createGrayscale(16, java.awt.image.DataBuffer.TYPE_USHORT, False)

			  Case java.awt.image.BufferedImage.TYPE_BYTE_BINARY
				  Return createGrayscale(1, java.awt.image.DataBuffer.TYPE_BYTE, False)

			  Case java.awt.image.BufferedImage.TYPE_BYTE_INDEXED

				  Dim bi As New java.awt.image.BufferedImage(1, 1, java.awt.image.BufferedImage.TYPE_BYTE_INDEXED)
				  Dim icm As java.awt.image.IndexColorModel = CType(bi.colorModel, java.awt.image.IndexColorModel)
				  Dim mapSize As Integer = icm.mapSize
				  Dim redLUT As SByte() = New SByte(mapSize - 1){}
				  Dim greenLUT As SByte() = New SByte(mapSize - 1){}
				  Dim blueLUT As SByte() = New SByte(mapSize - 1){}
				  Dim alphaLUT As SByte() = New SByte(mapSize - 1){}

				  icm.getReds(redLUT)
				  icm.getGreens(greenLUT)
				  icm.getBlues(blueLUT)
				  icm.getAlphas(alphaLUT)

				  Return createIndexed(redLUT, greenLUT, blueLUT, alphaLUT, 8, java.awt.image.DataBuffer.TYPE_BYTE)
			  Case Else
				  Throw New System.ArgumentException("Invalid BufferedImage type!")
			End Select
		End Function

	End Class

End Namespace