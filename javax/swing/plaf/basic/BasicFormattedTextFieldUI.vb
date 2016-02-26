Imports javax.swing

'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the look and feel implementation for
	''' <code>JFormattedTextField</code>.
	''' 
	''' @since 1.4
	''' </summary>
	Public Class BasicFormattedTextFieldUI
		Inherits BasicTextFieldUI

		''' <summary>
		''' Creates a UI for a JFormattedTextField.
		''' </summary>
		''' <param name="c"> the formatted text field </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicFormattedTextFieldUI
		End Function

		''' <summary>
		''' Fetches the name used as a key to lookup properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name "FormattedTextField" </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "FormattedTextField"
			End Get
		End Property
	End Class

End Namespace