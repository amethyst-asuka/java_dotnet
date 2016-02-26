'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines a listener for menu events.
	''' 
	''' @author Georges Saab
	''' </summary>
	Public Interface MenuListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a menu is selected.
		''' </summary>
		''' <param name="e">  a MenuEvent object </param>
		Sub menuSelected(ByVal e As MenuEvent)
		''' <summary>
		''' Invoked when the menu is deselected.
		''' </summary>
		''' <param name="e">  a MenuEvent object </param>
		Sub menuDeselected(ByVal e As MenuEvent)
		''' <summary>
		''' Invoked when the menu is canceled.
		''' </summary>
		''' <param name="e">  a MenuEvent object </param>
		Sub menuCanceled(ByVal e As MenuEvent)
	End Interface

End Namespace