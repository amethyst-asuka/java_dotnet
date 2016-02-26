'
' * Copyright (c) 2002, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>This interface should be used by new code in preference to the
	''' <seealso cref="NotificationBroadcaster"/> interface.</p>
	''' 
	''' <p>Implementations of this interface and of {@code NotificationBroadcaster}
	''' should be careful about synchronization.  In particular, it is not a good
	''' idea for an implementation to hold any locks while it is calling a
	''' listener.  To deal with the possibility that the list of listeners might
	''' change while a notification is being dispatched, a good strategy is to
	''' use a <seealso cref="CopyOnWriteArrayList"/> for this list.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface NotificationEmitter
		Inherits NotificationBroadcaster

		''' <summary>
		''' <p>Removes a listener from this MBean.  The MBean must have a
		''' listener that exactly matches the given <code>listener</code>,
		''' <code>filter</code>, and <code>handback</code> parameters.  If
		''' there is more than one such listener, only one is removed.</p>
		''' 
		''' <p>The <code>filter</code> and <code>handback</code> parameters
		''' may be null if and only if they are null in a listener to be
		''' removed.</p>
		''' </summary>
		''' <param name="listener"> A listener that was previously added to this
		''' MBean. </param>
		''' <param name="filter"> The filter that was specified when the listener
		''' was added. </param>
		''' <param name="handback"> The handback that was specified when the listener was
		''' added.
		''' </param>
		''' <exception cref="ListenerNotFoundException"> The listener is not
		''' registered with the MBean, or it is not registered with the
		''' given filter and handback. </exception>
		Sub removeNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object)
	End Interface

End Namespace