Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.invoke


	''' <summary>
	''' This class consists exclusively of static methods that help adapt
	''' method handles to other JVM types, such as interfaces.
	''' </summary>
	Public Class MethodHandleProxies

		Private Sub New() ' do not instantiate
		End Sub

		''' <summary>
		''' Produces an instance of the given single-method interface which redirects
		''' its calls to the given method handle.
		''' <p>
		''' A single-method interface is an interface which declares a uniquely named method.
		''' When determining the uniquely named method of a single-method interface,
		''' the public {@code Object} methods ({@code toString}, {@code equals}, {@code hashCode})
		''' are disregarded.  For example, <seealso cref="java.util.Comparator"/> is a single-method interface,
		''' even though it re-declares the {@code Object.equals} method.
		''' <p>
		''' The interface must be public.  No additional access checks are performed.
		''' <p>
		''' The resulting instance of the required type will respond to
		''' invocation of the type's uniquely named method by calling
		''' the given target on the incoming arguments,
		''' and returning or throwing whatever the target
		''' returns or throws.  The invocation will be as if by
		''' {@code target.invoke}.
		''' The target's type will be checked before the
		''' instance is created, as if by a call to {@code asType},
		''' which may result in a {@code WrongMethodTypeException}.
		''' <p>
		''' The uniquely named method is allowed to be multiply declared,
		''' with distinct type descriptors.  (E.g., it can be overloaded,
		''' or can possess bridge methods.)  All such declarations are
		''' connected directly to the target method handle.
		''' Argument and return types are adjusted by {@code asType}
		''' for each individual declaration.
		''' <p>
		''' The wrapper instance will implement the requested interface
		''' and its super-types, but no other single-method interfaces.
		''' This means that the instance will not unexpectedly
		''' pass an {@code instanceof} test for any unrequested type.
		''' <p style="font-size:smaller;">
		''' <em>Implementation Note:</em>
		''' Therefore, each instance must implement a unique single-method interface.
		''' Implementations may not bundle together
		''' multiple single-method interfaces onto single implementation classes
		''' in the style of <seealso cref="java.awt.AWTEventMulticaster"/>.
		''' <p>
		''' The method handle may throw an <em>undeclared exception</em>,
		''' which means any checked exception (or other checked throwable)
		''' not declared by the requested type's single abstract method.
		''' If this happens, the throwable will be wrapped in an instance of
		''' <seealso cref="java.lang.reflect.UndeclaredThrowableException UndeclaredThrowableException"/>
		''' and thrown in that wrapped form.
		''' <p>
		''' Like <seealso cref="java.lang.Integer#valueOf  java.lang.[Integer].valueOf"/>,
		''' {@code asInterfaceInstance} is a factory method whose results are defined
		''' by their behavior.
		''' It is not guaranteed to return a new instance for every call.
		''' <p>
		''' Because of the possibility of <seealso cref="java.lang.reflect.Method#isBridge bridge methods"/>
		''' and other corner cases, the interface may also have several abstract methods
		''' with the same name but having distinct descriptors (types of returns and parameters).
		''' In this case, all the methods are bound in common to the one given target.
		''' The type check and effective {@code asType} conversion is applied to each
		''' method type descriptor, and all abstract methods are bound to the target in common.
		''' Beyond this type check, no further checks are made to determine that the
		''' abstract methods are related in any way.
		''' <p>
		''' Future versions of this API may accept additional types,
		''' such as abstract classes with single abstract methods.
		''' Future versions of this API may also equip wrapper instances
		''' with one or more additional public "marker" interfaces.
		''' <p>
		''' If a security manager is installed, this method is caller sensitive.
		''' During any invocation of the target method handle via the returned wrapper,
		''' the original creator of the wrapper (the caller) will be visible
		''' to context checks requested by the security manager.
		''' </summary>
		''' @param <T> the desired type of the wrapper, a single-method interface </param>
		''' <param name="intfc"> a class object representing {@code T} </param>
		''' <param name="target"> the method handle to invoke from the wrapper </param>
		''' <returns> a correctly-typed wrapper for the given target </returns>
		''' <exception cref="NullPointerException"> if either argument is null </exception>
		''' <exception cref="IllegalArgumentException"> if the {@code intfc} is not a
		'''         valid argument to this method </exception>
		''' <exception cref="WrongMethodTypeException"> if the target cannot
		'''         be converted to the type required by the requested interface </exception>
		' Other notes to implementors:
		' <p>
		' No stable mapping is promised between the single-method interface and
		' the implementation class C.  Over time, several implementation
		' classes might be used for the same type.
		' <p>
		' If the implementation is able
		' to prove that a wrapper of the required type
		' has already been created for a given
		' method handle, or for another method handle with the
		' same behavior, the implementation may return that wrapper in place of
		' a new wrapper.
		' <p>
		' This method is designed to apply to common use cases
		' where a single method handle must interoperate with
		' an interface that implements a function-like
		' API.  Additional variations, such as single-abstract-method classes with
		' private constructors, or interfaces with multiple but related
		' entry points, must be covered by hand-written or automatically
		' generated adapter classes.
		'
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function asInterfaceInstance(Of T)(ByVal intfc As [Class], ByVal target As MethodHandle) As T
			If (Not intfc.interface) OrElse (Not Modifier.isPublic(intfc.modifiers)) Then Throw newIllegalArgumentException("not a public interface", intfc.name)
			Dim mh As MethodHandle
			If System.securityManager IsNot Nothing Then
				Dim caller As  [Class] = sun.reflect.Reflection.callerClass
				Dim ccl As  ClassLoader = If(caller IsNot Nothing, caller.classLoader, Nothing)
				sun.reflect.misc.ReflectUtil.checkProxyPackageAccess(ccl, intfc)
				mh = If(ccl IsNot Nothing, bindCaller(target, caller), target)
			Else
				mh = target
			End If
			Dim proxyLoader As  ClassLoader = intfc.classLoader
			If proxyLoader Is Nothing Then
				Dim cl As  ClassLoader = Thread.CurrentThread.contextClassLoader ' avoid use of BCP
				proxyLoader = If(cl IsNot Nothing, cl, ClassLoader.systemClassLoader)
			End If
			Dim methods As Method() = getSingleNameMethods(intfc)
			If methods Is Nothing Then Throw newIllegalArgumentException("not a single-method interface", intfc.name)
			Dim vaTargets As MethodHandle() = New MethodHandle(methods.Length - 1){}
			For i As Integer = 0 To methods.Length - 1
				Dim sm As Method = methods(i)
				Dim smMT As MethodType = MethodType.methodType(sm.returnType, sm.parameterTypes)
				Dim checkTarget As MethodHandle = mh.asType(smMT) ' make throw WMT
				checkTarget = checkTarget.asType(checkTarget.type().changeReturnType(GetType(Object)))
				vaTargets(i) = checkTarget.asSpreader(GetType(Object()), smMT.parameterCount())
			Next i
			Dim ih As InvocationHandler = New InvocationHandlerAnonymousInnerClassHelper

			Dim proxy_Renamed As Object
			If System.securityManager IsNot Nothing Then
				' sun.invoke.WrapperInstance is a restricted interface not accessible
				' by any non-null class loader.
				Dim loader As  ClassLoader = proxyLoader
				proxy_Renamed = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Else
				proxy_Renamed = Proxy.newProxyInstance(proxyLoader, New [Class](){ intfc, GetType(sun.invoke.WrapperInstance) }, ih)
			End If
			Return intfc.cast(proxy_Renamed)
		End Function

		Private Class InvocationHandlerAnonymousInnerClassHelper
			Implements InvocationHandler

			Private Function getArg(ByVal name As String) As Object
				If CObj(name) Is "getWrapperInstanceTarget" Then Return target
				If CObj(name) Is "getWrapperInstanceType" Then Return intfc
				Throw New AssertionError
			End Function
			Public Overridable Function invoke(ByVal proxy_Renamed As Object, ByVal method As Method, ByVal args As Object()) As Object Implements InvocationHandler.invoke
				For i As Integer = 0 To methods.length - 1
					If method.Equals(methods(i)) Then Return vaTargets(i).invokeExact(args)
				Next i
				If method.declaringClass Is GetType(sun.invoke.WrapperInstance) Then Return getArg(method.name)
				If isObjectMethod(method) Then Return callObjectMethod(proxy_Renamed, method, args)
				Throw newInternalError("bad proxy method: " & method)
			End Function
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				Return Proxy.newProxyInstance(loader, New [Class](){ intfc, GetType(sun.invoke.WrapperInstance) }, ih)
			End Function
		End Class

		Private Shared Function bindCaller(ByVal target As MethodHandle, ByVal hostClass As [Class]) As MethodHandle
			Dim cbmh As MethodHandle = MethodHandleImpl.bindCaller(target, hostClass)
			If target.varargsCollector Then
				Dim type As MethodType = cbmh.type()
				Dim arity As Integer = type.parameterCount()
				Return cbmh.asVarargsCollector(type.parameterType(arity-1))
			End If
			Return cbmh
		End Function

		''' <summary>
		''' Determines if the given object was produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>. </summary>
		''' <param name="x"> any reference </param>
		''' <returns> true if the reference is not null and points to an object produced by {@code asInterfaceInstance} </returns>
		Public Shared Function isWrapperInstance(ByVal x As Object) As Boolean
			Return TypeOf x Is sun.invoke.WrapperInstance
		End Function

		Private Shared Function asWrapperInstance(ByVal x As Object) As sun.invoke.WrapperInstance
			Try
				If x IsNot Nothing Then Return CType(x, sun.invoke.WrapperInstance)
			Catch ex As  ClassCastException
			End Try
			Throw newIllegalArgumentException("not a wrapper instance")
		End Function

		''' <summary>
		''' Produces or recovers a target method handle which is behaviorally
		''' equivalent to the unique method of this wrapper instance.
		''' The object {@code x} must have been produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>.
		''' This requirement may be tested via <seealso cref="#isWrapperInstance isWrapperInstance"/>. </summary>
		''' <param name="x"> any reference </param>
		''' <returns> a method handle implementing the unique method </returns>
		''' <exception cref="IllegalArgumentException"> if the reference x is not to a wrapper instance </exception>
		Public Shared Function wrapperInstanceTarget(ByVal x As Object) As MethodHandle
			Return asWrapperInstance(x).wrapperInstanceTarget
		End Function

		''' <summary>
		''' Recovers the unique single-method interface type for which this wrapper instance was created.
		''' The object {@code x} must have been produced by a call to <seealso cref="#asInterfaceInstance asInterfaceInstance"/>.
		''' This requirement may be tested via <seealso cref="#isWrapperInstance isWrapperInstance"/>. </summary>
		''' <param name="x"> any reference </param>
		''' <returns> the single-method interface type for which the wrapper was created </returns>
		''' <exception cref="IllegalArgumentException"> if the reference x is not to a wrapper instance </exception>
		Public Shared Function wrapperInstanceType(ByVal x As Object) As  [Class]
			Return asWrapperInstance(x).wrapperInstanceType
		End Function

		Private Shared Function isObjectMethod(ByVal m As Method) As Boolean
			Select Case m.name
			Case "toString"
				Return (m.returnType Is GetType(String) AndAlso m.parameterTypes.Length = 0)
			Case "hashCode"
				Return (m.returnType Is GetType(Integer) AndAlso m.parameterTypes.Length = 0)
			Case "equals"
				Return (m.returnType Is GetType(Boolean) AndAlso m.parameterTypes.Length = 1 AndAlso m.parameterTypes(0) Is GetType(Object))
			End Select
			Return False
		End Function

		Private Shared Function callObjectMethod(ByVal self As Object, ByVal m As Method, ByVal args As Object()) As Object
			assert(isObjectMethod(m)) : m
			Select Case m.name
			Case "toString"
				Return self.GetType().name & "@" &  java.lang.[Integer].toHexString(self.GetHashCode())
			Case "hashCode"
				Return System.identityHashCode(self)
			Case "equals"
				Return (self Is args(0))
			End Select
			Return Nothing
		End Function

		Private Shared Function getSingleNameMethods(ByVal intfc As [Class]) As Method()
			Dim methods As New List(Of Method)
			Dim uniqueName As String = Nothing
			For Each m As Method In intfc.methods
				If isObjectMethod(m) Then Continue For
				If Not Modifier.isAbstract(m.modifiers) Then Continue For
				Dim mname As String = m.name
				If uniqueName Is Nothing Then
					uniqueName = mname
				ElseIf Not uniqueName.Equals(mname) Then
					Return Nothing ' too many abstract methods
				End If
				methods.Add(m)
			Next m
			If uniqueName Is Nothing Then Return Nothing
			Return methods.ToArray()
		End Function
	End Class

End Namespace