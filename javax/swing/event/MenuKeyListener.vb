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
	''' MenuKeyListener
	''' 
	''' @author Georges Saab
	''' </summary>
	Public Interface MenuKeyListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a key has been typed.
		''' This event occurs when a key press is followed by a key release.
		''' </summary>
		Sub menuKeyTyped(ByVal e As MenuKeyEvent)

		''' <summary>
		''' Invoked when a key has been pressed.
		''' </summary>
		Sub menuKeyPressed(ByVal e As MenuKeyEvent)

		''' <summary>
		''' Invoked when a key has been released.
		''' </summary>
		Sub menuKeyReleased(ByVal e As MenuKeyEvent)
	End Interface

End Namespace