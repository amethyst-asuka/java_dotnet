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
	''' <p>
	''' A very important part of the text package is the <code>View</code> class.
	''' As the name suggests it represents a view of the text model,
	''' or a piece of the text model.
	''' It is this class that is responsible for the look of the text component.
	''' The view is not intended to be some completely new thing that one must
	''' learn, but rather is much like a lightweight component.
	''' <p>
	''' By default, a view is very light.  It contains a reference to the parent
	''' view from which it can fetch many things without holding state, and it
	''' contains a reference to a portion of the model (<code>Element</code>).
	''' A view does not
	''' have to exactly represent an element in the model, that is simply a typical
	''' and therefore convenient mapping.  A view can alternatively maintain a couple
	''' of Position objects to maintain its location in the model (i.e. represent
	''' a fragment of an element).  This is typically the result of formatting where
	''' views have been broken down into pieces.  The convenience of a substantial
	''' relationship to the element makes it easier to build factories to produce the
	''' views, and makes it easier  to keep track of the view pieces as the model is
	''' changed and the view must be changed to reflect the model.  Simple views
	''' therefore represent an Element directly and complex views do not.
	''' <p>
	''' A view has the following responsibilities:
	'''  <dl>
	''' 
	'''    <dt><b>Participate in layout.</b>
	'''    <dd>
	'''    <p>The view has a <code>setSize</code> method which is like
	'''    <code>doLayout</code> and <code>setSize</code> in <code>Component</code> combined.
	'''    The view has a <code>preferenceChanged</code> method which is
	'''    like <code>invalidate</code> in <code>Component</code> except that one can
	'''    invalidate just one axis
	'''    and the child requesting the change is identified.
	'''    <p>A View expresses the size that it would like to be in terms of three
	'''    values, a minimum, a preferred, and a maximum span.  Layout in a view is
	'''    can be done independently upon each axis.  For a properly functioning View
	'''    implementation, the minimum span will be &lt;= the preferred span which in turn
	'''    will be &lt;= the maximum span.
	'''    </p>
	'''    <p style="text-align:center"><img src="doc-files/View-flexibility.jpg"
	'''                     alt="The above text describes this graphic.">
	'''    <p>The minimum set of methods for layout are:
	'''    <ul>
	'''    <li><seealso cref="#getMinimumSpan(int) getMinimumSpan"/>
	'''    <li><seealso cref="#getPreferredSpan(int) getPreferredSpan"/>
	'''    <li><seealso cref="#getMaximumSpan(int) getMaximumSpan"/>
	'''    <li><seealso cref="#getAlignment(int) getAlignment"/>
	'''    <li><seealso cref="#preferenceChanged(javax.swing.text.View, boolean, boolean) preferenceChanged"/>
	'''    <li><seealso cref="#setSize(float, float) setSize"/>
	'''    </ul>
	''' 
	'''  <p>The <code>setSize</code> method should be prepared to be called a number of times
	'''    (i.e. It may be called even if the size didn't change).
	'''    The <code>setSize</code> method
	'''    is generally called to make sure the View layout is complete prior to trying
	'''    to perform an operation on it that requires an up-to-date layout.  A view's
	'''    size should <em>always</em> be set to a value within the minimum and maximum
	'''    span specified by that view.  Additionally, the view must always call the
	'''    <code>preferenceChanged</code> method on the parent if it has changed the
	'''    values for the
	'''    layout it would like, and expects the parent to honor.  The parent View is
	'''    not required to recognize a change until the <code>preferenceChanged</code>
	'''    has been sent.
	'''    This allows parent View implementations to cache the child requirements if
	'''    desired.  The calling sequence looks something like the following:
	'''    </p>
	'''    <p style="text-align:center">
	'''      <img src="doc-files/View-layout.jpg"
	'''       alt="Sample calling sequence between parent view and child view:
	'''       setSize, getMinimum, getPreferred, getMaximum, getAlignment, setSize">
	'''    <p>The exact calling sequence is up to the layout functionality of
	'''    the parent view (if the view has any children).  The view may collect
	'''    the preferences of the children prior to determining what it will give
	'''    each child, or it might iteratively update the children one at a time.
	'''    </p>
	''' 
	'''    <dt><b>Render a portion of the model.</b>
	'''    <dd>
	'''    <p>This is done in the paint method, which is pretty much like a component
	'''    paint method.  Views are expected to potentially populate a fairly large
	'''    tree.  A <code>View</code> has the following semantics for rendering:
	'''    </p>
	'''    <ul>
	'''    <li>The view gets its allocation from the parent at paint time, so it
	'''    must be prepared to redo layout if the allocated area is different from
	'''    what it is prepared to deal with.
	'''    <li>The coordinate system is the same as the hosting <code>Component</code>
	'''    (i.e. the <code>Component</code> returned by the
	'''    <seealso cref="#getContainer getContainer"/> method).
	'''    This means a child view lives in the same coordinate system as the parent
	'''    view unless the parent has explicitly changed the coordinate system.
	'''    To schedule itself to be repainted a view can call repaint on the hosting
	'''    <code>Component</code>.
	'''    <li>The default is to <em>not clip</em> the children.  It is more efficient
	'''    to allow a view to clip only if it really feels it needs clipping.
	'''    <li>The <code>Graphics</code> object given is not initialized in any way.
	'''    A view should set any settings needed.
	'''    <li>A <code>View</code> is inherently transparent.  While a view may render into its
	'''    entire allocation, typically a view does not.  Rendering is performed by
	'''    traversing down the tree of <code>View</code> implementations.
	'''    Each <code>View</code> is responsible
	'''    for rendering its children.  This behavior is depended upon for thread
	'''    safety.  While view implementations do not necessarily have to be implemented
	'''    with thread safety in mind, other view implementations that do make use of
	'''    concurrency can depend upon a tree traversal to guarantee thread safety.
	'''    <li>The order of views relative to the model is up to the implementation.
	'''    Although child views will typically be arranged in the same order that they
	'''    occur in the model, they may be visually arranged in an entirely different
	'''    order.  View implementations may have Z-Order associated with them if the
	'''    children are overlapping.
	'''    </ul>
	'''    <p>The methods for rendering are:
	'''    <ul>
	'''    <li><seealso cref="#paint(java.awt.Graphics, java.awt.Shape) paint"/>
	'''    </ul>
	''' 
	'''    <dt><b>Translate between the model and view coordinate systems.</b>
	'''    <dd>
	'''    <p>Because the view objects are produced from a factory and therefore cannot
	'''    necessarily be counted upon to be in a particular pattern, one must be able
	'''    to perform translation to properly locate spatial representation of the model.
	'''    The methods for doing this are:
	'''    <ul>
	'''    <li><seealso cref="#modelToView(int, javax.swing.text.Position.Bias, int, javax.swing.text.Position.Bias, java.awt.Shape) modelToView"/>
	'''    <li><seealso cref="#viewToModel(float, float, java.awt.Shape, javax.swing.text.Position.Bias[]) viewToModel"/>
	'''    <li><seealso cref="#getDocument() getDocument"/>
	'''    <li><seealso cref="#getElement() getElement"/>
	'''    <li><seealso cref="#getStartOffset() getStartOffset"/>
	'''    <li><seealso cref="#getEndOffset() getEndOffset"/>
	'''    </ul>
	'''    <p>The layout must be valid prior to attempting to make the translation.
	'''    The translation is not valid, and must not be attempted while changes
	'''    are being broadcasted from the model via a <code>DocumentEvent</code>.
	'''    </p>
	''' 
	'''    <dt><b>Respond to changes from the model.</b>
	'''    <dd>
	'''    <p>If the overall view is represented by many pieces (which is the best situation
	'''    if one want to be able to change the view and write the least amount of new code),
	'''    it would be impractical to have a huge number of <code>DocumentListener</code>s.
	'''    If each
	'''    view listened to the model, only a few would actually be interested in the
	'''    changes broadcasted at any given time.   Since the model has no knowledge of
	'''    views, it has no way to filter the broadcast of change information.  The view
	'''    hierarchy itself is instead responsible for propagating the change information.
	'''    At any level in the view hierarchy, that view knows enough about its children to
	'''    best distribute the change information further.   Changes are therefore broadcasted
	'''    starting from the root of the view hierarchy.
	'''    The methods for doing this are:
	'''    <ul>
	'''    <li><seealso cref="#insertUpdate insertUpdate"/>
	'''    <li><seealso cref="#removeUpdate removeUpdate"/>
	'''    <li><seealso cref="#changedUpdate changedUpdate"/>
	'''    </ul>
	'''    <p>
	''' </dl>
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public MustInherit Class View
		Implements javax.swing.SwingConstants

		''' <summary>
		''' Creates a new <code>View</code> object.
		''' </summary>
		''' <param name="elem"> the <code>Element</code> to represent </param>
		Public Sub New(ByVal elem As Element)
			Me.elem = elem
		End Sub

		''' <summary>
		''' Returns the parent of the view.
		''' </summary>
		''' <returns> the parent, or <code>null</code> if none exists </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getParent() As View 'JavaToDotNetTempPropertyGetparent
		Public Overridable Property parent As View
			Get
				Return parent
			End Get
			Set(ByVal parent As View)
		End Property

		''' <summary>
		'''  Returns a boolean that indicates whether
		'''  the view is visible or not.  By default
		'''  all views are visible.
		''' </summary>
		'''  <returns> always returns true </returns>
		Public Overridable Property visible As Boolean
			Get
				Return True
			End Get
		End Property


		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns>   the span the view would like to be rendered into.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public MustOverride Function getPreferredSpan(ByVal axis As Integer) As Single

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns>  the minimum span the view can be rendered into </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overridable Function getMinimumSpan(ByVal axis As Integer) As Single
			Dim w As Integer = getResizeWeight(axis)
			If w = 0 Then Return getPreferredSpan(axis)
			Return 0
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns>  the maximum span the view can be rendered into </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overridable Function getMaximumSpan(ByVal axis As Integer) As Single
			Dim w As Integer = getResizeWeight(axis)
			If w = 0 Then Return getPreferredSpan(axis)
			Return Integer.MaxValue
		End Function

		''' <summary>
		''' Child views can call this on the parent to indicate that
		''' the preference has changed and should be reconsidered
		''' for layout.  By default this just propagates upward to
		''' the next parent.  The root view will call
		''' <code>revalidate</code> on the associated text component.
		''' </summary>
		''' <param name="child"> the child view </param>
		''' <param name="width"> true if the width preference has changed </param>
		''' <param name="height"> true if the height preference has changed </param>
		''' <seealso cref= javax.swing.JComponent#revalidate </seealso>
		Public Overridable Sub preferenceChanged(ByVal child As View, ByVal width As Boolean, ByVal height As Boolean)
			Dim ___parent As View = parent
			If ___parent IsNot Nothing Then ___parent.preferenceChanged(Me, width, height)
		End Sub

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  The desired alignment is returned.  This should be
		''' a value &gt;= 0.0 and &lt;= 1.0, where 0 indicates alignment at
		''' the origin and 1.0 indicates alignment to the full span
		''' away from the origin.  An alignment of 0.5 would be the
		''' center of the view.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns> the value 0.5 </returns>
		Public Overridable Function getAlignment(ByVal axis As Integer) As Single
			Return 0.5f
		End Function

		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.  The view may need to do layout and create child
		''' views to enable itself to render into the given allocation.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="allocation"> the allocated region to render into </param>
		Public MustOverride Sub paint(ByVal g As Graphics, ByVal allocation As Shape)

			' if the parent is null then propogate down the view tree
			If parent Is Nothing Then
				For i As Integer = 0 To viewCount - 1
					If getView(i).parent Is Me Then getView(i).parent = Nothing
				Next i
			End If
			Me.parent = parent
		End Sub

		''' <summary>
		''' Returns the number of views in this view.  Since
		''' the default is to not be a composite view this
		''' returns 0.
		''' </summary>
		''' <returns> the number of views &gt;= 0 </returns>
		''' <seealso cref= View#getViewCount </seealso>
		Public Overridable Property viewCount As Integer
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' Gets the <i>n</i>th child view.  Since there are no
		''' children by default, this returns <code>null</code>.
		''' </summary>
		''' <param name="n"> the number of the view to get, &gt;= 0 &amp;&amp; &lt; getViewCount() </param>
		''' <returns> the view </returns>
		Public Overridable Function getView(ByVal n As Integer) As View
			Return Nothing
		End Function


		''' <summary>
		''' Removes all of the children.  This is a convenience
		''' call to <code>replace</code>.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Sub removeAll()
			replace(0, viewCount, Nothing)
		End Sub

		''' <summary>
		''' Removes one of the children at the given position.
		''' This is a convenience call to <code>replace</code>.
		''' @since 1.3
		''' </summary>
		Public Overridable Sub remove(ByVal i As Integer)
			replace(i, 1, Nothing)
		End Sub

		''' <summary>
		''' Inserts a single child view.  This is a convenience
		''' call to <code>replace</code>.
		''' </summary>
		''' <param name="offs"> the offset of the view to insert before &gt;= 0 </param>
		''' <param name="v"> the view </param>
		''' <seealso cref= #replace
		''' @since 1.3 </seealso>
		Public Overridable Sub insert(ByVal offs As Integer, ByVal v As View)
			Dim one As View() = New View(0){}
			one(0) = v
			replace(offs, 0, one)
		End Sub

		''' <summary>
		''' Appends a single child view.  This is a convenience
		''' call to <code>replace</code>.
		''' </summary>
		''' <param name="v"> the view </param>
		''' <seealso cref= #replace
		''' @since 1.3 </seealso>
		Public Overridable Sub append(ByVal v As View)
			Dim one As View() = New View(0){}
			one(0) = v
			replace(viewCount, 0, one)
		End Sub

		''' <summary>
		''' Replaces child views.  If there are no views to remove
		''' this acts as an insert.  If there are no views to
		''' add this acts as a remove.  Views being removed will
		''' have the parent set to <code>null</code>, and the internal reference
		''' to them removed so that they can be garbage collected.
		''' This is implemented to do nothing, because by default
		''' a view has no children.
		''' </summary>
		''' <param name="offset"> the starting index into the child views to insert
		'''   the new views.  This should be a value &gt;= 0 and &lt;= getViewCount </param>
		''' <param name="length"> the number of existing child views to remove
		'''   This should be a value &gt;= 0 and &lt;= (getViewCount() - offset). </param>
		''' <param name="views"> the child views to add.  This value can be
		'''   <code>null</code> to indicate no children are being added
		'''   (useful to remove).
		''' @since 1.3 </param>
		Public Overridable Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal views As View())
		End Sub

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
		Public Overridable Function getViewIndex(ByVal pos As Integer, ByVal b As Position.Bias) As Integer
			Return -1
		End Function

		''' <summary>
		''' Fetches the allocation for the given child view.
		''' This enables finding out where various views
		''' are located, without assuming how the views store
		''' their location.  This returns <code>null</code> since the
		''' default is to not have any child views.
		''' </summary>
		''' <param name="index"> the index of the child, &gt;= 0 &amp;&amp; &lt;
		'''          <code>getViewCount()</code> </param>
		''' <param name="a">  the allocation to this view </param>
		''' <returns> the allocation to the child </returns>
		Public Overridable Function getChildAllocation(ByVal index As Integer, ByVal a As Shape) As Shape
			Return Nothing
		End Function

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location at which one might place a caret.
		''' Some views may not be visible,
		''' they might not be in the same order found in the model, or they just
		''' might not allow access to some of the locations in the model.
		''' This method enables specifying a position to convert
		''' within the range of &gt;=0.  If the value is -1, a position
		''' will be calculated automatically.  If the value &lt; -1,
		''' the {@code BadLocationException} will be thrown.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="a"> the allocated region in which to render </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard.
		'''  This will be one of the following values:
		''' <ul>
		''' <li>SwingConstants.WEST
		''' <li>SwingConstants.EAST
		''' <li>SwingConstants.NORTH
		''' <li>SwingConstants.SOUTH
		''' </ul> </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> the given position is not a valid
		'''                                 position within the document </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code>
		'''          doesn't have one of the legal values above </exception>
		Public Overridable Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			If pos < -1 Then Throw New BadLocationException("Invalid position", pos)

			biasRet(0) = Position.Bias.Forward
			Select Case direction
			Case NORTH, SOUTH
				If pos = -1 Then
					pos = If(direction = NORTH, Math.Max(0, endOffset - 1), startOffset)
					Exit Select
				End If
				Dim target As JTextComponent = CType(container, JTextComponent)
				Dim c As Caret = If(target IsNot Nothing, target.caret, Nothing)
				' YECK! Ideally, the x location from the magic caret position
				' would be passed in.
				Dim mcp As Point
				If c IsNot Nothing Then
					mcp = c.magicCaretPosition
				Else
					mcp = Nothing
				End If
				Dim x As Integer
				If mcp Is Nothing Then
					Dim loc As Rectangle = target.modelToView(pos)
					x = If(loc Is Nothing, 0, loc.x)
				Else
					x = mcp.x
				End If
				If direction = NORTH Then
					pos = Utilities.getPositionAbove(target, pos, x)
				Else
					pos = Utilities.getPositionBelow(target, pos, x)
				End If
			Case WEST
				If pos = -1 Then
					pos = Math.Max(0, endOffset - 1)
				Else
					pos = Math.Max(0, pos - 1)
				End If
			Case EAST
				If pos = -1 Then
					pos = startOffset
				Else
					pos = Math.Min(pos + 1, document.length)
				End If
			Case Else
				Throw New System.ArgumentException("Bad direction: " & direction)
			End Select
			Return pos
		End Function

		''' <summary>
		''' Provides a mapping, for a given character,
		''' from the document model coordinate space
		''' to the view coordinate space.
		''' </summary>
		''' <param name="pos"> the position of the desired character (&gt;=0) </param>
		''' <param name="a"> the area of the view, which encompasses the requested character </param>
		''' <param name="b"> the bias toward the previous character or the
		'''  next character represented by the offset, in case the
		'''  position is a boundary of two views; <code>b</code> will have one
		'''  of these values:
		''' <ul>
		''' <li> <code>Position.Bias.Forward</code>
		''' <li> <code>Position.Bias.Backward</code>
		''' </ul> </param>
		''' <returns> the bounding box, in view coordinate space,
		'''          of the character at the specified position </returns>
		''' <exception cref="BadLocationException">  if the specified position does
		'''   not represent a valid location in the associated document </exception>
		''' <exception cref="IllegalArgumentException"> if <code>b</code> is not one of the
		'''          legal <code>Position.Bias</code> values listed above </exception>
		''' <seealso cref= View#viewToModel </seealso>
		Public MustOverride Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape

		''' <summary>
		''' Provides a mapping, for a given region,
		''' from the document model coordinate space
		''' to the view coordinate space. The specified region is
		''' created as a union of the first and last character positions.
		''' </summary>
		''' <param name="p0"> the position of the first character (&gt;=0) </param>
		''' <param name="b0"> the bias of the first character position,
		'''  toward the previous character or the
		'''  next character represented by the offset, in case the
		'''  position is a boundary of two views; <code>b0</code> will have one
		'''  of these values:
		''' <ul style="list-style-type:none">
		''' <li> <code>Position.Bias.Forward</code>
		''' <li> <code>Position.Bias.Backward</code>
		''' </ul> </param>
		''' <param name="p1"> the position of the last character (&gt;=0) </param>
		''' <param name="b1"> the bias for the second character position, defined
		'''          one of the legal values shown above </param>
		''' <param name="a"> the area of the view, which encompasses the requested region </param>
		''' <returns> the bounding box which is a union of the region specified
		'''          by the first and last character positions </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''   not represent a valid location in the associated document </exception>
		''' <exception cref="IllegalArgumentException"> if <code>b0</code> or
		'''          <code>b1</code> are not one of the
		'''          legal <code>Position.Bias</code> values listed above </exception>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overridable Function modelToView(ByVal p0 As Integer, ByVal b0 As Position.Bias, ByVal p1 As Integer, ByVal b1 As Position.Bias, ByVal a As Shape) As Shape
			Dim s0 As Shape = modelToView(p0, a, b0)
			Dim s1 As Shape
			If p1 = endOffset Then
				Try
					s1 = modelToView(p1, a, b1)
				Catch ble As BadLocationException
					s1 = Nothing
				End Try
				If s1 Is Nothing Then
					' Assume extends left to right.
					Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
					s1 = New Rectangle(alloc.x + alloc.width - 1, alloc.y, 1, alloc.height)
				End If
			Else
				s1 = modelToView(p1, a, b1)
			End If
			Dim r0 As Rectangle = s0.bounds
			Dim r1 As Rectangle = If(TypeOf s1 Is Rectangle, CType(s1, Rectangle), s1.bounds)
			If r0.y <> r1.y Then
				' If it spans lines, force it to be the width of the view.
				Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
				r0.x = alloc.x
				r0.width = alloc.width
			End If
			r0.add(r1)
			Return r0
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.  The <code>biasReturn</code>
		''' argument will be filled in to indicate that the point given is
		''' closer to the next character in the model or the previous
		''' character in the model.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region in which to render </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0.  The <code>biasReturn</code>
		'''  argument will be
		''' filled in to indicate that the point given is closer to the next
		''' character in the model or the previous character in the model. </returns>
		Public MustOverride Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer

		''' <summary>
		''' Gives notification that something was inserted into
		''' the document in a location that this view is responsible for.
		''' To reduce the burden to subclasses, this functionality is
		''' spread out into the following calls that subclasses can
		''' reimplement:
		''' <ol>
		''' <li><seealso cref="#updateChildren updateChildren"/> is called
		''' if there were any changes to the element this view is
		''' responsible for.  If this view has child views that are
		''' represent the child elements, then this method should do
		''' whatever is necessary to make sure the child views correctly
		''' represent the model.
		''' <li><seealso cref="#forwardUpdate forwardUpdate"/> is called
		''' to forward the DocumentEvent to the appropriate child views.
		''' <li><seealso cref="#updateLayout updateLayout"/> is called to
		''' give the view a chance to either repair its layout, to reschedule
		''' layout, or do nothing.
		''' </ol>
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overridable Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			If viewCount > 0 Then
				Dim elem As Element = element
				Dim ec As DocumentEvent.ElementChange = e.getChange(elem)
				If ec IsNot Nothing Then
					If Not updateChildren(ec, e, f) Then ec = Nothing
				End If
				forwardUpdate(ec, e, a, f)
				updateLayout(ec, e, a)
			End If
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' To reduce the burden to subclasses, this functionality is
		''' spread out into the following calls that subclasses can
		''' reimplement:
		''' <ol>
		''' <li><seealso cref="#updateChildren updateChildren"/> is called
		''' if there were any changes to the element this view is
		''' responsible for.  If this view has child views that are
		''' represent the child elements, then this method should do
		''' whatever is necessary to make sure the child views correctly
		''' represent the model.
		''' <li><seealso cref="#forwardUpdate forwardUpdate"/> is called
		''' to forward the DocumentEvent to the appropriate child views.
		''' <li><seealso cref="#updateLayout updateLayout"/> is called to
		''' give the view a chance to either repair its layout, to reschedule
		''' layout, or do nothing.
		''' </ol>
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overridable Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			If viewCount > 0 Then
				Dim elem As Element = element
				Dim ec As DocumentEvent.ElementChange = e.getChange(elem)
				If ec IsNot Nothing Then
					If Not updateChildren(ec, e, f) Then ec = Nothing
				End If
				forwardUpdate(ec, e, a, f)
				updateLayout(ec, e, a)
			End If
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' To reduce the burden to subclasses, this functionality is
		''' spread out into the following calls that subclasses can
		''' reimplement:
		''' <ol>
		''' <li><seealso cref="#updateChildren updateChildren"/> is called
		''' if there were any changes to the element this view is
		''' responsible for.  If this view has child views that are
		''' represent the child elements, then this method should do
		''' whatever is necessary to make sure the child views correctly
		''' represent the model.
		''' <li><seealso cref="#forwardUpdate forwardUpdate"/> is called
		''' to forward the DocumentEvent to the appropriate child views.
		''' <li><seealso cref="#updateLayout updateLayout"/> is called to
		''' give the view a chance to either repair its layout, to reschedule
		''' layout, or do nothing.
		''' </ol>
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overridable Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			If viewCount > 0 Then
				Dim elem As Element = element
				Dim ec As DocumentEvent.ElementChange = e.getChange(elem)
				If ec IsNot Nothing Then
					If Not updateChildren(ec, e, f) Then ec = Nothing
				End If
				forwardUpdate(ec, e, a, f)
				updateLayout(ec, e, a)
			End If
		End Sub

		''' <summary>
		''' Fetches the model associated with the view.
		''' </summary>
		''' <returns> the view model, <code>null</code> if none </returns>
		''' <seealso cref= View#getDocument </seealso>
		Public Overridable Property document As Document
			Get
				Return elem.document
			End Get
		End Property

		''' <summary>
		''' Fetches the portion of the model for which this view is
		''' responsible.
		''' </summary>
		''' <returns> the starting offset into the model &gt;= 0 </returns>
		''' <seealso cref= View#getStartOffset </seealso>
		Public Overridable Property startOffset As Integer
			Get
				Return elem.startOffset
			End Get
		End Property

		''' <summary>
		''' Fetches the portion of the model for which this view is
		''' responsible.
		''' </summary>
		''' <returns> the ending offset into the model &gt;= 0 </returns>
		''' <seealso cref= View#getEndOffset </seealso>
		Public Overridable Property endOffset As Integer
			Get
				Return elem.endOffset
			End Get
		End Property

		''' <summary>
		''' Fetches the structural portion of the subject that this
		''' view is mapped to.  The view may not be responsible for the
		''' entire portion of the element.
		''' </summary>
		''' <returns> the subject </returns>
		''' <seealso cref= View#getElement </seealso>
		Public Overridable Property element As Element
			Get
				Return elem
			End Get
		End Property

		''' <summary>
		''' Fetch a <code>Graphics</code> for rendering.
		''' This can be used to determine
		''' font characteristics, and will be different for a print view
		''' than a component view.
		''' </summary>
		''' <returns> a <code>Graphics</code> object for rendering
		''' @since 1.3 </returns>
		Public Overridable Property graphics As Graphics
			Get
				' PENDING(prinz) this is a temporary implementation
				Dim c As Component = container
				Return c.graphics
			End Get
		End Property

		''' <summary>
		''' Fetches the attributes to use when rendering.  By default
		''' this simply returns the attributes of the associated element.
		''' This method should be used rather than using the element
		''' directly to obtain access to the attributes to allow
		''' view-specific attributes to be mixed in or to allow the
		''' view to have view-specific conversion of attributes by
		''' subclasses.
		''' Each view should document what attributes it recognizes
		''' for the purpose of rendering or layout, and should always
		''' access them through the <code>AttributeSet</code> returned
		''' by this method.
		''' </summary>
		Public Overridable Property attributes As AttributeSet
			Get
				Return elem.attributes
			End Get
		End Property

		''' <summary>
		''' Tries to break this view on the given axis.  This is
		''' called by views that try to do formatting of their
		''' children.  For example, a view of a paragraph will
		''' typically try to place its children into row and
		''' views representing chunks of text can sometimes be
		''' broken down into smaller pieces.
		''' <p>
		''' This is implemented to return the view itself, which
		''' represents the default behavior on not being
		''' breakable.  If the view does support breaking, the
		''' starting offset of the view returned should be the
		''' given offset, and the end offset should be less than
		''' or equal to the end offset of the view being broken.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <param name="offset"> the location in the document model
		'''   that a broken fragment would occupy &gt;= 0.  This
		'''   would be the starting offset of the fragment
		'''   returned </param>
		''' <param name="pos"> the position along the axis that the
		'''  broken view would occupy &gt;= 0.  This may be useful for
		'''  things like tab calculations </param>
		''' <param name="len"> specifies the distance along the axis
		'''  where a potential break is desired &gt;= 0 </param>
		''' <returns> the fragment of the view that represents the
		'''  given span, if the view can be broken.  If the view
		'''  doesn't support breaking behavior, the view itself is
		'''  returned. </returns>
		''' <seealso cref= ParagraphView </seealso>
		Public Overridable Function breakView(ByVal axis As Integer, ByVal offset As Integer, ByVal pos As Single, ByVal len As Single) As View
			Return Me
		End Function

		''' <summary>
		''' Creates a view that represents a portion of the element.
		''' This is potentially useful during formatting operations
		''' for taking measurements of fragments of the view.  If
		''' the view doesn't support fragmenting (the default), it
		''' should return itself.
		''' </summary>
		''' <param name="p0"> the starting offset &gt;= 0.  This should be a value
		'''   greater or equal to the element starting offset and
		'''   less than the element ending offset. </param>
		''' <param name="p1"> the ending offset &gt; p0.  This should be a value
		'''   less than or equal to the elements end offset and
		'''   greater than the elements starting offset. </param>
		''' <returns> the view fragment, or itself if the view doesn't
		'''   support breaking into fragments </returns>
		''' <seealso cref= LabelView </seealso>
		Public Overridable Function createFragment(ByVal p0 As Integer, ByVal p1 As Integer) As View
			Return Me
		End Function

		''' <summary>
		''' Determines how attractive a break opportunity in
		''' this view is.  This can be used for determining which
		''' view is the most attractive to call <code>breakView</code>
		''' on in the process of formatting.  A view that represents
		''' text that has whitespace in it might be more attractive
		''' than a view that has no whitespace, for example.  The
		''' higher the weight, the more attractive the break.  A
		''' value equal to or lower than <code>BadBreakWeight</code>
		''' should not be considered for a break.  A value greater
		''' than or equal to <code>ForcedBreakWeight</code> should
		''' be broken.
		''' <p>
		''' This is implemented to provide the default behavior
		''' of returning <code>BadBreakWeight</code> unless the length
		''' is greater than the length of the view in which case the
		''' entire view represents the fragment.  Unless a view has
		''' been written to support breaking behavior, it is not
		''' attractive to try and break the view.  An example of
		''' a view that does support breaking is <code>LabelView</code>.
		''' An example of a view that uses break weight is
		''' <code>ParagraphView</code>.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <param name="pos"> the potential location of the start of the
		'''   broken view &gt;= 0.  This may be useful for calculating tab
		'''   positions </param>
		''' <param name="len"> specifies the relative length from <em>pos</em>
		'''   where a potential break is desired &gt;= 0 </param>
		''' <returns> the weight, which should be a value between
		'''   ForcedBreakWeight and BadBreakWeight </returns>
		''' <seealso cref= LabelView </seealso>
		''' <seealso cref= ParagraphView </seealso>
		''' <seealso cref= #BadBreakWeight </seealso>
		''' <seealso cref= #GoodBreakWeight </seealso>
		''' <seealso cref= #ExcellentBreakWeight </seealso>
		''' <seealso cref= #ForcedBreakWeight </seealso>
		Public Overridable Function getBreakWeight(ByVal axis As Integer, ByVal pos As Single, ByVal len As Single) As Integer
			If len > getPreferredSpan(axis) Then Return GoodBreakWeight
			Return BadBreakWeight
		End Function

		''' <summary>
		''' Determines the resizability of the view along the
		''' given axis.  A value of 0 or less is not resizable.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''          <code>View.Y_AXIS</code> </param>
		''' <returns> the weight </returns>
		Public Overridable Function getResizeWeight(ByVal axis As Integer) As Integer
			Return 0
		End Function

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view along the given axis, if it
		''' has any layout duties.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overridable Sub setSize(ByVal width As Single, ByVal height As Single)
		End Sub

		''' <summary>
		''' Fetches the container hosting the view.  This is useful for
		''' things like scheduling a repaint, finding out the host
		''' components font, etc.  The default implementation
		''' of this is to forward the query to the parent view.
		''' </summary>
		''' <returns> the container, <code>null</code> if none </returns>
		Public Overridable Property container As Container
			Get
				Dim v As View = parent
				Return If(v IsNot Nothing, v.container, Nothing)
			End Get
		End Property

		''' <summary>
		''' Fetches the <code>ViewFactory</code> implementation that is feeding
		''' the view hierarchy.  Normally the views are given this
		''' as an argument to updates from the model when they
		''' are most likely to need the factory, but this
		''' method serves to provide it at other times.
		''' </summary>
		''' <returns> the factory, <code>null</code> if none </returns>
		Public Overridable Property viewFactory As ViewFactory
			Get
				Dim v As View = parent
				Return If(v IsNot Nothing, v.viewFactory, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the tooltip text at the specified location. The default
		''' implementation returns the value from the child View identified by
		''' the passed in location.
		''' 
		''' @since 1.4 </summary>
		''' <seealso cref= JTextComponent#getToolTipText </seealso>
		Public Overridable Function getToolTipText(ByVal x As Single, ByVal y As Single, ByVal allocation As Shape) As String
			Dim ___viewIndex As Integer = getViewIndex(x, y, allocation)
			If ___viewIndex >= 0 Then
				allocation = getChildAllocation(___viewIndex, allocation)
				Dim rect As Rectangle = If(TypeOf allocation Is Rectangle, CType(allocation, Rectangle), allocation.bounds)
				If rect.contains(x, y) Then Return getView(___viewIndex).getToolTipText(x, y, allocation)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the child view index representing the given position in
		''' the view. This iterates over all the children returning the
		''' first with a bounds that contains <code>x</code>, <code>y</code>.
		''' </summary>
		''' <param name="x"> the x coordinate </param>
		''' <param name="y"> the y coordinate </param>
		''' <param name="allocation"> current allocation of the View. </param>
		''' <returns>  index of the view representing the given location, or
		'''   -1 if no view represents that position
		''' @since 1.4 </returns>
		Public Overridable Function getViewIndex(ByVal x As Single, ByVal y As Single, ByVal allocation As Shape) As Integer
			For counter As Integer = viewCount - 1 To 0 Step -1
				Dim ___childAllocation As Shape = getChildAllocation(counter, allocation)

				If ___childAllocation IsNot Nothing Then
					Dim rect As Rectangle = If(TypeOf ___childAllocation Is Rectangle, CType(___childAllocation, Rectangle), ___childAllocation.bounds)

					If rect.contains(x, y) Then Return counter
				End If
			Next counter
			Return -1
		End Function

		''' <summary>
		''' Updates the child views in response to receiving notification
		''' that the model changed, and there is change record for the
		''' element this view is responsible for.  This is implemented
		''' to assume the child views are directly responsible for the
		''' child elements of the element this view represents.  The
		''' <code>ViewFactory</code> is used to create child views for each element
		''' specified as added in the <code>ElementChange</code>, starting at the
		''' index specified in the given <code>ElementChange</code>.  The number of
		''' child views representing the removed elements specified are
		''' removed.
		''' </summary>
		''' <param name="ec"> the change information for the element this view
		'''  is responsible for.  This should not be <code>null</code> if
		'''  this method gets called </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="f"> the factory to use to build child views </param>
		''' <returns> whether or not the child views represent the
		'''  child elements of the element this view is responsible
		'''  for.  Some views create children that represent a portion
		'''  of the element they are responsible for, and should return
		'''  false.  This information is used to determine if views
		'''  in the range of the added elements should be forwarded to
		'''  or not </returns>
		''' <seealso cref= #insertUpdate </seealso>
		''' <seealso cref= #removeUpdate </seealso>
		''' <seealso cref= #changedUpdate
		''' @since 1.3 </seealso>
		Protected Friend Overridable Function updateChildren(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal f As ViewFactory) As Boolean
			Dim removedElems As Element() = ec.childrenRemoved
			Dim addedElems As Element() = ec.childrenAdded
			Dim added As View() = Nothing
			If addedElems IsNot Nothing Then
				added = New View(addedElems.Length - 1){}
				For i As Integer = 0 To addedElems.Length - 1
					added(i) = f.create(addedElems(i))
				Next i
			End If
			Dim nremoved As Integer = 0
			Dim index As Integer = ec.index
			If removedElems IsNot Nothing Then nremoved = removedElems.Length
			replace(index, nremoved, added)
			Return True
		End Function

		''' <summary>
		''' Forwards the given <code>DocumentEvent</code> to the child views
		''' that need to be notified of the change to the model.
		''' If there were changes to the element this view is
		''' responsible for, that should be considered when
		''' forwarding (i.e. new child views should not get
		''' notified).
		''' </summary>
		''' <param name="ec"> changes to the element this view is responsible
		'''  for (may be <code>null</code> if there were no changes). </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= #insertUpdate </seealso>
		''' <seealso cref= #removeUpdate </seealso>
		''' <seealso cref= #changedUpdate
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub forwardUpdate(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			calculateUpdateIndexes(e)

			Dim hole0 As Integer = lastUpdateIndex + 1
			Dim hole1 As Integer = hole0
			Dim addedElems As Element() = If(ec IsNot Nothing, ec.childrenAdded, Nothing)
			If (addedElems IsNot Nothing) AndAlso (addedElems.Length > 0) Then
				hole0 = ec.index
				hole1 = hole0 + addedElems.Length - 1
			End If

			' forward to any view not in the forwarding hole
			' formed by added elements (i.e. they will be updated
			' by initialization.
			For i As Integer = firstUpdateIndex To lastUpdateIndex
				If Not((i >= hole0) AndAlso (i <= hole1)) Then
					Dim v As View = getView(i)
					If v IsNot Nothing Then
						Dim childAlloc As Shape = getChildAllocation(i, a)
						forwardUpdateToView(v, e, childAlloc, f)
					End If
				End If
			Next i
		End Sub

		''' <summary>
		''' Calculates the first and the last indexes of the child views
		''' that need to be notified of the change to the model. </summary>
		''' <param name="e"> the change information from the associated document </param>
		Friend Overridable Sub calculateUpdateIndexes(ByVal e As DocumentEvent)
			Dim pos As Integer = e.offset
			firstUpdateIndex = getViewIndex(pos, Position.Bias.Forward)
			If firstUpdateIndex = -1 AndAlso e.type Is DocumentEvent.EventType.REMOVE AndAlso pos >= endOffset Then firstUpdateIndex = viewCount - 1
			lastUpdateIndex = firstUpdateIndex
			Dim v As View = If(firstUpdateIndex >= 0, getView(firstUpdateIndex), Nothing)
			If v IsNot Nothing Then
				If (v.startOffset = pos) AndAlso (pos > 0) Then firstUpdateIndex = Math.Max(firstUpdateIndex - 1, 0)
			End If
			If e.type IsNot DocumentEvent.EventType.REMOVE Then
				lastUpdateIndex = getViewIndex(pos + e.length, Position.Bias.Forward)
				If lastUpdateIndex < 0 Then lastUpdateIndex = viewCount - 1
			End If
			firstUpdateIndex = Math.Max(firstUpdateIndex, 0)
		End Sub

		''' <summary>
		''' Updates the view to reflect the changes.
		''' </summary>
		Friend Overridable Sub updateAfterChange()
			' Do nothing by default. Should be overridden in subclasses, if any.
		End Sub

		''' <summary>
		''' Forwards the <code>DocumentEvent</code> to the give child view.  This
		''' simply messages the view with a call to <code>insertUpdate</code>,
		''' <code>removeUpdate</code>, or <code>changedUpdate</code> depending
		''' upon the type of the event.  This is called by
		''' <seealso cref="#forwardUpdate forwardUpdate"/> to forward
		''' the event to children that need it.
		''' </summary>
		''' <param name="v"> the child view to forward the event to </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= #forwardUpdate
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub forwardUpdateToView(ByVal v As View, ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			Dim type As DocumentEvent.EventType = e.type
			If type Is DocumentEvent.EventType.INSERT Then
				v.insertUpdate(e, a, f)
			ElseIf type Is DocumentEvent.EventType.REMOVE Then
				v.removeUpdate(e, a, f)
			Else
				v.changedUpdate(e, a, f)
			End If
		End Sub

		''' <summary>
		''' Updates the layout in response to receiving notification of
		''' change from the model.  This is implemented to call
		''' <code>preferenceChanged</code> to reschedule a new layout
		''' if the <code>ElementChange</code> record is not <code>null</code>.
		''' </summary>
		''' <param name="ec"> changes to the element this view is responsible
		'''  for (may be <code>null</code> if there were no changes) </param>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <seealso cref= #insertUpdate </seealso>
		''' <seealso cref= #removeUpdate </seealso>
		''' <seealso cref= #changedUpdate
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub updateLayout(ByVal ec As DocumentEvent.ElementChange, ByVal e As DocumentEvent, ByVal a As Shape)
			If (ec IsNot Nothing) AndAlso (a IsNot Nothing) Then
				' should damage more intelligently
				preferenceChanged(Nothing, True, True)
				Dim host As Container = container
				If host IsNot Nothing Then host.repaint()
			End If
		End Sub

		''' <summary>
		''' The weight to indicate a view is a bad break
		''' opportunity for the purpose of formatting.  This
		''' value indicates that no attempt should be made to
		''' break the view into fragments as the view has
		''' not been written to support fragmenting.
		''' </summary>
		''' <seealso cref= #getBreakWeight </seealso>
		''' <seealso cref= #GoodBreakWeight </seealso>
		''' <seealso cref= #ExcellentBreakWeight </seealso>
		''' <seealso cref= #ForcedBreakWeight </seealso>
		Public Const BadBreakWeight As Integer = 0

		''' <summary>
		''' The weight to indicate a view supports breaking,
		''' but better opportunities probably exist.
		''' </summary>
		''' <seealso cref= #getBreakWeight </seealso>
		''' <seealso cref= #BadBreakWeight </seealso>
		''' <seealso cref= #ExcellentBreakWeight </seealso>
		''' <seealso cref= #ForcedBreakWeight </seealso>
		Public Const GoodBreakWeight As Integer = 1000

		''' <summary>
		''' The weight to indicate a view supports breaking,
		''' and this represents a very attractive place to
		''' break.
		''' </summary>
		''' <seealso cref= #getBreakWeight </seealso>
		''' <seealso cref= #BadBreakWeight </seealso>
		''' <seealso cref= #GoodBreakWeight </seealso>
		''' <seealso cref= #ForcedBreakWeight </seealso>
		Public Const ExcellentBreakWeight As Integer = 2000

		''' <summary>
		''' The weight to indicate a view supports breaking,
		''' and must be broken to be represented properly
		''' when placed in a view that formats its children
		''' by breaking them.
		''' </summary>
		''' <seealso cref= #getBreakWeight </seealso>
		''' <seealso cref= #BadBreakWeight </seealso>
		''' <seealso cref= #GoodBreakWeight </seealso>
		''' <seealso cref= #ExcellentBreakWeight </seealso>
		Public Const ForcedBreakWeight As Integer = 3000

		''' <summary>
		''' Axis for format/break operations.
		''' </summary>
		Public Shared ReadOnly X_AXIS As Integer = HORIZONTAL

		''' <summary>
		''' Axis for format/break operations.
		''' </summary>
		Public Shared ReadOnly Y_AXIS As Integer = VERTICAL

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it. This is
		''' implemented to default the bias to <code>Position.Bias.Forward</code>
		''' which was previously implied.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region in which to render </param>
		''' <returns> the bounding box of the given position is returned </returns>
		''' <exception cref="BadLocationException">  if the given position does
		'''   not represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView
		''' @deprecated </seealso>
		<Obsolete> _
		Public Overridable Function modelToView(ByVal pos As Integer, ByVal a As Shape) As Shape
			Return modelToView(pos, a, Position.Bias.Forward)
		End Function


		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region in which to render </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel
		''' @deprecated </seealso>
		<Obsolete> _
		Public Overridable Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape) As Integer
			sharedBiasReturn(0) = Position.Bias.Forward
			Return viewToModel(x, y, a, sharedBiasReturn)
		End Function

		' static argument available for viewToModel calls since only
		' one thread at a time may call this method.
		Friend Shared ReadOnly sharedBiasReturn As Position.Bias() = New Position.Bias(0){}

		Private parent As View
		Private elem As Element

		''' <summary>
		''' The index of the first child view to be notified.
		''' </summary>
		Friend firstUpdateIndex As Integer

		''' <summary>
		''' The index of the last child view to be notified.
		''' </summary>
		Friend lastUpdateIndex As Integer

	End Class

End Namespace