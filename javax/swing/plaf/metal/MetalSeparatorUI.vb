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
	''' A Metal L&amp;F implementation of SeparatorUI.  This implementation
	''' is a "combined" view/controller.
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
	''' @author Jeff Shapiro
	''' </summary>

	Public Class MetalSeparatorUI
		Inherits javax.swing.plaf.basic.BasicSeparatorUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New MetalSeparatorUI
		End Function

		Protected Friend Overrides Sub installDefaults(ByVal s As JSeparator)
			LookAndFeel.installColors(s, "Separator.background", "Separator.foreground")
		End Sub

		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
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

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			If CType(c, JSeparator).orientation = JSeparator.VERTICAL Then
				Return New java.awt.Dimension(2, 0)
			Else
				Return New java.awt.Dimension(0, 2)
			End If
		End Function
	End Class

End Namespace