Imports System
Imports System.Runtime.CompilerServices
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
	''' All items in a menu must belong to the class
	''' <code>MenuItem</code>, or one of its subclasses.
	''' <p>
	''' The default <code>MenuItem</code> object embodies
	''' a simple labeled menu item.
	''' <p>
	''' This picture of a menu bar shows five menu items:
	''' <IMG SRC="doc-files/MenuBar-1.gif" alt="The following text describes this graphic."
	''' style="float:center; margin: 7px 10px;">
	''' <br style="clear:left;">
	''' The first two items are simple menu items, labeled
	''' <code>"Basic"</code> and <code>"Simple"</code>.
	''' Following these two items is a separator, which is itself
	''' a menu item, created with the label <code>"-"</code>.
	''' Next is an instance of <code>CheckboxMenuItem</code>
	''' labeled <code>"Check"</code>. The final menu item is a
	''' submenu labeled <code>"More&nbsp;Examples"</code>,
	''' and this submenu is an instance of <code>Menu</code>.
	''' <p>
	''' When a menu item is selected, AWT sends an action event to
	''' the menu item. Since the event is an
	''' instance of <code>ActionEvent</code>, the <code>processEvent</code>
	''' method examines the event and passes it along to
	''' <code>processActionEvent</code>. The latter method redirects the
	''' event to any <code>ActionListener</code> objects that have
	''' registered an interest in action events generated by this
	''' menu item.
	''' <P>
	''' Note that the subclass <code>Menu</code> overrides this behavior and
	''' does not send any event to the frame until one of its subitems is
	''' selected.
	''' 
	''' @author Sami Shaio
	''' </summary>
	Public Class MenuItem
		Inherits MenuComponent
		Implements Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setMenuItemAccessor(New sun.awt.AWTAccessor.MenuItemAccessor()
	'		{
	'				public boolean isEnabled(MenuItem item)
	'				{
	'					Return item.enabled;
	'				}
	'
	'				public String getLabel(MenuItem item)
	'				{
	'					Return item.label;
	'				}
	'
	'				public MenuShortcut getShortcut(MenuItem item)
	'				{
	'					Return item.shortcut;
	'				}
	'
	'				public String getActionCommandImpl(MenuItem item)
	'				{
	'					Return item.getActionCommandImpl();
	'				}
	'
	'				public boolean isItemEnabled(MenuItem item)
	'				{
	'					Return item.isItemEnabled();
	'				}
	'			});
		End Sub

		''' <summary>
		''' A value to indicate whether a menu item is enabled
		''' or not.  If it is enabled, <code>enabled</code> will
		''' be set to true.  Else <code>enabled</code> will
		''' be set to false.
		''' 
		''' @serial </summary>
		''' <seealso cref= #isEnabled() </seealso>
		''' <seealso cref= #setEnabled(boolean) </seealso>
		Friend enabled As Boolean = True

		''' <summary>
		''' <code>label</code> is the label of a menu item.
		''' It can be any string.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLabel() </seealso>
		''' <seealso cref= #setLabel(String) </seealso>
		Friend label_Renamed As String

		''' <summary>
		''' This field indicates the command tha has been issued
		''' by a  particular menu item.
		''' By default the <code>actionCommand</code>
		''' is the label of the menu item, unless it has been
		''' set using setActionCommand.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setActionCommand(String) </seealso>
		''' <seealso cref= #getActionCommand() </seealso>
		Friend actionCommand As String

		''' <summary>
		''' The eventMask is ONLY set by subclasses via enableEvents.
		''' The mask should NOT be set when listeners are registered
		''' so that we can distinguish the difference between when
		''' listeners request events and subclasses request them.
		''' 
		''' @serial
		''' </summary>
		Friend eventMask As Long

		<NonSerialized> _
		Friend actionListener As ActionListener

		''' <summary>
		''' A sequence of key stokes that ia associated with
		''' a menu item.
		''' Note :in 1.1.2 you must use setActionCommand()
		''' on a menu item in order for its shortcut to
		''' work.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getShortcut() </seealso>
		''' <seealso cref= #setShortcut(MenuShortcut) </seealso>
		''' <seealso cref= #deleteShortcut() </seealso>
		Private shortcut As MenuShortcut = Nothing

		Private Const base As String = "menuitem"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -21757335363267194L

		''' <summary>
		''' Constructs a new MenuItem with an empty label and no keyboard
		''' shortcut. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since    JDK1.1 </seealso>
		Public Sub New()
			Me.New("", Nothing)
		End Sub

		''' <summary>
		''' Constructs a new MenuItem with the specified label
		''' and no keyboard shortcut. Note that use of "-" in
		''' a label is reserved to indicate a separator between
		''' menu items. By default, all menu items except for
		''' separators are enabled. </summary>
		''' <param name="label"> the label for this menu item. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since       JDK1.0 </seealso>
		Public Sub New(  label_Renamed As String)
			Me.New(label_Renamed, Nothing)
		End Sub

		''' <summary>
		''' Create a menu item with an associated keyboard shortcut.
		''' Note that use of "-" in a label is reserved to indicate
		''' a separator between menu items. By default, all menu
		''' items except for separators are enabled. </summary>
		''' <param name="label"> the label for this menu item. </param>
		''' <param name="s"> the instance of <code>MenuShortcut</code>
		'''                       associated with this menu item. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since       JDK1.1 </seealso>
		Public Sub New(  label_Renamed As String,   s As MenuShortcut)
			Me.label_Renamed = label_Renamed
			Me.shortcut = s
		End Sub

		''' <summary>
		''' Construct a name for this MenuComponent.  Called by getName() when
		''' the name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(MenuItem)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the menu item's peer.  The peer allows us to modify the
		''' appearance of the menu item without changing its functionality.
		''' </summary>
		Public Overridable Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = Toolkit.defaultToolkit.createMenuItem(Me)
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the label for this menu item. </summary>
		''' <returns>  the label of this menu item, or <code>null</code>
		'''                   if this menu item has no label. </returns>
		''' <seealso cref=     java.awt.MenuItem#setLabel
		''' @since   JDK1.0 </seealso>
		Public Overridable Property label As String
			Get
				Return label_Renamed
			End Get
			Set(  label_Renamed As String)
				Me.label_Renamed = label_Renamed
				Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
			End Set
		End Property


		''' <summary>
		''' Checks whether this menu item is enabled. </summary>
		''' <seealso cref=        java.awt.MenuItem#setEnabled
		''' @since      JDK1.0 </seealso>
		Public Overridable Property enabled As Boolean
			Get
				Return enabled
			End Get
			Set(  b As Boolean)
				enable(b)
			End Set
		End Property


		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setEnabled(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub enable()
			enabled = True
			Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.enabled = True
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setEnabled(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Sub enable(  b As Boolean)
			If b Then
				enable()
			Else
				disable()
			End If
		End Sub

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>setEnabled(boolean)</code>. 
		<Obsolete("As of JDK version 1.1,"), MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disable()
			enabled = False
			Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.enabled = False
		End Sub

		''' <summary>
		''' Get the <code>MenuShortcut</code> object associated with this
		''' menu item, </summary>
		''' <returns>      the menu shortcut associated with this menu item,
		'''                   or <code>null</code> if none has been specified. </returns>
		''' <seealso cref=         java.awt.MenuItem#setShortcut
		''' @since       JDK1.1 </seealso>
		Public Overridable Property shortcut As MenuShortcut
			Get
				Return shortcut
			End Get
			Set(  s As MenuShortcut)
				shortcut = s
				Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
			End Set
		End Property


		''' <summary>
		''' Delete any <code>MenuShortcut</code> object associated
		''' with this menu item.
		''' @since      JDK1.1
		''' </summary>
		Public Overridable Sub deleteShortcut()
			shortcut = Nothing
			Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
			If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
		End Sub

	'    
	'     * Delete a matching MenuShortcut associated with this MenuItem.
	'     * Used when iterating Menus.
	'     
		Friend Overridable Sub deleteShortcut(  s As MenuShortcut)
			If s.Equals(shortcut) Then
				shortcut = Nothing
				Dim peer_Renamed As java.awt.peer.MenuItemPeer = CType(Me.peer, java.awt.peer.MenuItemPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
			End If
		End Sub

	'    
	'     * The main goal of this method is to post an appropriate event
	'     * to the event queue when menu shortcut is pressed. However,
	'     * in subclasses this method may do more than just posting
	'     * an event.
	'     
		Friend Overridable Sub doMenuEvent(  [when] As Long,   modifiers As Integer)
			Toolkit.eventQueue.postEvent(New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, actionCommand, [when], modifiers))
		End Sub

	'    
	'     * Returns true if the item and all its ancestors are
	'     * enabled, false otherwise
	'     
		Private Property itemEnabled As Boolean
			Get
				' Fix For 6185151: Menu shortcuts of all menuitems within a menu
				' should be disabled when the menu itself is disabled
				If Not enabled Then Return False
				Dim container_Renamed As MenuContainer = parent_NoClientCode
				Do
					If Not(TypeOf container_Renamed Is Menu) Then Return True
					Dim menu_Renamed As Menu = CType(container_Renamed, Menu)
					If Not menu_Renamed.enabled Then Return False
					container_Renamed = menu_Renamed.parent_NoClientCode
				Loop While container_Renamed IsNot Nothing
				Return True
			End Get
		End Property

	'    
	'     * Post an ActionEvent to the target (on
	'     * keydown) and the item is enabled.
	'     * Returns true if there is an associated shortcut.
	'     
		Friend Overridable Function handleShortcut(  e As KeyEvent) As Boolean
			Dim s As New MenuShortcut(e.keyCode, (e.modifiers And InputEvent.SHIFT_MASK) > 0)
			Dim sE As New MenuShortcut(e.extendedKeyCode, (e.modifiers And InputEvent.SHIFT_MASK) > 0)
			' Fix For 6185151: Menu shortcuts of all menuitems within a menu
			' should be disabled when the menu itself is disabled
			If (s.Equals(shortcut) OrElse sE.Equals(shortcut)) AndAlso itemEnabled Then
				' MenuShortcut match -- issue an event on keydown.
				If e.iD = KeyEvent.KEY_PRESSED Then
					doMenuEvent(e.when, e.modifiers)
				Else
					' silently eat key release.
				End If
				Return True
			End If
			Return False
		End Function

		Friend Overridable Function getShortcutMenuItem(  s As MenuShortcut) As MenuItem
			Return If(s.Equals(shortcut), Me, Nothing)
		End Function

		''' <summary>
		''' Enables event delivery to this menu item for events
		''' to be defined by the specified event mask parameter
		''' <p>
		''' Since event types are automatically enabled when a listener for
		''' that type is added to the menu item, this method only needs
		''' to be invoked by subclasses of <code>MenuItem</code> which desire to
		''' have the specified event types delivered to <code>processEvent</code>
		''' regardless of whether a listener is registered.
		''' </summary>
		''' <param name="eventsToEnable"> the event mask defining the event types </param>
		''' <seealso cref=         java.awt.MenuItem#processEvent </seealso>
		''' <seealso cref=         java.awt.MenuItem#disableEvents </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Sub enableEvents(  eventsToEnable As Long)
			eventMask = eventMask Or eventsToEnable
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Disables event delivery to this menu item for events
		''' defined by the specified event mask parameter.
		''' </summary>
		''' <param name="eventsToDisable"> the event mask defining the event types </param>
		''' <seealso cref=         java.awt.MenuItem#processEvent </seealso>
		''' <seealso cref=         java.awt.MenuItem#enableEvents </seealso>
		''' <seealso cref=         java.awt.Component#disableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Sub disableEvents(  eventsToDisable As Long)
			eventMask = eventMask And Not eventsToDisable
		End Sub

		''' <summary>
		''' Sets the command name of the action event that is fired
		''' by this menu item.
		''' <p>
		''' By default, the action command is set to the label of
		''' the menu item. </summary>
		''' <param name="command">   the action command to be set
		'''                                for this menu item. </param>
		''' <seealso cref=         java.awt.MenuItem#getActionCommand
		''' @since       JDK1.1 </seealso>
		Public Overridable Property actionCommand As String
			Set(  command As String)
				actionCommand = command
			End Set
			Get
				Return actionCommandImpl
			End Get
		End Property


		' This is final so it can be called on the Toolkit thread.
		Friend Property actionCommandImpl As String
			Get
				Return (If(actionCommand Is Nothing, label_Renamed, actionCommand))
			End Get
		End Property

		''' <summary>
		''' Adds the specified action listener to receive action events
		''' from this menu item.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the action listener. </param>
		''' <seealso cref=        #removeActionListener </seealso>
		''' <seealso cref=        #getActionListeners </seealso>
		''' <seealso cref=        java.awt.event.ActionEvent </seealso>
		''' <seealso cref=        java.awt.event.ActionListener
		''' @since      JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addActionListener(  l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.add(actionListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified action listener so it no longer receives
		''' action events from this menu item.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the action listener. </param>
		''' <seealso cref=        #addActionListener </seealso>
		''' <seealso cref=        #getActionListeners </seealso>
		''' <seealso cref=        java.awt.event.ActionEvent </seealso>
		''' <seealso cref=        java.awt.event.ActionListener
		''' @since      JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeActionListener(  l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.remove(actionListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this menu item.
		''' </summary>
		''' <returns> all of this menu item's <code>ActionListener</code>s
		'''         or an empty array if no action
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=        #addActionListener </seealso>
		''' <seealso cref=        #removeActionListener </seealso>
		''' <seealso cref=        java.awt.event.ActionEvent </seealso>
		''' <seealso cref=        java.awt.event.ActionListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Returns an array of all the objects currently registered
		''' as <code><em>Foo</em>Listener</code>s
		''' upon this <code>MenuItem</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>MenuItem</code> <code>m</code>
		''' for its action listeners with the following code:
		''' 
		''' <pre>ActionListener[] als = (ActionListener[])(m.getListeners(ActionListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this menu item,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getActionListeners
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(  listenerType As [Class]) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(ActionListener) Then l = actionListener
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		''' <summary>
		''' Processes events on this menu item. If the event is an
		''' instance of <code>ActionEvent</code>, it invokes
		''' <code>processActionEvent</code>, another method
		''' defined by <code>MenuItem</code>.
		''' <p>
		''' Currently, menu items only support action events.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=         java.awt.MenuItem#processActionEvent
		''' @since       JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(  e As AWTEvent)
			If TypeOf e Is ActionEvent Then processActionEvent(CType(e, ActionEvent))
		End Sub

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(  e As AWTEvent) As Boolean
			If e.id = ActionEvent.ACTION_PERFORMED Then
				If (eventMask And AWTEvent.ACTION_EVENT_MASK) <> 0 OrElse actionListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes action events occurring on this menu item,
		''' by dispatching them to any registered
		''' <code>ActionListener</code> objects.
		''' This method is not called unless action events are
		''' enabled for this component. Action events are enabled
		''' when one of the following occurs:
		''' <ul>
		''' <li>An <code>ActionListener</code> object is registered
		''' via <code>addActionListener</code>.
		''' <li>Action events are enabled via <code>enableEvents</code>.
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the action event </param>
		''' <seealso cref=         java.awt.event.ActionEvent </seealso>
		''' <seealso cref=         java.awt.event.ActionListener </seealso>
		''' <seealso cref=         java.awt.MenuItem#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processActionEvent(  e As ActionEvent)
			Dim listener As ActionListener = actionListener
			If listener IsNot Nothing Then listener.actionPerformed(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>MenuItem</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns> the parameter string of this menu item </returns>
		Public Overrides Function paramString() As String
			Dim str As String = ",label=" & label_Renamed
			If shortcut IsNot Nothing Then str &= ",shortcut=" & shortcut
			Return MyBase.paramString() + str
		End Function


	'     Serialization support.
	'     

		''' <summary>
		''' Menu item serialized data version.
		''' 
		''' @serial
		''' </summary>
		Private menuItemSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ActionListeners</code>
		''' as optional data. The non-serializable listeners are
		''' detected and no attempt is made to serialize them.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of 0
		'''   or more pairs; the pair consists of a <code>String</code>
		'''   and an <code>Object</code>; the <code>String</code>
		'''   indicates the type of object and is one of the following:
		'''   <code>actionListenerK</code> indicating an
		'''     <code>ActionListener</code> object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, actionListenerK, actionListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if it
		''' isn't <code>null</code> adds a listener to receive
		''' action events fired by the <code>Menu</code> Item.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= #removeActionListener(ActionListener) </seealso>
		''' <seealso cref= #addActionListener(ActionListener) </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(  s As java.io.ObjectInputStream)
		  ' HeadlessException will be thrown from MenuComponent's readObject
		  s.defaultReadObject()

		  Dim keyOrNull As Object
		  keyOrNull = s.readObject()
		  Do While Nothing IsNot keyOrNull
			Dim key As String = CStr(keyOrNull).intern()

			If actionListenerK = key Then
			  addActionListener(CType(s.readObject(), ActionListener))

			Else ' skip value for unrecognized key
			  s.readObject()
			End If
			  keyOrNull = s.readObject()
		  Loop
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this MenuItem.
		''' For menu items, the AccessibleContext takes the form of an
		''' AccessibleAWTMenuItem.
		''' A new AccessibleAWTMenuItem instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTMenuItem that serves as the
		'''         AccessibleContext of this MenuItem
		''' @since 1.3 </returns>
		Public  Overrides ReadOnly Property  accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTMenuItem(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' Inner class of MenuItem used to provide default support for
		''' accessibility.  This class is not meant to be used directly by
		''' application developers, but is instead meant only to be
		''' subclassed by menu component developers.
		''' <p>
		''' This class implements accessibility support for the
		''' <code>MenuItem</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu item user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTMenuItem
			Inherits AccessibleAWTMenuComponent
			Implements AccessibleAction, AccessibleValue

			Private ReadOnly outerInstance As MenuItem

			Public Sub New(  outerInstance As MenuItem)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -217847831945965825L

			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			''' object does not have a name </returns>
			Public Overridable Property accessibleName As String
				Get
					If accessibleName IsNot Nothing Then
						Return accessibleName
					Else
						If outerInstance.label Is Nothing Then
							Return MyBase.accessibleName
						Else
							Return outerInstance.label
						End If
					End If
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.MENU_ITEM
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleAction associated with this object.  In the
			''' implementation of the Java Accessibility API for this [Class],
			''' return this object, which is responsible for implementing the
			''' AccessibleAction interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleValue associated with this object.  In the
			''' implementation of the Java Accessibility API for this [Class],
			''' return this object, which is responsible for implementing the
			''' AccessibleValue interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the number of Actions available in this object.  The
			''' default behavior of a menu item is to have one action.
			''' </summary>
			''' <returns> 1, the number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Return a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			Public Overridable Function getAccessibleActionDescription(  i As Integer) As String
				If i = 0 Then
					' [[[PENDING:  WDW -- need to provide a localized string]]]
					Return "click"
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Perform the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			Public Overridable Function doAccessibleAction(  i As Integer) As Boolean
				If i = 0 Then
					' Simulate a button click
					Toolkit.eventQueue.postEvent(New ActionEvent(MenuItem.this, ActionEvent.ACTION_PERFORMED, outerInstance.actionCommand, EventQueue.mostRecentEventTime, 0))
					Return True
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Get the value of this object as a Number.
			''' </summary>
			''' <returns> An Integer of 0 if this isn't selected or an Integer of 1 if
			''' this is selected. </returns>
			''' <seealso cref= javax.swing.AbstractButton#isSelected() </seealso>
			Public Overridable Property currentAccessibleValue As Number
				Get
					Return Convert.ToInt32(0)
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> True if the value was set. </returns>
			Public Overridable Function setCurrentAccessibleValue(  n As Number) As Boolean
				Return False
			End Function

			''' <summary>
			''' Get the minimum value of this object as a Number.
			''' </summary>
			''' <returns> An Integer of 0. </returns>
			Public Overridable Property minimumAccessibleValue As Number
				Get
					Return Convert.ToInt32(0)
				End Get
			End Property

			''' <summary>
			''' Get the maximum value of this object as a Number.
			''' </summary>
			''' <returns> An Integer of 0. </returns>
			Public Overridable Property maximumAccessibleValue As Number
				Get
					Return Convert.ToInt32(0)
				End Get
			End Property

		End Class ' class AccessibleAWTMenuItem

	End Class

End Namespace