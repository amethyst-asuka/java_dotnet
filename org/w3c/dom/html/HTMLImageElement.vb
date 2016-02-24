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
	'''  Embedded image. See the  IMG element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLImageElement
		Inherits HTMLElement

		''' <summary>
		'''  URI designating the source of this image, for low-resolution output.
		''' </summary>
		Property lowSrc As String

		''' <summary>
		'''  The name of the element (for backwards compatibility).
		''' </summary>
		Property name As String

		''' <summary>
		'''  Aligns this object (vertically or horizontally)  with respect to its
		''' surrounding text. See the  align attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Alternate text for user agents not rendering the normal content of
		''' this element. See the  alt attribute definition in HTML 4.0.
		''' </summary>
		Property alt As String

		''' <summary>
		'''  Width of border around image. See the  border attribute definition in
		''' HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property border As String

		''' <summary>
		'''  Override height. See the  height attribute definition in HTML 4.0.
		''' </summary>
		Property height As String

		''' <summary>
		'''  Horizontal space to the left and right of this image. See the  hspace
		''' attribute definition in HTML 4.0. This attribute is deprecated in HTML
		''' 4.0.
		''' </summary>
		Property hspace As String

		''' <summary>
		'''  Use server-side image map. See the  ismap attribute definition in HTML
		''' 4.0.
		''' </summary>
		Property isMap As Boolean

		''' <summary>
		'''  URI designating a long description of this image or frame. See the
		''' longdesc attribute definition in HTML 4.0.
		''' </summary>
		Property longDesc As String

		''' <summary>
		'''  URI designating the source of this image. See the  src attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property src As String

		''' <summary>
		'''  Use client-side image map. See the  usemap attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property useMap As String

		''' <summary>
		'''  Vertical space above and below this image. See the  vspace attribute
		''' definition in HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property vspace As String

		''' <summary>
		'''  Override width. See the  width attribute definition in HTML 4.0.
		''' </summary>
		Property width As String

	End Interface

End Namespace