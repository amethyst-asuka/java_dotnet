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
	'''  Generic embedded object.  Note. In principle, all properties on the object
	''' element are read-write but in some environments some properties may be
	''' read-only once the underlying object is instantiated. See the  OBJECT
	''' element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLObjectElement
		Inherits HTMLElement

		''' <summary>
		'''  Returns the <code>FORM</code> element containing this control. Returns
		''' <code>null</code> if this control is not within the context of a form.
		''' </summary>
		ReadOnly Property form As HTMLFormElement

		''' <summary>
		'''  Applet class file. See the <code>code</code> attribute for
		''' HTMLAppletElement.
		''' </summary>
		Property code As String

		''' <summary>
		'''  Aligns this object (vertically or horizontally)  with respect to its
		''' surrounding text. See the  align attribute definition in HTML 4.0.
		''' This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property align As String

		''' <summary>
		'''  Space-separated list of archives. See the  archive attribute definition
		'''  in HTML 4.0.
		''' </summary>
		Property archive As String

		''' <summary>
		'''  Width of border around the object. See the  border attribute definition
		'''  in HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property border As String

		''' <summary>
		'''  Base URI for <code>classid</code> , <code>data</code> , and
		''' <code>archive</code> attributes. See the  codebase attribute definition
		'''  in HTML 4.0.
		''' </summary>
		Property codeBase As String

		''' <summary>
		'''  Content type for data downloaded via <code>classid</code> attribute.
		''' See the  codetype attribute definition in HTML 4.0.
		''' </summary>
		Property codeType As String

		''' <summary>
		'''  A URI specifying the location of the object's data.  See the  data
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property data As String

		''' <summary>
		'''  Declare (for future reference), but do not instantiate, this object.
		''' See the  declare attribute definition in HTML 4.0.
		''' </summary>
		Property [declare] As Boolean

		''' <summary>
		'''  Override height. See the  height attribute definition in HTML 4.0.
		''' </summary>
		Property height As String

		''' <summary>
		'''  Horizontal space to the left and right of this image, applet, or
		''' object. See the  hspace attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Property hspace As String

		''' <summary>
		'''  Form control or object name when submitted with a form. See the  name
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  Message to render while loading the object. See the  standby attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property standby As String

		''' <summary>
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

		''' <summary>
		'''  Content type for data downloaded via <code>data</code> attribute. See
		''' the  type attribute definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

		''' <summary>
		'''  Use client-side image map. See the  usemap attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property useMap As String

		''' <summary>
		'''  Vertical space above and below this image, applet, or object. See the
		''' vspace attribute definition in HTML 4.0. This attribute is deprecated
		''' in HTML 4.0.
		''' </summary>
		Property vspace As String

		''' <summary>
		'''  Override width. See the  width attribute definition in HTML 4.0.
		''' </summary>
		Property width As String

		''' <summary>
		'''  The document this object contains, if there is any and it is
		''' available, or <code>null</code> otherwise.
		''' @since DOM Level 2
		''' </summary>
		ReadOnly Property contentDocument As org.w3c.dom.Document

	End Interface

End Namespace