Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports javax.swing.event

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A View that tries to flow it's children into some
	''' partially constrained space.  This can be used to
	''' build things like paragraphs, pages, etc.  The
	''' flow is made up of the following pieces of functionality.
	''' <ul>
	''' <li>A logical set of child views, which as used as a
	''' layout pool from which a physical view is formed.
	''' <li>A strategy for translating the logical view to
	''' a physical (flowed) view.
	''' <li>Constraints for the strategy to work against.
	''' <li>A physical structure, that represents the flow.
	''' The children of this view are where the pieces of
	''' of the logical views are placed to create the flow.
	''' </ul>
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View
	''' @since 1.3 </seealso>
	Public MustInherit Class FlowView
		Inherits BoxView

		''' <summary>
		''' Constructs a FlowView for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem, axis)
			layoutSpan = Integer.MaxValue
			strategy = New FlowStrategy
		End Sub

		''' <summary>
		''' Fetches the axis along which views should be
		''' flowed.  By default, this will be the axis
		''' orthogonal to the axis along which the flow
		''' rows are tiled (the axis of the default flow
		''' rows themselves).  This is typically used
		''' by the <code>FlowStrategy</code>.
		''' </summary>
		Public Overridable Property flowAxis As Integer
			Get
				If axis = Y_AXIS Then Return X_AXIS
				Return Y_AXIS
			End Get
		End Property

		''' <summary>
		''' Fetch the constraining span to flow against for
		''' the given child index.  This is called by the
		''' FlowStrategy while it is updating the flow.
		''' A flow can be shaped by providing different values
		''' for the row constraints.  By default, the entire
		''' span inside of the insets along the flow axis
		''' is returned.
		''' </summary>
		''' <param name="index"> the index of the row being updated.
		'''   This should be a value &gt;= 0 and &lt; getViewCount(). </param>
		''' <seealso cref= #getFlowStart </seealso>
		Public Overridable Function getFlowSpan(ByVal index As Integer) As Integer
			Return layoutSpan
		End Function

		''' <summary>
		''' Fetch the location along the flow axis that the
		''' flow span will start at.  This is called by the
		''' FlowStrategy while it is updating the flow.
		''' A flow can be shaped by providing different values
		''' for the row constraints.
		''' </summary>
		''' <param name="index"> the index of the row being updated.
		'''   This should be a value &gt;= 0 and &lt; getViewCount(). </param>
		''' <seealso cref= #getFlowSpan </seealso>
		Public Overridable Function getFlowStart(ByVal index As Integer) As Integer
			Return 0
		End Function

		''' <summary>
		''' Create a View that should be used to hold a
		''' a rows worth of children in a flow.  This is
		''' called by the FlowStrategy when new children
		''' are added or removed (i.e. rows are added or
		''' removed) in the process of updating the flow.
		''' </summary>
		Protected Friend MustOverride Function createRow() As View

		' ---- BoxView methods -------------------------------------

		''' <summary>
		''' Loads all of the children to initialize the view.
		''' This is called by the <code>setParent</code> method.
		''' This is reimplemented to not load any children directly
		''' (as they are created in the process of formatting).
		''' If the layoutPool variable is null, an instance of
		''' LogicalView is created to represent the logical view
		''' that is used in the process of formatting.
		''' </summary>
		''' <param name="f"> the view factory </param>
		Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
			If layoutPool Is Nothing Then layoutPool = New LogicalView(element)
			layoutPool.parent = Me

			' This synthetic insertUpdate call gives the strategy a chance
			' to initialize.
			strategy.insertUpdate(Me, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Fetches the child view index representing the given position in
		''' the model.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position </returns>
		Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
			If pos >= startOffset AndAlso (pos < endOffset) Then
				For counter As Integer = 0 To viewCount - 1
					Dim v As View = getView(counter)
					If pos >= v.startOffset AndAlso pos < v.endOffset Then Return counter
				Next counter
			End If
			Return -1
		End Function

		''' <summary>
		''' Lays out the children.  If the span along the flow
		''' axis has changed, layout is marked as invalid which
		''' which will cause the superclass behavior to recalculate
		''' the layout along the box axis.  The FlowStrategy.layout
		''' method will be called to rebuild the flow rows as
		''' appropriate.  If the height of this view changes
		''' (determined by the preferred size along the box axis),
		''' a preferenceChanged is called.  Following all of that,
		''' the normal box layout of the superclass is performed.
		''' </summary>
		''' <param name="width">  the width to lay out against &gt;= 0.  This is
		'''   the width inside of the inset area. </param>
		''' <param name="height"> the height to lay out against &gt;= 0 This
		'''   is the height inside of the inset area. </param>
		Protected Friend Overrides Sub layout(ByVal width As Integer, ByVal height As Integer)
			Dim faxis As Integer = flowAxis
			Dim newSpan As Integer
			If faxis = X_AXIS Then
				newSpan = width
			Else
				newSpan = height
			End If
			If layoutSpan <> newSpan Then
				layoutChanged(faxis)
				layoutChanged(axis)
				layoutSpan = newSpan
			End If

			' repair the flow if necessary
			If Not isLayoutValid(faxis) Then
				Dim heightAxis As Integer = axis
				Dim oldFlowHeight As Integer = If(heightAxis = X_AXIS, width, height)
				strategy.layout(Me)
				Dim newFlowHeight As Integer = CInt(Fix(getPreferredSpan(heightAxis)))
				If oldFlowHeight <> newFlowHeight Then
					Dim p As View = parent
					If p IsNot Nothing Then p.preferenceChanged(Me, (heightAxis = X_AXIS), (heightAxis = Y_AXIS))

					' PENDING(shannonh)
					' Temporary fix for 4250847
					' Can be removed when TraversalContext is added
					Dim host As Component = container
					If host IsNot Nothing Then host.repaint()
				End If
			End If

			MyBase.layout(width, height)
		End Sub

		''' <summary>
		''' Calculate requirements along the minor axis.  This
		''' is implemented to forward the request to the logical
		''' view by calling getMinimumSpan, getPreferredSpan, and
		''' getMaximumSpan on it.
		''' </summary>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			If r Is Nothing Then r = New javax.swing.SizeRequirements
			Dim pref As Single = layoutPool.getPreferredSpan(axis)
			Dim min As Single = layoutPool.getMinimumSpan(axis)
			' Don't include insets, Box.getXXXSpan will include them.
			r.minimum = CInt(Fix(min))
			r.preferred = Math.Max(r.minimum, CInt(Fix(pref)))
			r.maximum = Integer.MaxValue
			r.alignment = 0.5f
			Return r
		End Function

		' ---- View methods ----------------------------------------------------

		''' <summary>
		''' Gives notification that something was inserted into the document
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			layoutPool.insertUpdate(changes, a, f)
			strategy.insertUpdate(Me, changes, getInsideAllocation(a))
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overrides Sub removeUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			layoutPool.removeUpdate(changes, a, f)
			strategy.removeUpdate(Me, changes, getInsideAllocation(a))
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overrides Sub changedUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			layoutPool.changedUpdate(changes, a, f)
			strategy.changedUpdate(Me, changes, getInsideAllocation(a))
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				MyBase.parent = parent
				If parent Is Nothing AndAlso layoutPool IsNot Nothing Then layoutPool.parent = Nothing
			End Set
		End Property

		' --- variables -----------------------------------------------

		''' <summary>
		''' Default constraint against which the flow is
		''' created against.
		''' </summary>
		Protected Friend layoutSpan As Integer

		''' <summary>
		''' These are the views that represent the child elements
		''' of the element this view represents (The logical view
		''' to translate to a physical view).  These are not
		''' directly children of this view.  These are either
		''' placed into the rows directly or used for the purpose
		''' of breaking into smaller chunks, to form the physical
		''' view.
		''' </summary>
		Protected Friend layoutPool As View

		''' <summary>
		''' The behavior for keeping the flow updated.  By
		''' default this is a singleton shared by all instances
		''' of FlowView (FlowStrategy is stateless).  Subclasses
		''' can create an alternative strategy, which might keep
		''' state.
		''' </summary>
		Protected Friend strategy As FlowStrategy

		''' <summary>
		''' Strategy for maintaining the physical form
		''' of the flow.  The default implementation is
		''' completely stateless, and recalculates the
		''' entire flow if the layout is invalid on the
		''' given FlowView.  Alternative strategies can
		''' be implemented by subclassing, and might
		''' perform incremental repair to the layout
		''' or alternative breaking behavior.
		''' @since 1.3
		''' </summary>
		Public Class FlowStrategy
			Friend damageStart As Position = Nothing
			Friend viewBuffer As List(Of View)

			Friend Overridable Sub addDamage(ByVal fv As FlowView, ByVal offset As Integer)
				If offset >= fv.startOffset AndAlso offset < fv.endOffset Then
					If damageStart Is Nothing OrElse offset < damageStart.offset Then
						Try
							damageStart = fv.document.createPosition(offset)
						Catch e As BadLocationException
							' shouldn't happen since offset is inside view bounds
							assert(False)
						End Try
					End If
				End If
			End Sub

			Friend Overridable Sub unsetDamage()
				damageStart = Nothing
			End Sub

			''' <summary>
			''' Gives notification that something was inserted into the document
			''' in a location that the given flow view is responsible for.  The
			''' strategy should update the appropriate changed region (which
			''' depends upon the strategy used for repair).
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="alloc"> the current allocation of the view inside of the insets.
			'''   This value will be null if the view has not yet been displayed. </param>
			''' <seealso cref= View#insertUpdate </seealso>
			Public Overridable Sub insertUpdate(ByVal fv As FlowView, ByVal e As DocumentEvent, ByVal alloc As Rectangle)
				' FlowView.loadChildren() makes a synthetic call into this,
				' passing null as e
				If e IsNot Nothing Then addDamage(fv, e.offset)

				If alloc IsNot Nothing Then
					Dim host As Component = fv.container
					If host IsNot Nothing Then host.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				Else
					fv.preferenceChanged(Nothing, True, True)
				End If
			End Sub

			''' <summary>
			''' Gives notification that something was removed from the document
			''' in a location that the given flow view is responsible for.
			''' </summary>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="alloc"> the current allocation of the view inside of the insets. </param>
			''' <seealso cref= View#removeUpdate </seealso>
			Public Overridable Sub removeUpdate(ByVal fv As FlowView, ByVal e As DocumentEvent, ByVal alloc As Rectangle)
				addDamage(fv, e.offset)
				If alloc IsNot Nothing Then
					Dim host As Component = fv.container
					If host IsNot Nothing Then host.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				Else
					fv.preferenceChanged(Nothing, True, True)
				End If
			End Sub

			''' <summary>
			''' Gives notification from the document that attributes were changed
			''' in a location that this view is responsible for.
			''' </summary>
			''' <param name="fv">     the <code>FlowView</code> containing the changes </param>
			''' <param name="e">      the <code>DocumentEvent</code> describing the changes
			'''               done to the Document </param>
			''' <param name="alloc">  Bounds of the View </param>
			''' <seealso cref= View#changedUpdate </seealso>
			Public Overridable Sub changedUpdate(ByVal fv As FlowView, ByVal e As DocumentEvent, ByVal alloc As Rectangle)
				addDamage(fv, e.offset)
				If alloc IsNot Nothing Then
					Dim host As Component = fv.container
					If host IsNot Nothing Then host.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				Else
					fv.preferenceChanged(Nothing, True, True)
				End If
			End Sub

			''' <summary>
			''' This method gives flow strategies access to the logical
			''' view of the FlowView.
			''' </summary>
			Protected Friend Overridable Function getLogicalView(ByVal fv As FlowView) As View
				Return fv.layoutPool
			End Function

			''' <summary>
			''' Update the flow on the given FlowView.  By default, this causes
			''' all of the rows (child views) to be rebuilt to match the given
			''' constraints for each row.  This is called by a FlowView.layout
			''' to update the child views in the flow.
			''' </summary>
			''' <param name="fv"> the view to reflow </param>
			Public Overridable Sub layout(ByVal fv As FlowView)
				Dim pool As View = getLogicalView(fv)
				Dim rowIndex, p0 As Integer
				Dim p1 As Integer = fv.endOffset

				If fv.majorAllocValid Then
					If damageStart Is Nothing Then Return
					' In some cases there's no view at position damageStart, so
					' step back and search again. See 6452106 for details.
					Dim offset As Integer = damageStart.offset
					rowIndex = fv.getViewIndexAtPosition(offset)
					Do While rowIndex < 0
						offset -= 1
						rowIndex = fv.getViewIndexAtPosition(offset)
					Loop
					If rowIndex > 0 Then rowIndex -= 1
					p0 = fv.getView(rowIndex).startOffset
				Else
					rowIndex = 0
					p0 = fv.startOffset
				End If
				reparentViews(pool, p0)

				viewBuffer = New List(Of View)(10, 10)
				Dim rowCount As Integer = fv.viewCount
				Do While p0 < p1
					Dim row As View
					If rowIndex >= rowCount Then
						row = fv.createRow()
						fv.append(row)
					Else
						row = fv.getView(rowIndex)
					End If
					p0 = layoutRow(fv, rowIndex, p0)
					rowIndex += 1
				Loop
				viewBuffer = Nothing

				If rowIndex < rowCount Then fv.replace(rowIndex, rowCount - rowIndex, Nothing)
				unsetDamage()
			End Sub

			''' <summary>
			''' Creates a row of views that will fit within the
			''' layout span of the row.  This is called by the layout method.
			''' This is implemented to fill the row by repeatedly calling
			''' the createView method until the available span has been
			''' exhausted, a forced break was encountered, or the createView
			''' method returned null.  If the remaining span was exhausted,
			''' the adjustRow method will be called to perform adjustments
			''' to the row to try and make it fit into the given span.
			''' </summary>
			''' <param name="rowIndex"> the index of the row to fill in with views.  The
			'''   row is assumed to be empty on entry. </param>
			''' <param name="pos">  The current position in the children of
			'''   this views element from which to start. </param>
			''' <returns> the position to start the next row </returns>
			Protected Friend Overridable Function layoutRow(ByVal fv As FlowView, ByVal rowIndex As Integer, ByVal pos As Integer) As Integer
				Dim row As View = fv.getView(rowIndex)
				Dim x As Single = fv.getFlowStart(rowIndex)
				Dim spanLeft As Single = fv.getFlowSpan(rowIndex)
				Dim [end] As Integer = fv.endOffset
				Dim te As TabExpander = If(TypeOf fv Is TabExpander, CType(fv, TabExpander), Nothing)
				Dim flowAxis As Integer = fv.flowAxis

				Dim breakWeight As Integer = BadBreakWeight
				Dim breakX As Single = 0f
				Dim breakSpan As Single = 0f
				Dim breakIndex As Integer = -1
				Dim n As Integer = 0

				viewBuffer.Clear()
				Do While pos < [end] AndAlso spanLeft >= 0
					Dim v As View = createView(fv, pos, CInt(Fix(spanLeft)), rowIndex)
					If v Is Nothing Then Exit Do

					Dim bw As Integer = v.getBreakWeight(flowAxis, x, spanLeft)
					If bw >= ForcedBreakWeight Then
						Dim w As View = v.breakView(flowAxis, pos, x, spanLeft)
						If w IsNot Nothing Then
							viewBuffer.Add(w)
						ElseIf n = 0 Then
							' if the view does not break, and it is the only view
							' in a row, use the whole view
							viewBuffer.Add(v)
						End If
						Exit Do
					ElseIf bw >= breakWeight AndAlso bw > BadBreakWeight Then
						breakWeight = bw
						breakX = x
						breakSpan = spanLeft
						breakIndex = n
					End If

					Dim chunkSpan As Single
					If flowAxis = X_AXIS AndAlso TypeOf v Is TabableView Then
						chunkSpan = CType(v, TabableView).getTabbedSpan(x, te)
					Else
						chunkSpan = v.getPreferredSpan(flowAxis)
					End If

					If chunkSpan > spanLeft AndAlso breakIndex >= 0 Then
						' row is too long, and we may break
						If breakIndex < n Then v = viewBuffer(breakIndex)
						For i As Integer = n - 1 To breakIndex Step -1
							viewBuffer.RemoveAt(i)
						Next i
						v = v.breakView(flowAxis, v.startOffset, breakX, breakSpan)
					End If

					spanLeft -= chunkSpan
					x += chunkSpan
					viewBuffer.Add(v)
					pos = v.endOffset
					n += 1
				Loop

				Dim views As View() = New View(viewBuffer.Count - 1){}
				viewBuffer.ToArray(views)
				row.replace(0, row.viewCount, views)
				Return (If(views.Length > 0, row.endOffset, pos))
			End Function

			''' <summary>
			''' Adjusts the given row if possible to fit within the
			''' layout span.  By default this will try to find the
			''' highest break weight possible nearest the end of
			''' the row.  If a forced break is encountered, the
			''' break will be positioned there.
			''' </summary>
			''' <param name="rowIndex"> the row to adjust to the current layout
			'''  span. </param>
			''' <param name="desiredSpan"> the current layout span &gt;= 0 </param>
			''' <param name="x"> the location r starts at. </param>
			Protected Friend Overridable Sub adjustRow(ByVal fv As FlowView, ByVal rowIndex As Integer, ByVal desiredSpan As Integer, ByVal x As Integer)
				Dim flowAxis As Integer = fv.flowAxis
				Dim r As View = fv.getView(rowIndex)
				Dim n As Integer = r.viewCount
				Dim span As Integer = 0
				Dim bestWeight As Integer = BadBreakWeight
				Dim bestSpan As Integer = 0
				Dim bestIndex As Integer = -1
				Dim v As View
				For i As Integer = 0 To n - 1
					v = r.getView(i)
					Dim spanLeft As Integer = desiredSpan - span

					Dim w As Integer = v.getBreakWeight(flowAxis, x + span, spanLeft)
					If (w >= bestWeight) AndAlso (w > BadBreakWeight) Then
						bestWeight = w
						bestIndex = i
						bestSpan = span
						If w >= ForcedBreakWeight Then Exit For
					End If
					span += v.getPreferredSpan(flowAxis)
				Next i
				If bestIndex < 0 Then Return

				' Break the best candidate view, and patch up the row.
				Dim spanLeft As Integer = desiredSpan - bestSpan
				v = r.getView(bestIndex)
				v = v.breakView(flowAxis, v.startOffset, x + bestSpan, spanLeft)
				Dim va As View() = New View(0){}
				va(0) = v
				Dim lv As View = getLogicalView(fv)
				Dim p0 As Integer = r.getView(bestIndex).startOffset
				Dim p1 As Integer = r.endOffset
				For i As Integer = 0 To lv.viewCount - 1
					Dim tmpView As View = lv.getView(i)
					If tmpView.endOffset > p1 Then Exit For
					If tmpView.startOffset >= p0 Then tmpView.parent = lv
				Next i
				r.replace(bestIndex, n - bestIndex, va)
			End Sub

			Friend Overridable Sub reparentViews(ByVal pool As View, ByVal startPos As Integer)
				Dim n As Integer = pool.getViewIndex(startPos, Position.Bias.Forward)
				If n >= 0 Then
					For i As Integer = n To pool.viewCount - 1
						pool.getView(i).parent = pool
					Next i
				End If
			End Sub

			''' <summary>
			''' Creates a view that can be used to represent the current piece
			''' of the flow.  This can be either an entire view from the
			''' logical view, or a fragment of the logical view.
			''' </summary>
			''' <param name="fv"> the view holding the flow </param>
			''' <param name="startOffset"> the start location for the view being created </param>
			''' <param name="spanLeft"> the about of span left to fill in the row </param>
			''' <param name="rowIndex"> the row the view will be placed into </param>
			Protected Friend Overridable Function createView(ByVal fv As FlowView, ByVal startOffset As Integer, ByVal spanLeft As Integer, ByVal rowIndex As Integer) As View
				' Get the child view that contains the given starting position
				Dim lv As View = getLogicalView(fv)
				Dim childIndex As Integer = lv.getViewIndex(startOffset, Position.Bias.Forward)
				Dim v As View = lv.getView(childIndex)
				If startOffset=v.startOffset Then Return v

				' return a fragment.
				v = v.createFragment(startOffset, v.endOffset)
				Return v
			End Function
		End Class

		''' <summary>
		''' This class can be used to represent a logical view for
		''' a flow.  It keeps the children updated to reflect the state
		''' of the model, gives the logical child views access to the
		''' view hierarchy, and calculates a preferred span.  It doesn't
		''' do any rendering, layout, or model/view translation.
		''' </summary>
		Friend Class LogicalView
			Inherits CompositeView

			Friend Sub New(ByVal elem As Element)
				MyBase.New(elem)
			End Sub

			Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
				Dim elem As Element = element
				If elem.leaf Then Return 0
				Return MyBase.getViewIndexAtPosition(pos)
			End Function

			Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
				Dim elem As Element = element
				If elem.leaf Then
					Dim v As View = New LabelView(elem)
					append(v)
				Else
					MyBase.loadChildren(f)
				End If
			End Sub

			''' <summary>
			''' Fetches the attributes to use when rendering.  This view
			''' isn't directly responsible for an element so it returns
			''' the outer classes attributes.
			''' </summary>
			Public Property Overrides attributes As AttributeSet
				Get
					Dim p As View = parent
					Return If(p IsNot Nothing, p.attributes, Nothing)
				End Get
			End Property

			''' <summary>
			''' Determines the preferred span for this view along an
			''' axis.
			''' </summary>
			''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
			''' <returns>   the span the view would like to be rendered into.
			'''           Typically the view is told to render into the span
			'''           that is returned, although there is no guarantee.
			'''           The parent may choose to resize or break the view. </returns>
			''' <seealso cref= View#getPreferredSpan </seealso>
			Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
				Dim maxpref As Single = 0
				Dim pref As Single = 0
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					pref += v.getPreferredSpan(axis)
					If v.getBreakWeight(axis, 0, Integer.MaxValue) >= ForcedBreakWeight Then
						maxpref = Math.Max(maxpref, pref)
						pref = 0
					End If
				Next i
				maxpref = Math.Max(maxpref, pref)
				Return maxpref
			End Function

			''' <summary>
			''' Determines the minimum span for this view along an
			''' axis.  The is implemented to find the minimum unbreakable
			''' span.
			''' </summary>
			''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
			''' <returns>  the span the view would like to be rendered into.
			'''           Typically the view is told to render into the span
			'''           that is returned, although there is no guarantee.
			'''           The parent may choose to resize or break the view. </returns>
			''' <seealso cref= View#getPreferredSpan </seealso>
			Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
				Dim maxmin As Single = 0
				Dim min As Single = 0
				Dim nowrap As Boolean = False
				Dim n As Integer = viewCount
				For i As Integer = 0 To n - 1
					Dim v As View = getView(i)
					If v.getBreakWeight(axis, 0, Integer.MaxValue) = BadBreakWeight Then
						min += v.getPreferredSpan(axis)
						nowrap = True
					ElseIf nowrap Then
						maxmin = Math.Max(min, maxmin)
						nowrap = False
						min = 0
					End If
					If TypeOf v Is ComponentView Then maxmin = Math.Max(maxmin, v.getMinimumSpan(axis))
				Next i
				maxmin = Math.Max(maxmin, min)
				Return maxmin
			End Function

			''' <summary>
			''' Forward the DocumentEvent to the given child view.  This
			''' is implemented to reparent the child to the logical view
			''' (the children may have been parented by a row in the flow
			''' if they fit without breaking) and then execute the superclass
			''' behavior.
			''' </summary>
			''' <param name="v"> the child view to forward the event to. </param>
			''' <param name="e"> the change information from the associated document </param>
			''' <param name="a"> the current allocation of the view </param>
			''' <param name="f"> the factory to use to rebuild if the view has children </param>
			''' <seealso cref= #forwardUpdate
			''' @since 1.3 </seealso>
			Protected Friend Overrides Sub forwardUpdateToView(ByVal v As View, ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				Dim ___parent As View = v.parent
				v.parent = Me
				MyBase.forwardUpdateToView(v, e, a, f)
				v.parent = ___parent
			End Sub

			''' <summary>
			''' {@inheritDoc} </summary>
			Protected Friend Overrides Sub forwardUpdate(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				' Update the view responsible for the changed element by invocation of
				' super method.
				MyBase.forwardUpdate(ec, e, a, f)
				' Re-calculate the update indexes and update the views followed by
				' the changed place. Note: we update the views only when insertion or
				' removal takes place.
				Dim type As DocumentEvent.EventType = e.type
				If type Is DocumentEvent.EventType.INSERT OrElse type Is DocumentEvent.EventType.REMOVE Then
					firstUpdateIndex = Math.Min((lastUpdateIndex + 1), (viewCount - 1))
					lastUpdateIndex = Math.Max((viewCount - 1), 0)
					For i As Integer = firstUpdateIndex To lastUpdateIndex
						Dim v As View = getView(i)
						If v IsNot Nothing Then v.updateAfterChange()
					Next i
				End If
			End Sub

			' The following methods don't do anything useful, they
			' simply keep the class from being abstract.

			''' <summary>
			''' Renders using the given rendering surface and area on that
			''' surface.  This is implemented to do nothing, the logical
			''' view is never visible.
			''' </summary>
			''' <param name="g"> the rendering surface to use </param>
			''' <param name="allocation"> the allocated region to render into </param>
			''' <seealso cref= View#paint </seealso>
			Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			End Sub

			''' <summary>
			''' Tests whether a point lies before the rectangle range.
			''' Implemented to return false, as hit detection is not
			''' performed on the logical view.
			''' </summary>
			''' <param name="x"> the X coordinate &gt;= 0 </param>
			''' <param name="y"> the Y coordinate &gt;= 0 </param>
			''' <param name="alloc"> the rectangle </param>
			''' <returns> true if the point is before the specified range </returns>
			Protected Friend Overrides Function isBefore(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
				Return False
			End Function

			''' <summary>
			''' Tests whether a point lies after the rectangle range.
			''' Implemented to return false, as hit detection is not
			''' performed on the logical view.
			''' </summary>
			''' <param name="x"> the X coordinate &gt;= 0 </param>
			''' <param name="y"> the Y coordinate &gt;= 0 </param>
			''' <param name="alloc"> the rectangle </param>
			''' <returns> true if the point is after the specified range </returns>
			Protected Friend Overrides Function isAfter(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean
				Return False
			End Function

			''' <summary>
			''' Fetches the child view at the given point.
			''' Implemented to return null, as hit detection is not
			''' performed on the logical view.
			''' </summary>
			''' <param name="x"> the X coordinate &gt;= 0 </param>
			''' <param name="y"> the Y coordinate &gt;= 0 </param>
			''' <param name="alloc"> the parent's allocation on entry, which should
			'''   be changed to the child's allocation on exit </param>
			''' <returns> the child view </returns>
			Protected Friend Overrides Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View
				Return Nothing
			End Function

			''' <summary>
			''' Returns the allocation for a given child.
			''' Implemented to do nothing, as the logical view doesn't
			''' perform layout on the children.
			''' </summary>
			''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
			''' <param name="a">  the allocation to the interior of the box on entry,
			'''   and the allocation of the child view at the index on exit. </param>
			Protected Friend Overrides Sub childAllocation(ByVal index As Integer, ByVal a As Rectangle)
			End Sub
		End Class


	End Class

End Namespace