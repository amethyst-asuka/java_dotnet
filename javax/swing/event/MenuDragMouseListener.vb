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
	''' Defines a menu mouse-drag listener.
	''' 
	''' @author Georges Saab
	''' </summary>
	Public Interface MenuDragMouseListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when the dragged mouse has entered a menu component's
		''' display area.
		''' </summary>
		''' <param name="e">  a MenuDragMouseEvent object </param>
		Sub menuDragMouseEntered(ByVal e As MenuDragMouseEvent)
		''' <summary>
		''' Invoked when the dragged mouse has left a menu component's
		''' display area.
		''' </summary>
		''' <param name="e">  a MenuDragMouseEvent object </param>
		Sub menuDragMouseExited(ByVal e As MenuDragMouseEvent)
		''' <summary>
		''' Invoked when the mouse is being dragged in a menu component's
		''' display area.
		''' </summary>
		''' <param name="e">  a MenuDragMouseEvent object </param>
		Sub menuDragMouseDragged(ByVal e As MenuDragMouseEvent)
		''' <summary>
		''' Invoked when a dragged mouse is release in a menu component's
		''' display area.
		''' </summary>
		''' <param name="e">  a MenuDragMouseEvent object </param>
		Sub menuDragMouseReleased(ByVal e As MenuDragMouseEvent)
	End Interface

End Namespace