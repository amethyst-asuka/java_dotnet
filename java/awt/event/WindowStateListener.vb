'
' * Copyright (c) 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The listener interface for receiving window state events.
	''' <p>
	''' The class that is interested in processing a window state event
	''' either implements this interface (and all the methods it contains)
	''' or extends the abstract <code>WindowAdapter</code> class
	''' (overriding only the methods of interest).
	''' <p>
	''' The listener object created from that class is then registered with
	''' a window using the <code>Window</code>'s
	''' <code>addWindowStateListener</code> method.  When the window's
	''' state changes by virtue of being iconified, maximized etc., the
	''' <code>windowStateChanged</code> method in the listener object is
	''' invoked, and the <code>WindowEvent</code> is passed to it.
	''' </summary>
	''' <seealso cref= java.awt.event.WindowAdapter </seealso>
	''' <seealso cref= java.awt.event.WindowEvent
	''' 
	''' @since 1.4 </seealso>
	Public Interface WindowStateListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when window state is changed.
		''' </summary>
		Sub windowStateChanged(ByVal e As WindowEvent)
	End Interface

End Namespace