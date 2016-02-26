Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' BasicPanel implementation
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class BasicPanelUI
		Inherits PanelUI

		' Shared UI object
		Private Shared panelUI As PanelUI

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			If panelUI Is Nothing Then panelUI = New BasicPanelUI
			Return panelUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			Dim p As JPanel = CType(c, JPanel)
			MyBase.installUI(p)
			installDefaults(p)
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			Dim p As JPanel = CType(c, JPanel)
			uninstallDefaults(p)
			MyBase.uninstallUI(c)
		End Sub

		Protected Friend Overridable Sub installDefaults(ByVal p As JPanel)
			LookAndFeel.installColorsAndFont(p, "Panel.background", "Panel.foreground", "Panel.font")
			LookAndFeel.installBorder(p,"Panel.border")
			LookAndFeel.installProperty(p, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults(ByVal p As JPanel)
			LookAndFeel.uninstallBorder(p)
		End Sub


		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim border As Border = c.border
			If TypeOf border Is AbstractBorder Then Return CType(border, AbstractBorder).getBaseline(c, width, height)
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Dim border As Border = c.border
			If TypeOf border Is AbstractBorder Then Return CType(border, AbstractBorder).getBaselineResizeBehavior(c)
			Return Component.BaselineResizeBehavior.OTHER
		End Function
	End Class

End Namespace