Imports Microsoft.VisualBasic
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
	''' A {@code Window} object is a top-level window with no borders and no
	''' menubar.
	''' The default layout for a window is {@code BorderLayout}.
	''' <p>
	''' A window must have either a frame, dialog, or another window defined as its
	''' owner when it's constructed.
	''' <p>
	''' In a multi-screen environment, you can create a {@code Window}
	''' on a different screen device by constructing the {@code Window}
	''' with <seealso cref="#Window(Window, GraphicsConfiguration)"/>.  The
	''' {@code GraphicsConfiguration} object is one of the
	''' {@code GraphicsConfiguration} objects of the target screen device.
	''' <p>
	''' In a virtual device multi-screen environment in which the desktop
	''' area could span multiple physical screen devices, the bounds of all
	''' configurations are relative to the virtual device coordinate system.
	''' The origin of the virtual-coordinate system is at the upper left-hand
	''' corner of the primary physical screen.  Depending on the location of
	''' the primary screen in the virtual device, negative coordinates are
	''' possible, as shown in the following figure.
	''' <p>
	''' <img src="doc-files/MultiScreen.gif"
	''' alt="Diagram shows virtual device containing 4 physical screens. Primary physical screen shows coords (0,0), other screen shows (-80,-100)."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' In such an environment, when calling {@code setLocation},
	''' you must pass a virtual coordinate to this method.  Similarly,
	''' calling {@code getLocationOnScreen} on a {@code Window} returns
	''' virtual device coordinates.  Call the {@code getBounds} method
	''' of a {@code GraphicsConfiguration} to find its origin in the virtual
	''' coordinate system.
	''' <p>
	''' The following code sets the location of a {@code Window}
	''' at (10, 10) relative to the origin of the physical screen
	''' of the corresponding {@code GraphicsConfiguration}.  If the
	''' bounds of the {@code GraphicsConfiguration} is not taken
	''' into account, the {@code Window} location would be set
	''' at (10, 10) relative to the virtual-coordinate system and would appear
	''' on the primary physical screen, which might be different from the
	''' physical screen of the specified {@code GraphicsConfiguration}.
	''' 
	''' <pre>
	'''      Window w = new Window(Window owner, GraphicsConfiguration gc);
	'''      Rectangle bounds = gc.getBounds();
	'''      w.setLocation(10 + bounds.x, 10 + bounds.y);
	''' </pre>
	''' 
	''' <p>
	''' Note: the location and size of top-level windows (including
	''' {@code Window}s, {@code Frame}s, and {@code Dialog}s)
	''' are under the control of the desktop's window management system.
	''' Calls to {@code setLocation}, {@code setSize}, and
	''' {@code setBounds} are requests (not directives) which are
	''' forwarded to the window management system.  Every effort will be
	''' made to honor such requests.  However, in some cases the window
	''' management system may ignore such requests, or modify the requested
	''' geometry in order to place and size the {@code Window} in a way
	''' that more closely matches the desktop settings.
	''' <p>
	''' Due to the asynchronous nature of native event handling, the results
	''' returned by {@code getBounds}, {@code getLocation},
	''' {@code getLocationOnScreen}, and {@code getSize} might not
	''' reflect the actual geometry of the Window on screen until the last
	''' request has been processed.  During the processing of subsequent
	''' requests these values might change accordingly while the window
	''' management system fulfills the requests.
	''' <p>
	''' An application may set the size and location of an invisible
	''' {@code Window} arbitrarily, but the window management system may
	''' subsequently change its size and/or location when the
	''' {@code Window} is made visible. One or more {@code ComponentEvent}s
	''' will be generated to indicate the new geometry.
	''' <p>
	''' Windows are capable of generating the following WindowEvents:
	''' WindowOpened, WindowClosed, WindowGainedFocus, WindowLostFocus.
	''' 
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff </summary>
	''' <seealso cref= WindowEvent </seealso>
	''' <seealso cref= #addWindowListener </seealso>
	''' <seealso cref= java.awt.BorderLayout
	''' @since       JDK1.0 </seealso>
	Public Class Window
		Inherits Container
		Implements Accessible

		''' <summary>
		''' Enumeration of available <i>window types</i>.
		''' 
		''' A window type defines the generic visual appearance and behavior of a
		''' top-level window. For example, the type may affect the kind of
		''' decorations of a decorated {@code Frame} or {@code Dialog} instance.
		''' <p>
		''' Some platforms may not fully support a certain window type. Depending on
		''' the level of support, some properties of the window type may be
		''' disobeyed.
		''' </summary>
		''' <seealso cref=   #getType </seealso>
		''' <seealso cref=   #setType
		''' @since 1.7 </seealso>
		Public Enum Type
			''' <summary>
			''' Represents a <i>normal</i> window.
			''' 
			''' This is the default type for objects of the {@code Window} class or
			''' its descendants. Use this type for regular top-level windows.
			''' </summary>
			NORMAL

			''' <summary>
			''' Represents a <i>utility</i> window.
			''' 
			''' A utility window is usually a small window such as a toolbar or a
			''' palette. The native system may render the window with smaller
			''' title-bar if the window is either a {@code Frame} or a {@code
			''' Dialog} object, and if it has its decorations enabled.
			''' </summary>
			UTILITY

			''' <summary>
			''' Represents a <i>popup</i> window.
			''' 
			''' A popup window is a temporary window such as a drop-down menu or a
			''' tooltip. On some platforms, windows of that type may be forcibly
			''' made undecorated even if they are instances of the {@code Frame} or
			''' {@code Dialog} [Class], and have decorations enabled.
			''' </summary>
			POPUP
		End Enum

		''' <summary>
		''' This represents the warning message that is
		''' to be displayed in a non secure window. ie :
		''' a window that has a security manager installed that denies
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}.
		''' This message can be displayed anywhere in the window.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getWarningString </seealso>
		Friend warningString As String

		''' <summary>
		''' {@code icons} is the graphical way we can
		''' represent the frames and dialogs.
		''' {@code Window} can't display icon but it's
		''' being inherited by owned {@code Dialog}s.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getIconImages </seealso>
		''' <seealso cref= #setIconImages </seealso>
		<NonSerialized> _
		Friend icons As IList(Of Image)

		''' <summary>
		''' Holds the reference to the component which last had focus in this window
		''' before it lost focus.
		''' </summary>
		<NonSerialized> _
		Private temporaryLostComponent As Component

		Friend Shared systemSyncLWRequests As Boolean = False
		Friend syncLWRequests As Boolean = False
		<NonSerialized> _
		Friend beforeFirstShow As Boolean = True
		<NonSerialized> _
		Private disposing As Boolean = False
		<NonSerialized> _
		Friend disposerRecord As WindowDisposerRecord = Nothing

		Friend Const OPENED As Integer = &H1

		''' <summary>
		''' An Integer value representing the Window State.
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #show </seealso>
		Friend state As Integer

		''' <summary>
		''' A boolean value representing Window always-on-top state
		''' @since 1.5
		''' @serial </summary>
		''' <seealso cref= #setAlwaysOnTop </seealso>
		''' <seealso cref= #isAlwaysOnTop </seealso>
		Private alwaysOnTop As Boolean

		''' <summary>
		''' Contains all the windows that have a peer object associated,
		''' i. e. between addNotify() and removeNotify() calls. The list
		''' of all Window instances can be obtained from AppContext object.
		''' 
		''' @since 1.6
		''' </summary>
		Private Shared ReadOnly allWindows As New sun.awt.util.IdentityArrayList(Of Window)

		''' <summary>
		''' A vector containing all the windows this
		''' window currently owns.
		''' @since 1.2 </summary>
		''' <seealso cref= #getOwnedWindows </seealso>
		<NonSerialized> _
		Friend ownedWindowList As New List(Of WeakReference(Of Window))

	'    
	'     * We insert a weak reference into the Vector of all Windows in AppContext
	'     * instead of 'this' so that garbage collection can still take place
	'     * correctly.
	'     
		<NonSerialized> _
		Private weakThis As WeakReference(Of Window)

		<NonSerialized> _
		Friend showWithParent As Boolean

		''' <summary>
		''' Contains the modal dialog that blocks this window, or null
		''' if the window is unblocked.
		''' 
		''' @since 1.6
		''' </summary>
		<NonSerialized> _
		Friend modalBlocker As Dialog

		''' <summary>
		''' @serial
		''' </summary>
		''' <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
		''' <seealso cref= #getModalExclusionType </seealso>
		''' <seealso cref= #setModalExclusionType
		''' 
		''' @since 1.6 </seealso>
		Friend modalExclusionType As Dialog.ModalExclusionType

		<NonSerialized> _
		Friend windowListener As WindowListener
		<NonSerialized> _
		Friend windowStateListener As WindowStateListener
		<NonSerialized> _
		Friend windowFocusListener As WindowFocusListener

		<NonSerialized> _
		Friend inputContext As java.awt.im.InputContext
		<NonSerialized> _
		Private inputContextLock As New Object

		''' <summary>
		''' Unused. Maintained for serialization backward-compatibility.
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private focusMgr As FocusManager

		''' <summary>
		''' Indicates whether this Window can become the focused Window.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getFocusableWindowState </seealso>
		''' <seealso cref= #setFocusableWindowState
		''' @since 1.4 </seealso>
		Private focusableWindowState As Boolean = True

		''' <summary>
		''' Indicates whether this window should receive focus on
		''' subsequently being shown (with a call to {@code setVisible(true)}), or
		''' being moved to the front (with a call to {@code toFront()}).
		''' 
		''' @serial </summary>
		''' <seealso cref= #setAutoRequestFocus </seealso>
		''' <seealso cref= #isAutoRequestFocus
		''' @since 1.7 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private autoRequestFocus As Boolean = True

	'    
	'     * Indicates that this window is being shown. This flag is set to true at
	'     * the beginning of show() and to false at the end of show().
	'     *
	'     * @see #show()
	'     * @see Dialog#shouldBlock
	'     
		<NonSerialized> _
		Friend isInShow As Boolean = False

		''' <summary>
		''' The opacity level of the window
		''' 
		''' @serial </summary>
		''' <seealso cref= #setOpacity(float) </seealso>
		''' <seealso cref= #getOpacity()
		''' @since 1.7 </seealso>
		Private opacity As Single = 1.0f

		''' <summary>
		''' The shape assigned to this window. This field is set to {@code null} if
		''' no shape is set (rectangular window).
		''' 
		''' @serial </summary>
		''' <seealso cref= #getShape() </seealso>
		''' <seealso cref= #setShape(Shape)
		''' @since 1.7 </seealso>
		Private shape As Shape = Nothing

		Private Const base As String = "win"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 4497834738069338734L

		Private Shared ReadOnly log As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.Window")

		Private Shared ReadOnly locationByPlatformProp As Boolean

		<NonSerialized> _
		Friend isTrayIconWindow As Boolean = False

		''' <summary>
		''' These fields are initialized in the native peer code
		''' or via AWTAccessor's WindowAccessor.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private securityWarningWidth As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private securityWarningHeight As Integer = 0

		''' <summary>
		''' These fields represent the desired location for the security
		''' warning if this window is untrusted.
		''' See com.sun.awt.SecurityWarning for more details.
		''' </summary>
		<NonSerialized> _
		Private securityWarningPointX As Double = 2.0
		<NonSerialized> _
		Private securityWarningPointY As Double = 0.0
		<NonSerialized> _
		Private securityWarningAlignmentX As Single = RIGHT_ALIGNMENT
		<NonSerialized> _
		Private securityWarningAlignmentY As Single = TOP_ALIGNMENT

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

			Dim s As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.awt.syncLWRequests"))
			systemSyncLWRequests = (s IsNot Nothing AndAlso s.Equals("true"))
			s = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.awt.Window.locationByPlatform"))
			locationByPlatformProp = (s IsNot Nothing AndAlso s.Equals("true"))
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setWindowAccessor(New sun.awt.AWTAccessor.WindowAccessor()
	'		{
	'			public float getOpacity(Window window)
	'			{
	'				Return window.opacity;
	'			}
	'			public void setOpacity(Window window, float opacity)
	'			{
	'				window.setOpacity(opacity);
	'			}
	'			public Shape getShape(Window window)
	'			{
	'				Return window.getShape();
	'			}
	'			public void setShape(Window window, Shape shape)
	'			{
	'				window.setShape(shape);
	'			}
	'			public void setOpaque(Window window, boolean opaque)
	'			{
	'				Color bg = window.getBackground();
	'				if (bg == Nothing)
	'				{
	'					bg = New Color(0, 0, 0, 0);
	'				}
	'				window.setBackground(New Color(bg.getRed(), bg.getGreen(), bg.getBlue(), opaque ? 255 : 0));
	'			}
	'			public void updateWindow(Window window)
	'			{
	'				window.updateWindow();
	'			}
	'
	'			public Dimension getSecurityWarningSize(Window window)
	'			{
	'				Return New Dimension(window.securityWarningWidth, window.securityWarningHeight);
	'			}
	'
	'			public void setSecurityWarningSize(Window window, int width, int height)
	'			{
	'				window.securityWarningWidth = width;
	'				window.securityWarningHeight = height;
	'			}
	'
	'			public void setSecurityWarningPosition(Window window, Point2D point, float alignmentX, float alignmentY)
	'			{
	'				window.securityWarningPointX = point.getX();
	'				window.securityWarningPointY = point.getY();
	'				window.securityWarningAlignmentX = alignmentX;
	'				window.securityWarningAlignmentY = alignmentY;
	'
	'				synchronized(window.getTreeLock())
	'				{
	'					WindowPeer peer = (WindowPeer)window.getPeer();
	'					if (peer != Nothing)
	'					{
	'						peer.repositionSecurityWarning();
	'					}
	'				}
	'			}
	'
	'			public Point2D calculateSecurityWarningPosition(Window window, double x, double y, double w, double h)
	'			{
	'				Return window.calculateSecurityWarningPosition(x, y, w, h);
	'			}
	'
	'			public void setLWRequestStatus(Window changed, boolean status)
	'			{
	'				changed.syncLWRequests = status;
	'			}
	'
	'			public boolean isAutoRequestFocus(Window w)
	'			{
	'				Return w.autoRequestFocus;
	'			}
	'
	'			public boolean isTrayIconWindow(Window w)
	'			{
	'				Return w.isTrayIconWindow;
	'			}
	'
	'			public void setTrayIconWindow(Window w, boolean isTrayIconWindow)
	'			{
	'				w.isTrayIconWindow = isTrayIconWindow;
	'			}
	'		}); ' WindowAccessor
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs for fields that may be
		'''   accessed from C.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible window in default size with the
		''' specified {@code GraphicsConfiguration}.
		''' <p>
		''' If there is a security manager, then it is invoked to check
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}
		''' to determine whether or not the window must be displayed with
		''' a warning banner.
		''' </summary>
		''' <param name="gc"> the {@code GraphicsConfiguration} of the target screen
		'''     device. If {@code gc} is {@code null}, the system default
		'''     {@code GraphicsConfiguration} is assumed </param>
		''' <exception cref="IllegalArgumentException"> if {@code gc}
		'''    is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Friend Sub New(ByVal gc As GraphicsConfiguration)
			init(gc)
		End Sub

		<NonSerialized> _
		Friend anchor As New Object
		Friend Class WindowDisposerRecord
			Implements sun.java2d.DisposerRecord

			Friend owner As WeakReference(Of Window)
			Friend ReadOnly weakThis As WeakReference(Of Window)
			Friend ReadOnly context As WeakReference(Of sun.awt.AppContext)

			Friend Sub New(ByVal context As sun.awt.AppContext, ByVal victim As Window)
				weakThis = victim.weakThis
				Me.context = New WeakReference(Of sun.awt.AppContext)(context)
			End Sub

			Public Overridable Sub updateOwner()
				Dim victim As Window = weakThis.get()
				owner = If(victim Is Nothing, Nothing, New WeakReference(Of Window)(victim.owner))
			End Sub

			Public Overridable Sub dispose()
				If owner IsNot Nothing Then
					Dim parent As Window = owner.get()
					If parent IsNot Nothing Then parent.removeOwnedWindow(weakThis)
				End If
				Dim ac As sun.awt.AppContext = context.get()
				If Nothing IsNot ac Then Window.removeFromWindowList(ac, weakThis)
			End Sub
		End Class

		Private Function initGC(ByVal gc As GraphicsConfiguration) As GraphicsConfiguration
			GraphicsEnvironment.checkHeadless()

			If gc Is Nothing Then gc = GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice.defaultConfiguration
			graphicsConfiguration = gc

			Return gc
		End Function

		Private Sub init(ByVal gc As GraphicsConfiguration)
			GraphicsEnvironment.checkHeadless()

			syncLWRequests = systemSyncLWRequests

			weakThis = New WeakReference(Of Window)(Me)
			addToWindowList()

			warningStringing()
			Me.cursor = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
			Me.visible = False

			gc = initGC(gc)

			If gc.device.type <> GraphicsDevice.TYPE_RASTER_SCREEN Then Throw New IllegalArgumentException("not a screen device")
			layout = New BorderLayout

			' offset the initial location with the original of the screen 
			' and any insets                                              
			Dim screenBounds As Rectangle = gc.bounds
			Dim screenInsets As Insets = toolkit.getScreenInsets(gc)
			Dim x_Renamed As Integer = x + screenBounds.x + screenInsets.left
			Dim y_Renamed As Integer = y + screenBounds.y + screenInsets.top
			If x_Renamed <> Me.x OrElse y_Renamed <> Me.y Then
				locationion(x_Renamed, y_Renamed)
				' reset after setLocation 
				locationByPlatform = locationByPlatformProp
			End If

			modalExclusionType = Dialog.ModalExclusionType.NO_EXCLUDE
			disposerRecord = New WindowDisposerRecord(appContext, Me)
			sun.java2d.Disposer.addRecord(anchor, disposerRecord)

			sun.awt.SunToolkit.checkAndSetPolicy(Me)
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible window in the default size.
		''' <p>
		''' If there is a security manager set, it is invoked to check
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}.
		''' If that check fails with a {@code SecurityException} then a warning
		''' banner is created.
		''' </summary>
		''' <exception cref="HeadlessException"> when
		'''     {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Friend Sub New()
			GraphicsEnvironment.checkHeadless()
			init(CType(Nothing, GraphicsConfiguration))
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible window with the specified
		''' {@code Frame} as its owner. The window will not be focusable
		''' unless its owner is showing on the screen.
		''' <p>
		''' If there is a security manager set, it is invoked to check
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}.
		''' If that check fails with a {@code SecurityException} then a warning
		''' banner is created.
		''' </summary>
		''' <param name="owner"> the {@code Frame} to act as owner or {@code null}
		'''    if this window has no owner </param>
		''' <exception cref="IllegalArgumentException"> if the {@code owner}'s
		'''    {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    {@code GraphicsEnvironment.isHeadless} returns {@code true}
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #isShowing </seealso>
		Public Sub New(ByVal owner As Frame)
			Me.New(If(owner Is Nothing, CType(Nothing, GraphicsConfiguration), owner.graphicsConfiguration))
			ownedInit(owner)
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible window with the specified
		''' {@code Window} as its owner. This window will not be focusable
		''' unless its nearest owning {@code Frame} or {@code Dialog}
		''' is showing on the screen.
		''' <p>
		''' If there is a security manager set, it is invoked to check
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}.
		''' If that check fails with a {@code SecurityException} then a
		''' warning banner is created.
		''' </summary>
		''' <param name="owner"> the {@code Window} to act as owner or
		'''     {@code null} if this window has no owner </param>
		''' <exception cref="IllegalArgumentException"> if the {@code owner}'s
		'''     {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     {@code GraphicsEnvironment.isHeadless()} returns
		'''     {@code true}
		''' </exception>
		''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=       #isShowing
		''' 
		''' @since     1.2 </seealso>
		Public Sub New(ByVal owner As Window)
			Me.New(If(owner Is Nothing, CType(Nothing, GraphicsConfiguration), owner.graphicsConfiguration))
			ownedInit(owner)
		End Sub

		''' <summary>
		''' Constructs a new, initially invisible window with the specified owner
		''' {@code Window} and a {@code GraphicsConfiguration}
		''' of a screen device. The Window will not be focusable unless
		''' its nearest owning {@code Frame} or {@code Dialog}
		''' is showing on the screen.
		''' <p>
		''' If there is a security manager set, it is invoked to check
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}. If that
		''' check fails with a {@code SecurityException} then a warning banner
		''' is created.
		''' </summary>
		''' <param name="owner"> the window to act as owner or {@code null}
		'''     if this window has no owner </param>
		''' <param name="gc"> the {@code GraphicsConfiguration} of the target
		'''     screen device; if {@code gc} is {@code null},
		'''     the system default {@code GraphicsConfiguration} is assumed </param>
		''' <exception cref="IllegalArgumentException"> if {@code gc}
		'''     is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     {@code GraphicsEnvironment.isHeadless()} returns
		'''     {@code true}
		''' </exception>
		''' <seealso cref=       java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref=       GraphicsConfiguration#getBounds </seealso>
		''' <seealso cref=       #isShowing
		''' @since     1.3 </seealso>
		Public Sub New(ByVal owner As Window, ByVal gc As GraphicsConfiguration)
			Me.New(gc)
			ownedInit(owner)
		End Sub

		Private Sub ownedInit(ByVal owner As Window)
			Me.parent = owner
			If owner IsNot Nothing Then
				owner.addOwnedWindow(weakThis)
				If owner.alwaysOnTop Then
					Try
						alwaysOnTop = True
					Catch ignore As SecurityException
					End Try
				End If
			End If

			' WindowDisposerRecord requires a proper value of parent field.
			disposerRecord.updateOwner()
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Window)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Returns the sequence of images to be displayed as the icon for this window.
		''' <p>
		''' This method returns a copy of the internally stored list, so all operations
		''' on the returned object will not affect the window's behavior.
		''' </summary>
		''' <returns>    the copy of icon images' list for this window, or
		'''            empty list if this window doesn't have icon images. </returns>
		''' <seealso cref=       #setIconImages </seealso>
		''' <seealso cref=       #setIconImage(Image)
		''' @since     1.6 </seealso>
		Public Overridable Property iconImages As IList(Of Image)
			Get
				Dim icons As IList(Of Image) = Me.icons
				If icons Is Nothing OrElse icons.Count = 0 Then Return New List(Of Image)
				Return New List(Of Image)(icons)
			End Get
			Set(ByVal icons As IList(Of T1))
				Me.icons = If(icons Is Nothing, New List(Of Image), New List(Of Image)(icons))
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.updateIconImages()
				' Always send a property change event
				firePropertyChange("iconImage", Nothing, Nothing)
			End Set
		End Property


		''' <summary>
		''' Sets the image to be displayed as the icon for this window.
		''' <p>
		''' This method can be used instead of <seealso cref="#setIconImages setIconImages()"/>
		''' to specify a single image as a window's icon.
		''' <p>
		''' The following statement:
		''' <pre>
		'''     setIconImage(image);
		''' </pre>
		''' is equivalent to:
		''' <pre>
		'''     ArrayList&lt;Image&gt; imageList = new ArrayList&lt;Image&gt;();
		'''     imageList.add(image);
		'''     setIconImages(imageList);
		''' </pre>
		''' <p>
		''' Note : Native windowing systems may use different images of differing
		''' dimensions to represent a window, depending on the context (e.g.
		''' window decoration, window list, taskbar, etc.). They could also use
		''' just a single image for all contexts or no image at all.
		''' </summary>
		''' <param name="image"> the icon image to be displayed. </param>
		''' <seealso cref=       #setIconImages </seealso>
		''' <seealso cref=       #getIconImages()
		''' @since     1.6 </seealso>
		Public Overridable Property iconImage As Image
			Set(ByVal image_Renamed As Image)
				Dim imageList As New List(Of Image)
				If image_Renamed IsNot Nothing Then imageList.Add(image_Renamed)
				iconImages = imageList
			End Set
		End Property

		''' <summary>
		''' Makes this Window displayable by creating the connection to its
		''' native screen resource.
		''' This method is called internally by the toolkit and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= Container#removeNotify
		''' @since JDK1.0 </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				Dim parent_Renamed As Container = Me.parent
				If parent_Renamed IsNot Nothing AndAlso parent_Renamed.peer Is Nothing Then parent_Renamed.addNotify()
				If peer Is Nothing Then peer = toolkit.createWindow(Me)
				SyncLock allWindows
					allWindows.add(Me)
				End SyncLock
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub removeNotify()
			SyncLock treeLock
				SyncLock allWindows
					allWindows.remove(Me)
				End SyncLock
				MyBase.removeNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Causes this Window to be sized to fit the preferred size
		''' and layouts of its subcomponents. The resulting width and
		''' height of the window are automatically enlarged if either
		''' of dimensions is less than the minimum size as specified
		''' by the previous call to the {@code setMinimumSize} method.
		''' <p>
		''' If the window and/or its owner are not displayable yet,
		''' both of them are made displayable before calculating
		''' the preferred size. The Window is validated after its
		''' size is being calculated.
		''' </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #setMinimumSize </seealso>
		Public Overridable Sub pack()
			Dim parent_Renamed As Container = Me.parent
			If parent_Renamed IsNot Nothing AndAlso parent_Renamed.peer Is Nothing Then parent_Renamed.addNotify()
			If peer Is Nothing Then addNotify()
			Dim newSize As Dimension = preferredSize
			If peer IsNot Nothing Then clientSizeize(newSize.width, newSize.height)

			If beforeFirstShow Then isPacked = True

			validateUnconditionally()
		End Sub

		''' <summary>
		''' Sets the minimum size of this window to a constant
		''' value.  Subsequent calls to {@code getMinimumSize}
		''' will always return this value. If current window's
		''' size is less than {@code minimumSize} the size of the
		''' window is automatically enlarged to honor the minimum size.
		''' <p>
		''' If the {@code setSize} or {@code setBounds} methods
		''' are called afterwards with a width or height less than
		''' that was specified by the {@code setMinimumSize} method
		''' the window is automatically enlarged to meet
		''' the {@code minimumSize} value. The {@code minimumSize}
		''' value also affects the behaviour of the {@code pack} method.
		''' <p>
		''' The default behavior is restored by setting the minimum size
		''' parameter to the {@code null} value.
		''' <p>
		''' Resizing operation may be restricted if the user tries
		''' to resize window below the {@code minimumSize} value.
		''' This behaviour is platform-dependent.
		''' </summary>
		''' <param name="minimumSize"> the new minimum size of this window </param>
		''' <seealso cref= Component#setMinimumSize </seealso>
		''' <seealso cref= #getMinimumSize </seealso>
		''' <seealso cref= #isMinimumSizeSet </seealso>
		''' <seealso cref= #setSize(Dimension) </seealso>
		''' <seealso cref= #pack
		''' @since 1.6 </seealso>
		Public Overrides Property minimumSize As Dimension
			Set(ByVal minimumSize As Dimension)
				SyncLock treeLock
					MyBase.minimumSize = minimumSize
					Dim size_Renamed As Dimension = size
					If minimumSizeSet Then
						If size_Renamed.width < minimumSize.width OrElse size_Renamed.height < minimumSize.height Then
							Dim nw As Integer = Math.Max(width, minimumSize.width)
							Dim nh As Integer = Math.Max(height, minimumSize.height)
							sizeize(nw, nh)
						End If
					End If
					If peer IsNot Nothing Then CType(peer, java.awt.peer.WindowPeer).updateMinimumSize()
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The {@code d.width} and {@code d.height} values
		''' are automatically enlarged if either is less than
		''' the minimum size as specified by previous call to
		''' {@code setMinimumSize}.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <seealso cref= #getSize </seealso>
		''' <seealso cref= #setBounds </seealso>
		''' <seealso cref= #setMinimumSize
		''' @since 1.6 </seealso>
		Public Overrides Property size As Dimension
			Set(ByVal d As Dimension)
				MyBase.size = d
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The {@code width} and {@code height} values
		''' are automatically enlarged if either is less than
		''' the minimum size as specified by previous call to
		''' {@code setMinimumSize}.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <seealso cref= #getSize </seealso>
		''' <seealso cref= #setBounds </seealso>
		''' <seealso cref= #setMinimumSize
		''' @since 1.6 </seealso>
		Public Overrides Sub setSize(ByVal width As Integer, ByVal height As Integer)
			MyBase.sizeize(width, height)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		Public Overrides Sub setLocation(ByVal x As Integer, ByVal y As Integer)
			MyBase.locationion(x, y)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		Public Overrides Property location As Point
			Set(ByVal p As Point)
				MyBase.location = p
			End Set
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by {@code setBounds(int, int, int, int)}. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overrides Sub reshape(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			If minimumSizeSet Then
				Dim minSize As Dimension = minimumSize
				If width < minSize.width Then width = minSize.width
				If height < minSize.height Then height = minSize.height
			End If
			MyBase.reshape(x, y, width, height)
		End Sub

		Friend Overridable Sub setClientSize(ByVal w As Integer, ByVal h As Integer)
			SyncLock treeLock
				boundsOp = java.awt.peer.ComponentPeer.SET_CLIENT_SIZE
				boundsnds(x, y, w, h)
			End SyncLock
		End Sub

		Private Shared ReadOnly beforeFirstWindowShown As New java.util.concurrent.atomic.AtomicBoolean(True)

		Friend Sub closeSplashScreen()
			If isTrayIconWindow Then Return
			If beforeFirstWindowShown.getAndSet(False) Then
				' We don't use SplashScreen.getSplashScreen() to avoid instantiating
				' the object if it hasn't been requested by user code explicitly
				sun.awt.SunToolkit.closeSplashScreen()
				SplashScreen.markClosed()
			End If
		End Sub

		''' <summary>
		''' Shows or hides this {@code Window} depending on the value of parameter
		''' {@code b}.
		''' <p>
		''' If the method shows the window then the window is also made
		''' focused under the following conditions:
		''' <ul>
		''' <li> The {@code Window} meets the requirements outlined in the
		'''      <seealso cref="#isFocusableWindow"/> method.
		''' <li> The {@code Window}'s {@code autoRequestFocus} property is of the {@code true} value.
		''' <li> Native windowing system allows the {@code Window} to get focused.
		''' </ul>
		''' There is an exception for the second condition (the value of the
		''' {@code autoRequestFocus} property). The property is not taken into account if the
		''' window is a modal dialog, which blocks the currently focused window.
		''' <p>
		''' Developers must never assume that the window is the focused or active window
		''' until it receives a WINDOW_GAINED_FOCUS or WINDOW_ACTIVATED event. </summary>
		''' <param name="b">  if {@code true}, makes the {@code Window} visible,
		''' otherwise hides the {@code Window}.
		''' If the {@code Window} and/or its owner
		''' are not yet displayable, both are made displayable.  The
		''' {@code Window} will be validated prior to being made visible.
		''' If the {@code Window} is already visible, this will bring the
		''' {@code Window} to the front.<p>
		''' If {@code false}, hides this {@code Window}, its subcomponents, and all
		''' of its owned children.
		''' The {@code Window} and its subcomponents can be made visible again
		''' with a call to {@code #setVisible(true)}. </param>
		''' <seealso cref= java.awt.Component#isDisplayable </seealso>
		''' <seealso cref= java.awt.Component#setVisible </seealso>
		''' <seealso cref= java.awt.Window#toFront </seealso>
		''' <seealso cref= java.awt.Window#dispose </seealso>
		''' <seealso cref= java.awt.Window#setAutoRequestFocus </seealso>
		''' <seealso cref= java.awt.Window#isFocusableWindow </seealso>
		Public Overrides Property visible As Boolean
			Set(ByVal b As Boolean)
				MyBase.visible = b
			End Set
		End Property

		''' <summary>
		''' Makes the Window visible. If the Window and/or its owner
		''' are not yet displayable, both are made displayable.  The
		''' Window will be validated prior to being made visible.
		''' If the Window is already visible, this will bring the Window
		''' to the front. </summary>
		''' <seealso cref=       Component#isDisplayable </seealso>
		''' <seealso cref=       #toFront </seealso>
		''' @deprecated As of JDK version 1.5, replaced by
		''' <seealso cref="#setVisible(boolean)"/>. 
		<Obsolete("As of JDK version 1.5, replaced by")> _
		Public Overrides Sub show()
			If peer Is Nothing Then addNotify()
			validateUnconditionally()

			isInShow = True
			If visible Then
				toFront()
			Else
				beforeFirstShow = False
				closeSplashScreen()
				Dialog.checkShouldBeBlocked(Me)
				MyBase.show()
				SyncLock treeLock
					Me.locationByPlatform = False
				End SyncLock
				For i As Integer = 0 To ownedWindowList.Count - 1
					Dim child As Window = ownedWindowList(i).get()
					If (child IsNot Nothing) AndAlso child.showWithParent Then
						child.show()
						child.showWithParent = False
					End If ' endif
				Next i ' endfor
				If Not modalBlocked Then
					updateChildrenBlocking()
				Else
					' fix for 6532736: after this window is shown, its blocker
					' should be raised to front
					modalBlocker.toFront_NoClientCode()
				End If
				If TypeOf Me Is Frame OrElse TypeOf Me Is Dialog Then updateChildFocusableWindowState(Me)
			End If
			isInShow = False

			' If first time shown, generate WindowOpened event
			If (state And OPENED) = 0 Then
				postWindowEvent(WindowEvent.WINDOW_OPENED)
				state = state Or OPENED
			End If
		End Sub

		Shared Sub updateChildFocusableWindowState(ByVal w As Window)
			If w.peer IsNot Nothing AndAlso w.showing Then CType(w.peer, java.awt.peer.WindowPeer).updateFocusableWindowState()
			For i As Integer = 0 To w.ownedWindowList.Count - 1
				Dim child As Window = w.ownedWindowList(i).get()
				If child IsNot Nothing Then updateChildFocusableWindowState(child)
			Next i
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub postWindowEvent(ByVal id As Integer)
			If windowListener IsNot Nothing OrElse (eventMask And AWTEvent.WINDOW_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.WINDOW_EVENT_MASK) Then
				Dim e As New WindowEvent(Me, id)
				Toolkit.eventQueue.postEvent(e)
			End If
		End Sub

		''' <summary>
		''' Hide this Window, its subcomponents, and all of its owned children.
		''' The Window and its subcomponents can be made visible again
		''' with a call to {@code show}. </summary>
		''' <seealso cref= #show </seealso>
		''' <seealso cref= #dispose </seealso>
		''' @deprecated As of JDK version 1.5, replaced by
		''' <seealso cref="#setVisible(boolean)"/>. 
		<Obsolete("As of JDK version 1.5, replaced by")> _
		Public Overrides Sub hide()
			SyncLock ownedWindowList
				For i As Integer = 0 To ownedWindowList.Count - 1
					Dim child As Window = ownedWindowList(i).get()
					If (child IsNot Nothing) AndAlso child.visible Then
						child.hide()
						child.showWithParent = True
					End If
				Next i
			End SyncLock
			If modalBlocked Then modalBlocker.unblockWindow(Me)
			MyBase.hide()
			SyncLock treeLock
				Me.locationByPlatform = False
			End SyncLock
		End Sub

		Friend NotOverridable Overrides Sub clearMostRecentFocusOwnerOnHide()
			' do nothing 
		End Sub

		''' <summary>
		''' Releases all of the native screen resources used by this
		''' {@code Window}, its subcomponents, and all of its owned
		''' children. That is, the resources for these {@code Component}s
		''' will be destroyed, any memory they consume will be returned to the
		''' OS, and they will be marked as undisplayable.
		''' <p>
		''' The {@code Window} and its subcomponents can be made displayable
		''' again by rebuilding the native resources with a subsequent call to
		''' {@code pack} or {@code show}. The states of the recreated
		''' {@code Window} and its subcomponents will be identical to the
		''' states of these objects at the point where the {@code Window}
		''' was disposed (not accounting for additional modifications between
		''' those actions).
		''' <p>
		''' <b>Note</b>: When the last displayable window
		''' within the Java virtual machine (VM) is disposed of, the VM may
		''' terminate.  See <a href="doc-files/AWTThreadIssues.html#Autoshutdown">
		''' AWT Threading Issues</a> for more information. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #pack </seealso>
		''' <seealso cref= #show </seealso>
		Public Overridable Sub dispose()
			doDispose()
		End Sub

	'    
	'     * Fix for 4872170.
	'     * If dispose() is called on parent then its children have to be disposed as well
	'     * as reported in javadoc. So we need to implement this functionality even if a
	'     * child overrides dispose() in a wrong way without calling super.dispose().
	'     
		Friend Overridable Sub disposeImpl()
			Dispose()
			If peer IsNot Nothing Then doDispose()
		End Sub

		Friend Overridable Sub doDispose()
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'		class DisposeAction implements Runnable
	'	{
	'		public void run()
	'		{
	'			disposing = True;
	'			try
	'			{
	'				' Check if this window is the fullscreen window for the
	'				' device. Exit the fullscreen mode prior to disposing
	'				' of the window if that's the case.
	'				GraphicsDevice gd = getGraphicsConfiguration().getDevice();
	'				if (gd.getFullScreenWindow() == Window.this)
	'				{
	'					gd.setFullScreenWindow(Nothing);
	'				}
	'
	'				Object[] ownedWindowArray;
	'				synchronized(ownedWindowList)
	'				{
	'					ownedWindowArray = New Object[ownedWindowList.size()];
	'					ownedWindowList.copyInto(ownedWindowArray);
	'				}
	'				for (int i = 0; i < ownedWindowArray.length; i += 1)
	'				{
	'					Window child = (Window)(((WeakReference)(ownedWindowArray[i])).get());
	'					if (child != Nothing)
	'					{
	'						child.disposeImpl();
	'					}
	'				}
	'				hide();
	'				beforeFirstShow = True;
	'				removeNotify();
	'				synchronized(inputContextLock)
	'				{
	'					if (inputContext != Nothing)
	'					{
	'						inputContext.dispose();
	'						inputContext = Nothing;
	'					}
	'				}
	'				clearCurrentFocusCycleRootOnHide();
	'			}
	'			finally
	'			{
	'				disposing = False;
	'			}
	'		}
	'	}
			Dim fireWindowClosedEvent As Boolean = displayable
			Dim action As New DisposeAction
			If EventQueue.dispatchThread Then
				action.run()
			Else
				Try
					EventQueue.invokeAndWait(Me, action)
				Catch e As InterruptedException
					Console.Error.WriteLine("Disposal was interrupted:")
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				Catch e As InvocationTargetException
					Console.Error.WriteLine("Exception during disposal:")
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End If
			' Execute outside the Runnable because postWindowEvent is
			' synchronized on (this). We don't need to synchronize the call
			' on the EventQueue anyways.
			If fireWindowClosedEvent Then postWindowEvent(WindowEvent.WINDOW_CLOSED)
		End Sub

	'    
	'     * Should only be called while holding the tree lock.
	'     * It's overridden here because parent == owner in Window,
	'     * and we shouldn't adjust counter on owner
	'     
		Friend Overrides Sub adjustListeningChildrenOnParent(ByVal mask As Long, ByVal num As Integer)
		End Sub

		' Should only be called while holding tree lock
		Friend Overrides Sub adjustDecendantsOnParent(ByVal num As Integer)
			' do nothing since parent == owner and we shouldn't
			' ajust counter on owner
		End Sub

		''' <summary>
		''' If this Window is visible, brings this Window to the front and may make
		''' it the focused Window.
		''' <p>
		''' Places this Window at the top of the stacking order and shows it in
		''' front of any other Windows in this VM. No action will take place if this
		''' Window is not visible. Some platforms do not allow Windows which own
		''' other Windows to appear on top of those owned Windows. Some platforms
		''' may not permit this VM to place its Windows above windows of native
		''' applications, or Windows of other VMs. This permission may depend on
		''' whether a Window in this VM is already focused. Every attempt will be
		''' made to move this Window as high as possible in the stacking order;
		''' however, developers should not assume that this method will move this
		''' Window above all other windows in every situation.
		''' <p>
		''' Developers must never assume that this Window is the focused or active
		''' Window until this Window receives a WINDOW_GAINED_FOCUS or WINDOW_ACTIVATED
		''' event. On platforms where the top-most window is the focused window, this
		''' method will <b>probably</b> focus this Window (if it is not already focused)
		''' under the following conditions:
		''' <ul>
		''' <li> The window meets the requirements outlined in the
		'''      <seealso cref="#isFocusableWindow"/> method.
		''' <li> The window's property {@code autoRequestFocus} is of the
		'''      {@code true} value.
		''' <li> Native windowing system allows the window to get focused.
		''' </ul>
		''' On platforms where the stacking order does not typically affect the focused
		''' window, this method will <b>probably</b> leave the focused and active
		''' Windows unchanged.
		''' <p>
		''' If this method causes this Window to be focused, and this Window is a
		''' Frame or a Dialog, it will also become activated. If this Window is
		''' focused, but it is not a Frame or a Dialog, then the first Frame or
		''' Dialog that is an owner of this Window will be activated.
		''' <p>
		''' If this window is blocked by modal dialog, then the blocking dialog
		''' is brought to the front and remains above the blocked window.
		''' </summary>
		''' <seealso cref=       #toBack </seealso>
		''' <seealso cref=       #setAutoRequestFocus </seealso>
		''' <seealso cref=       #isFocusableWindow </seealso>
		Public Overridable Sub toFront()
			toFront_NoClientCode()
		End Sub

		' This functionality is implemented in a final package-private method
		' to insure that it cannot be overridden by client subclasses.
		Friend Sub toFront_NoClientCode()
			If visible Then
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.toFront()
				If modalBlocked Then modalBlocker.toFront_NoClientCode()
			End If
		End Sub

		''' <summary>
		''' If this Window is visible, sends this Window to the back and may cause
		''' it to lose focus or activation if it is the focused or active Window.
		''' <p>
		''' Places this Window at the bottom of the stacking order and shows it
		''' behind any other Windows in this VM. No action will take place is this
		''' Window is not visible. Some platforms do not allow Windows which are
		''' owned by other Windows to appear below their owners. Every attempt will
		''' be made to move this Window as low as possible in the stacking order;
		''' however, developers should not assume that this method will move this
		''' Window below all other windows in every situation.
		''' <p>
		''' Because of variations in native windowing systems, no guarantees about
		''' changes to the focused and active Windows can be made. Developers must
		''' never assume that this Window is no longer the focused or active Window
		''' until this Window receives a WINDOW_LOST_FOCUS or WINDOW_DEACTIVATED
		''' event. On platforms where the top-most window is the focused window,
		''' this method will <b>probably</b> cause this Window to lose focus. In
		''' that case, the next highest, focusable Window in this VM will receive
		''' focus. On platforms where the stacking order does not typically affect
		''' the focused window, this method will <b>probably</b> leave the focused
		''' and active Windows unchanged.
		''' </summary>
		''' <seealso cref=       #toFront </seealso>
		Public Overridable Sub toBack()
			toBack_NoClientCode()
		End Sub

		' This functionality is implemented in a final package-private method
		' to insure that it cannot be overridden by client subclasses.
		Friend Sub toBack_NoClientCode()
			If alwaysOnTop Then
				Try
					alwaysOnTop = False
				Catch e As SecurityException
				End Try
			End If
			If visible Then
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.toBack()
			End If
		End Sub

		''' <summary>
		''' Returns the toolkit of this frame. </summary>
		''' <returns>    the toolkit of this window. </returns>
		''' <seealso cref=       Toolkit </seealso>
		''' <seealso cref=       Toolkit#getDefaultToolkit </seealso>
		''' <seealso cref=       Component#getToolkit </seealso>
		Public Property Overrides toolkit As Toolkit
			Get
				Return Toolkit.defaultToolkit
			End Get
		End Property

		''' <summary>
		''' Gets the warning string that is displayed with this window.
		''' If this window is insecure, the warning string is displayed
		''' somewhere in the visible area of the window. A window is
		''' insecure if there is a security manager and the security
		''' manager denies
		''' {@code AWTPermission("showWindowWithoutWarningBanner")}.
		''' <p>
		''' If the window is secure, then {@code getWarningString}
		''' returns {@code null}. If the window is insecure, this
		''' method checks for the system property
		''' {@code awt.appletWarning}
		''' and returns the string value of that property. </summary>
		''' <returns>    the warning string for this window. </returns>
		Public Property warningString As String
			Get
				Return warningString
			End Get
		End Property

		Private Sub setWarningString()
			warningString = Nothing
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Try
					sm.checkPermission(sun.security.util.SecurityConstants.AWT.TOPLEVEL_WINDOW_PERMISSION)
				Catch se As SecurityException
					' make sure the privileged action is only
					' for getting the property! We don't want the
					' above checkPermission call to always succeed!
					warningString = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("awt.appletWarning", "Java Applet Window"))
				End Try
			End If
		End Sub

		''' <summary>
		''' Gets the {@code Locale} object that is associated
		''' with this window, if the locale has been set.
		''' If no locale has been set, then the default locale
		''' is returned. </summary>
		''' <returns>    the locale that is set for this window. </returns>
		''' <seealso cref=       java.util.Locale
		''' @since     JDK1.1 </seealso>
		Public Property Overrides locale As java.util.Locale
			Get
			  If Me.locale Is Nothing Then Return java.util.Locale.default
			  Return Me.locale
			End Get
		End Property

		''' <summary>
		''' Gets the input context for this window. A window always has an input context,
		''' which is shared by subcomponents unless they create and set their own. </summary>
		''' <seealso cref= Component#getInputContext
		''' @since 1.2 </seealso>
		Public Property Overrides inputContext As java.awt.im.InputContext
			Get
				SyncLock inputContextLock
					If inputContext Is Nothing Then inputContext = java.awt.im.InputContext.instance
				End SyncLock
				Return inputContext
			End Get
		End Property

		''' <summary>
		''' Set the cursor image to a specified cursor.
		''' <p>
		''' The method may have no visual effect if the Java platform
		''' implementation and/or the native system do not support
		''' changing the mouse cursor shape. </summary>
		''' <param name="cursor"> One of the constants defined
		'''            by the {@code Cursor} class. If this parameter is null
		'''            then the cursor for this window will be set to the type
		'''            Cursor.DEFAULT_CURSOR. </param>
		''' <seealso cref=       Component#getCursor </seealso>
		''' <seealso cref=       Cursor
		''' @since     JDK1.1 </seealso>
		Public Overrides Property cursor As Cursor
			Set(ByVal cursor_Renamed As Cursor)
				If cursor_Renamed Is Nothing Then cursor_Renamed = Cursor.getPredefinedCursor(Cursor.DEFAULT_CURSOR)
				MyBase.cursor = cursor_Renamed
			End Set
		End Property

		''' <summary>
		''' Returns the owner of this window.
		''' @since 1.2
		''' </summary>
		Public Overridable Property owner As Window
			Get
				Return owner_NoClientCode
			End Get
		End Property
		Friend Property owner_NoClientCode As Window
			Get
				Return CType(parent, Window)
			End Get
		End Property

		''' <summary>
		''' Return an array containing all the windows this
		''' window currently owns.
		''' @since 1.2
		''' </summary>
		Public Overridable Property ownedWindows As Window()
			Get
				Return ownedWindows_NoClientCode
			End Get
		End Property
		Friend Property ownedWindows_NoClientCode As Window()
			Get
				Dim realCopy As Window()
    
				SyncLock ownedWindowList
					' Recall that ownedWindowList is actually a Vector of
					' WeakReferences and calling get() on one of these references
					' may return null. Make two arrays-- one the size of the
					' Vector (fullCopy with size fullSize), and one the size of
					' all non-null get()s (realCopy with size realSize).
					Dim fullSize As Integer = ownedWindowList.Count
					Dim realSize As Integer = 0
					Dim fullCopy As Window() = New Window(fullSize - 1){}
    
					For i As Integer = 0 To fullSize - 1
						fullCopy(realSize) = ownedWindowList(i).get()
    
						If fullCopy(realSize) IsNot Nothing Then realSize += 1
					Next i
    
					If fullSize <> realSize Then
						realCopy = java.util.Arrays.copyOf(fullCopy, realSize)
					Else
						realCopy = fullCopy
					End If
				End SyncLock
    
				Return realCopy
			End Get
		End Property

		Friend Overridable Property modalBlocked As Boolean
			Get
				Return modalBlocker IsNot Nothing
			End Get
		End Property

		Friend Overridable Sub setModalBlocked(ByVal blocker As Dialog, ByVal blocked As Boolean, ByVal peerCall As Boolean)
			Me.modalBlocker = If(blocked, blocker, Nothing)
			If peerCall Then
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.modalBlockedked(blocker, blocked)
			End If
		End Sub

		Friend Overridable Property modalBlocker As Dialog
			Get
				Return modalBlocker
			End Get
		End Property

	'    
	'     * Returns a list of all displayable Windows, i. e. all the
	'     * Windows which peer is not null.
	'     *
	'     * @see #addNotify
	'     * @see #removeNotify
	'     
		Shared allWindows As sun.awt.util.IdentityArrayList(Of Window)
			Get
				SyncLock allWindows
					Dim v As New sun.awt.util.IdentityArrayList(Of Window)
					v.addAll(allWindows)
					Return v
				End SyncLock
			End Get
		End Property

		Shared allUnblockedWindows As sun.awt.util.IdentityArrayList(Of Window)
			Get
				SyncLock allWindows
					Dim unblocked As New sun.awt.util.IdentityArrayList(Of Window)
					For i As Integer = 0 To allWindows.size() - 1
						Dim w As Window = allWindows.get(i)
						If Not w.modalBlocked Then unblocked.add(w)
					Next i
					Return unblocked
				End SyncLock
			End Get
		End Property

		Private Shared Function getWindows(ByVal appContext As sun.awt.AppContext) As Window()
			SyncLock GetType(Window)
				Dim realCopy As Window()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim windowList As List(Of WeakReference(Of Window)) = CType(appContext.get(GetType(Window)), List(Of WeakReference(Of Window)))
				If windowList IsNot Nothing Then
					Dim fullSize As Integer = windowList.Count
					Dim realSize As Integer = 0
					Dim fullCopy As Window() = New Window(fullSize - 1){}
					For i As Integer = 0 To fullSize - 1
						Dim w As Window = windowList(i).get()
						If w IsNot Nothing Then
							fullCopy(realSize) = w
							realSize += 1
						End If
					Next i
					If fullSize <> realSize Then
						realCopy = java.util.Arrays.copyOf(fullCopy, realSize)
					Else
						realCopy = fullCopy
					End If
				Else
					realCopy = New Window(){}
				End If
				Return realCopy
			End SyncLock
		End Function

		''' <summary>
		''' Returns an array of all {@code Window}s, both owned and ownerless,
		''' created by this application.
		''' If called from an applet, the array includes only the {@code Window}s
		''' accessible by that applet.
		''' <p>
		''' <b>Warning:</b> this method may return system created windows, such
		''' as a print dialog. Applications should not assume the existence of
		''' these dialogs, nor should an application assume anything about these
		''' dialogs such as component positions, {@code LayoutManager}s
		''' or serialization.
		''' </summary>
		''' <seealso cref= Frame#getFrames </seealso>
		''' <seealso cref= Window#getOwnerlessWindows
		''' 
		''' @since 1.6 </seealso>
		Public Property Shared windows As Window()
			Get
				Return getWindows(sun.awt.AppContext.appContext)
			End Get
		End Property

		''' <summary>
		''' Returns an array of all {@code Window}s created by this application
		''' that have no owner. They include {@code Frame}s and ownerless
		''' {@code Dialog}s and {@code Window}s.
		''' If called from an applet, the array includes only the {@code Window}s
		''' accessible by that applet.
		''' <p>
		''' <b>Warning:</b> this method may return system created windows, such
		''' as a print dialog. Applications should not assume the existence of
		''' these dialogs, nor should an application assume anything about these
		''' dialogs such as component positions, {@code LayoutManager}s
		''' or serialization.
		''' </summary>
		''' <seealso cref= Frame#getFrames </seealso>
		''' <seealso cref= Window#getWindows()
		''' 
		''' @since 1.6 </seealso>
		Public Property Shared ownerlessWindows As Window()
			Get
				Dim allWindows_Renamed As Window() = Window.windows
    
				Dim ownerlessCount As Integer = 0
				For Each w As Window In allWindows_Renamed
					If w.owner Is Nothing Then ownerlessCount += 1
				Next w
    
				Dim ownerless As Window() = New Window(ownerlessCount - 1){}
				Dim c As Integer = 0
				For Each w As Window In allWindows_Renamed
					If w.owner Is Nothing Then
						ownerless(c) = w
						c += 1
					End If
				Next w
    
				Return ownerless
			End Get
		End Property

		Friend Overridable Property documentRoot As Window
			Get
				SyncLock treeLock
					Dim w As Window = Me
					Do While w.owner IsNot Nothing
						w = w.owner
					Loop
					Return w
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Specifies the modal exclusion type for this window. If a window is modal
		''' excluded, it is not blocked by some modal dialogs. See {@link
		''' java.awt.Dialog.ModalExclusionType Dialog.ModalExclusionType} for
		''' possible modal exclusion types.
		''' <p>
		''' If the given type is not supported, {@code NO_EXCLUDE} is used.
		''' <p>
		''' Note: changing the modal exclusion type for a visible window may have no
		''' effect until it is hidden and then shown again.
		''' </summary>
		''' <param name="exclusionType"> the modal exclusion type for this window; a {@code null}
		'''     value is equivalent to {@link Dialog.ModalExclusionType#NO_EXCLUDE
		'''     NO_EXCLUDE} </param>
		''' <exception cref="SecurityException"> if the calling thread does not have permission
		'''     to set the modal exclusion property to the window with the given
		'''     {@code exclusionType} </exception>
		''' <seealso cref= java.awt.Dialog.ModalExclusionType </seealso>
		''' <seealso cref= java.awt.Window#getModalExclusionType </seealso>
		''' <seealso cref= java.awt.Toolkit#isModalExclusionTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Property modalExclusionType As Dialog.ModalExclusionType
			Set(ByVal exclusionType As Dialog.ModalExclusionType)
				If exclusionType Is Nothing Then exclusionType = Dialog.ModalExclusionType.NO_EXCLUDE
				If Not Toolkit.defaultToolkit.isModalExclusionTypeSupported(exclusionType) Then exclusionType = Dialog.ModalExclusionType.NO_EXCLUDE
				If modalExclusionType = exclusionType Then Return
				If exclusionType = Dialog.ModalExclusionType.TOOLKIT_EXCLUDE Then
					Dim sm As SecurityManager = System.securityManager
					If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.AWT.TOOLKIT_MODALITY_PERMISSION)
				End If
				modalExclusionType = exclusionType
    
				' if we want on-fly changes, we need to uncomment the lines below
				'   and override the method in Dialog to use modalShow() instead
				'   of updateChildrenBlocking()
		' 
		'        if (isModalBlocked()) {
		'            modalBlocker.unblockWindow(this);
		'        }
		'        Dialog.checkShouldBeBlocked(this);
		'        updateChildrenBlocking();
		' 
			End Set
			Get
				Return modalExclusionType
			End Get
		End Property


		Friend Overridable Function isModalExcluded(ByVal exclusionType As Dialog.ModalExclusionType) As Boolean
			If (modalExclusionType IsNot Nothing) AndAlso modalExclusionType.CompareTo(exclusionType) >= 0 Then Return True
			Dim owner_Renamed As Window = owner_NoClientCode
			Return (owner_Renamed IsNot Nothing) AndAlso owner_Renamed.isModalExcluded(exclusionType)
		End Function

		Friend Overridable Sub updateChildrenBlocking()
			Dim childHierarchy As New List(Of Window)
			Dim ownedWindows_Renamed As Window() = ownedWindows
			For i As Integer = 0 To ownedWindows_Renamed.Length - 1
				childHierarchy.Add(ownedWindows_Renamed(i))
			Next i
			Dim k As Integer = 0
			Do While k < childHierarchy.Count
				Dim w As Window = childHierarchy(k)
				If w.visible Then
					If w.modalBlocked Then
						Dim blocker As Dialog = w.modalBlocker
						blocker.unblockWindow(w)
					End If
					Dialog.checkShouldBeBlocked(w)
					Dim wOwned As Window() = w.ownedWindows
					For j As Integer = 0 To wOwned.Length - 1
						childHierarchy.Add(wOwned(j))
					Next j
				End If
				k += 1
			Loop
		End Sub

		''' <summary>
		''' Adds the specified window listener to receive window events from
		''' this window.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window listener </param>
		''' <seealso cref= #removeWindowListener </seealso>
		''' <seealso cref= #getWindowListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addWindowListener(ByVal l As WindowListener)
			If l Is Nothing Then Return
			newEventsOnly = True
			windowListener = AWTEventMulticaster.add(windowListener, l)
		End Sub

		''' <summary>
		''' Adds the specified window state listener to receive window
		''' events from this window.  If {@code l} is {@code null},
		''' no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window state listener </param>
		''' <seealso cref= #removeWindowStateListener </seealso>
		''' <seealso cref= #getWindowStateListeners
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addWindowStateListener(ByVal l As WindowStateListener)
			If l Is Nothing Then Return
			windowStateListener = AWTEventMulticaster.add(windowStateListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Adds the specified window focus listener to receive window events
		''' from this window.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window focus listener </param>
		''' <seealso cref= #removeWindowFocusListener </seealso>
		''' <seealso cref= #getWindowFocusListeners
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addWindowFocusListener(ByVal l As WindowFocusListener)
			If l Is Nothing Then Return
			windowFocusListener = AWTEventMulticaster.add(windowFocusListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified window listener so that it no longer
		''' receives window events from this window.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window listener </param>
		''' <seealso cref= #addWindowListener </seealso>
		''' <seealso cref= #getWindowListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeWindowListener(ByVal l As WindowListener)
			If l Is Nothing Then Return
			windowListener = AWTEventMulticaster.remove(windowListener, l)
		End Sub

		''' <summary>
		''' Removes the specified window state listener so that it no
		''' longer receives window events from this window.  If
		''' {@code l} is {@code null}, no exception is thrown and
		''' no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window state listener </param>
		''' <seealso cref= #addWindowStateListener </seealso>
		''' <seealso cref= #getWindowStateListeners
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeWindowStateListener(ByVal l As WindowStateListener)
			If l Is Nothing Then Return
			windowStateListener = AWTEventMulticaster.remove(windowStateListener, l)
		End Sub

		''' <summary>
		''' Removes the specified window focus listener so that it no longer
		''' receives window events from this window.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the window focus listener </param>
		''' <seealso cref= #addWindowFocusListener </seealso>
		''' <seealso cref= #getWindowFocusListeners
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeWindowFocusListener(ByVal l As WindowFocusListener)
			If l Is Nothing Then Return
			windowFocusListener = AWTEventMulticaster.remove(windowFocusListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the window listeners
		''' registered on this window.
		''' </summary>
		''' <returns> all of this window's {@code WindowListener}s
		'''         or an empty array if no window
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref= #addWindowListener </seealso>
		''' <seealso cref= #removeWindowListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property windowListeners As WindowListener()
			Get
				Return getListeners(GetType(WindowListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the window focus listeners
		''' registered on this window.
		''' </summary>
		''' <returns> all of this window's {@code WindowFocusListener}s
		'''         or an empty array if no window focus
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref= #addWindowFocusListener </seealso>
		''' <seealso cref= #removeWindowFocusListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property windowFocusListeners As WindowFocusListener()
			Get
				Return getListeners(GetType(WindowFocusListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the window state listeners
		''' registered on this window.
		''' </summary>
		''' <returns> all of this window's {@code WindowStateListener}s
		'''         or an empty array if no window state
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref= #addWindowStateListener </seealso>
		''' <seealso cref= #removeWindowStateListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property windowStateListeners As WindowStateListener()
			Get
				Return getListeners(GetType(WindowStateListener))
			End Get
		End Property


		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this {@code Window}.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' 
		''' You can specify the {@code listenerType} argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' {@code Window} {@code w}
		''' for its window listeners with the following code:
		''' 
		''' <pre>WindowListener[] wls = (WindowListener[])(w.getListeners(WindowListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          {@code java.util.EventListener} </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this window,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if {@code listenerType}
		'''          doesn't specify a class or interface that implements
		'''          {@code java.util.EventListener} </exception>
		''' <exception cref="NullPointerException"> if {@code listenerType} is {@code null}
		''' </exception>
		''' <seealso cref= #getWindowListeners
		''' @since 1.3 </seealso>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As [Class]) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(WindowFocusListener) Then
				l = windowFocusListener
			ElseIf listenerType Is GetType(WindowStateListener) Then
				l = windowStateListener
			ElseIf listenerType Is GetType(WindowListener) Then
				l = windowListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is handled at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			Select Case e.id
			  Case WindowEvent.WINDOW_OPENED, WindowEvent.WINDOW_CLOSING, WindowEvent.WINDOW_CLOSED, WindowEvent.WINDOW_ICONIFIED, WindowEvent.WINDOW_DEICONIFIED, WindowEvent.WINDOW_ACTIVATED, WindowEvent.WINDOW_DEACTIVATED
				If (eventMask And AWTEvent.WINDOW_EVENT_MASK) <> 0 OrElse windowListener IsNot Nothing Then Return True
				Return False
			  Case WindowEvent.WINDOW_GAINED_FOCUS, WindowEvent.WINDOW_LOST_FOCUS
				If (eventMask And AWTEvent.WINDOW_FOCUS_EVENT_MASK) <> 0 OrElse windowFocusListener IsNot Nothing Then Return True
				Return False
			  Case WindowEvent.WINDOW_STATE_CHANGED
				If (eventMask And AWTEvent.WINDOW_STATE_EVENT_MASK) <> 0 OrElse windowStateListener IsNot Nothing Then Return True
				Return False
			  Case Else
			End Select
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this window. If the event is an
		''' {@code WindowEvent}, it invokes the
		''' {@code processWindowEvent} method, else it invokes its
		''' superclass's {@code processEvent}.
		''' <p>Note that if the event parameter is {@code null}
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is WindowEvent Then
				Select Case e.iD
					Case WindowEvent.WINDOW_OPENED, WindowEvent.WINDOW_CLOSING, WindowEvent.WINDOW_CLOSED, WindowEvent.WINDOW_ICONIFIED, WindowEvent.WINDOW_DEICONIFIED, WindowEvent.WINDOW_ACTIVATED, WindowEvent.WINDOW_DEACTIVATED
						processWindowEvent(CType(e, WindowEvent))
					Case WindowEvent.WINDOW_GAINED_FOCUS, WindowEvent.WINDOW_LOST_FOCUS
						processWindowFocusEvent(CType(e, WindowEvent))
					Case WindowEvent.WINDOW_STATE_CHANGED
						processWindowStateEvent(CType(e, WindowEvent))
				End Select
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes window events occurring on this window by
		''' dispatching them to any registered WindowListener objects.
		''' NOTE: This method will not be called unless window events
		''' are enabled for this component; this happens when one of the
		''' following occurs:
		''' <ul>
		''' <li>A WindowListener object is registered via
		'''     {@code addWindowListener}
		''' <li>Window events are enabled via {@code enableEvents}
		''' </ul>
		''' <p>Note that if the event parameter is {@code null}
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the window event </param>
		''' <seealso cref= Component#enableEvents </seealso>
		Protected Friend Overridable Sub processWindowEvent(ByVal e As WindowEvent)
			Dim listener As WindowListener = windowListener
			If listener IsNot Nothing Then
				Select Case e.iD
					Case WindowEvent.WINDOW_OPENED
						listener.windowOpened(e)
					Case WindowEvent.WINDOW_CLOSING
						listener.windowClosing(e)
					Case WindowEvent.WINDOW_CLOSED
						listener.windowClosed(e)
					Case WindowEvent.WINDOW_ICONIFIED
						listener.windowIconified(e)
					Case WindowEvent.WINDOW_DEICONIFIED
						listener.windowDeiconified(e)
					Case WindowEvent.WINDOW_ACTIVATED
						listener.windowActivated(e)
					Case WindowEvent.WINDOW_DEACTIVATED
						listener.windowDeactivated(e)
					Case Else
				End Select
			End If
		End Sub

		''' <summary>
		''' Processes window focus event occurring on this window by
		''' dispatching them to any registered WindowFocusListener objects.
		''' NOTE: this method will not be called unless window focus events
		''' are enabled for this window. This happens when one of the
		''' following occurs:
		''' <ul>
		''' <li>a WindowFocusListener is registered via
		'''     {@code addWindowFocusListener}
		''' <li>Window focus events are enabled via {@code enableEvents}
		''' </ul>
		''' <p>Note that if the event parameter is {@code null}
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the window focus event </param>
		''' <seealso cref= Component#enableEvents
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub processWindowFocusEvent(ByVal e As WindowEvent)
			Dim listener As WindowFocusListener = windowFocusListener
			If listener IsNot Nothing Then
				Select Case e.iD
					Case WindowEvent.WINDOW_GAINED_FOCUS
						listener.windowGainedFocus(e)
					Case WindowEvent.WINDOW_LOST_FOCUS
						listener.windowLostFocus(e)
					Case Else
				End Select
			End If
		End Sub

		''' <summary>
		''' Processes window state event occurring on this window by
		''' dispatching them to any registered {@code WindowStateListener}
		''' objects.
		''' NOTE: this method will not be called unless window state events
		''' are enabled for this window.  This happens when one of the
		''' following occurs:
		''' <ul>
		''' <li>a {@code WindowStateListener} is registered via
		'''    {@code addWindowStateListener}
		''' <li>window state events are enabled via {@code enableEvents}
		''' </ul>
		''' <p>Note that if the event parameter is {@code null}
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the window state event </param>
		''' <seealso cref= java.awt.Component#enableEvents
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub processWindowStateEvent(ByVal e As WindowEvent)
			Dim listener As WindowStateListener = windowStateListener
			If listener IsNot Nothing Then
				Select Case e.iD
					Case WindowEvent.WINDOW_STATE_CHANGED
						listener.windowStateChanged(e)
					Case Else
				End Select
			End If
		End Sub

		''' <summary>
		''' Implements a debugging hook -- checks to see if
		''' the user has typed <i>control-shift-F1</i>.  If so,
		''' the list of child windows is dumped to {@code System.out}. </summary>
		''' <param name="e">  the keyboard event </param>
		Friend Overrides Sub preProcessKeyEvent(ByVal e As KeyEvent)
			' Dump the list of child windows to System.out.
			If e.actionKey AndAlso e.keyCode = KeyEvent.VK_F1 AndAlso e.controlDown AndAlso e.shiftDown AndAlso e.iD = KeyEvent.KEY_PRESSED Then list(System.out, 0)
		End Sub

		Friend Overrides Sub postProcessKeyEvent(ByVal e As KeyEvent)
			' Do nothing
		End Sub


		''' <summary>
		''' Sets whether this window should always be above other windows.  If
		''' there are multiple always-on-top windows, their relative order is
		''' unspecified and platform dependent.
		''' <p>
		''' If some other window is already always-on-top then the
		''' relative order between these windows is unspecified (depends on
		''' platform).  No window can be brought to be over the always-on-top
		''' window except maybe another always-on-top window.
		''' <p>
		''' All windows owned by an always-on-top window inherit this state and
		''' automatically become always-on-top.  If a window ceases to be
		''' always-on-top, the windows that it owns will no longer be
		''' always-on-top.  When an always-on-top window is sent {@link #toBack
		''' toBack}, its always-on-top state is set to {@code false}.
		''' 
		''' <p> When this method is called on a window with a value of
		''' {@code true}, and the window is visible and the platform
		''' supports always-on-top for this window, the window is immediately
		''' brought forward, "sticking" it in the top-most position. If the
		''' window isn`t currently visible, this method sets the always-on-top
		''' state to {@code true} but does not bring the window forward.
		''' When the window is later shown, it will be always-on-top.
		''' 
		''' <p> When this method is called on a window with a value of
		''' {@code false} the always-on-top state is set to normal. It may also
		''' cause an unspecified, platform-dependent change in the z-order of
		''' top-level windows, but other always-on-top windows will remain in
		''' top-most position. Calling this method with a value of {@code false}
		''' on a window that has a normal state has no effect.
		''' 
		''' <p><b>Note</b>: some platforms might not support always-on-top
		''' windows.  To detect if always-on-top windows are supported by the
		''' current platform, use <seealso cref="Toolkit#isAlwaysOnTopSupported()"/> and
		''' <seealso cref="Window#isAlwaysOnTopSupported()"/>.  If always-on-top mode
		''' isn't supported for this window or this window's toolkit does not
		''' support always-on-top windows, calling this method has no effect.
		''' <p>
		''' If a SecurityManager is installed, the calling thread must be
		''' granted the AWTPermission "setWindowAlwaysOnTop" in
		''' order to set the value of this property. If this
		''' permission is not granted, this method will throw a
		''' SecurityException, and the current value of the property will
		''' be left unchanged.
		''' </summary>
		''' <param name="alwaysOnTop"> true if the window should always be above other
		'''        windows </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		'''         permission to set the value of always-on-top property
		''' </exception>
		''' <seealso cref= #isAlwaysOnTop </seealso>
		''' <seealso cref= #toFront </seealso>
		''' <seealso cref= #toBack </seealso>
		''' <seealso cref= AWTPermission </seealso>
		''' <seealso cref= #isAlwaysOnTopSupported </seealso>
		''' <seealso cref= #getToolkit </seealso>
		''' <seealso cref= Toolkit#isAlwaysOnTopSupported
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Sub setAlwaysOnTop(ByVal alwaysOnTop As Boolean) 'JavaToDotNetTempPropertySetalwaysOnTop
		Public Property alwaysOnTop As Boolean
			Set(ByVal alwaysOnTop As Boolean)
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.SET_WINDOW_ALWAYS_ON_TOP_PERMISSION)
    
				Dim oldAlwaysOnTop As Boolean
				SyncLock Me
					oldAlwaysOnTop = Me.alwaysOnTop
					Me.alwaysOnTop = alwaysOnTop
				End SyncLock
				If oldAlwaysOnTop <> alwaysOnTop Then
					If alwaysOnTopSupported Then
						Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
						SyncLock treeLock
							If peer_Renamed IsNot Nothing Then peer_Renamed.updateAlwaysOnTopState()
						End SyncLock
					End If
					firePropertyChange("alwaysOnTop", oldAlwaysOnTop, alwaysOnTop)
				End If
				ownedWindowsAlwaysOnTop = alwaysOnTop
			End Set
			Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Property ownedWindowsAlwaysOnTop As Boolean
			Set(ByVal alwaysOnTop As Boolean)
				Dim ownedWindowArray As WeakReference(Of Window)()
				SyncLock ownedWindowList
					ownedWindowArray = New WeakReference(ownedWindowList.Count - 1){}
					ownedWindowList.CopyTo(ownedWindowArray)
				End SyncLock
    
				For Each ref As WeakReference(Of Window) In ownedWindowArray
					Dim window_Renamed As Window = ref.get()
					If window_Renamed IsNot Nothing Then
						Try
							window_Renamed.alwaysOnTop = alwaysOnTop
						Catch ignore As SecurityException
						End Try
					End If
				Next ref
			End Set
		End Property

		''' <summary>
		''' Returns whether the always-on-top mode is supported for this
		''' window. Some platforms may not support always-on-top windows, some
		''' may support only some kinds of top-level windows; for example,
		''' a platform may not support always-on-top modal dialogs.
		''' </summary>
		''' <returns> {@code true}, if the always-on-top mode is supported for
		'''         this window and this window's toolkit supports always-on-top windows,
		'''         {@code false} otherwise
		''' </returns>
		''' <seealso cref= #setAlwaysOnTop(boolean) </seealso>
		''' <seealso cref= #getToolkit </seealso>
		''' <seealso cref= Toolkit#isAlwaysOnTopSupported
		''' @since 1.6 </seealso>
		Public Overridable Property alwaysOnTopSupported As Boolean
			Get
				Return Toolkit.defaultToolkit.alwaysOnTopSupported
			End Get
		End Property


			Return alwaysOnTop
		End Function


		''' <summary>
		''' Returns the child Component of this Window that has focus if this Window
		''' is focused; returns null otherwise.
		''' </summary>
		''' <returns> the child Component with focus, or null if this Window is not
		'''         focused </returns>
		''' <seealso cref= #getMostRecentFocusOwner </seealso>
		''' <seealso cref= #isFocused </seealso>
		Public Overridable Property focusOwner As Component
			Get
				Return If(focused, KeyboardFocusManager.currentKeyboardFocusManager.focusOwner, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the child Component of this Window that will receive the focus
		''' when this Window is focused. If this Window is currently focused, this
		''' method returns the same Component as {@code getFocusOwner()}. If
		''' this Window is not focused, then the child Component that most recently
		''' requested focus will be returned. If no child Component has ever
		''' requested focus, and this is a focusable Window, then this Window's
		''' initial focusable Component is returned. If no child Component has ever
		''' requested focus, and this is a non-focusable Window, null is returned.
		''' </summary>
		''' <returns> the child Component that will receive focus when this Window is
		'''         focused </returns>
		''' <seealso cref= #getFocusOwner </seealso>
		''' <seealso cref= #isFocused </seealso>
		''' <seealso cref= #isFocusableWindow
		''' @since 1.4 </seealso>
		Public Overridable Property mostRecentFocusOwner As Component
			Get
				If focused Then
					Return focusOwner
				Else
					Dim mostRecent As Component = KeyboardFocusManager.getMostRecentFocusOwner(Me)
					If mostRecent IsNot Nothing Then
						Return mostRecent
					Else
						Return If(focusableWindow, focusTraversalPolicy.getInitialComponent(Me), Nothing)
					End If
				End If
			End Get
		End Property

		''' <summary>
		''' Returns whether this Window is active. Only a Frame or a Dialog may be
		''' active. The native windowing system may denote the active Window or its
		''' children with special decorations, such as a highlighted title bar. The
		''' active Window is always either the focused Window, or the first Frame or
		''' Dialog that is an owner of the focused Window.
		''' </summary>
		''' <returns> whether this is the active Window. </returns>
		''' <seealso cref= #isFocused
		''' @since 1.4 </seealso>
		Public Overridable Property active As Boolean
			Get
				Return (KeyboardFocusManager.currentKeyboardFocusManager.activeWindow Is Me)
			End Get
		End Property

		''' <summary>
		''' Returns whether this Window is focused. If there exists a focus owner,
		''' the focused Window is the Window that is, or contains, that focus owner.
		''' If there is no focus owner, then no Window is focused.
		''' <p>
		''' If the focused Window is a Frame or a Dialog it is also the active
		''' Window. Otherwise, the active Window is the first Frame or Dialog that
		''' is an owner of the focused Window.
		''' </summary>
		''' <returns> whether this is the focused Window. </returns>
		''' <seealso cref= #isActive
		''' @since 1.4 </seealso>
		Public Overridable Property focused As Boolean
			Get
				Return (KeyboardFocusManager.currentKeyboardFocusManager.globalFocusedWindow Is Me)
			End Get
		End Property

		''' <summary>
		''' Gets a focus traversal key for this Window. (See {@code
		''' setFocusTraversalKeys} for a full description of each key.)
		''' <p>
		''' If the traversal key has not been explicitly set for this Window,
		''' then this Window's parent's traversal key is returned. If the
		''' traversal key has not been explicitly set for any of this Window's
		''' ancestors, then the current KeyboardFocusManager's default traversal key
		''' is returned.
		''' </summary>
		''' <param name="id"> one of KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS </param>
		''' <returns> the AWTKeyStroke for the specified key </returns>
		''' <seealso cref= Container#setFocusTraversalKeys </seealso>
		''' <seealso cref= KeyboardFocusManager#FORWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#BACKWARD_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#UP_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <seealso cref= KeyboardFocusManager#DOWN_CYCLE_TRAVERSAL_KEYS </seealso>
		''' <exception cref="IllegalArgumentException"> if id is not one of
		'''         KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS,
		'''         KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS, or
		'''         KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS
		''' @since 1.4 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function getFocusTraversalKeys(ByVal id As Integer) As java.util.Set(Of AWTKeyStroke)
			If id < 0 OrElse id >= KeyboardFocusManager.TRAVERSAL_KEY_LENGTH Then Throw New IllegalArgumentException("invalid focus traversal key identifier")

			' Okay to return Set directly because it is an unmodifiable view
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim keystrokes As java.util.Set = If(focusTraversalKeys IsNot Nothing, focusTraversalKeys(id), Nothing)

			If keystrokes IsNot Nothing Then
				Return keystrokes
			Else
				Return KeyboardFocusManager.currentKeyboardFocusManager.getDefaultFocusTraversalKeys(id)
			End If
		End Function

		''' <summary>
		''' Does nothing because Windows must always be roots of a focus traversal
		''' cycle. The passed-in value is ignored.
		''' </summary>
		''' <param name="focusCycleRoot"> this value is ignored </param>
		''' <seealso cref= #isFocusCycleRoot </seealso>
		''' <seealso cref= Container#setFocusTraversalPolicy </seealso>
		''' <seealso cref= Container#getFocusTraversalPolicy
		''' @since 1.4 </seealso>
		Public NotOverridable Overrides Property focusCycleRoot As Boolean
			Set(ByVal focusCycleRoot As Boolean)
			End Set
			Get
				Return True
			End Get
		End Property


		''' <summary>
		''' Always returns {@code null} because Windows have no ancestors; they
		''' represent the top of the Component hierarchy.
		''' </summary>
		''' <returns> {@code null} </returns>
		''' <seealso cref= Container#isFocusCycleRoot()
		''' @since 1.4 </seealso>
		Public Property NotOverridable Overrides focusCycleRootAncestor As Container
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns whether this Window can become the focused Window, that is,
		''' whether this Window or any of its subcomponents can become the focus
		''' owner. For a Frame or Dialog to be focusable, its focusable Window state
		''' must be set to {@code true}. For a Window which is not a Frame or
		''' Dialog to be focusable, its focusable Window state must be set to
		''' {@code true}, its nearest owning Frame or Dialog must be
		''' showing on the screen, and it must contain at least one Component in
		''' its focus traversal cycle. If any of these conditions is not met, then
		''' neither this Window nor any of its subcomponents can become the focus
		''' owner.
		''' </summary>
		''' <returns> {@code true} if this Window can be the focused Window;
		'''         {@code false} otherwise </returns>
		''' <seealso cref= #getFocusableWindowState </seealso>
		''' <seealso cref= #setFocusableWindowState </seealso>
		''' <seealso cref= #isShowing </seealso>
		''' <seealso cref= Component#isFocusable
		''' @since 1.4 </seealso>
		Public Property focusableWindow As Boolean
			Get
				' If a Window/Frame/Dialog was made non-focusable, then it is always
				' non-focusable.
				If Not focusableWindowState Then Return False
    
				' All other tests apply only to Windows.
				If TypeOf Me Is Frame OrElse TypeOf Me Is Dialog Then Return True
    
				' A Window must have at least one Component in its root focus
				' traversal cycle to be focusable.
				If focusTraversalPolicy.getDefaultComponent(Me) Is Nothing Then Return False
    
				' A Window's nearest owning Frame or Dialog must be showing on the
				' screen.
				Dim owner_Renamed As Window = owner
				Do While owner_Renamed IsNot Nothing
					If TypeOf owner_Renamed Is Frame OrElse TypeOf owner_Renamed Is Dialog Then Return owner_Renamed.showing
					owner_Renamed = owner_Renamed.owner
				Loop
    
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns whether this Window can become the focused Window if it meets
		''' the other requirements outlined in {@code isFocusableWindow}. If
		''' this method returns {@code false}, then
		''' {@code isFocusableWindow} will return {@code false} as well.
		''' If this method returns {@code true}, then
		''' {@code isFocusableWindow} may return {@code true} or
		''' {@code false} depending upon the other requirements which must be
		''' met in order for a Window to be focusable.
		''' <p>
		''' By default, all Windows have a focusable Window state of
		''' {@code true}.
		''' </summary>
		''' <returns> whether this Window can be the focused Window </returns>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= #setFocusableWindowState </seealso>
		''' <seealso cref= #isShowing </seealso>
		''' <seealso cref= Component#setFocusable
		''' @since 1.4 </seealso>
		Public Overridable Property focusableWindowState As Boolean
			Get
				Return focusableWindowState
			End Get
			Set(ByVal focusableWindowState As Boolean)
				Dim oldFocusableWindowState As Boolean
				SyncLock Me
					oldFocusableWindowState = Me.focusableWindowState
					Me.focusableWindowState = focusableWindowState
				End SyncLock
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(Me.peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.updateFocusableWindowState()
				firePropertyChange("focusableWindowState", oldFocusableWindowState, focusableWindowState)
				If oldFocusableWindowState AndAlso (Not focusableWindowState) AndAlso focused Then
					Dim owner_Renamed As Window = owner
					Do While owner_Renamed IsNot Nothing
							Dim toFocus As Component = KeyboardFocusManager.getMostRecentFocusOwner(owner_Renamed)
							If toFocus IsNot Nothing AndAlso toFocus.requestFocus(False, sun.awt.CausedFocusEvent.Cause.ACTIVATION) Then Return
						owner_Renamed = owner_Renamed.owner
					Loop
					KeyboardFocusManager.currentKeyboardFocusManager.clearGlobalFocusOwnerPriv()
				End If
			End Set
		End Property


		''' <summary>
		''' Sets whether this window should receive focus on
		''' subsequently being shown (with a call to <seealso cref="#setVisible setVisible(true)"/>),
		''' or being moved to the front (with a call to <seealso cref="#toFront"/>).
		''' <p>
		''' Note that <seealso cref="#setVisible setVisible(true)"/> may be called indirectly
		''' (e.g. when showing an owner of the window makes the window to be shown).
		''' <seealso cref="#toFront"/> may also be called indirectly (e.g. when
		''' <seealso cref="#setVisible setVisible(true)"/> is called on already visible window).
		''' In all such cases this property takes effect as well.
		''' <p>
		''' The value of the property is not inherited by owned windows.
		''' </summary>
		''' <param name="autoRequestFocus"> whether this window should be focused on
		'''        subsequently being shown or being moved to the front </param>
		''' <seealso cref= #isAutoRequestFocus </seealso>
		''' <seealso cref= #isFocusableWindow </seealso>
		''' <seealso cref= #setVisible </seealso>
		''' <seealso cref= #toFront
		''' @since 1.7 </seealso>
		Public Overridable Property autoRequestFocus As Boolean
			Set(ByVal autoRequestFocus As Boolean)
				Me.autoRequestFocus = autoRequestFocus
			End Set
			Get
				Return autoRequestFocus
			End Get
		End Property


		''' <summary>
		''' Adds a PropertyChangeListener to the listener list. The listener is
		''' registered for all bound properties of this [Class], including the
		''' following:
		''' <ul>
		'''    <li>this Window's font ("font")</li>
		'''    <li>this Window's background color ("background")</li>
		'''    <li>this Window's foreground color ("foreground")</li>
		'''    <li>this Window's focusability ("focusable")</li>
		'''    <li>this Window's focus traversal keys enabled state
		'''        ("focusTraversalKeysEnabled")</li>
		'''    <li>this Window's Set of FORWARD_TRAVERSAL_KEYS
		'''        ("forwardFocusTraversalKeys")</li>
		'''    <li>this Window's Set of BACKWARD_TRAVERSAL_KEYS
		'''        ("backwardFocusTraversalKeys")</li>
		'''    <li>this Window's Set of UP_CYCLE_TRAVERSAL_KEYS
		'''        ("upCycleFocusTraversalKeys")</li>
		'''    <li>this Window's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		'''        ("downCycleFocusTraversalKeys")</li>
		'''    <li>this Window's focus traversal policy ("focusTraversalPolicy")
		'''        </li>
		'''    <li>this Window's focusable Window state ("focusableWindowState")
		'''        </li>
		'''    <li>this Window's always-on-top state("alwaysOnTop")</li>
		''' </ul>
		''' Note that if this Window is inheriting a bound property, then no
		''' event will be fired in response to a change in the inherited property.
		''' <p>
		''' If listener is null, no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="listener">  the PropertyChangeListener to be added
		''' </param>
		''' <seealso cref= Component#removePropertyChangeListener </seealso>
		''' <seealso cref= #addPropertyChangeListener(java.lang.String,java.beans.PropertyChangeListener) </seealso>
		Public Overrides Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			MyBase.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list for a specific
		''' property. The specified property may be user-defined, or one of the
		''' following:
		''' <ul>
		'''    <li>this Window's font ("font")</li>
		'''    <li>this Window's background color ("background")</li>
		'''    <li>this Window's foreground color ("foreground")</li>
		'''    <li>this Window's focusability ("focusable")</li>
		'''    <li>this Window's focus traversal keys enabled state
		'''        ("focusTraversalKeysEnabled")</li>
		'''    <li>this Window's Set of FORWARD_TRAVERSAL_KEYS
		'''        ("forwardFocusTraversalKeys")</li>
		'''    <li>this Window's Set of BACKWARD_TRAVERSAL_KEYS
		'''        ("backwardFocusTraversalKeys")</li>
		'''    <li>this Window's Set of UP_CYCLE_TRAVERSAL_KEYS
		'''        ("upCycleFocusTraversalKeys")</li>
		'''    <li>this Window's Set of DOWN_CYCLE_TRAVERSAL_KEYS
		'''        ("downCycleFocusTraversalKeys")</li>
		'''    <li>this Window's focus traversal policy ("focusTraversalPolicy")
		'''        </li>
		'''    <li>this Window's focusable Window state ("focusableWindowState")
		'''        </li>
		'''    <li>this Window's always-on-top state("alwaysOnTop")</li>
		''' </ul>
		''' Note that if this Window is inheriting a bound property, then no
		''' event will be fired in response to a change in the inherited property.
		''' <p>
		''' If listener is null, no exception is thrown and no action is performed.
		''' </summary>
		''' <param name="propertyName"> one of the property names listed above </param>
		''' <param name="listener"> the PropertyChangeListener to be added
		''' </param>
		''' <seealso cref= #addPropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
		''' <seealso cref= Component#removePropertyChangeListener </seealso>
		Public Overrides Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
			MyBase.addPropertyChangeListener(propertyName, listener)
		End Sub

		''' <summary>
		''' Indicates if this container is a validate root.
		''' <p>
		''' {@code Window} objects are the validate roots, and, therefore, they
		''' override this method to return {@code true}.
		''' </summary>
		''' <returns> {@code true}
		''' @since 1.7 </returns>
		''' <seealso cref= java.awt.Container#isValidateRoot </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Dispatches an event to this window or one of its sub components. </summary>
		''' <param name="e"> the event </param>
		Friend Overrides Sub dispatchEventImpl(ByVal e As AWTEvent)
			If e.iD = ComponentEvent.COMPONENT_RESIZED Then
				invalidate()
				validate()
			End If
			MyBase.dispatchEventImpl(e)
		End Sub

		''' @deprecated As of JDK version 1.1
		''' replaced by {@code dispatchEvent(AWTEvent)}. 
		<Obsolete("As of JDK version 1.1")> _
		Public Overrides Function postEvent(ByVal e As [Event]) As Boolean
			If handleEvent(e) Then
				e.consume()
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Checks if this Window is showing on screen. </summary>
		''' <seealso cref= Component#setVisible </seealso>
		Public Property Overrides showing As Boolean
			Get
				Return visible
			End Get
		End Property

		Friend Overridable Property disposing As Boolean
			Get
				Return disposing
			End Get
		End Property

		''' @deprecated As of J2SE 1.4, replaced by
		''' <seealso cref="Component#applyComponentOrientation Component.applyComponentOrientation"/>. 
		<Obsolete("As of J2SE 1.4, replaced by")> _
		Public Overridable Sub applyResourceBundle(ByVal rb As java.util.ResourceBundle)
			applyComponentOrientation(ComponentOrientation.getOrientation(rb))
		End Sub

		''' @deprecated As of J2SE 1.4, replaced by
		''' <seealso cref="Component#applyComponentOrientation Component.applyComponentOrientation"/>. 
		<Obsolete("As of J2SE 1.4, replaced by")> _
		Public Overridable Sub applyResourceBundle(ByVal rbName As String)
			applyResourceBundle(java.util.ResourceBundle.getBundle(rbName))
		End Sub

	'   
	'    * Support for tracking all windows owned by this window
	'    
		Friend Overridable Sub addOwnedWindow(ByVal weakWindow As WeakReference(Of Window))
			If weakWindow IsNot Nothing Then
				SyncLock ownedWindowList
					' this if statement should really be an assert, but we don't
					' have asserts...
					If Not ownedWindowList.Contains(weakWindow) Then ownedWindowList.Add(weakWindow)
				End SyncLock
			End If
		End Sub

		Friend Overridable Sub removeOwnedWindow(ByVal weakWindow As WeakReference(Of Window))
			If weakWindow IsNot Nothing Then ownedWindowList.Remove(weakWindow)
		End Sub

		Friend Overridable Sub connectOwnedWindow(ByVal child As Window)
			child.parent = Me
			addOwnedWindow(child.weakThis)
			child.disposerRecord.updateOwner()
		End Sub

		Private Sub addToWindowList()
			SyncLock GetType(Window)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim windowList As List(Of WeakReference(Of Window)) = CType(appContext.get(GetType(Window)), List(Of WeakReference(Of Window)))
				If windowList Is Nothing Then
					windowList = New List(Of WeakReference(Of Window))
					appContext.put(GetType(Window), windowList)
				End If
				windowList.Add(weakThis)
			End SyncLock
		End Sub

		Private Shared Sub removeFromWindowList(ByVal context As sun.awt.AppContext, ByVal weakThis As WeakReference(Of Window))
			SyncLock GetType(Window)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim windowList As List(Of WeakReference(Of Window)) = CType(context.get(GetType(Window)), List(Of WeakReference(Of Window)))
				If windowList IsNot Nothing Then windowList.Remove(weakThis)
			End SyncLock
		End Sub

		Private Sub removeFromWindowList()
			removeFromWindowList(appContext, weakThis)
		End Sub

		''' <summary>
		''' Window type.
		''' 
		''' Synchronization: ObjectLock
		''' </summary>
		Private type As Type = Type.NORMAL

		''' <summary>
		''' Sets the type of the window.
		''' 
		''' This method can only be called while the window is not displayable.
		''' </summary>
		''' <exception cref="IllegalComponentStateException"> if the window
		'''         is displayable. </exception>
		''' <exception cref="IllegalArgumentException"> if the type is {@code null} </exception>
		''' <seealso cref=    Component#isDisplayable </seealso>
		''' <seealso cref=    #getType
		''' @since 1.7 </seealso>
		Public Overridable Property type As Type
			Set(ByVal type As Type)
				If type Is Nothing Then Throw New IllegalArgumentException("type should not be null.")
				SyncLock treeLock
					If displayable Then Throw New IllegalComponentStateException("The window is displayable.")
					SyncLock objectLock
						Me.type = type
					End SyncLock
				End SyncLock
			End Set
			Get
				SyncLock objectLock
					Return type
				End SyncLock
			End Get
		End Property


		''' <summary>
		''' The window serialized data version.
		''' 
		''' @serial
		''' </summary>
		Private windowSerializedDataVersion As Integer = 2

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable {@code WindowListener}s and
		''' {@code WindowFocusListener}s as optional data.
		''' Writes a list of child windows as optional data.
		''' Writes a list of icon images as optional data
		''' </summary>
		''' <param name="s"> the {@code ObjectOutputStream} to write
		''' @serialData {@code null} terminated sequence of
		'''    0 or more pairs; the pair consists of a {@code String}
		'''    and {@code Object}; the {@code String}
		'''    indicates the type of object and is one of the following:
		'''    {@code windowListenerK} indicating a
		'''      {@code WindowListener} object;
		'''    {@code windowFocusWindowK} indicating a
		'''      {@code WindowFocusListener} object;
		'''    {@code ownedWindowK} indicating a child
		'''      {@code Window} object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(java.io.ObjectOutputStream, java.lang.String, java.util.EventListener) </seealso>
		''' <seealso cref= Component#windowListenerK </seealso>
		''' <seealso cref= Component#windowFocusListenerK </seealso>
		''' <seealso cref= Component#ownedWindowK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			SyncLock Me
				' Update old focusMgr fields so that our object stream can be read
				' by previous releases
				focusMgr = New FocusManager
				focusMgr.focusRoot = Me
				focusMgr.focusOwner = mostRecentFocusOwner

				s.defaultWriteObject()

				' Clear fields so that we don't keep extra references around
				focusMgr = Nothing

				AWTEventMulticaster.save(s, windowListenerK, windowListener)
				AWTEventMulticaster.save(s, windowFocusListenerK, windowFocusListener)
				AWTEventMulticaster.save(s, windowStateListenerK, windowStateListener)
			End SyncLock

			s.writeObject(Nothing)

			SyncLock ownedWindowList
				For i As Integer = 0 To ownedWindowList.Count - 1
					Dim child As Window = ownedWindowList(i).get()
					If child IsNot Nothing Then
						s.writeObject(ownedWindowK)
						s.writeObject(child)
					End If
				Next i
			End SyncLock
			s.writeObject(Nothing)

			'write icon array
			If icons IsNot Nothing Then
				For Each i As Image In icons
					If TypeOf i Is java.io.Serializable Then s.writeObject(i)
				Next i
			End If
			s.writeObject(Nothing)
		End Sub

		'
		' Part of deserialization procedure to be called before
		' user's code.
		'
		Private Sub initDeserializedWindow()
			warningStringing()
			inputContextLock = New Object

			' Deserialized Windows are not yet visible.
			visible = False

			weakThis = New WeakReference(Of )(Me)

			anchor = New Object
			disposerRecord = New WindowDisposerRecord(appContext, Me)
			sun.java2d.Disposer.addRecord(anchor, disposerRecord)

			addToWindowList()
			initGC(Nothing)
			ownedWindowList = New List(Of )
		End Sub

		Private Sub deserializeResources(ByVal s As java.io.ObjectInputStream)

				If windowSerializedDataVersion < 2 Then
					' Translate old-style focus tracking to new model. For 1.4 and
					' later releases, we'll rely on the Window's initial focusable
					' Component.
					If focusMgr IsNot Nothing Then
						If focusMgr.focusOwner IsNot Nothing Then KeyboardFocusManager.mostRecentFocusOwnerner(Me, focusMgr.focusOwner)
					End If

					' This field is non-transient and relies on default serialization.
					' However, the default value is insufficient, so we need to set
					' it explicitly for object data streams prior to 1.4.
					focusableWindowState = True


				End If

			Dim keyOrNull As Object
			keyOrNull = s.readObject()
			Do While Nothing IsNot keyOrNull
				Dim key As String = CStr(keyOrNull).intern()

				If windowListenerK = key Then
					addWindowListener(CType(s.readObject(), WindowListener))
				ElseIf windowFocusListenerK = key Then
					addWindowFocusListener(CType(s.readObject(), WindowFocusListener))
				ElseIf windowStateListenerK = key Then
					addWindowStateListener(CType(s.readObject(), WindowStateListener)) ' skip value for unrecognized key
				Else
					s.readObject()
				End If
				keyOrNull = s.readObject()
			Loop

			Try
				keyOrNull = s.readObject()
				Do While Nothing IsNot keyOrNull
					Dim key As String = CStr(keyOrNull).intern()

					If ownedWindowK = key Then
						connectOwnedWindow(CType(s.readObject(), Window))

					Else ' skip value for unrecognized key
						s.readObject()
					End If
					keyOrNull = s.readObject()
				Loop

				'read icons
				Dim obj As Object = s.readObject() 'Throws OptionalDataException
											 'for pre1.6 objects.
				icons = New List(Of Image) 'Frame.readObject() assumes
												'pre1.6 version if icons is null.
				Do While obj IsNot Nothing
					If TypeOf obj Is Image Then icons.Add(CType(obj, Image))
					obj = s.readObject()
				Loop
			Catch e As java.io.OptionalDataException
				' 1.1 serialized form
				' ownedWindowList will be updated by Frame.readObject
			End Try

		End Sub

		''' <summary>
		''' Reads the {@code ObjectInputStream} and an optional
		''' list of listeners to receive various events fired by
		''' the component; also reads a list of
		''' (possibly {@code null}) child windows.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the {@code ObjectInputStream} to read </param>
		''' <exception cref="HeadlessException"> if
		'''   {@code GraphicsEnvironment.isHeadless} returns
		'''   {@code true} </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			 GraphicsEnvironment.checkHeadless()
			 initDeserializedWindow()
			 Dim f As java.io.ObjectInputStream.GetField = s.readFields()

			 syncLWRequests = f.get("syncLWRequests", systemSyncLWRequests)
			 state = f.get("state", 0)
			 focusableWindowState = f.get("focusableWindowState", True)
			 windowSerializedDataVersion = f.get("windowSerializedDataVersion", 1)
			 locationByPlatform = f.get("locationByPlatform", locationByPlatformProp)
			 ' Note: 1.4 (or later) doesn't use focusMgr
			 focusMgr = CType(f.get("focusMgr", Nothing), FocusManager)
			 Dim et As Dialog.ModalExclusionType = CType(f.get("modalExclusionType", Dialog.ModalExclusionType.NO_EXCLUDE), Dialog.ModalExclusionType)
			 modalExclusionType = et ' since 6.0
			 Dim aot As Boolean = f.get("alwaysOnTop", False)
			 If aot Then alwaysOnTop = aot ' since 1.5; subject to permission check
			 shape = CType(f.get("shape", Nothing), Shape)
			 opacity = CSng(f.get("opacity", 1.0f))

			 Me.securityWarningWidth = 0
			 Me.securityWarningHeight = 0
			 Me.securityWarningPointX = 2.0
			 Me.securityWarningPointY = 0.0
			 Me.securityWarningAlignmentX = RIGHT_ALIGNMENT
			 Me.securityWarningAlignmentY = TOP_ALIGNMENT

			 deserializeResources(s)
		End Sub

	'    
	'     * --- Accessibility Support ---
	'     *
	'     

		''' <summary>
		''' Gets the AccessibleContext associated with this Window.
		''' For windows, the AccessibleContext takes the form of an
		''' AccessibleAWTWindow.
		''' A new AccessibleAWTWindow instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTWindow that serves as the
		'''         AccessibleContext of this Window
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTWindow(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' {@code Window} class.  It provides an implementation of the
		''' Java Accessibility API appropriate to window user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTWindow
			Inherits AccessibleAWTContainer

			Private ReadOnly outerInstance As Window

			Public Sub New(ByVal outerInstance As Window)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 4215068635060671780L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= javax.accessibility.AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.WINDOW
				End Get
			End Property

			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= javax.accessibility.AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.focusOwner IsNot Nothing Then states.add(AccessibleState.ACTIVE)
					Return states
				End Get
			End Property

		End Class ' inner class AccessibleAWTWindow

		Friend Overrides Property graphicsConfiguration As GraphicsConfiguration
			Set(ByVal gc As GraphicsConfiguration)
				If gc Is Nothing Then gc = GraphicsEnvironment.localGraphicsEnvironment.defaultScreenDevice.defaultConfiguration
				SyncLock treeLock
					MyBase.graphicsConfiguration = gc
					If log.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then log.finer("+ Window.setGraphicsConfiguration(): new GC is " & vbLf & "+ " & graphicsConfiguration_NoClientCode & vbLf & "+ this is " & Me)
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Sets the location of the window relative to the specified
		''' component according to the following scenarios.
		''' <p>
		''' The target screen mentioned below is a screen to which
		''' the window should be placed after the setLocationRelativeTo
		''' method is called.
		''' <ul>
		''' <li>If the component is {@code null}, or the {@code
		''' GraphicsConfiguration} associated with this component is
		''' {@code null}, the window is placed in the center of the
		''' screen. The center point can be obtained with the {@link
		''' GraphicsEnvironment#getCenterPoint
		''' GraphicsEnvironment.getCenterPoint} method.
		''' <li>If the component is not {@code null}, but it is not
		''' currently showing, the window is placed in the center of
		''' the target screen defined by the {@code
		''' GraphicsConfiguration} associated with this component.
		''' <li>If the component is not {@code null} and is shown on
		''' the screen, then the window is located in such a way that
		''' the center of the window coincides with the center of the
		''' component.
		''' </ul>
		''' <p>
		''' If the screens configuration does not allow the window to
		''' be moved from one screen to another, then the window is
		''' only placed at the location determined according to the
		''' above conditions and its {@code GraphicsConfiguration} is
		''' not changed.
		''' <p>
		''' <b>Note</b>: If the lower edge of the window is out of the screen,
		''' then the window is placed to the side of the {@code Component}
		''' that is closest to the center of the screen. So if the
		''' component is on the right part of the screen, the window
		''' is placed to its left, and vice versa.
		''' <p>
		''' If after the window location has been calculated, the upper,
		''' left, or right edge of the window is out of the screen,
		''' then the window is located in such a way that the upper,
		''' left, or right edge of the window coincides with the
		''' corresponding edge of the screen. If both left and right
		''' edges of the window are out of the screen, the window is
		''' placed at the left side of the screen. The similar placement
		''' will occur if both top and bottom edges are out of the screen.
		''' In that case, the window is placed at the top side of the screen.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <param name="c">  the component in relation to which the window's location
		'''           is determined </param>
		''' <seealso cref= java.awt.GraphicsEnvironment#getCenterPoint
		''' @since 1.4 </seealso>
		Public Overridable Property locationRelativeTo As Component
			Set(ByVal c As Component)
				' target location
				Dim dx As Integer = 0, dy As Integer = 0
				' target GC
				Dim gc As GraphicsConfiguration = graphicsConfiguration_NoClientCode
				Dim gcBounds As Rectangle = gc.bounds
    
				Dim windowSize As Dimension = size
    
				' search a top-level of c
				Dim componentWindow As Window = sun.awt.SunToolkit.getContainingWindow(c)
				If (c Is Nothing) OrElse (componentWindow Is Nothing) Then
					Dim ge As GraphicsEnvironment = GraphicsEnvironment.localGraphicsEnvironment
					gc = ge.defaultScreenDevice.defaultConfiguration
					gcBounds = gc.bounds
					Dim centerPoint As Point = ge.centerPoint
					dx = centerPoint.x - windowSize.width \ 2
					dy = centerPoint.y - windowSize.height \ 2
				ElseIf Not c.showing Then
					gc = componentWindow.graphicsConfiguration
					gcBounds = gc.bounds
					dx = gcBounds.x + (gcBounds.width - windowSize.width) \ 2
					dy = gcBounds.y + (gcBounds.height - windowSize.height) \ 2
				Else
					gc = componentWindow.graphicsConfiguration
					gcBounds = gc.bounds
					Dim compSize As Dimension = c.size
					Dim compLocation As Point = c.locationOnScreen
					dx = compLocation.x + ((compSize.width - windowSize.width) \ 2)
					dy = compLocation.y + ((compSize.height - windowSize.height) \ 2)
    
					' Adjust for bottom edge being offscreen
					If dy + windowSize.height > gcBounds.y + gcBounds.height Then
						dy = gcBounds.y + gcBounds.height - windowSize.height
						If compLocation.x - gcBounds.x + compSize.width \ 2 < gcBounds.width \ 2 Then
							dx = compLocation.x + compSize.width
						Else
							dx = compLocation.x - windowSize.width
						End If
					End If
				End If
    
				' Avoid being placed off the edge of the screen:
				' bottom
				If dy + windowSize.height > gcBounds.y + gcBounds.height Then dy = gcBounds.y + gcBounds.height - windowSize.height
				' top
				If dy < gcBounds.y Then dy = gcBounds.y
				' right
				If dx + windowSize.width > gcBounds.x + gcBounds.width Then dx = gcBounds.x + gcBounds.width - windowSize.width
				' left
				If dx < gcBounds.x Then dx = gcBounds.x
    
				locationion(dx, dy)
			End Set
		End Property

		''' <summary>
		''' Overridden from Component.  Top-level Windows should not propagate a
		''' MouseWheelEvent beyond themselves into their owning Windows.
		''' </summary>
		Friend Overridable Sub deliverMouseWheelToAncestor(ByVal e As MouseWheelEvent)
		End Sub

		''' <summary>
		''' Overridden from Component.  Top-level Windows don't dispatch to ancestors
		''' </summary>
		Friend Overrides Function dispatchMouseWheelToAncestor(ByVal e As MouseWheelEvent) As Boolean
			Return False
		End Function

		''' <summary>
		''' Creates a new strategy for multi-buffering on this component.
		''' Multi-buffering is useful for rendering performance.  This method
		''' attempts to create the best strategy available with the number of
		''' buffers supplied.  It will always create a {@code BufferStrategy}
		''' with that number of buffers.
		''' A page-flipping strategy is attempted first, then a blitting strategy
		''' using accelerated buffers.  Finally, an unaccelerated blitting
		''' strategy is used.
		''' <p>
		''' Each time this method is called,
		''' the existing buffer strategy for this component is discarded. </summary>
		''' <param name="numBuffers"> number of buffers to create </param>
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
		''' <param name="numBuffers"> number of buffers to create, including the front buffer </param>
		''' <param name="caps"> the required capabilities for creating the buffer strategy;
		''' cannot be {@code null} </param>
		''' <exception cref="AWTException"> if the capabilities supplied could not be
		''' supported or met; this may happen, for example, if there is not enough
		''' accelerated memory currently available, or if page flipping is specified
		''' but not possible. </exception>
		''' <exception cref="IllegalArgumentException"> if numBuffers is less than 1, or if
		''' caps is {@code null} </exception>
		''' <seealso cref= #getBufferStrategy
		''' @since 1.4 </seealso>
		Public Overrides Sub createBufferStrategy(ByVal numBuffers As Integer, ByVal caps As BufferCapabilities)
			MyBase.createBufferStrategy(numBuffers, caps)
		End Sub

		''' <summary>
		''' Returns the {@code BufferStrategy} used by this component.  This
		''' method will return null if a {@code BufferStrategy} has not yet
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

		Friend Overridable Property temporaryLostComponent As Component
			Get
				Return temporaryLostComponent
			End Get
		End Property
		Friend Overridable Function setTemporaryLostComponent(ByVal component_Renamed As Component) As Component
			Dim previousComp As Component = temporaryLostComponent
			' Check that "component" is an acceptable focus owner and don't store it otherwise
			' - or later we will have problems with opposite while handling  WINDOW_GAINED_FOCUS
			If component_Renamed Is Nothing OrElse component_Renamed.canBeFocusOwner() Then
				temporaryLostComponent = component_Renamed
			Else
				temporaryLostComponent = Nothing
			End If
			Return previousComp
		End Function

		''' <summary>
		''' Checks whether this window can contain focus owner.
		''' Verifies that it is focusable and as container it can container focus owner.
		''' @since 1.5
		''' </summary>
		Friend Overrides Function canContainFocusOwner(ByVal focusOwnerCandidate As Component) As Boolean
			Return MyBase.canContainFocusOwner(focusOwnerCandidate) AndAlso focusableWindow
		End Function

		Private locationByPlatform As Boolean = locationByPlatformProp


		''' <summary>
		''' Sets whether this Window should appear at the default location for the
		''' native windowing system or at the current location (returned by
		''' {@code getLocation}) the next time the Window is made visible.
		''' This behavior resembles a native window shown without programmatically
		''' setting its location.  Most windowing systems cascade windows if their
		''' locations are not explicitly set. The actual location is determined once the
		''' window is shown on the screen.
		''' <p>
		''' This behavior can also be enabled by setting the System Property
		''' "java.awt.Window.locationByPlatform" to "true", though calls to this method
		''' take precedence.
		''' <p>
		''' Calls to {@code setVisible}, {@code setLocation} and
		''' {@code setBounds} after calling {@code setLocationByPlatform} clear
		''' this property of the Window.
		''' <p>
		''' For example, after the following code is executed:
		''' <pre>
		''' setLocationByPlatform(true);
		''' setVisible(true);
		''' boolean flag = isLocationByPlatform();
		''' </pre>
		''' The window will be shown at platform's default location and
		''' {@code flag} will be {@code false}.
		''' <p>
		''' In the following sample:
		''' <pre>
		''' setLocationByPlatform(true);
		''' setLocation(10, 10);
		''' boolean flag = isLocationByPlatform();
		''' setVisible(true);
		''' </pre>
		''' The window will be shown at (10, 10) and {@code flag} will be
		''' {@code false}.
		''' </summary>
		''' <param name="locationByPlatform"> {@code true} if this Window should appear
		'''        at the default location, {@code false} if at the current location </param>
		''' <exception cref="IllegalComponentStateException"> if the window
		'''         is showing on screen and locationByPlatform is {@code true}. </exception>
		''' <seealso cref= #setLocation </seealso>
		''' <seealso cref= #isShowing </seealso>
		''' <seealso cref= #setVisible </seealso>
		''' <seealso cref= #isLocationByPlatform </seealso>
		''' <seealso cref= java.lang.System#getProperty(String)
		''' @since 1.5 </seealso>
		Public Overridable Property locationByPlatform As Boolean
			Set(ByVal locationByPlatform As Boolean)
				SyncLock treeLock
					If locationByPlatform AndAlso showing Then Throw New IllegalComponentStateException("The window is showing on screen.")
					Me.locationByPlatform = locationByPlatform
				End SyncLock
			End Set
			Get
				SyncLock treeLock
					Return locationByPlatform
				End SyncLock
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The {@code width} or {@code height} values
		''' are automatically enlarged if either is less than
		''' the minimum size as specified by previous call to
		''' {@code setMinimumSize}.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <seealso cref= #getBounds </seealso>
		''' <seealso cref= #setLocation(int, int) </seealso>
		''' <seealso cref= #setLocation(Point) </seealso>
		''' <seealso cref= #setSize(int, int) </seealso>
		''' <seealso cref= #setSize(Dimension) </seealso>
		''' <seealso cref= #setMinimumSize </seealso>
		''' <seealso cref= #setLocationByPlatform </seealso>
		''' <seealso cref= #isLocationByPlatform
		''' @since 1.6 </seealso>
		Public Overrides Sub setBounds(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			SyncLock treeLock
				If boundsOp = java.awt.peer.ComponentPeer.SET_LOCATION OrElse boundsOp = java.awt.peer.ComponentPeer.SET_BOUNDS Then locationByPlatform = False
				MyBase.boundsnds(x, y, width, height)
			End SyncLock
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The {@code r.width} or {@code r.height} values
		''' will be automatically enlarged if either is less than
		''' the minimum size as specified by previous call to
		''' {@code setMinimumSize}.
		''' <p>
		''' The method changes the geometry-related data. Therefore,
		''' the native windowing system may ignore such requests, or it may modify
		''' the requested data, so that the {@code Window} object is placed and sized
		''' in a way that corresponds closely to the desktop settings.
		''' </summary>
		''' <seealso cref= #getBounds </seealso>
		''' <seealso cref= #setLocation(int, int) </seealso>
		''' <seealso cref= #setLocation(Point) </seealso>
		''' <seealso cref= #setSize(int, int) </seealso>
		''' <seealso cref= #setSize(Dimension) </seealso>
		''' <seealso cref= #setMinimumSize </seealso>
		''' <seealso cref= #setLocationByPlatform </seealso>
		''' <seealso cref= #isLocationByPlatform
		''' @since 1.6 </seealso>
		Public Overrides Property bounds As Rectangle
			Set(ByVal r As Rectangle)
				boundsnds(r.x, r.y, r.width, r.height)
			End Set
		End Property

		''' <summary>
		''' Determines whether this component will be displayed on the screen. </summary>
		''' <returns> {@code true} if the component and all of its ancestors
		'''          until a toplevel window are visible, {@code false} otherwise </returns>
		Friend Property Overrides recursivelyVisible As Boolean
			Get
				' 5079694 fix: for a toplevel to be displayed, its parent doesn't have to be visible.
				' We're overriding isRecursivelyVisible to implement this policy.
				Return visible
			End Get
		End Property


		' ******************** SHAPES & TRANSPARENCY CODE ********************

		''' <summary>
		''' Returns the opacity of the window.
		''' </summary>
		''' <returns> the opacity of the window
		''' </returns>
		''' <seealso cref= Window#setOpacity(float) </seealso>
		''' <seealso cref= GraphicsDevice.WindowTranslucency
		''' 
		''' @since 1.7 </seealso>
		Public Overridable Property opacity As Single
			Get
				SyncLock treeLock
					Return opacity
				End SyncLock
			End Get
			Set(ByVal opacity As Single)
				SyncLock treeLock
					If opacity < 0.0f OrElse opacity > 1.0f Then Throw New IllegalArgumentException("The value of opacity should be in the range [0.0f .. 1.0f].")
					If opacity < 1.0f Then
						Dim gc As GraphicsConfiguration = graphicsConfiguration
						Dim gd As GraphicsDevice = gc.device
						If gc.device.fullScreenWindow Is Me Then Throw New IllegalComponentStateException("Setting opacity for full-screen window is not supported.")
						If Not gd.isWindowTranslucencySupported(GraphicsDevice.WindowTranslucency.TRANSLUCENT) Then Throw New UnsupportedOperationException("TRANSLUCENT translucency is not supported.")
					End If
					Me.opacity = opacity
					Dim peer_Renamed As java.awt.peer.WindowPeer = CType(peer, java.awt.peer.WindowPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.opacity = opacity
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Returns the shape of the window.
		''' 
		''' The value returned by this method may not be the same as
		''' previously set with {@code setShape(shape)}, but it is guaranteed
		''' to represent the same shape.
		''' </summary>
		''' <returns> the shape of the window or {@code null} if no
		'''     shape is specified for the window
		''' </returns>
		''' <seealso cref= Window#setShape(Shape) </seealso>
		''' <seealso cref= GraphicsDevice.WindowTranslucency
		''' 
		''' @since 1.7 </seealso>
		Public Overridable Property shape As Shape
			Get
				SyncLock treeLock
					Return If(shape Is Nothing, Nothing, New java.awt.geom.Path2D.Float(shape))
				End SyncLock
			End Get
			Set(ByVal shape As Shape)
				SyncLock treeLock
					If shape IsNot Nothing Then
						Dim gc As GraphicsConfiguration = graphicsConfiguration
						Dim gd As GraphicsDevice = gc.device
						If gc.device.fullScreenWindow Is Me Then Throw New IllegalComponentStateException("Setting shape for full-screen window is not supported.")
						If Not gd.isWindowTranslucencySupported(GraphicsDevice.WindowTranslucency.PERPIXEL_TRANSPARENT) Then Throw New UnsupportedOperationException("PERPIXEL_TRANSPARENT translucency is not supported.")
					End If
					Me.shape = If(shape Is Nothing, Nothing, New java.awt.geom.Path2D.Float(shape))
					Dim peer_Renamed As java.awt.peer.WindowPeer = CType(peer, java.awt.peer.WindowPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.applyShape(If(shape Is Nothing, Nothing, sun.java2d.pipe.Region.getInstance(shape, Nothing)))
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Gets the background color of this window.
		''' <p>
		''' Note that the alpha component of the returned color indicates whether
		''' the window is in the non-opaque (per-pixel translucent) mode.
		''' </summary>
		''' <returns> this component's background color
		''' </returns>
		''' <seealso cref= Window#setBackground(Color) </seealso>
		''' <seealso cref= Window#isOpaque </seealso>
		''' <seealso cref= GraphicsDevice.WindowTranslucency </seealso>
		Public Property Overrides background As Color
			Get
				Return MyBase.background
			End Get
			Set(ByVal bgColor As Color)
				Dim oldBg As Color = background
				MyBase.background = bgColor
				If oldBg IsNot Nothing AndAlso oldBg.Equals(bgColor) Then Return
				Dim oldAlpha As Integer = If(oldBg IsNot Nothing, oldBg.alpha, 255)
				Dim alpha As Integer = If(bgColor IsNot Nothing, bgColor.alpha, 255)
				If (oldAlpha = 255) AndAlso (alpha < 255) Then ' non-opaque window
					Dim gc As GraphicsConfiguration = graphicsConfiguration
					Dim gd As GraphicsDevice = gc.device
					If gc.device.fullScreenWindow Is Me Then Throw New IllegalComponentStateException("Making full-screen window non opaque is not supported.")
					If Not gc.translucencyCapable Then
						Dim capableGC As GraphicsConfiguration = gd.translucencyCapableGC
						If capableGC Is Nothing Then Throw New UnsupportedOperationException("PERPIXEL_TRANSLUCENT translucency is not supported")
						graphicsConfiguration = capableGC
					End If
					layersOpaqueque(Me, False)
				ElseIf (oldAlpha < 255) AndAlso (alpha = 255) Then
					layersOpaqueque(Me, True)
				End If
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.opaque = alpha = 255
			End Set
		End Property


		''' <summary>
		''' Indicates if the window is currently opaque.
		''' <p>
		''' The method returns {@code false} if the background color of the window
		''' is not {@code null} and the alpha component of the color is less than
		''' {@code 1.0f}. The method returns {@code true} otherwise.
		''' </summary>
		''' <returns> {@code true} if the window is opaque, {@code false} otherwise
		''' </returns>
		''' <seealso cref= Window#getBackground </seealso>
		''' <seealso cref= Window#setBackground(Color)
		''' @since 1.7 </seealso>
		Public Property Overrides opaque As Boolean
			Get
				Dim bg As Color = background
				Return If(bg IsNot Nothing, bg.alpha = 255, True)
			End Get
		End Property

		Private Sub updateWindow()
			SyncLock treeLock
				Dim peer_Renamed As java.awt.peer.WindowPeer = CType(peer, java.awt.peer.WindowPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.updateWindow()
			End SyncLock
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.7
		''' </summary>
		Public Overrides Sub paint(ByVal g As Graphics)
			If Not opaque Then
				Dim gg As Graphics = g.create()
				Try
					If TypeOf gg Is Graphics2D Then
						gg.color = background
						CType(gg, Graphics2D).composite = AlphaComposite.getInstance(AlphaComposite.SRC)
						gg.fillRect(0, 0, width, height)
					End If
				Finally
					gg.Dispose()
				End Try
			End If
			MyBase.paint(g)
		End Sub

		Private Shared Sub setLayersOpaque(ByVal component_Renamed As Component, ByVal isOpaque As Boolean)
			' Shouldn't use instanceof to avoid loading Swing classes
			'    if it's a pure AWT application.
			If sun.awt.SunToolkit.isInstanceOf(component_Renamed, "javax.swing.RootPaneContainer") Then
				Dim rpc As javax.swing.RootPaneContainer = CType(component_Renamed, javax.swing.RootPaneContainer)
				Dim root As javax.swing.JRootPane = rpc.rootPane
				Dim lp As javax.swing.JLayeredPane = root.layeredPane
				Dim c As Container = root.contentPane
				Dim content As javax.swing.JComponent = If(TypeOf c Is javax.swing.JComponent, CType(c, javax.swing.JComponent), Nothing)
				lp.opaque = isOpaque
				root.opaque = isOpaque
				If content IsNot Nothing Then
					content.opaque = isOpaque

					' Iterate down one level to see whether we have a JApplet
					' (which is also a RootPaneContainer) which requires processing
					Dim numChildren As Integer = content.componentCount
					If numChildren > 0 Then
						Dim child As Component = content.getComponent(0)
						' It's OK to use instanceof here because we've
						' already loaded the RootPaneContainer class by now
						If TypeOf child Is javax.swing.RootPaneContainer Then layersOpaqueque(child, isOpaque)
					End If
				End If
			End If
		End Sub


		' ************************** MIXING CODE *******************************

		' A window has an owner, but it does NOT have a container
		Friend Property NotOverridable Overrides container As Container
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Applies the shape to the component </summary>
		''' <param name="shape"> Shape to be applied to the component </param>
		Friend NotOverridable Overrides Sub applyCompoundShape(ByVal shape As sun.java2d.pipe.Region)
			' The shape calculated by mixing code is not intended to be applied
			' to windows or frames
		End Sub

		Friend NotOverridable Overrides Sub applyCurrentShape()
			' The shape calculated by mixing code is not intended to be applied
			' to windows or frames
		End Sub

		Friend NotOverridable Overrides Sub mixOnReshaping()
			' The shape calculated by mixing code is not intended to be applied
			' to windows or frames
		End Sub

		Friend Property NotOverridable Overrides locationOnWindow As Point
			Get
				Return New Point(0, 0)
			End Get
		End Property

		' ****************** END OF MIXING CODE ********************************

		''' <summary>
		''' Limit the given double value with the given range.
		''' </summary>
		Private Shared Function limit(ByVal value As Double, ByVal min As Double, ByVal max As Double) As Double
			value = Math.Max(value, min)
			value = Math.Min(value, max)
			Return value
		End Function

		''' <summary>
		''' Calculate the position of the security warning.
		''' 
		''' This method gets the window location/size as reported by the native
		''' system since the locally cached values may represent outdated data.
		''' 
		''' The method is used from the native code, or via AWTAccessor.
		''' 
		''' NOTE: this method is invoked on the toolkit thread, and therefore is not
		''' supposed to become public/user-overridable.
		''' </summary>
		Private Function calculateSecurityWarningPosition(ByVal x As Double, ByVal y As Double, ByVal w As Double, ByVal h As Double) As java.awt.geom.Point2D
			' The position according to the spec of SecurityWarning.setPosition()
			Dim wx As Double = x + w * securityWarningAlignmentX + securityWarningPointX
			Dim wy As Double = y + h * securityWarningAlignmentY + securityWarningPointY

			' First, make sure the warning is not too far from the window bounds
			wx = Window.limit(wx, x - securityWarningWidth - 2, x + w + 2)
			wy = Window.limit(wy, y - securityWarningHeight - 2, y + h + 2)

			' Now make sure the warning window is visible on the screen
			Dim graphicsConfig As GraphicsConfiguration = graphicsConfiguration_NoClientCode
			Dim screenBounds As Rectangle = graphicsConfig.bounds
			Dim screenInsets As Insets = Toolkit.defaultToolkit.getScreenInsets(graphicsConfig)

			wx = Window.limit(wx, screenBounds.x + screenInsets.left, screenBounds.x + screenBounds.width - screenInsets.right - securityWarningWidth)
			wy = Window.limit(wy, screenBounds.y + screenInsets.top, screenBounds.y + screenBounds.height - screenInsets.bottom - securityWarningHeight)

			Return New java.awt.geom.Point2D.Double(wx, wy)
		End Function


		' a window doesn't need to be updated in the Z-order.
		Friend Overrides Sub updateZOrder()
		End Sub

	End Class ' class Window


	''' <summary>
	''' This class is no longer used, but is maintained for Serialization
	''' backward-compatibility.
	''' </summary>
	<Serializable> _
	Friend Class FocusManager
		Friend focusRoot As Container
		Friend focusOwner As Component

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Friend Const serialVersionUID As Long = 2491878825643557906L
	End Class

End Namespace