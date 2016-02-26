Imports System.Collections.Generic
Imports javax.swing.event

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
Namespace javax.swing


	''' <summary>
	''' <code>RowSorter</code> provides the basis for sorting and filtering.
	''' Beyond creating and installing a <code>RowSorter</code>, you very rarely
	''' need to interact with one directly.  Refer to
	''' <seealso cref="javax.swing.table.TableRowSorter TableRowSorter"/> for a concrete
	''' implementation of <code>RowSorter</code> for <code>JTable</code>.
	''' <p>
	''' <code>RowSorter</code>'s primary role is to provide a mapping between
	''' two coordinate systems: that of the view (for example a
	''' <code>JTable</code>) and that of the underlying data source, typically a
	''' model.
	''' <p>
	''' The view invokes the following methods on the <code>RowSorter</code>:
	''' <ul>
	''' <li><code>toggleSortOrder</code> &#151; The view invokes this when the
	'''     appropriate user gesture has occurred to trigger a sort.  For example,
	'''     the user clicked a column header in a table.
	''' <li>One of the model change methods &#151; The view invokes a model
	'''     change method when the underlying model
	'''     has changed.  There may be order dependencies in how the events are
	'''     delivered, so a <code>RowSorter</code> should not update its mapping
	'''     until one of these methods is invoked.
	''' </ul>
	''' Because the view makes extensive use of  the
	''' <code>convertRowIndexToModel</code>,
	''' <code>convertRowIndexToView</code> and <code>getViewRowCount</code> methods,
	''' these methods need to be fast.
	''' <p>
	''' <code>RowSorter</code> provides notification of changes by way of
	''' <code>RowSorterListener</code>.  Two types of notification are sent:
	''' <ul>
	''' <li><code>RowSorterEvent.Type.SORT_ORDER_CHANGED</code> &#151; notifies
	'''     listeners that the sort order has changed.  This is typically followed
	'''     by a notification that the sort has changed.
	''' <li><code>RowSorterEvent.Type.SORTED</code> &#151; notifies listeners that
	'''     the mapping maintained by the <code>RowSorter</code> has changed in
	'''     some way.
	''' </ul>
	''' <code>RowSorter</code> implementations typically don't have a one-to-one
	''' mapping with the underlying model, but they can.
	''' For example, if a database does the sorting,
	''' <code>toggleSortOrder</code> might call through to the database
	''' (on a background thread), and override the mapping methods to return the
	''' argument that is passed in.
	''' <p>
	''' Concrete implementations of <code>RowSorter</code>
	''' need to reference a model such as <code>TableModel</code> or
	''' <code>ListModel</code>.  The view classes, such as
	''' <code>JTable</code> and <code>JList</code>, will also have a
	''' reference to the model.  To avoid ordering dependencies,
	''' <code>RowSorter</code> implementations should not install a
	''' listener on the model.  Instead the view class will call into the
	''' <code>RowSorter</code> when the model changes.  For
	''' example, if a row is updated in a <code>TableModel</code>
	''' <code>JTable</code> invokes <code>rowsUpdated</code>.
	''' When the model changes, the view may call into any of the following methods:
	''' <code>modelStructureChanged</code>, <code>allRowsChanged</code>,
	''' <code>rowsInserted</code>, <code>rowsDeleted</code> and
	''' <code>rowsUpdated</code>.
	''' </summary>
	''' @param <M> the type of the underlying model </param>
	''' <seealso cref= javax.swing.table.TableRowSorter
	''' @since 1.6 </seealso>
	Public MustInherit Class RowSorter(Of M)
		Private listenerList As New EventListenerList

		''' <summary>
		''' Creates a <code>RowSorter</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the underlying model.
		''' </summary>
		''' <returns> the underlying model </returns>
		Public MustOverride ReadOnly Property model As M

		''' <summary>
		''' Reverses the sort order of the specified column.  It is up to
		''' subclasses to provide the exact behavior when invoked.  Typically
		''' this will reverse the sort order from ascending to descending (or
		''' descending to ascending) if the specified column is already the
		''' primary sorted column; otherwise, makes the specified column
		''' the primary sorted column, with an ascending sort order.  If
		''' the specified column is not sortable, this method has no
		''' effect.
		''' <p>
		''' If this results in changing the sort order and sorting, the
		''' appropriate <code>RowSorterListener</code> notification will be
		''' sent.
		''' </summary>
		''' <param name="column"> the column to toggle the sort ordering of, in
		'''        terms of the underlying model </param>
		''' <exception cref="IndexOutOfBoundsException"> if column is outside the range of
		'''         the underlying model </exception>
		Public MustOverride Sub toggleSortOrder(ByVal column As Integer)

		''' <summary>
		''' Returns the location of <code>index</code> in terms of the
		''' underlying model.  That is, for the row <code>index</code> in
		''' the coordinates of the view this returns the row index in terms
		''' of the underlying model.
		''' </summary>
		''' <param name="index"> the row index in terms of the underlying view </param>
		''' <returns> row index in terms of the view </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>index</code> is outside the
		'''         range of the view </exception>
		Public MustOverride Function convertRowIndexToModel(ByVal index As Integer) As Integer

		''' <summary>
		''' Returns the location of <code>index</code> in terms of the
		''' view.  That is, for the row <code>index</code> in the
		''' coordinates of the underlying model this returns the row index
		''' in terms of the view.
		''' </summary>
		''' <param name="index"> the row index in terms of the underlying model </param>
		''' <returns> row index in terms of the view, or -1 if index has been
		'''         filtered out of the view </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>index</code> is outside
		'''         the range of the model </exception>
		Public MustOverride Function convertRowIndexToView(ByVal index As Integer) As Integer

		''' <summary>
		''' Sets the current sort keys.
		''' </summary>
		''' <param name="keys"> the new <code>SortKeys</code>; <code>null</code>
		'''        is a shorthand for specifying an empty list,
		'''        indicating that the view should be unsorted </param>
		Public MustOverride Property sortKeys(Of T1 As SortKey) As IList(Of T1)

		''' <summary>
		''' Returns the current sort keys.  This must return a {@code
		''' non-null List} and may return an unmodifiable {@code List}. If
		''' you need to change the sort keys, make a copy of the returned
		''' {@code List}, mutate the copy and invoke {@code setSortKeys}
		''' with the new list.
		''' </summary>
		''' <returns> the current sort order </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Returns the number of rows in the view.  If the contents have
		''' been filtered this might differ from the row count of the
		''' underlying model.
		''' </summary>
		''' <returns> number of rows in the view </returns>
		''' <seealso cref= #getModelRowCount </seealso>
		Public MustOverride ReadOnly Property viewRowCount As Integer

		''' <summary>
		''' Returns the number of rows in the underlying model.
		''' </summary>
		''' <returns> number of rows in the underlying model </returns>
		''' <seealso cref= #getViewRowCount </seealso>
		Public MustOverride ReadOnly Property modelRowCount As Integer

		''' <summary>
		''' Invoked when the underlying model structure has completely
		''' changed.  For example, if the number of columns in a
		''' <code>TableModel</code> changed, this method would be invoked.
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		Public MustOverride Sub modelStructureChanged()

		''' <summary>
		''' Invoked when the contents of the underlying model have
		''' completely changed. The structure of the table is the same,
		''' only the contents have changed. This is typically sent when it
		''' is too expensive to characterize the change in terms of the
		''' other methods.
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		Public MustOverride Sub allRowsChanged()

		''' <summary>
		''' Invoked when rows have been inserted into the underlying model
		''' in the specified range (inclusive).
		''' <p>
		''' The arguments give the indices of the effected range.
		''' The first argument is in terms of the model before the change, and
		''' must be less than or equal to the size of the model before the change.
		''' The second argument is in terms of the model after the change and must
		''' be less than the size of the model after the change. For example,
		''' if you have a 5-row model and add 3 items to the end of the model
		''' the indices are 5, 7.
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		''' <param name="firstRow"> the first row </param>
		''' <param name="endRow"> the last row </param>
		''' <exception cref="IndexOutOfBoundsException"> if either argument is invalid, or
		'''         <code>firstRow</code> &gt; <code>endRow</code> </exception>
		Public MustOverride Sub rowsInserted(ByVal firstRow As Integer, ByVal endRow As Integer)

		''' <summary>
		''' Invoked when rows have been deleted from the underlying model
		''' in the specified range (inclusive).
		''' <p>
		''' The arguments give the indices of the effected range and
		''' are in terms of the model <b>before</b> the change.
		''' For example, if you have a 5-row model and delete 3 items from the end
		''' of the model the indices are 2, 4.
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		''' <param name="firstRow"> the first row </param>
		''' <param name="endRow"> the last row </param>
		''' <exception cref="IndexOutOfBoundsException"> if either argument is outside
		'''         the range of the model before the change, or
		'''         <code>firstRow</code> &gt; <code>endRow</code> </exception>
		Public MustOverride Sub rowsDeleted(ByVal firstRow As Integer, ByVal endRow As Integer)

		''' <summary>
		''' Invoked when rows have been changed in the underlying model
		''' between the specified range (inclusive).
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		''' <param name="firstRow"> the first row, in terms of the underlying model </param>
		''' <param name="endRow"> the last row, in terms of the underlying model </param>
		''' <exception cref="IndexOutOfBoundsException"> if either argument is outside
		'''         the range of the underlying model, or
		'''         <code>firstRow</code> &gt; <code>endRow</code> </exception>
		Public MustOverride Sub rowsUpdated(ByVal firstRow As Integer, ByVal endRow As Integer)

		''' <summary>
		''' Invoked when the column in the rows have been updated in
		''' the underlying model between the specified range.
		''' <p>
		''' You normally do not call this method.  This method is public
		''' to allow view classes to call it.
		''' </summary>
		''' <param name="firstRow"> the first row, in terms of the underlying model </param>
		''' <param name="endRow"> the last row, in terms of the underlying model </param>
		''' <param name="column"> the column that has changed, in terms of the underlying
		'''        model </param>
		''' <exception cref="IndexOutOfBoundsException"> if either argument is outside
		'''         the range of the underlying model after the change,
		'''         <code>firstRow</code> &gt; <code>endRow</code>, or
		'''         <code>column</code> is outside the range of the underlying
		'''          model </exception>
		Public MustOverride Sub rowsUpdated(ByVal firstRow As Integer, ByVal endRow As Integer, ByVal column As Integer)

		''' <summary>
		''' Adds a <code>RowSorterListener</code> to receive notification
		''' about this <code>RowSorter</code>.  If the same
		''' listener is added more than once it will receive multiple
		''' notifications.  If <code>l</code> is <code>null</code> nothing
		''' is done.
		''' </summary>
		''' <param name="l"> the <code>RowSorterListener</code> </param>
		Public Overridable Sub addRowSorterListener(ByVal l As RowSorterListener)
			listenerList.add(GetType(RowSorterListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>RowSorterListener</code>.  If
		''' <code>l</code> is <code>null</code> nothing is done.
		''' </summary>
		''' <param name="l"> the <code>RowSorterListener</code> </param>
		Public Overridable Sub removeRowSorterListener(ByVal l As RowSorterListener)
			listenerList.remove(GetType(RowSorterListener), l)
		End Sub

		''' <summary>
		''' Notifies listener that the sort order has changed.
		''' </summary>
		Protected Friend Overridable Sub fireSortOrderChanged()
			fireRowSorterChanged(New RowSorterEvent(Me))
		End Sub

		''' <summary>
		''' Notifies listener that the mapping has changed.
		''' </summary>
		''' <param name="lastRowIndexToModel"> the mapping from model indices to
		'''        view indices prior to the sort, may be <code>null</code> </param>
		Protected Friend Overridable Sub fireRowSorterChanged(ByVal lastRowIndexToModel As Integer())
			fireRowSorterChanged(New RowSorterEvent(Me, RowSorterEvent.Type.SORTED, lastRowIndexToModel))
		End Sub

		Friend Overridable Sub fireRowSorterChanged(ByVal [event] As RowSorterEvent)
			Dim listeners As Object() = listenerList.listenerList
			For i As Integer = listeners.Length - 2 To 0 Step -2
				If listeners(i) Is GetType(RowSorterListener) Then CType(listeners(i + 1), RowSorterListener).sorterChanged([event])
			Next i
		End Sub

		''' <summary>
		''' SortKey describes the sort order for a particular column.  The
		''' column index is in terms of the underlying model, which may differ
		''' from that of the view.
		''' 
		''' @since 1.6
		''' </summary>
		Public Class SortKey
			Private column As Integer
			Private sortOrder As javax.swing.SortOrder

			''' <summary>
			''' Creates a <code>SortKey</code> for the specified column with
			''' the specified sort order.
			''' </summary>
			''' <param name="column"> index of the column, in terms of the model </param>
			''' <param name="sortOrder"> the sorter order </param>
			''' <exception cref="IllegalArgumentException"> if <code>sortOrder</code> is
			'''         <code>null</code> </exception>
			Public Sub New(ByVal column As Integer, ByVal sortOrder As javax.swing.SortOrder)
				If sortOrder Is Nothing Then Throw New System.ArgumentException("sort order must be non-null")
				Me.column = column
				Me.sortOrder = sortOrder
			End Sub

			''' <summary>
			''' Returns the index of the column.
			''' </summary>
			''' <returns> index of column </returns>
			Public Property column As Integer
				Get
					Return column
				End Get
			End Property

			''' <summary>
			''' Returns the sort order of the column.
			''' </summary>
			''' <returns> the sort order of the column </returns>
			Public Property sortOrder As javax.swing.SortOrder
				Get
					Return sortOrder
				End Get
			End Property

			''' <summary>
			''' Returns the hash code for this <code>SortKey</code>.
			''' </summary>
			''' <returns> hash code </returns>
			Public Overrides Function GetHashCode() As Integer
				Dim result As Integer = 17
				result = 37 * result + column
				result = 37 * result + sortOrder.GetHashCode()
				Return result
			End Function

			''' <summary>
			''' Returns true if this object equals the specified object.
			''' If the specified object is a <code>SortKey</code> and
			''' references the same column and sort order, the two objects
			''' are equal.
			''' </summary>
			''' <param name="o"> the object to compare to </param>
			''' <returns> true if <code>o</code> is equal to this <code>SortKey</code> </returns>
			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If o Is Me Then Return True
				If TypeOf o Is SortKey Then Return (CType(o, SortKey).column = column AndAlso CType(o, SortKey).sortOrder = sortOrder)
				Return False
			End Function
		End Class
	End Class

End Namespace