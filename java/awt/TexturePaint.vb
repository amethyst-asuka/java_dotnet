'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	''' <summary>
	''' The <code>TexturePaint</code> class provides a way to fill a
	''' <seealso cref="Shape"/> with a texture that is specified as
	''' a <seealso cref="BufferedImage"/>. The size of the <code>BufferedImage</code>
	''' object should be small because the <code>BufferedImage</code> data
	''' is copied by the <code>TexturePaint</code> object.
	''' At construction time, the texture is anchored to the upper
	''' left corner of a <seealso cref="Rectangle2D"/> that is
	''' specified in user space.  Texture is computed for
	''' locations in the device space by conceptually replicating the
	''' specified <code>Rectangle2D</code> infinitely in all directions
	''' in user space and mapping the <code>BufferedImage</code> to each
	''' replicated <code>Rectangle2D</code>. </summary>
	''' <seealso cref= Paint </seealso>
	''' <seealso cref= Graphics2D#setPaint
	''' @version 1.48, 06/05/07 </seealso>

	Public Class TexturePaint
		Implements Paint

		Friend bufImg As java.awt.image.BufferedImage
		Friend tx As Double
		Friend ty As Double
		Friend sx As Double
		Friend sy As Double

		''' <summary>
		''' Constructs a <code>TexturePaint</code> object. </summary>
		''' <param name="txtr"> the <code>BufferedImage</code> object with the texture
		''' used for painting </param>
		''' <param name="anchor"> the <code>Rectangle2D</code> in user space used to
		''' anchor and replicate the texture </param>
		Public Sub New(ByVal txtr As java.awt.image.BufferedImage, ByVal anchor As java.awt.geom.Rectangle2D)
			Me.bufImg = txtr
			Me.tx = anchor.x
			Me.ty = anchor.y
			Me.sx = anchor.width / bufImg.width
			Me.sy = anchor.height / bufImg.height
		End Sub

		''' <summary>
		''' Returns the <code>BufferedImage</code> texture used to
		''' fill the shapes. </summary>
		''' <returns> a <code>BufferedImage</code>. </returns>
		Public Overridable Property image As java.awt.image.BufferedImage
			Get
				Return bufImg
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the anchor rectangle which positions and
		''' sizes the textured image. </summary>
		''' <returns> the <code>Rectangle2D</code> used to anchor and
		''' size this <code>TexturePaint</code>. </returns>
		Public Overridable Property anchorRect As java.awt.geom.Rectangle2D
			Get
				Return New java.awt.geom.Rectangle2D.Double(tx, ty, sx * bufImg.width, sy * bufImg.height)
			End Get
		End Property

		''' <summary>
		''' Creates and returns a <seealso cref="PaintContext"/> used to
		''' generate a tiled image pattern.
		''' See the <seealso cref="Paint#createContext specification"/> of the
		''' method in the <seealso cref="Paint"/> interface for information
		''' on null parameter handling.
		''' </summary>
		''' <param name="cm"> the preferred <seealso cref="ColorModel"/> which represents the most convenient
		'''           format for the caller to receive the pixel data, or {@code null}
		'''           if there is no preference. </param>
		''' <param name="deviceBounds"> the device space bounding box
		'''                     of the graphics primitive being rendered. </param>
		''' <param name="userBounds"> the user space bounding box
		'''                   of the graphics primitive being rendered. </param>
		''' <param name="xform"> the <seealso cref="AffineTransform"/> from user
		'''              space into device space. </param>
		''' <param name="hints"> the set of hints that the context object can use to
		'''              choose between rendering alternatives. </param>
		''' <returns> the {@code PaintContext} for
		'''         generating color patterns. </returns>
		''' <seealso cref= Paint </seealso>
		''' <seealso cref= PaintContext </seealso>
		''' <seealso cref= ColorModel </seealso>
		''' <seealso cref= Rectangle </seealso>
		''' <seealso cref= Rectangle2D </seealso>
		''' <seealso cref= AffineTransform </seealso>
		''' <seealso cref= RenderingHints </seealso>
		Public Overridable Function createContext(ByVal cm As java.awt.image.ColorModel, ByVal deviceBounds As Rectangle, ByVal userBounds As java.awt.geom.Rectangle2D, ByVal xform As java.awt.geom.AffineTransform, ByVal hints As RenderingHints) As PaintContext Implements Paint.createContext
			If xform Is Nothing Then
				xform = New java.awt.geom.AffineTransform
			Else
				xform = CType(xform.clone(), java.awt.geom.AffineTransform)
			End If
			xform.translate(tx, ty)
			xform.scale(sx, sy)

			Return TexturePaintContext.getContext(bufImg, xform, hints, deviceBounds)
		End Function

		''' <summary>
		''' Returns the transparency mode for this <code>TexturePaint</code>. </summary>
		''' <returns> the transparency mode for this <code>TexturePaint</code>
		''' as an integer value. </returns>
		''' <seealso cref= Transparency </seealso>
		Public Overridable Property transparency As Integer Implements Transparency.getTransparency
			Get
				Return (bufImg.colorModel).transparency
			End Get
		End Property

	End Class

End Namespace