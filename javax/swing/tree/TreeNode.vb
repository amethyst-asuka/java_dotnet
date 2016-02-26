Imports System.Collections

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.tree


	''' <summary>
	''' Defines the requirements for an object that can be used as a
	''' tree node in a JTree.
	''' <p>
	''' Implementations of <code>TreeNode</code> that override <code>equals</code>
	''' will typically need to override <code>hashCode</code> as well.  Refer
	''' to <seealso cref="javax.swing.tree.TreeModel"/> for more information.
	''' 
	''' For further information and examples of using tree nodes,
	''' see <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/tree.html">How to Use Tree Nodes</a>
	''' in <em>The Java Tutorial.</em>
	''' 
	''' @author Rob Davis
	''' @author Scott Violet
	''' </summary>

	Public Interface TreeNode
		''' <summary>
		''' Returns the child <code>TreeNode</code> at index
		''' <code>childIndex</code>.
		''' </summary>
		Function getChildAt(ByVal childIndex As Integer) As TreeNode

		''' <summary>
		''' Returns the number of children <code>TreeNode</code>s the receiver
		''' contains.
		''' </summary>
		ReadOnly Property childCount As Integer

		''' <summary>
		''' Returns the parent <code>TreeNode</code> of the receiver.
		''' </summary>
		ReadOnly Property parent As TreeNode

		''' <summary>
		''' Returns the index of <code>node</code> in the receivers children.
		''' If the receiver does not contain <code>node</code>, -1 will be
		''' returned.
		''' </summary>
		Function getIndex(ByVal node As TreeNode) As Integer

		''' <summary>
		''' Returns true if the receiver allows children.
		''' </summary>
		ReadOnly Property allowsChildren As Boolean

		''' <summary>
		''' Returns true if the receiver is a leaf.
		''' </summary>
		ReadOnly Property leaf As Boolean

		''' <summary>
		''' Returns the children of the receiver as an <code>Enumeration</code>.
		''' </summary>
		Function children() As System.Collections.IEnumerator
	End Interface

End Namespace