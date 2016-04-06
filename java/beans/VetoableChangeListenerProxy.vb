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
	''' specifically for adding a {@code VetoableChangeListener}
	''' with a "constrained" property.
	''' Instances of this class can be added
	''' as {@code VetoableChangeListener}s to a bean
	''' which supports firing vetoable change events.
	''' <p>
	''' If the object has a {@code getVetoableChangeListeners} method
	''' then the array returned could be a mixture of {@code VetoableChangeListener}
	''' and {@code VetoableChangeListenerProxy} objects.
	''' </summary>
	''' <seealso cref= java.util.EventListenerProxy </seealso>
	''' <seealso cref= VetoableChangeSupport#getVetoableChangeListeners
	''' @since 1.4 </seealso>
	Public Class VetoableChangeListenerProxy
		Inherits java.util.EventListenerProxy(Of VetoableChangeListener)
		Implements VetoableChangeListener

		Private ReadOnly propertyName As String

		''' <summary>
		''' Constructor which binds the {@code VetoableChangeListener}
		''' to a specific property.
		''' </summary>
		''' <param name="propertyName">  the name of the property to listen on </param>
		''' <param name="listener">      the listener object </param>
		Public Sub New(  propertyName As String,   listener As VetoableChangeListener)
			MyBase.New(listener)
			Me.propertyName = propertyName
		End Sub

		''' <summary>
		''' Forwards the property change event to the listener delegate.
		''' </summary>
		''' <param name="event">  the property change event
		''' </param>
		''' <exception cref="PropertyVetoException"> if the recipient wishes the property
		'''                                  change to be rolled back </exception>
		Public Overridable Sub vetoableChange(  [event] As PropertyChangeEvent) Implements VetoableChangeListener.vetoableChange
			listener.vetoableChange([event])
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