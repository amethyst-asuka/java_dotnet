Imports javax.swing
Imports javax.swing.text

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
	''' <seealso cref="javax.swing.JPasswordField"/>.
	''' 
	''' @author  Shannon Hickey
	''' @since 1.7
	''' </summary>
	Public Class SynthPasswordFieldUI
		Inherits SynthTextFieldUI

		''' <summary>
		''' Creates a UI for a JPasswordField.
		''' </summary>
		''' <param name="c"> the JPasswordField </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthPasswordFieldUI
		End Function

		''' <summary>
		''' Fetches the name used as a key to look up properties through the
		''' UIManager.  This is used as a prefix to all the standard
		''' text properties.
		''' </summary>
		''' <returns> the name ("PasswordField") </returns>
		Protected Friend Property Overrides propertyPrefix As String
			Get
				Return "PasswordField"
			End Get
		End Property

		''' <summary>
		''' Creates a view (PasswordView) for an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the view </returns>
		Public Overrides Function create(ByVal elem As Element) As View
			Return New PasswordView(elem)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Friend Overrides Sub paintBackground(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal c As JComponent)
			context.painter.paintPasswordFieldBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintPasswordFieldBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installKeyboardActions()
			MyBase.installKeyboardActions()
			Dim map As ActionMap = SwingUtilities.getUIActionMap(component)
			If map IsNot Nothing AndAlso map.get(DefaultEditorKit.selectWordAction) IsNot Nothing Then
				Dim a As Action = map.get(DefaultEditorKit.selectLineAction)
				If a IsNot Nothing Then map.put(DefaultEditorKit.selectWordAction, a)
			End If
		End Sub
	End Class

End Namespace