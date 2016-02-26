Imports System

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


	' java import

	' RI import

	''' <summary>
	''' <p>This is the interface for MBean manipulation on the agent
	''' side. It contains the methods necessary for the creation,
	''' registration, and deletion of MBeans as well as the access methods
	''' for registered MBeans.  This is the core component of the JMX
	''' infrastructure.</p>
	''' 
	''' <p>User code does not usually implement this interface.  Instead,
	''' an object that implements this interface is obtained with one of
	''' the methods in the <seealso cref="javax.management.MBeanServerFactory"/> class.</p>
	''' 
	''' <p>Every MBean which is added to the MBean server becomes
	''' manageable: its attributes and operations become remotely
	''' accessible through the connectors/adaptors connected to that MBean
	''' server.  A Java object cannot be registered in the MBean server
	''' unless it is a JMX compliant MBean.</p>
	''' 
	''' <p id="notif">When an MBean is registered or unregistered in the
	''' MBean server a {@link javax.management.MBeanServerNotification
	''' MBeanServerNotification} Notification is emitted. To register an
	''' object as listener to MBeanServerNotifications you should call the
	''' MBean server method {@link #addNotificationListener
	''' addNotificationListener} with <CODE>ObjectName</CODE> the
	''' <CODE>ObjectName</CODE> of the {@link
	''' javax.management.MBeanServerDelegate MBeanServerDelegate}.  This
	''' <CODE>ObjectName</CODE> is: <BR>
	''' <CODE>JMImplementation:type=MBeanServerDelegate</CODE>.</p>
	''' 
	''' <p>An object obtained from the {@link
	''' MBeanServerFactory#createMBeanServer(String) createMBeanServer} or
	''' <seealso cref="MBeanServerFactory#newMBeanServer(String) newMBeanServer"/>
	''' methods of the <seealso cref="MBeanServerFactory"/> class applies security
	''' checks to its methods, as follows.</p>
	''' 
	''' <p>First, if there is no security manager ({@link
	''' System#getSecurityManager()} is null), then an implementation of
	''' this interface is free not to make any checks.</p>
	''' 
	''' <p>Assuming that there is a security manager, or that the
	''' implementation chooses to make checks anyway, the checks are made
	''' as detailed below.  In what follows, and unless otherwise specified,
	''' {@code className} is the
	''' string returned by <seealso cref="MBeanInfo#getClassName()"/> for the target
	''' MBean.</p>
	''' 
	''' <p>If a security check fails, the method throws {@link
	''' SecurityException}.</p>
	''' 
	''' <p>For methods that can throw <seealso cref="InstanceNotFoundException"/>,
	''' this exception is thrown for a non-existent MBean, regardless of
	''' permissions.  This is because a non-existent MBean has no
	''' <code>className</code>.</p>
	''' 
	''' <ul>
	''' 
	''' <li><p>For the <seealso cref="#invoke invoke"/> method, the caller's
	''' permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, operationName, name, "invoke")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#getAttribute getAttribute"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, attribute, name, "getAttribute")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#getAttributes getAttributes"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "getAttribute")}.
	''' Additionally, for each attribute <em>a</em> in the {@link
	''' AttributeList}, if the caller's permissions do not imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, <em>a</em>, name, "getAttribute")}, the
	''' MBean server will behave as if that attribute had not been in the
	''' supplied list.</p>
	''' 
	''' <li><p>For the <seealso cref="#setAttribute setAttribute"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, attrName, name, "setAttribute")}, where
	''' <code>attrName</code> is {@link Attribute#getName()
	''' attribute.getName()}.</p>
	''' 
	''' <li><p>For the <seealso cref="#setAttributes setAttributes"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "setAttribute")}.
	''' Additionally, for each attribute <em>a</em> in the {@link
	''' AttributeList}, if the caller's permissions do not imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, <em>a</em>, name, "setAttribute")}, the
	''' MBean server will behave as if that attribute had not been in the
	''' supplied list.</p>
	''' 
	''' <li><p>For the <code>addNotificationListener</code> methods,
	''' the caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name,
	''' "addNotificationListener")}.</p>
	''' 
	''' <li><p>For the <code>removeNotificationListener</code> methods,
	''' the caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name,
	''' "removeNotificationListener")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#getMBeanInfo getMBeanInfo"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "getMBeanInfo")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#getObjectInstance getObjectInstance"/> method,
	''' the caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "getObjectInstance")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#isInstanceOf isInstanceOf"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "isInstanceOf")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#queryMBeans queryMBeans"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(null, null, null, "queryMBeans")}.
	''' Additionally, for each MBean <em>n</em> that matches <code>name</code>,
	''' if the caller's permissions do not imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, <em>n</em>, "queryMBeans")}, the
	''' MBean server will behave as if that MBean did not exist.</p>
	''' 
	''' <p>Certain query elements perform operations on the MBean server.
	''' If the caller does not have the required permissions for a given
	''' MBean, that MBean will not be included in the result of the query.
	''' The standard query elements that are affected are {@link
	''' Query#attr(String)}, <seealso cref="Query#attr(String,String)"/>, and {@link
	''' Query#classattr()}.</p>
	''' 
	''' <li><p>For the <seealso cref="#queryNames queryNames"/> method, the checks
	''' are the same as for <code>queryMBeans</code> except that
	''' <code>"queryNames"</code> is used instead of
	''' <code>"queryMBeans"</code> in the <code>MBeanPermission</code>
	''' objects.  Note that a <code>"queryMBeans"</code> permission implies
	''' the corresponding <code>"queryNames"</code> permission.</p>
	''' 
	''' <li><p>For the <seealso cref="#getDomains getDomains"/> method, the caller's
	''' permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(null, null, null, "getDomains")}.  Additionally,
	''' for each domain <var>d</var> in the returned array, if the caller's
	''' permissions do not imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(null, null, new ObjectName("<var>d</var>:x=x"),
	''' "getDomains")}, the domain is eliminated from the array.  Here,
	''' <code>x=x</code> is any <var>key=value</var> pair, needed to
	''' satisfy ObjectName's constructor but not otherwise relevant.</p>
	''' 
	''' <li><p>For the <seealso cref="#getClassLoader getClassLoader"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, loaderName,
	''' "getClassLoader")}.</p>
	''' 
	''' <li><p>For the <seealso cref="#getClassLoaderFor getClassLoaderFor"/> method,
	''' the caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, mbeanName,
	''' "getClassLoaderFor")}.</p>
	''' 
	''' <li><p>For the {@link #getClassLoaderRepository
	''' getClassLoaderRepository} method, the caller's permissions must
	''' imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(null, null, null, "getClassLoaderRepository")}.</p>
	''' 
	''' <li><p>For the deprecated <code>deserialize</code> methods, the
	''' required permissions are the same as for the methods that replace
	''' them.</p>
	''' 
	''' <li><p>For the <code>instantiate</code> methods, the caller's
	''' permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, null, "instantiate")},
	''' where {@code className} is the name of the class which is to
	''' be instantiated.</p>
	''' 
	''' <li><p>For the <seealso cref="#registerMBean registerMBean"/> method, the
	''' caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "registerMBean")}.
	''' 
	''' <p>If the <code>MBeanPermission</code> check succeeds, the MBean's
	''' class is validated by checking that its {@link
	''' java.security.ProtectionDomain ProtectionDomain} implies {@link
	''' MBeanTrustPermission#MBeanTrustPermission(String)
	''' MBeanTrustPermission("register")}.</p>
	''' 
	''' <p>Finally, if the <code>name</code> argument is null, another
	''' <code>MBeanPermission</code> check is made using the
	''' <code>ObjectName</code> returned by {@link
	''' MBeanRegistration#preRegister MBeanRegistration.preRegister}.</p>
	''' 
	''' <li><p>For the <code>createMBean</code> methods, the caller's
	''' permissions must imply the permissions needed by the equivalent
	''' <code>instantiate</code> followed by
	''' <code>registerMBean</code>.</p>
	''' 
	''' <li><p>For the <seealso cref="#unregisterMBean unregisterMBean"/> method,
	''' the caller's permissions must imply {@link
	''' MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	''' MBeanPermission(className, null, name, "unregisterMBean")}.</p>
	''' 
	''' </ul>
	''' 
	''' @since 1.5
	''' </summary>

	' DELETED:
	' *
	' * <li><p>For the {@link #isRegistered isRegistered} method, the
	' * caller's permissions must imply {@link
	' * MBeanPermission#MBeanPermission(String,String,ObjectName,String)
	' * MBeanPermission(null, null, name, "isRegistered")}.</p>
	' 
	Public Interface MBeanServer
		Inherits MBeanServerConnection

		''' <summary>
		''' {@inheritDoc}
		''' <p>If this method successfully creates an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeMBeanException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeErrorException"> {@inheritDoc} </exception>
		Function createMBean(ByVal className As String, ByVal name As ObjectName) As ObjectInstance

		''' <summary>
		''' {@inheritDoc}
		''' <p>If this method successfully creates an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeMBeanException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeErrorException"> {@inheritDoc} </exception>
		Function createMBean(ByVal className As String, ByVal name As ObjectName, ByVal loaderName As ObjectName) As ObjectInstance

		''' <summary>
		''' {@inheritDoc}
		''' <p>If this method successfully creates an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeMBeanException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeErrorException"> {@inheritDoc} </exception>
		Function createMBean(ByVal className As String, ByVal name As ObjectName, Object ByVal  As params(), ByVal signature As String()) As ObjectInstance

		''' <summary>
		''' {@inheritDoc}
		''' <p>If this method successfully creates an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeMBeanException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeErrorException"> {@inheritDoc} </exception>
		Function createMBean(ByVal className As String, ByVal name As ObjectName, ByVal loaderName As ObjectName, Object ByVal  As params(), ByVal signature As String()) As ObjectInstance

		''' <summary>
		''' <p>Registers a pre-existing object as an MBean with the MBean
		''' server. If the object name given is null, the MBean must
		''' provide its own name by implementing the {@link
		''' javax.management.MBeanRegistration MBeanRegistration} interface
		''' and returning the name from the {@link
		''' MBeanRegistration#preRegister preRegister} method.
		''' 
		''' <p>If this method successfully registers an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <param name="object"> The  MBean to be registered as an MBean. </param>
		''' <param name="name"> The object name of the MBean. May be null.
		''' </param>
		''' <returns> An <CODE>ObjectInstance</CODE>, containing the
		''' <CODE>ObjectName</CODE> and the Java class name of the newly
		''' registered MBean.  If the contained <code>ObjectName</code>
		''' is <code>n</code>, the contained Java class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(n)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="InstanceAlreadyExistsException"> The MBean is already
		''' under the control of the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The
		''' <CODE>preRegister</CODE> (<CODE>MBeanRegistration</CODE>
		''' interface) method of the MBean has thrown an exception. The
		''' MBean will not be registered. </exception>
		''' <exception cref="RuntimeMBeanException"> If the <CODE>postRegister</CODE>
		''' (<CODE>MBeanRegistration</CODE> interface) method of the MBean throws a
		''' <CODE>RuntimeException</CODE>, the <CODE>registerMBean</CODE> method will
		''' throw a <CODE>RuntimeMBeanException</CODE>, although the MBean
		''' registration succeeded. In such a case, the MBean will be actually
		''' registered even though the <CODE>registerMBean</CODE> method
		''' threw an exception.  Note that <CODE>RuntimeMBeanException</CODE> can
		''' also be thrown by <CODE>preRegister</CODE>, in which case the MBean
		''' will not be registered. </exception>
		''' <exception cref="RuntimeErrorException"> If the <CODE>postRegister</CODE>
		''' (<CODE>MBeanRegistration</CODE> interface) method of the MBean throws an
		''' <CODE>Error</CODE>, the <CODE>registerMBean</CODE> method will
		''' throw a <CODE>RuntimeErrorException</CODE>, although the MBean
		''' registration succeeded. In such a case, the MBean will be actually
		''' registered even though the <CODE>registerMBean</CODE> method
		''' threw an exception.  Note that <CODE>RuntimeErrorException</CODE> can
		''' also be thrown by <CODE>preRegister</CODE>, in which case the MBean
		''' will not be registered. </exception>
		''' <exception cref="NotCompliantMBeanException"> This object is not a JMX
		''' compliant MBean </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <CODE>java.lang.IllegalArgumentException</CODE>: The object
		''' passed in parameter is null or no object name is specified. </exception>
		''' <seealso cref= javax.management.MBeanRegistration </seealso>
		Function registerMBean(ByVal [object] As Object, ByVal name As ObjectName) As ObjectInstance

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>If this method successfully unregisters an MBean, a notification
		''' is sent as described <a href="#notif">above</a>.</p>
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeMBeanException"> {@inheritDoc} </exception>
		''' <exception cref="RuntimeErrorException"> {@inheritDoc} </exception>
		Sub unregisterMBean(ByVal name As ObjectName)

		' doc comment inherited from MBeanServerConnection
		Function getObjectInstance(ByVal name As ObjectName) As ObjectInstance

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function queryMBeans(ByVal name As ObjectName, ByVal query As QueryExp) As java.util.Set(Of ObjectInstance)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function queryNames(ByVal name As ObjectName, ByVal query As QueryExp) As java.util.Set(Of ObjectName)

		' doc comment inherited from MBeanServerConnection
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function isRegistered(ByVal name As ObjectName) As Boolean

		''' <summary>
		''' Returns the number of MBeans registered in the MBean server.
		''' </summary>
		''' <returns> the number of registered MBeans, wrapped in an Integer.
		''' If the caller's permissions are restricted, this number may
		''' be greater than the number of MBeans the caller can access. </returns>
		ReadOnly Property mBeanCount As Integer?

		' doc comment inherited from MBeanServerConnection
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function getAttribute(ByVal name As ObjectName, ByVal attribute As String) As Object

		' doc comment inherited from MBeanServerConnection
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function getAttributes(ByVal name As ObjectName, ByVal attributes As String()) As AttributeList

		' doc comment inherited from MBeanServerConnection
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Sub setAttribute(ByVal name As ObjectName, ByVal attribute As Attribute)

		' doc comment inherited from MBeanServerConnection
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Function setAttributes(ByVal name As ObjectName, ByVal attributes As AttributeList) As AttributeList

		' doc comment inherited from MBeanServerConnection
		Function invoke(ByVal name As ObjectName, ByVal operationName As String, Object ByVal  As params(), ByVal signature As String()) As Object

		' doc comment inherited from MBeanServerConnection
		ReadOnly Property defaultDomain As String

		' doc comment inherited from MBeanServerConnection
		ReadOnly Property domains As String()

		' doc comment inherited from MBeanServerConnection, plus:
		''' <summary>
		''' {@inheritDoc}
		''' If the source of the notification
		''' is a reference to an MBean object, the MBean server will replace it
		''' by that MBean's ObjectName.  Otherwise the source is unchanged.
		''' </summary>
		Sub addNotificationListener(ByVal name As ObjectName, ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object)

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <exception cref="RuntimeOperationsException"> {@inheritDoc} </exception>
		Sub addNotificationListener(ByVal name As ObjectName, ByVal listener As ObjectName, ByVal filter As NotificationFilter, ByVal handback As Object)

		' doc comment inherited from MBeanServerConnection
		Sub removeNotificationListener(ByVal name As ObjectName, ByVal listener As ObjectName)

		' doc comment inherited from MBeanServerConnection
		Sub removeNotificationListener(ByVal name As ObjectName, ByVal listener As ObjectName, ByVal filter As NotificationFilter, ByVal handback As Object)

		' doc comment inherited from MBeanServerConnection
		Sub removeNotificationListener(ByVal name As ObjectName, ByVal listener As NotificationListener)

		' doc comment inherited from MBeanServerConnection
		Sub removeNotificationListener(ByVal name As ObjectName, ByVal listener As NotificationListener, ByVal filter As NotificationFilter, ByVal handback As Object)

		' doc comment inherited from MBeanServerConnection
		Function getMBeanInfo(ByVal name As ObjectName) As MBeanInfo


		' doc comment inherited from MBeanServerConnection
		Function isInstanceOf(ByVal name As ObjectName, ByVal className As String) As Boolean

		''' <summary>
		''' <p>Instantiates an object using the list of all class loaders
		''' registered in the MBean server's {@link
		''' javax.management.loading.ClassLoaderRepository Class Loader
		''' Repository}.  The object's class should have a public
		''' constructor.  This method returns a reference to the newly
		''' created object.  The newly created object is not registered in
		''' the MBean server.</p>
		''' 
		''' <p>This method is equivalent to {@link
		''' #instantiate(String,Object[],String[])
		''' instantiate(className, (Object[]) null, (String[]) null)}.</p>
		''' </summary>
		''' <param name="className"> The class name of the object to be instantiated.
		''' </param>
		''' <returns> The newly instantiated object.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <CODE>java.lang.ClassNotFoundException</CODE> or the
		''' <CODE>java.lang.Exception</CODE> that occurred when trying to
		''' invoke the object's constructor. </exception>
		''' <exception cref="MBeanException"> The constructor of the object has
		''' thrown an exception </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <CODE>java.lang.IllegalArgumentException</CODE>: The className
		''' passed in parameter is null. </exception>
		Function instantiate(ByVal className As String) As Object


		''' <summary>
		''' <p>Instantiates an object using the class Loader specified by its
		''' <CODE>ObjectName</CODE>.  If the loader name is null, the
		''' ClassLoader that loaded the MBean Server will be used.  The
		''' object's class should have a public constructor.  This method
		''' returns a reference to the newly created object.  The newly
		''' created object is not registered in the MBean server.</p>
		''' 
		''' <p>This method is equivalent to {@link
		''' #instantiate(String,ObjectName,Object[],String[])
		''' instantiate(className, loaderName, (Object[]) null, (String[])
		''' null)}.</p>
		''' </summary>
		''' <param name="className"> The class name of the MBean to be instantiated. </param>
		''' <param name="loaderName"> The object name of the class loader to be used.
		''' </param>
		''' <returns> The newly instantiated object.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <CODE>java.lang.ClassNotFoundException</CODE> or the
		''' <CODE>java.lang.Exception</CODE> that occurred when trying to
		''' invoke the object's constructor. </exception>
		''' <exception cref="MBeanException"> The constructor of the object has
		''' thrown an exception. </exception>
		''' <exception cref="InstanceNotFoundException"> The specified class loader
		''' is not registered in the MBeanServer. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <CODE>java.lang.IllegalArgumentException</CODE>: The className
		''' passed in parameter is null. </exception>
		Function instantiate(ByVal className As String, ByVal loaderName As ObjectName) As Object

		''' <summary>
		''' <p>Instantiates an object using the list of all class loaders
		''' registered in the MBean server {@link
		''' javax.management.loading.ClassLoaderRepository Class Loader
		''' Repository}.  The object's class should have a public
		''' constructor.  The call returns a reference to the newly created
		''' object.  The newly created object is not registered in the
		''' MBean server.</p>
		''' </summary>
		''' <param name="className"> The class name of the object to be instantiated. </param>
		''' <param name="params"> An array containing the parameters of the
		''' constructor to be invoked. </param>
		''' <param name="signature"> An array containing the signature of the
		''' constructor to be invoked.
		''' </param>
		''' <returns> The newly instantiated object.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <CODE>java.lang.ClassNotFoundException</CODE> or the
		''' <CODE>java.lang.Exception</CODE> that occurred when trying to
		''' invoke the object's constructor. </exception>
		''' <exception cref="MBeanException"> The constructor of the object has
		''' thrown an exception </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <CODE>java.lang.IllegalArgumentException</CODE>: The className
		''' passed in parameter is null. </exception>
		Function instantiate(ByVal className As String, Object ByVal  As params(), ByVal signature As String()) As Object

		''' <summary>
		''' <p>Instantiates an object. The class loader to be used is
		''' identified by its object name. If the object name of the loader
		''' is null, the ClassLoader that loaded the MBean server will be
		''' used.  The object's class should have a public constructor.
		''' The call returns a reference to the newly created object.  The
		''' newly created object is not registered in the MBean server.</p>
		''' </summary>
		''' <param name="className"> The class name of the object to be instantiated. </param>
		''' <param name="params"> An array containing the parameters of the
		''' constructor to be invoked. </param>
		''' <param name="signature"> An array containing the signature of the
		''' constructor to be invoked. </param>
		''' <param name="loaderName"> The object name of the class loader to be used.
		''' </param>
		''' <returns> The newly instantiated object.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a <CODE>java.lang.ClassNotFoundException</CODE> or the <CODE>java.lang.Exception</CODE> that
		''' occurred when trying to invoke the object's constructor. </exception>
		''' <exception cref="MBeanException"> The constructor of the object has
		''' thrown an exception </exception>
		''' <exception cref="InstanceNotFoundException"> The specified class loader
		''' is not registered in the MBean server. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <CODE>java.lang.IllegalArgumentException</CODE>: The className
		''' passed in parameter is null. </exception>
		Function instantiate(ByVal className As String, ByVal loaderName As ObjectName, Object ByVal  As params(), ByVal signature As String()) As Object

		''' <summary>
		''' <p>De-serializes a byte array in the context of the class loader
		''' of an MBean.</p>
		''' </summary>
		''' <param name="name"> The name of the MBean whose class loader should be
		''' used for the de-serialization. </param>
		''' <param name="data"> The byte array to be de-sererialized.
		''' </param>
		''' <returns> The de-serialized object stream.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' found. </exception>
		''' <exception cref="OperationsException"> Any of the usual Input/Output
		''' related exceptions.
		''' </exception>
		''' @deprecated Use <seealso cref="#getClassLoaderFor getClassLoaderFor"/> to
		''' obtain the appropriate class loader for deserialization. 
		<Obsolete("Use <seealso cref="#getClassLoaderFor getClassLoaderFor"/> to")> _
		Function deserialize(ByVal name As ObjectName, ByVal data As SByte()) As java.io.ObjectInputStream


		''' <summary>
		''' <p>De-serializes a byte array in the context of a given MBean
		''' class loader.  The class loader is found by loading the class
		''' <code>className</code> through the {@link
		''' javax.management.loading.ClassLoaderRepository Class Loader
		''' Repository}.  The resultant class's class loader is the one to
		''' use.
		''' </summary>
		''' <param name="className"> The name of the class whose class loader should be
		''' used for the de-serialization. </param>
		''' <param name="data"> The byte array to be de-sererialized.
		''' </param>
		''' <returns>  The de-serialized object stream.
		''' </returns>
		''' <exception cref="OperationsException"> Any of the usual Input/Output
		''' related exceptions. </exception>
		''' <exception cref="ReflectionException"> The specified class could not be
		''' loaded by the class loader repository
		''' </exception>
		''' @deprecated Use <seealso cref="#getClassLoaderRepository"/> to obtain the
		''' class loader repository and use it to deserialize. 
		<Obsolete("Use <seealso cref="#getClassLoaderRepository"/> to obtain the")> _
		Function deserialize(ByVal className As String, ByVal data As SByte()) As java.io.ObjectInputStream


		''' <summary>
		''' <p>De-serializes a byte array in the context of a given MBean
		''' class loader.  The class loader is the one that loaded the
		''' class with name "className".  The name of the class loader to
		''' be used for loading the specified class is specified.  If null,
		''' the MBean Server's class loader will be used.</p>
		''' </summary>
		''' <param name="className"> The name of the class whose class loader should be
		''' used for the de-serialization. </param>
		''' <param name="data"> The byte array to be de-sererialized. </param>
		''' <param name="loaderName"> The name of the class loader to be used for
		''' loading the specified class.  If null, the MBean Server's class
		''' loader will be used.
		''' </param>
		''' <returns>  The de-serialized object stream.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The specified class loader
		''' MBean is not found. </exception>
		''' <exception cref="OperationsException"> Any of the usual Input/Output
		''' related exceptions. </exception>
		''' <exception cref="ReflectionException"> The specified class could not be
		''' loaded by the specified class loader.
		''' </exception>
		''' @deprecated Use <seealso cref="#getClassLoader getClassLoader"/> to obtain
		''' the class loader for deserialization. 
		<Obsolete("Use <seealso cref="#getClassLoader getClassLoader"/> to obtain")> _
		Function deserialize(ByVal className As String, ByVal loaderName As ObjectName, ByVal data As SByte()) As java.io.ObjectInputStream

		''' <summary>
		''' <p>Return the <seealso cref="java.lang.ClassLoader"/> that was used for
		''' loading the class of the named MBean.</p>
		''' </summary>
		''' <param name="mbeanName"> The ObjectName of the MBean.
		''' </param>
		''' <returns> The ClassLoader used for that MBean.  If <var>l</var>
		''' is the MBean's actual ClassLoader, and <var>r</var> is the
		''' returned value, then either:
		''' 
		''' <ul>
		''' <li><var>r</var> is identical to <var>l</var>; or
		''' <li>the result of <var>r</var>{@link
		''' ClassLoader#loadClass(String) .loadClass(<var>s</var>)} is the
		''' same as <var>l</var>{@link ClassLoader#loadClass(String)
		''' .loadClass(<var>s</var>)} for any string <var>s</var>.
		''' </ul>
		''' 
		''' What this means is that the ClassLoader may be wrapped in
		''' another ClassLoader for security or other reasons.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> if the named MBean is not found.
		'''  </exception>
		Function getClassLoaderFor(ByVal mbeanName As ObjectName) As ClassLoader

		''' <summary>
		''' <p>Return the named <seealso cref="java.lang.ClassLoader"/>.</p>
		''' </summary>
		''' <param name="loaderName"> The ObjectName of the ClassLoader.  May be
		''' null, in which case the MBean server's own ClassLoader is
		''' returned.
		''' </param>
		''' <returns> The named ClassLoader.  If <var>l</var> is the actual
		''' ClassLoader with that name, and <var>r</var> is the returned
		''' value, then either:
		''' 
		''' <ul>
		''' <li><var>r</var> is identical to <var>l</var>; or
		''' <li>the result of <var>r</var>{@link
		''' ClassLoader#loadClass(String) .loadClass(<var>s</var>)} is the
		''' same as <var>l</var>{@link ClassLoader#loadClass(String)
		''' .loadClass(<var>s</var>)} for any string <var>s</var>.
		''' </ul>
		''' 
		''' What this means is that the ClassLoader may be wrapped in
		''' another ClassLoader for security or other reasons.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> if the named ClassLoader is
		'''    not found.
		'''  </exception>
		Function getClassLoader(ByVal loaderName As ObjectName) As ClassLoader

		''' <summary>
		''' <p>Return the ClassLoaderRepository for this MBeanServer. </summary>
		''' <returns> The ClassLoaderRepository for this MBeanServer.
		'''  </returns>
		ReadOnly Property classLoaderRepository As javax.management.loading.ClassLoaderRepository
	End Interface

End Namespace