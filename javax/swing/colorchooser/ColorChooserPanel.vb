Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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


	Friend NotInheritable Class ColorChooserPanel
		Inherits AbstractColorChooserPanel
		Implements java.beans.PropertyChangeListener

		Private Const MASK As Integer = &HFF000000L
		Private ReadOnly model As ColorModel
		Private ReadOnly panel As ColorPanel
		Private ReadOnly slider As DiagramComponent
		Private ReadOnly diagram As DiagramComponent
		Private ReadOnly text As javax.swing.JFormattedTextField
		Private ReadOnly label As javax.swing.JLabel

		Friend Sub New(ByVal model As ColorModel)
			Me.model = model
			Me.panel = New ColorPanel(Me.model)
			Me.slider = New DiagramComponent(Me.panel, False)
			Me.diagram = New DiagramComponent(Me.panel, True)
			Me.text = New javax.swing.JFormattedTextField
			Me.label = New javax.swing.JLabel(Nothing, Nothing, javax.swing.SwingConstants.RIGHT)
			ValueFormatter.init(6, True, Me.text)
		End Sub

		Public Overrides Property enabled As Boolean
			Set(ByVal enabled As Boolean)
				MyBase.enabled = enabled
				enabledled(Me, enabled)
			End Set
		End Property

		Private Shared Sub setEnabled(ByVal container As java.awt.Container, ByVal enabled As Boolean)
			For Each component As java.awt.Component In container.components
				component.enabled = enabled
				If TypeOf component Is java.awt.Container Then enabledled(CType(component, java.awt.Container), enabled)
			Next component
		End Sub

		Public Overrides Sub updateChooser()
			Dim color As java.awt.Color = colorFromModel
			If color IsNot Nothing Then
				Me.panel.color = color
				Me.text.value = Convert.ToInt32(color.rGB)
				Me.slider.repaint()
				Me.diagram.repaint()
			End If
		End Sub

		Protected Friend Overrides Sub buildChooser()
			If 0 = componentCount Then
				layout = New java.awt.GridBagLayout

				Dim gbc As New java.awt.GridBagConstraints

				gbc.gridx = 3
				gbc.gridwidth = 2
				gbc.weighty = 1.0
				gbc.anchor = java.awt.GridBagConstraints.NORTH
				gbc.fill = java.awt.GridBagConstraints.HORIZONTAL
				gbc.insets.top = 10
				gbc.insets.right = 10
				add(Me.panel, gbc)

				gbc.gridwidth = 1
				gbc.weightx = 1.0
				gbc.weighty = 0.0
				gbc.anchor = java.awt.GridBagConstraints.CENTER
				gbc.insets.right = 5
				gbc.insets.bottom = 10
				add(Me.label, gbc)

				gbc.gridx = 4
				gbc.weightx = 0.0
				gbc.insets.right = 10
				add(Me.text, gbc)

				gbc.gridx = 2
				gbc.gridheight = 2
				gbc.anchor = java.awt.GridBagConstraints.NORTH
				gbc.ipadx = Me.text.preferredSize.height
				gbc.ipady = preferredSize.height
				add(Me.slider, gbc)

				gbc.gridx = 1
				gbc.insets.left = 10
				gbc.ipadx = gbc.ipady
				add(Me.diagram, gbc)

				Me.label.labelFor = Me.text
				Me.text.addPropertyChangeListener("value", Me) ' NON-NLS: the property name
				Me.slider.border = Me.text.border
				Me.diagram.border = Me.text.border

				inheritsPopupMenuenu(Me, True) ' CR:4966112
			End If
			Dim label As String = Me.model.getText(Me, "HexCode") ' NON-NLS: suffix
			Dim ___visible As Boolean = label IsNot Nothing
			Me.text.visible = ___visible
			Me.text.accessibleContext.accessibleDescription = label
			Me.label.visible = ___visible
			If ___visible Then
				Me.label.text = label
				Dim ___mnemonic As Integer = Me.model.getInteger(Me, "HexCodeMnemonic") ' NON-NLS: suffix
				If ___mnemonic > 0 Then
					Me.label.displayedMnemonic = ___mnemonic
					___mnemonic = Me.model.getInteger(Me, "HexCodeMnemonicIndex") ' NON-NLS: suffix
					If ___mnemonic >= 0 Then Me.label.displayedMnemonicIndex = ___mnemonic
				End If
			End If
			Me.panel.buildPanel()
		End Sub

		Public Property Overrides displayName As String
			Get
				Return Me.model.getText(Me, "Name") ' NON-NLS: suffix
			End Get
		End Property

		Public Property Overrides mnemonic As Integer
			Get
				Return Me.model.getInteger(Me, "Mnemonic") ' NON-NLS: suffix
			End Get
		End Property

		Public Property Overrides displayedMnemonicIndex As Integer
			Get
				Return Me.model.getInteger(Me, "DisplayedMnemonicIndex") ' NON-NLS: suffix
			End Get
		End Property

		Public Property Overrides smallDisplayIcon As javax.swing.Icon
			Get
				Return Nothing
			End Get
		End Property

		Public Property Overrides largeDisplayIcon As javax.swing.Icon
			Get
				Return Nothing
			End Get
		End Property

		Public Sub propertyChange(ByVal [event] As java.beans.PropertyChangeEvent)
			Dim model As ColorSelectionModel = colorSelectionModel
			If model IsNot Nothing Then
				Dim [object] As Object = [event].newValue
				If TypeOf [object] Is Integer? Then
					Dim value As Integer = MASK And model.selectedColor.rGB Or CInt(Fix([object]))
					model.selectedColor = New java.awt.Color(value, True)
				End If
			End If
			Me.text.selectAll()
		End Sub

		''' <summary>
		''' Allows to show context popup for all components recursively.
		''' </summary>
		''' <param name="component">  the root component of the tree </param>
		''' <param name="value">      whether or not the popup menu is inherited </param>
		Private Shared Sub setInheritsPopupMenu(ByVal component As javax.swing.JComponent, ByVal value As Boolean)
			component.inheritsPopupMenu = value
			For Each [object] As Object In component.components
				If TypeOf [object] Is javax.swing.JComponent Then inheritsPopupMenuenu(CType([object], javax.swing.JComponent), value)
			Next [object]
		End Sub
	End Class

End Namespace