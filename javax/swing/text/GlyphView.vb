Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports javax.swing.event
import static sun.swing.SwingUtilities2.IMPLIED_CR

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
	''' A GlyphView is a styled chunk of text that represents a view
	''' mapped over an element in the text model. This view is generally
	''' responsible for displaying text glyphs using character level
	''' attributes in some way.
	''' An implementation of the GlyphPainter class is used to do the
	''' actual rendering and model/view translations.  This separates
	''' rendering from layout and management of the association with
	''' the model.
	''' <p>
	''' The view supports breaking for the purpose of formatting.
	''' The fragments produced by breaking share the view that has
	''' primary responsibility for the element (i.e. they are nested
	''' classes and carry only a small amount of state of their own)
	''' so they can share its resources.
	''' <p>
	''' Since this view
	''' represents text that may have tabs embedded in it, it implements the
	''' <code>TabableView</code> interface.  Tabs will only be
	''' expanded if this view is embedded in a container that does
	''' tab expansion.  ParagraphView is an example of a container
	''' that does tab expansion.
	''' <p>
	''' 
	''' @since 1.3
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class GlyphView
		Inherits View
		Implements TabableView, ICloneable

		''' <summary>
		''' Constructs a new view wrapped on an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			offset = 0
			length = 0
			Dim ___parent As Element = elem.parentElement
			Dim attr As AttributeSet = elem.attributes

			'         if there was an implied CR
			impliedCR = (attr IsNot Nothing AndAlso attr.getAttribute(IMPLIED_CR) IsNot Nothing AndAlso ___parent IsNot Nothing AndAlso ___parent.elementCount > 1)
			'         if this is non-empty paragraph
			skipWidth = elem.name.Equals("br")
		End Sub

		''' <summary>
		''' Creates a shallow copy.  This is used by the
		''' createFragment and breakView methods.
		''' </summary>
		''' <returns> the copy </returns>
		Protected Friend Function clone() As Object
			Dim o As Object
			Try
				o = MyBase.clone()
			Catch cnse As CloneNotSupportedException
				o = Nothing
			End Try
			Return o
		End Function

		''' <summary>
		''' Fetch the currently installed glyph painter.
		''' If a painter has not yet been installed, and
		''' a default was not yet needed, null is returned.
		''' </summary>
		Public Overridable Property glyphPainter As GlyphPainter
			Get
				Return painter
			End Get
			Set(ByVal p As GlyphPainter)
				painter = p
			End Set
		End Property


		''' <summary>
		''' Fetch a reference to the text that occupies
		''' the given range.  This is normally used by
		''' the GlyphPainter to determine what characters
		''' it should render glyphs for.
		''' </summary>
		''' <param name="p0">  the starting document offset &gt;= 0 </param>
		''' <param name="p1">  the ending document offset &gt;= p0 </param>
		''' <returns>    the <code>Segment</code> containing the text </returns>
		 Public Overridable Function getText(ByVal p0 As Integer, ByVal p1 As Integer) As Segment
			 ' When done with the returned Segment it should be released by
			 ' invoking:
			 '    SegmentCache.releaseSharedSegment(segment);
			 Dim ___text As Segment = SegmentCache.sharedSegment
			 Try
				 Dim doc As Document = document
				 doc.getText(p0, p1 - p0, ___text)
			 Catch bl As BadLocationException
				 Throw New StateInvariantError("GlyphView: Stale view: " & bl)
			 End Try
			 Return ___text
		 End Function

		''' <summary>
		''' Fetch the background color to use to render the
		''' glyphs.  If there is no background color, null should
		''' be returned.  This is implemented to call
		''' <code>StyledDocument.getBackground</code> if the associated
		''' document is a styled document, otherwise it returns null.
		''' </summary>
		Public Overridable Property background As Color
			Get
				Dim doc As Document = document
				If TypeOf doc Is StyledDocument Then
					Dim attr As AttributeSet = attributes
					If attr.isDefined(StyleConstants.Background) Then Return CType(doc, StyledDocument).getBackground(attr)
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Fetch the foreground color to use to render the
		''' glyphs.  If there is no foreground color, null should
		''' be returned.  This is implemented to call
		''' <code>StyledDocument.getBackground</code> if the associated
		''' document is a StyledDocument.  If the associated document
		''' is not a StyledDocument, the associated components foreground
		''' color is used.  If there is no associated component, null
		''' is returned.
		''' </summary>
		Public Overridable Property foreground As Color
			Get
				Dim doc As Document = document
				If TypeOf doc Is StyledDocument Then
					Dim attr As AttributeSet = attributes
					Return CType(doc, StyledDocument).getForeground(attr)
				End If
				Dim c As Component = container
				If c IsNot Nothing Then Return c.foreground
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Fetch the font that the glyphs should be based
		''' upon.  This is implemented to call
		''' <code>StyledDocument.getFont</code> if the associated
		''' document is a StyledDocument.  If the associated document
		''' is not a StyledDocument, the associated components font
		''' is used.  If there is no associated component, null
		''' is returned.
		''' </summary>
		Public Overridable Property font As Font
			Get
				Dim doc As Document = document
				If TypeOf doc Is StyledDocument Then
					Dim attr As AttributeSet = attributes
					Return CType(doc, StyledDocument).getFont(attr)
				End If
				Dim c As Component = container
				If c IsNot Nothing Then Return c.font
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Determine if the glyphs should be underlined.  If true,
		''' an underline should be drawn through the baseline.
		''' </summary>
		Public Overridable Property underline As Boolean
			Get
				Dim attr As AttributeSet = attributes
				Return StyleConstants.isUnderline(attr)
			End Get
		End Property

		''' <summary>
		''' Determine if the glyphs should have a strikethrough
		''' line.  If true, a line should be drawn through the center
		''' of the glyphs.
		''' </summary>
		Public Overridable Property strikeThrough As Boolean
			Get
				Dim attr As AttributeSet = attributes
				Return StyleConstants.isStrikeThrough(attr)
			End Get
		End Property

		''' <summary>
		''' Determine if the glyphs should be rendered as superscript.
		''' </summary>
		Public Overridable Property subscript As Boolean
			Get
				Dim attr As AttributeSet = attributes
				Return StyleConstants.isSubscript(attr)
			End Get
		End Property

		''' <summary>
		''' Determine if the glyphs should be rendered as subscript.
		''' </summary>
		Public Overridable Property superscript As Boolean
			Get
				Dim attr As AttributeSet = attributes
				Return StyleConstants.isSuperscript(attr)
			End Get
		End Property

		''' <summary>
		''' Fetch the TabExpander to use if tabs are present in this view.
		''' </summary>
		Public Overridable Property tabExpander As TabExpander
			Get
				Return expander
			End Get
		End Property

		''' <summary>
		''' Check to see that a glyph painter exists.  If a painter
		''' doesn't exist, a default glyph painter will be installed.
		''' </summary>
		Protected Friend Overridable Sub checkPainter()
			If painter Is Nothing Then
				If defaultPainter Is Nothing Then
					' the classname should probably come from a property file.
					Dim classname As String = "javax.swing.text.GlyphPainter1"
					Try
						Dim c As Type
						Dim loader As ClassLoader = Me.GetType().classLoader
						If loader IsNot Nothing Then
							c = loader.loadClass(classname)
						Else
							c = Type.GetType(classname)
						End If
						Dim o As Object = c.newInstance()
						If TypeOf o Is GlyphPainter Then defaultPainter = CType(o, GlyphPainter)
					Catch e As Exception
						Throw New StateInvariantError("GlyphView: Can't load glyph painter: " & classname)
					End Try
				End If
				glyphPainter = defaultPainter.getPainter(Me, startOffset, endOffset)
			End If
		End Sub

		' --- TabableView methods --------------------------------------

		''' <summary>
		''' Determines the desired span when using the given
		''' tab expansion implementation.
		''' </summary>
		''' <param name="x"> the position the view would be located
		'''  at for the purpose of tab expansion &gt;= 0. </param>
		''' <param name="e"> how to expand the tabs when encountered. </param>
		''' <returns> the desired span &gt;= 0 </returns>
		''' <seealso cref= TabableView#getTabbedSpan </seealso>
		Public Overridable Function getTabbedSpan(ByVal x As Single, ByVal e As TabExpander) As Single Implements TabableView.getTabbedSpan
			checkPainter()

			Dim old As TabExpander = expander
			expander = e

			If expander IsNot old Then preferenceChanged(Nothing, True, False)

			Me.x = CInt(Fix(x))
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			Dim width As Single = painter.getSpan(Me, p0, p1, expander, x)
			Return width
		End Function

		''' <summary>
		''' Determines the span along the same axis as tab
		''' expansion for a portion of the view.  This is
		''' intended for use by the TabExpander for cases
		''' where the tab expansion involves aligning the
		''' portion of text that doesn't have whitespace
		''' relative to the tab stop.  There is therefore
		''' an assumption that the range given does not
		''' contain tabs.
		''' <p>
		''' This method can be called while servicing the
		''' getTabbedSpan or getPreferredSize.  It has to
		''' arrange for its own text buffer to make the
		''' measurements.
		''' </summary>
		''' <param name="p0"> the starting document offset &gt;= 0 </param>
		''' <param name="p1"> the ending document offset &gt;= p0 </param>
		''' <returns> the span &gt;= 0 </returns>
		Public Overridable Function getPartialSpan(ByVal p0 As Integer, ByVal p1 As Integer) As Single Implements TabableView.getPartialSpan
			checkPainter()
			Dim width As Single = painter.getSpan(Me, p0, p1, expander, x)
			Return width
		End Function

		' --- View methods ---------------------------------------------

		''' <summary>
		''' Fetches the portion of the model that this view is responsible for.
		''' </summary>
		''' <returns> the starting offset into the model </returns>
		''' <seealso cref= View#getStartOffset </seealso>
		Public Property Overrides startOffset As Integer
			Get
				Dim e As Element = element
				Return If(length > 0, e.startOffset + offset, e.startOffset)
			End Get
		End Property

		''' <summary>
		''' Fetches the portion of the model that this view is responsible for.
		''' </summary>
		''' <returns> the ending offset into the model </returns>
		''' <seealso cref= View#getEndOffset </seealso>
		Public Property Overrides endOffset As Integer
			Get
				Dim e As Element = element
				Return If(length > 0, e.startOffset + offset + length, e.endOffset)
			End Get
		End Property

		''' <summary>
		''' Lazily initializes the selections field
		''' </summary>
		Private Sub initSelections(ByVal p0 As Integer, ByVal p1 As Integer)
			Dim viewPosCount As Integer = p1 - p0 + 1
			If selections Is Nothing OrElse viewPosCount > selections.Length Then
				selections = New SByte(viewPosCount - 1){}
				Return
			End If
			Dim i As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i < viewPosCount

				selections(i += 1) = 0
			Loop
		End Sub

		''' <summary>
		''' Renders a portion of a text style run.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into </param>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			checkPainter()

			Dim paintedText As Boolean = False
			Dim c As Component = container
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			Dim bg As Color = background
			Dim fg As Color = foreground

			If c IsNot Nothing AndAlso (Not c.enabled) Then fg = (If(TypeOf c Is JTextComponent, CType(c, JTextComponent).disabledTextColor, javax.swing.UIManager.getColor("textInactiveText")))
			If bg IsNot Nothing Then
				g.color = bg
				g.fillRect(alloc.x, alloc.y, alloc.width, alloc.height)
			End If
			If TypeOf c Is JTextComponent Then
				Dim tc As JTextComponent = CType(c, JTextComponent)
				Dim h As Highlighter = tc.highlighter
				If TypeOf h Is LayeredHighlighter Then CType(h, LayeredHighlighter).paintLayeredHighlights(g, p0, p1, a, tc, Me)
			End If

			If Utilities.isComposedTextElement(element) Then
				Utilities.paintComposedText(g, a.bounds, Me)
				paintedText = True
			ElseIf TypeOf c Is JTextComponent Then
				Dim tc As JTextComponent = CType(c, JTextComponent)
				Dim selFG As Color = tc.selectedTextColor

				If (tc.highlighter IsNot Nothing) AndAlso (selFG IsNot Nothing) AndAlso (Not selFG.Equals(fg)) Then ' there's a highlighter (bug 4532590), and
					' selected text color is different from regular foreground

					Dim h As Highlighter.Highlight() = tc.highlighter.highlights
					If h.Length <> 0 Then
						Dim initialized As Boolean = False
						Dim viewSelectionCount As Integer = 0
						For i As Integer = 0 To h.Length - 1
							Dim highlight As Highlighter.Highlight = h(i)
							Dim hStart As Integer = highlight.startOffset
							Dim hEnd As Integer = highlight.endOffset
							If hStart > p1 OrElse hEnd < p0 Then Continue For
							If Not sun.swing.SwingUtilities2.useSelectedTextColor(highlight, tc) Then Continue For
							If hStart <= p0 AndAlso hEnd >= p1 Then
								' the whole view is selected
								paintTextUsingColor(g, a, selFG, p0, p1)
								paintedText = True
								Exit For
							End If
							' the array is lazily created only when the view
							' is partially selected
							If Not initialized Then
								initSelections(p0, p1)
								initialized = True
							End If
							hStart = Math.Max(p0, hStart)
							hEnd = Math.Min(p1, hEnd)
							paintTextUsingColor(g, a, selFG, hStart, hEnd)
							' the array represents view positions [0, p1-p0+1]
							' later will iterate this array and sum its
							' elements. Positions with sum == 0 are not selected.
							selections(hStart-p0) += 1
							selections(hEnd-p0) -= 1

							viewSelectionCount += 1
						Next i

						If (Not paintedText) AndAlso viewSelectionCount > 0 Then
							' the view is partially selected
							Dim curPos As Integer = -1
							Dim startPos As Integer = 0
							Dim viewLen As Integer = p1 - p0
							Dim tempVar As Boolean = curPos < viewLen
							curPos += 1
							Do While tempVar
								' searching for the next selection start
								Do While curPos < viewLen AndAlso selections(curPos) = 0
									curPos += 1
								Loop
								If startPos <> curPos Then paintTextUsingColor(g, a, fg, p0 + startPos, p0 + curPos)
								Dim checkSum As Integer = 0
								' searching for next start position of unselected text
								checkSum += selections(curPos)
								Do While curPos < viewLen AndAlso checkSum <> 0
									curPos += 1
									checkSum += selections(curPos)
								Loop
								startPos = curPos
								tempVar = curPos < viewLen
								curPos += 1
							Loop
							paintedText = True
						End If
					End If
				End If
			End If
			If Not paintedText Then paintTextUsingColor(g, a, fg, p0, p1)
		End Sub

		''' <summary>
		''' Paints the specified region of text in the specified color.
		''' </summary>
		Friend Sub paintTextUsingColor(ByVal g As Graphics, ByVal a As Shape, ByVal c As Color, ByVal p0 As Integer, ByVal p1 As Integer)
			' render the glyphs
			g.color = c
			painter.paint(Me, g, a, p0, p1)

			' render underline or strikethrough if set.
			Dim ___underline As Boolean = underline
			Dim strike As Boolean = strikeThrough
			If ___underline OrElse strike Then
				' calculate x coordinates
				Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
				Dim ___parent As View = parent
				If (___parent IsNot Nothing) AndAlso (___parent.endOffset = p1) Then
					' strip whitespace on end
					Dim s As Segment = getText(p0, p1)
					Do While Char.IsWhiteSpace(s.last())
						p1 -= 1
						s.count -= 1
					Loop
					SegmentCache.releaseSharedSegment(s)
				End If
				Dim x0 As Integer = alloc.x
				Dim p As Integer = startOffset
				If p <> p0 Then x0 += CInt(Fix(painter.getSpan(Me, p, p0, tabExpander, x0)))
				Dim x1 As Integer = x0 + CInt(Fix(painter.getSpan(Me, p0, p1, tabExpander, x0)))

				' calculate y coordinate
				Dim y As Integer = alloc.y + CInt(Fix(painter.getHeight(Me) - painter.getDescent(Me)))
				If ___underline Then
					Dim yTmp As Integer = y + 1
					g.drawLine(x0, yTmp, x1, yTmp)
				End If
				If strike Then
					' move y coordinate above baseline
					Dim yTmp As Integer = y - CInt(Fix(painter.getAscent(Me) * 0.3f))
					g.drawLine(x0, yTmp, x1, yTmp)
				End If

			End If
		End Sub

		''' <summary>
		''' Determines the minimum span for this view along an axis.
		''' 
		''' <p>This implementation returns the longest non-breakable area within
		''' the view as a minimum span for {@code View.X_AXIS}.</p>
		''' </summary>
		''' <param name="axis">  may be either {@code View.X_AXIS} or {@code View.Y_AXIS} </param>
		''' <returns>      the minimum span the view can be rendered into </returns>
		''' <exception cref="IllegalArgumentException"> if the {@code axis} parameter is invalid </exception>
		''' <seealso cref=         javax.swing.text.View#getMinimumSpan </seealso>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			Select Case axis
				Case View.X_AXIS
					If minimumSpan < 0 Then
						minimumSpan = 0
						Dim p0 As Integer = startOffset
						Dim p1 As Integer = endOffset
						Do While p1 > p0
							Dim ___breakSpot As Integer = getBreakSpot(p0, p1)
							If ___breakSpot = java.text.BreakIterator.DONE Then ___breakSpot = p0
							minimumSpan = Math.Max(minimumSpan, getPartialSpan(___breakSpot, p1))
							' Note: getBreakSpot returns the *last* breakspot
							p1 = ___breakSpot - 1
						Loop
					End If
					Return minimumSpan
				Case View.Y_AXIS
					Return MyBase.getMinimumSpan(axis)
				Case Else
					Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			If impliedCR Then Return 0
			checkPainter()
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			Select Case axis
			Case View.X_AXIS
				If skipWidth Then Return 0
				Return painter.getSpan(Me, p0, p1, expander, Me.x)
			Case View.Y_AXIS
				Dim h As Single = painter.getHeight(Me)
				If superscript Then h += h/3
				Return h
			Case Else
				Throw New System.ArgumentException("Invalid axis: " & axis)
			End Select
		End Function

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  For the label, the alignment is along the font
		''' baseline for the y axis, and the superclasses alignment
		''' along the x axis.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns> the desired alignment.  This should be a value
		'''   between 0.0 and 1.0 inclusive, where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin.  An alignment of 0.5 would be the
		'''   center of the view. </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			checkPainter()
			If axis = View.Y_AXIS Then
				Dim sup As Boolean = superscript
				Dim [sub] As Boolean = subscript
				Dim h As Single = painter.getHeight(Me)
				Dim d As Single = painter.getDescent(Me)
				Dim a As Single = painter.getAscent(Me)
				Dim align As Single
				If sup Then
					align = 1.0f
				ElseIf [sub] Then
					align = If(h > 0, (h - (d + (a / 2))) / h, 0)
				Else
					align = If(h > 0, (h - d) / h, 0)
				End If
				Return align
			End If
			Return MyBase.getAlignment(axis)
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
		''' <param name="a">   the allocated region to render into </param>
		''' <param name="b">   either <code>Position.Bias.Forward</code>
		'''                or <code>Position.Bias.Backward</code> </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not represent a
		'''   valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			checkPainter()
			Return painter.modelToView(Me, pos, b, a)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <param name="y"> the Y coordinate &gt;= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="biasReturn"> either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> is returned as the
		'''  zero-th element of this array </param>
		''' <returns> the location within the model that best represents the
		'''  given point of view &gt;= 0 </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer
			checkPainter()
			Return painter.viewToModel(Me, x, y, a, biasReturn)
		End Function

		''' <summary>
		''' Determines how attractive a break opportunity in
		''' this view is.  This can be used for determining which
		''' view is the most attractive to call <code>breakView</code>
		''' on in the process of formatting.  The
		''' higher the weight, the more attractive the break.  A
		''' value equal to or lower than <code>View.BadBreakWeight</code>
		''' should not be considered for a break.  A value greater
		''' than or equal to <code>View.ForcedBreakWeight</code> should
		''' be broken.
		''' <p>
		''' This is implemented to forward to the superclass for
		''' the Y_AXIS.  Along the X_AXIS the following values
		''' may be returned.
		''' <dl>
		''' <dt><b>View.ExcellentBreakWeight</b>
		''' <dd>if there is whitespace proceeding the desired break
		'''   location.
		''' <dt><b>View.BadBreakWeight</b>
		''' <dd>if the desired break location results in a break
		'''   location of the starting offset.
		''' <dt><b>View.GoodBreakWeight</b>
		''' <dd>if the other conditions don't occur.
		''' </dl>
		''' This will normally result in the behavior of breaking
		''' on a whitespace location if one can be found, otherwise
		''' breaking between characters.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <param name="pos"> the potential location of the start of the
		'''   broken view &gt;= 0.  This may be useful for calculating tab
		'''   positions. </param>
		''' <param name="len"> specifies the relative length from <em>pos</em>
		'''   where a potential break is desired &gt;= 0. </param>
		''' <returns> the weight, which should be a value between
		'''   View.ForcedBreakWeight and View.BadBreakWeight. </returns>
		''' <seealso cref= LabelView </seealso>
		''' <seealso cref= ParagraphView </seealso>
		''' <seealso cref= View#BadBreakWeight </seealso>
		''' <seealso cref= View#GoodBreakWeight </seealso>
		''' <seealso cref= View#ExcellentBreakWeight </seealso>
		''' <seealso cref= View#ForcedBreakWeight </seealso>
		Public Overrides Function getBreakWeight(ByVal axis As Integer, ByVal pos As Single, ByVal len As Single) As Integer
			If axis = View.X_AXIS Then
				checkPainter()
				Dim p0 As Integer = startOffset
				Dim p1 As Integer = painter.getBoundedPosition(Me, p0, pos, len)
				Return If(p1 = p0, View.BadBreakWeight, If(getBreakSpot(p0, p1) <> java.text.BreakIterator.DONE, View.ExcellentBreakWeight, View.GoodBreakWeight))
			End If
			Return MyBase.getBreakWeight(axis, pos, len)
		End Function

		''' <summary>
		''' Breaks this view on the given axis at the given length.
		''' This is implemented to attempt to break on a whitespace
		''' location, and returns a fragment with the whitespace at
		''' the end.  If a whitespace location can't be found, the
		''' nearest character is used.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <param name="p0"> the location in the model where the
		'''  fragment should start it's representation &gt;= 0. </param>
		''' <param name="pos"> the position along the axis that the
		'''  broken view would occupy &gt;= 0.  This may be useful for
		'''  things like tab calculations. </param>
		''' <param name="len"> specifies the distance along the axis
		'''  where a potential break is desired &gt;= 0. </param>
		''' <returns> the fragment of the view that represents the
		'''  given span, if the view can be broken.  If the view
		'''  doesn't support breaking behavior, the view itself is
		'''  returned. </returns>
		''' <seealso cref= View#breakView </seealso>
		Public Overrides Function breakView(ByVal axis As Integer, ByVal p0 As Integer, ByVal pos As Single, ByVal len As Single) As View
			If axis = View.X_AXIS Then
				checkPainter()
				Dim p1 As Integer = painter.getBoundedPosition(Me, p0, pos, len)
				Dim ___breakSpot As Integer = getBreakSpot(p0, p1)

				If ___breakSpot <> -1 Then p1 = ___breakSpot
				' else, no break in the region, return a fragment of the
				' bounded region.
				If p0 = startOffset AndAlso p1 = endOffset Then Return Me
				Dim v As GlyphView = CType(createFragment(p0, p1), GlyphView)
				v.x = CInt(Fix(pos))
				Return v
			End If
			Return Me
		End Function

		''' <summary>
		''' Returns a location to break at in the passed in region, or
		''' BreakIterator.DONE if there isn't a good location to break at
		''' in the specified region.
		''' </summary>
		Private Function getBreakSpot(ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			If breakSpots Is Nothing Then
				' Re-calculate breakpoints for the whole view
				Dim start As Integer = startOffset
				Dim [end] As Integer = endOffset
				Dim bs As Integer() = New Integer([end] + 1 - start - 1){}
				Dim ix As Integer = 0

				' Breaker should work on the parent element because there may be
				' a valid breakpoint at the end edge of the view (space, etc.)
				Dim ___parent As Element = element.parentElement
				Dim pstart As Integer = (If(___parent Is Nothing, start, ___parent.startOffset))
				Dim pend As Integer = (If(___parent Is Nothing, [end], ___parent.endOffset))

				Dim s As Segment = getText(pstart, pend)
				s.first()
				Dim ___breaker As java.text.BreakIterator = breaker
				___breaker.text = s

				' Backward search should start from end+1 unless there's NO end+1
				Dim startFrom As Integer = [end] + (If(pend > [end], 1, 0))
				Do
					startFrom = ___breaker.preceding(s.offset + (startFrom - pstart)) + (pstart - s.offset)
					If startFrom > start Then
						' The break spot is within the view
						bs(ix) = startFrom
						ix += 1
					Else
						Exit Do
					End If
				Loop

				SegmentCache.releaseSharedSegment(s)
				breakSpots = New Integer(ix - 1){}
				Array.Copy(bs, 0, breakSpots, 0, ix)
			End If

			Dim ___breakSpot As Integer = java.text.BreakIterator.DONE
			For i As Integer = 0 To breakSpots.Length - 1
				Dim bsp As Integer = breakSpots(i)
				If bsp <= p1 Then
					If bsp > p0 Then ___breakSpot = bsp
					Exit For
				End If
			Next i
			Return ___breakSpot
		End Function

		''' <summary>
		''' Return break iterator appropriate for the current document.
		''' 
		''' For non-i18n documents a fast whitespace-based break iterator is used.
		''' </summary>
		Private Property breaker As java.text.BreakIterator
			Get
				Dim doc As Document = document
				If (doc IsNot Nothing) AndAlso Boolean.TRUE.Equals(doc.getProperty(AbstractDocument.MultiByteProperty)) Then
					Dim c As Container = container
					Dim locale As java.util.Locale = (If(c Is Nothing, java.util.Locale.default, c.locale))
					Return java.text.BreakIterator.getLineInstance(locale)
				Else
					Return New WhitespaceBasedBreakIterator
				End If
			End Get
		End Property

		''' <summary>
		''' Creates a view that represents a portion of the element.
		''' This is potentially useful during formatting operations
		''' for taking measurements of fragments of the view.  If
		''' the view doesn't support fragmenting (the default), it
		''' should return itself.
		''' <p>
		''' This view does support fragmenting.  It is implemented
		''' to return a nested class that shares state in this view
		''' representing only a portion of the view.
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
		Public Overrides Function createFragment(ByVal p0 As Integer, ByVal p1 As Integer) As View
			checkPainter()
			Dim elem As Element = element
			Dim v As GlyphView = CType(clone(), GlyphView)
			v.offset = p0 - elem.startOffset
			v.length = p1 - p0
			v.painter = painter.getPainter(v, p0, p1)
			v.justificationInfo = Nothing
			Return v
		End Function

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be
		''' visible, they might not be in the same order found in the model, or
		''' they just might not allow access to some of the locations in the
		''' model.
		''' This method enables specifying a position to convert
		''' within the range of &gt;=0.  If the value is -1, a position
		''' will be calculated automatically.  If the value &lt; -1,
		''' the {@code BadLocationException} will be thrown.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard.
		'''  This may be SwingConstants.WEST, SwingConstants.EAST,
		'''  SwingConstants.NORTH, or SwingConstants.SOUTH. </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position. </returns>
		''' <exception cref="BadLocationException"> the given position is not a valid
		'''                                 position within the document </exception>
		''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
		Public Overrides Function getNextVisualPositionFrom(ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer

			If pos < -1 Then Throw New BadLocationException("invalid position", pos)
			Return painter.getNextVisualPositionFrom(Me, pos, b, a, direction, biasRet)
		End Function

		''' <summary>
		''' Gives notification that something was inserted into
		''' the document in a location that this view is responsible for.
		''' This is implemented to call preferenceChanged along the
		''' axis the glyphs are rendered.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#insertUpdate </seealso>
		Public Overrides Sub insertUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			justificationInfo = Nothing
			breakSpots = Nothing
			minimumSpan = -1
			syncCR()
			preferenceChanged(Nothing, True, False)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that this view is responsible for.
		''' This is implemented to call preferenceChanged along the
		''' axis the glyphs are rendered.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overrides Sub removeUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			justificationInfo = Nothing
			breakSpots = Nothing
			minimumSpan = -1
			syncCR()
			preferenceChanged(Nothing, True, False)
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' This is implemented to call preferenceChanged along both the
		''' horizontal and vertical axis.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			minimumSpan = -1
			syncCR()
			preferenceChanged(Nothing, True, True)
		End Sub

		' checks if the paragraph is empty and updates impliedCR flag
		' accordingly
		Private Sub syncCR()
			If impliedCR Then
				Dim ___parent As Element = element.parentElement
				impliedCR = (___parent IsNot Nothing AndAlso ___parent.elementCount > 1)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc} </summary>
		Friend Overrides Sub updateAfterChange()
			' Drop the break spots. They will be re-calculated during
			' layout. It is necessary for proper line break calculation.
			breakSpots = Nothing
		End Sub

		''' <summary>
		''' Class to hold data needed to justify this GlyphView in a PargraphView.Row
		''' </summary>
		Friend Class JustificationInfo
			'justifiable content start
			Friend ReadOnly start As Integer
			'justifiable content end
			Friend ReadOnly [end] As Integer
			Friend ReadOnly leadingSpaces As Integer
			Friend ReadOnly contentSpaces As Integer
			Friend ReadOnly trailingSpaces As Integer
			Friend ReadOnly hasTab As Boolean
			Friend ReadOnly spaceMap As BitArray
			Friend Sub New(ByVal start As Integer, ByVal [end] As Integer, ByVal leadingSpaces As Integer, ByVal contentSpaces As Integer, ByVal trailingSpaces As Integer, ByVal hasTab As Boolean, ByVal spaceMap As BitArray)
				Me.start = start
				Me.end = [end]
				Me.leadingSpaces = leadingSpaces
				Me.contentSpaces = contentSpaces
				Me.trailingSpaces = trailingSpaces
				Me.hasTab = hasTab
				Me.spaceMap = spaceMap
			End Sub
		End Class



		Friend Overridable Function getJustificationInfo(ByVal rowStartOffset As Integer) As JustificationInfo
			If justificationInfo IsNot Nothing Then Return justificationInfo
			'states for the parsing
			Const TRAILING As Integer = 0
			Const CONTENT As Integer = 1
			Const SPACES As Integer = 2
			Dim ___startOffset As Integer = startOffset
			Dim ___endOffset As Integer = endOffset
			Dim segment As Segment = getText(___startOffset, ___endOffset)
			Dim txtOffset As Integer = segment.offset
			Dim txtEnd As Integer = segment.offset + segment.count - 1
			Dim startContentPosition As Integer = txtEnd + 1
			Dim endContentPosition As Integer = txtOffset - 1
			Dim lastTabPosition As Integer = txtOffset - 1
			Dim trailingSpaces As Integer = 0
			Dim contentSpaces As Integer = 0
			Dim leadingSpaces As Integer = 0
			Dim hasTab As Boolean = False
			Dim spaceMap As New BitArray(___endOffset - ___startOffset + 1)

			'we parse conent to the right of the rightmost TAB only.
			'we are looking for the trailing and leading spaces.
			'position after the leading spaces (startContentPosition)
			'position before the trailing spaces (endContentPosition)
			Dim i As Integer = txtEnd
			Dim state As Integer = TRAILING
			Do While i >= txtOffset
				If " "c = segment.array(i) Then
					spaceMap.Set(i - txtOffset, True)
					If state = TRAILING Then
						trailingSpaces += 1
					ElseIf state = CONTENT Then
						state = SPACES
						leadingSpaces = 1
					ElseIf state = SPACES Then
						leadingSpaces += 1
					End If
				ElseIf ControlChars.Tab = segment.array(i) Then
					hasTab = True
					Exit Do
				Else
					If state = TRAILING Then
						If ControlChars.Lf <> segment.array(i) AndAlso ControlChars.Cr <> segment.array(i) Then
							state = CONTENT
							endContentPosition = i
						End If
					ElseIf state = CONTENT Then
						'do nothing
					ElseIf state = SPACES Then
						contentSpaces += leadingSpaces
						leadingSpaces = 0
					End If
					startContentPosition = i
				End If
				i -= 1
			Loop

			SegmentCache.releaseSharedSegment(segment)

			Dim startJustifiableContent As Integer = -1
			If startContentPosition < txtEnd Then startJustifiableContent = startContentPosition - txtOffset
			Dim endJustifiableContent As Integer = -1
			If endContentPosition > txtOffset Then endJustifiableContent = endContentPosition - txtOffset
			justificationInfo = New JustificationInfo(startJustifiableContent, endJustifiableContent, leadingSpaces, contentSpaces, trailingSpaces, hasTab, spaceMap)
			Return justificationInfo
		End Function

		' --- variables ------------------------------------------------

		''' <summary>
		''' Used by paint() to store highlighted view positions
		''' </summary>
		Private selections As SByte() = Nothing

		Friend offset As Integer
		Friend length As Integer
		' if it is an implied newline character
		Friend impliedCR As Boolean
		Friend skipWidth As Boolean

		''' <summary>
		''' how to expand tabs
		''' </summary>
		Friend expander As TabExpander

		''' <summary>
		''' Cached minimum x-span value </summary>
		Private minimumSpan As Single = -1

		''' <summary>
		''' Cached breakpoints within the view </summary>
		Private breakSpots As Integer() = Nothing

		''' <summary>
		''' location for determining tab expansion against.
		''' </summary>
		Friend x As Integer

		''' <summary>
		''' Glyph rendering functionality.
		''' </summary>
		Friend painter As GlyphPainter

		''' <summary>
		''' The prototype painter used by default.
		''' </summary>
		Friend Shared defaultPainter As GlyphPainter

		Private justificationInfo As JustificationInfo = Nothing

		''' <summary>
		''' A class to perform rendering of the glyphs.
		''' This can be implemented to be stateless, or
		''' to hold some information as a cache to
		''' facilitate faster rendering and model/view
		''' translation.  At a minimum, the GlyphPainter
		''' allows a View implementation to perform its
		''' duties independant of a particular version
		''' of JVM and selection of capabilities (i.e.
		''' shaping for i18n, etc).
		''' 
		''' @since 1.3
		''' </summary>
		Public MustInherit Class GlyphPainter

			''' <summary>
			''' Determine the span the glyphs given a start location
			''' (for tab expansion).
			''' </summary>
			Public MustOverride Function getSpan(ByVal v As GlyphView, ByVal p0 As Integer, ByVal p1 As Integer, ByVal e As TabExpander, ByVal x As Single) As Single

			Public MustOverride Function getHeight(ByVal v As GlyphView) As Single

			Public MustOverride Function getAscent(ByVal v As GlyphView) As Single

			Public MustOverride Function getDescent(ByVal v As GlyphView) As Single

			''' <summary>
			''' Paint the glyphs representing the given range.
			''' </summary>
			Public MustOverride Sub paint(ByVal v As GlyphView, ByVal g As Graphics, ByVal a As Shape, ByVal p0 As Integer, ByVal p1 As Integer)

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' This is shared by the broken views.
			''' </summary>
			''' <param name="v">     the <code>GlyphView</code> containing the
			'''              destination coordinate space </param>
			''' <param name="pos">   the position to convert </param>
			''' <param name="bias">  either <code>Position.Bias.Forward</code>
			'''                  or <code>Position.Bias.Backward</code> </param>
			''' <param name="a">     Bounds of the View </param>
			''' <returns>      the bounding box of the given position </returns>
			''' <exception cref="BadLocationException">  if the given position does not represent a
			'''   valid location in the associated document </exception>
			''' <seealso cref= View#modelToView </seealso>
			Public MustOverride Function modelToView(ByVal v As GlyphView, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal a As Shape) As Shape

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.
			''' </summary>
			''' <param name="v">          the <code>GlyphView</code> to provide a mapping for </param>
			''' <param name="x">          the X coordinate </param>
			''' <param name="y">          the Y coordinate </param>
			''' <param name="a">          the allocated region to render into </param>
			''' <param name="biasReturn"> either <code>Position.Bias.Forward</code>
			'''                   or <code>Position.Bias.Backward</code>
			'''                   is returned as the zero-th element of this array </param>
			''' <returns> the location within the model that best represents the
			'''         given point of view </returns>
			''' <seealso cref= View#viewToModel </seealso>
			Public MustOverride Function viewToModel(ByVal v As GlyphView, ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer

			''' <summary>
			''' Determines the model location that represents the
			''' maximum advance that fits within the given span.
			''' This could be used to break the given view.  The result
			''' should be a location just shy of the given advance.  This
			''' differs from viewToModel which returns the closest
			''' position which might be proud of the maximum advance.
			''' </summary>
			''' <param name="v"> the view to find the model location to break at. </param>
			''' <param name="p0"> the location in the model where the
			'''  fragment should start it's representation &gt;= 0. </param>
			''' <param name="x">  the graphic location along the axis that the
			'''  broken view would occupy &gt;= 0.  This may be useful for
			'''  things like tab calculations. </param>
			''' <param name="len"> specifies the distance into the view
			'''  where a potential break is desired &gt;= 0. </param>
			''' <returns> the maximum model location possible for a break. </returns>
			''' <seealso cref= View#breakView </seealso>
			Public MustOverride Function getBoundedPosition(ByVal v As GlyphView, ByVal p0 As Integer, ByVal x As Single, ByVal len As Single) As Integer

			''' <summary>
			''' Create a painter to use for the given GlyphView.  If
			''' the painter carries state it can create another painter
			''' to represent a new GlyphView that is being created.  If
			''' the painter doesn't hold any significant state, it can
			''' return itself.  The default behavior is to return itself. </summary>
			''' <param name="v">  the <code>GlyphView</code> to provide a painter for </param>
			''' <param name="p0"> the starting document offset &gt;= 0 </param>
			''' <param name="p1"> the ending document offset &gt;= p0 </param>
			Public Overridable Function getPainter(ByVal v As GlyphView, ByVal p0 As Integer, ByVal p1 As Integer) As GlyphPainter
				Return Me
			End Function

			''' <summary>
			''' Provides a way to determine the next visually represented model
			''' location that one might place a caret.  Some views may not be
			''' visible, they might not be in the same order found in the model, or
			''' they just might not allow access to some of the locations in the
			''' model.
			''' </summary>
			''' <param name="v"> the view to use </param>
			''' <param name="pos"> the position to convert &gt;= 0 </param>
			''' <param name="b">   either <code>Position.Bias.Forward</code>
			'''                or <code>Position.Bias.Backward</code> </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <param name="direction"> the direction from the current position that can
			'''  be thought of as the arrow keys typically found on a keyboard.
			'''  This may be SwingConstants.WEST, SwingConstants.EAST,
			'''  SwingConstants.NORTH, or SwingConstants.SOUTH. </param>
			''' <param name="biasRet">  either <code>Position.Bias.Forward</code>
			'''                 or <code>Position.Bias.Backward</code>
			'''                 is returned as the zero-th element of this array </param>
			''' <returns> the location within the model that best represents the next
			'''  location visual position. </returns>
			''' <exception cref="BadLocationException"> </exception>
			''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
			Public Overridable Function getNextVisualPositionFrom(ByVal v As GlyphView, ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer

				Dim startOffset As Integer = v.startOffset
				Dim endOffset As Integer = v.endOffset
				Dim text As Segment

				Select Case direction
				Case View.NORTH, View.SOUTH
					If pos <> -1 Then Return -1
					Dim container As Container = v.container

					If TypeOf container Is JTextComponent Then
						Dim c As Caret = CType(container, JTextComponent).caret
						Dim magicPoint As Point
						magicPoint = If(c IsNot Nothing, c.magicCaretPosition, Nothing)

						If magicPoint Is Nothing Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
						Dim value As Integer = v.viewToModel(magicPoint.x, 0f, a, biasRet)
						Return value
					End If
				Case View.EAST
					If startOffset = v.document.length Then
						If pos = -1 Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
						' End case for bidi text where newline is at beginning
						' of line.
						Return -1
					End If
					If pos = -1 Then
						biasRet(0) = Position.Bias.Forward
						Return startOffset
					End If
					If pos = endOffset Then Return -1
					pos += 1
					If pos = endOffset Then
						' Assumed not used in bidi text, GlyphPainter2 will
						' override as necessary, therefore return -1.
						Return -1
					Else
						biasRet(0) = Position.Bias.Forward
					End If
					Return pos
				Case View.WEST
					If startOffset = v.document.length Then
						If pos = -1 Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
						' End case for bidi text where newline is at beginning
						' of line.
						Return -1
					End If
					If pos = -1 Then
						' Assumed not used in bidi text, GlyphPainter2 will
						' override as necessary, therefore return -1.
						biasRet(0) = Position.Bias.Forward
						Return endOffset - 1
					End If
					If pos = startOffset Then Return -1
					biasRet(0) = Position.Bias.Forward
					Return (pos - 1)
				Case Else
					Throw New System.ArgumentException("Bad direction: " & direction)
				End Select
				Return pos

			End Function
		End Class
	End Class

End Namespace