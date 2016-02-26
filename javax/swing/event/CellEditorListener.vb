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
	''' CellEditorListener defines the interface for an object that listens
	''' to changes in a CellEditor
	''' 
	''' @author Alan Chung
	''' </summary>

	Public Interface CellEditorListener
		Inherits java.util.EventListener

		''' <summary>
		''' This tells the listeners the editor has ended editing </summary>
		Sub editingStopped(ByVal e As javax.swing.event.ChangeEvent)

		''' <summary>
		''' This tells the listeners the editor has canceled editing </summary>
		Sub editingCanceled(ByVal e As javax.swing.event.ChangeEvent)
	End Interface

End Namespace