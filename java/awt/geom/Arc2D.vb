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
	''' <CODE>Arc2D</CODE> is the abstract superclass for all objects that
	''' store a 2D arc defined by a framing rectangle,
	''' start angle, angular extent (length of the arc), and a closure type
	''' (<CODE>OPEN</CODE>, <CODE>CHORD</CODE>, or <CODE>PIE</CODE>).
	''' <p>
	''' <a name="inscribes">
	''' The arc is a partial section of a full ellipse which
	''' inscribes the framing rectangle of its parent <seealso cref="RectangularShape"/>.
	''' </a>
	''' <a name="angles">
	''' The angles are specified relative to the non-square
	''' framing rectangle such that 45 degrees always falls on the line from
	''' the center of the ellipse to the upper right corner of the framing
	''' rectangle.
	''' As a result, if the framing rectangle is noticeably longer along one
	''' axis than the other, the angles to the start and end of the arc segment
	''' will be skewed farther along the longer axis of the frame.
	''' </a>
	''' <p>
	''' The actual storage representation of the coordinates is left to
	''' the subclass.
	''' 
	''' @author      Jim Graham
	''' @since 1.2
	''' </summary>
	Public MustInherit Class Arc2D
		Inherits RectangularShape

		''' <summary>
		''' The closure type for an open arc with no path segments
		''' connecting the two ends of the arc segment.
		''' @since 1.2
		''' </summary>
		Public Const OPEN As Integer = 0

		''' <summary>
		''' The closure type for an arc closed by drawing a straight
		''' line segment from the start of the arc segment to the end of the
		''' arc segment.
		''' @since 1.2
		''' </summary>
		Public Const CHORD As Integer = 1

		''' <summary>
		''' The closure type for an arc closed by drawing straight line
		''' segments from the start of the arc segment to the center
		''' of the full ellipse and from that point to the end of the arc segment.
		''' @since 1.2
		''' </summary>
		Public Const PIE As Integer = 2

		''' <summary>
		''' This class defines an arc specified in {@code float} precision.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class Float
			Inherits Arc2D

			''' <summary>
			''' The X coordinate of the upper-left corner of the framing
			''' rectangle of the arc.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public x As Single

			''' <summary>
			''' The Y coordinate of the upper-left corner of the framing
			''' rectangle of the arc.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public y As Single

			''' <summary>
			''' The overall width of the full ellipse of which this arc is
			''' a partial section (not considering the
			''' angular extents).
			''' @since 1.2
			''' @serial
			''' </summary>
			Public width As Single

			''' <summary>
			''' The overall height of the full ellipse of which this arc is
			''' a partial section (not considering the
			''' angular extents).
			''' @since 1.2
			''' @serial
			''' </summary>
			Public height As Single

			''' <summary>
			''' The starting angle of the arc in degrees.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public start As Single

			''' <summary>
			''' The angular extent of the arc in degrees.
			''' @since 1.2
			''' @serial
			''' </summary>
			Public extent As Single

			''' <summary>
			''' Constructs a new OPEN arc, initialized to location (0, 0),
			''' size (0, 0), angular extents (start = 0, extent = 0).
			''' @since 1.2
			''' </summary>
			Public Sub New()
				MyBase.New(OPEN)
			End Sub

			''' <summary>
			''' Constructs a new arc, initialized to location (0, 0),
			''' size (0, 0), angular extents (start = 0, extent = 0), and
			''' the specified closure type.
			''' </summary>
			''' <param name="type"> The closure type for the arc:
			''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			''' @since 1.2 </param>
			Public Sub New(ByVal type As Integer)
				MyBase.New(type)
			End Sub

			''' <summary>
			''' Constructs a new arc, initialized to the specified location,
			''' size, angular extents, and closure type.
			''' </summary>
			''' <param name="x"> The X coordinate of the upper-left corner of
			'''          the arc's framing rectangle. </param>
			''' <param name="y"> The Y coordinate of the upper-left corner of
			'''          the arc's framing rectangle. </param>
			''' <param name="w"> The overall width of the full ellipse of which
			'''          this arc is a partial section. </param>
			''' <param name="h"> The overall height of the full ellipse of which this
			'''          arc is a partial section. </param>
			''' <param name="start"> The starting angle of the arc in degrees. </param>
			''' <param name="extent"> The angular extent of the arc in degrees. </param>
			''' <param name="type"> The closure type for the arc:
			''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			''' @since 1.2 </param>
			Public Sub New(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal start As Single, ByVal extent As Single, ByVal type As Integer)
				MyBase.New(type)
				Me.x = x
				Me.y = y
				Me.width = w
				Me.height = h
				Me.start = start
				Me.extent = extent
			End Sub

			''' <summary>
			''' Constructs a new arc, initialized to the specified location,
			''' size, angular extents, and closure type.
			''' </summary>
			''' <param name="ellipseBounds"> The framing rectangle that defines the
			''' outer boundary of the full ellipse of which this arc is a
			''' partial section. </param>
			''' <param name="start"> The starting angle of the arc in degrees. </param>
			''' <param name="extent"> The angular extent of the arc in degrees. </param>
			''' <param name="type"> The closure type for the arc:
			''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			''' @since 1.2 </param>
			Public Sub New(ByVal ellipseBounds As Rectangle2D, ByVal start As Single, ByVal extent As Single, ByVal type As Integer)
				MyBase.New(type)
				Me.x = CSng(ellipseBounds.x)
				Me.y = CSng(ellipseBounds.y)
				Me.width = CSng(ellipseBounds.width)
				Me.height = CSng(ellipseBounds.height)
				Me.start = start
				Me.extent = extent
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' Note that the arc
			''' <a href="Arc2D.html#inscribes">partially inscribes</a>
			''' the framing rectangle of this {@code RectangularShape}.
			''' 
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  x As Double
				Get
					Return CDbl(x)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' Note that the arc
			''' <a href="Arc2D.html#inscribes">partially inscribes</a>
			''' the framing rectangle of this {@code RectangularShape}.
			''' 
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  y As Double
				Get
					Return CDbl(y)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' Note that the arc
			''' <a href="Arc2D.html#inscribes">partially inscribes</a>
			''' the framing rectangle of this {@code RectangularShape}.
			''' 
			''' @since 1.2
			''' </summary>
			Public  Overrides ReadOnly Property  width As Double
				Get
					Return CDbl(width)
				End Get
			End Property

			''' <summary>
			''' {@inheritDoc}
			''' Note that the arc
			''' <a href="Arc2D.html#inscribes">partially inscribes</a>
			''' the framing rectangle of this {@code RectangularShape}.
			''' 
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
            Public Overrides Property angleStart As Double
                Get
                    Return CDbl(start)
                End Get
                Set(ByVal angSt As Double)
                    Me.start = CSng(angSt)
                End Set
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Property angleExtent As Double
                Get
                    Return CDbl(extent)
                End Get
                Set(ByVal angExt As Double)
                    Me.extent = CSng(angExt)
                End Set
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides ReadOnly Property empty As Boolean
                Get
                    Return (width <= 0.0 OrElse height <= 0.0)
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides Sub setArc(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)
				Me.arcType = closure
				Me.x = CSng(x)
				Me.y = CSng(y)
				Me.width = CSng(w)
				Me.height = CSng(h)
				Me.start = CSng(angSt)
				Me.extent = CSng(angExt)
			End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Protected Friend Overrides Function makeBounds(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Rectangle2D
				Return New Rectangle2D.Float(CSng(x), CSng(y), CSng(w), CSng(h))
			End Function

	'        
	'         * JDK 1.6 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 9130893014586380278L

			''' <summary>
			''' Writes the default serializable fields to the
			''' <code>ObjectOutputStream</code> followed by a byte
			''' indicating the arc type of this <code>Arc2D</code>
			''' instance.
			''' 
			''' @serialData
			''' <ol>
			''' <li>The default serializable fields.
			''' <li>
			''' followed by a <code>byte</code> indicating the arc type
			''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			''' </ol>
			''' </summary>
			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				s.defaultWriteObject()

				s.writeByte(arcType)
			End Sub

			''' <summary>
			''' Reads the default serializable fields from the
			''' <code>ObjectInputStream</code> followed by a byte
			''' indicating the arc type of this <code>Arc2D</code>
			''' instance.
			''' 
			''' @serialData
			''' <ol>
			''' <li>The default serializable fields.
			''' <li>
			''' followed by a <code>byte</code> indicating the arc type
			''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
			''' </ol>
			''' </summary>
			Private Sub readObject(ByVal s As java.io.ObjectInputStream)
				s.defaultReadObject()

				Try
					arcType = s.readByte()
				Catch iae As IllegalArgumentException
					Throw New java.io.InvalidObjectException(iae.Message)
				End Try
			End Sub
		End Class

        ''' <summary>
        ''' This class defines an arc specified in {@code double} precision.
        ''' @since 1.2
        ''' </summary>
        <Serializable>
        Public Class [Double]
            Inherits Arc2D

            ''' <summary>
            ''' The X coordinate of the upper-left corner of the framing
            ''' rectangle of the arc.
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public x As Double

            ''' <summary>
            ''' The Y coordinate of the upper-left corner of the framing
            ''' rectangle of the arc.
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public y As Double

            ''' <summary>
            ''' The overall width of the full ellipse of which this arc is
            ''' a partial section (not considering the angular extents).
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public width As Double

            ''' <summary>
            ''' The overall height of the full ellipse of which this arc is
            ''' a partial section (not considering the angular extents).
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public height As Double

            ''' <summary>
            ''' The starting angle of the arc in degrees.
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public start As Double

            ''' <summary>
            ''' The angular extent of the arc in degrees.
            ''' @since 1.2
            ''' @serial
            ''' </summary>
            Public extent As Double

            ''' <summary>
            ''' Constructs a new OPEN arc, initialized to location (0, 0),
            ''' size (0, 0), angular extents (start = 0, extent = 0).
            ''' @since 1.2
            ''' </summary>
            Sub New()
                MyBase.New(OPEN)
            End Sub

            ''' <summary>
            ''' Constructs a new arc, initialized to location (0, 0),
            ''' size (0, 0), angular extents (start = 0, extent = 0), and
            ''' the specified closure type.
            ''' </summary>
            ''' <param name="type"> The closure type for the arc:
            ''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
            ''' @since 1.2 </param>
            Sub New(ByVal type As Integer)
                MyBase.New(type)
            End Sub

            ''' <summary>
            ''' Constructs a new arc, initialized to the specified location,
            ''' size, angular extents, and closure type.
            ''' </summary>
            ''' <param name="x"> The X coordinate of the upper-left corner
            '''          of the arc's framing rectangle. </param>
            ''' <param name="y"> The Y coordinate of the upper-left corner
            '''          of the arc's framing rectangle. </param>
            ''' <param name="w"> The overall width of the full ellipse of which this
            '''          arc is a partial section. </param>
            ''' <param name="h"> The overall height of the full ellipse of which this
            '''          arc is a partial section. </param>
            ''' <param name="start"> The starting angle of the arc in degrees. </param>
            ''' <param name="extent"> The angular extent of the arc in degrees. </param>
            ''' <param name="type"> The closure type for the arc:
            ''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
            ''' @since 1.2 </param>
            Sub New(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal start As Double, ByVal extent As Double, ByVal type As Integer)
                MyBase.New(type)
                Me.x = x
                Me.y = y
                Me.width = w
                Me.height = h
                Me.start = start
                Me.extent = extent
            End Sub

            ''' <summary>
            ''' Constructs a new arc, initialized to the specified location,
            ''' size, angular extents, and closure type.
            ''' </summary>
            ''' <param name="ellipseBounds"> The framing rectangle that defines the
            ''' outer boundary of the full ellipse of which this arc is a
            ''' partial section. </param>
            ''' <param name="start"> The starting angle of the arc in degrees. </param>
            ''' <param name="extent"> The angular extent of the arc in degrees. </param>
            ''' <param name="type"> The closure type for the arc:
            ''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
            ''' @since 1.2 </param>
            Sub New(ByVal ellipseBounds As Rectangle2D, ByVal start As Double, ByVal extent As Double, ByVal type As Integer)
                MyBase.New(type)
                Me.x = ellipseBounds.x
                Me.y = ellipseBounds.y
                Me.width = ellipseBounds.width
                Me.height = ellipseBounds.height
                Me.start = start
                Me.extent = extent
            End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' Note that the arc
            ''' <a href="Arc2D.html#inscribes">partially inscribes</a>
            ''' the framing rectangle of this {@code RectangularShape}.
            ''' 
            ''' @since 1.2
            ''' </summary>
            Public  Overrides ReadOnly Property  x As Double
                Get
                    Return x
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' Note that the arc
            ''' <a href="Arc2D.html#inscribes">partially inscribes</a>
            ''' the framing rectangle of this {@code RectangularShape}.
            ''' 
            ''' @since 1.2
            ''' </summary>
            Public  Overrides ReadOnly Property  y As Double
                Get
                    Return y
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' Note that the arc
            ''' <a href="Arc2D.html#inscribes">partially inscribes</a>
            ''' the framing rectangle of this {@code RectangularShape}.
            ''' 
            ''' @since 1.2
            ''' </summary>
            Public  Overrides ReadOnly Property  width As Double
                Get
                    Return width
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' Note that the arc
            ''' <a href="Arc2D.html#inscribes">partially inscribes</a>
            ''' the framing rectangle of this {@code RectangularShape}.
            ''' 
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
            'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overrides Function getAngleStart() As Double 'JavaToDotNetTempPropertyGetangleStart
            Public  Overrides ReadOnly Property  angleStart As Double
                Get
                    Return start
                End Get
                Set(ByVal angSt As Double)
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overrides Function getAngleExtent() As Double 'JavaToDotNetTempPropertyGetangleExtent
            Public  Overrides ReadOnly Property  angleExtent As Double
                Get
                    Return extent
                End Get
                Set(ByVal angExt As Double)
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public  Overrides ReadOnly Property  empty As Boolean
                Get
                    Return (width <= 0.0 OrElse height <= 0.0)
                End Get
            End Property

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Public Overrides Sub setArc(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)
                Me.arcType = closure
                Me.x = x
                Me.y = y
                Me.width = w
                Me.height = h
                Me.start = angSt
                Me.extent = angExt
            End Sub

            Me.start = angSt
            End Sub

            Me.extent = angExt
            End Sub

            ''' <summary>
            ''' {@inheritDoc}
            ''' @since 1.2
            ''' </summary>
            Protected Friend Overrides Function makeBounds(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Rectangle2D
                Return New Rectangle2D.Double(x, y, w, h)
            End Function

            '        
            '         * JDK 1.6 serialVersionUID
            '         
            Private Const serialVersionUID As Long = 728264085846882001L

            ''' <summary>
            ''' Writes the default serializable fields to the
            ''' <code>ObjectOutputStream</code> followed by a byte
            ''' indicating the arc type of this <code>Arc2D</code>
            ''' instance.
            ''' 
            ''' @serialData
            ''' <ol>
            ''' <li>The default serializable fields.
            ''' <li>
            ''' followed by a <code>byte</code> indicating the arc type
            ''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
            ''' </ol>
            ''' </summary>
            Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
                s.defaultWriteObject()

                s.writeByte(arcType)
            End Sub

            ''' <summary>
            ''' Reads the default serializable fields from the
            ''' <code>ObjectInputStream</code> followed by a byte
            ''' indicating the arc type of this <code>Arc2D</code>
            ''' instance.
            ''' 
            ''' @serialData
            ''' <ol>
            ''' <li>The default serializable fields.
            ''' <li>
            ''' followed by a <code>byte</code> indicating the arc type
            ''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
            ''' </ol>
            ''' </summary>
            Private Sub readObject(ByVal s As java.io.ObjectInputStream)
                s.defaultReadObject()

                Try
                    arcType = s.readByte()
                Catch iae As IllegalArgumentException
                    Throw New java.io.InvalidObjectException(iae.message)
                End Try
            End Sub
        End Class

        Private type As Integer

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' <p>
		''' This constructor creates an object with a default closure
		''' type of <seealso cref="#OPEN"/>.  It is provided only to enable
		''' serialization of subclasses.
		''' </summary>
		''' <seealso cref= java.awt.geom.Arc2D.Float </seealso>
		''' <seealso cref= java.awt.geom.Arc2D.Double </seealso>
		Protected Friend Sub New()
			Me.New(OPEN)
		End Sub

		''' <summary>
		''' This is an abstract class that cannot be instantiated directly.
		''' Type-specific implementation subclasses are available for
		''' instantiation and provide a number of formats for storing
		''' the information necessary to satisfy the various accessor
		''' methods below.
		''' </summary>
		''' <param name="type"> The closure type of this arc:
		''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>. </param>
		''' <seealso cref= java.awt.geom.Arc2D.Float </seealso>
		''' <seealso cref= java.awt.geom.Arc2D.Double
		''' @since 1.2 </seealso>
		Protected Friend Sub New(ByVal type As Integer)
			arcType = type
		End Sub

		''' <summary>
		''' Returns the starting angle of the arc.
		''' </summary>
		''' <returns> A double value that represents the starting angle
		''' of the arc in degrees. </returns>
		''' <seealso cref= #setAngleStart
		''' @since 1.2 </seealso>
		Public MustOverride Property angleStart As Double

		''' <summary>
		''' Returns the angular extent of the arc.
		''' </summary>
		''' <returns> A double value that represents the angular extent
		''' of the arc in degrees. </returns>
		''' <seealso cref= #setAngleExtent
		''' @since 1.2 </seealso>
		Public MustOverride Property angleExtent As Double

		''' <summary>
		''' Returns the arc closure type of the arc: <seealso cref="#OPEN"/>,
		''' <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>. </summary>
		''' <returns> One of the integer constant closure types defined
		''' in this class. </returns>
		''' <seealso cref= #setArcType
		''' @since 1.2 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getArcType() As Integer 'JavaToDotNetTempPropertyGetarcType
		Public Overridable Property arcType As Integer
			Get
				Return type
			End Get
			Set(ByVal type As Integer)
		End Property

		''' <summary>
		''' Returns the starting point of the arc.  This point is the
		''' intersection of the ray from the center defined by the
		''' starting angle and the elliptical boundary of the arc.
		''' </summary>
		''' <returns> A <CODE>Point2D</CODE> object representing the
		''' x,y coordinates of the starting point of the arc.
		''' @since 1.2 </returns>
		Public Overridable Property startPoint As Point2D
			Get
				Dim angle As Double = System.Math.toRadians(-angleStart)
				Dim x_Renamed As Double = x +  (System.Math.Cos(angle) * 0.5 + 0.5) * width
				Dim y_Renamed As Double = y +  (System.Math.Sin(angle) * 0.5 + 0.5) * height
				Return New Point2D.Double(x_Renamed, y_Renamed)
			End Get
		End Property

		''' <summary>
		''' Returns the ending point of the arc.  This point is the
		''' intersection of the ray from the center defined by the
		''' starting angle plus the angular extent of the arc and the
		''' elliptical boundary of the arc.
		''' </summary>
		''' <returns> A <CODE>Point2D</CODE> object representing the
		''' x,y coordinates  of the ending point of the arc.
		''' @since 1.2 </returns>
		Public Overridable Property endPoint As Point2D
			Get
				Dim angle As Double = System.Math.toRadians(-angleStart - angleExtent)
				Dim x_Renamed As Double = x +  (System.Math.Cos(angle) * 0.5 + 0.5) * width
				Dim y_Renamed As Double = y +  (System.Math.Sin(angle) * 0.5 + 0.5) * height
				Return New Point2D.Double(x_Renamed, y_Renamed)
			End Get
		End Property

		''' <summary>
		''' Sets the location, size, angular extents, and closure type of
		''' this arc to the specified double values.
		''' </summary>
		''' <param name="x"> The X coordinate of the upper-left corner of the arc. </param>
		''' <param name="y"> The Y coordinate of the upper-left corner of the arc. </param>
		''' <param name="w"> The overall width of the full ellipse of which
		'''          this arc is a partial section. </param>
		''' <param name="h"> The overall height of the full ellipse of which
		'''          this arc is a partial section. </param>
		''' <param name="angSt"> The starting angle of the arc in degrees. </param>
		''' <param name="angExt"> The angular extent of the arc in degrees. </param>
		''' <param name="closure"> The closure type for the arc:
		''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		''' @since 1.2 </param>
		Public MustOverride Sub setArc(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)

		''' <summary>
		''' Sets the location, size, angular extents, and closure type of
		''' this arc to the specified values.
		''' </summary>
		''' <param name="loc"> The <CODE>Point2D</CODE> representing the coordinates of
		''' the upper-left corner of the arc. </param>
		''' <param name="size"> The <CODE>Dimension2D</CODE> representing the width
		''' and height of the full ellipse of which this arc is
		''' a partial section. </param>
		''' <param name="angSt"> The starting angle of the arc in degrees. </param>
		''' <param name="angExt"> The angular extent of the arc in degrees. </param>
		''' <param name="closure"> The closure type for the arc:
		''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		''' @since 1.2 </param>
		Public Overridable Sub setArc(ByVal loc As Point2D, ByVal size As Dimension2D, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)
			arcArc(loc.x, loc.y, size.width, size.height, angSt, angExt, closure)
		End Sub

		''' <summary>
		''' Sets the location, size, angular extents, and closure type of
		''' this arc to the specified values.
		''' </summary>
		''' <param name="rect"> The framing rectangle that defines the
		''' outer boundary of the full ellipse of which this arc is a
		''' partial section. </param>
		''' <param name="angSt"> The starting angle of the arc in degrees. </param>
		''' <param name="angExt"> The angular extent of the arc in degrees. </param>
		''' <param name="closure"> The closure type for the arc:
		''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		''' @since 1.2 </param>
		Public Overridable Sub setArc(ByVal rect As Rectangle2D, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)
			arcArc(rect.x, rect.y, rect.width, rect.height, angSt, angExt, closure)
		End Sub

		''' <summary>
		''' Sets this arc to be the same as the specified arc.
		''' </summary>
		''' <param name="a"> The <CODE>Arc2D</CODE> to use to set the arc's values.
		''' @since 1.2 </param>
		Public Overridable Property arc As Arc2D
			Set(ByVal a As Arc2D)
				arcArc(a.x, a.y, a.width, a.height, a.angleStart, a.angleExtent, a.type)
			End Set
		End Property

		''' <summary>
		''' Sets the position, bounds, angular extents, and closure type of
		''' this arc to the specified values. The arc is defined by a center
		''' point and a radius rather than a framing rectangle for the full ellipse.
		''' </summary>
		''' <param name="x"> The X coordinate of the center of the arc. </param>
		''' <param name="y"> The Y coordinate of the center of the arc. </param>
		''' <param name="radius"> The radius of the arc. </param>
		''' <param name="angSt"> The starting angle of the arc in degrees. </param>
		''' <param name="angExt"> The angular extent of the arc in degrees. </param>
		''' <param name="closure"> The closure type for the arc:
		''' <seealso cref="#OPEN"/>, <seealso cref="#CHORD"/>, or <seealso cref="#PIE"/>.
		''' @since 1.2 </param>
		Public Overridable Sub setArcByCenter(ByVal x As Double, ByVal y As Double, ByVal radius As Double, ByVal angSt As Double, ByVal angExt As Double, ByVal closure As Integer)
			arcArc(x - radius, y - radius, radius * 2.0, radius * 2.0, angSt, angExt, closure)
		End Sub

		''' <summary>
		''' Sets the position, bounds, and angular extents of this arc to the
		''' specified value. The starting angle of the arc is tangent to the
		''' line specified by points (p1, p2), the ending angle is tangent to
		''' the line specified by points (p2, p3), and the arc has the
		''' specified radius.
		''' </summary>
		''' <param name="p1"> The first point that defines the arc. The starting
		''' angle of the arc is tangent to the line specified by points (p1, p2). </param>
		''' <param name="p2"> The second point that defines the arc. The starting
		''' angle of the arc is tangent to the line specified by points (p1, p2).
		''' The ending angle of the arc is tangent to the line specified by
		''' points (p2, p3). </param>
		''' <param name="p3"> The third point that defines the arc. The ending angle
		''' of the arc is tangent to the line specified by points (p2, p3). </param>
		''' <param name="radius"> The radius of the arc.
		''' @since 1.2 </param>
		Public Overridable Sub setArcByTangent(ByVal p1 As Point2D, ByVal p2 As Point2D, ByVal p3 As Point2D, ByVal radius As Double)
			Dim ang1 As Double = System.Math.Atan2(p1.y - p2.y, p1.x - p2.x)
			Dim ang2 As Double = System.Math.Atan2(p3.y - p2.y, p3.x - p2.x)
			Dim diff As Double = ang2 - ang1
			If diff > System.Math.PI Then
				ang2 -= System.Math.PI * 2.0
			ElseIf diff < -Math.PI Then
				ang2 += System.Math.PI * 2.0
			End If
			Dim bisect As Double = (ang1 + ang2) / 2.0
			Dim theta As Double = System.Math.Abs(ang2 - bisect)
			Dim dist As Double = radius / System.Math.Sin(theta)
			Dim x_Renamed As Double = p2.x + dist * System.Math.Cos(bisect)
			Dim y_Renamed As Double = p2.y + dist * System.Math.Sin(bisect)
			' REMIND: This needs some work...
			If ang1 < ang2 Then
				ang1 -= System.Math.PI / 2.0
				ang2 += System.Math.PI / 2.0
			Else
				ang1 += System.Math.PI / 2.0
				ang2 -= System.Math.PI / 2.0
			End If
			ang1 = System.Math.toDegrees(-ang1)
			ang2 = System.Math.toDegrees(-ang2)
			diff = ang2 - ang1
			If diff < 0 Then
				diff += 360
			Else
				diff -= 360
			End If
			arcByCenterter(x_Renamed, y_Renamed, radius, ang1, diff, type)
		End Sub



		''' <summary>
		''' Sets the starting angle of this arc to the angle that the
		''' specified point defines relative to the center of this arc.
		''' The angular extent of the arc will remain the same.
		''' </summary>
		''' <param name="p"> The <CODE>Point2D</CODE> that defines the starting angle. </param>
		''' <seealso cref= #getAngleStart
		''' @since 1.2 </seealso>
		Public Overridable Property angleStart As Point2D
			Set(ByVal p As Point2D)
				' Bias the dx and dy by the height and width of the oval.
				Dim dx As Double = height * (p.x - centerX)
				Dim dy As Double = width * (p.y - centerY)
				angleStart = -Math.toDegrees (System.Math.Atan2(dy, dx))
			End Set
		End Property

		''' <summary>
		''' Sets the starting angle and angular extent of this arc using two
		''' sets of coordinates. The first set of coordinates is used to
		''' determine the angle of the starting point relative to the arc's
		''' center. The second set of coordinates is used to determine the
		''' angle of the end point relative to the arc's center.
		''' The arc will always be non-empty and extend counterclockwise
		''' from the first point around to the second point.
		''' </summary>
		''' <param name="x1"> The X coordinate of the arc's starting point. </param>
		''' <param name="y1"> The Y coordinate of the arc's starting point. </param>
		''' <param name="x2"> The X coordinate of the arc's ending point. </param>
		''' <param name="y2"> The Y coordinate of the arc's ending point.
		''' @since 1.2 </param>
		Public Overridable Sub setAngles(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double)
			Dim x_Renamed As Double = centerX
			Dim y_Renamed As Double = centerY
			Dim w As Double = width
			Dim h As Double = height
			' Note: reversing the Y equations negates the angle to adjust
			' for the upside down coordinate system.
			' Also we should bias atans by the height and width of the oval.
			Dim ang1 As Double = System.Math.Atan2(w * (y_Renamed - y1), h * (x1 - x_Renamed))
			Dim ang2 As Double = System.Math.Atan2(w * (y_Renamed - y2), h * (x2 - x_Renamed))
			ang2 -= ang1
			If ang2 <= 0.0 Then ang2 += System.Math.PI * 2.0
			angleStart = System.Math.toDegrees(ang1)
			angleExtent = System.Math.toDegrees(ang2)
		End Sub

		''' <summary>
		''' Sets the starting angle and angular extent of this arc using
		''' two points. The first point is used to determine the angle of
		''' the starting point relative to the arc's center.
		''' The second point is used to determine the angle of the end point
		''' relative to the arc's center.
		''' The arc will always be non-empty and extend counterclockwise
		''' from the first point around to the second point.
		''' </summary>
		''' <param name="p1"> The <CODE>Point2D</CODE> that defines the arc's
		''' starting point. </param>
		''' <param name="p2"> The <CODE>Point2D</CODE> that defines the arc's
		''' ending point.
		''' @since 1.2 </param>
		Public Overridable Sub setAngles(ByVal p1 As Point2D, ByVal p2 As Point2D)
			anglesles(p1.x, p1.y, p2.x, p2.y)
		End Sub

			If type < OPEN OrElse type > PIE Then Throw New IllegalArgumentException("invalid type for Arc: " & type)
			Me.type = type
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' Note that the arc
		''' <a href="Arc2D.html#inscribes">partially inscribes</a>
		''' the framing rectangle of this {@code RectangularShape}.
		''' 
		''' @since 1.2
		''' </summary>
		Public Overrides Sub setFrame(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double)
			arcArc(x, y, w, h, angleStart, angleExtent, type)
		End Sub

		''' <summary>
		''' Returns the high-precision framing rectangle of the arc.  The framing
		''' rectangle contains only the part of this <code>Arc2D</code> that is
		''' in between the starting and ending angles and contains the pie
		''' wedge, if this <code>Arc2D</code> has a <code>PIE</code> closure type.
		''' <p>
		''' This method differs from the
		''' <seealso cref="RectangularShape#getBounds() getBounds"/> in that the
		''' <code>getBounds</code> method only returns the bounds of the
		''' enclosing ellipse of this <code>Arc2D</code> without considering
		''' the starting and ending angles of this <code>Arc2D</code>.
		''' </summary>
		''' <returns> the <CODE>Rectangle2D</CODE> that represents the arc's
		''' framing rectangle.
		''' @since 1.2 </returns>
		Public  Overrides ReadOnly Property  bounds2D As Rectangle2D
			Get
				If empty Then Return makeBounds(x, y, width, height)
				Dim x1, y1, x2, y2 As Double
				If arcType = PIE Then
						y2 = 0.0
							x2 = y2
								y1 = x2
								x1 = y1
				Else
						y1 = 1.0
						x1 = y1
						y2 = -1.0
						x2 = y2
				End If
				Dim angle As Double = 0.0
				For i As Integer = 0 To 5
					If i < 4 Then
						' 0-3 are the four quadrants
						angle += 90.0
						If Not containsAngle(angle) Then Continue For
					ElseIf i = 4 Then
						' 4 is start angle
						angle = angleStart
					Else
						' 5 is end angle
						angle += angleExtent
					End If
					Dim rads As Double = System.Math.toRadians(-angle)
					Dim xe As Double = System.Math.Cos(rads)
					Dim ye As Double = System.Math.Sin(rads)
					x1 = System.Math.Min(x1, xe)
					y1 = System.Math.Min(y1, ye)
					x2 = System.Math.Max(x2, xe)
					y2 = System.Math.Max(y2, ye)
				Next i
				Dim w As Double = width
				Dim h As Double = height
				x2 = (x2 - x1) * 0.5 * w
				y2 = (y2 - y1) * 0.5 * h
				x1 = x + (x1 * 0.5 + 0.5) * w
				y1 = y + (y1 * 0.5 + 0.5) * h
				Return makeBounds(x1, y1, x2, y2)
			End Get
		End Property

		''' <summary>
		''' Constructs a <code>Rectangle2D</code> of the appropriate precision
		''' to hold the parameters calculated to be the framing rectangle
		''' of this arc.
		''' </summary>
		''' <param name="x"> The X coordinate of the upper-left corner of the
		''' framing rectangle. </param>
		''' <param name="y"> The Y coordinate of the upper-left corner of the
		''' framing rectangle. </param>
		''' <param name="w"> The width of the framing rectangle. </param>
		''' <param name="h"> The height of the framing rectangle. </param>
		''' <returns> a <code>Rectangle2D</code> that is the framing rectangle
		'''     of this arc.
		''' @since 1.2 </returns>
		Protected Friend MustOverride Function makeBounds(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Rectangle2D

	'    
	'     * Normalizes the specified angle into the range -180 to 180.
	'     
		Friend Shared Function normalizeDegrees(ByVal angle As Double) As Double
			If angle > 180.0 Then
				If angle <= (180.0 + 360.0) Then
					angle = angle - 360.0
				Else
					angle = System.Math.IEEERemainder(angle, 360.0)
					' IEEEremainder can return -180 here for some input values...
					If angle = -180.0 Then angle = 180.0
				End If
			ElseIf angle <= -180.0 Then
				If angle > (-180.0 - 360.0) Then
					angle = angle + 360.0
				Else
					angle = System.Math.IEEERemainder(angle, 360.0)
					' IEEEremainder can return -180 here for some input values...
					If angle = -180.0 Then angle = 180.0
				End If
			End If
			Return angle
		End Function

		''' <summary>
		''' Determines whether or not the specified angle is within the
		''' angular extents of the arc.
		''' </summary>
		''' <param name="angle"> The angle to test.
		''' </param>
		''' <returns> <CODE>true</CODE> if the arc contains the angle,
		''' <CODE>false</CODE> if the arc doesn't contain the angle.
		''' @since 1.2 </returns>
		Public Overridable Function containsAngle(ByVal angle As Double) As Boolean
			Dim angExt As Double = angleExtent
			Dim backwards As Boolean = (angExt < 0.0)
			If backwards Then angExt = -angExt
			If angExt >= 360.0 Then Return True
			angle = normalizeDegrees(angle) - normalizeDegrees(angleStart)
			If backwards Then angle = -angle
			If angle < 0.0 Then angle += 360.0


			Return (angle >= 0.0) AndAlso (angle < angExt)
		End Function

		''' <summary>
		''' Determines whether or not the specified point is inside the boundary
		''' of the arc.
		''' </summary>
		''' <param name="x"> The X coordinate of the point to test. </param>
		''' <param name="y"> The Y coordinate of the point to test.
		''' </param>
		''' <returns> <CODE>true</CODE> if the point lies within the bound of
		''' the arc, <CODE>false</CODE> if the point lies outside of the
		''' arc's bounds.
		''' @since 1.2 </returns>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double) As Boolean
			' Normalize the coordinates compared to the ellipse
			' having a center at 0,0 and a radius of 0.5.
			Dim ellw As Double = width
			If ellw <= 0.0 Then Return False
			Dim normx As Double = (x - x) / ellw - 0.5
			Dim ellh As Double = height
			If ellh <= 0.0 Then Return False
			Dim normy As Double = (y - y) / ellh - 0.5
			Dim distSq As Double = (normx * normx + normy * normy)
			If distSq >= 0.25 Then Return False
			Dim angExt As Double = System.Math.Abs(angleExtent)
			If angExt >= 360.0 Then Return True
			Dim inarc As Boolean = containsAngle(-Math.toDegrees (System.Math.Atan2(normy, normx)))
			If type = PIE Then Return inarc
			' CHORD and OPEN behave the same way
			If inarc Then
				If angExt >= 180.0 Then Return True
				' point must be outside the "pie triangle"
			Else
				If angExt <= 180.0 Then Return False
				' point must be inside the "pie triangle"
			End If
			' The point is inside the pie triangle iff it is on the same
			' side of the line connecting the ends of the arc as the center.
			Dim angle As Double = System.Math.toRadians(-angleStart)
			Dim x1 As Double = System.Math.Cos(angle)
			Dim y1 As Double = System.Math.Sin(angle)
			angle += System.Math.toRadians(-angleExtent)
			Dim x2 As Double = System.Math.Cos(angle)
			Dim y2 As Double = System.Math.Sin(angle)
			Dim inside As Boolean = (Line2D.relativeCCW(x1, y1, x2, y2, 2*normx, 2*normy) * Line2D.relativeCCW(x1, y1, x2, y2, 0, 0) >= 0)
			Return If(inarc, (Not inside), inside)
		End Function

		''' <summary>
		''' Determines whether or not the interior of the arc intersects
		''' the interior of the specified rectangle.
		''' </summary>
		''' <param name="x"> The X coordinate of the rectangle's upper-left corner. </param>
		''' <param name="y"> The Y coordinate of the rectangle's upper-left corner. </param>
		''' <param name="w"> The width of the rectangle. </param>
		''' <param name="h"> The height of the rectangle.
		''' </param>
		''' <returns> <CODE>true</CODE> if the arc intersects the rectangle,
		''' <CODE>false</CODE> if the arc doesn't intersect the rectangle.
		''' @since 1.2 </returns>
		Public Overrides Function intersects(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean

			Dim aw As Double = width
			Dim ah As Double = height

			If w <= 0 OrElse h <= 0 OrElse aw <= 0 OrElse ah <= 0 Then Return False
			Dim ext As Double = angleExtent
			If ext = 0 Then Return False

			Dim ax As Double = x
			Dim ay As Double = y
			Dim axw As Double = ax + aw
			Dim ayh As Double = ay + ah
			Dim xw As Double = x + w
			Dim yh As Double = y + h

			' check bbox
			If x >= axw OrElse y >= ayh OrElse xw <= ax OrElse yh <= ay Then Return False

			' extract necessary data
			Dim axc As Double = centerX
			Dim ayc As Double = centerY
			Dim sp As Point2D = startPoint
			Dim ep As Point2D = endPoint
			Dim sx As Double = sp.x
			Dim sy As Double = sp.y
			Dim ex As Double = ep.x
			Dim ey As Double = ep.y

	'        
	'         * Try to catch rectangles that intersect arc in areas
	'         * outside of rectagle with left top corner coordinates
	'         * (min(center x, start point x, end point x),
	'         *  min(center y, start point y, end point y))
	'         * and rigth bottom corner coordinates
	'         * (max(center x, start point x, end point x),
	'         *  max(center y, start point y, end point y)).
	'         * So we'll check axis segments outside of rectangle above.
	'         
			If ayc >= y AndAlso ayc <= yh Then ' 0 and 180
				If (sx < xw AndAlso ex < xw AndAlso axc < xw AndAlso axw > x AndAlso containsAngle(0)) OrElse (sx > x AndAlso ex > x AndAlso axc > x AndAlso ax < xw AndAlso containsAngle(180)) Then Return True
			End If
			If axc >= x AndAlso axc <= xw Then ' 90 and 270
				If (sy > y AndAlso ey > y AndAlso ayc > y AndAlso ay < yh AndAlso containsAngle(90)) OrElse (sy < yh AndAlso ey < yh AndAlso ayc < yh AndAlso ayh > y AndAlso containsAngle(270)) Then Return True
			End If

	'        
	'         * For PIE we should check intersection with pie slices;
	'         * also we should do the same for arcs with extent is greater
	'         * than 180, because we should cover case of rectangle, which
	'         * situated between center of arc and chord, but does not
	'         * intersect the chord.
	'         
			Dim rect As Rectangle2D = New Rectangle2D.Double(x, y, w, h)
			If type = PIE OrElse System.Math.Abs(ext) > 180 Then
				' for PIE: try to find intersections with pie slices
				If rect.intersectsLine(axc, ayc, sx, sy) OrElse rect.intersectsLine(axc, ayc, ex, ey) Then Return True
			Else
				' for CHORD and OPEN: try to find intersections with chord
				If rect.intersectsLine(sx, sy, ex, ey) Then Return True
			End If

			' finally check the rectangle corners inside the arc
			If contains(x, y) OrElse contains(x + w, y) OrElse contains(x, y + h) OrElse contains(x + w, y + h) Then Return True

			Return False
		End Function

		''' <summary>
		''' Determines whether or not the interior of the arc entirely contains
		''' the specified rectangle.
		''' </summary>
		''' <param name="x"> The X coordinate of the rectangle's upper-left corner. </param>
		''' <param name="y"> The Y coordinate of the rectangle's upper-left corner. </param>
		''' <param name="w"> The width of the rectangle. </param>
		''' <param name="h"> The height of the rectangle.
		''' </param>
		''' <returns> <CODE>true</CODE> if the arc contains the rectangle,
		''' <CODE>false</CODE> if the arc doesn't contain the rectangle.
		''' @since 1.2 </returns>
		Public Overrides Function contains(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As Boolean
			Return contains(x, y, w, h, Nothing)
		End Function

		''' <summary>
		''' Determines whether or not the interior of the arc entirely contains
		''' the specified rectangle.
		''' </summary>
		''' <param name="r"> The <CODE>Rectangle2D</CODE> to test.
		''' </param>
		''' <returns> <CODE>true</CODE> if the arc contains the rectangle,
		''' <CODE>false</CODE> if the arc doesn't contain the rectangle.
		''' @since 1.2 </returns>
		Public Overrides Function contains(ByVal r As Rectangle2D) As Boolean
			Return contains(r.x, r.y, r.width, r.height, r)
		End Function

		Private Function contains(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double, ByVal origrect As Rectangle2D) As Boolean
			If Not(contains(x, y) AndAlso contains(x + w, y) AndAlso contains(x, y + h) AndAlso contains(x + w, y + h)) Then Return False
			' If the shape is convex then we have done all the testing
			' we need.  Only PIE arcs can be concave and then only if
			' the angular extents are greater than 180 degrees.
			If type <> PIE OrElse System.Math.Abs(angleExtent) <= 180.0 Then Return True
			' For a PIE shape we have an additional test for the case where
			' the angular extents are greater than 180 degrees and all four
			' rectangular corners are inside the shape but one of the
			' rectangle edges spans across the "missing wedge" of the arc.
			' We can test for this case by checking if the rectangle intersects
			' either of the pie angle segments.
			If origrect Is Nothing Then origrect = New Rectangle2D.Double(x, y, w, h)
			Dim halfW As Double = width / 2.0
			Dim halfH As Double = height / 2.0
			Dim xc As Double = x + halfW
			Dim yc As Double = y + halfH
			Dim angle As Double = System.Math.toRadians(-angleStart)
			Dim xe As Double = xc + halfW * System.Math.Cos(angle)
			Dim ye As Double = yc + halfH * System.Math.Sin(angle)
			If origrect.intersectsLine(xc, yc, xe, ye) Then Return False
			angle += System.Math.toRadians(-angleExtent)
			xe = xc + halfW * System.Math.Cos(angle)
			ye = yc + halfH * System.Math.Sin(angle)
			Return Not origrect.intersectsLine(xc, yc, xe, ye)
		End Function

		''' <summary>
		''' Returns an iteration object that defines the boundary of the
		''' arc.
		''' This iterator is multithread safe.
		''' <code>Arc2D</code> guarantees that
		''' modifications to the geometry of the arc
		''' do not affect any iterations of that geometry that
		''' are already in process.
		''' </summary>
		''' <param name="at"> an optional <CODE>AffineTransform</CODE> to be applied
		''' to the coordinates as they are returned in the iteration, or null
		''' if the untransformed coordinates are desired.
		''' </param>
		''' <returns> A <CODE>PathIterator</CODE> that defines the arc's boundary.
		''' @since 1.2 </returns>
		Public Overridable Function getPathIterator(ByVal at As AffineTransform) As PathIterator
			Return New ArcIterator(Me, at)
		End Function

		''' <summary>
		''' Returns the hashcode for this <code>Arc2D</code>. </summary>
		''' <returns> the hashcode for this <code>Arc2D</code>.
		''' @since 1.6 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim bits As Long = java.lang.[Double].doubleToLongBits(x)
			bits += java.lang.[Double].doubleToLongBits(y) * 37
			bits += java.lang.[Double].doubleToLongBits(width) * 43
			bits += java.lang.[Double].doubleToLongBits(height) * 47
			bits += java.lang.[Double].doubleToLongBits(angleStart) * 53
			bits += java.lang.[Double].doubleToLongBits(angleExtent) * 59
			bits += arcType * 61
			Return ((CInt(bits)) Xor (CInt(Fix(bits >> 32))))
		End Function

		''' <summary>
		''' Determines whether or not the specified <code>Object</code> is
		''' equal to this <code>Arc2D</code>.  The specified
		''' <code>Object</code> is equal to this <code>Arc2D</code>
		''' if it is an instance of <code>Arc2D</code> and if its
		''' location, size, arc extents and type are the same as this
		''' <code>Arc2D</code>. </summary>
		''' <param name="obj">  an <code>Object</code> to be compared with this
		'''             <code>Arc2D</code>. </param>
		''' <returns>  <code>true</code> if <code>obj</code> is an instance
		'''          of <code>Arc2D</code> and has the same values;
		'''          <code>false</code> otherwise.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If TypeOf obj Is Arc2D Then
				Dim a2d As Arc2D = CType(obj, Arc2D)
				Return ((x = a2d.x) AndAlso (y = a2d.y) AndAlso (width = a2d.width) AndAlso (height = a2d.height) AndAlso (angleStart = a2d.angleStart) AndAlso (angleExtent = a2d.angleExtent) AndAlso (arcType = a2d.arcType))
			End If
			Return False
		End Function
	End Class

End Namespace