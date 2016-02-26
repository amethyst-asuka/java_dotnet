Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>The client end of a JMX API connector.  An object of this type can
	''' be used to establish a connection to a connector server.</p>
	''' 
	''' <p>A newly-created object of this type is unconnected.  Its {@link
	''' #connect connect} method must be called before it can be used.
	''' However, objects created by {@link
	''' JMXConnectorFactory#connect(JMXServiceURL, Map)
	''' JMXConnectorFactory.connect} are already connected.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface JMXConnector
		Inherits java.io.Closeable

		''' <summary>
		''' <p>Name of the attribute that specifies the credentials to send
		''' to the connector server during connection.  The value
		''' associated with this attribute, if any, is a serializable
		''' object of an appropriate type for the server's {@link
		''' JMXAuthenticator}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		 public static final String CREDENTIALS = "jmx.remote.credentials";

		''' <summary>
		''' <p>Establishes the connection to the connector server.  This
		''' method is equivalent to {@link #connect(Map)
		''' connect(null)}.</p>
		''' </summary>
		''' <exception cref="IOException"> if the connection could not be made
		''' because of a communication problem.
		''' </exception>
		''' <exception cref="SecurityException"> if the connection could not be
		''' made for security reasons. </exception>
		Sub connect()

		''' <summary>
		''' <p>Establishes the connection to the connector server.</p>
		''' 
		''' <p>If <code>connect</code> has already been called successfully
		''' on this object, calling it again has no effect.  If, however,
		''' <seealso cref="#close"/> was called after <code>connect</code>, the new
		''' <code>connect</code> will throw an <code>IOException</code>.
		''' 
		''' <p>Otherwise, either <code>connect</code> has never been called
		''' on this object, or it has been called but produced an
		''' exception.  Then calling <code>connect</code> will attempt to
		''' establish a connection to the connector server.</p>
		''' </summary>
		''' <param name="env"> the properties of the connection.  Properties in
		''' this map override properties in the map specified when the
		''' <code>JMXConnector</code> was created, if any.  This parameter
		''' can be null, which is equivalent to an empty map.
		''' </param>
		''' <exception cref="IOException"> if the connection could not be made
		''' because of a communication problem.
		''' </exception>
		''' <exception cref="SecurityException"> if the connection could not be
		''' made for security reasons. </exception>
		Sub connect(Of T1)(ByVal env As IDictionary(Of T1))

		''' <summary>
		''' <p>Returns an <code>MBeanServerConnection</code> object
		''' representing a remote MBean server.  For a given
		''' <code>JMXConnector</code>, two successful calls to this method
		''' will usually return the same <code>MBeanServerConnection</code>
		''' object, though this is not required.</p>
		''' 
		''' <p>For each method in the returned
		''' <code>MBeanServerConnection</code>, calling the method causes
		''' the corresponding method to be called in the remote MBean
		''' server.  The value returned by the MBean server method is the
		''' value returned to the client.  If the MBean server method
		''' produces an <code>Exception</code>, the same
		''' <code>Exception</code> is seen by the client.  If the MBean
		''' server method, or the attempt to call it, produces an
		''' <code>Error</code>, the <code>Error</code> is wrapped in a
		''' <seealso cref="JMXServerErrorException"/>, which is seen by the
		''' client.</p>
		''' 
		''' <p>Calling this method is equivalent to calling
		''' <seealso cref="#getMBeanServerConnection(Subject) getMBeanServerConnection(null)"/>
		''' meaning that no delegation subject is specified and that all the
		''' operations called on the <code>MBeanServerConnection</code> must
		''' use the authenticated subject, if any.</p>
		''' </summary>
		''' <returns> an object that implements the
		''' <code>MBeanServerConnection</code> interface by forwarding its
		''' methods to the remote MBean server.
		''' </returns>
		''' <exception cref="IOException"> if a valid
		''' <code>MBeanServerConnection</code> cannot be created, for
		''' instance because the connection to the remote MBean server has
		''' not yet been established (with the {@link #connect(Map)
		''' connect} method), or it has been closed, or it has broken. </exception>
		ReadOnly Property mBeanServerConnection As javax.management.MBeanServerConnection

		''' <summary>
		''' <p>Returns an <code>MBeanServerConnection</code> object representing
		''' a remote MBean server on which operations are performed on behalf of
		''' the supplied delegation subject. For a given <code>JMXConnector</code>
		''' and <code>Subject</code>, two successful calls to this method will
		''' usually return the same <code>MBeanServerConnection</code> object,
		''' though this is not required.</p>
		''' 
		''' <p>For each method in the returned
		''' <code>MBeanServerConnection</code>, calling the method causes
		''' the corresponding method to be called in the remote MBean
		''' server on behalf of the given delegation subject instead of the
		''' authenticated subject. The value returned by the MBean server
		''' method is the value returned to the client. If the MBean server
		''' method produces an <code>Exception</code>, the same
		''' <code>Exception</code> is seen by the client. If the MBean
		''' server method, or the attempt to call it, produces an
		''' <code>Error</code>, the <code>Error</code> is wrapped in a
		''' <seealso cref="JMXServerErrorException"/>, which is seen by the
		''' client.</p>
		''' </summary>
		''' <param name="delegationSubject"> the <code>Subject</code> on behalf of
		''' which requests will be performed.  Can be null, in which case
		''' requests will be performed on behalf of the authenticated
		''' Subject, if any.
		''' </param>
		''' <returns> an object that implements the <code>MBeanServerConnection</code>
		''' interface by forwarding its methods to the remote MBean server on behalf
		''' of a given delegation subject.
		''' </returns>
		''' <exception cref="IOException"> if a valid <code>MBeanServerConnection</code>
		''' cannot be created, for instance because the connection to the remote
		''' MBean server has not yet been established (with the {@link #connect(Map)
		''' connect} method), or it has been closed, or it has broken. </exception>
		Function getMBeanServerConnection(ByVal delegationSubject As javax.security.auth.Subject) As javax.management.MBeanServerConnection

		''' <summary>
		''' <p>Closes the client connection to its server.  Any ongoing or new
		''' request using the MBeanServerConnection returned by {@link
		''' #getMBeanServerConnection()} will get an
		''' <code>IOException</code>.</p>
		''' 
		''' <p>If <code>close</code> has already been called successfully
		''' on this object, calling it again has no effect.  If
		''' <code>close</code> has never been called, or if it was called
		''' but produced an exception, an attempt will be made to close the
		''' connection.  This attempt can succeed, in which case
		''' <code>close</code> will return normally, or it can generate an
		''' exception.</p>
		''' 
		''' <p>Closing a connection is a potentially slow operation.  For
		''' example, if the server has crashed, the close operation might
		''' have to wait for a network protocol timeout.  Callers that do
		''' not want to block in a close operation should do it in a
		''' separate thread.</p>
		''' </summary>
		''' <exception cref="IOException"> if the connection cannot be closed
		''' cleanly.  If this exception is thrown, it is not known whether
		''' the server end of the connection has been cleanly closed. </exception>
		Sub close()

		''' <summary>
		''' <p>Adds a listener to be informed of changes in connection
		''' status.  The listener will receive notifications of type {@link
		''' JMXConnectionNotification}.  An implementation can send other
		''' types of notifications too.</p>
		''' 
		''' <p>Any number of listeners can be added with this method.  The
		''' same listener can be added more than once with the same or
		''' different values for the filter and handback.  There is no
		''' special treatment of a duplicate entry.  For example, if a
		''' listener is registered twice with no filter, then its
		''' <code>handleNotification</code> method will be called twice for
		''' each notification.</p>
		''' </summary>
		''' <param name="listener"> a listener to receive connection status
		''' notifications. </param>
		''' <param name="filter"> a filter to select which notifications are to be
		''' delivered to the listener, or null if all notifications are to
		''' be delivered. </param>
		''' <param name="handback"> an object to be given to the listener along
		''' with each notification.  Can be null.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>listener</code> is
		''' null.
		''' </exception>
		''' <seealso cref= #removeConnectionNotificationListener </seealso>
		''' <seealso cref= javax.management.NotificationBroadcaster#addNotificationListener </seealso>
		Sub addConnectionNotificationListener(ByVal listener As javax.management.NotificationListener, ByVal filter As javax.management.NotificationFilter, ByVal handback As Object)

		''' <summary>
		''' <p>Removes a listener from the list to be informed of changes
		''' in status.  The listener must previously have been added.  If
		''' there is more than one matching listener, all are removed.</p>
		''' </summary>
		''' <param name="listener"> a listener to receive connection status
		''' notifications.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>listener</code> is
		''' null.
		''' </exception>
		''' <exception cref="ListenerNotFoundException"> if the listener is not
		''' registered with this <code>JMXConnector</code>.
		''' </exception>
		''' <seealso cref= #removeConnectionNotificationListener(NotificationListener,
		''' NotificationFilter, Object) </seealso>
		''' <seealso cref= #addConnectionNotificationListener </seealso>
		''' <seealso cref= javax.management.NotificationEmitter#removeNotificationListener </seealso>
		Sub removeConnectionNotificationListener(ByVal listener As javax.management.NotificationListener)

		''' <summary>
		''' <p>Removes a listener from the list to be informed of changes
		''' in status.  The listener must previously have been added with
		''' the same three parameters.  If there is more than one matching
		''' listener, only one is removed.</p>
		''' </summary>
		''' <param name="l"> a listener to receive connection status notifications. </param>
		''' <param name="f"> a filter to select which notifications are to be
		''' delivered to the listener.  Can be null. </param>
		''' <param name="handback"> an object to be given to the listener along
		''' with each notification.  Can be null.
		''' </param>
		''' <exception cref="ListenerNotFoundException"> if the listener is not
		''' registered with this <code>JMXConnector</code>, or is not
		''' registered with the given filter and handback.
		''' </exception>
		''' <seealso cref= #removeConnectionNotificationListener(NotificationListener) </seealso>
		''' <seealso cref= #addConnectionNotificationListener </seealso>
		''' <seealso cref= javax.management.NotificationEmitter#removeNotificationListener </seealso>
		Sub removeConnectionNotificationListener(ByVal l As javax.management.NotificationListener, ByVal f As javax.management.NotificationFilter, ByVal handback As Object)

		''' <summary>
		''' <p>Gets this connection's ID from the connector server.  For a
		''' given connector server, every connection will have a unique id
		''' which does not change during the lifetime of the
		''' connection.</p>
		''' </summary>
		''' <returns> the unique ID of this connection.  This is the same as
		''' the ID that the connector server includes in its {@link
		''' JMXConnectionNotification}s.  The {@link
		''' javax.management.remote package description} describes the
		''' conventions for connection IDs.
		''' </returns>
		''' <exception cref="IOException"> if the connection ID cannot be obtained,
		''' for instance because the connection is closed or broken. </exception>
		ReadOnly Property connectionId As String
	End Interface

End Namespace