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
	''' Filters are objects that know how to "filter out" nodes. If a
	''' <code>NodeIterator</code> or <code>TreeWalker</code> is given a
	''' <code>NodeFilter</code>, it applies the filter before it returns the next
	''' node. If the filter says to accept the node, the traversal logic returns
	''' it; otherwise, traversal looks for the next node and pretends that the
	''' node that was rejected was not there.
	''' <p>The DOM does not provide any filters. <code>NodeFilter</code> is just an
	''' interface that users can implement to provide their own filters.
	''' <p><code>NodeFilters</code> do not need to know how to traverse from node
	''' to node, nor do they need to know anything about the data structure that
	''' is being traversed. This makes it very easy to write filters, since the
	''' only thing they have to know how to do is evaluate a single node. One
	''' filter may be used with a number of different kinds of traversals,
	''' encouraging code reuse.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Traversal-Range-20001113'>Document Object Model (DOM) Level 2 Traversal and Range Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface NodeFilter
		' Constants returned by acceptNode
		''' <summary>
		''' Accept the node. Navigation methods defined for
		''' <code>NodeIterator</code> or <code>TreeWalker</code> will return this
		''' node.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short FILTER_ACCEPT = 1;
		''' <summary>
		''' Reject the node. Navigation methods defined for
		''' <code>NodeIterator</code> or <code>TreeWalker</code> will not return
		''' this node. For <code>TreeWalker</code>, the children of this node
		''' will also be rejected. <code>NodeIterators</code> treat this as a
		''' synonym for <code>FILTER_SKIP</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short FILTER_REJECT = 2;
		''' <summary>
		''' Skip this single node. Navigation methods defined for
		''' <code>NodeIterator</code> or <code>TreeWalker</code> will not return
		''' this node. For both <code>NodeIterator</code> and
		''' <code>TreeWalker</code>, the children of this node will still be
		''' considered.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short FILTER_SKIP = 3;

		' Constants for whatToShow
		''' <summary>
		''' Show all <code>Nodes</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_ALL = &HFFFFFFFF;
		''' <summary>
		''' Show <code>Element</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_ELEMENT = &H1;
		''' <summary>
		''' Show <code>Attr</code> nodes. This is meaningful only when creating an
		''' <code>NodeIterator</code> or <code>TreeWalker</code> with an
		''' attribute node as its <code>root</code>; in this case, it means that
		''' the attribute node will appear in the first position of the iteration
		''' or traversal. Since attributes are never children of other nodes,
		''' they do not appear when traversing over the document tree.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_ATTRIBUTE = &H2;
		''' <summary>
		''' Show <code>Text</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_TEXT = &H4;
		''' <summary>
		''' Show <code>CDATASection</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_CDATA_SECTION = &H8;
		''' <summary>
		''' Show <code>EntityReference</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_ENTITY_REFERENCE = &H10;
		''' <summary>
		''' Show <code>Entity</code> nodes. This is meaningful only when creating
		''' an <code>NodeIterator</code> or <code>TreeWalker</code> with an
		''' <code>Entity</code> node as its <code>root</code>; in this case, it
		''' means that the <code>Entity</code> node will appear in the first
		''' position of the traversal. Since entities are not part of the
		''' document tree, they do not appear when traversing over the document
		''' tree.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_ENTITY = &H20;
		''' <summary>
		''' Show <code>ProcessingInstruction</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_PROCESSING_INSTRUCTION = &H40;
		''' <summary>
		''' Show <code>Comment</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_COMMENT = &H80;
		''' <summary>
		''' Show <code>Document</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_DOCUMENT = &H100;
		''' <summary>
		''' Show <code>DocumentType</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_DOCUMENT_TYPE = &H200;
		''' <summary>
		''' Show <code>DocumentFragment</code> nodes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_DOCUMENT_FRAGMENT = &H400;
		''' <summary>
		''' Show <code>Notation</code> nodes. This is meaningful only when creating
		''' an <code>NodeIterator</code> or <code>TreeWalker</code> with a
		''' <code>Notation</code> node as its <code>root</code>; in this case, it
		''' means that the <code>Notation</code> node will appear in the first
		''' position of the traversal. Since notations are not part of the
		''' document tree, they do not appear when traversing over the document
		''' tree.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SHOW_NOTATION = &H800;

		''' <summary>
		''' Test whether a specified node is visible in the logical view of a
		''' <code>TreeWalker</code> or <code>NodeIterator</code>. This function
		''' will be called by the implementation of <code>TreeWalker</code> and
		''' <code>NodeIterator</code>; it is not normally called directly from
		''' user code. (Though you could do so if you wanted to use the same
		''' filter to guide your own application logic.) </summary>
		''' <param name="n"> The node to check to see if it passes the filter or not. </param>
		''' <returns> A constant to determine whether the node is accepted,
		'''   rejected, or skipped, as defined above. </returns>
		Function acceptNode(ByVal n As org.w3c.dom.Node) As Short

	End Interface

End Namespace