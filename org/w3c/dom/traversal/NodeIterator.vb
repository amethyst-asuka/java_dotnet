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

Namespace org.w3c.dom.traversal


	''' <summary>
	''' <code>NodeIterators</code> are used to step through a set of nodes, e.g.
	''' the set of nodes in a <code>NodeList</code>, the document subtree
	''' governed by a particular <code>Node</code>, the results of a query, or
	''' any other set of nodes. The set of nodes to be iterated is determined by
	''' the implementation of the <code>NodeIterator</code>. DOM Level 2
	''' specifies a single <code>NodeIterator</code> implementation for
	''' document-order traversal of a document subtree. Instances of these
	''' <code>NodeIterators</code> are created by calling
	''' <code>DocumentTraversal</code><code>.createNodeIterator()</code>.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Traversal-Range-20001113'>Document Object Model (DOM) Level 2 Traversal and Range Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface NodeIterator
		''' <summary>
		''' The root node of the <code>NodeIterator</code>, as specified when it
		''' was created.
		''' </summary>
		ReadOnly Property root As org.w3c.dom.Node

		''' <summary>
		''' This attribute determines which node types are presented via the
		''' <code>NodeIterator</code>. The available set of constants is defined
		''' in the <code>NodeFilter</code> interface.  Nodes not accepted by
		''' <code>whatToShow</code> will be skipped, but their children may still
		''' be considered. Note that this skip takes precedence over the filter,
		''' if any.
		''' </summary>
		ReadOnly Property whatToShow As Integer

		''' <summary>
		''' The <code>NodeFilter</code> used to screen nodes.
		''' </summary>
		ReadOnly Property filter As NodeFilter

		''' <summary>
		'''  The value of this flag determines whether the children of entity
		''' reference nodes are visible to the <code>NodeIterator</code>. If
		''' false, these children  and their descendants will be rejected. Note
		''' that this rejection takes precedence over <code>whatToShow</code> and
		''' the filter. Also note that this is currently the only situation where
		''' <code>NodeIterators</code> may reject a complete subtree rather than
		''' skipping individual nodes.
		''' <br>
		''' <br> To produce a view of the document that has entity references
		''' expanded and does not expose the entity reference node itself, use
		''' the <code>whatToShow</code> flags to hide the entity reference node
		''' and set <code>expandEntityReferences</code> to true when creating the
		''' <code>NodeIterator</code>. To produce a view of the document that has
		''' entity reference nodes but no entity expansion, use the
		''' <code>whatToShow</code> flags to show the entity reference node and
		''' set <code>expandEntityReferences</code> to false.
		''' </summary>
		ReadOnly Property expandEntityReferences As Boolean

		''' <summary>
		''' Returns the next node in the set and advances the position of the
		''' <code>NodeIterator</code> in the set. After a
		''' <code>NodeIterator</code> is created, the first call to
		''' <code>nextNode()</code> returns the first node in the set. </summary>
		''' <returns> The next <code>Node</code> in the set being iterated over, or
		'''   <code>null</code> if there are no more members in that set. </returns>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if this method is called after the
		'''   <code>detach</code> method was invoked. </exception>
		Function nextNode() As org.w3c.dom.Node

		''' <summary>
		''' Returns the previous node in the set and moves the position of the
		''' <code>NodeIterator</code> backwards in the set. </summary>
		''' <returns> The previous <code>Node</code> in the set being iterated over,
		'''   or <code>null</code> if there are no more members in that set. </returns>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if this method is called after the
		'''   <code>detach</code> method was invoked. </exception>
		Function previousNode() As org.w3c.dom.Node

		''' <summary>
		''' Detaches the <code>NodeIterator</code> from the set which it iterated
		''' over, releasing any computational resources and placing the
		''' <code>NodeIterator</code> in the INVALID state. After
		''' <code>detach</code> has been invoked, calls to <code>nextNode</code>
		''' or <code>previousNode</code> will raise the exception
		''' INVALID_STATE_ERR.
		''' </summary>
		Sub detach()

	End Interface

End Namespace