Imports System
Imports javax.swing

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
Namespace javax.swing.plaf.basic



	''' <summary>
	''' JButton object that draws a scaled Arrow in one of the cardinal directions.
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
	Public Class BasicArrowButton
		Inherits JButton
		Implements SwingConstants

			''' <summary>
			''' The direction of the arrow. One of
			''' {@code SwingConstants.NORTH}, {@code SwingConstants.SOUTH},
			''' {@code SwingConstants.EAST} or {@code SwingConstants.WEST}.
			''' </summary>
			Protected Friend direction As Integer

			Private shadow As java.awt.Color
			Private darkShadow As java.awt.Color
			Private highlight As java.awt.Color

			''' <summary>
			''' Creates a {@code BasicArrowButton} whose arrow
			''' is drawn in the specified direction and with the specified
			''' colors.
			''' </summary>
			''' <param name="direction"> the direction of the arrow; one of
			'''        {@code SwingConstants.NORTH}, {@code SwingConstants.SOUTH},
			'''        {@code SwingConstants.EAST} or {@code SwingConstants.WEST} </param>
			''' <param name="background"> the background color of the button </param>
			''' <param name="shadow"> the color of the shadow </param>
			''' <param name="darkShadow"> the color of the dark shadow </param>
			''' <param name="highlight"> the color of the highlight
			''' @since 1.4 </param>
			Public Sub New(ByVal direction As Integer, ByVal background As java.awt.Color, ByVal shadow As java.awt.Color, ByVal darkShadow As java.awt.Color, ByVal highlight As java.awt.Color)
				MyBase.New()
				requestFocusEnabled = False
				direction = direction
				background = background
				Me.shadow = shadow
				Me.darkShadow = darkShadow
				Me.highlight = highlight
			End Sub

			''' <summary>
			''' Creates a {@code BasicArrowButton} whose arrow
			''' is drawn in the specified direction.
			''' </summary>
			''' <param name="direction"> the direction of the arrow; one of
			'''        {@code SwingConstants.NORTH}, {@code SwingConstants.SOUTH},
			'''        {@code SwingConstants.EAST} or {@code SwingConstants.WEST} </param>
			Public Sub New(ByVal direction As Integer)
				Me.New(direction, UIManager.getColor("control"), UIManager.getColor("controlShadow"), UIManager.getColor("controlDkShadow"), UIManager.getColor("controlLtHighlight"))
			End Sub

			''' <summary>
			''' Returns the direction of the arrow.
			''' </summary>
			Public Overridable Property direction As Integer
				Get
					Return direction
				End Get
				Set(ByVal direction As Integer)
					Me.direction = direction
				End Set
			End Property


			Public Overridable Sub paint(ByVal g As java.awt.Graphics)
				Dim origColor As java.awt.Color
				Dim isPressed, isEnabled As Boolean
				Dim w, h, ___size As Integer

				w = size.width
				h = size.height
				origColor = g.color
				isPressed = model.pressed
				isEnabled = enabled

				g.color = background
				g.fillRect(1, 1, w-2, h-2)

				'/ Draw the proper Border
				If border IsNot Nothing AndAlso Not(TypeOf border Is javax.swing.plaf.UIResource) Then
					paintBorder(g)
				ElseIf isPressed Then
					g.color = shadow
					g.drawRect(0, 0, w-1, h-1)
				Else
					' Using the background color set above
					g.drawLine(0, 0, 0, h-1)
					g.drawLine(1, 0, w-2, 0)

					g.color = highlight ' inner 3D border
					g.drawLine(1, 1, 1, h-3)
					g.drawLine(2, 1, w-3, 1)

					g.color = shadow ' inner 3D border
					g.drawLine(1, h-2, w-2, h-2)
					g.drawLine(w-2, 1, w-2, h-3)

					g.color = darkShadow ' black drop shadow  __|
					g.drawLine(0, h-1, w-1, h-1)
					g.drawLine(w-1, h-1, w-1, 0)
				End If

				' If there's no room to draw arrow, bail
				If h < 5 OrElse w < 5 Then
					g.color = origColor
					Return
				End If

				If isPressed Then g.translate(1, 1)

				' Draw the arrow
				___size = Math.Min((h - 4) \ 3, (w - 4) \ 3)
				___size = Math.Max(___size, 2)
				paintTriangle(g, (w - ___size) \ 2, (h - ___size) \ 2, ___size, direction, isEnabled)

				' Reset the Graphics back to it's original settings
				If isPressed Then g.translate(-1, -1)
				g.color = origColor

			End Sub

			''' <summary>
			''' Returns the preferred size of the {@code BasicArrowButton}.
			''' </summary>
			''' <returns> the preferred size </returns>
			Public Property Overrides preferredSize As java.awt.Dimension
				Get
					Return New java.awt.Dimension(16, 16)
				End Get
			End Property

			''' <summary>
			''' Returns the minimum size of the {@code BasicArrowButton}.
			''' </summary>
			''' <returns> the minimum size </returns>
			Public Property Overrides minimumSize As java.awt.Dimension
				Get
					Return New java.awt.Dimension(5, 5)
				End Get
			End Property

			''' <summary>
			''' Returns the maximum size of the {@code BasicArrowButton}.
			''' </summary>
			''' <returns> the maximum size </returns>
			Public Property Overrides maximumSize As java.awt.Dimension
				Get
					Return New java.awt.Dimension(Integer.MaxValue, Integer.MaxValue)
				End Get
			End Property

			''' <summary>
			''' Returns whether the arrow button should get the focus.
			''' {@code BasicArrowButton}s are used as a child component of
			''' composite components such as {@code JScrollBar} and
			''' {@code JComboBox}. Since the composite component typically gets the
			''' focus, this method is overriden to return {@code false}.
			''' </summary>
			''' <returns> {@code false} </returns>
			Public Overridable Property focusTraversable As Boolean
				Get
				  Return False
				End Get
			End Property

			''' <summary>
			''' Paints a triangle.
			''' </summary>
			''' <param name="g"> the {@code Graphics} to draw to </param>
			''' <param name="x"> the x coordinate </param>
			''' <param name="y"> the y coordinate </param>
			''' <param name="size"> the size of the triangle to draw </param>
			''' <param name="direction"> the direction in which to draw the arrow;
			'''        one of {@code SwingConstants.NORTH},
			'''        {@code SwingConstants.SOUTH}, {@code SwingConstants.EAST} or
			'''        {@code SwingConstants.WEST} </param>
			''' <param name="isEnabled"> whether or not the arrow is drawn enabled </param>
			Public Overridable Sub paintTriangle(ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal size As Integer, ByVal direction As Integer, ByVal isEnabled As Boolean)
				Dim oldColor As java.awt.Color = g.color
				Dim mid, i, j As Integer

				j = 0
				size = Math.Max(size, 2)
				mid = (size \ 2) - 1

				g.translate(x, y)
				If isEnabled Then
					g.color = darkShadow
				Else
					g.color = shadow
				End If

				Select Case direction
				Case NORTH
					For i = 0 To size - 1
						g.drawLine(mid-i, i, mid+i, i)
					Next i
					If Not isEnabled Then
						g.color = highlight
						g.drawLine(mid-i+2, i, mid+i, i)
					End If
				Case SOUTH
					If Not isEnabled Then
						g.translate(1, 1)
						g.color = highlight
						For i = size-1 To 0 Step -1
							g.drawLine(mid-i, j, mid+i, j)
							j += 1
						Next i
						g.translate(-1, -1)
						g.color = shadow
					End If

					j = 0
					For i = size-1 To 0 Step -1
						g.drawLine(mid-i, j, mid+i, j)
						j += 1
					Next i
				Case WEST
					For i = 0 To size - 1
						g.drawLine(i, mid-i, i, mid+i)
					Next i
					If Not isEnabled Then
						g.color = highlight
						g.drawLine(i, mid-i+2, i, mid+i)
					End If
				Case EAST
					If Not isEnabled Then
						g.translate(1, 1)
						g.color = highlight
						For i = size-1 To 0 Step -1
							g.drawLine(j, mid-i, j, mid+i)
							j += 1
						Next i
						g.translate(-1, -1)
						g.color = shadow
					End If

					j = 0
					For i = size-1 To 0 Step -1
						g.drawLine(j, mid-i, j, mid+i)
						j += 1
					Next i
				End Select
				g.translate(-x, -y)
				g.color = oldColor
			End Sub

	End Class

End Namespace