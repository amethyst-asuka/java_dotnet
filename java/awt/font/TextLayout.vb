Imports System
Imports System.Collections.Generic

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

'
' * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
' * (C) Copyright IBM Corp. 1996-2003, All Rights Reserved
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


	''' 
	''' <summary>
	''' <code>TextLayout</code> is an immutable graphical representation of styled
	''' character data.
	''' <p>
	''' It provides the following capabilities:
	''' <ul>
	''' <li>implicit bidirectional analysis and reordering,
	''' <li>cursor positioning and movement, including split cursors for
	''' mixed directional text,
	''' <li>highlighting, including both logical and visual highlighting
	''' for mixed directional text,
	''' <li>multiple baselines (roman, hanging, and centered),
	''' <li>hit testing,
	''' <li>justification,
	''' <li>default font substitution,
	''' <li>metric information such as ascent, descent, and advance, and
	''' <li>rendering
	''' </ul>
	''' <p>
	''' A <code>TextLayout</code> object can be rendered using
	''' its <code>draw</code> method.
	''' <p>
	''' <code>TextLayout</code> can be constructed either directly or through
	''' the use of a <seealso cref="LineBreakMeasurer"/>.  When constructed directly, the
	''' source text represents a single paragraph.  <code>LineBreakMeasurer</code>
	''' allows styled text to be broken into lines that fit within a particular
	''' width.  See the <code>LineBreakMeasurer</code> documentation for more
	''' information.
	''' <p>
	''' <code>TextLayout</code> construction logically proceeds as follows:
	''' <ul>
	''' <li>paragraph attributes are extracted and examined,
	''' <li>text is analyzed for bidirectional reordering, and reordering
	''' information is computed if needed,
	''' <li>text is segmented into style runs
	''' <li>fonts are chosen for style runs, first by using a font if the
	''' attribute <seealso cref="TextAttribute#FONT"/> is present, otherwise by computing
	''' a default font using the attributes that have been defined
	''' <li>if text is on multiple baselines, the runs or subruns are further
	''' broken into subruns sharing a common baseline,
	''' <li>glyphvectors are generated for each run using the chosen font,
	''' <li>final bidirectional reordering is performed on the glyphvectors
	''' </ul>
	''' <p>
	''' All graphical information returned from a <code>TextLayout</code>
	''' object's methods is relative to the origin of the
	''' <code>TextLayout</code>, which is the intersection of the
	''' <code>TextLayout</code> object's baseline with its left edge.  Also,
	''' coordinates passed into a <code>TextLayout</code> object's methods
	''' are assumed to be relative to the <code>TextLayout</code> object's
	''' origin.  Clients usually need to translate between a
	''' <code>TextLayout</code> object's coordinate system and the coordinate
	''' system in another object (such as a
	''' <seealso cref="java.awt.Graphics Graphics"/> object).
	''' <p>
	''' <code>TextLayout</code> objects are constructed from styled text,
	''' but they do not retain a reference to their source text.  Thus,
	''' changes in the text previously used to generate a <code>TextLayout</code>
	''' do not affect the <code>TextLayout</code>.
	''' <p>
	''' Three methods on a <code>TextLayout</code> object
	''' (<code>getNextRightHit</code>, <code>getNextLeftHit</code>, and
	''' <code>hitTestChar</code>) return instances of <seealso cref="TextHitInfo"/>.
	''' The offsets contained in these <code>TextHitInfo</code> objects
	''' are relative to the start of the <code>TextLayout</code>, <b>not</b>
	''' to the text used to create the <code>TextLayout</code>.  Similarly,
	''' <code>TextLayout</code> methods that accept <code>TextHitInfo</code>
	''' instances as parameters expect the <code>TextHitInfo</code> object's
	''' offsets to be relative to the <code>TextLayout</code>, not to any
	''' underlying text storage model.
	''' <p>
	''' <strong>Examples</strong>:<p>
	''' Constructing and drawing a <code>TextLayout</code> and its bounding
	''' rectangle:
	''' <blockquote><pre>
	'''   Graphics2D g = ...;
	'''   Point2D loc = ...;
	'''   Font font = Font.getFont("Helvetica-bold-italic");
	'''   FontRenderContext frc = g.getFontRenderContext();
	'''   TextLayout layout = new TextLayout("This is a string", font, frc);
	'''   layout.draw(g, (float)loc.getX(), (float)loc.getY());
	''' 
	'''   Rectangle2D bounds = layout.getBounds();
	'''   bounds.setRect(bounds.getX()+loc.getX(),
	'''                  bounds.getY()+loc.getY(),
	'''                  bounds.getWidth(),
	'''                  bounds.getHeight());
	'''   g.draw(bounds);
	''' </pre>
	''' </blockquote>
	''' <p>
	''' Hit-testing a <code>TextLayout</code> (determining which character is at
	''' a particular graphical location):
	''' <blockquote><pre>
	'''   Point2D click = ...;
	'''   TextHitInfo hit = layout.hitTestChar(
	'''                         (float) (click.getX() - loc.getX()),
	'''                         (float) (click.getY() - loc.getY()));
	''' </pre>
	''' </blockquote>
	''' <p>
	''' Responding to a right-arrow key press:
	''' <blockquote><pre>
	'''   int insertionIndex = ...;
	'''   TextHitInfo next = layout.getNextRightHit(insertionIndex);
	'''   if (next != null) {
	'''       // translate graphics to origin of layout on screen
	'''       g.translate(loc.getX(), loc.getY());
	'''       Shape[] carets = layout.getCaretShapes(next.getInsertionIndex());
	'''       g.draw(carets[0]);
	'''       if (carets[1] != null) {
	'''           g.draw(carets[1]);
	'''       }
	'''   }
	''' </pre></blockquote>
	''' <p>
	''' Drawing a selection range corresponding to a substring in the source text.
	''' The selected area may not be visually contiguous:
	''' <blockquote><pre>
	'''   // selStart, selLimit should be relative to the layout,
	'''   // not to the source text
	''' 
	'''   int selStart = ..., selLimit = ...;
	'''   Color selectionColor = ...;
	'''   Shape selection = layout.getLogicalHighlightShape(selStart, selLimit);
	'''   // selection may consist of disjoint areas
	'''   // graphics is assumed to be tranlated to origin of layout
	'''   g.setColor(selectionColor);
	'''   g.fill(selection);
	''' </pre></blockquote>
	''' <p>
	''' Drawing a visually contiguous selection range.  The selection range may
	''' correspond to more than one substring in the source text.  The ranges of
	''' the corresponding source text substrings can be obtained with
	''' <code>getLogicalRangesForVisualSelection()</code>:
	''' <blockquote><pre>
	'''   TextHitInfo selStart = ..., selLimit = ...;
	'''   Shape selection = layout.getVisualHighlightShape(selStart, selLimit);
	'''   g.setColor(selectionColor);
	'''   g.fill(selection);
	'''   int[] ranges = getLogicalRangesForVisualSelection(selStart, selLimit);
	'''   // ranges[0], ranges[1] is the first selection range,
	'''   // ranges[2], ranges[3] is the second selection range, etc.
	''' </pre></blockquote>
	''' <p>
	''' Note: Font rotations can cause text baselines to be rotated, and
	''' multiple runs with different rotations can cause the baseline to
	''' bend or zig-zag.  In order to account for this (rare) possibility,
	''' some APIs are specified to return metrics and take parameters 'in
	''' baseline-relative coordinates' (e.g. ascent, advance), and others
	''' are in 'in standard coordinates' (e.g. getBounds).  Values in
	''' baseline-relative coordinates map the 'x' coordinate to the
	''' distance along the baseline, (positive x is forward along the
	''' baseline), and the 'y' coordinate to a distance along the
	''' perpendicular to the baseline at 'x' (positive y is 90 degrees
	''' clockwise from the baseline vector).  Values in standard
	''' coordinates are measured along the x and y axes, with 0,0 at the
	''' origin of the TextLayout.  Documentation for each relevant API
	''' indicates what values are in what coordinate system.  In general,
	''' measurement-related APIs are in baseline-relative coordinates,
	''' while display-related APIs are in standard coordinates.
	''' </summary>
	''' <seealso cref= LineBreakMeasurer </seealso>
	''' <seealso cref= TextAttribute </seealso>
	''' <seealso cref= TextHitInfo </seealso>
	''' <seealso cref= LayoutPath </seealso>
	Public NotInheritable Class TextLayout
		Implements Cloneable

		Private characterCount As Integer
		Private isVerticalLine As Boolean = False
		Private baseline As SByte
		Private baselineOffsets As Single() ' why have these ?
		Private textLine As TextLine

		' cached values computed from GlyphSets and set info:
		' all are recomputed from scratch in buildCache()
		Private lineMetrics As TextLine.TextLineMetrics = Nothing
		Private visibleAdvance As Single
		Private hashCodeCache As Integer

	'    
	'     * TextLayouts are supposedly immutable.  If you mutate a TextLayout under
	'     * the covers (like the justification code does) you'll need to set this
	'     * back to false.  Could be replaced with textLine != null <--> cacheIsValid.
	'     
		Private cacheIsValid As Boolean = False


		' This value is obtained from an attribute, and constrained to the
		' interval [0,1].  If 0, the layout cannot be justified.
		Private justifyRatio As Single

		' If a layout is produced by justification, then that layout
		' cannot be justified.  To enforce this constraint the
		' justifyRatio of the justified layout is set to this value.
		Private Const ALREADY_JUSTIFIED As Single = -53.9f

		' dx and dy specify the distance between the TextLayout's origin
		' and the origin of the leftmost GlyphSet (TextLayoutComponent,
		' actually).  They were used for hanging punctuation support,
		' which is no longer implemented.  Currently they are both always 0,
		' and TextLayout is not guaranteed to work with non-zero dx, dy
		' values right now.  They were left in as an aide and reminder to
		' anyone who implements hanging punctuation or other similar stuff.
		' They are static now so they don't take up space in TextLayout
		' instances.
		Private Shared dx As Single
		Private Shared dy As Single

	'    
	'     * Natural bounds is used internally.  It is built on demand in
	'     * getNaturalBounds.
	'     
		Private naturalBounds As java.awt.geom.Rectangle2D = Nothing

	'    
	'     * boundsRect encloses all of the bits this TextLayout can draw.  It
	'     * is build on demand in getBounds.
	'     
		Private boundsRect As java.awt.geom.Rectangle2D = Nothing

	'    
	'     * flag to supress/allow carets inside of ligatures when hit testing or
	'     * arrow-keying
	'     
		Private caretsInLigaturesAreAllowed As Boolean = False

		''' <summary>
		''' Defines a policy for determining the strong caret location.
		''' This class contains one method, <code>getStrongCaret</code>, which
		''' is used to specify the policy that determines the strong caret in
		''' dual-caret text.  The strong caret is used to move the caret to the
		''' left or right. Instances of this class can be passed to
		''' <code>getCaretShapes</code>, <code>getNextLeftHit</code> and
		''' <code>getNextRightHit</code> to customize strong caret
		''' selection.
		''' <p>
		''' To specify alternate caret policies, subclass <code>CaretPolicy</code>
		''' and override <code>getStrongCaret</code>.  <code>getStrongCaret</code>
		''' should inspect the two <code>TextHitInfo</code> arguments and choose
		''' one of them as the strong caret.
		''' <p>
		''' Most clients do not need to use this class.
		''' </summary>
		Public Class CaretPolicy

			''' <summary>
			''' Constructs a <code>CaretPolicy</code>.
			''' </summary>
			 Public Sub New()
			 End Sub

			''' <summary>
			''' Chooses one of the specified <code>TextHitInfo</code> instances as
			''' a strong caret in the specified <code>TextLayout</code>. </summary>
			''' <param name="hit1"> a valid hit in <code>layout</code> </param>
			''' <param name="hit2"> a valid hit in <code>layout</code> </param>
			''' <param name="layout"> the <code>TextLayout</code> in which
			'''        <code>hit1</code> and <code>hit2</code> are used </param>
			''' <returns> <code>hit1</code> or <code>hit2</code>
			'''        (or an equivalent <code>TextHitInfo</code>), indicating the
			'''        strong caret. </returns>
			Public Overridable Function getStrongCaret(ByVal hit1 As TextHitInfo, ByVal hit2 As TextHitInfo, ByVal layout As TextLayout) As TextHitInfo

				' default implementation just calls private method on layout
				Return layout.getStrongHit(hit1, hit2)
			End Function
		End Class

		''' <summary>
		''' This <code>CaretPolicy</code> is used when a policy is not specified
		''' by the client.  With this policy, a hit on a character whose direction
		''' is the same as the line direction is stronger than a hit on a
		''' counterdirectional character.  If the characters' directions are
		''' the same, a hit on the leading edge of a character is stronger
		''' than a hit on the trailing edge of a character.
		''' </summary>
		Public Shared ReadOnly DEFAULT_CARET_POLICY As New CaretPolicy

		''' <summary>
		''' Constructs a <code>TextLayout</code> from a <code>String</code>
		''' and a <seealso cref="Font"/>.  All the text is styled using the specified
		''' <code>Font</code>.
		''' <p>
		''' The <code>String</code> must specify a single paragraph of text,
		''' because an entire paragraph is required for the bidirectional
		''' algorithm. </summary>
		''' <param name="string"> the text to display </param>
		''' <param name="font"> a <code>Font</code> used to style the text </param>
		''' <param name="frc"> contains information about a graphics device which is needed
		'''       to measure the text correctly.
		'''       Text measurements can vary slightly depending on the
		'''       device resolution, and attributes such as antialiasing.  This
		'''       parameter does not specify a translation between the
		'''       <code>TextLayout</code> and user space. </param>
		Public Sub New(ByVal [string] As String, ByVal font_Renamed As java.awt.Font, ByVal frc As FontRenderContext)

			If font_Renamed Is Nothing Then Throw New IllegalArgumentException("Null font passed to TextLayout constructor.")

			If string_Renamed Is Nothing Then Throw New IllegalArgumentException("Null string passed to TextLayout constructor.")

			If string_Renamed.length() = 0 Then Throw New IllegalArgumentException("Zero length string passed to TextLayout constructor.")

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim attributes As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = Nothing
			If font_Renamed.hasLayoutAttributes() Then attributes = font_Renamed.attributes

			Dim text As Char() = string_Renamed.ToCharArray()
			If sameBaselineUpTo(font_Renamed, text, 0, text.Length) = text.Length Then
				fastInit(text, font_Renamed, attributes, frc)
			Else
				Dim [as] As java.text.AttributedString = If(attributes Is Nothing, New java.text.AttributedString(string_Renamed), New java.text.AttributedString(string_Renamed, attributes))
				[as].addAttribute(TextAttribute.FONT, font_Renamed)
				standardInit([as].iterator, text, frc)
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>TextLayout</code> from a <code>String</code>
		''' and an attribute set.
		''' <p>
		''' All the text is styled using the provided attributes.
		''' <p>
		''' <code>string</code> must specify a single paragraph of text because an
		''' entire paragraph is required for the bidirectional algorithm. </summary>
		''' <param name="string"> the text to display </param>
		''' <param name="attributes"> the attributes used to style the text </param>
		''' <param name="frc"> contains information about a graphics device which is needed
		'''       to measure the text correctly.
		'''       Text measurements can vary slightly depending on the
		'''       device resolution, and attributes such as antialiasing.  This
		'''       parameter does not specify a translation between the
		'''       <code>TextLayout</code> and user space. </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal [string] As String, ByVal attributes As IDictionary(Of T1), ByVal frc As FontRenderContext)
			If string_Renamed Is Nothing Then Throw New IllegalArgumentException("Null string passed to TextLayout constructor.")

			If attributes Is Nothing Then Throw New IllegalArgumentException("Null map passed to TextLayout constructor.")

			If string_Renamed.length() = 0 Then Throw New IllegalArgumentException("Zero length string passed to TextLayout constructor.")

			Dim text As Char() = string_Renamed.ToCharArray()
			Dim font_Renamed As java.awt.Font = singleFont(text, 0, text.Length, attributes)
			If font_Renamed IsNot Nothing Then
				fastInit(text, font_Renamed, attributes, frc)
			Else
				Dim [as] As New java.text.AttributedString(string_Renamed, attributes)
				standardInit([as].iterator, text, frc)
			End If
		End Sub

	'    
	'     * Determines a font for the attributes, and if a single font can render
	'     * all the text on one baseline, return it, otherwise null.  If the
	'     * attributes specify a font, assume it can display all the text without
	'     * checking.
	'     * If the AttributeSet contains an embedded graphic, return null.
	'     
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function singleFont(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal text As Char(), ByVal start As Integer, ByVal limit As Integer, ByVal attributes As IDictionary(Of T1)) As java.awt.Font

			If attributes(TextAttribute.CHAR_REPLACEMENT) IsNot Nothing Then Return Nothing

			Dim font_Renamed As java.awt.Font = Nothing
			Try
				font_Renamed = CType(attributes(TextAttribute.FONT), java.awt.Font)
			Catch e As  ClassCastException
			End Try
			If font_Renamed Is Nothing Then
				If attributes(TextAttribute.FAMILY) IsNot Nothing Then
					font_Renamed = java.awt.Font.getFont(attributes)
					If font_Renamed.canDisplayUpTo(text, start, limit) <> -1 Then Return Nothing
				Else
					Dim resolver As sun.font.FontResolver = sun.font.FontResolver.instance
					Dim iter As sun.text.CodePointIterator = sun.text.CodePointIterator.create(text, start, limit)
					Dim fontIndex As Integer = resolver.nextFontRunIndex(iter)
					If iter.charIndex() = limit Then font_Renamed = resolver.getFont(fontIndex, attributes)
				End If
			End If

			If sameBaselineUpTo(font_Renamed, text, start, limit) <> limit Then Return Nothing

			Return font_Renamed
		End Function

		''' <summary>
		''' Constructs a <code>TextLayout</code> from an iterator over styled text.
		''' <p>
		''' The iterator must specify a single paragraph of text because an
		''' entire paragraph is required for the bidirectional
		''' algorithm. </summary>
		''' <param name="text"> the styled text to display </param>
		''' <param name="frc"> contains information about a graphics device which is needed
		'''       to measure the text correctly.
		'''       Text measurements can vary slightly depending on the
		'''       device resolution, and attributes such as antialiasing.  This
		'''       parameter does not specify a translation between the
		'''       <code>TextLayout</code> and user space. </param>
		Public Sub New(ByVal text As java.text.AttributedCharacterIterator, ByVal frc As FontRenderContext)

			If text Is Nothing Then Throw New IllegalArgumentException("Null iterator passed to TextLayout constructor.")

			Dim start As Integer = text.beginIndex
			Dim limit As Integer = text.endIndex
			If start = limit Then Throw New IllegalArgumentException("Zero length iterator passed to TextLayout constructor.")

			Dim len As Integer = limit - start
			text.first()
			Dim chars As Char() = New Char(len - 1){}
			Dim n As Integer = 0
			Dim c As Char = text.first()
			Do While c <> java.text.CharacterIterator.DONE
				chars(n) = c
				n += 1
				c = text.next()
			Loop

			text.first()
			If text.runLimit = limit Then

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim attributes As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = text.attributes
				Dim font_Renamed As java.awt.Font = singleFont(chars, 0, len, attributes)
				If font_Renamed IsNot Nothing Then
					fastInit(chars, font_Renamed, attributes, frc)
					Return
				End If
			End If

			standardInit(text, chars, frc)
		End Sub

		''' <summary>
		''' Creates a <code>TextLayout</code> from a <seealso cref="TextLine"/> and
		''' some paragraph data.  This method is used by <seealso cref="TextMeasurer"/>. </summary>
		''' <param name="textLine"> the line measurement attributes to apply to the
		'''       the resulting <code>TextLayout</code> </param>
		''' <param name="baseline"> the baseline of the text </param>
		''' <param name="baselineOffsets"> the baseline offsets for this
		''' <code>TextLayout</code>.  This should already be normalized to
		''' <code>baseline</code> </param>
		''' <param name="justifyRatio"> <code>0</code> if the <code>TextLayout</code>
		'''     cannot be justified; <code>1</code> otherwise. </param>
		Friend Sub New(ByVal textLine_Renamed As TextLine, ByVal baseline As SByte, ByVal baselineOffsets As Single(), ByVal justifyRatio As Single)

			Me.characterCount = textLine_Renamed.characterCount()
			Me.baseline = baseline
			Me.baselineOffsets = baselineOffsets
			Me.textLine = textLine_Renamed
			Me.justifyRatio = justifyRatio
		End Sub

		''' <summary>
		''' Initialize the paragraph-specific data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Sub paragraphInit(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal aBaseline As SByte, ByVal lm As sun.font.CoreMetrics, ByVal paragraphAttrs As IDictionary(Of T1), ByVal text As Char())

			baseline = aBaseline

			' normalize to current baseline
			baselineOffsets = TextLine.getNormalizedOffsets(lm.baselineOffsets, baseline)

			justifyRatio = sun.font.AttributeValues.getJustification(paragraphAttrs)
			Dim shaper As java.awt.font.NumericShaper = sun.font.AttributeValues.getNumericShaping(paragraphAttrs)
			If shaper IsNot Nothing Then shaper.shape(text, 0, text.Length)
		End Sub

	'    
	'     * the fast init generates a single glyph set.  This requires:
	'     * all one style
	'     * all renderable by one font (ie no embedded graphics)
	'     * all on one baseline
	'     
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Sub fastInit(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal chars As Char(), ByVal font_Renamed As java.awt.Font, ByVal attrs As IDictionary(Of T1), ByVal frc As FontRenderContext)

			' Object vf = attrs.get(TextAttribute.ORIENTATION);
			' isVerticalLine = TextAttribute.ORIENTATION_VERTICAL.equals(vf);
			isVerticalLine = False

			Dim lm As LineMetrics = font_Renamed.getLineMetrics(chars, 0, chars.Length, frc)
			Dim cm As sun.font.CoreMetrics = sun.font.CoreMetrics.get(lm)
			Dim glyphBaseline As SByte = CByte(cm.baselineIndex)

			If attrs Is Nothing Then
				baseline = glyphBaseline
				baselineOffsets = cm.baselineOffsets
				justifyRatio = 1.0f
			Else
				paragraphInit(glyphBaseline, cm, attrs, chars)
			End If

			characterCount = chars.Length

			textLine = TextLine.fastCreateTextLine(frc, chars, font_Renamed, cm, attrs)
		End Sub

	'    
	'     * the standard init generates multiple glyph sets based on style,
	'     * renderable, and baseline runs.
	'     * @param chars the text in the iterator, extracted into a char array
	'     
		Private Sub standardInit(ByVal text As java.text.AttributedCharacterIterator, ByVal chars As Char(), ByVal frc As FontRenderContext)

			characterCount = chars.Length

			' set paragraph attributes
				' If there's an embedded graphic at the start of the
				' paragraph, look for the first non-graphic character
				' and use it and its font to initialize the paragraph.
				' If not, use the first graphic to initialize.

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim paragraphAttrs As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = text.attributes

				Dim haveFont As Boolean = TextLine.advanceToFirstFont(text)

				If haveFont Then
					Dim defaultFont As java.awt.Font = TextLine.getFontAtCurrentPos(text)
					Dim charsStart As Integer = text.index - text.beginIndex
					Dim lm As LineMetrics = defaultFont.getLineMetrics(chars, charsStart, charsStart+1, frc)
					Dim cm As sun.font.CoreMetrics = sun.font.CoreMetrics.get(lm)
					paragraphInit(CByte(cm.baselineIndex), cm, paragraphAttrs, chars)
				Else
					' hmmm what to do here?  Just try to supply reasonable
					' values I guess.

					Dim graphic As GraphicAttribute = CType(paragraphAttrs(TextAttribute.CHAR_REPLACEMENT), GraphicAttribute)
					Dim defaultBaseline As SByte = getBaselineFromGraphic(graphic)
					Dim cm As sun.font.CoreMetrics = sun.font.GraphicComponent.createCoreMetrics(graphic)
					paragraphInit(defaultBaseline, cm, paragraphAttrs, chars)
				End If

			textLine = TextLine.standardCreateTextLine(frc, text, chars, baselineOffsets)
		End Sub

	'    
	'     * A utility to rebuild the ascent/descent/leading/advance cache.
	'     * You'll need to call this if you clone and mutate (like justification,
	'     * editing methods do)
	'     
		Private Sub ensureCache()
			If Not cacheIsValid Then buildCache()
		End Sub

		Private Sub buildCache()
			lineMetrics = textLine.metrics

			' compute visibleAdvance
			If textLine.directionLTR Then

				Dim lastNonSpace As Integer = characterCount-1
				Do While lastNonSpace <> -1
					Dim logIndex As Integer = textLine.visualToLogical(lastNonSpace)
					If Not textLine.isCharSpace(logIndex) Then
						Exit Do
					Else
						lastNonSpace -= 1
					End If
				Loop
				If lastNonSpace = characterCount-1 Then
					visibleAdvance = lineMetrics.advance
				ElseIf lastNonSpace = -1 Then
					visibleAdvance = 0
				Else
					Dim logIndex As Integer = textLine.visualToLogical(lastNonSpace)
					visibleAdvance = textLine.getCharLinePosition(logIndex) + textLine.getCharAdvance(logIndex)
				End If
			Else

				Dim leftmostNonSpace As Integer = 0
				Do While leftmostNonSpace <> characterCount
					Dim logIndex As Integer = textLine.visualToLogical(leftmostNonSpace)
					If Not textLine.isCharSpace(logIndex) Then
						Exit Do
					Else
						leftmostNonSpace += 1
					End If
				Loop
				If leftmostNonSpace = characterCount Then
					visibleAdvance = 0
				ElseIf leftmostNonSpace = 0 Then
					visibleAdvance = lineMetrics.advance
				Else
					Dim logIndex As Integer = textLine.visualToLogical(leftmostNonSpace)
					Dim pos As Single = textLine.getCharLinePosition(logIndex)
					visibleAdvance = lineMetrics.advance - pos
				End If
			End If

			' naturalBounds, boundsRect will be generated on demand
			naturalBounds = Nothing
			boundsRect = Nothing

			' hashCode will be regenerated on demand
			hashCodeCache = 0

			cacheIsValid = True
		End Sub

		''' <summary>
		''' The 'natural bounds' encloses all the carets the layout can draw.
		''' 
		''' </summary>
		Private Property naturalBounds As java.awt.geom.Rectangle2D
			Get
				ensureCache()
    
				If naturalBounds Is Nothing Then naturalBounds = textLine.italicBounds
    
				Return naturalBounds
			End Get
		End Property

		''' <summary>
		''' Creates a copy of this <code>TextLayout</code>.
		''' </summary>
		Protected Friend Function clone() As Object
	'        
	'         * !!! I think this is safe.  Once created, nothing mutates the
	'         * glyphvectors or arrays.  But we need to make sure.
	'         * {jbr} actually, that's not quite true.  The justification code
	'         * mutates after cloning.  It doesn't actually change the glyphvectors
	'         * (that's impossible) but it replaces them with justified sets.  This
	'         * is a problem for GlyphIterator creation, since new GlyphIterators
	'         * are created by cloning a prototype.  If the prototype has outdated
	'         * glyphvectors, so will the new ones.  A partial solution is to set the
	'         * prototypical GlyphIterator to null when the glyphvectors change.  If
	'         * you forget this one time, you're hosed.
	'         
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

	'    
	'     * Utility to throw an expection if an invalid TextHitInfo is passed
	'     * as a parameter.  Avoids code duplication.
	'     
		Private Sub checkTextHit(ByVal hit As TextHitInfo)
			If hit Is Nothing Then Throw New IllegalArgumentException("TextHitInfo is null.")

			If hit.insertionIndex < 0 OrElse hit.insertionIndex > characterCount Then Throw New IllegalArgumentException("TextHitInfo is out of range")
		End Sub

		''' <summary>
		''' Creates a copy of this <code>TextLayout</code> justified to the
		''' specified width.
		''' <p>
		''' If this <code>TextLayout</code> has already been justified, an
		''' exception is thrown.  If this <code>TextLayout</code> object's
		''' justification ratio is zero, a <code>TextLayout</code> identical
		''' to this <code>TextLayout</code> is returned. </summary>
		''' <param name="justificationWidth"> the width to use when justifying the line.
		''' For best results, it should not be too different from the current
		''' advance of the line. </param>
		''' <returns> a <code>TextLayout</code> justified to the specified width. </returns>
		''' <exception cref="Error"> if this layout has already been justified, an Error is
		''' thrown. </exception>
		Public Function getJustifiedLayout(ByVal justificationWidth As Single) As TextLayout

			If justificationWidth <= 0 Then Throw New IllegalArgumentException("justificationWidth <= 0 passed to TextLayout.getJustifiedLayout()")

			If justifyRatio = ALREADY_JUSTIFIED Then Throw New [Error]("Can't justify again.")

			ensureCache() ' make sure textLine is not null

			' default justification range to exclude trailing logical whitespace
			Dim limit As Integer = characterCount
			Do While limit > 0 AndAlso textLine.isCharWhitespace(limit-1)
				limit -= 1
			Loop

			Dim newLine As TextLine = textLine.getJustifiedLine(justificationWidth, justifyRatio, 0, limit)
			If newLine IsNot Nothing Then Return New TextLayout(newLine, baseline, baselineOffsets, ALREADY_JUSTIFIED)

			Return Me
		End Function

		''' <summary>
		''' Justify this layout.  Overridden by subclassers to control justification
		''' (if there were subclassers, that is...)
		''' 
		''' The layout will only justify if the paragraph attributes (from the
		''' source text, possibly defaulted by the layout attributes) indicate a
		''' non-zero justification ratio.  The text will be justified to the
		''' indicated width.  The current implementation also adjusts hanging
		''' punctuation and trailing whitespace to overhang the justification width.
		''' Once justified, the layout may not be rejustified.
		''' <p>
		''' Some code may rely on immutablity of layouts.  Subclassers should not
		''' call this directly, but instead should call getJustifiedLayout, which
		''' will call this method on a clone of this layout, preserving
		''' the original.
		''' </summary>
		''' <param name="justificationWidth"> the width to use when justifying the line.
		''' For best results, it should not be too different from the current
		''' advance of the line. </param>
		''' <seealso cref= #getJustifiedLayout(float) </seealso>
		Protected Friend Sub handleJustify(ByVal justificationWidth As Single)
		  ' never called
		End Sub


		''' <summary>
		''' Returns the baseline for this <code>TextLayout</code>.
		''' The baseline is one of the values defined in <code>Font</code>,
		''' which are roman, centered and hanging.  Ascent and descent are
		''' relative to this baseline.  The <code>baselineOffsets</code>
		''' are also relative to this baseline. </summary>
		''' <returns> the baseline of this <code>TextLayout</code>. </returns>
		''' <seealso cref= #getBaselineOffsets() </seealso>
		''' <seealso cref= Font </seealso>
		Public Property baseline As SByte
			Get
				Return baseline
			End Get
		End Property

		''' <summary>
		''' Returns the offsets array for the baselines used for this
		''' <code>TextLayout</code>.
		''' <p>
		''' The array is indexed by one of the values defined in
		''' <code>Font</code>, which are roman, centered and hanging.  The
		''' values are relative to this <code>TextLayout</code> object's
		''' baseline, so that <code>getBaselineOffsets[getBaseline()] == 0</code>.
		''' Offsets are added to the position of the <code>TextLayout</code>
		''' object's baseline to get the position for the new baseline. </summary>
		''' <returns> the offsets array containing the baselines used for this
		'''    <code>TextLayout</code>. </returns>
		''' <seealso cref= #getBaseline() </seealso>
		''' <seealso cref= Font </seealso>
		Public Property baselineOffsets As Single()
			Get
				Dim offsets As Single() = New Single(baselineOffsets.Length - 1){}
				Array.Copy(baselineOffsets, 0, offsets, 0, offsets.Length)
				Return offsets
			End Get
		End Property

		''' <summary>
		''' Returns the advance of this <code>TextLayout</code>.
		''' The advance is the distance from the origin to the advance of the
		''' rightmost (bottommost) character.  This is in baseline-relative
		''' coordinates. </summary>
		''' <returns> the advance of this <code>TextLayout</code>. </returns>
		Public Property advance As Single
			Get
				ensureCache()
				Return lineMetrics.advance
			End Get
		End Property

		''' <summary>
		''' Returns the advance of this <code>TextLayout</code>, minus trailing
		''' whitespace.  This is in baseline-relative coordinates. </summary>
		''' <returns> the advance of this <code>TextLayout</code> without the
		'''      trailing whitespace. </returns>
		''' <seealso cref= #getAdvance() </seealso>
		Public Property visibleAdvance As Single
			Get
				ensureCache()
				Return visibleAdvance
			End Get
		End Property

		''' <summary>
		''' Returns the ascent of this <code>TextLayout</code>.
		''' The ascent is the distance from the top (right) of the
		''' <code>TextLayout</code> to the baseline.  It is always either
		''' positive or zero.  The ascent is sufficient to
		''' accommodate superscripted text and is the maximum of the sum of the
		''' ascent, offset, and baseline of each glyph.  The ascent is
		''' the maximum ascent from the baseline of all the text in the
		''' TextLayout.  It is in baseline-relative coordinates. </summary>
		''' <returns> the ascent of this <code>TextLayout</code>. </returns>
		Public Property ascent As Single
			Get
				ensureCache()
				Return lineMetrics.ascent
			End Get
		End Property

		''' <summary>
		''' Returns the descent of this <code>TextLayout</code>.
		''' The descent is the distance from the baseline to the bottom (left) of
		''' the <code>TextLayout</code>.  It is always either positive or zero.
		''' The descent is sufficient to accommodate subscripted text and is the
		''' maximum of the sum of the descent, offset, and baseline of each glyph.
		''' This is the maximum descent from the baseline of all the text in
		''' the TextLayout.  It is in baseline-relative coordinates. </summary>
		''' <returns> the descent of this <code>TextLayout</code>. </returns>
		Public Property descent As Single
			Get
				ensureCache()
				Return lineMetrics.descent
			End Get
		End Property

		''' <summary>
		''' Returns the leading of the <code>TextLayout</code>.
		''' The leading is the suggested interline spacing for this
		''' <code>TextLayout</code>.  This is in baseline-relative
		''' coordinates.
		''' <p>
		''' The leading is computed from the leading, descent, and baseline
		''' of all glyphvectors in the <code>TextLayout</code>.  The algorithm
		''' is roughly as follows:
		''' <blockquote><pre>
		''' maxD = 0;
		''' maxDL = 0;
		''' for (GlyphVector g in all glyphvectors) {
		'''    maxD = max(maxD, g.getDescent() + offsets[g.getBaseline()]);
		'''    maxDL = max(maxDL, g.getDescent() + g.getLeading() +
		'''                       offsets[g.getBaseline()]);
		''' }
		''' return maxDL - maxD;
		''' </pre></blockquote> </summary>
		''' <returns> the leading of this <code>TextLayout</code>. </returns>
		Public Property leading As Single
			Get
				ensureCache()
				Return lineMetrics.leading
			End Get
		End Property

		''' <summary>
		''' Returns the bounds of this <code>TextLayout</code>.
		''' The bounds are in standard coordinates.
		''' <p>Due to rasterization effects, this bounds might not enclose all of the
		''' pixels rendered by the TextLayout.</p>
		''' It might not coincide exactly with the ascent, descent,
		''' origin or advance of the <code>TextLayout</code>. </summary>
		''' <returns> a <seealso cref="Rectangle2D"/> that is the bounds of this
		'''        <code>TextLayout</code>. </returns>
		Public Property bounds As java.awt.geom.Rectangle2D
			Get
				ensureCache()
    
				If boundsRect Is Nothing Then
					Dim vb As java.awt.geom.Rectangle2D = textLine.visualBounds
					If dx <> 0 OrElse dy <> 0 Then vb.rectect(vb.x - dx, vb.y - dy, vb.width, vb.height)
					boundsRect = vb
				End If
    
				Dim bounds_Renamed As java.awt.geom.Rectangle2D = New java.awt.geom.Rectangle2D.Float
				bounds_Renamed.rect = boundsRect
    
				Return bounds_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns the pixel bounds of this <code>TextLayout</code> when
		''' rendered in a graphics with the given
		''' <code>FontRenderContext</code> at the given location.  The
		''' graphics render context need not be the same as the
		''' <code>FontRenderContext</code> used to create this
		''' <code>TextLayout</code>, and can be null.  If it is null, the
		''' <code>FontRenderContext</code> of this <code>TextLayout</code>
		''' is used. </summary>
		''' <param name="frc"> the <code>FontRenderContext</code> of the <code>Graphics</code>. </param>
		''' <param name="x"> the x-coordinate at which to render this <code>TextLayout</code>. </param>
		''' <param name="y"> the y-coordinate at which to render this <code>TextLayout</code>. </param>
		''' <returns> a <code>Rectangle</code> bounding the pixels that would be affected. </returns>
		''' <seealso cref= GlyphVector#getPixelBounds
		''' @since 1.6 </seealso>
		Public Function getPixelBounds(ByVal frc As FontRenderContext, ByVal x As Single, ByVal y As Single) As java.awt.Rectangle
			Return textLine.getPixelBounds(frc, x, y)
		End Function

		''' <summary>
		''' Returns <code>true</code> if this <code>TextLayout</code> has
		''' a left-to-right base direction or <code>false</code> if it has
		''' a right-to-left base direction.  The <code>TextLayout</code>
		''' has a base direction of either left-to-right (LTR) or
		''' right-to-left (RTL).  The base direction is independent of the
		''' actual direction of text on the line, which may be either LTR,
		''' RTL, or mixed. Left-to-right layouts by default should position
		''' flush left.  If the layout is on a tabbed line, the
		''' tabs run left to right, so that logically successive layouts position
		''' left to right.  The opposite is true for RTL layouts. By default they
		''' should position flush left, and tabs run right-to-left. </summary>
		''' <returns> <code>true</code> if the base direction of this
		'''         <code>TextLayout</code> is left-to-right; <code>false</code>
		'''         otherwise. </returns>
		Public Property leftToRight As Boolean
			Get
				Return textLine.directionLTR
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this <code>TextLayout</code> is vertical. </summary>
		''' <returns> <code>true</code> if this <code>TextLayout</code> is vertical;
		'''      <code>false</code> otherwise. </returns>
		Public Property vertical As Boolean
			Get
				Return isVerticalLine
			End Get
		End Property

		''' <summary>
		''' Returns the number of characters represented by this
		''' <code>TextLayout</code>. </summary>
		''' <returns> the number of characters in this <code>TextLayout</code>. </returns>
		Public Property characterCount As Integer
			Get
				Return characterCount
			End Get
		End Property

	'    
	'     * carets and hit testing
	'     *
	'     * Positions on a text line are represented by instances of TextHitInfo.
	'     * Any TextHitInfo with characterOffset between 0 and characterCount-1,
	'     * inclusive, represents a valid position on the line.  Additionally,
	'     * [-1, trailing] and [characterCount, leading] are valid positions, and
	'     * represent positions at the logical start and end of the line,
	'     * respectively.
	'     *
	'     * The characterOffsets in TextHitInfo's used and returned by TextLayout
	'     * are relative to the beginning of the text layout, not necessarily to
	'     * the beginning of the text storage the client is using.
	'     *
	'     *
	'     * Every valid TextHitInfo has either one or two carets associated with it.
	'     * A caret is a visual location in the TextLayout indicating where text at
	'     * the TextHitInfo will be displayed on screen.  If a TextHitInfo
	'     * represents a location on a directional boundary, then there are two
	'     * possible visible positions for newly inserted text.  Consider the
	'     * following example, in which capital letters indicate right-to-left text,
	'     * and the overall line direction is left-to-right:
	'     *
	'     * Text Storage: [ a, b, C, D, E, f ]
	'     * Display:        a b E D C f
	'     *
	'     * The text hit info (1, t) represents the trailing side of 'b'.  If 'q',
	'     * a left-to-right character is inserted into the text storage at this
	'     * location, it will be displayed between the 'b' and the 'E':
	'     *
	'     * Text Storage: [ a, b, q, C, D, E, f ]
	'     * Display:        a b q E D C f
	'     *
	'     * However, if a 'W', which is right-to-left, is inserted into the storage
	'     * after 'b', the storage and display will be:
	'     *
	'     * Text Storage: [ a, b, W, C, D, E, f ]
	'     * Display:        a b E D C W f
	'     *
	'     * So, for the original text storage, two carets should be displayed for
	'     * location (1, t): one visually between 'b' and 'E' and one visually
	'     * between 'C' and 'f'.
	'     *
	'     *
	'     * When two carets are displayed for a TextHitInfo, one caret is the
	'     * 'strong' caret and the other is the 'weak' caret.  The strong caret
	'     * indicates where an inserted character will be displayed when that
	'     * character's direction is the same as the direction of the TextLayout.
	'     * The weak caret shows where an character inserted character will be
	'     * displayed when the character's direction is opposite that of the
	'     * TextLayout.
	'     *
	'     *
	'     * Clients should not be overly concerned with the details of correct
	'     * caret display. TextLayout.getCaretShapes(TextHitInfo) will return an
	'     * array of two paths representing where carets should be displayed.
	'     * The first path in the array is the strong caret; the second element,
	'     * if non-null, is the weak caret.  If the second element is null,
	'     * then there is no weak caret for the given TextHitInfo.
	'     *
	'     *
	'     * Since text can be visually reordered, logically consecutive
	'     * TextHitInfo's may not be visually consecutive.  One implication of this
	'     * is that a client cannot tell from inspecting a TextHitInfo whether the
	'     * hit represents the first (or last) caret in the layout.  Clients
	'     * can call getVisualOtherHit();  if the visual companion is
	'     * (-1, TRAILING) or (characterCount, LEADING), then the hit is at the
	'     * first (last) caret position in the layout.
	'     

		Private Function getCaretInfo(ByVal caret As Integer, ByVal bounds As java.awt.geom.Rectangle2D, ByVal info As Single()) As Single()

			Dim top1X, top2X As Single
			Dim bottom1X, bottom2X As Single

			If caret = 0 OrElse caret = characterCount Then

				Dim pos As Single
				Dim logIndex As Integer
				If caret = characterCount Then
					logIndex = textLine.visualToLogical(characterCount-1)
					pos = textLine.getCharLinePosition(logIndex) + textLine.getCharAdvance(logIndex)
				Else
					logIndex = textLine.visualToLogical(caret)
					pos = textLine.getCharLinePosition(logIndex)
				End If
				Dim angle As Single = textLine.getCharAngle(logIndex)
				Dim shift As Single = textLine.getCharShift(logIndex)
				pos += angle * shift
					top2X = pos + angle*textLine.getCharAscent(logIndex)
					top1X = top2X
					bottom2X = pos - angle*textLine.getCharDescent(logIndex)
					bottom1X = bottom2X
			Else

					Dim logIndex As Integer = textLine.visualToLogical(caret-1)
					Dim angle1 As Single = textLine.getCharAngle(logIndex)
					Dim pos1 As Single = textLine.getCharLinePosition(logIndex) + textLine.getCharAdvance(logIndex)
					If angle1 <> 0 Then
						pos1 += angle1 * textLine.getCharShift(logIndex)
						top1X = pos1 + angle1*textLine.getCharAscent(logIndex)
						bottom1X = pos1 - angle1*textLine.getCharDescent(logIndex)
					Else
							bottom1X = pos1
							top1X = bottom1X
					End If
					Dim logIndex As Integer = textLine.visualToLogical(caret)
					Dim angle2 As Single = textLine.getCharAngle(logIndex)
					Dim pos2 As Single = textLine.getCharLinePosition(logIndex)
					If angle2 <> 0 Then
						pos2 += angle2*textLine.getCharShift(logIndex)
						top2X = pos2 + angle2*textLine.getCharAscent(logIndex)
						bottom2X = pos2 - angle2*textLine.getCharDescent(logIndex)
					Else
							bottom2X = pos2
							top2X = bottom2X
					End If
			End If

			Dim topX As Single = (top1X + top2X) / 2
			Dim bottomX As Single = (bottom1X + bottom2X) / 2

			If info Is Nothing Then info = New Single(1){}

			If isVerticalLine Then
				info(1) = CSng((topX - bottomX) / bounds.width)
				info(0) = CSng(topX + (info(1)*bounds.x))
			Else
				info(1) = CSng((topX - bottomX) / bounds.height)
				info(0) = CSng(bottomX + (info(1)*bounds.maxY))
			End If

			Return info
		End Function

		''' <summary>
		''' Returns information about the caret corresponding to <code>hit</code>.
		''' The first element of the array is the intersection of the caret with
		''' the baseline, as a distance along the baseline. The second element
		''' of the array is the inverse slope (run/rise) of the caret, measured
		''' with respect to the baseline at that point.
		''' <p>
		''' This method is meant for informational use.  To display carets, it
		''' is better to use <code>getCaretShapes</code>. </summary>
		''' <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		''' <param name="bounds"> the bounds to which the caret info is constructed.
		'''     The bounds is in baseline-relative coordinates. </param>
		''' <returns> a two-element array containing the position and slope of
		''' the caret.  The returned caret info is in baseline-relative coordinates. </returns>
		''' <seealso cref= #getCaretShapes(int, Rectangle2D, TextLayout.CaretPolicy) </seealso>
		''' <seealso cref= Font#getItalicAngle </seealso>
		Public Function getCaretInfo(ByVal hit As TextHitInfo, ByVal bounds As java.awt.geom.Rectangle2D) As Single()
			ensureCache()
			checkTextHit(hit)

			Return getCaretInfoTestInternal(hit, bounds)
		End Function

		' this version provides extra info in the float array
		' the first two values are as above
		' the next four values are the endpoints of the caret, as computed
		' using the hit character's offset (baseline + ssoffset) and
		' natural ascent and descent.
		' these  values are trimmed to the bounds where required to fit,
		' but otherwise independent of it.
		Private Function getCaretInfoTestInternal(ByVal hit As TextHitInfo, ByVal bounds As java.awt.geom.Rectangle2D) As Single()
			ensureCache()
			checkTextHit(hit)

			Dim info As Single() = New Single(5){}

			' get old data first
			getCaretInfo(hitToCaret(hit), bounds, info)

			' then add our new data
			Dim iangle, ixbase, p1x, p1y, p2x, p2y As Double

			Dim charix As Integer = hit.charIndex
			Dim lead As Boolean = hit.leadingEdge
			Dim ltr As Boolean = textLine.directionLTR
			Dim horiz As Boolean = Not vertical

			If charix = -1 OrElse charix = characterCount Then
				' !!! note: want non-shifted, baseline ascent and descent here!
				' TextLine should return appropriate line metrics object for these values
				Dim m As java.awt.font.TextLine.TextLineMetrics = textLine.metrics
				Dim low As Boolean = ltr = (charix = -1)
				iangle = 0
				If horiz Then
						p2x = If(low, 0, m.advance)
						p1x = p2x
					p1y = -m.ascent
					p2y = m.descent
				Else
						p2y = If(low, 0, m.advance)
						p1y = p2y
					p1x = m.descent
					p2x = m.ascent
				End If
			Else
				Dim thiscm As sun.font.CoreMetrics = textLine.getCoreMetricsAt(charix)
				iangle = thiscm.italicAngle
				ixbase = textLine.getCharLinePosition(charix, lead)
				If thiscm.baselineIndex < 0 Then
					' this is a graphic, no italics, use entire line height for caret
					Dim m As java.awt.font.TextLine.TextLineMetrics = textLine.metrics
					If horiz Then
							p2x = ixbase
							p1x = p2x
						If thiscm.baselineIndex = GraphicAttribute.TOP_ALIGNMENT Then
							p1y = -m.ascent
							p2y = p1y + thiscm.height
						Else
							p2y = m.descent
							p1y = p2y - thiscm.height
						End If
					Else
							p2y = ixbase
							p1y = p2y
						p1x = m.descent
						p2x = m.ascent
						' !!! top/bottom adjustment not implemented for vertical
					End If
				Else
					Dim bo As Single = baselineOffsets(thiscm.baselineIndex)
					If horiz Then
						ixbase += iangle * thiscm.ssOffset
						p1x = ixbase + iangle * thiscm.ascent
						p2x = ixbase - iangle * thiscm.descent
						p1y = bo - thiscm.ascent
						p2y = bo + thiscm.descent
					Else
						ixbase -= iangle * thiscm.ssOffset
						p1y = ixbase + iangle * thiscm.ascent
						p2y = ixbase - iangle * thiscm.descent
						p1x = bo + thiscm.ascent
						p2x = bo + thiscm.descent
					End If
				End If
			End If

			info(2) = CSng(p1x)
			info(3) = CSng(p1y)
			info(4) = CSng(p2x)
			info(5) = CSng(p2y)

			Return info
		End Function

		''' <summary>
		''' Returns information about the caret corresponding to <code>hit</code>.
		''' This method is a convenience overload of <code>getCaretInfo</code> and
		''' uses the natural bounds of this <code>TextLayout</code>. </summary>
		''' <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		''' <returns> the information about a caret corresponding to a hit.  The
		'''     returned caret info is in baseline-relative coordinates. </returns>
		Public Function getCaretInfo(ByVal hit As TextHitInfo) As Single()

			Return getCaretInfo(hit, naturalBounds)
		End Function

		''' <summary>
		''' Returns a caret index corresponding to <code>hit</code>.
		''' Carets are numbered from left to right (top to bottom) starting from
		''' zero. This always places carets next to the character hit, on the
		''' indicated side of the character. </summary>
		''' <param name="hit"> a hit on a character in this <code>TextLayout</code> </param>
		''' <returns> a caret index corresponding to the specified hit. </returns>
		Private Function hitToCaret(ByVal hit As TextHitInfo) As Integer

			Dim hitIndex As Integer = hit.charIndex

			If hitIndex < 0 Then
				Return If(textLine.directionLTR, 0, characterCount)
			ElseIf hitIndex >= characterCount Then
				Return If(textLine.directionLTR, characterCount, 0)
			End If

			Dim visIndex As Integer = textLine.logicalToVisual(hitIndex)

			If hit.leadingEdge <> textLine.isCharLTR(hitIndex) Then visIndex += 1

			Return visIndex
		End Function

		''' <summary>
		''' Given a caret index, return a hit whose caret is at the index.
		''' The hit is NOT guaranteed to be strong!!!
		''' </summary>
		''' <param name="caret"> a caret index. </param>
		''' <returns> a hit on this layout whose strong caret is at the requested
		''' index. </returns>
		Private Function caretToHit(ByVal caret As Integer) As TextHitInfo

			If caret = 0 OrElse caret = characterCount Then

				If (caret = characterCount) = textLine.directionLTR Then
					Return TextHitInfo.leading(characterCount)
				Else
					Return TextHitInfo.trailing(-1)
				End If
			Else

				Dim charIndex As Integer = textLine.visualToLogical(caret)
				Dim leading_Renamed As Boolean = textLine.isCharLTR(charIndex)

				Return If(leading_Renamed, TextHitInfo.leading(charIndex), TextHitInfo.trailing(charIndex))
			End If
		End Function

		Private Function caretIsValid(ByVal caret As Integer) As Boolean

			If caret = characterCount OrElse caret = 0 Then Return True

			Dim offset As Integer = textLine.visualToLogical(caret)

			If Not textLine.isCharLTR(offset) Then
				offset = textLine.visualToLogical(caret-1)
				If textLine.isCharLTR(offset) Then Return True
			End If

			' At this point, the leading edge of the character
			' at offset is at the given caret.

			Return textLine.caretAtOffsetIsValid(offset)
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the right (bottom); if there
		''' is no such hit, returns <code>null</code>.
		''' If the hit character index is out of bounds, an
		''' <seealso cref="IllegalArgumentException"/> is thrown. </summary>
		''' <param name="hit"> a hit on a character in this layout </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' right (bottom) of the caret of the provided hit or <code>null</code>. </returns>
		Public Function getNextRightHit(ByVal hit As TextHitInfo) As TextHitInfo
			ensureCache()
			checkTextHit(hit)

			Dim caret As Integer = hitToCaret(hit)

			If caret = characterCount Then Return Nothing

			Do
				caret += 1
			Loop While Not caretIsValid(caret)

			Return caretToHit(caret)
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the right (bottom); if no
		''' such hit, returns <code>null</code>.  The hit is to the right of
		''' the strong caret at the specified offset, as determined by the
		''' specified policy.
		''' The returned hit is the stronger of the two possible
		''' hits, as determined by the specified policy. </summary>
		''' <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		''' Cannot be less than 0 or greater than this <code>TextLayout</code>
		''' object's character count. </param>
		''' <param name="policy"> the policy used to select the strong caret </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' right (bottom) of the caret of the provided hit, or <code>null</code>. </returns>
		Public Function getNextRightHit(ByVal offset As Integer, ByVal policy As CaretPolicy) As TextHitInfo

			If offset < 0 OrElse offset > characterCount Then Throw New IllegalArgumentException("Offset out of bounds in TextLayout.getNextRightHit()")

			If policy Is Nothing Then Throw New IllegalArgumentException("Null CaretPolicy passed to TextLayout.getNextRightHit()")

			Dim hit1 As TextHitInfo = TextHitInfo.afterOffset(offset)
			Dim hit2 As TextHitInfo = hit1.otherHit

			Dim nextHit As TextHitInfo = getNextRightHit(policy.getStrongCaret(hit1, hit2, Me))

			If nextHit IsNot Nothing Then
				Dim otherHit As TextHitInfo = getVisualOtherHit(nextHit)
				Return policy.getStrongCaret(otherHit, nextHit, Me)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the right (bottom); if no
		''' such hit, returns <code>null</code>.  The hit is to the right of
		''' the strong caret at the specified offset, as determined by the
		''' default policy.
		''' The returned hit is the stronger of the two possible
		''' hits, as determined by the default policy. </summary>
		''' <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		''' Cannot be less than 0 or greater than the <code>TextLayout</code>
		''' object's character count. </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' right (bottom) of the caret of the provided hit, or <code>null</code>. </returns>
		Public Function getNextRightHit(ByVal offset As Integer) As TextHitInfo

			Return getNextRightHit(offset, DEFAULT_CARET_POLICY)
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the left (top); if no such
		''' hit, returns <code>null</code>.
		''' If the hit character index is out of bounds, an
		''' <code>IllegalArgumentException</code> is thrown. </summary>
		''' <param name="hit"> a hit on a character in this <code>TextLayout</code>. </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		Public Function getNextLeftHit(ByVal hit As TextHitInfo) As TextHitInfo
			ensureCache()
			checkTextHit(hit)

			Dim caret As Integer = hitToCaret(hit)

			If caret = 0 Then Return Nothing

			Do
				caret -= 1
			Loop While Not caretIsValid(caret)

			Return caretToHit(caret)
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the left (top); if no
		''' such hit, returns <code>null</code>.  The hit is to the left of
		''' the strong caret at the specified offset, as determined by the
		''' specified policy.
		''' The returned hit is the stronger of the two possible
		''' hits, as determined by the specified policy. </summary>
		''' <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		''' Cannot be less than 0 or greater than this <code>TextLayout</code>
		''' object's character count. </param>
		''' <param name="policy"> the policy used to select the strong caret </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		Public Function getNextLeftHit(ByVal offset As Integer, ByVal policy As CaretPolicy) As TextHitInfo

			If policy Is Nothing Then Throw New IllegalArgumentException("Null CaretPolicy passed to TextLayout.getNextLeftHit()")

			If offset < 0 OrElse offset > characterCount Then Throw New IllegalArgumentException("Offset out of bounds in TextLayout.getNextLeftHit()")

			Dim hit1 As TextHitInfo = TextHitInfo.afterOffset(offset)
			Dim hit2 As TextHitInfo = hit1.otherHit

			Dim nextHit As TextHitInfo = getNextLeftHit(policy.getStrongCaret(hit1, hit2, Me))

			If nextHit IsNot Nothing Then
				Dim otherHit As TextHitInfo = getVisualOtherHit(nextHit)
				Return policy.getStrongCaret(otherHit, nextHit, Me)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the hit for the next caret to the left (top); if no
		''' such hit, returns <code>null</code>.  The hit is to the left of
		''' the strong caret at the specified offset, as determined by the
		''' default policy.
		''' The returned hit is the stronger of the two possible
		''' hits, as determined by the default policy. </summary>
		''' <param name="offset"> an insertion offset in this <code>TextLayout</code>.
		''' Cannot be less than 0 or greater than this <code>TextLayout</code>
		''' object's character count. </param>
		''' <returns> a hit whose caret appears at the next position to the
		''' left (top) of the caret of the provided hit, or <code>null</code>. </returns>
		Public Function getNextLeftHit(ByVal offset As Integer) As TextHitInfo

			Return getNextLeftHit(offset, DEFAULT_CARET_POLICY)
		End Function

		''' <summary>
		''' Returns the hit on the opposite side of the specified hit's caret. </summary>
		''' <param name="hit"> the specified hit </param>
		''' <returns> a hit that is on the opposite side of the specified hit's
		'''    caret. </returns>
		Public Function getVisualOtherHit(ByVal hit As TextHitInfo) As TextHitInfo

			ensureCache()
			checkTextHit(hit)

			Dim hitCharIndex As Integer = hit.charIndex

			Dim charIndex As Integer
			Dim leading_Renamed As Boolean

			If hitCharIndex = -1 OrElse hitCharIndex = characterCount Then

				Dim visIndex As Integer
				If textLine.directionLTR = (hitCharIndex = -1) Then
					visIndex = 0
				Else
					visIndex = characterCount-1
				End If

				charIndex = textLine.visualToLogical(visIndex)

				If textLine.directionLTR = (hitCharIndex = -1) Then
					' at left end
					leading_Renamed = textLine.isCharLTR(charIndex)
				Else
					' at right end
					leading_Renamed = Not textLine.isCharLTR(charIndex)
				End If
			Else

				Dim visIndex As Integer = textLine.logicalToVisual(hitCharIndex)

				Dim movedToRight As Boolean
				If textLine.isCharLTR(hitCharIndex) = hit.leadingEdge Then
					visIndex -= 1
					movedToRight = False
				Else
					visIndex += 1
					movedToRight = True
				End If

				If visIndex > -1 AndAlso visIndex < characterCount Then
					charIndex = textLine.visualToLogical(visIndex)
					leading_Renamed = movedToRight = textLine.isCharLTR(charIndex)
				Else
					charIndex = If(movedToRight = textLine.directionLTR, characterCount, -1)
					leading_Renamed = charIndex = characterCount
				End If
			End If

			Return If(leading_Renamed, TextHitInfo.leading(charIndex), TextHitInfo.trailing(charIndex))
		End Function

		Private Function getCaretPath(ByVal hit As TextHitInfo, ByVal bounds As java.awt.geom.Rectangle2D) As Double()
			Dim info As Single() = getCaretInfo(hit, bounds)
			Return New Double() { info(2), info(3), info(4), info(5) }
		End Function

		''' <summary>
		''' Return an array of four floats corresponding the endpoints of the caret
		''' x0, y0, x1, y1.
		''' 
		''' This creates a line along the slope of the caret intersecting the
		''' baseline at the caret
		''' position, and extending from ascent above the baseline to descent below
		''' it.
		''' </summary>
		Private Function getCaretPath(ByVal caret As Integer, ByVal bounds As java.awt.geom.Rectangle2D, ByVal clipToBounds As Boolean) As Double()

			Dim info As Single() = getCaretInfo(caret, bounds, Nothing)

			Dim pos As Double = info(0)
			Dim slope As Double = info(1)

			Dim x0, y0, x1, y1 As Double
			Dim x2 As Double = -3141.59, y2 As Double = -2.7 ' values are there to make compiler happy

			Dim left As Double = bounds.x
			Dim right As Double = left + bounds.width
			Dim top As Double = bounds.y
			Dim bottom As Double = top + bounds.height

			Dim threePoints As Boolean = False

			If isVerticalLine Then

				If slope >= 0 Then
					x0 = left
					x1 = right
				Else
					x1 = left
					x0 = right
				End If

				y0 = pos + x0 * slope
				y1 = pos + x1 * slope

				' y0 <= y1, always

				If clipToBounds Then
					If y0 < top Then
						If slope <= 0 OrElse y1 <= top Then
								y1 = top
								y0 = y1
						Else
							threePoints = True
							y0 = top
							y2 = top
							x2 = x1 + (top-y1)/slope
							If y1 > bottom Then y1 = bottom
						End If
					ElseIf y1 > bottom Then
						If slope >= 0 OrElse y0 >= bottom Then
								y1 = bottom
								y0 = y1
						Else
							threePoints = True
							y1 = bottom
							y2 = bottom
							x2 = x0 + (bottom-x1)/slope
						End If
					End If
				End If

			Else

				If slope >= 0 Then
					y0 = bottom
					y1 = top
				Else
					y1 = bottom
					y0 = top
				End If

				x0 = pos - y0 * slope
				x1 = pos - y1 * slope

				' x0 <= x1, always

				If clipToBounds Then
					If x0 < left Then
						If slope <= 0 OrElse x1 <= left Then
								x1 = left
								x0 = x1
						Else
							threePoints = True
							x0 = left
							x2 = left
							y2 = y1 - (left-x1)/slope
							If x1 > right Then x1 = right
						End If
					ElseIf x1 > right Then
						If slope >= 0 OrElse x0 >= right Then
								x1 = right
								x0 = x1
						Else
							threePoints = True
							x1 = right
							x2 = right
							y2 = y0 - (right-x0)/slope
						End If
					End If
				End If
			End If

			Return If(threePoints, New Double() { x0, y0, x2, y2, x1, y1 }, New Double){ x0, y0, x1, y1 }
		End Function


		Private Shared Function pathToShape(ByVal path As Double(), ByVal close As Boolean, ByVal lp As sun.font.LayoutPathImpl) As java.awt.geom.GeneralPath
			Dim result As New java.awt.geom.GeneralPath(java.awt.geom.GeneralPath.WIND_EVEN_ODD, path.Length)
			result.moveTo(CSng(path(0)), CSng(path(1)))
			For i As Integer = 2 To path.Length - 1 Step 2
				result.lineTo(CSng(path(i)), CSng(path(i+1)))
			Next i
			If close Then result.closePath()

			If lp IsNot Nothing Then result = CType(lp.mapShape(result), java.awt.geom.GeneralPath)
			Return result
		End Function

		''' <summary>
		''' Returns a <seealso cref="Shape"/> representing the caret at the specified
		''' hit inside the specified bounds. </summary>
		''' <param name="hit"> the hit at which to generate the caret </param>
		''' <param name="bounds"> the bounds of the <code>TextLayout</code> to use
		'''    in generating the caret.  The bounds is in baseline-relative
		'''    coordinates. </param>
		''' <returns> a <code>Shape</code> representing the caret.  The returned
		'''    shape is in standard coordinates. </returns>
		Public Function getCaretShape(ByVal hit As TextHitInfo, ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.Shape
			ensureCache()
			checkTextHit(hit)

			If bounds Is Nothing Then Throw New IllegalArgumentException("Null Rectangle2D passed to TextLayout.getCaret()")

			Return pathToShape(getCaretPath(hit, bounds), False, textLine.layoutPath)
		End Function

		''' <summary>
		''' Returns a <code>Shape</code> representing the caret at the specified
		''' hit inside the natural bounds of this <code>TextLayout</code>. </summary>
		''' <param name="hit"> the hit at which to generate the caret </param>
		''' <returns> a <code>Shape</code> representing the caret.  The returned
		'''     shape is in standard coordinates. </returns>
		Public Function getCaretShape(ByVal hit As TextHitInfo) As java.awt.Shape

			Return getCaretShape(hit, naturalBounds)
		End Function

		''' <summary>
		''' Return the "stronger" of the TextHitInfos.  The TextHitInfos
		''' should be logical or visual counterparts.  They are not
		''' checked for validity.
		''' </summary>
		Private Function getStrongHit(ByVal hit1 As TextHitInfo, ByVal hit2 As TextHitInfo) As TextHitInfo

			' right now we're using the following rule for strong hits:
			' A hit on a character with a lower level
			' is stronger than one on a character with a higher level.
			' If this rule ties, the hit on the leading edge of a character wins.
			' If THIS rule ties, hit1 wins.  Both rules shouldn't tie, unless the
			' infos aren't counterparts of some sort.

			Dim hit1Level As SByte = getCharacterLevel(hit1.charIndex)
			Dim hit2Level As SByte = getCharacterLevel(hit2.charIndex)

			If hit1Level = hit2Level Then
				If hit2.leadingEdge AndAlso (Not hit1.leadingEdge) Then
					Return hit2
				Else
					Return hit1
				End If
			Else
				Return If(hit1Level < hit2Level, hit1, hit2)
			End If
		End Function

		''' <summary>
		''' Returns the level of the character at <code>index</code>.
		''' Indices -1 and <code>characterCount</code> are assigned the base
		''' level of this <code>TextLayout</code>. </summary>
		''' <param name="index"> the index of the character from which to get the level </param>
		''' <returns> the level of the character at the specified index. </returns>
		Public Function getCharacterLevel(ByVal index As Integer) As SByte

			' hmm, allow indices at endpoints?  For now, yes.
			If index < -1 OrElse index > characterCount Then Throw New IllegalArgumentException("Index is out of range in getCharacterLevel.")

			ensureCache()
			If index = -1 OrElse index = characterCount Then Return CByte(If(textLine.directionLTR, 0, 1))

			Return textLine.getCharLevel(index)
		End Function

		''' <summary>
		''' Returns two paths corresponding to the strong and weak caret. </summary>
		''' <param name="offset"> an offset in this <code>TextLayout</code> </param>
		''' <param name="bounds"> the bounds to which to extend the carets.  The
		''' bounds is in baseline-relative coordinates. </param>
		''' <param name="policy"> the specified <code>CaretPolicy</code> </param>
		''' <returns> an array of two paths.  Element zero is the strong
		''' caret.  If there are two carets, element one is the weak caret,
		''' otherwise it is <code>null</code>. The returned shapes
		''' are in standard coordinates. </returns>
		Public Function getCaretShapes(ByVal offset As Integer, ByVal bounds As java.awt.geom.Rectangle2D, ByVal policy As CaretPolicy) As java.awt.Shape()

			ensureCache()

			If offset < 0 OrElse offset > characterCount Then Throw New IllegalArgumentException("Offset out of bounds in TextLayout.getCaretShapes()")

			If bounds Is Nothing Then Throw New IllegalArgumentException("Null Rectangle2D passed to TextLayout.getCaretShapes()")

			If policy Is Nothing Then Throw New IllegalArgumentException("Null CaretPolicy passed to TextLayout.getCaretShapes()")

			Dim result As java.awt.Shape() = New java.awt.Shape(1){}

			Dim hit As TextHitInfo = TextHitInfo.afterOffset(offset)

			Dim hitCaret As Integer = hitToCaret(hit)

			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			Dim hitShape As java.awt.Shape = pathToShape(getCaretPath(hit, bounds), False, lp)
			Dim otherHit As TextHitInfo = hit.otherHit
			Dim otherCaret As Integer = hitToCaret(otherHit)

			If hitCaret = otherCaret Then
				result(0) = hitShape
			Else ' more than one caret
				Dim otherShape As java.awt.Shape = pathToShape(getCaretPath(otherHit, bounds), False, lp)

				Dim strongHit_Renamed As TextHitInfo = policy.getStrongCaret(hit, otherHit, Me)
				Dim hitIsStrong As Boolean = strongHit_Renamed.Equals(hit)

				If hitIsStrong Then ' then other is weak
					result(0) = hitShape
					result(1) = otherShape
				Else
					result(0) = otherShape
					result(1) = hitShape
				End If
			End If

			Return result
		End Function

		''' <summary>
		''' Returns two paths corresponding to the strong and weak caret.
		''' This method is a convenience overload of <code>getCaretShapes</code>
		''' that uses the default caret policy. </summary>
		''' <param name="offset"> an offset in this <code>TextLayout</code> </param>
		''' <param name="bounds"> the bounds to which to extend the carets.  This is
		'''     in baseline-relative coordinates. </param>
		''' <returns> two paths corresponding to the strong and weak caret as
		'''    defined by the <code>DEFAULT_CARET_POLICY</code>.  These are
		'''    in standard coordinates. </returns>
		Public Function getCaretShapes(ByVal offset As Integer, ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.Shape()
			' {sfb} parameter checking is done in overloaded version
			Return getCaretShapes(offset, bounds, DEFAULT_CARET_POLICY)
		End Function

		''' <summary>
		''' Returns two paths corresponding to the strong and weak caret.
		''' This method is a convenience overload of <code>getCaretShapes</code>
		''' that uses the default caret policy and this <code>TextLayout</code>
		''' object's natural bounds. </summary>
		''' <param name="offset"> an offset in this <code>TextLayout</code> </param>
		''' <returns> two paths corresponding to the strong and weak caret as
		'''    defined by the <code>DEFAULT_CARET_POLICY</code>.  These are
		'''    in standard coordinates. </returns>
		Public Function getCaretShapes(ByVal offset As Integer) As java.awt.Shape()
			' {sfb} parameter checking is done in overloaded version
			Return getCaretShapes(offset, naturalBounds, DEFAULT_CARET_POLICY)
		End Function

		' A utility to return a path enclosing the given path
		' Path0 must be left or top of path1
		' {jbr} no assumptions about size of path0, path1 anymore.
		Private Function boundingShape(ByVal path0 As Double(), ByVal path1 As Double()) As java.awt.geom.GeneralPath

			' Really, we want the path to be a convex hull around all of the
			' points in path0 and path1.  But we can get by with less than
			' that.  We do need to prevent the two segments which
			' join path0 to path1 from crossing each other.  So, if we
			' traverse path0 from top to bottom, we'll traverse path1 from
			' bottom to top (and vice versa).

			Dim result As java.awt.geom.GeneralPath = pathToShape(path0, False, Nothing)

			Dim sameDirection As Boolean

			If isVerticalLine Then
				sameDirection = (path0(1) > path0(path0.Length-1)) = (path1(1) > path1(path1.Length-1))
			Else
				sameDirection = (path0(0) > path0(path0.Length-2)) = (path1(0) > path1(path1.Length-2))
			End If

			Dim start As Integer
			Dim limit As Integer
			Dim increment As Integer

			If sameDirection Then
				start = path1.Length-2
				limit = -2
				increment = -2
			Else
				start = 0
				limit = path1.Length
				increment = 2
			End If

			Dim i As Integer = start
			Do While i <> limit
				result.lineTo(CSng(path1(i)), CSng(path1(i+1)))
				i += increment
			Loop

			result.closePath()

			Return result
		End Function

		' A utility to convert a pair of carets into a bounding path
		' {jbr} Shape is never outside of bounds.
		Private Function caretBoundingShape(ByVal caret0 As Integer, ByVal caret1 As Integer, ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.geom.GeneralPath

			If caret0 > caret1 Then
				Dim temp As Integer = caret0
				caret0 = caret1
				caret1 = temp
			End If

			Return boundingShape(getCaretPath(caret0, bounds, True), getCaretPath(caret1, bounds, True))
		End Function

	'    
	'     * A utility to return the path bounding the area to the left (top) of the
	'     * layout.
	'     * Shape is never outside of bounds.
	'     
		Private Function leftShape(ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.geom.GeneralPath

			Dim path0 As Double()
			If isVerticalLine Then
				path0 = New Double() { bounds.x, bounds.y, bounds.x + bounds.width, bounds.y }
			Else
				path0 = New Double() { bounds.x, bounds.y + bounds.height, bounds.x, bounds.y }
			End If

			Dim path1 As Double() = getCaretPath(0, bounds, True)

			Return boundingShape(path0, path1)
		End Function

	'    
	'     * A utility to return the path bounding the area to the right (bottom) of
	'     * the layout.
	'     
		Private Function rightShape(ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.geom.GeneralPath
			Dim path1 As Double()
			If isVerticalLine Then
				path1 = New Double() { bounds.x, bounds.y + bounds.height, bounds.x + bounds.width, bounds.y + bounds.height }
			Else
				path1 = New Double() { bounds.x + bounds.width, bounds.y + bounds.height, bounds.x + bounds.width, bounds.y }
			End If

			Dim path0 As Double() = getCaretPath(characterCount, bounds, True)

			Return boundingShape(path0, path1)
		End Function

		''' <summary>
		''' Returns the logical ranges of text corresponding to a visual selection. </summary>
		''' <param name="firstEndpoint"> an endpoint of the visual range </param>
		''' <param name="secondEndpoint"> the other endpoint of the visual range.
		''' This endpoint can be less than <code>firstEndpoint</code>. </param>
		''' <returns> an array of integers representing start/limit pairs for the
		''' selected ranges. </returns>
		''' <seealso cref= #getVisualHighlightShape(TextHitInfo, TextHitInfo, Rectangle2D) </seealso>
		Public Function getLogicalRangesForVisualSelection(ByVal firstEndpoint As TextHitInfo, ByVal secondEndpoint As TextHitInfo) As Integer()
			ensureCache()

			checkTextHit(firstEndpoint)
			checkTextHit(secondEndpoint)

			' !!! probably want to optimize for all LTR text

			Dim included As Boolean() = New Boolean(characterCount - 1){}

			Dim startIndex As Integer = hitToCaret(firstEndpoint)
			Dim limitIndex As Integer = hitToCaret(secondEndpoint)

			If startIndex > limitIndex Then
				Dim t As Integer = startIndex
				startIndex = limitIndex
				limitIndex = t
			End If

	'        
	'         * now we have the visual indexes of the glyphs at the start and limit
	'         * of the selection range walk through runs marking characters that
	'         * were included in the visual range there is probably a more efficient
	'         * way to do this, but this ought to work, so hey
	'         

			If startIndex < limitIndex Then
				Dim visIndex As Integer = startIndex
				Do While visIndex < limitIndex
					included(textLine.visualToLogical(visIndex)) = True
					visIndex += 1
				Loop
			End If

	'        
	'         * count how many runs we have, ought to be one or two, but perhaps
	'         * things are especially weird
	'         
			Dim count As Integer = 0
			Dim inrun As Boolean = False
			For i As Integer = 0 To characterCount - 1
				If included(i) <> inrun Then
					inrun = Not inrun
					If inrun Then count += 1
				End If
			Next i

			Dim ranges As Integer() = New Integer(count * 2 - 1){}
			count = 0
			inrun = False
			For i As Integer = 0 To characterCount - 1
				If included(i) <> inrun Then
					ranges(count) = i
					count += 1
					inrun = Not inrun
				End If
			Next i
			If inrun Then
				ranges(count) = characterCount
				count += 1
			End If

			Return ranges
		End Function

		''' <summary>
		''' Returns a path enclosing the visual selection in the specified range,
		''' extended to <code>bounds</code>.
		''' <p>
		''' If the selection includes the leftmost (topmost) position, the selection
		''' is extended to the left (top) of <code>bounds</code>.  If the
		''' selection includes the rightmost (bottommost) position, the selection
		''' is extended to the right (bottom) of the bounds.  The height
		''' (width on vertical lines) of the selection is always extended to
		''' <code>bounds</code>.
		''' <p>
		''' Although the selection is always contiguous, the logically selected
		''' text can be discontiguous on lines with mixed-direction text.  The
		''' logical ranges of text selected can be retrieved using
		''' <code>getLogicalRangesForVisualSelection</code>.  For example,
		''' consider the text 'ABCdef' where capital letters indicate
		''' right-to-left text, rendered on a right-to-left line, with a visual
		''' selection from 0L (the leading edge of 'A') to 3T (the trailing edge
		''' of 'd').  The text appears as follows, with bold underlined areas
		''' representing the selection:
		''' <br><pre>
		'''    d<u><b>efCBA  </b></u>
		''' </pre>
		''' The logical selection ranges are 0-3, 4-6 (ABC, ef) because the
		''' visually contiguous text is logically discontiguous.  Also note that
		''' since the rightmost position on the layout (to the right of 'A') is
		''' selected, the selection is extended to the right of the bounds. </summary>
		''' <param name="firstEndpoint"> one end of the visual selection </param>
		''' <param name="secondEndpoint"> the other end of the visual selection </param>
		''' <param name="bounds"> the bounding rectangle to which to extend the selection.
		'''     This is in baseline-relative coordinates. </param>
		''' <returns> a <code>Shape</code> enclosing the selection.  This is in
		'''     standard coordinates. </returns>
		''' <seealso cref= #getLogicalRangesForVisualSelection(TextHitInfo, TextHitInfo) </seealso>
		''' <seealso cref= #getLogicalHighlightShape(int, int, Rectangle2D) </seealso>
		Public Function getVisualHighlightShape(ByVal firstEndpoint As TextHitInfo, ByVal secondEndpoint As TextHitInfo, ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.Shape
			ensureCache()

			checkTextHit(firstEndpoint)
			checkTextHit(secondEndpoint)

			If bounds Is Nothing Then Throw New IllegalArgumentException("Null Rectangle2D passed to TextLayout.getVisualHighlightShape()")

			Dim result As New java.awt.geom.GeneralPath(java.awt.geom.GeneralPath.WIND_EVEN_ODD)

			Dim firstCaret As Integer = hitToCaret(firstEndpoint)
			Dim secondCaret As Integer = hitToCaret(secondEndpoint)

			result.append(caretBoundingShape(firstCaret, secondCaret, bounds), False)

			If firstCaret = 0 OrElse secondCaret = 0 Then
				Dim ls As java.awt.geom.GeneralPath = leftShape(bounds)
				If Not ls.bounds.empty Then result.append(ls, False)
			End If

			If firstCaret = characterCount OrElse secondCaret = characterCount Then
				Dim rs As java.awt.geom.GeneralPath = rightShape(bounds)
				If Not rs.bounds.empty Then result.append(rs, False)
			End If

			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			If lp IsNot Nothing Then result = CType(lp.mapShape(result), java.awt.geom.GeneralPath) ' dlf cast safe?

			Return result
		End Function

		''' <summary>
		''' Returns a <code>Shape</code> enclosing the visual selection in the
		''' specified range, extended to the bounds.  This method is a
		''' convenience overload of <code>getVisualHighlightShape</code> that
		''' uses the natural bounds of this <code>TextLayout</code>. </summary>
		''' <param name="firstEndpoint"> one end of the visual selection </param>
		''' <param name="secondEndpoint"> the other end of the visual selection </param>
		''' <returns> a <code>Shape</code> enclosing the selection.  This is
		'''     in standard coordinates. </returns>
		Public Function getVisualHighlightShape(ByVal firstEndpoint As TextHitInfo, ByVal secondEndpoint As TextHitInfo) As java.awt.Shape
			Return getVisualHighlightShape(firstEndpoint, secondEndpoint, naturalBounds)
		End Function

		''' <summary>
		''' Returns a <code>Shape</code> enclosing the logical selection in the
		''' specified range, extended to the specified <code>bounds</code>.
		''' <p>
		''' If the selection range includes the first logical character, the
		''' selection is extended to the portion of <code>bounds</code> before
		''' the start of this <code>TextLayout</code>.  If the range includes
		''' the last logical character, the selection is extended to the portion
		''' of <code>bounds</code> after the end of this <code>TextLayout</code>.
		''' The height (width on vertical lines) of the selection is always
		''' extended to <code>bounds</code>.
		''' <p>
		''' The selection can be discontiguous on lines with mixed-direction text.
		''' Only those characters in the logical range between start and limit
		''' appear selected.  For example, consider the text 'ABCdef' where capital
		''' letters indicate right-to-left text, rendered on a right-to-left line,
		''' with a logical selection from 0 to 4 ('ABCd').  The text appears as
		''' follows, with bold standing in for the selection, and underlining for
		''' the extension:
		''' <br><pre>
		'''    <u><b>d</b></u>ef<u><b>CBA  </b></u>
		''' </pre>
		''' The selection is discontiguous because the selected characters are
		''' visually discontiguous. Also note that since the range includes the
		''' first logical character (A), the selection is extended to the portion
		''' of the <code>bounds</code> before the start of the layout, which in
		''' this case (a right-to-left line) is the right portion of the
		''' <code>bounds</code>. </summary>
		''' <param name="firstEndpoint"> an endpoint in the range of characters to select </param>
		''' <param name="secondEndpoint"> the other endpoint of the range of characters
		''' to select. Can be less than <code>firstEndpoint</code>.  The range
		''' includes the character at min(firstEndpoint, secondEndpoint), but
		''' excludes max(firstEndpoint, secondEndpoint). </param>
		''' <param name="bounds"> the bounding rectangle to which to extend the selection.
		'''     This is in baseline-relative coordinates. </param>
		''' <returns> an area enclosing the selection.  This is in standard
		'''     coordinates. </returns>
		''' <seealso cref= #getVisualHighlightShape(TextHitInfo, TextHitInfo, Rectangle2D) </seealso>
		Public Function getLogicalHighlightShape(ByVal firstEndpoint As Integer, ByVal secondEndpoint As Integer, ByVal bounds As java.awt.geom.Rectangle2D) As java.awt.Shape
			If bounds Is Nothing Then Throw New IllegalArgumentException("Null Rectangle2D passed to TextLayout.getLogicalHighlightShape()")

			ensureCache()

			If firstEndpoint > secondEndpoint Then
				Dim t As Integer = firstEndpoint
				firstEndpoint = secondEndpoint
				secondEndpoint = t
			End If

			If firstEndpoint < 0 OrElse secondEndpoint > characterCount Then Throw New IllegalArgumentException("Range is invalid in TextLayout.getLogicalHighlightShape()")

			Dim result As New java.awt.geom.GeneralPath(java.awt.geom.GeneralPath.WIND_EVEN_ODD)

			Dim carets As Integer() = New Integer(9){} ' would this ever not handle all cases?
			Dim count As Integer = 0

			If firstEndpoint < secondEndpoint Then
				Dim logIndex As Integer = firstEndpoint
				Do
					carets(count) = hitToCaret(TextHitInfo.leading(logIndex))
					count += 1
					Dim ltr As Boolean = textLine.isCharLTR(logIndex)

					Do
						logIndex += 1
					Loop While logIndex < secondEndpoint AndAlso textLine.isCharLTR(logIndex) = ltr

					Dim hitCh As Integer = logIndex
					carets(count) = hitToCaret(TextHitInfo.trailing(hitCh - 1))
					count += 1

					If count = carets.Length Then
						Dim temp As Integer() = New Integer(carets.Length + 10 - 1){}
						Array.Copy(carets, 0, temp, 0, count)
						carets = temp
					End If
				Loop While logIndex < secondEndpoint
			Else
				count = 2
					carets(1) = hitToCaret(TextHitInfo.leading(firstEndpoint))
					carets(0) = carets(1)
			End If

			' now create paths for pairs of carets

			For i As Integer = 0 To count - 1 Step 2
				result.append(caretBoundingShape(carets(i), carets(i+1), bounds), False)
			Next i

			If firstEndpoint <> secondEndpoint Then
				If (textLine.directionLTR AndAlso firstEndpoint = 0) OrElse ((Not textLine.directionLTR) AndAlso secondEndpoint = characterCount) Then
					Dim ls As java.awt.geom.GeneralPath = leftShape(bounds)
					If Not ls.bounds.empty Then result.append(ls, False)
				End If

				If (textLine.directionLTR AndAlso secondEndpoint = characterCount) OrElse ((Not textLine.directionLTR) AndAlso firstEndpoint = 0) Then

					Dim rs As java.awt.geom.GeneralPath = rightShape(bounds)
					If Not rs.bounds.empty Then result.append(rs, False)
				End If
			End If

			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			If lp IsNot Nothing Then result = CType(lp.mapShape(result), java.awt.geom.GeneralPath) ' dlf cast safe?
			Return result
		End Function

		''' <summary>
		''' Returns a <code>Shape</code> enclosing the logical selection in the
		''' specified range, extended to the natural bounds of this
		''' <code>TextLayout</code>.  This method is a convenience overload of
		''' <code>getLogicalHighlightShape</code> that uses the natural bounds of
		''' this <code>TextLayout</code>. </summary>
		''' <param name="firstEndpoint"> an endpoint in the range of characters to select </param>
		''' <param name="secondEndpoint"> the other endpoint of the range of characters
		''' to select. Can be less than <code>firstEndpoint</code>.  The range
		''' includes the character at min(firstEndpoint, secondEndpoint), but
		''' excludes max(firstEndpoint, secondEndpoint). </param>
		''' <returns> a <code>Shape</code> enclosing the selection.  This is in
		'''     standard coordinates. </returns>
		Public Function getLogicalHighlightShape(ByVal firstEndpoint As Integer, ByVal secondEndpoint As Integer) As java.awt.Shape

			Return getLogicalHighlightShape(firstEndpoint, secondEndpoint, naturalBounds)
		End Function

		''' <summary>
		''' Returns the black box bounds of the characters in the specified range.
		''' The black box bounds is an area consisting of the union of the bounding
		''' boxes of all the glyphs corresponding to the characters between start
		''' and limit.  This area can be disjoint. </summary>
		''' <param name="firstEndpoint"> one end of the character range </param>
		''' <param name="secondEndpoint"> the other end of the character range.  Can be
		''' less than <code>firstEndpoint</code>. </param>
		''' <returns> a <code>Shape</code> enclosing the black box bounds.  This is
		'''     in standard coordinates. </returns>
		Public Function getBlackBoxBounds(ByVal firstEndpoint As Integer, ByVal secondEndpoint As Integer) As java.awt.Shape
			ensureCache()

			If firstEndpoint > secondEndpoint Then
				Dim t As Integer = firstEndpoint
				firstEndpoint = secondEndpoint
				secondEndpoint = t
			End If

			If firstEndpoint < 0 OrElse secondEndpoint > characterCount Then Throw New IllegalArgumentException("Invalid range passed to TextLayout.getBlackBoxBounds()")

	'        
	'         * return an area that consists of the bounding boxes of all the
	'         * characters from firstEndpoint to limit
	'         

			Dim result As New java.awt.geom.GeneralPath(java.awt.geom.GeneralPath.WIND_NON_ZERO)

			If firstEndpoint < characterCount Then
				For logIndex As Integer = firstEndpoint To secondEndpoint - 1

					Dim r As java.awt.geom.Rectangle2D = textLine.getCharBounds(logIndex)
					If Not r.empty Then result.append(r, False)
				Next logIndex
			End If

			If dx <> 0 OrElse dy <> 0 Then
				Dim tx As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getTranslateInstance(dx, dy)
				result = CType(tx.createTransformedShape(result), java.awt.geom.GeneralPath)
			End If
			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			If lp IsNot Nothing Then result = CType(lp.mapShape(result), java.awt.geom.GeneralPath)

			'return new Highlight(result, false);
			Return result
		End Function

		''' <summary>
		''' Returns the distance from the point (x,&nbsp;y) to the caret along
		''' the line direction defined in <code>caretInfo</code>.  Distance is
		''' negative if the point is to the left of the caret on a horizontal
		''' line, or above the caret on a vertical line.
		''' Utility for use by hitTestChar.
		''' </summary>
		Private Function caretToPointDistance(ByVal caretInfo As Single(), ByVal x As Single, ByVal y As Single) As Single
			' distanceOffBaseline is negative if you're 'above' baseline

			Dim lineDistance As Single = If(isVerticalLine, y, x)
			Dim distanceOffBaseline As Single = If(isVerticalLine, -x, y)

			Return lineDistance - caretInfo(0) + (distanceOffBaseline*caretInfo(1))
		End Function

		''' <summary>
		''' Returns a <code>TextHitInfo</code> corresponding to the
		''' specified point.
		''' Coordinates outside the bounds of the <code>TextLayout</code>
		''' map to hits on the leading edge of the first logical character,
		''' or the trailing edge of the last logical character, as appropriate,
		''' regardless of the position of that character in the line.  Only the
		''' direction along the baseline is used to make this evaluation. </summary>
		''' <param name="x"> the x offset from the origin of this
		'''     <code>TextLayout</code>.  This is in standard coordinates. </param>
		''' <param name="y"> the y offset from the origin of this
		'''     <code>TextLayout</code>.  This is in standard coordinates. </param>
		''' <param name="bounds"> the bounds of the <code>TextLayout</code>.  This
		'''     is in baseline-relative coordinates. </param>
		''' <returns> a hit describing the character and edge (leading or trailing)
		'''     under the specified point. </returns>
		Public Function hitTestChar(ByVal x As Single, ByVal y As Single, ByVal bounds As java.awt.geom.Rectangle2D) As TextHitInfo
			' check boundary conditions

			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			Dim prev As Boolean = False
			If lp IsNot Nothing Then
				Dim pt As New java.awt.geom.Point2D.Float(x, y)
				prev = lp.pointToPath(pt, pt)
				x = pt.x
				y = pt.y
			End If

			If vertical Then
				If y < bounds.minY Then
					Return TextHitInfo.leading(0)
				ElseIf y >= bounds.maxY Then
					Return TextHitInfo.trailing(characterCount-1)
				End If
			Else
				If x < bounds.minX Then
					Return If(leftToRight, TextHitInfo.leading(0), TextHitInfo.trailing(characterCount-1))
				ElseIf x >= bounds.maxX Then
					Return If(leftToRight, TextHitInfo.trailing(characterCount-1), TextHitInfo.leading(0))
				End If
			End If

			' revised hit test
			' the original seems too complex and fails miserably with italic offsets
			' the natural tendency is to move towards the character you want to hit
			' so we'll just measure distance to the center of each character's visual
			' bounds, pick the closest one, then see which side of the character's
			' center line (italic) the point is on.
			' this tends to make it easier to hit narrow characters, which can be a
			' bit odd if you're visually over an adjacent wide character. this makes
			' a difference with bidi, so perhaps i need to revisit this yet again.

			Dim distance As Double = java.lang.[Double].Max_Value
			Dim index As Integer = 0
			Dim trail As Integer = -1
			Dim lcm As sun.font.CoreMetrics = Nothing
			Dim icx As Single = 0, icy As Single = 0, ia As Single = 0, cy As Single = 0, dya As Single = 0, ydsq As Single = 0

			For i As Integer = 0 To characterCount - 1
				If Not textLine.caretAtOffsetIsValid(i) Then Continue For
				If trail = -1 Then trail = i
				Dim cm As sun.font.CoreMetrics = textLine.getCoreMetricsAt(i)
				If cm IsNot lcm Then
					lcm = cm
					' just work around baseline mess for now
					If cm.baselineIndex = GraphicAttribute.TOP_ALIGNMENT Then
						cy = -(textLine.metrics.ascent - cm.ascent) + cm.ssOffset
					ElseIf cm.baselineIndex = GraphicAttribute.BOTTOM_ALIGNMENT Then
						cy = textLine.metrics.descent - cm.descent + cm.ssOffset
					Else
						cy = cm.effectiveBaselineOffset(baselineOffsets) + cm.ssOffset
					End If
					Dim dy As Single = (cm.descent - cm.ascent) / 2 - cy
					dya = dy * cm.italicAngle
					cy += dy
					ydsq = (cy - y)*(cy - y)
				End If
				Dim cx As Single = textLine.getCharXPosition(i)
				Dim ca As Single = textLine.getCharAdvance(i)
				Dim dx As Single = ca / 2
				cx += dx - dya

				' proximity in x (along baseline) is two times as important as proximity in y
				Dim nd As Double = System.Math.Sqrt(4*(cx - x)*(cx - x) + ydsq)
				If nd < distance Then
					distance = nd
					index = i
					trail = -1
					icx = cx
					icy = cy
					ia = cm.italicAngle
				End If
			Next i
			Dim left As Boolean = x < icx - (y - icy) * ia
			Dim leading_Renamed As Boolean = textLine.isCharLTR(index) = left
			If trail = -1 Then trail = characterCount
			Dim result As TextHitInfo = If(leading_Renamed, TextHitInfo.leading(index), TextHitInfo.trailing(trail-1))
			Return result
		End Function

		''' <summary>
		''' Returns a <code>TextHitInfo</code> corresponding to the
		''' specified point.  This method is a convenience overload of
		''' <code>hitTestChar</code> that uses the natural bounds of this
		''' <code>TextLayout</code>. </summary>
		''' <param name="x"> the x offset from the origin of this
		'''     <code>TextLayout</code>.  This is in standard coordinates. </param>
		''' <param name="y"> the y offset from the origin of this
		'''     <code>TextLayout</code>.  This is in standard coordinates. </param>
		''' <returns> a hit describing the character and edge (leading or trailing)
		''' under the specified point. </returns>
		Public Function hitTestChar(ByVal x As Single, ByVal y As Single) As TextHitInfo

			Return hitTestChar(x, y, naturalBounds)
		End Function

		''' <summary>
		''' Returns the hash code of this <code>TextLayout</code>. </summary>
		''' <returns> the hash code of this <code>TextLayout</code>. </returns>
		Public Overrides Function GetHashCode() As Integer
			If hashCodeCache = 0 Then
				ensureCache()
				hashCodeCache = textLine.GetHashCode()
			End If
			Return hashCodeCache
		End Function

		''' <summary>
		''' Returns <code>true</code> if the specified <code>Object</code> is a
		''' <code>TextLayout</code> object and if the specified <code>Object</code>
		''' equals this <code>TextLayout</code>. </summary>
		''' <param name="obj"> an <code>Object</code> to test for equality </param>
		''' <returns> <code>true</code> if the specified <code>Object</code>
		'''      equals this <code>TextLayout</code>; <code>false</code>
		'''      otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return (TypeOf obj Is TextLayout) AndAlso Equals(CType(obj, TextLayout))
		End Function

		''' <summary>
		''' Returns <code>true</code> if the two layouts are equal.
		''' Two layouts are equal if they contain equal glyphvectors in the same order. </summary>
		''' <param name="rhs"> the <code>TextLayout</code> to compare to this
		'''       <code>TextLayout</code> </param>
		''' <returns> <code>true</code> if the specified <code>TextLayout</code>
		'''      equals this <code>TextLayout</code>.
		'''  </returns>
		Public Overrides Function Equals(ByVal rhs As TextLayout) As Boolean

			If rhs Is Nothing Then Return False
			If rhs Is Me Then Return True

			ensureCache()
			Return textLine.Equals(rhs.textLine)
		End Function

		''' <summary>
		''' Returns debugging information for this <code>TextLayout</code>. </summary>
		''' <returns> the <code>textLine</code> of this <code>TextLayout</code>
		'''        as a <code>String</code>. </returns>
		Public Overrides Function ToString() As String
			ensureCache()
			Return textLine.ToString()
		End Function

		''' <summary>
		''' Renders this <code>TextLayout</code> at the specified location in
		''' the specified <seealso cref="java.awt.Graphics2D Graphics2D"/> context.
		''' The origin of the layout is placed at x,&nbsp;y.  Rendering may touch
		''' any point within <code>getBounds()</code> of this position.  This
		''' leaves the <code>g2</code> unchanged.  Text is rendered along the
		''' baseline path. </summary>
		''' <param name="g2"> the <code>Graphics2D</code> context into which to render
		'''         the layout </param>
		''' <param name="x"> the X coordinate of the origin of this <code>TextLayout</code> </param>
		''' <param name="y"> the Y coordinate of the origin of this <code>TextLayout</code> </param>
		''' <seealso cref= #getBounds() </seealso>
		Public Sub draw(ByVal g2 As java.awt.Graphics2D, ByVal x As Single, ByVal y As Single)

			If g2 Is Nothing Then Throw New IllegalArgumentException("Null Graphics2D passed to TextLayout.draw()")

			textLine.draw(g2, x - dx, y - dy)
		End Sub

		''' <summary>
		''' Package-only method for testing ONLY.  Please don't abuse.
		''' </summary>
		Friend Property textLineForTesting As TextLine
			Get
    
				Return textLine
			End Get
		End Property

		''' 
		''' <summary>
		''' Return the index of the first character with a different baseline from the
		''' character at start, or limit if all characters between start and limit have
		''' the same baseline.
		''' </summary>
		Private Shared Function sameBaselineUpTo(ByVal font_Renamed As java.awt.Font, ByVal text As Char(), ByVal start As Integer, ByVal limit As Integer) As Integer
			' current implementation doesn't support multiple baselines
			Return limit
	'        
	'        byte bl = font.getBaselineFor(text[start++]);
	'        while (start < limit && font.getBaselineFor(text[start]) == bl) {
	'            ++start;
	'        }
	'        return start;
	'        
		End Function

		Friend Shared Function getBaselineFromGraphic(ByVal graphic As GraphicAttribute) As SByte

			Dim alignment As SByte = CByte(graphic.alignment)

			If alignment = GraphicAttribute.BOTTOM_ALIGNMENT OrElse alignment = GraphicAttribute.TOP_ALIGNMENT Then

				Return CByte(GraphicAttribute.ROMAN_BASELINE)
			Else
				Return alignment
			End If
		End Function

		''' <summary>
		''' Returns a <code>Shape</code> representing the outline of this
		''' <code>TextLayout</code>. </summary>
		''' <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
		'''     outline of this <code>TextLayout</code>. </param>
		''' <returns> a <code>Shape</code> that is the outline of this
		'''     <code>TextLayout</code>.  This is in standard coordinates. </returns>
		Public Function getOutline(ByVal tx As java.awt.geom.AffineTransform) As java.awt.Shape
			ensureCache()
			Dim result As java.awt.Shape = textLine.getOutline(tx)
			Dim lp As sun.font.LayoutPathImpl = textLine.layoutPath
			If lp IsNot Nothing Then result = lp.mapShape(result)
			Return result
		End Function

		''' <summary>
		''' Return the LayoutPath, or null if the layout path is the
		''' default path (x maps to advance, y maps to offset). </summary>
		''' <returns> the layout path
		''' @since 1.6 </returns>
		Public Property layoutPath As LayoutPath
			Get
				Return textLine.layoutPath
			End Get
		End Property

	   ''' <summary>
	   ''' Convert a hit to a point in standard coordinates.  The point is
	   ''' on the baseline of the character at the leading or trailing
	   ''' edge of the character, as appropriate.  If the path is
	   ''' broken at the side of the character represented by the hit, the
	   ''' point will be adjacent to the character. </summary>
	   ''' <param name="hit"> the hit to check.  This must be a valid hit on
	   ''' the TextLayout. </param>
	   ''' <param name="point"> the returned point. The point is in standard
	   '''     coordinates. </param>
	   ''' <exception cref="IllegalArgumentException"> if the hit is not valid for the
	   ''' TextLayout. </exception>
	   ''' <exception cref="NullPointerException"> if hit or point is null.
	   ''' @since 1.6 </exception>
		Public Sub hitToPoint(ByVal hit As TextHitInfo, ByVal point As java.awt.geom.Point2D)
			If hit Is Nothing OrElse point Is Nothing Then Throw New NullPointerException((If(hit Is Nothing, "hit", "point")) & " can't be null")
			ensureCache()
			checkTextHit(hit)

			Dim adv As Single = 0
			Dim [off] As Single = 0

			Dim ix As Integer = hit.charIndex
			Dim leading_Renamed As Boolean = hit.leadingEdge
			Dim ltr As Boolean
			If ix = -1 OrElse ix = textLine.characterCount() Then
				ltr = textLine.directionLTR
				adv = If(ltr = (ix = -1), 0, lineMetrics.advance)
			Else
				ltr = textLine.isCharLTR(ix)
				adv = textLine.getCharLinePosition(ix, leading_Renamed)
				[off] = textLine.getCharYPosition(ix)
			End If
			point.locationion(adv, [off])
			Dim lp As LayoutPath = textLine.layoutPath
			If lp IsNot Nothing Then lp.pathToPoint(point, ltr <> leading_Renamed, point)
		End Sub
	End Class

End Namespace