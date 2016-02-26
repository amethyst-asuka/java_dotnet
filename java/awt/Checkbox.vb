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
	''' A check box is a graphical component that can be in either an
	''' "on" (<code>true</code>) or "off" (<code>false</code>) state.
	''' Clicking on a check box changes its state from
	''' "on" to "off," or from "off" to "on."
	''' <p>
	''' The following code example creates a set of check boxes in
	''' a grid layout:
	''' 
	''' <hr><blockquote><pre>
	''' setLayout(new GridLayout(3, 1));
	''' add(new Checkbox("one", null, true));
	''' add(new Checkbox("two"));
	''' add(new Checkbox("three"));
	''' </pre></blockquote><hr>
	''' <p>
	''' This image depicts the check boxes and grid layout
	''' created by this code example:
	''' <p>
	''' <img src="doc-files/Checkbox-1.gif" alt="The following context describes the graphic."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' The button labeled <code>one</code> is in the "on" state, and the
	''' other two are in the "off" state. In this example, which uses the
	''' <code>GridLayout</code> [Class], the states of the three check
	''' boxes are set independently.
	''' <p>
	''' Alternatively, several check boxes can be grouped together under
	''' the control of a single object, using the
	''' <code>CheckboxGroup</code> class.
	''' In a check box group, at most one button can be in the "on"
	''' state at any given time. Clicking on a check box to turn it on
	''' forces any other check box in the same group that is on
	''' into the "off" state.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.GridLayout </seealso>
	''' <seealso cref=         java.awt.CheckboxGroup
	''' @since       JDK1.0 </seealso>
	Public Class Checkbox
		Inherits Component
		Implements ItemSelectable, Accessible

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' The label of the Checkbox.
		''' This field can be null.
		''' @serial </summary>
		''' <seealso cref= #getLabel() </seealso>
		''' <seealso cref= #setLabel(String) </seealso>
		Friend label As String

		''' <summary>
		''' The state of the <code>Checkbox</code>.
		''' @serial </summary>
		''' <seealso cref= #getState() </seealso>
		''' <seealso cref= #setState(boolean) </seealso>
		Friend state As Boolean

		''' <summary>
		''' The check box group.
		''' This field can be null indicating that the checkbox
		''' is not a group checkbox.
		''' @serial </summary>
		''' <seealso cref= #getCheckboxGroup() </seealso>
		''' <seealso cref= #setCheckboxGroup(CheckboxGroup) </seealso>
		Friend group As CheckboxGroup

		<NonSerialized> _
		Friend itemListener As ItemListener

		Private Const base As String = "checkbox"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 7270714317450821763L

		''' <summary>
		''' Helper function for setState and CheckboxGroup.setSelectedCheckbox
		''' Should remain package-private.
		''' </summary>
		Friend Overridable Property stateInternal As Boolean
			Set(ByVal state As Boolean)
				Me.state = state
				Dim peer_Renamed As java.awt.peer.CheckboxPeer = CType(Me.peer, java.awt.peer.CheckboxPeer)
				If peer_Renamed IsNot Nothing Then peer_Renamed.state = state
			End Set
		End Property

		''' <summary>
		''' Creates a check box with an empty string for its label.
		''' The state of this check box is set to "off," and it is not
		''' part of any check box group. </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New("", False, Nothing)
		End Sub

		''' <summary>
		''' Creates a check box with the specified label.  The state
		''' of this check box is set to "off," and it is not part of
		''' any check box group.
		''' </summary>
		''' <param name="label">   a string label for this check box,
		'''                        or <code>null</code> for no label. </param>
		''' <exception cref="HeadlessException"> if
		'''      <code>GraphicsEnvironment.isHeadless</code>
		'''      returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String)
			Me.New(label_Renamed, False, Nothing)
		End Sub

		''' <summary>
		''' Creates a check box with the specified label
		''' and sets the specified state.
		''' This check box is not part of any check box group.
		''' </summary>
		''' <param name="label">   a string label for this check box,
		'''                        or <code>null</code> for no label </param>
		''' <param name="state">    the initial state of this check box </param>
		''' <exception cref="HeadlessException"> if
		'''     <code>GraphicsEnvironment.isHeadless</code>
		'''     returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String, ByVal state As Boolean)
			Me.New(label_Renamed, state, Nothing)
		End Sub

		''' <summary>
		''' Constructs a Checkbox with the specified label, set to the
		''' specified state, and in the specified check box group.
		''' </summary>
		''' <param name="label">   a string label for this check box,
		'''                        or <code>null</code> for no label. </param>
		''' <param name="state">   the initial state of this check box. </param>
		''' <param name="group">   a check box group for this check box,
		'''                           or <code>null</code> for no group. </param>
		''' <exception cref="HeadlessException"> if
		'''     <code>GraphicsEnvironment.isHeadless</code>
		'''     returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since     JDK1.1 </seealso>
		Public Sub New(ByVal label_Renamed As String, ByVal state As Boolean, ByVal group As CheckboxGroup)
			GraphicsEnvironment.checkHeadless()
			Me.label = label_Renamed
			Me.state = state
			Me.group = group
			If state AndAlso (group IsNot Nothing) Then group.selectedCheckbox = Me
		End Sub

		''' <summary>
		''' Creates a check box with the specified label, in the specified
		''' check box group, and set to the specified state.
		''' </summary>
		''' <param name="label">   a string label for this check box,
		'''                        or <code>null</code> for no label. </param>
		''' <param name="group">   a check box group for this check box,
		'''                           or <code>null</code> for no group. </param>
		''' <param name="state">   the initial state of this check box. </param>
		''' <exception cref="HeadlessException"> if
		'''    <code>GraphicsEnvironment.isHeadless</code>
		'''    returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since     JDK1.1 </seealso>
		Public Sub New(ByVal label_Renamed As String, ByVal group As CheckboxGroup, ByVal state As Boolean)
			Me.New(label_Renamed, state, group)
		End Sub

		''' <summary>
		''' Constructs a name for this component.  Called by
		''' <code>getName</code> when the name is <code>null</code>.
		''' </summary>
		''' <returns> a name for this component </returns>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Checkbox)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer of the Checkbox. The peer allows you to change the
		''' look of the Checkbox without changing its functionality.
		''' </summary>
		''' <seealso cref=     java.awt.Toolkit#createCheckbox(java.awt.Checkbox) </seealso>
		''' <seealso cref=     java.awt.Component#getToolkit() </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createCheckbox(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the label of this check box.
		''' </summary>
		''' <returns>   the label of this check box, or <code>null</code>
		'''                  if this check box has no label. </returns>
		''' <seealso cref=      #setLabel(String) </seealso>
		Public Overridable Property label As String
			Get
				Return label
			End Get
			Set(ByVal label_Renamed As String)
				Dim testvalid As Boolean = False
    
				SyncLock Me
					If label_Renamed <> Me.label AndAlso (Me.label Is Nothing OrElse (Not Me.label.Equals(label_Renamed))) Then
						Me.label = label_Renamed
						Dim peer_Renamed As java.awt.peer.CheckboxPeer = CType(Me.peer, java.awt.peer.CheckboxPeer)
						If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
						testvalid = True
					End If
				End SyncLock
    
				' This could change the preferred size of the Component.
				If testvalid Then invalidateIfValid()
			End Set
		End Property


		''' <summary>
		''' Determines whether this check box is in the "on" or "off" state.
		''' The boolean value <code>true</code> indicates the "on" state,
		''' and <code>false</code> indicates the "off" state.
		''' </summary>
		''' <returns>    the state of this check box, as a boolean value </returns>
		''' <seealso cref=       #setState </seealso>
		Public Overridable Property state As Boolean
			Get
				Return state
			End Get
			Set(ByVal state As Boolean)
				' Cannot hold check box lock when calling group.setSelectedCheckbox. 
				Dim group As CheckboxGroup = Me.group
				If group IsNot Nothing Then
					If state Then
						group.selectedCheckbox = Me
					ElseIf group.selectedCheckbox Is Me Then
						state = True
					End If
				End If
				stateInternal = state
			End Set
		End Property


		''' <summary>
		''' Returns an array (length 1) containing the checkbox
		''' label or null if the checkbox is not selected. </summary>
		''' <seealso cref= ItemSelectable </seealso>
		Public Overridable Property selectedObjects As Object() Implements ItemSelectable.getSelectedObjects
			Get
				If state Then
					Dim items As Object() = New Object(0){}
					items(0) = label
					Return items
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Determines this check box's group. </summary>
		''' <returns>     this check box's group, or <code>null</code>
		'''               if the check box is not part of a check box group. </returns>
		''' <seealso cref=        #setCheckboxGroup(CheckboxGroup) </seealso>
		Public Overridable Property checkboxGroup As CheckboxGroup
			Get
				Return group
			End Get
			Set(ByVal g As CheckboxGroup)
				Dim oldGroup As CheckboxGroup
				Dim oldState As Boolean
    
		'         Do nothing if this check box has already belonged
		'         * to the check box group g.
		'         
				If Me.group Is g Then Return
    
				SyncLock Me
					oldGroup = Me.group
					oldState = state
    
					Me.group = g
					Dim peer_Renamed As java.awt.peer.CheckboxPeer = CType(Me.peer, java.awt.peer.CheckboxPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.checkboxGroup = g
					If Me.group IsNot Nothing AndAlso state Then
						If Me.group.selectedCheckbox IsNot Nothing Then
							state = False
						Else
							Me.group.selectedCheckbox = Me
						End If
					End If
				End SyncLock
    
		'         Locking check box below could cause deadlock with
		'         * CheckboxGroup's setSelectedCheckbox method.
		'         *
		'         * Fix for 4726853 by kdm@sparc.spb.su
		'         * Here we should check if this check box was selected
		'         * in the previous group and set selected check box to
		'         * null for that group if so.
		'         
				If oldGroup IsNot Nothing AndAlso oldState Then oldGroup.selectedCheckbox = Nothing
			End Set
		End Property


		''' <summary>
		''' Adds the specified item listener to receive item events from
		''' this check box.  Item events are sent to listeners in response
		''' to user input, but not in response to calls to setState().
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">    the item listener </param>
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
		''' Removes the specified item listener so that the item listener
		''' no longer receives item events from this check box.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">    the item listener </param>
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
		''' registered on this checkbox.
		''' </summary>
		''' <returns> all of this checkbox's <code>ItemListener</code>s
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
		''' upon this <code>Checkbox</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>Checkbox</code> <code>c</code>
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
		'''          <code><em>Foo</em>Listener</code>s on this checkbox,
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
		''' Processes events on this check box.
		''' If the event is an instance of <code>ItemEvent</code>,
		''' this method invokes the <code>processItemEvent</code> method.
		''' Otherwise, it calls its superclass's <code>processEvent</code> method.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=           java.awt.event.ItemEvent </seealso>
		''' <seealso cref=           #processItemEvent
		''' @since         JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ItemEvent Then
				processItemEvent(CType(e, ItemEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes item events occurring on this check box by
		''' dispatching them to any registered
		''' <code>ItemListener</code> objects.
		''' <p>
		''' This method is not called unless item events are
		''' enabled for this component. Item events are enabled
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
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processItemEvent(ByVal e As ItemEvent)
			Dim listener As ItemListener = itemListener
			If listener IsNot Nothing Then listener.itemStateChanged(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Checkbox</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>    the parameter string of this check box </returns>
		Protected Friend Overrides Function paramString() As String
			Dim str As String = MyBase.paramString()
			Dim label_Renamed As String = Me.label
			If label_Renamed IsNot Nothing Then str &= ",label=" & label_Renamed
			Return str & ",state=" & state
		End Function


	'     Serialization support.
	'     

	'    
	'     * Serialized data version
	'     * @serial
	'     
		Private checkboxSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ItemListeners</code>
		''' as optional data.  The non-serializable
		''' <code>ItemListeners</code> are detected and
		''' no attempt is made to serialize them.
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write
		''' @serialData <code>null</code> terminated sequence of 0
		'''   or more pairs; the pair consists of a <code>String</code>
		'''   and an <code>Object</code>; the <code>String</code> indicates
		'''   the type of object and is one of the following:
		'''   <code>itemListenerK</code> indicating an
		'''     <code>ItemListener</code> object
		''' </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#itemListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, itemListenerK, itemListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if it
		''' isn't <code>null</code> adds a listener to receive
		''' item events fired by the <code>Checkbox</code>.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code>
		''' @serial </exception>
		''' <seealso cref= #removeItemListener(ItemListener) </seealso>
		''' <seealso cref= #addItemListener(ItemListener) </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  GraphicsEnvironment.checkHeadless()
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
		''' Initialize JNI field and method ids
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this Checkbox.
		''' For checkboxes, the AccessibleContext takes the form of an
		''' AccessibleAWTCheckbox.
		''' A new AccessibleAWTCheckbox is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTCheckbox that serves as the
		'''         AccessibleContext of this Checkbox
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTCheckbox(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Checkbox</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to checkbox user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTCheckbox
			Inherits AccessibleAWTComponent
			Implements ItemListener, AccessibleAction, AccessibleValue

			Private ReadOnly outerInstance As Checkbox

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 7881579233144754107L

			Public Sub New(ByVal outerInstance As Checkbox)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.addItemListener(Me)
			End Sub

			''' <summary>
			''' Fire accessible property change events when the state of the
			''' toggle button changes.
			''' </summary>
			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent) Implements ItemListener.itemStateChanged
				Dim cb As Checkbox = CType(e.source, Checkbox)
				If outerInstance.accessibleContext IsNot Nothing Then
					If cb.state Then
						outerInstance.accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.CHECKED)
					Else
						outerInstance.accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.CHECKED, Nothing)
					End If
				End If
			End Sub

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
			''' <returns> true if the the action was performed; else false. </returns>
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
			''' <returns> True if the value was set; else False </returns>
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
			''' <returns> an instance of AccessibleRole describing the role of
			''' the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.CHECK_BOX
				End Get
			End Property

			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.state Then states.add(AccessibleState.CHECKED)
					Return states
				End Get
			End Property


		End Class ' inner class AccessibleAWTCheckbox

	End Class

End Namespace