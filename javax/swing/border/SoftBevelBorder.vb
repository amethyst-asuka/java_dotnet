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
	''' A class which implements a raised or lowered bevel with
	''' softened corners.
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
	''' @author Amy Fowler
	''' @author Chester Rose
	''' </summary>
	Public Class SoftBevelBorder
		Inherits BevelBorder

		''' <summary>
		''' Creates a bevel border with the specified type and whose
		''' colors will be derived from the background color of the
		''' component passed into the paintBorder method. </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		Public Sub New(ByVal bevelType As Integer)
			MyBase.New(bevelType)
		End Sub

		''' <summary>
		''' Creates a bevel border with the specified type, highlight and
		''' shadow colors. </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		''' <param name="highlight"> the color to use for the bevel highlight </param>
		''' <param name="shadow"> the color to use for the bevel shadow </param>
		Public Sub New(ByVal bevelType As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
			MyBase.New(bevelType, highlight, shadow)
		End Sub

		''' <summary>
		''' Creates a bevel border with the specified type, highlight
		''' shadow colors. </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		''' <param name="highlightOuterColor"> the color to use for the bevel outer highlight </param>
		''' <param name="highlightInnerColor"> the color to use for the bevel inner highlight </param>
		''' <param name="shadowOuterColor"> the color to use for the bevel outer shadow </param>
		''' <param name="shadowInnerColor"> the color to use for the bevel inner shadow </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal bevelType As Integer, ByVal highlightOuterColor As java.awt.Color, ByVal highlightInnerColor As java.awt.Color, ByVal shadowOuterColor As java.awt.Color, ByVal shadowInnerColor As java.awt.Color)
			MyBase.New(bevelType, highlightOuterColor, highlightInnerColor, shadowOuterColor, shadowInnerColor)
		End Sub

		''' <summary>
		''' Paints the border for the specified component with the specified
		''' position and size. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim oldColor As java.awt.Color = g.color
			g.translate(x, y)

			If bevelType = RAISED Then
				g.color = getHighlightOuterColor(c)
				g.drawLine(0, 0, width-2, 0)
				g.drawLine(0, 0, 0, height-2)
				g.drawLine(1, 1, 1, 1)

				g.color = getHighlightInnerColor(c)
				g.drawLine(2, 1, width-2, 1)
				g.drawLine(1, 2, 1, height-2)
				g.drawLine(2, 2, 2, 2)
				g.drawLine(0, height-1, 0, height-2)
				g.drawLine(width-1, 0, width-1, 0)

				g.color = getShadowOuterColor(c)
				g.drawLine(2, height-1, width-1, height-1)
				g.drawLine(width-1, 2, width-1, height-1)

				g.color = getShadowInnerColor(c)
				g.drawLine(width-2, height-2, width-2, height-2)


			ElseIf bevelType = LOWERED Then
				g.color = getShadowOuterColor(c)
				g.drawLine(0, 0, width-2, 0)
				g.drawLine(0, 0, 0, height-2)
				g.drawLine(1, 1, 1, 1)

				g.color = getShadowInnerColor(c)
				g.drawLine(2, 1, width-2, 1)
				g.drawLine(1, 2, 1, height-2)
				g.drawLine(2, 2, 2, 2)
				g.drawLine(0, height-1, 0, height-2)
				g.drawLine(width-1, 0, width-1, 0)

				g.color = getHighlightOuterColor(c)
				g.drawLine(2, height-1, width-1, height-1)
				g.drawLine(width-1, 2, width-1, height-1)

				g.color = getHighlightInnerColor(c)
				g.drawLine(width-2, height-2, width-2, height-2)
			End If
			g.translate(-x, -y)
			g.color = oldColor
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			insets.set(3, 3, 3, 3)
			Return insets
		End Function

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return False
			End Get
		End Property

	End Class

End Namespace