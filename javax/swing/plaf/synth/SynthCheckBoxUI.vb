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
	''' <seealso cref="javax.swing.JCheckBox"/>.
	''' 
	''' @author Jeff Dinkins
	''' @since 1.7
	''' </summary>
	Public Class SynthCheckBoxUI
		Inherits SynthRadioButtonUI

		' ********************************
		'            Create PLAF
		' ********************************
		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="b"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal b As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthCheckBoxUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "CheckBox."
			End Get
		End Property

		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			context.painter.paintCheckBoxBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintCheckBoxBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace