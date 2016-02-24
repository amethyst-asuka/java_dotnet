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
	'''  The anchor element. See the  A element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLAnchorElement
		Inherits HTMLElement

		''' <summary>
		'''  A single character access key to give access to the form control. See
		''' the  accesskey attribute definition in HTML 4.0.
		''' </summary>
		Property accessKey As String

		''' <summary>
		'''  The character encoding of the linked resource. See the  charset
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property charset As String

		''' <summary>
		'''  Comma-separated list of lengths, defining an active region geometry.
		''' See also <code>shape</code> for the shape of the region. See the
		''' coords attribute definition in HTML 4.0.
		''' </summary>
		Property coords As String

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
		'''  Anchor name. See the  name attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  Forward link type. See the  rel attribute definition in HTML 4.0.
		''' </summary>
		Property rel As String

		''' <summary>
		'''  Reverse link type. See the  rev attribute definition in HTML 4.0.
		''' </summary>
		Property rev As String

		''' <summary>
		'''  The shape of the active area. The coordinates are given by
		''' <code>coords</code> . See the  shape attribute definition in HTML 4.0.
		''' </summary>
		Property shape As String

		''' <summary>
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

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

		''' <summary>
		'''  Removes keyboard focus from this element.
		''' </summary>
		Sub blur()

		''' <summary>
		'''  Gives keyboard focus to this element.
		''' </summary>
		Sub focus()

	End Interface

End Namespace