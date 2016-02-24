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
	'''  Form control.  Note. Depending upon the environment in which the page is
	''' being viewed, the value property may be read-only for the file upload
	''' input type. For the "password" input type, the actual value returned may
	''' be masked to prevent unauthorized use. See the  INPUT element definition
	''' in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLInputElement
		Inherits HTMLElement

		''' <summary>
		'''  When the <code>type</code> attribute of the element has the value
		''' "Text", "File" or "Password", this represents the HTML value attribute
		''' of the element. The value of this attribute does not change if the
		''' contents of the corresponding form control, in an interactive user
		''' agent, changes. Changing this attribute, however, resets the contents
		''' of the form control. See the  value attribute definition in HTML 4.0.
		''' </summary>
		Property defaultValue As String

		''' <summary>
		'''  When <code>type</code> has the value "Radio" or "Checkbox", this
		''' represents the HTML checked attribute of the element. The value of
		''' this attribute does not change if the state of the corresponding form
		''' control, in an interactive user agent, changes. Changes to this
		''' attribute, however, resets the state of the form control. See the
		''' checked attribute definition in HTML 4.0.
		''' </summary>
		Property defaultChecked As Boolean

		''' <summary>
		'''  Returns the <code>FORM</code> element containing this control. Returns
		''' <code>null</code> if this control is not within the context of a form.
		''' </summary>
		ReadOnly Property form As HTMLFormElement

		''' <summary>
		'''  A comma-separated list of content types that a server processing this
		''' form will handle correctly. See the  accept attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property accept As String

		''' <summary>
		'''  A single character access key to give access to the form control. See
		''' the  accesskey attribute definition in HTML 4.0.
		''' </summary>
		Property accessKey As String

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
		'''  When the <code>type</code> attribute of the element has the value
		''' "Radio" or "Checkbox", this represents the current state of the form
		''' control, in an interactive user agent. Changes to this attribute
		''' change the state of the form control, but do not change the value of
		''' the HTML value attribute of the element.
		''' </summary>
		Property checked As Boolean

		''' <summary>
		'''  The control is unavailable in this context. See the  disabled
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  Maximum number of characters for text fields, when <code>type</code>
		''' has the value "Text" or "Password". See the  maxlength attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property maxLength As Integer

		''' <summary>
		'''  Form control or object name when submitted with a form. See the  name
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  This control is read-only. Relevant only when <code>type</code> has
		''' the value "Text" or "Password". See the  readonly attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property [readOnly] As Boolean

		''' <summary>
		'''  Size information. The precise meaning is specific to each type of
		''' field.  See the  size attribute definition in HTML 4.0.
		''' </summary>
		Property size As String

		''' <summary>
		'''  When the <code>type</code> attribute has the value "Image", this
		''' attribute specifies the location of the image to be used to decorate
		''' the graphical submit button. See the  src attribute definition in HTML
		''' 4.0.
		''' </summary>
		Property src As String

		''' <summary>
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

		''' <summary>
		'''  The type of control created. See the  type attribute definition in
		''' HTML 4.0.
		''' </summary>
		Function [getType]() As String

		''' <summary>
		'''  Use client-side image map. See the  usemap attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property useMap As String

		''' <summary>
		'''  When the <code>type</code> attribute of the element has the value
		''' "Text", "File" or "Password", this represents the current contents of
		''' the corresponding form control, in an interactive user agent. Changing
		''' this attribute changes the contents of the form control, but does not
		''' change the value of the HTML value attribute of the element. When the
		''' <code>type</code> attribute of the element has the value "Button",
		''' "Hidden", "Submit", "Reset", "Image", "Checkbox" or "Radio", this
		''' represents the HTML value attribute of the element. See the  value
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property value As String

		''' <summary>
		'''  Removes keyboard focus from this element.
		''' </summary>
		Sub blur()

		''' <summary>
		'''  Gives keyboard focus to this element.
		''' </summary>
		Sub focus()

		''' <summary>
		'''  Select the contents of the text area. For <code>INPUT</code> elements
		''' whose <code>type</code> attribute has one of the following values:
		''' "Text", "File", or "Password".
		''' </summary>
		Sub [select]()

		''' <summary>
		'''  Simulate a mouse-click. For <code>INPUT</code> elements whose
		''' <code>type</code> attribute has one of the following values: "Button",
		''' "Checkbox", "Radio", "Reset", or "Submit".
		''' </summary>
		Sub click()

	End Interface

End Namespace