Imports System.Collections

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.geom


	''' <summary>
	''' An <code>Area</code> object stores and manipulates a
	''' resolution-independent description of an enclosed area of
	''' 2-dimensional space.
	''' <code>Area</code> objects can be transformed and can perform
	''' various Constructive Area Geometry (CAG) operations when combined
	''' with other <code>Area</code> objects.
	''' The CAG operations include area
	''' <seealso cref="#add addition"/>, <seealso cref="#subtract subtraction"/>,
	''' <seealso cref="#intersect intersection"/>, and <seealso cref="#exclusiveOr exclusive or"/>.
	''' See the linked method documentation for examples of the various
	''' operations.
	''' <p>
	''' The <code>Area</code> class implements the <code>Shape</code>
	''' interface and provides full support for all of its hit-testing
	''' and path iteration facilities, but an <code>Area</code> is more
	''' specific than a generalized path in a number of ways:
	''' <ul>
	''' <li>Only closed paths and sub-paths are stored.
	'''     <code>Area</code> objects constructed from unclosed paths
	'''     are implicitly closed during construction as if those paths
	'''     had been filled by the <code>Graphics2D.fill</code> method.
	''' <li>The interiors of the individual stored sub-paths are all
	'''     non-empty and non-overlapping.  Paths are decomposed during
	'''     construction into separate component non-overlapping parts,
	'''     empty pieces of the path are discarded, and then these
	'''     non-empty and non-overlapping properties are maintained
	'''     through all subsequent CAG operations.  Outlines of different
	'''     component sub-paths may touch each other, as long as they
	'''     do not cross so that their enclosed areas overlap.
	''' <li>The geometry of the path describing the outline of the
	'''     <code>Area</code> resembles the path from which it was
	'''     constructed only in that it describes the same enclosed
	'''     2-dimensional area, but may use entirely different types
	'''     and ordering of the path segments to do so.
	''' </ul>
	''' Interesting issues which are not always obvious when using
	''' the <code>Area</code> include:
	''' <ul>
	''' <li>Creating an <code>Area</code> from an unclosed (open)
	'''     <code>Shape</code> results in a closed outline in the
	'''     <code>Area</code> object.
	''' <li>Creating an <code>Area</code> from a <code>Shape</code>
	'''     which encloses no area (even when "closed") produces an
	'''     empty <code>Area</code>.  A common example of this issue
	'''     is that producing an <code>Area</code> from a line will
	'''     be empty since the line encloses no area.  An empty
	'''     <code>Area</code> will iterate no geometry in its
	'''     <code>PathIterator</code> objects.
	''' <li>A self-intersecting <code>Shape</code> may be split into
	'''     two (or more) sub-paths each enclosing one of the
	'''     non-intersecting portions of the original path.
	''' <li>An <code>Area</code> may take more path segments to
	'''     describe the same geometry even when the original
	'''     outline is simple and obvious.  The analysis that the
	'''     <code>Area</code> class must perform on the path may
	'''     not reflect the same concepts of "simple and obvious"
	'''     as a human being perceives.
	''' </ul>
	''' 
	''' @since 1.2
	''' </summary>
	Public Class Area
		Implements java.awt.Shape, Cloneable

		Private Shared EmptyCurves As New ArrayList

		Private curves As ArrayList

		''' <summary>
		''' Default constructor which creates an empty area.
		''' @since 1.2
		''' </summary>
		Public Sub New()
			curves = EmptyCurves
		End Sub

		''' <summary>
		''' The <code>Area</code> class creates an area geometry from the
		''' specified <seealso cref="Shape"/> object.  The geometry is explicitly
		''' closed, if the <code>Shape</code> is not already closed.  The
		''' fill rule (even-odd or winding) specified by the geometry of the
		''' <code>Shape</code> is used to determine the resulting enclosed area. </summary>
		''' <param name="s">  the <code>Shape</code> from which the area is constructed </param>
		''' <exception cref="NullPointerException"> if <code>s</code> is null
		''' @since 1.2 </exception>
		Public Sub New(  s As java.awt.Shape)
			If TypeOf s Is Area Then
				curves = CType(s, Area).curves
			Else
				curves = pathToCurves(s.getPathIterator(Nothing))
			End If
		End Sub

		Private Shared Function pathToCurves(  pi As PathIterator) As ArrayList
			Dim curves As New ArrayList
			Dim windingRule As Integer = pi.windingRule
			' coords array is big enough for holding:
			'     coordinates returned from currentSegment (6)
			'     OR
			'         two subdivided quadratic curves (2+4+4=10)
			'         AND
			'             0-1 horizontal splitting parameters
			'             OR
			'             2 parametric equation derivative coefficients
			'     OR
			'         three subdivided cubic curves (2+6+6+6=20)
			'         AND
			'             0-2 horizontal splitting parameters
			'             OR
			'             3 parametric equation derivative coefficients
			Dim coords As Double() = New Double(22){}
			Dim movx As Double = 0, movy As Double = 0
			Dim curx As Double = 0, cury As Double = 0
			Dim newx, newy As Double
			Do While Not pi.done
				Select Case pi.currentSegment(coords)
				Case PathIterator.SEG_MOVETO
					sun.awt.geom.Curve.insertLine(curves, curx, cury, movx, movy)
						movx = coords(0)
						curx = movx
						movy = coords(1)
						cury = movy
					sun.awt.geom.Curve.insertMove(curves, movx, movy)
				Case PathIterator.SEG_LINETO
					newx = coords(0)
					newy = coords(1)
					sun.awt.geom.Curve.insertLine(curves, curx, cury, newx, newy)
					curx = newx
					cury = newy
				Case PathIterator.SEG_QUADTO
					newx = coords(2)
					newy = coords(3)
					sun.awt.geom.Curve.insertQuad(curves, curx, cury, coords)
					curx = newx
					cury = newy
				Case PathIterator.SEG_CUBICTO
					newx = coords(4)
					newy = coords(5)
					sun.awt.geom.Curve.insertCubic(curves, curx, cury, coords)
					curx = newx
					cury = newy
				Case PathIterator.SEG_CLOSE
					sun.awt.geom.Curve.insertLine(curves, curx, cury, movx, movy)
					curx = movx
					cury = movy
				End Select
				pi.next()
			Loop
			sun.awt.geom.Curve.insertLine(curves, curx, cury, movx, movy)
			Dim [operator] As sun.awt.geom.AreaOp
			If windingRule = PathIterator.WIND_EVEN_ODD Then
				[operator] = New sun.awt.geom.AreaOp.EOWindOp
			Else
				[operator] = New sun.awt.geom.AreaOp.NZWindOp
			End If
			Return [operator].calculate(curves, EmptyCurves)
		End Function

		''' <summary>
		''' Adds the shape of the specified <code>Area</code> to the
		''' shape of this <code>Area</code>.
		''' The resulting shape of this <code>Area</code> will include
		''' the union of both shapes, or all areas that were contained
		''' in either this or the specified <code>Area</code>.
		''' <pre>
		'''     // Example:
		'''     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		'''     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		'''     a1.add(a2);
		''' 
		'''        a1(before)     +         a2         =     a1(after)
		''' 
		'''     ################     ################     ################
		'''     ##############         ##############     ################
		'''     ############             ############     ################
		'''     ##########                 ##########     ################
		'''     ########                     ########     ################
		'''     ######                         ######     ######    ######
		'''     ####                             ####     ####        ####
		'''     ##                                 ##     ##            ##
		''' </pre> </summary>
		''' <param name="rhs">  the <code>Area</code> to be added to the
		'''          current shape </param>
		''' <exception cref="NullPointerException"> if <code>rhs</code> is null
		''' @since 1.2 </exception>
		Public Overridable Sub add(  rhs As Area)
			curves = (New sun.awt.geom.AreaOp.AddOp).calculate(Me.curves, rhs.curves)
			invalidateBounds()
		End Sub

		''' <summary>
		''' Subtracts the shape of the specified <code>Area</code> from the
		''' shape of this <code>Area</code>.
		''' The resulting shape of this <code>Area</code> will include
		''' areas that were contained only in this <code>Area</code>
		''' and not in the specified <code>Area</code>.
		''' <pre>
		'''     // Example:
		'''     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		'''     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		'''     a1.subtract(a2);
		''' 
		'''        a1(before)     -         a2         =     a1(after)
		''' 
		'''     ################     ################
		'''     ##############         ##############     ##
		'''     ############             ############     ####
		'''     ##########                 ##########     ######
		'''     ########                     ########     ########
		'''     ######                         ######     ######
		'''     ####                             ####     ####
		'''     ##                                 ##     ##
		''' </pre> </summary>
		''' <param name="rhs">  the <code>Area</code> to be subtracted from the
		'''          current shape </param>
		''' <exception cref="NullPointerException"> if <code>rhs</code> is null
		''' @since 1.2 </exception>
		Public Overridable Sub subtract(  rhs As Area)
			curves = (New sun.awt.geom.AreaOp.SubOp).calculate(Me.curves, rhs.curves)
			invalidateBounds()
		End Sub

		''' <summary>
		''' Sets the shape of this <code>Area</code> to the intersection of
		''' its current shape and the shape of the specified <code>Area</code>.
		''' The resulting shape of this <code>Area</code> will include
		''' only areas that were contained in both this <code>Area</code>
		''' and also in the specified <code>Area</code>.
		''' <pre>
		'''     // Example:
		'''     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		'''     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		'''     a1.intersect(a2);
		''' 
		'''      a1(before)   intersect     a2         =     a1(after)
		''' 
		'''     ################     ################     ################
		'''     ##############         ##############       ############
		'''     ############             ############         ########
		'''     ##########                 ##########           ####
		'''     ########                     ########
		'''     ######                         ######
		'''     ####                             ####
		'''     ##                                 ##
		''' </pre> </summary>
		''' <param name="rhs">  the <code>Area</code> to be intersected with this
		'''          <code>Area</code> </param>
		''' <exception cref="NullPointerException"> if <code>rhs</code> is null
		''' @since 1.2 </exception>
		Public Overridable Sub intersect(  rhs As Area)
			curves = (New sun.awt.geom.AreaOp.IntOp).calculate(Me.curves, rhs.curves)
			invalidateBounds()
		End Sub

		''' <summary>
		''' Sets the shape of this <code>Area</code> to be the combined area
		''' of its current shape and the shape of the specified <code>Area</code>,
		''' minus their intersection.
		''' The resulting shape of this <code>Area</code> will include
		''' only areas that were contained in either this <code>Area</code>
		''' or in the specified <code>Area</code>, but not in both.
		''' <pre>
		'''     // Example:
		'''     Area a1 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 0,8]);
		'''     Area a2 = new Area([triangle 0,0 =&gt; 8,0 =&gt; 8,8]);
		'''     a1.exclusiveOr(a2);
		''' 
		'''        a1(before)    xor        a2         =     a1(after)
		''' 
		'''     ################     ################
		'''     ##############         ##############     ##            ##
		'''     ############             ############     ####        ####
		'''     ##########                 ##########     ######    ######
		'''     ########                     ########     ################
		'''     ######                         ######     ######    ######
		'''     ####                             ####     ####        ####
		'''     ##                                 ##     ##            ##
		''' </pre> </summary>
		''' <param name="rhs">  the <code>Area</code> to be exclusive ORed with this
		'''          <code>Area</code>. </param>
		''' <exception cref="NullPointerException"> if <code>rhs</code> is null
		''' @since 1.2 </exception>
		Public Overridable Sub exclusiveOr(  rhs As Area)
			curves = (New sun.awt.geom.AreaOp.XorOp).calculate(Me.curves, rhs.curves)
			invalidateBounds()
		End Sub

		''' <summary>
		''' Removes all of the geometry from this <code>Area</code> and
		''' restores it to an empty area.
		''' @since 1.2
		''' </summary>
		Public Overridable Sub reset()
			curves = New ArrayList
			invalidateBounds()
		End Sub

		''' <summary>
		''' Tests whether this <code>Area</code> object encloses any area. </summary>
		''' <returns>    <code>true</code> if this <code>Area</code> object
		''' represents an empty area; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Property empty As Boolean
			Get
				Return (curves.Count = 0)
			End Get
		End Property

		''' <summary>
		''' Tests whether this <code>Area</code> consists entirely of
		''' straight edged polygonal geometry. </summary>
		''' <returns>    <code>true</code> if the geometry of this
		''' <code>Area</code> consists entirely of line segments;
		''' <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Property polygonal As Boolean
			Get
				Dim enum_ As System.Collections.IEnumerator = curves.elements()
				Do While enum_.hasMoreElements()
					If CType(enum_.nextElement(), sun.awt.geom.Curve).order > 1 Then Return False
				Loop
				Return True
			End Get
		End Property

		''' <summary>
		''' Tests whether this <code>Area</code> is rectangular in shape. </summary>
		''' <returns>    <code>true</code> if the geometry of this
		''' <code>Area</code> is rectangular in shape; <code>false</code>
		''' otherwise.
		''' @since 1.2 </returns>
		Public Overridable Property rectangular As Boolean
			Get
				Dim size As Integer = curves.Count
				If size = 0 Then Return True
				If size > 3 Then Return False
				Dim c1 As sun.awt.geom.Curve = CType(curves(1), sun.awt.geom.Curve)
				Dim c2 As sun.awt.geom.Curve = CType(curves(2), sun.awt.geom.Curve)
				If c1.order <> 1 OrElse c2.order <> 1 Then Return False
				If c1.xTop <> c1.xBot OrElse c2.xTop <> c2.xBot Then Return False
				If c1.yTop <> c2.yTop OrElse c1.yBot <> c2.yBot Then Return False
				Return True
			End Get
		End Property

		''' <summary>
		''' Tests whether this <code>Area</code> is comprised of a single
		''' closed subpath.  This method returns <code>true</code> if the
		''' path contains 0 or 1 subpaths, or <code>false</code> if the path
		''' contains more than 1 subpath.  The subpaths are counted by the
		''' number of <seealso cref="PathIterator#SEG_MOVETO SEG_MOVETO"/>  segments
		''' that appear in the path. </summary>
		''' <returns>    <code>true</code> if the <code>Area</code> is comprised
		''' of a single basic geometry; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Property singular As Boolean
			Get
				If curves.Count < 3 Then Return True
				Dim enum_ As System.Collections.IEnumerator = curves.elements()
				enum_.nextElement() ' First Order0 "moveto"
				Do While enum_.hasMoreElements()
					If CType(enum_.nextElement(), sun.awt.geom.Curve).order = 0 Then Return False
				Loop
				Return True
			End Get
		End Property

		Private cachedBounds As Rectangle2D
		Private Sub invalidateBounds()
			cachedBounds = Nothing
		End Sub
		Private Property cachedBounds As Rectangle2D
			Get
				If cachedBounds IsNot Nothing Then Return cachedBounds
				Dim r As Rectangle2D = New Rectangle2D.Double
				If curves.Count > 0 Then
					Dim c As sun.awt.geom.Curve = CType(curves(0), sun.awt.geom.Curve)
					' First point is always an order 0 curve (moveto)
					r.rectect(c.x0, c.y0, 0, 0)
					For i As Integer = 1 To curves.Count - 1
						CType(curves(i), sun.awt.geom.Curve).enlarge(r)
					Next i
				End If
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (cachedBounds = r)
			End Get
		End Property

		''' <summary>
		''' Returns a high precision bounding <seealso cref="Rectangle2D"/> that
		''' completely encloses this <code>Area</code>.
		''' <p>
		''' The Area class will attempt to return the tightest bounding
		''' box possible for the Shape.  The bounding box will not be
		''' padded to include the control points of curves in the outline
		''' of the Shape, but should tightly fit the actual geometry of
		''' the outline itself. </summary>
		''' <returns>    the bounding <code>Rectangle2D</code> for the
		''' <code>Area</code>.
		''' @since 1.2 </returns>
		Public Overridable Property bounds2D As Rectangle2D
			Get
				Return cachedBounds.bounds2D
			End Get
		End Property

		''' <summary>
		''' Returns a bounding <seealso cref="Rectangle"/> that completely encloses
		''' this <code>Area</code>.
		''' <p>
		''' The Area class will attempt to return the tightest bounding
		''' box possible for the Shape.  The bounding box will not be
		''' padded to include the control points of curves in the outline
		''' of the Shape, but should tightly fit the actual geometry of
		''' the outline itself.  Since the returned object represents
		''' the bounding box with integers, the bounding box can only be
		''' as tight as the nearest integer coordinates that encompass
		''' the geometry of the Shape. </summary>
		''' <returns>    the bounding <code>Rectangle</code> for the
		''' <code>Area</code>.
		''' @since 1.2 </returns>
		Public Overridable Property bounds As java.awt.Rectangle
			Get
				Return cachedBounds.bounds
			End Get
		End Property

		''' <summary>
		''' Returns an exact copy of this <code>Area</code> object. </summary>
		''' <returns>    Created clone object
		''' @since 1.2 </returns>
		Public Overridable Function clone() As Object
			Return New Area(Me)
		End Function

		''' <summary>
		''' Tests whether the geometries of the two <code>Area</code> objects
		''' are equal.
		''' This method will return false if the argument is null. </summary>
		''' <param name="other">  the <code>Area</code> to be compared to this
		'''          <code>Area</code> </param>
		''' <returns>  <code>true</code> if the two geometries are equal;
		'''          <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overrides Function Equals(  other As Area) As Boolean
			' REMIND: A *much* simpler operation should be possible...
			' Should be able to do a curve-wise comparison since all Areas
			' should evaluate their curves in the same top-down order.
			If other Is Me Then Return True
			If other Is Nothing Then Return False
			Dim c As ArrayList = (New sun.awt.geom.AreaOp.XorOp).calculate(Me.curves, other.curves)
			Return c.Count = 0
		End Function

		''' <summary>
		''' Transforms the geometry of this <code>Area</code> using the specified
		''' <seealso cref="AffineTransform"/>.  The geometry is transformed in place, which
		''' permanently changes the enclosed area defined by this object. </summary>
		''' <param name="t">  the transformation used to transform the area </param>
		''' <exception cref="NullPointerException"> if <code>t</code> is null
		''' @since 1.2 </exception>
		Public Overridable Sub transform(  t As AffineTransform)
			If t Is Nothing Then Throw New NullPointerException("transform must not be null")
			' REMIND: A simpler operation can be performed for some types
			' of transform.
			curves = pathToCurves(getPathIterator(t))
			invalidateBounds()
		End Sub

		''' <summary>
		''' Creates a new <code>Area</code> object that contains the same
		''' geometry as this <code>Area</code> transformed by the specified
		''' <code>AffineTransform</code>.  This <code>Area</code> object
		''' is unchanged. </summary>
		''' <param name="t">  the specified <code>AffineTransform</code> used to transform
		'''           the new <code>Area</code> </param>
		''' <exception cref="NullPointerException"> if <code>t</code> is null </exception>
		''' <returns>   a new <code>Area</code> object representing the transformed
		'''           geometry.
		''' @since 1.2 </returns>
		Public Overridable Function createTransformedArea(  t As AffineTransform) As Area
			Dim a As New Area(Me)
			a.transform(t)
			Return a
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function contains(  x As Double,   y As Double) As Boolean
			If Not cachedBounds.contains(x, y) Then Return False
			Dim enum_ As System.Collections.IEnumerator = curves.elements()
			Dim crossings As Integer = 0
			Do While enum_.hasMoreElements()
				Dim c As sun.awt.geom.Curve = CType(enum_.nextElement(), sun.awt.geom.Curve)
				crossings += c.crossingsFor(x, y)
			Loop
			Return ((crossings And 1) = 1)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function contains(  p As Point2D) As Boolean
			Return contains(p.x, p.y)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function contains(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If w < 0 OrElse h < 0 Then Return False
			If Not cachedBounds.contains(x, y, w, h) Then Return False
			Dim c As sun.awt.geom.Crossings = sun.awt.geom.Crossings.findCrossings(curves, x, y, x+w, y+h)
			Return (c IsNot Nothing AndAlso c.covers(y, y+h))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function contains(  r As Rectangle2D) As Boolean
			Return contains(r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function intersects(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If w < 0 OrElse h < 0 Then Return False
			If Not cachedBounds.intersects(x, y, w, h) Then Return False
			Dim c As sun.awt.geom.Crossings = sun.awt.geom.Crossings.findCrossings(curves, x, y, x+w, y+h)
			Return (c Is Nothing OrElse (Not c.empty))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function intersects(  r As Rectangle2D) As Boolean
			Return intersects(r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' Creates a <seealso cref="PathIterator"/> for the outline of this
		''' <code>Area</code> object.  This <code>Area</code> object is unchanged. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to
		''' the coordinates as they are returned in the iteration, or
		''' <code>null</code> if untransformed coordinates are desired </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this <code>Area</code>, one
		'''          segment at a time.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(  at As AffineTransform) As PathIterator
			Return New AreaIterator(curves, at)
		End Function

		''' <summary>
		''' Creates a <code>PathIterator</code> for the flattened outline of
		''' this <code>Area</code> object.  Only uncurved path segments
		''' represented by the SEG_MOVETO, SEG_LINETO, and SEG_CLOSE point
		''' types are returned by the iterator.  This <code>Area</code>
		''' object is unchanged. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be
		''' applied to the coordinates as they are returned in the
		''' iteration, or <code>null</code> if untransformed coordinates
		''' are desired </param>
		''' <param name="flatness"> the maximum amount that the control points
		''' for a given curve can vary from colinear before a subdivided
		''' curve is replaced by a straight line connecting the end points </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		''' geometry of the outline of this <code>Area</code>, one segment
		''' at a time.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(  at As AffineTransform,   flatness As Double) As PathIterator
			Return New FlatteningPathIterator(getPathIterator(at), flatness)
		End Function
	End Class

	Friend Class AreaIterator
		Implements PathIterator

		Private transform As AffineTransform
		Private curves As ArrayList
		Private index As Integer
		Private prevcurve As sun.awt.geom.Curve
		Private thiscurve As sun.awt.geom.Curve

		Public Sub New(  curves As ArrayList,   at As AffineTransform)
			Me.curves = curves
			Me.transform = at
			If curves.Count >= 1 Then thiscurve = CType(curves(0), sun.awt.geom.Curve)
		End Sub

		Public Overridable Property windingRule As Integer Implements PathIterator.getWindingRule
			Get
				' REMIND: Which is better, EVEN_ODD or NON_ZERO?
				'         The paths calculated could be classified either way.
				'return WIND_EVEN_ODD;
				Return WIND_NON_ZERO
			End Get
		End Property

		Public Overridable Property done As Boolean Implements PathIterator.isDone
			Get
				Return (prevcurve Is Nothing AndAlso thiscurve Is Nothing)
			End Get
		End Property

		Public Overridable Sub [next]() Implements PathIterator.next
			If prevcurve IsNot Nothing Then
				prevcurve = Nothing
			Else
				prevcurve = thiscurve
				index += 1
				If index < curves.Count Then
					thiscurve = CType(curves(index), sun.awt.geom.Curve)
					If thiscurve.order <> 0 AndAlso prevcurve.x1 = thiscurve.x0 AndAlso prevcurve.y1 = thiscurve.y0 Then prevcurve = Nothing
				Else
					thiscurve = Nothing
				End If
			End If
		End Sub

		Public Overridable Function currentSegment(  coords As Single()) As Integer Implements PathIterator.currentSegment
			Dim dcoords As Double() = New Double(5){}
			Dim segtype As Integer = currentSegment(dcoords)
			Dim numpoints As Integer = (If(segtype = SEG_CLOSE, 0, (If(segtype = SEG_QUADTO, 2, (If(segtype = SEG_CUBICTO, 3, 1))))))
			For i As Integer = 0 To numpoints * 2 - 1
				coords(i) = CSng(dcoords(i))
			Next i
			Return segtype
		End Function

		Public Overridable Function currentSegment(  coords As Double()) As Integer Implements PathIterator.currentSegment
			Dim segtype As Integer
			Dim numpoints As Integer
			If prevcurve IsNot Nothing Then
				' Need to finish off junction between curves
				If thiscurve Is Nothing OrElse thiscurve.order = 0 Then Return SEG_CLOSE
				coords(0) = thiscurve.x0
				coords(1) = thiscurve.y0
				segtype = SEG_LINETO
				numpoints = 1
			ElseIf thiscurve Is Nothing Then
				Throw New java.util.NoSuchElementException("area iterator out of bounds")
			Else
				segtype = thiscurve.getSegment(coords)
				numpoints = thiscurve.order
				If numpoints = 0 Then numpoints = 1
			End If
			If transform IsNot Nothing Then transform.transform(coords, 0, coords, 0, numpoints)
			Return segtype
		End Function
	End Class

End Namespace