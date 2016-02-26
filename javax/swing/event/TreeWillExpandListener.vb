'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.event


	''' <summary>
	''' The listener that's notified when a tree expands or collapses
	''' a node.
	''' For further information and examples see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/treewillexpandlistener.html">How to Write a Tree-Will-Expand Listener</a>,
	''' a section in <em>The Java Tutorial.</em>
	'''  
	''' @author Scott Violet
	''' </summary>

	Public Interface TreeWillExpandListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked whenever a node in the tree is about to be expanded.
		''' </summary>
		Sub treeWillExpand(ByVal [event] As TreeExpansionEvent)

		''' <summary>
		''' Invoked whenever a node in the tree is about to be collapsed.
		''' </summary>
		Sub treeWillCollapse(ByVal [event] As TreeExpansionEvent)
	End Interface

End Namespace