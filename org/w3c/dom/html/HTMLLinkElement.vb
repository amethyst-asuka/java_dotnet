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
	'''  The <code>LINK</code> element specifies a link to an external resource,
	''' and defines this document's relationship to that resource (or vice versa).
	'''  See the  LINK element definition in HTML 4.0  (see also the
	''' <code>LinkStyle</code> interface in the  module).
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLLinkElement
		Inherits HTMLElement

		''' <summary>
		'''  Enables/disables the link. This is currently only used for style sheet
		''' links, and may be used to activate or deactivate style sheets.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  The character encoding of the resource being linked to. See the
		''' charset attribute definition in HTML 4.0.
		''' </summary>
		Property charset As String

		''' <summary>
		'''  The URI of the linked resource. See the  href attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property href As String

		''' <summary>
		'''  Language code of the linked resource. See the  hreflang attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property hreflang As String

		''' <summary>
		'''  Designed for use with one or more target media. See the  media
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property media As String

		''' <summary>
		'''  Forward link type. See the  rel attribute definition in HTML 4.0.
		''' </summary>
		Property rel As String

		''' <summary>
		'''  Reverse link type. See the  rev attribute definition in HTML 4.0.
		''' </summary>
		Property rev As String

		''' <summary>
		'''  Frame to render the resource in. See the  target attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property target As String

		''' <summary>
		'''  Advisory content type. See the  type attribute definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

	End Interface

End Namespace