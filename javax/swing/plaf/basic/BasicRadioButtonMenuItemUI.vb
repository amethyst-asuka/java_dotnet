Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.border

'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' BasicRadioButtonMenuItem implementation
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' </summary>
	Public Class BasicRadioButtonMenuItemUI
		Inherits BasicMenuItemUI

		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Return New BasicRadioButtonMenuItemUI
		End Function

		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "RadioButtonMenuItem"
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void processMouseEvent(JMenuItem item,MouseEvent e,MenuElement path() ,MenuSelectionManager manager)
			Dim p As Point = e.point
			If p.x >= 0 AndAlso p.x < item.width AndAlso p.y >= 0 AndAlso p.y < item.height Then
				If e.iD = MouseEvent.MOUSE_RELEASED Then
					manager.clearSelectedPath()
					item.doClick(0)
					item.armed = False
				Else
					manager.selectedPath = path
				End If
			ElseIf item.model.armed Then
				Dim newPath As MenuElement() = New MenuElement(path.length-2){}
				Dim i, c As Integer
				i=0
				c=path.length-1
				Do While i<c
					newPath(i) = path(i)
					i += 1
				Loop
				manager.selectedPath = newPath
			End If
	End Class

End Namespace