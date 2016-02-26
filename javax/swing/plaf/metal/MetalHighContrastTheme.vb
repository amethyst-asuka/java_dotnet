Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports javax.swing.plaf.metal
Imports javax.swing
Imports javax.swing.border

'
' * Copyright (c) 2001, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' A high contrast theme. This is used on Windows if the system property
	''' awt.highContrast.on is true.
	''' 
	''' @author Michael C. Albers
	''' </summary>
	Friend Class MetalHighContrastTheme
		Inherits DefaultMetalTheme

		Private Shared ReadOnly primary1 As New ColorUIResource(0, 0, 0)
		Private Shared ReadOnly primary2 As New ColorUIResource(204, 204, 204)
		Private Shared ReadOnly primary3 As New ColorUIResource(255, 255, 255)
		Private Shared ReadOnly primaryHighlight As New ColorUIResource(102, 102, 102)
		Private Shared ReadOnly secondary2 As New ColorUIResource(204, 204, 204)
		Private Shared ReadOnly secondary3 As New ColorUIResource(255, 255, 255)
		Private Shared ReadOnly controlHighlight As New ColorUIResource(102, 102, 102)


		' This does not override getSecondary1 (102,102,102)

		Public Property Overrides name As String
			Get
				Return "Contrast"
			End Get
		End Property

		Protected Friend Property Overrides primary1 As ColorUIResource
			Get
				Return primary1
			End Get
		End Property

		Protected Friend Property Overrides primary2 As ColorUIResource
			Get
				Return primary2
			End Get
		End Property

		Protected Friend Property Overrides primary3 As ColorUIResource
			Get
				Return primary3
			End Get
		End Property

		Public Property Overrides primaryControlHighlight As ColorUIResource
			Get
				Return primaryHighlight
			End Get
		End Property

		Protected Friend Property Overrides secondary2 As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		Protected Friend Property Overrides secondary3 As ColorUIResource
			Get
				Return secondary3
			End Get
		End Property

		Public Property Overrides controlHighlight As ColorUIResource
			Get
				' This was super.getSecondary3();
				Return secondary2
			End Get
		End Property

		Public Property Overrides focusColor As ColorUIResource
			Get
				Return black
			End Get
		End Property

		Public Property Overrides textHighlightColor As ColorUIResource
			Get
				Return black
			End Get
		End Property

		Public Property Overrides highlightedTextColor As ColorUIResource
			Get
				Return white
			End Get
		End Property

		Public Property Overrides menuSelectedBackground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		Public Property Overrides menuSelectedForeground As ColorUIResource
			Get
				Return white
			End Get
		End Property

		Public Property Overrides acceleratorForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		Public Property Overrides acceleratorSelectedForeground As ColorUIResource
			Get
				Return white
			End Get
		End Property

		Public Overrides Sub addCustomEntriesToTable(ByVal table As UIDefaults)
			Dim blackLineBorder As Border = New BorderUIResource(New LineBorder(black))
			Dim whiteLineBorder As Border = New BorderUIResource(New LineBorder(white))
			Dim textBorder As Object = New BorderUIResource(New CompoundBorder(blackLineBorder, New BasicBorders.MarginBorder))

			Dim defaults As Object() = { "ToolTip.border", blackLineBorder, "TitledBorder.border", blackLineBorder, "TextField.border", textBorder, "PasswordField.border", textBorder, "TextArea.border", textBorder, "TextPane.border", textBorder, "EditorPane.border", textBorder, "ComboBox.background", windowBackground, "ComboBox.foreground", userTextColor, "ComboBox.selectionBackground", textHighlightColor, "ComboBox.selectionForeground", highlightedTextColor, "ProgressBar.foreground", userTextColor, "ProgressBar.background", windowBackground, "ProgressBar.selectionForeground", windowBackground, "ProgressBar.selectionBackground", userTextColor, "OptionPane.errorDialog.border.background", primary1, "OptionPane.errorDialog.titlePane.foreground", primary3, "OptionPane.errorDialog.titlePane.background", primary1, "OptionPane.errorDialog.titlePane.shadow", primary2, "OptionPane.questionDialog.border.background", primary1, "OptionPane.questionDialog.titlePane.foreground", primary3, "OptionPane.questionDialog.titlePane.background", primary1, "OptionPane.questionDialog.titlePane.shadow", primary2, "OptionPane.warningDialog.border.background", primary1, "OptionPane.warningDialog.titlePane.foreground", primary3, "OptionPane.warningDialog.titlePane.background", primary1, "OptionPane.warningDialog.titlePane.shadow", primary2 }

			table.putDefaults(defaults)
		End Sub

		''' <summary>
		''' Returns true if this is a theme provided by the core platform.
		''' </summary>
		Friend Property Overrides systemTheme As Boolean
			Get
				Return (Me.GetType() = GetType(MetalHighContrastTheme))
			End Get
		End Property
	End Class

End Namespace