Imports System
Imports System.Runtime.CompilerServices

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
	''' This class provides default implementations for the JFC <code>Action</code>
	''' interface. Standard behaviors like the get and set methods for
	''' <code>Action</code> object properties (icon, text, and enabled) are defined
	''' here. The developer need only subclass this abstract class and
	''' define the <code>actionPerformed</code> method.
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
	''' @author Georges Saab </summary>
	''' <seealso cref= Action </seealso>
	<Serializable> _
	Public MustInherit Class AbstractAction
		Implements Action, ICloneable

			Public MustOverride WriteOnly Property enabled Implements Action.setEnabled As Boolean
			Public MustOverride Sub putValue(ByVal key As String, ByVal value As Object) Implements Action.putValue
		''' <summary>
		''' Whether or not actions should reconfigure all properties on null.
		''' </summary>
		Private Shared RECONFIGURE_ON_NULL As Boolean?

		''' <summary>
		''' Specifies whether action is enabled; the default is true.
		''' </summary>
		Protected Friend enabled As Boolean = True


		''' <summary>
		''' Contains the array of key bindings.
		''' </summary>
		<NonSerialized> _
		Private arrayTable As ArrayTable

		''' <summary>
		''' Whether or not to reconfigure all action properties from the
		''' specified event.
		''' </summary>
		Friend Shared Function shouldReconfigure(ByVal e As PropertyChangeEvent) As Boolean
			If e.propertyName Is Nothing Then
				SyncLock GetType(AbstractAction)
					If RECONFIGURE_ON_NULL Is Nothing Then RECONFIGURE_ON_NULL = Convert.ToBoolean(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.actions.reconfigureOnNull", "false")))
					Return RECONFIGURE_ON_NULL
				End SyncLock
			End If
			Return False
		End Function

		''' <summary>
		''' Sets the enabled state of a component from an Action.
		''' </summary>
		''' <param name="c"> the Component to set the enabled state on </param>
		''' <param name="a"> the Action to set the enabled state from, may be null </param>
		Friend Shared Sub setEnabledFromAction(ByVal c As JComponent, ByVal a As Action)
			c.enabled = If(a IsNot Nothing, a.enabled, True)
		End Sub

		''' <summary>
		''' Sets the tooltip text of a component from an Action.
		''' </summary>
		''' <param name="c"> the Component to set the tooltip text on </param>
		''' <param name="a"> the Action to set the tooltip text from, may be null </param>
		Friend Shared Sub setToolTipTextFromAction(ByVal c As JComponent, ByVal a As Action)
			c.toolTipText = If(a IsNot Nothing, CStr(a.getValue(Action.SHORT_DESCRIPTION)), Nothing)
		End Sub

		Friend Shared Function hasSelectedKey(ByVal a As Action) As Boolean
			Return (a IsNot Nothing AndAlso a.getValue(Action.SELECTED_KEY) IsNot Nothing)
		End Function

		Friend Shared Function isSelected(ByVal a As Action) As Boolean
			Return Boolean.TRUE.Equals(a.getValue(Action.SELECTED_KEY))
		End Function



		''' <summary>
		''' Creates an {@code Action}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates an {@code Action} with the specified name.
		''' </summary>
		''' <param name="name"> the name ({@code Action.NAME}) for the action; a
		'''        value of {@code null} is ignored </param>
		Public Sub New(ByVal name As String)
			putValue(Action.NAME, name)
		End Sub

		''' <summary>
		''' Creates an {@code Action} with the specified name and small icon.
		''' </summary>
		''' <param name="name"> the name ({@code Action.NAME}) for the action; a
		'''        value of {@code null} is ignored </param>
		''' <param name="icon"> the small icon ({@code Action.SMALL_ICON}) for the action; a
		'''        value of {@code null} is ignored </param>
		Public Sub New(ByVal name As String, ByVal icon As Icon)
			Me.New(name)
			putValue(Action.SMALL_ICON, icon)
		End Sub

		''' <summary>
		''' Gets the <code>Object</code> associated with the specified key.
		''' </summary>
		''' <param name="key"> a string containing the specified <code>key</code> </param>
		''' <returns> the binding <code>Object</code> stored with this key; if there
		'''          are no keys, it will return <code>null</code> </returns>
		''' <seealso cref= Action#getValue </seealso>
		Public Overridable Function getValue(ByVal key As String) As Object Implements Action.getValue
			If key = "enabled" Then Return enabled
			If arrayTable Is Nothing Then Return Nothing
			Return arrayTable.get(key)
		End Function

		''' <summary>
		''' Sets the <code>Value</code> associated with the specified key.
		''' </summary>
		''' <param name="key">  the <code>String</code> that identifies the stored object </param>
		''' <param name="newValue"> the <code>Object</code> to store using this key </param>
		''' <seealso cref= Action#putValue </seealso>
		Public Overridable Sub putValue(ByVal key As String, ByVal newValue As Object) Implements Action.putValue
			Dim oldValue As Object = Nothing
			If key = "enabled" Then
				' Treat putValue("enabled") the same way as a call to setEnabled.
				' If we don't do this it means the two may get out of sync, and a
				' bogus property change notification would be sent.
				'
				' To avoid dependencies between putValue & setEnabled this
				' directly changes enabled. If we instead called setEnabled
				' to change enabled, it would be possible for stack
				' overflow in the case where a developer implemented setEnabled
				' in terms of putValue.
				If newValue Is Nothing OrElse Not(TypeOf newValue Is Boolean?) Then newValue = False
				oldValue = enabled
				enabled = CBool(newValue)
			Else
				If arrayTable Is Nothing Then arrayTable = New ArrayTable
				If arrayTable.containsKey(key) Then oldValue = arrayTable.get(key)
				' Remove the entry for key if newValue is null
				' else put in the newValue for key.
				If newValue Is Nothing Then
					arrayTable.remove(key)
				Else
					arrayTable.put(key,newValue)
				End If
			End If
			firePropertyChange(key, oldValue, newValue)
		End Sub

		''' <summary>
		''' Returns true if the action is enabled.
		''' </summary>
		''' <returns> true if the action is enabled, false otherwise </returns>
		''' <seealso cref= Action#isEnabled </seealso>
		Public Overridable Property enabled As Boolean Implements Action.isEnabled
			Get
				Return enabled
			End Get
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = Me.enabled
    
				If oldValue <> newValue Then
					Me.enabled = newValue
					firePropertyChange("enabled", Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue))
				End If
			End Set
		End Property



		''' <summary>
		''' Returns an array of <code>Object</code>s which are keys for
		''' which values have been set for this <code>AbstractAction</code>,
		''' or <code>null</code> if no keys have values set. </summary>
		''' <returns> an array of key objects, or <code>null</code> if no
		'''                  keys have values set
		''' @since 1.3 </returns>
		Public Overridable Property keys As Object()
			Get
				If arrayTable Is Nothing Then Return Nothing
				Dim ___keys As Object() = New Object(arrayTable.size() - 1){}
				arrayTable.getKeys(___keys)
				Return ___keys
			End Get
		End Property

		''' <summary>
		''' If any <code>PropertyChangeListeners</code> have been registered, the
		''' <code>changeSupport</code> field describes them.
		''' </summary>
		Protected Friend changeSupport As javax.swing.event.SwingPropertyChangeSupport

		''' <summary>
		''' Supports reporting bound property changes.  This method can be called
		''' when a bound property has changed and it will send the appropriate
		''' <code>PropertyChangeEvent</code> to any registered
		''' <code>PropertyChangeListeners</code>.
		''' </summary>
		Protected Friend Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If changeSupport Is Nothing OrElse (oldValue IsNot Nothing AndAlso newValue IsNot Nothing AndAlso oldValue.Equals(newValue)) Then Return
			changeSupport.firePropertyChange(propertyName, oldValue, newValue)
		End Sub


		''' <summary>
		''' Adds a <code>PropertyChangeListener</code> to the listener list.
		''' The listener is registered for all properties.
		''' <p>
		''' A <code>PropertyChangeEvent</code> will get fired in response to setting
		''' a bound property, e.g. <code>setFont</code>, <code>setBackground</code>,
		''' or <code>setForeground</code>.
		''' Note that if the current component is inheriting its foreground,
		''' background, or font from its container, then no event will be
		''' fired in response to a change in the inherited property.
		''' </summary>
		''' <param name="listener">  The <code>PropertyChangeListener</code> to be added
		''' </param>
		''' <seealso cref= Action#addPropertyChangeListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(ByVal listener As PropertyChangeListener) Implements Action.addPropertyChangeListener
			If changeSupport Is Nothing Then changeSupport = New javax.swing.event.SwingPropertyChangeSupport(Me)
			changeSupport.addPropertyChangeListener(listener)
		End Sub


		''' <summary>
		''' Removes a <code>PropertyChangeListener</code> from the listener list.
		''' This removes a <code>PropertyChangeListener</code> that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  the <code>PropertyChangeListener</code> to be removed
		''' </param>
		''' <seealso cref= Action#removePropertyChangeListener </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(ByVal listener As PropertyChangeListener) Implements Action.removePropertyChangeListener
			If changeSupport Is Nothing Then Return
			changeSupport.removePropertyChangeListener(listener)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>PropertyChangeListener</code>s added
		''' to this AbstractAction with addPropertyChangeListener().
		''' </summary>
		''' <returns> all of the <code>PropertyChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property propertyChangeListeners As PropertyChangeListener()
			Get
				If changeSupport Is Nothing Then Return New PropertyChangeListener(){}
				Return changeSupport.propertyChangeListeners
			End Get
		End Property


		''' <summary>
		''' Clones the abstract action. This gives the clone
		''' its own copy of the key/value list,
		''' which is not handled for you by <code>Object.clone()</code>.
		''' 
		''' </summary>

		Protected Friend Overridable Function clone() As Object
			Dim newAction As AbstractAction = CType(MyBase.clone(), AbstractAction)
			SyncLock Me
				If arrayTable IsNot Nothing Then newAction.arrayTable = CType(arrayTable.clone(), ArrayTable)
			End SyncLock
			Return newAction
		End Function

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Store the default fields
			s.defaultWriteObject()

			' And the keys
			ArrayTable.writeArrayTable(s, arrayTable)
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			For counter As Integer = s.readInt() - 1 To 0 Step -1
				putValue(CStr(s.readObject()), s.readObject())
			Next counter
		End Sub
	End Class

End Namespace