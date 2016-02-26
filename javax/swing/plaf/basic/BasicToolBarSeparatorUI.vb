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

Namespace javax.swing.plaf.basic



	''' <summary>
	''' A Basic L&amp;F implementation of ToolBarSeparatorUI.  This implementation
	''' is a "combined" view/controller.
	''' <p>
	''' 
	''' @author Jeff Shapiro
	''' </summary>

	Public Class BasicToolBarSeparatorUI
		Inherits javax.swing.plaf.basic.BasicSeparatorUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicToolBarSeparatorUI
		End Function

		Protected Friend Overrides Sub installDefaults(ByVal s As JSeparator)
			Dim size As java.awt.Dimension = CType(s, javax.swing.JToolBar.Separator).separatorSize

			If size Is Nothing OrElse TypeOf size Is UIResource Then
				Dim sep As javax.swing.JToolBar.Separator = CType(s, javax.swing.JToolBar.Separator)
				size = CType(UIManager.get("ToolBar.separatorSize"), java.awt.Dimension)
				If size IsNot Nothing Then
					If sep.orientation = JSeparator.HORIZONTAL Then size = New java.awt.Dimension(size.height, size.width)
					sep.separatorSize = size
				End If
			End If
		End Sub

		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As JComponent)
		End Sub

		Public Overrides Function getPreferredSize(ByVal c As JComponent) As java.awt.Dimension
			Dim size As java.awt.Dimension = CType(c, javax.swing.JToolBar.Separator).separatorSize

			If size IsNot Nothing Then
				Return size.size
			Else
				Return Nothing
			End If
		End Function
	End Class

End Namespace