'
' * Copyright (c) 1999, 2005, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management


	''' <summary>
	''' <p>Interface implemented by an MBean that emits Notifications. It
	''' allows a listener to be registered with the MBean as a notification
	''' listener.</p>
	''' 
	''' <h3>Notification dispatch</h3>
	''' 
	''' <p>When an MBean emits a notification, it considers each listener that has been
	''' added with <seealso cref="#addNotificationListener addNotificationListener"/> and not
	''' subsequently removed with <seealso cref="#removeNotificationListener removeNotificationListener"/>.
	''' If a filter was provided with that listener, and if the filter's
	''' <seealso cref="NotificationFilter#isNotificationEnabled isNotificationEnabled"/> method returns
	''' false, the listener is ignored.  Otherwise, the listener's
	''' <seealso cref="NotificationListener#handleNotification handleNotification"/> method is called with
	''' the notification, as well as the handback object that was provided to
	''' {@code addNotificationListener}.</p>
	''' 
	''' <p>If the same listener is added more than once, it is considered as many times as it was
	''' added.  It is often useful to add the same listener with different filters or handback
	''' objects.</p>
	''' 
	''' <p>Implementations of this interface can differ regarding the thread in which the methods
	''' of filters and listeners are called.</p>
	''' 
	''' <p>If the method call of a filter or listener throws an <seealso cref="Exception"/>, then that
	''' exception should not prevent other listeners from being invoked.  However, if the method
	''' call throws an <seealso cref="Error"/>, then it is recommended that processing of the notification
	''' stop at that point, and if it is possible to propagate the {@code Error} to the sender of
	''' the notification, this should be done.</p>
	''' 
	''' <p>New code should use the <seealso cref="NotificationEmitter"/> interface
	''' instead.</p>
	''' 
	''' <p>Implementations of this interface and of {@code NotificationEmitter}
	''' should be careful about synchronization.  In particular, it is not a good
	''' idea for an implementation to hold any locks while it is calling a
	''' listener.  To deal with the possibility that the list of listeners might
	''' change while a notification is being dispatched, a good strategy is to
	''' use a <seealso cref="CopyOnWriteArrayList"/> for this list.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface NotificationBroadcaster

		''' <summary>
		''' Adds a listener to this MBean.
		''' </summary>
		''' <param name="listener"> The listener object which will handle the
		''' notifications emitted by the broadcaster. </param>
		''' <param name="filter"> The filter object. If filter is null, no
		''' filtering will be performed before handling notifications. </param>
		''' <param name="handback"> An opaque object to be sent back to the
		''' listener when a notification is emitted. This object cannot be
		''' used by the Notification broadcaster object. It should be
		''' resent unchanged with the notification to the listener.
		''' </param>
		''' <exception cref="IllegalArgumentException"> Listener parameter is null.
		''' </exception>
		''' <seealso cref= #removeNotificationListener </seealso>
		Sub addNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object)

		''' <summary>
		''' Removes a listener from this MBean.  If the listener
		''' has been registered with different handback objects or
		''' notification filters, all entries corresponding to the listener
		''' will be removed.
		''' </summary>
		''' <param name="listener"> A listener that was previously added to this
		''' MBean.
		''' </param>
		''' <exception cref="ListenerNotFoundException"> The listener is not
		''' registered with the MBean.
		''' </exception>
		''' <seealso cref= #addNotificationListener </seealso>
		''' <seealso cref= NotificationEmitter#removeNotificationListener </seealso>
		Sub removeNotificationListener(ByVal listener As NotificationListener)

		''' <summary>
		''' <p>Returns an array indicating, for each notification this
		''' MBean may send, the name of the Java class of the notification
		''' and the notification type.</p>
		''' 
		''' <p>It is not illegal for the MBean to send notifications not
		''' described in this array.  However, some clients of the MBean
		''' server may depend on the array being complete for their correct
		''' functioning.</p>
		''' </summary>
		''' <returns> the array of possible notifications. </returns>
		ReadOnly Property notificationInfo As MBeanNotificationInfo()
	End Interface

End Namespace