'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.event



	''' <summary>
	''' This subclass of {@code java.beans.PropertyChangeSupport} is almost
	''' identical in functionality. The only difference is if constructed with
	''' {@code SwingPropertyChangeSupport(sourceBean, true)} it ensures
	''' listeners are only ever notified on the <i>Event Dispatch Thread</i>.
	''' 
	''' @author Igor Kushnirskiy
	''' </summary>

	Public NotInheritable Class SwingPropertyChangeSupport
		Inherits java.beans.PropertyChangeSupport

		''' <summary>
		''' Constructs a SwingPropertyChangeSupport object.
		''' </summary>
		''' <param name="sourceBean">  The bean to be given as the source for any
		'''        events. </param>
		''' <exception cref="NullPointerException"> if {@code sourceBean} is
		'''         {@code null} </exception>
		Public Sub New(ByVal sourceBean As Object)
			Me.New(sourceBean, False)
		End Sub

		''' <summary>
		''' Constructs a SwingPropertyChangeSupport object.
		''' </summary>
		''' <param name="sourceBean"> the bean to be given as the source for any events </param>
		''' <param name="notifyOnEDT"> whether to notify listeners on the <i>Event
		'''        Dispatch Thread</i> only
		''' </param>
		''' <exception cref="NullPointerException"> if {@code sourceBean} is
		'''         {@code null}
		''' @since 1.6 </exception>
		Public Sub New(ByVal sourceBean As Object, ByVal notifyOnEDT As Boolean)
			MyBase.New(sourceBean)
			Me.notifyOnEDT = notifyOnEDT
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>
		''' If <seealso cref="#isNotifyOnEDT"/> is {@code true} and called off the
		''' <i>Event Dispatch Thread</i> this implementation uses
		''' {@code SwingUtilities.invokeLater} to send out the notification
		''' on the <i>Event Dispatch Thread</i>. This ensures  listeners
		''' are only ever notified on the <i>Event Dispatch Thread</i>.
		''' </summary>
		''' <exception cref="NullPointerException"> if {@code evt} is
		'''         {@code null}
		''' @since 1.6 </exception>
		Public Sub firePropertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			If evt Is Nothing Then Throw New NullPointerException
			If (Not notifyOnEDT) OrElse javax.swing.SwingUtilities.eventDispatchThread Then
				MyBase.firePropertyChange(evt)
			Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				javax.swing.SwingUtilities.invokeLater(New Runnable()
	'			{
	'					public void run()
	'					{
	'						firePropertyChange(evt);
	'					}
	'				});
			End If
		End Sub

		''' <summary>
		''' Returns {@code notifyOnEDT} property.
		''' </summary>
		''' <returns> {@code notifyOnEDT} property </returns>
		''' <seealso cref= #SwingPropertyChangeSupport(Object sourceBean, boolean notifyOnEDT)
		''' @since 1.6 </seealso>
		Public Property notifyOnEDT As Boolean
			Get
				Return notifyOnEDT
			End Get
		End Property

		' Serialization version ID
		Friend Const serialVersionUID As Long = 7162625831330845068L

		''' <summary>
		''' whether to notify listeners on EDT
		''' 
		''' @serial
		''' @since 1.6
		''' </summary>
		Private ReadOnly notifyOnEDT As Boolean
	End Class

End Namespace