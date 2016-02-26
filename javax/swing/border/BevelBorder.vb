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
	''' A class which implements a simple two-line bevel border.
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
	Public Class BevelBorder
		Inherits AbstractBorder

		''' <summary>
		''' Raised bevel type. </summary>
		Public Const RAISED As Integer = 0
		''' <summary>
		''' Lowered bevel type. </summary>
		Public Const LOWERED As Integer = 1

		Protected Friend bevelType As Integer
		Protected Friend highlightOuter As java.awt.Color
		Protected Friend highlightInner As java.awt.Color
		Protected Friend shadowInner As java.awt.Color
		Protected Friend shadowOuter As java.awt.Color

		''' <summary>
		''' Creates a bevel border with the specified type and whose
		''' colors will be derived from the background color of the
		''' component passed into the paintBorder method. </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		Public Sub New(ByVal bevelType As Integer)
			Me.bevelType = bevelType
		End Sub

		''' <summary>
		''' Creates a bevel border with the specified type, highlight and
		''' shadow colors. </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		''' <param name="highlight"> the color to use for the bevel highlight </param>
		''' <param name="shadow"> the color to use for the bevel shadow </param>
		Public Sub New(ByVal bevelType As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
			Me.New(bevelType, highlight.brighter(), highlight, shadow, shadow.brighter())
		End Sub

		''' <summary>
		''' Creates a bevel border with the specified type, highlight and
		''' shadow colors.
		''' </summary>
		''' <param name="bevelType"> the type of bevel for the border </param>
		''' <param name="highlightOuterColor"> the color to use for the bevel outer highlight </param>
		''' <param name="highlightInnerColor"> the color to use for the bevel inner highlight </param>
		''' <param name="shadowOuterColor"> the color to use for the bevel outer shadow </param>
		''' <param name="shadowInnerColor"> the color to use for the bevel inner shadow </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal bevelType As Integer, ByVal highlightOuterColor As java.awt.Color, ByVal highlightInnerColor As java.awt.Color, ByVal shadowOuterColor As java.awt.Color, ByVal shadowInnerColor As java.awt.Color)
			Me.New(bevelType)
			Me.highlightOuter = highlightOuterColor
			Me.highlightInner = highlightInnerColor
			Me.shadowOuter = shadowOuterColor
			Me.shadowInner = shadowInnerColor
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
			If bevelType = RAISED Then
				 paintRaisedBevel(c, g, x, y, width, height)

			ElseIf bevelType = LOWERED Then
				 paintLoweredBevel(c, g, x, y, width, height)
			End If
		End Sub

		''' <summary>
		''' Reinitialize the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		Public Overrides Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
			insets.set(2, 2, 2, 2)
			Return insets
		End Function

		''' <summary>
		''' Returns the outer highlight color of the bevel border
		''' when rendered on the specified component.  If no highlight
		''' color was specified at instantiation, the highlight color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the highlight may be derived
		''' @since 1.3 </param>
		Public Overridable Function getHighlightOuterColor(ByVal c As java.awt.Component) As java.awt.Color
			Dim highlight As java.awt.Color = highlightOuterColor
			Return If(highlight IsNot Nothing, highlight, c.background.brighter().brighter())
		End Function

		''' <summary>
		''' Returns the inner highlight color of the bevel border
		''' when rendered on the specified component.  If no highlight
		''' color was specified at instantiation, the highlight color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the highlight may be derived
		''' @since 1.3 </param>
		Public Overridable Function getHighlightInnerColor(ByVal c As java.awt.Component) As java.awt.Color
			Dim highlight As java.awt.Color = highlightInnerColor
			Return If(highlight IsNot Nothing, highlight, c.background.brighter())
		End Function

		''' <summary>
		''' Returns the inner shadow color of the bevel border
		''' when rendered on the specified component.  If no shadow
		''' color was specified at instantiation, the shadow color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the shadow may be derived
		''' @since 1.3 </param>
		Public Overridable Function getShadowInnerColor(ByVal c As java.awt.Component) As java.awt.Color
			Dim shadow As java.awt.Color = shadowInnerColor
			Return If(shadow IsNot Nothing, shadow, c.background.darker())
		End Function

		''' <summary>
		''' Returns the outer shadow color of the bevel border
		''' when rendered on the specified component.  If no shadow
		''' color was specified at instantiation, the shadow color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the shadow may be derived
		''' @since 1.3 </param>
		Public Overridable Function getShadowOuterColor(ByVal c As java.awt.Component) As java.awt.Color
			Dim shadow As java.awt.Color = shadowOuterColor
			Return If(shadow IsNot Nothing, shadow, c.background.darker().darker())
		End Function

		''' <summary>
		''' Returns the outer highlight color of the bevel border.
		''' Will return null if no highlight color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property highlightOuterColor As java.awt.Color
			Get
				Return highlightOuter
			End Get
		End Property

		''' <summary>
		''' Returns the inner highlight color of the bevel border.
		''' Will return null if no highlight color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property highlightInnerColor As java.awt.Color
			Get
				Return highlightInner
			End Get
		End Property

		''' <summary>
		''' Returns the inner shadow color of the bevel border.
		''' Will return null if no shadow color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property shadowInnerColor As java.awt.Color
			Get
				Return shadowInner
			End Get
		End Property

		''' <summary>
		''' Returns the outer shadow color of the bevel border.
		''' Will return null if no shadow color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property shadowOuterColor As java.awt.Color
			Get
				Return shadowOuter
			End Get
		End Property

		''' <summary>
		''' Returns the type of the bevel border.
		''' </summary>
		Public Overridable Property bevelType As Integer
			Get
				Return bevelType
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Friend Overridable Sub paintRaisedBevel(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim oldColor As java.awt.Color = g.color
			Dim h As Integer = height
			Dim w As Integer = width

			g.translate(x, y)

			g.color = getHighlightOuterColor(c)
			g.drawLine(0, 0, 0, h-2)
			g.drawLine(1, 0, w-2, 0)

			g.color = getHighlightInnerColor(c)
			g.drawLine(1, 1, 1, h-3)
			g.drawLine(2, 1, w-3, 1)

			g.color = getShadowOuterColor(c)
			g.drawLine(0, h-1, w-1, h-1)
			g.drawLine(w-1, 0, w-1, h-2)

			g.color = getShadowInnerColor(c)
			g.drawLine(1, h-2, w-2, h-2)
			g.drawLine(w-2, 1, w-2, h-3)

			g.translate(-x, -y)
			g.color = oldColor

		End Sub

		Protected Friend Overridable Sub paintLoweredBevel(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			Dim oldColor As java.awt.Color = g.color
			Dim h As Integer = height
			Dim w As Integer = width

			g.translate(x, y)

			g.color = getShadowInnerColor(c)
			g.drawLine(0, 0, 0, h-1)
			g.drawLine(1, 0, w-1, 0)

			g.color = getShadowOuterColor(c)
			g.drawLine(1, 1, 1, h-2)
			g.drawLine(2, 1, w-2, 1)

			g.color = getHighlightOuterColor(c)
			g.drawLine(1, h-1, w-1, h-1)
			g.drawLine(w-1, 1, w-1, h-2)

			g.color = getHighlightInnerColor(c)
			g.drawLine(2, h-2, w-2, h-2)
			g.drawLine(w-2, 2, w-2, h-3)

			g.translate(-x, -y)
			g.color = oldColor

		End Sub

	End Class

End Namespace