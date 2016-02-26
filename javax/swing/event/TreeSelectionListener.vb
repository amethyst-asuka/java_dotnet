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
	''' The listener that's notified when the selection in a TreeSelectionModel
	''' changes.
	''' For more information and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/events/treeselectionlistener.html">How to Write a Tree Selection Listener</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' </summary>
	''' <seealso cref= javax.swing.tree.TreeSelectionModel </seealso>
	''' <seealso cref= javax.swing.JTree
	''' 
	''' @author Scott Violet </seealso>
	Public Interface TreeSelectionListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called whenever the value of the selection changes. </summary>
		''' <param name="e"> the event that characterizes the change. </param>
		Sub valueChanged(ByVal e As TreeSelectionEvent)
	End Interface

End Namespace