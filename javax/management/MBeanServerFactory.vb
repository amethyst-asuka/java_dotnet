Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
import static com.sun.jmx.defaults.JmxProperties.JMX_INITIAL_BUILDER
import static com.sun.jmx.defaults.JmxProperties.MBEANSERVER_LOGGER

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management



	''' <summary>
	''' <p>Provides MBean server references.  There are no instances of
	''' this class.</p>
	''' 
	''' <p>Since JMX 1.2 this class makes it possible to replace the default
	''' MBeanServer implementation. This is done using the
	''' <seealso cref="javax.management.MBeanServerBuilder"/> class.
	''' The class of the initial MBeanServerBuilder to be
	''' instantiated can be specified through the
	''' <b>javax.management.builder.initial</b> system property.
	''' The specified class must be a public subclass of
	''' <seealso cref="javax.management.MBeanServerBuilder"/>, and must have a public
	''' empty constructor.
	''' <p>By default, if no value for that property is specified, an instance of
	''' {@link
	''' javax.management.MBeanServerBuilder javax.management.MBeanServerBuilder}
	''' is created. Otherwise, the MBeanServerFactory attempts to load the
	''' specified class using
	''' {@link java.lang.Thread#getContextClassLoader()
	'''   Thread.currentThread().getContextClassLoader()}, or if that is null,
	''' <seealso cref="java.lang.Class#forName(java.lang.String) Class.forName()"/>. Then
	''' it creates an initial instance of that Class using
	''' <seealso cref="java.lang.Class#newInstance()"/>. If any checked exception
	''' is raised during this process (e.g.
	''' <seealso cref="java.lang.ClassNotFoundException"/>,
	''' <seealso cref="java.lang.InstantiationException"/>) the MBeanServerFactory
	''' will propagate this exception from within a RuntimeException.</p>
	''' 
	''' <p>The <b>javax.management.builder.initial</b> system property is
	''' consulted every time a new MBeanServer needs to be created, and the
	''' class pointed to by that property is loaded. If that class is different
	''' from that of the current MBeanServerBuilder, then a new MBeanServerBuilder
	''' is created. Otherwise, the MBeanServerFactory may create a new
	''' MBeanServerBuilder or reuse the current one.</p>
	''' 
	''' <p>If the class pointed to by the property cannot be
	''' loaded, or does not correspond to a valid subclass of MBeanServerBuilder
	''' then an exception is propagated, and no MBeanServer can be created until
	''' the <b>javax.management.builder.initial</b> system property is reset to
	''' valid value.</p>
	''' 
	''' <p>The MBeanServerBuilder makes it possible to wrap the MBeanServers
	''' returned by the default MBeanServerBuilder implementation, for the purpose
	''' of e.g. adding an additional security layer.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanServerFactory

	'    
	'     * There are no instances of this class so don't generate the
	'     * default public constructor.
	'     
		Private Sub New()

		End Sub

		''' <summary>
		''' The builder that will be used to construct MBeanServers.
		''' 
		''' 
		''' </summary>
		Private Shared builder As MBeanServerBuilder = Nothing

		''' <summary>
		''' Provide a new <seealso cref="javax.management.MBeanServerBuilder"/>. </summary>
		''' <param name="builder"> The new MBeanServerBuilder that will be used to
		'''        create <seealso cref="javax.management.MBeanServer"/>s. </param>
		''' <exception cref="IllegalArgumentException"> if the given builder is null.
		''' </exception>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("setMBeanServerBuilder")</code>.
		''' 
		'''  </exception>
		' public static synchronized void
		'    setMBeanServerBuilder(MBeanServerBuilder builder) {
		'    checkPermission("setMBeanServerBuilder");
		'    MBeanServerFactory.builder = builder;
		' }

		''' <summary>
		''' Get the current <seealso cref="javax.management.MBeanServerBuilder"/>.
		''' </summary>
		''' <returns> the current <seealso cref="javax.management.MBeanServerBuilder"/>.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("getMBeanServerBuilder")</code>.
		''' 
		'''  </exception>
		' public static synchronized MBeanServerBuilder getMBeanServerBuilder() {
		'     checkPermission("getMBeanServerBuilder");
		'     return builder;
		' }

		''' <summary>
		''' Remove internal MBeanServerFactory references to a created
		''' MBeanServer. This allows the garbage collector to remove the
		''' MBeanServer object.
		''' </summary>
		''' <param name="mbeanServer"> the MBeanServer object to remove.
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if
		''' <code>mbeanServer</code> was not generated by one of the
		''' <code>createMBeanServer</code> methods, or if
		''' <code>releaseMBeanServer</code> was already called on it.
		''' </exception>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("releaseMBeanServer")</code>. </exception>
		Public Shared Sub releaseMBeanServer(ByVal mbeanServer As MBeanServer)
			checkPermission("releaseMBeanServer")

			removeMBeanServer(mbeanServer)
		End Sub

		''' <summary>
		''' <p>Return a new object implementing the MBeanServer interface
		''' with a standard default domain name.  The default domain name
		''' is used as the domain part in the ObjectName of MBeans when the
		''' domain is specified by the user is null.</p>
		''' 
		''' <p>The standard default domain name is
		''' <code>DefaultDomain</code>.</p>
		''' 
		''' <p>The MBeanServer reference is internally kept. This will
		''' allow <CODE>findMBeanServer</CODE> to return a reference to
		''' this MBeanServer object.</p>
		''' 
		''' <p>This method is equivalent to <code>createMBeanServer(null)</code>.
		''' </summary>
		''' <returns> the newly created MBeanServer.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and the
		''' caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("createMBeanServer")</code>.
		''' </exception>
		''' <exception cref="JMRuntimeException"> if the property
		''' <code>javax.management.builder.initial</code> exists but the
		''' class it names cannot be instantiated through a public
		''' no-argument constructor; or if the instantiated builder returns
		''' null from its {@link MBeanServerBuilder#newMBeanServerDelegate
		''' newMBeanServerDelegate} or {@link
		''' MBeanServerBuilder#newMBeanServer newMBeanServer} methods.
		''' </exception>
		''' <exception cref="ClassCastException"> if the property
		''' <code>javax.management.builder.initial</code> exists and can be
		''' instantiated but is not assignment compatible with {@link
		''' MBeanServerBuilder}. </exception>
		Public Shared Function createMBeanServer() As MBeanServer
			Return createMBeanServer(Nothing)
		End Function

		''' <summary>
		''' <p>Return a new object implementing the <seealso cref="MBeanServer"/>
		''' interface with the specified default domain name.  The given
		''' domain name is used as the domain part in the ObjectName of
		''' MBeans when the domain is specified by the user is null.</p>
		''' 
		''' <p>The MBeanServer reference is internally kept. This will
		''' allow <CODE>findMBeanServer</CODE> to return a reference to
		''' this MBeanServer object.</p>
		''' </summary>
		''' <param name="domain"> the default domain name for the created
		''' MBeanServer.  This is the value that will be returned by {@link
		''' MBeanServer#getDefaultDomain}.
		''' </param>
		''' <returns> the newly created MBeanServer.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("createMBeanServer")</code>.
		''' </exception>
		''' <exception cref="JMRuntimeException"> if the property
		''' <code>javax.management.builder.initial</code> exists but the
		''' class it names cannot be instantiated through a public
		''' no-argument constructor; or if the instantiated builder returns
		''' null from its {@link MBeanServerBuilder#newMBeanServerDelegate
		''' newMBeanServerDelegate} or {@link
		''' MBeanServerBuilder#newMBeanServer newMBeanServer} methods.
		''' </exception>
		''' <exception cref="ClassCastException"> if the property
		''' <code>javax.management.builder.initial</code> exists and can be
		''' instantiated but is not assignment compatible with {@link
		''' MBeanServerBuilder}. </exception>
		Public Shared Function createMBeanServer(ByVal domain As String) As MBeanServer
			checkPermission("createMBeanServer")

			Dim mBeanServer As MBeanServer = newMBeanServer(domain)
			addMBeanServer(mBeanServer)
			Return mBeanServer
		End Function

		''' <summary>
		''' <p>Return a new object implementing the MBeanServer interface
		''' with a standard default domain name, without keeping an
		''' internal reference to this new object.  The default domain name
		''' is used as the domain part in the ObjectName of MBeans when the
		''' domain is specified by the user is null.</p>
		''' 
		''' <p>The standard default domain name is
		''' <code>DefaultDomain</code>.</p>
		''' 
		''' <p>No reference is kept. <CODE>findMBeanServer</CODE> will not
		''' be able to return a reference to this MBeanServer object, but
		''' the garbage collector will be able to remove the MBeanServer
		''' object when it is no longer referenced.</p>
		''' 
		''' <p>This method is equivalent to <code>newMBeanServer(null)</code>.</p>
		''' </summary>
		''' <returns> the newly created MBeanServer.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and the
		''' caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("newMBeanServer")</code>.
		''' </exception>
		''' <exception cref="JMRuntimeException"> if the property
		''' <code>javax.management.builder.initial</code> exists but the
		''' class it names cannot be instantiated through a public
		''' no-argument constructor; or if the instantiated builder returns
		''' null from its {@link MBeanServerBuilder#newMBeanServerDelegate
		''' newMBeanServerDelegate} or {@link
		''' MBeanServerBuilder#newMBeanServer newMBeanServer} methods.
		''' </exception>
		''' <exception cref="ClassCastException"> if the property
		''' <code>javax.management.builder.initial</code> exists and can be
		''' instantiated but is not assignment compatible with {@link
		''' MBeanServerBuilder}. </exception>
		Public Shared Function newMBeanServer() As MBeanServer
			Return newMBeanServer(Nothing)
		End Function

		''' <summary>
		''' <p>Return a new object implementing the MBeanServer interface
		''' with the specified default domain name, without keeping an
		''' internal reference to this new object.  The given domain name
		''' is used as the domain part in the ObjectName of MBeans when the
		''' domain is specified by the user is null.</p>
		''' 
		''' <p>No reference is kept. <CODE>findMBeanServer</CODE> will not
		''' be able to return a reference to this MBeanServer object, but
		''' the garbage collector will be able to remove the MBeanServer
		''' object when it is no longer referenced.</p>
		''' </summary>
		''' <param name="domain"> the default domain name for the created
		''' MBeanServer.  This is the value that will be returned by {@link
		''' MBeanServer#getDefaultDomain}.
		''' </param>
		''' <returns> the newly created MBeanServer.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and the
		''' caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("newMBeanServer")</code>.
		''' </exception>
		''' <exception cref="JMRuntimeException"> if the property
		''' <code>javax.management.builder.initial</code> exists but the
		''' class it names cannot be instantiated through a public
		''' no-argument constructor; or if the instantiated builder returns
		''' null from its {@link MBeanServerBuilder#newMBeanServerDelegate
		''' newMBeanServerDelegate} or {@link
		''' MBeanServerBuilder#newMBeanServer newMBeanServer} methods.
		''' </exception>
		''' <exception cref="ClassCastException"> if the property
		''' <code>javax.management.builder.initial</code> exists and can be
		''' instantiated but is not assignment compatible with {@link
		''' MBeanServerBuilder}. </exception>
		Public Shared Function newMBeanServer(ByVal domain As String) As MBeanServer
			checkPermission("newMBeanServer")

			' Get the builder. Creates a new one if necessary.
			'
			Dim mbsBuilder As MBeanServerBuilder = newMBeanServerBuilder
			' Returned value cannot be null.  NullPointerException if violated.

			SyncLock mbsBuilder
				Dim [delegate] As MBeanServerDelegate = mbsBuilder.newMBeanServerDelegate()
				If [delegate] Is Nothing Then
					Dim msg As String = "MBeanServerBuilder.newMBeanServerDelegate() " & "returned null"
					Throw New JMRuntimeException(msg)
				End If
				Dim mbeanServer As MBeanServer = mbsBuilder.newMBeanServer(domain,Nothing,[delegate])
				If mbeanServer Is Nothing Then
					Const msg As String = "MBeanServerBuilder.newMBeanServer() returned null"
					Throw New JMRuntimeException(msg)
				End If
				Return mbeanServer
			End SyncLock
		End Function

		''' <summary>
		''' <p>Return a list of registered MBeanServer objects.  A
		''' registered MBeanServer object is one that was created by one of
		''' the <code>createMBeanServer</code> methods and not subsequently
		''' released with <code>releaseMBeanServer</code>.</p>
		''' </summary>
		''' <param name="agentId"> The agent identifier of the MBeanServer to
		''' retrieve.  If this parameter is null, all registered
		''' MBeanServers in this JVM are returned.  Otherwise, only
		''' MBeanServers whose id is equal to <code>agentId</code> are
		''' returned.  The id of an MBeanServer is the
		''' <code>MBeanServerId</code> attribute of its delegate MBean.
		''' </param>
		''' <returns> A list of MBeanServer objects.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and the
		''' caller's permissions do not include or imply <code>{@link
		''' MBeanServerPermission}("findMBeanServer")</code>. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function findMBeanServer(ByVal agentId As String) As List(Of MBeanServer)

			checkPermission("findMBeanServer")

			If agentId Is Nothing Then Return New List(Of MBeanServer)(mBeanServerList)

			Dim result As New List(Of MBeanServer)
			For Each mbs As MBeanServer In mBeanServerList
				Dim name As String = mBeanServerId(mbs)
				If agentId.Equals(name) Then result.Add(mbs)
			Next mbs
			Return result
		End Function

		''' <summary>
		''' Return the ClassLoaderRepository used by the given MBeanServer.
		''' This method is equivalent to {@link
		''' MBeanServer#getClassLoaderRepository() server.getClassLoaderRepository()}. </summary>
		''' <param name="server"> The MBeanServer under examination. Since JMX 1.2,
		''' if <code>server</code> is <code>null</code>, the result is a
		''' <seealso cref="NullPointerException"/>.  This behavior differs from what
		''' was implemented in JMX 1.1 - where the possibility to use
		''' <code>null</code> was deprecated. </param>
		''' <returns> The Class Loader Repository used by the given MBeanServer. </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not include or imply <code>{@link
		''' MBeanPermission}("getClassLoaderRepository")</code>.
		''' </exception>
		''' <exception cref="NullPointerException"> if <code>server</code> is null.
		''' 
		'''  </exception>
		Public Shared Function getClassLoaderRepository(ByVal server As MBeanServer) As javax.management.loading.ClassLoaderRepository
			Return server.classLoaderRepository
		End Function

		Private Shared Function mBeanServerId(ByVal mbs As MBeanServer) As String
			Try
				Return CStr(mbs.getAttribute(MBeanServerDelegate.DELEGATE_NAME, "MBeanServerId"))
			Catch e As JMException
				com.sun.jmx.defaults.JmxProperties.MISC_LOGGER.finest("Ignoring exception while getting MBeanServerId: " & e)
				Return Nothing
			End Try
		End Function

		Private Shared Sub checkPermission(ByVal action As String)
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim perm As java.security.Permission = New MBeanServerPermission(action)
				sm.checkPermission(perm)
			End If
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub addMBeanServer(ByVal mbs As MBeanServer)
			mBeanServerList.Add(mbs)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub removeMBeanServer(ByVal mbs As MBeanServer)
			Dim removed As Boolean = mBeanServerList.Remove(mbs)
			If Not removed Then
				MBEANSERVER_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerFactory).name, "removeMBeanServer(MBeanServer)", "MBeanServer was not in list!")
				Throw New System.ArgumentException("MBeanServer was not in list!")
			End If
		End Sub

		Private Shared ReadOnly mBeanServerList As New List(Of MBeanServer)

		''' <summary>
		''' Load the builder class through the context class loader. </summary>
		''' <param name="builderClassName"> The name of the builder class.
		'''  </param>
		Private Shared Function loadBuilderClass(ByVal builderClassName As String) As Type
			Dim loader As ClassLoader = Thread.CurrentThread.contextClassLoader

			If loader IsNot Nothing Then Return loader.loadClass(builderClassName)

			' No context class loader? Try with Class.forName()
			Return sun.reflect.misc.ReflectUtil.forName(builderClassName)
		End Function

		''' <summary>
		''' Creates the initial builder according to the
		''' javax.management.builder.initial System property - if specified.
		''' If any checked exception needs to be thrown, it is embedded in
		''' a JMRuntimeException.
		''' 
		''' </summary>
		Private Shared Function newBuilder(ByVal builderClass As Type) As MBeanServerBuilder
			Try
				Dim abuilder As Object = builderClass.newInstance()
				Return CType(abuilder, MBeanServerBuilder)
			Catch x As Exception
				Throw x
			Catch x As Exception
				Dim msg As String = "Failed to instantiate a MBeanServerBuilder from " & builderClass & ": " & x
				Throw New JMRuntimeException(msg, x)
			End Try
		End Function

		''' <summary>
		''' Instantiate a new builder according to the
		''' javax.management.builder.initial System property - if needed.
		''' 
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub checkMBeanServerBuilder()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction(JMX_INITIAL_BUILDER)
				Dim builderClassName As String = java.security.AccessController.doPrivileged(act)

				Try
					Dim newBuilderClass As Type
					If builderClassName Is Nothing OrElse builderClassName.Length = 0 Then
						newBuilderClass = GetType(MBeanServerBuilder)
					Else
						newBuilderClass = loadBuilderClass(builderClassName)
					End If

					' Check whether a new builder needs to be created
					If builder IsNot Nothing Then
						Dim builderClass As Type = builder.GetType()
						If newBuilderClass Is builderClass Then Return ' no need to create a new builder...
					End If

					' Create a new builder
					builder = newBuilder(newBuilderClass)
				Catch x As ClassNotFoundException
					Dim msg As String = "Failed to load MBeanServerBuilder class " & builderClassName & ": " & x
					Throw New JMRuntimeException(msg, x)
				End Try
			Catch x As Exception
				If MBEANSERVER_LOGGER.isLoggable(java.util.logging.Level.FINEST) Then
					Dim strb As (New StringBuilder).Append("Failed to instantiate MBeanServerBuilder: ").append(x).append(vbLf & vbTab & vbTab & "Check the value of the ").append(JMX_INITIAL_BUILDER).append(" property.")
					MBEANSERVER_LOGGER.logp(java.util.logging.Level.FINEST, GetType(MBeanServerFactory).name, "checkMBeanServerBuilder", strb.ToString())
				End If
				Throw x
			End Try
		End Sub

		''' <summary>
		''' Get the current <seealso cref="javax.management.MBeanServerBuilder"/>,
		''' as specified by the current value of the
		''' javax.management.builder.initial property.
		''' 
		''' This method consults the property and instantiates a new builder
		''' if needed.
		''' </summary>
		''' <returns> the new current <seealso cref="javax.management.MBeanServerBuilder"/>.
		''' </returns>
		''' <exception cref="SecurityException"> if there is a SecurityManager and
		''' the caller's permissions do not make it possible to instantiate
		''' a new builder. </exception>
		''' <exception cref="JMRuntimeException"> if the builder instantiation
		'''   fails with a checked exception -
		'''   <seealso cref="java.lang.ClassNotFoundException"/> etc...
		''' 
		'''  </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared newMBeanServerBuilder As MBeanServerBuilder
			Get
				checkMBeanServerBuilder()
				Return builder
			End Get
		End Property

	End Class

End Namespace