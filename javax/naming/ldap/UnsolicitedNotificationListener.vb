'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface is for handling <tt>UnsolicitedNotificationEvent</tt>.
	''' "Unsolicited notification" is defined in
	''' <A HREF="http://www.ietf.org/rfc/rfc2251.txt">RFC 2251</A>.
	''' It allows the server to send unsolicited notifications to the client.
	''' A <tt>UnsolicitedNotificationListener</tt> must:
	''' <ol>
	''' <li>Implement this interface and its method
	''' <li>Implement <tt>NamingListener.namingExceptionThrown()</tt> so
	''' that it will be notified of exceptions thrown while attempting to
	''' collect unsolicited notification events.
	''' <li>Register with the context using one of the <tt>addNamingListener()</tt>
	''' methods from <tt>EventContext</tt> or <tt>EventDirContext</tt>.
	''' Only the <tt>NamingListener</tt> argument of these methods are applicable;
	''' the rest are ignored for a <tt>UnsolicitedNotificationListener</tt>.
	''' (These arguments might be applicable to the listener if it implements
	''' other listener interfaces).
	''' </ol>
	''' 
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= UnsolicitedNotificationEvent </seealso>
	''' <seealso cref= UnsolicitedNotification </seealso>
	''' <seealso cref= javax.naming.event.EventContext#addNamingListener </seealso>
	''' <seealso cref= javax.naming.event.EventDirContext#addNamingListener </seealso>
	''' <seealso cref= javax.naming.event.EventContext#removeNamingListener
	''' @since 1.3 </seealso>
	Public Interface UnsolicitedNotificationListener
		Inherits javax.naming.event.NamingListener

		''' <summary>
		''' Called when an unsolicited notification has been received.
		''' </summary>
		''' <param name="evt"> The non-null UnsolicitedNotificationEvent </param>
		 Sub notificationReceived(ByVal evt As UnsolicitedNotificationEvent)
	End Interface

End Namespace