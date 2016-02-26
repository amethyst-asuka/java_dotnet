Imports System

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>An MBean whose management interface is determined by reflection
	''' on a Java interface, and that emits notifications.</p>
	''' 
	''' <p>The following example shows how to use the public constructor
	''' {@link #StandardEmitterMBean(Object, Class, NotificationEmitter)
	''' StandardEmitterMBean(implementation, mbeanInterface, emitter)} to
	''' create an MBean emitting notifications with any
	''' implementation class name <i>Impl</i>, with a management
	''' interface defined (as for current Standard MBeans) by any interface
	''' <i>Intf</i>, and with any implementation of the interface
	''' <seealso cref="NotificationEmitter"/>. The example uses the class
	''' <seealso cref="NotificationBroadcasterSupport"/> as an implementation
	''' of the interface <seealso cref="NotificationEmitter"/>.</p>
	''' 
	'''     <pre>
	'''     MBeanServer mbs;
	'''     ...
	'''     final String[] types = new String[] {"sun.disc.space","sun.disc.alarm"};
	'''     final MBeanNotificationInfo info = new MBeanNotificationInfo(
	'''                                          types,
	'''                                          Notification.class.getName(),
	'''                                          "Notification about disc info.");
	'''     final NotificationEmitter emitter =
	'''                    new NotificationBroadcasterSupport(info);
	''' 
	'''     final Intf impl = new Impl(...);
	'''     final Object mbean = new StandardEmitterMBean(
	'''                                     impl, Intf.class, emitter);
	'''     mbs.registerMBean(mbean, objectName);
	'''     </pre>
	''' </summary>
	''' <seealso cref= StandardMBean
	''' 
	''' @since 1.6 </seealso>
	Public Class StandardEmitterMBean
		Inherits StandardMBean
		Implements NotificationEmitter

		Private Shared ReadOnly NO_NOTIFICATION_INFO As MBeanNotificationInfo() = New MBeanNotificationInfo(){}

		Private ReadOnly emitter As NotificationEmitter
		Private ReadOnly notificationInfo As MBeanNotificationInfo()

		''' <summary>
		''' <p>Make an MBean whose management interface is specified by
		''' {@code mbeanInterface}, with the given implementation and
		''' where notifications are handled by the given {@code NotificationEmitter}.
		''' The resultant MBean implements the {@code NotificationEmitter} interface
		''' by forwarding its methods to {@code emitter}.  It is legal and useful
		''' for {@code implementation} and {@code emitter} to be the same object.</p>
		''' 
		''' <p>If {@code emitter} is an instance of {@code
		''' NotificationBroadcasterSupport} then the MBean's {@link #sendNotification
		''' sendNotification} method will call {@code emitter.}{@link
		''' NotificationBroadcasterSupport#sendNotification sendNotification}.</p>
		''' 
		''' <p>The array returned by <seealso cref="#getNotificationInfo()"/> on the
		''' new MBean is a copy of the array returned by
		''' {@code emitter.}{@link NotificationBroadcaster#getNotificationInfo
		''' getNotificationInfo()} at the time of construction.  If the array
		''' returned by {@code emitter.getNotificationInfo()} later changes,
		''' that will have no effect on this object's
		''' {@code getNotificationInfo()}.</p>
		''' </summary>
		''' <param name="implementation"> the implementation of the MBean interface. </param>
		''' <param name="mbeanInterface"> a Standard MBean interface. </param>
		''' <param name="emitter"> the object that will handle notifications.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code mbeanInterface}
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if the given {@code implementation} does not implement the
		'''    specified interface, or if {@code emitter} is null. </exception>
		Public Sub New(Of T)(ByVal implementation As T, ByVal mbeanInterface As Type, ByVal emitter As NotificationEmitter)
			Me.New(implementation, mbeanInterface, False, emitter)
		End Sub

		''' <summary>
		''' <p>Make an MBean whose management interface is specified by
		''' {@code mbeanInterface}, with the given implementation and where
		''' notifications are handled by the given {@code
		''' NotificationEmitter}.  This constructor can be used to make
		''' either Standard MBeans or MXBeans.  The resultant MBean
		''' implements the {@code NotificationEmitter} interface by
		''' forwarding its methods to {@code emitter}.  It is legal and
		''' useful for {@code implementation} and {@code emitter} to be the
		''' same object.</p>
		''' 
		''' <p>If {@code emitter} is an instance of {@code
		''' NotificationBroadcasterSupport} then the MBean's {@link #sendNotification
		''' sendNotification} method will call {@code emitter.}{@link
		''' NotificationBroadcasterSupport#sendNotification sendNotification}.</p>
		''' 
		''' <p>The array returned by <seealso cref="#getNotificationInfo()"/> on the
		''' new MBean is a copy of the array returned by
		''' {@code emitter.}{@link NotificationBroadcaster#getNotificationInfo
		''' getNotificationInfo()} at the time of construction.  If the array
		''' returned by {@code emitter.getNotificationInfo()} later changes,
		''' that will have no effect on this object's
		''' {@code getNotificationInfo()}.</p>
		''' </summary>
		''' <param name="implementation"> the implementation of the MBean interface. </param>
		''' <param name="mbeanInterface"> a Standard MBean interface. </param>
		''' <param name="isMXBean"> If true, the {@code mbeanInterface} parameter
		''' names an MXBean interface and the resultant MBean is an MXBean. </param>
		''' <param name="emitter"> the object that will handle notifications.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code mbeanInterface}
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if the given {@code implementation} does not implement the
		'''    specified interface, or if {@code emitter} is null. </exception>
		Public Sub New(Of T)(ByVal implementation As T, ByVal mbeanInterface As Type, ByVal isMXBean As Boolean, ByVal emitter As NotificationEmitter)
			MyBase.New(implementation, mbeanInterface, isMXBean)
			If emitter Is Nothing Then Throw New System.ArgumentException("Null emitter")
			Me.emitter = emitter
			Dim infos As MBeanNotificationInfo() = emitter.notificationInfo
			If infos Is Nothing OrElse infos.Length = 0 Then
				Me.notificationInfo = NO_NOTIFICATION_INFO
			Else
				Me.notificationInfo = infos.clone()
			End If
		End Sub

		''' <summary>
		''' <p>Make an MBean whose management interface is specified by
		''' {@code mbeanInterface}, and
		''' where notifications are handled by the given {@code NotificationEmitter}.
		''' The resultant MBean implements the {@code NotificationEmitter} interface
		''' by forwarding its methods to {@code emitter}.</p>
		''' 
		''' <p>If {@code emitter} is an instance of {@code
		''' NotificationBroadcasterSupport} then the MBean's {@link #sendNotification
		''' sendNotification} method will call {@code emitter.}{@link
		''' NotificationBroadcasterSupport#sendNotification sendNotification}.</p>
		''' 
		''' <p>The array returned by <seealso cref="#getNotificationInfo()"/> on the
		''' new MBean is a copy of the array returned by
		''' {@code emitter.}{@link NotificationBroadcaster#getNotificationInfo
		''' getNotificationInfo()} at the time of construction.  If the array
		''' returned by {@code emitter.getNotificationInfo()} later changes,
		''' that will have no effect on this object's
		''' {@code getNotificationInfo()}.</p>
		''' 
		''' <p>This constructor must be called from a subclass that implements
		''' the given {@code mbeanInterface}.</p>
		''' </summary>
		''' <param name="mbeanInterface"> a StandardMBean interface. </param>
		''' <param name="emitter"> the object that will handle notifications.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code mbeanInterface}
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if {@code this} does not implement the specified interface, or
		'''    if {@code emitter} is null. </exception>
		Protected Friend Sub New(ByVal mbeanInterface As Type, ByVal emitter As NotificationEmitter)
			Me.New(mbeanInterface, False, emitter)
		End Sub

		''' <summary>
		''' <p>Make an MBean whose management interface is specified by
		''' {@code mbeanInterface}, and where notifications are handled by
		''' the given {@code NotificationEmitter}.  This constructor can be
		''' used to make either Standard MBeans or MXBeans.  The resultant
		''' MBean implements the {@code NotificationEmitter} interface by
		''' forwarding its methods to {@code emitter}.</p>
		''' 
		''' <p>If {@code emitter} is an instance of {@code
		''' NotificationBroadcasterSupport} then the MBean's {@link #sendNotification
		''' sendNotification} method will call {@code emitter.}{@link
		''' NotificationBroadcasterSupport#sendNotification sendNotification}.</p>
		''' 
		''' <p>The array returned by <seealso cref="#getNotificationInfo()"/> on the
		''' new MBean is a copy of the array returned by
		''' {@code emitter.}{@link NotificationBroadcaster#getNotificationInfo
		''' getNotificationInfo()} at the time of construction.  If the array
		''' returned by {@code emitter.getNotificationInfo()} later changes,
		''' that will have no effect on this object's
		''' {@code getNotificationInfo()}.</p>
		''' 
		''' <p>This constructor must be called from a subclass that implements
		''' the given {@code mbeanInterface}.</p>
		''' </summary>
		''' <param name="mbeanInterface"> a StandardMBean interface. </param>
		''' <param name="isMXBean"> If true, the {@code mbeanInterface} parameter
		''' names an MXBean interface and the resultant MBean is an MXBean. </param>
		''' <param name="emitter"> the object that will handle notifications.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code mbeanInterface}
		'''    does not follow JMX design patterns for Management Interfaces, or
		'''    if {@code this} does not implement the specified interface, or
		'''    if {@code emitter} is null. </exception>
		Protected Friend Sub New(ByVal mbeanInterface As Type, ByVal isMXBean As Boolean, ByVal emitter As NotificationEmitter)
			MyBase.New(mbeanInterface, isMXBean)
			If emitter Is Nothing Then Throw New System.ArgumentException("Null emitter")
			Me.emitter = emitter
			Dim infos As MBeanNotificationInfo() = emitter.notificationInfo
			If infos Is Nothing OrElse infos.Length = 0 Then
				Me.notificationInfo = NO_NOTIFICATION_INFO
			Else
				Me.notificationInfo = infos.clone()
			End If
		End Sub

		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener) Implements NotificationBroadcaster.removeNotificationListener
			emitter.removeNotificationListener(listener)
		End Sub

		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationEmitter.removeNotificationListener
			emitter.removeNotificationListener(listener, filter, handback)
		End Sub

		Public Overridable Sub addNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationBroadcaster.addNotificationListener
			emitter.addNotificationListener(listener, filter, handback)
		End Sub

		Public Overridable Property notificationInfo As MBeanNotificationInfo() Implements NotificationBroadcaster.getNotificationInfo
			Get
				' this getter might get called from the super constructor
				' when the notificationInfo has not been properly set yet
				If notificationInfo Is Nothing Then Return NO_NOTIFICATION_INFO
				If notificationInfo.Length = 0 Then
					Return notificationInfo
				Else
					Return notificationInfo.clone()
				End If
			End Get
		End Property

		''' <summary>
		''' <p>Sends a notification.</p>
		''' 
		''' <p>If the {@code emitter} parameter to the constructor was an
		''' instance of {@code NotificationBroadcasterSupport} then this
		''' method will call {@code emitter.}{@link
		''' NotificationBroadcasterSupport#sendNotification
		''' sendNotification}.</p>
		''' </summary>
		''' <param name="n"> the notification to send.
		''' </param>
		''' <exception cref="ClassCastException"> if the {@code emitter} parameter to the
		''' constructor was not a {@code NotificationBroadcasterSupport}. </exception>
		Public Overridable Sub sendNotification(ByVal n As Notification)
			If TypeOf emitter Is NotificationBroadcasterSupport Then
				CType(emitter, NotificationBroadcasterSupport).sendNotification(n)
			Else
				Dim msg As String = "Cannot sendNotification when emitter is not an " & "instance of NotificationBroadcasterSupport: " & emitter.GetType().name
				Throw New ClassCastException(msg)
			End If
		End Sub

		''' <summary>
		''' <p>Get the MBeanNotificationInfo[] that will be used in the
		''' MBeanInfo returned by this MBean.</p>
		''' 
		''' <p>The default implementation of this method returns
		''' <seealso cref="#getNotificationInfo()"/>.</p>
		''' </summary>
		''' <param name="info"> The default MBeanInfo derived by reflection. </param>
		''' <returns> the MBeanNotificationInfo[] for the new MBeanInfo. </returns>
		Friend Overrides Function getNotifications(ByVal info As MBeanInfo) As MBeanNotificationInfo()
			Return notificationInfo
		End Function
	End Class

End Namespace