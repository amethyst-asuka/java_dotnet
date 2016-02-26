Imports System
Imports javax.swing
Imports javax.swing.event

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

Namespace javax.swing.table

	''' <summary>
	'''  The <code>TableModel</code> interface specifies the methods the
	'''  <code>JTable</code> will use to interrogate a tabular data model. <p>
	''' 
	'''  The <code>JTable</code> can be set up to display any data
	'''  model which implements the
	'''  <code>TableModel</code> interface with a couple of lines of code:
	'''  <pre>
	'''      TableModel myData = new MyTableModel();
	'''      JTable table = new JTable(myData);
	'''  </pre><p>
	''' 
	''' For further documentation, see <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/table.html#data">Creating a Table Model</a>
	''' in <em>The Java Tutorial</em>.
	''' 
	''' @author Philip Milne </summary>
	''' <seealso cref= JTable </seealso>

	Public Interface TableModel
		''' <summary>
		''' Returns the number of rows in the model. A
		''' <code>JTable</code> uses this method to determine how many rows it
		''' should display.  This method should be quick, as it
		''' is called frequently during rendering.
		''' </summary>
		''' <returns> the number of rows in the model </returns>
		''' <seealso cref= #getColumnCount </seealso>
		ReadOnly Property rowCount As Integer

		''' <summary>
		''' Returns the number of columns in the model. A
		''' <code>JTable</code> uses this method to determine how many columns it
		''' should create and display by default.
		''' </summary>
		''' <returns> the number of columns in the model </returns>
		''' <seealso cref= #getRowCount </seealso>
		ReadOnly Property columnCount As Integer

		''' <summary>
		''' Returns the name of the column at <code>columnIndex</code>.  This is used
		''' to initialize the table's column header name.  Note: this name does
		''' not need to be unique; two columns in a table can have the same name.
		''' </summary>
		''' <param name="columnIndex">     the index of the column </param>
		''' <returns>  the name of the column </returns>
		Function getColumnName(ByVal columnIndex As Integer) As String

		''' <summary>
		''' Returns the most specific superclass for all the cell values
		''' in the column.  This is used by the <code>JTable</code> to set up a
		''' default renderer and editor for the column.
		''' </summary>
		''' <param name="columnIndex">  the index of the column </param>
		''' <returns> the common ancestor class of the object values in the model. </returns>
		Function getColumnClass(ByVal columnIndex As Integer) As Type

		''' <summary>
		''' Returns true if the cell at <code>rowIndex</code> and
		''' <code>columnIndex</code>
		''' is editable.  Otherwise, <code>setValueAt</code> on the cell will not
		''' change the value of that cell.
		''' </summary>
		''' <param name="rowIndex">        the row whose value to be queried </param>
		''' <param name="columnIndex">     the column whose value to be queried </param>
		''' <returns>  true if the cell is editable </returns>
		''' <seealso cref= #setValueAt </seealso>
		Function isCellEditable(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Boolean

		''' <summary>
		''' Returns the value for the cell at <code>columnIndex</code> and
		''' <code>rowIndex</code>.
		''' </summary>
		''' <param name="rowIndex">        the row whose value is to be queried </param>
		''' <param name="columnIndex">     the column whose value is to be queried </param>
		''' <returns>  the value Object at the specified cell </returns>
		Function getValueAt(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Object

		''' <summary>
		''' Sets the value in the cell at <code>columnIndex</code> and
		''' <code>rowIndex</code> to <code>aValue</code>.
		''' </summary>
		''' <param name="aValue">           the new value </param>
		''' <param name="rowIndex">         the row whose value is to be changed </param>
		''' <param name="columnIndex">      the column whose value is to be changed </param>
		''' <seealso cref= #getValueAt </seealso>
		''' <seealso cref= #isCellEditable </seealso>
		Sub setValueAt(ByVal aValue As Object, ByVal rowIndex As Integer, ByVal columnIndex As Integer)

		''' <summary>
		''' Adds a listener to the list that is notified each time a change
		''' to the data model occurs.
		''' </summary>
		''' <param name="l">               the TableModelListener </param>
		Sub addTableModelListener(ByVal l As TableModelListener)

		''' <summary>
		''' Removes a listener from the list that is notified each time a
		''' change to the data model occurs.
		''' </summary>
		''' <param name="l">               the TableModelListener </param>
		Sub removeTableModelListener(ByVal l As TableModelListener)
	End Interface

End Namespace