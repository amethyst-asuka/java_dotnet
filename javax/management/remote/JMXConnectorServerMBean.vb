Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>MBean interface for connector servers.  A JMX API connector server
	''' is attached to an MBean server, and establishes connections to that
	''' MBean server for remote clients.</p>
	''' 
	''' <p>A newly-created connector server is <em>inactive</em>, and does
	''' not yet listen for connections.  Only when its <seealso cref="#start start"/>
	''' method has been called does it start listening for connections.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface JMXConnectorServerMBean
		''' <summary>
		''' <p>Activates the connector server, that is, starts listening for
		''' client connections.  Calling this method when the connector
		''' server is already active has no effect.  Calling this method
		''' when the connector server has been stopped will generate an
		''' <seealso cref="IOException"/>.</p>
		''' </summary>
		''' <exception cref="IOException"> if it is not possible to start listening
		''' or if the connector server has been stopped.
		''' </exception>
		''' <exception cref="IllegalStateException"> if the connector server has
		''' not been attached to an MBean server. </exception>
		Sub start()

		''' <summary>
		''' <p>Deactivates the connector server, that is, stops listening for
		''' client connections.  Calling this method will also close all
		''' client connections that were made by this server.  After this
		''' method returns, whether normally or with an exception, the
		''' connector server will not create any new client
		''' connections.</p>
		''' 
		''' <p>Once a connector server has been stopped, it cannot be started
		''' again.</p>
		''' 
		''' <p>Calling this method when the connector server has already
		''' been stopped has no effect.  Calling this method when the
		''' connector server has not yet been started will disable the
		''' connector server object permanently.</p>
		''' 
		''' <p>If closing a client connection produces an exception, that
		''' exception is not thrown from this method.  A {@link
		''' JMXConnectionNotification} with type {@link
		''' JMXConnectionNotification#FAILED} is emitted from this MBean
		''' with the connection ID of the connection that could not be
		''' closed.</p>
		''' 
		''' <p>Closing a connector server is a potentially slow operation.
		''' For example, if a client machine with an open connection has
		''' crashed, the close operation might have to wait for a network
		''' protocol timeout.  Callers that do not want to block in a close
		''' operation should do it in a separate thread.</p>
		''' </summary>
		''' <exception cref="IOException"> if the server cannot be closed cleanly.
		''' When this exception is thrown, the server has already attempted
		''' to close all client connections.  All client connections are
		''' closed except possibly those that generated exceptions when the
		''' server attempted to close them. </exception>
		Sub [stop]()

		''' <summary>
		''' <p>Determines whether the connector server is active.  A connector
		''' server starts being active when its <seealso cref="#start start"/> method
		''' returns successfully and remains active until either its
		''' <seealso cref="#stop stop"/> method is called or the connector server
		''' fails.</p>
		''' </summary>
		''' <returns> true if the connector server is active. </returns>
		ReadOnly Property active As Boolean

		''' <summary>
		''' <p>Inserts an object that intercepts requests for the MBean server
		''' that arrive through this connector server.  This object will be
		''' supplied as the <code>MBeanServer</code> for any new connection
		''' created by this connector server.  Existing connections are
		''' unaffected.</p>
		''' 
		''' <p>This method can be called more than once with different
		''' <seealso cref="MBeanServerForwarder"/> objects.  The result is a chain
		''' of forwarders.  The last forwarder added is the first in the chain.
		''' In more detail:</p>
		''' 
		''' <ul>
		''' <li><p>If this connector server is already associated with an
		''' <code>MBeanServer</code> object, then that object is given to
		''' {@link MBeanServerForwarder#setMBeanServer
		''' mbsf.setMBeanServer}.  If doing so produces an exception, this
		''' method throws the same exception without any other effect.</p>
		''' 
		''' <li><p>If this connector is not already associated with an
		''' <code>MBeanServer</code> object, or if the
		''' <code>mbsf.setMBeanServer</code> call just mentioned succeeds,
		''' then <code>mbsf</code> becomes this connector server's
		''' <code>MBeanServer</code>.</p>
		''' </ul>
		''' </summary>
		''' <param name="mbsf"> the new <code>MBeanServerForwarder</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the call to {@link
		''' MBeanServerForwarder#setMBeanServer mbsf.setMBeanServer} fails
		''' with <code>IllegalArgumentException</code>.  This includes the
		''' case where <code>mbsf</code> is null. </exception>
		WriteOnly Property mBeanServerForwarder As MBeanServerForwarder

		''' <summary>
		''' <p>The list of IDs for currently-open connections to this
		''' connector server.</p>
		''' </summary>
		''' <returns> a new string array containing the list of IDs.  If
		''' there are no currently-open connections, this array will be
		''' empty. </returns>
		ReadOnly Property connectionIds As String()

		''' <summary>
		''' <p>The address of this connector server.</p>
		''' <p>
		''' The address returned may not be the exact original <seealso cref="JMXServiceURL"/>
		''' that was supplied when creating the connector server, since the original
		''' address may not always be complete. For example the port number may be
		''' dynamically allocated when starting the connector server. Instead the
		''' address returned is the actual <seealso cref="JMXServiceURL"/> of the
		''' <seealso cref="JMXConnectorServer"/>. This is the address that clients supply
		''' to <seealso cref="JMXConnectorFactory#connect(JMXServiceURL)"/>.
		''' </p>
		''' <p>Note that the address returned may be {@code null} if
		'''    the {@code JMXConnectorServer} is not yet <seealso cref="#isActive active"/>.
		''' </p>
		''' </summary>
		''' <returns> the address of this connector server, or null if it
		''' does not have one. </returns>
		ReadOnly Property address As JMXServiceURL

		''' <summary>
		''' <p>The attributes for this connector server.</p>
		''' </summary>
		''' <returns> a read-only map containing the attributes for this
		''' connector server.  Attributes whose values are not serializable
		''' are omitted from this map.  If there are no serializable
		''' attributes, the returned map is empty. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property attributes As IDictionary(Of String, ?)

		''' <summary>
		''' <p>Returns a client stub for this connector server.  A client
		''' stub is a serializable object whose {@link
		''' JMXConnector#connect(Map) connect} method can be used to make
		''' one new connection to this connector server.</p>
		''' 
		''' <p>A given connector need not support the generation of client
		''' stubs.  However, the connectors specified by the JMX Remote API do
		''' (JMXMP Connector and RMI Connector).</p>
		''' </summary>
		''' <param name="env"> client connection parameters of the same sort that
		''' can be provided to {@link JMXConnector#connect(Map)
		''' JMXConnector.connect(Map)}.  Can be null, which is equivalent
		''' to an empty map.
		''' </param>
		''' <returns> a client stub that can be used to make a new connection
		''' to this connector server.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this connector
		''' server does not support the generation of client stubs.
		''' </exception>
		''' <exception cref="IllegalStateException"> if the JMXConnectorServer is
		''' not started (see <seealso cref="JMXConnectorServerMBean#isActive()"/>).
		''' </exception>
		''' <exception cref="IOException"> if a communications problem means that a
		''' stub cannot be created.
		'''  </exception>
		Function toJMXConnector(Of T1)(ByVal env As IDictionary(Of T1)) As JMXConnector
	End Interface

End Namespace