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
	''' A popup menu listener
	''' 
	''' @author Arnaud Weber
	''' </summary>
	Public Interface PopupMenuListener
		Inherits java.util.EventListener

		''' <summary>
		'''  This method is called before the popup menu becomes visible
		''' </summary>
		Sub popupMenuWillBecomeVisible(ByVal e As PopupMenuEvent)

		''' <summary>
		''' This method is called before the popup menu becomes invisible
		''' Note that a JPopupMenu can become invisible any time
		''' </summary>
		Sub popupMenuWillBecomeInvisible(ByVal e As PopupMenuEvent)

		''' <summary>
		''' This method is called when the popup menu is canceled
		''' </summary>
		Sub popupMenuCanceled(ByVal e As PopupMenuEvent)
	End Interface

End Namespace