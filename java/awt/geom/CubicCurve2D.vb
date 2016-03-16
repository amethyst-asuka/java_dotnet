Imports System
import static System.Math.abs
import static System.Math.max
import static System.Math.ulp

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>CubicCurve2D</code> class defines a cubic parametric curve
	''' segment in {@code (x,y)} coordinate space.
	''' <p>
	''' This class is only the abstract superclass for all objects which
	''' store a 2D cubic curve segment.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class CubicCurve2D
		Implements java.awt.Shape, Cloneable

		''' <summary>
		''' A cubic parametric curve segment specified with
		''' {@code float} coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits CubicCurve2D

			''' <summary>
			''' The X coordinate of the start point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x1 As Single

			''' <summary>
			''' The Y coordinate of the start point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y1 As Single

			''' <summary>
			''' The X coordinate of the first control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx1 As Single

			''' <summary>
			''' The Y coordinate of the first control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly1 As Single

			''' <summary>
			''' The X coordinate of the second control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx2 As Single

			''' <summary>
			''' The Y coordinate of the second control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly2 As Single

			''' <summary>
			''' The X coordinate of the end point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x2 As Single

			''' <summary>
			''' The Y coordinate of the end point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y2 As Single

			''' <summary>
			''' Constructs and initializes a CubicCurve with coordinates
			''' (0, 0, 0, 0, 0, 0, 0, 0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes a {@code CubicCurve2D} from
			''' the specified {@code float} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate for the start point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="y1"> the Y coordinate for the start point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrlx1"> the X coordinate for the first control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrly1"> the Y coordinate for the first control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrlx2"> the X coordinate for the second control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrly2"> the Y coordinate for the second control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="x2"> the X coordinate for the end point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="y2"> the Y coordinate for the end point
			'''           of the resulting {@code CubicCurve2D}
			''' @since 1.2 </param>
			Public Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal ctrlx1 As Single, ByVal ctrly1 As Single, ByVal ctrlx2 As Single, ByVal ctrly2 As Single, ByVal x2 As Single, ByVal y2 As Single)
				curverve(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2)
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
			Public Property Overrides ctrlX1 As Double
				Get
					Return CDbl(ctrlx1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY1 As Double
				Get
					Return CDbl(ctrly1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlP1 As Point2D
				Get
					Return New Point2D.Float(ctrlx1, ctrly1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlX2 As Double
				Get
					Return CDbl(ctrlx2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY2 As Double
				Get
					Return CDbl(ctrly2)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlP2 As Point2D
				Get
					Return New Point2D.Float(ctrlx2, ctrly2)
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
			Public Overrides Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double)
				Me.x1 = CSng(x1)
				Me.y1 = CSng(y1)
				Me.ctrlx1 = CSng(ctrlx1)
				Me.ctrly1 = CSng(ctrly1)
				Me.ctrlx2 = CSng(ctrlx2)
				Me.ctrly2 = CSng(ctrly2)
				Me.x2 = CSng(x2)
				Me.y2 = CSng(y2)
			End Sub

			''' <summary>
			''' Sets the location of the end points and control points
			''' of this curve to the specified {@code float} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate used to set the start point
			'''           of this {@code CubicCurve2D} </param>
			''' <param name="y1"> the Y coordinate used to set the start point
			'''           of this {@code CubicCurve2D} </param>
			''' <param name="ctrlx1"> the X coordinate used to set the first control point
			'''               of this {@code CubicCurve2D} </param>
			''' <param name="ctrly1"> the Y coordinate used to set the first control point
			'''               of this {@code CubicCurve2D} </param>
			''' <param name="ctrlx2"> the X coordinate used to set the second control point
			'''               of this {@code CubicCurve2D} </param>
			''' <param name="ctrly2"> the Y coordinate used to set the second control point
			'''               of this {@code CubicCurve2D} </param>
			''' <param name="x2"> the X coordinate used to set the end point
			'''           of this {@code CubicCurve2D} </param>
			''' <param name="y2"> the Y coordinate used to set the end point
			'''           of this {@code CubicCurve2D}
			''' @since 1.2 </param>
			Public Overridable Sub setCurve(ByVal x1 As Single, ByVal y1 As Single, ByVal ctrlx1 As Single, ByVal ctrly1 As Single, ByVal ctrlx2 As Single, ByVal ctrly2 As Single, ByVal x2 As Single, ByVal y2 As Single)
				Me.x1 = x1
				Me.y1 = y1
				Me.ctrlx1 = ctrlx1
				Me.ctrly1 = ctrly1
				Me.ctrlx2 = ctrlx2
				Me.ctrly2 = ctrly2
				Me.x2 = x2
				Me.y2 = y2
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Dim left As Single = System.Math.Min (System.Math.Min(x1, x2), System.Math.Min(ctrlx1, ctrlx2))
					Dim top As Single = System.Math.Min (System.Math.Min(y1, y2), System.Math.Min(ctrly1, ctrly2))
					Dim right As Single = System.Math.Max (System.Math.Max(x1, x2), System.Math.Max(ctrlx1, ctrlx2))
					Dim bottom As Single = System.Math.Max (System.Math.Max(y1, y2), System.Math.Max(ctrly1, ctrly2))
					Return New Rectangle2D.Float(left, top, right - left, bottom - top)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -1272015596714244385L
		End Class

		''' <summary>
		''' A cubic parametric curve segment specified with
		''' {@code double} coordinates.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits CubicCurve2D

			''' <summary>
			''' The X coordinate of the start point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x1 As Double

			''' <summary>
			''' The Y coordinate of the start point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y1 As Double

			''' <summary>
			''' The X coordinate of the first control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx1 As Double

			''' <summary>
			''' The Y coordinate of the first control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly1 As Double

			''' <summary>
			''' The X coordinate of the second control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrlx2 As Double

			''' <summary>
			''' The Y coordinate of the second control point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public ctrly2 As Double

			''' <summary>
			''' The X coordinate of the end point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x2 As Double

			''' <summary>
			''' The Y coordinate of the end point
			''' of the cubic curve segment.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y2 As Double

			''' <summary>
			''' Constructs and initializes a CubicCurve with coordinates
			''' (0, 0, 0, 0, 0, 0, 0, 0).
			''' @since 1.2
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
			End Function

			''' <summary>
			''' Constructs and initializes a {@code CubicCurve2D} from
			''' the specified {@code double} coordinates.
			''' </summary>
			''' <param name="x1"> the X coordinate for the start point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="y1"> the Y coordinate for the start point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrlx1"> the X coordinate for the first control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrly1"> the Y coordinate for the first control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrlx2"> the X coordinate for the second control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="ctrly2"> the Y coordinate for the second control point
			'''               of the resulting {@code CubicCurve2D} </param>
			''' <param name="x2"> the X coordinate for the end point
			'''           of the resulting {@code CubicCurve2D} </param>
			''' <param name="y2"> the Y coordinate for the end point
			'''           of the resulting {@code CubicCurve2D}
			''' @since 1.2 </param>
			Function java.lang.Double(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double) As [Public] Overridable
				curverve(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2)
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
			Public Property Overrides ctrlX1 As Double
				Get
					Return ctrlx1
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY1 As Double
				Get
					Return ctrly1
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlP1 As Point2D
				Get
					Return New Point2D.Double(ctrlx1, ctrly1)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlX2 As Double
				Get
					Return ctrlx2
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlY2 As Double
				Get
					Return ctrly2
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides ctrlP2 As Point2D
				Get
					Return New Point2D.Double(ctrlx2, ctrly2)
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
			Public Overrides Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double)
				Me.x1 = x1
				Me.y1 = y1
				Me.ctrlx1 = ctrlx1
				Me.ctrly1 = ctrly1
				Me.ctrlx2 = ctrlx2
				Me.ctrly2 = ctrly2
				Me.x2 = x2
				Me.y2 = y2
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Dim left As Double = System.Math.Min (System.Math.Min(x1, x2), System.Math.Min(ctrlx1, ctrlx2))
					Dim top As Double = System.Math.Min (System.Math.Min(y1, y2), System.Math.Min(ctrly1, ctrly2))
					Dim right As Double = System.Math.Max (System.Math.Max(x1, x2), System.Math.Max(ctrlx1, ctrlx2))
					Dim bottom As Double = System.Math.Max (System.Math.Max(y1, y2), System.Math.Max(ctrly1, ctrly2))
					Return New Rectangle2D.Double(left, top, right - left, bottom - top)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -4202960122839707295L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.CubicCurve2D.Float </seealso>
		''' <seealso cref= java.awt.geom.CubicCurve2D.Double
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the X coordinate of the start point in double precision. </summary>
		''' <returns> the X coordinate of the start point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property x1 As Double

		''' <summary>
		''' Returns the Y coordinate of the start point in double precision. </summary>
		''' <returns> the Y coordinate of the start point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property y1 As Double

		''' <summary>
		''' Returns the start point. </summary>
		''' <returns> a {@code Point2D} that is the start point of
		'''         the {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property p1 As Point2D

		''' <summary>
		''' Returns the X coordinate of the first control point in double precision. </summary>
		''' <returns> the X coordinate of the first control point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlX1 As Double

		''' <summary>
		''' Returns the Y coordinate of the first control point in double precision. </summary>
		''' <returns> the Y coordinate of the first control point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlY1 As Double

		''' <summary>
		''' Returns the first control point. </summary>
		''' <returns> a {@code Point2D} that is the first control point of
		'''         the {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlP1 As Point2D

		''' <summary>
		''' Returns the X coordinate of the second control point
		''' in double precision. </summary>
		''' <returns> the X coordinate of the second control point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlX2 As Double

		''' <summary>
		''' Returns the Y coordinate of the second control point
		''' in double precision. </summary>
		''' <returns> the Y coordinate of the second control point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlY2 As Double

		''' <summary>
		''' Returns the second control point. </summary>
		''' <returns> a {@code Point2D} that is the second control point of
		'''         the {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property ctrlP2 As Point2D

		''' <summary>
		''' Returns the X coordinate of the end point in double precision. </summary>
		''' <returns> the X coordinate of the end point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property x2 As Double

		''' <summary>
		''' Returns the Y coordinate of the end point in double precision. </summary>
		''' <returns> the Y coordinate of the end point of the
		'''         {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property y2 As Double

		''' <summary>
		''' Returns the end point. </summary>
		''' <returns> a {@code Point2D} that is the end point of
		'''         the {@code CubicCurve2D}.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property p2 As Point2D

		''' <summary>
		''' Sets the location of the end points and control points of this curve
		''' to the specified double coordinates.
		''' </summary>
		''' <param name="x1"> the X coordinate used to set the start point
		'''           of this {@code CubicCurve2D} </param>
		''' <param name="y1"> the Y coordinate used to set the start point
		'''           of this {@code CubicCurve2D} </param>
		''' <param name="ctrlx1"> the X coordinate used to set the first control point
		'''               of this {@code CubicCurve2D} </param>
		''' <param name="ctrly1"> the Y coordinate used to set the first control point
		'''               of this {@code CubicCurve2D} </param>
		''' <param name="ctrlx2"> the X coordinate used to set the second control point
		'''               of this {@code CubicCurve2D} </param>
		''' <param name="ctrly2"> the Y coordinate used to set the second control point
		'''               of this {@code CubicCurve2D} </param>
		''' <param name="x2"> the X coordinate used to set the end point
		'''           of this {@code CubicCurve2D} </param>
		''' <param name="y2"> the Y coordinate used to set the end point
		'''           of this {@code CubicCurve2D}
		''' @since 1.2 </param>
		Public MustOverride Sub setCurve(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double)

		''' <summary>
		''' Sets the location of the end points and control points of this curve
		''' to the double coordinates at the specified offset in the specified
		''' array. </summary>
		''' <param name="coords"> a double array containing coordinates </param>
		''' <param name="offset"> the index of <code>coords</code> from which to begin
		'''          setting the end points and control points of this curve
		'''          to the coordinates contained in <code>coords</code>
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal coords As Double(), ByVal offset As Integer)
			curverve(coords(offset + 0), coords(offset + 1), coords(offset + 2), coords(offset + 3), coords(offset + 4), coords(offset + 5), coords(offset + 6), coords(offset + 7))
		End Sub

		''' <summary>
		''' Sets the location of the end points and control points of this curve
		''' to the specified <code>Point2D</code> coordinates. </summary>
		''' <param name="p1"> the first specified <code>Point2D</code> used to set the
		'''          start point of this curve </param>
		''' <param name="cp1"> the second specified <code>Point2D</code> used to set the
		'''          first control point of this curve </param>
		''' <param name="cp2"> the third specified <code>Point2D</code> used to set the
		'''          second control point of this curve </param>
		''' <param name="p2"> the fourth specified <code>Point2D</code> used to set the
		'''          end point of this curve
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal p1 As Point2D, ByVal cp1 As Point2D, ByVal cp2 As Point2D, ByVal p2 As Point2D)
			curverve(p1.x, p1.y, cp1.x, cp1.y, cp2.x, cp2.y, p2.x, p2.y)
		End Sub

		''' <summary>
		''' Sets the location of the end points and control points of this curve
		''' to the coordinates of the <code>Point2D</code> objects at the specified
		''' offset in the specified array. </summary>
		''' <param name="pts"> an array of <code>Point2D</code> objects </param>
		''' <param name="offset">  the index of <code>pts</code> from which to begin setting
		'''          the end points and control points of this curve to the
		'''          points contained in <code>pts</code>
		''' @since 1.2 </param>
		Public Overridable Sub setCurve(ByVal pts As Point2D(), ByVal offset As Integer)
			curverve(pts(offset + 0).x, pts(offset + 0).y, pts(offset + 1).x, pts(offset + 1).y, pts(offset + 2).x, pts(offset + 2).y, pts(offset + 3).x, pts(offset + 3).y)
		End Sub

		''' <summary>
		''' Sets the location of the end points and control points of this curve
		''' to the same as those in the specified <code>CubicCurve2D</code>. </summary>
		''' <param name="c"> the specified <code>CubicCurve2D</code>
		''' @since 1.2 </param>
		Public Overridable Property curve As CubicCurve2D
			Set(ByVal c As CubicCurve2D)
				curverve(c.x1, c.y1, c.ctrlX1, c.ctrlY1, c.ctrlX2, c.ctrlY2, c.x2, c.y2)
			End Set
		End Property

		''' <summary>
		''' Returns the square of the flatness of the cubic curve specified
		''' by the indicated control points. The flatness is the maximum distance
		''' of a control point from the line connecting the end points.
		''' </summary>
		''' <param name="x1"> the X coordinate that specifies the start point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="y1"> the Y coordinate that specifies the start point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="ctrlx1"> the X coordinate that specifies the first control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrly1"> the Y coordinate that specifies the first control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrlx2"> the X coordinate that specifies the second control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrly2"> the Y coordinate that specifies the second control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="x2"> the X coordinate that specifies the end point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="y2"> the Y coordinate that specifies the end point
		'''           of a {@code CubicCurve2D} </param>
		''' <returns> the square of the flatness of the {@code CubicCurve2D}
		'''          represented by the specified coordinates.
		''' @since 1.2 </returns>
		Public Shared Function getFlatnessSq(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
			Return System.Math.Max(Line2D.ptSegDistSq(x1, y1, x2, y2, ctrlx1, ctrly1), Line2D.ptSegDistSq(x1, y1, x2, y2, ctrlx2, ctrly2))

		End Function

		''' <summary>
		''' Returns the flatness of the cubic curve specified
		''' by the indicated control points. The flatness is the maximum distance
		''' of a control point from the line connecting the end points.
		''' </summary>
		''' <param name="x1"> the X coordinate that specifies the start point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="y1"> the Y coordinate that specifies the start point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="ctrlx1"> the X coordinate that specifies the first control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrly1"> the Y coordinate that specifies the first control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrlx2"> the X coordinate that specifies the second control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="ctrly2"> the Y coordinate that specifies the second control point
		'''               of a {@code CubicCurve2D} </param>
		''' <param name="x2"> the X coordinate that specifies the end point
		'''           of a {@code CubicCurve2D} </param>
		''' <param name="y2"> the Y coordinate that specifies the end point
		'''           of a {@code CubicCurve2D} </param>
		''' <returns> the flatness of the {@code CubicCurve2D}
		'''          represented by the specified coordinates.
		''' @since 1.2 </returns>
		Public Shared Function getFlatness(ByVal x1 As Double, ByVal y1 As Double, ByVal ctrlx1 As Double, ByVal ctrly1 As Double, ByVal ctrlx2 As Double, ByVal ctrly2 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
			Return System.Math.Sqrt(getFlatnessSq(x1, y1, ctrlx1, ctrly1, ctrlx2, ctrly2, x2, y2))
		End Function

		''' <summary>
		''' Returns the square of the flatness of the cubic curve specified
		''' by the control points stored in the indicated array at the
		''' indicated index. The flatness is the maximum distance
		''' of a control point from the line connecting the end points. </summary>
		''' <param name="coords"> an array containing coordinates </param>
		''' <param name="offset"> the index of <code>coords</code> from which to begin
		'''          getting the end points and control points of the curve </param>
		''' <returns> the square of the flatness of the <code>CubicCurve2D</code>
		'''          specified by the coordinates in <code>coords</code> at
		'''          the specified offset.
		''' @since 1.2 </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public static double getFlatnessSq(double coords() , int offset)
			Return getFlatnessSq(coords(offset + 0), coords(offset + 1), coords(offset + 2), coords(offset + 3), coords(offset + 4), coords(offset + 5), coords(offset + 6), coords(offset + 7))

		''' <summary>
		''' Returns the flatness of the cubic curve specified
		''' by the control points stored in the indicated array at the
		''' indicated index.  The flatness is the maximum distance
		''' of a control point from the line connecting the end points. </summary>
		''' <param name="coords"> an array containing coordinates </param>
		''' <param name="offset"> the index of <code>coords</code> from which to begin
		'''          getting the end points and control points of the curve </param>
		''' <returns> the flatness of the <code>CubicCurve2D</code>
		'''          specified by the coordinates in <code>coords</code> at
		'''          the specified offset.
		''' @since 1.2 </returns>
		public static Double getFlatness(Double coords() , Integer offset)
			Return getFlatness(coords(offset + 0), coords(offset + 1), coords(offset + 2), coords(offset + 3), coords(offset + 4), coords(offset + 5), coords(offset + 6), coords(offset + 7))

		''' <summary>
		''' Returns the square of the flatness of this curve.  The flatness is the
		''' maximum distance of a control point from the line connecting the
		''' end points. </summary>
		''' <returns> the square of the flatness of this curve.
		''' @since 1.2 </returns>
		public Double flatnessSq
			Return getFlatnessSq(x1, y1, ctrlX1, ctrlY1, ctrlX2, ctrlY2, x2, y2)

		''' <summary>
		''' Returns the flatness of this curve.  The flatness is the
		''' maximum distance of a control point from the line connecting the
		''' end points. </summary>
		''' <returns> the flatness of this curve.
		''' @since 1.2 </returns>
		public Double flatness
			Return getFlatness(x1, y1, ctrlX1, ctrlY1, ctrlX2, ctrlY2, x2, y2)

		''' <summary>
		''' Subdivides this cubic curve and stores the resulting two
		''' subdivided curves into the left and right curve parameters.
		''' Either or both of the left and right objects may be the same
		''' as this object or null. </summary>
		''' <param name="left"> the cubic curve object for storing for the left or
		''' first half of the subdivided curve </param>
		''' <param name="right"> the cubic curve object for storing for the right or
		''' second half of the subdivided curve
		''' @since 1.2 </param>
		public  Sub  subdivide(CubicCurve2D left, CubicCurve2D right)
			subdivide(Me, left, right)

		''' <summary>
		''' Subdivides the cubic curve specified by the <code>src</code> parameter
		''' and stores the resulting two subdivided curves into the
		''' <code>left</code> and <code>right</code> curve parameters.
		''' Either or both of the <code>left</code> and <code>right</code> objects
		''' may be the same as the <code>src</code> object or <code>null</code>. </summary>
		''' <param name="src"> the cubic curve to be subdivided </param>
		''' <param name="left"> the cubic curve object for storing the left or
		''' first half of the subdivided curve </param>
		''' <param name="right"> the cubic curve object for storing the right or
		''' second half of the subdivided curve
		''' @since 1.2 </param>
		public static  Sub  subdivide(CubicCurve2D src, CubicCurve2D left, CubicCurve2D right)
			Dim x1_Renamed As Double = src.x1
			Dim y1_Renamed As Double = src.y1
			Dim ctrlx1_Renamed As Double = src.ctrlX1
			Dim ctrly1_Renamed As Double = src.ctrlY1
			Dim ctrlx2_Renamed As Double = src.ctrlX2
			Dim ctrly2_Renamed As Double = src.ctrlY2
			Dim x2_Renamed As Double = src.x2
			Dim y2_Renamed As Double = src.y2
			Dim centerx As Double = (ctrlx1_Renamed + ctrlx2_Renamed) / 2.0
			Dim centery As Double = (ctrly1_Renamed + ctrly2_Renamed) / 2.0
			ctrlx1_Renamed = (x1_Renamed + ctrlx1_Renamed) / 2.0
			ctrly1_Renamed = (y1_Renamed + ctrly1_Renamed) / 2.0
			ctrlx2_Renamed = (x2_Renamed + ctrlx2_Renamed) / 2.0
			ctrly2_Renamed = (y2_Renamed + ctrly2_Renamed) / 2.0
			Dim ctrlx12 As Double = (ctrlx1_Renamed + centerx) / 2.0
			Dim ctrly12 As Double = (ctrly1_Renamed + centery) / 2.0
			Dim ctrlx21 As Double = (ctrlx2_Renamed + centerx) / 2.0
			Dim ctrly21 As Double = (ctrly2_Renamed + centery) / 2.0
			centerx = (ctrlx12 + ctrlx21) / 2.0
			centery = (ctrly12 + ctrly21) / 2.0
			If left IsNot Nothing Then left.curverve(x1_Renamed, y1_Renamed, ctrlx1_Renamed, ctrly1_Renamed, ctrlx12, ctrly12, centerx, centery)
			If right IsNot Nothing Then right.curverve(centerx, centery, ctrlx21, ctrly21, ctrlx2_Renamed, ctrly2_Renamed, x2_Renamed, y2_Renamed)

		''' <summary>
		''' Subdivides the cubic curve specified by the coordinates
		''' stored in the <code>src</code> array at indices <code>srcoff</code>
		''' through (<code>srcoff</code>&nbsp;+&nbsp;7) and stores the
		''' resulting two subdivided curves into the two result arrays at the
		''' corresponding indices.
		''' Either or both of the <code>left</code> and <code>right</code>
		''' arrays may be <code>null</code> or a reference to the same array
		''' as the <code>src</code> array.
		''' Note that the last point in the first subdivided curve is the
		''' same as the first point in the second subdivided curve. Thus,
		''' it is possible to pass the same array for <code>left</code>
		''' and <code>right</code> and to use offsets, such as <code>rightoff</code>
		''' equals (<code>leftoff</code> + 6), in order
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
		public static  Sub  subdivide(Double src() , Integer srcoff, Double left(), Integer leftoff, Double right(), Integer rightoff)
			Dim x1_Renamed As Double = src(srcoff + 0)
			Dim y1_Renamed As Double = src(srcoff + 1)
			Dim ctrlx1_Renamed As Double = src(srcoff + 2)
			Dim ctrly1_Renamed As Double = src(srcoff + 3)
			Dim ctrlx2_Renamed As Double = src(srcoff + 4)
			Dim ctrly2_Renamed As Double = src(srcoff + 5)
			Dim x2_Renamed As Double = src(srcoff + 6)
			Dim y2_Renamed As Double = src(srcoff + 7)
			If left IsNot Nothing Then
				left(leftoff + 0) = x1_Renamed
				left(leftoff + 1) = y1_Renamed
			End If
			If right IsNot Nothing Then
				right(rightoff + 6) = x2_Renamed
				right(rightoff + 7) = y2_Renamed
			End If
			x1_Renamed = (x1_Renamed + ctrlx1_Renamed) / 2.0
			y1_Renamed = (y1_Renamed + ctrly1_Renamed) / 2.0
			x2_Renamed = (x2_Renamed + ctrlx2_Renamed) / 2.0
			y2_Renamed = (y2_Renamed + ctrly2_Renamed) / 2.0
			Dim centerx As Double = (ctrlx1_Renamed + ctrlx2_Renamed) / 2.0
			Dim centery As Double = (ctrly1_Renamed + ctrly2_Renamed) / 2.0
			ctrlx1_Renamed = (x1_Renamed + centerx) / 2.0
			ctrly1_Renamed = (y1_Renamed + centery) / 2.0
			ctrlx2_Renamed = (x2_Renamed + centerx) / 2.0
			ctrly2_Renamed = (y2_Renamed + centery) / 2.0
			centerx = (ctrlx1_Renamed + ctrlx2_Renamed) / 2.0
			centery = (ctrly1_Renamed + ctrly2_Renamed) / 2.0
			If left IsNot Nothing Then
				left(leftoff + 2) = x1_Renamed
				left(leftoff + 3) = y1_Renamed
				left(leftoff + 4) = ctrlx1_Renamed
				left(leftoff + 5) = ctrly1_Renamed
				left(leftoff + 6) = centerx
				left(leftoff + 7) = centery
			End If
			If right IsNot Nothing Then
				right(rightoff + 0) = centerx
				right(rightoff + 1) = centery
				right(rightoff + 2) = ctrlx2_Renamed
				right(rightoff + 3) = ctrly2_Renamed
				right(rightoff + 4) = x2_Renamed
				right(rightoff + 5) = y2_Renamed
			End If

		''' <summary>
		''' Solves the cubic whose coefficients are in the <code>eqn</code>
		''' array and places the non-complex roots back into the same array,
		''' returning the number of roots.  The solved cubic is represented
		''' by the equation:
		''' <pre>
		'''     eqn = {c, b, a, d}
		'''     dx^3 + ax^2 + bx + c = 0
		''' </pre>
		''' A return value of -1 is used to distinguish a constant equation
		''' that might be always 0 or never 0 from an equation that has no
		''' zeroes. </summary>
		''' <param name="eqn"> an array containing coefficients for a cubic </param>
		''' <returns> the number of roots, or -1 if the equation is a constant.
		''' @since 1.2 </returns>
		public static Integer solveCubic(Double eqn())
			Return solveCubic(eqn, eqn)

		''' <summary>
		''' Solve the cubic whose coefficients are in the <code>eqn</code>
		''' array and place the non-complex roots into the <code>res</code>
		''' array, returning the number of roots.
		''' The cubic solved is represented by the equation:
		'''     eqn = {c, b, a, d}
		'''     dx^3 + ax^2 + bx + c = 0
		''' A return value of -1 is used to distinguish a constant equation,
		''' which may be always 0 or never 0, from an equation which has no
		''' zeroes. </summary>
		''' <param name="eqn"> the specified array of coefficients to use to solve
		'''        the cubic equation </param>
		''' <param name="res"> the array that contains the non-complex roots
		'''        resulting from the solution of the cubic equation </param>
		''' <returns> the number of roots, or -1 if the equation is a constant
		''' @since 1.3 </returns>
		public static Integer solveCubic(Double eqn() , Double res())
			' From Graphics Gems:
			' http://tog.acm.org/resources/GraphicsGems/gems/Roots3And4.c
			Dim d As Double = eqn(3)
			If d = 0 Then Return QuadCurve2D.solveQuadratic(eqn, res)

			' normal form: x^3 + Ax^2 + Bx + C = 0 
			Dim A As Double = eqn(2) / d
			Dim B As Double = eqn(1) / d
			Dim C As Double = eqn(0) / d


			'  substitute x = y - A/3 to eliminate quadratic term:
			'     x^3 +Px + Q = 0
			'
			' Since we actually need P/3 and Q/2 for all of the
			' calculations that follow, we will calculate
			' p = P/3
			' q = Q/2
			' instead and use those values for simplicity of the code.
			Dim sq_A As Double = A * A
			Dim p As Double = 1.0/3 * (-1.0/3 * sq_A + B)
			Dim q As Double = 1.0/2 * (2.0/27 * A * sq_A - 1.0/3 * A * B + C)

			' use Cardano's formula 

			Dim cb_p As Double = p * p * p
			Dim D_Renamed As Double = q * q + cb_p

			Dim [sub] As Double = 1.0/3 * A

			Dim num As Integer
			If D_Renamed < 0 Then ' Casus irreducibilis: three real solutions
				' see: http://en.wikipedia.org/wiki/Cubic_function#Trigonometric_.28and_hyperbolic.29_method
				Dim phi As Double = 1.0/3 * System.Math.Acos(-q / System.Math.Sqrt(-cb_p))
				Dim t As Double = 2 * System.Math.Sqrt(-p)

				If res = eqn Then eqn = java.util.Arrays.copyOf(eqn, 4)

				res(0) = (t * System.Math.Cos(phi))
				res(1) = (-t * System.Math.Cos(phi + System.Math.PI / 3))
				res(2) = (-t * System.Math.Cos(phi - System.Math.PI / 3))
				num = 3

				For i As Integer = 0 To num - 1
					res(i) -= [sub]
				Next i

			Else
				' Please see the comment in fixRoots marked 'XXX' before changing
				' any of the code in this case.
				Dim sqrt_D As Double = System.Math.Sqrt(D_Renamed)
				Dim u As Double = System.Math.cbrt(sqrt_D - q)
				Dim v As Double = - System.Math.cbrt(sqrt_D + q)
				Dim uv As Double = u+v

				num = 1

				Dim err As Double = 1200000000*ulp(abs(uv) + abs([sub]))
				If iszero(D_Renamed, err) OrElse within(u, v, err) Then
					If res = eqn Then eqn = java.util.Arrays.copyOf(eqn, 4)
					res(1) = -(uv / 2) - [sub]
					num = 2
				End If
				' this must be done after the potential Arrays.copyOf
				res(0) = uv - [sub]
			End If

			If num > 1 Then ' num == 3 || num == 2 num = fixRoots(eqn, res, num)
			If num > 2 AndAlso (res(2) = res(1) OrElse res(2) = res(0)) Then num -= 1
			If num > 1 AndAlso res(1) = res(0) Then
				num -= 1
				res(1) = res(num)
			End If
			Return num

		' preconditions: eqn != res && eqn[3] != 0 && num > 1
		' This method tries to improve the accuracy of the roots of eqn (which
		' should be in res). It also might eliminate roots in res if it decideds
		' that they're not real roots. It will not check for roots that the
		' computation of res might have missed, so this method should only be
		' used when the roots in res have been computed using an algorithm
		' that never underestimates the number of roots (such as solveCubic above)
		private static Integer fixRoots(Double() eqn, Double() res, Integer num)
			Dim intervals As Double() = {eqn(1), 2*eqn(2), 3*eqn(3)}
			Dim critCount As Integer = QuadCurve2D.solveQuadratic(intervals, intervals)
			If critCount = 2 AndAlso intervals(0) = intervals(1) Then critCount -= 1
			If critCount = 2 AndAlso intervals(0) > intervals(1) Then
				Dim tmp As Double = intervals(0)
				intervals(0) = intervals(1)
				intervals(1) = tmp
			End If

			' below we use critCount to possibly filter out roots that shouldn't
			' have been computed. We require that eqn[3] != 0, so eqn is a proper
			' cubic, which means that its limits at -/+inf are -/+inf or +/-inf.
			' Therefore, if critCount==2, the curve is shaped like a sideways S,
			' and it could have 1-3 roots. If critCount==0 it is monotonic, and
			' if critCount==1 it is monotonic with a single point where it is
			' flat. In the last 2 cases there can only be 1 root. So in cases
			' where num > 1 but critCount < 2, we eliminate all roots in res
			' except one.

			If num = 3 Then
				Dim xe As Double = getRootUpperBound(eqn)
				Dim x0 As Double = -xe

				java.util.Arrays.sort(res, 0, num)
				If critCount = 2 Then
					' this just tries to improve the accuracy of the computed
					' roots using Newton's method.
					res(0) = refineRootWithHint(eqn, x0, intervals(0), res(0))
					res(1) = refineRootWithHint(eqn, intervals(0), intervals(1), res(1))
					res(2) = refineRootWithHint(eqn, intervals(1), xe, res(2))
					Return 3
				ElseIf critCount = 1 Then
					' we only need fx0 and fxe for the sign of the polynomial
					' at -inf and +inf respectively, so we don't need to do
					' fx0 = solveEqn(eqn, 3, x0); fxe = solveEqn(eqn, 3, xe)
					Dim fxe As Double = eqn(3)
					Dim fx0 As Double = -fxe

					Dim x1_Renamed As Double = intervals(0)
					Dim fx1 As Double = solveEqn(eqn, 3, x1_Renamed)

					' if critCount == 1 or critCount == 0, but num == 3 then
					' something has gone wrong. This branch and the one below
					' would ideally never execute, but if they do we can't know
					' which of the computed roots is closest to the real root;
					' therefore, we can't use refineRootWithHint. But even if
					' we did know, being here most likely means that the
					' curve is very flat close to two of the computed roots
					' (or maybe even all three). This might make Newton's method
					' fail altogether, which would be a pain to detect and fix.
					' This is why we use a very stable bisection method.
					If oppositeSigns(fx0, fx1) Then
						res(0) = bisectRootWithHint(eqn, x0, x1_Renamed, res(0))
					ElseIf oppositeSigns(fx1, fxe) Then
						res(0) = bisectRootWithHint(eqn, x1_Renamed, xe, res(2)) ' fx1 must be 0
					Else
						res(0) = x1_Renamed
					End If
					' return 1
				ElseIf critCount = 0 Then
					res(0) = bisectRootWithHint(eqn, x0, xe, res(1))
					' return 1
				End If
			ElseIf num = 2 AndAlso critCount = 2 Then
				' XXX: here we assume that res[0] has better accuracy than res[1].
				' This is true because this method is only used from solveCubic
				' which puts in res[0] the root that it would compute anyway even
				' if num==1. If this method is ever used from any other method, or
				' if the solveCubic implementation changes, this assumption should
				' be reevaluated, and the choice of goodRoot might have to become
				' goodRoot = (abs(eqn'(res[0])) > abs(eqn'(res[1]))) ? res[0] : res[1]
				' where eqn' is the derivative of eqn.
				Dim goodRoot As Double = res(0)
				Dim badRoot As Double = res(1)
				Dim x1_Renamed As Double = intervals(0)
				Dim x2_Renamed As Double = intervals(1)
				' If a cubic curve really has 2 roots, one of those roots must be
				' at a critical point. That can't be goodRoot, so we compute x to
				' be the farthest critical point from goodRoot. If there are two
				' roots, x must be the second one, so we evaluate eqn at x, and if
				' it is zero (or close enough) we put x in res[1] (or badRoot, if
				' |solveEqn(eqn, 3, badRoot)| < |solveEqn(eqn, 3, x)| but this
				' shouldn't happen often).
				Dim x As Double = If(abs(x1_Renamed - goodRoot) > abs(x2_Renamed - goodRoot), x1_Renamed, x2_Renamed)
				Dim fx As Double = solveEqn(eqn, 3, x)

				If iszero(fx, 10000000*ulp(x)) Then
					Dim badRootVal As Double = solveEqn(eqn, 3, badRoot)
					res(1) = If(abs(badRootVal) < abs(fx), badRoot, x)
					Return 2
				End If
			End If ' else there can only be one root - goodRoot, and it is already in res[0]

			Return 1

		' use newton's method.
		private static Double refineRootWithHint(Double() eqn, Double min, Double max, Double t)
			If Not inInterval(t, min, max) Then Return t
			Dim deriv As Double() = {eqn(1), 2*eqn(2), 3*eqn(3)}
			Dim origt As Double = t
			For i As Integer = 0 To 2
				Dim slope As Double = solveEqn(deriv, 2, t)
				Dim y As Double = solveEqn(eqn, 3, t)
				Dim delta As Double = - (y / slope)
				Dim newt As Double = t + delta

				If slope = 0 OrElse y = 0 OrElse t = newt Then Exit For

				t = newt
			Next i
			If within(t, origt, 1000*ulp(origt)) AndAlso inInterval(t, min, max) Then Return t
			Return origt

		private static Double bisectRootWithHint(Double() eqn, Double x0, Double xe, Double hint)
			Dim delta1 As Double = System.Math.Min(abs(hint - x0) / 64, 0.0625)
			Dim delta2 As Double = System.Math.Min(abs(hint - xe) / 64, 0.0625)
			Dim x02 As Double = hint - delta1
			Dim xe2 As Double = hint + delta2
			Dim fx02 As Double = solveEqn(eqn, 3, x02)
			Dim fxe2 As Double = solveEqn(eqn, 3, xe2)
			Do While oppositeSigns(fx02, fxe2)
				If x02 >= xe2 Then Return x02
				x0 = x02
				xe = xe2
				delta1 /= 64
				delta2 /= 64
				x02 = hint - delta1
				xe2 = hint + delta2
				fx02 = solveEqn(eqn, 3, x02)
				fxe2 = solveEqn(eqn, 3, xe2)
			Loop
			If fx02 = 0 Then Return x02
			If fxe2 = 0 Then Return xe2

			Return bisectRoot(eqn, x0, xe)

		private static Double bisectRoot(Double() eqn, Double x0, Double xe)
			Dim fx0 As Double = solveEqn(eqn, 3, x0)
			Dim m As Double = x0 + (xe - x0) / 2
			Do While m <> x0 AndAlso m <> xe
				Dim fm As Double = solveEqn(eqn, 3, m)
				If fm = 0 Then Return m
				If oppositeSigns(fx0, fm) Then
					xe = m
				Else
					fx0 = fm
					x0 = m
				End If
				m = x0 + (xe-x0)/2
			Loop
			Return m

		private static Boolean inInterval(Double t, Double min, Double max)
			Return min <= t AndAlso t <= max

		private static Boolean within(Double x, Double y, Double err)
			Dim d As Double = y - x
			Return (d <= err AndAlso d >= -err)

		private static Boolean iszero(Double x, Double err)
			Return within(x, 0, err)

		private static Boolean oppositeSigns(Double x1, Double x2)
			Return (x1 < 0 AndAlso x2 > 0) OrElse (x1 > 0 AndAlso x2 < 0)

		private static Double solveEqn(Double eqn() , Integer order, Double t)
			Dim v As Double = eqn(order)
			order -= 1
			Do While order >= 0
				v = v * t + eqn(order)
				order -= 1
			Loop
			Return v

	'    
	'     * Computes M+1 where M is an upper bound for all the roots in of eqn.
	'     * See: http://en.wikipedia.org/wiki/Sturm%27s_theorem#Applications.
	'     * The above link doesn't contain a proof, but I [dlila] proved it myself
	'     * so the result is reliable. The proof isn't difficult, but it's a bit
	'     * long to include here.
	'     * Precondition: eqn must represent a cubic polynomial
	'     
		private static Double getRootUpperBound(Double() eqn)
			Dim d As Double = eqn(3)
			Dim a As Double = eqn(2)
			Dim b As Double = eqn(1)
			Dim c As Double = eqn(0)

			Dim M As Double = 1 + max(max(abs(a), abs(b)), abs(c)) / abs(d)
			M += ulp(M) + 1
			Return M


		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Double x, Double y)
			If Not(x * 0.0 + y * 0.0 = 0.0) Then Return False
			' We count the "Y" crossings to determine if the point is
			' inside the curve bounded by its closing line.
			Dim x1_Renamed As Double = x1
			Dim y1_Renamed As Double = y1
			Dim x2_Renamed As Double = x2
			Dim y2_Renamed As Double = y2
			Dim crossings As Integer = (sun.awt.geom.Curve.pointCrossingsForLine(x, y, x1_Renamed, y1_Renamed, x2_Renamed, y2_Renamed) + sun.awt.geom.Curve.pointCrossingsForCubic(x, y, x1_Renamed, y1_Renamed, ctrlX1, ctrlY1, ctrlX2, ctrlY2, x2_Renamed, y2_Renamed, 0))
			Return ((crossings And 1) = 1)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean contains(Point2D p)
			Return contains(p.x, p.y)

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		public Boolean intersects(Double x, Double y, Double w, Double h)
			' Trivially reject non-existant rectangles
			If w <= 0 OrElse h <= 0 Then Return False

			Dim numCrossings As Integer = rectCrossings(x, y, w, h)
			' the intended return value is
			' numCrossings != 0 || numCrossings == Curve.RECT_INTERSECTS
			' but if (numCrossings != 0) numCrossings == INTERSECTS won't matter
			' and if !(numCrossings != 0) then numCrossings == 0, so
			' numCrossings != RECT_INTERSECT
			Return numCrossings <> 0

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

			Dim numCrossings As Integer = rectCrossings(x, y, w, h)
			Return Not(numCrossings = 0 OrElse numCrossings = sun.awt.geom.Curve.RECT_INTERSECTS)

		private Integer rectCrossings(Double x, Double y, Double w, Double h)
			Dim crossings As Integer = 0
			If Not(x1 = x2 AndAlso y1 = y2) Then
				crossings = sun.awt.geom.Curve.rectCrossingsForLine(crossings, x, y, x+w, y+h, x1, y1, x2, y2)
				If crossings = sun.awt.geom.Curve.RECT_INTERSECTS Then Return crossings
			End If
			' we call this with the curve's direction reversed, because we wanted
			' to call rectCrossingsForLine first, because it's cheaper.
			Return sun.awt.geom.Curve.rectCrossingsForCubic(crossings, x, y, x+w, y+h, x2, y2, ctrlX2, ctrlY2, ctrlX1, ctrlY1, x1, y1, 0)

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
		''' shape.
		''' The iterator for this class is not multi-threaded safe,
		''' which means that this <code>CubicCurve2D</code> class does not
		''' guarantee that modifications to the geometry of this
		''' <code>CubicCurve2D</code> object do not affect any iterations of
		''' that geometry that are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		''' coordinates as they are returned in the iteration, or <code>null</code>
		''' if untransformed coordinates are desired </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this <code>CubicCurve2D</code>, one
		'''          segment at a time.
		''' @since 1.2 </returns>
		public PathIterator getPathIterator(AffineTransform at)
			Return New CubicIterator(Me, at)

		''' <summary>
		''' Return an iteration object that defines the boundary of the
		''' flattened shape.
		''' The iterator for this class is not multi-threaded safe,
		''' which means that this <code>CubicCurve2D</code> class does not
		''' guarantee that modifications to the geometry of this
		''' <code>CubicCurve2D</code> object do not affect any iterations of
		''' that geometry that are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to the
		''' coordinates as they are returned in the iteration, or <code>null</code>
		''' if untransformed coordinates are desired </param>
		''' <param name="flatness"> the maximum amount that the control points
		''' for a given curve can vary from colinear before a subdivided
		''' curve is replaced by a straight line connecting the end points </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		''' geometry of the outline of this <code>CubicCurve2D</code>,
		''' one segment at a time.
		''' @since 1.2 </returns>
		public PathIterator getPathIterator(AffineTransform at, Double flatness)
			Return New FlatteningPathIterator(getPathIterator(at), flatness)

		''' <summary>
		''' Creates a new object of the same class as this object.
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