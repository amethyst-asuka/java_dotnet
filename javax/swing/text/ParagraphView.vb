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
	''' View of a simple line-wrapping paragraph that supports
	''' multiple fonts, colors, components, icons, etc.  It is
	''' basically a vertical box with a margin around it.  The
	''' contents of the box are a bunch of rows which are special
	''' horizontal boxes.  This view creates a collection of
	''' views that represent the child elements of the paragraph
	''' element.  Each of these views are placed into a row
	''' directly if they will fit, otherwise the <code>breakView</code>
	''' method is called to try and carve the view into pieces
	''' that fit.
	''' 
	''' @author  Timothy Prinzing
	''' @author  Scott Violet
	''' @author  Igor Kushnirskiy </summary>
	''' <seealso cref=     View </seealso>
	Public Class ParagraphView
		Inherits FlowView
		Implements TabExpander

		''' <summary>
		''' Constructs a <code>ParagraphView</code> for the given element.
		''' </summary>
		''' <param name="elem"> the element that this view is responsible for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem, View.Y_AXIS)
			propertiesFromAttributestes()
			Dim doc As Document = elem.document
			Dim i18nFlag As Object = doc.getProperty(AbstractDocument.I18NProperty)
			If (i18nFlag IsNot Nothing) AndAlso i18nFlag.Equals(Boolean.TRUE) Then
				Try
					If i18nStrategy Is Nothing Then
						' the classname should probably come from a property file.
						Dim classname As String = "javax.swing.text.TextLayoutStrategy"
						Dim loader As ClassLoader = Me.GetType().classLoader
						If loader IsNot Nothing Then
							i18nStrategy = loader.loadClass(classname)
						Else
							i18nStrategy = Type.GetType(classname)
						End If
					End If
					Dim o As Object = i18nStrategy.newInstance()
					If TypeOf o Is FlowStrategy Then strategy = CType(o, FlowStrategy)
				Catch e As Exception
					Throw New StateInvariantError("ParagraphView: Can't create i18n strategy: " & e.message)
				End Try
			End If
		End Sub

		''' <summary>
		''' Sets the type of justification.
		''' </summary>
		''' <param name="j"> one of the following values:
		''' <ul>
		''' <li><code>StyleConstants.ALIGN_LEFT</code>
		''' <li><code>StyleConstants.ALIGN_CENTER</code>
		''' <li><code>StyleConstants.ALIGN_RIGHT</code>
		''' </ul> </param>
		Protected Friend Overridable Property justification As Integer
			Set(ByVal j As Integer)
				justification = j
			End Set
		End Property

		''' <summary>
		''' Sets the line spacing.
		''' </summary>
		''' <param name="ls"> the value is a factor of the line hight </param>
		Protected Friend Overridable Property lineSpacing As Single
			Set(ByVal ls As Single)
				lineSpacing = ls
			End Set
		End Property

		''' <summary>
		''' Sets the indent on the first line.
		''' </summary>
		''' <param name="fi"> the value in points </param>
		Protected Friend Overridable Property firstLineIndent As Single
			Set(ByVal fi As Single)
				firstLineIndent = CInt(Fix(fi))
			End Set
		End Property

		''' <summary>
		''' Set the cached properties from the attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()
			Dim attr As AttributeSet = attributes
			If attr IsNot Nothing Then
				paragraphInsets = attr
				Dim a As Integer? = CInt(Fix(attr.getAttribute(StyleConstants.Alignment)))
				Dim ___alignment As Integer
				If a Is Nothing Then
					Dim doc As Document = element.document
					Dim o As Object = doc.getProperty(java.awt.font.TextAttribute.RUN_DIRECTION)
					If (o IsNot Nothing) AndAlso o.Equals(java.awt.font.TextAttribute.RUN_DIRECTION_RTL) Then
						___alignment = StyleConstants.ALIGN_RIGHT
					Else
						___alignment = StyleConstants.ALIGN_LEFT
					End If
				Else
					___alignment = a
				End If
				justification = ___alignment
				lineSpacing = StyleConstants.getLineSpacing(attr)
				firstLineIndent = StyleConstants.getFirstLineIndent(attr)
			End If
		End Sub

		''' <summary>
		''' Returns the number of views that this view is
		''' responsible for.
		''' The child views of the paragraph are rows which
		''' have been used to arrange pieces of the <code>View</code>s
		''' that represent the child elements.  This is the number
		''' of views that have been tiled in two dimensions,
		''' and should be equivalent to the number of child elements
		''' to the element this view is responsible for.
		''' </summary>
		''' <returns> the number of views that this <code>ParagraphView</code>
		'''          is responsible for </returns>
		Protected Friend Overridable Property layoutViewCount As Integer
			Get
				Return layoutPool.viewCount
			End Get
		End Property

		''' <summary>
		''' Returns the view at a given <code>index</code>.
		''' The child views of the paragraph are rows which
		''' have been used to arrange pieces of the <code>Views</code>
		''' that represent the child elements.  This methods returns
		''' the view responsible for the child element index
		''' (prior to breaking).  These are the Views that were
		''' produced from a factory (to represent the child
		''' elements) and used for layout.
		''' </summary>
		''' <param name="index"> the <code>index</code> of the desired view </param>
		''' <returns> the view at <code>index</code> </returns>
		Protected Friend Overridable Function getLayoutView(ByVal index As Integer) As View
			Return layoutPool.getView(index)
		End Function

		''' <summary>
		''' Returns the next visual position for the cursor, in
		''' either the east or west direction.
		''' Overridden from <code>CompositeView</code>. </summary>
		''' <param name="pos"> position into the model </param>
		''' <param name="b"> either <code>Position.Bias.Forward</code> or
		'''          <code>Position.Bias.Backward</code> </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> either <code>SwingConstants.NORTH</code>
		'''          or <code>SwingConstants.SOUTH</code> </param>
		''' <param name="biasRet"> an array containing the bias that were checked
		'''  in this method </param>
		''' <returns> the location in the model that represents the
		'''  next location visual position </returns>
		Protected Friend Overrides Function getNextNorthSouthVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Dim vIndex As Integer
			If pos = -1 Then
				vIndex = If(direction = NORTH, viewCount - 1, 0)
			Else
				If b Is Position.Bias.Backward AndAlso pos > 0 Then
					vIndex = getViewIndexAtPosition(pos - 1)
				Else
					vIndex = getViewIndexAtPosition(pos)
				End If
				If direction = NORTH Then
					If vIndex = 0 Then Return -1
					vIndex -= 1
				Else
					vIndex += 1
					If vIndex >= viewCount Then Return -1
					End If
			End If
			' vIndex gives index of row to look in.
			Dim text As JTextComponent = CType(container, JTextComponent)
			Dim c As Caret = text.caret
			Dim magicPoint As Point
			magicPoint = If(c IsNot Nothing, c.magicCaretPosition, Nothing)
			Dim x As Integer
			If magicPoint Is Nothing Then
				Dim posBounds As Shape
				Try
					posBounds = text.uI.modelToView(text, pos, b)
				Catch exc As BadLocationException
					posBounds = Nothing
				End Try
				If posBounds Is Nothing Then
					x = 0
				Else
					x = posBounds.bounds.x
				End If
			Else
				x = magicPoint.x
			End If
			Return getClosestPositionTo(pos, b, a, direction, biasRet, vIndex, x)
		End Function

		''' <summary>
		''' Returns the closest model position to <code>x</code>.
		''' <code>rowIndex</code> gives the index of the view that corresponds
		''' that should be looked in. </summary>
		''' <param name="pos">  position into the model </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> one of the following values:
		''' <ul>
		''' <li><code>SwingConstants.NORTH</code>
		''' <li><code>SwingConstants.SOUTH</code>
		''' </ul> </param>
		''' <param name="biasRet"> an array containing the bias that were checked
		'''  in this method </param>
		''' <param name="rowIndex"> the index of the view </param>
		''' <param name="x"> the x coordinate of interest </param>
		''' <returns> the closest model position to <code>x</code> </returns>
		' NOTE: This will not properly work if ParagraphView contains
		' other ParagraphViews. It won't raise, but this does not message
		' the children views with getNextVisualPositionFrom.
		Protected Friend Overridable Function getClosestPositionTo(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias(), ByVal rowIndex As Integer, ByVal x As Integer) As Integer
			Dim text As JTextComponent = CType(container, JTextComponent)
			Dim doc As Document = document
			Dim ___row As View = getView(rowIndex)
			Dim lastPos As Integer = -1
			' This could be made better to check backward positions too.
			biasRet(0) = Position.Bias.Forward
			Dim vc As Integer = 0
			Dim numViews As Integer = ___row.viewCount
			Do While vc < numViews
				Dim v As View = ___row.getView(vc)
				Dim start As Integer = v.startOffset
				Dim ltr As Boolean = AbstractDocument.isLeftToRight(doc, start, start + 1)
				If ltr Then
					lastPos = start
					Dim [end] As Integer = v.endOffset
					Do While lastPos < [end]
						Dim xx As Single = text.modelToView(lastPos).bounds.x
						If xx >= x Then
							lastPos += 1
							Do While lastPos < [end] AndAlso text.modelToView(lastPos).bounds.x = xx
								lastPos += 1
							Loop
								lastPos -= 1
								Return lastPos
						End If
						lastPos += 1
					Loop
					lastPos -= 1
				Else
					For lastPos = v.endOffset - 1 To start Step -1
						Dim xx As Single = text.modelToView(lastPos).bounds.x
						If xx >= x Then
							lastPos -= 1
							Do While lastPos >= start AndAlso text.modelToView(lastPos).bounds.x = xx
								lastPos -= 1
							Loop
								lastPos += 1
								Return lastPos
						End If
					Next lastPos
					lastPos += 1
				End If
				vc += 1
			Loop
			If lastPos = -1 Then Return startOffset
			Return lastPos
		End Function

		''' <summary>
		''' Determines in which direction the next view lays.
		''' Consider the <code>View</code> at index n.
		''' Typically the <code>View</code>s are layed out
		''' from left to right, so that the <code>View</code>
		''' to the EAST will be at index n + 1, and the
		''' <code>View</code> to the WEST will be at index n - 1.
		''' In certain situations, such as with bidirectional text,
		''' it is possible that the <code>View</code> to EAST is not
		''' at index n + 1, but rather at index n - 1,
		''' or that the <code>View</code> to the WEST is not at
		''' index n - 1, but index n + 1.  In this case this method
		''' would return true, indicating the <code>View</code>s are
		''' layed out in descending order.
		''' <p>
		''' This will return true if the text is layed out right
		''' to left at position, otherwise false.
		''' </summary>
		''' <param name="position"> position into the model </param>
		''' <param name="bias"> either <code>Position.Bias.Forward</code> or
		'''          <code>Position.Bias.Backward</code> </param>
		''' <returns> true if the text is layed out right to left at
		'''         position, otherwise false. </returns>
		Protected Friend Overrides Function flipEastAndWestAtEnds(ByVal position As Integer, ByVal bias As Position.Bias) As Boolean
			Dim doc As Document = document
			position = startOffset
			Return Not AbstractDocument.isLeftToRight(doc, position, position + 1)
		End Function

		' --- FlowView methods ---------------------------------------------

		''' <summary>
		''' Fetches the constraining span to flow against for
		''' the given child index. </summary>
		''' <param name="index"> the index of the view being queried </param>
		''' <returns> the constraining span for the given view at
		'''  <code>index</code>
		''' @since 1.3 </returns>
		Public Overrides Function getFlowSpan(ByVal index As Integer) As Integer
			Dim child As View = getView(index)
			Dim adjust As Integer = 0
			If TypeOf child Is Row Then
				Dim ___row As Row = CType(child, Row)
				adjust = ___row.leftInset + ___row.rightInset
			End If
			Return If(layoutSpan = Integer.MaxValue, layoutSpan, (layoutSpan - adjust))
		End Function

		''' <summary>
		''' Fetches the location along the flow axis that the
		''' flow span will start at. </summary>
		''' <param name="index"> the index of the view being queried </param>
		''' <returns> the location for the given view at
		'''  <code>index</code>
		''' @since 1.3 </returns>
		Public Overrides Function getFlowStart(ByVal index As Integer) As Integer
			Dim child As View = getView(index)
			Dim adjust As Integer = 0
			If TypeOf child Is Row Then
				Dim ___row As Row = CType(child, Row)
				adjust = ___row.leftInset
			End If
			Return tabBase + adjust
		End Function

		''' <summary>
		''' Create a <code>View</code> that should be used to hold a
		''' a row's worth of children in a flow. </summary>
		''' <returns> the new <code>View</code>
		''' @since 1.3 </returns>
		Protected Friend Overrides Function createRow() As View
			Return New Row(Me, element)
		End Function

		' --- TabExpander methods ------------------------------------------

		''' <summary>
		''' Returns the next tab stop position given a reference position.
		''' This view implements the tab coordinate system, and calls
		''' <code>getTabbedSpan</code> on the logical children in the process
		''' of layout to determine the desired span of the children.  The
		''' logical children can delegate their tab expansion upward to
		''' the paragraph which knows how to expand tabs.
		''' <code>LabelView</code> is an example of a view that delegates
		''' its tab expansion needs upward to the paragraph.
		''' <p>
		''' This is implemented to try and locate a <code>TabSet</code>
		''' in the paragraph element's attribute set.  If one can be
		''' found, its settings will be used, otherwise a default expansion
		''' will be provided.  The base location for for tab expansion
		''' is the left inset from the paragraphs most recent allocation
		''' (which is what the layout of the children is based upon).
		''' </summary>
		''' <param name="x"> the X reference position </param>
		''' <param name="tabOffset"> the position within the text stream
		'''   that the tab occurred at &gt;= 0 </param>
		''' <returns> the trailing end of the tab expansion &gt;= 0 </returns>
		''' <seealso cref= TabSet </seealso>
		''' <seealso cref= TabStop </seealso>
		''' <seealso cref= LabelView </seealso>
		Public Overridable Function nextTabStop(ByVal x As Single, ByVal tabOffset As Integer) As Single Implements TabExpander.nextTabStop
			' If the text isn't left justified, offset by 10 pixels!
			If justification <> StyleConstants.ALIGN_LEFT Then Return x + 10.0f
			x -= tabBase
			Dim tabs As TabSet = tabSet
			If tabs Is Nothing Then Return CSng(tabBase + ((CInt(Fix(x)) \ 72 + 1) * 72))
			Dim tab As TabStop = tabs.getTabAfter(x +.01f)
			If tab Is Nothing Then Return tabBase + x + 5.0f
			Dim ___alignment As Integer = tab.alignment
			Dim ___offset As Integer
			Select Case ___alignment
			Case Else
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
			Case TabStop.ALIGN_LEFT
				' Simple case, left tab.
				Return tabBase + tab.position
			Case TabStop.ALIGN_BAR
				' PENDING: what does this mean?
				Return tabBase + tab.position
			Case TabStop.ALIGN_RIGHT, TabStop.ALIGN_CENTER
				___offset = findOffsetToCharactersInString(tabChars, tabOffset + 1)
			Case TabStop.ALIGN_DECIMAL
				___offset = findOffsetToCharactersInString(tabDecimalChars, tabOffset + 1)
			End Select
			If ___offset = -1 Then ___offset = endOffset
			Dim charsSize As Single = getPartialSize(tabOffset + 1, ___offset)
			Select Case ___alignment
			Case TabStop.ALIGN_RIGHT, TabStop.ALIGN_DECIMAL
				' right and decimal are treated the same way, the new
				' position will be the location of the tab less the
				' partialSize.
				Return tabBase + Math.Max(x, tab.position - charsSize)
			Case TabStop.ALIGN_CENTER
				' Similar to right, but half the partialSize.
				Return tabBase + Math.Max(x, tab.position - charsSize / 2.0f)
			End Select
			' will never get here!
			Return x
		End Function

		''' <summary>
		''' Gets the <code>Tabset</code> to be used in calculating tabs.
		''' </summary>
		''' <returns> the <code>TabSet</code> </returns>
		Protected Friend Overridable Property tabSet As TabSet
			Get
				Return StyleConstants.getTabSet(element.attributes)
			End Get
		End Property

		''' <summary>
		''' Returns the size used by the views between
		''' <code>startOffset</code> and <code>endOffset</code>.
		''' This uses <code>getPartialView</code> to calculate the
		''' size if the child view implements the
		''' <code>TabableView</code> interface. If a
		''' size is needed and a <code>View</code> does not implement
		''' the <code>TabableView</code> interface,
		''' the <code>preferredSpan</code> will be used.
		''' </summary>
		''' <param name="startOffset"> the starting document offset &gt;= 0 </param>
		''' <param name="endOffset"> the ending document offset &gt;= startOffset </param>
		''' <returns> the size &gt;= 0 </returns>
		Protected Friend Overridable Function getPartialSize(ByVal startOffset As Integer, ByVal endOffset As Integer) As Single
			Dim ___size As Single = 0.0f
			Dim ___viewIndex As Integer
			Dim numViews As Integer = viewCount
			Dim ___view As View
			Dim viewEnd As Integer
			Dim tempEnd As Integer

			' Have to search layoutPool!
			' PENDING: when ParagraphView supports breaking location
			' into layoutPool will have to change!
			___viewIndex = element.getElementIndex(startOffset)
			numViews = layoutPool.viewCount
			Do While startOffset < endOffset AndAlso ___viewIndex < numViews
				___view = layoutPool.getView(___viewIndex)
				___viewIndex += 1
				viewEnd = ___view.endOffset
				tempEnd = Math.Min(endOffset, viewEnd)
				If TypeOf ___view Is TabableView Then
					___size += CType(___view, TabableView).getPartialSpan(startOffset, tempEnd)
				ElseIf startOffset = ___view.startOffset AndAlso tempEnd = ___view.endOffset Then
					___size += ___view.getPreferredSpan(View.X_AXIS)
				Else
					' PENDING: should we handle this better?
					Return 0.0f
				End If
				startOffset = viewEnd
			Loop
			Return ___size
		End Function

		''' <summary>
		''' Finds the next character in the document with a character in
		''' <code>string</code>, starting at offset <code>start</code>. If
		''' there are no characters found, -1 will be returned.
		''' </summary>
		''' <param name="string"> the string of characters </param>
		''' <param name="start"> where to start in the model &gt;= 0 </param>
		''' <returns> the document offset, or -1 if no characters found </returns>
		Protected Friend Overridable Function findOffsetToCharactersInString(ByVal [string] As Char(), ByVal start As Integer) As Integer
			Dim stringLength As Integer = [string].Length
			Dim [end] As Integer = endOffset
			Dim seg As New Segment
			Try
				document.getText(start, [end] - start, seg)
			Catch ble As BadLocationException
				Return -1
			End Try
			Dim counter As Integer = seg.offset
			Dim maxCounter As Integer = seg.offset + seg.count
			Do While counter < maxCounter
				Dim currentChar As Char = seg.array(counter)
				For subCounter As Integer = 0 To stringLength - 1
					If currentChar = [string](subCounter) Then Return counter - seg.offset + start
				Next subCounter
				counter += 1
			Loop
			' No match.
			Return -1
		End Function

		''' <summary>
		''' Returns where the tabs are calculated from. </summary>
		''' <returns> where tabs are calculated from </returns>
		Protected Friend Overridable Property tabBase As Single
			Get
				Return CSng(tabBase)
			End Get
		End Property

		' ---- View methods ----------------------------------------------------

		''' <summary>
		''' Renders using the given rendering surface and area on that
		''' surface.  This is implemented to delegate to the superclass
		''' after stashing the base coordinate for tab calculations.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			tabBase = alloc.x + leftInset
			MyBase.paint(g, a)

			' line with the negative firstLineIndent value needs
			' special handling
			If firstLineIndent < 0 Then
				Dim sh As Shape = getChildAllocation(0, a)
				If (sh IsNot Nothing) AndAlso sh.intersects(alloc) Then
					Dim x As Integer = alloc.x + leftInset + firstLineIndent
					Dim y As Integer = alloc.y + topInset

					Dim clip As Rectangle = g.clipBounds
					tempRect.x = x + getOffset(X_AXIS, 0)
					tempRect.y = y + getOffset(Y_AXIS, 0)
					tempRect.width = getSpan(X_AXIS, 0) - firstLineIndent
					tempRect.height = getSpan(Y_AXIS, 0)
					If tempRect.intersects(clip) Then
						tempRect.x = tempRect.x - firstLineIndent
						paintChild(g, tempRect, 0)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  This is implemented to give the alignment to the
		''' center of the first row along the y axis, and the default
		''' along the x axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''   <code>View.Y_AXIS</code> </param>
		''' <returns> the desired alignment.  This should be a value
		'''   between 0.0 and 1.0 inclusive, where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin.  An alignment of 0.5 would be the
		'''   center of the view. </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			Select Case axis
			Case Y_AXIS
				Dim a As Single = 0.5f
				If viewCount <> 0 Then
					Dim paragraphSpan As Integer = CInt(Fix(getPreferredSpan(View.Y_AXIS)))
					Dim v As View = getView(0)
					Dim rowSpan As Integer = CInt(Fix(v.getPreferredSpan(View.Y_AXIS)))
					a = If(paragraphSpan <> 0, (CSng(rowSpan \ 2)) / paragraphSpan, 0)
				End If
				Return a
			Case X_AXIS
				Return 0.5f
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Breaks this view on the given axis at the given length.
		''' <p>
		''' <code>ParagraphView</code> instances are breakable
		''' along the <code>Y_AXIS</code> only, and only if
		''' <code>len</code> is after the first line.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''  or <code>View.Y_AXIS</code> </param>
		''' <param name="len"> specifies where a potential break is desired
		'''  along the given axis &gt;= 0 </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <returns> the fragment of the view that represents the
		'''  given span, if the view can be broken; if the view
		'''  doesn't support breaking behavior, the view itself is
		'''  returned </returns>
		''' <seealso cref= View#breakView </seealso>
		Public Overridable Function breakView(ByVal axis As Integer, ByVal len As Single, ByVal a As Shape) As View
			If axis = View.Y_AXIS Then
				If a IsNot Nothing Then
					Dim alloc As Rectangle = a.bounds
					sizeize(alloc.width, alloc.height)
				End If
				' Determine what row to break on.

				' PENDING(prinz) add break support
				Return Me
			End If
			Return Me
		End Function

		''' <summary>
		''' Gets the break weight for a given location.
		''' <p>
		''' <code>ParagraphView</code> instances are breakable
		''' along the <code>Y_AXIS</code> only, and only if
		''' <code>len</code> is after the first row.  If the length
		''' is less than one row, a value of <code>BadBreakWeight</code>
		''' is returned.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code>
		'''  or <code>View.Y_AXIS</code> </param>
		''' <param name="len"> specifies where a potential break is desired &gt;= 0 </param>
		''' <returns> a value indicating the attractiveness of breaking here;
		'''  either <code>GoodBreakWeight</code> or <code>BadBreakWeight</code> </returns>
		''' <seealso cref= View#getBreakWeight </seealso>
		Public Overridable Function getBreakWeight(ByVal axis As Integer, ByVal len As Single) As Integer
			If axis = View.Y_AXIS Then Return BadBreakWeight
			Return BadBreakWeight
		End Function

		''' <summary>
		''' Calculate the needs for the paragraph along the minor axis.
		''' 
		''' <p>This uses size requirements of the superclass, modified to take into
		''' account the non-breakable areas at the adjacent views edges.  The minimal
		''' size requirements for such views should be no less than the sum of all
		''' adjacent fragments.</p>
		''' 
		''' <p>If the {@code axis} parameter is neither {@code View.X_AXIS} nor
		''' {@code View.Y_AXIS}, <seealso cref="IllegalArgumentException"/> is thrown.  If the
		''' {@code r} parameter is {@code null,} a new {@code SizeRequirements}
		''' object is created, otherwise the supplied {@code SizeRequirements}
		''' object is returned.</p>
		''' </summary>
		''' <param name="axis">  the minor axis </param>
		''' <param name="r">     the input {@code SizeRequirements} object </param>
		''' <returns>      the new or adjusted {@code SizeRequirements} object </returns>
		''' <exception cref="IllegalArgumentException">  if the {@code axis} parameter is invalid </exception>
		Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			r = MyBase.calculateMinorAxisRequirements(axis, r)

			Dim min As Single = 0
			Dim glue As Single = 0
			Dim n As Integer = layoutViewCount
			For i As Integer = 0 To n - 1
				Dim v As View = getLayoutView(i)
				Dim ___span As Single = v.getMinimumSpan(axis)
				If v.getBreakWeight(axis, 0, v.getMaximumSpan(axis)) > View.BadBreakWeight Then
					' find the longest non-breakable fragments at the view edges
					Dim p0 As Integer = v.startOffset
					Dim p1 As Integer = v.endOffset
					Dim start As Single = findEdgeSpan(v, axis, p0, p0, p1)
					Dim [end] As Single = findEdgeSpan(v, axis, p1, p0, p1)
					glue += start
					min = Math.Max(min, Math.Max(___span, glue))
					glue = [end]
				Else
					' non-breakable view
					glue += ___span
					min = Math.Max(min, glue)
				End If
			Next i
			r.minimum = Math.Max(r.minimum, CInt(Fix(min)))
			r.preferred = Math.Max(r.minimum, r.preferred)
			r.maximum = Math.Max(r.preferred, r.maximum)

			Return r
		End Function

		''' <summary>
		''' Binary search for the longest non-breakable fragment at the view edge.
		''' </summary>
		Private Function findEdgeSpan(ByVal v As View, ByVal axis As Integer, ByVal fp As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Single
			Dim len As Integer = p1 - p0
			If len <= 1 Then
				' further fragmentation is not possible
				Return v.getMinimumSpan(axis)
			Else
				Dim mid As Integer = p0 + len \ 2
				Dim startEdge As Boolean = mid > fp
				' initial view is breakable hence must support fragmentation
				Dim f As View = If(startEdge, v.createFragment(fp, mid), v.createFragment(mid, fp))
				Dim breakable As Boolean = f.getBreakWeight(axis, 0, f.getMaximumSpan(axis)) > View.BadBreakWeight
				If breakable = startEdge Then
					p1 = mid
				Else
					p0 = mid
				End If
				Return findEdgeSpan(f, axis, fp, p0, p1)
			End If
		End Function

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the
		'''  associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overrides Sub changedUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			' update any property settings stored, and layout should be
			' recomputed
			propertiesFromAttributestes()
			layoutChanged(X_AXIS)
			layoutChanged(Y_AXIS)
			MyBase.changedUpdate(changes, a, f)
		End Sub


		' --- variables -----------------------------------------------

		Private justification As Integer
		Private lineSpacing As Single
		''' <summary>
		''' Indentation for the first line, from the left inset. </summary>
		Protected Friend firstLineIndent As Integer = 0

		''' <summary>
		''' Used by the TabExpander functionality to determine
		''' where to base the tab calculations.  This is basically
		''' the location of the left side of the paragraph.
		''' </summary>
		Private tabBase As Integer

		''' <summary>
		''' Used to create an i18n-based layout strategy
		''' </summary>
		Friend Shared i18nStrategy As Type

		''' <summary>
		''' Used for searching for a tab. </summary>
		Friend Shared tabChars As Char()
		''' <summary>
		''' Used for searching for a tab or decimal character. </summary>
		Friend Shared tabDecimalChars As Char()

		Shared Sub New()
			tabChars = New Char(0){}
			tabChars(0) = ControlChars.Tab
			tabDecimalChars = New Char(1){}
			tabDecimalChars(0) = ControlChars.Tab
			tabDecimalChars(1) = "."c
		End Sub

		''' <summary>
		''' Internally created view that has the purpose of holding
		''' the views that represent the children of the paragraph
		''' that have been arranged in rows.
		''' </summary>
		Friend Class Row
			Inherits BoxView

			Private ReadOnly outerInstance As ParagraphView


			Friend Sub New(ByVal outerInstance As ParagraphView, ByVal elem As Element)
					Me.outerInstance = outerInstance
				MyBase.New(elem, View.X_AXIS)
			End Sub

			''' <summary>
			''' This is reimplemented to do nothing since the
			''' paragraph fills in the row with its needed
			''' children.
			''' </summary>
			Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
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

			Public Overrides Function getAlignment(ByVal axis As Integer) As Single
				If axis = View.X_AXIS Then
					Select Case outerInstance.justification
					Case StyleConstants.ALIGN_LEFT
						Return 0
					Case StyleConstants.ALIGN_RIGHT
						Return 1
					Case StyleConstants.ALIGN_CENTER
						Return 0.5f
					Case StyleConstants.ALIGN_JUSTIFIED
						Dim rv As Single = 0.5f
						'if we can justifiy the content always align to
						'the left.
						If justifiableDocument Then rv = 0f
						Return rv
					End Select
				End If
				Return MyBase.getAlignment(axis)
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.  This is
			''' implemented to let the superclass find the position along
			''' the major axis and the allocation of the row is used
			''' along the minor axis, so that even though the children
			''' are different heights they all get the same caret height.
			''' </summary>
			''' <param name="pos"> the position to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position </returns>
			''' <exception cref="BadLocationException">  if the given position does not represent a
			'''   valid location in the associated document </exception>
			''' <seealso cref= View#modelToView </seealso>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				Dim r As Rectangle = a.bounds
				Dim v As View = getViewAtPosition(pos, r)
				If (v IsNot Nothing) AndAlso ((Not v.element.leaf)) Then Return MyBase.modelToView(pos, a, b)
				r = a.bounds
				Dim ___height As Integer = r.height
				Dim y As Integer = r.y
				Dim loc As Shape = MyBase.modelToView(pos, a, b)
				r = loc.bounds
				r.height = ___height
				r.y = y
				Return r
			End Function

			''' <summary>
			''' Range represented by a row in the paragraph is only
			''' a subset of the total range of the paragraph element. </summary>
			''' <seealso cref= View#getRange </seealso>
			Public Property Overrides startOffset As Integer
				Get
					Dim offs As Integer = Integer.MaxValue
					Dim n As Integer = viewCount
					For i As Integer = 0 To n - 1
						Dim v As View = getView(i)
						offs = Math.Min(offs, v.startOffset)
					Next i
					Return offs
				End Get
			End Property

			Public Property Overrides endOffset As Integer
				Get
					Dim offs As Integer = 0
					Dim n As Integer = viewCount
					For i As Integer = 0 To n - 1
						Dim v As View = getView(i)
						offs = Math.Max(offs, v.endOffset)
					Next i
					Return offs
				End Get
			End Property

			''' <summary>
			''' Perform layout for the minor axis of the box (i.e. the
			''' axis orthogonal to the axis that it represents).  The results
			''' of the layout should be placed in the given arrays which represent
			''' the allocations to the children along the minor axis.
			''' <p>
			''' This is implemented to do a baseline layout of the children
			''' by calling BoxView.baselineLayout.
			''' </summary>
			''' <param name="targetSpan"> the total span given to the view, which
			'''  would be used to layout the children. </param>
			''' <param name="axis"> the axis being layed out. </param>
			''' <param name="offsets"> the offsets from the origin of the view for
			'''  each of the child views.  This is a return value and is
			'''  filled in by the implementation of this method. </param>
			''' <param name="spans"> the span of each child view.  This is a return
			'''  value and is filled in by the implementation of this method. </param>
			''' <returns> the offset and span for each child view in the
			'''  offsets and spans parameters </returns>
			Protected Friend Overrides Sub layoutMinorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
				baselineLayout(targetSpan, axis, offsets, spans)
			End Sub

			Protected Friend Overrides Function calculateMinorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
				Return baselineRequirements(axis, r)
			End Function


			Private Property lastRow As Boolean
				Get
					Dim ___parent As View
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return ((___parent = parent) Is Nothing OrElse Me Is ___parent.getView(___parent.viewCount - 1))
				End Get
			End Property

			Private Property brokenRow As Boolean
				Get
					Dim rv As Boolean = False
					Dim viewsCount As Integer = viewCount
					If viewsCount > 0 Then
						Dim lastView As View = getView(viewsCount - 1)
						If lastView.getBreakWeight(X_AXIS, 0, 0) >= ForcedBreakWeight Then rv = True
					End If
					Return rv
				End Get
			End Property

			Private Property justifiableDocument As Boolean
				Get
					Return ((Not Boolean.TRUE.Equals(document.getProperty(AbstractDocument.I18NProperty))))
				End Get
			End Property

			''' <summary>
			''' Whether we need to justify this {@code Row}.
			''' At this time (jdk1.6) we support justification on for non
			''' 18n text.
			''' </summary>
			''' <returns> {@code true} if this {@code Row} should be justified. </returns>
			Private Property justifyEnabled As Boolean
				Get
					Dim ret As Boolean = (outerInstance.justification = StyleConstants.ALIGN_JUSTIFIED)
    
					'no justification for i18n documents
					ret = ret AndAlso justifiableDocument
    
					'no justification for the last row
					ret = ret AndAlso Not lastRow
    
					'no justification for the broken rows
					ret = ret AndAlso Not brokenRow
    
					Return ret
				End Get
			End Property


			'Calls super method after setting spaceAddon to 0.
			'Justification should not affect MajorAxisRequirements
			Protected Friend Overrides Function calculateMajorAxisRequirements(ByVal axis As Integer, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
				Dim oldJustficationData As Integer() = justificationData
				justificationData = Nothing
				Dim ret As javax.swing.SizeRequirements = MyBase.calculateMajorAxisRequirements(axis, r)
				If justifyEnabled Then justificationData = oldJustficationData
				Return ret
			End Function

			Protected Friend Overrides Sub layoutMajorAxis(ByVal targetSpan As Integer, ByVal axis As Integer, ByVal offsets As Integer(), ByVal spans As Integer())
				Dim oldJustficationData As Integer() = justificationData
				justificationData = Nothing
				MyBase.layoutMajorAxis(targetSpan, axis, offsets, spans)
				If Not justifyEnabled Then Return

				Dim currentSpan As Integer = 0
				For Each ___span As Integer In spans
					currentSpan += ___span
				Next ___span
				If currentSpan = targetSpan Then Return

				' we justify text by enlarging spaces by the {@code spaceAddon}.
				' justification is started to the right of the rightmost TAB.
				' leading and trailing spaces are not extendable.
				'
				' GlyphPainter1 uses
				' justificationData
				' for all painting and measurement.

				Dim extendableSpaces As Integer = 0
				Dim startJustifiableContent As Integer = -1
				Dim endJustifiableContent As Integer = -1
				Dim lastLeadingSpaces As Integer = 0

				Dim rowStartOffset As Integer = startOffset
				Dim rowEndOffset As Integer = endOffset
				Dim spaceMap As Integer() = New Integer(rowEndOffset - rowStartOffset - 1){}
				java.util.Arrays.fill(spaceMap, 0)
				For i As Integer = viewCount - 1 To 0 Step -1
					Dim ___view As View = getView(i)
					If TypeOf ___view Is GlyphView Then
						Dim justificationInfo As GlyphView.JustificationInfo = CType(___view, GlyphView).getJustificationInfo(rowStartOffset)
						Dim viewStartOffset As Integer = ___view.startOffset
						Dim ___offset As Integer = viewStartOffset - rowStartOffset
						For j As Integer = 0 To justificationInfo.spaceMap.length() - 1
							If justificationInfo.spaceMap.Get(j) Then spaceMap(j + ___offset) = 1
						Next j
						If startJustifiableContent > 0 Then
							If justificationInfo.end >= 0 Then
								extendableSpaces += justificationInfo.trailingSpaces
							Else
								lastLeadingSpaces += justificationInfo.trailingSpaces
							End If
						End If
						If justificationInfo.start >= 0 Then
							startJustifiableContent = justificationInfo.start + viewStartOffset
							extendableSpaces += lastLeadingSpaces
						End If
						If justificationInfo.end >= 0 AndAlso endJustifiableContent < 0 Then endJustifiableContent = justificationInfo.end + viewStartOffset
						extendableSpaces += justificationInfo.contentSpaces
						lastLeadingSpaces = justificationInfo.leadingSpaces
						If justificationInfo.hasTab Then Exit For
					End If
				Next i
				If extendableSpaces <= 0 Then Return
				Dim adjustment As Integer = (targetSpan - currentSpan)
				Dim spaceAddon As Integer = If(extendableSpaces > 0, adjustment \ extendableSpaces, 0)
				Dim spaceAddonLeftoverEnd As Integer = -1
				Dim i As Integer = startJustifiableContent - rowStartOffset
				Dim leftover As Integer = adjustment - spaceAddon * extendableSpaces
				Do While leftover > 0
					spaceAddonLeftoverEnd = i
					leftover -= spaceMap(i)
					i += 1
				Loop
				If spaceAddon > 0 OrElse spaceAddonLeftoverEnd >= 0 Then
					justificationData = If(oldJustficationData IsNot Nothing, oldJustficationData, New Integer(END_JUSTIFIABLE){})
					justificationData(SPACE_ADDON) = spaceAddon
					justificationData(SPACE_ADDON_LEFTOVER_END) = spaceAddonLeftoverEnd
					justificationData(START_JUSTIFIABLE) = startJustifiableContent - rowStartOffset
					justificationData(END_JUSTIFIABLE) = endJustifiableContent - rowStartOffset
					MyBase.layoutMajorAxis(targetSpan, axis, offsets, spans)
				End If
			End Sub

			'for justified row we assume the maximum horizontal span
			'is MAX_VALUE.
			Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
				Dim ret As Single
				If View.X_AXIS = axis AndAlso justifyEnabled Then
					ret = Single.MaxValue
				Else
				  ret = MyBase.getMaximumSpan(axis)
				End If
				Return ret
			End Function

			''' <summary>
			''' Fetches the child view index representing the given position in
			''' the model.
			''' </summary>
			''' <param name="pos"> the position &gt;= 0 </param>
			''' <returns>  index of the view representing the given position, or
			'''   -1 if no view represents that position </returns>
			Protected Friend Overrides Function getViewIndexAtPosition(ByVal pos As Integer) As Integer
				' This is expensive, but are views are not necessarily layed
				' out in model order.
				If pos < startOffset OrElse pos >= endOffset Then Return -1
				For counter As Integer = viewCount - 1 To 0 Step -1
					Dim v As View = getView(counter)
					If pos >= v.startOffset AndAlso pos < v.endOffset Then Return counter
				Next counter
				Return -1
			End Function

			''' <summary>
			''' Gets the left inset.
			''' </summary>
			''' <returns> the inset </returns>
			Protected Friend Property Overrides leftInset As Short
				Get
					Dim parentView As View
					Dim adjustment As Integer = 0
					parentView = parent
					If parentView IsNot Nothing Then 'use firstLineIdent for the first row
						If Me Is parentView.getView(0) Then adjustment = outerInstance.firstLineIndent
					End If
					Return CShort(Fix(MyBase.leftInset + adjustment))
				End Get
			End Property

			Protected Friend Property Overrides bottomInset As Short
				Get
					Return CShort(Fix(MyBase.bottomInset + (If(minorRequest IsNot Nothing, minorRequest.preferred, 0)) * outerInstance.lineSpacing))
				End Get
			End Property

			Friend Const SPACE_ADDON As Integer = 0
			Friend Const SPACE_ADDON_LEFTOVER_END As Integer = 1
			Friend Const START_JUSTIFIABLE As Integer = 2
			'this should be the last index in justificationData
			Friend Const END_JUSTIFIABLE As Integer = 3

			Friend justificationData As Integer() = Nothing
		End Class

	End Class

End Namespace