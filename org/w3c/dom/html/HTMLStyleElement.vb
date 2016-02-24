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
	'''  Style information. See the  STYLE element definition in HTML 4.0, the
	''' module and the <code>LinkStyle</code> interface in the  module.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLStyleElement
		Inherits HTMLElement

		''' <summary>
		'''  Enables/disables the style sheet.
		''' </summary>
		Property disabled As Boolean

		''' <summary>
		'''  Designed for use with one or more target media. See the  media
		''' attribute definition in HTML 4.0.
		''' </summary>
		Property media As String

		''' <summary>
		'''  The content type pf the style sheet language. See the  type attribute
		''' definition in HTML 4.0.
		''' </summary>
		Function [getType]() As String
		WriteOnly Property type As String

	End Interface

End Namespace