Imports Microsoft.VisualBasic
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


	Friend Class GradientPaintContext
		Implements PaintContext

		Friend Shared xrgbmodel As java.awt.image.ColorModel = New java.awt.image.DirectColorModel(24, &Hff0000, &Hff00, &Hff)
		Friend Shared xbgrmodel As java.awt.image.ColorModel = New java.awt.image.DirectColorModel(24, &Hff, &Hff00, &Hff0000)

		Friend Shared cachedModel As java.awt.image.ColorModel
		Friend Shared cached As WeakReference(Of java.awt.image.Raster)

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function getCachedRaster(ByVal cm As java.awt.image.ColorModel, ByVal w As Integer, ByVal h As Integer) As java.awt.image.Raster
			If cm Is cachedModel Then
				If cached IsNot Nothing Then
					Dim ras As java.awt.image.Raster = CType(cached.get(), java.awt.image.Raster)
					If ras IsNot Nothing AndAlso ras.width >= w AndAlso ras.height >= h Then
						cached = Nothing
						Return ras
					End If
				End If
			End If
			Return cm.createCompatibleWritableRaster(w, h)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub putCachedRaster(ByVal cm As java.awt.image.ColorModel, ByVal ras As java.awt.image.Raster)
			If cached IsNot Nothing Then
				Dim cras As java.awt.image.Raster = CType(cached.get(), java.awt.image.Raster)
				If cras IsNot Nothing Then
					Dim cw As Integer = cras.width
					Dim ch As Integer = cras.height
					Dim iw As Integer = ras.width
					Dim ih As Integer = ras.height
					If cw >= iw AndAlso ch >= ih Then Return
					If cw * ch >= iw * ih Then Return
				End If
			End If
			cachedModel = cm
			cached = New WeakReference(Of )(ras)
		End Sub

		Friend x1 As Double
		Friend y1 As Double
		Friend dx As Double
		Friend dy As Double
		Friend cyclic As Boolean
		Friend interp As Integer()
		Friend saved As java.awt.image.Raster
		Friend model As java.awt.image.ColorModel

		Public Sub New(ByVal cm As java.awt.image.ColorModel, ByVal p1 As java.awt.geom.Point2D, ByVal p2 As java.awt.geom.Point2D, ByVal xform As java.awt.geom.AffineTransform, ByVal c1 As Color, ByVal c2 As Color, ByVal cyclic As Boolean)
			' First calculate the distance moved in user space when
			' we move a single unit along the X & Y axes in device space.
			Dim xvec As java.awt.geom.Point2D = New java.awt.geom.Point2D.Double(1, 0)
			Dim yvec As java.awt.geom.Point2D = New java.awt.geom.Point2D.Double(0, 1)
			Try
				Dim inverse As java.awt.geom.AffineTransform = xform.createInverse()
				inverse.deltaTransform(xvec, xvec)
				inverse.deltaTransform(yvec, yvec)
			Catch e As java.awt.geom.NoninvertibleTransformException
				xvec.locationion(0, 0)
				yvec.locationion(0, 0)
			End Try

			' Now calculate the (square of the) user space distance
			' between the anchor points. This value equals:
			'     (UserVec . UserVec)
			Dim udx As Double = p2.x - p1.x
			Dim udy As Double = p2.y - p1.y
			Dim ulenSq As Double = udx * udx + udy * udy

			If ulenSq <= java.lang.[Double].MIN_VALUE Then
				dx = 0
				dy = 0
			Else
				' Now calculate the proportional distance moved along the
				' vector from p1 to p2 when we move a unit along X & Y in
				' device space.
				'
				' The length of the projection of the Device Axis Vector is
				' its dot product with the Unit User Vector:
				'     (DevAxisVec . (UserVec / Len(UserVec))
				'
				' The "proportional" length is that length divided again
				' by the length of the User Vector:
				'     (DevAxisVec . (UserVec / Len(UserVec))) / Len(UserVec)
				' which simplifies to:
				'     ((DevAxisVec . UserVec) / Len(UserVec)) / Len(UserVec)
				' which simplifies to:
				'     (DevAxisVec . UserVec) / LenSquared(UserVec)
				dx = (xvec.x * udx + xvec.y * udy) / ulenSq
				dy = (yvec.x * udx + yvec.y * udy) / ulenSq

				If cyclic Then
					dx = dx Mod 1.0
					dy = dy Mod 1.0
				Else
					' We are acyclic
					If dx < 0 Then
						' If we are using the acyclic form below, we need
						' dx to be non-negative for simplicity of scanning
						' across the scan lines for the transition points.
						' To ensure that constraint, we negate the dx/dy
						' values and swap the points and colors.
						Dim p As java.awt.geom.Point2D = p1
						p1 = p2
						p2 = p
						Dim c As Color = c1
						c1 = c2
						c2 = c
						dx = -dx
						dy = -dy
					End If
				End If
			End If

			Dim dp1 As java.awt.geom.Point2D = xform.transform(p1, Nothing)
			Me.x1 = dp1.x
			Me.y1 = dp1.y

			Me.cyclic = cyclic
			Dim rgb1 As Integer = c1.rGB
			Dim rgb2 As Integer = c2.rGB
			Dim a1 As Integer = (rgb1 >> 24) And &Hff
			Dim r1 As Integer = (rgb1 >> 16) And &Hff
			Dim g1 As Integer = (rgb1 >> 8) And &Hff
			Dim b1 As Integer = (rgb1) And &Hff
			Dim da As Integer = ((rgb2 >> 24) And &Hff) - a1
			Dim dr As Integer = ((rgb2 >> 16) And &Hff) - r1
			Dim dg As Integer = ((rgb2 >> 8) And &Hff) - g1
			Dim db As Integer = ((rgb2) And &Hff) - b1
			If a1 = &Hff AndAlso da = 0 Then
				model = xrgbmodel
				If TypeOf cm Is java.awt.image.DirectColorModel Then
					Dim dcm As java.awt.image.DirectColorModel = CType(cm, java.awt.image.DirectColorModel)
					Dim tmp As Integer = dcm.alphaMask
					If (tmp = 0 OrElse tmp = &Hff) AndAlso dcm.redMask = &Hff AndAlso dcm.greenMask = &Hff00 AndAlso dcm.blueMask = &Hff0000 Then
						model = xbgrmodel
						tmp = r1
						r1 = b1
						b1 = tmp
						tmp = dr
						dr = db
						db = tmp
					End If
				End If
			Else
				model = java.awt.image.ColorModel.rGBdefault
			End If
			interp = New Integer(If(cyclic, 513, 257) - 1){}
			For i As Integer = 0 To 256
				Dim rel As Single = i / 256.0f
				Dim rgb As Integer = ((CInt(Fix(a1 + da * rel))) << 24) Or ((CInt(Fix(r1 + dr * rel))) << 16) Or ((CInt(Fix(g1 + dg * rel))) << 8) Or ((CInt(Fix(b1 + db * rel))))
				interp(i) = rgb
				If cyclic Then interp(512 - i) = rgb
			Next i
		End Sub

		''' <summary>
		''' Release the resources allocated for the operation.
		''' </summary>
		Public Overridable Sub dispose() Implements PaintContext.dispose
			If saved IsNot Nothing Then
				putCachedRaster(model, saved)
				saved = Nothing
			End If
		End Sub

		''' <summary>
		''' Return the ColorModel of the output.
		''' </summary>
		Public Overridable Property colorModel As java.awt.image.ColorModel Implements PaintContext.getColorModel
			Get
				Return model
			End Get
		End Property

		''' <summary>
		''' Return a Raster containing the colors generated for the graphics
		''' operation. </summary>
		''' <param name="x">,y,w,h The area in device space for which colors are
		''' generated. </param>
		Public Overridable Function getRaster(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As java.awt.image.Raster Implements PaintContext.getRaster
			Dim rowrel As Double = (x - x1) * dx + (y - y1) * dy

			Dim rast As java.awt.image.Raster = saved
			If rast Is Nothing OrElse rast.width < w OrElse rast.height < h Then
				rast = getCachedRaster(model, w, h)
				saved = rast
			End If
			Dim irast As sun.awt.image.IntegerComponentRaster = CType(rast, sun.awt.image.IntegerComponentRaster)
			Dim [off] As Integer = irast.getDataOffset(0)
			Dim adjust As Integer = irast.scanlineStride - w
			Dim pixels As Integer() = irast.dataStorage

			If cyclic Then
				cycleFillRaster(pixels, [off], adjust, w, h, rowrel, dx, dy)
			Else
				clipFillRaster(pixels, [off], adjust, w, h, rowrel, dx, dy)
			End If

			irast.markDirty()

			Return rast
		End Function

		Friend Overridable Sub cycleFillRaster(ByVal pixels As Integer(), ByVal [off] As Integer, ByVal adjust As Integer, ByVal w As Integer, ByVal h As Integer, ByVal rowrel As Double, ByVal dx As Double, ByVal dy As Double)
			rowrel = rowrel Mod 2.0
			Dim irowrel As Integer = (CInt(Fix(rowrel * (1 << 30)))) << 1
			Dim idx As Integer = CInt(Fix(-dx * (1 << 31)))
			Dim idy As Integer = CInt(Fix(-dy * (1 << 31)))
			h -= 1
			Do While h >= 0
				Dim icolrel As Integer = irowrel
				For j As Integer = w To 1 Step -1
					pixels([off]) = interp(CInt(CUInt(icolrel) >> 23))
					[off] += 1
					icolrel += idx
				Next j

				[off] += adjust
				irowrel += idy
				h -= 1
			Loop
		End Sub

		Friend Overridable Sub clipFillRaster(ByVal pixels As Integer(), ByVal [off] As Integer, ByVal adjust As Integer, ByVal w As Integer, ByVal h As Integer, ByVal rowrel As Double, ByVal dx As Double, ByVal dy As Double)
			h -= 1
			Do While h >= 0
				Dim colrel As Double = rowrel
				Dim j As Integer = w
				If colrel <= 0.0 Then
					Dim rgb As Integer = interp(0)
					Do
						pixels([off]) = rgb
						[off] += 1
						colrel += dx
						j -= 1
					Loop While j > 0 AndAlso colrel <= 0.0
				End If
				j -= 1
				Do While colrel < 1.0 AndAlso j >= 0
					pixels([off]) = interp(CInt(Fix(colrel * 256)))
					[off] += 1
					colrel += dx
					j -= 1
				Loop
				If j > 0 Then
					Dim rgb As Integer = interp(256)
					Do
						pixels([off]) = rgb
						[off] += 1
						j -= 1
					Loop While j > 0
				End If

				[off] += adjust
				rowrel += dy
				h -= 1
			Loop
		End Sub
	End Class

End Namespace