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
	'''  The <code>FORM</code> element encompasses behavior similar to a collection
	''' and an element. It provides direct access to the contained input elements
	''' as well as the attributes of the form element. See the  FORM element
	''' definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLFormElement
		Inherits HTMLElement

		''' <summary>
		'''  Returns a collection of all control elements in the form.
		''' </summary>
		ReadOnly Property elements As HTMLCollection

		''' <summary>
		'''  The number of form controls in the form.
		''' </summary>
		ReadOnly Property length As Integer

		''' <summary>
		'''  Names the form.
		''' </summary>
		Property name As String

		''' <summary>
		'''  List of character sets supported by the server. See the
		''' accept-charset attribute definition in HTML 4.0.
		''' </summary>
		Property acceptCharset As String

		''' <summary>
		'''  Server-side form handler. See the  action attribute definition in HTML
		''' 4.0.
		''' </summary>
		Property action As String

		''' <summary>
		'''  The content type of the submitted form,  generally
		''' "application/x-www-form-urlencoded".  See the  enctype attribute
		''' definition in HTML 4.0.
		''' </summary>
		Property enctype As String

		''' <summary>
		'''  HTTP method used to submit form. See the  method attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property method As String

		''' <summary>
		'''  Frame to render the resource in. See the  target attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property target As String

		''' <summary>
		'''  Submits the form. It performs the same action as a  submit button.
		''' </summary>
		Sub submit()

		''' <summary>
		'''  Restores a form element's default values. It performs  the same action
		''' as a reset button.
		''' </summary>
		Sub reset()

	End Interface

End Namespace