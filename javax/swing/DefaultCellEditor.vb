Imports System
Imports javax.swing.table
Imports javax.swing.event
Imports javax.swing.tree

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

Namespace javax.swing


	''' <summary>
	''' The default editor for table and tree cells.
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
	''' @author Alan Chung
	''' @author Philip Milne
	''' </summary>

	Public Class DefaultCellEditor
		Inherits AbstractCellEditor
		Implements TableCellEditor, TreeCellEditor

	'
	'  Instance Variables
	'

		''' <summary>
		''' The Swing component being edited. </summary>
		Protected Friend editorComponent As JComponent
		''' <summary>
		''' The delegate class which handles all methods sent from the
		''' <code>CellEditor</code>.
		''' </summary>
		Protected Friend [delegate] As EditorDelegate
		''' <summary>
		''' An integer specifying the number of clicks needed to start editing.
		''' Even if <code>clickCountToStart</code> is defined as zero, it
		''' will not initiate until a click occurs.
		''' </summary>
		Protected Friend clickCountToStart As Integer = 1

	'
	'  Constructors
	'

		''' <summary>
		''' Constructs a <code>DefaultCellEditor</code> that uses a text field.
		''' </summary>
		''' <param name="textField">  a <code>JTextField</code> object </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal textField As JTextField)
			editorComponent = textField
			Me.clickCountToStart = 2
			[delegate] = New EditorDelegateAnonymousInnerClassHelper
			textField.addActionListener([delegate])
		End Sub

		Private Class EditorDelegateAnonymousInnerClassHelper
			Inherits EditorDelegate

			Public Overrides Property value As Object
				Set(ByVal value As Object)
					textField.text = If(value IsNot Nothing, value.ToString(), "")
				End Set
			End Property

			Public Property Overrides cellEditorValue As Object
				Get
					Return textField.text
				End Get
			End Property
		End Class

		''' <summary>
		''' Constructs a <code>DefaultCellEditor</code> object that uses a check box.
		''' </summary>
		''' <param name="checkBox">  a <code>JCheckBox</code> object </param>
		Public Sub New(ByVal checkBox As JCheckBox)
			editorComponent = checkBox
			[delegate] = New EditorDelegateAnonymousInnerClassHelper2
			checkBox.addActionListener([delegate])
			checkBox.requestFocusEnabled = False
		End Sub

		Private Class EditorDelegateAnonymousInnerClassHelper2
			Inherits EditorDelegate

			Public Overrides Property value As Object
				Set(ByVal value As Object)
					Dim selected As Boolean = False
					If TypeOf value Is Boolean? Then
						selected = CBool(value)
					ElseIf TypeOf value Is String Then
						selected = value.Equals("true")
					End If
					checkBox.selected = selected
				End Set
			End Property

			Public Property Overrides cellEditorValue As Object
				Get
					Return Convert.ToBoolean(checkBox.selected)
				End Get
			End Property
		End Class

		''' <summary>
		''' Constructs a <code>DefaultCellEditor</code> object that uses a
		''' combo box.
		''' </summary>
		''' <param name="comboBox">  a <code>JComboBox</code> object </param>
		Public Sub New(ByVal comboBox As JComboBox)
			editorComponent = comboBox
			comboBox.putClientProperty("JComboBox.isTableCellEditor", Boolean.TRUE)
			[delegate] = New EditorDelegateAnonymousInnerClassHelper3
			comboBox.addActionListener([delegate])
		End Sub

		Private Class EditorDelegateAnonymousInnerClassHelper3
			Inherits EditorDelegate

			Public Overrides Property value As Object
				Set(ByVal value As Object)
					comboBox.selectedItem = value
				End Set
			End Property

			Public Property Overrides cellEditorValue As Object
				Get
					Return comboBox.selectedItem
				End Get
			End Property

			Public Overrides Function shouldSelectCell(ByVal anEvent As java.util.EventObject) As Boolean
				If TypeOf anEvent Is MouseEvent Then
					Dim e As MouseEvent = CType(anEvent, MouseEvent)
					Return e.iD <> MouseEvent.MOUSE_DRAGGED
				End If
				Return True
			End Function
			Public Overrides Function stopCellEditing() As Boolean
				If comboBox.editable Then comboBox.actionPerformed(New ActionEvent(DefaultCellEditor.this, 0, ""))
				Return MyBase.stopCellEditing()
			End Function
		End Class

		''' <summary>
		''' Returns a reference to the editor component.
		''' </summary>
		''' <returns> the editor <code>Component</code> </returns>
		Public Overridable Property component As java.awt.Component
			Get
				Return editorComponent
			End Get
		End Property

	'
	'  Modifying
	'

		''' <summary>
		''' Specifies the number of clicks needed to start editing.
		''' </summary>
		''' <param name="count">  an int specifying the number of clicks needed to start editing </param>
		''' <seealso cref= #getClickCountToStart </seealso>
		Public Overridable Property clickCountToStart As Integer
			Set(ByVal count As Integer)
				clickCountToStart = count
			End Set
			Get
				Return clickCountToStart
			End Get
		End Property


	'
	'  Override the implementations of the superclass, forwarding all methods
	'  from the CellEditor interface to our delegate.
	'

		''' <summary>
		''' Forwards the message from the <code>CellEditor</code> to
		''' the <code>delegate</code>. </summary>
		''' <seealso cref= EditorDelegate#getCellEditorValue </seealso>
		Public Property Overrides cellEditorValue As Object Implements CellEditor.getCellEditorValue
			Get
				Return [delegate].cellEditorValue
			End Get
		End Property

		''' <summary>
		''' Forwards the message from the <code>CellEditor</code> to
		''' the <code>delegate</code>. </summary>
		''' <seealso cref= EditorDelegate#isCellEditable(EventObject) </seealso>
		Public Overrides Function isCellEditable(ByVal anEvent As java.util.EventObject) As Boolean Implements CellEditor.isCellEditable
			Return [delegate].isCellEditable(anEvent)
		End Function

		''' <summary>
		''' Forwards the message from the <code>CellEditor</code> to
		''' the <code>delegate</code>. </summary>
		''' <seealso cref= EditorDelegate#shouldSelectCell(EventObject) </seealso>
		Public Overrides Function shouldSelectCell(ByVal anEvent As java.util.EventObject) As Boolean Implements CellEditor.shouldSelectCell
			Return [delegate].shouldSelectCell(anEvent)
		End Function

		''' <summary>
		''' Forwards the message from the <code>CellEditor</code> to
		''' the <code>delegate</code>. </summary>
		''' <seealso cref= EditorDelegate#stopCellEditing </seealso>
		Public Overrides Function stopCellEditing() As Boolean Implements CellEditor.stopCellEditing
			Return [delegate].stopCellEditing()
		End Function

		''' <summary>
		''' Forwards the message from the <code>CellEditor</code> to
		''' the <code>delegate</code>. </summary>
		''' <seealso cref= EditorDelegate#cancelCellEditing </seealso>
		Public Overrides Sub cancelCellEditing() Implements CellEditor.cancelCellEditing
			[delegate].cancelCellEditing()
		End Sub

	'
	'  Implementing the TreeCellEditor Interface
	'

		''' <summary>
		''' Implements the <code>TreeCellEditor</code> interface. </summary>
		Public Overridable Function getTreeCellEditorComponent(ByVal tree As JTree, ByVal value As Object, ByVal isSelected As Boolean, ByVal expanded As Boolean, ByVal leaf As Boolean, ByVal row As Integer) As java.awt.Component
			Dim stringValue As String = tree.convertValueToText(value, isSelected, expanded, leaf, row, False)

			[delegate].value = stringValue
			Return editorComponent
		End Function

	'
	'  Implementing the CellEditor Interface
	'
		''' <summary>
		''' Implements the <code>TableCellEditor</code> interface. </summary>
		Public Overridable Function getTableCellEditorComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal row As Integer, ByVal column As Integer) As java.awt.Component Implements TableCellEditor.getTableCellEditorComponent
			[delegate].value = value
			If TypeOf editorComponent Is JCheckBox Then
				'in order to avoid a "flashing" effect when clicking a checkbox
				'in a table, it is important for the editor to have as a border
				'the same border that the renderer has, and have as the background
				'the same color as the renderer has. This is primarily only
				'needed for JCheckBox since this editor doesn't fill all the
				'visual space of the table cell, unlike a text field.
				Dim renderer As TableCellRenderer = table.getCellRenderer(row, column)
				Dim c As java.awt.Component = renderer.getTableCellRendererComponent(table, value, isSelected, True, row, column)
				If c IsNot Nothing Then
					editorComponent.opaque = True
					editorComponent.background = c.background
					If TypeOf c Is JComponent Then editorComponent.border = CType(c, JComponent).border
				Else
					editorComponent.opaque = False
				End If
			End If
			Return editorComponent
		End Function


	'
	'  Protected EditorDelegate class
	'

		''' <summary>
		''' The protected <code>EditorDelegate</code> class.
		''' </summary>
		<Serializable> _
		Protected Friend Class EditorDelegate
			Implements ActionListener, ItemListener

			Private ReadOnly outerInstance As DefaultCellEditor

			Public Sub New(ByVal outerInstance As DefaultCellEditor)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			'''  The value of this cell. </summary>
			Protected Friend value As Object

		   ''' <summary>
		   ''' Returns the value of this cell. </summary>
		   ''' <returns> the value of this cell </returns>
			Public Overridable Property cellEditorValue As Object
				Get
					Return value
				End Get
			End Property

		   ''' <summary>
		   ''' Sets the value of this cell. </summary>
		   ''' <param name="value"> the new value of this cell </param>
			Public Overridable Property value As Object
				Set(ByVal value As Object)
					Me.value = value
				End Set
			End Property

		   ''' <summary>
		   ''' Returns true if <code>anEvent</code> is <b>not</b> a
		   ''' <code>MouseEvent</code>.  Otherwise, it returns true
		   ''' if the necessary number of clicks have occurred, and
		   ''' returns false otherwise.
		   ''' </summary>
		   ''' <param name="anEvent">         the event </param>
		   ''' <returns>  true  if cell is ready for editing, false otherwise </returns>
		   ''' <seealso cref= #setClickCountToStart </seealso>
		   ''' <seealso cref= #shouldSelectCell </seealso>
			Public Overridable Function isCellEditable(ByVal anEvent As java.util.EventObject) As Boolean
				If TypeOf anEvent Is MouseEvent Then Return CType(anEvent, MouseEvent).clickCount >= outerInstance.clickCountToStart
				Return True
			End Function

		   ''' <summary>
		   ''' Returns true to indicate that the editing cell may
		   ''' be selected.
		   ''' </summary>
		   ''' <param name="anEvent">         the event </param>
		   ''' <returns>  true </returns>
		   ''' <seealso cref= #isCellEditable </seealso>
			Public Overridable Function shouldSelectCell(ByVal anEvent As java.util.EventObject) As Boolean
				Return True
			End Function

		   ''' <summary>
		   ''' Returns true to indicate that editing has begun.
		   ''' </summary>
		   ''' <param name="anEvent">          the event </param>
			Public Overridable Function startCellEditing(ByVal anEvent As java.util.EventObject) As Boolean
				Return True
			End Function

		   ''' <summary>
		   ''' Stops editing and
		   ''' returns true to indicate that editing has stopped.
		   ''' This method calls <code>fireEditingStopped</code>.
		   ''' </summary>
		   ''' <returns>  true </returns>
			Public Overridable Function stopCellEditing() As Boolean
				outerInstance.fireEditingStopped()
				Return True
			End Function

		   ''' <summary>
		   ''' Cancels editing.  This method calls <code>fireEditingCanceled</code>.
		   ''' </summary>
		   Public Overridable Sub cancelCellEditing()
			   outerInstance.fireEditingCanceled()
		   End Sub

		   ''' <summary>
		   ''' When an action is performed, editing is ended. </summary>
		   ''' <param name="e"> the action event </param>
		   ''' <seealso cref= #stopCellEditing </seealso>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.stopCellEditing()
			End Sub

		   ''' <summary>
		   ''' When an item's state changes, editing is ended. </summary>
		   ''' <param name="e"> the action event </param>
		   ''' <seealso cref= #stopCellEditing </seealso>
			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent)
				outerInstance.stopCellEditing()
			End Sub
		End Class

	End Class ' End of class JCellEditor

End Namespace