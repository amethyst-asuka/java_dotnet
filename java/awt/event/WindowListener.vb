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
	''' The listener interface for receiving window events.
	''' The class that is interested in processing a window event
	''' either implements this interface (and all the methods it
	''' contains) or extends the abstract <code>WindowAdapter</code> class
	''' (overriding only the methods of interest).
	''' The listener object created from that class is then registered with a
	''' Window using the window's <code>addWindowListener</code>
	''' method. When the window's status changes by virtue of being opened,
	''' closed, activated or deactivated, iconified or deiconified,
	''' the relevant method in the listener object is invoked, and the
	''' <code>WindowEvent</code> is passed to it.
	''' 
	''' @author Carl Quinn
	''' </summary>
	''' <seealso cref= WindowAdapter </seealso>
	''' <seealso cref= WindowEvent </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/windowlistener.html">Tutorial: How to Write Window Listeners</a>
	''' 
	''' @since 1.1 </seealso>
	Public Interface WindowListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked the first time a window is made visible.
		''' </summary>
		Sub windowOpened(  e As WindowEvent)

		''' <summary>
		''' Invoked when the user attempts to close the window
		''' from the window's system menu.
		''' </summary>
		Sub windowClosing(  e As WindowEvent)

		''' <summary>
		''' Invoked when a window has been closed as the result
		''' of calling dispose on the window.
		''' </summary>
		Sub windowClosed(  e As WindowEvent)

		''' <summary>
		''' Invoked when a window is changed from a normal to a
		''' minimized state. For many platforms, a minimized window
		''' is displayed as the icon specified in the window's
		''' iconImage property. </summary>
		''' <seealso cref= java.awt.Frame#setIconImage </seealso>
		Sub windowIconified(  e As WindowEvent)

		''' <summary>
		''' Invoked when a window is changed from a minimized
		''' to a normal state.
		''' </summary>
		Sub windowDeiconified(  e As WindowEvent)

		''' <summary>
		''' Invoked when the Window is set to be the active Window. Only a Frame or
		''' a Dialog can be the active Window. The native windowing system may
		''' denote the active Window or its children with special decorations, such
		''' as a highlighted title bar. The active Window is always either the
		''' focused Window, or the first Frame or Dialog that is an owner of the
		''' focused Window.
		''' </summary>
		Sub windowActivated(  e As WindowEvent)

		''' <summary>
		''' Invoked when a Window is no longer the active Window. Only a Frame or a
		''' Dialog can be the active Window. The native windowing system may denote
		''' the active Window or its children with special decorations, such as a
		''' highlighted title bar. The active Window is always either the focused
		''' Window, or the first Frame or Dialog that is an owner of the focused
		''' Window.
		''' </summary>
		Sub windowDeactivated(  e As WindowEvent)
	End Interface

End Namespace