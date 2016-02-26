Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An implementation of <code>RowSorter</code> that provides sorting and
	''' filtering around a grid-based data model.
	''' Beyond creating and installing a <code>RowSorter</code>, you very rarely
	''' need to interact with one directly.  Refer to
	''' <seealso cref="javax.swing.table.TableRowSorter TableRowSorter"/> for a concrete
	''' implementation of <code>RowSorter</code> for <code>JTable</code>.
	''' <p>
	''' Sorting is done based on the current <code>SortKey</code>s, in order.
	''' If two objects are equal (the <code>Comparator</code> for the
	''' column returns 0) the next <code>SortKey</code> is used.  If no
	''' <code>SortKey</code>s remain or the order is <code>UNSORTED</code>, then
	''' the order of the rows in the model is used.
	''' <p>
	''' Sorting of each column is done by way of a <code>Comparator</code>
	''' that you can specify using the <code>setComparator</code> method.
	''' If a <code>Comparator</code> has not been specified, the
	''' <code>Comparator</code> returned by
	''' <code>Collator.getInstance()</code> is used on the results of
	''' calling <code>toString</code> on the underlying objects.  The
	''' <code>Comparator</code> is never passed <code>null</code>.  A
	''' <code>null</code> value is treated as occurring before a
	''' non-<code>null</code> value, and two <code>null</code> values are
	''' considered equal.
	''' <p>
	''' If you specify a <code>Comparator</code> that casts its argument to
	''' a type other than that provided by the model, a
	''' <code>ClassCastException</code> will be thrown when the data is sorted.
	''' <p>
	''' In addition to sorting, <code>DefaultRowSorter</code> provides the
	''' ability to filter rows.  Filtering is done by way of a
	''' <code>RowFilter</code> that is specified using the
	''' <code>setRowFilter</code> method.  If no filter has been specified all
	''' rows are included.
	''' <p>
	''' By default, rows are in unsorted order (the same as the model) and
	''' every column is sortable. The default <code>Comparator</code>s are
	''' documented in the subclasses (for example, {@link
	''' javax.swing.table.TableRowSorter TableRowSorter}).
	''' <p>
	''' If the underlying model structure changes (the
	''' <code>modelStructureChanged</code> method is invoked) the following
	''' are reset to their default values: <code>Comparator</code>s by
	''' column, current sort order, and whether each column is sortable. To
	''' find the default <code>Comparator</code>s, see the concrete
	''' implementation (for example, {@link
	''' javax.swing.table.TableRowSorter TableRowSorter}).  The default
	''' sort order is unsorted (the same as the model), and columns are
	''' sortable by default.
	''' <p>
	''' If the underlying model structure changes (the
	''' <code>modelStructureChanged</code> method is invoked) the following
	''' are reset to their default values: <code>Comparator</code>s by column,
	''' current sort order and whether a column is sortable.
	''' <p>
	''' <code>DefaultRowSorter</code> is an abstract class.  Concrete
	''' subclasses must provide access to the underlying data by invoking
	''' {@code setModelWrapper}. The {@code setModelWrapper} method
	''' <b>must</b> be invoked soon after the constructor is
	''' called, ideally from within the subclass's constructor.
	''' Undefined behavior will result if you use a {@code
	''' DefaultRowSorter} without specifying a {@code ModelWrapper}.
	''' <p>
	''' <code>DefaultRowSorter</code> has two formal type parameters.  The
	''' first type parameter corresponds to the class of the model, for example
	''' <code>DefaultTableModel</code>.  The second type parameter
	''' corresponds to the class of the identifier passed to the
	''' <code>RowFilter</code>.  Refer to <code>TableRowSorter</code> and
	''' <code>RowFilter</code> for more details on the type parameters.
	''' </summary>
	''' @param <M> the type of the model </param>
	''' @param <I> the type of the identifier passed to the <code>RowFilter</code> </param>
	''' <seealso cref= javax.swing.table.TableRowSorter </seealso>
	''' <seealso cref= javax.swing.table.DefaultTableModel </seealso>
	''' <seealso cref= java.text.Collator
	''' @since 1.6 </seealso>
	Public MustInherit Class DefaultRowSorter(Of M, I)
		Inherits RowSorter(Of M)

		''' <summary>
		''' Whether or not we resort on TableModelEvent.UPDATEs.
		''' </summary>
		Private sortsOnUpdates As Boolean

		''' <summary>
		''' View (JTable) -> model.
		''' </summary>
		Private viewToModel As Row()

		''' <summary>
		''' model -> view (JTable)
		''' </summary>
		Private modelToView As Integer()

		''' <summary>
		''' Comparators specified by column.
		''' </summary>
		Private comparators As IComparer()

		''' <summary>
		''' Whether or not the specified column is sortable, by column.
		''' </summary>
		Private ___isSortable As Boolean()

		''' <summary>
		''' Cached SortKeys for the current sort.
		''' </summary>
		Private cachedSortKeys As SortKey()

		''' <summary>
		''' Cached comparators for the current sort
		''' </summary>
		Private sortComparators As IComparer()

		''' <summary>
		''' Developer supplied Filter.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private filter As RowFilter(Of ?, ?)

		''' <summary>
		''' Value passed to the filter.  The same instance is passed to the
		''' filter for different rows.
		''' </summary>
		Private filterEntry As FilterEntry

		''' <summary>
		''' The sort keys.
		''' </summary>
		Private sortKeys As IList(Of SortKey)

		''' <summary>
		''' Whether or not to use getStringValueAt.  This is indexed by column.
		''' </summary>
		Private ___useToString As Boolean()

		''' <summary>
		''' Indicates the contents are sorted.  This is used if
		''' getSortsOnUpdates is false and an update event is received.
		''' </summary>
		Private sorted As Boolean

		''' <summary>
		''' Maximum number of sort keys.
		''' </summary>
		Private maxSortKeys As Integer

		''' <summary>
		''' Provides access to the data we're sorting/filtering.
		''' </summary>
		Private modelWrapper As ModelWrapper(Of M, I)

		''' <summary>
		''' Size of the model. This is used to enforce error checking within
		''' the table changed notification methods (such as rowsInserted).
		''' </summary>
		Private modelRowCount As Integer


		''' <summary>
		''' Creates an empty <code>DefaultRowSorter</code>.
		''' </summary>
		Public Sub New()
			sortKeys = java.util.Collections.emptyList()
			maxSortKeys = 3
		End Sub

		''' <summary>
		''' Sets the model wrapper providing the data that is being sorted and
		''' filtered.
		''' </summary>
		''' <param name="modelWrapper"> the model wrapper responsible for providing the
		'''         data that gets sorted and filtered </param>
		''' <exception cref="IllegalArgumentException"> if {@code modelWrapper} is
		'''         {@code null} </exception>
		Protected Friend Property modelWrapper As ModelWrapper(Of M, I)
			Set(ByVal modelWrapper As ModelWrapper(Of M, I))
				If modelWrapper Is Nothing Then Throw New System.ArgumentException("modelWrapper most be non-null")
				Dim last As ModelWrapper(Of M, I) = Me.modelWrapper
				Me.modelWrapper = modelWrapper
				If last IsNot Nothing Then
					modelStructureChanged()
				Else
					' If last is null, we're in the constructor. If we're in
					' the constructor we don't want to call to overridable methods.
					modelRowCount = modelWrapper.rowCount
				End If
			End Set
			Get
				Return modelWrapper
			End Get
		End Property


		''' <summary>
		''' Returns the underlying model.
		''' </summary>
		''' <returns> the underlying model </returns>
		Public Property model As M
			Get
				Return modelWrapper.model
			End Get
		End Property

		''' <summary>
		''' Sets whether or not the specified column is sortable.  The specified
		''' value is only checked when <code>toggleSortOrder</code> is invoked.
		''' It is still possible to sort on a column that has been marked as
		''' unsortable by directly setting the sort keys.  The default is
		''' true.
		''' </summary>
		''' <param name="column"> the column to enable or disable sorting on, in terms
		'''        of the underlying model </param>
		''' <param name="sortable"> whether or not the specified column is sortable </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>column</code> is outside
		'''         the range of the model </exception>
		''' <seealso cref= #toggleSortOrder </seealso>
		''' <seealso cref= #setSortKeys </seealso>
		Public Overridable Sub setSortable(ByVal column As Integer, ByVal sortable As Boolean)
			checkColumn(column)
			If ___isSortable Is Nothing Then
				___isSortable = New Boolean(modelWrapper.columnCount - 1){}
				For i As Integer = ___isSortable.Length - 1 To 0 Step -1
					___isSortable(i) = True
				Next i
			End If
			___isSortable(column) = sortable
		End Sub

		''' <summary>
		''' Returns true if the specified column is sortable; otherwise, false.
		''' </summary>
		''' <param name="column"> the column to check sorting for, in terms of the
		'''        underlying model </param>
		''' <returns> true if the column is sortable </returns>
		''' <exception cref="IndexOutOfBoundsException"> if column is outside
		'''         the range of the underlying model </exception>
		Public Overridable Function isSortable(ByVal column As Integer) As Boolean
			checkColumn(column)
			Return If(___isSortable Is Nothing, True, ___isSortable(column))
		End Function

		''' <summary>
		''' Sets the sort keys. This creates a copy of the supplied
		''' {@code List}; subsequent changes to the supplied
		''' {@code List} do not effect this {@code DefaultRowSorter}.
		''' If the sort keys have changed this triggers a sort.
		''' </summary>
		''' <param name="sortKeys"> the new <code>SortKeys</code>; <code>null</code>
		'''        is a shorthand for specifying an empty list,
		'''        indicating that the view should be unsorted </param>
		''' <exception cref="IllegalArgumentException"> if any of the values in
		'''         <code>sortKeys</code> are null or have a column index outside
		'''         the range of the model </exception>
		Public Overridable Property sortKeys(Of T1 As SortKey) As IList(Of T1)
			Set(ByVal sortKeys As IList(Of T1))
				Dim old As IList(Of SortKey) = Me.sortKeys
				If sortKeys IsNot Nothing AndAlso sortKeys.Count > 0 Then
					Dim max As Integer = modelWrapper.columnCount
					For Each key As SortKey In sortKeys
						If key Is Nothing OrElse key.column < 0 OrElse key.column >= max Then Throw New System.ArgumentException("Invalid SortKey")
					Next key
					Me.sortKeys = java.util.Collections.unmodifiableList(New List(Of SortKey)(sortKeys))
				Else
					Me.sortKeys = java.util.Collections.emptyList()
				End If
				If Not Me.sortKeys.Equals(old) Then
					fireSortOrderChanged()
					If viewToModel Is Nothing Then
						' Currently unsorted, use sort so that internal fields
						' are correctly set.
						sort()
					Else
						sortExistingData()
					End If
				End If
			End Set
			Get
				Return sortKeys
			End Get
		End Property

		''' <summary>
		''' Returns the current sort keys.  This returns an unmodifiable
		''' {@code non-null List}. If you need to change the sort keys,
		''' make a copy of the returned {@code List}, mutate the copy
		''' and invoke {@code setSortKeys} with the new list.
		''' </summary>
		''' <returns> the current sort order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Sets the maximum number of sort keys.  The number of sort keys
		''' determines how equal values are resolved when sorting.  For
		''' example, assume a table row sorter is created and
		''' <code>setMaxSortKeys(2)</code> is invoked on it. The user
		''' clicks the header for column 1, causing the table rows to be
		''' sorted based on the items in column 1.  Next, the user clicks
		''' the header for column 2, causing the table to be sorted based
		''' on the items in column 2; if any items in column 2 are equal,
		''' then those particular rows are ordered based on the items in
		''' column 1. In this case, we say that the rows are primarily
		''' sorted on column 2, and secondarily on column 1.  If the user
		''' then clicks the header for column 3, then the items are
		''' primarily sorted on column 3 and secondarily sorted on column
		''' 2.  Because the maximum number of sort keys has been set to 2
		''' with <code>setMaxSortKeys</code>, column 1 no longer has an
		''' effect on the order.
		''' <p>
		''' The maximum number of sort keys is enforced by
		''' <code>toggleSortOrder</code>.  You can specify more sort
		''' keys by invoking <code>setSortKeys</code> directly and they will
		''' all be honored.  However if <code>toggleSortOrder</code> is subsequently
		''' invoked the maximum number of sort keys will be enforced.
		''' The default value is 3.
		''' </summary>
		''' <param name="max"> the maximum number of sort keys </param>
		''' <exception cref="IllegalArgumentException"> if <code>max</code> &lt; 1 </exception>
		Public Overridable Property maxSortKeys As Integer
			Set(ByVal max As Integer)
				If max < 1 Then Throw New System.ArgumentException("Invalid max")
				maxSortKeys = max
			End Set
			Get
				Return maxSortKeys
			End Get
		End Property


		''' <summary>
		''' If true, specifies that a sort should happen when the underlying
		''' model is updated (<code>rowsUpdated</code> is invoked).  For
		''' example, if this is true and the user edits an entry the
		''' location of that item in the view may change.  The default is
		''' false.
		''' </summary>
		''' <param name="sortsOnUpdates"> whether or not to sort on update events </param>
		Public Overridable Property sortsOnUpdates As Boolean
			Set(ByVal sortsOnUpdates As Boolean)
				Me.sortsOnUpdates = sortsOnUpdates
			End Set
			Get
				Return sortsOnUpdates
			End Get
		End Property


		''' <summary>
		''' Sets the filter that determines which rows, if any, should be
		''' hidden from the view.  The filter is applied before sorting.  A value
		''' of <code>null</code> indicates all values from the model should be
		''' included.
		''' <p>
		''' <code>RowFilter</code>'s <code>include</code> method is passed an
		''' <code>Entry</code> that wraps the underlying model.  The number
		''' of columns in the <code>Entry</code> corresponds to the
		''' number of columns in the <code>ModelWrapper</code>.  The identifier
		''' comes from the <code>ModelWrapper</code> as well.
		''' <p>
		''' This method triggers a sort.
		''' </summary>
		''' <param name="filter"> the filter used to determine what entries should be
		'''        included </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Property rowFilter(Of T1) As RowFilter(Of T1)
			Set(ByVal filter As RowFilter(Of T1))
				Me.filter = filter
				sort()
			End Set
			Get
				Return filter
			End Get
		End Property

		''' <summary>
		''' Returns the filter that determines which rows, if any, should
		''' be hidden from view.
		''' </summary>
		''' <returns> the filter </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Reverses the sort order from ascending to descending (or
		''' descending to ascending) if the specified column is already the
		''' primary sorted column; otherwise, makes the specified column
		''' the primary sorted column, with an ascending sort order.  If
		''' the specified column is not sortable, this method has no
		''' effect.
		''' </summary>
		''' <param name="column"> index of the column to make the primary sorted column,
		'''        in terms of the underlying model </param>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <seealso cref= #setSortable(int,boolean) </seealso>
		''' <seealso cref= #setMaxSortKeys(int) </seealso>
		Public Overridable Sub toggleSortOrder(ByVal column As Integer)
			checkColumn(column)
			If isSortable(column) Then
				Dim keys As IList(Of SortKey) = New List(Of SortKey)(sortKeys)
				Dim sortKey As SortKey
				Dim sortIndex As Integer
				For sortIndex = keys.Count - 1 To 0 Step -1
					If keys(sortIndex).column = column Then Exit For
				Next sortIndex
				If sortIndex = -1 Then
					' Key doesn't exist
					sortKey = New SortKey(column, javax.swing.SortOrder.ASCENDING)
					keys.Insert(0, sortKey)
				ElseIf sortIndex = 0 Then
					' It's the primary sorting key, toggle it
					keys(0) = toggle(keys(0))
				Else
					' It's not the first, but was sorted on, remove old
					' entry, insert as first with ascending.
					keys.RemoveAt(sortIndex)
					keys.Insert(0, New SortKey(column, javax.swing.SortOrder.ASCENDING))
				End If
				If keys.Count > maxSortKeys Then keys = keys.subList(0, maxSortKeys)
				sortKeys = keys
			End If
		End Sub

		Private Function toggle(ByVal key As SortKey) As SortKey
			If key.sortOrder = javax.swing.SortOrder.ASCENDING Then Return New SortKey(key.column, javax.swing.SortOrder.DESCENDING)
			Return New SortKey(key.column, javax.swing.SortOrder.ASCENDING)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function convertRowIndexToView(ByVal index As Integer) As Integer
			If modelToView Is Nothing Then
				If index < 0 OrElse index >= modelWrapper.rowCount Then Throw New System.IndexOutOfRangeException("Invalid index")
				Return index
			End If
			Return modelToView(index)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Function convertRowIndexToModel(ByVal index As Integer) As Integer
			If viewToModel Is Nothing Then
				If index < 0 OrElse index >= modelWrapper.rowCount Then Throw New System.IndexOutOfRangeException("Invalid index")
				Return index
			End If
			Return viewToModel(index).modelIndex
		End Function

		Private Property unsorted As Boolean
			Get
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim keys As IList(Of ? As SortKey) = sortKeys
				Dim keySize As Integer = keys.Count
				Return (keySize = 0 OrElse keys(0).sortOrder = javax.swing.SortOrder.UNSORTED)
			End Get
		End Property

		''' <summary>
		''' Sorts the existing filtered data.  This should only be used if
		''' the filter hasn't changed.
		''' </summary>
		Private Sub sortExistingData()
			Dim lastViewToModel As Integer() = getViewToModelAsInts(viewToModel)

			updateUseToString()
			cacheSortKeys(sortKeys)

			If unsorted Then
				If rowFilter Is Nothing Then
					viewToModel = Nothing
					modelToView = Nothing
				Else
					Dim included As Integer = 0
					For i As Integer = 0 To modelToView.Length - 1
						If modelToView(i) <> -1 Then
							viewToModel(included).modelIndex = i
							modelToView(i) = included
							included += 1
						End If
					Next i
				End If
			Else
				' sort the data
				java.util.Arrays.sort(viewToModel)

				' Update the modelToView array
				modelToViewFromViewToModel = False
			End If
			fireRowSorterChanged(lastViewToModel)
		End Sub

		''' <summary>
		''' Sorts and filters the rows in the view based on the sort keys
		''' of the columns currently being sorted and the filter, if any,
		''' associated with this sorter.  An empty <code>sortKeys</code> list
		''' indicates that the view should unsorted, the same as the model.
		''' </summary>
		''' <seealso cref= #setRowFilter </seealso>
		''' <seealso cref= #setSortKeys </seealso>
		Public Overridable Sub sort()
			sorted = True
			Dim lastViewToModel As Integer() = getViewToModelAsInts(viewToModel)
			updateUseToString()
			If unsorted Then
				' Unsorted
				cachedSortKeys = New SortKey(){}
				If rowFilter Is Nothing Then
					' No filter & unsorted
					If viewToModel IsNot Nothing Then
						' sorted -> unsorted
						viewToModel = Nothing
						modelToView = Nothing
					Else
						' unsorted -> unsorted
						' No need to do anything.
						Return
					End If
				Else
					' There is filter, reset mappings
					initializeFilteredMapping()
				End If
			Else
				cacheSortKeys(sortKeys)

				If rowFilter IsNot Nothing Then
					initializeFilteredMapping()
				Else
					createModelToView(modelWrapper.rowCount)
					createViewToModel(modelWrapper.rowCount)
				End If

				' sort them
				java.util.Arrays.sort(viewToModel)

				' Update the modelToView array
				modelToViewFromViewToModel = False
			End If
			fireRowSorterChanged(lastViewToModel)
		End Sub

		''' <summary>
		''' Updates the useToString mapping before a sort.
		''' </summary>
		Private Sub updateUseToString()
			Dim i As Integer = modelWrapper.columnCount
			If ___useToString Is Nothing OrElse ___useToString.Length <> i Then ___useToString = New Boolean(i - 1){}
			For i = i - 1 To 0 Step -1
				___useToString(i) = useToString(i)
			Next i
		End Sub

		''' <summary>
		''' Resets the viewToModel and modelToView mappings based on
		''' the current Filter.
		''' </summary>
		Private Sub initializeFilteredMapping()
			Dim rowCount As Integer = modelWrapper.rowCount
			Dim i, j As Integer
			Dim excludedCount As Integer = 0

			' Update model -> view
			createModelToView(rowCount)
			For i = 0 To rowCount - 1
				If include(i) Then
					modelToView(i) = i - excludedCount
				Else
					modelToView(i) = -1
					excludedCount += 1
				End If
			Next i

			' Update view -> model
			createViewToModel(rowCount - excludedCount)
			i = 0
			j = 0
			Do While i < rowCount
				If modelToView(i) <> -1 Then
					viewToModel(j).modelIndex = i
					j += 1
				End If
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Makes sure the modelToView array is of size rowCount.
		''' </summary>
		Private Sub createModelToView(ByVal rowCount As Integer)
			If modelToView Is Nothing OrElse modelToView.Length <> rowCount Then modelToView = New Integer(rowCount - 1){}
		End Sub

		''' <summary>
		''' Resets the viewToModel array to be of size rowCount.
		''' </summary>
		Private Sub createViewToModel(ByVal rowCount As Integer)
			Dim recreateFrom As Integer = 0
			If viewToModel IsNot Nothing Then
				recreateFrom = Math.Min(rowCount, viewToModel.Length)
				If viewToModel.Length <> rowCount Then
					Dim oldViewToModel As Row() = viewToModel
					viewToModel = New Row(rowCount - 1){}
					Array.Copy(oldViewToModel, 0, viewToModel, 0, recreateFrom)
				End If
			Else
				viewToModel = New Row(rowCount - 1){}
			End If
			Dim i As Integer
			For i = 0 To recreateFrom - 1
				viewToModel(i).modelIndex = i
			Next i
			For i = recreateFrom To rowCount - 1
				viewToModel(i) = New Row(Me, i)
			Next i
		End Sub

		''' <summary>
		''' Caches the sort keys before a sort.
		''' </summary>
		Private Sub cacheSortKeys(Of T1 As SortKey)(ByVal keys As IList(Of T1))
			Dim keySize As Integer = keys.Count
			sortComparators = New IComparer(keySize - 1){}
			For i As Integer = 0 To keySize - 1
				sortComparators(i) = getComparator0(keys(i).column)
			Next i
			cachedSortKeys = keys.ToArray()
		End Sub

		''' <summary>
		''' Returns whether or not to convert the value to a string before
		''' doing comparisons when sorting.  If true
		''' <code>ModelWrapper.getStringValueAt</code> will be used, otherwise
		''' <code>ModelWrapper.getValueAt</code> will be used.  It is up to
		''' subclasses, such as <code>TableRowSorter</code>, to honor this value
		''' in their <code>ModelWrapper</code> implementation.
		''' </summary>
		''' <param name="column"> the index of the column to test, in terms of the
		'''        underlying model </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>column</code> is not valid </exception>
		Protected Friend Overridable Function useToString(ByVal column As Integer) As Boolean
			Return (getComparator(column) Is Nothing)
		End Function

		''' <summary>
		''' Refreshes the modelToView mapping from that of viewToModel.
		''' If <code>unsetFirst</code> is true, all indices in modelToView are
		''' first set to -1.
		''' </summary>
		Private Property modelToViewFromViewToModel As Boolean
			Set(ByVal unsetFirst As Boolean)
				Dim i As Integer
				If unsetFirst Then
					For i = modelToView.Length - 1 To 0 Step -1
						modelToView(i) = -1
					Next i
				End If
				For i = viewToModel.Length - 1 To 0 Step -1
					modelToView(viewToModel(i).modelIndex) = i
				Next i
			End Set
		End Property

		Private Function getViewToModelAsInts(ByVal viewToModel As Row()) As Integer()
			If viewToModel IsNot Nothing Then
				Dim viewToModelI As Integer() = New Integer(viewToModel.Length - 1){}
				For i As Integer = viewToModel.Length - 1 To 0 Step -1
					viewToModelI(i) = viewToModel(i).modelIndex
				Next i
				Return viewToModelI
			End If
			Return New Integer(){}
		End Function

		''' <summary>
		''' Sets the <code>Comparator</code> to use when sorting the specified
		''' column.  This does not trigger a sort.  If you want to sort after
		''' setting the comparator you need to explicitly invoke <code>sort</code>.
		''' </summary>
		''' <param name="column"> the index of the column the <code>Comparator</code> is
		'''        to be used for, in terms of the underlying model </param>
		''' <param name="comparator"> the <code>Comparator</code> to use </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>column</code> is outside
		'''         the range of the underlying model </exception>
		Public Overridable Sub setComparator(Of T1)(ByVal column As Integer, ByVal comparator As IComparer(Of T1))
			checkColumn(column)
			If comparators Is Nothing Then comparators = New IComparer(modelWrapper.columnCount - 1){}
			comparators(column) = comparator
		End Sub

		''' <summary>
		''' Returns the <code>Comparator</code> for the specified
		''' column.  This will return <code>null</code> if a <code>Comparator</code>
		''' has not been specified for the column.
		''' </summary>
		''' <param name="column"> the column to fetch the <code>Comparator</code> for, in
		'''        terms of the underlying model </param>
		''' <returns> the <code>Comparator</code> for the specified column </returns>
		''' <exception cref="IndexOutOfBoundsException"> if column is outside
		'''         the range of the underlying model </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function getComparator(ByVal column As Integer) As IComparer(Of ?)
			checkColumn(column)
			If comparators IsNot Nothing Then Return comparators(column)
			Return Nothing
		End Function

		' Returns the Comparator to use during sorting.  Where as
		' getComparator() may return null, this will never return null.
		Private Function getComparator0(ByVal column As Integer) As IComparer
			Dim ___comparator As IComparer = getComparator(column)
			If ___comparator IsNot Nothing Then Return ___comparator
			' This should be ok as useToString(column) should have returned
			' true in this case.
			Return java.text.Collator.instance
		End Function

		Private Function getFilterEntry(ByVal modelIndex As Integer) As RowFilter.Entry(Of M, I)
			If filterEntry Is Nothing Then filterEntry = New FilterEntry(Me)
			filterEntry.modelIndex = modelIndex
			Return filterEntry
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property viewRowCount As Integer
			Get
				If viewToModel IsNot Nothing Then Return viewToModel.Length
				Return modelWrapper.rowCount
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property modelRowCount As Integer
			Get
				Return modelWrapper.rowCount
			End Get
		End Property

		Private Sub allChanged()
			modelToView = Nothing
			viewToModel = Nothing
			comparators = Nothing
			___isSortable = Nothing
			If unsorted Then
				' Keys are already empty, to force a resort we have to
				' call sort
				sort()
			Else
				sortKeys = Nothing
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub modelStructureChanged()
			allChanged()
			modelRowCount = modelWrapper.rowCount
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub allRowsChanged()
			modelRowCount = modelWrapper.rowCount
			sort()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub rowsInserted(ByVal firstRow As Integer, ByVal endRow As Integer)
			checkAgainstModel(firstRow, endRow)
			Dim newModelRowCount As Integer = modelWrapper.rowCount
			If endRow >= newModelRowCount Then Throw New System.IndexOutOfRangeException("Invalid range")
			modelRowCount = newModelRowCount
			If shouldOptimizeChange(firstRow, endRow) Then rowsInserted0(firstRow, endRow)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub rowsDeleted(ByVal firstRow As Integer, ByVal endRow As Integer)
			checkAgainstModel(firstRow, endRow)
			If firstRow >= modelRowCount OrElse endRow >= modelRowCount Then Throw New System.IndexOutOfRangeException("Invalid range")
			modelRowCount = modelWrapper.rowCount
			If shouldOptimizeChange(firstRow, endRow) Then rowsDeleted0(firstRow, endRow)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub rowsUpdated(ByVal firstRow As Integer, ByVal endRow As Integer)
			checkAgainstModel(firstRow, endRow)
			If firstRow >= modelRowCount OrElse endRow >= modelRowCount Then Throw New System.IndexOutOfRangeException("Invalid range")
			If sortsOnUpdates Then
				If shouldOptimizeChange(firstRow, endRow) Then rowsUpdated0(firstRow, endRow)
			Else
				sorted = False
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overridable Sub rowsUpdated(ByVal firstRow As Integer, ByVal endRow As Integer, ByVal column As Integer)
			checkColumn(column)
			rowsUpdated(firstRow, endRow)
		End Sub

		Private Sub checkAgainstModel(ByVal firstRow As Integer, ByVal endRow As Integer)
			If firstRow > endRow OrElse firstRow < 0 OrElse endRow < 0 OrElse firstRow > modelRowCount Then Throw New System.IndexOutOfRangeException("Invalid range")
		End Sub

		''' <summary>
		''' Returns true if the specified row should be included.
		''' </summary>
		Private Function include(ByVal row As Integer) As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim filter As RowFilter(Of ?, ?) = rowFilter
			If filter IsNot Nothing Then Return filter.include(getFilterEntry(row))
			' null filter, always include the row.
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function compare(ByVal model1 As Integer, ByVal model2 As Integer) As Integer
			Dim column As Integer
			Dim sortOrder As javax.swing.SortOrder
			Dim v1, v2 As Object
			Dim result As Integer

			For counter As Integer = 0 To cachedSortKeys.Length - 1
				column = cachedSortKeys(counter).column
				sortOrder = cachedSortKeys(counter).sortOrder
				If sortOrder = javax.swing.SortOrder.UNSORTED Then
					result = model1 - model2
				Else
					' v1 != null && v2 != null
					If ___useToString(column) Then
						v1 = modelWrapper.getStringValueAt(model1, column)
						v2 = modelWrapper.getStringValueAt(model2, column)
					Else
						v1 = modelWrapper.getValueAt(model1, column)
						v2 = modelWrapper.getValueAt(model2, column)
					End If
					' Treat nulls as < then non-null
					If v1 Is Nothing Then
						If v2 Is Nothing Then
							result = 0
						Else
							result = -1
						End If
					ElseIf v2 Is Nothing Then
						result = 1
					Else
						result = sortComparators(counter).Compare(v1, v2)
					End If
					If sortOrder = javax.swing.SortOrder.DESCENDING Then result *= -1
				End If
				If result <> 0 Then Return result
			Next counter
			' If we get here, they're equal. Fallback to model order.
			Return model1 - model2
		End Function

		''' <summary>
		''' Whether not we are filtering/sorting.
		''' </summary>
		Private Property transformed As Boolean
			Get
				Return (viewToModel IsNot Nothing)
			End Get
		End Property

		''' <summary>
		''' Insets new set of entries.
		''' </summary>
		''' <param name="toAdd"> the Rows to add, sorted </param>
		''' <param name="current"> the array to insert the items into </param>
		Private Sub insertInOrder(ByVal toAdd As IList(Of Row), ByVal current As Row())
			Dim last As Integer = 0
			Dim index As Integer
			Dim max As Integer = toAdd.Count
			For i As Integer = 0 To max - 1
				index = java.util.Arrays.binarySearch(current, toAdd(i))
				If index < 0 Then index = -1 - index
				Array.Copy(current, last, viewToModel, last + i, index - last)
				viewToModel(index + i) = toAdd(i)
				last = index
			Next i
			Array.Copy(current, last, viewToModel, last + max, current.Length - last)
		End Sub

		''' <summary>
		''' Returns true if we should try and optimize the processing of the
		''' <code>TableModelEvent</code>.  If this returns false, assume the
		''' event was dealt with and no further processing needs to happen.
		''' </summary>
		Private Function shouldOptimizeChange(ByVal firstRow As Integer, ByVal lastRow As Integer) As Boolean
			If Not transformed Then Return False
			If (Not sorted) OrElse (lastRow - firstRow) > viewToModel.Length \ 10 Then
				' We either weren't sorted, or to much changed, sort it all
				sort()
				Return False
			End If
			Return True
		End Function

		Private Sub rowsInserted0(ByVal firstRow As Integer, ByVal lastRow As Integer)
			Dim oldViewToModel As Integer() = getViewToModelAsInts(viewToModel)
			Dim i As Integer
			Dim delta As Integer = (lastRow - firstRow) + 1
			Dim added As IList(Of Row) = New List(Of Row)(delta)

			' Build the list of Rows to add into added
			For i = firstRow To lastRow
				If include(i) Then added.Add(New Row(Me, i))
			Next i

			' Adjust the model index of rows after the effected region
			Dim viewIndex As Integer
			For i = modelToView.Length - 1 To firstRow Step -1
				viewIndex = modelToView(i)
				If viewIndex <> -1 Then viewToModel(viewIndex).modelIndex += delta
			Next i

			' Insert newly added rows into viewToModel
			If added.Count > 0 Then
				java.util.Collections.sort(added)
				Dim lastViewToModel As Row() = viewToModel
				viewToModel = New Row(viewToModel.Length + added.Count - 1){}
				insertInOrder(added, lastViewToModel)
			End If

			' Update modelToView
			createModelToView(modelWrapper.rowCount)
			modelToViewFromViewToModel = True

			' Notify of change
			fireRowSorterChanged(oldViewToModel)
		End Sub

		Private Sub rowsDeleted0(ByVal firstRow As Integer, ByVal lastRow As Integer)
			Dim oldViewToModel As Integer() = getViewToModelAsInts(viewToModel)
			Dim removedFromView As Integer = 0
			Dim i As Integer
			Dim viewIndex As Integer

			' Figure out how many visible rows are going to be effected.
			For i = firstRow To lastRow
				viewIndex = modelToView(i)
				If viewIndex <> -1 Then
					removedFromView += 1
					viewToModel(viewIndex) = Nothing
				End If
			Next i

			' Update the model index of rows after the effected region
			Dim delta As Integer = lastRow - firstRow + 1
			For i = modelToView.Length - 1 To lastRow + 1 Step -1
				viewIndex = modelToView(i)
				If viewIndex <> -1 Then viewToModel(viewIndex).modelIndex -= delta
			Next i

			' Then patch up the viewToModel array
			If removedFromView > 0 Then
				Dim newViewToModel As Row() = New Row(viewToModel.Length - removedFromView - 1){}
				Dim newIndex As Integer = 0
				Dim last As Integer = 0
				For i = 0 To viewToModel.Length - 1
					If viewToModel(i) Is Nothing Then
						Array.Copy(viewToModel, last, newViewToModel, newIndex, i - last)
						newIndex += (i - last)
						last = i + 1
					End If
				Next i
				Array.Copy(viewToModel, last, newViewToModel, newIndex, viewToModel.Length - last)
				viewToModel = newViewToModel
			End If

			' Update the modelToView mapping
			createModelToView(modelWrapper.rowCount)
			modelToViewFromViewToModel = True

			' And notify of change
			fireRowSorterChanged(oldViewToModel)
		End Sub

		Private Sub rowsUpdated0(ByVal firstRow As Integer, ByVal lastRow As Integer)
			Dim oldViewToModel As Integer() = getViewToModelAsInts(viewToModel)
			Dim i, j As Integer
			Dim delta As Integer = lastRow - firstRow + 1
			Dim modelIndex As Integer
			Dim last As Integer
			Dim index As Integer

			If rowFilter Is Nothing Then
				' Sorting only:

				' Remove the effected rows
				Dim updated As Row() = New Row(delta - 1){}
				j = 0
				i = firstRow
				Do While i <= lastRow
					updated(j) = viewToModel(modelToView(i))
					i += 1
					j += 1
				Loop

				' Sort the update rows
				java.util.Arrays.sort(updated)

				' Build the intermediary array: the array of
				' viewToModel without the effected rows.
				Dim intermediary As Row() = New Row(viewToModel.Length - delta - 1){}
				i = 0
				j = 0
				Do While i < viewToModel.Length
					modelIndex = viewToModel(i).modelIndex
					If modelIndex < firstRow OrElse modelIndex > lastRow Then
						intermediary(j) = viewToModel(i)
						j += 1
					End If
					i += 1
				Loop

				' Build the new viewToModel
				insertInOrder(java.util.Arrays.asList(updated), intermediary)

				' Update modelToView
				modelToViewFromViewToModel = False
			Else
				' Sorting & filtering.

				' Remove the effected rows, adding them to updated and setting
				' modelToView to -2 for any rows that were not filtered out
				Dim updated As IList(Of Row) = New List(Of Row)(delta)
				Dim newlyVisible As Integer = 0
				Dim newlyHidden As Integer = 0
				Dim effected As Integer = 0
				For i = firstRow To lastRow
					If modelToView(i) = -1 Then
						' This row was filtered out
						If include(i) Then
							' No longer filtered
							updated.Add(New Row(Me, i))
							newlyVisible += 1
						End If
					Else
						' This row was visible, make sure it should still be
						' visible.
						If Not include(i) Then
							newlyHidden += 1
						Else
							updated.Add(viewToModel(modelToView(i)))
						End If
						modelToView(i) = -2
						effected += 1
					End If
				Next i

				' Sort the updated rows
				java.util.Collections.sort(updated)

				' Build the intermediary array: the array of
				' viewToModel without the updated rows.
				Dim intermediary As Row() = New Row(viewToModel.Length - effected - 1){}
				i = 0
				j = 0
				Do While i < viewToModel.Length
					modelIndex = viewToModel(i).modelIndex
					If modelToView(modelIndex) <> -2 Then
						intermediary(j) = viewToModel(i)
						j += 1
					End If
					i += 1
				Loop

				' Recreate viewToModel, if necessary
				If newlyVisible <> newlyHidden Then viewToModel = New Row(viewToModel.Length + newlyVisible - newlyHidden - 1){}

				' Rebuild the new viewToModel array
				insertInOrder(updated, intermediary)

				' Update modelToView
				modelToViewFromViewToModel = True
			End If
			' And finally fire a sort event.
			fireRowSorterChanged(oldViewToModel)
		End Sub

		Private Sub checkColumn(ByVal column As Integer)
			If column < 0 OrElse column >= modelWrapper.columnCount Then Throw New System.IndexOutOfRangeException("column beyond range of TableModel")
		End Sub


		''' <summary>
		''' <code>DefaultRowSorter.ModelWrapper</code> is responsible for providing
		''' the data that gets sorted by <code>DefaultRowSorter</code>.  You
		''' normally do not interact directly with <code>ModelWrapper</code>.
		''' Subclasses of <code>DefaultRowSorter</code> provide an
		''' implementation of <code>ModelWrapper</code> wrapping another model.
		''' For example,
		''' <code>TableRowSorter</code> provides a <code>ModelWrapper</code> that
		''' wraps a <code>TableModel</code>.
		''' <p>
		''' <code>ModelWrapper</code> makes a distinction between values as
		''' <code>Object</code>s and <code>String</code>s.  This allows
		''' implementations to provide a custom string
		''' converter to be used instead of invoking <code>toString</code> on the
		''' object.
		''' </summary>
		''' @param <M> the type of the underlying model </param>
		''' @param <I> the identifier supplied to the filter
		''' @since 1.6 </param>
		''' <seealso cref= RowFilter </seealso>
		''' <seealso cref= RowFilter.Entry </seealso>
		Protected Friend MustInherit Class ModelWrapper(Of M, I)
			''' <summary>
			''' Creates a new <code>ModelWrapper</code>.
			''' </summary>
			Protected Friend Sub New()
			End Sub

			''' <summary>
			''' Returns the underlying model that this <code>Model</code> is
			''' wrapping.
			''' </summary>
			''' <returns> the underlying model </returns>
			Public MustOverride ReadOnly Property model As M

			''' <summary>
			''' Returns the number of columns in the model.
			''' </summary>
			''' <returns> the number of columns in the model </returns>
			Public MustOverride ReadOnly Property columnCount As Integer

			''' <summary>
			''' Returns the number of rows in the model.
			''' </summary>
			''' <returns> the number of rows in the model </returns>
			Public MustOverride ReadOnly Property rowCount As Integer

			''' <summary>
			''' Returns the value at the specified index.
			''' </summary>
			''' <param name="row"> the row index </param>
			''' <param name="column"> the column index </param>
			''' <returns> the value at the specified index </returns>
			''' <exception cref="IndexOutOfBoundsException"> if the indices are outside
			'''         the range of the model </exception>
			Public MustOverride Function getValueAt(ByVal row As Integer, ByVal column As Integer) As Object

			''' <summary>
			''' Returns the value as a <code>String</code> at the specified
			''' index.  This implementation uses <code>toString</code> on
			''' the result from <code>getValueAt</code> (making sure
			''' to return an empty string for null values).  Subclasses that
			''' override this method should never return null.
			''' </summary>
			''' <param name="row"> the row index </param>
			''' <param name="column"> the column index </param>
			''' <returns> the value at the specified index as a <code>String</code> </returns>
			''' <exception cref="IndexOutOfBoundsException"> if the indices are outside
			'''         the range of the model </exception>
			Public Overridable Function getStringValueAt(ByVal row As Integer, ByVal column As Integer) As String
				Dim o As Object = getValueAt(row, column)
				If o Is Nothing Then Return ""
				Dim [string] As String = o.ToString()
				If [string] Is Nothing Then Return ""
				Return [string]
			End Function

			''' <summary>
			''' Returns the identifier for the specified row.  The return value
			''' of this is used as the identifier for the
			''' <code>RowFilter.Entry</code> that is passed to the
			''' <code>RowFilter</code>.
			''' </summary>
			''' <param name="row"> the row to return the identifier for, in terms of
			'''            the underlying model </param>
			''' <returns> the identifier </returns>
			''' <seealso cref= RowFilter.Entry#getIdentifier </seealso>
			Public MustOverride Function getIdentifier(ByVal row As Integer) As I
		End Class


		''' <summary>
		''' RowFilter.Entry implementation that delegates to the ModelWrapper.
		''' getFilterEntry(int) creates the single instance of this that is
		''' passed to the Filter.  Only call getFilterEntry(int) to get
		''' the instance.
		''' </summary>
		Private Class FilterEntry
			Inherits RowFilter.Entry(Of M, I)

			Private ReadOnly outerInstance As DefaultRowSorter

			Public Sub New(ByVal outerInstance As DefaultRowSorter)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' The index into the model, set in getFilterEntry
			''' </summary>
			Friend modelIndex As Integer

			Public Overridable Property model As M
				Get
					Return outerInstance.modelWrapper.model
				End Get
			End Property

			Public Overridable Property valueCount As Integer
				Get
					Return outerInstance.modelWrapper.columnCount
				End Get
			End Property

			Public Overridable Function getValue(ByVal index As Integer) As Object
				Return outerInstance.modelWrapper.getValueAt(modelIndex, index)
			End Function

			Public Overridable Function getStringValue(ByVal index As Integer) As String
				Return outerInstance.modelWrapper.getStringValueAt(modelIndex, index)
			End Function

			Public Overridable Property identifier As I
				Get
					Return outerInstance.modelWrapper.getIdentifier(modelIndex)
				End Get
			End Property
		End Class


		''' <summary>
		''' Row is used to handle the actual sorting by way of Comparable.  It
		''' will use the sortKeys to do the actual comparison.
		''' </summary>
		' NOTE: this class is static so that it can be placed in an array
		Private Class Row
			Implements IComparable(Of Row)

			Private sorter As DefaultRowSorter
			Friend modelIndex As Integer

			Public Sub New(ByVal sorter As DefaultRowSorter, ByVal index As Integer)
				Me.sorter = sorter
				modelIndex = index
			End Sub

			Public Overridable Function compareTo(ByVal o As Row) As Integer
				Return sorter.Compare(modelIndex, o.modelIndex)
			End Function
		End Class
	End Class

End Namespace