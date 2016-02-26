Imports Microsoft.VisualBasic
Imports System

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
	''' A collection of methods to deal with various text
	''' related activities.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class Utilities
		''' <summary>
		''' If <code>view</code>'s container is a <code>JComponent</code> it
		''' is returned, after casting.
		''' </summary>
		Friend Shared Function getJComponent(ByVal ___view As View) As javax.swing.JComponent
			If ___view IsNot Nothing Then
				Dim component As java.awt.Component = ___view.container
				If TypeOf component Is javax.swing.JComponent Then Return CType(component, javax.swing.JComponent)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Draws the given text, expanding any tabs that are contained
		''' using the given tab expansion technique.  This particular
		''' implementation renders in a 1.1 style coordinate system
		''' where ints are used and 72dpi is assumed.
		''' </summary>
		''' <param name="s">  the source of the text </param>
		''' <param name="x">  the X origin &gt;= 0 </param>
		''' <param name="y">  the Y origin &gt;= 0 </param>
		''' <param name="g">  the graphics context </param>
		''' <param name="e">  how to expand the tabs.  If this value is null,
		'''   tabs will be expanded as a space character. </param>
		''' <param name="startOffset"> starting offset of the text in the document &gt;= 0 </param>
		''' <returns>  the X location at the end of the rendered text </returns>
		Public Shared Function drawTabbedText(ByVal s As Segment, ByVal x As Integer, ByVal y As Integer, ByVal g As java.awt.Graphics, ByVal e As TabExpander, ByVal startOffset As Integer) As Integer
			Return drawTabbedText(Nothing, s, x, y, g, e, startOffset)
		End Function

		''' <summary>
		''' Draws the given text, expanding any tabs that are contained
		''' using the given tab expansion technique.  This particular
		''' implementation renders in a 1.1 style coordinate system
		''' where ints are used and 72dpi is assumed.
		''' </summary>
		''' <param name="view"> View requesting rendering, may be null. </param>
		''' <param name="s">  the source of the text </param>
		''' <param name="x">  the X origin &gt;= 0 </param>
		''' <param name="y">  the Y origin &gt;= 0 </param>
		''' <param name="g">  the graphics context </param>
		''' <param name="e">  how to expand the tabs.  If this value is null,
		'''   tabs will be expanded as a space character. </param>
		''' <param name="startOffset"> starting offset of the text in the document &gt;= 0 </param>
		''' <returns>  the X location at the end of the rendered text </returns>
		Friend Shared Function drawTabbedText(ByVal ___view As View, ByVal s As Segment, ByVal x As Integer, ByVal y As Integer, ByVal g As java.awt.Graphics, ByVal e As TabExpander, ByVal startOffset As Integer) As Integer
			Return drawTabbedText(___view, s, x, y, g, e, startOffset, Nothing)
		End Function

		' In addition to the previous method it can extend spaces for
		' justification.
		'
		' all params are the same as in the preious method except the last
		' one:
		' @param justificationData justificationData for the row.
		' if null not justification is needed
		Friend Shared Function drawTabbedText(ByVal ___view As View, ByVal s As Segment, ByVal x As Integer, ByVal y As Integer, ByVal g As java.awt.Graphics, ByVal e As TabExpander, ByVal startOffset As Integer, ByVal justificationData As Integer ()) As Integer
			Dim component As javax.swing.JComponent = getJComponent(___view)
			Dim metrics As java.awt.FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(component, g)
			Dim nextX As Integer = x
			Dim txt As Char() = s.array
			Dim txtOffset As Integer = s.offset
			Dim flushLen As Integer = 0
			Dim flushIndex As Integer = s.offset
			Dim spaceAddon As Integer = 0
			Dim spaceAddonLeftoverEnd As Integer = -1
			Dim startJustifiableContent As Integer = 0
			Dim endJustifiableContent As Integer = 0
			If justificationData IsNot Nothing Then
				Dim offset As Integer = - startOffset + txtOffset
				Dim parent As View = Nothing
				parent = ___view.parent
				If ___view IsNot Nothing AndAlso parent IsNot Nothing Then offset += parent.startOffset
				spaceAddon = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON)
				spaceAddonLeftoverEnd = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON_LEFTOVER_END) + offset
				startJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.START_JUSTIFIABLE) + offset
				endJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.END_JUSTIFIABLE) + offset
			End If
			Dim n As Integer = s.offset + s.count
			For i As Integer = txtOffset To n - 1
				If txt(i) = ControlChars.Tab OrElse ((spaceAddon <> 0 OrElse i <= spaceAddonLeftoverEnd) AndAlso (txt(i) = " "c) AndAlso startJustifiableContent <= i AndAlso i <= endJustifiableContent) Then
					If flushLen > 0 Then
						nextX = sun.swing.SwingUtilities2.drawChars(component, g, txt, flushIndex, flushLen, x, y)
						flushLen = 0
					End If
					flushIndex = i + 1
					If txt(i) = ControlChars.Tab Then
						If e IsNot Nothing Then
							nextX = CInt(Fix(e.nextTabStop(CSng(nextX), startOffset + i - txtOffset)))
						Else
							nextX += metrics.charWidth(" "c)
						End If
					ElseIf txt(i) = " "c Then
						nextX += metrics.charWidth(" "c) + spaceAddon
						If i <= spaceAddonLeftoverEnd Then nextX += 1
					End If
					x = nextX
				ElseIf (txt(i) = ControlChars.Lf) OrElse (txt(i) = ControlChars.Cr) Then
					If flushLen > 0 Then
						nextX = sun.swing.SwingUtilities2.drawChars(component, g, txt, flushIndex, flushLen, x, y)
						flushLen = 0
					End If
					flushIndex = i + 1
					x = nextX
				Else
					flushLen += 1
				End If
			Next i
			If flushLen > 0 Then nextX = sun.swing.SwingUtilities2.drawChars(component, g,txt, flushIndex, flushLen, x, y)
			Return nextX
		End Function

		''' <summary>
		''' Determines the width of the given segment of text taking tabs
		''' into consideration.  This is implemented in a 1.1 style coordinate
		''' system where ints are used and 72dpi is assumed.
		''' </summary>
		''' <param name="s">  the source of the text </param>
		''' <param name="metrics"> the font metrics to use for the calculation </param>
		''' <param name="x">  the X origin &gt;= 0 </param>
		''' <param name="e">  how to expand the tabs.  If this value is null,
		'''   tabs will be expanded as a space character. </param>
		''' <param name="startOffset"> starting offset of the text in the document &gt;= 0 </param>
		''' <returns>  the width of the text </returns>
		Public Shared Function getTabbedTextWidth(ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer) As Integer
			Return getTabbedTextWidth(Nothing, s, metrics, x, e, startOffset, Nothing)
		End Function


		' In addition to the previous method it can extend spaces for
		' justification.
		'
		' all params are the same as in the preious method except the last
		' one:
		' @param justificationData justificationData for the row.
		' if null not justification is needed
		Friend Shared Function getTabbedTextWidth(ByVal ___view As View, ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer, ByVal justificationData As Integer()) As Integer
			Dim nextX As Integer = x
			Dim txt As Char() = s.array
			Dim txtOffset As Integer = s.offset
			Dim n As Integer = s.offset + s.count
			Dim charCount As Integer = 0
			Dim spaceAddon As Integer = 0
			Dim spaceAddonLeftoverEnd As Integer = -1
			Dim startJustifiableContent As Integer = 0
			Dim endJustifiableContent As Integer = 0
			If justificationData IsNot Nothing Then
				Dim offset As Integer = - startOffset + txtOffset
				Dim parent As View = Nothing
				parent = ___view.parent
				If ___view IsNot Nothing AndAlso parent IsNot Nothing Then offset += parent.startOffset
				spaceAddon = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON)
				spaceAddonLeftoverEnd = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON_LEFTOVER_END) + offset
				startJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.START_JUSTIFIABLE) + offset
				endJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.END_JUSTIFIABLE) + offset
			End If

			For i As Integer = txtOffset To n - 1
				If txt(i) = ControlChars.Tab OrElse ((spaceAddon <> 0 OrElse i <= spaceAddonLeftoverEnd) AndAlso (txt(i) = " "c) AndAlso startJustifiableContent <= i AndAlso i <= endJustifiableContent) Then
					nextX += metrics.charsWidth(txt, i-charCount, charCount)
					charCount = 0
					If txt(i) = ControlChars.Tab Then
						If e IsNot Nothing Then
							nextX = CInt(Fix(e.nextTabStop(CSng(nextX), startOffset + i - txtOffset)))
						Else
							nextX += metrics.charWidth(" "c)
						End If
					ElseIf txt(i) = " "c Then
						nextX += metrics.charWidth(" "c) + spaceAddon
						If i <= spaceAddonLeftoverEnd Then nextX += 1
					End If
				ElseIf txt(i) = ControlChars.Lf Then
				' Ignore newlines, they take up space and we shouldn't be
				' counting them.
					nextX += metrics.charsWidth(txt, i - charCount, charCount)
					charCount = 0
				Else
					charCount += 1
				End If
			Next i
			nextX += metrics.charsWidth(txt, n - charCount, charCount)
			Return nextX - x
		End Function

		''' <summary>
		''' Determines the relative offset into the given text that
		''' best represents the given span in the view coordinate
		''' system.  This is implemented in a 1.1 style coordinate
		''' system where ints are used and 72dpi is assumed.
		''' </summary>
		''' <param name="s">  the source of the text </param>
		''' <param name="metrics"> the font metrics to use for the calculation </param>
		''' <param name="x0"> the starting view location representing the start
		'''   of the given text &gt;= 0. </param>
		''' <param name="x">  the target view location to translate to an
		'''   offset into the text &gt;= 0. </param>
		''' <param name="e">  how to expand the tabs.  If this value is null,
		'''   tabs will be expanded as a space character. </param>
		''' <param name="startOffset"> starting offset of the text in the document &gt;= 0 </param>
		''' <returns>  the offset into the text &gt;= 0 </returns>
		Public Shared Function getTabbedTextOffset(ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x0 As Integer, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer) As Integer
			Return getTabbedTextOffset(s, metrics, x0, x, e, startOffset, True)
		End Function

		Friend Shared Function getTabbedTextOffset(ByVal ___view As View, ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x0 As Integer, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer, ByVal justificationData As Integer()) As Integer
			Return getTabbedTextOffset(___view, s, metrics, x0, x, e, startOffset, True, justificationData)
		End Function

		Public Shared Function getTabbedTextOffset(ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x0 As Integer, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer, ByVal round As Boolean) As Integer
			Return getTabbedTextOffset(Nothing, s, metrics, x0, x, e, startOffset, round, Nothing)
		End Function

		' In addition to the previous method it can extend spaces for
		' justification.
		'
		' all params are the same as in the preious method except the last
		' one:
		' @param justificationData justificationData for the row.
		' if null not justification is needed
		Friend Shared Function getTabbedTextOffset(ByVal ___view As View, ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x0 As Integer, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer, ByVal round As Boolean, ByVal justificationData As Integer()) As Integer
			If x0 >= x Then Return 0
			Dim nextX As Integer = x0
			' s may be a shared segment, so it is copied prior to calling
			' the tab expander
			Dim txt As Char() = s.array
			Dim txtOffset As Integer = s.offset
			Dim txtCount As Integer = s.count
			Dim spaceAddon As Integer = 0
			Dim spaceAddonLeftoverEnd As Integer = -1
			Dim startJustifiableContent As Integer = 0
			Dim endJustifiableContent As Integer = 0
			If justificationData IsNot Nothing Then
				Dim offset As Integer = - startOffset + txtOffset
				Dim parent As View = Nothing
				parent = ___view.parent
				If ___view IsNot Nothing AndAlso parent IsNot Nothing Then offset += parent.startOffset
				spaceAddon = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON)
				spaceAddonLeftoverEnd = justificationData(javax.swing.text.ParagraphView.Row.SPACE_ADDON_LEFTOVER_END) + offset
				startJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.START_JUSTIFIABLE) + offset
				endJustifiableContent = justificationData(javax.swing.text.ParagraphView.Row.END_JUSTIFIABLE) + offset
			End If
			Dim n As Integer = s.offset + s.count
			For i As Integer = s.offset To n - 1
				If txt(i) = ControlChars.Tab OrElse ((spaceAddon <> 0 OrElse i <= spaceAddonLeftoverEnd) AndAlso (txt(i) = " "c) AndAlso startJustifiableContent <= i AndAlso i <= endJustifiableContent) Then
					If txt(i) = ControlChars.Tab Then
						If e IsNot Nothing Then
							nextX = CInt(Fix(e.nextTabStop(CSng(nextX), startOffset + i - txtOffset)))
						Else
							nextX += metrics.charWidth(" "c)
						End If
					ElseIf txt(i) = " "c Then
						nextX += metrics.charWidth(" "c) + spaceAddon
						If i <= spaceAddonLeftoverEnd Then nextX += 1
					End If
				Else
					nextX += metrics.charWidth(txt(i))
				End If
				If x < nextX Then
					' found the hit position... return the appropriate side
					Dim offset As Integer

					' the length of the string measured as a whole may differ from
					' the sum of individual character lengths, for example if
					' fractional metrics are enabled; and we must guard from this.
					If round Then
						offset = i + 1 - txtOffset

						Dim width As Integer = metrics.charsWidth(txt, txtOffset, offset)
						Dim span As Integer = x - x0

						If span < width Then
							Do While offset > 0
								Dim nextWidth As Integer = If(offset > 1, metrics.charsWidth(txt, txtOffset, offset - 1), 0)

								If span >= nextWidth Then
									If span - nextWidth < width - span Then offset -= 1

									Exit Do
								End If

								width = nextWidth
								offset -= 1
							Loop
						End If
					Else
						offset = i - txtOffset

						Do While offset > 0 AndAlso metrics.charsWidth(txt, txtOffset, offset) > (x - x0)
							offset -= 1
						Loop
					End If

					Return offset
				End If
			Next i

			' didn't find, return end offset
			Return txtCount
		End Function

		''' <summary>
		''' Determine where to break the given text to fit
		''' within the given span. This tries to find a word boundary. </summary>
		''' <param name="s">  the source of the text </param>
		''' <param name="metrics"> the font metrics to use for the calculation </param>
		''' <param name="x0"> the starting view location representing the start
		'''   of the given text. </param>
		''' <param name="x">  the target view location to translate to an
		'''   offset into the text. </param>
		''' <param name="e">  how to expand the tabs.  If this value is null,
		'''   tabs will be expanded as a space character. </param>
		''' <param name="startOffset"> starting offset in the document of the text </param>
		''' <returns>  the offset into the given text </returns>
		Public Shared Function getBreakLocation(ByVal s As Segment, ByVal metrics As java.awt.FontMetrics, ByVal x0 As Integer, ByVal x As Integer, ByVal e As TabExpander, ByVal startOffset As Integer) As Integer
			Dim txt As Char() = s.array
			Dim txtOffset As Integer = s.offset
			Dim txtCount As Integer = s.count
			Dim index As Integer = Utilities.getTabbedTextOffset(s, metrics, x0, x, e, startOffset, False)

			If index >= txtCount - 1 Then Return txtCount

			For i As Integer = txtOffset + index To txtOffset Step -1
				Dim ch As Char = txt(i)
				If AscW(ch) < 256 Then
					' break on whitespace
					If Char.IsWhiteSpace(ch) Then
						index = i - txtOffset + 1
						Exit For
					End If
				Else
					' a multibyte char found; use BreakIterator to find line break
					Dim bit As BreakIterator = BreakIterator.lineInstance
					bit.text = s
					Dim breakPos As Integer = bit.preceding(i + 1)
					If breakPos > txtOffset Then index = breakPos - txtOffset
					Exit For
				End If
			Next i
			Return index
		End Function

		''' <summary>
		''' Determines the starting row model position of the row that contains
		''' the specified model position.  The component given must have a
		''' size to compute the result.  If the component doesn't have a size
		''' a value of -1 will be returned.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the position &gt;= 0 if the request can be computed, otherwise
		'''  a value of -1 will be returned. </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getRowStart(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim r As java.awt.Rectangle = c.modelToView(offs)
			If r Is Nothing Then Return -1
			Dim lastOffs As Integer = offs
			Dim y As Integer = r.y
			Do While (r IsNot Nothing) AndAlso (y = r.y)
				' Skip invisible elements
				If r.height <>0 Then offs = lastOffs
				lastOffs -= 1
				r = If(lastOffs >= 0, c.modelToView(lastOffs), Nothing)
			Loop
			Return offs
		End Function

		''' <summary>
		''' Determines the ending row model position of the row that contains
		''' the specified model position.  The component given must have a
		''' size to compute the result.  If the component doesn't have a size
		''' a value of -1 will be returned.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the position &gt;= 0 if the request can be computed, otherwise
		'''  a value of -1 will be returned. </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getRowEnd(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim r As java.awt.Rectangle = c.modelToView(offs)
			If r Is Nothing Then Return -1
			Dim n As Integer = c.document.length
			Dim lastOffs As Integer = offs
			Dim y As Integer = r.y
			Do While (r IsNot Nothing) AndAlso (y = r.y)
				' Skip invisible elements
				If r.height <>0 Then offs = lastOffs
				lastOffs += 1
				r = If(lastOffs <= n, c.modelToView(lastOffs), Nothing)
			Loop
			Return offs
		End Function

		''' <summary>
		''' Determines the position in the model that is closest to the given
		''' view location in the row above.  The component given must have a
		''' size to compute the result.  If the component doesn't have a size
		''' a value of -1 will be returned.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <returns> the position &gt;= 0 if the request can be computed, otherwise
		'''  a value of -1 will be returned. </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getPositionAbove(ByVal c As JTextComponent, ByVal offs As Integer, ByVal x As Integer) As Integer
			Dim lastOffs As Integer = getRowStart(c, offs) - 1
			If lastOffs < 0 Then Return -1
			Dim bestSpan As Integer = Integer.MaxValue
			Dim y As Integer = 0
			Dim r As java.awt.Rectangle = Nothing
			If lastOffs >= 0 Then
				r = c.modelToView(lastOffs)
				y = r.y
			End If
			Do While (r IsNot Nothing) AndAlso (y = r.y)
				Dim span As Integer = Math.Abs(r.x - x)
				If span < bestSpan Then
					offs = lastOffs
					bestSpan = span
				End If
				lastOffs -= 1
				r = If(lastOffs >= 0, c.modelToView(lastOffs), Nothing)
			Loop
			Return offs
		End Function

		''' <summary>
		''' Determines the position in the model that is closest to the given
		''' view location in the row below.  The component given must have a
		''' size to compute the result.  If the component doesn't have a size
		''' a value of -1 will be returned.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <param name="x"> the X coordinate &gt;= 0 </param>
		''' <returns> the position &gt;= 0 if the request can be computed, otherwise
		'''  a value of -1 will be returned. </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getPositionBelow(ByVal c As JTextComponent, ByVal offs As Integer, ByVal x As Integer) As Integer
			Dim lastOffs As Integer = getRowEnd(c, offs) + 1
			If lastOffs <= 0 Then Return -1
			Dim bestSpan As Integer = Integer.MaxValue
			Dim n As Integer = c.document.length
			Dim y As Integer = 0
			Dim r As java.awt.Rectangle = Nothing
			If lastOffs <= n Then
				r = c.modelToView(lastOffs)
				y = r.y
			End If
			Do While (r IsNot Nothing) AndAlso (y = r.y)
				Dim span As Integer = Math.Abs(x - r.x)
				If span < bestSpan Then
					offs = lastOffs
					bestSpan = span
				End If
				lastOffs += 1
				r = If(lastOffs <= n, c.modelToView(lastOffs), Nothing)
			Loop
			Return offs
		End Function

		''' <summary>
		''' Determines the start of a word for the given model location.
		''' Uses BreakIterator.getWordInstance() to actually get the words.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the location in the model of the word start &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getWordStart(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim doc As Document = c.document
			Dim line As Element = getParagraphElement(c, offs)
			If line Is Nothing Then Throw New BadLocationException("No word at " & offs, offs)
			Dim lineStart As Integer = line.startOffset
			Dim lineEnd As Integer = Math.Min(line.endOffset, doc.length)

			Dim seg As Segment = SegmentCache.sharedSegment
			doc.getText(lineStart, lineEnd - lineStart, seg)
			If seg.count > 0 Then
				Dim words As BreakIterator = BreakIterator.getWordInstance(c.locale)
				words.text = seg
				Dim wordPosition As Integer = seg.offset + offs - lineStart
				If wordPosition >= words.last() Then wordPosition = words.last() - 1
				words.following(wordPosition)
				offs = lineStart + words.previous() - seg.offset
			End If
			SegmentCache.releaseSharedSegment(seg)
			Return offs
		End Function

		''' <summary>
		''' Determines the end of a word for the given location.
		''' Uses BreakIterator.getWordInstance() to actually get the words.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the location in the model of the word end &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getWordEnd(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim doc As Document = c.document
			Dim line As Element = getParagraphElement(c, offs)
			If line Is Nothing Then Throw New BadLocationException("No word at " & offs, offs)
			Dim lineStart As Integer = line.startOffset
			Dim lineEnd As Integer = Math.Min(line.endOffset, doc.length)

			Dim seg As Segment = SegmentCache.sharedSegment
			doc.getText(lineStart, lineEnd - lineStart, seg)
			If seg.count > 0 Then
				Dim words As BreakIterator = BreakIterator.getWordInstance(c.locale)
				words.text = seg
				Dim wordPosition As Integer = offs - lineStart + seg.offset
				If wordPosition >= words.last() Then wordPosition = words.last() - 1
				offs = lineStart + words.following(wordPosition) - seg.offset
			End If
			SegmentCache.releaseSharedSegment(seg)
			Return offs
		End Function

		''' <summary>
		''' Determines the start of the next word for the given location.
		''' Uses BreakIterator.getWordInstance() to actually get the words.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the location in the model of the word start &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getNextWord(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim ___nextWord As Integer
			Dim line As Element = getParagraphElement(c, offs)
			___nextWord = getNextWordInParagraph(c, line, offs, False)
			Do While ___nextWord = BreakIterator.DONE

				' didn't find in this line, try the next line
				offs = line.endOffset
				line = getParagraphElement(c, offs)
				___nextWord = getNextWordInParagraph(c, line, offs, True)
			Loop
			Return ___nextWord
		End Function

		''' <summary>
		''' Finds the next word in the given elements text.  The first
		''' parameter allows searching multiple paragraphs where even
		''' the first offset is desired.
		''' Returns the offset of the next word, or BreakIterator.DONE
		''' if there are no more words in the element.
		''' </summary>
		Friend Shared Function getNextWordInParagraph(ByVal c As JTextComponent, ByVal line As Element, ByVal offs As Integer, ByVal first As Boolean) As Integer
			If line Is Nothing Then Throw New BadLocationException("No more words", offs)
			Dim doc As Document = line.document
			Dim lineStart As Integer = line.startOffset
			Dim lineEnd As Integer = Math.Min(line.endOffset, doc.length)
			If (offs >= lineEnd) OrElse (offs < lineStart) Then Throw New BadLocationException("No more words", offs)
			Dim seg As Segment = SegmentCache.sharedSegment
			doc.getText(lineStart, lineEnd - lineStart, seg)
			Dim words As BreakIterator = BreakIterator.getWordInstance(c.locale)
			words.text = seg
			If (first AndAlso (words.first() = (seg.offset + offs - lineStart))) AndAlso ((Not Char.IsWhiteSpace(seg.array(words.first())))) Then Return offs
			Dim wordPosition As Integer = words.following(seg.offset + offs - lineStart)
			If (wordPosition = BreakIterator.DONE) OrElse (wordPosition >= seg.offset + seg.count) Then Return BreakIterator.DONE
			' if we haven't shot past the end... check to
			' see if the current boundary represents whitespace.
			' if so, we need to try again
			Dim ch As Char = seg.array(wordPosition)
			If Not Char.IsWhiteSpace(ch) Then Return lineStart + wordPosition - seg.offset

			' it was whitespace, try again.  The assumption
			' is that it must be a word start if the last
			' one had whitespace following it.
			wordPosition = words.next()
			If wordPosition <> BreakIterator.DONE Then
				offs = lineStart + wordPosition - seg.offset
				If offs <> lineEnd Then Return offs
			End If
			SegmentCache.releaseSharedSegment(seg)
			Return BreakIterator.DONE
		End Function


		''' <summary>
		''' Determine the start of the prev word for the given location.
		''' Uses BreakIterator.getWordInstance() to actually get the words.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the offset in the document &gt;= 0 </param>
		''' <returns> the location in the model of the word start &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> if the offset is out of range </exception>
		Public Shared Function getPreviousWord(ByVal c As JTextComponent, ByVal offs As Integer) As Integer
			Dim prevWord As Integer
			Dim line As Element = getParagraphElement(c, offs)
			prevWord = getPrevWordInParagraph(c, line, offs)
			Do While prevWord = BreakIterator.DONE

				' didn't find in this line, try the prev line
				offs = line.startOffset - 1
				line = getParagraphElement(c, offs)
				prevWord = getPrevWordInParagraph(c, line, offs)
			Loop
			Return prevWord
		End Function

		''' <summary>
		''' Finds the previous word in the given elements text.  The first
		''' parameter allows searching multiple paragraphs where even
		''' the first offset is desired.
		''' Returns the offset of the next word, or BreakIterator.DONE
		''' if there are no more words in the element.
		''' </summary>
		Friend Shared Function getPrevWordInParagraph(ByVal c As JTextComponent, ByVal line As Element, ByVal offs As Integer) As Integer
			If line Is Nothing Then Throw New BadLocationException("No more words", offs)
			Dim doc As Document = line.document
			Dim lineStart As Integer = line.startOffset
			Dim lineEnd As Integer = line.endOffset
			If (offs > lineEnd) OrElse (offs < lineStart) Then Throw New BadLocationException("No more words", offs)
			Dim seg As Segment = SegmentCache.sharedSegment
			doc.getText(lineStart, lineEnd - lineStart, seg)
			Dim words As BreakIterator = BreakIterator.getWordInstance(c.locale)
			words.text = seg
			If words.following(seg.offset + offs - lineStart) = BreakIterator.DONE Then words.last()
			Dim wordPosition As Integer = words.previous()
			If wordPosition = (seg.offset + offs - lineStart) Then wordPosition = words.previous()

			If wordPosition = BreakIterator.DONE Then Return BreakIterator.DONE
			' if we haven't shot past the end... check to
			' see if the current boundary represents whitespace.
			' if so, we need to try again
			Dim ch As Char = seg.array(wordPosition)
			If Not Char.IsWhiteSpace(ch) Then Return lineStart + wordPosition - seg.offset

			' it was whitespace, try again.  The assumption
			' is that it must be a word start if the last
			' one had whitespace following it.
			wordPosition = words.previous()
			If wordPosition <> BreakIterator.DONE Then Return lineStart + wordPosition - seg.offset
			SegmentCache.releaseSharedSegment(seg)
			Return BreakIterator.DONE
		End Function

		''' <summary>
		''' Determines the element to use for a paragraph/line.
		''' </summary>
		''' <param name="c"> the editor </param>
		''' <param name="offs"> the starting offset in the document &gt;= 0 </param>
		''' <returns> the element </returns>
		Public Shared Function getParagraphElement(ByVal c As JTextComponent, ByVal offs As Integer) As Element
			Dim doc As Document = c.document
			If TypeOf doc Is StyledDocument Then Return CType(doc, StyledDocument).getParagraphElement(offs)
			Dim map As Element = doc.defaultRootElement
			Dim index As Integer = map.getElementIndex(offs)
			Dim paragraph As Element = map.getElement(index)
			If (offs >= paragraph.startOffset) AndAlso (offs < paragraph.endOffset) Then Return paragraph
			Return Nothing
		End Function

		Friend Shared Function isComposedTextElement(ByVal doc As Document, ByVal offset As Integer) As Boolean
			Dim elem As Element = doc.defaultRootElement
			Do While Not elem.leaf
				elem = elem.getElement(elem.getElementIndex(offset))
			Loop
			Return isComposedTextElement(elem)
		End Function

		Friend Shared Function isComposedTextElement(ByVal elem As Element) As Boolean
			Dim [as] As AttributeSet = elem.attributes
			Return isComposedTextAttributeDefined([as])
		End Function

		Friend Shared Function isComposedTextAttributeDefined(ByVal [as] As AttributeSet) As Boolean
			Return (([as] IsNot Nothing) AndAlso ([as].isDefined(StyleConstants.ComposedTextAttribute)))
		End Function

		''' <summary>
		''' Draws the given composed text passed from an input method.
		''' </summary>
		''' <param name="view"> View hosting text </param>
		''' <param name="attr"> the attributes containing the composed text </param>
		''' <param name="g">  the graphics context </param>
		''' <param name="x">  the X origin </param>
		''' <param name="y">  the Y origin </param>
		''' <param name="p0"> starting offset in the composed text to be rendered </param>
		''' <param name="p1"> ending offset in the composed text to be rendered </param>
		''' <returns>  the new insertion position </returns>
		Friend Shared Function drawComposedText(ByVal ___view As View, ByVal attr As AttributeSet, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal p0 As Integer, ByVal p1 As Integer) As Integer
			Dim g2d As java.awt.Graphics2D = CType(g, java.awt.Graphics2D)
			Dim [as] As AttributedString = CType(attr.getAttribute(StyleConstants.ComposedTextAttribute), AttributedString)
			[as].addAttribute(java.awt.font.TextAttribute.FONT, g.font)

			If p0 >= p1 Then Return x

			Dim aci As AttributedCharacterIterator = [as].getIterator(Nothing, p0, p1)
			Return x + CInt(Fix(sun.swing.SwingUtilities2.drawString(getJComponent(___view), g2d,aci,x,y)))
		End Function

		''' <summary>
		''' Paints the composed text in a GlyphView
		''' </summary>
		Friend Shared Sub paintComposedText(ByVal g As java.awt.Graphics, ByVal alloc As java.awt.Rectangle, ByVal v As GlyphView)
			If TypeOf g Is java.awt.Graphics2D Then
				Dim g2d As java.awt.Graphics2D = CType(g, java.awt.Graphics2D)
				Dim p0 As Integer = v.startOffset
				Dim p1 As Integer = v.endOffset
				Dim attrSet As AttributeSet = v.element.attributes
				Dim [as] As AttributedString = CType(attrSet.getAttribute(StyleConstants.ComposedTextAttribute), AttributedString)
				Dim start As Integer = v.element.startOffset
				Dim y As Integer = alloc.y + alloc.height - CInt(Fix(v.glyphPainter.getDescent(v)))
				Dim x As Integer = alloc.x

				'Add text attributes
				[as].addAttribute(java.awt.font.TextAttribute.FONT, v.font)
				[as].addAttribute(java.awt.font.TextAttribute.FOREGROUND, v.foreground)
				If StyleConstants.isBold(v.attributes) Then [as].addAttribute(java.awt.font.TextAttribute.WEIGHT, java.awt.font.TextAttribute.WEIGHT_BOLD)
				If StyleConstants.isItalic(v.attributes) Then [as].addAttribute(java.awt.font.TextAttribute.POSTURE, java.awt.font.TextAttribute.POSTURE_OBLIQUE)
				If v.underline Then [as].addAttribute(java.awt.font.TextAttribute.UNDERLINE, java.awt.font.TextAttribute.UNDERLINE_ON)
				If v.strikeThrough Then [as].addAttribute(java.awt.font.TextAttribute.STRIKETHROUGH, java.awt.font.TextAttribute.STRIKETHROUGH_ON)
				If v.superscript Then [as].addAttribute(java.awt.font.TextAttribute.SUPERSCRIPT, java.awt.font.TextAttribute.SUPERSCRIPT_SUPER)
				If v.subscript Then [as].addAttribute(java.awt.font.TextAttribute.SUPERSCRIPT, java.awt.font.TextAttribute.SUPERSCRIPT_SUB)

				' draw
				Dim aci As AttributedCharacterIterator = [as].getIterator(Nothing, p0 - start, p1 - start)
				sun.swing.SwingUtilities2.drawString(getJComponent(v), g2d,aci,x,y)
			End If
		End Sub

	'    
	'     * Convenience function for determining ComponentOrientation.  Helps us
	'     * avoid having Munge directives throughout the code.
	'     
		Friend Shared Function isLeftToRight(ByVal c As java.awt.Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function


		''' <summary>
		''' Provides a way to determine the next visually represented model
		''' location that one might place a caret.  Some views may not be visible,
		''' they might not be in the same order found in the model, or they just
		''' might not allow access to some of the locations in the model.
		''' <p>
		''' This implementation assumes the views are layed out in a logical
		''' manner. That is, that the view at index x + 1 is visually after
		''' the View at index x, and that the View at index x - 1 is visually
		''' before the View at x. There is support for reversing this behavior
		''' only if the passed in <code>View</code> is an instance of
		''' <code>CompositeView</code>. The <code>CompositeView</code>
		''' must then override the <code>flipEastAndWestAtEnds</code> method.
		''' </summary>
		''' <param name="v"> View to query </param>
		''' <param name="pos"> the position to convert &gt;= 0 </param>
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
		''' <param name="biasRet"> an array contain the bias that was checked </param>
		''' <returns> the location within the model that best represents the next
		'''  location visual position </returns>
		''' <exception cref="BadLocationException"> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>direction</code> is invalid </exception>
		Friend Shared Function getNextVisualPositionFrom(ByVal v As View, ByVal pos As Integer, ByVal b As Position.Bias, ByVal alloc As java.awt.Shape, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			If v.viewCount = 0 Then Return pos
			Dim top As Boolean = (direction = javax.swing.SwingConstants.NORTH OrElse direction = javax.swing.SwingConstants.WEST)
			Dim retValue As Integer
			If pos = -1 Then
				' Start from the first View.
				Dim childIndex As Integer = If(top, v.viewCount - 1, 0)
				Dim child As View = v.getView(childIndex)
				Dim childBounds As java.awt.Shape = v.getChildAllocation(childIndex, alloc)
				retValue = child.getNextVisualPositionFrom(pos, b, childBounds, direction, biasRet)
				If retValue = -1 AndAlso (Not top) AndAlso v.viewCount > 1 Then
					' Special case that should ONLY happen if first view
					' isn't valid (can happen when end position is put at
					' beginning of line.
					child = v.getView(1)
					childBounds = v.getChildAllocation(1, alloc)
					retValue = child.getNextVisualPositionFrom(-1, biasRet(0), childBounds, direction, biasRet)
				End If
			Else
				Dim increment As Integer = If(top, -1, 1)
				Dim childIndex As Integer
				If b Is Position.Bias.Backward AndAlso pos > 0 Then
					childIndex = v.getViewIndex(pos - 1, Position.Bias.Forward)
				Else
					childIndex = v.getViewIndex(pos, Position.Bias.Forward)
				End If
				Dim child As View = v.getView(childIndex)
				Dim childBounds As java.awt.Shape = v.getChildAllocation(childIndex, alloc)
				retValue = child.getNextVisualPositionFrom(pos, b, childBounds, direction, biasRet)
				If (direction = javax.swing.SwingConstants.EAST OrElse direction = javax.swing.SwingConstants.WEST) AndAlso (TypeOf v Is CompositeView) AndAlso CType(v, CompositeView).flipEastAndWestAtEnds(pos, b) Then increment *= -1
				childIndex += increment
				If retValue = -1 AndAlso childIndex >= 0 AndAlso childIndex < v.viewCount Then
					child = v.getView(childIndex)
					childBounds = v.getChildAllocation(childIndex, alloc)
					retValue = child.getNextVisualPositionFrom(-1, b, childBounds, direction, biasRet)
					' If there is a bias change, it is a fake position
					' and we should skip it. This is usually the result
					' of two elements side be side flowing the same way.
					If retValue = pos AndAlso biasRet(0) IsNot b Then Return getNextVisualPositionFrom(v, pos, biasRet(0), alloc, direction, biasRet)
				ElseIf retValue <> -1 AndAlso biasRet(0) IsNot b AndAlso ((increment = 1 AndAlso child.endOffset = retValue) OrElse (increment = -1 AndAlso child.startOffset = retValue)) AndAlso childIndex >= 0 AndAlso childIndex < v.viewCount Then
					' Reached the end of a view, make sure the next view
					' is a different direction.
					child = v.getView(childIndex)
					childBounds = v.getChildAllocation(childIndex, alloc)
					Dim originalBias As Position.Bias = biasRet(0)
					Dim nextPos As Integer = child.getNextVisualPositionFrom(-1, b, childBounds, direction, biasRet)
					If biasRet(0) Is b Then
						retValue = nextPos
					Else
						biasRet(0) = originalBias
					End If
				End If
			End If
			Return retValue
		End Function
	End Class

End Namespace