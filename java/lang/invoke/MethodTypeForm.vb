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
	''' Shared information for a group of method types, which differ
	''' only by reference types, and therefore share a common erasure
	''' and wrapping.
	''' <p>
	''' For an empirical discussion of the structure of method types,
	''' see <a href="http://groups.google.com/group/jvm-languages/browse_thread/thread/ac9308ae74da9b7e/">
	''' the thread "Avoiding Boxing" on jvm-languages</a>.
	''' There are approximately 2000 distinct erased method types in the JDK.
	''' There are a little over 10 times that number of unerased types.
	''' No more than half of these are likely to be loaded at once.
	''' @author John Rose
	''' </summary>
	Friend NotInheritable Class MethodTypeForm
		Friend ReadOnly argToSlotTable, slotToArgTable As Integer()
		Friend ReadOnly argCounts As Long ' packed slot & value counts
		Friend ReadOnly primCounts As Long ' packed prim & double counts
		Friend ReadOnly erasedType_Renamed As MethodType ' the canonical erasure
		Friend ReadOnly basicType_Renamed As MethodType ' the canonical erasure, with primitives simplified

		' Cached adapter information:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend ReadOnly methodHandles_Renamed As SoftReference(Of MethodHandle)()
		' Indexes into methodHandles:
		Friend Const MH_BASIC_INV As Integer = 0, MH_NF_INV As Integer = 1, MH_UNINIT_CS As Integer = 2, MH_LIMIT As Integer = 3 ' uninitialized call site -  cached helper for LF.NamedFunction -  cached instance of MH.invokeBasic

		' Cached lambda form information, for basic types only:
		Friend ReadOnly SoftReference As Stable
		' Indexes into lambdaForms:
		Friend Const LF_INVVIRTUAL As Integer = 0, LF_INVSTATIC As Integer = 1, LF_INVSPECIAL As Integer = 2, LF_NEWINVSPECIAL As Integer = 3, LF_INVINTERFACE As Integer = 4, LF_INVSTATIC_INIT As Integer = 5, LF_INTERPRET As Integer = 6, LF_REBIND As Integer = 7, LF_DELEGATE As Integer = 8, LF_DELEGATE_BLOCK_INLINING As Integer = 9, LF_EX_LINKER As Integer = 10, LF_EX_INVOKER As Integer = 11, LF_GEN_LINKER As Integer = 12, LF_GEN_INVOKER As Integer = 13, LF_CS_LINKER As Integer = 14, LF_MH_LINKER As Integer = 15, LF_GWC As Integer = 16, LF_GWT As Integer = 17, LF_LIMIT As Integer = 18 ' guardWithTest -  guardWithCatch (catchException) -  linkToCallSite_MH -  linkToCallSite_CS -  generic MHs.invoke -  generic invoke_MT (for invokehandle) -  MHs.invokeExact -  invokeExact_MT (for invokehandle) -  Counting DelegatingMethodHandle w/ @DontInline -  DelegatingMethodHandle -  BoundMethodHandle -  LF interpreter -  DMH invokeStatic with <clinit> barrier -  DMH invokeVirtual

		''' <summary>
		''' Return the type corresponding uniquely (1-1) to this MT-form.
		'''  It might have any primitive returns or arguments, but will have no references except Object.
		''' </summary>
		Public Function erasedType() As MethodType
			Return erasedType_Renamed
		End Function

		''' <summary>
		''' Return the basic type derived from the erased type of this MT-form.
		'''  A basic type is erased (all references Object) and also has all primitive
		'''  types (except int, long, float, double, void) normalized to int.
		'''  Such basic types correspond to low-level JVM calling sequences.
		''' </summary>
		Public Function basicType() As MethodType
			Return basicType_Renamed
		End Function

		Private Function assertIsBasicType() As Boolean
			' primitives must be flattened also
			assert(erasedType_Renamed Is basicType_Renamed) : "erasedType: " & erasedType_Renamed & " != basicType: " & basicType_Renamed
			Return True
		End Function

		Public Function cachedMethodHandle(ByVal which As Integer) As MethodHandle
			assert(assertIsBasicType())
			Dim entry As SoftReference(Of MethodHandle) = methodHandles_Renamed(which)
			Return If(entry IsNot Nothing, entry.get(), Nothing)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Function setCachedMethodHandle(ByVal which As Integer, ByVal mh As MethodHandle) As MethodHandle
			' Simulate a CAS, to avoid racy duplication of results.
			Dim entry As SoftReference(Of MethodHandle) = methodHandles_Renamed(which)
			If entry IsNot Nothing Then
				Dim prev As MethodHandle = entry.get()
				If prev IsNot Nothing Then Return prev
			End If
			methodHandles_Renamed(which) = New SoftReference(Of )(mh)
			Return mh
		End Function

		Public Function cachedLambdaForm(ByVal which As Integer) As LambdaForm
			assert(assertIsBasicType())
			Dim entry As SoftReference(Of LambdaForm) = lambdaForms(which)
			Return If(entry IsNot Nothing, entry.get(), Nothing)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Function setCachedLambdaForm(ByVal which As Integer, ByVal form As LambdaForm) As LambdaForm
			' Simulate a CAS, to avoid racy duplication of results.
			Dim entry As SoftReference(Of LambdaForm) = lambdaForms(which)
			If entry IsNot Nothing Then
				Dim prev As LambdaForm = entry.get()
				If prev IsNot Nothing Then Return prev
			End If
			lambdaForms(which) = New SoftReference(Of )(form)
			Return form
		End Function

		''' <summary>
		''' Build an MTF for a given type, which must have all references erased to Object.
		''' This MTF will stand for that type and all un-erased variations.
		''' Eagerly compute some basic properties of the type, common to all variations.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Sub New(ByVal erasedType As MethodType)
			Me.erasedType_Renamed = erasedType

			Dim ptypes As Class() = erasedType.ptypes()
			Dim ptypeCount As Integer = ptypes.Length
			Dim pslotCount As Integer = ptypeCount ' temp. estimate
			Dim rtypeCount As Integer = 1 ' temp. estimate
			Dim rslotCount As Integer = 1 ' temp. estimate

			Dim argToSlotTab As Integer() = Nothing, slotToArgTab As Integer() = Nothing

			' Walk the argument types, looking for primitives.
			Dim pac As Integer = 0, lac As Integer = 0, prc As Integer = 0, lrc As Integer = 0
			Dim epts As Class() = ptypes
			Dim bpts As Class() = epts
			For i As Integer = 0 To epts.Length - 1
				Dim pt As Class = epts(i)
				If pt IsNot GetType(Object) Then
					pac += 1
					Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(pt)
					If w.doubleWord Then lac += 1
					If w.subwordOrInt AndAlso pt IsNot GetType(Integer) Then
						If bpts = epts Then bpts = bpts.clone()
						bpts(i) = GetType(Integer)
					End If
				End If
			Next i
			pslotCount += lac ' #slots = #args + #longs
			Dim rt As Class = erasedType.returnType()
			Dim bt As Class = rt
			If rt IsNot GetType(Object) Then
				prc += 1 ' even void.class counts as a prim here
				Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(rt)
				If w.doubleWord Then lrc += 1
				If w.subwordOrInt AndAlso rt IsNot GetType(Integer) Then bt = GetType(Integer)
				' adjust #slots, #args
				If rt Is GetType(void) Then
						rslotCount = 0
						rtypeCount = rslotCount
				Else
					rslotCount += lrc
				End If
			End If
			If epts = bpts AndAlso bt Is rt Then
				Me.basicType_Renamed = erasedType
			Else
				Me.basicType_Renamed = MethodType.makeImpl(bt, bpts, True)
				' fill in rest of data from the basic type:
				Dim that As MethodTypeForm = Me.basicType_Renamed.form()
				assert(Me IsNot that)
				Me.primCounts = that.primCounts
				Me.argCounts = that.argCounts
				Me.argToSlotTable = that.argToSlotTable
				Me.slotToArgTable = that.slotToArgTable
				Me.methodHandles_Renamed = Nothing
				Me.lambdaForms = Nothing
				Return
			End If
			If lac <> 0 Then
				Dim slot As Integer = ptypeCount + lac
				slotToArgTab = New Integer(slot){}
				argToSlotTab = New Integer(1+ptypeCount - 1){}
				argToSlotTab(0) = slot ' argument "-1" is past end of slots
				For i As Integer = 0 To epts.Length - 1
					Dim pt As Class = epts(i)
					Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forBasicType(pt)
					If w.doubleWord Then slot -= 1
					slot -= 1
					slotToArgTab(slot) = i+1 ' "+1" see argSlotToParameter note
					argToSlotTab(1+i) = slot
				Next i
				assert(slot = 0) ' filled the table
			ElseIf pac <> 0 Then
				' have primitives but no long primitives; share slot counts with generic
				assert(ptypeCount = pslotCount)
				Dim that As MethodTypeForm = MethodType.genericMethodType(ptypeCount).form()
				assert(Me IsNot that)
				slotToArgTab = that.slotToArgTable
				argToSlotTab = that.argToSlotTable
			Else
				Dim slot As Integer = ptypeCount ' first arg is deepest in stack
				slotToArgTab = New Integer(slot){}
				argToSlotTab = New Integer(1+ptypeCount - 1){}
				argToSlotTab(0) = slot ' argument "-1" is past end of slots
				For i As Integer = 0 To ptypeCount - 1
					slot -= 1
					slotToArgTab(slot) = i+1 ' "+1" see argSlotToParameter note
					argToSlotTab(1+i) = slot
				Next i
			End If
			Me.primCounts = pack(lrc, prc, lac, pac)
			Me.argCounts = pack(rslotCount, rtypeCount, pslotCount, ptypeCount)
			Me.argToSlotTable = argToSlotTab
			Me.slotToArgTable = slotToArgTab

			If pslotCount >= 256 Then Throw newIllegalArgumentException("too many arguments")

			' Initialize caches, but only for basic types
			assert(basicType_Renamed Is erasedType)
			Me.lambdaForms = New SoftReference(LF_LIMIT - 1){}
			Me.methodHandles_Renamed = New SoftReference(MH_LIMIT - 1){}
		End Sub

		Private Shared Function pack(ByVal a As Integer, ByVal b As Integer, ByVal c As Integer, ByVal d As Integer) As Long
			assert(((a Or b Or c Or d) And (Not &HFFFF)) = 0)
			Dim hw As Long = ((a << 16) Or b), lw As Long = ((c << 16) Or d)
			Return (hw << 32) Or lw
		End Function
		Private Shared Function unpack(ByVal packed As Long, ByVal word As Integer) As Char ' word==0 => return a, ==3 => return d
			assert(word <= 3)
			Return CChar(packed >> ((3-word) * 16))
		End Function

		Public Function parameterCount() As Integer ' # outgoing values
			Return unpack(argCounts, 3)
		End Function
		Public Function parameterSlotCount() As Integer ' # outgoing interpreter slots
			Return unpack(argCounts, 2)
		End Function
		Public Function returnCount() As Integer ' = 0 (V), or 1
			Return unpack(argCounts, 1)
		End Function
		Public Function returnSlotCount() As Integer ' = 0 (V), 2 (J/D), or 1
			Return unpack(argCounts, 0)
		End Function
		Public Function primitiveParameterCount() As Integer
			Return unpack(primCounts, 3)
		End Function
		Public Function longPrimitiveParameterCount() As Integer
			Return unpack(primCounts, 2)
		End Function
		Public Function primitiveReturnCount() As Integer ' = 0 (obj), or 1
			Return unpack(primCounts, 1)
		End Function
		Public Function longPrimitiveReturnCount() As Integer ' = 1 (J/D), or 0
			Return unpack(primCounts, 0)
		End Function
		Public Function hasPrimitives() As Boolean
			Return primCounts <> 0
		End Function
		Public Function hasNonVoidPrimitives() As Boolean
			If primCounts = 0 Then Return False
			If primitiveParameterCount() <> 0 Then Return True
			Return (primitiveReturnCount() <> 0 AndAlso returnCount() <> 0)
		End Function
		Public Function hasLongPrimitives() As Boolean
			Return (longPrimitiveParameterCount() Or longPrimitiveReturnCount()) <> 0
		End Function
		Public Function parameterToArgSlot(ByVal i As Integer) As Integer
			Return argToSlotTable(1+i)
		End Function
		Public Function argSlotToParameter(ByVal argSlot As Integer) As Integer
			' Note:  Empty slots are represented by zero in this table.
			' Valid arguments slots contain incremented entries, so as to be non-zero.
			' We return -1 the caller to mean an empty slot.
			Return slotToArgTable(argSlot) - 1
		End Function

		Shared Function findForm(ByVal mt As MethodType) As MethodTypeForm
			Dim erased As MethodType = canonicalize(mt, [ERASE], [ERASE])
			If erased Is Nothing Then
				' It is already erased.  Make a new MethodTypeForm.
				Return New MethodTypeForm(mt)
			Else
				' Share the MethodTypeForm with the erased version.
				Return erased.form()
			End If
		End Function

		''' <summary>
		''' Codes for <seealso cref="#canonicalize(java.lang.Class, int)"/>.
		''' ERASE means change every reference to {@code Object}.
		''' WRAP means convert primitives (including {@code void} to their
		''' corresponding wrapper types.  UNWRAP means the reverse of WRAP.
		''' INTS means convert all non-void primitive types to int or long,
		''' according to size.  LONGS means convert all non-void primitives
		''' to long, regardless of size.  RAW_RETURN means convert a type
		''' (assumed to be a return type) to int if it is smaller than an int,
		''' or if it is void.
		''' </summary>
		Public Const NO_CHANGE As Integer = 0, [ERASE] As Integer = 1, WRAP As Integer = 2, UNWRAP As Integer = 3, INTS As Integer = 4, LONGS As Integer = 5, RAW_RETURN As Integer = 6

		''' <summary>
		''' Canonicalize the types in the given method type.
		''' If any types change, intern the new type, and return it.
		''' Otherwise return null.
		''' </summary>
		Public Shared Function canonicalize(ByVal mt As MethodType, ByVal howRet As Integer, ByVal howArgs As Integer) As MethodType
			Dim ptypes As Class() = mt.ptypes()
			Dim ptc As Class() = MethodTypeForm.canonicalizeAll(ptypes, howArgs)
			Dim rtype As Class = mt.returnType()
			Dim rtc As Class = MethodTypeForm.canonicalize(rtype, howRet)
			If ptc Is Nothing AndAlso rtc Is Nothing Then Return Nothing
			' Find the erased version of the method type:
			If rtc Is Nothing Then rtc = rtype
			If ptc Is Nothing Then ptc = ptypes
			Return MethodType.makeImpl(rtc, ptc, True)
		End Function

		''' <summary>
		''' Canonicalize the given return or param type.
		'''  Return null if the type is already canonicalized.
		''' </summary>
		Friend Shared Function canonicalize(ByVal t As Class, ByVal how As Integer) As Class
			Dim ct As Class
			If t Is GetType(Object) Then
				' no change, ever
			ElseIf Not t.primitive Then
				Select Case how
					Case UNWRAP
						ct = sun.invoke.util.Wrapper.asPrimitiveType(t)
						If ct IsNot t Then Return ct
					Case RAW_RETURN, [ERASE]
						Return GetType(Object)
				End Select
			ElseIf t Is GetType(void) Then
				' no change, usually
				Select Case how
					Case RAW_RETURN
						Return GetType(Integer)
					Case WRAP
						Return GetType(Void)
				End Select
			Else
				' non-void primitive
				Select Case how
					Case WRAP
						Return sun.invoke.util.Wrapper.asWrapperType(t)
					Case INTS
						If t Is GetType(Integer) OrElse t Is GetType(Long) Then Return Nothing ' no change
						If t Is GetType(Double) Then Return GetType(Long)
						Return GetType(Integer)
					Case LONGS
						If t Is GetType(Long) Then Return Nothing ' no change
						Return GetType(Long)
					Case RAW_RETURN
						If t Is GetType(Integer) OrElse t Is GetType(Long) OrElse t Is GetType(Single) OrElse t Is GetType(Double) Then Return Nothing ' no change
						' everything else returns as an int
						Return GetType(Integer)
				End Select
			End If
			' no change; return null to signify
			Return Nothing
		End Function

		''' <summary>
		''' Canonicalize each param type in the given array.
		'''  Return null if all types are already canonicalized.
		''' </summary>
		Friend Shared Function canonicalizeAll(ByVal ts As Class(), ByVal how As Integer) As Class()
			Dim cs As Class() = Nothing
			Dim imax As Integer = ts.Length
			Dim i As Integer = 0
			Do While i < imax
				Dim c As Class = canonicalize(ts(i), how)
				If c Is GetType(void) Then c = Nothing ' a Void parameter was unwrapped to void; ignore
				If c IsNot Nothing Then
					If cs Is Nothing Then cs = ts.clone()
					cs(i) = c
				End If
				i += 1
			Loop
			Return cs
		End Function

		Public Overrides Function ToString() As String
			Return "Form" & erasedType_Renamed
		End Function
	End Class

End Namespace