Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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
	''' A box that does layout asynchronously.  This
	''' is useful to keep the GUI event thread moving by
	''' not doing any layout on it.  The layout is done
	''' on a granularity of operations on the child views.
	''' After each child view is accessed for some part
	''' of layout (a potentially time consuming operation)
	''' the remaining tasks can be abandoned or a new higher
	''' priority task (i.e. to service a synchronous request
	''' or a visible area) can be taken on.
	''' <p>
	''' While the child view is being accessed
	''' a read lock is acquired on the associated document
	''' so that the model is stable while being accessed.
	''' 
	''' @author  Timothy Prinzing
	''' @since   1.3
	''' </summary>
	Public Class AsyncBoxView
		Inherits View

		''' <summary>
		''' Construct a box view that does asynchronous layout.
		''' </summary>
		''' <param name="elem"> the element of the model to represent </param>
		''' <param name="axis"> the axis to tile along.  This can be
		'''  either X_AXIS or Y_AXIS. </param>
		Public Sub New(ByVal elem As Element, ByVal axis As Integer)
			MyBase.New(elem)
			stats = New List(Of ChildState)
			Me.axis = axis
			locator = New ChildLocator(Me)
			flushTask = New FlushTask(Me)
			minorSpan = Short.MaxValue
			estimatedMajorSpan = False
		End Sub

		''' <summary>
		''' Fetch the major axis (the axis the children
		''' are tiled along).  This will have a value of
		''' either X_AXIS or Y_AXIS.
		''' </summary>
		Public Overridable Property majorAxis As Integer
			Get
				Return axis
			End Get
		End Property

		''' <summary>
		''' Fetch the minor axis (the axis orthogonal
		''' to the tiled axis).  This will have a value of
		''' either X_AXIS or Y_AXIS.
		''' </summary>
		Public Overridable Property minorAxis As Integer
			Get
				Return If(axis = X_AXIS, Y_AXIS, X_AXIS)
			End Get
		End Property

		''' <summary>
		''' Get the top part of the margin around the view.
		''' </summary>
		Public Overridable Property topInset As Single
			Get
				Return topInset
			End Get
			Set(ByVal i As Single)
				topInset = i
			End Set
		End Property


		''' <summary>
		''' Get the bottom part of the margin around the view.
		''' </summary>
		Public Overridable Property bottomInset As Single
			Get
				Return bottomInset
			End Get
			Set(ByVal i As Single)
				bottomInset = i
			End Set
		End Property


		''' <summary>
		''' Get the left part of the margin around the view.
		''' </summary>
		Public Overridable Property leftInset As Single
			Get
				Return leftInset
			End Get
			Set(ByVal i As Single)
				leftInset = i
			End Set
		End Property


		''' <summary>
		''' Get the right part of the margin around the view.
		''' </summary>
		Public Overridable Property rightInset As Single
			Get
				Return rightInset
			End Get
			Set(ByVal i As Single)
				rightInset = i
			End Set
		End Property


		''' <summary>
		''' Fetch the span along an axis that is taken up by the insets.
		''' </summary>
		''' <param name="axis"> the axis to determine the total insets along,
		'''  either X_AXIS or Y_AXIS.
		''' @since 1.4 </param>
		Protected Friend Overridable Function getInsetSpan(ByVal axis As Integer) As Single
			Dim margin As Single = If(axis = X_AXIS, leftInset + rightInset, topInset + bottomInset)
			Return margin
		End Function

		''' <summary>
		''' Set the estimatedMajorSpan property that determines if the
		''' major span should be treated as being estimated.  If this
		''' property is true, the value of setSize along the major axis
		''' will change the requirements along the major axis and incremental
		''' changes will be ignored until all of the children have been updated
		''' (which will cause the property to automatically be set to false).
		''' If the property is false the value of the majorSpan will be
		''' considered to be accurate and incremental changes will be
		''' added into the total as they are calculated.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Property estimatedMajorSpan As Boolean
			Set(ByVal isEstimated As Boolean)
				estimatedMajorSpan = isEstimated
			End Set
			Get
				Return estimatedMajorSpan
			End Get
		End Property


		''' <summary>
		''' Fetch the object representing the layout state of
		''' of the child at the given index.
		''' </summary>
		''' <param name="index"> the child index.  This should be a
		'''   value &gt;= 0 and &lt; getViewCount(). </param>
		Protected Friend Overridable Function getChildState(ByVal index As Integer) As ChildState
			SyncLock stats
				If (index >= 0) AndAlso (index < stats.Count) Then Return stats(index)
				Return Nothing
			End SyncLock
		End Function

		''' <summary>
		''' Fetch the queue to use for layout.
		''' </summary>
		Protected Friend Overridable Property layoutQueue As LayoutQueue
			Get
				Return LayoutQueue.defaultQueue
			End Get
		End Property

		''' <summary>
		''' New ChildState records are created through
		''' this method to allow subclasses the extend
		''' the ChildState records to do/hold more
		''' </summary>
		Protected Friend Overridable Function createChildState(ByVal v As View) As ChildState
			Return New ChildState(Me, v)
		End Function

		''' <summary>
		''' Requirements changed along the major axis.
		''' This is called by the thread doing layout for
		''' the given ChildState object when it has completed
		''' fetching the child views new preferences.
		''' Typically this would be the layout thread, but
		''' might be the event thread if it is trying to update
		''' something immediately (such as to perform a
		''' model/view translation).
		''' <p>
		''' This is implemented to mark the major axis as having
		''' changed so that a future check to see if the requirements
		''' need to be published to the parent view will consider
		''' the major axis.  If the span along the major axis is
		''' not estimated, it is updated by the given delta to reflect
		''' the incremental change.  The delta is ignored if the
		''' major span is estimated.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub majorRequirementChange(ByVal cs As ChildState, ByVal delta As Single)
			If estimatedMajorSpan = False Then majorSpan += delta
			majorChanged = True
		End Sub

		''' <summary>
		''' Requirements changed along the minor axis.
		''' This is called by the thread doing layout for
		''' the given ChildState object when it has completed
		''' fetching the child views new preferences.
		''' Typically this would be the layout thread, but
		''' might be the GUI thread if it is trying to update
		''' something immediately (such as to perform a
		''' model/view translation).
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub minorRequirementChange(ByVal cs As ChildState)
			minorChanged = True
		End Sub

		''' <summary>
		''' Publish the changes in preferences upward to the parent
		''' view.  This is normally called by the layout thread.
		''' </summary>
		Protected Friend Overridable Sub flushRequirementChanges()
			Dim doc As AbstractDocument = CType(document, AbstractDocument)
			Try
				doc.readLock()

				Dim ___parent As View = Nothing
				Dim horizontal As Boolean = False
				Dim vertical As Boolean = False

				SyncLock Me
					' perform tasks that iterate over the children while
					' preventing the collection from changing.
					SyncLock stats
						Dim n As Integer = viewCount
						If (n > 0) AndAlso (minorChanged OrElse estimatedMajorSpan) Then
							Dim q As LayoutQueue = layoutQueue
							Dim min As ChildState = getChildState(0)
							Dim pref As ChildState = getChildState(0)
							Dim span As Single = 0f
							For i As Integer = 1 To n - 1
								Dim cs As ChildState = getChildState(i)
								If minorChanged Then
									If cs.min > min.min Then min = cs
									If cs.pref > pref.pref Then pref = cs
								End If
								If estimatedMajorSpan Then span += cs.majorSpan
							Next i

							If minorChanged Then
								minRequest = min
								prefRequest = pref
							End If
							If estimatedMajorSpan Then
								majorSpan = span
								estimatedMajorSpan = False
								majorChanged = True
							End If
						End If
					End SyncLock

					' message preferenceChanged
					If majorChanged OrElse minorChanged Then
						___parent = parent
						If ___parent IsNot Nothing Then
							If axis = X_AXIS Then
								horizontal = majorChanged
								vertical = minorChanged
							Else
								vertical = majorChanged
								horizontal = minorChanged
							End If
						End If
						majorChanged = False
						minorChanged = False
					End If
				End SyncLock

				' propagate a preferenceChanged, using the
				' layout thread.
				If ___parent IsNot Nothing Then
					___parent.preferenceChanged(Me, horizontal, vertical)

					' probably want to change this to be more exact.
					Dim c As Component = container
					If c IsNot Nothing Then c.repaint()
				End If
			Finally
				doc.readUnlock()
			End Try
		End Sub

		''' <summary>
		''' Calls the superclass to update the child views, and
		''' updates the status records for the children.  This
		''' is expected to be called while a write lock is held
		''' on the model so that interaction with the layout
		''' thread will not happen (i.e. the layout thread
		''' acquires a read lock before doing anything).
		''' </summary>
		''' <param name="offset"> the starting offset into the child views &gt;= 0 </param>
		''' <param name="length"> the number of existing views to replace &gt;= 0 </param>
		''' <param name="views"> the child views to insert </param>
		Public Overrides Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal views As View())
			SyncLock stats
				' remove the replaced state records
				For i As Integer = 0 To length - 1
					Dim cs As ChildState = stats.Remove(offset)
					Dim csSpan As Single = cs.majorSpan

					cs.childView.parent = Nothing
					If csSpan <> 0 Then majorRequirementChange(cs, -csSpan)
				Next i

				' insert the state records for the new children
				Dim q As LayoutQueue = layoutQueue
				If views IsNot Nothing Then
					For i As Integer = 0 To views.Length - 1
						Dim s As ChildState = createChildState(views(i))
						stats.Insert(offset + i, s)
						q.addTask(s)
					Next i
				End If

				' notify that the size changed
				q.addTask(flushTask)
			End SyncLock
		End Sub

		''' <summary>
		''' Loads all of the children to initialize the view.
		''' This is called by the <seealso cref="#setParent setParent"/>
		''' method.  Subclasses can reimplement this to initialize
		''' their child views in a different manner.  The default
		''' implementation creates a child view for each
		''' child element.
		''' <p>
		''' Normally a write-lock is held on the Document while
		''' the children are being changed, which keeps the rendering
		''' and layout threads safe.  The exception to this is when
		''' the view is initialized to represent an existing element
		''' (via this method), so it is synchronized to exclude
		''' preferenceChanged while we are initializing.
		''' </summary>
		''' <param name="f"> the view factory </param>
		''' <seealso cref= #setParent </seealso>
		Protected Friend Overridable Sub loadChildren(ByVal f As ViewFactory)
			Dim e As Element = element
			Dim n As Integer = e.elementCount
			If n > 0 Then
				Dim added As View() = New View(n - 1){}
				For i As Integer = 0 To n - 1
					added(i) = f.create(e.getElement(i))
				Next i
				replace(0, 0, added)
			End If
		End Sub

		''' <summary>
		''' Fetches the child view index representing the given position in
		''' the model.  This is implemented to fetch the view in the case
		''' where there is a child view for each child element.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Function getViewIndexAtPosition(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
			Dim isBackward As Boolean = (b Is Position.Bias.Backward)
			pos = If(isBackward, Math.Max(0, pos - 1), pos)
			Dim elem As Element = element
			Return elem.getElementIndex(pos)
		End Function

		''' <summary>
		''' Update the layout in response to receiving notification of
		''' change from the model.  This is implemented to note the
		''' change on the ChildLocator so that offsets of the children
		''' will be correctly computed.
		''' </summary>
		''' <param name="ec"> changes to the element this view is responsible
		'''  for (may be null if there were no changes). </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <seealso cref= #insertUpdate </seealso>
		''' <seealso cref= #removeUpdate </seealso>
		''' <seealso cref= #changedUpdate </seealso>
		Protected Friend Overridable Sub updateLayout(ByVal ec As javax.swing.event.DocumentEvent.ElementChange, ByVal e As javax.swing.event.DocumentEvent, ByVal a As Shape)
			If ec IsNot Nothing Then
				' the newly inserted children don't have a valid
				' offset so the child locator needs to be messaged
				' that the child prior to the new children has
				' changed size.
				Dim index As Integer = Math.Max(ec.index - 1, 0)
				Dim cs As ChildState = getChildState(index)
				locator.childChanged(cs)
			End If
		End Sub

		' --- View methods ------------------------------------

		''' <summary>
		''' Sets the parent of the view.
		''' This is reimplemented to provide the superclass
		''' behavior as well as calling the <code>loadChildren</code>
		''' method if this view does not already have children.
		''' The children should not be loaded in the
		''' constructor because the act of setting the parent
		''' may cause them to try to search up the hierarchy
		''' (to get the hosting Container for example).
		''' If this view has children (the view is being moved
		''' from one place in the view hierarchy to another),
		''' the <code>loadChildren</code> method will not be called.
		''' </summary>
		''' <param name="parent"> the parent of the view, null if none </param>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				MyBase.parent = parent
				If (parent IsNot Nothing) AndAlso (viewCount = 0) Then
					Dim f As ViewFactory = viewFactory
					loadChildren(f)
				End If
			End Set
		End Property

		''' <summary>
		''' Child views can call this on the parent to indicate that
		''' the preference has changed and should be reconsidered
		''' for layout.  This is reimplemented to queue new work
		''' on the layout thread.  This method gets messaged from
		''' multiple threads via the children.
		''' </summary>
		''' <param name="child"> the child view </param>
		''' <param name="width"> true if the width preference has changed </param>
		''' <param name="height"> true if the height preference has changed </param>
		''' <seealso cref= javax.swing.JComponent#revalidate </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub preferenceChanged(ByVal child As View, ByVal width As Boolean, ByVal height As Boolean)
			If child Is Nothing Then
				parent.preferenceChanged(Me, width, height)
			Else
				If changing IsNot Nothing Then
					Dim cv As View = changing.childView
					If cv Is child Then
						' size was being changed on the child, no need to
						' queue work for it.
						changing.preferenceChanged(width, height)
						Return
					End If
				End If
				Dim index As Integer = getViewIndex(child.startOffset, Position.Bias.Forward)
				Dim cs As ChildState = getChildState(index)
				cs.preferenceChanged(width, height)
				Dim q As LayoutQueue = layoutQueue
				q.addTask(cs)
				q.addTask(flushTask)
			End If
		End Sub

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view if the view caches any layout
		''' information.
		''' <p>
		''' Since the major axis is updated asynchronously and should be
		''' the sum of the tiled children the call is ignored for the major
		''' axis.  Since the minor axis is flexible, work is queued to resize
		''' the children if the minor span changes.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
			spanOnAxisxis(X_AXIS, width)
			spanOnAxisxis(Y_AXIS, height)
		End Sub

		''' <summary>
		''' Retrieves the size of the view along an axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns> the current span of the view along the given axis, >= 0 </returns>
		Friend Overridable Function getSpanOnAxis(ByVal axis As Integer) As Single
			If axis = majorAxis Then Return majorSpan
			Return minorSpan
		End Function

		''' <summary>
		''' Sets the size of the view along an axis.  Since the major
		''' axis is updated asynchronously and should be the sum of the
		''' tiled children the call is ignored for the major axis.  Since
		''' the minor axis is flexible, work is queued to resize the
		''' children if the minor span changes.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <param name="span"> the span to layout to >= 0 </param>
		Friend Overridable Sub setSpanOnAxis(ByVal axis As Integer, ByVal span As Single)
			Dim margin As Single = getInsetSpan(axis)
			If axis = minorAxis Then
				Dim targetSpan As Single = span - margin
				If targetSpan <> minorSpan Then
					minorSpan = targetSpan

					' mark all of the ChildState instances as needing to
					' resize the child, and queue up work to fix them.
					Dim n As Integer = viewCount
					If n <> 0 Then
						Dim q As LayoutQueue = layoutQueue
						For i As Integer = 0 To n - 1
							Dim cs As ChildState = getChildState(i)
							cs.childSizeValid = False
							q.addTask(cs)
						Next i
						q.addTask(flushTask)
					End If
				End If
			Else
				' along the major axis the value is ignored
				' unless the estimatedMajorSpan property is
				' true.
				If estimatedMajorSpan Then majorSpan = span - margin
			End If
		End Sub

		''' <summary>
		''' Render the view using the given allocation and
		''' rendering surface.
		''' <p>
		''' This is implemented to determine whether or not the
		''' desired region to be rendered (i.e. the unclipped
		''' area) is up to date or not.  If up-to-date the children
		''' are rendered.  If not up-to-date, a task to build
		''' the desired area is placed on the layout queue as
		''' a high priority task.  This keeps by event thread
		''' moving by rendering if ready, and postponing until
		''' a later time if not ready (since paint requests
		''' can be rescheduled).
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="alloc"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal alloc As Shape)
			SyncLock locator
				locator.allocation = alloc
				locator.paintChildren(g)
			End SyncLock
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			Dim margin As Single = getInsetSpan(axis)
			If axis = Me.axis Then Return majorSpan + margin
			If prefRequest IsNot Nothing Then
				Dim child As View = prefRequest.childView
				Return child.getPreferredSpan(axis) + margin
			End If

			' nothing is known about the children yet
			Return margin + 30
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>  the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			If axis = Me.axis Then Return getPreferredSpan(axis)
			If minRequest IsNot Nothing Then
				Dim child As View = minRequest.childView
				Return child.getMinimumSpan(axis)
			End If

			' nothing is known about the children yet
			If axis = X_AXIS Then
				Return leftInset + rightInset + 5
			Else
				Return topInset + bottomInset + 5
			End If
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis type </exception>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If axis = Me.axis Then Return getPreferredSpan(axis)
			Return Integer.MaxValue
		End Function


		''' <summary>
		''' Returns the number of views in this view.  Since
		''' the default is to not be a composite view this
		''' returns 0.
		''' </summary>
		''' <returns> the number of views &gt;= 0 </returns>
		''' <seealso cref= View#getViewCount </seealso>
		Public Property Overrides viewCount As Integer
			Get
				SyncLock stats
					Return stats.Count
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Gets the nth child view.  Since there are no
		''' children by default, this returns null.
		''' </summary>
		''' <param name="n"> the number of the view to get, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <returns> the view </returns>
		Public Overrides Function getView(ByVal n As Integer) As View
			Dim cs As ChildState = getChildState(n)
			If cs IsNot Nothing Then Return cs.childView
			Return Nothing
		End Function

		''' <summary>
		''' Fetches the allocation for the given child view.
		''' This enables finding out where various views
		''' are located, without assuming the views store
		''' their location.  This returns null since the
		''' default is to not have any child views.
		''' </summary>
		''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <param name="a">  the allocation to this view. </param>
		''' <returns> the allocation to the child </returns>
		Public Overrides Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
			Dim ca As Shape = locator.getChildAllocation(index, a)
			Return ca
		End Function

		''' <summary>
		''' Returns the child view index representing the given position in
		''' the model.  By default a view has no children so this is implemented
		''' to return -1 to indicate there is no valid child index for any
		''' position.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position
		''' @since 1.3 </returns>
		Public Overrides Function getViewIndex(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
			Return getViewIndexAtPosition(pos, b)
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="b"> the bias toward the previous character or the
		'''  next character represented by the offset, in case the
		'''  position is a boundary of two views. </param>
		''' <returns> the bounding box of the given position is returned </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''   not represent a valid location in the associated document </exception>
		''' <exception cref="IllegalArgumentException"> for an invalid bias argument </exception>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim index As Integer = getViewIndex(pos, b)
			Dim ca As Shape = locator.getChildAllocation(index, a)

			' forward to the child view, and make sure we don't
			' interact with the layout thread by synchronizing
			' on the child state.
			Dim cs As ChildState = getChildState(index)
			SyncLock cs
				Dim cv As View = cs.childView
				Dim v As Shape = cv.modelToView(pos, ca, b)
				Return v
			End SyncLock
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.  The biasReturn argument will be
		''' filled in to indicate that the point given is closer to the next
		''' character in the model or the previous character in the model.
		''' <p>
		''' This is expected to be called by the GUI thread, holding a
		''' read-lock on the associated model.  It is implemented to
		''' locate the child view and determine it's allocation with a
		''' lock on the ChildLocator object, and to call viewToModel
		''' on the child view with a lock on the ChildState object
		''' to avoid interaction with the layout thread.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0.  The biasReturn argument will be
		''' filled in to indicate that the point given is closer to the next
		''' character in the model or the previous character in the model. </returns>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer
			Dim pos As Integer ' return position
			Dim index As Integer ' child index to forward to
			Dim ca As Shape ' child allocation

			' locate the child view and it's allocation so that
			' we can forward to it.  Make sure the layout thread
			' doesn't change anything by trying to flush changes
			' to the parent while the GUI thread is trying to
			' find the child and it's allocation.
			SyncLock locator
				index = locator.getViewIndexAtPoint(x, y, a)
				ca = locator.getChildAllocation(index, a)
			End SyncLock

			' forward to the child view, and make sure we don't
			' interact with the layout thread by synchronizing
			' on the child state.
			Dim cs As ChildState = getChildState(index)
			SyncLock cs
				Dim v As View = cs.childView
				pos = v.viewToModel(x, y, ca, biasReturn)
			End SyncLock
			Return pos
		End Function

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be visible,
		''' they might not be in the same order found in the model, or they just
		''' might not allow access to some of the locations in the model.
		''' This method enables specifying a position to convert
		''' within the range of &gt;=0.  If the value is -1, a position
		''' will be calculated automatically.  If the value &lt; -1,
		''' the {@code BadLocationException} will be thrown.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard;
		'''  this may be one of the following:
		'''  <ul style="list-style-type:none">
		'''  <li><code>SwingConstants.WEST</code></li>
		'''  <li><code>SwingConstants.EAST</code></li>
		'''  <li><code>SwingConstants.NORTH</code></li>
		'''  <li><code>SwingConstants.SOUTH</code></li>
		'''  </ul> </param>
		''' <param name="biasRet"> an array contain the bias that was checked </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> the given position is not a valid
		'''                                 position within the document </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code> is invalid </exception>
		Public Overrides Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			If pos < -1 Then Throw New BadLocationException("invalid position", pos)
			Return Utilities.getNextVisualPositionFrom(Me, pos, b, a, direction, biasRet)
		End Function

		' --- variables -----------------------------------------

		''' <summary>
		''' The major axis against which the children are
		''' tiled.
		''' </summary>
		Friend axis As Integer

		''' <summary>
		''' The children and their layout statistics.
		''' </summary>
		Friend stats As IList(Of ChildState)

		''' <summary>
		''' Current span along the major axis.  This
		''' is also the value returned by getMinimumSize,
		''' getPreferredSize, and getMaximumSize along
		''' the major axis.
		''' </summary>
		Friend majorSpan As Single

		''' <summary>
		''' Is the span along the major axis estimated?
		''' </summary>
		Friend estimatedMajorSpan As Boolean

		''' <summary>
		''' Current span along the minor axis.  This
		''' is what layout was done against (i.e. things
		''' are flexible in this direction).
		''' </summary>
		Friend minorSpan As Single

		''' <summary>
		''' Object that manages the offsets of the
		''' children.  All locking for management of
		''' child locations is on this object.
		''' </summary>
		Protected Friend locator As ChildLocator

		Friend topInset As Single
		Friend bottomInset As Single
		Friend leftInset As Single
		Friend rightInset As Single

		Friend minRequest As ChildState
		Friend prefRequest As ChildState
		Friend majorChanged As Boolean
		Friend minorChanged As Boolean
		Friend flushTask As Runnable

		''' <summary>
		''' Child that is actively changing size.  This often
		''' causes a preferenceChanged, so this is a cache to
		''' possibly speed up the marking the state.  It also
		''' helps flag an opportunity to avoid adding to flush
		''' task to the layout queue.
		''' </summary>
		Friend changing As ChildState

		''' <summary>
		''' A class to manage the effective position of the
		''' child views in a localized area while changes are
		''' being made around the localized area.  The AsyncBoxView
		''' may be continuously changing, but the visible area
		''' needs to remain fairly stable until the layout thread
		''' decides to publish an update to the parent.
		''' @since 1.3
		''' </summary>
		Public Class ChildLocator
			Private ReadOnly outerInstance As AsyncBoxView


			''' <summary>
			''' construct a child locator.
			''' </summary>
			Public Sub New(ByVal outerInstance As AsyncBoxView)
					Me.outerInstance = outerInstance
				lastAlloc = New Rectangle
				childAlloc = New Rectangle
			End Sub

			''' <summary>
			''' Notification that a child changed.  This can effect
			''' whether or not new offset calculations are needed.
			''' This is called by a ChildState object that has
			''' changed it's major span.  This can therefore be
			''' called by multiple threads.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub childChanged(ByVal cs As ChildState)
				If lastValidOffset Is Nothing Then
					lastValidOffset = cs
				ElseIf cs.childView.startOffset < lastValidOffset.childView.startOffset Then
					lastValidOffset = cs
				End If
			End Sub

			''' <summary>
			''' Paint the children that intersect the clip area.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Sub paintChildren(ByVal g As Graphics)
				Dim clip As Rectangle = g.clipBounds
				Dim targetOffset As Single = If(outerInstance.axis = X_AXIS, clip.x - lastAlloc.x, clip.y - lastAlloc.y)
				Dim index As Integer = getViewIndexAtVisualOffset(targetOffset)
				Dim n As Integer = outerInstance.viewCount
				Dim offs As Single = outerInstance.getChildState(index).majorOffset
				For i As Integer = index To n - 1
					Dim cs As ChildState = outerInstance.getChildState(i)
					cs.majorOffset = offs
					Dim ca As Shape = getChildAllocation(i)
					If intersectsClip(ca, clip) Then
						SyncLock cs
							Dim v As View = cs.childView
							v.paint(g, ca)
						End SyncLock
					Else
						' done painting intersection
						Exit For
					End If
					offs += cs.majorSpan
				Next i
			End Sub

			''' <summary>
			''' Fetch the allocation to use for a child view.
			''' This will update the offsets for all children
			''' not yet updated before the given index.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overridable Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
				If a Is Nothing Then Return Nothing
				allocation = a
				Dim cs As ChildState = outerInstance.getChildState(index)
				If lastValidOffset Is Nothing Then lastValidOffset = outerInstance.getChildState(0)
				If cs.childView.startOffset > lastValidOffset.childView.startOffset Then updateChildOffsetsToIndex(index)
				Dim ca As Shape = getChildAllocation(index)
				Return ca
			End Function

			''' <summary>
			''' Fetches the child view index at the given point.
			''' This is called by the various View methods that
			''' need to calculate which child to forward a message
			''' to.  This should be called by a block synchronized
			''' on this object, and would typically be followed
			''' with one or more calls to getChildAllocation that
			''' should also be in the synchronized block.
			''' </summary>
			''' <param name="x"> the X coordinate &gt;= 0 </param>
			''' <param name="y"> the Y coordinate &gt;= 0 </param>
			''' <param name="a"> the allocation to the View </param>
			''' <returns> the nearest child index </returns>
			Public Overridable Function getViewIndexAtPoint(ByVal x As Single, ByVal y As Single, ByVal a As Shape) As Integer
				allocation = a
				Dim targetOffset As Single = If(outerInstance.axis = X_AXIS, x - lastAlloc.x, y - lastAlloc.y)
				Dim index As Integer = getViewIndexAtVisualOffset(targetOffset)
				Return index
			End Function

			''' <summary>
			''' Fetch the allocation to use for a child view.
			''' <em>This does not update the offsets in the ChildState
			''' records.</em>
			''' </summary>
			Protected Friend Overridable Function getChildAllocation(ByVal index As Integer) As Shape
				Dim cs As ChildState = outerInstance.getChildState(index)
				If Not cs.layoutValid Then cs.run()
				If outerInstance.axis = X_AXIS Then
					childAlloc.x = lastAlloc.x + CInt(Fix(cs.majorOffset))
					childAlloc.y = lastAlloc.y + CInt(Fix(cs.minorOffset))
					childAlloc.width = CInt(Fix(cs.majorSpan))
					childAlloc.height = CInt(Fix(cs.minorSpan))
				Else
					childAlloc.y = lastAlloc.y + CInt(Fix(cs.majorOffset))
					childAlloc.x = lastAlloc.x + CInt(Fix(cs.minorOffset))
					childAlloc.height = CInt(Fix(cs.majorSpan))
					childAlloc.width = CInt(Fix(cs.minorSpan))
				End If
				childAlloc.x += CInt(Fix(outerInstance.leftInset))
				childAlloc.y += CInt(Fix(outerInstance.rightInset))
				Return childAlloc
			End Function

			''' <summary>
			''' Copy the currently allocated shape into the Rectangle
			''' used to store the current allocation.  This would be
			''' a floating point rectangle in a Java2D-specific implementation.
			''' </summary>
			Protected Friend Overridable Property allocation As Shape
				Set(ByVal a As Shape)
					If TypeOf a Is Rectangle Then
						lastAlloc.bounds = CType(a, Rectangle)
					Else
						lastAlloc.bounds = a.bounds
					End If
					outerInstance.sizeize(lastAlloc.width, lastAlloc.height)
				End Set
			End Property

			''' <summary>
			''' Locate the view responsible for an offset into the box
			''' along the major axis.  Make sure that offsets are set
			''' on the ChildState objects up to the given target span
			''' past the desired offset.
			''' </summary>
			''' <returns>   index of the view representing the given visual
			'''   location (targetOffset), or -1 if no view represents
			'''   that location </returns>
			Protected Friend Overridable Function getViewIndexAtVisualOffset(ByVal targetOffset As Single) As Integer
				Dim n As Integer = outerInstance.viewCount
				If n > 0 Then
					Dim lastValid As Boolean = (lastValidOffset IsNot Nothing)

					If lastValidOffset Is Nothing Then lastValidOffset = outerInstance.getChildState(0)
					If targetOffset > outerInstance.majorSpan Then
						' should only get here on the first time display.
						If Not lastValid Then Return 0
						Dim pos As Integer = lastValidOffset.childView.startOffset
						Dim index As Integer = outerInstance.getViewIndex(pos, Position.Bias.Forward)
						Return index
					ElseIf targetOffset > lastValidOffset.majorOffset Then
						' roll offset calculations forward
						Return updateChildOffsets(targetOffset)
					Else
						' no changes prior to the needed offset
						' this should be a binary search
						Dim offs As Single = 0f
						For i As Integer = 0 To n - 1
							Dim cs As ChildState = outerInstance.getChildState(i)
							Dim nextOffs As Single = offs + cs.majorSpan
							If targetOffset < nextOffs Then Return i
							offs = nextOffs
						Next i
					End If
				End If
				Return n - 1
			End Function

			''' <summary>
			''' Move the location of the last offset calculation forward
			''' to the desired offset.
			''' </summary>
			Friend Overridable Function updateChildOffsets(ByVal targetOffset As Single) As Integer
				Dim n As Integer = outerInstance.viewCount
				Dim targetIndex As Integer = n - 1
				Dim pos As Integer = lastValidOffset.childView.startOffset
				Dim startIndex As Integer = outerInstance.getViewIndex(pos, Position.Bias.Forward)
				Dim start As Single = lastValidOffset.majorOffset
				Dim lastOffset As Single = start
				For i As Integer = startIndex To n - 1
					Dim cs As ChildState = outerInstance.getChildState(i)
					cs.majorOffset = lastOffset
					lastOffset += cs.majorSpan
					If targetOffset < lastOffset Then
						targetIndex = i
						lastValidOffset = cs
						Exit For
					End If
				Next i

				Return targetIndex
			End Function

			''' <summary>
			''' Move the location of the last offset calculation forward
			''' to the desired index.
			''' </summary>
			Friend Overridable Sub updateChildOffsetsToIndex(ByVal index As Integer)
				Dim pos As Integer = lastValidOffset.childView.startOffset
				Dim startIndex As Integer = outerInstance.getViewIndex(pos, Position.Bias.Forward)
				Dim lastOffset As Single = lastValidOffset.majorOffset
				For i As Integer = startIndex To index
					Dim cs As ChildState = outerInstance.getChildState(i)
					cs.majorOffset = lastOffset
					lastOffset += cs.majorSpan
				Next i
			End Sub

			Friend Overridable Function intersectsClip(ByVal childAlloc As Shape, ByVal clip As Rectangle) As Boolean
				Dim cs As Rectangle = If(TypeOf childAlloc Is Rectangle, CType(childAlloc, Rectangle), childAlloc.bounds)
				If cs.intersects(clip) Then Return lastAlloc.intersects(cs)
				Return False
			End Function

			''' <summary>
			''' The location of the last offset calculation
			''' that is valid.
			''' </summary>
			Protected Friend lastValidOffset As ChildState

			''' <summary>
			''' The last seen allocation (for repainting when changes
			''' are flushed upward).
			''' </summary>
			Protected Friend lastAlloc As Rectangle

			''' <summary>
			''' A shape to use for the child allocation to avoid
			''' creating a lot of garbage.
			''' </summary>
			Protected Friend childAlloc As Rectangle
		End Class

		''' <summary>
		''' A record representing the layout state of a
		''' child view.  It is runnable as a task on another
		''' thread.  All access to the child view that is
		''' based upon a read-lock on the model should synchronize
		''' on this object (i.e. The layout thread and the GUI
		''' thread can both have a read lock on the model at the
		''' same time and are not protected from each other).
		''' Access to a child view hierarchy is serialized via
		''' synchronization on the ChildState instance.
		''' @since 1.3
		''' </summary>
		Public Class ChildState
			Implements Runnable

			Private ReadOnly outerInstance As AsyncBoxView


			''' <summary>
			''' Construct a child status.  This needs to start
			''' out as fairly large so we don't falsely begin with
			''' the idea that all of the children are visible.
			''' @since 1.4
			''' </summary>
			Public Sub New(ByVal outerInstance As AsyncBoxView, ByVal v As View)
					Me.outerInstance = outerInstance
				child = v
				minorValid = False
				majorValid = False
				childSizeValid = False
				child.parent = AsyncBoxView.this
			End Sub

			''' <summary>
			''' Fetch the child view this record represents
			''' </summary>
			Public Overridable Property childView As View
				Get
					Return child
				End Get
			End Property

			''' <summary>
			''' Update the child state.  This should be
			''' called by the thread that desires to spend
			''' time updating the child state (intended to
			''' be the layout thread).
			''' <p>
			''' This acquires a read lock on the associated
			''' document for the duration of the update to
			''' ensure the model is not changed while it is
			''' operating.  The first thing to do would be
			''' to see if any work actually needs to be done.
			''' The following could have conceivably happened
			''' while the state was waiting to be updated:
			''' <ol>
			''' <li>The child may have been removed from the
			''' view hierarchy.
			''' <li>The child may have been updated by a
			''' higher priority operation (i.e. the child
			''' may have become visible).
			''' </ol>
			''' </summary>
			Public Overridable Sub run()
				Dim doc As AbstractDocument = CType(outerInstance.document, AbstractDocument)
				Try
					doc.readLock()
					If minorValid AndAlso majorValid AndAlso childSizeValid Then Return
					If child.parent Is AsyncBoxView.this Then
						' this may overwrite anothers threads cached
						' value for actively changing... but that just
						' means it won't use the cache if there is an
						' overwrite.
						SyncLock AsyncBoxView.this
							outerInstance.changing = Me
						End SyncLock
						updateChild()
						SyncLock AsyncBoxView.this
							outerInstance.changing = Nothing
						End SyncLock

						' setting the child size on the minor axis
						' may have caused it to change it's preference
						' along the major axis.
						updateChild()
					End If
				Finally
					doc.readUnlock()
				End Try
			End Sub

			Friend Overridable Sub updateChild()
				Dim minorUpdated As Boolean = False
				SyncLock Me
					If Not minorValid Then
						Dim minorAxis As Integer = outerInstance.minorAxis
						min = child.getMinimumSpan(minorAxis)
						pref = child.getPreferredSpan(minorAxis)
						max = child.getMaximumSpan(minorAxis)
						minorValid = True
						minorUpdated = True
					End If
				End SyncLock
				If minorUpdated Then outerInstance.minorRequirementChange(Me)

				Dim majorUpdated As Boolean = False
				Dim delta As Single = 0.0f
				SyncLock Me
					If Not majorValid Then
						Dim old As Single = span
						span = child.getPreferredSpan(outerInstance.axis)
						delta = span - old
						majorValid = True
						majorUpdated = True
					End If
				End SyncLock
				If majorUpdated Then
					outerInstance.majorRequirementChange(Me, delta)
					outerInstance.locator.childChanged(Me)
				End If

				SyncLock Me
					If Not childSizeValid Then
						Dim w As Single
						Dim h As Single
						If outerInstance.axis = X_AXIS Then
							w = span
							h = minorSpan
						Else
							w = minorSpan
							h = span
						End If
						childSizeValid = True
						child.sizeize(w, h)
					End If
				End SyncLock

			End Sub

			''' <summary>
			''' What is the span along the minor axis.
			''' </summary>
			Public Overridable Property minorSpan As Single
				Get
					If max < outerInstance.minorSpan Then Return max
					' make it the target width, or as small as it can get.
					Return Math.Max(min, outerInstance.minorSpan)
				End Get
			End Property

			''' <summary>
			''' What is the offset along the minor axis
			''' </summary>
			Public Overridable Property minorOffset As Single
				Get
					If max < outerInstance.minorSpan Then
						' can't make the child this wide, align it
						Dim align As Single = child.getAlignment(outerInstance.minorAxis)
						Return ((outerInstance.minorSpan - max) * align)
					End If
					Return 0f
				End Get
			End Property

			''' <summary>
			''' What is the span along the major axis.
			''' </summary>
			Public Overridable Property majorSpan As Single
				Get
					Return span
				End Get
			End Property

			''' <summary>
			''' Get the offset along the major axis
			''' </summary>
			Public Overridable Property majorOffset As Single
				Get
					Return offset
				End Get
				Set(ByVal offs As Single)
					offset = offs
				End Set
			End Property


			''' <summary>
			''' Mark preferences changed for this child.
			''' </summary>
			''' <param name="width"> true if the width preference has changed </param>
			''' <param name="height"> true if the height preference has changed </param>
			''' <seealso cref= javax.swing.JComponent#revalidate </seealso>
			Public Overridable Sub preferenceChanged(ByVal width As Boolean, ByVal height As Boolean)
				If outerInstance.axis = X_AXIS Then
					If width Then majorValid = False
					If height Then minorValid = False
				Else
					If width Then minorValid = False
					If height Then majorValid = False
				End If
				childSizeValid = False
			End Sub

			''' <summary>
			''' Has the child view been laid out.
			''' </summary>
			Public Overridable Property layoutValid As Boolean
				Get
					Return (minorValid AndAlso majorValid AndAlso childSizeValid)
				End Get
			End Property

			' minor axis
			Private min As Single
			Private pref As Single
			Private max As Single
			Private minorValid As Boolean

			' major axis
			Private span As Single
			Private offset As Single
			Private majorValid As Boolean

			Private child As View
			Private childSizeValid As Boolean
		End Class

		''' <summary>
		''' Task to flush requirement changes upward
		''' </summary>
		Friend Class FlushTask
			Implements Runnable

			Private ReadOnly outerInstance As AsyncBoxView

			Public Sub New(ByVal outerInstance As AsyncBoxView)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub run()
				outerInstance.flushRequirementChanges()
			End Sub

		End Class

	End Class

End Namespace