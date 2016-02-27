Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>TextMeasurer</code> class provides the primitive operations
	''' needed for line break: measuring up to a given advance, determining the
	''' advance of a range of characters, and generating a
	''' <code>TextLayout</code> for a range of characters. It also provides
	''' methods for incremental editing of paragraphs.
	''' <p>
	''' A <code>TextMeasurer</code> object is constructed with an
	''' <seealso cref="java.text.AttributedCharacterIterator AttributedCharacterIterator"/>
	''' representing a single paragraph of text.  The value returned by the
	''' <seealso cref="AttributedCharacterIterator#getBeginIndex() getBeginIndex"/>
	''' method of <code>AttributedCharacterIterator</code>
	''' defines the absolute index of the first character.  The value
	''' returned by the
	''' <seealso cref="AttributedCharacterIterator#getEndIndex() getEndIndex"/>
	''' method of <code>AttributedCharacterIterator</code> defines the index
	''' past the last character.  These values define the range of indexes to
	''' use in calls to the <code>TextMeasurer</code>.  For example, calls to
	''' get the advance of a range of text or the line break of a range of text
	''' must use indexes between the beginning and end index values.  Calls to
	''' <seealso cref="#insertChar(java.text.AttributedCharacterIterator, int) insertChar"/>
	''' and
	''' <seealso cref="#deleteChar(java.text.AttributedCharacterIterator, int) deleteChar"/>
	''' reset the <code>TextMeasurer</code> to use the beginning index and end
	''' index of the <code>AttributedCharacterIterator</code> passed in those calls.
	''' <p>
	''' Most clients will use the more convenient <code>LineBreakMeasurer</code>,
	''' which implements the standard line break policy (placing as many words
	''' as will fit on each line).
	''' 
	''' @author John Raley </summary>
	''' <seealso cref= LineBreakMeasurer
	''' @since 1.3 </seealso>

	Public NotInheritable Class TextMeasurer
		Implements Cloneable

		' Number of lines to format to.
		Private Shared EST_LINES As Single = CSng(2.1)

	'    
	'    static {
	'        String s = System.getProperty("estLines");
	'        if (s != null) {
	'            try {
	'                Float f = new Float(s);
	'                EST_LINES = f.floatValue();
	'            }
	'            catch(NumberFormatException e) {
	'            }
	'        }
	'        //System.out.println("EST_LINES="+EST_LINES);
	'    }
	'    

		Private fFrc As java.awt.font.FontRenderContext

		Private fStart As Integer

		' characters in source text
		Private fChars As Char()

		' Bidi for this paragraph
		Private fBidi As java.text.Bidi

		' Levels array for chars in this paragraph - needed to reorder
		' trailing counterdirectional whitespace
		Private fLevels As SByte()

		' line components in logical order
		Private fComponents As sun.font.TextLineComponent()

		' index where components begin
		Private fComponentStart As Integer

		' index where components end
		Private fComponentLimit As Integer

		Private haveLayoutWindow As Boolean

		' used to find valid starting points for line components
		Private fLineBreak As java.text.BreakIterator = Nothing
		Private charIter As CharArrayIterator = Nothing
		Friend layoutCount As Integer = 0
		Friend layoutCharCount As Integer = 0

		' paragraph, with resolved fonts and styles
		Private fParagraph As StyledParagraph

		' paragraph data - same across all layouts
		Private fIsDirectionLTR As Boolean
		Private fBaseline As SByte
		Private fBaselineOffsets As Single()
		Private fJustifyRatio As Single = 1

		''' <summary>
		''' Constructs a <code>TextMeasurer</code> from the source text.
		''' The source text should be a single entire paragraph. </summary>
		''' <param name="text"> the source paragraph.  Cannot be null. </param>
		''' <param name="frc"> the information about a graphics device which is needed
		'''       to measure the text correctly.  Cannot be null. </param>
		Public Sub New(ByVal text As java.text.AttributedCharacterIterator, ByVal frc As java.awt.font.FontRenderContext)

			fFrc = frc
			initAll(text)
		End Sub

		Protected Friend Function clone() As Object
			Dim other As TextMeasurer
			Try
				other = CType(MyBase.clone(), TextMeasurer)
			Catch e As CloneNotSupportedException
				Throw New [Error]()
			End Try
			If fComponents IsNot Nothing Then other.fComponents = fComponents.clone()
			Return other
		End Function

		Private Sub invalidateComponents()
				fComponentLimit = fChars.Length
				fComponentStart = fComponentLimit
			fComponents = Nothing
			haveLayoutWindow = False
		End Sub

		''' <summary>
		''' Initialize state, including fChars array, direction, and
		''' fBidi.
		''' </summary>
		Private Sub initAll(ByVal text As java.text.AttributedCharacterIterator)

			fStart = text.beginIndex

			' extract chars
			fChars = New Char(text.endIndex - fStart - 1){}

			Dim n As Integer = 0
			Dim c As Char = text.first()
			Do While c <> java.text.CharacterIterator.DONE
				fChars(n) = c
				n += 1
				c = text.next()
			Loop

			text.first()

			fBidi = New java.text.Bidi(text)
			If fBidi.leftToRight Then fBidi = Nothing

			text.first()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim paragraphAttrs As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = text.attributes
			Dim shaper As NumericShaper = sun.font.AttributeValues.getNumericShaping(paragraphAttrs)
			If shaper IsNot Nothing Then shaper.shape(fChars, 0, fChars.Length)

			fParagraph = New StyledParagraph(text, fChars)

			' set paragraph attributes
				' If there's an embedded graphic at the start of the
				' paragraph, look for the first non-graphic character
				' and use it and its font to initialize the paragraph.
				' If not, use the first graphic to initialize.
				fJustifyRatio = sun.font.AttributeValues.getJustification(paragraphAttrs)

				Dim haveFont As Boolean = TextLine.advanceToFirstFont(text)

				If haveFont Then
					Dim defaultFont As java.awt.Font = TextLine.getFontAtCurrentPos(text)
					Dim charsStart As Integer = text.index - text.beginIndex
					Dim lm As LineMetrics = defaultFont.getLineMetrics(fChars, charsStart, charsStart+1, fFrc)
					fBaseline = CByte(lm.baselineIndex)
					fBaselineOffsets = lm.baselineOffsets
				Else
					' hmmm what to do here?  Just try to supply reasonable
					' values I guess.

					Dim graphic As GraphicAttribute = CType(paragraphAttrs(TextAttribute.CHAR_REPLACEMENT), GraphicAttribute)
					fBaseline = TextLayout.getBaselineFromGraphic(graphic)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim fmap As New Dictionary(Of java.text.AttributedCharacterIterator.Attribute, ?)(5, CSng(0.9))
					Dim dummyFont As New java.awt.Font(fmap)
					Dim lm As LineMetrics = dummyFont.getLineMetrics(" ", 0, 1, fFrc)
					fBaselineOffsets = lm.baselineOffsets
				End If
				fBaselineOffsets = TextLine.getNormalizedOffsets(fBaselineOffsets, fBaseline)

			invalidateComponents()
		End Sub

		''' <summary>
		''' Generate components for the paragraph.  fChars, fBidi should have been
		''' initialized already.
		''' </summary>
		Private Sub generateComponents(ByVal startingAt As Integer, ByVal endingAt As Integer)

			If collectStats Then formattedChars += (endingAt-startingAt)
			Dim layoutFlags As Integer = 0 ' no extra info yet, bidi determines run and line direction
			Dim factory As New sun.font.TextLabelFactory(fFrc, fChars, fBidi, layoutFlags)

			Dim charsLtoV As Integer() = Nothing

			If fBidi IsNot Nothing Then
				fLevels = sun.font.BidiUtils.getLevels(fBidi)
				Dim charsVtoL As Integer() = sun.font.BidiUtils.createVisualToLogicalMap(fLevels)
				charsLtoV = sun.font.BidiUtils.createInverseMap(charsVtoL)
				fIsDirectionLTR = fBidi.baseIsLeftToRight()
			Else
				fLevels = Nothing
				fIsDirectionLTR = True
			End If

			Try
				fComponents = TextLine.getComponents(fParagraph, fChars, startingAt, endingAt, charsLtoV, fLevels, factory)
			Catch e As IllegalArgumentException
				Console.WriteLine("startingAt=" & startingAt & "; endingAt=" & endingAt)
				Console.WriteLine("fComponentLimit=" & fComponentLimit)
				Throw e
			End Try

			fComponentStart = startingAt
			fComponentLimit = endingAt
			'debugFormatCount += (endingAt-startingAt);
		End Sub

		Private Function calcLineBreak(ByVal pos As Integer, ByVal maxAdvance As Single) As Integer

			' either of these statements removes the bug:
			'generateComponents(0, fChars.length);
			'generateComponents(pos, fChars.length);

			Dim startPos As Integer = pos
			Dim width As Single = maxAdvance

			Dim tlcIndex As Integer
			Dim tlcStart As Integer = fComponentStart

			For tlcIndex = 0 To fComponents.Length - 1
				Dim gaLimit As Integer = tlcStart + fComponents(tlcIndex).numCharacters
				If gaLimit > startPos Then
					Exit For
				Else
					tlcStart = gaLimit
				End If
			Next tlcIndex

			' tlcStart is now the start of the tlc at tlcIndex

			Do While tlcIndex < fComponents.Length

				Dim tlc As sun.font.TextLineComponent = fComponents(tlcIndex)
				Dim numCharsInGa As Integer = tlc.numCharacters

				Dim lineBreak As Integer = tlc.getLineBreakIndex(startPos - tlcStart, width)
				If lineBreak = numCharsInGa AndAlso tlcIndex < fComponents.Length Then
					width -= tlc.getAdvanceBetween(startPos - tlcStart, lineBreak)
					tlcStart += numCharsInGa
					startPos = tlcStart
				Else
					Return tlcStart + lineBreak
				End If
				tlcIndex += 1
			Loop

			If fComponentLimit < fChars.Length Then
				' format more text and try again
				'if (haveLayoutWindow) {
				'    outOfWindow++;
				'}

				generateComponents(pos, fChars.Length)
				Return calcLineBreak(pos, maxAdvance)
			End If

			Return fChars.Length
		End Function

		''' <summary>
		''' According to the Unicode Bidirectional Behavior specification
		''' (Unicode Standard 2.0, section 3.11), whitespace at the ends
		''' of lines which would naturally flow against the base direction
		''' must be made to flow with the line direction, and moved to the
		''' end of the line.  This method returns the start of the sequence
		''' of trailing whitespace characters to move to the end of a
		''' line taken from the given range.
		''' </summary>
		Private Function trailingCdWhitespaceStart(ByVal startPos As Integer, ByVal limitPos As Integer) As Integer

			If fLevels IsNot Nothing Then
				' Back up over counterdirectional whitespace
				Dim baseLevel As SByte = CByte(If(fIsDirectionLTR, 0, 1))
				Dim cdWsStart As Integer = limitPos
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While cdWsStart -= 1 >= startPos
					If (fLevels(cdWsStart) Mod 2) = baseLevel OrElse Character.getDirectionality(fChars(cdWsStart)) <> Character.DIRECTIONALITY_WHITESPACE Then
							cdWsStart += 1
							Return cdWsStart
					End If
				Loop
			End If

			Return startPos
		End Function

		Private Function makeComponentsOnRange(ByVal startPos As Integer, ByVal limitPos As Integer) As sun.font.TextLineComponent()

			' sigh I really hate to do this here since it's part of the
			' bidi algorithm.
			' cdWsStart is the start of the trailing counterdirectional
			' whitespace
			Dim cdWsStart As Integer = trailingCdWhitespaceStart(startPos, limitPos)

			Dim tlcIndex As Integer
			Dim tlcStart As Integer = fComponentStart

			For tlcIndex = 0 To fComponents.Length - 1
				Dim gaLimit As Integer = tlcStart + fComponents(tlcIndex).numCharacters
				If gaLimit > startPos Then
					Exit For
				Else
					tlcStart = gaLimit
				End If
			Next tlcIndex

			' tlcStart is now the start of the tlc at tlcIndex

			Dim componentCount As Integer
				Dim Split As Boolean = False
				Dim compStart As Integer = tlcStart
				Dim lim As Integer=tlcIndex
				Dim cont As Boolean=True
				Do While cont
					Dim gaLimit As Integer = compStart + fComponents(lim).numCharacters
					If cdWsStart > System.Math.Max(compStart, startPos) AndAlso cdWsStart < System.Math.Min(gaLimit, limitPos) Then Split = True
					If gaLimit >= limitPos Then
						cont=False
					Else
						compStart = gaLimit
					End If
					lim += 1
				Loop
				componentCount = lim-tlcIndex
				If Split Then componentCount += 1

			Dim components As sun.font.TextLineComponent() = New sun.font.TextLineComponent(componentCount - 1){}
			Dim newCompIndex As Integer = 0
			Dim linePos As Integer = startPos

			Dim breakPt As Integer = cdWsStart

			Dim subsetFlag As Integer
			If breakPt = startPos Then
				subsetFlag = If(fIsDirectionLTR, sun.font.TextLineComponent.LEFT_TO_RIGHT, sun.font.TextLineComponent.RIGHT_TO_LEFT)
				breakPt = limitPos
			Else
				subsetFlag = sun.font.TextLineComponent.UNCHANGED
			End If

			Do While linePos < limitPos

				Dim compLength As Integer = fComponents(tlcIndex).numCharacters
				Dim tlcLimit As Integer = tlcStart + compLength

				Dim start As Integer = System.Math.Max(linePos, tlcStart)
				Dim limit As Integer = System.Math.Min(breakPt, tlcLimit)

				components(newCompIndex) = fComponents(tlcIndex).getSubset(start-tlcStart, limit-tlcStart, subsetFlag)
				newCompIndex += 1
				linePos += (limit-start)
				If linePos = breakPt Then
					breakPt = limitPos
					subsetFlag = If(fIsDirectionLTR, sun.font.TextLineComponent.LEFT_TO_RIGHT, sun.font.TextLineComponent.RIGHT_TO_LEFT)
				End If
				If linePos = tlcLimit Then
					tlcIndex += 1
					tlcStart = tlcLimit
				End If
			Loop

			Return components
		End Function

		Private Function makeTextLineOnRange(ByVal startPos As Integer, ByVal limitPos As Integer) As TextLine

			Dim charsLtoV As Integer() = Nothing
			Dim charLevels As SByte() = Nothing

			If fBidi IsNot Nothing Then
				Dim lineBidi As java.text.Bidi = fBidi.createLineBidi(startPos, limitPos)
				charLevels = sun.font.BidiUtils.getLevels(lineBidi)
				Dim charsVtoL As Integer() = sun.font.BidiUtils.createVisualToLogicalMap(charLevels)
				charsLtoV = sun.font.BidiUtils.createInverseMap(charsVtoL)
			End If

			Dim components As sun.font.TextLineComponent() = makeComponentsOnRange(startPos, limitPos)

			Return New TextLine(fFrc, components, fBaselineOffsets, fChars, startPos, limitPos, charsLtoV, charLevels, fIsDirectionLTR)

		End Function

		Private Sub ensureComponents(ByVal start As Integer, ByVal limit As Integer)

			If start < fComponentStart OrElse limit > fComponentLimit Then generateComponents(start, limit)
		End Sub

		Private Sub makeLayoutWindow(ByVal localStart As Integer)

			Dim compStart As Integer = localStart
			Dim compLimit As Integer = fChars.Length

			' If we've already gone past the layout window, format to end of paragraph
			If layoutCount > 0 AndAlso (Not haveLayoutWindow) Then
				Dim avgLineLength As Single = System.Math.Max(layoutCharCount \ layoutCount, 1)
				compLimit = System.Math.Min(localStart + CInt(Fix(avgLineLength*EST_LINES)), fChars.Length)
			End If

			If localStart > 0 OrElse compLimit < fChars.Length Then
				If charIter Is Nothing Then
					charIter = New CharArrayIterator(fChars)
				Else
					charIter.reset(fChars)
				End If
				If fLineBreak Is Nothing Then fLineBreak = java.text.BreakIterator.lineInstance
				fLineBreak.text = charIter
				If localStart > 0 Then
					If Not fLineBreak.isBoundary(localStart) Then compStart = fLineBreak.preceding(localStart)
				End If
				If compLimit < fChars.Length Then
					If Not fLineBreak.isBoundary(compLimit) Then compLimit = fLineBreak.following(compLimit)
				End If
			End If

			ensureComponents(compStart, compLimit)
			haveLayoutWindow = True
		End Sub

		''' <summary>
		''' Returns the index of the first character which will not fit on
		''' on a line beginning at <code>start</code> and possible
		''' measuring up to <code>maxAdvance</code> in graphical width.
		''' </summary>
		''' <param name="start"> the character index at which to start measuring.
		'''  <code>start</code> is an absolute index, not relative to the
		'''  start of the paragraph </param>
		''' <param name="maxAdvance"> the graphical width in which the line must fit </param>
		''' <returns> the index after the last character that will fit
		'''  on a line beginning at <code>start</code>, which is not longer
		'''  than <code>maxAdvance</code> in graphical width </returns>
		''' <exception cref="IllegalArgumentException"> if <code>start</code> is
		'''          less than the beginning of the paragraph. </exception>
		Public Function getLineBreakIndex(ByVal start As Integer, ByVal maxAdvance As Single) As Integer

			Dim localStart As Integer = start - fStart

			If (Not haveLayoutWindow) OrElse localStart < fComponentStart OrElse localStart >= fComponentLimit Then makeLayoutWindow(localStart)

			Return calcLineBreak(localStart, maxAdvance) + fStart
		End Function

		''' <summary>
		''' Returns the graphical width of a line beginning at <code>start</code>
		''' and including characters up to <code>limit</code>.
		''' <code>start</code> and <code>limit</code> are absolute indices,
		''' not relative to the start of the paragraph.
		''' </summary>
		''' <param name="start"> the character index at which to start measuring </param>
		''' <param name="limit"> the character index at which to stop measuring </param>
		''' <returns> the graphical width of a line beginning at <code>start</code>
		'''   and including characters up to <code>limit</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>limit</code> is less
		'''         than <code>start</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>start</code> or
		'''          <code>limit</code> is not between the beginning of
		'''          the paragraph and the end of the paragraph. </exception>
		Public Function getAdvanceBetween(ByVal start As Integer, ByVal limit As Integer) As Single

			Dim localStart As Integer = start - fStart
			Dim localLimit As Integer = limit - fStart

			ensureComponents(localStart, localLimit)
			Dim line As TextLine = makeTextLineOnRange(localStart, localLimit)
			Return line.metrics.advance
			' could cache line in case getLayout is called with same start, limit
		End Function

		''' <summary>
		''' Returns a <code>TextLayout</code> on the given character range.
		''' </summary>
		''' <param name="start"> the index of the first character </param>
		''' <param name="limit"> the index after the last character.  Must be greater
		'''   than <code>start</code> </param>
		''' <returns> a <code>TextLayout</code> for the characters beginning at
		'''  <code>start</code> up to (but not including) <code>limit</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>limit</code> is less
		'''         than <code>start</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>start</code> or
		'''          <code>limit</code> is not between the beginning of
		'''          the paragraph and the end of the paragraph. </exception>
		Public Function getLayout(ByVal start As Integer, ByVal limit As Integer) As TextLayout

			Dim localStart As Integer = start - fStart
			Dim localLimit As Integer = limit - fStart

			ensureComponents(localStart, localLimit)
			Dim textLine_Renamed As TextLine = makeTextLineOnRange(localStart, localLimit)

			If localLimit < fChars.Length Then
				layoutCharCount += limit-start
				layoutCount += 1
			End If

			Return New TextLayout(textLine_Renamed, fBaseline, fBaselineOffsets, fJustifyRatio)
		End Function

		Private formattedChars As Integer = 0
		Private Shared wantStats As Boolean = False '"true".equals(System.getProperty("collectStats"));
		Private collectStats As Boolean = False

		Private Sub printStats()
			Console.WriteLine("formattedChars: " & formattedChars)
			'formattedChars = 0;
			collectStats = False
		End Sub

		''' <summary>
		''' Updates the <code>TextMeasurer</code> after a single character has
		''' been inserted
		''' into the paragraph currently represented by this
		''' <code>TextMeasurer</code>.  After this call, this
		''' <code>TextMeasurer</code> is equivalent to a new
		''' <code>TextMeasurer</code> created from the text;  however, it will
		''' usually be more efficient to update an existing
		''' <code>TextMeasurer</code> than to create a new one from scratch.
		''' </summary>
		''' <param name="newParagraph"> the text of the paragraph after performing
		''' the insertion.  Cannot be null. </param>
		''' <param name="insertPos"> the position in the text where the character was
		''' inserted.  Must not be less than the start of
		''' <code>newParagraph</code>, and must be less than the end of
		''' <code>newParagraph</code>. </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>insertPos</code> is less
		'''         than the start of <code>newParagraph</code> or greater than
		'''         or equal to the end of <code>newParagraph</code> </exception>
		''' <exception cref="NullPointerException"> if <code>newParagraph</code> is
		'''         <code>null</code> </exception>
		Public Sub insertChar(ByVal newParagraph As java.text.AttributedCharacterIterator, ByVal insertPos As Integer)

			If collectStats Then printStats()
			If wantStats Then collectStats = True

			fStart = newParagraph.beginIndex
			Dim [end] As Integer = newParagraph.endIndex
			If [end] - fStart <> fChars.Length+1 Then initAll(newParagraph)

			Dim newChars As Char() = New Char([end]-fStart - 1){}
			Dim newCharIndex As Integer = insertPos - fStart
			Array.Copy(fChars, 0, newChars, 0, newCharIndex)

			Dim newChar As Char = newParagraph.indexdex(insertPos)
			newChars(newCharIndex) = newChar
			Array.Copy(fChars, newCharIndex, newChars, newCharIndex+1, [end]-insertPos-1)
			fChars = newChars

			If fBidi IsNot Nothing OrElse java.text.Bidi.requiresBidi(newChars, newCharIndex, newCharIndex + 1) OrElse newParagraph.getAttribute(TextAttribute.BIDI_EMBEDDING) IsNot Nothing Then

				fBidi = New java.text.Bidi(newParagraph)
				If fBidi.leftToRight Then fBidi = Nothing
			End If

			fParagraph = StyledParagraph.insertChar(newParagraph, fChars, insertPos, fParagraph)
			invalidateComponents()
		End Sub

		''' <summary>
		''' Updates the <code>TextMeasurer</code> after a single character has
		''' been deleted
		''' from the paragraph currently represented by this
		''' <code>TextMeasurer</code>.  After this call, this
		''' <code>TextMeasurer</code> is equivalent to a new <code>TextMeasurer</code>
		''' created from the text;  however, it will usually be more efficient
		''' to update an existing <code>TextMeasurer</code> than to create a new one
		''' from scratch.
		''' </summary>
		''' <param name="newParagraph"> the text of the paragraph after performing
		''' the deletion.  Cannot be null. </param>
		''' <param name="deletePos"> the position in the text where the character was removed.
		''' Must not be less than
		''' the start of <code>newParagraph</code>, and must not be greater than the
		''' end of <code>newParagraph</code>. </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>deletePos</code> is
		'''         less than the start of <code>newParagraph</code> or greater
		'''         than the end of <code>newParagraph</code> </exception>
		''' <exception cref="NullPointerException"> if <code>newParagraph</code> is
		'''         <code>null</code> </exception>
		Public Sub deleteChar(ByVal newParagraph As java.text.AttributedCharacterIterator, ByVal deletePos As Integer)

			fStart = newParagraph.beginIndex
			Dim [end] As Integer = newParagraph.endIndex
			If [end] - fStart <> fChars.Length-1 Then initAll(newParagraph)

			Dim newChars As Char() = New Char([end]-fStart - 1){}
			Dim changedIndex As Integer = deletePos-fStart

			Array.Copy(fChars, 0, newChars, 0, deletePos-fStart)
			Array.Copy(fChars, changedIndex+1, newChars, changedIndex, [end]-deletePos)
			fChars = newChars

			If fBidi IsNot Nothing Then
				fBidi = New java.text.Bidi(newParagraph)
				If fBidi.leftToRight Then fBidi = Nothing
			End If

			fParagraph = StyledParagraph.deleteChar(newParagraph, fChars, deletePos, fParagraph)
			invalidateComponents()
		End Sub

		''' <summary>
		''' NOTE:  This method is only for LineBreakMeasurer's use.  It is package-
		''' private because it returns internal data.
		''' </summary>
		Friend Property chars As Char()
			Get
    
				Return fChars
			End Get
		End Property
	End Class

End Namespace