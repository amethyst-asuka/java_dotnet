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
	'''  A selectable choice. See the  OPTION element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLOptionElement
		Inherits HTMLElement

		''' <summary>
		'''  Returns the <code>FORM</code> element containing this control. Returns
		''' <code>null</code> if this control is not within the context of a form.
		''' </summary>
		ReadOnly Property form As HTMLFormElement

		''' <summary>
		'''  Represents the value of the HTML selected attribute. The value of this
		''' attribute does not change if the state of the corresponding form
		''' control, in an interactive user agent, changes. Changing
		''' <code>defaultSelected</code> , however, resets the state of the form
		''' control. See the  selected attribute definition in HTML 4.0.
		''' </summary>
		Property defaultSelected As Boolean

		''' <summary>
		'''  The text contained within the option element.
		''' </summary>
		ReadOnly Property text As String

		''' <summary>
		'''  The index of this <code>OPTION</code> in its parent <code>SELECT</code>
		'''  , starting from 0.
		''' </summary>
		ReadOnly Property index As Integer

		''' <summary>
		'''  The control is unavailable in this context. See the  disabled
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  Option label for use in hierarchical menus. See the  label attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property label As String

		''' <summary>
		'''  Represents the current state of the corresponding form control, in an
		''' interactive user agent. Changing this attribute changes the state of
		''' the form control, but does not change the value of the HTML selected
		''' attribute of the element.
		''' </summary>
		Property selected As Boolean

		''' <summary>
		'''  The current form control value. See the  value attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property value As String

	End Interface

End Namespace