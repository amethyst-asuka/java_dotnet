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
	''' This class represents a check box that can be included in a menu.
	''' Selecting the check box in the menu changes its state from
	''' "on" to "off" or from "off" to "on."
	''' <p>
	''' The following picture depicts a menu which contains an instance
	''' of <code>CheckBoxMenuItem</code>:
	''' <p>
	''' <img src="doc-files/MenuBar-1.gif"
	''' alt="Menu labeled Examples, containing items Basic, Simple, Check, and More Examples. The Check item is a CheckBoxMenuItem instance, in the off state."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' The item labeled <code>Check</code> shows a check box menu item
	''' in its "off" state.
	''' <p>
	''' When a check box menu item is selected, AWT sends an item event to
	''' the item. Since the event is an instance of <code>ItemEvent</code>,
	''' the <code>processEvent</code> method examines the event and passes
	''' it along to <code>processItemEvent</code>. The latter method redirects
	''' the event to any <code>ItemListener</code> objects that have
	''' registered an interest in item events generated by this menu item.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.event.ItemEvent </seealso>
	''' <seealso cref=         java.awt.event.ItemListener
	''' @since       JDK1.0 </seealso>
	Public Class CheckboxMenuItem
		Inherits MenuItem
		Implements ItemSelectable, Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setCheckboxMenuItemAccessor(New sun.awt.AWTAccessor.CheckboxMenuItemAccessor()
	'		{
	'				public boolean getState(CheckboxMenuItem cmi)
	'				{
	'					Return cmi.state;
	'				}
	'			});
		End Sub

        ''' <summary>
        ''' The state of a checkbox menu item
        ''' @serial </summary>
        ''' <seealso cref= #getState() </seealso>
        ''' <seealso cref= #setState(boolean) </seealso>
        Friend _state As Boolean = False

        <NonSerialized> _
		Friend itemListener As ItemListener

		Private Const base As String = "chkmenuitem"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = 6190621106981774043L

		''' <summary>
		''' Create a check box menu item with an empty label.
		''' The item's state is initially set to "off." </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since   JDK1.1 </seealso>
		Public Sub New()
			Me.New("", False)
		End Sub

		''' <summary>
		''' Create a check box menu item with the specified label.
		''' The item's state is initially set to "off."
		''' </summary>
		''' <param name="label">   a string label for the check box menu item,
		'''                or <code>null</code> for an unlabeled menu item. </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String)
			Me.New(label_Renamed, False)
		End Sub

		''' <summary>
		''' Create a check box menu item with the specified label and state. </summary>
		''' <param name="label">   a string label for the check box menu item,
		'''                     or <code>null</code> for an unlabeled menu item. </param>
		''' <param name="state">   the initial state of the menu item, where
		'''                     <code>true</code> indicates "on" and
		'''                     <code>false</code> indicates "off." </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since      JDK1.1 </seealso>
		Public Sub New(ByVal label_Renamed As String, ByVal state As Boolean)
			MyBase.New(label_Renamed)
			Me.state = state
		End Sub

		''' <summary>
		''' Construct a name for this MenuComponent.  Called by getName() when
		''' the name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(CheckboxMenuItem)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer of the checkbox item.  This peer allows us to
		''' change the look of the checkbox item without changing its
		''' functionality.
		''' Most applications do not call this method directly. </summary>
		''' <seealso cref=     java.awt.Toolkit#createCheckboxMenuItem(java.awt.CheckboxMenuItem) </seealso>
		''' <seealso cref=     java.awt.Component#getToolkit() </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = Toolkit.defaultToolkit.createCheckboxMenuItem(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Determines whether the state of this check box menu item
		''' is "on" or "off."
		''' </summary>
		''' <returns>      the state of this check box menu item, where
		'''                     <code>true</code> indicates "on" and
		'''                     <code>false</code> indicates "off" </returns>
		''' <seealso cref=        #setState </seealso>
		Public Overridable Property state As Boolean
			Get
				Return state
			End Get
			Set(ByVal b As Boolean)
				state = b
				Dim peer_Renamed As java.awt.peer.CheckboxMenuItemPeer = CType(Me.peer, java.awt.peer.CheckboxMenuItemPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.state = b
			End Set
		End Property


		''' <summary>
		''' Returns the an array (length 1) containing the checkbox menu item
		''' label or null if the checkbox is not selected. </summary>
		''' <seealso cref= ItemSelectable </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property selectedObjects As Object() Implements ItemSelectable.getSelectedObjects
			Get
				If state Then
					Dim items As Object() = New Object(0){}
					items(0) = label_Renamed
					Return items
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Adds the specified item listener to receive item events from
		''' this check box menu item.  Item events are sent in response to user
		''' actions, but not in response to calls to setState().
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the item listener </param>
		''' <seealso cref=           #removeItemListener </seealso>
		''' <seealso cref=           #getItemListeners </seealso>
		''' <seealso cref=           #setState </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addItemListener(ByVal l As ItemListener) Implements ItemSelectable.addItemListener
			If l Is Nothing Then Return
			itemListener = AWTEventMulticaster.add(itemListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified item listener so that it no longer receives
		''' item events from this check box menu item.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the item listener </param>
		''' <seealso cref=           #addItemListener </seealso>
		''' <seealso cref=           #getItemListeners </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeItemListener(ByVal l As ItemListener) Implements ItemSelectable.removeItemListener
			If l Is Nothing Then Return
			itemListener = AWTEventMulticaster.remove(itemListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the item listeners
		''' registered on this checkbox menuitem.
		''' </summary>
		''' <returns> all of this checkbox menuitem's <code>ItemListener</code>s
		'''         or an empty array if no item
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=           #addItemListener </seealso>
		''' <seealso cref=           #removeItemListener </seealso>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           java.awt.event.ItemListener
		''' @since 1.4 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return getListeners(GetType(ItemListener))
			End Get
		End Property

        ''' <summary>
        ''' Returns an array of all the objects currently registered
        ''' as <code><em>Foo</em>Listener</code>s
        ''' upon this <code>CheckboxMenuItem</code>.
        ''' <code><em>Foo</em>Listener</code>s are registered using the
        ''' <code>add<em>Foo</em>Listener</code> method.
        ''' 
        ''' <p>
        ''' You can specify the <code>listenerType</code> argument
        ''' with a class literal, such as
        ''' <code><em>Foo</em>Listener.class</code>.
        ''' For example, you can query a
        ''' <code>CheckboxMenuItem</code> <code>c</code>
        ''' for its item listeners with the following code:
        ''' 
        ''' <pre>ItemListener[] ils = (ItemListener[])(c.getListeners(ItemListener.class));</pre>
        ''' 
        ''' If no such listeners exist, this method returns an empty array.
        ''' </summary>
        ''' <param name="listenerType"> the type of listeners requested; this parameter
        '''          should specify an interface that descends from
        '''          <code>java.util.EventListener</code> </param>
        ''' <returns> an array of all objects registered as
        '''          <code><em>Foo</em>Listener</code>s on this checkbox menuitem,
        '''          or an empty array if no such
        '''          listeners have been added </returns>
        ''' <exception cref="ClassCastException"> if <code>listenerType</code>
        '''          doesn't specify a class or interface that implements
        '''          <code>java.util.EventListener</code>
        ''' </exception>
        ''' <seealso cref= #getItemListeners
        ''' @since 1.3 </seealso>
        Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As [Class]) As T()
            Dim l As java.util.EventListener = Nothing
            If listenerType Is GetType(ItemListener) Then
                l = itemListener
            Else
                Return MyBase.getListeners(listenerType)
            End If
            Return AWTEventMulticaster.getListeners(l, listenerType)
        End Function

        ' REMIND: remove when filtering is done at lower level
        Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			If e.id = ItemEvent.ITEM_STATE_CHANGED Then
				If (eventMask And AWTEvent.ITEM_EVENT_MASK) <> 0 OrElse itemListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this check box menu item.
		''' If the event is an instance of <code>ItemEvent</code>,
		''' this method invokes the <code>processItemEvent</code> method.
		''' If the event is not an item event,
		''' it invokes <code>processEvent</code> on the superclass.
		''' <p>
		''' Check box menu items currently support only item events.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=          java.awt.event.ItemEvent </seealso>
		''' <seealso cref=          #processItemEvent
		''' @since        JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ItemEvent Then
				processItemEvent(CType(e, ItemEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes item events occurring on this check box menu item by
		''' dispatching them to any registered <code>ItemListener</code> objects.
		''' <p>
		''' This method is not called unless item events are
		''' enabled for this menu item. Item events are enabled
		''' when one of the following occurs:
		''' <ul>
		''' <li>An <code>ItemListener</code> object is registered
		''' via <code>addItemListener</code>.
		''' <li>Item events are enabled via <code>enableEvents</code>.
		''' </ul>
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the item event </param>
		''' <seealso cref=         java.awt.event.ItemEvent </seealso>
		''' <seealso cref=         java.awt.event.ItemListener </seealso>
		''' <seealso cref=         #addItemListener </seealso>
		''' <seealso cref=         java.awt.MenuItem#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processItemEvent(ByVal e As ItemEvent)
			Dim listener As ItemListener = itemListener
			If listener IsNot Nothing Then listener.itemStateChanged(e)
		End Sub

	'    
	'     * Post an ItemEvent and toggle state.
	'     
		Friend Overrides Sub doMenuEvent(ByVal [when] As Long, ByVal modifiers As Integer)
			state = (Not state)
			Toolkit.eventQueue.postEvent(New ItemEvent(Me, ItemEvent.ITEM_STATE_CHANGED, label,If(state, ItemEvent.SELECTED, ItemEvent.DESELECTED)))
		End Sub

		''' <summary>
		''' Returns a string representing the state of this
		''' <code>CheckBoxMenuItem</code>. This
		''' method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>     the parameter string of this check box menu item </returns>
		Public Overrides Function paramString() As String
			Return MyBase.paramString() & ",state=" & state
		End Function

	'     Serialization support.
	'     

	'    
	'     * Serial Data Version
	'     * @serial
	'     
		Private checkboxMenuItemSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ItemListeners</code>
		''' as optional data.  The non-serializable
		''' <code>ItemListeners</code> are detected and
		''' no attempt is made to serialize them.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of
		'''  0 or more pairs; the pair consists of a <code>String</code>
		'''  and an <code>Object</code>; the <code>String</code> indicates
		'''  the type of object and is one of the following:
		'''  <code>itemListenerK</code> indicating an
		'''    <code>ItemListener</code> object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#itemListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, itemListenerK, itemListener)
		  s.writeObject(Nothing)
		End Sub

	'    
	'     * Reads the <code>ObjectInputStream</code> and if it
	'     * isn't <code>null</code> adds a listener to receive
	'     * item events fired by the <code>Checkbox</code> menu item.
	'     * Unrecognized keys or values will be ignored.
	'     *
	'     * @param s the <code>ObjectInputStream</code> to read
	'     * @serial
	'     * @see removeActionListener()
	'     * @see addActionListener()
	'     * @see #writeObject
	'     
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  s.defaultReadObject()

		  Dim keyOrNull As Object
		  keyOrNull = s.readObject()
		  Do While Nothing IsNot keyOrNull
			Dim key As String = CStr(keyOrNull).intern()

			If itemListenerK = key Then
			  addItemListener(CType(s.readObject(), ItemListener))

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
        ''' Gets the AccessibleContext associated with this CheckboxMenuItem.
        ''' For checkbox menu items, the AccessibleContext takes the
        ''' form of an AccessibleAWTCheckboxMenuItem.
        ''' A new AccessibleAWTCheckboxMenuItem is created if necessary.
        ''' </summary>
        ''' <returns> an AccessibleAWTCheckboxMenuItem that serves as the
        '''         AccessibleContext of this CheckboxMenuItem
        ''' @since 1.3 </returns>
        Public ReadOnly Property accessibleContext As AccessibleContext
            Get
                If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTCheckboxMenuItem(Me)
                Return accessibleContext
            End Get
        End Property

        ''' <summary>
        ''' Inner class of CheckboxMenuItem used to provide default support for
        ''' accessibility.  This class is not meant to be used directly by
        ''' application developers, but is instead meant only to be
        ''' subclassed by menu component developers.
        ''' <p>
        ''' This class implements accessibility support for the
        ''' <code>CheckboxMenuItem</code> class.  It provides an implementation
        ''' of the Java Accessibility API appropriate to checkbox menu item
        ''' user-interface elements.
        ''' @since 1.3
        ''' </summary>
        Protected Friend Class AccessibleAWTCheckboxMenuItem
			Inherits AccessibleAWTMenuItem
			Implements AccessibleAction, AccessibleValue

			Private ReadOnly outerInstance As CheckboxMenuItem

			Public Sub New(ByVal outerInstance As CheckboxMenuItem)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -1122642964303476L

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
			''' Returns the number of Actions available in this object.
			''' If there is more than one, the first one is the "default"
			''' action.
			''' </summary>
			''' <returns> the number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer
				Get
					Return 0 '  To be fully implemented in a future release
				End Get
			End Property

			''' <summary>
			''' Return a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String
				Return Nothing '  To be fully implemented in a future release
			End Function

			''' <summary>
			''' Perform the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean
				Return False '  To be fully implemented in a future release
			End Function

			''' <summary>
			''' Get the value of this object as a Number.  If the value has not been
			''' set, the return value will be null.
			''' </summary>
			''' <returns> value of the object </returns>
			''' <seealso cref= #setCurrentAccessibleValue </seealso>
			Public Overridable Property currentAccessibleValue As Number
				Get
					Return Nothing '  To be fully implemented in a future release
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> true if the value was set; otherwise false </returns>
			''' <seealso cref= #getCurrentAccessibleValue </seealso>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean
				Return False '  To be fully implemented in a future release
			End Function

			''' <summary>
			''' Get the minimum value of this object as a Number.
			''' </summary>
			''' <returns> Minimum value of the object; null if this object does not
			''' have a minimum value </returns>
			''' <seealso cref= #getMaximumAccessibleValue </seealso>
			Public Overridable Property minimumAccessibleValue As Number
				Get
					Return Nothing '  To be fully implemented in a future release
				End Get
			End Property

			''' <summary>
			''' Get the maximum value of this object as a Number.
			''' </summary>
			''' <returns> Maximum value of the object; null if this object does not
			''' have a maximum value </returns>
			''' <seealso cref= #getMinimumAccessibleValue </seealso>
			Public Overridable Property maximumAccessibleValue As Number
				Get
					Return Nothing '  To be fully implemented in a future release
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.CHECK_BOX
				End Get
			End Property

		End Class ' class AccessibleAWTMenuItem

	End Class

End Namespace