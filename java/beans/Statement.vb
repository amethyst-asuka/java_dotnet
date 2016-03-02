Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans



	''' <summary>
	''' A <code>Statement</code> object represents a primitive statement
	''' in which a single method is applied to a target and
	''' a set of arguments - as in <code>"a.setFoo(b)"</code>.
	''' Note that where this example uses names
	''' to denote the target and its argument, a statement
	''' object does not require a name space and is constructed with
	''' the values themselves.
	''' The statement object associates the named method
	''' with its environment as a simple set of values:
	''' the target and an array of argument values.
	''' 
	''' @since 1.4
	''' 
	''' @author Philip Milne
	''' </summary>
	Public Class Statement

		Private Shared emptyArray As Object() = {}

		Friend Shared defaultExceptionListener As ExceptionListener = New ExceptionListenerAnonymousInnerClassHelper

		Private Class ExceptionListenerAnonymousInnerClassHelper
			Implements ExceptionListener

			Public Overridable Sub exceptionThrown(ByVal e As Exception) Implements ExceptionListener.exceptionThrown
				Console.Error.WriteLine(e)
				' e.printStackTrace();
				Console.Error.WriteLine("Continuing ...")
			End Sub
		End Class

		Private ReadOnly acc As java.security.AccessControlContext = java.security.AccessController.context
		Private ReadOnly target As Object
		Private ReadOnly methodName As String
		Private ReadOnly arguments As Object()
		Friend loader As  ClassLoader

		''' <summary>
		''' Creates a new <seealso cref="Statement"/> object
		''' for the specified target object to invoke the method
		''' specified by the name and by the array of arguments.
		''' <p>
		''' The {@code target} and the {@code methodName} values should not be {@code null}.
		''' Otherwise an attempt to execute this {@code Expression}
		''' will result in a {@code NullPointerException}.
		''' If the {@code arguments} value is {@code null},
		''' an empty array is used as the value of the {@code arguments} property.
		''' </summary>
		''' <param name="target">  the target object of this statement </param>
		''' <param name="methodName">  the name of the method to invoke on the specified target </param>
		''' <param name="arguments">  the array of arguments to invoke the specified method </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal target As Object, ByVal methodName As String, ByVal arguments As Object())
			Me.target = target
			Me.methodName = methodName
			Me.arguments = If(arguments Is Nothing, emptyArray, arguments.clone())
		End Sub

		''' <summary>
		''' Returns the target object of this statement.
		''' If this method returns {@code null},
		''' the <seealso cref="#execute"/> method
		''' throws a {@code NullPointerException}.
		''' </summary>
		''' <returns> the target object of this statement </returns>
		Public Overridable Property target As Object
			Get
				Return target
			End Get
		End Property

		''' <summary>
		''' Returns the name of the method to invoke.
		''' If this method returns {@code null},
		''' the <seealso cref="#execute"/> method
		''' throws a {@code NullPointerException}.
		''' </summary>
		''' <returns> the name of the method </returns>
		Public Overridable Property methodName As String
			Get
				Return methodName
			End Get
		End Property

		''' <summary>
		''' Returns the arguments for the method to invoke.
		''' The number of arguments and their types
		''' must match the method being  called.
		''' {@code null} can be used as a synonym of an empty array.
		''' </summary>
		''' <returns> the array of arguments </returns>
		Public Overridable Property arguments As Object()
			Get
				Return Me.arguments.clone()
			End Get
		End Property

		''' <summary>
		''' The {@code execute} method finds a method whose name is the same
		''' as the {@code methodName} property, and invokes the method on
		''' the target.
		''' 
		''' When the target's class defines many methods with the given name
		''' the implementation should choose the most specific method using
		''' the algorithm specified in the Java Language Specification
		''' (15.11). The dynamic class of the target and arguments are used
		''' in place of the compile-time type information and, like the
		''' <seealso cref="java.lang.reflect.Method"/> class itself, conversion between
		''' primitive values and their associated wrapper classes is handled
		''' internally.
		''' <p>
		''' The following method types are handled as special cases:
		''' <ul>
		''' <li>
		''' Static methods may be called by using a class object as the target.
		''' <li>
		''' The reserved method name "new" may be used to call a class's constructor
		''' as if all classes defined static "new" methods. Constructor invocations
		''' are typically considered {@code Expression}s rather than {@code Statement}s
		''' as they return a value.
		''' <li>
		''' The method names "get" and "set" defined in the <seealso cref="java.util.List"/>
		''' interface may also be applied to array instances, mapping to
		''' the static methods of the same name in the {@code Array} class.
		''' </ul>
		''' </summary>
		''' <exception cref="NullPointerException"> if the value of the {@code target} or
		'''                              {@code methodName} property is {@code null} </exception>
		''' <exception cref="NoSuchMethodException"> if a matching method is not found </exception>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''                           it denies the method invocation </exception>
		''' <exception cref="Exception"> that is thrown by the invoked method
		''' </exception>
		''' <seealso cref= java.lang.reflect.Method </seealso>
		Public Overridable Sub execute()
			invoke()
		End Sub

		Friend Overridable Function invoke() As Object
			Dim acc As java.security.AccessControlContext = Me.acc
			If (acc Is Nothing) AndAlso (System.securityManager IsNot Nothing) Then Throw New SecurityException("AccessControlContext is not set")
			Try
				Return java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
						acc)
			Catch exception_Renamed As java.security.PrivilegedActionException
				Throw exception_Renamed.exception
			End Try
		End Function

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Object
				Return outerInstance.invokeInternal()
			End Function
		End Class

		Private Function invokeInternal() As Object
			Dim target_Renamed As Object = target
			Dim methodName_Renamed As String = methodName

			If target_Renamed Is Nothing OrElse methodName_Renamed Is Nothing Then Throw New NullPointerException((If(target_Renamed Is Nothing, "target", "methodName")) & " should not be null")

			Dim arguments_Renamed As Object() = arguments
			If arguments_Renamed Is Nothing Then arguments_Renamed = emptyArray
			' Class.forName() won't load classes outside
			' of core from a class inside core. Special
			' case this method.
			If target_Renamed Is GetType(Class) AndAlso methodName_Renamed.Equals("forName") Then Return com.sun.beans.finder.ClassFinder.resolveClass(CStr(arguments_Renamed(0)), Me.loader)
			Dim argClasses As  [Class]() = New [Class](arguments_Renamed.Length - 1){}
			For i As Integer = 0 To arguments_Renamed.Length - 1
				argClasses(i) = If(arguments_Renamed(i) Is Nothing, Nothing, arguments_Renamed(i).GetType())
			Next i

			Dim m As AccessibleObject = Nothing
			If TypeOf target_Renamed Is Class Then
	'            
	'            For class methods, simluate the effect of a meta class
	'            by taking the union of the static methods of the
	'            actual [Class], with the instance methods of "Class.class"
	'            and the overloaded "newInstance" methods defined by the
	'            constructors.
	'            This way "System.class", for example, will perform both
	'            the static method getProperties() and the instance method
	'            getSuperclass() defined in "Class.class".
	'            
				If methodName_Renamed.Equals("new") Then methodName_Renamed = "newInstance"
				' Provide a short form for array instantiation by faking an nary-constructor.
				If methodName_Renamed.Equals("newInstance") AndAlso CType(target_Renamed, [Class]).array Then
					Dim result As Object = Array.newInstance(CType(target_Renamed, [Class]).componentType, arguments_Renamed.Length)
					For i As Integer = 0 To arguments_Renamed.Length - 1
						Array.set(result, i, arguments_Renamed(i))
					Next i
					Return result
				End If
				If methodName_Renamed.Equals("newInstance") AndAlso arguments_Renamed.Length <> 0 Then
					' The Character [Class], as of 1.4, does not have a constructor
					' which takes a String. All of the other "wrapper" classes
					' for Java's primitive types have a String constructor so we
					' fake such a constructor here so that this special case can be
					' ignored elsewhere.
					If target_Renamed Is GetType(Character) AndAlso arguments_Renamed.Length = 1 AndAlso argClasses(0) Is GetType(String) Then Return New Character(CStr(arguments_Renamed(0)).Chars(0))
					Try
						m = com.sun.beans.finder.ConstructorFinder.findConstructor(CType(target_Renamed, [Class]), argClasses)
					Catch exception_Renamed As NoSuchMethodException
						m = Nothing
					End Try
				End If
				If m Is Nothing AndAlso target_Renamed IsNot GetType(Class) Then m = getMethod(CType(target_Renamed, [Class]), methodName_Renamed, argClasses)
				If m Is Nothing Then m = getMethod(GetType(Class), methodName_Renamed, argClasses)
			Else
	'            
	'            This special casing of arrays is not necessary, but makes files
	'            involving arrays much shorter and simplifies the archiving infrastrcure.
	'            The Array.set() method introduces an unusual idea - that of a static method
	'            changing the state of an instance. Normally statements with side
	'            effects on objects are instance methods of the objects themselves
	'            and we reinstate this rule (perhaps temporarily) by special-casing arrays.
	'            
				If target_Renamed.GetType().IsArray AndAlso (methodName_Renamed.Equals("set") OrElse methodName_Renamed.Equals("get")) Then
					Dim index As Integer = CInt(Fix(arguments_Renamed(0)))
					If methodName_Renamed.Equals("get") Then
						Return Array.get(target_Renamed, index)
					Else
						Array.set(target_Renamed, index, arguments_Renamed(1))
						Return Nothing
					End If
				End If
				m = getMethod(target_Renamed.GetType(), methodName_Renamed, argClasses)
			End If
			If m IsNot Nothing Then
				Try
					If TypeOf m Is Method Then
						Return sun.reflect.misc.MethodUtil.invoke(CType(m, Method), target_Renamed, arguments_Renamed)
					Else
						Return CType(m, Constructor).newInstance(arguments_Renamed)
					End If
				Catch iae As IllegalAccessException
					Throw New Exception("Statement cannot invoke: " & methodName_Renamed & " on " & target_Renamed.GetType(), iae)
				Catch ite As InvocationTargetException
					Dim te As Throwable = ite.targetException
					If TypeOf te Is Exception Then
						Throw CType(te, Exception)
					Else
						Throw ite
					End If
				End Try
			End If
			Throw New NoSuchMethodException(ToString())
		End Function

		Friend Overridable Function instanceName(ByVal instance As Object) As String
			If instance Is Nothing Then
				Return "null"
			ElseIf instance.GetType() Is GetType(String) Then
				Return """" & CStr(instance) & """"
			Else
				' Note: there is a minor problem with using the non-caching
				' NameGenerator method. The return value will not have
				' specific information about the inner class name. For example,
				' In 1.4.2 an inner class would be represented as JList$1 now
				' would be named Class.

				Return NameGenerator.unqualifiedClassName(instance.GetType())
			End If
		End Function

		''' <summary>
		''' Prints the value of this statement using a Java-style syntax.
		''' </summary>
		Public Overrides Function ToString() As String
			' Respect a subclass's implementation here.
			Dim target_Renamed As Object = target
			Dim methodName_Renamed As String = methodName
			Dim arguments_Renamed As Object() = arguments
			If arguments_Renamed Is Nothing Then arguments_Renamed = emptyArray
			Dim result As New StringBuffer(instanceName(target_Renamed) & "." & methodName_Renamed & "(")
			Dim n As Integer = arguments_Renamed.Length
			For i As Integer = 0 To n - 1
				result.append(instanceName(arguments_Renamed(i)))
				If i <> n -1 Then result.append(", ")
			Next i
			result.append(");")
			Return result.ToString()
		End Function

		Friend Shared Function getMethod(ByVal type As [Class], ByVal name As String, ParamArray ByVal args As  [Class]()) As Method
			Try
				Return com.sun.beans.finder.MethodFinder.findMethod(type, name, args)
			Catch exception_Renamed As NoSuchMethodException
				Return Nothing
			End Try
		End Function
	End Class

End Namespace