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
	''' An abstract adapter class for receiving window events.
	''' The methods in this class are empty. This class exists as
	''' convenience for creating listener objects.
	''' <P>
	''' Extend this class to create a <code>WindowEvent</code> listener
	''' and override the methods for the events of interest. (If you implement the
	''' <code>WindowListener</code> interface, you have to define all of
	''' the methods in it. This abstract class defines null methods for them
	''' all, so you can only have to define methods for events you care about.)
	''' <P>
	''' Create a listener object using the extended class and then register it with
	''' a Window using the window's <code>addWindowListener</code>
	''' method. When the window's status changes by virtue of being opened,
	''' closed, activated or deactivated, iconified or deiconified,
	''' the relevant method in the listener
	''' object is invoked, and the <code>WindowEvent</code> is passed to it.
	''' </summary>
	''' <seealso cref= WindowEvent </seealso>
	''' <seealso cref= WindowListener </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/windowlistener.html">Tutorial: Writing a Window Listener</a>
	''' 
	''' @author Carl Quinn
	''' @author Amy Fowler
	''' @author David Mendenhall
	''' @since 1.1 </seealso>
	Public MustInherit Class WindowAdapter
		Implements WindowListener, WindowStateListener, WindowFocusListener

		''' <summary>
		''' Invoked when a window has been opened.
		''' </summary>
		Public Overridable Sub windowOpened(  e As WindowEvent) Implements WindowListener.windowOpened
		End Sub

		''' <summary>
		''' Invoked when a window is in the process of being closed.
		''' The close operation can be overridden at this point.
		''' </summary>
		Public Overridable Sub windowClosing(  e As WindowEvent) Implements WindowListener.windowClosing
		End Sub

		''' <summary>
		''' Invoked when a window has been closed.
		''' </summary>
		Public Overridable Sub windowClosed(  e As WindowEvent) Implements WindowListener.windowClosed
		End Sub

		''' <summary>
		''' Invoked when a window is iconified.
		''' </summary>
		Public Overridable Sub windowIconified(  e As WindowEvent) Implements WindowListener.windowIconified
		End Sub

		''' <summary>
		''' Invoked when a window is de-iconified.
		''' </summary>
		Public Overridable Sub windowDeiconified(  e As WindowEvent) Implements WindowListener.windowDeiconified
		End Sub

		''' <summary>
		''' Invoked when a window is activated.
		''' </summary>
		Public Overridable Sub windowActivated(  e As WindowEvent) Implements WindowListener.windowActivated
		End Sub

		''' <summary>
		''' Invoked when a window is de-activated.
		''' </summary>
		Public Overridable Sub windowDeactivated(  e As WindowEvent) Implements WindowListener.windowDeactivated
		End Sub

		''' <summary>
		''' Invoked when a window state is changed.
		''' @since 1.4
		''' </summary>
		Public Overridable Sub windowStateChanged(  e As WindowEvent) Implements WindowStateListener.windowStateChanged
		End Sub

		''' <summary>
		''' Invoked when the Window is set to be the focused Window, which means
		''' that the Window, or one of its subcomponents, will receive keyboard
		''' events.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Sub windowGainedFocus(  e As WindowEvent) Implements WindowFocusListener.windowGainedFocus
		End Sub

		''' <summary>
		''' Invoked when the Window is no longer the focused Window, which means
		''' that keyboard events will no longer be delivered to the Window or any of
		''' its subcomponents.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Sub windowLostFocus(  e As WindowEvent) Implements WindowFocusListener.windowLostFocus
		End Sub
	End Class

End Namespace