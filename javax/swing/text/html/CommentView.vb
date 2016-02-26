Imports javax.swing.text
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' CommentView subclasses HiddenTagView to contain a JTextArea showing
	''' a comment. When the textarea is edited the comment is
	''' reset. As this inherits from EditableView if the JTextComponent is
	''' not editable, the textarea will not be visible.
	''' 
	''' @author  Scott Violet
	''' </summary>
	Friend Class CommentView
		Inherits HiddenTagView

		Friend Sub New(ByVal e As Element)
			MyBase.New(e)
		End Sub

		Protected Friend Overrides Function createComponent() As Component
			Dim host As Container = container
			If host IsNot Nothing AndAlso (Not CType(host, JTextComponent).editable) Then Return Nothing
			Dim ta As New JTextArea(representedText)
			Dim doc As Document = document
			Dim font As Font
			If TypeOf doc Is StyledDocument Then
				font = CType(doc, StyledDocument).getFont(attributes)
				ta.font = font
			Else
				font = ta.font
			End If
			updateYAlign(font)
			ta.border = CBorder
			ta.document.addDocumentListener(Me)
			ta.focusable = visible
			Return ta
		End Function

		Friend Overrides Sub resetBorder()
		End Sub

		''' <summary>
		''' This is subclassed to put the text on the Comment attribute of
		''' the Element's AttributeSet.
		''' </summary>
		Friend Overrides Sub _updateModelFromText()
			Dim textC As JTextComponent = textComponent
			Dim doc As Document = document
			If textC IsNot Nothing AndAlso doc IsNot Nothing Then
				Dim text As String = textC.text
				Dim sas As New SimpleAttributeSet
				isSettingAttributes = True
				Try
					sas.addAttribute(HTML.Attribute.COMMENT, text)
					CType(doc, StyledDocument).characterAttributestes(startOffset, endOffset - startOffset, sas, False)
				Finally
					isSettingAttributes = False
				End Try
			End If
		End Sub

		Friend Property Overrides textComponent As JTextComponent
			Get
				Return CType(component, JTextComponent)
			End Get
		End Property

		Friend Property Overrides representedText As String
			Get
				Dim [as] As AttributeSet = element.attributes
				If [as] IsNot Nothing Then
					Dim comment As Object = [as].getAttribute(HTML.Attribute.COMMENT)
					If TypeOf comment Is String Then Return CStr(comment)
				End If
				Return ""
			End Get
		End Property

		Friend Shared ReadOnly CBorder As Border = New CommentBorder
		Friend Const commentPadding As Integer = 3
		Friend Shared ReadOnly commentPaddingD As Integer = commentPadding * 3

		Friend Class CommentBorder
			Inherits LineBorder

			Friend Sub New()
				MyBase.New(Color.black, 1)
			End Sub

			Public Overridable Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				MyBase.paintBorder(c, g, x + commentPadding, y, width - commentPaddingD, height)
			End Sub

			Public Overridable Function getBorderInsets(ByVal c As Component, ByVal insets As Insets) As Insets
				Dim retI As Insets = MyBase.getBorderInsets(c, insets)

				retI.left += commentPadding
				retI.right += commentPadding
				Return retI
			End Function

			Public Property Overrides borderOpaque As Boolean
				Get
					Return False
				End Get
			End Property
		End Class ' End of class CommentView.CommentBorder
	End Class ' End of CommentView

End Namespace