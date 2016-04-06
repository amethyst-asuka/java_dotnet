'
' * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines an interface for classes that know how to layout Containers
	''' based on a layout constraints object.
	''' 
	''' This interface extends the LayoutManager interface to deal with layouts
	''' explicitly in terms of constraint objects that specify how and where
	''' components should be added to the layout.
	''' <p>
	''' This minimal extension to LayoutManager is intended for tool
	''' providers who wish to the creation of constraint-based layouts.
	''' It does not yet provide full, general support for custom
	''' constraint-based layout managers.
	''' </summary>
	''' <seealso cref= LayoutManager </seealso>
	''' <seealso cref= Container
	''' 
	''' @author      Jonni Kanerva </seealso>
	Public Interface LayoutManager2
		Inherits LayoutManager

		''' <summary>
		''' Adds the specified component to the layout, using the specified
		''' constraint object. </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints">  where/how the component is added to the layout. </param>
		Sub addLayoutComponent(  comp As Component,   constraints As Object)

		''' <summary>
		''' Calculates the maximum size dimensions for the specified container,
		''' given the components it contains. </summary>
		''' <seealso cref= java.awt.Component#getMaximumSize </seealso>
		''' <seealso cref= LayoutManager </seealso>
		Function maximumLayoutSize(  target As Container) As Dimension

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Function getLayoutAlignmentX(  target As Container) As Single

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		Function getLayoutAlignmentY(  target As Container) As Single

		''' <summary>
		''' Invalidates the layout, indicating that if the layout manager
		''' has cached information it should be discarded.
		''' </summary>
		Sub invalidateLayout(  target As Container)

	End Interface

End Namespace