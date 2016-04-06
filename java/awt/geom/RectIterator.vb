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
	''' A utility class to iterate over the path segments of a rectangle
	''' through the PathIterator interface.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Friend Class RectIterator
		Implements PathIterator

		Friend x, y, w, h As Double
		Friend affine As AffineTransform
		Friend index As Integer

		Friend Sub New(  r As Rectangle2D,   at As AffineTransform)
			Me.x = r.x
			Me.y = r.y
			Me.w = r.width
			Me.h = r.height
			Me.affine = at
			If w < 0 OrElse h < 0 Then index = 6
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
				Return index > 5
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
		Public Overridable Function currentSegment(  coords As Single()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("rect iterator out of bounds")
			If index = 5 Then Return SEG_CLOSE
			coords(0) = CSng(x)
			coords(1) = CSng(y)
			If index = 1 OrElse index = 2 Then coords(0) += CSng(w)
			If index = 2 OrElse index = 3 Then coords(1) += CSng(h)
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
			Return (If(index = 0, SEG_MOVETO, SEG_LINETO))
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
		Public Overridable Function currentSegment(  coords As Double()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("rect iterator out of bounds")
			If index = 5 Then Return SEG_CLOSE
			coords(0) = x
			coords(1) = y
			If index = 1 OrElse index = 2 Then coords(0) += w
			If index = 2 OrElse index = 3 Then coords(1) += h
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
			Return (If(index = 0, SEG_MOVETO, SEG_LINETO))
		End Function
	End Class

End Namespace