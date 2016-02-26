'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' ListDataListener
	''' 
	''' @author Hans Muller
	''' </summary>
	Public Interface ListDataListener
		Inherits java.util.EventListener

		''' <summary>
		''' Sent after the indices in the index0,index1
		''' interval have been inserted in the data model.
		''' The new interval includes both index0 and index1.
		''' </summary>
		''' <param name="e">  a <code>ListDataEvent</code> encapsulating the
		'''    event information </param>
		Sub intervalAdded(ByVal e As ListDataEvent)


		''' <summary>
		''' Sent after the indices in the index0,index1 interval
		''' have been removed from the data model.  The interval
		''' includes both index0 and index1.
		''' </summary>
		''' <param name="e">  a <code>ListDataEvent</code> encapsulating the
		'''    event information </param>
		Sub intervalRemoved(ByVal e As ListDataEvent)


		''' <summary>
		''' Sent when the contents of the list has changed in a way
		''' that's too complex to characterize with the previous
		''' methods. For example, this is sent when an item has been
		''' replaced. Index0 and index1 bracket the change.
		''' </summary>
		''' <param name="e">  a <code>ListDataEvent</code> encapsulating the
		'''    event information </param>
		Sub contentsChanged(ByVal e As ListDataEvent)
	End Interface

End Namespace