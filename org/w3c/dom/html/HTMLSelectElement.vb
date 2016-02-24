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
	'''  The select element allows the selection of an option. The contained
	''' options can be directly accessed through the select element as a
	''' collection. See the  SELECT element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLSelectElement
		Inherits HTMLElement

		''' <summary>
		'''  The type of this form control. This is the string "select-multiple"
		''' when the multiple attribute is <code>true</code> and the string
		''' "select-one" when <code>false</code> .
		''' </summary>
		Function [getType]() As String

		''' <summary>
		'''  The ordinal index of the selected option, starting from 0. The value
		''' -1 is returned if no element is selected. If multiple options are
		''' selected, the index of the first selected option is returned.
		''' </summary>
		Property selectedIndex As Integer

		''' <summary>
		'''  The current form control value.
		''' </summary>
		Property value As String

		''' <summary>
		'''  The number of options in this <code>SELECT</code> .
		''' </summary>
		ReadOnly Property length As Integer

		''' <summary>
		'''  Returns the <code>FORM</code> element containing this control. Returns
		''' <code>null</code> if this control is not within the context of a form.
		''' </summary>
		ReadOnly Property form As HTMLFormElement

		''' <summary>
		'''  The collection of <code>OPTION</code> elements contained by this
		''' element.
		''' </summary>
		ReadOnly Property options As HTMLCollection

		''' <summary>
		'''  The control is unavailable in this context. See the  disabled
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  If true, multiple <code>OPTION</code> elements may  be selected in
		''' this <code>SELECT</code> . See the  multiple attribute definition in
		''' HTML 4.0.
		''' </summary>
		Property multiple As Boolean

		''' <summary>
		'''  Form control or object name when submitted with a form. See the  name
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  Number of visible rows. See the  size attribute definition in HTML 4.0.
		''' </summary>
		Property size As Integer

		''' <summary>
		'''  Index that represents the element's position in the tabbing order. See
		''' the  tabindex attribute definition in HTML 4.0.
		''' </summary>
		Property tabIndex As Integer

		''' <summary>
		'''  Add a new element to the collection of <code>OPTION</code> elements
		''' for this <code>SELECT</code> . This method is the equivalent of the
		''' <code>appendChild</code> method of the <code>Node</code> interface if
		''' the <code>before</code> parameter is <code>null</code> . It is
		''' equivalent to the <code>insertBefore</code> method on the parent of
		''' <code>before</code> in all other cases. </summary>
		''' <param name="element">  The element to add. </param>
		''' <param name="before">  The element to insert before, or <code>null</code> for
		'''   the tail of the list. </param>
		''' <exception cref="DOMException">
		'''    NOT_FOUND_ERR: Raised if <code>before</code> is not a descendant of
		'''   the <code>SELECT</code> element. </exception>
		Sub add(ByVal element As HTMLElement, ByVal before As HTMLElement)

		''' <summary>
		'''  Remove an element from the collection of <code>OPTION</code> elements
		''' for this <code>SELECT</code> . Does nothing if no element has the given
		'''  index. </summary>
		''' <param name="index">  The index of the item to remove, starting from 0. </param>
		Sub remove(ByVal index As Integer)

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