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
	''' This class creates a labeled button. The application can cause
	''' some action to happen when the button is pushed. This image
	''' depicts three views of a "<code>Quit</code>" button as it appears
	''' under the Solaris operating system:
	''' <p>
	''' <img src="doc-files/Button-1.gif" alt="The following context describes the graphic"
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' The first view shows the button as it appears normally.
	''' The second view shows the button
	''' when it has input focus. Its outline is darkened to let the
	''' user know that it is an active object. The third view shows the
	''' button when the user clicks the mouse over the button, and thus
	''' requests that an action be performed.
	''' <p>
	''' The gesture of clicking on a button with the mouse
	''' is associated with one instance of <code>ActionEvent</code>,
	''' which is sent out when the mouse is both pressed and released
	''' over the button. If an application is interested in knowing
	''' when the button has been pressed but not released, as a separate
	''' gesture, it can specialize <code>processMouseEvent</code>,
	''' or it can register itself as a listener for mouse events by
	''' calling <code>addMouseListener</code>. Both of these methods are
	''' defined by <code>Component</code>, the abstract superclass of
	''' all components.
	''' <p>
	''' When a button is pressed and released, AWT sends an instance
	''' of <code>ActionEvent</code> to the button, by calling
	''' <code>processEvent</code> on the button. The button's
	''' <code>processEvent</code> method receives all events
	''' for the button; it passes an action event along by
	''' calling its own <code>processActionEvent</code> method.
	''' The latter method passes the action event on to any action
	''' listeners that have registered an interest in action
	''' events generated by this button.
	''' <p>
	''' If an application wants to perform some action based on
	''' a button being pressed and released, it should implement
	''' <code>ActionListener</code> and register the new listener
	''' to receive events from this button, by calling the button's
	''' <code>addActionListener</code> method. The application can
	''' make use of the button's action command as a messaging protocol.
	''' 
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.event.ActionEvent </seealso>
	''' <seealso cref=         java.awt.event.ActionListener </seealso>
	''' <seealso cref=         java.awt.Component#processMouseEvent </seealso>
	''' <seealso cref=         java.awt.Component#addMouseListener
	''' @since       JDK1.0 </seealso>
	Public Class Button
		Inherits Component
		Implements Accessible

		''' <summary>
		''' The button's label.  This value may be null.
		''' @serial </summary>
		''' <seealso cref= #getLabel() </seealso>
		''' <seealso cref= #setLabel(String) </seealso>
		Friend label As String

		''' <summary>
		''' The action to be performed once a button has been
		''' pressed.  This value may be null.
		''' @serial </summary>
		''' <seealso cref= #getActionCommand() </seealso>
		''' <seealso cref= #setActionCommand(String) </seealso>
		Friend actionCommand As String

		<NonSerialized> _
		Friend actionListener As ActionListener

		Private Const base As String = "button"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -8774683716313001058L


		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Initialize JNI field and method IDs for fields that may be
		''' accessed from C.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		''' <summary>
		''' Constructs a button with an empty string for its label.
		''' </summary>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New()
			Me.New("")
		End Sub

		''' <summary>
		''' Constructs a button with the specified label.
		''' </summary>
		''' <param name="label">  a string label for the button, or
		'''               <code>null</code> for no label </param>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Sub New(ByVal label_Renamed As String)
			GraphicsEnvironment.checkHeadless()
			Me.label = label_Renamed
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Button)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Creates the peer of the button.  The button's peer allows the
		''' application to change the look of the button without changing
		''' its functionality.
		''' </summary>
		''' <seealso cref=     java.awt.Toolkit#createButton(java.awt.Button) </seealso>
		''' <seealso cref=     java.awt.Component#getToolkit() </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If peer Is Nothing Then peer = toolkit.createButton(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the label of this button.
		''' </summary>
		''' <returns>    the button's label, or <code>null</code>
		'''                if the button has no label. </returns>
		''' <seealso cref=       java.awt.Button#setLabel </seealso>
		Public Overridable Property label As String
			Get
				Return label
			End Get
			Set(ByVal label_Renamed As String)
				Dim testvalid As Boolean = False
    
				SyncLock Me
					If label_Renamed <> Me.label AndAlso (Me.label Is Nothing OrElse (Not Me.label.Equals(label_Renamed))) Then
						Me.label = label_Renamed
						Dim peer_Renamed As java.awt.peer.ButtonPeer = CType(Me.peer, java.awt.peer.ButtonPeer)
						If peer_Renamed IsNot Nothing Then peer_Renamed.label = label_Renamed
						testvalid = True
					End If
				End SyncLock
    
				' This could change the preferred size of the Component.
				If testvalid Then invalidateIfValid()
			End Set
		End Property


		''' <summary>
		''' Sets the command name for the action event fired
		''' by this button. By default this action command is
		''' set to match the label of the button.
		''' </summary>
		''' <param name="command">  a string used to set the button's
		'''                  action command.
		'''            If the string is <code>null</code> then the action command
		'''            is set to match the label of the button. </param>
		''' <seealso cref=       java.awt.event.ActionEvent
		''' @since     JDK1.1 </seealso>
		Public Overridable Property actionCommand As String
			Set(ByVal command As String)
				actionCommand = command
			End Set
			Get
				Return (If(actionCommand Is Nothing, label, actionCommand))
			End Get
		End Property


		''' <summary>
		''' Adds the specified action listener to receive action events from
		''' this button. Action events occur when a user presses or releases
		''' the mouse over this button.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l"> the action listener </param>
		''' <seealso cref=           #removeActionListener </seealso>
		''' <seealso cref=           #getActionListeners </seealso>
		''' <seealso cref=           java.awt.event.ActionListener
		''' @since         JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.add(actionListener, l)
			newEventsOnly = True
		End Sub

		''' <summary>
		''' Removes the specified action listener so that it no longer
		''' receives action events from this button. Action events occur
		''' when a user presses or releases the mouse over this button.
		''' If l is null, no exception is thrown and no action is performed.
		''' <p>Refer to <a href="doc-files/AWTThreadIssues.html#ListenersThreads"
		''' >AWT Threading Issues</a> for details on AWT's threading model.
		''' </summary>
		''' <param name="l">     the action listener </param>
		''' <seealso cref=             #addActionListener </seealso>
		''' <seealso cref=             #getActionListeners </seealso>
		''' <seealso cref=             java.awt.event.ActionListener
		''' @since           JDK1.1 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			If l Is Nothing Then Return
			actionListener = AWTEventMulticaster.remove(actionListener, l)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this button.
		''' </summary>
		''' <returns> all of this button's <code>ActionListener</code>s
		'''         or an empty array if no action
		'''         listeners are currently registered
		''' </returns>
		''' <seealso cref=             #addActionListener </seealso>
		''' <seealso cref=             #removeActionListener </seealso>
		''' <seealso cref=             java.awt.event.ActionListener
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
		''' upon this <code>Button</code>.
		''' <code><em>Foo</em>Listener</code>s are registered using the
		''' <code>add<em>Foo</em>Listener</code> method.
		''' 
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as
		''' <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a
		''' <code>Button</code> <code>b</code>
		''' for its action listeners with the following code:
		''' 
		''' <pre>ActionListener[] als = (ActionListener[])(b.getListeners(ActionListener.class));</pre>
		''' 
		''' If no such listeners exist, this method returns an empty array.
		''' </summary>
		''' <param name="listenerType"> the type of listeners requested; this parameter
		'''          should specify an interface that descends from
		'''          <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s on this button,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code>
		'''          doesn't specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getActionListeners
		''' @since 1.3 </seealso>
		Public Overrides Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Class) As T()
			Dim l As java.util.EventListener = Nothing
			If listenerType Is GetType(ActionListener) Then
				l = actionListener
			Else
				Return MyBase.getListeners(listenerType)
			End If
			Return AWTEventMulticaster.getListeners(l, listenerType)
		End Function

		' REMIND: remove when filtering is done at lower level
		Friend Overrides Function eventEnabled(ByVal e As AWTEvent) As Boolean
			If e.id = ActionEvent.ACTION_PERFORMED Then
				If (eventMask And AWTEvent.ACTION_EVENT_MASK) <> 0 OrElse actionListener IsNot Nothing Then Return True
				Return False
			End If
			Return MyBase.eventEnabled(e)
		End Function

		''' <summary>
		''' Processes events on this button. If an event is
		''' an instance of <code>ActionEvent</code>, this method invokes
		''' the <code>processActionEvent</code> method. Otherwise,
		''' it invokes <code>processEvent</code> on the superclass.
		''' <p>Note that if the event parameter is <code>null</code>
		''' the behavior is unspecified and may result in an
		''' exception.
		''' </summary>
		''' <param name="e"> the event </param>
		''' <seealso cref=          java.awt.event.ActionEvent </seealso>
		''' <seealso cref=          java.awt.Button#processActionEvent
		''' @since        JDK1.1 </seealso>
		Protected Friend Overrides Sub processEvent(ByVal e As AWTEvent)
			If TypeOf e Is ActionEvent Then
				processActionEvent(CType(e, ActionEvent))
				Return
			End If
			MyBase.processEvent(e)
		End Sub

		''' <summary>
		''' Processes action events occurring on this button
		''' by dispatching them to any registered
		''' <code>ActionListener</code> objects.
		''' <p>
		''' This method is not called unless action events are
		''' enabled for this button. Action events are enabled
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
		''' <seealso cref=         java.awt.event.ActionListener </seealso>
		''' <seealso cref=         java.awt.Button#addActionListener </seealso>
		''' <seealso cref=         java.awt.Component#enableEvents
		''' @since       JDK1.1 </seealso>
		Protected Friend Overridable Sub processActionEvent(ByVal e As ActionEvent)
			Dim listener As ActionListener = actionListener
			If listener IsNot Nothing Then listener.actionPerformed(e)
		End Sub

		''' <summary>
		''' Returns a string representing the state of this <code>Button</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>     the parameter string of this button </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString() & ",label=" & label
		End Function


	'     Serialization support.
	'     

	'    
	'     * Button Serial Data Version.
	'     * @serial
	'     
		Private buttonSerializedDataVersion As Integer = 1

		''' <summary>
		''' Writes default serializable fields to stream.  Writes
		''' a list of serializable <code>ActionListeners</code>
		''' as optional data.  The non-serializable
		''' <code>ActionListeners</code> are detected and
		''' no attempt is made to serialize them.
		''' 
		''' @serialData <code>null</code> terminated sequence of 0 or
		'''   more pairs: the pair consists of a <code>String</code>
		'''   and an <code>Object</code>; the <code>String</code>
		'''   indicates the type of object and is one of the following:
		'''   <code>actionListenerK</code> indicating an
		'''     <code>ActionListener</code> object
		''' </summary>
		''' <param name="s"> the <code>ObjectOutputStream</code> to write </param>
		''' <seealso cref= AWTEventMulticaster#save(ObjectOutputStream, String, EventListener) </seealso>
		''' <seealso cref= java.awt.Component#actionListenerK </seealso>
		''' <seealso cref= #readObject(ObjectInputStream) </seealso>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
		  s.defaultWriteObject()

		  AWTEventMulticaster.save(s, actionListenerK, actionListener)
		  s.writeObject(Nothing)
		End Sub

		''' <summary>
		''' Reads the <code>ObjectInputStream</code> and if
		''' it isn't <code>null</code> adds a listener to
		''' receive action events fired by the button.
		''' Unrecognized keys or values will be ignored.
		''' </summary>
		''' <param name="s"> the <code>ObjectInputStream</code> to read </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code>
		''' @serial </exception>
		''' <seealso cref= #removeActionListener(ActionListener) </seealso>
		''' <seealso cref= #addActionListener(ActionListener) </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #writeObject(ObjectOutputStream) </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
		  GraphicsEnvironment.checkHeadless()
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


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with
		''' this <code>Button</code>. For buttons, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleAWTButton</code>.
		''' A new <code>AccessibleAWTButton</code> instance is
		''' created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleAWTButton</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>Button</code>
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this Button.
		''' @since 1.3 </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTButton(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Button</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to button user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTButton
			Inherits AccessibleAWTComponent
			Implements AccessibleAction, AccessibleValue

			Private ReadOnly outerInstance As Button

			Public Sub New(ByVal outerInstance As Button)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = -5932203980244017102L

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
			''' Get the AccessibleAction associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
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
			''' implementation of the Java Accessibility API for this class,
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
			''' default behavior of a button is to have one action - toggle
			''' the button.
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
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String
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
			''' <returns> true if the the action was performed; else false. </returns>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean
				If i = 0 Then
					' Simulate a button click
					Toolkit.eventQueue.postEvent(New ActionEvent(Button.this, ActionEvent.ACTION_PERFORMED, outerInstance.actionCommand))
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
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean
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

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PUSH_BUTTON
				End Get
			End Property
		End Class ' inner class AccessibleAWTButton

	End Class

End Namespace