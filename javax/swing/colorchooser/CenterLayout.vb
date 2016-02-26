Imports System

'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser



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
				size.width += insets.left + insets.right
				size.height += insets.top + insets.bottom
				Return size
			Else
				Return New Dimension(0, 0)
			End If
		End Function

		Public Overridable Function minimumLayoutSize(ByVal cont As Container) As Dimension
			Return preferredLayoutSize(cont)
		End Function

		Public Overridable Sub layoutContainer(ByVal container As Container)
			Try
			   Dim c As Component = container.getComponent(0)

			   c.size = c.preferredSize
			   Dim size As Dimension = c.size
			   Dim containerSize As Dimension = container.size
			   Dim containerInsets As Insets = container.insets
			   containerSize.width -= containerInsets.left + containerInsets.right
			   containerSize.height -= containerInsets.top + containerInsets.bottom
			   Dim componentLeft As Integer = (containerSize.width / 2) - (size.width / 2)
			   Dim componentTop As Integer = (containerSize.height / 2) - (size.height / 2)
			   componentLeft += containerInsets.left
			   componentTop += containerInsets.top

				c.boundsnds(componentLeft, componentTop, size.width, size.height)
			 Catch e As Exception
			 End Try
		End Sub
	End Class

End Namespace