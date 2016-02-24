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
	''' The peer interface for <seealso cref="Component"/>. This is the top level peer
	''' interface for widgets and defines the bulk of methods for AWT component
	''' peers. Most component peers have to implement this interface (via one
	''' of the subinterfaces), except menu components, which implement
	''' <seealso cref="MenuComponentPeer"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface ComponentPeer

		''' <summary>
		''' Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		''' a change in the component location only.
		''' </summary>
		''' <seealso cref= #setBounds(int, int, int, int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SET_LOCATION = 1;

		''' <summary>
		''' Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		''' a change in the component size only.
		''' </summary>
		''' <seealso cref= #setBounds(int, int, int, int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SET_SIZE = 2;

		''' <summary>
		''' Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		''' a change in the component size and location.
		''' </summary>
		''' <seealso cref= #setBounds(int, int, int, int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SET_BOUNDS = 3;

		''' <summary>
		''' Operation for <seealso cref="#setBounds(int, int, int, int, int)"/>, indicating
		''' a change in the component client size. This is used for setting
		''' the 'inside' size of windows, without the border insets.
		''' </summary>
		''' <seealso cref= #setBounds(int, int, int, int, int) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SET_CLIENT_SIZE = 4;

		''' <summary>
		''' Resets the setBounds() operation to DEFAULT_OPERATION. This is not
		''' passed into <seealso cref="#setBounds(int, int, int, int, int)"/>.
		''' 
		''' TODO: This is only used internally and should probably be moved outside
		'''       the peer interface.
		''' </summary>
		''' <seealso cref= Component#setBoundsOp </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int RESET_OPERATION = 5;

		''' <summary>
		''' A flag that is used to suppress checks for embedded frames.
		''' 
		''' TODO: This is only used internally and should probably be moved outside
		'''       the peer interface.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int NO_EMBEDDED_CHECK = (1 << 14);

		''' <summary>
		''' The default operation, which is to set size and location.
		''' 
		''' TODO: This is only used internally and should probably be moved outside
		'''       the peer interface.
		''' </summary>
		''' <seealso cref= Component#setBoundsOp </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DEFAULT_OPERATION = SET_BOUNDS;

		''' <summary>
		''' Determines if a component has been obscured, i.e. by an overlapping
		''' window or similar. This is used by JViewport for optimizing performance.
		''' This doesn't have to be implemented, when
		''' <seealso cref="#canDetermineObscurity()"/> returns {@code false}.
		''' </summary>
		''' <returns> {@code true} when the component has been obscured,
		'''         {@code false} otherwise
		''' </returns>
		''' <seealso cref= #canDetermineObscurity() </seealso>
		''' <seealso cref= javax.swing.JViewport#needsRepaintAfterBlit </seealso>
		ReadOnly Property obscured As Boolean

		''' <summary>
		''' Returns {@code true} when the peer can determine if a component
		''' has been obscured, {@code false} false otherwise.
		''' </summary>
		''' <returns> {@code true} when the peer can determine if a component
		'''         has been obscured, {@code false} false otherwise
		''' </returns>
		''' <seealso cref= #isObscured() </seealso>
		''' <seealso cref= javax.swing.JViewport#needsRepaintAfterBlit </seealso>
		Function canDetermineObscurity() As Boolean

		''' <summary>
		''' Makes a component visible or invisible.
		''' </summary>
		''' <param name="v"> {@code true} to make a component visible,
		'''          {@code false} to make it invisible
		''' </param>
		''' <seealso cref= Component#setVisible(boolean) </seealso>
		WriteOnly Property visible As Boolean

		''' <summary>
		''' Enables or disables a component. Disabled components are usually grayed
		''' out and cannot be activated.
		''' </summary>
		''' <param name="e"> {@code true} to enable the component, {@code false}
		'''          to disable it
		''' </param>
		''' <seealso cref= Component#setEnabled(boolean) </seealso>
		WriteOnly Property enabled As Boolean

		''' <summary>
		''' Paints the component to the specified graphics context. This is called
		''' by <seealso cref="Component#paintAll(Graphics)"/> to paint the component.
		''' </summary>
		''' <param name="g"> the graphics context to paint to
		''' </param>
		''' <seealso cref= Component#paintAll(Graphics) </seealso>
		Sub paint(ByVal g As Graphics)

		''' <summary>
		''' Prints the component to the specified graphics context. This is called
		''' by <seealso cref="Component#printAll(Graphics)"/> to print the component.
		''' </summary>
		''' <param name="g"> the graphics context to print to
		''' </param>
		''' <seealso cref= Component#printAll(Graphics) </seealso>
		Sub print(ByVal g As Graphics)

		''' <summary>
		''' Sets the location or size or both of the component. The location is
		''' specified relative to the component's parent. The {@code op}
		''' parameter specifies which properties change. If it is
		''' <seealso cref="#SET_LOCATION"/>, then only the location changes (and the size
		''' parameters can be ignored). If {@code op} is <seealso cref="#SET_SIZE"/>,
		''' then only the size changes (and the location can be ignored). If
		''' {@code op} is <seealso cref="#SET_BOUNDS"/>, then both change. There is a
		''' special value <seealso cref="#SET_CLIENT_SIZE"/>, which is used only for
		''' window-like components to set the size of the client (i.e. the 'inner'
		''' size, without the insets of the window borders).
		''' </summary>
		''' <param name="x"> the X location of the component </param>
		''' <param name="y"> the Y location of the component </param>
		''' <param name="width"> the width of the component </param>
		''' <param name="height"> the height of the component </param>
		''' <param name="op"> the operation flag
		''' </param>
		''' <seealso cref= #SET_BOUNDS </seealso>
		''' <seealso cref= #SET_LOCATION </seealso>
		''' <seealso cref= #SET_SIZE </seealso>
		''' <seealso cref= #SET_CLIENT_SIZE </seealso>
		Sub setBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal op As Integer)

		''' <summary>
		''' Called to let the component peer handle events.
		''' </summary>
		''' <param name="e"> the AWT event to handle
		''' </param>
		''' <seealso cref= Component#dispatchEvent(AWTEvent) </seealso>
		Sub handleEvent(ByVal e As AWTEvent)

		''' <summary>
		''' Called to coalesce paint events.
		''' </summary>
		''' <param name="e"> the paint event to consider to coalesce
		''' </param>
		''' <seealso cref= EventQueue#coalescePaintEvent </seealso>
		Sub coalescePaintEvent(ByVal e As java.awt.event.PaintEvent)

		''' <summary>
		''' Determines the location of the component on the screen.
		''' </summary>
		''' <returns> the location of the component on the screen
		''' </returns>
		''' <seealso cref= Component#getLocationOnScreen() </seealso>
		ReadOnly Property locationOnScreen As Point

		''' <summary>
		''' Determines the preferred size of the component.
		''' </summary>
		''' <returns> the preferred size of the component
		''' </returns>
		''' <seealso cref= Component#getPreferredSize() </seealso>
		ReadOnly Property preferredSize As Dimension

		''' <summary>
		''' Determines the minimum size of the component.
		''' </summary>
		''' <returns> the minimum size of the component
		''' </returns>
		''' <seealso cref= Component#getMinimumSize() </seealso>
		ReadOnly Property minimumSize As Dimension

		''' <summary>
		''' Returns the color model used by the component.
		''' </summary>
		''' <returns> the color model used by the component
		''' </returns>
		''' <seealso cref= Component#getColorModel() </seealso>
		ReadOnly Property colorModel As java.awt.image.ColorModel

		''' <summary>
		''' Returns a graphics object to paint on the component.
		''' </summary>
		''' <returns> a graphics object to paint on the component
		''' </returns>
		''' <seealso cref= Component#getGraphics() </seealso>
		' TODO: Maybe change this to force Graphics2D, since many things will
		' break with plain Graphics nowadays.
		ReadOnly Property graphics As Graphics

		''' <summary>
		''' Returns a font metrics object to determine the metrics properties of
		''' the specified font.
		''' </summary>
		''' <param name="font"> the font to determine the metrics for
		''' </param>
		''' <returns> a font metrics object to determine the metrics properties of
		'''         the specified font
		''' </returns>
		''' <seealso cref= Component#getFontMetrics(Font) </seealso>
		Function getFontMetrics(ByVal font As Font) As FontMetrics

		''' <summary>
		''' Disposes all resources held by the component peer. This is called
		''' when the component has been disconnected from the component hierarchy
		''' and is about to be garbage collected.
		''' </summary>
		''' <seealso cref= Component#removeNotify() </seealso>
		Sub dispose()

		''' <summary>
		''' Sets the foreground color of this component.
		''' </summary>
		''' <param name="c"> the foreground color to set
		''' </param>
		''' <seealso cref= Component#setForeground(Color) </seealso>
		WriteOnly Property foreground As Color

		''' <summary>
		''' Sets the background color of this component.
		''' </summary>
		''' <param name="c"> the background color to set
		''' </param>
		''' <seealso cref= Component#setBackground(Color) </seealso>
		WriteOnly Property background As Color

		''' <summary>
		''' Sets the font of this component.
		''' </summary>
		''' <param name="f"> the font of this component
		''' </param>
		''' <seealso cref= Component#setFont(Font) </seealso>
		WriteOnly Property font As Font

		''' <summary>
		''' Updates the cursor of the component.
		''' </summary>
		''' <seealso cref= Component#updateCursorImmediately </seealso>
		Sub updateCursorImmediately()

		''' <summary>
		''' Requests focus on this component.
		''' </summary>
		''' <param name="lightweightChild"> the actual lightweight child that requests the
		'''        focus </param>
		''' <param name="temporary"> {@code true} if the focus change is temporary,
		'''        {@code false} otherwise </param>
		''' <param name="focusedWindowChangeAllowed"> {@code true} if changing the
		'''        focus of the containing window is allowed or not </param>
		''' <param name="time"> the time of the focus change request </param>
		''' <param name="cause"> the cause of the focus change request
		''' </param>
		''' <returns> {@code true} if the focus change is guaranteed to be
		'''         granted, {@code false} otherwise </returns>
		Function requestFocus(ByVal lightweightChild As Component, ByVal temporary As Boolean, ByVal focusedWindowChangeAllowed As Boolean, ByVal time As Long, ByVal cause As sun.awt.CausedFocusEvent.Cause) As Boolean

		''' <summary>
		''' Returns {@code true} when the component takes part in the focus
		''' traversal, {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} when the component takes part in the focus
		'''         traversal, {@code false} otherwise </returns>
		ReadOnly Property focusable As Boolean

		''' <summary>
		''' Creates an image using the specified image producer.
		''' </summary>
		''' <param name="producer"> the image producer from which the image pixels will be
		'''        produced
		''' </param>
		''' <returns> the created image
		''' </returns>
		''' <seealso cref= Component#createImage(ImageProducer) </seealso>
		Function createImage(ByVal producer As java.awt.image.ImageProducer) As Image

		''' <summary>
		''' Creates an empty image with the specified width and height. This is
		''' generally used as a non-accelerated backbuffer for drawing onto the
		''' component (e.g. by Swing).
		''' </summary>
		''' <param name="width"> the width of the image </param>
		''' <param name="height"> the height of the image
		''' </param>
		''' <returns> the created image
		''' </returns>
		''' <seealso cref= Component#createImage(int, int) </seealso>
		' TODO: Maybe make that return a BufferedImage, because some stuff will
		' break if a different kind of image is returned.
		Function createImage(ByVal width As Integer, ByVal height As Integer) As Image

		''' <summary>
		''' Creates an empty volatile image with the specified width and height.
		''' This is generally used as an accelerated backbuffer for drawing onto
		''' the component (e.g. by Swing).
		''' </summary>
		''' <param name="width"> the width of the image </param>
		''' <param name="height"> the height of the image
		''' </param>
		''' <returns> the created volatile image
		''' </returns>
		''' <seealso cref= Component#createVolatileImage(int, int) </seealso>
		' TODO: Include capabilities here and fix Component#createVolatileImage
		Function createVolatileImage(ByVal width As Integer, ByVal height As Integer) As java.awt.image.VolatileImage

		''' <summary>
		''' Prepare the specified image for rendering on this component. This should
		''' start loading the image (if not already loaded) and create an
		''' appropriate screen representation.
		''' </summary>
		''' <param name="img"> the image to prepare </param>
		''' <param name="w"> the width of the screen representation </param>
		''' <param name="h"> the height of the screen representation </param>
		''' <param name="o"> an image observer to observe the progress
		''' </param>
		''' <returns> {@code true} if the image is already fully prepared,
		'''         {@code false} otherwise
		''' </returns>
		''' <seealso cref= Component#prepareImage(Image, int, int, ImageObserver) </seealso>
		Function prepareImage(ByVal img As Image, ByVal w As Integer, ByVal h As Integer, ByVal o As java.awt.image.ImageObserver) As Boolean

		''' <summary>
		''' Determines the status of the construction of the screen representaion
		''' of the specified image.
		''' </summary>
		''' <param name="img"> the image to check </param>
		''' <param name="w"> the target width </param>
		''' <param name="h"> the target height </param>
		''' <param name="o"> the image observer to notify
		''' </param>
		''' <returns> the status as bitwise ORed ImageObserver flags
		''' </returns>
		''' <seealso cref= Component#checkImage(Image, int, int, ImageObserver) </seealso>
		Function checkImage(ByVal img As Image, ByVal w As Integer, ByVal h As Integer, ByVal o As java.awt.image.ImageObserver) As Integer

		''' <summary>
		''' Returns the graphics configuration that corresponds to this component.
		''' </summary>
		''' <returns> the graphics configuration that corresponds to this component
		''' </returns>
		''' <seealso cref= Component#getGraphicsConfiguration() </seealso>
		ReadOnly Property graphicsConfiguration As GraphicsConfiguration

		''' <summary>
		''' Determines if the component handles wheel scrolling itself. Otherwise
		''' it is delegated to the component's parent.
		''' </summary>
		''' <returns> {@code true} if the component handles wheel scrolling,
		'''         {@code false} otherwise
		''' </returns>
		''' <seealso cref= Component#dispatchEventImpl(AWTEvent) </seealso>
		Function handlesWheelScrolling() As Boolean

		''' <summary>
		''' Create {@code numBuffers} flipping buffers with the specified
		''' buffer capabilities.
		''' </summary>
		''' <param name="numBuffers"> the number of buffers to create </param>
		''' <param name="caps"> the buffer capabilities
		''' </param>
		''' <exception cref="AWTException"> if flip buffering is not supported
		''' </exception>
		''' <seealso cref= Component.FlipBufferStrategy#createBuffers </seealso>
		Sub createBuffers(ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)

		''' <summary>
		''' Returns the back buffer as image.
		''' </summary>
		''' <returns> the back buffer as image
		''' </returns>
		''' <seealso cref= Component.FlipBufferStrategy#getBackBuffer </seealso>
		ReadOnly Property backBuffer As Image

		''' <summary>
		''' Move the back buffer to the front buffer.
		''' </summary>
		''' <param name="x1"> the area to be flipped, upper left X coordinate </param>
		''' <param name="y1"> the area to be flipped, upper left Y coordinate </param>
		''' <param name="x2"> the area to be flipped, lower right X coordinate </param>
		''' <param name="y2"> the area to be flipped, lower right Y coordinate </param>
		''' <param name="flipAction"> the flip action to perform
		''' </param>
		''' <seealso cref= Component.FlipBufferStrategy#flip </seealso>
		Sub flip(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal flipAction As BufferCapabilities.FlipContents)

		''' <summary>
		''' Destroys all created buffers.
		''' </summary>
		''' <seealso cref= Component.FlipBufferStrategy#destroyBuffers </seealso>
		Sub destroyBuffers()

		''' <summary>
		''' Reparents this peer to the new parent referenced by
		''' {@code newContainer} peer. Implementation depends on toolkit and
		''' container.
		''' </summary>
		''' <param name="newContainer"> peer of the new parent container
		''' 
		''' @since 1.5 </param>
		Sub reparent(ByVal newContainer As ContainerPeer)

		''' <summary>
		''' Returns whether this peer supports reparenting to another parent without
		''' destroying the peer.
		''' </summary>
		''' <returns> true if appropriate reparent is supported, false otherwise
		''' 
		''' @since 1.5 </returns>
		ReadOnly Property reparentSupported As Boolean

		''' <summary>
		''' Used by lightweight implementations to tell a ComponentPeer to layout
		''' its sub-elements.  For instance, a lightweight Checkbox needs to layout
		''' the box, as well as the text label.
		''' </summary>
		''' <seealso cref= Component#validate() </seealso>
		Sub layout()

		''' <summary>
		''' Applies the shape to the native component window.
		''' @since 1.7
		''' </summary>
		''' <seealso cref= Component#applyCompoundShape </seealso>
		Sub applyShape(ByVal shape As sun.java2d.pipe.Region)

		''' <summary>
		''' Lowers this component at the bottom of the above HW peer. If the above parameter
		''' is null then the method places this component at the top of the Z-order.
		''' </summary>
		WriteOnly Property zOrder As ComponentPeer

		''' <summary>
		''' Updates internal data structures related to the component's GC.
		''' </summary>
		''' <returns> if the peer needs to be recreated for the changes to take effect
		''' @since 1.7 </returns>
		Function updateGraphicsData(ByVal gc As GraphicsConfiguration) As Boolean
	End Interface

End Namespace