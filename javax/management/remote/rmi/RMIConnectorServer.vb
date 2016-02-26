Imports System
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
	''' <p>A JMX API connector server that creates RMI-based connections
	''' from remote clients.  Usually, such connector servers are made
	''' using {@link javax.management.remote.JMXConnectorServerFactory
	''' JMXConnectorServerFactory}.  However, specialized applications can
	''' use this class directly, for example with an <seealso cref="RMIServerImpl"/>
	''' object.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RMIConnectorServer
		Inherits javax.management.remote.JMXConnectorServer

		''' <summary>
		''' <p>Name of the attribute that specifies whether the {@link
		''' RMIServer} stub that represents an RMI connector server should
		''' override an existing stub at the same address.  The value
		''' associated with this attribute, if any, should be a string that
		''' is equal, ignoring case, to <code>"true"</code> or
		''' <code>"false"</code>.  The default value is false.</p>
		''' </summary>
		Public Const JNDI_REBIND_ATTRIBUTE As String = "jmx.remote.jndi.rebind"

		''' <summary>
		''' <p>Name of the attribute that specifies the {@link
		''' RMIClientSocketFactory} for the RMI objects created in
		''' conjunction with this connector. The value associated with this
		''' attribute must be of type <code>RMIClientSocketFactory</code> and can
		''' only be specified in the <code>Map</code> argument supplied when
		''' creating a connector server.</p>
		''' </summary>
		Public Const RMI_CLIENT_SOCKET_FACTORY_ATTRIBUTE As String = "jmx.remote.rmi.client.socket.factory"

		''' <summary>
		''' <p>Name of the attribute that specifies the {@link
		''' RMIServerSocketFactory} for the RMI objects created in
		''' conjunction with this connector. The value associated with this
		''' attribute must be of type <code>RMIServerSocketFactory</code> and can
		''' only be specified in the <code>Map</code> argument supplied when
		''' creating a connector server.</p>
		''' </summary>
		Public Const RMI_SERVER_SOCKET_FACTORY_ATTRIBUTE As String = "jmx.remote.rmi.server.socket.factory"

		''' <summary>
		''' <p>Makes an <code>RMIConnectorServer</code>.
		''' This is equivalent to calling {@link #RMIConnectorServer(
		''' JMXServiceURL,Map,RMIServerImpl,MBeanServer)
		''' RMIConnectorServer(directoryURL,environment,null,null)}</p>
		''' </summary>
		''' <param name="url"> the URL defining how to create the connector server.
		''' Cannot be null.
		''' </param>
		''' <param name="environment"> attributes governing the creation and
		''' storing of the RMI object.  Can be null, which is equivalent to
		''' an empty Map.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>url</code> is null.
		''' </exception>
		''' <exception cref="MalformedURLException"> if <code>url</code> does not
		''' conform to the syntax for an RMI connector, or if its protocol
		''' is not recognized by this implementation. Only "rmi" and "iiop"
		''' are valid when this constructor is used.
		''' </exception>
		''' <exception cref="IOException"> if the connector server cannot be created
		''' for some reason or if it is inevitable that its {@link #start()
		''' start} method will fail. </exception>
		Public Sub New(Of T1)(ByVal url As javax.management.remote.JMXServiceURL, ByVal environment As IDictionary(Of T1))
			Me.New(url, environment, CType(Nothing, javax.management.MBeanServer))
		End Sub

		''' <summary>
		''' <p>Makes an <code>RMIConnectorServer</code> for the given MBean
		''' server.
		''' This is equivalent to calling {@link #RMIConnectorServer(
		''' JMXServiceURL,Map,RMIServerImpl,MBeanServer)
		''' RMIConnectorServer(directoryURL,environment,null,mbeanServer)}</p>
		''' </summary>
		''' <param name="url"> the URL defining how to create the connector server.
		''' Cannot be null.
		''' </param>
		''' <param name="environment"> attributes governing the creation and
		''' storing of the RMI object.  Can be null, which is equivalent to
		''' an empty Map.
		''' </param>
		''' <param name="mbeanServer"> the MBean server to which the new connector
		''' server is attached, or null if it will be attached by being
		''' registered as an MBean in the MBean server.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>url</code> is null.
		''' </exception>
		''' <exception cref="MalformedURLException"> if <code>url</code> does not
		''' conform to the syntax for an RMI connector, or if its protocol
		''' is not recognized by this implementation. Only "rmi" and "iiop"
		''' are valid when this constructor is used.
		''' </exception>
		''' <exception cref="IOException"> if the connector server cannot be created
		''' for some reason or if it is inevitable that its {@link #start()
		''' start} method will fail. </exception>
		Public Sub New(Of T1)(ByVal url As javax.management.remote.JMXServiceURL, ByVal environment As IDictionary(Of T1), ByVal mbeanServer As javax.management.MBeanServer)
			Me.New(url, environment, CType(Nothing, RMIServerImpl), mbeanServer)
		End Sub

		''' <summary>
		''' <p>Makes an <code>RMIConnectorServer</code> for the given MBean
		''' server.</p>
		''' </summary>
		''' <param name="url"> the URL defining how to create the connector server.
		''' Cannot be null.
		''' </param>
		''' <param name="environment"> attributes governing the creation and
		''' storing of the RMI object.  Can be null, which is equivalent to
		''' an empty Map.
		''' </param>
		''' <param name="rmiServerImpl"> An implementation of the RMIServer interface,
		'''  consistent with the protocol type specified in <var>url</var>.
		'''  If this parameter is non null, the protocol type specified by
		'''  <var>url</var> is not constrained, and is assumed to be valid.
		'''  Otherwise, only "rmi" and "iiop" will be recognized.
		''' </param>
		''' <param name="mbeanServer"> the MBean server to which the new connector
		''' server is attached, or null if it will be attached by being
		''' registered as an MBean in the MBean server.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>url</code> is null.
		''' </exception>
		''' <exception cref="MalformedURLException"> if <code>url</code> does not
		''' conform to the syntax for an RMI connector, or if its protocol
		''' is not recognized by this implementation. Only "rmi" and "iiop"
		''' are recognized when <var>rmiServerImpl</var> is null.
		''' </exception>
		''' <exception cref="IOException"> if the connector server cannot be created
		''' for some reason or if it is inevitable that its {@link #start()
		''' start} method will fail.
		''' </exception>
		''' <seealso cref= #start </seealso>
		Public Sub New(Of T1)(ByVal url As javax.management.remote.JMXServiceURL, ByVal environment As IDictionary(Of T1), ByVal rmiServerImpl As RMIServerImpl, ByVal mbeanServer As javax.management.MBeanServer)
			MyBase.New(mbeanServer)

			If url Is Nothing Then Throw New System.ArgumentException("Null JMXServiceURL")
			If rmiServerImpl Is Nothing Then
				Dim prt As String = url.protocol
				If prt Is Nothing OrElse Not(prt.Equals("rmi") OrElse prt.Equals("iiop")) Then
					Dim msg As String = "Invalid protocol type: " & prt
					Throw New java.net.MalformedURLException(msg)
				End If
				Dim urlPath As String = url.uRLPath
				If (Not urlPath.Equals("")) AndAlso (Not urlPath.Equals("/")) AndAlso (Not urlPath.StartsWith("/jndi/")) Then
					Dim msg As String = "URL path must be empty or start with " & "/jndi/"
					Throw New java.net.MalformedURLException(msg)
				End If
			End If

			If environment Is Nothing Then
				Me.attributes = java.util.Collections.emptyMap()
			Else
				com.sun.jmx.remote.util.EnvHelp.checkAttributes(environment)
				Me.attributes = java.util.Collections.unmodifiableMap(environment)
			End If

			Me.address = url
			Me.rmiServerImpl = rmiServerImpl
		End Sub

		''' <summary>
		''' <p>Returns a client stub for this connector server.  A client
		''' stub is a serializable object whose {@link
		''' JMXConnector#connect(Map) connect} method can be used to make
		''' one new connection to this connector server.</p>
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
		''' not started (see <seealso cref="#isActive()"/>).
		''' </exception>
		''' <exception cref="IOException"> if a communications problem means that a
		''' stub cannot be created.
		'''  </exception>
		Public Overrides Function toJMXConnector(Of T1)(ByVal env As IDictionary(Of T1)) As javax.management.remote.JMXConnector
			' The serialized for of rmiServerImpl is automatically
			' a RMI server stub.
			If Not active Then Throw New IllegalStateException("Connector is not active")

			' Merge maps
			Dim usemap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)(If(Me.attributes Is Nothing, java.util.Collections.emptyMap(Of String, Object)(), Me.attributes))

			If env IsNot Nothing Then
				com.sun.jmx.remote.util.EnvHelp.checkAttributes(env)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				usemap.putAll(env)
			End If

			usemap = com.sun.jmx.remote.util.EnvHelp.filterAttributes(usemap)

			Dim stub As RMIServer=CType(rmiServerImpl.toStub(), RMIServer)

			Return New RMIConnector(stub, usemap)
		End Function

		''' <summary>
		''' <p>Activates the connector server, that is starts listening for
		''' client connections.  Calling this method when the connector
		''' server is already active has no effect.  Calling this method
		''' when the connector server has been stopped will generate an
		''' <code>IOException</code>.</p>
		''' 
		''' <p>The behavior of this method when called for the first time
		''' depends on the parameters that were supplied at construction,
		''' as described below.</p>
		''' 
		''' <p>First, an object of a subclass of <seealso cref="RMIServerImpl"/> is
		''' required, to export the connector server through RMI:</p>
		''' 
		''' <ul>
		''' 
		''' <li>If an <code>RMIServerImpl</code> was supplied to the
		''' constructor, it is used.
		''' 
		''' <li>Otherwise, if the protocol part of the
		''' <code>JMXServiceURL</code> supplied to the constructor was
		''' <code>iiop</code>, an object of type <seealso cref="RMIIIOPServerImpl"/>
		''' is created.
		''' 
		''' <li>Otherwise, if the <code>JMXServiceURL</code>
		''' was null, or its protocol part was <code>rmi</code>, an object
		''' of type <seealso cref="RMIJRMPServerImpl"/> is created.
		''' 
		''' <li>Otherwise, the implementation can create an
		''' implementation-specific <seealso cref="RMIServerImpl"/> or it can throw
		''' <seealso cref="MalformedURLException"/>.
		''' 
		''' </ul>
		''' 
		''' <p>If the given address includes a JNDI directory URL as
		''' specified in the package documentation for {@link
		''' javax.management.remote.rmi}, then this
		''' <code>RMIConnectorServer</code> will bootstrap by binding the
		''' <code>RMIServerImpl</code> to the given address.</p>
		''' 
		''' <p>If the URL path part of the <code>JMXServiceURL</code> was
		''' empty or a single slash (<code>/</code>), then the RMI object
		''' will not be bound to a directory.  Instead, a reference to it
		''' will be encoded in the URL path of the RMIConnectorServer
		''' address (returned by <seealso cref="#getAddress()"/>).  The encodings for
		''' <code>rmi</code> and <code>iiop</code> are described in the
		''' package documentation for {@link
		''' javax.management.remote.rmi}.</p>
		''' 
		''' <p>The behavior when the URL path is neither empty nor a JNDI
		''' directory URL, or when the protocol is neither <code>rmi</code>
		''' nor <code>iiop</code>, is implementation defined, and may
		''' include throwing <seealso cref="MalformedURLException"/> when the
		''' connector server is created or when it is started.</p>
		''' </summary>
		''' <exception cref="IllegalStateException"> if the connector server has
		''' not been attached to an MBean server. </exception>
		''' <exception cref="IOException"> if the connector server cannot be
		''' started, or in the case of the {@code iiop} protocol, that
		''' RMI/IIOP is not supported. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub start()
			Dim tracing As Boolean = logger.traceOn()

			If state = STARTED Then
				If tracing Then logger.trace("start", "already started")
				Return
			ElseIf state = STOPPED Then
				If tracing Then logger.trace("start", "already stopped")
				Throw New java.io.IOException("The server has been stopped.")
			End If

			If mBeanServer Is Nothing Then Throw New IllegalStateException("This connector server is not " & "attached to an MBean server")

			' Check the internal access file property to see
			' if an MBeanServerForwarder is to be provided
			'
			If attributes IsNot Nothing Then
				' Check if access file property is specified
				'
				Dim accessFile As String = CStr(attributes("jmx.remote.x.access.file"))
				If accessFile IsNot Nothing Then
					' Access file property specified, create an instance
					' of the MBeanServerFileAccessController class
					'
					Dim mbsf As javax.management.remote.MBeanServerForwarder
					Try
						mbsf = New com.sun.jmx.remote.security.MBeanServerFileAccessController(accessFile)
					Catch e As java.io.IOException
						Throw com.sun.jmx.remote.util.EnvHelp.initCause(New System.ArgumentException(e.Message), e)
					End Try
					' Set the MBeanServerForwarder
					'
					mBeanServerForwarder = mbsf
				End If
			End If

			Try
				If tracing Then logger.trace("start", "setting default class loader")
				defaultClassLoader = com.sun.jmx.remote.util.EnvHelp.resolveServerClassLoader(attributes, mBeanServer)
			Catch infc As javax.management.InstanceNotFoundException
				Dim x As New System.ArgumentException("ClassLoader not found: " & infc)
				Throw com.sun.jmx.remote.util.EnvHelp.initCause(x,infc)
			End Try

			If tracing Then logger.trace("start", "setting RMIServer object")
			Dim rmiServer As RMIServerImpl

			If rmiServerImpl IsNot Nothing Then
				rmiServer = rmiServerImpl
			Else
				rmiServer = newServer()
			End If

			rmiServer.mBeanServer = mBeanServer
			rmiServer.defaultClassLoader = defaultClassLoader
			rmiServer.rMIConnectorServer = Me
			rmiServer.export()

			Try
				If tracing Then logger.trace("start", "getting RMIServer object to export")
				Dim objref As RMIServer = objectToBind(rmiServer, attributes)

				If address IsNot Nothing AndAlso address.uRLPath.StartsWith("/jndi/") Then
					Dim jndiUrl As String = address.uRLPath.Substring(6)

					If tracing Then logger.trace("start", "Using external directory: " & jndiUrl)

					Dim stringBoolean As String = CStr(attributes(JNDI_REBIND_ATTRIBUTE))
					Dim rebind As Boolean = com.sun.jmx.remote.util.EnvHelp.computeBooleanFromString(stringBoolean)

					If tracing Then logger.trace("start", JNDI_REBIND_ATTRIBUTE & "=" & rebind)

					Try
						If tracing Then logger.trace("start", "binding to " & jndiUrl)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
						Dim usemap As Dictionary(Of ?, ?) = com.sun.jmx.remote.util.EnvHelp.mapToHashtable(attributes)

						bind(jndiUrl, usemap, objref, rebind)

						boundJndiUrl = jndiUrl
					Catch e As javax.naming.NamingException
						' fit e in the nested exception if we are on 1.4
						Throw newIOException("Cannot bind to URL [" & jndiUrl & "]: " & e, e)
					End Try
				Else
					' if jndiURL is null, we must encode the stub into the URL.
					If tracing Then logger.trace("start", "Encoding URL")

					encodeStubInAddress(objref, attributes)

					If tracing Then logger.trace("start", "Encoded URL: " & Me.address)
				End If
			Catch e As Exception
				Try
					rmiServer.close()
				Catch x As Exception
					' OK: we are already throwing another exception
				End Try
				If TypeOf e Is Exception Then
					Throw CType(e, Exception)
				ElseIf TypeOf e Is java.io.IOException Then
					Throw CType(e, java.io.IOException)
				Else
					Throw newIOException("Got unexpected exception while " & "starting the connector server: " & e, e)
				End If
			End Try

			rmiServerImpl = rmiServer

			SyncLock openedServers
				openedServers.add(Me)
			End SyncLock

			state = STARTED

			If tracing Then
				logger.trace("start", "Connector Server Address = " & address)
				logger.trace("start", "started.")
			End If
		End Sub

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
		''' JMXConnectionNotification} is emitted from this MBean with the
		''' connection ID of the connection that could not be closed.</p>
		''' 
		''' <p>Closing a connector server is a potentially slow operation.
		''' For example, if a client machine with an open connection has
		''' crashed, the close operation might have to wait for a network
		''' protocol timeout.  Callers that do not want to block in a close
		''' operation should do it in a separate thread.</p>
		''' 
		''' <p>This method calls the method {@link RMIServerImpl#close()
		''' close} on the connector server's <code>RMIServerImpl</code>
		''' object.</p>
		''' 
		''' <p>If the <code>RMIServerImpl</code> was bound to a JNDI
		''' directory by the <seealso cref="#start() start"/> method, it is unbound
		''' from the directory by this method.</p>
		''' </summary>
		''' <exception cref="IOException"> if the server cannot be closed cleanly,
		''' or if the <code>RMIServerImpl</code> cannot be unbound from the
		''' directory.  When this exception is thrown, the server has
		''' already attempted to close all client connections, if
		''' appropriate; to call <seealso cref="RMIServerImpl#close()"/>; and to
		''' unbind the <code>RMIServerImpl</code> from its directory, if
		''' appropriate.  All client connections are closed except possibly
		''' those that generated exceptions when the server attempted to
		''' close them. </exception>
		Public Overrides Sub [stop]()
			Dim tracing As Boolean = logger.traceOn()

			SyncLock Me
				If state = STOPPED Then
					If tracing Then logger.trace("stop","already stopped.")
					Return
				ElseIf state = CREATED Then
					If tracing Then logger.trace("stop","not started yet.")
				End If

				If tracing Then logger.trace("stop", "stopping.")
				state = STOPPED
			End SyncLock

			SyncLock openedServers
				openedServers.remove(Me)
			End SyncLock

			Dim exception As java.io.IOException = Nothing

			' rmiServerImpl can be null if stop() called without start()
			If rmiServerImpl IsNot Nothing Then
				Try
					If tracing Then logger.trace("stop", "closing RMI server.")
					rmiServerImpl.close()
				Catch e As java.io.IOException
					If tracing Then logger.trace("stop", "failed to close RMI server: " & e)
					If logger.debugOn() Then logger.debug("stop",e)
					exception = e
				End Try
			End If

			If boundJndiUrl IsNot Nothing Then
				Try
					If tracing Then logger.trace("stop", "unbind from external directory: " & boundJndiUrl)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim usemap As Dictionary(Of ?, ?) = com.sun.jmx.remote.util.EnvHelp.mapToHashtable(attributes)

					Dim ctx As New javax.naming.InitialContext(usemap)

					ctx.unbind(boundJndiUrl)

					ctx.close()
				Catch e As javax.naming.NamingException
					If tracing Then logger.trace("stop", "failed to unbind RMI server: " & e)
					If logger.debugOn() Then logger.debug("stop",e)
					' fit e in as the nested exception if we are on 1.4
					If exception Is Nothing Then exception = newIOException("Cannot bind to URL: " & e, e)
				End Try
			End If

			If exception IsNot Nothing Then Throw exception

			If tracing Then logger.trace("stop", "stopped")
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Overrides active As Boolean
			Get
				Return (state = STARTED)
			End Get
		End Property

		Public Property Overrides address As javax.management.remote.JMXServiceURL
			Get
				If Not active Then Return Nothing
				Return address
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property Overrides attributes As IDictionary(Of String, ?)
			Get
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim map As IDictionary(Of String, ?) = com.sun.jmx.remote.util.EnvHelp.filterAttributes(attributes)
				Return java.util.Collections.unmodifiableMap(map)
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Property mBeanServerForwarder As javax.management.remote.MBeanServerForwarder
			Set(ByVal mbsf As javax.management.remote.MBeanServerForwarder)
				MyBase.mBeanServerForwarder = mbsf
				If rmiServerImpl IsNot Nothing Then rmiServerImpl.mBeanServer = mBeanServer
			End Set
		End Property

	'     We repeat the definitions of connection{Opened,Closed,Failed}
	'       here so that they are accessible to other classes in this package
	'       even though they have protected access.  

		Protected Friend Overrides Sub connectionOpened(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)
			MyBase.connectionOpened(connectionId, message, userData)
		End Sub

		Protected Friend Overrides Sub connectionClosed(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)
			MyBase.connectionClosed(connectionId, message, userData)
		End Sub

		Protected Friend Overrides Sub connectionFailed(ByVal connectionId As String, ByVal message As String, ByVal userData As Object)
			MyBase.connectionFailed(connectionId, message, userData)
		End Sub

		''' <summary>
		''' Bind a stub to a registry. </summary>
		''' <param name="jndiUrl"> URL of the stub in the registry, extracted
		'''        from the <code>JMXServiceURL</code>. </param>
		''' <param name="attributes"> A Hashtable containing environment parameters,
		'''        built from the Map specified at this object creation. </param>
		''' <param name="rmiServer"> The object to bind in the registry </param>
		''' <param name="rebind"> true if the object must be rebound.
		'''  </param>
		Friend Overridable Sub bind(Of T1)(ByVal jndiUrl As String, ByVal attributes As Dictionary(Of T1), ByVal rmiServer As RMIServer, ByVal rebind As Boolean)
			' if jndiURL is not null, we nust bind the stub to a
			' directory.
			Dim ctx As New javax.naming.InitialContext(attributes)

			If rebind Then
				ctx.rebind(jndiUrl, rmiServer)
			Else
				ctx.bind(jndiUrl, rmiServer)
			End If
			ctx.close()
		End Sub

		''' <summary>
		''' Creates a new RMIServerImpl.
		''' 
		''' </summary>
		Friend Overridable Function newServer() As RMIServerImpl
			Dim iiop As Boolean = isIiopURL(address,True)
			Dim port As Integer
			If address Is Nothing Then
				port = 0
			Else
				port = address.port
			End If
			If iiop Then
				Return newIIOPServer(attributes)
			Else
				Return newJRMPServer(attributes, port)
			End If
		End Function

		''' <summary>
		''' Encode a stub into the JMXServiceURL. </summary>
		''' <param name="rmiServer"> The stub object to encode in the URL </param>
		''' <param name="attributes"> A Map containing environment parameters,
		'''        built from the Map specified at this object creation.
		'''  </param>
		Private Sub encodeStubInAddress(Of T1)(ByVal rmiServer As RMIServer, ByVal attributes As IDictionary(Of T1))

			Dim protocol, host As String
			Dim port As Integer

			If address Is Nothing Then
				If com.sun.jmx.remote.internal.IIOPHelper.isStub(rmiServer) Then
					protocol = "iiop"
				Else
					protocol = "rmi"
				End If
				host = Nothing ' will default to local host name
				port = 0
			Else
				protocol = address.protocol
				host = If(address.host.Equals(""), Nothing, address.host)
				port = address.port
			End If

			Dim urlPath As String = encodeStub(rmiServer, attributes)

			address = New javax.management.remote.JMXServiceURL(protocol, host, port, urlPath)
		End Sub

		Friend Shared Function isIiopURL(ByVal directoryURL As javax.management.remote.JMXServiceURL, ByVal [strict] As Boolean) As Boolean
			Dim protocol As String = directoryURL.protocol
			If protocol.Equals("rmi") Then
				Return False
			ElseIf protocol.Equals("iiop") Then
				Return True
			ElseIf [strict] Then

				Throw New java.net.MalformedURLException("URL must have protocol " & """rmi"" or ""iiop"": """ & protocol & """")
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the IOR of the given rmiServer.
		''' 
		''' </summary>
		Friend Shared Function encodeStub(Of T1)(ByVal rmiServer As RMIServer, ByVal env As IDictionary(Of T1)) As String
			If com.sun.jmx.remote.internal.IIOPHelper.isStub(rmiServer) Then
				Return "/ior/" & encodeIIOPStub(rmiServer, env)
			Else
				Return "/stub/" & encodeJRMPStub(rmiServer, env)
			End If
		End Function

		Friend Shared Function encodeJRMPStub(Of T1)(ByVal rmiServer As RMIServer, ByVal env As IDictionary(Of T1)) As String
			Dim bout As New java.io.ByteArrayOutputStream
			Dim oout As New java.io.ObjectOutputStream(bout)
			oout.writeObject(rmiServer)
			oout.close()
			Dim bytes As SByte() = bout.toByteArray()
			Return byteArrayToBase64(bytes)
		End Function

		Friend Shared Function encodeIIOPStub(Of T1)(ByVal rmiServer As RMIServer, ByVal env As IDictionary(Of T1)) As String
			Try
				Dim orb As Object = com.sun.jmx.remote.internal.IIOPHelper.getOrb(rmiServer)
				Return com.sun.jmx.remote.internal.IIOPHelper.objectToString(orb, rmiServer)
			Catch x As Exception
				Throw newIOException(x.Message, x)
			End Try
		End Function

		''' <summary>
		''' Object that we will bind to the registry.
		''' This object is a stub connected to our RMIServerImpl.
		''' 
		''' </summary>
		Private Shared Function objectToBind(Of T1)(ByVal rmiServer As RMIServerImpl, ByVal env As IDictionary(Of T1)) As RMIServer
			Return RMIConnector.connectStub(CType(rmiServer.toStub(), RMIServer),env)
		End Function

		Private Shared Function newJRMPServer(Of T1)(ByVal env As IDictionary(Of T1), ByVal port As Integer) As RMIServerImpl
			Dim csf As java.rmi.server.RMIClientSocketFactory = CType(env(RMI_CLIENT_SOCKET_FACTORY_ATTRIBUTE), java.rmi.server.RMIClientSocketFactory)
			Dim ssf As java.rmi.server.RMIServerSocketFactory = CType(env(RMI_SERVER_SOCKET_FACTORY_ATTRIBUTE), java.rmi.server.RMIServerSocketFactory)
			Return New RMIJRMPServerImpl(port, csf, ssf, env)
		End Function

		Private Shared Function newIIOPServer(Of T1)(ByVal env As IDictionary(Of T1)) As RMIServerImpl
			Return New RMIIIOPServerImpl(env)
		End Function

		Private Shared Function byteArrayToBase64(ByVal a As SByte()) As String
			Dim aLen As Integer = a.Length
			Dim numFullGroups As Integer = aLen\3
			Dim numBytesInPartialGroup As Integer = aLen - 3*numFullGroups
			Dim resultLen As Integer = 4*((aLen + 2)\3)
			Dim result As New StringBuilder(resultLen)

			' Translate all full groups from byte array elements to Base64
			Dim inCursor As Integer = 0
			For i As Integer = 0 To numFullGroups - 1
				Dim byte0 As Integer = a(inCursor) And &Hff
				inCursor += 1
				Dim byte1 As Integer = a(inCursor) And &Hff
				inCursor += 1
				Dim byte2 As Integer = a(inCursor) And &Hff
				inCursor += 1
				result.Append(intToAlpha(byte0 >> 2))
				result.Append(intToAlpha((byte0 << 4) And &H3f Or (byte1 >> 4)))
				result.Append(intToAlpha((byte1 << 2) And &H3f Or (byte2 >> 6)))
				result.Append(intToAlpha(byte2 And &H3f))
			Next i

			' Translate partial group if present
			If numBytesInPartialGroup <> 0 Then
				Dim byte0 As Integer = a(inCursor) And &Hff
				inCursor += 1
				result.Append(intToAlpha(byte0 >> 2))
				If numBytesInPartialGroup = 1 Then
					result.Append(intToAlpha((byte0 << 4) And &H3f))
					result.Append("==")
				Else
					' assert numBytesInPartialGroup == 2;
					Dim byte1 As Integer = a(inCursor) And &Hff
					inCursor += 1
					result.Append(intToAlpha((byte0 << 4) And &H3f Or (byte1 >> 4)))
					result.Append(intToAlpha((byte1 << 2) And &H3f))
					result.Append("="c)
				End If
			End If
			' assert inCursor == a.length;
			' assert result.length() == resultLen;
			Return result.ToString()
		End Function

		''' <summary>
		''' This array is a lookup table that translates 6-bit positive integer
		''' index values into their "Base64 Alphabet" equivalents as specified
		''' in Table 1 of RFC 2045.
		''' </summary>
		Private Shared ReadOnly intToAlpha As Char() = { "A"c, "B"c, "C"c, "D"c, "E"c, "F"c, "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "+"c, "/"c }

		''' <summary>
		''' Construct a new IOException with a nested exception.
		''' The nested exception is set only if JDK {@literal >= 1.4}
		''' </summary>
		Private Shared Function newIOException(ByVal message As String, ByVal cause As Exception) As java.io.IOException
			Dim x As New java.io.IOException(message)
			Return com.sun.jmx.remote.util.EnvHelp.initCause(x,cause)
		End Function


		' Private variables
		' -----------------

		Private Shared logger As New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.rmi", "RMIConnectorServer")

		Private address As javax.management.remote.JMXServiceURL
		Private rmiServerImpl As RMIServerImpl
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private ReadOnly attributes As IDictionary(Of String, ?)
		Private defaultClassLoader As ClassLoader = Nothing

		Private boundJndiUrl As String

		' state
		Private Const CREATED As Integer = 0
		Private Const STARTED As Integer = 1
		Private Const STOPPED As Integer = 2

		Private state As Integer = CREATED
		Private Shared ReadOnly openedServers As java.util.Set(Of RMIConnectorServer) = New HashSet(Of RMIConnectorServer)
	End Class

End Namespace