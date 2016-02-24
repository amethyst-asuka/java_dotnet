Imports System

'
' * Copyright (c) 1997, Oracle and/or its affiliates. All rights reserved.
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
	''' A utility class to iterate over the path segments of an rounded rectangle
	''' through the PathIterator interface.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Friend Class RoundRectIterator
		Implements PathIterator

		Friend x, y, w, h, aw, ah As Double
		Friend affine As AffineTransform
		Friend index As Integer

		Friend Sub New(ByVal rr As RoundRectangle2D, ByVal at As AffineTransform)
			Me.x = rr.x
			Me.y = rr.y
			Me.w = rr.width
			Me.h = rr.height
			Me.aw = Math.Min(w, Math.Abs(rr.arcWidth))
			Me.ah = Math.Min(h, Math.Abs(rr.arcHeight))
			Me.affine = at
			If aw < 0 OrElse ah < 0 Then index = ctrlpts.Length
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
				Return index >= ctrlpts.Length
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

		Private Shared ReadOnly angle As Double = Math.PI / 4.0
		Private Shared ReadOnly a As Double = 1.0 - Math.cos(angle)
		Private Shared ReadOnly b As Double = Math.tan(angle)
		Private Shared ReadOnly c As Double = Math.sqrt(1.0 + b * b) - 1 + a
		Private Shared ReadOnly cv As Double = 4.0 / 3.0 * a * b / c
		Private Shared ReadOnly acv As Double = (1.0 - cv) / 2.0

		' For each array:
		'     4 values for each point {v0, v1, v2, v3}:
		'         point = (x + v0 * w + v1 * arcWidth,
		'                  y + v2 * h + v3 * arcHeight);
		Private Shared ctrlpts As Double()() = { New Double() { 0.0, 0.0, 0.0, 0.5 }, New Double() { 0.0, 0.0, 1.0, -0.5 }, New Double() { 0.0, 0.0, 1.0, -acv, 0.0, acv, 1.0, 0.0, 0.0, 0.5, 1.0, 0.0 }, New Double() { 1.0, -0.5, 1.0, 0.0 }, New Double() { 1.0, -acv, 1.0, 0.0, 1.0, 0.0, 1.0, -acv, 1.0, 0.0, 1.0, -0.5 }, New Double() { 1.0, 0.0, 0.0, 0.5 }, New Double() { 1.0, 0.0, 0.0, acv, 1.0, -acv, 0.0, 0.0, 1.0, -0.5, 0.0, 0.0 }, New Double() { 0.0, 0.5, 0.0, 0.0 }, New Double() { 0.0, acv, 0.0, 0.0, 0.0, 0.0, 0.0, acv, 0.0, 0.0, 0.0, 0.5 }, New Double() { } }
		Private Shared types As Integer() = { SEG_MOVETO, SEG_LINETO, SEG_CUBICTO, SEG_LINETO, SEG_CUBICTO, SEG_LINETO, SEG_CUBICTO, SEG_LINETO, SEG_CUBICTO, SEG_CLOSE }

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
			If done Then Throw New NoSuchElementException("roundrect iterator out of bounds")
			Dim ctrls As Double() = ctrlpts(index)
			Dim nc As Integer = 0
			For i As Integer = 0 To ctrls.Length - 1 Step 4
				coords(nc) = CSng(x + ctrls(i + 0) * w + ctrls(i + 1) * aw)
				nc += 1
				coords(nc) = CSng(y + ctrls(i + 2) * h + ctrls(i + 3) * ah)
				nc += 1
			Next i
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, nc \ 2)
			Return types(index)
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
			If done Then Throw New NoSuchElementException("roundrect iterator out of bounds")
			Dim ctrls As Double() = ctrlpts(index)
			Dim nc As Integer = 0
			For i As Integer = 0 To ctrls.Length - 1 Step 4
				coords(nc) = (x + ctrls(i + 0) * w + ctrls(i + 1) * aw)
				nc += 1
				coords(nc) = (y + ctrls(i + 2) * h + ctrls(i + 3) * ah)
				nc += 1
			Next i
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, nc \ 2)
			Return types(index)
		End Function
	End Class

End Namespace