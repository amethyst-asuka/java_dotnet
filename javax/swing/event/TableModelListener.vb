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
	''' TableModelListener defines the interface for an object that listens
	''' to changes in a TableModel.
	''' 
	''' @author Alan Chung </summary>
	''' <seealso cref= javax.swing.table.TableModel </seealso>

	Public Interface TableModelListener
		Inherits java.util.EventListener

		''' <summary>
		''' This fine grain notification tells listeners the exact range
		''' of cells, rows, or columns that changed.
		''' </summary>
		Sub tableChanged(ByVal e As TableModelEvent)
	End Interface

End Namespace