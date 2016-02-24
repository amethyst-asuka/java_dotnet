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
	'''  Ordered list. See the  OL element definition in HTML 4.0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLOListElement
		Inherits HTMLElement

		''' <summary>
		'''  Reduce spacing between list items. See the  compact attribute
		''' definition in HTML 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property compact As Boolean

		''' <summary>
		'''  Starting sequence number. See the  start attribute definition in HTML
		''' 4.0. This attribute is deprecated in HTML 4.0.
		''' </summary>
		Property start As Integer

		''' <summary>
		'''  Numbering style. See the  type attribute definition in HTML 4.0. This
		''' attribute is deprecated in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

	End Interface

End Namespace