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
	''' The <code>RoundRectangle2D</code> class defines a rectangle with
	''' rounded corners defined by a location {@code (x,y)}, a
	''' dimension {@code (w x h)}, and the width and height of an arc
	''' with which to round the corners.
	''' <p>
	''' This class is the abstract superclass for all objects that
	''' store a 2D rounded rectangle.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class RoundRectangle2D
		Inherits RectangularShape

		''' <summary>
		''' The <code>Float</code> class defines a rectangle with rounded
		''' corners all specified in <code>float</code> coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits RoundRectangle2D

			''' <summary>
			''' The X coordinate of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Single

			''' <summary>
			''' The Y coordinate of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Single

			''' <summary>
			''' The width of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Single

			''' <summary>
			''' The height of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Single

			''' <summary>
			''' The width of the arc that rounds off the corners.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public arcwidth As Single

			''' <summary>
			''' The height of the arc that rounds off the corners.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public archeight As Single

			''' <summary>
			''' Constructs a new <code>RoundRectangle2D</code>, initialized to
			''' location (0.0,&nbsp;0.0), size (0.0,&nbsp;0.0), and corner arcs
			''' of radius 0.0.
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes a <code>RoundRectangle2D</code>
			''' from the specified <code>float</code> coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="y"> the Y coordinate of the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="w"> the width to which to set the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="h"> the height to which to set the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="arcw"> the width of the arc to use to round off the
			'''             corners of the newly constructed
			'''             <code>RoundRectangle2D</code> </param>
			''' <param name="arch"> the height of the arc to use to round off the
			'''             corners of the newly constructed
			'''             <code>RoundRectangle2D</code>
			''' @since 1.2 </param>
			Public Sub New(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal arcw As Single, ByVal arch As Single)
				roundRectect(x, y, w, h, arcw, arch)
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
			Public  Overrides ReadOnly Property  arcWidth As Double
				Get
					Return CDbl(arcwidth)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  arcHeight As Double
				Get
					Return CDbl(archeight)
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
			''' Sets the location, size, and corner radii of this
			''' <code>RoundRectangle2D</code> to the specified
			''' <code>float</code> values.
			''' </summary>
			''' <param name="x"> the X coordinate to which to set the
			'''          location of this <code>RoundRectangle2D</code> </param>
			''' <param name="y"> the Y coordinate to which to set the
			'''          location of this <code>RoundRectangle2D</code> </param>
			''' <param name="w"> the width to which to set this
			'''          <code>RoundRectangle2D</code> </param>
			''' <param name="h"> the height to which to set this
			'''          <code>RoundRectangle2D</code> </param>
			''' <param name="arcw"> the width to which to set the arc of this
			'''             <code>RoundRectangle2D</code> </param>
			''' <param name="arch"> the height to which to set the arc of this
			'''             <code>RoundRectangle2D</code>
			''' @since 1.2 </param>
			Public Overridable Sub setRoundRect(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal arcw As Single, ByVal arch As Single)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
				Me.arcwidth = arcw
				Me.archeight = arch
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setRoundRect(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal arcw As Double, ByVal arch As Double)
				Me.x = CSng(x)
				Me.y = CSng(y)
				Me.width = CSng(w)
				Me.height = CSng(h)
				Me.arcwidth = CSng(arcw)
				Me.archeight = CSng(arch)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Property roundRect As RoundRectangle2D
				Set(ByVal rr As RoundRectangle2D)
					Me.x = CSng(rr.x)
					Me.y = CSng(rr.y)
					Me.width = CSng(rr.width)
					Me.height = CSng(rr.height)
					Me.arcwidth = CSng(rr.arcWidth)
					Me.archeight = CSng(rr.arcHeight)
				End Set
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
				Get
					Return New Rectangle2D.Float(x, y, width, height)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -3423150618393866922L
		End Class

		''' <summary>
		''' The <code>Double</code> class defines a rectangle with rounded
		''' corners all specified in <code>double</code> coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits RoundRectangle2D

			''' <summary>
			''' The X coordinate of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Double

			''' <summary>
			''' The Y coordinate of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Double

			''' <summary>
			''' The width of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Double

			''' <summary>
			''' The height of this <code>RoundRectangle2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Double

			''' <summary>
			''' The width of the arc that rounds off the corners.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public arcwidth As Double

			''' <summary>
			''' The height of the arc that rounds off the corners.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public archeight As Double

			''' <summary>
			''' Constructs a new <code>RoundRectangle2D</code>, initialized to
			''' location (0.0,&nbsp;0.0), size (0.0,&nbsp;0.0), and corner arcs
			''' of radius 0.0.
			''' @since 1.2
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
			End Function

			''' <summary>
			''' Constructs and initializes a <code>RoundRectangle2D</code>
			''' from the specified <code>double</code> coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="y"> the Y coordinate of the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="w"> the width to which to set the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="h"> the height to which to set the newly
			'''          constructed <code>RoundRectangle2D</code> </param>
			''' <param name="arcw"> the width of the arc to use to round off the
			'''             corners of the newly constructed
			'''             <code>RoundRectangle2D</code> </param>
			''' <param name="arch"> the height of the arc to use to round off the
			'''             corners of the newly constructed
			'''             <code>RoundRectangle2D</code>
			''' @since 1.2 </param>
			Function java.lang.Double(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal arcw As Double, ByVal arch As Double) As [Public] Overridable
				roundRectect(x, y, w, h, arcw, arch)
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
			Public  Overrides ReadOnly Property  arcWidth As Double
				Get
					Return arcwidth
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  arcHeight As Double
				Get
					Return archeight
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
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setRoundRect(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal arcw As Double, ByVal arch As Double)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
				Me.arcwidth = arcw
				Me.archeight = arch
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Property roundRect As RoundRectangle2D
				Set(ByVal rr As RoundRectangle2D)
					Me.x = rr.x
					Me.y = rr.y
					Me.width = rr.width
					Me.height = rr.height
					Me.arcwidth = rr.arcWidth
					Me.archeight = rr.arcHeight
				End Set
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
				Get
					Return New Rectangle2D.Double(x, y, width, height)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 1048939333485206117L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.RoundRectangle2D.Float </seealso>
		''' <seealso cref= java.awt.geom.RoundRectangle2D.Double
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Gets the width of the arc that rounds off the corners. </summary>
		''' <returns> the width of the arc that rounds off the corners
		''' of this <code>RoundRectangle2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property arcWidth As Double

		''' <summary>
		''' Gets the height of the arc that rounds off the corners. </summary>
		''' <returns> the height of the arc that rounds off the corners
		''' of this <code>RoundRectangle2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property arcHeight As Double

		''' <summary>
		''' Sets the location, size, and corner radii of this
		''' <code>RoundRectangle2D</code> to the specified
		''' <code>double</code> values.
		''' </summary>
		''' <param name="x"> the X coordinate to which to set the
		'''          location of this <code>RoundRectangle2D</code> </param>
		''' <param name="y"> the Y coordinate to which to set the
		'''          location of this <code>RoundRectangle2D</code> </param>
		''' <param name="w"> the width to which to set this
		'''          <code>RoundRectangle2D</code> </param>
		''' <param name="h"> the height to which to set this
		'''          <code>RoundRectangle2D</code> </param>
		''' <param name="arcWidth"> the width to which to set the arc of this
		'''                 <code>RoundRectangle2D</code> </param>
		''' <param name="arcHeight"> the height to which to set the arc of this
		'''                  <code>RoundRectangle2D</code>
		''' @since 1.2 </param>
		Public MustOverride Sub setRoundRect(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal arcWidth As Double, ByVal arcHeight As Double)

		''' <summary>
		''' Sets this <code>RoundRectangle2D</code> to be the same as the
		''' specified <code>RoundRectangle2D</code>. </summary>
		''' <param name="rr"> the specified <code>RoundRectangle2D</code>
		''' @since 1.2 </param>
		Public Overridable Property roundRect As RoundRectangle2D
			Set(ByVal rr As RoundRectangle2D)
				roundRectect(rr.x, rr.y, rr.width, rr.height, rr.arcWidth, rr.arcHeight)
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Sub setFrame(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double)
			roundRectect(x, y, w, h, arcWidth, arcHeight)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double) As Boolean
			If empty Then Return False
			Dim rrx0 As Double = x
			Dim rry0 As Double = y
			Dim rrx1 As Double = rrx0 + width
			Dim rry1 As Double = rry0 + height
			' Check for trivial rejection - point is outside bounding rectangle
			If x < rrx0 OrElse y < rry0 OrElse x >= rrx1 OrElse y >= rry1 Then Return False
			Dim aw As Double = System.Math.Min(width, System.Math.Abs(arcWidth)) / 2.0
			Dim ah As Double = System.Math.Min(height, System.Math.Abs(arcHeight)) / 2.0
			' Check which corner point is in and do circular containment
			' test - otherwise simple acceptance
			rrx0 += aw
			rrx0 = rrx1 - aw
			If x >= rrx0 AndAlso x < rrx0 Then Return True
			rry0 += ah
			rry0 = rry1 - ah
			If y >= rry0 AndAlso y < rry0 Then Return True
			x = (x - rrx0) / aw
			y = (y - rry0) / ah
			Return (x * x + y * y <= 1.0)
		End Function

		Private Function classify(ByVal coord As Double, ByVal left As Double, ByVal right As Double, ByVal arcsize As Double) As Integer
			If coord < left Then
				Return 0
			ElseIf coord < left + arcsize Then
				Return 1
			ElseIf coord < right - arcsize Then
				Return 2
			ElseIf coord < right Then
				Return 3
			Else
				Return 4
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function intersects(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean
			If empty OrElse w <= 0 OrElse h <= 0 Then Return False
			Dim rrx0 As Double = x
			Dim rry0 As Double = y
			Dim rrx1 As Double = rrx0 + width
			Dim rry1 As Double = rry0 + height
			' Check for trivial rejection - bounding rectangles do not intersect
			If x + w <= rrx0 OrElse x >= rrx1 OrElse y + h <= rry0 OrElse y >= rry1 Then Return False
			Dim aw As Double = System.Math.Min(width, System.Math.Abs(arcWidth)) / 2.0
			Dim ah As Double = System.Math.Min(height, System.Math.Abs(arcHeight)) / 2.0
			Dim x0class As Integer = classify(x, rrx0, rrx1, aw)
			Dim x1class As Integer = classify(x + w, rrx0, rrx1, aw)
			Dim y0class As Integer = classify(y, rry0, rry1, ah)
			Dim y1class As Integer = classify(y + h, rry0, rry1, ah)
			' Trivially accept if any point is inside inner rectangle
			If x0class = 2 OrElse x1class = 2 OrElse y0class = 2 OrElse y1class = 2 Then Return True
			' Trivially accept if either edge spans inner rectangle
			If (x0class < 2 AndAlso x1class > 2) OrElse (y0class < 2 AndAlso y1class > 2) Then Return True
			' Since neither edge spans the center, then one of the corners
			' must be in one of the rounded edges.  We detect this case if
			' a [xy]0class is 3 or a [xy]1class is 1.  One of those two cases
			' must be true for each direction.
			' We now find a "nearest point" to test for being inside a rounded
			' corner.
				If x1class = 1 Then
						x = x + w - (rrx0 + aw)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					x = (x = x - (rrx1 - aw))
				End If
				If y1class = 1 Then
						y = y + h - (rry0 + ah)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					y = (y = y - (rry1 - ah))
				End If
			x = x / aw
			y = y / ah
			Return (x * x + y * y <= 1.0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean
			If empty OrElse w <= 0 OrElse h <= 0 Then Return False
			Return (contains(x, y) AndAlso contains(x + w, y) AndAlso contains(x, y + h) AndAlso contains(x + w, y + h))
		End Function

		''' <summary>
		''' Returns an iteration object that defines the boundary of this
		''' <code>RoundRectangle2D</code>.
		''' The iterator for this class is multi-threaded safe, which means
		''' that this <code>RoundRectangle2D</code> class guarantees that
		''' modifications to the geometry of this <code>RoundRectangle2D</code>
		''' object do not affect any iterations of that geometry that
		''' are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to
		''' the coordinates as they are returned in the iteration, or
		''' <code>null</code> if untransformed coordinates are desired </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this
		'''          <code>RoundRectangle2D</code>, one segment at a time.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(ByVal at As AffineTransform) As PathIterator
			Return New RoundRectIterator(Me, at)
		End Function

		''' <summary>
		''' Returns the hashcode for this <code>RoundRectangle2D</code>. </summary>
		''' <returns> the hashcode for this <code>RoundRectangle2D</code>.
		''' @since 1.6 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(x)
			bits += java.lang.[Double].doubleToLongBits(y) * 37
			bits += java.lang.[Double].doubleToLongBits(width) * 43
			bits += java.lang.[Double].doubleToLongBits(height) * 47
			bits += java.lang.[Double].doubleToLongBits(arcWidth) * 53
			bits += java.lang.[Double].doubleToLongBits(arcHeight) * 59
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Determines whether or not the specified <code>Object</code> is
		''' equal to this <code>RoundRectangle2D</code>.  The specified
		''' <code>Object</code> is equal to this <code>RoundRectangle2D</code>
		''' if it is an instance of <code>RoundRectangle2D</code> and if its
		''' location, size, and corner arc dimensions are the same as this
		''' <code>RoundRectangle2D</code>. </summary>
		''' <param name="obj">  an <code>Object</code> to be compared with this
		'''             <code>RoundRectangle2D</code>. </param>
		''' <returns>  <code>true</code> if <code>obj</code> is an instance
		'''          of <code>RoundRectangle2D</code> and has the same values;
		'''          <code>false</code> otherwise.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If TypeOf obj Is RoundRectangle2D Then
				Dim rr2d As RoundRectangle2D = CType(obj, RoundRectangle2D)
				Return ((x = rr2d.x) AndAlso (y = rr2d.y) AndAlso (width = rr2d.width) AndAlso (height = rr2d.height) AndAlso (arcWidth = rr2d.arcWidth) AndAlso (arcHeight = rr2d.arcHeight))
			End If
			Return False
		End Function
	End Class

End Namespace