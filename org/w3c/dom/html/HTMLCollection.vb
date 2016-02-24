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
	'''  An <code>HTMLCollection</code> is a list of nodes. An individual node may
	''' be accessed by either ordinal index or the node's<code>name</code> or
	''' <code>id</code> attributes.  Note: Collections in the HTML DOM are assumed
	''' to be  live meaning that they are automatically updated when the
	''' underlying document is changed.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/CR-DOM-Level-2-20000510'>Document Object Model (DOM) Level 2 Specification</a>.
	''' </summary>
	Public Interface HTMLCollection
		''' <summary>
		'''  This attribute specifies the length or  size of the list.
		''' </summary>
		ReadOnly Property length As Integer

		''' <summary>
		'''  This method retrieves a node specified by ordinal index. Nodes are
		''' numbered in tree order (depth-first traversal order). </summary>
		''' <param name="index">  The index of the node to be fetched. The index origin is
		'''   0. </param>
		''' <returns>  The <code>Node</code> at the corresponding position upon
		'''   success. A value of <code>null</code> is returned if the index is
		'''   out of range. </returns>
		Function item(ByVal index As Integer) As org.w3c.dom.Node

		''' <summary>
		'''  This method retrieves a <code>Node</code> using a name. It first
		''' searches for a <code>Node</code> with a matching <code>id</code>
		''' attribute. If it doesn't find one, it then searches for a
		''' <code>Node</code> with a matching <code>name</code> attribute, but
		''' only on those elements that are allowed a name attribute. </summary>
		''' <param name="name">  The name of the <code>Node</code> to be fetched. </param>
		''' <returns>  The <code>Node</code> with a <code>name</code> or
		'''   <code>id</code> attribute whose value corresponds to the specified
		'''   string. Upon failure (e.g., no node with this name exists), returns
		'''   <code>null</code> . </returns>
		Function namedItem(ByVal name As String) As org.w3c.dom.Node

	End Interface

End Namespace