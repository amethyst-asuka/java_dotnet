Imports System

'
' * Copyright (c) 1998, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Center-positioning layout manager.
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>
	<Serializable> _
	Friend Class CenterLayout
		Implements LayoutManager

		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
		End Sub
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
		End Sub

		Public Overridable Function preferredLayoutSize(ByVal container As Container) As Dimension
			Dim c As Component = container.getComponent(0)
			If c IsNot Nothing Then
				Dim size As Dimension = c.preferredSize
				Dim insets As Insets = container.insets

				Return New Dimension(size.width + insets.left + insets.right, size.height + insets.top + insets.bottom)
			Else
				Return New Dimension(0, 0)
			End If
		End Function

		Public Overridable Function minimumLayoutSize(ByVal cont As Container) As Dimension
			Return preferredLayoutSize(cont)
		End Function

		Public Overridable Sub layoutContainer(ByVal container As Container)
			If container.componentCount > 0 Then
				Dim c As Component = container.getComponent(0)
				Dim pref As Dimension = c.preferredSize
				Dim containerWidth As Integer = container.width
				Dim containerHeight As Integer = container.height
				Dim containerInsets As Insets = container.insets

				containerWidth -= containerInsets.left + containerInsets.right
				containerHeight -= containerInsets.top + containerInsets.bottom

				Dim left As Integer = (containerWidth - pref.width) / 2 + containerInsets.left
				Dim right As Integer = (containerHeight - pref.height) / 2 + containerInsets.top

				c.boundsnds(left, right, pref.width, pref.height)
			End If
		End Sub
	End Class

End Namespace