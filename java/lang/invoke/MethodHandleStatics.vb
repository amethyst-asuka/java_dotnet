Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2011, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' This class consists exclusively of static names internal to the
	''' method handle implementation.
	''' Usage:  {@code import static java.lang.invoke.MethodHandleStatics.*}
	''' @author John Rose, JSR 292 EG
	''' </summary>
	'non-public
	 Friend Class MethodHandleStatics

		Private Sub New() ' do not instantiate
		End Sub

		Friend Shared ReadOnly UNSAFE As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

		Friend Shared ReadOnly DEBUG_METHOD_HANDLE_NAMES As Boolean
		Friend Shared ReadOnly DUMP_CLASS_FILES As Boolean
		Friend Shared ReadOnly TRACE_INTERPRETER As Boolean
		Friend Shared ReadOnly TRACE_METHOD_LINKAGE As Boolean
		Friend Shared ReadOnly COMPILE_THRESHOLD As Integer
		Friend Shared ReadOnly DONT_INLINE_THRESHOLD As Integer
		Friend Shared ReadOnly PROFILE_LEVEL As Integer
		Friend Shared ReadOnly PROFILE_GWT As Boolean
		Friend Shared ReadOnly CUSTOMIZE_THRESHOLD As Integer

		Shared Sub New()
			Dim values As Object() = New Object(8){}
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			DEBUG_METHOD_HANDLE_NAMES = CBool(values(0))
			DUMP_CLASS_FILES = CBool(values(1))
			TRACE_INTERPRETER = CBool(values(2))
			TRACE_METHOD_LINKAGE = CBool(values(3))
			COMPILE_THRESHOLD = CInt(Fix(values(4)))
			DONT_INLINE_THRESHOLD = CInt(Fix(values(5)))
			PROFILE_LEVEL = CInt(Fix(values(6)))
			PROFILE_GWT = CBool(values(7))
			CUSTOMIZE_THRESHOLD = CInt(Fix(values(8)))

			If CUSTOMIZE_THRESHOLD < -1 OrElse CUSTOMIZE_THRESHOLD > 127 Then Throw newInternalError("CUSTOMIZE_THRESHOLD should be in [-1...127] range")
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				values(0) = Boolean.getBoolean("java.lang.invoke.MethodHandle.DEBUG_NAMES")
				values(1) = Boolean.getBoolean("java.lang.invoke.MethodHandle.DUMP_CLASS_FILES")
				values(2) = Boolean.getBoolean("java.lang.invoke.MethodHandle.TRACE_INTERPRETER")
				values(3) = Boolean.getBoolean("java.lang.invoke.MethodHandle.TRACE_METHOD_LINKAGE")
				values(4) = Integer.getInteger("java.lang.invoke.MethodHandle.COMPILE_THRESHOLD", 0)
				values(5) = Integer.getInteger("java.lang.invoke.MethodHandle.DONT_INLINE_THRESHOLD", 30)
				values(6) = Integer.getInteger("java.lang.invoke.MethodHandle.PROFILE_LEVEL", 0)
				values(7) = Convert.ToBoolean(System.getProperty("java.lang.invoke.MethodHandle.PROFILE_GWT", "true"))
				values(8) = Integer.getInteger("java.lang.invoke.MethodHandle.CUSTOMIZE_THRESHOLD", 127)
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Tell if any of the debugging switches are turned on.
		'''  If this is the case, it is reasonable to perform extra checks or save extra information.
		''' </summary>
		'non-public
	 Friend Shared Function debugEnabled() As Boolean
			Return (DEBUG_METHOD_HANDLE_NAMES Or DUMP_CLASS_FILES Or TRACE_INTERPRETER Or TRACE_METHOD_LINKAGE)
	 End Function

		'non-public
	 Friend Shared Function getNameString(ByVal target As MethodHandle, ByVal type As MethodType) As String
			If type Is Nothing Then type = target.type()
			Dim name As MemberName = Nothing
			If target IsNot Nothing Then name = target.internalMemberName()
			If name Is Nothing Then Return "invoke" & type
			Return name.name + type
	 End Function

		'non-public
	 Friend Shared Function getNameString(ByVal target As MethodHandle, ByVal typeHolder As MethodHandle) As String
			Return getNameString(target,If(typeHolder Is Nothing, CType(Nothing, MethodType), typeHolder.type()))
	 End Function

		'non-public
	 Friend Shared Function getNameString(ByVal target As MethodHandle) As String
			Return getNameString(target, CType(Nothing, MethodType))
	 End Function

		'non-public
	 Friend Shared Function addTypeString(ByVal obj As Object, ByVal target As MethodHandle) As String
			Dim str As String = Convert.ToString(obj)
			If target Is Nothing Then Return str
			Dim paren As Integer = str.IndexOf("("c)
			If paren >= 0 Then str = str.Substring(0, paren)
			Return str + target.type()
	 End Function

		' handy shared exception makers (they simplify the common case code)
		'non-public
	 Friend Shared Function newInternalError(ByVal message As String) As InternalError
			Return New InternalError(message)
	 End Function
		'non-public
	 Friend Shared Function newInternalError(ByVal message As String, ByVal cause As Throwable) As InternalError
			Return New InternalError(message, cause)
	 End Function
		'non-public
	 Friend Shared Function newInternalError(ByVal cause As Throwable) As InternalError
			Return New InternalError(cause)
	 End Function
		'non-public
	 Friend Shared Function newIllegalStateException(ByVal message As String) As RuntimeException
			Return New IllegalStateException(message)
	 End Function
		'non-public
	 Friend Shared Function newIllegalStateException(ByVal message As String, ByVal obj As Object) As RuntimeException
			Return New IllegalStateException(message(message, obj))
	 End Function
		'non-public
	 Friend Shared Function newIllegalArgumentException(ByVal message As String) As RuntimeException
			Return New IllegalArgumentException(message)
	 End Function
		'non-public
	 Friend Shared Function newIllegalArgumentException(ByVal message As String, ByVal obj As Object) As RuntimeException
			Return New IllegalArgumentException(message(message, obj))
	 End Function
		'non-public
	 Friend Shared Function newIllegalArgumentException(ByVal message As String, ByVal obj As Object, ByVal obj2 As Object) As RuntimeException
			Return New IllegalArgumentException(message(message, obj, obj2))
	 End Function
		''' <summary>
		''' Propagate unchecked exceptions and errors, but wrap anything checked and throw that instead. </summary>
		'non-public
	 Friend Shared Function uncaughtException(ByVal ex As Throwable) As [Error]
			If TypeOf ex Is Error Then Throw CType(ex, [Error])
			If TypeOf ex Is RuntimeException Then Throw CType(ex, RuntimeException)
			Throw newInternalError("uncaught exception", ex)
	 End Function
		Friend Shared Function NYI() As [Error]
			Throw New AssertionError("NYI")
		End Function
		Private Shared Function message(ByVal message_Renamed As String, ByVal obj As Object) As String
			If obj IsNot Nothing Then message_Renamed = message_Renamed & ": " & obj
			Return message_Renamed
		End Function
		Private Shared Function message(ByVal message_Renamed As String, ByVal obj As Object, ByVal obj2 As Object) As String
			If obj IsNot Nothing OrElse obj2 IsNot Nothing Then message_Renamed = message_Renamed & ": " & obj & ", " & obj2
			Return message_Renamed
		End Function
	 End Class

End Namespace