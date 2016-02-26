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
	''' HyperlinkListener
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface HyperlinkListener
		Inherits java.util.EventListener

		''' <summary>
		''' Called when a hypertext link is updated.
		''' </summary>
		''' <param name="e"> the event responsible for the update </param>
		Sub hyperlinkUpdate(ByVal e As HyperlinkEvent)
	End Interface

End Namespace