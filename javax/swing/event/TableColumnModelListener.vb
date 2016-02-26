'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' TableColumnModelListener defines the interface for an object that listens
	''' to changes in a TableColumnModel.
	''' 
	''' @author Alan Chung </summary>
	''' <seealso cref= TableColumnModelEvent </seealso>

	Public Interface TableColumnModelListener
		Inherits java.util.EventListener

		''' <summary>
		''' Tells listeners that a column was added to the model. </summary>
		Sub columnAdded(ByVal e As TableColumnModelEvent)

		''' <summary>
		''' Tells listeners that a column was removed from the model. </summary>
		Sub columnRemoved(ByVal e As TableColumnModelEvent)

		''' <summary>
		''' Tells listeners that a column was repositioned. </summary>
		Sub columnMoved(ByVal e As TableColumnModelEvent)

		''' <summary>
		''' Tells listeners that a column was moved due to a margin change. </summary>
		Sub columnMarginChanged(ByVal e As javax.swing.event.ChangeEvent)

		''' <summary>
		''' Tells listeners that the selection model of the
		''' TableColumnModel changed.
		''' </summary>
		Sub columnSelectionChanged(ByVal e As javax.swing.event.ListSelectionEvent)
	End Interface

End Namespace