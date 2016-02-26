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
	''' An abstract adapter class for receiving internal frame events.
	''' The methods in this class are empty. This class exists as
	''' convenience for creating listener objects, and is functionally
	''' equivalent to the WindowAdapter class in the AWT.
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/internalframelistener.html">How to Write an Internal Frame Listener</a>
	''' in <em>The Java Tutorial</em>
	''' </summary>
	''' <seealso cref= InternalFrameEvent </seealso>
	''' <seealso cref= InternalFrameListener </seealso>
	''' <seealso cref= java.awt.event.WindowListener
	''' 
	''' @author Thomas Ball </seealso>
	Public MustInherit Class InternalFrameAdapter
		Implements InternalFrameListener

		''' <summary>
		''' Invoked when an internal frame has been opened.
		''' </summary>
		Public Overridable Sub internalFrameOpened(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameOpened
		End Sub

		''' <summary>
		''' Invoked when an internal frame is in the process of being closed.
		''' The close operation can be overridden at this point.
		''' </summary>
		Public Overridable Sub internalFrameClosing(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosing
		End Sub

		''' <summary>
		''' Invoked when an internal frame has been closed.
		''' </summary>
		Public Overridable Sub internalFrameClosed(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameClosed
		End Sub

		''' <summary>
		''' Invoked when an internal frame is iconified.
		''' </summary>
		Public Overridable Sub internalFrameIconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameIconified
		End Sub

		''' <summary>
		''' Invoked when an internal frame is de-iconified.
		''' </summary>
		Public Overridable Sub internalFrameDeiconified(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeiconified
		End Sub

		''' <summary>
		''' Invoked when an internal frame is activated.
		''' </summary>
		Public Overridable Sub internalFrameActivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameActivated
		End Sub

		''' <summary>
		''' Invoked when an internal frame is de-activated.
		''' </summary>
		Public Overridable Sub internalFrameDeactivated(ByVal e As InternalFrameEvent) Implements InternalFrameListener.internalFrameDeactivated
		End Sub
	End Class

End Namespace