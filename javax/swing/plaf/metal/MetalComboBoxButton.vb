Imports javax.swing.plaf.basic
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.border

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
	''' JButton subclass to help out MetalComboBoxUI
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= MetalComboBoxButton
	''' @author Tom Santos </seealso>
	Public Class MetalComboBoxButton
		Inherits JButton

		Protected Friend comboBox As JComboBox
		Protected Friend listBox As JList
		Protected Friend rendererPane As CellRendererPane
		Protected Friend comboIcon As Icon
		Protected Friend iconOnly As Boolean = False

		Public Property comboBox As JComboBox
			Get
				Return comboBox
			End Get
			Set(ByVal cb As JComboBox)
				comboBox = cb
			End Set
		End Property

		Public Property comboIcon As Icon
			Get
				Return comboIcon
			End Get
			Set(ByVal i As Icon)
				comboIcon = i
			End Set
		End Property

		Public Property iconOnly As Boolean
			Get
				Return iconOnly
			End Get
			Set(ByVal isIconOnly As Boolean)
				iconOnly = isIconOnly
			End Set
		End Property

		Friend Sub New()
			MyBase.New("")
			Dim ___model As DefaultButtonModel = New DefaultButtonModelAnonymousInnerClassHelper
			model = ___model
		End Sub

		Private Class DefaultButtonModelAnonymousInnerClassHelper
			Inherits DefaultButtonModel

			Public Overrides Property armed As Boolean
				Set(ByVal armed As Boolean)
					MyBase.armed = If(pressed, True, armed)
				End Set
			End Property
		End Class

		Public Sub New(ByVal cb As JComboBox, ByVal i As Icon, ByVal pane As CellRendererPane, ByVal list As JList)
			Me.New()
			comboBox = cb
			comboIcon = i
			rendererPane = pane
			listBox = list
			enabled = comboBox.enabled
		End Sub

		Public Sub New(ByVal cb As JComboBox, ByVal i As Icon, ByVal onlyIcon As Boolean, ByVal pane As CellRendererPane, ByVal list As JList)
			Me.New(cb, i, pane, list)
			iconOnly = onlyIcon
		End Sub

		Public Overridable Property focusTraversable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Property enabled As Boolean
			Set(ByVal enabled As Boolean)
				MyBase.enabled = enabled
    
				' Set the background and foreground to the combobox colors.
				If enabled Then
					background = comboBox.background
					foreground = comboBox.foreground
				Else
					background = UIManager.getColor("ComboBox.disabledBackground")
					foreground = UIManager.getColor("ComboBox.disabledForeground")
				End If
			End Set
		End Property

		Public Overrides Sub paintComponent(ByVal g As Graphics)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(comboBox)

			' Paint the button as usual
			MyBase.paintComponent(g)

			Dim ___insets As Insets = insets

			Dim ___width As Integer = width - (___insets.left + ___insets.right)
			Dim ___height As Integer = height - (___insets.top + ___insets.bottom)

			If ___height <= 0 OrElse ___width <= 0 Then Return

			Dim left As Integer = ___insets.left
			Dim top As Integer = ___insets.top
			Dim right As Integer = left + (___width - 1)
			Dim bottom As Integer = top + (___height - 1)

			Dim iconWidth As Integer = 0
			Dim iconLeft As Integer = If(leftToRight, right, left)

			' Paint the icon
			If comboIcon IsNot Nothing Then
				iconWidth = comboIcon.iconWidth
				Dim iconHeight As Integer = comboIcon.iconHeight
				Dim iconTop As Integer = 0

				If iconOnly Then
					iconLeft = (width \ 2) - (iconWidth \ 2)
					iconTop = (height \ 2) - (iconHeight \ 2)
				Else
					If leftToRight Then
						iconLeft = (left + (___width - 1)) - iconWidth
					Else
						iconLeft = left
					End If
					iconTop = (top + ((bottom - top) \ 2)) - (iconHeight \ 2)
				End If

				comboIcon.paintIcon(Me, g, iconLeft, iconTop)

				' Paint the focus
				If comboBox.hasFocus() AndAlso ((Not MetalLookAndFeel.usingOcean()) OrElse comboBox.editable) Then
					g.color = MetalLookAndFeel.focusColor
					g.drawRect(left - 1, top - 1, ___width + 3, ___height + 1)
				End If
			End If

			If MetalLookAndFeel.usingOcean() Then Return

			' Let the renderer paint
			If (Not iconOnly) AndAlso comboBox IsNot Nothing Then
				Dim renderer As ListCellRenderer = comboBox.renderer
				Dim c As Component
				Dim renderPressed As Boolean = model.pressed
				c = renderer.getListCellRendererComponent(listBox, comboBox.selectedItem, -1, renderPressed, False)
				c.font = rendererPane.font

				If model.armed AndAlso model.pressed Then
					If opaque Then c.background = UIManager.getColor("Button.select")
					c.foreground = comboBox.foreground
				ElseIf Not comboBox.enabled Then
					If opaque Then c.background = UIManager.getColor("ComboBox.disabledBackground")
					c.foreground = UIManager.getColor("ComboBox.disabledForeground")
				Else
					c.foreground = comboBox.foreground
					c.background = comboBox.background
				End If


				Dim cWidth As Integer = ___width - (___insets.right + iconWidth)

				' Fix for 4238829: should lay out the JPanel.
				Dim shouldValidate As Boolean = False
				If TypeOf c Is JPanel Then shouldValidate = True

				If leftToRight Then
					rendererPane.paintComponent(g, c, Me, left, top, cWidth, ___height, shouldValidate)
				Else
					rendererPane.paintComponent(g, c, Me, left + iconWidth, top, cWidth, ___height, shouldValidate)
				End If
			End If
		End Sub

		Public Property Overrides minimumSize As Dimension
			Get
				Dim ret As New Dimension
				Dim ___insets As Insets = insets
				ret.width = ___insets.left + comboIcon.iconWidth + ___insets.right
				ret.height = ___insets.bottom + comboIcon.iconHeight + ___insets.top
				Return ret
			End Get
		End Property
	End Class

End Namespace