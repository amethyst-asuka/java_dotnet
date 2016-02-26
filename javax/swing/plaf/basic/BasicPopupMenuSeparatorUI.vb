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

Namespace javax.swing.plaf.basic


	''' <summary>
	''' A Basic L&amp;F implementation of PopupMenuSeparatorUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Jeff Shapiro
	''' </summary>

	Public Class BasicPopupMenuSeparatorUI
		Inherits BasicSeparatorUI

		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicPopupMenuSeparatorUI
		End Function

		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim s As java.awt.Dimension = c.size

			g.color = c.foreground
			g.drawLine(0, 0, s.width, 0)

			g.color = c.background
			g.drawLine(0, 1, s.width, 1)
		End Sub

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Return New java.awt.Dimension(0, 2)
		End Function

	End Class

End Namespace