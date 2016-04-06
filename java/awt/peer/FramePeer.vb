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
	''' The peer interface for <seealso cref="Frame"/>. This adds a couple of frame specific
	''' methods to the <seealso cref="WindowPeer"/> interface.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface FramePeer
		Inherits WindowPeer

		''' <summary>
		''' Sets the title on the frame.
		''' </summary>
		''' <param name="title"> the title to set
		''' </param>
		''' <seealso cref= Frame#setTitle(String) </seealso>
		WriteOnly Property title As String

		''' <summary>
		''' Sets the menu bar for the frame.
		''' </summary>
		''' <param name="mb"> the menu bar to set
		''' </param>
		''' <seealso cref= Frame#setMenuBar(MenuBar) </seealso>
		WriteOnly Property menuBar As MenuBar

		''' <summary>
		''' Sets if the frame should be resizable or not.
		''' </summary>
		''' <param name="resizeable"> {@code true} when the frame should be resizable,
		'''        {@code false} if not
		''' </param>
		''' <seealso cref= Frame#setResizable(boolean) </seealso>
		WriteOnly Property resizable As Boolean

		''' <summary>
		''' Changes the state of the frame.
		''' </summary>
		''' <param name="state"> the new state
		''' </param>
		''' <seealso cref= Frame#setExtendedState(int) </seealso>
		Property state As Integer


		''' <summary>
		''' Sets the bounds of the frame when it becomes maximized.
		''' </summary>
		''' <param name="bounds"> the maximized bounds of the frame
		''' </param>
		''' <seealso cref= Frame#setMaximizedBounds(Rectangle) </seealso>
		WriteOnly Property maximizedBounds As Rectangle

		''' <summary>
		''' Sets the size and location for embedded frames. (On embedded frames,
		''' setLocation() and setBounds() always set the frame to (0,0) for
		''' backwards compatibility.
		''' </summary>
		''' <param name="x"> the X location </param>
		''' <param name="y"> the Y location </param>
		''' <param name="width"> the width of the frame </param>
		''' <param name="height"> the height of the frame
		''' </param>
		''' <seealso cref= EmbeddedFrame#setBoundsPrivate(int, int, int, int) </seealso>
		' TODO: This is only used in EmbeddedFrame, and should probably be moved
		' into an EmbeddedFramePeer which would extend FramePeer
		Sub setBoundsPrivate(  x As Integer,   y As Integer,   width As Integer,   height As Integer)

		''' <summary>
		''' Returns the size and location for embedded frames. (On embedded frames,
		''' setLocation() and setBounds() always set the frame to (0,0) for
		''' backwards compatibility.
		''' </summary>
		''' <returns> the bounds of an embedded frame
		''' </returns>
		''' <seealso cref= EmbeddedFrame#getBoundsPrivate() </seealso>
		' TODO: This is only used in EmbeddedFrame, and should probably be moved
		' into an EmbeddedFramePeer which would extend FramePeer
		ReadOnly Property boundsPrivate As Rectangle

		''' <summary>
		''' Requests the peer to emulate window activation.
		''' </summary>
		''' <param name="activate"> activate or deactivate the window </param>
		Sub emulateActivation(  activate As Boolean)
	End Interface

End Namespace