Imports javax.swing
Imports javax.swing.plaf

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
	''' <seealso cref="javax.swing.JCheckBoxMenuItem"/>.
	''' 
	''' @author Leif Samuelsson
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' @since 1.7
	''' </summary>
	Public Class SynthCheckBoxMenuItemUI
		Inherits SynthMenuItemUI

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthCheckBoxMenuItemUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "CheckBoxMenuItem"
			End Get
		End Property

		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			context.painter.paintCheckBoxMenuItemBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintCheckBoxMenuItemBorder(context, g, x, y, w, h)
		End Sub
	End Class

End Namespace