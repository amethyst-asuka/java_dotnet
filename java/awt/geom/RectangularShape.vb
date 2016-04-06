Imports Microsoft.VisualBasic
Imports System

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
    ''' <code>RectangularShape</code> is the base class for a number of
    ''' <seealso cref="Shape"/> objects whose geometry is defined by a rectangular frame.
    ''' This class does not directly specify any specific geometry by
    ''' itself, but merely provides manipulation methods inherited by
    ''' a whole category of <code>Shape</code> objects.
    ''' The manipulation methods provided by this class can be used to
    ''' query and modify the rectangular frame, which provides a reference
    ''' for the subclasses to define their geometry.
    ''' 
    ''' @author      Jim Graham
    ''' @since 1.2
    ''' </summary>
    Public MustInherit Class RectangularShape : Inherits java.lang.Object
        Implements java.awt.Shape, Cloneable

        Public MustOverride Function getPathIterator(  at As java.awt.geom.AffineTransform,   flatness As Double) As java.awt.geom.PathIterator
        Public MustOverride Function getPathIterator(  at As java.awt.geom.AffineTransform) As java.awt.geom.PathIterator
        Public MustOverride Function contains(  r As java.awt.geom.Rectangle2D) As Boolean
        Public MustOverride Function contains(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
        Public MustOverride Function intersects(  r As java.awt.geom.Rectangle2D) As Boolean
        Public MustOverride Function intersects(  x As Double,   y As Double,   w As Double,   h As Double) As Boolean
        Public MustOverride Function contains(  p As java.awt.geom.Point2D) As Boolean
        Public MustOverride Function contains(  x As Double,   y As Double) As Boolean
        Public MustOverride ReadOnly Property bounds2D As java.awt.geom.Rectangle2D

        ''' <summary>
        ''' This is an abstract class that cannot be instantiated directly.
        ''' </summary>
        ''' <seealso cref= Arc2D </seealso>
        ''' <seealso cref= Ellipse2D </seealso>
        ''' <seealso cref= Rectangle2D </seealso>
        ''' <seealso cref= RoundRectangle2D
        ''' @since 1.2 </seealso>
        Protected Friend Sub New()
        End Sub

        ''' <summary>
        ''' Returns the X coordinate of the upper-left corner of
        ''' the framing rectangle in <code>double</code> precision. </summary>
        ''' <returns> the X coordinate of the upper-left corner of
        ''' the framing rectangle.
        ''' @since 1.2 </returns>
        Public MustOverride ReadOnly Property x As Double

        ''' <summary>
        ''' Returns the Y coordinate of the upper-left corner of
        ''' the framing rectangle in <code>double</code> precision. </summary>
        ''' <returns> the Y coordinate of the upper-left corner of
        ''' the framing rectangle.
        ''' @since 1.2 </returns>
        Public MustOverride ReadOnly Property y As Double

        ''' <summary>
        ''' Returns the width of the framing rectangle in
        ''' <code>double</code> precision. </summary>
        ''' <returns> the width of the framing rectangle.
        ''' @since 1.2 </returns>
        Public MustOverride ReadOnly Property width As Double

        ''' <summary>
        ''' Returns the height of the framing rectangle
        ''' in <code>double</code> precision. </summary>
        ''' <returns> the height of the framing rectangle.
        ''' @since 1.2 </returns>
        Public MustOverride ReadOnly Property height As Double

        ''' <summary>
        ''' Returns the smallest X coordinate of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the smallest X coordinate of the framing
        '''          rectangle of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property minX As Double
            Get
                Return x
            End Get
        End Property

        ''' <summary>
        ''' Returns the smallest Y coordinate of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the smallest Y coordinate of the framing
        '''          rectangle of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property minY As Double
            Get
                Return y
            End Get
        End Property

        ''' <summary>
        ''' Returns the largest X coordinate of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the largest X coordinate of the framing
        '''          rectangle of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property maxX As Double
            Get
                Return x + width
            End Get
        End Property

        ''' <summary>
        ''' Returns the largest Y coordinate of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the largest Y coordinate of the framing
        '''          rectangle of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property maxY As Double
            Get
                Return y + height
            End Get
        End Property

        ''' <summary>
        ''' Returns the X coordinate of the center of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the X coordinate of the center of the framing rectangle
        '''          of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property centerX As Double
            Get
                Return x + width / 2.0
            End Get
        End Property

        ''' <summary>
        ''' Returns the Y coordinate of the center of the framing
        ''' rectangle of the <code>Shape</code> in <code>double</code>
        ''' precision. </summary>
        ''' <returns> the Y coordinate of the center of the framing rectangle
        '''          of the <code>Shape</code>.
        ''' @since 1.2 </returns>
        Public Overridable ReadOnly Property centerY As Double
            Get
                Return y + height / 2.0
            End Get
        End Property

        ''' <summary>
        ''' Returns the framing <seealso cref="Rectangle2D"/>
        ''' that defines the overall shape of this object. </summary>
        ''' <returns> a <code>Rectangle2D</code>, specified in
        ''' <code>double</code> coordinates. </returns>
        ''' <seealso cref= #setFrame(double, double, double, double) </seealso>
        ''' <seealso cref= #setFrame(Point2D, Dimension2D) </seealso>
        ''' <seealso cref= #setFrame(Rectangle2D)
        ''' @since 1.2 </seealso>
        Public Overridable Property frame As Rectangle2D
            Get
                Return New Rectangle2D.Double(x, y, width, height)
            End Get
            Set(  r As Rectangle2D)
                setFrame(r.x, r.y, r.width, r.height)
            End Set
        End Property

        ''' <summary>
        ''' Determines whether the <code>RectangularShape</code> is empty.
        ''' When the <code>RectangularShape</code> is empty, it encloses no
        ''' area. </summary>
        ''' <returns> <code>true</code> if the <code>RectangularShape</code> is empty;
        '''          <code>false</code> otherwise.
        ''' @since 1.2 </returns>
        Public MustOverride ReadOnly Property empty As Boolean

        ''' <summary>
        ''' Sets the location and size of the framing rectangle of this
        ''' <code>Shape</code> to the specified rectangular values.
        ''' </summary>
        ''' <param name="x"> the X coordinate of the upper-left corner of the
        '''          specified rectangular shape </param>
        ''' <param name="y"> the Y coordinate of the upper-left corner of the
        '''          specified rectangular shape </param>
        ''' <param name="w"> the width of the specified rectangular shape </param>
        ''' <param name="h"> the height of the specified rectangular shape </param>
        ''' <seealso cref= #getFrame
        ''' @since 1.2 </seealso>
        Public MustOverride Sub setFrame(  x As Double,   y As Double,   w As Double,   h As Double)

        ''' <summary>
        ''' Sets the location and size of the framing rectangle of this
        ''' <code>Shape</code> to the specified <seealso cref="Point2D"/> and
        ''' <seealso cref="Dimension2D"/>, respectively.  The framing rectangle is used
        ''' by the subclasses of <code>RectangularShape</code> to define
        ''' their geometry. </summary>
        ''' <param name="loc"> the specified <code>Point2D</code> </param>
        ''' <param name="size"> the specified <code>Dimension2D</code> </param>
        ''' <seealso cref= #getFrame
        ''' @since 1.2 </seealso>
        Public Overridable Sub setFrame(  loc As Point2D,   size As Dimension2D)
            frameame(loc.x, loc.y, size.width, size.height)
        End Sub

        ''' <summary>
        ''' Sets the diagonal of the framing rectangle of this <code>Shape</code>
        ''' based on the two specified coordinates.  The framing rectangle is
        ''' used by the subclasses of <code>RectangularShape</code> to define
        ''' their geometry.
        ''' </summary>
        ''' <param name="x1"> the X coordinate of the start point of the specified diagonal </param>
        ''' <param name="y1"> the Y coordinate of the start point of the specified diagonal </param>
        ''' <param name="x2"> the X coordinate of the end point of the specified diagonal </param>
        ''' <param name="y2"> the Y coordinate of the end point of the specified diagonal
        ''' @since 1.2 </param>
        Public Overridable Sub setFrameFromDiagonal(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double)
            If x2 < x1 Then
                Dim t As Double = x1
                x1 = x2
                x2 = t
            End If
            If y2 < y1 Then
                Dim t As Double = y1
                y1 = y2
                y2 = t
            End If
            frameame(x1, y1, x2 - x1, y2 - y1)
        End Sub

        ''' <summary>
        ''' Sets the diagonal of the framing rectangle of this <code>Shape</code>
        ''' based on two specified <code>Point2D</code> objects.  The framing
        ''' rectangle is used by the subclasses of <code>RectangularShape</code>
        ''' to define their geometry.
        ''' </summary>
        ''' <param name="p1"> the start <code>Point2D</code> of the specified diagonal </param>
        ''' <param name="p2"> the end <code>Point2D</code> of the specified diagonal
        ''' @since 1.2 </param>
        Public Overridable Sub setFrameFromDiagonal(  p1 As Point2D,   p2 As Point2D)
            frameFromDiagonalnal(p1.x, p1.y, p2.x, p2.y)
        End Sub

        ''' <summary>
        ''' Sets the framing rectangle of this <code>Shape</code>
        ''' based on the specified center point coordinates and corner point
        ''' coordinates.  The framing rectangle is used by the subclasses of
        ''' <code>RectangularShape</code> to define their geometry.
        ''' </summary>
        ''' <param name="centerX"> the X coordinate of the specified center point </param>
        ''' <param name="centerY"> the Y coordinate of the specified center point </param>
        ''' <param name="cornerX"> the X coordinate of the specified corner point </param>
        ''' <param name="cornerY"> the Y coordinate of the specified corner point
        ''' @since 1.2 </param>
        Public Overridable Sub setFrameFromCenter(  centerX As Double,   centerY As Double,   cornerX As Double,   cornerY As Double)
            Dim halfW As Double = System.Math.Abs(cornerX - centerX)
            Dim halfH As Double = System.Math.Abs(cornerY - centerY)
            frameame(centerX - halfW, centerY - halfH, halfW * 2.0, halfH * 2.0)
        End Sub

        ''' <summary>
        ''' Sets the framing rectangle of this <code>Shape</code> based on a
        ''' specified center <code>Point2D</code> and corner
        ''' <code>Point2D</code>.  The framing rectangle is used by the subclasses
        ''' of <code>RectangularShape</code> to define their geometry. </summary>
        ''' <param name="center"> the specified center <code>Point2D</code> </param>
        ''' <param name="corner"> the specified corner <code>Point2D</code>
        ''' @since 1.2 </param>
        Public Overridable Sub setFrameFromCenter(  center As Point2D,   corner As Point2D)
            frameFromCenterter(center.x, center.y, corner.x, corner.y)
        End Sub

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overridable Function contains(  p As Point2D) As Boolean
            Return contains(p.x, p.y)
        End Function

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overridable Function intersects(  r As Rectangle2D) As Boolean
            Return intersects(r.x, r.y, r.width, r.height)
        End Function

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overridable Function contains(  r As Rectangle2D) As Boolean
            Return contains(r.x, r.y, r.width, r.height)
        End Function

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overridable ReadOnly Property bounds As java.awt.Rectangle
            Get
                Dim width_Renamed As Double = width
                Dim height_Renamed As Double = height
                If width_Renamed < 0 OrElse height_Renamed < 0 Then Return New java.awt.Rectangle
                Dim x_Renamed As Double = x
                Dim y_Renamed As Double = y
                Dim x1 As Double = System.Math.Floor(x_Renamed)
                Dim y1 As Double = System.Math.Floor(y_Renamed)
                Dim x2 As Double = System.Math.Ceiling(x_Renamed + width_Renamed)
                Dim y2 As Double = System.Math.Ceiling(y_Renamed + height_Renamed)
                Return New java.awt.Rectangle(CInt(Fix(x1)), CInt(Fix(y1)), CInt(Fix(x2 - x1)), CInt(Fix(y2 - y1)))
            End Get
        End Property

        ''' <summary>
        ''' Returns an iterator object that iterates along the
        ''' <code>Shape</code> object's boundary and provides access to a
        ''' flattened view of the outline of the <code>Shape</code>
        ''' object's geometry.
        ''' <p>
        ''' Only SEG_MOVETO, SEG_LINETO, and SEG_CLOSE point types will
        ''' be returned by the iterator.
        ''' <p>
        ''' The amount of subdivision of the curved segments is controlled
        ''' by the <code>flatness</code> parameter, which specifies the
        ''' maximum distance that any point on the unflattened transformed
        ''' curve can deviate from the returned flattened path segments.
        ''' An optional <seealso cref="AffineTransform"/> can
        ''' be specified so that the coordinates returned in the iteration are
        ''' transformed accordingly. </summary>
        ''' <param name="at"> an optional <code>AffineTransform</code> to be applied to the
        '''          coordinates as they are returned in the iteration,
        '''          or <code>null</code> if untransformed coordinates are desired. </param>
        ''' <param name="flatness"> the maximum distance that the line segments used to
        '''          approximate the curved segments are allowed to deviate
        '''          from any point on the original curve </param>
        ''' <returns> a <code>PathIterator</code> object that provides access to
        '''          the <code>Shape</code> object's flattened geometry.
        ''' @since 1.2 </returns>
        Public Overridable Function getPathIterator(  at As AffineTransform,   flatness As Double) As PathIterator
            Return New FlatteningPathIterator(getPathIterator(at), flatness)
        End Function

        ''' <summary>
        ''' Creates a new object of the same class and with the same
        ''' contents as this object. </summary>
        ''' <returns>     a clone of this instance. </returns>
        ''' <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
        ''' <seealso cref=        java.lang.Cloneable
        ''' @since      1.2 </seealso>
        Public Overrides Function clone() As Object
            Try
                Return MyBase.clone()
            Catch e As CloneNotSupportedException
                ' this shouldn't happen, since we are Cloneable
                Throw New InternalError(e)
            End Try
        End Function
    End Class

End Namespace