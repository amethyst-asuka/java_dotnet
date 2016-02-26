'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' Listener for changes in the caret position of a text
	''' component.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface CaretListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called when the caret position is updated.
		''' </summary>
		''' <param name="e"> the caret event </param>
		Sub caretUpdate(ByVal e As CaretEvent)
	End Interface

End Namespace