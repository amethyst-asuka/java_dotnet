Imports javax.swing.event

'
' * Copyright (c) 1997, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface defines the methods any general editor should be able
	''' to implement. <p>
	''' 
	''' Having this interface enables complex components (the client of the
	''' editor) such as <code>JTree</code> and
	''' <code>JTable</code> to allow any generic editor to
	''' edit values in a table cell, or tree cell, etc.  Without this generic
	''' editor interface, <code>JTable</code> would have to know about specific editors,
	''' such as <code>JTextField</code>, <code>JCheckBox</code>, <code>JComboBox</code>,
	''' etc.  In addition, without this interface, clients of editors such as
	''' <code>JTable</code> would not be able
	''' to work with any editors developed in the future by the user
	''' or a 3rd party ISV. <p>
	''' 
	''' To use this interface, a developer creating a new editor can have the
	''' new component implement the interface.  Or the developer can
	''' choose a wrapper based approach and provide a companion object which
	''' implements the <code>CellEditor</code> interface (See
	''' <code>DefaultCellEditor</code> for example).  The wrapper approach
	''' is particularly useful if the user want to use a 3rd party ISV
	''' editor with <code>JTable</code>, but the ISV didn't implement the
	''' <code>CellEditor</code> interface.  The user can simply create an object
	''' that contains an instance of the 3rd party editor object and "translate"
	''' the <code>CellEditor</code> API into the 3rd party editor's API.
	''' </summary>
	''' <seealso cref= javax.swing.event.CellEditorListener
	''' 
	''' @author Alan Chung </seealso>
	Public Interface CellEditor

		''' <summary>
		''' Returns the value contained in the editor. </summary>
		''' <returns> the value contained in the editor </returns>
		ReadOnly Property cellEditorValue As Object

		''' <summary>
		''' Asks the editor if it can start editing using <code>anEvent</code>.
		''' <code>anEvent</code> is in the invoking component coordinate system.
		''' The editor can not assume the Component returned by
		''' <code>getCellEditorComponent</code> is installed.  This method
		''' is intended for the use of client to avoid the cost of setting up
		''' and installing the editor component if editing is not possible.
		''' If editing can be started this method returns true.
		''' </summary>
		''' <param name="anEvent">         the event the editor should use to consider
		'''                          whether to begin editing or not </param>
		''' <returns>  true if editing can be started </returns>
		''' <seealso cref= #shouldSelectCell </seealso>
		Function isCellEditable(ByVal anEvent As java.util.EventObject) As Boolean

		''' <summary>
		''' Returns true if the editing cell should be selected, false otherwise.
		''' Typically, the return value is true, because is most cases the editing
		''' cell should be selected.  However, it is useful to return false to
		''' keep the selection from changing for some types of edits.
		''' eg. A table that contains a column of check boxes, the user might
		''' want to be able to change those checkboxes without altering the
		''' selection.  (See Netscape Communicator for just such an example)
		''' Of course, it is up to the client of the editor to use the return
		''' value, but it doesn't need to if it doesn't want to.
		''' </summary>
		''' <param name="anEvent">         the event the editor should use to start
		'''                          editing </param>
		''' <returns>  true if the editor would like the editing cell to be selected;
		'''    otherwise returns false </returns>
		''' <seealso cref= #isCellEditable </seealso>
		Function shouldSelectCell(ByVal anEvent As java.util.EventObject) As Boolean

		''' <summary>
		''' Tells the editor to stop editing and accept any partially edited
		''' value as the value of the editor.  The editor returns false if
		''' editing was not stopped; this is useful for editors that validate
		''' and can not accept invalid entries.
		''' </summary>
		''' <returns>  true if editing was stopped; false otherwise </returns>
		Function stopCellEditing() As Boolean

		''' <summary>
		''' Tells the editor to cancel editing and not accept any partially
		''' edited value.
		''' </summary>
		Sub cancelCellEditing()

		''' <summary>
		''' Adds a listener to the list that's notified when the editor
		''' stops, or cancels editing.
		''' </summary>
		''' <param name="l">               the CellEditorListener </param>
		Sub addCellEditorListener(ByVal l As CellEditorListener)

		''' <summary>
		''' Removes a listener from the list that's notified
		''' </summary>
		''' <param name="l">               the CellEditorListener </param>
		Sub removeCellEditorListener(ByVal l As CellEditorListener)
	End Interface

End Namespace