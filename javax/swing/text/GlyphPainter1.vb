Imports Microsoft.VisualBasic

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A class to perform rendering of the glyphs.
	''' This can be implemented to be stateless, or
	''' to hold some information as a cache to
	''' facilitate faster rendering and model/view
	''' translation.  At a minimum, the GlyphPainter
	''' allows a View implementation to perform its
	''' duties independent of a particular version
	''' of JVM and selection of capabilities (i.e.
	''' shaping for i18n, etc).
	''' <p>
	''' This implementation is intended for operation
	''' under the JDK1.1 API of the Java Platform.
	''' Since the JDK is backward compatible with
	''' JDK1.1 API, this class will also function on
	''' Java 2.  The JDK introduces improved
	''' API for rendering text however, so the GlyphPainter2
	''' is recommended for the DK.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= GlyphView </seealso>
	Friend Class GlyphPainter1
		Inherits GlyphView.GlyphPainter

		''' <summary>
		''' Determine the span the glyphs given a start location
		''' (for tab expansion).
		''' </summary>
		Public Overridable Function getSpan(ByVal v As GlyphView, ByVal p0 As Integer, ByVal p1 As Integer, ByVal e As TabExpander, ByVal x As Single) As Single
			sync(v)
			Dim text As Segment = v.getText(p0, p1)
			Dim ___justificationData As Integer() = getJustificationData(v)
			Dim width As Integer = Utilities.getTabbedTextWidth(v, text, metrics, CInt(Fix(x)), e, p0, ___justificationData)
			SegmentCache.releaseSharedSegment(text)
			Return width
		End Function

		Public Overridable Function getHeight(ByVal v As GlyphView) As Single
			sync(v)
			Return metrics.height
		End Function

		''' <summary>
		''' Fetches the ascent above the baseline for the glyphs
		''' corresponding to the given range in the model.
		''' </summary>
		Public Overridable Function getAscent(ByVal v As GlyphView) As Single
			sync(v)
			Return metrics.ascent
		End Function

		''' <summary>
		''' Fetches the descent below the baseline for the glyphs
		''' corresponding to the given range in the model.
		''' </summary>
		Public Overridable Function getDescent(ByVal v As GlyphView) As Single
			sync(v)
			Return metrics.descent
		End Function

		''' <summary>
		''' Paints the glyphs representing the given range.
		''' </summary>
		Public Overridable Sub paint(ByVal v As GlyphView, ByVal g As Graphics, ByVal a As Shape, ByVal p0 As Integer, ByVal p1 As Integer)
			sync(v)
			Dim text As Segment
			Dim expander As TabExpander = v.tabExpander
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)

			' determine the x coordinate to render the glyphs
			Dim x As Integer = alloc.x
			Dim p As Integer = v.startOffset
			Dim ___justificationData As Integer() = getJustificationData(v)
			If p <> p0 Then
				text = v.getText(p, p0)
				Dim width As Integer = Utilities.getTabbedTextWidth(v, text, metrics, x, expander, p, ___justificationData)
				x += width
				SegmentCache.releaseSharedSegment(text)
			End If

			' determine the y coordinate to render the glyphs
			Dim y As Integer = alloc.y + metrics.height - metrics.descent

			' render the glyphs
			text = v.getText(p0, p1)
			g.font = metrics.font

			Utilities.drawTabbedText(v, text, x, y, g, expander,p0, ___justificationData)
			SegmentCache.releaseSharedSegment(text)
		End Sub

		Public Overridable Function modelToView(ByVal v As GlyphView, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal a As Shape) As Shape

			sync(v)
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			Dim p0 As Integer = v.startOffset
			Dim p1 As Integer = v.endOffset
			Dim expander As TabExpander = v.tabExpander
			Dim text As Segment

			If pos = p1 Then Return New Rectangle(alloc.x + alloc.width, alloc.y, 0, metrics.height)
			If (pos >= p0) AndAlso (pos <= p1) Then
				' determine range to the left of the position
				text = v.getText(p0, pos)
				Dim ___justificationData As Integer() = getJustificationData(v)
				Dim width As Integer = Utilities.getTabbedTextWidth(v, text, metrics, alloc.x, expander, p0, ___justificationData)
				SegmentCache.releaseSharedSegment(text)
				Return New Rectangle(alloc.x + width, alloc.y, 0, metrics.height)
			End If
			Throw New BadLocationException("modelToView - can't convert", p1)
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="v"> the view containing the view coordinates </param>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="biasReturn"> always returns <code>Position.Bias.Forward</code>
		'''   as the zero-th element of this array </param>
		''' <returns> the location within the model that best represents the
		'''  given point in the view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overridable Function viewToModel(ByVal v As GlyphView, ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer

			sync(v)
			Dim alloc As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			Dim p0 As Integer = v.startOffset
			Dim p1 As Integer = v.endOffset
			Dim expander As TabExpander = v.tabExpander
			Dim text As Segment = v.getText(p0, p1)
			Dim ___justificationData As Integer() = getJustificationData(v)
			Dim offs As Integer = Utilities.getTabbedTextOffset(v, text, metrics, alloc.x, CInt(Fix(x)), expander, p0, ___justificationData)
			SegmentCache.releaseSharedSegment(text)
			Dim retValue As Integer = p0 + offs
			If retValue = p1 Then retValue -= 1
			biasReturn(0) = Position.Bias.Forward
			Return retValue
		End Function

		''' <summary>
		''' Determines the best location (in the model) to break
		''' the given view.
		''' This method attempts to break on a whitespace
		''' location.  If a whitespace location can't be found, the
		''' nearest character location is returned.
		''' </summary>
		''' <param name="v"> the view </param>
		''' <param name="p0"> the location in the model where the
		'''  fragment should start its representation >= 0 </param>
		''' <param name="pos"> the graphic location along the axis that the
		'''  broken view would occupy >= 0; this may be useful for
		'''  things like tab calculations </param>
		''' <param name="len"> specifies the distance into the view
		'''  where a potential break is desired >= 0 </param>
		''' <returns> the model location desired for a break </returns>
		''' <seealso cref= View#breakView </seealso>
		Public Overridable Function getBoundedPosition(ByVal v As GlyphView, ByVal p0 As Integer, ByVal x As Single, ByVal len As Single) As Integer
			sync(v)
			Dim expander As TabExpander = v.tabExpander
			Dim s As Segment = v.getText(p0, v.endOffset)
			Dim ___justificationData As Integer() = getJustificationData(v)
			Dim index As Integer = Utilities.getTabbedTextOffset(v, s, metrics, CInt(Fix(x)), CInt(Fix(x+len)), expander, p0, False, ___justificationData)
			SegmentCache.releaseSharedSegment(s)
			Dim p1 As Integer = p0 + index
			Return p1
		End Function

		Friend Overridable Sub sync(ByVal v As GlyphView)
			Dim f As Font = v.font
			If (metrics Is Nothing) OrElse ((Not f.Equals(metrics.font))) Then
				' fetch a new FontMetrics
				Dim c As Container = v.container
				metrics = If(c IsNot Nothing, c.getFontMetrics(f), Toolkit.defaultToolkit.getFontMetrics(f))
			End If
		End Sub



		''' <returns> justificationData from the ParagraphRow this GlyphView
		''' is in or {@code null} if no justification is needed </returns>
		Private Function getJustificationData(ByVal v As GlyphView) As Integer()
			Dim parent As View = v.parent
			Dim ret As Integer() = Nothing
			If TypeOf parent Is ParagraphView.Row Then
				Dim row As ParagraphView.Row = (CType(parent, ParagraphView.Row))
				ret = row.justificationData
			End If
			Return ret
		End Function

		' --- variables ---------------------------------------------

		Friend metrics As FontMetrics
	End Class

End Namespace