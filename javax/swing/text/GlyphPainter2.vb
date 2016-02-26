Imports Microsoft.VisualBasic

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
	''' under the JDK.  It uses the
	''' java.awt.font.TextLayout class to do i18n capable
	''' rendering.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= GlyphView </seealso>
	Friend Class GlyphPainter2
		Inherits GlyphView.GlyphPainter

		Public Sub New(ByVal layout As TextLayout)
			Me.layout = layout
		End Sub

		''' <summary>
		''' Create a painter to use for the given GlyphView.
		''' </summary>
		Public Overridable Function getPainter(ByVal v As GlyphView, ByVal p0 As Integer, ByVal p1 As Integer) As GlyphView.GlyphPainter
			Return Nothing
		End Function

		''' <summary>
		''' Determine the span the glyphs given a start location
		''' (for tab expansion).  This implementation assumes it
		''' has no tabs (i.e. TextLayout doesn't deal with tab
		''' expansion).
		''' </summary>
		Public Overridable Function getSpan(ByVal v As GlyphView, ByVal p0 As Integer, ByVal p1 As Integer, ByVal e As TabExpander, ByVal x As Single) As Single

			If (p0 = v.startOffset) AndAlso (p1 = v.endOffset) Then Return layout.advance
			Dim p As Integer = v.startOffset
			Dim index0 As Integer = p0 - p
			Dim index1 As Integer = p1 - p

			Dim hit0 As TextHitInfo = TextHitInfo.afterOffset(index0)
			Dim hit1 As TextHitInfo = TextHitInfo.beforeOffset(index1)
			Dim locs As Single() = layout.getCaretInfo(hit0)
			Dim x0 As Single = locs(0)
			locs = layout.getCaretInfo(hit1)
			Dim x1 As Single = locs(0)
			Return If(x1 > x0, x1 - x0, x0 - x1)
		End Function

		Public Overridable Function getHeight(ByVal v As GlyphView) As Single
			Return layout.ascent + layout.descent + layout.leading
		End Function

		''' <summary>
		''' Fetch the ascent above the baseline for the glyphs
		''' corresponding to the given range in the model.
		''' </summary>
		Public Overridable Function getAscent(ByVal v As GlyphView) As Single
			Return layout.ascent
		End Function

		''' <summary>
		''' Fetch the descent below the baseline for the glyphs
		''' corresponding to the given range in the model.
		''' </summary>
		Public Overridable Function getDescent(ByVal v As GlyphView) As Single
			Return layout.descent
		End Function

		''' <summary>
		''' Paint the glyphs for the given view.  This is implemented
		''' to only render if the Graphics is of type Graphics2D which
		''' is required by TextLayout (and this should be the case if
		''' running on the JDK).
		''' </summary>
		Public Overridable Sub paint(ByVal v As GlyphView, ByVal g As Graphics, ByVal a As Shape, ByVal p0 As Integer, ByVal p1 As Integer)
			If TypeOf g Is Graphics2D Then
				Dim alloc As java.awt.geom.Rectangle2D = a.bounds2D
				Dim g2d As Graphics2D = CType(g, Graphics2D)
				Dim y As Single = CSng(alloc.y) + layout.ascent + layout.leading
				Dim x As Single = CSng(alloc.x)
				If p0 > v.startOffset OrElse p1 < v.endOffset Then
					Try
						'TextLayout can't render only part of it's range, so if a
						'partial range is required, add a clip region.
						Dim s As Shape = v.modelToView(p0, Position.Bias.Forward, p1, Position.Bias.Backward, a)
						Dim savedClip As Shape = g.clip
						g2d.clip(s)
						layout.draw(g2d, x, y)
						g.clip = savedClip
					Catch e As BadLocationException
					End Try
				Else
					layout.draw(g2d, x, y)
				End If
			End If
		End Sub

		Public Overridable Function modelToView(ByVal v As GlyphView, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal a As Shape) As Shape
			Dim offs As Integer = pos - v.startOffset
			Dim alloc As java.awt.geom.Rectangle2D = a.bounds2D
			Dim hit As TextHitInfo = If(bias Is Position.Bias.Forward, TextHitInfo.afterOffset(offs), TextHitInfo.beforeOffset(offs))
			Dim locs As Single() = layout.getCaretInfo(hit)

			' vertical at the baseline, should use slope and check if glyphs
			' are being rendered vertically.
			alloc.rectect(alloc.x + locs(0), alloc.y, 1, alloc.height)
			Return alloc
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="v"> the view containing the view coordinates </param>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="biasReturn"> either <code>Position.Bias.Forward</code>
		'''  or <code>Position.Bias.Backward</code> is returned as the
		'''  zero-th element of this array </param>
		''' <returns> the location within the model that best represents the
		'''  given point of view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overridable Function viewToModel(ByVal v As GlyphView, ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal biasReturn As Position.Bias()) As Integer

			Dim alloc As java.awt.geom.Rectangle2D = If(TypeOf a Is java.awt.geom.Rectangle2D, CType(a, java.awt.geom.Rectangle2D), a.bounds2D)
			'Move the y co-ord of the hit onto the baseline.  This is because TextLayout supports
			'italic carets and we do not.
			Dim hit As TextHitInfo = layout.hitTestChar(x - CSng(alloc.x), 0)
			Dim pos As Integer = hit.insertionIndex

			If pos = v.endOffset Then pos -= 1

			biasReturn(0) = If(hit.leadingEdge, Position.Bias.Forward, Position.Bias.Backward)
			Return pos + v.startOffset
		End Function

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
		'''  fragment should start it's representation >= 0. </param>
		''' <param name="pos"> the graphic location along the axis that the
		'''  broken view would occupy >= 0.  This may be useful for
		'''  things like tab calculations. </param>
		''' <param name="len"> specifies the distance into the view
		'''  where a potential break is desired >= 0. </param>
		''' <returns> the maximum model location possible for a break. </returns>
		''' <seealso cref= View#breakView </seealso>
		Public Overridable Function getBoundedPosition(ByVal v As GlyphView, ByVal p0 As Integer, ByVal x As Single, ByVal len As Single) As Integer
			If len < 0 Then Throw New System.ArgumentException("Length must be >= 0.")
			' note: this only works because swing uses TextLayouts that are
			' only pure rtl or pure ltr
			Dim hit As TextHitInfo
			If layout.leftToRight Then
				hit = layout.hitTestChar(len, 0)
			Else
				hit = layout.hitTestChar(layout.advance - len, 0)
			End If
			Return v.startOffset + hit.charIndex
		End Function

		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be
		''' visible, they might not be in the same order found in the model, or
		''' they just might not allow access to some of the locations in the
		''' model.
		''' </summary>
		''' <param name="v"> the view to use </param>
		''' <param name="pos"> the position to convert >= 0 </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <param name="direction"> the direction from the current position that can
		'''  be thought of as the arrow keys typically found on a keyboard.
		'''  This may be SwingConstants.WEST, SwingConstants.EAST,
		'''  SwingConstants.NORTH, or SwingConstants.SOUTH. </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position. </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> for an invalid direction </exception>
			Public Overridable Function getNextVisualPositionFrom(ByVal v As GlyphView, ByVal pos As Integer, ByVal b As Position.Bias, ByVal a As Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer

				Dim doc As Document = v.document
				Dim startOffset As Integer = v.startOffset
				Dim endOffset As Integer = v.endOffset
				Dim text As Segment
				Dim viewIsLeftToRight As Boolean
				Dim currentHit, nextHit As TextHitInfo

				Select Case direction
				Case View.NORTH
				Case View.SOUTH
				Case View.EAST
					viewIsLeftToRight = AbstractDocument.isLeftToRight(doc, startOffset, endOffset)

					If startOffset = doc.length Then
						If pos = -1 Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
						' End case for bidi text where newline is at beginning
						' of line.
						Return -1
					End If
					If pos = -1 Then
						' Entering view from the left.
						If viewIsLeftToRight Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						Else
							text = v.getText(endOffset - 1, endOffset)
							Dim c As Char = text.array(text.offset)
							SegmentCache.releaseSharedSegment(text)
							If c = ControlChars.Lf Then
								biasRet(0) = Position.Bias.Forward
								Return endOffset-1
							End If
							biasRet(0) = Position.Bias.Backward
							Return endOffset
						End If
					End If
					If b Is Position.Bias.Forward Then
						currentHit = TextHitInfo.afterOffset(pos-startOffset)
					Else
						currentHit = TextHitInfo.beforeOffset(pos-startOffset)
					End If
					nextHit = layout.getNextRightHit(currentHit)
					If nextHit Is Nothing Then Return -1
					If viewIsLeftToRight <> layout.leftToRight Then nextHit = layout.getVisualOtherHit(nextHit)
					pos = nextHit.insertionIndex + startOffset

					If pos = endOffset Then
						' A move to the right from an internal position will
						' only take us to the endOffset in a left to right run.
						text = v.getText(endOffset - 1, endOffset)
						Dim c As Char = text.array(text.offset)
						SegmentCache.releaseSharedSegment(text)
						If c = ControlChars.Lf Then Return -1
						biasRet(0) = Position.Bias.Backward
					Else
						biasRet(0) = Position.Bias.Forward
					End If
					Return pos
				Case View.WEST
					viewIsLeftToRight = AbstractDocument.isLeftToRight(doc, startOffset, endOffset)

					If startOffset = doc.length Then
						If pos = -1 Then
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
						' End case for bidi text where newline is at beginning
						' of line.
						Return -1
					End If
					If pos = -1 Then
						' Entering view from the right
						If viewIsLeftToRight Then
							text = v.getText(endOffset - 1, endOffset)
							Dim c As Char = text.array(text.offset)
							SegmentCache.releaseSharedSegment(text)
							If (c = ControlChars.Lf) OrElse Char.isSpaceChar(c) Then
								biasRet(0) = Position.Bias.Forward
								Return endOffset - 1
							End If
							biasRet(0) = Position.Bias.Backward
							Return endOffset
						Else
							biasRet(0) = Position.Bias.Forward
							Return startOffset
						End If
					End If
					If b Is Position.Bias.Forward Then
						currentHit = TextHitInfo.afterOffset(pos-startOffset)
					Else
						currentHit = TextHitInfo.beforeOffset(pos-startOffset)
					End If
					nextHit = layout.getNextLeftHit(currentHit)
					If nextHit Is Nothing Then Return -1
					If viewIsLeftToRight <> layout.leftToRight Then nextHit = layout.getVisualOtherHit(nextHit)
					pos = nextHit.insertionIndex + startOffset

					If pos = endOffset Then
						' A move to the left from an internal position will
						' only take us to the endOffset in a right to left run.
						text = v.getText(endOffset - 1, endOffset)
						Dim c As Char = text.array(text.offset)
						SegmentCache.releaseSharedSegment(text)
						If c = ControlChars.Lf Then Return -1
						biasRet(0) = Position.Bias.Backward
					Else
						biasRet(0) = Position.Bias.Forward
					End If
					Return pos
				Case Else
					Throw New System.ArgumentException("Bad direction: " & direction)
				End Select
				Return pos

			End Function
		' --- variables ---------------------------------------------

		Friend layout As TextLayout

	End Class

End Namespace