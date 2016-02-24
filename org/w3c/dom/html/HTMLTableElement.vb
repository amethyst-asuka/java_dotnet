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
	'''  The create* and delete* methods on the table allow authors to construct
	''' and modify tables. HTML 4.0 specifies that only one of each of the
	''' <code>CAPTION</code> , <code>THEAD</code> , and <code>TFOOT</code>
	''' elements may exist in a table. Therefore, if one exists, and the
	''' createTHead() or createTFoot() method is called, the method returns the
	''' existing THead or TFoot element. See the  TABLE element definition in HTML
	''' 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLTableElement
		Inherits HTMLElement

		''' <summary>
		'''  Returns the table's <code>CAPTION</code> , or void if none exists.
		''' </summary>
		Property caption As HTMLTableCaptionElement

		''' <summary>
		'''  Returns the table's <code>THEAD</code> , or <code>null</code> if none
		''' exists.
		''' </summary>
		Property tHead As HTMLTableSectionElement

		''' <summary>
		'''  Returns the table's <code>TFOOT</code> , or <code>null</code> if none
		''' exists.
		''' </summary>
		Property tFoot As HTMLTableSectionElement

		''' <summary>
		'''  Returns a collection of all the rows in the table, including all in
		''' <code>THEAD</code> , <code>TFOOT</code> , all <code>TBODY</code>
		''' elements.
		''' </summary>
		ReadOnly Property rows As HTMLCollection

		''' <summary>
		'''  Returns a collection of the defined table bodies.
		''' </summary>
		ReadOnly Property tBodies As HTMLCollection

		''' <summary>
		'''  Specifies the table's position with respect to the rest of the
		''' document. See the  align attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Cell background color. See the  bgcolor attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property bgColor As String

		''' <summary>
		'''  The width of the border around the table. See the  border attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property border As String

		''' <summary>
		'''  Specifies the horizontal and vertical space between cell content and
		''' cell borders. See the  cellpadding attribute definition in HTML 4.0.
		''' </summary>
		Property cellPadding As String

		''' <summary>
		'''  Specifies the horizontal and vertical separation between cells. See
		''' the  cellspacing attribute definition in HTML 4.0.
		''' </summary>
		Property cellSpacing As String

		''' <summary>
		'''  Specifies which external table borders to render. See the  frame
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property frame As String

		''' <summary>
		'''  Specifies which internal table borders to render. See the  rules
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property rules As String

		''' <summary>
		'''  Description about the purpose or structure of a table. See the
		''' summary attribute definition in HTML 4.0.
		''' </summary>
		Property summary As String

		''' <summary>
		'''  Specifies the desired table width. See the  width attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property width As String

		''' <summary>
		'''  Create a table header row or return an existing one. </summary>
		''' <returns>  A new table header element (<code>THEAD</code> ). </returns>
		Function createTHead() As HTMLElement

		''' <summary>
		'''  Delete the header from the table, if one exists.
		''' </summary>
		Sub deleteTHead()

		''' <summary>
		'''  Create a table footer row or return an existing one. </summary>
		''' <returns>  A footer element (<code>TFOOT</code> ). </returns>
		Function createTFoot() As HTMLElement

		''' <summary>
		'''  Delete the footer from the table, if one exists.
		''' </summary>
		Sub deleteTFoot()

		''' <summary>
		'''  Create a new table caption object or return an existing one. </summary>
		''' <returns>  A <code>CAPTION</code> element. </returns>
		Function createCaption() As HTMLElement

		''' <summary>
		'''  Delete the table caption, if one exists.
		''' </summary>
		Sub deleteCaption()

		''' <summary>
		'''  Insert a new empty row in the table. The new row is inserted
		''' immediately before and in the same section as the current
		''' <code>index</code> th row in the table. If <code>index</code> is equal
		''' to the number of rows, the new row is appended. In addition, when the
		''' table is empty the row is inserted into a <code>TBODY</code> which is
		''' created and inserted into the table. Note. A table row cannot be empty
		''' according to HTML 4.0 Recommendation. </summary>
		''' <param name="index">  The row number where to insert a new row. This index
		'''   starts from 0 and is relative to all the rows contained inside the
		'''   table, regardless of section parentage. </param>
		''' <returns>  The newly created row. </returns>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified index is greater than the
		'''   number of rows or if the index is negative. </exception>
		Function insertRow(ByVal index As Integer) As HTMLElement

		''' <summary>
		'''  Delete a table row. </summary>
		''' <param name="index">  The index of the row to be deleted. This index starts
		'''   from 0 and is relative to all the rows contained inside the table,
		'''   regardless of section parentage. </param>
		''' <exception cref="DOMException">
		'''    INDEX_SIZE_ERR: Raised if the specified index is greater than or
		'''   equal to the number of rows or if the index is negative. </exception>
		Sub deleteRow(ByVal index As Integer)

	End Interface

End Namespace