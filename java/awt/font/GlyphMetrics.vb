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
	''' The <code>GlyphMetrics</code> class represents information for a
	''' single glyph.   A glyph is the visual representation of one or more
	''' characters.  Many different glyphs can be used to represent a single
	''' character or combination of characters.  <code>GlyphMetrics</code>
	''' instances are produced by <seealso cref="java.awt.Font Font"/> and are applicable
	''' to a specific glyph in a particular <code>Font</code>.
	''' <p>
	''' Glyphs are either STANDARD, LIGATURE, COMBINING, or COMPONENT.
	''' <ul>
	''' <li>STANDARD glyphs are commonly used to represent single characters.
	''' <li>LIGATURE glyphs are used to represent sequences of characters.
	''' <li>COMPONENT glyphs in a <seealso cref="GlyphVector"/> do not correspond to a
	''' particular character in a text model. Instead, COMPONENT glyphs are
	''' added for typographical reasons, such as Arabic justification.
	''' <li>COMBINING glyphs embellish STANDARD or LIGATURE glyphs, such
	''' as accent marks.  Carets do not appear before COMBINING glyphs.
	''' </ul>
	''' <p>
	''' Other metrics available through <code>GlyphMetrics</code> are the
	''' components of the advance, the visual bounds, and the left and right
	''' side bearings.
	''' <p>
	''' Glyphs for a rotated font, or obtained from a <code>GlyphVector</code>
	''' which has applied a rotation to the glyph, can have advances that
	''' contain both X and Y components.  Usually the advance only has one
	''' component.
	''' <p>
	''' The advance of a glyph is the distance from the glyph's origin to the
	''' origin of the next glyph along the baseline, which is either vertical
	''' or horizontal.  Note that, in a <code>GlyphVector</code>,
	''' the distance from a glyph to its following glyph might not be the
	''' glyph's advance, because of kerning or other positioning adjustments.
	''' <p>
	''' The bounds is the smallest rectangle that completely contains the
	''' outline of the glyph.  The bounds rectangle is relative to the
	''' glyph's origin.  The left-side bearing is the distance from the glyph
	''' origin to the left of its bounds rectangle. If the left-side bearing is
	''' negative, part of the glyph is drawn to the left of its origin.  The
	''' right-side bearing is the distance from the right side of the bounds
	''' rectangle to the next glyph origin (the origin plus the advance).  If
	''' negative, part of the glyph is drawn to the right of the next glyph's
	''' origin.  Note that the bounds does not necessarily enclose all the pixels
	''' affected when rendering the glyph, because of rasterization and pixel
	''' adjustment effects.
	''' <p>
	''' Although instances of <code>GlyphMetrics</code> can be directly
	''' constructed, they are almost always obtained from a
	''' <code>GlyphVector</code>.  Once constructed, <code>GlyphMetrics</code>
	''' objects are immutable.
	''' <p>
	''' <strong>Example</strong>:<p>
	''' Querying a <code>Font</code> for glyph information
	''' <blockquote><pre>
	''' Font font = ...;
	''' int glyphIndex = ...;
	''' GlyphMetrics metrics = GlyphVector.getGlyphMetrics(glyphIndex);
	''' int isStandard = metrics.isStandard();
	''' float glyphAdvance = metrics.getAdvance();
	''' </pre></blockquote> </summary>
	''' <seealso cref= java.awt.Font </seealso>
	''' <seealso cref= GlyphVector </seealso>

	Public NotInheritable Class GlyphMetrics
		''' <summary>
		''' Indicates whether the metrics are for a horizontal or vertical baseline.
		''' </summary>
		Private horizontal As Boolean

		''' <summary>
		''' The x-component of the advance.
		''' </summary>
		Private advanceX As Single

		''' <summary>
		''' The y-component of the advance.
		''' </summary>
		Private advanceY As Single

		''' <summary>
		''' The bounds of the associated glyph.
		''' </summary>
		Private bounds As java.awt.geom.Rectangle2D.Float

		''' <summary>
		''' Additional information about the glyph encoded as a byte.
		''' </summary>
		Private glyphType As SByte

		''' <summary>
		''' Indicates a glyph that represents a single standard
		''' character.
		''' </summary>
		Public Const STANDARD As SByte = 0

		''' <summary>
		''' Indicates a glyph that represents multiple characters
		''' as a ligature, for example 'fi' or 'ffi'.  It is followed by
		''' filler glyphs for the remaining characters. Filler and combining
		''' glyphs can be intermixed to control positioning of accent marks
		''' on the logically preceding ligature.
		''' </summary>
		Public Const LIGATURE As SByte = 1

		''' <summary>
		''' Indicates a glyph that represents a combining character,
		''' such as an umlaut.  There is no caret position between this glyph
		''' and the preceding glyph.
		''' </summary>
		Public Const COMBINING As SByte = 2

		''' <summary>
		''' Indicates a glyph with no corresponding character in the
		''' backing store.  The glyph is associated with the character
		''' represented by the logically preceding non-component glyph.  This
		''' is used for kashida justification or other visual modifications to
		''' existing glyphs.  There is no caret position between this glyph
		''' and the preceding glyph.
		''' </summary>
		Public Const COMPONENT As SByte = 3

		''' <summary>
		''' Indicates a glyph with no visual representation. It can
		''' be added to the other code values to indicate an invisible glyph.
		''' </summary>
		Public Const WHITESPACE As SByte = 4

		''' <summary>
		''' Constructs a <code>GlyphMetrics</code> object. </summary>
		''' <param name="advance"> the advance width of the glyph </param>
		''' <param name="bounds"> the black box bounds of the glyph </param>
		''' <param name="glyphType"> the type of the glyph </param>
		Public Sub New(ByVal advance As Single, ByVal bounds As java.awt.geom.Rectangle2D, ByVal glyphType As SByte)
			Me.horizontal = True
			Me.advanceX = advance
			Me.advanceY = 0
			Me.bounds = New java.awt.geom.Rectangle2D.Float
			Me.bounds.rect = bounds
			Me.glyphType = glyphType
		End Sub

		''' <summary>
		''' Constructs a <code>GlyphMetrics</code> object. </summary>
		''' <param name="horizontal"> if true, metrics are for a horizontal baseline,
		'''   otherwise they are for a vertical baseline </param>
		''' <param name="advanceX"> the X-component of the glyph's advance </param>
		''' <param name="advanceY"> the Y-component of the glyph's advance </param>
		''' <param name="bounds"> the visual bounds of the glyph </param>
		''' <param name="glyphType"> the type of the glyph
		''' @since 1.4 </param>
		Public Sub New(ByVal horizontal As Boolean, ByVal advanceX As Single, ByVal advanceY As Single, ByVal bounds As java.awt.geom.Rectangle2D, ByVal glyphType As SByte)

			Me.horizontal = horizontal
			Me.advanceX = advanceX
			Me.advanceY = advanceY
			Me.bounds = New java.awt.geom.Rectangle2D.Float
			Me.bounds.rect = bounds
			Me.glyphType = glyphType
		End Sub

		''' <summary>
		''' Returns the advance of the glyph along the baseline (either
		''' horizontal or vertical). </summary>
		''' <returns> the advance of the glyph </returns>
		Public Property advance As Single
			Get
				Return If(horizontal, advanceX, advanceY)
			End Get
		End Property

		''' <summary>
		''' Returns the x-component of the advance of the glyph. </summary>
		''' <returns> the x-component of the advance of the glyph
		''' @since 1.4 </returns>
		Public Property advanceX As Single
			Get
				Return advanceX
			End Get
		End Property

		''' <summary>
		''' Returns the y-component of the advance of the glyph. </summary>
		''' <returns> the y-component of the advance of the glyph
		''' @since 1.4 </returns>
		Public Property advanceY As Single
			Get
				Return advanceY
			End Get
		End Property

		''' <summary>
		''' Returns the bounds of the glyph. This is the bounding box of the glyph outline.
		''' Because of rasterization and pixel alignment effects, it does not necessarily
		''' enclose the pixels that are affected when rendering the glyph. </summary>
		''' <returns> a <seealso cref="Rectangle2D"/> that is the bounds of the glyph. </returns>
		Public Property bounds2D As java.awt.geom.Rectangle2D
			Get
				Return New java.awt.geom.Rectangle2D.Float(bounds.x, bounds.y, bounds.width, bounds.height)
			End Get
		End Property

		''' <summary>
		''' Returns the left (top) side bearing of the glyph.
		''' <p>
		''' This is the distance from 0,&nbsp;0 to the left (top) of the glyph
		''' bounds.  If the bounds of the glyph is to the left of (above) the
		''' origin, the LSB is negative. </summary>
		''' <returns> the left side bearing of the glyph. </returns>
		Public Property lSB As Single
			Get
				Return If(horizontal, bounds.x, bounds.y)
			End Get
		End Property

		''' <summary>
		''' Returns the right (bottom) side bearing of the glyph.
		''' <p>
		''' This is the distance from the right (bottom) of the glyph bounds to
		''' the advance. If the bounds of the glyph is to the right of (below)
		''' the advance, the RSB is negative. </summary>
		''' <returns> the right side bearing of the glyph. </returns>
		Public Property rSB As Single
			Get
				Return If(horizontal, advanceX - bounds.x - bounds.width, advanceY - bounds.y - bounds.height)
			End Get
		End Property

		''' <summary>
		''' Returns the raw glyph type code. </summary>
		''' <returns> the raw glyph type code. </returns>
		Public Property type As Integer
			Get
				Return glyphType
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is a standard glyph. </summary>
		''' <returns> <code>true</code> if this is a standard glyph;
		'''          <code>false</code> otherwise. </returns>
		Public Property standard As Boolean
			Get
				Return (glyphType And &H3) = STANDARD
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is a ligature glyph. </summary>
		''' <returns> <code>true</code> if this is a ligature glyph;
		'''          <code>false</code> otherwise. </returns>
		Public Property ligature As Boolean
			Get
				Return (glyphType And &H3) = LIGATURE
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is a combining glyph. </summary>
		''' <returns> <code>true</code> if this is a combining glyph;
		'''          <code>false</code> otherwise. </returns>
		Public Property combining As Boolean
			Get
				Return (glyphType And &H3) = COMBINING
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is a component glyph. </summary>
		''' <returns> <code>true</code> if this is a component glyph;
		'''          <code>false</code> otherwise. </returns>
		Public Property component As Boolean
			Get
				Return (glyphType And &H3) = COMPONENT
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this is a whitespace glyph. </summary>
		''' <returns> <code>true</code> if this is a whitespace glyph;
		'''          <code>false</code> otherwise. </returns>
		Public Property whitespace As Boolean
			Get
				Return (glyphType And &H4) = WHITESPACE
			End Get
		End Property
	End Class

End Namespace