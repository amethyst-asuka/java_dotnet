Imports System.Collections.Generic
Imports javax.swing.event
Imports javax.swing

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
	''' Defines the requirements for a table column model object suitable for
	''' use with <code>JTable</code>.
	''' 
	''' @author Alan Chung
	''' @author Philip Milne </summary>
	''' <seealso cref= DefaultTableColumnModel </seealso>
	Public Interface TableColumnModel
	'
	' Modifying the model
	'

		''' <summary>
		'''  Appends <code>aColumn</code> to the end of the
		'''  <code>tableColumns</code> array.
		'''  This method posts a <code>columnAdded</code>
		'''  event to its listeners.
		''' </summary>
		''' <param name="aColumn">         the <code>TableColumn</code> to be added </param>
		''' <seealso cref=     #removeColumn </seealso>
		Sub addColumn(ByVal aColumn As TableColumn)

		''' <summary>
		'''  Deletes the <code>TableColumn</code> <code>column</code> from the
		'''  <code>tableColumns</code> array.  This method will do nothing if
		'''  <code>column</code> is not in the table's column list.
		'''  This method posts a <code>columnRemoved</code>
		'''  event to its listeners.
		''' </summary>
		''' <param name="column">          the <code>TableColumn</code> to be removed </param>
		''' <seealso cref=     #addColumn </seealso>
		Sub removeColumn(ByVal column As TableColumn)

		''' <summary>
		''' Moves the column and its header at <code>columnIndex</code> to
		''' <code>newIndex</code>.  The old column at <code>columnIndex</code>
		''' will now be found at <code>newIndex</code>.  The column that used
		''' to be at <code>newIndex</code> is shifted left or right
		''' to make room.  This will not move any columns if
		''' <code>columnIndex</code> equals <code>newIndex</code>.  This method
		''' posts a <code>columnMoved</code> event to its listeners.
		''' </summary>
		''' <param name="columnIndex">                     the index of column to be moved </param>
		''' <param name="newIndex">                        index of the column's new location </param>
		''' <exception cref="IllegalArgumentException">      if <code>columnIndex</code> or
		'''                                          <code>newIndex</code>
		'''                                          are not in the valid range </exception>
		Sub moveColumn(ByVal columnIndex As Integer, ByVal newIndex As Integer)

		''' <summary>
		''' Sets the <code>TableColumn</code>'s column margin to
		''' <code>newMargin</code>.  This method posts
		''' a <code>columnMarginChanged</code> event to its listeners.
		''' </summary>
		''' <param name="newMargin">       the width, in pixels, of the new column margins </param>
		''' <seealso cref=     #getColumnMargin </seealso>
		Property columnMargin As Integer

	'
	' Querying the model
	'

		''' <summary>
		''' Returns the number of columns in the model. </summary>
		''' <returns> the number of columns in the model </returns>
		ReadOnly Property columnCount As Integer

		''' <summary>
		''' Returns an <code>Enumeration</code> of all the columns in the model. </summary>
		''' <returns> an <code>Enumeration</code> of all the columns in the model </returns>
		ReadOnly Property columns As System.Collections.IEnumerator(Of TableColumn)

		''' <summary>
		''' Returns the index of the first column in the table
		''' whose identifier is equal to <code>identifier</code>,
		''' when compared using <code>equals</code>.
		''' </summary>
		''' <param name="columnIdentifier">        the identifier object </param>
		''' <returns>          the index of the first table column
		'''                  whose identifier is equal to <code>identifier</code> </returns>
		''' <exception cref="IllegalArgumentException">      if <code>identifier</code>
		'''                          is <code>null</code>, or no
		'''                          <code>TableColumn</code> has this
		'''                          <code>identifier</code> </exception>
		''' <seealso cref=             #getColumn </seealso>
		Function getColumnIndex(ByVal columnIdentifier As Object) As Integer

		''' <summary>
		''' Returns the <code>TableColumn</code> object for the column at
		''' <code>columnIndex</code>.
		''' </summary>
		''' <param name="columnIndex">     the index of the desired column </param>
		''' <returns>  the <code>TableColumn</code> object for
		'''                          the column at <code>columnIndex</code> </returns>
		Function getColumn(ByVal columnIndex As Integer) As TableColumn


		''' <summary>
		''' Returns the index of the column that lies on the
		''' horizontal point, <code>xPosition</code>;
		''' or -1 if it lies outside the any of the column's bounds.
		''' 
		''' In keeping with Swing's separable model architecture, a
		''' TableColumnModel does not know how the table columns actually appear on
		''' screen.  The visual presentation of the columns is the responsibility
		''' of the view/controller object using this model (typically JTable).  The
		''' view/controller need not display the columns sequentially from left to
		''' right.  For example, columns could be displayed from right to left to
		''' accommodate a locale preference or some columns might be hidden at the
		''' request of the user.  Because the model does not know how the columns
		''' are laid out on screen, the given <code>xPosition</code> should not be
		''' considered to be a coordinate in 2D graphics space.  Instead, it should
		''' be considered to be a width from the start of the first column in the
		''' model.  If the column index for a given X coordinate in 2D space is
		''' required, <code>JTable.columnAtPoint</code> can be used instead.
		''' </summary>
		''' <returns>  the index of the column; or -1 if no column is found </returns>
		''' <seealso cref= javax.swing.JTable#columnAtPoint </seealso>
		Function getColumnIndexAtX(ByVal xPosition As Integer) As Integer

		''' <summary>
		''' Returns the total width of all the columns. </summary>
		''' <returns> the total computed width of all columns </returns>
		ReadOnly Property totalColumnWidth As Integer

	'
	' Selection
	'

		''' <summary>
		''' Sets whether the columns in this model may be selected. </summary>
		''' <param name="flag">   true if columns may be selected; otherwise false </param>
		''' <seealso cref= #getColumnSelectionAllowed </seealso>
		Property columnSelectionAllowed As Boolean


		''' <summary>
		''' Returns an array of indicies of all selected columns. </summary>
		''' <returns> an array of integers containing the indicies of all
		'''          selected columns; or an empty array if nothing is selected </returns>
		ReadOnly Property selectedColumns As Integer()

		''' <summary>
		''' Returns the number of selected columns.
		''' </summary>
		''' <returns> the number of selected columns; or 0 if no columns are selected </returns>
		ReadOnly Property selectedColumnCount As Integer

		''' <summary>
		''' Sets the selection model.
		''' </summary>
		''' <param name="newModel">  a <code>ListSelectionModel</code> object </param>
		''' <seealso cref= #getSelectionModel </seealso>
		Property selectionModel As ListSelectionModel


	'
	' Listener
	'

		''' <summary>
		''' Adds a listener for table column model events.
		''' </summary>
		''' <param name="x">  a <code>TableColumnModelListener</code> object </param>
		Sub addColumnModelListener(ByVal x As TableColumnModelListener)

		''' <summary>
		''' Removes a listener for table column model events.
		''' </summary>
		''' <param name="x">  a <code>TableColumnModelListener</code> object </param>
		Sub removeColumnModelListener(ByVal x As TableColumnModelListener)
	End Interface

End Namespace