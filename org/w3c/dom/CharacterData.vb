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
' * Copyright (c) 2004 World Wide Web Consortium,
' *
' * (Massachusetts Institute of Technology, European Research Consortium for
' * Informatics and Mathematics, Keio University). All Rights Reserved. This
' * work is distributed under the W3C(r) Software License [1] in the hope that
' * it will be useful, but WITHOUT ANY WARRANTY; without even the implied
' * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' *
' * [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
' 

Namespace org.w3c.dom

	''' <summary>
	''' The <code>CharacterData</code> interface extends Node with a set of
	''' attributes and methods for accessing character data in the DOM. For
	''' clarity this set is defined here rather than on each object that uses
	''' these attributes and methods. No DOM objects correspond directly to
	''' <code>CharacterData</code>, though <code>Text</code> and others do
	''' inherit the interface from it. All <code>offsets</code> in this interface
	''' start from <code>0</code>.
	''' <p>As explained in the <code>DOMString</code> interface, text strings in
	''' the DOM are represented in UTF-16, i.e. as a sequence of 16-bit units. In
	''' the following, the term 16-bit units is used whenever necessary to
	''' indicate that indexing on CharacterData is done in 16-bit units.
	''' <p>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document Object Model (DOM) Level 3 Core Specification</a>.
	''' </summary>
	Public Interface CharacterData
		Inherits Node

		''' <summary>
		''' The character data of the node that implements this interface. The DOM
		''' implementation may not put arbitrary limits on the amount of data
		''' that may be stored in a <code>CharacterData</code> node. However,
		''' implementation limits may mean that the entirety of a node's data may
		''' not fit into a single <code>DOMString</code>. In such cases, the user
		''' may call <code>substringData</code> to retrieve the data in
		''' appropriately sized pieces. </summary>
		''' <exception cref="DOMException">
		'''   DOMSTRING_SIZE_ERR: Raised when it would return more characters than
		'''   fit in a <code>DOMString</code> variable on the implementation
		'''   platform. </exception>
		Property data As String

		''' <summary>
		''' The number of 16-bit units that are available through <code>data</code>
		''' and the <code>substringData</code> method below. This may have the
		''' value zero, i.e., <code>CharacterData</code> nodes may be empty.
		''' </summary>
		ReadOnly Property length As Integer

		''' <summary>
		''' Extracts a range of data from the node. </summary>
		''' <param name="offset"> Start offset of substring to extract. </param>
		''' <param name="count"> The number of 16-bit units to extract. </param>
		''' <returns> The specified substring. If the sum of <code>offset</code> and
		'''   <code>count</code> exceeds the <code>length</code>, then all 16-bit
		'''   units to the end of the data are returned. </returns>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if the specified <code>offset</code> is
		'''   negative or greater than the number of 16-bit units in
		'''   <code>data</code>, or if the specified <code>count</code> is
		'''   negative.
		'''   <br>DOMSTRING_SIZE_ERR: Raised if the specified range of text does
		'''   not fit into a <code>DOMString</code>. </exception>
		Function substringData(ByVal offset As Integer, ByVal count As Integer) As String

		''' <summary>
		''' Append the string to the end of the character data of the node. Upon
		''' success, <code>data</code> provides access to the concatenation of
		''' <code>data</code> and the <code>DOMString</code> specified. </summary>
		''' <param name="arg"> The <code>DOMString</code> to append. </param>
		''' <exception cref="DOMException">
		'''   NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly. </exception>
		Sub appendData(ByVal arg As String)

		''' <summary>
		''' Insert a string at the specified 16-bit unit offset. </summary>
		''' <param name="offset"> The character offset at which to insert. </param>
		''' <param name="arg"> The <code>DOMString</code> to insert. </param>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if the specified <code>offset</code> is
		'''   negative or greater than the number of 16-bit units in
		'''   <code>data</code>.
		'''   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly. </exception>
		Sub insertData(ByVal offset As Integer, ByVal arg As String)

		''' <summary>
		''' Remove a range of 16-bit units from the node. Upon success,
		''' <code>data</code> and <code>length</code> reflect the change. </summary>
		''' <param name="offset"> The offset from which to start removing. </param>
		''' <param name="count"> The number of 16-bit units to delete. If the sum of
		'''   <code>offset</code> and <code>count</code> exceeds
		'''   <code>length</code> then all 16-bit units from <code>offset</code>
		'''   to the end of the data are deleted. </param>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if the specified <code>offset</code> is
		'''   negative or greater than the number of 16-bit units in
		'''   <code>data</code>, or if the specified <code>count</code> is
		'''   negative.
		'''   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly. </exception>
		Sub deleteData(ByVal offset As Integer, ByVal count As Integer)

		''' <summary>
		''' Replace the characters starting at the specified 16-bit unit offset
		''' with the specified string. </summary>
		''' <param name="offset"> The offset from which to start replacing. </param>
		''' <param name="count"> The number of 16-bit units to replace. If the sum of
		'''   <code>offset</code> and <code>count</code> exceeds
		'''   <code>length</code>, then all 16-bit units to the end of the data
		'''   are replaced; (i.e., the effect is the same as a <code>remove</code>
		'''    method call with the same range, followed by an <code>append</code>
		'''    method invocation). </param>
		''' <param name="arg"> The <code>DOMString</code> with which the range must be
		'''   replaced. </param>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if the specified <code>offset</code> is
		'''   negative or greater than the number of 16-bit units in
		'''   <code>data</code>, or if the specified <code>count</code> is
		'''   negative.
		'''   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this node is readonly. </exception>
		Sub replaceData(ByVal offset As Integer, ByVal count As Integer, ByVal arg As String)

	End Interface

End Namespace