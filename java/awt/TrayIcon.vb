Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>TrayIcon</code> object represents a tray icon that can be
	''' added to the <seealso cref="SystemTray system tray"/>. A
	''' <code>TrayIcon</code> can have a tooltip (text), an image, a popup
	''' menu, and a set of listeners associated with it.
	''' 
	''' <p>A <code>TrayIcon</code> can generate various {@link MouseEvent
	''' MouseEvents} and supports adding corresponding listeners to receive
	''' notification of these events.  <code>TrayIcon</code> processes some
	''' of the events by itself.  For example, by default, when the
	''' right-mouse click is performed on the <code>TrayIcon</code> it
	''' displays the specified popup menu.  When the mouse hovers
	''' over the <code>TrayIcon</code> the tooltip is displayed.
	''' 
	''' <p><strong>Note:</strong> When the <code>MouseEvent</code> is
	''' dispatched to its registered listeners its <code>component</code>
	''' property will be set to <code>null</code>.  (See {@link
	''' java.awt.event.ComponentEvent#getComponent}) The
	''' <code>source</code> property will be set to this
	''' <code>TrayIcon</code>. (See {@link
	''' java.util.EventObject#getSource})
	''' 
	''' <p><b>Note:</b> A well-behaved <seealso cref="TrayIcon"/> implementation
	''' will assign different gestures to showing a popup menu and
	''' selecting a tray icon.
	''' 
	''' <p>A <code>TrayIcon</code> can generate an {@link ActionEvent
	''' ActionEvent}.  On some platforms, this occurs when the user selects
	''' the tray icon using either the mouse or keyboard.
	''' 
	''' <p>If a SecurityManager is installed, the AWTPermission
	''' {@code accessSystemTray} must be granted in order to create
	''' a {@code TrayIcon}. Otherwise the constructor will throw a
	''' SecurityException.
	''' 
	''' <p> See the <seealso cref="SystemTray"/> class overview for an example on how
	''' to use the <code>TrayIcon</code> API.
	''' 
	''' @since 1.6 </summary>
	''' <seealso cref= SystemTray#add </seealso>
	''' <seealso cref= java.awt.event.ComponentEvent#getComponent </seealso>
	''' <seealso cref= java.util.EventObject#getSource
	''' 
	''' @author Bino George
	''' @author Denis Mikhalkin
	''' @author Sharon Zakhour
	''' @author Anton Tarasov </seealso>
	Public Class TrayIcon

		Private image_Renamed As Image
		Private tooltip As String
		Private popup As PopupMenu
		Private autosize As Boolean
		Private id As Integer
		Private actionCommand As String

		<NonSerialized> _
		Private peer As java.awt.peer.TrayIconPeer

		<NonSerialized> _
		Friend mouseListener As MouseListener
		<NonSerialized> _
		Friend mouseMotionListener As MouseMotionListener
		<NonSerialized> _
		Friend actionListener As ActionListener

	'    
	'     * The tray icon's AccessControlContext.
	'     *
	'     * Unlike the acc in Component, this field is made final
	'     * because TrayIcon is not serializable.
	'     
		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context

	'    
	'     * Returns the acc this tray icon was constructed with.
	'     
		Friend Property accessControlContext As java.security.AccessControlContext
			Get
				If acc Is Nothing Then Throw New SecurityException("TrayIcon is missing AccessControlContext")
				Return acc
			End Get
		End Property

		Shared Sub New()
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setTrayIconAccessor(New sun.awt.AWTAccessor.TrayIconAccessor()
	'		{
	'				public void addNotify(TrayIcon trayIcon) throws AWTException
	'				{
	'					trayIcon.addNotify();
	'				}
	'				public void removeNotify(TrayIcon trayIcon)
	'				{
	'					trayIcon.removeNotify();
	'				}
	'			});
		End Sub

		Private Sub New()
			SystemTray.checkSystemTrayAllowed()
			If GraphicsEnvironment.headless Then Throw New HeadlessException
			If Not SystemTray.supported Then Throw New UnsupportedOperationException
			sun.awt.SunToolkit.insertTargetMapping(Me, sun.awt.AppContext.appContext)
		End Sub

		''' <summary>
		''' Creates a <code>TrayIcon</code> with the specified image.
		''' </summary>
		''' <param name="image"> the <code>Image</code> to be used </param>
		''' <exception cref="IllegalArgumentException"> if <code>image</code> is
		''' <code>null</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if the system tray isn't
		''' supported by the current platform </exception>
		''' <exception cref="HeadlessException"> if
		''' {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException"> if {@code accessSystemTray} permission
		''' is not granted </exception>
		''' <seealso cref= SystemTray#add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image, String, PopupMenu) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= AWTPermission </seealso>
		Public Sub New(ByVal image_Renamed As Image)
			Me.New()
			If image_Renamed Is Nothing Then Throw New IllegalArgumentException("creating TrayIcon with null Image")
			image = image_Renamed
		End Sub

		''' <summary>
		''' Creates a <code>TrayIcon</code> with the specified image and
		''' tooltip text.
		''' </summary>
		''' <param name="image"> the <code>Image</code> to be used </param>
		''' <param name="tooltip"> the string to be used as tooltip text; if the
		''' value is <code>null</code> no tooltip is shown </param>
		''' <exception cref="IllegalArgumentException"> if <code>image</code> is
		''' <code>null</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if the system tray isn't
		''' supported by the current platform </exception>
		''' <exception cref="HeadlessException"> if
		''' {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException"> if {@code accessSystemTray} permission
		''' is not granted </exception>
		''' <seealso cref= SystemTray#add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image, String, PopupMenu) </seealso>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= AWTPermission </seealso>
		Public Sub New(ByVal image_Renamed As Image, ByVal tooltip As String)
			Me.New(image_Renamed)
			toolTip = tooltip
		End Sub

		''' <summary>
		''' Creates a <code>TrayIcon</code> with the specified image,
		''' tooltip and popup menu.
		''' </summary>
		''' <param name="image"> the <code>Image</code> to be used </param>
		''' <param name="tooltip"> the string to be used as tooltip text; if the
		''' value is <code>null</code> no tooltip is shown </param>
		''' <param name="popup"> the menu to be used for the tray icon's popup
		''' menu; if the value is <code>null</code> no popup menu is shown </param>
		''' <exception cref="IllegalArgumentException"> if <code>image</code> is <code>null</code> </exception>
		''' <exception cref="UnsupportedOperationException"> if the system tray isn't
		''' supported by the current platform </exception>
		''' <exception cref="HeadlessException"> if
		''' {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException"> if {@code accessSystemTray} permission
		''' is not granted </exception>
		''' <seealso cref= SystemTray#add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image) </seealso>
		''' <seealso cref= PopupMenu </seealso>
		''' <seealso cref= MouseListener </seealso>
		''' <seealso cref= #addMouseListener(MouseListener) </seealso>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= AWTPermission </seealso>
		Public Sub New(ByVal image_Renamed As Image, ByVal tooltip As String, ByVal popup As PopupMenu)
			Me.New(image_Renamed, tooltip)
			popupMenu = popup
		End Sub

		''' <summary>
		''' Sets the image for this <code>TrayIcon</code>.  The previous
		''' tray icon image is discarded without calling the {@link
		''' java.awt.Image#flush} method &#151; you will need to call it
		''' manually.
		''' 
		''' <p> If the image represents an animated image, it will be
		''' animated automatically.
		''' 
		''' <p> See the <seealso cref="#setImageAutoSize(boolean)"/> property for
		''' details on the size of the displayed image.
		''' 
		''' <p> Calling this method with the same image that is currently
		''' being used has no effect.
		''' </summary>
		''' <exception cref="NullPointerException"> if <code>image</code> is <code>null</code> </exception>
		''' <param name="image"> the non-null <code>Image</code> to be used </param>
		''' <seealso cref= #getImage </seealso>
		''' <seealso cref= Image </seealso>
		''' <seealso cref= SystemTray#add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon#TrayIcon(Image, String) </seealso>
		Public Overridable Property image As Image
			Set(ByVal image_Renamed As Image)
				If image_Renamed Is Nothing Then Throw New NullPointerException("setting null Image")
				Me.image_Renamed = image_Renamed
    
				Dim peer As java.awt.peer.TrayIconPeer = Me.peer
				If peer IsNot Nothing Then peer.updateImage()
			End Set
			Get
				Return image_Renamed
			End Get
		End Property


		''' <summary>
		''' Sets the popup menu for this <code>TrayIcon</code>.  If
		''' <code>popup</code> is <code>null</code>, no popup menu will be
		''' associated with this <code>TrayIcon</code>.
		''' 
		''' <p>Note that this <code>popup</code> must not be added to any
		''' parent before or after it is set on the tray icon.  If you add
		''' it to some parent, the <code>popup</code> may be removed from
		''' that parent.
		''' 
		''' <p>The {@code popup} can be set on one {@code TrayIcon} only.
		''' Setting the same popup on multiple {@code TrayIcon}s will cause
		''' an {@code IllegalArgumentException}.
		''' 
		''' <p><strong>Note:</strong> Some platforms may not support
		''' showing the user-specified popup menu component when the user
		''' right-clicks the tray icon.  In this situation, either no menu
		''' will be displayed or, on some systems, a native version of the
		''' menu may be displayed.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if the {@code popup} is already
		''' set for another {@code TrayIcon} </exception>
		''' <param name="popup"> a <code>PopupMenu</code> or <code>null</code> to
		''' remove any popup menu </param>
		''' <seealso cref= #getPopupMenu </seealso>
		Public Overridable Property popupMenu As PopupMenu
			Set(ByVal popup As PopupMenu)
				If popup Is Me.popup Then Return
				SyncLock GetType(TrayIcon)
					If popup IsNot Nothing Then
						If popup.isTrayIconPopup Then Throw New IllegalArgumentException("the PopupMenu is already set for another TrayIcon")
						popup.isTrayIconPopup = True
					End If
					If Me.popup IsNot Nothing Then Me.popup.isTrayIconPopup = False
					Me.popup = popup
				End SyncLock
			End Set
			Get
				Return popup
			End Get
		End Property


		''' <summary>
		''' Sets the tooltip string for this <code>TrayIcon</code>. The
		''' tooltip is displayed automatically when the mouse hovers over
		''' the icon.  Setting the tooltip to <code>null</code> removes any
		''' tooltip text.
		''' 
		''' When displayed, the tooltip string may be truncated on some platforms;
		''' the number of characters that may be displayed is platform-dependent.
		''' </summary>
		''' <param name="tooltip"> the string for the tooltip; if the value is
		''' <code>null</code> no tooltip is shown </param>
		''' <seealso cref= #getToolTip </seealso>
		Public Overridable Property toolTip As String
			Set(ByVal tooltip As String)
				Me.tooltip = tooltip
    
				Dim peer As java.awt.peer.TrayIconPeer = Me.peer
				If peer IsNot Nothing Then peer.toolTip = tooltip
			End Set
			Get
				Return tooltip
			End Get
		End Property


		''' <summary>
		''' Sets the auto-size property.  Auto-size determines whether the
		''' tray image is automatically sized to fit the space allocated
		''' for the image on the tray.  By default, the auto-size property
		''' is set to <code>false</code>.
		''' 
		''' <p> If auto-size is <code>false</code>, and the image size
		''' doesn't match the tray icon space, the image is painted as-is
		''' inside that space &#151; if larger than the allocated space, it will
		''' be cropped.
		''' 
		''' <p> If auto-size is <code>true</code>, the image is stretched or shrunk to
		''' fit the tray icon space.
		''' </summary>
		''' <param name="autosize"> <code>true</code> to auto-size the image,
		''' <code>false</code> otherwise </param>
		''' <seealso cref= #isImageAutoSize </seealso>
		Public Overridable Property imageAutoSize As Boolean
			Set(ByVal autosize As Boolean)
				Me.autosize = autosize
    
				Dim peer As java.awt.peer.TrayIconPeer = Me.peer
				If peer IsNot Nothing Then peer.updateImage()
			End Set
			Get
				Return autosize
			End Get
		End Property


		''' <summary>
		''' Adds the specified mouse listener to receive mouse events from
		''' this <code>TrayIcon</code>.  Calling this method with a
		''' <code>null</code> value has no effect.
		''' 
		''' <p><b>Note</b>: The {@code MouseEvent}'s coordinates (received
		''' from the {@code TrayIcon}) are relative to the screen, not the
		''' {@code TrayIcon}.
		''' 
		''' <p> <b>Note: </b>The <code>MOUSE_ENTERED</code> and
		''' <code>MOUSE_EXITED</code> mouse events are not supported.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener"> the mouse listener </param>
		''' <seealso cref=      java.awt.event.MouseEvent </seealso>
		''' <seealso cref=      java.awt.event.MouseListener </seealso>
		''' <seealso cref=      #removeMouseListener(MouseListener) </seealso>
		''' <seealso cref=      #getMouseListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addMouseListener(ByVal listener As MouseListener)
			If listener Is Nothing Then Return
			mouseListener = AWTEventMulticaster.add(mouseListener, listener)
		End Sub

		''' <summary>
		''' Removes the specified mouse listener.  Calling this method with
		''' <code>null</code> or an invalid value has no effect.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener">   the mouse listener </param>
		''' <seealso cref=      java.awt.event.MouseEvent </seealso>
		''' <seealso cref=      java.awt.event.MouseListener </seealso>
		''' <seealso cref=      #addMouseListener(MouseListener) </seealso>
		''' <seealso cref=      #getMouseListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeMouseListener(ByVal listener As MouseListener)
			If listener Is Nothing Then Return
			mouseListener = AWTEventMulticaster.remove(mouseListener, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the mouse listeners
		''' registered on this <code>TrayIcon</code>.
		''' </summary>
		''' <returns> all of the <code>MouseListeners</code> registered on
		''' this <code>TrayIcon</code> or an empty array if no mouse
		''' listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addMouseListener(MouseListener) </seealso>
		''' <seealso cref=      #removeMouseListener(MouseListener) </seealso>
		''' <seealso cref=      java.awt.event.MouseListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mouseListeners As MouseListener()
			Get
				Return AWTEventMulticaster.getListeners(mouseListener, GetType(MouseListener))
			End Get
		End Property

		''' <summary>
		''' Adds the specified mouse listener to receive mouse-motion
		''' events from this <code>TrayIcon</code>.  Calling this method
		''' with a <code>null</code> value has no effect.
		''' 
		''' <p><b>Note</b>: The {@code MouseEvent}'s coordinates (received
		''' from the {@code TrayIcon}) are relative to the screen, not the
		''' {@code TrayIcon}.
		''' 
		''' <p> <b>Note: </b>The <code>MOUSE_DRAGGED</code> mouse event is not supported.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener">   the mouse listener </param>
		''' <seealso cref=      java.awt.event.MouseEvent </seealso>
		''' <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		''' <seealso cref=      #removeMouseMotionListener(MouseMotionListener) </seealso>
		''' <seealso cref=      #getMouseMotionListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addMouseMotionListener(ByVal listener As MouseMotionListener)
			If listener Is Nothing Then Return
			mouseMotionListener = AWTEventMulticaster.add(mouseMotionListener, listener)
		End Sub

		''' <summary>
		''' Removes the specified mouse-motion listener.  Calling this method with
		''' <code>null</code> or an invalid value has no effect.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener">   the mouse listener </param>
		''' <seealso cref=      java.awt.event.MouseEvent </seealso>
		''' <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		''' <seealso cref=      #addMouseMotionListener(MouseMotionListener) </seealso>
		''' <seealso cref=      #getMouseMotionListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeMouseMotionListener(ByVal listener As MouseMotionListener)
			If listener Is Nothing Then Return
			mouseMotionListener = AWTEventMulticaster.remove(mouseMotionListener, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the mouse-motion listeners
		''' registered on this <code>TrayIcon</code>.
		''' </summary>
		''' <returns> all of the <code>MouseInputListeners</code> registered on
		''' this <code>TrayIcon</code> or an empty array if no mouse
		''' listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addMouseMotionListener(MouseMotionListener) </seealso>
		''' <seealso cref=      #removeMouseMotionListener(MouseMotionListener) </seealso>
		''' <seealso cref=      java.awt.event.MouseMotionListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mouseMotionListeners As MouseMotionListener()
			Get
				Return AWTEventMulticaster.getListeners(mouseMotionListener, GetType(MouseMotionListener))
			End Get
		End Property

		''' <summary>
		''' Returns the command name of the action event fired by this tray icon.
		''' </summary>
		''' <returns> the action command name, or <code>null</code> if none exists </returns>
		''' <seealso cref= #addActionListener(ActionListener) </seealso>
		''' <seealso cref= #setActionCommand(String) </seealso>
		Public Overridable Property actionCommand As String
			Get
				Return actionCommand
			End Get
			Set(ByVal command As String)
				actionCommand = command
			End Set
		End Property


		''' <summary>
		''' Adds the specified action listener to receive
		''' <code>ActionEvent</code>s from this <code>TrayIcon</code>.
		''' Action events usually occur when a user selects the tray icon,
		''' using either the mouse or keyboard.  The conditions in which
		''' action events are generated are platform-dependent.
		''' 
		''' <p>Calling this method with a <code>null</code> value has no
		''' effect.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener"> the action listener </param>
		''' <seealso cref=           #removeActionListener </seealso>
		''' <seealso cref=           #getActionListeners </seealso>
		''' <seealso cref=           java.awt.event.ActionListener </seealso>
		''' <seealso cref= #setActionCommand(String) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addActionListener(ByVal listener As ActionListener)
			If listener Is Nothing Then Return
			actionListener = AWTEventMulticaster.add(actionListener, listener)
		End Sub

		''' <summary>
		''' Removes the specified action listener.  Calling this method with
		''' <code>null</code> or an invalid value has no effect.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="listener">   the action listener </param>
		''' <seealso cref=      java.awt.event.ActionEvent </seealso>
		''' <seealso cref=      java.awt.event.ActionListener </seealso>
		''' <seealso cref=      #addActionListener(ActionListener) </seealso>
		''' <seealso cref=      #getActionListeners </seealso>
		''' <seealso cref= #setActionCommand(String) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeActionListener(ByVal listener As ActionListener)
			If listener Is Nothing Then Return
			actionListener = AWTEventMulticaster.remove(actionListener, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this <code>TrayIcon</code>.
		''' </summary>
		''' <returns> all of the <code>ActionListeners</code> registered on
		''' this <code>TrayIcon</code> or an empty array if no action
		''' listeners are currently registered
		''' </returns>
		''' <seealso cref=      #addActionListener(ActionListener) </seealso>
		''' <seealso cref=      #removeActionListener(ActionListener) </seealso>
		''' <seealso cref=      java.awt.event.ActionListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return AWTEventMulticaster.getListeners(actionListener, GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' The message type determines which icon will be displayed in the
		''' caption of the message, and a possible system sound a message
		''' may generate upon showing.
		''' </summary>
		''' <seealso cref= TrayIcon </seealso>
		''' <seealso cref= TrayIcon#displayMessage(String, String, MessageType)
		''' @since 1.6 </seealso>
		Public Enum MessageType
			''' <summary>
			''' An error message </summary>
			[ERROR]
			''' <summary>
			''' A warning message </summary>
			WARNING
			''' <summary>
			''' An information message </summary>
			INFO
			''' <summary>
			''' Simple message </summary>
			NONE
		End Enum

		''' <summary>
		''' Displays a popup message near the tray icon.  The message will
		''' disappear after a time or if the user clicks on it.  Clicking
		''' on the message may trigger an {@code ActionEvent}.
		''' 
		''' <p>Either the caption or the text may be <code>null</code>, but an
		''' <code>NullPointerException</code> is thrown if both are
		''' <code>null</code>.
		''' 
		''' When displayed, the caption or text strings may be truncated on
		''' some platforms; the number of characters that may be displayed is
		''' platform-dependent.
		''' 
		''' <p><strong>Note:</strong> Some platforms may not support
		''' showing a message.
		''' </summary>
		''' <param name="caption"> the caption displayed above the text, usually in
		''' bold; may be <code>null</code> </param>
		''' <param name="text"> the text displayed for the particular message; may be
		''' <code>null</code> </param>
		''' <param name="messageType"> an enum indicating the message type </param>
		''' <exception cref="NullPointerException"> if both <code>caption</code>
		''' and <code>text</code> are <code>null</code> </exception>
		Public Overridable Sub displayMessage(ByVal caption As String, ByVal text As String, ByVal messageType As MessageType)
			If caption Is Nothing AndAlso text Is Nothing Then Throw New NullPointerException("displaying the message with both caption and text being null")

			Dim peer As java.awt.peer.TrayIconPeer = Me.peer
			If peer IsNot Nothing Then peer.displayMessage(caption, text, messageType.name())
		End Sub

		''' <summary>
		''' Returns the size, in pixels, of the space that the tray icon
		''' occupies in the system tray.  For the tray icon that is not yet
		''' added to the system tray, the returned size is equal to the
		''' result of the <seealso cref="SystemTray#getTrayIconSize"/>.
		''' </summary>
		''' <returns> the size of the tray icon, in pixels </returns>
		''' <seealso cref= TrayIcon#setImageAutoSize(boolean) </seealso>
		''' <seealso cref= java.awt.Image </seealso>
		''' <seealso cref= TrayIcon#getSize() </seealso>
		Public Overridable Property size As Dimension
			Get
				Return SystemTray.systemTray.trayIconSize
			End Get
		End Property

		' ****************************************************************
		' ****************************************************************

		Friend Overridable Sub addNotify()
			SyncLock Me
				If peer Is Nothing Then
					Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
					If TypeOf toolkit_Renamed Is sun.awt.SunToolkit Then
						peer = CType(Toolkit.defaultToolkit, sun.awt.SunToolkit).createTrayIcon(Me)
					ElseIf TypeOf toolkit_Renamed Is sun.awt.HeadlessToolkit Then
						peer = CType(Toolkit.defaultToolkit, sun.awt.HeadlessToolkit).createTrayIcon(Me)
					End If
				End If
			End SyncLock
			peer.toolTip = tooltip
		End Sub

		Friend Overridable Sub removeNotify()
			Dim p As java.awt.peer.TrayIconPeer = Nothing
			SyncLock Me
				p = peer
				peer = Nothing
			End SyncLock
			If p IsNot Nothing Then p.Dispose()
		End Sub

		Friend Overridable Property iD As Integer
			Set(ByVal id As Integer)
				Me.id = id
			End Set
			Get
				Return id
			End Get
		End Property


		Friend Overridable Sub dispatchEvent(ByVal e As AWTEvent)
			EventQueue.currentEventAndMostRecentTime = e
			Toolkit.defaultToolkit.notifyAWTEventListeners(e)
			processEvent(e)
		End Sub

		Friend Overridable Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is MouseEvent Then
				Select Case e.iD
				Case MouseEvent.MOUSE_PRESSED, MouseEvent.MOUSE_RELEASED, MouseEvent.MOUSE_CLICKED
					processMouseEvent(CType(e, MouseEvent))
				Case MouseEvent.MOUSE_MOVED
					processMouseMotionEvent(CType(e, MouseEvent))
				Case Else
					Return
				End Select
			ElseIf TypeOf e Is ActionEvent Then
				processActionEvent(CType(e, ActionEvent))
			End If
		End Sub

		Friend Overridable Sub processMouseEvent(ByVal e As MouseEvent)
			Dim listener As MouseListener = mouseListener

			If listener IsNot Nothing Then
				Dim id_Renamed As Integer = e.iD
				Select Case id_Renamed
				Case MouseEvent.MOUSE_PRESSED
					listener.mousePressed(e)
				Case MouseEvent.MOUSE_RELEASED
					listener.mouseReleased(e)
				Case MouseEvent.MOUSE_CLICKED
					listener.mouseClicked(e)
				Case Else
					Return
				End Select
			End If
		End Sub

		Friend Overridable Sub processMouseMotionEvent(ByVal e As MouseEvent)
			Dim listener As MouseMotionListener = mouseMotionListener
			If listener IsNot Nothing AndAlso e.iD = MouseEvent.MOUSE_MOVED Then listener.mouseMoved(e)
		End Sub

		Friend Overridable Sub processActionEvent(ByVal e As ActionEvent)
			Dim listener As ActionListener = actionListener
			If listener IsNot Nothing Then listener.actionPerformed(e)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
	End Class

End Namespace