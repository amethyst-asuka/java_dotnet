Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Provides an implementation of {@link
	''' javax.management.NotificationEmitter NotificationEmitter}
	''' interface.  This can be used as the super class of an MBean that
	''' sends notifications.</p>
	''' 
	''' <p>By default, the notification dispatch model is synchronous.
	''' That is, when a thread calls sendNotification, the
	''' <code>NotificationListener.handleNotification</code> method of each listener
	''' is called within that thread. You can override this default
	''' by overriding <code>handleNotification</code> in a subclass, or by passing an
	''' Executor to the constructor.</p>
	''' 
	''' <p>If the method call of a filter or listener throws an <seealso cref="Exception"/>,
	''' then that exception does not prevent other listeners from being invoked.  However,
	''' if the method call of a filter or of {@code Executor.execute} or of
	''' {@code handleNotification} (when no {@code Excecutor} is specified) throws an
	''' <seealso cref="Error"/>, then that {@code Error} is propagated to the caller of
	''' <seealso cref="#sendNotification sendNotification"/>.</p>
	''' 
	''' <p>Remote listeners added using the JMX Remote API (see JMXConnector) are not
	''' usually called synchronously.  That is, when sendNotification returns, it is
	''' not guaranteed that any remote listeners have yet received the notification.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class NotificationBroadcasterSupport
		Implements NotificationEmitter

		''' <summary>
		''' Constructs a NotificationBroadcasterSupport where each listener is invoked by the
		''' thread sending the notification. This constructor is equivalent to
		''' {@link NotificationBroadcasterSupport#NotificationBroadcasterSupport(Executor,
		''' MBeanNotificationInfo[] info) NotificationBroadcasterSupport(null, null)}.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, CType(Nothing, MBeanNotificationInfo()))
		End Sub

		''' <summary>
		''' Constructs a NotificationBroadcasterSupport where each listener is invoked using
		''' the given <seealso cref="java.util.concurrent.Executor"/>. When {@link #sendNotification
		''' sendNotification} is called, a listener is selected if it was added with a null
		''' <seealso cref="NotificationFilter"/>, or if {@link NotificationFilter#isNotificationEnabled
		''' isNotificationEnabled} returns true for the notification being sent. The call to
		''' <code>NotificationFilter.isNotificationEnabled</code> takes place in the thread
		''' that called <code>sendNotification</code>. Then, for each selected listener,
		''' <seealso cref="Executor#execute executor.execute"/> is called with a command
		''' that calls the <code>handleNotification</code> method.
		''' This constructor is equivalent to
		''' {@link NotificationBroadcasterSupport#NotificationBroadcasterSupport(Executor,
		''' MBeanNotificationInfo[] info) NotificationBroadcasterSupport(executor, null)}. </summary>
		''' <param name="executor"> an executor used by the method <code>sendNotification</code> to
		''' send each notification. If it is null, the thread calling <code>sendNotification</code>
		''' will invoke the <code>handleNotification</code> method itself.
		''' @since 1.6 </param>
		Public Sub New(ByVal executor As java.util.concurrent.Executor)
			Me.New(executor, CType(Nothing, MBeanNotificationInfo()))
		End Sub

		''' <summary>
		''' <p>Constructs a NotificationBroadcasterSupport with information
		''' about the notifications that may be sent.  Each listener is
		''' invoked by the thread sending the notification.  This
		''' constructor is equivalent to {@link
		''' NotificationBroadcasterSupport#NotificationBroadcasterSupport(Executor,
		''' MBeanNotificationInfo[] info)
		''' NotificationBroadcasterSupport(null, info)}.</p>
		''' 
		''' <p>If the <code>info</code> array is not empty, then it is
		''' cloned by the constructor as if by {@code info.clone()}, and
		''' each call to <seealso cref="#getNotificationInfo()"/> returns a new
		''' clone.</p>
		''' </summary>
		''' <param name="info"> an array indicating, for each notification this
		''' MBean may send, the name of the Java class of the notification
		''' and the notification type.  Can be null, which is equivalent to
		''' an empty array.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ParamArray ByVal info As MBeanNotificationInfo())
			Me.New(Nothing, info)
		End Sub

		''' <summary>
		''' <p>Constructs a NotificationBroadcasterSupport with information about the notifications that may be sent,
		''' and where each listener is invoked using the given <seealso cref="java.util.concurrent.Executor"/>.</p>
		''' 
		''' <p>When <seealso cref="#sendNotification sendNotification"/> is called, a
		''' listener is selected if it was added with a null {@link
		''' NotificationFilter}, or if {@link
		''' NotificationFilter#isNotificationEnabled isNotificationEnabled}
		''' returns true for the notification being sent. The call to
		''' <code>NotificationFilter.isNotificationEnabled</code> takes
		''' place in the thread that called
		''' <code>sendNotification</code>. Then, for each selected
		''' listener, <seealso cref="Executor#execute executor.execute"/> is called
		''' with a command that calls the <code>handleNotification</code>
		''' method.</p>
		''' 
		''' <p>If the <code>info</code> array is not empty, then it is
		''' cloned by the constructor as if by {@code info.clone()}, and
		''' each call to <seealso cref="#getNotificationInfo()"/> returns a new
		''' clone.</p>
		''' </summary>
		''' <param name="executor"> an executor used by the method
		''' <code>sendNotification</code> to send each notification. If it
		''' is null, the thread calling <code>sendNotification</code> will
		''' invoke the <code>handleNotification</code> method itself.
		''' </param>
		''' <param name="info"> an array indicating, for each notification this
		''' MBean may send, the name of the Java class of the notification
		''' and the notification type.  Can be null, which is equivalent to
		''' an empty array.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal executor As java.util.concurrent.Executor, ParamArray ByVal info As MBeanNotificationInfo())
			Me.executor = If(executor IsNot Nothing, executor, defaultExecutor)

			notifInfo = If(info Is Nothing, NO_NOTIFICATION_INFO, info.clone())
		End Sub

		''' <summary>
		''' Adds a listener.
		''' </summary>
		''' <param name="listener"> The listener to receive notifications. </param>
		''' <param name="filter"> The filter object. If filter is null, no
		''' filtering will be performed before handling notifications. </param>
		''' <param name="handback"> An opaque object to be sent back to the
		''' listener when a notification is emitted. This object cannot be
		''' used by the Notification broadcaster object. It should be
		''' resent unchanged with the notification to the listener.
		''' </param>
		''' <exception cref="IllegalArgumentException"> thrown if the listener is null.
		''' </exception>
		''' <seealso cref= #removeNotificationListener </seealso>
		Public Overridable Sub addNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationBroadcaster.addNotificationListener

			If listener Is Nothing Then Throw New System.ArgumentException("Listener can't be null")

			listenerList.Add(New ListenerInfo(listener, filter, handback))
		End Sub

		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener) Implements NotificationBroadcaster.removeNotificationListener

			Dim wildcard As ListenerInfo = New WildcardListenerInfo(listener)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
			Dim removed As Boolean = listenerList.removeAll(java.util.Collections.singleton(wildcard))
			If Not removed Then Throw New ListenerNotFoundException("Listener not registered")
		End Sub

		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationEmitter.removeNotificationListener

			Dim li As New ListenerInfo(listener, filter, handback)
			Dim removed As Boolean = listenerList.Remove(li)
			If Not removed Then Throw New ListenerNotFoundException("Listener not registered " & "(with this filter and " & "handback)")
		End Sub

		Public Overridable Property notificationInfo As MBeanNotificationInfo() Implements NotificationBroadcaster.getNotificationInfo
			Get
				If notifInfo.Length = 0 Then
					Return notifInfo
				Else
					Return notifInfo.clone()
				End If
			End Get
		End Property


		''' <summary>
		''' Sends a notification.
		''' 
		''' If an {@code Executor} was specified in the constructor, it will be given one
		''' task per selected listener to deliver the notification to that listener.
		''' </summary>
		''' <param name="notification"> The notification to send. </param>
		Public Overridable Sub sendNotification(ByVal ___notification As Notification)

			If ___notification Is Nothing Then Return

			Dim enabled As Boolean

			For Each li As ListenerInfo In listenerList
				Try
					enabled = li.filter Is Nothing OrElse li.filter.isNotificationEnabled(___notification)
				Catch e As Exception
					If logger.debugOn() Then logger.debug("sendNotification", e)

					Continue For
				End Try

				If enabled Then executor.execute(New SendNotifJob(Me, ___notification, li))
			Next li
		End Sub

		''' <summary>
		''' <p>This method is called by {@link #sendNotification
		''' sendNotification} for each listener in order to send the
		''' notification to that listener.  It can be overridden in
		''' subclasses to change the behavior of notification delivery,
		''' for instance to deliver the notification in a separate
		''' thread.</p>
		''' 
		''' <p>The default implementation of this method is equivalent to
		''' <pre>
		''' listener.handleNotification(notif, handback);
		''' </pre>
		''' </summary>
		''' <param name="listener"> the listener to which the notification is being
		''' delivered. </param>
		''' <param name="notif"> the notification being delivered to the listener. </param>
		''' <param name="handback"> the handback object that was supplied when the
		''' listener was added.
		'''  </param>
		Protected Friend Overridable Sub handleNotification(ByVal listener As NotificationListener, ByVal notif As Notification, ByVal handback As Object)
			listener.handleNotification(notif, handback)
		End Sub

		' private stuff
		Private Class ListenerInfo
			Friend listener As NotificationListener
			Friend filter As NotificationFilter
			Friend handback As Object

			Friend Sub New(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object)
				Me.listener = listener
				Me.filter = filter
				Me.handback = handback
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not(TypeOf o Is ListenerInfo) Then Return False
				Dim li As ListenerInfo = CType(o, ListenerInfo)
				If TypeOf li Is WildcardListenerInfo Then
					Return (li.listener Is listener)
				Else
					Return (li.listener Is listener AndAlso li.filter Is filter AndAlso li.handback Is handback)
				End If
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return java.util.Objects.hashCode(listener)
			End Function
		End Class

		Private Class WildcardListenerInfo
			Inherits ListenerInfo

			Friend Sub New(ByVal listener As NotificationListener)
				MyBase.New(listener, Nothing, Nothing)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				assert(Not(TypeOf o Is WildcardListenerInfo))
				Return o.Equals(Me)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return MyBase.GetHashCode()
			End Function
		End Class

		Private listenerList As IList(Of ListenerInfo) = New java.util.concurrent.CopyOnWriteArrayList(Of ListenerInfo)

		' since 1.6
		Private ReadOnly executor As java.util.concurrent.Executor
		Private ReadOnly notifInfo As MBeanNotificationInfo()

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private final static java.util.concurrent.Executor defaultExecutor = New java.util.concurrent.Executor()
	'	{
	'			' DirectExecutor using caller thread
	'			public void execute(Runnable r)
	'			{
	'				r.run();
	'			}
	'		};

		Private Shared ReadOnly NO_NOTIFICATION_INFO As MBeanNotificationInfo() = New MBeanNotificationInfo(){}

		Private Class SendNotifJob
			Implements Runnable

			Private ReadOnly outerInstance As NotificationBroadcasterSupport

			Public Sub New(ByVal outerInstance As NotificationBroadcasterSupport, ByVal notif As Notification, ByVal listenerInfo As ListenerInfo)
					Me.outerInstance = outerInstance
				Me.notif = notif
				Me.listenerInfo = listenerInfo
			End Sub

			Public Overridable Sub run()
				Try
					outerInstance.handleNotification(listenerInfo.listener, notif, listenerInfo.handback)
				Catch e As Exception
					If logger.debugOn() Then logger.debug("SendNotifJob-run", e)
				End Try
			End Sub

			Private ReadOnly notif As Notification
			Private ReadOnly listenerInfo As ListenerInfo
		End Class

		Private Shared ReadOnly logger As New com.sun.jmx.remote.util.ClassLogger("javax.management", "NotificationBroadcasterSupport")
	End Class

End Namespace