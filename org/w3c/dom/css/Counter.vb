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
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.css

	''' <summary>
	'''  The <code>Counter</code> interface is used to represent any counter or
	''' counters function value. This interface reflects the values in the
	''' underlying style property.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Style-20001113'>Document Object Model (DOM) Level 2 Style Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface Counter
		''' <summary>
		'''  This attribute is used for the identifier of the counter.
		''' </summary>
		ReadOnly Property identifier As String

		''' <summary>
		'''  This attribute is used for the style of the list.
		''' </summary>
		ReadOnly Property listStyle As String

		''' <summary>
		'''  This attribute is used for the separator of the nested counters.
		''' </summary>
		ReadOnly Property separator As String

	End Interface

End Namespace