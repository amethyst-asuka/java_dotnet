Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text

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

Namespace javax.management.remote.rmi




	''' <summary>
	''' <p>An RMI object representing a connector server.  Remote clients
	''' can make connections using the <seealso cref="#newClient(Object)"/> method.  This
	''' method returns an RMI object representing the connection.</p>
	''' 
	''' <p>User code does not usually reference this class directly.
	''' RMI connection servers are usually created with the class {@link
	''' RMIConnectorServer}.  Remote clients usually create connections
	''' either with <seealso cref="javax.management.remote.JMXConnectorFactory"/>
	''' or by instantiating <seealso cref="RMIConnector"/>.</p>
	''' 
	''' <p>This is an abstract class.  Concrete subclasses define the
	''' details of the client connection objects, such as whether they use
	''' JRMP or IIOP.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class RMIServerImpl
		Implements java.io.Closeable, RMIServer

		''' <summary>
		''' <p>Constructs a new <code>RMIServerImpl</code>.</p>
		''' </summary>
		''' <param name="env"> the environment containing attributes for the new
		''' <code>RMIServerImpl</code>.  Can be null, which is equivalent
		''' to an empty Map. </param>
		Public Sub New(Of T1)(ByVal env As IDictionary(Of T1))
			Me.env = If(env Is Nothing, java.util.Collections.emptyMap(Of String, Object)(), env)
		End Sub

		Friend Overridable Property rMIConnectorServer As RMIConnectorServer
			Set(ByVal connServer As RMIConnectorServer)
				Me.connServer = connServer
			End Set
		End Property

		''' <summary>
		''' <p>Exports this RMI object.</p>
		''' </summary>
		''' <exception cref="IOException"> if this RMI object cannot be exported. </exception>
		Protected Friend MustOverride Sub export()

		''' <summary>
		''' Returns a remotable stub for this server object. </summary>
		''' <returns> a remotable stub. </returns>
		''' <exception cref="IOException"> if the stub cannot be obtained - e.g the
		'''            RMIServerImpl has not been exported yet.
		'''  </exception>
		Public MustOverride Function toStub() As java.rmi.Remote

		''' <summary>
		''' <p>Sets the default <code>ClassLoader</code> for this connector
		''' server. New client connections will use this classloader.
		''' Existing client connections are unaffected.</p>
		''' </summary>
		''' <param name="cl"> the new <code>ClassLoader</code> to be used by this
		''' connector server.
		''' </param>
		''' <seealso cref= #getDefaultClassLoader </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property defaultClassLoader As ClassLoader
			Set(ByVal cl As ClassLoader)
				Me.cl = cl
			End Set
			Get
				Return cl
			End Get
		End Property


		''' <summary>
		''' <p>Sets the <code>MBeanServer</code> to which this connector
		''' server is attached. New client connections will interact
		''' with this <code>MBeanServer</code>. Existing client connections are
		''' unaffected.</p>
		''' </summary>
		''' <param name="mbs"> the new <code>MBeanServer</code>.  Can be null, but
		''' new client connections will be refused as long as it is.
		''' </param>
		''' <seealso cref= #getMBeanServer </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mBeanServer As javax.management.MBeanServer
			Set(ByVal mbs As javax.management.MBeanServer)
				Me.mbeanServer = mbs
			End Set
			Get
				Return mbeanServer
			End Get
		End Property


		Public Overridable Property version As String Implements RMIServer.getVersion
			Get
				' Expected format is: "protocol-version implementation-name"
				Try
					Return "1.0 java_runtime_" & System.getProperty("java.runtime.version")
				Catch e As SecurityException
					Return "1.0 "
				End Try
			End Get
		End Property

		''' <summary>
		''' <p>Creates a new client connection.  This method calls {@link
		''' #makeClient makeClient} and adds the returned client connection
		''' object to an internal list.  When this
		''' <code>RMIServerImpl</code> is shut down via its {@link
		''' #close()} method, the <seealso cref="RMIConnection#close() close()"/>
		''' method of each object remaining in the list is called.</p>
		''' 
		''' <p>The fact that a client connection object is in this internal
		''' list does not prevent it from being garbage collected.</p>
		''' </summary>
		''' <param name="credentials"> this object specifies the user-defined
		''' credentials to be passed in to the server in order to
		''' authenticate the caller before creating the
		''' <code>RMIConnection</code>.  Can be null.
		''' </param>
		''' <returns> the newly-created <code>RMIConnection</code>.  This is
		''' usually the object created by <code>makeClient</code>, though
		''' an implementation may choose to wrap that object in another
		''' object implementing <code>RMIConnection</code>.
		''' </returns>
		''' <exception cref="IOException"> if the new client object cannot be
		''' created or exported.
		''' </exception>
		''' <exception cref="SecurityException"> if the given credentials do not allow
		''' the server to authenticate the user successfully.
		''' </exception>
		''' <exception cref="IllegalStateException"> if <seealso cref="#getMBeanServer()"/>
		''' is null. </exception>
		Public Overridable Function newClient(ByVal credentials As Object) As RMIConnection Implements RMIServer.newClient
			Return doNewClient(credentials)
		End Function

		''' <summary>
		''' This method could be overridden by subclasses defined in this package
		''' to perform additional operations specific to the underlying transport
		''' before creating the new client connection.
		''' </summary>
		Friend Overridable Function doNewClient(ByVal credentials As Object) As RMIConnection
			Dim tracing As Boolean = logger.traceOn()

			If tracing Then logger.trace("newClient","making new client")

			If mBeanServer Is Nothing Then Throw New IllegalStateException("Not attached to an MBean server")

			Dim subject As javax.security.auth.Subject = Nothing
			Dim authenticator As javax.management.remote.JMXAuthenticator = CType(env(javax.management.remote.JMXConnectorServer.AUTHENTICATOR), javax.management.remote.JMXAuthenticator)
			If authenticator Is Nothing Then
	'            
	'             * Create the JAAS-based authenticator only if authentication
	'             * has been enabled
	'             
				If env("jmx.remote.x.password.file") IsNot Nothing OrElse env("jmx.remote.x.login.config") IsNot Nothing Then authenticator = New com.sun.jmx.remote.security.JMXPluggableAuthenticator(env)
			End If
			If authenticator IsNot Nothing Then
				If tracing Then logger.trace("newClient","got authenticator: " & authenticator.GetType().name)
				Try
					subject = authenticator.authenticate(credentials)
				Catch e As SecurityException
					logger.trace("newClient", "Authentication failed: " & e)
					Throw e
				End Try
			End If

			If tracing Then
				If subject IsNot Nothing Then
					logger.trace("newClient","subject is not null")
				Else
					logger.trace("newClient","no subject")
				End If
			End If

			Dim connectionId As String = makeConnectionId(protocol, subject)

			If tracing Then logger.trace("newClient","making new connection: " & connectionId)

			Dim client As RMIConnection = makeClient(connectionId, subject)

			dropDeadReferences()
			Dim wr As New WeakReference(Of RMIConnection)(client)
			SyncLock clientList
				clientList.Add(wr)
			End SyncLock

			connServer.connectionOpened(connectionId, "Connection opened", Nothing)

			SyncLock clientList
				If Not clientList.Contains(wr) Then Throw New java.io.IOException("The connection is refused.")
			End SyncLock

			If tracing Then logger.trace("newClient","new connection done: " & connectionId)

			Return client
		End Function

		''' <summary>
		''' <p>Creates a new client connection.  This method is called by
		''' the public method <seealso cref="#newClient(Object)"/>.</p>
		''' </summary>
		''' <param name="connectionId"> the ID of the new connection.  Every
		''' connection opened by this connector server will have a
		''' different ID.  The behavior is unspecified if this parameter is
		''' null.
		''' </param>
		''' <param name="subject"> the authenticated subject.  Can be null.
		''' </param>
		''' <returns> the newly-created <code>RMIConnection</code>.
		''' </returns>
		''' <exception cref="IOException"> if the new client object cannot be
		''' created or exported. </exception>
		Protected Friend MustOverride Function makeClient(ByVal connectionId As String, ByVal subject As javax.security.auth.Subject) As RMIConnection

		''' <summary>
		''' <p>Closes a client connection made by <seealso cref="#makeClient makeClient"/>.
		''' </summary>
		''' <param name="client"> a connection previously returned by
		''' <code>makeClient</code> on which the <code>closeClient</code>
		''' method has not previously been called.  The behavior is
		''' unspecified if these conditions are violated, including the
		''' case where <code>client</code> is null.
		''' </param>
		''' <exception cref="IOException"> if the client connection cannot be
		''' closed. </exception>
		Protected Friend MustOverride Sub closeClient(ByVal client As RMIConnection)

		''' <summary>
		''' <p>Returns the protocol string for this object.  The string is
		''' <code>rmi</code> for RMI/JRMP and <code>iiop</code> for RMI/IIOP.
		''' </summary>
		''' <returns> the protocol string for this object. </returns>
		Protected Friend MustOverride ReadOnly Property protocol As String

		''' <summary>
		''' <p>Method called when a client connection created by {@link
		''' #makeClient makeClient} is closed.  A subclass that defines
		''' <code>makeClient</code> must arrange for this method to be
		''' called when the resultant object's {@link RMIConnection#close()
		''' close} method is called.  This enables it to be removed from
		''' the <code>RMIServerImpl</code>'s list of connections.  It is
		''' not an error for <code>client</code> not to be in that
		''' list.</p>
		''' 
		''' <p>After removing <code>client</code> from the list of
		''' connections, this method calls {@link #closeClient
		''' closeClient(client)}.</p>
		''' </summary>
		''' <param name="client"> the client connection that has been closed.
		''' </param>
		''' <exception cref="IOException"> if <seealso cref="#closeClient"/> throws this
		''' exception.
		''' </exception>
		''' <exception cref="NullPointerException"> if <code>client</code> is null. </exception>
		Protected Friend Overridable Sub clientClosed(ByVal client As RMIConnection)
			Dim debug As Boolean = logger.debugOn()

			If debug Then logger.trace("clientClosed","client=" & client)

			If client Is Nothing Then Throw New NullPointerException("Null client")

			SyncLock clientList
				dropDeadReferences()
				Dim it As IEnumerator(Of WeakReference(Of RMIConnection)) = clientList.GetEnumerator()
				Do While it.MoveNext()
					Dim wr As WeakReference(Of RMIConnection) = it.Current
					If wr.get() Is client Then
						it.remove()
						Exit Do
					End If
				Loop
	'             It is not a bug for this loop not to find the client.  In
	'               our close() method, we remove a client from the list before
	'               calling its close() method.  
			End SyncLock

			If debug Then logger.trace("clientClosed", "closing client.")
			closeClient(client)

			If debug Then logger.trace("clientClosed", "sending notif")
			connServer.connectionClosed(client.connectionId, "Client connection closed", Nothing)

			If debug Then logger.trace("clientClosed","done")
		End Sub

		''' <summary>
		''' <p>Closes this connection server.  This method first calls the
		''' <seealso cref="#closeServer()"/> method so that no new client connections
		''' will be accepted.  Then, for each remaining {@link
		''' RMIConnection} object returned by {@link #makeClient
		''' makeClient}, its <seealso cref="RMIConnection#close() close"/> method is
		''' called.</p>
		''' 
		''' <p>The behavior when this method is called more than once is
		''' unspecified.</p>
		''' 
		''' <p>If <seealso cref="#closeServer()"/> throws an
		''' <code>IOException</code>, the individual connections are
		''' nevertheless closed, and then the <code>IOException</code> is
		''' thrown from this method.</p>
		''' 
		''' <p>If <seealso cref="#closeServer()"/> returns normally but one or more
		''' of the individual connections throws an
		''' <code>IOException</code>, then, after closing all the
		''' connections, one of those <code>IOException</code>s is thrown
		''' from this method.  If more than one connection throws an
		''' <code>IOException</code>, it is unspecified which one is thrown
		''' from this method.</p>
		''' </summary>
		''' <exception cref="IOException"> if <seealso cref="#closeServer()"/> or one of the
		''' <seealso cref="RMIConnection#close()"/> calls threw
		''' <code>IOException</code>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub close()
			Dim tracing As Boolean = logger.traceOn()
			Dim debug As Boolean = logger.debugOn()

			If tracing Then logger.trace("close","closing")

			Dim ioException As java.io.IOException = Nothing
			Try
				If debug Then logger.debug("close","closing Server")
				closeServer()
			Catch e As java.io.IOException
				If tracing Then logger.trace("close","Failed to close server: " & e)
				If debug Then logger.debug("close",e)
				ioException = e
			End Try

			If debug Then logger.debug("close","closing Clients")
			' Loop to close all clients
			Do
				SyncLock clientList
					If debug Then logger.debug("close","droping dead references")
					dropDeadReferences()

					If debug Then logger.debug("close","client count: " & clientList.Count)
					If clientList.Count = 0 Then Exit Do
	'                 Loop until we find a non-null client.  Because we called
	'                   dropDeadReferences(), this will usually be the first
	'                   element of the list, but a garbage collection could have
	'                   happened in between.  
					Dim it As IEnumerator(Of WeakReference(Of RMIConnection)) = clientList.GetEnumerator()
					Do While it.MoveNext()
						Dim wr As WeakReference(Of RMIConnection) = it.Current
						Dim client As RMIConnection = wr.get()
						it.remove()
						If client IsNot Nothing Then
							Try
								client.close()
							Catch e As java.io.IOException
								If tracing Then logger.trace("close","Failed to close client: " & e)
								If debug Then logger.debug("close",e)
								If ioException Is Nothing Then ioException = e
							End Try
							Exit Do
						End If
					Loop
				End SyncLock
			Loop

			If notifBuffer IsNot Nothing Then notifBuffer.Dispose()

			If ioException IsNot Nothing Then
				If tracing Then logger.trace("close","close failed.")
				Throw ioException
			End If

			If tracing Then logger.trace("close","closed.")
		End Sub

		''' <summary>
		''' <p>Called by <seealso cref="#close()"/> to close the connector server.
		''' After returning from this method, the connector server must
		''' not accept any new connections.</p>
		''' </summary>
		''' <exception cref="IOException"> if the attempt to close the connector
		''' server failed. </exception>
		Protected Friend MustOverride Sub closeServer()

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function makeConnectionId(ByVal protocol As String, ByVal subject As javax.security.auth.Subject) As String
			connectionIdNumber += 1

			Dim clientHost As String = ""
			Try
				clientHost = java.rmi.server.RemoteServer.clientHost
	'            
	'             * According to the rules specified in the javax.management.remote
	'             * package description, a numeric IPv6 address (detected by the
	'             * presence of otherwise forbidden ":" character) forming a part
	'             * of the connection id must be enclosed in square brackets.
	'             
				If clientHost.Contains(":") Then clientHost = "[" & clientHost & "]"
			Catch e As java.rmi.server.ServerNotActiveException
				logger.trace("makeConnectionId", "getClientHost", e)
			End Try

			Dim buf As New StringBuilder
			buf.Append(protocol).append(":")
			If clientHost.Length > 0 Then buf.Append("//").append(clientHost)
			buf.Append(" ")
			If subject IsNot Nothing Then
				Dim principals As java.util.Set(Of java.security.Principal) = subject.principals
				Dim sep As String = ""
				Dim it As IEnumerator(Of java.security.Principal) = principals.GetEnumerator()
				Do While it.MoveNext()
					Dim p As java.security.Principal = it.Current
					Dim name As String = p.name.Replace(" "c, "_"c).replace(";"c, ":"c)
					buf.Append(sep).append(name)
					sep = ";"
				Loop
			End If
			buf.Append(" ").append(connectionIdNumber)
			If logger.traceOn() Then logger.trace("newConnectionId","connectionId=" & buf)
			Return buf.ToString()
		End Function

		Private Sub dropDeadReferences()
			SyncLock clientList
				Dim it As IEnumerator(Of WeakReference(Of RMIConnection)) = clientList.GetEnumerator()
				Do While it.MoveNext()
					Dim wr As WeakReference(Of RMIConnection) = it.Current
					If wr.get() Is Nothing Then it.remove()
				Loop
			End SyncLock
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Property notifBuffer As com.sun.jmx.remote.internal.NotificationBuffer
			Get
				'Notification buffer is lazily created when the first client connects
				If notifBuffer Is Nothing Then notifBuffer = com.sun.jmx.remote.internal.ArrayNotificationBuffer.getNotificationBuffer(mbeanServer, env)
				Return notifBuffer
			End Get
		End Property

		Private Shared ReadOnly logger As New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.rmi", "RMIServerImpl")

		''' <summary>
		''' List of WeakReference values.  Each one references an
		'''    RMIConnection created by this object, or null if the
		'''    RMIConnection has been garbage-collected.  
		''' </summary>
		Private ReadOnly clientList As IList(Of WeakReference(Of RMIConnection)) = New List(Of WeakReference(Of RMIConnection))

		Private cl As ClassLoader

		Private mbeanServer As javax.management.MBeanServer

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly env As IDictionary(Of String, ?)

		Private connServer As RMIConnectorServer

		Private Shared connectionIdNumber As Integer

		Private notifBuffer As com.sun.jmx.remote.internal.NotificationBuffer
	End Class

End Namespace