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
	''' A utility class to iterate over the path segments of an ellipse
	''' through the PathIterator interface.
	''' 
	''' @author      Jim Graham
	''' </summary>
	Friend Class EllipseIterator
		Implements PathIterator

		Friend x, y, w, h As Double
		Friend affine As AffineTransform
		Friend index As Integer

		Friend Sub New(ByVal e As Ellipse2D, ByVal at As AffineTransform)
			Me.x = e.x
			Me.y = e.y
			Me.w = e.width
			Me.h = e.height
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

		' ArcIterator.btan(Math.PI/2)
		Public Const CtrlVal As Double = 0.5522847498307933

	'    
	'     * ctrlpts contains the control points for a set of 4 cubic
	'     * bezier curves that approximate a circle of radius 0.5
	'     * centered at 0.5, 0.5
	'     
		Private Shared ReadOnly pcv As Double = 0.5 + CtrlVal * 0.5
		Private Shared ReadOnly ncv As Double = 0.5 - CtrlVal * 0.5
		Private Shared ctrlpts As Double()() = { New Double() { 1.0, pcv, pcv, 1.0, 0.5, 1.0 }, New Double() { ncv, 1.0, 0.0, pcv, 0.0, 0.5 }, New Double() { 0.0, ncv, ncv, 0.0, 0.5, 0.0 }, New Double() { pcv, 0.0, 1.0, ncv, 1.0, 0.5 } }

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
			If done Then Throw New NoSuchElementException("ellipse iterator out of bounds")
			If index = 5 Then Return SEG_CLOSE
			If index = 0 Then
				Dim ctrls As Double() = ctrlpts(3)
				coords(0) = CSng(x + ctrls(4) * w)
				coords(1) = CSng(y + ctrls(5) * h)
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_MOVETO
			End If
			Dim ctrls As Double() = ctrlpts(index - 1)
			coords(0) = CSng(x + ctrls(0) * w)
			coords(1) = CSng(y + ctrls(1) * h)
			coords(2) = CSng(x + ctrls(2) * w)
			coords(3) = CSng(y + ctrls(3) * h)
			coords(4) = CSng(x + ctrls(4) * w)
			coords(5) = CSng(y + ctrls(5) * h)
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
			If done Then Throw New NoSuchElementException("ellipse iterator out of bounds")
			If index = 5 Then Return SEG_CLOSE
			If index = 0 Then
				Dim ctrls As Double() = ctrlpts(3)
				coords(0) = x + ctrls(4) * w
				coords(1) = y + ctrls(5) * h
				If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 1)
				Return SEG_MOVETO
			End If
			Dim ctrls As Double() = ctrlpts(index - 1)
			coords(0) = x + ctrls(0) * w
			coords(1) = y + ctrls(1) * h
			coords(2) = x + ctrls(2) * w
			coords(3) = y + ctrls(3) * h
			coords(4) = x + ctrls(4) * w
			coords(5) = y + ctrls(5) * h
			If affine IsNot Nothing Then affine.transform(coords, 0, coords, 0, 3)
			Return SEG_CUBICTO
		End Function
	End Class

End Namespace