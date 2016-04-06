Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

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
	''' Trusted implementation code for MethodHandle.
	''' @author jrose
	''' </summary>
	'non-public
	 Friend MustInherit Class MethodHandleImpl
		' Do not adjust this except for special platforms:
		Private Shared ReadOnly MAX_ARITY As Integer
		Shared Sub New()
			Dim values As Object() = { 255 }
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			MAX_ARITY = CInt(Fix(values(0)))
				Dim cache As MethodHandle() = TYPED_ACCESSORS.get(GetType(Object()))
					OBJECT_ARRAY_GETTER = makeIntrinsic(getAccessor(GetType(Object()), False), Intrinsic.ARRAY_LOAD)
					cache(GETTER_INDEX) = OBJECT_ARRAY_GETTER
					OBJECT_ARRAY_SETTER = makeIntrinsic(getAccessor(GetType(Object()), True), Intrinsic.ARRAY_STORE)
					cache(SETTER_INDEX) = OBJECT_ARRAY_SETTER

				assert(InvokerBytecodeGenerator.isStaticallyInvocable(ArrayAccessor.OBJECT_ARRAY_GETTER.internalMemberName()))
				assert(InvokerBytecodeGenerator.isStaticallyInvocable(ArrayAccessor.OBJECT_ARRAY_SETTER.internalMemberName()))
				ARRAYS = makeArrays()
				FILL_ARRAYS = makeFillArrays()

				Try
					NF_checkSpreadArgument = New NamedFunction(MHI.getDeclaredMethod("checkSpreadArgument", GetType(Object), GetType(Integer)))
					NF_guardWithCatch = New NamedFunction(MHI.getDeclaredMethod("guardWithCatch", GetType(MethodHandle), GetType(Class), GetType(MethodHandle), GetType(Object())))
					NF_throwException = New NamedFunction(MHI.getDeclaredMethod("throwException", GetType(Throwable)))
					NF_profileBoolean = New NamedFunction(MHI.getDeclaredMethod("profileBoolean", GetType(Boolean), GetType(Integer())))

					NF_checkSpreadArgument.resolve()
					NF_guardWithCatch.resolve()
					NF_throwException.resolve()
					NF_profileBoolean.resolve()

					MH_castReference = IMPL_LOOKUP.findStatic(MHI, "castReference", MethodType.methodType(GetType(Object), GetType(Class), GetType(Object)))
					MH_copyAsPrimitiveArray = IMPL_LOOKUP.findStatic(MHI, "copyAsPrimitiveArray", MethodType.methodType(GetType(Object), GetType(sun.invoke.util.Wrapper), GetType(Object())))
					MH_arrayIdentity = IMPL_LOOKUP.findStatic(MHI, "identity", MethodType.methodType(GetType(Object()), GetType(Object())))
					MH_fillNewArray = IMPL_LOOKUP.findStatic(MHI, "fillNewArray", MethodType.methodType(GetType(Object()), GetType(Integer), GetType(Object())))
					MH_fillNewTypedArray = IMPL_LOOKUP.findStatic(MHI, "fillNewTypedArray", MethodType.methodType(GetType(Object()), GetType(Object()), GetType(Integer), GetType(Object())))

					MH_selectAlternative = makeIntrinsic(IMPL_LOOKUP.findStatic(MHI, "selectAlternative", MethodType.methodType(GetType(MethodHandle), GetType(Boolean), GetType(MethodHandle), GetType(MethodHandle))), Intrinsic.SELECT_ALTERNATIVE)
				Catch ex As ReflectiveOperationException
					Throw newInternalError(ex)
				End Try
				Dim THIS_CLASS As  [Class] = GetType(CountingWrapper)
				Try
					NF_maybeStopCounting = New NamedFunction(THIS_CLASS.getDeclaredMethod("maybeStopCounting", GetType(Object)))
				Catch ex As ReflectiveOperationException
					Throw newInternalError(ex)
				End Try
				Dim THIS_CLASS As  [Class] = GetType(BindCaller)
				assert(checkCallerClass(THIS_CLASS, THIS_CLASS))
				Try
					MH_checkCallerClass = IMPL_LOOKUP.findStatic(THIS_CLASS, "checkCallerClass", MethodType.methodType(GetType(Boolean), GetType(Class), GetType(Class)))
					assert(CBool(MH_checkCallerClass.invokeExact(THIS_CLASS, THIS_CLASS)))
				Catch ex As Throwable
					Throw New InternalError(ex)
				End Try
				Dim values As Object() = {Nothing}
				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
				T_BYTES = CType(values(0), SByte())
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As Void
				values(0) =  java.lang.[Integer].getInteger(GetType(MethodHandleImpl).name & ".MAX_ARITY", 255)
				Return Nothing
			End Function
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				Try
					Dim tClass As  [Class] = GetType(T)
					Dim tName As String = tClass.name
					Dim tResource As String = tName.Substring(tName.LastIndexOf("."c)+1) & ".class"
					Dim uconn As java.net.URLConnection = tClass.getResource(tResource).openConnection()
					Dim len As Integer = uconn.contentLength
					Dim bytes As SByte() = New SByte(len - 1){}
					Using str As java.io.InputStream = uconn.inputStream
						Dim nr As Integer = str.read(bytes)
						If nr <> len Then Throw New java.io.IOException(tResource)
					End Using
					values(0) = bytes
				Catch ex As java.io.IOException
					Throw New InternalError(ex)
				End Try
				Return Nothing
			End Function
		End Class

		'/ Factory methods to create method handles:

		Friend Shared Sub initStatics()
			' Trigger selected static initializations.
			MemberName.Factory.INSTANCE.GetType()
		End Sub

		Friend Shared Function makeArrayElementAccessor(  arrayClass As [Class],   isSetter As Boolean) As MethodHandle
			If arrayClass Is GetType(Object()) Then Return (If(isSetter, ArrayAccessor.OBJECT_ARRAY_SETTER, ArrayAccessor.OBJECT_ARRAY_GETTER))
			If Not arrayClass.array Then Throw newIllegalArgumentException("not an array: " & arrayClass)
			Dim cache As MethodHandle() = ArrayAccessor.TYPED_ACCESSORS.get(arrayClass)
			Dim cacheIndex As Integer = (If(isSetter, ArrayAccessor.SETTER_INDEX, ArrayAccessor.GETTER_INDEX))
			Dim mh As MethodHandle = cache(cacheIndex)
			If mh IsNot Nothing Then Return mh
			mh = ArrayAccessor.getAccessor(arrayClass, isSetter)
			Dim correctType As MethodType = ArrayAccessor.correctType(arrayClass, isSetter)
			If mh.type() IsNot correctType Then
				assert(mh.type().parameterType(0) Is GetType(Object()))
				assert((If(isSetter, mh.type().parameterType(2), mh.type().returnType())) = GetType(Object))
				assert(isSetter OrElse correctType.parameterType(0).componentType Is correctType.returnType())
				' safe to view non-strictly, because element type follows from array type
				mh = mh.viewAsType(correctType, False)
			End If
			mh = makeIntrinsic(mh, (If(isSetter, Intrinsic.ARRAY_STORE, Intrinsic.ARRAY_LOAD)))
			' Atomically update accessor cache.
			SyncLock cache
				If cache(cacheIndex) Is Nothing Then
					cache(cacheIndex) = mh
				Else
					' Throw away newly constructed accessor and use cached version.
					mh = cache(cacheIndex)
				End If
			End SyncLock
			Return mh
		End Function

		Friend NotInheritable Class ArrayAccessor
			'/ Support for array element access
			Friend Const GETTER_INDEX As Integer = 0, SETTER_INDEX As Integer = 1, INDEX_LIMIT As Integer = 2
			Friend Shared ReadOnly TYPED_ACCESSORS As  [Class]Value(Of MethodHandle()) = New ClassValueAnonymousInnerClassHelper(Of T)

			Private Class ClassValueAnonymousInnerClassHelper(Of T)
				Inherits ClassValue(Of T)

				Protected Friend Overrides Function computeValue(  type As [Class]) As MethodHandle()
					Return New MethodHandle(INDEX_LIMIT - 1){}
				End Function
			End Class
			Friend Shared ReadOnly OBJECT_ARRAY_GETTER, OBJECT_ARRAY_SETTER As MethodHandle

			Friend Shared Function getElementI(  a As Integer(),   i As Integer) As Integer
				Return a(i)
			End Function
			Friend Shared Function getElementJ(  a As Long(),   i As Integer) As Long
				Return a(i)
			End Function
			Friend Shared Function getElementF(  a As Single(),   i As Integer) As Single
				Return a(i)
			End Function
			Friend Shared Function getElementD(  a As Double(),   i As Integer) As Double
				Return a(i)
			End Function
			Friend Shared Function getElementZ(  a As Boolean(),   i As Integer) As Boolean
				Return a(i)
			End Function
			Friend Shared Function getElementB(  a As SByte(),   i As Integer) As SByte
				Return a(i)
			End Function
			Friend Shared Function getElementS(  a As Short(),   i As Integer) As Short
				Return a(i)
			End Function
			Friend Shared Function getElementC(  a As Char(),   i As Integer) As Char
				Return a(i)
			End Function
			Friend Shared Function getElementL(  a As Object(),   i As Integer) As Object
				Return a(i)
			End Function

			Friend Shared Sub setElementI(  a As Integer(),   i As Integer,   x As Integer)
				a(i) = x
			End Sub
			Friend Shared Sub setElementJ(  a As Long(),   i As Integer,   x As Long)
				a(i) = x
			End Sub
			Friend Shared Sub setElementF(  a As Single(),   i As Integer,   x As Single)
				a(i) = x
			End Sub
			Friend Shared Sub setElementD(  a As Double(),   i As Integer,   x As Double)
				a(i) = x
			End Sub
			Friend Shared Sub setElementZ(  a As Boolean(),   i As Integer,   x As Boolean)
				a(i) = x
			End Sub
			Friend Shared Sub setElementB(  a As SByte(),   i As Integer,   x As SByte)
				a(i) = x
			End Sub
			Friend Shared Sub setElementS(  a As Short(),   i As Integer,   x As Short)
				a(i) = x
			End Sub
			Friend Shared Sub setElementC(  a As Char(),   i As Integer,   x As Char)
				a(i) = x
			End Sub
			Friend Shared Sub setElementL(  a As Object(),   i As Integer,   x As Object)
				a(i) = x
			End Sub

			Friend Shared Function name(  arrayClass As [Class],   isSetter As Boolean) As String
				Dim elemClass As  [Class] = arrayClass.componentType
				If elemClass Is Nothing Then Throw newIllegalArgumentException("not an array", arrayClass)
				Return (If((Not isSetter), "getElement", "setElement")) + sun.invoke.util.Wrapper.basicTypeChar(elemClass)
			End Function
			Friend Shared Function type(  arrayClass As [Class],   isSetter As Boolean) As MethodType
				Dim elemClass As  [Class] = arrayClass.componentType
				Dim arrayArgClass As  [Class] = arrayClass
				If Not elemClass.primitive Then
					arrayArgClass = GetType(Object())
					elemClass = GetType(Object)
				End If
				Return If((Not isSetter), MethodType.methodType(elemClass, arrayArgClass, GetType(Integer)), MethodType.methodType(GetType(void), arrayArgClass, GetType(Integer), elemClass))
			End Function
			Friend Shared Function correctType(  arrayClass As [Class],   isSetter As Boolean) As MethodType
				Dim elemClass As  [Class] = arrayClass.componentType
				Return If((Not isSetter), MethodType.methodType(elemClass, arrayClass, GetType(Integer)), MethodType.methodType(GetType(void), arrayClass, GetType(Integer), elemClass))
			End Function
			Friend Shared Function getAccessor(  arrayClass As [Class],   isSetter As Boolean) As MethodHandle
				Dim name As String = name(arrayClass, isSetter)
				Dim type As MethodType = type(arrayClass, isSetter)
				Try
					Return IMPL_LOOKUP.findStatic(GetType(ArrayAccessor), name, type)
				Catch ex As ReflectiveOperationException
					Throw uncaughtException(ex)
				End Try
			End Function
		End Class

		''' <summary>
		''' Create a JVM-level adapter method handle to conform the given method
		''' handle to the similar newType, using only pairwise argument conversions.
		''' For each argument, convert incoming argument to the exact type needed.
		''' The argument conversions allowed are casting, boxing and unboxing,
		''' integral widening or narrowing, and floating point widening or narrowing. </summary>
		''' <param name="srcType"> required call type </param>
		''' <param name="target"> original method handle </param>
		''' <param name="strict"> if true, only asType conversions are allowed; if false, explicitCastArguments conversions allowed </param>
		''' <param name="monobox"> if true, unboxing conversions are assumed to be exactly typed (Integer to int only, not long or double) </param>
		''' <returns> an adapter to the original handle with the desired new type,
		'''          or the original target if the types are already identical
		'''          or null if the adaptation cannot be made </returns>
		Friend Shared Function makePairwiseConvert(  target As MethodHandle,   srcType As MethodType,   [strict] As Boolean,   monobox As Boolean) As MethodHandle
			Dim dstType As MethodType = target.type()
			If srcType Is dstType Then Return target
			Return makePairwiseConvertByEditor(target, srcType, [strict], monobox)
		End Function

		Private Shared Function countNonNull(  array As Object()) As Integer
			Dim count As Integer = 0
			For Each x As Object In array
				If x IsNot Nothing Then count += 1
			Next x
			Return count
		End Function

		Friend Shared Function makePairwiseConvertByEditor(  target As MethodHandle,   srcType As MethodType,   [strict] As Boolean,   monobox As Boolean) As MethodHandle
			Dim convSpecs As Object() = computeValueConversions(srcType, target.type(), [strict], monobox)
			Dim convCount As Integer = countNonNull(convSpecs)
			If convCount = 0 Then Return target.viewAsType(srcType, [strict])
			Dim basicSrcType As MethodType = srcType.basicType()
			Dim midType As MethodType = target.type().basicType()
			Dim mh As BoundMethodHandle = target.rebind()
			' FIXME: Reduce number of bindings when there is more than one Class conversion.
			' FIXME: Reduce number of bindings when there are repeated conversions.
			For i As Integer = 0 To convSpecs.Length-2
				Dim convSpec As Object = convSpecs(i)
				If convSpec Is Nothing Then Continue For
				Dim fn As MethodHandle
				If TypeOf convSpec Is Class Then
					fn = Lazy.MH_castReference.bindTo(convSpec)
				Else
					fn = CType(convSpec, MethodHandle)
				End If
				Dim newType As  [Class] = basicSrcType.parameterType(i)
				convCount -= 1
				If convCount = 0 Then
					midType = srcType
				Else
					midType = midType.changeParameterType(i, newType)
				End If
				Dim form2 As LambdaForm = mh.editor().filterArgumentForm(1+i, BasicType.basicType(newType))
				mh = mh.copyWithExtendL(midType, form2, fn)
				mh = mh.rebind()
			Next i
			Dim convSpec As Object = convSpecs(convSpecs.Length-1)
			If convSpec IsNot Nothing Then
				Dim fn As MethodHandle
				If TypeOf convSpec Is Class Then
					If convSpec Is GetType(void) Then
						fn = Nothing
					Else
						fn = Lazy.MH_castReference.bindTo(convSpec)
					End If
				Else
					fn = CType(convSpec, MethodHandle)
				End If
				Dim newType As  [Class] = basicSrcType.returnType()
				convCount -= 1
				assert(convCount = 0)
				midType = srcType
				If fn IsNot Nothing Then
					mh = mh.rebind() ' rebind if too complex
					Dim form2 As LambdaForm = mh.editor().filterReturnForm(BasicType.basicType(newType), False)
					mh = mh.copyWithExtendL(midType, form2, fn)
				Else
					Dim form2 As LambdaForm = mh.editor().filterReturnForm(BasicType.basicType(newType), True)
					mh = mh.copyWith(midType, form2)
				End If
			End If
			assert(convCount = 0)
			assert(mh.type().Equals(srcType))
			Return mh
		End Function

		Friend Shared Function makePairwiseConvertIndirect(  target As MethodHandle,   srcType As MethodType,   [strict] As Boolean,   monobox As Boolean) As MethodHandle
			assert(target.type().parameterCount() = srcType.parameterCount())
			' Calculate extra arguments (temporaries) required in the names array.
			Dim convSpecs As Object() = computeValueConversions(srcType, target.type(), [strict], monobox)
			Dim INARG_COUNT As Integer = srcType.parameterCount()
			Dim convCount As Integer = countNonNull(convSpecs)
			Dim retConv As Boolean = (convSpecs(INARG_COUNT) IsNot Nothing)
			Dim retVoid As Boolean = srcType.returnType() Is GetType(void)
			If retConv AndAlso retVoid Then
				convCount -= 1
				retConv = False
			End If

			Const IN_MH As Integer = 0
			Const INARG_BASE As Integer = 1
			Dim INARG_LIMIT As Integer = INARG_BASE + INARG_COUNT
			Dim NAME_LIMIT As Integer = INARG_LIMIT + convCount + 1
			Dim RETURN_CONV As Integer = (If((Not retConv), -1, NAME_LIMIT - 1))
			Dim OUT_CALL As Integer = (If((Not retConv), NAME_LIMIT, RETURN_CONV)) - 1
			Dim RESULT As Integer = (If(retVoid, -1, NAME_LIMIT - 1))

			' Now build a LambdaForm.
			Dim lambdaType As MethodType = srcType.basicType().invokerType()
			Dim names As Name() = arguments(NAME_LIMIT - INARG_LIMIT, lambdaType)

			' Collect the arguments to the outgoing call, maybe with conversions:
			Const OUTARG_BASE As Integer = 0 ' target MH is Name.function, name Name.arguments[0]
			Dim outArgs As Object() = New Object(OUTARG_BASE + INARG_COUNT - 1){}

			Dim nameCursor As Integer = INARG_LIMIT
			For i As Integer = 0 To INARG_COUNT - 1
				Dim convSpec As Object = convSpecs(i)
				If convSpec Is Nothing Then
					' do nothing: difference is trivial
					outArgs(OUTARG_BASE + i) = names(INARG_BASE + i)
					Continue For
				End If

				Dim conv As Name
				If TypeOf convSpec Is Class Then
					Dim convClass As  [Class] = CType(convSpec, [Class])
					conv = New Name(Lazy.MH_castReference, convClass, names(INARG_BASE + i))
				Else
					Dim fn As MethodHandle = CType(convSpec, MethodHandle)
					conv = New Name(fn, names(INARG_BASE + i))
				End If
				assert(names(nameCursor) Is Nothing)
				names(nameCursor) = conv
				nameCursor += 1
				assert(outArgs(OUTARG_BASE + i) Is Nothing)
				outArgs(OUTARG_BASE + i) = conv
			Next i

			' Build argument array for the call.
			assert(nameCursor = OUT_CALL)
			names(OUT_CALL) = New Name(target, outArgs)

			Dim convSpec As Object = convSpecs(INARG_COUNT)
			If Not retConv Then
				assert(OUT_CALL = names.Length-1)
			Else
				Dim conv As Name
				If convSpec Is GetType(void) Then
					conv = New Name(LambdaForm.constantZero(BasicType.basicType(srcType.returnType())))
				ElseIf TypeOf convSpec Is Class Then
					Dim convClass As  [Class] = CType(convSpec, [Class])
					conv = New Name(Lazy.MH_castReference, convClass, names(OUT_CALL))
				Else
					Dim fn As MethodHandle = CType(convSpec, MethodHandle)
					If fn.type().parameterCount() = 0 Then
						conv = New Name(fn) ' don't pass retval to  Sub  conversion
					Else
						conv = New Name(fn, names(OUT_CALL))
					End If
				End If
				assert(names(RETURN_CONV) Is Nothing)
				names(RETURN_CONV) = conv
				assert(RETURN_CONV = names.Length-1)
			End If

			Dim form As New LambdaForm("convert", lambdaType.parameterCount(), names, RESULT)
			Return SimpleMethodHandle.make(srcType, form)
		End Function

		''' <summary>
		''' Identity function, with reference cast. </summary>
		''' <param name="t"> an arbitrary reference type </param>
		''' <param name="x"> an arbitrary reference value </param>
		''' <returns> the same value x </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function castReference(Of T, U)(  t As [Class],   x As U) As T
			' inlined Class.cast because we can't ForceInline it
			If x IsNot Nothing AndAlso (Not t.isInstance(x)) Then Throw newClassCastException(t, x)
			Return CType(x, T)
		End Function

		Private Shared Function newClassCastException(  t As [Class],   obj As Object) As  ClassCastException
			Return New ClassCastException("Cannot cast " & obj.GetType().name & " to " & t.name)
		End Function

		Friend Shared Function computeValueConversions(  srcType As MethodType,   dstType As MethodType,   [strict] As Boolean,   monobox As Boolean) As Object()
			Dim INARG_COUNT As Integer = srcType.parameterCount()
			Dim convSpecs As Object() = New Object(INARG_COUNT){}
			For i As Integer = 0 To INARG_COUNT
				Dim isRet As Boolean = (i = INARG_COUNT)
				Dim src As  [Class] = If(isRet, dstType.returnType(), srcType.parameterType(i))
				Dim dst As  [Class] = If(isRet, srcType.returnType(), dstType.parameterType(i))
				If Not sun.invoke.util.VerifyType.isNullConversion(src, dst, [strict]) Then 'keepInterfaces= convSpecs(i) = valueConversion(src, dst, [strict], monobox)
			Next i
			Return convSpecs
		End Function
		Friend Shared Function makePairwiseConvert(  target As MethodHandle,   srcType As MethodType,   [strict] As Boolean) As MethodHandle
			Return makePairwiseConvert(target, srcType, [strict], False) 'monobox=
		End Function

		''' <summary>
		''' Find a conversion function from the given source to the given destination.
		''' This conversion function will be used as a LF NamedFunction.
		''' Return a Class object if a simple cast is needed.
		''' Return void.class if  Sub  is involved.
		''' </summary>
		Friend Shared Function valueConversion(  src As [Class],   dst As [Class],   [strict] As Boolean,   monobox As Boolean) As Object
			assert((Not sun.invoke.util.VerifyType.isNullConversion(src, dst, [strict]))) ' caller responsibility - keepInterfaces=
			If dst Is GetType(void) Then Return dst
			Dim fn As MethodHandle
			If src.primitive Then
				If src Is GetType(void) Then
					Return GetType(void) ' caller must recognize this specially
				ElseIf dst.primitive Then
					' Examples: int->byte, byte->int, boolean->int (!strict)
					fn = sun.invoke.util.ValueConversions.convertPrimitive(src, dst)
				Else
					' Examples: int->Integer, boolean->Object, float->Number
					Dim wsrc As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(src)
					fn = sun.invoke.util.ValueConversions.boxExact(wsrc)
					assert(fn.type().parameterType(0) Is wsrc.primitiveType())
					assert(fn.type().returnType() Is wsrc.wrapperType())
					If Not sun.invoke.util.VerifyType.isNullConversion(wsrc.wrapperType(), dst, [strict]) Then
						' Corner case, such as int->Long, which will probably fail.
						Dim mt As MethodType = MethodType.methodType(dst, src)
						If [strict] Then
							fn = fn.asType(mt)
						Else
							fn = MethodHandleImpl.makePairwiseConvert(fn, mt, False) 'strict=
						End If
					End If
				End If
			ElseIf dst.primitive Then
				Dim wdst As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(dst)
				If monobox OrElse src Is wdst.wrapperType() Then
					' Use a strongly-typed unboxer, if possible.
					fn = sun.invoke.util.ValueConversions.unboxExact(wdst, [strict])
				Else
					' Examples:  Object->int, Number->int, Comparable->int, Byte->int
					' must include additional conversions
					' src must be examined at runtime, to detect Byte, Character, etc.
					fn = (If([strict], sun.invoke.util.ValueConversions.unboxWiden(wdst), sun.invoke.util.ValueConversions.unboxCast(wdst)))
				End If
			Else
				' Simple reference conversion.
				' Note:  Do not check for a class hierarchy relation
				' between src and dst.  In all cases a 'null' argument
				' will pass the cast conversion.
				Return dst
			End If
			assert(fn.type().parameterCount() <= 1) : "pc" & java.util.Arrays.asList(src.simpleName, dst.simpleName, fn)
			Return fn
		End Function

		Friend Shared Function makeVarargsCollector(  target As MethodHandle,   arrayType As [Class]) As MethodHandle
			Dim type As MethodType = target.type()
			Dim last As Integer = type.parameterCount() - 1
			If type.parameterType(last) IsNot arrayType Then target = target.asType(type.changeParameterType(last, arrayType))
			target = target.asFixedArity() ' make sure this attribute is turned off
			Return New AsVarargsCollector(target, arrayType)
		End Function

		Private NotInheritable Class AsVarargsCollector
			Inherits DelegatingMethodHandle

			Private ReadOnly target As MethodHandle
			Private ReadOnly arrayType As  [Class]
			Private MethodHandle As Stable

			Friend Sub New(  target As MethodHandle,   arrayType As [Class])
				Me.New(target.type(), target, arrayType)
			End Sub
			Friend Sub New(  type As MethodType,   target As MethodHandle,   arrayType As [Class])
				MyBase.New(type, target)
				Me.target = target
				Me.arrayType = arrayType
				Me.asCollectorCache = target.asCollector(arrayType, 0)
			End Sub

			Public  Overrides ReadOnly Property  varargsCollector As Boolean
				Get
					Return True
				End Get
			End Property

			Protected Friend  Overrides ReadOnly Property  target As MethodHandle
				Get
					Return target
				End Get
			End Property

			Public Overrides Function asFixedArity() As MethodHandle
				Return target
			End Function

			Friend Overrides Function setVarargs(  member As MemberName) As MethodHandle
				If member.varargs Then Return Me
				Return asFixedArity()
			End Function

			Public Overrides Function asTypeUncached(  newType As MethodType) As MethodHandle
				Dim type As MethodType = Me.type()
				Dim collectArg As Integer = type.parameterCount() - 1
				Dim newArity As Integer = newType.parameterCount()
				If newArity = collectArg+1 AndAlso newType.parameterType(collectArg).IsSubclassOf(type.parameterType(collectArg)) Then
					' if arity and trailing parameter are compatible, do normal thing
						asTypeCache = asFixedArity().asType(newType)
						Return asTypeCache
				End If
				' check cache
				Dim acc As MethodHandle = asCollectorCache
				If acc IsNot Nothing AndAlso acc.type().parameterCount() = newArity Then
						asTypeCache = acc.asType(newType)
						Return asTypeCache
				End If
				' build and cache a collector
				Dim arrayLength As Integer = newArity - collectArg
				Dim collector As MethodHandle
				Try
					collector = asFixedArity().asCollector(arrayType, arrayLength)
					assert(collector.type().parameterCount() = newArity) : "newArity=" & newArity & " but collector=" & collector
				Catch ex As IllegalArgumentException
					Throw New WrongMethodTypeException("cannot build collector", ex)
				End Try
				asCollectorCache = collector
					asTypeCache = collector.asType(newType)
					Return asTypeCache
			End Function

			Friend Overrides Function viewAsTypeChecks(  newType As MethodType,   [strict] As Boolean) As Boolean
				MyBase.viewAsTypeChecks(newType, True)
				If [strict] Then Return True
				' extra assertion for non-strict checks:
				assert(newType.lastParameterType().componentType.IsSubclassOf(type().lastParameterType().componentType)) : java.util.Arrays.asList(Me, newType)
				Return True
			End Function
		End Class

		''' <summary>
		''' Factory method:  Spread selected argument. </summary>
		Friend Shared Function makeSpreadArguments(  target As MethodHandle,   spreadArgType As [Class],   spreadArgPos As Integer,   spreadArgCount As Integer) As MethodHandle
			Dim targetType As MethodType = target.type()

			For i As Integer = 0 To spreadArgCount - 1
				Dim arg As  [Class] = sun.invoke.util.VerifyType.spreadArgElementType(spreadArgType, i)
				If arg Is Nothing Then arg = GetType(Object)
				targetType = targetType.changeParameterType(spreadArgPos + i, arg)
			Next i
			target = target.asType(targetType)

			Dim srcType As MethodType = targetType.replaceParameterTypes(spreadArgPos, spreadArgPos + spreadArgCount, spreadArgType)
			' Now build a LambdaForm.
			Dim lambdaType As MethodType = srcType.invokerType()
			Dim names As Name() = arguments(spreadArgCount + 2, lambdaType)
			Dim nameCursor As Integer = lambdaType.parameterCount()
			Dim indexes As Integer() = New Integer(targetType.parameterCount() - 1){}

			Dim i As Integer = 0
			Dim argIndex As Integer = 1
			Do While i < targetType.parameterCount() + 1
				Dim src As  [Class] = lambdaType.parameterType(i)
				If i = spreadArgPos Then
					' Spread the array.
					Dim aload As MethodHandle = MethodHandles.arrayElementGetter(spreadArgType)
					Dim array As Name = names(argIndex)
					names(nameCursor) = New Name(Lazy.NF_checkSpreadArgument, array, spreadArgCount)
					nameCursor += 1
					Dim j As Integer = 0
					Do While j < spreadArgCount
						indexes(i) = nameCursor
						names(nameCursor) = New Name(aload, array, j)
						nameCursor += 1
						i += 1
						j += 1
					Loop
				ElseIf i < indexes.Length Then
					indexes(i) = argIndex
				End If
				i += 1
				argIndex += 1
			Loop
			assert(nameCursor = names.Length-1) ' leave room for the final call

			' Build argument array for the call.
			Dim targetArgs As Name() = New Name(targetType.parameterCount() - 1){}
			For i As Integer = 0 To targetType.parameterCount() - 1
				Dim idx As Integer = indexes(i)
				targetArgs(i) = names(idx)
			Next i
			names(names.Length - 1) = New Name(target, CType(targetArgs, Object()))

			Dim form As New LambdaForm("spread", lambdaType.parameterCount(), names)
			Return SimpleMethodHandle.make(srcType, form)
		End Function

		Friend Shared Sub checkSpreadArgument(  av As Object,   n As Integer)
			If av Is Nothing Then
				If n = 0 Then Return
			ElseIf TypeOf av Is Object() Then
				Dim len As Integer = CType(av, Object()).Length
				If len = n Then Return
			Else
				Dim len As Integer = java.lang.reflect.Array.getLength(av)
				If len = n Then Return
			End If
			' fall through to error:
			Throw newIllegalArgumentException("array is not of length " & n)
		End Sub

		''' <summary>
		''' Pre-initialized NamedFunctions for bootstrapping purposes.
		''' Factored in an inner class to delay initialization until first usage.
		''' </summary>
		Friend Class Lazy
			Private Shared ReadOnly MHI As  [Class] = GetType(MethodHandleImpl)

			Private Shared ReadOnly ARRAYS As MethodHandle()
			Private Shared ReadOnly FILL_ARRAYS As MethodHandle()

			Friend Shared ReadOnly NF_checkSpreadArgument As NamedFunction
			Friend Shared ReadOnly NF_guardWithCatch As NamedFunction
			Friend Shared ReadOnly NF_throwException As NamedFunction
			Friend Shared ReadOnly NF_profileBoolean As NamedFunction

			Friend Shared ReadOnly MH_castReference As MethodHandle
			Friend Shared ReadOnly MH_selectAlternative As MethodHandle
			Friend Shared ReadOnly MH_copyAsPrimitiveArray As MethodHandle
			Friend Shared ReadOnly MH_fillNewTypedArray As MethodHandle
			Friend Shared ReadOnly MH_fillNewArray As MethodHandle
			Friend Shared ReadOnly MH_arrayIdentity As MethodHandle

		End Class

		''' <summary>
		''' Factory method:  Collect or filter selected argument(s). </summary>
		Friend Shared Function makeCollectArguments(  target As MethodHandle,   collector As MethodHandle,   collectArgPos As Integer,   retainOriginalArgs As Boolean) As MethodHandle
			Dim targetType As MethodType = target.type() ' (a..., c, [b...])=>r
			Dim collectorType As MethodType = collector.type() ' (b...)=>c
			Dim collectArgCount As Integer = collectorType.parameterCount()
			Dim collectValType As  [Class] = collectorType.returnType()
			Dim collectValCount As Integer = (If(collectValType Is GetType(void), 0, 1))
			Dim srcType As MethodType = targetType.dropParameterTypes(collectArgPos, collectArgPos+collectValCount) ' (a..., [b...])=>r
			If Not retainOriginalArgs Then ' (a..., b...)=>r srcType = srcType.insertParameterTypes(collectArgPos, collectorType.parameterList())
			' in  arglist: [0: ...keep1 | cpos: collect...  | cpos+cacount: keep2... ]
			' out arglist: [0: ...keep1 | cpos: collectVal? | cpos+cvcount: keep2... ]
			' out(retain): [0: ...keep1 | cpos: cV? coll... | cpos+cvc+cac: keep2... ]

			' Now build a LambdaForm.
			Dim lambdaType As MethodType = srcType.invokerType()
			Dim names As Name() = arguments(2, lambdaType)
			Dim collectNamePos As Integer = names.Length - 2
			Dim targetNamePos As Integer = names.Length - 1

			Dim collectorArgs As Name() = java.util.Arrays.copyOfRange(names, 1 + collectArgPos, 1 + collectArgPos + collectArgCount)
			names(collectNamePos) = New Name(collector, CType(collectorArgs, Object()))

			' Build argument array for the target.
			' Incoming LF args to copy are: [ (mh) headArgs collectArgs tailArgs ].
			' Output argument array is [ headArgs (collectVal)? (collectArgs)? tailArgs ].
			Dim targetArgs As Name() = New Name(targetType.parameterCount() - 1){}
			Dim inputArgPos As Integer = 1 ' incoming LF args to copy to target
			Dim targetArgPos As Integer = 0 ' fill pointer for targetArgs
			Dim chunk As Integer = collectArgPos ' |headArgs|
			Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk)
			inputArgPos += chunk
			targetArgPos += chunk
			If collectValType IsNot GetType(void) Then
				targetArgs(targetArgPos) = names(collectNamePos)
				targetArgPos += 1
			End If
			chunk = collectArgCount
			If retainOriginalArgs Then
				Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk)
				targetArgPos += chunk ' optionally pass on the collected chunk
			End If
			inputArgPos += chunk
			chunk = targetArgs.Length - targetArgPos ' all the rest
			Array.Copy(names, inputArgPos, targetArgs, targetArgPos, chunk)
			assert(inputArgPos + chunk = collectNamePos) ' use of rest of input args also
			names(targetNamePos) = New Name(target, CType(targetArgs, Object()))

			Dim form As New LambdaForm("collect", lambdaType.parameterCount(), names)
			Return SimpleMethodHandle.make(srcType, form)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function selectAlternative(  testResult As Boolean,   target As MethodHandle,   fallback As MethodHandle) As MethodHandle
			If testResult Then
				Return target
			Else
				Return fallback
			End If
		End Function

		' Intrinsified by C2. Counters are used during parsing to calculate branch frequencies.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function profileBoolean(  result As Boolean,   counters As Integer()) As Boolean
			' Profile is int[2] where [0] and [1] correspond to false and true occurrences respectively.
			Dim idx As Integer = If(result, 1, 0)
			Try
				counters(idx) = System.Math.addExact(counters(idx), 1)
			Catch e As ArithmeticException
				' Avoid continuous overflow by halving the problematic count.
				counters(idx) = counters(idx) \ 2
			End Try
			Return result
		End Function

		Friend Shared Function makeGuardWithTest(  test As MethodHandle,   target As MethodHandle,   fallback As MethodHandle) As MethodHandle
			Dim type As MethodType = target.type()
			assert(test.type().Equals(type.changeReturnType(GetType(Boolean))) AndAlso fallback.type().Equals(type))
			Dim basicType As MethodType = type.basicType()
			Dim form As LambdaForm = makeGuardWithTestForm(basicType)
			Dim mh As BoundMethodHandle
			Try
				If PROFILE_GWT Then
					Dim counts As Integer() = New Integer(1){}
					mh = CType(BoundMethodHandle.speciesData_LLLL().constructor().invokeBasic(type, form, CObj(test), CObj(profile(target)), CObj(profile(fallback)), counts), BoundMethodHandle)
				Else
					mh = CType(BoundMethodHandle.speciesData_LLL().constructor().invokeBasic(type, form, CObj(test), CObj(profile(target)), CObj(profile(fallback))), BoundMethodHandle)
				End If
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
			assert(mh.type() Is type)
			Return mh
		End Function


		Friend Shared Function profile(  target As MethodHandle) As MethodHandle
			If DONT_INLINE_THRESHOLD >= 0 Then
				Return makeBlockInlningWrapper(target)
			Else
				Return target
			End If
		End Function

		''' <summary>
		''' Block inlining during JIT-compilation of a target method handle if it hasn't been invoked enough times.
		''' Corresponding LambdaForm has @DontInline when compiled into bytecode.
		''' </summary>
		Friend Shared Function makeBlockInlningWrapper(  target As MethodHandle) As MethodHandle
			Dim lform As LambdaForm = PRODUCE_BLOCK_INLINING_FORM.apply(target)
			Return New CountingWrapper(target, lform, PRODUCE_BLOCK_INLINING_FORM, PRODUCE_REINVOKER_FORM, DONT_INLINE_THRESHOLD)
		End Function

		''' <summary>
		''' Constructs reinvoker lambda form which block inlining during JIT-compilation for a particular method handle </summary>
		Private Shared ReadOnly PRODUCE_BLOCK_INLINING_FORM As java.util.function.Function(Of MethodHandle, LambdaForm) = New FunctionAnonymousInnerClassHelper(Of T, R)

		Private Class FunctionAnonymousInnerClassHelper(Of T, R)
			Implements java.util.function.Function(Of T, R)

			Public Overrides Function apply(  target As MethodHandle) As LambdaForm
				Return DelegatingMethodHandle.makeReinvokerForm(target, MethodTypeForm.LF_DELEGATE_BLOCK_INLINING, GetType(CountingWrapper), "reinvoker.dontInline", False, DelegatingMethodHandle.NF_getTarget, CountingWrapper.NF_maybeStopCounting)
			End Function
		End Class

		''' <summary>
		''' Constructs simple reinvoker lambda form for a particular method handle </summary>
		Private Shared ReadOnly PRODUCE_REINVOKER_FORM As java.util.function.Function(Of MethodHandle, LambdaForm) = New FunctionAnonymousInnerClassHelper2(Of T, R)

		Private Class FunctionAnonymousInnerClassHelper2(Of T, R)
			Implements java.util.function.Function(Of T, R)

			Public Overrides Function apply(  target As MethodHandle) As LambdaForm
				Return DelegatingMethodHandle.makeReinvokerForm(target, MethodTypeForm.LF_DELEGATE, GetType(DelegatingMethodHandle), DelegatingMethodHandle.NF_getTarget)
			End Function
		End Class

		''' <summary>
		''' Counting method handle. It has 2 states: counting and non-counting.
		''' It is in counting state for the first n invocations and then transitions to non-counting state.
		''' Behavior in counting and non-counting states is determined by lambda forms produced by
		''' countingFormProducer & nonCountingFormProducer respectively.
		''' </summary>
		Friend Class CountingWrapper
			Inherits DelegatingMethodHandle

			Private ReadOnly target As MethodHandle
			Private count As Integer
			Private countingFormProducer As java.util.function.Function(Of MethodHandle, LambdaForm)
			Private nonCountingFormProducer As java.util.function.Function(Of MethodHandle, LambdaForm)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private isCounting As Boolean

			Private Sub New(  target As MethodHandle,   lform As LambdaForm,   countingFromProducer As java.util.function.Function(Of MethodHandle, LambdaForm),   nonCountingFormProducer As java.util.function.Function(Of MethodHandle, LambdaForm),   count As Integer)
				MyBase.New(target.type(), lform)
				Me.target = target
				Me.count = count
				Me.countingFormProducer = countingFromProducer
				Me.nonCountingFormProducer = nonCountingFormProducer
				Me.isCounting = (count > 0)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Protected Friend  Overrides ReadOnly Property  target As MethodHandle
				Get
					Return target
				End Get
			End Property

			Public Overrides Function asTypeUncached(  newType As MethodType) As MethodHandle
				Dim newTarget As MethodHandle = target.asType(newType)
				Dim wrapper As MethodHandle
				If isCounting Then
					Dim lform As LambdaForm
					lform = countingFormProducer.apply(newTarget)
					wrapper = New CountingWrapper(newTarget, lform, countingFormProducer, nonCountingFormProducer, DONT_INLINE_THRESHOLD)
				Else
					wrapper = newTarget ' no need for a counting wrapper anymore
				End If
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return (asTypeCache = wrapper)
			End Function

			Friend Overridable Function countDown() As Boolean
				If count <= 0 Then
					' Try to limit number of updates. MethodHandle.updateForm() doesn't guarantee LF update visibility.
					If isCounting Then
						isCounting = False
						Return True
					Else
						Return False
					End If
				Else
					count -= 1
					Return False
				End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Sub maybeStopCounting(  o1 As Object)
				 Dim wrapper As CountingWrapper = CType(o1, CountingWrapper)
				 If wrapper.countDown() Then
					 ' Reached invocation threshold. Replace counting behavior with a non-counting one.
					 Dim lform As LambdaForm = wrapper.nonCountingFormProducer.apply(wrapper.target)
					 lform.compileToBytecode() ' speed up warmup by avoiding LF interpretation again after transition
					 wrapper.updateForm(lform)
				 End If
			End Sub

			Friend Shared ReadOnly NF_maybeStopCounting As NamedFunction
		End Class

		Friend Shared Function makeGuardWithTestForm(  basicType As MethodType) As LambdaForm
			Dim lform As LambdaForm = basicType.form().cachedLambdaForm(MethodTypeForm.LF_GWT)
			If lform IsNot Nothing Then Return lform
			Const THIS_MH As Integer = 0 ' the BMH_LLL
			Const ARG_BASE As Integer = 1 ' start of incoming arguments
			Dim ARG_LIMIT As Integer = ARG_BASE + basicType.parameterCount()
			Dim nameCursor As Integer = ARG_LIMIT
			Dim GET_TEST As Integer = nameCursor
			nameCursor += 1
			Dim GET_TARGET As Integer = nameCursor
			nameCursor += 1
			Dim GET_FALLBACK As Integer = nameCursor
			nameCursor += 1
				Dim GET_COUNTERS As Integer = If(PROFILE_GWT, nameCursor, -1)
				nameCursor += 1
			Dim CALL_TEST As Integer = nameCursor
			nameCursor += 1
				Dim PROFILE_Renamed As Integer = If(GET_COUNTERS <> -1, nameCursor, -1)
				nameCursor += 1
			Dim TEST As Integer = nameCursor-1 ' previous statement: either PROFILE or CALL_TEST
			Dim SELECT_ALT As Integer = nameCursor
			nameCursor += 1
			Dim CALL_TARGET As Integer = nameCursor
			nameCursor += 1
			assert(CALL_TARGET = SELECT_ALT+1) ' must be true to trigger IBG.emitSelectAlternative

			Dim lambdaType As MethodType = basicType.invokerType()
			Dim names As Name() = arguments(nameCursor - ARG_LIMIT, lambdaType)

			Dim data As BoundMethodHandle.SpeciesData = If(GET_COUNTERS <> -1, BoundMethodHandle.speciesData_LLLL(), BoundMethodHandle.speciesData_LLL())
			names(THIS_MH) = names(THIS_MH).withConstraint(data)
			names(GET_TEST) = New Name(data.getterFunction(0), names(THIS_MH))
			names(GET_TARGET) = New Name(data.getterFunction(1), names(THIS_MH))
			names(GET_FALLBACK) = New Name(data.getterFunction(2), names(THIS_MH))
			If GET_COUNTERS <> -1 Then names(GET_COUNTERS) = New Name(data.getterFunction(3), names(THIS_MH))
			Dim invokeArgs As Object() = java.util.Arrays.copyOfRange(names, 0, ARG_LIMIT, GetType(Object()))

			' call test
			Dim testType As MethodType = basicType.changeReturnType(GetType(Boolean)).basicType()
			invokeArgs(0) = names(GET_TEST)
			names(CALL_TEST) = New Name(testType, invokeArgs)

			' profile branch
			If PROFILE_Renamed <> -1 Then names(PROFILE_Renamed) = New Name(Lazy.NF_profileBoolean, names(CALL_TEST), names(GET_COUNTERS))
			' call selectAlternative
			names(SELECT_ALT) = New Name(Lazy.MH_selectAlternative, names(TEST), names(GET_TARGET), names(GET_FALLBACK))

			' call target or fallback
			invokeArgs(0) = names(SELECT_ALT)
			names(CALL_TARGET) = New Name(basicType, invokeArgs)

			lform = New LambdaForm("guard", lambdaType.parameterCount(), names, True) 'forceInline=

			Return basicType.form().cachedLambdaFormorm(MethodTypeForm.LF_GWT, lform)
		End Function

		''' <summary>
		''' The LambaForm shape for catchException combinator is the following:
		''' <blockquote><pre>{@code
		'''  guardWithCatch=Lambda(a0:L,a1:L,a2:L)=>{
		'''    t3:L=BoundMethodHandle$Species_LLLLL.argL0(a0:L);
		'''    t4:L=BoundMethodHandle$Species_LLLLL.argL1(a0:L);
		'''    t5:L=BoundMethodHandle$Species_LLLLL.argL2(a0:L);
		'''    t6:L=BoundMethodHandle$Species_LLLLL.argL3(a0:L);
		'''    t7:L=BoundMethodHandle$Species_LLLLL.argL4(a0:L);
		'''    t8:L=MethodHandle.invokeBasic(t6:L,a1:L,a2:L);
		'''    t9:L=MethodHandleImpl.guardWithCatch(t3:L,t4:L,t5:L,t8:L);
		'''   t10:I=MethodHandle.invokeBasic(t7:L,t9:L);t10:I}
		''' }</pre></blockquote>
		''' 
		''' argL0 and argL2 are target and catcher method handles. argL1 is exception class.
		''' argL3 and argL4 are auxiliary method handles: argL3 boxes arguments and wraps them into Object[]
		''' (ValueConversions.array()) and argL4 unboxes result if necessary (ValueConversions.unbox()).
		''' 
		''' Having t8 and t10 passed outside and not hardcoded into a lambda form allows to share lambda forms
		''' among catchException combinators with the same basic type.
		''' </summary>
		Private Shared Function makeGuardWithCatchForm(  basicType As MethodType) As LambdaForm
			Dim lambdaType As MethodType = basicType.invokerType()

			Dim lform As LambdaForm = basicType.form().cachedLambdaForm(MethodTypeForm.LF_GWC)
			If lform IsNot Nothing Then Return lform
			Const THIS_MH As Integer = 0 ' the BMH_LLLLL
			Const ARG_BASE As Integer = 1 ' start of incoming arguments
			Dim ARG_LIMIT As Integer = ARG_BASE + basicType.parameterCount()

			Dim nameCursor As Integer = ARG_LIMIT
			Dim GET_TARGET As Integer = nameCursor
			nameCursor += 1
			Dim GET_CLASS As Integer = nameCursor
			nameCursor += 1
			Dim GET_CATCHER As Integer = nameCursor
			nameCursor += 1
			Dim GET_COLLECT_ARGS As Integer = nameCursor
			nameCursor += 1
			Dim GET_UNBOX_RESULT As Integer = nameCursor
			nameCursor += 1
			Dim BOXED_ARGS As Integer = nameCursor
			nameCursor += 1
			Dim TRY_CATCH As Integer = nameCursor
			nameCursor += 1
			Dim UNBOX_RESULT As Integer = nameCursor
			nameCursor += 1

			Dim names As Name() = arguments(nameCursor - ARG_LIMIT, lambdaType)

			Dim data As BoundMethodHandle.SpeciesData = BoundMethodHandle.speciesData_LLLLL()
			names(THIS_MH) = names(THIS_MH).withConstraint(data)
			names(GET_TARGET) = New Name(data.getterFunction(0), names(THIS_MH))
			names(GET_CLASS) = New Name(data.getterFunction(1), names(THIS_MH))
			names(GET_CATCHER) = New Name(data.getterFunction(2), names(THIS_MH))
			names(GET_COLLECT_ARGS) = New Name(data.getterFunction(3), names(THIS_MH))
			names(GET_UNBOX_RESULT) = New Name(data.getterFunction(4), names(THIS_MH))

			' FIXME: rework argument boxing/result unboxing logic for LF interpretation

			' t_{i}:L=MethodHandle.invokeBasic(collectArgs:L,a1:L,...);
			Dim collectArgsType As MethodType = basicType.changeReturnType(GetType(Object))
			Dim invokeBasic As MethodHandle = MethodHandles.basicInvoker(collectArgsType)
			Dim args As Object() = New Object(invokeBasic.type().parameterCount() - 1){}
			args(0) = names(GET_COLLECT_ARGS)
			Array.Copy(names, ARG_BASE, args, 1, ARG_LIMIT-ARG_BASE)
			names(BOXED_ARGS) = New Name(makeIntrinsic(invokeBasic, Intrinsic.GUARD_WITH_CATCH), args)

			' t_{i+1}:L=MethodHandleImpl.guardWithCatch(target:L,exType:L,catcher:L,t_{i}:L);
			Dim gwcArgs As Object() = {names(GET_TARGET), names(GET_CLASS), names(GET_CATCHER), names(BOXED_ARGS)}
			names(TRY_CATCH) = New Name(Lazy.NF_guardWithCatch, gwcArgs)

			' t_{i+2}:I=MethodHandle.invokeBasic(unbox:L,t_{i+1}:L);
			Dim invokeBasicUnbox As MethodHandle = MethodHandles.basicInvoker(MethodType.methodType(basicType.rtype(), GetType(Object)))
			Dim unboxArgs As Object() = {names(GET_UNBOX_RESULT), names(TRY_CATCH)}
			names(UNBOX_RESULT) = New Name(invokeBasicUnbox, unboxArgs)

			lform = New LambdaForm("guardWithCatch", lambdaType.parameterCount(), names)

			Return basicType.form().cachedLambdaFormorm(MethodTypeForm.LF_GWC, lform)
		End Function

		Friend Shared Function makeGuardWithCatch(  target As MethodHandle,   exType As [Class],   catcher As MethodHandle) As MethodHandle
			Dim type As MethodType = target.type()
			Dim form As LambdaForm = makeGuardWithCatchForm(type.basicType())

			' Prepare auxiliary method handles used during LambdaForm interpreation.
			' Box arguments and wrap them into Object[]: ValueConversions.array().
			Dim varargsType As MethodType = type.changeReturnType(GetType(Object()))
			Dim collectArgs As MethodHandle = varargsArray(type.parameterCount()).asType(varargsType)
			' Result unboxing: ValueConversions.unbox() OR ValueConversions.identity() OR ValueConversions.ignore().
			Dim unboxResult As MethodHandle
			Dim rtype As  [Class] = type.returnType()
			If rtype.primitive Then
				If rtype Is GetType(void) Then
					unboxResult = sun.invoke.util.ValueConversions.ignore()
				Else
					Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(type.returnType())
					unboxResult = sun.invoke.util.ValueConversions.unboxExact(w)
				End If
			Else
				unboxResult = MethodHandles.identity(GetType(Object))
			End If

			Dim data As BoundMethodHandle.SpeciesData = BoundMethodHandle.speciesData_LLLLL()
			Dim mh As BoundMethodHandle
			Try
				mh = CType(data.constructor().invokeBasic(type, form, CObj(target), CObj(exType), CObj(catcher), CObj(collectArgs), CObj(unboxResult)), BoundMethodHandle)
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
			assert(mh.type() Is type)
			Return mh
		End Function

		''' <summary>
		''' Intrinsified during LambdaForm compilation
		''' (see <seealso cref="InvokerBytecodeGenerator#emitGuardWithCatch emitGuardWithCatch"/>).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function guardWithCatch(  target As MethodHandle,   exType As [Class],   catcher As MethodHandle, ParamArray   av As Object()) As Object
			' Use asFixedArity() to avoid unnecessary boxing of last argument for VarargsCollector case.
			Try
				Return target.asFixedArity().invokeWithArguments(av)
			Catch t As Throwable
				If Not exType.isInstance(t) Then Throw t
				Return catcher.asFixedArity().invokeWithArguments(prepend(t, av))
			End Try
		End Function

		''' <summary>
		''' Prepend an element {@code elem} to an {@code array}. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function prepend(  elem As Object,   array As Object()) As Object()
			Dim newArray As Object() = New Object(array.Length){}
			newArray(0) = elem
			Array.Copy(array, 0, newArray, 1, array.Length)
			Return newArray
		End Function

		Friend Shared Function throwException(  type As MethodType) As MethodHandle
			assert(type.parameterType(0).IsSubclassOf(GetType(Throwable)))
			Dim arity As Integer = type.parameterCount()
			If arity > 1 Then
				Dim mh As MethodHandle = throwException(type.dropParameterTypes(1, arity))
				mh = MethodHandles.dropArguments(mh, 1, type.parameterList().subList(1, arity))
				Return mh
			End If
			Return makePairwiseConvert(Lazy.NF_throwException.resolvedHandle(), type, False, True)
		End Function

		Friend Shared Function throwException(Of T As Throwable)(  t As T) As sun.invoke.empty.Empty
			Throw t
		End Function

		Friend Shared FAKE_METHOD_HANDLE_INVOKE As MethodHandle() = New MethodHandle(1){}
		Friend Shared Function fakeMethodHandleInvoke(  method As MemberName) As MethodHandle
			Dim idx As Integer
			assert(method.methodHandleInvoke)
			Select Case method.name
			Case "invoke"
				idx = 0
			Case "invokeExact"
				idx = 1
			Case Else
				Throw New InternalError(method.name)
			End Select
			Dim mh As MethodHandle = FAKE_METHOD_HANDLE_INVOKE(idx)
			If mh IsNot Nothing Then Return mh
			Dim type As MethodType = MethodType.methodType(GetType(Object), GetType(UnsupportedOperationException), GetType(MethodHandle), GetType(Object()))
			mh = throwException(type)
			mh = mh.bindTo(New UnsupportedOperationException("cannot reflectively invoke MethodHandle"))
			If Not method.invocationType.Equals(mh.type()) Then Throw New InternalError(method.ToString())
			mh = mh.withInternalMemberName(method, False)
			mh = mh.asVarargsCollector(GetType(Object()))
			assert(method.varargs)
			FAKE_METHOD_HANDLE_INVOKE(idx) = mh
			Return mh
		End Function

		''' <summary>
		''' Create an alias for the method handle which, when called,
		''' appears to be called from the same class loader and protection domain
		''' as hostClass.
		''' This is an expensive no-op unless the method which is called
		''' is sensitive to its caller.  A small number of system methods
		''' are in this category, including Class.forName and Method.invoke.
		''' </summary>
		Friend Shared Function bindCaller(  mh As MethodHandle,   hostClass As [Class]) As MethodHandle
			Return BindCaller.bindCaller(mh, hostClass)
		End Function

		' Put the whole mess into its own nested class.
		' That way we can lazily load the code and set up the constants.
		Private Class BindCaller
			Shared Function bindCaller(  mh As MethodHandle,   hostClass As [Class]) As MethodHandle
				' Do not use this function to inject calls into system classes.
				If hostClass Is Nothing OrElse (hostClass.array OrElse hostClass.primitive OrElse hostClass.name.StartsWith("java.") OrElse hostClass.name.StartsWith("sun.")) Then Throw New InternalError ' does not happen, and should not anyway
				' For simplicity, convert mh to a varargs-like method.
				Dim vamh As MethodHandle = prepareForInvoker(mh)
				' Cache the result of makeInjectedInvoker once per argument class.
				Dim bccInvoker As MethodHandle = CV_makeInjectedInvoker.get(hostClass)
				Return restoreToType(bccInvoker.bindTo(vamh), mh, hostClass)
			End Function

			Private Shared Function makeInjectedInvoker(  hostClass As [Class]) As MethodHandle
				Dim bcc As  [Class] = UNSAFE.defineAnonymousClass(hostClass, T_BYTES, Nothing)
				If hostClass.classLoader IsNot bcc.classLoader Then Throw New InternalError(hostClass.name & " (CL)")
				Try
					If hostClass.protectionDomain IsNot bcc.protectionDomain Then Throw New InternalError(hostClass.name & " (PD)")
				Catch ex As SecurityException
					' Self-check was blocked by security manager.  This is OK.
					' In fact the whole try body could be turned into an assertion.
				End Try
				Try
					Dim init As MethodHandle = IMPL_LOOKUP.findStatic(bcc, "init", MethodType.methodType(GetType(void)))
					init.invokeExact() ' force initialization of the class
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
				Dim bccInvoker As MethodHandle
				Try
					Dim invokerMT As MethodType = MethodType.methodType(GetType(Object), GetType(MethodHandle), GetType(Object()))
					bccInvoker = IMPL_LOOKUP.findStatic(bcc, "invoke_V", invokerMT)
				Catch ex As ReflectiveOperationException
					Throw uncaughtException(ex)
				End Try
				' Test the invoker, to ensure that it really injects into the right place.
				Try
					Dim vamh As MethodHandle = prepareForInvoker(MH_checkCallerClass)
					Dim ok As Object = bccInvoker.invokeExact(vamh, New Object(){hostClass, bcc})
				Catch ex As Throwable
					Throw New InternalError(ex)
				End Try
				Return bccInvoker
			End Function
			Private Shared CV_makeInjectedInvoker As  [Class]Value(Of MethodHandle) = New ClassValueAnonymousInnerClassHelper(Of T)

			Private Class ClassValueAnonymousInnerClassHelper(Of T)
				Inherits ClassValue(Of T)

				Protected Friend Overrides Function computeValue(  hostClass As [Class]) As MethodHandle
					Return makeInjectedInvoker(hostClass)
				End Function
			End Class

			' Adapt mh so that it can be called directly from an injected invoker:
			Private Shared Function prepareForInvoker(  mh As MethodHandle) As MethodHandle
				mh = mh.asFixedArity()
				Dim mt As MethodType = mh.type()
				Dim arity As Integer = mt.parameterCount()
				Dim vamh As MethodHandle = mh.asType(mt.generic())
				vamh.internalForm().compileToBytecode() ' eliminate LFI stack frames
				vamh = vamh.asSpreader(GetType(Object()), arity)
				vamh.internalForm().compileToBytecode() ' eliminate LFI stack frames
				Return vamh
			End Function

			' Undo the adapter effect of prepareForInvoker:
			Private Shared Function restoreToType(  vamh As MethodHandle,   original As MethodHandle,   hostClass As [Class]) As MethodHandle
				Dim type As MethodType = original.type()
				Dim mh As MethodHandle = vamh.asCollector(GetType(Object()), type.parameterCount())
				Dim member As MemberName = original.internalMemberName()
				mh = mh.asType(type)
				mh = New WrappedMember(mh, type, member, original.invokeSpecial, hostClass)
				Return mh
			End Function

			Private Shared ReadOnly MH_checkCallerClass As MethodHandle

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private Shared Function checkCallerClass(  expected As [Class],   expected2 As [Class]) As Boolean
				' This method is called via MH_checkCallerClass and so it's
				' correct to ask for the immediate caller here.
				Dim actual As  [Class] = sun.reflect.Reflection.callerClass
				If actual IsNot expected AndAlso actual IsNot expected2 Then Throw New InternalError("found " & actual.name & ", expected " & expected.name +(If(expected Is expected2, "", ", or else " & expected2.name)))
				Return True
			End Function

			Private Shared ReadOnly T_BYTES As SByte()

			' The following class is used as a template for Unsafe.defineAnonymousClass:
			Private Class T
				Friend Shared Sub init() ' side effect: initializes this class
				End Sub
				Friend Shared Function invoke_V(  vamh As MethodHandle,   args As Object()) As Object
					Return vamh.invokeExact(args)
				End Function
			End Class
		End Class


		''' <summary>
		''' This subclass allows a wrapped method handle to be re-associated with an arbitrary member name. </summary>
		Private NotInheritable Class WrappedMember
			Inherits DelegatingMethodHandle

			Private ReadOnly target As MethodHandle
			Private ReadOnly member As MemberName
			Private ReadOnly callerClass As  [Class]
			Private ReadOnly isInvokeSpecial_Renamed As Boolean

			Private Sub New(  target As MethodHandle,   type As MethodType,   member As MemberName,   isInvokeSpecial As Boolean,   callerClass As [Class])
				MyBase.New(type, target)
				Me.target = target
				Me.member = member
				Me.callerClass = callerClass
				Me.isInvokeSpecial_Renamed = isInvokeSpecial
			End Sub

			Friend Overrides Function internalMemberName() As MemberName
				Return member
			End Function
			Friend Overrides Function internalCallerClass() As  [Class]
				Return callerClass
			End Function
			Friend  Overrides ReadOnly Property  invokeSpecial As Boolean
				Get
					Return isInvokeSpecial_Renamed
				End Get
			End Property
			Protected Friend  Overrides ReadOnly Property  target As MethodHandle
				Get
					Return target
				End Get
			End Property
			Public Overrides Function asTypeUncached(  newType As MethodType) As MethodHandle
				' This MH is an alias for target, except for the MemberName
				' Drop the MemberName if there is any conversion.
					asTypeCache = target.asType(newType)
					Return asTypeCache
			End Function
		End Class

		Friend Shared Function makeWrappedMember(  target As MethodHandle,   member As MemberName,   isInvokeSpecial As Boolean) As MethodHandle
			If member.Equals(target.internalMemberName()) AndAlso isInvokeSpecial = target.invokeSpecial Then Return target
			Return New WrappedMember(target, target.type(), member, isInvokeSpecial, Nothing)
		End Function

		''' <summary>
		''' Intrinsic IDs </summary>
		'non-public
		Friend Enum Intrinsic
			SELECT_ALTERNATIVE
			GUARD_WITH_CATCH
			NEW_ARRAY
			ARRAY_LOAD
			ARRAY_STORE
			IDENTITY
			ZERO
			NONE ' no intrinsic associated
		End Enum

		''' <summary>
		''' Mark arbitrary method handle as intrinsic.
		''' InvokerBytecodeGenerator uses this info to produce more efficient bytecode shape. 
		''' </summary>
		Private NotInheritable Class IntrinsicMethodHandle
			Inherits DelegatingMethodHandle

			Private ReadOnly target As MethodHandle
			Private ReadOnly intrinsicName_Renamed As Intrinsic

			Friend Sub New(  target As MethodHandle,   intrinsicName As Intrinsic)
				MyBase.New(target.type(), target)
				Me.target = target
				Me.intrinsicName_Renamed = intrinsicName
			End Sub

			Protected Friend  Overrides ReadOnly Property  target As MethodHandle
				Get
					Return target
				End Get
			End Property

			Friend Overrides Function intrinsicName() As Intrinsic
				Return intrinsicName_Renamed
			End Function

			Public Overrides Function asTypeUncached(  newType As MethodType) As MethodHandle
				' This MH is an alias for target, except for the intrinsic name
				' Drop the name if there is any conversion.
					asTypeCache = target.asType(newType)
					Return asTypeCache
			End Function

			Friend Overrides Function internalProperties() As String
				Return MyBase.internalProperties() & vbLf & "& Intrinsic=" & intrinsicName_Renamed
			End Function

			Public Overrides Function asCollector(  arrayType As [Class],   arrayLength As Integer) As MethodHandle
				If intrinsicName_Renamed = Intrinsic.IDENTITY Then
					Dim resultType As MethodType = type().asCollectorType(arrayType, arrayLength)
					Dim newArray As MethodHandle = MethodHandleImpl.varargsArray(arrayType, arrayLength)
					Return newArray.asType(resultType)
				End If
				Return MyBase.asCollector(arrayType, arrayLength)
			End Function
		End Class

		Friend Shared Function makeIntrinsic(  target As MethodHandle,   intrinsicName As Intrinsic) As MethodHandle
			If intrinsicName Is target.intrinsicName() Then Return target
			Return New IntrinsicMethodHandle(target, intrinsicName)
		End Function

		Friend Shared Function makeIntrinsic(  type As MethodType,   form As LambdaForm,   intrinsicName As Intrinsic) As MethodHandle
			Return New IntrinsicMethodHandle(SimpleMethodHandle.make(type, form), intrinsicName)
		End Function

		'/ Collection of multiple arguments.

		Private Shared Function findCollector(  name As String,   nargs As Integer,   rtype As [Class], ParamArray   ptypes As  [Class]()) As MethodHandle
			Dim type As MethodType = MethodType.genericMethodType(nargs).changeReturnType(rtype).insertParameterTypes(0, ptypes)
			Try
				Return IMPL_LOOKUP.findStatic(GetType(MethodHandleImpl), name, type)
			Catch ex As ReflectiveOperationException
				Return Nothing
			End Try
		End Function

		Private Shared ReadOnly NO_ARGS_ARRAY As Object() = {}
		Private Shared Function makeArray(ParamArray   args As Object()) As Object()
			Return args
		End Function
		Private Shared Function array() As Object()
			Return NO_ARGS_ARRAY
		End Function
		Private Shared Function array(  a0 As Object) As Object()
						Return makeArray(a0)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object) As Object()
						Return makeArray(a0, a1)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object) As Object()
						Return makeArray(a0, a1, a2)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object) As Object()
						Return makeArray(a0, a1, a2, a3)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4, a5)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4, a5, a6)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4, a5, a6, a7)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object,   a8 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4, a5, a6, a7, a8)
		End Function
		Private Shared Function array(  a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object,   a8 As Object,   a9 As Object) As Object()
						Return makeArray(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)
		End Function
		Private Shared Function makeArrays() As MethodHandle()
			Dim mhs As New List(Of MethodHandle)
			Do
				Dim mh As MethodHandle = findCollector("array", mhs.Count, GetType(Object()))
				If mh Is Nothing Then Exit Do
				mh = makeIntrinsic(mh, Intrinsic.NEW_ARRAY)
				mhs.Add(mh)
			Loop
			assert(mhs.Count = 11) ' current number of methods
			Return mhs.ToArray()
		End Function

		' filling versions of the above:
		' using Integer len instead of int len and no varargs to avoid bootstrapping problems
		Private Shared Function fillNewArray(  len As Integer?,   args As Object()) As Object() 'not ...
			Dim a As Object() = New Object(len - 1){}
			fillWithArguments(a, 0, args)
			Return a
		End Function
		Private Shared Function fillNewTypedArray(  example As Object(),   len As Integer?,   args As Object()) As Object() 'not ...
			Dim a As Object() = java.util.Arrays.copyOf(example, len)
			assert(a.GetType() IsNot GetType(Object()))
			fillWithArguments(a, 0, args)
			Return a
		End Function
		Private Shared Sub fillWithArguments(  a As Object(),   pos As Integer, ParamArray   args As Object())
			Array.Copy(args, 0, a, pos, args.Length)
		End Sub
		' using Integer pos instead of int pos to avoid bootstrapping problems
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object) As Object()
						fillWithArguments(a, pos, a0)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object) As Object()
						fillWithArguments(a, pos, a0, a1)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object,   a8 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7, a8)
						Return a
		End Function
		Private Shared Function fillArray(  pos As Integer?,   a As Object(),   a0 As Object,   a1 As Object,   a2 As Object,   a3 As Object,   a4 As Object,   a5 As Object,   a6 As Object,   a7 As Object,   a8 As Object,   a9 As Object) As Object()
						fillWithArguments(a, pos, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9)
						Return a
		End Function

		Private Const FILL_ARRAYS_COUNT As Integer = 11 ' current number of fillArray methods

		Private Shared Function makeFillArrays() As MethodHandle()
			Dim mhs As New List(Of MethodHandle)
			mhs.Add(Nothing) ' there is no empty fill; at least a0 is required
			Do
				Dim mh As MethodHandle = findCollector("fillArray", mhs.Count, GetType(Object()), GetType(Integer), GetType(Object()))
				If mh Is Nothing Then Exit Do
				mhs.Add(mh)
			Loop
			assert(mhs.Count = FILL_ARRAYS_COUNT)
			Return mhs.ToArray()
		End Function

		Private Shared Function copyAsPrimitiveArray(  w As sun.invoke.util.Wrapper, ParamArray   boxes As Object()) As Object
			Dim a As Object = w.makeArray(boxes.Length)
			w.copyArrayUnboxing(boxes, 0, a, 0, boxes.Length)
			Return a
		End Function

		''' <summary>
		''' Return a method handle that takes the indicated number of Object
		'''  arguments and returns an Object array of them, as if for varargs.
		''' </summary>
		Friend Shared Function varargsArray(  nargs As Integer) As MethodHandle
			Dim mh As MethodHandle = Lazy.ARRAYS(nargs)
			If mh IsNot Nothing Then Return mh
			mh = findCollector("array", nargs, GetType(Object()))
			If mh IsNot Nothing Then mh = makeIntrinsic(mh, Intrinsic.NEW_ARRAY)
			If mh IsNot Nothing Then
					Lazy.ARRAYS(nargs) = mh
					Return Lazy.ARRAYS(nargs)
			End If
			mh = buildVarargsArray(Lazy.MH_fillNewArray, Lazy.MH_arrayIdentity, nargs)
			assert(assertCorrectArity(mh, nargs))
			mh = makeIntrinsic(mh, Intrinsic.NEW_ARRAY)
				Lazy.ARRAYS(nargs) = mh
				Return Lazy.ARRAYS(nargs)
		End Function

		Private Shared Function assertCorrectArity(  mh As MethodHandle,   arity As Integer) As Boolean
			assert(mh.type().parameterCount() = arity) : "arity != " & arity & ": " & mh
			Return True
		End Function

		' Array identity function (used as Lazy.MH_arrayIdentity).
		Friend Shared Function identity(Of T)(  x As T()) As T()
			Return x
		End Function

		Private Shared Function buildVarargsArray(  newArray As MethodHandle,   finisher As MethodHandle,   nargs As Integer) As MethodHandle
			' Build up the result mh as a sequence of fills like this:
			'   finisher(fill(fill(newArrayWA(23,x1..x10),10,x11..x20),20,x21..x23))
			' The various fill(_,10*I,___*[J]) are reusable.
			Dim leftLen As Integer = System.Math.Min(nargs, LEFT_ARGS) ' absorb some arguments immediately
			Dim rightLen As Integer = nargs - leftLen
			Dim leftCollector As MethodHandle = newArray.bindTo(nargs)
			leftCollector = leftCollector.asCollector(GetType(Object()), leftLen)
			Dim mh As MethodHandle = finisher
			If rightLen > 0 Then
				Dim rightFiller As MethodHandle = fillToRight(LEFT_ARGS + rightLen)
				If mh Is Lazy.MH_arrayIdentity Then
					mh = rightFiller
				Else
					mh = MethodHandles.collectArguments(mh, 0, rightFiller)
				End If
			End If
			If mh Is Lazy.MH_arrayIdentity Then
				mh = leftCollector
			Else
				mh = MethodHandles.collectArguments(mh, 0, leftCollector)
			End If
			Return mh
		End Function

		Private Shared ReadOnly LEFT_ARGS As Integer = FILL_ARRAYS_COUNT - 1
		Private Shared ReadOnly FILL_ARRAY_TO_RIGHT As MethodHandle() = New MethodHandle(MAX_ARITY){}
		''' <summary>
		''' fill_array_to_right(N).invoke(a, argL..arg[N-1])
		'''  fills a[L]..a[N-1] with corresponding arguments,
		'''  and then returns a.  The value L is a global constant (LEFT_ARGS).
		''' </summary>
		Private Shared Function fillToRight(  nargs As Integer) As MethodHandle
			Dim filler As MethodHandle = FILL_ARRAY_TO_RIGHT(nargs)
			If filler IsNot Nothing Then Return filler
			filler = buildFiller(nargs)
			assert(assertCorrectArity(filler, nargs - LEFT_ARGS + 1))
				FILL_ARRAY_TO_RIGHT(nargs) = filler
				Return FILL_ARRAY_TO_RIGHT(nargs)
		End Function
		Private Shared Function buildFiller(  nargs As Integer) As MethodHandle
			If nargs <= LEFT_ARGS Then Return Lazy.MH_arrayIdentity ' no args to fill; return the array unchanged
			' we need room for both mh and a in mh.invoke(a, arg*[nargs])
			Dim CHUNK As Integer = LEFT_ARGS
			Dim rightLen As Integer = nargs Mod CHUNK
			Dim midLen As Integer = nargs - rightLen
			If rightLen = 0 Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				midLen = nargs - (rightLen = CHUNK)
				If FILL_ARRAY_TO_RIGHT(midLen) Is Nothing Then
					' build some precursors from left to right
					For j As Integer = LEFT_ARGS Mod CHUNK To midLen - 1 Step CHUNK
						If j > LEFT_ARGS Then fillToRight(j)
					Next j
				End If
			End If
			If midLen < LEFT_ARGS Then rightLen = nargs - (midLen = LEFT_ARGS)
			assert(rightLen > 0)
			Dim midFill As MethodHandle = fillToRight(midLen) ' recursive fill
			Dim rightFill As MethodHandle = Lazy.FILL_ARRAYS(rightLen).bindTo(midLen) ' [midLen..nargs-1]
			assert(midFill.type().parameterCount() = 1 + midLen - LEFT_ARGS)
			assert(rightFill.type().parameterCount() = 1 + rightLen)

			' Combine the two fills:
			'   right(mid(a, x10..x19), x20..x23)
			' The final product will look like this:
			'   right(mid(newArrayLeft(24, x0..x9), x10..x19), x20..x23)
			If midLen = LEFT_ARGS Then
				Return rightFill
			Else
				Return MethodHandles.collectArguments(rightFill, 0, midFill)
			End If
		End Function

		' Type-polymorphic version of varargs maker.
		Private Shared ReadOnly TYPED_COLLECTORS As  [Class]Value(Of MethodHandle()) = New ClassValueAnonymousInnerClassHelper(Of T)

		Private Class ClassValueAnonymousInnerClassHelper(Of T)
			Inherits ClassValue(Of T)

			Protected Friend Overrides Function computeValue(  type As [Class]) As MethodHandle()
				Return New MethodHandle(255){}
			End Function
		End Class

		Friend Const MAX_JVM_ARITY As Integer = 255 ' limit imposed by the JVM

		''' <summary>
		''' Return a method handle that takes the indicated number of
		'''  typed arguments and returns an array of them.
		'''  The type argument is the array type.
		''' </summary>
		Friend Shared Function varargsArray(  arrayType As [Class],   nargs As Integer) As MethodHandle
			Dim elemType As  [Class] = arrayType.componentType
			If elemType Is Nothing Then Throw New IllegalArgumentException("not an array: " & arrayType)
			' FIXME: Need more special casing and caching here.
			If nargs >= MAX_JVM_ARITY\2 - 1 Then
				Dim slots As Integer = nargs
				Dim MAX_ARRAY_SLOTS As Integer = MAX_JVM_ARITY - 1 ' 1 for receiver MH
				If slots <= MAX_ARRAY_SLOTS AndAlso elemType.primitive Then slots *= sun.invoke.util.Wrapper.forPrimitiveType(elemType).stackSlots()
				If slots > MAX_ARRAY_SLOTS Then Throw New IllegalArgumentException("too many arguments: " & arrayType.simpleName & ", length " & nargs)
			End If
			If elemType Is GetType(Object) Then Return varargsArray(nargs)
			' other cases:  primitive arrays, subtypes of Object[]
			Dim cache As MethodHandle() = TYPED_COLLECTORS.get(elemType)
			Dim mh As MethodHandle = If(nargs < cache.Length, cache(nargs), Nothing)
			If mh IsNot Nothing Then Return mh
			If nargs = 0 Then
				Dim example As Object = java.lang.reflect.Array.newInstance(arrayType.componentType, 0)
				mh = MethodHandles.constant(arrayType, example)
			ElseIf elemType.primitive Then
				Dim builder As MethodHandle = Lazy.MH_fillNewArray
				Dim producer As MethodHandle = buildArrayProducer(arrayType)
				mh = buildVarargsArray(builder, producer, nargs)
			Else
				Dim objArrayType As  [Class] = arrayType.asSubclass(GetType(Object()))
				Dim example As Object() = java.util.Arrays.copyOf(NO_ARGS_ARRAY, 0, objArrayType)
				Dim builder As MethodHandle = Lazy.MH_fillNewTypedArray.bindTo(example)
				Dim producer As MethodHandle = Lazy.MH_arrayIdentity ' must be weakly typed
				mh = buildVarargsArray(builder, producer, nargs)
			End If
			mh = mh.asType(MethodType.methodType(arrayType, java.util.Collections.nCopies(Of [Class])(nargs, elemType)))
			mh = makeIntrinsic(mh, Intrinsic.NEW_ARRAY)
			assert(assertCorrectArity(mh, nargs))
			If nargs < cache.Length Then cache(nargs) = mh
			Return mh
		End Function

		Private Shared Function buildArrayProducer(  arrayType As [Class]) As MethodHandle
			Dim elemType As  [Class] = arrayType.componentType
			assert(elemType.primitive)
			Return Lazy.MH_copyAsPrimitiveArray.bindTo(sun.invoke.util.Wrapper.forPrimitiveType(elemType))
		End Function

		'non-public
	 Friend Shared Sub assertSame(  mh1 As Object,   mh2 As Object)
			If mh1 IsNot mh2 Then
				Dim msg As String = String.Format("mh1 != mh2: mh1 = {0} (form: {1}); mh2 = {2} (form: {3})", mh1, CType(mh1, MethodHandle).form, mh2, CType(mh2, MethodHandle).form)
				Throw newInternalError(msg)
			End If
	 End Sub
	 End Class

End Namespace