'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A class which extends the {@code EventListenerProxy}
	''' specifically for adding a {@code PropertyChangeListener}
	''' with a "bound" property.
	''' Instances of this class can be added
	''' as {@code PropertyChangeListener}s to a bean
	''' which supports firing property change events.
	''' <p>
	''' If the object has a {@code getPropertyChangeListeners} method
	''' then the array returned could be a mixture of {@code PropertyChangeListener}
	''' and {@code PropertyChangeListenerProxy} objects.
	''' </summary>
	''' <seealso cref= java.util.EventListenerProxy </seealso>
	''' <seealso cref= PropertyChangeSupport#getPropertyChangeListeners
	''' @since 1.4 </seealso>
	Public Class PropertyChangeListenerProxy
		Inherits java.util.EventListenerProxy(Of PropertyChangeListener)
		Implements PropertyChangeListener

		Private ReadOnly propertyName As String

		''' <summary>
		''' Constructor which binds the {@code PropertyChangeListener}
		''' to a specific property.
		''' </summary>
		''' <param name="propertyName">  the name of the property to listen on </param>
		''' <param name="listener">      the listener object </param>
		Public Sub New(  propertyName As String,   listener As PropertyChangeListener)
			MyBase.New(listener)
			Me.propertyName = propertyName
		End Sub

		''' <summary>
		''' Forwards the property change event to the listener delegate.
		''' </summary>
		''' <param name="event">  the property change event </param>
		Public Overridable Sub propertyChange(  [event] As PropertyChangeEvent) Implements PropertyChangeListener.propertyChange
			listener.propertyChange([event])
		End Sub

		''' <summary>
		''' Returns the name of the named property associated with the listener.
		''' </summary>
		''' <returns> the name of the named property associated with the listener </returns>
		Public Overridable Property propertyName As String
			Get
				Return Me.propertyName
			End Get
		End Property
	End Class

End Namespace