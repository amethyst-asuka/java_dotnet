Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.event

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' View of plain text (text with only one font and color)
	''' that does line-wrapping.  This view expects that its
	''' associated element has child elements that represent
	''' the lines it should be wrapping.  It is implemented
	''' as a vertical box that contains logical line views.
	''' The logical line views are nested classes that render
	''' the logical line as multiple physical line if the logical
	''' line is too wide to fit within the allocation.  The
	''' line views draw upon the outer class for its state
	''' to reduce their memory requirements.
	''' <p>
	''' The line views do all of their rendering through the
	''' <code>drawLine</code> method which in turn does all of
	''' its rendering through the <code>drawSelectedText</code>
	''' and <code>drawUnselectedText</code> methods.  This
	''' enables subclasses to easily specialize the rendering
	''' without concern for the layout aspects.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	Public Class WrappedPlainView
		Inherits BoxView
		Implements TabExpander

		''' <summary>
		''' Creates a new WrappedPlainView.  Lines will be wrapped
		''' on character boundaries.
		''' </summary>
		''' <param name="elem"> the element underlying the view </param>
		Public Sub New(ByVal elem As Element)
			Me.New(elem, False)
		End Sub

		''' <summary>
		''' Creates a new WrappedPlainView.  Lines can be wrapped on
		''' either character or word boundaries depending upon the
		''' setting of the wordWrap parameter.
		''' </summary>
		''' <param name="elem"> the element underlying the view </param>
		''' <param name="wordWrap"> should lines be wrapped on word boundaries? </param>
		Public Sub New(ByVal elem As Element, ByVal wordWrap As Boolean)
			MyBase.New(elem, Y_AXIS)
			Me.wordWrap = wordWrap
		End Sub

		''' <summary>
		''' Returns the tab size set for the document, defaulting to 8.
		''' </summary>
		''' <returns> the tab size </returns>
		Protected Friend Overridable Property tabSize As Integer
			Get
				Dim i As Integer? = CInt(Fix(document.getProperty(PlainDocument.tabSizeAttribute)))
				Dim ___size As Integer = If(i IsNot Nothing, i, 8)
				Return ___size
			End Get
		End Property

		''' <summary>
		''' Renders a line of text, suppressing whitespace at the end
		''' and expanding any tabs.  This is implemented to make calls
		''' to the methods <code>drawUnselectedText</code> and
		''' <code>drawSelectedText</code> so that the way selected and
		''' unselected text are rendered can be customized.
		''' </summary>
		''' <param name="p0"> the starting document location to use &gt;= 0 </param>
		''' <param name="p1"> the ending document location to use &gt;= p1 </param>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X position &gt;= 0 </param>
		''' <param name="y"> the starting Y position &gt;= 0 </param>
		''' <seealso cref= #drawUnselectedText </seealso>
		''' <seealso cref= #drawSelectedText </seealso>
		Protected Friend Overridable Sub drawLine(ByVal p0 As Integer, ByVal p1 As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim lineMap As Element = element
			Dim line As Element = lineMap.getElement(lineMap.getElementIndex(p0))
			Dim elem As Element

			Try
				If line.leaf Then
					 drawText(line, p0, p1, g, x, y)
				Else
					' this line contains the composed text.
					Dim idx As Integer = line.getElementIndex(p0)
					Dim lastIdx As Integer = line.getElementIndex(p1)
					Do While idx <= lastIdx
						elem = line.getElement(idx)
						Dim start As Integer = Math.Max(elem.startOffset, p0)
						Dim [end] As Integer = Math.Min(elem.endOffset, p1)
						x = drawText(elem, start, [end], g, x, y)
						idx += 1
					Loop
				End If
			Catch e As BadLocationException
				Throw New StateInvariantError("Can't render: " & p0 & "," & p1)
			End Try
		End Sub

		Private Function drawText(ByVal elem As Element, ByVal p0 As Integer, ByVal p1 As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer) As Integer
			p1 = Math.Min(document.length, p1)
			Dim attr As AttributeSet = elem.attributes

			If Utilities.isComposedTextAttributeDefined(attr) Then
				g.color = unselected
				x = Utilities.drawComposedText(Me, attr, g, x, y, p0-elem.startOffset, p1-elem.startOffset)
			Else
				If sel0 = sel1 OrElse selected Is unselected Then
					' no selection, or it is invisible
					x = drawUnselectedText(g, x, y, p0, p1)
				ElseIf (p0 >= sel0 AndAlso p0 <= sel1) AndAlso (p1 >= sel0 AndAlso p1 <= sel1) Then
					x = drawSelectedText(g, x, y, p0, p1)
				ElseIf sel0 >= p0 AndAlso sel0 <= p1 Then
					If sel1 >= p0 AndAlso sel1 <= p1 Then
						x = drawUnselectedText(g, x, y, p0, sel0)
						x = drawSelectedText(g, x, y, sel0, sel1)
						x = drawUnselectedText(g, x, y, sel1, p1)
					Else
						x = drawUnselectedText(g, x, y, p0, sel0)
						x = drawSelectedText(g, x, y, sel0, p1)
					End If
				ElseIf sel1 >= p0 AndAlso sel1 <= p1 Then
					x = drawSelectedText(g, x, y, p0, sel1)
					x = drawUnselectedText(g, x, y, sel1, p1)
				Else
					x = drawUnselectedText(g, x, y, p0, p1)
				End If
			End If

			Return x
		End Function

		''' <summary>
		''' Renders the given range in the model as normal unselected
		''' text.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="p0"> the beginning position in the model &gt;= 0 </param>
		''' <param name="p1"> the ending position in the model &gt;= p0 </param>
		''' <returns> the X location of the end of the range &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the range is invalid </exception>
		Protected Friend Overridable Function drawUnselectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			g.color = unselected
			Dim doc As Document = document
			Dim segment As Segment = SegmentCache.sharedSegment
			doc.getText(p0, p1 - p0, segment)
			Dim ret As Integer = Utilities.drawTabbedText(Me, segment, x, y, g, Me, p0)
			SegmentCache.releaseSharedSegment(segment)
			Return ret
		End Function

		''' <summary>
		''' Renders the given range in the model as selected text.  This
		''' is implemented to render the text in the color specified in
		''' the hosting component.  It assumes the highlighter will render
		''' the selected background.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="p0"> the beginning position in the model &gt;= 0 </param>
		''' <param name="p1"> the ending position in the model &gt;= p0 </param>
		''' <returns> the location of the end of the range. </returns>
		''' <exception cref="BadLocationException"> if the range is invalid </exception>
		Protected Friend Overridable Function drawSelectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			g.color = selected
			Dim doc As Document = document
			Dim segment As Segment = SegmentCache.sharedSegment
			doc.getText(p0, p1 - p0, segment)
			Dim ret As Integer = Utilities.drawTabbedText(Me, segment, x, y, g, Me, p0)
			SegmentCache.releaseSharedSegment(segment)
			Return ret
		End Function

		''' <summary>
		''' Gives access to a buffer that can be used to fetch
		''' text from the associated document.
		''' </summary>
		''' <returns> the buffer </returns>
		Protected Friend Property lineBuffer As Segment
			Get
				If lineBuffer Is Nothing Then lineBuffer = New Segment
				Return lineBuffer
			End Get
		End Property

		''' <summary>
		''' This is called by the nested wrapped line
		''' views to determine the break location.  This can
		''' be reimplemented to alter the breaking behavior.
		''' It will either break at word or character boundaries
		''' depending upon the break argument given at
		''' construction.
		''' </summary>
		Protected Friend Overridable Function calculateBreakPosition(ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			Dim p As Integer
			Dim segment As Segment = SegmentCache.sharedSegment
			loadText(segment, p0, p1)
			Dim currentWidth As Integer = width
			If wordWrap Then
				p = p0 + Utilities.getBreakLocation(segment, metrics, tabBase, tabBase + currentWidth, Me, p0)
			Else
				p = p0 + Utilities.getTabbedTextOffset(segment, metrics, tabBase, tabBase + currentWidth, Me, p0, False)
			End If
			SegmentCache.releaseSharedSegment(segment)
			Return p
		End Function

		''' <summary>
		''' Loads all of the children to initialize the view.
		''' This is called by the <code>setParent</code> method.
		''' Subclasses can reimplement this to initialize their
		''' child views in a different manner.  The default
		''' implementation creates a child view for each
		''' child element.
		''' </summary>
		''' <param name="f"> the view factory </param>
		Protected Friend Overrides Sub loadChildren(ByVal f As ViewFactory)
			Dim e As Element = element
			Dim n As Integer = e.elementCount
			If n > 0 Then
				Dim added As View() = New View(n - 1){}
				For i As Integer = 0 To n - 1
					added(i) = New WrappedLine(Me, e.getElement(i))
				Next i
				replace(0, 0, added)
			End If
		End Sub

		''' <summary>
		''' Update the child views in response to a
		''' document event.
		''' </summary>
		Friend Overridable Sub updateChildren(ByVal e As DocumentEvent, ByVal a As Shape)
			Dim elem As Element = element
			Dim ec As DocumentEvent.ElementChange = e.getChange(elem)
			If ec IsNot Nothing Then
				' the structure of this element changed.
				Dim removedElems As Element() = ec.childrenRemoved
				Dim addedElems As Element() = ec.childrenAdded
				Dim added As View() = New View(addedElems.Length - 1){}
				For i As Integer = 0 To addedElems.Length - 1
					added(i) = New WrappedLine(Me, addedElems(i))
				Next i
				replace(ec.index, removedElems.Length, added)

				' should damge a little more intelligently.
				If a IsNot Nothing Then
					preferenceChanged(Nothing, True, True)
					container.repaint()
				End If
			End If

			' update font metrics which may be used by the child views
			updateMetrics()
		End Sub

		''' <summary>
		''' Load the text buffer with the given range
		''' of text.  This is used by the fragments
		''' broken off of this view as well as this
		''' view itself.
		''' </summary>
		Friend Sub loadText(ByVal segment As Segment, ByVal p0 As Integer, ByVal p1 As Integer)
			Try
				Dim doc As Document = document
				doc.getText(p0, p1 - p0, segment)
			Catch bl As BadLocationException
				Throw New StateInvariantError("Can't get line text")
			End Try
		End Sub

		Friend Sub updateMetrics()
			Dim host As Component = container
			Dim f As Font = host.font
			metrics = host.getFontMetrics(f)
			tabSize = tabSize * metrics.charWidth("m"c)
		End Sub

		' --- TabExpander methods ------------------------------------------

		''' <summary>
		''' Returns the next tab stop position after a given reference position.
		''' This implementation does not support things like centering so it
		''' ignores the tabOffset argument.
		''' </summary>
		''' <param name="x"> the current position &gt;= 0 </param>
		''' <param name="tabOffset"> the position within the text stream
		'''   that the tab occurred at &gt;= 0. </param>
		''' <returns> the tab stop, measured in points &gt;= 0 </returns>
		Public Overridable Function nextTabStop(ByVal x As Single, ByVal tabOffset As Integer) As Single Implements TabExpander.nextTabStop
			If tabSize = 0 Then Return x
			Dim ntabs As Integer = (CInt(Fix(x)) - tabBase) \ tabSize
			Return tabBase + ((ntabs + 1) * tabSize)
		End Function


		' --- View methods -------------------------------------

		''' <summary>
		''' Renders using the given rendering surface and area
		''' on that surface.  This is implemented to stash the
		''' selection positions, selection colors, and font
		''' metrics for the nested lines to use.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into
		''' </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim alloc As Rectangle = CType(a, Rectangle)
			tabBase = alloc.x
			Dim host As JTextComponent = CType(container, JTextComponent)
			sel0 = host.selectionStart
			sel1 = host.selectionEnd
			unselected = If(host.enabled, host.foreground, host.disabledTextColor)
			Dim c As Caret = host.caret
			selected = If(c.selectionVisible AndAlso host.highlighter IsNot Nothing, host.selectedTextColor, unselected)
			g.font = host.font

			' superclass paints the children
			MyBase.paint(g, a)
		End Sub

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view along the given axis, if it
		''' has any layout duties.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
			updateMetrics()
			If CInt(Fix(width)) <> width Then
				' invalidate the view itself since the desired widths
				' of the children will be based upon this views width.
				preferenceChanged(Nothing, True, True)
				widthChanging = True
			End If
			MyBase.sizeize(width, height)
			widthChanging = False
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.  This is implemented to provide the superclass
		''' behavior after first making sure that the current font
		''' metrics are cached (for the nested lines which use
		''' the metrics to determine the height of the potentially
		''' wrapped lines).
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>  the span the view would like to be rendered into.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <seealso cref= View#getPreferredSpan </seealso>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			updateMetrics()
			Return MyBase.getPreferredSpan(axis)
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.  This is implemented to provide the superclass
		''' behavior after first making sure that the current font
		''' metrics are cached (for the nested lines which use
		''' the metrics to determine the height of the potentially
		''' wrapped lines).
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>  the span the view would like to be rendered into.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <seealso cref= View#getMinimumSpan </seealso>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			updateMetrics()
			Return MyBase.getMinimumSpan(axis)
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.  This is implemented to provide the superclass
		''' behavior after first making sure that the current font
		''' metrics are cached (for the nested lines which use
		''' the metrics to determine the height of the potentially
		''' wrapped lines).
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>  the span the view would like to be rendered into.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <seealso cref= View#getMaximumSpan </seealso>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			updateMetrics()
			Return MyBase.getMaximumSpan(axis)
		End Function

		''' <summary>
		''' Gives notification that something was inserted into the
		''' document in a location that this view is responsible for.
		''' This is implemented to simply update the children.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			updateChildren(e, a)

			Dim alloc As Rectangle = If((a IsNot Nothing) AndAlso allocationValid, getInsideAllocation(a), Nothing)
			Dim pos As Integer = e.offset
			Dim v As View = getViewAtPosition(pos, alloc)
			If v IsNot Nothing Then v.insertUpdate(e, alloc, f)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the
		''' document in a location that this view is responsible for.
		''' This is implemented to simply update the children.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overrides Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			updateChildren(e, a)

			Dim alloc As Rectangle = If((a IsNot Nothing) AndAlso allocationValid, getInsideAllocation(a), Nothing)
			Dim pos As Integer = e.offset
			Dim v As View = getViewAtPosition(pos, alloc)
			If v IsNot Nothing Then v.removeUpdate(e, alloc, f)
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			updateChildren(e, a)
		End Sub

		' --- variables -------------------------------------------

		Friend metrics As FontMetrics
		Friend lineBuffer As Segment
		Friend widthChanging As Boolean
		Friend tabBase As Integer
		Friend tabSize As Integer
		Friend wordWrap As Boolean

		Friend sel0 As Integer
		Friend sel1 As Integer
		Friend unselected As Color
		Friend selected As Color


		''' <summary>
		''' Simple view of a line that wraps if it doesn't
		''' fit withing the horizontal space allocated.
		''' This class tries to be lightweight by carrying little
		''' state of it's own and sharing the state of the outer class
		''' with it's sibblings.
		''' </summary>
		Friend Class WrappedLine
			Inherits View

			Private ReadOnly outerInstance As WrappedPlainView


			Friend Sub New(ByVal outerInstance As WrappedPlainView, ByVal elem As Element)
					Me.outerInstance = outerInstance
				MyBase.New(elem)
				lineCount = -1
			End Sub

			''' <summary>
			''' Determines the preferred span for this view along an
			''' axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns>   the span the view would like to be rendered into.
			'''           Typically the view is told to render into the span
			'''           that is returned, although there is no guarantee.
			'''           The parent may choose to resize or break the view. </returns>
			''' <seealso cref= View#getPreferredSpan </seealso>
			Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
				Select Case axis
				Case View.X_AXIS
					Dim width As Single = outerInstance.width
					If width = Integer.MaxValue Then Return 100f
					Return width
				Case View.Y_AXIS
					If lineCount < 0 OrElse outerInstance.widthChanging Then breakLines(startOffset)
					Return lineCount * outerInstance.metrics.height
				Case Else
					Throw New System.ArgumentException("Invalid axis: " & axis)
				End Select
			End Function

			''' <summary>
			''' Renders using the given rendering surface and area on that
			''' surface.  The view may need to do layout and create child
			''' views to enable itself to render into the given allocation.
			''' </summary>
			''' <param name="g"> the rendering surface to use </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <seealso cref= View#paint </seealso>
			Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
				Dim alloc As Rectangle = CType(a, Rectangle)
				Dim y As Integer = alloc.y + outerInstance.metrics.ascent
				Dim x As Integer = alloc.x

				Dim host As JTextComponent = CType(container, JTextComponent)
				Dim h As Highlighter = host.highlighter
				Dim dh As LayeredHighlighter = If(TypeOf h Is LayeredHighlighter, CType(h, LayeredHighlighter), Nothing)

				Dim start As Integer = startOffset
				Dim [end] As Integer = endOffset
				Dim p0 As Integer = start
				Dim ___lineEnds As Integer() = lineEnds
				For i As Integer = 0 To lineCount - 1
					Dim p1 As Integer = If(___lineEnds Is Nothing, [end], start + ___lineEnds(i))
					If dh IsNot Nothing Then
						Dim hOffset As Integer = If(p1 = [end], (p1 - 1), p1)
						dh.paintLayeredHighlights(g, p0, hOffset, a, host, Me)
					End If
					outerInstance.drawLine(p0, p1, g, x, y)

					p0 = p1
					y += outerInstance.metrics.height
				Next i
			End Sub

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="pos"> the position to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position is returned </returns>
			''' <exception cref="BadLocationException">  if the given position does not represent a
			'''   valid location in the associated document </exception>
			''' <seealso cref= View#modelToView </seealso>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				Dim alloc As Rectangle = a.bounds
				alloc.height = outerInstance.metrics.height
				alloc.width = 1

				Dim p0 As Integer = startOffset
				If pos < p0 OrElse pos > endOffset Then Throw New BadLocationException("Position out of range", pos)

				Dim testP As Integer = If(b Is Position.Bias.Forward, pos, Math.Max(p0, pos - 1))
				Dim line As Integer = 0
				Dim ___lineEnds As Integer() = lineEnds
				If ___lineEnds IsNot Nothing Then
					line = findLine(testP - p0)
					If line > 0 Then p0 += ___lineEnds(line - 1)
					alloc.y += alloc.height * line
				End If

				If pos > p0 Then
					Dim segment As Segment = SegmentCache.sharedSegment
					outerInstance.loadText(segment, p0, pos)
					alloc.x += Utilities.getTabbedTextWidth(segment, outerInstance.metrics, alloc.x, WrappedPlainView.this, p0)
					SegmentCache.releaseSharedSegment(segment)
				End If
				Return alloc
			End Function

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.
			''' </summary>
			''' <param name="fx"> the X coordinate </param>
			''' <param name="fy"> the Y coordinate </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the location within the model that best represents the
			'''  given point in the view </returns>
			''' <seealso cref= View#viewToModel </seealso>
			Public Overrides Function viewToModel(ByVal fx As Single, ByVal fy As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
				' PENDING(prinz) implement bias properly
				bias(0) = Position.Bias.Forward

				Dim alloc As Rectangle = CType(a, Rectangle)
				Dim x As Integer = CInt(Fix(fx))
				Dim y As Integer = CInt(Fix(fy))
				If y < alloc.y Then
					' above the area covered by this icon, so the the position
					' is assumed to be the start of the coverage for this view.
					Return startOffset
				ElseIf y > alloc.y + alloc.height Then
					' below the area covered by this icon, so the the position
					' is assumed to be the end of the coverage for this view.
					Return endOffset - 1
				Else
					' positioned within the coverage of this view vertically,
					' so we figure out which line the point corresponds to.
					' if the line is greater than the number of lines contained, then
					' simply use the last line as it represents the last possible place
					' we can position to.
					alloc.height = outerInstance.metrics.height
					Dim line As Integer = (If(alloc.height > 0, (y - alloc.y) / alloc.height, lineCount - 1))
					If line >= lineCount Then
						Return endOffset - 1
					Else
						Dim p0 As Integer = startOffset
						Dim p1 As Integer
						If lineCount = 1 Then
							p1 = endOffset
						Else
							Dim ___lineEnds As Integer() = lineEnds
							p1 = p0 + ___lineEnds(line)
							If line > 0 Then p0 += ___lineEnds(line - 1)
						End If

						If x < alloc.x Then
							' point is to the left of the line
							Return p0
						ElseIf x > alloc.x + alloc.width Then
							' point is to the right of the line
							Return p1 - 1
						Else
							' Determine the offset into the text
							Dim segment As Segment = SegmentCache.sharedSegment
							outerInstance.loadText(segment, p0, p1)
							Dim n As Integer = Utilities.getTabbedTextOffset(segment, outerInstance.metrics, alloc.x, x, WrappedPlainView.this, p0)
							SegmentCache.releaseSharedSegment(segment)
							Return Math.Min(p0 + n, p1 - 1)
						End If
					End If
				End If
			End Function

			Public Overrides Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				update(e, a)
			End Sub

			Public Overrides Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
				update(e, a)
			End Sub

			Private Sub update(ByVal ev As DocumentEvent, ByVal a As Shape)
				Dim oldCount As Integer = lineCount
				breakLines(ev.offset)
				If oldCount <> lineCount Then
					outerInstance.preferenceChanged(Me, False, True)
					' have to repaint any views after the receiver.
					container.repaint()
				ElseIf a IsNot Nothing Then
					Dim c As Component = container
					Dim alloc As Rectangle = CType(a, Rectangle)
					c.repaint(alloc.x, alloc.y, alloc.width, alloc.height)
				End If
			End Sub

			''' <summary>
			''' Returns line cache. If the cache was GC'ed, recreates it.
			''' If there's no cache, returns null
			''' </summary>
			Friend Property lineEnds As Integer()
				Get
					If lineCache Is Nothing Then
						Return Nothing
					Else
						Dim ___lineEnds As Integer() = lineCache.get()
						If ___lineEnds Is Nothing Then
							' Cache was GC'ed, so rebuild it
							Return breakLines(startOffset)
						Else
							Return ___lineEnds
						End If
					End If
				End Get
			End Property

			''' <summary>
			''' Creates line cache if text breaks into more than one physical line. </summary>
			''' <param name="startPos"> position to start breaking from </param>
			''' <returns> the cache created, ot null if text breaks into one line </returns>
			Friend Function breakLines(ByVal startPos As Integer) As Integer()
				Dim ___lineEnds As Integer() = If(lineCache Is Nothing, Nothing, lineCache.get())
				Dim oldLineEnds As Integer() = ___lineEnds
				Dim start As Integer = startOffset
				Dim lineIndex As Integer = 0
				If ___lineEnds IsNot Nothing Then
					lineIndex = findLine(startPos - start)
					If lineIndex > 0 Then lineIndex -= 1
				End If

				Dim p0 As Integer = If(lineIndex = 0, start, start + ___lineEnds(lineIndex - 1))
				Dim p1 As Integer = endOffset
				Do While p0 < p1
					Dim p As Integer = outerInstance.calculateBreakPosition(p0, p1)
						p += 1
						p0 = If(p = p0, p, p)

					If lineIndex = 0 AndAlso p0 >= p1 Then
						' do not use cache if there's only one line
						lineCache = Nothing
						___lineEnds = Nothing
						lineIndex = 1
						Exit Do
					ElseIf ___lineEnds Is Nothing OrElse lineIndex >= ___lineEnds.Length Then
						' we have 2+ lines, and the cache is not big enough
						' we try to estimate total number of lines
						Dim growFactor As Double = (CDbl(p1 - start) / (p0 - start))
						Dim newSize As Integer = CInt(Fix(Math.Ceiling((lineIndex + 1) * growFactor)))
						newSize = Math.Max(newSize, lineIndex + 2)
						Dim tmp As Integer() = New Integer(newSize - 1){}
						If ___lineEnds IsNot Nothing Then Array.Copy(___lineEnds, 0, tmp, 0, lineIndex)
						___lineEnds = tmp
					End If
					___lineEnds(lineIndex) = p0 - start
					lineIndex += 1
				Loop

				lineCount = lineIndex
				If lineCount > 1 Then
					' check if the cache is too big
					Dim maxCapacity As Integer = lineCount + lineCount \ 3
					If ___lineEnds.Length > maxCapacity Then
						Dim tmp As Integer() = New Integer(maxCapacity - 1){}
						Array.Copy(___lineEnds, 0, tmp, 0, lineCount)
						___lineEnds = tmp
					End If
				End If

				If ___lineEnds IsNot Nothing AndAlso ___lineEnds <> oldLineEnds Then lineCache = New SoftReference(Of Integer())(___lineEnds)
				Return ___lineEnds
			End Function

			''' <summary>
			''' Binary search in the cache for line containing specified offset
			''' (which is relative to the beginning of the view). This method
			''' assumes that cache exists.
			''' </summary>
			Private Function findLine(ByVal offset As Integer) As Integer
				Dim ___lineEnds As Integer() = lineCache.get()
				If offset < ___lineEnds(0) Then
					Return 0
				ElseIf offset > ___lineEnds(lineCount - 1) Then
					Return lineCount
				Else
					Return findLine(___lineEnds, offset, 0, lineCount - 1)
				End If
			End Function

			Private Function findLine(ByVal array As Integer(), ByVal offset As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
				If max - min <= 1 Then
					Return max
				Else
					Dim mid As Integer = (max + min) \ 2
					Return If(offset < array(mid), findLine(array, offset, min, mid), findLine(array, offset, mid, max))
				End If
			End Function

			Friend lineCount As Integer
			Friend lineCache As SoftReference(Of Integer()) = Nothing
		End Class
	End Class

End Namespace