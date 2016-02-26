Imports javax.swing

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal




	''' <summary>
	''' JButton object for Metal scrollbar arrows.
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
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalScrollButton
		Inherits javax.swing.plaf.basic.BasicArrowButton

	  Private Shared shadowColor As java.awt.Color
	  Private Shared highlightColor As java.awt.Color
	  Private isFreeStanding As Boolean = False

	  Private buttonWidth As Integer

			Public Sub New(ByVal direction As Integer, ByVal width As Integer, ByVal freeStanding As Boolean)
				MyBase.New(direction)

				shadowColor = UIManager.getColor("ScrollBar.darkShadow")
				highlightColor = UIManager.getColor("ScrollBar.highlight")

				buttonWidth = width
				isFreeStanding = freeStanding
			End Sub

			Public Overridable Property freeStanding As Boolean
				Set(ByVal freeStanding As Boolean)
					isFreeStanding = freeStanding
				End Set
			End Property

			Public Overrides Sub paint(ByVal g As java.awt.Graphics)
				Dim leftToRight As Boolean = MetalUtils.isLeftToRight(Me)
				Dim isEnabled As Boolean = parent.enabled

				Dim arrowColor As java.awt.Color = If(isEnabled, MetalLookAndFeel.controlInfo, MetalLookAndFeel.controlDisabled)
				Dim isPressed As Boolean = model.pressed
				Dim ___width As Integer = width
				Dim ___height As Integer = height
				Dim w As Integer = ___width
				Dim h As Integer = ___height
				Dim arrowHeight As Integer = (___height+1) \ 4
				Dim arrowWidth As Integer = (___height+1) \ 2

				If isPressed Then
					g.color = MetalLookAndFeel.controlShadow
				Else
					g.color = background
				End If

				g.fillRect(0, 0, ___width, ___height)

				If direction = NORTH Then
					If Not isFreeStanding Then
						___height +=1
						g.translate(0, -1)
						___width += 2
						If Not leftToRight Then g.translate(-1, 0)
					End If

					' Draw the arrow
					g.color = arrowColor
					Dim startY As Integer = ((h+1) - arrowHeight) \ 2
					Dim startX As Integer = (w \ 2)
					'                  System.out.println( "startX :" + startX + " startY :"+startY);
					For line As Integer = 0 To arrowHeight - 1
						g.drawLine(startX-line, startY+line, startX +line+1, startY+line)
					Next line
	'              g.drawLine( 7, 6, 8, 6 );
	'                g.drawLine( 6, 7, 9, 7 );
	'                g.drawLine( 5, 8, 10, 8 );
	'                g.drawLine( 4, 9, 11, 9 );

					If isEnabled Then
						g.color = highlightColor

						If Not isPressed Then
							g.drawLine(1, 1, ___width - 3, 1)
							g.drawLine(1, 1, 1, ___height - 1)
						End If

						g.drawLine(___width - 1, 1, ___width - 1, ___height - 1)

						g.color = shadowColor
						g.drawLine(0, 0, ___width - 2, 0)
						g.drawLine(0, 0, 0, ___height - 1)
						g.drawLine(___width - 2, 2, ___width - 2, ___height - 1)
					Else
						MetalUtils.drawDisabledBorder(g, 0, 0, ___width, ___height+1)
					End If
					If Not isFreeStanding Then
						___height -= 1
						g.translate(0, 1)
						___width -= 2
						If Not leftToRight Then g.translate(1, 0)
					End If
				ElseIf direction = SOUTH Then
					If Not isFreeStanding Then
						___height += 1
						___width += 2
						If Not leftToRight Then g.translate(-1, 0)
					End If

					' Draw the arrow
					g.color = arrowColor

					Dim startY As Integer = (((h+1) - arrowHeight) \ 2)+ arrowHeight-1
					Dim startX As Integer = (w \ 2)

					'          System.out.println( "startX2 :" + startX + " startY2 :"+startY);

					For line As Integer = 0 To arrowHeight - 1
						g.drawLine(startX-line, startY-line, startX +line+1, startY-line)
					Next line

	'              g.drawLine( 4, 5, 11, 5 );
	'                g.drawLine( 5, 6, 10, 6 );
	'                g.drawLine( 6, 7, 9, 7 );
	'                g.drawLine( 7, 8, 8, 8 ); 

					If isEnabled Then
						g.color = highlightColor

						If Not isPressed Then
							g.drawLine(1, 0, ___width - 3, 0)
							g.drawLine(1, 0, 1, ___height - 3)
						End If

						g.drawLine(1, ___height - 1, ___width - 1, ___height - 1)
						g.drawLine(___width - 1, 0, ___width - 1, ___height - 1)

						g.color = shadowColor
						g.drawLine(0, 0, 0, ___height - 2)
						g.drawLine(___width - 2, 0, ___width - 2, ___height - 2)
						g.drawLine(2, ___height - 2, ___width - 2, ___height - 2)
					Else
						MetalUtils.drawDisabledBorder(g, 0,-1, ___width, ___height+1)
					End If

					If Not isFreeStanding Then
						___height -= 1
						___width -= 2
						If Not leftToRight Then g.translate(1, 0)
					End If
				ElseIf direction = EAST Then
					If Not isFreeStanding Then
						___height += 2
						___width += 1
					End If

					' Draw the arrow
					g.color = arrowColor

					Dim startX As Integer = (((w+1) - arrowHeight) \ 2) + arrowHeight-1
					Dim startY As Integer = (h \ 2)

					'System.out.println( "startX2 :" + startX + " startY2 :"+startY);

					For line As Integer = 0 To arrowHeight - 1
						g.drawLine(startX-line, startY-line, startX -line, startY+line+1)
					Next line


	'              g.drawLine( 5, 4, 5, 11 );
	'                g.drawLine( 6, 5, 6, 10 );
	'                g.drawLine( 7, 6, 7, 9 );
	'                g.drawLine( 8, 7, 8, 8 );

					If isEnabled Then
						g.color = highlightColor

						If Not isPressed Then
							g.drawLine(0, 1, ___width - 3, 1)
							g.drawLine(0, 1, 0, ___height - 3)
						End If

						g.drawLine(___width - 1, 1, ___width - 1, ___height - 1)
						g.drawLine(0, ___height - 1, ___width - 1, ___height - 1)

						g.color = shadowColor
						g.drawLine(0, 0,___width - 2, 0)
						g.drawLine(___width - 2, 2, ___width - 2, ___height - 2)
						g.drawLine(0, ___height - 2, ___width - 2, ___height - 2)
					Else
						MetalUtils.drawDisabledBorder(g,-1,0, ___width+1, ___height)
					End If
					If Not isFreeStanding Then
						___height -= 2
						___width -= 1
					End If
				ElseIf direction = WEST Then
					If Not isFreeStanding Then
						___height += 2
						___width += 1
						g.translate(-1, 0)
					End If

					' Draw the arrow
					g.color = arrowColor

					Dim startX As Integer = (((w+1) - arrowHeight) \ 2)
					Dim startY As Integer = (h \ 2)


					For line As Integer = 0 To arrowHeight - 1
						g.drawLine(startX+line, startY-line, startX +line, startY+line+1)
					Next line

	'              g.drawLine( 6, 7, 6, 8 );
	'                g.drawLine( 7, 6, 7, 9 );
	'                g.drawLine( 8, 5, 8, 10 );
	'                g.drawLine( 9, 4, 9, 11 );

					If isEnabled Then
						g.color = highlightColor


						If Not isPressed Then
							g.drawLine(1, 1, ___width - 1, 1)
							g.drawLine(1, 1, 1, ___height - 3)
						End If

						g.drawLine(1, ___height - 1, ___width - 1, ___height - 1)

						g.color = shadowColor
						g.drawLine(0, 0, ___width - 1, 0)
						g.drawLine(0, 0, 0, ___height - 2)
						g.drawLine(2, ___height - 2, ___width - 1, ___height - 2)
					Else
						MetalUtils.drawDisabledBorder(g,0,0, ___width+1, ___height)
					End If

					If Not isFreeStanding Then
						___height -= 2
						___width -= 1
						g.translate(1, 0)
					End If
				End If
			End Sub

			Public Property Overrides preferredSize As java.awt.Dimension
				Get
					If direction = NORTH Then
						Return New java.awt.Dimension(buttonWidth, buttonWidth - 2)
					ElseIf direction = SOUTH Then
						Return New java.awt.Dimension(buttonWidth, buttonWidth - (If(isFreeStanding, 1, 2)))
					ElseIf direction = EAST Then
						Return New java.awt.Dimension(buttonWidth - (If(isFreeStanding, 1, 2)), buttonWidth)
					ElseIf direction = WEST Then
						Return New java.awt.Dimension(buttonWidth - 2, buttonWidth)
					Else
						Return New java.awt.Dimension(0, 0)
					End If
				End Get
			End Property

			Public Property Overrides minimumSize As java.awt.Dimension
				Get
					Return preferredSize
				End Get
			End Property

			Public Property Overrides maximumSize As java.awt.Dimension
				Get
					Return New java.awt.Dimension(Integer.MaxValue, Integer.MaxValue)
				End Get
			End Property

			Public Overridable Property buttonWidth As Integer
				Get
					Return buttonWidth
				End Get
			End Property
	End Class

End Namespace