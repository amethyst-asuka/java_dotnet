'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used with the CHAR_REPLACEMENT attribute.
	''' <p>
	''' The <code>GraphicAttribute</code> class represents a graphic embedded
	''' in text. Clients subclass this class to implement their own char
	''' replacement graphics.  Clients wishing to embed shapes and images in
	''' text need not subclass this class.  Instead, clients can use the
	''' <seealso cref="ShapeGraphicAttribute"/> and <seealso cref="ImageGraphicAttribute"/>
	''' classes.
	''' <p>
	''' Subclasses must ensure that their objects are immutable once they
	''' are constructed.  Mutating a <code>GraphicAttribute</code> that
	''' is used in a <seealso cref="TextLayout"/> results in undefined behavior from the
	''' <code>TextLayout</code>.
	''' </summary>
	Public MustInherit Class GraphicAttribute

		Private fAlignment As Integer

		''' <summary>
		''' Aligns top of graphic to top of line.
		''' </summary>
		Public Const TOP_ALIGNMENT As Integer = -1

		''' <summary>
		''' Aligns bottom of graphic to bottom of line.
		''' </summary>
		Public Const BOTTOM_ALIGNMENT As Integer = -2

		''' <summary>
		''' Aligns origin of graphic to roman baseline of line.
		''' </summary>
		Public Const ROMAN_BASELINE As Integer = java.awt.Font.ROMAN_BASELINE

		''' <summary>
		''' Aligns origin of graphic to center baseline of line.
		''' </summary>
		Public Const CENTER_BASELINE As Integer = java.awt.Font.CENTER_BASELINE

		''' <summary>
		''' Aligns origin of graphic to hanging baseline of line.
		''' </summary>
		Public Const HANGING_BASELINE As Integer = java.awt.Font.HANGING_BASELINE

		''' <summary>
		''' Constructs a <code>GraphicAttribute</code>.
		''' Subclasses use this to define the alignment of the graphic. </summary>
		''' <param name="alignment"> an int representing one of the
		''' <code>GraphicAttribute</code> alignment fields </param>
		''' <exception cref="IllegalArgumentException"> if alignment is not one of the
		''' five defined values. </exception>
		Protected Friend Sub New(ByVal alignment As Integer)
			If alignment < BOTTOM_ALIGNMENT OrElse alignment > HANGING_BASELINE Then Throw New IllegalArgumentException("bad alignment")
			fAlignment = alignment
		End Sub

		''' <summary>
		''' Returns the ascent of this <code>GraphicAttribute</code>.  A
		''' graphic can be rendered above its ascent. </summary>
		''' <returns> the ascent of this <code>GraphicAttribute</code>. </returns>
		''' <seealso cref= #getBounds() </seealso>
		Public MustOverride ReadOnly Property ascent As Single


		''' <summary>
		''' Returns the descent of this <code>GraphicAttribute</code>.  A
		''' graphic can be rendered below its descent. </summary>
		''' <returns> the descent of this <code>GraphicAttribute</code>. </returns>
		''' <seealso cref= #getBounds() </seealso>
		Public MustOverride ReadOnly Property descent As Single

		''' <summary>
		''' Returns the advance of this <code>GraphicAttribute</code>.  The
		''' <code>GraphicAttribute</code> object's advance is the distance
		''' from the point at which the graphic is rendered and the point where
		''' the next character or graphic is rendered.  A graphic can be
		''' rendered beyond its advance </summary>
		''' <returns> the advance of this <code>GraphicAttribute</code>. </returns>
		''' <seealso cref= #getBounds() </seealso>
		Public MustOverride ReadOnly Property advance As Single

        ''' <summary>
        ''' Returns a <seealso cref="Rectangle2D"/> that encloses all of the
        ''' bits drawn by this <code>GraphicAttribute</code> relative to the
        ''' rendering position.
        ''' A graphic may be rendered beyond its origin, ascent, descent,
        ''' or advance;  but if it is, this method's implementation must
        ''' indicate where the graphic is rendered.
        ''' Default bounds is the rectangle (0, -ascent, advance, ascent+descent). </summary>
        ''' <returns> a <code>Rectangle2D</code> that encloses all of the bits
        ''' rendered by this <code>GraphicAttribute</code>. </returns>
        Public Overridable ReadOnly Property bounds As java.awt.geom.Rectangle2D
            Get
                Dim ascent_Renamed As Single = ascent
                Return New java.awt.geom.Rectangle2D.Float(0, -ascent_Renamed, advance, ascent_Renamed + descent)
            End Get
        End Property

        ''' <summary>
        ''' Return a <seealso cref="java.awt.Shape"/> that represents the region that
        ''' this <code>GraphicAttribute</code> renders.  This is used when a
        ''' <seealso cref="TextLayout"/> is requested to return the outline of the text.
        ''' The (untransformed) shape must not extend outside the rectangular
        ''' bounds returned by <code>getBounds</code>.
        ''' The default implementation returns the rectangle returned by
        ''' <seealso cref="#getBounds"/>, transformed by the provided <seealso cref="AffineTransform"/>
        ''' if present. </summary>
        ''' <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
        '''   outline of this <code>GraphicAttribute</code>. This can be null. </param>
        ''' <returns> a <code>Shape</code> representing this graphic attribute,
        '''   suitable for stroking or filling.
        ''' @since 1.6 </returns>
        Public Overridable Function getOutline(ByVal tx As java.awt.geom.AffineTransform) As java.awt.Shape
			Dim b As java.awt.Shape = bounds
			If tx IsNot Nothing Then b = tx.createTransformedShape(b)
			Return b
		End Function

		''' <summary>
		''' Renders this <code>GraphicAttribute</code> at the specified
		''' location. </summary>
		''' <param name="graphics"> the <seealso cref="Graphics2D"/> into which to render the
		''' graphic </param>
		''' <param name="x"> the user-space X coordinate where the graphic is rendered </param>
		''' <param name="y"> the user-space Y coordinate where the graphic is rendered </param>
		Public MustOverride Sub draw(ByVal graphics As java.awt.Graphics2D, ByVal x As Single, ByVal y As Single)

        ''' <summary>
        ''' Returns the alignment of this <code>GraphicAttribute</code>.
        ''' Alignment can be to a particular baseline, or to the absolute top
        ''' or bottom of a line. </summary>
        ''' <returns> the alignment of this <code>GraphicAttribute</code>. </returns>
        Public ReadOnly Property alignment As Integer
            Get

                Return fAlignment
            End Get
        End Property

        ''' <summary>
        ''' Returns the justification information for this
        ''' <code>GraphicAttribute</code>.  Subclasses
        ''' can override this method to provide different justification
        ''' information. </summary>
        ''' <returns> a <seealso cref="GlyphJustificationInfo"/> object that contains the
        ''' justification information for this <code>GraphicAttribute</code>. </returns>
        Public Overridable ReadOnly Property justificationInfo As GlyphJustificationInfo
            Get

                ' should we cache this?
                Dim advance_Renamed As Single = advance

                Return New GlyphJustificationInfo(advance_Renamed, False, 2, advance_Renamed / 3, advance_Renamed / 3, False, 1, 0, 0) ' shrinkRightLimit -  shrinkLeftLimit -  shrinkPriority -  shrinkAbsorb -  growRightLimit -  growLeftLimit -  growPriority -  growAbsorb -  weight
            End Get
        End Property
    End Class

End Namespace