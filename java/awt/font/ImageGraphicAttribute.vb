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
	''' The <code>ImageGraphicAttribute</code> class is an implementation of
	''' <seealso cref="GraphicAttribute"/> which draws images in
	''' a <seealso cref="TextLayout"/>. </summary>
	''' <seealso cref= GraphicAttribute </seealso>

	Public NotInheritable Class ImageGraphicAttribute
		Inherits GraphicAttribute

		Private fImage As java.awt.Image
		Private fImageWidth, fImageHeight As Single
		Private fOriginX, fOriginY As Single

		''' <summary>
		''' Constucts an <code>ImageGraphicAttribute</code> from the specified
		''' <seealso cref="Image"/>.  The origin is at (0,&nbsp;0). </summary>
		''' <param name="image"> the <code>Image</code> rendered by this
		''' <code>ImageGraphicAttribute</code>.
		''' This object keeps a reference to <code>image</code>. </param>
		''' <param name="alignment"> one of the alignments from this
		''' <code>ImageGraphicAttribute</code> </param>
		Public Sub New(ByVal image_Renamed As java.awt.Image, ByVal alignment As Integer)

			Me.New(image_Renamed, alignment, 0, 0)
		End Sub

		''' <summary>
		''' Constructs an <code>ImageGraphicAttribute</code> from the specified
		''' <code>Image</code>. The point
		''' (<code>originX</code>,&nbsp;<code>originY</code>) in the
		''' <code>Image</code> appears at the origin of the
		''' <code>ImageGraphicAttribute</code> within the text. </summary>
		''' <param name="image"> the <code>Image</code> rendered by this
		''' <code>ImageGraphicAttribute</code>.
		''' This object keeps a reference to <code>image</code>. </param>
		''' <param name="alignment"> one of the alignments from this
		''' <code>ImageGraphicAttribute</code> </param>
		''' <param name="originX"> the X coordinate of the point within
		''' the <code>Image</code> that appears at the origin of the
		''' <code>ImageGraphicAttribute</code> in the text line. </param>
		''' <param name="originY"> the Y coordinate of the point within
		''' the <code>Image</code> that appears at the origin of the
		''' <code>ImageGraphicAttribute</code> in the text line. </param>
		Public Sub New(ByVal image_Renamed As java.awt.Image, ByVal alignment As Integer, ByVal originX As Single, ByVal originY As Single)

			MyBase.New(alignment)

			' Can't clone image
			' fImage = (Image) image.clone();
			fImage = image_Renamed

			fImageWidth = image_Renamed.getWidth(Nothing)
			fImageHeight = image_Renamed.getHeight(Nothing)

			' ensure origin is in Image?
			fOriginX = originX
			fOriginY = originY
		End Sub

		''' <summary>
		''' Returns the ascent of this <code>ImageGraphicAttribute</code>.  The
		''' ascent of an <code>ImageGraphicAttribute</code> is the distance
		''' from the top of the image to the origin. </summary>
		''' <returns> the ascent of this <code>ImageGraphicAttribute</code>. </returns>
		Public Property Overrides ascent As Single
			Get
    
				Return Math.Max(0, fOriginY)
			End Get
		End Property

		''' <summary>
		''' Returns the descent of this <code>ImageGraphicAttribute</code>.
		''' The descent of an <code>ImageGraphicAttribute</code> is the
		''' distance from the origin to the bottom of the image. </summary>
		''' <returns> the descent of this <code>ImageGraphicAttribute</code>. </returns>
		Public Property Overrides descent As Single
			Get
    
				Return Math.Max(0, fImageHeight-fOriginY)
			End Get
		End Property

		''' <summary>
		''' Returns the advance of this <code>ImageGraphicAttribute</code>.
		''' The advance of an <code>ImageGraphicAttribute</code> is the
		''' distance from the origin to the right edge of the image. </summary>
		''' <returns> the advance of this <code>ImageGraphicAttribute</code>. </returns>
		Public Property Overrides advance As Single
			Get
    
				Return Math.Max(0, fImageWidth-fOriginX)
			End Get
		End Property

		''' <summary>
		''' Returns a <seealso cref="Rectangle2D"/> that encloses all of the
		''' bits rendered by this <code>ImageGraphicAttribute</code>, relative
		''' to the rendering position.  A graphic can be rendered beyond its
		''' origin, ascent, descent, or advance;  but if it is, this
		''' method's implementation must indicate where the graphic is rendered. </summary>
		''' <returns> a <code>Rectangle2D</code> that encloses all of the bits
		''' rendered by this <code>ImageGraphicAttribute</code>. </returns>
		Public Property Overrides bounds As java.awt.geom.Rectangle2D
			Get
    
				Return New java.awt.geom.Rectangle2D.Float(-fOriginX, -fOriginY, fImageWidth, fImageHeight)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub draw(ByVal graphics As java.awt.Graphics2D, ByVal x As Single, ByVal y As Single)

			graphics.drawImage(fImage, CInt(Fix(x-fOriginX)), CInt(Fix(y-fOriginY)), Nothing)
		End Sub

		''' <summary>
		''' Returns a hashcode for this <code>ImageGraphicAttribute</code>. </summary>
		''' <returns>  a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer

			Return fImage.GetHashCode()
		End Function

		''' <summary>
		''' Compares this <code>ImageGraphicAttribute</code> to the specified
		''' <seealso cref="Object"/>. </summary>
		''' <param name="rhs"> the <code>Object</code> to compare for equality </param>
		''' <returns> <code>true</code> if this
		''' <code>ImageGraphicAttribute</code> equals <code>rhs</code>;
		''' <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal rhs As Object) As Boolean

			Try
				Return Equals(CType(rhs, ImageGraphicAttribute))
			Catch e As ClassCastException
				Return False
			End Try
		End Function

		''' <summary>
		''' Compares this <code>ImageGraphicAttribute</code> to the specified
		''' <code>ImageGraphicAttribute</code>. </summary>
		''' <param name="rhs"> the <code>ImageGraphicAttribute</code> to compare for
		''' equality </param>
		''' <returns> <code>true</code> if this
		''' <code>ImageGraphicAttribute</code> equals <code>rhs</code>;
		''' <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal rhs As ImageGraphicAttribute) As Boolean

			If rhs Is Nothing Then Return False

			If Me Is rhs Then Return True

			If fOriginX <> rhs.fOriginX OrElse fOriginY <> rhs.fOriginY Then Return False

			If alignment <> rhs.alignment Then Return False

			If Not fImage.Equals(rhs.fImage) Then Return False

			Return True
		End Function
	End Class

End Namespace