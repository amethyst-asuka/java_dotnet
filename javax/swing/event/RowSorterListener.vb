'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>RowSorterListener</code>s are notified of changes to a
	''' <code>RowSorter</code>.
	''' </summary>
	''' <seealso cref= javax.swing.RowSorter
	''' @since 1.6 </seealso>
	Public Interface RowSorterListener
		Inherits java.util.EventListener

		''' <summary>
		''' Notification that the <code>RowSorter</code> has changed.  The event
		''' describes the scope of the change.
		''' </summary>
		''' <param name="e"> the event, will not be null </param>
		Sub sorterChanged(ByVal e As RowSorterEvent)
	End Interface

End Namespace