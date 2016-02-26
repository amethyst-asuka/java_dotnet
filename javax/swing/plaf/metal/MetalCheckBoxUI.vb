Imports javax.swing
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
	''' CheckboxUI implementation for MetalCheckboxUI
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
	''' @author Michael C. Albers
	''' 
	''' </summary>
	Public Class MetalCheckBoxUI
		Inherits MetalRadioButtonUI

		' NOTE: MetalCheckBoxUI inherts from MetalRadioButtonUI instead
		' of BasicCheckBoxUI because we want to pick up all the
		' painting changes made in MetalRadioButtonUI.

		Private Shared ReadOnly METAL_CHECK_BOX_UI_KEY As New Object

		Private Shared ReadOnly propertyPrefix As String = "CheckBox" & "."

		Private defaults_initialized As Boolean = False

		' ********************************
		'         Create PlAF
		' ********************************
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim checkboxUI As MetalCheckBoxUI = CType(appContext.get(METAL_CHECK_BOX_UI_KEY), MetalCheckBoxUI)
			If checkboxUI Is Nothing Then
				checkboxUI = New MetalCheckBoxUI
				appContext.put(METAL_CHECK_BOX_UI_KEY, checkboxUI)
			End If
			Return checkboxUI
		End Function

		Public Property Overrides propertyPrefix As String
			Get
				Return propertyPrefix
			End Get
		End Property

		' ********************************
		'          Defaults
		' ********************************
		Public Overrides Sub installDefaults(ByVal b As AbstractButton)
			MyBase.installDefaults(b)
			If Not defaults_initialized Then
				icon = UIManager.getIcon(propertyPrefix & "icon")
				defaults_initialized = True
			End If
		End Sub

		Protected Friend Overrides Sub uninstallDefaults(ByVal b As AbstractButton)
			MyBase.uninstallDefaults(b)
			defaults_initialized = False
		End Sub

	End Class

End Namespace