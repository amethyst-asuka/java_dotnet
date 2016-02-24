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
	'''  Push button. See the  BUTTON element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLButtonElement
		Inherits HTMLElement

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
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

		''' <summary>
		'''  The type of button. See the  type attribute definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String

		''' <summary>
		'''  The current form control value. See the  value attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property value As String

	End Interface

End Namespace