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
Namespace javax.swing.border


	''' <summary>
	''' A class which implements a line border of arbitrary thickness
	''' and of a single color.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author David Kloba
	''' </summary>
	Public Class LineBorder
		Inherits AbstractBorder

		Private Shared blackLine As Border
		Private Shared grayLine As Border

		Protected Friend thickness As Integer
		Protected Friend lineColor As java.awt.Color
		Protected Friend roundedCorners As Boolean

		''' <summary>
		''' Convenience method for getting the Color.black LineBorder of thickness 1.
		''' </summary>
		Public Shared Function createBlackLineBorder() As Border
			If blackLine Is Nothing Then blackLine = New LineBorder(java.awt.Color.black, 1)
			Return blackLine
		End Function

		''' <summary>
		''' Convenience method for getting the Color.gray LineBorder of thickness 1.
		''' </summary>
		Public Shared Function createGrayLineBorder() As Border
			If grayLine Is Nothing Then grayLine = New LineBorder(java.awt.Color.gray, 1)
			Return grayLine
		End Function

		''' <summary>
		''' Creates a line border with the specified color and a
		''' thickness = 1. </summary>
		''' <param name="color"> the color for the border </param>
		Public Sub New(ByVal color As java.awt.Color)
			Me.New(color, 1, False)
		End Sub

		''' <summary>
		''' Creates a line border with the specified color and thickness. </summary>
		''' <param name="color"> the color of the border </param>
		''' <param name="thickness"> the thickness of the border </param>
		Public Sub New(ByVal color As java.awt.Color, ByVal thickness As Integer)
			Me.New(color, thickness, False)
		End Sub

		''' <summary>
		''' Creates a line border with the specified color, thickness,
		''' and corner shape. </summary>
		''' <param name="color"> the color of the border </param>
		''' <param name="thickness"> the thickness of the border </param>
		''' <param name="roundedCorners"> whether or not border corners should be round
		''' @since 1.3 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal color As java.awt.Color, ByVal thickness As Integer, ByVal roundedCorners As Boolean)
			lineColor = color
			Me.thickness = thickness
			Me.roundedCorners = roundedCorners
		End Sub

		''' <summary>
		''' Paints the border for the specified component with the
		''' specified position and size. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			If (Me.thickness > 0) AndAlso (TypeOf g Is java.awt.Graphics2D) Then
				Dim g2d As java.awt.Graphics2D = CType(g, java.awt.Graphics2D)

				Dim oldColor As java.awt.Color = g2d.color
				g2d.color = Me.lineColor

				Dim outer As java.awt.Shape
				Dim inner As java.awt.Shape

				Dim offs As Integer = Me.thickness
				Dim size As Integer = offs + offs
				If Me.roundedCorners Then
					Dim arc As Single =.2f * offs
					outer = New java.awt.geom.RoundRectangle2D.Float(x, y, width, height, offs, offs)
					inner = New java.awt.geom.RoundRectangle2D.Float(x + offs, y + offs, width - size, height - size, arc, arc)
				Else
					outer = New java.awt.geom.Rectangle2D.Float(x, y, width, height)
					inner = New java.awt.geom.Rectangle2D.Float(x + offs, y + offs, width - size, height - size)
				End If
				Dim path As java.awt.geom.Path2D = New java.awt.geom.Path2D.Float(java.awt.geom.Path2D.WIND_EVEN_ODD)
				path.append(outer, False)
				path.append(inner, False)
				g2d.fill(path)
				g2d.color = oldColor
			End If
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			insets.set(thickness, thickness, thickness, thickness)
			Return insets
		End Function

		''' <summary>
		''' Returns the color of the border.
		''' </summary>
		Public Overridable Property lineColor As java.awt.Color
			Get
				Return lineColor
			End Get
		End Property

		''' <summary>
		''' Returns the thickness of the border.
		''' </summary>
		Public Overridable Property thickness As Integer
			Get
				Return thickness
			End Get
		End Property

		''' <summary>
		''' Returns whether this border will be drawn with rounded corners.
		''' @since 1.3
		''' </summary>
		Public Overridable Property roundedCorners As Boolean
			Get
				Return roundedCorners
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return Not roundedCorners
			End Get
		End Property

	End Class

End Namespace