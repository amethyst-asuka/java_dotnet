Imports System.Runtime.CompilerServices
Imports javax.swing
Imports javax.swing.plaf.basic
Imports javax.swing.border
Imports javax.swing.plaf

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
	''' RadioButtonUI implementation for MetalRadioButtonUI
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
	''' @author Michael C. Albers (Metal modifications)
	''' @author Jeff Dinkins (original BasicRadioButtonCode)
	''' </summary>
	Public Class MetalRadioButtonUI
		Inherits BasicRadioButtonUI

		Private Shared ReadOnly METAL_RADIO_BUTTON_UI_KEY As New Object

		Protected Friend focusColor As Color
		Protected Friend selectColor As Color
		Protected Friend disabledTextColor As Color

		Private defaults_initialized As Boolean = False

		' ********************************
		'        Create PlAF
		' ********************************
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim ___metalRadioButtonUI As MetalRadioButtonUI = CType(appContext.get(METAL_RADIO_BUTTON_UI_KEY), MetalRadioButtonUI)
			If ___metalRadioButtonUI Is Nothing Then
				___metalRadioButtonUI = New MetalRadioButtonUI
				appContext.put(METAL_RADIO_BUTTON_UI_KEY, ___metalRadioButtonUI)
			End If
			Return ___metalRadioButtonUI
		End Function

		' ********************************
		'        Install Defaults
		' ********************************
		Public Overrides Sub installDefaults(ByVal b As AbstractButton)
			MyBase.installDefaults(b)
			If Not defaults_initialized Then
				focusColor = UIManager.getColor(propertyPrefix & "focus")
				selectColor = UIManager.getColor(propertyPrefix & "select")
				disabledTextColor = UIManager.getColor(propertyPrefix & "disabledText")
				defaults_initialized = True
			End If
			LookAndFeel.installProperty(b, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overrides Sub uninstallDefaults(ByVal b As AbstractButton)
			MyBase.uninstallDefaults(b)
			defaults_initialized = False
		End Sub

		' ********************************
		'         Default Accessors
		' ********************************
		Protected Friend Overridable Property selectColor As Color
			Get
				Return selectColor
			End Get
		End Property

		Protected Friend Overridable Property disabledTextColor As Color
			Get
				Return disabledTextColor
			End Get
		End Property

		Protected Friend Overridable Property focusColor As Color
			Get
				Return focusColor
			End Get
		End Property


		' ********************************
		'        Paint Methods
		' ********************************
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)

			Dim b As AbstractButton = CType(c, AbstractButton)
			Dim model As ButtonModel = b.model

			Dim size As Dimension = c.size

			Dim w As Integer = size.width
			Dim h As Integer = size.height

			Dim f As Font = c.font
			g.font = f
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g, f)

			Dim viewRect As New Rectangle(size)
			Dim iconRect As New Rectangle
			Dim textRect As New Rectangle

			Dim i As Insets = c.insets
			viewRect.x += i.left
			viewRect.y += i.top
			viewRect.width -= (i.right + viewRect.x)
			viewRect.height -= (i.bottom + viewRect.y)

			Dim altIcon As Icon = b.icon
			Dim selectedIcon As Icon = Nothing
			Dim disabledIcon As Icon = Nothing

			Dim text As String = SwingUtilities.layoutCompoundLabel(c, fm, b.text,If(altIcon IsNot Nothing, altIcon, defaultIcon), b.verticalAlignment, b.horizontalAlignment, b.verticalTextPosition, b.horizontalTextPosition, viewRect, iconRect, textRect, b.iconTextGap)

			' fill background
			If c.opaque Then
				g.color = b.background
				g.fillRect(0,0, size.width, size.height)
			End If


			' Paint the radio button
			If altIcon IsNot Nothing Then

				If Not model.enabled Then
					If model.selected Then
					   altIcon = b.disabledSelectedIcon
					Else
					   altIcon = b.disabledIcon
					End If
				ElseIf model.pressed AndAlso model.armed Then
					altIcon = b.pressedIcon
					If altIcon Is Nothing Then altIcon = b.selectedIcon
				ElseIf model.selected Then
					If b.rolloverEnabled AndAlso model.rollover Then
							altIcon = b.rolloverSelectedIcon
							If altIcon Is Nothing Then altIcon = b.selectedIcon
					Else
							altIcon = b.selectedIcon
					End If
				ElseIf b.rolloverEnabled AndAlso model.rollover Then
					altIcon = b.rolloverIcon
				End If

				If altIcon Is Nothing Then altIcon = b.icon

				altIcon.paintIcon(c, g, iconRect.x, iconRect.y)

			Else
				defaultIcon.paintIcon(c, g, iconRect.x, iconRect.y)
			End If


			' Draw the Text
			If text IsNot Nothing Then
				Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
				If v IsNot Nothing Then
					v.paint(g, textRect)
				Else
				   Dim mnemIndex As Integer = b.displayedMnemonicIndex
				   If model.enabled Then
					   ' *** paint the text normally
					   g.color = b.foreground
				   Else
					   ' *** paint the text disabled
					   g.color = disabledTextColor
				   End If
				   sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c,g,text, mnemIndex, textRect.x, textRect.y + fm.ascent)
				End If
			   If b.hasFocus() AndAlso b.focusPainted AndAlso textRect.width > 0 AndAlso textRect.height > 0 Then paintFocus(g,textRect,size)
			End If
		End Sub

		Protected Friend Overrides Sub paintFocus(ByVal g As Graphics, ByVal t As Rectangle, ByVal d As Dimension)
			g.color = focusColor
			g.drawRect(t.x-1, t.y-1, t.width+1, t.height+1)
		End Sub
	End Class

End Namespace