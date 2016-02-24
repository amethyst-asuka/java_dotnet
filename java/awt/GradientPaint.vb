'
' * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>GradientPaint</code> class provides a way to fill
	''' a <seealso cref="Shape"/> with a linear color gradient pattern.
	''' If <seealso cref="Point"/> P1 with <seealso cref="Color"/> C1 and <code>Point</code> P2 with
	''' <code>Color</code> C2 are specified in user space, the
	''' <code>Color</code> on the P1, P2 connecting line is proportionally
	''' changed from C1 to C2.  Any point P not on the extended P1, P2
	''' connecting line has the color of the point P' that is the perpendicular
	''' projection of P on the extended P1, P2 connecting line.
	''' Points on the extended line outside of the P1, P2 segment can be colored
	''' in one of two ways.
	''' <ul>
	''' <li>
	''' If the gradient is cyclic then the points on the extended P1, P2
	''' connecting line cycle back and forth between the colors C1 and C2.
	''' <li>
	''' If the gradient is acyclic then points on the P1 side of the segment
	''' have the constant <code>Color</code> C1 while points on the P2 side
	''' have the constant <code>Color</code> C2.
	''' </ul>
	''' </summary>
	''' <seealso cref= Paint </seealso>
	''' <seealso cref= Graphics2D#setPaint
	''' @version 10 Feb 1997 </seealso>

	Public Class GradientPaint
		Implements Paint

		Friend p1 As java.awt.geom.Point2D.Float
		Friend p2 As java.awt.geom.Point2D.Float
		Friend color1 As Color
		Friend color2 As Color
		Friend cyclic As Boolean

		''' <summary>
		''' Constructs a simple acyclic <code>GradientPaint</code> object. </summary>
		''' <param name="x1"> x coordinate of the first specified
		''' <code>Point</code> in user space </param>
		''' <param name="y1"> y coordinate of the first specified
		''' <code>Point</code> in user space </param>
		''' <param name="color1"> <code>Color</code> at the first specified
		''' <code>Point</code> </param>
		''' <param name="x2"> x coordinate of the second specified
		''' <code>Point</code> in user space </param>
		''' <param name="y2"> y coordinate of the second specified
		''' <code>Point</code> in user space </param>
		''' <param name="color2"> <code>Color</code> at the second specified
		''' <code>Point</code> </param>
		''' <exception cref="NullPointerException"> if either one of colors is null </exception>
		Public Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal color1 As Color, ByVal x2 As Single, ByVal y2 As Single, ByVal color2 As Color)
			If (color1 Is Nothing) OrElse (color2 Is Nothing) Then Throw New NullPointerException("Colors cannot be null")

			p1 = New java.awt.geom.Point2D.Float(x1, y1)
			p2 = New java.awt.geom.Point2D.Float(x2, y2)
			Me.color1 = color1
			Me.color2 = color2
		End Sub

		''' <summary>
		''' Constructs a simple acyclic <code>GradientPaint</code> object. </summary>
		''' <param name="pt1"> the first specified <code>Point</code> in user space </param>
		''' <param name="color1"> <code>Color</code> at the first specified
		''' <code>Point</code> </param>
		''' <param name="pt2"> the second specified <code>Point</code> in user space </param>
		''' <param name="color2"> <code>Color</code> at the second specified
		''' <code>Point</code> </param>
		''' <exception cref="NullPointerException"> if either one of colors or points
		''' is null </exception>
		Public Sub New(ByVal pt1 As java.awt.geom.Point2D, ByVal color1 As Color, ByVal pt2 As java.awt.geom.Point2D, ByVal color2 As Color)
			If (color1 Is Nothing) OrElse (color2 Is Nothing) OrElse (pt1 Is Nothing) OrElse (pt2 Is Nothing) Then Throw New NullPointerException("Colors and points should be non-null")

			p1 = New java.awt.geom.Point2D.Float(CSng(pt1.x), CSng(pt1.y))
			p2 = New java.awt.geom.Point2D.Float(CSng(pt2.x), CSng(pt2.y))
			Me.color1 = color1
			Me.color2 = color2
		End Sub

		''' <summary>
		''' Constructs either a cyclic or acyclic <code>GradientPaint</code>
		''' object depending on the <code>boolean</code> parameter. </summary>
		''' <param name="x1"> x coordinate of the first specified
		''' <code>Point</code> in user space </param>
		''' <param name="y1"> y coordinate of the first specified
		''' <code>Point</code> in user space </param>
		''' <param name="color1"> <code>Color</code> at the first specified
		''' <code>Point</code> </param>
		''' <param name="x2"> x coordinate of the second specified
		''' <code>Point</code> in user space </param>
		''' <param name="y2"> y coordinate of the second specified
		''' <code>Point</code> in user space </param>
		''' <param name="color2"> <code>Color</code> at the second specified
		''' <code>Point</code> </param>
		''' <param name="cyclic"> <code>true</code> if the gradient pattern should cycle
		''' repeatedly between the two colors; <code>false</code> otherwise </param>
		Public Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal color1 As Color, ByVal x2 As Single, ByVal y2 As Single, ByVal color2 As Color, ByVal cyclic As Boolean)
			Me.New(x1, y1, color1, x2, y2, color2)
			Me.cyclic = cyclic
		End Sub

		''' <summary>
		''' Constructs either a cyclic or acyclic <code>GradientPaint</code>
		''' object depending on the <code>boolean</code> parameter. </summary>
		''' <param name="pt1"> the first specified <code>Point</code>
		''' in user space </param>
		''' <param name="color1"> <code>Color</code> at the first specified
		''' <code>Point</code> </param>
		''' <param name="pt2"> the second specified <code>Point</code>
		''' in user space </param>
		''' <param name="color2"> <code>Color</code> at the second specified
		''' <code>Point</code> </param>
		''' <param name="cyclic"> <code>true</code> if the gradient pattern should cycle
		''' repeatedly between the two colors; <code>false</code> otherwise </param>
		''' <exception cref="NullPointerException"> if either one of colors or points
		''' is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal pt1 As java.awt.geom.Point2D, ByVal color1 As Color, ByVal pt2 As java.awt.geom.Point2D, ByVal color2 As Color, ByVal cyclic As Boolean)
			Me.New(pt1, color1, pt2, color2)
			Me.cyclic = cyclic
		End Sub

		''' <summary>
		''' Returns a copy of the point P1 that anchors the first color. </summary>
		''' <returns> a <seealso cref="Point2D"/> object that is a copy of the point
		''' that anchors the first color of this
		''' <code>GradientPaint</code>. </returns>
		Public Overridable Property point1 As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Float(p1.x, p1.y)
			End Get
		End Property

		''' <summary>
		''' Returns the color C1 anchored by the point P1. </summary>
		''' <returns> a <code>Color</code> object that is the color
		''' anchored by P1. </returns>
		Public Overridable Property color1 As Color
			Get
				Return color1
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the point P2 which anchors the second color. </summary>
		''' <returns> a <seealso cref="Point2D"/> object that is a copy of the point
		''' that anchors the second color of this
		''' <code>GradientPaint</code>. </returns>
		Public Overridable Property point2 As java.awt.geom.Point2D
			Get
				Return New java.awt.geom.Point2D.Float(p2.x, p2.y)
			End Get
		End Property

		''' <summary>
		''' Returns the color C2 anchored by the point P2. </summary>
		''' <returns> a <code>Color</code> object that is the color
		''' anchored by P2. </returns>
		Public Overridable Property color2 As Color
			Get
				Return color2
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the gradient cycles repeatedly
		''' between the two colors C1 and C2. </summary>
		''' <returns> <code>true</code> if the gradient cycles repeatedly
		''' between the two colors; <code>false</code> otherwise. </returns>
		Public Overridable Property cyclic As Boolean
			Get
				Return cyclic
			End Get
		End Property

		''' <summary>
		''' Creates and returns a <seealso cref="PaintContext"/> used to
		''' generate a linear color gradient pattern.
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

			Return New GradientPaintContext(cm, p1, p2, xform, color1, color2, cyclic)
		End Function

		''' <summary>
		''' Returns the transparency mode for this <code>GradientPaint</code>. </summary>
		''' <returns> an integer value representing this <code>GradientPaint</code>
		''' object's transparency mode. </returns>
		''' <seealso cref= Transparency </seealso>
		Public Overridable Property transparency As Integer Implements Transparency.getTransparency
			Get
				Dim a1 As Integer = color1.alpha
				Dim a2 As Integer = color2.alpha
				Return (If((a1 And a2) = &Hff, OPAQUE, TRANSLUCENT))
			End Get
		End Property

	End Class

End Namespace