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
	''' A generic implementation of SingleSelectionModel.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Dave Moore
	''' </summary>
	<Serializable> _
	Public Class DefaultSingleSelectionModel
		Implements SingleSelectionModel

	'     Only one ModelChangeEvent is needed per model instance since the
	'     * event's only (read-only) state is the source property.  The source
	'     * of events generated here is always "this".
	'     
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing
		''' <summary>
		''' The collection of registered listeners </summary>
		Protected Friend listenerList As New EventListenerList

		Private index As Integer = -1

		' implements javax.swing.SingleSelectionModel
		Public Overridable Property selectedIndex As Integer Implements SingleSelectionModel.getSelectedIndex
			Get
				Return index
			End Get
			Set(ByVal index As Integer)
				If Me.index <> index Then
					Me.index = index
					fireStateChanged()
				End If
			End Set
		End Property

		' implements javax.swing.SingleSelectionModel

		' implements javax.swing.SingleSelectionModel
		Public Overridable Sub clearSelection() Implements SingleSelectionModel.clearSelection
			selectedIndex = -1
		End Sub

		' implements javax.swing.SingleSelectionModel
		Public Overridable Property selected As Boolean Implements SingleSelectionModel.isSelected
			Get
				Dim ret As Boolean = False
				If selectedIndex <> -1 Then ret = True
				Return ret
			End Get
		End Property

		''' <summary>
		''' Adds a <code>ChangeListener</code> to the button.
		''' </summary>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener) Implements SingleSelectionModel.addChangeListener
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>ChangeListener</code> from the button.
		''' </summary>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener) Implements SingleSelectionModel.removeChangeListener
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the change listeners
		''' registered on this <code>DefaultSingleSelectionModel</code>.
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
		''' is created lazily. </summary>
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
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a <code>DefaultSingleSelectionModel</code>
		''' instance <code>m</code>
		''' for its change listeners
		''' with the following code:
		''' 
		''' <pre>ChangeListener[] cls = (ChangeListener[])(m.getListeners(ChangeListener.class));</pre>
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
		''' <seealso cref= #getChangeListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function
	End Class

End Namespace