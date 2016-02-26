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
	''' A class which implements a simple etched border which can
	''' either be etched-in or etched-out.  If no highlight/shadow
	''' colors are initialized when the border is created, then
	''' these colors will be dynamically derived from the background
	''' color of the component argument passed into the paintBorder()
	''' method.
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
	''' @author Amy Fowler
	''' </summary>
	Public Class EtchedBorder
		Inherits AbstractBorder

		''' <summary>
		''' Raised etched type. </summary>
		Public Const RAISED As Integer = 0
		''' <summary>
		''' Lowered etched type. </summary>
		Public Const LOWERED As Integer = 1

		Protected Friend etchType As Integer
		Protected Friend highlight As java.awt.Color
		Protected Friend shadow As java.awt.Color

		''' <summary>
		''' Creates a lowered etched border whose colors will be derived
		''' from the background color of the component passed into
		''' the paintBorder method.
		''' </summary>
		Public Sub New()
			Me.New(LOWERED)
		End Sub

		''' <summary>
		''' Creates an etched border with the specified etch-type
		''' whose colors will be derived
		''' from the background color of the component passed into
		''' the paintBorder method. </summary>
		''' <param name="etchType"> the type of etch to be drawn by the border </param>
		Public Sub New(ByVal etchType As Integer)
			Me.New(etchType, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a lowered etched border with the specified highlight and
		''' shadow colors. </summary>
		''' <param name="highlight"> the color to use for the etched highlight </param>
		''' <param name="shadow"> the color to use for the etched shadow </param>
		Public Sub New(ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
			Me.New(LOWERED, highlight, shadow)
		End Sub

		''' <summary>
		''' Creates an etched border with the specified etch-type,
		''' highlight and shadow colors. </summary>
		''' <param name="etchType"> the type of etch to be drawn by the border </param>
		''' <param name="highlight"> the color to use for the etched highlight </param>
		''' <param name="shadow"> the color to use for the etched shadow </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal etchType As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color)
			Me.etchType = etchType
			Me.highlight = highlight
			Me.shadow = shadow
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
			Dim w As Integer = width
			Dim h As Integer = height

			g.translate(x, y)

			g.color = If(etchType = LOWERED, getShadowColor(c), getHighlightColor(c))
			g.drawRect(0, 0, w-2, h-2)

			g.color = If(etchType = LOWERED, getHighlightColor(c), getShadowColor(c))
			g.drawLine(1, h-3, 1, 1)
			g.drawLine(1, 1, w-3, 1)

			g.drawLine(0, h-1, w-1, h-1)
			g.drawLine(w-1, h-1, w-1, 0)

			g.translate(-x, -y)
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
		''' Returns whether or not the border is opaque.
		''' </summary>
		Public Property Overrides borderOpaque As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns which etch-type is set on the etched border.
		''' </summary>
		Public Overridable Property etchType As Integer
			Get
				Return etchType
			End Get
		End Property

		''' <summary>
		''' Returns the highlight color of the etched border
		''' when rendered on the specified component.  If no highlight
		''' color was specified at instantiation, the highlight color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the highlight may be derived
		''' @since 1.3 </param>
		Public Overridable Function getHighlightColor(ByVal c As java.awt.Component) As java.awt.Color
			Return If(highlight IsNot Nothing, highlight, c.background.brighter())
		End Function

		''' <summary>
		''' Returns the highlight color of the etched border.
		''' Will return null if no highlight color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property highlightColor As java.awt.Color
			Get
				Return highlight
			End Get
		End Property

		''' <summary>
		''' Returns the shadow color of the etched border
		''' when rendered on the specified component.  If no shadow
		''' color was specified at instantiation, the shadow color
		''' is derived from the specified component's background color. </summary>
		''' <param name="c"> the component for which the shadow may be derived
		''' @since 1.3 </param>
		Public Overridable Function getShadowColor(ByVal c As java.awt.Component) As java.awt.Color
			Return If(shadow IsNot Nothing, shadow, c.background.darker())
		End Function

		''' <summary>
		''' Returns the shadow color of the etched border.
		''' Will return null if no shadow color was specified
		''' at instantiation.
		''' @since 1.3
		''' </summary>
		Public Overridable Property shadowColor As java.awt.Color
			Get
				Return shadow
			End Get
		End Property

	End Class

End Namespace