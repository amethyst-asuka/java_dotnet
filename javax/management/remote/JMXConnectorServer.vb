Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Superclass of every connector server.  A connector server is
	''' attached to an MBean server.  It listens for client connection
	''' requests and creates a connection for each one.</p>
	''' 
	''' <p>A connector server is associated with an MBean server either by
	''' registering it in that MBean server, or by passing the MBean server
	''' to its constructor.</p>
	''' 
	''' <p>A connector server is inactive when created.  It only starts
	''' listening for client connections when the <seealso cref="#start() start"/>
	''' method is called.  A connector server stops listening for client
	''' connections when the <seealso cref="#stop() stop"/> method is called or when
	''' the connector server is unregistered from its MBean server.</p>
	''' 
	''' <p>Stopping a connector server does not unregister it from its
	''' MBean server.  A connector server once stopped cannot be
	''' restarted.</p>
	''' 
	''' <p>Each time a client connection is made or broken, a notification
	''' of class <seealso cref="JMXConnectionNotification"/> is emitted.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class JMXConnectorServer
		Inherits javax.management.NotificationBroadcasterSupport
		Implements JMXConnectorServerMBean, javax.management.MBeanRegistration, JMXAddressable

			Public MustOverride Function preRegister(ByVal server As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public MustOverride ReadOnly Property attributes As IDictionary(Of String, ?) Implements JMXConnectorServerMBean.getAttributes
			Public MustOverride ReadOnly Property address As JMXServiceURL Implements JMXConnectorServerMBean.getAddress, JMXAddressable.getAddress
			Public MustOverride ReadOnly Property active As Boolean Implements JMXConnectorServerMBean.isActive
			Public MustOverride Sub [stop]() Implements JMXConnectorServerMBean.stop
			Public MustOverride Sub start() Implements JMXConnectorServerMBean.start

		''' <summary>
		''' <p>Name of the attribute that specifies the authenticator for a
		''' connector server.  The value associated with this attribute, if
		''' any, must be an object that implements the interface {@link
		''' JMXAuthenticator}.</p>
		''' </summary>
		Public Const AUTHENTICATOR As String = "jmx.remote.authenticator"

		''' <summary>
		''' <p>Constructs a connector server that will be registered as an
		''' MBean in the MBean server it is attached to.  This constructor
		''' is typically called by one of the <code>createMBean</code>
		''' methods when creating, within an MBean server, a connector
		''' server that makes it available remotely.</p>
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' <p>Constructs a connector server that is attached to the given
		''' MBean server.  A connector server that is created in this way
		''' can be registered in a different MBean server, or not registered
		''' in any MBean server.</p>
		''' </summary>
		''' <param name="mbeanServer"> the MBean server that this connector server
		''' is attached to.  Null if this connector server will be attached
		''' to an MBean server by being registered in it. </param>
		Public Sub New(ByVal mbeanServer As javax.management.MBeanServer)
			Me.mbeanServer = mbeanServer
		End Sub

		''' <summary>
		''' <p>Returns the MBean server that this connector server is
		''' attached to.</p>
		''' </summary>
		''' <returns> the MBean server that this connector server is attached
		''' to, or null if it is not yet attached to an MBean server. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mBeanServer As javax.management.MBeanServer
			Get
				Return mbeanServer
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mBeanServerForwarder Implements JMXConnectorServerMBean.setMBeanServerForwarder As MBeanServerForwarder
			Set(ByVal mbsf As MBeanServerForwarder)
				If mbsf Is Nothing Then Throw New System.ArgumentException("Invalid null argument: mbsf")
    
				If mbeanServer IsNot Nothing Then mbsf.mBeanServer = mbeanServer
				mbeanServer = mbsf
			End Set
		End Property

		Public Overridable Property connectionIds As String() Implements JMXConnectorServerMBean.getConnectionIds
			Get
				SyncLock connectionIds
					Return connectionIds.ToArray()
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' <p>Returns a client stub for this connector server.  A client
		''' stub is a serializable object whose {@link
		''' JMXConnector#connect(Map) connect} method can be used to make
		''' one new connection to this connector server.</p>
		''' 
		''' <p>A given connector need not support the generation of client
		''' stubs.  However, the connectors specified by the JMX Remote API do
		''' (JMXMP Connector and RMI Connector).</p>
		''' 
		''' <p>The default implementation of this method uses {@link
		''' #getAddress} and <seealso cref="JMXConnectorFactory"/> to generate the
		''' stub, with code equivalent to the following:</p>
		''' 
		''' <pre>
		''' JMXServiceURL addr = <seealso cref="#getAddress() getAddress()"/>;
		''' return {@link JMXConnectorFactory#newJMXConnector(JMXServiceURL, Map)
		'''          JMXConnectorFactory.newJMXConnector(addr, env)};
		''' </pre>
		''' 
		''' <p>A connector server for which this is inappropriate must
		''' override this method so that it either implements the
		''' appropriate logic or throws {@link
		''' UnsupportedOperationException}.</p>
		''' </summary>
		''' <param name="env"> client connection parameters of the same sort that
		''' could be provided to {@link JMXConnector#connect(Map)
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
		Public Overridable Function toJMXConnector(Of T1)(ByVal env As IDictionary(Of T1)) As JMXConnector Implements JMXConnectorServerMBean.toJMXConnector
			If Not active Then Throw New IllegalStateException("Connector is not active")
			Dim addr As JMXServiceURL = address
			Return JMXConnectorFactory.newJMXConnector(addr, env)
		End Function

		''' <summary>
		''' <p>Returns an array indicating the notifications that this MBean
		''' sends. The implementation in <code>JMXConnectorServer</code>
		''' returns an array with one element, indicating that it can emit
		''' notifications of class <seealso cref="JMXConnectionNotification"/> with
		''' the types defined in that class.  A subclass that can emit other
		''' notifications should return an array that contains this element
		''' plus descriptions of the other notifications.</p>
		''' </summary>
		''' <returns> the array of possible notifications. </returns>
		Public Property Overrides notificationInfo As javax.management.MBeanNotificationInfo()
			Get
				Dim types As String() = { JMXConnectionNotification.OPENED, JMXConnectionNotification.CLOSED, JMXConnectionNotification.FAILED }
				Dim className As String = GetType(JMXConnectionNotification).name
				Const description As String = "A client connection has been opened or closed"
				Return New javax.management.MBeanNotificationInfo() { New javax.management.MBeanNotificationInfo(types, className, description) }
			End Get
		End Property

		''' <summary>
		''' <p>Called by a subclass when a new client connection is opened.
		''' Adds <code>connectionId</code> to the list returned by {@link
		''' #getConnectionIds()}, then emits a {@link
		''' JMXConnectionNotification} with type {@link
		''' JMXConnectionNotification#OPENED}.</p>
		''' </summary>
		''' <param name="connectionId"> the ID of the new connection.  This must be
		''' different from the ID of any connection previously opened by
		''' this connector server.
		''' </param>
		''' <param name="message"> the message for the emitted {@link
		''' JMXConnectionNotification}.  Can be null.  See {@link
		''' Notification#getMessage()}.
		''' </param>
		''' <param name="userData"> the <code>userData</code> for the emitted
		''' <seealso cref="JMXConnectionNotification"/>.  Can be null.  See {@link
		''' Notification#getUserData()}.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>connectionId</code> is
		''' null. </exception>
		Protected Friend Overridable Sub connectionOpened(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)

			If connectionId Is Nothing Then Throw New NullPointerException("Illegal null argument")

			SyncLock connectionIds
				connectionIds.Add(connectionId)
			End SyncLock

			sendNotification(JMXConnectionNotification.OPENED, connectionId, message, userData)
		End Sub

		''' <summary>
		''' <p>Called by a subclass when a client connection is closed
		''' normally.  Removes <code>connectionId</code> from the list returned
		''' by <seealso cref="#getConnectionIds()"/>, then emits a {@link
		''' JMXConnectionNotification} with type {@link
		''' JMXConnectionNotification#CLOSED}.</p>
		''' </summary>
		''' <param name="connectionId"> the ID of the closed connection.
		''' </param>
		''' <param name="message"> the message for the emitted {@link
		''' JMXConnectionNotification}.  Can be null.  See {@link
		''' Notification#getMessage()}.
		''' </param>
		''' <param name="userData"> the <code>userData</code> for the emitted
		''' <seealso cref="JMXConnectionNotification"/>.  Can be null.  See {@link
		''' Notification#getUserData()}.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>connectionId</code>
		''' is null. </exception>
		Protected Friend Overridable Sub connectionClosed(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)

			If connectionId Is Nothing Then Throw New NullPointerException("Illegal null argument")

			SyncLock connectionIds
				connectionIds.Remove(connectionId)
			End SyncLock

			sendNotification(JMXConnectionNotification.CLOSED, connectionId, message, userData)
		End Sub

		''' <summary>
		''' <p>Called by a subclass when a client connection fails.
		''' Removes <code>connectionId</code> from the list returned by
		''' <seealso cref="#getConnectionIds()"/>, then emits a {@link
		''' JMXConnectionNotification} with type {@link
		''' JMXConnectionNotification#FAILED}.</p>
		''' </summary>
		''' <param name="connectionId"> the ID of the failed connection.
		''' </param>
		''' <param name="message"> the message for the emitted {@link
		''' JMXConnectionNotification}.  Can be null.  See {@link
		''' Notification#getMessage()}.
		''' </param>
		''' <param name="userData"> the <code>userData</code> for the emitted
		''' <seealso cref="JMXConnectionNotification"/>.  Can be null.  See {@link
		''' Notification#getUserData()}.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>connectionId</code> is
		''' null. </exception>
		Protected Friend Overridable Sub connectionFailed(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)

			If connectionId Is Nothing Then Throw New NullPointerException("Illegal null argument")

			SyncLock connectionIds
				connectionIds.Remove(connectionId)
			End SyncLock

			sendNotification(JMXConnectionNotification.FAILED, connectionId, message, userData)
		End Sub

		Private Sub sendNotification(ByVal type As String, ByVal connectionId As String, ByVal message As String, ByVal userData As Object)
			Dim notif As javax.management.Notification = New JMXConnectionNotification(type, notificationSource, connectionId, nextSequenceNumber(), message, userData)
			sendNotification(notif)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property notificationSource As Object
			Get
				If myName IsNot Nothing Then
					Return myName
				Else
					Return Me
				End If
			End Get
		End Property

		Private Shared Function nextSequenceNumber() As Long
			SyncLock sequenceNumberLock
					Dim tempVar As Long = sequenceNumber
					sequenceNumber += 1
					Return tempVar
			End SyncLock
		End Function

		' implements MBeanRegistration
		''' <summary>
		''' <p>Called by an MBean server when this connector server is
		''' registered in that MBean server.  This connector server becomes
		''' attached to the MBean server and its <seealso cref="#getMBeanServer()"/>
		''' method will return <code>mbs</code>.</p>
		''' 
		''' <p>If this connector server is already attached to an MBean
		''' server, this method has no effect.  The MBean server it is
		''' attached to is not necessarily the one it is being registered
		''' in.</p>
		''' </summary>
		''' <param name="mbs"> the MBean server in which this connection server is
		''' being registered.
		''' </param>
		''' <param name="name"> The object name of the MBean.
		''' </param>
		''' <returns> The name under which the MBean is to be registered.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>mbs</code> or
		''' <code>name</code> is null. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function preRegister(ByVal mbs As javax.management.MBeanServer, ByVal name As javax.management.ObjectName) As javax.management.ObjectName
			If mbs Is Nothing OrElse name Is Nothing Then Throw New NullPointerException("Null MBeanServer or ObjectName")
			If mbeanServer Is Nothing Then
				mbeanServer = mbs
				myName = name
			End If
			Return name
		End Function

		Public Overridable Sub postRegister(ByVal registrationDone As Boolean?)
			' do nothing
		End Sub

		''' <summary>
		''' <p>Called by an MBean server when this connector server is
		''' unregistered from that MBean server.  If this connector server
		''' was attached to that MBean server by being registered in it,
		''' and if the connector server is still active,
		''' then unregistering it will call the <seealso cref="#stop stop"/> method.
		''' If the <code>stop</code> method throws an exception, the
		''' unregistration attempt will fail.  It is recommended to call
		''' the <code>stop</code> method explicitly before unregistering
		''' the MBean.</p>
		''' </summary>
		''' <exception cref="IOException"> if thrown by the <seealso cref="#stop stop"/> method. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub preDeregister()
			If myName IsNot Nothing AndAlso active Then
				[stop]()
				myName = Nothing ' just in case stop is buggy and doesn't stop
			End If
		End Sub

		Public Overridable Sub postDeregister()
			myName = Nothing
		End Sub

		''' <summary>
		''' The MBeanServer used by this server to execute a client request.
		''' </summary>
		Private mbeanServer As javax.management.MBeanServer = Nothing

		''' <summary>
		''' The name used to registered this server in an MBeanServer.
		''' It is null if the this server is not registered or has been unregistered.
		''' </summary>
		Private myName As javax.management.ObjectName

		Private ReadOnly connectionIds As IList(Of String) = New List(Of String)

		Private Shared ReadOnly sequenceNumberLock As Integer() = New Integer(){}
		Private Shared sequenceNumber As Long
	End Class

End Namespace