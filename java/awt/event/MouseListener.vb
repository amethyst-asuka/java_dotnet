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
	''' The listener interface for receiving "interesting" mouse events
	''' (press, release, click, enter, and exit) on a component.
	''' (To track mouse moves and mouse drags, use the
	''' <code>MouseMotionListener</code>.)
	''' <P>
	''' The class that is interested in processing a mouse event
	''' either implements this interface (and all the methods it
	''' contains) or extends the abstract <code>MouseAdapter</code> class
	''' (overriding only the methods of interest).
	''' <P>
	''' The listener object created from that class is then registered with a
	''' component using the component's <code>addMouseListener</code>
	''' method. A mouse event is generated when the mouse is pressed, released
	''' clicked (pressed and released). A mouse event is also generated when
	''' the mouse cursor enters or leaves a component. When a mouse event
	''' occurs, the relevant method in the listener object is invoked, and
	''' the <code>MouseEvent</code> is passed to it.
	''' 
	''' @author Carl Quinn
	''' </summary>
	''' <seealso cref= MouseAdapter </seealso>
	''' <seealso cref= MouseEvent </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/mouselistener.html">Tutorial: Writing a Mouse Listener</a>
	''' 
	''' @since 1.1 </seealso>
	Public Interface MouseListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when the mouse button has been clicked (pressed
		''' and released) on a component.
		''' </summary>
		Sub mouseClicked(ByVal e As MouseEvent)

		''' <summary>
		''' Invoked when a mouse button has been pressed on a component.
		''' </summary>
		Sub mousePressed(ByVal e As MouseEvent)

		''' <summary>
		''' Invoked when a mouse button has been released on a component.
		''' </summary>
		Sub mouseReleased(ByVal e As MouseEvent)

		''' <summary>
		''' Invoked when the mouse enters a component.
		''' </summary>
		Sub mouseEntered(ByVal e As MouseEvent)

		''' <summary>
		''' Invoked when the mouse exits a component.
		''' </summary>
		Sub mouseExited(ByVal e As MouseEvent)
	End Interface

End Namespace