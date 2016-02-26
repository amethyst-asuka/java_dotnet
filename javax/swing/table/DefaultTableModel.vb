Imports System
Imports System.Collections
Imports System.Collections.Generic

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
	''' This is an implementation of <code>TableModel</code> that
	''' uses a <code>Vector</code> of <code>Vectors</code> to store the
	''' cell value objects.
	''' <p>
	''' <strong>Warning:</strong> <code>DefaultTableModel</code> returns a
	''' column class of <code>Object</code>.  When
	''' <code>DefaultTableModel</code> is used with a
	''' <code>TableRowSorter</code> this will result in extensive use of
	''' <code>toString</code>, which for non-<code>String</code> data types
	''' is expensive.  If you use <code>DefaultTableModel</code> with a
	''' <code>TableRowSorter</code> you are strongly encouraged to override
	''' <code>getColumnClass</code> to return the appropriate type.
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
	''' @author Philip Milne
	''' </summary>
	''' <seealso cref= TableModel </seealso>
	''' <seealso cref= #getDataVector </seealso>
	<Serializable> _
	Public Class DefaultTableModel
		Inherits AbstractTableModel

	'
	' Instance Variables
	'

		''' <summary>
		''' The <code>Vector</code> of <code>Vectors</code> of
		''' <code>Object</code> values.
		''' </summary>
		Protected Friend dataVector As ArrayList

		''' <summary>
		''' The <code>Vector</code> of column identifiers. </summary>
		Protected Friend columnIdentifiers As ArrayList

	'
	' Constructors
	'

		''' <summary>
		'''  Constructs a default <code>DefaultTableModel</code>
		'''  which is a table of zero columns and zero rows.
		''' </summary>
		Public Sub New()
			Me.New(0, 0)
		End Sub

		Private Shared Function newVector(ByVal size As Integer) As ArrayList
			Dim v As New ArrayList(size)
			v.Capacity = size
			Return v
		End Function

		''' <summary>
		'''  Constructs a <code>DefaultTableModel</code> with
		'''  <code>rowCount</code> and <code>columnCount</code> of
		'''  <code>null</code> object values.
		''' </summary>
		''' <param name="rowCount">           the number of rows the table holds </param>
		''' <param name="columnCount">        the number of columns the table holds
		''' </param>
		''' <seealso cref= #setValueAt </seealso>
		Public Sub New(ByVal rowCount As Integer, ByVal columnCount As Integer)
			Me.New(newVector(columnCount), rowCount)
		End Sub

		''' <summary>
		'''  Constructs a <code>DefaultTableModel</code> with as many columns
		'''  as there are elements in <code>columnNames</code>
		'''  and <code>rowCount</code> of <code>null</code>
		'''  object values.  Each column's name will be taken from
		'''  the <code>columnNames</code> vector.
		''' </summary>
		''' <param name="columnNames">       <code>vector</code> containing the names
		'''                          of the new columns; if this is
		'''                          <code>null</code> then the model has no columns </param>
		''' <param name="rowCount">           the number of rows the table holds </param>
		''' <seealso cref= #setDataVector </seealso>
		''' <seealso cref= #setValueAt </seealso>
		Public Sub New(ByVal columnNames As ArrayList, ByVal rowCount As Integer)
			dataVectortor(newVector(rowCount), columnNames)
		End Sub

		''' <summary>
		'''  Constructs a <code>DefaultTableModel</code> with as many
		'''  columns as there are elements in <code>columnNames</code>
		'''  and <code>rowCount</code> of <code>null</code>
		'''  object values.  Each column's name will be taken from
		'''  the <code>columnNames</code> array.
		''' </summary>
		''' <param name="columnNames">       <code>array</code> containing the names
		'''                          of the new columns; if this is
		'''                          <code>null</code> then the model has no columns </param>
		''' <param name="rowCount">           the number of rows the table holds </param>
		''' <seealso cref= #setDataVector </seealso>
		''' <seealso cref= #setValueAt </seealso>
		Public Sub New(ByVal columnNames As Object(), ByVal rowCount As Integer)
			Me.New(convertToVector(columnNames), rowCount)
		End Sub

		''' <summary>
		'''  Constructs a <code>DefaultTableModel</code> and initializes the table
		'''  by passing <code>data</code> and <code>columnNames</code>
		'''  to the <code>setDataVector</code> method.
		''' </summary>
		''' <param name="data">              the data of the table, a <code>Vector</code>
		'''                          of <code>Vector</code>s of <code>Object</code>
		'''                          values </param>
		''' <param name="columnNames">       <code>vector</code> containing the names
		'''                          of the new columns </param>
		''' <seealso cref= #getDataVector </seealso>
		''' <seealso cref= #setDataVector </seealso>
		Public Sub New(ByVal data As ArrayList, ByVal columnNames As ArrayList)
			dataVectortor(data, columnNames)
		End Sub

		''' <summary>
		'''  Constructs a <code>DefaultTableModel</code> and initializes the table
		'''  by passing <code>data</code> and <code>columnNames</code>
		'''  to the <code>setDataVector</code>
		'''  method. The first index in the <code>Object[][]</code> array is
		'''  the row index and the second is the column index.
		''' </summary>
		''' <param name="data">              the data of the table </param>
		''' <param name="columnNames">       the names of the columns </param>
		''' <seealso cref= #getDataVector </seealso>
		''' <seealso cref= #setDataVector </seealso>
		Public Sub New(ByVal data As Object()(), ByVal columnNames As Object())
			dataVectortor(data, columnNames)
		End Sub

		''' <summary>
		'''  Returns the <code>Vector</code> of <code>Vectors</code>
		'''  that contains the table's
		'''  data values.  The vectors contained in the outer vector are
		'''  each a single row of values.  In other words, to get to the cell
		'''  at row 1, column 5: <p>
		''' 
		'''  <code>((Vector)getDataVector().elementAt(1)).elementAt(5);</code>
		''' </summary>
		''' <returns>  the vector of vectors containing the tables data values
		''' </returns>
		''' <seealso cref= #newDataAvailable </seealso>
		''' <seealso cref= #newRowsAdded </seealso>
		''' <seealso cref= #setDataVector </seealso>
		Public Overridable Property dataVector As ArrayList
			Get
				Return dataVector
			End Get
		End Property

		Private Shared Function nonNullVector(ByVal v As ArrayList) As ArrayList
			Return If(v IsNot Nothing, v, New ArrayList)
		End Function

		''' <summary>
		'''  Replaces the current <code>dataVector</code> instance variable
		'''  with the new <code>Vector</code> of rows, <code>dataVector</code>.
		'''  Each row is represented in <code>dataVector</code> as a
		'''  <code>Vector</code> of <code>Object</code> values.
		'''  <code>columnIdentifiers</code> are the names of the new
		'''  columns.  The first name in <code>columnIdentifiers</code> is
		'''  mapped to column 0 in <code>dataVector</code>. Each row in
		'''  <code>dataVector</code> is adjusted to match the number of
		'''  columns in <code>columnIdentifiers</code>
		'''  either by truncating the <code>Vector</code> if it is too long,
		'''  or adding <code>null</code> values if it is too short.
		'''  <p>Note that passing in a <code>null</code> value for
		'''  <code>dataVector</code> results in unspecified behavior,
		'''  an possibly an exception.
		''' </summary>
		''' <param name="dataVector">         the new data vector </param>
		''' <param name="columnIdentifiers">     the names of the columns </param>
		''' <seealso cref= #getDataVector </seealso>
		Public Overridable Sub setDataVector(ByVal dataVector As ArrayList, ByVal columnIdentifiers As ArrayList)
			Me.dataVector = nonNullVector(dataVector)
			Me.columnIdentifiers = nonNullVector(columnIdentifiers)
			justifyRows(0, rowCount)
			fireTableStructureChanged()
		End Sub

		''' <summary>
		'''  Replaces the value in the <code>dataVector</code> instance
		'''  variable with the values in the array <code>dataVector</code>.
		'''  The first index in the <code>Object[][]</code>
		'''  array is the row index and the second is the column index.
		'''  <code>columnIdentifiers</code> are the names of the new columns.
		''' </summary>
		''' <param name="dataVector">                the new data vector </param>
		''' <param name="columnIdentifiers"> the names of the columns </param>
		''' <seealso cref= #setDataVector(Vector, Vector) </seealso>
		Public Overridable Sub setDataVector(ByVal dataVector As Object()(), ByVal columnIdentifiers As Object())
			dataVectortor(convertToVector(dataVector), convertToVector(columnIdentifiers))
		End Sub

		''' <summary>
		'''  Equivalent to <code>fireTableChanged</code>.
		''' </summary>
		''' <param name="event">  the change event
		'''  </param>
		Public Overridable Sub newDataAvailable(ByVal [event] As javax.swing.event.TableModelEvent)
			fireTableChanged([event])
		End Sub

	'
	' Manipulating rows
	'

		Private Sub justifyRows(ByVal [from] As Integer, ByVal [to] As Integer)
			' Sometimes the DefaultTableModel is subclassed
			' instead of the AbstractTableModel by mistake.
			' Set the number of rows for the case when getRowCount
			' is overridden.
			dataVector.Capacity = rowCount

			For i As Integer = from To [to] - 1
				If dataVector(i) Is Nothing Then dataVector(i) = New ArrayList
				CType(dataVector(i), ArrayList).Capacity = columnCount
			Next i
		End Sub

		''' <summary>
		'''  Ensures that the new rows have the correct number of columns.
		'''  This is accomplished by  using the <code>setSize</code> method in
		'''  <code>Vector</code> which truncates vectors
		'''  which are too long, and appends <code>null</code>s if they
		'''  are too short.
		'''  This method also sends out a <code>tableChanged</code>
		'''  notification message to all the listeners.
		''' </summary>
		''' <param name="e">         this <code>TableModelEvent</code> describes
		'''                           where the rows were added.
		'''                           If <code>null</code> it assumes
		'''                           all the rows were newly added </param>
		''' <seealso cref= #getDataVector </seealso>
		Public Overridable Sub newRowsAdded(ByVal e As javax.swing.event.TableModelEvent)
			justifyRows(e.firstRow, e.lastRow + 1)
			fireTableChanged(e)
		End Sub

		''' <summary>
		'''  Equivalent to <code>fireTableChanged</code>.
		''' </summary>
		'''  <param name="event"> the change event
		'''  </param>
		Public Overridable Sub rowsRemoved(ByVal [event] As javax.swing.event.TableModelEvent)
			fireTableChanged([event])
		End Sub

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use <code>setRowCount</code> instead.
		''' </summary>
	'    
	'     *  Sets the number of rows in the model.  If the new size is greater
	'     *  than the current size, new rows are added to the end of the model
	'     *  If the new size is less than the current size, all
	'     *  rows at index <code>rowCount</code> and greater are discarded.
	'     *
	'     * @param   rowCount   the new number of rows
	'     * @see #setRowCount
	'     
		Public Overridable Property numRows As Integer
			Set(ByVal rowCount As Integer)
				Dim old As Integer = rowCount
				If old = rowCount Then Return
				dataVector.Capacity = rowCount
				If rowCount <= old Then
					fireTableRowsDeleted(rowCount, old-1)
				Else
					justifyRows(old, rowCount)
					fireTableRowsInserted(old, rowCount-1)
				End If
			End Set
		End Property

		''' <summary>
		'''  Sets the number of rows in the model.  If the new size is greater
		'''  than the current size, new rows are added to the end of the model
		'''  If the new size is less than the current size, all
		'''  rows at index <code>rowCount</code> and greater are discarded.
		''' </summary>
		'''  <seealso cref= #setColumnCount
		''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setRowCount(ByVal rowCount As Integer) 'JavaToDotNetTempPropertySetrowCount
		Public Overridable Property rowCount As Integer
			Set(ByVal rowCount As Integer)
				numRows = rowCount
			End Set
			Get
		End Property

		''' <summary>
		'''  Adds a row to the end of the model.  The new row will contain
		'''  <code>null</code> values unless <code>rowData</code> is specified.
		'''  Notification of the row being added will be generated.
		''' </summary>
		''' <param name="rowData">          optional data of the row being added </param>
		Public Overridable Sub addRow(ByVal rowData As ArrayList)
			insertRow(rowCount, rowData)
		End Sub

		''' <summary>
		'''  Adds a row to the end of the model.  The new row will contain
		'''  <code>null</code> values unless <code>rowData</code> is specified.
		'''  Notification of the row being added will be generated.
		''' </summary>
		''' <param name="rowData">          optional data of the row being added </param>
		Public Overridable Sub addRow(ByVal rowData As Object())
			addRow(convertToVector(rowData))
		End Sub

		''' <summary>
		'''  Inserts a row at <code>row</code> in the model.  The new row
		'''  will contain <code>null</code> values unless <code>rowData</code>
		'''  is specified.  Notification of the row being added will be generated.
		''' </summary>
		''' <param name="row">             the row index of the row to be inserted </param>
		''' <param name="rowData">         optional data of the row being added </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the row was invalid </exception>
		Public Overridable Sub insertRow(ByVal row As Integer, ByVal rowData As ArrayList)
			dataVector.Insert(row, rowData)
			justifyRows(row, row+1)
			fireTableRowsInserted(row, row)
		End Sub

		''' <summary>
		'''  Inserts a row at <code>row</code> in the model.  The new row
		'''  will contain <code>null</code> values unless <code>rowData</code>
		'''  is specified.  Notification of the row being added will be generated.
		''' </summary>
		''' <param name="row">      the row index of the row to be inserted </param>
		''' <param name="rowData">          optional data of the row being added </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the row was invalid </exception>
		Public Overridable Sub insertRow(ByVal row As Integer, ByVal rowData As Object())
			insertRow(row, convertToVector(rowData))
		End Sub

		Private Shared Function gcd(ByVal i As Integer, ByVal j As Integer) As Integer
			Return If(j = 0, i, gcd(j, i Mod j))
		End Function

		Private Shared Sub rotate(ByVal v As ArrayList, ByVal a As Integer, ByVal b As Integer, ByVal shift As Integer)
			Dim size As Integer = b - a
			Dim r As Integer = size - shift
			Dim g As Integer = gcd(size, r)
			For i As Integer = 0 To g - 1
				Dim [to] As Integer = i
				Dim tmp As Object = v(a + [to])
				Dim [from] As Integer = ([to] + r) Mod size
				Do While [from] <> i
					v(a + [to]) = v(a + [from])
					[to] = [from]
					[from] = ([to] + r) Mod size
				Loop
				v(a + [to]) = tmp
			Next i
		End Sub

		''' <summary>
		'''  Moves one or more rows from the inclusive range <code>start</code> to
		'''  <code>end</code> to the <code>to</code> position in the model.
		'''  After the move, the row that was at index <code>start</code>
		'''  will be at index <code>to</code>.
		'''  This method will send a <code>tableChanged</code> notification
		'''   message to all the listeners.
		''' 
		'''  <pre>
		'''  Examples of moves:
		''' 
		'''  1. moveRow(1,3,5);
		'''          a|B|C|D|e|f|g|h|i|j|k   - before
		'''          a|e|f|g|h|B|C|D|i|j|k   - after
		''' 
		'''  2. moveRow(6,7,1);
		'''          a|b|c|d|e|f|G|H|i|j|k   - before
		'''          a|G|H|b|c|d|e|f|i|j|k   - after
		'''  </pre>
		''' </summary>
		''' <param name="start">       the starting row index to be moved </param>
		''' <param name="end">         the ending row index to be moved </param>
		''' <param name="to">          the destination of the rows to be moved </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if any of the elements
		''' would be moved out of the table's range
		'''  </exception>
		Public Overridable Sub moveRow(ByVal start As Integer, ByVal [end] As Integer, ByVal [to] As Integer)
			Dim shift As Integer = [to] - start
			Dim first, last As Integer
			If shift < 0 Then
				first = [to]
				last = [end]
			Else
				first = start
				last = [to] + [end] - start
			End If
			rotate(dataVector, first, last + 1, shift)

			fireTableRowsUpdated(first, last)
		End Sub

		''' <summary>
		'''  Removes the row at <code>row</code> from the model.  Notification
		'''  of the row being removed will be sent to all the listeners.
		''' </summary>
		''' <param name="row">      the row index of the row to be removed </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the row was invalid </exception>
		Public Overridable Sub removeRow(ByVal row As Integer)
			dataVector.RemoveAt(row)
			fireTableRowsDeleted(row, row)
		End Sub

	'
	' Manipulating columns
	'

		''' <summary>
		''' Replaces the column identifiers in the model.  If the number of
		''' <code>newIdentifier</code>s is greater than the current number
		''' of columns, new columns are added to the end of each row in the model.
		''' If the number of <code>newIdentifier</code>s is less than the current
		''' number of columns, all the extra columns at the end of a row are
		''' discarded.
		''' </summary>
		''' <param name="columnIdentifiers">  vector of column identifiers.  If
		'''                          <code>null</code>, set the model
		'''                          to zero columns </param>
		''' <seealso cref= #setNumRows </seealso>
		Public Overridable Property columnIdentifiers As ArrayList
			Set(ByVal columnIdentifiers As ArrayList)
				dataVectortor(dataVector, columnIdentifiers)
			End Set
		End Property

		''' <summary>
		''' Replaces the column identifiers in the model.  If the number of
		''' <code>newIdentifier</code>s is greater than the current number
		''' of columns, new columns are added to the end of each row in the model.
		''' If the number of <code>newIdentifier</code>s is less than the current
		''' number of columns, all the extra columns at the end of a row are
		''' discarded.
		''' </summary>
		''' <param name="newIdentifiers">  array of column identifiers.
		'''                          If <code>null</code>, set
		'''                          the model to zero columns </param>
		''' <seealso cref= #setNumRows </seealso>
		Public Overridable Property columnIdentifiers As Object()
			Set(ByVal newIdentifiers As Object())
				columnIdentifiers = convertToVector(newIdentifiers)
			End Set
		End Property

		''' <summary>
		'''  Sets the number of columns in the model.  If the new size is greater
		'''  than the current size, new columns are added to the end of the model
		'''  with <code>null</code> cell values.
		'''  If the new size is less than the current size, all columns at index
		'''  <code>columnCount</code> and greater are discarded.
		''' </summary>
		'''  <param name="columnCount">  the new number of columns in the model
		''' </param>
		'''  <seealso cref= #setColumnCount
		''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setColumnCount(ByVal columnCount As Integer) 'JavaToDotNetTempPropertySetcolumnCount
		Public Overridable Property columnCount As Integer
			Set(ByVal columnCount As Integer)
				columnIdentifiers.Capacity = columnCount
				justifyRows(0, rowCount)
				fireTableStructureChanged()
			End Set
			Get
		End Property

		''' <summary>
		'''  Adds a column to the model.  The new column will have the
		'''  identifier <code>columnName</code>, which may be null.  This method
		'''  will send a
		'''  <code>tableChanged</code> notification message to all the listeners.
		'''  This method is a cover for <code>addColumn(Object, Vector)</code> which
		'''  uses <code>null</code> as the data vector.
		''' </summary>
		''' <param name="columnName"> the identifier of the column being added </param>
		Public Overridable Sub addColumn(ByVal columnName As Object)
			addColumn(columnName, CType(Nothing, ArrayList))
		End Sub

		''' <summary>
		'''  Adds a column to the model.  The new column will have the
		'''  identifier <code>columnName</code>, which may be null.
		'''  <code>columnData</code> is the
		'''  optional vector of data for the column.  If it is <code>null</code>
		'''  the column is filled with <code>null</code> values.  Otherwise,
		'''  the new data will be added to model starting with the first
		'''  element going to row 0, etc.  This method will send a
		'''  <code>tableChanged</code> notification message to all the listeners.
		''' </summary>
		''' <param name="columnName"> the identifier of the column being added </param>
		''' <param name="columnData">       optional data of the column being added </param>
		Public Overridable Sub addColumn(ByVal columnName As Object, ByVal columnData As ArrayList)
			columnIdentifiers.Add(columnName)
			If columnData IsNot Nothing Then
				Dim columnSize As Integer = columnData.Count
				If columnSize > rowCount Then dataVector.Capacity = columnSize
				justifyRows(0, rowCount)
				Dim newColumn As Integer = columnCount - 1
				For i As Integer = 0 To columnSize - 1
					  Dim row As ArrayList = CType(dataVector(i), ArrayList)
					  row(newColumn) = columnData(i)
				Next i
			Else
				justifyRows(0, rowCount)
			End If

			fireTableStructureChanged()
		End Sub

		''' <summary>
		'''  Adds a column to the model.  The new column will have the
		'''  identifier <code>columnName</code>.  <code>columnData</code> is the
		'''  optional array of data for the column.  If it is <code>null</code>
		'''  the column is filled with <code>null</code> values.  Otherwise,
		'''  the new data will be added to model starting with the first
		'''  element going to row 0, etc.  This method will send a
		'''  <code>tableChanged</code> notification message to all the listeners.
		''' </summary>
		''' <seealso cref= #addColumn(Object, Vector) </seealso>
		Public Overridable Sub addColumn(ByVal columnName As Object, ByVal columnData As Object())
			addColumn(columnName, convertToVector(columnData))
		End Sub

	'
	' Implementing the TableModel interface
	'

			Return dataVector.Count
		End Function

			Return columnIdentifiers.Count
		End Function

		''' <summary>
		''' Returns the column name.
		''' </summary>
		''' <returns> a name for this column using the string value of the
		''' appropriate member in <code>columnIdentifiers</code>.
		''' If <code>columnIdentifiers</code> does not have an entry
		''' for this index, returns the default
		''' name provided by the superclass. </returns>
		Public Overrides Function getColumnName(ByVal column As Integer) As String
			Dim id As Object = Nothing
			' This test is to cover the case when
			' getColumnCount has been subclassed by mistake ...
			If column < columnIdentifiers.Count AndAlso (column >= 0) Then id = columnIdentifiers(column)
			Return If(id Is Nothing, MyBase.getColumnName(column), id.ToString())
		End Function

		''' <summary>
		''' Returns true regardless of parameter values.
		''' </summary>
		''' <param name="row">             the row whose value is to be queried </param>
		''' <param name="column">          the column whose value is to be queried </param>
		''' <returns>                  true </returns>
		''' <seealso cref= #setValueAt </seealso>
		Public Overrides Function isCellEditable(ByVal row As Integer, ByVal column As Integer) As Boolean
			Return True
		End Function

		''' <summary>
		''' Returns an attribute value for the cell at <code>row</code>
		''' and <code>column</code>.
		''' </summary>
		''' <param name="row">             the row whose value is to be queried </param>
		''' <param name="column">          the column whose value is to be queried </param>
		''' <returns>                  the value Object at the specified cell </returns>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if an invalid row or
		'''               column was given </exception>
		Public Overrides Function getValueAt(ByVal row As Integer, ByVal column As Integer) As Object
			Dim rowVector As ArrayList = CType(dataVector(row), ArrayList)
			Return rowVector(column)
		End Function

		''' <summary>
		''' Sets the object value for the cell at <code>column</code> and
		''' <code>row</code>.  <code>aValue</code> is the new value.  This method
		''' will generate a <code>tableChanged</code> notification.
		''' </summary>
		''' <param name="aValue">          the new value; this can be null </param>
		''' <param name="row">             the row whose value is to be changed </param>
		''' <param name="column">          the column whose value is to be changed </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if an invalid row or
		'''               column was given </exception>
		Public Overrides Sub setValueAt(ByVal aValue As Object, ByVal row As Integer, ByVal column As Integer)
			Dim rowVector As ArrayList = CType(dataVector(row), ArrayList)
			rowVector(column) = aValue
			fireTableCellUpdated(row, column)
		End Sub

	'
	' Protected Methods
	'

		''' <summary>
		''' Returns a vector that contains the same objects as the array. </summary>
		''' <param name="anArray">  the array to be converted </param>
		''' <returns>  the new vector; if <code>anArray</code> is <code>null</code>,
		'''                          returns <code>null</code> </returns>
		Protected Friend Shared Function convertToVector(ByVal anArray As Object()) As ArrayList
			If anArray Is Nothing Then Return Nothing
			Dim v As New List(Of Object)(anArray.Length)
			For Each o As Object In anArray
				v.Add(o)
			Next o
			Return v
		End Function

		''' <summary>
		''' Returns a vector of vectors that contains the same objects as the array. </summary>
		''' <param name="anArray">  the double array to be converted </param>
		''' <returns> the new vector of vectors; if <code>anArray</code> is
		'''                          <code>null</code>, returns <code>null</code> </returns>
		Protected Friend Shared Function convertToVector(ByVal anArray As Object()()) As ArrayList
			If anArray Is Nothing Then Return Nothing
			Dim v As New List(Of ArrayList)(anArray.Length)
			For Each o As Object() In anArray
				v.Add(convertToVector(o))
			Next o
			Return v
		End Function

	End Class ' End of class DefaultTableModel

End Namespace