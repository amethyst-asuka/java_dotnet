Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	''' <summary>
	''' A point representing a location in {@code (x,y)} coordinate space,
	''' specified in integer precision.
	''' 
	''' @author      Sami Shaio
	''' @since       1.0
	''' </summary>
	<Serializable> _
	Public Class Point
		Inherits java.awt.geom.Point2D

		''' <summary>
		''' The X coordinate of this <code>Point</code>.
		''' If no X coordinate is set it will default to 0.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLocation() </seealso>
		''' <seealso cref= #move(int, int)
		''' @since 1.0 </seealso>
		Public x As Integer

		''' <summary>
		''' The Y coordinate of this <code>Point</code>.
		''' If no Y coordinate is set it will default to 0.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLocation() </seealso>
		''' <seealso cref= #move(int, int)
		''' @since 1.0 </seealso>
		Public y As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -5276940640259749850L

		''' <summary>
		''' Constructs and initializes a point at the origin
		''' (0,&nbsp;0) of the coordinate space.
		''' @since       1.1
		''' </summary>
		Public Sub New()
			Me.New(0, 0)
		End Sub

		''' <summary>
		''' Constructs and initializes a point with the same location as
		''' the specified <code>Point</code> object. </summary>
		''' <param name="p"> a point
		''' @since       1.1 </param>
		Public Sub New(ByVal p As Point)
			Me.New(p.x, p.y)
		End Sub

		''' <summary>
		''' Constructs and initializes a point at the specified
		''' {@code (x,y)} location in the coordinate space. </summary>
		''' <param name="x"> the X coordinate of the newly constructed <code>Point</code> </param>
		''' <param name="y"> the Y coordinate of the newly constructed <code>Point</code>
		''' @since 1.0 </param>
		Public Sub New(ByVal x As Integer, ByVal y As Integer)
			Me.x = x
			Me.y = y
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Property Overrides x As Double
			Get
				Return x
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Property Overrides y As Double
			Get
				Return y
			End Get
		End Property

		''' <summary>
		''' Returns the location of this point.
		''' This method is included for completeness, to parallel the
		''' <code>getLocation</code> method of <code>Component</code>. </summary>
		''' <returns>      a copy of this point, at the same location </returns>
		''' <seealso cref=         java.awt.Component#getLocation </seealso>
		''' <seealso cref=         java.awt.Point#setLocation(java.awt.Point) </seealso>
		''' <seealso cref=         java.awt.Point#setLocation(int, int)
		''' @since       1.1 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property location As Point
			Get
				Return New Point(x, y)
			End Get
			Set(ByVal p As Point)
				locationion(p.x, p.y)
			End Set
		End Property


		''' <summary>
		''' Changes the point to have the specified location.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>setLocation</code> method of <code>Component</code>.
		''' Its behavior is identical with <code>move(int,&nbsp;int)</code>. </summary>
		''' <param name="x"> the X coordinate of the new location </param>
		''' <param name="y"> the Y coordinate of the new location </param>
		''' <seealso cref=         java.awt.Component#setLocation(int, int) </seealso>
		''' <seealso cref=         java.awt.Point#getLocation </seealso>
		''' <seealso cref=         java.awt.Point#move(int, int)
		''' @since       1.1 </seealso>
		Public Overridable Sub setLocation(ByVal x As Integer, ByVal y As Integer)
			move(x, y)
		End Sub

		''' <summary>
		''' Sets the location of this point to the specified double coordinates.
		''' The double values will be rounded to integer values.
		''' Any number smaller than <code>Integer.MIN_VALUE</code>
		''' will be reset to <code>MIN_VALUE</code>, and any number
		''' larger than <code> [Integer].MAX_VALUE</code> will be
		''' reset to <code>MAX_VALUE</code>.
		''' </summary>
		''' <param name="x"> the X coordinate of the new location </param>
		''' <param name="y"> the Y coordinate of the new location </param>
		''' <seealso cref= #getLocation </seealso>
		Public Overrides Sub setLocation(ByVal x As Double, ByVal y As Double)
			Me.x = CInt(Fix(Math.Floor(x+0.5)))
			Me.y = CInt(Fix(Math.Floor(y+0.5)))
		End Sub

		''' <summary>
		''' Moves this point to the specified location in the
		''' {@code (x,y)} coordinate plane. This method
		''' is identical with <code>setLocation(int,&nbsp;int)</code>. </summary>
		''' <param name="x"> the X coordinate of the new location </param>
		''' <param name="y"> the Y coordinate of the new location </param>
		''' <seealso cref=         java.awt.Component#setLocation(int, int) </seealso>
		Public Overridable Sub move(ByVal x As Integer, ByVal y As Integer)
			Me.x = x
			Me.y = y
		End Sub

		''' <summary>
		''' Translates this point, at location {@code (x,y)},
		''' by {@code dx} along the {@code x} axis and {@code dy}
		''' along the {@code y} axis so that it now represents the point
		''' {@code (x+dx,y+dy)}.
		''' </summary>
		''' <param name="dx">   the distance to move this point
		'''                            along the X axis </param>
		''' <param name="dy">    the distance to move this point
		'''                            along the Y axis </param>
		Public Overridable Sub translate(ByVal dx As Integer, ByVal dy As Integer)
			Me.x += dx
			Me.y += dy
		End Sub

		''' <summary>
		''' Determines whether or not two points are equal. Two instances of
		''' <code>Point2D</code> are equal if the values of their
		''' <code>x</code> and <code>y</code> member fields, representing
		''' their position in the coordinate space, are the same. </summary>
		''' <param name="obj"> an object to be compared with this <code>Point2D</code> </param>
		''' <returns> <code>true</code> if the object to be compared is
		'''         an instance of <code>Point2D</code> and has
		'''         the same values; <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Point Then
				Dim pt As Point = CType(obj, Point)
				Return (x = pt.x) AndAlso (y = pt.y)
			End If
			Return MyBase.Equals(obj)
		End Function

		''' <summary>
		''' Returns a string representation of this point and its location
		''' in the {@code (x,y)} coordinate space. This method is
		''' intended to be used only for debugging purposes, and the content
		''' and format of the returned string may vary between implementations.
		''' The returned string may be empty but may not be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this point </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[x=" & x & ",y=" & y & "]"
		End Function
	End Class

End Namespace