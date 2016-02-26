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
	''' The default layout manager for Popup menus and menubars.  This
	''' class is an extension of BoxLayout which adds the UIResource tag
	''' so that pluggable L&amp;Fs can distinguish it from user-installed
	''' layout managers on menus.
	''' 
	''' @author Georges Saab
	''' </summary>

	Public Class DefaultMenuLayout
		Inherits BoxLayout
		Implements javax.swing.plaf.UIResource

		Public Sub New(ByVal target As java.awt.Container, ByVal axis As Integer)
			MyBase.New(target, axis)
		End Sub

		Public Overridable Function preferredLayoutSize(ByVal target As java.awt.Container) As java.awt.Dimension
			If TypeOf target Is JPopupMenu Then
				Dim popupMenu As JPopupMenu = CType(target, JPopupMenu)
				sun.swing.MenuItemLayoutHelper.clearUsedClientProperties(popupMenu)
				If popupMenu.componentCount = 0 Then Return New java.awt.Dimension(0, 0)
			End If

			' Make BoxLayout recalculate cached preferred sizes
			MyBase.invalidateLayout(target)

			Return MyBase.preferredLayoutSize(target)
		End Function
	End Class

End Namespace