Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' A flow strategy that uses java.awt.font.LineBreakMeasureer to
	''' produce java.awt.font.TextLayout for i18n capable rendering.
	''' If the child view being placed into the flow is of type
	''' GlyphView and can be rendered by TextLayout, a GlyphPainter
	''' that uses TextLayout is plugged into the GlyphView.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Friend Class TextLayoutStrategy
		Inherits FlowView.FlowStrategy

		''' <summary>
		''' Constructs a layout strategy for paragraphs based
		''' upon java.awt.font.LineBreakMeasurer.
		''' </summary>
		Public Sub New()
			text = New AttributedSegment
		End Sub

		' --- FlowStrategy methods --------------------------------------------

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
		Public Overridable Sub insertUpdate(ByVal fv As FlowView, ByVal e As javax.swing.event.DocumentEvent, ByVal alloc As Rectangle)
			sync(fv)
			MyBase.insertUpdate(fv, e, alloc)
		End Sub

		''' <summary>
		''' Gives notification that something was removed from the document
		''' in a location that the given flow view is responsible for.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="alloc"> the current allocation of the view inside of the insets. </param>
		''' <seealso cref= View#removeUpdate </seealso>
		Public Overridable Sub removeUpdate(ByVal fv As FlowView, ByVal e As javax.swing.event.DocumentEvent, ByVal alloc As Rectangle)
			sync(fv)
			MyBase.removeUpdate(fv, e, alloc)
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.
		''' </summary>
		''' <param name="changes"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children </param>
		''' <seealso cref= View#changedUpdate </seealso>
		Public Overridable Sub changedUpdate(ByVal fv As FlowView, ByVal e As javax.swing.event.DocumentEvent, ByVal alloc As Rectangle)
			sync(fv)
			MyBase.changedUpdate(fv, e, alloc)
		End Sub

		''' <summary>
		''' Does a a full layout on the given View.  This causes all of
		''' the rows (child views) to be rebuilt to match the given
		''' constraints for each row.  This is called by a FlowView.layout
		''' to update the child views in the flow.
		''' </summary>
		''' <param name="fv"> the view to reflow </param>
		Public Overridable Sub layout(ByVal fv As FlowView)
			MyBase.layout(fv)
		End Sub

		''' <summary>
		''' Creates a row of views that will fit within the
		''' layout span of the row.  This is implemented to execute the
		''' superclass functionality (which fills the row with child
		''' views or view fragments) and follow that with bidi reordering
		''' of the unidirectional view fragments.
		''' </summary>
		''' <param name="row"> the row to fill in with views.  This is assumed
		'''   to be empty on entry. </param>
		''' <param name="pos">  The current position in the children of
		'''   this views element from which to start. </param>
		''' <returns> the position to start the next row </returns>
		Protected Friend Overridable Function layoutRow(ByVal fv As FlowView, ByVal rowIndex As Integer, ByVal p0 As Integer) As Integer
			Dim p1 As Integer = MyBase.layoutRow(fv, rowIndex, p0)
			Dim row As View = fv.getView(rowIndex)
			Dim doc As Document = fv.document
			Dim i18nFlag As Object = doc.getProperty(AbstractDocument.I18NProperty)
			If (i18nFlag IsNot Nothing) AndAlso i18nFlag.Equals(Boolean.TRUE) Then
				Dim n As Integer = row.viewCount
				If n > 1 Then
					Dim d As AbstractDocument = CType(fv.document, AbstractDocument)
					Dim bidiRoot As Element = d.bidiRootElement
					Dim levels As SByte() = New SByte(n - 1){}
					Dim reorder As View() = New View(n - 1){}

					For i As Integer = 0 To n - 1
						Dim v As View = row.getView(i)
						Dim bidiIndex As Integer =bidiRoot.getElementIndex(v.startOffset)
						Dim bidiElem As Element = bidiRoot.getElement(bidiIndex)
						levels(i) = CByte(StyleConstants.getBidiLevel(bidiElem.attributes))
						reorder(i) = v
					Next i

					sun.font.BidiUtils.reorderVisually(levels, reorder)
					row.replace(0, n, reorder)
				End If
			End If
			Return p1
		End Function

		''' <summary>
		''' Adjusts the given row if possible to fit within the
		''' layout span.  Since all adjustments were already
		''' calculated by the LineBreakMeasurer, this is implemented
		''' to do nothing.
		''' </summary>
		''' <param name="r"> the row to adjust to the current layout
		'''  span. </param>
		''' <param name="desiredSpan"> the current layout span >= 0 </param>
		''' <param name="x"> the location r starts at. </param>
		Protected Friend Overridable Sub adjustRow(ByVal fv As FlowView, ByVal rowIndex As Integer, ByVal desiredSpan As Integer, ByVal x As Integer)
		End Sub

		''' <summary>
		''' Creates a unidirectional view that can be used to represent the
		''' current chunk.  This can be either an entire view from the
		''' logical view, or a fragment of the view.
		''' </summary>
		''' <param name="fv"> the view holding the flow </param>
		''' <param name="startOffset"> the start location for the view being created </param>
		''' <param name="spanLeft"> the about of span left to fill in the row </param>
		''' <param name="rowIndex"> the row the view will be placed into </param>
		Protected Friend Overridable Function createView(ByVal fv As FlowView, ByVal startOffset As Integer, ByVal spanLeft As Integer, ByVal rowIndex As Integer) As View
			' Get the child view that contains the given starting position
			Dim lv As View = getLogicalView(fv)
			Dim row As View = fv.getView(rowIndex)
			Dim requireNextWord As Boolean = If(viewBuffer.Count = 0, False, True)
			Dim childIndex As Integer = lv.getViewIndex(startOffset, Position.Bias.Forward)
			Dim v As View = lv.getView(childIndex)

			Dim endOffset As Integer = getLimitingOffset(v, startOffset, spanLeft, requireNextWord)
			If endOffset = startOffset Then Return Nothing

			Dim frag As View
			If (startOffset=v.startOffset) AndAlso (endOffset = v.endOffset) Then
				' return the entire view
				frag = v
			Else
				' return a unidirectional fragment.
				frag = v.createFragment(startOffset, endOffset)
			End If

			If (TypeOf frag Is GlyphView) AndAlso (measurer IsNot Nothing) Then
				' install a TextLayout based renderer if the view is responsible
				' for glyphs.  If the view represents a tab, the default
				' glyph painter is used (may want to handle tabs differently).
				Dim isTab As Boolean = False
				Dim p0 As Integer = frag.startOffset
				Dim p1 As Integer = frag.endOffset
				If (p1 - p0) = 1 Then
					' check for tab
					Dim s As Segment = CType(frag, GlyphView).getText(p0, p1)
					Dim ch As Char = s.first()
					If ch = ControlChars.Tab Then isTab = True
				End If
				Dim tl As TextLayout = If(isTab, Nothing, measurer.nextLayout(spanLeft, text.toIteratorIndex(endOffset), requireNextWord))
				If tl IsNot Nothing Then CType(frag, GlyphView).glyphPainter = New GlyphPainter2(tl)
			End If
			Return frag
		End Function

		''' <summary>
		''' Calculate the limiting offset for the next view fragment.
		''' At most this would be the entire view (i.e. the limiting
		''' offset would be the end offset in that case).  If the range
		''' contains a tab or a direction change, that will limit the
		''' offset to something less.  This value is then fed to the
		''' LineBreakMeasurer as a limit to consider in addition to the
		''' remaining span.
		''' </summary>
		''' <param name="v"> the logical view representing the starting offset. </param>
		''' <param name="startOffset"> the model location to start at. </param>
		Friend Overridable Function getLimitingOffset(ByVal v As View, ByVal startOffset As Integer, ByVal spanLeft As Integer, ByVal requireNextWord As Boolean) As Integer
			Dim endOffset As Integer = v.endOffset

			' check for direction change
			Dim doc As Document = v.document
			If TypeOf doc Is AbstractDocument Then
				Dim d As AbstractDocument = CType(doc, AbstractDocument)
				Dim bidiRoot As Element = d.bidiRootElement
				If bidiRoot.elementCount > 1 Then
					Dim bidiIndex As Integer = bidiRoot.getElementIndex(startOffset)
					Dim bidiElem As Element = bidiRoot.getElement(bidiIndex)
					endOffset = Math.Min(bidiElem.endOffset, endOffset)
				End If
			End If

			' check for tab
			If TypeOf v Is GlyphView Then
				Dim s As Segment = CType(v, GlyphView).getText(startOffset, endOffset)
				Dim ch As Char = s.first()
				If ch = ControlChars.Tab Then
					' if the first character is a tab, create a dedicated
					' view for just the tab
					endOffset = startOffset + 1
				Else
					ch = s.next()
					Do While ch <> Segment.DONE
						If ch = ControlChars.Tab Then
							' found a tab, don't include it in the text
							endOffset = startOffset + s.index - s.beginIndex
							Exit Do
						End If
						ch = s.next()
					Loop
				End If
			End If

			' determine limit from LineBreakMeasurer
			Dim limitIndex As Integer = text.toIteratorIndex(endOffset)
			If measurer IsNot Nothing Then
				Dim index As Integer = text.toIteratorIndex(startOffset)
				If measurer.position <> index Then measurer.position = index
				limitIndex = measurer.nextOffset(spanLeft, limitIndex, requireNextWord)
			End If
			Dim pos As Integer = text.toModelPosition(limitIndex)
			Return pos
		End Function

		''' <summary>
		''' Synchronize the strategy with its FlowView.  Allows the strategy
		''' to update its state to account for changes in that portion of the
		''' model represented by the FlowView.  Also allows the strategy
		''' to update the FlowView in response to these changes.
		''' </summary>
		Friend Overridable Sub sync(ByVal fv As FlowView)
			Dim lv As View = getLogicalView(fv)
			text.view = lv

			Dim container As Container = fv.container
			Dim frc As FontRenderContext = sun.swing.SwingUtilities2.0 getFontRenderContext(container)
			Dim iter As java.text.BreakIterator
			Dim c As Container = fv.container
			If c IsNot Nothing Then
				iter = java.text.BreakIterator.getLineInstance(c.locale)
			Else
				iter = java.text.BreakIterator.lineInstance
			End If

			Dim shaper As Object = Nothing
			If TypeOf c Is javax.swing.JComponent Then shaper = CType(c, javax.swing.JComponent).getClientProperty(TextAttribute.NUMERIC_SHAPING)
			text.shaper = shaper

			measurer = New LineBreakMeasurer(text, iter, frc)

			' If the children of the FlowView's logical view are GlyphViews, they
			' need to have their painters updated.
			Dim n As Integer = lv.viewCount
			For i As Integer = 0 To n - 1
				Dim child As View = lv.getView(i)
				If TypeOf child Is GlyphView Then
					Dim p0 As Integer = child.startOffset
					Dim p1 As Integer = child.endOffset
					measurer.position = text.toIteratorIndex(p0)
					Dim layout As TextLayout = measurer.nextLayout(Single.MaxValue, text.toIteratorIndex(p1), False)
					CType(child, GlyphView).glyphPainter = New GlyphPainter2(layout)
				End If
			Next i

			' Reset measurer.
			measurer.position = text.beginIndex

		End Sub

		' --- variables -------------------------------------------------------

		Private measurer As LineBreakMeasurer
		Private text As AttributedSegment

		''' <summary>
		''' Implementation of AttributedCharacterIterator that supports
		''' the GlyphView attributes for rendering the glyphs through a
		''' TextLayout.
		''' </summary>
		Friend Class AttributedSegment
			Inherits Segment
			Implements java.text.AttributedCharacterIterator

			Friend Sub New()
			End Sub

			Friend Overridable Property view As View
				Get
					Return v
				End Get
				Set(ByVal v As View)
					Me.v = v
					Dim doc As Document = v.document
					Dim p0 As Integer = v.startOffset
					Dim p1 As Integer = v.endOffset
					Try
						doc.getText(p0, p1 - p0, Me)
					Catch bl As BadLocationException
						Throw New System.ArgumentException("Invalid view")
					End Try
					first()
				End Set
			End Property


			''' <summary>
			''' Get a boundary position for the font.
			''' This is implemented to assume that two fonts are
			''' equal if their references are equal (i.e. that the
			''' font came from a cache).
			''' </summary>
			''' <returns> the location in model coordinates.  This is
			'''  not the same as the Segment coordinates. </returns>
			Friend Overridable Function getFontBoundary(ByVal childIndex As Integer, ByVal dir As Integer) As Integer
				Dim child As View = v.getView(childIndex)
				Dim f As Font = getFont(childIndex)
				childIndex += dir
				Do While (childIndex >= 0) AndAlso (childIndex < v.viewCount)
					Dim [next] As Font = getFont(childIndex)
					If [next] IsNot f Then Exit Do
					child = v.getView(childIndex)
					childIndex += dir
				Loop
				Return If(dir < 0, child.startOffset, child.endOffset)
			End Function

			''' <summary>
			''' Get the font at the given child index.
			''' </summary>
			Friend Overridable Function getFont(ByVal childIndex As Integer) As Font
				Dim child As View = v.getView(childIndex)
				If TypeOf child Is GlyphView Then Return CType(child, GlyphView).font
				Return Nothing
			End Function

			Friend Overridable Function toModelPosition(ByVal index As Integer) As Integer
				Return v.startOffset + (index - beginIndex)
			End Function

			Friend Overridable Function toIteratorIndex(ByVal pos As Integer) As Integer
				Return pos - v.startOffset + beginIndex
			End Function

			Private Property shaper As Object
				Set(ByVal shaper As Object)
					Me.shaper = shaper
				End Set
			End Property

			' --- AttributedCharacterIterator methods -------------------------

			''' <summary>
			''' Returns the index of the first character of the run
			''' with respect to all attributes containing the current character.
			''' </summary>
			Public Overridable Property runStart As Integer
				Get
					Dim pos As Integer = toModelPosition(index)
					Dim i As Integer = v.getViewIndex(pos, Position.Bias.Forward)
					Dim child As View = v.getView(i)
					Return toIteratorIndex(child.startOffset)
				End Get
			End Property

			''' <summary>
			''' Returns the index of the first character of the run
			''' with respect to the given attribute containing the current character.
			''' </summary>
			Public Overridable Function getRunStart(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Integer
				If TypeOf attribute Is TextAttribute Then
					Dim pos As Integer = toModelPosition(index)
					Dim i As Integer = v.getViewIndex(pos, Position.Bias.Forward)
					If attribute Is TextAttribute.FONT Then Return toIteratorIndex(getFontBoundary(i, -1))
				End If
				Return beginIndex
			End Function

			''' <summary>
			''' Returns the index of the first character of the run
			''' with respect to the given attributes containing the current character.
			''' </summary>
			Public Overridable Function getRunStart(Of T1 As Attribute)(ByVal attributes As [Set](Of T1)) As Integer
				Dim ___index As Integer = beginIndex
				Dim a As Object() = attributes.ToArray()
				For i As Integer = 0 To a.Length - 1
					Dim attr As TextAttribute = CType(a(i), TextAttribute)
					___index = Math.Max(getRunStart(attr), ___index)
				Next i
				Return Math.Min(index, ___index)
			End Function

			''' <summary>
			''' Returns the index of the first character following the run
			''' with respect to all attributes containing the current character.
			''' </summary>
			Public Overridable Property runLimit As Integer
				Get
					Dim pos As Integer = toModelPosition(index)
					Dim i As Integer = v.getViewIndex(pos, Position.Bias.Forward)
					Dim child As View = v.getView(i)
					Return toIteratorIndex(child.endOffset)
				End Get
			End Property

			''' <summary>
			''' Returns the index of the first character following the run
			''' with respect to the given attribute containing the current character.
			''' </summary>
			Public Overridable Function getRunLimit(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Integer
				If TypeOf attribute Is TextAttribute Then
					Dim pos As Integer = toModelPosition(index)
					Dim i As Integer = v.getViewIndex(pos, Position.Bias.Forward)
					If attribute Is TextAttribute.FONT Then Return toIteratorIndex(getFontBoundary(i, 1))
				End If
				Return endIndex
			End Function

			''' <summary>
			''' Returns the index of the first character following the run
			''' with respect to the given attributes containing the current character.
			''' </summary>
			Public Overridable Function getRunLimit(Of T1 As Attribute)(ByVal attributes As [Set](Of T1)) As Integer
				Dim ___index As Integer = endIndex
				Dim a As Object() = attributes.ToArray()
				For i As Integer = 0 To a.Length - 1
					Dim attr As TextAttribute = CType(a(i), TextAttribute)
					___index = Math.Min(getRunLimit(attr), ___index)
				Next i
				Return Math.Max(index, ___index)
			End Function

			''' <summary>
			''' Returns a map with the attributes defined on the current
			''' character.
			''' </summary>
			Public Overridable Property attributes As IDictionary(Of Attribute, Object)
				Get
					Dim ka As Object() = keys.ToArray()
					Dim h As New Dictionary(Of Attribute, Object)
					For i As Integer = 0 To ka.Length - 1
						Dim a As TextAttribute = CType(ka(i), TextAttribute)
						Dim value As Object = getAttribute(a)
						If value IsNot Nothing Then h(a) = value
					Next i
					Return h
				End Get
			End Property

			''' <summary>
			''' Returns the value of the named attribute for the current character.
			''' Returns null if the attribute is not defined. </summary>
			''' <param name="attribute"> the key of the attribute whose value is requested. </param>
			Public Overridable Function getAttribute(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Object
				Dim pos As Integer = toModelPosition(index)
				Dim childIndex As Integer = v.getViewIndex(pos, Position.Bias.Forward)
				If attribute Is TextAttribute.FONT Then
					Return getFont(childIndex)
				ElseIf attribute Is TextAttribute.RUN_DIRECTION Then
					Return v.document.getProperty(TextAttribute.RUN_DIRECTION)
				ElseIf attribute Is TextAttribute.NUMERIC_SHAPING Then
					Return shaper
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the keys of all attributes defined on the
			''' iterator's text range. The set is empty if no
			''' attributes are defined.
			''' </summary>
			Public Overridable Property allAttributeKeys As [Set](Of Attribute)
				Get
					Return keys
				End Get
			End Property

			Friend v As View

			Friend Shared keys As [Set](Of Attribute)

			Shared Sub New()
				keys = New HashSet(Of Attribute)
				keys.add(TextAttribute.FONT)
				keys.add(TextAttribute.RUN_DIRECTION)
				keys.add(TextAttribute.NUMERIC_SHAPING)
			End Sub

			Private shaper As Object = Nothing
		End Class

	End Class

End Namespace