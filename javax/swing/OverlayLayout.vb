Imports System

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
	''' A layout manager to arrange components over the top
	''' of each other.  The requested size of the container
	''' will be the largest requested size of the children,
	''' taking alignment needs into consideration.
	''' 
	''' The alignment is based upon what is needed to properly
	''' fit the children in the allocation area.  The children
	''' will be placed such that their alignment points are all
	''' on top of each other.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author   Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public Class OverlayLayout
		Implements LayoutManager2

		''' <summary>
		''' Constructs a layout manager that performs overlay
		''' arrangement of the children.  The layout manager
		''' created is dedicated to the given container.
		''' </summary>
		''' <param name="target">  the container to do layout against </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal target As Container)
			Me.target = target
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
		''' Indicates a child has changed its layout related information,
		''' which causes any cached calculations to be flushed.
		''' </summary>
		''' <param name="target"> the container </param>
		Public Overridable Sub invalidateLayout(ByVal target As Container)
			checkContainer(target)
			xChildren = Nothing
			yChildren = Nothing
			xTotal = Nothing
			yTotal = Nothing
		End Sub

		''' <summary>
		''' Adds the specified component to the layout. Used by
		''' this class to know when to invalidate layout.
		''' </summary>
		''' <param name="name"> the name of the component </param>
		''' <param name="comp"> the the component to be added </param>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Removes the specified component from the layout. Used by
		''' this class to know when to invalidate layout.
		''' </summary>
		''' <param name="comp"> the component to remove </param>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Adds the specified component to the layout, using the specified
		''' constraint object. Used by this class to know when to invalidate
		''' layout.
		''' </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints">  where/how the component is added to the layout. </param>
		Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object)
			invalidateLayout(comp.parent)
		End Sub

		''' <summary>
		''' Returns the preferred dimensions for this layout given the components
		''' in the specified target container.  Recomputes the layout if it
		''' has been invalidated.  Factors in the current inset setting returned
		''' by getInsets().
		''' </summary>
		''' <param name="target"> the component which needs to be laid out </param>
		''' <returns> a Dimension object containing the preferred dimensions </returns>
		''' <seealso cref= #minimumLayoutSize </seealso>
		Public Overridable Function preferredLayoutSize(ByVal target As Container) As Dimension
			checkContainer(target)
			checkRequests()

			Dim size As New Dimension(xTotal.preferred, yTotal.preferred)
			Dim insets As Insets = target.insets
			size.width += insets.left + insets.right
			size.height += insets.top + insets.bottom
			Return size
		End Function

		''' <summary>
		''' Returns the minimum dimensions needed to lay out the components
		''' contained in the specified target container.  Recomputes the layout
		''' if it has been invalidated, and factors in the current inset setting.
		''' </summary>
		''' <param name="target"> the component which needs to be laid out </param>
		''' <returns> a Dimension object containing the minimum dimensions </returns>
		''' <seealso cref= #preferredLayoutSize </seealso>
		Public Overridable Function minimumLayoutSize(ByVal target As Container) As Dimension
			checkContainer(target)
			checkRequests()

			Dim size As New Dimension(xTotal.minimum, yTotal.minimum)
			Dim insets As Insets = target.insets
			size.width += insets.left + insets.right
			size.height += insets.top + insets.bottom
			Return size
		End Function

		''' <summary>
		''' Returns the maximum dimensions needed to lay out the components
		''' contained in the specified target container.  Recomputes the
		''' layout if it has been invalidated, and factors in the inset setting
		''' returned by <code>getInset</code>.
		''' </summary>
		''' <param name="target"> the component that needs to be laid out </param>
		''' <returns> a <code>Dimension</code> object containing the maximum
		'''         dimensions </returns>
		''' <seealso cref= #preferredLayoutSize </seealso>
		Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
			checkContainer(target)
			checkRequests()

			Dim size As New Dimension(xTotal.maximum, yTotal.maximum)
			Dim insets As Insets = target.insets
			size.width += insets.left + insets.right
			size.height += insets.top + insets.bottom
			Return size
		End Function

		''' <summary>
		''' Returns the alignment along the x axis for the container.
		''' </summary>
		''' <param name="target"> the container </param>
		''' <returns> the alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f </returns>
		Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
			checkContainer(target)
			checkRequests()
			Return xTotal.alignment
		End Function

		''' <summary>
		''' Returns the alignment along the y axis for the container.
		''' </summary>
		''' <param name="target"> the container </param>
		''' <returns> the alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f </returns>
		Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
			checkContainer(target)
			checkRequests()
			Return yTotal.alignment
		End Function

		''' <summary>
		''' Called by the AWT when the specified container needs to be laid out.
		''' </summary>
		''' <param name="target">  the container to lay out
		''' </param>
		''' <exception cref="AWTError">  if the target isn't the container specified to the
		'''                      constructor </exception>
		Public Overridable Sub layoutContainer(ByVal target As Container)
			checkContainer(target)
			checkRequests()

			Dim nChildren As Integer = target.componentCount
			Dim xOffsets As Integer() = New Integer(nChildren - 1){}
			Dim xSpans As Integer() = New Integer(nChildren - 1){}
			Dim yOffsets As Integer() = New Integer(nChildren - 1){}
			Dim ySpans As Integer() = New Integer(nChildren - 1){}

			' determine the child placements
			Dim alloc As Dimension = target.size
			Dim [in] As Insets = target.insets
			alloc.width -= [in].left + [in].right
			alloc.height -= [in].top + [in].bottom
			SizeRequirements.calculateAlignedPositions(alloc.width, xTotal, xChildren, xOffsets, xSpans)
			SizeRequirements.calculateAlignedPositions(alloc.height, yTotal, yChildren, yOffsets, ySpans)

			' flush changes to the container
			For i As Integer = 0 To nChildren - 1
				Dim c As Component = target.getComponent(i)
				c.boundsnds([in].left + xOffsets(i), [in].top + yOffsets(i), xSpans(i), ySpans(i))
			Next i
		End Sub

		Friend Overridable Sub checkContainer(ByVal target As Container)
			If Me.target IsNot target Then Throw New AWTError("OverlayLayout can't be shared")
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
					Dim min As Dimension = c.minimumSize
					Dim typ As Dimension = c.preferredSize
					Dim max As Dimension = c.maximumSize
					xChildren(i) = New SizeRequirements(min.width, typ.width, max.width, c.alignmentX)
					yChildren(i) = New SizeRequirements(min.height, typ.height, max.height, c.alignmentY)
				Next i

				xTotal = SizeRequirements.getAlignedSizeRequirements(xChildren)
				yTotal = SizeRequirements.getAlignedSizeRequirements(yChildren)
			End If
		End Sub

		Private target As Container
		Private xChildren As SizeRequirements()
		Private yChildren As SizeRequirements()
		Private xTotal As SizeRequirements
		Private yTotal As SizeRequirements

	End Class

End Namespace