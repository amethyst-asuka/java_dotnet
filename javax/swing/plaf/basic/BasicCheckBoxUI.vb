Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' CheckboxUI implementation for BasicCheckboxUI
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
	''' @author Jeff Dinkins
	''' </summary>
	Public Class BasicCheckBoxUI
		Inherits BasicRadioButtonUI

		Private Shared ReadOnly BASIC_CHECK_BOX_UI_KEY As New Object

		Private Shared ReadOnly propertyPrefix As String = "CheckBox" & "."

		' ********************************
		'            Create PLAF
		' ********************************
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Dim appContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim checkboxUI As BasicCheckBoxUI = CType(appContext.get(BASIC_CHECK_BOX_UI_KEY), BasicCheckBoxUI)
			If checkboxUI Is Nothing Then
				checkboxUI = New BasicCheckBoxUI
				appContext.put(BASIC_CHECK_BOX_UI_KEY, checkboxUI)
			End If
			Return checkboxUI
		End Function

		Public Property Overrides propertyPrefix As String
			Get
				Return propertyPrefix
			End Get
		End Property

	End Class

End Namespace