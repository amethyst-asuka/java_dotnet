'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.peer


	''' <summary>
	''' The peer interface for <seealso cref="Window"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface WindowPeer
		Inherits ContainerPeer

		''' <summary>
		''' Makes this window the topmost window on the desktop.
		''' </summary>
		''' <seealso cref= Window#toFront() </seealso>
		Sub toFront()

		''' <summary>
		''' Makes this window the bottommost window on the desktop.
		''' </summary>
		''' <seealso cref= Window#toBack() </seealso>
		Sub toBack()

		''' <summary>
		''' Updates the window's always-on-top state.
		''' Sets if the window should always stay
		''' on top of all other windows or not.
		''' </summary>
		''' <seealso cref= Window#getAlwaysOnTop() </seealso>
		''' <seealso cref= Window#setAlwaysOnTop(boolean) </seealso>
		Sub updateAlwaysOnTopState()

		''' <summary>
		''' Updates the window's focusable state.
		''' </summary>
		''' <seealso cref= Window#setFocusableWindowState(boolean) </seealso>
		Sub updateFocusableWindowState()

		''' <summary>
		''' Sets if this window is blocked by a modal dialog or not.
		''' </summary>
		''' <param name="blocker"> the blocking modal dialog </param>
		''' <param name="blocked"> {@code true} to block the window, {@code false}
		'''        to unblock it </param>
		Sub setModalBlocked(ByVal blocker As Dialog, ByVal blocked As Boolean)

		''' <summary>
		''' Updates the minimum size on the peer.
		''' </summary>
		''' <seealso cref= Window#setMinimumSize(Dimension) </seealso>
		Sub updateMinimumSize()

		''' <summary>
		''' Updates the icons for the window.
		''' </summary>
		''' <seealso cref= Window#setIconImages(java.util.List) </seealso>
		Sub updateIconImages()

		''' <summary>
		''' Sets the level of opacity for the window.
		''' </summary>
		''' <seealso cref= Window#setOpacity(float) </seealso>
		WriteOnly Property opacity As Single

		''' <summary>
		''' Enables the per-pixel alpha support for the window.
		''' </summary>
		''' <seealso cref= Window#setBackground(Color) </seealso>
		WriteOnly Property opaque As Boolean

		''' <summary>
		''' Updates the native part of non-opaque window.
		''' </summary>
		''' <seealso cref= Window#setBackground(Color) </seealso>
		Sub updateWindow()

		''' <summary>
		''' Instructs the peer to update the position of the security warning.
		''' </summary>
		Sub repositionSecurityWarning()
	End Interface

End Namespace