Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing


	''' <summary>
	''' A <code>SpringLayout</code> lays out the children of its associated container
	''' according to a set of constraints.
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/layout/spring.html">How to Use SpringLayout</a>
	''' in <em>The Java Tutorial</em> for examples of using
	''' <code>SpringLayout</code>.
	''' 
	''' <p>
	''' Each constraint,
	''' represented by a <code>Spring</code> object,
	''' controls the vertical or horizontal distance
	''' between two component edges.
	''' The edges can belong to
	''' any child of the container,
	''' or to the container itself.
	''' For example,
	''' the allowable width of a component
	''' can be expressed using a constraint
	''' that controls the distance between the west (left) and east (right)
	''' edges of the component.
	''' The allowable <em>y</em> coordinates for a component
	''' can be expressed by constraining the distance between
	''' the north (top) edge of the component
	''' and the north edge of its container.
	''' 
	''' <P>
	''' Every child of a <code>SpringLayout</code>-controlled container,
	''' as well as the container itself,
	''' has exactly one set of constraints
	''' associated with it.
	''' These constraints are represented by
	''' a <code>SpringLayout.Constraints</code> object.
	''' By default,
	''' <code>SpringLayout</code> creates constraints
	''' that make their associated component
	''' have the minimum, preferred, and maximum sizes
	''' returned by the component's
	''' <seealso cref="java.awt.Component#getMinimumSize"/>,
	''' <seealso cref="java.awt.Component#getPreferredSize"/>, and
	''' <seealso cref="java.awt.Component#getMaximumSize"/>
	''' methods. The <em>x</em> and <em>y</em> positions are initially not
	''' constrained, so that until you constrain them the <code>Component</code>
	''' will be positioned at 0,0 relative to the <code>Insets</code> of the
	''' parent <code>Container</code>.
	''' 
	''' <p>
	''' You can change
	''' a component's constraints in several ways.
	''' You can
	''' use one of the
	''' <seealso cref="#putConstraint putConstraint"/>
	''' methods
	''' to establish a spring
	''' linking the edges of two components within the same container.
	''' Or you can get the appropriate <code>SpringLayout.Constraints</code>
	''' object using
	''' <seealso cref="#getConstraints getConstraints"/>
	''' and then modify one or more of its springs.
	''' Or you can get the spring for a particular edge of a component
	''' using <seealso cref="#getConstraint getConstraint"/>,
	''' and modify it.
	''' You can also associate
	''' your own <code>SpringLayout.Constraints</code> object
	''' with a component by specifying the constraints object
	''' when you add the component to its container
	''' (using
	''' <seealso cref="Container#add(Component, Object)"/>).
	''' 
	''' <p>
	''' The <code>Spring</code> object representing each constraint
	''' has a minimum, preferred, maximum, and current value.
	''' The current value of the spring
	''' is somewhere between the minimum and maximum values,
	''' according to the formula given in the
	''' <seealso cref="Spring#sum"/> method description.
	''' When the minimum, preferred, and maximum values are the same,
	''' the current value is always equal to them;
	''' this inflexible spring is called a <em>strut</em>.
	''' You can create struts using the factory method
	''' <seealso cref="Spring#constant(int)"/>.
	''' The <code>Spring</code> class also provides factory methods
	''' for creating other kinds of springs,
	''' including springs that depend on other springs.
	''' 
	''' <p>
	''' In a <code>SpringLayout</code>, the position of each edge is dependent on
	''' the position of just one other edge. If a constraint is subsequently added
	''' to create a new binding for an edge, the previous binding is discarded
	''' and the edge remains dependent on a single edge.
	''' Springs should only be attached
	''' between edges of the container and its immediate children; the behavior
	''' of the <code>SpringLayout</code> when presented with constraints linking
	''' the edges of components from different containers (either internal or
	''' external) is undefined.
	''' 
	''' <h3>
	''' SpringLayout vs. Other Layout Managers
	''' </h3>
	''' 
	''' <blockquote>
	''' <hr>
	''' <strong>Note:</strong>
	''' Unlike many layout managers,
	''' <code>SpringLayout</code> doesn't automatically set the location of
	''' the components it manages.
	''' If you hand-code a GUI that uses <code>SpringLayout</code>,
	''' remember to initialize component locations by constraining the west/east
	''' and north/south locations.
	''' <p>
	''' Depending on the constraints you use,
	''' you may also need to set the size of the container explicitly.
	''' <hr>
	''' </blockquote>
	''' 
	''' <p>
	''' Despite the simplicity of <code>SpringLayout</code>,
	''' it can emulate the behavior of most other layout managers.
	''' For some features,
	''' such as the line breaking provided by <code>FlowLayout</code>,
	''' you'll need to
	''' create a special-purpose subclass of the <code>Spring</code> class.
	''' 
	''' <p>
	''' <code>SpringLayout</code> also provides a way to solve
	''' many of the difficult layout
	''' problems that cannot be solved by nesting combinations
	''' of <code>Box</code>es. That said, <code>SpringLayout</code> honors the
	''' <code>LayoutManager2</code> contract correctly and so can be nested with
	''' other layout managers -- a technique that can be preferable to
	''' creating the constraints implied by the other layout managers.
	''' <p>
	''' The asymptotic complexity of the layout operation of a <code>SpringLayout</code>
	''' is linear in the number of constraints (and/or components).
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= Spring </seealso>
	''' <seealso cref= SpringLayout.Constraints
	''' 
	''' @author      Philip Milne
	''' @author      Scott Violet
	''' @author      Joe Winchester
	''' @since       1.4 </seealso>
	Public Class SpringLayout
		Implements java.awt.LayoutManager2

		Private componentConstraints As IDictionary(Of java.awt.Component, Constraints) = New Dictionary(Of java.awt.Component, Constraints)

		Private cyclicReference As Spring = Spring.constant(Spring.UNSET)
		Private cyclicSprings As [Set](Of Spring)
		Private acyclicSprings As [Set](Of Spring)


		''' <summary>
		''' Specifies the top edge of a component's bounding rectangle.
		''' </summary>
		Public Const NORTH As String = "North"

		''' <summary>
		''' Specifies the bottom edge of a component's bounding rectangle.
		''' </summary>
		Public Const SOUTH As String = "South"

		''' <summary>
		''' Specifies the right edge of a component's bounding rectangle.
		''' </summary>
		Public Const EAST As String = "East"

		''' <summary>
		''' Specifies the left edge of a component's bounding rectangle.
		''' </summary>
		Public Const WEST As String = "West"

		''' <summary>
		''' Specifies the horizontal center of a component's bounding rectangle.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const HORIZONTAL_CENTER As String = "HorizontalCenter"

		''' <summary>
		''' Specifies the vertical center of a component's bounding rectangle.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const VERTICAL_CENTER As String = "VerticalCenter"

		''' <summary>
		''' Specifies the baseline of a component.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const BASELINE As String = "Baseline"

		''' <summary>
		''' Specifies the width of a component's bounding rectangle.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const WIDTH As String = "Width"

		''' <summary>
		''' Specifies the height of a component's bounding rectangle.
		''' 
		''' @since 1.6
		''' </summary>
		Public Const HEIGHT As String = "Height"

		Private Shared ALL_HORIZONTAL As String() = {WEST, WIDTH, EAST, HORIZONTAL_CENTER}

		Private Shared ALL_VERTICAL As String() = {NORTH, HEIGHT, SOUTH, VERTICAL_CENTER, BASELINE}

		''' <summary>
		''' A <code>Constraints</code> object holds the
		''' constraints that govern the way a component's size and position
		''' change in a container controlled by a <code>SpringLayout</code>.
		''' A <code>Constraints</code> object is
		''' like a <code>Rectangle</code>, in that it
		''' has <code>x</code>, <code>y</code>,
		''' <code>width</code>, and <code>height</code> properties.
		''' In the <code>Constraints</code> object, however,
		''' these properties have
		''' <code>Spring</code> values instead of integers.
		''' In addition,
		''' a <code>Constraints</code> object
		''' can be manipulated as four edges
		''' -- north, south, east, and west --
		''' using the <code>constraint</code> property.
		''' 
		''' <p>
		''' The following formulas are always true
		''' for a <code>Constraints</code> object (here WEST and <code>x</code> are synonyms, as are and NORTH and <code>y</code>):
		''' 
		''' <pre>
		'''               EAST = WEST + WIDTH
		'''              SOUTH = NORTH + HEIGHT
		'''  HORIZONTAL_CENTER = WEST + WIDTH/2
		'''    VERTICAL_CENTER = NORTH + HEIGHT/2
		'''  ABSOLUTE_BASELINE = NORTH + RELATIVE_BASELINE*
		''' </pre>
		''' <p>
		''' For example, if you have specified the WIDTH and WEST (X) location
		''' the EAST is calculated as WEST + WIDTH.  If you instead specified
		''' the WIDTH and EAST locations the WEST (X) location is then calculated
		''' as EAST - WIDTH.
		''' <p>
		''' [RELATIVE_BASELINE is a private constraint that is set automatically when
		''' the SpringLayout.Constraints(Component) constructor is called or when
		''' a constraints object is registered with a SpringLayout object.]
		''' <p>
		''' <b>Note</b>: In this document,
		''' operators represent methods
		''' in the <code>Spring</code> class.
		''' For example, "a + b" is equal to
		''' <code>Spring.sum(a, b)</code>,
		''' and "a - b" is equal to
		''' <code>Spring.sum(a, Spring.minus(b))</code>.
		''' See the
		''' <seealso cref="Spring Spring API documentation"/>
		''' for further details
		''' of spring arithmetic.
		''' 
		''' <p>
		''' 
		''' Because a <code>Constraints</code> object's properties --
		''' representing its edges, size, and location -- can all be set
		''' independently and yet are interrelated,
		''' a <code>Constraints</code> object can become <em>over-constrained</em>.
		''' For example, if the <code>WEST</code>, <code>WIDTH</code> and
		''' <code>EAST</code> edges are all set, steps must be taken to ensure that
		''' the first of the formulas above holds.  To do this, the
		''' <code>Constraints</code>
		''' object throws away the <em>least recently set</em>
		''' constraint so as to make the formulas hold.
		''' @since 1.4
		''' </summary>
		Public Class Constraints
		   Private x As Spring
		   Private y As Spring
		   Private width As Spring
		   Private height As Spring
		   Private east As Spring
		   Private south As Spring
			Private horizontalCenter As Spring
			Private verticalCenter As Spring
			Private baseline As Spring

			Private horizontalHistory As IList(Of String) = New List(Of String)(2)
			Private verticalHistory As IList(Of String) = New List(Of String)(2)

			' Used for baseline calculations
			Private c As java.awt.Component

		   ''' <summary>
		   ''' Creates an empty <code>Constraints</code> object.
		   ''' </summary>
		   Public Sub New()
		   End Sub

		   ''' <summary>
		   ''' Creates a <code>Constraints</code> object with the
		   ''' specified values for its
		   ''' <code>x</code> and <code>y</code> properties.
		   ''' The <code>height</code> and <code>width</code> springs
		   ''' have <code>null</code> values.
		   ''' </summary>
		   ''' <param name="x">  the spring controlling the component's <em>x</em> value </param>
		   ''' <param name="y">  the spring controlling the component's <em>y</em> value </param>
		   Public Sub New(ByVal x As Spring, ByVal y As Spring)
			   x = x
			   y = y
		   End Sub

		   ''' <summary>
		   ''' Creates a <code>Constraints</code> object with the
		   ''' specified values for its
		   ''' <code>x</code>, <code>y</code>, <code>width</code>,
		   ''' and <code>height</code> properties.
		   ''' Note: If the <code>SpringLayout</code> class
		   ''' encounters <code>null</code> values in the
		   ''' <code>Constraints</code> object of a given component,
		   ''' it replaces them with suitable defaults.
		   ''' </summary>
		   ''' <param name="x">  the spring value for the <code>x</code> property </param>
		   ''' <param name="y">  the spring value for the <code>y</code> property </param>
		   ''' <param name="width">  the spring value for the <code>width</code> property </param>
		   ''' <param name="height">  the spring value for the <code>height</code> property </param>
		   Public Sub New(ByVal x As Spring, ByVal y As Spring, ByVal width As Spring, ByVal height As Spring)
			   x = x
			   y = y
			   width = width
			   height = height
		   End Sub

			''' <summary>
			''' Creates a <code>Constraints</code> object with
			''' suitable <code>x</code>, <code>y</code>, <code>width</code> and
			''' <code>height</code> springs for component, <code>c</code>.
			''' The <code>x</code> and <code>y</code> springs are constant
			''' springs  initialised with the component's location at
			''' the time this method is called. The <code>width</code> and
			''' <code>height</code> springs are special springs, created by
			''' the <code>Spring.width()</code> and <code>Spring.height()</code>
			''' methods, which track the size characteristics of the component
			''' when they change.
			''' </summary>
			''' <param name="c">  the component whose characteristics will be reflected by this Constraints object </param>
			''' <exception cref="NullPointerException"> if <code>c</code> is null.
			''' @since 1.5 </exception>
			Public Sub New(ByVal c As java.awt.Component)
				Me.c = c
				x = Spring.constant(c.x)
				y = Spring.constant(c.y)
				width = Spring.width(c)
				height = Spring.height(c)
			End Sub

			Private Sub pushConstraint(ByVal name As String, ByVal value As Spring, ByVal horizontal As Boolean)
				Dim valid As Boolean = True
				Dim history As IList(Of String) = If(horizontal, horizontalHistory, verticalHistory)
				If history.Contains(name) Then
					history.Remove(name)
					valid = False
				ElseIf history.Count = 2 AndAlso value IsNot Nothing Then
					history.RemoveAt(0)
					valid = False
				End If
				If value IsNot Nothing Then history.Add(name)
				If Not valid Then
					Dim all As String() = If(horizontal, ALL_HORIZONTAL, ALL_VERTICAL)
					For Each s As String In all
						If Not history.Contains(s) Then constraintint(s, Nothing)
					Next s
				End If
			End Sub

		   Private Function sum(ByVal s1 As Spring, ByVal s2 As Spring) As Spring
			   Return If(s1 Is Nothing OrElse s2 Is Nothing, Nothing, Spring.sum(s1, s2))
		   End Function

		   Private Function difference(ByVal s1 As Spring, ByVal s2 As Spring) As Spring
			   Return If(s1 Is Nothing OrElse s2 Is Nothing, Nothing, Spring.difference(s1, s2))
		   End Function

			Private Function scale(ByVal s As Spring, ByVal factor As Single) As Spring
				Return If(s Is Nothing, Nothing, Spring.scale(s, factor))
			End Function

			Private Function getBaselineFromHeight(ByVal height As Integer) As Integer
				If height < 0 Then Return -c.getBaseline(c.preferredSize.width, -height)
				Return c.getBaseline(c.preferredSize.width, height)
			End Function

			Private Function getHeightFromBaseLine(ByVal baseline As Integer) As Integer
				Dim prefSize As java.awt.Dimension = c.preferredSize
				Dim prefHeight As Integer = prefSize.height
				Dim prefBaseline As Integer = c.getBaseline(prefSize.width, prefHeight)
				If prefBaseline = baseline Then Return prefHeight
				' Valid baseline
				Select Case c.baselineResizeBehavior
				Case CONSTANT_DESCENT
					Return prefHeight + (baseline - prefBaseline)
				Case CENTER_OFFSET
					Return prefHeight + 2 * (baseline - prefBaseline)
				Case CONSTANT_ASCENT
					' Component baseline and specified baseline will NEVER
					' match, fall through to default
				Case Else ' OTHER
					' No way to map from baseline to height.
				End Select
				Return Integer.MinValue
			End Function

			 Private Function heightToRelativeBaseline(ByVal s As Spring) As Spring
				Return New SpringMapAnonymousInnerClassHelper
			 End Function

			Private Class SpringMapAnonymousInnerClassHelper
				Inherits SpringMap

				Protected Friend Overridable Function map(ByVal i As Integer) As Integer
				   Return outerInstance.getBaselineFromHeight(i)
				End Function

				Protected Friend Overridable Function inv(ByVal i As Integer) As Integer
					Return outerInstance.getHeightFromBaseLine(i)
				End Function
			End Class

			Private Function relativeBaselineToHeight(ByVal s As Spring) As Spring
				Return New SpringMapAnonymousInnerClassHelper2
			End Function

			Private Class SpringMapAnonymousInnerClassHelper2
				Inherits SpringMap

				Protected Friend Overridable Function map(ByVal i As Integer) As Integer
					Return outerInstance.getHeightFromBaseLine(i)
				End Function

				 Protected Friend Overridable Function inv(ByVal i As Integer) As Integer
					Return outerInstance.getBaselineFromHeight(i)
				 End Function
			End Class

			Private Function defined(ByVal history As IList, ByVal s1 As String, ByVal s2 As String) As Boolean
				Return history.Contains(s1) AndAlso history.Contains(s2)
			End Function

		   ''' <summary>
		   ''' Sets the <code>x</code> property,
		   ''' which controls the <code>x</code> value
		   ''' of a component's location.
		   ''' </summary>
		   ''' <param name="x"> the spring controlling the <code>x</code> value
		   '''          of a component's location
		   ''' </param>
		   ''' <seealso cref= #getX </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Property x As Spring
			   Set(ByVal x As Spring)
				   Me.x = x
				   pushConstraint(WEST, x, True)
			   End Set
			   Get
				   If x Is Nothing Then
					   If defined(horizontalHistory, EAST, WIDTH) Then
						   x = difference(east, width)
					   ElseIf defined(horizontalHistory, HORIZONTAL_CENTER, WIDTH) Then
						   x = difference(horizontalCenter, scale(width, 0.5f))
					   ElseIf defined(horizontalHistory, HORIZONTAL_CENTER, EAST) Then
						   x = difference(scale(horizontalCenter, 2f), east)
					   End If
				   End If
				   Return x
			   End Get
		   End Property


		   ''' <summary>
		   ''' Sets the <code>y</code> property,
		   ''' which controls the <code>y</code> value
		   ''' of a component's location.
		   ''' </summary>
		   ''' <param name="y"> the spring controlling the <code>y</code> value
		   '''          of a component's location
		   ''' </param>
		   ''' <seealso cref= #getY </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Property y As Spring
			   Set(ByVal y As Spring)
				   Me.y = y
				   pushConstraint(NORTH, y, False)
			   End Set
			   Get
				   If y Is Nothing Then
					   If defined(verticalHistory, SOUTH, HEIGHT) Then
						   y = difference(south, height)
					   ElseIf defined(verticalHistory, VERTICAL_CENTER, HEIGHT) Then
						   y = difference(verticalCenter, scale(height, 0.5f))
					   ElseIf defined(verticalHistory, VERTICAL_CENTER, SOUTH) Then
						   y = difference(scale(verticalCenter, 2f), south)
					   ElseIf defined(verticalHistory, BASELINE, HEIGHT) Then
						   y = difference(baseline, heightToRelativeBaseline(height))
					   ElseIf defined(verticalHistory, BASELINE, SOUTH) Then
						   y = scale(difference(baseline, heightToRelativeBaseline(south)), 2f)
		'
		'               } else if (defined(verticalHistory, BASELINE, VERTICAL_CENTER)) {
		'                   y = scale(difference(baseline, heightToRelativeBaseline(scale(verticalCenter, 2))), 1f/(1-2*0.5f));
		'
					   End If
				   End If
				   Return y
			   End Get
		   End Property


		   ''' <summary>
		   ''' Sets the <code>width</code> property,
		   ''' which controls the width of a component.
		   ''' </summary>
		   ''' <param name="width"> the spring controlling the width of this
		   ''' <code>Constraints</code> object
		   ''' </param>
		   ''' <seealso cref= #getWidth </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Property width As Spring
			   Set(ByVal width As Spring)
				   Me.width = width
				   pushConstraint(Constraints.WIDTH, width, True)
			   End Set
			   Get
				   If width Is Nothing Then
					   If horizontalHistory.Contains(EAST) Then
						   width = difference(east, x)
					   ElseIf horizontalHistory.Contains(HORIZONTAL_CENTER) Then
						   width = scale(difference(horizontalCenter, x), 2f)
					   End If
				   End If
				   Return width
			   End Get
		   End Property


		   ''' <summary>
		   ''' Sets the <code>height</code> property,
		   ''' which controls the height of a component.
		   ''' </summary>
		   ''' <param name="height"> the spring controlling the height of this <code>Constraints</code>
		   ''' object
		   ''' </param>
		   ''' <seealso cref= #getHeight </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Property height As Spring
			   Set(ByVal height As Spring)
				   Me.height = height
				   pushConstraint(Constraints.HEIGHT, height, False)
			   End Set
			   Get
				   If height Is Nothing Then
					   If verticalHistory.Contains(SOUTH) Then
						   height = difference(south, y)
					   ElseIf verticalHistory.Contains(VERTICAL_CENTER) Then
						   height = scale(difference(verticalCenter, y), 2f)
					   ElseIf verticalHistory.Contains(BASELINE) Then
						   height = relativeBaselineToHeight(difference(baseline, y))
					   End If
				   End If
				   Return height
			   End Get
		   End Property


		   Private Property east As Spring
			   Set(ByVal east As Spring)
				   Me.east = east
				   pushConstraint(Constraints.EAST, east, True)
			   End Set
			   Get
				   If east Is Nothing Then east = sum(x, width)
				   Return east
			   End Get
		   End Property


		   Private Property south As Spring
			   Set(ByVal south As Spring)
				   Me.south = south
				   pushConstraint(Constraints.SOUTH, south, False)
			   End Set
			   Get
				   If south Is Nothing Then south = sum(y, height)
				   Return south
			   End Get
		   End Property


			Private Property horizontalCenter As Spring
				Get
					If horizontalCenter Is Nothing Then horizontalCenter = sum(x, scale(width, 0.5f))
					Return horizontalCenter
				End Get
				Set(ByVal horizontalCenter As Spring)
					Me.horizontalCenter = horizontalCenter
					pushConstraint(HORIZONTAL_CENTER, horizontalCenter, True)
				End Set
			End Property


			Private Property verticalCenter As Spring
				Get
					If verticalCenter Is Nothing Then verticalCenter = sum(y, scale(height, 0.5f))
					Return verticalCenter
				End Get
				Set(ByVal verticalCenter As Spring)
					Me.verticalCenter = verticalCenter
					pushConstraint(VERTICAL_CENTER, verticalCenter, False)
				End Set
			End Property


			Private Property baseline As Spring
				Get
					If baseline Is Nothing Then baseline = sum(y, heightToRelativeBaseline(height))
					Return baseline
				End Get
				Set(ByVal baseline As Spring)
					Me.baseline = baseline
					pushConstraint(Constraints.BASELINE, baseline, False)
				End Set
			End Property


		   ''' <summary>
		   ''' Sets the spring controlling the specified edge.
		   ''' The edge must have one of the following values:
		   ''' <code>SpringLayout.NORTH</code>,
		   ''' <code>SpringLayout.SOUTH</code>,
		   ''' <code>SpringLayout.EAST</code>,
		   ''' <code>SpringLayout.WEST</code>,
		   ''' <code>SpringLayout.HORIZONTAL_CENTER</code>,
		   ''' <code>SpringLayout.VERTICAL_CENTER</code>,
		   ''' <code>SpringLayout.BASELINE</code>,
		   ''' <code>SpringLayout.WIDTH</code> or
		   ''' <code>SpringLayout.HEIGHT</code>.
		   ''' For any other <code>String</code> value passed as the edge,
		   ''' no action is taken. For a <code>null</code> edge, a
		   ''' <code>NullPointerException</code> is thrown.
		   ''' <p>
		   ''' <b>Note:</b> This method can affect {@code x} and {@code y} values
		   ''' previously set for this {@code Constraints}.
		   ''' </summary>
		   ''' <param name="edgeName"> the edge to be set </param>
		   ''' <param name="s"> the spring controlling the specified edge
		   ''' </param>
		   ''' <exception cref="NullPointerException"> if <code>edgeName</code> is <code>null</code>
		   ''' </exception>
		   ''' <seealso cref= #getConstraint </seealso>
		   ''' <seealso cref= #NORTH </seealso>
		   ''' <seealso cref= #SOUTH </seealso>
		   ''' <seealso cref= #EAST </seealso>
		   ''' <seealso cref= #WEST </seealso>
		   ''' <seealso cref= #HORIZONTAL_CENTER </seealso>
		   ''' <seealso cref= #VERTICAL_CENTER </seealso>
		   ''' <seealso cref= #BASELINE </seealso>
		   ''' <seealso cref= #WIDTH </seealso>
		   ''' <seealso cref= #HEIGHT </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Sub setConstraint(ByVal edgeName As String, ByVal s As Spring)
			   edgeName = edgeName.intern()
			   If edgeName = WEST Then
				   x = s
			   ElseIf edgeName = NORTH Then
				   y = s
			   ElseIf edgeName = EAST Then
				   east = s
			   ElseIf edgeName = SOUTH Then
				   south = s
			   ElseIf edgeName = HORIZONTAL_CENTER Then
				   horizontalCenter = s
			   ElseIf edgeName = WIDTH Then
				   width = s
			   ElseIf edgeName = HEIGHT Then
				   height = s
			   ElseIf edgeName = VERTICAL_CENTER Then
				   verticalCenter = s
			   ElseIf edgeName = BASELINE Then
				   baseline = s
			   End If
		   End Sub

		   ''' <summary>
		   ''' Returns the value of the specified edge, which may be
		   ''' a derived value, or even <code>null</code>.
		   ''' The edge must have one of the following values:
		   ''' <code>SpringLayout.NORTH</code>,
		   ''' <code>SpringLayout.SOUTH</code>,
		   ''' <code>SpringLayout.EAST</code>,
		   ''' <code>SpringLayout.WEST</code>,
		   ''' <code>SpringLayout.HORIZONTAL_CENTER</code>,
		   ''' <code>SpringLayout.VERTICAL_CENTER</code>,
		   ''' <code>SpringLayout.BASELINE</code>,
		   ''' <code>SpringLayout.WIDTH</code> or
		   ''' <code>SpringLayout.HEIGHT</code>.
		   ''' For any other <code>String</code> value passed as the edge,
		   ''' <code>null</code> will be returned. Throws
		   ''' <code>NullPointerException</code> for a <code>null</code> edge.
		   ''' </summary>
		   ''' <param name="edgeName"> the edge whose value
		   '''                 is to be returned
		   ''' </param>
		   ''' <returns> the spring controlling the specified edge, may be <code>null</code>
		   ''' </returns>
		   ''' <exception cref="NullPointerException"> if <code>edgeName</code> is <code>null</code>
		   ''' </exception>
		   ''' <seealso cref= #setConstraint </seealso>
		   ''' <seealso cref= #NORTH </seealso>
		   ''' <seealso cref= #SOUTH </seealso>
		   ''' <seealso cref= #EAST </seealso>
		   ''' <seealso cref= #WEST </seealso>
		   ''' <seealso cref= #HORIZONTAL_CENTER </seealso>
		   ''' <seealso cref= #VERTICAL_CENTER </seealso>
		   ''' <seealso cref= #BASELINE </seealso>
		   ''' <seealso cref= #WIDTH </seealso>
		   ''' <seealso cref= #HEIGHT </seealso>
		   ''' <seealso cref= SpringLayout.Constraints </seealso>
		   Public Overridable Function getConstraint(ByVal edgeName As String) As Spring
			   edgeName = edgeName.intern()
			   Return If(edgeName = WEST, x, If(edgeName = NORTH, y, If(edgeName = EAST, east, If(edgeName = SOUTH, south, If(edgeName = WIDTH, width, If(edgeName = HEIGHT, height, If(edgeName = HORIZONTAL_CENTER, horizontalCenter, If(edgeName = VERTICAL_CENTER, verticalCenter, If(edgeName = BASELINE, baseline, Nothing)))))))))
		   End Function

		   'pp
	 Friend Overridable Sub reset()
			   Dim allSprings As Spring() = {x, y, width, height, east, south, horizontalCenter, verticalCenter, baseline}
			   For Each s As Spring In allSprings
				   If s IsNot Nothing Then s.value = Spring.UNSET
			   Next s
	 End Sub
		End Class

	   Private Class SpringProxy
		   Inherits Spring

		   Private edgeName As String
		   Private c As java.awt.Component
		   Private l As SpringLayout

		   Public Sub New(ByVal edgeName As String, ByVal c As java.awt.Component, ByVal l As SpringLayout)
			   Me.edgeName = edgeName
			   Me.c = c
			   Me.l = l
		   End Sub

		   Private Property constraint As Spring
			   Get
				   Return l.getConstraints(c).getConstraint(edgeName)
			   End Get
		   End Property

		   Public Property Overrides minimumValue As Integer
			   Get
				   Return constraint.minimumValue
			   End Get
		   End Property

		   Public Property Overrides preferredValue As Integer
			   Get
				   Return constraint.preferredValue
			   End Get
		   End Property

		   Public Property Overrides maximumValue As Integer
			   Get
				   Return constraint.maximumValue
			   End Get
		   End Property

		   Public Property Overrides value As Integer
			   Get
				   Return constraint.value
			   End Get
			   Set(ByVal size As Integer)
				   constraint.value = size
			   End Set
		   End Property


		   'pp
	 Friend Overrides Function isCyclic(ByVal l As SpringLayout) As Boolean
			   Return l.isCyclic(constraint)
	 End Function

		   Public Overrides Function ToString() As String
			   Return "SpringProxy for " & edgeName & " edge of " & c.name & "."
		   End Function
	   End Class

		''' <summary>
		''' Constructs a new <code>SpringLayout</code>.
		''' </summary>
		Public Sub New()
		End Sub

		Private Sub resetCyclicStatuses()
			cyclicSprings = New HashSet(Of Spring)
			acyclicSprings = New HashSet(Of Spring)
		End Sub

		Private Property parent As java.awt.Container
			Set(ByVal p As java.awt.Container)
				resetCyclicStatuses()
				Dim pc As Constraints = getConstraints(p)
    
				pc.x = Spring.constant(0)
				pc.y = Spring.constant(0)
				' The applyDefaults() method automatically adds width and
				' height springs that delegate their calculations to the
				' getMinimumSize(), getPreferredSize() and getMaximumSize()
				' methods of the relevant component. In the case of the
				' parent this will cause an infinite loop since these
				' methods, in turn, delegate their calculations to the
				' layout manager. Check for this case and replace the
				' the springs that would cause this problem with a
				' constant springs that supply default values.
				Dim ___width As Spring = pc.width
				If TypeOf ___width Is Spring.WidthSpring AndAlso CType(___width, Spring.WidthSpring).c Is p Then pc.width = Spring.constant(0, 0, Integer.MaxValue)
				Dim ___height As Spring = pc.height
				If TypeOf ___height Is Spring.HeightSpring AndAlso CType(___height, Spring.HeightSpring).c Is p Then pc.height = Spring.constant(0, 0, Integer.MaxValue)
			End Set
		End Property

		'pp
	 Friend Overridable Function isCyclic(ByVal s As Spring) As Boolean
			If s Is Nothing Then Return False
			If cyclicSprings.contains(s) Then Return True
			If acyclicSprings.contains(s) Then Return False
			cyclicSprings.add(s)
			Dim result As Boolean = s.isCyclic(Me)
			If Not result Then
				acyclicSprings.add(s)
				cyclicSprings.remove(s)
			Else
				Console.Error.WriteLine(s & " is cyclic. ")
			End If
			Return result
	 End Function

		Private Function abandonCycles(ByVal s As Spring) As Spring
			Return If(isCyclic(s), cyclicReference, s)
		End Function

		' LayoutManager methods.

		''' <summary>
		''' Has no effect,
		''' since this layout manager does not
		''' use a per-component string.
		''' </summary>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As java.awt.Component)
		End Sub

		''' <summary>
		''' Removes the constraints associated with the specified component.
		''' </summary>
		''' <param name="c"> the component being removed from the container </param>
		Public Overridable Sub removeLayoutComponent(ByVal c As java.awt.Component)
			componentConstraints.Remove(c)
		End Sub

		Private Shared Function addInsets(ByVal width As Integer, ByVal height As Integer, ByVal p As java.awt.Container) As java.awt.Dimension
			Dim i As java.awt.Insets = p.insets
			Return New java.awt.Dimension(width + i.left + i.right, height + i.top + i.bottom)
		End Function

		Public Overridable Function minimumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			parent = parent
			Dim pc As Constraints = getConstraints(parent)
			Return addInsets(abandonCycles(pc.width).minimumValue, abandonCycles(pc.height).minimumValue, parent)
		End Function

		Public Overridable Function preferredLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			parent = parent
			Dim pc As Constraints = getConstraints(parent)
			Return addInsets(abandonCycles(pc.width).preferredValue, abandonCycles(pc.height).preferredValue, parent)
		End Function

		' LayoutManager2 methods.

		Public Overridable Function maximumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			parent = parent
			Dim pc As Constraints = getConstraints(parent)
			Return addInsets(abandonCycles(pc.width).maximumValue, abandonCycles(pc.height).maximumValue, parent)
		End Function

		''' <summary>
		''' If <code>constraints</code> is an instance of
		''' <code>SpringLayout.Constraints</code>,
		''' associates the constraints with the specified component.
		''' <p> </summary>
		''' <param name="component"> the component being added </param>
		''' <param name="constraints"> the component's constraints
		''' </param>
		''' <seealso cref= SpringLayout.Constraints </seealso>
		Public Overridable Sub addLayoutComponent(ByVal component As java.awt.Component, ByVal constraints As Object)
			If TypeOf constraints Is Constraints Then putConstraints(component, CType(constraints, Constraints))
		End Sub

		''' <summary>
		''' Returns 0.5f (centered).
		''' </summary>
		Public Overridable Function getLayoutAlignmentX(ByVal p As java.awt.Container) As Single
			Return 0.5f
		End Function

		''' <summary>
		''' Returns 0.5f (centered).
		''' </summary>
		Public Overridable Function getLayoutAlignmentY(ByVal p As java.awt.Container) As Single
			Return 0.5f
		End Function

		Public Overridable Sub invalidateLayout(ByVal p As java.awt.Container)
		End Sub

		' End of LayoutManger2 methods

	   ''' <summary>
	   ''' Links edge <code>e1</code> of component <code>c1</code> to
	   ''' edge <code>e2</code> of component <code>c2</code>,
	   ''' with a fixed distance between the edges. This
	   ''' constraint will cause the assignment
	   ''' <pre>
	   '''     value(e1, c1) = value(e2, c2) + pad</pre>
	   ''' to take place during all subsequent layout operations.
	   ''' <p> </summary>
	   ''' <param name="e1"> the edge of the dependent </param>
	   ''' <param name="c1"> the component of the dependent </param>
	   ''' <param name="pad"> the fixed distance between dependent and anchor </param>
	   ''' <param name="e2"> the edge of the anchor </param>
	   ''' <param name="c2"> the component of the anchor
	   ''' </param>
	   ''' <seealso cref= #putConstraint(String, Component, Spring, String, Component) </seealso>
		Public Overridable Sub putConstraint(ByVal e1 As String, ByVal c1 As java.awt.Component, ByVal pad As Integer, ByVal e2 As String, ByVal c2 As java.awt.Component)
			putConstraint(e1, c1, Spring.constant(pad), e2, c2)
		End Sub

		''' <summary>
		''' Links edge <code>e1</code> of component <code>c1</code> to
		''' edge <code>e2</code> of component <code>c2</code>. As edge
		''' <code>(e2, c2)</code> changes value, edge <code>(e1, c1)</code> will
		''' be calculated by taking the (spring) sum of <code>(e2, c2)</code>
		''' and <code>s</code>.
		''' Each edge must have one of the following values:
		''' <code>SpringLayout.NORTH</code>,
		''' <code>SpringLayout.SOUTH</code>,
		''' <code>SpringLayout.EAST</code>,
		''' <code>SpringLayout.WEST</code>,
		''' <code>SpringLayout.VERTICAL_CENTER</code>,
		''' <code>SpringLayout.HORIZONTAL_CENTER</code> or
		''' <code>SpringLayout.BASELINE</code>.
		''' <p> </summary>
		''' <param name="e1"> the edge of the dependent </param>
		''' <param name="c1"> the component of the dependent </param>
		''' <param name="s"> the spring linking dependent and anchor </param>
		''' <param name="e2"> the edge of the anchor </param>
		''' <param name="c2"> the component of the anchor
		''' </param>
		''' <seealso cref= #putConstraint(String, Component, int, String, Component) </seealso>
		''' <seealso cref= #NORTH </seealso>
		''' <seealso cref= #SOUTH </seealso>
		''' <seealso cref= #EAST </seealso>
		''' <seealso cref= #WEST </seealso>
		''' <seealso cref= #VERTICAL_CENTER </seealso>
		''' <seealso cref= #HORIZONTAL_CENTER </seealso>
		''' <seealso cref= #BASELINE </seealso>
		Public Overridable Sub putConstraint(ByVal e1 As String, ByVal c1 As java.awt.Component, ByVal s As Spring, ByVal e2 As String, ByVal c2 As java.awt.Component)
			putConstraint(e1, c1, Spring.sum(s, getConstraint(e2, c2)))
		End Sub

		Private Sub putConstraint(ByVal e As String, ByVal c As java.awt.Component, ByVal s As Spring)
			If s IsNot Nothing Then getConstraints(c).constraintint(e, s)
		End Sub

		Private Function applyDefaults(ByVal c As java.awt.Component, ByVal constraints As Constraints) As Constraints
			If constraints Is Nothing Then constraints = New Constraints
			If constraints.c Is Nothing Then constraints.c = c
			If constraints.horizontalHistory.size() < 2 Then applyDefaults(constraints, WEST, Spring.constant(0), WIDTH, Spring.width(c), constraints.horizontalHistory)
			If constraints.verticalHistory.size() < 2 Then applyDefaults(constraints, NORTH, Spring.constant(0), HEIGHT, Spring.height(c), constraints.verticalHistory)
			Return constraints
		End Function

		Private Sub applyDefaults(ByVal constraints As Constraints, ByVal name1 As String, ByVal spring1 As Spring, ByVal name2 As String, ByVal spring2 As Spring, ByVal history As IList(Of String))
			If history.Count = 0 Then
				constraints.constraintint(name1, spring1)
				constraints.constraintint(name2, spring2)
			Else
				' At this point there must be exactly one constraint defined already.
				' Check width/height first.
				If constraints.getConstraint(name2) Is Nothing Then
					constraints.constraintint(name2, spring2)
				Else
					' If width/height is already defined, install a default for x/y.
					constraints.constraintint(name1, spring1)
				End If
				' Either way, leave the user's constraint topmost on the stack.
				Collections.rotate(history, 1)
			End If
		End Sub

		Private Sub putConstraints(ByVal component As java.awt.Component, ByVal constraints As Constraints)
			componentConstraints(component) = applyDefaults(component, constraints)
		End Sub

		''' <summary>
		''' Returns the constraints for the specified component.
		''' Note that,
		''' unlike the <code>GridBagLayout</code>
		''' <code>getConstraints</code> method,
		''' this method does not clone constraints.
		''' If no constraints
		''' have been associated with this component,
		''' this method
		''' returns a default constraints object positioned at
		''' 0,0 relative to the parent's Insets and its width/height
		''' constrained to the minimum, maximum, and preferred sizes of the
		''' component. The size characteristics
		''' are not frozen at the time this method is called;
		''' instead this method returns a constraints object
		''' whose characteristics track the characteristics
		''' of the component as they change.
		''' </summary>
		''' <param name="c"> the component whose constraints will be returned
		''' </param>
		''' <returns>      the constraints for the specified component </returns>
		Public Overridable Function getConstraints(ByVal c As java.awt.Component) As Constraints
		   Dim result As Constraints = componentConstraints(c)
		   If result Is Nothing Then
			   If TypeOf c Is javax.swing.JComponent Then
					Dim cp As Object = CType(c, javax.swing.JComponent).getClientProperty(GetType(SpringLayout))
					If TypeOf cp Is Constraints Then Return applyDefaults(c, CType(cp, Constraints))
			   End If
				result = New Constraints
				putConstraints(c, result)
		   End If
		   Return result
		End Function

		''' <summary>
		''' Returns the spring controlling the distance between
		''' the specified edge of
		''' the component and the top or left edge of its parent. This
		''' method, instead of returning the current binding for the
		''' edge, returns a proxy that tracks the characteristics
		''' of the edge even if the edge is subsequently rebound.
		''' Proxies are intended to be used in builder environments
		''' where it is useful to allow the user to define the
		''' constraints for a layout in any order. Proxies do, however,
		''' provide the means to create cyclic dependencies amongst
		''' the constraints of a layout. Such cycles are detected
		''' internally by <code>SpringLayout</code> so that
		''' the layout operation always terminates.
		''' </summary>
		''' <param name="edgeName"> must be one of
		''' <code>SpringLayout.NORTH</code>,
		''' <code>SpringLayout.SOUTH</code>,
		''' <code>SpringLayout.EAST</code>,
		''' <code>SpringLayout.WEST</code>,
		''' <code>SpringLayout.VERTICAL_CENTER</code>,
		''' <code>SpringLayout.HORIZONTAL_CENTER</code> or
		''' <code>SpringLayout.BASELINE</code> </param>
		''' <param name="c"> the component whose edge spring is desired
		''' </param>
		''' <returns> a proxy for the spring controlling the distance between the
		'''         specified edge and the top or left edge of its parent
		''' </returns>
		''' <seealso cref= #NORTH </seealso>
		''' <seealso cref= #SOUTH </seealso>
		''' <seealso cref= #EAST </seealso>
		''' <seealso cref= #WEST </seealso>
		''' <seealso cref= #VERTICAL_CENTER </seealso>
		''' <seealso cref= #HORIZONTAL_CENTER </seealso>
		''' <seealso cref= #BASELINE </seealso>
		Public Overridable Function getConstraint(ByVal edgeName As String, ByVal c As java.awt.Component) As Spring
			' The interning here is unnecessary; it was added for efficiency.
			edgeName = edgeName.intern()
			Return New SpringProxy(edgeName, c, Me)
		End Function

		Public Overridable Sub layoutContainer(ByVal parent As java.awt.Container)
			parent = parent

			Dim n As Integer = parent.componentCount
			getConstraints(parent).reset()
			For i As Integer = 0 To n - 1
				getConstraints(parent.getComponent(i)).reset()
			Next i

			Dim insets As java.awt.Insets = parent.insets
			Dim pc As Constraints = getConstraints(parent)
			abandonCycles(pc.x).value = 0
			abandonCycles(pc.y).value = 0
			abandonCycles(pc.width).value = parent.width - insets.left - insets.right
			abandonCycles(pc.height).value = parent.height - insets.top - insets.bottom

			For i As Integer = 0 To n - 1
				Dim c As java.awt.Component = parent.getComponent(i)
				Dim cc As Constraints = getConstraints(c)
				Dim x As Integer = abandonCycles(cc.x).value
				Dim y As Integer = abandonCycles(cc.y).value
				Dim ___width As Integer = abandonCycles(cc.width).value
				Dim ___height As Integer = abandonCycles(cc.height).value
				c.boundsnds(insets.left + x, insets.top + y, ___width, ___height)
			Next i
		End Sub
	End Class

End Namespace