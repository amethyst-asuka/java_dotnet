Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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

Namespace java.awt


	Friend MustInherit Class TexturePaintContext
		Implements PaintContext

		Public Shared xrgbmodel As java.awt.image.ColorModel = New java.awt.image.DirectColorModel(24, &Hff0000, &Hff00, &Hff)
		Public Shared argbmodel As java.awt.image.ColorModel = java.awt.image.ColorModel.rGBdefault

		Friend colorModel As java.awt.image.ColorModel
		Friend bWidth As Integer
		Friend bHeight As Integer
		Friend maxWidth As Integer

		Friend outRas As java.awt.image.WritableRaster

		Friend xOrg As Double
		Friend yOrg As Double
		Friend incXAcross As Double
		Friend incYAcross As Double
		Friend incXDown As Double
		Friend incYDown As Double

		Friend colincx As Integer
		Friend colincy As Integer
		Friend colincxerr As Integer
		Friend colincyerr As Integer
		Friend rowincx As Integer
		Friend rowincy As Integer
		Friend rowincxerr As Integer
		Friend rowincyerr As Integer

		Public Shared Function getContext(ByVal bufImg As java.awt.image.BufferedImage, ByVal xform As java.awt.geom.AffineTransform, ByVal hints As RenderingHints, ByVal devBounds As Rectangle) As PaintContext
			Dim raster_Renamed As java.awt.image.WritableRaster = bufImg.raster
			Dim cm As java.awt.image.ColorModel = bufImg.colorModel
			Dim maxw As Integer = devBounds.width
			Dim val As Object = hints.get(RenderingHints.KEY_INTERPOLATION)
			Dim filter As Boolean = (If(val Is Nothing, (hints.get(RenderingHints.KEY_RENDERING) Is RenderingHints.VALUE_RENDER_QUALITY), (val IsNot RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR)))
			If TypeOf raster_Renamed Is sun.awt.image.IntegerInterleavedRaster AndAlso ((Not filter) OrElse isFilterableDCM(cm)) Then
				Dim iir As sun.awt.image.IntegerInterleavedRaster = CType(raster_Renamed, sun.awt.image.IntegerInterleavedRaster)
				If iir.numDataElements = 1 AndAlso iir.pixelStride = 1 Then Return New Int(iir, cm, xform, maxw, filter)
			ElseIf TypeOf raster_Renamed Is sun.awt.image.ByteInterleavedRaster Then
				Dim bir As sun.awt.image.ByteInterleavedRaster = CType(raster_Renamed, sun.awt.image.ByteInterleavedRaster)
				If bir.numDataElements = 1 AndAlso bir.pixelStride = 1 Then
					If filter Then
						If isFilterableICM(cm) Then Return New ByteFilter(bir, cm, xform, maxw)
					Else
						Return New Byte(bir, cm, xform, maxw)
					End If
				End If
			End If
			Return New Any(raster_Renamed, cm, xform, maxw, filter)
		End Function

		Public Shared Function isFilterableICM(ByVal cm As java.awt.image.ColorModel) As Boolean
			If TypeOf cm Is java.awt.image.IndexColorModel Then
				Dim icm As java.awt.image.IndexColorModel = CType(cm, java.awt.image.IndexColorModel)
				If icm.mapSize <= 256 Then Return True
			End If
			Return False
		End Function

		Public Shared Function isFilterableDCM(ByVal cm As java.awt.image.ColorModel) As Boolean
			If TypeOf cm Is java.awt.image.DirectColorModel Then
				Dim dcm As java.awt.image.DirectColorModel = CType(cm, java.awt.image.DirectColorModel)
				Return (isMaskOK(dcm.alphaMask, True) AndAlso isMaskOK(dcm.redMask, False) AndAlso isMaskOK(dcm.greenMask, False) AndAlso isMaskOK(dcm.blueMask, False))
			End If
			Return False
		End Function

		Public Shared Function isMaskOK(ByVal mask As Integer, ByVal canbezero As Boolean) As Boolean
			If canbezero AndAlso mask = 0 Then Return True
			Return (mask = &Hff OrElse mask = &Hff00 OrElse mask = &Hff0000 OrElse mask = &Hff000000L)
		End Function

		Public Shared Function getInternedColorModel(ByVal cm As java.awt.image.ColorModel) As java.awt.image.ColorModel
			If xrgbmodel Is cm OrElse xrgbmodel.Equals(cm) Then Return xrgbmodel
			If argbmodel Is cm OrElse argbmodel.Equals(cm) Then Return argbmodel
			Return cm
		End Function

		Friend Sub New(ByVal cm As java.awt.image.ColorModel, ByVal xform As java.awt.geom.AffineTransform, ByVal bWidth As Integer, ByVal bHeight As Integer, ByVal maxw As Integer)
			Me.colorModel = getInternedColorModel(cm)
			Me.bWidth = bWidth
			Me.bHeight = bHeight
			Me.maxWidth = maxw

			Try
				xform = xform.createInverse()
			Catch e As java.awt.geom.NoninvertibleTransformException
				xform.toScaleale(0, 0)
			End Try
			Me.incXAcross = [mod](xform.scaleX, bWidth)
			Me.incYAcross = [mod](xform.shearY, bHeight)
			Me.incXDown = [mod](xform.shearX, bWidth)
			Me.incYDown = [mod](xform.scaleY, bHeight)
			Me.xOrg = xform.translateX
			Me.yOrg = xform.translateY
			Me.colincx = CInt(Fix(incXAcross))
			Me.colincy = CInt(Fix(incYAcross))
			Me.colincxerr = fractAsInt(incXAcross)
			Me.colincyerr = fractAsInt(incYAcross)
			Me.rowincx = CInt(Fix(incXDown))
			Me.rowincy = CInt(Fix(incYDown))
			Me.rowincxerr = fractAsInt(incXDown)
			Me.rowincyerr = fractAsInt(incYDown)

		End Sub

		Friend Shared Function fractAsInt(ByVal d As Double) As Integer
			Return CInt(Fix((d Mod 1.0) *  java.lang.[Integer].Max_Value))
		End Function

		Friend Shared Function [mod](ByVal num As Double, ByVal den As Double) As Double
			num = num Mod den
			If num < 0 Then
				num += den
				If num >= den Then num = 0
			End If
			Return num
		End Function

		''' <summary>
		''' Release the resources allocated for the operation.
		''' </summary>
		Public Overridable Sub dispose() Implements PaintContext.dispose
			dropRaster(colorModel, outRas)
		End Sub

		''' <summary>
		''' Return the ColorModel of the output.
		''' </summary>
		Public Overridable Property colorModel As java.awt.image.ColorModel Implements PaintContext.getColorModel
			Get
				Return colorModel
			End Get
		End Property

		''' <summary>
		''' Return a Raster containing the colors generated for the graphics
		''' operation. </summary>
		''' <param name="x">,y,w,h The area in device space for which colors are
		''' generated. </param>
		Public Overridable Function getRaster(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As java.awt.image.Raster Implements PaintContext.getRaster
			If outRas Is Nothing OrElse outRas.width < w OrElse outRas.height < h Then outRas = makeRaster((If(h = 1, System.Math.Max(w, maxWidth), w)), h)
			Dim X_Renamed As Double = [mod](xOrg + x * incXAcross + y * incXDown, bWidth)
			Dim Y_Renamed As Double = [mod](yOrg + x * incYAcross + y * incYDown, bHeight)

			rasterter(CInt(Fix(X_Renamed)), CInt(Fix(Y_Renamed)), fractAsInt(X_Renamed), fractAsInt(Y_Renamed), w, h, bWidth, bHeight, colincx, colincxerr, colincy, colincyerr, rowincx, rowincxerr, rowincy, rowincyerr)

			sun.awt.image.SunWritableRaster.markDirty(outRas)

			Return outRas
		End Function

		Private Shared xrgbRasRef As WeakReference(Of java.awt.image.Raster)
		Private Shared argbRasRef As WeakReference(Of java.awt.image.Raster)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function makeRaster(ByVal cm As java.awt.image.ColorModel, ByVal srcRas As java.awt.image.Raster, ByVal w As Integer, ByVal h As Integer) As java.awt.image.WritableRaster
			If xrgbmodel Is cm Then
				If xrgbRasRef IsNot Nothing Then
					Dim wr As java.awt.image.WritableRaster = CType(xrgbRasRef.get(), java.awt.image.WritableRaster)
					If wr IsNot Nothing AndAlso wr.width >= w AndAlso wr.height >= h Then
						xrgbRasRef = Nothing
						Return wr
					End If
				End If
				' If we are going to cache this Raster, make it non-tiny
				If w <= 32 AndAlso h <= 32 Then
						h = 32
						w = h
				End If
			ElseIf argbmodel Is cm Then
				If argbRasRef IsNot Nothing Then
					Dim wr As java.awt.image.WritableRaster = CType(argbRasRef.get(), java.awt.image.WritableRaster)
					If wr IsNot Nothing AndAlso wr.width >= w AndAlso wr.height >= h Then
						argbRasRef = Nothing
						Return wr
					End If
				End If
				' If we are going to cache this Raster, make it non-tiny
				If w <= 32 AndAlso h <= 32 Then
						h = 32
						w = h
				End If
			End If
			If srcRas IsNot Nothing Then
				Return srcRas.createCompatibleWritableRaster(w, h)
			Else
				Return cm.createCompatibleWritableRaster(w, h)
			End If
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub dropRaster(ByVal cm As java.awt.image.ColorModel, ByVal outRas As java.awt.image.Raster)
			If outRas Is Nothing Then Return
			If xrgbmodel Is cm Then
				xrgbRasRef = New WeakReference(Of )(outRas)
			ElseIf argbmodel Is cm Then
				argbRasRef = New WeakReference(Of )(outRas)
			End If
		End Sub

		Private Shared byteRasRef As WeakReference(Of java.awt.image.Raster)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function makeByteRaster(ByVal srcRas As java.awt.image.Raster, ByVal w As Integer, ByVal h As Integer) As java.awt.image.WritableRaster
			If byteRasRef IsNot Nothing Then
				Dim wr As java.awt.image.WritableRaster = CType(byteRasRef.get(), java.awt.image.WritableRaster)
				If wr IsNot Nothing AndAlso wr.width >= w AndAlso wr.height >= h Then
					byteRasRef = Nothing
					Return wr
				End If
			End If
			' If we are going to cache this Raster, make it non-tiny
			If w <= 32 AndAlso h <= 32 Then
					h = 32
					w = h
			End If
			Return srcRas.createCompatibleWritableRaster(w, h)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub dropByteRaster(ByVal outRas As java.awt.image.Raster)
			If outRas Is Nothing Then Return
			byteRasRef = New WeakReference(Of )(outRas)
		End Sub

		Public MustOverride Function makeRaster(ByVal w As Integer, ByVal h As Integer) As java.awt.image.WritableRaster
		Public MustOverride Sub setRaster(ByVal x As Integer, ByVal y As Integer, ByVal xerr As Integer, ByVal yerr As Integer, ByVal w As Integer, ByVal h As Integer, ByVal bWidth As Integer, ByVal bHeight As Integer, ByVal colincx As Integer, ByVal colincxerr As Integer, ByVal colincy As Integer, ByVal colincyerr As Integer, ByVal rowincx As Integer, ByVal rowincxerr As Integer, ByVal rowincy As Integer, ByVal rowincyerr As Integer)

	'    
	'     * Blends the four ARGB values in the rgbs array using the factors
	'     * described by xmul and ymul in the following ratio:
	'     *
	'     *     rgbs[0] * (1-xmul) * (1-ymul) +
	'     *     rgbs[1] * (  xmul) * (1-ymul) +
	'     *     rgbs[2] * (1-xmul) * (  ymul) +
	'     *     rgbs[3] * (  xmul) * (  ymul)
	'     *
	'     * xmul and ymul are integer values in the half-open range [0, 2^31)
	'     * where 0 == 0.0 and 2^31 == 1.0.
	'     *
	'     * Note that since the range is half-open, the values are always
	'     * logically less than 1.0.  This makes sense because while choosing
	'     * pixels to blend, when the error values reach 1.0 we move to the
	'     * next pixel and reset them to 0.0.
	'     
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public static int blend(int rgbs() , int xmul, int ymul)
			' xmul/ymul are 31 bits wide, (0 => 2^31-1)
			' shift them to 12 bits wide, (0 => 2^12-1)
			xmul = (CInt(CUInt(xmul) >> 19))
			ymul = (CInt(CUInt(ymul) >> 19))
			Dim accumA, accumR, accumG, accumB As Integer
				accumB = 0
					accumG = accumB
						accumR = accumG
						accumA = accumR
			For i As Integer = 0 To 3
				Dim rgb As Integer = rgbs(i)
				' The complement of the [xy]mul values (1-[xy]mul) can result
				' in new values in the range (1 => 2^12).  Thus for any given
				' loop iteration, the values could be anywhere in (0 => 2^12).
				xmul = (1<<12) - xmul
				If (i And 1) = 0 Then ymul = (1<<12) - ymul
				' xmul and ymul are each 12 bits (0 => 2^12)
				' factor is thus 24 bits (0 => 2^24)
				Dim factor As Integer = xmul * ymul
				If factor <> 0 Then
					' accum variables will accumulate 32 bits
					' bytes extracted from rgb fit in 8 bits (0 => 255)
					' byte * factor thus fits in 32 bits (0 => 255 * 2^24)
					accumA += (((CInt(CUInt(rgb) >> 24))) * factor)
					accumR += (((CInt(CUInt(rgb) >> 16)) And &Hff) * factor)
					accumG += (((CInt(CUInt(rgb) >> 8)) And &Hff) * factor)
					accumB += (((rgb) And &Hff) * factor)
				End If
			Next i
			Return (((CInt(CUInt((accumA + (1<<23))) >> 24)) << 24) Or ((CInt(CUInt((accumR + (1<<23))) >> 24)) << 16) Or ((CInt(CUInt((accumG + (1<<23))) >> 24)) << 8) Or ((CInt(CUInt((accumB + (1<<23))) >> 24))))

		static class Int extends TexturePaintContext
			Dim srcRas As sun.awt.image.IntegerInterleavedRaster
			Dim inData As Integer()
			Dim inOff As Integer
			Dim inSpan As Integer
			Dim outData As Integer()
			Dim outOff As Integer
			Dim outSpan As Integer
			Dim filter As Boolean

			public Int(sun.awt.image.IntegerInterleavedRaster srcRas, java.awt.image.ColorModel cm, java.awt.geom.AffineTransform xform, Integer maxw, Boolean filter)
				MyBase(cm, xform, srcRas.width, srcRas.height, maxw)
				Me.srcRas = srcRas
				Me.inData = srcRas.dataStorage
				Me.inSpan = srcRas.scanlineStride
				Me.inOff = srcRas.getDataOffset(0)
				Me.filter = filter

			public java.awt.image.WritableRaster makeRaster(Integer w, Integer h)
				Dim ras As java.awt.image.WritableRaster = makeRaster(colorModel, srcRas, w, h)
				Dim iiRas As sun.awt.image.IntegerInterleavedRaster = CType(ras, sun.awt.image.IntegerInterleavedRaster)
				outData = iiRas.dataStorage
				outSpan = iiRas.scanlineStride
				outOff = iiRas.getDataOffset(0)
				Return ras

			public  Sub  rasterter(Integer x, Integer y, Integer xerr, Integer yerr, Integer w, Integer h, Integer bWidth, Integer bHeight, Integer colincx, Integer colincxerr, Integer colincy, Integer colincyerr, Integer rowincx, Integer rowincxerr, Integer rowincy, Integer rowincyerr)
				Dim inData As Integer() = Me.inData
				Dim outData As Integer() = Me.outData
				Dim out As Integer = outOff
				Dim inSpan As Integer = Me.inSpan
				Dim inOff As Integer = Me.inOff
				Dim outSpan As Integer = Me.outSpan
				Dim filter As Boolean = Me.filter
				Dim normalx As Boolean = (colincx = 1 AndAlso colincxerr = 0 AndAlso colincy = 0 AndAlso colincyerr = 0) AndAlso Not filter
				Dim rowx As Integer = x
				Dim rowy As Integer = y
				Dim rowxerr As Integer = xerr
				Dim rowyerr As Integer = yerr
				If normalx Then outSpan -= w
				Dim rgbs As Integer() = If(filter, New Integer(3){}, Nothing)
				For j As Integer = 0 To h - 1
					If normalx Then
						Dim [in] As Integer = inOff + rowy * inSpan + bWidth
						x = bWidth - rowx
						out += w
						If bWidth >= 32 Then
							Dim i As Integer = w
							Do While i > 0
								Dim copyw As Integer = If(i < x, i, x)
								Array.Copy(inData, [in] - x, outData, out - i, copyw)
								i -= copyw
								x -= copyw
								If x = 0 Then x = bWidth
							Loop
						Else
							For i As Integer = w To 1 Step -1
								outData(out - i) = inData([in] - x)
								x -= 1
								If x = 0 Then x = bWidth
							Next i
						End If
					Else
						x = rowx
						y = rowy
						xerr = rowxerr
						yerr = rowyerr
						For i As Integer = 0 To w - 1
							If filter Then
								Dim nextx, nexty As Integer
								nextx = x + 1
								If nextx >= bWidth Then nextx = 0
								nexty = y + 1
								If nexty >= bHeight Then nexty = 0
								rgbs(0) = inData(inOff + y * inSpan + x)
								rgbs(1) = inData(inOff + y * inSpan + nextx)
								rgbs(2) = inData(inOff + nexty * inSpan + x)
								rgbs(3) = inData(inOff + nexty * inSpan + nextx)
								outData(out + i) = TexturePaintContext.blend(rgbs, xerr, yerr)
							Else
								outData(out + i) = inData(inOff + y * inSpan + x)
							End If
							xerr += colincxerr
							If xerr < 0 Then
								xerr = xerr And  java.lang.[Integer].Max_Value
								x += 1
							End If
							x += colincx
							If x >= bWidth Then x -= bWidth
							yerr += colincyerr
							If yerr < 0 Then
								yerr = yerr And  java.lang.[Integer].Max_Value
								y += 1
							End If
							y += colincy
							If y >= bHeight Then y -= bHeight
						Next i
					End If
					rowxerr += rowincxerr
					If rowxerr < 0 Then
						rowxerr = rowxerr And  java.lang.[Integer].Max_Value
						rowx += 1
					End If
					rowx += rowincx
					If rowx >= bWidth Then rowx -= bWidth
					rowyerr += rowincyerr
					If rowyerr < 0 Then
						rowyerr = rowyerr And  java.lang.[Integer].Max_Value
						rowy += 1
					End If
					rowy += rowincy
					If rowy >= bHeight Then rowy -= bHeight
					out += outSpan
				Next j

		static class Byte extends TexturePaintContext
			Dim srcRas As sun.awt.image.ByteInterleavedRaster
			Dim inData As SByte()
			Dim inOff As Integer
			Dim inSpan As Integer
			Dim outData As SByte()
			Dim outOff As Integer
			Dim outSpan As Integer

			public Byte(sun.awt.image.ByteInterleavedRaster srcRas, java.awt.image.ColorModel cm, java.awt.geom.AffineTransform xform, Integer maxw)
				MyBase(cm, xform, srcRas.width, srcRas.height, maxw)
				Me.srcRas = srcRas
				Me.inData = srcRas.dataStorage
				Me.inSpan = srcRas.scanlineStride
				Me.inOff = srcRas.getDataOffset(0)

			public java.awt.image.WritableRaster makeRaster(Integer w, Integer h)
				Dim ras As java.awt.image.WritableRaster = makeByteRaster(srcRas, w, h)
				Dim biRas As sun.awt.image.ByteInterleavedRaster = CType(ras, sun.awt.image.ByteInterleavedRaster)
				outData = biRas.dataStorage
				outSpan = biRas.scanlineStride
				outOff = biRas.getDataOffset(0)
				Return ras

			public  Sub  Dispose()
				dropByteRaster(outRas)

			public  Sub  rasterter(Integer x, Integer y, Integer xerr, Integer yerr, Integer w, Integer h, Integer bWidth, Integer bHeight, Integer colincx, Integer colincxerr, Integer colincy, Integer colincyerr, Integer rowincx, Integer rowincxerr, Integer rowincy, Integer rowincyerr)
				Dim inData As SByte() = Me.inData
				Dim outData As SByte() = Me.outData
				Dim out As Integer = outOff
				Dim inSpan As Integer = Me.inSpan
				Dim inOff As Integer = Me.inOff
				Dim outSpan As Integer = Me.outSpan
				Dim normalx As Boolean = (colincx = 1 AndAlso colincxerr = 0 AndAlso colincy = 0 AndAlso colincyerr = 0)
				Dim rowx As Integer = x
				Dim rowy As Integer = y
				Dim rowxerr As Integer = xerr
				Dim rowyerr As Integer = yerr
				If normalx Then outSpan -= w
				For j As Integer = 0 To h - 1
					If normalx Then
						Dim [in] As Integer = inOff + rowy * inSpan + bWidth
						x = bWidth - rowx
						out += w
						If bWidth >= 32 Then
							Dim i As Integer = w
							Do While i > 0
								Dim copyw As Integer = If(i < x, i, x)
								Array.Copy(inData, [in] - x, outData, out - i, copyw)
								i -= copyw
								x -= copyw
								If x = 0 Then x = bWidth
							Loop
						Else
							For i As Integer = w To 1 Step -1
								outData(out - i) = inData([in] - x)
								x -= 1
								If x = 0 Then x = bWidth
							Next i
						End If
					Else
						x = rowx
						y = rowy
						xerr = rowxerr
						yerr = rowyerr
						For i As Integer = 0 To w - 1
							outData(out + i) = inData(inOff + y * inSpan + x)
							xerr += colincxerr
							If xerr < 0 Then
								xerr = xerr And  java.lang.[Integer].Max_Value
								x += 1
							End If
							x += colincx
							If x >= bWidth Then x -= bWidth
							yerr += colincyerr
							If yerr < 0 Then
								yerr = yerr And  java.lang.[Integer].Max_Value
								y += 1
							End If
							y += colincy
							If y >= bHeight Then y -= bHeight
						Next i
					End If
					rowxerr += rowincxerr
					If rowxerr < 0 Then
						rowxerr = rowxerr And  java.lang.[Integer].Max_Value
						rowx += 1
					End If
					rowx += rowincx
					If rowx >= bWidth Then rowx -= bWidth
					rowyerr += rowincyerr
					If rowyerr < 0 Then
						rowyerr = rowyerr And  java.lang.[Integer].Max_Value
						rowy += 1
					End If
					rowy += rowincy
					If rowy >= bHeight Then rowy -= bHeight
					out += outSpan
				Next j

		static class ByteFilter extends TexturePaintContext
			Dim srcRas As sun.awt.image.ByteInterleavedRaster
			Dim inPalette As Integer()
			Dim inData As SByte()
			Dim inOff As Integer
			Dim inSpan As Integer
			Dim outData As Integer()
			Dim outOff As Integer
			Dim outSpan As Integer

			public ByteFilter(sun.awt.image.ByteInterleavedRaster srcRas, java.awt.image.ColorModel cm, java.awt.geom.AffineTransform xform, Integer maxw)
				MyBase((If(cm.transparency = Transparency.OPAQUE, xrgbmodel, argbmodel)), xform, srcRas.width, srcRas.height, maxw)
				Me.inPalette = New Integer(255){}
				CType(cm, java.awt.image.IndexColorModel).getRGBs(Me.inPalette)
				Me.srcRas = srcRas
				Me.inData = srcRas.dataStorage
				Me.inSpan = srcRas.scanlineStride
				Me.inOff = srcRas.getDataOffset(0)

			public java.awt.image.WritableRaster makeRaster(Integer w, Integer h)
				' Note that we do not pass srcRas to makeRaster since it
				' is a Byte Raster and this colorModel needs an Int Raster
				Dim ras As java.awt.image.WritableRaster = makeRaster(colorModel, Nothing, w, h)
				Dim iiRas As sun.awt.image.IntegerInterleavedRaster = CType(ras, sun.awt.image.IntegerInterleavedRaster)
				outData = iiRas.dataStorage
				outSpan = iiRas.scanlineStride
				outOff = iiRas.getDataOffset(0)
				Return ras

			public  Sub  rasterter(Integer x, Integer y, Integer xerr, Integer yerr, Integer w, Integer h, Integer bWidth, Integer bHeight, Integer colincx, Integer colincxerr, Integer colincy, Integer colincyerr, Integer rowincx, Integer rowincxerr, Integer rowincy, Integer rowincyerr)
				Dim inData As SByte() = Me.inData
				Dim outData As Integer() = Me.outData
				Dim out As Integer = outOff
				Dim inSpan As Integer = Me.inSpan
				Dim inOff As Integer = Me.inOff
				Dim outSpan As Integer = Me.outSpan
				Dim rowx As Integer = x
				Dim rowy As Integer = y
				Dim rowxerr As Integer = xerr
				Dim rowyerr As Integer = yerr
				Dim rgbs As Integer() = New Integer(3){}
				For j As Integer = 0 To h - 1
					x = rowx
					y = rowy
					xerr = rowxerr
					yerr = rowyerr
					For i As Integer = 0 To w - 1
						Dim nextx, nexty As Integer
						nextx = x + 1
						If nextx >= bWidth Then nextx = 0
						nexty = y + 1
						If nexty >= bHeight Then nexty = 0
						rgbs(0) = inPalette(&Hff And inData(inOff + x + inSpan * y))
						rgbs(1) = inPalette(&Hff And inData(inOff + nextx + inSpan * y))
						rgbs(2) = inPalette(&Hff And inData(inOff + x + inSpan * nexty))
						rgbs(3) = inPalette(&Hff And inData(inOff + nextx + inSpan * nexty))
						outData(out + i) = TexturePaintContext.blend(rgbs, xerr, yerr)
						xerr += colincxerr
						If xerr < 0 Then
							xerr = xerr And  java.lang.[Integer].Max_Value
							x += 1
						End If
						x += colincx
						If x >= bWidth Then x -= bWidth
						yerr += colincyerr
						If yerr < 0 Then
							yerr = yerr And  java.lang.[Integer].Max_Value
							y += 1
						End If
						y += colincy
						If y >= bHeight Then y -= bHeight
					Next i
					rowxerr += rowincxerr
					If rowxerr < 0 Then
						rowxerr = rowxerr And  java.lang.[Integer].Max_Value
						rowx += 1
					End If
					rowx += rowincx
					If rowx >= bWidth Then rowx -= bWidth
					rowyerr += rowincyerr
					If rowyerr < 0 Then
						rowyerr = rowyerr And  java.lang.[Integer].Max_Value
						rowy += 1
					End If
					rowy += rowincy
					If rowy >= bHeight Then rowy -= bHeight
					out += outSpan
				Next j

		static class Any extends TexturePaintContext
			Dim srcRas As java.awt.image.WritableRaster
			Dim filter As Boolean

			public Any(java.awt.image.WritableRaster srcRas, java.awt.image.ColorModel cm, java.awt.geom.AffineTransform xform, Integer maxw, Boolean filter)
				MyBase(cm, xform, srcRas.width, srcRas.height, maxw)
				Me.srcRas = srcRas
				Me.filter = filter

			public java.awt.image.WritableRaster makeRaster(Integer w, Integer h)
				Return makeRaster(colorModel, srcRas, w, h)

			public  Sub  rasterter(Integer x, Integer y, Integer xerr, Integer yerr, Integer w, Integer h, Integer bWidth, Integer bHeight, Integer colincx, Integer colincxerr, Integer colincy, Integer colincyerr, Integer rowincx, Integer rowincxerr, Integer rowincy, Integer rowincyerr)
				Dim data As Object = Nothing
				Dim rowx As Integer = x
				Dim rowy As Integer = y
				Dim rowxerr As Integer = xerr
				Dim rowyerr As Integer = yerr
				Dim srcRas As java.awt.image.WritableRaster = Me.srcRas
				Dim outRas As java.awt.image.WritableRaster = Me.outRas
				Dim rgbs As Integer() = If(filter, New Integer(3){}, Nothing)
				For j As Integer = 0 To h - 1
					x = rowx
					y = rowy
					xerr = rowxerr
					yerr = rowyerr
					For i As Integer = 0 To w - 1
						data = srcRas.getDataElements(x, y, data)
						If filter Then
							Dim nextx, nexty As Integer
							nextx = x + 1
							If nextx >= bWidth Then nextx = 0
							nexty = y + 1
							If nexty >= bHeight Then nexty = 0
							rgbs(0) = colorModel.getRGB(data)
							data = srcRas.getDataElements(nextx, y, data)
							rgbs(1) = colorModel.getRGB(data)
							data = srcRas.getDataElements(x, nexty, data)
							rgbs(2) = colorModel.getRGB(data)
							data = srcRas.getDataElements(nextx, nexty, data)
							rgbs(3) = colorModel.getRGB(data)
							Dim rgb As Integer = TexturePaintContext.blend(rgbs, xerr, yerr)
							data = colorModel.getDataElements(rgb, data)
						End If
						outRas.dataElementsnts(i, j, data)
						xerr += colincxerr
						If xerr < 0 Then
							xerr = xerr And  java.lang.[Integer].Max_Value
							x += 1
						End If
						x += colincx
						If x >= bWidth Then x -= bWidth
						yerr += colincyerr
						If yerr < 0 Then
							yerr = yerr And  java.lang.[Integer].Max_Value
							y += 1
						End If
						y += colincy
						If y >= bHeight Then y -= bHeight
					Next i
					rowxerr += rowincxerr
					If rowxerr < 0 Then
						rowxerr = rowxerr And  java.lang.[Integer].Max_Value
						rowx += 1
					End If
					rowx += rowincx
					If rowx >= bWidth Then rowx -= bWidth
					rowyerr += rowincyerr
					If rowyerr < 0 Then
						rowyerr = rowyerr And  java.lang.[Integer].Max_Value
						rowy += 1
					End If
					rowy += rowincy
					If rowy >= bHeight Then rowy -= bHeight
				Next j
	End Class

End Namespace