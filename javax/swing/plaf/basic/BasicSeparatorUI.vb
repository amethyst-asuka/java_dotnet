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
	''' A Basic L&amp;F implementation of SeparatorUI.  This implementation
	''' is a "combined" view/controller.
	''' 
	''' @author Georges Saab
	''' @author Jeff Shapiro
	''' </summary>

	Public Class BasicSeparatorUI
		Inherits javax.swing.plaf.SeparatorUI

		Protected Friend shadow As java.awt.Color
		Protected Friend highlight As java.awt.Color

		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicSeparatorUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			installDefaults(CType(c, JSeparator))
			installListeners(CType(c, JSeparator))
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults(CType(c, JSeparator))
			uninstallListeners(CType(c, JSeparator))
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal s As JSeparator)
			LookAndFeel.installColors(s, "Separator.background", "Separator.foreground")
			LookAndFeel.installProperty(s, "opaque", Boolean.FALSE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal s As JSeparator)
		End Sub

		Protected Friend Overridable Sub installListeners(ByVal s As JSeparator)
		End Sub

		Protected Friend Overridable Sub uninstallListeners(ByVal s As JSeparator)
		End Sub

		Public Overridable Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
			Dim s As java.awt.Dimension = c.size

			If CType(c, JSeparator).orientation = JSeparator.VERTICAL Then
			  g.color = c.foreground
			  g.drawLine(0, 0, 0, s.height)

			  g.color = c.background
			  g.drawLine(1, 0, 1, s.height)
			Else ' HORIZONTAL
			  g.color = c.foreground
			  g.drawLine(0, 0, s.width, 0)

			  g.color = c.background
			  g.drawLine(0, 1, s.width, 1)
			End If
		End Sub

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			If CType(c, JSeparator).orientation = JSeparator.VERTICAL Then
				Return New java.awt.Dimension(2, 0)
			Else
				Return New java.awt.Dimension(0, 2)
			End If
		End Function

		Public Overridable Function getMinimumSize(ByVal c As JComponent) As java.awt.Dimension
			Return Nothing
		End Function
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As java.awt.Dimension
			Return Nothing
		End Function
	End Class

End Namespace