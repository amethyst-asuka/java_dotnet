Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' A utility class to iterate over the path segments of an arc
	''' through the PathIterator interface.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Friend Class ArcIterator
		Implements PathIterator

		Friend x, y, w, h, angStRad, increment, cv As Double
		Friend affine As AffineTransform
		Friend index As Integer
		Friend arcSegs As Integer
		Friend lineSegs As Integer

		Friend Sub New(ByVal a As Arc2D, ByVal at As AffineTransform)
			Me.w = a.width / 2
			Me.h = a.height / 2
			Me.x = a.x + w
			Me.y = a.y + h
			Me.angStRad = -Math.toRadians(a.angleStart)
			Me.affine = at
			Dim ext As Double = -a.angleExtent
			If ext >= 360.0 OrElse ext <= -360 Then
				arcSegs = 4
				Me.increment = Math.PI / 2
				' btan(Math.PI / 2);
				Me.cv = 0.5522847498307933
				If ext < 0 Then
					increment = -increment
					cv = -cv
				End If
			Else
				arcSegs = CInt(Fix(Math.Ceiling(Math.Abs(ext) / 90.0)))
				Me.increment = Math.toRadians(ext / arcSegs)
				Me.cv = btan(increment)
				If cv = 0 Then arcSegs = 0
			End If
			Select Case a.arcType
			Case Arc2D.OPEN
				lineSegs = 0
			Case Arc2D.CHORD
				lineSegs = 1
			Case Arc2D.PIE
				lineSegs = 2
			End Select
			If w < 0 OrElse h < 0 Then
					lineSegs = -1
					arcSegs = lineSegs
			End If
		End Sub

		''' <summary>
		''' Return the winding rule for determining the insideness of the
		''' path. </summary>
		''' <seealso cref= #WIND_EVEN_ODD </seealso>
		''' <seealso cref= #WIND_NON_ZERO </seealso>
		Public Overridable Property windingRule As Integer Implements PathIterator.getWindingRule
			Get
				Return WIND_NON_ZERO
			End Get
		End Property

		''' <summary>
		''' Tests if there are more points to read. </summary>
		''' <returns> true if there are more points to read </returns>
		Public Overridable Property done As Boolean Implements PathIterator.isDone
			Get
				Return index > arcSegs + lineSegs
			End Get
		End Property

		''' <summary>
		''' Moves the iterator to the next segment of the path forwards
		''' along the primary direction of traversal as long as there are
		''' more points in that direction.
		''' </summary>
		Public Overridable Sub [next]() Implements PathIterator.next
			index += 1
		End Sub

	'    
	'     * btan computes the length (k) of the control segments at
	'     * the beginning and end of a cubic bezier that approximates
	'     * a segment of an arc with extent less than or equal to
	'     * 90 degrees.  This length (k) will be used to generate the
	'     * 2 bezier control points for such a segment.
	'     *
	'     *   Assumptions:
	'     *     a) arc is centered on 0,0 with radius of 1.0
	'     *     b) arc extent is less than 90 degrees
	'     *     c) control points should preserve tangent
	'     *     d) control segments should have equal length
	'     *
	'     *   Initial data:
	'     *     start angle: ang1
	'     *     end angle:   ang2 = ang1 + extent
	'     *     start point: P1 = (x1, y1) = (cos(ang1), sin(ang1))
	'     *     end point:   P4 = (x4, y4) = (cos(ang2), sin(ang2))
	'     *
	'     *   Control points:
	'     *     P2 = (x2, y2)
	'     *     | x2 = x1 - k * sin(ang1) = cos(ang1) - k * sin(ang1)
	'     *     | y2 = y1 + k * cos(ang1) = sin(ang1) + k * cos(ang1)
	'     *
	'     *     P3 = (x3, y3)
	'     *     | x3 = x4 + k * sin(ang2) = cos(ang2) + k * sin(ang2)
	'     *     | y3 = y4 - k * cos(ang2) = sin(ang2) - k * cos(ang2)
	'     *
	'     * The formula for this length (k) can be found using the
	'     * following derivations:
	'     *
	'     *   Midpoints:
	'     *     a) bezier (t = 1/2)
	'     *        bPm = P1 * (1-t)^3 +
	'     *              3 * P2 * t * (1-t)^2 +
	'     *              3 * P3 * t^2 * (1-t) +
	'     *              P4 * t^3 =
	'     *            = (P1 + 3P2 + 3P3 + P4)/8
	'     *
	'     *     b) arc
	'     *        aPm = (cos((ang1 + ang2)/2), sin((ang1 + ang2)/2))
	'     *
	'     *   Let angb = (ang2 - ang1)/2; angb is half of the angle
	'     *   between ang1 and ang2.
	'     *
	'     *   Solve the equation bPm == aPm
	'     *
	'     *     a) For xm coord:
	'     *        x1 + 3*x2 + 3*x3 + x4 = 8*cos((ang1 + ang2)/2)
	'     *
	'     *        cos(ang1) + 3*cos(ang1) - 3*k*sin(ang1) +
	'     *        3*cos(ang2) + 3*k*sin(ang2) + cos(ang2) =
	'     *        = 8*cos((ang1 + ang2)/2)
	'     *
	'     *        4*cos(ang1) + 4*cos(ang2) + 3*k*(sin(ang2) - sin(ang1)) =
	'     *        = 8*cos((ang1 + ang2)/2)
	'     *
	'     *        8*cos((ang1 + ang2)/2)*cos((ang2 - ang1)/2) +
	'     *        6*k*sin((ang2 - ang1)/2)*cos((ang1 + ang2)/2) =
	'     *        = 8*cos((ang1 + ang2)/2)
	'     *
	'     *        4*cos(angb) + 3*k*sin(angb) = 4
	'     *
	'     *        k = 4 / 3 * (1 - cos(angb)) / sin(angb)
	'     *
	'     *     b) For ym coord we derive the same formula.
	'     *
	'     * Since this formula can generate "NaN" values for small
	'     * angles, we will derive a safer form that does not involve
	'     * dividing by very small values:
	'     *     (1 - cos(angb)) / sin(angb) =
	'     *     = (1 - cos(angb))*(1 + cos(angb)) / sin(angb)*(1 + cos(angb)) =
	'     *     = (1 - cos(angb)^2) / sin(angb)*(1 + cos(angb)) =
	'     *     = sin(angb)^2 / sin(angb)*(1 + cos(angb)) =
	'     *     = sin(angb) / (1 + cos(angb))
	'     *
	'     
		Private Shared Function btan(ByVal increment As Double) As Double
			increment /= 2.0
			Return 4.0 / 3.0 * Math.Sin(increment) / (1.0 + Math.Cos(increment))
		End Function

		''' <summary>
		''' Returns the coordinates and type of the current path segment in
		''' the iteration.
		''' The return value is the path segment type:
		''' SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		''' A float array of length 6 must be passed in and may be used to
		''' store the coordinates of the point(s).
		''' Each point is stored as a pair of float x,y coordinates.
		''' SEG_MOVETO and SEG_LINETO types will return one point,
		''' SEG_QUADTO will return two points,
		''' SEG_CUBICTO will return 3 points
		''' and SEG_CLOSE will not return any points. </summary>
		''' <seealso cref= #SEG_MOVETO </seealso>
		''' <seealso cref= #SEG_LINETO </seealso>
		''' <seealso cref= #SEG_QUADTO </seealso>
		''' <seealso cref= #SEG_CUBICTO </seealso>
		''' <seealso cref= #SEG_CLOSE </seealso>
		Public Overridable Function currentSegment(ByVal coords As Single()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("arc iterator out of bounds")
			Dim angle As Double = angStRad
			If index = 0 Then
				coords(0) = CSng(x + Math.Cos(angle) * w)
				coords(1) = CSng(y + Math.Sin(angle) * h)
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_MOVETO
			End If
			If index > arcSegs Then
				If index = arcSegs + lineSegs Then Return SEG_CLOSE
				coords(0) = CSng(x)
				coords(1) = CSng(y)
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_LINETO
			End If
			angle += increment * (index - 1)
			Dim relx As Double = Math.Cos(angle)
			Dim rely As Double = Math.Sin(angle)
			coords(0) = CSng(x + (relx - cv * rely) * w)
			coords(1) = CSng(y + (rely + cv * relx) * h)
			angle += increment
			relx = Math.Cos(angle)
			rely = Math.Sin(angle)
			coords(2) = CSng(x + (relx + cv * rely) * w)
			coords(3) = CSng(y + (rely - cv * relx) * h)
			coords(4) = CSng(x + relx * w)
			coords(5) = CSng(y + rely * h)
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 3)
			Return SEG_CUBICTO
		End Function

		''' <summary>
		''' Returns the coordinates and type of the current path segment in
		''' the iteration.
		''' The return value is the path segment type:
		''' SEG_MOVETO, SEG_LINETO, SEG_QUADTO, SEG_CUBICTO, or SEG_CLOSE.
		''' A double array of length 6 must be passed in and may be used to
		''' store the coordinates of the point(s).
		''' Each point is stored as a pair of double x,y coordinates.
		''' SEG_MOVETO and SEG_LINETO types will return one point,
		''' SEG_QUADTO will return two points,
		''' SEG_CUBICTO will return 3 points
		''' and SEG_CLOSE will not return any points. </summary>
		''' <seealso cref= #SEG_MOVETO </seealso>
		''' <seealso cref= #SEG_LINETO </seealso>
		''' <seealso cref= #SEG_QUADTO </seealso>
		''' <seealso cref= #SEG_CUBICTO </seealso>
		''' <seealso cref= #SEG_CLOSE </seealso>
		Public Overridable Function currentSegment(ByVal coords As Double()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("arc iterator out of bounds")
			Dim angle As Double = angStRad
			If index = 0 Then
				coords(0) = x + Math.Cos(angle) * w
				coords(1) = y + Math.Sin(angle) * h
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_MOVETO
			End If
			If index > arcSegs Then
				If index = arcSegs + lineSegs Then Return SEG_CLOSE
				coords(0) = x
				coords(1) = y
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_LINETO
			End If
			angle += increment * (index - 1)
			Dim relx As Double = Math.Cos(angle)
			Dim rely As Double = Math.Sin(angle)
			coords(0) = x + (relx - cv * rely) * w
			coords(1) = y + (rely + cv * relx) * h
			angle += increment
			relx = Math.Cos(angle)
			rely = Math.Sin(angle)
			coords(2) = x + (relx + cv * rely) * w
			coords(3) = y + (rely - cv * relx) * h
			coords(4) = x + relx * w
			coords(5) = y + rely * h
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 3)
			Return SEG_CUBICTO
		End Function
	End Class

End Namespace