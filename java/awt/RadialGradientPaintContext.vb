Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the actual implementation for the RadialGradientPaint.
	''' This is where the pixel processing is done.  A RadialGradienPaint
	''' only supports circular gradients, but it should be possible to scale
	''' the circle to look approximately elliptical, by means of a
	''' gradient transform passed into the RadialGradientPaint constructor.
	''' 
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	''' </summary>
	Friend NotInheritable Class RadialGradientPaintContext
		Inherits MultipleGradientPaintContext

		''' <summary>
		''' True when (focus == center). </summary>
		Private isSimpleFocus As Boolean = False

		''' <summary>
		''' True when (cycleMethod == NO_CYCLE). </summary>
		Private isNonCyclic As Boolean = False

		''' <summary>
		''' Radius of the outermost circle defining the 100% gradient stop. </summary>
		Private radius As Single

		''' <summary>
		''' Variables representing center and focus points. </summary>
		Private centerX, centerY, focusX, focusY As Single

		''' <summary>
		''' Radius of the gradient circle squared. </summary>
		Private radiusSq As Single

		''' <summary>
		''' Constant part of X, Y user space coordinates. </summary>
		Private constA, constB As Single

		''' <summary>
		''' Constant second order delta for simple loop. </summary>
		Private gDeltaDelta As Single

		''' <summary>
		''' This value represents the solution when focusX == X.  It is called
		''' trivial because it is easier to calculate than the general case.
		''' </summary>
		Private trivial As Single

		''' <summary>
		''' Amount for offset when clamping focus. </summary>
		Private Const SCALEBACK As Single =.99f

		''' <summary>
		''' Constructor for RadialGradientPaintContext.
		''' </summary>
		''' <param name="paint"> the {@code RadialGradientPaint} from which this context
		'''              is created </param>
		''' <param name="cm"> the {@code ColorModel} that receives
		'''           the {@code Paint} data (this is used only as a hint) </param>
		''' <param name="deviceBounds"> the device space bounding box of the
		'''                     graphics primitive being rendered </param>
		''' <param name="userBounds"> the user space bounding box of the
		'''                   graphics primitive being rendered </param>
		''' <param name="t"> the {@code AffineTransform} from user
		'''          space into device space (gradientTransform should be
		'''          concatenated with this) </param>
		''' <param name="hints"> the hints that the context object uses to choose
		'''              between rendering alternatives </param>
		''' <param name="cx"> the center X coordinate in user space of the circle defining
		'''           the gradient.  The last color of the gradient is mapped to
		'''           the perimeter of this circle. </param>
		''' <param name="cy"> the center Y coordinate in user space of the circle defining
		'''           the gradient.  The last color of the gradient is mapped to
		'''           the perimeter of this circle. </param>
		''' <param name="r"> the radius of the circle defining the extents of the
		'''          color gradient </param>
		''' <param name="fx"> the X coordinate in user space to which the first color
		'''           is mapped </param>
		''' <param name="fy"> the Y coordinate in user space to which the first color
		'''           is mapped </param>
		''' <param name="fractions"> the fractions specifying the gradient distribution </param>
		''' <param name="colors"> the gradient colors </param>
		''' <param name="cycleMethod"> either NO_CYCLE, REFLECT, or REPEAT </param>
		''' <param name="colorSpace"> which colorspace to use for interpolation,
		'''                   either SRGB or LINEAR_RGB </param>
		Friend Sub New(ByVal paint As RadialGradientPaint, ByVal cm As java.awt.image.ColorModel, ByVal deviceBounds As Rectangle, ByVal userBounds As java.awt.geom.Rectangle2D, ByVal t As java.awt.geom.AffineTransform, ByVal hints As RenderingHints, ByVal cx As Single, ByVal cy As Single, ByVal r As Single, ByVal fx As Single, ByVal fy As Single, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As java.awt.MultipleGradientPaint.CycleMethod, ByVal colorSpace As java.awt.MultipleGradientPaint.ColorSpaceType)
			MyBase.New(paint, cm, deviceBounds, userBounds, t, hints, fractions, colors, cycleMethod, colorSpace)

			' copy some parameters
			centerX = cx
			centerY = cy
			focusX = fx
			focusY = fy
			radius = r

			Me.isSimpleFocus = (focusX = centerX) AndAlso (focusY = centerY)
			Me.isNonCyclic = (cycleMethod = java.awt.MultipleGradientPaint.CycleMethod.NO_CYCLE)

			' for use in the quadractic equation
			radiusSq = radius * radius

			Dim dX As Single = focusX - centerX
			Dim dY As Single = focusY - centerY

			Dim distSq As Double = (dX * dX) + (dY * dY)

			' test if distance from focus to center is greater than the radius
			If distSq > radiusSq * SCALEBACK Then
				' clamp focus to radius
				Dim scalefactor As Single = CSng (System.Math.Sqrt(radiusSq * SCALEBACK / distSq))
				dX = dX * scalefactor
				dY = dY * scalefactor
				focusX = centerX + dX
				focusY = centerY + dY
			End If

			' calculate the solution to be used in the case where X == focusX
			' in cyclicCircularGradientFillRaster()
			trivial = CSng (System.Math.Sqrt(radiusSq - (dX * dX)))

			' constant parts of X, Y user space coordinates
			constA = a02 - centerX
			constB = a12 - centerY

			' constant second order delta for simple loop
			gDeltaDelta = 2 * (a00 * a00 + a10 * a10) / radiusSq
		End Sub

		''' <summary>
		''' Return a Raster containing the colors generated for the graphics
		''' operation.
		''' </summary>
		''' <param name="x">,y,w,h the area in device space for which colors are
		''' generated. </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		protected  Sub  fillRaster(int pixels() , int off, int adjust, int x, int y, int w, int h)
			If isSimpleFocus AndAlso isNonCyclic AndAlso isSimpleLookup Then
				simpleNonCyclicFillRaster(pixels, off, adjust, x, y, w, h)
			Else
				cyclicCircularGradientFillRaster(pixels, off, adjust, x, y, w, h)
			End If

		''' <summary>
		''' This code works in the simplest of cases, where the focus == center
		''' point, the gradient is noncyclic, and the gradient lookup method is
		''' fast (single array index, no conversion necessary).
		''' </summary>
		private  Sub  simpleNonCyclicFillRaster(Integer pixels() , Integer off, Integer adjust, Integer x, Integer y, Integer w, Integer h)
	'         We calculate sqrt(X^2 + Y^2) relative to the radius
	'         * size to get the fraction for the color to use.
	'         *
	'         * Each step along the scanline adds (a00, a10) to (X, Y).
	'         * If we precalculate:
	'         *   gRel = X^2+Y^2
	'         * for the start of the row, then for each step we need to
	'         * calculate:
	'         *   gRel' = (X+a00)^2 + (Y+a10)^2
	'         *         = X^2 + 2*X*a00 + a00^2 + Y^2 + 2*Y*a10 + a10^2
	'         *         = (X^2+Y^2) + 2*(X*a00+Y*a10) + (a00^2+a10^2)
	'         *         = gRel + 2*(X*a00+Y*a10) + (a00^2+a10^2)
	'         *         = gRel + 2*DP + SD
	'         * (where DP = dot product between X,Y and a00,a10
	'         *  and   SD = dot product square of the delta vector)
	'         * For the step after that we get:
	'         *   gRel'' = (X+2*a00)^2 + (Y+2*a10)^2
	'         *          = X^2 + 4*X*a00 + 4*a00^2 + Y^2 + 4*Y*a10 + 4*a10^2
	'         *          = (X^2+Y^2) + 4*(X*a00+Y*a10) + 4*(a00^2+a10^2)
	'         *          = gRel  + 4*DP + 4*SD
	'         *          = gRel' + 2*DP + 3*SD
	'         * The increment changed by:
	'         *     (gRel'' - gRel') - (gRel' - gRel)
	'         *   = (2*DP + 3*SD) - (2*DP + SD)
	'         *   = 2*SD
	'         * Note that this value depends only on the (inverse of the)
	'         * transformation matrix and so is a constant for the loop.
	'         * To make this all relative to the unit circle, we need to
	'         * divide all values as follows:
	'         *   [XY] /= radius
	'         *   gRel /= radiusSq
	'         *   DP   /= radiusSq
	'         *   SD   /= radiusSq
	'         
			' coordinates of UL corner in "user space" relative to center
			Dim rowX As Single = (a00*x) + (a01*y) + constA
			Dim rowY As Single = (a10*x) + (a11*y) + constB

			' second order delta calculated in constructor
			Dim gDeltaDelta As Single = Me.gDeltaDelta

			' adjust is (scan-w) of pixels array, we need (scan)
			adjust += w

			' rgb of the 1.0 color used when the distance exceeds gradient radius
			Dim rgbclip As Integer = gradient(fastGradientArraySize)

			For j As Integer = 0 To h - 1
				' these values depend on the coordinates of the start of the row
				Dim gRel As Single = (rowX * rowX + rowY * rowY) / radiusSq
				Dim gDelta As Single = (2 * (a00 * rowX + a10 * rowY) / radiusSq + gDeltaDelta/2)

	'             Use optimized loops for any cases where gRel >= 1.
	'             * We do not need to calculate sqrt(gRel) for these
	'             * values since sqrt(N>=1) == (M>=1).
	'             * Note that gRel follows a parabola which can only be < 1
	'             * for a small region around the center on each scanline. In
	'             * particular:
	'             *   gDeltaDelta is always positive
	'             *   gDelta is <0 until it crosses the midpoint, then >0
	'             * To the left and right of that region, it will always be
	'             * >=1 out to infinity, so we can process the line in 3
	'             * regions:
	'             *   out to the left  - quick fill until gRel < 1, updating gRel
	'             *   in the heart     - slow fraction=sqrt fill while gRel < 1
	'             *   out to the right - quick fill rest of scanline, ignore gRel
	'             
				Dim i As Integer = 0
				' Quick fill for "out to the left"
				Do While i < w AndAlso gRel >= 1.0f
					pixels(off + i) = rgbclip
					gRel += gDelta
					gDelta += gDeltaDelta
					i += 1
				Loop
				' Slow fill for "in the heart"
				Do While i < w AndAlso gRel < 1.0f
					Dim gIndex As Integer

					If gRel <= 0 Then
						gIndex = 0
					Else
						Dim fIndex As Single = gRel * SQRT_LUT_SIZE
						Dim iIndex As Integer = CInt(Fix(fIndex))
						Dim s0 As Single = sqrtLut(iIndex)
						Dim s1 As Single = sqrtLut(iIndex+1) - s0
						fIndex = s0 + (fIndex - iIndex) * s1
						gIndex = CInt(Fix(fIndex * fastGradientArraySize))
					End If

					' store the color at this point
					pixels(off + i) = gradient(gIndex)

					' incremental calculation
					gRel += gDelta
					gDelta += gDeltaDelta
					i += 1
				Loop
				' Quick fill to end of line for "out to the right"
				Do While i < w
					pixels(off + i) = rgbclip
					i += 1
				Loop

				off += adjust
				rowX += a01
				rowY += a11
			Next j

		' SQRT_LUT_SIZE must be a power of 2 for the test above to work.
		private static final Integer SQRT_LUT_SIZE = (1 << 11)
		private static Single sqrtLut() = New Single(SQRT_LUT_SIZE){}
		static RadialGradientPaintContext()
			For i As Integer = 0 To sqrtLut.Length - 1
				sqrtLut(i) = CSng (System.Math.Sqrt(i / (CSng(SQRT_LUT_SIZE))))
			Next i

		''' <summary>
		''' Fill the raster, cycling the gradient colors when a point falls outside
		''' of the perimeter of the 100% stop circle.
		''' 
		''' This calculation first computes the intersection point of the line
		''' from the focus through the current point in the raster, and the
		''' perimeter of the gradient circle.
		''' 
		''' Then it determines the percentage distance of the current point along
		''' that line (focus is 0%, perimeter is 100%).
		''' 
		''' Equation of a circle centered at (a,b) with radius r:
		'''     (x-a)^2 + (y-b)^2 = r^2
		''' Equation of a line with slope m and y-intercept b:
		'''     y = mx + b
		''' Replacing y in the circle equation and solving using the quadratic
		''' formula produces the following set of equations.  Constant factors have
		''' been extracted out of the inner loop.
		''' </summary>
		private  Sub  cyclicCircularGradientFillRaster(Integer pixels() , Integer off, Integer adjust, Integer x, Integer y, Integer w, Integer h)
			' constant part of the C factor of the quadratic equation
			Dim constC As Double = -radiusSq + (centerX * centerX) + (centerY * centerY)

			' coefficients of the quadratic equation (Ax^2 + Bx + C = 0)
			Dim A, B, C As Double

			' slope and y-intercept of the focus-perimeter line
			Dim slope, yintcpt As Double

			' intersection with circle X,Y coordinate
			Dim solutionX, solutionY As Double

			' constant parts of X, Y coordinates
			Dim constX As Single = (a00*x) + (a01*y) + a02
			Dim constY As Single = (a10*x) + (a11*y) + a12

			' constants in inner loop quadratic formula
			Dim precalc2 As Single = 2 * centerY
			Dim precalc3 As Single = -2 * centerX

			' value between 0 and 1 specifying position in the gradient
			Dim g As Single

			' determinant of quadratic formula (should always be > 0)
			Dim det As Single

			' sq distance from the current point to focus
			Dim currentToFocusSq As Single

			' sq distance from the intersect point to focus
			Dim intersectToFocusSq As Single

			' temp variables for change in X,Y squared
			Dim deltaXSq, deltaYSq As Single

			' used to index pixels array
			Dim indexer As Integer = off

			' incremental index change for pixels array
			Dim pixInc As Integer = w+adjust

			' for every row
			For j As Integer = 0 To h - 1

				' user space point; these are constant from column to column
				Dim X As Single = (a01*j) + constX
				Dim Y As Single = (a11*j) + constY

				' for every column (inner loop begins here)
				For i As Integer = 0 To w - 1

					If X = focusX Then
						' special case to avoid divide by zero
						solutionX = focusX
						solutionY = centerY
						solutionY += If(Y > focusY, trivial, -trivial)
					Else
						' slope and y-intercept of the focus-perimeter line
						slope = (Y - focusY) / (X - focusX)
						yintcpt = Y - (slope * X)

						' use the quadratic formula to calculate the
						' intersection point
						A = (slope * slope) + 1
						B = precalc3 + (-2 * slope * (centerY - yintcpt))
						C = constC + (yintcpt* (yintcpt - precalc2))

						det = CSng (System.Math.Sqrt((B * B) - (4 * A * C)))
						solutionX = -B

						' choose the positive or negative root depending
						' on where the X coord lies with respect to the focus
						solutionX += If(X < focusX, -det, det)
						solutionX = solutionX / (2 * A) ' divisor
						solutionY = (slope * solutionX) + yintcpt
					End If

					' Calculate the square of the distance from the current point
					' to the focus and the square of the distance from the
					' intersection point to the focus. Want the squares so we can
					' do 1 square root after division instead of 2 before.

					deltaXSq = X - focusX
					deltaXSq = deltaXSq * deltaXSq

					deltaYSq = Y - focusY
					deltaYSq = deltaYSq * deltaYSq

					currentToFocusSq = deltaXSq + deltaYSq

					deltaXSq = CSng(solutionX) - focusX
					deltaXSq = deltaXSq * deltaXSq

					deltaYSq = CSng(solutionY) - focusY
					deltaYSq = deltaYSq * deltaYSq

					intersectToFocusSq = deltaXSq + deltaYSq

					' get the percentage (0-1) of the current point along the
					' focus-circumference line
					g = CSng (System.Math.Sqrt(currentToFocusSq / intersectToFocusSq))

					' store the color at this point
					pixels(indexer + i) = indexIntoGradientsArrays(g)

					' incremental change in X, Y
					X += a00
					Y += a10
				Next i 'end inner loop

				indexer += pixInc
			Next j 'end outer loop
	End Class

End Namespace