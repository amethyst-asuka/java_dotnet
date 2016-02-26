Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' InnerShadowEffect - This effect currently only works with ARGB type buffered
	''' images.
	''' 
	''' @author Created by Jasper Potts (Jun 18, 2007)
	''' </summary>
	Friend Class InnerShadowEffect
		Inherits ShadowEffect

		' =================================================================================================================
		' Effect Methods

		''' <summary>
		''' Get the type of this effect, one of UNDER,BLENDED,OVER. UNDER means the result of apply effect should be painted
		''' under the src image. BLENDED means the result of apply sffect contains a modified src image so just it should be
		''' painted. OVER means the result of apply effect should be painted over the src image.
		''' </summary>
		''' <returns> The effect type </returns>
		Friend Property Overrides effectType As Effect.EffectType
			Get
				Return Effect.EffectType.OVER
			End Get
		End Property

		''' <summary>
		''' Apply the effect to the src image generating the result . The result image may or may not contain the source
		''' image depending on what the effect type is.
		''' </summary>
		''' <param name="src"> The source image for applying the effect to </param>
		''' <param name="dst"> The dstination image to paint effect result into. If this is null then a new image will be created </param>
		''' <param name="w">   The width of the src image to apply effect to, this allow the src and dst buffers to be bigger than
		'''            the area the need effect applied to it </param>
		''' <param name="h">   The height of the src image to apply effect to, this allow the src and dst buffers to be bigger than
		'''            the area the need effect applied to it </param>
		''' <returns> Image with the result of the effect </returns>
		Friend Overrides Function applyEffect(ByVal src As java.awt.image.BufferedImage, ByVal dst As java.awt.image.BufferedImage, ByVal w As Integer, ByVal h As Integer) As java.awt.image.BufferedImage
			If src Is Nothing OrElse src.type <> java.awt.image.BufferedImage.TYPE_INT_ARGB Then Throw New System.ArgumentException("Effect only works with " & "source images of type BufferedImage.TYPE_INT_ARGB.")
			If dst IsNot Nothing AndAlso dst.type <> java.awt.image.BufferedImage.TYPE_INT_ARGB Then Throw New System.ArgumentException("Effect only works with " & "destination images of type BufferedImage.TYPE_INT_ARGB.")
			' calculate offset
			Dim trangleAngle As Double = Math.toRadians(angle - 90)
			Dim offsetX As Integer = CInt(Fix(Math.Sin(trangleAngle) * distance))
			Dim offsetY As Integer = CInt(Fix(Math.Cos(trangleAngle) * distance))
			' clac expanded size
			Dim tmpOffX As Integer = offsetX + size
			Dim tmpOffY As Integer = offsetX + size
			Dim tmpW As Integer = w + offsetX + size + size
			Dim tmpH As Integer = h + offsetX + size
			' create tmp buffers
			Dim lineBuf As Integer() = arrayCache.getTmpIntArray(w)
			Dim srcAlphaBuf As SByte() = arrayCache.getTmpByteArray1(tmpW * tmpH)
			java.util.Arrays.fill(srcAlphaBuf, CByte(&HFF))
			Dim tmpBuf1 As SByte() = arrayCache.getTmpByteArray2(tmpW * tmpH)
			Dim tmpBuf2 As SByte() = arrayCache.getTmpByteArray3(tmpW * tmpH)
			' extract src image alpha channel and inverse and offset
			Dim srcRaster As java.awt.image.Raster = src.raster
			For y As Integer = 0 To h - 1
				Dim dy As Integer = (y + tmpOffY)
				Dim offset As Integer = dy * tmpW
				srcRaster.getDataElements(0, y, w, 1, lineBuf)
				For x As Integer = 0 To w - 1
					Dim dx As Integer = x + tmpOffX
					srcAlphaBuf(offset + dx) = CByte((255 - (CInt(CUInt((lineBuf(x) And &HFF000000L)) >> 24))) And &HFF)
				Next x
			Next y
			' blur
			Dim kernel As Single() = EffectUtils.createGaussianKernel(size * 2)
			EffectUtils.blur(srcAlphaBuf, tmpBuf2, tmpW, tmpH, kernel, size * 2) ' horizontal pass
			EffectUtils.blur(tmpBuf2, tmpBuf1, tmpH, tmpW, kernel, size * 2) ' vertical pass
			'rescale
			Dim ___spread As Single = Math.Min(1 / (1 - (0.01f * Me.spread)), 255)
			For i As Integer = 0 To tmpBuf1.Length - 1
				Dim val As Integer = CInt(Fix((CInt(tmpBuf1(i)) And &HFF) * ___spread))
				tmpBuf1(i) = If(val > 255, CByte(&HFF), CByte(val))
			Next i
			' create color image with shadow color and greyscale image as alpha
			If dst Is Nothing Then dst = New java.awt.image.BufferedImage(w, h, java.awt.image.BufferedImage.TYPE_INT_ARGB)
			Dim shadowRaster As java.awt.image.WritableRaster = dst.raster
			Dim red As Integer = color.red, green As Integer = color.green, blue As Integer = color.blue
			For y As Integer = 0 To h - 1
				Dim srcY As Integer = y + tmpOffY
				Dim offset As Integer = srcY * tmpW
				Dim shadowOffset As Integer = (srcY - offsetY) * tmpW
				For x As Integer = 0 To w - 1
					Dim srcX As Integer = x + tmpOffX
					Dim origianlAlphaVal As Integer = 255 - (CInt(srcAlphaBuf(offset + srcX)) And &HFF)
					Dim shadowVal As Integer = CInt(tmpBuf1(shadowOffset + (srcX - offsetX))) And &HFF
					Dim alphaVal As Integer = Math.Min(origianlAlphaVal, shadowVal)
					lineBuf(x) = (CByte(alphaVal) And &HFF) << 24 Or red << 16 Or green << 8 Or blue
				Next x
				shadowRaster.dataElementsnts(0, y, w, 1, lineBuf)
			Next y
			Return dst
		End Function
	End Class

End Namespace