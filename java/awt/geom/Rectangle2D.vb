Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Rectangle2D</code> class describes a rectangle
	''' defined by a location {@code (x,y)} and dimension
	''' {@code (w x h)}.
	''' <p>
	''' This class is only the abstract superclass for all objects that
	''' store a 2D rectangle.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class Rectangle2D
		Inherits RectangularShape

		''' <summary>
		''' The bitmask that indicates that a point lies to the left of
		''' this <code>Rectangle2D</code>.
		''' @since 1.2
		''' </summary>
		Public Const OUT_LEFT As Integer = 1

		''' <summary>
		''' The bitmask that indicates that a point lies above
		''' this <code>Rectangle2D</code>.
		''' @since 1.2
		''' </summary>
		Public Const OUT_TOP As Integer = 2

		''' <summary>
		''' The bitmask that indicates that a point lies to the right of
		''' this <code>Rectangle2D</code>.
		''' @since 1.2
		''' </summary>
		Public Const OUT_RIGHT As Integer = 4

		''' <summary>
		''' The bitmask that indicates that a point lies below
		''' this <code>Rectangle2D</code>.
		''' @since 1.2
		''' </summary>
		Public Const OUT_BOTTOM As Integer = 8

		''' <summary>
		''' The <code>Float</code> class defines a rectangle specified in float
		''' coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits Rectangle2D

			''' <summary>
			''' The X coordinate of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Single

			''' <summary>
			''' The Y coordinate of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Single

			''' <summary>
			''' The width of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Single

			''' <summary>
			''' The height of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Single

			''' <summary>
			''' Constructs a new <code>Rectangle2D</code>, initialized to
			''' location (0.0,&nbsp;0.0) and size (0.0,&nbsp;0.0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes a <code>Rectangle2D</code>
			''' from the specified <code>float</code> coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the upper-left corner
			'''          of the newly constructed <code>Rectangle2D</code> </param>
			''' <param name="y"> the Y coordinate of the upper-left corner
			'''          of the newly constructed <code>Rectangle2D</code> </param>
			''' <param name="w"> the width of the newly constructed
			'''          <code>Rectangle2D</code> </param>
			''' <param name="h"> the height of the newly constructed
			'''          <code>Rectangle2D</code>
			''' @since 1.2 </param>
			Public Sub New(  x As Single,   y As Single,   w As Single,   h As Single)
				rectect(x, y, w, h)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  x As Double
				Get
					Return CDbl(x)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  y As Double
				Get
					Return CDbl(y)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  width As Double
				Get
					Return CDbl(width)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  height As Double
				Get
					Return CDbl(height)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  empty As Boolean
				Get
					Return (width <= 0.0f) OrElse (height <= 0.0f)
				End Get
			End Property

			''' <summary>
			''' Sets the location and size of this <code>Rectangle2D</code>
			''' to the specified <code>float</code> values.
			''' </summary>
			''' <param name="x"> the X coordinate of the upper-left corner
			'''          of this <code>Rectangle2D</code> </param>
			''' <param name="y"> the Y coordinate of the upper-left corner
			'''          of this <code>Rectangle2D</code> </param>
			''' <param name="w"> the width of this <code>Rectangle2D</code> </param>
			''' <param name="h"> the height of this <code>Rectangle2D</code>
			''' @since 1.2 </param>
			Public Overridable Sub setRect(  x As Single,   y As Single,   w As Single,   h As Single)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setRect(  x As Double,   y As Double,   w As Double,   h As Double)
				Me.x = CSng(x)
				Me.y = CSng(y)
				Me.width = CSng(w)
				Me.height = CSng(h)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Property rect As Rectangle2D
				Set(  r As Rectangle2D)
					Me.x = CSng(r.x)
					Me.y = CSng(r.y)
					Me.width = CSng(r.width)
					Me.height = CSng(r.height)
				End Set
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function outcode(  x As Double,   y As Double) As Integer
	'            
	'             * Note on casts to double below.  If the arithmetic of
	'             * x+w or y+h is done in float, then some bits may be
	'             * lost if the binary exponents of x/y and w/h are not
	'             * similar.  By converting to double before the addition
	'             * we force the addition to be carried out in double to
	'             * avoid rounding error in the comparison.
	'             *
	'             * See bug 4320890 for problems that this inaccuracy causes.
	'             
				Dim out As Integer = 0
				If Me.width <= 0 Then
					out = out Or OUT_LEFT Or OUT_RIGHT
				ElseIf x < Me.x Then
					out = out Or OUT_LEFT
				ElseIf x > Me.x + CDbl(Me.width) Then
					out = out Or OUT_RIGHT
				End If
				If Me.height <= 0 Then
					out = out Or OUT_TOP Or OUT_BOTTOM
				ElseIf y < Me.y Then
					out = out Or OUT_TOP
				ElseIf y > Me.y + CDbl(Me.height) Then
					out = out Or OUT_BOTTOM
				End If
				Return out
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
				Get
					Return New Float(x, y, width, height)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function createIntersection(  r As Rectangle2D) As Rectangle2D
				Dim dest As Rectangle2D
				If TypeOf r Is Float Then
					dest = New Rectangle2D.Float
				Else
					dest = New Rectangle2D.Double
				End If
				Rectangle2D.intersect(Me, r, dest)
				Return dest
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function createUnion(  r As Rectangle2D) As Rectangle2D
				Dim dest As Rectangle2D
				If TypeOf r Is Float Then
					dest = New Rectangle2D.Float
				Else
					dest = New Rectangle2D.Double
				End If
				Rectangle2D.union(Me, r, dest)
				Return dest
			End Function

			''' <summary>
			''' Returns the <code>String</code> representation of this
			''' <code>Rectangle2D</code>. </summary>
			''' <returns> a <code>String</code> representing this
			''' <code>Rectangle2D</code>.
			''' @since 1.2 </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[x=" & x & ",y=" & y & ",w=" & width & ",h=" & height & "]"
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 3798716824173675777L
		End Class

		''' <summary>
		''' The <code>Double</code> class defines a rectangle specified in
		''' double coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits Rectangle2D

			''' <summary>
			''' The X coordinate of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Double

			''' <summary>
			''' The Y coordinate of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Double

			''' <summary>
			''' The width of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Double

			''' <summary>
			''' The height of this <code>Rectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Double

			''' <summary>
			''' Constructs a new <code>Rectangle2D</code>, initialized to
			''' location (0,&nbsp;0) and size (0,&nbsp;0).
			''' @since 1.2
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
			End Function

			''' <summary>
			''' Constructs and initializes a <code>Rectangle2D</code>
			''' from the specified <code>double</code> coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the upper-left corner
			'''          of the newly constructed <code>Rectangle2D</code> </param>
			''' <param name="y"> the Y coordinate of the upper-left corner
			'''          of the newly constructed <code>Rectangle2D</code> </param>
			''' <param name="w"> the width of the newly constructed
			'''          <code>Rectangle2D</code> </param>
			''' <param name="h"> the height of the newly constructed
			'''          <code>Rectangle2D</code>
			''' @since 1.2 </param>
			Function java.lang.Double(  x As Double,   y As Double,   w As Double,   h As Double) As [Public] Overridable
				rectect(x, y, w, h)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  x As Double
				Get
					Return x
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  y As Double
				Get
					Return y
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  width As Double
				Get
					Return width
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  height As Double
				Get
					Return height
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  empty As Boolean
				Get
					Return (width <= 0.0) OrElse (height <= 0.0)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setRect(  x As Double,   y As Double,   w As Double,   h As Double)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Property rect As Rectangle2D
				Set(  r As Rectangle2D)
					Me.x = r.x
					Me.y = r.y
					Me.width = r.width
					Me.height = r.height
				End Set
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function outcode(  x As Double,   y As Double) As Integer
				Dim out As Integer = 0
				If Me.width <= 0 Then
					out = out Or OUT_LEFT Or OUT_RIGHT
				ElseIf x < Me.x Then
					out = out Or OUT_LEFT
				ElseIf x > Me.x + Me.width Then
					out = out Or OUT_RIGHT
				End If
				If Me.height <= 0 Then
					out = out Or OUT_TOP Or OUT_BOTTOM
				ElseIf y < Me.y Then
					out = out Or OUT_TOP
				ElseIf y > Me.y + Me.height Then
					out = out Or OUT_BOTTOM
				End If
				Return out
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
				Get
					Return New Double?(x, y, width, height)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function createIntersection(  r As Rectangle2D) As Rectangle2D
				Dim dest As Rectangle2D = New Rectangle2D.Double
				Rectangle2D.intersect(Me, r, dest)
				Return dest
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Function createUnion(  r As Rectangle2D) As Rectangle2D
				Dim dest As Rectangle2D = New Rectangle2D.Double
				Rectangle2D.union(Me, r, dest)
				Return dest
			End Function

			''' <summary>
			''' Returns the <code>String</code> representation of this
			''' <code>Rectangle2D</code>. </summary>
			''' <returns> a <code>String</code> representing this
			''' <code>Rectangle2D</code>.
			''' @since 1.2 </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[x=" & x & ",y=" & y & ",w=" & width & ",h=" & height & "]"
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 7771313791441850493L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.Rectangle2D.Float </seealso>
		''' <seealso cref= java.awt.geom.Rectangle2D.Double </seealso>
		''' <seealso cref= java.awt.Rectangle
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Sets the location and size of this <code>Rectangle2D</code>
		''' to the specified <code>double</code> values.
		''' </summary>
		''' <param name="x"> the X coordinate of the upper-left corner
		'''          of this <code>Rectangle2D</code> </param>
		''' <param name="y"> the Y coordinate of the upper-left corner
		'''          of this <code>Rectangle2D</code> </param>
		''' <param name="w"> the width of this <code>Rectangle2D</code> </param>
		''' <param name="h"> the height of this <code>Rectangle2D</code>
		''' @since 1.2 </param>
		Public MustOverride Sub setRect(  x As Double,   y As Double,   w As Double,   h As Double)

		''' <summary>
		''' Sets this <code>Rectangle2D</code> to be the same as the specified
		''' <code>Rectangle2D</code>. </summary>
		''' <param name="r"> the specified <code>Rectangle2D</code>
		''' @since 1.2 </param>
		Public Overridable Property rect As Rectangle2D
			Set(  r As Rectangle2D)
				rectect(r.x, r.y, r.width, r.height)
			End Set
		End Property

		''' <summary>
		''' Tests if the specified line segment intersects the interior of this
		''' <code>Rectangle2D</code>.
		''' </summary>
		''' <param name="x1"> the X coordinate of the start point of the specified
		'''           line segment </param>
		''' <param name="y1"> the Y coordinate of the start point of the specified
		'''           line segment </param>
		''' <param name="x2"> the X coordinate of the end point of the specified
		'''           line segment </param>
		''' <param name="y2"> the Y coordinate of the end point of the specified
		'''           line segment </param>
		''' <returns> <code>true</code> if the specified line segment intersects
		''' the interior of this <code>Rectangle2D</code>; <code>false</code>
		''' otherwise.
		''' @since 1.2 </returns>
		Public Overridable Function intersectsLine(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double) As Boolean
			Dim out1, out2 As Integer
			out2 = outcode(x2, y2)
			If out2 = 0 Then Return True
			out1 = outcode(x1, y1)
			Do While out1 <> 0
				If (out1 And out2) <> 0 Then Return False
				If (out1 And (OUT_LEFT Or OUT_RIGHT)) <> 0 Then
					Dim x_Renamed As Double = x
					If (out1 And OUT_RIGHT) <> 0 Then x_Renamed += width
					y1 = y1 + (x_Renamed - x1) * (y2 - y1) / (x2 - x1)
					x1 = x_Renamed
				Else
					Dim y_Renamed As Double = y
					If (out1 And OUT_BOTTOM) <> 0 Then y_Renamed += height
					x1 = x1 + (y_Renamed - y1) * (x2 - x1) / (y2 - y1)
					y1 = y_Renamed
				End If
				out1 = outcode(x1, y1)
			Loop
			Return True
		End Function

		''' <summary>
		''' Tests if the specified line segment intersects the interior of this
		''' <code>Rectangle2D</code>. </summary>
		''' <param name="l"> the specified <seealso cref="Line2D"/> to test for intersection
		''' with the interior of this <code>Rectangle2D</code> </param>
		''' <returns> <code>true</code> if the specified <code>Line2D</code>
		''' intersects the interior of this <code>Rectangle2D</code>;
		''' <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overridable Function intersectsLine(  l As Line2D) As Boolean
			Return intersectsLine(l.x1, l.y1, l.x2, l.y2)
		End Function

		''' <summary>
		''' Determines where the specified coordinates lie with respect
		''' to this <code>Rectangle2D</code>.
		''' This method computes a binary OR of the appropriate mask values
		''' indicating, for each side of this <code>Rectangle2D</code>,
		''' whether or not the specified coordinates are on the same side
		''' of the edge as the rest of this <code>Rectangle2D</code>. </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <returns> the logical OR of all appropriate out codes. </returns>
		''' <seealso cref= #OUT_LEFT </seealso>
		''' <seealso cref= #OUT_TOP </seealso>
		''' <seealso cref= #OUT_RIGHT </seealso>
		''' <seealso cref= #OUT_BOTTOM
		''' @since 1.2 </seealso>
		Public MustOverride Function outcode(  x As Double,   y As Double) As Integer

		''' <summary>
		''' Determines where the specified <seealso cref="Point2D"/> lies with
		''' respect to this <code>Rectangle2D</code>.
		''' This method computes a binary OR of the appropriate mask values
		''' indicating, for each side of this <code>Rectangle2D</code>,
		''' whether or not the specified <code>Point2D</code> is on the same
		''' side of the edge as the rest of this <code>Rectangle2D</code>. </summary>
		''' <param name="p"> the specified <code>Point2D</code> </param>
		''' <returns> the logical OR of all appropriate out codes. </returns>
		''' <seealso cref= #OUT_LEFT </seealso>
		''' <seealso cref= #OUT_TOP </seealso>
		''' <seealso cref= #OUT_RIGHT </seealso>
		''' <seealso cref= #OUT_BOTTOM
		''' @since 1.2 </seealso>
		Public Overridable Function outcode(  p As Point2D) As Integer
			Return outcode(p.x, p.y)
		End Function

		''' <summary>
		''' Sets the location and size of the outer bounds of this
		''' <code>Rectangle2D</code> to the specified rectangular values.
		''' </summary>
		''' <param name="x"> the X coordinate of the upper-left corner
		'''          of this <code>Rectangle2D</code> </param>
		''' <param name="y"> the Y coordinate of the upper-left corner
		'''          of this <code>Rectangle2D</code> </param>
		''' <param name="w"> the width of this <code>Rectangle2D</code> </param>
		''' <param name="h"> the height of this <code>Rectangle2D</code>
		''' @since 1.2 </param>
		Public Overrides Sub setFrame(  x As Double,   y As Double,   w As Double,   h As Double)
			rectect(x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
			Get
				Return CType(clone(), Rectangle2D)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(  x As Double,   y As Double) As Boolean
			Dim x0 As Double = x
			Dim y0 As Double = y
			Return (x >= x0 AndAlso y >= y0 AndAlso x < x0 + width AndAlso y < y0 + height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function intersects(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If empty OrElse w <= 0 OrElse h <= 0 Then Return False
			Dim x0 As Double = x
			Dim y0 As Double = y
			Return (x + w > x0 AndAlso y + h > y0 AndAlso x < x0 + width AndAlso y < y0 + height)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
			If empty OrElse w <= 0 OrElse h <= 0 Then Return False
			Dim x0 As Double = x
			Dim y0 As Double = y
			Return (x >= x0 AndAlso y >= y0 AndAlso (x + w) <= x0 + width AndAlso (y + h) <= y0 + height)
		End Function

		''' <summary>
		''' Returns a new <code>Rectangle2D</code> object representing the
		''' intersection of this <code>Rectangle2D</code> with the specified
		''' <code>Rectangle2D</code>. </summary>
		''' <param name="r"> the <code>Rectangle2D</code> to be intersected with
		''' this <code>Rectangle2D</code> </param>
		''' <returns> the largest <code>Rectangle2D</code> contained in both
		'''          the specified <code>Rectangle2D</code> and in this
		'''          <code>Rectangle2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride Function createIntersection(  r As Rectangle2D) As Rectangle2D

		''' <summary>
		''' Intersects the pair of specified source <code>Rectangle2D</code>
		''' objects and puts the result into the specified destination
		''' <code>Rectangle2D</code> object.  One of the source rectangles
		''' can also be the destination to avoid creating a third Rectangle2D
		''' object, but in this case the original points of this source
		''' rectangle will be overwritten by this method. </summary>
		''' <param name="src1"> the first of a pair of <code>Rectangle2D</code>
		''' objects to be intersected with each other </param>
		''' <param name="src2"> the second of a pair of <code>Rectangle2D</code>
		''' objects to be intersected with each other </param>
		''' <param name="dest"> the <code>Rectangle2D</code> that holds the
		''' results of the intersection of <code>src1</code> and
		''' <code>src2</code>
		''' @since 1.2 </param>
		Public Shared Sub intersect(  src1 As Rectangle2D,   src2 As Rectangle2D,   dest As Rectangle2D)
			Dim x1 As Double = System.Math.Max(src1.minX, src2.minX)
			Dim y1 As Double = System.Math.Max(src1.minY, src2.minY)
			Dim x2 As Double = System.Math.Min(src1.maxX, src2.maxX)
			Dim y2 As Double = System.Math.Min(src1.maxY, src2.maxY)
			dest.frameame(x1, y1, x2-x1, y2-y1)
		End Sub

		''' <summary>
		''' Returns a new <code>Rectangle2D</code> object representing the
		''' union of this <code>Rectangle2D</code> with the specified
		''' <code>Rectangle2D</code>. </summary>
		''' <param name="r"> the <code>Rectangle2D</code> to be combined with
		''' this <code>Rectangle2D</code> </param>
		''' <returns> the smallest <code>Rectangle2D</code> containing both
		''' the specified <code>Rectangle2D</code> and this
		''' <code>Rectangle2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride Function createUnion(  r As Rectangle2D) As Rectangle2D

		''' <summary>
		''' Unions the pair of source <code>Rectangle2D</code> objects
		''' and puts the result into the specified destination
		''' <code>Rectangle2D</code> object.  One of the source rectangles
		''' can also be the destination to avoid creating a third Rectangle2D
		''' object, but in this case the original points of this source
		''' rectangle will be overwritten by this method. </summary>
		''' <param name="src1"> the first of a pair of <code>Rectangle2D</code>
		''' objects to be combined with each other </param>
		''' <param name="src2"> the second of a pair of <code>Rectangle2D</code>
		''' objects to be combined with each other </param>
		''' <param name="dest"> the <code>Rectangle2D</code> that holds the
		''' results of the union of <code>src1</code> and
		''' <code>src2</code>
		''' @since 1.2 </param>
		Public Shared Sub union(  src1 As Rectangle2D,   src2 As Rectangle2D,   dest As Rectangle2D)
			Dim x1 As Double = System.Math.Min(src1.minX, src2.minX)
			Dim y1 As Double = System.Math.Min(src1.minY, src2.minY)
			Dim x2 As Double = System.Math.Max(src1.maxX, src2.maxX)
			Dim y2 As Double = System.Math.Max(src1.maxY, src2.maxY)
			dest.frameFromDiagonalnal(x1, y1, x2, y2)
		End Sub

		''' <summary>
		''' Adds a point, specified by the double precision arguments
		''' <code>newx</code> and <code>newy</code>, to this
		''' <code>Rectangle2D</code>.  The resulting <code>Rectangle2D</code>
		''' is the smallest <code>Rectangle2D</code> that
		''' contains both the original <code>Rectangle2D</code> and the
		''' specified point.
		''' <p>
		''' After adding a point, a call to <code>contains</code> with the
		''' added point as an argument does not necessarily return
		''' <code>true</code>. The <code>contains</code> method does not
		''' return <code>true</code> for points on the right or bottom
		''' edges of a rectangle. Therefore, if the added point falls on
		''' the left or bottom edge of the enlarged rectangle,
		''' <code>contains</code> returns <code>false</code> for that point. </summary>
		''' <param name="newx"> the X coordinate of the new point </param>
		''' <param name="newy"> the Y coordinate of the new point
		''' @since 1.2 </param>
		Public Overridable Sub add(  newx As Double,   newy As Double)
			Dim x1 As Double = System.Math.Min(minX, newx)
			Dim x2 As Double = System.Math.Max(maxX, newx)
			Dim y1 As Double = System.Math.Min(minY, newy)
			Dim y2 As Double = System.Math.Max(maxY, newy)
			rectect(x1, y1, x2 - x1, y2 - y1)
		End Sub

		''' <summary>
		''' Adds the <code>Point2D</code> object <code>pt</code> to this
		''' <code>Rectangle2D</code>.
		''' The resulting <code>Rectangle2D</code> is the smallest
		''' <code>Rectangle2D</code> that contains both the original
		''' <code>Rectangle2D</code> and the specified <code>Point2D</code>.
		''' <p>
		''' After adding a point, a call to <code>contains</code> with the
		''' added point as an argument does not necessarily return
		''' <code>true</code>. The <code>contains</code>
		''' method does not return <code>true</code> for points on the right
		''' or bottom edges of a rectangle. Therefore, if the added point falls
		''' on the left or bottom edge of the enlarged rectangle,
		''' <code>contains</code> returns <code>false</code> for that point. </summary>
		''' <param name="pt"> the new <code>Point2D</code> to add to this
		''' <code>Rectangle2D</code>.
		''' @since 1.2 </param>
		Public Overridable Sub add(  pt As Point2D)
			add(pt.x, pt.y)
		End Sub

		''' <summary>
		''' Adds a <code>Rectangle2D</code> object to this
		''' <code>Rectangle2D</code>.  The resulting <code>Rectangle2D</code>
		''' is the union of the two <code>Rectangle2D</code> objects. </summary>
		''' <param name="r"> the <code>Rectangle2D</code> to add to this
		''' <code>Rectangle2D</code>.
		''' @since 1.2 </param>
		Public Overridable Sub add(  r As Rectangle2D)
			Dim x1 As Double = System.Math.Min(minX, r.minX)
			Dim x2 As Double = System.Math.Max(maxX, r.maxX)
			Dim y1 As Double = System.Math.Min(minY, r.minY)
			Dim y2 As Double = System.Math.Max(maxY, r.maxY)
			rectect(x1, y1, x2 - x1, y2 - y1)
		End Sub

		''' <summary>
		''' Returns an iteration object that defines the boundary of this
		''' <code>Rectangle2D</code>.
		''' The iterator for this class is multi-threaded safe, which means
		''' that this <code>Rectangle2D</code> class guarantees that
		''' modifications to the geometry of this <code>Rectangle2D</code>
		''' object do not affect any iterations of that geometry that
		''' are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to
		''' the coordinates as they are returned in the iteration, or
		''' <code>null</code> if untransformed coordinates are desired </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this
		'''          <code>Rectangle2D</code>, one segment at a time.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(  at As AffineTransform) As PathIterator
			Return New RectIterator(Me, at)
		End Function

		''' <summary>
		''' Returns an iteration object that defines the boundary of the
		''' flattened <code>Rectangle2D</code>.  Since rectangles are already
		''' flat, the <code>flatness</code> parameter is ignored.
		''' The iterator for this class is multi-threaded safe, which means
		''' that this <code>Rectangle2D</code> class guarantees that
		''' modifications to the geometry of this <code>Rectangle2D</code>
		''' object do not affect any iterations of that geometry that
		''' are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to
		''' the coordinates as they are returned in the iteration, or
		''' <code>null</code> if untransformed coordinates are desired </param>
		''' <param name="flatness"> the maximum distance that the line segments used to
		''' approximate the curved segments are allowed to deviate from any
		''' point on the original curve.  Since rectangles are already flat,
		''' the <code>flatness</code> parameter is ignored. </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this
		'''          <code>Rectangle2D</code>, one segment at a time.
		''' @since 1.2 </returns>
		Public Overrides Function getPathIterator(  at As AffineTransform,   flatness As Double) As PathIterator
			Return New RectIterator(Me, at)
		End Function

		''' <summary>
		''' Returns the hashcode for this <code>Rectangle2D</code>. </summary>
		''' <returns> the hashcode for this <code>Rectangle2D</code>.
		''' @since 1.2 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(x)
			bits += java.lang.[Double].doubleToLongBits(y) * 37
			bits += java.lang.[Double].doubleToLongBits(width) * 43
			bits += java.lang.[Double].doubleToLongBits(height) * 47
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Determines whether or not the specified <code>Object</code> is
		''' equal to this <code>Rectangle2D</code>.  The specified
		''' <code>Object</code> is equal to this <code>Rectangle2D</code>
		''' if it is an instance of <code>Rectangle2D</code> and if its
		''' location and size are the same as this <code>Rectangle2D</code>. </summary>
		''' <param name="obj"> an <code>Object</code> to be compared with this
		''' <code>Rectangle2D</code>. </param>
		''' <returns>     <code>true</code> if <code>obj</code> is an instance
		'''                     of <code>Rectangle2D</code> and has
		'''                     the same values; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Me Then Return True
			If TypeOf obj Is Rectangle2D Then
				Dim r2d As Rectangle2D = CType(obj, Rectangle2D)
				Return ((x = r2d.x) AndAlso (y = r2d.y) AndAlso (width = r2d.width) AndAlso (height = r2d.height))
			End If
			Return False
		End Function
	End Class

End Namespace