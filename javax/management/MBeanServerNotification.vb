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
	''' Represents a notification emitted by the MBean Server through the MBeanServerDelegate MBean.
	''' The MBean Server emits the following types of notifications: MBean registration, MBean
	''' unregistration.
	''' <P>
	''' To receive MBeanServerNotifications, you need to register a listener with
	''' the <seealso cref="MBeanServerDelegate MBeanServerDelegate"/> MBean
	''' that represents the MBeanServer. The ObjectName of the MBeanServerDelegate is
	''' <seealso cref="MBeanServerDelegate#DELEGATE_NAME"/>, which is
	''' <CODE>JMImplementation:type=MBeanServerDelegate</CODE>.
	''' 
	''' <p>The following code prints a message every time an MBean is registered
	''' or unregistered in the MBean Server {@code mbeanServer}:</p>
	''' 
	''' <pre>
	''' private static final NotificationListener printListener = new NotificationListener() {
	'''     public void handleNotification(Notification n, Object handback) {
	'''         if (!(n instanceof MBeanServerNotification)) {
	'''             System.out.println("Ignored notification of class " + n.getClass().getName());
	'''             return;
	'''         }
	'''         MBeanServerNotification mbsn = (MBeanServerNotification) n;
	'''         String what;
	'''         if (n.getType().equals(MBeanServerNotification.REGISTRATION_NOTIFICATION))
	'''             what = "MBean registered";
	'''         else if (n.getType().equals(MBeanServerNotification.UNREGISTRATION_NOTIFICATION))
	'''             what = "MBean unregistered";
	'''         else
	'''             what = "Unknown type " + n.getType();
	'''         System.out.println("Received MBean Server notification: " + what + ": " +
	'''                 mbsn.getMBeanName());
	'''     }
	''' };
	''' 
	''' ...
	'''     mbeanServer.addNotificationListener(
	'''             MBeanServerDelegate.DELEGATE_NAME, printListener, null, null);
	''' </pre>
	''' 
	''' <p id="group">
	''' An MBean which is not an <seealso cref="MBeanServerDelegate"/> may also emit
	''' MBeanServerNotifications. In particular, there is a convention for
	''' MBeans to emit an MBeanServerNotification for a group of MBeans.</p>
	''' 
	''' <p>An MBeanServerNotification emitted to denote the registration or
	''' unregistration of a group of MBeans has the following characteristics:
	''' <ul><li>Its <seealso cref="Notification#getType() notification type"/> is
	'''     {@code "JMX.mbean.registered.group"} or
	'''     {@code "JMX.mbean.unregistered.group"}, which can also be written {@link
	'''     MBeanServerNotification#REGISTRATION_NOTIFICATION}{@code + ".group"} or
	'''     {@link
	'''     MBeanServerNotification#UNREGISTRATION_NOTIFICATION}{@code + ".group"}.
	''' </li>
	''' <li>Its <seealso cref="#getMBeanName() MBean name"/> is an ObjectName pattern
	'''     that selects the set (or a superset) of the MBeans being registered
	'''     or unregistered</li>
	''' <li>Its <seealso cref="Notification#getUserData() user data"/> can optionally
	'''     be set to an array of ObjectNames containing the names of all MBeans
	'''     being registered or unregistered.</li>
	''' </ul>
	''' 
	''' <p>
	''' MBeans which emit these group registration/unregistration notifications will
	''' declare them in their {@link MBeanInfo#getNotifications()
	''' MBeanNotificationInfo}.
	''' </p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanServerNotification
		Inherits Notification


		' Serial version 
		Private Const serialVersionUID As Long = 2876477500475969677L
		''' <summary>
		''' Notification type denoting that an MBean has been registered.
		''' Value is "JMX.mbean.registered".
		''' </summary>
		Public Const REGISTRATION_NOTIFICATION As String = "JMX.mbean.registered"
		''' <summary>
		''' Notification type denoting that an MBean has been unregistered.
		''' Value is "JMX.mbean.unregistered".
		''' </summary>
		Public Const UNREGISTRATION_NOTIFICATION As String = "JMX.mbean.unregistered"
		''' <summary>
		''' @serial The object names of the MBeans concerned by this notification
		''' </summary>
		Private ReadOnly objectName As ObjectName

		''' <summary>
		''' Creates an MBeanServerNotification object specifying object names of
		''' the MBeans that caused the notification and the specified notification
		''' type.
		''' </summary>
		''' <param name="type"> A string denoting the type of the
		''' notification. Set it to one these values: {@link
		''' #REGISTRATION_NOTIFICATION}, {@link
		''' #UNREGISTRATION_NOTIFICATION}. </param>
		''' <param name="source"> The MBeanServerNotification object responsible
		''' for forwarding MBean server notification. </param>
		''' <param name="sequenceNumber"> A sequence number that can be used to order
		''' received notifications. </param>
		''' <param name="objectName"> The object name of the MBean that caused the
		''' notification.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal ___objectName As ObjectName)
			MyBase.New(type, source, sequenceNumber)
			Me.objectName = ___objectName
		End Sub

		''' <summary>
		''' Returns the  object name of the MBean that caused the notification.
		''' </summary>
		''' <returns> the object name of the MBean that caused the notification. </returns>
		Public Overridable Property mBeanName As ObjectName
			Get
				Return objectName
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[mbeanName=" & objectName & "]"

		End Function

	End Class

End Namespace