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
	''' The {@code LinearGradientPaint} class provides a way to fill
	''' a <seealso cref="java.awt.Shape"/> with a linear color gradient pattern.  The user
	''' may specify two or more gradient colors, and this paint will provide an
	''' interpolation between each color.  The user also specifies start and end
	''' points which define where in user space the color gradient should begin
	''' and end.
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
	''' The user may also select what action the {@code LinearGradientPaint} object
	''' takes when it is filling the space outside the start and end points by
	''' setting {@code CycleMethod} to either {@code REFLECTION} or {@code REPEAT}.
	''' The distances between any two colors in any of the reflected or repeated
	''' copies of the gradient are the same as the distance between those same two
	''' colors between the start and end points.
	''' Note that some minor variations in distances may occur due to sampling at
	''' the granularity of a pixel.
	''' If no cycle method is specified, {@code NO_CYCLE} will be chosen by
	''' default, which means the endpoint colors will be used to fill the
	''' remaining area.
	''' <p>
	''' The colorSpace parameter allows the user to specify in which colorspace
	''' the interpolation should be performed, default sRGB or linearized RGB.
	''' 
	''' <p>
	''' The following code demonstrates typical usage of
	''' {@code LinearGradientPaint}:
	''' <pre>
	'''     Point2D start = new Point2D.Float(0, 0);
	'''     Point2D end = new Point2D.Float(50, 50);
	'''     float[] dist = {0.0f, 0.2f, 1.0f};
	'''     Color[] colors = {Color.RED, Color.WHITE, Color.BLUE};
	'''     LinearGradientPaint p =
	'''         new LinearGradientPaint(start, end, dist, colors);
	''' </pre>
	''' <p>
	''' This code will create a {@code LinearGradientPaint} which interpolates
	''' between red and white for the first 20% of the gradient and between white
	''' and blue for the remaining 80%.
	''' 
	''' <p>
	''' This image demonstrates the example code above for each
	''' of the three cycle methods:
	''' <center>
	''' <img src = "doc-files/LinearGradientPaint.png"
	''' alt="image showing the output of the example code">
	''' </center>
	''' </summary>
	''' <seealso cref= java.awt.Paint </seealso>
	''' <seealso cref= java.awt.Graphics2D#setPaint
	''' @author Nicholas Talian, Vincent Hardy, Jim Graham, Jerry Evans
	''' @since 1.6 </seealso>
	Public NotInheritable Class LinearGradientPaint
		Inherits MultipleGradientPaint

		''' <summary>
		''' Gradient start and end points. </summary>
		Private ReadOnly start, [end] As java.awt.geom.Point2D

		''' <summary>
		''' Constructs a {@code LinearGradientPaint} with a default
		''' {@code NO_CYCLE} repeating method and {@code SRGB} color space.
		''' </summary>
		''' <param name="startX"> the X coordinate of the gradient axis start point
		'''               in user space </param>
		''' <param name="startY"> the Y coordinate of the gradient axis start point
		'''               in user space </param>
		''' <param name="endX">   the X coordinate of the gradient axis end point
		'''               in user space </param>
		''' <param name="endY">   the Y coordinate of the gradient axis end point
		'''               in user space </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null, </exception>
		''' <exception cref="IllegalArgumentException">
		''' if start and end points are the same points,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal startX As Single, ByVal startY As Single, ByVal endX As Single, ByVal endY As Single, ByVal fractions As Single(), ByVal colors As Color())
			Me.New(New java.awt.geom.Point2D.Float(startX, startY), New java.awt.geom.Point2D.Float(endX, endY), fractions, colors, CycleMethod.NO_CYCLE)
		End Sub

		''' <summary>
		''' Constructs a {@code LinearGradientPaint} with a default {@code SRGB}
		''' color space.
		''' </summary>
		''' <param name="startX"> the X coordinate of the gradient axis start point
		'''               in user space </param>
		''' <param name="startY"> the Y coordinate of the gradient axis start point
		'''               in user space </param>
		''' <param name="endX">   the X coordinate of the gradient axis end point
		'''               in user space </param>
		''' <param name="endY">   the Y coordinate of the gradient axis end point
		'''               in user space </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if start and end points are the same points,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal startX As Single, ByVal startY As Single, ByVal endX As Single, ByVal endY As Single, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(New java.awt.geom.Point2D.Float(startX, startY), New java.awt.geom.Point2D.Float(endX, endY), fractions, colors, cycleMethod)
		End Sub

		''' <summary>
		''' Constructs a {@code LinearGradientPaint} with a default
		''' {@code NO_CYCLE} repeating method and {@code SRGB} color space.
		''' </summary>
		''' <param name="start"> the gradient axis start {@code Point2D} in user space </param>
		''' <param name="end"> the gradient axis end {@code Point2D} in user space </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value
		''' </param>
		''' <exception cref="NullPointerException">
		''' if one of the points is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if start and end points are the same points,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal start As java.awt.geom.Point2D, ByVal [end] As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color())
			Me.New(start, [end], fractions, colors, CycleMethod.NO_CYCLE)
		End Sub

		''' <summary>
		''' Constructs a {@code LinearGradientPaint} with a default {@code SRGB}
		''' color space.
		''' </summary>
		''' <param name="start"> the gradient axis start {@code Point2D} in user space </param>
		''' <param name="end"> the gradient axis end {@code Point2D} in user space </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value </param>
		''' <param name="cycleMethod"> either {@code NO_CYCLE}, {@code REFLECT},
		'''                    or {@code REPEAT}
		''' </param>
		''' <exception cref="NullPointerException">
		''' if one of the points is null,
		''' or {@code fractions} array is null,
		''' or {@code colors} array is null,
		''' or {@code cycleMethod} is null </exception>
		''' <exception cref="IllegalArgumentException">
		''' if start and end points are the same points,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
		Public Sub New(ByVal start As java.awt.geom.Point2D, ByVal [end] As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod)
			Me.New(start, [end], fractions, colors, cycleMethod, ColorSpaceType.SRGB, New java.awt.geom.AffineTransform)
		End Sub

		''' <summary>
		''' Constructs a {@code LinearGradientPaint}.
		''' </summary>
		''' <param name="start"> the gradient axis start {@code Point2D} in user space </param>
		''' <param name="end"> the gradient axis end {@code Point2D} in user space </param>
		''' <param name="fractions"> numbers ranging from 0.0 to 1.0 specifying the
		'''                  distribution of colors along the gradient </param>
		''' <param name="colors"> array of colors corresponding to each fractional value </param>
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
		''' if start and end points are the same points,
		''' or {@code fractions.length != colors.length},
		''' or {@code colors} is less than 2 in size,
		''' or a {@code fractions} value is less than 0.0 or greater than 1.0,
		''' or the {@code fractions} are not provided in strictly increasing order </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal start As java.awt.geom.Point2D, ByVal [end] As java.awt.geom.Point2D, ByVal fractions As Single(), ByVal colors As Color(), ByVal cycleMethod As CycleMethod, ByVal colorSpace As ColorSpaceType, ByVal gradientTransform As java.awt.geom.AffineTransform)
			MyBase.New(fractions, colors, cycleMethod, colorSpace, gradientTransform)

			' check input parameters
			If start Is Nothing OrElse [end] Is Nothing Then Throw New NullPointerException("Start and end points must be" & "non-null")

			If start.Equals([end]) Then Throw New IllegalArgumentException("Start point cannot equal" & "endpoint")

			' copy the points...
			Me.start = New java.awt.geom.Point2D.Double(start.x, start.y)
			Me.end = New java.awt.geom.Point2D.Double([end].x, [end].y)
		End Sub

		''' <summary>
		''' Creates and returns a <seealso cref="PaintContext"/> used to
		''' generate a linear color gradient pattern.
		''' See the <seealso cref="Paint#createContext specification"/> of the
		''' method in the <seealso cref="Paint"/> interface for information
		''' on null parameter handling.
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

			If (fractions.Length = 2) AndAlso (cycleMethod <> CycleMethod.REPEAT) AndAlso (colorSpace = ColorSpaceType.SRGB) Then
				' faster to use the basic GradientPaintContext for this
				' common case
				Dim cyclic As Boolean = (cycleMethod <> CycleMethod.NO_CYCLE)
				Return New GradientPaintContext(cm, start, [end], transform, colors(0), colors(1), cyclic)
			Else
				Return New LinearGradientPaintContext(Me, cm, deviceBounds, userBounds, transform, hints, start, [end], fractions, colors, cycleMethod, colorSpace)
			End If
		End Function

		''' <summary>
		''' Returns a copy of the start point of the gradient axis.
		''' </summary>
		''' <returns> a {@code Point2D} object that is a copy of the point
		''' that anchors the first color of this {@code LinearGradientPaint} </returns>
		Public Property startPoint As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Double(start.x, start.y)
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the end point of the gradient axis.
		''' </summary>
		''' <returns> a {@code Point2D} object that is a copy of the point
		''' that anchors the last color of this {@code LinearGradientPaint} </returns>
		Public Property endPoint As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Double([end].x, [end].y)
			End Get
		End Property
	End Class

End Namespace