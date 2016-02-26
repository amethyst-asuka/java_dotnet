Imports javax.swing.table

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

Namespace javax.swing.event

	''' <summary>
	''' TableModelEvent is used to notify listeners that a table model
	''' has changed. The model event describes changes to a TableModel
	''' and all references to rows and columns are in the co-ordinate
	''' system of the model.
	''' Depending on the parameters used in the constructors, the TableModelevent
	''' can be used to specify the following types of changes:
	''' 
	''' <pre>
	''' TableModelEvent(source);              //  The data, ie. all rows changed
	''' TableModelEvent(source, HEADER_ROW);  //  Structure change, reallocate TableColumns
	''' TableModelEvent(source, 1);           //  Row 1 changed
	''' TableModelEvent(source, 3, 6);        //  Rows 3 to 6 inclusive changed
	''' TableModelEvent(source, 2, 2, 6);     //  Cell at (2, 6) changed
	''' TableModelEvent(source, 3, 6, ALL_COLUMNS, INSERT); // Rows (3, 6) were inserted
	''' TableModelEvent(source, 3, 6, ALL_COLUMNS, DELETE); // Rows (3, 6) were deleted
	''' </pre>
	''' 
	''' It is possible to use other combinations of the parameters, not all of them
	''' are meaningful. By subclassing, you can add other information, for example:
	''' whether the event WILL happen or DID happen. This makes the specification
	''' of rows in DELETE events more useful but has not been included in
	''' the swing package as the JTable only needs post-event notification.
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
	''' @author Philip Milne </summary>
	''' <seealso cref= TableModel </seealso>
	Public Class TableModelEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Identifies the addition of new rows or columns. </summary>
		Public Const INSERT As Integer = 1
		''' <summary>
		''' Identifies a change to existing data. </summary>
		Public Const UPDATE As Integer = 0
		''' <summary>
		''' Identifies the removal of rows or columns. </summary>
		Public Const DELETE As Integer = -1

		''' <summary>
		''' Identifies the header row. </summary>
		Public Const HEADER_ROW As Integer = -1

		''' <summary>
		''' Specifies all columns in a row or rows. </summary>
		Public Const ALL_COLUMNS As Integer = -1

	'
	'  Instance Variables
	'

		Protected Friend type As Integer
		Protected Friend firstRow As Integer
		Protected Friend lastRow As Integer
		Protected Friend column As Integer

	'
	' Constructors
	'

		''' <summary>
		'''  All row data in the table has changed, listeners should discard any state
		'''  that was based on the rows and requery the <code>TableModel</code>
		'''  to get the new row count and all the appropriate values.
		'''  The <code>JTable</code> will repaint the entire visible region on
		'''  receiving this event, querying the model for the cell values that are visible.
		'''  The structure of the table ie, the column names, types and order
		'''  have not changed.
		''' </summary>
		Public Sub New(ByVal source As TableModel)
			' Use Integer.MAX_VALUE instead of getRowCount() in case rows were deleted.
			Me.New(source, 0, Integer.MaxValue, ALL_COLUMNS, UPDATE)
		End Sub

		''' <summary>
		'''  This row of data has been updated.
		'''  To denote the arrival of a completely new table with a different structure
		'''  use <code>HEADER_ROW</code> as the value for the <code>row</code>.
		'''  When the <code>JTable</code> receives this event and its
		'''  <code>autoCreateColumnsFromModel</code>
		'''  flag is set it discards any TableColumns that it had and reallocates
		'''  default ones in the order they appear in the model. This is the
		'''  same as calling <code>setModel(TableModel)</code> on the <code>JTable</code>.
		''' </summary>
		Public Sub New(ByVal source As TableModel, ByVal row As Integer)
			Me.New(source, row, row, ALL_COLUMNS, UPDATE)
		End Sub

		''' <summary>
		'''  The data in rows [<I>firstRow</I>, <I>lastRow</I>] have been updated.
		''' </summary>
		Public Sub New(ByVal source As TableModel, ByVal firstRow As Integer, ByVal lastRow As Integer)
			Me.New(source, firstRow, lastRow, ALL_COLUMNS, UPDATE)
		End Sub

		''' <summary>
		'''  The cells in column <I>column</I> in the range
		'''  [<I>firstRow</I>, <I>lastRow</I>] have been updated.
		''' </summary>
		Public Sub New(ByVal source As TableModel, ByVal firstRow As Integer, ByVal lastRow As Integer, ByVal column As Integer)
			Me.New(source, firstRow, lastRow, column, UPDATE)
		End Sub

		''' <summary>
		'''  The cells from (firstRow, column) to (lastRow, column) have been changed.
		'''  The <I>column</I> refers to the column index of the cell in the model's
		'''  co-ordinate system. When <I>column</I> is ALL_COLUMNS, all cells in the
		'''  specified range of rows are considered changed.
		'''  <p>
		'''  The <I>type</I> should be one of: INSERT, UPDATE and DELETE.
		''' </summary>
		Public Sub New(ByVal source As TableModel, ByVal firstRow As Integer, ByVal lastRow As Integer, ByVal column As Integer, ByVal type As Integer)
			MyBase.New(source)
			Me.firstRow = firstRow
			Me.lastRow = lastRow
			Me.column = column
			Me.type = type
		End Sub

	'
	' Querying Methods
	'

	   ''' <summary>
	   ''' Returns the first row that changed.  HEADER_ROW means the meta data,
	   ''' ie. names, types and order of the columns.
	   ''' </summary>
		Public Overridable Property firstRow As Integer
			Get
				Return firstRow
			End Get
		End Property

		''' <summary>
		''' Returns the last row that changed. </summary>
		Public Overridable Property lastRow As Integer
			Get
				Return lastRow
			End Get
		End Property

		''' <summary>
		'''  Returns the column for the event.  If the return
		'''  value is ALL_COLUMNS; it means every column in the specified
		'''  rows changed.
		''' </summary>
		Public Overridable Property column As Integer
			Get
				Return column
			End Get
		End Property

		''' <summary>
		'''  Returns the type of event - one of: INSERT, UPDATE and DELETE.
		''' </summary>
		Public Overridable Property type As Integer
			Get
				Return type
			End Get
		End Property
	End Class

End Namespace