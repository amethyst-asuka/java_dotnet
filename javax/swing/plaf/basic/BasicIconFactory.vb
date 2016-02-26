Imports System
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
	''' Factory object that can vend Icons appropriate for the basic L &amp; F.
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
	''' @author David Kloba
	''' @author Georges Saab
	''' </summary>
	<Serializable> _
	Public Class BasicIconFactory
		Private Shared frame_icon As Icon
		Private Shared checkBoxIcon As Icon
		Private Shared radioButtonIcon As Icon
		Private Shared checkBoxMenuItemIcon As Icon
		Private Shared radioButtonMenuItemIcon As Icon
		Private Shared menuItemCheckIcon As Icon
		Private Shared menuItemArrowIcon As Icon
		Private Shared menuArrowIcon As Icon

		Public Property Shared menuItemCheckIcon As Icon
			Get
				If menuItemCheckIcon Is Nothing Then menuItemCheckIcon = New MenuItemCheckIcon
				Return menuItemCheckIcon
			End Get
		End Property

		Public Property Shared menuItemArrowIcon As Icon
			Get
				If menuItemArrowIcon Is Nothing Then menuItemArrowIcon = New MenuItemArrowIcon
				Return menuItemArrowIcon
			End Get
		End Property

		Public Property Shared menuArrowIcon As Icon
			Get
				If menuArrowIcon Is Nothing Then menuArrowIcon = New MenuArrowIcon
				Return menuArrowIcon
			End Get
		End Property

		Public Property Shared checkBoxIcon As Icon
			Get
				If checkBoxIcon Is Nothing Then checkBoxIcon = New CheckBoxIcon
				Return checkBoxIcon
			End Get
		End Property

		Public Property Shared radioButtonIcon As Icon
			Get
				If radioButtonIcon Is Nothing Then radioButtonIcon = New RadioButtonIcon
				Return radioButtonIcon
			End Get
		End Property

		Public Property Shared checkBoxMenuItemIcon As Icon
			Get
				If checkBoxMenuItemIcon Is Nothing Then checkBoxMenuItemIcon = New CheckBoxMenuItemIcon
				Return checkBoxMenuItemIcon
			End Get
		End Property

		Public Property Shared radioButtonMenuItemIcon As Icon
			Get
				If radioButtonMenuItemIcon Is Nothing Then radioButtonMenuItemIcon = New RadioButtonMenuItemIcon
				Return radioButtonMenuItemIcon
			End Get
		End Property

		Public Shared Function createEmptyFrameIcon() As Icon
			If frame_icon Is Nothing Then frame_icon = New EmptyFrameIcon
			Return frame_icon
		End Function

		<Serializable> _
		Private Class EmptyFrameIcon
			Implements Icon

			Friend height As Integer = 16
			Friend width As Integer = 14
			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return width
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return height
				End Get
			End Property
		End Class

		<Serializable> _
		Private Class CheckBoxIcon
			Implements Icon

			Friend Const csize As Integer = 13
			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return csize
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return csize
				End Get
			End Property
		End Class

		<Serializable> _
		Private Class RadioButtonIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 13
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 13
				End Get
			End Property
		End Class ' end class RadioButtonIcon


		<Serializable> _
		Private Class CheckBoxMenuItemIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
				Dim b As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = b.model
				Dim isSelected As Boolean = model.selected
				If isSelected Then
					g.drawLine(x+7, y+1, x+7, y+3)
					g.drawLine(x+6, y+2, x+6, y+4)
					g.drawLine(x+5, y+3, x+5, y+5)
					g.drawLine(x+4, y+4, x+4, y+6)
					g.drawLine(x+3, y+5, x+3, y+7)
					g.drawLine(x+2, y+4, x+2, y+6)
					g.drawLine(x+1, y+3, x+1, y+5)
				End If
			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 9
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 9
				End Get
			End Property

		End Class ' End class CheckBoxMenuItemIcon


		<Serializable> _
		Private Class RadioButtonMenuItemIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
				Dim b As AbstractButton = CType(c, AbstractButton)
				Dim model As ButtonModel = b.model
				If b.selected = True Then g.fillOval(x+1, y+1, iconWidth, iconHeight)
			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 6
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 6
				End Get
			End Property

		End Class ' End class RadioButtonMenuItemIcon


		<Serializable> _
		Private Class MenuItemCheckIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 9
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 9
				End Get
			End Property

		End Class ' End class MenuItemCheckIcon

		<Serializable> _
		Private Class MenuItemArrowIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 4
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 8
				End Get
			End Property

		End Class ' End class MenuItemArrowIcon

		<Serializable> _
		Private Class MenuArrowIcon
			Implements Icon, javax.swing.plaf.UIResource

			Public Overridable Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer) Implements Icon.paintIcon
				Dim p As New java.awt.Polygon
				p.addPoint(x, y)
				p.addPoint(x+iconWidth, y+iconHeight\2)
				p.addPoint(x, y+iconHeight)
				g.fillPolygon(p)

			End Sub
			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return 4
				End Get
			End Property
			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return 8
				End Get
			End Property
		End Class ' End class MenuArrowIcon
	End Class

End Namespace