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
	'''  The object used to represent the <code>TH</code> and <code>TD</code>
	''' elements. See the  TD element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLTableCellElement
		Inherits HTMLElement

		''' <summary>
		'''  The index of this cell in the row, starting from 0. This index is in
		''' document tree order and not display order.
		''' </summary>
		ReadOnly Property cellIndex As Integer

		''' <summary>
		'''  Abbreviation for header cells. See the  abbr attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property abbr As String

		''' <summary>
		'''  Horizontal alignment of data in cell. See the  align attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Names group of related headers. See the  axis attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property axis As String

		''' <summary>
		'''  Cell background color. See the  bgcolor attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
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
		'''  Number of columns spanned by cell. See the  colspan attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property colSpan As Integer

		''' <summary>
		'''  List of <code>id</code> attribute values for header cells. See the
		''' headers attribute definition in HTML 4.0.
		''' </summary>
		Property headers As String

		''' <summary>
		'''  Cell height. See the  height attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property height As String

		''' <summary>
		'''  Suppress word wrapping. See the  nowrap attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property noWrap As Boolean

		''' <summary>
		'''  Number of rows spanned by cell. See the  rowspan attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property rowSpan As Integer

		''' <summary>
		'''  Scope covered by header cells. See the  scope attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property scope As String

		''' <summary>
		'''  Vertical alignment of data in cell. See the  valign attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property vAlign As String

		''' <summary>
		'''  Cell width. See the  width attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property width As String

	End Interface

End Namespace