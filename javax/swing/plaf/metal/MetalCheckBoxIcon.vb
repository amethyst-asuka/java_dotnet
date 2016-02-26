Imports System
Imports javax.swing
Imports javax.swing.plaf

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
	''' CheckboxIcon implementation for OrganicCheckBoxUI
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
	''' @author Steve Wilson
	''' </summary>
	<Serializable> _
	Public Class MetalCheckBoxIcon
		Implements Icon, UIResource

		Protected Friend Overridable Property controlSize As Integer
			Get
				Return 13
			End Get
		End Property

		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)

			Dim cb As JCheckBox = CType(c, JCheckBox)
			Dim model As ButtonModel = cb.model
			Dim ___controlSize As Integer = controlSize

			Dim drawCheck As Boolean = model.selected

			If model.enabled Then
				If cb.borderPaintedFlat Then
					g.color = MetalLookAndFeel.controlDarkShadow
					g.drawRect(x+1, y, ___controlSize-1, ___controlSize-1)
				End If
				If model.pressed AndAlso model.armed Then
					If cb.borderPaintedFlat Then
						g.color = MetalLookAndFeel.controlShadow
						g.fillRect(x+2, y+1, ___controlSize-2, ___controlSize-2)
					Else
						g.color = MetalLookAndFeel.controlShadow
						g.fillRect(x, y, ___controlSize-1, ___controlSize-1)
						MetalUtils.drawPressed3DBorder(g, x, y, ___controlSize, ___controlSize)
					End If
				ElseIf Not cb.borderPaintedFlat Then
					MetalUtils.drawFlush3DBorder(g, x, y, ___controlSize, ___controlSize)
				End If
				g.color = MetalLookAndFeel.controlInfo
			Else
				g.color = MetalLookAndFeel.controlShadow
				g.drawRect(x, y, ___controlSize-1, ___controlSize-1)
			End If


			If drawCheck Then
				If cb.borderPaintedFlat Then x += 1
				drawCheck(c,g,x,y)
			End If
		End Sub

		Protected Friend Overridable Sub drawCheck(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim ___controlSize As Integer = controlSize
			g.fillRect(x+3, y+5, 2, ___controlSize-8)
			g.drawLine(x+(___controlSize-4), y+3, x+5, y+(___controlSize-6))
			g.drawLine(x+(___controlSize-4), y+4, x+5, y+(___controlSize-5))
		End Sub

		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return controlSize
			End Get
		End Property

		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return controlSize
			End Get
		End Property
	End Class

End Namespace