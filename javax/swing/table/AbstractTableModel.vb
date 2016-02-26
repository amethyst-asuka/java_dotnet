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
	'''  This abstract class provides default implementations for most of
	'''  the methods in the <code>TableModel</code> interface. It takes care of
	'''  the management of listeners and provides some conveniences for generating
	'''  <code>TableModelEvents</code> and dispatching them to the listeners.
	'''  To create a concrete <code>TableModel</code> as a subclass of
	'''  <code>AbstractTableModel</code> you need only provide implementations
	'''  for the following three methods:
	''' 
	'''  <pre>
	'''  public int getRowCount();
	'''  public int getColumnCount();
	'''  public Object getValueAt(int row, int column);
	'''  </pre>
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
	<Serializable> _
	Public MustInherit Class AbstractTableModel
		Implements TableModel

			Public MustOverride Function getValueAt(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Object Implements TableModel.getValueAt
			Public MustOverride Function getColumnName(ByVal columnIndex As Integer) As String Implements TableModel.getColumnName
			Public MustOverride ReadOnly Property columnCount As Integer Implements TableModel.getColumnCount
			Public MustOverride ReadOnly Property rowCount As Integer Implements TableModel.getRowCount
	'
	' Instance Variables
	'

		''' <summary>
		''' List of listeners </summary>
		Protected Friend listenerList As New EventListenerList

	'
	' Default Implementation of the Interface
	'

		''' <summary>
		'''  Returns a default name for the column using spreadsheet conventions:
		'''  A, B, C, ... Z, AA, AB, etc.  If <code>column</code> cannot be found,
		'''  returns an empty string.
		''' </summary>
		''' <param name="column">  the column being queried </param>
		''' <returns> a string containing the default name of <code>column</code> </returns>
		Public Overridable Function getColumnName(ByVal column As Integer) As String Implements TableModel.getColumnName
			Dim result As String = ""
			Do While column >= 0
				result = ChrW(AscW(CChar(column Mod 26))+AscW("A"c)) + result
				column = column \ 26 - 1
			Loop
			Return result
		End Function

		''' <summary>
		''' Returns a column given its name.
		''' Implementation is naive so this should be overridden if
		''' this method is to be called often. This method is not
		''' in the <code>TableModel</code> interface and is not used by the
		''' <code>JTable</code>.
		''' </summary>
		''' <param name="columnName"> string containing name of column to be located </param>
		''' <returns> the column with <code>columnName</code>, or -1 if not found </returns>
		Public Overridable Function findColumn(ByVal columnName As String) As Integer
			For i As Integer = 0 To columnCount - 1
				If columnName.Equals(getColumnName(i)) Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		'''  Returns <code>Object.class</code> regardless of <code>columnIndex</code>.
		''' </summary>
		'''  <param name="columnIndex">  the column being queried </param>
		'''  <returns> the Object.class </returns>
		Public Overridable Function getColumnClass(ByVal columnIndex As Integer) As Type Implements TableModel.getColumnClass
			Return GetType(Object)
		End Function

		''' <summary>
		'''  Returns false.  This is the default implementation for all cells.
		''' </summary>
		'''  <param name="rowIndex">  the row being queried </param>
		'''  <param name="columnIndex"> the column being queried </param>
		'''  <returns> false </returns>
		Public Overridable Function isCellEditable(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Boolean Implements TableModel.isCellEditable
			Return False
		End Function

		''' <summary>
		'''  This empty implementation is provided so users don't have to implement
		'''  this method if their data model is not editable.
		''' </summary>
		'''  <param name="aValue">   value to assign to cell </param>
		'''  <param name="rowIndex">   row of cell </param>
		'''  <param name="columnIndex">  column of cell </param>
		Public Overridable Sub setValueAt(ByVal aValue As Object, ByVal rowIndex As Integer, ByVal columnIndex As Integer) Implements TableModel.setValueAt
		End Sub


	'
	'  Managing Listeners
	'

		''' <summary>
		''' Adds a listener to the list that's notified each time a change
		''' to the data model occurs.
		''' </summary>
		''' <param name="l">               the TableModelListener </param>
		Public Overridable Sub addTableModelListener(ByVal l As TableModelListener) Implements TableModel.addTableModelListener
			listenerList.add(GetType(TableModelListener), l)
		End Sub

		''' <summary>
		''' Removes a listener from the list that's notified each time a
		''' change to the data model occurs.
		''' </summary>
		''' <param name="l">               the TableModelListener </param>
		Public Overridable Sub removeTableModelListener(ByVal l As TableModelListener) Implements TableModel.removeTableModelListener
			listenerList.remove(GetType(TableModelListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the table model listeners
		''' registered on this model.
		''' </summary>
		''' <returns> all of this model's <code>TableModelListener</code>s
		'''         or an empty
		'''         array if no table model listeners are currently registered
		''' </returns>
		''' <seealso cref= #addTableModelListener </seealso>
		''' <seealso cref= #removeTableModelListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property tableModelListeners As TableModelListener()
			Get
				Return listenerList.getListeners(GetType(TableModelListener))
			End Get
		End Property

	'
	'  Fire methods
	'

		''' <summary>
		''' Notifies all listeners that all cell values in the table's
		''' rows may have changed. The number of rows may also have changed
		''' and the <code>JTable</code> should redraw the
		''' table from scratch. The structure of the table (as in the order of the
		''' columns) is assumed to be the same.
		''' </summary>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		''' <seealso cref= javax.swing.JTable#tableChanged(TableModelEvent) </seealso>
		Public Overridable Sub fireTableDataChanged()
			fireTableChanged(New TableModelEvent(Me))
		End Sub

		''' <summary>
		''' Notifies all listeners that the table's structure has changed.
		''' The number of columns in the table, and the names and types of
		''' the new columns may be different from the previous state.
		''' If the <code>JTable</code> receives this event and its
		''' <code>autoCreateColumnsFromModel</code>
		''' flag is set it discards any table columns that it had and reallocates
		''' default columns in the order they appear in the model. This is the
		''' same as calling <code>setModel(TableModel)</code> on the
		''' <code>JTable</code>.
		''' </summary>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTableStructureChanged()
			fireTableChanged(New TableModelEvent(Me, TableModelEvent.HEADER_ROW))
		End Sub

		''' <summary>
		''' Notifies all listeners that rows in the range
		''' <code>[firstRow, lastRow]</code>, inclusive, have been inserted.
		''' </summary>
		''' <param name="firstRow">  the first row </param>
		''' <param name="lastRow">   the last row
		''' </param>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList
		'''  </seealso>
		Public Overridable Sub fireTableRowsInserted(ByVal firstRow As Integer, ByVal lastRow As Integer)
			fireTableChanged(New TableModelEvent(Me, firstRow, lastRow, TableModelEvent.ALL_COLUMNS, TableModelEvent.INSERT))
		End Sub

		''' <summary>
		''' Notifies all listeners that rows in the range
		''' <code>[firstRow, lastRow]</code>, inclusive, have been updated.
		''' </summary>
		''' <param name="firstRow">  the first row </param>
		''' <param name="lastRow">   the last row
		''' </param>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTableRowsUpdated(ByVal firstRow As Integer, ByVal lastRow As Integer)
			fireTableChanged(New TableModelEvent(Me, firstRow, lastRow, TableModelEvent.ALL_COLUMNS, TableModelEvent.UPDATE))
		End Sub

		''' <summary>
		''' Notifies all listeners that rows in the range
		''' <code>[firstRow, lastRow]</code>, inclusive, have been deleted.
		''' </summary>
		''' <param name="firstRow">  the first row </param>
		''' <param name="lastRow">   the last row
		''' </param>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTableRowsDeleted(ByVal firstRow As Integer, ByVal lastRow As Integer)
			fireTableChanged(New TableModelEvent(Me, firstRow, lastRow, TableModelEvent.ALL_COLUMNS, TableModelEvent.DELETE))
		End Sub

		''' <summary>
		''' Notifies all listeners that the value of the cell at
		''' <code>[row, column]</code> has been updated.
		''' </summary>
		''' <param name="row">  row of cell which has been updated </param>
		''' <param name="column">  column of cell which has been updated </param>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTableCellUpdated(ByVal row As Integer, ByVal column As Integer)
			fireTableChanged(New TableModelEvent(Me, row, row, column))
		End Sub

		''' <summary>
		''' Forwards the given notification event to all
		''' <code>TableModelListeners</code> that registered
		''' themselves as listeners for this table model.
		''' </summary>
		''' <param name="e">  the event to be forwarded
		''' </param>
		''' <seealso cref= #addTableModelListener </seealso>
		''' <seealso cref= TableModelEvent </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Public Overridable Sub fireTableChanged(ByVal e As TableModelEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(TableModelListener) Then CType(___listeners(i+1), TableModelListener).tableChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>AbstractTableModel</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal,
		''' such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' model <code>m</code>
		''' for its table model listeners with the following code:
		''' 
		''' <pre>TableModelListener[] tmls = (TableModelListener[])(m.getListeners(TableModelListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this component,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getTableModelListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function
	End Class ' End of class AbstractTableModel

End Namespace