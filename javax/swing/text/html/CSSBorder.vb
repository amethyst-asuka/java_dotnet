Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' CSS-style borders for HTML elements.
	''' 
	''' @author Sergey Groznyh
	''' </summary>
	Friend Class CSSBorder
		Inherits javax.swing.border.AbstractBorder

		''' <summary>
		''' Indices for the attribute groups. </summary>
		Friend Const COLOR As Integer = 0, STYLE As Integer = 1, WIDTH As Integer = 2

		''' <summary>
		''' Indices for the box sides within the attribute group. </summary>
		Friend Const TOP As Integer = 0, RIGHT As Integer = 1, BOTTOM As Integer = 2, LEFT As Integer = 3

		''' <summary>
		''' The attribute groups. </summary>
		Friend Shared ReadOnly ATTRIBUTES As javax.swing.text.html.CSS.Attribute()() = { New javax.swing.text.html.CSS.Attribute() { javax.swing.text.html.CSS.Attribute.BORDER_TOP_COLOR, javax.swing.text.html.CSS.Attribute.BORDER_RIGHT_COLOR, javax.swing.text.html.CSS.Attribute.BORDER_BOTTOM_COLOR, javax.swing.text.html.CSS.Attribute.BORDER_LEFT_COLOR }, New javax.swing.text.html.CSS.Attribute() { javax.swing.text.html.CSS.Attribute.BORDER_TOP_STYLE, javax.swing.text.html.CSS.Attribute.BORDER_RIGHT_STYLE, javax.swing.text.html.CSS.Attribute.BORDER_BOTTOM_STYLE, javax.swing.text.html.CSS.Attribute.BORDER_LEFT_STYLE }, New javax.swing.text.html.CSS.Attribute() { javax.swing.text.html.CSS.Attribute.BORDER_TOP_WIDTH, javax.swing.text.html.CSS.Attribute.BORDER_RIGHT_WIDTH, javax.swing.text.html.CSS.Attribute.BORDER_BOTTOM_WIDTH, javax.swing.text.html.CSS.Attribute.BORDER_LEFT_WIDTH } }

		''' <summary>
		''' Parsers for the border properties. </summary>
		Friend Shared ReadOnly PARSERS As javax.swing.text.html.CSS.CssValue() = { New javax.swing.text.html.CSS.ColorValue, New javax.swing.text.html.CSS.BorderStyle, New javax.swing.text.html.CSS.BorderWidthValue(Nothing, 0) }

		''' <summary>
		''' Default values for the border properties. </summary>
		Friend Shared ReadOnly DEFAULTS As Object() = { javax.swing.text.html.CSS.Attribute.BORDER_COLOR, PARSERS(1).parseCssValue(javax.swing.text.html.CSS.Attribute.BORDER_STYLE.defaultValue), PARSERS(2).parseCssValue(javax.swing.text.html.CSS.Attribute.BORDER_WIDTH.defaultValue) }

		''' <summary>
		''' Attribute set containing border properties. </summary>
		Friend ReadOnly attrs As javax.swing.text.AttributeSet

		''' <summary>
		''' Initialize the attribute set.
		''' </summary>
		Friend Sub New(ByVal attrs As javax.swing.text.AttributeSet)
			Me.attrs = attrs
		End Sub

		''' <summary>
		''' Return the border color for the given side.
		''' </summary>
		Private Function getBorderColor(ByVal side As Integer) As java.awt.Color
			Dim o As Object = attrs.getAttribute(ATTRIBUTES(COLOR)(side))
			Dim cv As javax.swing.text.html.CSS.ColorValue
			If TypeOf o Is javax.swing.text.html.CSS.ColorValue Then
				cv = CType(o, javax.swing.text.html.CSS.ColorValue)
			Else
				' Marker for the default value.  Use 'color' property value as the
				' computed value of the 'border-color' property (CSS2 8.5.2)
				cv = CType(attrs.getAttribute(javax.swing.text.html.CSS.Attribute.COLOR), javax.swing.text.html.CSS.ColorValue)
				If cv Is Nothing Then cv = CType(PARSERS(COLOR).parseCssValue(javax.swing.text.html.CSS.Attribute.COLOR.defaultValue), javax.swing.text.html.CSS.ColorValue)
			End If
			Return cv.value
		End Function

		''' <summary>
		''' Return the border width for the given side.
		''' </summary>
		Private Function getBorderWidth(ByVal side As Integer) As Integer
			Dim ___width As Integer = 0
			Dim bs As javax.swing.text.html.CSS.BorderStyle = CType(attrs.getAttribute(ATTRIBUTES(STYLE)(side)), javax.swing.text.html.CSS.BorderStyle)
			If (bs IsNot Nothing) AndAlso (bs.value IsNot javax.swing.text.html.CSS.Value.NONE) Then
				' The 'border-style' value of "none" forces the computed value
				' of 'border-width' to be 0 (CSS2 8.5.3)
				Dim bw As javax.swing.text.html.CSS.LengthValue = CType(attrs.getAttribute(ATTRIBUTES(WIDTH)(side)), javax.swing.text.html.CSS.LengthValue)
				If bw Is Nothing Then bw = CType(DEFAULTS(WIDTH), javax.swing.text.html.CSS.LengthValue)
				___width = CInt(Fix(bw.getValue(True)))
			End If
			Return ___width
		End Function

		''' <summary>
		''' Return an array of border widths in the TOP, RIGHT, BOTTOM, LEFT order.
		''' </summary>
		Private Property widths As Integer()
			Get
				Dim ___widths As Integer() = New Integer(3){}
				For i As Integer = 0 To ___widths.Length - 1
					___widths(i) = getBorderWidth(i)
				Next i
				Return ___widths
			End Get
		End Property

		''' <summary>
		''' Return the border style for the given side.
		''' </summary>
		Private Function getBorderStyle(ByVal side As Integer) As javax.swing.text.html.CSS.Value
			Dim ___style As javax.swing.text.html.CSS.BorderStyle = CType(attrs.getAttribute(ATTRIBUTES(STYLE)(side)), javax.swing.text.html.CSS.BorderStyle)
			If ___style Is Nothing Then ___style = CType(DEFAULTS(STYLE), javax.swing.text.html.CSS.BorderStyle)
			Return ___style.value
		End Function

		''' <summary>
		''' Return border shape for {@code side} as if the border has zero interior
		''' length.  Shape start is at (0,0); points are added clockwise.
		''' </summary>
		Private Function getBorderShape(ByVal side As Integer) As java.awt.Polygon
			Dim shape As java.awt.Polygon = Nothing
			Dim ___widths As Integer() = widths
			If ___widths(side) <> 0 Then
				shape = New java.awt.Polygon(New Integer(3){}, New Integer(3){}, 0)
				shape.addPoint(0, 0)
				shape.addPoint(-___widths((side + 3) Mod 4), -___widths(side))
				shape.addPoint(___widths((side + 1) Mod 4), -___widths(side))
				shape.addPoint(0, 0)
			End If
			Return shape
		End Function

		''' <summary>
		''' Return the border painter appropriate for the given side.
		''' </summary>
		Private Function getBorderPainter(ByVal side As Integer) As BorderPainter
			Dim ___style As javax.swing.text.html.CSS.Value = getBorderStyle(side)
			Return borderPainters(___style)
		End Function

		''' <summary>
		''' Return the color with brightness adjusted by the specified factor.
		''' 
		''' The factor values are between 0.0 (no change) and 1.0 (turn into white).
		''' Negative factor values decrease brigthness (ie, 1.0 turns into black).
		''' </summary>
		Friend Shared Function getAdjustedColor(ByVal c As java.awt.Color, ByVal factor As Double) As java.awt.Color
			Dim f As Double = 1 - Math.Min(Math.Abs(factor), 1)
			Dim inc As Double = (If(factor > 0, 255 * (1 - f), 0))
			Return New java.awt.Color(CInt(Fix(c.red * f + inc)), CInt(Fix(c.green * f + inc)), CInt(Fix(c.blue * f + inc)))
		End Function


		' The javax.swing.border.Border methods.  

		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			Dim ___widths As Integer() = widths
			insets.set(___widths(TOP), ___widths(LEFT), ___widths(BOTTOM), ___widths(RIGHT))
			Return insets
		End Function

		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			If Not(TypeOf g Is java.awt.Graphics2D) Then Return

			Dim g2 As java.awt.Graphics2D = CType(g.create(), java.awt.Graphics2D)

			Dim ___widths As Integer() = widths

			' Position and size of the border interior.
			Dim intX As Integer = x + ___widths(LEFT)
			Dim intY As Integer = y + ___widths(TOP)
			Dim intWidth As Integer = width - (___widths(RIGHT) + ___widths(LEFT))
			Dim intHeight As Integer = height - (___widths(TOP) + ___widths(BOTTOM))

			' Coordinates of the interior corners, from NW clockwise.
			Dim intCorners As Integer()() = { New Integer() { intX, intY }, New Integer() { intX + intWidth, intY }, New Integer() { intX + intWidth, intY + intHeight }, New Integer() { intX, intY + intHeight } }

			' Draw the borders for all sides.
			For i As Integer = 0 To 3
				Dim ___style As javax.swing.text.html.CSS.Value = getBorderStyle(i)
				Dim shape As java.awt.Polygon = getBorderShape(i)
				If (___style IsNot javax.swing.text.html.CSS.Value.NONE) AndAlso (shape IsNot Nothing) Then
					Dim sideLength As Integer = (If(i Mod 2 = 0, intWidth, intHeight))

					' "stretch" the border shape by the interior area dimension
					shape.xpoints(2) += sideLength
					shape.xpoints(3) += sideLength
					Dim ___color As java.awt.Color = getBorderColor(i)
					Dim painter As BorderPainter = getBorderPainter(i)

					Dim angle As Double = i * Math.PI / 2
					g2.clip = g.clip ' Restore initial clip
					g2.translate(intCorners(i)(0), intCorners(i)(1))
					g2.rotate(angle)
					g2.clip(shape)
					painter.paint(shape, g2, ___color, i)
					g2.rotate(-angle)
					g2.translate(-intCorners(i)(0), -intCorners(i)(1))
				End If
			Next i
			g2.Dispose()
		End Sub


		' Border painters.  

		Friend Interface BorderPainter
			''' <summary>
			''' The painter should paint the border as if it were at the top and the
			''' coordinates of the NW corner of the interior area is (0, 0).  The
			''' caller is responsible for the appropriate affine transformations.
			''' 
			''' Clip is set by the caller to the exact border shape so it's safe to
			''' simply draw into the shape's bounding rectangle.
			''' </summary>
			Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
		End Interface

		''' <summary>
		''' Painter for the "none" and "hidden" CSS border styles.
		''' </summary>
		Friend Class NullPainter
			Implements BorderPainter

			Public Overridable Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				' Do nothing.
			End Sub
		End Class

		''' <summary>
		''' Painter for the "solid" CSS border style.
		''' </summary>
		Friend Class SolidPainter
			Implements BorderPainter

			Public Overridable Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				g.color = color
				g.fillPolygon(shape)
			End Sub
		End Class

		''' <summary>
		''' Defines a method for painting strokes in the specified direction using
		''' the given length and color patterns.
		''' </summary>
		Friend MustInherit Class StrokePainter
			Implements BorderPainter

				Public MustOverride Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
			''' <summary>
			''' Paint strokes repeatedly using the given length and color patterns.
			''' </summary>
			Friend Overridable Sub paintStrokes(ByVal r As java.awt.Rectangle, ByVal g As java.awt.Graphics, ByVal axis As Integer, ByVal lengthPattern As Integer(), ByVal colorPattern As java.awt.Color())
				Dim xAxis As Boolean = (axis = javax.swing.text.View.X_AXIS)
				Dim start As Integer = 0
				Dim [end] As Integer = (If(xAxis, r.width, r.height))
				Do While start < [end]
					For i As Integer = 0 To lengthPattern.Length - 1
						If start >= [end] Then Exit For
						Dim length As Integer = lengthPattern(i)
						Dim c As java.awt.Color = colorPattern(i)
						If c IsNot Nothing Then
							Dim x As Integer = r.x + (If(xAxis, start, 0))
							Dim y As Integer = r.y + (If(xAxis, 0, start))
							Dim width As Integer = If(xAxis, length, r.width)
							Dim height As Integer = If(xAxis, r.height, length)
							g.color = c
							g.fillRect(x, y, width, height)
						End If
						start += length
					Next i
				Loop
			End Sub
		End Class

		''' <summary>
		''' Painter for the "double" CSS border style.
		''' </summary>
		Friend Class DoublePainter
			Inherits StrokePainter

			Public Overrides Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				Dim r As java.awt.Rectangle = shape.bounds
				Dim length As Integer = Math.Max(r.height / 3, 1)
				Dim lengthPattern As Integer() = { length, length }
				Dim colorPattern As java.awt.Color() = { color, Nothing }
				paintStrokes(r, g, javax.swing.text.View.Y_AXIS, lengthPattern, colorPattern)
			End Sub
		End Class

		''' <summary>
		''' Painter for the "dotted" and "dashed" CSS border styles.
		''' </summary>
		Friend Class DottedDashedPainter
			Inherits StrokePainter

			Friend ReadOnly factor As Integer

			Friend Sub New(ByVal factor As Integer)
				Me.factor = factor
			End Sub

			Public Overrides Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				Dim r As java.awt.Rectangle = shape.bounds
				Dim length As Integer = r.height * factor
				Dim lengthPattern As Integer() = { length, length }
				Dim colorPattern As java.awt.Color() = { color, Nothing }
				paintStrokes(r, g, javax.swing.text.View.X_AXIS, lengthPattern, colorPattern)
			End Sub
		End Class

		''' <summary>
		''' Painter that defines colors for "shadow" and "light" border sides.
		''' </summary>
		Friend MustInherit Class ShadowLightPainter
			Inherits StrokePainter

			''' <summary>
			''' Return the "shadow" border side color.
			''' </summary>
			Friend Shared Function getShadowColor(ByVal c As java.awt.Color) As java.awt.Color
				Return CSSBorder.getAdjustedColor(c, -0.3)
			End Function

			''' <summary>
			''' Return the "light" border side color.
			''' </summary>
			Friend Shared Function getLightColor(ByVal c As java.awt.Color) As java.awt.Color
				Return CSSBorder.getAdjustedColor(c, 0.7)
			End Function
		End Class

		''' <summary>
		''' Painter for the "groove" and "ridge" CSS border styles.
		''' </summary>
		Friend Class GrooveRidgePainter
			Inherits ShadowLightPainter

			Friend ReadOnly type As javax.swing.text.html.CSS.Value

			Friend Sub New(ByVal type As javax.swing.text.html.CSS.Value)
				Me.type = type
			End Sub

			Public Overrides Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				Dim r As java.awt.Rectangle = shape.bounds
				Dim length As Integer = Math.Max(r.height / 2, 1)
				Dim lengthPattern As Integer() = { length, length }
				Dim colorPattern As java.awt.Color() = If(((side + 1) Mod 4 < 2) = (type Is javax.swing.text.html.CSS.Value.GROOVE), New java.awt.Color() { getShadowColor(color), getLightColor(color) }, New java.awt.Color){ getLightColor(color), getShadowColor(color) }
				paintStrokes(r, g, javax.swing.text.View.Y_AXIS, lengthPattern, colorPattern)
			End Sub
		End Class

		''' <summary>
		''' Painter for the "inset" and "outset" CSS border styles.
		''' </summary>
		Friend Class InsetOutsetPainter
			Inherits ShadowLightPainter

			Friend type As javax.swing.text.html.CSS.Value

			Friend Sub New(ByVal type As javax.swing.text.html.CSS.Value)
				Me.type = type
			End Sub

			Public Overrides Sub paint(ByVal shape As java.awt.Polygon, ByVal g As java.awt.Graphics, ByVal color As java.awt.Color, ByVal side As Integer)
				g.color = If(((side + 1) Mod 4 < 2) = (type Is javax.swing.text.html.CSS.Value.INSET), getShadowColor(color), getLightColor(color))
				g.fillPolygon(shape)
			End Sub
		End Class

		''' <summary>
		''' Add the specified painter to the painters map.
		''' </summary>
		Friend Shared Sub registerBorderPainter(ByVal style As javax.swing.text.html.CSS.Value, ByVal painter As BorderPainter)
			borderPainters(style) = painter
		End Sub

		''' <summary>
		''' Map the border style values to the border painter objects. </summary>
		Friend Shared borderPainters As IDictionary(Of javax.swing.text.html.CSS.Value, BorderPainter) = New Dictionary(Of javax.swing.text.html.CSS.Value, BorderPainter)

		' Initialize the border painters map with the pre-defined values.  
		Shared Sub New()
			registerBorderPainter(javax.swing.text.html.CSS.Value.NONE, New NullPainter)
			registerBorderPainter(javax.swing.text.html.CSS.Value.HIDDEN, New NullPainter)
			registerBorderPainter(javax.swing.text.html.CSS.Value.SOLID, New SolidPainter)
			registerBorderPainter(javax.swing.text.html.CSS.Value.DOUBLE, New DoublePainter)
			registerBorderPainter(javax.swing.text.html.CSS.Value.DOTTED, New DottedDashedPainter(1))
			registerBorderPainter(javax.swing.text.html.CSS.Value.DASHED, New DottedDashedPainter(3))
			registerBorderPainter(javax.swing.text.html.CSS.Value.GROOVE, New GrooveRidgePainter(javax.swing.text.html.CSS.Value.GROOVE))
			registerBorderPainter(javax.swing.text.html.CSS.Value.RIDGE, New GrooveRidgePainter(javax.swing.text.html.CSS.Value.RIDGE))
			registerBorderPainter(javax.swing.text.html.CSS.Value.INSET, New InsetOutsetPainter(javax.swing.text.html.CSS.Value.INSET))
			registerBorderPainter(javax.swing.text.html.CSS.Value.OUTSET, New InsetOutsetPainter(javax.swing.text.html.CSS.Value.OUTSET))
		End Sub
	End Class

End Namespace