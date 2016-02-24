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
	''' <code>TreeWalker</code> objects are used to navigate a document tree or
	''' subtree using the view of the document defined by their
	''' <code>whatToShow</code> flags and filter (if any). Any function which
	''' performs navigation using a <code>TreeWalker</code> will automatically
	''' support any view defined by a <code>TreeWalker</code>.
	''' <p>Omitting nodes from the logical view of a subtree can result in a
	''' structure that is substantially different from the same subtree in the
	''' complete, unfiltered document. Nodes that are siblings in the
	''' <code>TreeWalker</code> view may be children of different, widely
	''' separated nodes in the original view. For instance, consider a
	''' <code>NodeFilter</code> that skips all nodes except for Text nodes and
	''' the root node of a document. In the logical view that results, all text
	''' nodes will be siblings and appear as direct children of the root node, no
	''' matter how deeply nested the structure of the original document.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Traversal-Range-20001113'>Document Object Model (DOM) Level 2 Traversal and Range Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface TreeWalker
		''' <summary>
		''' The <code>root</code> node of the <code>TreeWalker</code>, as specified
		''' when it was created.
		''' </summary>
		ReadOnly Property root As org.w3c.dom.Node

		''' <summary>
		''' This attribute determines which node types are presented via the
		''' <code>TreeWalker</code>. The available set of constants is defined in
		''' the <code>NodeFilter</code> interface.  Nodes not accepted by
		''' <code>whatToShow</code> will be skipped, but their children may still
		''' be considered. Note that this skip takes precedence over the filter,
		''' if any.
		''' </summary>
		ReadOnly Property whatToShow As Integer

		''' <summary>
		''' The filter used to screen nodes.
		''' </summary>
		ReadOnly Property filter As NodeFilter

		''' <summary>
		''' The value of this flag determines whether the children of entity
		''' reference nodes are visible to the <code>TreeWalker</code>. If false,
		''' these children  and their descendants will be rejected. Note that
		''' this rejection takes precedence over <code>whatToShow</code> and the
		''' filter, if any.
		''' <br> To produce a view of the document that has entity references
		''' expanded and does not expose the entity reference node itself, use
		''' the <code>whatToShow</code> flags to hide the entity reference node
		''' and set <code>expandEntityReferences</code> to true when creating the
		''' <code>TreeWalker</code>. To produce a view of the document that has
		''' entity reference nodes but no entity expansion, use the
		''' <code>whatToShow</code> flags to show the entity reference node and
		''' set <code>expandEntityReferences</code> to false.
		''' </summary>
		ReadOnly Property expandEntityReferences As Boolean

		''' <summary>
		''' The node at which the <code>TreeWalker</code> is currently positioned.
		''' <br>Alterations to the DOM tree may cause the current node to no longer
		''' be accepted by the <code>TreeWalker</code>'s associated filter.
		''' <code>currentNode</code> may also be explicitly set to any node,
		''' whether or not it is within the subtree specified by the
		''' <code>root</code> node or would be accepted by the filter and
		''' <code>whatToShow</code> flags. Further traversal occurs relative to
		''' <code>currentNode</code> even if it is not part of the current view,
		''' by applying the filters in the requested direction; if no traversal
		''' is possible, <code>currentNode</code> is not changed.
		''' </summary>
		Property currentNode As org.w3c.dom.Node

		''' <summary>
		''' Moves to and returns the closest visible ancestor node of the current
		''' node. If the search for <code>parentNode</code> attempts to step
		''' upward from the <code>TreeWalker</code>'s <code>root</code> node, or
		''' if it fails to find a visible ancestor node, this method retains the
		''' current position and returns <code>null</code>. </summary>
		''' <returns> The new parent node, or <code>null</code> if the current node
		'''   has no parent  in the <code>TreeWalker</code>'s logical view. </returns>
		Function parentNode() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the first visible child of the
		''' current node, and returns the new node. If the current node has no
		''' visible children, returns <code>null</code>, and retains the current
		''' node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   visible children  in the <code>TreeWalker</code>'s logical view. </returns>
		Function firstChild() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the last visible child of the
		''' current node, and returns the new node. If the current node has no
		''' visible children, returns <code>null</code>, and retains the current
		''' node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   children  in the <code>TreeWalker</code>'s logical view. </returns>
		Function lastChild() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the previous sibling of the
		''' current node, and returns the new node. If the current node has no
		''' visible previous sibling, returns <code>null</code>, and retains the
		''' current node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   previous sibling.  in the <code>TreeWalker</code>'s logical view. </returns>
		Function previousSibling() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the next sibling of the current
		''' node, and returns the new node. If the current node has no visible
		''' next sibling, returns <code>null</code>, and retains the current node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   next sibling.  in the <code>TreeWalker</code>'s logical view. </returns>
		Function nextSibling() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the previous visible node in
		''' document order relative to the current node, and returns the new
		''' node. If the current node has no previous node,  or if the search for
		''' <code>previousNode</code> attempts to step upward from the
		''' <code>TreeWalker</code>'s <code>root</code> node,  returns
		''' <code>null</code>, and retains the current node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   previous node  in the <code>TreeWalker</code>'s logical view. </returns>
		Function previousNode() As org.w3c.dom.Node

		''' <summary>
		''' Moves the <code>TreeWalker</code> to the next visible node in document
		''' order relative to the current node, and returns the new node. If the
		''' current node has no next node, or if the search for nextNode attempts
		''' to step upward from the <code>TreeWalker</code>'s <code>root</code>
		''' node, returns <code>null</code>, and retains the current node. </summary>
		''' <returns> The new node, or <code>null</code> if the current node has no
		'''   next node  in the <code>TreeWalker</code>'s logical view. </returns>
		Function nextNode() As org.w3c.dom.Node

	End Interface

End Namespace