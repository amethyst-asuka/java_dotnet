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
	''' The default layout manager for <code>JViewport</code>.
	''' <code>ViewportLayout</code> defines
	''' a policy for layout that should be useful for most applications.
	''' The viewport makes its view the same size as the viewport,
	''' however it will not make the view smaller than its minimum size.
	''' As the viewport grows the view is kept bottom justified until
	''' the entire view is visible, subsequently the view is kept top
	''' justified.
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
	''' @author Hans Muller
	''' </summary>
	<Serializable> _
	Public Class ViewportLayout
		Implements java.awt.LayoutManager

		' Single instance used by JViewport.
		Friend Shared SHARED_INSTANCE As New ViewportLayout

		''' <summary>
		''' Adds the specified component to the layout. Not used by this class. </summary>
		''' <param name="name"> the name of the component </param>
		''' <param name="c"> the the component to be added </param>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As java.awt.Component)
		End Sub

		''' <summary>
		''' Removes the specified component from the layout. Not used by
		''' this class. </summary>
		''' <param name="c"> the component to remove </param>
		Public Overridable Sub removeLayoutComponent(ByVal c As java.awt.Component)
		End Sub


		''' <summary>
		''' Returns the preferred dimensions for this layout given the components
		''' in the specified target container. </summary>
		''' <param name="parent"> the component which needs to be laid out </param>
		''' <returns> a <code>Dimension</code> object containing the
		'''          preferred dimensions </returns>
		''' <seealso cref= #minimumLayoutSize </seealso>
		Public Overridable Function preferredLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			Dim view As java.awt.Component = CType(parent, JViewport).view
			If view Is Nothing Then
				Return New java.awt.Dimension(0, 0)
			ElseIf TypeOf view Is Scrollable Then
				Return CType(view, Scrollable).preferredScrollableViewportSize
			Else
				Return view.preferredSize
			End If
		End Function


		''' <summary>
		''' Returns the minimum dimensions needed to layout the components
		''' contained in the specified target container.
		''' </summary>
		''' <param name="parent"> the component which needs to be laid out </param>
		''' <returns> a <code>Dimension</code> object containing the minimum
		'''          dimensions </returns>
		''' <seealso cref= #preferredLayoutSize </seealso>
		Public Overridable Function minimumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			Return New java.awt.Dimension(4, 4)
		End Function


		''' <summary>
		''' Called by the AWT when the specified container needs to be laid out.
		''' </summary>
		''' <param name="parent">  the container to lay out
		''' </param>
		''' <exception cref="AWTError"> if the target isn't the container specified to the
		'''                      <code>BoxLayout</code> constructor </exception>
		Public Overridable Sub layoutContainer(ByVal parent As java.awt.Container)
			Dim vp As JViewport = CType(parent, JViewport)
			Dim view As java.awt.Component = vp.view
			Dim scrollableView As Scrollable = Nothing

			If view Is Nothing Then
				Return
			ElseIf TypeOf view Is Scrollable Then
				scrollableView = CType(view, Scrollable)
			End If

	'         All of the dimensions below are in view coordinates, except
	'         * vpSize which we're converting.
	'         

			Dim insets As java.awt.Insets = vp.insets
			Dim viewPrefSize As java.awt.Dimension = view.preferredSize
			Dim vpSize As java.awt.Dimension = vp.size
			Dim extentSize As java.awt.Dimension = vp.toViewCoordinates(vpSize)
			Dim viewSize As New java.awt.Dimension(viewPrefSize)

			If scrollableView IsNot Nothing Then
				If scrollableView.scrollableTracksViewportWidth Then viewSize.width = vpSize.width
				If scrollableView.scrollableTracksViewportHeight Then viewSize.height = vpSize.height
			End If

			Dim viewPosition As java.awt.Point = vp.viewPosition

	'         If the new viewport size would leave empty space to the
	'         * right of the view, right justify the view or left justify
	'         * the view when the width of the view is smaller than the
	'         * container.
	'         
			If scrollableView Is Nothing OrElse vp.parent Is Nothing OrElse vp.parent.componentOrientation.leftToRight Then
				If (viewPosition.x + extentSize.width) > viewSize.width Then viewPosition.x = Math.Max(0, viewSize.width - extentSize.width)
			Else
				If extentSize.width > viewSize.width Then
					viewPosition.x = viewSize.width - extentSize.width
				Else
					viewPosition.x = Math.Max(0, Math.Min(viewSize.width - extentSize.width, viewPosition.x))
				End If
			End If

	'         If the new viewport size would leave empty space below the
	'         * view, bottom justify the view or top justify the view when
	'         * the height of the view is smaller than the container.
	'         
			If (viewPosition.y + extentSize.height) > viewSize.height Then viewPosition.y = Math.Max(0, viewSize.height - extentSize.height)

	'         If we haven't been advised about how the viewports size
	'         * should change wrt to the viewport, i.e. if the view isn't
	'         * an instance of Scrollable, then adjust the views size as follows.
	'         *
	'         * If the origin of the view is showing and the viewport is
	'         * bigger than the views preferred size, then make the view
	'         * the same size as the viewport.
	'         
			If scrollableView Is Nothing Then
				If (viewPosition.x = 0) AndAlso (vpSize.width > viewPrefSize.width) Then viewSize.width = vpSize.width
				If (viewPosition.y = 0) AndAlso (vpSize.height > viewPrefSize.height) Then viewSize.height = vpSize.height
			End If
			vp.viewPosition = viewPosition
			vp.viewSize = viewSize
		End Sub
	End Class

End Namespace