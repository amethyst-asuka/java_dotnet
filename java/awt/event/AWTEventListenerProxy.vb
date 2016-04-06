'
' * Copyright (c) 2001, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event


	''' <summary>
	''' A class which extends the {@code EventListenerProxy}
	''' specifically for adding an {@code AWTEventListener}
	''' for a specific event mask.
	''' Instances of this class can be added as {@code AWTEventListener}s
	''' to a {@code Toolkit} object.
	''' <p>
	''' The {@code getAWTEventListeners} method of {@code Toolkit}
	''' can return a mixture of {@code AWTEventListener}
	''' and {@code AWTEventListenerProxy} objects.
	''' </summary>
	''' <seealso cref= java.awt.Toolkit </seealso>
	''' <seealso cref= java.util.EventListenerProxy
	''' @since 1.4 </seealso>
	Public Class AWTEventListenerProxy
		Inherits java.util.EventListenerProxy(Of AWTEventListener)
		Implements AWTEventListener

		Private ReadOnly eventMask As Long

		''' <summary>
		''' Constructor which binds the {@code AWTEventListener}
		''' to a specific event mask.
		''' </summary>
		''' <param name="eventMask">  the bitmap of event types to receive </param>
		''' <param name="listener">   the listener object </param>
		Public Sub New(  eventMask As Long,   listener As AWTEventListener)
			MyBase.New(listener)
			Me.eventMask = eventMask
		End Sub

		''' <summary>
		''' Forwards the AWT event to the listener delegate.
		''' </summary>
		''' <param name="event">  the AWT event </param>
		Public Overridable Sub eventDispatched(  [event] As java.awt.AWTEvent) Implements AWTEventListener.eventDispatched
			listener.eventDispatched(event_Renamed)
		End Sub

		''' <summary>
		''' Returns the event mask associated with the listener.
		''' </summary>
		''' <returns> the event mask associated with the listener </returns>
		Public Overridable Property eventMask As Long
			Get
				Return Me.eventMask
			End Get
		End Property
	End Class

End Namespace