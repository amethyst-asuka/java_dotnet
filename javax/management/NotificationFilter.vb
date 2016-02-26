'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' To be implemented by a any class acting as a notification filter.
	''' It allows a registered notification listener to filter the notifications of interest.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface NotificationFilter
		Inherits java.io.Serializable

		''' <summary>
		''' Invoked before sending the specified notification to the listener.
		''' </summary>
		''' <param name="notification"> The notification to be sent. </param>
		''' <returns> <CODE>true</CODE> if the notification has to be sent to the listener, <CODE>false</CODE> otherwise. </returns>
		Function isNotificationEnabled(ByVal notification As Notification) As Boolean
	End Interface

End Namespace