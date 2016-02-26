Imports System
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.border

'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' This utility class draws the horizontal bars which indicate a MetalComboBox
	''' </summary>
	''' <seealso cref= MetalComboBoxUI
	''' @author Tom Santos </seealso>
	<Serializable> _
	Public Class MetalComboBoxIcon
		Implements Icon

		''' <summary>
		''' Paints the horizontal bars for the
		''' </summary>
		Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim component As JComponent = CType(c, JComponent)
			Dim ___iconWidth As Integer = iconWidth

			g.translate(x, y)

			g.color = If(component.enabled, MetalLookAndFeel.controlInfo, MetalLookAndFeel.controlShadow)
			g.drawLine(0, 0, ___iconWidth - 1, 0)
			g.drawLine(1, 1, 1 + (___iconWidth - 3), 1)
			g.drawLine(2, 2, 2 + (___iconWidth - 5), 2)
			g.drawLine(3, 3, 3 + (___iconWidth - 7), 3)
			g.drawLine(4, 4, 4 + (___iconWidth - 9), 4)

			g.translate(-x, -y)
		End Sub

		''' <summary>
		''' Created a stub to satisfy the interface.
		''' </summary>
		Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
			Get
				Return 10
			End Get
		End Property

		''' <summary>
		''' Created a stub to satisfy the interface.
		''' </summary>
		Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
			Get
				Return 5
			End Get
		End Property

	End Class

End Namespace