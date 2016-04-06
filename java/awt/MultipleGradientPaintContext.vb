Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' <summary>
	''' This is the superclass for all PaintContexts which use a multiple color
	''' gradient to fill in their raster.  It provides the actual color
	''' interpolation functionality.  Subclasses only have to deal with using
	''' the gradient to fill pixels in a raster.
	''' 
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	''' </summary>
	Friend MustInherit Class MultipleGradientPaintContext
		Implements PaintContext

		''' <summary>
		''' The PaintContext's ColorModel.  This is ARGB if colors are not all
		''' opaque, otherwise it is RGB.
		''' </summary>
		Protected Friend model As java.awt.image.ColorModel

		''' <summary>
		''' Color model used if gradient colors are all opaque. </summary>
		Private Shared xrgbmodel As java.awt.image.ColorModel = New java.awt.image.DirectColorModel(24, &Hff0000, &Hff00, &Hff)

		''' <summary>
		''' The cached ColorModel. </summary>
		Protected Friend Shared cachedModel As java.awt.image.ColorModel

		''' <summary>
		''' The cached raster, which is reusable among instances. </summary>
		Protected Friend Shared cached As WeakReference(Of java.awt.image.Raster)

		''' <summary>
		''' Raster is reused whenever possible. </summary>
		Protected Friend saved As java.awt.image.Raster

		''' <summary>
		''' The method to use when painting out of the gradient bounds. </summary>
		Protected Friend cycleMethod As java.awt.MultipleGradientPaint.CycleMethod

		''' <summary>
		''' The ColorSpace in which to perform the interpolation </summary>
		Protected Friend colorSpace As java.awt.MultipleGradientPaint.ColorSpaceType

		''' <summary>
		''' Elements of the inverse transform matrix. </summary>
		Protected Friend a00, a01, a10, a11, a02, a12 As Single

		''' <summary>
		''' This boolean specifies whether we are in simple lookup mode, where an
		''' input value between 0 and 1 may be used to directly index into a single
		''' array of gradient colors.  If this boolean value is false, then we have
		''' to use a 2-step process where we have to determine which gradient array
		''' we fall into, then determine the index into that array.
		''' </summary>
		Protected Friend isSimpleLookup As Boolean

		''' <summary>
		''' Size of gradients array for scaling the 0-1 index when looking up
		''' colors the fast way.
		''' </summary>
		Protected Friend fastGradientArraySize As Integer

		''' <summary>
		''' Array which contains the interpolated color values for each interval,
		''' used by calculateSingleArrayGradient().  It is protected for possible
		''' direct access by subclasses.
		''' </summary>
		Protected Friend gradient As Integer()

		''' <summary>
		''' Array of gradient arrays, one array for each interval.  Used by
		''' calculateMultipleArrayGradient().
		''' </summary>
		Private gradients As Integer()()

		''' <summary>
		''' Normalized intervals array. </summary>
		Private normalizedIntervals As Single()

		''' <summary>
		''' Fractions array. </summary>
		Private fractions As Single()

		''' <summary>
		''' Used to determine if gradient colors are all opaque. </summary>
		Private transparencyTest As Integer

		''' <summary>
		''' Color space conversion lookup tables. </summary>
		Private Shared ReadOnly SRGBtoLinearRGB As Integer() = New Integer(255){}
		Private Shared ReadOnly LinearRGBtoSRGB As Integer() = New Integer(255){}

		Shared Sub New()
			' build the tables
			For k As Integer = 0 To 255
				SRGBtoLinearRGB(k) = convertSRGBtoLinearRGB(k)
				LinearRGBtoSRGB(k) = convertLinearRGBtoSRGB(k)
			Next k
		End Sub

		''' <summary>
		''' Constant number of max colors between any 2 arbitrary colors.
		''' Used for creating and indexing gradients arrays.
		''' </summary>
		Protected Friend Const GRADIENT_SIZE As Integer = 256
		Protected Friend Shared ReadOnly GRADIENT_SIZE_INDEX As Integer = GRADIENT_SIZE -1

		''' <summary>
		''' Maximum length of the fast single-array.  If the estimated array size
		''' is greater than this, switch over to the slow lookup method.
		''' No particular reason for choosing this number, but it seems to provide
		''' satisfactory performance for the common case (fast lookup).
		''' </summary>
		Private Const MAX_GRADIENT_ARRAY_SIZE As Integer = 5000

		''' <summary>
		''' Constructor for MultipleGradientPaintContext superclass.
		''' </summary>
		Protected Friend Sub New(  mgp As MultipleGradientPaint,   cm As java.awt.image.ColorModel,   deviceBounds As Rectangle,   userBounds As java.awt.geom.Rectangle2D,   t As java.awt.geom.AffineTransform,   hints As RenderingHints,   fractions As Single(),   colors As Color(),   cycleMethod As java.awt.MultipleGradientPaint.CycleMethod,   colorSpace As java.awt.MultipleGradientPaint.ColorSpaceType)
			If deviceBounds Is Nothing Then Throw New NullPointerException("Device bounds cannot be null")

			If userBounds Is Nothing Then Throw New NullPointerException("User bounds cannot be null")

			If t Is Nothing Then Throw New NullPointerException("Transform cannot be null")

			If hints Is Nothing Then Throw New NullPointerException("RenderingHints cannot be null")

			' The inverse transform is needed to go from device to user space.
			' Get all the components of the inverse transform matrix.
			Dim tInv As java.awt.geom.AffineTransform
			Try
				' the following assumes that the caller has copied the incoming
				' transform and is not concerned about it being modified
				t.invert()
				tInv = t
			Catch e As java.awt.geom.NoninvertibleTransformException
				' just use identity transform in this case; better to show
				' (incorrect) results than to throw an exception and/or no-op
				tInv = New java.awt.geom.AffineTransform
			End Try
			Dim m As Double() = New Double(5){}
			tInv.getMatrix(m)
			a00 = CSng(m(0))
			a10 = CSng(m(1))
			a01 = CSng(m(2))
			a11 = CSng(m(3))
			a02 = CSng(m(4))
			a12 = CSng(m(5))

			' copy some flags
			Me.cycleMethod = cycleMethod
			Me.colorSpace = colorSpace

			' we can avoid copying this array since we do not modify its values
			Me.fractions = fractions

			' note that only one of these values can ever be non-null (we either
			' store the fast gradient array or the slow one, but never both
			' at the same time)
			Dim gradient As Integer() = If(mgp.gradient IsNot Nothing, mgp.gradient.get(), Nothing)
			Dim gradients As Integer()() = If(mgp.gradients IsNot Nothing, mgp.gradients.get(), Nothing)

			If gradient Is Nothing AndAlso gradients Is Nothing Then
				' we need to (re)create the appropriate values
				calculateLookupData(colors)

				' now cache the calculated values in the
				' MultipleGradientPaint instance for future use
				mgp.model = Me.model
				mgp.normalizedIntervals = Me.normalizedIntervals
				mgp.isSimpleLookup = Me.isSimpleLookup
				If isSimpleLookup Then
					' only cache the fast array
					mgp.fastGradientArraySize = Me.fastGradientArraySize
					mgp.gradient = New SoftReference(Of Integer())(Me.gradient)
				Else
					' only cache the slow array
					mgp.gradients = New SoftReference(Of Integer()())(Me.gradients)
				End If
			Else
				' use the values cached in the MultipleGradientPaint instance
				Me.model = mgp.model
				Me.normalizedIntervals = mgp.normalizedIntervals
				Me.isSimpleLookup = mgp.isSimpleLookup
				Me.gradient = gradient
				Me.fastGradientArraySize = mgp.fastGradientArraySize
				Me.gradients = gradients
			End If
		End Sub

		''' <summary>
		''' This function is the meat of this class.  It calculates an array of
		''' gradient colors based on an array of fractions and color values at
		''' those fractions.
		''' </summary>
		Private Sub calculateLookupData(  colors As Color())
			Dim normalizedColors As Color()
			If colorSpace = java.awt.MultipleGradientPaint.ColorSpaceType.LINEAR_RGB Then
				' create a new colors array
				normalizedColors = New Color(colors.Length - 1){}
				' convert the colors using the lookup table
				For i As Integer = 0 To colors.Length - 1
					Dim argb As Integer = colors(i).rGB
					Dim a As Integer = CInt(CUInt(argb) >> 24)
					Dim r As Integer = SRGBtoLinearRGB((argb >> 16) And &Hff)
					Dim g As Integer = SRGBtoLinearRGB((argb >> 8) And &Hff)
					Dim b As Integer = SRGBtoLinearRGB((argb) And &Hff)
					normalizedColors(i) = New Color(r, g, b, a)
				Next i
			Else
				' we can just use this array by reference since we do not
				' modify its values in the case of SRGB
				normalizedColors = colors
			End If

			' this will store the intervals (distances) between gradient stops
			normalizedIntervals = New Single(fractions.Length-2){}

			' convert from fractions into intervals
			For i As Integer = 0 To normalizedIntervals.Length - 1
				' interval distance is equal to the difference in positions
				normalizedIntervals(i) = Me.fractions(i+1) - Me.fractions(i)
			Next i

			' initialize to be fully opaque for ANDing with colors
			transparencyTest = &Hff000000L

			' array of interpolation arrays
			gradients = New Integer(normalizedIntervals.Length - 1)(){}

			' find smallest interval
			Dim Imin As Single = 1
			For i As Integer = 0 To normalizedIntervals.Length - 1
				Imin = If(Imin > normalizedIntervals(i), normalizedIntervals(i), Imin)
			Next i

			' Estimate the size of the entire gradients array.
			' This is to prevent a tiny interval from causing the size of array
			' to explode.  If the estimated size is too large, break to using
			' separate arrays for each interval, and using an indexing scheme at
			' look-up time.
			Dim estimatedSize As Integer = 0
			For i As Integer = 0 To normalizedIntervals.Length - 1
				estimatedSize += (normalizedIntervals(i)/Imin) * GRADIENT_SIZE
			Next i

			If estimatedSize > MAX_GRADIENT_ARRAY_SIZE Then
				' slow method
				calculateMultipleArrayGradient(normalizedColors)
			Else
				' fast method
				calculateSingleArrayGradient(normalizedColors, Imin)
			End If

			' use the most "economical" model
			If (CInt(CUInt(transparencyTest) >> 24)) = &Hff Then
				model = xrgbmodel
			Else
				model = java.awt.image.ColorModel.rGBdefault
			End If
		End Sub

		''' <summary>
		''' FAST LOOKUP METHOD
		''' 
		''' This method calculates the gradient color values and places them in a
		''' single int array, gradient[].  It does this by allocating space for
		''' each interval based on its size relative to the smallest interval in
		''' the array.  The smallest interval is allocated 255 interpolated values
		''' (the maximum number of unique in-between colors in a 24 bit color
		''' system), and all other intervals are allocated
		''' size = (255 * the ratio of their size to the smallest interval).
		''' 
		''' This scheme expedites a speedy retrieval because the colors are
		''' distributed along the array according to their user-specified
		''' distribution.  All that is needed is a relative index from 0 to 1.
		''' 
		''' The only problem with this method is that the possibility exists for
		''' the array size to balloon in the case where there is a
		''' disproportionately small gradient interval.  In this case the other
		''' intervals will be allocated huge space, but much of that data is
		''' redundant.  We thus need to use the space conserving scheme below.
		''' </summary>
		''' <param name="Imin"> the size of the smallest interval </param>
		Private Sub calculateSingleArrayGradient(  colors As Color(),   Imin As Single)
			' set the flag so we know later it is a simple (fast) lookup
			isSimpleLookup = True

			' 2 colors to interpolate
			Dim rgb1, rgb2 As Integer

			'the eventual size of the single array
			Dim gradientsTot As Integer = 1

			' for every interval (transition between 2 colors)
			For i As Integer = 0 To gradients.Length - 1
				' create an array whose size is based on the ratio to the
				' smallest interval
				Dim nGradients As Integer = CInt(Fix((normalizedIntervals(i)/Imin)*255f))
				gradientsTot += nGradients
				gradients(i) = New Integer(nGradients - 1){}

				' the 2 colors (keyframes) to interpolate between
				rgb1 = colors(i).rGB
				rgb2 = colors(i+1).rGB

				' fill this array with the colors in between rgb1 and rgb2
				interpolate(rgb1, rgb2, gradients(i))

				' if the colors are opaque, transparency should still
				' be 0xff000000
				transparencyTest = transparencyTest And rgb1
				transparencyTest = transparencyTest And rgb2
			Next i

			' put all gradients in a single array
			gradient = New Integer(gradientsTot - 1){}
			Dim curOffset As Integer = 0
			For i As Integer = 0 To gradients.Length - 1
				Array.Copy(gradients(i), 0, gradient, curOffset, gradients(i).Length)
				curOffset += gradients(i).Length
			Next i
			gradient(gradient.Length-1) = colors(colors.Length-1).rGB

			' if interpolation occurred in Linear RGB space, convert the
			' gradients back to sRGB using the lookup table
			If colorSpace = java.awt.MultipleGradientPaint.ColorSpaceType.LINEAR_RGB Then
				For i As Integer = 0 To gradient.Length - 1
					gradient(i) = convertEntireColorLinearRGBtoSRGB(gradient(i))
				Next i
			End If

			fastGradientArraySize = gradient.Length - 1
		End Sub

		''' <summary>
		''' SLOW LOOKUP METHOD
		''' 
		''' This method calculates the gradient color values for each interval and
		''' places each into its own 255 size array.  The arrays are stored in
		''' gradients[][].  (255 is used because this is the maximum number of
		''' unique colors between 2 arbitrary colors in a 24 bit color system.)
		''' 
		''' This method uses the minimum amount of space (only 255 * number of
		''' intervals), but it aggravates the lookup procedure, because now we
		''' have to find out which interval to select, then calculate the index
		''' within that interval.  This causes a significant performance hit,
		''' because it requires this calculation be done for every point in
		''' the rendering loop.
		''' 
		''' For those of you who are interested, this is a classic example of the
		''' time-space tradeoff.
		''' </summary>
		Private Sub calculateMultipleArrayGradient(  colors As Color())
			' set the flag so we know later it is a non-simple lookup
			isSimpleLookup = False

			' 2 colors to interpolate
			Dim rgb1, rgb2 As Integer

			' for every interval (transition between 2 colors)
			For i As Integer = 0 To gradients.Length - 1
				' create an array of the maximum theoretical size for
				' each interval
				gradients(i) = New Integer(GRADIENT_SIZE - 1){}

				' get the the 2 colors
				rgb1 = colors(i).rGB
				rgb2 = colors(i+1).rGB

				' fill this array with the colors in between rgb1 and rgb2
				interpolate(rgb1, rgb2, gradients(i))

				' if the colors are opaque, transparency should still
				' be 0xff000000
				transparencyTest = transparencyTest And rgb1
				transparencyTest = transparencyTest And rgb2
			Next i

			' if interpolation occurred in Linear RGB space, convert the
			' gradients back to SRGB using the lookup table
			If colorSpace = java.awt.MultipleGradientPaint.ColorSpaceType.LINEAR_RGB Then
				For j As Integer = 0 To gradients.Length - 1
					For i As Integer = 0 To gradients(j).Length - 1
						gradients(j)(i) = convertEntireColorLinearRGBtoSRGB(gradients(j)(i))
					Next i
				Next j
			End If
		End Sub

		''' <summary>
		''' Yet another helper function.  This one linearly interpolates between
		''' 2 colors, filling up the output array.
		''' </summary>
		''' <param name="rgb1"> the start color </param>
		''' <param name="rgb2"> the end color </param>
		''' <param name="output"> the output array of colors; must not be null </param>
		Private Sub interpolate(  rgb1 As Integer,   rgb2 As Integer,   output As Integer())
			' color components
			Dim a1, r1, g1, b1, da, dr, dg, db As Integer

			' step between interpolated values
			Dim stepSize As Single = 1.0f / output.Length

			' extract color components from packed integer
			a1 = (rgb1 >> 24) And &Hff
			r1 = (rgb1 >> 16) And &Hff
			g1 = (rgb1 >> 8) And &Hff
			b1 = (rgb1) And &Hff

			' calculate the total change in alpha, red, green, blue
			da = ((rgb2 >> 24) And &Hff) - a1
			dr = ((rgb2 >> 16) And &Hff) - r1
			dg = ((rgb2 >> 8) And &Hff) - g1
			db = ((rgb2) And &Hff) - b1

			' for each step in the interval calculate the in-between color by
			' multiplying the normalized current position by the total color
			' change (0.5 is added to prevent truncation round-off error)
			For i As Integer = 0 To output.Length - 1
				output(i) = ((CInt(Fix((a1 + i * da * stepSize) + 0.5)) << 24)) Or ((CInt(Fix((r1 + i * dr * stepSize) + 0.5)) << 16)) Or ((CInt(Fix((g1 + i * dg * stepSize) + 0.5)) << 8)) Or ((CInt(Fix((b1 + i * db * stepSize) + 0.5))))
			Next i
		End Sub

		''' <summary>
		''' Yet another helper function.  This one extracts the color components
		''' of an integer RGB triple, converts them from LinearRGB to SRGB, then
		''' recompacts them into an int.
		''' </summary>
		Private Function convertEntireColorLinearRGBtoSRGB(  rgb As Integer) As Integer
			' color components
			Dim a1, r1, g1, b1 As Integer

			' extract red, green, blue components
			a1 = (rgb >> 24) And &Hff
			r1 = (rgb >> 16) And &Hff
			g1 = (rgb >> 8) And &Hff
			b1 = (rgb) And &Hff

			' use the lookup table
			r1 = LinearRGBtoSRGB(r1)
			g1 = LinearRGBtoSRGB(g1)
			b1 = LinearRGBtoSRGB(b1)

			' re-compact the components
			Return ((a1 << 24) Or (r1 << 16) Or (g1 << 8) Or (b1))
		End Function

		''' <summary>
		''' Helper function to index into the gradients array.  This is necessary
		''' because each interval has an array of colors with uniform size 255.
		''' However, the color intervals are not necessarily of uniform length, so
		''' a conversion is required.
		''' </summary>
		''' <param name="position"> the unmanipulated position, which will be mapped
		'''                 into the range 0 to 1
		''' @returns integer color to display </param>
		Protected Friend Function indexIntoGradientsArrays(  position As Single) As Integer
			' first, manipulate position value depending on the cycle method
			If cycleMethod = java.awt.MultipleGradientPaint.CycleMethod.NO_CYCLE Then
				If position > 1 Then
					' upper bound is 1
					position = 1
				ElseIf position < 0 Then
					' lower bound is 0
					position = 0
				End If
			ElseIf cycleMethod = java.awt.MultipleGradientPaint.CycleMethod.REPEAT Then
				' get the fractional part
				' (modulo behavior discards integer component)
				position = position - CInt(Fix(position))

				'position should now be between -1 and 1
				If position < 0 Then
					' force it to be in the range 0-1
					position = position + 1
				End If ' cycleMethod == CycleMethod.REFLECT
			Else
				If position < 0 Then position = -position

				' get the integer part
				Dim part As Integer = CInt(Fix(position))

				' get the fractional part
				position = position - part

				If (part And 1) = 1 Then position = 1 - position
			End If

			' now, get the color based on this 0-1 position...

			If isSimpleLookup Then
				' easy to compute: just scale index by array size
				Return gradient(CInt(Fix(position * fastGradientArraySize)))
			Else
				' more complicated computation, to save space

				' for all the gradient interval arrays
				For i As Integer = 0 To gradients.Length - 1
					If position < fractions(i+1) Then
						' this is the array we want
						Dim delta As Single = position - fractions(i)

						' this is the interval we want
						Dim index As Integer = CInt(Fix((delta / normalizedIntervals(i)) * (GRADIENT_SIZE_INDEX)))

						Return gradients(i)(index)
					End If
				Next i
			End If

			Return gradients(gradients.Length - 1)(GRADIENT_SIZE_INDEX)
		End Function

		''' <summary>
		''' Helper function to convert a color component in sRGB space to linear
		''' RGB space.  Used to build a static lookup table.
		''' </summary>
		Private Shared Function convertSRGBtoLinearRGB(  color_Renamed As Integer) As Integer
			Dim input, output As Single

			input = color_Renamed / 255.0f
			If input <= 0.04045f Then
				output = input / 12.92f
			Else
				output = CSng (System.Math.Pow((input + 0.055) / 1.055, 2.4))
			End If

			Return System.Math.Round(output * 255.0f)
		End Function

		''' <summary>
		''' Helper function to convert a color component in linear RGB space to
		''' SRGB space.  Used to build a static lookup table.
		''' </summary>
		Private Shared Function convertLinearRGBtoSRGB(  color_Renamed As Integer) As Integer
			Dim input, output As Single

			input = color_Renamed/255.0f
			If input <= 0.0031308 Then
				output = input * 12.92f
			Else
				output = (1.055f * (CSng (System.Math.Pow(input, (1.0 / 2.4))))) - 0.055f
			End If

			Return System.Math.Round(output * 255.0f)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Function getRaster(  x As Integer,   y As Integer,   w As Integer,   h As Integer) As java.awt.image.Raster Implements PaintContext.getRaster
			' If working raster is big enough, reuse it. Otherwise,
			' build a large enough new one.
			Dim raster_Renamed As java.awt.image.Raster = saved
			If raster_Renamed Is Nothing OrElse raster_Renamed.width < w OrElse raster_Renamed.height < h Then
				raster_Renamed = getCachedRaster(model, w, h)
				saved = raster_Renamed
			End If

			' Access raster internal int array. Because we use a DirectColorModel,
			' we know the DataBuffer is of type DataBufferInt and the SampleModel
			' is SinglePixelPackedSampleModel.
			' Adjust for initial offset in DataBuffer and also for the scanline
			' stride.
			' These calls make the DataBuffer non-acceleratable, but the
			' Raster is never Stable long enough to accelerate anyway...
			Dim rasterDB As java.awt.image.DataBufferInt = CType(raster_Renamed.dataBuffer, java.awt.image.DataBufferInt)
			Dim pixels As Integer() = rasterDB.getData(0)
			Dim [off] As Integer = rasterDB.offset
			Dim scanlineStride As Integer = CType(raster_Renamed.sampleModel, java.awt.image.SinglePixelPackedSampleModel).scanlineStride
			Dim adjust As Integer = scanlineStride - w

			fillRaster(pixels, [off], adjust, x, y, w, h) ' delegate to subclass

			Return raster_Renamed
		End Function

		Protected Friend MustOverride Sub fillRaster(Integer   As pixels(),   [off] As Integer,   adjust As Integer,   x As Integer,   y As Integer,   w As Integer,   h As Integer)


		''' <summary>
		''' Took this cacheRaster code from GradientPaint. It appears to recycle
		''' rasters for use by any other instance, as long as they are sufficiently
		''' large.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function getCachedRaster(  cm As java.awt.image.ColorModel,   w As Integer,   h As Integer) As java.awt.image.Raster
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

		''' <summary>
		''' Took this cacheRaster code from GradientPaint. It appears to recycle
		''' rasters for use by any other instance, as long as they are sufficiently
		''' large.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub putCachedRaster(  cm As java.awt.image.ColorModel,   ras As java.awt.image.Raster)
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
			cached = New WeakReference(Of java.awt.image.Raster)(ras)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Sub dispose() Implements PaintContext.dispose
			If saved IsNot Nothing Then
				putCachedRaster(model, saved)
				saved = Nothing
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property colorModel As java.awt.image.ColorModel Implements PaintContext.getColorModel
			Get
				Return model
			End Get
		End Property
	End Class

End Namespace