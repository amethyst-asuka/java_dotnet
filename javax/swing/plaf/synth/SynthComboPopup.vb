Imports javax.swing

'
' * Copyright (c) 2002, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth



	''' <summary>
	''' Synth's ComboPopup.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class SynthComboPopup
		Inherits javax.swing.plaf.basic.BasicComboPopup

		Public Sub New(ByVal combo As JComboBox)
			MyBase.New(combo)
		End Sub

		''' <summary>
		''' Configures the list which is used to hold the combo box items in the
		''' popup. This method is called when the UI class
		''' is created.
		''' </summary>
		''' <seealso cref= #createList </seealso>
		Protected Friend Overrides Sub configureList()
			list.font = comboBox.font
			list.cellRenderer = comboBox.renderer
			list.focusable = False
			list.selectionMode = ListSelectionModel.SINGLE_SELECTION
			Dim selectedIndex As Integer = comboBox.selectedIndex
			If selectedIndex = -1 Then
				list.clearSelection()
			Else
				list.selectedIndex = selectedIndex
				list.ensureIndexIsVisible(selectedIndex)
			End If
			installListListeners()
		End Sub

		''' <summary>
		''' @inheritDoc
		''' 
		''' Overridden to take into account any popup insets specified in
		''' SynthComboBoxUI
		''' </summary>
		Protected Friend Overrides Function computePopupBounds(ByVal px As Integer, ByVal py As Integer, ByVal pw As Integer, ByVal ph As Integer) As Rectangle
			Dim ___ui As javax.swing.plaf.ComboBoxUI = comboBox.uI
			If TypeOf ___ui Is SynthComboBoxUI Then
				Dim sui As SynthComboBoxUI = CType(___ui, SynthComboBoxUI)
				If sui.popupInsets IsNot Nothing Then
					Dim i As Insets = sui.popupInsets
					Return MyBase.computePopupBounds(px + i.left, py + i.top, pw - i.left - i.right, ph - i.top - i.bottom)
				End If
			End If
			Return MyBase.computePopupBounds(px, py, pw, ph)
		End Function
	End Class

End Namespace