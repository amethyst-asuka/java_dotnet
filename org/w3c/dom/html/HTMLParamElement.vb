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
	'''  Parameters fed to the <code>OBJECT</code> element. See the  PARAM element
	''' definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLParamElement
		Inherits HTMLElement

		''' <summary>
		'''  The name of a run-time parameter. See the  name attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property name As String

		''' <summary>
		'''  Content type for the <code>value</code> attribute when
		''' <code>valuetype</code> has the value "ref". See the  type attribute
		''' definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

		''' <summary>
		'''  The value of a run-time parameter. See the  value attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property value As String

		''' <summary>
		'''  Information about the meaning of the <code>value</code> attribute
		''' value. See the  valuetype attribute definition in HTML 4.0.
		''' </summary>
		Property valueType As String

	End Interface

End Namespace