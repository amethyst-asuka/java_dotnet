Imports javax.swing.undo

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
	''' Interface implemented by a class interested in hearing about
	''' undoable operations.
	''' 
	''' @author Ray Ryan
	''' </summary>

	Public Interface UndoableEditListener
		Inherits java.util.EventListener

		''' <summary>
		''' An undoable edit happened
		''' </summary>
		Sub undoableEditHappened(ByVal e As UndoableEditEvent)
	End Interface

End Namespace