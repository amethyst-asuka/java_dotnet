Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans


	''' <summary>
	''' This is a utility class that can be used by beans that support constrained
	''' properties.  It manages a list of listeners and dispatches
	''' <seealso cref="PropertyChangeEvent"/>s to them.  You can use an instance of this class
	''' as a member field of your bean and delegate these types of work to it.
	''' The <seealso cref="VetoableChangeListener"/> can be registered for all properties
	''' or for a property specified by name.
	''' <p>
	''' Here is an example of {@code VetoableChangeSupport} usage that follows
	''' the rules and recommendations laid out in the JavaBeans&trade; specification:
	''' <pre>{@code
	''' public class MyBean {
	'''     private final VetoableChangeSupport vcs = new VetoableChangeSupport(this);
	''' 
	'''     public  Sub  addVetoableChangeListener(VetoableChangeListener listener) {
	'''         this.vcs.addVetoableChangeListener(listener);
	'''     }
	''' 
	'''     public  Sub  removeVetoableChangeListener(VetoableChangeListener listener) {
	'''         this.vcs.removeVetoableChangeListener(listener);
	'''     }
	''' 
	'''     private String value;
	''' 
	'''     public String getValue() {
	'''         return this.value;
	'''     }
	''' 
	'''     public  Sub  setValue(String newValue) throws PropertyVetoException {
	'''         String oldValue = this.value;
	'''         this.vcs.fireVetoableChange("value", oldValue, newValue);
	'''         this.value = newValue;
	'''     }
	''' 
	'''     [...]
	''' }
	''' }</pre>
	''' <p>
	''' A {@code VetoableChangeSupport} instance is thread-safe.
	''' <p>
	''' This class is serializable.  When it is serialized it will save
	''' (and restore) any listeners that are themselves serializable.  Any
	''' non-serializable listeners will be skipped during serialization.
	''' </summary>
	''' <seealso cref= PropertyChangeSupport </seealso>
	<Serializable> _
	Public Class VetoableChangeSupport
		Private map As New VetoableChangeListenerMap

		''' <summary>
		''' Constructs a <code>VetoableChangeSupport</code> object.
		''' </summary>
		''' <param name="sourceBean">  The bean to be given as the source for any events. </param>
		Public Sub New(  sourceBean As Object)
			If sourceBean Is Nothing Then Throw New NullPointerException
			source = sourceBean
		End Sub

		''' <summary>
		''' Add a VetoableChangeListener to the listener list.
		''' The listener is registered for all properties.
		''' The same listener object may be added more than once, and will be called
		''' as many times as it is added.
		''' If <code>listener</code> is null, no exception is thrown and no action
		''' is taken.
		''' </summary>
		''' <param name="listener">  The VetoableChangeListener to be added </param>
		Public Overridable Sub addVetoableChangeListener(  listener As VetoableChangeListener)
			If listener Is Nothing Then Return
			If TypeOf listener Is VetoableChangeListenerProxy Then
				Dim proxy As VetoableChangeListenerProxy = CType(listener, VetoableChangeListenerProxy)
				' Call two argument add method.
				addVetoableChangeListener(proxy.propertyName, proxy.listener)
			Else
				Me.map.add(Nothing, listener)
			End If
		End Sub

		''' <summary>
		''' Remove a VetoableChangeListener from the listener list.
		''' This removes a VetoableChangeListener that was registered
		''' for all properties.
		''' If <code>listener</code> was added more than once to the same event
		''' source, it will be notified one less time after being removed.
		''' If <code>listener</code> is null, or was never added, no exception is
		''' thrown and no action is taken.
		''' </summary>
		''' <param name="listener">  The VetoableChangeListener to be removed </param>
		Public Overridable Sub removeVetoableChangeListener(  listener As VetoableChangeListener)
			If listener Is Nothing Then Return
			If TypeOf listener Is VetoableChangeListenerProxy Then
				Dim proxy As VetoableChangeListenerProxy = CType(listener, VetoableChangeListenerProxy)
				' Call two argument remove method.
				removeVetoableChangeListener(proxy.propertyName, proxy.listener)
			Else
				Me.map.remove(Nothing, listener)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the listeners that were added to the
		''' VetoableChangeSupport object with addVetoableChangeListener().
		''' <p>
		''' If some listeners have been added with a named property, then
		''' the returned array will be a mixture of VetoableChangeListeners
		''' and <code>VetoableChangeListenerProxy</code>s. If the calling
		''' method is interested in distinguishing the listeners then it must
		''' test each element to see if it's a
		''' <code>VetoableChangeListenerProxy</code>, perform the cast, and examine
		''' the parameter.
		''' 
		''' <pre>{@code
		''' VetoableChangeListener[] listeners = bean.getVetoableChangeListeners();
		''' for (int i = 0; i < listeners.length; i++) {
		'''        if (listeners[i] instanceof VetoableChangeListenerProxy) {
		'''     VetoableChangeListenerProxy proxy =
		'''                    (VetoableChangeListenerProxy)listeners[i];
		'''     if (proxy.getPropertyName().equals("foo")) {
		'''       // proxy is a VetoableChangeListener which was associated
		'''       // with the property named "foo"
		'''     }
		'''   }
		''' }
		''' }</pre>
		''' </summary>
		''' <seealso cref= VetoableChangeListenerProxy </seealso>
		''' <returns> all of the <code>VetoableChangeListeners</code> added or an
		'''         empty array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property vetoableChangeListeners As VetoableChangeListener()
			Get
				Return Me.map.listeners
			End Get
		End Property

		''' <summary>
		''' Add a VetoableChangeListener for a specific property.  The listener
		''' will be invoked only when a call on fireVetoableChange names that
		''' specific property.
		''' The same listener object may be added more than once.  For each
		''' property,  the listener will be invoked the number of times it was added
		''' for that property.
		''' If <code>propertyName</code> or <code>listener</code> is null, no
		''' exception is thrown and no action is taken.
		''' </summary>
		''' <param name="propertyName">  The name of the property to listen on. </param>
		''' <param name="listener">  The VetoableChangeListener to be added </param>
		Public Overridable Sub addVetoableChangeListener(  propertyName As String,   listener As VetoableChangeListener)
			If listener Is Nothing OrElse propertyName Is Nothing Then Return
			listener = Me.map.extract(listener)
			If listener IsNot Nothing Then Me.map.add(propertyName, listener)
		End Sub

		''' <summary>
		''' Remove a VetoableChangeListener for a specific property.
		''' If <code>listener</code> was added more than once to the same event
		''' source for the specified property, it will be notified one less time
		''' after being removed.
		''' If <code>propertyName</code> is null, no exception is thrown and no
		''' action is taken.
		''' If <code>listener</code> is null, or was never added for the specified
		''' property, no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="propertyName">  The name of the property that was listened on. </param>
		''' <param name="listener">  The VetoableChangeListener to be removed </param>
		Public Overridable Sub removeVetoableChangeListener(  propertyName As String,   listener As VetoableChangeListener)
			If listener Is Nothing OrElse propertyName Is Nothing Then Return
			listener = Me.map.extract(listener)
			If listener IsNot Nothing Then Me.map.remove(propertyName, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the listeners which have been associated
		''' with the named property.
		''' </summary>
		''' <param name="propertyName">  The name of the property being listened to </param>
		''' <returns> all the <code>VetoableChangeListeners</code> associated with
		'''         the named property.  If no such listeners have been added,
		'''         or if <code>propertyName</code> is null, an empty array is
		'''         returned.
		''' @since 1.4 </returns>
		Public Overridable Function getVetoableChangeListeners(  propertyName As String) As VetoableChangeListener()
			Return Me.map.getListeners(propertyName)
		End Function

		''' <summary>
		''' Reports a constrained property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' Any listener can throw a {@code PropertyVetoException} to veto the update.
		''' If one of the listeners vetoes the update, this method passes
		''' a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		''' to all listeners that already confirmed this update
		''' and throws the {@code PropertyVetoException} again.
		''' <p>
		''' No event is fired if old and new values are equal and non-null.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#fireVetoableChange(PropertyChangeEvent)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that is about to change </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		''' <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
		Public Overridable Sub fireVetoableChange(  propertyName As String,   oldValue As Object,   newValue As Object)
			If oldValue Is Nothing OrElse newValue Is Nothing OrElse (Not oldValue.Equals(newValue)) Then fireVetoableChange(New PropertyChangeEvent(Me.source, propertyName, oldValue, newValue))
		End Sub

		''' <summary>
		''' Reports an integer constrained property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' Any listener can throw a {@code PropertyVetoException} to veto the update.
		''' If one of the listeners vetoes the update, this method passes
		''' a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		''' to all listeners that already confirmed this update
		''' and throws the {@code PropertyVetoException} again.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#fireVetoableChange(String, Object, Object)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that is about to change </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		''' <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
		Public Overridable Sub fireVetoableChange(  propertyName As String,   oldValue As Integer,   newValue As Integer)
			If oldValue <> newValue Then fireVetoableChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue))
		End Sub

		''' <summary>
		''' Reports a boolean constrained property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' Any listener can throw a {@code PropertyVetoException} to veto the update.
		''' If one of the listeners vetoes the update, this method passes
		''' a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		''' to all listeners that already confirmed this update
		''' and throws the {@code PropertyVetoException} again.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#fireVetoableChange(String, Object, Object)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that is about to change </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		''' <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
		Public Overridable Sub fireVetoableChange(  propertyName As String,   oldValue As Boolean,   newValue As Boolean)
			If oldValue <> newValue Then fireVetoableChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue))
		End Sub

		''' <summary>
		''' Fires a property change event to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' Any listener can throw a {@code PropertyVetoException} to veto the update.
		''' If one of the listeners vetoes the update, this method passes
		''' a new "undo" {@code PropertyChangeEvent} that reverts to the old value
		''' to all listeners that already confirmed this update
		''' and throws the {@code PropertyVetoException} again.
		''' <p>
		''' No event is fired if the given event's old and new values are equal and non-null.
		''' </summary>
		''' <param name="event">  the {@code PropertyChangeEvent} to be fired </param>
		''' <exception cref="PropertyVetoException"> if one of listeners vetoes the property update </exception>
		Public Overridable Sub fireVetoableChange(  [event] As PropertyChangeEvent)
			Dim oldValue As Object = [event].oldValue
			Dim newValue As Object = [event].newValue
			If oldValue Is Nothing OrElse newValue Is Nothing OrElse (Not oldValue.Equals(newValue)) Then
				Dim name As String = [event].propertyName

				Dim common As VetoableChangeListener() = Me.map.get(Nothing)
				Dim named As VetoableChangeListener() = If(name IsNot Nothing, Me.map.get(name), Nothing)

				Dim listeners As VetoableChangeListener()
				If common Is Nothing Then
					listeners = named
				ElseIf named Is Nothing Then
					listeners = common
				Else
					listeners = New VetoableChangeListener(common.Length + named.Length - 1){}
					Array.Copy(common, 0, listeners, 0, common.Length)
					Array.Copy(named, 0, listeners, common.Length, named.Length)
				End If
				If listeners IsNot Nothing Then
					Dim current As Integer = 0
					Try
						Do While current < listeners.Length
							listeners(current).vetoableChange([event])
							current += 1
						Loop
					Catch veto As PropertyVetoException
						[event] = New PropertyChangeEvent(Me.source, name, newValue, oldValue)
						For i As Integer = 0 To current - 1
							Try
								listeners(i).vetoableChange([event])
							Catch exception_Renamed As PropertyVetoException
								' ignore exceptions that occur during rolling back
							End Try
						Next i
						Throw veto ' rethrow the veto exception
					End Try
				End If
			End If
		End Sub

		''' <summary>
		''' Check if there are any listeners for a specific property, including
		''' those registered on all properties.  If <code>propertyName</code>
		''' is null, only check for listeners registered on all properties.
		''' </summary>
		''' <param name="propertyName">  the property name. </param>
		''' <returns> true if there are one or more listeners for the given property </returns>
		Public Overridable Function hasListeners(  propertyName As String) As Boolean
			Return Me.map.hasListeners(propertyName)
		End Function

		''' <summary>
		''' @serialData Null terminated list of <code>VetoableChangeListeners</code>.
		''' <p>
		''' At serialization time we skip non-serializable listeners and
		''' only serialize the serializable listeners.
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			Dim children As Dictionary(Of String, VetoableChangeSupport) = Nothing
			Dim listeners As VetoableChangeListener() = Nothing
			SyncLock Me.map
				For Each entry As KeyValuePair(Of String, VetoableChangeListener()) In Me.map.entries
					Dim [property] As String = entry.Key
					If [property] Is Nothing Then
						listeners = entry.Value
					Else
						If children Is Nothing Then children = New Dictionary(Of )
						Dim vcs As New VetoableChangeSupport(Me.source)
						vcs.map.set(Nothing, entry.Value)
						children([property]) = vcs
					End If
				Next entry
			End SyncLock
			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("children", children)
			fields.put("source", Me.source)
			fields.put("vetoableChangeSupportSerializedDataVersion", 2)
			s.writeFields()

			If listeners IsNot Nothing Then
				For Each l As VetoableChangeListener In listeners
					If TypeOf l Is java.io.Serializable Then s.writeObject(l)
				Next l
			End If
			s.writeObject(Nothing)
		End Sub

		Private Sub readObject(  s As java.io.ObjectInputStream)
			Me.map = New VetoableChangeListenerMap

			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim children As Dictionary(Of String, VetoableChangeSupport) = CType(fields.get("children", Nothing), Dictionary(Of String, VetoableChangeSupport))
			Me.source = fields.get("source", Nothing)
			fields.get("vetoableChangeSupportSerializedDataVersion", 2)

			Dim listenerOrNull As Object
			listenerOrNull = s.readObject()
			Do While Nothing IsNot listenerOrNull
				Me.map.add(Nothing, CType(listenerOrNull, VetoableChangeListener))
				listenerOrNull = s.readObject()
			Loop
			If children IsNot Nothing Then
				For Each entry As KeyValuePair(Of String, VetoableChangeSupport) In children
					For Each listener As VetoableChangeListener In entry.Value.vetoableChangeListeners
						Me.map.add(entry.Key, listener)
					Next listener
				Next entry
			End If
		End Sub

		''' <summary>
		''' The object to be provided as the "source" for any generated events.
		''' </summary>
		Private source As Object

		''' <summary>
		''' @serialField children                                   Hashtable
		''' @serialField source                                     Object
		''' @serialField vetoableChangeSupportSerializedDataVersion int
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("children", GetType(Hashtable)), New java.io.ObjectStreamField("source", GetType(Object)), New java.io.ObjectStreamField("vetoableChangeSupportSerializedDataVersion",  java.lang.[Integer].TYPE) }

		''' <summary>
		''' Serialization version ID, so we're compatible with JDK 1.1
		''' </summary>
		Friend Const serialVersionUID As Long = -5090210921595982017L

		''' <summary>
		''' This is a <seealso cref="ChangeListenerMap ChangeListenerMap"/> implementation
		''' that works with <seealso cref="VetoableChangeListener VetoableChangeListener"/> objects.
		''' </summary>
		Private NotInheritable Class VetoableChangeListenerMap
			Inherits ChangeListenerMap(Of VetoableChangeListener)

			Private Shared ReadOnly EMPTY As VetoableChangeListener() = {}

			''' <summary>
			''' Creates an array of <seealso cref="VetoableChangeListener VetoableChangeListener"/> objects.
			''' This method uses the same instance of the empty array
			''' when {@code length} equals {@code 0}.
			''' </summary>
			''' <param name="length">  the array length </param>
			''' <returns>        an array with specified length </returns>
			Protected Friend Overrides Function newArray(  length As Integer) As VetoableChangeListener()
				Return If(0 < length, New VetoableChangeListener(length - 1){}, EMPTY)
			End Function

			''' <summary>
			''' Creates a <seealso cref="VetoableChangeListenerProxy VetoableChangeListenerProxy"/>
			''' object for the specified property.
			''' </summary>
			''' <param name="name">      the name of the property to listen on </param>
			''' <param name="listener">  the listener to process events </param>
			''' <returns>          a {@code VetoableChangeListenerProxy} object </returns>
			Protected Friend Overrides Function newProxy(  name As String,   listener As VetoableChangeListener) As VetoableChangeListener
				Return New VetoableChangeListenerProxy(name, listener)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Function extract(  listener As VetoableChangeListener) As VetoableChangeListener
				Do While TypeOf listener Is VetoableChangeListenerProxy
					listener = CType(listener, VetoableChangeListenerProxy).listener
				Loop
				Return listener
			End Function
		End Class
	End Class

End Namespace