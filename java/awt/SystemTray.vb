Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>SystemTray</code> class represents the system tray for a
	''' desktop.  On Microsoft Windows it is referred to as the "Taskbar
	''' Status Area", on Gnome it is referred to as the "Notification
	''' Area", on KDE it is referred to as the "System Tray".  The system
	''' tray is shared by all applications running on the desktop.
	''' 
	''' <p> On some platforms the system tray may not be present or may not
	''' be supported, in this case <seealso cref="SystemTray#getSystemTray()"/>
	''' throws <seealso cref="UnsupportedOperationException"/>.  To detect whether the
	''' system tray is supported, use <seealso cref="SystemTray#isSupported"/>.
	''' 
	''' <p>The <code>SystemTray</code> may contain one or more {@link
	''' TrayIcon TrayIcons}, which are added to the tray using the {@link
	''' #add} method, and removed when no longer needed, using the
	''' <seealso cref="#remove"/>.  <code>TrayIcon</code> consists of an
	''' image, a popup menu and a set of associated listeners.  Please see
	''' the <seealso cref="TrayIcon"/> class for details.
	''' 
	''' <p>Every Java application has a single <code>SystemTray</code>
	''' instance that allows the app to interface with the system tray of
	''' the desktop while the app is running.  The <code>SystemTray</code>
	''' instance can be obtained from the <seealso cref="#getSystemTray"/> method.
	''' An application may not create its own instance of
	''' <code>SystemTray</code>.
	''' 
	''' <p>The following code snippet demonstrates how to access
	''' and customize the system tray:
	''' <pre>
	''' <code>
	'''     <seealso cref="TrayIcon"/> trayIcon = null;
	'''     if (SystemTray.isSupported()) {
	'''         // get the SystemTray instance
	'''         SystemTray tray = SystemTray.<seealso cref="#getSystemTray"/>;
	'''         // load an image
	'''         <seealso cref="java.awt.Image"/> image = <seealso cref="java.awt.Toolkit#getImage(String) Toolkit.getDefaultToolkit().getImage"/>(...);
	'''         // create a action listener to listen for default action executed on the tray icon
	'''         <seealso cref="java.awt.event.ActionListener"/> listener = new <seealso cref="java.awt.event.ActionListener ActionListener"/>() {
	'''             public  Sub  <seealso cref="java.awt.event.ActionListener#actionPerformed actionPerformed"/>(<seealso cref="java.awt.event.ActionEvent"/> e) {
	'''                 // execute default action of the application
	'''                 // ...
	'''             }
	'''         };
	'''         // create a popup menu
	'''         <seealso cref="java.awt.PopupMenu"/> popup = new <seealso cref="java.awt.PopupMenu#PopupMenu PopupMenu"/>();
	'''         // create menu item for the default action
	'''         MenuItem defaultItem = new MenuItem(...);
	'''         defaultItem.addActionListener(listener);
	'''         popup.add(defaultItem);
	'''         /// ... add other items
	'''         // construct a TrayIcon
	'''         trayIcon = new <seealso cref="TrayIcon#TrayIcon(java.awt.Image, String, java.awt.PopupMenu) TrayIcon"/>(image, "Tray Demo", popup);
	'''         // set the TrayIcon properties
	'''         trayIcon.<seealso cref="TrayIcon#addActionListener(java.awt.event.ActionListener) addActionListener"/>(listener);
	'''         // ...
	'''         // add the tray image
	'''         try {
	'''             tray.<seealso cref="SystemTray#add(TrayIcon) add"/>(trayIcon);
	'''         } catch (AWTException e) {
	'''             System.err.println(e);
	'''         }
	'''         // ...
	'''     } else {
	'''         // disable tray option in your application or
	'''         // perform other actions
	'''         ...
	'''     }
	'''     // ...
	'''     // some time later
	'''     // the application state has changed - update the image
	'''     if (trayIcon != null) {
	'''         trayIcon.<seealso cref="TrayIcon#setImage(java.awt.Image) setImage"/>(updatedImage);
	'''     }
	'''     // ...
	''' </code>
	''' </pre>
	''' 
	''' @since 1.6 </summary>
	''' <seealso cref= TrayIcon
	''' 
	''' @author Bino George
	''' @author Denis Mikhalkin
	''' @author Sharon Zakhour
	''' @author Anton Tarasov </seealso>
	Public Class SystemTray
		Private Shared systemTray_Renamed As SystemTray
		Private currentIconID As Integer = 0 ' each TrayIcon added gets a unique ID

		<NonSerialized> _
		Private peer As java.awt.peer.SystemTrayPeer

		Private Shared ReadOnly EMPTY_TRAY_ARRAY As TrayIcon() = New TrayIcon(){}

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setSystemTrayAccessor(New sun.awt.AWTAccessor.SystemTrayAccessor()
	'		{
	'				public  Sub  firePropertyChange(SystemTray tray, String propertyName, Object oldValue, Object newValue)
	'				{
	'					tray.firePropertyChange(propertyName, oldValue, newValue);
	'				}
	'			});
		End Sub

		''' <summary>
		''' Private <code>SystemTray</code> constructor.
		''' 
		''' </summary>
		Private Sub New()
			addNotify()
		End Sub

		''' <summary>
		''' Gets the <code>SystemTray</code> instance that represents the
		''' desktop's tray area.  This always returns the same instance per
		''' application.  On some platforms the system tray may not be
		''' supported.  You may use the <seealso cref="#isSupported"/> method to
		''' check if the system tray is supported.
		''' 
		''' <p>If a SecurityManager is installed, the AWTPermission
		''' {@code accessSystemTray} must be granted in order to get the
		''' {@code SystemTray} instance. Otherwise this method will throw a
		''' SecurityException.
		''' </summary>
		''' <returns> the <code>SystemTray</code> instance that represents
		''' the desktop's tray area </returns>
		''' <exception cref="UnsupportedOperationException"> if the system tray isn't
		''' supported by the current platform </exception>
		''' <exception cref="HeadlessException"> if
		''' <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if {@code accessSystemTray} permission
		''' is not granted </exception>
		''' <seealso cref= #add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon </seealso>
		''' <seealso cref= #isSupported </seealso>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= AWTPermission </seealso>
		PublicShared ReadOnly PropertysystemTray As SystemTray
			Get
				checkSystemTrayAllowed()
				If GraphicsEnvironment.headless Then Throw New HeadlessException
    
				initializeSystemTrayIfNeeded()
    
				If Not supported Then Throw New UnsupportedOperationException("The system tray is not supported on the current platform.")
    
				Return systemTray_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns whether the system tray is supported on the current
		''' platform.  In addition to displaying the tray icon, minimal
		''' system tray support includes either a popup menu (see {@link
		''' TrayIcon#setPopupMenu(PopupMenu)}) or an action event (see
		''' <seealso cref="TrayIcon#addActionListener(ActionListener)"/>).
		''' 
		''' <p>Developers should not assume that all of the system tray
		''' functionality is supported.  To guarantee that the tray icon's
		''' default action is always accessible, add the default action to
		''' both the action listener and the popup menu.  See the {@link
		''' SystemTray example} for an example of how to do this.
		''' 
		''' <p><b>Note</b>: When implementing <code>SystemTray</code> and
		''' <code>TrayIcon</code> it is <em>strongly recommended</em> that
		''' you assign different gestures to the popup menu and an action
		''' event.  Overloading a gesture for both purposes is confusing
		''' and may prevent the user from accessing one or the other.
		''' </summary>
		''' <seealso cref= #getSystemTray </seealso>
		''' <returns> <code>false</code> if no system tray access is supported; this
		''' method returns <code>true</code> if the minimal system tray access is
		''' supported but does not guarantee that all system tray
		''' functionality is supported for the current platform </returns>
		PublicShared ReadOnly Propertysupported As Boolean
			Get
				Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
				If TypeOf toolkit_Renamed Is sun.awt.SunToolkit Then
					' connecting tray to native resource
					initializeSystemTrayIfNeeded()
					Return CType(toolkit_Renamed, sun.awt.SunToolkit).traySupported
				ElseIf TypeOf toolkit_Renamed Is sun.awt.HeadlessToolkit Then
					' skip initialization as the init routine
					' throws HeadlessException
					Return CType(toolkit_Renamed, sun.awt.HeadlessToolkit).traySupported
				Else
					Return False
				End If
			End Get
		End Property

		''' <summary>
		''' Adds a <code>TrayIcon</code> to the <code>SystemTray</code>.
		''' The tray icon becomes visible in the system tray once it is
		''' added.  The order in which icons are displayed in a tray is not
		''' specified - it is platform and implementation-dependent.
		''' 
		''' <p> All icons added by the application are automatically
		''' removed from the <code>SystemTray</code> upon application exit
		''' and also when the desktop system tray becomes unavailable.
		''' </summary>
		''' <param name="trayIcon"> the <code>TrayIcon</code> to be added </param>
		''' <exception cref="NullPointerException"> if <code>trayIcon</code> is
		''' <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the same instance of
		''' a <code>TrayIcon</code> is added more than once </exception>
		''' <exception cref="AWTException"> if the desktop system tray is missing </exception>
		''' <seealso cref= #remove(TrayIcon) </seealso>
		''' <seealso cref= #getSystemTray </seealso>
		''' <seealso cref= TrayIcon </seealso>
		''' <seealso cref= java.awt.Image </seealso>
		Public Overridable Sub add(ByVal trayIcon_Renamed As TrayIcon)
			If trayIcon_Renamed Is Nothing Then Throw New NullPointerException("adding null TrayIcon")
			Dim oldArray As TrayIcon() = Nothing, newArray As TrayIcon() = Nothing
			Dim icons As List(Of TrayIcon) = Nothing
			SyncLock Me
				oldArray = systemTray_Renamed.trayIcons
				icons = CType(sun.awt.AppContext.appContext.get(GetType(TrayIcon)), List(Of TrayIcon))
				If icons Is Nothing Then
					icons = New List(Of TrayIcon)(3)
					sun.awt.AppContext.appContext.put(GetType(TrayIcon), icons)

				ElseIf icons.Contains(trayIcon_Renamed) Then
					Throw New IllegalArgumentException("adding TrayIcon that is already added")
				End If
				icons.Add(trayIcon_Renamed)
				newArray = systemTray_Renamed.trayIcons

				currentIconID += 1
				trayIcon_Renamed.iD = currentIconID
			End SyncLock
			Try
				trayIcon_Renamed.addNotify()
			Catch e As AWTException
				icons.Remove(trayIcon_Renamed)
				Throw e
			End Try
			firePropertyChange("trayIcons", oldArray, newArray)
		End Sub

		''' <summary>
		''' Removes the specified <code>TrayIcon</code> from the
		''' <code>SystemTray</code>.
		''' 
		''' <p> All icons added by the application are automatically
		''' removed from the <code>SystemTray</code> upon application exit
		''' and also when the desktop system tray becomes unavailable.
		''' 
		''' <p> If <code>trayIcon</code> is <code>null</code> or was not
		''' added to the system tray, no exception is thrown and no action
		''' is performed.
		''' </summary>
		''' <param name="trayIcon"> the <code>TrayIcon</code> to be removed </param>
		''' <seealso cref= #add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon </seealso>
		Public Overridable Sub remove(ByVal trayIcon_Renamed As TrayIcon)
			If trayIcon_Renamed Is Nothing Then Return
			Dim oldArray As TrayIcon() = Nothing, newArray As TrayIcon() = Nothing
			SyncLock Me
				oldArray = systemTray_Renamed.trayIcons
				Dim icons As List(Of TrayIcon) = CType(sun.awt.AppContext.appContext.get(GetType(TrayIcon)), List(Of TrayIcon))
				' TrayIcon with no peer is not contained in the array.
				If icons Is Nothing OrElse (Not icons.Remove(trayIcon_Renamed)) Then Return
				trayIcon_Renamed.removeNotify()
				newArray = systemTray_Renamed.trayIcons
			End SyncLock
			firePropertyChange("trayIcons", oldArray, newArray)
		End Sub

		''' <summary>
		''' Returns an array of all icons added to the tray by this
		''' application.  You can't access the icons added by another
		''' application.  Some browsers partition applets in different
		''' code bases into separate contexts, and establish walls between
		''' these contexts.  In such a scenario, only the tray icons added
		''' from this context will be returned.
		''' 
		''' <p> The returned array is a copy of the actual array and may be
		''' modified in any way without affecting the system tray.  To
		''' remove a <code>TrayIcon</code> from the
		''' <code>SystemTray</code>, use the {@link
		''' #remove(TrayIcon)} method.
		''' </summary>
		''' <returns> an array of all tray icons added to this tray, or an
		''' empty array if none has been added </returns>
		''' <seealso cref= #add(TrayIcon) </seealso>
		''' <seealso cref= TrayIcon </seealso>
		Public Overridable Property trayIcons As TrayIcon()
			Get
				Dim icons As List(Of TrayIcon) = CType(sun.awt.AppContext.appContext.get(GetType(TrayIcon)), List(Of TrayIcon))
				If icons IsNot Nothing Then Return CType(icons.ToArray(), TrayIcon())
				Return EMPTY_TRAY_ARRAY
			End Get
		End Property

		''' <summary>
		''' Returns the size, in pixels, of the space that a tray icon will
		''' occupy in the system tray.  Developers may use this methods to
		''' acquire the preferred size for the image property of a tray icon
		''' before it is created.  For convenience, there is a similar
		''' method <seealso cref="TrayIcon#getSize"/> in the <code>TrayIcon</code> class.
		''' </summary>
		''' <returns> the default size of a tray icon, in pixels </returns>
		''' <seealso cref= TrayIcon#setImageAutoSize(boolean) </seealso>
		''' <seealso cref= java.awt.Image </seealso>
		''' <seealso cref= TrayIcon#getSize() </seealso>
		Public Overridable Property trayIconSize As Dimension
			Get
				Return peer.trayIconSize
			End Get
		End Property

		''' <summary>
		''' Adds a {@code PropertyChangeListener} to the list of listeners for the
		''' specific property. The following properties are currently supported:
		''' 
		''' <table border=1 summary="SystemTray properties">
		''' <tr>
		'''    <th>Property</th>
		'''    <th>Description</th>
		''' </tr>
		''' <tr>
		'''    <td>{@code trayIcons}</td>
		'''    <td>The {@code SystemTray}'s array of {@code TrayIcon} objects.
		'''        The array is accessed via the <seealso cref="#getTrayIcons"/> method.<br>
		'''        This property is changed when a tray icon is added to (or removed
		'''        from) the system tray.<br> For example, this property is changed
		'''        when the system tray becomes unavailable on the desktop<br>
		'''        and the tray icons are automatically removed.</td>
		''' </tr>
		''' <tr>
		'''    <td>{@code systemTray}</td>
		'''    <td>This property contains {@code SystemTray} instance when the system tray
		'''        is available or <code>null</code> otherwise.<br> This property is changed
		'''        when the system tray becomes available or unavailable on the desktop.<br>
		'''        The property is accessed by the <seealso cref="#getSystemTray"/> method.</td>
		''' </tr>
		''' </table>
		''' <p>
		''' The {@code listener} listens to property changes only in this context.
		''' <p>
		''' If {@code listener} is {@code null}, no exception is thrown
		''' and no action is performed.
		''' </summary>
		''' <param name="propertyName"> the specified property </param>
		''' <param name="listener"> the property change listener to be added
		''' </param>
		''' <seealso cref= #removePropertyChangeListener </seealso>
		''' <seealso cref= #getPropertyChangeListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
			If listener Is Nothing Then Return
			currentChangeSupport.addPropertyChangeListener(propertyName, listener)
		End Sub

		''' <summary>
		''' Removes a {@code PropertyChangeListener} from the listener list
		''' for a specific property.
		''' <p>
		''' The {@code PropertyChangeListener} must be from this context.
		''' <p>
		''' If {@code propertyName} or {@code listener} is {@code null} or invalid,
		''' no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="propertyName"> the specified property </param>
		''' <param name="listener"> the PropertyChangeListener to be removed
		''' </param>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #getPropertyChangeListeners </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(ByVal propertyName As String, ByVal listener As java.beans.PropertyChangeListener)
			If listener Is Nothing Then Return
			currentChangeSupport.removePropertyChangeListener(propertyName, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the listeners that have been associated
		''' with the named property.
		''' <p>
		''' Only the listeners in this context are returned.
		''' </summary>
		''' <param name="propertyName"> the specified property </param>
		''' <returns> all of the {@code PropertyChangeListener}s associated with
		'''         the named property; if no such listeners have been added or
		'''         if {@code propertyName} is {@code null} or invalid, an empty
		'''         array is returned
		''' </returns>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #removePropertyChangeListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getPropertyChangeListeners(ByVal propertyName As String) As java.beans.PropertyChangeListener()
			Return currentChangeSupport.getPropertyChangeListeners(propertyName)
		End Function


		' ***************************************************************
		' ***************************************************************


		''' <summary>
		''' Support for reporting bound property changes for Object properties.
		''' This method can be called when a bound property has changed and it will
		''' send the appropriate PropertyChangeEvent to any registered
		''' PropertyChangeListeners.
		''' </summary>
		''' <param name="propertyName"> the property whose value has changed </param>
		''' <param name="oldValue"> the property's previous value </param>
		''' <param name="newValue"> the property's new value </param>
		Private Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If oldValue IsNot Nothing AndAlso newValue IsNot Nothing AndAlso oldValue.Equals(newValue) Then Return
			currentChangeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub

		''' <summary>
		''' Returns the current PropertyChangeSupport instance for the
		''' calling thread's context.
		''' </summary>
		''' <returns> this thread's context's PropertyChangeSupport </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property currentChangeSupport As java.beans.PropertyChangeSupport
			Get
				Dim changeSupport As java.beans.PropertyChangeSupport = CType(sun.awt.AppContext.appContext.get(GetType(SystemTray)), java.beans.PropertyChangeSupport)
    
				If changeSupport Is Nothing Then
					changeSupport = New java.beans.PropertyChangeSupport(Me)
					sun.awt.AppContext.appContext.put(GetType(SystemTray), changeSupport)
				End If
				Return changeSupport
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Sub addNotify()
			If peer Is Nothing Then
				Dim toolkit_Renamed As Toolkit = Toolkit.defaultToolkit
				If TypeOf toolkit_Renamed Is sun.awt.SunToolkit Then
					peer = CType(Toolkit.defaultToolkit, sun.awt.SunToolkit).createSystemTray(Me)
				ElseIf TypeOf toolkit_Renamed Is sun.awt.HeadlessToolkit Then
					peer = CType(Toolkit.defaultToolkit, sun.awt.HeadlessToolkit).createSystemTray(Me)
				End If
			End If
		End Sub

		Friend Shared Sub checkSystemTrayAllowed()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(sun.security.util.SecurityConstants.AWT.ACCESS_SYSTEM_TRAY_PERMISSION)
		End Sub

		Private Shared Sub initializeSystemTrayIfNeeded()
			SyncLock GetType(SystemTray)
				If systemTray_Renamed Is Nothing Then systemTray_Renamed = New SystemTray
			End SyncLock
		End Sub
	End Class

End Namespace