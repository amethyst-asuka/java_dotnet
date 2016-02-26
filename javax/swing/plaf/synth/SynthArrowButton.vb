Imports Microsoft.VisualBasic
Imports System
Imports javax.swing

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' JButton object that draws a scaled Arrow in one of the cardinal directions.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class SynthArrowButton
		Inherits JButton
		Implements SwingConstants, javax.swing.plaf.UIResource

		Private direction As Integer

		Public Sub New(ByVal direction As Integer)
			MyBase.New()
			MyBase.focusable = False
			direction = direction
			defaultCapable = False
		End Sub

		Public Property Overrides uIClassID As String
			Get
				Return "ArrowButtonUI"
			End Get
		End Property

		Public Overrides Sub updateUI()
			uI = New SynthArrowButtonUI
		End Sub

		Public Overridable Property direction As Integer
			Set(ByVal dir As Integer)
				direction = dir
				putClientProperty("__arrow_direction__", Convert.ToInt32(dir))
				repaint()
			End Set
			Get
				Return direction
			End Get
		End Property


		Public Overridable Property focusable As Boolean
			Set(ByVal focusable As Boolean)
			End Set
		End Property

		Private Class SynthArrowButtonUI
			Inherits SynthButtonUI

			Protected Friend Overrides Sub installDefaults(ByVal b As AbstractButton)
				MyBase.installDefaults(b)
				updateStyle(b)
			End Sub

			Protected Friend Overrides Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
				Dim button As SynthArrowButton = CType(context.component, SynthArrowButton)
				context.painter.paintArrowButtonForeground(context, g, 0, 0, button.width, button.height, button.direction)
			End Sub

			Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
				context.painter.paintArrowButtonBackground(context, g, 0, 0, c.width, c.height)
			End Sub

			Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				context.painter.paintArrowButtonBorder(context, g, x, y, w,h)
			End Sub

			Public Overridable Property minimumSize As Dimension
				Get
					Return New Dimension(5, 5)
				End Get
			End Property

			Public Overridable Property maximumSize As Dimension
				Get
					Return New Dimension(Integer.MaxValue, Integer.MaxValue)
				End Get
			End Property

			Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
				Dim ___context As SynthContext = getContext(c)
				Dim [dim] As Dimension = Nothing
				If ___context.component.name = "ScrollBar.button" Then [dim] = CType(___context.style.get(___context, "ScrollBar.buttonSize"), Dimension)
				If [dim] Is Nothing Then
					' For all other cases (including Spinner, ComboBox), we will
					' fall back on the single ArrowButton.size value to create
					' a square return value
					Dim size As Integer = ___context.style.getInt(___context, "ArrowButton.size", 16)
					[dim] = New Dimension(size, size)
				End If

				' handle scaling for sizeVarients for special case components. The
				' key "JComponent.sizeVariant" scales for large/small/mini
				' components are based on Apples LAF
				Dim parent As Container = ___context.component.parent
				If TypeOf parent Is JComponent AndAlso Not(TypeOf parent Is JComboBox) Then
					Dim scaleKey As Object = CType(parent, JComponent).getClientProperty("JComponent.sizeVariant")
					If scaleKey IsNot Nothing Then
						If "large".Equals(scaleKey) Then
							[dim] = New Dimension(CInt(Fix([dim].width * 1.15)), CInt(Fix([dim].height * 1.15)))
						ElseIf "small".Equals(scaleKey) Then
							[dim] = New Dimension(CInt(Fix([dim].width * 0.857)), CInt(Fix([dim].height * 0.857)))
						ElseIf "mini".Equals(scaleKey) Then
							[dim] = New Dimension(CInt(Fix([dim].width * 0.714)), CInt(Fix([dim].height * 0.714)))
						End If
					End If
				End If

				___context.Dispose()
				Return [dim]
			End Function
		End Class
	End Class

End Namespace