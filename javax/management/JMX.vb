Imports System

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Static methods from the JMX API.  There are no instances of this class.
	''' 
	''' @since 1.6
	''' </summary>
	Public Class JMX
	'     Code within this package can prove that by providing this instance of
	'     * this class.
	'     
		Friend Shared ReadOnly proof As New JMX

		Private Sub New()
		End Sub

		''' <summary>
		''' The name of the <a href="Descriptor.html#defaultValue">{@code
		''' defaultValue}</a> field.
		''' </summary>
		Public Const DEFAULT_VALUE_FIELD As String = "defaultValue"

		''' <summary>
		''' The name of the <a href="Descriptor.html#immutableInfo">{@code
		''' immutableInfo}</a> field.
		''' </summary>
		Public Const IMMUTABLE_INFO_FIELD As String = "immutableInfo"

		''' <summary>
		''' The name of the <a href="Descriptor.html#interfaceClassName">{@code
		''' interfaceClassName}</a> field.
		''' </summary>
		Public Const INTERFACE_CLASS_NAME_FIELD As String = "interfaceClassName"

		''' <summary>
		''' The name of the <a href="Descriptor.html#legalValues">{@code
		''' legalValues}</a> field.
		''' </summary>
		Public Const LEGAL_VALUES_FIELD As String = "legalValues"

		''' <summary>
		''' The name of the <a href="Descriptor.html#maxValue">{@code
		''' maxValue}</a> field.
		''' </summary>
		Public Const MAX_VALUE_FIELD As String = "maxValue"

		''' <summary>
		''' The name of the <a href="Descriptor.html#minValue">{@code
		''' minValue}</a> field.
		''' </summary>
		Public Const MIN_VALUE_FIELD As String = "minValue"

		''' <summary>
		''' The name of the <a href="Descriptor.html#mxbean">{@code
		''' mxbean}</a> field.
		''' </summary>
		Public Const MXBEAN_FIELD As String = "mxbean"

		''' <summary>
		''' The name of the <a href="Descriptor.html#openType">{@code
		''' openType}</a> field.
		''' </summary>
		Public Const OPEN_TYPE_FIELD As String = "openType"

		''' <summary>
		''' The name of the <a href="Descriptor.html#originalType">{@code
		''' originalType}</a> field.
		''' </summary>
		Public Const ORIGINAL_TYPE_FIELD As String = "originalType"

		''' <summary>
		''' <p>Make a proxy for a Standard MBean in a local or remote
		''' MBean Server.</p>
		''' 
		''' <p>If you have an MBean Server {@code mbs} containing an MBean
		''' with <seealso cref="ObjectName"/> {@code name}, and if the MBean's
		''' management interface is described by the Java interface
		''' {@code MyMBean}, you can construct a proxy for the MBean like
		''' this:</p>
		''' 
		''' <pre>
		''' MyMBean proxy = JMX.newMBeanProxy(mbs, name, MyMBean.class);
		''' </pre>
		''' 
		''' <p>Suppose, for example, {@code MyMBean} looks like this:</p>
		''' 
		''' <pre>
		''' public interface MyMBean {
		'''     public String getSomeAttribute();
		'''     public void setSomeAttribute(String value);
		'''     public void someOperation(String param1, int param2);
		''' }
		''' </pre>
		''' 
		''' <p>Then you can execute:</p>
		''' 
		''' <ul>
		''' 
		''' <li>{@code proxy.getSomeAttribute()} which will result in a
		''' call to {@code mbs.}{@link MBeanServerConnection#getAttribute
		''' getAttribute}{@code (name, "SomeAttribute")}.
		''' 
		''' <li>{@code proxy.setSomeAttribute("whatever")} which will result
		''' in a call to {@code mbs.}{@link MBeanServerConnection#setAttribute
		''' setAttribute}{@code (name, new Attribute("SomeAttribute", "whatever"))}.
		''' 
		''' <li>{@code proxy.someOperation("param1", 2)} which will be
		''' translated into a call to {@code mbs.}{@link
		''' MBeanServerConnection#invoke invoke}{@code (name, "someOperation", <etc>)}.
		''' 
		''' </ul>
		''' 
		''' <p>The object returned by this method is a
		''' <seealso cref="Proxy"/> whose {@code InvocationHandler} is an
		''' <seealso cref="MBeanServerInvocationHandler"/>.</p>
		''' 
		''' <p>This method is equivalent to {@link
		''' #newMBeanProxy(MBeanServerConnection, ObjectName, Class,
		''' boolean) newMBeanProxy(connection, objectName, interfaceClass,
		''' false)}.</p>
		''' </summary>
		''' <param name="connection"> the MBean server to forward to. </param>
		''' <param name="objectName"> the name of the MBean within
		''' {@code connection} to forward to. </param>
		''' <param name="interfaceClass"> the management interface that the MBean
		''' exports, which will also be implemented by the returned proxy.
		''' </param>
		''' @param <T> allows the compiler to know that if the {@code
		''' interfaceClass} parameter is {@code MyMBean.class}, for
		''' example, then the return type is {@code MyMBean}.
		''' </param>
		''' <returns> the new proxy instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if {@code interfaceClass} is not
		''' a <a href="package-summary.html#mgIface">compliant MBean
		''' interface</a> </exception>
		Public Shared Function newMBeanProxy(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type) As T
			Return newMBeanProxy(connection, ___objectName, interfaceClass, False)
		End Function

		''' <summary>
		''' <p>Make a proxy for a Standard MBean in a local or remote MBean
		''' Server that may also support the methods of {@link
		''' NotificationEmitter}.</p>
		''' 
		''' <p>This method behaves the same as {@link
		''' #newMBeanProxy(MBeanServerConnection, ObjectName, Class)}, but
		''' additionally, if {@code notificationEmitter} is {@code
		''' true}, then the MBean is assumed to be a {@link
		''' NotificationBroadcaster} or <seealso cref="NotificationEmitter"/> and the
		''' returned proxy will implement <seealso cref="NotificationEmitter"/> as
		''' well as {@code interfaceClass}.  A call to {@link
		''' NotificationBroadcaster#addNotificationListener} on the proxy
		''' will result in a call to {@link
		''' MBeanServerConnection#addNotificationListener(ObjectName,
		''' NotificationListener, NotificationFilter, Object)}, and
		''' likewise for the other methods of {@link
		''' NotificationBroadcaster} and <seealso cref="NotificationEmitter"/>.</p>
		''' </summary>
		''' <param name="connection"> the MBean server to forward to. </param>
		''' <param name="objectName"> the name of the MBean within
		''' {@code connection} to forward to. </param>
		''' <param name="interfaceClass"> the management interface that the MBean
		''' exports, which will also be implemented by the returned proxy. </param>
		''' <param name="notificationEmitter"> make the returned proxy
		''' implement <seealso cref="NotificationEmitter"/> by forwarding its methods
		''' via {@code connection}.
		''' </param>
		''' @param <T> allows the compiler to know that if the {@code
		''' interfaceClass} parameter is {@code MyMBean.class}, for
		''' example, then the return type is {@code MyMBean}.
		''' </param>
		''' <returns> the new proxy instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if {@code interfaceClass} is not
		''' a <a href="package-summary.html#mgIface">compliant MBean
		''' interface</a> </exception>
		Public Shared Function newMBeanProxy(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type, ByVal notificationEmitter As Boolean) As T
			Return createProxy(connection, ___objectName, interfaceClass, notificationEmitter, False)
		End Function

		''' <summary>
		''' Make a proxy for an MXBean in a local or remote MBean Server.
		''' 
		''' <p>If you have an MBean Server {@code mbs} containing an
		''' MXBean with <seealso cref="ObjectName"/> {@code name}, and if the
		''' MXBean's management interface is described by the Java
		''' interface {@code MyMXBean}, you can construct a proxy for
		''' the MXBean like this:</p>
		''' 
		''' <pre>
		''' MyMXBean proxy = JMX.newMXBeanProxy(mbs, name, MyMXBean.class);
		''' </pre>
		''' 
		''' <p>Suppose, for example, {@code MyMXBean} looks like this:</p>
		''' 
		''' <pre>
		''' public interface MyMXBean {
		'''     public String getSimpleAttribute();
		'''     public void setSimpleAttribute(String value);
		'''     public <seealso cref="java.lang.management.MemoryUsage"/> getMappedAttribute();
		'''     public void setMappedAttribute(MemoryUsage memoryUsage);
		'''     public MemoryUsage someOperation(String param1, MemoryUsage param2);
		''' }
		''' </pre>
		''' 
		''' <p>Then:</p>
		''' 
		''' <ul>
		''' 
		''' <li><p>{@code proxy.getSimpleAttribute()} will result in a
		''' call to {@code mbs.}{@link MBeanServerConnection#getAttribute
		''' getAttribute}{@code (name, "SimpleAttribute")}.</p>
		''' 
		''' <li><p>{@code proxy.setSimpleAttribute("whatever")} will result
		''' in a call to {@code mbs.}{@link
		''' MBeanServerConnection#setAttribute setAttribute}<code>(name,
		''' new Attribute("SimpleAttribute", "whatever"))</code>.</p>
		''' 
		'''     <p>Because {@code String} is a <em>simple type</em>, in the
		'''     sense of <seealso cref="javax.management.openmbean.SimpleType"/>, it
		'''     is not changed in the context of an MXBean.  The MXBean
		'''     proxy behaves the same as a Standard MBean proxy (see
		'''     {@link #newMBeanProxy(MBeanServerConnection, ObjectName,
		'''     Class) newMBeanProxy}) for the attribute {@code
		'''     SimpleAttribute}.</p>
		''' 
		''' <li><p>{@code proxy.getMappedAttribute()} will result in a call
		''' to {@code mbs.getAttribute("MappedAttribute")}.  The MXBean
		''' mapping rules mean that the actual type of the attribute {@code
		''' MappedAttribute} will be {@link
		''' javax.management.openmbean.CompositeData CompositeData} and
		''' that is what the {@code mbs.getAttribute} call will return.
		''' The proxy will then convert the {@code CompositeData} back into
		''' the expected type {@code MemoryUsage} using the MXBean mapping
		''' rules.</p>
		''' 
		''' <li><p>Similarly, {@code proxy.setMappedAttribute(memoryUsage)}
		''' will convert the {@code MemoryUsage} argument into a {@code
		''' CompositeData} before calling {@code mbs.setAttribute}.</p>
		''' 
		''' <li><p>{@code proxy.someOperation("whatever", memoryUsage)}
		''' will convert the {@code MemoryUsage} argument into a {@code
		''' CompositeData} and call {@code mbs.invoke}.  The value returned
		''' by {@code mbs.invoke} will be also be a {@code CompositeData},
		''' and the proxy will convert this into the expected type {@code
		''' MemoryUsage} using the MXBean mapping rules.</p>
		''' 
		''' </ul>
		''' 
		''' <p>The object returned by this method is a
		''' <seealso cref="Proxy"/> whose {@code InvocationHandler} is an
		''' <seealso cref="MBeanServerInvocationHandler"/>.</p>
		''' 
		''' <p>This method is equivalent to {@link
		''' #newMXBeanProxy(MBeanServerConnection, ObjectName, Class,
		''' boolean) newMXBeanProxy(connection, objectName, interfaceClass,
		''' false)}.</p>
		''' </summary>
		''' <param name="connection"> the MBean server to forward to. </param>
		''' <param name="objectName"> the name of the MBean within
		''' {@code connection} to forward to. </param>
		''' <param name="interfaceClass"> the MXBean interface,
		''' which will also be implemented by the returned proxy.
		''' </param>
		''' @param <T> allows the compiler to know that if the {@code
		''' interfaceClass} parameter is {@code MyMXBean.class}, for
		''' example, then the return type is {@code MyMXBean}.
		''' </param>
		''' <returns> the new proxy instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if {@code interfaceClass} is not
		''' a <seealso cref="javax.management.MXBean compliant MXBean interface"/> </exception>
		Public Shared Function newMXBeanProxy(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type) As T
			Return newMXBeanProxy(connection, ___objectName, interfaceClass, False)
		End Function

		''' <summary>
		''' <p>Make a proxy for an MXBean in a local or remote MBean
		''' Server that may also support the methods of {@link
		''' NotificationEmitter}.</p>
		''' 
		''' <p>This method behaves the same as {@link
		''' #newMXBeanProxy(MBeanServerConnection, ObjectName, Class)}, but
		''' additionally, if {@code notificationEmitter} is {@code
		''' true}, then the MXBean is assumed to be a {@link
		''' NotificationBroadcaster} or <seealso cref="NotificationEmitter"/> and the
		''' returned proxy will implement <seealso cref="NotificationEmitter"/> as
		''' well as {@code interfaceClass}.  A call to {@link
		''' NotificationBroadcaster#addNotificationListener} on the proxy
		''' will result in a call to {@link
		''' MBeanServerConnection#addNotificationListener(ObjectName,
		''' NotificationListener, NotificationFilter, Object)}, and
		''' likewise for the other methods of {@link
		''' NotificationBroadcaster} and <seealso cref="NotificationEmitter"/>.</p>
		''' </summary>
		''' <param name="connection"> the MBean server to forward to. </param>
		''' <param name="objectName"> the name of the MBean within
		''' {@code connection} to forward to. </param>
		''' <param name="interfaceClass"> the MXBean interface,
		''' which will also be implemented by the returned proxy. </param>
		''' <param name="notificationEmitter"> make the returned proxy
		''' implement <seealso cref="NotificationEmitter"/> by forwarding its methods
		''' via {@code connection}.
		''' </param>
		''' @param <T> allows the compiler to know that if the {@code
		''' interfaceClass} parameter is {@code MyMXBean.class}, for
		''' example, then the return type is {@code MyMXBean}.
		''' </param>
		''' <returns> the new proxy instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if {@code interfaceClass} is not
		''' a <seealso cref="javax.management.MXBean compliant MXBean interface"/> </exception>
		Public Shared Function newMXBeanProxy(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type, ByVal notificationEmitter As Boolean) As T
			Return createProxy(connection, ___objectName, interfaceClass, notificationEmitter, True)
		End Function

		''' <summary>
		''' <p>Test whether an interface is an MXBean interface.
		''' An interface is an MXBean interface if it is public,
		''' annotated <seealso cref="MXBean &#64;MXBean"/> or {@code @MXBean(true)}
		''' or if it does not have an {@code @MXBean} annotation
		''' and its name ends with "{@code MXBean}".</p>
		''' </summary>
		''' <param name="interfaceClass"> The candidate interface.
		''' </param>
		''' <returns> true if {@code interfaceClass} is a
		''' <seealso cref="javax.management.MXBean compliant MXBean interface"/>
		''' </returns>
		''' <exception cref="NullPointerException"> if {@code interfaceClass} is null. </exception>
		Public Shared Function isMXBeanInterface(ByVal interfaceClass As Type) As Boolean
			If Not interfaceClass.IsInterface Then Return False
			If (Not Modifier.isPublic(interfaceClass.modifiers)) AndAlso (Not com.sun.jmx.mbeanserver.Introspector.ALLOW_NONPUBLIC_MBEAN) Then Return False
			Dim a As MXBean = interfaceClass.getAnnotation(GetType(MXBean))
			If a IsNot Nothing Then Return a.value()
			Return interfaceClass.name.EndsWith("MXBean")
			' We don't bother excluding the case where the name is
			' exactly the string "MXBean" since that would mean there
			' was no package name, which is pretty unlikely in practice.
		End Function

		''' <summary>
		''' Centralised M(X)Bean proxy creation code </summary>
		''' <param name="connection"> <seealso cref="MBeanServerConnection"/> to use </param>
		''' <param name="objectName"> M(X)Bean object name </param>
		''' <param name="interfaceClass"> M(X)Bean interface class </param>
		''' <param name="notificationEmitter"> Is a notification emitter? </param>
		''' <param name="isMXBean"> Is an MXBean? </param>
		''' <returns> Returns an M(X)Bean proxy generated for the provided interface class </returns>
		''' <exception cref="SecurityException"> </exception>
		''' <exception cref="IllegalArgumentException"> </exception>
		Private Shared Function createProxy(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type, ByVal notificationEmitter As Boolean, ByVal isMXBean As Boolean) As T

			Try
				If isMXBean Then
					' Check interface for MXBean compliance
					com.sun.jmx.mbeanserver.Introspector.testComplianceMXBeanInterface(interfaceClass)
				Else
					' Check interface for MBean compliance
					com.sun.jmx.mbeanserver.Introspector.testComplianceMBeanInterface(interfaceClass)
				End If
			Catch e As NotCompliantMBeanException
				Throw New System.ArgumentException(e)
			End Try

			Dim handler As InvocationHandler = New MBeanServerInvocationHandler(connection, ___objectName, isMXBean)
			Dim interfaces As Type()
			If notificationEmitter Then
				interfaces = New Type() {interfaceClass, GetType(NotificationEmitter)}
			Else
				interfaces = New Type() {interfaceClass}
			End If

			Dim proxy As Object = Proxy.newProxyInstance(interfaceClass.classLoader, interfaces, handler)
			Return interfaceClass.cast(proxy)
		End Function
	End Class

End Namespace