'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Peer interface for <seealso cref="MouseInfo"/>. This is used to get some additional
	''' information about the mouse.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface MouseInfoPeer

		''' <summary>
		''' This method does two things: it fills the point fields with
		''' the current coordinates of the mouse cursor and returns the
		''' number of the screen device where the pointer is located.
		''' The number of the screen device is only returned for independent
		''' devices (which are not parts of a virtual screen device).
		''' For virtual screen devices, 0 is returned.
		''' Mouse coordinates are also calculated depending on whether
		''' or not the screen device is virtual. For virtual screen
		''' devices, pointer coordinates are calculated in the virtual
		''' coordinate system. Otherwise, coordinates are calculated in
		''' the coordinate system of the screen device where the pointer
		''' is located.
		''' See java.awt.GraphicsConfiguration documentation for more
		''' details about virtual screen devices.
		''' </summary>
		Function fillPointWithCoords(  point As java.awt.Point) As Integer

		''' <summary>
		''' Returns whether or not the window is located under the mouse
		''' pointer. The window is considered to be under the mouse pointer
		''' if it is showing on the screen, and the mouse pointer is above
		''' the part of the window that is not obscured by any other windows.
		''' </summary>
		Function isWindowUnderMouse(  w As java.awt.Window) As Boolean

	End Interface

End Namespace