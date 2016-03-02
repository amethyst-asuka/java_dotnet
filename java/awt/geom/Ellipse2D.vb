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
	''' The <code>Ellipse2D</code> class describes an ellipse that is defined
	''' by a framing rectangle.
	''' <p>
	''' This class is only the abstract superclass for all objects which
	''' store a 2D ellipse.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class Ellipse2D
		Inherits RectangularShape

		''' <summary>
		''' The <code>Float</code> class defines an ellipse specified
		''' in <code>float</code> precision.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits Ellipse2D

			''' <summary>
			''' The X coordinate of the upper-left corner of the
			''' framing rectangle of this {@code Ellipse2D}.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Single

			''' <summary>
			''' The Y coordinate of the upper-left corner of the
			''' framing rectangle of this {@code Ellipse2D}.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Single

			''' <summary>
			''' The overall width of this <code>Ellipse2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Single

			''' <summary>
			''' The overall height of this <code>Ellipse2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Single

			''' <summary>
			''' Constructs a new <code>Ellipse2D</code>, initialized to
			''' location (0,&nbsp;0) and size (0,&nbsp;0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes an <code>Ellipse2D</code> from the
			''' specified coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the upper-left corner
			'''          of the framing rectangle </param>
			''' <param name="y"> the Y coordinate of the upper-left corner
			'''          of the framing rectangle </param>
			''' <param name="w"> the width of the framing rectangle </param>
			''' <param name="h"> the height of the framing rectangle
			''' @since 1.2 </param>
			Public Sub New(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single)
				frameame(x, y, w, h)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides x As Double
				Get
					Return CDbl(x)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides y As Double
				Get
					Return CDbl(y)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides width As Double
				Get
					Return CDbl(width)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides height As Double
				Get
					Return CDbl(height)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Property Overrides empty As Boolean
				Get
					Return (width <= 0.0 OrElse height <= 0.0)
				End Get
			End Property

			''' <summary>
			''' Sets the location and size of the framing rectangle of this
			''' <code>Shape</code> to the specified rectangular values.
			''' </summary>
			''' <param name="x"> the X coordinate of the upper-left corner of the
			'''              specified rectangular shape </param>
			''' <param name="y"> the Y coordinate of the upper-left corner of the
			'''              specified rectangular shape </param>
			''' <param name="w"> the width of the specified rectangular shape </param>
			''' <param name="h"> the height of the specified rectangular shape
			''' @since 1.2 </param>
			Public Overridable Sub setFrame(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setFrame(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double)
				Me.x = CSng(x)
				Me.y = CSng(y)
				Me.width = CSng(w)
				Me.height = CSng(h)
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Return New Rectangle2D.Float(x, y, width, height)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -6633761252372475977L
		End Class

		''' <summary>
		''' The <code>Double</code> class defines an ellipse specified
		''' in <code>double</code> precision.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits Ellipse2D

			''' <summary>
			''' The X coordinate of the upper-left corner of the
			''' framing rectangle of this {@code Ellipse2D}.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Double

			''' <summary>
			''' The Y coordinate of the upper-left corner of the
			''' framing rectangle of this {@code Ellipse2D}.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Double

			''' <summary>
			''' The overall width of this <code>Ellipse2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Double

			''' <summary>
			''' The overall height of the <code>Ellipse2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Double

            ''' <summary>
            ''' Constructs a new <code>Ellipse2D</code>, initialized to
            ''' location (0,&nbsp;0) and size (0,&nbsp;0).
            ''' @since 1.2
            ''' </summary>
            Sub New()
            End Sub

            ''' <summary>
            ''' Constructs and initializes an <code>Ellipse2D</code> from the
            ''' specified coordinates.
            ''' </summary>
            ''' <param name="x"> the X coordinate of the upper-left corner
            '''        of the framing rectangle </param>
            ''' <param name="y"> the Y coordinate of the upper-left corner
            '''        of the framing rectangle </param>
            ''' <param name="w"> the width of the framing rectangle </param>
            ''' <param name="h"> the height of the framing rectangle
            ''' @since 1.2 </param>
            Sub New(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double)
                frameame(x, y, w, h)
            End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides ReadOnly Property x As Double
                Get
                    Return x
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides ReadOnly Property y As Double
                Get
                    Return y
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides ReadOnly Property width As Double
                Get
                    Return width
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides ReadOnly Property height As Double
                Get
                    Return height
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Property Overrides empty As Boolean
				Get
					Return (width <= 0.0 OrElse height <= 0.0)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overrides Sub setFrame(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' @since 1.2
			''' </summary>
			Public Overridable Property bounds2D As Rectangle2D
				Get
					Return New Rectangle2D.Double(x, y, width, height)
				End Get
			End Property

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 5555464816372320683L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.Ellipse2D.Float </seealso>
		''' <seealso cref= java.awt.geom.Ellipse2D.Double
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double) As Boolean
			' Normalize the coordinates compared to the ellipse
			' having a center at 0,0 and a radius of 0.5.
			Dim ellw As Double = width
			If ellw <= 0.0 Then Return False
			Dim normx As Double = (x - x) / ellw - 0.5
			Dim ellh As Double = height
			If ellh <= 0.0 Then Return False
			Dim normy As Double = (y - y) / ellh - 0.5
			Return (normx * normx + normy * normy) < 0.25
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function intersects(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean
			If w <= 0.0 OrElse h <= 0.0 Then Return False
			' Normalize the rectangular coordinates compared to the ellipse
			' having a center at 0,0 and a radius of 0.5.
			Dim ellw As Double = width
			If ellw <= 0.0 Then Return False
			Dim normx0 As Double = (x - x) / ellw - 0.5
			Dim normx1 As Double = normx0 + w / ellw
			Dim ellh As Double = height
			If ellh <= 0.0 Then Return False
			Dim normy0 As Double = (y - y) / ellh - 0.5
			Dim normy1 As Double = normy0 + h / ellh
			' find nearest x (left edge, right edge, 0.0)
			' find nearest y (top edge, bottom edge, 0.0)
			' if nearest x,y is inside circle of radius 0.5, then intersects
			Dim nearx, neary As Double
			If normx0 > 0.0 Then
				' center to left of X extents
				nearx = normx0
			ElseIf normx1 < 0.0 Then
				' center to right of X extents
				nearx = normx1
			Else
				nearx = 0.0
			End If
			If normy0 > 0.0 Then
				' center above Y extents
				neary = normy0
			ElseIf normy1 < 0.0 Then
				' center below Y extents
				neary = normy1
			Else
				neary = 0.0
			End If
			Return (nearx * nearx + neary * neary) < 0.25
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean
			Return (contains(x, y) AndAlso contains(x + w, y) AndAlso contains(x, y + h) AndAlso contains(x + w, y + h))
		End Function

		''' <summary>
		''' Returns an iteration object that defines the boundary of this
		''' <code>Ellipse2D</code>.
		''' The iterator for this class is multi-threaded safe, which means
		''' that this <code>Ellipse2D</code> class guarantees that
		''' modifications to the geometry of this <code>Ellipse2D</code>
		''' object do not affect any iterations of that geometry that
		''' are already in process. </summary>
		''' <param name="at"> an optional <code>AffineTransform</code> to be applied to
		''' the coordinates as they are returned in the iteration, or
		''' <code>null</code> if untransformed coordinates are desired </param>
		''' <returns>    the <code>PathIterator</code> object that returns the
		'''          geometry of the outline of this <code>Ellipse2D</code>,
		'''          one segment at a time.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(ByVal at As AffineTransform) As PathIterator
			Return New EllipseIterator(Me, at)
		End Function

		''' <summary>
		''' Returns the hashcode for this <code>Ellipse2D</code>. </summary>
		''' <returns> the hashcode for this <code>Ellipse2D</code>.
		''' @since 1.6 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(x)
			bits += java.lang.[Double].doubleToLongBits(y) * 37
			bits += java.lang.[Double].doubleToLongBits(width) * 43
			bits += java.lang.[Double].doubleToLongBits(height) * 47
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Determines whether or not the specified <code>Object</code> is
		''' equal to this <code>Ellipse2D</code>.  The specified
		''' <code>Object</code> is equal to this <code>Ellipse2D</code>
		''' if it is an instance of <code>Ellipse2D</code> and if its
		''' location and size are the same as this <code>Ellipse2D</code>. </summary>
		''' <param name="obj">  an <code>Object</code> to be compared with this
		'''             <code>Ellipse2D</code>. </param>
		''' <returns>  <code>true</code> if <code>obj</code> is an instance
		'''          of <code>Ellipse2D</code> and has the same values;
		'''          <code>false</code> otherwise.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If TypeOf obj Is Ellipse2D Then
				Dim e2R As Ellipse2D = CType(obj, Ellipse2D)
				Return ((x = e2R.x) AndAlso (y = e2R.y) AndAlso (width = e2R.width) AndAlso (height = e2R.height))
			End If
			Return False
		End Function
	End Class

End Namespace