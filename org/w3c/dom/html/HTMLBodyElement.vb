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
	'''  The HTML document body. This element is always present in the DOM API,
	''' even if the tags are not present in the source document. See the  BODY
	''' element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLBodyElement
		Inherits HTMLElement

		''' <summary>
		'''  Color of active links (after mouse-button down, but before
		''' mouse-button up). See the  alink attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property aLink As String

		''' <summary>
		'''  URI of the background texture tile image. See the  background
		''' attribute definition in HTML 4.0. This attribute is deprecated in HTML
		''' 4.0.
		''' </summary>
		Property background As String

		''' <summary>
		'''  Document background color. See the  bgcolor attribute definition in
		''' HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property bgColor As String

		''' <summary>
		'''  Color of links that are not active and unvisited. See the  link
		''' attribute definition in HTML 4.0. This attribute is deprecated in HTML
		''' 4.0.
		''' </summary>
		Property link As String

		''' <summary>
		'''  Document text color. See the  text attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property text As String

		''' <summary>
		'''  Color of links that have been visited by the user. See the  vlink
		''' attribute definition in HTML 4.0. This attribute is deprecated in HTML
		''' 4.0.
		''' </summary>
		Property vLink As String

	End Interface

End Namespace