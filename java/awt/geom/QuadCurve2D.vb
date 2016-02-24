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
	''' The <code>QuadCurve2D</code> class defines a quadratic parametric curve
	''' segment in {@code (x,y)} coordinate space.
	''' <p>
	''' This class is only the abstract superclass for all objects that
	''' store a 2D quadratic curve segment.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class QuadCurve2D
		Implements java.awt.Shape, Cloneable

		''' <summary>
		''' A quadratic parametric curve segment specified with
		''' {@code float} coordinates.
		''' 
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits QuadCurve2D

			''' <summary>
			''' The X coordinate of the start point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x1 As Single

			''' <summary>
			''' The Y coordinate of the start point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y1 As Single

			''' <summary>
			''' The X coordinate of the control point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx As Single

			''' <summary>
			''' The Y coordinate of the control point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly As Single

			''' <summary>
			''' The X coordinate of the end point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x2 As Single

			''' <summary>
			''' The Y coordinate of the end point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y2 As Single

			''' <summary>
			''' Constructs and initializes a <code>QuadCurve2D</code> with
			''' coordinates (0, 0, 0, 0, 0, 0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes a <code>QuadCurve2D</code> from the
			''' specified {@code float} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate of the start point </param>
			''' <param name="y1"> the Y coordinate of the start point </param>
			''' <param name="ctrlx"> the X coordinate of the control point </param>
			''' <param name="ctrly"> the Y coordinate of the control point </param>
			''' <param name="x2"> the X coordinate of the end point </param>
			''' <param name="y2"> the Y coordinate of the end point
			''' @since 1.2 </param>
			Public Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal ctrlx As Single, ByVal ctrly As Single, ByVal x2 As Single, ByVal y2 As Single)
				curverve(x1, y1, ctrlx, ctrly, x2, y2)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides x1 As Double
				Get
					Return CDbl(x1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides y1 As Double
				Get
					Return CDbl(y1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides p1 As Point2D
				Get
					Return New Point2D.Float(x1, y1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlX As Double
				Get
					Return CDbl(ctrlx)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY As Double
				Get
					Return CDbl(ctrly)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlPt As Point2D
				Get
					Return New Point2D.Float(ctrlx, ctrly)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides x2 As Double
				Get
					Return CDbl(x2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides y2 As Double
				Get
					Return CDbl(y2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides p2 As Point2D
				Get
					Return New Point2D.Float(x2, y2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double)
				Me.x1 = CSng(x1)
				Me.y1 = CSng(y1)
				Me.ctrlx = CSng(ctrlx)
				Me.ctrly = CSng(ctrly)
				Me.x2 = CSng(x2)
				Me.y2 = CSng(y2)
			End Sub

			''' <summary>
			''' Sets the location of the end points and control point of this curve
			''' to the specified {@code float} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate of the start point </param>
			''' <param name="y1"> the Y coordinate of the start point </param>
			''' <param name="ctrlx"> the X coordinate of the control point </param>
			''' <param name="ctrly"> the Y coordinate of the control point </param>
			''' <param name="x2"> the X coordinate of the end point </param>
			''' <param name="y2"> the Y coordinate of the end point
			''' @since 1.2 </param>
			Public Overridable Sub setCurve(ByVal x1 As Single, ByVal y1 As Single, ByVal ctrlx As Single, ByVal ctrly As Single, ByVal x2 As Single, ByVal y2 As Single)
				Me.x1 = x1
				Me.y1 = y1
				Me.ctrlx = ctrlx
				Me.ctrly = ctrly
				Me.x2 = x2
				Me.y2 = y2
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Dim left As Single = Math.Min(Math.Min(x1, x2), ctrlx)
					Dim top As Single = Math.Min(Math.Min(y1, y2), ctrly)
					Dim right As Single = Math.Max(Math.Max(x1, x2), ctrlx)
					Dim bottom As Single = Math.Max(Math.Max(y1, y2), ctrly)
					Return New Rectangle2D.Float(left, top, right - left, bottom - top)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -8511188402130719609L
		End Class

		''' <summary>
		''' A quadratic parametric curve segment specified with
		''' {@code double} coordinates.
		''' 
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits QuadCurve2D

			''' <summary>
			''' The X coordinate of the start point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x1 As Double

			''' <summary>
			''' The Y coordinate of the start point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y1 As Double

			''' <summary>
			''' The X coordinate of the control point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx As Double

			''' <summary>
			''' The Y coordinate of the control point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly As Double

			''' <summary>
			''' The X coordinate of the end point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x2 As Double

			''' <summary>
			''' The Y coordinate of the end point of the quadratic curve
			''' segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y2 As Double

			''' <summary>
			''' Constructs and initializes a <code>QuadCurve2D</code> with
			''' coordinates (0, 0, 0, 0, 0, 0).
			''' @since 1.2
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
			End Function

			''' <summary>
			''' Constructs and initializes a <code>QuadCurve2D</code> from the
			''' specified {@code double} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate of the start point </param>
			''' <param name="y1"> the Y coordinate of the start point </param>
			''' <param name="ctrlx"> the X coordinate of the control point </param>
			''' <param name="ctrly"> the Y coordinate of the control point </param>
			''' <param name="x2"> the X coordinate of the end point </param>
			''' <param name="y2"> the Y coordinate of the end point
			''' @since 1.2 </param>
			Function java.lang.Double(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double) As [Public] Overridable
				curverve(x1, y1, ctrlx, ctrly, x2, y2)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides x1 As Double
				Get
					Return x1
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides y1 As Double
				Get
					Return y1
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides p1 As Point2D
				Get
					Return New Point2D.Double(x1, y1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlX As Double
				Get
					Return ctrlx
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY As Double
				Get
					Return ctrly
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlPt As Point2D
				Get
					Return New Point2D.Double(ctrlx, ctrly)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides x2 As Double
				Get
					Return x2
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides y2 As Double
				Get
					Return y2
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides p2 As Point2D
				Get
					Return New Point2D.Double(x2, y2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double)
				Me.x1 = x1
				Me.y1 = y1
				Me.ctrlx = ctrlx
				Me.ctrly = ctrly
				Me.x2 = x2
				Me.y2 = y2
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Dim left As Double = Math.Min(Math.Min(x1, x2), ctrlx)
					Dim top As Double = Math.Min(Math.Min(y1, y2), ctrly)
					Dim right As Double = Math.Max(Math.Max(x1, x2), ctrlx)
					Dim bottom As Double = Math.Max(Math.Max(y1, y2), ctrly)
					Return New Rectangle2D.Double(left, top, right - left, bottom - top)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 4217149928428559721L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.QuadCurve2D.Float </seealso>
		''' <seealso cref= java.awt.geom.QuadCurve2D.Double
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the X coordinate of the start point in
		''' <code>double</code> in precision. </summary>
		''' <returns> the X coordinate of the start point.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property x1 As Double

		''' <summary>
		''' Returns the Y coordinate of the start point in
		''' <code>double</code> precision. </summary>
		''' <returns> the Y coordinate of the start point.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property y1 As Double

		''' <summary>
		''' Returns the start point. </summary>
		''' <returns> a <code>Point2D</code> that is the start point of this
		'''          <code>QuadCurve2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property p1 As Point2D

		''' <summary>
		''' Returns the X coordinate of the control point in
		''' <code>double</code> precision. </summary>
		''' <returns> X coordinate the control point
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlX As Double

		''' <summary>
		''' Returns the Y coordinate of the control point in
		''' <code>double</code> precision. </summary>
		''' <returns> the Y coordinate of the control point.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlY As Double

		''' <summary>
		''' Returns the control point. </summary>
		''' <returns> a <code>Point2D</code> that is the control point of this
		'''          <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlPt As Point2D

		''' <summary>
		''' Returns the X coordinate of the end point in
		''' <code>double</code> precision. </summary>
		''' <returns> the x coordinate of the end point.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property x2 As Double

		''' <summary>
		''' Returns the Y coordinate of the end point in
		''' <code>double</code> precision. </summary>
		''' <returns> the Y coordinate of the end point.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property y2 As Double

		''' <summary>
		''' Returns the end point. </summary>
		''' <returns> a <code>Point</code> object that is the end point
		'''          of this <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property p2 As Point2D

		''' <summary>
		''' Sets the location of the end points and control point of this curve
		''' to the specified <code>double</code> coordinates.
		''' </summary>
		''' <param name="x1"> the X coordinate of the start point </param>
		''' <param name="y1"> the Y coordinate of the start point </param>
		''' <param name="ctrlx"> the X coordinate of the control point </param>
		''' <param name="ctrly"> the Y coordinate of the control point </param>
		''' <param name="x2"> the X coordinate of the end point </param>
		''' <param name="y2"> the Y coordinate of the end point
		''' @since 1.2 </param>
		Public MustOverride Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double)

		''' <summary>
		''' Sets the location of the end points and control points of this
		''' <code>QuadCurve2D</code> to the <code>double</code> coordinates at
		''' the specified offset in the specified array. </summary>
		''' <param name="coords"> the array containing coordinate values </param>
		''' <param name="offset"> the index into the array from which to start
		'''          getting the coordinate values and assigning them to this
		'''          <code>QuadCurve2D</code>
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal coords As Double(), ByVal offset As Integer)
			curverve(coords(offset + 0), coords(offset + 1), coords(offset + 2), coords(offset + 3), coords(offset + 4), coords(offset + 5))
		End Sub

		''' <summary>
		''' Sets the location of the end points and control point of this
		''' <code>QuadCurve2D</code> to the specified <code>Point2D</code>
		''' coordinates. </summary>
		''' <param name="p1"> the start point </param>
		''' <param name="cp"> the control point </param>
		''' <param name="p2"> the end point
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal p1 As Point2D, ByVal cp As Point2D, ByVal p2 As Point2D)
			curverve(p1.x, p1.y, cp.x, cp.y, p2.x, p2.y)
		End Sub

		''' <summary>
		''' Sets the location of the end points and control points of this
		''' <code>QuadCurve2D</code> to the coordinates of the
		''' <code>Point2D</code> objects at the specified offset in
		''' the specified array. </summary>
		''' <param name="pts"> an array containing <code>Point2D</code> that define
		'''          coordinate values </param>
		''' <param name="offset"> the index into <code>pts</code> from which to start
		'''          getting the coordinate values and assigning them to this
		'''          <code>QuadCurve2D</code>
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal pts As Point2D(), ByVal offset As Integer)
			curverve(pts(offset + 0).x, pts(offset + 0).y, pts(offset + 1).x, pts(offset + 1).y, pts(offset + 2).x, pts(offset + 2).y)
		End Sub

		''' <summary>
		''' Sets the location of the end points and control point of this
		''' <code>QuadCurve2D</code> to the same as those in the specified
		''' <code>QuadCurve2D</code>. </summary>
		''' <param name="c"> the specified <code>QuadCurve2D</code>
		''' @since 1.2 </param>
		Public Overridable Property curve As QuadCurve2D
			Set(ByVal c As QuadCurve2D)
				curverve(c.x1, c.y1, c.ctrlX, c.ctrlY, c.x2, c.y2)
			End Set
		End Property

		''' <summary>
		''' Returns the square of the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of the
		''' quadratic curve specified by the indicated control points.
		''' </summary>
		''' <param name="x1"> the X coordinate of the start point </param>
		''' <param name="y1"> the Y coordinate of the start point </param>
		''' <param name="ctrlx"> the X coordinate of the control point </param>
		''' <param name="ctrly"> the Y coordinate of the control point </param>
		''' <param name="x2"> the X coordinate of the end point </param>
		''' <param name="y2"> the Y coordinate of the end point </param>
		''' <returns> the square of the flatness of the quadratic curve
		'''          defined by the specified coordinates.
		''' @since 1.2 </returns>
		Public Shared Function getFlatnessSq(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
			Return Line2D.ptSegDistSq(x1, y1, x2, y2, ctrlx, ctrly)
		End Function

		''' <summary>
		''' Returns the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of the
		''' quadratic curve specified by the indicated control points.
		''' </summary>
		''' <param name="x1"> the X coordinate of the start point </param>
		''' <param name="y1"> the Y coordinate of the start point </param>
		''' <param name="ctrlx"> the X coordinate of the control point </param>
		''' <param name="ctrly"> the Y coordinate of the control point </param>
		''' <param name="x2"> the X coordinate of the end point </param>
		''' <param name="y2"> the Y coordinate of the end point </param>
		''' <returns> the flatness of the quadratic curve defined by the
		'''          specified coordinates.
		''' @since 1.2 </returns>
		Public Shared Function getFlatness(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx As Double, ByVal ctrly As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
			Return Line2D.ptSegDist(x1, y1, x2, y2, ctrlx, ctrly)
		End Function

		''' <summary>
		''' Returns the square of the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of the
		''' quadratic curve specified by the control points stored in the
		''' indicated array at the indicated index. </summary>
		''' <param name="coords"> an array containing coordinate values </param>
		''' <param name="offset"> the index into <code>coords</code> from which to
		'''          to start getting the values from the array </param>
		''' <returns> the flatness of the quadratic curve that is defined by the
		'''          values in the specified array at the specified index.
		''' @since 1.2 </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public static double getFlatnessSq(double coords() , int offset)
			Return Line2D.ptSegDistSq(coords(offset + 0), coords(offset + 1), coords(offset + 4), coords(offset + 5), coords(offset + 2), coords(offset + 3))

		''' <summary>
		''' Returns the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of the
		''' quadratic curve specified by the control points stored in the
		''' indicated array at the indicated index. </summary>
		''' <param name="coords"> an array containing coordinate values </param>
		''' <param name="offset"> the index into <code>coords</code> from which to
		'''          start getting the coordinate values </param>
		''' <returns> the flatness of a quadratic curve defined by the
		'''          specified array at the specified offset.
		''' @since 1.2 </returns>
		public static Double getFlatness(Double coords() , Integer offset)
			Return Line2D.ptSegDist(coords(offset + 0), coords(offset + 1), coords(offset + 4), coords(offset + 5), coords(offset + 2), coords(offset + 3))

		''' <summary>
		''' Returns the square of the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of this
		''' <code>QuadCurve2D</code>. </summary>
		''' <returns> the square of the flatness of this
		'''          <code>QuadCurve2D</code>.
		''' @since 1.2 </returns>
		public Double flatnessSq
			Return Line2D.ptSegDistSq(x1, y1, x2, y2, ctrlX, ctrlY)

		''' <summary>
		''' Returns the flatness, or maximum distance of a
		''' control point from the line connecting the end points, of this
		''' <code>QuadCurve2D</code>. </summary>
		''' <returns> the flatness of this <code>QuadCurve2D</code>.
		''' @since 1.2 </returns>
		public Double flatness
			Return Line2D.ptSegDist(x1, y1, x2, y2, ctrlX, ctrlY)

		''' <summary>
		''' Subdivides this <code>QuadCurve2D</code> and stores the resulting
		''' two subdivided curves into the <code>left</code> and
		''' <code>right</code> curve parameters.
		''' Either or both of the <code>left</code> and <code>right</code>
		''' objects can be the same as this <code>QuadCurve2D</code> or
		''' <code>null</code>. </summary>
		''' <param name="left"> the <code>QuadCurve2D</code> object for storing the
		''' left or first half of the subdivided curve </param>
		''' <param name="right"> the <code>QuadCurve2D</code> object for storing the
		''' right or second half of the subdivided curve
		''' @since 1.2 </param>
		public void subdivide(QuadCurve2D left, QuadCurve2D right)
			subdivide(Me, left, right)

		''' <summary>
		''' Subdivides the quadratic curve specified by the <code>src</code>
		''' parameter and stores the resulting two subdivided curves into the
		''' <code>left</code> and <code>right</code> curve parameters.
		''' Either or both of the <code>left</code> and <code>right</code>
		''' objects can be the same as the <code>src</code> object or
		''' <code>null</code>. </summary>
		''' <param name="src"> the quadratic curve to be subdivided </param>
		''' <param name="left"> the <code>QuadCurve2D</code> object for storing the
		'''          left or first half of the subdivided curve </param>
		''' <param name="right"> the <code>QuadCurve2D</code> object for storing the
		'''          right or second half of the subdivided curve
		''' @since 1.2 </param>
		public static void subdivide(QuadCurve2D src, QuadCurve2D left, QuadCurve2D right)
			Dim x1_Renamed As Double = src.x1
			Dim y1_Renamed As Double = src.y1
			Dim ctrlx_Renamed As Double = src.ctrlX
			Dim ctrly_Renamed As Double = src.ctrlY
			Dim x2_Renamed As Double = src.x2
			Dim y2_Renamed As Double = src.y2
			Dim ctrlx1 As Double = (x1_Renamed + ctrlx_Renamed) / 2.0
			Dim ctrly1 As Double = (y1_Renamed + ctrly_Renamed) / 2.0
			Dim ctrlx2 As Double = (x2_Renamed + ctrlx_Renamed) / 2.0
			Dim ctrly2 As Double = (y2_Renamed + ctrly_Renamed) / 2.0
			ctrlx_Renamed = (ctrlx1 + ctrlx2) / 2.0
			ctrly_Renamed = (ctrly1 + ctrly2) / 2.0
			If left IsNot Nothing Then left.curverve(x1_Renamed, y1_Renamed, ctrlx1, ctrly1, ctrlx_Renamed, ctrly_Renamed)
			If right IsNot Nothing Then right.curverve(ctrlx_Renamed, ctrly_Renamed, ctrlx2, ctrly2, x2_Renamed, y2_Renamed)

		''' <summary>
		''' Subdivides the quadratic curve specified by the coordinates
		''' stored in the <code>src</code> array at indices
		''' <code>srcoff</code> through <code>srcoff</code>&nbsp;+&nbsp;5
		''' and stores the resulting two subdivided curves into the two
		''' result arrays at the corresponding indices.
		''' Either or both of the <code>left</code> and <code>right</code>
		''' arrays can be <code>null</code> or a reference to the same array
		''' and offset as the <code>src</code> array.
		''' Note that the last point in the first subdivided curve is the
		''' same as the first point in the second subdivided curve.  Thus,
		''' it is possible to pass the same array for <code>left</code> and
		''' <code>right</code> and to use offsets such that
		''' <code>rightoff</code> equals <code>leftoff</code> + 4 in order
		''' to avoid allocating extra storage for this common point. </summary>
		''' <param name="src"> the array holding the coordinates for the source curve </param>
		''' <param name="srcoff"> the offset into the array of the beginning of the
		''' the 6 source coordinates </param>
		''' <param name="left"> the array for storing the coordinates for the first
		''' half of the subdivided curve </param>
		''' <param name="leftoff"> the offset into the array of the beginning of the
		''' the 6 left coordinates </param>
		''' <param name="right"> the array for storing the coordinates for the second
		''' half of the subdivided curve </param>
		''' <param name="rightoff"> the offset into the array of the beginning of the
		''' the 6 right coordinates
		''' @since 1.2 </param>
		public static void subdivide(Double src() , Integer srcoff, Double left(), Integer leftoff, Double right(), Integer rightoff)
			Dim x1_Renamed As Double = src(srcoff + 0)
			Dim y1_Renamed As Double = src(srcoff + 1)
			Dim ctrlx_Renamed As Double = src(srcoff + 2)
			Dim ctrly_Renamed As Double = src(srcoff + 3)
			Dim x2_Renamed As Double = src(srcoff + 4)
			Dim y2_Renamed As Double = src(srcoff + 5)
			If left IsNot Nothing Then
				left(leftoff + 0) = x1_Renamed
				left(leftoff + 1) = y1_Renamed
			End If
			If right IsNot Nothing Then
				right(rightoff + 4) = x2_Renamed
				right(rightoff + 5) = y2_Renamed
			End If
			x1_Renamed = (x1_Renamed + ctrlx_Renamed) / 2.0
			y1_Renamed = (y1_Renamed + ctrly_Renamed) / 2.0
			x2_Renamed = (x2_Renamed + ctrlx_Renamed) / 2.0
			y2_Renamed = (y2_Renamed + ctrly_Renamed) / 2.0
			ctrlx_Renamed = (x1_Renamed + x2_Renamed) / 2.0
			ctrly_Renamed = (y1_Renamed + y2_Renamed) / 2.0
			If left IsNot Nothing Then
				left(leftoff + 2) = x1_Renamed
				left(leftoff + 3) = y1_Renamed
				left(leftoff + 4) = ctrlx_Renamed
				left(leftoff + 5) = ctrly_Renamed
			End If
			If right IsNot Nothing Then
				right(rightoff + 0) = ctrlx_Renamed
				right(rightoff + 1) = ctrly_Renamed
				right(rightoff + 2) = x2_Renamed
				right(rightoff + 3) = y2_Renamed
			End If

		''' <summary>
		''' Solves the quadratic whose coefficients are in the <code>eqn</code>
		''' array and places the non-complex roots back into the same array,
		''' returning the number of roots.  The quadratic solved is represented
		''' by the equation:
		''' <pre>
		'''     eqn = {C, B, A};
		'''     ax^2 + bx + c = 0
		''' </pre>
		''' A return value of <code>-1</code> is used to distinguish a constant
		''' equation, which might be always 0 or never 0, from an equation that
		''' has no zeroes. </summary>
		''' <param name="eqn"> the array that contains the quadratic coefficients </param>
		''' <returns> the number of roots, or <code>-1</code> if the equation is
		'''          a constant
		''' @since 1.2 </returns>
		public static Integer solveQuadratic(Double eqn())
			Return solveQuadratic(eqn, eqn)

		''' <summary>
		''' Solves the quadratic whose coefficients are in the <code>eqn</code>
		''' array and places the non-complex roots into the <code>res</code>
		''' array, returning the number of roots.
		''' The quadratic solved is represented by the equation:
		''' <pre>
		'''     eqn = {C, B, A};
		'''     ax^2 + bx + c = 0
		''' </pre>
		''' A return value of <code>-1</code> is used to distinguish a constant
		''' equation, which might be always 0 or never 0, from an equation that
		''' has no zeroes. </summary>
		''' <param name="eqn"> the specified array of coefficients to use to solve
		'''        the quadratic equation </param>
		''' <param name="res"> the array that contains the non-complex roots
		'''        resulting from the solution of the quadratic equation </param>
		''' <returns> the number of roots, or <code>-1</code> if the equation is
		'''  a constant.
		''' @since 1.3 </returns>
		public static Integer solveQuadratic(Double eqn() , Double res())
			Dim a As Double = eqn(2)
			Dim b As Double = eqn(1)
			Dim c As Double = eqn(0)
			Dim roots As Integer = 0
			If a = 0.0 Then
				' The quadratic parabola has degenerated to a line.
				If b = 0.0 Then Return -1
				res(roots) = -c / b
				roots += 1
			Else
				' From Numerical Recipes, 5.6, Quadratic and Cubic Equations
				Dim d As Double = b * b - 4.0 * a * c
				If d < 0.0 Then Return 0
				d = Math.Sqrt(d)
				' For accuracy, calculate one root using:
				'     (-b +/- d) / 2a
				' and the other using:
				'     2c / (-b +/- d)
				' Choose the sign of the +/- so that b+d gets larger in magnitude
				If b < 0.0 Then d = -d
				Dim q As Double = (b + d) / -2.0
				' We already tested a for being 0 above
				res(roots) = q / a
				roots += 1
				If q <> 0.0 Then
					res(roots) = c / q
					roots += 1
				End If
			End If
			Return roots

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Double x, Double y)

			Dim x1_Renamed As Double = x1
			Dim y1_Renamed As Double = y1
			Dim xc As Double = ctrlX
			Dim yc As Double = ctrlY
			Dim x2_Renamed As Double = x2
			Dim y2_Renamed As Double = y2

	'        
	'         * We have a convex shape bounded by quad curve Pc(t)
	'         * and ine Pl(t).
	'         *
	'         *     P1 = (x1, y1) - start point of curve
	'         *     P2 = (x2, y2) - end point of curve
	'         *     Pc = (xc, yc) - control point
	'         *
	'         *     Pq(t) = P1*(1 - t)^2 + 2*Pc*t*(1 - t) + P2*t^2 =
	'         *           = (P1 - 2*Pc + P2)*t^2 + 2*(Pc - P1)*t + P1
	'         *     Pl(t) = P1*(1 - t) + P2*t
	'         *     t = [0:1]
	'         *
	'         *     P = (x, y) - point of interest
	'         *
	'         * Let's look at second derivative of quad curve equation:
	'         *
	'         *     Pq''(t) = 2 * (P1 - 2 * Pc + P2) = Pq''
	'         *     It's constant vector.
	'         *
	'         * Let's draw a line through P to be parallel to this
	'         * vector and find the intersection of the quad curve
	'         * and the line.
	'         *
	'         * Pq(t) is point of intersection if system of equations
	'         * below has the solution.
	'         *
	'         *     L(s) = P + Pq''*s == Pq(t)
	'         *     Pq''*s + (P - Pq(t)) == 0
	'         *
	'         *     | xq''*s + (x - xq(t)) == 0
	'         *     | yq''*s + (y - yq(t)) == 0
	'         *
	'         * This system has the solution if rank of its matrix equals to 1.
	'         * That is, determinant of the matrix should be zero.
	'         *
	'         *     (y - yq(t))*xq'' == (x - xq(t))*yq''
	'         *
	'         * Let's solve this equation with 't' variable.
	'         * Also let kx = x1 - 2*xc + x2
	'         *          ky = y1 - 2*yc + y2
	'         *
	'         *     t0q = (1/2)*((x - x1)*ky - (y - y1)*kx) /
	'         *                 ((xc - x1)*ky - (yc - y1)*kx)
	'         *
	'         * Let's do the same for our line Pl(t):
	'         *
	'         *     t0l = ((x - x1)*ky - (y - y1)*kx) /
	'         *           ((x2 - x1)*ky - (y2 - y1)*kx)
	'         *
	'         * It's easy to check that t0q == t0l. This fact means
	'         * we can compute t0 only one time.
	'         *
	'         * In case t0 < 0 or t0 > 1, we have an intersections outside
	'         * of shape bounds. So, P is definitely out of shape.
	'         *
	'         * In case t0 is inside [0:1], we should calculate Pq(t0)
	'         * and Pl(t0). We have three points for now, and all of them
	'         * lie on one line. So, we just need to detect, is our point
	'         * of interest between points of intersections or not.
	'         *
	'         * If the denominator in the t0q and t0l equations is
	'         * zero, then the points must be collinear and so the
	'         * curve is degenerate and encloses no area.  Thus the
	'         * result is false.
	'         
			Dim kx As Double = x1_Renamed - 2 * xc + x2_Renamed
			Dim ky As Double = y1_Renamed - 2 * yc + y2_Renamed
			Dim dx As Double = x - x1_Renamed
			Dim dy As Double = y - y1_Renamed
			Dim dxl As Double = x2_Renamed - x1_Renamed
			Dim dyl As Double = y2_Renamed - y1_Renamed

			Dim t0 As Double = (dx * ky - dy * kx) / (dxl * ky - dyl * kx)
			If t0 < 0 OrElse t0 > 1 OrElse t0 <> t0 Then Return False

			Dim xb As Double = kx * t0 * t0 + 2 * (xc - x1_Renamed) * t0 + x1_Renamed
			Dim yb As Double = ky * t0 * t0 + 2 * (yc - y1_Renamed) * t0 + y1_Renamed
			Dim xl As Double = dxl * t0 + x1_Renamed
			Dim yl As Double = dyl * t0 + y1_Renamed

			Return (x >= xb AndAlso x < xl) OrElse (x >= xl AndAlso x < xb) OrElse (y >= yb AndAlso y < yl) OrElse (y >= yl AndAlso y < yb)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Point2D p)
			Return contains(p.x, p.y)

		''' <summary>
		''' Fill an array with the coefficients of the parametric equation
		''' in t, ready for solving against val with solveQuadratic.
		''' We currently have:
		'''     val = Py(t) = C1*(1-t)^2 + 2*CP*t*(1-t) + C2*t^2
		'''                 = C1 - 2*C1*t + C1*t^2 + 2*CP*t - 2*CP*t^2 + C2*t^2
		'''                 = C1 + (2*CP - 2*C1)*t + (C1 - 2*CP + C2)*t^2
		'''               0 = (C1 - val) + (2*CP - 2*C1)*t + (C1 - 2*CP + C2)*t^2
		'''               0 = C + Bt + At^2
		'''     C = C1 - val
		'''     B = 2*CP - 2*C1
		'''     A = C1 - 2*CP + C2
		''' </summary>
		private static void fillEqn(Double eqn() , Double val, Double c1, Double cp, Double c2)
			eqn(0) = c1 - val
			eqn(1) = cp + cp - c1 - c1
			eqn(2) = c1 - cp - cp + c2
			Return

		''' <summary>
		''' Evaluate the t values in the first num slots of the vals[] array
		''' and place the evaluated values back into the same array.  Only
		''' evaluate t values that are within the range &lt;0, 1&gt;, including
		''' the 0 and 1 ends of the range iff the include0 or include1
		''' booleans are true.  If an "inflection" equation is handed in,
		''' then any points which represent a point of inflection for that
		''' quadratic equation are also ignored.
		''' </summary>
		private static Integer evalQuadratic(Double vals() , Integer num, Boolean include0, Boolean include1, Double inflect(), Double c1, Double ctrl, Double c2)
			Dim j As Integer = 0
			For i As Integer = 0 To num - 1
				Dim t As Double = vals(i)
				If (If(include0, t >= 0, t > 0)) AndAlso (If(include1, t <= 1, t < 1)) AndAlso (inflect Is Nothing OrElse inflect(1) + 2*inflect(2)*t <> 0) Then
					Dim u As Double = 1 - t
					vals(j) = c1*u*u + 2*ctrl*t*u + c2*t*t
					j += 1
				End If
			Next i
			Return j

		private static final Integer BELOW = -2
		private static final Integer LOWEDGE = -1
		private static final Integer INSIDE = 0
		private static final Integer HIGHEDGE = 1
		private static final Integer ABOVE = 2

		''' <summary>
		''' Determine where coord lies with respect to the range from
		''' low to high.  It is assumed that low &lt;= high.  The return
		''' value is one of the 5 values BELOW, LOWEDGE, INSIDE, HIGHEDGE,
		''' or ABOVE.
		''' </summary>
		private static Integer getTag(Double coord, Double low, Double high)
			If coord <= low Then Return (If(coord < low, BELOW, LOWEDGE))
			If coord >= high Then Return (If(coord > high, ABOVE, HIGHEDGE))
			Return INSIDE

		''' <summary>
		''' Determine if the pttag represents a coordinate that is already
		''' in its test range, or is on the border with either of the two
		''' opttags representing another coordinate that is "towards the
		''' inside" of that test range.  In other words, are either of the
		''' two "opt" points "drawing the pt inward"?
		''' </summary>
		private static Boolean inwards(Integer pttag, Integer opt1tag, Integer opt2tag)
			Select Case pttag
			Case Else
				Return False
			Case LOWEDGE
				Return (opt1tag >= INSIDE OrElse opt2tag >= INSIDE)
			Case INSIDE
				Return True
			Case HIGHEDGE
				Return (opt1tag <= INSIDE OrElse opt2tag <= INSIDE)
			End Select

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean intersects(Double x, Double y, Double w, Double h)
			' Trivially reject non-existant rectangles
			If w <= 0 OrElse h <= 0 Then Return False

			' Trivially accept if either endpoint is inside the rectangle
			' (not on its border since it may end there and not go inside)
			' Record where they lie with respect to the rectangle.
			'     -1 => left, 0 => inside, 1 => right
			Dim x1_Renamed As Double = x1
			Dim y1_Renamed As Double = y1
			Dim x1tag As Integer = getTag(x1_Renamed, x, x+w)
			Dim y1tag As Integer = getTag(y1_Renamed, y, y+h)
			If x1tag = INSIDE AndAlso y1tag = INSIDE Then Return True
			Dim x2_Renamed As Double = x2
			Dim y2_Renamed As Double = y2
			Dim x2tag As Integer = getTag(x2_Renamed, x, x+w)
			Dim y2tag As Integer = getTag(y2_Renamed, y, y+h)
			If x2tag = INSIDE AndAlso y2tag = INSIDE Then Return True
			Dim ctrlx_Renamed As Double = ctrlX
			Dim ctrly_Renamed As Double = ctrlY
			Dim ctrlxtag As Integer = getTag(ctrlx_Renamed, x, x+w)
			Dim ctrlytag As Integer = getTag(ctrly_Renamed, y, y+h)

			' Trivially reject if all points are entirely to one side of
			' the rectangle.
			If x1tag < INSIDE AndAlso x2tag < INSIDE AndAlso ctrlxtag < INSIDE Then Return False ' All points left
			If y1tag < INSIDE AndAlso y2tag < INSIDE AndAlso ctrlytag < INSIDE Then Return False ' All points above
			If x1tag > INSIDE AndAlso x2tag > INSIDE AndAlso ctrlxtag > INSIDE Then Return False ' All points right
			If y1tag > INSIDE AndAlso y2tag > INSIDE AndAlso ctrlytag > INSIDE Then Return False ' All points below

			' Test for endpoints on the edge where either the segment
			' or the curve is headed "inwards" from them
			' Note: These tests are a superset of the fast endpoint tests
			'       above and thus repeat those tests, but take more time
			'       and cover more cases
			If inwards(x1tag, x2tag, ctrlxtag) AndAlso inwards(y1tag, y2tag, ctrlytag) Then Return True
			If inwards(x2tag, x1tag, ctrlxtag) AndAlso inwards(y2tag, y1tag, ctrlytag) Then Return True

			' Trivially accept if endpoints span directly across the rectangle
			Dim xoverlap As Boolean = (x1tag * x2tag <= 0)
			Dim yoverlap As Boolean = (y1tag * y2tag <= 0)
			If x1tag = INSIDE AndAlso x2tag = INSIDE AndAlso yoverlap Then Return True
			If y1tag = INSIDE AndAlso y2tag = INSIDE AndAlso xoverlap Then Return True

			' We now know that both endpoints are outside the rectangle
			' but the 3 points are not all on one side of the rectangle.
			' Therefore the curve cannot be contained inside the rectangle,
			' but the rectangle might be contained inside the curve, or
			' the curve might intersect the boundary of the rectangle.

			Dim eqn As Double() = New Double(2){}
			Dim res As Double() = New Double(2){}
			If Not yoverlap Then
				' Both Y coordinates for the closing segment are above or
				' below the rectangle which means that we can only intersect
				' if the curve crosses the top (or bottom) of the rectangle
				' in more than one place and if those crossing locations
				' span the horizontal range of the rectangle.
				fillEqn(eqn, (If(y1tag < INSIDE, y, y+h)), y1, ctrly, y2_Renamed)
				Return (solveQuadratic(eqn, res) = 2 AndAlso evalQuadratic(res, 2, True, True, Nothing, x1_Renamed, ctrlx_Renamed, x2_Renamed) = 2 AndAlso getTag(res(0), x, x+w) * getTag(res(1), x, x+w) <= 0)
			End If

			' Y ranges overlap.  Now we examine the X ranges
			If Not xoverlap Then
				' Both X coordinates for the closing segment are left of
				' or right of the rectangle which means that we can only
				' intersect if the curve crosses the left (or right) edge
				' of the rectangle in more than one place and if those
				' crossing locations span the vertical range of the rectangle.
				fillEqn(eqn, (If(x1tag < INSIDE, x, x+w)), x1, ctrlx, x2_Renamed)
				Return (solveQuadratic(eqn, res) = 2 AndAlso evalQuadratic(res, 2, True, True, Nothing, y1_Renamed, ctrly_Renamed, y2_Renamed) = 2 AndAlso getTag(res(0), y, y+h) * getTag(res(1), y, y+h) <= 0)
			End If

			' The X and Y ranges of the endpoints overlap the X and Y
			' ranges of the rectangle, now find out how the endpoint
			' line segment intersects the Y range of the rectangle
			Dim dx As Double = x2_Renamed - x1_Renamed
			Dim dy As Double = y2_Renamed - y1_Renamed
			Dim k As Double = y2_Renamed * x1_Renamed - x2_Renamed * y1_Renamed
			Dim c1tag, c2tag As Integer
			If y1tag = INSIDE Then
				c1tag = x1tag
			Else
				c1tag = getTag((k + dx * (If(y1tag < INSIDE, y, y+h))) / dy, x, x+w)
			End If
			If y2tag = INSIDE Then
				c2tag = x2tag
			Else
				c2tag = getTag((k + dx * (If(y2tag < INSIDE, y, y+h))) / dy, x, x+w)
			End If
			' If the part of the line segment that intersects the Y range
			' of the rectangle crosses it horizontally - trivially accept
			If c1tag * c2tag <= 0 Then Return True

			' Now we know that both the X and Y ranges intersect and that
			' the endpoint line segment does not directly cross the rectangle.
			'
			' We can almost treat this case like one of the cases above
			' where both endpoints are to one side, except that we will
			' only get one intersection of the curve with the vertical
			' side of the rectangle.  This is because the endpoint segment
			' accounts for the other intersection.
			'
			' (Remember there is overlap in both the X and Y ranges which
			'  means that the segment must cross at least one vertical edge
			'  of the rectangle - in particular, the "near vertical side" -
			'  leaving only one intersection for the curve.)
			'
			' Now we calculate the y tags of the two intersections on the
			' "near vertical side" of the rectangle.  We will have one with
			' the endpoint segment, and one with the curve.  If those two
			' vertical intersections overlap the Y range of the rectangle,
			' we have an intersection.  Otherwise, we don't.

			' c1tag = vertical intersection class of the endpoint segment
			'
			' Choose the y tag of the endpoint that was not on the same
			' side of the rectangle as the subsegment calculated above.
			' Note that we can "steal" the existing Y tag of that endpoint
			' since it will be provably the same as the vertical intersection.
			c1tag = (If(c1tag * x1tag <= 0, y1tag, y2tag))

			' c2tag = vertical intersection class of the curve
			'
			' We have to calculate this one the straightforward way.
			' Note that the c2tag can still tell us which vertical edge
			' to test against.
			fillEqn(eqn, (If(c2tag < INSIDE, x, x+w)), x1, ctrlx, x2_Renamed)
			Dim num As Integer = solveQuadratic(eqn, res)

			' Note: We should be able to assert(num == 2); since the
			' X range "crosses" (not touches) the vertical boundary,
			' but we pass num to evalQuadratic for completeness.
			evalQuadratic(res, num, True, True, Nothing, y1_Renamed, ctrly_Renamed, y2_Renamed)

			' Note: We can assert(num evals == 1); since one of the
			' 2 crossings will be out of the [0,1] range.
			c2tag = getTag(res(0), y, y+h)

			' Finally, we have an intersection if the two crossings
			' overlap the Y range of the rectangle.
			Return (c1tag * c2tag <= 0)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean intersects(Rectangle2D r)
			Return intersects(r.x, r.y, r.width, r.height)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Double x, Double y, Double w, Double h)
			If w <= 0 OrElse h <= 0 Then Return False
			' Assertion: Quadratic curves closed by connecting their
			' endpoints are always convex.
			Return (contains(x, y) AndAlso contains(x + w, y) AndAlso contains(x + w, y + h) AndAlso contains(x, y + h))

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Rectangle2D r)
			Return contains(r.x, r.y, r.width, r.height)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public java.awt.Rectangle bounds
			Return bounds2D.bounds

		''' <summary>
		''' Returns an iteration object that defines the boundary of the
		''' shape of this <code>QuadCurve2D</code>.
		''' The iterator for this class is not multi-threaded safe,
		''' which means that this <code>QuadCurve2D</code> class does not
		''' guarantee that modifications to the geometry of this
		''' <code>QuadCurve2D</code> object do not affect any iterations of
		''' that geometry that are already in process. </summary>
		''' <param name="at"> an optional <seealso cref="AffineTransform"/> to apply to the
		'''          shape boundary </param>
		''' <returns> a <seealso cref="PathIterator"/> object that defines the boundary
		'''          of the shape.
		''' @since 1.2 </returns>
		public PathIterator getPathIterator(AffineTransform at)
			Return New QuadIterator(Me, at)

		''' <summary>
		''' Returns an iteration object that defines the boundary of the
		''' flattened shape of this <code>QuadCurve2D</code>.
		''' The iterator for this class is not multi-threaded safe,
		''' which means that this <code>QuadCurve2D</code> class does not
		''' guarantee that modifications to the geometry of this
		''' <code>QuadCurve2D</code> object do not affect any iterations of
		''' that geometry that are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to apply
		'''          to the boundary of the shape </param>
		''' <param name="flatness"> the maximum distance that the control points for a
		'''          subdivided curve can be with respect to a line connecting
		'''          the end points of this curve before this curve is
		'''          replaced by a straight line connecting the end points. </param>
		''' <returns> a <code>PathIterator</code> object that defines the
		'''          flattened boundary of the shape.
		''' @since 1.2 </returns>
		public PathIterator getPathIterator(AffineTransform at, Double flatness)
			Return New FlatteningPathIterator(getPathIterator(at), flatness)

		''' <summary>
		''' Creates a new object of the same class and with the same contents
		''' as this object.
		''' </summary>
		''' <returns>     a clone of this instance. </returns>
		''' <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		''' <seealso cref=        java.lang.Cloneable
		''' @since      1.2 </seealso>
		public Object clone()
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
	End Class

End Namespace