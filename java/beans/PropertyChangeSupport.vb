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
	''' This is a utility class that can be used by beans that support bound
	''' properties.  It manages a list of listeners and dispatches
	''' <seealso cref="PropertyChangeEvent"/>s to them.  You can use an instance of this class
	''' as a member field of your bean and delegate these types of work to it.
	''' The <seealso cref="PropertyChangeListener"/> can be registered for all properties
	''' or for a property specified by name.
	''' <p>
	''' Here is an example of {@code PropertyChangeSupport} usage that follows
	''' the rules and recommendations laid out in the JavaBeans&trade; specification:
	''' <pre>
	''' public class MyBean {
	'''     private final PropertyChangeSupport pcs = new PropertyChangeSupport(this);
	''' 
	'''     public  Sub  addPropertyChangeListener(PropertyChangeListener listener) {
	'''         this.pcs.addPropertyChangeListener(listener);
	'''     }
	''' 
	'''     public  Sub  removePropertyChangeListener(PropertyChangeListener listener) {
	'''         this.pcs.removePropertyChangeListener(listener);
	'''     }
	''' 
	'''     private String value;
	''' 
	'''     public String getValue() {
	'''         return this.value;
	'''     }
	''' 
	'''     public  Sub  setValue(String newValue) {
	'''         String oldValue = this.value;
	'''         this.value = newValue;
	'''         this.pcs.firePropertyChange("value", oldValue, newValue);
	'''     }
	''' 
	'''     [...]
	''' }
	''' </pre>
	''' <p>
	''' A {@code PropertyChangeSupport} instance is thread-safe.
	''' <p>
	''' This class is serializable.  When it is serialized it will save
	''' (and restore) any listeners that are themselves serializable.  Any
	''' non-serializable listeners will be skipped during serialization.
	''' </summary>
	''' <seealso cref= VetoableChangeSupport </seealso>
	<Serializable> _
	Public Class PropertyChangeSupport
		Private map As New PropertyChangeListenerMap

		''' <summary>
		''' Constructs a <code>PropertyChangeSupport</code> object.
		''' </summary>
		''' <param name="sourceBean">  The bean to be given as the source for any events. </param>
		Public Sub New(  sourceBean As Object)
			If sourceBean Is Nothing Then Throw New NullPointerException
			source = sourceBean
		End Sub

		''' <summary>
		''' Add a PropertyChangeListener to the listener list.
		''' The listener is registered for all properties.
		''' The same listener object may be added more than once, and will be called
		''' as many times as it is added.
		''' If <code>listener</code> is null, no exception is thrown and no action
		''' is taken.
		''' </summary>
		''' <param name="listener">  The PropertyChangeListener to be added </param>
		Public Overridable Sub addPropertyChangeListener(  listener As PropertyChangeListener)
			If listener Is Nothing Then Return
			If TypeOf listener Is PropertyChangeListenerProxy Then
				Dim proxy As PropertyChangeListenerProxy = CType(listener, PropertyChangeListenerProxy)
				' Call two argument add method.
				addPropertyChangeListener(proxy.propertyName, proxy.listener)
			Else
				Me.map.add(Nothing, listener)
			End If
		End Sub

		''' <summary>
		''' Remove a PropertyChangeListener from the listener list.
		''' This removes a PropertyChangeListener that was registered
		''' for all properties.
		''' If <code>listener</code> was added more than once to the same event
		''' source, it will be notified one less time after being removed.
		''' If <code>listener</code> is null, or was never added, no exception is
		''' thrown and no action is taken.
		''' </summary>
		''' <param name="listener">  The PropertyChangeListener to be removed </param>
		Public Overridable Sub removePropertyChangeListener(  listener As PropertyChangeListener)
			If listener Is Nothing Then Return
			If TypeOf listener Is PropertyChangeListenerProxy Then
				Dim proxy As PropertyChangeListenerProxy = CType(listener, PropertyChangeListenerProxy)
				' Call two argument remove method.
				removePropertyChangeListener(proxy.propertyName, proxy.listener)
			Else
				Me.map.remove(Nothing, listener)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the listeners that were added to the
		''' PropertyChangeSupport object with addPropertyChangeListener().
		''' <p>
		''' If some listeners have been added with a named property, then
		''' the returned array will be a mixture of PropertyChangeListeners
		''' and <code>PropertyChangeListenerProxy</code>s. If the calling
		''' method is interested in distinguishing the listeners then it must
		''' test each element to see if it's a
		''' <code>PropertyChangeListenerProxy</code>, perform the cast, and examine
		''' the parameter.
		''' 
		''' <pre>{@code
		''' PropertyChangeListener[] listeners = bean.getPropertyChangeListeners();
		''' for (int i = 0; i < listeners.length; i++) {
		'''   if (listeners[i] instanceof PropertyChangeListenerProxy) {
		'''     PropertyChangeListenerProxy proxy =
		'''                    (PropertyChangeListenerProxy)listeners[i];
		'''     if (proxy.getPropertyName().equals("foo")) {
		'''       // proxy is a PropertyChangeListener which was associated
		'''       // with the property named "foo"
		'''     }
		'''   }
		''' }
		''' }</pre>
		''' </summary>
		''' <seealso cref= PropertyChangeListenerProxy </seealso>
		''' <returns> all of the <code>PropertyChangeListeners</code> added or an
		'''         empty array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property propertyChangeListeners As PropertyChangeListener()
			Get
				Return Me.map.listeners
			End Get
		End Property

		''' <summary>
		''' Add a PropertyChangeListener for a specific property.  The listener
		''' will be invoked only when a call on firePropertyChange names that
		''' specific property.
		''' The same listener object may be added more than once.  For each
		''' property,  the listener will be invoked the number of times it was added
		''' for that property.
		''' If <code>propertyName</code> or <code>listener</code> is null, no
		''' exception is thrown and no action is taken.
		''' </summary>
		''' <param name="propertyName">  The name of the property to listen on. </param>
		''' <param name="listener">  The PropertyChangeListener to be added </param>
		Public Overridable Sub addPropertyChangeListener(  propertyName As String,   listener As PropertyChangeListener)
			If listener Is Nothing OrElse propertyName Is Nothing Then Return
			listener = Me.map.extract(listener)
			If listener IsNot Nothing Then Me.map.add(propertyName, listener)
		End Sub

		''' <summary>
		''' Remove a PropertyChangeListener for a specific property.
		''' If <code>listener</code> was added more than once to the same event
		''' source for the specified property, it will be notified one less time
		''' after being removed.
		''' If <code>propertyName</code> is null,  no exception is thrown and no
		''' action is taken.
		''' If <code>listener</code> is null, or was never added for the specified
		''' property, no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="propertyName">  The name of the property that was listened on. </param>
		''' <param name="listener">  The PropertyChangeListener to be removed </param>
		Public Overridable Sub removePropertyChangeListener(  propertyName As String,   listener As PropertyChangeListener)
			If listener Is Nothing OrElse propertyName Is Nothing Then Return
			listener = Me.map.extract(listener)
			If listener IsNot Nothing Then Me.map.remove(propertyName, listener)
		End Sub

		''' <summary>
		''' Returns an array of all the listeners which have been associated
		''' with the named property.
		''' </summary>
		''' <param name="propertyName">  The name of the property being listened to </param>
		''' <returns> all of the <code>PropertyChangeListeners</code> associated with
		'''         the named property.  If no such listeners have been added,
		'''         or if <code>propertyName</code> is null, an empty array is
		'''         returned.
		''' @since 1.4 </returns>
		Public Overridable Function getPropertyChangeListeners(  propertyName As String) As PropertyChangeListener()
			Return Me.map.getListeners(propertyName)
		End Function

		''' <summary>
		''' Reports a bound property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal and non-null.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#firePropertyChange(PropertyChangeEvent)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		Public Overridable Sub firePropertyChange(  propertyName As String,   oldValue As Object,   newValue As Object)
			If oldValue Is Nothing OrElse newValue Is Nothing OrElse (Not oldValue.Equals(newValue)) Then firePropertyChange(New PropertyChangeEvent(Me.source, propertyName, oldValue, newValue))
		End Sub

		''' <summary>
		''' Reports an integer bound property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#firePropertyChange(String, Object, Object)"/>  method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		Public Overridable Sub firePropertyChange(  propertyName As String,   oldValue As Integer,   newValue As Integer)
			If oldValue <> newValue Then firePropertyChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue))
		End Sub

		''' <summary>
		''' Reports a boolean bound property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#firePropertyChange(String, Object, Object)"/>  method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property </param>
		Public Overridable Sub firePropertyChange(  propertyName As String,   oldValue As Boolean,   newValue As Boolean)
			If oldValue <> newValue Then firePropertyChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue))
		End Sub

		''' <summary>
		''' Fires a property change event to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if the given event's old and new values are equal and non-null.
		''' </summary>
		''' <param name="event">  the {@code PropertyChangeEvent} to be fired </param>
		Public Overridable Sub firePropertyChange(  [event] As PropertyChangeEvent)
			Dim oldValue As Object = [event].oldValue
			Dim newValue As Object = [event].newValue
			If oldValue Is Nothing OrElse newValue Is Nothing OrElse (Not oldValue.Equals(newValue)) Then
				Dim name As String = [event].propertyName

				Dim common As PropertyChangeListener() = Me.map.get(Nothing)
				Dim named As PropertyChangeListener() = If(name IsNot Nothing, Me.map.get(name), Nothing)

				fire(common, [event])
				fire(named, [event])
			End If
		End Sub

		Private Shared Sub fire(  listeners As PropertyChangeListener(),   [event] As PropertyChangeEvent)
			If listeners IsNot Nothing Then
				For Each listener As PropertyChangeListener In listeners
					listener.propertyChange([event])
				Next listener
			End If
		End Sub

		''' <summary>
		''' Reports a bound indexed property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal and non-null.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#firePropertyChange(PropertyChangeEvent)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="index">         the index of the property element that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property
		''' @since 1.5 </param>
		Public Overridable Sub fireIndexedPropertyChange(  propertyName As String,   index As Integer,   oldValue As Object,   newValue As Object)
			If oldValue Is Nothing OrElse newValue Is Nothing OrElse (Not oldValue.Equals(newValue)) Then firePropertyChange(New IndexedPropertyChangeEvent(source, propertyName, oldValue, newValue, index))
		End Sub

		''' <summary>
		''' Reports an integer bound indexed property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#fireIndexedPropertyChange(String, int, Object, Object)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="index">         the index of the property element that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property
		''' @since 1.5 </param>
		Public Overridable Sub fireIndexedPropertyChange(  propertyName As String,   index As Integer,   oldValue As Integer,   newValue As Integer)
			If oldValue <> newValue Then fireIndexedPropertyChange(propertyName, index, Convert.ToInt32(oldValue), Convert.ToInt32(newValue))
		End Sub

		''' <summary>
		''' Reports a boolean bound indexed property update to listeners
		''' that have been registered to track updates of
		''' all properties or a property with the specified name.
		''' <p>
		''' No event is fired if old and new values are equal.
		''' <p>
		''' This is merely a convenience wrapper around the more general
		''' <seealso cref="#fireIndexedPropertyChange(String, int, Object, Object)"/> method.
		''' </summary>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="index">         the index of the property element that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property
		''' @since 1.5 </param>
		Public Overridable Sub fireIndexedPropertyChange(  propertyName As String,   index As Integer,   oldValue As Boolean,   newValue As Boolean)
			If oldValue <> newValue Then fireIndexedPropertyChange(propertyName, index, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue))
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
		''' @serialData Null terminated list of <code>PropertyChangeListeners</code>.
		''' <p>
		''' At serialization time we skip non-serializable listeners and
		''' only serialize the serializable listeners.
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			Dim children As Dictionary(Of String, PropertyChangeSupport) = Nothing
			Dim listeners As PropertyChangeListener() = Nothing
			SyncLock Me.map
				For Each entry As KeyValuePair(Of String, PropertyChangeListener()) In Me.map.entries
					Dim [property] As String = entry.Key
					If [property] Is Nothing Then
						listeners = entry.Value
					Else
						If children Is Nothing Then children = New Dictionary(Of )
						Dim pcs As New PropertyChangeSupport(Me.source)
						pcs.map.set(Nothing, entry.Value)
						children([property]) = pcs
					End If
				Next entry
			End SyncLock
			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("children", children)
			fields.put("source", Me.source)
			fields.put("propertyChangeSupportSerializedDataVersion", 2)
			s.writeFields()

			If listeners IsNot Nothing Then
				For Each l As PropertyChangeListener In listeners
					If TypeOf l Is java.io.Serializable Then s.writeObject(l)
				Next l
			End If
			s.writeObject(Nothing)
		End Sub

		Private Sub readObject(  s As java.io.ObjectInputStream)
			Me.map = New PropertyChangeListenerMap

			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim children As Dictionary(Of String, PropertyChangeSupport) = CType(fields.get("children", Nothing), Dictionary(Of String, PropertyChangeSupport))
			Me.source = fields.get("source", Nothing)
			fields.get("propertyChangeSupportSerializedDataVersion", 2)

			Dim listenerOrNull As Object
			listenerOrNull = s.readObject()
			Do While Nothing IsNot listenerOrNull
				Me.map.add(Nothing, CType(listenerOrNull, PropertyChangeListener))
				listenerOrNull = s.readObject()
			Loop
			If children IsNot Nothing Then
				For Each entry As KeyValuePair(Of String, PropertyChangeSupport) In children
					For Each listener As PropertyChangeListener In entry.Value.propertyChangeListeners
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
		''' @serialField propertyChangeSupportSerializedDataVersion int
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("children", GetType(Hashtable)), New java.io.ObjectStreamField("source", GetType(Object)), New java.io.ObjectStreamField("propertyChangeSupportSerializedDataVersion",  java.lang.[Integer].TYPE) }

		''' <summary>
		''' Serialization version ID, so we're compatible with JDK 1.1
		''' </summary>
		Friend Const serialVersionUID As Long = 6401253773779951803L

		''' <summary>
		''' This is a <seealso cref="ChangeListenerMap ChangeListenerMap"/> implementation
		''' that works with <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
		''' </summary>
		Private NotInheritable Class PropertyChangeListenerMap
			Inherits ChangeListenerMap(Of PropertyChangeListener)

			Private Shared ReadOnly EMPTY As PropertyChangeListener() = {}

			''' <summary>
			''' Creates an array of <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
			''' This method uses the same instance of the empty array
			''' when {@code length} equals {@code 0}.
			''' </summary>
			''' <param name="length">  the array length </param>
			''' <returns>        an array with specified length </returns>
			Protected Friend Overrides Function newArray(  length As Integer) As PropertyChangeListener()
				Return If(0 < length, New PropertyChangeListener(length - 1){}, EMPTY)
			End Function

			''' <summary>
			''' Creates a <seealso cref="PropertyChangeListenerProxy PropertyChangeListenerProxy"/>
			''' object for the specified property.
			''' </summary>
			''' <param name="name">      the name of the property to listen on </param>
			''' <param name="listener">  the listener to process events </param>
			''' <returns>          a {@code PropertyChangeListenerProxy} object </returns>
			Protected Friend Overrides Function newProxy(  name As String,   listener As PropertyChangeListener) As PropertyChangeListener
				Return New PropertyChangeListenerProxy(name, listener)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Function extract(  listener As PropertyChangeListener) As PropertyChangeListener
				Do While TypeOf listener Is PropertyChangeListenerProxy
					listener = CType(listener, PropertyChangeListenerProxy).listener
				Loop
				Return listener
			End Function
		End Class
	End Class

End Namespace