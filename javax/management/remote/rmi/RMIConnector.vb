Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading

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
	''' <p>A connection to a remote RMI connector.  Usually, such
	''' connections are made using {@link
	''' javax.management.remote.JMXConnectorFactory JMXConnectorFactory}.
	''' However, specialized applications can use this class directly, for
	''' example with an <seealso cref="RMIServer"/> stub obtained without going
	''' through JNDI.</p>
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class RMIConnector
		Implements javax.management.remote.JMXConnector, javax.management.remote.JMXAddressable

		Private Shared ReadOnly logger As New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.rmi", "RMIConnector")

		Private Const serialVersionUID As Long = 817323035842634473L

		Private Sub New(Of T1)(ByVal rmiServer As RMIServer, ByVal address As javax.management.remote.JMXServiceURL, ByVal environment As IDictionary(Of T1))
			If rmiServer Is Nothing AndAlso address Is Nothing Then Throw New System.ArgumentException("rmiServer and jmxServiceURL both null")
			initTransients()

			Me.rmiServer = rmiServer
			Me.jmxServiceURL = address
			If environment Is Nothing Then
				Me.env = java.util.Collections.emptyMap()
			Else
				com.sun.jmx.remote.util.EnvHelp.checkAttributes(environment)
				Me.env = java.util.Collections.unmodifiableMap(environment)
			End If
		End Sub

		''' <summary>
		''' <p>Constructs an <code>RMIConnector</code> that will connect
		''' the RMI connector server with the given address.</p>
		''' 
		''' <p>The address can refer directly to the connector server,
		''' using one of the following syntaxes:</p>
		''' 
		''' <pre>
		''' service:jmx:rmi://<em>[host[:port]]</em>/stub/<em>encoded-stub</em>
		''' service:jmx:iiop://<em>[host[:port]]</em>/ior/<em>encoded-IOR</em>
		''' </pre>
		''' 
		''' <p>(Here, the square brackets <code>[]</code> are not part of the
		''' address but indicate that the host and port are optional.)</p>
		''' 
		''' <p>The address can instead indicate where to find an RMI stub
		''' through JNDI, using one of the following syntaxes:</p>
		''' 
		''' <pre>
		''' service:jmx:rmi://<em>[host[:port]]</em>/jndi/<em>jndi-name</em>
		''' service:jmx:iiop://<em>[host[:port]]</em>/jndi/<em>jndi-name</em>
		''' </pre>
		''' 
		''' <p>An implementation may also recognize additional address
		''' syntaxes, for example:</p>
		''' 
		''' <pre>
		''' service:jmx:iiop://<em>[host[:port]]</em>/stub/<em>encoded-stub</em>
		''' </pre>
		''' </summary>
		''' <param name="url"> the address of the RMI connector server.
		''' </param>
		''' <param name="environment"> additional attributes specifying how to make
		''' the connection.  For JNDI-based addresses, these attributes can
		''' usefully include JNDI attributes recognized by {@link
		''' InitialContext#InitialContext(Hashtable) InitialContext}.  This
		''' parameter can be null, which is equivalent to an empty Map.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>url</code>
		''' is null. </exception>
		Public Sub New(Of T1)(ByVal url As javax.management.remote.JMXServiceURL, ByVal environment As IDictionary(Of T1))
			Me.New(Nothing, url, environment)
		End Sub

		''' <summary>
		''' <p>Constructs an <code>RMIConnector</code> using the given RMI stub.
		''' </summary>
		''' <param name="rmiServer"> an RMI stub representing the RMI connector server. </param>
		''' <param name="environment"> additional attributes specifying how to make
		''' the connection.  This parameter can be null, which is
		''' equivalent to an empty Map.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>rmiServer</code>
		''' is null. </exception>
		Public Sub New(Of T1)(ByVal rmiServer As RMIServer, ByVal environment As IDictionary(Of T1))
			Me.New(rmiServer, Nothing, environment)
		End Sub

		''' <summary>
		''' <p>Returns a string representation of this object.  In general,
		''' the <code>toString</code> method returns a string that
		''' "textually represents" this object. The result should be a
		''' concise but informative representation that is easy for a
		''' person to read.</p>
		''' </summary>
		''' <returns> a String representation of this object.
		'''  </returns>
		Public Overrides Function ToString() As String
			Dim b As New StringBuilder(Me.GetType().name)
			b.Append(":")
			If rmiServer IsNot Nothing Then b.Append(" rmiServer=").append(rmiServer.ToString())
			If jmxServiceURL IsNot Nothing Then
				If rmiServer IsNot Nothing Then b.Append(",")
				b.Append(" jmxServiceURL=").append(jmxServiceURL.ToString())
			End If
			Return b.ToString()
		End Function

		''' <summary>
		''' <p>The address of this connector.</p>
		''' </summary>
		''' <returns> the address of this connector, or null if it
		''' does not have one.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property address As javax.management.remote.JMXServiceURL
			Get
				Return jmxServiceURL
			End Get
		End Property

		'--------------------------------------------------------------------
		' implements JMXConnector interface
		'--------------------------------------------------------------------

		''' <exception cref="IOException"> if the connection could not be made because of a
		'''   communication problem, or in the case of the {@code iiop} protocol,
		'''   that RMI/IIOP is not supported </exception>
		Public Overridable Sub connect()
			connect(Nothing)
		End Sub

		''' <exception cref="IOException"> if the connection could not be made because of a
		'''   communication problem, or in the case of the {@code iiop} protocol,
		'''   that RMI/IIOP is not supported </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub connect(Of T1)(ByVal environment As IDictionary(Of T1))
			Dim tracing As Boolean = logger.traceOn()
			Dim idstr As String = (If(tracing, "[" & Me.ToString() & "]", Nothing))

			If terminated Then
				logger.trace("connect",idstr & " already closed.")
				Throw New java.io.IOException("Connector closed")
			End If
			If connected Then
				logger.trace("connect",idstr & " already connected.")
				Return
			End If

			Try
				If tracing Then logger.trace("connect",idstr & " connecting...")

				Dim usemap As IDictionary(Of String, Object) = New Dictionary(Of String, Object)(If(Me.env Is Nothing, java.util.Collections.emptyMap(Of String, Object)(), Me.env))


				If environment IsNot Nothing Then
					com.sun.jmx.remote.util.EnvHelp.checkAttributes(environment)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
					usemap.putAll(environment)
				End If

				' Get RMIServer stub from directory or URL encoding if needed.
				If tracing Then logger.trace("connect",idstr & " finding stub...")
				Dim stub As RMIServer = If(rmiServer IsNot Nothing, rmiServer, findRMIServer(jmxServiceURL, usemap))

				' Check for secure RMIServer stub if the corresponding
				' client-side environment property is set to "true".
				'
				Dim stringBoolean As String = CStr(usemap("jmx.remote.x.check.stub"))
				Dim checkStub As Boolean = com.sun.jmx.remote.util.EnvHelp.computeBooleanFromString(stringBoolean)

				If checkStub Then checkStub(stub, rmiServerImplStubClass)

				' Connect IIOP Stub if needed.
				If tracing Then logger.trace("connect",idstr & " connecting stub...")
				stub = connectStub(stub,usemap)
				idstr = (If(tracing, "[" & Me.ToString() & "]", Nothing))

				' Calling newClient on the RMIServer stub.
				If tracing Then logger.trace("connect",idstr & " getting connection...")
				Dim credentials As Object = usemap(CREDENTIALS)

				Try
					connection = getConnection(stub, credentials, checkStub)
				Catch re As java.rmi.RemoteException
					If jmxServiceURL IsNot Nothing Then
						Dim pro As String = jmxServiceURL.protocol
						Dim path As String = jmxServiceURL.uRLPath

						If "rmi".Equals(pro) AndAlso path.StartsWith("/jndi/iiop:") Then
							Dim mfe As New java.net.MalformedURLException("Protocol is rmi but JNDI scheme is iiop: " & jmxServiceURL)
							mfe.initCause(re)
							Throw mfe
						End If
					End If
					Throw re
				End Try

				' Always use one of:
				'   ClassLoader provided in Map at connect time,
				'   or contextClassLoader at connect time.
				If tracing Then logger.trace("connect",idstr & " getting class loader...")
				defaultClassLoader = com.sun.jmx.remote.util.EnvHelp.resolveClientClassLoader(usemap)

				usemap(javax.management.remote.JMXConnectorFactory.DEFAULT_CLASS_LOADER) = defaultClassLoader

				rmiNotifClient = New RMINotifClient(Me, defaultClassLoader, usemap)

				env = usemap
				Dim checkPeriod As Long = com.sun.jmx.remote.util.EnvHelp.getConnectionCheckPeriod(usemap)
				communicatorAdmin = New RMIClientCommunicatorAdmin(Me, checkPeriod)

				connected = True

				' The connectionId variable is used in doStart(), when
				' reconnecting, to identify the "old" connection.
				'
				connectionId = connectionId

				Dim connectedNotif As javax.management.Notification = New javax.management.remote.JMXConnectionNotification(javax.management.remote.JMXConnectionNotification.OPENED, Me, connectionId, clientNotifSeqNo, "Successful connection", Nothing)
				clientNotifSeqNo += 1
				sendNotification(connectedNotif)

				If tracing Then logger.trace("connect",idstr & " done...")
			Catch e As java.io.IOException
				If tracing Then logger.trace("connect",idstr & " failed to connect: " & e)
				Throw e
			Catch e As Exception
				If tracing Then logger.trace("connect",idstr & " failed to connect: " & e)
				Throw e
			Catch e As javax.naming.NamingException
				Dim msg As String = "Failed to retrieve RMIServer stub: " & e
				If tracing Then logger.trace("connect",idstr & " " & msg)
				Throw com.sun.jmx.remote.util.EnvHelp.initCause(New java.io.IOException(msg),e)
			End Try
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property connectionId As String
			Get
				If terminated OrElse (Not connected) Then
					If logger.traceOn() Then logger.trace("getConnectionId","[" & Me.ToString() & "] not connected.")
    
					Throw New java.io.IOException("Not connected")
				End If
    
				' we do a remote call to have an IOException if the connection is broken.
				' see the bug 4939578
				Return connection.connectionId
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property mBeanServerConnection As javax.management.MBeanServerConnection
			Get
				Return getMBeanServerConnection(Nothing)
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getMBeanServerConnection(ByVal delegationSubject As javax.security.auth.Subject) As javax.management.MBeanServerConnection

			If terminated Then
				If logger.traceOn() Then logger.trace("getMBeanServerConnection","[" & Me.ToString() & "] already closed.")
				Throw New java.io.IOException("Connection closed")
			ElseIf Not connected Then
				If logger.traceOn() Then logger.trace("getMBeanServerConnection","[" & Me.ToString() & "] is not connected.")
				Throw New java.io.IOException("Not connected")
			End If

			Return getConnectionWithSubject(delegationSubject)
		End Function

		Public Overridable Sub addConnectionNotificationListener(ByVal listener As javax.management.NotificationListener, ByVal filter As javax.management.NotificationFilter, ByVal handback As Object)
			If listener Is Nothing Then Throw New NullPointerException("listener")
			connectionBroadcaster.addNotificationListener(listener, filter, handback)
		End Sub

		Public Overridable Sub removeConnectionNotificationListener(ByVal listener As javax.management.NotificationListener)
			If listener Is Nothing Then Throw New NullPointerException("listener")
			connectionBroadcaster.removeNotificationListener(listener)
		End Sub

		Public Overridable Sub removeConnectionNotificationListener(ByVal listener As javax.management.NotificationListener, ByVal filter As javax.management.NotificationFilter, ByVal handback As Object)
			If listener Is Nothing Then Throw New NullPointerException("listener")
			connectionBroadcaster.removeNotificationListener(listener, filter, handback)
		End Sub

		Private Sub sendNotification(ByVal n As javax.management.Notification)
			connectionBroadcaster.sendNotification(n)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub close()
			close(False)
		End Sub

		' allows to do close after setting the flag "terminated" to true.
		' It is necessary to avoid a deadlock, see 6296324
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub close(ByVal intern As Boolean)
			Dim tracing As Boolean = logger.traceOn()
			Dim debug As Boolean = logger.debugOn()
			Dim idstr As String = (If(tracing, "[" & Me.ToString() & "]", Nothing))

			If Not intern Then
				' Return if already cleanly closed.
				'
				If terminated Then
					If closeException Is Nothing Then
						If tracing Then logger.trace("close",idstr & " already closed.")
						Return
					End If
				Else
					terminated = True
				End If
			End If

			If closeException IsNot Nothing AndAlso tracing Then
				' Already closed, but not cleanly. Attempt again.
				'
				If tracing Then
					logger.trace("close",idstr & " had failed: " & closeException)
					logger.trace("close",idstr & " attempting to close again.")
				End If
			End If

			Dim savedConnectionId As String = Nothing
			If connected Then savedConnectionId = connectionId

			closeException = Nothing

			If tracing Then logger.trace("close",idstr & " closing.")

			If communicatorAdmin IsNot Nothing Then communicatorAdmin.terminate()

			If rmiNotifClient IsNot Nothing Then
				Try
					rmiNotifClient.terminate()
					If tracing Then logger.trace("close",idstr & " RMI Notification client terminated.")
				Catch x As Exception
					closeException = x
					If tracing Then logger.trace("close",idstr & " Failed to terminate RMI Notification client: " & x)
					If debug Then logger.debug("close",x)
				End Try
			End If

			If connection IsNot Nothing Then
				Try
					connection.close()
					If tracing Then logger.trace("close",idstr & " closed.")
				Catch nse As java.rmi.NoSuchObjectException
					' OK, the server maybe closed itself.
				Catch e As java.io.IOException
					closeException = e
					If tracing Then logger.trace("close",idstr & " Failed to close RMIServer: " & e)
					If debug Then logger.debug("close",e)
				End Try
			End If

			' Clean up MBeanServerConnection table
			'
			rmbscMap.clear()

	'         Send notification of closure.  We don't do this if the user
	'         * never called connect() on the connector, because there's no
	'         * connection id in that case.  

			If savedConnectionId IsNot Nothing Then
				Dim closedNotif As javax.management.Notification = New javax.management.remote.JMXConnectionNotification(javax.management.remote.JMXConnectionNotification.CLOSED, Me, savedConnectionId, clientNotifSeqNo, "Client has been closed", Nothing)
				clientNotifSeqNo += 1
				sendNotification(closedNotif)
			End If

			' throw exception if needed
			'
			If closeException IsNot Nothing Then
				If tracing Then logger.trace("close",idstr & " failed to close: " & closeException)
				If TypeOf closeException Is java.io.IOException Then Throw CType(closeException, java.io.IOException)
				If TypeOf closeException Is Exception Then Throw CType(closeException, Exception)
				Dim x As New java.io.IOException("Failed to close: " & closeException)
				Throw com.sun.jmx.remote.util.EnvHelp.initCause(x,closeException)
			End If
		End Sub

		' added for re-connection
		Private Function addListenerWithSubject(ByVal name As javax.management.ObjectName, ByVal filter As java.rmi.MarshalledObject(Of javax.management.NotificationFilter), ByVal delegationSubject As javax.security.auth.Subject, ByVal reconnect As Boolean) As Integer?

			Dim debug As Boolean = logger.debugOn()
			If debug Then logger.debug("addListenerWithSubject", "(ObjectName,MarshalledObject,Subject)")

			Dim names As javax.management.ObjectName() = {name}
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim filters As java.rmi.MarshalledObject(Of javax.management.NotificationFilter)() = com.sun.jmx.mbeanserver.Util.cast(New java.rmi.MarshalledObject(Of ?)() {filter})
			Dim delegationSubjects As javax.security.auth.Subject() = { delegationSubject }

			Dim listenerIDs As Integer?() = addListenersWithSubjects(names,filters,delegationSubjects, reconnect)

			If debug Then logger.debug("addListenerWithSubject","listenerID=" & listenerIDs(0))
			Return listenerIDs(0)
		End Function

		' added for re-connection
		Private Function addListenersWithSubjects(ByVal names As javax.management.ObjectName(), ByVal filters As java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(), ByVal delegationSubjects As javax.security.auth.Subject(), ByVal reconnect As Boolean) As Integer?()

			Dim debug As Boolean = logger.debugOn()
			If debug Then logger.debug("addListenersWithSubjects", "(ObjectName[],MarshalledObject[],Subject[])")

			Dim old As ClassLoader = pushDefaultClassLoader()
			Dim listenerIDs As Integer?() = Nothing

			Try
				listenerIDs = connection.addNotificationListeners(names, filters, delegationSubjects)
			Catch noe As java.rmi.NoSuchObjectException
				' maybe reconnect
				If reconnect Then
					communicatorAdmin.gotIOException(noe)

					listenerIDs = connection.addNotificationListeners(names, filters, delegationSubjects)
				Else
					Throw noe
				End If
			Catch ioe As java.io.IOException
				' send a failed notif if necessary
				communicatorAdmin.gotIOException(ioe)
			Finally
				popDefaultClassLoader(old)
			End Try

			If debug Then logger.debug("addListenersWithSubjects","registered " & (If(listenerIDs Is Nothing, 0, listenerIDs.Length)) & " listener(s)")
			Return listenerIDs
		End Function

		'--------------------------------------------------------------------
		' Implementation of MBeanServerConnection
		'--------------------------------------------------------------------
		Private Class RemoteMBeanServerConnection
			Implements javax.management.MBeanServerConnection

			Private ReadOnly outerInstance As RMIConnector

			Private delegationSubject As javax.security.auth.Subject

			Public Sub New(ByVal outerInstance As RMIConnector)
					Me.outerInstance = outerInstance
				Me.New(Nothing)
			End Sub

			Public Sub New(ByVal outerInstance As RMIConnector, ByVal delegationSubject As javax.security.auth.Subject)
					Me.outerInstance = outerInstance
				Me.delegationSubject = delegationSubject
			End Sub

			Public Overridable Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName) As javax.management.ObjectInstance
				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName)", "className=" & className & ", name=" & name)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.createMBean(className, name, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.createMBean(className, name, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try
			End Function

			Public Overridable Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName, ByVal loaderName As javax.management.ObjectName) As javax.management.ObjectInstance

				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName,ObjectName)", "className=" & className & ", name=" & name & ", loaderName=" & loaderName & ")")

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.createMBean(className, name, loaderName, delegationSubject)

				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.createMBean(className, name, loaderName, delegationSubject)

				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try
			End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public javax.management.ObjectInstance createMBean(String className, javax.management.ObjectName name, Object params() , String signature()) throws javax.management.ReflectionException, javax.management.InstanceAlreadyExistsException, javax.management.MBeanRegistrationException, javax.management.MBeanException, javax.management.NotCompliantMBeanException, java.io.IOException
				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName,Object[],String[])", "className=" & className & ", name=" & name & ", signature=" & strings(signature))

				Dim sParams As New java.rmi.MarshalledObject(Of Object())(params)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.createMBean(className, name, sParams, signature, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.createMBean(className, name, sParams, signature, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public javax.management.ObjectInstance createMBean(String className, javax.management.ObjectName name, javax.management.ObjectName loaderName, Object params() , String signature()) throws javax.management.ReflectionException, javax.management.InstanceAlreadyExistsException, javax.management.MBeanRegistrationException, javax.management.MBeanException, javax.management.NotCompliantMBeanException, javax.management.InstanceNotFoundException, java.io.IOException
				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName,ObjectName,Object[],String[])", "className=" & className & ", name=" & name & ", loaderName=" & loaderName & ", signature=" & strings(signature))

				Dim sParams As New java.rmi.MarshalledObject(Of Object())(params)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.createMBean(className, name, loaderName, sParams, signature, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.createMBean(className, name, loaderName, sParams, signature, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public void unregisterMBean(javax.management.ObjectName name) throws javax.management.InstanceNotFoundException, javax.management.MBeanRegistrationException, java.io.IOException
				If logger.debugOn() Then logger.debug("unregisterMBean", "name=" & name)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.unregisterMBean(name, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.unregisterMBean(name, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public javax.management.ObjectInstance getObjectInstance(javax.management.ObjectName name) throws javax.management.InstanceNotFoundException, java.io.IOException
				If logger.debugOn() Then logger.debug("getObjectInstance", "name=" & name)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getObjectInstance(name, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getObjectInstance(name, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public java.util.Set(Of javax.management.ObjectInstance) queryMBeans(javax.management.ObjectName name, javax.management.QueryExp query) throws java.io.IOException
				If logger.debugOn() Then logger.debug("queryMBeans", "name=" & name & ", query=" & query)

				Dim sQuery As New java.rmi.MarshalledObject(Of javax.management.QueryExp)(query)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.queryMBeans(name, sQuery, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.queryMBeans(name, sQuery, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public java.util.Set(Of javax.management.ObjectName) queryNames(javax.management.ObjectName name, javax.management.QueryExp query) throws java.io.IOException
				If logger.debugOn() Then logger.debug("queryNames", "name=" & name & ", query=" & query)

				Dim sQuery As New java.rmi.MarshalledObject(Of javax.management.QueryExp)(query)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.queryNames(name, sQuery, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.queryNames(name, sQuery, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public Boolean isRegistered(javax.management.ObjectName name) throws java.io.IOException
				If logger.debugOn() Then logger.debug("isRegistered", "name=" & name)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.isRegistered(name, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.isRegistered(name, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public Integer? mBeanCount throws java.io.IOException
				If logger.debugOn() Then logger.debug("getMBeanCount", "")

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getMBeanCount(delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getMBeanCount(delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public Object getAttribute(javax.management.ObjectName name, String attribute) throws javax.management.MBeanException, javax.management.AttributeNotFoundException, javax.management.InstanceNotFoundException, javax.management.ReflectionException, java.io.IOException
				If logger.debugOn() Then logger.debug("getAttribute", "name=" & name & ", attribute=" & attribute)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getAttribute(name, attribute, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getAttribute(name, attribute, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public javax.management.AttributeList getAttributes(javax.management.ObjectName name, String() attributes) throws javax.management.InstanceNotFoundException, javax.management.ReflectionException, java.io.IOException
				If logger.debugOn() Then logger.debug("getAttributes", "name=" & name & ", attributes=" & strings(attributes))

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getAttributes(name, attributes, delegationSubject)

				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getAttributes(name, attributes, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try


			public void attributeute(javax.management.ObjectName name, javax.management.Attribute attribute) throws javax.management.InstanceNotFoundException, javax.management.AttributeNotFoundException, javax.management.InvalidAttributeValueException, javax.management.MBeanException, javax.management.ReflectionException, java.io.IOException

				If logger.debugOn() Then logger.debug("setAttribute", "name=" & name & ", attribute name=" & attribute.name)

				Dim sAttribute As New java.rmi.MarshalledObject(Of javax.management.Attribute)(attribute)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.attributeute(name, sAttribute, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.attributeute(name, sAttribute, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public javax.management.AttributeList attributestes(javax.management.ObjectName name, javax.management.AttributeList attributes) throws javax.management.InstanceNotFoundException, javax.management.ReflectionException, java.io.IOException

				If logger.debugOn() Then logger.debug("setAttributes", "name=" & name & ", attribute names=" & getAttributesNames(attributes))

				Dim sAttributes As New java.rmi.MarshalledObject(Of javax.management.AttributeList)(attributes)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.attributestes(name, sAttributes, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.attributestes(name, sAttributes, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try


			public Object invoke(javax.management.ObjectName name, String operationName, Object params() , String signature()) throws javax.management.InstanceNotFoundException, javax.management.MBeanException, javax.management.ReflectionException, java.io.IOException

				If logger.debugOn() Then logger.debug("invoke", "name=" & name & ", operationName=" & operationName & ", signature=" & strings(signature))

				Dim sParams As New java.rmi.MarshalledObject(Of Object())(params)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.invoke(name, operationName, sParams, signature, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.invoke(name, operationName, sParams, signature, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try


			public String defaultDomain throws java.io.IOException
				If logger.debugOn() Then logger.debug("getDefaultDomain", "")

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getDefaultDomain(delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getDefaultDomain(delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public String() domains throws java.io.IOException
				If logger.debugOn() Then logger.debug("getDomains", "")

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getDomains(delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getDomains(delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public javax.management.MBeanInfo getMBeanInfo(javax.management.ObjectName name) throws javax.management.InstanceNotFoundException, javax.management.IntrospectionException, javax.management.ReflectionException, java.io.IOException

				If logger.debugOn() Then logger.debug("getMBeanInfo", "name=" & name)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.getMBeanInfo(name, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.getMBeanInfo(name, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try


			public Boolean isInstanceOf(javax.management.ObjectName name, String className) throws javax.management.InstanceNotFoundException, java.io.IOException
				If logger.debugOn() Then logger.debug("isInstanceOf", "name=" & name & ", className=" & className)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					Return outerInstance.connection.isInstanceOf(name, className, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					Return outerInstance.connection.isInstanceOf(name, className, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public void addNotificationListener(javax.management.ObjectName name, javax.management.ObjectName listener, javax.management.NotificationFilter filter, Object handback) throws javax.management.InstanceNotFoundException, java.io.IOException

				If logger.debugOn() Then logger.debug("addNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "name=" & name & ", listener=" & listener & ", filter=" & filter & ", handback=" & handback)

				Dim sFilter As New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(filter)
				Dim sHandback As New java.rmi.MarshalledObject(Of Object)(handback)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.addNotificationListener(name, listener, sFilter, sHandback, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.addNotificationListener(name, listener, sFilter, sHandback, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public void removeNotificationListener(javax.management.ObjectName name, javax.management.ObjectName listener) throws javax.management.InstanceNotFoundException, javax.management.ListenerNotFoundException, java.io.IOException

				If logger.debugOn() Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName)", "name=" & name & ", listener=" & listener)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.removeNotificationListener(name, listener, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.removeNotificationListener(name, listener, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			public void removeNotificationListener(javax.management.ObjectName name, javax.management.ObjectName listener, javax.management.NotificationFilter filter, Object handback) throws javax.management.InstanceNotFoundException, javax.management.ListenerNotFoundException, java.io.IOException
				If logger.debugOn() Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "name=" & name & ", listener=" & listener & ", filter=" & filter & ", handback=" & handback)

				Dim sFilter As New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(filter)
				Dim sHandback As New java.rmi.MarshalledObject(Of Object)(handback)
				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.removeNotificationListener(name, listener, sFilter, sHandback, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.removeNotificationListener(name, listener, sFilter, sHandback, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

			' Specific Notification Handle ----------------------------------

			public void addNotificationListener(javax.management.ObjectName name, javax.management.NotificationListener listener, javax.management.NotificationFilter filter, Object handback) throws javax.management.InstanceNotFoundException, java.io.IOException

				Dim debug As Boolean = logger.debugOn()

				If debug Then logger.debug("addNotificationListener" & "(ObjectName,NotificationListener," & "NotificationFilter,Object)", "name=" & name & ", listener=" & listener & ", filter=" & filter & ", handback=" & handback)

				Dim listenerID As Integer? = outerInstance.addListenerWithSubject(name, New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(filter), delegationSubject,True)
				outerInstance.rmiNotifClient.addNotificationListener(listenerID, name, listener, filter, handback, delegationSubject)

			public void removeNotificationListener(javax.management.ObjectName name, javax.management.NotificationListener listener) throws javax.management.InstanceNotFoundException, javax.management.ListenerNotFoundException, java.io.IOException

				Dim debug As Boolean = logger.debugOn()

				If debug Then logger.debug("removeNotificationListener" & "(ObjectName,NotificationListener)", "name=" & name & ", listener=" & listener)

				Dim ret As Integer?() = outerInstance.rmiNotifClient.removeNotificationListener(name, listener)

				If debug Then logger.debug("removeNotificationListener", "listenerIDs=" & objects(ret))

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()

				Try
					outerInstance.connection.removeNotificationListeners(name, ret, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.removeNotificationListeners(name, ret, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try


			public void removeNotificationListener(javax.management.ObjectName name, javax.management.NotificationListener listener, javax.management.NotificationFilter filter, Object handback) throws javax.management.InstanceNotFoundException, javax.management.ListenerNotFoundException, java.io.IOException
				Dim debug As Boolean = logger.debugOn()

				If debug Then logger.debug("removeNotificationListener" & "(ObjectName,NotificationListener," & "NotificationFilter,Object)", "name=" & name & ", listener=" & listener & ", filter=" & filter & ", handback=" & handback)

				Dim ret As Integer? = outerInstance.rmiNotifClient.removeNotificationListener(name, listener, filter, handback)

				If debug Then logger.debug("removeNotificationListener", "listenerID=" & ret)

				Dim old As ClassLoader = outerInstance.pushDefaultClassLoader()
				Try
					outerInstance.connection.removeNotificationListeners(name, New Integer?() {ret}, delegationSubject)
				Catch ioe As java.io.IOException
					outerInstance.communicatorAdmin.gotIOException(ioe)

					outerInstance.connection.removeNotificationListeners(name, New Integer?() {ret}, delegationSubject)
				Finally
					outerInstance.popDefaultClassLoader(old)
				End Try

		End Class

		'--------------------------------------------------------------------
		private class RMINotifClient extends com.sun.jmx.remote.internal.ClientNotifForwarder
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			public RMINotifClient(ClassLoader cl, IDictionary(Of String, ?) env)
				MyBase(cl, env)

			protected javax.management.remote.NotificationResult fetchNotifs(Long clientSequenceNumber, Integer maxNotifications, Long timeout) throws java.io.IOException, ClassNotFoundException

				Dim retried As Boolean = False
				Do ' used for a successful re-connection
							   ' or a transient network problem
					Try
						Return connection.fetchNotifications(clientSequenceNumber, maxNotifications, timeout) ' return normally
					Catch ioe As java.io.IOException
						' Examine the chain of exceptions to determine whether this
						' is a deserialization issue. If so - we propagate the
						' appropriate exception to the caller, who will then
						' proceed with fetching notifications one by one
						rethrowDeserializationException(ioe)

						Try
							communicatorAdmin.gotIOException(ioe)
							' reconnection OK, back to "while" to do again
						Catch ee As java.io.IOException
							Dim toClose As Boolean = False

							SyncLock Me
								If terminated Then
									' the connection is closed.
									Throw ioe
								ElseIf retried Then
									toClose = True
								End If
							End SyncLock

							If toClose Then
								' JDK-8049303
								' We received an IOException - but the communicatorAdmin
								' did not close the connection - possibly because
								' the original exception was raised by a transient network
								' problem?
								' We already know that this exception is not due to a deserialization
								' issue as we already took care of that before involving the
								' communicatorAdmin. Moreover - we already made one retry attempt
								' at fetching the same batch of notifications - and the
								' problem persisted.
								' Since trying again doesn't seem to solve the issue, we will now
								' close the connection. Doing otherwise might cause the
								' NotifFetcher thread to die silently.
								Dim failedNotif As javax.management.Notification = New javax.management.remote.JMXConnectionNotification(javax.management.remote.JMXConnectionNotification.FAILED, Me, connectionId, clientNotifSeqNo, "Failed to communicate with the server: " & ioe.ToString(), ioe)
								clientNotifSeqNo += 1

								sendNotification(failedNotif)

								Try
									close(True)
								Catch e As Exception
									' OK.
									' We are closing
								End Try
								Throw ioe ' the connection is closed here.
							Else
								' JDK-8049303 possible transient network problem,
								' let's try one more time
								retried = True
							End If
						End Try
					End Try
				Loop

			private void rethrowDeserializationException(java.io.IOException ioe) throws ClassNotFoundException, java.io.IOException
				' specially treating for an UnmarshalException
				If TypeOf ioe Is java.rmi.UnmarshalException Then
					Throw ioe ' the fix of 6937053 made ClientNotifForwarder.fetchNotifs
							   ' fetch one by one with UnmarshalException
				ElseIf TypeOf ioe Is java.rmi.MarshalException Then
					' IIOP will throw MarshalException wrapping a NotSerializableException
					' when a server fails to serialize a response.
					Dim [me] As java.rmi.MarshalException = CType(ioe, java.rmi.MarshalException)
					If TypeOf [me].detail Is java.io.NotSerializableException Then Throw CType([me].detail, java.io.NotSerializableException)
				End If

				' Not serialization problem, return.

			protected Integer? addListenerForMBeanRemovedNotif() throws java.io.IOException, javax.management.InstanceNotFoundException
				Dim clientFilter As New javax.management.NotificationFilterSupport
				clientFilter.enableType(javax.management.MBeanServerNotification.UNREGISTRATION_NOTIFICATION)
				Dim sFilter As New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(clientFilter)

				Dim listenerIDs As Integer?()
				Dim names As javax.management.ObjectName() = {javax.management.MBeanServerDelegate.DELEGATE_NAME}
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim filters As java.rmi.MarshalledObject(Of javax.management.NotificationFilter)() = com.sun.jmx.mbeanserver.Util.cast(New java.rmi.MarshalledObject(Of ?)() {sFilter})
				Dim subjects As javax.security.auth.Subject() = {Nothing}
				Try
					listenerIDs = connection.addNotificationListeners(names, filters, subjects)

				Catch ioe As java.io.IOException
					communicatorAdmin.gotIOException(ioe)

					listenerIDs = connection.addNotificationListeners(names, filters, subjects)
				End Try
				Return listenerIDs(0)

			protected void removeListenerForMBeanRemovedNotif(Integer? id) throws java.io.IOException, javax.management.InstanceNotFoundException, javax.management.ListenerNotFoundException
				Try
					connection.removeNotificationListeners(javax.management.MBeanServerDelegate.DELEGATE_NAME, New Integer?() {id}, Nothing)
				Catch ioe As java.io.IOException
					communicatorAdmin.gotIOException(ioe)

					connection.removeNotificationListeners(javax.management.MBeanServerDelegate.DELEGATE_NAME, New Integer?() {id}, Nothing)
				End Try


			protected void lostNotifs(String message, Long number)
				Const notifType As String = javax.management.remote.JMXConnectionNotification.NOTIFS_LOST

				Dim n As New javax.management.remote.JMXConnectionNotification(notifType, RMIConnector.this, connectionId, clientNotifCounter, message, Convert.ToInt64(number))
				clientNotifCounter += 1
				sendNotification(n)

		private class RMIClientCommunicatorAdmin extends com.sun.jmx.remote.internal.ClientCommunicatorAdmin
			public RMIClientCommunicatorAdmin(Long period)
				MyBase(period)

			public void gotIOException(java.io.IOException ioe) throws java.io.IOException
				If TypeOf ioe Is java.rmi.NoSuchObjectException Then
					' need to restart
					MyBase.gotIOException(ioe)

					Return
				End If

				' check if the connection is broken
				Try
					connection.getDefaultDomain(Nothing)
				Catch ioexc As java.io.IOException
					Dim toClose As Boolean = False

					SyncLock Me
						If Not terminated Then
							terminated = True

							toClose = True
						End If
					End SyncLock

					If toClose Then
						' we should close the connection,
						' but send a failed notif at first
						Dim failedNotif As javax.management.Notification = New javax.management.remote.JMXConnectionNotification(javax.management.remote.JMXConnectionNotification.FAILED, Me, connectionId, clientNotifSeqNo, "Failed to communicate with the server: " & ioe.ToString(), ioe)
						clientNotifSeqNo += 1

						sendNotification(failedNotif)

						Try
							close(True)
						Catch e As Exception
							' OK.
							' We are closing
						End Try
					End If
				End Try

				' forward the exception
				If TypeOf ioe Is java.rmi.ServerException Then
	'                 Need to unwrap the exception.
	'                   Some user-thrown exception at server side will be wrapped by
	'                   rmi into a ServerException.
	'                   For example, a RMIConnnectorServer will wrap a
	'                   ClassNotFoundException into a UnmarshalException, and rmi
	'                   will throw a ServerException at client side which wraps this
	'                   UnmarshalException.
	'                   No failed notif here.
	'                 
					Dim tt As Exception = CType(ioe, java.rmi.ServerException).detail

					If TypeOf tt Is java.io.IOException Then
						Throw CType(tt, java.io.IOException)
					ElseIf TypeOf tt Is Exception Then
						Throw CType(tt, Exception)
					End If
				End If

				Throw ioe

			public void reconnectNotificationListeners(com.sun.jmx.remote.internal.ClientListenerInfo() old) throws java.io.IOException
				Dim len As Integer = old.length
				Dim i As Integer

				Dim clis As com.sun.jmx.remote.internal.ClientListenerInfo() = New com.sun.jmx.remote.internal.ClientListenerInfo(len - 1){}

				Dim subjects As javax.security.auth.Subject() = New javax.security.auth.Subject(len - 1){}
				Dim names As javax.management.ObjectName() = New javax.management.ObjectName(len - 1){}
				Dim listeners As javax.management.NotificationListener() = New javax.management.NotificationListener(len - 1){}
				Dim filters As javax.management.NotificationFilter() = New javax.management.NotificationFilter(len - 1){}
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim mFilters As java.rmi.MarshalledObject(Of javax.management.NotificationFilter)() = com.sun.jmx.mbeanserver.Util.cast(New java.rmi.MarshalledObject(Of ?)(len - 1){})
				Dim handbacks As Object() = New Object(len - 1){}

				For i = 0 To len - 1
					subjects(i) = old(i).delegationSubject
					names(i) = old(i).objectName
					listeners(i) = old(i).listener
					filters(i) = old(i).notificationFilter
					mFilters(i) = New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(filters(i))
					handbacks(i) = old(i).handback
				Next i

				Try
					Dim ids As Integer?() = addListenersWithSubjects(names,mFilters,subjects,False)

					For i = 0 To len - 1
						clis(i) = New com.sun.jmx.remote.internal.ClientListenerInfo(ids(i), names(i), listeners(i), filters(i), handbacks(i), subjects(i))
					Next i

					rmiNotifClient.postReconnection(clis)

					Return
				Catch infe As javax.management.InstanceNotFoundException
					' OK, we will do one by one
				End Try

				Dim j As Integer = 0
				For i = 0 To len - 1
					Try
						Dim id As Integer? = addListenerWithSubject(names(i), New java.rmi.MarshalledObject(Of javax.management.NotificationFilter)(filters(i)), subjects(i), False)

						clis(j) = New com.sun.jmx.remote.internal.ClientListenerInfo(id, names(i), listeners(i), filters(i), handbacks(i), subjects(i))
						j += 1
					Catch infe As javax.management.InstanceNotFoundException
						logger.warning("reconnectNotificationListeners", "Can't reconnect listener for " & names(i))
					End Try
				Next i

				If j <> len Then
					Dim tmp As com.sun.jmx.remote.internal.ClientListenerInfo() = clis
					clis = New com.sun.jmx.remote.internal.ClientListenerInfo(j - 1){}
					Array.Copy(tmp, 0, clis, 0, j)
				End If

				rmiNotifClient.postReconnection(clis)

			protected void checkConnection() throws java.io.IOException
				If logger.debugOn() Then logger.debug("RMIClientCommunicatorAdmin-checkConnection", "Calling the method getDefaultDomain.")

				connection.getDefaultDomain(Nothing)

			protected void doStart() throws java.io.IOException
				' Get RMIServer stub from directory or URL encoding if needed.
				Dim stub As RMIServer
				Try
					stub = If(rmiServer IsNot Nothing, rmiServer, findRMIServer(jmxServiceURL, env))
				Catch ne As javax.naming.NamingException
					Throw New java.io.IOException("Failed to get a RMI stub: " & ne)
				End Try

				' Connect IIOP Stub if needed.
				stub = connectStub(stub,env)

				' Calling newClient on the RMIServer stub.
				Dim credentials As Object = env(CREDENTIALS)
				connection = stub.newClient(credentials)

				' notif issues
				Dim old As com.sun.jmx.remote.internal.ClientListenerInfo() = rmiNotifClient.preReconnection()

				reconnectNotificationListeners(old)

				connectionId = connectionId

				Dim reconnectedNotif As javax.management.Notification = New javax.management.remote.JMXConnectionNotification(javax.management.remote.JMXConnectionNotification.OPENED, Me, connectionId, clientNotifSeqNo, "Reconnected to server", Nothing)
				clientNotifSeqNo += 1
				sendNotification(reconnectedNotif)


			protected void doStop()
				Try
					close()
				Catch ioe As java.io.IOException
					logger.warning("RMIClientCommunicatorAdmin-doStop", "Failed to call the method close():" & ioe)
					logger.debug("RMIClientCommunicatorAdmin-doStop",ioe)
				End Try

		'--------------------------------------------------------------------
		' Private stuff - Serialization
		'--------------------------------------------------------------------
		''' <summary>
		''' <p>In order to be usable, an IIOP stub must be connected to an ORB.
		''' The stub is automatically connected to the ORB if:
		''' <ul>
		'''     <li> It was returned by the COS naming</li>
		'''     <li> Its server counterpart has been registered in COS naming
		'''          through JNDI.</li>
		''' </ul>
		''' Otherwise, it is not connected. A stub which is deserialized
		''' from Jini is not connected. A stub which is obtained from a
		''' non registered RMIIIOPServerImpl is not a connected.<br>
		''' A stub which is not connected can't be serialized, and thus
		''' can't be registered in Jini. A stub which is not connected can't
		''' be used to invoke methods on the server.
		''' <p>
		''' In order to palliate this, this method will connect the
		''' given stub if it is not yet connected. If the given
		''' <var>RMIServer</var> is not an instance of
		''' <seealso cref="javax.rmi.CORBA.Stub javax.rmi.CORBA.Stub"/>, then the
		''' method do nothing and simply returns that stub. Otherwise,
		''' this method will attempt to connect the stub to an ORB as
		''' follows:
		''' <ul>
		''' <li>This method looks in the provided <var>environment</var> for
		''' the "java.naming.corba.orb" property. If it is found, the
		''' referenced object (an <seealso cref="org.omg.CORBA.ORB ORB"/>) is used to
		''' connect the stub. Otherwise, a new org.omg.CORBA.ORB is created
		''' by calling {@link
		''' org.omg.CORBA.ORB#init(String[], Properties)
		''' org.omg.CORBA.ORB.init((String[])null,(Properties)null)}</li>
		''' <li>The new created ORB is kept in a static
		''' <seealso cref="WeakReference"/> and can be reused for connecting other
		''' stubs. However, no reference is ever kept on the ORB provided
		''' in the <var>environment</var> map, if any.</li>
		''' </ul> </summary>
		''' <param name="rmiServer"> A RMI Server Stub. </param>
		''' <param name="environment"> An environment map, possibly containing an ORB. </param>
		''' <returns> the given stub. </returns>
		''' <exception cref="IllegalArgumentException"> if the
		'''      <tt>java.naming.corba.orb</tt> property is specified and
		'''      does not point to an <seealso cref="org.omg.CORBA.ORB ORB"/>. </exception>
		''' <exception cref="IOException"> if the connection to the ORB failed.
		'''  </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		static RMIServer connectStub(RMIServer rmiServer, IDictionary(Of String, ?) environment) throws java.io.IOException
			If com.sun.jmx.remote.internal.IIOPHelper.isStub(rmiServer) Then
				Try
					com.sun.jmx.remote.internal.IIOPHelper.getOrb(rmiServer)
				Catch x As System.NotSupportedException
					' BAD_OPERATION
					com.sun.jmx.remote.internal.IIOPHelper.connect(rmiServer, resolveOrb(environment))
				End Try
			End If
			Return rmiServer

		''' <summary>
		''' Get the ORB specified by <var>environment</var>, or create a
		''' new one.
		''' <p>This method looks in the provided <var>environment</var> for
		''' the "java.naming.corba.orb" property. If it is found, the
		''' referenced object (an <seealso cref="org.omg.CORBA.ORB ORB"/>) is
		''' returned. Otherwise, a new org.omg.CORBA.ORB is created
		''' by calling {@link
		''' org.omg.CORBA.ORB#init(String[], java.util.Properties)
		''' org.omg.CORBA.ORB.init((String[])null,(Properties)null)}
		''' <p>The new created ORB is kept in a static
		''' <seealso cref="WeakReference"/> and can be reused for connecting other
		''' stubs. However, no reference is ever kept on the ORB provided
		''' in the <var>environment</var> map, if any. </summary>
		''' <param name="environment"> An environment map, possibly containing an ORB. </param>
		''' <returns> An ORB. </returns>
		''' <exception cref="IllegalArgumentException"> if the
		'''      <tt>java.naming.corba.orb</tt> property is specified and
		'''      does not point to an <seealso cref="org.omg.CORBA.ORB ORB"/>. </exception>
		''' <exception cref="IOException"> if the ORB initialization failed.
		'''  </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		static Object resolveOrb(IDictionary(Of String, ?) environment) throws java.io.IOException
			If environment IsNot Nothing Then
				Dim orb As Object = environment.get(com.sun.jmx.remote.util.EnvHelp.DEFAULT_ORB)
				If orb IsNot Nothing AndAlso Not(com.sun.jmx.remote.internal.IIOPHelper.isOrb(orb)) Then Throw New System.ArgumentException(com.sun.jmx.remote.util.EnvHelp.DEFAULT_ORB & " must be an instance of org.omg.CORBA.ORB.")
				If orb IsNot Nothing Then Return orb
			End If
			Dim orb As Object = If(RMIConnector.orb Is Nothing, Nothing, RMIConnector.orb.get())
			If orb IsNot Nothing Then Return orb

			Dim newOrb As Object = com.sun.jmx.remote.internal.IIOPHelper.createOrb(CType(Nothing, String()), CType(Nothing, java.util.Properties))
			RMIConnector.orb = New WeakReference(Of Object)(newOrb)
			Return newOrb

		''' <summary>
		''' Read RMIConnector fields from an {@link java.io.ObjectInputStream
		''' ObjectInputStream}.
		''' Calls <code>s.defaultReadObject()</code> and then initializes
		''' all transient variables that need initializing. </summary>
		''' <param name="s"> The ObjectInputStream to read from. </param>
		''' <exception cref="InvalidObjectException"> if none of <var>rmiServer</var> stub
		'''    or <var>jmxServiceURL</var> are set. </exception>
		''' <seealso cref= #RMIConnector(JMXServiceURL,Map) </seealso>
		''' <seealso cref= #RMIConnector(RMIServer,Map)
		'''  </seealso>
		private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
			s.defaultReadObject()

			If rmiServer Is Nothing AndAlso jmxServiceURL Is Nothing Then Throw New java.io.InvalidObjectException("rmiServer and jmxServiceURL both null")

			initTransients()

		''' <summary>
		''' Writes the RMIConnector fields to an {@link java.io.ObjectOutputStream
		''' ObjectOutputStream}.
		''' <p>Connects the underlying RMIServer stub to an ORB, if needed,
		''' before serializing it. This is done using the environment
		''' map that was provided to the constructor, if any, and as documented
		''' in <seealso cref="javax.management.remote.rmi"/>.</p>
		''' <p>This method then calls <code>s.defaultWriteObject()</code>.
		''' Usually, <var>rmiServer</var> is null if this object
		''' was constructed with a JMXServiceURL, and <var>jmxServiceURL</var>
		''' is null if this object is constructed with a RMIServer stub.
		''' <p>Note that the environment Map is not serialized, since the objects
		''' it contains are assumed to be contextual and relevant only
		''' with respect to the local environment (class loader, ORB, etc...).</p>
		''' <p>After an RMIConnector is deserialized, it is assumed that the
		''' user will call <seealso cref="#connect(Map)"/>, providing a new Map that
		''' can contain values which are contextually relevant to the new
		''' local environment.</p>
		''' <p>Since connection to the ORB is needed prior to serializing, and
		''' since the ORB to connect to is one of those contextual parameters,
		''' it is not recommended to re-serialize a just de-serialized object -
		''' as the de-serialized object has no map. Thus, when an RMIConnector
		''' object is needed for serialization or transmission to a remote
		''' application, it is recommended to obtain a new RMIConnector stub
		''' by calling <seealso cref="RMIConnectorServer#toJMXConnector(Map)"/>.</p> </summary>
		''' <param name="s"> The ObjectOutputStream to write to. </param>
		''' <exception cref="InvalidObjectException"> if none of <var>rmiServer</var> stub
		'''    or <var>jmxServiceURL</var> are set. </exception>
		''' <seealso cref= #RMIConnector(JMXServiceURL,Map) </seealso>
		''' <seealso cref= #RMIConnector(RMIServer,Map)
		'''  </seealso>
		private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			If rmiServer Is Nothing AndAlso jmxServiceURL Is Nothing Then Throw New java.io.InvalidObjectException("rmiServer and jmxServiceURL both null.")
			connectStub(Me.rmiServer,env)
			s.defaultWriteObject()

		' Initialization of transient variables.
		private void initTransients()
			rmbscMap = New java.util.WeakHashMap(Of javax.security.auth.Subject, WeakReference(Of javax.management.MBeanServerConnection))
			connected = False
			terminated = False

			connectionBroadcaster = New javax.management.NotificationBroadcasterSupport

		'--------------------------------------------------------------------
		' Private stuff - Check if stub can be trusted.
		'--------------------------------------------------------------------

		private static void checkStub(java.rmi.Remote stub, Type stubClass)

			' Check remote stub is from the expected class.
			'
			If stub.GetType() IsNot stubClass Then
				If Not Proxy.isProxyClass(stub.GetType()) Then
					Throw New SecurityException("Expecting a " & stubClass.name & " stub!")
				Else
					Dim handler As InvocationHandler = Proxy.getInvocationHandler(stub)
					If handler.GetType() IsNot GetType(java.rmi.server.RemoteObjectInvocationHandler) Then
						Throw New SecurityException("Expecting a dynamic proxy instance with a " & GetType(java.rmi.server.RemoteObjectInvocationHandler).name & " invocation handler!")
					Else
						stub = CType(handler, java.rmi.Remote)
					End If
				End If
			End If

			' Check RemoteRef in stub is from the expected class
			' "sun.rmi.server.UnicastRef2".
			'
			Dim ref As java.rmi.server.RemoteRef = CType(stub, java.rmi.server.RemoteObject).ref
			If ref.GetType() IsNot GetType(sun.rmi.server.UnicastRef2) Then Throw New SecurityException("Expecting a " & GetType(sun.rmi.server.UnicastRef2).name & " remote reference in stub!")

			' Check RMIClientSocketFactory in stub is from the expected class
			' "javax.rmi.ssl.SslRMIClientSocketFactory".
			'
			Dim liveRef As sun.rmi.transport.LiveRef = CType(ref, sun.rmi.server.UnicastRef2).liveRef
			Dim csf As java.rmi.server.RMIClientSocketFactory = liveRef.clientSocketFactory
			If csf Is Nothing OrElse csf.GetType() IsNot GetType(javax.rmi.ssl.SslRMIClientSocketFactory) Then Throw New SecurityException("Expecting a " & GetType(javax.rmi.ssl.SslRMIClientSocketFactory).name & " RMI client socket factory in stub!")

		'--------------------------------------------------------------------
		' Private stuff - RMIServer creation
		'--------------------------------------------------------------------

		private RMIServer findRMIServer(javax.management.remote.JMXServiceURL directoryURL, IDictionary(Of String, Object) environment) throws javax.naming.NamingException, java.io.IOException
			Dim isIiop As Boolean = RMIConnectorServer.isIiopURL(directoryURL,True)
			If isIiop Then environment.put(com.sun.jmx.remote.util.EnvHelp.DEFAULT_ORB,resolveOrb(environment))

			Dim path As String = directoryURL.uRLPath
			Dim [end] As Integer = path.IndexOf(";"c)
			If [end] < 0 Then [end] = path.Length
			If path.StartsWith("/jndi/") Then
				Return findRMIServerJNDI(path.Substring(6, [end] - 6), environment, isIiop)
			ElseIf path.StartsWith("/stub/") Then
				Return findRMIServerJRMP(path.Substring(6, [end] - 6), environment, isIiop)
			ElseIf path.StartsWith("/ior/") Then
				If Not com.sun.jmx.remote.internal.IIOPHelper.available Then Throw New java.io.IOException("iiop protocol not available")
				Return findRMIServerIIOP(path.Substring(5, [end] - 5), environment, isIiop)
			Else
				Dim msg As String = "URL path must begin with /jndi/ or /stub/ " & "or /ior/: " & path
				Throw New java.net.MalformedURLException(msg)
			End If

		''' <summary>
		''' Lookup the RMIServer stub in a directory. </summary>
		''' <param name="jndiURL"> A JNDI URL indicating the location of the Stub
		'''                (see <seealso cref="javax.management.remote.rmi"/>), e.g.:
		'''   <ul><li><tt>rmi://registry-host:port/rmi-stub-name</tt></li>
		'''       <li>or <tt>iiop://cosnaming-host:port/iiop-stub-name</tt></li>
		'''       <li>or <tt>ldap://ldap-host:port/java-container-dn</tt></li>
		'''   </ul> </param>
		''' <param name="env"> the environment Map passed to the connector. </param>
		''' <param name="isIiop"> true if the stub is expected to be an IIOP stub. </param>
		''' <returns> The retrieved RMIServer stub. </returns>
		''' <exception cref="NamingException"> if the stub couldn't be found.
		'''  </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private RMIServer findRMIServerJNDI(String jndiURL, IDictionary(Of String, ?) env, Boolean isIiop) throws javax.naming.NamingException

			Dim ctx As New javax.naming.InitialContext(com.sun.jmx.remote.util.EnvHelp.mapToHashtable(env))

			Dim objref As Object = ctx.lookup(jndiURL)
			ctx.close()

			If isIiop Then
				Return narrowIIOPServer(objref)
			Else
				Return narrowJRMPServer(objref)
			End If

		private static RMIServer narrowJRMPServer(Object objref)

			Return CType(objref, RMIServer)

		private static RMIServer narrowIIOPServer(Object objref)
			Try
				Return com.sun.jmx.remote.internal.IIOPHelper.narrow(objref, GetType(RMIServer))
			Catch e As ClassCastException
				If logger.traceOn() Then logger.trace("narrowIIOPServer","Failed to narrow objref=" & objref & ": " & e)
				If logger.debugOn() Then logger.debug("narrowIIOPServer",e)
				Return Nothing
			End Try

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private RMIServer findRMIServerIIOP(String ior, IDictionary(Of String, ?) env, Boolean isIiop)
			' could forbid "rmi:" URL here -- but do we need to?
			Dim orb As Object = env(com.sun.jmx.remote.util.EnvHelp.DEFAULT_ORB)
			Dim stub As Object = com.sun.jmx.remote.internal.IIOPHelper.stringToObject(orb, ior)
			Return com.sun.jmx.remote.internal.IIOPHelper.narrow(stub, GetType(RMIServer))

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private RMIServer findRMIServerJRMP(String base64, IDictionary(Of String, ?) env, Boolean isIiop) throws java.io.IOException
			' could forbid "iiop:" URL here -- but do we need to?
			Dim serialized As SByte()
			Try
				serialized = base64ToByteArray(base64)
			Catch e As System.ArgumentException
				Throw New java.net.MalformedURLException("Bad BASE64 encoding: " & e.Message)
			End Try
			Dim bin As New java.io.ByteArrayInputStream(serialized)

			Dim loader As ClassLoader = com.sun.jmx.remote.util.EnvHelp.resolveClientClassLoader(env)
			Dim oin As java.io.ObjectInputStream = If(loader Is Nothing, New java.io.ObjectInputStream(bin), New ObjectInputStreamWithLoader(bin, loader))
			Dim stub As Object
			Try
				stub = oin.readObject()
			Catch e As ClassNotFoundException
				Throw New java.net.MalformedURLException("Class not found: " & e)
			End Try
			Return CType(stub, RMIServer)

		private static final class ObjectInputStreamWithLoader extends java.io.ObjectInputStream
			ObjectInputStreamWithLoader(java.io.InputStream in, ClassLoader cl) throws java.io.IOException
				MyBase(in)
				Me.loader = cl

			protected Type resolveClass(java.io.ObjectStreamClass classDesc) throws java.io.IOException, ClassNotFoundException
				Dim name As String = classDesc.name
				sun.reflect.misc.ReflectUtil.checkPackageAccess(name)
				Return Type.GetType(name, False, loader)

			private final ClassLoader loader

		private javax.management.MBeanServerConnection getConnectionWithSubject(javax.security.auth.Subject delegationSubject)
			Dim conn As javax.management.MBeanServerConnection = Nothing

			If delegationSubject Is Nothing Then
				conn = nullSubjectConnRef.get()
				If nullSubjectConnRef Is Nothing OrElse conn Is Nothing Then
					conn = New RemoteMBeanServerConnection(Me, Nothing)
					nullSubjectConnRef = New WeakReference(conn)
				End If
			Else
				Dim wr As WeakReference(Of javax.management.MBeanServerConnection) = rmbscMap.get(delegationSubject)
				conn = wr.get()
				If wr Is Nothing OrElse conn Is Nothing Then
					conn = New RemoteMBeanServerConnection(Me, delegationSubject)
					rmbscMap.put(delegationSubject, New WeakReference(conn))
				End If
			End If
			Return conn

	'    
	'       The following section of code avoids a class loading problem
	'       with RMI.  The problem is that an RMI stub, when deserializing
	'       a remote method return value or exception, will first of all
	'       consult the first non-bootstrap class loader it finds in the
	'       call stack.  This can lead to behavior that is not portable
	'       between implementations of the JMX Remote API.  Notably, an
	'       implementation on J2SE 1.4 will find the RMI stub's loader on
	'       the stack.  But in J2SE 5, this stub is loaded by the
	'       bootstrap loader, so RMI will find the loader of the user code
	'       that called an MBeanServerConnection method.
	'
	'       To avoid this problem, we take advantage of what the RMI stub
	'       is doing internally.  Each remote call will end up calling
	'       ref.invoke(...), where ref is the RemoteRef parameter given to
	'       the RMI stub's constructor.  It is within this call that the
	'       deserialization will happen.  So we fabricate our own RemoteRef
	'       that delegates everything to the "real" one but that is loaded
	'       by a class loader that knows no other classes.  The class
	'       loader NoCallStackClassLoader does this: the RemoteRef is an
	'       instance of the class named by proxyRefClassName, which is
	'       fabricated by the class loader using byte code that is defined
	'       by the string below.
	'
	'       The call stack when the deserialization happens is thus this:
	'       MBeanServerConnection.getAttribute (or whatever)
	'       -> RMIConnectionImpl_Stub.getAttribute
	'          -> ProxyRef.invoke(...getAttribute...)
	'             -> UnicastRef.invoke(...getAttribute...)
	'                -> internal RMI stuff
	'
	'       Here UnicastRef is the RemoteRef created when the stub was
	'       deserialized (which is of some RMI internal class).  It and the
	'       "internal RMI stuff" are loaded by the bootstrap loader, so are
	'       transparent to the stack search.  The first non-bootstrap
	'       loader found is our ProxyRefLoader, as required.
	'
	'       In a future version of this code as integrated into J2SE 5,
	'       this workaround could be replaced by direct access to the
	'       internals of RMI.  For now, we use the same code base for J2SE
	'       and for the standalone Reference Implementation.
	'
	'       The byte code below encodes the following class, compiled using
	'       J2SE 1.4.2 with the -g:none option.
	'
	'        package com.sun.jmx.remote.internal;
	'
	'        import java.lang.reflect.Method;
	'        import java.rmi.Remote;
	'        import java.rmi.server.RemoteRef;
	'        import com.sun.jmx.remote.internal.ProxyRef;
	'
	'        public class PRef extends ProxyRef {
	'            public PRef(RemoteRef ref) {
	'                super(ref);
	'            }
	'
	'            public Object invoke(Remote obj, Method method,
	'                                 Object[] params, long opnum)
	'                    throws Exception {
	'                return ref.invoke(obj, method, params, opnum);
	'            }
	'        }
	'     

		private static final String rmiServerImplStubClassName = GetType(RMIServer).name & "Impl_Stub"
		private static final Type rmiServerImplStubClass
		private static final String rmiConnectionImplStubClassName = GetType(RMIConnection).name & "Impl_Stub"
		private static final Type rmiConnectionImplStubClass
		private static final String pRefClassName = "com.sun.jmx.remote.internal.PRef"
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static final Constructor(Of ?) proxyRefConstructor
		static RMIConnector()
			Dim pRefByteCodeString As String = "\312\376\272\276" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "." & ChrW(&H).ToString() & "\27\12" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\15\11" & ChrW(&H).ToString() & "\4" & ChrW(&H).ToString() & "\16\13" & ChrW(&H).ToString() & "\17" & ChrW(&H).ToString() & "\20\7" & ChrW(&H).ToString() & "\21\7" & ChrW(&H).ToString() & "\22\1" & ChrW(&H).ToString() & "\6<init>\1" & ChrW(&H).ToString() & "\36(Ljava/rmi/server/RemoteRef;" & ")V\1" & ChrW(&H).ToString() & "\4Code\1" & ChrW(&H).ToString() & "\6invoke\1" & ChrW(&H).ToString() & "S(Ljava/rmi/Remote;Ljava/lang/reflec" & "t/Method;[Ljava/lang/Object;J)Ljava/lang/Object;\1" & ChrW(&H).ToString() & "\12Exception" & "s\7" & ChrW(&H).ToString() & "\23\14" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\7\14" & ChrW(&H).ToString() & "\24" & ChrW(&H).ToString() & "\25\7" & ChrW(&H).ToString() & "\26\14" & ChrW(&H).ToString() & "\11" & ChrW(&H).ToString() & "\12\1" & ChrW(&H).ToString() & "\40com/" & "sun/jmx/remote/internal/PRef\1" & ChrW(&H).ToString() & "$com/sun/jmx/remote/internal/Pr" & "oxyRef\1" & ChrW(&H).ToString() & "\23java/lang/Exception\1" & ChrW(&H).ToString() & "\3ref\1" & ChrW(&H).ToString() & "\33Ljava/rmi/serve" & "r/RemoteRef;\1" & ChrW(&H).ToString() & "\31java/rmi/server/RemoteRef" & ChrW(&H).ToString() & "!" & ChrW(&H).ToString() & "\4" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\7" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\10" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\22" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\6*+\267" & ChrW(&H).ToString() & "\1\261" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\11" & ChrW(&H).ToString() & "\12" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\10" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\33" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\17*\264" & ChrW(&H).ToString() & "\2+,-\26\4\271" & ChrW(&H).ToString() & "\3\6" & ChrW(&H).ToString() & "\260" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\13" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\4" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & ChrW(&H).ToString()
			Dim pRefByteCode As SByte() = NoCallStackClassLoader.stringToBytes(pRefByteCodeString)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.security.PrivilegedExceptionAction<Constructor<?>> action = New java.security.PrivilegedExceptionAction<Constructor<?>>()
	'		{
	'			public Constructor<?> run() throws Exception
	'			{
	'				Class thisClass = RMIConnector.class;
	'				ClassLoader thisLoader = thisClass.getClassLoader();
	'				ProtectionDomain thisProtectionDomain = thisClass.getProtectionDomain();
	'				String[] otherClassNames = {ProxyRef.class.getName()};
	'				ClassLoader cl = New NoCallStackClassLoader(pRefClassName, pRefByteCode, otherClassNames, thisLoader, thisProtectionDomain);
	'				Class c = cl.loadClass(pRefClassName);
	'				Return c.getConstructor(RemoteRef.class);
	'			}
	'		};

			Dim serverStubClass As Type
			Try
				serverStubClass = Type.GetType(rmiServerImplStubClassName)
			Catch e As Exception
				logger.error("<clinit>", "Failed to instantiate " & rmiServerImplStubClassName & ": " & e)
				logger.debug("<clinit>",e)
				serverStubClass = Nothing
			End Try
			rmiServerImplStubClass = serverStubClass

			Dim stubClass As Type
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim constr As Constructor(Of ?)
			Try
				stubClass = Type.GetType(rmiConnectionImplStubClassName)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				constr = CType(java.security.AccessController.doPrivileged(action), Constructor(Of ?))
			Catch e As Exception
				logger.error("<clinit>", "Failed to initialize proxy reference constructor " & "for " & rmiConnectionImplStubClassName & ": " & e)
				logger.debug("<clinit>",e)
				stubClass = Nothing
				constr = Nothing
			End Try
			rmiConnectionImplStubClass = stubClass
			proxyRefConstructor = constr
			Dim proxyStubByteCodeString As String = "\312\376\272\276" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\63" & ChrW(&H).ToString() & "+\12" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & "\30\7" & ChrW(&H).ToString() & "\31\12" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & "\32\12" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\33\7" & ChrW(&H).ToString() & "\34\12" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\35\12" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\36\12" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\37\12" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & " " & "\12" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & "!\7" & ChrW(&H).ToString() & """\7" & ChrW(&H).ToString() & "#\1" & ChrW(&H).ToString() & "\6<init>\1" & ChrW(&H).ToString() & "\3()V\1" & ChrW(&H).ToString() & "\4Code\1" & ChrW(&H).ToString() & "\7_in" & "voke\1" & ChrW(&H).ToString() & "K(Lorg/omg/CORBA/portable/OutputStream;)Lorg/omg/CORBA" & "/portable/InputStream;\1" & ChrW(&H).ToString() & "\15StackMapTable\7" & ChrW(&H).ToString() & "\34\1" & ChrW(&H).ToString() & "\12Except" & "ions\7" & ChrW(&H).ToString() & "$\1" & ChrW(&H).ToString() & "\15_releaseReply\1" & ChrW(&H).ToString() & "'(Lorg/omg/CORBA/portable/Inp" & "utStream;)V\14" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\16\1" & ChrW(&H).ToString() & "-com/sun/jmx/remote/protocol/iiop/" & "PInputStream\14" & ChrW(&H).ToString() & "\20" & ChrW(&H).ToString() & "\21\14" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\27\1" & ChrW(&H).ToString() & "+org/omg/CORBA/porta" & "ble/ApplicationException\14" & ChrW(&H).ToString() & "%" & ChrW(&H).ToString() & "&\14" & ChrW(&H).ToString() & "'" & ChrW(&H).ToString() & "(\14" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & ")\14" & ChrW(&H).ToString() & "*" & ChrW(&H).ToString() & "&" & "\14" & ChrW(&H).ToString() & "\26" & ChrW(&H).ToString() & "\27\1" & ChrW(&H).ToString() & "*com/sun/jmx/remote/protocol/iiop/ProxyStub\1" & ChrW(&H).ToString() & "<org/omg/stub/javax/management/remote/rmi/_RMIConnection_Stu" & "b\1" & ChrW(&H).ToString() & ")org/omg/CORBA/portable/RemarshalException\1" & ChrW(&H).ToString() & "\16getInput" & "Stream\1" & ChrW(&H).ToString() & "&()Lorg/omg/CORBA/portable/InputStream;\1" & ChrW(&H).ToString() & "\5getId\1" & ChrW(&H).ToString() & "\24()Ljava/lang/String;\1" & ChrW(&H9).ToString() & "(Ljava/lang/String;Lorg/omg/CORB" & "A/portable/InputStream;)V\1" & ChrW(&H).ToString() & "\25getProxiedInputStream" & ChrW(&H).ToString() & "!" & ChrW(&H).ToString() & "\13" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\3" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\16" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\17" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\21" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\5" & "*\267" & ChrW(&H).ToString() & "\1\261" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\20" & ChrW(&H).ToString() & "\21" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\17" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "G" & ChrW(&H).ToString() & "\4" & ChrW(&H).ToString() & "\4" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "'\273" & ChrW(&H).ToString() & "\2Y*+\267" & ChrW(&H).ToString() & "\3\267" & ChrW(&H).ToString() & "\4\260M\273" & ChrW(&H).ToString() & "\2Y,\266" & ChrW(&H).ToString() & "\6\267" & ChrW(&H).ToString() & "\4N" & "\273" & ChrW(&H).ToString() & "\5Y,\266" & ChrW(&H).ToString() & "\7-\267" & ChrW(&H).ToString() & "\10\277" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\22" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\1M\7" & ChrW(&H).ToString() & "\23" & ChrW(&H).ToString() & "\24" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\5" & ChrW(&H).ToString() & "\25" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\26" & ChrW(&H).ToString() & "\27" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\17" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "'" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\22+\306" & ChrW(&H).ToString() & "\13+\300" & ChrW(&H).ToString() & "\2\266" & ChrW(&H).ToString() & "\11L*+" & "\267" & ChrW(&H).ToString() & "\12\261" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\22" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\3" & ChrW(&H).ToString() & "\1\14" & ChrW(&H).ToString() & ChrW(&H).ToString()
			Dim pInputStreamByteCodeString As String = "\312\376\272\276" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\63" & ChrW(&H).ToString() & "\36\12" & ChrW(&H).ToString() & "\7" & ChrW(&H).ToString() & "\17\11" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\20\12" & ChrW(&H).ToString() & "\21" & ChrW(&H).ToString() & "\22\12" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\23\12" & ChrW(&H).ToString() & "\24" & ChrW(&H).ToString() & "\25\7" & ChrW(&H).ToString() & "\26\7" & ChrW(&H).ToString() & "\27\1" & ChrW(&H).ToString() & "\6<init>\1" & ChrW(&H).ToString() & "'(" & "Lorg/omg/CORBA/portable/InputStream;)V\1" & ChrW(&H).ToString() & "\4Code\1" & ChrW(&H).ToString() & "\10read_an" & "y\1" & ChrW(&H).ToString() & "\25()Lorg/omg/CORBA/Any;\1" & ChrW(&H).ToString() & "\12read_value\1" & ChrW(&H).ToString() & ")(Ljava/lang" & "/Class;)Ljava/io/Serializable;\14" & ChrW(&H).ToString() & "\10" & ChrW(&H).ToString() & "\11\14" & ChrW(&H).ToString() & "\30" & ChrW(&H).ToString() & "\31\7" & ChrW(&H).ToString() & "\32" & "\14" & ChrW(&H).ToString() & "\13" & ChrW(&H).ToString() & "\14\14" & ChrW(&H).ToString() & "\33" & ChrW(&H).ToString() & "\34\7" & ChrW(&H).ToString() & "\35\14" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\16\1" & ChrW(&H).ToString() & "-com/sun/jmx" & "/remote/protocol/iiop/PInputStream\1" & ChrW(&H).ToString() & "\61com/sun/jmx/remote/pr" & "otocol/iiop/ProxyInputStream\1" & ChrW(&H).ToString() & "\2in\1" & ChrW(&H).ToString() & "$Lorg/omg/CORBA/portab" & "le/InputStream;\1" & ChrW(&H).ToString() & """org/omg/CORBA/portable/InputStream\1" & ChrW(&H).ToString() & "\6n" & "arrow\1" & ChrW(&H).ToString() & "*()Lorg/omg/CORBA_2_3/portable/InputStream;\1" & ChrW(&H).ToString() & "&org/o" & "mg/CORBA_2_3/portable/InputStream" & ChrW(&H).ToString() & "!" & ChrW(&H).ToString() & "\6" & ChrW(&H).ToString() & "\7" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\3" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\10" & ChrW(&H).ToString() & "\11" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\12" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\22" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\6*+\267" & ChrW(&H).ToString() & "\1\261" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\13" & ChrW(&H).ToString() & "\14" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\12" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\24" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\10*\264" & ChrW(&H).ToString() & "\2\266" & ChrW(&H).ToString() & "\3\260" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\15" & ChrW(&H).ToString() & "\16" & ChrW(&H).ToString() & "\1" & ChrW(&H).ToString() & "\12" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\25" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & "\2" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & "\11*\266" & ChrW(&H).ToString() & "\4+\266" & ChrW(&H).ToString() & "\5\260" & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString() & ChrW(&H).ToString()
			Dim proxyStubByteCode As SByte() = NoCallStackClassLoader.stringToBytes(proxyStubByteCodeString)
			Dim pInputStreamByteCode As SByte() = NoCallStackClassLoader.stringToBytes(pInputStreamByteCodeString)
			Dim classNames As String()={proxyStubClassName, pInputStreamClassName}
			Dim byteCodes As SByte()() = { proxyStubByteCode, pInputStreamByteCode }
			Dim otherClassNames As String() = { iiopConnectionStubClassName, ProxyInputStreamClassName }
			If com.sun.jmx.remote.internal.IIOPHelper.available Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.PrivilegedExceptionAction<Class> action = New java.security.PrivilegedExceptionAction<Class>()
	'			{
	'			  public Class run() throws Exception
	'			  {
	'				Class thisClass = RMIConnector.class;
	'				ClassLoader thisLoader = thisClass.getClassLoader();
	'				ProtectionDomain thisProtectionDomain = thisClass.getProtectionDomain();
	'				ClassLoader cl = New NoCallStackClassLoader(classNames, byteCodes, otherClassNames, thisLoader, thisProtectionDomain);
	'				Return cl.loadClass(proxyStubClassName);
	'			  }
	'			};
				Dim stubClass As Type
				Try
					stubClass = java.security.AccessController.doPrivileged(action)
				Catch e As Exception
					logger.error("<clinit>", "Unexpected exception making shadow IIOP stub class: " & e)
					logger.debug("<clinit>",e)
					stubClass = Nothing
				End Try
				proxyStubClass = stubClass
			Else
				proxyStubClass = Nothing
			End If

		private static RMIConnection shadowJrmpStub(java.rmi.server.RemoteObject stub) throws InstantiationException, IllegalAccessException, InvocationTargetException, ClassNotFoundException, NoSuchMethodException
			Dim ref As java.rmi.server.RemoteRef = stub.ref
			Dim proxyRef As java.rmi.server.RemoteRef = CType(proxyRefConstructor.newInstance(New Object() {ref}), java.rmi.server.RemoteRef)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim rmiConnectionImplStubConstructor As Constructor(Of ?) = rmiConnectionImplStubClass.GetConstructor(GetType(java.rmi.server.RemoteRef))
			Dim args As Object() = {proxyRef}
			Dim proxyStub As RMIConnection = CType(rmiConnectionImplStubConstructor.newInstance(args), RMIConnection)
			Return proxyStub

	'    
	'       The following code performs a similar trick for RMI/IIOP to the
	'       one described above for RMI/JRMP.  Unlike JRMP, though, we
	'       can't easily insert an object between the RMIConnection stub
	'       and the RMI/IIOP deserialization code, as explained below.
	'
	'       A method in an RMI/IIOP stub does the following.  It makes an
	'       org.omg.CORBA_2_3.portable.OutputStream for each request, and
	'       writes the parameters to it.  Then it calls
	'       _invoke(OutputStream) which it inherits from CORBA's
	'       ObjectImpl.  That returns an
	'       org.omg.CORBA_2_3.portable.InputStream.  The return value is
	'       read from this InputStream.  So the stack during
	'       deserialization looks like this:
	'
	'       MBeanServerConnection.getAttribute (or whatever)
	'       -> _RMIConnection_Stub.getAttribute
	'          -> Util.readAny (a CORBA method)
	'             -> InputStream.read_any
	'                -> internal CORBA stuff
	'
	'       What we would have *liked* to have done would be the same thing
	'       as for RMI/JRMP.  We create a "ProxyDelegate" that is an
	'       org.omg.CORBA.portable.Delegate that simply forwards every
	'       operation to the real original Delegate from the RMIConnection
	'       stub, except that the InputStream returned by _invoke is
	'       wrapped by a "ProxyInputStream" that is loaded by our
	'       NoCallStackClassLoader.
	'
	'       Unfortunately, this doesn't work, at least with Sun's J2SE
	'       1.4.2, because the CORBA code is not designed to allow you to
	'       change Delegates arbitrarily.  You get a ClassCastException
	'       from code that expects the Delegate to implement an internal
	'       interface.
	'
	'       So instead we do the following.  We create a subclass of the
	'       stub that overrides the _invoke method so as to wrap the
	'       returned InputStream in a ProxyInputStream.  We create a
	'       subclass of ProxyInputStream using the NoCallStackClassLoader
	'       and override its read_any and read_value(Class) methods.
	'       (These are the only methods called during deserialization of
	'       MBeanServerConnection return values.)  We extract the Delegate
	'       from the original stub and insert it into our subclass stub,
	'       and away we go.  The state of a stub consists solely of its
	'       Delegate.
	'
	'       We also need to catch ApplicationException, which will encode
	'       any exceptions declared in the throws clause of the called
	'       method.  Its InputStream needs to be wrapped in a
	'       ProxyInputSteam too.
	'
	'       We override _releaseReply in the stub subclass so that it
	'       replaces a ProxyInputStream argument with the original
	'       InputStream.  This avoids problems if the implementation of
	'       _releaseReply ends up casting this InputStream to an
	'       implementation-specific interface (which in Sun's J2SE 5 it
	'       does).
	'
	'       It is not strictly necessary for the stub subclass to be loaded
	'       by a NoCallStackClassLoader, since the call-stack search stops
	'       at the ProxyInputStream subclass.  However, it is convenient
	'       for two reasons.  One is that it means that the
	'       ProxyInputStream subclass can be accessed directly, without
	'       using reflection.  The other is that it avoids build problems,
	'       since usually stubs are created after other classes are
	'       compiled, so we can't access them from this class without,
	'       again, using reflection.
	'
	'       The strings below encode the following two Java classes,
	'       compiled using javac -g:none.
	'
	'        package com.sun.jmx.remote.protocol.iiop;
	'
	'        import org.omg.stub.javax.management.remote.rmi._RMIConnection_Stub;
	'
	'        import org.omg.CORBA.portable.ApplicationException;
	'        import org.omg.CORBA.portable.InputStream;
	'        import org.omg.CORBA.portable.OutputStream;
	'        import org.omg.CORBA.portable.RemarshalException;
	'
	'        public class ProxyStub extends _RMIConnection_Stub {
	'            public InputStream _invoke(OutputStream out)
	'                    throws ApplicationException, RemarshalException {
	'                try {
	'                    return new PInputStream(super._invoke(out));
	'                } catch (ApplicationException e) {
	'                    InputStream pis = new PInputStream(e.getInputStream());
	'                    throw new ApplicationException(e.getId(), pis);
	'                }
	'            }
	'
	'            public void _releaseReply(InputStream in) {
	'                if (in != null)
	'                    in = ((PInputStream)in).getProxiedInputStream();
	'                super._releaseReply(in);
	'            }
	'        }
	'
	'        package com.sun.jmx.remote.protocol.iiop;
	'
	'        public class PInputStream extends ProxyInputStream {
	'            public PInputStream(org.omg.CORBA.portable.InputStream in) {
	'                super(in);
	'            }
	'
	'            public org.omg.CORBA.Any read_any() {
	'                return in.read_any();
	'            }
	'
	'            public java.io.Serializable read_value(Class clz) {
	'                return narrow().read_value(clz);
	'            }
	'        }
	'
	'
	'     
		private static final String iiopConnectionStubClassName = "org.omg.stub.javax.management.remote.rmi._RMIConnection_Stub"
		private static final String proxyStubClassName = "com.sun.jmx.remote.protocol.iiop.ProxyStub"
		private static final String ProxyInputStreamClassName = "com.sun.jmx.remote.protocol.iiop.ProxyInputStream"
		private static final String pInputStreamClassName = "com.sun.jmx.remote.protocol.iiop.PInputStream"
		private static final Type proxyStubClass

	  private static RMIConnection shadowIiopStub(Object stub) throws InstantiationException, IllegalAccessException
			Dim proxyStub As Object = Nothing
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				proxyStub = java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Object>()
	'			{
	'				public Object run() throws Exception
	'				{
	'					Return proxyStubClass.newInstance();
	'				}
	'			});
			Catch e As java.security.PrivilegedActionException
				Throw New InternalError
			End Try
			com.sun.jmx.remote.internal.IIOPHelper.delegateate(proxyStub, com.sun.jmx.remote.internal.IIOPHelper.getDelegate(stub))
			Return CType(proxyStub, RMIConnection)
		private static RMIConnection getConnection(RMIServer server, Object credentials, Boolean checkStub) throws java.io.IOException
			Dim c As RMIConnection = server.newClient(credentials)
			If checkStub Then checkStub(c, rmiConnectionImplStubClass)
			Try
				If c.GetType() Is rmiConnectionImplStubClass Then Return shadowJrmpStub(CType(c, java.rmi.server.RemoteObject))
				If c.GetType().name.Equals(iiopConnectionStubClassName) Then Return shadowIiopStub(c)
				logger.trace("getConnection", "Did not wrap " & c.GetType() & " to foil " & "stack search for classes: class loading semantics " & "may be incorrect")
			Catch e As Exception
				logger.error("getConnection", "Could not wrap " & c.GetType() & " to foil " & "stack search for classes: class loading semantics " & "may be incorrect: " & e)
				logger.debug("getConnection",e)
				' so just return the original stub, which will work for all
				' but the most exotic class loading situations
			End Try
			Return c

		private static SByte() base64ToByteArray(String s)
			Dim sLen As Integer = s.length()
			Dim numGroups As Integer = sLen\4
			If 4*numGroups <> sLen Then Throw New System.ArgumentException("String length must be a multiple of four.")
			Dim missingBytesInLastGroup As Integer = 0
			Dim numFullGroups As Integer = numGroups
			If sLen <> 0 Then
				If s.Chars(sLen-1) = "="c Then
					missingBytesInLastGroup += 1
					numFullGroups -= 1
				End If
				If s.Chars(sLen-2) = "="c Then missingBytesInLastGroup += 1
			End If
			Dim result As SByte() = New SByte(3*numGroups - missingBytesInLastGroup - 1){}

			' Translate all full groups from base64 to byte array elements
			Dim inCursor As Integer = 0, outCursor As Integer = 0
			For i As Integer = 0 To numFullGroups - 1
				Dim ch0 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				Dim ch1 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				Dim ch2 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				Dim ch3 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				result(outCursor) = CByte((ch0 << 2) Or (ch1 >> 4))
				outCursor += 1
				result(outCursor) = CByte((ch1 << 4) Or (ch2 >> 2))
				outCursor += 1
				result(outCursor) = CByte((ch2 << 6) Or ch3)
				outCursor += 1
			Next i

			' Translate partial group, if present
			If missingBytesInLastGroup <> 0 Then
				Dim ch0 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				Dim ch1 As Integer = base64toInt(s.Chars(inCursor))
				inCursor += 1
				result(outCursor) = CByte((ch0 << 2) Or (ch1 >> 4))
				outCursor += 1

				If missingBytesInLastGroup = 1 Then
					Dim ch2 As Integer = base64toInt(s.Chars(inCursor))
					inCursor += 1
					result(outCursor) = CByte((ch1 << 4) Or (ch2 >> 2))
					outCursor += 1
				End If
			End If
			' assert inCursor == s.length()-missingBytesInLastGroup;
			' assert outCursor == result.length;
			Return result

		''' <summary>
		''' Translates the specified character, which is assumed to be in the
		''' "Base 64 Alphabet" into its equivalent 6-bit positive integer.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if
		'''        c is not in the Base64 Alphabet. </exception>
		private static Integer base64toInt(Char c)
			Dim result As Integer

			If c >= ___base64ToInt.Length Then
				result = -1
			Else
				result = ___base64ToInt(c)
			End If

			If result < 0 Then Throw New System.ArgumentException("Illegal character " & c)
			Return result

		''' <summary>
		''' This array is a lookup table that translates unicode characters
		''' drawn from the "Base64 Alphabet" (as specified in Table 1 of RFC 2045)
		''' into their 6-bit positive integer equivalents.  Characters that
		''' are not in the Base64 alphabet but fall within the bounds of the
		''' array are translated to -1.
		''' </summary>
		private static final SByte ___base64ToInt() = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51 }

		'--------------------------------------------------------------------
		' Private stuff - Find / Set default class loader
		'--------------------------------------------------------------------
		private ClassLoader pushDefaultClassLoader()
			Dim t As Thread = Thread.CurrentThread
			Dim old As ClassLoader = t.contextClassLoader
			If defaultClassLoader IsNot Nothing Then Return old

		private void popDefaultClassLoader(final ClassLoader old)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'		{
	'			public Void run()
	'			{
	'				Thread.currentThread().setContextClassLoader(old);
	'				Return Nothing;
	'			}
	'		});

		'--------------------------------------------------------------------
		' Private variables
		'--------------------------------------------------------------------
		''' <summary>
		''' @serial The RMIServer stub of the RMI JMX Connector server to
		''' which this client connector is (or will be) connected. This
		''' field can be null when <var>jmxServiceURL</var> is not
		''' null. This includes the case where <var>jmxServiceURL</var>
		''' contains a serialized RMIServer stub. If both
		''' <var>rmiServer</var> and <var>jmxServiceURL</var> are null then
		''' serialization will fail.
		''' </summary>
		''' <seealso cref= #RMIConnector(RMIServer,Map)
		'''  </seealso>
		private final RMIServer rmiServer

		''' <summary>
		''' @serial The JMXServiceURL of the RMI JMX Connector server to
		''' which this client connector will be connected. This field can
		''' be null when <var>rmiServer</var> is not null. If both
		''' <var>rmiServer</var> and <var>jmxServiceURL</var> are null then
		''' serialization will fail.
		''' </summary>
		''' <seealso cref= #RMIConnector(JMXServiceURL,Map)
		'''  </seealso>
		private final javax.management.remote.JMXServiceURL jmxServiceURL

		' ---------------------------------------------------------
		' WARNING - WARNING - WARNING - WARNING - WARNING - WARNING
		' ---------------------------------------------------------
		' Any transient variable which needs to be initialized should
		' be initialized in the method initTransient()
		private transient IDictionary(Of String, Object) env
		private transient ClassLoader defaultClassLoader
		private transient RMIConnection connection
		private transient String connectionId

		private transient Long clientNotifSeqNo = 0

		private transient java.util.WeakHashMap(Of javax.security.auth.Subject, WeakReference(Of javax.management.MBeanServerConnection)) rmbscMap
		private transient WeakReference(Of javax.management.MBeanServerConnection) nullSubjectConnRef = Nothing

		private transient RMINotifClient rmiNotifClient
		' = new RMINotifClient(new Integer(0));

		private transient Long clientNotifCounter = 0

		private transient Boolean connected
		' = false;
		private transient Boolean terminated
		' = false;

		private transient Exception closeException

		private transient javax.management.NotificationBroadcasterSupport connectionBroadcaster

		private transient com.sun.jmx.remote.internal.ClientCommunicatorAdmin communicatorAdmin

		''' <summary>
		''' A static WeakReference to an <seealso cref="org.omg.CORBA.ORB ORB"/> to
		''' connect unconnected stubs.
		''' 
		''' </summary>
		private static volatile WeakReference(Of Object) orb = Nothing

		' TRACES & DEBUG
		'---------------
		private static String objects(final Object() objs)
			If objs Is Nothing Then
				Return "null"
			Else
				Return java.util.Arrays.asList(objs).ToString()
			End If

		private static String strings(final String() strs)
			Return objects(strs)

		static String getAttributesNames(javax.management.AttributeList attributes)
			Return attributes IsNot Nothing ? attributes.asList().stream().map(javax.management.Attribute::getName).collect(java.util.stream.Collectors.joining("[", ", ", "]")) : "[]"
	End Class

End Namespace