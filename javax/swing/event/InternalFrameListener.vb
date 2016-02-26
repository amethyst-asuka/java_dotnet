'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The listener interface for receiving internal frame events.
	''' This class is functionally equivalent to the WindowListener class
	''' in the AWT.
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/internalframelistener.html">How to Write an Internal Frame Listener</a>
	''' in <em>The Java Tutorial</em> for further documentation.
	''' </summary>
	''' <seealso cref= java.awt.event.WindowListener
	''' 
	''' @author Thomas Ball </seealso>
	Public Interface InternalFrameListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a internal frame has been opened. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#show </seealso>
		Sub internalFrameOpened(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame is in the process of being closed.
		''' The close operation can be overridden at this point. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setDefaultCloseOperation </seealso>
		Sub internalFrameClosing(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame has been closed. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setClosed </seealso>
		Sub internalFrameClosed(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame is iconified. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setIcon </seealso>
		Sub internalFrameIconified(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame is de-iconified. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setIcon </seealso>
		Sub internalFrameDeiconified(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame is activated. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setSelected </seealso>
		Sub internalFrameActivated(ByVal e As InternalFrameEvent)

		''' <summary>
		''' Invoked when an internal frame is de-activated. </summary>
		''' <seealso cref= javax.swing.JInternalFrame#setSelected </seealso>
		Sub internalFrameDeactivated(ByVal e As InternalFrameEvent)
	End Interface

End Namespace