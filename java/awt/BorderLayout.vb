Imports System

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
	''' A border layout lays out a container, arranging and resizing
	''' its components to fit in five regions:
	''' north, south, east, west, and center.
	''' Each region may contain no more than one component, and
	''' is identified by a corresponding constant:
	''' <code>NORTH</code>, <code>SOUTH</code>, <code>EAST</code>,
	''' <code>WEST</code>, and <code>CENTER</code>.  When adding a
	''' component to a container with a border layout, use one of these
	''' five constants, for example:
	''' <pre>
	'''    Panel p = new Panel();
	'''    p.setLayout(new BorderLayout());
	'''    p.add(new Button("Okay"), BorderLayout.SOUTH);
	''' </pre>
	''' As a convenience, <code>BorderLayout</code> interprets the
	''' absence of a string specification the same as the constant
	''' <code>CENTER</code>:
	''' <pre>
	'''    Panel p2 = new Panel();
	'''    p2.setLayout(new BorderLayout());
	'''    p2.add(new TextArea());  // Same as p.add(new TextArea(), BorderLayout.CENTER);
	''' </pre>
	''' <p>
	''' In addition, <code>BorderLayout</code> supports the relative
	''' positioning constants, <code>PAGE_START</code>, <code>PAGE_END</code>,
	''' <code>LINE_START</code>, and <code>LINE_END</code>.
	''' In a container whose <code>ComponentOrientation</code> is set to
	''' <code>ComponentOrientation.LEFT_TO_RIGHT</code>, these constants map to
	''' <code>NORTH</code>, <code>SOUTH</code>, <code>WEST</code>, and
	''' <code>EAST</code>, respectively.
	''' <p>
	''' For compatibility with previous releases, <code>BorderLayout</code>
	''' also includes the relative positioning constants <code>BEFORE_FIRST_LINE</code>,
	''' <code>AFTER_LAST_LINE</code>, <code>BEFORE_LINE_BEGINS</code> and
	''' <code>AFTER_LINE_ENDS</code>.  These are equivalent to
	''' <code>PAGE_START</code>, <code>PAGE_END</code>, <code>LINE_START</code>
	''' and <code>LINE_END</code> respectively.  For
	''' consistency with the relative positioning constants used by other
	''' components, the latter constants are preferred.
	''' <p>
	''' Mixing both absolute and relative positioning constants can lead to
	''' unpredictable results.  If
	''' you use both types, the relative constants will take precedence.
	''' For example, if you add components using both the <code>NORTH</code>
	''' and <code>PAGE_START</code> constants in a container whose
	''' orientation is <code>LEFT_TO_RIGHT</code>, only the
	''' <code>PAGE_START</code> will be layed out.
	''' <p>
	''' NOTE: Currently (in the Java 2 platform v1.2),
	''' <code>BorderLayout</code> does not support vertical
	''' orientations.  The <code>isVertical</code> setting on the container's
	''' <code>ComponentOrientation</code> is not respected.
	''' <p>
	''' The components are laid out according to their
	''' preferred sizes and the constraints of the container's size.
	''' The <code>NORTH</code> and <code>SOUTH</code> components may
	''' be stretched horizontally; the <code>EAST</code> and
	''' <code>WEST</code> components may be stretched vertically;
	''' the <code>CENTER</code> component may stretch both horizontally
	''' and vertically to fill any space left over.
	''' <p>
	''' Here is an example of five buttons in an applet laid out using
	''' the <code>BorderLayout</code> layout manager:
	''' <p>
	''' <img src="doc-files/BorderLayout-1.gif"
	''' alt="Diagram of an applet demonstrating BorderLayout.
	'''      Each section of the BorderLayout contains a Button corresponding to its position in the layout, one of:
	'''      North, West, Center, East, or South."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' The code for this applet is as follows:
	''' 
	''' <hr><blockquote><pre>
	''' import java.awt.*;
	''' import java.applet.Applet;
	''' 
	''' public class buttonDir extends Applet {
	'''   public void init() {
	'''     setLayout(new BorderLayout());
	'''     add(new Button("North"), BorderLayout.NORTH);
	'''     add(new Button("South"), BorderLayout.SOUTH);
	'''     add(new Button("East"), BorderLayout.EAST);
	'''     add(new Button("West"), BorderLayout.WEST);
	'''     add(new Button("Center"), BorderLayout.CENTER);
	'''   }
	''' }
	''' </pre></blockquote><hr>
	''' <p>
	''' @author      Arthur van Hoff </summary>
	''' <seealso cref=         java.awt.Container#add(String, Component) </seealso>
	''' <seealso cref=         java.awt.ComponentOrientation
	''' @since       JDK1.0 </seealso>
	<Serializable> _
	Public Class BorderLayout
		Implements LayoutManager2

		''' <summary>
		''' Constructs a border layout with the horizontal gaps
		''' between components.
		''' The horizontal gap is specified by <code>hgap</code>.
		''' </summary>
		''' <seealso cref= #getHgap() </seealso>
		''' <seealso cref= #setHgap(int)
		''' 
		''' @serial </seealso>
			Friend hgap As Integer

		''' <summary>
		''' Constructs a border layout with the vertical gaps
		''' between components.
		''' The vertical gap is specified by <code>vgap</code>.
		''' </summary>
		''' <seealso cref= #getVgap() </seealso>
		''' <seealso cref= #setVgap(int)
		''' @serial </seealso>
			Friend vgap As Integer

		''' <summary>
		''' Constant to specify components location to be the
		'''      north portion of the border layout.
		''' @serial </summary>
		''' <seealso cref= #getChild(String, boolean) </seealso>
		''' <seealso cref= #addLayoutComponent </seealso>
		''' <seealso cref= #getLayoutAlignmentX </seealso>
		''' <seealso cref= #getLayoutAlignmentY </seealso>
		''' <seealso cref= #removeLayoutComponent </seealso>
			Friend north As Component
		 ''' <summary>
		 ''' Constant to specify components location to be the
		 '''      west portion of the border layout.
		 ''' @serial </summary>
		 ''' <seealso cref= #getChild(String, boolean) </seealso>
		 ''' <seealso cref= #addLayoutComponent </seealso>
		 ''' <seealso cref= #getLayoutAlignmentX </seealso>
		 ''' <seealso cref= #getLayoutAlignmentY </seealso>
		 ''' <seealso cref= #removeLayoutComponent </seealso>
			Friend west As Component
		''' <summary>
		''' Constant to specify components location to be the
		'''      east portion of the border layout.
		''' @serial </summary>
		''' <seealso cref= #getChild(String, boolean) </seealso>
		''' <seealso cref= #addLayoutComponent </seealso>
		''' <seealso cref= #getLayoutAlignmentX </seealso>
		''' <seealso cref= #getLayoutAlignmentY </seealso>
		''' <seealso cref= #removeLayoutComponent </seealso>
			Friend east As Component
		''' <summary>
		''' Constant to specify components location to be the
		'''      south portion of the border layout.
		''' @serial </summary>
		''' <seealso cref= #getChild(String, boolean) </seealso>
		''' <seealso cref= #addLayoutComponent </seealso>
		''' <seealso cref= #getLayoutAlignmentX </seealso>
		''' <seealso cref= #getLayoutAlignmentY </seealso>
		''' <seealso cref= #removeLayoutComponent </seealso>
		Friend south As Component
		''' <summary>
		''' Constant to specify components location to be the
		'''      center portion of the border layout.
		''' @serial </summary>
		''' <seealso cref= #getChild(String, boolean) </seealso>
		''' <seealso cref= #addLayoutComponent </seealso>
		''' <seealso cref= #getLayoutAlignmentX </seealso>
		''' <seealso cref= #getLayoutAlignmentY </seealso>
		''' <seealso cref= #removeLayoutComponent </seealso>
			Friend center As Component

		''' 
		''' <summary>
		''' A relative positioning constant, that can be used instead of
		''' north, south, east, west or center.
		''' mixing the two types of constants can lead to unpredictable results.  If
		''' you use both types, the relative constants will take precedence.
		''' For example, if you add components using both the <code>NORTH</code>
		''' and <code>BEFORE_FIRST_LINE</code> constants in a container whose
		''' orientation is <code>LEFT_TO_RIGHT</code>, only the
		''' <code>BEFORE_FIRST_LINE</code> will be layed out.
		''' This will be the same for lastLine, firstItem, lastItem.
		''' @serial
		''' </summary>
		Friend firstLine As Component
		 ''' <summary>
		 ''' A relative positioning constant, that can be used instead of
		 ''' north, south, east, west or center.
		 ''' Please read Description for firstLine.
		 ''' @serial
		 ''' </summary>
			Friend lastLine As Component
		 ''' <summary>
		 ''' A relative positioning constant, that can be used instead of
		 ''' north, south, east, west or center.
		 ''' Please read Description for firstLine.
		 ''' @serial
		 ''' </summary>
			Friend firstItem As Component
		''' <summary>
		''' A relative positioning constant, that can be used instead of
		''' north, south, east, west or center.
		''' Please read Description for firstLine.
		''' @serial
		''' </summary>
			Friend lastItem As Component

		''' <summary>
		''' The north layout constraint (top of container).
		''' </summary>
		Public Const NORTH As String = "North"

		''' <summary>
		''' The south layout constraint (bottom of container).
		''' </summary>
		Public Const SOUTH As String = "South"

		''' <summary>
		''' The east layout constraint (right side of container).
		''' </summary>
		Public Const EAST As String = "East"

		''' <summary>
		''' The west layout constraint (left side of container).
		''' </summary>
		Public Const WEST As String = "West"

		''' <summary>
		''' The center layout constraint (middle of container).
		''' </summary>
		Public Const CENTER As String = "Center"

		''' <summary>
		''' Synonym for PAGE_START.  Exists for compatibility with previous
		''' versions.  PAGE_START is preferred.
		''' </summary>
		''' <seealso cref= #PAGE_START
		''' @since 1.2 </seealso>
		Public Const BEFORE_FIRST_LINE As String = "First"

		''' <summary>
		''' Synonym for PAGE_END.  Exists for compatibility with previous
		''' versions.  PAGE_END is preferred.
		''' </summary>
		''' <seealso cref= #PAGE_END
		''' @since 1.2 </seealso>
		Public Const AFTER_LAST_LINE As String = "Last"

		''' <summary>
		''' Synonym for LINE_START.  Exists for compatibility with previous
		''' versions.  LINE_START is preferred.
		''' </summary>
		''' <seealso cref= #LINE_START
		''' @since 1.2 </seealso>
		Public Const BEFORE_LINE_BEGINS As String = "Before"

		''' <summary>
		''' Synonym for LINE_END.  Exists for compatibility with previous
		''' versions.  LINE_END is preferred.
		''' </summary>
		''' <seealso cref= #LINE_END
		''' @since 1.2 </seealso>
		Public Const AFTER_LINE_ENDS As String = "After"

		''' <summary>
		''' The component comes before the first line of the layout's content.
		''' For Western, left-to-right and top-to-bottom orientations, this is
		''' equivalent to NORTH.
		''' </summary>
		''' <seealso cref= java.awt.Component#getComponentOrientation
		''' @since 1.4 </seealso>
		Public Const PAGE_START As String = BEFORE_FIRST_LINE

		''' <summary>
		''' The component comes after the last line of the layout's content.
		''' For Western, left-to-right and top-to-bottom orientations, this is
		''' equivalent to SOUTH.
		''' </summary>
		''' <seealso cref= java.awt.Component#getComponentOrientation
		''' @since 1.4 </seealso>
		Public Const PAGE_END As String = AFTER_LAST_LINE

		''' <summary>
		''' The component goes at the beginning of the line direction for the
		''' layout. For Western, left-to-right and top-to-bottom orientations,
		''' this is equivalent to WEST.
		''' </summary>
		''' <seealso cref= java.awt.Component#getComponentOrientation
		''' @since 1.4 </seealso>
		Public Const LINE_START As String = BEFORE_LINE_BEGINS

		''' <summary>
		''' The component goes at the end of the line direction for the
		''' layout. For Western, left-to-right and top-to-bottom orientations,
		''' this is equivalent to EAST.
		''' </summary>
		''' <seealso cref= java.awt.Component#getComponentOrientation
		''' @since 1.4 </seealso>
		Public Const LINE_END As String = AFTER_LINE_ENDS

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -8658291919501921765L

		''' <summary>
		''' Constructs a new border layout with
		''' no gaps between components.
		''' </summary>
		Public Sub New()
			Me.New(0, 0)
		End Sub

		''' <summary>
		''' Constructs a border layout with the specified gaps
		''' between components.
		''' The horizontal gap is specified by <code>hgap</code>
		''' and the vertical gap is specified by <code>vgap</code>. </summary>
		''' <param name="hgap">   the horizontal gap. </param>
		''' <param name="vgap">   the vertical gap. </param>
		Public Sub New(ByVal hgap As Integer, ByVal vgap As Integer)
			Me.hgap = hgap
			Me.vgap = vgap
		End Sub

		''' <summary>
		''' Returns the horizontal gap between components.
		''' @since   JDK1.1
		''' </summary>
		Public Overridable Property hgap As Integer
			Get
				Return hgap
			End Get
			Set(ByVal hgap As Integer)
				Me.hgap = hgap
			End Set
		End Property


		''' <summary>
		''' Returns the vertical gap between components.
		''' @since   JDK1.1
		''' </summary>
		Public Overridable Property vgap As Integer
			Get
				Return vgap
			End Get
			Set(ByVal vgap As Integer)
				Me.vgap = vgap
			End Set
		End Property


		''' <summary>
		''' Adds the specified component to the layout, using the specified
		''' constraint object.  For border layouts, the constraint must be
		''' one of the following constants:  <code>NORTH</code>,
		''' <code>SOUTH</code>, <code>EAST</code>,
		''' <code>WEST</code>, or <code>CENTER</code>.
		''' <p>
		''' Most applications do not call this method directly. This method
		''' is called when a component is added to a container using the
		''' <code>Container.add</code> method with the same argument types. </summary>
		''' <param name="comp">         the component to be added. </param>
		''' <param name="constraints">  an object that specifies how and where
		'''                       the component is added to the layout. </param>
		''' <seealso cref=     java.awt.Container#add(java.awt.Component, java.lang.Object) </seealso>
		''' <exception cref="IllegalArgumentException">  if the constraint object is not
		'''                 a string, or if it not one of the five specified
		'''              constants.
		''' @since   JDK1.1 </exception>
		Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object) Implements LayoutManager2.addLayoutComponent
		  SyncLock comp.treeLock
			If (constraints Is Nothing) OrElse (TypeOf constraints Is String) Then
				addLayoutComponent(CStr(constraints), comp)
			Else
				Throw New IllegalArgumentException("cannot add to layout: constraint must be a string (or null)")
			End If
		  End SyncLock
		End Sub

		''' @deprecated  replaced by <code>addLayoutComponent(Component, Object)</code>. 
		<Obsolete(" replaced by <code>addLayoutComponent(Component, Object)</code>.")> _
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component) Implements LayoutManager.addLayoutComponent
		  SyncLock comp.treeLock
			' Special case:  treat null the same as "Center". 
			If name Is Nothing Then name = "Center"

	'         Assign the component to one of the known regions of the layout.
	'         
			If "Center".Equals(name) Then
				center = comp
			ElseIf "North".Equals(name) Then
				north = comp
			ElseIf "South".Equals(name) Then
				south = comp
			ElseIf "East".Equals(name) Then
				east = comp
			ElseIf "West".Equals(name) Then
				west = comp
			ElseIf BEFORE_FIRST_LINE.Equals(name) Then
				firstLine = comp
			ElseIf AFTER_LAST_LINE.Equals(name) Then
				lastLine = comp
			ElseIf BEFORE_LINE_BEGINS.Equals(name) Then
				firstItem = comp
			ElseIf AFTER_LINE_ENDS.Equals(name) Then
				lastItem = comp
			Else
				Throw New IllegalArgumentException("cannot add to layout: unknown constraint: " & name)
			End If
		  End SyncLock
		End Sub

		''' <summary>
		''' Removes the specified component from this border layout. This
		''' method is called when a container calls its <code>remove</code> or
		''' <code>removeAll</code> methods. Most applications do not call this
		''' method directly. </summary>
		''' <param name="comp">   the component to be removed. </param>
		''' <seealso cref=     java.awt.Container#remove(java.awt.Component) </seealso>
		''' <seealso cref=     java.awt.Container#removeAll() </seealso>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component) Implements LayoutManager.removeLayoutComponent
		  SyncLock comp.treeLock
			If comp Is center Then
				center = Nothing
			ElseIf comp Is north Then
				north = Nothing
			ElseIf comp Is south Then
				south = Nothing
			ElseIf comp Is east Then
				east = Nothing
			ElseIf comp Is west Then
				west = Nothing
			End If
			If comp Is firstLine Then
				firstLine = Nothing
			ElseIf comp Is lastLine Then
				lastLine = Nothing
			ElseIf comp Is firstItem Then
				firstItem = Nothing
			ElseIf comp Is lastItem Then
				lastItem = Nothing
			End If
		  End SyncLock
		End Sub

		''' <summary>
		''' Gets the component that was added using the given constraint
		''' </summary>
		''' <param name="constraints">  the desired constraint, one of <code>CENTER</code>,
		'''                       <code>NORTH</code>, <code>SOUTH</code>,
		'''                       <code>WEST</code>, <code>EAST</code>,
		'''                       <code>PAGE_START</code>, <code>PAGE_END</code>,
		'''                       <code>LINE_START</code>, <code>LINE_END</code> </param>
		''' <returns>  the component at the given location, or <code>null</code> if
		'''          the location is empty </returns>
		''' <exception cref="IllegalArgumentException">  if the constraint object is
		'''              not one of the nine specified constants </exception>
		''' <seealso cref=     #addLayoutComponent(java.awt.Component, java.lang.Object)
		''' @since 1.5 </seealso>
		Public Overridable Function getLayoutComponent(ByVal constraints As Object) As Component
			If CENTER.Equals(constraints) Then
				Return center
			ElseIf NORTH.Equals(constraints) Then
				Return north
			ElseIf SOUTH.Equals(constraints) Then
				Return south
			ElseIf WEST.Equals(constraints) Then
				Return west
			ElseIf EAST.Equals(constraints) Then
				Return east
			ElseIf PAGE_START.Equals(constraints) Then
				Return firstLine
			ElseIf PAGE_END.Equals(constraints) Then
				Return lastLine
			ElseIf LINE_START.Equals(constraints) Then
				Return firstItem
			ElseIf LINE_END.Equals(constraints) Then
				Return lastItem
			Else
				Throw New IllegalArgumentException("cannot get component: unknown constraint: " & constraints)
			End If
		End Function


		''' <summary>
		''' Returns the component that corresponds to the given constraint location
		''' based on the target <code>Container</code>'s component orientation.
		''' Components added with the relative constraints <code>PAGE_START</code>,
		''' <code>PAGE_END</code>, <code>LINE_START</code>, and <code>LINE_END</code>
		''' take precedence over components added with the explicit constraints
		''' <code>NORTH</code>, <code>SOUTH</code>, <code>WEST</code>, and <code>EAST</code>.
		''' The <code>Container</code>'s component orientation is used to determine the location of components
		''' added with <code>LINE_START</code> and <code>LINE_END</code>.
		''' </summary>
		''' <param name="constraints">     the desired absolute position, one of <code>CENTER</code>,
		'''                          <code>NORTH</code>, <code>SOUTH</code>,
		'''                          <code>EAST</code>, <code>WEST</code> </param>
		''' <param name="target">     the {@code Container} used to obtain
		'''                     the constraint location based on the target
		'''                     {@code Container}'s component orientation. </param>
		''' <returns>  the component at the given location, or <code>null</code> if
		'''          the location is empty </returns>
		''' <exception cref="IllegalArgumentException">  if the constraint object is
		'''              not one of the five specified constants </exception>
		''' <exception cref="NullPointerException">  if the target parameter is null </exception>
		''' <seealso cref=     #addLayoutComponent(java.awt.Component, java.lang.Object)
		''' @since 1.5 </seealso>
		Public Overridable Function getLayoutComponent(ByVal target As Container, ByVal constraints As Object) As Component
			Dim ltr As Boolean = target.componentOrientation.leftToRight
			Dim result As Component = Nothing

			If NORTH.Equals(constraints) Then
				result = If(firstLine IsNot Nothing, firstLine, north)
			ElseIf SOUTH.Equals(constraints) Then
				result = If(lastLine IsNot Nothing, lastLine, south)
			ElseIf WEST.Equals(constraints) Then
				result = If(ltr, firstItem, lastItem)
				If result Is Nothing Then result = west
			ElseIf EAST.Equals(constraints) Then
				result = If(ltr, lastItem, firstItem)
				If result Is Nothing Then result = east
			ElseIf CENTER.Equals(constraints) Then
				result = center
			Else
				Throw New IllegalArgumentException("cannot get component: invalid constraint: " & constraints)
			End If

			Return result
		End Function


		''' <summary>
		''' Gets the constraints for the specified component
		''' </summary>
		''' <param name="comp"> the component to be queried </param>
		''' <returns>  the constraint for the specified component,
		'''          or null if component is null or is not present
		'''          in this layout </returns>
		''' <seealso cref= #addLayoutComponent(java.awt.Component, java.lang.Object)
		''' @since 1.5 </seealso>
		Public Overridable Function getConstraints(ByVal comp As Component) As Object
			'fix for 6242148 : API method java.awt.BorderLayout.getConstraints(null) should return null
			If comp Is Nothing Then Return Nothing
			If comp Is center Then
				Return CENTER
			ElseIf comp Is north Then
				Return NORTH
			ElseIf comp Is south Then
				Return SOUTH
			ElseIf comp Is west Then
				Return WEST
			ElseIf comp Is east Then
				Return EAST
			ElseIf comp Is firstLine Then
				Return PAGE_START
			ElseIf comp Is lastLine Then
				Return PAGE_END
			ElseIf comp Is firstItem Then
				Return LINE_START
			ElseIf comp Is lastItem Then
				Return LINE_END
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Determines the minimum size of the <code>target</code> container
		''' using this layout manager.
		''' <p>
		''' This method is called when a container calls its
		''' <code>getMinimumSize</code> method. Most applications do not call
		''' this method directly. </summary>
		''' <param name="target">   the container in which to do the layout. </param>
		''' <returns>  the minimum dimensions needed to lay out the subcomponents
		'''          of the specified container. </returns>
		''' <seealso cref=     java.awt.Container </seealso>
		''' <seealso cref=     java.awt.BorderLayout#preferredLayoutSize </seealso>
		''' <seealso cref=     java.awt.Container#getMinimumSize() </seealso>
		Public Overridable Function minimumLayoutSize(ByVal target As Container) As Dimension Implements LayoutManager.minimumLayoutSize
		  SyncLock target.treeLock
			Dim [dim] As New Dimension(0, 0)

			Dim ltr As Boolean = target.componentOrientation.leftToRight
			Dim c As Component = Nothing

			c=getChild(EAST,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.minimumSize
				[dim].width += d.width + hgap
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(WEST,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.minimumSize
				[dim].width += d.width + hgap
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(CENTER,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.minimumSize
				[dim].width += d.width
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(NORTH,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.minimumSize
				[dim].width = Math.Max(d.width, [dim].width)
				[dim].height += d.height + vgap
			End If
			c=getChild(SOUTH,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.minimumSize
				[dim].width = Math.Max(d.width, [dim].width)
				[dim].height += d.height + vgap
			End If

			Dim insets_Renamed As Insets = target.insets
			[dim].width += insets_Renamed.left + insets_Renamed.right
			[dim].height += insets_Renamed.top + insets_Renamed.bottom

			Return [dim]
		  End SyncLock
		End Function

		''' <summary>
		''' Determines the preferred size of the <code>target</code>
		''' container using this layout manager, based on the components
		''' in the container.
		''' <p>
		''' Most applications do not call this method directly. This method
		''' is called when a container calls its <code>getPreferredSize</code>
		''' method. </summary>
		''' <param name="target">   the container in which to do the layout. </param>
		''' <returns>  the preferred dimensions to lay out the subcomponents
		'''          of the specified container. </returns>
		''' <seealso cref=     java.awt.Container </seealso>
		''' <seealso cref=     java.awt.BorderLayout#minimumLayoutSize </seealso>
		''' <seealso cref=     java.awt.Container#getPreferredSize() </seealso>
		Public Overridable Function preferredLayoutSize(ByVal target As Container) As Dimension Implements LayoutManager.preferredLayoutSize
		  SyncLock target.treeLock
			Dim [dim] As New Dimension(0, 0)

			Dim ltr As Boolean = target.componentOrientation.leftToRight
			Dim c As Component = Nothing

			c=getChild(EAST,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.preferredSize
				[dim].width += d.width + hgap
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(WEST,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.preferredSize
				[dim].width += d.width + hgap
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(CENTER,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.preferredSize
				[dim].width += d.width
				[dim].height = Math.Max(d.height, [dim].height)
			End If
			c=getChild(NORTH,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.preferredSize
				[dim].width = Math.Max(d.width, [dim].width)
				[dim].height += d.height + vgap
			End If
			c=getChild(SOUTH,ltr)
			If c IsNot Nothing Then
				Dim d As Dimension = c.preferredSize
				[dim].width = Math.Max(d.width, [dim].width)
				[dim].height += d.height + vgap
			End If

			Dim insets_Renamed As Insets = target.insets
			[dim].width += insets_Renamed.left + insets_Renamed.right
			[dim].height += insets_Renamed.top + insets_Renamed.bottom

			Return [dim]
		  End SyncLock
		End Function

		''' <summary>
		''' Returns the maximum dimensions for this layout given the components
		''' in the specified target container. </summary>
		''' <param name="target"> the component which needs to be laid out </param>
		''' <seealso cref= Container </seealso>
		''' <seealso cref= #minimumLayoutSize </seealso>
		''' <seealso cref= #preferredLayoutSize </seealso>
		Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension Implements LayoutManager2.maximumLayoutSize
			Return New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Function

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Overridable Function getLayoutAlignmentX(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentX
			Return 0.5f
		End Function

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Public Overridable Function getLayoutAlignmentY(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentY
			Return 0.5f
		End Function

		''' <summary>
		''' Invalidates the layout, indicating that if the layout manager
		''' has cached information it should be discarded.
		''' </summary>
		Public Overridable Sub invalidateLayout(ByVal target As Container) Implements LayoutManager2.invalidateLayout
		End Sub

		''' <summary>
		''' Lays out the container argument using this border layout.
		''' <p>
		''' This method actually reshapes the components in the specified
		''' container in order to satisfy the constraints of this
		''' <code>BorderLayout</code> object. The <code>NORTH</code>
		''' and <code>SOUTH</code> components, if any, are placed at
		''' the top and bottom of the container, respectively. The
		''' <code>WEST</code> and <code>EAST</code> components are
		''' then placed on the left and right, respectively. Finally,
		''' the <code>CENTER</code> object is placed in any remaining
		''' space in the middle.
		''' <p>
		''' Most applications do not call this method directly. This method
		''' is called when a container calls its <code>doLayout</code> method. </summary>
		''' <param name="target">   the container in which to do the layout. </param>
		''' <seealso cref=     java.awt.Container </seealso>
		''' <seealso cref=     java.awt.Container#doLayout() </seealso>
		Public Overridable Sub layoutContainer(ByVal target As Container) Implements LayoutManager.layoutContainer
		  SyncLock target.treeLock
			Dim insets_Renamed As Insets = target.insets
			Dim top As Integer = insets_Renamed.top
			Dim bottom As Integer = target.height - insets_Renamed.bottom
			Dim left As Integer = insets_Renamed.left
			Dim right As Integer = target.width - insets_Renamed.right

			Dim ltr As Boolean = target.componentOrientation.leftToRight
			Dim c As Component = Nothing

			c=getChild(NORTH,ltr)
			If c IsNot Nothing Then
				c.sizeize(right - left, c.height)
				Dim d As Dimension = c.preferredSize
				c.boundsnds(left, top, right - left, d.height)
				top += d.height + vgap
			End If
			c=getChild(SOUTH,ltr)
			If c IsNot Nothing Then
				c.sizeize(right - left, c.height)
				Dim d As Dimension = c.preferredSize
				c.boundsnds(left, bottom - d.height, right - left, d.height)
				bottom -= d.height + vgap
			End If
			c=getChild(EAST,ltr)
			If c IsNot Nothing Then
				c.sizeize(c.width, bottom - top)
				Dim d As Dimension = c.preferredSize
				c.boundsnds(right - d.width, top, d.width, bottom - top)
				right -= d.width + hgap
			End If
			c=getChild(WEST,ltr)
			If c IsNot Nothing Then
				c.sizeize(c.width, bottom - top)
				Dim d As Dimension = c.preferredSize
				c.boundsnds(left, top, d.width, bottom - top)
				left += d.width + hgap
			End If
			c=getChild(CENTER,ltr)
			If c IsNot Nothing Then c.boundsnds(left, top, right - left, bottom - top)
		  End SyncLock
		End Sub

		''' <summary>
		''' Get the component that corresponds to the given constraint location
		''' </summary>
		''' <param name="key">     The desired absolute position,
		'''                  either NORTH, SOUTH, EAST, or WEST. </param>
		''' <param name="ltr">     Is the component line direction left-to-right? </param>
		Private Function getChild(ByVal key As String, ByVal ltr As Boolean) As Component
			Dim result As Component = Nothing

			If key = NORTH Then
				result = If(firstLine IsNot Nothing, firstLine, north)
			ElseIf key = SOUTH Then
				result = If(lastLine IsNot Nothing, lastLine, south)
			ElseIf key = WEST Then
				result = If(ltr, firstItem, lastItem)
				If result Is Nothing Then result = west
			ElseIf key = EAST Then
				result = If(ltr, lastItem, firstItem)
				If result Is Nothing Then result = east
			ElseIf key = CENTER Then
				result = center
			End If
			If result IsNot Nothing AndAlso (Not result.visible) Then result = Nothing
			Return result
		End Function

		''' <summary>
		''' Returns a string representation of the state of this border layout. </summary>
		''' <returns>    a string representation of this border layout. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[hgap=" & hgap & ",vgap=" & vgap & "]"
		End Function
	End Class

End Namespace