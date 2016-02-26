'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JFormattedTextField"/>.
	''' 
	''' @since 1.7
	''' </summary>
	Public Class SynthFormattedTextFieldUI
		Inherits SynthTextFieldUI

		''' <summary>
		''' Creates a UI for a JFormattedTextField.
		''' </summary>
		''' <param name="c"> the formatted text field </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthFormattedTextFieldUI
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

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			context.painter.paintFormattedTextFieldBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintFormattedTextFieldBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace