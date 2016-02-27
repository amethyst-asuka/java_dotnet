Imports System
Imports System.Runtime.InteropServices

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
	''' The JVM interface for the method handles package is all here.
	''' This is an interface internal and private to an implementation of JSR 292.
	''' <em>This class is not part of the JSR 292 standard.</em>
	''' @author jrose
	''' </summary>
	Friend Class MethodHandleNatives

		Private Sub New() ' static only
		End Sub

		'/ MemberName support

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub init(ByVal self As MemberName, ByVal ref As Object)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub expand(ByVal self As MemberName)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function resolve(ByVal self As MemberName, ByVal caller As [Class]) As MemberName
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function getMembers(ByVal defc As [Class], ByVal matchName As String, ByVal matchSig As String, ByVal matchFlags As Integer, ByVal caller As [Class], ByVal skip As Integer, ByVal results As MemberName()) As Integer
		End Function

		'/ Field layout queries parallel to sun.misc.Unsafe:
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function objectFieldOffset(ByVal self As MemberName) As Long
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function staticFieldOffset(ByVal self As MemberName) As Long
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function staticFieldBase(ByVal self As MemberName) As Object
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function getMemberVMInfo(ByVal self As MemberName) As Object
		End Function

		'/ MethodHandle support

		''' <summary>
		''' Fetch MH-related JVM parameter.
		'''  which=0 retrieves MethodHandlePushLimit
		'''  which=1 retrieves stack slot push size (in address units)
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function getConstant(ByVal which As Integer) As Integer
		End Function

		Friend Shared ReadOnly COUNT_GWT As Boolean

		'/ CallSite support

		''' <summary>
		''' Tell the JVM that we need to change the target of a CallSite. </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub setCallSiteTargetNormal(ByVal site As CallSite, ByVal target As MethodHandle)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub setCallSiteTargetVolatile(ByVal site As CallSite, ByVal target As MethodHandle)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub registerNatives()
		End Sub
		Shared Sub New()
			registerNatives()
			COUNT_GWT = getConstant(Constants.GC_COUNT_GWT) <> 0

			' The JVM calls MethodHandleNatives.<clinit>.  Cascade the <clinit> calls as needed:
			MethodHandleImpl.initStatics()
			Dim HR_MASK As Integer = ((1 << REF_getField) Or (1 << REF_putField) Or (1 << REF_invokeVirtual) Or (1 << REF_invokeSpecial) Or (1 << REF_invokeInterface))
			For refKind As SByte = REF_NONE+1 To REF_LIMIT - 1
				assert(refKindHasReceiver(refKind) = (((1<<refKind) And HR_MASK) <> 0)) : refKind
			Next refKind
			assert(verifyConstants())
		End Sub

		' All compile-time constants go here.
		' There is an opportunity to check them against the JVM's idea of them.
		Friend Class Constants
			Friend Sub New() ' static only
			End Sub
			' MethodHandleImpl
			Friend Const GC_COUNT_GWT As Integer = 4, GC_LAMBDA_SUPPORT As Integer = 5 ' for getConstant

			' MemberName
			' The JVM uses values of -2 and above for vtable indexes.
			' Field values are simple positive offsets.
			' Ref: src/share/vm/oops/methodOop.hpp
			' This value is negative enough to avoid such numbers,
			' but not too negative.
			Friend Const MN_IS_METHOD As Integer = &H10000, MN_IS_CONSTRUCTOR As Integer = &H20000, MN_IS_FIELD As Integer = &H40000, MN_IS_TYPE As Integer = &H80000, MN_CALLER_SENSITIVE As Integer = &H100000, MN_REFERENCE_KIND_SHIFT As Integer = 24, MN_REFERENCE_KIND_MASK As Integer = &HF000000 >> MN_REFERENCE_KIND_SHIFT, MN_SEARCH_SUPERCLASSES As Integer = &H100000, MN_SEARCH_INTERFACES As Integer = &H200000 ' refKind -  @CallerSensitive annotation detected -  nested type -  field -  constructor -  method (not constructor)
					' The SEARCH_* bits are not for MN.flags but for the matchFlags argument of MHN.getMembers:

			''' <summary>
			''' Basic types as encoded in the JVM.  These code values are not
			''' intended for use outside this class.  They are used as part of
			''' a private interface between the JVM and this class.
			''' </summary>
			Friend Const T_BOOLEAN As Integer = 4, T_CHAR As Integer = 5, T_FLOAT As Integer = 6, T_DOUBLE As Integer = 7, T_BYTE As Integer = 8, T_SHORT As Integer = 9, T_INT As Integer = 10, T_LONG As Integer = 11, T_OBJECT As Integer = 12, T_VOID As Integer = 14, T_ILLEGAL As Integer = 99
				'T_ARRAY    = 13
				'T_ADDRESS  = 15

			''' <summary>
			''' Constant pool entry types.
			''' </summary>
			Friend Const CONSTANT_Utf8 As SByte = 1, CONSTANT_Integer As SByte = 3, CONSTANT_Float As SByte = 4, CONSTANT_Long As SByte = 5, CONSTANT_Double As SByte = 6, CONSTANT_Class As SByte = 7, CONSTANT_String As SByte = 8, CONSTANT_Fieldref As SByte = 9, CONSTANT_Methodref As SByte = 10, CONSTANT_InterfaceMethodref As SByte = 11, CONSTANT_NameAndType As SByte = 12, CONSTANT_MethodHandle As SByte = 15, CONSTANT_MethodType As SByte = 16, CONSTANT_InvokeDynamic As SByte = 18, CONSTANT_LIMIT As SByte = 19 ' Limit to tags found in classfiles -  JSR 292 -  JSR 292

			''' <summary>
			''' Access modifier flags.
			''' </summary>
			Friend Const ACC_PUBLIC As Char = &H1, ACC_PRIVATE As Char = &H2, ACC_PROTECTED As Char = &H4, ACC_STATIC As Char = &H8, ACC_FINAL As Char = &H10, ACC_SYNCHRONIZED As Char = &H20, ACC_VOLATILE As Char = &H40, ACC_TRANSIENT As Char = &H80, ACC_NATIVE As Char = &H100, ACC_INTERFACE As Char = &H200, ACC_ABSTRACT As Char = &H400, ACC_STRICT As Char = &H800, ACC_SYNTHETIC As Char = &H1000, ACC_ANNOTATION As Char = &H2000, ACC_ENUM As Char = &H4000, ACC_SUPER As Char = ACC_SYNCHRONIZED, ACC_BRIDGE As Char = ACC_VOLATILE, ACC_VARARGS As Char = ACC_TRANSIENT
				' aliases:

			''' <summary>
			''' Constant pool reference-kind codes, as used by CONSTANT_MethodHandle CP entries.
			''' </summary>
			Friend Const REF_NONE As SByte = 0, REF_getField As SByte = 1, REF_getStatic As SByte = 2, REF_putField As SByte = 3, REF_putStatic As SByte = 4, REF_invokeVirtual As SByte = 5, REF_invokeStatic As SByte = 6, REF_invokeSpecial As SByte = 7, REF_newInvokeSpecial As SByte = 8, REF_invokeInterface As SByte = 9, REF_LIMIT As SByte = 10 ' null value
		End Class

		Friend Shared Function refKindIsValid(ByVal refKind As Integer) As Boolean
			Return (refKind > REF_NONE AndAlso refKind < REF_LIMIT)
		End Function
		Friend Shared Function refKindIsField(ByVal refKind As SByte) As Boolean
			assert(refKindIsValid(refKind))
			Return (refKind <= REF_putStatic)
		End Function
		Friend Shared Function refKindIsGetter(ByVal refKind As SByte) As Boolean
			assert(refKindIsValid(refKind))
			Return (refKind <= REF_getStatic)
		End Function
		Friend Shared Function refKindIsSetter(ByVal refKind As SByte) As Boolean
			Return refKindIsField(refKind) AndAlso Not refKindIsGetter(refKind)
		End Function
		Friend Shared Function refKindIsMethod(ByVal refKind As SByte) As Boolean
			Return (Not refKindIsField(refKind)) AndAlso (refKind <> REF_newInvokeSpecial)
		End Function
		Friend Shared Function refKindIsConstructor(ByVal refKind As SByte) As Boolean
			Return (refKind = REF_newInvokeSpecial)
		End Function
		Friend Shared Function refKindHasReceiver(ByVal refKind As SByte) As Boolean
			assert(refKindIsValid(refKind))
			Return (refKind And 1) <> 0
		End Function
		Friend Shared Function refKindIsStatic(ByVal refKind As SByte) As Boolean
			Return (Not refKindHasReceiver(refKind)) AndAlso (refKind <> REF_newInvokeSpecial)
		End Function
		Friend Shared Function refKindDoesDispatch(ByVal refKind As SByte) As Boolean
			assert(refKindIsValid(refKind))
			Return (refKind = REF_invokeVirtual OrElse refKind = REF_invokeInterface)
		End Function
		Friend Shared Function refKindName(ByVal refKind As SByte) As String
			assert(refKindIsValid(refKind))
			Select Case refKind
			Case REF_getField
				Return "getField"
			Case REF_getStatic
				Return "getStatic"
			Case REF_putField
				Return "putField"
			Case REF_putStatic
				Return "putStatic"
			Case REF_invokeVirtual
				Return "invokeVirtual"
			Case REF_invokeStatic
				Return "invokeStatic"
			Case REF_invokeSpecial
				Return "invokeSpecial"
			Case REF_newInvokeSpecial
				Return "newInvokeSpecial"
			Case REF_invokeInterface
				Return "invokeInterface"
			Case Else
				Return "REF_???"
			End Select
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getNamedCon(ByVal which As Integer, ByVal name As Object()) As Integer
		End Function
		Friend Shared Function verifyConstants() As Boolean
			Dim box As Object() = { Nothing }
			Dim i As Integer = 0
			Do
				box(0) = Nothing
				Dim vmval As Integer = getNamedCon(i, box)
				If box(0) Is Nothing Then Exit Do
				Dim name As String = CStr(box(0))
				Try
					Dim con As Field = GetType(Constants).getDeclaredField(name)
					Dim jval As Integer = con.getInt(Nothing)
					If jval = vmval Then
						i += 1
						Continue Do
					End If
					Dim err As String = (name & ": JVM has " & vmval & " while Java has " & jval)
					If name.Equals("CONV_OP_LIMIT") Then
						Console.Error.WriteLine("warning: " & err)
						i += 1
						Continue Do
					End If
					Throw New InternalError(err)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch NoSuchFieldException Or IllegalAccessException ex
					Dim err As String = (name & ": JVM has " & vmval & " which Java does not define")
					' ignore exotic ops the JVM cares about; we just wont issue them
					'System.err.println("warning: "+err);
					i += 1
					Continue Do
				End Try
				i += 1
			Loop
			Return True
		End Function

		' Up-calls from the JVM.
		' These must NOT be public.

		''' <summary>
		''' The JVM is linking an invokedynamic instruction.  Create a reified call site for it.
		''' </summary>
		Friend Shared Function linkCallSite(ByVal callerObj As Object, ByVal bootstrapMethodObj As Object, ByVal nameObj As Object, ByVal typeObj As Object, ByVal staticArguments As Object, ByVal appendixResult As Object()) As MemberName
			Dim bootstrapMethod As MethodHandle = CType(bootstrapMethodObj, MethodHandle)
			Dim caller As  [Class] = CType(callerObj, [Class])
			Dim name As String = nameObj.ToString().intern()
			Dim type As MethodType = CType(typeObj, MethodType)
			If Not TRACE_METHOD_LINKAGE Then Return linkCallSiteImpl(caller, bootstrapMethod, name, type, staticArguments, appendixResult)
			Return linkCallSiteTracing(caller, bootstrapMethod, name, type, staticArguments, appendixResult)
		End Function
		Friend Shared Function linkCallSiteImpl(ByVal caller As [Class], ByVal bootstrapMethod As MethodHandle, ByVal name As String, ByVal type As MethodType, ByVal staticArguments As Object, ByVal appendixResult As Object()) As MemberName
			Dim callSite_Renamed As CallSite = CallSite.makeSite(bootstrapMethod, name, type, staticArguments, caller)
			If TypeOf callSite_Renamed Is ConstantCallSite Then
				appendixResult(0) = callSite_Renamed.dynamicInvoker()
				Return Invokers.linkToTargetMethod(type)
			Else
				appendixResult(0) = callSite_Renamed
				Return Invokers.linkToCallSiteMethod(type)
			End If
		End Function
		' Tracing logic:
		Friend Shared Function linkCallSiteTracing(ByVal caller As [Class], ByVal bootstrapMethod As MethodHandle, ByVal name As String, ByVal type As MethodType, ByVal staticArguments As Object, ByVal appendixResult As Object()) As MemberName
			Dim bsmReference As Object = bootstrapMethod.internalMemberName()
			If bsmReference Is Nothing Then bsmReference = bootstrapMethod
			Dim staticArglist As Object = (If(TypeOf staticArguments Is Object(), CType(staticArguments, Object()), staticArguments))
			Console.WriteLine("linkCallSite " & caller.name & " " & bsmReference & " " & name+type & "/" & staticArglist)
			Try
				Dim res As MemberName = linkCallSiteImpl(caller, bootstrapMethod, name, type, staticArguments, appendixResult)
				Console.WriteLine("linkCallSite => " & res & " + " & appendixResult(0))
				Return res
			Catch ex As Throwable
				Console.WriteLine("linkCallSite => throw " & ex)
				Throw ex
			End Try
		End Function

		''' <summary>
		''' The JVM wants a pointer to a MethodType.  Oblige it by finding or creating one.
		''' </summary>
		Friend Shared Function findMethodHandleType(ByVal rtype As [Class], ByVal ptypes As  [Class]()) As MethodType
			Return MethodType.makeImpl(rtype, ptypes, True)
		End Function

		''' <summary>
		''' The JVM wants to link a call site that requires a dynamic type check.
		''' Name is a type-checking invoker, invokeExact or invoke.
		''' Return a JVM method (MemberName) to handle the invoking.
		''' The method assumes the following arguments on the stack:
		''' 0: the method handle being invoked
		''' 1-N: the arguments to the method handle invocation
		''' N+1: an optional, implicitly added argument (typically the given MethodType)
		''' <p>
		''' The nominal method at such a call site is an instance of
		''' a signature-polymorphic method (see @PolymorphicSignature).
		''' Such method instances are user-visible entities which are
		''' "split" from the generic placeholder method in {@code MethodHandle}.
		''' (Note that the placeholder method is not identical with any of
		''' its instances.  If invoked reflectively, is guaranteed to throw an
		''' {@code UnsupportedOperationException}.)
		''' If the signature-polymorphic method instance is ever reified,
		''' it appears as a "copy" of the original placeholder
		''' (a native final member of {@code MethodHandle}) except
		''' that its type descriptor has shape required by the instance,
		''' and the method instance is <em>not</em> varargs.
		''' The method instance is also marked synthetic, since the
		''' method (by definition) does not appear in Java source code.
		''' <p>
		''' The JVM is allowed to reify this method as instance metadata.
		''' For example, {@code invokeBasic} is always reified.
		''' But the JVM may instead call {@code linkMethod}.
		''' If the result is an * ordered pair of a {@code (method, appendix)},
		''' the method gets all the arguments (0..N inclusive)
		''' plus the appendix (N+1), and uses the appendix to complete the call.
		''' In this way, one reusable method (called a "linker method")
		''' can perform the function of any number of polymorphic instance
		''' methods.
		''' <p>
		''' Linker methods are allowed to be weakly typed, with any or
		''' all references rewritten to {@code Object} and any primitives
		''' (except {@code long}/{@code float}/{@code double})
		''' rewritten to {@code int}.
		''' A linker method is trusted to return a strongly typed result,
		''' according to the specific method type descriptor of the
		''' signature-polymorphic instance it is emulating.
		''' This can involve (as necessary) a dynamic check using
		''' data extracted from the appendix argument.
		''' <p>
		''' The JVM does not inspect the appendix, other than to pass
		''' it verbatim to the linker method at every call.
		''' This means that the JDK runtime has wide latitude
		''' for choosing the shape of each linker method and its
		''' corresponding appendix.
		''' Linker methods should be generated from {@code LambdaForm}s
		''' so that they do not become visible on stack traces.
		''' <p>
		''' The {@code linkMethod} call is free to omit the appendix
		''' (returning null) and instead emulate the required function
		''' completely in the linker method.
		''' As a corner case, if N==255, no appendix is possible.
		''' In this case, the method returned must be custom-generated to
		''' to perform any needed type checking.
		''' <p>
		''' If the JVM does not reify a method at a call site, but instead
		''' calls {@code linkMethod}, the corresponding call represented
		''' in the bytecodes may mention a valid method which is not
		''' representable with a {@code MemberName}.
		''' Therefore, use cases for {@code linkMethod} tend to correspond to
		''' special cases in reflective code such as {@code findVirtual}
		''' or {@code revealDirect}.
		''' </summary>
		Friend Shared Function linkMethod(ByVal callerClass As [Class], ByVal refKind As Integer, ByVal defc As [Class], ByVal name As String, ByVal type As Object, ByVal appendixResult As Object()) As MemberName
			If Not TRACE_METHOD_LINKAGE Then Return linkMethodImpl(callerClass, refKind, defc, name, type, appendixResult)
			Return linkMethodTracing(callerClass, refKind, defc, name, type, appendixResult)
		End Function
		Friend Shared Function linkMethodImpl(ByVal callerClass As [Class], ByVal refKind As Integer, ByVal defc As [Class], ByVal name As String, ByVal type As Object, ByVal appendixResult As Object()) As MemberName
			Try
				If defc Is GetType(MethodHandle) AndAlso refKind = REF_invokeVirtual Then Return Invokers.methodHandleInvokeLinkerMethod(name, fixMethodType(callerClass, type), appendixResult)
			Catch ex As Throwable
				If TypeOf ex Is LinkageError Then
					Throw CType(ex, LinkageError)
				Else
					Throw New LinkageError(ex.message, ex)
				End If
			End Try
			Throw New LinkageError("no such method " & defc.name & "." & name+type)
		End Function
		Private Shared Function fixMethodType(ByVal callerClass As [Class], ByVal type As Object) As MethodType
			If TypeOf type Is MethodType Then
				Return CType(type, MethodType)
			Else
				Return MethodType.fromMethodDescriptorString(CStr(type), callerClass.classLoader)
			End If
		End Function
		' Tracing logic:
		Friend Shared Function linkMethodTracing(ByVal callerClass As [Class], ByVal refKind As Integer, ByVal defc As [Class], ByVal name As String, ByVal type As Object, ByVal appendixResult As Object()) As MemberName
			Console.WriteLine("linkMethod " & defc.name & "." & name+type & "/" &  java.lang.[Integer].toHexString(refKind))
			Try
				Dim res As MemberName = linkMethodImpl(callerClass, refKind, defc, name, type, appendixResult)
				Console.WriteLine("linkMethod => " & res & " + " & appendixResult(0))
				Return res
			Catch ex As Throwable
				Console.WriteLine("linkMethod => throw " & ex)
				Throw ex
			End Try
		End Function


		''' <summary>
		''' The JVM is resolving a CONSTANT_MethodHandle CP entry.  And it wants our help.
		''' It will make an up-call to this method.  (Do not change the name or signature.)
		''' The type argument is a Class for field requests and a MethodType for non-fields.
		''' <p>
		''' Recent versions of the JVM may also pass a resolved MemberName for the type.
		''' In that case, the name is ignored and may be null.
		''' </summary>
		Friend Shared Function linkMethodHandleConstant(ByVal callerClass As [Class], ByVal refKind As Integer, ByVal defc As [Class], ByVal name As String, ByVal type As Object) As MethodHandle
			Try
				Dim lookup As Lookup = IMPL_LOOKUP.in(callerClass)
				assert(refKindIsValid(refKind))
				Return lookup.linkMethodHandleConstant(CByte(refKind), defc, name, type)
			Catch ex As IllegalAccessException
				Dim cause As Throwable = ex.InnerException
				If TypeOf cause Is AbstractMethodError Then
					Throw CType(cause, AbstractMethodError)
				Else
					Dim err As [Error] = New IllegalAccessError(ex.Message)
					Throw initCauseFrom(err, ex)
				End If
			Catch ex As NoSuchMethodException
				Dim err As [Error] = New NoSuchMethodError(ex.Message)
				Throw initCauseFrom(err, ex)
			Catch ex As NoSuchFieldException
				Dim err As [Error] = New NoSuchFieldError(ex.Message)
				Throw initCauseFrom(err, ex)
			Catch ex As ReflectiveOperationException
				Dim err As [Error] = New IncompatibleClassChangeError
				Throw initCauseFrom(err, ex)
			End Try
		End Function

		''' <summary>
		''' Use best possible cause for err.initCause(), substituting the
		''' cause for err itself if the cause has the same (or better) type.
		''' </summary>
		Private Shared Function initCauseFrom(ByVal err As [Error], ByVal ex As Exception) As [Error]
			Dim th As Throwable = ex.cause
			If err.GetType().IsInstanceOfType(th) Then Return CType(th, [Error])
			err.initCause(If(th Is Nothing, ex, th))
			Return err
		End Function

		''' <summary>
		''' Is this method a caller-sensitive method?
		''' I.e., does it call Reflection.getCallerClass or a similer method
		''' to ask about the identity of its caller?
		''' </summary>
		Friend Shared Function isCallerSensitive(ByVal mem As MemberName) As Boolean
			If Not mem.invocable Then ' fields are not caller sensitive Return False

			Return mem.callerSensitive OrElse canBeCalledVirtual(mem)
		End Function

		Friend Shared Function canBeCalledVirtual(ByVal mem As MemberName) As Boolean
			assert(mem.invocable)
			Dim defc As  [Class] = mem.declaringClass
			Select Case mem.name
			Case "checkMemberAccess"
				Return canBeCalledVirtual(mem, GetType(java.lang.SecurityManager))
			Case "getContextClassLoader"
				Return canBeCalledVirtual(mem, GetType(Thread))
			End Select
			Return False
		End Function

		Friend Shared Function canBeCalledVirtual(ByVal symbolicRef As MemberName, ByVal definingClass As [Class]) As Boolean
			Dim symbolicRefClass As  [Class] = symbolicRef.declaringClass
			If symbolicRefClass Is definingClass Then Return True
			If symbolicRef.static OrElse symbolicRef.private Then Return False
			Return (symbolicRefClass.IsSubclassOf(definingClass) OrElse symbolicRefClass.interface) ' Mdef implements Msym -  Msym overrides Mdef
		End Function
	End Class

End Namespace