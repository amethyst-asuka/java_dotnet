'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the requirements for a tree node object that can change --
	''' by adding or removing child nodes, or by changing the contents
	''' of a user object stored in the node.
	''' </summary>
	''' <seealso cref= DefaultMutableTreeNode </seealso>
	''' <seealso cref= javax.swing.JTree
	''' 
	''' @author Rob Davis
	''' @author Scott Violet </seealso>

	Public Interface MutableTreeNode
		Inherits TreeNode

		''' <summary>
		''' Adds <code>child</code> to the receiver at <code>index</code>.
		''' <code>child</code> will be messaged with <code>setParent</code>.
		''' </summary>
		Sub insert(ByVal child As MutableTreeNode, ByVal index As Integer)

		''' <summary>
		''' Removes the child at <code>index</code> from the receiver.
		''' </summary>
		Sub remove(ByVal index As Integer)

		''' <summary>
		''' Removes <code>node</code> from the receiver. <code>setParent</code>
		''' will be messaged on <code>node</code>.
		''' </summary>
		Sub remove(ByVal node As MutableTreeNode)

		''' <summary>
		''' Resets the user object of the receiver to <code>object</code>.
		''' </summary>
		WriteOnly Property userObject As Object

		''' <summary>
		''' Removes the receiver from its parent.
		''' </summary>
		Sub removeFromParent()

		''' <summary>
		''' Sets the parent of the receiver to <code>newParent</code>.
		''' </summary>
		WriteOnly Property parent As MutableTreeNode
	End Interface

End Namespace