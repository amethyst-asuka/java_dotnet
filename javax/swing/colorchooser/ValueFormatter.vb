Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser


	Friend NotInheritable Class ValueFormatter
		Inherits javax.swing.JFormattedTextField.AbstractFormatter
		Implements java.awt.event.FocusListener, Runnable

		Friend Shared Sub init(ByVal length As Integer, ByVal hex As Boolean, ByVal text As javax.swing.JFormattedTextField)
			Dim formatter As New ValueFormatter(length, hex)
			text.columns = length
			text.formatterFactory = New javax.swing.text.DefaultFormatterFactory(formatter)
			text.horizontalAlignment = javax.swing.SwingConstants.RIGHT
			text.minimumSize = text.preferredSize
			text.addFocusListener(formatter)
		End Sub

		Private ReadOnly filter As javax.swing.text.DocumentFilter = New DocumentFilterAnonymousInnerClassHelper

		Private Class DocumentFilterAnonymousInnerClassHelper
			Inherits javax.swing.text.DocumentFilter

			Public Overrides Sub remove(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer)
				If outerInstance.isValid(fb.document.length - length) Then fb.remove(offset, length)
			End Sub

			Public Overrides Sub replace(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal [set] As javax.swing.text.AttributeSet)
				If outerInstance.isValid(fb.document.length + text.Length - length) AndAlso outerInstance.isValid(text) Then fb.replace(offset, length, text.ToUpper(ENGLISH), [set])
			End Sub

			Public Overrides Sub insertString(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal text As String, ByVal [set] As javax.swing.text.AttributeSet)
				If outerInstance.isValid(fb.document.length + text.Length) AndAlso outerInstance.isValid(text) Then fb.insertString(offset, text.ToUpper(ENGLISH), [set])
			End Sub
		End Class

		Private ReadOnly length As Integer
		Private ReadOnly radix As Integer

		Private text As javax.swing.JFormattedTextField

		Friend Sub New(ByVal length As Integer, ByVal hex As Boolean)
			Me.length = length
			Me.radix = If(hex, 16, 10)
		End Sub

		Public Overrides Function stringToValue(ByVal text As String) As Object
			Try
				Return Convert.ToInt32(text, Me.radix)
			Catch nfe As NumberFormatException
				Dim pe As New java.text.ParseException("illegal format", 0)
				pe.initCause(nfe)
				Throw pe
			End Try
		End Function

		Public Overrides Function valueToString(ByVal [object] As Object) As String
			If TypeOf [object] Is Integer? Then
				If Me.radix = 10 Then Return [object].ToString()
				Dim value As Integer = CInt(Fix([object]))
				Dim index As Integer = Me.length
				Dim array As Char() = New Char(index - 1){}
				Dim tempVar As Boolean = 0 < index
				index -= 1
				Do While tempVar
					array(index) = Char.forDigit(value And &HF, Me.radix)
					value >>= 4
					tempVar = 0 < index
					index -= 1
				Loop
				Return (New String(array)).ToUpper(ENGLISH)
			End If
			Throw New java.text.ParseException("illegal object", 0)
		End Function

		Protected Friend Property Overrides documentFilter As javax.swing.text.DocumentFilter
			Get
				Return Me.filter
			End Get
		End Property

		Public Sub focusGained(ByVal [event] As java.awt.event.FocusEvent)
			Dim source As Object = [event].source
			If TypeOf source Is javax.swing.JFormattedTextField Then
				Me.text = CType(source, javax.swing.JFormattedTextField)
				javax.swing.SwingUtilities.invokeLater(Me)
			End If
		End Sub

		Public Sub focusLost(ByVal [event] As java.awt.event.FocusEvent)
		End Sub

		Public Sub run()
			If Me.text IsNot Nothing Then Me.text.selectAll()
		End Sub

		Private Function isValid(ByVal length As Integer) As Boolean
			Return (0 <= length) AndAlso (length <= Me.length)
		End Function

		Private Function isValid(ByVal text As String) As Boolean
			Dim length As Integer = text.Length
			For i As Integer = 0 To length - 1
				Dim ch As Char = text.Chars(i)
				If Char.digit(ch, Me.radix) < 0 Then Return False
			Next i
			Return True
		End Function
	End Class

End Namespace