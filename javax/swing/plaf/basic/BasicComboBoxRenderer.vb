Imports System
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border

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
	''' ComboBox renderer
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Arnaud Weber
	''' </summary>
	<Serializable> _
	Public Class BasicComboBoxRenderer
		Inherits JLabel
		Implements ListCellRenderer

	   ''' <summary>
	   ''' An empty <code>Border</code>. This field might not be used. To change the
	   ''' <code>Border</code> used by this renderer directly set it using
	   ''' the <code>setBorder</code> method.
	   ''' </summary>
		Protected Friend Shared noFocusBorder As Border = New EmptyBorder(1, 1, 1, 1)
		Private Shared ReadOnly SAFE_NO_FOCUS_BORDER As Border = New EmptyBorder(1, 1, 1, 1)

		Public Sub New()
			MyBase.New()
			opaque = True
			border = noFocusBorder
		End Sub

		Private Property Shared noFocusBorder As Border
			Get
				If System.securityManager IsNot Nothing Then
					Return SAFE_NO_FOCUS_BORDER
				Else
					Return noFocusBorder
				End If
			End Get
		End Property

		Public Property Overrides preferredSize As Dimension
			Get
				Dim ___size As Dimension
    
				If (Me.text Is Nothing) OrElse (Me.text.Equals("")) Then
					text = " "
					___size = MyBase.preferredSize
					text = ""
				Else
					___size = MyBase.preferredSize
				End If
    
				Return ___size
			End Get
		End Property

		Public Overridable Function getListCellRendererComponent(ByVal list As JList, ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As Component

			''' <summary>
			'''if (isSelected) {
			'''    setBackground(UIManager.getColor("ComboBox.selectionBackground"));
			'''    setForeground(UIManager.getColor("ComboBox.selectionForeground"));
			''' } else {
			'''    setBackground(UIManager.getColor("ComboBox.background"));
			'''    setForeground(UIManager.getColor("ComboBox.foreground"));
			''' }*
			''' </summary>

			If isSelected Then
				background = list.selectionBackground
				foreground = list.selectionForeground
			Else
				background = list.background
				foreground = list.foreground
			End If

			font = list.font

			If TypeOf value Is Icon Then
				icon = CType(value, Icon)
			Else
				text = If(value Is Nothing, "", value.ToString())
			End If
			Return Me
		End Function


		''' <summary>
		''' A subclass of BasicComboBoxRenderer that implements UIResource.
		''' BasicComboBoxRenderer doesn't implement UIResource
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
			Inherits BasicComboBoxRenderer
			Implements javax.swing.plaf.UIResource

		End Class
	End Class

End Namespace