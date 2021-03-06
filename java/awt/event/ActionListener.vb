'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event


	''' <summary>
	''' The listener interface for receiving action events.
	''' The class that is interested in processing an action event
	''' implements this interface, and the object created with that
	''' class is registered with a component, using the component's
	''' <code>addActionListener</code> method. When the action event
	''' occurs, that object's <code>actionPerformed</code> method is
	''' invoked.
	''' </summary>
	''' <seealso cref= ActionEvent </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/actionlistener.html">How to Write an Action Listener</a>
	''' 
	''' @author Carl Quinn
	''' @since 1.1 </seealso>
	Public Interface ActionListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when an action occurs.
		''' </summary>
		Sub actionPerformed(  e As ActionEvent)

	End Interface

End Namespace