Imports javax.swing
Imports javax.swing.text
Imports javax.swing.event
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the Windows look and feel for a password field.
	''' The only difference from the standard text field is that
	''' the view of the text is simply a string of the echo
	''' character as specified in JPasswordField, rather than the
	''' real text contained in the field.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class BasicPasswordFieldUI
		Inherits BasicTextFieldUI

		''' <summary>
		''' Creates a UI for a JPasswordField.
		''' </summary>
		''' <param name="c"> the JPasswordField </param>
		''' <returns> the UI </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicPasswordFieldUI
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
		''' Installs the necessary properties on the JPasswordField.
		''' @since 1.6
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()
			Dim prefix As String = propertyPrefix
			Dim echoChar As Char? = CChar(UIManager.defaults(prefix & ".echoChar"))
			If echoChar IsNot Nothing Then LookAndFeel.installProperty(component, "echoChar", echoChar)
		End Sub

		''' <summary>
		''' Creates a view (PasswordView) for an element.
		''' </summary>
		''' <param name="elem"> the element </param>
		''' <returns> the view </returns>
		Public Overrides Function create(ByVal elem As Element) As View
			Return New PasswordView(elem)
		End Function

		''' <summary>
		''' Create the action map for Password Field.  This map provides
		''' same actions for double mouse click and
		''' and for triple mouse click (see bug 4231444).
		''' </summary>

		Friend Overrides Function createActionMap() As ActionMap
			Dim map As ActionMap = MyBase.createActionMap()
			If map.get(DefaultEditorKit.selectWordAction) IsNot Nothing Then
				Dim a As Action = map.get(DefaultEditorKit.selectLineAction)
				If a IsNot Nothing Then
					map.remove(DefaultEditorKit.selectWordAction)
					map.put(DefaultEditorKit.selectWordAction, a)
				End If
			End If
			Return map
		End Function

	End Class

End Namespace