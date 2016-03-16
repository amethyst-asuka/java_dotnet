Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports javax.accessibility

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
Namespace java.awt

	''' <summary>
	''' A <code>Frame</code> is a top-level window with a title and a border.
	''' <p>
	''' The size of the frame includes any area designated for the
	''' border.  The dimensions of the border area may be obtained
	''' using the <code>getInsets</code> method, however, since
	''' these dimensions are platform-dependent, a valid insets
	''' value cannot be obtained until the frame is made displayable
	''' by either calling <code>pack</code> or <code>show</code>.
	''' Since the border area is included in the overall size of the
	''' frame, the border effectively obscures a portion of the frame,
	''' constraining the area available for rendering and/or displaying
	''' subcomponents to the rectangle which has an upper-left corner
	''' location of <code>(insets.left, insets.top)</code>, and has a size of
	''' <code>width - (insets.left + insets.right)</code> by
	''' <code>height - (insets.top + insets.bottom)</code>.
	''' <p>
	''' The default layout for a frame is <code>BorderLayout</code>.
	''' <p>
	''' A frame may have its native decorations (i.e. <code>Frame</code>
	''' and <code>Titlebar</code>) turned off
	''' with <code>setUndecorated</code>. This can only be done while the frame
	''' is not <seealso cref="Component#isDisplayable() displayable"/>.
	''' <p>
	''' In a multi-screen environment, you can create a <code>Frame</code>
	''' on a different screen device by constructing the <code>Frame</code>
	''' with <seealso cref="#Frame(GraphicsConfiguration)"/> or
	''' <seealso cref="#Frame(String title, GraphicsConfiguration)"/>.  The
	''' <code>GraphicsConfiguration</code> object is one of the
	''' <code>GraphicsConfiguration</code> objects of the target screen
	''' device.
	''' <p>
	''' In a virtual device multi-screen environment in which the desktop
	''' area could span multiple physical screen devices, the bounds of all
	''' configurations are relative to the virtual-coordinate system.  The
	''' origin of the virtual-coordinate system is at the upper left-hand
	''' corner of the primary physical screen.  Depending on the location
	''' of the primary screen in the virtual device, negative coordinates
	''' are possible, as shown in the following figure.
	''' <p>
	''' <img src="doc-files/MultiScreen.gif"
	''' alt="Diagram of virtual device encompassing three physical screens and one primary physical screen. The primary physical screen
	''' shows (0,0) coords while a different physical screen shows (-80,-100) coords."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' In such an environment, when calling <code>setLocation</code>,
	''' you must pass a virtual coordinate to this method.  Similarly,
	''' calling <code>getLocationOnScreen</code> on a <code>Frame</code>
	''' returns virtual device coordinates.  Call the <code>getBounds</code>
	''' method of a <code>GraphicsConfiguration</code> to find its origin in
	''' the virtual coordinate system.
	''' <p>
	''' The following code sets the
	''' location of the <code>Frame</code> at (10, 10) relative
	''' to the origin of the physical screen of the corresponding
	''' <code>GraphicsConfiguration</code>.  If the bounds of the
	''' <code>GraphicsConfiguration</code> is not taken into account, the
	''' <code>Frame</code> location would be set at (10, 10) relative to the
	''' virtual-coordinate system and would appear on the primary physical
	''' screen, which might be different from the physical screen of the
	''' specified <code>GraphicsConfiguration</code>.
	''' 
	''' <pre>
	'''      Frame f = new Frame(GraphicsConfiguration gc);
	'''      Rectangle bounds = gc.getBounds();
	'''      f.setLocation(10 + bounds.x, 10 + bounds.y);
	''' </pre>
	''' 
	''' <p>
	''' Frames are capable of generating the following types of
	''' <code>WindowEvent</code>s:
	''' <ul>
	''' <li><code>WINDOW_OPENED</code>
	''' <li><code>WINDOW_CLOSING</code>:
	'''     <br>If the program doesn't
	'''     explicitly hide or dispose the window while processing
	'''     this event, the window close operation is canceled.
	''' <li><code>WINDOW_CLOSED</code>
	''' <li><code>WINDOW_ICONIFIED</code>
	''' <li><code>WINDOW_DEICONIFIED</code>
	''' <li><code>WINDOW_ACTIVATED</code>
	''' <li><code>WINDOW_DEACTIVATED</code>
	''' <li><code>WINDOW_GAINED_FOCUS</code>
	''' <li><code>WINDOW_LOST_FOCUS</code>
	''' <li><code>WINDOW_STATE_CHANGED</code>
	''' </ul>
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref= WindowEvent </seealso>
	''' <seealso cref= Window#addWindowListener
	''' @since       JDK1.0 </seealso>
	Public Class Frame
		Inherits Window
		Implements MenuContainer

	'     Note: These are being obsoleted;  programs should use the Cursor class
	'     * variables going forward. See Cursor and Component.setCursor.
	'     

	   ''' @deprecated   replaced by <code>Cursor.DEFAULT_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.DEFAULT_CURSOR</code>.")> _
		Public Const DEFAULT_CURSOR As Integer = Cursor.DEFAULT_CURSOR


	   ''' @deprecated   replaced by <code>Cursor.CROSSHAIR_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.CROSSHAIR_CURSOR</code>.")> _
		Public Const CROSSHAIR_CURSOR As Integer = Cursor.CROSSHAIR_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.TEXT_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.TEXT_CURSOR</code>.")> _
		Public Const TEXT_CURSOR As Integer = Cursor.TEXT_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.WAIT_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.WAIT_CURSOR</code>.")> _
		Public Const WAIT_CURSOR As Integer = Cursor.WAIT_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.SW_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.SW_RESIZE_CURSOR</code>.")> _
		Public Const SW_RESIZE_CURSOR As Integer = Cursor.SW_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.SE_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.SE_RESIZE_CURSOR</code>.")> _
		Public Const SE_RESIZE_CURSOR As Integer = Cursor.SE_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.NW_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.NW_RESIZE_CURSOR</code>.")> _
		Public Const NW_RESIZE_CURSOR As Integer = Cursor.NW_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.NE_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.NE_RESIZE_CURSOR</code>.")> _
		Public Const NE_RESIZE_CURSOR As Integer = Cursor.NE_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.N_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.N_RESIZE_CURSOR</code>.")> _
		Public Const N_RESIZE_CURSOR As Integer = Cursor.N_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.S_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.S_RESIZE_CURSOR</code>.")> _
		Public Const S_RESIZE_CURSOR As Integer = Cursor.S_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.W_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.W_RESIZE_CURSOR</code>.")> _
		Public Const W_RESIZE_CURSOR As Integer = Cursor.W_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.E_RESIZE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.E_RESIZE_CURSOR</code>.")> _
		Public Const E_RESIZE_CURSOR As Integer = Cursor.E_RESIZE_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.HAND_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.HAND_CURSOR</code>.")> _
		Public Const HAND_CURSOR As Integer = Cursor.HAND_CURSOR

	   ''' @deprecated   replaced by <code>Cursor.MOVE_CURSOR</code>. 
		<Obsolete("  replaced by <code>Cursor.MOVE_CURSOR</code>.")> _
		Public Const MOVE_CURSOR As Integer = Cursor.MOVE_CURSOR


		''' <summary>
		''' Frame is in the "normal" state.  This symbolic constant names a
		''' frame state with all state bits cleared. </summary>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= #getExtendedState </seealso>
		Public Const NORMAL As Integer = 0

		''' <summary>
		''' This state bit indicates that frame is iconified. </summary>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= #getExtendedState </seealso>
		Public Const ICONIFIED As Integer = 1

		''' <summary>
		''' This state bit indicates that frame is maximized in the
		''' horizontal direction. </summary>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= #getExtendedState
		''' @since 1.4 </seealso>
		Public Const MAXIMIZED_HORIZ As Integer = 2

		''' <summary>
		''' This state bit indicates that frame is maximized in the
		''' vertical direction. </summary>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= #getExtendedState
		''' @since 1.4 </seealso>
		Public Const MAXIMIZED_VERT As Integer = 4

		''' <summary>
		''' This state bit mask indicates that frame is fully maximized
		''' (that is both horizontally and vertically).  It is just a
		''' convenience alias for
		''' <code>MAXIMIZED_VERT&nbsp;|&nbsp;MAXIMIZED_HORIZ</code>.
		''' 
		''' <p>Note that the correct test for frame being fully maximized is
		''' <pre>
		'''     (state &amp; Frame.MAXIMIZED_BOTH) == Frame.MAXIMIZED_BOTH
		''' </pre>
		''' 
		''' <p>To test is frame is maximized in <em>some</em> direction use
		''' <pre>
		'''     (state &amp; Frame.MAXIMIZED_BOTH) != 0
		''' </pre>
		''' </summary>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= #getExtendedState
		''' @since 1.4 </seealso>
		Public Shared ReadOnly MAXIMIZED_BOTH As Integer = MAXIMIZED_VERT Or MAXIMIZED_HORIZ

		''' <summary>
		''' Maximized bounds for this frame. </summary>
		''' <seealso cref=     #setMaximizedBounds(Rectangle) </seealso>
		''' <seealso cref=     #getMaximizedBounds
		''' @serial
		''' @since 1.4 </seealso>
		Friend maximizedBounds As Rectangle


		''' <summary>
		''' This is the title of the frame.  It can be changed
		''' at any time.  <code>title</code> can be null and if
		''' this is the case the <code>title</code> = "".
		''' 
		''' @serial </summary>
		''' <seealso cref= #getTitle </seealso>
		''' <seealso cref= #setTitle(String) </seealso>
		Friend title As String = "Untitled"

		''' <summary>
		''' The frames menubar.  If <code>menuBar</code> = null
		''' the frame will not have a menubar.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getMenuBar </seealso>
		''' <seealso cref= #setMenuBar(MenuBar) </seealso>
		Friend menuBar As MenuBar

		''' <summary>
		''' This field indicates whether the frame is resizable.
		''' This property can be changed at any time.
		''' <code>resizable</code> will be true if the frame is
		''' resizable, otherwise it will be false.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isResizable() </seealso>
		Friend resizable As Boolean = True

		''' <summary>
		''' This field indicates whether the frame is undecorated.
		''' This property can only be changed while the frame is not displayable.
		''' <code>undecorated</code> will be true if the frame is
		''' undecorated, otherwise it will be false.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setUndecorated(boolean) </seealso>
		''' <seealso cref= #isUndecorated() </seealso>
		''' <seealso cref= Component#isDisplayable()
		''' @since 1.4 </seealso>
		Friend undecorated As Boolean = False

		''' <summary>
		''' <code>mbManagement</code> is only used by the Motif implementation.
		''' 
		''' @serial
		''' </summary>
		Friend mbManagement As Boolean = False ' used only by the Motif impl.

		' XXX: uwe: abuse old field for now
		' will need to take care of serialization
		Private Shadows state As Integer = NORMAL

	'    
	'     * The Windows owned by the Frame.
	'     * Note: in 1.2 this has been superceded by Window.ownedWindowList
	'     *
	'     * @serial
	'     * @see java.awt.Window#ownedWindowList
	'     
		Friend ownedWindows As List(Of Window)

		Private Const base As String = "frame"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 2673458971256075116L

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setFrameAccessor(New sun.awt.AWTAccessor.FrameAccessor()
	'		{
	'				public  Sub  setExtendedState(Frame frame, int state)
	'				{
	'					synchronized(frame.getObjectLock())
	'					{
	'						frame.state = state;
	'					}
	'				}
	'				public int getExtendedState(Frame frame)
	'				{
	'					synchronized(frame.getObjectLock())
	'					{
	'						Return frame.state;
	'					}
	'				}
	'				public Rectangle getMaximizedBounds(Frame frame)
	'				{
	'					synchronized(frame.getObjectLock())
	'					{
	'						Return frame.maximizedBounds;
	'					}
	'				}
	'			}
		   )
		End Sub

		''' <summary>
		''' Constructs a new instance of <code>Frame</code> that is
		''' initially invisible.  The title of the <code>Frame</code>
		''' is empty. </summary>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible(boolean) </seealso>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible {@code Frame} with the
		''' specified {@code GraphicsConfiguration}.
		''' </summary>
		''' <param name="gc"> the <code>GraphicsConfiguration</code>
		''' of the target screen device. If <code>gc</code>
		''' is <code>null</code>, the system default
		''' <code>GraphicsConfiguration</code> is assumed. </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>gc</code> is not from a screen device. </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless()
		''' @since     1.3 </seealso>
		Public Sub New(ByVal gc As GraphicsConfiguration)
			Me.New("", gc)
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible <code>Frame</code> object
		''' with the specified title. </summary>
		''' <param name="title"> the title to be displayed in the frame's border.
		'''              A <code>null</code> value
		'''              is treated as an empty string, "". </param>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		''' <seealso cref= java.awt.Component#setSize </seealso>
		''' <seealso cref= java.awt.Component#setVisible(boolean) </seealso>
		''' <seealso cref= java.awt.GraphicsConfiguration#getBounds </seealso>
		Public Sub New(ByVal title As String)
			init(title, Nothing)
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible <code>Frame</code> object
		''' with the specified title and a
		''' <code>GraphicsConfiguration</code>. </summary>
		''' <param name="title"> the title to be displayed in the frame's border.
		'''              A <code>null</code> value
		'''              is treated as an empty string, "". </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code>
		''' of the target screen device.  If <code>gc</code> is
		''' <code>null</code>, the system default
		''' <code>GraphicsConfiguration</code> is assumed. </param>
		''' <exception cref="IllegalArgumentException"> if <code>gc</code>
		''' is not from a screen device. </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		''' <seealso cref= java.awt.Component#setSize </seealso>
		''' <seealso cref= java.awt.Component#setVisible(boolean) </seealso>
		''' <seealso cref= java.awt.GraphicsConfiguration#getBounds
		''' @since 1.3 </seealso>
		Public Sub New(ByVal title As String, ByVal gc As GraphicsConfiguration)
			MyBase.New(gc)
			init(title, gc)
		End Sub

		Private Sub init(ByVal title As String, ByVal gc As GraphicsConfiguration)
			Me.title = title
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Frame)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Makes this Frame displayable by connecting it to
		''' a native screen resource.  Making a frame displayable will
		''' cause any of its children to be made displayable.
		''' This method is called internally by the toolkit and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #removeNotify </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createFrame(Me)
				Dim p As java.awt.peer.FramePeer = CType(peer, java.awt.peer.FramePeer)
				Dim menuBar_Renamed As MenuBar = Me.menuBar
				If menuBar_Renamed IsNot Nothing Then
					mbManagement = True
					menuBar_Renamed.addNotify()
					p.menuBar = menuBar_Renamed
				End If
				p.maximizedBounds = maximizedBounds
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the title of the frame.  The title is displayed in the
		''' frame's border. </summary>
		''' <returns>    the title of this frame, or an empty string ("")
		'''                if this frame doesn't have a title. </returns>
		''' <seealso cref=       #setTitle(String) </seealso>
		Public Overridable Property title As String
			Get
				Return title
			End Get
			Set(ByVal title As String)
				Dim oldTitle As String = Me.title
				If title Is Nothing Then title = ""
    
    
				SyncLock Me
					Me.title = title
					Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.title = title
				End SyncLock
				firePropertyChange("title", oldTitle, title)
			End Set
		End Property


		''' <summary>
		''' Returns the image to be displayed as the icon for this frame.
		''' <p>
		''' This method is obsolete and kept for backward compatibility
		''' only. Use <seealso cref="Window#getIconImages Window.getIconImages()"/> instead.
		''' <p>
		''' If a list of several images was specified as a Window's icon,
		''' this method will return the first item of the list.
		''' </summary>
		''' <returns>    the icon image for this frame, or <code>null</code>
		'''                    if this frame doesn't have an icon image. </returns>
		''' <seealso cref=       #setIconImage(Image) </seealso>
		''' <seealso cref=       Window#getIconImages() </seealso>
		''' <seealso cref=       Window#setIconImages </seealso>
		Public Overridable Property iconImage As Image
			Get
				Dim icons As IList(Of Image) = Me.icons
				If icons IsNot Nothing Then
					If icons.Count > 0 Then Return icons(0)
				End If
				Return Nothing
			End Get
			Set(ByVal image_Renamed As Image)
				MyBase.iconImage = image_Renamed
			End Set
		End Property


		''' <summary>
		''' Gets the menu bar for this frame. </summary>
		''' <returns>    the menu bar for this frame, or <code>null</code>
		'''                   if this frame doesn't have a menu bar. </returns>
		''' <seealso cref=       #setMenuBar(MenuBar) </seealso>
		Public Overridable Property menuBar As MenuBar
			Get
				Return menuBar
			End Get
			Set(ByVal mb As MenuBar)
				SyncLock treeLock
					If menuBar Is mb Then Return
					If (mb IsNot Nothing) AndAlso (mb.parent IsNot Nothing) Then mb.parent.remove(mb)
					If menuBar IsNot Nothing Then remove(menuBar)
					menuBar = mb
					If menuBar IsNot Nothing Then
						menuBar.parent = Me
    
						Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
						If peer_Renamed IsNot Nothing Then
							mbManagement = True
							menuBar.addNotify()
							invalidateIfValid()
							peer_Renamed.menuBar = menuBar
						End If
					End If
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Indicates whether this frame is resizable by the user.
		''' By default, all frames are initially resizable. </summary>
		''' <returns>    <code>true</code> if the user can resize this frame;
		'''                        <code>false</code> otherwise. </returns>
		''' <seealso cref=       java.awt.Frame#setResizable(boolean) </seealso>
		Public Overridable Property resizable As Boolean
			Get
				Return resizable
			End Get
			Set(ByVal resizable As Boolean)
				Dim oldResizable As Boolean = Me.resizable
				Dim testvalid As Boolean = False
    
				SyncLock Me
					Me.resizable = resizable
					Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
					If peer_Renamed IsNot Nothing Then
						peer_Renamed.resizable = resizable
						testvalid = True
					End If
				End SyncLock
    
				' On some platforms, changing the resizable state affects
				' the insets of the Frame. If we could, we'd call invalidate()
				' from the peer, but we need to guarantee that we're not holding
				' the Frame lock when we call invalidate().
				If testvalid Then invalidateIfValid()
				firePropertyChange("resizable", oldResizable, resizable)
			End Set
		End Property



		''' <summary>
		''' Sets the state of this frame (obsolete).
		''' <p>
		''' In older versions of JDK a frame state could only be NORMAL or
		''' ICONIFIED.  Since JDK 1.4 set of supported frame states is
		''' expanded and frame state is represented as a bitwise mask.
		''' <p>
		''' For compatibility with applications developed
		''' earlier this method still accepts
		''' {@code Frame.NORMAL} and
		''' {@code Frame.ICONIFIED} only.  The iconic
		''' state of the frame is only changed, other aspects
		''' of frame state are not affected by this method. If
		''' the state passed to this method is neither {@code
		''' Frame.NORMAL} nor {@code Frame.ICONIFIED} the
		''' method performs no actions at all.
		''' <p>Note that if the state is not supported on a
		''' given platform, neither the state nor the return
		''' value of the <seealso cref="#getState"/> method will be
		''' changed. The application may determine whether a
		''' specific state is supported via the {@link
		''' java.awt.Toolkit#isFrameStateSupported} method.
		''' <p><b>If the frame is currently visible on the
		''' screen</b> (the <seealso cref="#isShowing"/> method returns
		''' {@code true}), the developer should examine the
		''' return value of the  {@link
		''' java.awt.event.WindowEvent#getNewState} method of
		''' the {@code WindowEvent} received through the
		''' <seealso cref="java.awt.event.WindowStateListener"/> to
		''' determine that the state has actually been
		''' changed.
		''' <p><b>If the frame is not visible on the
		''' screen</b>, the events may or may not be
		''' generated.  In this case the developer may assume
		''' that the state changes immediately after this
		''' method returns.  Later, when the {@code
		''' setVisible(true)} method is invoked, the frame
		''' will attempt to apply this state. Receiving any
		''' {@link
		''' java.awt.event.WindowEvent#WINDOW_STATE_CHANGED}
		''' events is not guaranteed in this case also.
		''' </summary>
		''' <param name="state"> either <code>Frame.NORMAL</code> or
		'''     <code>Frame.ICONIFIED</code>. </param>
		''' <seealso cref= #setExtendedState(int) </seealso>
		''' <seealso cref= java.awt.Window#addWindowStateListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setState(ByVal state As Integer) 'JavaToDotNetTempPropertySetstate
		Public Overridable Property state As Integer
			Set(ByVal state As Integer)
				Dim current As Integer = extendedState
				If state = ICONIFIED AndAlso (current And ICONIFIED) = 0 Then
					extendedState = current Or ICONIFIED
				ElseIf state = NORMAL AndAlso (current And ICONIFIED) <> 0 Then
					extendedState = current And (Not ICONIFIED)
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the state of this frame. The state is
		''' represented as a bitwise mask.
		''' <ul>
		''' <li><code>NORMAL</code>
		''' <br>Indicates that no state bits are set.
		''' <li><code>ICONIFIED</code>
		''' <li><code>MAXIMIZED_HORIZ</code>
		''' <li><code>MAXIMIZED_VERT</code>
		''' <li><code>MAXIMIZED_BOTH</code>
		''' <br>Concatenates <code>MAXIMIZED_HORIZ</code>
		''' and <code>MAXIMIZED_VERT</code>.
		''' </ul>
		''' <p>Note that if the state is not supported on a
		''' given platform, neither the state nor the return
		''' value of the <seealso cref="#getExtendedState"/> method will
		''' be changed. The application may determine whether
		''' a specific state is supported via the {@link
		''' java.awt.Toolkit#isFrameStateSupported} method.
		''' <p><b>If the frame is currently visible on the
		''' screen</b> (the <seealso cref="#isShowing"/> method returns
		''' {@code true}), the developer should examine the
		''' return value of the {@link
		''' java.awt.event.WindowEvent#getNewState} method of
		''' the {@code WindowEvent} received through the
		''' <seealso cref="java.awt.event.WindowStateListener"/> to
		''' determine that the state has actually been
		''' changed.
		''' <p><b>If the frame is not visible on the
		''' screen</b>, the events may or may not be
		''' generated.  In this case the developer may assume
		''' that the state changes immediately after this
		''' method returns.  Later, when the {@code
		''' setVisible(true)} method is invoked, the frame
		''' will attempt to apply this state. Receiving any
		''' {@link
		''' java.awt.event.WindowEvent#WINDOW_STATE_CHANGED}
		''' events is not guaranteed in this case also.
		''' </summary>
		''' <param name="state"> a bitwise mask of frame state constants
		''' @since   1.4 </param>
		''' <seealso cref= java.awt.Window#addWindowStateListener </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setExtendedState(ByVal state As Integer) 'JavaToDotNetTempPropertySetextendedState
		Public Overridable Property extendedState As Integer
			Set(ByVal state As Integer)
				If Not isFrameStateSupported(state) Then Return
				SyncLock objectLock
					Me.state = state
				End SyncLock
				' peer.setState must be called outside of object lock
				' synchronization block to avoid possible deadlock
				Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.state = state
			End Set
			Get
		End Property
		Private Function isFrameStateSupported(ByVal state As Integer) As Boolean
			If Not toolkit.isFrameStateSupported(state) Then
				' * Toolkit.isFrameStateSupported returns always false
				' on compound state even if all parts are supported;
				' * if part of state is not supported, state is not supported;
				' * MAXIMIZED_BOTH is not a compound state.
				If ((state And ICONIFIED) <> 0) AndAlso (Not toolkit.isFrameStateSupported(ICONIFIED)) Then
					Return False
				Else
					state = state And Not ICONIFIED
				End If
				Return toolkit.isFrameStateSupported(state)
			End If
			Return True
		End Function

			Return If((extendedState And ICONIFIED) <> 0, ICONIFIED, NORMAL)
		End Function


			SyncLock objectLock
				Return state
			End SyncLock
		End Function


		''' <summary>
		''' Sets the maximized bounds for this frame.
		''' <p>
		''' When a frame is in maximized state the system supplies some
		''' defaults bounds.  This method allows some or all of those
		''' system supplied values to be overridden.
		''' <p>
		''' If <code>bounds</code> is <code>null</code>, accept bounds
		''' supplied by the system.  If non-<code>null</code> you can
		''' override some of the system supplied values while accepting
		''' others by setting those fields you want to accept from system
		''' to <code> java.lang.[Integer].MAX_VALUE</code>.
		''' <p>
		''' Note, the given maximized bounds are used as a hint for the native
		''' system, because the underlying platform may not support setting the
		''' location and/or size of the maximized windows.  If that is the case, the
		''' provided values do not affect the appearance of the frame in the
		''' maximized state.
		''' </summary>
		''' <param name="bounds">  bounds for the maximized state </param>
		''' <seealso cref= #getMaximizedBounds()
		''' @since 1.4 </seealso>
		Public Overridable Property maximizedBounds As Rectangle
			Set(ByVal bounds As Rectangle)
				SyncLock objectLock
					Me.maximizedBounds = bounds
				End SyncLock
				Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.maximizedBounds = bounds
			End Set
			Get
				SyncLock objectLock
					Return maximizedBounds
				End SyncLock
			End Get
		End Property



		''' <summary>
		''' Disables or enables decorations for this frame.
		''' <p>
		''' This method can only be called while the frame is not displayable. To
		''' make this frame decorated, it must be opaque and have the default shape,
		''' otherwise the {@code IllegalComponentStateException} will be thrown.
		''' Refer to <seealso cref="Window#setShape"/>, <seealso cref="Window#setOpacity"/> and {@link
		''' Window#setBackground} for details
		''' </summary>
		''' <param name="undecorated"> {@code true} if no frame decorations are to be
		'''         enabled; {@code false} if frame decorations are to be enabled
		''' </param>
		''' <exception cref="IllegalComponentStateException"> if the frame is displayable </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and this frame does not have the default shape </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and this frame opacity is less than {@code 1.0f} </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and the alpha value of this frame background
		'''      color is less than {@code 1.0f}
		''' </exception>
		''' <seealso cref=    #isUndecorated </seealso>
		''' <seealso cref=    Component#isDisplayable </seealso>
		''' <seealso cref=    Window#getShape </seealso>
		''' <seealso cref=    Window#getOpacity </seealso>
		''' <seealso cref=    Window#getBackground </seealso>
		''' <seealso cref=    javax.swing.JFrame#setDefaultLookAndFeelDecorated(boolean)
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property undecorated As Boolean
			Set(ByVal undecorated As Boolean)
				' Make sure we don't run in the middle of peer creation.
				SyncLock treeLock
					If displayable Then Throw New IllegalComponentStateException("The frame is displayable.")
					If Not undecorated Then
						If opacity < 1.0f Then Throw New IllegalComponentStateException("The frame is not opaque")
						If shape IsNot Nothing Then Throw New IllegalComponentStateException("The frame does not have a default shape")
						Dim bg As Color = background
						If (bg IsNot Nothing) AndAlso (bg.alpha < 255) Then Throw New IllegalComponentStateException("The frame background color is not opaque")
					End If
					Me.undecorated = undecorated
				End SyncLock
			End Set
			Get
				Return undecorated
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property opacity As Single
			Set(ByVal opacity As Single)
				SyncLock treeLock
					If (opacity < 1.0f) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The frame is decorated")
					MyBase.opacity = opacity
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property shape As Shape
			Set(ByVal shape As Shape)
				SyncLock treeLock
					If (shape IsNot Nothing) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The frame is decorated")
					MyBase.shape = shape
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property background As Color
			Set(ByVal bgColor As Color)
				SyncLock treeLock
					If (bgColor IsNot Nothing) AndAlso (bgColor.alpha < 255) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The frame is decorated")
					MyBase.background = bgColor
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Removes the specified menu bar from this frame. </summary>
		''' <param name="m">   the menu component to remove.
		'''           If <code>m</code> is <code>null</code>, then
		'''           no action is taken </param>
		Public Overridable Sub remove(ByVal m As MenuComponent) Implements MenuContainer.remove
			If m Is Nothing Then Return
			SyncLock treeLock
				If m Is menuBar Then
					menuBar = Nothing
					Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
					If peer_Renamed IsNot Nothing Then
						mbManagement = True
						invalidateIfValid()
						peer_Renamed.menuBar = Nothing
						m.removeNotify()
					End If
					m.parent = Nothing
				Else
					MyBase.remove(m)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Makes this Frame undisplayable by removing its connection
		''' to its native screen resource. Making a Frame undisplayable
		''' will cause any of its children to be made undisplayable.
		''' This method is called by the toolkit internally and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #addNotify </seealso>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.FramePeer = CType(Me.peer, java.awt.peer.FramePeer)
				If peer_Renamed IsNot Nothing Then
					' get the latest Frame state before disposing
					state

					If menuBar IsNot Nothing Then
						mbManagement = True
						peer_Renamed.menuBar = Nothing
						menuBar.removeNotify()
					End If
				End If
				MyBase.removeNotify()
			End SyncLock
		End Sub

		Friend Overrides Sub postProcessKeyEvent(ByVal e As KeyEvent)
			If menuBar IsNot Nothing AndAlso menuBar.handleShortcut(e) Then
				e.consume()
				Return
			End If
			MyBase.postProcessKeyEvent(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Frame</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns> the parameter string of this frame </returns>
		Protected Friend Overrides Function paramString() As String
			Dim str As String = MyBase.paramString()
			If title IsNot Nothing Then str &= ",title=" & title
			If resizable Then str &= ",resizable"
			Dim state_Renamed As Integer = extendedState
			If state_Renamed = NORMAL Then
				str &= ",normal"
			Else
				If (state_Renamed And ICONIFIED) <> 0 Then str &= ",iconified"
				If (state_Renamed And MAXIMIZED_BOTH) = MAXIMIZED_BOTH Then
					str &= ",maximized"
				ElseIf (state_Renamed And MAXIMIZED_HORIZ) <> 0 Then
					str &= ",maximized_horiz"
				ElseIf (state_Renamed And MAXIMIZED_VERT) <> 0 Then
					str &= ",maximized_vert"
				End If
			End If
			Return str
		End Function

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Component.setCursor(Cursor)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property cursor As Integer
			Set(ByVal cursorType As Integer)
				If cursorType < DEFAULT_CURSOR OrElse cursorType > MOVE_CURSOR Then Throw New IllegalArgumentException("illegal cursor type")
				cursor = Cursor.getPredefinedCursor(cursorType)
			End Set
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>Component.getCursor()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property cursorType As Integer
			Get
				Return (cursor.type)
			End Get
		End Property

		''' <summary>
		''' Returns an array of all {@code Frame}s created by this application.
		''' If called from an applet, the array includes only the {@code Frame}s
		''' accessible by that applet.
		''' <p>
		''' <b>Warning:</b> this method may return system created frames, such
		''' as a shared, hidden frame which is used by Swing. Applications
		''' should not assume the existence of these frames, nor should an
		''' application assume anything about these frames such as component
		''' positions, <code>LayoutManager</code>s or serialization.
		''' <p>
		''' <b>Note</b>: To obtain a list of all ownerless windows, including
		''' ownerless {@code Dialog}s (introduced in release 1.6), use {@link
		''' Window#getOwnerlessWindows Window.getOwnerlessWindows}.
		''' </summary>
		''' <seealso cref= Window#getWindows() </seealso>
		''' <seealso cref= Window#getOwnerlessWindows
		''' 
		''' @since 1.2 </seealso>
		Public Property Shared frames As Frame()
			Get
				Dim allWindows_Renamed As Window() = Window.windows
    
				Dim frameCount As Integer = 0
				For Each w As Window In allWindows_Renamed
					If TypeOf w Is Frame Then frameCount += 1
				Next w
    
				Dim frames_Renamed As Frame() = New Frame(frameCount - 1){}
				Dim c As Integer = 0
				For Each w As Window In allWindows_Renamed
					If TypeOf w Is Frame Then
						frames_Renamed(c) = CType(w, Frame)
						c += 1
					End If
				Next w
    
				Return frames_Renamed
			End Get
		End Property

	'     Serialization support.  If there's a MenuBar we restore
	'     * its (transient) parent field here.  Likewise for top level
	'     * windows that are "owned" by this frame.
	'     

		''' <summary>
		''' <code>Frame</code>'s Serialized Data Version.
		''' 
		''' @serial
		''' </summary>
		Private frameSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' an optional serializable icon <code>Image</code>, which is
		''' available as of 1.4.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData an optional icon <code>Image</code> </param>
		''' <seealso cref= java.awt.Image </seealso>
		''' <seealso cref= #getIconImage </seealso>
		''' <seealso cref= #setIconImage(Image) </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If icons IsNot Nothing AndAlso icons.Count > 0 Then
				Dim icon1 As Image = icons(0)
				If TypeOf icon1 Is java.io.Serializable Then
					s.writeObject(icon1)
					Return
				End If
			End If
			s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code>.  Tries
		''' to read an icon <code>Image</code>, which is optional
		''' data available as of 1.4.  If an icon <code>Image</code>
		''' is not available, but anything other than an EOF
		''' is detected, an <code>OptionalDataException</code>
		''' will be thrown.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="java.io.OptionalDataException"> if an icon <code>Image</code>
		'''   is not available, but anything other than an EOF
		'''   is detected </exception>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless() </seealso>
		''' <seealso cref= java.awt.Image </seealso>
		''' <seealso cref= #getIconImage </seealso>
		''' <seealso cref= #setIconImage(Image) </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  ' HeadlessException is thrown by Window's readObject
		  s.defaultReadObject()
		  Try
			  Dim icon As Image = CType(s.readObject(), Image)
			  If icons Is Nothing Then
				  icons = New List(Of Image)
				  icons.Add(icon)
			  End If
		  Catch e As java.io.OptionalDataException
			  ' pre-1.4 instances will not have this optional data.
			  ' 1.6 and later instances serialize icons in the Window class
			  ' e.eof will be true to indicate that there is no more
			  ' data available for this object.

			  ' If e.eof is not true, throw the exception as it
			  ' might have been caused by unrelated reasons.
			  If Not e.eof Then Throw (e)
		  End Try

		  If menuBar IsNot Nothing Then menuBar.parent = Me

		  ' Ensure 1.1 serialized Frames can read & hook-up
		  ' owned windows properly
		  '
		  If ownedWindows IsNot Nothing Then
			  For i As Integer = 0 To ownedWindows.Count - 1
				  connectOwnedWindow(ownedWindows(i))
			  Next i
			  ownedWindows = Nothing
		  End If
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

	'    
	'     * --- Accessibility Support ---
	'     *
	'     

		''' <summary>
		''' Gets the AccessibleContext associated with this Frame.
		''' For frames, the AccessibleContext takes the form of an
		''' AccessibleAWTFrame.
		''' A new AccessibleAWTFrame instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTFrame that serves as the
		'''         AccessibleContext of this Frame
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTFrame(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Frame</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to frame user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTFrame
			Inherits AccessibleAWTWindow

			Private ReadOnly outerInstance As Frame

			Public Sub New(ByVal outerInstance As Frame)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -6172960752956030250L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.FRAME
				End Get
			End Property

			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.focusOwner IsNot Nothing Then states.add(AccessibleState.ACTIVE)
					If outerInstance.resizable Then states.add(AccessibleState.RESIZABLE)
					Return states
				End Get
			End Property


		End Class ' inner class AccessibleAWTFrame

	End Class

End Namespace