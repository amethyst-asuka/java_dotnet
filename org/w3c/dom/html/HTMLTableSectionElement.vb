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
	'''  The <code>THEAD</code> , <code>TFOOT</code> , and <code>TBODY</code>
	''' elements.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLTableSectionElement
		Inherits HTMLElement

		''' <summary>
		'''  Horizontal alignment of data in cells. See the <code>align</code>
		''' attribute for HTMLTheadElement for details.
		''' </summary>
		Property align As String

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
		'''  Vertical alignment of data in cells. See the <code>valign</code>
		''' attribute for HTMLTheadElement for details.
		''' </summary>
		Property vAlign As String

		''' <summary>
		'''  The collection of rows in this table section.
		''' </summary>
		ReadOnly Property rows As HTMLCollection

		''' <summary>
		'''  Insert a row into this section. The new row is inserted immediately
		''' before the current <code>index</code> th row in this section. If
		''' <code>index</code> is equal to the number of rows in this section, the
		''' new row is appended. </summary>
		''' <param name="index">  The row number where to insert a new row. This index
		'''   starts from 0 and is relative only to the rows contained inside this
		'''   section, not all the rows in the table. </param>
		''' <returns>  The newly created row. </returns>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified index is greater than the
		'''   number of rows of if the index is neagative. </exception>
		Function insertRow(ByVal index As Integer) As HTMLElement

		''' <summary>
		'''  Delete a row from this section. </summary>
		''' <param name="index">  The index of the row to be deleted. This index starts
		'''   from 0 and is relative only to the rows contained inside this
		'''   section, not all the rows in the table. </param>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified index is greater than or
		'''   equal to the number of rows or if the index is negative. </exception>
		Sub deleteRow(ByVal index As Integer)

	End Interface

End Namespace