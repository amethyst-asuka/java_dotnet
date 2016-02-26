Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.Threading
Imports javax.accessibility
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.table
Imports javax.swing.border
Imports javax.print.attribute
Imports sun.swing.SwingUtilities2.Section

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
	''' The <code>JTable</code> is used to display and edit regular two-dimensional tables
	''' of cells.
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/table.html">How to Use Tables</a>
	''' in <em>The Java Tutorial</em>
	''' for task-oriented documentation and examples of using <code>JTable</code>.
	''' 
	''' <p>
	''' The <code>JTable</code> has many
	''' facilities that make it possible to customize its rendering and editing
	''' but provides defaults for these features so that simple tables can be
	''' set up easily.  For example, to set up a table with 10 rows and 10
	''' columns of numbers:
	''' 
	''' <pre>
	'''      TableModel dataModel = new AbstractTableModel() {
	'''          public int getColumnCount() { return 10; }
	'''          public int getRowCount() { return 10;}
	'''          public Object getValueAt(int row, int col) { return new Integer(row*col); }
	'''      };
	'''      JTable table = new JTable(dataModel);
	'''      JScrollPane scrollpane = new JScrollPane(table);
	''' </pre>
	''' <p>
	''' {@code JTable}s are typically placed inside of a {@code JScrollPane}.  By
	''' default, a {@code JTable} will adjust its width such that
	''' a horizontal scrollbar is unnecessary.  To allow for a horizontal scrollbar,
	''' invoke <seealso cref="#setAutoResizeMode"/> with {@code AUTO_RESIZE_OFF}.
	''' Note that if you wish to use a <code>JTable</code> in a standalone
	''' view (outside of a <code>JScrollPane</code>) and want the header
	''' displayed, you can get it using <seealso cref="#getTableHeader"/> and
	''' display it separately.
	''' <p>
	''' To enable sorting and filtering of rows, use a
	''' {@code RowSorter}.
	''' You can set up a row sorter in either of two ways:
	''' <ul>
	'''   <li>Directly set the {@code RowSorter}. For example:
	'''        {@code table.setRowSorter(new TableRowSorter(model))}.
	'''   <li>Set the {@code autoCreateRowSorter}
	'''       property to {@code true}, so that the {@code JTable}
	'''       creates a {@code RowSorter} for
	'''       you. For example: {@code setAutoCreateRowSorter(true)}.
	''' </ul>
	''' <p>
	''' When designing applications that use the <code>JTable</code> it is worth paying
	''' close attention to the data structures that will represent the table's data.
	''' The <code>DefaultTableModel</code> is a model implementation that
	''' uses a <code>Vector</code> of <code>Vector</code>s of <code>Object</code>s to
	''' store the cell values. As well as copying the data from an
	''' application into the <code>DefaultTableModel</code>,
	''' it is also possible to wrap the data in the methods of the
	''' <code>TableModel</code> interface so that the data can be passed to the
	''' <code>JTable</code> directly, as in the example above. This often results
	''' in more efficient applications because the model is free to choose the
	''' internal representation that best suits the data.
	''' A good rule of thumb for deciding whether to use the <code>AbstractTableModel</code>
	''' or the <code>DefaultTableModel</code> is to use the <code>AbstractTableModel</code>
	''' as the base class for creating subclasses and the <code>DefaultTableModel</code>
	''' when subclassing is not required.
	''' <p>
	''' The "TableExample" directory in the demo area of the source distribution
	''' gives a number of complete examples of <code>JTable</code> usage,
	''' covering how the <code>JTable</code> can be used to provide an
	''' editable view of data taken from a database and how to modify
	''' the columns in the display to use specialized renderers and editors.
	''' <p>
	''' The <code>JTable</code> uses integers exclusively to refer to both the rows and the columns
	''' of the model that it displays. The <code>JTable</code> simply takes a tabular range of cells
	''' and uses <code>getValueAt(int, int)</code> to retrieve the
	''' values from the model during painting.  It is important to remember that
	''' the column and row indexes returned by various <code>JTable</code> methods
	''' are in terms of the <code>JTable</code> (the view) and are not
	''' necessarily the same indexes used by the model.
	''' <p>
	''' By default, columns may be rearranged in the <code>JTable</code> so that the
	''' view's columns appear in a different order to the columns in the model.
	''' This does not affect the implementation of the model at all: when the
	''' columns are reordered, the <code>JTable</code> maintains the new order of the columns
	''' internally and converts its column indices before querying the model.
	''' <p>
	''' So, when writing a <code>TableModel</code>, it is not necessary to listen for column
	''' reordering events as the model will be queried in its own coordinate
	''' system regardless of what is happening in the view.
	''' In the examples area there is a demonstration of a sorting algorithm making
	''' use of exactly this technique to interpose yet another coordinate system
	''' where the order of the rows is changed, rather than the order of the columns.
	''' <p>
	''' Similarly when using the sorting and filtering functionality
	''' provided by <code>RowSorter</code> the underlying
	''' <code>TableModel</code> does not need to know how to do sorting,
	''' rather <code>RowSorter</code> will handle it.  Coordinate
	''' conversions will be necessary when using the row based methods of
	''' <code>JTable</code> with the underlying <code>TableModel</code>.
	''' All of <code>JTable</code>s row based methods are in terms of the
	''' <code>RowSorter</code>, which is not necessarily the same as that
	''' of the underlying <code>TableModel</code>.  For example, the
	''' selection is always in terms of <code>JTable</code> so that when
	''' using <code>RowSorter</code> you will need to convert using
	''' <code>convertRowIndexToView</code> or
	''' <code>convertRowIndexToModel</code>.  The following shows how to
	''' convert coordinates from <code>JTable</code> to that of the
	''' underlying model:
	''' <pre>
	'''   int[] selection = table.getSelectedRows();
	'''   for (int i = 0; i &lt; selection.length; i++) {
	'''     selection[i] = table.convertRowIndexToModel(selection[i]);
	'''   }
	'''   // selection is now in terms of the underlying TableModel
	''' </pre>
	''' <p>
	''' By default if sorting is enabled <code>JTable</code> will persist the
	''' selection and variable row heights in terms of the model on
	''' sorting.  For example if row 0, in terms of the underlying model,
	''' is currently selected, after the sort row 0, in terms of the
	''' underlying model will be selected.  Visually the selection may
	''' change, but in terms of the underlying model it will remain the
	''' same.  The one exception to that is if the model index is no longer
	''' visible or was removed.  For example, if row 0 in terms of model
	''' was filtered out the selection will be empty after the sort.
	''' <p>
	''' J2SE 5 adds methods to <code>JTable</code> to provide convenient access to some
	''' common printing needs. Simple new <seealso cref="#print()"/> methods allow for quick
	''' and easy addition of printing support to your application. In addition, a new
	''' <seealso cref="#getPrintable"/> method is available for more advanced printing needs.
	''' <p>
	''' As for all <code>JComponent</code> classes, you can use
	''' <seealso cref="InputMap"/> and <seealso cref="ActionMap"/> to associate an
	''' <seealso cref="Action"/> object with a <seealso cref="KeyStroke"/> and execute the
	''' action under specified conditions.
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
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
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A component which displays data in a two dimensional grid.
	''' 
	''' @author Philip Milne
	''' @author Shannon Hickey (printing support) </summary>
	''' <seealso cref= javax.swing.table.DefaultTableModel </seealso>
	''' <seealso cref= javax.swing.table.TableRowSorter </seealso>
	' The first versions of the JTable, contained in Swing-0.1 through
	' * Swing-0.4, were written by Alan Chung.
	' 
	Public Class JTable
		Inherits JComponent
		Implements TableModelListener, Scrollable, TableColumnModelListener, ListSelectionListener, CellEditorListener, Accessible, RowSorterListener

	'
	' Static Constants
	'

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TableUI"

		''' <summary>
		''' Do not adjust column widths automatically; use a horizontal scrollbar instead. </summary>
		Public Const AUTO_RESIZE_OFF As Integer = 0

		''' <summary>
		''' When a column is adjusted in the UI, adjust the next column the opposite way. </summary>
		Public Const AUTO_RESIZE_NEXT_COLUMN As Integer = 1

		''' <summary>
		''' During UI adjustment, change subsequent columns to preserve the total width;
		''' this is the default behavior. 
		''' </summary>
		Public Const AUTO_RESIZE_SUBSEQUENT_COLUMNS As Integer = 2

		''' <summary>
		''' During all resize operations, apply adjustments to the last column only. </summary>
		Public Const AUTO_RESIZE_LAST_COLUMN As Integer = 3

		''' <summary>
		''' During all resize operations, proportionately resize all columns. </summary>
		Public Const AUTO_RESIZE_ALL_COLUMNS As Integer = 4


		''' <summary>
		''' Printing modes, used in printing <code>JTable</code>s.
		''' </summary>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean) </seealso>
		''' <seealso cref= #getPrintable
		''' @since 1.5 </seealso>
		Public Enum PrintMode

			''' <summary>
			''' Printing mode that prints the table at its current size,
			''' spreading both columns and rows across multiple pages if necessary.
			''' </summary>
			NORMAL

			''' <summary>
			''' Printing mode that scales the output smaller, if necessary,
			''' to fit the table's entire width (and thereby all columns) on each page;
			''' Rows are spread across multiple pages as necessary.
			''' </summary>
			FIT_WIDTH
		End Enum


	'
	' Instance Variables
	'

		''' <summary>
		''' The <code>TableModel</code> of the table. </summary>
		Protected Friend dataModel As TableModel

		''' <summary>
		''' The <code>TableColumnModel</code> of the table. </summary>
		Protected Friend columnModel As TableColumnModel

		''' <summary>
		''' The <code>ListSelectionModel</code> of the table, used to keep track of row selections. </summary>
		Protected Friend selectionModel As ListSelectionModel

		''' <summary>
		''' The <code>TableHeader</code> working with the table. </summary>
		Protected Friend tableHeader As JTableHeader

		''' <summary>
		''' The height in pixels of each row in the table. </summary>
		Protected Friend rowHeight As Integer

		''' <summary>
		''' The height in pixels of the margin between the cells in each row. </summary>
		Protected Friend rowMargin As Integer

		''' <summary>
		''' The color of the grid. </summary>
		Protected Friend gridColor As Color

		''' <summary>
		''' The table draws horizontal lines between cells if <code>showHorizontalLines</code> is true. </summary>
		Protected Friend showHorizontalLines As Boolean

		''' <summary>
		''' The table draws vertical lines between cells if <code>showVerticalLines</code> is true. </summary>
		Protected Friend showVerticalLines As Boolean

		''' <summary>
		'''  Determines if the table automatically resizes the
		'''  width of the table's columns to take up the entire width of the
		'''  table, and how it does the resizing.
		''' </summary>
		Protected Friend autoResizeMode As Integer

		''' <summary>
		'''  The table will query the <code>TableModel</code> to build the default
		'''  set of columns if this is true.
		''' </summary>
		Protected Friend autoCreateColumnsFromModel As Boolean

		''' <summary>
		''' Used by the <code>Scrollable</code> interface to determine the initial visible area. </summary>
		Protected Friend preferredViewportSize As Dimension

		''' <summary>
		''' True if row selection is allowed in this table. </summary>
		Protected Friend rowSelectionAllowed As Boolean

		''' <summary>
		''' Obsolete as of Java 2 platform v1.3.  Please use the
		''' <code>rowSelectionAllowed</code> property and the
		''' <code>columnSelectionAllowed</code> property of the
		''' <code>columnModel</code> instead. Or use the
		''' method <code>getCellSelectionEnabled</code>.
		''' </summary>
	'    
	'     * If true, both a row selection and a column selection
	'     * can be non-empty at the same time, the selected cells are the
	'     * the cells whose row and column are both selected.
	'     
		Protected Friend cellSelectionEnabled As Boolean

		''' <summary>
		''' If editing, the <code>Component</code> that is handling the editing. </summary>
		<NonSerialized> _
		Protected Friend editorComp As Component

		''' <summary>
		''' The active cell editor object, that overwrites the screen real estate
		''' occupied by the current cell and allows the user to change its contents.
		''' {@code null} if the table isn't currently editing.
		''' </summary>
		<NonSerialized> _
		Protected Friend cellEditor As TableCellEditor

		''' <summary>
		''' Identifies the column of the cell being edited. </summary>
		<NonSerialized> _
		Protected Friend editingColumn As Integer

		''' <summary>
		''' Identifies the row of the cell being edited. </summary>
		<NonSerialized> _
		Protected Friend editingRow As Integer

		''' <summary>
		''' A table of objects that display the contents of a cell,
		''' indexed by class as declared in <code>getColumnClass</code>
		''' in the <code>TableModel</code> interface.
		''' </summary>
		<NonSerialized> _
		Protected Friend defaultRenderersByColumnClass As Hashtable

		''' <summary>
		''' A table of objects that display and edit the contents of a cell,
		''' indexed by class as declared in <code>getColumnClass</code>
		''' in the <code>TableModel</code> interface.
		''' </summary>
		<NonSerialized> _
		Protected Friend defaultEditorsByColumnClass As Hashtable

		''' <summary>
		''' The foreground color of selected cells. </summary>
		Protected Friend selectionForeground As Color

		''' <summary>
		''' The background color of selected cells. </summary>
		Protected Friend selectionBackground As Color

	'
	' Private state
	'

		' WARNING: If you directly access this field you should also change the
		' SortManager.modelRowSizes field as well.
		Private rowModel As SizeSequence
		Private dragEnabled As Boolean
		Private surrendersFocusOnKeystroke As Boolean
		Private editorRemover As PropertyChangeListener = Nothing
		''' <summary>
		''' The last value of getValueIsAdjusting from the column selection models
		''' columnSelectionChanged notification. Used to test if a repaint is
		''' needed.
		''' </summary>
		Private columnSelectionAdjusting As Boolean
		''' <summary>
		''' The last value of getValueIsAdjusting from the row selection models
		''' valueChanged notification. Used to test if a repaint is needed.
		''' </summary>
		Private rowSelectionAdjusting As Boolean

		''' <summary>
		''' To communicate errors between threads during printing.
		''' </summary>
		Private printError As Exception

		''' <summary>
		''' True when setRowHeight(int) has been invoked.
		''' </summary>
		Private isRowHeightSet As Boolean

		''' <summary>
		''' If true, on a sort the selection is reset.
		''' </summary>
		Private updateSelectionOnSort As Boolean

		''' <summary>
		''' Information used in sorting.
		''' </summary>
		<NonSerialized> _
		Private sortManager As SortManager

		''' <summary>
		''' If true, when sorterChanged is invoked it's value is ignored.
		''' </summary>
		Private ignoreSortChange As Boolean

		''' <summary>
		''' Whether or not sorterChanged has been invoked.
		''' </summary>
		Private ___sorterChanged As Boolean

		''' <summary>
		''' If true, any time the model changes a new RowSorter is set.
		''' </summary>
		Private autoCreateRowSorter As Boolean

		''' <summary>
		''' Whether or not the table always fills the viewport height. </summary>
		''' <seealso cref= #setFillsViewportHeight </seealso>
		''' <seealso cref= #getScrollableTracksViewportHeight </seealso>
		Private fillsViewportHeight As Boolean

		''' <summary>
		''' The drop mode for this component.
		''' </summary>
		Private dropMode As DropMode = DropMode.USE_SELECTION

		''' <summary>
		''' The drop location.
		''' </summary>
		<NonSerialized> _
		Private dropLocation As DropLocation

		''' <summary>
		''' A subclass of <code>TransferHandler.DropLocation</code> representing
		''' a drop location for a <code>JTable</code>.
		''' </summary>
		''' <seealso cref= #getDropLocation
		''' @since 1.6 </seealso>
		Public NotInheritable Class DropLocation
			Inherits TransferHandler.DropLocation

			Private ReadOnly row As Integer
			Private ReadOnly col As Integer
			Private ReadOnly ___isInsertRow As Boolean
			Private ReadOnly isInsertCol As Boolean

			Private Sub New(ByVal p As Point, ByVal row As Integer, ByVal col As Integer, ByVal isInsertRow As Boolean, ByVal isInsertCol As Boolean)

				MyBase.New(p)
				Me.row = row
				Me.col = col
				Me.___isInsertRow = isInsertRow
				Me.isInsertCol = isInsertCol
			End Sub

			''' <summary>
			''' Returns the row index where a dropped item should be placed in the
			''' table. Interpretation of the value depends on the return of
			''' <code>isInsertRow()</code>. If that method returns
			''' <code>true</code> this value indicates the index where a new
			''' row should be inserted. Otherwise, it represents the value
			''' of an existing row on which the data was dropped. This index is
			''' in terms of the view.
			''' <p>
			''' <code>-1</code> indicates that the drop occurred over empty space,
			''' and no row could be calculated.
			''' </summary>
			''' <returns> the drop row </returns>
			Public Property row As Integer
				Get
					Return row
				End Get
			End Property

			''' <summary>
			''' Returns the column index where a dropped item should be placed in the
			''' table. Interpretation of the value depends on the return of
			''' <code>isInsertColumn()</code>. If that method returns
			''' <code>true</code> this value indicates the index where a new
			''' column should be inserted. Otherwise, it represents the value
			''' of an existing column on which the data was dropped. This index is
			''' in terms of the view.
			''' <p>
			''' <code>-1</code> indicates that the drop occurred over empty space,
			''' and no column could be calculated.
			''' </summary>
			''' <returns> the drop row </returns>
			Public Property column As Integer
				Get
					Return col
				End Get
			End Property

			''' <summary>
			''' Returns whether or not this location represents an insert
			''' of a row.
			''' </summary>
			''' <returns> whether or not this is an insert row </returns>
			Public Property insertRow As Boolean
				Get
					Return ___isInsertRow
				End Get
			End Property

			''' <summary>
			''' Returns whether or not this location represents an insert
			''' of a column.
			''' </summary>
			''' <returns> whether or not this is an insert column </returns>
			Public Property insertColumn As Boolean
				Get
					Return isInsertCol
				End Get
			End Property

			''' <summary>
			''' Returns a string representation of this drop location.
			''' This method is intended to be used for debugging purposes,
			''' and the content and format of the returned string may vary
			''' between implementations.
			''' </summary>
			''' <returns> a string representation of this drop location </returns>
			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[dropPoint=" & dropPoint & "," & "row=" & row & "," & "column=" & col & "," & "insertRow=" & ___isInsertRow & "," & "insertColumn=" & isInsertCol & "]"
			End Function
		End Class

	'
	' Constructors
	'

		''' <summary>
		''' Constructs a default <code>JTable</code> that is initialized with a default
		''' data model, a default column model, and a default selection
		''' model.
		''' </summary>
		''' <seealso cref= #createDefaultDataModel </seealso>
		''' <seealso cref= #createDefaultColumnModel </seealso>
		''' <seealso cref= #createDefaultSelectionModel </seealso>
		Public Sub New()
			Me.New(Nothing, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> that is initialized with
		''' <code>dm</code> as the data model, a default column model,
		''' and a default selection model.
		''' </summary>
		''' <param name="dm">        the data model for the table </param>
		''' <seealso cref= #createDefaultColumnModel </seealso>
		''' <seealso cref= #createDefaultSelectionModel </seealso>
		Public Sub New(ByVal dm As TableModel)
			Me.New(dm, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> that is initialized with
		''' <code>dm</code> as the data model, <code>cm</code>
		''' as the column model, and a default selection model.
		''' </summary>
		''' <param name="dm">        the data model for the table </param>
		''' <param name="cm">        the column model for the table </param>
		''' <seealso cref= #createDefaultSelectionModel </seealso>
		Public Sub New(ByVal dm As TableModel, ByVal cm As TableColumnModel)
			Me.New(dm, cm, Nothing)
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> that is initialized with
		''' <code>dm</code> as the data model, <code>cm</code> as the
		''' column model, and <code>sm</code> as the selection model.
		''' If any of the parameters are <code>null</code> this method
		''' will initialize the table with the corresponding default model.
		''' The <code>autoCreateColumnsFromModel</code> flag is set to false
		''' if <code>cm</code> is non-null, otherwise it is set to true
		''' and the column model is populated with suitable
		''' <code>TableColumns</code> for the columns in <code>dm</code>.
		''' </summary>
		''' <param name="dm">        the data model for the table </param>
		''' <param name="cm">        the column model for the table </param>
		''' <param name="sm">        the row selection model for the table </param>
		''' <seealso cref= #createDefaultDataModel </seealso>
		''' <seealso cref= #createDefaultColumnModel </seealso>
		''' <seealso cref= #createDefaultSelectionModel </seealso>
		Public Sub New(ByVal dm As TableModel, ByVal cm As TableColumnModel, ByVal sm As ListSelectionModel)
			MyBase.New()
			layout = Nothing

			focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, JComponent.managingFocusForwardTraversalKeys)
			focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, JComponent.managingFocusBackwardTraversalKeys)
			If cm Is Nothing Then
				cm = createDefaultColumnModel()
				autoCreateColumnsFromModel = True
			End If
			columnModel = cm

			If sm Is Nothing Then sm = createDefaultSelectionModel()
			selectionModel = sm

		' Set the model last, that way if the autoCreatColumnsFromModel has
		' been set above, we will automatically populate an empty columnModel
		' with suitable columns for the new model.
			If dm Is Nothing Then dm = createDefaultDataModel()
			model = dm

			initializeLocalVars()
			updateUI()
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> with <code>numRows</code>
		''' and <code>numColumns</code> of empty cells using
		''' <code>DefaultTableModel</code>.  The columns will have
		''' names of the form "A", "B", "C", etc.
		''' </summary>
		''' <param name="numRows">           the number of rows the table holds </param>
		''' <param name="numColumns">        the number of columns the table holds </param>
		''' <seealso cref= javax.swing.table.DefaultTableModel </seealso>
		Public Sub New(ByVal numRows As Integer, ByVal numColumns As Integer)
			Me.New(New DefaultTableModel(numRows, numColumns))
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> to display the values in the
		''' <code>Vector</code> of <code>Vectors</code>, <code>rowData</code>,
		''' with column names, <code>columnNames</code>.  The
		''' <code>Vectors</code> contained in <code>rowData</code>
		''' should contain the values for that row. In other words,
		''' the value of the cell at row 1, column 5 can be obtained
		''' with the following code:
		''' 
		''' <pre>((Vector)rowData.elementAt(1)).elementAt(5);</pre>
		''' <p> </summary>
		''' <param name="rowData">           the data for the new table </param>
		''' <param name="columnNames">       names of each column </param>
		Public Sub New(ByVal rowData As ArrayList, ByVal columnNames As ArrayList)
			Me.New(New DefaultTableModel(rowData, columnNames))
		End Sub

		''' <summary>
		''' Constructs a <code>JTable</code> to display the values in the two dimensional array,
		''' <code>rowData</code>, with column names, <code>columnNames</code>.
		''' <code>rowData</code> is an array of rows, so the value of the cell at row 1,
		''' column 5 can be obtained with the following code:
		''' 
		''' <pre> rowData[1][5]; </pre>
		''' <p>
		''' All rows must be of the same length as <code>columnNames</code>.
		''' <p> </summary>
		''' <param name="rowData">           the data for the new table </param>
		''' <param name="columnNames">       names of each column </param>
		Public Sub New(ByVal rowData As Object()(), ByVal columnNames As Object())
			Me.New(New AbstractTableModelAnonymousInnerClassHelper
		End Sub

		Private Class AbstractTableModelAnonymousInnerClassHelper
			Inherits AbstractTableModel

			Public Overrides Function getColumnName(ByVal column As Integer) As String
				Return columnNames(column).ToString()
			End Function
			Public Property Overrides rowCount As Integer
				Get
					Return rowData.length
				End Get
			End Property
			Public Property Overrides columnCount As Integer
				Get
					Return columnNames.length
				End Get
			End Property
			Public Overrides Function getValueAt(ByVal row As Integer, ByVal col As Integer) As Object
				Return rowData(row)(col)
			End Function
			Public Overrides Function isCellEditable(ByVal row As Integer, ByVal column As Integer) As Boolean
				Return True
			End Function
			Public Overrides Sub setValueAt(ByVal value As Object, ByVal row As Integer, ByVal col As Integer)
				rowData(row)(col) = value
				fireTableCellUpdated(row, col)
			End Sub
		End Class

		''' <summary>
		''' Calls the <code>configureEnclosingScrollPane</code> method.
		''' </summary>
		''' <seealso cref= #configureEnclosingScrollPane </seealso>
		Public Overrides Sub addNotify()
			MyBase.addNotify()
			configureEnclosingScrollPane()
		End Sub

		''' <summary>
		''' If this <code>JTable</code> is the <code>viewportView</code> of an enclosing <code>JScrollPane</code>
		''' (the usual situation), configure this <code>ScrollPane</code> by, amongst other things,
		''' installing the table's <code>tableHeader</code> as the <code>columnHeaderView</code> of the scroll pane.
		''' When a <code>JTable</code> is added to a <code>JScrollPane</code> in the usual way,
		''' using <code>new JScrollPane(myTable)</code>, <code>addNotify</code> is
		''' called in the <code>JTable</code> (when the table is added to the viewport).
		''' <code>JTable</code>'s <code>addNotify</code> method in turn calls this method,
		''' which is protected so that this default installation procedure can
		''' be overridden by a subclass.
		''' </summary>
		''' <seealso cref= #addNotify </seealso>
		Protected Friend Overridable Sub configureEnclosingScrollPane()
			Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
			If TypeOf parent Is JViewport Then
				Dim port As JViewport = CType(parent, JViewport)
				Dim gp As Container = port.parent
				If TypeOf gp Is JScrollPane Then
					Dim scrollPane As JScrollPane = CType(gp, JScrollPane)
					' Make certain we are the viewPort's view and not, for
					' example, the rowHeaderView of the scrollPane -
					' an implementor of fixed columns might do this.
					Dim viewport As JViewport = scrollPane.viewport
					If viewport Is Nothing OrElse SwingUtilities.getUnwrappedView(viewport) IsNot Me Then Return
					scrollPane.columnHeaderView = tableHeader
					' configure the scrollpane for any LAF dependent settings
					configureEnclosingScrollPaneUI()
				End If
			End If
		End Sub

		''' <summary>
		''' This is a sub-part of configureEnclosingScrollPane() that configures
		''' anything on the scrollpane that may change when the look and feel
		''' changes. It needed to be split out from configureEnclosingScrollPane() so
		''' that it can be called from updateUI() when the LAF changes without
		''' causing the regression found in bug 6687962. This was because updateUI()
		''' is called from the constructor which then caused
		''' configureEnclosingScrollPane() to be called by the constructor which
		''' changes its contract for any subclass that overrides it. So by splitting
		''' it out in this way configureEnclosingScrollPaneUI() can be called both
		''' from configureEnclosingScrollPane() and updateUI() in a safe manor.
		''' </summary>
		Private Sub configureEnclosingScrollPaneUI()
			Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
			If TypeOf parent Is JViewport Then
				Dim port As JViewport = CType(parent, JViewport)
				Dim gp As Container = port.parent
				If TypeOf gp Is JScrollPane Then
					Dim scrollPane As JScrollPane = CType(gp, JScrollPane)
					' Make certain we are the viewPort's view and not, for
					' example, the rowHeaderView of the scrollPane -
					' an implementor of fixed columns might do this.
					Dim viewport As JViewport = scrollPane.viewport
					If viewport Is Nothing OrElse SwingUtilities.getUnwrappedView(viewport) IsNot Me Then Return
					'  scrollPane.getViewport().setBackingStoreEnabled(true);
					Dim ___border As Border = scrollPane.border
					If ___border Is Nothing OrElse TypeOf ___border Is UIResource Then
						Dim scrollPaneBorder As Border = UIManager.getBorder("Table.scrollPaneBorder")
						If scrollPaneBorder IsNot Nothing Then scrollPane.border = scrollPaneBorder
					End If
					' add JScrollBar corner component if available from LAF and not already set by the user
					Dim corner As Component = scrollPane.getCorner(JScrollPane.UPPER_TRAILING_CORNER)
					If corner Is Nothing OrElse TypeOf corner Is UIResource Then
						corner = Nothing
						Try
							corner = CType(UIManager.get("Table.scrollPaneCornerComponent"), Component)
						Catch e As Exception
							' just ignore and don't set corner
						End Try
						scrollPane.cornerner(JScrollPane.UPPER_TRAILING_CORNER, corner)
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Calls the <code>unconfigureEnclosingScrollPane</code> method.
		''' </summary>
		''' <seealso cref= #unconfigureEnclosingScrollPane </seealso>
		Public Overrides Sub removeNotify()
			KeyboardFocusManager.currentKeyboardFocusManager.removePropertyChangeListener("permanentFocusOwner", editorRemover)
			editorRemover = Nothing
			unconfigureEnclosingScrollPane()
			MyBase.removeNotify()
		End Sub

		''' <summary>
		''' Reverses the effect of <code>configureEnclosingScrollPane</code>
		''' by replacing the <code>columnHeaderView</code> of the enclosing
		''' scroll pane with <code>null</code>. <code>JTable</code>'s
		''' <code>removeNotify</code> method calls
		''' this method, which is protected so that this default uninstallation
		''' procedure can be overridden by a subclass.
		''' </summary>
		''' <seealso cref= #removeNotify </seealso>
		''' <seealso cref= #configureEnclosingScrollPane
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub unconfigureEnclosingScrollPane()
			Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
			If TypeOf parent Is JViewport Then
				Dim port As JViewport = CType(parent, JViewport)
				Dim gp As Container = port.parent
				If TypeOf gp Is JScrollPane Then
					Dim scrollPane As JScrollPane = CType(gp, JScrollPane)
					' Make certain we are the viewPort's view and not, for
					' example, the rowHeaderView of the scrollPane -
					' an implementor of fixed columns might do this.
					Dim viewport As JViewport = scrollPane.viewport
					If viewport Is Nothing OrElse SwingUtilities.getUnwrappedView(viewport) IsNot Me Then Return
					scrollPane.columnHeaderView = Nothing
					' remove ScrollPane corner if one was added by the LAF
					Dim corner As Component = scrollPane.getCorner(JScrollPane.UPPER_TRAILING_CORNER)
					If TypeOf corner Is UIResource Then scrollPane.cornerner(JScrollPane.UPPER_TRAILING_CORNER, Nothing)
				End If
			End If
		End Sub

		Friend Overrides Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "rowHeight" Then
				If Not isRowHeightSet Then
					rowHeight = CType(value, Number)
					isRowHeightSet = False
				End If
				Return
			End If
			MyBase.uIPropertyrty(propertyName, value)
		End Sub

	'
	' Static Methods
	'

		''' <summary>
		''' Equivalent to <code>new JScrollPane(aTable)</code>.
		''' </summary>
		''' @deprecated As of Swing version 1.0.2,
		''' replaced by <code>new JScrollPane(aTable)</code>. 
		<Obsolete("As of Swing version 1.0.2,")> _
		Public Shared Function createScrollPaneForTable(ByVal aTable As JTable) As JScrollPane
			Return New JScrollPane(aTable)
		End Function

	'
	' Table Attributes
	'

		''' <summary>
		''' Sets the <code>tableHeader</code> working with this <code>JTable</code> to <code>newHeader</code>.
		''' It is legal to have a <code>null</code> <code>tableHeader</code>.
		''' </summary>
		''' <param name="tableHeader">                       new tableHeader </param>
		''' <seealso cref=     #getTableHeader
		''' @beaninfo
		'''  bound: true
		'''  description: The JTableHeader instance which renders the column headers. </seealso>
		Public Overridable Property tableHeader As JTableHeader
			Set(ByVal tableHeader As JTableHeader)
				If Me.tableHeader IsNot tableHeader Then
					Dim old As JTableHeader = Me.tableHeader
					' Release the old header
					If old IsNot Nothing Then old.table = Nothing
					Me.tableHeader = tableHeader
					If tableHeader IsNot Nothing Then tableHeader.table = Me
					firePropertyChange("tableHeader", old, tableHeader)
				End If
			End Set
			Get
				Return tableHeader
			End Get
		End Property


		''' <summary>
		''' Sets the height, in pixels, of all cells to <code>rowHeight</code>,
		''' revalidates, and repaints.
		''' The height of the cells will be equal to the row height minus
		''' the row margin.
		''' </summary>
		''' <param name="rowHeight">                       new row height </param>
		''' <exception cref="IllegalArgumentException">      if <code>rowHeight</code> is
		'''                                          less than 1 </exception>
		''' <seealso cref=     #getRowHeight
		''' @beaninfo
		'''  bound: true
		'''  description: The height of the specified row. </seealso>
		Public Overridable Property rowHeight As Integer
			Set(ByVal rowHeight As Integer)
				If rowHeight <= 0 Then Throw New System.ArgumentException("New row height less than 1")
				Dim old As Integer = Me.rowHeight
				Me.rowHeight = rowHeight
				rowModel = Nothing
				If sortManager IsNot Nothing Then sortManager.modelRowSizes = Nothing
				isRowHeightSet = True
				resizeAndRepaint()
				firePropertyChange("rowHeight", old, rowHeight)
			End Set
			Get
				Return rowHeight
			End Get
		End Property


		Private Property rowModel As SizeSequence
			Get
				If rowModel Is Nothing Then rowModel = New SizeSequence(rowCount, rowHeight)
				Return rowModel
			End Get
		End Property

		''' <summary>
		''' Sets the height for <code>row</code> to <code>rowHeight</code>,
		''' revalidates, and repaints. The height of the cells in this row
		''' will be equal to the row height minus the row margin.
		''' </summary>
		''' <param name="row">                             the row whose height is being
		'''                                            changed </param>
		''' <param name="rowHeight">                       new row height, in pixels </param>
		''' <exception cref="IllegalArgumentException">      if <code>rowHeight</code> is
		'''                                          less than 1
		''' @beaninfo
		'''  bound: true
		'''  description: The height in pixels of the cells in <code>row</code>
		''' @since 1.3 </exception>
		Public Overridable Sub setRowHeight(ByVal row As Integer, ByVal rowHeight As Integer)
			If rowHeight <= 0 Then Throw New System.ArgumentException("New row height less than 1")
			rowModel.sizeize(row, rowHeight)
			If sortManager IsNot Nothing Then sortManager.viewRowHeightght(row, rowHeight)
			resizeAndRepaint()
		End Sub

		''' <summary>
		''' Returns the height, in pixels, of the cells in <code>row</code>. </summary>
		''' <param name="row">              the row whose height is to be returned </param>
		''' <returns> the height, in pixels, of the cells in the row
		''' @since 1.3 </returns>
		Public Overridable Function getRowHeight(ByVal row As Integer) As Integer
			Return If(rowModel Is Nothing, rowHeight, rowModel.getSize(row))
		End Function

		''' <summary>
		''' Sets the amount of empty space between cells in adjacent rows.
		''' </summary>
		''' <param name="rowMargin">  the number of pixels between cells in a row </param>
		''' <seealso cref=     #getRowMargin
		''' @beaninfo
		'''  bound: true
		'''  description: The amount of space between cells. </seealso>
		Public Overridable Property rowMargin As Integer
			Set(ByVal rowMargin As Integer)
				Dim old As Integer = Me.rowMargin
				Me.rowMargin = rowMargin
				resizeAndRepaint()
				firePropertyChange("rowMargin", old, rowMargin)
			End Set
			Get
				Return rowMargin
			End Get
		End Property


		''' <summary>
		''' Sets the <code>rowMargin</code> and the <code>columnMargin</code> --
		''' the height and width of the space between cells -- to
		''' <code>intercellSpacing</code>.
		''' </summary>
		''' <param name="intercellSpacing">        a <code>Dimension</code>
		'''                                  specifying the new width
		'''                                  and height between cells </param>
		''' <seealso cref=     #getIntercellSpacing
		''' @beaninfo
		'''  description: The spacing between the cells,
		'''               drawn in the background color of the JTable. </seealso>
		Public Overridable Property intercellSpacing As Dimension
			Set(ByVal intercellSpacing As Dimension)
				' Set the rowMargin here and columnMargin in the TableColumnModel
				rowMargin = intercellSpacing.height
				columnModel.columnMargin = intercellSpacing.width
    
				resizeAndRepaint()
			End Set
			Get
				Return New Dimension(columnModel.columnMargin, rowMargin)
			End Get
		End Property


		''' <summary>
		''' Sets the color used to draw grid lines to <code>gridColor</code> and redisplays.
		''' The default color is look and feel dependent.
		''' </summary>
		''' <param name="gridColor">                       the new color of the grid lines </param>
		''' <exception cref="IllegalArgumentException">      if <code>gridColor</code> is <code>null</code> </exception>
		''' <seealso cref=     #getGridColor
		''' @beaninfo
		'''  bound: true
		'''  description: The grid color. </seealso>
		Public Overridable Property gridColor As Color
			Set(ByVal gridColor As Color)
				If gridColor Is Nothing Then Throw New System.ArgumentException("New color is null")
				Dim old As Color = Me.gridColor
				Me.gridColor = gridColor
				firePropertyChange("gridColor", old, gridColor)
				' Redraw
				repaint()
			End Set
			Get
				Return gridColor
			End Get
		End Property


		''' <summary>
		'''  Sets whether the table draws grid lines around cells.
		'''  If <code>showGrid</code> is true it does; if it is false it doesn't.
		'''  There is no <code>getShowGrid</code> method as this state is held
		'''  in two variables -- <code>showHorizontalLines</code> and <code>showVerticalLines</code> --
		'''  each of which can be queried independently.
		''' </summary>
		''' <param name="showGrid">                 true if table view should draw grid lines
		''' </param>
		''' <seealso cref=     #setShowVerticalLines </seealso>
		''' <seealso cref=     #setShowHorizontalLines
		''' @beaninfo
		'''  description: The color used to draw the grid lines. </seealso>
		Public Overridable Property showGrid As Boolean
			Set(ByVal showGrid As Boolean)
				showHorizontalLines = showGrid
				showVerticalLines = showGrid
    
				' Redraw
				repaint()
			End Set
		End Property

		''' <summary>
		'''  Sets whether the table draws horizontal lines between cells.
		'''  If <code>showHorizontalLines</code> is true it does; if it is false it doesn't.
		''' </summary>
		''' <param name="showHorizontalLines">      true if table view should draw horizontal lines </param>
		''' <seealso cref=     #getShowHorizontalLines </seealso>
		''' <seealso cref=     #setShowGrid </seealso>
		''' <seealso cref=     #setShowVerticalLines
		''' @beaninfo
		'''  bound: true
		'''  description: Whether horizontal lines should be drawn in between the cells. </seealso>
		Public Overridable Property showHorizontalLines As Boolean
			Set(ByVal showHorizontalLines As Boolean)
				Dim old As Boolean = Me.showHorizontalLines
				Me.showHorizontalLines = showHorizontalLines
				firePropertyChange("showHorizontalLines", old, showHorizontalLines)
    
				' Redraw
				repaint()
			End Set
			Get
				Return showHorizontalLines
			End Get
		End Property

		''' <summary>
		'''  Sets whether the table draws vertical lines between cells.
		'''  If <code>showVerticalLines</code> is true it does; if it is false it doesn't.
		''' </summary>
		''' <param name="showVerticalLines">              true if table view should draw vertical lines </param>
		''' <seealso cref=     #getShowVerticalLines </seealso>
		''' <seealso cref=     #setShowGrid </seealso>
		''' <seealso cref=     #setShowHorizontalLines
		''' @beaninfo
		'''  bound: true
		'''  description: Whether vertical lines should be drawn in between the cells. </seealso>
		Public Overridable Property showVerticalLines As Boolean
			Set(ByVal showVerticalLines As Boolean)
				Dim old As Boolean = Me.showVerticalLines
				Me.showVerticalLines = showVerticalLines
				firePropertyChange("showVerticalLines", old, showVerticalLines)
				' Redraw
				repaint()
			End Set
			Get
				Return showVerticalLines
			End Get
		End Property



		''' <summary>
		''' Sets the table's auto resize mode when the table is resized.  For further
		''' information on how the different resize modes work, see
		''' <seealso cref="#doLayout"/>.
		''' </summary>
		''' <param name="mode"> One of 5 legal values:
		'''                   AUTO_RESIZE_OFF,
		'''                   AUTO_RESIZE_NEXT_COLUMN,
		'''                   AUTO_RESIZE_SUBSEQUENT_COLUMNS,
		'''                   AUTO_RESIZE_LAST_COLUMN,
		'''                   AUTO_RESIZE_ALL_COLUMNS
		''' </param>
		''' <seealso cref=     #getAutoResizeMode </seealso>
		''' <seealso cref=     #doLayout
		''' @beaninfo
		'''  bound: true
		'''  description: Whether the columns should adjust themselves automatically.
		'''        enum: AUTO_RESIZE_OFF                JTable.AUTO_RESIZE_OFF
		'''              AUTO_RESIZE_NEXT_COLUMN        JTable.AUTO_RESIZE_NEXT_COLUMN
		'''              AUTO_RESIZE_SUBSEQUENT_COLUMNS JTable.AUTO_RESIZE_SUBSEQUENT_COLUMNS
		'''              AUTO_RESIZE_LAST_COLUMN        JTable.AUTO_RESIZE_LAST_COLUMN
		'''              AUTO_RESIZE_ALL_COLUMNS        JTable.AUTO_RESIZE_ALL_COLUMNS </seealso>
		Public Overridable Property autoResizeMode As Integer
			Set(ByVal mode As Integer)
				If (mode = AUTO_RESIZE_OFF) OrElse (mode = AUTO_RESIZE_NEXT_COLUMN) OrElse (mode = AUTO_RESIZE_SUBSEQUENT_COLUMNS) OrElse (mode = AUTO_RESIZE_LAST_COLUMN) OrElse (mode = AUTO_RESIZE_ALL_COLUMNS) Then
					Dim old As Integer = autoResizeMode
					autoResizeMode = mode
					resizeAndRepaint()
					If tableHeader IsNot Nothing Then tableHeader.resizeAndRepaint()
					firePropertyChange("autoResizeMode", old, autoResizeMode)
				End If
			End Set
			Get
				Return autoResizeMode
			End Get
		End Property


		''' <summary>
		''' Sets this table's <code>autoCreateColumnsFromModel</code> flag.
		''' This method calls <code>createDefaultColumnsFromModel</code> if
		''' <code>autoCreateColumnsFromModel</code> changes from false to true.
		''' </summary>
		''' <param name="autoCreateColumnsFromModel">   true if <code>JTable</code> should automatically create columns </param>
		''' <seealso cref=     #getAutoCreateColumnsFromModel </seealso>
		''' <seealso cref=     #createDefaultColumnsFromModel
		''' @beaninfo
		'''  bound: true
		'''  description: Automatically populates the columnModel when a new TableModel is submitted. </seealso>
		Public Overridable Property autoCreateColumnsFromModel As Boolean
			Set(ByVal autoCreateColumnsFromModel As Boolean)
				If Me.autoCreateColumnsFromModel <> autoCreateColumnsFromModel Then
					Dim old As Boolean = Me.autoCreateColumnsFromModel
					Me.autoCreateColumnsFromModel = autoCreateColumnsFromModel
					If autoCreateColumnsFromModel Then createDefaultColumnsFromModel()
					firePropertyChange("autoCreateColumnsFromModel", old, autoCreateColumnsFromModel)
				End If
			End Set
			Get
				Return autoCreateColumnsFromModel
			End Get
		End Property


		''' <summary>
		''' Creates default columns for the table from
		''' the data model using the <code>getColumnCount</code> method
		''' defined in the <code>TableModel</code> interface.
		''' <p>
		''' Clears any existing columns before creating the
		''' new columns based on information from the model.
		''' </summary>
		''' <seealso cref=     #getAutoCreateColumnsFromModel </seealso>
		Public Overridable Sub createDefaultColumnsFromModel()
			Dim m As TableModel = model
			If m IsNot Nothing Then
				' Remove any current columns
				Dim cm As TableColumnModel = columnModel
				Do While cm.columnCount > 0
					cm.removeColumn(cm.getColumn(0))
				Loop

				' Create new columns from the data model info
				For i As Integer = 0 To m.columnCount - 1
					Dim newColumn As New TableColumn(i)
					addColumn(newColumn)
				Next i
			End If
		End Sub

		''' <summary>
		''' Sets a default cell renderer to be used if no renderer has been set in
		''' a <code>TableColumn</code>. If renderer is <code>null</code>,
		''' removes the default renderer for this column class.
		''' </summary>
		''' <param name="columnClass">     set the default cell renderer for this columnClass </param>
		''' <param name="renderer">        default cell renderer to be used for this
		'''                         columnClass </param>
		''' <seealso cref=     #getDefaultRenderer </seealso>
		''' <seealso cref=     #setDefaultEditor </seealso>
		Public Overridable Sub setDefaultRenderer(ByVal columnClass As Type, ByVal renderer As TableCellRenderer)
			If renderer IsNot Nothing Then
				defaultRenderersByColumnClass(columnClass) = renderer
			Else
				defaultRenderersByColumnClass.Remove(columnClass)
			End If
		End Sub

		''' <summary>
		''' Returns the cell renderer to be used when no renderer has been set in
		''' a <code>TableColumn</code>. During the rendering of cells the renderer is fetched from
		''' a <code>Hashtable</code> of entries according to the class of the cells in the column. If
		''' there is no entry for this <code>columnClass</code> the method returns
		''' the entry for the most specific superclass. The <code>JTable</code> installs entries
		''' for <code>Object</code>, <code>Number</code>, and <code>Boolean</code>, all of which can be modified
		''' or replaced.
		''' </summary>
		''' <param name="columnClass">   return the default cell renderer
		'''                        for this columnClass </param>
		''' <returns>  the renderer for this columnClass </returns>
		''' <seealso cref=     #setDefaultRenderer </seealso>
		''' <seealso cref=     #getColumnClass </seealso>
		Public Overridable Function getDefaultRenderer(ByVal columnClass As Type) As TableCellRenderer
			If columnClass Is Nothing Then
				Return Nothing
			Else
				Dim renderer As Object = defaultRenderersByColumnClass(columnClass)
				If renderer IsNot Nothing Then
					Return CType(renderer, TableCellRenderer)
				Else
					Dim c As Type = columnClass.BaseType
					If c Is Nothing AndAlso columnClass IsNot GetType(Object) Then c = GetType(Object)
					Return getDefaultRenderer(c)
				End If
			End If
		End Function

		''' <summary>
		''' Sets a default cell editor to be used if no editor has been set in
		''' a <code>TableColumn</code>. If no editing is required in a table, or a
		''' particular column in a table, uses the <code>isCellEditable</code>
		''' method in the <code>TableModel</code> interface to ensure that this
		''' <code>JTable</code> will not start an editor in these columns.
		''' If editor is <code>null</code>, removes the default editor for this
		''' column class.
		''' </summary>
		''' <param name="columnClass">  set the default cell editor for this columnClass </param>
		''' <param name="editor">   default cell editor to be used for this columnClass </param>
		''' <seealso cref=     TableModel#isCellEditable </seealso>
		''' <seealso cref=     #getDefaultEditor </seealso>
		''' <seealso cref=     #setDefaultRenderer </seealso>
		Public Overridable Sub setDefaultEditor(ByVal columnClass As Type, ByVal editor As TableCellEditor)
			If editor IsNot Nothing Then
				defaultEditorsByColumnClass(columnClass) = editor
			Else
				defaultEditorsByColumnClass.Remove(columnClass)
			End If
		End Sub

		''' <summary>
		''' Returns the editor to be used when no editor has been set in
		''' a <code>TableColumn</code>. During the editing of cells the editor is fetched from
		''' a <code>Hashtable</code> of entries according to the class of the cells in the column. If
		''' there is no entry for this <code>columnClass</code> the method returns
		''' the entry for the most specific superclass. The <code>JTable</code> installs entries
		''' for <code>Object</code>, <code>Number</code>, and <code>Boolean</code>, all of which can be modified
		''' or replaced.
		''' </summary>
		''' <param name="columnClass">  return the default cell editor for this columnClass </param>
		''' <returns> the default cell editor to be used for this columnClass </returns>
		''' <seealso cref=     #setDefaultEditor </seealso>
		''' <seealso cref=     #getColumnClass </seealso>
		Public Overridable Function getDefaultEditor(ByVal columnClass As Type) As TableCellEditor
			If columnClass Is Nothing Then
				Return Nothing
			Else
				Dim editor As Object = defaultEditorsByColumnClass(columnClass)
				If editor IsNot Nothing Then
					Return CType(editor, TableCellEditor)
				Else
					Return getDefaultEditor(columnClass.BaseType)
				End If
			End If
		End Function

		''' <summary>
		''' Turns on or off automatic drag handling. In order to enable automatic
		''' drag handling, this property should be set to {@code true}, and the
		''' table's {@code TransferHandler} needs to be {@code non-null}.
		''' The default value of the {@code dragEnabled} property is {@code false}.
		''' <p>
		''' The job of honoring this property, and recognizing a user drag gesture,
		''' lies with the look and feel implementation, and in particular, the table's
		''' {@code TableUI}. When automatic drag handling is enabled, most look and
		''' feels (including those that subclass {@code BasicLookAndFeel}) begin a
		''' drag and drop operation whenever the user presses the mouse button over
		''' an item (in single selection mode) or a selection (in other selection
		''' modes) and then moves the mouse a few pixels. Setting this property to
		''' {@code true} can therefore have a subtle effect on how selections behave.
		''' <p>
		''' If a look and feel is used that ignores this property, you can still
		''' begin a drag and drop operation by calling {@code exportAsDrag} on the
		''' table's {@code TransferHandler}.
		''' </summary>
		''' <param name="b"> whether or not to enable automatic drag handling </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.4
		''' 
		''' @beaninfo
		'''  description: determines whether automatic drag handling is enabled
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso GraphicsEnvironment.headless Then Throw New HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the drop mode for this component. For backward compatibility,
		''' the default for this property is <code>DropMode.USE_SELECTION</code>.
		''' Usage of one of the other modes is recommended, however, for an
		''' improved user experience. <code>DropMode.ON</code>, for instance,
		''' offers similar behavior of showing items as selected, but does so without
		''' affecting the actual selection in the table.
		''' <p>
		''' <code>JTable</code> supports the following drop modes:
		''' <ul>
		'''    <li><code>DropMode.USE_SELECTION</code></li>
		'''    <li><code>DropMode.ON</code></li>
		'''    <li><code>DropMode.INSERT</code></li>
		'''    <li><code>DropMode.INSERT_ROWS</code></li>
		'''    <li><code>DropMode.INSERT_COLS</code></li>
		'''    <li><code>DropMode.ON_OR_INSERT</code></li>
		'''    <li><code>DropMode.ON_OR_INSERT_ROWS</code></li>
		'''    <li><code>DropMode.ON_OR_INSERT_COLS</code></li>
		''' </ul>
		''' <p>
		''' The drop mode is only meaningful if this component has a
		''' <code>TransferHandler</code> that accepts drops.
		''' </summary>
		''' <param name="dropMode"> the drop mode to use </param>
		''' <exception cref="IllegalArgumentException"> if the drop mode is unsupported
		'''         or <code>null</code> </exception>
		''' <seealso cref= #getDropMode </seealso>
		''' <seealso cref= #getDropLocation </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' @since 1.6 </seealso>
		Public Property dropMode As DropMode
			Set(ByVal dropMode As DropMode)
				If dropMode IsNot Nothing Then
					Select Case dropMode
						Case DropMode.USE_SELECTION, ON, INSERT, INSERT_ROWS, INSERT_COLS, ON_OR_INSERT, ON_OR_INSERT_ROWS, ON_OR_INSERT_COLS
							Me.dropMode = dropMode
							Return
					End Select
				End If
    
				Throw New System.ArgumentException(dropMode & ": Unsupported drop mode for table")
			End Set
			Get
				Return dropMode
			End Get
		End Property


		''' <summary>
		''' Calculates a drop location in this component, representing where a
		''' drop at the given point should insert data.
		''' </summary>
		''' <param name="p"> the point to calculate a drop location for </param>
		''' <returns> the drop location, or <code>null</code> </returns>
		Friend Overrides Function dropLocationForPoint(ByVal p As Point) As DropLocation
			Dim ___location As DropLocation = Nothing

			Dim row As Integer = rowAtPoint(p)
			Dim col As Integer = columnAtPoint(p)
			Dim outside As Boolean = Boolean.TRUE Is getClientProperty("Table.isFileList") AndAlso sun.swing.SwingUtilities2.pointOutsidePrefSize(Me, row, col, p)

			Dim rect As Rectangle = getCellRect(row, col, True)
			Dim xSection, ySection As sun.swing.SwingUtilities2.Section
			Dim between As Boolean = False
			Dim ltr As Boolean = componentOrientation.leftToRight

			Select Case dropMode
				Case DropMode.USE_SELECTION, ON
					If row = -1 OrElse col = -1 OrElse outside Then
						___location = New DropLocation(p, -1, -1, False, False)
					Else
						___location = New DropLocation(p, row, col, False, False)
					End If
				Case DropMode.INSERT
					If row = -1 AndAlso col = -1 Then
						___location = New DropLocation(p, 0, 0, True, True)
						Exit Select
					End If

					xSection = sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, True)

					If row = -1 Then
						If xSection Is LEADING Then
							___location = New DropLocation(p, rowCount, col, True, True)
						ElseIf xSection Is TRAILING Then
							___location = New DropLocation(p, rowCount, col + 1, True, True)
						Else
							___location = New DropLocation(p, rowCount, col, True, False)
						End If
					ElseIf xSection Is LEADING OrElse xSection Is TRAILING Then
						ySection = sun.swing.SwingUtilities2.liesInVertical(rect, p, True)
						If ySection Is LEADING Then
							between = True
						ElseIf ySection Is TRAILING Then
							row += 1
							between = True
						End If

						___location = New DropLocation(p, row,If(xSection Is TRAILING, col + 1, col), between, True)
					Else
						If sun.swing.SwingUtilities2.liesInVertical(rect, p, False) = TRAILING Then row += 1

						___location = New DropLocation(p, row, col, True, False)
					End If

				Case DropMode.INSERT_ROWS
					If row = -1 AndAlso col = -1 Then
						___location = New DropLocation(p, -1, -1, False, False)
						Exit Select
					End If

					If row = -1 Then
						___location = New DropLocation(p, rowCount, col, True, False)
						Exit Select
					End If

					If sun.swing.SwingUtilities2.liesInVertical(rect, p, False) = TRAILING Then row += 1

					___location = New DropLocation(p, row, col, True, False)
				Case DropMode.ON_OR_INSERT_ROWS
					If row = -1 AndAlso col = -1 Then
						___location = New DropLocation(p, -1, -1, False, False)
						Exit Select
					End If

					If row = -1 Then
						___location = New DropLocation(p, rowCount, col, True, False)
						Exit Select
					End If

					ySection = sun.swing.SwingUtilities2.liesInVertical(rect, p, True)
					If ySection Is LEADING Then
						between = True
					ElseIf ySection Is TRAILING Then
						row += 1
						between = True
					End If

					___location = New DropLocation(p, row, col, between, False)
				Case DropMode.INSERT_COLS
					If row = -1 Then
						___location = New DropLocation(p, -1, -1, False, False)
						Exit Select
					End If

					If col = -1 Then
						___location = New DropLocation(p, columnCount, col, False, True)
						Exit Select
					End If

					If sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, False) = TRAILING Then col += 1

					___location = New DropLocation(p, row, col, False, True)
				Case DropMode.ON_OR_INSERT_COLS
					If row = -1 Then
						___location = New DropLocation(p, -1, -1, False, False)
						Exit Select
					End If

					If col = -1 Then
						___location = New DropLocation(p, row, columnCount, False, True)
						Exit Select
					End If

					xSection = sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, True)
					If xSection Is LEADING Then
						between = True
					ElseIf xSection Is TRAILING Then
						col += 1
						between = True
					End If

					___location = New DropLocation(p, row, col, False, between)
				Case DropMode.ON_OR_INSERT
					If row = -1 AndAlso col = -1 Then
						___location = New DropLocation(p, 0, 0, True, True)
						Exit Select
					End If

					xSection = sun.swing.SwingUtilities2.liesInHorizontal(rect, p, ltr, True)

					If row = -1 Then
						If xSection Is LEADING Then
							___location = New DropLocation(p, rowCount, col, True, True)
						ElseIf xSection Is TRAILING Then
							___location = New DropLocation(p, rowCount, col + 1, True, True)
						Else
							___location = New DropLocation(p, rowCount, col, True, False)
						End If

						Exit Select
					End If

					ySection = sun.swing.SwingUtilities2.liesInVertical(rect, p, True)
					If ySection Is LEADING Then
						between = True
					ElseIf ySection Is TRAILING Then
						row += 1
						between = True
					End If

					___location = New DropLocation(p, row,If(xSection Is TRAILING, col + 1, col), between, xSection IsNot MIDDLE)

				Case Else
					Debug.Assert(False, "Unexpected drop mode")
			End Select

			Return ___location
		End Function

		''' <summary>
		''' Called to set or clear the drop location during a DnD operation.
		''' In some cases, the component may need to use it's internal selection
		''' temporarily to indicate the drop location. To help facilitate this,
		''' this method returns and accepts as a parameter a state object.
		''' This state object can be used to store, and later restore, the selection
		''' state. Whatever this method returns will be passed back to it in
		''' future calls, as the state parameter. If it wants the DnD system to
		''' continue storing the same state, it must pass it back every time.
		''' Here's how this is used:
		''' <p>
		''' Let's say that on the first call to this method the component decides
		''' to save some state (because it is about to use the selection to show
		''' a drop index). It can return a state object to the caller encapsulating
		''' any saved selection state. On a second call, let's say the drop location
		''' is being changed to something else. The component doesn't need to
		''' restore anything yet, so it simply passes back the same state object
		''' to have the DnD system continue storing it. Finally, let's say this
		''' method is messaged with <code>null</code>. This means DnD
		''' is finished with this component for now, meaning it should restore
		''' state. At this point, it can use the state parameter to restore
		''' said state, and of course return <code>null</code> since there's
		''' no longer anything to store.
		''' </summary>
		''' <param name="location"> the drop location (as calculated by
		'''        <code>dropLocationForPoint</code>) or <code>null</code>
		'''        if there's no longer a valid drop location </param>
		''' <param name="state"> the state object saved earlier for this component,
		'''        or <code>null</code> </param>
		''' <param name="forDrop"> whether or not the method is being called because an
		'''        actual drop occurred </param>
		''' <returns> any saved state for this component, or <code>null</code> if none </returns>
		Friend Overrides Function setDropLocation(ByVal location As TransferHandler.DropLocation, ByVal state As Object, ByVal forDrop As Boolean) As Object

			Dim retVal As Object = Nothing
			Dim tableLocation As DropLocation = CType(location, DropLocation)

			If dropMode = DropMode.USE_SELECTION Then
				If tableLocation Is Nothing Then
					If (Not forDrop) AndAlso state IsNot Nothing Then
						clearSelection()

						Dim rows As Integer() = CType(state, Integer()())(0)
						Dim cols As Integer() = CType(state, Integer()())(1)
						Dim anchleads As Integer() = CType(state, Integer()())(2)

						For Each row As Integer In rows
							addRowSelectionInterval(row, row)
						Next row

						For Each col As Integer In cols
							addColumnSelectionInterval(col, col)
						Next col

						sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(selectionModel, anchleads(1), anchleads(0))

						sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(columnModel.selectionModel, anchleads(3), anchleads(2))
					End If
				Else
					If dropLocation Is Nothing Then
						retVal = New Integer(){ selectedRows, selectedColumns, {getAdjustedIndex(selectionModel.anchorSelectionIndex, True), getAdjustedIndex(selectionModel.leadSelectionIndex, True), getAdjustedIndex(columnModel.selectionModel.anchorSelectionIndex, False), getAdjustedIndex(columnModel.selectionModel.leadSelectionIndex, False)}}
					Else
						retVal = state
					End If

					If tableLocation.row = -1 Then
						clearSelectionAndLeadAnchor()
					Else
						rowSelectionIntervalval(tableLocation.row, tableLocation.row)
						columnSelectionIntervalval(tableLocation.column, tableLocation.column)
					End If
				End If
			End If

			Dim old As DropLocation = dropLocation
			dropLocation = tableLocation
			firePropertyChange("dropLocation", old, dropLocation)

			Return retVal
		End Function

		''' <summary>
		''' Returns the location that this component should visually indicate
		''' as the drop location during a DnD operation over the component,
		''' or {@code null} if no location is to currently be shown.
		''' <p>
		''' This method is not meant for querying the drop location
		''' from a {@code TransferHandler}, as the drop location is only
		''' set after the {@code TransferHandler}'s <code>canImport</code>
		''' has returned and has allowed for the location to be shown.
		''' <p>
		''' When this property changes, a property change event with
		''' name "dropLocation" is fired by the component.
		''' </summary>
		''' <returns> the drop location </returns>
		''' <seealso cref= #setDropMode </seealso>
		''' <seealso cref= TransferHandler#canImport(TransferHandler.TransferSupport)
		''' @since 1.6 </seealso>
		Public Property dropLocation As DropLocation
			Get
				Return dropLocation
			End Get
		End Property

		''' <summary>
		''' Specifies whether a {@code RowSorter} should be created for the
		''' table whenever its model changes.
		''' <p>
		''' When {@code setAutoCreateRowSorter(true)} is invoked, a {@code
		''' TableRowSorter} is immediately created and installed on the
		''' table.  While the {@code autoCreateRowSorter} property remains
		''' {@code true}, every time the model is changed, a new {@code
		''' TableRowSorter} is created and set as the table's row sorter.
		''' The default value for the {@code autoCreateRowSorter}
		''' property is {@code false}.
		''' </summary>
		''' <param name="autoCreateRowSorter"> whether or not a {@code RowSorter}
		'''        should be automatically created </param>
		''' <seealso cref= javax.swing.table.TableRowSorter
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''  description: Whether or not to turn on sorting by default.
		''' @since 1.6 </seealso>
		Public Overridable Property autoCreateRowSorter As Boolean
			Set(ByVal autoCreateRowSorter As Boolean)
				Dim oldValue As Boolean = Me.autoCreateRowSorter
				Me.autoCreateRowSorter = autoCreateRowSorter
				If autoCreateRowSorter Then rowSorter = New TableRowSorter(Of TableModel)(model)
				firePropertyChange("autoCreateRowSorter", oldValue, autoCreateRowSorter)
			End Set
			Get
				Return autoCreateRowSorter
			End Get
		End Property


		''' <summary>
		''' Specifies whether the selection should be updated after sorting.
		''' If true, on sorting the selection is reset such that
		''' the same rows, in terms of the model, remain selected.  The default
		''' is true.
		''' </summary>
		''' <param name="update"> whether or not to update the selection on sorting
		''' @beaninfo
		'''        bound: true
		'''       expert: true
		'''  description: Whether or not to update the selection on sorting
		''' @since 1.6 </param>
		Public Overridable Property updateSelectionOnSort As Boolean
			Set(ByVal update As Boolean)
				If updateSelectionOnSort <> update Then
					updateSelectionOnSort = update
					firePropertyChange("updateSelectionOnSort", (Not update), update)
				End If
			End Set
			Get
				Return updateSelectionOnSort
			End Get
		End Property


		''' <summary>
		''' Sets the <code>RowSorter</code>.  <code>RowSorter</code> is used
		''' to provide sorting and filtering to a <code>JTable</code>.
		''' <p>
		''' This method clears the selection and resets any variable row heights.
		''' <p>
		''' This method fires a <code>PropertyChangeEvent</code> when appropriate,
		''' with the property name <code>"rowSorter"</code>.  For
		''' backward-compatibility, this method fires an additional event with the
		''' property name <code>"sorter"</code>.
		''' <p>
		''' If the underlying model of the <code>RowSorter</code> differs from
		''' that of this <code>JTable</code> undefined behavior will result.
		''' </summary>
		''' <param name="sorter"> the <code>RowSorter</code>; <code>null</code> turns
		'''        sorting off </param>
		''' <seealso cref= javax.swing.table.TableRowSorter
		''' @beaninfo
		'''        bound: true
		'''  description: The table's RowSorter
		''' @since 1.6 </seealso>
		Public Overridable Property rowSorter(Of T1 As TableModel) As RowSorter(Of T1)
			Set(ByVal sorter As RowSorter(Of T1))
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim oldRowSorter As RowSorter(Of ? As TableModel) = Nothing
				If sortManager IsNot Nothing Then
					oldRowSorter = sortManager.sorter
					sortManager.Dispose()
					sortManager = Nothing
				End If
				rowModel = Nothing
				clearSelectionAndLeadAnchor()
				If sorter IsNot Nothing Then sortManager = New SortManager(Me, sorter)
				resizeAndRepaint()
				firePropertyChange("rowSorter", oldRowSorter, sorter)
				firePropertyChange("sorter", oldRowSorter, sorter)
			End Set
			Get
				Return If(sortManager IsNot Nothing, sortManager.sorter, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the object responsible for sorting.
		''' </summary>
		''' <returns> the object responsible for sorting
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

	'
	' Selection methods
	'
		''' <summary>
		''' Sets the table's selection mode to allow only single selections, a single
		''' contiguous interval, or multiple intervals.
		''' <P>
		''' <b>Note:</b>
		''' <code>JTable</code> provides all the methods for handling
		''' column and row selection.  When setting states,
		''' such as <code>setSelectionMode</code>, it not only
		''' updates the mode for the row selection model but also sets similar
		''' values in the selection model of the <code>columnModel</code>.
		''' If you want to have the row and column selection models operating
		''' in different modes, set them both directly.
		''' <p>
		''' Both the row and column selection models for <code>JTable</code>
		''' default to using a <code>DefaultListSelectionModel</code>
		''' so that <code>JTable</code> works the same way as the
		''' <code>JList</code>. See the <code>setSelectionMode</code> method
		''' in <code>JList</code> for details about the modes.
		''' </summary>
		''' <seealso cref= JList#setSelectionMode
		''' @beaninfo
		''' description: The selection mode used by the row and column selection models.
		'''        enum: SINGLE_SELECTION            ListSelectionModel.SINGLE_SELECTION
		'''              SINGLE_INTERVAL_SELECTION   ListSelectionModel.SINGLE_INTERVAL_SELECTION
		'''              MULTIPLE_INTERVAL_SELECTION ListSelectionModel.MULTIPLE_INTERVAL_SELECTION </seealso>
		Public Overridable Property selectionMode As Integer
			Set(ByVal selectionMode As Integer)
				clearSelection()
				selectionModel.selectionMode = selectionMode
				columnModel.selectionModel.selectionMode = selectionMode
			End Set
		End Property

		''' <summary>
		''' Sets whether the rows in this model can be selected.
		''' </summary>
		''' <param name="rowSelectionAllowed">   true if this model will allow row selection </param>
		''' <seealso cref= #getRowSelectionAllowed
		''' @beaninfo
		'''  bound: true
		'''    attribute: visualUpdate true
		'''  description: If true, an entire row is selected for each selected cell. </seealso>
		Public Overridable Property rowSelectionAllowed As Boolean
			Set(ByVal rowSelectionAllowed As Boolean)
				Dim old As Boolean = Me.rowSelectionAllowed
				Me.rowSelectionAllowed = rowSelectionAllowed
				If old <> rowSelectionAllowed Then repaint()
				firePropertyChange("rowSelectionAllowed", old, rowSelectionAllowed)
			End Set
			Get
				Return rowSelectionAllowed
			End Get
		End Property


		''' <summary>
		''' Sets whether the columns in this model can be selected.
		''' </summary>
		''' <param name="columnSelectionAllowed">   true if this model will allow column selection </param>
		''' <seealso cref= #getColumnSelectionAllowed
		''' @beaninfo
		'''  bound: true
		'''    attribute: visualUpdate true
		'''  description: If true, an entire column is selected for each selected cell. </seealso>
		Public Overridable Property columnSelectionAllowed As Boolean
			Set(ByVal columnSelectionAllowed As Boolean)
				Dim old As Boolean = columnModel.columnSelectionAllowed
				columnModel.columnSelectionAllowed = columnSelectionAllowed
				If old <> columnSelectionAllowed Then repaint()
				firePropertyChange("columnSelectionAllowed", old, columnSelectionAllowed)
			End Set
			Get
				Return columnModel.columnSelectionAllowed
			End Get
		End Property


		''' <summary>
		''' Sets whether this table allows both a column selection and a
		''' row selection to exist simultaneously. When set,
		''' the table treats the intersection of the row and column selection
		''' models as the selected cells. Override <code>isCellSelected</code> to
		''' change this default behavior. This method is equivalent to setting
		''' both the <code>rowSelectionAllowed</code> property and
		''' <code>columnSelectionAllowed</code> property of the
		''' <code>columnModel</code> to the supplied value.
		''' </summary>
		''' <param name="cellSelectionEnabled">     true if simultaneous row and column
		'''                                  selection is allowed </param>
		''' <seealso cref= #getCellSelectionEnabled </seealso>
		''' <seealso cref= #isCellSelected
		''' @beaninfo
		'''  bound: true
		'''    attribute: visualUpdate true
		'''  description: Select a rectangular region of cells rather than
		'''               rows or columns. </seealso>
		Public Overridable Property cellSelectionEnabled As Boolean
			Set(ByVal cellSelectionEnabled As Boolean)
				rowSelectionAllowed = cellSelectionEnabled
				columnSelectionAllowed = cellSelectionEnabled
				Dim old As Boolean = Me.cellSelectionEnabled
				Me.cellSelectionEnabled = cellSelectionEnabled
				firePropertyChange("cellSelectionEnabled", old, cellSelectionEnabled)
			End Set
			Get
				Return rowSelectionAllowed AndAlso columnSelectionAllowed
			End Get
		End Property


		''' <summary>
		'''  Selects all rows, columns, and cells in the table.
		''' </summary>
		Public Overridable Sub selectAll()
			' If I'm currently editing, then I should stop editing
			If editing Then removeEditor()
			If rowCount > 0 AndAlso columnCount > 0 Then
				Dim oldLead As Integer
				Dim oldAnchor As Integer
				Dim selModel As ListSelectionModel

				selModel = selectionModel
				selModel.valueIsAdjusting = True
				oldLead = getAdjustedIndex(selModel.leadSelectionIndex, True)
				oldAnchor = getAdjustedIndex(selModel.anchorSelectionIndex, True)

				rowSelectionIntervalval(0, rowCount-1)

				' this is done to restore the anchor and lead
				sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(selModel, oldLead, oldAnchor)

				selModel.valueIsAdjusting = False

				selModel = columnModel.selectionModel
				selModel.valueIsAdjusting = True
				oldLead = getAdjustedIndex(selModel.leadSelectionIndex, False)
				oldAnchor = getAdjustedIndex(selModel.anchorSelectionIndex, False)

				columnSelectionIntervalval(0, columnCount-1)

				' this is done to restore the anchor and lead
				sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(selModel, oldLead, oldAnchor)

				selModel.valueIsAdjusting = False
			End If
		End Sub

		''' <summary>
		''' Deselects all selected columns and rows.
		''' </summary>
		Public Overridable Sub clearSelection()
			selectionModel.clearSelection()
			columnModel.selectionModel.clearSelection()
		End Sub

		Private Sub clearSelectionAndLeadAnchor()
			selectionModel.valueIsAdjusting = True
			columnModel.selectionModel.valueIsAdjusting = True

			clearSelection()

			selectionModel.anchorSelectionIndex = -1
			selectionModel.leadSelectionIndex = -1
			columnModel.selectionModel.anchorSelectionIndex = -1
			columnModel.selectionModel.leadSelectionIndex = -1

			selectionModel.valueIsAdjusting = False
			columnModel.selectionModel.valueIsAdjusting = False
		End Sub

		Private Function getAdjustedIndex(ByVal index As Integer, ByVal row As Boolean) As Integer
			Dim compare As Integer = If(row, rowCount, columnCount)
			Return If(index < compare, index, -1)
		End Function

		Private Function boundRow(ByVal row As Integer) As Integer
			If row < 0 OrElse row >= rowCount Then Throw New System.ArgumentException("Row index out of range")
			Return row
		End Function

		Private Function boundColumn(ByVal col As Integer) As Integer
			If col< 0 OrElse col >= columnCount Then Throw New System.ArgumentException("Column index out of range")
			Return col
		End Function

		''' <summary>
		''' Selects the rows from <code>index0</code> to <code>index1</code>,
		''' inclusive.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or
		'''                                          <code>index1</code> lie outside
		'''                                          [0, <code>getRowCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub setRowSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			selectionModel.selectionIntervalval(boundRow(index0), boundRow(index1))
		End Sub

		''' <summary>
		''' Selects the columns from <code>index0</code> to <code>index1</code>,
		''' inclusive.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or
		'''                                          <code>index1</code> lie outside
		'''                                          [0, <code>getColumnCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub setColumnSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			columnModel.selectionModel.selectionIntervalval(boundColumn(index0), boundColumn(index1))
		End Sub

		''' <summary>
		''' Adds the rows from <code>index0</code> to <code>index1</code>, inclusive, to
		''' the current selection.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or <code>index1</code>
		'''                                          lie outside [0, <code>getRowCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub addRowSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			selectionModel.addSelectionInterval(boundRow(index0), boundRow(index1))
		End Sub

		''' <summary>
		''' Adds the columns from <code>index0</code> to <code>index1</code>,
		''' inclusive, to the current selection.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or
		'''                                          <code>index1</code> lie outside
		'''                                          [0, <code>getColumnCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub addColumnSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			columnModel.selectionModel.addSelectionInterval(boundColumn(index0), boundColumn(index1))
		End Sub

		''' <summary>
		''' Deselects the rows from <code>index0</code> to <code>index1</code>, inclusive.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or
		'''                                          <code>index1</code> lie outside
		'''                                          [0, <code>getRowCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub removeRowSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			selectionModel.removeSelectionInterval(boundRow(index0), boundRow(index1))
		End Sub

		''' <summary>
		''' Deselects the columns from <code>index0</code> to <code>index1</code>, inclusive.
		''' </summary>
		''' <exception cref="IllegalArgumentException">      if <code>index0</code> or
		'''                                          <code>index1</code> lie outside
		'''                                          [0, <code>getColumnCount()</code>-1] </exception>
		''' <param name="index0"> one end of the interval </param>
		''' <param name="index1"> the other end of the interval </param>
		Public Overridable Sub removeColumnSelectionInterval(ByVal index0 As Integer, ByVal index1 As Integer)
			columnModel.selectionModel.removeSelectionInterval(boundColumn(index0), boundColumn(index1))
		End Sub

		''' <summary>
		''' Returns the index of the first selected row, -1 if no row is selected. </summary>
		''' <returns> the index of the first selected row </returns>
		Public Overridable Property selectedRow As Integer
			Get
				Return selectionModel.minSelectionIndex
			End Get
		End Property

		''' <summary>
		''' Returns the index of the first selected column,
		''' -1 if no column is selected. </summary>
		''' <returns> the index of the first selected column </returns>
		Public Overridable Property selectedColumn As Integer
			Get
				Return columnModel.selectionModel.minSelectionIndex
			End Get
		End Property

		''' <summary>
		''' Returns the indices of all selected rows.
		''' </summary>
		''' <returns> an array of integers containing the indices of all selected rows,
		'''         or an empty array if no row is selected </returns>
		''' <seealso cref= #getSelectedRow </seealso>
		Public Overridable Property selectedRows As Integer()
			Get
				Dim iMin As Integer = selectionModel.minSelectionIndex
				Dim iMax As Integer = selectionModel.maxSelectionIndex
    
				If (iMin = -1) OrElse (iMax = -1) Then Return New Integer(){}
    
				Dim rvTmp As Integer() = New Integer(1+ (iMax - iMin) - 1){}
				Dim n As Integer = 0
				For i As Integer = iMin To iMax
					If selectionModel.isSelectedIndex(i) Then
						rvTmp(n) = i
						n += 1
					End If
				Next i
				Dim rv As Integer() = New Integer(n - 1){}
				Array.Copy(rvTmp, 0, rv, 0, n)
				Return rv
			End Get
		End Property

		''' <summary>
		''' Returns the indices of all selected columns.
		''' </summary>
		''' <returns> an array of integers containing the indices of all selected columns,
		'''         or an empty array if no column is selected </returns>
		''' <seealso cref= #getSelectedColumn </seealso>
		Public Overridable Property selectedColumns As Integer()
			Get
				Return columnModel.selectedColumns
			End Get
		End Property

		''' <summary>
		''' Returns the number of selected rows.
		''' </summary>
		''' <returns> the number of selected rows, 0 if no rows are selected </returns>
		Public Overridable Property selectedRowCount As Integer
			Get
				Dim iMin As Integer = selectionModel.minSelectionIndex
				Dim iMax As Integer = selectionModel.maxSelectionIndex
				Dim count As Integer = 0
    
				For i As Integer = iMin To iMax
					If selectionModel.isSelectedIndex(i) Then count += 1
				Next i
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns the number of selected columns.
		''' </summary>
		''' <returns> the number of selected columns, 0 if no columns are selected </returns>
		Public Overridable Property selectedColumnCount As Integer
			Get
				Return columnModel.selectedColumnCount
			End Get
		End Property

		''' <summary>
		''' Returns true if the specified index is in the valid range of rows,
		''' and the row at that index is selected.
		''' </summary>
		''' <returns> true if <code>row</code> is a valid index and the row at
		'''              that index is selected (where 0 is the first row) </returns>
		Public Overridable Function isRowSelected(ByVal row As Integer) As Boolean
			Return selectionModel.isSelectedIndex(row)
		End Function

		''' <summary>
		''' Returns true if the specified index is in the valid range of columns,
		''' and the column at that index is selected.
		''' </summary>
		''' <param name="column">   the column in the column model </param>
		''' <returns> true if <code>column</code> is a valid index and the column at
		'''              that index is selected (where 0 is the first column) </returns>
		Public Overridable Function isColumnSelected(ByVal column As Integer) As Boolean
			Return columnModel.selectionModel.isSelectedIndex(column)
		End Function

		''' <summary>
		''' Returns true if the specified indices are in the valid range of rows
		''' and columns and the cell at the specified position is selected. </summary>
		''' <param name="row">   the row being queried </param>
		''' <param name="column">  the column being queried
		''' </param>
		''' <returns> true if <code>row</code> and <code>column</code> are valid indices
		'''              and the cell at index <code>(row, column)</code> is selected,
		'''              where the first row and first column are at index 0 </returns>
		Public Overridable Function isCellSelected(ByVal row As Integer, ByVal column As Integer) As Boolean
			If (Not rowSelectionAllowed) AndAlso (Not columnSelectionAllowed) Then Return False
			Return ((Not rowSelectionAllowed) OrElse isRowSelected(row)) AndAlso ((Not columnSelectionAllowed) OrElse isColumnSelected(column))
		End Function

		Private Sub changeSelectionModel(ByVal sm As ListSelectionModel, ByVal index As Integer, ByVal toggle As Boolean, ByVal extend As Boolean, ByVal selected As Boolean, ByVal anchor As Integer, ByVal anchorSelected As Boolean)
			If extend Then
				If toggle Then
					If anchorSelected Then
						sm.addSelectionInterval(anchor, index)
					Else
						sm.removeSelectionInterval(anchor, index)
						' this is a Windows-only behavior that we want for file lists
						If Boolean.TRUE Is getClientProperty("Table.isFileList") Then
							sm.addSelectionInterval(index, index)
							sm.anchorSelectionIndex = anchor
						End If
					End If
				Else
					sm.selectionIntervalval(anchor, index)
				End If
			Else
				If toggle Then
					If selected Then
						sm.removeSelectionInterval(index, index)
					Else
						sm.addSelectionInterval(index, index)
					End If
				Else
					sm.selectionIntervalval(index, index)
				End If
			End If
		End Sub

		''' <summary>
		''' Updates the selection models of the table, depending on the state of the
		''' two flags: <code>toggle</code> and <code>extend</code>. Most changes
		''' to the selection that are the result of keyboard or mouse events received
		''' by the UI are channeled through this method so that the behavior may be
		''' overridden by a subclass. Some UIs may need more functionality than
		''' this method provides, such as when manipulating the lead for discontiguous
		''' selection, and may not call into this method for some selection changes.
		''' <p>
		''' This implementation uses the following conventions:
		''' <ul>
		''' <li> <code>toggle</code>: <em>false</em>, <code>extend</code>: <em>false</em>.
		'''      Clear the previous selection and ensure the new cell is selected.
		''' <li> <code>toggle</code>: <em>false</em>, <code>extend</code>: <em>true</em>.
		'''      Extend the previous selection from the anchor to the specified cell,
		'''      clearing all other selections.
		''' <li> <code>toggle</code>: <em>true</em>, <code>extend</code>: <em>false</em>.
		'''      If the specified cell is selected, deselect it. If it is not selected, select it.
		''' <li> <code>toggle</code>: <em>true</em>, <code>extend</code>: <em>true</em>.
		'''      Apply the selection state of the anchor to all cells between it and the
		'''      specified cell.
		''' </ul> </summary>
		''' <param name="rowIndex">   affects the selection at <code>row</code> </param>
		''' <param name="columnIndex">  affects the selection at <code>column</code> </param>
		''' <param name="toggle">  see description above </param>
		''' <param name="extend">  if true, extend the current selection
		''' 
		''' @since 1.3 </param>
		Public Overridable Sub changeSelection(ByVal rowIndex As Integer, ByVal columnIndex As Integer, ByVal toggle As Boolean, ByVal extend As Boolean)
			Dim rsm As ListSelectionModel = selectionModel
			Dim csm As ListSelectionModel = columnModel.selectionModel

			Dim anchorRow As Integer = getAdjustedIndex(rsm.anchorSelectionIndex, True)
			Dim anchorCol As Integer = getAdjustedIndex(csm.anchorSelectionIndex, False)

			Dim anchorSelected As Boolean = True

			If anchorRow = -1 Then
				If rowCount > 0 Then anchorRow = 0
				anchorSelected = False
			End If

			If anchorCol = -1 Then
				If columnCount > 0 Then anchorCol = 0
				anchorSelected = False
			End If

			' Check the selection here rather than in each selection model.
			' This is significant in cell selection mode if we are supposed
			' to be toggling the selection. In this case it is better to
			' ensure that the cell's selection state will indeed be changed.
			' If this were done in the code for the selection model it
			' might leave a cell in selection state if the row was
			' selected but the column was not - as it would toggle them both.
			Dim selected As Boolean = isCellSelected(rowIndex, columnIndex)
			anchorSelected = anchorSelected AndAlso isCellSelected(anchorRow, anchorCol)

			changeSelectionModel(csm, columnIndex, toggle, extend, selected, anchorCol, anchorSelected)
			changeSelectionModel(rsm, rowIndex, toggle, extend, selected, anchorRow, anchorSelected)

			' Scroll after changing the selection as blit scrolling is immediate,
			' so that if we cause the repaint after the scroll we end up painting
			' everything!
			If autoscrolls Then
				Dim ___cellRect As Rectangle = getCellRect(rowIndex, columnIndex, False)
				If ___cellRect IsNot Nothing Then scrollRectToVisible(___cellRect)
			End If
		End Sub

		''' <summary>
		''' Returns the foreground color for selected cells.
		''' </summary>
		''' <returns> the <code>Color</code> object for the foreground property </returns>
		''' <seealso cref= #setSelectionForeground </seealso>
		''' <seealso cref= #setSelectionBackground </seealso>
		Public Overridable Property selectionForeground As Color
			Get
				Return selectionForeground
			End Get
			Set(ByVal selectionForeground As Color)
				Dim old As Color = Me.selectionForeground
				Me.selectionForeground = selectionForeground
				firePropertyChange("selectionForeground", old, selectionForeground)
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the background color for selected cells.
		''' </summary>
		''' <returns> the <code>Color</code> used for the background of selected list items </returns>
		''' <seealso cref= #setSelectionBackground </seealso>
		''' <seealso cref= #setSelectionForeground </seealso>
		Public Overridable Property selectionBackground As Color
			Get
				Return selectionBackground
			End Get
			Set(ByVal selectionBackground As Color)
				Dim old As Color = Me.selectionBackground
				Me.selectionBackground = selectionBackground
				firePropertyChange("selectionBackground", old, selectionBackground)
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the <code>TableColumn</code> object for the column in the table
		''' whose identifier is equal to <code>identifier</code>, when compared using
		''' <code>equals</code>.
		''' </summary>
		''' <returns>  the <code>TableColumn</code> object that matches the identifier </returns>
		''' <exception cref="IllegalArgumentException">      if <code>identifier</code> is <code>null</code> or no <code>TableColumn</code> has this identifier
		''' </exception>
		''' <param name="identifier">                      the identifier object </param>
		Public Overridable Function getColumn(ByVal identifier As Object) As TableColumn
			Dim cm As TableColumnModel = columnModel
			Dim columnIndex As Integer = cm.getColumnIndex(identifier)
			Return cm.getColumn(columnIndex)
		End Function

	'
	' Informally implement the TableModel interface.
	'

		''' <summary>
		''' Maps the index of the column in the view at
		''' <code>viewColumnIndex</code> to the index of the column
		''' in the table model.  Returns the index of the corresponding
		''' column in the model.  If <code>viewColumnIndex</code>
		''' is less than zero, returns <code>viewColumnIndex</code>.
		''' </summary>
		''' <param name="viewColumnIndex">     the index of the column in the view </param>
		''' <returns>  the index of the corresponding column in the model
		''' </returns>
		''' <seealso cref= #convertColumnIndexToView </seealso>
		Public Overridable Function convertColumnIndexToModel(ByVal viewColumnIndex As Integer) As Integer
			Return sun.swing.SwingUtilities2.convertColumnIndexToModel(columnModel, viewColumnIndex)
		End Function

		''' <summary>
		''' Maps the index of the column in the table model at
		''' <code>modelColumnIndex</code> to the index of the column
		''' in the view.  Returns the index of the
		''' corresponding column in the view; returns -1 if this column is not
		''' being displayed.  If <code>modelColumnIndex</code> is less than zero,
		''' returns <code>modelColumnIndex</code>.
		''' </summary>
		''' <param name="modelColumnIndex">     the index of the column in the model </param>
		''' <returns>   the index of the corresponding column in the view
		''' </returns>
		''' <seealso cref= #convertColumnIndexToModel </seealso>
		Public Overridable Function convertColumnIndexToView(ByVal modelColumnIndex As Integer) As Integer
			Return sun.swing.SwingUtilities2.convertColumnIndexToView(columnModel, modelColumnIndex)
		End Function

		''' <summary>
		''' Maps the index of the row in terms of the
		''' <code>TableModel</code> to the view.  If the contents of the
		''' model are not sorted the model and view indices are the same.
		''' </summary>
		''' <param name="modelRowIndex"> the index of the row in terms of the model </param>
		''' <returns> the index of the corresponding row in the view, or -1 if
		'''         the row isn't visible </returns>
		''' <exception cref="IndexOutOfBoundsException"> if sorting is enabled and passed an
		'''         index outside the number of rows of the <code>TableModel</code> </exception>
		''' <seealso cref= javax.swing.table.TableRowSorter
		''' @since 1.6 </seealso>
		Public Overridable Function convertRowIndexToView(ByVal modelRowIndex As Integer) As Integer
			Dim sorter As RowSorter = rowSorter
			If sorter IsNot Nothing Then Return sorter.convertRowIndexToView(modelRowIndex)
			Return modelRowIndex
		End Function

		''' <summary>
		''' Maps the index of the row in terms of the view to the
		''' underlying <code>TableModel</code>.  If the contents of the
		''' model are not sorted the model and view indices are the same.
		''' </summary>
		''' <param name="viewRowIndex"> the index of the row in the view </param>
		''' <returns> the index of the corresponding row in the model </returns>
		''' <exception cref="IndexOutOfBoundsException"> if sorting is enabled and passed an
		'''         index outside the range of the <code>JTable</code> as
		'''         determined by the method <code>getRowCount</code> </exception>
		''' <seealso cref= javax.swing.table.TableRowSorter </seealso>
		''' <seealso cref= #getRowCount
		''' @since 1.6 </seealso>
		Public Overridable Function convertRowIndexToModel(ByVal viewRowIndex As Integer) As Integer
			Dim sorter As RowSorter = rowSorter
			If sorter IsNot Nothing Then Return sorter.convertRowIndexToModel(viewRowIndex)
			Return viewRowIndex
		End Function

		''' <summary>
		''' Returns the number of rows that can be shown in the
		''' <code>JTable</code>, given unlimited space.  If a
		''' <code>RowSorter</code> with a filter has been specified, the
		''' number of rows returned may differ from that of the underlying
		''' <code>TableModel</code>.
		''' </summary>
		''' <returns> the number of rows shown in the <code>JTable</code> </returns>
		''' <seealso cref= #getColumnCount </seealso>
		Public Overridable Property rowCount As Integer
			Get
				Dim sorter As RowSorter = rowSorter
				If sorter IsNot Nothing Then Return sorter.viewRowCount
				Return model.rowCount
			End Get
		End Property

		''' <summary>
		''' Returns the number of columns in the column model. Note that this may
		''' be different from the number of columns in the table model.
		''' </summary>
		''' <returns>  the number of columns in the table </returns>
		''' <seealso cref= #getRowCount </seealso>
		''' <seealso cref= #removeColumn </seealso>
		Public Overridable Property columnCount As Integer
			Get
				Return columnModel.columnCount
			End Get
		End Property

		''' <summary>
		''' Returns the name of the column appearing in the view at
		''' column position <code>column</code>.
		''' </summary>
		''' <param name="column">    the column in the view being queried </param>
		''' <returns> the name of the column at position <code>column</code>
		'''                    in the view where the first column is column 0 </returns>
		Public Overridable Function getColumnName(ByVal column As Integer) As String
			Return model.getColumnName(convertColumnIndexToModel(column))
		End Function

		''' <summary>
		''' Returns the type of the column appearing in the view at
		''' column position <code>column</code>.
		''' </summary>
		''' <param name="column">   the column in the view being queried </param>
		''' <returns> the type of the column at position <code>column</code>
		'''          in the view where the first column is column 0 </returns>
		Public Overridable Function getColumnClass(ByVal column As Integer) As Type
			Return model.getColumnClass(convertColumnIndexToModel(column))
		End Function

		''' <summary>
		''' Returns the cell value at <code>row</code> and <code>column</code>.
		''' <p>
		''' <b>Note</b>: The column is specified in the table view's display
		'''              order, and not in the <code>TableModel</code>'s column
		'''              order.  This is an important distinction because as the
		'''              user rearranges the columns in the table,
		'''              the column at a given index in the view will change.
		'''              Meanwhile the user's actions never affect the model's
		'''              column ordering.
		''' </summary>
		''' <param name="row">             the row whose value is to be queried </param>
		''' <param name="column">          the column whose value is to be queried </param>
		''' <returns>  the Object at the specified cell </returns>
		Public Overridable Function getValueAt(ByVal row As Integer, ByVal column As Integer) As Object
			Return model.getValueAt(convertRowIndexToModel(row), convertColumnIndexToModel(column))
		End Function

		''' <summary>
		''' Sets the value for the cell in the table model at <code>row</code>
		''' and <code>column</code>.
		''' <p>
		''' <b>Note</b>: The column is specified in the table view's display
		'''              order, and not in the <code>TableModel</code>'s column
		'''              order.  This is an important distinction because as the
		'''              user rearranges the columns in the table,
		'''              the column at a given index in the view will change.
		'''              Meanwhile the user's actions never affect the model's
		'''              column ordering.
		''' 
		''' <code>aValue</code> is the new value.
		''' </summary>
		''' <param name="aValue">          the new value </param>
		''' <param name="row">             the row of the cell to be changed </param>
		''' <param name="column">          the column of the cell to be changed </param>
		''' <seealso cref= #getValueAt </seealso>
		Public Overridable Sub setValueAt(ByVal aValue As Object, ByVal row As Integer, ByVal column As Integer)
			model.valueAteAt(aValue, convertRowIndexToModel(row), convertColumnIndexToModel(column))
		End Sub

		''' <summary>
		''' Returns true if the cell at <code>row</code> and <code>column</code>
		''' is editable.  Otherwise, invoking <code>setValueAt</code> on the cell
		''' will have no effect.
		''' <p>
		''' <b>Note</b>: The column is specified in the table view's display
		'''              order, and not in the <code>TableModel</code>'s column
		'''              order.  This is an important distinction because as the
		'''              user rearranges the columns in the table,
		'''              the column at a given index in the view will change.
		'''              Meanwhile the user's actions never affect the model's
		'''              column ordering.
		''' 
		''' </summary>
		''' <param name="row">      the row whose value is to be queried </param>
		''' <param name="column">   the column whose value is to be queried </param>
		''' <returns>  true if the cell is editable </returns>
		''' <seealso cref= #setValueAt </seealso>
		Public Overridable Function isCellEditable(ByVal row As Integer, ByVal column As Integer) As Boolean
			Return model.isCellEditable(convertRowIndexToModel(row), convertColumnIndexToModel(column))
		End Function
	'
	' Adding and removing columns in the view
	'

		''' <summary>
		'''  Appends <code>aColumn</code> to the end of the array of columns held by
		'''  this <code>JTable</code>'s column model.
		'''  If the column name of <code>aColumn</code> is <code>null</code>,
		'''  sets the column name of <code>aColumn</code> to the name
		'''  returned by <code>getModel().getColumnName()</code>.
		'''  <p>
		'''  To add a column to this <code>JTable</code> to display the
		'''  <code>modelColumn</code>'th column of data in the model with a
		'''  given <code>width</code>, <code>cellRenderer</code>,
		'''  and <code>cellEditor</code> you can use:
		'''  <pre>
		''' 
		'''      addColumn(new TableColumn(modelColumn, width, cellRenderer, cellEditor));
		''' 
		'''  </pre>
		'''  [Any of the <code>TableColumn</code> constructors can be used
		'''  instead of this one.]
		'''  The model column number is stored inside the <code>TableColumn</code>
		'''  and is used during rendering and editing to locate the appropriates
		'''  data values in the model. The model column number does not change
		'''  when columns are reordered in the view.
		''' </summary>
		'''  <param name="aColumn">         the <code>TableColumn</code> to be added </param>
		'''  <seealso cref=    #removeColumn </seealso>
		Public Overridable Sub addColumn(ByVal aColumn As TableColumn)
			If aColumn.headerValue Is Nothing Then
				Dim modelColumn As Integer = aColumn.modelIndex
				Dim ___columnName As String = model.getColumnName(modelColumn)
				aColumn.headerValue = ___columnName
			End If
			columnModel.addColumn(aColumn)
		End Sub

		''' <summary>
		'''  Removes <code>aColumn</code> from this <code>JTable</code>'s
		'''  array of columns.  Note: this method does not remove the column
		'''  of data from the model; it just removes the <code>TableColumn</code>
		'''  that was responsible for displaying it.
		''' </summary>
		'''  <param name="aColumn">         the <code>TableColumn</code> to be removed </param>
		'''  <seealso cref=    #addColumn </seealso>
		Public Overridable Sub removeColumn(ByVal aColumn As TableColumn)
			columnModel.removeColumn(aColumn)
		End Sub

		''' <summary>
		''' Moves the column <code>column</code> to the position currently
		''' occupied by the column <code>targetColumn</code> in the view.
		''' The old column at <code>targetColumn</code> is
		''' shifted left or right to make room.
		''' </summary>
		''' <param name="column">                  the index of column to be moved </param>
		''' <param name="targetColumn">            the new index of the column </param>
		Public Overridable Sub moveColumn(ByVal column As Integer, ByVal targetColumn As Integer)
			columnModel.moveColumn(column, targetColumn)
		End Sub

	'
	' Cover methods for various models and helper methods
	'

		''' <summary>
		''' Returns the index of the column that <code>point</code> lies in,
		''' or -1 if the result is not in the range
		''' [0, <code>getColumnCount()</code>-1].
		''' </summary>
		''' <param name="point">   the location of interest </param>
		''' <returns>  the index of the column that <code>point</code> lies in,
		'''          or -1 if the result is not in the range
		'''          [0, <code>getColumnCount()</code>-1] </returns>
		''' <seealso cref=     #rowAtPoint </seealso>
		Public Overridable Function columnAtPoint(ByVal point As Point) As Integer
			Dim ___x As Integer = point.x
			If Not componentOrientation.leftToRight Then ___x = width - ___x - 1
			Return columnModel.getColumnIndexAtX(___x)
		End Function

		''' <summary>
		''' Returns the index of the row that <code>point</code> lies in,
		''' or -1 if the result is not in the range
		''' [0, <code>getRowCount()</code>-1].
		''' </summary>
		''' <param name="point">   the location of interest </param>
		''' <returns>  the index of the row that <code>point</code> lies in,
		'''          or -1 if the result is not in the range
		'''          [0, <code>getRowCount()</code>-1] </returns>
		''' <seealso cref=     #columnAtPoint </seealso>
		Public Overridable Function rowAtPoint(ByVal point As Point) As Integer
			Dim ___y As Integer = point.y
			Dim result As Integer = If(rowModel Is Nothing, ___y\rowHeight, rowModel.getIndex(___y))
			If result < 0 Then
				Return -1
			ElseIf result >= rowCount Then
				Return -1
			Else
				Return result
			End If
		End Function

		''' <summary>
		''' Returns a rectangle for the cell that lies at the intersection of
		''' <code>row</code> and <code>column</code>.
		''' If <code>includeSpacing</code> is true then the value returned
		''' has the full height and width of the row and column
		''' specified. If it is false, the returned rectangle is inset by the
		''' intercell spacing to return the true bounds of the rendering or
		''' editing component as it will be set during rendering.
		''' <p>
		''' If the column index is valid but the row index is less
		''' than zero the method returns a rectangle with the
		''' <code>y</code> and <code>height</code> values set appropriately
		''' and the <code>x</code> and <code>width</code> values both set
		''' to zero. In general, when either the row or column indices indicate a
		''' cell outside the appropriate range, the method returns a rectangle
		''' depicting the closest edge of the closest cell that is within
		''' the table's range. When both row and column indices are out
		''' of range the returned rectangle covers the closest
		''' point of the closest cell.
		''' <p>
		''' In all cases, calculations that use this method to calculate
		''' results along one axis will not fail because of anomalies in
		''' calculations along the other axis. When the cell is not valid
		''' the <code>includeSpacing</code> parameter is ignored.
		''' </summary>
		''' <param name="row">                   the row index where the desired cell
		'''                                is located </param>
		''' <param name="column">                the column index where the desired cell
		'''                                is located in the display; this is not
		'''                                necessarily the same as the column index
		'''                                in the data model for the table; the
		'''                                <seealso cref="#convertColumnIndexToView(int)"/>
		'''                                method may be used to convert a data
		'''                                model column index to a display
		'''                                column index </param>
		''' <param name="includeSpacing">        if false, return the true cell bounds -
		'''                                computed by subtracting the intercell
		'''                                spacing from the height and widths of
		'''                                the column and row models
		''' </param>
		''' <returns>  the rectangle containing the cell at location
		'''          <code>row</code>,<code>column</code> </returns>
		''' <seealso cref= #getIntercellSpacing </seealso>
		Public Overridable Function getCellRect(ByVal row As Integer, ByVal column As Integer, ByVal includeSpacing As Boolean) As Rectangle
			Dim r As New Rectangle
			Dim valid As Boolean = True
			If row < 0 Then
				' y = height = 0;
				valid = False
			ElseIf row >= rowCount Then
				r.y = height
				valid = False
			Else
				r.height = getRowHeight(row)
				r.y = If(rowModel Is Nothing, row * r.height, rowModel.getPosition(row))
			End If

			If column < 0 Then
				If Not componentOrientation.leftToRight Then r.x = width
				' otherwise, x = width = 0;
				valid = False
			ElseIf column >= columnCount Then
				If componentOrientation.leftToRight Then r.x = width
				' otherwise, x = width = 0;
				valid = False
			Else
				Dim cm As TableColumnModel = columnModel
				If componentOrientation.leftToRight Then
					For i As Integer = 0 To column - 1
						r.x += cm.getColumn(i).width
					Next i
				Else
					For i As Integer = cm.columnCount-1 To column + 1 Step -1
						r.x += cm.getColumn(i).width
					Next i
				End If
				r.width = cm.getColumn(column).width
			End If

			If valid AndAlso (Not includeSpacing) Then
				' Bound the margins by their associated dimensions to prevent
				' returning bounds with negative dimensions.
				Dim rm As Integer = Math.Min(rowMargin, r.height)
				Dim cm As Integer = Math.Min(columnModel.columnMargin, r.width)
				' This is not the same as grow(), it rounds differently.
				r.boundsnds(r.x + cm\2, r.y + rm\2, r.width - cm, r.height - rm)
			End If
			Return r
		End Function

		Private Function viewIndexForColumn(ByVal aColumn As TableColumn) As Integer
			Dim cm As TableColumnModel = columnModel
			For ___column As Integer = 0 To cm.columnCount - 1
				If cm.getColumn(___column) Is aColumn Then Return ___column
			Next ___column
			Return -1
		End Function

		''' <summary>
		''' Causes this table to lay out its rows and columns.  Overridden so
		''' that columns can be resized to accommodate a change in the size of
		''' a containing parent.
		''' Resizes one or more of the columns in the table
		''' so that the total width of all of this <code>JTable</code>'s
		''' columns is equal to the width of the table.
		''' <p>
		''' Before the layout begins the method gets the
		''' <code>resizingColumn</code> of the <code>tableHeader</code>.
		''' When the method is called as a result of the resizing of an enclosing window,
		''' the <code>resizingColumn</code> is <code>null</code>. This means that resizing
		''' has taken place "outside" the <code>JTable</code> and the change -
		''' or "delta" - should be distributed to all of the columns regardless
		''' of this <code>JTable</code>'s automatic resize mode.
		''' <p>
		''' If the <code>resizingColumn</code> is not <code>null</code>, it is one of
		''' the columns in the table that has changed size rather than
		''' the table itself. In this case the auto-resize modes govern
		''' the way the extra (or deficit) space is distributed
		''' amongst the available columns.
		''' <p>
		''' The modes are:
		''' <ul>
		''' <li>  AUTO_RESIZE_OFF: Don't automatically adjust the column's
		''' widths at all. Use a horizontal scrollbar to accommodate the
		''' columns when their sum exceeds the width of the
		''' <code>Viewport</code>.  If the <code>JTable</code> is not
		''' enclosed in a <code>JScrollPane</code> this may
		''' leave parts of the table invisible.
		''' <li>  AUTO_RESIZE_NEXT_COLUMN: Use just the column after the
		''' resizing column. This results in the "boundary" or divider
		''' between adjacent cells being independently adjustable.
		''' <li>  AUTO_RESIZE_SUBSEQUENT_COLUMNS: Use all columns after the
		''' one being adjusted to absorb the changes.  This is the
		''' default behavior.
		''' <li>  AUTO_RESIZE_LAST_COLUMN: Automatically adjust the
		''' size of the last column only. If the bounds of the last column
		''' prevent the desired size from being allocated, set the
		''' width of the last column to the appropriate limit and make
		''' no further adjustments.
		''' <li>  AUTO_RESIZE_ALL_COLUMNS: Spread the delta amongst all the columns
		''' in the <code>JTable</code>, including the one that is being
		''' adjusted.
		''' </ul>
		''' <p>
		''' <b>Note:</b> When a <code>JTable</code> makes adjustments
		'''   to the widths of the columns it respects their minimum and
		'''   maximum values absolutely.  It is therefore possible that,
		'''   even after this method is called, the total width of the columns
		'''   is still not equal to the width of the table. When this happens
		'''   the <code>JTable</code> does not put itself
		'''   in AUTO_RESIZE_OFF mode to bring up a scroll bar, or break other
		'''   commitments of its current auto-resize mode -- instead it
		'''   allows its bounds to be set larger (or smaller) than the total of the
		'''   column minimum or maximum, meaning, either that there
		'''   will not be enough room to display all of the columns, or that the
		'''   columns will not fill the <code>JTable</code>'s bounds.
		'''   These respectively, result in the clipping of some columns
		'''   or an area being painted in the <code>JTable</code>'s
		'''   background color during painting.
		''' <p>
		'''   The mechanism for distributing the delta amongst the available
		'''   columns is provided in a private method in the <code>JTable</code>
		'''   class:
		''' <pre>
		'''   adjustSizes(long targetSize, final Resizable3 r, boolean inverse)
		''' </pre>
		'''   an explanation of which is provided in the following section.
		'''   <code>Resizable3</code> is a private
		'''   interface that allows any data structure containing a collection
		'''   of elements with a size, preferred size, maximum size and minimum size
		'''   to have its elements manipulated by the algorithm.
		''' 
		''' <H3> Distributing the delta </H3>
		''' 
		''' <H4> Overview </H4>
		''' <P>
		''' Call "DELTA" the difference between the target size and the
		''' sum of the preferred sizes of the elements in r. The individual
		''' sizes are calculated by taking the original preferred
		''' sizes and adding a share of the DELTA - that share being based on
		''' how far each preferred size is from its limiting bound (minimum or
		''' maximum).
		''' 
		''' <H4>Definition</H4>
		''' <P>
		''' Call the individual constraints min[i], max[i], and pref[i].
		''' <p>
		''' Call their respective sums: MIN, MAX, and PREF.
		''' <p>
		''' Each new size will be calculated using:
		''' 
		''' <pre>
		'''          size[i] = pref[i] + delta[i]
		''' </pre>
		''' where each individual delta[i] is calculated according to:
		''' <p>
		''' If (DELTA &lt; 0) we are in shrink mode where:
		''' 
		''' <PRE>
		'''                        DELTA
		'''          delta[i] = ------------ * (pref[i] - min[i])
		'''                     (PREF - MIN)
		''' </PRE>
		''' If (DELTA &gt; 0) we are in expand mode where:
		''' 
		''' <PRE>
		'''                        DELTA
		'''          delta[i] = ------------ * (max[i] - pref[i])
		'''                      (MAX - PREF)
		''' </PRE>
		''' <P>
		''' The overall effect is that the total size moves that same percentage,
		''' k, towards the total minimum or maximum and that percentage guarantees
		''' accommodation of the required space, DELTA.
		''' 
		''' <H4>Details</H4>
		''' <P>
		''' Naive evaluation of the formulae presented here would be subject to
		''' the aggregated rounding errors caused by doing this operation in finite
		''' precision (using ints). To deal with this, the multiplying factor above,
		''' is constantly recalculated and this takes account of the rounding
		''' errors in the previous iterations. The result is an algorithm that
		''' produces a set of integers whose values exactly sum to the supplied
		''' <code>targetSize</code>, and does so by spreading the rounding
		''' errors evenly over the given elements.
		''' 
		''' <H4>When the MAX and MIN bounds are hit</H4>
		''' <P>
		''' When <code>targetSize</code> is outside the [MIN, MAX] range,
		''' the algorithm sets all sizes to their appropriate limiting value
		''' (maximum or minimum).
		''' 
		''' </summary>
		Public Overridable Sub doLayout()
			Dim ___resizingColumn As TableColumn = resizingColumn
			If ___resizingColumn Is Nothing Then
				widthsFromPreferredWidths = False
			Else
				' JTable behaves like a layout manger - but one in which the
				' user can come along and dictate how big one of the children
				' (columns) is supposed to be.

				' A column has been resized and JTable may need to distribute
				' any overall delta to other columns, according to the resize mode.
				Dim columnIndex As Integer = viewIndexForColumn(___resizingColumn)
				Dim delta As Integer = width - columnModel.totalColumnWidth
				accommodateDelta(columnIndex, delta)
				delta = width - columnModel.totalColumnWidth

				' If the delta cannot be completely accomodated, then the
				' resizing column will have to take any remainder. This means
				' that the column is not being allowed to take the requested
				' width. This happens under many circumstances: For example,
				' AUTO_RESIZE_NEXT_COLUMN specifies that any delta be distributed
				' to the column after the resizing column. If one were to attempt
				' to resize the last column of the table, there would be no
				' columns after it, and hence nowhere to distribute the delta.
				' It would then be given entirely back to the resizing column,
				' preventing it from changing size.
				If delta <> 0 Then ___resizingColumn.width = ___resizingColumn.width + delta

				' At this point the JTable has to work out what preferred sizes
				' would have resulted in the layout the user has chosen.
				' Thereafter, during window resizing etc. it has to work off
				' the preferred sizes as usual - the idea being that, whatever
				' the user does, everything stays in synch and things don't jump
				' around.
				widthsFromPreferredWidths = True
			End If

			MyBase.doLayout()
		End Sub

		Private Property resizingColumn As TableColumn
			Get
				Return If(tableHeader Is Nothing, Nothing, tableHeader.resizingColumn)
			End Get
		End Property

		''' <summary>
		''' Sizes the table columns to fit the available space. </summary>
		''' @deprecated As of Swing version 1.0.3,
		''' replaced by <code>doLayout()</code>. 
		''' <seealso cref= #doLayout </seealso>
		<Obsolete("As of Swing version 1.0.3,")> _
		Public Overridable Sub sizeColumnsToFit(ByVal lastColumnOnly As Boolean)
			Dim oldAutoResizeMode As Integer = autoResizeMode
			autoResizeMode = If(lastColumnOnly, AUTO_RESIZE_LAST_COLUMN, AUTO_RESIZE_ALL_COLUMNS)
			sizeColumnsToFit(-1)
			autoResizeMode = oldAutoResizeMode
		End Sub

		''' <summary>
		''' Obsolete as of Java 2 platform v1.4.  Please use the
		''' <code>doLayout()</code> method instead. </summary>
		''' <param name="resizingColumn">    the column whose resizing made this adjustment
		'''                          necessary or -1 if there is no such column </param>
		''' <seealso cref=  #doLayout </seealso>
		Public Overridable Sub sizeColumnsToFit(ByVal resizingColumn As Integer)
			If resizingColumn = -1 Then
				widthsFromPreferredWidths = False
			Else
				If autoResizeMode = AUTO_RESIZE_OFF Then
					Dim aColumn As TableColumn = columnModel.getColumn(resizingColumn)
					aColumn.preferredWidth = aColumn.width
				Else
					Dim delta As Integer = width - columnModel.totalColumnWidth
					accommodateDelta(resizingColumn, delta)
					widthsFromPreferredWidths = True
				End If
			End If
		End Sub

		Private Property widthsFromPreferredWidths As Boolean
			Set(ByVal inverse As Boolean)
				Dim totalWidth As Integer = width
				Dim totalPreferred As Integer = preferredSize.width
				Dim target As Integer = If((Not inverse), totalWidth, totalPreferred)
    
				Dim cm As TableColumnModel = columnModel
				Dim r As Resizable3 = New Resizable3AnonymousInnerClassHelper
    
				adjustSizes(target, r, inverse)
			End Set
		End Property

		Private Class Resizable3AnonymousInnerClassHelper
			Implements Resizable3

			Public Overridable Property elementCount As Integer
				Get
					Return cm.columnCount
				End Get
			End Property
			Public Overridable Function getLowerBoundAt(ByVal i As Integer) As Integer
				Return cm.getColumn(i).minWidth
			End Function
			Public Overridable Function getUpperBoundAt(ByVal i As Integer) As Integer
				Return cm.getColumn(i).maxWidth
			End Function
			Public Overridable Function getMidPointAt(ByVal i As Integer) As Integer
				If Not inverse Then
					Return cm.getColumn(i).preferredWidth
				Else
					Return cm.getColumn(i).width
				End If
			End Function
			Public Overridable Sub setSizeAt(ByVal s As Integer, ByVal i As Integer)
				If Not inverse Then
					cm.getColumn(i).width = s
				Else
					cm.getColumn(i).preferredWidth = s
				End If
			End Sub
		End Class


		' Distribute delta over columns, as indicated by the autoresize mode.
		Private Sub accommodateDelta(ByVal resizingColumnIndex As Integer, ByVal delta As Integer)
			Dim ___columnCount As Integer = columnCount
			Dim [from] As Integer = resizingColumnIndex
			Dim [to] As Integer

			' Use the mode to determine how to absorb the changes.
			Select Case autoResizeMode
				Case AUTO_RESIZE_NEXT_COLUMN
					[from] = [from] + 1
					[to] = Math.Min([from] + 1, ___columnCount)
				Case AUTO_RESIZE_SUBSEQUENT_COLUMNS
					[from] = [from] + 1
					[to] = ___columnCount
				Case AUTO_RESIZE_LAST_COLUMN
					[from] = ___columnCount - 1
					[to] = [from] + 1
				Case AUTO_RESIZE_ALL_COLUMNS
					[from] = 0
					[to] = ___columnCount
				Case Else
					Return
			End Select

			Dim start As Integer = [from]
			Dim [end] As Integer = [to]
			Dim cm As TableColumnModel = columnModel
			Dim r As Resizable3 = New Resizable3AnonymousInnerClassHelper2

			Dim totalWidth As Integer = 0
			For i As Integer = from To [to] - 1
				Dim aColumn As TableColumn = columnModel.getColumn(i)
				Dim input As Integer = aColumn.width
				totalWidth = totalWidth + input
			Next i

			adjustSizes(totalWidth + delta, r, False)
		End Sub

		Private Class Resizable3AnonymousInnerClassHelper2
			Implements Resizable3

			Public Overridable Property elementCount As Integer
				Get
					Return end-start
				End Get
			End Property
			Public Overridable Function getLowerBoundAt(ByVal i As Integer) As Integer
				Return cm.getColumn(i+start).minWidth
			End Function
			Public Overridable Function getUpperBoundAt(ByVal i As Integer) As Integer
				Return cm.getColumn(i+start).maxWidth
			End Function
			Public Overridable Function getMidPointAt(ByVal i As Integer) As Integer
				Return cm.getColumn(i+start).width
			End Function
			Public Overridable Sub setSizeAt(ByVal s As Integer, ByVal i As Integer)
				cm.getColumn(i+start).width = s
			End Sub
		End Class

		Private Interface Resizable2
			ReadOnly Property elementCount As Integer
			Function getLowerBoundAt(ByVal i As Integer) As Integer
			Function getUpperBoundAt(ByVal i As Integer) As Integer
			Sub setSizeAt(ByVal newSize As Integer, ByVal i As Integer)
		End Interface

		Private Interface Resizable3
			Inherits Resizable2

			Function getMidPointAt(ByVal i As Integer) As Integer
		End Interface


		Private Sub adjustSizes(ByVal target As Long, ByVal r As Resizable3, ByVal inverse As Boolean)
			Dim N As Integer = r.elementCount
			Dim totalPreferred As Long = 0
			For i As Integer = 0 To N - 1
				totalPreferred += r.getMidPointAt(i)
			Next i
			Dim s As Resizable2
			If (target < totalPreferred) = (Not inverse) Then
				s = New Resizable2AnonymousInnerClassHelper
			Else
				s = New Resizable2AnonymousInnerClassHelper2
			End If
			adjustSizes(target, s, (Not inverse))
		End Sub

		Private Class Resizable2AnonymousInnerClassHelper
			Implements Resizable2

			Public Overridable Property elementCount As Integer
				Get
					Return r.elementCount
				End Get
			End Property
			Public Overridable Function getLowerBoundAt(ByVal i As Integer) As Integer
				Return r.getLowerBoundAt(i)
			End Function
			Public Overridable Function getUpperBoundAt(ByVal i As Integer) As Integer
				Return r.getMidPointAt(i)
			End Function
			Public Overridable Sub setSizeAt(ByVal newSize As Integer, ByVal i As Integer)
				r.sizeAteAt(newSize, i)
			End Sub

		End Class

		Private Class Resizable2AnonymousInnerClassHelper2
			Implements Resizable2

			Public Overridable Property elementCount As Integer
				Get
					Return r.elementCount
				End Get
			End Property
			Public Overridable Function getLowerBoundAt(ByVal i As Integer) As Integer
				Return r.getMidPointAt(i)
			End Function
			Public Overridable Function getUpperBoundAt(ByVal i As Integer) As Integer
				Return r.getUpperBoundAt(i)
			End Function
			Public Overridable Sub setSizeAt(ByVal newSize As Integer, ByVal i As Integer)
				r.sizeAteAt(newSize, i)
			End Sub

		End Class

		Private Sub adjustSizes(ByVal target As Long, ByVal r As Resizable2, ByVal limitToRange As Boolean)
			Dim totalLowerBound As Long = 0
			Dim totalUpperBound As Long = 0
			For i As Integer = 0 To r.elementCount - 1
				totalLowerBound += r.getLowerBoundAt(i)
				totalUpperBound += r.getUpperBoundAt(i)
			Next i

			If limitToRange Then target = Math.Min(Math.Max(totalLowerBound, target), totalUpperBound)

			For i As Integer = 0 To r.elementCount - 1
				Dim lowerBound As Integer = r.getLowerBoundAt(i)
				Dim upperBound As Integer = r.getUpperBoundAt(i)
				' Check for zero. This happens when the distribution of the delta
				' finishes early due to a series of "fixed" entries at the end.
				' In this case, lowerBound == upperBound, for all subsequent terms.
				Dim newSize As Integer
				If totalLowerBound = totalUpperBound Then
					newSize = lowerBound
				Else
					Dim f As Double = CDbl(target - totalLowerBound)/(totalUpperBound - totalLowerBound)
					newSize = CInt(Fix(Math.Round(lowerBound+f*(upperBound - lowerBound))))
					' We'd need to round manually in an all integer version.
					' size[i] = (int)(((totalUpperBound - target) * lowerBound +
					'     (target - totalLowerBound) * upperBound)/(totalUpperBound-totalLowerBound));
				End If
				r.sizeAteAt(newSize, i)
				target -= newSize
				totalLowerBound -= lowerBound
				totalUpperBound -= upperBound
			Next i
		End Sub

		''' <summary>
		''' Overrides <code>JComponent</code>'s <code>getToolTipText</code>
		''' method in order to allow the renderer's tips to be used
		''' if it has text set.
		''' <p>
		''' <b>Note:</b> For <code>JTable</code> to properly display
		''' tooltips of its renderers
		''' <code>JTable</code> must be a registered component with the
		''' <code>ToolTipManager</code>.
		''' This is done automatically in <code>initializeLocalVars</code>,
		''' but if at a later point <code>JTable</code> is told
		''' <code>setToolTipText(null)</code> it will unregister the table
		''' component, and no tips from renderers will display anymore.
		''' </summary>
		''' <seealso cref= JComponent#getToolTipText </seealso>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			Dim tip As String = Nothing
			Dim p As Point = [event].point

			' Locate the renderer under the event location
			Dim hitColumnIndex As Integer = columnAtPoint(p)
			Dim hitRowIndex As Integer = rowAtPoint(p)

			If (hitColumnIndex <> -1) AndAlso (hitRowIndex <> -1) Then
				Dim renderer As TableCellRenderer = getCellRenderer(hitRowIndex, hitColumnIndex)
				Dim component As Component = prepareRenderer(renderer, hitRowIndex, hitColumnIndex)

				' Now have to see if the component is a JComponent before
				' getting the tip
				If TypeOf component Is JComponent Then
					' Convert the event to the renderer's coordinate system
					Dim ___cellRect As Rectangle = getCellRect(hitRowIndex, hitColumnIndex, False)
					p.translate(-___cellRect.x, -___cellRect.y)
					Dim newEvent As New MouseEvent(component, [event].iD, [event].when, [event].modifiers, p.x, p.y, [event].xOnScreen, [event].yOnScreen, [event].clickCount, [event].popupTrigger, MouseEvent.NOBUTTON)

					tip = CType(component, JComponent).getToolTipText(newEvent)
				End If
			End If

			' No tip from the renderer get our own tip
			If tip Is Nothing Then tip = toolTipText

			Return tip
		End Function

	'
	' Editing Support
	'

		''' <summary>
		''' Sets whether editors in this JTable get the keyboard focus
		''' when an editor is activated as a result of the JTable
		''' forwarding keyboard events for a cell.
		''' By default, this property is false, and the JTable
		''' retains the focus unless the cell is clicked.
		''' </summary>
		''' <param name="surrendersFocusOnKeystroke"> true if the editor should get the focus
		'''          when keystrokes cause the editor to be
		'''          activated
		''' 
		''' </param>
		''' <seealso cref= #getSurrendersFocusOnKeystroke
		''' @since 1.4 </seealso>
		Public Overridable Property surrendersFocusOnKeystroke As Boolean
			Set(ByVal surrendersFocusOnKeystroke As Boolean)
				Me.surrendersFocusOnKeystroke = surrendersFocusOnKeystroke
			End Set
			Get
				Return surrendersFocusOnKeystroke
			End Get
		End Property


		''' <summary>
		''' Programmatically starts editing the cell at <code>row</code> and
		''' <code>column</code>, if those indices are in the valid range, and
		''' the cell at those indices is editable.
		''' Note that this is a convenience method for
		''' <code>editCellAt(int, int, null)</code>.
		''' </summary>
		''' <param name="row">                             the row to be edited </param>
		''' <param name="column">                          the column to be edited </param>
		''' <returns>  false if for any reason the cell cannot be edited,
		'''                or if the indices are invalid </returns>
		Public Overridable Function editCellAt(ByVal row As Integer, ByVal column As Integer) As Boolean
			Return editCellAt(row, column, Nothing)
		End Function

		''' <summary>
		''' Programmatically starts editing the cell at <code>row</code> and
		''' <code>column</code>, if those indices are in the valid range, and
		''' the cell at those indices is editable.
		''' To prevent the <code>JTable</code> from
		''' editing a particular table, column or cell value, return false from
		''' the <code>isCellEditable</code> method in the <code>TableModel</code>
		''' interface.
		''' </summary>
		''' <param name="row">     the row to be edited </param>
		''' <param name="column">  the column to be edited </param>
		''' <param name="e">       event to pass into <code>shouldSelectCell</code>;
		'''                  note that as of Java 2 platform v1.2, the call to
		'''                  <code>shouldSelectCell</code> is no longer made </param>
		''' <returns>  false if for any reason the cell cannot be edited,
		'''                or if the indices are invalid </returns>
		Public Overridable Function editCellAt(ByVal row As Integer, ByVal column As Integer, ByVal e As EventObject) As Boolean
			If cellEditor IsNot Nothing AndAlso (Not cellEditor.stopCellEditing()) Then Return False

			If row < 0 OrElse row >= rowCount OrElse column < 0 OrElse column >= columnCount Then Return False

			If Not isCellEditable(row, column) Then Return False

			If editorRemover Is Nothing Then
				Dim fm As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
				editorRemover = New CellEditorRemover(Me, fm)
				fm.addPropertyChangeListener("permanentFocusOwner", editorRemover)
			End If

			Dim editor As TableCellEditor = getCellEditor(row, column)
			If editor IsNot Nothing AndAlso editor.isCellEditable(e) Then
				editorComp = prepareEditor(editor, row, column)
				If editorComp Is Nothing Then
					removeEditor()
					Return False
				End If
				editorComp.bounds = getCellRect(row, column, False)
				add(editorComp)
				editorComp.validate()
				editorComp.repaint()

				cellEditor = editor
				editingRow = row
				editingColumn = column
				editor.addCellEditorListener(Me)

				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if a cell is being edited.
		''' </summary>
		''' <returns>  true if the table is editing a cell </returns>
		''' <seealso cref=     #editingColumn </seealso>
		''' <seealso cref=     #editingRow </seealso>
		Public Overridable Property editing As Boolean
			Get
				Return cellEditor IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the component that is handling the editing session.
		''' If nothing is being edited, returns null.
		''' </summary>
		''' <returns>  Component handling editing session </returns>
		Public Overridable Property editorComponent As Component
			Get
				Return editorComp
			End Get
		End Property

		''' <summary>
		''' Returns the index of the column that contains the cell currently
		''' being edited.  If nothing is being edited, returns -1.
		''' </summary>
		''' <returns>  the index of the column that contains the cell currently
		'''          being edited; returns -1 if nothing being edited </returns>
		''' <seealso cref= #editingRow </seealso>
		Public Overridable Property editingColumn As Integer
			Get
				Return editingColumn
			End Get
		End Property

		''' <summary>
		''' Returns the index of the row that contains the cell currently
		''' being edited.  If nothing is being edited, returns -1.
		''' </summary>
		''' <returns>  the index of the row that contains the cell currently
		'''          being edited; returns -1 if nothing being edited </returns>
		''' <seealso cref= #editingColumn </seealso>
		Public Overridable Property editingRow As Integer
			Get
				Return editingRow
			End Get
		End Property

	'
	' Managing TableUI
	'

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the <code>TableUI</code> object that renders this component </returns>
		Public Overridable Property uI As TableUI
			Get
				Return CType(ui, TableUI)
			End Get
			Set(ByVal ui As TableUI)
				If Me.ui IsNot ui Then
					MyBase.uI = ui
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the L&amp;F has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			' Update the UIs of the cell renderers, cell editors and header renderers.
			Dim cm As TableColumnModel = columnModel
			For ___column As Integer = 0 To cm.columnCount - 1
				Dim aColumn As TableColumn = cm.getColumn(___column)
				SwingUtilities.updateRendererOrEditorUI(aColumn.cellRenderer)
				SwingUtilities.updateRendererOrEditorUI(aColumn.cellEditor)
				SwingUtilities.updateRendererOrEditorUI(aColumn.headerRenderer)
			Next ___column

			' Update the UIs of all the default renderers.
			Dim defaultRenderers As System.Collections.IEnumerator = defaultRenderersByColumnClass.Values.GetEnumerator()
			Do While defaultRenderers.hasMoreElements()
				SwingUtilities.updateRendererOrEditorUI(defaultRenderers.nextElement())
			Loop

			' Update the UIs of all the default editors.
			Dim defaultEditors As System.Collections.IEnumerator = defaultEditorsByColumnClass.Values.GetEnumerator()
			Do While defaultEditors.hasMoreElements()
				SwingUtilities.updateRendererOrEditorUI(defaultEditors.nextElement())
			Loop

			' Update the UI of the table header
			If tableHeader IsNot Nothing AndAlso tableHeader.parent Is Nothing Then tableHeader.updateUI()

			' Update UI applied to parent ScrollPane
			configureEnclosingScrollPaneUI()

			uI = CType(UIManager.getUI(Me), TableUI)
		End Sub

		''' <summary>
		''' Returns the suffix used to construct the name of the L&amp;F class used to
		''' render this component.
		''' </summary>
		''' <returns> the string "TableUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


	'
	' Managing models
	'

		''' <summary>
		''' Sets the data model for this table to <code>newModel</code> and registers
		''' with it for listener notifications from the new data model.
		''' </summary>
		''' <param name="dataModel">        the new data source for this table </param>
		''' <exception cref="IllegalArgumentException">      if <code>newModel</code> is <code>null</code> </exception>
		''' <seealso cref=     #getModel
		''' @beaninfo
		'''  bound: true
		'''  description: The model that is the source of the data for this view. </seealso>
		Public Overridable Property model As TableModel
			Set(ByVal dataModel As TableModel)
				If dataModel Is Nothing Then Throw New System.ArgumentException("Cannot set a null TableModel")
				If Me.dataModel IsNot dataModel Then
					Dim old As TableModel = Me.dataModel
					If old IsNot Nothing Then old.removeTableModelListener(Me)
					Me.dataModel = dataModel
					dataModel.addTableModelListener(Me)
    
					tableChanged(New TableModelEvent(dataModel, TableModelEvent.HEADER_ROW))
    
					firePropertyChange("model", old, dataModel)
    
					If autoCreateRowSorter Then rowSorter = New TableRowSorter(Of TableModel)(dataModel)
				End If
			End Set
			Get
				Return dataModel
			End Get
		End Property


		''' <summary>
		''' Sets the column model for this table to <code>newModel</code> and registers
		''' for listener notifications from the new column model. Also sets
		''' the column model of the <code>JTableHeader</code> to <code>columnModel</code>.
		''' </summary>
		''' <param name="columnModel">        the new data source for this table </param>
		''' <exception cref="IllegalArgumentException">      if <code>columnModel</code> is <code>null</code> </exception>
		''' <seealso cref=     #getColumnModel
		''' @beaninfo
		'''  bound: true
		'''  description: The object governing the way columns appear in the view. </seealso>
		Public Overridable Property columnModel As TableColumnModel
			Set(ByVal columnModel As TableColumnModel)
				If columnModel Is Nothing Then Throw New System.ArgumentException("Cannot set a null ColumnModel")
				Dim old As TableColumnModel = Me.columnModel
				If columnModel IsNot old Then
					If old IsNot Nothing Then old.removeColumnModelListener(Me)
					Me.columnModel = columnModel
					columnModel.addColumnModelListener(Me)
    
					' Set the column model of the header as well.
					If tableHeader IsNot Nothing Then tableHeader.columnModel = columnModel
    
					firePropertyChange("columnModel", old, columnModel)
					resizeAndRepaint()
				End If
			End Set
			Get
				Return columnModel
			End Get
		End Property


		''' <summary>
		''' Sets the row selection model for this table to <code>newModel</code>
		''' and registers for listener notifications from the new selection model.
		''' </summary>
		''' <param name="newModel">        the new selection model </param>
		''' <exception cref="IllegalArgumentException">      if <code>newModel</code> is <code>null</code> </exception>
		''' <seealso cref=     #getSelectionModel
		''' @beaninfo
		'''      bound: true
		'''      description: The selection model for rows. </seealso>
		Public Overridable Property selectionModel As ListSelectionModel
			Set(ByVal newModel As ListSelectionModel)
				If newModel Is Nothing Then Throw New System.ArgumentException("Cannot set a null SelectionModel")
    
				Dim oldModel As ListSelectionModel = selectionModel
    
				If newModel IsNot oldModel Then
					If oldModel IsNot Nothing Then oldModel.removeListSelectionListener(Me)
    
					selectionModel = newModel
					newModel.addListSelectionListener(Me)
    
					firePropertyChange("selectionModel", oldModel, newModel)
					repaint()
				End If
			End Set
			Get
				Return selectionModel
			End Get
		End Property


	'
	' RowSorterListener
	'

		''' <summary>
		''' <code>RowSorterListener</code> notification that the
		''' <code>RowSorter</code> has changed in some way.
		''' </summary>
		''' <param name="e"> the <code>RowSorterEvent</code> describing the change </param>
		''' <exception cref="NullPointerException"> if <code>e</code> is <code>null</code>
		''' @since 1.6 </exception>
		Public Overridable Sub sorterChanged(ByVal e As RowSorterEvent) Implements RowSorterListener.sorterChanged
			If e.type = RowSorterEvent.Type.SORT_ORDER_CHANGED Then
				Dim header As JTableHeader = tableHeader
				If header IsNot Nothing Then header.repaint()
			ElseIf e.type = RowSorterEvent.Type.SORTED Then
				___sorterChanged = True
				If Not ignoreSortChange Then sortedTableChanged(e, Nothing)
			End If
		End Sub


		''' <summary>
		''' SortManager provides support for managing the selection and variable
		''' row heights when sorting is enabled. This information is encapsulated
		''' into a class to avoid bulking up JTable.
		''' </summary>
		Private NotInheritable Class SortManager
			Private ReadOnly outerInstance As JTable

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend sorter As RowSorter(Of ? As TableModel)

			' Selection, in terms of the model. This is lazily created
			' as needed.
			Private modelSelection As ListSelectionModel
			Private modelLeadIndex As Integer
			' Set to true while in the process of changing the selection.
			' If this is true the selection change is ignored.
			Private syncingSelection As Boolean
			' Temporary cache of selection, in terms of model. This is only used
			' if we don't need the full weight of modelSelection.
			Private lastModelSelection As Integer()

			' Heights of the rows in terms of the model.
			Private modelRowSizes As SizeSequence


			Friend Sub New(ByVal outerInstance As JTable, Of T1 As TableModel)(ByVal sorter As RowSorter(Of T1))
					Me.outerInstance = outerInstance
				Me.sorter = sorter
				sorter.addRowSorterListener(JTable.this)
			End Sub

			''' <summary>
			''' Disposes any resources used by this SortManager.
			''' </summary>
			Public Sub dispose()
				If sorter IsNot Nothing Then sorter.removeRowSorterListener(JTable.this)
			End Sub

			''' <summary>
			''' Sets the height for a row at a specified index.
			''' </summary>
			Public Sub setViewRowHeight(ByVal viewIndex As Integer, ByVal rowHeight As Integer)
				If modelRowSizes Is Nothing Then modelRowSizes = New SizeSequence(outerInstance.model.rowCount, outerInstance.rowHeight)
				modelRowSizes.sizeize(outerInstance.convertRowIndexToModel(viewIndex),rowHeight)
			End Sub

			''' <summary>
			''' Invoked when the underlying model has completely changed.
			''' </summary>
			Public Sub allChanged()
				modelLeadIndex = -1
				modelSelection = Nothing
				modelRowSizes = Nothing
			End Sub

			''' <summary>
			''' Invoked when the selection, on the view, has changed.
			''' </summary>
			Public Sub viewSelectionChanged(ByVal e As ListSelectionEvent)
				If (Not syncingSelection) AndAlso modelSelection IsNot Nothing Then modelSelection = Nothing
			End Sub

			''' <summary>
			''' Invoked when either the table model has changed, or the RowSorter
			''' has changed. This is invoked prior to notifying the sorter of the
			''' change.
			''' </summary>
			Public Sub prepareForChange(ByVal sortEvent As RowSorterEvent, ByVal change As ModelChange)
				If outerInstance.updateSelectionOnSort Then cacheSelection(sortEvent, change)
			End Sub

			''' <summary>
			''' Updates the internal cache of the selection based on the change.
			''' </summary>
			Private Sub cacheSelection(ByVal sortEvent As RowSorterEvent, ByVal change As ModelChange)
				If sortEvent IsNot Nothing Then
					' sort order changed. If modelSelection is null and filtering
					' is enabled we need to cache the selection in terms of the
					' underlying model, this will allow us to correctly restore
					' the selection even if rows are filtered out.
					If modelSelection Is Nothing AndAlso sorter.viewRowCount <> outerInstance.model.rowCount Then
						modelSelection = New DefaultListSelectionModel
						Dim viewSelection As ListSelectionModel = outerInstance.selectionModel
						Dim min As Integer = viewSelection.minSelectionIndex
						Dim max As Integer = viewSelection.maxSelectionIndex
						Dim modelIndex As Integer
						For viewIndex As Integer = min To max
							If viewSelection.isSelectedIndex(viewIndex) Then
								modelIndex = outerInstance.convertRowIndexToModel(sortEvent, viewIndex)
								If modelIndex <> -1 Then modelSelection.addSelectionInterval(modelIndex, modelIndex)
							End If
						Next viewIndex
						modelIndex = outerInstance.convertRowIndexToModel(sortEvent, viewSelection.leadSelectionIndex)
						sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(modelSelection, modelIndex, modelIndex)
					ElseIf modelSelection Is Nothing Then
						' Sorting changed, haven't cached selection in terms
						' of model and no filtering. Temporarily cache selection.
						cacheModelSelection(sortEvent)
					End If
				ElseIf change.allRowsChanged Then
					' All the rows have changed, chuck any cached selection.
					modelSelection = Nothing
				ElseIf modelSelection IsNot Nothing Then
					' Table changed, reflect changes in cached selection model.
					Select Case change.type
					Case TableModelEvent.DELETE
						modelSelection.removeIndexInterval(change.startModelIndex, change.endModelIndex)
					Case TableModelEvent.INSERT
						modelSelection.insertIndexInterval(change.startModelIndex, change.length, True)
					Case Else
					End Select
				Else
					' table changed, but haven't cached rows, temporarily
					' cache them.
					cacheModelSelection(Nothing)
				End If
			End Sub

			Private Sub cacheModelSelection(ByVal sortEvent As RowSorterEvent)
				lastModelSelection = outerInstance.convertSelectionToModel(sortEvent)
				modelLeadIndex = outerInstance.convertRowIndexToModel(sortEvent, outerInstance.selectionModel.leadSelectionIndex)
			End Sub

			''' <summary>
			''' Inovked when either the table has changed or the sorter has changed
			''' and after the sorter has been notified. If necessary this will
			''' reapply the selection and variable row heights.
			''' </summary>
			Public Sub processChange(ByVal sortEvent As RowSorterEvent, ByVal change As ModelChange, ByVal sorterChanged As Boolean)
				If change IsNot Nothing Then
					If change.allRowsChanged Then
						modelRowSizes = Nothing
						outerInstance.rowModel = Nothing
					ElseIf modelRowSizes IsNot Nothing Then
						If change.type = TableModelEvent.INSERT Then
							modelRowSizes.insertEntries(change.startModelIndex, change.endModelIndex - change.startModelIndex + 1, outerInstance.rowHeight)
						ElseIf change.type = TableModelEvent.DELETE Then
							modelRowSizes.removeEntries(change.startModelIndex, change.endModelIndex - change.startModelIndex +1)
						End If
					End If
				End If
				If sorterChanged Then
					viewRowHeightsFromModeldel()
					restoreSelection(change)
				End If
			End Sub

			''' <summary>
			''' Resets the variable row heights in terms of the view from
			''' that of the variable row heights in terms of the model.
			''' </summary>
			Private Sub setViewRowHeightsFromModel()
				If modelRowSizes IsNot Nothing Then
					outerInstance.rowModel.sizeszes(outerInstance.rowCount, outerInstance.rowHeight)
					For viewIndex As Integer = outerInstance.rowCount - 1 To 0 Step -1
						Dim modelIndex As Integer = outerInstance.convertRowIndexToModel(viewIndex)
						outerInstance.rowModel.sizeize(viewIndex, modelRowSizes.getSize(modelIndex))
					Next viewIndex
				End If
			End Sub

			''' <summary>
			''' Restores the selection from that in terms of the model.
			''' </summary>
			Private Sub restoreSelection(ByVal change As ModelChange)
				syncingSelection = True
				If lastModelSelection IsNot Nothing Then
					outerInstance.restoreSortingSelection(lastModelSelection, modelLeadIndex, change)
					lastModelSelection = Nothing
				ElseIf modelSelection IsNot Nothing Then
					Dim viewSelection As ListSelectionModel = outerInstance.selectionModel
					viewSelection.valueIsAdjusting = True
					viewSelection.clearSelection()
					Dim min As Integer = modelSelection.minSelectionIndex
					Dim max As Integer = modelSelection.maxSelectionIndex
					Dim viewIndex As Integer
					For modelIndex As Integer = min To max
						If modelSelection.isSelectedIndex(modelIndex) Then
							viewIndex = outerInstance.convertRowIndexToView(modelIndex)
							If viewIndex <> -1 Then viewSelection.addSelectionInterval(viewIndex, viewIndex)
						End If
					Next modelIndex
					' Restore the lead
					Dim viewLeadIndex As Integer = modelSelection.leadSelectionIndex
					If viewLeadIndex <> -1 AndAlso (Not modelSelection.selectionEmpty) Then viewLeadIndex = outerInstance.convertRowIndexToView(viewLeadIndex)
					sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(viewSelection, viewLeadIndex, viewLeadIndex)
					viewSelection.valueIsAdjusting = False
				End If
				syncingSelection = False
			End Sub
		End Class


		''' <summary>
		''' ModelChange is used when sorting to restore state, it corresponds
		''' to data from a TableModelEvent.  The values are precalculated as
		''' they are used extensively.
		''' </summary>
		Private NotInheritable Class ModelChange
			Private ReadOnly outerInstance As JTable

			' Starting index of the change, in terms of the model
			Friend startModelIndex As Integer

			' Ending index of the change, in terms of the model
			Friend endModelIndex As Integer

			' Type of change
			Friend type As Integer

			' Number of rows in the model
			Friend modelRowCount As Integer

			' The event that triggered this.
			Friend [event] As TableModelEvent

			' Length of the change (end - start + 1)
			Friend length As Integer

			' True if the event indicates all the contents have changed
			Friend allRowsChanged As Boolean

			Friend Sub New(ByVal outerInstance As JTable, ByVal e As TableModelEvent)
					Me.outerInstance = outerInstance
				startModelIndex = Math.Max(0, e.firstRow)
				endModelIndex = e.lastRow
				modelRowCount = outerInstance.model.rowCount
				If endModelIndex < 0 Then endModelIndex = Math.Max(0, modelRowCount - 1)
				length = endModelIndex - startModelIndex + 1
				type = e.type
				[event] = e
				allRowsChanged = (e.lastRow = Integer.MaxValue)
			End Sub
		End Class

		''' <summary>
		''' Invoked when <code>sorterChanged</code> is invoked, or
		''' when <code>tableChanged</code> is invoked and sorting is enabled.
		''' </summary>
		Private Sub sortedTableChanged(ByVal sortedEvent As RowSorterEvent, ByVal e As TableModelEvent)
			Dim editingModelIndex As Integer = -1
			Dim change As ModelChange = If(e IsNot Nothing, New ModelChange(Me, e), Nothing)

			If (change Is Nothing OrElse (Not change.allRowsChanged)) AndAlso Me.editingRow <> -1 Then editingModelIndex = convertRowIndexToModel(sortedEvent, Me.editingRow)

			sortManager.prepareForChange(sortedEvent, change)

			If e IsNot Nothing Then
				If change.type = TableModelEvent.UPDATE Then repaintSortedRows(change)
				notifySorter(change)
				If change.type <> TableModelEvent.UPDATE Then ___sorterChanged = True
			Else
				___sorterChanged = True
			End If

			sortManager.processChange(sortedEvent, change, ___sorterChanged)

			If ___sorterChanged Then
				' Update the editing row
				If Me.editingRow <> -1 Then
					Dim newIndex As Integer = If(editingModelIndex = -1, -1, convertRowIndexToView(editingModelIndex,change))
					restoreSortingEditingRow(newIndex)
				End If

				' And handle the appropriate repainting.
				If e Is Nothing OrElse change.type <> TableModelEvent.UPDATE Then resizeAndRepaint()
			End If

			' Check if lead/anchor need to be reset.
			If change IsNot Nothing AndAlso change.allRowsChanged Then
				clearSelectionAndLeadAnchor()
				resizeAndRepaint()
			End If
		End Sub

		''' <summary>
		''' Repaints the sort of sorted rows in response to a TableModelEvent.
		''' </summary>
		Private Sub repaintSortedRows(ByVal change As ModelChange)
			If change.startModelIndex > change.endModelIndex OrElse change.startModelIndex + 10 < change.endModelIndex Then
				' Too much has changed, punt
				repaint()
				Return
			End If
			Dim eventColumn As Integer = change.event.column
			Dim columnViewIndex As Integer = eventColumn
			If columnViewIndex = TableModelEvent.ALL_COLUMNS Then
				columnViewIndex = 0
			Else
				columnViewIndex = convertColumnIndexToView(columnViewIndex)
				If columnViewIndex = -1 Then Return
			End If
			Dim modelIndex As Integer = change.startModelIndex
			Do While modelIndex <= change.endModelIndex
				Dim viewIndex As Integer = convertRowIndexToView(modelIndex)
				modelIndex += 1
				If viewIndex <> -1 Then
					Dim dirty As Rectangle = getCellRect(viewIndex, columnViewIndex, False)
					Dim ___x As Integer = dirty.x
					Dim w As Integer = dirty.width
					If eventColumn = TableModelEvent.ALL_COLUMNS Then
						___x = 0
						w = width
					End If
					repaint(___x, dirty.y, w, dirty.height)
				End If
			Loop
		End Sub

		''' <summary>
		''' Restores the selection after a model event/sort order changes.
		''' All coordinates are in terms of the model.
		''' </summary>
		Private Sub restoreSortingSelection(ByVal selection As Integer(), ByVal lead As Integer, ByVal change As ModelChange)
			' Convert the selection from model to view
			For i As Integer = selection.Length - 1 To 0 Step -1
				selection(i) = convertRowIndexToView(selection(i), change)
			Next i
			lead = convertRowIndexToView(lead, change)

			' Check for the common case of no change in selection for 1 row
			If selection.Length = 0 OrElse (selection.Length = 1 AndAlso selection(0) = selectedRow) Then Return

			' And apply the new selection
			selectionModel.valueIsAdjusting = True
			selectionModel.clearSelection()
			For i As Integer = selection.Length - 1 To 0 Step -1
				If selection(i) <> -1 Then selectionModel.addSelectionInterval(selection(i), selection(i))
			Next i
			sun.swing.SwingUtilities2.leadAnchorWithoutSelectionion(selectionModel, lead, lead)
			selectionModel.valueIsAdjusting = False
		End Sub

		''' <summary>
		''' Restores the editing row after a model event/sort order change.
		''' </summary>
		''' <param name="editingRow"> new index of the editingRow, in terms of the view </param>
		Private Sub restoreSortingEditingRow(ByVal editingRow As Integer)
			If editingRow = -1 Then
				' Editing row no longer being shown, cancel editing
				Dim editor As TableCellEditor = cellEditor
				If editor IsNot Nothing Then
					' First try and cancel
					editor.cancelCellEditing()
					If cellEditor IsNot Nothing Then removeEditor()
				End If
			Else
				' Repositioning handled in BasicTableUI
				Me.editingRow = editingRow
				repaint()
			End If
		End Sub

		''' <summary>
		''' Notifies the sorter of a change in the underlying model.
		''' </summary>
		Private Sub notifySorter(ByVal change As ModelChange)
			Try
				ignoreSortChange = True
				___sorterChanged = False
				Select Case change.type
				Case TableModelEvent.UPDATE
					If change.event.lastRow = Integer.MaxValue Then
						sortManager.sorter.allRowsChanged()
					ElseIf change.event.column = TableModelEvent.ALL_COLUMNS Then
						sortManager.sorter.rowsUpdated(change.startModelIndex, change.endModelIndex)
					Else
						sortManager.sorter.rowsUpdated(change.startModelIndex, change.endModelIndex, change.event.column)
					End If
				Case TableModelEvent.INSERT
					sortManager.sorter.rowsInserted(change.startModelIndex, change.endModelIndex)
				Case TableModelEvent.DELETE
					sortManager.sorter.rowsDeleted(change.startModelIndex, change.endModelIndex)
				End Select
			Finally
				ignoreSortChange = False
			End Try
		End Sub

		''' <summary>
		''' Converts a model index to view index.  This is called when the
		''' sorter or model changes and sorting is enabled.
		''' </summary>
		''' <param name="change"> describes the TableModelEvent that initiated the change;
		'''        will be null if called as the result of a sort </param>
		Private Function convertRowIndexToView(ByVal modelIndex As Integer, ByVal change As ModelChange) As Integer
			If modelIndex < 0 Then Return -1
			If change IsNot Nothing AndAlso modelIndex >= change.startModelIndex Then
				If change.type = TableModelEvent.INSERT Then
					If modelIndex + change.length >= change.modelRowCount Then Return -1
					Return sortManager.sorter.convertRowIndexToView(modelIndex + change.length)
				ElseIf change.type = TableModelEvent.DELETE Then
					If modelIndex <= change.endModelIndex Then
						' deleted
						Return -1
					Else
						If modelIndex - change.length >= change.modelRowCount Then Return -1
						Return sortManager.sorter.convertRowIndexToView(modelIndex - change.length)
					End If
				End If
				' else, updated
			End If
			If modelIndex >= model.rowCount Then Return -1
			Return sortManager.sorter.convertRowIndexToView(modelIndex)
		End Function

		''' <summary>
		''' Converts the selection to model coordinates.  This is used when
		''' the model changes or the sorter changes.
		''' </summary>
		Private Function convertSelectionToModel(ByVal e As RowSorterEvent) As Integer()
			Dim selection As Integer() = selectedRows
			For i As Integer = selection.Length - 1 To 0 Step -1
				selection(i) = convertRowIndexToModel(e, selection(i))
			Next i
			Return selection
		End Function

		Private Function convertRowIndexToModel(ByVal e As RowSorterEvent, ByVal viewIndex As Integer) As Integer
			If e IsNot Nothing Then
				If e.previousRowCount = 0 Then Return viewIndex
				' range checking handled by RowSorterEvent
				Return e.convertPreviousRowIndexToModel(viewIndex)
			End If
			' Make sure the viewIndex is valid
			If viewIndex < 0 OrElse viewIndex >= rowCount Then Return -1
			Return convertRowIndexToModel(viewIndex)
		End Function

	'
	' Implementing TableModelListener interface
	'

		''' <summary>
		''' Invoked when this table's <code>TableModel</code> generates
		''' a <code>TableModelEvent</code>.
		''' The <code>TableModelEvent</code> should be constructed in the
		''' coordinate system of the model; the appropriate mapping to the
		''' view coordinate system is performed by this <code>JTable</code>
		''' when it receives the event.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by <code>JTable</code>.
		''' <p>
		''' Note that as of 1.3, this method clears the selection, if any.
		''' </summary>
		Public Overridable Sub tableChanged(ByVal e As TableModelEvent) Implements TableModelListener.tableChanged
			If e Is Nothing OrElse e.firstRow = TableModelEvent.HEADER_ROW Then
				' The whole thing changed
				clearSelectionAndLeadAnchor()

				rowModel = Nothing

				If sortManager IsNot Nothing Then
					Try
						ignoreSortChange = True
						sortManager.sorter.modelStructureChanged()
					Finally
						ignoreSortChange = False
					End Try
					sortManager.allChanged()
				End If

				If autoCreateColumnsFromModel Then
					' This will effect invalidation of the JTable and JTableHeader.
					createDefaultColumnsFromModel()
					Return
				End If

				resizeAndRepaint()
				Return
			End If

			If sortManager IsNot Nothing Then
				sortedTableChanged(Nothing, e)
				Return
			End If

			' The totalRowHeight calculated below will be incorrect if
			' there are variable height rows. Repaint the visible region,
			' but don't return as a revalidate may be necessary as well.
			If rowModel IsNot Nothing Then repaint()

			If e.type = TableModelEvent.INSERT Then
				tableRowsInserted(e)
				Return
			End If

			If e.type = TableModelEvent.DELETE Then
				tableRowsDeleted(e)
				Return
			End If

			Dim modelColumn As Integer = e.column
			Dim start As Integer = e.firstRow
			Dim [end] As Integer = e.lastRow

			Dim dirtyRegion As Rectangle
			If modelColumn = TableModelEvent.ALL_COLUMNS Then
				' 1 or more rows changed
				dirtyRegion = New Rectangle(0, start * rowHeight, columnModel.totalColumnWidth, 0)
			Else
				' A cell or column of cells has changed.
				' Unlike the rest of the methods in the JTable, the TableModelEvent
				' uses the coordinate system of the model instead of the view.
				' This is the only place in the JTable where this "reverse mapping"
				' is used.
				Dim ___column As Integer = convertColumnIndexToView(modelColumn)
				dirtyRegion = getCellRect(start, ___column, False)
			End If

			' Now adjust the height of the dirty region according to the value of "end".
			' Check for Integer.MAX_VALUE as this will cause an overflow.
			If [end] <> Integer.MaxValue Then
				dirtyRegion.height = ([end]-start+1)*rowHeight
				repaint(dirtyRegion.x, dirtyRegion.y, dirtyRegion.width, dirtyRegion.height)
			' In fact, if the end is Integer.MAX_VALUE we need to revalidate anyway
			' because the scrollbar may need repainting.
			Else
				clearSelectionAndLeadAnchor()
				resizeAndRepaint()
				rowModel = Nothing
			End If
		End Sub

	'    
	'     * Invoked when rows have been inserted into the table.
	'     * <p>
	'     * Application code will not use these methods explicitly, they
	'     * are used internally by JTable.
	'     *
	'     * @param e the TableModelEvent encapsulating the insertion
	'     
		Private Sub tableRowsInserted(ByVal e As TableModelEvent)
			Dim start As Integer = e.firstRow
			Dim [end] As Integer = e.lastRow
			If start < 0 Then start = 0
			If [end] < 0 Then [end] = rowCount-1

			' Adjust the selection to account for the new rows.
			Dim length As Integer = [end] - start + 1
			selectionModel.insertIndexInterval(start, length, True)

			' If we have variable height rows, adjust the row model.
			If rowModel IsNot Nothing Then rowModel.insertEntries(start, length, rowHeight)
			Dim rh As Integer = rowHeight
			Dim drawRect As New Rectangle(0, start * rh, columnModel.totalColumnWidth, (rowCount-start) * rh)

			revalidate()
			' PENDING(milne) revalidate calls repaint() if parent is a ScrollPane
			' repaint still required in the unusual case where there is no ScrollPane
			repaint(drawRect)
		End Sub

	'    
	'     * Invoked when rows have been removed from the table.
	'     * <p>
	'     * Application code will not use these methods explicitly, they
	'     * are used internally by JTable.
	'     *
	'     * @param e the TableModelEvent encapsulating the deletion
	'     
		Private Sub tableRowsDeleted(ByVal e As TableModelEvent)
			Dim start As Integer = e.firstRow
			Dim [end] As Integer = e.lastRow
			If start < 0 Then start = 0
			If [end] < 0 Then [end] = rowCount-1

			Dim deletedCount As Integer = [end] - start + 1
			Dim previousRowCount As Integer = rowCount + deletedCount
			' Adjust the selection to account for the new rows
			selectionModel.removeIndexInterval(start, [end])

			' If we have variable height rows, adjust the row model.
			If rowModel IsNot Nothing Then rowModel.removeEntries(start, deletedCount)

			Dim rh As Integer = rowHeight
			Dim drawRect As New Rectangle(0, start * rh, columnModel.totalColumnWidth, (previousRowCount - start) * rh)

			revalidate()
			' PENDING(milne) revalidate calls repaint() if parent is a ScrollPane
			' repaint still required in the unusual case where there is no ScrollPane
			repaint(drawRect)
		End Sub

	'
	' Implementing TableColumnModelListener interface
	'

		''' <summary>
		''' Invoked when a column is added to the table column model.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnAdded(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnAdded
			' If I'm currently editing, then I should stop editing
			If editing Then removeEditor()
			resizeAndRepaint()
		End Sub

		''' <summary>
		''' Invoked when a column is removed from the table column model.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnRemoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnRemoved
			' If I'm currently editing, then I should stop editing
			If editing Then removeEditor()
			resizeAndRepaint()
		End Sub

		''' <summary>
		''' Invoked when a column is repositioned. If a cell is being
		''' edited, then editing is stopped and the cell is redrawn.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">   the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnMoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnMoved
			If editing AndAlso (Not cellEditor.stopCellEditing()) Then cellEditor.cancelCellEditing()
			repaint()
		End Sub

		''' <summary>
		''' Invoked when a column is moved due to a margin change.
		''' If a cell is being edited, then editing is stopped and the cell
		''' is redrawn.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">    the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnMarginChanged(ByVal e As ChangeEvent)
			If editing AndAlso (Not cellEditor.stopCellEditing()) Then cellEditor.cancelCellEditing()
			Dim ___resizingColumn As TableColumn = resizingColumn
			' Need to do this here, before the parent's
			' layout manager calls getPreferredSize().
			If ___resizingColumn IsNot Nothing AndAlso autoResizeMode = AUTO_RESIZE_OFF Then ___resizingColumn.preferredWidth = ___resizingColumn.width
			resizeAndRepaint()
		End Sub

		Private Function limit(ByVal i As Integer, ByVal a As Integer, ByVal b As Integer) As Integer
			Return Math.Min(b, Math.Max(i, a))
		End Function

		''' <summary>
		''' Invoked when the selection model of the <code>TableColumnModel</code>
		''' is changed.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= TableColumnModelListener </seealso>
		Public Overridable Sub columnSelectionChanged(ByVal e As ListSelectionEvent)
			Dim isAdjusting As Boolean = e.valueIsAdjusting
			If columnSelectionAdjusting AndAlso (Not isAdjusting) Then
				' The assumption is that when the model is no longer adjusting
				' we will have already gotten all the changes, and therefore
				' don't need to do an additional paint.
				columnSelectionAdjusting = False
				Return
			End If
			columnSelectionAdjusting = isAdjusting
			' The getCellRect() call will fail unless there is at least one row.
			If rowCount <= 0 OrElse columnCount <= 0 Then Return
			Dim firstIndex As Integer = limit(e.firstIndex, 0, columnCount-1)
			Dim lastIndex As Integer = limit(e.lastIndex, 0, columnCount-1)
			Dim minRow As Integer = 0
			Dim maxRow As Integer = rowCount - 1
			If rowSelectionAllowed Then
				minRow = selectionModel.minSelectionIndex
				maxRow = selectionModel.maxSelectionIndex
				Dim leadRow As Integer = getAdjustedIndex(selectionModel.leadSelectionIndex, True)

				If minRow = -1 OrElse maxRow = -1 Then
					If leadRow = -1 Then Return

					' only thing to repaint is the lead
						maxRow = leadRow
						minRow = maxRow
				Else
					' We need to consider more than just the range between
					' the min and max selected index. The lead row, which could
					' be outside this range, should be considered also.
					If leadRow <> -1 Then
						minRow = Math.Min(minRow, leadRow)
						maxRow = Math.Max(maxRow, leadRow)
					End If
				End If
			End If
			Dim firstColumnRect As Rectangle = getCellRect(minRow, firstIndex, False)
			Dim lastColumnRect As Rectangle = getCellRect(maxRow, lastIndex, False)
			Dim dirtyRegion As Rectangle = firstColumnRect.union(lastColumnRect)
			repaint(dirtyRegion)
		End Sub

	'
	' Implementing ListSelectionListener interface
	'

		''' <summary>
		''' Invoked when the row selection changes -- repaints to show the new
		''' selection.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">   the event received </param>
		''' <seealso cref= ListSelectionListener </seealso>
		Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
			If sortManager IsNot Nothing Then sortManager.viewSelectionChanged(e)
			Dim isAdjusting As Boolean = e.valueIsAdjusting
			If rowSelectionAdjusting AndAlso (Not isAdjusting) Then
				' The assumption is that when the model is no longer adjusting
				' we will have already gotten all the changes, and therefore
				' don't need to do an additional paint.
				rowSelectionAdjusting = False
				Return
			End If
			rowSelectionAdjusting = isAdjusting
			' The getCellRect() calls will fail unless there is at least one column.
			If rowCount <= 0 OrElse columnCount <= 0 Then Return
			Dim firstIndex As Integer = limit(e.firstIndex, 0, rowCount-1)
			Dim lastIndex As Integer = limit(e.lastIndex, 0, rowCount-1)
			Dim firstRowRect As Rectangle = getCellRect(firstIndex, 0, False)
			Dim lastRowRect As Rectangle = getCellRect(lastIndex, columnCount-1, False)
			Dim dirtyRegion As Rectangle = firstRowRect.union(lastRowRect)
			repaint(dirtyRegion)
		End Sub

	'
	' Implementing the CellEditorListener interface
	'

		''' <summary>
		''' Invoked when editing is finished. The changes are saved and the
		''' editor is discarded.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= CellEditorListener </seealso>
		Public Overridable Sub editingStopped(ByVal e As ChangeEvent)
			' Take in the new value
			Dim editor As TableCellEditor = cellEditor
			If editor IsNot Nothing Then
				Dim value As Object = editor.cellEditorValue
				valueAteAt(value, editingRow, editingColumn)
				removeEditor()
			End If
		End Sub

		''' <summary>
		''' Invoked when editing is canceled. The editor object is discarded
		''' and the cell is rendered once again.
		''' <p>
		''' Application code will not use these methods explicitly, they
		''' are used internally by JTable.
		''' </summary>
		''' <param name="e">  the event received </param>
		''' <seealso cref= CellEditorListener </seealso>
		Public Overridable Sub editingCanceled(ByVal e As ChangeEvent)
			removeEditor()
		End Sub

	'
	' Implementing the Scrollable interface
	'

		''' <summary>
		''' Sets the preferred size of the viewport for this table.
		''' </summary>
		''' <param name="size">  a <code>Dimension</code> object specifying the <code>preferredSize</code> of a
		'''              <code>JViewport</code> whose view is this table </param>
		''' <seealso cref= Scrollable#getPreferredScrollableViewportSize
		''' @beaninfo
		''' description: The preferred size of the viewport. </seealso>
		Public Overridable Property preferredScrollableViewportSize As Dimension
			Set(ByVal size As Dimension)
				preferredViewportSize = size
			End Set
			Get
				Return preferredViewportSize
			End Get
		End Property


		''' <summary>
		''' Returns the scroll increment (in pixels) that completely exposes one new
		''' row or column (depending on the orientation).
		''' <p>
		''' This method is called each time the user requests a unit scroll.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> either <code>SwingConstants.VERTICAL</code>
		'''                  or <code>SwingConstants.HORIZONTAL</code> </param>
		''' <param name="direction"> less than zero to scroll up/left,
		'''                  greater than zero for down/right </param>
		''' <returns> the "unit" increment for scrolling in the specified direction </returns>
		''' <seealso cref= Scrollable#getScrollableUnitIncrement </seealso>
		Public Overridable Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			Dim ___leadingRow As Integer
			Dim ___leadingCol As Integer
			Dim leadingCellRect As Rectangle

			Dim leadingVisibleEdge As Integer
			Dim leadingCellEdge As Integer
			Dim leadingCellSize As Integer

			___leadingRow = getLeadingRow(visibleRect)
			___leadingCol = getLeadingCol(visibleRect)
			If orientation = SwingConstants.VERTICAL AndAlso ___leadingRow < 0 Then
				' Couldn't find leading row - return some default value
				Return rowHeight
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso ___leadingCol < 0 Then
				' Couldn't find leading col - return some default value
				Return 100
			End If

			' Note that it's possible for one of leadingCol or leadingRow to be
			' -1, depending on the orientation.  This is okay, as getCellRect()
			' still provides enough information to calculate the unit increment.
			leadingCellRect = getCellRect(___leadingRow, ___leadingCol, True)
			leadingVisibleEdge = leadingEdge(visibleRect, orientation)
			leadingCellEdge = leadingEdge(leadingCellRect, orientation)

			If orientation = SwingConstants.VERTICAL Then
				leadingCellSize = leadingCellRect.height

			Else
				leadingCellSize = leadingCellRect.width
			End If

			' 4 cases:
			' #1: Leading cell fully visible, reveal next cell
			' #2: Leading cell fully visible, hide leading cell
			' #3: Leading cell partially visible, hide rest of leading cell
			' #4: Leading cell partially visible, reveal rest of leading cell

			If leadingVisibleEdge = leadingCellEdge Then ' Leading cell is fully
														 ' visible
				' Case #1: Reveal previous cell
				If direction < 0 Then
					Dim retVal As Integer = 0

					If orientation = SwingConstants.VERTICAL Then
						' Loop past any zero-height rows
						___leadingRow -= 1
						Do While ___leadingRow >= 0
							retVal = getRowHeight(___leadingRow)
							If retVal <> 0 Then Exit Do
							___leadingRow -= 1
						Loop
					Else ' HORIZONTAL
						' Loop past any zero-width cols
						___leadingCol -= 1
						Do While ___leadingCol >= 0
							retVal = getCellRect(___leadingRow, ___leadingCol, True).width
							If retVal <> 0 Then Exit Do
							___leadingCol -= 1
						Loop
					End If
					Return retVal
				Else ' Case #2: hide leading cell
					Return leadingCellSize
				End If
			Else ' Leading cell is partially hidden
				' Compute visible, hidden portions
				Dim hiddenAmt As Integer = Math.Abs(leadingVisibleEdge - leadingCellEdge)
				Dim visibleAmt As Integer = leadingCellSize - hiddenAmt

				If direction > 0 Then
					' Case #3: hide showing portion of leading cell
					Return visibleAmt
				Else ' Case #4: reveal hidden portion of leading cell
					Return hiddenAmt
				End If
			End If
		End Function

		''' <summary>
		''' Returns <code>visibleRect.height</code> or
		''' <code>visibleRect.width</code>,
		''' depending on this table's orientation.  Note that as of Swing 1.1.1
		''' (Java 2 v 1.2.2) the value
		''' returned will ensure that the viewport is cleanly aligned on
		''' a row boundary.
		''' </summary>
		''' <returns> <code>visibleRect.height</code> or
		'''                                  <code>visibleRect.width</code>
		'''                                  per the orientation </returns>
		''' <seealso cref= Scrollable#getScrollableBlockIncrement </seealso>
		Public Overridable Function getScrollableBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer

			If rowCount = 0 Then
				' Short-circuit empty table model
				If SwingConstants.VERTICAL = orientation Then
					Dim rh As Integer = rowHeight
					Return If(rh > 0, Math.Max(rh, (visibleRect.height / rh) * rh), visibleRect.height)
				Else
					Return visibleRect.width
				End If
			End If
			' Shortcut for vertical scrolling of a table w/ uniform row height
			If Nothing Is rowModel AndAlso SwingConstants.VERTICAL = orientation Then
				Dim row As Integer = rowAtPoint(visibleRect.location)
				Debug.Assert(row <> -1)
				Dim col As Integer = columnAtPoint(visibleRect.location)
				Dim ___cellRect As Rectangle = getCellRect(row, col, True)

				If ___cellRect.y = visibleRect.y Then
					Dim rh As Integer = rowHeight
					Debug.Assert(rh > 0)
					Return Math.Max(rh, (visibleRect.height / rh) * rh)
				End If
			End If
			If direction < 0 Then
				Return getPreviousBlockIncrement(visibleRect, orientation)
			Else
				Return getNextBlockIncrement(visibleRect, orientation)
			End If
		End Function

		''' <summary>
		''' Called to get the block increment for upward scrolling in cases of
		''' horizontal scrolling, or for vertical scrolling of a table with
		''' variable row heights.
		''' </summary>
		Private Function getPreviousBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer) As Integer
			' Measure back from visible leading edge
			' If we hit the cell on its leading edge, it becomes the leading cell.
			' Else, use following cell

			Dim row As Integer
			Dim col As Integer

			Dim newEdge As Integer
			Dim newCellLoc As Point

			Dim visibleLeadingEdge As Integer = leadingEdge(visibleRect, orientation)
			Dim leftToRight As Boolean = componentOrientation.leftToRight
			Dim newLeadingEdge As Integer

			' Roughly determine the new leading edge by measuring back from the
			' leading visible edge by the size of the visible rect, and find the
			' cell there.
			If orientation = SwingConstants.VERTICAL Then
				newEdge = visibleLeadingEdge - visibleRect.height
				Dim ___x As Integer = visibleRect.x + (If(leftToRight, 0, visibleRect.width))
				newCellLoc = New Point(___x, newEdge)
			ElseIf leftToRight Then
				newEdge = visibleLeadingEdge - visibleRect.width
				newCellLoc = New Point(newEdge, visibleRect.y)
			Else ' Horizontal, right-to-left
				newEdge = visibleLeadingEdge + visibleRect.width
				newCellLoc = New Point(newEdge - 1, visibleRect.y)
			End If
			row = rowAtPoint(newCellLoc)
			col = columnAtPoint(newCellLoc)

			' If we're measuring past the beginning of the table, we get an invalid
			' cell.  Just go to the beginning of the table in this case.
			If orientation = SwingConstants.VERTICAL And row < 0 Then
				newLeadingEdge = 0
			ElseIf orientation = SwingConstants.HORIZONTAL And col < 0 Then
				If leftToRight Then
					newLeadingEdge = 0
				Else
					newLeadingEdge = width
				End If
			Else
				' Refine our measurement
				Dim newCellRect As Rectangle = getCellRect(row, col, True)
				Dim newCellLeadingEdge As Integer = leadingEdge(newCellRect, orientation)
				Dim newCellTrailingEdge As Integer = trailingEdge(newCellRect, orientation)

				' Usually, we hit in the middle of newCell, and want to scroll to
				' the beginning of the cell after newCell.  But there are a
				' couple corner cases where we want to scroll to the beginning of
				' newCell itself.  These cases are:
				' 1) newCell is so large that it ends at or extends into the
				'    visibleRect (newCell is the leading cell, or is adjacent to
				'    the leading cell)
				' 2) newEdge happens to fall right on the beginning of a cell

				' Case 1
				If (orientation = SwingConstants.VERTICAL OrElse leftToRight) AndAlso (newCellTrailingEdge >= visibleLeadingEdge) Then
					newLeadingEdge = newCellLeadingEdge
				ElseIf orientation = SwingConstants.HORIZONTAL AndAlso (Not leftToRight) AndAlso newCellTrailingEdge <= visibleLeadingEdge Then
					newLeadingEdge = newCellLeadingEdge
				' Case 2:
				ElseIf newEdge = newCellLeadingEdge Then
					newLeadingEdge = newCellLeadingEdge
				' Common case: scroll to cell after newCell
				Else
					newLeadingEdge = newCellTrailingEdge
				End If
			End If
			Return Math.Abs(visibleLeadingEdge - newLeadingEdge)
		End Function

		''' <summary>
		''' Called to get the block increment for downward scrolling in cases of
		''' horizontal scrolling, or for vertical scrolling of a table with
		''' variable row heights.
		''' </summary>
		Private Function getNextBlockIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer) As Integer
			' Find the cell at the trailing edge.  Return the distance to put
			' that cell at the leading edge.
			Dim ___trailingRow As Integer = getTrailingRow(visibleRect)
			Dim ___trailingCol As Integer = getTrailingCol(visibleRect)

			Dim ___cellRect As Rectangle
			Dim cellFillsVis As Boolean

			Dim cellLeadingEdge As Integer
			Dim cellTrailingEdge As Integer
			Dim newLeadingEdge As Integer
			Dim visibleLeadingEdge As Integer = leadingEdge(visibleRect, orientation)

			' If we couldn't find trailing cell, just return the size of the
			' visibleRect.  Note that, for instance, we don't need the
			' trailingCol to proceed if we're scrolling vertically, because
			' cellRect will still fill in the required dimensions.  This would
			' happen if we're scrolling vertically, and the table is not wide
			' enough to fill the visibleRect.
			If orientation = SwingConstants.VERTICAL AndAlso ___trailingRow < 0 Then
				Return visibleRect.height
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso ___trailingCol < 0 Then
				Return visibleRect.width
			End If
			___cellRect = getCellRect(___trailingRow, ___trailingCol, True)
			cellLeadingEdge = leadingEdge(___cellRect, orientation)
			cellTrailingEdge = trailingEdge(___cellRect, orientation)

			If orientation = SwingConstants.VERTICAL OrElse componentOrientation.leftToRight Then
				cellFillsVis = cellLeadingEdge <= visibleLeadingEdge
			Else ' Horizontal, right-to-left
				cellFillsVis = cellLeadingEdge >= visibleLeadingEdge
			End If

			If cellFillsVis Then
				' The visibleRect contains a single large cell.  Scroll to the end
				' of this cell, so the following cell is the first cell.
				newLeadingEdge = cellTrailingEdge
			ElseIf cellTrailingEdge = trailingEdge(visibleRect, orientation) Then
				' The trailing cell happens to end right at the end of the
				' visibleRect.  Again, scroll to the beginning of the next cell.
				newLeadingEdge = cellTrailingEdge
			Else
				' Common case: the trailing cell is partially visible, and isn't
				' big enough to take up the entire visibleRect.  Scroll so it
				' becomes the leading cell.
				newLeadingEdge = cellLeadingEdge
			End If
			Return Math.Abs(newLeadingEdge - visibleLeadingEdge)
		End Function

	'    
	'     * Return the row at the top of the visibleRect
	'     *
	'     * May return -1
	'     
		Private Function getLeadingRow(ByVal visibleRect As Rectangle) As Integer
			Dim leadingPoint As Point

			If componentOrientation.leftToRight Then
				leadingPoint = New Point(visibleRect.x, visibleRect.y)
			Else
				leadingPoint = New Point(visibleRect.x + visibleRect.width - 1, visibleRect.y)
			End If
			Return rowAtPoint(leadingPoint)
		End Function

	'    
	'     * Return the column at the leading edge of the visibleRect.
	'     *
	'     * May return -1
	'     
		Private Function getLeadingCol(ByVal visibleRect As Rectangle) As Integer
			Dim leadingPoint As Point

			If componentOrientation.leftToRight Then
				leadingPoint = New Point(visibleRect.x, visibleRect.y)
			Else
				leadingPoint = New Point(visibleRect.x + visibleRect.width - 1, visibleRect.y)
			End If
			Return columnAtPoint(leadingPoint)
		End Function

	'    
	'     * Return the row at the bottom of the visibleRect.
	'     *
	'     * May return -1
	'     
		Private Function getTrailingRow(ByVal visibleRect As Rectangle) As Integer
			Dim trailingPoint As Point

			If componentOrientation.leftToRight Then
				trailingPoint = New Point(visibleRect.x, visibleRect.y + visibleRect.height - 1)
			Else
				trailingPoint = New Point(visibleRect.x + visibleRect.width - 1, visibleRect.y + visibleRect.height - 1)
			End If
			Return rowAtPoint(trailingPoint)
		End Function

	'    
	'     * Return the column at the trailing edge of the visibleRect.
	'     *
	'     * May return -1
	'     
		Private Function getTrailingCol(ByVal visibleRect As Rectangle) As Integer
			Dim trailingPoint As Point

			If componentOrientation.leftToRight Then
				trailingPoint = New Point(visibleRect.x + visibleRect.width - 1, visibleRect.y)
			Else
				trailingPoint = New Point(visibleRect.x, visibleRect.y)
			End If
			Return columnAtPoint(trailingPoint)
		End Function

	'    
	'     * Returns the leading edge ("beginning") of the given Rectangle.
	'     * For VERTICAL, this is the top, for left-to-right, the left side, and for
	'     * right-to-left, the right side.
	'     
		Private Function leadingEdge(ByVal rect As Rectangle, ByVal orientation As Integer) As Integer
			If orientation = SwingConstants.VERTICAL Then
				Return rect.y
			ElseIf componentOrientation.leftToRight Then
				Return rect.x
			Else ' Horizontal, right-to-left
				Return rect.x + rect.width
			End If
		End Function

	'    
	'     * Returns the trailing edge ("end") of the given Rectangle.
	'     * For VERTICAL, this is the bottom, for left-to-right, the right side, and
	'     * for right-to-left, the left side.
	'     
		Private Function trailingEdge(ByVal rect As Rectangle, ByVal orientation As Integer) As Integer
			If orientation = SwingConstants.VERTICAL Then
				Return rect.y + rect.height
			ElseIf componentOrientation.leftToRight Then
				Return rect.x + rect.width
			Else ' Horizontal, right-to-left
				Return rect.x
			End If
		End Function

		''' <summary>
		''' Returns false if <code>autoResizeMode</code> is set to
		''' <code>AUTO_RESIZE_OFF</code>, which indicates that the
		''' width of the viewport does not determine the width
		''' of the table.  Otherwise returns true.
		''' </summary>
		''' <returns> false if <code>autoResizeMode</code> is set
		'''   to <code>AUTO_RESIZE_OFF</code>, otherwise returns true </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportWidth </seealso>
		Public Overridable Property scrollableTracksViewportWidth As Boolean Implements Scrollable.getScrollableTracksViewportWidth
			Get
				Return Not(autoResizeMode = AUTO_RESIZE_OFF)
			End Get
		End Property

		''' <summary>
		''' Returns {@code false} to indicate that the height of the viewport does
		''' not determine the height of the table, unless
		''' {@code getFillsViewportHeight} is {@code true} and the preferred height
		''' of the table is smaller than the viewport's height.
		''' </summary>
		''' <returns> {@code false} unless {@code getFillsViewportHeight} is
		'''         {@code true} and the table needs to be stretched to fill
		'''         the viewport </returns>
		''' <seealso cref= Scrollable#getScrollableTracksViewportHeight </seealso>
		''' <seealso cref= #setFillsViewportHeight </seealso>
		''' <seealso cref= #getFillsViewportHeight </seealso>
		Public Overridable Property scrollableTracksViewportHeight As Boolean Implements Scrollable.getScrollableTracksViewportHeight
			Get
				Dim parent As Container = SwingUtilities.getUnwrappedParent(Me)
				Return fillsViewportHeight AndAlso TypeOf parent Is JViewport AndAlso parent.height > preferredSize.height
			End Get
		End Property

		''' <summary>
		''' Sets whether or not this table is always made large enough
		''' to fill the height of an enclosing viewport. If the preferred
		''' height of the table is smaller than the viewport, then the table
		''' will be stretched to fill the viewport. In other words, this
		''' ensures the table is never smaller than the viewport.
		''' The default for this property is {@code false}.
		''' </summary>
		''' <param name="fillsViewportHeight"> whether or not this table is always
		'''        made large enough to fill the height of an enclosing
		'''        viewport </param>
		''' <seealso cref= #getFillsViewportHeight </seealso>
		''' <seealso cref= #getScrollableTracksViewportHeight
		''' @since 1.6
		''' @beaninfo
		'''      bound: true
		'''      description: Whether or not this table is always made large enough
		'''                   to fill the height of an enclosing viewport </seealso>
		Public Overridable Property fillsViewportHeight As Boolean
			Set(ByVal fillsViewportHeight As Boolean)
				Dim old As Boolean = Me.fillsViewportHeight
				Me.fillsViewportHeight = fillsViewportHeight
				resizeAndRepaint()
				firePropertyChange("fillsViewportHeight", old, fillsViewportHeight)
			End Set
			Get
				Return fillsViewportHeight
			End Get
		End Property


	'
	' Protected Methods
	'

		Protected Friend Overrides Function processKeyBinding(ByVal ks As KeyStroke, ByVal e As KeyEvent, ByVal condition As Integer, ByVal pressed As Boolean) As Boolean
			Dim retValue As Boolean = MyBase.processKeyBinding(ks, e, condition, pressed)

			' Start editing when a key is typed. UI classes can disable this behavior
			' by setting the client property JTable.autoStartsEdit to Boolean.FALSE.
			If (Not retValue) AndAlso condition = WHEN_ANCESTOR_OF_FOCUSED_COMPONENT AndAlso focusOwner AndAlso (Not Boolean.FALSE.Equals(getClientProperty("JTable.autoStartsEdit"))) Then
				' We do not have a binding for the event.
				Dim ___editorComponent As Component = editorComponent
				If ___editorComponent Is Nothing Then
					' Only attempt to install the editor on a KEY_PRESSED,
					If e Is Nothing OrElse e.iD <> KeyEvent.KEY_PRESSED Then Return False
					' Don't start when just a modifier is pressed
					Dim code As Integer = e.keyCode
					If code = KeyEvent.VK_SHIFT OrElse code = KeyEvent.VK_CONTROL OrElse code = KeyEvent.VK_ALT Then Return False
					' Try to install the editor
					Dim leadRow As Integer = selectionModel.leadSelectionIndex
					Dim leadColumn As Integer = columnModel.selectionModel.leadSelectionIndex
					If leadRow <> -1 AndAlso leadColumn <> -1 AndAlso (Not editing) Then
						If Not editCellAt(leadRow, leadColumn, e) Then Return False
					End If
					___editorComponent = editorComponent
					If ___editorComponent Is Nothing Then Return False
				End If
				' If the editorComponent is a JComponent, pass the event to it.
				If TypeOf ___editorComponent Is JComponent Then
					retValue = CType(___editorComponent, JComponent).processKeyBinding(ks, e, WHEN_FOCUSED, pressed)
					' If we have started an editor as a result of the user
					' pressing a key and the surrendersFocusOnKeystroke property
					' is true, give the focus to the new editor.
					If surrendersFocusOnKeystroke Then ___editorComponent.requestFocus()
				End If
			End If
			Return retValue
		End Function

		''' <summary>
		''' Creates default cell renderers for objects, numbers, doubles, dates,
		''' booleans, and icons. </summary>
		''' <seealso cref= javax.swing.table.DefaultTableCellRenderer
		'''  </seealso>
		Protected Friend Overridable Sub createDefaultRenderers()
			defaultRenderersByColumnClass = New UIDefaults(8, 0.75f)

			' Objects
			defaultRenderersByColumnClass(GetType(Object)) = CType(t, UIDefaults.LazyValue) -> New DefaultTableCellRenderer.UIResource

			' Numbers
			defaultRenderersByColumnClass(GetType(Number)) = CType(t, UIDefaults.LazyValue) -> New NumberRenderer

			' Doubles and Floats
			defaultRenderersByColumnClass(GetType(Single?)) = CType(t, UIDefaults.LazyValue) -> New DoubleRenderer
			defaultRenderersByColumnClass(GetType(Double)) = CType(t, UIDefaults.LazyValue) -> New DoubleRenderer

			' Dates
			defaultRenderersByColumnClass(GetType(DateTime)) = CType(t, UIDefaults.LazyValue) -> New DateRenderer

			' Icons and ImageIcons
			defaultRenderersByColumnClass(GetType(Icon)) = CType(t, UIDefaults.LazyValue) -> New IconRenderer
			defaultRenderersByColumnClass(GetType(ImageIcon)) = CType(t, UIDefaults.LazyValue) -> New IconRenderer

			' Booleans
			defaultRenderersByColumnClass(GetType(Boolean)) = CType(t, UIDefaults.LazyValue) -> New BooleanRenderer
		End Sub

		''' <summary>
		''' Default Renderers
		''' 
		''' </summary>
		Friend Class NumberRenderer
			Inherits DefaultTableCellRenderer.UIResource

			Public Sub New()
				MyBase.New()
				horizontalAlignment = JLabel.RIGHT
			End Sub
		End Class

		Friend Class DoubleRenderer
			Inherits NumberRenderer

			Friend formatter As java.text.NumberFormat
			Public Sub New()
				MyBase.New()
			End Sub

			Public Overrides Property value As Object
				Set(ByVal value As Object)
					If formatter Is Nothing Then formatter = java.text.NumberFormat.instance
					text = If(value Is Nothing, "", formatter.format(value))
				End Set
			End Property
		End Class

		Friend Class DateRenderer
			Inherits DefaultTableCellRenderer.UIResource

			Friend formatter As java.text.DateFormat
			Public Sub New()
				MyBase.New()
			End Sub

			Public Overridable Property value As Object
				Set(ByVal value As Object)
					If formatter Is Nothing Then formatter = java.text.DateFormat.dateInstance
					text = If(value Is Nothing, "", formatter.format(value))
				End Set
			End Property
		End Class

		Friend Class IconRenderer
			Inherits DefaultTableCellRenderer.UIResource

			Public Sub New()
				MyBase.New()
				horizontalAlignment = JLabel.CENTER
			End Sub
			Public Overridable Property value As Object
				Set(ByVal value As Object)
					icon = If(TypeOf value Is Icon, CType(value, Icon), Nothing)
				End Set
			End Property
		End Class


		Friend Class BooleanRenderer
			Inherits JCheckBox
			Implements TableCellRenderer, UIResource

			Private Shared ReadOnly noFocusBorder As Border = New EmptyBorder(1, 1, 1, 1)

			Public Sub New()
				MyBase.New()
				horizontalAlignment = JLabel.CENTER
				borderPainted = True
			End Sub

			Public Overridable Function getTableCellRendererComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal hasFocus As Boolean, ByVal row As Integer, ByVal column As Integer) As Component
				If isSelected Then
					foreground = table.selectionForeground
					MyBase.background = table.selectionBackground
				Else
					foreground = table.foreground
					background = table.background
				End If
				selected = (value IsNot Nothing AndAlso CBool(value))

				If hasFocus Then
					border = UIManager.getBorder("Table.focusCellHighlightBorder")
				Else
					border = noFocusBorder
				End If

				Return Me
			End Function
		End Class

		''' <summary>
		''' Creates default cell editors for objects, numbers, and boolean values. </summary>
		''' <seealso cref= DefaultCellEditor </seealso>
		Protected Friend Overridable Sub createDefaultEditors()
			defaultEditorsByColumnClass = New UIDefaults(3, 0.75f)

			' Objects
			defaultEditorsByColumnClass(GetType(Object)) = CType(t, UIDefaults.LazyValue) -> New GenericEditor

			' Numbers
			defaultEditorsByColumnClass(GetType(Number)) = CType(t, UIDefaults.LazyValue) -> New NumberEditor

			' Booleans
			defaultEditorsByColumnClass(GetType(Boolean)) = CType(t, UIDefaults.LazyValue) -> New BooleanEditor
		End Sub

		''' <summary>
		''' Default Editors
		''' </summary>
		Friend Class GenericEditor
			Inherits DefaultCellEditor

			Friend argTypes As Type() = {GetType(String)}
			Friend constructor As System.Reflection.ConstructorInfo
			Friend value As Object

			Public Sub New()
				MyBase.New(New JTextField)
				component.name = "Table.editor"
			End Sub

			Public Overrides Function stopCellEditing() As Boolean
				Dim s As String = CStr(MyBase.cellEditorValue)
				' Here we are dealing with the case where a user
				' has deleted the string value in a cell, possibly
				' after a failed validation. Return null, so that
				' they have the option to replace the value with
				' null or use escape to restore the original.
				' For Strings, return "" for backward compatibility.
				Try
					If "".Equals(s) Then
						If constructor.declaringClass = GetType(String) Then value = s
						Return MyBase.stopCellEditing()
					End If

					sun.swing.SwingUtilities2.checkAccess(constructor.modifiers)
					value = constructor.newInstance(New Object(){s})
				Catch e As Exception
					CType(component, JComponent).border = New LineBorder(Color.red)
					Return False
				End Try
				Return MyBase.stopCellEditing()
			End Function

			Public Overrides Function getTableCellEditorComponent(ByVal table As JTable, ByVal value As Object, ByVal isSelected As Boolean, ByVal row As Integer, ByVal column As Integer) As Component
				Me.value = Nothing
				CType(component, JComponent).border = New LineBorder(Color.black)
				Try
					Dim type As Type = table.getColumnClass(column)
					' Since our obligation is to produce a value which is
					' assignable for the required type it is OK to use the
					' String constructor for columns which are declared
					' to contain Objects. A String is an Object.
					If type Is GetType(Object) Then type = GetType(String)
					sun.reflect.misc.ReflectUtil.checkPackageAccess(type)
					sun.swing.SwingUtilities2.checkAccess(type.modifiers)
					constructor = type.GetConstructor(argTypes)
				Catch e As Exception
					Return Nothing
				End Try
				Return MyBase.getTableCellEditorComponent(table, value, isSelected, row, column)
			End Function

			Public Property Overrides cellEditorValue As Object
				Get
					Return value
				End Get
			End Property
		End Class

		Friend Class NumberEditor
			Inherits GenericEditor

			Public Sub New()
				CType(component, JTextField).horizontalAlignment = JTextField.RIGHT
			End Sub
		End Class

		Friend Class BooleanEditor
			Inherits DefaultCellEditor

			Public Sub New()
				MyBase.New(New JCheckBox)
				Dim checkBox As JCheckBox = CType(component, JCheckBox)
				checkBox.horizontalAlignment = JCheckBox.CENTER
			End Sub
		End Class

		''' <summary>
		''' Initializes table properties to their default values.
		''' </summary>
		Protected Friend Overridable Sub initializeLocalVars()
			updateSelectionOnSort = True
			opaque = True
			createDefaultRenderers()
			createDefaultEditors()

			tableHeader = createDefaultTableHeader()

			showGrid = True
			autoResizeMode = AUTO_RESIZE_SUBSEQUENT_COLUMNS
			rowHeight = 16
			isRowHeightSet = False
			rowMargin = 1
			rowSelectionAllowed = True
			cellEditor = Nothing
			editingColumn = -1
			editingRow = -1
			surrendersFocusOnKeystroke = False
			preferredScrollableViewportSize = New Dimension(450, 400)

			' I'm registered to do tool tips so we can draw tips for the renderers
			Dim ___toolTipManager As ToolTipManager = ToolTipManager.sharedInstance()
			___toolTipManager.registerComponent(Me)

			autoscrolls = True
		End Sub

		''' <summary>
		''' Returns the default table model object, which is
		''' a <code>DefaultTableModel</code>.  A subclass can override this
		''' method to return a different table model object.
		''' </summary>
		''' <returns> the default table model object </returns>
		''' <seealso cref= javax.swing.table.DefaultTableModel </seealso>
		Protected Friend Overridable Function createDefaultDataModel() As TableModel
			Return New DefaultTableModel
		End Function

		''' <summary>
		''' Returns the default column model object, which is
		''' a <code>DefaultTableColumnModel</code>.  A subclass can override this
		''' method to return a different column model object.
		''' </summary>
		''' <returns> the default column model object </returns>
		''' <seealso cref= javax.swing.table.DefaultTableColumnModel </seealso>
		Protected Friend Overridable Function createDefaultColumnModel() As TableColumnModel
			Return New DefaultTableColumnModel
		End Function

		''' <summary>
		''' Returns the default selection model object, which is
		''' a <code>DefaultListSelectionModel</code>.  A subclass can override this
		''' method to return a different selection model object.
		''' </summary>
		''' <returns> the default selection model object </returns>
		''' <seealso cref= javax.swing.DefaultListSelectionModel </seealso>
		Protected Friend Overridable Function createDefaultSelectionModel() As ListSelectionModel
			Return New DefaultListSelectionModel
		End Function

		''' <summary>
		''' Returns the default table header object, which is
		''' a <code>JTableHeader</code>.  A subclass can override this
		''' method to return a different table header object.
		''' </summary>
		''' <returns> the default table header object </returns>
		''' <seealso cref= javax.swing.table.JTableHeader </seealso>
		Protected Friend Overridable Function createDefaultTableHeader() As JTableHeader
			Return New JTableHeader(columnModel)
		End Function

		''' <summary>
		''' Equivalent to <code>revalidate</code> followed by <code>repaint</code>.
		''' </summary>
		Protected Friend Overridable Sub resizeAndRepaint()
			revalidate()
			repaint()
		End Sub

		''' <summary>
		''' Returns the active cell editor, which is {@code null} if the table
		''' is not currently editing.
		''' </summary>
		''' <returns> the {@code TableCellEditor} that does the editing,
		'''         or {@code null} if the table is not currently editing. </returns>
		''' <seealso cref= #cellEditor </seealso>
		''' <seealso cref= #getCellEditor(int, int) </seealso>
		Public Overridable Property cellEditor As TableCellEditor
			Get
				Return cellEditor
			End Get
			Set(ByVal anEditor As TableCellEditor)
				Dim oldEditor As TableCellEditor = cellEditor
				cellEditor = anEditor
				firePropertyChange("tableCellEditor", oldEditor, anEditor)
			End Set
		End Property


		''' <summary>
		''' Sets the <code>editingColumn</code> variable. </summary>
		''' <param name="aColumn">  the column of the cell to be edited
		''' </param>
		''' <seealso cref= #editingColumn </seealso>
		Public Overridable Property editingColumn As Integer
			Set(ByVal aColumn As Integer)
				editingColumn = aColumn
			End Set
		End Property

		''' <summary>
		''' Sets the <code>editingRow</code> variable. </summary>
		''' <param name="aRow">  the row of the cell to be edited
		''' </param>
		''' <seealso cref= #editingRow </seealso>
		Public Overridable Property editingRow As Integer
			Set(ByVal aRow As Integer)
				editingRow = aRow
			End Set
		End Property

		''' <summary>
		''' Returns an appropriate renderer for the cell specified by this row and
		''' column. If the <code>TableColumn</code> for this column has a non-null
		''' renderer, returns that.  If not, finds the class of the data in
		''' this column (using <code>getColumnClass</code>)
		''' and returns the default renderer for this type of data.
		''' <p>
		''' <b>Note:</b>
		''' Throughout the table package, the internal implementations always
		''' use this method to provide renderers so that this default behavior
		''' can be safely overridden by a subclass.
		''' </summary>
		''' <param name="row">       the row of the cell to render, where 0 is the first row </param>
		''' <param name="column">    the column of the cell to render,
		'''                  where 0 is the first column </param>
		''' <returns> the assigned renderer; if <code>null</code>
		'''                  returns the default renderer
		'''                  for this type of object </returns>
		''' <seealso cref= javax.swing.table.DefaultTableCellRenderer </seealso>
		''' <seealso cref= javax.swing.table.TableColumn#setCellRenderer </seealso>
		''' <seealso cref= #setDefaultRenderer </seealso>
		Public Overridable Function getCellRenderer(ByVal row As Integer, ByVal column As Integer) As TableCellRenderer
			Dim ___tableColumn As TableColumn = columnModel.getColumn(column)
			Dim renderer As TableCellRenderer = ___tableColumn.cellRenderer
			If renderer Is Nothing Then renderer = getDefaultRenderer(getColumnClass(column))
			Return renderer
		End Function

		''' <summary>
		''' Prepares the renderer by querying the data model for the
		''' value and selection state
		''' of the cell at <code>row</code>, <code>column</code>.
		''' Returns the component (may be a <code>Component</code>
		''' or a <code>JComponent</code>) under the event location.
		''' <p>
		''' During a printing operation, this method will configure the
		''' renderer without indicating selection or focus, to prevent
		''' them from appearing in the printed output. To do other
		''' customizations based on whether or not the table is being
		''' printed, you can check the value of
		''' <seealso cref="javax.swing.JComponent#isPaintingForPrint()"/>, either here
		''' or within custom renderers.
		''' <p>
		''' <b>Note:</b>
		''' Throughout the table package, the internal implementations always
		''' use this method to prepare renderers so that this default behavior
		''' can be safely overridden by a subclass.
		''' </summary>
		''' <param name="renderer">  the <code>TableCellRenderer</code> to prepare </param>
		''' <param name="row">       the row of the cell to render, where 0 is the first row </param>
		''' <param name="column">    the column of the cell to render,
		'''                  where 0 is the first column </param>
		''' <returns>          the <code>Component</code> under the event location </returns>
		Public Overridable Function prepareRenderer(ByVal renderer As TableCellRenderer, ByVal row As Integer, ByVal column As Integer) As Component
			Dim value As Object = getValueAt(row, column)

			Dim isSelected As Boolean = False
			Dim hasFocus As Boolean = False

			' Only indicate the selection and focused cell if not printing
			If Not paintingForPrint Then
				isSelected = isCellSelected(row, column)

				Dim rowIsLead As Boolean = (selectionModel.leadSelectionIndex = row)
				Dim colIsLead As Boolean = (columnModel.selectionModel.leadSelectionIndex = column)

				hasFocus = (rowIsLead AndAlso colIsLead) AndAlso focusOwner
			End If

			Return renderer.getTableCellRendererComponent(Me, value, isSelected, hasFocus, row, column)
		End Function

		''' <summary>
		''' Returns an appropriate editor for the cell specified by
		''' <code>row</code> and <code>column</code>. If the
		''' <code>TableColumn</code> for this column has a non-null editor,
		''' returns that.  If not, finds the class of the data in this
		''' column (using <code>getColumnClass</code>)
		''' and returns the default editor for this type of data.
		''' <p>
		''' <b>Note:</b>
		''' Throughout the table package, the internal implementations always
		''' use this method to provide editors so that this default behavior
		''' can be safely overridden by a subclass.
		''' </summary>
		''' <param name="row">       the row of the cell to edit, where 0 is the first row </param>
		''' <param name="column">    the column of the cell to edit,
		'''                  where 0 is the first column </param>
		''' <returns>          the editor for this cell;
		'''                  if <code>null</code> return the default editor for
		'''                  this type of cell </returns>
		''' <seealso cref= DefaultCellEditor </seealso>
		Public Overridable Function getCellEditor(ByVal row As Integer, ByVal column As Integer) As TableCellEditor
			Dim ___tableColumn As TableColumn = columnModel.getColumn(column)
			Dim editor As TableCellEditor = ___tableColumn.cellEditor
			If editor Is Nothing Then editor = getDefaultEditor(getColumnClass(column))
			Return editor
		End Function


		''' <summary>
		''' Prepares the editor by querying the data model for the value and
		''' selection state of the cell at <code>row</code>, <code>column</code>.
		''' <p>
		''' <b>Note:</b>
		''' Throughout the table package, the internal implementations always
		''' use this method to prepare editors so that this default behavior
		''' can be safely overridden by a subclass.
		''' </summary>
		''' <param name="editor">  the <code>TableCellEditor</code> to set up </param>
		''' <param name="row">     the row of the cell to edit,
		'''                where 0 is the first row </param>
		''' <param name="column">  the column of the cell to edit,
		'''                where 0 is the first column </param>
		''' <returns> the <code>Component</code> being edited </returns>
		Public Overridable Function prepareEditor(ByVal editor As TableCellEditor, ByVal row As Integer, ByVal column As Integer) As Component
			Dim value As Object = getValueAt(row, column)
			Dim isSelected As Boolean = isCellSelected(row, column)
			Dim comp As Component = editor.getTableCellEditorComponent(Me, value, isSelected, row, column)
			If TypeOf comp Is JComponent Then
				Dim jComp As JComponent = CType(comp, JComponent)
				If jComp.nextFocusableComponent Is Nothing Then jComp.nextFocusableComponent = Me
			End If
			Return comp
		End Function

		''' <summary>
		''' Discards the editor object and frees the real estate it used for
		''' cell rendering.
		''' </summary>
		Public Overridable Sub removeEditor()
			KeyboardFocusManager.currentKeyboardFocusManager.removePropertyChangeListener("permanentFocusOwner", editorRemover)
			editorRemover = Nothing

			Dim editor As TableCellEditor = cellEditor
			If editor IsNot Nothing Then
				editor.removeCellEditorListener(Me)
				If editorComp IsNot Nothing Then
					Dim focusOwner As Component = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
					Dim isFocusOwnerInTheTable As Boolean = If(focusOwner IsNot Nothing, SwingUtilities.isDescendingFrom(focusOwner, Me), False)
					remove(editorComp)
					If isFocusOwnerInTheTable Then requestFocusInWindow()
				End If

				Dim ___cellRect As Rectangle = getCellRect(editingRow, editingColumn, False)

				cellEditor = Nothing
				editingColumn = -1
				editingRow = -1
				editorComp = Nothing

				repaint(___cellRect)
			End If
		End Sub

	'
	' Serialization
	'

		''' <summary>
		''' See readObject() and writeObject() in JComponent for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If (ui IsNot Nothing) AndAlso (uIClassID.Equals(uiClassID)) Then ui.installUI(Me)
			createDefaultRenderers()
			createDefaultEditors()

			' If ToolTipText != null, then the tooltip has already been
			' registered by JComponent.readObject() and we don't want
			' to re-register here
			If toolTipText Is Nothing Then ToolTipManager.sharedInstance().registerComponent(Me)
		End Sub

	'     Called from the JComponent's EnableSerializationFocusListener to
	'     * do any Swing-specific pre-serialization configuration.
	'     
		Friend Overrides Sub compWriteObjectNotify()
			MyBase.compWriteObjectNotify()
			' If ToolTipText != null, then the tooltip has already been
			' unregistered by JComponent.compWriteObjectNotify()
			If toolTipText Is Nothing Then ToolTipManager.sharedInstance().unregisterComponent(Me)
		End Sub

		''' <summary>
		''' Returns a string representation of this table. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this table </returns>
		Protected Friend Overrides Function paramString() As String
			Dim gridColorString As String = (If(gridColor IsNot Nothing, gridColor.ToString(), ""))
			Dim showHorizontalLinesString As String = (If(showHorizontalLines, "true", "false"))
			Dim showVerticalLinesString As String = (If(showVerticalLines, "true", "false"))
			Dim autoResizeModeString As String
			If autoResizeMode = AUTO_RESIZE_OFF Then
				autoResizeModeString = "AUTO_RESIZE_OFF"
			ElseIf autoResizeMode = AUTO_RESIZE_NEXT_COLUMN Then
				autoResizeModeString = "AUTO_RESIZE_NEXT_COLUMN"
			ElseIf autoResizeMode = AUTO_RESIZE_SUBSEQUENT_COLUMNS Then
				autoResizeModeString = "AUTO_RESIZE_SUBSEQUENT_COLUMNS"
			ElseIf autoResizeMode = AUTO_RESIZE_LAST_COLUMN Then
				autoResizeModeString = "AUTO_RESIZE_LAST_COLUMN"
			ElseIf autoResizeMode = AUTO_RESIZE_ALL_COLUMNS Then
				autoResizeModeString = "AUTO_RESIZE_ALL_COLUMNS"
			Else
				autoResizeModeString = ""
			End If
			Dim autoCreateColumnsFromModelString As String = (If(autoCreateColumnsFromModel, "true", "false"))
			Dim preferredViewportSizeString As String = (If(preferredViewportSize IsNot Nothing, preferredViewportSize.ToString(), ""))
			Dim rowSelectionAllowedString As String = (If(rowSelectionAllowed, "true", "false"))
			Dim cellSelectionEnabledString As String = (If(cellSelectionEnabled, "true", "false"))
			Dim selectionForegroundString As String = (If(selectionForeground IsNot Nothing, selectionForeground.ToString(), ""))
			Dim selectionBackgroundString As String = (If(selectionBackground IsNot Nothing, selectionBackground.ToString(), ""))

			Return MyBase.paramString() & ",autoCreateColumnsFromModel=" & autoCreateColumnsFromModelString & ",autoResizeMode=" & autoResizeModeString & ",cellSelectionEnabled=" & cellSelectionEnabledString & ",editingColumn=" & editingColumn & ",editingRow=" & editingRow & ",gridColor=" & gridColorString & ",preferredViewportSize=" & preferredViewportSizeString & ",rowHeight=" & rowHeight & ",rowMargin=" & rowMargin & ",rowSelectionAllowed=" & rowSelectionAllowedString & ",selectionBackground=" & selectionBackgroundString & ",selectionForeground=" & selectionForegroundString & ",showHorizontalLines=" & showHorizontalLinesString & ",showVerticalLines=" & showVerticalLinesString
		End Function

		' This class tracks changes in the keyboard focus state. It is used
		' when the JTable is editing to determine when to cancel the edit.
		' If focus switches to a component outside of the jtable, but in the
		' same window, this will cancel editing.
		Friend Class CellEditorRemover
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As JTable

			Friend ___focusManager As KeyboardFocusManager

			Public Sub New(ByVal outerInstance As JTable, ByVal fm As KeyboardFocusManager)
					Me.outerInstance = outerInstance
				Me.___focusManager = fm
			End Sub

			Public Overridable Sub propertyChange(ByVal ev As PropertyChangeEvent)
				If (Not outerInstance.editing) OrElse outerInstance.getClientProperty("terminateEditOnFocusLost") IsNot Boolean.TRUE Then Return

				Dim c As Component = ___focusManager.permanentFocusOwner
				Do While c IsNot Nothing
					If c Is JTable.this Then
						' focus remains inside the table
						Return
					ElseIf (TypeOf c Is Window) OrElse (TypeOf c Is java.applet.Applet AndAlso c.parent Is Nothing) Then
						If c Is SwingUtilities.getRoot(JTable.this) Then
							If Not outerInstance.cellEditor.stopCellEditing() Then outerInstance.cellEditor.cancelCellEditing()
						End If
						Exit Do
					End If
					c = c.parent
				Loop
			End Sub
		End Class

	'///////////////
	' Printing Support
	'///////////////

		''' <summary>
		''' A convenience method that displays a printing dialog, and then prints
		''' this <code>JTable</code> in mode <code>PrintMode.FIT_WIDTH</code>,
		''' with no header or footer text. A modal progress dialog, with an abort
		''' option, will be shown for the duration of printing.
		''' <p>
		''' Note: In headless mode, no dialogs are shown and printing
		''' occurs on the default printer.
		''' </summary>
		''' <returns> true, unless printing is cancelled by the user </returns>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request </exception>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''                          to be aborted </exception>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean, PrintService) </seealso>
		''' <seealso cref= #getPrintable
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function print() As Boolean

			Return print(PrintMode.FIT_WIDTH)
		End Function

		''' <summary>
		''' A convenience method that displays a printing dialog, and then prints
		''' this <code>JTable</code> in the given printing mode,
		''' with no header or footer text. A modal progress dialog, with an abort
		''' option, will be shown for the duration of printing.
		''' <p>
		''' Note: In headless mode, no dialogs are shown and printing
		''' occurs on the default printer.
		''' </summary>
		''' <param name="printMode">        the printing mode that the printable should use </param>
		''' <returns> true, unless printing is cancelled by the user </returns>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request </exception>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''                          to be aborted </exception>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean, PrintService) </seealso>
		''' <seealso cref= #getPrintable
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function print(ByVal printMode As PrintMode) As Boolean

			Return print(printMode, Nothing, Nothing)
		End Function

		''' <summary>
		''' A convenience method that displays a printing dialog, and then prints
		''' this <code>JTable</code> in the given printing mode,
		''' with the specified header and footer text. A modal progress dialog,
		''' with an abort option, will be shown for the duration of printing.
		''' <p>
		''' Note: In headless mode, no dialogs are shown and printing
		''' occurs on the default printer.
		''' </summary>
		''' <param name="printMode">        the printing mode that the printable should use </param>
		''' <param name="headerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a header,
		'''                          or null for none </param>
		''' <param name="footerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a footer,
		'''                          or null for none </param>
		''' <returns> true, unless printing is cancelled by the user </returns>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request </exception>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''                          to be aborted </exception>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean, PrintService) </seealso>
		''' <seealso cref= #getPrintable
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function print(ByVal printMode As PrintMode, ByVal headerFormat As java.text.MessageFormat, ByVal footerFormat As java.text.MessageFormat) As Boolean

			Dim showDialogs As Boolean = Not GraphicsEnvironment.headless
			Return print(printMode, headerFormat, footerFormat, showDialogs, Nothing, showDialogs)
		End Function

		''' <summary>
		''' Prints this table, as specified by the fully featured
		''' {@link #print(JTable.PrintMode, MessageFormat, MessageFormat,
		''' boolean, PrintRequestAttributeSet, boolean, PrintService) print}
		''' method, with the default printer specified as the print service.
		''' </summary>
		''' <param name="printMode">        the printing mode that the printable should use </param>
		''' <param name="headerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a header,
		'''                          or <code>null</code> for none </param>
		''' <param name="footerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a footer,
		'''                          or <code>null</code> for none </param>
		''' <param name="showPrintDialog">  whether or not to display a print dialog </param>
		''' <param name="attr">             a <code>PrintRequestAttributeSet</code>
		'''                          specifying any printing attributes,
		'''                          or <code>null</code> for none </param>
		''' <param name="interactive">      whether or not to print in an interactive mode </param>
		''' <returns> true, unless printing is cancelled by the user </returns>
		''' <exception cref="HeadlessException"> if the method is asked to show a printing
		'''                           dialog or run interactively, and
		'''                           <code>GraphicsEnvironment.isHeadless</code>
		'''                           returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if this thread is not allowed to
		'''                           initiate a print job request </exception>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''                          to be aborted </exception>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean, PrintService) </seealso>
		''' <seealso cref= #getPrintable
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function print(ByVal printMode As PrintMode, ByVal headerFormat As java.text.MessageFormat, ByVal footerFormat As java.text.MessageFormat, ByVal showPrintDialog As Boolean, ByVal attr As PrintRequestAttributeSet, ByVal interactive As Boolean) As Boolean

			Return print(printMode, headerFormat, footerFormat, showPrintDialog, attr, interactive, Nothing)
		End Function

		''' <summary>
		''' Prints this <code>JTable</code>. Takes steps that the majority of
		''' developers would take in order to print a <code>JTable</code>.
		''' In short, it prepares the table, calls <code>getPrintable</code> to
		''' fetch an appropriate <code>Printable</code>, and then sends it to the
		''' printer.
		''' <p>
		''' A <code>boolean</code> parameter allows you to specify whether or not
		''' a printing dialog is displayed to the user. When it is, the user may
		''' use the dialog to change the destination printer or printing attributes,
		''' or even to cancel the print. Another two parameters allow for a
		''' <code>PrintService</code> and printing attributes to be specified.
		''' These parameters can be used either to provide initial values for the
		''' print dialog, or to specify values when the dialog is not shown.
		''' <p>
		''' A second <code>boolean</code> parameter allows you to specify whether
		''' or not to perform printing in an interactive mode. If <code>true</code>,
		''' a modal progress dialog, with an abort option, is displayed for the
		''' duration of printing . This dialog also prevents any user action which
		''' may affect the table. However, it can not prevent the table from being
		''' modified by code (for example, another thread that posts updates using
		''' <code>SwingUtilities.invokeLater</code>). It is therefore the
		''' responsibility of the developer to ensure that no other code modifies
		''' the table in any way during printing (invalid modifications include
		''' changes in: size, renderers, or underlying data). Printing behavior is
		''' undefined when the table is changed during printing.
		''' <p>
		''' If <code>false</code> is specified for this parameter, no dialog will
		''' be displayed and printing will begin immediately on the event-dispatch
		''' thread. This blocks any other events, including repaints, from being
		''' processed until printing is complete. Although this effectively prevents
		''' the table from being changed, it doesn't provide a good user experience.
		''' For this reason, specifying <code>false</code> is only recommended when
		''' printing from an application with no visible GUI.
		''' <p>
		''' Note: Attempting to show the printing dialog or run interactively, while
		''' in headless mode, will result in a <code>HeadlessException</code>.
		''' <p>
		''' Before fetching the printable, this method will gracefully terminate
		''' editing, if necessary, to prevent an editor from showing in the printed
		''' result. Additionally, <code>JTable</code> will prepare its renderers
		''' during printing such that selection and focus are not indicated.
		''' As far as customizing further how the table looks in the printout,
		''' developers can provide custom renderers or paint code that conditionalize
		''' on the value of <seealso cref="javax.swing.JComponent#isPaintingForPrint()"/>.
		''' <p>
		''' See <seealso cref="#getPrintable"/> for more description on how the table is
		''' printed.
		''' </summary>
		''' <param name="printMode">        the printing mode that the printable should use </param>
		''' <param name="headerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a header,
		'''                          or <code>null</code> for none </param>
		''' <param name="footerFormat">     a <code>MessageFormat</code> specifying the text
		'''                          to be used in printing a footer,
		'''                          or <code>null</code> for none </param>
		''' <param name="showPrintDialog">  whether or not to display a print dialog </param>
		''' <param name="attr">             a <code>PrintRequestAttributeSet</code>
		'''                          specifying any printing attributes,
		'''                          or <code>null</code> for none </param>
		''' <param name="interactive">      whether or not to print in an interactive mode </param>
		''' <param name="service">          the destination <code>PrintService</code>,
		'''                          or <code>null</code> to use the default printer </param>
		''' <returns> true, unless printing is cancelled by the user </returns>
		''' <exception cref="HeadlessException"> if the method is asked to show a printing
		'''                           dialog or run interactively, and
		'''                           <code>GraphicsEnvironment.isHeadless</code>
		'''                           returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''          <seealso cref="java.lang.SecurityManager#checkPrintJobAccess"/>
		'''          method disallows this thread from creating a print job request </exception>
		''' <exception cref="PrinterException"> if an error in the print system causes the job
		'''                          to be aborted </exception>
		''' <seealso cref= #getPrintable </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Function print(ByVal printMode As PrintMode, ByVal headerFormat As java.text.MessageFormat, ByVal footerFormat As java.text.MessageFormat, ByVal showPrintDialog As Boolean, ByVal attr As PrintRequestAttributeSet, ByVal interactive As Boolean, ByVal service As javax.print.PrintService) As Boolean

			' complain early if an invalid parameter is specified for headless mode
			Dim isHeadless As Boolean = GraphicsEnvironment.headless
			If isHeadless Then
				If showPrintDialog Then Throw New HeadlessException("Can't show print dialog.")

				If interactive Then Throw New HeadlessException("Can't run interactively.")
			End If

			' Get a PrinterJob.
			' Do this before anything with side-effects since it may throw a
			' security exception - in which case we don't want to do anything else.
			Dim job As PrinterJob = PrinterJob.printerJob

			If editing Then
				' try to stop cell editing, and failing that, cancel it
				If Not cellEditor.stopCellEditing() Then cellEditor.cancelCellEditing()
			End If

			If attr Is Nothing Then attr = New HashPrintRequestAttributeSet

			Dim printingStatus As sun.swing.PrintingStatus

			 ' fetch the Printable
			Dim ___printable As Printable = getPrintable(printMode, headerFormat, footerFormat)

			If interactive Then
				' wrap the Printable so that we can print on another thread
				___printable = New ThreadSafePrintable(Me, ___printable)
				printingStatus = sun.swing.PrintingStatus.createPrintingStatus(Me, job)
				___printable = printingStatus.createNotificationPrintable(___printable)
			Else
				' to please compiler
				printingStatus = Nothing
			End If

			' set the printable on the PrinterJob
			job.printable = ___printable

			' if specified, set the PrintService on the PrinterJob
			If service IsNot Nothing Then job.printService = service

			' if requested, show the print dialog
			If showPrintDialog AndAlso (Not job.printDialog(attr)) Then Return False

			' if not interactive, just print on this thread (no dialog)
			If Not interactive Then
				' do the printing
				job.print(attr)

				' we're done
				Return True
			End If

			' make sure this is clear since we'll check it after
			printError = Nothing

			' to synchronize on
			Dim lock As New Object

			' copied so we can access from the inner class
			Dim copyAttr As PrintRequestAttributeSet = attr

			' this runnable will be used to do the printing
			' (and save any throwables) on another thread
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Runnable runnable = New Runnable()
	'		{
	'			public void run()
	'			{
	'				try
	'				{
	'					' do the printing
	'					job.print(copyAttr);
	'				}
	'				catch (Throwable t)
	'				{
	'					' save any Throwable to be rethrown
	'					synchronized(lock)
	'					{
	'						printError = t;
	'					}
	'				}
	'				finally
	'				{
	'					' we're finished - hide the dialog
	'					printingStatus.dispose();
	'				}
	'			}
	'		};

			' start printing on another thread
			Dim th As New Thread(runnable)
			th.Start()

			printingStatus.showModal(True)

			' look for any error that the printing may have generated
			Dim pe As Exception
			SyncLock lock
				pe = printError
				printError = Nothing
			End SyncLock

			' check the type of error and handle it
			If pe IsNot Nothing Then
				' a subclass of PrinterException meaning the job was aborted,
				' in this case, by the user
				If TypeOf pe Is PrinterAbortException Then
					Return False
				ElseIf TypeOf pe Is PrinterException Then
					Throw CType(pe, PrinterException)
				ElseIf TypeOf pe Is Exception Then
					Throw CType(pe, Exception)
				ElseIf TypeOf pe Is Exception Then
					Throw CType(pe, [Error])
				End If

				' can not happen
				Throw New AssertionError(pe)
			End If

			Return True
		End Function

		''' <summary>
		''' Return a <code>Printable</code> for use in printing this JTable.
		''' <p>
		''' This method is meant for those wishing to customize the default
		''' <code>Printable</code> implementation used by <code>JTable</code>'s
		''' <code>print</code> methods. Developers wanting simply to print the table
		''' should use one of those methods directly.
		''' <p>
		''' The <code>Printable</code> can be requested in one of two printing modes.
		''' In both modes, it spreads table rows naturally in sequence across
		''' multiple pages, fitting as many rows as possible per page.
		''' <code>PrintMode.NORMAL</code> specifies that the table be
		''' printed at its current size. In this mode, there may be a need to spread
		''' columns across pages in a similar manner to that of the rows. When the
		''' need arises, columns are distributed in an order consistent with the
		''' table's <code>ComponentOrientation</code>.
		''' <code>PrintMode.FIT_WIDTH</code> specifies that the output be
		''' scaled smaller, if necessary, to fit the table's entire width
		''' (and thereby all columns) on each page. Width and height are scaled
		''' equally, maintaining the aspect ratio of the output.
		''' <p>
		''' The <code>Printable</code> heads the portion of table on each page
		''' with the appropriate section from the table's <code>JTableHeader</code>,
		''' if it has one.
		''' <p>
		''' Header and footer text can be added to the output by providing
		''' <code>MessageFormat</code> arguments. The printing code requests
		''' Strings from the formats, providing a single item which may be included
		''' in the formatted string: an <code>Integer</code> representing the current
		''' page number.
		''' <p>
		''' You are encouraged to read the documentation for
		''' <code>MessageFormat</code> as some characters, such as single-quote,
		''' are special and need to be escaped.
		''' <p>
		''' Here's an example of creating a <code>MessageFormat</code> that can be
		''' used to print "Duke's Table: Page - " and the current page number:
		''' 
		''' <pre>
		'''     // notice the escaping of the single quote
		'''     // notice how the page number is included with "{0}"
		'''     MessageFormat format = new MessageFormat("Duke''s Table: Page - {0}");
		''' </pre>
		''' <p>
		''' The <code>Printable</code> constrains what it draws to the printable
		''' area of each page that it prints. Under certain circumstances, it may
		''' find it impossible to fit all of a page's content into that area. In
		''' these cases the output may be clipped, but the implementation
		''' makes an effort to do something reasonable. Here are a few situations
		''' where this is known to occur, and how they may be handled by this
		''' particular implementation:
		''' <ul>
		'''   <li>In any mode, when the header or footer text is too wide to fit
		'''       completely in the printable area -- print as much of the text as
		'''       possible starting from the beginning, as determined by the table's
		'''       <code>ComponentOrientation</code>.
		'''   <li>In any mode, when a row is too tall to fit in the
		'''       printable area -- print the upper-most portion of the row
		'''       and paint no lower border on the table.
		'''   <li>In <code>PrintMode.NORMAL</code> when a column
		'''       is too wide to fit in the printable area -- print the center
		'''       portion of the column and leave the left and right borders
		'''       off the table.
		''' </ul>
		''' <p>
		''' It is entirely valid for this <code>Printable</code> to be wrapped
		''' inside another in order to create complex reports and documents. You may
		''' even request that different pages be rendered into different sized
		''' printable areas. The implementation must be prepared to handle this
		''' (possibly by doing its layout calculations on the fly). However,
		''' providing different heights to each page will likely not work well
		''' with <code>PrintMode.NORMAL</code> when it has to spread columns
		''' across pages.
		''' <p>
		''' As far as customizing how the table looks in the printed result,
		''' <code>JTable</code> itself will take care of hiding the selection
		''' and focus during printing. For additional customizations, your
		''' renderers or painting code can customize the look based on the value
		''' of <seealso cref="javax.swing.JComponent#isPaintingForPrint()"/>
		''' <p>
		''' Also, <i>before</i> calling this method you may wish to <i>first</i>
		''' modify the state of the table, such as to cancel cell editing or
		''' have the user size the table appropriately. However, you must not
		''' modify the state of the table <i>after</i> this <code>Printable</code>
		''' has been fetched (invalid modifications include changes in size or
		''' underlying data). The behavior of the returned <code>Printable</code>
		''' is undefined once the table has been changed.
		''' </summary>
		''' <param name="printMode">     the printing mode that the printable should use </param>
		''' <param name="headerFormat">  a <code>MessageFormat</code> specifying the text to
		'''                       be used in printing a header, or null for none </param>
		''' <param name="footerFormat">  a <code>MessageFormat</code> specifying the text to
		'''                       be used in printing a footer, or null for none </param>
		''' <returns> a <code>Printable</code> for printing this JTable </returns>
		''' <seealso cref= #print(JTable.PrintMode, MessageFormat, MessageFormat,
		'''             boolean, PrintRequestAttributeSet, boolean) </seealso>
		''' <seealso cref= Printable </seealso>
		''' <seealso cref= PrinterJob
		''' 
		''' @since 1.5 </seealso>
		Public Overridable Function getPrintable(ByVal printMode As PrintMode, ByVal headerFormat As java.text.MessageFormat, ByVal footerFormat As java.text.MessageFormat) As Printable

			Return New TablePrintable(Me, printMode, headerFormat, footerFormat)
		End Function


		''' <summary>
		''' A <code>Printable</code> implementation that wraps another
		''' <code>Printable</code>, making it safe for printing on another thread.
		''' </summary>
		Private Class ThreadSafePrintable
			Implements Printable

			Private ReadOnly outerInstance As JTable


			''' <summary>
			''' The delegate <code>Printable</code>. </summary>
			Private printDelegate As Printable

			''' <summary>
			''' To communicate any return value when delegating.
			''' </summary>
			Private retVal As Integer

			''' <summary>
			''' To communicate any <code>Throwable</code> when delegating.
			''' </summary>
			Private retThrowable As Exception

			''' <summary>
			''' Construct a <code>ThreadSafePrintable</code> around the given
			''' delegate.
			''' </summary>
			''' <param name="printDelegate"> the <code>Printable</code> to delegate to </param>
			Public Sub New(ByVal outerInstance As JTable, ByVal printDelegate As Printable)
					Me.outerInstance = outerInstance
				Me.printDelegate = printDelegate
			End Sub

			''' <summary>
			''' Prints the specified page into the given <seealso cref="Graphics"/>
			''' context, in the specified format.
			''' <p>
			''' Regardless of what thread this method is called on, all calls into
			''' the delegate will be done on the event-dispatch thread.
			''' </summary>
			''' <param name="graphics">    the context into which the page is drawn </param>
			''' <param name="pageFormat">  the size and orientation of the page being drawn </param>
			''' <param name="pageIndex">   the zero based index of the page to be drawn </param>
			''' <returns>  PAGE_EXISTS if the page is rendered successfully, or
			'''          NO_SUCH_PAGE if a non-existent page index is specified </returns>
			''' <exception cref="PrinterException"> if an error causes printing to be aborted </exception>
			Public Overridable Function print(ByVal graphics As Graphics, ByVal pageFormat As PageFormat, ByVal pageIndex As Integer) As Integer

				' We'll use this Runnable
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Runnable runnable = New Runnable()
	'			{
	'				public synchronized void run()
	'				{
	'					try
	'					{
	'						' call into the delegate and save the return value
	'						retVal = printDelegate.print(graphics, pageFormat, pageIndex);
	'					}
	'					catch (Throwable throwable)
	'					{
	'						' save any Throwable to be rethrown
	'						retThrowable = throwable;
	'					}
	'					finally
	'					{
	'						' notify the caller that we're done
	'						notifyAll();
	'					}
	'				}
	'			};

				SyncLock runnable
					' make sure these are initialized
					retVal = -1
					retThrowable = Nothing

					' call into the EDT
					SwingUtilities.invokeLater(runnable)

					' wait for the runnable to finish
					Do While retVal = -1 AndAlso retThrowable Is Nothing
						Try
							runnable.wait()
						Catch ie As InterruptedException
							' short process, safe to ignore interrupts
						End Try
					Loop

					' if the delegate threw a throwable, rethrow it here
					If retThrowable IsNot Nothing Then
						If TypeOf retThrowable Is PrinterException Then
							Throw CType(retThrowable, PrinterException)
						ElseIf TypeOf retThrowable Is Exception Then
							Throw CType(retThrowable, Exception)
						ElseIf TypeOf retThrowable Is Exception Then
							Throw CType(retThrowable, [Error])
						End If

						' can not happen
						Throw New AssertionError(retThrowable)
					End If

					Return retVal
				End SyncLock
			End Function
		End Class


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JTable.
		''' For tables, the AccessibleContext takes the form of an
		''' AccessibleJTable.
		''' A new AccessibleJTable instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJTable that serves as the
		'''         AccessibleContext of this JTable </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTable(Me)
				Return accessibleContext
			End Get
		End Property

		'
		' *** should also implement AccessibleSelection?
		' *** and what's up with keyboard navigation/manipulation?
		'
		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTable</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to table user-interface elements.
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
		Protected Friend Class AccessibleJTable
			Inherits AccessibleJComponent
			Implements AccessibleSelection, ListSelectionListener, TableModelListener, TableColumnModelListener, CellEditorListener, PropertyChangeListener, AccessibleExtendedTable

			Private ReadOnly outerInstance As JTable


			Friend previousFocusedRow As Integer
			Friend previousFocusedCol As Integer

			''' <summary>
			''' AccessibleJTable constructor
			''' 
			''' @since 1.5
			''' </summary>
			Protected Friend Sub New(ByVal outerInstance As JTable)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.addPropertyChangeListener(Me)
				outerInstance.selectionModel.addListSelectionListener(Me)
				Dim tcm As TableColumnModel = outerInstance.columnModel
				tcm.addColumnModelListener(Me)
				tcm.selectionModel.addListSelectionListener(Me)
				outerInstance.model.addTableModelListener(Me)
				previousFocusedRow = outerInstance.selectionModel.leadSelectionIndex
				previousFocusedCol = outerInstance.columnModel.selectionModel.leadSelectionIndex
			End Sub

		' Listeners to track model, etc. changes to as to re-place the other
		' listeners

			''' <summary>
			''' Track changes to selection model, column model, etc. so as to
			''' be able to re-place listeners on those in order to pass on
			''' information to the Accessibility PropertyChange mechanism
			''' </summary>
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim name As String = e.propertyName
				Dim oldValue As Object = e.oldValue
				Dim newValue As Object = e.newValue

					' re-set tableModel listeners
				If name.CompareTo("model") = 0 Then

					If oldValue IsNot Nothing AndAlso TypeOf oldValue Is TableModel Then CType(oldValue, TableModel).removeTableModelListener(Me)
					If newValue IsNot Nothing AndAlso TypeOf newValue Is TableModel Then CType(newValue, TableModel).addTableModelListener(Me)

					' re-set selectionModel listeners
				ElseIf name.CompareTo("selectionModel") = 0 Then

					Dim source As Object = e.source
					If source Is JTable.this Then ' row selection model

						If oldValue IsNot Nothing AndAlso TypeOf oldValue Is ListSelectionModel Then CType(oldValue, ListSelectionModel).removeListSelectionListener(Me)
						If newValue IsNot Nothing AndAlso TypeOf newValue Is ListSelectionModel Then CType(newValue, ListSelectionModel).addListSelectionListener(Me)

					ElseIf source Is outerInstance.columnModel Then

						If oldValue IsNot Nothing AndAlso TypeOf oldValue Is ListSelectionModel Then CType(oldValue, ListSelectionModel).removeListSelectionListener(Me)
						If newValue IsNot Nothing AndAlso TypeOf newValue Is ListSelectionModel Then CType(newValue, ListSelectionModel).addListSelectionListener(Me)

					Else
					  '        System.out.println("!!! Bug in source of selectionModel propertyChangeEvent");
					End If

					' re-set columnModel listeners
					' and column's selection property listener as well
				ElseIf name.CompareTo("columnModel") = 0 Then

					If oldValue IsNot Nothing AndAlso TypeOf oldValue Is TableColumnModel Then
						Dim tcm As TableColumnModel = CType(oldValue, TableColumnModel)
						tcm.removeColumnModelListener(Me)
						tcm.selectionModel.removeListSelectionListener(Me)
					End If
					If newValue IsNot Nothing AndAlso TypeOf newValue Is TableColumnModel Then
						Dim tcm As TableColumnModel = CType(newValue, TableColumnModel)
						tcm.addColumnModelListener(Me)
						tcm.selectionModel.addListSelectionListener(Me)
					End If

					' re-se cellEditor listeners
				ElseIf name.CompareTo("tableCellEditor") = 0 Then

					If oldValue IsNot Nothing AndAlso TypeOf oldValue Is TableCellEditor Then CType(oldValue, TableCellEditor).removeCellEditorListener(Me)
					If newValue IsNot Nothing AndAlso TypeOf newValue Is TableCellEditor Then CType(newValue, TableCellEditor).addCellEditorListener(Me)
				End If
			End Sub


		' Listeners to echo changes to the AccessiblePropertyChange mechanism

	'        
	'         * Describes a change in the accessible table model.
	'         
			Protected Friend Class AccessibleJTableModelChange
				Implements AccessibleTableModelChange

				Private ReadOnly outerInstance As JTable.AccessibleJTable


				Protected Friend type As Integer
				Protected Friend firstRow As Integer
				Protected Friend lastRow As Integer
				Protected Friend firstColumn As Integer
				Protected Friend lastColumn As Integer

				Protected Friend Sub New(ByVal outerInstance As JTable.AccessibleJTable, ByVal type As Integer, ByVal firstRow As Integer, ByVal lastRow As Integer, ByVal firstColumn As Integer, ByVal lastColumn As Integer)
						Me.outerInstance = outerInstance
					Me.type = type
					Me.firstRow = firstRow
					Me.lastRow = lastRow
					Me.firstColumn = firstColumn
					Me.lastColumn = lastColumn
				End Sub

				Public Overridable Property type As Integer Implements AccessibleTableModelChange.getType
					Get
						Return type
					End Get
				End Property

				Public Overridable Property firstRow As Integer Implements AccessibleTableModelChange.getFirstRow
					Get
						Return firstRow
					End Get
				End Property

				Public Overridable Property lastRow As Integer Implements AccessibleTableModelChange.getLastRow
					Get
						Return lastRow
					End Get
				End Property

				Public Overridable Property firstColumn As Integer Implements AccessibleTableModelChange.getFirstColumn
					Get
						Return firstColumn
					End Get
				End Property

				Public Overridable Property lastColumn As Integer Implements AccessibleTableModelChange.getLastColumn
					Get
						Return lastColumn
					End Get
				End Property
			End Class

			''' <summary>
			''' Track changes to the table contents
			''' </summary>
			Public Overridable Sub tableChanged(ByVal e As TableModelEvent) Implements TableModelListener.tableChanged
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)
			   If e IsNot Nothing Then
				   Dim firstColumn As Integer = e.column
				   Dim lastColumn As Integer = e.column
				   If firstColumn = TableModelEvent.ALL_COLUMNS Then
					   firstColumn = 0
					   lastColumn = outerInstance.columnCount - 1
				   End If

				   ' Fire a property change event indicating the table model
				   ' has changed.
				   Dim change As New AccessibleJTableModelChange(Me, e.type, e.firstRow, e.lastRow, firstColumn, lastColumn)
				   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)
			   End If
			End Sub

			''' <summary>
			''' Track changes to the table contents (row insertions)
			''' </summary>
			Public Overridable Sub tableRowsInserted(ByVal e As TableModelEvent)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)

			   ' Fire a property change event indicating the table model
			   ' has changed.
			   Dim firstColumn As Integer = e.column
			   Dim lastColumn As Integer = e.column
			   If firstColumn = TableModelEvent.ALL_COLUMNS Then
				   firstColumn = 0
				   lastColumn = outerInstance.columnCount - 1
			   End If
			   Dim change As New AccessibleJTableModelChange(Me, e.type, e.firstRow, e.lastRow, firstColumn, lastColumn)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)
			End Sub

			''' <summary>
			''' Track changes to the table contents (row deletions)
			''' </summary>
			Public Overridable Sub tableRowsDeleted(ByVal e As TableModelEvent)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)

			   ' Fire a property change event indicating the table model
			   ' has changed.
			   Dim firstColumn As Integer = e.column
			   Dim lastColumn As Integer = e.column
			   If firstColumn = TableModelEvent.ALL_COLUMNS Then
				   firstColumn = 0
				   lastColumn = outerInstance.columnCount - 1
			   End If
			   Dim change As New AccessibleJTableModelChange(Me, e.type, e.firstRow, e.lastRow, firstColumn, lastColumn)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)
			End Sub

			''' <summary>
			''' Track changes to the table contents (column insertions)
			''' </summary>
			Public Overridable Sub columnAdded(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnAdded
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)

			   ' Fire a property change event indicating the table model
			   ' has changed.
			   Dim type As Integer = AccessibleTableModelChange.INSERT
			   Dim change As New AccessibleJTableModelChange(Me, type, 0, 0, e.fromIndex, e.toIndex)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)
			End Sub

			''' <summary>
			''' Track changes to the table contents (column deletions)
			''' </summary>
			Public Overridable Sub columnRemoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnRemoved
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)
			   ' Fire a property change event indicating the table model
			   ' has changed.
			   Dim type As Integer = AccessibleTableModelChange.DELETE
			   Dim change As New AccessibleJTableModelChange(Me, type, 0, 0, e.fromIndex, e.toIndex)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)
			End Sub

			''' <summary>
			''' Track changes of a column repositioning.
			''' </summary>
			''' <seealso cref= TableColumnModelListener </seealso>
			Public Overridable Sub columnMoved(ByVal e As TableColumnModelEvent) Implements TableColumnModelListener.columnMoved
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)

			   ' Fire property change events indicating the table model
			   ' has changed.
			   Dim type As Integer = AccessibleTableModelChange.DELETE
			   Dim change As New AccessibleJTableModelChange(Me, type, 0, 0, e.fromIndex, e.fromIndex)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change)

			   Dim type2 As Integer = AccessibleTableModelChange.INSERT
			   Dim change2 As New AccessibleJTableModelChange(Me, type2, 0, 0, e.toIndex, e.toIndex)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_MODEL_CHANGED, Nothing, change2)
			End Sub

			''' <summary>
			''' Track changes of a column moving due to margin changes.
			''' </summary>
			''' <seealso cref= TableColumnModelListener </seealso>
			Public Overridable Sub columnMarginChanged(ByVal e As ChangeEvent)
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)
			End Sub

			''' <summary>
			''' Track that the selection model of the TableColumnModel changed.
			''' </summary>
			''' <seealso cref= TableColumnModelListener </seealso>
			Public Overridable Sub columnSelectionChanged(ByVal e As ListSelectionEvent)
				' we should now re-place our TableColumn listener
			End Sub

			''' <summary>
			''' Track changes to a cell's contents.
			''' 
			''' Invoked when editing is finished. The changes are saved, the
			''' editor object is discarded, and the cell is rendered once again.
			''' </summary>
			''' <seealso cref= CellEditorListener </seealso>
			Public Overridable Sub editingStopped(ByVal e As ChangeEvent)
			   ' it'd be great if we could figure out which cell, and pass that
			   ' somehow as a parameter
			   outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, Nothing)
			End Sub

			''' <summary>
			''' Invoked when editing is canceled. The editor object is discarded
			''' and the cell is rendered once again.
			''' </summary>
			''' <seealso cref= CellEditorListener </seealso>
			Public Overridable Sub editingCanceled(ByVal e As ChangeEvent)
				' nothing to report, 'cause nothing changed
			End Sub

			''' <summary>
			''' Track changes to table cell selections
			''' </summary>
			Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Convert.ToBoolean(False), Convert.ToBoolean(True))

				' Using lead selection index to cover both cases: node selected and node
				' is focused but not selected (Ctrl+up/down)
				Dim focusedRow As Integer = outerInstance.selectionModel.leadSelectionIndex
				Dim focusedCol As Integer = outerInstance.columnModel.selectionModel.leadSelectionIndex

				If focusedRow <> previousFocusedRow OrElse focusedCol <> previousFocusedCol Then
					Dim oldA As Accessible = getAccessibleAt(previousFocusedRow, previousFocusedCol)
					Dim newA As Accessible = getAccessibleAt(focusedRow, focusedCol)
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY, oldA, newA)
					previousFocusedRow = focusedRow
					previousFocusedCol = focusedCol
				End If
			End Sub




		' AccessibleContext support

			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TABLE
				End Get
			End Property

			''' <summary>
			''' Returns the <code>Accessible</code> child, if one exists,
			''' contained at the local coordinate <code>Point</code>.
			''' </summary>
			''' <param name="p"> the point defining the top-left corner of the
			'''    <code>Accessible</code>, given in the coordinate space
			'''    of the object's parent </param>
			''' <returns> the <code>Accessible</code>, if it exists,
			'''    at the specified location; else <code>null</code> </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Dim column As Integer = outerInstance.columnAtPoint(p)
				Dim row As Integer = outerInstance.rowAtPoint(p)

				If (column <> -1) AndAlso (row <> -1) Then
					Dim aColumn As TableColumn = outerInstance.columnModel.getColumn(column)
					Dim renderer As TableCellRenderer = aColumn.cellRenderer
					If renderer Is Nothing Then
						Dim columnClass As Type = outerInstance.getColumnClass(column)
						renderer = outerInstance.getDefaultRenderer(columnClass)
					End If
					Dim component As Component = renderer.getTableCellRendererComponent(JTable.this, Nothing, False, False, row, column)
					Return New AccessibleJTableCell(Me, JTable.this, row, column, getAccessibleIndexAt(row, column))
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement <code>Accessible</code>,
			''' then this method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return (outerInstance.columnCount * outerInstance.rowCount)
				End Get
			End Property

			''' <summary>
			''' Returns the nth <code>Accessible</code> child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				If i < 0 OrElse i >= accessibleChildrenCount Then
					Return Nothing
				Else
					' children increase across, and then down, for tables
					' (arbitrary decision)
					Dim column As Integer = getAccessibleColumnAtIndex(i)
					Dim row As Integer = getAccessibleRowAtIndex(i)

					Dim aColumn As TableColumn = outerInstance.columnModel.getColumn(column)
					Dim renderer As TableCellRenderer = aColumn.cellRenderer
					If renderer Is Nothing Then
						Dim columnClass As Type = outerInstance.getColumnClass(column)
						renderer = outerInstance.getDefaultRenderer(columnClass)
					End If
					Dim component As Component = renderer.getTableCellRendererComponent(JTable.this, Nothing, False, False, row, column)
					Return New AccessibleJTableCell(Me, JTable.this, row, column, getAccessibleIndexAt(row, column))
				End If
			End Function

		' AccessibleSelection support

			''' <summary>
			''' Returns the number of <code>Accessible</code> children
			''' currently selected.
			''' If no children are selected, the return value will be 0.
			''' </summary>
			''' <returns> the number of items currently selected </returns>
			Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				Get
					Dim rowsSel As Integer = outerInstance.selectedRowCount
					Dim colsSel As Integer = outerInstance.selectedColumnCount
    
					If outerInstance.cellSelectionEnabled Then ' a contiguous block
						Return rowsSel * colsSel
    
					Else
						' a column swath and a row swath, with a shared block
						If outerInstance.rowSelectionAllowed AndAlso outerInstance.columnSelectionAllowed Then
							Return rowsSel * outerInstance.columnCount + colsSel * outerInstance.rowCount - rowsSel * colsSel
    
						' just one or more rows in selection
						ElseIf outerInstance.rowSelectionAllowed Then
							Return rowsSel * outerInstance.columnCount
    
						' just one or more rows in selection
						ElseIf outerInstance.columnSelectionAllowed Then
							Return colsSel * outerInstance.rowCount
    
						Else
							Return 0 ' JTable doesn't allow selections
						End If
					End If
				End Get
			End Property

			''' <summary>
			''' Returns an <code>Accessible</code> representing the
			''' specified selected child in the object.  If there
			''' isn't a selection, or there are fewer children selected
			''' than the integer passed in, the return
			''' value will be <code>null</code>.
			''' <p>Note that the index represents the i-th selected child, which
			''' is different from the i-th child.
			''' </summary>
			''' <param name="i"> the zero-based index of selected children </param>
			''' <returns> the i-th selected child </returns>
			''' <seealso cref= #getAccessibleSelectionCount </seealso>
			Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				If i < 0 OrElse i > accessibleSelectionCount Then Return Nothing

				Dim rowsSel As Integer = outerInstance.selectedRowCount
				Dim colsSel As Integer = outerInstance.selectedColumnCount
				Dim rowIndicies As Integer() = outerInstance.selectedRows
				Dim colIndicies As Integer() = outerInstance.selectedColumns
				Dim ttlCols As Integer = outerInstance.columnCount
				Dim ttlRows As Integer = outerInstance.rowCount
				Dim r As Integer
				Dim c As Integer

				If outerInstance.cellSelectionEnabled Then ' a contiguous block
					r = rowIndicies(i \ colsSel)
					c = colIndicies(i Mod colsSel)
					Return getAccessibleChild((r * ttlCols) + c)
				Else

					' a column swath and a row swath, with a shared block
					If outerInstance.rowSelectionAllowed AndAlso outerInstance.columnSelectionAllowed Then

						' Situation:
						'   We have a table, like the 6x3 table below,
						'   wherein three colums and one row selected
						'   (selected cells marked with "*", unselected "0"):
						'
						'            0 * 0 * * 0
						'            * * * * * *
						'            0 * 0 * * 0
						'

						' State machine below walks through the array of
						' selected rows in two states: in a selected row,
						' and not in one; continuing until we are in a row
						' in which the ith selection exists.  Then we return
						' the appropriate cell.  In the state machine, we
						' always do rows above the "current" selected row first,
						' then the cells in the selected row.  If we're done
						' with the state machine before finding the requested
						' selected child, we handle the rows below the last
						' selected row at the end.
						'
						Dim curIndex As Integer = i
						Const IN_ROW As Integer = 0
						Const NOT_IN_ROW As Integer = 1
						Dim state As Integer = (If(rowIndicies(0) = 0, IN_ROW, NOT_IN_ROW))
						Dim j As Integer = 0
						Dim prevRow As Integer = -1
						Do While j < rowIndicies.Length
							Select Case state

							Case IN_ROW ' on individual row full of selections
								If curIndex < ttlCols Then ' it's here!
									c = curIndex Mod ttlCols
									r = rowIndicies(j)
									Return getAccessibleChild((r * ttlCols) + c) ' not here
								Else
									curIndex -= ttlCols
								End If
								' is the next row in table selected or not?
								If j + 1 = rowIndicies.Length OrElse rowIndicies(j) <> rowIndicies(j+1) - 1 Then
									state = NOT_IN_ROW
									prevRow = rowIndicies(j)
								End If
								j += 1 ' we didn't return earlier, so go to next row

							Case NOT_IN_ROW ' sparse bunch of rows of selections
								If curIndex < (colsSel * (rowIndicies(j) - (If(prevRow = -1, 0, (prevRow + 1))))) Then

									' it's here!
									c = colIndicies(curIndex Mod colsSel)
									r = (If(j > 0, rowIndicies(j-1) + 1, 0)) + curIndex \ colsSel
									Return getAccessibleChild((r * ttlCols) + c) ' not here
								Else
									curIndex -= colsSel * (rowIndicies(j) - (If(prevRow = -1, 0, (prevRow + 1))))
								End If
								state = IN_ROW
							End Select
						Loop
						' we got here, so we didn't find it yet; find it in
						' the last sparse bunch of rows
						If curIndex < (colsSel * (ttlRows - (If(prevRow = -1, 0, (prevRow + 1))))) Then ' it's here!
							c = colIndicies(curIndex Mod colsSel)
							r = rowIndicies(j-1) + curIndex \ colsSel + 1
							Return getAccessibleChild((r * ttlCols) + c) ' not here
						Else
							' we shouldn't get to this spot in the code!
	'                      System.out.println("Bug in AccessibleJTable.getAccessibleSelection()");
						End If

					' one or more rows selected
					ElseIf outerInstance.rowSelectionAllowed Then
						c = i Mod ttlCols
						r = rowIndicies(i \ ttlCols)
						Return getAccessibleChild((r * ttlCols) + c)

					' one or more columns selected
					ElseIf outerInstance.columnSelectionAllowed Then
						c = colIndicies(i Mod colsSel)
						r = i \ colsSel
						Return getAccessibleChild((r * ttlCols) + c)
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Determines if the current child of this object is selected.
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this
			'''    <code>Accessible</code> object </param>
			''' <returns> true if the current child of this object is selected </returns>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				Dim column As Integer = getAccessibleColumnAtIndex(i)
				Dim row As Integer = getAccessibleRowAtIndex(i)
				Return outerInstance.isCellSelected(row, column)
			End Function

			''' <summary>
			''' Adds the specified <code>Accessible</code> child of the
			''' object to the object's selection.  If the object supports
			''' multiple selections, the specified child is added to
			''' any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified child is already selected, this method has no effect.
			''' <p>
			''' This method only works on <code>JTable</code>s which have
			''' individual cell selection enabled.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
				' TIGER - 4495286
				Dim column As Integer = getAccessibleColumnAtIndex(i)
				Dim row As Integer = getAccessibleRowAtIndex(i)
				outerInstance.changeSelection(row, column, True, False)
			End Sub

			''' <summary>
			''' Removes the specified child of the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' <p>
			''' This method only works on <code>JTables</code> which have
			''' individual cell selection enabled.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				If outerInstance.cellSelectionEnabled Then
					Dim column As Integer = getAccessibleColumnAtIndex(i)
					Dim row As Integer = getAccessibleRowAtIndex(i)
					outerInstance.removeRowSelectionInterval(row, row)
					outerInstance.removeColumnSelectionInterval(column, column)
				End If
			End Sub

			''' <summary>
			''' Clears the selection in the object, so that no children in the
			''' object are selected.
			''' </summary>
			Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
				outerInstance.clearSelection()
			End Sub

			''' <summary>
			''' Causes every child of the object to be selected, but only
			''' if the <code>JTable</code> supports multiple selections,
			''' and if individual cell selection is enabled.
			''' </summary>
			Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
				If outerInstance.cellSelectionEnabled Then outerInstance.selectAll()
			End Sub

			' begin AccessibleExtendedTable implementation -------------

			''' <summary>
			''' Returns the row number of an index in the table.
			''' </summary>
			''' <param name="index"> the zero-based index in the table </param>
			''' <returns> the zero-based row of the table if one exists;
			''' otherwise -1.
			''' @since 1.4 </returns>
			Public Overridable Function getAccessibleRow(ByVal index As Integer) As Integer Implements AccessibleExtendedTable.getAccessibleRow
				Return getAccessibleRowAtIndex(index)
			End Function

			''' <summary>
			''' Returns the column number of an index in the table.
			''' </summary>
			''' <param name="index"> the zero-based index in the table </param>
			''' <returns> the zero-based column of the table if one exists;
			''' otherwise -1.
			''' @since 1.4 </returns>
			Public Overridable Function getAccessibleColumn(ByVal index As Integer) As Integer Implements AccessibleExtendedTable.getAccessibleColumn
				Return getAccessibleColumnAtIndex(index)
			End Function

			''' <summary>
			''' Returns the index at a row and column in the table.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <param name="c"> zero-based column of the table </param>
			''' <returns> the zero-based index in the table if one exists;
			''' otherwise -1.
			''' @since 1.4 </returns>
			Public Overridable Function getAccessibleIndex(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleExtendedTable.getAccessibleIndex
				Return getAccessibleIndexAt(r, c)
			End Function

			' end of AccessibleExtendedTable implementation ------------

			' start of AccessibleTable implementation ------------------

			Private caption As Accessible
			Private summary As Accessible
			Private rowDescription As Accessible()
			Private columnDescription As Accessible()

			''' <summary>
			''' Gets the <code>AccessibleTable</code> associated with this
			''' object.  In the implementation of the Java Accessibility
			''' API for this class, return this object, which is responsible
			''' for implementing the <code>AccessibleTables</code> interface
			''' on behalf of itself.
			''' </summary>
			''' <returns> this object
			''' @since 1.3 </returns>
			Public Overridable Property accessibleTable As AccessibleTable
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the caption for the table.
			''' </summary>
			''' <returns> the caption for the table
			''' @since 1.3 </returns>
			Public Overridable Property accessibleCaption As Accessible Implements AccessibleTable.getAccessibleCaption
				Get
					Return Me.caption
				End Get
				Set(ByVal a As Accessible)
					Dim oldCaption As Accessible = caption
					Me.caption = a
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_CAPTION_CHANGED, oldCaption, Me.caption)
				End Set
			End Property


			''' <summary>
			''' Returns the summary description of the table.
			''' </summary>
			''' <returns> the summary description of the table
			''' @since 1.3 </returns>
			Public Overridable Property accessibleSummary As Accessible Implements AccessibleTable.getAccessibleSummary
				Get
					Return Me.summary
				End Get
				Set(ByVal a As Accessible)
					Dim oldSummary As Accessible = summary
					Me.summary = a
					outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_TABLE_SUMMARY_CHANGED, oldSummary, Me.summary)
				End Set
			End Property


	'        
	'         * Returns the total number of rows in this table.
	'         *
	'         * @return the total number of rows in this table
	'         
			Public Overridable Property accessibleRowCount As Integer Implements AccessibleTable.getAccessibleRowCount
				Get
					Return outerInstance.rowCount
				End Get
			End Property

	'        
	'         * Returns the total number of columns in the table.
	'         *
	'         * @return the total number of columns in the table
	'         
			Public Overridable Property accessibleColumnCount As Integer Implements AccessibleTable.getAccessibleColumnCount
				Get
					Return outerInstance.columnCount
				End Get
			End Property

	'        
	'         * Returns the <code>Accessible</code> at a specified row
	'         * and column in the table.
	'         *
	'         * @param r zero-based row of the table
	'         * @param c zero-based column of the table
	'         * @return the <code>Accessible</code> at the specified row and column
	'         * in the table
	'         
			Public Overridable Function getAccessibleAt(ByVal r As Integer, ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleAt
				Return getAccessibleChild((r * accessibleColumnCount) + c)
			End Function

			''' <summary>
			''' Returns the number of rows occupied by the <code>Accessible</code>
			''' at a specified row and column in the table.
			''' </summary>
			''' <returns> the number of rows occupied by the <code>Accessible</code>
			'''     at a specified row and column in the table
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleRowExtentAt
				Return 1
			End Function

			''' <summary>
			''' Returns the number of columns occupied by the
			''' <code>Accessible</code> at a given (row, column).
			''' </summary>
			''' <returns> the number of columns occupied by the <code>Accessible</code>
			'''     at a specified row and column in the table
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleColumnExtentAt
				Return 1
			End Function

			''' <summary>
			''' Returns the row headers as an <code>AccessibleTable</code>.
			''' </summary>
			''' <returns> an <code>AccessibleTable</code> representing the row
			''' headers
			''' @since 1.3 </returns>
			Public Overridable Property accessibleRowHeader As AccessibleTable Implements AccessibleTable.getAccessibleRowHeader
				Get
					' row headers are not supported
					Return Nothing
				End Get
				Set(ByVal a As AccessibleTable)
					' row headers are not supported
				End Set
			End Property


			''' <summary>
			''' Returns the column headers as an <code>AccessibleTable</code>.
			''' </summary>
			'''  <returns> an <code>AccessibleTable</code> representing the column
			'''          headers, or <code>null</code> if the table header is
			'''          <code>null</code>
			''' @since 1.3 </returns>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overridable Function getAccessibleColumnHeader() As AccessibleTable Implements AccessibleTable.getAccessibleColumnHeader 'JavaToDotNetTempPropertyGetaccessibleColumnHeader
			Public Overridable Property accessibleColumnHeader As AccessibleTable Implements AccessibleTable.getAccessibleColumnHeader
				Get
					Dim header As JTableHeader = outerInstance.tableHeader
					Return If(header Is Nothing, Nothing, New AccessibleTableHeader(Me, header))
				End Get
				Set(ByVal table As AccessibleTable)
			End Property

	'        
	'         * Private class representing a table column header
	'         
			Private Class AccessibleTableHeader
				Implements AccessibleTable

				Private ReadOnly outerInstance As JTable.AccessibleJTable

				Private header As JTableHeader
				Private headerModel As TableColumnModel

				Friend Sub New(ByVal outerInstance As JTable.AccessibleJTable, ByVal header As JTableHeader)
						Me.outerInstance = outerInstance
					Me.header = header
					Me.headerModel = header.columnModel
				End Sub

				''' <summary>
				''' Returns the caption for the table.
				''' </summary>
				''' <returns> the caption for the table </returns>
				Public Overridable Property accessibleCaption As Accessible Implements AccessibleTable.getAccessibleCaption
					Get
						Return Nothing
					End Get
					Set(ByVal a As Accessible)
					End Set
				End Property



				''' <summary>
				''' Returns the summary description of the table.
				''' </summary>
				''' <returns> the summary description of the table </returns>
				Public Overridable Property accessibleSummary As Accessible Implements AccessibleTable.getAccessibleSummary
					Get
						Return Nothing
					End Get
					Set(ByVal a As Accessible)
					End Set
				End Property


				''' <summary>
				''' Returns the number of rows in the table.
				''' </summary>
				''' <returns> the number of rows in the table </returns>
				Public Overridable Property accessibleRowCount As Integer Implements AccessibleTable.getAccessibleRowCount
					Get
						Return 1
					End Get
				End Property

				''' <summary>
				''' Returns the number of columns in the table.
				''' </summary>
				''' <returns> the number of columns in the table </returns>
				Public Overridable Property accessibleColumnCount As Integer Implements AccessibleTable.getAccessibleColumnCount
					Get
						Return headerModel.columnCount
					End Get
				End Property

				''' <summary>
				''' Returns the Accessible at a specified row and column
				''' in the table.
				''' </summary>
				''' <param name="row"> zero-based row of the table </param>
				''' <param name="column"> zero-based column of the table </param>
				''' <returns> the Accessible at the specified row and column </returns>
				Public Overridable Function getAccessibleAt(ByVal row As Integer, ByVal column As Integer) As Accessible Implements AccessibleTable.getAccessibleAt


					' TIGER - 4715503
					Dim aColumn As TableColumn = headerModel.getColumn(column)
					Dim renderer As TableCellRenderer = aColumn.headerRenderer
					If renderer Is Nothing Then renderer = header.defaultRenderer
					Dim component As Component = renderer.getTableCellRendererComponent(header.table, aColumn.headerValue, False, False, -1, column)

					Return New AccessibleJTableHeaderCell(row, column, outerInstance.tableHeader, component)
				End Function

				''' <summary>
				''' Returns the number of rows occupied by the Accessible at
				''' a specified row and column in the table.
				''' </summary>
				''' <returns> the number of rows occupied by the Accessible at a
				''' given specified (row, column) </returns>
				Public Overridable Function getAccessibleRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleRowExtentAt
					Return 1
				End Function

				''' <summary>
				''' Returns the number of columns occupied by the Accessible at
				''' a specified row and column in the table.
				''' </summary>
				''' <returns> the number of columns occupied by the Accessible at a
				''' given specified row and column </returns>
				Public Overridable Function getAccessibleColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer Implements AccessibleTable.getAccessibleColumnExtentAt
					Return 1
				End Function

				''' <summary>
				''' Returns the row headers as an AccessibleTable.
				''' </summary>
				''' <returns> an AccessibleTable representing the row
				''' headers </returns>
				Public Overridable Property accessibleRowHeader As AccessibleTable Implements AccessibleTable.getAccessibleRowHeader
					Get
						Return Nothing
					End Get
					Set(ByVal table As AccessibleTable)
					End Set
				End Property


				''' <summary>
				''' Returns the column headers as an AccessibleTable.
				''' </summary>
				''' <returns> an AccessibleTable representing the column
				''' headers </returns>
				Public Overridable Property accessibleColumnHeader As AccessibleTable Implements AccessibleTable.getAccessibleColumnHeader
					Get
						Return Nothing
					End Get
				End Property

				End Sub

				''' <summary>
				''' Returns the description of the specified row in the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <returns> the description of the row
				''' @since 1.3 </returns>
				Public Overridable Function getAccessibleRowDescription(ByVal r As Integer) As Accessible Implements AccessibleTable.getAccessibleRowDescription
					Return Nothing
				End Function

				''' <summary>
				''' Sets the description text of the specified row of the table.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="a"> the description of the row
				''' @since 1.3 </param>
				Public Overridable Sub setAccessibleRowDescription(ByVal r As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleRowDescription
				End Sub

				''' <summary>
				''' Returns the description text of the specified column in the table.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the text description of the column
				''' @since 1.3 </returns>
				Public Overridable Function getAccessibleColumnDescription(ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleColumnDescription
					Return Nothing
				End Function

				''' <summary>
				''' Sets the description text of the specified column in the table.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <param name="a"> the text description of the column
				''' @since 1.3 </param>
				Public Overridable Sub setAccessibleColumnDescription(ByVal c As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleColumnDescription
				End Sub

				''' <summary>
				''' Returns a boolean value indicating whether the accessible at
				''' a specified row and column is selected.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the boolean value true if the accessible at the
				''' row and column is selected. Otherwise, the boolean value
				''' false
				''' @since 1.3 </returns>
				Public Overridable Function isAccessibleSelected(ByVal r As Integer, ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleSelected
					Return False
				End Function

				''' <summary>
				''' Returns a boolean value indicating whether the specified row
				''' is selected.
				''' </summary>
				''' <param name="r"> zero-based row of the table </param>
				''' <returns> the boolean value true if the specified row is selected.
				''' Otherwise, false.
				''' @since 1.3 </returns>
				Public Overridable Function isAccessibleRowSelected(ByVal r As Integer) As Boolean Implements AccessibleTable.isAccessibleRowSelected
					Return False
				End Function

				''' <summary>
				''' Returns a boolean value indicating whether the specified column
				''' is selected.
				''' </summary>
				''' <param name="c"> zero-based column of the table </param>
				''' <returns> the boolean value true if the specified column is selected.
				''' Otherwise, false.
				''' @since 1.3 </returns>
				Public Overridable Function isAccessibleColumnSelected(ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleColumnSelected
					Return False
				End Function

				''' <summary>
				''' Returns the selected rows in a table.
				''' </summary>
				''' <returns> an array of selected rows where each element is a
				''' zero-based row of the table
				''' @since 1.3 </returns>
				Public Overridable Property selectedAccessibleRows As Integer()
					Get
						Return New Integer(){}
					End Get
				End Property

				''' <summary>
				''' Returns the selected columns in a table.
				''' </summary>
				''' <returns> an array of selected columns where each element is a
				''' zero-based column of the table
				''' @since 1.3 </returns>
				Public Overridable Property selectedAccessibleColumns As Integer()
					Get
						Return New Integer(){}
					End Get
				End Property
			End Class


			''' <summary>
			''' Sets the column headers as an <code>AccessibleTable</code>.
			''' </summary>
			''' <param name="a"> an <code>AccessibleTable</code> representing the
			''' column headers
			''' @since 1.3 </param>
			Public Overridable Property accessibleColumnHeader Implements AccessibleTable.setAccessibleColumnHeader As AccessibleTable
				Set(ByVal a As AccessibleTable)
					' XXX not implemented
				End Set
			End Property

			''' <summary>
			''' Returns the description of the specified row in the table.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <returns> the description of the row
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleRowDescription(ByVal r As Integer) As Accessible Implements AccessibleTable.getAccessibleRowDescription
				If r < 0 OrElse r >= accessibleRowCount Then Throw New System.ArgumentException(Convert.ToString(r))
				If rowDescription Is Nothing Then
					Return Nothing
				Else
					Return rowDescription(r)
				End If
			End Function

			''' <summary>
			''' Sets the description text of the specified row of the table.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <param name="a"> the description of the row
			''' @since 1.3 </param>
			Public Overridable Sub setAccessibleRowDescription(ByVal r As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleRowDescription
				If r < 0 OrElse r >= accessibleRowCount Then Throw New System.ArgumentException(Convert.ToString(r))
				If rowDescription Is Nothing Then
					Dim numRows As Integer = accessibleRowCount
					rowDescription = New Accessible(numRows - 1){}
				End If
				rowDescription(r) = a
			End Sub

			''' <summary>
			''' Returns the description of the specified column in the table.
			''' </summary>
			''' <param name="c"> zero-based column of the table </param>
			''' <returns> the description of the column
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleColumnDescription(ByVal c As Integer) As Accessible Implements AccessibleTable.getAccessibleColumnDescription
				If c < 0 OrElse c >= accessibleColumnCount Then Throw New System.ArgumentException(Convert.ToString(c))
				If columnDescription Is Nothing Then
					Return Nothing
				Else
					Return columnDescription(c)
				End If
			End Function

			''' <summary>
			''' Sets the description text of the specified column of the table.
			''' </summary>
			''' <param name="c"> zero-based column of the table </param>
			''' <param name="a"> the description of the column
			''' @since 1.3 </param>
			Public Overridable Sub setAccessibleColumnDescription(ByVal c As Integer, ByVal a As Accessible) Implements AccessibleTable.setAccessibleColumnDescription
				If c < 0 OrElse c >= accessibleColumnCount Then Throw New System.ArgumentException(Convert.ToString(c))
				If columnDescription Is Nothing Then
					Dim numColumns As Integer = accessibleColumnCount
					columnDescription = New Accessible(numColumns - 1){}
				End If
				columnDescription(c) = a
			End Sub

			''' <summary>
			''' Returns a boolean value indicating whether the accessible at a
			''' given (row, column) is selected.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <param name="c"> zero-based column of the table </param>
			''' <returns> the boolean value true if the accessible at (row, column)
			'''     is selected; otherwise, the boolean value false
			''' @since 1.3 </returns>
			Public Overridable Function isAccessibleSelected(ByVal r As Integer, ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleSelected
				Return outerInstance.isCellSelected(r, c)
			End Function

			''' <summary>
			''' Returns a boolean value indicating whether the specified row
			''' is selected.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <returns> the boolean value true if the specified row is selected;
			'''     otherwise, false
			''' @since 1.3 </returns>
			Public Overridable Function isAccessibleRowSelected(ByVal r As Integer) As Boolean Implements AccessibleTable.isAccessibleRowSelected
				Return outerInstance.isRowSelected(r)
			End Function

			''' <summary>
			''' Returns a boolean value indicating whether the specified column
			''' is selected.
			''' </summary>
			''' <param name="c"> zero-based column of the table </param>
			''' <returns> the boolean value true if the specified column is selected;
			'''     otherwise, false
			''' @since 1.3 </returns>
			Public Overridable Function isAccessibleColumnSelected(ByVal c As Integer) As Boolean Implements AccessibleTable.isAccessibleColumnSelected
				Return outerInstance.isColumnSelected(c)
			End Function

			''' <summary>
			''' Returns the selected rows in a table.
			''' </summary>
			''' <returns> an array of selected rows where each element is a
			'''     zero-based row of the table
			''' @since 1.3 </returns>
			Public Overridable Property selectedAccessibleRows As Integer()
				Get
					Return outerInstance.selectedRows
				End Get
			End Property

			''' <summary>
			''' Returns the selected columns in a table.
			''' </summary>
			''' <returns> an array of selected columns where each element is a
			'''     zero-based column of the table
			''' @since 1.3 </returns>
			Public Overridable Property selectedAccessibleColumns As Integer()
				Get
					Return outerInstance.selectedColumns
				End Get
			End Property

			''' <summary>
			''' Returns the row at a given index into the table.
			''' </summary>
			''' <param name="i"> zero-based index into the table </param>
			''' <returns> the row at a given index
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleRowAtIndex(ByVal i As Integer) As Integer
				Dim columnCount As Integer = accessibleColumnCount
				If columnCount = 0 Then
					Return -1
				Else
					Return (i \ columnCount)
				End If
			End Function

			''' <summary>
			''' Returns the column at a given index into the table.
			''' </summary>
			''' <param name="i"> zero-based index into the table </param>
			''' <returns> the column at a given index
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleColumnAtIndex(ByVal i As Integer) As Integer
				Dim columnCount As Integer = accessibleColumnCount
				If columnCount = 0 Then
					Return -1
				Else
					Return (i Mod columnCount)
				End If
			End Function

			''' <summary>
			''' Returns the index at a given (row, column) in the table.
			''' </summary>
			''' <param name="r"> zero-based row of the table </param>
			''' <param name="c"> zero-based column of the table </param>
			''' <returns> the index into the table
			''' @since 1.3 </returns>
			Public Overridable Function getAccessibleIndexAt(ByVal r As Integer, ByVal c As Integer) As Integer
				Return ((r * accessibleColumnCount) + c)
			End Function

			' end of AccessibleTable implementation --------------------

			''' <summary>
			''' The class provides an implementation of the Java Accessibility
			''' API appropriate to table cells.
			''' </summary>
			Protected Friend Class AccessibleJTableCell
				Inherits AccessibleContext
				Implements Accessible, AccessibleComponent

				Private ReadOnly outerInstance As JTable.AccessibleJTable


				Private parent As JTable
				Private row As Integer
				Private column As Integer
				Private index As Integer

				''' <summary>
				'''  Constructs an <code>AccessibleJTableHeaderEntry</code>.
				''' @since 1.4
				''' </summary>
				Public Sub New(ByVal outerInstance As JTable.AccessibleJTable, ByVal t As JTable, ByVal r As Integer, ByVal c As Integer, ByVal i As Integer)
						Me.outerInstance = outerInstance
					parent = t
					row = r
					column = c
					index = i
					Me.accessibleParent = parent
				End Sub

				''' <summary>
				''' Gets the <code>AccessibleContext</code> associated with this
				''' component. In the implementation of the Java Accessibility
				''' API for this class, return this object, which is its own
				''' <code>AccessibleContext</code>.
				''' </summary>
				''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						Return Me
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleContext for the table cell renderer.
				''' </summary>
				''' <returns> the <code>AccessibleContext</code> for the table
				''' cell renderer if one exists;
				''' otherwise, returns <code>null</code>.
				''' @since 1.6 </returns>
				Protected Friend Overridable Property currentAccessibleContext As AccessibleContext
					Get
						Dim aColumn As TableColumn = columnModel.getColumn(column)
						Dim renderer As TableCellRenderer = aColumn.cellRenderer
						If renderer Is Nothing Then
							Dim columnClass As Type = getColumnClass(column)
							renderer = getDefaultRenderer(columnClass)
						End If
						Dim component As Component = renderer.getTableCellRendererComponent(JTable.this, getValueAt(row, column), False, False, row, column)
						If TypeOf component Is Accessible Then
							Return component.accessibleContext
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the table cell renderer component.
				''' </summary>
				''' <returns> the table cell renderer component if one exists;
				''' otherwise, returns <code>null</code>.
				''' @since 1.6 </returns>
				Protected Friend Overridable Property currentComponent As Component
					Get
						Dim aColumn As TableColumn = columnModel.getColumn(column)
						Dim renderer As TableCellRenderer = aColumn.cellRenderer
						If renderer Is Nothing Then
							Dim columnClass As Type = getColumnClass(column)
							renderer = getDefaultRenderer(columnClass)
						End If
						Return renderer.getTableCellRendererComponent(JTable.this, Nothing, False, False, row, column)
					End Get
				End Property

			' AccessibleContext methods

				''' <summary>
				''' Gets the accessible name of this object.
				''' </summary>
				''' <returns> the localized name of the object; <code>null</code>
				'''     if this object does not have a name </returns>
				Public Property Overrides accessibleName As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim name As String = ac.accessibleName
							If (name IsNot Nothing) AndAlso (name <> "") Then Return name
						End If
						If (accessibleName IsNot Nothing) AndAlso (accessibleName <> "") Then
							Return accessibleName
						Else
							' fall back to the client property
							Return CStr(getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleName = s
						Else
							MyBase.accessibleName = s
						End If
					End Set
				End Property


				'
				' *** should check toolTip text for desc. (needs MouseEvent)
				'
				''' <summary>
				''' Gets the accessible description of this object.
				''' </summary>
				''' <returns> the localized description of the object;
				'''     <code>null</code> if this object does not have
				'''     a description </returns>
				Public Property Overrides accessibleDescription As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleDescription
						Else
							Return MyBase.accessibleDescription
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleDescription = s
						Else
							MyBase.accessibleDescription = s
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the role of this object.
				''' </summary>
				''' <returns> an instance of <code>AccessibleRole</code>
				'''      describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleRole
						Else
							Return AccessibleRole.UNKNOWN
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the state set of this object.
				''' </summary>
				''' <returns> an instance of <code>AccessibleStateSet</code>
				'''     containing the current state set of the object </returns>
				''' <seealso cref= AccessibleState </seealso>
				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						Dim [as] As AccessibleStateSet = Nothing
    
						If ac IsNot Nothing Then [as] = ac.accessibleStateSet
						If [as] Is Nothing Then [as] = New AccessibleStateSet
						Dim rjt As Rectangle = outerInstance.visibleRect
						Dim rcell As Rectangle = outerInstance.getCellRect(row, column, False)
						If rjt.intersects(rcell) Then
							[as].add(AccessibleState.SHOWING)
						Else
							If [as].contains(AccessibleState.SHOWING) Then [as].remove(AccessibleState.SHOWING)
						End If
						If parent.isCellSelected(row, column) Then
							[as].add(AccessibleState.SELECTED)
						ElseIf [as].contains(AccessibleState.SELECTED) Then
							[as].remove(AccessibleState.SELECTED)
						End If
						If (row = selectedRow) AndAlso (column = selectedColumn) Then [as].add(AccessibleState.ACTIVE)
						[as].add(AccessibleState.TRANSIENT)
						Return [as]
					End Get
				End Property

				''' <summary>
				''' Gets the <code>Accessible</code> parent of this object.
				''' </summary>
				''' <returns> the Accessible parent of this object;
				'''     <code>null</code> if this object does not
				'''     have an <code>Accessible</code> parent </returns>
				Public Property Overrides accessibleParent As Accessible
					Get
						Return parent
					End Get
				End Property

				''' <summary>
				''' Gets the index of this object in its accessible parent.
				''' </summary>
				''' <returns> the index of this object in its parent; -1 if this
				'''     object does not have an accessible parent </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return index
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children in the object.
				''' </summary>
				''' <returns> the number of accessible children in the object </returns>
				Public Property Overrides accessibleChildrenCount As Integer
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleChildrenCount
						Else
							Return 0
						End If
					End Get
				End Property

				''' <summary>
				''' Returns the specified <code>Accessible</code> child of the
				''' object.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the <code>Accessible</code> child of the object </returns>
				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						Dim ___accessibleChild As Accessible = ac.getAccessibleChild(i)
						ac.accessibleParent = Me
						Return ___accessibleChild
					Else
						Return Nothing
					End If
				End Function

				''' <summary>
				''' Gets the locale of the component. If the component
				''' does not have a locale, then the locale of its parent
				''' is returned.
				''' </summary>
				''' <returns> this component's locale; if this component does
				'''    not have a locale, the locale of its parent is returned </returns>
				''' <exception cref="IllegalComponentStateException"> if the
				'''    <code>Component</code> does not have its own locale
				'''    and has not yet been added to a containment hierarchy
				'''    such that the locale can be determined from the
				'''    containing parent </exception>
				''' <seealso cref= #setLocale </seealso>
				Public Property Overrides locale As Locale
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.locale
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Adds a <code>PropertyChangeListener</code> to the listener list.
				''' The listener is registered for all properties.
				''' </summary>
				''' <param name="l">  the <code>PropertyChangeListener</code>
				'''     to be added </param>
				Public Overridable Sub addPropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.addPropertyChangeListener(l)
					Else
						MyBase.addPropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Removes a <code>PropertyChangeListener</code> from the
				''' listener list. This removes a <code>PropertyChangeListener</code>
				''' that was registered for all properties.
				''' </summary>
				''' <param name="l">  the <code>PropertyChangeListener</code>
				'''    to be removed </param>
				Public Overridable Sub removePropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.removePropertyChangeListener(l)
					Else
						MyBase.removePropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Gets the <code>AccessibleAction</code> associated with this
				''' object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleAction</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleAction As AccessibleAction
					Get
						Return currentAccessibleContext.accessibleAction
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleComponent</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleComponent</code>, or
				'''    <code>null</code> </returns>
				Public Property Overrides accessibleComponent As AccessibleComponent
					Get
						Return Me ' to override getBounds()
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleSelection</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleSelection</code>, or
				'''    <code>null</code> </returns>
				Public Property Overrides accessibleSelection As AccessibleSelection
					Get
						Return currentAccessibleContext.accessibleSelection
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleText</code> associated with this
				''' object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleText</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleText As AccessibleText
					Get
						Return currentAccessibleContext.accessibleText
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleValue</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleValue</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleValue As AccessibleValue
					Get
						Return currentAccessibleContext.accessibleValue
					End Get
				End Property


			' AccessibleComponent methods

				''' <summary>
				''' Gets the background color of this object.
				''' </summary>
				''' <returns> the background color, if supported, of the object;
				'''     otherwise, <code>null</code> </returns>
				Public Overridable Property background As Color Implements AccessibleComponent.getBackground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).background
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.background
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).background = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.background = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the foreground color of this object.
				''' </summary>
				''' <returns> the foreground color, if supported, of the object;
				'''     otherwise, <code>null</code> </returns>
				Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).foreground
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.foreground
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).foreground = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.foreground = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>Cursor</code> of this object.
				''' </summary>
				''' <returns> the <code>Cursor</code>, if supported,
				'''    of the object; otherwise, <code>null</code> </returns>
				Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).cursor
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.cursor
							Else
								Dim ap As Accessible = accessibleParent
								If TypeOf ap Is AccessibleComponent Then
									Return CType(ap, AccessibleComponent).cursor
								Else
									Return Nothing
								End If
							End If
						End If
					End Get
					Set(ByVal c As Cursor)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).cursor = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.cursor = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>Font</code> of this object.
				''' </summary>
				''' <returns> the <code>Font</code>,if supported,
				'''   for the object; otherwise, <code>null</code> </returns>
				Public Overridable Property font As Font Implements AccessibleComponent.getFont
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).font
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.font
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal f As Font)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).font = f
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.font = f
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>FontMetrics</code> of this object.
				''' </summary>
				''' <param name="f"> the <code>Font</code> </param>
				''' <returns> the <code>FontMetrics</code> object, if supported;
				'''    otherwise <code>null</code> </returns>
				''' <seealso cref= #getFont </seealso>
				Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getFontMetrics(f)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Return c.getFontMetrics(f)
						Else
							Return Nothing
						End If
					End If
				End Function

				''' <summary>
				''' Determines if the object is enabled.
				''' </summary>
				''' <returns> true if object is enabled; otherwise, false </returns>
				Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).enabled
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.enabled
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).enabled = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.enabled = b
						End If
					End Set
				End Property


				''' <summary>
				''' Determines if this object is visible.  Note: this means that the
				''' object intends to be visible; however, it may not in fact be
				''' showing on the screen because one of the objects that this object
				''' is contained by is not visible.  To determine if an object is
				''' showing on the screen, use <code>isShowing</code>.
				''' </summary>
				''' <returns> true if object is visible; otherwise, false </returns>
				Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).visible
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.visible
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).visible = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.visible = b
						End If
					End Set
				End Property


				''' <summary>
				''' Determines if the object is showing.  This is determined
				''' by checking the visibility of the object and ancestors
				''' of the object.  Note: this will return true even if the
				''' object is obscured by another (for example,
				''' it happens to be underneath a menu that was pulled down).
				''' </summary>
				''' <returns> true if the object is showing; otherwise, false </returns>
				Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							If ac.accessibleParent IsNot Nothing Then
								Return CType(ac, AccessibleComponent).showing
							Else
								' Fixes 4529616 - AccessibleJTableCell.isShowing()
								' returns false when the cell on the screen
								' if no parent
								Return visible
							End If
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.showing
							Else
								Return False
							End If
						End If
					End Get
				End Property

				''' <summary>
				''' Checks whether the specified point is within this
				''' object's bounds, where the point's x and y coordinates
				''' are defined to be relative to the coordinate system of
				''' the object.
				''' </summary>
				''' <param name="p"> the <code>Point</code> relative to the
				'''    coordinate system of the object </param>
				''' <returns> true if object contains <code>Point</code>;
				'''    otherwise false </returns>
				Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Dim r As Rectangle = CType(ac, AccessibleComponent).bounds
						Return r.contains(p)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Dim r As Rectangle = c.bounds
							Return r.contains(p)
						Else
							Return bounds.contains(p)
						End If
					End If
				End Function

				''' <summary>
				''' Returns the location of the object on the screen.
				''' </summary>
				''' <returns> location of object on screen -- can be
				'''    <code>null</code> if this object is not on the screen </returns>
				Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
					Get
						If parent IsNot Nothing AndAlso parent.showing Then
							Dim parentLocation As Point = parent.locationOnScreen
							Dim componentLocation As Point = location
							componentLocation.translate(parentLocation.x, parentLocation.y)
							Return componentLocation
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the location of the object relative to the parent
				''' in the form of a point specifying the object's
				''' top-left corner in the screen's coordinate space.
				''' </summary>
				''' <returns> an instance of <code>Point</code> representing
				'''    the top-left corner of the object's bounds in the
				'''    coordinate space of the screen; <code>null</code> if
				'''    this object or its parent are not on the screen </returns>
				Public Overridable Property location As Point Implements AccessibleComponent.getLocation
					Get
						If parent IsNot Nothing Then
							Dim r As Rectangle = parent.getCellRect(row, column, False)
							If r IsNot Nothing Then Return r.location
						End If
						Return Nothing
					End Get
					Set(ByVal p As Point)
		'              if ((parent != null)  && (parent.contains(p))) {
		'                  ensureIndexIsVisible(indexInParent);
		'              }
					End Set
				End Property


				Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
					Get
						If parent IsNot Nothing Then
							Return parent.getCellRect(row, column, False)
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal r As Rectangle)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).bounds = r
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.bounds = r
						End If
					End Set
				End Property


				Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
					Get
						If parent IsNot Nothing Then
							Dim r As Rectangle = parent.getCellRect(row, column, False)
							If r IsNot Nothing Then Return r.size
						End If
						Return Nothing
					End Get
					Set(ByVal d As Dimension)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).size = d
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.size = d
						End If
					End Set
				End Property


				Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getAccessibleAt(p)
					Else
						Return Nothing
					End If
				End Function

				Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).focusTraversable
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.focusTraversable
							Else
								Return False
							End If
						End If
					End Get
				End Property

				Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).requestFocus()
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.requestFocus()
					End If
				End Sub

				Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).addFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.addFocusListener(l)
					End If
				End Sub

				Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).removeFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.removeFocusListener(l)
					End If
				End Sub

			End Class ' inner class AccessibleJTableCell

			' Begin AccessibleJTableHeader ========== // TIGER - 4715503

			''' <summary>
			''' This class implements accessibility for JTable header cells.
			''' </summary>
			Private Class AccessibleJTableHeaderCell
				Inherits AccessibleContext
				Implements Accessible, AccessibleComponent

				Private ReadOnly outerInstance As JTable.AccessibleJTable


				Private row As Integer
				Private column As Integer
				Private parent As JTableHeader
				Private rendererComponent As Component

				''' <summary>
				''' Constructs an <code>AccessibleJTableHeaderEntry</code> instance.
				''' </summary>
				''' <param name="row"> header cell row index </param>
				''' <param name="column"> header cell column index </param>
				''' <param name="parent"> header cell parent </param>
				''' <param name="rendererComponent"> component that renders the header cell </param>
				Public Sub New(ByVal outerInstance As JTable.AccessibleJTable, ByVal row As Integer, ByVal column As Integer, ByVal parent As JTableHeader, ByVal rendererComponent As Component)
						Me.outerInstance = outerInstance
					Me.row = row
					Me.column = column
					Me.parent = parent
					Me.rendererComponent = rendererComponent
					Me.accessibleParent = parent
				End Sub

				''' <summary>
				''' Gets the <code>AccessibleContext</code> associated with this
				''' component. In the implementation of the Java Accessibility
				''' API for this class, return this object, which is its own
				''' <code>AccessibleContext</code>.
				''' </summary>
				''' <returns> this object </returns>
				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						Return Me
					End Get
				End Property

	'            
	'             * Returns the AccessibleContext for the header cell
	'             * renderer.
	'             
				Private Property currentAccessibleContext As AccessibleContext
					Get
						Return rendererComponent.accessibleContext
					End Get
				End Property

	'            
	'             * Returns the component that renders the header cell.
	'             
				Private Property currentComponent As Component
					Get
						Return rendererComponent
					End Get
				End Property

				' AccessibleContext methods ==========

				''' <summary>
				''' Gets the accessible name of this object.
				''' </summary>
				''' <returns> the localized name of the object; <code>null</code>
				'''     if this object does not have a name </returns>
				Public Property Overrides accessibleName As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Dim name As String = ac.accessibleName
							If (name IsNot Nothing) AndAlso (name <> "") Then Return ac.accessibleName
						End If
						If (accessibleName IsNot Nothing) AndAlso (accessibleName <> "") Then
							Return accessibleName
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleName = s
						Else
							MyBase.accessibleName = s
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the accessible description of this object.
				''' </summary>
				''' <returns> the localized description of the object;
				'''     <code>null</code> if this object does not have
				'''     a description </returns>
				Public Property Overrides accessibleDescription As String
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleDescription
						Else
							Return MyBase.accessibleDescription
						End If
					End Get
					Set(ByVal s As String)
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							ac.accessibleDescription = s
						Else
							MyBase.accessibleDescription = s
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the role of this object.
				''' </summary>
				''' <returns> an instance of <code>AccessibleRole</code>
				'''      describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleRole
						Else
							Return AccessibleRole.UNKNOWN
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the state set of this object.
				''' </summary>
				''' <returns> an instance of <code>AccessibleStateSet</code>
				'''     containing the current state set of the object </returns>
				''' <seealso cref= AccessibleState </seealso>
				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						Dim [as] As AccessibleStateSet = Nothing
    
						If ac IsNot Nothing Then [as] = ac.accessibleStateSet
						If [as] Is Nothing Then [as] = New AccessibleStateSet
						Dim rjt As Rectangle = outerInstance.visibleRect
						Dim rcell As Rectangle = outerInstance.getCellRect(row, column, False)
						If rjt.intersects(rcell) Then
							[as].add(AccessibleState.SHOWING)
						Else
							If [as].contains(AccessibleState.SHOWING) Then [as].remove(AccessibleState.SHOWING)
						End If
						If outerInstance.isCellSelected(row, column) Then
							[as].add(AccessibleState.SELECTED)
						ElseIf [as].contains(AccessibleState.SELECTED) Then
							[as].remove(AccessibleState.SELECTED)
						End If
						If (row = selectedRow) AndAlso (column = selectedColumn) Then [as].add(AccessibleState.ACTIVE)
						[as].add(AccessibleState.TRANSIENT)
						Return [as]
					End Get
				End Property

				''' <summary>
				''' Gets the <code>Accessible</code> parent of this object.
				''' </summary>
				''' <returns> the Accessible parent of this object;
				'''     <code>null</code> if this object does not
				'''     have an <code>Accessible</code> parent </returns>
				Public Property Overrides accessibleParent As Accessible
					Get
						Return parent
					End Get
				End Property

				''' <summary>
				''' Gets the index of this object in its accessible parent.
				''' </summary>
				''' <returns> the index of this object in its parent; -1 if this
				'''     object does not have an accessible parent </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return column
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children in the object.
				''' </summary>
				''' <returns> the number of accessible children in the object </returns>
				Public Property Overrides accessibleChildrenCount As Integer
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.accessibleChildrenCount
						Else
							Return 0
						End If
					End Get
				End Property

				''' <summary>
				''' Returns the specified <code>Accessible</code> child of the
				''' object.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the <code>Accessible</code> child of the object </returns>
				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						Dim ___accessibleChild As Accessible = ac.getAccessibleChild(i)
						ac.accessibleParent = Me
						Return ___accessibleChild
					Else
						Return Nothing
					End If
				End Function

				''' <summary>
				''' Gets the locale of the component. If the component
				''' does not have a locale, then the locale of its parent
				''' is returned.
				''' </summary>
				''' <returns> this component's locale; if this component does
				'''    not have a locale, the locale of its parent is returned </returns>
				''' <exception cref="IllegalComponentStateException"> if the
				'''    <code>Component</code> does not have its own locale
				'''    and has not yet been added to a containment hierarchy
				'''    such that the locale can be determined from the
				'''    containing parent </exception>
				''' <seealso cref= #setLocale </seealso>
				Public Property Overrides locale As Locale
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If ac IsNot Nothing Then
							Return ac.locale
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Adds a <code>PropertyChangeListener</code> to the listener list.
				''' The listener is registered for all properties.
				''' </summary>
				''' <param name="l">  the <code>PropertyChangeListener</code>
				'''     to be added </param>
				Public Overridable Sub addPropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.addPropertyChangeListener(l)
					Else
						MyBase.addPropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Removes a <code>PropertyChangeListener</code> from the
				''' listener list. This removes a <code>PropertyChangeListener</code>
				''' that was registered for all properties.
				''' </summary>
				''' <param name="l">  the <code>PropertyChangeListener</code>
				'''    to be removed </param>
				Public Overridable Sub removePropertyChangeListener(ByVal l As PropertyChangeListener)
					Dim ac As AccessibleContext = currentAccessibleContext
					If ac IsNot Nothing Then
						ac.removePropertyChangeListener(l)
					Else
						MyBase.removePropertyChangeListener(l)
					End If
				End Sub

				''' <summary>
				''' Gets the <code>AccessibleAction</code> associated with this
				''' object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleAction</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleAction As AccessibleAction
					Get
						Return currentAccessibleContext.accessibleAction
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleComponent</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleComponent</code>, or
				'''    <code>null</code> </returns>
				Public Property Overrides accessibleComponent As AccessibleComponent
					Get
						Return Me ' to override getBounds()
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleSelection</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleSelection</code>, or
				'''    <code>null</code> </returns>
				Public Property Overrides accessibleSelection As AccessibleSelection
					Get
						Return currentAccessibleContext.accessibleSelection
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleText</code> associated with this
				''' object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleText</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleText As AccessibleText
					Get
						Return currentAccessibleContext.accessibleText
					End Get
				End Property

				''' <summary>
				''' Gets the <code>AccessibleValue</code> associated with
				''' this object if one exists.  Otherwise returns <code>null</code>.
				''' </summary>
				''' <returns> the <code>AccessibleValue</code>, or <code>null</code> </returns>
				Public Property Overrides accessibleValue As AccessibleValue
					Get
						Return currentAccessibleContext.accessibleValue
					End Get
				End Property


				' AccessibleComponent methods ==========

				''' <summary>
				''' Gets the background color of this object.
				''' </summary>
				''' <returns> the background color, if supported, of the object;
				'''     otherwise, <code>null</code> </returns>
				Public Overridable Property background As Color Implements AccessibleComponent.getBackground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).background
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.background
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).background = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.background = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the foreground color of this object.
				''' </summary>
				''' <returns> the foreground color, if supported, of the object;
				'''     otherwise, <code>null</code> </returns>
				Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).foreground
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.foreground
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal c As Color)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).foreground = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.foreground = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>Cursor</code> of this object.
				''' </summary>
				''' <returns> the <code>Cursor</code>, if supported,
				'''    of the object; otherwise, <code>null</code> </returns>
				Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).cursor
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.cursor
							Else
								Dim ap As Accessible = accessibleParent
								If TypeOf ap Is AccessibleComponent Then
									Return CType(ap, AccessibleComponent).cursor
								Else
									Return Nothing
								End If
							End If
						End If
					End Get
					Set(ByVal c As Cursor)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).cursor = c
						Else
							Dim cp As Component = currentComponent
							If cp IsNot Nothing Then cp.cursor = c
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>Font</code> of this object.
				''' </summary>
				''' <returns> the <code>Font</code>,if supported,
				'''   for the object; otherwise, <code>null</code> </returns>
				Public Overridable Property font As Font Implements AccessibleComponent.getFont
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).font
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.font
							Else
								Return Nothing
							End If
						End If
					End Get
					Set(ByVal f As Font)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).font = f
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.font = f
						End If
					End Set
				End Property


				''' <summary>
				''' Gets the <code>FontMetrics</code> of this object.
				''' </summary>
				''' <param name="f"> the <code>Font</code> </param>
				''' <returns> the <code>FontMetrics</code> object, if supported;
				'''    otherwise <code>null</code> </returns>
				''' <seealso cref= #getFont </seealso>
				Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getFontMetrics(f)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Return c.getFontMetrics(f)
						Else
							Return Nothing
						End If
					End If
				End Function

				''' <summary>
				''' Determines if the object is enabled.
				''' </summary>
				''' <returns> true if object is enabled; otherwise, false </returns>
				Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).enabled
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.enabled
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).enabled = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.enabled = b
						End If
					End Set
				End Property


				''' <summary>
				''' Determines if this object is visible.  Note: this means that the
				''' object intends to be visible; however, it may not in fact be
				''' showing on the screen because one of the objects that this object
				''' is contained by is not visible.  To determine if an object is
				''' showing on the screen, use <code>isShowing</code>.
				''' </summary>
				''' <returns> true if object is visible; otherwise, false </returns>
				Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).visible
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.visible
							Else
								Return False
							End If
						End If
					End Get
					Set(ByVal b As Boolean)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).visible = b
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.visible = b
						End If
					End Set
				End Property


				''' <summary>
				''' Determines if the object is showing.  This is determined
				''' by checking the visibility of the object and ancestors
				''' of the object.  Note: this will return true even if the
				''' object is obscured by another (for example,
				''' it happens to be underneath a menu that was pulled down).
				''' </summary>
				''' <returns> true if the object is showing; otherwise, false </returns>
				Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							If ac.accessibleParent IsNot Nothing Then
								Return CType(ac, AccessibleComponent).showing
							Else
								' Fixes 4529616 - AccessibleJTableCell.isShowing()
								' returns false when the cell on the screen
								' if no parent
								Return visible
							End If
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.showing
							Else
								Return False
							End If
						End If
					End Get
				End Property

				''' <summary>
				''' Checks whether the specified point is within this
				''' object's bounds, where the point's x and y coordinates
				''' are defined to be relative to the coordinate system of
				''' the object.
				''' </summary>
				''' <param name="p"> the <code>Point</code> relative to the
				'''    coordinate system of the object </param>
				''' <returns> true if object contains <code>Point</code>;
				'''    otherwise false </returns>
				Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Dim r As Rectangle = CType(ac, AccessibleComponent).bounds
						Return r.contains(p)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then
							Dim r As Rectangle = c.bounds
							Return r.contains(p)
						Else
							Return bounds.contains(p)
						End If
					End If
				End Function

				''' <summary>
				''' Returns the location of the object on the screen.
				''' </summary>
				''' <returns> location of object on screen -- can be
				'''    <code>null</code> if this object is not on the screen </returns>
				Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
					Get
						If parent IsNot Nothing AndAlso parent.showing Then
							Dim parentLocation As Point = parent.locationOnScreen
							Dim componentLocation As Point = location
							componentLocation.translate(parentLocation.x, parentLocation.y)
							Return componentLocation
						Else
							Return Nothing
						End If
					End Get
				End Property

				''' <summary>
				''' Gets the location of the object relative to the parent
				''' in the form of a point specifying the object's
				''' top-left corner in the screen's coordinate space.
				''' </summary>
				''' <returns> an instance of <code>Point</code> representing
				'''    the top-left corner of the object's bounds in the
				'''    coordinate space of the screen; <code>null</code> if
				'''    this object or its parent are not on the screen </returns>
				Public Overridable Property location As Point Implements AccessibleComponent.getLocation
					Get
						If parent IsNot Nothing Then
							Dim r As Rectangle = parent.getHeaderRect(column)
							If r IsNot Nothing Then Return r.location
						End If
						Return Nothing
					End Get
					Set(ByVal p As Point)
					End Set
				End Property


				''' <summary>
				''' Gets the bounds of this object in the form of a Rectangle object.
				''' The bounds specify this object's width, height, and location
				''' relative to its parent.
				''' </summary>
				''' <returns> A rectangle indicating this component's bounds; null if
				''' this object is not on the screen. </returns>
				''' <seealso cref= #contains </seealso>
				Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
					Get
						If parent IsNot Nothing Then
							Return parent.getHeaderRect(column)
						Else
							Return Nothing
						End If
					End Get
					Set(ByVal r As Rectangle)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).bounds = r
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.bounds = r
						End If
					End Set
				End Property


				''' <summary>
				''' Returns the size of this object in the form of a Dimension object.
				''' The height field of the Dimension object contains this object's
				''' height, and the width field of the Dimension object contains this
				''' object's width.
				''' </summary>
				''' <returns> A Dimension object that indicates the size of this component;
				''' null if this object is not on the screen </returns>
				''' <seealso cref= #setSize </seealso>
				Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
					Get
						If parent IsNot Nothing Then
							Dim r As Rectangle = parent.getHeaderRect(column)
							If r IsNot Nothing Then Return r.size
						End If
						Return Nothing
					End Get
					Set(ByVal d As Dimension)
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							CType(ac, AccessibleComponent).size = d
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then c.size = d
						End If
					End Set
				End Property


				''' <summary>
				''' Returns the Accessible child, if one exists, contained at the local
				''' coordinate Point.
				''' </summary>
				''' <param name="p"> The point relative to the coordinate system of this object. </param>
				''' <returns> the Accessible, if it exists, at the specified location;
				''' otherwise null </returns>
				Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						Return CType(ac, AccessibleComponent).getAccessibleAt(p)
					Else
						Return Nothing
					End If
				End Function

				''' <summary>
				''' Returns whether this object can accept focus or not.   Objects that
				''' can accept focus will also have the AccessibleState.FOCUSABLE state
				''' set in their AccessibleStateSets.
				''' </summary>
				''' <returns> true if object can accept focus; otherwise false </returns>
				''' <seealso cref= AccessibleContext#getAccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState#FOCUSABLE </seealso>
				''' <seealso cref= AccessibleState#FOCUSED </seealso>
				''' <seealso cref= AccessibleStateSet </seealso>
				Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
					Get
						Dim ac As AccessibleContext = currentAccessibleContext
						If TypeOf ac Is AccessibleComponent Then
							Return CType(ac, AccessibleComponent).focusTraversable
						Else
							Dim c As Component = currentComponent
							If c IsNot Nothing Then
								Return c.focusTraversable
							Else
								Return False
							End If
						End If
					End Get
				End Property

				''' <summary>
				''' Requests focus for this object.  If this object cannot accept focus,
				''' nothing will happen.  Otherwise, the object will attempt to take
				''' focus. </summary>
				''' <seealso cref= #isFocusTraversable </seealso>
				Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).requestFocus()
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.requestFocus()
					End If
				End Sub

				''' <summary>
				''' Adds the specified focus listener to receive focus events from this
				''' component.
				''' </summary>
				''' <param name="l"> the focus listener </param>
				''' <seealso cref= #removeFocusListener </seealso>
				Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).addFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.addFocusListener(l)
					End If
				End Sub

				''' <summary>
				''' Removes the specified focus listener so it no longer receives focus
				''' events from this component.
				''' </summary>
				''' <param name="l"> the focus listener </param>
				''' <seealso cref= #addFocusListener </seealso>
				Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
					Dim ac As AccessibleContext = currentAccessibleContext
					If TypeOf ac Is AccessibleComponent Then
						CType(ac, AccessibleComponent).removeFocusListener(l)
					Else
						Dim c As Component = currentComponent
						If c IsNot Nothing Then c.removeFocusListener(l)
					End If
				End Sub

			End Class ' inner class AccessibleJTableHeaderCell

		End Class ' inner class AccessibleJTable

	End Class ' End of Class JTable

End Namespace