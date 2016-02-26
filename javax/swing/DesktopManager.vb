'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' DesktopManager objects are owned by a JDesktopPane object. They are responsible
	''' for implementing L&amp;F specific behaviors for the JDesktopPane. JInternalFrame
	''' implementations should delegate specific behaviors to the DesktopManager. For
	''' instance, if a JInternalFrame was asked to iconify, it should try:
	''' <PRE>
	'''    getDesktopPane().getDesktopManager().iconifyFrame(frame);
	''' </PRE>
	''' This delegation allows each L&amp;F to provide custom behaviors for desktop-specific
	''' actions. (For example, how and where the internal frame's icon would appear.)
	''' <p>This class provides a policy for the various JInternalFrame methods, it is not
	''' meant to be called directly rather the various JInternalFrame methods will call
	''' into the DesktopManager.</p>
	''' </summary>
	''' <seealso cref= JDesktopPane </seealso>
	''' <seealso cref= JInternalFrame </seealso>
	''' <seealso cref= JInternalFrame.JDesktopIcon
	'''  
	''' @author David Kloba </seealso>
	Public Interface DesktopManager
		''' <summary>
		''' If possible, display this frame in an appropriate location.
		''' Normally, this is not called, as the creator of the JInternalFrame
		''' will add the frame to the appropriate parent.
		''' </summary>
		Sub openFrame(ByVal f As JInternalFrame)

		''' <summary>
		''' Generally, this call should remove the frame from it's parent. </summary>
		Sub closeFrame(ByVal f As JInternalFrame)

		''' <summary>
		''' Generally, the frame should be resized to match it's parents bounds. </summary>
		Sub maximizeFrame(ByVal f As JInternalFrame)
		''' <summary>
		''' Generally, this indicates that the frame should be restored to it's
		''' size and position prior to a maximizeFrame() call.
		''' </summary>
		Sub minimizeFrame(ByVal f As JInternalFrame)
		''' <summary>
		''' Generally, remove this frame from it's parent and add an iconic representation. </summary>
		Sub iconifyFrame(ByVal f As JInternalFrame)
		''' <summary>
		''' Generally, remove any iconic representation that is present and restore the
		''' frame to it's original size and location.
		''' </summary>
		Sub deiconifyFrame(ByVal f As JInternalFrame)

		''' <summary>
		''' Generally, indicate that this frame has focus. This is usually called after
		''' the JInternalFrame's IS_SELECTED_PROPERTY has been set to true.
		''' </summary>
		Sub activateFrame(ByVal f As JInternalFrame)

		''' <summary>
		''' Generally, indicate that this frame has lost focus. This is usually called
		''' after the JInternalFrame's IS_SELECTED_PROPERTY has been set to false.
		''' </summary>
		Sub deactivateFrame(ByVal f As JInternalFrame)

		''' <summary>
		''' This method is normally called when the user has indicated that
		''' they will begin dragging a component around. This method should be called
		''' prior to any dragFrame() calls to allow the DesktopManager to prepare any
		''' necessary state. Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub beginDraggingFrame(ByVal f As JComponent)

		''' <summary>
		''' The user has moved the frame. Calls to this method will be preceded by calls
		''' to beginDraggingFrame().
		'''  Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub dragFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer)
		''' <summary>
		''' This method signals the end of the dragging session. Any state maintained by
		''' the DesktopManager can be removed here.  Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub endDraggingFrame(ByVal f As JComponent)

		''' <summary>
		''' This methods is normally called when the user has indicated that
		''' they will begin resizing the frame. This method should be called
		''' prior to any resizeFrame() calls to allow the DesktopManager to prepare any
		''' necessary state.  Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub beginResizingFrame(ByVal f As JComponent, ByVal direction As Integer)
		''' <summary>
		''' The user has resized the component. Calls to this method will be preceded by calls
		''' to beginResizingFrame().
		'''  Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub resizeFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer)
		''' <summary>
		''' This method signals the end of the resize session. Any state maintained by
		''' the DesktopManager can be removed here.  Normally <b>f</b> will be a JInternalFrame.
		''' </summary>
		Sub endResizingFrame(ByVal f As JComponent)

		''' <summary>
		''' This is a primitive reshape method. </summary>
		Sub setBoundsForFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer)
	End Interface

End Namespace