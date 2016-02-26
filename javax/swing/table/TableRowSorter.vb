Imports System

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' An implementation of <code>RowSorter</code> that provides sorting
	''' and filtering using a <code>TableModel</code>.
	''' The following example shows adding sorting to a <code>JTable</code>:
	''' <pre>
	'''   TableModel myModel = createMyTableModel();
	'''   JTable table = new JTable(myModel);
	'''   table.setRowSorter(new TableRowSorter(myModel));
	''' </pre>
	''' This will do all the wiring such that when the user does the appropriate
	''' gesture, such as clicking on the column header, the table will
	''' visually sort.
	''' <p>
	''' <code>JTable</code>'s row-based methods and <code>JTable</code>'s
	''' selection model refer to the view and not the underlying
	''' model. Therefore, it is necessary to convert between the two.  For
	''' example, to get the selection in terms of <code>myModel</code>
	''' you need to convert the indices:
	''' <pre>
	'''   int[] selection = table.getSelectedRows();
	'''   for (int i = 0; i &lt; selection.length; i++) {
	'''     selection[i] = table.convertRowIndexToModel(selection[i]);
	'''   }
	''' </pre>
	''' Similarly to select a row in <code>JTable</code> based on
	''' a coordinate from the underlying model do the inverse:
	''' <pre>
	'''   table.setRowSelectionInterval(table.convertRowIndexToView(row),
	'''                                 table.convertRowIndexToView(row));
	''' </pre>
	''' <p>
	''' The previous example assumes you have not enabled filtering.  If you
	''' have enabled filtering <code>convertRowIndexToView</code> will return
	''' -1 for locations that are not visible in the view.
	''' <p>
	''' <code>TableRowSorter</code> uses <code>Comparator</code>s for doing
	''' comparisons. The following defines how a <code>Comparator</code> is
	''' chosen for a column:
	''' <ol>
	''' <li>If a <code>Comparator</code> has been specified for the column by the
	'''     <code>setComparator</code> method, use it.
	''' <li>If the column class as returned by <code>getColumnClass</code> is
	'''     <code>String</code>, use the <code>Comparator</code> returned by
	'''     <code>Collator.getInstance()</code>.
	''' <li>If the column class implements <code>Comparable</code>, use a
	'''     <code>Comparator</code> that invokes the <code>compareTo</code>
	'''     method.
	''' <li>If a <code>TableStringConverter</code> has been specified, use it
	'''     to convert the values to <code>String</code>s and then use the
	'''     <code>Comparator</code> returned by <code>Collator.getInstance()</code>.
	''' <li>Otherwise use the <code>Comparator</code> returned by
	'''     <code>Collator.getInstance()</code> on the results from
	'''     calling <code>toString</code> on the objects.
	''' </ol>
	''' <p>
	''' In addition to sorting <code>TableRowSorter</code> provides the ability
	''' to filter.  A filter is specified using the <code>setFilter</code>
	''' method. The following example will only show rows containing the string
	''' "foo":
	''' <pre>
	'''   TableModel myModel = createMyTableModel();
	'''   TableRowSorter sorter = new TableRowSorter(myModel);
	'''   sorter.setRowFilter(RowFilter.regexFilter(".*foo.*"));
	'''   JTable table = new JTable(myModel);
	'''   table.setRowSorter(sorter);
	''' </pre>
	''' <p>
	''' If the underlying model structure changes (the
	''' <code>modelStructureChanged</code> method is invoked) the following
	''' are reset to their default values: <code>Comparator</code>s by
	''' column, current sort order, and whether each column is sortable. The default
	''' sort order is natural (the same as the model), and columns are
	''' sortable by default.
	''' <p>
	''' <code>TableRowSorter</code> has one formal type parameter: the type
	''' of the model.  Passing in a type that corresponds exactly to your
	''' model allows you to filter based on your model without casting.
	''' Refer to the documentation of <code>RowFilter</code> for an example
	''' of this.
	''' <p>
	''' <b>WARNING:</b> <code>DefaultTableModel</code> returns a column
	''' class of <code>Object</code>.  As such all comparisons will
	''' be done using <code>toString</code>.  This may be unnecessarily
	''' expensive.  If the column only contains one type of value, such as
	''' an <code>Integer</code>, you should override <code>getColumnClass</code> and
	''' return the appropriate <code>Class</code>.  This will dramatically
	''' increase the performance of this class.
	''' </summary>
	''' @param <M> the type of the model, which must be an implementation of
	'''            <code>TableModel</code> </param>
	''' <seealso cref= javax.swing.JTable </seealso>
	''' <seealso cref= javax.swing.RowFilter </seealso>
	''' <seealso cref= javax.swing.table.DefaultTableModel </seealso>
	''' <seealso cref= java.text.Collator </seealso>
	''' <seealso cref= java.util.Comparator
	''' @since 1.6 </seealso>
	Public Class TableRowSorter(Of M As TableModel)
		Inherits javax.swing.DefaultRowSorter(Of M, Integer?)

		''' <summary>
		''' Comparator that uses compareTo on the contents.
		''' </summary>
		Private Shared ReadOnly COMPARABLE_COMPARATOR As IComparer = New ComparableComparator

		''' <summary>
		''' Underlying model.
		''' </summary>
		Private tableModel As M

		''' <summary>
		''' For toString conversions.
		''' </summary>
		Private stringConverter As TableStringConverter


		''' <summary>
		''' Creates a <code>TableRowSorter</code> with an empty model.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>TableRowSorter</code> using <code>model</code>
		''' as the underlying <code>TableModel</code>.
		''' </summary>
		''' <param name="model"> the underlying <code>TableModel</code> to use,
		'''        <code>null</code> is treated as an empty model </param>
		Public Sub New(ByVal model As M)
			model = model
		End Sub

		''' <summary>
		''' Sets the <code>TableModel</code> to use as the underlying model
		''' for this <code>TableRowSorter</code>.  A value of <code>null</code>
		''' can be used to set an empty model.
		''' </summary>
		''' <param name="model"> the underlying model to use, or <code>null</code> </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setModel(ByVal model As M) 'JavaToDotNetTempPropertySetmodel
		Public Overridable Property model As M
			Set(ByVal model As M)
				tableModel = model
				modelWrapper = New TableRowSorterModelWrapper(Me)
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the object responsible for converting values from the
		''' model to strings.  If non-<code>null</code> this
		''' is used to convert any object values, that do not have a
		''' registered <code>Comparator</code>, to strings.
		''' </summary>
		''' <param name="stringConverter"> the object responsible for converting values
		'''        from the model to strings </param>
		Public Overridable Property stringConverter As TableStringConverter
			Set(ByVal stringConverter As TableStringConverter)
				Me.stringConverter = stringConverter
			End Set
			Get
				Return stringConverter
			End Get
		End Property


		''' <summary>
		''' Returns the <code>Comparator</code> for the specified
		''' column.  If a <code>Comparator</code> has not been specified using
		''' the <code>setComparator</code> method a <code>Comparator</code>
		''' will be returned based on the column class
		''' (<code>TableModel.getColumnClass</code>) of the specified column.
		''' If the column class is <code>String</code>,
		''' <code>Collator.getInstance</code> is returned.  If the
		''' column class implements <code>Comparable</code> a private
		''' <code>Comparator</code> is returned that invokes the
		''' <code>compareTo</code> method.  Otherwise
		''' <code>Collator.getInstance</code> is returned.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function getComparator(ByVal column As Integer) As IComparer(Of ?)
			Dim ___comparator As IComparer = MyBase.getComparator(column)
			If ___comparator IsNot Nothing Then Return ___comparator
			Dim columnClass As Type = model.getColumnClass(column)
			If columnClass Is GetType(String) Then Return java.text.Collator.instance
			If columnClass.IsSubclassOf(GetType(IComparable)) Then Return COMPARABLE_COMPARATOR
			Return java.text.Collator.instance
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Protected Friend Overridable Function useToString(ByVal column As Integer) As Boolean
			Dim ___comparator As IComparer = MyBase.getComparator(column)
			If ___comparator IsNot Nothing Then Return False
			Dim columnClass As Type = model.getColumnClass(column)
			If columnClass Is GetType(String) Then Return False
			If columnClass.IsSubclassOf(GetType(IComparable)) Then Return False
			Return True
		End Function

		''' <summary>
		''' Implementation of DefaultRowSorter.ModelWrapper that delegates to a
		''' TableModel.
		''' </summary>
		Private Class TableRowSorterModelWrapper
			Inherits ModelWrapper(Of M, Integer?)

			Private ReadOnly outerInstance As TableRowSorter

			Public Sub New(ByVal outerInstance As TableRowSorter)
				Me.outerInstance = outerInstance
			End Sub

				Return outerInstance.tableModel
			End Function

			Public Overridable Property columnCount As Integer
				Get
					Return If(outerInstance.tableModel Is Nothing, 0, outerInstance.tableModel.columnCount)
				End Get
			End Property

			Public Overridable Property rowCount As Integer
				Get
					Return If(outerInstance.tableModel Is Nothing, 0, outerInstance.tableModel.rowCount)
				End Get
			End Property

			Public Overridable Function getValueAt(ByVal row As Integer, ByVal column As Integer) As Object
				Return outerInstance.tableModel.getValueAt(row, column)
			End Function

			Public Overridable Function getStringValueAt(ByVal row As Integer, ByVal column As Integer) As String
				Dim converter As TableStringConverter = outerInstance.stringConverter
				If converter IsNot Nothing Then
					' Use the converter
					Dim value As String = converter.ToString(outerInstance.tableModel, row, column)
					If value IsNot Nothing Then Return value
					Return ""
				End If

				' No converter, use getValueAt followed by toString
				Dim o As Object = getValueAt(row, column)
				If o Is Nothing Then Return ""
				Dim [string] As String = o.ToString()
				If [string] Is Nothing Then Return ""
				Return [string]
			End Function

			Public Overridable Function getIdentifier(ByVal index As Integer) As Integer?
				Return index
			End Function
		End Class


		Private Class ComparableComparator
			Implements IComparer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Function compare(ByVal o1 As Object, ByVal o2 As Object) As Integer
				Return CType(o1, IComparable).CompareTo(o2)
			End Function
		End Class
	End Class

End Namespace