Imports System

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

Namespace javax.management



	''' <summary>
	''' <p><seealso cref="InvocationHandler"/> that forwards methods in an MBean's
	''' management interface through the MBean server to the MBean.</p>
	''' 
	''' <p>Given an <seealso cref="MBeanServerConnection"/>, the <seealso cref="ObjectName"/>
	''' of an MBean within that MBean server, and a Java interface
	''' <code>Intf</code> that describes the management interface of the
	''' MBean using the patterns for a Standard MBean or an MXBean, this
	''' class can be used to construct a proxy for the MBean.  The proxy
	''' implements the interface <code>Intf</code> such that all of its
	''' methods are forwarded through the MBean server to the MBean.</p>
	''' 
	''' <p>If the {@code InvocationHandler} is for an MXBean, then the parameters of
	''' a method are converted from the type declared in the MXBean
	''' interface into the corresponding mapped type, and the return value
	''' is converted from the mapped type into the declared type.  For
	''' example, with the method<br>
	''' 
	''' {@code public List<String> reverse(List<String> list);}<br>
	''' 
	''' and given that the mapped type for {@code List<String>} is {@code
	''' String[]}, a call to {@code proxy.reverse(someList)} will convert
	''' {@code someList} from a {@code List<String>} to a {@code String[]},
	''' call the MBean operation {@code reverse}, then convert the returned
	''' {@code String[]} into a {@code List<String>}.</p>
	''' 
	''' <p>The method Object.toString(), Object.hashCode(), or
	''' Object.equals(Object), when invoked on a proxy using this
	''' invocation handler, is forwarded to the MBean server as a method on
	''' the proxied MBean only if it appears in one of the proxy's
	''' interfaces.  For a proxy created with {@link
	''' JMX#newMBeanProxy(MBeanServerConnection, ObjectName, Class)
	''' JMX.newMBeanProxy} or {@link
	''' JMX#newMXBeanProxy(MBeanServerConnection, ObjectName, Class)
	''' JMX.newMXBeanProxy}, this means that the method must appear in the
	''' Standard MBean or MXBean interface.  Otherwise these methods have
	''' the following behavior:
	''' <ul>
	''' <li>toString() returns a string representation of the proxy
	''' <li>hashCode() returns a hash code for the proxy such
	''' that two equal proxies have the same hash code
	''' <li>equals(Object)
	''' returns true if and only if the Object argument is of the same
	''' proxy class as this proxy, with an MBeanServerInvocationHandler
	''' that has the same MBeanServerConnection and ObjectName; if one
	''' of the {@code MBeanServerInvocationHandler}s was constructed with
	''' a {@code Class} argument then the other must have been constructed
	''' with the same {@code Class} for {@code equals} to return true.
	''' </ul>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanServerInvocationHandler
		Implements InvocationHandler

		''' <summary>
		''' <p>Invocation handler that forwards methods through an MBean
		''' server to a Standard MBean.  This constructor may be called
		''' instead of relying on {@link
		''' JMX#newMBeanProxy(MBeanServerConnection, ObjectName, Class)
		''' JMX.newMBeanProxy}, for instance if you need to supply a
		''' different <seealso cref="ClassLoader"/> to {@link Proxy#newProxyInstance
		''' Proxy.newProxyInstance}.</p>
		''' 
		''' <p>This constructor is not appropriate for an MXBean.  Use
		''' {@link #MBeanServerInvocationHandler(MBeanServerConnection,
		''' ObjectName, boolean)} for that.  This constructor is equivalent
		''' to {@code new MBeanServerInvocationHandler(connection,
		''' objectName, false)}.</p>
		''' </summary>
		''' <param name="connection"> the MBean server connection through which all
		''' methods of a proxy using this handler will be forwarded.
		''' </param>
		''' <param name="objectName"> the name of the MBean within the MBean server
		''' to which methods will be forwarded. </param>
		Public Sub New(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName)

			Me.New(connection, ___objectName, False)
		End Sub

		''' <summary>
		''' <p>Invocation handler that can forward methods through an MBean
		''' server to a Standard MBean or MXBean.  This constructor may be called
		''' instead of relying on {@link
		''' JMX#newMXBeanProxy(MBeanServerConnection, ObjectName, Class)
		''' JMX.newMXBeanProxy}, for instance if you need to supply a
		''' different <seealso cref="ClassLoader"/> to {@link Proxy#newProxyInstance
		''' Proxy.newProxyInstance}.</p>
		''' </summary>
		''' <param name="connection"> the MBean server connection through which all
		''' methods of a proxy using this handler will be forwarded.
		''' </param>
		''' <param name="objectName"> the name of the MBean within the MBean server
		''' to which methods will be forwarded.
		''' </param>
		''' <param name="isMXBean"> if true, the proxy is for an <seealso cref="MXBean"/>, and
		''' appropriate mappings will be applied to method parameters and return
		''' values.
		''' 
		''' @since 1.6 </param>
		Public Sub New(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal isMXBean As Boolean)
			If connection Is Nothing Then Throw New System.ArgumentException("Null connection")
			If Proxy.isProxyClass(connection.GetType()) Then
				If Proxy.getInvocationHandler(connection).GetType().IsSubclassOf(GetType(MBeanServerInvocationHandler)) Then Throw New System.ArgumentException("Wrapping MBeanServerInvocationHandler")
			End If
			If ___objectName Is Nothing Then Throw New System.ArgumentException("Null object name")
			Me.connection = connection
			Me.objectName = ___objectName
			Me.___isMXBean = isMXBean
		End Sub

		''' <summary>
		''' <p>The MBean server connection through which the methods of
		''' a proxy using this handler are forwarded.</p>
		''' </summary>
		''' <returns> the MBean server connection.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property mBeanServerConnection As MBeanServerConnection
			Get
				Return connection
			End Get
		End Property

		''' <summary>
		''' <p>The name of the MBean within the MBean server to which methods
		''' are forwarded.
		''' </summary>
		''' <returns> the object name.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property objectName As ObjectName
			Get
				Return objectName
			End Get
		End Property

		''' <summary>
		''' <p>If true, the proxy is for an MXBean, and appropriate mappings
		''' are applied to method parameters and return values.
		''' </summary>
		''' <returns> whether the proxy is for an MXBean.
		''' 
		''' @since 1.6 </returns>
		Public Overridable Property mXBean As Boolean
			Get
				Return ___isMXBean
			End Get
		End Property

		''' <summary>
		''' <p>Return a proxy that implements the given interface by
		''' forwarding its methods through the given MBean server to the
		''' named MBean.  As of 1.6, the methods {@link
		''' JMX#newMBeanProxy(MBeanServerConnection, ObjectName, Class)} and
		''' {@link JMX#newMBeanProxy(MBeanServerConnection, ObjectName, Class,
		''' boolean)} are preferred to this method.</p>
		''' 
		''' <p>This method is equivalent to {@link Proxy#newProxyInstance
		''' Proxy.newProxyInstance}<code>(interfaceClass.getClassLoader(),
		''' interfaces, handler)</code>.  Here <code>handler</code> is the
		''' result of {@link #MBeanServerInvocationHandler new
		''' MBeanServerInvocationHandler(connection, objectName)}, and
		''' <code>interfaces</code> is an array that has one element if
		''' <code>notificationBroadcaster</code> is false and two if it is
		''' true.  The first element of <code>interfaces</code> is
		''' <code>interfaceClass</code> and the second, if present, is
		''' <code>NotificationEmitter.class</code>.
		''' </summary>
		''' <param name="connection"> the MBean server to forward to. </param>
		''' <param name="objectName"> the name of the MBean within
		''' <code>connection</code> to forward to. </param>
		''' <param name="interfaceClass"> the management interface that the MBean
		''' exports, which will also be implemented by the returned proxy. </param>
		''' <param name="notificationBroadcaster"> make the returned proxy
		''' implement <seealso cref="NotificationEmitter"/> by forwarding its methods
		''' via <code>connection</code>. A call to {@link
		''' NotificationBroadcaster#addNotificationListener} on the proxy will
		''' result in a call to {@link
		''' MBeanServerConnection#addNotificationListener(ObjectName,
		''' NotificationListener, NotificationFilter, Object)}, and likewise
		''' for the other methods of <seealso cref="NotificationBroadcaster"/> and {@link
		''' NotificationEmitter}.
		''' </param>
		''' @param <T> allows the compiler to know that if the {@code
		''' interfaceClass} parameter is {@code MyMBean.class}, for example,
		''' then the return type is {@code MyMBean}.
		''' </param>
		''' <returns> the new proxy instance.
		''' </returns>
		''' <seealso cref= JMX#newMBeanProxy(MBeanServerConnection, ObjectName, Class, boolean) </seealso>
		Public Shared Function newProxyInstance(Of T)(ByVal connection As MBeanServerConnection, ByVal ___objectName As ObjectName, ByVal interfaceClass As Type, ByVal notificationBroadcaster As Boolean) As T
			Return JMX.newMBeanProxy(connection, ___objectName, interfaceClass, notificationBroadcaster)
		End Function

		Public Overridable Function invoke(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Dim methodClass As Type = method.declaringClass

			If methodClass.Equals(GetType(NotificationBroadcaster)) OrElse methodClass.Equals(GetType(NotificationEmitter)) Then Return invokeBroadcasterMethod(proxy, method, args)

			' local or not: equals, toString, hashCode
			If shouldDoLocally(proxy, method) Then Return doLocally(proxy, method, args)

			Try
				If mXBean Then
					Dim p As com.sun.jmx.mbeanserver.MXBeanProxy = findMXBeanProxy(methodClass)
					Return p.invoke(connection, objectName, method, args)
				Else
					Dim methodName As String = method.name
					Dim paramTypes As Type() = method.parameterTypes
					Dim returnType As Type = method.returnType

	'                 Inexplicably, InvocationHandler specifies that args is null
	'                   when the method takes no arguments rather than a
	'                   zero-length array.  
					Dim nargs As Integer = If(args Is Nothing, 0, args.Length)

					If methodName.StartsWith("get") AndAlso methodName.Length > 3 AndAlso nargs = 0 AndAlso (Not returnType.Equals(Void.TYPE)) Then Return connection.getAttribute(objectName, methodName.Substring(3))

					If methodName.StartsWith("is") AndAlso methodName.Length > 2 AndAlso nargs = 0 AndAlso (returnType.Equals(Boolean.TYPE) OrElse returnType.Equals(GetType(Boolean))) Then Return connection.getAttribute(objectName, methodName.Substring(2))

					If methodName.StartsWith("set") AndAlso methodName.Length > 3 AndAlso nargs = 1 AndAlso returnType.Equals(Void.TYPE) Then
						Dim attr As New Attribute(methodName.Substring(3), args(0))
						connection.attributeute(objectName, attr)
						Return Nothing
					End If

					Dim signature As String() = New String(paramTypes.Length - 1){}
					For i As Integer = 0 To paramTypes.Length - 1
						signature(i) = paramTypes(i).name
					Next i
					Return connection.invoke(objectName, methodName, args, signature)
				End If
			Catch e As MBeanException
				Throw e.targetException
			Catch re As RuntimeMBeanException
				Throw re.targetException
			Catch rre As RuntimeErrorException
				Throw rre.targetError
			End Try
	'         The invoke may fail because it can't get to the MBean, with
	'           one of the these exceptions declared by
	'           MBeanServerConnection.invoke:
	'           - RemoteException: can't talk to MBeanServer;
	'           - InstanceNotFoundException: objectName is not registered;
	'           - ReflectionException: objectName is registered but does not
	'             have the method being invoked.
	'           In all of these cases, the exception will be wrapped by the
	'           proxy mechanism in an UndeclaredThrowableException unless
	'           it happens to be declared in the "throws" clause of the
	'           method being invoked on the proxy.
	'         
		End Function

		Private Shared Function findMXBeanProxy(ByVal mxbeanInterface As Type) As com.sun.jmx.mbeanserver.MXBeanProxy
			SyncLock mxbeanProxies
				Dim proxyRef As WeakReference(Of com.sun.jmx.mbeanserver.MXBeanProxy) = mxbeanProxies.get(mxbeanInterface)
				Dim p As com.sun.jmx.mbeanserver.MXBeanProxy = If(proxyRef Is Nothing, Nothing, proxyRef.get())
				If p Is Nothing Then
					Try
						p = New com.sun.jmx.mbeanserver.MXBeanProxy(mxbeanInterface)
					Catch e As System.ArgumentException
						Dim msg As String = "Cannot make MXBean proxy for " & mxbeanInterface.name & ": " & e.Message
						Dim iae As New System.ArgumentException(msg, e.InnerException)
						iae.stackTrace = e.stackTrace
						Throw iae
					End Try
					mxbeanProxies.put(mxbeanInterface, New WeakReference(Of com.sun.jmx.mbeanserver.MXBeanProxy)(p))
				End If
				Return p
			End SyncLock
		End Function
		Private Shared ReadOnly mxbeanProxies As New java.util.WeakHashMap(Of Type, WeakReference(Of com.sun.jmx.mbeanserver.MXBeanProxy))

		Private Function invokeBroadcasterMethod(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Dim methodName As String = method.name
			Dim nargs As Integer = If(args Is Nothing, 0, args.Length)

			If methodName.Equals("addNotificationListener") Then
	'             The various throws of IllegalArgumentException here
	'               should not happen, since we know what the methods in
	'               NotificationBroadcaster and NotificationEmitter
	'               are.  
				If nargs <> 3 Then
					Dim msg As String = "Bad arg count to addNotificationListener: " & nargs
					Throw New System.ArgumentException(msg)
				End If
	'             Other inconsistencies will produce ClassCastException
	'               below.  

				Dim listener As NotificationListener = CType(args(0), NotificationListener)
				Dim filter As NotificationFilter = CType(args(1), NotificationFilter)
				Dim handback As Object = args(2)
				connection.addNotificationListener(objectName, listener, filter, handback)
				Return Nothing

			ElseIf methodName.Equals("removeNotificationListener") Then

	'             NullPointerException if method with no args, but that
	'               shouldn't happen because removeNL does have args.  
				Dim listener As NotificationListener = CType(args(0), NotificationListener)

				Select Case nargs
				Case 1
					connection.removeNotificationListener(objectName, listener)
					Return Nothing

				Case 3
					Dim filter As NotificationFilter = CType(args(1), NotificationFilter)
					Dim handback As Object = args(2)
					connection.removeNotificationListener(objectName, listener, filter, handback)
					Return Nothing

				Case Else
					Dim msg As String = "Bad arg count to removeNotificationListener: " & nargs
					Throw New System.ArgumentException(msg)
				End Select

			ElseIf methodName.Equals("getNotificationInfo") Then

				If args IsNot Nothing Then Throw New System.ArgumentException("getNotificationInfo has " & "args")

				Dim info As MBeanInfo = connection.getMBeanInfo(objectName)
				Return info.notifications

			Else
				Throw New System.ArgumentException("Bad method name: " & methodName)
			End If
		End Function

		Private Function shouldDoLocally(ByVal proxy As Object, ByVal method As Method) As Boolean
			Dim methodName As String = method.name
			If (methodName.Equals("hashCode") OrElse methodName.Equals("toString")) AndAlso method.parameterTypes.length = 0 AndAlso isLocal(proxy, method) Then Return True
			If methodName.Equals("equals") AndAlso java.util.Arrays.Equals(method.parameterTypes, New Type() {GetType(Object)}) AndAlso isLocal(proxy, method) Then Return True
			If methodName.Equals("finalize") AndAlso method.parameterTypes.length = 0 Then Return True
			Return False
		End Function

		Private Function doLocally(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Dim methodName As String = method.name

			If methodName.Equals("equals") Then

				If Me Is args(0) Then Return True

				If Not(TypeOf args(0) Is Proxy) Then Return False

				Dim ihandler As InvocationHandler = Proxy.getInvocationHandler(args(0))

				If ihandler Is Nothing OrElse Not(TypeOf ihandler Is MBeanServerInvocationHandler) Then Return False

				Dim handler As MBeanServerInvocationHandler = CType(ihandler, MBeanServerInvocationHandler)

				Return connection.Equals(handler.connection) AndAlso objectName.Equals(handler.objectName) AndAlso proxy.GetType().Equals(args(0).GetType())
			ElseIf methodName.Equals("toString") Then
				Return (If(mXBean, "MX", "M")) & "BeanProxy(" & connection & "[" & objectName & "])"
			ElseIf methodName.Equals("hashCode") Then
				Return objectName.GetHashCode()+connection.GetHashCode()
			ElseIf methodName.Equals("finalize") Then
				' ignore the finalizer invocation via proxy
				Return Nothing
			End If

			Throw New Exception("Unexpected method name: " & methodName)
		End Function

		Private Shared Function isLocal(ByVal proxy As Object, ByVal method As Method) As Boolean
			Dim interfaces As Type() = proxy.GetType().GetInterfaces()
			If interfaces Is Nothing Then Return True

			Dim methodName As String = method.name
			Dim params As Type() = method.parameterTypes
			For Each intf As Type In interfaces
				Try
					intf.GetMethod(methodName, params)
					Return False ' found method in one of our interfaces
				Catch nsme As NoSuchMethodException
					' OK.
				End Try
			Next intf

			Return True ' did not find in any interface
		End Function

		Private ReadOnly connection As MBeanServerConnection
		Private ReadOnly objectName As ObjectName
		Private ReadOnly ___isMXBean As Boolean
	End Class

End Namespace