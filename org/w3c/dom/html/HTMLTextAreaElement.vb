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
	'''  Multi-line text field. See the  TEXTAREA element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLTextAreaElement
		Inherits HTMLElement

		''' <summary>
		'''  Represents the contents of the element. The value of this attribute
		''' does not change if the contents of the corresponding form control, in
		''' an interactive user agent, changes. Changing this attribute, however,
		''' resets the contents of the form control.
		''' </summary>
		Property defaultValue As String

		''' <summary>
		'''  Returns the <code>FORM</code> element containing this control. Returns
		''' <code>null</code> if this control is not within the context of a form.
		''' </summary>
		ReadOnly Property form As HTMLFormElement

		''' <summary>
		'''  A single character access key to give access to the form control. See
		''' the  accesskey attribute definition in HTML 4.0.
		''' </summary>
		Property accessKey As String

		''' <summary>
		'''  Width of control (in characters). See the  cols attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property cols As Integer

		''' <summary>
		'''  The control is unavailable in this context. See the  disabled
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  Form control or object name when submitted with a form. See the  name
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  This control is read-only. See the  readonly attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property [readOnly] As Boolean

		''' <summary>
		'''  Number of text rows. See the  rows attribute definition in HTML 4.0.
		''' </summary>
		Property rows As Integer

		''' <summary>
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

		''' <summary>
		'''  The type of this form control. This the string "textarea".
		''' </summary>
		Function [getType]() As String

		''' <summary>
		'''  Represents the current contents of the corresponding form control, in
		''' an interactive user agent. Changing this attribute changes the
		''' contents of the form control, but does not change the contents of the
		''' element. If the entirety of the data can not fit into a single
		''' <code>DOMString</code> , the implementation may truncate the data.
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
		'''  Select the contents of the <code>TEXTAREA</code> .
		''' </summary>
		Sub [select]()

	End Interface

End Namespace