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
	''' A Metal L&amp;F implementation of PopupMenuSeparatorUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Jeff Shapiro
	''' </summary>

	Public Class MetalPopupMenuSeparatorUI
		Inherits MetalSeparatorUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalPopupMenuSeparatorUI
		End Function

		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim s As java.awt.Dimension = c.size

			g.color = c.foreground
			g.drawLine(0, 1, s.width, 1)

			g.color = c.background
			g.drawLine(0, 2, s.width, 2)
			g.drawLine(0, 0, 0, 0)
			g.drawLine(0, 3, 0, 3)
		End Sub

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(0, 4)
		End Function
	End Class

End Namespace