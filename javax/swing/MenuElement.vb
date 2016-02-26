'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing


	''' <summary>
	''' Any component that can be placed into a menu should implement this interface.
	''' This interface is used by <code>MenuSelectionManager</code>
	''' to handle selection and navigation in menu hierarchies.
	''' 
	''' @author Arnaud Weber
	''' </summary>

	Public Interface MenuElement

		''' <summary>
		''' Processes a mouse event. <code>event</code> is a <code>MouseEvent</code>
		''' with source being the receiving element's component.
		''' <code>path</code> is the path of the receiving element in the menu
		''' hierarchy including the receiving element itself.
		''' <code>manager</code> is the <code>MenuSelectionManager</code>
		''' for the menu hierarchy.
		''' This method should process the <code>MouseEvent</code> and change
		''' the menu selection if necessary
		''' by using <code>MenuSelectionManager</code>'s API
		''' Note: you do not have to forward the event to sub-components.
		''' This is done automatically by the <code>MenuSelectionManager</code>.
		''' </summary>
		Sub processMouseEvent(ByVal [event] As MouseEvent, MenuElement ByVal  As path(), ByVal manager As MenuSelectionManager)


		''' <summary>
		'''  Process a key event.
		''' </summary>
		Sub processKeyEvent(ByVal [event] As KeyEvent, MenuElement ByVal  As path(), ByVal manager As MenuSelectionManager)

		''' <summary>
		''' Call by the <code>MenuSelectionManager</code> when the
		''' <code>MenuElement</code> is added or remove from
		''' the menu selection.
		''' </summary>
		Sub menuSelectionChanged(ByVal isIncluded As Boolean)

		''' <summary>
		''' This method should return an array containing the sub-elements for the receiving menu element
		''' </summary>
		''' <returns> an array of MenuElements </returns>
		ReadOnly Property subElements As MenuElement()

		''' <summary>
		''' This method should return the java.awt.Component used to paint the receiving element.
		''' The returned component will be used to convert events and detect if an event is inside
		''' a MenuElement's component.
		''' </summary>
		''' <returns> the Component value </returns>
		ReadOnly Property component As Component
	End Interface

End Namespace