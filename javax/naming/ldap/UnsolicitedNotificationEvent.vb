'
' * Copyright (c) 1999, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.ldap

	''' <summary>
	''' This class represents an event fired in response to an unsolicited
	''' notification sent by the LDAP server.
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= UnsolicitedNotification </seealso>
	''' <seealso cref= UnsolicitedNotificationListener </seealso>
	''' <seealso cref= javax.naming.event.EventContext#addNamingListener </seealso>
	''' <seealso cref= javax.naming.event.EventDirContext#addNamingListener </seealso>
	''' <seealso cref= javax.naming.event.EventContext#removeNamingListener
	''' @since 1.3 </seealso>

	Public Class UnsolicitedNotificationEvent
		Inherits java.util.EventObject

		''' <summary>
		''' The notification that caused this event to be fired.
		''' @serial
		''' </summary>
		Private notice As UnsolicitedNotification

		''' <summary>
		''' Constructs a new instance of <tt>UnsolicitedNotificationEvent</tt>.
		''' </summary>
		''' <param name="src"> The non-null source that fired the event. </param>
		''' <param name="notice"> The non-null unsolicited notification. </param>
		Public Sub New(ByVal src As Object, ByVal notice As UnsolicitedNotification)
			MyBase.New(src)
			Me.notice = notice
		End Sub


		''' <summary>
		''' Returns the unsolicited notification. </summary>
		''' <returns> The non-null unsolicited notification that caused this
		''' event to be fired. </returns>
		Public Overridable Property notification As UnsolicitedNotification
			Get
				Return notice
			End Get
		End Property

		''' <summary>
		''' Invokes the <tt>notificationReceived()</tt> method on
		''' a listener using this event. </summary>
		''' <param name="listener"> The non-null listener on which to invoke
		''' <tt>notificationReceived</tt>. </param>
		Public Overridable Sub dispatch(ByVal listener As UnsolicitedNotificationListener)
			listener.notificationReceived(Me)
		End Sub

		Private Const serialVersionUID As Long = -2382603380799883705L
	End Class

End Namespace