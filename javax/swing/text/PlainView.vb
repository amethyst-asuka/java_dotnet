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
	''' Implements View interface for a simple multi-line text view
	''' that has text in one font and color.  The view represents each
	''' child element as a line of text.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     View </seealso>
	Public Class PlainView
		Inherits View
		Implements TabExpander

		''' <summary>
		''' Constructs a new PlainView wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
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
		''' <param name="lineIndex"> the line to draw &gt;= 0 </param>
		''' <param name="g"> the <code>Graphics</code> context </param>
		''' <param name="x"> the starting X position &gt;= 0 </param>
		''' <param name="y"> the starting Y position &gt;= 0 </param>
		''' <seealso cref= #drawUnselectedText </seealso>
		''' <seealso cref= #drawSelectedText </seealso>
		Protected Friend Overridable Sub drawLine(ByVal lineIndex As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim line As Element = element.getElement(lineIndex)
			Dim elem As Element

			Try
				If line.leaf Then
					drawElement(lineIndex, line, g, x, y)
				Else
					' this line contains the composed text.
					Dim count As Integer = line.elementCount
					For i As Integer = 0 To count - 1
						elem = line.getElement(i)
						x = drawElement(lineIndex, elem, g, x, y)
					Next i
				End If
			Catch e As BadLocationException
				Throw New StateInvariantError("Can't render line: " & lineIndex)
			End Try
		End Sub

		Private Function drawElement(ByVal lineIndex As Integer, ByVal elem As Element, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer) As Integer
			Dim p0 As Integer = elem.startOffset
			Dim p1 As Integer = elem.endOffset
			p1 = Math.Min(document.length, p1)

			If lineIndex = 0 Then x += firstLineOffset
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
		''' text.  Uses the foreground or disabled color to render the text.
		''' </summary>
		''' <param name="g"> the graphics context </param>
		''' <param name="x"> the starting X coordinate &gt;= 0 </param>
		''' <param name="y"> the starting Y coordinate &gt;= 0 </param>
		''' <param name="p0"> the beginning position in the model &gt;= 0 </param>
		''' <param name="p1"> the ending position in the model &gt;= 0 </param>
		''' <returns> the X location of the end of the range &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the range is invalid </exception>
		Protected Friend Overridable Function drawUnselectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			g.color = unselected
			Dim doc As Document = document
			Dim s As Segment = SegmentCache.sharedSegment
			doc.getText(p0, p1 - p0, s)
			Dim ret As Integer = Utilities.drawTabbedText(Me, s, x, y, g, Me, p0)
			SegmentCache.releaseSharedSegment(s)
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
		''' <param name="p1"> the ending position in the model &gt;= 0 </param>
		''' <returns> the location of the end of the range </returns>
		''' <exception cref="BadLocationException"> if the range is invalid </exception>
		Protected Friend Overridable Function drawSelectedText(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			g.color = selected
			Dim doc As Document = document
			Dim s As Segment = SegmentCache.sharedSegment
			doc.getText(p0, p1 - p0, s)
			Dim ret As Integer = Utilities.drawTabbedText(Me, s, x, y, g, Me, p0)
			SegmentCache.releaseSharedSegment(s)
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
		''' Checks to see if the font metrics and longest line
		''' are up-to-date.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Sub updateMetrics()
			Dim host As Component = container
			Dim f As Font = host.font
			If font IsNot f Then
				' The font changed, we need to recalculate the
				' longest line.
				calculateLongestLine()
				tabSize = tabSize * metrics.charWidth("m"c)
			End If
		End Sub

		' ---- View methods ----------------------------------------------------

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			updateMetrics()
			Select Case axis
			Case View.X_AXIS
				Return getLineWidth(longLine)
			Case View.Y_AXIS
				Return element.elementCount * metrics.height
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Renders using the given rendering surface and area on that surface.
		''' The view may need to do layout and create child views to enable
		''' itself to render into the given allocation.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into
		''' </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			Dim originalA As Shape = a
			a = adjustPaintRegion(a)
			Dim alloc As Rectangle = CType(a, Rectangle)
			tabBase = alloc.x
			Dim host As JTextComponent = CType(container, JTextComponent)
			Dim h As Highlighter = host.highlighter
			g.font = host.font
			sel0 = host.selectionStart
			sel1 = host.selectionEnd
			unselected = If(host.enabled, host.foreground, host.disabledTextColor)
			Dim c As Caret = host.caret
			selected = If(c.selectionVisible AndAlso h IsNot Nothing, host.selectedTextColor, unselected)
			updateMetrics()

			' If the lines are clipped then we don't expend the effort to
			' try and paint them.  Since all of the lines are the same height
			' with this object, determination of what lines need to be repainted
			' is quick.
			Dim clip As Rectangle = g.clipBounds
			Dim fontHeight As Integer = metrics.height
			Dim heightBelow As Integer = (alloc.y + alloc.height) - (clip.y + clip.height)
			Dim heightAbove As Integer = clip.y - alloc.y
			Dim linesBelow, linesAbove, linesTotal As Integer

			If fontHeight > 0 Then
				linesBelow = Math.Max(0, heightBelow \ fontHeight)
				linesAbove = Math.Max(0, heightAbove \ fontHeight)
				linesTotal = alloc.height / fontHeight
				If alloc.height Mod fontHeight <> 0 Then linesTotal += 1
			Else
					linesTotal = 0
						linesAbove = linesTotal
						linesBelow = linesAbove
			End If

			' update the visible lines
			Dim lineArea As Rectangle = lineToRect(a, linesAbove)
			Dim y As Integer = lineArea.y + metrics.ascent
			Dim x As Integer = lineArea.x
			Dim map As Element = element
			Dim lineCount As Integer = map.elementCount
			Dim endLine As Integer = Math.Min(lineCount, linesTotal - linesBelow)
			lineCount -= 1
			Dim dh As LayeredHighlighter = If(TypeOf h Is LayeredHighlighter, CType(h, LayeredHighlighter), Nothing)
			For line As Integer = linesAbove To endLine - 1
				If dh IsNot Nothing Then
					Dim lineElement As Element = map.getElement(line)
					If line = lineCount Then
						dh.paintLayeredHighlights(g, lineElement.startOffset, lineElement.endOffset, originalA, host, Me)
					Else
						dh.paintLayeredHighlights(g, lineElement.startOffset, lineElement.endOffset - 1, originalA, host, Me)
					End If
				End If
				drawLine(line, g, x, y)
				y += fontHeight
				If line = 0 Then x -= firstLineOffset
			Next line
		End Sub

		''' <summary>
		''' Should return a shape ideal for painting based on the passed in
		''' Shape <code>a</code>. This is useful if painting in a different
		''' region. The default implementation returns <code>a</code>.
		''' </summary>
		Friend Overridable Function adjustPaintRegion(ByVal a As Shape) As Shape
			Return a
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not
		'''   represent a valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			' line coordinates
			Dim doc As Document = document
			Dim map As Element = element
			Dim lineIndex As Integer = map.getElementIndex(pos)
			If lineIndex < 0 Then Return lineToRect(a, 0)
			Dim lineArea As Rectangle = lineToRect(a, lineIndex)

			' determine span from the start of the line
			tabBase = lineArea.x
			Dim line As Element = map.getElement(lineIndex)
			Dim p0 As Integer = line.startOffset
			Dim s As Segment = SegmentCache.sharedSegment
			doc.getText(p0, pos - p0, s)
			Dim xOffs As Integer = Utilities.getTabbedTextWidth(s, metrics, tabBase, Me,p0)
			SegmentCache.releaseSharedSegment(s)

			' fill in the results and return
			lineArea.x += xOffs
			lineArea.width = 1
			lineArea.height = metrics.height
			Return lineArea
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="fx"> the X coordinate &gt;= 0 </param>
		''' <param name="fy"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal fx As Single, ByVal fy As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			' PENDING(prinz) properly calculate bias
			bias(0) = Position.Bias.Forward

			Dim alloc As Rectangle = a.bounds
			Dim doc As Document = document
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
				Dim map As Element = doc.defaultRootElement
				Dim fontHeight As Integer = metrics.height
				Dim lineIndex As Integer = (If(fontHeight > 0, Math.Abs((y - alloc.y) / fontHeight), map.elementCount - 1))
				If lineIndex >= map.elementCount Then Return endOffset - 1
				Dim line As Element = map.getElement(lineIndex)
				Dim dx As Integer = 0
				If lineIndex = 0 Then
					alloc.x += firstLineOffset
					alloc.width -= firstLineOffset
				End If
				If x < alloc.x Then
					' point is to the left of the line
					Return line.startOffset
				ElseIf x > alloc.x + alloc.width Then
					' point is to the right of the line
					Return line.endOffset - 1
				Else
					' Determine the offset into the text
					Try
						Dim p0 As Integer = line.startOffset
						Dim p1 As Integer = line.endOffset - 1
						Dim s As Segment = SegmentCache.sharedSegment
						doc.getText(p0, p1 - p0, s)
						tabBase = alloc.x
						Dim offs As Integer = p0 + Utilities.getTabbedTextOffset(s, metrics, tabBase, x, Me, p0)
						SegmentCache.releaseSharedSegment(s)
						Return offs
					Catch e As BadLocationException
						' should not happen
						Return -1
					End Try
				End If
			End If
		End Function

		''' <summary>
		''' Gives notification that something was inserted into the document
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			updateDamage(changes, a, f)
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
			updateDamage(changes, a, f)
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
			updateDamage(changes, a, f)
		End Sub

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view along the given axis, if it
		''' has any layout duties.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
			MyBase.sizeize(width, height)
			updateMetrics()
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
			Dim ntabs As Integer = ((CInt(Fix(x))) - tabBase) \ tabSize
			Return tabBase + ((ntabs + 1) * tabSize)
		End Function

		' --- local methods ------------------------------------------------

		''' <summary>
		''' Repaint the region of change covered by the given document
		''' event.  Damages the line that begins the range to cover
		''' the case when the insert/remove is only on one line.
		''' If lines are added or removed, damages the whole
		''' view.  The longest line is checked to see if it has
		''' changed.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Sub updateDamage(ByVal changes As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			Dim host As Component = container
			updateMetrics()
			Dim elem As Element = element
			Dim ec As DocumentEvent.ElementChange = changes.getChange(elem)

			Dim added As Element() = If(ec IsNot Nothing, ec.childrenAdded, Nothing)
			Dim removed As Element() = If(ec IsNot Nothing, ec.childrenRemoved, Nothing)
			If ((added IsNot Nothing) AndAlso (added.Length > 0)) OrElse ((removed IsNot Nothing) AndAlso (removed.Length > 0)) Then
				' lines were added or removed...
				If added IsNot Nothing Then
					Dim currWide As Integer = getLineWidth(longLine)
					For i As Integer = 0 To added.Length - 1
						Dim w As Integer = getLineWidth(added(i))
						If w > currWide Then
							currWide = w
							longLine = added(i)
						End If
					Next i
				End If
				If removed IsNot Nothing Then
					For i As Integer = 0 To removed.Length - 1
						If removed(i) Is longLine Then
							calculateLongestLine()
							Exit For
						End If
					Next i
				End If
				preferenceChanged(Nothing, True, True)
				host.repaint()
			Else
				Dim map As Element = element
				Dim line As Integer = map.getElementIndex(changes.offset)
				damageLineRange(line, line, a, host)
				If changes.type Is DocumentEvent.EventType.INSERT Then
					' check to see if the line is longer than current
					' longest line.
					Dim w As Integer = getLineWidth(longLine)
					Dim e As Element = map.getElement(line)
					If e Is longLine Then
						preferenceChanged(Nothing, True, False)
					ElseIf getLineWidth(e) > w Then
						longLine = e
						preferenceChanged(Nothing, True, False)
					End If
				ElseIf changes.type Is DocumentEvent.EventType.REMOVE Then
					If map.getElement(line) Is longLine Then
						' removed from longest line... recalc
						calculateLongestLine()
						preferenceChanged(Nothing, True, False)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Repaint the given line range.
		''' </summary>
		''' <param name="host"> the component hosting the view (used to call repaint) </param>
		''' <param name="a">  the region allocated for the view to render into </param>
		''' <param name="line0"> the starting line number to repaint.  This must
		'''   be a valid line number in the model. </param>
		''' <param name="line1"> the ending line number to repaint.  This must
		'''   be a valid line number in the model.
		''' @since 1.4 </param>
		Protected Friend Overridable Sub damageLineRange(ByVal line0 As Integer, ByVal line1 As Integer, ByVal a As Shape, ByVal host As Component)
			If a IsNot Nothing Then
				Dim area0 As Rectangle = lineToRect(a, line0)
				Dim area1 As Rectangle = lineToRect(a, line1)
				If (area0 IsNot Nothing) AndAlso (area1 IsNot Nothing) Then
					Dim damage As Rectangle = area0.union(area1)
					host.repaint(damage.x, damage.y, damage.width, damage.height)
				Else
					host.repaint()
				End If
			End If
		End Sub

		''' <summary>
		''' Determine the rectangle that represents the given line.
		''' </summary>
		''' <param name="a">  the region allocated for the view to render into </param>
		''' <param name="line"> the line number to find the region of.  This must
		'''   be a valid line number in the model.
		''' @since 1.4 </param>
		Protected Friend Overridable Function lineToRect(ByVal a As Shape, ByVal line As Integer) As Rectangle
			Dim r As Rectangle = Nothing
			updateMetrics()
			If metrics IsNot Nothing Then
				Dim alloc As Rectangle = a.bounds
				If line = 0 Then
					alloc.x += firstLineOffset
					alloc.width -= firstLineOffset
				End If
				r = New Rectangle(alloc.x, alloc.y + (line * metrics.height), alloc.width, metrics.height)
			End If
			Return r
		End Function

		''' <summary>
		''' Iterate over the lines represented by the child elements
		''' of the element this view represents, looking for the line
		''' that is the longest.  The <em>longLine</em> variable is updated to
		''' represent the longest line contained.  The <em>font</em> variable
		''' is updated to indicate the font used to calculate the
		''' longest line.
		''' </summary>
		Private Sub calculateLongestLine()
			Dim c As Component = container
			font = c.font
			metrics = c.getFontMetrics(font)
			Dim doc As Document = document
			Dim lines As Element = element
			Dim n As Integer = lines.elementCount
			Dim maxWidth As Integer = -1
			For i As Integer = 0 To n - 1
				Dim line As Element = lines.getElement(i)
				Dim w As Integer = getLineWidth(line)
				If w > maxWidth Then
					maxWidth = w
					longLine = line
				End If
			Next i
		End Sub

		''' <summary>
		''' Calculate the width of the line represented by
		''' the given element.  It is assumed that the font
		''' and font metrics are up-to-date.
		''' </summary>
		Private Function getLineWidth(ByVal line As Element) As Integer
			If line Is Nothing Then Return 0
			Dim p0 As Integer = line.startOffset
			Dim p1 As Integer = line.endOffset
			Dim w As Integer
			Dim s As Segment = SegmentCache.sharedSegment
			Try
				line.document.getText(p0, p1 - p0, s)
				w = Utilities.getTabbedTextWidth(s, metrics, tabBase, Me, p0)
			Catch ble As BadLocationException
				w = 0
			End Try
			SegmentCache.releaseSharedSegment(s)
			Return w
		End Function

		' --- member variables -----------------------------------------------

		''' <summary>
		''' Font metrics for the current font.
		''' </summary>
		Protected Friend metrics As FontMetrics

		''' <summary>
		''' The current longest line.  This is used to calculate
		''' the preferred width of the view.  Since the calculation
		''' is potentially expensive we try to avoid it by stashing
		''' which line is currently the longest.
		''' </summary>
		Friend longLine As Element

		''' <summary>
		''' Font used to calculate the longest line... if this
		''' changes we need to recalculate the longest line
		''' </summary>
		Friend font As Font

		Friend lineBuffer As Segment
		Friend tabSize As Integer
		Friend tabBase As Integer

		Friend sel0 As Integer
		Friend sel1 As Integer
		Friend unselected As Color
		Friend selected As Color

		''' <summary>
		''' Offset of where to draw the first character on the first line.
		''' This is a hack and temporary until we can better address the problem
		''' of text measuring. This field is actually never set directly in
		''' PlainView, but by FieldView.
		''' </summary>
		Friend firstLineOffset As Integer

	End Class

End Namespace