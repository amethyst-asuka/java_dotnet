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

Namespace javax.swing.event


	''' <summary>
	''' The listener that's notified when a tree expands or collapses
	''' a node.
	''' For further documentation and examples see
	''' <a
	'''  href="https://docs.oracle.com/javase/tutorial/uiswing/events/treeexpansionlistener.html">How to Write a Tree Expansion Listener</a>,
	''' a section in <em>The Java Tutorial.</em>
	'''  
	''' @author Scott Violet
	''' </summary>

	Public Interface TreeExpansionListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called whenever an item in the tree has been expanded.
		''' </summary>
		Sub treeExpanded(ByVal [event] As TreeExpansionEvent)

		''' <summary>
		''' Called whenever an item in the tree has been collapsed.
		''' </summary>
		Sub treeCollapsed(ByVal [event] As TreeExpansionEvent)
	End Interface

End Namespace