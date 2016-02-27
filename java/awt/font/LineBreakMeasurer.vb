Imports System

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

'
' * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
' *
' * The original version of this source code and documentation is
' * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
' * of IBM. These materials are provided under terms of a License
' * Agreement between Taligent and Sun. This technology is protected
' * by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.awt.font


	''' <summary>
	''' The <code>LineBreakMeasurer</code> class allows styled text to be
	''' broken into lines (or segments) that fit within a particular visual
	''' advance.  This is useful for clients who wish to display a paragraph of
	''' text that fits within a specific width, called the <b>wrapping
	''' width</b>.
	''' <p>
	''' <code>LineBreakMeasurer</code> is constructed with an iterator over
	''' styled text.  The iterator's range should be a single paragraph in the
	''' text.
	''' <code>LineBreakMeasurer</code> maintains a position in the text for the
	''' start of the next text segment.  Initially, this position is the
	''' start of text.  Paragraphs are assigned an overall direction (either
	''' left-to-right or right-to-left) according to the bidirectional
	''' formatting rules.  All segments obtained from a paragraph have the
	''' same direction as the paragraph.
	''' <p>
	''' Segments of text are obtained by calling the method
	''' <code>nextLayout</code>, which returns a <seealso cref="TextLayout"/>
	''' representing the text that fits within the wrapping width.
	''' The <code>nextLayout</code> method moves the current position
	''' to the end of the layout returned from <code>nextLayout</code>.
	''' <p>
	''' <code>LineBreakMeasurer</code> implements the most commonly used
	''' line-breaking policy: Every word that fits within the wrapping
	''' width is placed on the line. If the first word does not fit, then all
	''' of the characters that fit within the wrapping width are placed on the
	''' line.  At least one character is placed on each line.
	''' <p>
	''' The <code>TextLayout</code> instances returned by
	''' <code>LineBreakMeasurer</code> treat tabs like 0-width spaces.  Clients
	''' who wish to obtain tab-delimited segments for positioning should use
	''' the overload of <code>nextLayout</code> which takes a limiting offset
	''' in the text.
	''' The limiting offset should be the first character after the tab.
	''' The <code>TextLayout</code> objects returned from this method end
	''' at the limit provided (or before, if the text between the current
	''' position and the limit won't fit entirely within the  wrapping
	''' width).
	''' <p>
	''' Clients who are laying out tab-delimited text need a slightly
	''' different line-breaking policy after the first segment has been
	''' placed on a line.  Instead of fitting partial words in the
	''' remaining space, they should place words which don't fit in the
	''' remaining space entirely on the next line.  This change of policy
	''' can be requested in the overload of <code>nextLayout</code> which
	''' takes a <code>boolean</code> parameter.  If this parameter is
	''' <code>true</code>, <code>nextLayout</code> returns
	''' <code>null</code> if the first word won't fit in
	''' the given space.  See the tab sample below.
	''' <p>
	''' In general, if the text used to construct the
	''' <code>LineBreakMeasurer</code> changes, a new
	''' <code>LineBreakMeasurer</code> must be constructed to reflect
	''' the change.  (The old <code>LineBreakMeasurer</code> continues to
	''' function properly, but it won't be aware of the text change.)
	''' Nevertheless, if the text change is the insertion or deletion of a
	''' single character, an existing <code>LineBreakMeasurer</code> can be
	''' 'updated' by calling <code>insertChar</code> or
	''' <code>deleteChar</code>. Updating an existing
	''' <code>LineBreakMeasurer</code> is much faster than creating a new one.
	''' Clients who modify text based on user typing should take advantage
	''' of these methods.
	''' <p>
	''' <strong>Examples</strong>:<p>
	''' Rendering a paragraph in a component
	''' <blockquote>
	''' <pre>{@code
	''' public void paint(Graphics graphics) {
	''' 
	'''     Point2D pen = new Point2D(10, 20);
	'''     Graphics2D g2d = (Graphics2D)graphics;
	'''     FontRenderContext frc = g2d.getFontRenderContext();
	''' 
	'''     // let styledText be an AttributedCharacterIterator containing at least
	'''     // one character
	''' 
	'''     LineBreakMeasurer measurer = new LineBreakMeasurer(styledText, frc);
	'''     float wrappingWidth = getSize().width - 15;
	''' 
	'''     while (measurer.getPosition() < fStyledText.length()) {
	''' 
	'''         TextLayout layout = measurer.nextLayout(wrappingWidth);
	''' 
	'''         pen.y += (layout.getAscent());
	'''         float dx = layout.isLeftToRight() ?
	'''             0 : (wrappingWidth - layout.getAdvance());
	''' 
	'''         layout.draw(graphics, pen.x + dx, pen.y);
	'''         pen.y += layout.getDescent() + layout.getLeading();
	'''     }
	''' }
	''' }</pre>
	''' </blockquote>
	''' <p>
	''' Rendering text with tabs.  For simplicity, the overall text
	''' direction is assumed to be left-to-right
	''' <blockquote>
	''' <pre>{@code
	''' public void paint(Graphics graphics) {
	''' 
	'''     float leftMargin = 10, rightMargin = 310;
	'''     float[] tabStops = { 100, 250 };
	''' 
	'''     // assume styledText is an AttributedCharacterIterator, and the number
	'''     // of tabs in styledText is tabCount
	''' 
	'''     int[] tabLocations = new int[tabCount+1];
	''' 
	'''     int i = 0;
	'''     for (char c = styledText.first(); c != styledText.DONE; c = styledText.next()) {
	'''         if (c == '\t') {
	'''             tabLocations[i++] = styledText.getIndex();
	'''         }
	'''     }
	'''     tabLocations[tabCount] = styledText.getEndIndex() - 1;
	''' 
	'''     // Now tabLocations has an entry for every tab's offset in
	'''     // the text.  For convenience, the last entry is tabLocations
	'''     // is the offset of the last character in the text.
	''' 
	'''     LineBreakMeasurer measurer = new LineBreakMeasurer(styledText);
	'''     int currentTab = 0;
	'''     float verticalPos = 20;
	''' 
	'''     while (measurer.getPosition() < styledText.getEndIndex()) {
	''' 
	'''         // Lay out and draw each line.  All segments on a line
	'''         // must be computed before any drawing can occur, since
	'''         // we must know the largest ascent on the line.
	'''         // TextLayouts are computed and stored in a Vector;
	'''         // their horizontal positions are stored in a parallel
	'''         // Vector.
	''' 
	'''         // lineContainsText is true after first segment is drawn
	'''         boolean lineContainsText = false;
	'''         boolean lineComplete = false;
	'''         float maxAscent = 0, maxDescent = 0;
	'''         float horizontalPos = leftMargin;
	'''         Vector layouts = new Vector(1);
	'''         Vector penPositions = new Vector(1);
	''' 
	'''         while (!lineComplete) {
	'''             float wrappingWidth = rightMargin - horizontalPos;
	'''             TextLayout layout =
	'''                     measurer.nextLayout(wrappingWidth,
	'''                                         tabLocations[currentTab]+1,
	'''                                         lineContainsText);
	''' 
	'''             // layout can be null if lineContainsText is true
	'''             if (layout != null) {
	'''                 layouts.addElement(layout);
	'''                 penPositions.addElement(new Float(horizontalPos));
	'''                 horizontalPos += layout.getAdvance();
	'''                 maxAscent = System.Math.max(maxAscent, layout.getAscent());
	'''                 maxDescent = System.Math.max(maxDescent,
	'''                     layout.getDescent() + layout.getLeading());
	'''             } else {
	'''                 lineComplete = true;
	'''             }
	''' 
	'''             lineContainsText = true;
	''' 
	'''             if (measurer.getPosition() == tabLocations[currentTab]+1) {
	'''                 currentTab++;
	'''             }
	''' 
	'''             if (measurer.getPosition() == styledText.getEndIndex())
	'''                 lineComplete = true;
	'''             else if (horizontalPos >= tabStops[tabStops.length-1])
	'''                 lineComplete = true;
	''' 
	'''             if (!lineComplete) {
	'''                 // move to next tab stop
	'''                 int j;
	'''                 for (j=0; horizontalPos >= tabStops[j]; j++) {}
	'''                 horizontalPos = tabStops[j];
	'''             }
	'''         }
	''' 
	'''         verticalPos += maxAscent;
	''' 
	'''         Enumeration layoutEnum = layouts.elements();
	'''         Enumeration positionEnum = penPositions.elements();
	''' 
	'''         // now iterate through layouts and draw them
	'''         while (layoutEnum.hasMoreElements()) {
	'''             TextLayout nextLayout = (TextLayout) layoutEnum.nextElement();
	'''             Float nextPosition = (Float) positionEnum.nextElement();
	'''             nextLayout.draw(graphics, nextPosition.floatValue(), verticalPos);
	'''         }
	''' 
	'''         verticalPos += maxDescent;
	'''     }
	''' }
	''' }</pre>
	''' </blockquote> </summary>
	''' <seealso cref= TextLayout </seealso>

	Public NotInheritable Class LineBreakMeasurer

		Private breakIter As java.text.BreakIterator
		Private start As Integer
		Private pos As Integer
		Private limit As Integer
		Private measurer As TextMeasurer
		Private charIter As CharArrayIterator

		''' <summary>
		''' Constructs a <code>LineBreakMeasurer</code> for the specified text.
		''' </summary>
		''' <param name="text"> the text for which this <code>LineBreakMeasurer</code>
		'''       produces <code>TextLayout</code> objects; the text must contain
		'''       at least one character; if the text available through
		'''       <code>iter</code> changes, further calls to this
		'''       <code>LineBreakMeasurer</code> instance are undefined (except,
		'''       in some cases, when <code>insertChar</code> or
		'''       <code>deleteChar</code> are invoked afterward - see below) </param>
		''' <param name="frc"> contains information about a graphics device which is
		'''       needed to measure the text correctly;
		'''       text measurements can vary slightly depending on the
		'''       device resolution, and attributes such as antialiasing; this
		'''       parameter does not specify a translation between the
		'''       <code>LineBreakMeasurer</code> and user space </param>
		''' <seealso cref= LineBreakMeasurer#insertChar </seealso>
		''' <seealso cref= LineBreakMeasurer#deleteChar </seealso>
		Public Sub New(ByVal text As java.text.AttributedCharacterIterator, ByVal frc As java.awt.font.FontRenderContext)
			Me.New(text, java.text.BreakIterator.lineInstance, frc)
		End Sub

		''' <summary>
		''' Constructs a <code>LineBreakMeasurer</code> for the specified text.
		''' </summary>
		''' <param name="text"> the text for which this <code>LineBreakMeasurer</code>
		'''     produces <code>TextLayout</code> objects; the text must contain
		'''     at least one character; if the text available through
		'''     <code>iter</code> changes, further calls to this
		'''     <code>LineBreakMeasurer</code> instance are undefined (except,
		'''     in some cases, when <code>insertChar</code> or
		'''     <code>deleteChar</code> are invoked afterward - see below) </param>
		''' <param name="breakIter"> the <seealso cref="BreakIterator"/> which defines line
		'''     breaks </param>
		''' <param name="frc"> contains information about a graphics device which is
		'''       needed to measure the text correctly;
		'''       text measurements can vary slightly depending on the
		'''       device resolution, and attributes such as antialiasing; this
		'''       parameter does not specify a translation between the
		'''       <code>LineBreakMeasurer</code> and user space </param>
		''' <exception cref="IllegalArgumentException"> if the text has less than one character </exception>
		''' <seealso cref= LineBreakMeasurer#insertChar </seealso>
		''' <seealso cref= LineBreakMeasurer#deleteChar </seealso>
		Public Sub New(ByVal text As java.text.AttributedCharacterIterator, ByVal breakIter As java.text.BreakIterator, ByVal frc As java.awt.font.FontRenderContext)
			If text.endIndex - text.beginIndex < 1 Then Throw New IllegalArgumentException("Text must contain at least one character.")

			Me.breakIter = breakIter
			Me.measurer = New TextMeasurer(text, frc)
			Me.limit = text.endIndex
				Me.start = text.beginIndex
				Me.pos = Me.start

			charIter = New CharArrayIterator(measurer.chars, Me.start)
			Me.breakIter.text = charIter
		End Sub

		''' <summary>
		''' Returns the position at the end of the next layout.  Does NOT
		''' update the current position of this <code>LineBreakMeasurer</code>.
		''' </summary>
		''' <param name="wrappingWidth"> the maximum visible advance permitted for
		'''    the text in the next layout </param>
		''' <returns> an offset in the text representing the limit of the
		'''    next <code>TextLayout</code>. </returns>
		Public Function nextOffset(ByVal wrappingWidth As Single) As Integer
			Return nextOffset(wrappingWidth, limit, False)
		End Function

		''' <summary>
		''' Returns the position at the end of the next layout.  Does NOT
		''' update the current position of this <code>LineBreakMeasurer</code>.
		''' </summary>
		''' <param name="wrappingWidth"> the maximum visible advance permitted for
		'''    the text in the next layout </param>
		''' <param name="offsetLimit"> the first character that can not be included
		'''    in the next layout, even if the text after the limit would fit
		'''    within the wrapping width; <code>offsetLimit</code> must be
		'''    greater than the current position </param>
		''' <param name="requireNextWord"> if <code>true</code>, the current position
		'''    that is returned if the entire next word does not fit within
		'''    <code>wrappingWidth</code>; if <code>false</code>, the offset
		'''    returned is at least one greater than the current position </param>
		''' <returns> an offset in the text representing the limit of the
		'''    next <code>TextLayout</code> </returns>
		Public Function nextOffset(ByVal wrappingWidth As Single, ByVal offsetLimit As Integer, ByVal requireNextWord As Boolean) As Integer

			Dim nextOffset_Renamed As Integer = pos

			If pos < limit Then
				If offsetLimit <= pos Then Throw New IllegalArgumentException("offsetLimit must be after current position")

				Dim charAtMaxAdvance As Integer = measurer.getLineBreakIndex(pos, wrappingWidth)

				If charAtMaxAdvance = limit Then
					nextOffset_Renamed = limit
				ElseIf Char.IsWhiteSpace(measurer.chars(charAtMaxAdvance-start)) Then
					nextOffset_Renamed = breakIter.following(charAtMaxAdvance)
				Else
				' Break is in a word;  back up to previous break.

					' NOTE:  I think that breakIter.preceding(limit) should be
					' equivalent to breakIter.last(), breakIter.previous() but
					' the authors of BreakIterator thought otherwise...
					' If they were equivalent then the first branch would be
					' unnecessary.
					Dim testPos As Integer = charAtMaxAdvance + 1
					If testPos = limit Then
						breakIter.last()
						nextOffset_Renamed = breakIter.previous()
					Else
						nextOffset_Renamed = breakIter.preceding(testPos)
					End If

					If nextOffset_Renamed <= pos Then
						' first word doesn't fit on line
						If requireNextWord Then
							nextOffset_Renamed = pos
						Else
							nextOffset_Renamed = System.Math.Max(pos+1, charAtMaxAdvance)
						End If
					End If
				End If
			End If

			If nextOffset_Renamed > offsetLimit Then nextOffset_Renamed = offsetLimit

			Return nextOffset_Renamed
		End Function

		''' <summary>
		''' Returns the next layout, and updates the current position.
		''' </summary>
		''' <param name="wrappingWidth"> the maximum visible advance permitted for
		'''     the text in the next layout </param>
		''' <returns> a <code>TextLayout</code>, beginning at the current
		'''     position, which represents the next line fitting within
		'''     <code>wrappingWidth</code> </returns>
		Public Function nextLayout(ByVal wrappingWidth As Single) As TextLayout
			Return nextLayout(wrappingWidth, limit, False)
		End Function

		''' <summary>
		''' Returns the next layout, and updates the current position.
		''' </summary>
		''' <param name="wrappingWidth"> the maximum visible advance permitted
		'''    for the text in the next layout </param>
		''' <param name="offsetLimit"> the first character that can not be
		'''    included in the next layout, even if the text after the limit
		'''    would fit within the wrapping width; <code>offsetLimit</code>
		'''    must be greater than the current position </param>
		''' <param name="requireNextWord"> if <code>true</code>, and if the entire word
		'''    at the current position does not fit within the wrapping width,
		'''    <code>null</code> is returned. If <code>false</code>, a valid
		'''    layout is returned that includes at least the character at the
		'''    current position </param>
		''' <returns> a <code>TextLayout</code>, beginning at the current
		'''    position, that represents the next line fitting within
		'''    <code>wrappingWidth</code>.  If the current position is at the end
		'''    of the text used by this <code>LineBreakMeasurer</code>,
		'''    <code>null</code> is returned </returns>
		Public Function nextLayout(ByVal wrappingWidth As Single, ByVal offsetLimit As Integer, ByVal requireNextWord As Boolean) As TextLayout

			If pos < limit Then
				Dim layoutLimit As Integer = nextOffset(wrappingWidth, offsetLimit, requireNextWord)
				If layoutLimit = pos Then Return Nothing

				Dim result As TextLayout = measurer.getLayout(pos, layoutLimit)
				pos = layoutLimit

				Return result
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the current position of this <code>LineBreakMeasurer</code>.
		''' </summary>
		''' <returns> the current position of this <code>LineBreakMeasurer</code> </returns>
		''' <seealso cref= #setPosition </seealso>
		Public Property position As Integer
			Get
				Return pos
			End Get
			Set(ByVal newPosition As Integer)
				If newPosition < start OrElse newPosition > limit Then Throw New IllegalArgumentException("position is out of range")
				pos = newPosition
			End Set
		End Property


		''' <summary>
		''' Updates this <code>LineBreakMeasurer</code> after a single
		''' character is inserted into the text, and sets the current
		''' position to the beginning of the paragraph.
		''' </summary>
		''' <param name="newParagraph"> the text after the insertion </param>
		''' <param name="insertPos"> the position in the text at which the character
		'''    is inserted </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>insertPos</code> is less
		'''         than the start of <code>newParagraph</code> or greater than
		'''         or equal to the end of <code>newParagraph</code> </exception>
		''' <exception cref="NullPointerException"> if <code>newParagraph</code> is
		'''         <code>null</code> </exception>
		''' <seealso cref= #deleteChar </seealso>
		Public Sub insertChar(ByVal newParagraph As java.text.AttributedCharacterIterator, ByVal insertPos As Integer)

			measurer.insertChar(newParagraph, insertPos)

			limit = newParagraph.endIndex
				start = newParagraph.beginIndex
				pos = start

			charIter.reset(measurer.chars, newParagraph.beginIndex)
			breakIter.text = charIter
		End Sub

		''' <summary>
		''' Updates this <code>LineBreakMeasurer</code> after a single
		''' character is deleted from the text, and sets the current
		''' position to the beginning of the paragraph. </summary>
		''' <param name="newParagraph"> the text after the deletion </param>
		''' <param name="deletePos"> the position in the text at which the character
		'''    is deleted </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>deletePos</code> is
		'''         less than the start of <code>newParagraph</code> or greater
		'''         than the end of <code>newParagraph</code> </exception>
		''' <exception cref="NullPointerException"> if <code>newParagraph</code> is
		'''         <code>null</code> </exception>
		''' <seealso cref= #insertChar </seealso>
		Public Sub deleteChar(ByVal newParagraph As java.text.AttributedCharacterIterator, ByVal deletePos As Integer)

			measurer.deleteChar(newParagraph, deletePos)

			limit = newParagraph.endIndex
				start = newParagraph.beginIndex
				pos = start

			charIter.reset(measurer.chars, start)
			breakIter.text = charIter
		End Sub
	End Class

End Namespace