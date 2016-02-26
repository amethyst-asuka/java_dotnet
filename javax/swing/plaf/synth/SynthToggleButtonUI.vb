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
	''' <seealso cref="javax.swing.JToggleButton"/>.
	''' 
	''' @author Jeff Dinkins
	''' @since 1.7
	''' </summary>
	Public Class SynthToggleButtonUI
		Inherits SynthButtonUI

		' ********************************
		'          Create PLAF
		' ********************************

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="b"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal b As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthToggleButtonUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "ToggleButton."
			End Get
		End Property

		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			If CType(c, javax.swing.AbstractButton).contentAreaFilled Then
				Dim x As Integer = 0, y As Integer = 0, w As Integer = c.width, h As Integer = c.height
				Dim painter As SynthPainter = context.painter
				painter.paintToggleButtonBackground(context, g, x, y, w, h)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintToggleButtonBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace