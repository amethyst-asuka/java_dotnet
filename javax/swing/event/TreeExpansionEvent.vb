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
	''' An event used to identify a single path in a tree.  The source
	''' returned by <b>getSource</b> will be an instance of JTree.
	''' <p>
	''' For further documentation and examples see
	''' the following sections in <em>The Java Tutorial</em>:
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/treeexpansionlistener.html">How to Write a Tree Expansion Listener</a> and
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/treewillexpandlistener.html">How to Write a Tree-Will-Expand Listener</a>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Scott Violet
	''' </summary>
	Public Class TreeExpansionEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Path to the value this event represents.
		''' </summary>
		Protected Friend path As javax.swing.tree.TreePath

		''' <summary>
		''' Constructs a TreeExpansionEvent object.
		''' </summary>
		''' <param name="source">  the Object that originated the event
		'''                (typically <code>this</code>) </param>
		''' <param name="path">    a TreePath object identifying the newly expanded
		'''                node </param>
		Public Sub New(ByVal source As Object, ByVal path As javax.swing.tree.TreePath)
			MyBase.New(source)
			Me.path = path
		End Sub

		''' <summary>
		''' Returns the path to the value that has been expanded/collapsed.
		''' </summary>
		Public Overridable Property path As javax.swing.tree.TreePath
			Get
				Return path
			End Get
		End Property
	End Class

End Namespace