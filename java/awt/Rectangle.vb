Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Rectangle</code> specifies an area in a coordinate space that is
	''' enclosed by the <code>Rectangle</code> object's upper-left point
	''' {@code (x,y)}
	''' in the coordinate space, its width, and its height.
	''' <p>
	''' A <code>Rectangle</code> object's <code>width</code> and
	''' <code>height</code> are <code>public</code> fields. The constructors
	''' that create a <code>Rectangle</code>, and the methods that can modify
	''' one, do not prevent setting a negative value for width or height.
	''' <p>
	''' <a name="Empty">
	''' A {@code Rectangle} whose width or height is exactly zero has location
	''' along those axes with zero dimension, but is otherwise considered empty.
	''' The <seealso cref="#isEmpty"/> method will return true for such a {@code Rectangle}.
	''' Methods which test if an empty {@code Rectangle} contains or intersects
	''' a point or rectangle will always return false if either dimension is zero.
	''' Methods which combine such a {@code Rectangle} with a point or rectangle
	''' will include the location of the {@code Rectangle} on that axis in the
	''' result as if the <seealso cref="#add(Point)"/> method were being called.
	''' </a>
	''' <p>
	''' <a name="NonExistant">
	''' A {@code Rectangle} whose width or height is negative has neither
	''' location nor dimension along those axes with negative dimensions.
	''' Such a {@code Rectangle} is treated as non-existant along those axes.
	''' Such a {@code Rectangle} is also empty with respect to containment
	''' calculations and methods which test if it contains or intersects a
	''' point or rectangle will always return false.
	''' Methods which combine such a {@code Rectangle} with a point or rectangle
	''' will ignore the {@code Rectangle} entirely in generating the result.
	''' If two {@code Rectangle} objects are combined and each has a negative
	''' dimension, the result will have at least one negative dimension.
	''' </a>
	''' <p>
	''' Methods which affect only the location of a {@code Rectangle} will
	''' operate on its location regardless of whether or not it has a negative
	''' or zero dimension along either axis.
	''' <p>
	''' Note that a {@code Rectangle} constructed with the default no-argument
	''' constructor will have dimensions of {@code 0x0} and therefore be empty.
	''' That {@code Rectangle} will still have a location of {@code (0,0)} and
	''' will contribute that location to the union and add operations.
	''' Code attempting to accumulate the bounds of a set of points should
	''' therefore initially construct the {@code Rectangle} with a specifically
	''' negative width and height or it should use the first point in the set
	''' to construct the {@code Rectangle}.
	''' For example:
	''' <pre>{@code
	'''     Rectangle bounds = new Rectangle(0, 0, -1, -1);
	'''     for (int i = 0; i < points.length; i++) {
	'''         bounds.add(points[i]);
	'''     }
	''' }</pre>
	''' or if we know that the points array contains at least one point:
	''' <pre>{@code
	'''     Rectangle bounds = new Rectangle(points[0]);
	'''     for (int i = 1; i < points.length; i++) {
	'''         bounds.add(points[i]);
	'''     }
	''' }</pre>
	''' <p>
	''' This class uses 32-bit integers to store its location and dimensions.
	''' Frequently operations may produce a result that exceeds the range of
	''' a 32-bit integer.
	''' The methods will calculate their results in a way that avoids any
	''' 32-bit overflow for intermediate results and then choose the best
	''' representation to store the final results back into the 32-bit fields
	''' which hold the location and dimensions.
	''' The location of the result will be stored into the <seealso cref="#x"/> and
	''' <seealso cref="#y"/> fields by clipping the true result to the nearest 32-bit value.
	''' The values stored into the <seealso cref="#width"/> and <seealso cref="#height"/> dimension
	''' fields will be chosen as the 32-bit values that encompass the largest
	''' part of the true result as possible.
	''' Generally this means that the dimension will be clipped independently
	''' to the range of 32-bit integers except that if the location had to be
	''' moved to store it into its pair of 32-bit fields then the dimensions
	''' will be adjusted relative to the "best representation" of the location.
	''' If the true result had a negative dimension and was therefore
	''' non-existant along one or both axes, the stored dimensions will be
	''' negative numbers in those axes.
	''' If the true result had a location that could be represented within
	''' the range of 32-bit integers, but zero dimension along one or both
	''' axes, then the stored dimensions will be zero in those axes.
	''' 
	''' @author      Sami Shaio
	''' @since 1.0
	''' </summary>
	<Serializable> _
	Public Class Rectangle
		Inherits java.awt.geom.Rectangle2D
		Implements Shape

		''' <summary>
		''' The X coordinate of the upper-left corner of the <code>Rectangle</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setLocation(int, int) </seealso>
		''' <seealso cref= #getLocation()
		''' @since 1.0 </seealso>
		Public x As Integer

		''' <summary>
		''' The Y coordinate of the upper-left corner of the <code>Rectangle</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setLocation(int, int) </seealso>
		''' <seealso cref= #getLocation()
		''' @since 1.0 </seealso>
		Public y As Integer

		''' <summary>
		''' The width of the <code>Rectangle</code>.
		''' @serial </summary>
		''' <seealso cref= #setSize(int, int) </seealso>
		''' <seealso cref= #getSize()
		''' @since 1.0 </seealso>
		Public width As Integer

		''' <summary>
		''' The height of the <code>Rectangle</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setSize(int, int) </seealso>
		''' <seealso cref= #getSize()
		''' @since 1.0 </seealso>
		Public height As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -4345857070255674764L

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose upper-left corner
		''' is at (0,&nbsp;0) in the coordinate space, and whose width and
		''' height are both zero.
		''' </summary>
		Public Sub New()
			Me.New(0, 0, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code>, initialized to match
		''' the values of the specified <code>Rectangle</code>. </summary>
		''' <param name="r">  the <code>Rectangle</code> from which to copy initial values
		'''           to a newly constructed <code>Rectangle</code>
		''' @since 1.1 </param>
		Public Sub New(ByVal r As Rectangle)
			Me.New(r.x, r.y, r.width, r.height)
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose upper-left corner is
		''' specified as
		''' {@code (x,y)} and whose width and height
		''' are specified by the arguments of the same name. </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <param name="width">    the width of the <code>Rectangle</code> </param>
		''' <param name="height">   the height of the <code>Rectangle</code>
		''' @since 1.0 </param>
		Public Sub New(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Me.x = x
			Me.y = y
			Me.width = width
			Me.height = height
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose upper-left corner
		''' is at (0,&nbsp;0) in the coordinate space, and whose width and
		''' height are specified by the arguments of the same name. </summary>
		''' <param name="width"> the width of the <code>Rectangle</code> </param>
		''' <param name="height"> the height of the <code>Rectangle</code> </param>
		Public Sub New(ByVal width As Integer, ByVal height As Integer)
			Me.New(0, 0, width, height)
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose upper-left corner is
		''' specified by the <seealso cref="Point"/> argument, and
		''' whose width and height are specified by the
		''' <seealso cref="Dimension"/> argument. </summary>
		''' <param name="p"> a <code>Point</code> that is the upper-left corner of
		''' the <code>Rectangle</code> </param>
		''' <param name="d"> a <code>Dimension</code>, representing the
		''' width and height of the <code>Rectangle</code> </param>
		Public Sub New(ByVal p As Point, ByVal d As Dimension)
			Me.New(p.x, p.y, d.width, d.height)
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose upper-left corner is the
		''' specified <code>Point</code>, and whose width and height are both zero. </summary>
		''' <param name="p"> a <code>Point</code> that is the top left corner
		''' of the <code>Rectangle</code> </param>
		Public Sub New(ByVal p As Point)
			Me.New(p.x, p.y, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a new <code>Rectangle</code> whose top left corner is
		''' (0,&nbsp;0) and whose width and height are specified
		''' by the <code>Dimension</code> argument. </summary>
		''' <param name="d"> a <code>Dimension</code>, specifying width and height </param>
		Public Sub New(ByVal d As Dimension)
			Me.New(0, 0, d.width, d.height)
		End Sub

		''' <summary>
		''' Returns the X coordinate of the bounding <code>Rectangle</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the X coordinate of the bounding <code>Rectangle</code>. </returns>
		Public Property Overrides x As Double
			Get
				Return x
			End Get
		End Property

		''' <summary>
		''' Returns the Y coordinate of the bounding <code>Rectangle</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the Y coordinate of the bounding <code>Rectangle</code>. </returns>
		Public Property Overrides y As Double
			Get
				Return y
			End Get
		End Property

		''' <summary>
		''' Returns the width of the bounding <code>Rectangle</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the width of the bounding <code>Rectangle</code>. </returns>
		Public Property Overrides width As Double
			Get
				Return width
			End Get
		End Property

		''' <summary>
		''' Returns the height of the bounding <code>Rectangle</code> in
		''' <code>double</code> precision. </summary>
		''' <returns> the height of the bounding <code>Rectangle</code>. </returns>
		Public Property Overrides height As Double
			Get
				Return height
			End Get
		End Property

		''' <summary>
		''' Gets the bounding <code>Rectangle</code> of this <code>Rectangle</code>.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>getBounds</code> method of
		''' <seealso cref="Component"/>. </summary>
		''' <returns>    a new <code>Rectangle</code>, equal to the
		''' bounding <code>Rectangle</code> for this <code>Rectangle</code>. </returns>
		''' <seealso cref=       java.awt.Component#getBounds </seealso>
		''' <seealso cref=       #setBounds(Rectangle) </seealso>
		''' <seealso cref=       #setBounds(int, int, int, int)
		''' @since     1.1 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property Overrides bounds As Rectangle Implements Shape.getBounds
			Get
				Return New Rectangle(x, y, width, height)
			End Get
			Set(ByVal r As Rectangle)
				boundsnds(r.x, r.y, r.width, r.height)
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Property Overrides bounds2D As java.awt.geom.Rectangle2D Implements Shape.getBounds2D
			Get
				Return New Rectangle(x, y, width, height)
			End Get
		End Property


		''' <summary>
		''' Sets the bounding <code>Rectangle</code> of this
		''' <code>Rectangle</code> to the specified
		''' <code>x</code>, <code>y</code>, <code>width</code>,
		''' and <code>height</code>.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>setBounds</code> method of <code>Component</code>. </summary>
		''' <param name="x"> the new X coordinate for the upper-left
		'''                    corner of this <code>Rectangle</code> </param>
		''' <param name="y"> the new Y coordinate for the upper-left
		'''                    corner of this <code>Rectangle</code> </param>
		''' <param name="width"> the new width for this <code>Rectangle</code> </param>
		''' <param name="height"> the new height for this <code>Rectangle</code> </param>
		''' <seealso cref=       #getBounds </seealso>
		''' <seealso cref=       java.awt.Component#setBounds(int, int, int, int)
		''' @since     1.1 </seealso>
		Public Overridable Sub setBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			reshape(x, y, width, height)
		End Sub

		''' <summary>
		''' Sets the bounds of this {@code Rectangle} to the integer bounds
		''' which encompass the specified {@code x}, {@code y}, {@code width},
		''' and {@code height}.
		''' If the parameters specify a {@code Rectangle} that exceeds the
		''' maximum range of integers, the result will be the best
		''' representation of the specified {@code Rectangle} intersected
		''' with the maximum integer bounds. </summary>
		''' <param name="x"> the X coordinate of the upper-left corner of
		'''                  the specified rectangle </param>
		''' <param name="y"> the Y coordinate of the upper-left corner of
		'''                  the specified rectangle </param>
		''' <param name="width"> the width of the specified rectangle </param>
		''' <param name="height"> the new height of the specified rectangle </param>
		Public Overrides Sub setRect(ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal height As Double)
			Dim newx, newy, neww, newh As Integer

			If x > 2.0 * Integer.MaxValue Then
				' Too far in positive X direction to represent...
				' We cannot even reach the left side of the specified
				' rectangle even with both x & width set to MAX_VALUE.
				' The intersection with the "maximal integer rectangle"
				' is non-existant so we should use a width < 0.
				' REMIND: Should we try to determine a more "meaningful"
				' adjusted value for neww than just "-1"?
				newx = Integer.MaxValue
				neww = -1
			Else
				newx = clip(x, False)
				If width >= 0 Then width += x-newx
				neww = clip(width, width >= 0)
			End If

			If y > 2.0 * Integer.MaxValue Then
				' Too far in positive Y direction to represent...
				newy = Integer.MaxValue
				newh = -1
			Else
				newy = clip(y, False)
				If height >= 0 Then height += y-newy
				newh = clip(height, height >= 0)
			End If

			reshape(newx, newy, neww, newh)
		End Sub
		' Return best integer representation for v, clipped to integer
		' range and floor-ed or ceiling-ed, depending on the boolean.
		Private Shared Function clip(ByVal v As Double, ByVal doceil As Boolean) As Integer
			If v <= Integer.MinValue Then Return Integer.MinValue
			If v >= Integer.MaxValue Then Return Integer.MaxValue
			Return CInt(Fix(If(doceil, Math.Ceiling(v), Math.Floor(v))))
		End Function

		''' <summary>
		''' Sets the bounding <code>Rectangle</code> of this
		''' <code>Rectangle</code> to the specified
		''' <code>x</code>, <code>y</code>, <code>width</code>,
		''' and <code>height</code>.
		''' <p> </summary>
		''' <param name="x"> the new X coordinate for the upper-left
		'''                    corner of this <code>Rectangle</code> </param>
		''' <param name="y"> the new Y coordinate for the upper-left
		'''                    corner of this <code>Rectangle</code> </param>
		''' <param name="width"> the new width for this <code>Rectangle</code> </param>
		''' <param name="height"> the new height for this <code>Rectangle</code> </param>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setBounds(int, int, int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub reshape(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Me.x = x
			Me.y = y
			Me.width = width
			Me.height = height
		End Sub

		''' <summary>
		''' Returns the location of this <code>Rectangle</code>.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>getLocation</code> method of <code>Component</code>. </summary>
		''' <returns> the <code>Point</code> that is the upper-left corner of
		'''                  this <code>Rectangle</code>. </returns>
		''' <seealso cref=       java.awt.Component#getLocation </seealso>
		''' <seealso cref=       #setLocation(Point) </seealso>
		''' <seealso cref=       #setLocation(int, int)
		''' @since     1.1 </seealso>
		Public Overridable Property location As Point
			Get
				Return New Point(x, y)
			End Get
			Set(ByVal p As Point)
				locationion(p.x, p.y)
			End Set
		End Property


		''' <summary>
		''' Moves this <code>Rectangle</code> to the specified location.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>setLocation</code> method of <code>Component</code>. </summary>
		''' <param name="x"> the X coordinate of the new location </param>
		''' <param name="y"> the Y coordinate of the new location </param>
		''' <seealso cref=       #getLocation </seealso>
		''' <seealso cref=       java.awt.Component#setLocation(int, int)
		''' @since     1.1 </seealso>
		Public Overridable Sub setLocation(ByVal x As Integer, ByVal y As Integer)
			move(x, y)
		End Sub

		''' <summary>
		''' Moves this <code>Rectangle</code> to the specified location.
		''' <p> </summary>
		''' <param name="x"> the X coordinate of the new location </param>
		''' <param name="y"> the Y coordinate of the new location </param>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setLocation(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub move(ByVal x As Integer, ByVal y As Integer)
			Me.x = x
			Me.y = y
		End Sub

		''' <summary>
		''' Translates this <code>Rectangle</code> the indicated distance,
		''' to the right along the X coordinate axis, and
		''' downward along the Y coordinate axis. </summary>
		''' <param name="dx"> the distance to move this <code>Rectangle</code>
		'''                 along the X axis </param>
		''' <param name="dy"> the distance to move this <code>Rectangle</code>
		'''                 along the Y axis </param>
		''' <seealso cref=       java.awt.Rectangle#setLocation(int, int) </seealso>
		''' <seealso cref=       java.awt.Rectangle#setLocation(java.awt.Point) </seealso>
		Public Overridable Sub translate(ByVal dx As Integer, ByVal dy As Integer)
			Dim oldv As Integer = Me.x
			Dim newv As Integer = oldv + dx
			If dx < 0 Then
				' moving leftward
				If newv > oldv Then
					' negative overflow
					' Only adjust width if it was valid (>= 0).
					If width >= 0 Then width += newv - Integer.MinValue
					newv = Integer.MinValue
				End If
			Else
				' moving rightward (or staying still)
				If newv < oldv Then
					' positive overflow
					If width >= 0 Then
						' Conceptually the same as:
						' width += newv; newv = MAX_VALUE; width -= newv;
						width += newv - Integer.MaxValue
						' With large widths and large displacements
						' we may overflow so we need to check it.
						If width < 0 Then width = Integer.MaxValue
					End If
					newv = Integer.MaxValue
				End If
			End If
			Me.x = newv

			oldv = Me.y
			newv = oldv + dy
			If dy < 0 Then
				' moving upward
				If newv > oldv Then
					' negative overflow
					If height >= 0 Then height += newv - Integer.MinValue
					newv = Integer.MinValue
				End If
			Else
				' moving downward (or staying still)
				If newv < oldv Then
					' positive overflow
					If height >= 0 Then
						height += newv - Integer.MaxValue
						If height < 0 Then height = Integer.MaxValue
					End If
					newv = Integer.MaxValue
				End If
			End If
			Me.y = newv
		End Sub

		''' <summary>
		''' Gets the size of this <code>Rectangle</code>, represented by
		''' the returned <code>Dimension</code>.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>getSize</code> method of <code>Component</code>. </summary>
		''' <returns> a <code>Dimension</code>, representing the size of
		'''            this <code>Rectangle</code>. </returns>
		''' <seealso cref=       java.awt.Component#getSize </seealso>
		''' <seealso cref=       #setSize(Dimension) </seealso>
		''' <seealso cref=       #setSize(int, int)
		''' @since     1.1 </seealso>
		Public Overridable Property size As Dimension
			Get
				Return New Dimension(width, height)
			End Get
			Set(ByVal d As Dimension)
				sizeize(d.width, d.height)
			End Set
		End Property


		''' <summary>
		''' Sets the size of this <code>Rectangle</code> to the specified
		''' width and height.
		''' <p>
		''' This method is included for completeness, to parallel the
		''' <code>setSize</code> method of <code>Component</code>. </summary>
		''' <param name="width"> the new width for this <code>Rectangle</code> </param>
		''' <param name="height"> the new height for this <code>Rectangle</code> </param>
		''' <seealso cref=       java.awt.Component#setSize(int, int) </seealso>
		''' <seealso cref=       #getSize
		''' @since     1.1 </seealso>
		Public Overridable Sub setSize(ByVal width As Integer, ByVal height As Integer)
			resize(width, height)
		End Sub

		''' <summary>
		''' Sets the size of this <code>Rectangle</code> to the specified
		''' width and height.
		''' <p> </summary>
		''' <param name="width"> the new width for this <code>Rectangle</code> </param>
		''' <param name="height"> the new height for this <code>Rectangle</code> </param>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setSize(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub resize(ByVal width As Integer, ByVal height As Integer)
			Me.width = width
			Me.height = height
		End Sub

		''' <summary>
		''' Checks whether or not this <code>Rectangle</code> contains the
		''' specified <code>Point</code>. </summary>
		''' <param name="p"> the <code>Point</code> to test </param>
		''' <returns>    <code>true</code> if the specified <code>Point</code>
		'''            is inside this <code>Rectangle</code>;
		'''            <code>false</code> otherwise.
		''' @since     1.1 </returns>
		Public Overridable Function contains(ByVal p As Point) As Boolean
			Return contains(p.x, p.y)
		End Function

		''' <summary>
		''' Checks whether or not this <code>Rectangle</code> contains the
		''' point at the specified location {@code (x,y)}.
		''' </summary>
		''' <param name="x"> the specified X coordinate </param>
		''' <param name="y"> the specified Y coordinate </param>
		''' <returns>    <code>true</code> if the point
		'''            {@code (x,y)} is inside this
		'''            <code>Rectangle</code>;
		'''            <code>false</code> otherwise.
		''' @since     1.1 </returns>
		Public Overridable Function contains(ByVal x As Integer, ByVal y As Integer) As Boolean
			Return inside(x, y)
		End Function

		''' <summary>
		''' Checks whether or not this <code>Rectangle</code> entirely contains
		''' the specified <code>Rectangle</code>.
		''' </summary>
		''' <param name="r">   the specified <code>Rectangle</code> </param>
		''' <returns>    <code>true</code> if the <code>Rectangle</code>
		'''            is contained entirely inside this <code>Rectangle</code>;
		'''            <code>false</code> otherwise
		''' @since     1.2 </returns>
		Public Overridable Function contains(ByVal r As Rectangle) As Boolean
			Return contains(r.x, r.y, r.width, r.height)
		End Function

		''' <summary>
		''' Checks whether this <code>Rectangle</code> entirely contains
		''' the <code>Rectangle</code>
		''' at the specified location {@code (X,Y)} with the
		''' specified dimensions {@code (W,H)}. </summary>
		''' <param name="X"> the specified X coordinate </param>
		''' <param name="Y"> the specified Y coordinate </param>
		''' <param name="W">   the width of the <code>Rectangle</code> </param>
		''' <param name="H">   the height of the <code>Rectangle</code> </param>
		''' <returns>    <code>true</code> if the <code>Rectangle</code> specified by
		'''            {@code (X, Y, W, H)}
		'''            is entirely enclosed inside this <code>Rectangle</code>;
		'''            <code>false</code> otherwise.
		''' @since     1.1 </returns>
		Public Overridable Function contains(ByVal X As Integer, ByVal Y As Integer, ByVal W As Integer, ByVal H As Integer) As Boolean
			Dim w_Renamed As Integer = Me.width
			Dim h_Renamed As Integer = Me.height
			If (w_Renamed Or h_Renamed Or W Or H) < 0 Then Return False
			' Note: if any dimension is zero, tests below must return false...
			Dim x_Renamed As Integer = Me.x
			Dim y_Renamed As Integer = Me.y
			If X < x_Renamed OrElse Y < y_Renamed Then Return False
			w_Renamed += x_Renamed
			W += X
			If W <= X Then
				' X+W overflowed or W was zero, return false if...
				' either original w or W was zero or
				' x+w did not overflow or
				' the overflowed x+w is smaller than the overflowed X+W
				If w_Renamed >= x_Renamed OrElse W > w_Renamed Then Return False
			Else
				' X+W did not overflow and W was not zero, return false if...
				' original w was zero or
				' x+w did not overflow and x+w is smaller than X+W
				If w_Renamed >= x_Renamed AndAlso W > w_Renamed Then Return False
			End If
			h_Renamed += y_Renamed
			H += Y
			If H <= Y Then
				If h_Renamed >= y_Renamed OrElse H > h_Renamed Then Return False
			Else
				If h_Renamed >= y_Renamed AndAlso H > h_Renamed Then Return False
			End If
			Return True
		End Function

		''' <summary>
		''' Checks whether or not this <code>Rectangle</code> contains the
		''' point at the specified location {@code (X,Y)}.
		''' </summary>
		''' <param name="X"> the specified X coordinate </param>
		''' <param name="Y"> the specified Y coordinate </param>
		''' <returns>    <code>true</code> if the point
		'''            {@code (X,Y)} is inside this
		'''            <code>Rectangle</code>;
		'''            <code>false</code> otherwise. </returns>
		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>contains(int, int)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Function inside(ByVal X As Integer, ByVal Y As Integer) As Boolean
			Dim w As Integer = Me.width
			Dim h As Integer = Me.height
			If (w Or h) < 0 Then Return False
			' Note: if either dimension is zero, tests below must return false...
			Dim x_Renamed As Integer = Me.x
			Dim y_Renamed As Integer = Me.y
			If X < x_Renamed OrElse Y < y_Renamed Then Return False
			w += x_Renamed
			h += y_Renamed
			'    overflow || intersect
			Return ((w < x_Renamed OrElse w > X) AndAlso (h < y_Renamed OrElse h > Y))
		End Function

		''' <summary>
		''' Determines whether or not this <code>Rectangle</code> and the specified
		''' <code>Rectangle</code> intersect. Two rectangles intersect if
		''' their intersection is nonempty.
		''' </summary>
		''' <param name="r"> the specified <code>Rectangle</code> </param>
		''' <returns>    <code>true</code> if the specified <code>Rectangle</code>
		'''            and this <code>Rectangle</code> intersect;
		'''            <code>false</code> otherwise. </returns>
		Public Overridable Function intersects(ByVal r As Rectangle) As Boolean
			Dim tw As Integer = Me.width
			Dim th As Integer = Me.height
			Dim rw As Integer = r.width
			Dim rh As Integer = r.height
			If rw <= 0 OrElse rh <= 0 OrElse tw <= 0 OrElse th <= 0 Then Return False
			Dim tx As Integer = Me.x
			Dim ty As Integer = Me.y
			Dim rx As Integer = r.x
			Dim ry As Integer = r.y
			rw += rx
			rh += ry
			tw += tx
			th += ty
			'      overflow || intersect
			Return ((rw < rx OrElse rw > tx) AndAlso (rh < ry OrElse rh > ty) AndAlso (tw < tx OrElse tw > rx) AndAlso (th < ty OrElse th > ry))
		End Function

		''' <summary>
		''' Computes the intersection of this <code>Rectangle</code> with the
		''' specified <code>Rectangle</code>. Returns a new <code>Rectangle</code>
		''' that represents the intersection of the two rectangles.
		''' If the two rectangles do not intersect, the result will be
		''' an empty rectangle.
		''' </summary>
		''' <param name="r">   the specified <code>Rectangle</code> </param>
		''' <returns>    the largest <code>Rectangle</code> contained in both the
		'''            specified <code>Rectangle</code> and in
		'''            this <code>Rectangle</code>; or if the rectangles
		'''            do not intersect, an empty rectangle. </returns>
		Public Overridable Function intersection(ByVal r As Rectangle) As Rectangle
			Dim tx1 As Integer = Me.x
			Dim ty1 As Integer = Me.y
			Dim rx1 As Integer = r.x
			Dim ry1 As Integer = r.y
			Dim tx2 As Long = tx1
			tx2 += Me.width
			Dim ty2 As Long = ty1
			ty2 += Me.height
			Dim rx2 As Long = rx1
			rx2 += r.width
			Dim ry2 As Long = ry1
			ry2 += r.height
			If tx1 < rx1 Then tx1 = rx1
			If ty1 < ry1 Then ty1 = ry1
			If tx2 > rx2 Then tx2 = rx2
			If ty2 > ry2 Then ty2 = ry2
			tx2 -= tx1
			ty2 -= ty1
			' tx2,ty2 will never overflow (they will never be
			' larger than the smallest of the two source w,h)
			' they might underflow, though...
			If tx2 < Integer.MinValue Then tx2 = Integer.MinValue
			If ty2 < Integer.MinValue Then ty2 = Integer.MinValue
			Return New Rectangle(tx1, ty1, CInt(tx2), CInt(ty2))
		End Function

		''' <summary>
		''' Computes the union of this <code>Rectangle</code> with the
		''' specified <code>Rectangle</code>. Returns a new
		''' <code>Rectangle</code> that
		''' represents the union of the two rectangles.
		''' <p>
		''' If either {@code Rectangle} has any dimension less than zero
		''' the rules for <a href=#NonExistant>non-existant</a> rectangles
		''' apply.
		''' If only one has a dimension less than zero, then the result
		''' will be a copy of the other {@code Rectangle}.
		''' If both have dimension less than zero, then the result will
		''' have at least one dimension less than zero.
		''' <p>
		''' If the resulting {@code Rectangle} would have a dimension
		''' too large to be expressed as an {@code int}, the result
		''' will have a dimension of {@code  [Integer].MAX_VALUE} along
		''' that dimension. </summary>
		''' <param name="r"> the specified <code>Rectangle</code> </param>
		''' <returns>    the smallest <code>Rectangle</code> containing both
		'''            the specified <code>Rectangle</code> and this
		'''            <code>Rectangle</code>. </returns>
		Public Overridable Function union(ByVal r As Rectangle) As Rectangle
			Dim tx2 As Long = Me.width
			Dim ty2 As Long = Me.height
			If (tx2 Or ty2) < 0 Then Return New Rectangle(r)
			Dim rx2 As Long = r.width
			Dim ry2 As Long = r.height
			If (rx2 Or ry2) < 0 Then Return New Rectangle(Me)
			Dim tx1 As Integer = Me.x
			Dim ty1 As Integer = Me.y
			tx2 += tx1
			ty2 += ty1
			Dim rx1 As Integer = r.x
			Dim ry1 As Integer = r.y
			rx2 += rx1
			ry2 += ry1
			If tx1 > rx1 Then tx1 = rx1
			If ty1 > ry1 Then ty1 = ry1
			If tx2 < rx2 Then tx2 = rx2
			If ty2 < ry2 Then ty2 = ry2
			tx2 -= tx1
			ty2 -= ty1
			' tx2,ty2 will never underflow since both original rectangles
			' were already proven to be non-empty
			' they might overflow, though...
			If tx2 > Integer.MaxValue Then tx2 = Integer.MaxValue
			If ty2 > Integer.MaxValue Then ty2 = Integer.MaxValue
			Return New Rectangle(tx1, ty1, CInt(tx2), CInt(ty2))
		End Function

		''' <summary>
		''' Adds a point, specified by the integer arguments {@code newx,newy}
		''' to the bounds of this {@code Rectangle}.
		''' <p>
		''' If this {@code Rectangle} has any dimension less than zero,
		''' the rules for <a href=#NonExistant>non-existant</a>
		''' rectangles apply.
		''' In that case, the new bounds of this {@code Rectangle} will
		''' have a location equal to the specified coordinates and
		''' width and height equal to zero.
		''' <p>
		''' After adding a point, a call to <code>contains</code> with the
		''' added point as an argument does not necessarily return
		''' <code>true</code>. The <code>contains</code> method does not
		''' return <code>true</code> for points on the right or bottom
		''' edges of a <code>Rectangle</code>. Therefore, if the added point
		''' falls on the right or bottom edge of the enlarged
		''' <code>Rectangle</code>, <code>contains</code> returns
		''' <code>false</code> for that point.
		''' If the specified point must be contained within the new
		''' {@code Rectangle}, a 1x1 rectangle should be added instead:
		''' <pre>
		'''     r.add(newx, newy, 1, 1);
		''' </pre> </summary>
		''' <param name="newx"> the X coordinate of the new point </param>
		''' <param name="newy"> the Y coordinate of the new point </param>
		Public Overridable Sub add(ByVal newx As Integer, ByVal newy As Integer)
			If (width Or height) < 0 Then
				Me.x = newx
				Me.y = newy
					Me.height = 0
					Me.width = Me.height
				Return
			End If
			Dim x1 As Integer = Me.x
			Dim y1 As Integer = Me.y
			Dim x2 As Long = Me.width
			Dim y2 As Long = Me.height
			x2 += x1
			y2 += y1
			If x1 > newx Then x1 = newx
			If y1 > newy Then y1 = newy
			If x2 < newx Then x2 = newx
			If y2 < newy Then y2 = newy
			x2 -= x1
			y2 -= y1
			If x2 > Integer.MaxValue Then x2 = Integer.MaxValue
			If y2 > Integer.MaxValue Then y2 = Integer.MaxValue
			reshape(x1, y1, CInt(x2), CInt(y2))
		End Sub

		''' <summary>
		''' Adds the specified {@code Point} to the bounds of this
		''' {@code Rectangle}.
		''' <p>
		''' If this {@code Rectangle} has any dimension less than zero,
		''' the rules for <a href=#NonExistant>non-existant</a>
		''' rectangles apply.
		''' In that case, the new bounds of this {@code Rectangle} will
		''' have a location equal to the coordinates of the specified
		''' {@code Point} and width and height equal to zero.
		''' <p>
		''' After adding a <code>Point</code>, a call to <code>contains</code>
		''' with the added <code>Point</code> as an argument does not
		''' necessarily return <code>true</code>. The <code>contains</code>
		''' method does not return <code>true</code> for points on the right
		''' or bottom edges of a <code>Rectangle</code>. Therefore if the added
		''' <code>Point</code> falls on the right or bottom edge of the
		''' enlarged <code>Rectangle</code>, <code>contains</code> returns
		''' <code>false</code> for that <code>Point</code>.
		''' If the specified point must be contained within the new
		''' {@code Rectangle}, a 1x1 rectangle should be added instead:
		''' <pre>
		'''     r.add(pt.x, pt.y, 1, 1);
		''' </pre> </summary>
		''' <param name="pt"> the new <code>Point</code> to add to this
		'''           <code>Rectangle</code> </param>
		Public Overridable Sub add(ByVal pt As Point)
			add(pt.x, pt.y)
		End Sub

		''' <summary>
		''' Adds a <code>Rectangle</code> to this <code>Rectangle</code>.
		''' The resulting <code>Rectangle</code> is the union of the two
		''' rectangles.
		''' <p>
		''' If either {@code Rectangle} has any dimension less than 0, the
		''' result will have the dimensions of the other {@code Rectangle}.
		''' If both {@code Rectangle}s have at least one dimension less
		''' than 0, the result will have at least one dimension less than 0.
		''' <p>
		''' If either {@code Rectangle} has one or both dimensions equal
		''' to 0, the result along those axes with 0 dimensions will be
		''' equivalent to the results obtained by adding the corresponding
		''' origin coordinate to the result rectangle along that axis,
		''' similar to the operation of the <seealso cref="#add(Point)"/> method,
		''' but contribute no further dimension beyond that.
		''' <p>
		''' If the resulting {@code Rectangle} would have a dimension
		''' too large to be expressed as an {@code int}, the result
		''' will have a dimension of {@code  [Integer].MAX_VALUE} along
		''' that dimension. </summary>
		''' <param name="r"> the specified <code>Rectangle</code> </param>
		Public Overridable Sub add(ByVal r As Rectangle)
			Dim tx2 As Long = Me.width
			Dim ty2 As Long = Me.height
			If (tx2 Or ty2) < 0 Then reshape(r.x, r.y, r.width, r.height)
			Dim rx2 As Long = r.width
			Dim ry2 As Long = r.height
			If (rx2 Or ry2) < 0 Then Return
			Dim tx1 As Integer = Me.x
			Dim ty1 As Integer = Me.y
			tx2 += tx1
			ty2 += ty1
			Dim rx1 As Integer = r.x
			Dim ry1 As Integer = r.y
			rx2 += rx1
			ry2 += ry1
			If tx1 > rx1 Then tx1 = rx1
			If ty1 > ry1 Then ty1 = ry1
			If tx2 < rx2 Then tx2 = rx2
			If ty2 < ry2 Then ty2 = ry2
			tx2 -= tx1
			ty2 -= ty1
			' tx2,ty2 will never underflow since both original
			' rectangles were non-empty
			' they might overflow, though...
			If tx2 > Integer.MaxValue Then tx2 = Integer.MaxValue
			If ty2 > Integer.MaxValue Then ty2 = Integer.MaxValue
			reshape(tx1, ty1, CInt(tx2), CInt(ty2))
		End Sub

		''' <summary>
		''' Resizes the <code>Rectangle</code> both horizontally and vertically.
		''' <p>
		''' This method modifies the <code>Rectangle</code> so that it is
		''' <code>h</code> units larger on both the left and right side,
		''' and <code>v</code> units larger at both the top and bottom.
		''' <p>
		''' The new <code>Rectangle</code> has {@code (x - h, y - v)}
		''' as its upper-left corner,
		''' width of {@code (width + 2h)},
		''' and a height of {@code (height + 2v)}.
		''' <p>
		''' If negative values are supplied for <code>h</code> and
		''' <code>v</code>, the size of the <code>Rectangle</code>
		''' decreases accordingly.
		''' The {@code grow} method will check for integer overflow
		''' and underflow, but does not check whether the resulting
		''' values of {@code width} and {@code height} grow
		''' from negative to non-negative or shrink from non-negative
		''' to negative. </summary>
		''' <param name="h"> the horizontal expansion </param>
		''' <param name="v"> the vertical expansion </param>
		Public Overridable Sub grow(ByVal h As Integer, ByVal v As Integer)
			Dim x0 As Long = Me.x
			Dim y0 As Long = Me.y
			Dim x1 As Long = Me.width
			Dim y1 As Long = Me.height
			x1 += x0
			y1 += y0

			x0 -= h
			y0 -= v
			x1 += h
			y1 += v

			If x1 < x0 Then
				' Non-existant in X direction
				' Final width must remain negative so subtract x0 before
				' it is clipped so that we avoid the risk that the clipping
				' of x0 will reverse the ordering of x0 and x1.
				x1 -= x0
				If x1 < Integer.MinValue Then x1 = Integer.MinValue
				If x0 < Integer.MinValue Then
					x0 = Integer.MinValue
				ElseIf x0 > Integer.MaxValue Then
					x0 = Integer.MaxValue
				End If ' (x1 >= x0)
			Else
				' Clip x0 before we subtract it from x1 in case the clipping
				' affects the representable area of the rectangle.
				If x0 < Integer.MinValue Then
					x0 = Integer.MinValue
				ElseIf x0 > Integer.MaxValue Then
					x0 = Integer.MaxValue
				End If
				x1 -= x0
				' The only way x1 can be negative now is if we clipped
				' x0 against MIN and x1 is less than MIN - in which case
				' we want to leave the width negative since the result
				' did not intersect the representable area.
				If x1 < Integer.MinValue Then
					x1 = Integer.MinValue
				ElseIf x1 > Integer.MaxValue Then
					x1 = Integer.MaxValue
				End If
			End If

			If y1 < y0 Then
				' Non-existant in Y direction
				y1 -= y0
				If y1 < Integer.MinValue Then y1 = Integer.MinValue
				If y0 < Integer.MinValue Then
					y0 = Integer.MinValue
				ElseIf y0 > Integer.MaxValue Then
					y0 = Integer.MaxValue
				End If ' (y1 >= y0)
			Else
				If y0 < Integer.MinValue Then
					y0 = Integer.MinValue
				ElseIf y0 > Integer.MaxValue Then
					y0 = Integer.MaxValue
				End If
				y1 -= y0
				If y1 < Integer.MinValue Then
					y1 = Integer.MinValue
				ElseIf y1 > Integer.MaxValue Then
					y1 = Integer.MaxValue
				End If
			End If

			reshape(CInt(x0), CInt(y0), CInt(x1), CInt(y1))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Property Overrides empty As Boolean
			Get
				Return (width <= 0) OrElse (height <= 0)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overrides Function outcode(ByVal x As Double, ByVal y As Double) As Integer
	'        
	'         * Note on casts to double below.  If the arithmetic of
	'         * x+w or y+h is done in int, then we may get integer
	'         * overflow. By converting to double before the addition
	'         * we force the addition to be carried out in double to
	'         * avoid overflow in the comparison.
	'         *
	'         * See bug 4320890 for problems that this can cause.
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
		Public Overridable Function createIntersection(ByVal r As java.awt.geom.Rectangle2D) As java.awt.geom.Rectangle2D
			If TypeOf r Is Rectangle Then Return intersection(CType(r, Rectangle))
			Dim dest As java.awt.geom.Rectangle2D = New java.awt.geom.Rectangle2D.Double
			java.awt.geom.Rectangle2D.intersect(Me, r, dest)
			Return dest
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.2
		''' </summary>
		Public Overridable Function createUnion(ByVal r As java.awt.geom.Rectangle2D) As java.awt.geom.Rectangle2D
			If TypeOf r Is Rectangle Then Return union(CType(r, Rectangle))
			Dim dest As java.awt.geom.Rectangle2D = New java.awt.geom.Rectangle2D.Double
			java.awt.geom.Rectangle2D.union(Me, r, dest)
			Return dest
		End Function

		''' <summary>
		''' Checks whether two rectangles are equal.
		''' <p>
		''' The result is <code>true</code> if and only if the argument is not
		''' <code>null</code> and is a <code>Rectangle</code> object that has the
		''' same upper-left corner, width, and height as
		''' this <code>Rectangle</code>. </summary>
		''' <param name="obj"> the <code>Object</code> to compare with
		'''                this <code>Rectangle</code> </param>
		''' <returns>    <code>true</code> if the objects are equal;
		'''            <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Rectangle Then
				Dim r As Rectangle = CType(obj, Rectangle)
				Return ((x = r.x) AndAlso (y = r.y) AndAlso (width = r.width) AndAlso (height = r.height))
			End If
			Return MyBase.Equals(obj)
		End Function

		''' <summary>
		''' Returns a <code>String</code> representing this
		''' <code>Rectangle</code> and its values. </summary>
		''' <returns> a <code>String</code> representing this
		'''               <code>Rectangle</code> object's coordinate and size values. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[x=" & x & ",y=" & y & ",width=" & width & ",height=" & height & "]"
		End Function
	End Class

End Namespace