Imports Microsoft.VisualBasic
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
' * @author Charlton Innovations, Inc.
' 

Namespace java.awt.font


	''' <summary>
	''' A <code>GlyphVector</code> object is a collection of glyphs
	''' containing geometric information for the placement of each glyph
	''' in a transformed coordinate space which corresponds to the
	''' device on which the <code>GlyphVector</code> is ultimately
	''' displayed.
	''' <p>
	''' The <code>GlyphVector</code> does not attempt any interpretation of
	''' the sequence of glyphs it contains.  Relationships between adjacent
	''' glyphs in sequence are solely used to determine the placement of
	''' the glyphs in the visual coordinate space.
	''' <p>
	''' Instances of <code>GlyphVector</code> are created by a <seealso cref="Font"/>.
	''' <p>
	''' In a text processing application that can cache intermediate
	''' representations of text, creation and subsequent caching of a
	''' <code>GlyphVector</code> for use during rendering is the fastest
	''' method to present the visual representation of characters to a user.
	''' <p>
	''' A <code>GlyphVector</code> is associated with exactly one
	''' <code>Font</code>, and can provide data useful only in relation to
	''' this <code>Font</code>.  In addition, metrics obtained from a
	''' <code>GlyphVector</code> are not generally geometrically scaleable
	''' since the pixelization and spacing are dependent on grid-fitting
	''' algorithms within a <code>Font</code>.  To facilitate accurate
	''' measurement of a <code>GlyphVector</code> and its component
	''' glyphs, you must specify a scaling transform, anti-alias mode, and
	''' fractional metrics mode when creating the <code>GlyphVector</code>.
	''' These characteristics can be derived from the destination device.
	''' <p>
	''' For each glyph in the <code>GlyphVector</code>, you can obtain:
	''' <ul>
	''' <li>the position of the glyph
	''' <li>the transform associated with the glyph
	''' <li>the metrics of the glyph in the context of the
	'''   <code>GlyphVector</code>.  The metrics of the glyph may be
	'''   different under different transforms, application specified
	'''   rendering hints, and the specific instance of the glyph within
	'''   the <code>GlyphVector</code>.
	''' </ul>
	''' <p>
	''' Altering the data used to create the <code>GlyphVector</code> does not
	''' alter the state of the <code>GlyphVector</code>.
	''' <p>
	''' Methods are provided to adjust the positions of the glyphs
	''' within the <code>GlyphVector</code>.  These methods are most
	''' appropriate for applications that are performing justification
	''' operations for the presentation of the glyphs.
	''' <p>
	''' Methods are provided to transform individual glyphs within the
	''' <code>GlyphVector</code>.  These methods are primarily useful for
	''' special effects.
	''' <p>
	''' Methods are provided to return both the visual, logical, and pixel bounds
	''' of the entire <code>GlyphVector</code> or of individual glyphs within
	''' the <code>GlyphVector</code>.
	''' <p>
	''' Methods are provided to return a <seealso cref="Shape"/> for the
	''' <code>GlyphVector</code>, and for individual glyphs within the
	''' <code>GlyphVector</code>. </summary>
	''' <seealso cref= Font </seealso>
	''' <seealso cref= GlyphMetrics </seealso>
	''' <seealso cref= TextLayout
	''' @author Charlton Innovations, Inc. </seealso>

	Public MustInherit Class GlyphVector
		Implements Cloneable

		'
		' methods associated with creation-time state
		'

		''' <summary>
		''' Returns the <code>Font</code> associated with this
		''' <code>GlyphVector</code>. </summary>
		''' <returns> <code>Font</code> used to create this
		''' <code>GlyphVector</code>. </returns>
		''' <seealso cref= Font </seealso>
		Public MustOverride ReadOnly Property font As java.awt.Font

		''' <summary>
		''' Returns the <seealso cref="FontRenderContext"/> associated with this
		''' <code>GlyphVector</code>. </summary>
		''' <returns> <code>FontRenderContext</code> used to create this
		''' <code>GlyphVector</code>. </returns>
		''' <seealso cref= FontRenderContext </seealso>
		''' <seealso cref= Font </seealso>
		Public MustOverride ReadOnly Property fontRenderContext As FontRenderContext

		'
		' methods associated with the GlyphVector as a whole
		'

		''' <summary>
		''' Assigns default positions to each glyph in this
		''' <code>GlyphVector</code>. This can destroy information
		''' generated during initial layout of this <code>GlyphVector</code>.
		''' </summary>
		Public MustOverride Sub performDefaultLayout()

		''' <summary>
		''' Returns the number of glyphs in this <code>GlyphVector</code>. </summary>
		''' <returns> number of glyphs in this <code>GlyphVector</code>. </returns>
		Public MustOverride ReadOnly Property numGlyphs As Integer

		''' <summary>
		''' Returns the glyphcode of the specified glyph.
		''' This return value is meaningless to anything other
		''' than the <code>Font</code> object that created this
		''' <code>GlyphVector</code>. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code>
		''' that corresponds to the glyph from which to retrieve the
		''' glyphcode. </param>
		''' <returns> the glyphcode of the glyph at the specified
		''' <code>glyphIndex</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		''' is less than 0 or greater than or equal to the
		''' number of glyphs in this <code>GlyphVector</code> </exception>
		Public MustOverride Function getGlyphCode(ByVal glyphIndex As Integer) As Integer

		''' <summary>
		''' Returns an array of glyphcodes for the specified glyphs.
		''' The contents of this return value are meaningless to anything other
		''' than the <code>Font</code> used to create this
		''' <code>GlyphVector</code>.  This method is used
		''' for convenience and performance when processing glyphcodes.
		''' If no array is passed in, a new array is created. </summary>
		''' <param name="beginGlyphIndex"> the index into this
		'''   <code>GlyphVector</code> at which to start retrieving glyphcodes </param>
		''' <param name="numEntries"> the number of glyphcodes to retrieve </param>
		''' <param name="codeReturn"> the array that receives the glyphcodes and is
		'''   then returned </param>
		''' <returns> an array of glyphcodes for the specified glyphs. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>numEntries</code> is
		'''   less than 0 </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>beginGlyphIndex</code>
		'''   is less than 0 </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the sum of
		'''   <code>beginGlyphIndex</code> and <code>numEntries</code> is
		'''   greater than the number of glyphs in this
		'''   <code>GlyphVector</code> </exception>
		Public MustOverride Function getGlyphCodes(ByVal beginGlyphIndex As Integer, ByVal numEntries As Integer, ByVal codeReturn As Integer()) As Integer()

		''' <summary>
		''' Returns the character index of the specified glyph.
		''' The character index is the index of the first logical
		''' character represented by the glyph.  The default
		''' implementation assumes a one-to-one, left-to-right mapping
		''' of glyphs to characters. </summary>
		''' <param name="glyphIndex"> the index of the glyph </param>
		''' <returns> the index of the first character represented by the glyph
		''' @since 1.4 </returns>
		Public Overridable Function getGlyphCharIndex(ByVal glyphIndex As Integer) As Integer
			Return glyphIndex
		End Function

		''' <summary>
		''' Returns the character indices of the specified glyphs.
		''' The character index is the index of the first logical
		''' character represented by the glyph.  Indices are returned
		''' in glyph order.  The default implementation invokes
		''' getGlyphCharIndex for each glyph, and subclassers will probably
		''' want to override this implementation for performance reasons.
		''' Use this method for convenience and performance
		''' in processing of glyphcodes. If no array is passed in,
		''' a new array is created. </summary>
		''' <param name="beginGlyphIndex"> the index of the first glyph </param>
		''' <param name="numEntries"> the number of glyph indices </param>
		''' <param name="codeReturn"> the array into which to return the character indices </param>
		''' <returns> an array of character indices, one per glyph.
		''' @since 1.4 </returns>
		Public Overridable Function getGlyphCharIndices(ByVal beginGlyphIndex As Integer, ByVal numEntries As Integer, ByVal codeReturn As Integer()) As Integer()
			If codeReturn Is Nothing Then codeReturn = New Integer(numEntries - 1){}
			Dim i As Integer = 0
			Dim j As Integer = beginGlyphIndex
			Do While i < numEntries
				codeReturn(i) = getGlyphCharIndex(j)
				i += 1
				j += 1
			Loop
			Return codeReturn
		End Function

		''' <summary>
		''' Returns the logical bounds of this <code>GlyphVector</code>.
		''' This method is used when positioning this <code>GlyphVector</code>
		''' in relation to visually adjacent <code>GlyphVector</code> objects. </summary>
		''' <returns> a <seealso cref="Rectangle2D"/> that is the logical bounds of this
		''' <code>GlyphVector</code>. </returns>
		Public MustOverride ReadOnly Property logicalBounds As java.awt.geom.Rectangle2D

		''' <summary>
		''' Returns the visual bounds of this <code>GlyphVector</code>
		''' The visual bounds is the bounding box of the outline of this
		''' <code>GlyphVector</code>.  Because of rasterization and
		''' alignment of pixels, it is possible that this box does not
		''' enclose all pixels affected by rendering this <code>GlyphVector</code>. </summary>
		''' <returns> a <code>Rectangle2D</code> that is the bounding box
		''' of this <code>GlyphVector</code>. </returns>
		Public MustOverride ReadOnly Property visualBounds As java.awt.geom.Rectangle2D

		''' <summary>
		''' Returns the pixel bounds of this <code>GlyphVector</code> when
		''' rendered in a graphics with the given
		''' <code>FontRenderContext</code> at the given location.  The
		''' renderFRC need not be the same as the
		''' <code>FontRenderContext</code> of this
		''' <code>GlyphVector</code>, and can be null.  If it is null, the
		''' <code>FontRenderContext</code> of this <code>GlyphVector</code>
		''' is used.  The default implementation returns the visual bounds,
		''' offset to x, y and rounded out to the next integer value (i.e. returns an
		''' integer rectangle which encloses the visual bounds) and
		''' ignores the FRC.  Subclassers should override this method. </summary>
		''' <param name="renderFRC"> the <code>FontRenderContext</code> of the <code>Graphics</code>. </param>
		''' <param name="x"> the x-coordinate at which to render this <code>GlyphVector</code>. </param>
		''' <param name="y"> the y-coordinate at which to render this <code>GlyphVector</code>. </param>
		''' <returns> a <code>Rectangle</code> bounding the pixels that would be affected.
		''' @since 1.4 </returns>
		Public Overridable Function getPixelBounds(ByVal renderFRC As FontRenderContext, ByVal x As Single, ByVal y As Single) As java.awt.Rectangle
					Dim rect As java.awt.geom.Rectangle2D = visualBounds
					Dim l As Integer = CInt(Fix (System.Math.Floor(rect.x + x)))
					Dim t As Integer = CInt(Fix (System.Math.Floor(rect.y + y)))
					Dim r As Integer = CInt(Fix (System.Math.Ceiling(rect.maxX + x)))
					Dim b As Integer = CInt(Fix (System.Math.Ceiling(rect.maxY + y)))
					Return New java.awt.Rectangle(l, t, r - l, b - t)
		End Function


		''' <summary>
		''' Returns a <code>Shape</code> whose interior corresponds to the
		''' visual representation of this <code>GlyphVector</code>. </summary>
		''' <returns> a <code>Shape</code> that is the outline of this
		''' <code>GlyphVector</code>. </returns>
		Public MustOverride ReadOnly Property outline As java.awt.Shape

		''' <summary>
		''' Returns a <code>Shape</code> whose interior corresponds to the
		''' visual representation of this <code>GlyphVector</code> when
		''' rendered at x,&nbsp;y. </summary>
		''' <param name="x"> the X coordinate of this <code>GlyphVector</code>. </param>
		''' <param name="y"> the Y coordinate of this <code>GlyphVector</code>. </param>
		''' <returns> a <code>Shape</code> that is the outline of this
		'''   <code>GlyphVector</code> when rendered at the specified
		'''   coordinates. </returns>
		Public MustOverride Function getOutline(ByVal x As Single, ByVal y As Single) As java.awt.Shape

		''' <summary>
		''' Returns a <code>Shape</code> whose interior corresponds to the
		''' visual representation of the specified glyph
		''' within this <code>GlyphVector</code>.
		''' The outline returned by this method is positioned around the
		''' origin of each individual glyph. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <returns> a <code>Shape</code> that is the outline of the glyph
		'''   at the specified <code>glyphIndex</code> of this
		'''   <code>GlyphVector</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		Public MustOverride Function getGlyphOutline(ByVal glyphIndex As Integer) As java.awt.Shape

		''' <summary>
		''' Returns a <code>Shape</code> whose interior corresponds to the
		''' visual representation of the specified glyph
		''' within this <code>GlyphVector</code>, offset to x,&nbsp;y.
		''' The outline returned by this method is positioned around the
		''' origin of each individual glyph. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <param name="x"> the X coordinate of the location of this {@code GlyphVector} </param>
		''' <param name="y"> the Y coordinate of the location of this {@code GlyphVector} </param>
		''' <returns> a <code>Shape</code> that is the outline of the glyph
		'''   at the specified <code>glyphIndex</code> of this
		'''   <code>GlyphVector</code> when rendered at the specified
		'''   coordinates. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code>
		''' @since 1.4 </exception>
		Public Overridable Function getGlyphOutline(ByVal glyphIndex As Integer, ByVal x As Single, ByVal y As Single) As java.awt.Shape
			Dim s As java.awt.Shape = getGlyphOutline(glyphIndex)
			Dim at As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getTranslateInstance(x,y)
			Return at.createTransformedShape(s)
		End Function

		''' <summary>
		''' Returns the position of the specified glyph relative to the
		''' origin of this <code>GlyphVector</code>.
		''' If <code>glyphIndex</code> equals the number of of glyphs in
		''' this <code>GlyphVector</code>, this method returns the position after
		''' the last glyph. This position is used to define the advance of
		''' the entire <code>GlyphVector</code>. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <returns> a <seealso cref="Point2D"/> object that is the position of the glyph
		'''   at the specified <code>glyphIndex</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than the number of glyphs
		'''   in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #setGlyphPosition </seealso>
		Public MustOverride Function getGlyphPosition(ByVal glyphIndex As Integer) As java.awt.geom.Point2D

		''' <summary>
		''' Sets the position of the specified glyph within this
		''' <code>GlyphVector</code>.
		''' If <code>glyphIndex</code> equals the number of of glyphs in
		''' this <code>GlyphVector</code>, this method sets the position after
		''' the last glyph. This position is used to define the advance of
		''' the entire <code>GlyphVector</code>. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <param name="newPos"> the <code>Point2D</code> at which to position the
		'''   glyph at the specified <code>glyphIndex</code> </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than the number of glyphs
		'''   in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #getGlyphPosition </seealso>
		Public MustOverride Sub setGlyphPosition(ByVal glyphIndex As Integer, ByVal newPos As java.awt.geom.Point2D)

		''' <summary>
		''' Returns the transform of the specified glyph within this
		''' <code>GlyphVector</code>.  The transform is relative to the
		''' glyph position.  If no special transform has been applied,
		''' <code>null</code> can be returned.  A null return indicates
		''' an identity transform. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <returns> an <seealso cref="AffineTransform"/> that is the transform of
		'''   the glyph at the specified <code>glyphIndex</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #setGlyphTransform </seealso>
		Public MustOverride Function getGlyphTransform(ByVal glyphIndex As Integer) As java.awt.geom.AffineTransform

		''' <summary>
		''' Sets the transform of the specified glyph within this
		''' <code>GlyphVector</code>.  The transform is relative to the glyph
		''' position.  A <code>null</code> argument for <code>newTX</code>
		''' indicates that no special transform is applied for the specified
		''' glyph.
		''' This method can be used to rotate, mirror, translate and scale the
		''' glyph.  Adding a transform can result in significant performance changes. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code> </param>
		''' <param name="newTX"> the new transform of the glyph at <code>glyphIndex</code> </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #getGlyphTransform </seealso>
		Public MustOverride Sub setGlyphTransform(ByVal glyphIndex As Integer, ByVal newTX As java.awt.geom.AffineTransform)

		''' <summary>
		''' Returns flags describing the global state of the GlyphVector.
		''' Flags not described below are reserved.  The default
		''' implementation returns 0 (meaning false) for the position adjustments,
		''' transforms, rtl, and complex flags.
		''' Subclassers should override this method, and make sure
		''' it correctly describes the GlyphVector and corresponds
		''' to the results of related calls. </summary>
		''' <returns> an int containing the flags describing the state </returns>
		''' <seealso cref= #FLAG_HAS_POSITION_ADJUSTMENTS </seealso>
		''' <seealso cref= #FLAG_HAS_TRANSFORMS </seealso>
		''' <seealso cref= #FLAG_RUN_RTL </seealso>
		''' <seealso cref= #FLAG_COMPLEX_GLYPHS </seealso>
		''' <seealso cref= #FLAG_MASK
		''' @since 1.4 </seealso>
		Public Overridable Property layoutFlags As Integer
			Get
						Return 0
			End Get
		End Property

		''' <summary>
		''' A flag used with getLayoutFlags that indicates that this <code>GlyphVector</code> has
		''' per-glyph transforms.
		''' @since 1.4
		''' </summary>
		Public Const FLAG_HAS_TRANSFORMS As Integer = 1

		''' <summary>
		''' A flag used with getLayoutFlags that indicates that this <code>GlyphVector</code> has
		''' position adjustments.  When this is true, the glyph positions don't match the
		''' accumulated default advances of the glyphs (for example, if kerning has been done).
		''' @since 1.4
		''' </summary>
		Public Const FLAG_HAS_POSITION_ADJUSTMENTS As Integer = 2

		''' <summary>
		''' A flag used with getLayoutFlags that indicates that this <code>GlyphVector</code> has
		''' a right-to-left run direction.  This refers to the glyph-to-char mapping and does
		''' not imply that the visual locations of the glyphs are necessarily in this order,
		''' although generally they will be.
		''' @since 1.4
		''' </summary>
		Public Const FLAG_RUN_RTL As Integer = 4

		''' <summary>
		''' A flag used with getLayoutFlags that indicates that this <code>GlyphVector</code> has
		''' a complex glyph-to-char mapping (one that does not map glyphs to chars one-to-one in
		''' strictly ascending or descending order matching the run direction).
		''' @since 1.4
		''' </summary>
		Public Const FLAG_COMPLEX_GLYPHS As Integer = 8

		''' <summary>
		''' A mask for supported flags from getLayoutFlags.  Only bits covered by the mask
		''' should be tested.
		''' @since 1.4
		''' </summary>
		Public Shared ReadOnly FLAG_MASK As Integer = FLAG_HAS_TRANSFORMS Or FLAG_HAS_POSITION_ADJUSTMENTS Or FLAG_RUN_RTL Or FLAG_COMPLEX_GLYPHS

		''' <summary>
		''' Returns an array of glyph positions for the specified glyphs.
		''' This method is used for convenience and performance when
		''' processing glyph positions.
		''' If no array is passed in, a new array is created.
		''' Even numbered array entries beginning with position zero are the X
		''' coordinates of the glyph numbered <code>beginGlyphIndex + position/2</code>.
		''' Odd numbered array entries beginning with position one are the Y
		''' coordinates of the glyph numbered <code>beginGlyphIndex + (position-1)/2</code>.
		''' If <code>beginGlyphIndex</code> equals the number of of glyphs in
		''' this <code>GlyphVector</code>, this method gets the position after
		''' the last glyph and this position is used to define the advance of
		''' the entire <code>GlyphVector</code>. </summary>
		''' <param name="beginGlyphIndex"> the index at which to begin retrieving
		'''   glyph positions </param>
		''' <param name="numEntries"> the number of glyphs to retrieve </param>
		''' <param name="positionReturn"> the array that receives the glyph positions
		'''   and is then returned. </param>
		''' <returns> an array of glyph positions specified by
		'''  <code>beginGlyphIndex</code> and <code>numEntries</code>. </returns>
		''' <exception cref="IllegalArgumentException"> if <code>numEntries</code> is
		'''   less than 0 </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>beginGlyphIndex</code>
		'''   is less than 0 </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the sum of
		'''   <code>beginGlyphIndex</code> and <code>numEntries</code>
		'''   is greater than the number of glyphs in this
		'''   <code>GlyphVector</code> plus one </exception>
		Public MustOverride Function getGlyphPositions(ByVal beginGlyphIndex As Integer, ByVal numEntries As Integer, ByVal positionReturn As Single()) As Single()

		''' <summary>
		''' Returns the logical bounds of the specified glyph within this
		''' <code>GlyphVector</code>.
		''' These logical bounds have a total of four edges, with two edges
		''' parallel to the baseline under the glyph's transform and the other two
		''' edges are shared with adjacent glyphs if they are present.  This
		''' method is useful for hit-testing of the specified glyph,
		''' positioning of a caret at the leading or trailing edge of a glyph,
		''' and for drawing a highlight region around the specified glyph. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code>
		'''   that corresponds to the glyph from which to retrieve its logical
		'''   bounds </param>
		''' <returns>  a <code>Shape</code> that is the logical bounds of the
		'''   glyph at the specified <code>glyphIndex</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #getGlyphVisualBounds </seealso>
		Public MustOverride Function getGlyphLogicalBounds(ByVal glyphIndex As Integer) As java.awt.Shape

		''' <summary>
		''' Returns the visual bounds of the specified glyph within the
		''' <code>GlyphVector</code>.
		''' The bounds returned by this method is positioned around the
		''' origin of each individual glyph. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code>
		'''   that corresponds to the glyph from which to retrieve its visual
		'''   bounds </param>
		''' <returns> a <code>Shape</code> that is the visual bounds of the
		'''   glyph at the specified <code>glyphIndex</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		''' <seealso cref= #getGlyphLogicalBounds </seealso>
		Public MustOverride Function getGlyphVisualBounds(ByVal glyphIndex As Integer) As java.awt.Shape

		''' <summary>
		''' Returns the pixel bounds of the glyph at index when this
		''' <code>GlyphVector</code> is rendered in a <code>Graphics</code> with the
		''' given <code>FontRenderContext</code> at the given location. The
		''' renderFRC need not be the same as the
		''' <code>FontRenderContext</code> of this
		''' <code>GlyphVector</code>, and can be null.  If it is null, the
		''' <code>FontRenderContext</code> of this <code>GlyphVector</code>
		''' is used.  The default implementation returns the visual bounds of the glyph,
		''' offset to x, y and rounded out to the next integer value, and
		''' ignores the FRC.  Subclassers should override this method. </summary>
		''' <param name="index"> the index of the glyph. </param>
		''' <param name="renderFRC"> the <code>FontRenderContext</code> of the <code>Graphics</code>. </param>
		''' <param name="x"> the X position at which to render this <code>GlyphVector</code>. </param>
		''' <param name="y"> the Y position at which to render this <code>GlyphVector</code>. </param>
		''' <returns> a <code>Rectangle</code> bounding the pixels that would be affected.
		''' @since 1.4 </returns>
		Public Overridable Function getGlyphPixelBounds(ByVal index As Integer, ByVal renderFRC As FontRenderContext, ByVal x As Single, ByVal y As Single) As java.awt.Rectangle
					Dim rect As java.awt.geom.Rectangle2D = getGlyphVisualBounds(index).bounds2D
					Dim l As Integer = CInt(Fix (System.Math.Floor(rect.x + x)))
					Dim t As Integer = CInt(Fix (System.Math.Floor(rect.y + y)))
					Dim r As Integer = CInt(Fix (System.Math.Ceiling(rect.maxX + x)))
					Dim b As Integer = CInt(Fix (System.Math.Ceiling(rect.maxY + y)))
					Return New java.awt.Rectangle(l, t, r - l, b - t)
		End Function

		''' <summary>
		''' Returns the metrics of the glyph at the specified index into
		''' this <code>GlyphVector</code>. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code>
		'''   that corresponds to the glyph from which to retrieve its metrics </param>
		''' <returns> a <seealso cref="GlyphMetrics"/> object that represents the
		'''   metrics of the glyph at the specified <code>glyphIndex</code>
		'''   into this <code>GlyphVector</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		Public MustOverride Function getGlyphMetrics(ByVal glyphIndex As Integer) As java.awt.font.GlyphMetrics

		''' <summary>
		''' Returns the justification information for the glyph at
		''' the specified index into this <code>GlyphVector</code>. </summary>
		''' <param name="glyphIndex"> the index into this <code>GlyphVector</code>
		'''   that corresponds to the glyph from which to retrieve its
		'''   justification properties </param>
		''' <returns> a <seealso cref="GlyphJustificationInfo"/> object that
		'''   represents the justification properties of the glyph at the
		'''   specified <code>glyphIndex</code> into this
		'''   <code>GlyphVector</code>. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>glyphIndex</code>
		'''   is less than 0 or greater than or equal to the number
		'''   of glyphs in this <code>GlyphVector</code> </exception>
		Public MustOverride Function getGlyphJustificationInfo(ByVal glyphIndex As Integer) As java.awt.font.GlyphJustificationInfo

		'
		' general utility methods
		'

		''' <summary>
		''' Tests if the specified <code>GlyphVector</code> exactly
		''' equals this <code>GlyphVector</code>. </summary>
		''' <param name="set"> the specified <code>GlyphVector</code> to test </param>
		''' <returns> <code>true</code> if the specified
		'''   <code>GlyphVector</code> equals this <code>GlyphVector</code>;
		'''   <code>false</code> otherwise. </returns>
		Public MustOverride Function Equals(ByVal [set] As GlyphVector) As Boolean
	End Class

End Namespace