'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2000 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE. See W3C License http://www.w3.org/Consortium/Legal/ for more
' * details.
' 

Namespace org.w3c.dom.html


	''' <summary>
	'''  A row in a table. See the  TR element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLTableRowElement
		Inherits HTMLElement

		''' <summary>
		'''  The index of this row, relative to the entire table, starting from 0.
		''' This is in document tree order and not display order. The
		''' <code>rowIndex</code> does not take into account sections (
		''' <code>THEAD</code> , <code>TFOOT</code> , or <code>TBODY</code> )
		''' within the table.
		''' </summary>
		ReadOnly Property rowIndex As Integer

		''' <summary>
		'''  The index of this row, relative to the current section (
		''' <code>THEAD</code> , <code>TFOOT</code> , or <code>TBODY</code> ),
		''' starting from 0.
		''' </summary>
		ReadOnly Property sectionRowIndex As Integer

		''' <summary>
		'''  The collection of cells in this row.
		''' </summary>
		ReadOnly Property cells As HTMLCollection

		''' <summary>
		'''  Horizontal alignment of data within cells of this row. See the  align
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Background color for rows. See the  bgcolor attribute definition in
		''' HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property bgColor As String

		''' <summary>
		'''  Alignment character for cells in a column. See the  char attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property ch As String

		''' <summary>
		'''  Offset of alignment character. See the  charoff attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property chOff As String

		''' <summary>
		'''  Vertical alignment of data within cells of this row. See the  valign
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property vAlign As String

		''' <summary>
		'''  Insert an empty <code>TD</code> cell into this row. If
		''' <code>index</code> is equal to the number of cells, the new cell is
		''' appended </summary>
		''' <param name="index">  The place to insert the cell, starting from 0. </param>
		''' <returns>  The newly created cell. </returns>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified <code>index</code> is
		'''   greater than the number of cells or if the index is negative. </exception>
		Function insertCell(ByVal index As Integer) As HTMLElement

		''' <summary>
		'''  Delete a cell from the current row. </summary>
		''' <param name="index">  The index of the cell to delete, starting from 0. </param>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified <code>index</code> is
		'''   greater than or equal to the number of cells or if the index is
		'''   negative. </exception>
		Sub deleteCell(ByVal index As Integer)

	End Interface

End Namespace