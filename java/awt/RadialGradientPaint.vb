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
	''' The {@code RadialGradientPaint} class provides a way to fill a shape with
	''' a circular radial color gradient pattern. The user may specify 2 or more
	''' gradient colors, and this paint will provide an interpolation between
	''' each color.
	''' <p>
	''' The user must specify the circle controlling the gradient pattern,
	''' which is described by a center point and a radius.  The user can also
	''' specify a separate focus point within that circle, which controls the
	''' location of the first color of the gradient.  By default the focus is
	''' set to be the center of the circle.
	''' <p>
	''' This paint will map the first color of the gradient to the focus point,
	''' and the last color to the perimeter of the circle, interpolating
	''' smoothly for any in-between colors specified by the user.  Any line drawn
	''' from the focus point to the circumference will thus span all the gradient
	''' colors.
	''' <p>
	''' Specifying a focus point outside of the radius of the circle will cause
	''' the rings of the gradient pattern to be centered on the point just inside
	''' the edge of the circle in the direction of the focus point.
	''' The rendering will internally use this modified location as if it were
	''' the specified focus point.
	''' <p>
	''' The user must provide an array of floats specifying how to distribute the
	''' colors along the gradient.  These values should range from 0.0 to 1.0 and
	''' act like keyframes along the gradient (they mark where the gradient should
	''' be exactly a particular color).
	''' <p>
	''' In the event that the user does not set the first keyframe value equal
	''' to 0 and/or the last keyframe value equal to 1, keyframes will be created
	''' at these positions and the first and last colors will be replicated there.
	''' So, if a user specifies the following arrays to construct a gradient:<br>
	''' <pre>
	'''     {Color.BLUE, Color.RED}, {.3f, .7f}
	''' </pre>
	''' this will be converted to a gradient with the following keyframes:<br>
	''' <pre>
	'''     {Color.BLUE, Color.BLUE, Color.RED, Color.RED}, {0f, .3f, .7f, 1f}
	''' </pre>
	''' 
	''' <p>
	''' The user may also select what action the {@code RadialGradientPaint} object
	''' takes when it is filling the space outside the circle's radius by
	''' setting {@code CycleMethod} to either {@code REFLECTION} or {@code REPEAT}.
	''' The gradient color proportions are equal for any particular line drawn
	''' from the focus point. The following figure shows that the distance AB
	''' is equal to the distance BC, and the distance AD is equal to the distance DE.
	''' <center>
	''' <img src = "doc-files/RadialGradientPaint-3.png" alt="image showing the
	''' distance AB=BC, and AD=DE">
	''' </center>
	''' If the gradient and graphics rendering transforms are uniformly scaled and
	''' the user sets the focus so that it coincides with the center of the circle,
	''' the gradient color proportions are equal for any line drawn from the center.
	''' The following figure shows the distances AB, BC, AD, and DE. They are all equal.
	''' <center>
	''' <img src = "doc-files/RadialGradientPaint-4.png" alt="image showing the
	''' distance of AB, BC, AD, and DE are all equal">
	''' </center>
	''' Note that some minor variations in distances may occur due to sampling at
	''' the granularity of a pixel.
	''' If no cycle method is specified, {@code NO_CYCLE} will be chosen by
	''' default, which means the the last keyframe color will be used to fill the
	''' remaining area.
	''' <p>
	''' The colorSpace parameter allows the user to specify in which colorspace
	''' the interpolation should be performed, default sRGB or linearized RGB.
	''' 
	''' <p>
	''' The following code demonstrates typical usage of
	''' {@code RadialGradientPaint}, where the center and focus points are
	''' the same:
	''' <pre>
	'''     Point2D center = new Point2D.Float(50, 50);
	'''     float radius = 25;
	'''     float[] dist = {0.0f, 0.2f, 1.0f};
	'''     Color[] colors = {Color.RED, Color.WHITE, Color.BLUE};
	'''     RadialGradientPaint p =
	'''         new RadialGradientPaint(center, radius, dist, colors);
	''' </pre>
	''' 
	''' <p>
	''' This image demonstrates the example code above, with default
	''' (centered) focus for each of the three cycle methods:
	''' <center>
	''' <img src = "doc-files/RadialGradientPaint-1.png" alt="image showing the
	''' output of the sameple code">
	''' </center>
	''' 
	''' <p>
	''' It is also possible to specify a non-centered focus point, as
	''' in the following code:
	''' <pre>
	'''     Point2D center = new Point2D.Float(50, 50);
	'''     float radius = 25;
	'''     Point2D focus = new Point2D.Float(40, 40);
	'''     float[] dist = {0.0f, 0.2f, 1.0f};
	'''     Color[] colors = {Color.RED, Color.WHITE, Color.BLUE};
	'''     RadialGradientPaint p =
	'''         new RadialGradientPaint(center, radius, focus,
	'''                                 dist, colors,
	'''                                 CycleMethod.NO_CYCLE);
	''' </pre>
	''' 
	''' <p>
	''' This image demonstrates the previous example code, with non-centered
	''' focus for each of the three cycle methods:
	''' <center>
	''' <img src = "doc-files/RadialGradientPaint-2.png" alt="image showing the
	''' output of the sample code">
	''' </center>
	''' </summary>
	''' <seealso cref= java.awt.Paint </seealso>
	''' <seealso cref= java.awt.Graphics2D#setPaint
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	''' @since 1.6 </seealso>
	Public NotInheritable Class RadialGradientPaint
		Inherits MultipleGradientPaint

		''' <summary>
		''' Focus point which defines the 0% gradient stop X coordinate. </summary>
		Private ReadOnly focus As java.awt.geom.Point2D

		''' <summary>
		''' Center of the circle defining the 100% gradient stop X coordinate. </summary>
		Private ReadOnly center As java.awt.geom.Point2D

		''' <summary>
		''' Radius of the outermost circle defining the 100% gradient stop. </summary>
		Private ReadOnly radius As Single

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code NO_CYCLE} repeating method and {@code SRGB} color space,
		''' using the center as the focus point.
		''' </summary>
		''' <param name="cx"> the X coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="cy"> the Y coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle.
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal cx As Single, ByVal cy As Single, ByVal radius As Single, ByVal fractions As Single(), ByVal colors As Color())
			Me.New(cx, cy, radius, cx, cy, fractions, colors, CycleMethod.NO_CYCLE)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code NO_CYCLE} repeating method and {@code SRGB} color space,
		''' using the center as the focus point.
		''' </summary>
		''' <param name="center"> the center point, in user space, of the circle defining
		'''               the gradient </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle.
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code center} point is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal center As java.awt.geom.Point2D, ByVal radius As Single, ByVal fractions As Single(), ByVal colors As Color())
			Me.New(center, radius, center, fractions, colors, CycleMethod.NO_CYCLE)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code SRGB} color space, using the center as the focus point.
		''' </summary>
		''' <param name="cx"> the X coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="cy"> the Y coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal cx As Single, ByVal cy As Single, ByVal radius As Single, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(cx, cy, radius, cx, cy, fractions, colors, cycleMethod)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code SRGB} color space, using the center as the focus point.
		''' </summary>
		''' <param name="center"> the center point, in user space, of the circle defining
		'''               the gradient </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code center} point is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal center As java.awt.geom.Point2D, ByVal radius As Single, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(center, radius, center, fractions, colors, cycleMethod)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code SRGB} color space.
		''' </summary>
		''' <param name="cx"> the X coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="cy"> the Y coordinate in user space of the center point of the
		'''           circle defining the gradient.  The last color of the
		'''           gradient is mapped to the perimeter of this circle. </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="fx"> the X coordinate of the point in user space to which the
		'''           first color is mapped </param>
		''' <param name="fy"> the Y coordinate of the point in user space to which the
		'''           first color is mapped </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal cx As Single, ByVal cy As Single, ByVal radius As Single, ByVal fx As Single, ByVal fy As Single, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(New java.awt.geom.Point2D.Float(cx, cy), radius, New java.awt.geom.Point2D.Float(fx, fy), fractions, colors, cycleMethod)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code SRGB} color space.
		''' </summary>
		''' <param name="center"> the center point, in user space, of the circle defining
		'''               the gradient.  The last color of the gradient is mapped
		'''               to the perimeter of this circle. </param>
		''' <param name="radius"> the radius of the circle defining the extents of the color
		'''               gradient </param>
		''' <param name="focus"> the point in user space to which the first color is mapped </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient. The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if one of the points is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal center As java.awt.geom.Point2D, ByVal radius As Single, ByVal focus As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(center, radius, focus, fractions, colors, cycleMethod, ColorSpaceType.SRGB, New java.awt.geom.AffineTransform)
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint}.
		''' </summary>
		''' <param name="center"> the center point in user space of the circle defining the
		'''               gradient.  The last color of the gradient is mapped to
		'''               the perimeter of this circle. </param>
		''' <param name="radius"> the radius of the circle defining the extents of the
		'''               color gradient </param>
		''' <param name="focus"> the point in user space to which the first color is mapped </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT} </param>
		''' <param name="colorSpace"> which color space to use for interpolation,
		'''                   either {@code SRGB} or {@code LINEAR_RGB} </param>
		''' <param name="gradientTransform"> transform to apply to the gradient
		''' </param>
		''' <exception cref="NullPointerException">
		''' if one of the points is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null,
		''' or {@code colorSpace} is null,
		''' or {@code gradientTransform} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code radius} is non-positive,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal center As java.awt.geom.Point2D, ByVal radius As Single, ByVal focus As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod, ByVal colorSpace As ColorSpaceType, ByVal gradientTransform As java.awt.geom.AffineTransform)
			MyBase.New(fractions, colors, cycleMethod, colorSpace, gradientTransform)

			' check input arguments
			If center Is Nothing Then Throw New NullPointerException("Center point must be non-null")

			If focus Is Nothing Then Throw New NullPointerException("Focus point must be non-null")

			If radius <= 0 Then Throw New IllegalArgumentException("Radius must be greater " & "than zero")

			' copy parameters
			Me.center = New java.awt.geom.Point2D.Double(center.x, center.y)
			Me.focus = New java.awt.geom.Point2D.Double(focus.x, focus.y)
			Me.radius = radius
		End Sub

		''' <summary>
		''' Constructs a {@code RadialGradientPaint} with a default
		''' {@code SRGB} color space.
		''' The gradient circle of the {@code RadialGradientPaint} is defined
		''' by the given bounding box.
		''' <p>
		''' This constructor is a more convenient way to express the
		''' following (equivalent) code:<br>
		''' 
		''' <pre>
		'''     double gw = gradientBounds.getWidth();
		'''     double gh = gradientBounds.getHeight();
		'''     double cx = gradientBounds.getCenterX();
		'''     double cy = gradientBounds.getCenterY();
		'''     Point2D center = new Point2D.Double(cx, cy);
		''' 
		'''     AffineTransform gradientTransform = new AffineTransform();
		'''     gradientTransform.translate(cx, cy);
		'''     gradientTransform.scale(gw / 2, gh / 2);
		'''     gradientTransform.translate(-cx, -cy);
		''' 
		'''     RadialGradientPaint gp =
		'''         new RadialGradientPaint(center, 1.0f, center,
		'''                                 fractions, colors,
		'''                                 cycleMethod,
		'''                                 ColorSpaceType.SRGB,
		'''                                 gradientTransform);
		''' </pre>
		''' </summary>
		''' <param name="gradientBounds"> the bounding box, in user space, of the circle
		'''                       defining the outermost extent of the gradient </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors to use in the gradient.  The first color
		'''               is used at the focus point, the last color around the
		'''               perimeter of the circle. </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code gradientBounds} is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if {@code gradientBounds} is empty,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal gradientBounds As java.awt.geom.Rectangle2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			' gradient center/focal point is the center of the bounding box,
			' radius is set to 1.0, and then we set a scale transform
			' to achieve an elliptical gradient defined by the bounding box
			Me.New(New java.awt.geom.Point2D.Double(gradientBounds.centerX, gradientBounds.centerY), 1.0f, New java.awt.geom.Point2D.Double(gradientBounds.centerX, gradientBounds.centerY), fractions, colors, cycleMethod, ColorSpaceType.SRGB, createGradientTransform(gradientBounds))

			If gradientBounds.empty Then Throw New IllegalArgumentException("Gradient bounds must be " & "non-empty")
		End Sub

		Private Shared Function createGradientTransform(ByVal r As java.awt.geom.Rectangle2D) As java.awt.geom.AffineTransform
			Dim cx As Double = r.centerX
			Dim cy As Double = r.centerY
			Dim xform As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getTranslateInstance(cx, cy)
			xform.scale(r.width/2, r.height/2)
			xform.translate(-cx, -cy)
			Return xform
		End Function

		''' <summary>
		''' Creates and returns a <seealso cref="PaintContext"/> used to
		''' generate a circular radial color gradient pattern.
		''' See the description of the <seealso cref="Paint#createContext createContext"/> method
		''' for information on null parameter handling.
		''' </summary>
		''' <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		'''           format for the caller to receive the pixel data, or {@code null}
		'''           if there is no preference. </param>
		''' <param name="deviceBounds"> the device space bounding box
		'''                     of the graphics primitive being rendered. </param>
		''' <param name="userBounds"> the user space bounding box
		'''                   of the graphics primitive being rendered. </param>
		''' <param name="transform"> the <seealso cref="AffineTransform"/> from user
		'''              space into device space. </param>
		''' <param name="hints"> the set of hints that the context object can use to
		'''              choose between rendering alternatives. </param>
		''' <returns> the {@code PaintContext} for
		'''         generating color patterns. </returns>
		''' <seealso cref= Paint </seealso>
		''' <seealso cref= PaintContext </seealso>
		''' <seealso cref= ColorModel </seealso>
		''' <seealso cref= Rectangle </seealso>
		''' <seealso cref= Rectangle2D </seealso>
		''' <seealso cref= AffineTransform </seealso>
		''' <seealso cref= RenderingHints </seealso>
		Public Overrides Function createContext(ByVal cm As java.awt.image.ColorModel, ByVal deviceBounds As Rectangle, ByVal userBounds As java.awt.geom.Rectangle2D, ByVal transform As java.awt.geom.AffineTransform, ByVal hints As RenderingHints) As PaintContext
			' avoid modifying the user's transform...
			transform = New java.awt.geom.AffineTransform(transform)
			' incorporate the gradient transform
			transform.concatenate(gradientTransform)

			Return New RadialGradientPaintContext(Me, cm, deviceBounds, userBounds, transform, hints, CSng(center.x), CSng(center.y), radius, CSng(focus.x), CSng(focus.y), fractions, colors, cycleMethod, colorSpace)
		End Function

		''' <summary>
		''' Returns a copy of the center point of the radial gradient.
		''' </summary>
		''' <returns> a {@code Point2D} object that is a copy of the center point </returns>
		Public Property centerPoint As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Double(center.x, center.y)
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the focus point of the radial gradient.
		''' Note that if the focus point specified when the radial gradient
		''' was constructed lies outside of the radius of the circle, this
		''' method will still return the original focus point even though
		''' the rendering may center the rings of color on a different
		''' point that lies inside the radius.
		''' </summary>
		''' <returns> a {@code Point2D} object that is a copy of the focus point </returns>
		Public Property focusPoint As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Double(focus.x, focus.y)
			End Get
		End Property

		''' <summary>
		''' Returns the radius of the circle defining the radial gradient.
		''' </summary>
		''' <returns> the radius of the circle defining the radial gradient </returns>
		Public Property radius As Single
			Get
				Return radius
			End Get
		End Property
	End Class

End Namespace