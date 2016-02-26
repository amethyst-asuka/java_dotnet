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
	''' Defines an object which listens for ChangeEvents.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Interface ChangeListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when the target of the listener has changed its state.
		''' </summary>
		''' <param name="e">  a ChangeEvent object </param>
		Sub stateChanged(ByVal e As ChangeEvent)
	End Interface

End Namespace