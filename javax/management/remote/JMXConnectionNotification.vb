Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.remote


	''' <summary>
	''' <p>Notification emitted when a client connection is opened or
	''' closed or when notifications are lost.  These notifications are
	''' sent by connector servers (instances of <seealso cref="JMXConnectorServer"/>)
	''' and by connector clients (instances of <seealso cref="JMXConnector"/>).  For
	''' certain connectors, a session can consist of a sequence of
	''' connections.  Connection-opened and connection-closed notifications
	''' will be sent for each one.</p>
	''' 
	''' <p>The notification type is one of the following:</p>
	''' 
	''' <table summary="JMXConnectionNotification Types">
	''' 
	''' <tr>
	''' <th align=left>Type</th>
	''' <th align=left>Meaning</th>
	''' </tr>
	''' 
	''' <tr>
	''' <td><code>jmx.remote.connection.opened</code></td>
	''' <td>A new client connection has been opened.</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td><code>jmx.remote.connection.closed</code></td>
	''' <td>A client connection has been closed.</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td><code>jmx.remote.connection.failed</code></td>
	''' <td>A client connection has failed unexpectedly.</td>
	''' </tr>
	''' 
	''' <tr>
	''' <td><code>jmx.remote.connection.notifs.lost</code></td>
	''' <td>A client connection has potentially lost notifications.  This
	''' notification only appears on the client side.</td>
	''' </tr>
	''' </table>
	''' 
	''' <p>The <code>timeStamp</code> of the notification is a time value
	''' (consistent with <seealso cref="System#currentTimeMillis()"/>) indicating
	''' when the notification was constructed.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class JMXConnectionNotification
		Inherits javax.management.Notification

		Private Const serialVersionUID As Long = -2331308725952627538L

		''' <summary>
		''' <p>Notification type string for a connection-opened notification.
		''' </summary>
		Public Const OPENED As String = "jmx.remote.connection.opened"

		''' <summary>
		''' <p>Notification type string for a connection-closed notification.
		''' </summary>
		Public Const CLOSED As String = "jmx.remote.connection.closed"

		''' <summary>
		''' <p>Notification type string for a connection-failed notification.
		''' </summary>
		Public Const FAILED As String = "jmx.remote.connection.failed"

		''' <summary>
		''' <p>Notification type string for a connection that has possibly
		''' lost notifications.</p>
		''' </summary>
		Public Const NOTIFS_LOST As String = "jmx.remote.connection.notifs.lost"

		''' <summary>
		''' Constructs a new connection notification.  The {@link
		''' #getSource() source} of the notification depends on whether it
		''' is being sent by a connector server or a connector client:
		''' 
		''' <ul>
		''' 
		''' <li>For a connector server, if it is registered in an MBean
		''' server, the source is the <seealso cref="ObjectName"/> under which it is
		''' registered.  Otherwise, it is a reference to the connector
		''' server object itself, an instance of a subclass of {@link
		''' JMXConnectorServer}.
		''' 
		''' <li>For a connector client, the source is a reference to the
		''' connector client object, an instance of a class implementing
		''' <seealso cref="JMXConnector"/>.
		''' 
		''' </ul>
		''' </summary>
		''' <param name="type"> the type of the notification.  This is usually one
		''' of the constants <seealso cref="#OPENED"/>, <seealso cref="#CLOSED"/>, {@link
		''' #FAILED}, <seealso cref="#NOTIFS_LOST"/>.  It is not an error for it to
		''' be a different string.
		''' </param>
		''' <param name="source"> the connector server or client emitting the
		''' notification.
		''' </param>
		''' <param name="connectionId"> the ID of the connection within its
		''' connector server.
		''' </param>
		''' <param name="sequenceNumber"> a non-negative integer.  It is expected
		''' but not required that this number will be greater than any
		''' previous <code>sequenceNumber</code> in a notification from
		''' this source.
		''' </param>
		''' <param name="message"> an unspecified text message, typically containing
		''' a human-readable description of the event.  Can be null.
		''' </param>
		''' <param name="userData"> an object whose type and meaning is defined by
		''' the connector server.  Can be null.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>type</code>,
		''' <code>source</code>, or <code>connectionId</code> is null.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>sequenceNumber</code> is negative. </exception>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal connectionId As String, ByVal sequenceNumber As Long, ByVal message As String, ByVal userData As Object)
	'         We don't know whether the parent class (Notification) will
	'           throw an exception if the type or source is null, because
	'           JMX 1.2 doesn't specify that.  So we make sure it is not
	'           null, in case it would throw the wrong exception
	'           (e.g. IllegalArgumentException instead of
	'           NullPointerException).  Likewise for the sequence number.  
			MyBase.New(CStr(nonNull(type)), nonNull(source), Math.Max(0, sequenceNumber), System.currentTimeMillis(), message)
			If type Is Nothing OrElse source Is Nothing OrElse connectionId Is Nothing Then Throw New NullPointerException("Illegal null argument")
			If sequenceNumber < 0 Then Throw New System.ArgumentException("Negative sequence number")
			Me.connectionId = connectionId
			userData = userData
		End Sub

		Private Shared Function nonNull(ByVal arg As Object) As Object
			If arg Is Nothing Then
				Return ""
			Else
				Return arg
			End If
		End Function

		''' <summary>
		''' <p>The connection ID to which this notification pertains.
		''' </summary>
		''' <returns> the connection ID. </returns>
		Public Overridable Property connectionId As String
			Get
				Return connectionId
			End Get
		End Property

		''' <summary>
		''' @serial The connection ID to which this notification pertains. </summary>
		''' <seealso cref= #getConnectionId()
		'''  </seealso>
		Private ReadOnly connectionId As String
	End Class

End Namespace