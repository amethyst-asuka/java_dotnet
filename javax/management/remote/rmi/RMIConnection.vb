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

Namespace javax.management.remote.rmi




	''' <summary>
	''' <p>RMI object used to forward an MBeanServer request from a client
	''' to its MBeanServer implementation on the server side.  There is one
	''' Remote object implementing this interface for each remote client
	''' connected to an RMI connector.</p>
	''' 
	''' <p>User code does not usually refer to this interface.  It is
	''' specified as part of the public API so that different
	''' implementations of that API will interoperate.</p>
	''' 
	''' <p>To ensure that client parameters will be deserialized at the
	''' server side with the correct classloader, client parameters such as
	''' parameters used to invoke a method are wrapped in a {@link
	''' MarshalledObject}.  An implementation of this interface must first
	''' get the appropriate class loader for the operation and its target,
	''' then deserialize the marshalled parameters with this classloader.
	''' Except as noted, a parameter that is a
	''' <code>MarshalledObject</code> or <code>MarshalledObject[]</code>
	''' must not be null; the behavior is unspecified if it is.</p>
	''' 
	''' <p>Class loading aspects are detailed in the
	''' <a href="{@docRoot}/../technotes/guides/jmx/JMX_1_4_specification.pdf">
	''' JMX Specification, version 1.4</a> PDF document.</p>
	''' 
	''' <p>Most methods in this interface parallel methods in the {@link
	''' MBeanServerConnection} interface.  Where an aspect of the behavior
	''' of a method is not specified here, it is the same as in the
	''' corresponding <code>MBeanServerConnection</code> method.
	''' 
	''' @since 1.5
	''' </summary>
	'
	' * Notice that we omit the type parameter from MarshalledObject everywhere,
	' * even though it would add useful information to the documentation.  The
	' * reason is that it was only added in Mustang (Java SE 6), whereas versions
	' * 1.4 and 2.0 of the JMX API must be implementable on Tiger per our
	' * commitments for JSR 255.  This is also why we suppress rawtypes warnings.
	' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface RMIConnection
		Inherits java.io.Closeable, java.rmi.Remote

		''' <summary>
		''' <p>Returns the connection ID.  This string is different for
		''' every open connection to a given RMI connector server.</p>
		''' </summary>
		''' <returns> the connection ID
		''' </returns>
		''' <seealso cref= RMIConnector#connect RMIConnector.connect
		''' </seealso>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		ReadOnly Property connectionId As String

		''' <summary>
		''' <p>Closes this connection.  On return from this method, the RMI
		''' object implementing this interface is unexported, so further
		''' remote calls to it will fail.</p>
		''' </summary>
		''' <exception cref="IOException"> if the connection could not be closed,
		''' or the Remote object could not be unexported, or there was a
		''' communication failure when transmitting the remote close
		''' request. </exception>
		Sub close()

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#createMBean(String,
		''' ObjectName)}.
		''' </summary>
		''' <param name="className"> The class name of the MBean to be instantiated. </param>
		''' <param name="name"> The object name of the MBean. May be null. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> An <code>ObjectInstance</code>, containing the
		''' <code>ObjectName</code> and the Java class name of the newly
		''' instantiated MBean.  If the contained <code>ObjectName</code>
		''' is <code>n</code>, the contained Java class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(n)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.ClassNotFoundException</code> or a
		''' <code>java.lang.Exception</code> that occurred
		''' when trying to invoke the MBean's constructor. </exception>
		''' <exception cref="InstanceAlreadyExistsException"> The MBean is already
		''' under the control of the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The
		''' <code>preRegister</code> (<code>MBeanRegistration</code>
		''' interface) method of the MBean has thrown an exception. The
		''' MBean will not be registered. </exception>
		''' <exception cref="MBeanException"> The constructor of the MBean has
		''' thrown an exception. </exception>
		''' <exception cref="NotCompliantMBeanException"> This class is not a JMX
		''' compliant MBean. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The className
		''' passed in parameter is null, the <code>ObjectName</code> passed
		''' in parameter contains a pattern or no <code>ObjectName</code>
		''' is specified for the MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As javax.management.ObjectInstance

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#createMBean(String,
		''' ObjectName, ObjectName)}.
		''' </summary>
		''' <param name="className"> The class name of the MBean to be instantiated. </param>
		''' <param name="name"> The object name of the MBean. May be null. </param>
		''' <param name="loaderName"> The object name of the class loader to be used. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> An <code>ObjectInstance</code>, containing the
		''' <code>ObjectName</code> and the Java class name of the newly
		''' instantiated MBean.  If the contained <code>ObjectName</code>
		''' is <code>n</code>, the contained Java class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(n)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.ClassNotFoundException</code> or a
		''' <code>java.lang.Exception</code> that occurred when trying to
		''' invoke the MBean's constructor. </exception>
		''' <exception cref="InstanceAlreadyExistsException"> The MBean is already
		''' under the control of the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The
		''' <code>preRegister</code> (<code>MBeanRegistration</code>
		''' interface) method of the MBean has thrown an exception. The
		''' MBean will not be registered. </exception>
		''' <exception cref="MBeanException"> The constructor of the MBean has
		''' thrown an exception. </exception>
		''' <exception cref="NotCompliantMBeanException"> This class is not a JMX
		''' compliant MBean. </exception>
		''' <exception cref="InstanceNotFoundException"> The specified class loader
		''' is not registered in the MBean server. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The className
		''' passed in parameter is null, the <code>ObjectName</code> passed
		''' in parameter contains a pattern or no <code>ObjectName</code>
		''' is specified for the MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName, ByVal loaderName As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As javax.management.ObjectInstance

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#createMBean(String,
		''' ObjectName, Object[], String[])}.  The <code>Object[]</code>
		''' parameter is wrapped in a <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="className"> The class name of the MBean to be instantiated. </param>
		''' <param name="name"> The object name of the MBean. May be null. </param>
		''' <param name="params"> An array containing the parameters of the
		''' constructor to be invoked, encapsulated into a
		''' <code>MarshalledObject</code>.  The encapsulated array can be
		''' null, equivalent to an empty array. </param>
		''' <param name="signature"> An array containing the signature of the
		''' constructor to be invoked.  Can be null, equivalent to an empty
		''' array. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> An <code>ObjectInstance</code>, containing the
		''' <code>ObjectName</code> and the Java class name of the newly
		''' instantiated MBean.  If the contained <code>ObjectName</code>
		''' is <code>n</code>, the contained Java class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(n)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.ClassNotFoundException</code> or a
		''' <code>java.lang.Exception</code> that occurred when trying to
		''' invoke the MBean's constructor. </exception>
		''' <exception cref="InstanceAlreadyExistsException"> The MBean is already
		''' under the control of the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The
		''' <code>preRegister</code> (<code>MBeanRegistration</code>
		''' interface) method of the MBean has thrown an exception. The
		''' MBean will not be registered. </exception>
		''' <exception cref="MBeanException"> The constructor of the MBean has
		''' thrown an exception. </exception>
		''' <exception cref="NotCompliantMBeanException"> This class is not a JMX
		''' compliant MBean. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The className
		''' passed in parameter is null, the <code>ObjectName</code> passed
		''' in parameter contains a pattern, or no <code>ObjectName</code>
		''' is specified for the MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName, ByVal params As java.rmi.MarshalledObject, String ByVal  As signature(), ByVal delegationSubject As javax.security.auth.Subject) As javax.management.ObjectInstance

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#createMBean(String,
		''' ObjectName, ObjectName, Object[], String[])}.  The
		''' <code>Object[]</code> parameter is wrapped in a
		''' <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="className"> The class name of the MBean to be instantiated. </param>
		''' <param name="name"> The object name of the MBean. May be null. </param>
		''' <param name="loaderName"> The object name of the class loader to be used. </param>
		''' <param name="params"> An array containing the parameters of the
		''' constructor to be invoked, encapsulated into a
		''' <code>MarshalledObject</code>.  The encapsulated array can be
		''' null, equivalent to an empty array. </param>
		''' <param name="signature"> An array containing the signature of the
		''' constructor to be invoked.  Can be null, equivalent to an empty
		''' array. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> An <code>ObjectInstance</code>, containing the
		''' <code>ObjectName</code> and the Java class name of the newly
		''' instantiated MBean.  If the contained <code>ObjectName</code>
		''' is <code>n</code>, the contained Java class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(n)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.ClassNotFoundException</code> or a
		''' <code>java.lang.Exception</code> that occurred when trying to
		''' invoke the MBean's constructor. </exception>
		''' <exception cref="InstanceAlreadyExistsException"> The MBean is already
		''' under the control of the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The
		''' <code>preRegister</code> (<code>MBeanRegistration</code>
		''' interface) method of the MBean has thrown an exception. The
		''' MBean will not be registered. </exception>
		''' <exception cref="MBeanException"> The constructor of the MBean has
		''' thrown an exception. </exception>
		''' <exception cref="NotCompliantMBeanException"> This class is not a JMX
		''' compliant MBean. </exception>
		''' <exception cref="InstanceNotFoundException"> The specified class loader
		''' is not registered in the MBean server. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The className
		''' passed in parameter is null, the <code>ObjectName</code> passed
		''' in parameter contains a pattern, or no <code>ObjectName</code>
		''' is specified for the MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function createMBean(ByVal className As String, ByVal name As javax.management.ObjectName, ByVal loaderName As javax.management.ObjectName, ByVal params As java.rmi.MarshalledObject, String ByVal  As signature(), ByVal delegationSubject As javax.security.auth.Subject) As javax.management.ObjectInstance

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#unregisterMBean(ObjectName)"/>.
		''' </summary>
		''' <param name="name"> The object name of the MBean to be unregistered. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="MBeanRegistrationException"> The preDeregister
		''' ((<code>MBeanRegistration</code> interface) method of the MBean
		''' has thrown an exception. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null or the MBean you are when trying to
		''' unregister is the {@link javax.management.MBeanServerDelegate
		''' MBeanServerDelegate} MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Sub unregisterMBean(ByVal name As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject)

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#getObjectInstance(ObjectName)"/>.
		''' </summary>
		''' <param name="name"> The object name of the MBean. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> The <code>ObjectInstance</code> associated with the MBean
		''' specified by <var>name</var>.  The contained <code>ObjectName</code>
		''' is <code>name</code> and the contained class name is
		''' <code><seealso cref="#getMBeanInfo getMBeanInfo(name)"/>.getClassName()</code>.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function getObjectInstance(ByVal name As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As javax.management.ObjectInstance

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#queryMBeans(ObjectName,
		''' QueryExp)}.  The <code>QueryExp</code> is wrapped in a
		''' <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The object name pattern identifying the MBeans to
		''' be retrieved. If null or no domain and key properties are
		''' specified, all the MBeans registered will be retrieved. </param>
		''' <param name="query"> The query expression to be applied for selecting
		''' MBeans, encapsulated into a <code>MarshalledObject</code>. If
		''' the <code>MarshalledObject</code> encapsulates a null value no
		''' query expression will be applied for selecting MBeans. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> A set containing the <code>ObjectInstance</code>
		''' objects for the selected MBeans.  If no MBean satisfies the
		''' query an empty list is returned.
		''' </returns>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function queryMBeans(ByVal name As javax.management.ObjectName, ByVal query As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject) As java.util.Set(Of javax.management.ObjectInstance)

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#queryNames(ObjectName,
		''' QueryExp)}.  The <code>QueryExp</code> is wrapped in a
		''' <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The object name pattern identifying the MBean names
		''' to be retrieved. If null or no domain and key properties are
		''' specified, the name of all registered MBeans will be retrieved. </param>
		''' <param name="query"> The query expression to be applied for selecting
		''' MBeans, encapsulated into a <code>MarshalledObject</code>. If
		''' the <code>MarshalledObject</code> encapsulates a null value no
		''' query expression will be applied for selecting MBeans. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> A set containing the ObjectNames for the MBeans
		''' selected.  If no MBean satisfies the query, an empty list is
		''' returned.
		''' </returns>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function queryNames(ByVal name As javax.management.ObjectName, ByVal query As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject) As java.util.Set(Of javax.management.ObjectName)

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#isRegistered(ObjectName)"/>.
		''' </summary>
		''' <param name="name"> The object name of the MBean to be checked. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> True if the MBean is already registered in the MBean
		''' server, false otherwise.
		''' </returns>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function isRegistered(ByVal name As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As Boolean

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#getMBeanCount()"/>.
		''' </summary>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> the number of MBeans registered.
		''' </returns>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function getMBeanCount(ByVal delegationSubject As javax.security.auth.Subject) As Integer?

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#getAttribute(ObjectName,
		''' String)}.
		''' </summary>
		''' <param name="name"> The object name of the MBean from which the
		''' attribute is to be retrieved. </param>
		''' <param name="attribute"> A String specifying the name of the attribute
		''' to be retrieved. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns>  The value of the retrieved attribute.
		''' </returns>
		''' <exception cref="AttributeNotFoundException"> The attribute specified
		''' is not accessible in the MBean. </exception>
		''' <exception cref="MBeanException"> Wraps an exception thrown by the
		''' MBean's getter. </exception>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.Exception</code> thrown when trying to invoke
		''' the getter. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null or the attribute in parameter is
		''' null. </exception>
		''' <exception cref="RuntimeMBeanException"> Wraps a runtime exception thrown
		''' by the MBean's getter. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred.
		''' </exception>
		''' <seealso cref= #setAttribute </seealso>
		Function getAttribute(ByVal name As javax.management.ObjectName, ByVal attribute As String, ByVal delegationSubject As javax.security.auth.Subject) As Object

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#getAttributes(ObjectName,
		''' String[])}.
		''' </summary>
		''' <param name="name"> The object name of the MBean from which the
		''' attributes are retrieved. </param>
		''' <param name="attributes"> A list of the attributes to be retrieved. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> The list of the retrieved attributes.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="ReflectionException"> An exception occurred when
		''' trying to invoke the getAttributes method of a Dynamic MBean. </exception>
		''' <exception cref="RuntimeOperationsException"> Wrap a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null or attributes in parameter is null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred.
		''' </exception>
		''' <seealso cref= #setAttributes </seealso>
		Function getAttributes(ByVal name As javax.management.ObjectName, ByVal attributes As String(), ByVal delegationSubject As javax.security.auth.Subject) As javax.management.AttributeList

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#setAttribute(ObjectName,
		''' Attribute)}.  The <code>Attribute</code> parameter is wrapped
		''' in a <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The name of the MBean within which the attribute is
		''' to be set. </param>
		''' <param name="attribute"> The identification of the attribute to be set
		''' and the value it is to be set to, encapsulated into a
		''' <code>MarshalledObject</code>. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="AttributeNotFoundException"> The attribute specified
		''' is not accessible in the MBean. </exception>
		''' <exception cref="InvalidAttributeValueException"> The value specified
		''' for the attribute is not valid. </exception>
		''' <exception cref="MBeanException"> Wraps an exception thrown by the
		''' MBean's setter. </exception>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.Exception</code> thrown when trying to invoke
		''' the setter. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null or the attribute in parameter is
		''' null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred.
		''' </exception>
		''' <seealso cref= #getAttribute </seealso>
		Sub setAttribute(ByVal name As javax.management.ObjectName, ByVal attribute As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject)

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#setAttributes(ObjectName,
		''' AttributeList)}.  The <code>AttributeList</code> parameter is
		''' wrapped in a <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The object name of the MBean within which the
		''' attributes are to be set. </param>
		''' <param name="attributes"> A list of attributes: The identification of
		''' the attributes to be set and the values they are to be set to,
		''' encapsulated into a <code>MarshalledObject</code>. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> The list of attributes that were set, with their new
		''' values.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="ReflectionException"> An exception occurred when
		''' trying to invoke the getAttributes method of a Dynamic MBean. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null or attributes in parameter is null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred.
		''' </exception>
		''' <seealso cref= #getAttributes </seealso>
		Function setAttributes(ByVal name As javax.management.ObjectName, ByVal attributes As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject) As javax.management.AttributeList

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#invoke(ObjectName,
		''' String, Object[], String[])}.  The <code>Object[]</code>
		''' parameter is wrapped in a <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The object name of the MBean on which the method is
		''' to be invoked. </param>
		''' <param name="operationName"> The name of the operation to be invoked. </param>
		''' <param name="params"> An array containing the parameters to be set when
		''' the operation is invoked, encapsulated into a
		''' <code>MarshalledObject</code>.  The encapsulated array can be
		''' null, equivalent to an empty array. </param>
		''' <param name="signature"> An array containing the signature of the
		''' operation. The class objects will be loaded using the same
		''' class loader as the one used for loading the MBean on which the
		''' operation was invoked.  Can be null, equivalent to an empty
		''' array. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> The object returned by the operation, which represents
		''' the result of invoking the operation on the MBean specified.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="MBeanException"> Wraps an exception thrown by the
		''' MBean's invoked method. </exception>
		''' <exception cref="ReflectionException"> Wraps a
		''' <code>java.lang.Exception</code> thrown while trying to invoke
		''' the method. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an {@link
		''' IllegalArgumentException} when <code>name</code> or
		''' <code>operationName</code> is null. </exception>
		Function invoke(ByVal name As javax.management.ObjectName, ByVal operationName As String, ByVal params As java.rmi.MarshalledObject, String ByVal  As signature(), ByVal delegationSubject As javax.security.auth.Subject) As Object

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#getDefaultDomain()"/>.
		''' </summary>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> the default domain.
		''' </returns>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function getDefaultDomain(ByVal delegationSubject As javax.security.auth.Subject) As String

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#getDomains()"/>.
		''' </summary>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> the list of domains.
		''' </returns>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function getDomains(ByVal delegationSubject As javax.security.auth.Subject) As String()

		''' <summary>
		''' Handles the method
		''' <seealso cref="javax.management.MBeanServerConnection#getMBeanInfo(ObjectName)"/>.
		''' </summary>
		''' <param name="name"> The name of the MBean to analyze </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> An instance of <code>MBeanInfo</code> allowing the
		''' retrieval of all attributes and operations of this MBean.
		''' </returns>
		''' <exception cref="IntrospectionException"> An exception occurred during
		''' introspection. </exception>
		''' <exception cref="InstanceNotFoundException"> The MBean specified was
		''' not found. </exception>
		''' <exception cref="ReflectionException"> An exception occurred when
		''' trying to invoke the getMBeanInfo of a Dynamic MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null. </exception>
		Function getMBeanInfo(ByVal name As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject) As javax.management.MBeanInfo

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#isInstanceOf(ObjectName,
		''' String)}.
		''' </summary>
		''' <param name="name"> The <code>ObjectName</code> of the MBean. </param>
		''' <param name="className"> The name of the class. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <returns> true if the MBean specified is an instance of the
		''' specified class according to the rules above, false otherwise.
		''' </returns>
		''' <exception cref="InstanceNotFoundException"> The MBean specified is not
		''' registered in the MBean server. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps a
		''' <code>java.lang.IllegalArgumentException</code>: The object
		''' name in parameter is null. </exception>
		Function isInstanceOf(ByVal name As javax.management.ObjectName, ByVal className As String, ByVal delegationSubject As javax.security.auth.Subject) As Boolean

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#addNotificationListener(ObjectName,
		''' ObjectName, NotificationFilter, Object)}.  The
		''' <code>NotificationFilter</code> parameter is wrapped in a
		''' <code>MarshalledObject</code>.  The <code>Object</code>
		''' (handback) parameter is also wrapped in a
		''' <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the listener should
		''' be added. </param>
		''' <param name="listener"> The object name of the listener which will
		''' handle the notifications emitted by the registered MBean. </param>
		''' <param name="filter"> The filter object, encapsulated into a
		''' <code>MarshalledObject</code>. If filter encapsulated in the
		''' <code>MarshalledObject</code> has a null value, no filtering
		''' will be performed before handling notifications. </param>
		''' <param name="handback"> The context to be sent to the listener when a
		''' notification is emitted, encapsulated into a
		''' <code>MarshalledObject</code>. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The MBean name of the
		''' notification listener or of the notification broadcaster does
		''' not match any of the registered MBeans. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an {@link
		''' IllegalArgumentException}.  The MBean named by
		''' <code>listener</code> exists but does not implement the
		''' <seealso cref="javax.management.NotificationListener"/> interface,
		''' or <code>name</code> or <code>listener</code> is null. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred.
		''' </exception>
		''' <seealso cref= #removeNotificationListener(ObjectName, ObjectName, Subject) </seealso>
		''' <seealso cref= #removeNotificationListener(ObjectName, ObjectName,
		''' MarshalledObject, MarshalledObject, Subject) </seealso>
		Sub addNotificationListener(ByVal name As javax.management.ObjectName, ByVal listener As javax.management.ObjectName, ByVal filter As java.rmi.MarshalledObject, ByVal handback As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject)

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#removeNotificationListener(ObjectName,
		''' ObjectName)}.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the listener should
		''' be removed. </param>
		''' <param name="listener"> The object name of the listener to be removed. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The MBean name provided
		''' does not match any of the registered MBeans. </exception>
		''' <exception cref="ListenerNotFoundException"> The listener is not
		''' registered in the MBean. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an {@link
		''' IllegalArgumentException} when <code>name</code> or
		''' <code>listener</code> is null.
		''' </exception>
		''' <seealso cref= #addNotificationListener </seealso>
		Sub removeNotificationListener(ByVal name As javax.management.ObjectName, ByVal listener As javax.management.ObjectName, ByVal delegationSubject As javax.security.auth.Subject)

		''' <summary>
		''' Handles the method {@link
		''' javax.management.MBeanServerConnection#removeNotificationListener(ObjectName,
		''' ObjectName, NotificationFilter, Object)}.  The
		''' <code>NotificationFilter</code> parameter is wrapped in a
		''' <code>MarshalledObject</code>.  The <code>Object</code>
		''' parameter is also wrapped in a <code>MarshalledObject</code>.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the listener should
		''' be removed. </param>
		''' <param name="listener"> A listener that was previously added to this
		''' MBean. </param>
		''' <param name="filter"> The filter that was specified when the listener
		''' was added, encapsulated into a <code>MarshalledObject</code>. </param>
		''' <param name="handback"> The handback that was specified when the
		''' listener was added, encapsulated into a <code>MarshalledObject</code>. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> The MBean name provided
		''' does not match any of the registered MBeans. </exception>
		''' <exception cref="ListenerNotFoundException"> The listener is not
		''' registered in the MBean, or it is not registered with the given
		''' filter and handback. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to perform this operation. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an {@link
		''' IllegalArgumentException} when <code>name</code> or
		''' <code>listener</code> is null.
		''' </exception>
		''' <seealso cref= #addNotificationListener </seealso>
		Sub removeNotificationListener(ByVal name As javax.management.ObjectName, ByVal listener As javax.management.ObjectName, ByVal filter As java.rmi.MarshalledObject, ByVal handback As java.rmi.MarshalledObject, ByVal delegationSubject As javax.security.auth.Subject)

		' Special Handling of Notifications -------------------------------------

		''' <summary>
		''' <p>Handles the method {@link
		''' javax.management.MBeanServerConnection#addNotificationListener(ObjectName,
		''' NotificationListener, NotificationFilter, Object)}.</p>
		''' 
		''' <p>Register for notifications from the given MBeans that match
		''' the given filters.  The remote client can subsequently retrieve
		''' the notifications using the {@link #fetchNotifications
		''' fetchNotifications} method.</p>
		''' 
		''' <p>For each listener, the original
		''' <code>NotificationListener</code> and <code>handback</code> are
		''' kept on the client side; in order for the client to be able to
		''' identify them, the server generates and returns a unique
		''' <code>listenerID</code>.  This <code>listenerID</code> is
		''' forwarded with the <code>Notifications</code> to the remote
		''' client.</p>
		''' 
		''' <p>If any one of the given (name, filter) pairs cannot be
		''' registered, then the operation fails with an exception, and no
		''' names or filters are registered.</p>
		''' </summary>
		''' <param name="names"> the <code>ObjectNames</code> identifying the
		''' MBeans emitting the Notifications. </param>
		''' <param name="filters"> an array of marshalled representations of the
		''' <code>NotificationFilters</code>.  Elements of this array can
		''' be null. </param>
		''' <param name="delegationSubjects"> the <code>Subjects</code> on behalf
		''' of which the listeners are being added.  Elements of this array
		''' can be null.  Also, the <code>delegationSubjects</code>
		''' parameter itself can be null, which is equivalent to an array
		''' of null values with the same size as the <code>names</code> and
		''' <code>filters</code> arrays.
		''' </param>
		''' <returns> an array of <code>listenerIDs</code> identifying the
		''' local listeners.  This array has the same number of elements as
		''' the parameters.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>names</code> or
		''' <code>filters</code> is null, or if <code>names</code> contains
		''' a null element, or if the three arrays do not all have the same
		''' size. </exception>
		''' <exception cref="ClassCastException"> if one of the elements of
		''' <code>filters</code> unmarshalls as a non-null object that is
		''' not a <code>NotificationFilter</code>. </exception>
		''' <exception cref="InstanceNotFoundException"> if one of the
		''' <code>names</code> does not correspond to any registered MBean. </exception>
		''' <exception cref="SecurityException"> if, for one of the MBeans, the
		''' client, or the delegated Subject if any, does not have
		''' permission to add a listener. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function addNotificationListeners(ByVal names As javax.management.ObjectName(), ByVal filters As java.rmi.MarshalledObject(), ByVal delegationSubjects As javax.security.auth.Subject()) As Integer?()

		''' <summary>
		''' <p>Handles the
		''' {@link javax.management.MBeanServerConnection#removeNotificationListener(ObjectName,NotificationListener)
		''' removeNotificationListener(ObjectName, NotificationListener)} and
		''' {@link javax.management.MBeanServerConnection#removeNotificationListener(ObjectName,NotificationListener,NotificationFilter,Object)
		''' removeNotificationListener(ObjectName, NotificationListener, NotificationFilter, Object)} methods.</p>
		''' 
		''' <p>This method removes one or more
		''' <code>NotificationListener</code>s from a given MBean in the
		''' MBean server.</p>
		''' 
		''' <p>The <code>NotificationListeners</code> are identified by the
		''' IDs which were returned by the {@link
		''' #addNotificationListeners(ObjectName[], MarshalledObject[],
		''' Subject[])} method.</p>
		''' </summary>
		''' <param name="name"> the <code>ObjectName</code> identifying the MBean
		''' emitting the Notifications. </param>
		''' <param name="listenerIDs"> the list of the IDs corresponding to the
		''' listeners to remove. </param>
		''' <param name="delegationSubject"> The <code>Subject</code> containing the
		''' delegation principals or <code>null</code> if the authentication
		''' principal is used instead.
		''' </param>
		''' <exception cref="InstanceNotFoundException"> if the given
		''' <code>name</code> does not correspond to any registered MBean. </exception>
		''' <exception cref="ListenerNotFoundException"> if one of the listeners was
		''' not found on the server side.  This exception can happen if the
		''' MBean discarded a listener for some reason other than a call to
		''' <code>MBeanServer.removeNotificationListener</code>. </exception>
		''' <exception cref="SecurityException"> if the client, or the delegated Subject
		''' if any, does not have permission to remove the listeners. </exception>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>ObjectName</code> or
		''' <code>listenerIds</code> is null or if <code>listenerIds</code>
		''' contains a null element. </exception>
		Sub removeNotificationListeners(ByVal name As javax.management.ObjectName, ByVal listenerIDs As Integer?(), ByVal delegationSubject As javax.security.auth.Subject)

		''' <summary>
		''' <p>Retrieves notifications from the connector server.  This
		''' method can block until there is at least one notification or
		''' until the specified timeout is reached.  The method can also
		''' return at any time with zero notifications.</p>
		''' 
		''' <p>A notification can be included in the result if its sequence
		''' number is no less than <code>clientSequenceNumber</code> and
		''' this client has registered at least one listener for the MBean
		''' generating the notification, with a filter that accepts the
		''' notification.  Each listener that is interested in the
		''' notification is identified by an Integer ID that was returned
		''' by {@link #addNotificationListeners(ObjectName[],
		''' MarshalledObject[], Subject[])}.</p>
		''' </summary>
		''' <param name="clientSequenceNumber"> the first sequence number that the
		''' client is interested in.  If negative, it is interpreted as
		''' meaning the sequence number that the next notification will
		''' have.
		''' </param>
		''' <param name="maxNotifications"> the maximum number of different
		''' notifications to return.  The <code>TargetedNotification</code>
		''' array in the returned <code>NotificationResult</code> can have
		''' more elements than this if the same notification appears more
		''' than once.  The behavior is unspecified if this parameter is
		''' negative.
		''' </param>
		''' <param name="timeout"> the maximum time in milliseconds to wait for a
		''' notification to arrive.  This can be 0 to indicate that the
		''' method should not wait if there are no notifications, but
		''' should return at once.  It can be <code>Long.MAX_VALUE</code>
		''' to indicate that there is no timeout.  The behavior is
		''' unspecified if this parameter is negative.
		''' </param>
		''' <returns> A <code>NotificationResult</code>.
		''' </returns>
		''' <exception cref="IOException"> if a general communication exception occurred. </exception>
		Function fetchNotifications(ByVal clientSequenceNumber As Long, ByVal maxNotifications As Integer, ByVal timeout As Long) As javax.management.remote.NotificationResult
	End Interface

End Namespace