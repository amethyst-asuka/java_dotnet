Imports System
Imports System.Runtime.CompilerServices

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
	''' Construction and caching of often-used invokers.
	''' @author jrose
	''' </summary>
	Friend Class Invokers
		' exact type (sans leading taget MH) for the outgoing call
		Private ReadOnly targetType As MethodType

		' Cached adapter information:
		Private ReadOnly MethodHandle As Stable()
		' Indexes into invokers:
		Friend Const INV_EXACT As Integer = 0, INV_GENERIC As Integer = 1, INV_BASIC As Integer = 2, INV_LIMIT As Integer = 3 ' MethodHandles.basicInvoker -  MethodHandles.invoker (generic invocation) -  MethodHandles.exactInvoker

		''' <summary>
		''' Compute and cache information common to all collecting adapters
		'''  that implement members of the erasure-family of the given erased type.
		''' </summary>
		'non-public
	 Friend Sub New(  targetType As MethodType)
			Me.targetType = targetType
	 End Sub

		'non-public
	 Friend Overridable Function exactInvoker() As MethodHandle
			Dim invoker As MethodHandle = cachedInvoker(INV_EXACT)
			If invoker IsNot Nothing Then Return invoker
			invoker = makeExactOrGeneralInvoker(True)
			Return cachedInvokerker(INV_EXACT, invoker)
	 End Function

		'non-public
	 Friend Overridable Function genericInvoker() As MethodHandle
			Dim invoker As MethodHandle = cachedInvoker(INV_GENERIC)
			If invoker IsNot Nothing Then Return invoker
			invoker = makeExactOrGeneralInvoker(False)
			Return cachedInvokerker(INV_GENERIC, invoker)
	 End Function

		'non-public
	 Friend Overridable Function basicInvoker() As MethodHandle
			Dim invoker As MethodHandle = cachedInvoker(INV_BASIC)
			If invoker IsNot Nothing Then Return invoker
			Dim basicType As MethodType = targetType.basicType()
			If basicType IsNot targetType Then Return cachedInvokerker(INV_BASIC, basicType.invokers().basicInvoker())
			invoker = basicType.form().cachedMethodHandle(MethodTypeForm.MH_BASIC_INV)
			If invoker Is Nothing Then
				Dim method As MemberName = invokeBasicMethod(basicType)
				invoker = DirectMethodHandle.make(method)
				assert(checkInvoker(invoker))
				invoker = basicType.form().cachedMethodHandledle(MethodTypeForm.MH_BASIC_INV, invoker)
			End If
			Return cachedInvokerker(INV_BASIC, invoker)
	 End Function

		Private Function cachedInvoker(  idx As Integer) As MethodHandle
			Return invokers(idx)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function setCachedInvoker(  idx As Integer,   invoker As MethodHandle) As MethodHandle
			' Simulate a CAS, to avoid racy duplication of results.
			Dim prev As MethodHandle = invokers(idx)
			If prev IsNot Nothing Then Return prev
				invokers(idx) = invoker
				Return invokers(idx)
		End Function

		Private Function makeExactOrGeneralInvoker(  isExact As Boolean) As MethodHandle
			Dim mtype As MethodType = targetType
			Dim invokerType As MethodType = mtype.invokerType()
			Dim which As Integer = (If(isExact, MethodTypeForm.LF_EX_INVOKER, MethodTypeForm.LF_GEN_INVOKER))
			Dim lform As LambdaForm = invokeHandleForm(mtype, False, which)
			Dim invoker As MethodHandle = BoundMethodHandle.bindSingle(invokerType, lform, mtype)
			Dim whichName As String = (If(isExact, "invokeExact", "invoke"))
			invoker = invoker.withInternalMemberName(MemberName.makeMethodHandleInvoke(whichName, mtype), False)
			assert(checkInvoker(invoker))
			maybeCompileToBytecode(invoker)
			Return invoker
		End Function

		''' <summary>
		''' If the target type seems to be common enough, eagerly compile the invoker to bytecodes. </summary>
		Private Sub maybeCompileToBytecode(  invoker As MethodHandle)
			Const EAGER_COMPILE_ARITY_LIMIT As Integer = 10
			If targetType Is targetType.erase() AndAlso targetType.parameterCount() < EAGER_COMPILE_ARITY_LIMIT Then invoker.form.compileToBytecode()
		End Sub

		' This next one is called from LambdaForm.NamedFunction.<init>.
		'non-public
	 Friend Shared Function invokeBasicMethod(  basicType As MethodType) As MemberName
			assert(basicType Is basicType.basicType())
			Try
				'Lookup.findVirtual(MethodHandle.class, name, type);
				Return IMPL_LOOKUP.resolveOrFail(REF_invokeVirtual, GetType(MethodHandle), "invokeBasic", basicType)
			Catch ex As ReflectiveOperationException
				Throw newInternalError("JVM cannot find invoker for " & basicType, ex)
			End Try
	 End Function

		Private Function checkInvoker(  invoker As MethodHandle) As Boolean
			assert(targetType.invokerType().Equals(invoker.type())) : java.util.Arrays.asList(targetType, targetType.invokerType(), invoker)
			assert(invoker.internalMemberName() Is Nothing OrElse invoker.internalMemberName().methodType.Equals(targetType))
			assert((Not invoker.varargsCollector))
			Return True
		End Function

		''' <summary>
		''' Find or create an invoker which passes unchanged a given number of arguments
		''' and spreads the rest from a trailing array argument.
		''' The invoker target type is the post-spread type {@code (TYPEOF(uarg*), TYPEOF(sarg*))=>RT}.
		''' All the {@code sarg}s must have a common type {@code C}.  (If there are none, {@code Object} is assumed.} </summary>
		''' <param name="leadingArgCount"> the number of unchanged (non-spread) arguments </param>
		''' <returns> {@code invoker.invokeExact(mh, uarg*, C[]{sarg*}) := (RT)mh.invoke(uarg*, sarg*)} </returns>
		'non-public
	 Friend Overridable Function spreadInvoker(  leadingArgCount As Integer) As MethodHandle
			Dim spreadArgCount As Integer = targetType.parameterCount() - leadingArgCount
			Dim postSpreadType As MethodType = targetType
			Dim argArrayType As  [Class] = impliedRestargType(postSpreadType, leadingArgCount)
			If postSpreadType.parameterSlotCount() <= MethodType.MAX_MH_INVOKER_ARITY Then Return genericInvoker().asSpreader(argArrayType, spreadArgCount)
			' Cannot build a generic invoker here of type ginvoker.invoke(mh, a*[254]).
			' Instead, factor sinvoker.invoke(mh, a) into ainvoker.invoke(filter(mh), a)
			' where filter(mh) == mh.asSpreader(Object[], spreadArgCount)
			Dim preSpreadType As MethodType = postSpreadType.replaceParameterTypes(leadingArgCount, postSpreadType.parameterCount(), argArrayType)
			Dim arrayInvoker As MethodHandle = MethodHandles.invoker(preSpreadType)
			Dim makeSpreader As MethodHandle = MethodHandles.insertArguments(Lazy.MH_asSpreader, 1, argArrayType, spreadArgCount)
			Return MethodHandles.filterArgument(arrayInvoker, 0, makeSpreader)
	 End Function

		Private Shared Function impliedRestargType(  restargType As MethodType,   fromPos As Integer) As  [Class]
			If restargType.generic Then ' can be nothing else Return GetType(Object())
			Dim maxPos As Integer = restargType.parameterCount()
			If fromPos >= maxPos Then ' reasonable default Return GetType(Object())
			Dim argType As  [Class] = restargType.parameterType(fromPos)
			For i As Integer = fromPos+1 To maxPos - 1
				If argType IsNot restargType.parameterType(i) Then Throw newIllegalArgumentException("need homogeneous rest arguments", restargType)
			Next i
			If argType Is GetType(Object) Then Return GetType(Object())
			Return Array.newInstance(argType, 0).GetType()
		End Function

		Public Overrides Function ToString() As String
			Return "Invokers" & targetType
		End Function

		Friend Shared Function methodHandleInvokeLinkerMethod(  name As String,   mtype As MethodType,   appendixResult As Object()) As MemberName
			Dim which As Integer
			Select Case name
			Case "invokeExact"
				which = MethodTypeForm.LF_EX_LINKER
			Case "invoke"
				which = MethodTypeForm.LF_GEN_LINKER
			Case Else
				Throw New InternalError("not invoker: " & name)
			End Select
			Dim lform As LambdaForm
			If mtype.parameterSlotCount() <= MethodType.MAX_MH_ARITY - MH_LINKER_ARG_APPENDED Then
				lform = invokeHandleForm(mtype, False, which)
				appendixResult(0) = mtype
			Else
				lform = invokeHandleForm(mtype, True, which)
			End If
			Return lform.vmentry
		End Function

		' argument count to account for trailing "appendix value" (typically the mtype)
		Private Const MH_LINKER_ARG_APPENDED As Integer = 1

		''' <summary>
		''' Returns an adapter for invokeExact or generic invoke, as a MH or constant pool linker.
		''' If !customized, caller is responsible for supplying, during adapter execution,
		''' a copy of the exact mtype.  This is because the adapter might be generalized to
		''' a basic type. </summary>
		''' <param name="mtype"> the caller's method type (either basic or full-custom) </param>
		''' <param name="customized"> whether to use a trailing appendix argument (to carry the mtype) </param>
		''' <param name="which"> bit-encoded 0x01 whether it is a CP adapter ("linker") or MHs.invoker value ("invoker");
		'''                          0x02 whether it is for invokeExact or generic invoke </param>
		Private Shared Function invokeHandleForm(  mtype As MethodType,   customized As Boolean,   which As Integer) As LambdaForm
			Dim isCached As Boolean
			If Not customized Then
				mtype = mtype.basicType() ' normalize Z to I, String to Object, etc.
				isCached = True
			Else
				isCached = False ' maybe cache if mtype == mtype.basicType()
			End If
			Dim isLinker, isGeneric As Boolean
			Dim debugName As String
			Select Case which
			Case MethodTypeForm.LF_EX_LINKER
				isLinker = True
				isGeneric = False
				debugName = "invokeExact_MT"
			Case MethodTypeForm.LF_EX_INVOKER
				isLinker = False
				isGeneric = False
				debugName = "exactInvoker"
			Case MethodTypeForm.LF_GEN_LINKER
				isLinker = True
				isGeneric = True
				debugName = "invoke_MT"
			Case MethodTypeForm.LF_GEN_INVOKER
				isLinker = False
				isGeneric = True
				debugName = "invoker"
			Case Else
				Throw New InternalError
			End Select
			Dim lform As LambdaForm
			If isCached Then
				lform = mtype.form().cachedLambdaForm(which)
				If lform IsNot Nothing Then Return lform
			End If
			' exactInvokerForm (Object,Object)Object
			'   link with java.lang.invoke.MethodHandle.invokeBasic(MethodHandle,Object,Object)Object/invokeSpecial
			Const THIS_MH As Integer = 0
			Dim CALL_MH As Integer = THIS_MH + (If(isLinker, 0, 1))
			Dim ARG_BASE As Integer = CALL_MH + 1
			Dim OUTARG_LIMIT As Integer = ARG_BASE + mtype.parameterCount()
			Dim INARG_LIMIT As Integer = OUTARG_LIMIT + (If(isLinker AndAlso (Not customized), 1, 0))
			Dim nameCursor As Integer = OUTARG_LIMIT
				Dim MTYPE_ARG As Integer = If(customized, -1, nameCursor)
				nameCursor += 1
			Dim CHECK_TYPE As Integer = nameCursor
			nameCursor += 1
				Dim CHECK_CUSTOM As Integer = If(CUSTOMIZE_THRESHOLD >= 0, nameCursor, -1)
				nameCursor += 1
			Dim LINKER_CALL As Integer = nameCursor
			nameCursor += 1
			Dim invokerFormType As MethodType = mtype.invokerType()
			If isLinker Then
				If Not customized Then invokerFormType = invokerFormType.appendParameterTypes(GetType(MemberName))
			Else
				invokerFormType = invokerFormType.invokerType()
			End If
			Dim names As Name() = arguments(nameCursor - INARG_LIMIT, invokerFormType)
			assert(names.Length = nameCursor) : java.util.Arrays.asList(mtype, customized, which, nameCursor, names.Length)
			If MTYPE_ARG >= INARG_LIMIT Then
				assert(names(MTYPE_ARG) Is Nothing)
				Dim speciesData As BoundMethodHandle.SpeciesData = BoundMethodHandle.speciesData_L()
				names(THIS_MH) = names(THIS_MH).withConstraint(speciesData)
				Dim getter As NamedFunction = speciesData.getterFunction(0)
				names(MTYPE_ARG) = New Name(getter, names(THIS_MH))
				' else if isLinker, then MTYPE is passed in from the caller (e.g., the JVM)
			End If

			' Make the final call.  If isGeneric, then prepend the result of type checking.
			Dim outCallType As MethodType = mtype.basicType()
			Dim outArgs As Object() = java.util.Arrays.copyOfRange(names, CALL_MH, OUTARG_LIMIT, GetType(Object()))
			Dim mtypeArg As Object = (If(customized, mtype, names(MTYPE_ARG)))
			If Not isGeneric Then
				names(CHECK_TYPE) = New Name(NF_checkExactType, names(CALL_MH), mtypeArg)
				' mh.invokeExact(a*):R => checkExactType(mh, TYPEOF(a*:R)); mh.invokeBasic(a*)
			Else
				names(CHECK_TYPE) = New Name(NF_checkGenericType, names(CALL_MH), mtypeArg)
				' mh.invokeGeneric(a*):R => checkGenericType(mh, TYPEOF(a*:R)).invokeBasic(a*)
				outArgs(0) = names(CHECK_TYPE)
			End If
			If CHECK_CUSTOM <> -1 Then names(CHECK_CUSTOM) = New Name(NF_checkCustomized, outArgs(0))
			names(LINKER_CALL) = New Name(outCallType, outArgs)
			lform = New LambdaForm(debugName, INARG_LIMIT, names)
			If isLinker Then lform.compileToBytecode() ' JVM needs a real methodOop
			If isCached Then lform = mtype.form().cachedLambdaFormorm(which, lform)
			Return lform
		End Function

		'non-public
	 Friend Shared Function newWrongMethodTypeException(  actual As MethodType,   expected As MethodType) As WrongMethodTypeException
			' FIXME: merge with JVM logic for throwing WMTE
			Return New WrongMethodTypeException("expected " & expected & " but found " & actual)
	 End Function

		''' <summary>
		''' Static definition of MethodHandle.invokeExact checking code. </summary>
		'non-public
	 Friend Shared ForceInline Sub checkExactType(  mhObj As Object,   expectedObj As Object)
			Dim mh As MethodHandle = CType(mhObj, MethodHandle)
			Dim expected As MethodType = CType(expectedObj, MethodType)
			Dim actual As MethodType = mh.type()
			If actual IsNot expected Then Throw newWrongMethodTypeException(expected, actual)
	 End Sub

		''' <summary>
		''' Static definition of MethodHandle.invokeGeneric checking code.
		''' Directly returns the type-adjusted MH to invoke, as follows:
		''' {@code (R)MH.invoke(a*) => MH.asType(TYPEOF(a*:R)).invokeBasic(a*)}
		''' </summary>
		'non-public
	 Friend Shared ForceInline Function checkGenericType(  mhObj As Object,   expectedObj As Object) As Object
			Dim mh As MethodHandle = CType(mhObj, MethodHandle)
			Dim expected As MethodType = CType(expectedObj, MethodType)
			Return mh.asType(expected)
	'         Maybe add more paths here.  Possible optimizations:
	'         * for (R)MH.invoke(a*),
	'         * let MT0 = TYPEOF(a*:R), MT1 = MH.type
	'         *
	'         * if MT0==MT1 or MT1 can be safely called by MT0
	'         *  => MH.invokeBasic(a*)
	'         * if MT1 can be safely called by MT0[R := Object]
	'         *  => MH.invokeBasic(a*) & checkcast(R)
	'         * if MT1 can be safely called by MT0[* := Object]
	'         *  => checkcast(A)* & MH.invokeBasic(a*) & checkcast(R)
	'         * if a big adapter BA can be pulled out of (MT0,MT1)
	'         *  => BA.invokeBasic(MT0,MH,a*)
	'         * if a local adapter LA can cached on static CS0 = new GICS(MT0)
	'         *  => CS0.LA.invokeBasic(MH,a*)
	'         * else
	'         *  => MH.asType(MT0).invokeBasic(A*)
	'         
	 End Function

		Friend Shared Function linkToCallSiteMethod(  mtype As MethodType) As MemberName
			Dim lform As LambdaForm = callSiteForm(mtype, False)
			Return lform.vmentry
		End Function

		Friend Shared Function linkToTargetMethod(  mtype As MethodType) As MemberName
			Dim lform As LambdaForm = callSiteForm(mtype, True)
			Return lform.vmentry
		End Function

		' skipCallSite is true if we are optimizing a ConstantCallSite
		Private Shared Function callSiteForm(  mtype As MethodType,   skipCallSite As Boolean) As LambdaForm
			mtype = mtype.basicType() ' normalize Z to I, String to Object, etc.
			Dim which As Integer = (If(skipCallSite, MethodTypeForm.LF_MH_LINKER, MethodTypeForm.LF_CS_LINKER))
			Dim lform As LambdaForm = mtype.form().cachedLambdaForm(which)
			If lform IsNot Nothing Then Return lform
			' exactInvokerForm (Object,Object)Object
			'   link with java.lang.invoke.MethodHandle.invokeBasic(MethodHandle,Object,Object)Object/invokeSpecial
			Const ARG_BASE As Integer = 0
			Dim OUTARG_LIMIT As Integer = ARG_BASE + mtype.parameterCount()
			Dim INARG_LIMIT As Integer = OUTARG_LIMIT + 1
			Dim nameCursor As Integer = OUTARG_LIMIT
			Dim APPENDIX_ARG As Integer = nameCursor
			nameCursor += 1
			Dim CSITE_ARG As Integer = If(skipCallSite, -1, APPENDIX_ARG)
				Dim CALL_MH As Integer = If(skipCallSite, APPENDIX_ARG, nameCursor)
				nameCursor += 1
			Dim LINKER_CALL As Integer = nameCursor
			nameCursor += 1
			Dim invokerFormType As MethodType = mtype.appendParameterTypes(If(skipCallSite, GetType(MethodHandle), GetType(CallSite)))
			Dim names As Name() = arguments(nameCursor - INARG_LIMIT, invokerFormType)
			assert(names.Length = nameCursor)
			assert(names(APPENDIX_ARG) IsNot Nothing)
			If Not skipCallSite Then names(CALL_MH) = New Name(NF_getCallSiteTarget, names(CSITE_ARG))
			' (site.)invokedynamic(a*):R => mh = site.getTarget(); mh.invokeBasic(a*)
			Const PREPEND_MH As Integer = 0, PREPEND_COUNT As Integer = 1
			Dim outArgs As Object() = java.util.Arrays.copyOfRange(names, ARG_BASE, OUTARG_LIMIT + PREPEND_COUNT, GetType(Object()))
			' prepend MH argument:
			Array.Copy(outArgs, 0, outArgs, PREPEND_COUNT, outArgs.Length - PREPEND_COUNT)
			outArgs(PREPEND_MH) = names(CALL_MH)
			names(LINKER_CALL) = New Name(mtype, outArgs)
			lform = New LambdaForm((If(skipCallSite, "linkToTargetMethod", "linkToCallSite")), INARG_LIMIT, names)
			lform.compileToBytecode() ' JVM needs a real methodOop
			lform = mtype.form().cachedLambdaFormorm(which, lform)
			Return lform
		End Function

		''' <summary>
		''' Static definition of MethodHandle.invokeGeneric checking code. </summary>
		'non-public
	 Friend Shared ForceInline Function getCallSiteTarget(  site As Object) As Object
			Return CType(site, CallSite).target
	 End Function

		'non-public
	 Friend Shared ForceInline Sub checkCustomized(  o As Object)
			Dim mh As MethodHandle = CType(o, MethodHandle)
			If mh.form.customized Is Nothing Then maybeCustomize(mh)
	 End Sub

		'non-public
	 Friend Shared DontInline Sub maybeCustomize(  mh As MethodHandle)
			Dim count As SByte = mh.customizationCount
			If count >= CUSTOMIZE_THRESHOLD Then
				mh.customize()
			Else
				mh.customizationCount = CByte(count+1)
			End If
	 End Sub

		' Local constant functions:
		Private Shared ReadOnly NF_checkExactType, NF_checkGenericType, NF_getCallSiteTarget, NF_checkCustomized As NamedFunction
		Shared Sub New()
			Try
					Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(Invokers).getDeclaredMethod("getCallSiteTarget", GetType(Object))), NF_checkCustomized = New NamedFunction(GetType(Invokers).getDeclaredMethod("checkCustomized", GetType(Object))) }
						Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(Invokers).getDeclaredMethod("checkGenericType", GetType(Object), GetType(Object))), NF_getCallSiteTarget = New NamedFunction(GetType(Invokers).getDeclaredMethod("getCallSiteTarget", GetType(Object))), NF_checkCustomized
							Dim TempNamedFunction As NamedFunction = New NamedFunction(GetType(Invokers).getDeclaredMethod("checkExactType", GetType(Object), GetType(Object))), NF_checkGenericType = New NamedFunction(GetType(Invokers).getDeclaredMethod("checkGenericType", GetType(Object), GetType(Object))), NF_getCallSiteTarget
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim nfs As NamedFunction() = { NF_checkExactType = New NamedFunction(GetType(Invokers).getDeclaredMethod("checkExactType", GetType(Object), GetType(Object))), NF_checkGenericType
				For Each nf As NamedFunction In nfs
					' Each nf must be statically invocable or we get tied up in our bootstraps.
					assert(InvokerBytecodeGenerator.isStaticallyInvocable(nf.member)) : nf
					nf.resolve()
				Next nf
			Catch ex As ReflectiveOperationException
				Throw newInternalError(ex)
			End Try
				Try
					MH_asSpreader = IMPL_LOOKUP.findVirtual(GetType(MethodHandle), "asSpreader", MethodType.methodType(GetType(MethodHandle), GetType(Class), GetType(Integer)))
				Catch ex As ReflectiveOperationException
					Throw newInternalError(ex)
				End Try
		End Sub

		Private Class Lazy
			Private Shared ReadOnly MH_asSpreader As MethodHandle

		End Class
	End Class

End Namespace