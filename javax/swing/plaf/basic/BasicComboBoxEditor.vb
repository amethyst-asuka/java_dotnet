Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The default editor for editable combo boxes. The editor is implemented as a JTextField.
	''' 
	''' @author Arnaud Weber
	''' @author Mark Davidson
	''' </summary>
	Public Class BasicComboBoxEditor
		Implements javax.swing.ComboBoxEditor, FocusListener

		Protected Friend editor As javax.swing.JTextField
		Private oldValue As Object

		Public Sub New()
			editor = createEditorComponent()
		End Sub

		Public Overridable Property editorComponent As java.awt.Component
			Get
				Return editor
			End Get
		End Property

		''' <summary>
		''' Creates the internal editor component. Override this to provide
		''' a custom implementation.
		''' </summary>
		''' <returns> a new editor component
		''' @since 1.6 </returns>
		Protected Friend Overridable Function createEditorComponent() As javax.swing.JTextField
			Dim editor As javax.swing.JTextField = New BorderlessTextField("",9)
			editor.border = Nothing
			Return editor
		End Function

		''' <summary>
		''' Sets the item that should be edited.
		''' </summary>
		''' <param name="anObject"> the displayed value of the editor </param>
		Public Overridable Property item As Object
			Set(ByVal anObject As Object)
				Dim text As String
    
				If anObject IsNot Nothing Then
					text = anObject.ToString()
					If text Is Nothing Then text = ""
					oldValue = anObject
				Else
					text = ""
				End If
				' workaround for 4530952
				If Not text.Equals(editor.text) Then editor.text = text
			End Set
			Get
				Dim newValue As Object = editor.text
    
				If oldValue IsNot Nothing AndAlso Not(TypeOf oldValue Is String) Then
					' The original value is not a string. Should return the value in it's
					' original type.
					If newValue.Equals(oldValue.ToString()) Then
						Return oldValue
					Else
						' Must take the value from the editor and get the value and cast it to the new type.
						Dim cls As Type = oldValue.GetType()
						Try
							Dim method As Method = sun.reflect.misc.MethodUtil.getMethod(cls, "valueOf", New Type(){GetType(String)})
							newValue = sun.reflect.misc.MethodUtil.invoke(method, oldValue, New Object() { editor.text})
						Catch ex As Exception
							' Fail silently and return the newValue (a String object)
						End Try
					End If
				End If
				Return newValue
			End Get
		End Property


		Public Overridable Sub selectAll()
			editor.selectAll()
			editor.requestFocus()
		End Sub

		' This used to do something but now it doesn't.  It couldn't be
		' removed because it would be an API change to do so.
		Public Overridable Sub focusGained(ByVal e As FocusEvent)
		End Sub

		' This used to do something but now it doesn't.  It couldn't be
		' removed because it would be an API change to do so.
		Public Overridable Sub focusLost(ByVal e As FocusEvent)
		End Sub

		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			editor.addActionListener(l)
		End Sub

		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			editor.removeActionListener(l)
		End Sub

		Friend Class BorderlessTextField
			Inherits javax.swing.JTextField

			Public Sub New(ByVal value As String, ByVal n As Integer)
				MyBase.New(value,n)
			End Sub

			' workaround for 4530952
			Public Overrides Property text As String
				Set(ByVal s As String)
					If text.Equals(s) Then Return
					MyBase.text = s
				End Set
			End Property

			Public Overridable Property border As javax.swing.border.Border
				Set(ByVal b As javax.swing.border.Border)
					If Not(TypeOf b Is UIResource) Then MyBase.border = b
				End Set
			End Property
		End Class

		''' <summary>
		''' A subclass of BasicComboBoxEditor that implements UIResource.
		''' BasicComboBoxEditor doesn't implement UIResource
		''' directly so that applications can safely override the
		''' cellRenderer property with BasicListCellRenderer subclasses.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		Public Class UIResource
			Inherits BasicComboBoxEditor
			Implements javax.swing.plaf.UIResource

		End Class
	End Class

End Namespace