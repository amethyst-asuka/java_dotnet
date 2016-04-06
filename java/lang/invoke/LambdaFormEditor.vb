Imports System
Imports System.Collections.Concurrent

'
' * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' Transforms on LFs.
	'''  A lambda-form editor can derive new LFs from its base LF.
	'''  The editor can cache derived LFs, which simplifies the reuse of their underlying bytecodes.
	'''  To support this caching, a LF has an optional pointer to its editor.
	''' </summary>
	Friend Class LambdaFormEditor
		Friend ReadOnly lambdaForm_Renamed As LambdaForm

		Private Sub New(  lambdaForm_Renamed As LambdaForm)
			Me.lambdaForm_Renamed = lambdaForm_Renamed
		End Sub

		' Factory method.
		Shared Function lambdaFormEditor(  lambdaForm_Renamed As LambdaForm) As LambdaFormEditor
			' TO DO:  Consider placing intern logic here, to cut down on duplication.
			' lambdaForm = findPreexistingEquivalent(lambdaForm)

			' Always use uncustomized version for editing.
			' It helps caching and customized LambdaForms reuse transformCache field to keep a link to uncustomized version.
			Return New LambdaFormEditor(lambdaForm_Renamed.uncustomize())
		End Function

		''' <summary>
		''' A description of a cached transform, possibly associated with the result of the transform.
		'''  The logical content is a sequence of byte values, starting with a Kind.ordinal value.
		'''  The sequence is unterminated, ending with an indefinite number of zero bytes.
		'''  Sequences that are simple (short enough and with small enough values) pack into a 64-bit java.lang.[Long].
		''' </summary>
		Private NotInheritable Class Transform
			Inherits SoftReference(Of LambdaForm)

			Friend ReadOnly packedBytes_Renamed As Long
			Friend ReadOnly fullBytes_Renamed As SByte()

			Private Enum Kind
				NO_KIND ' necessary because ordinal must be greater than zero
				BIND_ARG
				ADD_ARG
				DUP_ARG
				SPREAD_ARGS
				FILTER_ARG
				FILTER_RETURN
				FILTER_RETURN_TO_ZERO
				COLLECT_ARGS
				COLLECT_ARGS_TO_VOID
				COLLECT_ARGS_TO_ARRAY
				FOLD_ARGS
				FOLD_ARGS_TO_VOID
				PERMUTE_ARGS
				'maybe add more for guard with test, catch exception, pointwise type conversions
			End Enum

			Private Const STRESS_TEST As Boolean = False ' turn on to disable most packing
			Private Shared ReadOnly PACKED_BYTE_SIZE As Integer = (If(STRESS_TEST, 2, 4)), PACKED_BYTE_MASK As Integer = (1 << PACKED_BYTE_SIZE) - 1, PACKED_BYTE_MAX_LENGTH As Integer = (If(STRESS_TEST, 3, 64 \ PACKED_BYTE_SIZE))

			Private Shared Function packedBytes(  bytes As SByte()) As Long
				If bytes.Length > PACKED_BYTE_MAX_LENGTH Then Return 0
				Dim pb As Long = 0
				Dim bitset As Integer = 0
				For i As Integer = 0 To bytes.Length - 1
					Dim b As Integer = bytes(i) And &HFF
					bitset = bitset Or b
					pb = pb Or CLng(b) << (i * PACKED_BYTE_SIZE)
				Next i
				If Not inRange(bitset) Then Return 0
				Return pb
			End Function
			Private Shared Function packedBytes(  b0 As Integer,   b1 As Integer) As Long
				assert(inRange(b0 Or b1))
				Return ((b0 << 0*PACKED_BYTE_SIZE) Or (b1 << 1*PACKED_BYTE_SIZE))
			End Function
			Private Shared Function packedBytes(  b0 As Integer,   b1 As Integer,   b2 As Integer) As Long
				assert(inRange(b0 Or b1 Or b2))
				Return ((b0 << 0*PACKED_BYTE_SIZE) Or (b1 << 1*PACKED_BYTE_SIZE) Or (b2 << 2*PACKED_BYTE_SIZE))
			End Function
			Private Shared Function packedBytes(  b0 As Integer,   b1 As Integer,   b2 As Integer,   b3 As Integer) As Long
				assert(inRange(b0 Or b1 Or b2 Or b3))
				Return ((b0 << 0*PACKED_BYTE_SIZE) Or (b1 << 1*PACKED_BYTE_SIZE) Or (b2 << 2*PACKED_BYTE_SIZE) Or (b3 << 3*PACKED_BYTE_SIZE))
			End Function
			Private Shared Function inRange(  bitset As Integer) As Boolean
				assert((bitset And &HFF) = bitset) ' incoming values must fit in *unsigned* byte
				Return ((bitset And (Not PACKED_BYTE_MASK)) = 0)
			End Function
			Private Shared Function fullBytes(ParamArray   byteValues As Integer()) As SByte()
				Dim bytes As SByte() = New SByte(byteValues.Length - 1){}
				Dim i As Integer = 0
				For Each bv As Integer In byteValues
					bytes(i) = bval(bv)
					i += 1
				Next bv
				assert(packedBytes(bytes) = 0)
				Return bytes
			End Function

			Private Function byteAt(  i As Integer) As SByte
				Dim pb As Long = packedBytes_Renamed
				If pb = 0 Then
					If i >= fullBytes_Renamed.Length Then Return 0
					Return fullBytes_Renamed(i)
				End If
				assert(fullBytes_Renamed Is Nothing)
				If i > PACKED_BYTE_MAX_LENGTH Then Return 0
				Dim pos As Integer = (i * PACKED_BYTE_SIZE)
				Return CByte((CLng(CULng(pb) >> pos)) And PACKED_BYTE_MASK)
			End Function

			Friend Function kind() As Kind
				Return System.Enum.GetValues(GetType(Kind))(byteAt(0))
			End Function

			Private Sub New(  packedBytes As Long,   fullBytes As SByte(),   result As LambdaForm)
				MyBase.New(result)
				Me.packedBytes_Renamed = packedBytes
				Me.fullBytes_Renamed = fullBytes
			End Sub
			Private Sub New(  packedBytes As Long)
				Me.New(packedBytes, Nothing, Nothing)
				assert(packedBytes <> 0)
			End Sub
			Private Sub New(  fullBytes As SByte())
				Me.New(0, fullBytes, Nothing)
			End Sub

			Private Shared Function bval(  b As Integer) As SByte
				assert((b And &HFF) = b) ' incoming value must fit in *unsigned* byte
				Return CByte(b)
			End Function
			Private Shared Function bval(  k As Kind) As SByte
				Return bval(k.ordinal())
			End Function
			Shared Function [of](  k As Kind,   b1 As Integer) As Transform
				Dim b0 As SByte = bval(k)
				If inRange(b0 Or b1) Then
					Return New Transform(packedBytes(b0, b1))
				Else
					Return New Transform(fullBytes(b0, b1))
				End If
			End Function
			Shared Function [of](  k As Kind,   b1 As Integer,   b2 As Integer) As Transform
				Dim b0 As SByte = CByte(k.ordinal())
				If inRange(b0 Or b1 Or b2) Then
					Return New Transform(packedBytes(b0, b1, b2))
				Else
					Return New Transform(fullBytes(b0, b1, b2))
				End If
			End Function
			Shared Function [of](  k As Kind,   b1 As Integer,   b2 As Integer,   b3 As Integer) As Transform
				Dim b0 As SByte = CByte(k.ordinal())
				If inRange(b0 Or b1 Or b2 Or b3) Then
					Return New Transform(packedBytes(b0, b1, b2, b3))
				Else
					Return New Transform(fullBytes(b0, b1, b2, b3))
				End If
			End Function
			Private Shared ReadOnly NO_BYTES As SByte() = {}
			Shared Function [of](  k As Kind, ParamArray   b123 As Integer()) As Transform
				Return ofBothArrays(k, b123, NO_BYTES)
			End Function
			Shared Function [of](  k As Kind,   b1 As Integer,   b234 As SByte()) As Transform
				Return ofBothArrays(k, New Integer(){ b1 }, b234)
			End Function
			Shared Function [of](  k As Kind,   b1 As Integer,   b2 As Integer,   b345 As SByte()) As Transform
				Return ofBothArrays(k, New Integer(){ b1, b2 }, b345)
			End Function
			Private Shared Function ofBothArrays(  k As Kind,   b123 As Integer(),   b456 As SByte()) As Transform
				Dim fullBytes As SByte() = New SByte(1 + b123.Length + b456.Length - 1){}
				Dim i As Integer = 0
				fullBytes(i) = bval(k)
				i += 1
				For Each bv As Integer In b123
					fullBytes(i) = bval(bv)
					i += 1
				Next bv
				For Each bv As SByte In b456
					fullBytes(i) = bv
					i += 1
				Next bv
				Dim packedBytes As Long = packedBytes(fullBytes)
				If packedBytes <> 0 Then
					Return New Transform(packedBytes)
				Else
					Return New Transform(fullBytes)
				End If
			End Function

			Friend Function withResult(  result As LambdaForm) As Transform
				Return New Transform(Me.packedBytes_Renamed, Me.fullBytes_Renamed, result)
			End Function

			Public Overrides Function Equals(  obj As Object) As Boolean
				Return TypeOf obj Is Transform AndAlso Equals(CType(obj, Transform))
			End Function
			Public Overrides Function Equals(  that As Transform) As Boolean
				Return Me.packedBytes_Renamed = that.packedBytes_Renamed AndAlso java.util.Arrays.Equals(Me.fullBytes_Renamed, that.fullBytes_Renamed)
			End Function
			Public Overrides Function GetHashCode() As Integer
				If packedBytes_Renamed <> 0 Then
					assert(fullBytes_Renamed Is Nothing)
					Return java.lang.[Long].hashCode(packedBytes_Renamed)
				End If
				Return java.util.Arrays.hashCode(fullBytes_Renamed)
			End Function
			Public Overrides Function ToString() As String
				Dim buf As New StringBuilder
				Dim bits As Long = packedBytes_Renamed
				If bits <> 0 Then
					buf.append("(")
					Do While bits <> 0
						buf.append(bits And PACKED_BYTE_MASK)
						bits >>>= PACKED_BYTE_SIZE
						If bits <> 0 Then buf.append(",")
					Loop
					buf.append(")")
				End If
				If fullBytes_Renamed IsNot Nothing Then
					buf.append("unpacked")
					buf.append(java.util.Arrays.ToString(fullBytes_Renamed))
				End If
				Dim result As LambdaForm = get()
				If result IsNot Nothing Then
					buf.append(" result=")
					buf.append(result)
				End If
				Return buf.ToString()
			End Function
		End Class

		''' <summary>
		''' Find a previously cached transform equivalent to the given one, and return its result. </summary>
		Private Function getInCache(  key As Transform) As LambdaForm
			assert(key.get() Is Nothing)
			' The transformCache is one of null, Transform, Transform[], or ConcurrentHashMap.
			Dim c As Object = lambdaForm_Renamed.transformCache
			Dim k As Transform = Nothing
			If TypeOf c Is ConcurrentDictionary Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim m As ConcurrentDictionary(Of Transform, Transform) = CType(c, ConcurrentDictionary(Of Transform, Transform))
				k = m(key)
			ElseIf c Is Nothing Then
				Return Nothing
			ElseIf TypeOf c Is Transform Then
				' one-element cache avoids overhead of an array
				Dim t As Transform = CType(c, Transform)
				If t.Equals(key) Then k = t
			Else
				Dim ta As Transform() = CType(c, Transform())
				For i As Integer = 0 To ta.Length - 1
					Dim t As Transform = ta(i)
					If t Is Nothing Then Exit For
					If t.Equals(key) Then
						k = t
						Exit For
					End If
				Next i
			End If
			assert(k Is Nothing OrElse key.Equals(k))
			Return If(k IsNot Nothing, k.get(), Nothing)
		End Function

		''' <summary>
		''' Arbitrary but reasonable limits on Transform[] size for cache. </summary>
		Private Const MIN_CACHE_ARRAY_SIZE As Integer = 4, MAX_CACHE_ARRAY_SIZE As Integer = 16

		''' <summary>
		''' Cache a transform with its result, and return that result.
		'''  But if an equivalent transform has already been cached, return its result instead.
		''' </summary>
		Private Function putInCache(  key As Transform,   form As LambdaForm) As LambdaForm
			key = key.withResult(form)
			Dim pass As Integer = 0
			Do
				Dim c As Object = lambdaForm_Renamed.transformCache
				If TypeOf c Is ConcurrentDictionary Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim m As ConcurrentDictionary(Of Transform, Transform) = CType(c, ConcurrentDictionary(Of Transform, Transform))
					Dim k As Transform = m.GetOrAdd(key, key)
					If k Is Nothing Then Return form
					Dim result As LambdaForm = k.get()
					If result IsNot Nothing Then
						Return result
					Else
						If m.replace(key, k, key) Then
							Return form
						Else
							pass += 1
							Continue Do
						End If
					End If
				End If
				assert(pass = 0)
				SyncLock lambdaForm_Renamed
					c = lambdaForm_Renamed.transformCache
					If TypeOf c Is ConcurrentDictionary Then
						pass += 1
						Continue Do
					End If
					If c Is Nothing Then
						lambdaForm_Renamed.transformCache = key
						Return form
					End If
					Dim ta As Transform()
					If TypeOf c Is Transform Then
						Dim k As Transform = CType(c, Transform)
						If k.Equals(key) Then
							Dim result As LambdaForm = k.get()
							If result Is Nothing Then
								lambdaForm_Renamed.transformCache = key
								Return form
							Else
								Return result
							End If ' overwrite stale entry
						ElseIf k.get() Is Nothing Then
							lambdaForm_Renamed.transformCache = key
							Return form
						End If
						' expand one-element cache to small array
						ta = New Transform(MIN_CACHE_ARRAY_SIZE - 1){}
						ta(0) = k
						lambdaForm_Renamed.transformCache = ta
					Else
						' it is already expanded
						ta = CType(c, Transform())
					End If
					Dim len As Integer = ta.Length
					Dim stale As Integer = -1
					Dim i As Integer
					For i = 0 To len - 1
						Dim k As Transform = ta(i)
						If k Is Nothing Then Exit For
						If k.Equals(key) Then
							Dim result As LambdaForm = k.get()
							If result Is Nothing Then
								ta(i) = key
								Return form
							Else
								Return result
							End If
						ElseIf stale < 0 AndAlso k.get() Is Nothing Then
							stale = i ' remember 1st stale entry index
						End If
					Next i
					If i < len OrElse stale >= 0 Then
						' just fall through to cache update
					ElseIf len < MAX_CACHE_ARRAY_SIZE Then
						len = System.Math.Min(len * 2, MAX_CACHE_ARRAY_SIZE)
						ta = java.util.Arrays.copyOf(ta, len)
						lambdaForm_Renamed.transformCache = ta
					Else
						Dim m As New ConcurrentDictionary(Of Transform, Transform)(MAX_CACHE_ARRAY_SIZE * 2)
						For Each k As Transform In ta
							m(k) = k
						Next k
						lambdaForm_Renamed.transformCache = m
						' The second iteration will update for this query, concurrently.
						pass += 1
						Continue Do
					End If
					Dim idx As Integer = If(stale >= 0, stale, i)
					ta(idx) = key
					Return form
				End SyncLock
				pass += 1
			Loop
		End Function

		Private Function buffer() As LambdaFormBuffer
			Return New LambdaFormBuffer(lambdaForm_Renamed)
		End Function

		'/ Editing methods for method handles.  These need to have fast paths.

		Private Function oldSpeciesData() As BoundMethodHandle.SpeciesData
			Return BoundMethodHandle.speciesData(lambdaForm_Renamed)
		End Function
		Private Function newSpeciesData(  type As BasicType) As BoundMethodHandle.SpeciesData
			Return oldSpeciesData().extendWith(type)
		End Function

		Friend Overridable Function bindArgumentL(  mh As BoundMethodHandle,   pos As Integer,   value As Object) As BoundMethodHandle
			assert(mh.speciesData() Is oldSpeciesData())
			Dim bt As BasicType = L_TYPE
			Dim type2 As MethodType = bindArgumentType(mh, pos, bt)
			Dim form2 As LambdaForm = bindArgumentForm(1+pos)
			Return mh.copyWithExtendL(type2, form2, value)
		End Function
		Friend Overridable Function bindArgumentI(  mh As BoundMethodHandle,   pos As Integer,   value As Integer) As BoundMethodHandle
			assert(mh.speciesData() Is oldSpeciesData())
			Dim bt As BasicType = I_TYPE
			Dim type2 As MethodType = bindArgumentType(mh, pos, bt)
			Dim form2 As LambdaForm = bindArgumentForm(1+pos)
			Return mh.copyWithExtendI(type2, form2, value)
		End Function

		Friend Overridable Function bindArgumentJ(  mh As BoundMethodHandle,   pos As Integer,   value As Long) As BoundMethodHandle
			assert(mh.speciesData() Is oldSpeciesData())
			Dim bt As BasicType = J_TYPE
			Dim type2 As MethodType = bindArgumentType(mh, pos, bt)
			Dim form2 As LambdaForm = bindArgumentForm(1+pos)
			Return mh.copyWithExtendJ(type2, form2, value)
		End Function

		Friend Overridable Function bindArgumentF(  mh As BoundMethodHandle,   pos As Integer,   value As Single) As BoundMethodHandle
			assert(mh.speciesData() Is oldSpeciesData())
			Dim bt As BasicType = F_TYPE
			Dim type2 As MethodType = bindArgumentType(mh, pos, bt)
			Dim form2 As LambdaForm = bindArgumentForm(1+pos)
			Return mh.copyWithExtendF(type2, form2, value)
		End Function

		Friend Overridable Function bindArgumentD(  mh As BoundMethodHandle,   pos As Integer,   value As Double) As BoundMethodHandle
			assert(mh.speciesData() Is oldSpeciesData())
			Dim bt As BasicType = D_TYPE
			Dim type2 As MethodType = bindArgumentType(mh, pos, bt)
			Dim form2 As LambdaForm = bindArgumentForm(1+pos)
			Return mh.copyWithExtendD(type2, form2, value)
		End Function

		Private Function bindArgumentType(  mh As BoundMethodHandle,   pos As Integer,   bt As BasicType) As MethodType
			assert(mh.form Is lambdaForm_Renamed)
			assert(mh.form.names(1+pos).type_Renamed Is bt)
			assert(BasicType.basicType(mh.type().parameterType(pos)) Is bt)
			Return mh.type().dropParameterTypes(pos, pos+1)
		End Function

		'/ Editing methods for lambda forms.
		' Each editing method can (potentially) cache the edited LF so that it can be reused later.

		Friend Overridable Function bindArgumentForm(  pos As Integer) As LambdaForm
			Dim key As Transform = Transform.of(Transform.Kind.BIND_ARG, pos)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.parameterConstraint(0) Is newSpeciesData(lambdaForm_Renamed.parameterType(pos)))
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			Dim oldData As BoundMethodHandle.SpeciesData = oldSpeciesData()
			Dim newData As BoundMethodHandle.SpeciesData = newSpeciesData(lambdaForm_Renamed.parameterType(pos))
			Dim oldBaseAddress As Name = lambdaForm_Renamed.parameter(0) ' BMH holding the values
			Dim newBaseAddress As Name
			Dim getter As NamedFunction = newData.getterFunction(oldData.fieldCount())

			If pos <> 0 Then
				' The newly created LF will run with a different BMH.
				' Switch over any pre-existing BMH field references to the new BMH class.
				buf.replaceFunctions(oldData.getterFunctions(), newData.getterFunctions(), oldBaseAddress)
				newBaseAddress = oldBaseAddress.withConstraint(newData)
				buf.renameParameter(0, newBaseAddress)
				buf.replaceParameterByNewExpression(pos, New Name(getter, newBaseAddress))
			Else
				' cannot bind the MH arg itself, unless oldData is empty
				assert(oldData Is BoundMethodHandle.SpeciesData.EMPTY)
				newBaseAddress = (New Name(L_TYPE)).withConstraint(newData)
				buf.replaceParameterByNewExpression(0, New Name(getter, newBaseAddress))
				buf.insertParameter(0, newBaseAddress)
			End If

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function addArgumentForm(  pos As Integer,   type As BasicType) As LambdaForm
			Dim key As Transform = Transform.of(Transform.Kind.ADD_ARG, pos, type.ordinal())
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed+1)
				assert(form.parameterType(pos) Is type)
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			buf.insertParameter(pos, New Name(type))

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function dupArgumentForm(  srcPos As Integer,   dstPos As Integer) As LambdaForm
			Dim key As Transform = Transform.of(Transform.Kind.DUP_ARG, srcPos, dstPos)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed-1)
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			assert(lambdaForm_Renamed.parameter(srcPos).constraint Is Nothing)
			assert(lambdaForm_Renamed.parameter(dstPos).constraint Is Nothing)
			buf.replaceParameterByCopy(dstPos, srcPos)

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function spreadArgumentsForm(  pos As Integer,   arrayType As [Class],   arrayLength As Integer) As LambdaForm
			Dim elementType As  [Class] = arrayType.componentType
			Dim erasedArrayType As  [Class] = arrayType
			If Not elementType.primitive Then erasedArrayType = GetType(Object())
			Dim bt As BasicType = basicType(elementType)
			Dim elementTypeKey As Integer = bt.ordinal()
			If bt.basicTypeClass() IsNot elementType Then
				If elementType.primitive Then elementTypeKey = TYPE_LIMIT + sun.invoke.util.Wrapper.forPrimitiveType(elementType).ordinal()
			End If
			Dim key As Transform = Transform.of(Transform.Kind.SPREAD_ARGS, pos, elementTypeKey, arrayLength)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed - arrayLength + 1)
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			assert(pos <= MethodType.MAX_JVM_ARITY)
			assert(pos + arrayLength <= lambdaForm_Renamed.arity_Renamed)
			assert(pos > 0) ' cannot spread the MH arg itself

			Dim spreadParam As New Name(L_TYPE)
			Dim checkSpread As New Name(MethodHandleImpl.Lazy.NF_checkSpreadArgument, spreadParam, arrayLength)

			' insert the new expressions
			Dim exprPos As Integer = lambdaForm_Renamed.arity()
			buf.insertExpression(exprPos, checkSpread)
			exprPos += 1
			' adjust the arguments
			Dim aload As MethodHandle = MethodHandles.arrayElementGetter(erasedArrayType)
			For i As Integer = 0 To arrayLength - 1
				Dim loadArgument As New Name(aload, spreadParam, i)
				buf.insertExpression(exprPos + i, loadArgument)
				buf.replaceParameterByCopy(pos + i, exprPos + i)
			Next i
			buf.insertParameter(pos, spreadParam)

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function collectArgumentsForm(  pos As Integer,   collectorType As MethodType) As LambdaForm
			Dim collectorArity As Integer = collectorType.parameterCount()
			Dim dropResult As Boolean = (collectorType.returnType() Is GetType(void))
			If collectorArity = 1 AndAlso (Not dropResult) Then Return filterArgumentForm(pos, basicType(collectorType.parameterType(0)))
			Dim newTypes As BasicType() = BasicType.basicTypes(collectorType.parameterList())
			Dim kind As Transform.Kind = (If(dropResult, Transform.Kind.COLLECT_ARGS_TO_VOID, Transform.Kind.COLLECT_ARGS))
			If dropResult AndAlso collectorArity = 0 Then ' pure side effect pos = 1
			Dim key As Transform = Transform.of(kind, pos, collectorArity, BasicType.basicTypesOrd(newTypes))
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed - (If(dropResult, 0, 1)) + collectorArity)
				Return form
			End If
			form = makeArgumentCombinationForm(pos, collectorType, False, dropResult)
			Return putInCache(key, form)
		End Function

		Friend Overridable Function collectArgumentArrayForm(  pos As Integer,   arrayCollector As MethodHandle) As LambdaForm
			Dim collectorType As MethodType = arrayCollector.type()
			Dim collectorArity As Integer = collectorType.parameterCount()
			assert(arrayCollector.intrinsicName() Is Intrinsic.NEW_ARRAY)
			Dim arrayType As  [Class] = collectorType.returnType()
			Dim elementType As  [Class] = arrayType.componentType
			Dim argType As BasicType = basicType(elementType)
			Dim argTypeKey As Integer = argType.ordinal()
			If argType.basicTypeClass() IsNot elementType Then
				' return null if it requires more metadata (like String[].class)
				If Not elementType.primitive Then Return Nothing
				argTypeKey = TYPE_LIMIT + sun.invoke.util.Wrapper.forPrimitiveType(elementType).ordinal()
			End If
			assert(collectorType.parameterList().Equals(java.util.Collections.nCopies(collectorArity, elementType)))
			Dim kind As Transform.Kind = Transform.Kind.COLLECT_ARGS_TO_ARRAY
			Dim key As Transform = Transform.of(kind, pos, collectorArity, argTypeKey)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed - 1 + collectorArity)
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			assert(pos + 1 <= lambdaForm_Renamed.arity_Renamed)
			assert(pos > 0) ' cannot filter the MH arg itself

			Dim newParams As Name() = New Name(collectorArity - 1){}
			For i As Integer = 0 To collectorArity - 1
				newParams(i) = New Name(pos + i, argType)
			Next i
			Dim callCombiner As New Name(arrayCollector, CType(newParams, Object())) '...

			' insert the new expression
			Dim exprPos As Integer = lambdaForm_Renamed.arity()
			buf.insertExpression(exprPos, callCombiner)

			' insert new arguments
			Dim argPos As Integer = pos + 1 ' skip result parameter
			For Each newParam As Name In newParams
				buf.insertParameter(argPos, newParam)
				argPos += 1
			Next newParam
			assert(buf.LastIndexOf(callCombiner) = exprPos+newParams.Length)
			buf.replaceParameterByCopy(pos, exprPos+newParams.Length)

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function filterArgumentForm(  pos As Integer,   newType As BasicType) As LambdaForm
			Dim key As Transform = Transform.of(Transform.Kind.FILTER_ARG, pos, newType.ordinal())
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed)
				assert(form.parameterType(pos) Is newType)
				Return form
			End If

			Dim oldType As BasicType = lambdaForm_Renamed.parameterType(pos)
			Dim filterType As MethodType = MethodType.methodType(oldType.basicTypeClass(), newType.basicTypeClass())
			form = makeArgumentCombinationForm(pos, filterType, False, False)
			Return putInCache(key, form)
		End Function

		Private Function makeArgumentCombinationForm(  pos As Integer,   combinerType As MethodType,   keepArguments As Boolean,   dropResult As Boolean) As LambdaForm
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()
			Dim combinerArity As Integer = combinerType.parameterCount()
			Dim resultArity As Integer = (If(dropResult, 0, 1))

			assert(pos <= MethodType.MAX_JVM_ARITY)
			assert(pos + resultArity + (If(keepArguments, combinerArity, 0)) <= lambdaForm_Renamed.arity_Renamed)
			assert(pos > 0) ' cannot filter the MH arg itself
			assert(combinerType Is combinerType.basicType())
			assert(combinerType.returnType() IsNot GetType(void) OrElse dropResult)

			Dim oldData As BoundMethodHandle.SpeciesData = oldSpeciesData()
			Dim newData As BoundMethodHandle.SpeciesData = newSpeciesData(L_TYPE)

			' The newly created LF will run with a different BMH.
			' Switch over any pre-existing BMH field references to the new BMH class.
			Dim oldBaseAddress As Name = lambdaForm_Renamed.parameter(0) ' BMH holding the values
			buf.replaceFunctions(oldData.getterFunctions(), newData.getterFunctions(), oldBaseAddress)
			Dim newBaseAddress As Name = oldBaseAddress.withConstraint(newData)
			buf.renameParameter(0, newBaseAddress)

			Dim getCombiner As New Name(newData.getterFunction(oldData.fieldCount()), newBaseAddress)
			Dim combinerArgs As Object() = New Object(1 + combinerArity - 1){}
			combinerArgs(0) = getCombiner
			Dim newParams As Name()
			If keepArguments Then
				newParams = New Name(){}
				Array.Copy(lambdaForm_Renamed.names, pos + resultArity, combinerArgs, 1, combinerArity)
			Else
				newParams = New Name(combinerArity - 1){}
				Dim newTypes As BasicType() = basicTypes(combinerType.parameterList())
				For i As Integer = 0 To newTypes.Length - 1
					newParams(i) = New Name(pos + i, newTypes(i))
				Next i
				Array.Copy(newParams, 0, combinerArgs, 1, combinerArity)
			End If
			Dim callCombiner As New Name(combinerType, combinerArgs)

			' insert the two new expressions
			Dim exprPos As Integer = lambdaForm_Renamed.arity()
			buf.insertExpression(exprPos+0, getCombiner)
			buf.insertExpression(exprPos+1, callCombiner)

			' insert new arguments, if needed
			Dim argPos As Integer = pos + resultArity ' skip result parameter
			For Each newParam As Name In newParams
				buf.insertParameter(argPos, newParam)
				argPos += 1
			Next newParam
			assert(buf.LastIndexOf(callCombiner) = exprPos+1+newParams.Length)
			If Not dropResult Then buf.replaceParameterByCopy(pos, exprPos+1+newParams.Length)

			Return buf.endEdit()
		End Function

		Friend Overridable Function filterReturnForm(  newType As BasicType,   constantZero As Boolean) As LambdaForm
			Dim kind As Transform.Kind = (If(constantZero, Transform.Kind.FILTER_RETURN_TO_ZERO, Transform.Kind.FILTER_RETURN))
			Dim key As Transform = Transform.of(kind, newType.ordinal())
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed)
				assert(form.returnType() Is newType)
				Return form
			End If
			Dim buf As LambdaFormBuffer = buffer()
			buf.startEdit()

			Dim insPos As Integer = lambdaForm_Renamed.names.Length
			Dim callFilter As Name
			If constantZero Then
				' Synthesize a constant zero value for the given type.
				If newType Is V_TYPE Then
					callFilter = Nothing
				Else
					callFilter = New Name(constantZero(newType))
				End If
			Else
				Dim oldData As BoundMethodHandle.SpeciesData = oldSpeciesData()
				Dim newData As BoundMethodHandle.SpeciesData = newSpeciesData(L_TYPE)

				' The newly created LF will run with a different BMH.
				' Switch over any pre-existing BMH field references to the new BMH class.
				Dim oldBaseAddress As Name = lambdaForm_Renamed.parameter(0) ' BMH holding the values
				buf.replaceFunctions(oldData.getterFunctions(), newData.getterFunctions(), oldBaseAddress)
				Dim newBaseAddress As Name = oldBaseAddress.withConstraint(newData)
				buf.renameParameter(0, newBaseAddress)

				Dim getFilter As New Name(newData.getterFunction(oldData.fieldCount()), newBaseAddress)
				buf.insertExpression(insPos, getFilter)
				insPos += 1
				Dim oldType As BasicType = lambdaForm_Renamed.returnType()
				If oldType Is V_TYPE Then
					Dim filterType As MethodType = MethodType.methodType(newType.basicTypeClass())
					callFilter = New Name(filterType, getFilter)
				Else
					Dim filterType As MethodType = MethodType.methodType(newType.basicTypeClass(), oldType.basicTypeClass())
					callFilter = New Name(filterType, getFilter, lambdaForm_Renamed.names(lambdaForm_Renamed.result))
				End If
			End If

			If callFilter IsNot Nothing Then
				buf.insertExpression(insPos, callFilter)
				insPos += 1
			End If
			buf.result = callFilter

			form = buf.endEdit()
			Return putInCache(key, form)
		End Function

		Friend Overridable Function foldArgumentsForm(  foldPos As Integer,   dropResult As Boolean,   combinerType As MethodType) As LambdaForm
			Dim combinerArity As Integer = combinerType.parameterCount()
			Dim kind As Transform.Kind = (If(dropResult, Transform.Kind.FOLD_ARGS_TO_VOID, Transform.Kind.FOLD_ARGS))
			Dim key As Transform = Transform.of(kind, foldPos, combinerArity)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = lambdaForm_Renamed.arity_Renamed - (If(kind Is Transform.Kind.FOLD_ARGS, 1, 0)))
				Return form
			End If
			form = makeArgumentCombinationForm(foldPos, combinerType, True, dropResult)
			Return putInCache(key, form)
		End Function

		Friend Overridable Function permuteArgumentsForm(  skip As Integer,   reorder As Integer()) As LambdaForm
			assert(skip = 1) ' skip only the leading MH argument, names[0]
			Dim length As Integer = lambdaForm_Renamed.names.Length
			Dim outArgs As Integer = reorder.Length
			Dim inTypes As Integer = 0
			Dim nullPerm As Boolean = True
			For i As Integer = 0 To reorder.Length - 1
				Dim inArg As Integer = reorder(i)
				If inArg <> i Then nullPerm = False
				inTypes = System.Math.Max(inTypes, inArg+1)
			Next i
			assert(skip + reorder.Length = lambdaForm_Renamed.arity_Renamed)
			If nullPerm Then ' do not bother to cache Return lambdaForm_Renamed
			Dim key As Transform = Transform.of(Transform.Kind.PERMUTE_ARGS, reorder)
			Dim form As LambdaForm = getInCache(key)
			If form IsNot Nothing Then
				assert(form.arity_Renamed = skip+inTypes) : form
				Return form
			End If

			Dim types As BasicType() = New BasicType(inTypes - 1){}
			For i As Integer = 0 To outArgs - 1
				Dim inArg As Integer = reorder(i)
				types(inArg) = lambdaForm_Renamed.names(skip + i).type_Renamed
			Next i
			assert(skip + outArgs = lambdaForm_Renamed.arity_Renamed)
			assert(permutedTypesMatch(reorder, types, lambdaForm_Renamed.names, skip))
			Dim pos As Integer = 0
			Do While pos < outArgs AndAlso reorder(pos) = pos
				pos += 1
			Loop
			Dim names2 As Name() = New Name(length - outArgs + inTypes - 1){}
			Array.Copy(lambdaForm_Renamed.names, 0, names2, 0, skip + pos)
			Dim bodyLength As Integer = length - lambdaForm_Renamed.arity_Renamed
			Array.Copy(lambdaForm_Renamed.names, skip + outArgs, names2, skip + inTypes, bodyLength)
			Dim arity2 As Integer = names2.Length - bodyLength
			Dim result2 As Integer = lambdaForm_Renamed.result
			If result2 >= 0 Then
				If result2 < skip + outArgs Then
					result2 = reorder(result2 - skip)
				Else
					result2 = result2 - outArgs + inTypes
				End If
			End If
			For j As Integer = pos To outArgs - 1
				Dim n As Name = lambdaForm_Renamed.names(skip + j)
				Dim i As Integer = reorder(j)
				Dim n2 As Name = names2(skip + i)
				If n2 Is Nothing Then
						n2 = New Name(types(i))
						names2(skip + i) = n2
				Else
					assert(n2.type Is types(i))
				End If
				For k As Integer = arity2 To names2.Length - 1
					names2(k) = names2(k).replaceName(n, n2)
				Next k
			Next j
			For i As Integer = skip + pos To arity2 - 1
				If names2(i) Is Nothing Then names2(i) = argument(i, types(i - skip))
			Next i
			For j As Integer = lambdaForm_Renamed.arity_Renamed To lambdaForm_Renamed.names.Length - 1
				Dim i As Integer = j - lambdaForm_Renamed.arity_Renamed + arity2
				Dim n As Name = lambdaForm_Renamed.names(j)
				Dim n2 As Name = names2(i)
				If n IsNot n2 Then
					For k As Integer = i + 1 To names2.Length - 1
						names2(k) = names2(k).replaceName(n, n2)
					Next k
				End If
			Next j

			form = New LambdaForm(lambdaForm_Renamed.debugName, arity2, names2, result2)
			Return putInCache(key, form)
		End Function

		Friend Shared Function permutedTypesMatch(  reorder As Integer(),   types As BasicType(),   names As Name(),   skip As Integer) As Boolean
			For i As Integer = 0 To reorder.Length - 1
				assert(names(skip + i).param)
				assert(names(skip + i).type Is types(reorder(i)))
			Next i
			Return True
		End Function
	End Class

End Namespace