Imports System
Imports javax.swing.text
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' HiddenTagView subclasses EditableView to contain a JTextField showing
	''' the element name. When the textfield is edited the element name is
	''' reset. As this inherits from EditableView if the JTextComponent is
	''' not editable, the textfield will not be visible.
	''' 
	''' @author  Scott Violet
	''' </summary>
	Friend Class HiddenTagView
		Inherits EditableView
		Implements DocumentListener

		Friend Sub New(ByVal e As Element)
			MyBase.New(e)
			yAlign = 1
		End Sub

		Protected Friend Overrides Function createComponent() As Component
			Dim tf As New JTextField(element.name)
			Dim doc As Document = document
			Dim font As Font
			If TypeOf doc Is StyledDocument Then
				font = CType(doc, StyledDocument).getFont(attributes)
				tf.font = font
			Else
				font = tf.font
			End If
			tf.document.addDocumentListener(Me)
			updateYAlign(font)

			' Create a panel to wrap the textfield so that the textfields
			' laf border shows through.
			Dim panel As New JPanel(New BorderLayout)
			panel.background = Nothing
			If endTag Then
				panel.border = EndBorder
			Else
				panel.border = StartBorder
			End If
			panel.add(tf)
			Return panel
		End Function

		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			If axis = View.Y_AXIS Then Return yAlign
			Return 0.5f
		End Function

		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			If axis = View.X_AXIS AndAlso visible Then Return Math.Max(30, MyBase.getPreferredSpan(axis))
			Return MyBase.getMinimumSpan(axis)
		End Function

		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			If axis = View.X_AXIS AndAlso visible Then Return Math.Max(30, MyBase.getPreferredSpan(axis))
			Return MyBase.getPreferredSpan(axis)
		End Function

		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If axis = View.X_AXIS AndAlso visible Then Return Math.Max(30, MyBase.getMaximumSpan(axis))
			Return MyBase.getMaximumSpan(axis)
		End Function

		' DocumentListener methods
		Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
			updateModelFromText()
		End Sub

		Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
			updateModelFromText()
		End Sub

		Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
			updateModelFromText()
		End Sub

		' View method
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			If Not isSettingAttributes Then textFromModeldel()
		End Sub

		' local methods

		Friend Overridable Sub updateYAlign(ByVal font As Font)
			Dim c As Container = container
			Dim fm As FontMetrics = If(c IsNot Nothing, c.getFontMetrics(font), Toolkit.defaultToolkit.getFontMetrics(font))
			Dim h As Single = fm.height
			Dim d As Single = fm.descent
			yAlign = If(h > 0, (h - d) / h, 0)
		End Sub

		Friend Overridable Sub resetBorder()
			Dim comp As Component = component

			If comp IsNot Nothing Then
				If endTag Then
					CType(comp, JPanel).border = EndBorder
				Else
					CType(comp, JPanel).border = StartBorder
				End If
			End If
		End Sub

		''' <summary>
		''' This resets the text on the text component we created to match
		''' that of the AttributeSet for the Element we represent.
		''' <p>If this is invoked on the event dispatching thread, this
		''' directly invokes <code>_setTextFromModel</code>, otherwise
		''' <code>SwingUtilities.invokeLater</code> is used to schedule execution
		''' of <code>_setTextFromModel</code>.
		''' </summary>
		Friend Overridable Sub setTextFromModel()
			If SwingUtilities.eventDispatchThread Then
				_setTextFromModel()
			Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				SwingUtilities.invokeLater(New Runnable()
	'			{
	'				public void run()
	'				{
	'					_setTextFromModel();
	'				}
	'			});
			End If
		End Sub

		''' <summary>
		''' This resets the text on the text component we created to match
		''' that of the AttributeSet for the Element we represent.
		''' </summary>
		Friend Overridable Sub _setTextFromModel()
			Dim doc As Document = document
			Try
				isSettingAttributes = True
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
				Dim text As JTextComponent = textComponent
				If text IsNot Nothing Then
					text.text = representedText
					resetBorder()
					Dim host As Container = container
					If host IsNot Nothing Then
						preferenceChanged(Me, True, True)
						host.repaint()
					End If
				End If
			Finally
				isSettingAttributes = False
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			End Try
		End Sub

		''' <summary>
		''' This copies the text from the text component we've created
		''' to the Element's AttributeSet we represent.
		''' <p>If this is invoked on the event dispatching thread, this
		''' directly invokes <code>_updateModelFromText</code>, otherwise
		''' <code>SwingUtilities.invokeLater</code> is used to schedule execution
		''' of <code>_updateModelFromText</code>.
		''' </summary>
		Friend Overridable Sub updateModelFromText()
			If Not isSettingAttributes Then
				If SwingUtilities.eventDispatchThread Then
					_updateModelFromText()
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					SwingUtilities.invokeLater(New Runnable()
	'				{
	'					public void run()
	'					{
	'						_updateModelFromText();
	'					}
	'				});
				End If
			End If
		End Sub

		''' <summary>
		''' This copies the text from the text component we've created
		''' to the Element's AttributeSet we represent.
		''' </summary>
		Friend Overridable Sub _updateModelFromText()
			Dim doc As Document = document
			Dim name As Object = element.attributes.getAttribute(StyleConstants.NameAttribute)
			If (TypeOf name Is HTML.UnknownTag) AndAlso (TypeOf doc Is StyledDocument) Then
				Dim sas As New SimpleAttributeSet
				Dim ___textComponent As JTextComponent = textComponent
				If ___textComponent IsNot Nothing Then
					Dim text As String = ___textComponent.text
					isSettingAttributes = True
					Try
						sas.addAttribute(StyleConstants.NameAttribute, New HTML.UnknownTag(text))
						CType(doc, StyledDocument).characterAttributestes(startOffset, endOffset - startOffset, sas, False)
					Finally
						isSettingAttributes = False
					End Try
				End If
			End If
		End Sub

		Friend Overridable Property textComponent As JTextComponent
			Get
				Dim comp As Component = component
    
				Return If(comp Is Nothing, Nothing, CType(CType(comp, Container).getComponent(0), JTextComponent))
			End Get
		End Property

		Friend Overridable Property representedText As String
			Get
				Dim retValue As String = element.name
				Return If(retValue Is Nothing, "", retValue)
			End Get
		End Property

		Friend Overridable Property endTag As Boolean
			Get
				Dim [as] As AttributeSet = element.attributes
				If [as] IsNot Nothing Then
					Dim [end] As Object = [as].getAttribute(HTML.Attribute.ENDTAG)
					If [end] IsNot Nothing AndAlso (TypeOf [end] Is String) AndAlso CStr([end]).Equals("true") Then Return True
				End If
				Return False
			End Get
		End Property

		''' <summary>
		''' Alignment along the y axis, based on the font of the textfield. </summary>
		Friend yAlign As Single
		''' <summary>
		''' Set to true when setting attributes. </summary>
		Friend isSettingAttributes As Boolean


		' Following are for Borders that used for Unknown tags and comments.
		'
		' Border defines
		Friend Const circleR As Integer = 3
		Friend Shared ReadOnly circleD As Integer = circleR * 2
		Friend Const tagSize As Integer = 6
		Friend Const padding As Integer = 3
		Friend Shared ReadOnly UnknownTagBorderColor As Color = Color.black
		Friend Shared ReadOnly StartBorder As Border = New StartTagBorder
		Friend Shared ReadOnly EndBorder As Border = New EndTagBorder


		<Serializable> _
		Friend Class StartTagBorder
			Implements Border

			Public Overridable Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				g.color = UnknownTagBorderColor
				x += padding
				width -= (padding * 2)
				g.drawLine(x, y + circleR, x, y + height - circleR)
				g.drawArc(x, y + height - circleD - 1, circleD, circleD, 180, 90)
				g.drawArc(x, y, circleD, circleD, 90, 90)
				g.drawLine(x + circleR, y, x + width - tagSize, y)
				g.drawLine(x + circleR, y + height - 1, x + width - tagSize, y + height - 1)

				g.drawLine(x + width - tagSize, y, x + width - 1, y + height \ 2)
				g.drawLine(x + width - tagSize, y + height, x + width - 1, y + height \ 2)
			End Sub

			Public Overridable Function getBorderInsets(ByVal c As Component) As Insets
				Return New Insets(2, 2 + padding, 2, tagSize + 2 + padding)
			End Function

			Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
				Get
					Return False
				End Get
			End Property
		End Class ' End of class HiddenTagView.StartTagBorder


		<Serializable> _
		Friend Class EndTagBorder
			Implements Border

			Public Overridable Sub paintBorder(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				g.color = UnknownTagBorderColor
				x += padding
				width -= (padding * 2)
				g.drawLine(x + width - 1, y + circleR, x + width - 1, y + height - circleR)
				g.drawArc(x + width - circleD - 1, y + height - circleD - 1, circleD, circleD, 270, 90)
				g.drawArc(x + width - circleD - 1, y, circleD, circleD, 0, 90)
				g.drawLine(x + tagSize, y, x + width - circleR, y)
				g.drawLine(x + tagSize, y + height - 1, x + width - circleR, y + height - 1)

				g.drawLine(x + tagSize, y, x, y + height \ 2)
				g.drawLine(x + tagSize, y + height, x, y + height \ 2)
			End Sub

			Public Overridable Function getBorderInsets(ByVal c As Component) As Insets
				Return New Insets(2, tagSize + 2 + padding, 2, 2 + padding)
			End Function

			Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
				Get
					Return False
				End Get
			End Property
		End Class ' End of class HiddenTagView.EndTagBorder


	End Class ' End of HiddenTagView

End Namespace