Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading
Imports javax.management
import static com.sun.jmx.mbeanserver.Util.cast

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
	''' <p>Implementation of the <seealso cref="RMIConnection"/> interface.  User
	''' code will not usually reference this class.</p>
	''' 
	''' @since 1.5
	''' </summary>
	'
	' * Notice that we omit the type parameter from MarshalledObject everywhere,
	' * even though it would add useful information to the documentation.  The
	' * reason is that it was only added in Mustang (Java SE 6), whereas versions
	' * 1.4 and 2.0 of the JMX API must be implementable on Tiger per our
	' * commitments for JSR 255.
	' 
	Public Class RMIConnectionImpl
		Implements RMIConnection, java.rmi.server.Unreferenced

		''' <summary>
		''' Constructs a new <seealso cref="RMIConnection"/>. This connection can be
		''' used with either the JRMP or IIOP transport. This object does
		''' not export itself: it is the responsibility of the caller to
		''' export it appropriately (see {@link
		''' RMIJRMPServerImpl#makeClient(String,Subject)} and {@link
		''' RMIIIOPServerImpl#makeClient(String,Subject)}.
		''' </summary>
		''' <param name="rmiServer"> The RMIServerImpl object for which this
		''' connection is created.  The behavior is unspecified if this
		''' parameter is null. </param>
		''' <param name="connectionId"> The ID for this connection.  The behavior
		''' is unspecified if this parameter is null. </param>
		''' <param name="defaultClassLoader"> The default ClassLoader to be used
		''' when deserializing marshalled objects.  Can be null, to signify
		''' the bootstrap class loader. </param>
		''' <param name="subject"> the authenticated subject to be used for
		''' authorization.  Can be null, to signify that no subject has
		''' been authenticated. </param>
		''' <param name="env"> the environment containing attributes for the new
		''' <code>RMIServerImpl</code>.  Can be null, equivalent to an
		''' empty map. </param>
		Public Sub New(Of T1)(ByVal rmiServer As RMIServerImpl, ByVal connectionId As String, ByVal defaultClassLoader As ClassLoader, ByVal subject As javax.security.auth.Subject, ByVal env As IDictionary(Of T1))
			If rmiServer Is Nothing OrElse connectionId Is Nothing Then Throw New NullPointerException("Illegal null argument")
			If env Is Nothing Then env = java.util.Collections.emptyMap()
			Me.rmiServer = rmiServer
			Me.connectionId = connectionId
			Me.defaultClassLoader = defaultClassLoader

			Me.subjectDelegator = New com.sun.jmx.remote.security.SubjectDelegator
			Me.subject = subject
			If subject Is Nothing Then
				Me.acc = Nothing
				Me.removeCallerContext = False
			Else
				Me.removeCallerContext = com.sun.jmx.remote.security.SubjectDelegator.checkRemoveCallerContext(subject)
				If Me.removeCallerContext Then
					Me.acc = com.sun.jmx.remote.security.JMXSubjectDomainCombiner.getDomainCombinerContext(subject)
				Else
					Me.acc = com.sun.jmx.remote.security.JMXSubjectDomainCombiner.getContext(subject)
				End If
			End If
			Me.mbeanServer = rmiServer.mBeanServer

			Dim dcl As ClassLoader = defaultClassLoader

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			javax.management.loading.ClassLoaderRepository repository = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<javax.management.loading.ClassLoaderRepository>()
	'		{
	'				public ClassLoaderRepository run()
	'				{
	'					Return mbeanServer.getClassLoaderRepository();
	'				}
	'			},
				withPermissions(New MBeanPermission("*", "getClassLoaderRepository")))
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Me.classLoaderWithRepository = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<com.sun.jmx.remote.util.ClassLoaderWithRepository>()
	'		{
	'				public ClassLoaderWithRepository run()
	'				{
	'					Return New ClassLoaderWithRepository(repository, dcl);
	'				}
	'			},
				withPermissions(New RuntimePermission("createClassLoader")))

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Me.defaultContextClassLoader = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<ClassLoader>()
	'		{
	'			@Override public ClassLoader run()
	'			{
	'						Return New CombinedClassLoader(Thread.currentThread().getContextClassLoader(), dcl);
	'					}
	'				});

			serverCommunicatorAdmin = New RMIServerCommunicatorAdmin(Me, com.sun.jmx.remote.util.EnvHelp.getServerConnectionTimeout(env))

			Me.env = env
		End Sub

		Private Shared Function withPermissions(ParamArray ByVal perms As java.security.Permission()) As java.security.AccessControlContext
			Dim col As New java.security.Permissions

			For Each thePerm As java.security.Permission In perms
				col.add(thePerm)
			Next thePerm

			Dim pd As New java.security.ProtectionDomain(Nothing, col)
			Return New java.security.AccessControlContext(New java.security.ProtectionDomain() { pd })
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property serverNotifFwd As com.sun.jmx.remote.internal.ServerNotifForwarder
			Get
				' Lazily created when first use. Mainly when
				' addNotificationListener is first called.
				If serverNotifForwarder Is Nothing Then serverNotifForwarder = New com.sun.jmx.remote.internal.ServerNotifForwarder(mbeanServer, env, rmiServer.notifBuffer, connectionId)
				Return serverNotifForwarder
			End Get
		End Property

		Public Overridable Property connectionId As String Implements RMIConnection.getConnectionId
			Get
				' We should call reqIncomming() here... shouldn't we?
				Return connectionId
			End Get
		End Property

		Public Overridable Sub close() Implements RMIConnection.close
			Dim debug As Boolean = logger.debugOn()
			Dim idstr As String = (If(debug, "[" & Me.ToString() & "]", Nothing))

			SyncLock Me
				If terminated Then
					If debug Then logger.debug("close",idstr & " already terminated.")
					Return
				End If

				If debug Then logger.debug("close",idstr & " closing.")

				terminated = True

				If serverCommunicatorAdmin IsNot Nothing Then serverCommunicatorAdmin.terminate()

				If serverNotifForwarder IsNot Nothing Then serverNotifForwarder.terminate()
			End SyncLock

			rmiServer.clientClosed(Me)

			If debug Then logger.debug("close",idstr & " closed.")
		End Sub

		Public Overridable Sub unreferenced()
			logger.debug("unreferenced", "called")
			Try
				close()
				logger.debug("unreferenced", "done")
			Catch e As java.io.IOException
				logger.fine("unreferenced", e)
			End Try
		End Sub

		'-------------------------------------------------------------------------
		' MBeanServerConnection Wrapper
		'-------------------------------------------------------------------------

		Public Overridable Function createMBean(ByVal className As String, ByVal name As ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As ObjectInstance
			Try
				Dim params As Object() = { className, name }

				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName)", "connectionId=" & connectionId & ", className=" & className & ", name=" & name)

				Return CType(doPrivilegedOperation(CREATE_MBEAN, params, delegationSubject), ObjectInstance)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is InstanceAlreadyExistsException Then Throw CType(e, InstanceAlreadyExistsException)
				If TypeOf e Is MBeanRegistrationException Then Throw CType(e, MBeanRegistrationException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is NotCompliantMBeanException Then Throw CType(e, NotCompliantMBeanException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try
		End Function

		Public Overridable Function createMBean(ByVal className As String, ByVal name As ObjectName, ByVal loaderName As ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As ObjectInstance
			Try
				Dim params As Object() = { className, name, loaderName }

				If logger.debugOn() Then logger.debug("createMBean(String,ObjectName,ObjectName)", "connectionId=" & connectionId & ", className=" & className & ", name=" & name & ", loaderName=" & loaderName)

				Return CType(doPrivilegedOperation(CREATE_MBEAN_LOADER, params, delegationSubject), ObjectInstance)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is InstanceAlreadyExistsException Then Throw CType(e, InstanceAlreadyExistsException)
				If TypeOf e Is MBeanRegistrationException Then Throw CType(e, MBeanRegistrationException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is NotCompliantMBeanException Then Throw CType(e, NotCompliantMBeanException)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public ObjectInstance createMBean(String className, ObjectName name, java.rmi.MarshalledObject params, String signature() , javax.security.auth.Subject delegationSubject) throws ReflectionException, InstanceAlreadyExistsException, MBeanRegistrationException, MBeanException, NotCompliantMBeanException, java.io.IOException

			Dim values As Object()
			Dim debug As Boolean = logger.debugOn()

			If debug Then logger.debug("createMBean(String,ObjectName,Object[],String[])", "connectionId=" & connectionId & ", unwrapping parameters using classLoaderWithRepository.")

			values = nullIsEmpty(unwrap(params, classLoaderWithRepository, GetType(Object())))

			Try
				Dim params2 As Object() = { className, name, values, nullIsEmpty(signature) }

				If debug Then logger.debug("createMBean(String,ObjectName,Object[],String[])", "connectionId=" & connectionId & ", className=" & className & ", name=" & name & ", signature=" & strings(signature))

				Return CType(doPrivilegedOperation(CREATE_MBEAN_PARAMS, params2, delegationSubject), ObjectInstance)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is InstanceAlreadyExistsException Then Throw CType(e, InstanceAlreadyExistsException)
				If TypeOf e Is MBeanRegistrationException Then Throw CType(e, MBeanRegistrationException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is NotCompliantMBeanException Then Throw CType(e, NotCompliantMBeanException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public ObjectInstance createMBean(String className, ObjectName name, ObjectName loaderName, java.rmi.MarshalledObject params, String signature() , javax.security.auth.Subject delegationSubject) throws ReflectionException, InstanceAlreadyExistsException, MBeanRegistrationException, MBeanException, NotCompliantMBeanException, InstanceNotFoundException, java.io.IOException ' MarshalledObject

			Dim values As Object()
			Dim debug As Boolean = logger.debugOn()

			If debug Then logger.debug("createMBean(String,ObjectName,ObjectName,Object[],String[])", "connectionId=" & connectionId & ", unwrapping params with MBean extended ClassLoader.")

			values = nullIsEmpty(unwrap(params, getClassLoader(loaderName), defaultClassLoader, GetType(Object())))

			Try
				Dim params2 As Object() = { className, name, loaderName, values, nullIsEmpty(signature) }

			   If debug Then logger.debug("createMBean(String,ObjectName,ObjectName,Object[],String[])", "connectionId=" & connectionId & ", className=" & className & ", name=" & name & ", loaderName=" & loaderName & ", signature=" & strings(signature))

				Return CType(doPrivilegedOperation(CREATE_MBEAN_LOADER_PARAMS, params2, delegationSubject), ObjectInstance)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is InstanceAlreadyExistsException Then Throw CType(e, InstanceAlreadyExistsException)
				If TypeOf e Is MBeanRegistrationException Then Throw CType(e, MBeanRegistrationException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is NotCompliantMBeanException Then Throw CType(e, NotCompliantMBeanException)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public void unregisterMBean(ObjectName name, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, MBeanRegistrationException, java.io.IOException
			Try
				Dim params As Object() = { name }

				If logger.debugOn() Then logger.debug("unregisterMBean", "connectionId=" & connectionId & ", name=" & name)

				doPrivilegedOperation(UNREGISTER_MBEAN, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is MBeanRegistrationException Then Throw CType(e, MBeanRegistrationException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public ObjectInstance getObjectInstance(ObjectName name, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, java.io.IOException

			checkNonNull("ObjectName", name)

			Try
				Dim params As Object() = { name }

				If logger.debugOn() Then logger.debug("getObjectInstance", "connectionId=" & connectionId & ", name=" & name)

				Return CType(doPrivilegedOperation(GET_OBJECT_INSTANCE, params, delegationSubject), ObjectInstance)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public java.util.Set(Of ObjectInstance) queryMBeans(ObjectName name, java.rmi.MarshalledObject query, javax.security.auth.Subject delegationSubject) throws java.io.IOException ' MarshalledObject
			Dim queryValue As QueryExp
			Dim debug As Boolean=logger.debugOn()

			If debug Then logger.debug("queryMBeans", "connectionId=" & connectionId & " unwrapping query with defaultClassLoader.")

			queryValue = unwrap(query, defaultContextClassLoader, GetType(QueryExp))

			Try
				Dim params As Object() = { name, queryValue }

				If debug Then logger.debug("queryMBeans", "connectionId=" & connectionId & ", name=" & name & ", query=" & query)

				Return cast(doPrivilegedOperation(QUERY_MBEANS, params, delegationSubject))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public java.util.Set(Of ObjectName) queryNames(ObjectName name, java.rmi.MarshalledObject query, javax.security.auth.Subject delegationSubject) throws java.io.IOException ' MarshalledObject
			Dim queryValue As QueryExp
			Dim debug As Boolean=logger.debugOn()

			If debug Then logger.debug("queryNames", "connectionId=" & connectionId & " unwrapping query with defaultClassLoader.")

			queryValue = unwrap(query, defaultContextClassLoader, GetType(QueryExp))

			Try
				Dim params As Object() = { name, queryValue }

				If debug Then logger.debug("queryNames", "connectionId=" & connectionId & ", name=" & name & ", query=" & query)

				Return cast(doPrivilegedOperation(QUERY_NAMES, params, delegationSubject))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public Boolean isRegistered(ObjectName name, javax.security.auth.Subject delegationSubject) throws java.io.IOException
			Try
				Dim params As Object() = { name }
				Return CBool(doPrivilegedOperation(IS_REGISTERED, params, delegationSubject))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public Integer? getMBeanCount(javax.security.auth.Subject delegationSubject) throws java.io.IOException
			Try
				Dim params As Object() = { }

				If logger.debugOn() Then logger.debug("getMBeanCount", "connectionId=" & connectionId)

				Return CInt(Fix(doPrivilegedOperation(GET_MBEAN_COUNT, params, delegationSubject)))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public Object getAttribute(ObjectName name, String attribute, javax.security.auth.Subject delegationSubject) throws MBeanException, AttributeNotFoundException, InstanceNotFoundException, ReflectionException, java.io.IOException
			Try
				Dim params As Object() = { name, attribute }
				If logger.debugOn() Then logger.debug("getAttribute", "connectionId=" & connectionId & ", name=" & name & ", attribute=" & attribute)

				Return doPrivilegedOperation(GET_ATTRIBUTE, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is AttributeNotFoundException Then Throw CType(e, AttributeNotFoundException)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public AttributeList getAttributes(ObjectName name, String() attributes, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, ReflectionException, java.io.IOException
			Try
				Dim params As Object() = { name, attributes }

				If logger.debugOn() Then logger.debug("getAttributes", "connectionId=" & connectionId & ", name=" & name & ", attributes=" & strings(attributes))

				Return CType(doPrivilegedOperation(GET_ATTRIBUTES, params, delegationSubject), AttributeList)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public void attributeute(ObjectName name, java.rmi.MarshalledObject attribute, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, AttributeNotFoundException, InvalidAttributeValueException, MBeanException, ReflectionException, java.io.IOException ' MarshalledObject
			Dim attr As Attribute
			Dim debug As Boolean=logger.debugOn()

			If debug Then logger.debug("setAttribute", "connectionId=" & connectionId & " unwrapping attribute with MBean extended ClassLoader.")

			attr = unwrap(attribute, getClassLoaderFor(name), defaultClassLoader, GetType(Attribute))

			Try
				Dim params As Object() = { name, attr }

				If debug Then logger.debug("setAttribute", "connectionId=" & connectionId & ", name=" & name & ", attribute name=" & attr.name)

				doPrivilegedOperation(SET_ATTRIBUTE, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is AttributeNotFoundException Then Throw CType(e, AttributeNotFoundException)
				If TypeOf e Is InvalidAttributeValueException Then Throw CType(e, InvalidAttributeValueException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public AttributeList attributestes(ObjectName name, java.rmi.MarshalledObject attributes, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, ReflectionException, java.io.IOException ' MarshalledObject
			Dim attrlist As AttributeList
			Dim debug As Boolean=logger.debugOn()

			If debug Then logger.debug("setAttributes", "connectionId=" & connectionId & " unwrapping attributes with MBean extended ClassLoader.")

			attrlist = unwrap(attributes, getClassLoaderFor(name), defaultClassLoader, GetType(AttributeList))

			Try
				Dim params As Object() = { name, attrlist }

				If debug Then logger.debug("setAttributes", "connectionId=" & connectionId & ", name=" & name & ", attribute names=" & RMIConnector.getAttributesNames(attrlist))

				Return CType(doPrivilegedOperation(SET_ATTRIBUTES, params, delegationSubject), AttributeList)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public Object invoke(ObjectName name, String operationName, java.rmi.MarshalledObject params, String signature() , javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, MBeanException, ReflectionException, java.io.IOException ' MarshalledObject

			checkNonNull("ObjectName", name)
			checkNonNull("Operation name", operationName)

			Dim values As Object()
			Dim debug As Boolean=logger.debugOn()

			If debug Then logger.debug("invoke", "connectionId=" & connectionId & " unwrapping params with MBean extended ClassLoader.")

			values = nullIsEmpty(unwrap(params, getClassLoaderFor(name), defaultClassLoader, GetType(Object())))

			Try
				Dim params2 As Object() = { name, operationName, values, nullIsEmpty(signature) }

				If debug Then logger.debug("invoke", "connectionId=" & connectionId & ", name=" & name & ", operationName=" & operationName & ", signature=" & strings(signature))

				Return doPrivilegedOperation(___INVOKE, params2, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is MBeanException Then Throw CType(e, MBeanException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public String getDefaultDomain(javax.security.auth.Subject delegationSubject) throws java.io.IOException
			Try
				Dim params As Object() = { }

				If logger.debugOn() Then logger.debug("getDefaultDomain", "connectionId=" & connectionId)

				Return CStr(doPrivilegedOperation(GET_DEFAULT_DOMAIN, params, delegationSubject))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public String() getDomains(javax.security.auth.Subject delegationSubject) throws java.io.IOException
			Try
				Dim params As Object() = { }

				If logger.debugOn() Then logger.debug("getDomains", "connectionId=" & connectionId)

				Return CType(doPrivilegedOperation(GET_DOMAINS, params, delegationSubject), String())
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public MBeanInfo getMBeanInfo(ObjectName name, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, IntrospectionException, ReflectionException, java.io.IOException

			checkNonNull("ObjectName", name)

			Try
				Dim params As Object() = { name }

				If logger.debugOn() Then logger.debug("getMBeanInfo", "connectionId=" & connectionId & ", name=" & name)

				Return CType(doPrivilegedOperation(GET_MBEAN_INFO, params, delegationSubject), MBeanInfo)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is IntrospectionException Then Throw CType(e, IntrospectionException)
				If TypeOf e Is ReflectionException Then Throw CType(e, ReflectionException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public Boolean isInstanceOf(ObjectName name, String className, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, java.io.IOException

			checkNonNull("ObjectName", name)

			Try
				Dim params As Object() = { name, className }

				If logger.debugOn() Then logger.debug("isInstanceOf", "connectionId=" & connectionId & ", name=" & name & ", className=" & className)

				Return CBool(doPrivilegedOperation(IS_INSTANCE_OF, params, delegationSubject))
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public Integer?() addNotificationListeners(ObjectName() names, java.rmi.MarshalledObject() filters, javax.security.auth.Subject() delegationSubjects) throws InstanceNotFoundException, java.io.IOException ' MarshalledObject

			If names Is Nothing OrElse filters Is Nothing Then Throw New System.ArgumentException("Got null arguments.")

			Dim sbjs As javax.security.auth.Subject() = If(delegationSubjects IsNot Nothing, delegationSubjects, New javax.security.auth.Subject(names.length - 1){})
			If names.length <> filters.length OrElse filters.length <> sbjs.Length Then
				Const msg As String = "The value lengths of 3 parameters are not same."
				Throw New System.ArgumentException(msg)
			End If

			For i As Integer = 0 To names.length - 1
				If names(i) Is Nothing Then Throw New System.ArgumentException("Null Object name.")
			Next i

			Dim i As Integer=0
			Dim targetCl As ClassLoader
			Dim filterValues As NotificationFilter() = New NotificationFilter(names.length - 1){}
			Dim ids As Integer?() = New Integer?(names.length)
			Dim debug As Boolean=logger.debugOn()

			Try
				Do While i<names.length
					targetCl = getClassLoaderFor(names(i))

					If debug Then logger.debug("addNotificationListener" & "(ObjectName,NotificationFilter)", "connectionId=" & connectionId & " unwrapping filter with target extended ClassLoader.")

					filterValues(i) = unwrap(filters(i), targetCl, defaultClassLoader, GetType(NotificationFilter))

					If debug Then logger.debug("addNotificationListener" & "(ObjectName,NotificationFilter)", "connectionId=" & connectionId & ", name=" & names(i) & ", filter=" & filterValues(i))

					ids(i) = CInt(Fix(doPrivilegedOperation(ADD_NOTIFICATION_LISTENERS, New Object() { names(i), filterValues(i) }, sbjs(i))))
					i += 1
				Loop

				Return ids
			Catch e As Exception
				' remove all registered listeners
				For j As Integer = 0 To i - 1
					Try
						serverNotifFwd.removeNotificationListener(names(j), ids(j))
					Catch eee As Exception
						' strange
					End Try
				Next j

				If TypeOf e Is java.security.PrivilegedActionException Then e = extractException(e)

				If TypeOf e Is ClassCastException Then
					Throw CType(e, ClassCastException)
				ElseIf TypeOf e Is java.io.IOException Then
					Throw CType(e, java.io.IOException)
				ElseIf TypeOf e Is InstanceNotFoundException Then
					Throw CType(e, InstanceNotFoundException)
				ElseIf TypeOf e Is Exception Then
					Throw CType(e, Exception)
				Else
					Throw newIOException("Got unexpected server exception: " & e,e)
				End If
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public void addNotificationListener(ObjectName name, ObjectName listener, java.rmi.MarshalledObject filter, java.rmi.MarshalledObject handback, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, java.io.IOException ' MarshalledObject

			checkNonNull("Target MBean name", name)
			checkNonNull("Listener MBean name", listener)

			Dim filterValue As NotificationFilter
			Dim handbackValue As Object
			Dim debug As Boolean=logger.debugOn()

			Dim targetCl As ClassLoader = getClassLoaderFor(name)

			If debug Then logger.debug("addNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & " unwrapping filter with target extended ClassLoader.")

			filterValue = unwrap(filter, targetCl, defaultClassLoader, GetType(NotificationFilter))

			If debug Then logger.debug("addNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & " unwrapping handback with target extended ClassLoader.")

			handbackValue = unwrap(handback, targetCl, defaultClassLoader, GetType(Object))

			Try
				Dim params As Object() = { name, listener, filterValue, handbackValue }

				If debug Then logger.debug("addNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & ", name=" & name & ", listenerName=" & listener & ", filter=" & filterValue & ", handback=" & handbackValue)

				doPrivilegedOperation(ADD_NOTIFICATION_LISTENER_OBJECTNAME, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public void removeNotificationListeners(ObjectName name, java.lang.Integer() listenerIDs, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, ListenerNotFoundException, java.io.IOException

			If name Is Nothing OrElse listenerIDs Is Nothing Then Throw New System.ArgumentException("Illegal null parameter")

			For i As Integer = 0 To listenerIDs.length - 1
				If listenerIDs(i) Is Nothing Then Throw New System.ArgumentException("Null listener ID")
			Next i

			Try
				Dim params As Object() = { name, listenerIDs }

				If logger.debugOn() Then logger.debug("removeNotificationListener" & "(ObjectName,Integer[])", "connectionId=" & connectionId & ", name=" & name & ", listenerIDs=" & objects(listenerIDs))

				doPrivilegedOperation(REMOVE_NOTIFICATION_LISTENER, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ListenerNotFoundException Then Throw CType(e, ListenerNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public void removeNotificationListener(ObjectName name, ObjectName listener, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, ListenerNotFoundException, java.io.IOException

			checkNonNull("Target MBean name", name)
			checkNonNull("Listener MBean name", listener)

			Try
				Dim params As Object() = { name, listener }

				If logger.debugOn() Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName)", "connectionId=" & connectionId & ", name=" & name & ", listenerName=" & listener)

				doPrivilegedOperation(REMOVE_NOTIFICATION_LISTENER_OBJECTNAME, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ListenerNotFoundException Then Throw CType(e, ListenerNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public void removeNotificationListener(ObjectName name, ObjectName listener, java.rmi.MarshalledObject filter, java.rmi.MarshalledObject handback, javax.security.auth.Subject delegationSubject) throws InstanceNotFoundException, ListenerNotFoundException, java.io.IOException ' MarshalledObject

			checkNonNull("Target MBean name", name)
			checkNonNull("Listener MBean name", listener)

			Dim filterValue As NotificationFilter
			Dim handbackValue As Object
			Dim debug As Boolean=logger.debugOn()

			Dim targetCl As ClassLoader = getClassLoaderFor(name)

			If debug Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & " unwrapping filter with target extended ClassLoader.")

			filterValue = unwrap(filter, targetCl, defaultClassLoader, GetType(NotificationFilter))

			If debug Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & " unwrapping handback with target extended ClassLoader.")

			handbackValue = unwrap(handback, targetCl, defaultClassLoader, GetType(Object))

			Try
				Dim params As Object() = { name, listener, filterValue, handbackValue }

				If debug Then logger.debug("removeNotificationListener" & "(ObjectName,ObjectName,NotificationFilter,Object)", "connectionId=" & connectionId & ", name=" & name & ", listenerName=" & listener & ", filter=" & filterValue & ", handback=" & handbackValue)

				doPrivilegedOperation(REMOVE_NOTIFICATION_LISTENER_OBJECTNAME_FILTER_HANDBACK, params, delegationSubject)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is InstanceNotFoundException Then Throw CType(e, InstanceNotFoundException)
				If TypeOf e Is ListenerNotFoundException Then Throw CType(e, ListenerNotFoundException)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				Throw newIOException("Got unexpected server exception: " & e, e)
			End Try

		public javax.management.remote.NotificationResult fetchNotifications(Long clientSequenceNumber, Integer maxNotifications, Long timeout) throws java.io.IOException

			If logger.debugOn() Then logger.debug("fetchNotifications", "connectionId=" & connectionId & ", timeout=" & timeout)

			If maxNotifications < 0 OrElse timeout < 0 Then Throw New System.ArgumentException("Illegal negative argument")

			Dim serverTerminated As Boolean = serverCommunicatorAdmin.reqIncoming()
			Try
				If serverTerminated Then
					' we must not call fetchNotifs() if the server is
					' terminated (timeout elapsed).
					' returns null to force the client to stop fetching
					If logger.debugOn() Then logger.debug("fetchNotifications", "The notification server has been closed, " & "returns null to force the client to stop fetching")
					Return Nothing
				End If
				Dim csn As Long = clientSequenceNumber
				Dim mn As Integer = maxNotifications
				Dim t As Long = timeout
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.PrivilegedAction<javax.management.remote.NotificationResult> action = New java.security.PrivilegedAction<javax.management.remote.NotificationResult>()
	'			{
	'					public NotificationResult run()
	'					{
	'						Return getServerNotifFwd().fetchNotifs(csn, t, mn);
	'					}
	'			};
				If acc Is Nothing Then
					Return action.run()
				Else
					Return java.security.AccessController.doPrivileged(action, acc)
				End If
			Finally
				serverCommunicatorAdmin.rspOutgoing()
			End Try

		''' <summary>
		''' <p>Returns a string representation of this object.  In general,
		''' the <code>toString</code> method returns a string that
		''' "textually represents" this object. The result should be a
		''' concise but informative representation that is easy for a
		''' person to read.</p>
		''' </summary>
		''' <returns> a String representation of this object.
		'''  </returns>
		public String ToString()
			Return MyBase.ToString() & ": connectionId=" & connectionId

		'------------------------------------------------------------------------
		' private classes
		'------------------------------------------------------------------------

		private class PrivilegedOperation implements java.security.PrivilegedExceptionAction(Of Object)

			public PrivilegedOperation(Integer operation, Object() params)
				Me.operation = operation
				Me.params = params

			public Object run() throws Exception
				Return doOperation(operation, params)

			private Integer operation
			private Object() params

		'------------------------------------------------------------------------
		' private classes
		'------------------------------------------------------------------------
		private class RMIServerCommunicatorAdmin extends com.sun.jmx.remote.internal.ServerCommunicatorAdmin
			public RMIServerCommunicatorAdmin(Long timeout)
				MyBase(timeout)

			protected void doStop()
				Try
					close()
				Catch ie As java.io.IOException
					logger.warning("RMIServerCommunicatorAdmin-doStop", "Failed to close: " & ie)
					logger.debug("RMIServerCommunicatorAdmin-doStop",ie)
				End Try



		'------------------------------------------------------------------------
		' private methods
		'------------------------------------------------------------------------

		private ClassLoader getClassLoader(final ObjectName name) throws InstanceNotFoundException
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<ClassLoader>()
	'			{
	'						public ClassLoader run() throws InstanceNotFoundException
	'						{
	'							Return mbeanServer.getClassLoader(name);
	'						}
	'					},
						withPermissions(New MBeanPermission("*", "getClassLoader")))
			Catch pe As java.security.PrivilegedActionException
				Throw CType(extractException(pe), InstanceNotFoundException)
			End Try

		private ClassLoader getClassLoaderFor(final ObjectName name) throws InstanceNotFoundException
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<Object>()
	'			{
	'						public Object run() throws InstanceNotFoundException
	'						{
	'							Return mbeanServer.getClassLoaderFor(name);
	'						}
	'					},
						withPermissions(New MBeanPermission("*", "getClassLoaderFor")))
			Catch pe As java.security.PrivilegedActionException
				Throw CType(extractException(pe), InstanceNotFoundException)
			End Try

		private Object doPrivilegedOperation(final Integer operation, final Object() params, final javax.security.auth.Subject delegationSubject) throws java.security.PrivilegedActionException, java.io.IOException

			serverCommunicatorAdmin.reqIncoming()
			Try

				Dim reqACC As java.security.AccessControlContext
				If delegationSubject Is Nothing Then
					reqACC = acc
				Else
					If subject Is Nothing Then
						Dim msg As String = "Subject delegation cannot be enabled unless " & "an authenticated subject is put in place"
						Throw New SecurityException(msg)
					End If
					reqACC = subjectDelegator.delegatedContext(acc, delegationSubject, removeCallerContext)
				End If

				Dim op As New PrivilegedOperation(Me, operation, params)
				If reqACC Is Nothing Then
					Try
						Return op.run()
					Catch e As Exception
						If TypeOf e Is Exception Then Throw CType(e, Exception)
						Throw New java.security.PrivilegedActionException(e)
					End Try
				Else
					Return java.security.AccessController.doPrivileged(op, reqACC)
				End If
			Catch e As Exception
				Throw New javax.management.remote.JMXServerErrorException(e.ToString(),e)
			Finally
				serverCommunicatorAdmin.rspOutgoing()
			End Try

		private Object doOperation(Integer operation, Object() params) throws Exception

			Select Case operation

			Case CREATE_MBEAN
				Return mbeanServer.createMBean(CStr(params(0)), CType(params(1), ObjectName))

			Case CREATE_MBEAN_LOADER
				Return mbeanServer.createMBean(CStr(params(0)), CType(params(1), ObjectName), CType(params(2), ObjectName))

			Case CREATE_MBEAN_PARAMS
				Return mbeanServer.createMBean(CStr(params(0)), CType(params(1), ObjectName), CType(params(2), Object()), CType(params(3), String()))

			Case CREATE_MBEAN_LOADER_PARAMS
				Return mbeanServer.createMBean(CStr(params(0)), CType(params(1), ObjectName), CType(params(2), ObjectName), CType(params(3), Object()), CType(params(4), String()))

			Case GET_ATTRIBUTE
				Return mbeanServer.getAttribute(CType(params(0), ObjectName), CStr(params(1)))

			Case GET_ATTRIBUTES
				Return mbeanServer.getAttributes(CType(params(0), ObjectName), CType(params(1), String()))

			Case GET_DEFAULT_DOMAIN
				Return mbeanServer.defaultDomain

			Case GET_DOMAINS
				Return mbeanServer.domains

			Case GET_MBEAN_COUNT
				Return mbeanServer.mBeanCount

			Case GET_MBEAN_INFO
				Return mbeanServer.getMBeanInfo(CType(params(0), ObjectName))

			Case GET_OBJECT_INSTANCE
				Return mbeanServer.getObjectInstance(CType(params(0), ObjectName))

			Case ___INVOKE
				Return mbeanServer.invoke(CType(params(0), ObjectName), CStr(params(1)), CType(params(2), Object()), CType(params(3), String()))

			Case IS_INSTANCE_OF
				Return If(mbeanServer.isInstanceOf(CType(params(0), ObjectName), CStr(params(1))), Boolean.TRUE, Boolean.FALSE)

			Case IS_REGISTERED
				Return If(mbeanServer.isRegistered(CType(params(0), ObjectName)), Boolean.TRUE, Boolean.FALSE)

			Case QUERY_MBEANS
				Return mbeanServer.queryMBeans(CType(params(0), ObjectName), CType(params(1), QueryExp))

			Case QUERY_NAMES
				Return mbeanServer.queryNames(CType(params(0), ObjectName), CType(params(1), QueryExp))

			Case SET_ATTRIBUTE
				mbeanServer.attributeute(CType(params(0), ObjectName), CType(params(1), Attribute))
				Return Nothing

			Case SET_ATTRIBUTES
				Return mbeanServer.attributestes(CType(params(0), ObjectName), CType(params(1), AttributeList))

			Case UNREGISTER_MBEAN
				mbeanServer.unregisterMBean(CType(params(0), ObjectName))
				Return Nothing

			Case ADD_NOTIFICATION_LISTENERS
				Return serverNotifFwd.addNotificationListener(CType(params(0), ObjectName), CType(params(1), NotificationFilter))

			Case ADD_NOTIFICATION_LISTENER_OBJECTNAME
				mbeanServer.addNotificationListener(CType(params(0), ObjectName), CType(params(1), ObjectName), CType(params(2), NotificationFilter), params(3))
				Return Nothing

			Case REMOVE_NOTIFICATION_LISTENER
				serverNotifFwd.removeNotificationListener(CType(params(0), ObjectName), CType(params(1), Integer?()))
				Return Nothing

			Case REMOVE_NOTIFICATION_LISTENER_OBJECTNAME
				mbeanServer.removeNotificationListener(CType(params(0), ObjectName), CType(params(1), ObjectName))
				Return Nothing

			Case REMOVE_NOTIFICATION_LISTENER_OBJECTNAME_FILTER_HANDBACK
				mbeanServer.removeNotificationListener(CType(params(0), ObjectName), CType(params(1), ObjectName), CType(params(2), NotificationFilter), params(3))
				Return Nothing

			Case Else
				Throw New System.ArgumentException("Invalid operation")
			End Select

		private static class SetCcl implements java.security.PrivilegedExceptionAction(Of ClassLoader)
			private final ClassLoader classLoader

			SetCcl(ClassLoader classLoader)
				Me.classLoader = classLoader

			public ClassLoader run()
				Dim currentThread As Thread = Thread.CurrentThread
				Dim old As ClassLoader = currentThread.contextClassLoader
				currentThread.contextClassLoader = classLoader
				Return old

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static (Of T) T unwrap(final java.rmi.MarshalledObject(Of ?) mo, final ClassLoader cl, final Type wrappedClass) throws java.io.IOException
			If mo Is Nothing Then Return Nothing
			Try
				Dim old As ClassLoader = java.security.AccessController.doPrivileged(New SetCcl(cl))
				Try
					Return wrappedClass.cast(mo.get())
				Catch cnfe As ClassNotFoundException
					Throw New java.rmi.UnmarshalException(cnfe.ToString(), cnfe)
				Finally
					java.security.AccessController.doPrivileged(New SetCcl(old))
				End Try
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				If TypeOf e Is ClassNotFoundException Then Throw New java.rmi.UnmarshalException(e.ToString(), e)
				logger.warning("unwrap", "Failed to unmarshall object: " & e)
				logger.debug("unwrap", e)
			End Try
			Return Nothing

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private static (Of T) T unwrap(final java.rmi.MarshalledObject(Of ?) mo, final ClassLoader cl1, final ClassLoader cl2, final Type wrappedClass) throws java.io.IOException
			If mo Is Nothing Then Return Nothing
			Try
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				ClassLoader orderCL = java.security.AccessController.doPrivileged(New java.security.PrivilegedExceptionAction<ClassLoader>()
	'			{
	'					public ClassLoader run() throws Exception
	'					{
	'						Return New CombinedClassLoader(Thread.currentThread().getContextClassLoader(), New OrderClassLoaders(cl1, cl2));
	'					}
	'				}
			   )
				Return unwrap(mo, orderCL, wrappedClass)
			Catch pe As java.security.PrivilegedActionException
				Dim e As Exception = extractException(pe)
				If TypeOf e Is java.io.IOException Then Throw CType(e, java.io.IOException)
				If TypeOf e Is ClassNotFoundException Then Throw New java.rmi.UnmarshalException(e.ToString(), e)
				logger.warning("unwrap", "Failed to unmarshall object: " & e)
				logger.debug("unwrap", e)
			End Try
			Return Nothing

		''' <summary>
		''' Construct a new IOException with a nested exception.
		''' The nested exception is set only if JDK {@literal >= 1.4}
		''' </summary>
		private static java.io.IOException newIOException(String message, Exception cause)
			Dim x As New java.io.IOException(message)
			Return com.sun.jmx.remote.util.EnvHelp.initCause(x,cause)

		''' <summary>
		''' Iterate until we extract the real exception
		''' from a stack of PrivilegedActionExceptions.
		''' </summary>
		private static Exception extractException(Exception e)
			Do While TypeOf e Is java.security.PrivilegedActionException
				e = CType(e, java.security.PrivilegedActionException).exception
			Loop
			Return e

		private static final Object() NO_OBJECTS = New Object(){}
		private static final String() NO_STRINGS = New String(){}

	'    
	'     * The JMX spec doesn't explicitly say that a null Object[] or
	'     * String[] in e.g. MBeanServer.invoke is equivalent to an empty
	'     * array, but the RI behaves that way.  In the interests of
	'     * maximal interoperability, we make it so even when we're
	'     * connected to some other JMX implementation that might not do
	'     * that.  This should be clarified in the next version of JMX.
	'     
		private static Object() nullIsEmpty(Object() array)
			Return If(array Is Nothing, NO_OBJECTS, array)

		private static String() nullIsEmpty(String() array)
			Return If(array Is Nothing, NO_STRINGS, array)

	'    
	'     * Similarly, the JMX spec says for some but not all methods in
	'     * MBeanServer that take an ObjectName target, that if it's null
	'     * you get this exception.  We specify it for all of them, and
	'     * make it so for the ones where it's not specified in JMX even if
	'     * the JMX implementation doesn't do so.
	'     
		private static void checkNonNull(String what, Object x)
			If x Is Nothing Then
				Dim wrapped As Exception = New System.ArgumentException(what & " must not be null")
				Throw New RuntimeOperationsException(wrapped)
			End If

		'------------------------------------------------------------------------
		' private variables
		'------------------------------------------------------------------------

		private final javax.security.auth.Subject subject

		private final com.sun.jmx.remote.security.SubjectDelegator subjectDelegator

		private final Boolean removeCallerContext

		private final java.security.AccessControlContext acc

		private final RMIServerImpl rmiServer

		private final MBeanServer mbeanServer

		private final ClassLoader defaultClassLoader

		private final ClassLoader defaultContextClassLoader

		private final com.sun.jmx.remote.util.ClassLoaderWithRepository classLoaderWithRepository

		private Boolean terminated = False

		private final String connectionId

		private final com.sun.jmx.remote.internal.ServerCommunicatorAdmin serverCommunicatorAdmin

		' Method IDs for doOperation
		'---------------------------

		private final static Integer ADD_NOTIFICATION_LISTENERS = 1
		private final static Integer ADD_NOTIFICATION_LISTENER_OBJECTNAME = 2
		private final static Integer CREATE_MBEAN = 3
		private final static Integer CREATE_MBEAN_PARAMS = 4
		private final static Integer CREATE_MBEAN_LOADER = 5
		private final static Integer CREATE_MBEAN_LOADER_PARAMS = 6
		private final static Integer GET_ATTRIBUTE = 7
		private final static Integer GET_ATTRIBUTES = 8
		private final static Integer GET_DEFAULT_DOMAIN = 9
		private final static Integer GET_DOMAINS = 10
		private final static Integer GET_MBEAN_COUNT = 11
		private final static Integer GET_MBEAN_INFO = 12
		private final static Integer GET_OBJECT_INSTANCE = 13
		private final static Integer ___INVOKE = 14
		private final static Integer IS_INSTANCE_OF = 15
		private final static Integer IS_REGISTERED = 16
		private final static Integer QUERY_MBEANS = 17
		private final static Integer QUERY_NAMES = 18
		private final static Integer REMOVE_NOTIFICATION_LISTENER = 19
		private final static Integer REMOVE_NOTIFICATION_LISTENER_OBJECTNAME = 20
		private final static Integer REMOVE_NOTIFICATION_LISTENER_OBJECTNAME_FILTER_HANDBACK = 21
		private final static Integer SET_ATTRIBUTE = 22
		private final static Integer SET_ATTRIBUTES = 23
		private final static Integer UNREGISTER_MBEAN = 24

		' SERVER NOTIFICATION
		'--------------------

		private com.sun.jmx.remote.internal.ServerNotifForwarder serverNotifForwarder
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		private IDictionary(Of String, ?) env

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

		private static final com.sun.jmx.remote.util.ClassLogger logger = New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.rmi", "RMIConnectionImpl")

		private static final class CombinedClassLoader extends ClassLoader

			private final static class ClassLoaderWrapper extends ClassLoader
				ClassLoaderWrapper(ClassLoader cl)
					MyBase(cl)

				protected Type loadClass(String name, Boolean resolve) throws ClassNotFoundException
					Return MyBase.loadClass(name, resolve)

			Dim defaultCL As ClassLoaderWrapper

			private CombinedClassLoader(ClassLoader parent, ClassLoader defaultCL)
				MyBase(parent)
				Me.defaultCL = New ClassLoaderWrapper(defaultCL)

			protected Type loadClass(String name, Boolean resolve) throws ClassNotFoundException
				sun.reflect.misc.ReflectUtil.checkPackageAccess(name)
				Try
					MyBase.loadClass(name, resolve)
				Catch e As Exception
					Dim t As Exception = e
					Do While t IsNot Nothing
						If TypeOf t Is SecurityException Then Throw If(t Is e, CType(t, SecurityException), New SecurityException(t.Message, e))
						t = t.InnerException
					Loop
				End Try
				Dim cl As Type = defaultCL.loadClass(name, resolve)
				Return cl

	End Class

End Namespace