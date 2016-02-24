'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' A utility class to iterate over the path segments of a line segment
	''' through the PathIterator interface.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Friend Class LineIterator
		Implements PathIterator

		Friend line As Line2D
		Friend affine As AffineTransform
		Friend index As Integer

		Friend Sub New(ByVal l As Line2D, ByVal at As AffineTransform)
			Me.line = l
			Me.affine = at
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
				Return (index > 1)
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
		Public Overridable Function currentSegment(ByVal coords As Single()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("line iterator out of bounds")
			Dim type As Integer
			If index = 0 Then
				coords(0) = CSng(line.x1)
				coords(1) = CSng(line.y1)
				type = SEG_MOVETO
			Else
				coords(0) = CSng(line.x2)
				coords(1) = CSng(line.y2)
				type = SEG_LINETO
			End If
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
			Return type
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
			If done Then Throw New NoSuchElementException("line iterator out of bounds")
			Dim type As Integer
			If index = 0 Then
				coords(0) = line.x1
				coords(1) = line.y1
				type = SEG_MOVETO
			Else
				coords(0) = line.x2
				coords(1) = line.y2
				type = SEG_LINETO
			End If
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
			Return type
		End Function
	End Class

End Namespace