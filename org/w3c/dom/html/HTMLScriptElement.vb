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
	'''  Script statements. See the  SCRIPT element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLScriptElement
		Inherits HTMLElement

		''' <summary>
		'''  The script content of the element.
		''' </summary>
		Property text As String

		''' <summary>
		'''  Reserved for future use.
		''' </summary>
		Property htmlFor As String

		''' <summary>
		'''  Reserved for future use.
		''' </summary>
		Property [event] As String

		''' <summary>
		'''  The character encoding of the linked resource. See the  charset
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property charset As String

		''' <summary>
		'''  Indicates that the user agent can defer processing of the script.  See
		''' the  defer attribute definition in HTML 4.0.
		''' </summary>
		Property defer As Boolean

		''' <summary>
		'''  URI designating an external script. See the  src attribute definition
		''' in HTML 4.0.
		''' </summary>
		Property src As String

		''' <summary>
		'''  The content type of the script language. See the  type attribute
		''' definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

	End Interface

End Namespace