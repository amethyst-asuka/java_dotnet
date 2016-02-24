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
	''' Provides the actual implementation for the LinearGradientPaint.
	''' This is where the pixel processing is done.
	''' </summary>
	''' <seealso cref= java.awt.LinearGradientPaint </seealso>
	''' <seealso cref= java.awt.PaintContext </seealso>
	''' <seealso cref= java.awt.Paint
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans </seealso>
	Friend NotInheritable Class LinearGradientPaintContext
		Inherits MultipleGradientPaintContext

		''' <summary>
		''' The following invariants are used to process the gradient value from
		''' a device space coordinate, (X, Y):
		'''     g(X, Y) = dgdX*X + dgdY*Y + gc
		''' </summary>
		Private dgdX, dgdY, gc As Single

		''' <summary>
		''' Constructor for LinearGradientPaintContext.
		''' </summary>
		''' <param name="paint"> the {@code LinearGradientPaint} from which this context
		'''              is created </param>
		''' <param name="cm"> {@code ColorModel} that receives
		'''           the <code>Paint</code> data. This is used only as a hint. </param>
		''' <param name="deviceBounds"> the device space bounding box of the
		'''                     graphics primitive being rendered </param>
		''' <param name="userBounds"> the user space bounding box of the
		'''                   graphics primitive being rendered </param>
		''' <param name="t"> the {@code AffineTransform} from user
		'''          space into device space (gradientTransform should be
		'''          concatenated with this) </param>
		''' <param name="hints"> the hints that the context object uses to choose
		'''              between rendering alternatives </param>
		''' <param name="start"> gradient start point, in user space </param>
		''' <param name="end"> gradient end point, in user space </param>
		''' <param name="fractions"> the fractions specifying the gradient distribution </param>
		''' <param name="colors"> the gradient colors </param>
		''' <param name="cycleMethod"> either NO_CYCLE, REFLECT, or REPEAT </param>
		''' <param name="colorSpace"> which colorspace to use for interpolation,
		'''                   either SRGB or LINEAR_RGB </param>
		Friend Sub New(ByVal paint As LinearGradientPaint, ByVal cm As java.awt.image.ColorModel, ByVal deviceBounds As Rectangle, ByVal userBounds As java.awt.geom.Rectangle2D, ByVal t As java.awt.geom.AffineTransform, ByVal hints As RenderingHints, ByVal start As java.awt.geom.Point2D, ByVal [end] As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As java.awt.MultipleGradientPaint.CycleMethod, ByVal colorSpace As java.awt.MultipleGradientPaint.ColorSpaceType)
			MyBase.New(paint, cm, deviceBounds, userBounds, t, hints, fractions, colors, cycleMethod, colorSpace)

			' A given point in the raster should take on the same color as its
			' projection onto the gradient vector.
			' Thus, we want the projection of the current position vector
			' onto the gradient vector, then normalized with respect to the
			' length of the gradient vector, giving a value which can be mapped
			' into the range 0-1.
			'    projection =
			'        currentVector dot gradientVector / length(gradientVector)
			'    normalized = projection / length(gradientVector)

			Dim startx As Single = CSng(start.x)
			Dim starty As Single = CSng(start.y)
			Dim endx As Single = CSng([end].x)
			Dim endy As Single = CSng([end].y)

			Dim dx As Single = endx - startx ' change in x from start to end
			Dim dy As Single = endy - starty ' change in y from start to end
			Dim dSq As Single = dx*dx + dy*dy ' total distance squared

			' avoid repeated calculations by doing these divides once
			Dim constX As Single = dx/dSq
			Dim constY As Single = dy/dSq

			' incremental change along gradient for +x
			dgdX = a00*constX + a10*constY
			' incremental change along gradient for +y
			dgdY = a01*constX + a11*constY

			' constant, incorporates the translation components from the matrix
			gc = (a02-startx)*constX + (a12-starty)*constY
		End Sub

		''' <summary>
		''' Return a Raster containing the colors generated for the graphics
		''' operation.  This is where the area is filled with colors distributed
		''' linearly.
		''' </summary>
		''' <param name="x">,y,w,h the area in device space for which colors are
		''' generated. </param>
		Protected Friend Overrides Sub fillRaster(ByVal pixels As Integer(), ByVal [off] As Integer, ByVal adjust As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			' current value for row gradients
			Dim g As Single = 0

			' used to end iteration on rows
			Dim rowLimit As Integer = [off] + w

			' constant which can be pulled out of the inner loop
			Dim initConst As Single = (dgdX*x) + gc

			For i As Integer = 0 To h - 1 ' for every row

				' initialize current value to be start
				g = initConst + dgdY*(y+i)

				Do While [off] < rowLimit ' for every pixel in this row
					' get the color
					pixels([off]) = indexIntoGradientsArrays(g)
					[off] += 1

					' incremental change in g
					g += dgdX
				Loop

				' change in off from row to row
				[off] += adjust

				'rowlimit is width + offset
				rowLimit = [off] + w
			Next i
		End Sub
	End Class

End Namespace