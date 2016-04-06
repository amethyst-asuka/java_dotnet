Imports Microsoft.VisualBasic
Imports System

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
	''' The <code>ShapeGraphicAttribute</code> class is an implementation of
	''' <seealso cref="GraphicAttribute"/> that draws shapes in a <seealso cref="TextLayout"/>. </summary>
	''' <seealso cref= GraphicAttribute </seealso>
	Public NotInheritable Class ShapeGraphicAttribute
		Inherits GraphicAttribute

		Private fShape As java.awt.Shape
		Private fStroke As Boolean

		''' <summary>
		''' A key indicating the shape should be stroked with a 1-pixel wide stroke.
		''' </summary>
		Public Const STROKE As Boolean = True

		''' <summary>
		''' A key indicating the shape should be filled.
		''' </summary>
		Public Const FILL As Boolean = False

		' cache shape bounds, since GeneralPath doesn't
		Private fShapeBounds As java.awt.geom.Rectangle2D

		''' <summary>
		''' Constructs a <code>ShapeGraphicAttribute</code> for the specified
		''' <seealso cref="Shape"/>. </summary>
		''' <param name="shape"> the <code>Shape</code> to render.  The
		''' <code>Shape</code> is rendered with its origin at the origin of
		''' this <code>ShapeGraphicAttribute</code> in the
		''' host <code>TextLayout</code>.  This object maintains a reference to
		''' <code>shape</code>. </param>
		''' <param name="alignment"> one of the alignments from this
		''' <code>ShapeGraphicAttribute</code>. </param>
		''' <param name="stroke"> <code>true</code> if the <code>Shape</code> should be
		''' stroked; <code>false</code> if the <code>Shape</code> should be
		''' filled. </param>
		Public Sub New(  shape As java.awt.Shape,   alignment As Integer,   stroke As Boolean)

			MyBase.New(alignment)

			fShape = shape
			fStroke = stroke
			fShapeBounds = fShape.bounds2D
		End Sub

        ''' <summary>
        ''' Returns the ascent of this <code>ShapeGraphicAttribute</code>.  The
        ''' ascent of a <code>ShapeGraphicAttribute</code> is the positive
        ''' distance from the origin of its <code>Shape</code> to the top of
        ''' bounds of its <code>Shape</code>. </summary>
        ''' <returns> the ascent of this <code>ShapeGraphicAttribute</code>. </returns>
        Public Overrides ReadOnly Property ascent As Single
            Get

                Return CSng(System.Math.Max(0, -fShapeBounds.minY))
            End Get
        End Property

        ''' <summary>
        ''' Returns the descent of this <code>ShapeGraphicAttribute</code>.
        ''' The descent of a <code>ShapeGraphicAttribute</code> is the distance
        ''' from the origin of its <code>Shape</code> to the bottom of the
        ''' bounds of its <code>Shape</code>. </summary>
        ''' <returns> the descent of this <code>ShapeGraphicAttribute</code>. </returns>
        Public Overrides ReadOnly Property descent As Single
            Get

                Return CSng(System.Math.Max(0, fShapeBounds.maxY))
            End Get
        End Property

        ''' <summary>
        ''' Returns the advance of this <code>ShapeGraphicAttribute</code>.
        ''' The advance of a <code>ShapeGraphicAttribute</code> is the distance
        ''' from the origin of its <code>Shape</code> to the right side of the
        ''' bounds of its <code>Shape</code>. </summary>
        ''' <returns> the advance of this <code>ShapeGraphicAttribute</code>. </returns>
        Public Overrides ReadOnly Property advance As Single
            Get

                Return CSng(System.Math.Max(0, fShapeBounds.maxX))
            End Get
        End Property

        ''' <summary>
        ''' {@inheritDoc}
        ''' </summary>
        Public Overrides Sub draw(  graphics As java.awt.Graphics2D,   x As Single,   y As Single)

			' translating graphics to draw Shape !!!
			graphics.translate(CInt(Fix(x)), CInt(Fix(y)))

			Try
				If fStroke = STROKE Then
					' REMIND: set stroke to correct size
					graphics.draw(fShape)
				Else
					graphics.fill(fShape)
				End If
			Finally
				graphics.translate(-CInt(Fix(x)), -CInt(Fix(y)))
			End Try
		End Sub

        ''' <summary>
        ''' Returns a <seealso cref="Rectangle2D"/> that encloses all of the
        ''' bits drawn by this <code>ShapeGraphicAttribute</code> relative to
        ''' the rendering position.  A graphic can be rendered beyond its
        ''' origin, ascent, descent, or advance;  but if it does, this method's
        ''' implementation should indicate where the graphic is rendered. </summary>
        ''' <returns> a <code>Rectangle2D</code> that encloses all of the bits
        ''' rendered by this <code>ShapeGraphicAttribute</code>. </returns>
        Public Overrides ReadOnly Property bounds As java.awt.geom.Rectangle2D
            Get

                Dim bounds_Renamed As New java.awt.geom.Rectangle2D.Float
                bounds_Renamed.rect = fShapeBounds

                If fStroke = STROKE Then
                    bounds_Renamed.width += 1
                    bounds_Renamed.height += 1
                End If

                Return bounds_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Return a <seealso cref="java.awt.Shape"/> that represents the region that
        ''' this <code>ShapeGraphicAttribute</code> renders.  This is used when a
        ''' <seealso cref="TextLayout"/> is requested to return the outline of the text.
        ''' The (untransformed) shape must not extend outside the rectangular
        ''' bounds returned by <code>getBounds</code>. </summary>
        ''' <param name="tx"> an optional <seealso cref="AffineTransform"/> to apply to the
        '''   this <code>ShapeGraphicAttribute</code>. This can be null. </param>
        ''' <returns> the <code>Shape</code> representing this graphic attribute,
        '''   suitable for stroking or filling.
        ''' @since 1.6 </returns>
        Public Overrides Function getOutline(  tx As java.awt.geom.AffineTransform) As java.awt.Shape
			Return If(tx Is Nothing, fShape, tx.createTransformedShape(fShape))
		End Function

		''' <summary>
		''' Returns a hashcode for this <code>ShapeGraphicAttribute</code>. </summary>
		''' <returns>  a hash code value for this
		''' <code>ShapeGraphicAttribute</code>. </returns>
		Public Overrides Function GetHashCode() As Integer

			Return fShape.GetHashCode()
		End Function

		''' <summary>
		''' Compares this <code>ShapeGraphicAttribute</code> to the specified
		''' <code>Object</code>. </summary>
		''' <param name="rhs"> the <code>Object</code> to compare for equality </param>
		''' <returns> <code>true</code> if this
		''' <code>ShapeGraphicAttribute</code> equals <code>rhs</code>;
		''' <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(  rhs As Object) As Boolean

            Try
                Return Equals(CType(rhs, ShapeGraphicAttribute))
            Catch e As ClassCastException
                Return False
			End Try
		End Function

		''' <summary>
		''' Compares this <code>ShapeGraphicAttribute</code> to the specified
		''' <code>ShapeGraphicAttribute</code>. </summary>
		''' <param name="rhs"> the <code>ShapeGraphicAttribute</code> to compare for
		''' equality </param>
		''' <returns> <code>true</code> if this
		''' <code>ShapeGraphicAttribute</code> equals <code>rhs</code>;
		''' <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(  rhs As ShapeGraphicAttribute) As Boolean

			If rhs Is Nothing Then Return False

			If Me Is rhs Then Return True

			If fStroke <> rhs.fStroke Then Return False

			If alignment <> rhs.alignment Then Return False

			If Not fShape.Equals(rhs.fShape) Then Return False

			Return True
		End Function
	End Class

End Namespace