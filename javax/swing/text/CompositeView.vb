Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.event

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
	''' <code>CompositeView</code> is an abstract <code>View</code>
	''' implementation which manages one or more child views.
	''' (Note that <code>CompositeView</code> is intended
	''' for managing relatively small numbers of child views.)
	''' <code>CompositeView</code> is intended to be used as
	''' a starting point for <code>View</code> implementations,
	''' such as <code>BoxView</code>, that will contain child
	''' <code>View</code>s. Subclasses that wish to manage the
	''' collection of child <code>View</code>s should use the
	''' <seealso cref="#replace"/> method.  As <code>View</code> invokes
	''' <code>replace</code> during <code>DocumentListener</code>
	''' notification, you normally won't need to directly
	''' invoke <code>replace</code>.
	''' 
	''' <p>While <code>CompositeView</code>
	''' does not impose a layout policy on its child <code>View</code>s,
	''' it does allow for inseting the child <code>View</code>s
	''' it will contain.  The insets can be set by either
	''' <seealso cref="#setInsets"/> or <seealso cref="#setParagraphInsets"/>.
	''' 
	''' <p>In addition to the abstract methods of
	''' <seealso cref="javax.swing.text.View"/>,
	''' subclasses of <code>CompositeView</code> will need to
	''' override:
	''' <ul>
	''' <li><seealso cref="#isBefore"/> - Used to test if a given
	'''     <code>View</code> location is before the visual space
	'''     of the <code>CompositeView</code>.
	''' <li><seealso cref="#isAfter"/> - Used to test if a given
	'''     <code>View</code> location is after the visual space
	'''     of the <code>CompositeView</code>.
	''' <li><seealso cref="#getViewAtPoint"/> - Returns the view at
	'''     a given visual location.
	''' <li><seealso cref="#childAllocation"/> - Returns the bounds of
	'''     a particular child <code>View</code>.
	'''     <code>getChildAllocation</code> will invoke
	'''     <code>childAllocation</code> after offseting
	'''     the bounds by the <code>Inset</code>s of the
	'''     <code>CompositeView</code>.
	''' </ul>
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public MustInherit Class CompositeView
		Inherits View

		''' <summary>
		''' Constructs a <code>CompositeView</code> for the given element.
		''' </summary>
		''' <param name="elem">  the element this view is responsible for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			children = New View(0){}
			nchildren = 0
			childAlloc = New Rectangle
		End Sub

		''' <summary>
		''' Loads all of the children to initialize the view.
		''' This is called by the <seealso cref="#setParent"/>
		''' method.  Subclasses can reimplement this to initialize
		''' their child views in a different manner.  The default
		''' implementation creates a child view for each
		''' child element.
		''' </summary>
		''' <param name="f"> the view factory </param>
		''' <seealso cref= #setParent </seealso>
		Protected Friend Overridable Sub loadChildren(ByVal f As ViewFactory)
			If f Is Nothing Then Return
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

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Sets the parent of the view.
		''' This is reimplemented to provide the superclass
		''' behavior as well as calling the <code>loadChildren</code>
		''' method if this view does not already have children.
		''' The children should not be loaded in the
		''' constructor because the act of setting the parent
		''' may cause them to try to search up the hierarchy
		''' (to get the hosting <code>Container</code> for example).
		''' If this view has children (the view is being moved
		''' from one place in the view hierarchy to another),
		''' the <code>loadChildren</code> method will not be called.
		''' </summary>
		''' <param name="parent"> the parent of the view, <code>null</code> if none </param>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				MyBase.parent = parent
				If (parent IsNot Nothing) AndAlso (nchildren = 0) Then
					Dim f As ViewFactory = viewFactory
					loadChildren(f)
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the number of child views of this view.
		''' </summary>
		''' <returns> the number of views &gt;= 0 </returns>
		''' <seealso cref= #getView </seealso>
		Public Property Overrides viewCount As Integer
			Get
				Return nchildren
			End Get
		End Property

		''' <summary>
		''' Returns the n-th view in this container.
		''' </summary>
		''' <param name="n"> the number of the desired view, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <returns> the view at index <code>n</code> </returns>
		Public Overrides Function getView(ByVal n As Integer) As View
			Return children(n)
		End Function

		''' <summary>
		''' Replaces child views.  If there are no views to remove
		''' this acts as an insert.  If there are no views to
		''' add this acts as a remove.  Views being removed will
		''' have the parent set to <code>null</code>,
		''' and the internal reference to them removed so that they
		''' may be garbage collected.
		''' </summary>
		''' <param name="offset"> the starting index into the child views to insert
		'''   the new views; &gt;= 0 and &lt;= getViewCount </param>
		''' <param name="length"> the number of existing child views to remove;
		'''   this should be a value &gt;= 0 and &lt;= (getViewCount() - offset) </param>
		''' <param name="views"> the child views to add; this value can be
		'''  <code>null</code>
		'''   to indicate no children are being added (useful to remove) </param>
		Public Overrides Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal views As View())
			' make sure an array exists
			If views Is Nothing Then views = ZERO

			' update parent reference on removed views
			For i As Integer = offset To offset + length - 1
				If children(i).parent Is Me Then children(i).parent = Nothing
				children(i) = Nothing
			Next i

			' update the array
			Dim delta As Integer = views.Length - length
			Dim src As Integer = offset + length
			Dim nmove As Integer = nchildren - src
			Dim dest As Integer = src + delta
			If (nchildren + delta) >= children.Length Then
				' need to grow the array
				Dim newLength As Integer = Math.Max(2*children.Length, nchildren + delta)
				Dim newChildren As View() = New View(newLength - 1){}
				Array.Copy(children, 0, newChildren, 0, offset)
				Array.Copy(views, 0, newChildren, offset, views.Length)
				Array.Copy(children, src, newChildren, dest, nmove)
				children = newChildren
			Else
				' patch the existing array
				Array.Copy(children, src, children, dest, nmove)
				Array.Copy(views, 0, children, offset, views.Length)
			End If
			nchildren = nchildren + delta

			' update parent reference on added views
			For i As Integer = 0 To views.Length - 1
				views(i).parent = Me
			Next i
		End Sub

		''' <summary>
		''' Fetches the allocation for the given child view to
		''' render into. This enables finding out where various views
		''' are located.
		''' </summary>
		''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <param name="a">  the allocation to this view </param>
		''' <returns> the allocation to the child </returns>
		Public Overrides Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
			Dim alloc As Rectangle = getInsideAllocation(a)
			childAllocation(index, alloc)
			Return alloc
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="b"> a bias value of either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''   not represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim isBackward As Boolean = (b Is Position.Bias.Backward)
			Dim testPos As Integer = If(isBackward, Math.Max(0, pos - 1), pos)
			If isBackward AndAlso testPos < startOffset Then Return Nothing
			Dim vIndex As Integer = getViewIndexAtPosition(testPos)
			If (vIndex <> -1) AndAlso (vIndex < viewCount) Then
				Dim v As View = getView(vIndex)
				If v IsNot Nothing AndAlso testPos >= v.startOffset AndAlso testPos < v.endOffset Then
					Dim childShape As Shape = getChildAllocation(vIndex, a)
					If childShape Is Nothing Then Return Nothing
					Dim retShape As Shape = v.modelToView(pos, childShape, b)
					If retShape Is Nothing AndAlso v.endOffset = pos Then
						vIndex += 1
						If vIndex < viewCount Then
							v = getView(vIndex)
							retShape = v.modelToView(pos, getChildAllocation(vIndex, a), b)
						End If
					End If
					Return retShape
				End If
			End If
			Throw New BadLocationException("Position not represented by view", pos)
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="p0"> the position to convert &gt;= 0 </param>
		''' <param name="b0"> the bias toward the previous character or the
		'''  next character represented by p0, in case the
		'''  position is a boundary of two views; either
		'''  <code>Position.Bias.Forward</code> or
		'''  <code>Position.Bias.Backward</code> </param>
		''' <param name="p1"> the position to convert &gt;= 0 </param>
		''' <param name="b1"> the bias toward the previous character or the
		'''  next character represented by p1, in case the
		'''  position is a boundary of two views </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position is returned </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''   not represent a valid location in the associated document </exception>
		''' <exception cref="IllegalArgumentException"> for an invalid bias argument </exception>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function modelToView(ByVal p0 As Integer, ByVal b0 As Position.Bias, ByVal p1 As Integer, ByVal b1 As Position.Bias, ByVal a As Shape) As Shape
			If p0 = startOffset AndAlso p1 = endOffset Then Return a
			Dim alloc As Rectangle = getInsideAllocation(a)
			Dim r0 As New Rectangle(alloc)
			Dim v0 As View = getViewAtPosition(If(b0 Is Position.Bias.Backward, Math.Max(0, p0 - 1), p0), r0)
			Dim r1 As New Rectangle(alloc)
			Dim v1 As View = getViewAtPosition(If(b1 Is Position.Bias.Backward, Math.Max(0, p1 - 1), p1), r1)
			If v0 Is v1 Then
				If v0 Is Nothing Then Return a
				' Range contained in one view
				Return v0.modelToView(p0, b0, p1, b1, r0)
			End If
			' Straddles some views.
			Dim ___viewCount As Integer = viewCount
			Dim counter As Integer = 0
			Do While counter < ___viewCount
				Dim v As View
				' Views may not be in same order as model.
				' v0 or v1 may be null if there is a gap in the range this
				' view contains.
				v = getView(counter)
				If v Is v0 OrElse v Is v1 Then
					Dim endView As View
					Dim retRect As Rectangle
					Dim tempRect As New Rectangle
					If v Is v0 Then
						retRect = v0.modelToView(p0, b0, v0.endOffset, Position.Bias.Backward, r0).bounds
						endView = v1
					Else
						retRect = v1.modelToView(v1.startOffset, Position.Bias.Forward, p1, b1, r1).bounds
						endView = v0
					End If

					' Views entirely covered by range.
					counter += 1
					v = getView(counter)
					Do While counter < ___viewCount AndAlso v IsNot endView
						tempRect.bounds = alloc
						childAllocation(counter, tempRect)
						retRect.add(tempRect)
						counter += 1
						v = getView(counter)
					Loop

					' End view.
					If endView IsNot Nothing Then
						Dim endShape As Shape
						If endView Is v1 Then
							endShape = v1.modelToView(v1.startOffset, Position.Bias.Forward, p1, b1, r1)
						Else
							endShape = v0.modelToView(p0, b0, v0.endOffset, Position.Bias.Backward, r0)
						End If
						If TypeOf endShape Is Rectangle Then
							retRect.add(CType(endShape, Rectangle))
						Else
							retRect.add(endShape.bounds)
						End If
					End If
					Return retRect
				End If
				counter += 1
			Loop
			Throw New BadLocationException("Position not represented by view", p0)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x">   x coordinate of the view location to convert &gt;= 0 </param>
		''' <param name="y">   y coordinate of the view location to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="bias"> either <code>Position.Bias.Forward</code> or
		'''  <code>Position.Bias.Backward</code> </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Dim alloc As Rectangle = getInsideAllocation(a)
			If isBefore(CInt(Fix(x)), CInt(Fix(y)), alloc) Then
				' point is before the range represented
				Dim retValue As Integer = -1

				Try
					retValue = getNextVisualPositionFrom(-1, Position.Bias.Forward, a, EAST, bias)
				Catch ble As BadLocationException
				Catch iae As System.ArgumentException
				End Try
				If retValue = -1 Then
					retValue = startOffset
					bias(0) = Position.Bias.Forward
				End If
				Return retValue
			ElseIf isAfter(CInt(Fix(x)), CInt(Fix(y)), alloc) Then
				' point is after the range represented.
				Dim retValue As Integer = -1
				Try
					retValue = getNextVisualPositionFrom(-1, Position.Bias.Forward, a, WEST, bias)
				Catch ble As BadLocationException
				Catch iae As System.ArgumentException
				End Try

				If retValue = -1 Then
					' NOTE: this could actually use end offset with backward.
					retValue = endOffset - 1
					bias(0) = Position.Bias.Forward
				End If
				Return retValue
			Else
				' locate the child and pass along the request
				Dim v As View = getViewAtPoint(CInt(Fix(x)), CInt(Fix(y)), alloc)
				If v IsNot Nothing Then Return v.viewToModel(x, y, alloc, bias)
			End If
			Return -1
		End Function

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be visible,
		''' they might not be in the same order found in the model, or they just
		''' might not allow access to some of the locations in the model.
		''' This is a convenience method for <seealso cref="#getNextNorthSouthVisualPositionFrom"/>
		''' and <seealso cref="#getNextEastWestVisualPositionFrom"/>.
		''' This method enables specifying a position to convert
		''' within the range of &gt;=0.  If the value is -1, a position
		''' will be calculated automatically.  If the value &lt; -1,
		''' the {@code BadLocationException} will be thrown.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="b"> a bias value of either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard;
		'''  this may be one of the following:
		'''  <ul>
		'''  <li><code>SwingConstants.WEST</code>
		'''  <li><code>SwingConstants.EAST</code>
		'''  <li><code>SwingConstants.NORTH</code>
		'''  <li><code>SwingConstants.SOUTH</code>
		'''  </ul> </param>
		''' <param name="biasRet"> an array containing the bias that was checked </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> the given position is not a valid
		'''                                 position within the document </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code> is invalid </exception>
		Public Overrides Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			If pos < -1 Then Throw New BadLocationException("invalid position", pos)
			Dim alloc As Rectangle = getInsideAllocation(a)

			Select Case direction
			Case NORTH
				Return getNextNorthSouthVisualPositionFrom(pos, b, a, direction, biasRet)
			Case SOUTH
				Return getNextNorthSouthVisualPositionFrom(pos, b, a, direction, biasRet)
			Case EAST
				Return getNextEastWestVisualPositionFrom(pos, b, a, direction, biasRet)
			Case WEST
				Return getNextEastWestVisualPositionFrom(pos, b, a, direction, biasRet)
			Case Else
				Throw New System.ArgumentException("Bad direction: " & direction)
			End Select
		End Function

		''' <summary>
		''' Returns the child view index representing the given
		''' position in the model.  This is implemented to call the
		''' <code>getViewIndexByPosition</code>
		''' method for backward compatibility.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position
		''' @since 1.3 </returns>
		Public Overrides Function getViewIndex(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
			If b Is Position.Bias.Backward Then pos -= 1
			If (pos >= startOffset) AndAlso (pos < endOffset) Then Return getViewIndexAtPosition(pos)
			Return -1
		End Function

		' --- local methods ----------------------------------------------------


		''' <summary>
		''' Tests whether a point lies before the rectangle range.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="alloc"> the rectangle </param>
		''' <returns> true if the point is before the specified range </returns>
		Protected Friend MustOverride Function isBefore(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean

		''' <summary>
		''' Tests whether a point lies after the rectangle range.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="alloc"> the rectangle </param>
		''' <returns> true if the point is after the specified range </returns>
		Protected Friend MustOverride Function isAfter(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As Boolean

		''' <summary>
		''' Fetches the child view at the given coordinates.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="alloc"> the parent's allocation on entry, which should
		'''   be changed to the child's allocation on exit </param>
		''' <returns> the child view </returns>
		Protected Friend MustOverride Function getViewAtPoint(ByVal x As Integer, ByVal y As Integer, ByVal alloc As Rectangle) As View

		''' <summary>
		''' Returns the allocation for a given child.
		''' </summary>
		''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <param name="a">  the allocation to the interior of the box on entry,
		'''   and the allocation of the child view at the index on exit. </param>
		Protected Friend MustOverride Sub childAllocation(ByVal index As Integer, ByVal a As Rectangle)

		''' <summary>
		''' Fetches the child view that represents the given position in
		''' the model.  This is implemented to fetch the view in the case
		''' where there is a child view for each child element.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <param name="a">  the allocation to the interior of the box on entry,
		'''   and the allocation of the view containing the position on exit </param>
		''' <returns>  the view representing the given position, or
		'''   <code>null</code> if there isn't one </returns>
		Protected Friend Overridable Function getViewAtPosition(ByVal pos As Integer, ByVal a As Rectangle) As View
			Dim index As Integer = getViewIndexAtPosition(pos)
			If (index >= 0) AndAlso (index < viewCount) Then
				Dim v As View = getView(index)
				If a IsNot Nothing Then childAllocation(index, a)
				Return v
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Fetches the child view index representing the given position in
		''' the model.  This is implemented to fetch the view in the case
		''' where there is a child view for each child element.
		''' </summary>
		''' <param name="pos"> the position &gt;= 0 </param>
		''' <returns>  index of the view representing the given position, or
		'''   -1 if no view represents that position </returns>
		Protected Friend Overridable Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
			Dim elem As Element = element
			Return elem.getElementIndex(pos)
		End Function

		''' <summary>
		''' Translates the immutable allocation given to the view
		''' to a mutable allocation that represents the interior
		''' allocation (i.e. the bounds of the given allocation
		''' with the top, left, bottom, and right insets removed.
		''' It is expected that the returned value would be further
		''' mutated to represent an allocation to a child view.
		''' This is implemented to reuse an instance variable so
		''' it avoids creating excessive Rectangles.  Typically
		''' the result of calling this method would be fed to
		''' the <code>childAllocation</code> method.
		''' </summary>
		''' <param name="a"> the allocation given to the view </param>
		''' <returns> the allocation that represents the inside of the
		'''   view after the margins have all been removed; if the
		'''   given allocation was <code>null</code>,
		'''   the return value is <code>null</code> </returns>
		Protected Friend Overridable Function getInsideAllocation(ByVal a As Shape) As Rectangle
			If a IsNot Nothing Then
				' get the bounds, hopefully without allocating
				' a new rectangle.  The Shape argument should
				' not be modified... we copy it into the
				' child allocation.
				Dim alloc As Rectangle
				If TypeOf a Is Rectangle Then
					alloc = CType(a, Rectangle)
				Else
					alloc = a.bounds
				End If

				childAlloc.bounds = alloc
				childAlloc.x += leftInset
				childAlloc.y += topInset
				childAlloc.width -= leftInset + rightInset
				childAlloc.height -= topInset + bottomInset
				Return childAlloc
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Sets the insets from the paragraph attributes specified in
		''' the given attributes.
		''' </summary>
		''' <param name="attr"> the attributes </param>
		Protected Friend Overridable Property paragraphInsets As AttributeSet
			Set(ByVal attr As AttributeSet)
				' Since version 1.1 doesn't have scaling and assumes
				' a pixel is equal to a point, we just cast the point
				' sizes to integers.
				top = CShort(Fix(StyleConstants.getSpaceAbove(attr)))
				left = CShort(Fix(StyleConstants.getLeftIndent(attr)))
				bottom = CShort(Fix(StyleConstants.getSpaceBelow(attr)))
				right = CShort(Fix(StyleConstants.getRightIndent(attr)))
			End Set
		End Property

		''' <summary>
		''' Sets the insets for the view.
		''' </summary>
		''' <param name="top"> the top inset &gt;= 0 </param>
		''' <param name="left"> the left inset &gt;= 0 </param>
		''' <param name="bottom"> the bottom inset &gt;= 0 </param>
		''' <param name="right"> the right inset &gt;= 0 </param>
		Protected Friend Overridable Sub setInsets(ByVal top As Short, ByVal left As Short, ByVal bottom As Short, ByVal right As Short)
			Me.top = top
			Me.left = left
			Me.right = right
			Me.bottom = bottom
		End Sub

		''' <summary>
		''' Gets the left inset.
		''' </summary>
		''' <returns> the inset &gt;= 0 </returns>
		Protected Friend Overridable Property leftInset As Short
			Get
				Return left
			End Get
		End Property

		''' <summary>
		''' Gets the right inset.
		''' </summary>
		''' <returns> the inset &gt;= 0 </returns>
		Protected Friend Overridable Property rightInset As Short
			Get
				Return right
			End Get
		End Property

		''' <summary>
		''' Gets the top inset.
		''' </summary>
		''' <returns> the inset &gt;= 0 </returns>
		Protected Friend Overridable Property topInset As Short
			Get
				Return top
			End Get
		End Property

		''' <summary>
		''' Gets the bottom inset.
		''' </summary>
		''' <returns> the inset &gt;= 0 </returns>
		Protected Friend Overridable Property bottomInset As Short
			Get
				Return bottom
			End Get
		End Property

		''' <summary>
		''' Returns the next visual position for the cursor, in either the
		''' north or south direction.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="b"> a bias value of either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard;
		'''  this may be one of the following:
		'''  <ul>
		'''  <li><code>SwingConstants.NORTH</code>
		'''  <li><code>SwingConstants.SOUTH</code>
		'''  </ul> </param>
		''' <param name="biasRet"> an array containing the bias that was checked </param>
		''' <returns> the location within the model that best represents the next
		'''  north or south location </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code> is invalid </exception>
		''' <seealso cref= #getNextVisualPositionFrom
		''' </seealso>
		''' <returns> the next position west of the passed in position </returns>
		Protected Friend Overridable Function getNextNorthSouthVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Return Utilities.getNextVisualPositionFrom(Me, pos, b, a, direction, biasRet)
		End Function

		''' <summary>
		''' Returns the next visual position for the cursor, in either the
		''' east or west direction.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="b"> a bias value of either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard;
		'''  this may be one of the following:
		'''  <ul>
		'''  <li><code>SwingConstants.WEST</code>
		'''  <li><code>SwingConstants.EAST</code>
		'''  </ul> </param>
		''' <param name="biasRet"> an array containing the bias that was checked </param>
		''' <returns> the location within the model that best represents the next
		'''  west or east location </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code> is invalid </exception>
		''' <seealso cref= #getNextVisualPositionFrom </seealso>
		Protected Friend Overridable Function getNextEastWestVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Return Utilities.getNextVisualPositionFrom(Me, pos, b, a, direction, biasRet)
		End Function

		''' <summary>
		''' Determines in which direction the next view lays.
		''' Consider the <code>View</code> at index n. Typically the
		''' <code>View</code>s are layed out from left to right,
		''' so that the <code>View</code> to the EAST will be
		''' at index n + 1, and the <code>View</code> to the WEST
		''' will be at index n - 1. In certain situations,
		''' such as with bidirectional text, it is possible
		''' that the <code>View</code> to EAST is not at index n + 1,
		''' but rather at index n - 1, or that the <code>View</code>
		''' to the WEST is not at index n - 1, but index n + 1.
		''' In this case this method would return true, indicating the
		''' <code>View</code>s are layed out in descending order.
		''' <p>
		''' This unconditionally returns false, subclasses should override this
		''' method if there is the possibility for laying <code>View</code>s in
		''' descending order.
		''' </summary>
		''' <param name="position"> position into the model </param>
		''' <param name="bias"> either <code>Position.Bias.Forward</code> or
		'''          <code>Position.Bias.Backward</code> </param>
		''' <returns> false </returns>
		Protected Friend Overridable Function flipEastAndWestAtEnds(ByVal position As Integer, ByVal bias As Position.Bias) As Boolean
			Return False
		End Function


		' ---- member variables ---------------------------------------------


		Private Shared ZERO As View() = New View(){}

		Private children As View()
		Private nchildren As Integer
		Private left As Short
		Private right As Short
		Private top As Short
		Private bottom As Short
		Private childAlloc As Rectangle
	End Class

End Namespace