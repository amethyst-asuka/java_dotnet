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
	''' Should be implemented by an object that wants to receive notifications.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface NotificationListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a JMX notification occurs.
		''' The implementation of this method should return as soon as possible, to avoid
		''' blocking its notification broadcaster.
		''' </summary>
		''' <param name="notification"> The notification. </param>
		''' <param name="handback"> An opaque object which helps the listener to associate
		''' information regarding the MBean emitter. This object is passed to the
		''' addNotificationListener call and resent, without modification, to the
		''' listener. </param>
		Sub handleNotification(ByVal notification As Notification, ByVal handback As Object)
	End Interface

End Namespace