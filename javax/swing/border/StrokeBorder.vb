Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A class which implements a border of an arbitrary stroke.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI
	''' between applications running the same version of Swing.
	''' As of 1.4, support for long term storage of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Sergey A. Malenkov
	''' 
	''' @since 1.7
	''' </summary>
	Public Class StrokeBorder
		Inherits AbstractBorder

		Private ReadOnly stroke As java.awt.BasicStroke
		Private ReadOnly paint As java.awt.Paint

		''' <summary>
		''' Creates a border of the specified {@code stroke}.
		''' The component's foreground color will be used to render the border.
		''' </summary>
		''' <param name="stroke">  the <seealso cref="BasicStroke"/> object used to stroke a shape
		''' </param>
		''' <exception cref="NullPointerException"> if the specified {@code stroke} is {@code null} </exception>
		Public Sub New(ByVal stroke As java.awt.BasicStroke)
			Me.New(stroke, Nothing)
		End Sub

		''' <summary>
		''' Creates a border of the specified {@code stroke} and {@code paint}.
		''' If the specified {@code paint} is {@code null},
		''' the component's foreground color will be used to render the border.
		''' </summary>
		''' <param name="stroke">  the <seealso cref="BasicStroke"/> object used to stroke a shape </param>
		''' <param name="paint">   the <seealso cref="Paint"/> object used to generate a color
		''' </param>
		''' <exception cref="NullPointerException"> if the specified {@code stroke} is {@code null} </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal stroke As java.awt.BasicStroke, ByVal paint As java.awt.Paint)
			If stroke Is Nothing Then Throw New NullPointerException("border's stroke")
			Me.stroke = stroke
			Me.paint = paint
		End Sub

		''' <summary>
		''' Paints the border for the specified component
		''' with the specified position and size.
		''' If the border was not specified with a <seealso cref="Paint"/> object,
		''' the component's foreground color will be used to render the border.
		''' If the component's foreground color is not available,
		''' the default color of the <seealso cref="Graphics"/> object will be used.
		''' </summary>
		''' <param name="c">       the component for which this border is being painted </param>
		''' <param name="g">       the paint graphics </param>
		''' <param name="x">       the x position of the painted border </param>
		''' <param name="y">       the y position of the painted border </param>
		''' <param name="width">   the width of the painted border </param>
		''' <param name="height">  the height of the painted border
		''' </param>
		''' <exception cref="NullPointerException"> if the specified {@code g} is {@code null} </exception>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim size As Single = Me.stroke.lineWidth
			If size > 0.0f Then
				g = g.create()
				If TypeOf g Is java.awt.Graphics2D Then
					Dim g2d As java.awt.Graphics2D = CType(g, java.awt.Graphics2D)
					g2d.stroke = Me.stroke
					g2d.paint = If(Me.paint IsNot Nothing, Me.paint, If(c Is Nothing, Nothing, c.foreground))
					g2d.renderingHintint(java.awt.RenderingHints.KEY_ANTIALIASING, java.awt.RenderingHints.VALUE_ANTIALIAS_ON)
					g2d.draw(New java.awt.geom.Rectangle2D.Float(x + size / 2, y + size / 2, width - size, height - size))
				End If
				g.Dispose()
			End If
		End Sub

		''' <summary>
		''' Reinitializes the {@code insets} parameter
		''' with this border's current insets.
		''' Every inset is the smallest (closest to negative infinity) integer value
		''' that is greater than or equal to the line width of the stroke
		''' that is used to paint the border.
		''' </summary>
		''' <param name="c">       the component for which this border insets value applies </param>
		''' <param name="insets">  the {@code Insets} object to be reinitialized </param>
		''' <returns> the reinitialized {@code insets} parameter
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code insets} is {@code null}
		''' </exception>
		''' <seealso cref= Math#ceil </seealso>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			Dim size As Integer = CInt(Fix(Math.Ceiling(Me.stroke.lineWidth)))
			insets.set(size, size, size, size)
			Return insets
		End Function

		''' <summary>
		''' Returns the <seealso cref="BasicStroke"/> object used to stroke a shape
		''' during the border rendering.
		''' </summary>
		''' <returns> the <seealso cref="BasicStroke"/> object </returns>
		Public Overridable Property stroke As java.awt.BasicStroke
			Get
				Return Me.stroke
			End Get
		End Property

		''' <summary>
		''' Returns the <seealso cref="Paint"/> object used to generate a color
		''' during the border rendering.
		''' </summary>
		''' <returns> the <seealso cref="Paint"/> object or {@code null}
		'''         if the {@code paint} parameter is not set </returns>
		Public Overridable Property paint As java.awt.Paint
			Get
				Return Me.paint
			End Get
		End Property
	End Class

End Namespace