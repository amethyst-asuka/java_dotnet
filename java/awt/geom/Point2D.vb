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
	''' The <code>Point2D</code> class defines a point representing a location
	''' in {@code (x,y)} coordinate space.
	''' <p>
	''' This class is only the abstract superclass for all objects that
	''' store a 2D coordinate.
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class Point2D
		Implements Cloneable

		''' <summary>
		''' The <code>Float</code> class defines a point specified in float
		''' precision.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits Point2D

			''' <summary>
			''' The X coordinate of this <code>Point2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Single

			''' <summary>
			''' The Y coordinate of this <code>Point2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Single

			''' <summary>
			''' Constructs and initializes a <code>Point2D</code> with
			''' coordinates (0,&nbsp;0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Constructs and initializes a <code>Point2D</code> with
			''' the specified coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the newly
			'''          constructed <code>Point2D</code> </param>
			''' <param name="y"> the Y coordinate of the newly
			'''          constructed <code>Point2D</code>
			''' @since 1.2 </param>
			Public Sub New(  x As Single,   y As Single)
				Me.x = x
				Me.y = y
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
			Public Overrides Sub setLocation(  x As Double,   y As Double)
				Me.x = CSng(x)
				Me.y = CSng(y)
			End Sub

			''' <summary>
			''' Sets the location of this <code>Point2D</code> to the
			''' specified <code>float</code> coordinates.
			''' </summary>
			''' <param name="x"> the new X coordinate of this {@code Point2D} </param>
			''' <param name="y"> the new Y coordinate of this {@code Point2D}
			''' @since 1.2 </param>
			Public Overridable Sub setLocation(  x As Single,   y As Single)
				Me.x = x
				Me.y = y
			End Sub

			''' <summary>
			''' Returns a <code>String</code> that represents the value
			''' of this <code>Point2D</code>. </summary>
			''' <returns> a string representation of this <code>Point2D</code>.
			''' @since 1.2 </returns>
			Public Overrides Function ToString() As String
				Return "Point2D.Float[" & x & ", " & y & "]"
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -2870572449815403710L
		End Class

		''' <summary>
		''' The <code>Double</code> class defines a point specified in
		''' <code>double</code> precision.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Double?
			Inherits Point2D

			''' <summary>
			''' The X coordinate of this <code>Point2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Double

			''' <summary>
			''' The Y coordinate of this <code>Point2D</code>.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Double

			''' <summary>
			''' Constructs and initializes a <code>Point2D</code> with
			''' coordinates (0,&nbsp;0).
			''' @since 1.2
			''' </summary>
			Function java.lang.Double() As [Public] Overridable
			End Function

			''' <summary>
			''' Constructs and initializes a <code>Point2D</code> with the
			''' specified coordinates.
			''' </summary>
			''' <param name="x"> the X coordinate of the newly
			'''          constructed <code>Point2D</code> </param>
			''' <param name="y"> the Y coordinate of the newly
			'''          constructed <code>Point2D</code>
			''' @since 1.2 </param>
			Function java.lang.Double(  x As Double,   y As Double) As [Public] Overridable
				Me.x = x
				Me.y = y
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
			Public Overrides Sub setLocation(  x As Double,   y As Double)
				Me.x = x
				Me.y = y
			End Sub

			''' <summary>
			''' Returns a <code>String</code> that represents the value
			''' of this <code>Point2D</code>. </summary>
			''' <returns> a string representation of this <code>Point2D</code>.
			''' @since 1.2 </returns>
			Public Overrides Function ToString() As String
				Return "Point2D.Double[" & x & ", " & y & "]"
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 6150783262733311327L
		End Class

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <seealso cref= java.awt.geom.Point2D.Float </seealso>
		''' <seealso cref= java.awt.geom.Point2D.Double </seealso>
		''' <seealso cref= java.awt.Point
		''' @since 1.2 </seealso>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the X coordinate of this <code>Point2D</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the X coordinate of this <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property x As Double

		''' <summary>
		''' Returns the Y coordinate of this <code>Point2D</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the Y coordinate of this <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public MustOverride ReadOnly Property y As Double

		''' <summary>
		''' Sets the location of this <code>Point2D</code> to the
		''' specified <code>double</code> coordinates.
		''' </summary>
		''' <param name="x"> the new X coordinate of this {@code Point2D} </param>
		''' <param name="y"> the new Y coordinate of this {@code Point2D}
		''' @since 1.2 </param>
		Public MustOverride Sub setLocation(  x As Double,   y As Double)

		''' <summary>
		''' Sets the location of this <code>Point2D</code> to the same
		''' coordinates as the specified <code>Point2D</code> object. </summary>
		''' <param name="p"> the specified <code>Point2D</code> to which to set
		''' this <code>Point2D</code>
		''' @since 1.2 </param>
		Public Overridable Property location As Point2D
			Set(  p As Point2D)
				locationion(p.x, p.y)
			End Set
		End Property

		''' <summary>
		''' Returns the square of the distance between two points.
		''' </summary>
		''' <param name="x1"> the X coordinate of the first specified point </param>
		''' <param name="y1"> the Y coordinate of the first specified point </param>
		''' <param name="x2"> the X coordinate of the second specified point </param>
		''' <param name="y2"> the Y coordinate of the second specified point </param>
		''' <returns> the square of the distance between the two
		''' sets of specified coordinates.
		''' @since 1.2 </returns>
		Public Shared Function distanceSq(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double) As Double
			x1 -= x2
			y1 -= y2
			Return (x1 * x1 + y1 * y1)
		End Function

		''' <summary>
		''' Returns the distance between two points.
		''' </summary>
		''' <param name="x1"> the X coordinate of the first specified point </param>
		''' <param name="y1"> the Y coordinate of the first specified point </param>
		''' <param name="x2"> the X coordinate of the second specified point </param>
		''' <param name="y2"> the Y coordinate of the second specified point </param>
		''' <returns> the distance between the two sets of specified
		''' coordinates.
		''' @since 1.2 </returns>
		Public Shared Function distance(  x1 As Double,   y1 As Double,   x2 As Double,   y2 As Double) As Double
			x1 -= x2
			y1 -= y2
			Return System.Math.Sqrt(x1 * x1 + y1 * y1)
		End Function

		''' <summary>
		''' Returns the square of the distance from this
		''' <code>Point2D</code> to a specified point.
		''' </summary>
		''' <param name="px"> the X coordinate of the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <param name="py"> the Y coordinate of the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <returns> the square of the distance between this
		''' <code>Point2D</code> and the specified point.
		''' @since 1.2 </returns>
		Public Overridable Function distanceSq(  px As Double,   py As Double) As Double
			px -= x
			py -= y
			Return (px * px + py * py)
		End Function

		''' <summary>
		''' Returns the square of the distance from this
		''' <code>Point2D</code> to a specified <code>Point2D</code>.
		''' </summary>
		''' <param name="pt"> the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <returns> the square of the distance between this
		''' <code>Point2D</code> to a specified <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public Overridable Function distanceSq(  pt As Point2D) As Double
			Dim px As Double = pt.x - Me.x
			Dim py As Double = pt.y - Me.y
			Return (px * px + py * py)
		End Function

		''' <summary>
		''' Returns the distance from this <code>Point2D</code> to
		''' a specified point.
		''' </summary>
		''' <param name="px"> the X coordinate of the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <param name="py"> the Y coordinate of the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <returns> the distance between this <code>Point2D</code>
		''' and a specified point.
		''' @since 1.2 </returns>
		Public Overridable Function distance(  px As Double,   py As Double) As Double
			px -= x
			py -= y
			Return System.Math.Sqrt(px * px + py * py)
		End Function

		''' <summary>
		''' Returns the distance from this <code>Point2D</code> to a
		''' specified <code>Point2D</code>.
		''' </summary>
		''' <param name="pt"> the specified point to be measured
		'''           against this <code>Point2D</code> </param>
		''' <returns> the distance between this <code>Point2D</code> and
		''' the specified <code>Point2D</code>.
		''' @since 1.2 </returns>
		Public Overridable Function distance(  pt As Point2D) As Double
			Dim px As Double = pt.x - Me.x
			Dim py As Double = pt.y - Me.y
			Return System.Math.Sqrt(px * px + py * py)
		End Function

		''' <summary>
		''' Creates a new object of the same class and with the
		''' same contents as this object. </summary>
		''' <returns>     a clone of this instance. </returns>
		''' <exception cref="OutOfMemoryError">            if there is not enough memory. </exception>
		''' <seealso cref=        java.lang.Cloneable
		''' @since      1.2 </seealso>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function

		''' <summary>
		''' Returns the hashcode for this <code>Point2D</code>. </summary>
		''' <returns>      a hash code for this <code>Point2D</code>. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(x)
			bits = bits Xor java.lang.[Double].doubleToLongBits(y) * 31
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Determines whether or not two points are equal. Two instances of
		''' <code>Point2D</code> are equal if the values of their
		''' <code>x</code> and <code>y</code> member fields, representing
		''' their position in the coordinate space, are the same. </summary>
		''' <param name="obj"> an object to be compared with this <code>Point2D</code> </param>
		''' <returns> <code>true</code> if the object to be compared is
		'''         an instance of <code>Point2D</code> and has
		'''         the same values; <code>false</code> otherwise.
		''' @since 1.2 </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If TypeOf obj Is Point2D Then
				Dim p2d As Point2D = CType(obj, Point2D)
				Return (x = p2d.x) AndAlso (y = p2d.y)
			End If
			Return MyBase.Equals(obj)
		End Function
	End Class

End Namespace