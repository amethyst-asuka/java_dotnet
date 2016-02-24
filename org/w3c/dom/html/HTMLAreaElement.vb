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
	'''  Client-side image map area definition. See the  AREA element definition in
	''' HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLAreaElement
		Inherits HTMLElement

		''' <summary>
		'''  A single character access key to give access to the form control. See
		''' the  accesskey attribute definition in HTML 4.0.
		''' </summary>
		Property accessKey As String

		''' <summary>
		'''  Alternate text for user agents not rendering the normal content of
		''' this element. See the  alt attribute definition in HTML 4.0.
		''' </summary>
		Property alt As String

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
		'''  Specifies that this area is inactive, i.e., has no associated action.
		''' See the  nohref attribute definition in HTML 4.0.
		''' </summary>
		Property noHref As Boolean

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

	End Interface

End Namespace