Imports javax.swing.text
Imports javax.swing

'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A view that supports the &lt;ISINDEX&lt; tag.  This is implemented
	''' as a JPanel that contains
	''' 
	''' @author Sunita Mani
	''' </summary>

	Friend Class IsindexView
		Inherits ComponentView
		Implements ActionListener

		Friend textField As JTextField

		''' <summary>
		''' Creates an IsindexView
		''' </summary>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Creates the components necessary to to implement
		''' this view.  The component returned is a <code>JPanel</code>,
		''' that contains the PROMPT to the left and <code>JTextField</code>
		''' to the right.
		''' </summary>
		Public Overrides Function createComponent() As Component
			Dim attr As AttributeSet = element.attributes

			Dim panel As New JPanel(New BorderLayout)
			panel.background = Nothing

			Dim prompt As String = CStr(attr.getAttribute(HTML.Attribute.PROMPT))
			If prompt Is Nothing Then prompt = UIManager.getString("IsindexView.prompt")
			Dim label As New JLabel(prompt)

			textField = New JTextField
			textField.addActionListener(Me)
			panel.add(label, BorderLayout.WEST)
			panel.add(textField, BorderLayout.CENTER)
			panel.alignmentY = 1.0f
			panel.opaque = False
			Return panel
		End Function

		''' <summary>
		''' Responsible for processing the ActionEvent.
		''' In this case this is hitting enter/return
		''' in the text field.  This will construct the
		''' URL from the base URL of the document.
		''' To the URL is appended a '?' followed by the
		''' contents of the JTextField.  The search
		''' contents are URLEncoded.
		''' </summary>
		Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)

			Dim data As String = textField.text
			If data IsNot Nothing Then data = java.net.URLEncoder.encode(data)


			Dim attr As AttributeSet = element.attributes
			Dim hdoc As HTMLDocument = CType(element.document, HTMLDocument)

			Dim action As String = CStr(attr.getAttribute(HTML.Attribute.ACTION))
			If action Is Nothing Then action = hdoc.base.ToString()
			Try
				Dim url As New java.net.URL(action & "?" & data)
				Dim pane As JEditorPane = CType(container, JEditorPane)
				pane.page = url
			Catch e1 As java.net.MalformedURLException
			Catch e2 As java.io.IOException
			End Try
		End Sub
	End Class

End Namespace