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

Namespace org.w3c.dom.ranges


	''' <summary>
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Traversal-Range-20001113'>Document Object Model (DOM) Level 2 Traversal and Range Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface Range
		''' <summary>
		''' Node within which the Range begins </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property startContainer As org.w3c.dom.Node

		''' <summary>
		''' Offset within the starting node of the Range. </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property startOffset As Integer

		''' <summary>
		''' Node within which the Range ends </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property endContainer As org.w3c.dom.Node

		''' <summary>
		''' Offset within the ending node of the Range. </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property endOffset As Integer

		''' <summary>
		''' TRUE if the Range is collapsed </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property collapsed As Boolean

		''' <summary>
		''' The deepest common ancestor container of the Range's two
		''' boundary-points. </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		ReadOnly Property commonAncestorContainer As org.w3c.dom.Node

		''' <summary>
		''' Sets the attributes describing the start of the Range. </summary>
		''' <param name="refNode"> The <code>refNode</code> value. This parameter must be
		'''   different from <code>null</code>. </param>
		''' <param name="offset"> The <code>startOffset</code> value. </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if <code>refNode</code> or an ancestor
		'''   of <code>refNode</code> is an Entity, Notation, or DocumentType
		'''   node. </exception>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if <code>offset</code> is negative or greater
		'''   than the number of child units in <code>refNode</code>. Child units
		'''   are 16-bit units if <code>refNode</code> is a type of CharacterData
		'''   node (e.g., a Text or Comment node) or a ProcessingInstruction
		'''   node. Child units are Nodes in all other cases.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		Sub setStart(ByVal refNode As org.w3c.dom.Node, ByVal offset As Integer)

		''' <summary>
		''' Sets the attributes describing the end of a Range. </summary>
		''' <param name="refNode"> The <code>refNode</code> value. This parameter must be
		'''   different from <code>null</code>. </param>
		''' <param name="offset"> The <code>endOffset</code> value. </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if <code>refNode</code> or an ancestor
		'''   of <code>refNode</code> is an Entity, Notation, or DocumentType
		'''   node. </exception>
		''' <exception cref="DOMException">
		'''   INDEX_SIZE_ERR: Raised if <code>offset</code> is negative or greater
		'''   than the number of child units in <code>refNode</code>. Child units
		'''   are 16-bit units if <code>refNode</code> is a type of CharacterData
		'''   node (e.g., a Text or Comment node) or a ProcessingInstruction
		'''   node. Child units are Nodes in all other cases.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		Sub setEnd(ByVal refNode As org.w3c.dom.Node, ByVal offset As Integer)

		''' <summary>
		''' Sets the start position to be before a node </summary>
		''' <param name="refNode"> Range starts before <code>refNode</code> </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if the root container of
		'''   <code>refNode</code> is not an Attr, Document, or DocumentFragment
		'''   node or if <code>refNode</code> is a Document, DocumentFragment,
		'''   Attr, Entity, or Notation node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		WriteOnly Property startBefore As org.w3c.dom.Node

		''' <summary>
		''' Sets the start position to be after a node </summary>
		''' <param name="refNode"> Range starts after <code>refNode</code> </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if the root container of
		'''   <code>refNode</code> is not an Attr, Document, or DocumentFragment
		'''   node or if <code>refNode</code> is a Document, DocumentFragment,
		'''   Attr, Entity, or Notation node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		WriteOnly Property startAfter As org.w3c.dom.Node

		''' <summary>
		''' Sets the end position to be before a node. </summary>
		''' <param name="refNode"> Range ends before <code>refNode</code> </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if the root container of
		'''   <code>refNode</code> is not an Attr, Document, or DocumentFragment
		'''   node or if <code>refNode</code> is a Document, DocumentFragment,
		'''   Attr, Entity, or Notation node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		WriteOnly Property endBefore As org.w3c.dom.Node

		''' <summary>
		''' Sets the end of a Range to be after a node </summary>
		''' <param name="refNode"> Range ends after <code>refNode</code>. </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if the root container of
		'''   <code>refNode</code> is not an Attr, Document or DocumentFragment
		'''   node or if <code>refNode</code> is a Document, DocumentFragment,
		'''   Attr, Entity, or Notation node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		WriteOnly Property endAfter As org.w3c.dom.Node

		''' <summary>
		''' Collapse a Range onto one of its boundary-points </summary>
		''' <param name="toStart"> If TRUE, collapses the Range onto its start; if FALSE,
		'''   collapses it onto its end. </param>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		Sub collapse(ByVal toStart As Boolean)

		''' <summary>
		''' Select a node and its contents </summary>
		''' <param name="refNode"> The node to select. </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if an ancestor of <code>refNode</code>
		'''   is an Entity, Notation or DocumentType node or if
		'''   <code>refNode</code> is a Document, DocumentFragment, Attr, Entity,
		'''   or Notation node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		Sub selectNode(ByVal refNode As org.w3c.dom.Node)

		''' <summary>
		''' Select the contents within a node </summary>
		''' <param name="refNode"> Node to select from </param>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if <code>refNode</code> or an ancestor
		'''   of <code>refNode</code> is an Entity, Notation or DocumentType node. </exception>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>refNode</code> was created
		'''   from a different document than the one that created this range. </exception>
		Sub selectNodeContents(ByVal refNode As org.w3c.dom.Node)

		' CompareHow
		''' <summary>
		''' Compare start boundary-point of <code>sourceRange</code> to start
		''' boundary-point of Range on which <code>compareBoundaryPoints</code>
		''' is invoked.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short START_TO_START = 0;
		''' <summary>
		''' Compare start boundary-point of <code>sourceRange</code> to end
		''' boundary-point of Range on which <code>compareBoundaryPoints</code>
		''' is invoked.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short START_TO_END = 1;
		''' <summary>
		''' Compare end boundary-point of <code>sourceRange</code> to end
		''' boundary-point of Range on which <code>compareBoundaryPoints</code>
		''' is invoked.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short END_TO_END = 2;
		''' <summary>
		''' Compare end boundary-point of <code>sourceRange</code> to start
		''' boundary-point of Range on which <code>compareBoundaryPoints</code>
		''' is invoked.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short END_TO_START = 3;

		''' <summary>
		''' Compare the boundary-points of two Ranges in a document. </summary>
		''' <param name="how"> A code representing the type of comparison, as defined
		'''   above. </param>
		''' <param name="sourceRange"> The <code>Range</code> on which this current
		'''   <code>Range</code> is compared to. </param>
		''' <returns>  -1, 0 or 1 depending on whether the corresponding
		'''   boundary-point of the Range is respectively before, equal to, or
		'''   after the corresponding boundary-point of <code>sourceRange</code>. </returns>
		''' <exception cref="DOMException">
		'''   WRONG_DOCUMENT_ERR: Raised if the two Ranges are not in the same
		'''   Document or DocumentFragment.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		Function compareBoundaryPoints(ByVal how As Short, ByVal sourceRange As Range) As Short

		''' <summary>
		''' Removes the contents of a Range from the containing document or
		''' document fragment without returning a reference to the removed
		''' content. </summary>
		''' <exception cref="DOMException">
		'''   NO_MODIFICATION_ALLOWED_ERR: Raised if any portion of the content of
		'''   the Range is read-only or any of the nodes that contain any of the
		'''   content of the Range are read-only.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		Sub deleteContents()

		''' <summary>
		''' Moves the contents of a Range from the containing document or document
		''' fragment to a new DocumentFragment. </summary>
		''' <returns> A DocumentFragment containing the extracted contents. </returns>
		''' <exception cref="DOMException">
		'''   NO_MODIFICATION_ALLOWED_ERR: Raised if any portion of the content of
		'''   the Range is read-only or any of the nodes which contain any of the
		'''   content of the Range are read-only.
		'''   <br>HIERARCHY_REQUEST_ERR: Raised if a DocumentType node would be
		'''   extracted into the new DocumentFragment.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		Function extractContents() As org.w3c.dom.DocumentFragment

		''' <summary>
		''' Duplicates the contents of a Range </summary>
		''' <returns> A DocumentFragment that contains content equivalent to this
		'''   Range. </returns>
		''' <exception cref="DOMException">
		'''   HIERARCHY_REQUEST_ERR: Raised if a DocumentType node would be
		'''   extracted into the new DocumentFragment.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		Function cloneContents() As org.w3c.dom.DocumentFragment

		''' <summary>
		''' Inserts a node into the Document or DocumentFragment at the start of
		''' the Range. If the container is a Text node, this will be split at the
		''' start of the Range (as if the Text node's splitText method was
		''' performed at the insertion point) and the insertion will occur
		''' between the two resulting Text nodes. Adjacent Text nodes will not be
		''' automatically merged. If the node to be inserted is a
		''' DocumentFragment node, the children will be inserted rather than the
		''' DocumentFragment node itself. </summary>
		''' <param name="newNode"> The node to insert at the start of the Range </param>
		''' <exception cref="DOMException">
		'''   NO_MODIFICATION_ALLOWED_ERR: Raised if an ancestor container of the
		'''   start of the Range is read-only.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code>newNode</code> and the
		'''   container of the start of the Range were not created from the same
		'''   document.
		'''   <br>HIERARCHY_REQUEST_ERR: Raised if the container of the start of
		'''   the Range is of a type that does not allow children of the type of
		'''   <code>newNode</code> or if <code>newNode</code> is an ancestor of
		'''   the container.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		''' <exception cref="RangeException">
		'''   INVALID_NODE_TYPE_ERR: Raised if <code>newNode</code> is an Attr,
		'''   Entity, Notation, or Document node. </exception>
		Sub insertNode(ByVal newNode As org.w3c.dom.Node)

		''' <summary>
		''' Reparents the contents of the Range to the given node and inserts the
		''' node at the position of the start of the Range. </summary>
		''' <param name="newParent"> The node to surround the contents with. </param>
		''' <exception cref="DOMException">
		'''   NO_MODIFICATION_ALLOWED_ERR: Raised if an ancestor container of
		'''   either boundary-point of the Range is read-only.
		'''   <br>WRONG_DOCUMENT_ERR: Raised if <code> newParent</code> and the
		'''   container of the start of the Range were not created from the same
		'''   document.
		'''   <br>HIERARCHY_REQUEST_ERR: Raised if the container of the start of
		'''   the Range is of a type that does not allow children of the type of
		'''   <code>newParent</code> or if <code>newParent</code> is an ancestor
		'''   of the container or if <code>node</code> would end up with a child
		'''   node of a type not allowed by the type of <code>node</code>.
		'''   <br>INVALID_STATE_ERR: Raised if <code>detach()</code> has already
		'''   been invoked on this object. </exception>
		''' <exception cref="RangeException">
		'''   BAD_BOUNDARYPOINTS_ERR: Raised if the Range partially selects a
		'''   non-text node.
		'''   <br>INVALID_NODE_TYPE_ERR: Raised if <code> node</code> is an Attr,
		'''   Entity, DocumentType, Notation, Document, or DocumentFragment node. </exception>
		Sub surroundContents(ByVal newParent As org.w3c.dom.Node)

		''' <summary>
		''' Produces a new Range whose boundary-points are equal to the
		''' boundary-points of the Range. </summary>
		''' <returns> The duplicated Range. </returns>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		Function cloneRange() As Range

		''' <summary>
		''' Returns the contents of a Range as a string. This string contains only
		''' the data characters, not any markup. </summary>
		''' <returns> The contents of the Range. </returns>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		Function ToString() As String

		''' <summary>
		''' Called to indicate that the Range is no longer in use and that the
		''' implementation may relinquish any resources associated with this
		''' Range. Subsequent calls to any methods or attribute getters on this
		''' Range will result in a <code>DOMException</code> being thrown with an
		''' error code of <code>INVALID_STATE_ERR</code>. </summary>
		''' <exception cref="DOMException">
		'''   INVALID_STATE_ERR: Raised if <code>detach()</code> has already been
		'''   invoked on this object. </exception>
		Sub detach()

	End Interface

End Namespace