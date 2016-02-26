Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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


Namespace javax.swing


	''' <summary>
	''' A layout manager that allows multiple components to be laid out either
	''' vertically or horizontally. The components will not wrap so, for
	''' example, a vertical arrangement of components will stay vertically
	''' arranged when the frame is resized.
	''' <TABLE STYLE="FLOAT:RIGHT" BORDER="0" SUMMARY="layout">
	'''    <TR>
	'''      <TD ALIGN="CENTER">
	'''         <P STYLE="TEXT-ALIGN:CENTER"><IMG SRC="doc-files/BoxLayout-1.gif"
	'''          alt="The following text describes this graphic."
	'''          WIDTH="191" HEIGHT="201" STYLE="FLOAT:BOTTOM; BORDER:0">
	'''      </TD>
	'''    </TR>
	''' </TABLE>
	''' <p>
	''' Nesting multiple panels with different combinations of horizontal and
	''' vertical gives an effect similar to GridBagLayout, without the
	''' complexity. The diagram shows two panels arranged horizontally, each
	''' of which contains 3 components arranged vertically.
	''' 
	''' <p> The BoxLayout manager is constructed with an axis parameter that
	''' specifies the type of layout that will be done. There are four choices:
	''' 
	''' <blockquote><b><tt>X_AXIS</tt></b> - Components are laid out horizontally
	''' from left to right.</blockquote>
	''' 
	''' <blockquote><b><tt>Y_AXIS</tt></b> - Components are laid out vertically
	''' from top to bottom.</blockquote>
	''' 
	''' <blockquote><b><tt>LINE_AXIS</tt></b> - Components are laid out the way
	''' words are laid out in a line, based on the container's
	''' <tt>ComponentOrientation</tt> property. If the container's
	''' <tt>ComponentOrientation</tt> is horizontal then components are laid out
	''' horizontally, otherwise they are laid out vertically.  For horizontal
	''' orientations, if the container's <tt>ComponentOrientation</tt> is left to
	''' right then components are laid out left to right, otherwise they are laid
	''' out right to left. For vertical orientations components are always laid out
	''' from top to bottom.</blockquote>
	''' 
	''' <blockquote><b><tt>PAGE_AXIS</tt></b> - Components are laid out the way
	''' text lines are laid out on a page, based on the container's
	''' <tt>ComponentOrientation</tt> property. If the container's
	''' <tt>ComponentOrientation</tt> is horizontal then components are laid out
	''' vertically, otherwise they are laid out horizontally.  For horizontal
	''' orientations, if the container's <tt>ComponentOrientation</tt> is left to
	''' right then components are laid out left to right, otherwise they are laid
	''' out right to left.&nbsp; For vertical orientations components are always
	''' laid out from top to bottom.</blockquote>
	''' <p>
	''' For all directions, components are arranged in the same order as they were
	''' added to the container.
	''' <p>
	''' BoxLayout attempts to arrange components
	''' at their preferred widths (for horizontal layout)
	''' or heights (for vertical layout).
	''' For a horizontal layout,
	''' if not all the components are the same height,
	''' BoxLayout attempts to make all the components
	''' as high as the highest component.
	''' If that's not possible for a particular component,
	''' then BoxLayout aligns that component vertically,
	''' according to the component's Y alignment.
	''' By default, a component has a Y alignment of 0.5,
	''' which means that the vertical center of the component
	''' should have the same Y coordinate as
	''' the vertical centers of other components with 0.5 Y alignment.
	''' <p>
	''' Similarly, for a vertical layout,
	''' BoxLayout attempts to make all components in the column
	''' as wide as the widest component.
	''' If that fails, it aligns them horizontally
	''' according to their X alignments.  For <code>PAGE_AXIS</code> layout,
	''' horizontal alignment is done based on the leading edge of the component.
	''' In other words, an X alignment value of 0.0 means the left edge of a
	''' component if the container's <code>ComponentOrientation</code> is left to
	''' right and it means the right edge of the component otherwise.
	''' <p>
	''' Instead of using BoxLayout directly, many programs use the Box class.
	''' The Box class is a lightweight container that uses a BoxLayout.
	''' It also provides handy methods to help you use BoxLayout well.
	''' Adding components to multiple nested boxes is a powerful way to get
	''' the arrangement you want.
	''' <p>
	''' For further information and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/layout/box.html">How to Use BoxLayout</a>,
	''' a section in <em>The Java Tutorial.</em>
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
	''' <seealso cref= Box </seealso>
	''' <seealso cref= java.awt.ComponentOrientation </seealso>
	''' <seealso cref= JComponent#getAlignmentX </seealso>
	''' <seealso cref= JComponent#getAlignmentY
	''' 
	''' @author   Timothy Prinzing </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class BoxLayout
		Implements LayoutManager2

		''' <summary>
		''' Specifies that components should be laid out left to right.
		''' </summary>
		Public Const X_AXIS As Integer = 0

		''' <summary>
		''' Specifies that components should be laid out top to bottom.
		''' </summary>
		Public Const Y_AXIS As Integer = 1

		''' <summary>
		''' Specifies that components should be laid out in the direction of
		''' a line of text as determined by the target container's
		''' <code>ComponentOrientation</code> property.
		''' </summary>
		Public Const LINE_AXIS As Integer = 2

		''' <summary>
		''' Specifies that components should be laid out in the direction that
		''' lines flow across a page as determined by the target container's
		''' <code>ComponentOrientation</code> property.
		''' </summary>
		Public Const PAGE_AXIS As Integer = 3

		''' <summary>
		''' Creates a layout manager that will lay out components along the
		''' given axis.
		''' </summary>
		''' <param name="target">  the container that needs to be laid out </param>
		''' <param name="axis">  the axis to lay out components along. Can be one of:
		'''              <code>BoxLayout.X_AXIS</code>,
		'''              <code>BoxLayout.Y_AXIS</code>,
		'''              <code>BoxLayout.LINE_AXIS</code> or
		'''              <code>BoxLayout.PAGE_AXIS</code>
		''' </param>
		''' <exception cref="AWTError">  if the value of <code>axis</code> is invalid </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal target As Container, ByVal axis As Integer)
			If axis <> X_AXIS AndAlso axis <> Y_AXIS AndAlso axis <> LINE_AXIS AndAlso axis <> PAGE_AXIS Then Throw New AWTError("Invalid axis")
			Me.axis = axis
			Me.target = target
		End Sub

		''' <summary>
		''' Constructs a BoxLayout that
		''' produces debugging messages.
		''' </summary>
		''' <param name="target">  the container that needs to be laid out </param>
		''' <param name="axis">  the axis to lay out components along. Can be one of:
		'''              <code>BoxLayout.X_AXIS</code>,
		'''              <code>BoxLayout.Y_AXIS</code>,
		'''              <code>BoxLayout.LINE_AXIS</code> or
		'''              <code>BoxLayout.PAGE_AXIS</code>
		''' </param>
		''' <param name="dbg">  the stream to which debugging messages should be sent,
		'''   null if none </param>
		Friend Sub New(ByVal target As Container, ByVal axis As Integer, ByVal dbg As java.io.PrintStream)
			Me.New(target, axis)
			Me.dbg = dbg
		End Sub

		''' <summary>
		''' Returns the container that uses this layout manager.
		''' </summary>
		''' <returns> the container that uses this layout manager
		''' 
		''' @since 1.6 </returns>
		Public Property target As Container
			Get
				Return Me.target
			End Get
		End Property

		''' <summary>
		''' Returns the axis that was used to lay out components.
		''' Returns one of:
		''' <code>BoxLayout.X_AXIS</code>,
		''' <code>BoxLayout.Y_AXIS</code>,
		''' <code>BoxLayout.LINE_AXIS</code> or
		''' <code>BoxLayout.PAGE_AXIS</code>
		''' </summary>
		''' <returns> the axis that was used to lay out components
		''' 
		''' @since 1.6 </returns>
		Public Property axis As Integer
			Get
				Return Me.axis
			End Get
		End Property

		''' <summary>
		''' Indicates that a child has changed its layout related information,
		''' and thus any cached calculations should be flushed.
		''' <p>
		''' This method is called by AWT when the invalidate method is called
		''' on the Container.  Since the invalidate method may be called
		''' asynchronously to the event thread, this method may be called
		''' asynchronously.
		''' </summary>
		''' <param name="target">  the affected container
		''' </param>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub invalidateLayout(ByVal target As Container)
			checkContainer(target)
			xChildren = Nothing
			yChildren = Nothing
			xTotal = Nothing
			yTotal = Nothing
		End Sub

		''' <summary>
		''' Not used by this class.
		''' </summary>
		''' <param name="name"> the name of the component </param>
		''' <param name="comp"> the component </param>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Not used by this class.
		''' </summary>
		''' <param name="comp"> the component </param>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Not used by this class.
		''' </summary>
		''' <param name="comp"> the component </param>
		''' <param name="constraints"> constraints </param>
		Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Returns the preferred dimensions for this layout, given the components
		''' in the specified target container.
		''' </summary>
		''' <param name="target">  the container that needs to be laid out </param>
		''' <returns> the dimensions &gt;= 0 &amp;&amp; &lt;= Integer.MAX_VALUE </returns>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		''' <seealso cref= Container </seealso>
		''' <seealso cref= #minimumLayoutSize </seealso>
		''' <seealso cref= #maximumLayoutSize </seealso>
		Public Overridable Function preferredLayoutSize(ByVal target As Container) As Dimension
			Dim size As Dimension
			SyncLock Me
				checkContainer(target)
				checkRequests()
				size = New Dimension(xTotal.preferred, yTotal.preferred)
			End SyncLock

			Dim insets As Insets = target.insets
			size.width = CInt(Fix(Math.Min(CLng(Fix(size.width)) + CLng(Fix(insets.left)) + CLng(Fix(insets.right)), Integer.MaxValue)))
			size.height = CInt(Fix(Math.Min(CLng(Fix(size.height)) + CLng(Fix(insets.top)) + CLng(Fix(insets.bottom)), Integer.MaxValue)))
			Return size
		End Function

		''' <summary>
		''' Returns the minimum dimensions needed to lay out the components
		''' contained in the specified target container.
		''' </summary>
		''' <param name="target">  the container that needs to be laid out </param>
		''' <returns> the dimensions &gt;= 0 &amp;&amp; &lt;= Integer.MAX_VALUE </returns>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		''' <seealso cref= #preferredLayoutSize </seealso>
		''' <seealso cref= #maximumLayoutSize </seealso>
		Public Overridable Function minimumLayoutSize(ByVal target As Container) As Dimension
			Dim size As Dimension
			SyncLock Me
				checkContainer(target)
				checkRequests()
				size = New Dimension(xTotal.minimum, yTotal.minimum)
			End SyncLock

			Dim insets As Insets = target.insets
			size.width = CInt(Fix(Math.Min(CLng(Fix(size.width)) + CLng(Fix(insets.left)) + CLng(Fix(insets.right)), Integer.MaxValue)))
			size.height = CInt(Fix(Math.Min(CLng(Fix(size.height)) + CLng(Fix(insets.top)) + CLng(Fix(insets.bottom)), Integer.MaxValue)))
			Return size
		End Function

		''' <summary>
		''' Returns the maximum dimensions the target container can use
		''' to lay out the components it contains.
		''' </summary>
		''' <param name="target">  the container that needs to be laid out </param>
		''' <returns> the dimensions &gt;= 0 &amp;&amp; &lt;= Integer.MAX_VALUE </returns>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		''' <seealso cref= #preferredLayoutSize </seealso>
		''' <seealso cref= #minimumLayoutSize </seealso>
		Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
			Dim size As Dimension
			SyncLock Me
				checkContainer(target)
				checkRequests()
				size = New Dimension(xTotal.maximum, yTotal.maximum)
			End SyncLock

			Dim insets As Insets = target.insets
			size.width = CInt(Fix(Math.Min(CLng(Fix(size.width)) + CLng(Fix(insets.left)) + CLng(Fix(insets.right)), Integer.MaxValue)))
			size.height = CInt(Fix(Math.Min(CLng(Fix(size.height)) + CLng(Fix(insets.top)) + CLng(Fix(insets.bottom)), Integer.MaxValue)))
			Return size
		End Function

		''' <summary>
		''' Returns the alignment along the X axis for the container.
		''' If the box is horizontal, the default
		''' alignment will be returned. Otherwise, the alignment needed
		''' to place the children along the X axis will be returned.
		''' </summary>
		''' <param name="target">  the container </param>
		''' <returns> the alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f </returns>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
			checkContainer(target)
			checkRequests()
			Return xTotal.alignment
		End Function

		''' <summary>
		''' Returns the alignment along the Y axis for the container.
		''' If the box is vertical, the default
		''' alignment will be returned. Otherwise, the alignment needed
		''' to place the children along the Y axis will be returned.
		''' </summary>
		''' <param name="target">  the container </param>
		''' <returns> the alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f </returns>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
			checkContainer(target)
			checkRequests()
			Return yTotal.alignment
		End Function

		''' <summary>
		''' Called by the AWT <!-- XXX CHECK! --> when the specified container
		''' needs to be laid out.
		''' </summary>
		''' <param name="target">  the container to lay out
		''' </param>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      BoxLayout constructor </exception>
		Public Overridable Sub layoutContainer(ByVal target As Container)
			checkContainer(target)
			Dim nChildren As Integer = target.componentCount
			Dim xOffsets As Integer() = New Integer(nChildren - 1){}
			Dim xSpans As Integer() = New Integer(nChildren - 1){}
			Dim yOffsets As Integer() = New Integer(nChildren - 1){}
			Dim ySpans As Integer() = New Integer(nChildren - 1){}

			Dim alloc As Dimension = target.size
			Dim [in] As Insets = target.insets
			alloc.width -= [in].left + [in].right
			alloc.height -= [in].top + [in].bottom

			' Resolve axis to an absolute value (either X_AXIS or Y_AXIS)
			Dim o As ComponentOrientation = target.componentOrientation
			Dim absoluteAxis As Integer = resolveAxis(axis, o)
			Dim ltr As Boolean = If(absoluteAxis <> axis, o.leftToRight, True)


			' determine the child placements
			SyncLock Me
				checkRequests()

				If absoluteAxis = X_AXIS Then
					SizeRequirements.calculateTiledPositions(alloc.width, xTotal, xChildren, xOffsets, xSpans, ltr)
					SizeRequirements.calculateAlignedPositions(alloc.height, yTotal, yChildren, yOffsets, ySpans)
				Else
					SizeRequirements.calculateAlignedPositions(alloc.width, xTotal, xChildren, xOffsets, xSpans, ltr)
					SizeRequirements.calculateTiledPositions(alloc.height, yTotal, yChildren, yOffsets, ySpans)
				End If
			End SyncLock

			' flush changes to the container
			For i As Integer = 0 To nChildren - 1
				Dim c As Component = target.getComponent(i)
				c.boundsnds(CInt(Fix(Math.Min(CLng(Fix([in].left)) + CLng(xOffsets(i)), Integer.MaxValue))), CInt(Fix(Math.Min(CLng(Fix([in].top)) + CLng(yOffsets(i)), Integer.MaxValue))), xSpans(i), ySpans(i))

			Next i
			If dbg IsNot Nothing Then
				For i As Integer = 0 To nChildren - 1
					Dim c As Component = target.getComponent(i)
					dbg.println(c.ToString())
					dbg.println("X: " & xChildren(i))
					dbg.println("Y: " & yChildren(i))
				Next i
			End If

		End Sub

		Friend Overridable Sub checkContainer(ByVal target As Container)
			If Me.target IsNot target Then Throw New AWTError("BoxLayout can't be shared")
		End Sub

		Friend Overridable Sub checkRequests()
			If xChildren Is Nothing OrElse yChildren Is Nothing Then
				' The requests have been invalidated... recalculate
				' the request information.
				Dim n As Integer = target.componentCount
				xChildren = New SizeRequirements(n - 1){}
				yChildren = New SizeRequirements(n - 1){}
				For i As Integer = 0 To n - 1
					Dim c As Component = target.getComponent(i)
					If Not c.visible Then
						xChildren(i) = New SizeRequirements(0,0,0, c.alignmentX)
						yChildren(i) = New SizeRequirements(0,0,0, c.alignmentY)
						Continue For
					End If
					Dim min As Dimension = c.minimumSize
					Dim typ As Dimension = c.preferredSize
					Dim max As Dimension = c.maximumSize
					xChildren(i) = New SizeRequirements(min.width, typ.width, max.width, c.alignmentX)
					yChildren(i) = New SizeRequirements(min.height, typ.height, max.height, c.alignmentY)
				Next i

				' Resolve axis to an absolute value (either X_AXIS or Y_AXIS)
				Dim absoluteAxis As Integer = resolveAxis(axis,target.componentOrientation)

				If absoluteAxis = X_AXIS Then
					xTotal = SizeRequirements.getTiledSizeRequirements(xChildren)
					yTotal = SizeRequirements.getAlignedSizeRequirements(yChildren)
				Else
					xTotal = SizeRequirements.getAlignedSizeRequirements(xChildren)
					yTotal = SizeRequirements.getTiledSizeRequirements(yChildren)
				End If
			End If
		End Sub

		''' <summary>
		''' Given one of the 4 axis values, resolve it to an absolute axis.
		''' The relative axis values, PAGE_AXIS and LINE_AXIS are converted
		''' to their absolute couterpart given the target's ComponentOrientation
		''' value.  The absolute axes, X_AXIS and Y_AXIS are returned unmodified.
		''' </summary>
		''' <param name="axis"> the axis to resolve </param>
		''' <param name="o"> the ComponentOrientation to resolve against </param>
		''' <returns> the resolved axis </returns>
		Private Function resolveAxis(ByVal axis As Integer, ByVal o As ComponentOrientation) As Integer
			Dim absoluteAxis As Integer
			If axis = LINE_AXIS Then
				absoluteAxis = If(o.horizontal, X_AXIS, Y_AXIS)
			ElseIf axis = PAGE_AXIS Then
				absoluteAxis = If(o.horizontal, Y_AXIS, X_AXIS)
			Else
				absoluteAxis = axis
			End If
			Return absoluteAxis
		End Function


		Private axis As Integer
		Private target As Container

		<NonSerialized> _
		Private xChildren As SizeRequirements()
		<NonSerialized> _
		Private yChildren As SizeRequirements()
		<NonSerialized> _
		Private xTotal As SizeRequirements
		<NonSerialized> _
		Private yTotal As SizeRequirements

		<NonSerialized> _
		Private dbg As java.io.PrintStream
	End Class

End Namespace