Imports Microsoft.VisualBasic
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
Namespace javax.swing.text


	''' <summary>
	''' A view that arranges its children into a box shape by tiling
	''' its children along an axis.  The box is somewhat like that
	''' found in TeX where there is alignment of the
	''' children, flexibility of the children is considered, etc.
	''' This is a building block that might be useful to represent
	''' things like a collection of lines, paragraphs,
	''' lists, columns, pages, etc.  The axis along which the children are tiled is
	''' considered the major axis.  The orthogonal axis is the minor axis.
	''' <p>
	''' Layout for each axis is handled separately by the methods
	''' <code>layoutMajorAxis</code> and <code>layoutMinorAxis</code>.
	''' Subclasses can change the layout algorithm by
	''' reimplementing these methods.    These methods will be called
	''' as necessary depending upon whether or not there is cached
	''' layout information and the cache is considered
	''' valid.  These methods are typically called if the given size
	''' along the axis changes, or if <code>layoutChanged</code> is
	''' called to force an updated layout.  The <code>layoutChanged</code>
	''' method invalidates cached layout information, if there is any.
	''' The requirements published to the parent view are calculated by
	''' the methods <code>calculateMajorAxisRequirements</code>
	''' and  <code>calculateMinorAxisRequirements</code>.
	''' If the layout algorithm is changed, these methods will
	''' likely need to be reimplemented.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BoxView
		Inherits CompositeView

		''' <summary>
		''' Constructs a <code>BoxView</code>.
		''' </summary>
		''' <param name="elem"> the element this view is responsible for </param>
		''' <param name="axis"> either <code>View.X_AXIS</code> or <code>View.Y_AXIS</code> </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem)
			tempRect = New Rectangle
			Me.majorAxis = axis

			majorOffsets = New Integer(){}
			majorSpans = New Integer(){}
			majorReqValid = False
			majorAllocValid = False
			minorOffsets = New Integer(){}
			minorSpans = New Integer(){}
			minorReqValid = False
			minorAllocValid = False
		End Sub

		''' <summary>
		''' Fetches the tile axis property.  This is the axis along which
		''' the child views are tiled.
		''' </summary>
		''' <returns> the major axis of the box, either
		'''  <code>View.X_AXIS</code> or <code>View.Y_AXIS</code>
		''' 
		''' @since 1.3 </returns>
		Public Overridable Property axis As Integer
			Get
				Return majorAxis
			End Get
			Set(ByVal axis As Integer)
				Dim axisChanged As Boolean = (axis <> majorAxis)
				majorAxis = axis
				If axisChanged Then preferenceChanged(Nothing, True, True)
			End Set
		End Property


		''' <summary>
		''' Invalidates the layout along an axis.  This happens
		''' automatically if the preferences have changed for
		''' any of the child views.  In some cases the layout
		''' may need to be recalculated when the preferences
		''' have not changed.  The layout can be marked as
		''' invalid by calling this method.  The layout will
		''' be updated the next time the <code>setSize</code> method
		''' is called on this view (typically in paint).
		''' </summary>
		''' <param name="axis"> either <code>View.X_AXIS</code> or <code>View.Y_AXIS</code>
		''' 
		''' @since 1.3 </param>
		Public Overridable Sub layoutChanged(ByVal axis As Integer)
			If axis = majorAxis Then
				majorAllocValid = False
			Else
				minorAllocValid = False
			End If
		End Sub

		''' <summary>
		''' Determines if the layout is valid along the given axis.
		''' </summary>
		''' <param name="axis"> either <code>View.X_AXIS</code> or <code>View.Y_AXIS</code>
		''' 
		''' @since 1.4 </param>
		Protected Friend Overridable Function isLayoutValid(ByVal axis As Integer) As Boolean
			If axis = majorAxis Then
				Return majorAllocValid
			Else
				Return minorAllocValid
			End If
		End Function

		''' <summary>
		''' Paints a child.  By default
		''' that is all it does, but a subclass can use this to paint
		''' things relative to the child.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="alloc"> the allocated region to paint into </param>
		''' <param name="index"> the child index, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		Protected Friend Overridable Sub paintChild(ByVal g As Graphics, ByVal alloc As Rectangle, ByVal index As Integer)
			Dim child As View = getView(index)
			child.paint(g, alloc)
		End Sub

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Invalidates the layout and resizes the cache of
		''' requests/allocations.  The child allocations can still
		''' be accessed for the old layout, but the new children
		''' will have an offset and span of 0.
		''' </summary>
		''' <param name="index"> the starting index into the child views to insert
		'''   the new views; this should be a value &gt;= 0 and &lt;= getViewCount </param>
		''' <param name="length"> the number of existing child views to remove;
		'''   This should be a value &gt;= 0 and &lt;= (getViewCount() - offset) </param>
		''' <param name="elems"> the child views to add; this value can be
		'''   <code>null</code>to indicate no children are being added
		'''   (useful to remove) </param>
		Public Overrides Sub replace(ByVal index As Integer, ByVal length As Integer, ByVal elems As View())
			MyBase.replace(index, length, elems)

			' invalidate cache
			Dim nInserted As Integer = If(elems IsNot Nothing, elems.Length, 0)
			majorOffsets = updateLayoutArray(majorOffsets, index, nInserted)
			majorSpans = updateLayoutArray(majorSpans, index, nInserted)
			majorReqValid = False
			majorAllocValid = False
			minorOffsets = updateLayoutArray(minorOffsets, index, nInserted)
			minorSpans = updateLayoutArray(minorSpans, index, nInserted)
			minorReqValid = False
			minorAllocValid = False
		End Sub

		''' <summary>
		''' Resizes the given layout array to match the new number of
		''' child views.  The current number of child views are used to
		''' produce the new array.  The contents of the old array are
		''' inserted into the new array at the appropriate places so that
		''' the old layout information is transferred to the new array.
		''' </summary>
		''' <param name="oldArray"> the original layout array </param>
		''' <param name="offset"> location where new views will be inserted </param>
		''' <param name="nInserted"> the number of child views being inserted;
		'''          therefore the number of blank spaces to leave in the
		'''          new array at location <code>offset</code> </param>
		''' <returns> the new layout array </returns>
		Friend Overridable Function updateLayoutArray(ByVal oldArray As Integer(), ByVal offset As Integer, ByVal nInserted As Integer) As Integer()
			Dim n As Integer = viewCount
			Dim newArray As Integer() = New Integer(n - 1){}

			Array.Copy(oldArray, 0, newArray, 0, offset)
			Array.Copy(oldArray, offset, newArray, offset + nInserted, n - nInserted - offset)
			Return newArray
		End Function

		''' <summary>
		''' Forwards the given <code>DocumentEvent</code> to the child views
		''' that need to be notified of the change to the model.
		''' If a child changed its requirements and the allocation
		''' was valid prior to forwarding the portion of the box
		''' from the starting child to the end of the box will
		''' be repainted.
		''' </summary>
		''' <param name="ec"> changes to the element this view is responsible
		'''  for (may be <code>null</code> if there were no changes) </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= #insertUpdate </seealso>
		''' <seealso cref= #removeUpdate </seealso>
		''' <seealso cref= #changedUpdate
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub forwardUpdate(ByVal ec As javax.swing.event.DocumentEvent.ElementChange, ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			Dim wasValid As Boolean = isLayoutValid(majorAxis)
			MyBase.forwardUpdate(ec, e, a, f)

			' determine if a repaint is needed
			If wasValid AndAlso ((Not isLayoutValid(majorAxis))) Then
				' Repaint is needed because one of the tiled children
				' have changed their span along the major axis.  If there
				' is a hosting component and an allocated shape we repaint.
				Dim c As Component = container
				If (a IsNot Nothing) AndAlso (c IsNot Nothing) Then
					Dim pos As Integer = e.offset
					Dim index As Integer = getViewIndexAtPosition(pos)
					Dim alloc As Rectangle = getInsideAllocation(a)
					If majorAxis = X_AXIS Then
						alloc.x += majorOffsets(index)
						alloc.width -= majorOffsets(index)
					Else
						alloc.y += minorOffsets(index)
						alloc.height -= minorOffsets(index)
					End If
					c.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				End If
			End If
		End Sub

		''' <summary>
		''' This is called by a child to indicate its
		''' preferred span has changed.  This is implemented to
		''' throw away cached layout information so that new
		''' calculations will be done the next time the children
		''' need an allocation.
		''' </summary>
		''' <param name="child"> the child view </param>
		''' <param name="width"> true if the width preference should change </param>
		''' <param name="height"> true if the height preference should change </param>
		Public Overrides Sub preferenceChanged(ByVal child As View, ByVal width As Boolean, ByVal height As Boolean)
			Dim majorChanged As Boolean = If(majorAxis = X_AXIS, width, height)
			Dim minorChanged As Boolean = If(majorAxis = X_AXIS, height, width)
			If majorChanged Then
				majorReqValid = False
				majorAllocValid = False
			End If
			If minorChanged Then
				minorReqValid = False
				minorAllocValid = False
			End If
			MyBase.preferenceChanged(child, width, height)
		End Sub

		''' <summary>
		''' Gets the resize weight.  A value of 0 or less is not resizable.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns> the weight </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getResizeWeight(ByVal axis As Integer) As Integer
			checkRequests(axis)
			If axis = majorAxis Then
				If (majorRequest.preferred <> majorRequest.minimum) OrElse (majorRequest.preferred <> majorRequest.maximum) Then Return 1
			Else
				If (minorRequest.preferred <> minorRequest.minimum) OrElse (minorRequest.preferred <> minorRequest.maximum) Then Return 1
			End If
			Return 0
		End Function

		''' <summary>
		''' Sets the size of the view along an axis.  This should cause
		''' layout of the view along the given axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <param name="span"> the span to layout to >= 0 </param>
		Friend Overridable Sub setSpanOnAxis(ByVal axis As Integer, ByVal span As Single)
			If axis = majorAxis Then
				If majorSpan <> CInt(Fix(span)) Then majorAllocValid = False
				If Not majorAllocValid Then
					' layout the major axis
					majorSpan = CInt(Fix(span))
					checkRequests(majorAxis)
					layoutMajorAxis(majorSpan, axis, majorOffsets, majorSpans)
					majorAllocValid = True

					' flush changes to the children
					updateChildSizes()
				End If
			Else
				If (CInt(Fix(span))) <> minorSpan Then minorAllocValid = False
				If Not minorAllocValid Then
					' layout the minor axis
					minorSpan = CInt(Fix(span))
					checkRequests(axis)
					layoutMinorAxis(minorSpan, axis, minorOffsets, minorSpans)
					minorAllocValid = True

					' flush changes to the children
					updateChildSizes()
				End If
			End If
		End Sub

		''' <summary>
		''' Propagates the current allocations to the child views.
		''' </summary>
		Friend Overridable Sub updateChildSizes()
			Dim n As Integer = viewCount
			If majorAxis = X_AXIS Then
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					v.sizeize(CSng(majorSpans(i)), CSng(minorSpans(i)))
				Next i
			Else
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					v.sizeize(CSng(minorSpans(i)), CSng(majorSpans(i)))
				Next i
			End If
		End Sub

		''' <summary>
		''' Returns the size of the view along an axis.  This is implemented
		''' to return zero.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns> the current span of the view along the given axis, >= 0 </returns>
		Friend Overridable Function getSpanOnAxis(ByVal axis As Integer) As Single
			If axis = majorAxis Then
				Return majorSpan
			Else
				Return minorSpan
			End If
		End Function

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view if the view caches any layout
		''' information.  This is implemented to call the
		''' layout method with the sizes inside of the insets.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
			layout(Math.Max(0, CInt(Fix(width - leftInset - rightInset))), Math.Max(0, CInt(Fix(height - topInset - bottomInset))))
		End Sub

		''' <summary>
		''' Renders the <code>BoxView</code> using the given
		''' rendering surface and area
		''' on that surface.  Only the children that intersect
		''' the clip bounds of the given <code>Graphics</code>
		''' will be rendered.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			Dim alloc As Rectangle = If(TypeOf allocation Is Rectangle, CType(allocation, Rectangle), allocation.bounds)
			Dim n As Integer = viewCount
			Dim x As Integer = alloc.x + leftInset
			Dim y As Integer = alloc.y + topInset
			Dim clip As Rectangle = g.clipBounds
			For i As Integer = 0 To n - 1
				tempRect.x = x + getOffset(X_AXIS, i)
				tempRect.y = y + getOffset(Y_AXIS, i)
				tempRect.width = getSpan(X_AXIS, i)
				tempRect.height = getSpan(Y_AXIS, i)
				Dim trx0 As Integer = tempRect.x, trx1 As Integer = trx0 + tempRect.width
				Dim try0 As Integer = tempRect.y, try1 As Integer = try0 + tempRect.height
				Dim crx0 As Integer = clip.x, crx1 As Integer = crx0 + clip.width
				Dim cry0 As Integer = clip.y, cry1 As Integer = cry0 + clip.height
				' We should paint views that intersect with clipping region
				' even if the intersection has no inside points (is a line).
				' This is needed for supporting views that have zero width, like
				' views that contain only combining marks.
				If (trx1 >= crx0) AndAlso (try1 >= cry0) AndAlso (crx1 >= trx0) AndAlso (cry1 >= try0) Then paintChild(g, tempRect, i)
			Next i
		End Sub

		''' <summary>
		''' Fetches the allocation for the given child view.
		''' This enables finding out where various views
		''' are located.  This is implemented to return
		''' <code>null</code> if the layout is invalid,
		''' otherwise the superclass behavior is executed.
		''' </summary>
		''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &gt; getViewCount() </param>
		''' <param name="a">  the allocation to this view </param>
		''' <returns> the allocation to the child; or <code>null</code>
		'''          if <code>a</code> is <code>null</code>;
		'''          or <code>null</code> if the layout is invalid </returns>
		Public Overrides Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
			If a IsNot Nothing Then
				Dim ca As Shape = MyBase.getChildAllocation(index, a)
				If (ca IsNot Nothing) AndAlso ((Not allocationValid)) Then
					' The child allocation may not have been set yet.
					Dim r As Rectangle = If(TypeOf ca Is Rectangle, CType(ca, Rectangle), ca.bounds)
					If (r.width = 0) AndAlso (r.height = 0) Then Return Nothing
				End If
				Return ca
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.  This makes
		''' sure the allocation is valid before calling the superclass.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''  not represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			If Not allocationValid Then
				Dim alloc As Rectangle = a.bounds
				sizeize(alloc.width, alloc.height)
			End If
			Return MyBase.modelToView(pos, a, b)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x">   x coordinate of the view location to convert &gt;= 0 </param>
		''' <param name="y">   y coordinate of the view location to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			If Not allocationValid Then
				Dim alloc As Rectangle = a.bounds
				sizeize(alloc.width, alloc.height)
			End If
			Return MyBase.viewToModel(x, y, a, bias)
		End Function

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  This is implemented to give the total alignment
		''' needed to position the children with the alignment points
		''' lined up along the axis orthogonal to the axis that is
		''' being tiled.  The axis being tiled will request to be
		''' centered (i.e. 0.5f).
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''   or <code>View.Y_AXIS</code> </param>
		''' <returns> the desired alignment &gt;= 0.0f &amp;&amp; &lt;= 1.0f; this should
		'''   be a value between 0.0 and 1.0 where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin; an alignment of 0.5 would be the
		'''   center of the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			checkRequests(axis)
			If axis = majorAxis Then
				Return majorRequest.alignment
			Else
				Return minorRequest.alignment
			End If
		End Function

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			checkRequests(axis)
			Dim marginSpan As Single = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
			If axis = majorAxis Then
				Return (CSng(majorRequest.preferred)) + marginSpan
			Else
				Return (CSng(minorRequest.preferred)) + marginSpan
			End If
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>  the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			checkRequests(axis)
			Dim marginSpan As Single = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
			If axis = majorAxis Then
				Return (CSng(majorRequest.minimum)) + marginSpan
			Else
				Return (CSng(minorRequest.minimum)) + marginSpan
			End If
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''           or <code>View.Y_AXIS</code> </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			checkRequests(axis)
			Dim marginSpan As Single = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
			If axis = majorAxis Then
				Return (CSng(majorRequest.maximum)) + marginSpan
			Else
				Return (CSng(minorRequest.maximum)) + marginSpan
			End If
		End Function

		' --- local methods ----------------------------------------------------

		''' <summary>
		''' Are the allocations for the children still
		''' valid?
		''' </summary>
		''' <returns> true if allocations still valid </returns>
		Protected Friend Overridable Property allocationValid As Boolean
			Get
				Return (majorAllocValid AndAlso minorAllocValid)
			End Get
		End Property

		''' <summary>
		''' Determines if a point falls before an allocated region.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="innerAlloc"> the allocated region; this is the area
		'''   inside of the insets </param>
		''' <returns> true if the point lies before the region else false </returns>
		Protected Friend Overrides Function isBefore(ByVal x As Integer, ByVal y As Integer, ByVal innerAlloc As Rectangle) As Boolean
			If majorAxis = View.X_AXIS Then
				Return (x < innerAlloc.x)
			Else
				Return (y < innerAlloc.y)
			End If
		End Function

		''' <summary>
		''' Determines if a point falls after an allocated region.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="innerAlloc"> the allocated region; this is the area
		'''   inside of the insets </param>
		''' <returns> true if the point lies after the region else false </returns>
		Protected Friend Overrides Function isAfter(ByVal x As Integer, ByVal y As Integer, ByVal innerAlloc As Rectangle) As Boolean
			If majorAxis = View.X_AXIS Then
				Return (x > (innerAlloc.width + innerAlloc.x))
			Else
				Return (y > (innerAlloc.height + innerAlloc.y))
			End If
		End Function

		''' <summary>
		''' Fetches the child view at the given coordinates.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="alloc"> the parents inner allocation on entry, which should
		'''   be changed to the child's allocation on exit </param>
		''' <returns> the view </returns>
		Protected Friend Overrides Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View
			Dim n As Integer = viewCount
			If majorAxis = View.X_AXIS Then
				If x < (alloc.x + majorOffsets(0)) Then
					childAllocation(0, alloc)
					Return getView(0)
				End If
				For i As Integer = 0 To n - 1
					If x < (alloc.x + majorOffsets(i)) Then
						childAllocation(i - 1, alloc)
						Return getView(i - 1)
					End If
				Next i
				childAllocation(n - 1, alloc)
				Return getView(n - 1)
			Else
				If y < (alloc.y + majorOffsets(0)) Then
					childAllocation(0, alloc)
					Return getView(0)
				End If
				For i As Integer = 0 To n - 1
					If y < (alloc.y + majorOffsets(i)) Then
						childAllocation(i - 1, alloc)
						Return getView(i - 1)
					End If
				Next i
				childAllocation(n - 1, alloc)
				Return getView(n - 1)
			End If
		End Function

		''' <summary>
		''' Allocates a region for a child view.
		''' </summary>
		''' <param name="index"> the index of the child view to
		'''   allocate, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <param name="alloc"> the allocated region </param>
		Protected Friend Overrides Sub childAllocation(ByVal index As Integer, ByVal alloc As Rectangle)
			alloc.x += getOffset(X_AXIS, index)
			alloc.y += getOffset(Y_AXIS, index)
			alloc.width = getSpan(X_AXIS, index)
			alloc.height = getSpan(Y_AXIS, index)
		End Sub

		''' <summary>
		''' Perform layout on the box
		''' </summary>
		''' <param name="width"> the width (inside of the insets) &gt;= 0 </param>
		''' <param name="height"> the height (inside of the insets) &gt;= 0 </param>
		Protected Friend Overridable Sub layout(ByVal width As Integer, ByVal height As Integer)
			spanOnAxisxis(X_AXIS, width)
			spanOnAxisxis(Y_AXIS, height)
		End Sub

		''' <summary>
		''' Returns the current width of the box.  This is the width that
		''' it was last allocated. </summary>
		''' <returns> the current width of the box </returns>
		Public Overridable Property width As Integer
			Get
				Dim ___span As Integer
				If majorAxis = X_AXIS Then
					___span = majorSpan
				Else
					___span = minorSpan
				End If
				___span += leftInset - rightInset
				Return ___span
			End Get
		End Property

		''' <summary>
		''' Returns the current height of the box.  This is the height that
		''' it was last allocated. </summary>
		''' <returns> the current height of the box </returns>
		Public Overridable Property height As Integer
			Get
				Dim ___span As Integer
				If majorAxis = Y_AXIS Then
					___span = majorSpan
				Else
					___span = minorSpan
				End If
				___span += topInset - bottomInset
				Return ___span
			End Get
		End Property

		''' <summary>
		''' Performs layout for the major axis of the box (i.e. the
		''' axis that it represents). The results of the layout (the
		''' offset and span for each children) are placed in the given
		''' arrays which represent the allocations to the children
		''' along the major axis.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views; this is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view; this is a return
		'''  value and is filled in by the implementation of this method </param>
		Protected Friend Overridable Sub layoutMajorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
	'        
	'         * first pass, calculate the preferred sizes
	'         * and the flexibility to adjust the sizes.
	'         
			Dim preferred As Long = 0
			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				spans(i) = CInt(Fix(v.getPreferredSpan(axis)))
				preferred += spans(i)
			Next i

	'        
	'         * Second pass, expand or contract by as much as possible to reach
	'         * the target span.
	'         

			' determine the adjustment to be made
			Dim desiredAdjustment As Long = targetSpan - preferred
			Dim adjustmentFactor As Single = 0.0f
			Dim diffs As Integer() = Nothing

			If desiredAdjustment <> 0 Then
				Dim totalSpan As Long = 0
				diffs = New Integer(n - 1){}
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					Dim tmp As Integer
					If desiredAdjustment < 0 Then
						tmp = CInt(Fix(v.getMinimumSpan(axis)))
						diffs(i) = spans(i) - tmp
					Else
						tmp = CInt(Fix(v.getMaximumSpan(axis)))
						diffs(i) = tmp - spans(i)
					End If
					totalSpan += tmp
				Next i

				Dim maximumAdjustment As Single = Math.Abs(totalSpan - preferred)
					adjustmentFactor = desiredAdjustment / maximumAdjustment
					adjustmentFactor = Math.Min(adjustmentFactor, 1.0f)
					adjustmentFactor = Math.Max(adjustmentFactor, -1.0f)
			End If

			' make the adjustments
			Dim totalOffset As Integer = 0
			For i As Integer = 0 To n - 1
				offsets(i) = totalOffset
				If desiredAdjustment <> 0 Then
					Dim adjF As Single = adjustmentFactor * diffs(i)
					spans(i) += Math.Round(adjF)
				End If
				totalOffset = CInt(Fix(Math.Min(CLng(totalOffset) + CLng(spans(i)), Integer.MaxValue)))
			Next i
		End Sub

		''' <summary>
		''' Performs layout for the minor axis of the box (i.e. the
		''' axis orthogonal to the axis that it represents). The results
		''' of the layout (the offset and span for each children) are
		''' placed in the given arrays which represent the allocations to
		''' the children along the minor axis.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being layed out </param>
		''' <param name="offsets"> the offsets from the origin of the view for
		'''  each of the child views; this is a return value and is
		'''  filled in by the implementation of this method </param>
		''' <param name="spans"> the span of each child view; this is a return
		'''  value and is filled in by the implementation of this method </param>
		Protected Friend Overridable Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				Dim max As Integer = CInt(Fix(v.getMaximumSpan(axis)))
				If max < targetSpan Then
					' can't make the child this wide, align it
					Dim align As Single = v.getAlignment(axis)
					offsets(i) = CInt(Fix((targetSpan - max) * align))
					spans(i) = max
				Else
					' make it the target width, or as small as it can get.
					Dim min As Integer = CInt(Fix(v.getMinimumSpan(axis)))
					offsets(i) = 0
					spans(i) = Math.Max(min, targetSpan)
				End If
			Next i
		End Sub

		''' <summary>
		''' Calculates the size requirements for the major axis
		''' <code>axis</code>.
		''' </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <param name="r"> the <code>SizeRequirements</code> object;
		'''          if <code>null</code> one will be created </param>
		''' <returns> the newly initialized <code>SizeRequirements</code> object </returns>
		''' <seealso cref= javax.swing.SizeRequirements </seealso>
		Protected Friend Overridable Function calculateMajorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			' calculate tiled request
			Dim min As Single = 0
			Dim pref As Single = 0
			Dim max As Single = 0

			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				min += v.getMinimumSpan(axis)
				pref += v.getPreferredSpan(axis)
				max += v.getMaximumSpan(axis)
			Next i

			If r Is Nothing Then r = New javax.swing.SizeRequirements
			r.alignment = 0.5f
			r.minimum = CInt(Fix(min))
			r.preferred = CInt(Fix(pref))
			r.maximum = CInt(Fix(max))
			Return r
		End Function

		''' <summary>
		''' Calculates the size requirements for the minor axis
		''' <code>axis</code>.
		''' </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <param name="r"> the <code>SizeRequirements</code> object;
		'''          if <code>null</code> one will be created </param>
		''' <returns> the newly initialized <code>SizeRequirements</code> object </returns>
		''' <seealso cref= javax.swing.SizeRequirements </seealso>
		Protected Friend Overridable Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			Dim min As Integer = 0
			Dim pref As Long = 0
			Dim max As Integer = Integer.MaxValue
			Dim n As Integer = viewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				min = Math.Max(CInt(Fix(v.getMinimumSpan(axis))), min)
				pref = Math.Max(CInt(Fix(v.getPreferredSpan(axis))), pref)
				max = Math.Max(CInt(Fix(v.getMaximumSpan(axis))), max)
			Next i

			If r Is Nothing Then
				r = New javax.swing.SizeRequirements
				r.alignment = 0.5f
			End If
			r.preferred = CInt(pref)
			r.minimum = min
			r.maximum = max
			Return r
		End Function

		''' <summary>
		''' Checks the request cache and update if needed. </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <exception cref="IllegalArgumentException"> if <code>axis</code> is
		'''  neither <code>View.X_AXIS</code> nor <code>View.Y_AXIS</code> </exception>
		Friend Overridable Sub checkRequests(ByVal axis As Integer)
			If (axis <> X_AXIS) AndAlso (axis <> Y_AXIS) Then Throw New System.ArgumentException("Invalid axis: " & axis)
			If axis = majorAxis Then
				If Not majorReqValid Then
					majorRequest = calculateMajorAxisRequirements(axis, majorRequest)
					majorReqValid = True
				End If
			ElseIf Not minorReqValid Then
				minorRequest = calculateMinorAxisRequirements(axis, minorRequest)
				minorReqValid = True
			End If
		End Sub

		''' <summary>
		''' Computes the location and extent of each child view
		''' in this <code>BoxView</code> given the <code>targetSpan</code>,
		''' which is the width (or height) of the region we have to
		''' work with.
		''' </summary>
		''' <param name="targetSpan"> the total span given to the view, which
		'''  would be used to layout the children </param>
		''' <param name="axis"> the axis being studied, either
		'''          <code>View.X_AXIS</code> or <code>View.Y_AXIS</code> </param>
		''' <param name="offsets"> an empty array filled by this method with
		'''          values specifying the location  of each child view </param>
		''' <param name="spans">  an empty array filled by this method with
		'''          values specifying the extent of each child view </param>
		Protected Friend Overridable Sub baselineLayout(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
			Dim totalAscent As Integer = CInt(Fix(targetSpan * getAlignment(axis)))
			Dim totalDescent As Integer = targetSpan - totalAscent

			Dim n As Integer = viewCount

			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				Dim align As Single = v.getAlignment(axis)
				Dim viewSpan As Single

				If v.getResizeWeight(axis) > 0 Then
					' if resizable then resize to the best fit

					' the smallest span possible
					Dim minSpan As Single = v.getMinimumSpan(axis)
					' the largest span possible
					Dim maxSpan As Single = v.getMaximumSpan(axis)

					If align = 0.0f Then
						' if the alignment is 0 then we need to fit into the descent
						viewSpan = Math.Max(Math.Min(maxSpan, totalDescent), minSpan)
					ElseIf align = 1.0f Then
						' if the alignment is 1 then we need to fit into the ascent
						viewSpan = Math.Max(Math.Min(maxSpan, totalAscent), minSpan)
					Else
						' figure out the span that we must fit into
						Dim fitSpan As Single = Math.Min(totalAscent / align, totalDescent / (1.0f - align))
						' fit into the calculated span
						viewSpan = Math.Max(Math.Min(maxSpan, fitSpan), minSpan)
					End If
				Else
					' otherwise use the preferred spans
					viewSpan = v.getPreferredSpan(axis)
				End If

				offsets(i) = totalAscent - CInt(Fix(viewSpan * align))
				spans(i) = CInt(Fix(viewSpan))
			Next i
		End Sub

		''' <summary>
		''' Calculates the size requirements for this <code>BoxView</code>
		''' by examining the size of each child view.
		''' </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <param name="r"> the <code>SizeRequirements</code> object;
		'''          if <code>null</code> one will be created </param>
		''' <returns> the newly initialized <code>SizeRequirements</code> object </returns>
		Protected Friend Overridable Function baselineRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			Dim totalAscent As New javax.swing.SizeRequirements
			Dim totalDescent As New javax.swing.SizeRequirements

			If r Is Nothing Then r = New javax.swing.SizeRequirements

			r.alignment = 0.5f

			Dim n As Integer = viewCount

			' loop through all children calculating the max of all their ascents and
			' descents at minimum, preferred, and maximum sizes
			For i As Integer = 0 To n - 1
				Dim v As View = getView(i)
				Dim align As Single = v.getAlignment(axis)
				Dim ___span As Single
				Dim ascent As Integer
				Dim descent As Integer

				' find the maximum of the preferred ascents and descents
				___span = v.getPreferredSpan(axis)
				ascent = CInt(Fix(align * ___span))
				descent = CInt(Fix(___span - ascent))
				totalAscent.preferred = Math.Max(ascent, totalAscent.preferred)
				totalDescent.preferred = Math.Max(descent, totalDescent.preferred)

				If v.getResizeWeight(axis) > 0 Then
					' if the view is resizable then do the same for the minimum and
					' maximum ascents and descents
					___span = v.getMinimumSpan(axis)
					ascent = CInt(Fix(align * ___span))
					descent = CInt(Fix(___span - ascent))
					totalAscent.minimum = Math.Max(ascent, totalAscent.minimum)
					totalDescent.minimum = Math.Max(descent, totalDescent.minimum)

					___span = v.getMaximumSpan(axis)
					ascent = CInt(Fix(align * ___span))
					descent = CInt(Fix(___span - ascent))
					totalAscent.maximum = Math.Max(ascent, totalAscent.maximum)
					totalDescent.maximum = Math.Max(descent, totalDescent.maximum)
				Else
					' otherwise use the preferred
					totalAscent.minimum = Math.Max(ascent, totalAscent.minimum)
					totalDescent.minimum = Math.Max(descent, totalDescent.minimum)
					totalAscent.maximum = Math.Max(ascent, totalAscent.maximum)
					totalDescent.maximum = Math.Max(descent, totalDescent.maximum)
				End If
			Next i

			' we now have an overall preferred, minimum, and maximum ascent and descent

			' calculate the preferred span as the sum of the preferred ascent and preferred descent
			r.preferred = CInt(Fix(Math.Min(CLng(totalAscent.preferred) + CLng(totalDescent.preferred), Integer.MaxValue)))

			' calculate the preferred alignment as the preferred ascent divided by the preferred span
			If r.preferred > 0 Then r.alignment = CSng(totalAscent.preferred) / r.preferred


			If r.alignment = 0.0f Then
				' if the preferred alignment is 0 then the minimum and maximum spans are simply
				' the minimum and maximum descents since there's nothing above the baseline
				r.minimum = totalDescent.minimum
				r.maximum = totalDescent.maximum
			ElseIf r.alignment = 1.0f Then
				' if the preferred alignment is 1 then the minimum and maximum spans are simply
				' the minimum and maximum ascents since there's nothing below the baseline
				r.minimum = totalAscent.minimum
				r.maximum = totalAscent.maximum
			Else
				' we want to honor the preferred alignment so we calculate two possible minimum
				' span values using 1) the minimum ascent and the alignment, and 2) the minimum
				' descent and the alignment. We'll choose the larger of these two numbers.
				r.minimum = Math.Round(Math.Max(totalAscent.minimum / r.alignment, totalDescent.minimum / (1.0f - r.alignment)))
				' a similar calculation is made for the maximum but we choose the smaller number.
				r.maximum = Math.Round(Math.Min(totalAscent.maximum / r.alignment, totalDescent.maximum / (1.0f - r.alignment)))
			End If

			Return r
		End Function

		''' <summary>
		''' Fetches the offset of a particular child's current layout. </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <param name="childIndex"> the index of the requested child </param>
		''' <returns> the offset (location) for the specified child </returns>
		Protected Friend Overridable Function getOffset(ByVal axis As Integer, ByVal childIndex As Integer) As Integer
			Dim offsets As Integer() = If(axis = majorAxis, majorOffsets, minorOffsets)
			Return offsets(childIndex)
		End Function

		''' <summary>
		''' Fetches the span of a particular child's current layout. </summary>
		''' <param name="axis"> the axis being studied </param>
		''' <param name="childIndex"> the index of the requested child </param>
		''' <returns> the span (width or height) of the specified child </returns>
		Protected Friend Overridable Function getSpan(ByVal axis As Integer, ByVal childIndex As Integer) As Integer
			Dim spans As Integer() = If(axis = majorAxis, majorSpans, minorSpans)
			Return spans(childIndex)
		End Function

		''' <summary>
		''' Determines in which direction the next view lays.
		''' Consider the View at index n. Typically the <code>View</code>s
		''' are layed out from left to right, so that the <code>View</code>
		''' to the EAST will be at index n + 1, and the <code>View</code>
		''' to the WEST will be at index n - 1. In certain situations,
		''' such as with bidirectional text, it is possible
		''' that the <code>View</code> to EAST is not at index n + 1,
		''' but rather at index n - 1, or that the <code>View</code>
		''' to the WEST is not at index n - 1, but index n + 1.
		''' In this case this method would return true,
		''' indicating the <code>View</code>s are layed out in
		''' descending order. Otherwise the method would return false
		''' indicating the <code>View</code>s are layed out in ascending order.
		''' <p>
		''' If the receiver is laying its <code>View</code>s along the
		''' <code>Y_AXIS</code>, this will will return the value from
		''' invoking the same method on the <code>View</code>
		''' responsible for rendering <code>position</code> and
		''' <code>bias</code>. Otherwise this will return false.
		''' </summary>
		''' <param name="position"> position into the model </param>
		''' <param name="bias"> either <code>Position.Bias.Forward</code> or
		'''          <code>Position.Bias.Backward</code> </param>
		''' <returns> true if the <code>View</code>s surrounding the
		'''          <code>View</code> responding for rendering
		'''          <code>position</code> and <code>bias</code>
		'''          are layed out in descending order; otherwise false </returns>
		Protected Friend Overrides Function flipEastAndWestAtEnds(ByVal position As Integer, ByVal bias As Position.Bias) As Boolean
			If majorAxis = Y_AXIS Then
				Dim testPos As Integer = If(bias Is Position.Bias.Backward, Math.Max(0, position - 1), position)
				Dim index As Integer = getViewIndexAtPosition(testPos)
				If index <> -1 Then
					Dim v As View = getView(index)
					If v IsNot Nothing AndAlso TypeOf v Is CompositeView Then Return CType(v, CompositeView).flipEastAndWestAtEnds(position, bias)
				End If
			End If
			Return False
		End Function

		' --- variables ------------------------------------------------

		Friend majorAxis As Integer

		Friend majorSpan As Integer
		Friend minorSpan As Integer

	'    
	'     * Request cache
	'     
		Friend majorReqValid As Boolean
		Friend minorReqValid As Boolean
		Friend majorRequest As javax.swing.SizeRequirements
		Friend minorRequest As javax.swing.SizeRequirements

	'    
	'     * Allocation cache
	'     
		Friend majorAllocValid As Boolean
		Friend majorOffsets As Integer()
		Friend majorSpans As Integer()
		Friend minorAllocValid As Boolean
		Friend minorOffsets As Integer()
		Friend minorSpans As Integer()

		''' <summary>
		''' used in paint. </summary>
		Friend tempRect As Rectangle
	End Class

End Namespace