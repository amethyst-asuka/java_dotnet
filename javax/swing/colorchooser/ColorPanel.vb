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


	Friend NotInheritable Class ColorPanel
		Inherits javax.swing.JPanel
		Implements java.awt.event.ActionListener

		Private ReadOnly spinners As SlidingSpinner() = New SlidingSpinner(4){}
		Private ReadOnly values As Single() = New Single(Me.spinners.Length - 1){}

		Private ReadOnly model As ColorModel
		Private color As java.awt.Color
		Private x As Integer = 1
		Private y As Integer = 2
		Private z As Integer

		Friend Sub New(ByVal model As ColorModel)
			MyBase.New(New java.awt.GridBagLayout)

			Dim gbc As New java.awt.GridBagConstraints
			gbc.fill = java.awt.GridBagConstraints.HORIZONTAL

			gbc.gridx = 1
			Dim group As New javax.swing.ButtonGroup
			Dim ___border As javax.swing.border.EmptyBorder = Nothing
			For i As Integer = 0 To Me.spinners.Length - 1
				If i < 3 Then
					Dim button As New javax.swing.JRadioButton
					If i = 0 Then
						Dim ___insets As java.awt.Insets = button.insets
						___insets.left = button.preferredSize.width
						___border = New javax.swing.border.EmptyBorder(___insets)
						button.selected = True
						gbc.insets.top = 5
					End If
					add(button, gbc)
					group.add(button)
					button.actionCommand = Convert.ToString(i)
					button.addActionListener(Me)
					Me.spinners(i) = New SlidingSpinner(Me, button)
				Else
					Dim label As New javax.swing.JLabel
					add(label, gbc)
					label.border = ___border
					label.focusable = False
					Me.spinners(i) = New SlidingSpinner(Me, label)
				End If
			Next i
			gbc.gridx = 2
			gbc.weightx = 1.0
			gbc.insets.top = 0
			gbc.insets.left = 5
			For Each spinner As SlidingSpinner In Me.spinners
				add(spinner.slider, gbc)
				gbc.insets.top = 5
			Next spinner
			gbc.gridx = 3
			gbc.weightx = 0.0
			gbc.insets.top = 0
			For Each spinner As SlidingSpinner In Me.spinners
				add(spinner.spinner, gbc)
				gbc.insets.top = 5
			Next spinner
			focusTraversalPolicy = New java.awt.ContainerOrderFocusTraversalPolicy
			focusTraversalPolicyProvider = True
			focusable = False

			Me.model = model
		End Sub

		Public Sub actionPerformed(ByVal [event] As java.awt.event.ActionEvent)
			Try
				Me.z = Convert.ToInt32([event].actionCommand)
				Me.y = If(Me.z <> 2, 2, 1)
				Me.x = If(Me.z <> 0, 0, 1)
				parent.repaint()
			Catch exception As NumberFormatException
			End Try
		End Sub

		Friend Sub buildPanel()
			Dim count As Integer = Me.model.count
			Me.spinners(4).visible = count > 4
			For i As Integer = 0 To count - 1
				Dim text As String = Me.model.getLabel(Me, i)
				Dim [object] As Object = Me.spinners(i).label
				If TypeOf [object] Is javax.swing.JRadioButton Then
					Dim button As javax.swing.JRadioButton = CType([object], javax.swing.JRadioButton)
					button.text = text
					button.accessibleContext.accessibleDescription = text
				ElseIf TypeOf [object] Is javax.swing.JLabel Then
					Dim label As javax.swing.JLabel = CType([object], javax.swing.JLabel)
					label.text = text
				End If
				Me.spinners(i).rangenge(Me.model.getMinimum(i), Me.model.getMaximum(i))
				Me.spinners(i).value = Me.values(i)
				Me.spinners(i).slider.accessibleContext.accessibleName = text
				Me.spinners(i).spinner.accessibleContext.accessibleName = text
				Dim editor As javax.swing.JSpinner.DefaultEditor = CType(Me.spinners(i).spinner.editor, javax.swing.JSpinner.DefaultEditor)
				editor.textField.accessibleContext.accessibleName = text
				Me.spinners(i).slider.accessibleContext.accessibleDescription = text
				Me.spinners(i).spinner.accessibleContext.accessibleDescription = text
				editor.textField.accessibleContext.accessibleDescription = text
			Next i
		End Sub

		Friend Sub colorChanged()
			Me.color = New java.awt.Color(getColor(0), True)
			Dim parent As Object = parent
			If TypeOf parent Is ColorChooserPanel Then
				Dim chooser As ColorChooserPanel = CType(parent, ColorChooserPanel)
				chooser.selectedColor = Me.color
				chooser.repaint()
			End If
		End Sub

		Friend Property valueX As Single
			Get
				Return Me.spinners(Me.x).value
			End Get
		End Property

		Friend Property valueY As Single
			Get
				Return 1.0f - Me.spinners(Me.y).value
			End Get
		End Property

		Friend Property valueZ As Single
			Get
				Return 1.0f - Me.spinners(Me.z).value
			End Get
		End Property

		Friend Property value As Single
			Set(ByVal z As Single)
				Me.spinners(Me.z).value = 1.0f - z
				colorChanged()
			End Set
		End Property

		Friend Sub setValue(ByVal x As Single, ByVal y As Single)
			Me.spinners(Me.x).value = x
			Me.spinners(Me.y).value = 1.0f - y
			colorChanged()
		End Sub

		Friend Function getColor(ByVal z As Single) As Integer
			defaultValue = Me.x
			defaultValue = Me.y
			Me.values(Me.z) = 1.0f - z
			Return getColor(3)
		End Function

		Friend Function getColor(ByVal x As Single, ByVal y As Single) As Integer
			Me.values(Me.x) = x
			Me.values(Me.y) = 1.0f - y
			value = Me.z
			Return getColor(3)
		End Function

		Friend Property color As java.awt.Color
			Set(ByVal color As java.awt.Color)
				If Not color.Equals(Me.color) Then
					Me.color = color
					Me.model.colorlor(color.rGB, Me.values)
					For i As Integer = 0 To Me.model.count - 1
						Me.spinners(i).value = Me.values(i)
					Next i
				End If
			End Set
		End Property

		Private Function getColor(ByVal index As Integer) As Integer
			Do While index < Me.model.count
				value = index
				index += 1
			Loop
			Return Me.model.getColor(Me.values)
		End Function

		Private Property value As Integer
			Set(ByVal index As Integer)
				Me.values(index) = Me.spinners(index).value
			End Set
		End Property

		Private Property defaultValue As Integer
			Set(ByVal index As Integer)
				Dim ___value As Single = Me.model.getDefault(index)
				Me.values(index) = If(___value < 0.0f, Me.spinners(index).value, ___value)
			End Set
		End Property
	End Class

End Namespace