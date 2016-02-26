Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents  the MBean server from the management point of view.
	''' The MBeanServerDelegate MBean emits the MBeanServerNotifications when
	''' an MBean is registered/unregistered in the MBean server.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanServerDelegate
		Implements MBeanServerDelegateMBean, NotificationEmitter

		''' <summary>
		''' The MBean server agent identification. </summary>
		Private mbeanServerId As String

		''' <summary>
		''' The NotificationBroadcasterSupport object that sends the
		'''    notifications 
		''' </summary>
		Private ReadOnly broadcaster As NotificationBroadcasterSupport

		Private Shared oldStamp As Long = 0
		Private ReadOnly stamp As Long
		Private sequenceNumber As Long = 1

		Private Shared ReadOnly notifsInfo As MBeanNotificationInfo()

		Shared Sub New()
			Dim types As String() = { MBeanServerNotification.UNREGISTRATION_NOTIFICATION, MBeanServerNotification.REGISTRATION_NOTIFICATION }
			notifsInfo = New MBeanNotificationInfo(0){}
			notifsInfo(0) = New MBeanNotificationInfo(types, "javax.management.MBeanServerNotification", "Notifications sent by the MBeanServerDelegate MBean")
		End Sub

		''' <summary>
		''' Create a MBeanServerDelegate object.
		''' </summary>
		Public Sub New()
			stamp = stamp
			broadcaster = New NotificationBroadcasterSupport
		End Sub


		''' <summary>
		''' Returns the MBean server agent identity.
		''' </summary>
		''' <returns> the identity. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mBeanServerId As String Implements MBeanServerDelegateMBean.getMBeanServerId
			Get
				If mbeanServerId Is Nothing Then
					Dim localHost As String
					Try
						localHost = java.net.InetAddress.localHost.hostName
					Catch e As java.net.UnknownHostException
						com.sun.jmx.defaults.JmxProperties.MISC_LOGGER.finest("Can't get local host name, " & "using ""localhost"" instead. Cause is: " & e)
						localHost = "localhost"
					End Try
					mbeanServerId = localHost & "_" & stamp
				End If
				Return mbeanServerId
			End Get
		End Property

		''' <summary>
		''' Returns the full name of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification name. </returns>
		Public Overridable Property specificationName As String Implements MBeanServerDelegateMBean.getSpecificationName
			Get
				Return com.sun.jmx.defaults.ServiceName.JMX_SPEC_NAME
			End Get
		End Property

		''' <summary>
		''' Returns the version of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification version. </returns>
		Public Overridable Property specificationVersion As String Implements MBeanServerDelegateMBean.getSpecificationVersion
			Get
				Return com.sun.jmx.defaults.ServiceName.JMX_SPEC_VERSION
			End Get
		End Property

		''' <summary>
		''' Returns the vendor of the JMX specification implemented
		''' by this product.
		''' </summary>
		''' <returns> the specification vendor. </returns>
		Public Overridable Property specificationVendor As String Implements MBeanServerDelegateMBean.getSpecificationVendor
			Get
				Return com.sun.jmx.defaults.ServiceName.JMX_SPEC_VENDOR
			End Get
		End Property

		''' <summary>
		''' Returns the JMX implementation name (the name of this product).
		''' </summary>
		''' <returns> the implementation name. </returns>
		Public Overridable Property implementationName As String Implements MBeanServerDelegateMBean.getImplementationName
			Get
				Return com.sun.jmx.defaults.ServiceName.JMX_IMPL_NAME
			End Get
		End Property

		''' <summary>
		''' Returns the JMX implementation version (the version of this product).
		''' </summary>
		''' <returns> the implementation version. </returns>
		Public Overridable Property implementationVersion As String Implements MBeanServerDelegateMBean.getImplementationVersion
			Get
				Try
					Return System.getProperty("java.runtime.version")
				Catch e As SecurityException
					Return ""
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the JMX implementation vendor (the vendor of this product).
		''' </summary>
		''' <returns> the implementation vendor. </returns>
		Public Overridable Property implementationVendor As String Implements MBeanServerDelegateMBean.getImplementationVendor
			Get
				Return com.sun.jmx.defaults.ServiceName.JMX_IMPL_VENDOR
			End Get
		End Property

		' From NotificationEmitter extends NotificationBroacaster
		'
		Public Overridable Property notificationInfo As MBeanNotificationInfo() Implements NotificationBroadcaster.getNotificationInfo
			Get
				Dim len As Integer = MBeanServerDelegate.notifsInfo.Length
				Dim infos As MBeanNotificationInfo() = New MBeanNotificationInfo(len - 1){}
				Array.Copy(MBeanServerDelegate.notifsInfo,0,infos,0,len)
				Return infos
			End Get
		End Property

		' From NotificationEmitter extends NotificationBroacaster
		'
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationBroadcaster.addNotificationListener
			broadcaster.addNotificationListener(listener,filter,handback)
		End Sub

		' From NotificationEmitter extends NotificationBroacaster
		'
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object) Implements NotificationEmitter.removeNotificationListener
			broadcaster.removeNotificationListener(listener,filter,handback)
		End Sub

		' From NotificationEmitter extends NotificationBroacaster
		'
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeNotificationListener(ByVal listener As NotificationListener) Implements NotificationBroadcaster.removeNotificationListener
			broadcaster.removeNotificationListener(listener)
		End Sub

		''' <summary>
		''' Enables the MBean server to send a notification.
		''' If the passed <var>notification</var> has a sequence number lesser
		''' or equal to 0, then replace it with the delegate's own sequence
		''' number. </summary>
		''' <param name="notification"> The notification to send.
		'''  </param>
		Public Overridable Sub sendNotification(ByVal ___notification As Notification)
			If ___notification.sequenceNumber < 1 Then
				SyncLock Me
					___notification.sequenceNumber = Me.sequenceNumber
					Me.sequenceNumber += 1
				End SyncLock
			End If
			broadcaster.sendNotification(___notification)
		End Sub

		''' <summary>
		''' Defines the default ObjectName of the MBeanServerDelegate.
		''' 
		''' @since 1.6
		''' </summary>
		Public Shared ReadOnly DELEGATE_NAME As ObjectName = com.sun.jmx.mbeanserver.Util.newObjectName("JMImplementation:type=MBeanServerDelegate")

	'     Return a timestamp that is monotonically increasing even if
	'       System.currentTimeMillis() isn't (for example, if you call this
	'       constructor more than once in the same millisecond, or if the
	'       clock always returns the same value).  This means that the ids
	'       for a given JVM will always be distinact, though there is no
	'       such guarantee for two different JVMs.  
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared stamp As Long
			Get
				Dim s As Long = System.currentTimeMillis()
				If oldStamp >= s Then s = oldStamp + 1
				oldStamp = s
				Return s
			End Get
		End Property
	End Class

End Namespace