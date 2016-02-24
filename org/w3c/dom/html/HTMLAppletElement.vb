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
	'''  An embedded Java applet. See the  APPLET element definition in HTML 4.0.
	''' This element is deprecated in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLAppletElement
		Inherits HTMLElement

		''' <summary>
		'''  Aligns this object (vertically or horizontally)  with respect to its
		''' surrounding text. See the  align attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Alternate text for user agents not rendering the normal content of
		''' this element. See the  alt attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property alt As String

		''' <summary>
		'''  Comma-separated archive list. See the  archive attribute definition in
		''' HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property archive As String

		''' <summary>
		'''  Applet class file.  See the  code attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property code As String

		''' <summary>
		'''  Optional base URI for applet. See the  codebase attribute definition
		''' in HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property codeBase As String

		''' <summary>
		'''  Override height. See the  height attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property height As String

		''' <summary>
		'''  Horizontal space to the left and right of this image, applet, or
		''' object. See the  hspace attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property hspace As String

		''' <summary>
		'''  The name of the applet. See the  name attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  Serialized applet file. See the  object attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property [object] As String

		''' <summary>
		'''  Vertical space above and below this image, applet, or object. See the
		''' vspace attribute definition in HTML 4.0. This attribute is deprecated
		''' in HTML 4.0.
		''' </summary>
		Property vspace As String

		''' <summary>
		'''  Override width. See the  width attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property width As String

	End Interface

End Namespace