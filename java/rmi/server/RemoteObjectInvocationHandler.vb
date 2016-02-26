Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2015, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.rmi.server


	''' <summary>
	''' An implementation of the <code>InvocationHandler</code> interface for
	''' use with Java Remote Method Invocation (Java RMI).  This invocation
	''' handler can be used in conjunction with a dynamic proxy instance as a
	''' replacement for a pregenerated stub class.
	''' 
	''' <p>Applications are not expected to use this class directly.  A remote
	''' object exported to use a dynamic proxy with <seealso cref="UnicastRemoteObject"/>
	''' or <seealso cref="Activatable"/> has an instance of this class as that proxy's
	''' invocation handler.
	''' 
	''' @author  Ann Wollrath
	''' @since   1.5
	''' 
	''' </summary>
	Public Class RemoteObjectInvocationHandler
		Inherits RemoteObject
		Implements InvocationHandler

		Private Const serialVersionUID As Long = 2L

		' set to true if invocation handler allows finalize method (legacy behavior)
		Private Shared ReadOnly allowFinalizeInvocation As Boolean

		Shared Sub New()
			Dim propName As String = "sun.rmi.server.invocationhandler.allowFinalizeInvocation"
			Dim allowProp As String = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			If "".Equals(allowProp) Then
				allowFinalizeInvocation = True
			Else
				allowFinalizeInvocation = Convert.ToBoolean(allowProp)
			End If
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As String
				Return System.getProperty(propName)
			End Function
		End Class

		''' <summary>
		''' A weak hash map, mapping classes to weak hash maps that map
		''' method objects to method hashes.
		''' 
		''' </summary>
		Private Shared ReadOnly methodToHash_Maps As New MethodToHash_Maps

		''' <summary>
		''' Creates a new <code>RemoteObjectInvocationHandler</code> constructed
		''' with the specified <code>RemoteRef</code>.
		''' </summary>
		''' <param name="ref"> the remote ref
		''' </param>
		''' <exception cref="NullPointerException"> if <code>ref</code> is <code>null</code>
		'''  </exception>
		Public Sub New(ByVal ref As RemoteRef)
			MyBase.New(ref)
			If ref Is Nothing Then Throw New NullPointerException
		End Sub

		''' <summary>
		''' Processes a method invocation made on the encapsulating
		''' proxy instance, <code>proxy</code>, and returns the result.
		''' 
		''' <p><code>RemoteObjectInvocationHandler</code> implements this method
		''' as follows:
		''' 
		''' <p>If <code>method</code> is one of the following methods, it
		''' is processed as described below:
		''' 
		''' <ul>
		''' 
		''' <li><seealso cref="Object#hashCode Object.hashCode"/>: Returns the hash
		''' code value for the proxy.
		''' 
		''' <li><seealso cref="Object#equals Object.equals"/>: Returns <code>true</code>
		''' if the argument (<code>args[0]</code>) is an instance of a dynamic
		''' proxy class and this invocation handler is equal to the invocation
		''' handler of that argument, and returns <code>false</code> otherwise.
		''' 
		''' <li><seealso cref="Object#toString Object.toString"/>: Returns a string
		''' representation of the proxy.
		''' </ul>
		''' 
		''' <p>Otherwise, a remote call is made as follows:
		''' 
		''' <ul>
		''' <li>If <code>proxy</code> is not an instance of the interface
		''' <seealso cref="Remote"/>, then an <seealso cref="IllegalArgumentException"/> is thrown.
		''' 
		''' <li>Otherwise, the <seealso cref="RemoteRef#invoke invoke"/> method is invoked
		''' on this invocation handler's <code>RemoteRef</code>, passing
		''' <code>proxy</code>, <code>method</code>, <code>args</code>, and the
		''' method hash (defined in section 8.3 of the "Java Remote Method
		''' Invocation (RMI) Specification") for <code>method</code>, and the
		''' result is returned.
		''' 
		''' <li>If an exception is thrown by <code>RemoteRef.invoke</code> and
		''' that exception is a checked exception that is not assignable to any
		''' exception in the <code>throws</code> clause of the method
		''' implemented by the <code>proxy</code>'s [Class], then that exception
		''' is wrapped in an <seealso cref="UnexpectedException"/> and the wrapped
		''' exception is thrown.  Otherwise, the exception thrown by
		''' <code>invoke</code> is thrown by this method.
		''' </ul>
		''' 
		''' <p>The semantics of this method are unspecified if the
		''' arguments could not have been produced by an instance of some
		''' valid dynamic proxy class containing this invocation handler.
		''' </summary>
		''' <param name="proxy"> the proxy instance that the method was invoked on </param>
		''' <param name="method"> the <code>Method</code> instance corresponding to the
		''' interface method invoked on the proxy instance </param>
		''' <param name="args"> an array of objects containing the values of the
		''' arguments passed in the method invocation on the proxy instance, or
		''' <code>null</code> if the method takes no arguments </param>
		''' <returns> the value to return from the method invocation on the proxy
		''' instance </returns>
		''' <exception cref="Throwable"> the exception to throw from the method invocation
		''' on the proxy instance
		'''  </exception>
		Public Overridable Function invoke(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object Implements InvocationHandler.invoke
			If Not Proxy.isProxyClass(proxy.GetType()) Then Throw New IllegalArgumentException("not a proxy")

			If Proxy.getInvocationHandler(proxy) IsNot Me Then Throw New IllegalArgumentException("handler mismatch")

			If method.declaringClass = GetType(Object) Then
				Return invokeObjectMethod(proxy, method, args)
			ElseIf "finalize".Equals(method.name) AndAlso method.parameterCount = 0 AndAlso (Not allowFinalizeInvocation) Then
				Return Nothing ' ignore
			Else
				Return invokeRemoteMethod(proxy, method, args)
			End If
		End Function

		''' <summary>
		''' Handles java.lang.Object methods.
		''' 
		''' </summary>
		Private Function invokeObjectMethod(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Dim name As String = method.name

			If name.Equals("hashCode") Then
				Return GetHashCode()

			ElseIf name.Equals("equals") Then
				Dim obj As Object = args(0)
				Dim hdlr As InvocationHandler
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return proxy Is obj OrElse (obj IsNot Nothing AndAlso Proxy.isProxyClass(obj.GetType()) AndAlso TypeOf (hdlr = Proxy.getInvocationHandler(obj)) Is RemoteObjectInvocationHandler AndAlso Me.Equals(hdlr))

			ElseIf name.Equals("toString") Then
				Return proxyToString(proxy)

			Else
				Throw New IllegalArgumentException("unexpected Object method: " & method)
			End If
		End Function

		''' <summary>
		''' Handles remote methods.
		''' 
		''' </summary>
		Private Function invokeRemoteMethod(ByVal proxy As Object, ByVal method As Method, ByVal args As Object()) As Object
			Try
				If Not(TypeOf proxy Is java.rmi.Remote) Then Throw New IllegalArgumentException("proxy not Remote instance")
				Return ref.invoke(CType(proxy, java.rmi.Remote), method, args, getMethodHash(method))
			Catch e As Exception
				If Not(TypeOf e Is RuntimeException) Then
					Dim cl As  [Class] = proxy.GetType()
					Try
						method = cl.getMethod(method.name, method.parameterTypes)
					Catch nsme As NoSuchMethodException
						Throw CType((New IllegalArgumentException).initCause(nsme), IllegalArgumentException)
					End Try
					Dim thrownType As  [Class] = e.GetType()
					For Each declaredType As  [Class] In method.exceptionTypes
						If thrownType.IsSubclassOf(declaredType) Then Throw e
					Next declaredType
					e = New java.rmi.UnexpectedException("unexpected exception", e)
				End If
				Throw e
			End Try
		End Function

		''' <summary>
		''' Returns a string representation for a proxy that uses this invocation
		''' handler.
		''' 
		''' </summary>
		Private Function proxyToString(ByVal proxy As Object) As String
			Dim interfaces As  [Class]() = proxy.GetType().GetInterfaces()
			If interfaces.Length = 0 Then Return "Proxy[" & Me & "]"
			Dim iface As String = interfaces(0).name
			If iface.Equals("java.rmi.Remote") AndAlso interfaces.Length > 1 Then iface = interfaces(1).name
			Dim dot As Integer = iface.LastIndexOf("."c)
			If dot >= 0 Then iface = iface.Substring(dot + 1)
			Return "Proxy[" & iface & "," & Me & "]"
		End Function

		''' <exception cref="InvalidObjectException"> unconditionally
		'''  </exception>
		Private Sub readObjectNoData()
			Throw New java.io.InvalidObjectException("no data in stream; class: " & Me.GetType().name)
		End Sub

		''' <summary>
		''' Returns the method hash for the specified method.  Subsequent calls
		''' to "getMethodHash" passing the same method argument should be faster
		''' since this method caches internally the result of the method to
		''' method hash mapping.  The method hash is calculated using the
		''' "computeMethodHash" method.
		''' </summary>
		''' <param name="method"> the remote method </param>
		''' <returns> the method hash for the specified method </returns>
		Private Shared Function getMethodHash(ByVal method As Method) As Long
			Return methodToHash_Maps.get(method.declaringClass).get(method)
		End Function

		''' <summary>
		''' A weak hash map, mapping classes to weak hash maps that map
		''' method objects to method hashes.
		''' 
		''' </summary>
		Private Class MethodToHash_Maps
			Inherits sun.rmi.server.WeakClassHashMap(Of IDictionary(Of Method, Long?))

			Friend Sub New()
			End Sub

			Protected Friend Overridable Function computeValue(ByVal remoteClass As [Class]) As IDictionary(Of Method, Long?)
				Return New WeakHashMapAnonymousInnerClassHelper(Of K, V)
			End Function

			Private Class WeakHashMapAnonymousInnerClassHelper(Of K, V)
				Inherits java.util.WeakHashMap(Of K, V)

				<MethodImpl(MethodImplOptions.Synchronized)> _
				Public Overridable Function [get](ByVal key As Object) As Long?
					Dim hash As Long? = MyBase.get(key)
					If hash Is Nothing Then
						Dim method As Method = CType(key, Method)
						hash = sun.rmi.server.Util.computeMethodHash(method)
						put(method, hash)
					End If
					Return hash
				End Function
			End Class
		End Class
	End Class

End Namespace