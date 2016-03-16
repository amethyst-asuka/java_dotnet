Imports System

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Polygon</code> class encapsulates a description of a
	''' closed, two-dimensional region within a coordinate space. This
	''' region is bounded by an arbitrary number of line segments, each of
	''' which is one side of the polygon. Internally, a polygon
	''' comprises of a list of {@code (x,y)}
	''' coordinate pairs, where each pair defines a <i>vertex</i> of the
	''' polygon, and two successive pairs are the endpoints of a
	''' line that is a side of the polygon. The first and final
	''' pairs of {@code (x,y)} points are joined by a line segment
	''' that closes the polygon.  This <code>Polygon</code> is defined with
	''' an even-odd winding rule.  See
	''' <seealso cref="java.awt.geom.PathIterator#WIND_EVEN_ODD WIND_EVEN_ODD"/>
	''' for a definition of the even-odd winding rule.
	''' This class's hit-testing methods, which include the
	''' <code>contains</code>, <code>intersects</code> and <code>inside</code>
	''' methods, use the <i>insideness</i> definition described in the
	''' <seealso cref="Shape"/> class comments.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref= Shape
	''' @author      Herb Jellinek
	''' @since       1.0 </seealso>
	<Serializable> _
	Public Class Polygon
		Implements Shape

		''' <summary>
		''' The total number of points.  The value of <code>npoints</code>
		''' represents the number of valid points in this <code>Polygon</code>
		''' and might be less than the number of elements in
		''' <seealso cref="#xpoints xpoints"/> or <seealso cref="#ypoints ypoints"/>.
		''' This value can be NULL.
		''' 
		''' @serial </summary>
		''' <seealso cref= #addPoint(int, int)
		''' @since 1.0 </seealso>
		Public npoints As Integer

		''' <summary>
		''' The array of X coordinates.  The number of elements in
		''' this array might be more than the number of X coordinates
		''' in this <code>Polygon</code>.  The extra elements allow new points
		''' to be added to this <code>Polygon</code> without re-creating this
		''' array.  The value of <seealso cref="#npoints npoints"/> is equal to the
		''' number of valid points in this <code>Polygon</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #addPoint(int, int)
		''' @since 1.0 </seealso>
		Public xpoints As Integer()

		''' <summary>
		''' The array of Y coordinates.  The number of elements in
		''' this array might be more than the number of Y coordinates
		''' in this <code>Polygon</code>.  The extra elements allow new points
		''' to be added to this <code>Polygon</code> without re-creating this
		''' array.  The value of <code>npoints</code> is equal to the
		''' number of valid points in this <code>Polygon</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #addPoint(int, int)
		''' @since 1.0 </seealso>
		Public ypoints As Integer()

		''' <summary>
		''' The bounds of this {@code Polygon}.
		''' This value can be null.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getBoundingBox() </seealso>
		''' <seealso cref= #getBounds()
		''' @since 1.0 </seealso>
		Protected Friend bounds As Rectangle

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -6460061437900069969L

	'    
	'     * Default length for xpoints and ypoints.
	'     
		Private Const MIN_LENGTH As Integer = 4

		''' <summary>
		''' Creates an empty polygon.
		''' @since 1.0
		''' </summary>
		Public Sub New()
			xpoints = New Integer(MIN_LENGTH - 1){}
			ypoints = New Integer(MIN_LENGTH - 1){}
		End Sub

		''' <summary>
		''' Constructs and initializes a <code>Polygon</code> from the specified
		''' parameters. </summary>
		''' <param name="xpoints"> an array of X coordinates </param>
		''' <param name="ypoints"> an array of Y coordinates </param>
		''' <param name="npoints"> the total number of points in the
		'''                          <code>Polygon</code> </param>
		''' <exception cref="NegativeArraySizeException"> if the value of
		'''                       <code>npoints</code> is negative. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>npoints</code> is
		'''             greater than the length of <code>xpoints</code>
		'''             or the length of <code>ypoints</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>xpoints</code> or
		'''             <code>ypoints</code> is <code>null</code>.
		''' @since 1.0 </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public Polygon(int xpoints() , int ypoints(), int npoints)
			' Fix 4489009: should throw IndexOutofBoundsException instead
			' of OutofMemoryException if npoints is huge and > {x,y}points.length
			If npoints > xpoints.Length OrElse npoints > ypoints.Length Then Throw New IndexOutOfBoundsException("npoints > xpoints.length || " & "npoints > ypoints.length")
			' Fix 6191114: should throw NegativeArraySizeException with
			' negative npoints
			If npoints < 0 Then Throw New NegativeArraySizeException("npoints < 0")
			' Fix 6343431: Applet compatibility problems if arrays are not
			' exactly npoints in length
			Me.npoints = npoints
			Me.xpoints = java.util.Arrays.copyOf(xpoints, npoints)
			Me.ypoints = java.util.Arrays.copyOf(ypoints, npoints)

		''' <summary>
		''' Resets this <code>Polygon</code> object to an empty polygon.
		''' The coordinate arrays and the data in them are left untouched
		''' but the number of points is reset to zero to mark the old
		''' vertex data as invalid and to start accumulating new vertex
		''' data at the beginning.
		''' All internally-cached data relating to the old vertices
		''' are discarded.
		''' Note that since the coordinate arrays from before the reset
		''' are reused, creating a new empty <code>Polygon</code> might
		''' be more memory efficient than resetting the current one if
		''' the number of vertices in the new polygon data is significantly
		''' smaller than the number of vertices in the data from before the
		''' reset. </summary>
		''' <seealso cref=         java.awt.Polygon#invalidate
		''' @since 1.4 </seealso>
		public  Sub  reset()
			npoints = 0
			bounds = Nothing

		''' <summary>
		''' Invalidates or flushes any internally-cached data that depends
		''' on the vertex coordinates of this <code>Polygon</code>.
		''' This method should be called after any direct manipulation
		''' of the coordinates in the <code>xpoints</code> or
		''' <code>ypoints</code> arrays to avoid inconsistent results
		''' from methods such as <code>getBounds</code> or <code>contains</code>
		''' that might cache data from earlier computations relating to
		''' the vertex coordinates. </summary>
		''' <seealso cref=         java.awt.Polygon#getBounds
		''' @since 1.4 </seealso>
		public  Sub  invalidate()
			bounds = Nothing

		''' <summary>
		''' Translates the vertices of the <code>Polygon</code> by
		''' <code>deltaX</code> along the x axis and by
		''' <code>deltaY</code> along the y axis. </summary>
		''' <param name="deltaX"> the amount to translate along the X axis </param>
		''' <param name="deltaY"> the amount to translate along the Y axis
		''' @since 1.1 </param>
		public  Sub  translate(Integer deltaX, Integer deltaY)
			For i As Integer = 0 To npoints - 1
				xpoints(i) += deltaX
				ypoints(i) += deltaY
			Next i
			If bounds IsNot Nothing Then bounds.translate(deltaX, deltaY)

	'    
	'     * Calculates the bounding box of the points passed to the constructor.
	'     * Sets <code>bounds</code> to the result.
	'     * @param xpoints[] array of <i>x</i> coordinates
	'     * @param ypoints[] array of <i>y</i> coordinates
	'     * @param npoints the total number of points
	'     
		void calculateBounds(Integer xpoints() , Integer ypoints(), Integer npoints)
			Dim boundsMinX As Integer =  java.lang.[Integer].Max_Value
			Dim boundsMinY As Integer =  java.lang.[Integer].Max_Value
			Dim boundsMaxX As Integer =  java.lang.[Integer].MIN_VALUE
			Dim boundsMaxY As Integer =  java.lang.[Integer].MIN_VALUE

			For i As Integer = 0 To npoints - 1
				Dim x As Integer = xpoints(i)
				boundsMinX = System.Math.Min(boundsMinX, x)
				boundsMaxX = System.Math.Max(boundsMaxX, x)
				Dim y As Integer = ypoints(i)
				boundsMinY = System.Math.Min(boundsMinY, y)
				boundsMaxY = System.Math.Max(boundsMaxY, y)
			Next i
			bounds = New Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY)

	'    
	'     * Resizes the bounding box to accommodate the specified coordinates.
	'     * @param x,&nbsp;y the specified coordinates
	'     
		void updateBounds(Integer x, Integer y)
			If x < bounds.x Then
				bounds.width = bounds.width + (bounds.x - x)
				bounds.x = x
			Else
				bounds.width = System.Math.Max(bounds.width, x - bounds.x)
				' bounds.x = bounds.x;
			End If

			If y < bounds.y Then
				bounds.height = bounds.height + (bounds.y - y)
				bounds.y = y
			Else
				bounds.height = System.Math.Max(bounds.height, y - bounds.y)
				' bounds.y = bounds.y;
			End If

		''' <summary>
		''' Appends the specified coordinates to this <code>Polygon</code>.
		''' <p>
		''' If an operation that calculates the bounding box of this
		''' <code>Polygon</code> has already been performed, such as
		''' <code>getBounds</code> or <code>contains</code>, then this
		''' method updates the bounding box. </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <seealso cref=         java.awt.Polygon#getBounds </seealso>
		''' <seealso cref=         java.awt.Polygon#contains
		''' @since 1.0 </seealso>
		public  Sub  addPoint(Integer x, Integer y)
			If npoints >= xpoints.Length OrElse npoints >= ypoints.Length Then
				Dim newLength As Integer = npoints * 2
				' Make sure that newLength will be greater than MIN_LENGTH and
				' aligned to the power of 2
				If newLength < MIN_LENGTH Then
					newLength = MIN_LENGTH
				ElseIf (newLength And (newLength - 1)) <> 0 Then
					newLength =  java.lang.[Integer].highestOneBit(newLength)
				End If

				xpoints = java.util.Arrays.copyOf(xpoints, newLength)
				ypoints = java.util.Arrays.copyOf(ypoints, newLength)
			End If
			xpoints(npoints) = x
			ypoints(npoints) = y
			npoints += 1
			If bounds IsNot Nothing Then updateBounds(x, y)

		''' <summary>
		''' Gets the bounding box of this <code>Polygon</code>.
		''' The bounding box is the smallest <seealso cref="Rectangle"/> whose
		''' sides are parallel to the x and y axes of the
		''' coordinate space, and can completely contain the <code>Polygon</code>. </summary>
		''' <returns> a <code>Rectangle</code> that defines the bounds of this
		''' <code>Polygon</code>.
		''' @since 1.1 </returns>
		public Rectangle bounds
			Return boundingBox

		''' <summary>
		''' Returns the bounds of this <code>Polygon</code>. </summary>
		''' <returns> the bounds of this <code>Polygon</code>. </returns>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getBounds()</code>.
		''' @since 1.0 
		<Obsolete("As of JDK version 1.1,")> _
		public Rectangle boundingBox
			If npoints = 0 Then Return New Rectangle
			If bounds Is Nothing Then calculateBounds(xpoints, ypoints, npoints)
			Return bounds.bounds

		''' <summary>
		''' Determines whether the specified <seealso cref="Point"/> is inside this
		''' <code>Polygon</code>. </summary>
		''' <param name="p"> the specified <code>Point</code> to be tested </param>
		''' <returns> <code>true</code> if the <code>Polygon</code> contains the
		'''                  <code>Point</code>; <code>false</code> otherwise. </returns>
		''' <seealso cref= #contains(double, double)
		''' @since 1.0 </seealso>
		public Boolean contains(Point p)
			Return contains(p.x, p.y)

		''' <summary>
		''' Determines whether the specified coordinates are inside this
		''' <code>Polygon</code>.
		''' <p> </summary>
		''' <param name="x"> the specified X coordinate to be tested </param>
		''' <param name="y"> the specified Y coordinate to be tested </param>
		''' <returns> {@code true} if this {@code Polygon} contains
		'''         the specified coordinates {@code (x,y)};
		'''         {@code false} otherwise. </returns>
		''' <seealso cref= #contains(double, double)
		''' @since 1.1 </seealso>
		public Boolean contains(Integer x, Integer y)
			Return contains(CDbl(x), CDbl(y))

		''' <summary>
		''' Determines whether the specified coordinates are contained in this
		''' <code>Polygon</code>. </summary>
		''' <param name="x"> the specified X coordinate to be tested </param>
		''' <param name="y"> the specified Y coordinate to be tested </param>
		''' <returns> {@code true} if this {@code Polygon} contains
		'''         the specified coordinates {@code (x,y)};
		'''         {@code false} otherwise. </returns>
		''' <seealso cref= #contains(double, double) </seealso>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>contains(int, int)</code>.
		''' @since 1.0 
		<Obsolete("As of JDK version 1.1,")> _
		public Boolean inside(Integer x, Integer y)
			Return contains(CDbl(x), CDbl(y))

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public java.awt.geom.Rectangle2D bounds2D
			Return bounds

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Double x, Double y)
			If npoints <= 2 OrElse (Not boundingBox.contains(x, y)) Then Return False
			Dim hits As Integer = 0

			Dim lastx As Integer = xpoints(npoints - 1)
			Dim lasty As Integer = ypoints(npoints - 1)
			Dim curx, cury As Integer

			' Walk the edges of the polygon
			Dim i As Integer = 0
			Do While i < npoints
				curx = xpoints(i)
				cury = ypoints(i)

				If cury = lasty Then
					lastx = curx
				lasty = cury
				i += 1
					Continue Do
				End If

				Dim leftx As Integer
				If curx < lastx Then
					If x >= lastx Then
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					leftx = curx
				Else
					If x >= curx Then
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					leftx = lastx
				End If

				Dim test1, test2 As Double
				If cury < lasty Then
					If y < cury OrElse y >= lasty Then
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					If x < leftx Then
						hits += 1
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					test1 = x - curx
					test2 = y - cury
				Else
					If y < lasty OrElse y >= cury Then
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					If x < leftx Then
						hits += 1
						lastx = curx
				lasty = cury
				i += 1
						Continue Do
					End If
					test1 = x - lastx
					test2 = y - lasty
				End If

				If test1 < (test2 / (lasty - cury) * (lastx - curx)) Then hits += 1
				lastx = curx
				lasty = cury
				i += 1
			Loop

			Return ((hits And 1) <> 0)

		private sun.awt.geom.Crossings getCrossings(Double xlo, Double ylo, Double xhi, Double yhi)
			Dim cross As sun.awt.geom.Crossings = New sun.awt.geom.Crossings.EvenOdd(xlo, ylo, xhi, yhi)
			Dim lastx As Integer = xpoints(npoints - 1)
			Dim lasty As Integer = ypoints(npoints - 1)
			Dim curx, cury As Integer

			' Walk the edges of the polygon
			For i As Integer = 0 To npoints - 1
				curx = xpoints(i)
				cury = ypoints(i)
				If cross.accumulateLine(lastx, lasty, curx, cury) Then Return Nothing
				lastx = curx
				lasty = cury
			Next i

			Return cross

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(java.awt.geom.Point2D p)
			Return contains(p.x, p.y)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean intersects(Double x, Double y, Double w, Double h)
			If npoints <= 0 OrElse (Not boundingBox.intersects(x, y, w, h)) Then Return False

			Dim cross As sun.awt.geom.Crossings = getCrossings(x, y, x+w, y+h)
			Return (cross Is Nothing OrElse (Not cross.empty))

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean intersects(java.awt.geom.Rectangle2D r)
			Return intersects(r.x, r.y, r.width, r.height)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Double x, Double y, Double w, Double h)
			If npoints <= 0 OrElse (Not boundingBox.intersects(x, y, w, h)) Then Return False

			Dim cross As sun.awt.geom.Crossings = getCrossings(x, y, x+w, y+h)
			Return (cross IsNot Nothing AndAlso cross.covers(y, y+h))

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(java.awt.geom.Rectangle2D r)
			Return contains(r.x, r.y, r.width, r.height)

		''' <summary>
		''' Returns an iterator object that iterates along the boundary of this
		''' <code>Polygon</code> and provides access to the geometry
		''' of the outline of this <code>Polygon</code>.  An optional
		''' <seealso cref="AffineTransform"/> can be specified so that the coordinates
		''' returned in the iteration are transformed accordingly. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		'''          coordinates as they are returned in the iteration, or
		'''          <code>null</code> if untransformed coordinates are desired </param>
		''' <returns> a <seealso cref="PathIterator"/> object that provides access to the
		'''          geometry of this <code>Polygon</code>.
		''' @since 1.2 </returns>
		public java.awt.geom.PathIterator getPathIterator(java.awt.geom.AffineTransform at)
			Return New PolygonPathIterator(Me, Me, at)

		''' <summary>
		''' Returns an iterator object that iterates along the boundary of
		''' the <code>Shape</code> and provides access to the geometry of the
		''' outline of the <code>Shape</code>.  Only SEG_MOVETO, SEG_LINETO, and
		''' SEG_CLOSE point types are returned by the iterator.
		''' Since polygons are already flat, the <code>flatness</code> parameter
		''' is ignored.  An optional <code>AffineTransform</code> can be specified
		''' in which case the coordinates returned in the iteration are transformed
		''' accordingly. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		'''          coordinates as they are returned in the iteration, or
		'''          <code>null</code> if untransformed coordinates are desired </param>
		''' <param name="flatness"> the maximum amount that the control points
		'''          for a given curve can vary from colinear before a subdivided
		'''          curve is replaced by a straight line connecting the
		'''          endpoints.  Since polygons are already flat the
		'''          <code>flatness</code> parameter is ignored. </param>
		''' <returns> a <code>PathIterator</code> object that provides access to the
		'''          <code>Shape</code> object's geometry.
		''' @since 1.2 </returns>
		public java.awt.geom.PathIterator getPathIterator(java.awt.geom.AffineTransform at, Double flatness)
			Return getPathIterator(at)

'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'		class PolygonPathIterator implements java.awt.geom.PathIterator
	'	{
	'		Polygon poly;
	'		AffineTransform transform;
	'		int index;
	'
	'		public PolygonPathIterator(Polygon pg, AffineTransform at)
	'		{
	'			poly = pg;
	'			transform = at;
	'			if (pg.npoints == 0)
	'			{
	'				' Prevent a spurious SEG_CLOSE segment
	'				index = 1;
	'			}
	'		}
	'
	'		''' <summary>
	'		''' Returns the winding rule for determining the interior of the
	'		''' path. </summary>
	'		''' <returns> an integer representing the current winding rule. </returns>
	'		''' <seealso cref= PathIterator#WIND_NON_ZERO </seealso>
	'		public int getWindingRule()
	'		{
	'			Return WIND_EVEN_ODD;
	'		}
	'
	'		''' <summary>
	'		''' Tests if there are more points to read. </summary>
	'		''' <returns> <code>true</code> if there are more points to read;
	'		'''          <code>false</code> otherwise. </returns>
	'		public boolean isDone()
	'		{
	'			Return index > poly.npoints;
	'		}
	'
	'		''' <summary>
	'		''' Moves the iterator forwards, along the primary direction of
	'		''' traversal, to the next segment of the path when there are
	'		''' more points in that direction.
	'		''' </summary>
	'		public  Sub  next()
	'		{
	'			index += 1;
	'		}
	'
	'		''' <summary>
	'		''' Returns the coordinates and type of the current path segment in
	'		''' the iteration.
	'		''' The return value is the path segment type:
	'		''' SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
	'		''' A <code>float</code> array of length 2 must be passed in and
	'		''' can be used to store the coordinates of the point(s).
	'		''' Each point is stored as a pair of <code>float</code> x,&nbsp;y
	'		''' coordinates.  SEG_MOVETO and SEG_LINETO types return one
	'		''' point, and SEG_CLOSE does not return any points. </summary>
	'		''' <param name="coords"> a <code>float</code> array that specifies the
	'		''' coordinates of the point(s) </param>
	'		''' <returns> an integer representing the type and coordinates of the
	'		'''              current path segment. </returns>
	'		''' <seealso cref= PathIterator#SEG_MOVETO </seealso>
	'		''' <seealso cref= PathIterator#SEG_LINETO </seealso>
	'		''' <seealso cref= PathIterator#SEG_CLOSE </seealso>
	'		public int currentSegment(float[] coords)
	'		{
	'			if (index >= poly.npoints)
	'			{
	'				Return SEG_CLOSE;
	'			}
	'			coords[0] = poly.xpoints[index];
	'			coords[1] = poly.ypoints[index];
	'			if (transform != Nothing)
	'			{
	'				transform.transform(coords, 0, coords, 0, 1);
	'			}
	'			Return (index == 0 ? SEG_MOVETO : SEG_LINETO);
	'		}
	'
	'		''' <summary>
	'		''' Returns the coordinates and type of the current path segment in
	'		''' the iteration.
	'		''' The return value is the path segment type:
	'		''' SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
	'		''' A <code>double</code> array of length 2 must be passed in and
	'		''' can be used to store the coordinates of the point(s).
	'		''' Each point is stored as a pair of <code>double</code> x,&nbsp;y
	'		''' coordinates.
	'		''' SEG_MOVETO and SEG_LINETO types return one point,
	'		''' and SEG_CLOSE does not return any points. </summary>
	'		''' <param name="coords"> a <code>double</code> array that specifies the
	'		''' coordinates of the point(s) </param>
	'		''' <returns> an integer representing the type and coordinates of the
	'		'''              current path segment. </returns>
	'		''' <seealso cref= PathIterator#SEG_MOVETO </seealso>
	'		''' <seealso cref= PathIterator#SEG_LINETO </seealso>
	'		''' <seealso cref= PathIterator#SEG_CLOSE </seealso>
	'		public int currentSegment(double[] coords)
	'		{
	'			if (index >= poly.npoints)
	'			{
	'				Return SEG_CLOSE;
	'			}
	'			coords[0] = poly.xpoints[index];
	'			coords[1] = poly.ypoints[index];
	'			if (transform != Nothing)
	'			{
	'				transform.transform(coords, 0, coords, 0, 1);
	'			}
	'			Return (index == 0 ? SEG_MOVETO : SEG_LINETO);
	'		}
	'	}
	End Class

End Namespace