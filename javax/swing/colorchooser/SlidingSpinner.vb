Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2008, Oracle and/or its affiliates. All rights reserved.
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


	Friend NotInheritable Class SlidingSpinner
		Implements javax.swing.event.ChangeListener

		Private ReadOnly panel As ColorPanel
		Private ReadOnly label As javax.swing.JComponent
		Private ReadOnly model As New javax.swing.SpinnerNumberModel
		Private ReadOnly slider As New javax.swing.JSlider
		Private ReadOnly spinner As New javax.swing.JSpinner(Me.model)
		Private value As Single
		Private internal As Boolean

		Friend Sub New(ByVal panel As ColorPanel, ByVal label As javax.swing.JComponent)
			Me.panel = panel
			Me.label = label
			Me.slider.addChangeListener(Me)
			Me.spinner.addChangeListener(Me)
			Dim editor As javax.swing.JSpinner.DefaultEditor = CType(Me.spinner.editor, javax.swing.JSpinner.DefaultEditor)
			ValueFormatter.init(3, False, editor.textField)
			editor.focusable = False
			Me.spinner.focusable = False
		End Sub

		Friend Property label As javax.swing.JComponent
			Get
				Return Me.label
			End Get
		End Property

		Friend Property slider As javax.swing.JSlider
			Get
				Return Me.slider
			End Get
		End Property

		Friend Property spinner As javax.swing.JSpinner
			Get
				Return Me.spinner
			End Get
		End Property

		Friend Property value As Single
			Get
				Return Me.value
			End Get
			Set(ByVal value As Single)
				Dim min As Integer = Me.slider.minimum
				Dim max As Integer = Me.slider.maximum
				Me.internal = True
				Me.slider.value = min + CInt(Fix(value * CSng(max - min)))
				Me.spinner.value = Convert.ToInt32(Me.slider.value)
				Me.internal = False
				Me.value = value
			End Set
		End Property


		Friend Sub setRange(ByVal min As Integer, ByVal max As Integer)
			Me.internal = True
			Me.slider.minimum = min
			Me.slider.maximum = max
			Me.model.minimum = Convert.ToInt32(min)
			Me.model.maximum = Convert.ToInt32(max)
			Me.internal = False
		End Sub

		Friend Property visible As Boolean
			Set(ByVal visible As Boolean)
				Me.label.visible = visible
				Me.slider.visible = visible
				Me.spinner.visible = visible
			End Set
		End Property

		Public Sub stateChanged(ByVal [event] As javax.swing.event.ChangeEvent)
			If Not Me.internal Then
				If Me.spinner Is [event].source Then
					Dim ___value As Object = Me.spinner.value
					If TypeOf ___value Is Integer? Then
						Me.internal = True
						Me.slider.value = CInt(Fix(___value))
						Me.internal = False
					End If
				End If
				Dim ___value As Integer = Me.slider.value
				Me.internal = True
				Me.spinner.value = Convert.ToInt32(___value)
				Me.internal = False
				Dim min As Integer = Me.slider.minimum
				Dim max As Integer = Me.slider.maximum
				Me.value = CSng(___value - min) / CSng(max - min)
				Me.panel.colorChanged()
			End If
		End Sub
	End Class

End Namespace