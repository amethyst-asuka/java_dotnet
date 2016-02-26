Imports System
Imports javax.swing

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Convenient base class for defining Painter instances for rendering a
	''' region or component in Nimbus.
	''' 
	''' @author Jasper Potts
	''' @author Richard Bair
	''' </summary>
	Public MustInherit Class AbstractRegionPainter
		Implements javax.swing.Painter(Of JComponent)

		''' <summary>
		''' PaintContext, which holds a lot of the state needed for cache hinting and x/y value decoding
		''' The data contained within the context is typically only computed once and reused over
		''' multiple paint calls, whereas the other values (w, h, f, leftWidth, etc) are recomputed
		''' for each call to paint.
		''' 
		''' This field is retrieved from subclasses on each paint operation. It is up
		''' to the subclass to compute and cache the PaintContext over multiple calls.
		''' </summary>
		Private ctx As PaintContext
		''' <summary>
		''' The scaling factor. Recomputed on each call to paint.
		''' </summary>
		Private f As Single
	'    
	'      Various metrics used for decoding x/y values based on the canvas size
	'      and stretching insets.
	'
	'      On each call to paint, we first ask the subclass for the PaintContext.
	'      From the context we get the canvas size and stretching insets, and whether
	'      the algorithm should be "inverted", meaning the center section remains
	'      a fixed size and the other sections scale.
	'
	'      We then use these values to compute a series of metrics (listed below)
	'      which are used to decode points in a specific axis (x or y).
	'
	'      The leftWidth represents the distance from the left edge of the region
	'      to the first stretching inset, after accounting for any scaling factor
	'      (such as DPI scaling). The centerWidth is the distance between the leftWidth
	'      and the rightWidth. The rightWidth is the distance from the right edge,
	'      to the right inset (after scaling has been applied).
	'
	'      The same logic goes for topHeight, centerHeight, and bottomHeight.
	'
	'      The leftScale represents the proportion of the width taken by the left section.
	'      The same logic is applied to the other scales.
	'
	'      The various widths/heights are used to decode control points. The
	'      various scales are used to decode bezier handles (or anchors).
	'    
		''' <summary>
		''' The width of the left section. Recomputed on each call to paint.
		''' </summary>
		Private leftWidth As Single
		''' <summary>
		''' The height of the top section. Recomputed on each call to paint.
		''' </summary>
		Private topHeight As Single
		''' <summary>
		''' The width of the center section. Recomputed on each call to paint.
		''' </summary>
		Private centerWidth As Single
		''' <summary>
		''' The height of the center section. Recomputed on each call to paint.
		''' </summary>
		Private centerHeight As Single
		''' <summary>
		''' The width of the right section. Recomputed on each call to paint.
		''' </summary>
		Private rightWidth As Single
		''' <summary>
		''' The height of the bottom section. Recomputed on each call to paint.
		''' </summary>
		Private bottomHeight As Single
		''' <summary>
		''' The scaling factor to use for the left section. Recomputed on each call to paint.
		''' </summary>
		Private leftScale As Single
		''' <summary>
		''' The scaling factor to use for the top section. Recomputed on each call to paint.
		''' </summary>
		Private topScale As Single
		''' <summary>
		''' The scaling factor to use for the center section, in the horizontal
		''' direction. Recomputed on each call to paint.
		''' </summary>
		Private centerHScale As Single
		''' <summary>
		''' The scaling factor to use for the center section, in the vertical
		''' direction. Recomputed on each call to paint.
		''' </summary>
		Private centerVScale As Single
		''' <summary>
		''' The scaling factor to use for the right section. Recomputed on each call to paint.
		''' </summary>
		Private rightScale As Single
		''' <summary>
		''' The scaling factor to use for the bottom section. Recomputed on each call to paint.
		''' </summary>
		Private bottomScale As Single

		''' <summary>
		''' Create a new AbstractRegionPainter
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paint(ByVal g As Graphics2D, ByVal c As JComponent, ByVal w As Integer, ByVal h As Integer)
			'don't render if the width/height are too small
			If w <= 0 OrElse h <=0 Then Return

			Dim ___extendedCacheKeys As Object() = getExtendedCacheKeys(c)
			ctx = paintContext
			Dim cacheMode As PaintContext.CacheMode = If(ctx Is Nothing, PaintContext.CacheMode.NO_CACHING, ctx.cacheMode)
			If cacheMode = PaintContext.CacheMode.NO_CACHING OrElse (Not ImageCache.instance.isImageCachable(w, h)) OrElse TypeOf g Is java.awt.print.PrinterGraphics Then
				' no caching so paint directly
				paint0(g, c, w, h, ___extendedCacheKeys)
			ElseIf cacheMode = PaintContext.CacheMode.FIXED_SIZES Then
				paintWithFixedSizeCaching(g, c, w, h, ___extendedCacheKeys)
			Else
				' 9 Square caching
				paintWith9SquareCaching(g, ctx, c, w, h, ___extendedCacheKeys)
			End If
		End Sub

		''' <summary>
		''' Get any extra attributes which the painter implementation would like
		''' to include in the image cache lookups. This is checked for every call
		''' of the paint(g, c, w, h) method.
		''' </summary>
		''' <param name="c"> The component on the current paint call </param>
		''' <returns> Array of extra objects to be included in the cache key </returns>
		Protected Friend Overridable Function getExtendedCacheKeys(ByVal c As JComponent) As Object()
			Return Nothing
		End Function

		''' <summary>
		''' <p>Gets the PaintContext for this painting operation. This method is called on every
		''' paint, and so should be fast and produce no garbage. The PaintContext contains
		''' information such as cache hints. It also contains data necessary for decoding
		''' points at runtime, such as the stretching insets, the canvas size at which the
		''' encoded points were defined, and whether the stretching insets are inverted.</p>
		''' 
		''' <p> This method allows for subclasses to package the painting of different states
		''' with possibly different canvas sizes, etc, into one AbstractRegionPainter implementation.</p>
		''' </summary>
		''' <returns> a PaintContext associated with this paint operation. </returns>
		Protected Friend MustOverride ReadOnly Property paintContext As PaintContext

		''' <summary>
		''' <p>Configures the given Graphics2D. Often, rendering hints or compositing rules are
		''' applied to a Graphics2D object prior to painting, which should affect all of the
		''' subsequent painting operations. This method provides a convenient hook for configuring
		''' the Graphics object prior to rendering, regardless of whether the render operation is
		''' performed to an intermediate buffer or directly to the display.</p>
		''' </summary>
		''' <param name="g"> The Graphics2D object to configure. Will not be null. </param>
		Protected Friend Overridable Sub configureGraphics(ByVal g As Graphics2D)
			g.renderingHintint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON)
		End Sub

		''' <summary>
		''' Actually performs the painting operation. Subclasses must implement this method.
		''' The graphics object passed may represent the actual surface being rendered to,
		''' or it may be an intermediate buffer. It has also been pre-translated. Simply render
		''' the component as if it were located at 0, 0 and had a width of <code>width</code>
		''' and a height of <code>height</code>. For performance reasons, you may want to read
		''' the clip from the Graphics2D object and only render within that space.
		''' </summary>
		''' <param name="g"> The Graphics2D surface to paint to </param>
		''' <param name="c"> The JComponent related to the drawing event. For example, if the
		'''          region being rendered is Button, then <code>c</code> will be a
		'''          JButton. If the region being drawn is ScrollBarSlider, then the
		'''          component will be JScrollBar. This value may be null. </param>
		''' <param name="width"> The width of the region to paint. Note that in the case of
		'''              painting the foreground, this value may differ from c.getWidth(). </param>
		''' <param name="height"> The height of the region to paint. Note that in the case of
		'''               painting the foreground, this value may differ from c.getHeight(). </param>
		''' <param name="extendedCacheKeys"> The result of the call to getExtendedCacheKeys() </param>
		Protected Friend MustOverride Sub doPaint(ByVal g As Graphics2D, ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())

		''' <summary>
		''' Decodes and returns a float value representing the actual pixel location for
		''' the given encoded X value.
		''' </summary>
		''' <param name="x"> an encoded x value (0...1, or 1...2, or 2...3) </param>
		''' <returns> the decoded x value </returns>
		''' <exception cref="IllegalArgumentException">
		'''      if {@code x < 0} or {@code x > 3} </exception>
		Protected Friend Function decodeX(ByVal x As Single) As Single
			If x >= 0 AndAlso x <= 1 Then
				Return x * leftWidth
			ElseIf x > 1 AndAlso x < 2 Then
				Return ((x-1) * centerWidth) + leftWidth
			ElseIf x >= 2 AndAlso x <= 3 Then
				Return ((x-2) * rightWidth) + leftWidth + centerWidth
			Else
				Throw New System.ArgumentException("Invalid x")
			End If
		End Function

		''' <summary>
		''' Decodes and returns a float value representing the actual pixel location for
		''' the given encoded y value.
		''' </summary>
		''' <param name="y"> an encoded y value (0...1, or 1...2, or 2...3) </param>
		''' <returns> the decoded y value </returns>
		''' <exception cref="IllegalArgumentException">
		'''      if {@code y < 0} or {@code y > 3} </exception>
		Protected Friend Function decodeY(ByVal y As Single) As Single
			If y >= 0 AndAlso y <= 1 Then
				Return y * topHeight
			ElseIf y > 1 AndAlso y < 2 Then
				Return ((y-1) * centerHeight) + topHeight
			ElseIf y >= 2 AndAlso y <= 3 Then
				Return ((y-2) * bottomHeight) + topHeight + centerHeight
			Else
				Throw New System.ArgumentException("Invalid y")
			End If
		End Function

		''' <summary>
		''' Decodes and returns a float value representing the actual pixel location for
		''' the anchor point given the encoded X value of the control point, and the offset
		''' distance to the anchor from that control point.
		''' </summary>
		''' <param name="x"> an encoded x value of the bezier control point (0...1, or 1...2, or 2...3) </param>
		''' <param name="dx"> the offset distance to the anchor from the control point x </param>
		''' <returns> the decoded x location of the control point </returns>
		''' <exception cref="IllegalArgumentException">
		'''      if {@code x < 0} or {@code x > 3} </exception>
		Protected Friend Function decodeAnchorX(ByVal x As Single, ByVal dx As Single) As Single
			If x >= 0 AndAlso x <= 1 Then
				Return decodeX(x) + (dx * leftScale)
			ElseIf x > 1 AndAlso x < 2 Then
				Return decodeX(x) + (dx * centerHScale)
			ElseIf x >= 2 AndAlso x <= 3 Then
				Return decodeX(x) + (dx * rightScale)
			Else
				Throw New System.ArgumentException("Invalid x")
			End If
		End Function

		''' <summary>
		''' Decodes and returns a float value representing the actual pixel location for
		''' the anchor point given the encoded Y value of the control point, and the offset
		''' distance to the anchor from that control point.
		''' </summary>
		''' <param name="y"> an encoded y value of the bezier control point (0...1, or 1...2, or 2...3) </param>
		''' <param name="dy"> the offset distance to the anchor from the control point y </param>
		''' <returns> the decoded y position of the control point </returns>
		''' <exception cref="IllegalArgumentException">
		'''      if {@code y < 0} or {@code y > 3} </exception>
		Protected Friend Function decodeAnchorY(ByVal y As Single, ByVal dy As Single) As Single
			If y >= 0 AndAlso y <= 1 Then
				Return decodeY(y) + (dy * topScale)
			ElseIf y > 1 AndAlso y < 2 Then
				Return decodeY(y) + (dy * centerVScale)
			ElseIf y >= 2 AndAlso y <= 3 Then
				Return decodeY(y) + (dy * bottomScale)
			Else
				Throw New System.ArgumentException("Invalid y")
			End If
		End Function

		''' <summary>
		''' Decodes and returns a color, which is derived from a base color in UI
		''' defaults.
		''' </summary>
		''' <param name="key">     A key corresponding to the value in the UI Defaults table
		'''                of UIManager where the base color is defined </param>
		''' <param name="hOffset"> The hue offset used for derivation. </param>
		''' <param name="sOffset"> The saturation offset used for derivation. </param>
		''' <param name="bOffset"> The brightness offset used for derivation. </param>
		''' <param name="aOffset"> The alpha offset used for derivation. Between 0...255 </param>
		''' <returns> The derived color, whose color value will change if the parent
		'''         uiDefault color changes. </returns>
		Protected Friend Function decodeColor(ByVal key As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer) As Color
			If TypeOf UIManager.lookAndFeel Is NimbusLookAndFeel Then
				Dim laf As NimbusLookAndFeel = CType(UIManager.lookAndFeel, NimbusLookAndFeel)
				Return laf.getDerivedColor(key, hOffset, sOffset, bOffset, aOffset, True)
			Else
				' can not give a right answer as painter sould not be used outside
				' of nimbus laf but do the best we can
				Return Color.getHSBColor(hOffset,sOffset,bOffset)
			End If
		End Function

		''' <summary>
		''' Decodes and returns a color, which is derived from a offset between two
		''' other colors.
		''' </summary>
		''' <param name="color1">   The first color </param>
		''' <param name="color2">   The second color </param>
		''' <param name="midPoint"> The offset between color 1 and color 2, a value of 0.0 is
		'''                 color 1 and 1.0 is color 2; </param>
		''' <returns> The derived color </returns>
		Protected Friend Function decodeColor(ByVal color1 As Color, ByVal color2 As Color, ByVal midPoint As Single) As Color
			Return New Color(NimbusLookAndFeel.deriveARGB(color1, color2, midPoint))
		End Function

		''' <summary>
		''' Given parameters for creating a LinearGradientPaint, this method will
		''' create and return a linear gradient paint. One primary purpose for this
		''' method is to avoid creating a LinearGradientPaint where the start and
		''' end points are equal. In such a case, the end y point is slightly
		''' increased to avoid the overlap.
		''' </summary>
		''' <param name="x1"> </param>
		''' <param name="y1"> </param>
		''' <param name="x2"> </param>
		''' <param name="y2"> </param>
		''' <param name="midpoints"> </param>
		''' <param name="colors"> </param>
		''' <returns> a valid LinearGradientPaint. This method never returns null. </returns>
		''' <exception cref="NullPointerException">
		'''      if {@code midpoints} array is null,
		'''      or {@code colors} array is null, </exception>
		''' <exception cref="IllegalArgumentException">
		'''      if start and end points are the same points,
		'''      or {@code midpoints.length != colors.length},
		'''      or {@code colors} is less than 2 in size,
		'''      or a {@code midpoints} value is less than 0.0 or greater than 1.0,
		'''      or the {@code midpoints} are not provided in strictly increasing order </exception>
		Protected Friend Function decodeGradient(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal midpoints As Single(), ByVal colors As Color()) As LinearGradientPaint
			If x1 = x2 AndAlso y1 = y2 Then y2 +=.00001f
			Return New LinearGradientPaint(x1, y1, x2, y2, midpoints, colors)
		End Function

		''' <summary>
		''' Given parameters for creating a RadialGradientPaint, this method will
		''' create and return a radial gradient paint. One primary purpose for this
		''' method is to avoid creating a RadialGradientPaint where the radius
		''' is non-positive. In such a case, the radius is just slightly
		''' increased to avoid 0.
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		''' <param name="r"> </param>
		''' <param name="midpoints"> </param>
		''' <param name="colors"> </param>
		''' <returns> a valid RadialGradientPaint. This method never returns null. </returns>
		''' <exception cref="NullPointerException">
		'''      if {@code midpoints} array is null,
		'''      or {@code colors} array is null </exception>
		''' <exception cref="IllegalArgumentException">
		'''      if {@code r} is non-positive,
		'''      or {@code midpoints.length != colors.length},
		'''      or {@code colors} is less than 2 in size,
		'''      or a {@code midpoints} value is less than 0.0 or greater than 1.0,
		'''      or the {@code midpoints} are not provided in strictly increasing order </exception>
		Protected Friend Function decodeRadialGradient(ByVal x As Single, ByVal y As Single, ByVal r As Single, ByVal midpoints As Single(), ByVal colors As Color()) As RadialGradientPaint
			If r = 0f Then r =.00001f
			Return New RadialGradientPaint(x, y, r, midpoints, colors)
		End Function

		''' <summary>
		''' Get a color property from the given JComponent. First checks for a
		''' <code>getXXX()</code> method and if that fails checks for a client
		''' property with key <code>property</code>. If that still fails to return
		''' a Color then <code>defaultColor</code> is returned.
		''' </summary>
		''' <param name="c"> The component to get the color property from </param>
		''' <param name="property"> The name of a bean style property or client property </param>
		''' <param name="defaultColor"> The color to return if no color was obtained from
		'''        the component. </param>
		''' <returns> The color that was obtained from the component or defaultColor </returns>
		Protected Friend Function getComponentColor(ByVal c As JComponent, ByVal [property] As String, ByVal defaultColor As Color, ByVal saturationOffset As Single, ByVal brightnessOffset As Single, ByVal alphaOffset As Integer) As Color
			Dim color As Color = Nothing
			If c IsNot Nothing Then
				' handle some special cases for performance
				If "background".Equals([property]) Then
					color = c.background
				ElseIf "foreground".Equals([property]) Then
					color = c.foreground
				ElseIf TypeOf c Is JList AndAlso "selectionForeground".Equals([property]) Then
					color = CType(c, JList).selectionForeground
				ElseIf TypeOf c Is JList AndAlso "selectionBackground".Equals([property]) Then
					color = CType(c, JList).selectionBackground
				ElseIf TypeOf c Is JTable AndAlso "selectionForeground".Equals([property]) Then
					color = CType(c, JTable).selectionForeground
				ElseIf TypeOf c Is JTable AndAlso "selectionBackground".Equals([property]) Then
					color = CType(c, JTable).selectionBackground
				Else
					Dim s As String = "get" & Char.ToUpper([property].Chars(0)) + [property].Substring(1)
					Try
						Dim method As Method = sun.reflect.misc.MethodUtil.getMethod(c.GetType(), s, Nothing)
						color = CType(sun.reflect.misc.MethodUtil.invoke(method, c, Nothing), Color)
					Catch e As Exception
						'don't do anything, it just didn't work, that's all.
						'This could be a normal occurance if you use a property
						'name referring to a key in clientProperties instead of
						'a real property
					End Try
					If color Is Nothing Then
						Dim value As Object = c.getClientProperty([property])
						If TypeOf value Is Color Then color = CType(value, Color)
					End If
				End If
			End If
			' we return the defaultColor if the color found is null, or if
			' it is a UIResource. This is done because the color for the
			' ENABLED state is set on the component, but you don't want to use
			' that color for the over state. So we only respect the color
			' specified for the property if it was set by the user, as opposed
			' to set by us.
			If color Is Nothing OrElse TypeOf color Is javax.swing.plaf.UIResource Then
				Return defaultColor
			ElseIf saturationOffset <> 0 OrElse brightnessOffset <> 0 OrElse alphaOffset <> 0 Then
				Dim tmp As Single() = Color.RGBtoHSB(color.red, color.green, color.blue, Nothing)
				tmp(1) = clamp(tmp(1) + saturationOffset)
				tmp(2) = clamp(tmp(2) + brightnessOffset)
				Dim alpha As Integer = clamp(color.alpha + alphaOffset)
				Return New Color((Color.HSBtoRGB(tmp(0), tmp(1), tmp(2)) And &HFFFFFF) Or (alpha <<24))
			Else
				Return color
			End If
		End Function

		''' <summary>
		''' A class encapsulating state useful when painting. Generally, instances of this
		''' class are created once, and reused for each paint request without modification.
		''' This class contains values useful when hinting the cache engine, and when decoding
		''' control points and bezier curve anchors.
		''' </summary>
		Protected Friend Class PaintContext
			Protected Friend Enum CacheMode
				NO_CACHING
				FIXED_SIZES
				NINE_SQUARE_SCALE
			End Enum

			Private Shared EMPTY_INSETS As New Insets(0, 0, 0, 0)

			Private stretchingInsets As Insets
			Private canvasSize As Dimension
			Private inverted As Boolean
			Private cacheMode As CacheMode
			Private maxHorizontalScaleFactor As Double
			Private maxVerticalScaleFactor As Double

			Private a As Single ' insets.left
			Private b As Single ' canvasSize.width - insets.right
			Private c As Single ' insets.top
			Private d As Single ' canvasSize.height - insets.bottom;
			Private aPercent As Single ' only used if inverted == true
			Private bPercent As Single ' only used if inverted == true
			Private cPercent As Single ' only used if inverted == true
			Private dPercent As Single ' only used if inverted == true

			''' <summary>
			''' Creates a new PaintContext which does not attempt to cache or scale any cached
			''' images.
			''' </summary>
			''' <param name="insets"> The stretching insets. May be null. If null, then assumed to be 0, 0, 0, 0. </param>
			''' <param name="canvasSize"> The size of the canvas used when encoding the various x/y values. May be null.
			'''                   If null, then it is assumed that there are no encoded values, and any calls
			'''                   to one of the "decode" methods will return the passed in value. </param>
			''' <param name="inverted"> Whether to "invert" the meaning of the 9-square grid and stretching insets </param>
			Public Sub New(ByVal insets As Insets, ByVal canvasSize As Dimension, ByVal inverted As Boolean)
				Me.New(insets, canvasSize, inverted, Nothing, 1, 1)
			End Sub

			''' <summary>
			''' Creates a new PaintContext.
			''' </summary>
			''' <param name="insets"> The stretching insets. May be null. If null, then assumed to be 0, 0, 0, 0. </param>
			''' <param name="canvasSize"> The size of the canvas used when encoding the various x/y values. May be null.
			'''                   If null, then it is assumed that there are no encoded values, and any calls
			'''                   to one of the "decode" methods will return the passed in value. </param>
			''' <param name="inverted"> Whether to "invert" the meaning of the 9-square grid and stretching insets </param>
			''' <param name="cacheMode"> A hint as to which caching mode to use. If null, then set to no caching. </param>
			''' <param name="maxH"> The maximum scale in the horizontal direction to use before punting and redrawing from scratch.
			'''             For example, if maxH is 2, then we will attempt to scale any cached images up to 2x the canvas
			'''             width before redrawing from scratch. Reasonable maxH values may improve painting performance.
			'''             If set too high, then you may get poor looking graphics at higher zoom levels. Must be &gt;= 1. </param>
			''' <param name="maxV"> The maximum scale in the vertical direction to use before punting and redrawing from scratch.
			'''             For example, if maxV is 2, then we will attempt to scale any cached images up to 2x the canvas
			'''             height before redrawing from scratch. Reasonable maxV values may improve painting performance.
			'''             If set too high, then you may get poor looking graphics at higher zoom levels. Must be &gt;= 1. </param>
			Public Sub New(ByVal insets As Insets, ByVal canvasSize As Dimension, ByVal inverted As Boolean, ByVal cacheMode As CacheMode, ByVal maxH As Double, ByVal maxV As Double)
				If maxH < 1 OrElse maxH < 1 Then Throw New System.ArgumentException("Both maxH and maxV must be >= 1")

				Me.stretchingInsets = If(insets Is Nothing, EMPTY_INSETS, insets)
				Me.canvasSize = canvasSize
				Me.inverted = inverted
				Me.cacheMode = If(cacheMode Is Nothing, CacheMode.NO_CACHING, cacheMode)
				Me.maxHorizontalScaleFactor = maxH
				Me.maxVerticalScaleFactor = maxV

				If canvasSize IsNot Nothing Then
					a = stretchingInsets.left
					b = canvasSize.width - stretchingInsets.right
					c = stretchingInsets.top
					d = canvasSize.height - stretchingInsets.bottom
					Me.canvasSize = canvasSize
					Me.inverted = inverted
					If inverted Then
						Dim available As Single = canvasSize.width - (b - a)
						aPercent = If(available > 0f, a / available, 0f)
						bPercent = If(available > 0f, b / available, 0f)
						available = canvasSize.height - (d - c)
						cPercent = If(available > 0f, c / available, 0f)
						dPercent = If(available > 0f, d / available, 0f)
					End If
				End If
			End Sub
		End Class

		'---------------------- private methods

		'initializes the class to prepare it for being able to decode points
		Private Sub prepare(ByVal w As Single, ByVal h As Single)
			'if no PaintContext has been specified, reset the values and bail
			'also bail if the canvasSize was not set (since decoding will not work)
			If ctx Is Nothing OrElse ctx.canvasSize Is Nothing Then
				f = 1f
					rightWidth = 0f
						centerWidth = rightWidth
						leftWidth = centerWidth
					bottomHeight = 0f
						centerHeight = bottomHeight
						topHeight = centerHeight
					rightScale = 0f
						centerHScale = rightScale
						leftScale = centerHScale
					bottomScale = 0f
						centerVScale = bottomScale
						topScale = centerVScale
				Return
			End If

			'calculate the scaling factor, and the sizes for the various 9-square sections
			Dim scale As Number = CType(UIManager.get("scale"), Number)
			f = If(scale Is Nothing, 1f, scale)

			If ctx.inverted Then
				centerWidth = (ctx.b - ctx.a) * f
				Dim availableSpace As Single = w - centerWidth
				leftWidth = availableSpace * ctx.aPercent
				rightWidth = availableSpace * ctx.bPercent
				centerHeight = (ctx.d - ctx.c) * f
				availableSpace = h - centerHeight
				topHeight = availableSpace * ctx.cPercent
				bottomHeight = availableSpace * ctx.dPercent
			Else
				leftWidth = ctx.a * f
				rightWidth = CSng(ctx.canvasSize.width - ctx.b) * f
				centerWidth = w - leftWidth - rightWidth
				topHeight = ctx.c * f
				bottomHeight = CSng(ctx.canvasSize.height - ctx.d) * f
				centerHeight = h - topHeight - bottomHeight
			End If

			leftScale = If(ctx.a = 0f, 0f, leftWidth / ctx.a)
			centerHScale = If((ctx.b - ctx.a) = 0f, 0f, centerWidth / (ctx.b - ctx.a))
			rightScale = If((ctx.canvasSize.width - ctx.b) = 0f, 0f, rightWidth / (ctx.canvasSize.width - ctx.b))
			topScale = If(ctx.c = 0f, 0f, topHeight / ctx.c)
			centerVScale = If((ctx.d - ctx.c) = 0f, 0f, centerHeight / (ctx.d - ctx.c))
			bottomScale = If((ctx.canvasSize.height - ctx.d) = 0f, 0f, bottomHeight / (ctx.canvasSize.height - ctx.d))
		End Sub

		Private Sub paintWith9SquareCaching(ByVal g As Graphics2D, ByVal ctx As PaintContext, ByVal c As JComponent, ByVal w As Integer, ByVal h As Integer, ByVal extendedCacheKeys As Object())
			' check if we can scale to the requested size
			Dim canvas As Dimension = ctx.canvasSize
			Dim insets As Insets = ctx.stretchingInsets
			If insets.left + insets.right > w OrElse insets.top + insets.bottom > h Then Return

			If w <= (canvas.width * ctx.maxHorizontalScaleFactor) AndAlso h <= (canvas.height * ctx.maxVerticalScaleFactor) Then
				' get image at canvas size
				Dim img As VolatileImage = getImage(g.deviceConfiguration, c, canvas.width, canvas.height, extendedCacheKeys)
				If img IsNot Nothing Then
					' calculate dst inserts
					' todo: destination inserts need to take into acount scale factor for high dpi. Note: You can use f for this, I think
					Dim dstInsets As Insets
					If ctx.inverted Then
						Dim leftRight As Integer = (w-(canvas.width-(insets.left+insets.right)))/2
						Dim topBottom As Integer = (h-(canvas.height-(insets.top+insets.bottom)))/2
						dstInsets = New Insets(topBottom,leftRight,topBottom,leftRight)
					Else
						dstInsets = insets
					End If
					' paint 9 square scaled
					Dim oldScaleingHints As Object = g.getRenderingHint(RenderingHints.KEY_INTERPOLATION)
					g.renderingHintint(RenderingHints.KEY_INTERPOLATION,RenderingHints.VALUE_INTERPOLATION_BILINEAR)
					ImageScalingHelper.paint(g, 0, 0, w, h, img, insets, dstInsets, ImageScalingHelper.PaintType.PAINT9_STRETCH, ImageScalingHelper.PAINT_ALL)
					g.renderingHintint(RenderingHints.KEY_INTERPOLATION,If(oldScaleingHints IsNot Nothing, oldScaleingHints, RenderingHints.VALUE_INTERPOLATION_NEAREST_NEIGHBOR))
				Else
					' render directly
					paint0(g, c, w, h, extendedCacheKeys)
				End If
			Else
				' paint directly
				paint0(g, c, w, h, extendedCacheKeys)
			End If
		End Sub

		Private Sub paintWithFixedSizeCaching(ByVal g As Graphics2D, ByVal c As JComponent, ByVal w As Integer, ByVal h As Integer, ByVal extendedCacheKeys As Object())
			Dim img As VolatileImage = getImage(g.deviceConfiguration, c, w, h, extendedCacheKeys)
			If img IsNot Nothing Then
				'render cached image
				g.drawImage(img, 0, 0, Nothing)
			Else
				' render directly
				paint0(g, c, w, h, extendedCacheKeys)
			End If
		End Sub

		''' <summary>
		''' Gets the rendered image for this painter at the requested size, either from cache or create a new one </summary>
		Private Function getImage(ByVal config As GraphicsConfiguration, ByVal c As JComponent, ByVal w As Integer, ByVal h As Integer, ByVal extendedCacheKeys As Object()) As VolatileImage
			Dim ___imageCache As ImageCache = ImageCache.instance
			'get the buffer for this component
			Dim buffer As VolatileImage = CType(___imageCache.getImage(config, w, h, Me, extendedCacheKeys), VolatileImage)

			Dim renderCounter As Integer = 0 'to avoid any potential, though unlikely, infinite loop
			Dim tempVar As Boolean
			Do
				'validate the buffer so we can check for surface loss
				Dim bufferStatus As Integer = VolatileImage.IMAGE_INCOMPATIBLE
				If buffer IsNot Nothing Then bufferStatus = buffer.validate(config)

				'If the buffer status is incompatible or restored, then we need to re-render to the volatile image
				If bufferStatus = VolatileImage.IMAGE_INCOMPATIBLE OrElse bufferStatus = VolatileImage.IMAGE_RESTORED Then
					'if the buffer is null (hasn't been created), or isn't the right size, or has lost its contents,
					'then recreate the buffer
					If buffer Is Nothing OrElse buffer.width <> w OrElse buffer.height <> h OrElse bufferStatus = VolatileImage.IMAGE_INCOMPATIBLE Then
						'clear any resources related to the old back buffer
						If buffer IsNot Nothing Then
							buffer.flush()
							buffer = Nothing
						End If
						'recreate the buffer
						buffer = config.createCompatibleVolatileImage(w, h, Transparency.TRANSLUCENT)
						' put in cache for future
						___imageCache.imageage(buffer, config, w, h, Me, extendedCacheKeys)
					End If
					'create the graphics context with which to paint to the buffer
					Dim bg As Graphics2D = buffer.createGraphics()
					'clear the background before configuring the graphics
					bg.composite = AlphaComposite.Clear
					bg.fillRect(0, 0, w, h)
					bg.composite = AlphaComposite.SrcOver
					configureGraphics(bg)
					' paint the painter into buffer
					paint0(bg, c, w, h, extendedCacheKeys)
					'close buffer graphics
					bg.Dispose()
				End If
				tempVar = buffer.contentsLost() AndAlso renderCounter < 3
				renderCounter += 1
			Loop While tempVar
			' check if we failed
			If renderCounter = 3 Then Return Nothing
			' return image
			Return buffer
		End Function

		'convenience method which creates a temporary graphics object by creating a
		'clone of the passed in one, configuring it, drawing with it, disposing it.
		'These steps have to be taken to ensure that any hints set on the graphics
		'are removed subsequent to painting.
		Private Sub paint0(ByVal g As Graphics2D, ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())
			prepare(width, height)
			g = CType(g.create(), Graphics2D)
			configureGraphics(g)
			doPaint(g, c, width, height, extendedCacheKeys)
			g.Dispose()
		End Sub

		Private Function clamp(ByVal value As Single) As Single
			If value < 0 Then
				value = 0
			ElseIf value > 1 Then
				value = 1
			End If
			Return value
		End Function

		Private Function clamp(ByVal value As Integer) As Integer
			If value < 0 Then
				value = 0
			ElseIf value > 255 Then
				value = 255
			End If
			Return value
		End Function
	End Class

End Namespace