Imports Microsoft.VisualBasic
Imports System
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
	''' The flavor of method handle which implements a constant reference
	''' to a class member.
	''' @author jrose
	''' </summary>
	Friend Class DirectMethodHandle
		Inherits MethodHandle

		Friend ReadOnly member As MemberName

		' Constructors and factory methods in this class *must* be package scoped or private.
		Private Sub New(ByVal mtype As MethodType, ByVal form As LambdaForm, ByVal member As MemberName)
			MyBase.New(mtype, form)
			If Not member.resolved Then Throw New InternalError

			If member.declaringClass.interface AndAlso member.method AndAlso (Not member.abstract) Then
				' Check for corner case: invokeinterface of Object method
				Dim m As New MemberName(GetType(Object), member.name, member.methodType, member.referenceKind)
				m = MemberName.factory.resolveOrNull(m.referenceKind, m, Nothing)
				If m IsNot Nothing AndAlso m.public Then
					assert(member.referenceKind = m.referenceKind) ' else this.form is wrong
					member = m
				End If
			End If

			Me.member = member
		End Sub

		' Factory methods:
		Shared Function make(ByVal refKind As SByte, ByVal receiver As [Class], ByVal member As MemberName) As DirectMethodHandle
			Dim mtype As MethodType = member.methodOrFieldType
			If Not member.static Then
				If (Not receiver.IsSubclassOf(member.declaringClass)) OrElse member.constructor Then Throw New InternalError(member.ToString())
				mtype = mtype.insertParameterTypes(0, receiver)
			End If
			If Not member.field Then
				If refKind = REF_invokeSpecial Then
					member = member.asSpecial()
					Dim lform As LambdaForm = preparedLambdaForm(member)
					Return New Special(mtype, lform, member)
				Else
					Dim lform As LambdaForm = preparedLambdaForm(member)
					Return New DirectMethodHandle(mtype, lform, member)
				End If
			Else
				Dim lform As LambdaForm = preparedFieldLambdaForm(member)
				If member.static Then
					Dim offset As Long = MethodHandleNatives.staticFieldOffset(member)
					Dim base As Object = MethodHandleNatives.staticFieldBase(member)
					Return New StaticAccessor(mtype, lform, member, base, offset)
				Else
					Dim offset As Long = MethodHandleNatives.objectFieldOffset(member)
					assert(offset = CInt(offset))
					Return New Accessor(mtype, lform, member, CInt(offset))
				End If
			End If
		End Function
		Shared Function make(ByVal receiver As [Class], ByVal member As MemberName) As DirectMethodHandle
			Dim refKind As SByte = member.referenceKind
			If refKind = REF_invokeSpecial Then refKind = REF_invokeVirtual
			Return make(refKind, receiver, member)
		End Function
		Shared Function make(ByVal member As MemberName) As DirectMethodHandle
			If member.constructor Then Return makeAllocator(member)
			Return make(member.declaringClass, member)
		End Function
		Shared Function make(ByVal method As Method) As DirectMethodHandle
			Return make(method.declaringClass, New MemberName(method))
		End Function
		Shared Function make(ByVal field As Field) As DirectMethodHandle
			Return make(field.declaringClass, New MemberName(field))
		End Function
		Private Shared Function makeAllocator(ByVal ctor As MemberName) As DirectMethodHandle
			assert(ctor.constructor AndAlso ctor.name.Equals("<init>"))
			Dim instanceClass As  [Class] = ctor.declaringClass
			ctor = ctor.asConstructor()
			assert(ctor.constructor AndAlso ctor.referenceKind = REF_newInvokeSpecial) : ctor
			Dim mtype As MethodType = ctor.methodType.changeReturnType(instanceClass)
			Dim lform As LambdaForm = preparedLambdaForm(ctor)
			Dim init As MemberName = ctor.asSpecial()
			assert(init.methodType.returnType() Is GetType(void))
			Return New Constructor(mtype, lform, ctor, init, instanceClass)
		End Function

		Friend Overrides Function rebind() As BoundMethodHandle
			Return BoundMethodHandle.makeReinvoker(Me)
		End Function

		Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
			assert(Me.GetType() Is GetType(DirectMethodHandle)) ' must override in subclasses
			Return New DirectMethodHandle(mt, lf, member)
		End Function

		Friend Overrides Function internalProperties() As String
			Return vbLf & "& DMH.MN=" & internalMemberName()
		End Function

		'// Implementation methods.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function internalMemberName() As MemberName
			Return member
		End Function

		Private Shared ReadOnly IMPL_NAMES As MemberName.Factory = MemberName.factory

		''' <summary>
		''' Create a LF which can invoke the given method.
		''' Cache and share this structure among all methods with
		''' the same basicType and refKind.
		''' </summary>
		Private Shared Function preparedLambdaForm(ByVal m As MemberName) As LambdaForm
			assert(m.invocable) : m ' call preparedFieldLambdaForm instead
			Dim mtype As MethodType = m.invocationType.basicType()
			assert((Not m.methodHandleInvoke) OrElse "invokeBasic".Equals(m.name)) : m
			Dim which As Integer
			Select Case m.referenceKind
			Case REF_invokeVirtual
				which = LF_INVVIRTUAL
			Case REF_invokeStatic
				which = LF_INVSTATIC
			Case REF_invokeSpecial
				which = LF_INVSPECIAL
			Case REF_invokeInterface
				which = LF_INVINTERFACE
			Case REF_newInvokeSpecial
				which = LF_NEWINVSPECIAL
			Case Else
				Throw New InternalError(m.ToString())
			End Select
			If which = LF_INVSTATIC AndAlso shouldBeInitialized(m) Then
				' precompute the barrier-free version:
				preparedLambdaForm(mtype, which)
				which = LF_INVSTATIC_INIT
			End If
			Dim lform As LambdaForm = preparedLambdaForm(mtype, which)
			maybeCompile(lform, m)
			assert(lform.methodType().dropParameterTypes(0, 1).Equals(m.invocationType.basicType())) : java.util.Arrays.asList(m, m.invocationType.basicType(), lform, lform.methodType())
			Return lform
		End Function

		Private Shared Function preparedLambdaForm(ByVal mtype As MethodType, ByVal which As Integer) As LambdaForm
			Dim lform As LambdaForm = mtype.form().cachedLambdaForm(which)
			If lform IsNot Nothing Then Return lform
			lform = makePreparedLambdaForm(mtype, which)
			Return mtype.form().cachedLambdaFormorm(which, lform)
		End Function

		Private Shared Function makePreparedLambdaForm(ByVal mtype As MethodType, ByVal which As Integer) As LambdaForm
			Dim needsInit As Boolean = (which = LF_INVSTATIC_INIT)
			Dim doesAlloc As Boolean = (which = LF_NEWINVSPECIAL)
			Dim linkerName, lambdaName As String
			Select Case which
			Case LF_INVVIRTUAL
				linkerName = "linkToVirtual"
				lambdaName = "DMH.invokeVirtual"
			Case LF_INVSTATIC
				linkerName = "linkToStatic"
				lambdaName = "DMH.invokeStatic"
			Case LF_INVSTATIC_INIT
				linkerName = "linkToStatic"
				lambdaName = "DMH.invokeStaticInit"
			Case LF_INVSPECIAL
				linkerName = "linkToSpecial"
				lambdaName = "DMH.invokeSpecial"
			Case LF_INVINTERFACE
				linkerName = "linkToInterface"
				lambdaName = "DMH.invokeInterface"
			Case LF_NEWINVSPECIAL
				linkerName = "linkToSpecial"
				lambdaName = "DMH.newInvokeSpecial"
			Case Else
				Throw New InternalError("which=" & which)
			End Select
			Dim mtypeWithArg As MethodType = mtype.appendParameterTypes(GetType(MemberName))
			If doesAlloc Then mtypeWithArg = mtypeWithArg.insertParameterTypes(0, GetType(Object)).changeReturnType(GetType(void)) ' <init> returns  Sub  -  insert newly allocated obj
			Dim linker As New MemberName(GetType(MethodHandle), linkerName, mtypeWithArg, REF_invokeStatic)
			Try
				linker = IMPL_NAMES.resolveOrFail(REF_invokeStatic, linker, Nothing, GetType(NoSuchMethodException))
			Catch ex As ReflectiveOperationException
				Throw newInternalError(ex)
			End Try
			Const DMH_THIS As Integer = 0
			Const ARG_BASE As Integer = 1
			Dim ARG_LIMIT As Integer = ARG_BASE + mtype.parameterCount()
			Dim nameCursor As Integer = ARG_LIMIT
			Dim NEW_OBJ As Integer = (If(doesAlloc, nameCursor, -1))
			nameCursor += 1
			Dim GET_MEMBER As Integer = nameCursor
			nameCursor += 1
			Dim LINKER_CALL As Integer = nameCursor
			nameCursor += 1
			Dim names As Name() = arguments(nameCursor - ARG_LIMIT, mtype.invokerType())
			assert(names.Length = nameCursor)
			If doesAlloc Then
				' names = { argx,y,z,... new C, init method }
				names(NEW_OBJ) = New Name(Lazy.NF_allocateInstance, names(DMH_THIS))
				names(GET_MEMBER) = New Name(Lazy.NF_constructorMethod, names(DMH_THIS))
			ElseIf needsInit Then
				names(GET_MEMBER) = New Name(Lazy.NF_internalMemberNameEnsureInit, names(DMH_THIS))
			Else
				names(GET_MEMBER) = New Name(Lazy.NF_internalMemberName, names(DMH_THIS))
			End If
			assert(findDirectMethodHandle(names(GET_MEMBER)) Is names(DMH_THIS))
			Dim outArgs As Object() = java.util.Arrays.copyOfRange(names, ARG_BASE, GET_MEMBER+1, GetType(Object()))
			assert(outArgs(outArgs.Length-1) Is names(GET_MEMBER)) ' look, shifted args!
			Dim result As Integer = LAST_RESULT
			If doesAlloc Then
				assert(outArgs(outArgs.Length-2) Is names(NEW_OBJ)) ' got to move this one
				Array.Copy(outArgs, 0, outArgs, 1, outArgs.Length-2)
				outArgs(0) = names(NEW_OBJ)
				result = NEW_OBJ
			End If
			names(LINKER_CALL) = New Name(linker, outArgs)
			lambdaName &= "_" & shortenSignature(basicTypeSignature(mtype))
			Dim lform As New LambdaForm(lambdaName, ARG_LIMIT, names, result)
			' This is a tricky bit of code.  Don't send it through the LF interpreter.
			lform.compileToBytecode()
			Return lform
		End Function

		Friend Shared Function findDirectMethodHandle(ByVal name As Name) As Object
			If name.function Is Lazy.NF_internalMemberName OrElse name.function Is Lazy.NF_internalMemberNameEnsureInit OrElse name.function Is Lazy.NF_constructorMethod Then
				assert(name.arguments.length = 1)
				Return name.arguments(0)
			End If
			Return Nothing
		End Function

		Private Shared Sub maybeCompile(ByVal lform As LambdaForm, ByVal m As MemberName)
			If sun.invoke.util.VerifyAccess.isSamePackage(m.declaringClass, GetType(MethodHandle)) Then lform.compileToBytecode()
		End Sub

		''' <summary>
		''' Static wrapper for DirectMethodHandle.internalMemberName. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function internalMemberName(ByVal mh As Object) As Object
		'non-public
			Return CType(mh, DirectMethodHandle).member
		End Function

		''' <summary>
		''' Static wrapper for DirectMethodHandle.internalMemberName.
		''' This one also forces initialization.
		''' </summary>
		'non-public
	 Friend Shared Function internalMemberNameEnsureInit(ByVal mh As Object) As Object
			Dim dmh As DirectMethodHandle = CType(mh, DirectMethodHandle)
			dmh.ensureInitialized()
			Return dmh.member
	 End Function

		'non-public
	 Friend Shared Function shouldBeInitialized(ByVal member As MemberName) As Boolean
			Select Case member.referenceKind
			Case REF_invokeStatic, REF_getStatic, REF_putStatic, REF_newInvokeSpecial
			Case Else
				' No need to initialize the class on this kind of member.
				Return False
			End Select
			Dim cls As  [Class] = member.declaringClass
			If cls Is GetType(sun.invoke.util.ValueConversions) OrElse cls Is GetType(MethodHandleImpl) OrElse cls Is GetType(Invokers) Then Return False
			If sun.invoke.util.VerifyAccess.isSamePackage(GetType(MethodHandle), cls) OrElse sun.invoke.util.VerifyAccess.isSamePackage(GetType(sun.invoke.util.ValueConversions), cls) Then
				' It is a system class.  It is probably in the process of
				' being initialized, but we will help it along just to be safe.
				If UNSAFE.shouldBeInitialized(cls) Then UNSAFE.ensureClassInitialized(cls)
				Return False
			End If
			Return UNSAFE.shouldBeInitialized(cls)
	 End Function

		Private Class EnsureInitialized
			Inherits ClassValue(Of WeakReference(Of Thread))

			Protected Friend Overrides Function computeValue(ByVal type As [Class]) As WeakReference(Of Thread)
				UNSAFE.ensureClassInitialized(type)
				If UNSAFE.shouldBeInitialized(type) Then Return New WeakReference(Of )(Thread.CurrentThread)
				Return Nothing
			End Function
			Friend Shared ReadOnly INSTANCE As New EnsureInitialized
		End Class

		Private Sub ensureInitialized()
			If checkInitialized(member) Then
				' The coast is clear.  Delete the <clinit> barrier.
				If member.field Then
					updateForm(preparedFieldLambdaForm(member))
				Else
					updateForm(preparedLambdaForm(member))
				End If
			End If
		End Sub
		Private Shared Function checkInitialized(ByVal member As MemberName) As Boolean
			Dim defc As  [Class] = member.declaringClass
			Dim ref As WeakReference(Of Thread) = EnsureInitialized.INSTANCE.get(defc)
			If ref Is Nothing Then Return True ' the final state
			Dim clinitThread As Thread = ref.get()
			' Somebody may still be running defc.<clinit>.
			If clinitThread Is Thread.CurrentThread Then
				' If anybody is running defc.<clinit>, it is this thread.
				If UNSAFE.shouldBeInitialized(defc) Then Return False
			Else
				' We are in a random thread.  Block.
				UNSAFE.ensureClassInitialized(defc)
			End If
			assert((Not UNSAFE.shouldBeInitialized(defc)))
			' put it into the final state
			EnsureInitialized.INSTANCE.remove(defc)
			Return True
		End Function

		'non-public
	 Friend Shared Sub ensureInitialized(ByVal mh As Object)
			CType(mh, DirectMethodHandle).ensureInitialized()
	 End Sub

		''' <summary>
		''' This subclass represents invokespecial instructions. </summary>
		Friend Class Special
			Inherits DirectMethodHandle

			Private Sub New(ByVal mtype As MethodType, ByVal form As LambdaForm, ByVal member As MemberName)
				MyBase.New(mtype, form, member)
			End Sub
			Friend Property Overrides invokeSpecial As Boolean
				Get
					Return True
				End Get
			End Property
			Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
				Return New Special(mt, lf, member)
			End Function
		End Class

		''' <summary>
		''' This subclass handles constructor references. </summary>
		Friend Class Constructor
			Inherits DirectMethodHandle

			Friend ReadOnly initMethod As MemberName
			Friend ReadOnly instanceClass As  [Class]

			Private Sub New(ByVal mtype As MethodType, ByVal form As LambdaForm, ByVal constructor As MemberName, ByVal initMethod As MemberName, ByVal instanceClass As [Class])
				MyBase.New(mtype, form, constructor)
				Me.initMethod = initMethod
				Me.instanceClass = instanceClass
				assert(initMethod.resolved)
			End Sub
			Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
				Return New Constructor(mt, lf, member, initMethod, instanceClass)
			End Function
		End Class

		'non-public
	 Friend Shared Function constructorMethod(ByVal mh As Object) As Object
			Dim dmh As Constructor = CType(mh, Constructor)
			Return dmh.initMethod
	 End Function

		'non-public
	 Friend Shared Function allocateInstance(ByVal mh As Object) As Object
			Dim dmh As Constructor = CType(mh, Constructor)
			Return UNSAFE.allocateInstance(dmh.instanceClass)
	 End Function

		''' <summary>
		''' This subclass handles non-static field references. </summary>
		Friend Class Accessor
			Inherits DirectMethodHandle

			Friend ReadOnly fieldType As  [Class]
			Friend ReadOnly fieldOffset As Integer
			Private Sub New(ByVal mtype As MethodType, ByVal form As LambdaForm, ByVal member As MemberName, ByVal fieldOffset As Integer)
				MyBase.New(mtype, form, member)
				Me.fieldType = member.fieldType
				Me.fieldOffset = fieldOffset
			End Sub

			Friend Overrides Function checkCast(ByVal obj As Object) As Object
				Return fieldType.cast(obj)
			End Function
			Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
				Return New Accessor(mt, lf, member, fieldOffset)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function fieldOffset(ByVal accessorObj As Object) As Long
		'non-public
			' Note: We return a long because that is what Unsafe.getObject likes.
			' We store a plain int because it is more compact.
			Return CType(accessorObj, Accessor).fieldOffset
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function checkBase(ByVal obj As Object) As Object
		'non-public
			' Note that the object's class has already been verified,
			' since the parameter type of the Accessor method handle
			' is either member.getDeclaringClass or a subclass.
			' This was verified in DirectMethodHandle.make.
			' Therefore, the only remaining check is for null.
			' Since this check is *not* guaranteed by Unsafe.getInt
			' and its siblings, we need to make an explicit one here.
			obj.GetType() ' maybe throw NPE
			Return obj
		End Function

		''' <summary>
		''' This subclass handles static field references. </summary>
		Friend Class StaticAccessor
			Inherits DirectMethodHandle

			Private ReadOnly fieldType As  [Class]
			Private ReadOnly staticBase As Object
			Private ReadOnly staticOffset As Long

			Private Sub New(ByVal mtype As MethodType, ByVal form As LambdaForm, ByVal member As MemberName, ByVal staticBase As Object, ByVal staticOffset As Long)
				MyBase.New(mtype, form, member)
				Me.fieldType = member.fieldType
				Me.staticBase = staticBase
				Me.staticOffset = staticOffset
			End Sub

			Friend Overrides Function checkCast(ByVal obj As Object) As Object
				Return fieldType.cast(obj)
			End Function
			Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
				Return New StaticAccessor(mt, lf, member, staticBase, staticOffset)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function nullCheck(ByVal obj As Object) As Object
		'non-public
			obj.GetType()
			Return obj
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function staticBase(ByVal accessorObj As Object) As Object
		'non-public
			Return CType(accessorObj, StaticAccessor).staticBase
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function staticOffset(ByVal accessorObj As Object) As Long
		'non-public
			Return CType(accessorObj, StaticAccessor).staticOffset
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function checkCast(ByVal mh As Object, ByVal obj As Object) As Object
		'non-public
			Return CType(mh, DirectMethodHandle).checkCast(obj)
		End Function

		Friend Overridable Function checkCast(ByVal obj As Object) As Object
			Return member.returnType.cast(obj)
		End Function

		' Caching machinery for field accessors:
		Private Shared AF_GETFIELD As SByte = 0, AF_PUTFIELD As SByte = 1, AF_GETSTATIC As SByte = 2, AF_PUTSTATIC As SByte = 3, AF_GETSTATIC_INIT As SByte = 4, AF_PUTSTATIC_INIT As SByte = 5, AF_LIMIT As SByte = 6
		' Enumerate the different field kinds using Wrapper,
		' with an extra case added for checked references.
		Private Shared FT_LAST_WRAPPER As Integer = sun.invoke.util.Wrapper.values().length-1, FT_UNCHECKED_REF As Integer = sun.invoke.util.Wrapper.OBJECT.ordinal(), FT_CHECKED_REF As Integer = FT_LAST_WRAPPER+1, FT_LIMIT As Integer = FT_LAST_WRAPPER+2
		Private Shared Function afIndex(ByVal formOp As SByte, ByVal isVolatile As Boolean, ByVal ftypeKind As Integer) As Integer
			Return ((formOp * FT_LIMIT * 2) + (If(isVolatile, FT_LIMIT, 0)) + ftypeKind)
		End Function
		Private Shared ReadOnly ACCESSOR_FORMS As LambdaForm() = New LambdaForm(afIndex(AF_LIMIT, False, 0) - 1){}
		Private Shared Function ftypeKind(ByVal ftype As [Class]) As Integer
			If ftype.primitive Then
				Return sun.invoke.util.Wrapper.forPrimitiveType(ftype).ordinal()
			ElseIf sun.invoke.util.VerifyType.isNullReferenceConversion(GetType(Object), ftype) Then
				Return FT_UNCHECKED_REF
			Else
				Return FT_CHECKED_REF
			End If
		End Function

		''' <summary>
		''' Create a LF which can access the given field.
		''' Cache and share this structure among all fields with
		''' the same basicType and refKind.
		''' </summary>
		Private Shared Function preparedFieldLambdaForm(ByVal m As MemberName) As LambdaForm
			Dim ftype As  [Class] = m.fieldType
			Dim isVolatile As Boolean = m.volatile
			Dim formOp As SByte
			Select Case m.referenceKind
			Case REF_getField
				formOp = AF_GETFIELD
			Case REF_putField
				formOp = AF_PUTFIELD
			Case REF_getStatic
				formOp = AF_GETSTATIC
			Case REF_putStatic
				formOp = AF_PUTSTATIC
			Case Else
				Throw New InternalError(m.ToString())
			End Select
			If shouldBeInitialized(m) Then
				' precompute the barrier-free version:
				preparedFieldLambdaForm(formOp, isVolatile, ftype)
				assert((AF_GETSTATIC_INIT - AF_GETSTATIC) = (AF_PUTSTATIC_INIT - AF_PUTSTATIC))
				formOp += (AF_GETSTATIC_INIT - AF_GETSTATIC)
			End If
			Dim lform As LambdaForm = preparedFieldLambdaForm(formOp, isVolatile, ftype)
			maybeCompile(lform, m)
			assert(lform.methodType().dropParameterTypes(0, 1).Equals(m.invocationType.basicType())) : java.util.Arrays.asList(m, m.invocationType.basicType(), lform, lform.methodType())
			Return lform
		End Function
		Private Shared Function preparedFieldLambdaForm(ByVal formOp As SByte, ByVal isVolatile As Boolean, ByVal ftype As [Class]) As LambdaForm
			Dim afIndex As Integer = afIndex(formOp, isVolatile, ftypeKind(ftype))
			Dim lform As LambdaForm = ACCESSOR_FORMS(afIndex)
			If lform IsNot Nothing Then Return lform
			lform = makePreparedFieldLambdaForm(formOp, isVolatile, ftypeKind(ftype))
			ACCESSOR_FORMS(afIndex) = lform ' don't bother with a CAS
			Return lform
		End Function

		Private Shared Function makePreparedFieldLambdaForm(ByVal formOp As SByte, ByVal isVolatile As Boolean, ByVal ftypeKind As Integer) As LambdaForm
			Dim isGetter As Boolean = (formOp And 1) = (AF_GETFIELD And 1)
			Dim isStatic As Boolean = (formOp >= AF_GETSTATIC)
			Dim needsInit As Boolean = (formOp >= AF_GETSTATIC_INIT)
			Dim needsCast As Boolean = (ftypeKind = FT_CHECKED_REF)
			Dim fw As sun.invoke.util.Wrapper = (If(needsCast, sun.invoke.util.Wrapper.OBJECT, sun.invoke.util.Wrapper.values()(ftypeKind)))
			Dim ft As  [Class] = fw.primitiveType()
			assert(ftypeKind(If(needsCast, GetType(String), ft)) = ftypeKind)
			Dim tname As String = fw.primitiveSimpleName()
			Dim ctname As String = Char.ToUpper(tname.Chars(0)) + tname.Substring(1)
			If isVolatile Then ctname &= "Volatile"
			Dim getOrPut As String = (If(isGetter, "get", "put"))
			Dim linkerName As String = (getOrPut + ctname) ' getObject, putIntVolatile, etc.
			Dim linkerType As MethodType
			If isGetter Then
				linkerType = MethodType.methodType(ft, GetType(Object), GetType(Long))
			Else
				linkerType = MethodType.methodType(GetType(void), GetType(Object), GetType(Long), ft)
			End If
			Dim linker As New MemberName(GetType(sun.misc.Unsafe), linkerName, linkerType, REF_invokeVirtual)
			Try
				linker = IMPL_NAMES.resolveOrFail(REF_invokeVirtual, linker, Nothing, GetType(NoSuchMethodException))
			Catch ex As ReflectiveOperationException
				Throw newInternalError(ex)
			End Try

			' What is the external type of the lambda form?
			Dim mtype As MethodType
			If isGetter Then
				mtype = MethodType.methodType(ft)
			Else
				mtype = MethodType.methodType(GetType(void), ft)
			End If
			mtype = mtype.basicType() ' erase short to int, etc.
			If Not isStatic Then mtype = mtype.insertParameterTypes(0, GetType(Object))
			Const DMH_THIS As Integer = 0
			Const ARG_BASE As Integer = 1
			Dim ARG_LIMIT As Integer = ARG_BASE + mtype.parameterCount()
			' if this is for non-static access, the base pointer is stored at this index:
			Dim OBJ_BASE As Integer = If(isStatic, -1, ARG_BASE)
			' if this is for write access, the value to be written is stored at this index:
			Dim SET_VALUE As Integer = If(isGetter, -1, ARG_LIMIT - 1)
			Dim nameCursor As Integer = ARG_LIMIT
			Dim F_HOLDER As Integer = (If(isStatic, nameCursor, -1))
			nameCursor += 1
			Dim F_OFFSET As Integer = nameCursor
			nameCursor += 1
			Dim OBJ_CHECK As Integer = (If(OBJ_BASE >= 0, nameCursor, -1))
			nameCursor += 1
			Dim INIT_BAR As Integer = (If(needsInit, nameCursor, -1))
			nameCursor += 1
			Dim PRE_CAST As Integer = (If(needsCast AndAlso (Not isGetter), nameCursor, -1))
			nameCursor += 1
			Dim LINKER_CALL As Integer = nameCursor
			nameCursor += 1
			Dim POST_CAST As Integer = (If(needsCast AndAlso isGetter, nameCursor, -1))
			nameCursor += 1
			Dim RESULT As Integer = nameCursor-1 ' either the call or the cast
			Dim names As Name() = arguments(nameCursor - ARG_LIMIT, mtype.invokerType())
			If needsInit Then names(INIT_BAR) = New Name(Lazy.NF_ensureInitialized, names(DMH_THIS))
			If needsCast AndAlso (Not isGetter) Then names(PRE_CAST) = New Name(Lazy.NF_checkCast, names(DMH_THIS), names(SET_VALUE))
			Dim outArgs As Object() = New Object(1 + linkerType.parameterCount() - 1){}
			assert(outArgs.Length = (If(isGetter, 3, 4)))
			outArgs(0) = UNSAFE
			If isStatic Then
					names(F_HOLDER) = New Name(Lazy.NF_staticBase, names(DMH_THIS))
					outArgs(1) = names(F_HOLDER)
					names(F_OFFSET) = New Name(Lazy.NF_staticOffset, names(DMH_THIS))
					outArgs(2) = names(F_OFFSET)
			Else
					names(OBJ_CHECK) = New Name(Lazy.NF_checkBase, names(OBJ_BASE))
					outArgs(1) = names(OBJ_CHECK)
					names(F_OFFSET) = New Name(Lazy.NF_fieldOffset, names(DMH_THIS))
					outArgs(2) = names(F_OFFSET)
			End If
			If Not isGetter Then outArgs(3) = (If(needsCast, names(PRE_CAST), names(SET_VALUE)))
			For Each a As Object In outArgs
				assert(a IsNot Nothing)
			Next a
			names(LINKER_CALL) = New Name(linker, outArgs)
			If needsCast AndAlso isGetter Then names(POST_CAST) = New Name(Lazy.NF_checkCast, names(DMH_THIS), names(LINKER_CALL))
			For Each n As Name In names
				assert(n IsNot Nothing)
			Next n
			Dim fieldOrStatic As String = (If(isStatic, "Static", "Field"))
			Dim lambdaName As String = (linkerName + fieldOrStatic) ' significant only for debugging
			If needsCast Then lambdaName &= "Cast"
			If needsInit Then lambdaName &= "Init"
			Return New LambdaForm(lambdaName, ARG_LIMIT, names, RESULT)
		End Function

		''' <summary>
		''' Pre-initialized NamedFunctions for bootstrapping purposes.
		''' Factored in an inner class to delay initialization until first usage.
		''' </summary>
		Private Class Lazy
			Friend Shared ReadOnly NF_internalMemberName, NF_internalMemberNameEnsureInit, NF_ensureInitialized, NF_fieldOffset, NF_checkBase, NF_staticBase, NF_staticOffset, NF_checkCast, NF_allocateInstance, NF_constructorMethod As NamedFunction
			Shared Sub New()
				Try
						Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("allocateInstance", GetType(Object))), NF_constructorMethod = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("constructorMethod", GetType(Object))) }
							Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("checkCast", GetType(Object), GetType(Object))), NF_allocateInstance = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("allocateInstance", GetType(Object))), NF_constructorMethod
								Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("staticOffset", GetType(Object))), NF_checkCast = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("checkCast", GetType(Object), GetType(Object))), NF_allocateInstance
									Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("staticBase", GetType(Object))), NF_staticOffset = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("staticOffset", GetType(Object))), NF_checkCast
										Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("checkBase", GetType(Object))), NF_staticBase = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("staticBase", GetType(Object))), NF_staticOffset
											Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("fieldOffset", GetType(Object))), NF_checkBase = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("checkBase", GetType(Object))), NF_staticBase
												Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("ensureInitialized", GetType(Object))), NF_fieldOffset = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("fieldOffset", GetType(Object))), NF_checkBase
													Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("internalMemberNameEnsureInit", GetType(Object))), NF_ensureInitialized = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("ensureInitialized", GetType(Object))), NF_fieldOffset
														Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("internalMemberName", GetType(Object))), NF_internalMemberNameEnsureInit = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("internalMemberNameEnsureInit", GetType(Object))), NF_ensureInitialized
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
														Dim nfs As NamedFunction() = { NF_internalMemberName = New NamedFunction(GetType(DirectMethodHandle).getDeclaredMethod("internalMemberName", GetType(Object))), NF_internalMemberNameEnsureInit
					For Each nf As NamedFunction In nfs
						' Each nf must be statically invocable or we get tied up in our bootstraps.
						assert(InvokerBytecodeGenerator.isStaticallyInvocable(nf.member)) : nf
						nf.resolve()
					Next nf
				Catch ex As ReflectiveOperationException
					Throw newInternalError(ex)
				End Try
			End Sub
		End Class
	End Class

End Namespace