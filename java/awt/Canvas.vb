Imports javax.accessibility

'
' * Copyright (c) 1995, 2010, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt

	''' <summary>
	''' A <code>Canvas</code> component represents a blank rectangular
	''' area of the screen onto which the application can draw or from
	''' which the application can trap input events from the user.
	''' <p>
	''' An application must subclass the <code>Canvas</code> class in
	''' order to get useful functionality such as creating a custom
	''' component. The <code>paint</code> method must be overridden
	''' in order to perform custom graphics on the canvas.
	''' 
	''' @author      Sami Shaio
	''' @since       JDK1.0
	''' </summary>
	Public Class Canvas
		Inherits Component
		Implements Accessible

		Private Const base As String = "canvas"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -2284879212465893870L

		''' <summary>
		''' Constructs a new Canvas.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new Canvas given a GraphicsConfiguration object.
		''' </summary>
		''' <param name="config"> a reference to a GraphicsConfiguration object.
		''' </param>
		''' <seealso cref= GraphicsConfiguration </seealso>
		Public Sub New(ByVal config As GraphicsConfiguration)
			Me.New()
			graphicsConfiguration = config
		End Sub

		Friend Overrides Property graphicsConfiguration As GraphicsConfiguration
			Set(ByVal gc As GraphicsConfiguration)
				SyncLock treeLock
					Dim peer_Renamed As java.awt.peer.CanvasPeer = CType(peer, java.awt.peer.CanvasPeer)
					If peer_Renamed IsNot Nothing Then gc = peer_Renamed.getAppropriateGraphicsConfiguration(gc)
					MyBase.graphicsConfiguration = gc
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Canvas)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer of the canvas.  This peer allows you to change the
		''' user interface of the canvas without changing its functionality. </summary>
		''' <seealso cref=     java.awt.Toolkit#createCanvas(java.awt.Canvas) </seealso>
		''' <seealso cref=     java.awt.Component#getToolkit() </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createCanvas(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Paints this canvas.
		''' <p>
		''' Most applications that subclass <code>Canvas</code> should
		''' override this method in order to perform some useful operation
		''' (typically, custom painting of the canvas).
		''' The default operation is simply to clear the canvas.
		''' Applications that override this method need not call
		''' super.paint(g).
		''' </summary>
		''' <param name="g">   the specified Graphics context </param>
		''' <seealso cref=        #update(Graphics) </seealso>
		''' <seealso cref=        Component#paint(Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics)
			g.clearRect(0, 0, width, height)
		End Sub

		''' <summary>
		''' Updates this canvas.
		''' <p>
		''' This method is called in response to a call to <code>repaint</code>.
		''' The canvas is first cleared by filling it with the background
		''' color, and then completely redrawn by calling this canvas's
		''' <code>paint</code> method.
		''' Note: applications that override this method should either call
		''' super.update(g) or incorporate the functionality described
		''' above into their own code.
		''' </summary>
		''' <param name="g"> the specified Graphics context </param>
		''' <seealso cref=   #paint(Graphics) </seealso>
		''' <seealso cref=   Component#update(Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics)
			g.clearRect(0, 0, width, height)
			paint(g)
		End Sub

		Friend Overrides Function postsOldMouseEvents() As Boolean
			Return True
		End Function

		''' <summary>
		''' Creates a new strategy for multi-buffering on this component.
		''' Multi-buffering is useful for rendering performance.  This method
		''' attempts to create the best strategy available with the number of
		''' buffers supplied.  It will always create a <code>BufferStrategy</code>
		''' with that number of buffers.
		''' A page-flipping strategy is attempted first, then a blitting strategy
		''' using accelerated buffers.  Finally, an unaccelerated blitting
		''' strategy is used.
		''' <p>
		''' Each time this method is called,
		''' the existing buffer strategy for this component is discarded. </summary>
		''' <param name="numBuffers"> number of buffers to create, including the front buffer </param>
		''' <exception cref="IllegalArgumentException"> if numBuffers is less than 1. </exception>
		''' <exception cref="IllegalStateException"> if the component is not displayable </exception>
		''' <seealso cref= #isDisplayable </seealso>
		''' <seealso cref= #getBufferStrategy
		''' @since 1.4 </seealso>
		Public Overrides Sub createBufferStrategy(ByVal numBuffers As Integer)
			MyBase.createBufferStrategy(numBuffers)
		End Sub

		''' <summary>
		''' Creates a new strategy for multi-buffering on this component with the
		''' required buffer capabilities.  This is useful, for example, if only
		''' accelerated memory or page flipping is desired (as specified by the
		''' buffer capabilities).
		''' <p>
		''' Each time this method
		''' is called, the existing buffer strategy for this component is discarded. </summary>
		''' <param name="numBuffers"> number of buffers to create </param>
		''' <param name="caps"> the required capabilities for creating the buffer strategy;
		''' cannot be <code>null</code> </param>
		''' <exception cref="AWTException"> if the capabilities supplied could not be
		''' supported or met; this may happen, for example, if there is not enough
		''' accelerated memory currently available, or if page flipping is specified
		''' but not possible. </exception>
		''' <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
		''' caps is <code>null</code> </exception>
		''' <seealso cref= #getBufferStrategy
		''' @since 1.4 </seealso>
		Public Overrides Sub createBufferStrategy(ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
			MyBase.createBufferStrategy(numBuffers, caps)
		End Sub

		''' <summary>
		''' Returns the <code>BufferStrategy</code> used by this component.  This
		''' method will return null if a <code>BufferStrategy</code> has not yet
		''' been created or has been disposed.
		''' </summary>
		''' <returns> the buffer strategy used by this component </returns>
		''' <seealso cref= #createBufferStrategy
		''' @since 1.4 </seealso>
		Public Property Overrides bufferStrategy As java.awt.image.BufferStrategy
			Get
				Return MyBase.bufferStrategy
			End Get
		End Property

	'    
	'     * --- Accessibility Support ---
	'     *
	'     

		''' <summary>
		''' Gets the AccessibleContext associated with this Canvas.
		''' For canvases, the AccessibleContext takes the form of an
		''' AccessibleAWTCanvas.
		''' A new AccessibleAWTCanvas instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTCanvas that serves as the
		'''         AccessibleContext of this Canvas
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTCanvas(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Canvas</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to canvas user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTCanvas
			Inherits AccessibleAWTComponent

			Private ReadOnly outerInstance As Canvas

			Public Sub New(ByVal outerInstance As Canvas)
				Me.outerInstance = outerInstance
			End Sub

			Private Const serialVersionUID As Long = -6325592262103146699L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.CANVAS
				End Get
			End Property

		End Class ' inner class AccessibleAWTCanvas
	End Class

End Namespace