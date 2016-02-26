'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf



	''' <summary>
	''' The {@code JList} pluggable look and feel delegate.
	''' 
	''' @author Hans Muller
	''' </summary>

	Public MustInherit Class ListUI
		Inherits ComponentUI

		''' <summary>
		''' Returns the cell index in the specified {@code JList} closest to the
		''' given location in the list's coordinate system. To determine if the
		''' cell actually contains the specified location, compare the point against
		''' the cell's bounds, as provided by {@code getCellBounds}.
		''' This method returns {@code -1} if the list's model is empty.
		''' </summary>
		''' <param name="list"> the list </param>
		''' <param name="location"> the coordinates of the point </param>
		''' <returns> the cell index closest to the given location, or {@code -1} </returns>
		''' <exception cref="NullPointerException"> if {@code location} is null </exception>
		Public MustOverride Function locationToIndex(ByVal list As javax.swing.JList, ByVal location As java.awt.Point) As Integer


		''' <summary>
		''' Returns the origin in the given {@code JList}, of the specified item,
		''' in the list's coordinate system.
		''' Returns {@code null} if the index isn't valid.
		''' </summary>
		''' <param name="list"> the list </param>
		''' <param name="index"> the cell index </param>
		''' <returns> the origin of the cell, or {@code null} </returns>
		Public MustOverride Function indexToLocation(ByVal list As javax.swing.JList, ByVal index As Integer) As java.awt.Point


		''' <summary>
		''' Returns the bounding rectangle, in the given list's coordinate system,
		''' for the range of cells specified by the two indices.
		''' The indices can be supplied in any order.
		''' <p>
		''' If the smaller index is outside the list's range of cells, this method
		''' returns {@code null}. If the smaller index is valid, but the larger
		''' index is outside the list's range, the bounds of just the first index
		''' is returned. Otherwise, the bounds of the valid range is returned.
		''' </summary>
		''' <param name="list"> the list </param>
		''' <param name="index1"> the first index in the range </param>
		''' <param name="index2"> the second index in the range </param>
		''' <returns> the bounding rectangle for the range of cells, or {@code null} </returns>
		Public MustOverride Function getCellBounds(ByVal list As javax.swing.JList, ByVal index1 As Integer, ByVal index2 As Integer) As java.awt.Rectangle
	End Class

End Namespace