'
' * Copyright (c) 1995, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt.peer


	''' <summary>
	''' The peer interface for <seealso cref="List"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface ListPeer
		Inherits ComponentPeer

		''' <summary>
		''' Returns the indices of the list items that are currently selected.
		''' The returned array is not required to be a copy, the callers of this
		''' method already make sure it is not modified.
		''' </summary>
		''' <returns> the indices of the list items that are currently selected
		''' </returns>
		''' <seealso cref= List#getSelectedIndexes() </seealso>
		ReadOnly Property selectedIndexes As Integer()

		''' <summary>
		''' Adds an item to the list at the specified index.
		''' </summary>
		''' <param name="item"> the item to add to the list </param>
		''' <param name="index"> the index where to add the item into the list
		''' </param>
		''' <seealso cref= List#add(String, int) </seealso>
		Sub add(  item As String,   index As Integer)

		''' <summary>
		''' Deletes items from the list. All items from start to end should are
		''' deleted, including the item at the start and end indices.
		''' </summary>
		''' <param name="start"> the first item to be deleted </param>
		''' <param name="end"> the last item to be deleted </param>
		Sub delItems(  start As Integer,   [end] As Integer)

		''' <summary>
		''' Removes all items from the list.
		''' </summary>
		''' <seealso cref= List#removeAll() </seealso>
		Sub removeAll()

		''' <summary>
		''' Selects the item at the specified {@code index}.
		''' </summary>
		''' <param name="index"> the index of the item to select
		''' </param>
		''' <seealso cref= List#select(int) </seealso>
		Sub [select](  index As Integer)

		''' <summary>
		''' De-selects the item at the specified {@code index}.
		''' </summary>
		''' <param name="index"> the index of the item to de-select
		''' </param>
		''' <seealso cref= List#deselect(int) </seealso>
		Sub deselect(  index As Integer)

		''' <summary>
		''' Makes sure that the item at the specified {@code index} is visible,
		''' by scrolling the list or similar.
		''' </summary>
		''' <param name="index"> the index of the item to make visible
		''' </param>
		''' <seealso cref= List#makeVisible(int) </seealso>
		Sub makeVisible(  index As Integer)

		''' <summary>
		''' Toggles multiple selection mode on or off.
		''' </summary>
		''' <param name="m"> {@code true} for multiple selection mode,
		'''        {@code false} for single selection mode
		''' </param>
		''' <seealso cref= List#setMultipleMode(boolean) </seealso>
		WriteOnly Property multipleMode As Boolean

		''' <summary>
		''' Returns the preferred size for a list with the specified number of rows.
		''' </summary>
		''' <param name="rows"> the number of rows
		''' </param>
		''' <returns> the preferred size of the list
		''' </returns>
		''' <seealso cref= List#getPreferredSize(int) </seealso>
		Function getPreferredSize(  rows As Integer) As java.awt.Dimension

		''' <summary>
		''' Returns the minimum size for a list with the specified number of rows.
		''' </summary>
		''' <param name="rows"> the number of rows
		''' </param>
		''' <returns> the minimum size of the list
		''' </returns>
		''' <seealso cref= List#getMinimumSize(int) </seealso>
		Function getMinimumSize(  rows As Integer) As java.awt.Dimension

	End Interface

End Namespace