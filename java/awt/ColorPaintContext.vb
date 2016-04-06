Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1997, 2007, Oracle and/or its affiliates. All rights reserved.
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


	Friend Class ColorPaintContext
		Implements PaintContext

		Friend color_Renamed As Integer
		Friend savedTile As java.awt.image.WritableRaster

		Protected Friend Sub New(  color_Renamed As Integer,   cm As java.awt.image.ColorModel)
			Me.color_Renamed = color_Renamed
		End Sub

		Public Overridable Sub dispose() Implements PaintContext.dispose
		End Sub

	'    
	'     * Returns the RGB value representing the color in the default sRGB
	'     * {@link ColorModel}.
	'     * (Bits 24-31 are alpha, 16-23 are red, 8-15 are green, 0-7 are
	'     * blue).
	'     * @return the RGB value of the color in the default sRGB
	'     *         <code>ColorModel</code>.
	'     * @see java.awt.image.ColorModel#getRGBdefault
	'     * @see #getRed
	'     * @see #getGreen
	'     * @see #getBlue
	'     
		Friend Overridable Property rGB As Integer
			Get
				Return color_Renamed
			End Get
		End Property

		Public Overridable Property colorModel As java.awt.image.ColorModel Implements PaintContext.getColorModel
			Get
				Return java.awt.image.ColorModel.rGBdefault
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getRaster(  x As Integer,   y As Integer,   w As Integer,   h As Integer) As java.awt.image.Raster Implements PaintContext.getRaster
			Dim t As java.awt.image.WritableRaster = savedTile

			If t Is Nothing OrElse w > t.width OrElse h > t.height Then
				t = colorModel.createCompatibleWritableRaster(w, h)
				Dim icr As sun.awt.image.IntegerComponentRaster = CType(t, sun.awt.image.IntegerComponentRaster)
				java.util.Arrays.fill(icr.dataStorage, color_Renamed)
				' Note - markDirty is probably unnecessary since icr is brand new
				icr.markDirty()
				If w <= 64 AndAlso h <= 64 Then savedTile = t
			End If

			Return t
		End Function
	End Class

End Namespace