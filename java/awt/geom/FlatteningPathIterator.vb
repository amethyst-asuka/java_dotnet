Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>FlatteningPathIterator</code> class returns a flattened view of
	''' another <seealso cref="PathIterator"/> object.  Other <seealso cref="java.awt.Shape Shape"/>
	''' classes can use this class to provide flattening behavior for their paths
	''' without having to perform the interpolation calculations themselves.
	''' 
	''' @author Jim Graham
	''' </summary>
	Public Class FlatteningPathIterator
		Implements PathIterator

		Friend Const GROW_SIZE As Integer = 24 ' Multiple of cubic & quad curve size

		Friend src As PathIterator ' The source iterator

		Friend squareflat As Double ' Square of the flatness parameter
											' for testing against squared lengths

		Friend limit As Integer ' Maximum number of recursion levels

		Friend hold As Double() = New Double(13){} ' The cache of interpolated coords
											' Note that this must be long enough
											' to store a full cubic segment and
											' a relative cubic segment to avoid
											' aliasing when copying the coords
											' of a curve to the end of the array.
											' This is also serendipitously equal
											' to the size of a full quad segment
											' and 2 relative quad segments.

		Friend curx, cury As Double ' The ending x,y of the last segment

		Friend movx, movy As Double ' The x,y of the last move segment

		Friend holdType As Integer ' The type of the curve being held
											' for interpolation

		Friend holdEnd As Integer ' The index of the last curve segment
											' being held for interpolation

		Friend holdIndex As Integer ' The index of the curve segment
											' that was last interpolated.  This
											' is the curve segment ready to be
											' returned in the next call to
											' currentSegment().

		Friend levels As Integer() ' The recursion level at which
											' each curve being held in storage
											' was generated.

		Friend levelIndex As Integer ' The index of the entry in the
											' levels array of the curve segment
											' at the holdIndex

		Friend done As Boolean ' True when iteration is done

		''' <summary>
		''' Constructs a new <code>FlatteningPathIterator</code> object that
		''' flattens a path as it iterates over it.  The iterator does not
		''' subdivide any curve read from the source iterator to more than
		''' 10 levels of subdivision which yields a maximum of 1024 line
		''' segments per curve. </summary>
		''' <param name="src"> the original unflattened path being iterated over </param>
		''' <param name="flatness"> the maximum allowable distance between the
		''' control points and the flattened curve </param>
		Public Sub New(ByVal src As PathIterator, ByVal flatness As Double)
			Me.New(src, flatness, 10)
		End Sub

		''' <summary>
		''' Constructs a new <code>FlatteningPathIterator</code> object
		''' that flattens a path as it iterates over it.
		''' The <code>limit</code> parameter allows you to control the
		''' maximum number of recursive subdivisions that the iterator
		''' can make before it assumes that the curve is flat enough
		''' without measuring against the <code>flatness</code> parameter.
		''' The flattened iteration therefore never generates more than
		''' a maximum of <code>(2^limit)</code> line segments per curve. </summary>
		''' <param name="src"> the original unflattened path being iterated over </param>
		''' <param name="flatness"> the maximum allowable distance between the
		''' control points and the flattened curve </param>
		''' <param name="limit"> the maximum number of recursive subdivisions
		''' allowed for any curved segment </param>
		''' <exception cref="IllegalArgumentException"> if
		'''          <code>flatness</code> or <code>limit</code>
		'''          is less than zero </exception>
		Public Sub New(ByVal src As PathIterator, ByVal flatness As Double, ByVal limit As Integer)
			If flatness < 0.0 Then Throw New IllegalArgumentException("flatness must be >= 0")
			If limit < 0 Then Throw New IllegalArgumentException("limit must be >= 0")
			Me.src = src
			Me.squareflat = flatness * flatness
			Me.limit = limit
			Me.levels = New Integer(limit){}
			' prime the first path segment
			[next](False)
		End Sub

		''' <summary>
		''' Returns the flatness of this iterator. </summary>
		''' <returns> the flatness of this <code>FlatteningPathIterator</code>. </returns>
		Public Overridable Property flatness As Double
			Get
				Return System.Math.Sqrt(squareflat)
			End Get
		End Property

		''' <summary>
		''' Returns the recursion limit of this iterator. </summary>
		''' <returns> the recursion limit of this
		''' <code>FlatteningPathIterator</code>. </returns>
		Public Overridable Property recursionLimit As Integer
			Get
				Return limit
			End Get
		End Property

		''' <summary>
		''' Returns the winding rule for determining the interior of the
		''' path. </summary>
		''' <returns> the winding rule of the original unflattened path being
		''' iterated over. </returns>
		''' <seealso cref= PathIterator#WIND_EVEN_ODD </seealso>
		''' <seealso cref= PathIterator#WIND_NON_ZERO </seealso>
		Public Overridable Property windingRule As Integer Implements PathIterator.getWindingRule
			Get
				Return src.windingRule
			End Get
		End Property

		''' <summary>
		''' Tests if the iteration is complete. </summary>
		''' <returns> <code>true</code> if all the segments have
		''' been read; <code>false</code> otherwise. </returns>
		Public Overridable Property done As Boolean Implements PathIterator.isDone
			Get
				Return done
			End Get
		End Property

	'    
	'     * Ensures that the hold array can hold up to (want) more values.
	'     * It is currently holding (hold.length - holdIndex) values.
	'     
		Friend Overridable Sub ensureHoldCapacity(ByVal want As Integer)
			If holdIndex - want < 0 Then
				Dim have As Integer = hold.Length - holdIndex
				Dim newsize As Integer = hold.Length + GROW_SIZE
				Dim newhold As Double() = New Double(newsize - 1){}
				Array.Copy(hold, holdIndex, newhold, holdIndex + GROW_SIZE, have)
				hold = newhold
				holdIndex += GROW_SIZE
				holdEnd += GROW_SIZE
			End If
		End Sub

		''' <summary>
		''' Moves the iterator to the next segment of the path forwards
		''' along the primary direction of traversal as long as there are
		''' more points in that direction.
		''' </summary>
		Public Overridable Sub [next]() Implements PathIterator.next
			[next](True)
		End Sub

		Private Sub [next](ByVal doNext As Boolean)
			Dim level As Integer

			If holdIndex >= holdEnd Then
				If doNext Then src.next()
				If src.done Then
					done = True
					Return
				End If
				holdType = src.currentSegment(hold)
				levelIndex = 0
				levels(0) = 0
			End If

			Select Case holdType
			Case SEG_MOVETO, SEG_LINETO
				curx = hold(0)
				cury = hold(1)
				If holdType = SEG_MOVETO Then
					movx = curx
					movy = cury
				End If
				holdIndex = 0
				holdEnd = 0
			Case SEG_CLOSE
				curx = movx
				cury = movy
				holdIndex = 0
				holdEnd = 0
			Case SEG_QUADTO
				If holdIndex >= holdEnd Then
					' Move the coordinates to the end of the array.
					holdIndex = hold.Length - 6
					holdEnd = hold.Length - 2
					hold(holdIndex + 0) = curx
					hold(holdIndex + 1) = cury
					hold(holdIndex + 2) = hold(0)
					hold(holdIndex + 3) = hold(1)
						curx = hold(2)
						hold(holdIndex + 4) = curx
						cury = hold(3)
						hold(holdIndex + 5) = cury
				End If

				level = levels(levelIndex)
				Do While level < limit
					If QuadCurve2D.getFlatnessSq(hold, holdIndex) < squareflat Then Exit Do

					ensureHoldCapacity(4)
					QuadCurve2D.subdivide(hold, holdIndex, hold, holdIndex - 4, hold, holdIndex)
					holdIndex -= 4

					' Now that we have subdivided, we have constructed
					' two curves of one depth lower than the original
					' curve.  One of those curves is in the place of
					' the former curve and one of them is in the next
					' set of held coordinate slots.  We now set both
					' curves level values to the next higher level.
					level += 1
					levels(levelIndex) = level
					levelIndex += 1
					levels(levelIndex) = level
				Loop

				' This curve segment is flat enough, or it is too deep
				' in recursion levels to try to flatten any more.  The
				' two coordinates at holdIndex+4 and holdIndex+5 now
				' contain the endpoint of the curve which can be the
				' endpoint of an approximating line segment.
				holdIndex += 4
				levelIndex -= 1
			Case SEG_CUBICTO
				If holdIndex >= holdEnd Then
					' Move the coordinates to the end of the array.
					holdIndex = hold.Length - 8
					holdEnd = hold.Length - 2
					hold(holdIndex + 0) = curx
					hold(holdIndex + 1) = cury
					hold(holdIndex + 2) = hold(0)
					hold(holdIndex + 3) = hold(1)
					hold(holdIndex + 4) = hold(2)
					hold(holdIndex + 5) = hold(3)
						curx = hold(4)
						hold(holdIndex + 6) = curx
						cury = hold(5)
						hold(holdIndex + 7) = cury
				End If

				level = levels(levelIndex)
				Do While level < limit
					If CubicCurve2D.getFlatnessSq(hold, holdIndex) < squareflat Then Exit Do

					ensureHoldCapacity(6)
					CubicCurve2D.subdivide(hold, holdIndex, hold, holdIndex - 6, hold, holdIndex)
					holdIndex -= 6

					' Now that we have subdivided, we have constructed
					' two curves of one depth lower than the original
					' curve.  One of those curves is in the place of
					' the former curve and one of them is in the next
					' set of held coordinate slots.  We now set both
					' curves level values to the next higher level.
					level += 1
					levels(levelIndex) = level
					levelIndex += 1
					levels(levelIndex) = level
				Loop

				' This curve segment is flat enough, or it is too deep
				' in recursion levels to try to flatten any more.  The
				' two coordinates at holdIndex+6 and holdIndex+7 now
				' contain the endpoint of the curve which can be the
				' endpoint of an approximating line segment.
				holdIndex += 6
				levelIndex -= 1
			End Select
		End Sub

		''' <summary>
		''' Returns the coordinates and type of the current path segment in
		''' the iteration.
		''' The return value is the path segment type:
		''' SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
		''' A float array of length 6 must be passed in and can be used to
		''' store the coordinates of the point(s).
		''' Each point is stored as a pair of float x,y coordinates.
		''' SEG_MOVETO and SEG_LINETO types return one point,
		''' and SEG_CLOSE does not return any points. </summary>
		''' <param name="coords"> an array that holds the data returned from
		''' this method </param>
		''' <returns> the path segment type of the current path segment. </returns>
		''' <exception cref="NoSuchElementException"> if there
		'''          are no more elements in the flattening path to be
		'''          returned. </exception>
		''' <seealso cref= PathIterator#SEG_MOVETO </seealso>
		''' <seealso cref= PathIterator#SEG_LINETO </seealso>
		''' <seealso cref= PathIterator#SEG_CLOSE </seealso>
		Public Overridable Function currentSegment(ByVal coords As Single()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("flattening iterator out of bounds")
			Dim type As Integer = holdType
			If type <> SEG_CLOSE Then
				coords(0) = CSng(hold(holdIndex + 0))
				coords(1) = CSng(hold(holdIndex + 1))
				If type <> SEG_MOVETO Then type = SEG_LINETO
			End If
			Return type
		End Function

		''' <summary>
		''' Returns the coordinates and type of the current path segment in
		''' the iteration.
		''' The return value is the path segment type:
		''' SEG_MOVETO, SEG_LINETO, or SEG_CLOSE.
		''' A double array of length 6 must be passed in and can be used to
		''' store the coordinates of the point(s).
		''' Each point is stored as a pair of double x,y coordinates.
		''' SEG_MOVETO and SEG_LINETO types return one point,
		''' and SEG_CLOSE does not return any points. </summary>
		''' <param name="coords"> an array that holds the data returned from
		''' this method </param>
		''' <returns> the path segment type of the current path segment. </returns>
		''' <exception cref="NoSuchElementException"> if there
		'''          are no more elements in the flattening path to be
		'''          returned. </exception>
		''' <seealso cref= PathIterator#SEG_MOVETO </seealso>
		''' <seealso cref= PathIterator#SEG_LINETO </seealso>
		''' <seealso cref= PathIterator#SEG_CLOSE </seealso>
		Public Overridable Function currentSegment(ByVal coords As Double()) As Integer Implements PathIterator.currentSegment
			If done Then Throw New NoSuchElementException("flattening iterator out of bounds")
			Dim type As Integer = holdType
			If type <> SEG_CLOSE Then
				coords(0) = hold(holdIndex + 0)
				coords(1) = hold(holdIndex + 1)
				If type <> SEG_MOVETO Then type = SEG_LINETO
			End If
			Return type
		End Function
	End Class

End Namespace