Imports System
Imports javax.swing.event

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing

	''' <summary>
	''' The default implementation of a <code>Button</code> component's data model.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing. As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	<Serializable> _
	Public Class DefaultButtonModel
		Implements ButtonModel

		''' <summary>
		''' The bitmask used to store the state of the button. </summary>
		Protected Friend stateMask As Integer = 0

		''' <summary>
		''' The action command string fired by the button. </summary>
		Protected Friend actionCommand As String = Nothing

		''' <summary>
		''' The button group that the button belongs to. </summary>
		Protected Friend group As ButtonGroup = Nothing

		''' <summary>
		''' The button's mnemonic. </summary>
		Protected Friend mnemonic As Integer = 0

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per button model
		''' instance since the event's only state is the source property.
		''' The source of events generated is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		''' <summary>
		''' Stores the listeners on this model. </summary>
		Protected Friend listenerList As New EventListenerList

		' controls the usage of the MenuItem.disabledAreNavigable UIDefaults
		' property in the setArmed() method
		Private menuItem As Boolean = False

		''' <summary>
		''' Constructs a <code>DefaultButtonModel</code>.
		''' 
		''' </summary>
		Public Sub New()
			stateMask = 0
			enabled = True
		End Sub

		''' <summary>
		''' Identifies the "armed" bit in the bitmask, which
		''' indicates partial commitment towards choosing/triggering
		''' the button.
		''' </summary>
		Public Shared ReadOnly ARMED As Integer = 1 << 0

		''' <summary>
		''' Identifies the "selected" bit in the bitmask, which
		''' indicates that the button has been selected. Only needed for
		''' certain types of buttons - such as radio button or check box.
		''' </summary>
		Public Shared ReadOnly SELECTED As Integer = 1 << 1

		''' <summary>
		''' Identifies the "pressed" bit in the bitmask, which
		''' indicates that the button is pressed.
		''' </summary>
		Public Shared ReadOnly PRESSED As Integer = 1 << 2

		''' <summary>
		''' Identifies the "enabled" bit in the bitmask, which
		''' indicates that the button can be selected by
		''' an input device (such as a mouse pointer).
		''' </summary>
		Public Shared ReadOnly ENABLED As Integer = 1 << 3

		''' <summary>
		''' Identifies the "rollover" bit in the bitmask, which
		''' indicates that the mouse is over the button.
		''' </summary>
		Public Shared ReadOnly ROLLOVER As Integer = 1 << 4

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property actionCommand Implements ButtonModel.setActionCommand As String
			Set(ByVal actionCommand As String)
				Me.actionCommand = actionCommand
			End Set
			Get
				Return actionCommand
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property armed As Boolean Implements ButtonModel.isArmed
			Get
				Return (stateMask And ARMED) <> 0
			End Get
			Set(ByVal b As Boolean)
				If menuItem AndAlso UIManager.getBoolean("MenuItem.disabledAreNavigable") Then
					If (armed = b) Then Return
				Else
					If (armed = b) OrElse (Not enabled) Then Return
				End If
    
				If b Then
					stateMask = stateMask Or ARMED
				Else
					stateMask = stateMask And Not ARMED
				End If
    
				fireStateChanged()
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function isSelected() As Boolean Implements ButtonModel.isSelected 'JavaToDotNetTempPropertyGetselected
		Public Overridable Property selected As Boolean Implements ButtonModel.isSelected
			Get
				Return (stateMask And SELECTED) <> 0
			End Get
			Set(ByVal b As Boolean)
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property enabled As Boolean Implements ButtonModel.isEnabled
			Get
				Return (stateMask And ENABLED) <> 0
			End Get
			Set(ByVal b As Boolean)
				If enabled = b Then Return
    
				If b Then
					stateMask = stateMask Or ENABLED
				Else
					stateMask = stateMask And Not ENABLED
					' unarm and unpress, just in case
					stateMask = stateMask And Not ARMED
					stateMask = stateMask And Not PRESSED
				End If
    
    
				fireStateChanged()
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function isPressed() As Boolean Implements ButtonModel.isPressed 'JavaToDotNetTempPropertyGetpressed
		Public Overridable Property pressed As Boolean Implements ButtonModel.isPressed
			Get
				Return (stateMask And PRESSED) <> 0
			End Get
			Set(ByVal b As Boolean)
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function isRollover() As Boolean Implements ButtonModel.isRollover 'JavaToDotNetTempPropertyGetrollover
		Public Overridable Property rollover As Boolean Implements ButtonModel.isRollover
			Get
				Return (stateMask And ROLLOVER) <> 0
			End Get
			Set(ByVal b As Boolean)
		End Property



			If Me.selected = b Then Return

			If b Then
				stateMask = stateMask Or SELECTED
			Else
				stateMask = stateMask And Not SELECTED
			End If

			fireItemStateChanged(New ItemEvent(Me, ItemEvent.ITEM_STATE_CHANGED, Me,If(b, ItemEvent.SELECTED, ItemEvent.DESELECTED)))

			fireStateChanged()

		End Sub


			If (pressed = b) OrElse (Not enabled) Then Return

			If b Then
				stateMask = stateMask Or PRESSED
			Else
				stateMask = stateMask And Not PRESSED
			End If

			If (Not pressed) AndAlso armed Then
				Dim modifiers As Integer = 0
				Dim currentEvent As AWTEvent = EventQueue.currentEvent
				If TypeOf currentEvent Is InputEvent Then
					modifiers = CType(currentEvent, InputEvent).modifiers
				ElseIf TypeOf currentEvent Is ActionEvent Then
					modifiers = CType(currentEvent, ActionEvent).modifiers
				End If
				fireActionPerformed(New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, actionCommand, EventQueue.mostRecentEventTime, modifiers))
			End If

			fireStateChanged()
		End Sub

			If (rollover = b) OrElse (Not enabled) Then Return

			If b Then
				stateMask = stateMask Or ROLLOVER
			Else
				stateMask = stateMask And Not ROLLOVER
			End If

			fireStateChanged()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property mnemonic Implements ButtonModel.setMnemonic As Integer
			Set(ByVal key As Integer)
				mnemonic = key
				fireStateChanged()
			End Set
			Get
				Return mnemonic
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener) Implements ButtonModel.addChangeListener
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener) Implements ButtonModel.removeChangeListener
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the change listeners
		''' registered on this <code>DefaultButtonModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ChangeListener</code>s
		'''         or an empty
		'''         array if no change listeners are currently registered
		''' </returns>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= #removeChangeListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub addActionListener(ByVal l As ActionListener) Implements ButtonModel.addActionListener
			listenerList.add(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub removeActionListener(ByVal l As ActionListener) Implements ButtonModel.removeActionListener
			listenerList.remove(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the action listeners
		''' registered on this <code>DefaultButtonModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ActionListener</code>s
		'''         or an empty
		'''         array if no action listeners are currently registered
		''' </returns>
		''' <seealso cref= #addActionListener </seealso>
		''' <seealso cref= #removeActionListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="e"> the <code>ActionEvent</code> to deliver to listeners </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionPerformed(ByVal e As ActionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then CType(___listeners(i+1), ActionListener).actionPerformed(e)
			Next i
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub addItemListener(ByVal l As ItemListener) Implements ButtonModel.addItemListener
			listenerList.add(GetType(ItemListener), l)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub removeItemListener(ByVal l As ItemListener) Implements ButtonModel.removeItemListener
			listenerList.remove(GetType(ItemListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the item listeners
		''' registered on this <code>DefaultButtonModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ItemListener</code>s
		'''         or an empty
		'''         array if no item listeners are currently registered
		''' </returns>
		''' <seealso cref= #addItemListener </seealso>
		''' <seealso cref= #removeItemListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return listenerList.getListeners(GetType(ItemListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="e"> the <code>ItemEvent</code> to deliver to listeners </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireItemStateChanged(ByVal e As ItemEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ItemListener) Then CType(___listeners(i+1), ItemListener).itemStateChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a <code>DefaultButtonModel</code>
		''' instance <code>m</code>
		''' for its action listeners
		''' with the following code:
		''' 
		''' <pre>ActionListener[] als = (ActionListener[])(m.getListeners(ActionListener.class));</pre>
		''' 
		''' If no such listeners exist,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="listenerType">  the type of listeners requested;
		'''          this parameter should specify an interface
		'''          that descends from <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s
		'''          on this model,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code> doesn't
		'''          specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getActionListeners </seealso>
		''' <seealso cref= #getChangeListeners </seealso>
		''' <seealso cref= #getItemListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		''' <summary>
		''' Overridden to return <code>null</code>. </summary>
		Public Overridable Property selectedObjects As Object()
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Property group Implements ButtonModel.setGroup As ButtonGroup
			Set(ByVal group As ButtonGroup)
				Me.group = group
			End Set
			Get
				Return group
			End Get
		End Property


		Friend Overridable Property menuItem As Boolean
			Get
				Return menuItem
			End Get
			Set(ByVal menuItem As Boolean)
				Me.menuItem = menuItem
			End Set
		End Property

	End Class

End Namespace