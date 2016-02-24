Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
	''' A method type represents the arguments and return type accepted and
	''' returned by a method handle, or the arguments and return type passed
	''' and expected  by a method handle caller.  Method types must be properly
	''' matched between a method handle and all its callers,
	''' and the JVM's operations enforce this matching at, specifically
	''' during calls to <seealso cref="MethodHandle#invokeExact MethodHandle.invokeExact"/>
	''' and <seealso cref="MethodHandle#invoke MethodHandle.invoke"/>, and during execution
	''' of {@code invokedynamic} instructions.
	''' <p>
	''' The structure is a return type accompanied by any number of parameter types.
	''' The types (primitive, {@code void}, and reference) are represented by <seealso cref="Class"/> objects.
	''' (For ease of exposition, we treat {@code void} as if it were a type.
	''' In fact, it denotes the absence of a return type.)
	''' <p>
	''' All instances of {@code MethodType} are immutable.
	''' Two instances are completely interchangeable if they compare equal.
	''' Equality depends on pairwise correspondence of the return and parameter types and on nothing else.
	''' <p>
	''' This type can be created only by factory methods.
	''' All factory methods may cache values, though caching is not guaranteed.
	''' Some factory methods are static, while others are virtual methods which
	''' modify precursor method types, e.g., by changing a selected parameter.
	''' <p>
	''' Factory methods which operate on groups of parameter types
	''' are systematically presented in two versions, so that both Java arrays and
	''' Java lists can be used to work with groups of parameter types.
	''' The query methods {@code parameterArray} and {@code parameterList}
	''' also provide a choice between arrays and lists.
	''' <p>
	''' {@code MethodType} objects are sometimes derived from bytecode instructions
	''' such as {@code invokedynamic}, specifically from the type descriptor strings associated
	''' with the instructions in a class file's constant pool.
	''' <p>
	''' Like classes and strings, method types can also be represented directly
	''' in a class file's constant pool as constants.
	''' A method type may be loaded by an {@code ldc} instruction which refers
	''' to a suitable {@code CONSTANT_MethodType} constant pool entry.
	''' The entry refers to a {@code CONSTANT_Utf8} spelling for the descriptor string.
	''' (For full details on method type constants,
	''' see sections 4.4.8 and 5.4.3.5 of the Java Virtual Machine Specification.)
	''' <p>
	''' When the JVM materializes a {@code MethodType} from a descriptor string,
	''' all classes named in the descriptor must be accessible, and will be loaded.
	''' (But the classes need not be initialized, as is the case with a {@code CONSTANT_Class}.)
	''' This loading may occur at any time before the {@code MethodType} object is first derived.
	''' @author John Rose, JSR 292 EG
	''' </summary>
	<Serializable> _
	Public NotInheritable Class MethodType_Renamed
		Private Const serialVersionUID As Long = 292L ' {rtype, {ptype...}}

		' The rtype and ptypes fields define the structural identity of the method type:
		Private ReadOnly rtype_Renamed As Class
		Private ReadOnly ptypes_Renamed As Class()

		' The remaining fields are caches of various sorts:
		Private MethodTypeForm As Stable ' erased form, plus cached data about primitives
		Private MethodType_Renamed As Stable ' alternative wrapped/unwrapped version
		Private Invokers_Renamed As Stable ' cache of handy higher-order adapters
		Private [String] As Stable ' cache for toMethodDescriptorString

		''' <summary>
		''' Check the given parameters for validity and store them into the final fields.
		''' </summary>
		Private Sub New(ByVal rtype As Class, ByVal ptypes As Class(), ByVal trusted As Boolean)
			checkRtype(rtype)
			checkPtypes(ptypes)
			Me.rtype_Renamed = rtype
			' defensively copy the array passed in by the user
			Me.ptypes_Renamed = If(trusted, ptypes, java.util.Arrays.copyOf(ptypes, ptypes.Length))
		End Sub

		''' <summary>
		''' Construct a temporary unchecked instance of MethodType for use only as a key to the intern table.
		''' Does not check the given parameters for validity, and must be discarded after it is used as a searching key.
		''' The parameters are reversed for this constructor, so that is is not accidentally used.
		''' </summary>
		Private Sub New(ByVal ptypes As Class(), ByVal rtype As Class)
			Me.rtype_Renamed = rtype
			Me.ptypes_Renamed = ptypes
		End Sub

		'trusted
	 Friend Function form() As MethodTypeForm
		 Return form
	 End Function
		'trusted
	 Friend Function rtype() As Class
		 Return rtype_Renamed
	 End Function
		'trusted
	 Friend Function ptypes() As Class()
		 Return ptypes_Renamed
	 End Function

		Friend Property form As MethodTypeForm
			Set(ByVal f As MethodTypeForm)
				form = f
			End Set
		End Property

		''' <summary>
		''' This number, mandated by the JVM spec as 255,
		'''  is the maximum number of <em>slots</em>
		'''  that any Java method can receive in its argument list.
		'''  It limits both JVM signatures and method type objects.
		'''  The longest possible invocation will look like
		'''  {@code staticMethod(arg1, arg2, ..., arg255)} or
		'''  {@code x.virtualMethod(arg1, arg2, ..., arg254)}.
		''' </summary>
		'non-public
	 Friend Const MAX_JVM_ARITY As Integer = 255 ' this is mandated by the JVM spec.

		''' <summary>
		''' This number is the maximum arity of a method handle, 254.
		'''  It is derived from the absolute JVM-imposed arity by subtracting one,
		'''  which is the slot occupied by the method handle itself at the
		'''  beginning of the argument list used to invoke the method handle.
		'''  The longest possible invocation will look like
		'''  {@code mh.invoke(arg1, arg2, ..., arg254)}.
		''' </summary>
		' Issue:  Should we allow MH.invokeWithArguments to go to the full 255?
		'non-public
	 Friend Shared ReadOnly MAX_MH_ARITY As Integer = MAX_JVM_ARITY-1 ' deduct one for mh receiver

		''' <summary>
		''' This number is the maximum arity of a method handle invoker, 253.
		'''  It is derived from the absolute JVM-imposed arity by subtracting two,
		'''  which are the slots occupied by invoke method handle, and the
		'''  target method handle, which are both at the beginning of the argument
		'''  list used to invoke the target method handle.
		'''  The longest possible invocation will look like
		'''  {@code invokermh.invoke(targetmh, arg1, arg2, ..., arg253)}.
		''' </summary>
		'non-public
	 Friend Shared ReadOnly MAX_MH_INVOKER_ARITY As Integer = MAX_MH_ARITY-1 ' deduct one more for invoker

		Private Shared Sub checkRtype(ByVal rtype As Class)
			java.util.Objects.requireNonNull(rtype)
		End Sub
		Private Shared Sub checkPtype(ByVal ptype As Class)
			java.util.Objects.requireNonNull(ptype)
			If ptype Is GetType(void) Then Throw newIllegalArgumentException("parameter type cannot be void")
		End Sub
		''' <summary>
		''' Return number of extra slots (count of long/double args). </summary>
		Private Shared Function checkPtypes(ByVal ptypes As Class()) As Integer
			Dim slots As Integer = 0
			For Each ptype As Class In ptypes
				checkPtype(ptype)
				If ptype Is GetType(Double) OrElse ptype Is GetType(Long) Then slots += 1
			Next ptype
			checkSlotCount(ptypes.Length + slots)
			Return slots
		End Function
		Friend Shared Sub checkSlotCount(ByVal count As Integer)
			assert((MAX_JVM_ARITY And (MAX_JVM_ARITY+1)) = 0)
			' MAX_JVM_ARITY must be power of 2 minus 1 for following code trick to work:
			If (count And MAX_JVM_ARITY) <> count Then Throw newIllegalArgumentException("bad parameter count " & count)
		End Sub
		Private Shared Function newIndexOutOfBoundsException(ByVal num As Object) As IndexOutOfBoundsException
			If TypeOf num Is Integer? Then num = "bad index: " & num
			Return New IndexOutOfBoundsException(num.ToString())
		End Function

		Friend Shared ReadOnly internTable As New ConcurrentWeakInternSet(Of MethodType_Renamed)

		Friend Shared ReadOnly NO_PTYPES As Class() = {}

		''' <summary>
		''' Finds or creates an instance of the given method type. </summary>
		''' <param name="rtype">  the return type </param>
		''' <param name="ptypes"> the parameter types </param>
		''' <returns> a method type with the given components </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypes} is {@code void.class} </exception>
		Public Shared Function methodType(ByVal rtype As Class, ByVal ptypes As Class()) As MethodType
			Return makeImpl(rtype, ptypes, False)
		End Function

		''' <summary>
		''' Finds or creates a method type with the given components.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="rtype">  the return type </param>
		''' <param name="ptypes"> the parameter types </param>
		''' <returns> a method type with the given components </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypes} is {@code void.class} </exception>
		Public Shared Function methodType(ByVal rtype As Class, ByVal ptypes As IList(Of [Class])) As MethodType
			Dim notrust As Boolean = False ' random List impl. could return evil ptypes array
			Return makeImpl(rtype, listToArray(ptypes), notrust)
		End Function

		Private Shared Function listToArray(ByVal ptypes As IList(Of [Class])) As Class()
			' sanity check the size before the toArray call, since size might be huge
			checkSlotCount(ptypes.Count)
			Return ptypes.ToArray(NO_PTYPES)
		End Function

		''' <summary>
		''' Finds or creates a method type with the given components.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' The leading parameter type is prepended to the remaining array. </summary>
		''' <param name="rtype">  the return type </param>
		''' <param name="ptype0"> the first parameter type </param>
		''' <param name="ptypes"> the remaining parameter types </param>
		''' <returns> a method type with the given components </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} or {@code ptype0} or {@code ptypes} or any element of {@code ptypes} is null </exception>
		''' <exception cref="IllegalArgumentException"> if {@code ptype0} or {@code ptypes} or any element of {@code ptypes} is {@code void.class} </exception>
		Public Shared Function methodType(ByVal rtype As Class, ByVal ptype0 As Class, ParamArray ByVal ptypes As Class()) As MethodType
			Dim ptypes1 As Class() = New [Class](1+ptypes.Length - 1){}
			ptypes1(0) = ptype0
			Array.Copy(ptypes, 0, ptypes1, 1, ptypes.Length)
			Return makeImpl(rtype, ptypes1, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with the given components.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' The resulting method has no parameter types. </summary>
		''' <param name="rtype">  the return type </param>
		''' <returns> a method type with the given return value </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} is null </exception>
		Public Shared Function methodType(ByVal rtype As Class) As MethodType
			Return makeImpl(rtype, NO_PTYPES, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with the given components.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' The resulting method has the single given parameter type. </summary>
		''' <param name="rtype">  the return type </param>
		''' <param name="ptype0"> the parameter type </param>
		''' <returns> a method type with the given return value and parameter type </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} or {@code ptype0} is null </exception>
		''' <exception cref="IllegalArgumentException"> if {@code ptype0} is {@code void.class} </exception>
		Public Shared Function methodType(ByVal rtype As Class, ByVal ptype0 As Class) As MethodType
			Return makeImpl(rtype, New [Class](){ ptype0 }, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with the given components.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' The resulting method has the same parameter types as {@code ptypes},
		''' and the specified return type. </summary>
		''' <param name="rtype">  the return type </param>
		''' <param name="ptypes"> the method type which supplies the parameter types </param>
		''' <returns> a method type with the given components </returns>
		''' <exception cref="NullPointerException"> if {@code rtype} or {@code ptypes} is null </exception>
		Public Shared Function methodType(ByVal rtype As Class, ByVal ptypes As MethodType) As MethodType
			Return makeImpl(rtype, ptypes.ptypes_Renamed, True)
		End Function

		''' <summary>
		''' Sole factory method to find or create an interned method type. </summary>
		''' <param name="rtype"> desired return type </param>
		''' <param name="ptypes"> desired parameter types </param>
		''' <param name="trusted"> whether the ptypes can be used without cloning </param>
		''' <returns> the unique method type of the desired structure </returns>
		'trusted
	 Shared Function makeImpl(ByVal rtype As Class, ByVal ptypes As Class(), ByVal trusted As Boolean) As MethodType
			Dim mt As MethodType = internTable.get(New MethodType(ptypes, rtype))
			If mt IsNot Nothing Then Return mt
			If ptypes.Length = 0 Then
				ptypes = NO_PTYPES
				trusted = True
			End If
			mt = New MethodType(rtype, ptypes, trusted)
			' promote the object to the Real Thing, and reprobe
			mt.form = MethodTypeForm.findForm(mt)
			Return internTable.add(mt)
	 End Function
		Private Shared ReadOnly objectOnlyTypes As MethodType() = New MethodType_Renamed(19){}

		''' <summary>
		''' Finds or creates a method type whose components are {@code Object} with an optional trailing {@code Object[]} array.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' All parameters and the return type will be {@code Object},
		''' except the final array parameter if any, which will be {@code Object[]}. </summary>
		''' <param name="objectArgCount"> number of parameters (excluding the final array parameter if any) </param>
		''' <param name="finalArray"> whether there will be a trailing array parameter, of type {@code Object[]} </param>
		''' <returns> a generally applicable method type, for all calls of the given fixed argument count and a collected array of further arguments </returns>
		''' <exception cref="IllegalArgumentException"> if {@code objectArgCount} is negative or greater than 255 (or 254, if {@code finalArray} is true) </exception>
		''' <seealso cref= #genericMethodType(int) </seealso>
		Public Shared Function genericMethodType(ByVal objectArgCount As Integer, ByVal finalArray As Boolean) As MethodType
			Dim mt As MethodType
			checkSlotCount(objectArgCount)
			Dim ivarargs As Integer = (If((Not finalArray), 0, 1))
			Dim ootIndex As Integer = objectArgCount*2 + ivarargs
			If ootIndex < objectOnlyTypes.Length Then
				mt = objectOnlyTypes(ootIndex)
				If mt IsNot Nothing Then Return mt
			End If
			Dim ptypes As Class() = New [Class](objectArgCount + ivarargs - 1){}
			java.util.Arrays.fill(ptypes, GetType(Object))
			If ivarargs <> 0 Then ptypes(objectArgCount) = GetType(Object())
			mt = makeImpl(GetType(Object), ptypes, True)
			If ootIndex < objectOnlyTypes.Length Then objectOnlyTypes(ootIndex) = mt ' cache it here also!
			Return mt
		End Function

		''' <summary>
		''' Finds or creates a method type whose components are all {@code Object}.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' All parameters and the return type will be Object. </summary>
		''' <param name="objectArgCount"> number of parameters </param>
		''' <returns> a generally applicable method type, for all calls of the given argument count </returns>
		''' <exception cref="IllegalArgumentException"> if {@code objectArgCount} is negative or greater than 255 </exception>
		''' <seealso cref= #genericMethodType(int, boolean) </seealso>
		Public Shared Function genericMethodType(ByVal objectArgCount As Integer) As MethodType
			Return genericMethodType(objectArgCount, False)
		End Function

		''' <summary>
		''' Finds or creates a method type with a single different parameter type.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="num">    the index (zero-based) of the parameter type to change </param>
		''' <param name="nptype"> a new parameter type to replace the old one with </param>
		''' <returns> the same type, except with the selected parameter changed </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code num} is not a valid index into {@code parameterArray()} </exception>
		''' <exception cref="IllegalArgumentException"> if {@code nptype} is {@code void.class} </exception>
		''' <exception cref="NullPointerException"> if {@code nptype} is null </exception>
		Public Function changeParameterType(ByVal num As Integer, ByVal nptype As Class) As MethodType
			If parameterType(num) Is nptype Then Return Me
			checkPtype(nptype)
			Dim nptypes As Class() = ptypes_Renamed.clone()
			nptypes(num) = nptype
			Return makeImpl(rtype_Renamed, nptypes, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with additional parameter types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="num">    the position (zero-based) of the inserted parameter type(s) </param>
		''' <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		''' <returns> the same type, except with the selected parameter(s) inserted </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		'''                                  or if the resulting method type would have more than 255 parameter slots </exception>
		''' <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		Public Function insertParameterTypes(ByVal num As Integer, ParamArray ByVal ptypesToInsert As Class()) As MethodType
			Dim len As Integer = ptypes_Renamed.Length
			If num < 0 OrElse num > len Then Throw newIndexOutOfBoundsException(num)
			Dim ins As Integer = checkPtypes(ptypesToInsert)
			checkSlotCount(parameterSlotCount() + ptypesToInsert.Length + ins)
			Dim ilen As Integer = ptypesToInsert.Length
			If ilen = 0 Then Return Me
			Dim nptypes As Class() = java.util.Arrays.copyOfRange(ptypes_Renamed, 0, len+ilen)
			Array.Copy(nptypes, num, nptypes, num+ilen, len-num)
			Array.Copy(ptypesToInsert, 0, nptypes, num, ilen)
			Return makeImpl(rtype_Renamed, nptypes, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with additional parameter types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="ptypesToInsert"> zero or more new parameter types to insert after the end of the parameter list </param>
		''' <returns> the same type, except with the selected parameter(s) appended </returns>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		'''                                  or if the resulting method type would have more than 255 parameter slots </exception>
		''' <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		Public Function appendParameterTypes(ParamArray ByVal ptypesToInsert As Class()) As MethodType
			Return insertParameterTypes(parameterCount(), ptypesToInsert)
		End Function

		''' <summary>
		''' Finds or creates a method type with additional parameter types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="num">    the position (zero-based) of the inserted parameter type(s) </param>
		''' <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		''' <returns> the same type, except with the selected parameter(s) inserted </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		'''                                  or if the resulting method type would have more than 255 parameter slots </exception>
		''' <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		Public Function insertParameterTypes(ByVal num As Integer, ByVal ptypesToInsert As IList(Of [Class])) As MethodType
			Return insertParameterTypes(num, listToArray(ptypesToInsert))
		End Function

		''' <summary>
		''' Finds or creates a method type with additional parameter types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="ptypesToInsert"> zero or more new parameter types to insert after the end of the parameter list </param>
		''' <returns> the same type, except with the selected parameter(s) appended </returns>
		''' <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		'''                                  or if the resulting method type would have more than 255 parameter slots </exception>
		''' <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		Public Function appendParameterTypes(ByVal ptypesToInsert As IList(Of [Class])) As MethodType
			Return insertParameterTypes(parameterCount(), ptypesToInsert)
		End Function

		 ''' <summary>
		 ''' Finds or creates a method type with modified parameter types.
		 ''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		 ''' <param name="start">  the position (zero-based) of the first replaced parameter type(s) </param>
		 ''' <param name="end">    the position (zero-based) after the last replaced parameter type(s) </param>
		 ''' <param name="ptypesToInsert"> zero or more new parameter types to insert into the parameter list </param>
		 ''' <returns> the same type, except with the selected parameter(s) replaced </returns>
		 ''' <exception cref="IndexOutOfBoundsException"> if {@code start} is negative or greater than {@code parameterCount()}
		 '''                                  or if {@code end} is negative or greater than {@code parameterCount()}
		 '''                                  or if {@code start} is greater than {@code end} </exception>
		 ''' <exception cref="IllegalArgumentException"> if any element of {@code ptypesToInsert} is {@code void.class}
		 '''                                  or if the resulting method type would have more than 255 parameter slots </exception>
		 ''' <exception cref="NullPointerException"> if {@code ptypesToInsert} or any of its elements is null </exception>
		'non-public
	 Friend Function replaceParameterTypes(ByVal start As Integer, ByVal [end] As Integer, ParamArray ByVal ptypesToInsert As Class()) As MethodType
			If start = [end] Then Return insertParameterTypes(start, ptypesToInsert)
			Dim len As Integer = ptypes_Renamed.Length
			If Not(0 <= start AndAlso start <= [end] AndAlso [end] <= len) Then Throw newIndexOutOfBoundsException("start=" & start & " end=" & [end])
			Dim ilen As Integer = ptypesToInsert.Length
			If ilen = 0 Then Return dropParameterTypes(start, [end])
			Return dropParameterTypes(start, [end]).insertParameterTypes(start, ptypesToInsert)
	 End Function

		''' <summary>
		''' Replace the last arrayLength parameter types with the component type of arrayType. </summary>
		''' <param name="arrayType"> any array type </param>
		''' <param name="arrayLength"> the number of parameter types to change </param>
		''' <returns> the resulting type </returns>
		'non-public
	 Friend Function asSpreaderType(ByVal arrayType As Class, ByVal arrayLength As Integer) As MethodType
			assert(parameterCount() >= arrayLength)
			Dim spreadPos As Integer = ptypes_Renamed.Length - arrayLength
			If arrayLength = 0 Then ' nothing to change Return Me
			If arrayType Is GetType(Object()) Then
				If generic Then ' nothing to change Return Me
				If spreadPos = 0 Then
					' no leading arguments to preserve; go generic
					Dim res As MethodType = genericMethodType(arrayLength)
					If rtype_Renamed IsNot GetType(Object) Then res = res.changeReturnType(rtype_Renamed)
					Return res
				End If
			End If
			Dim elemType As Class = arrayType.componentType
			assert(elemType IsNot Nothing)
			For i As Integer = spreadPos To ptypes_Renamed.Length - 1
				If ptypes_Renamed(i) IsNot elemType Then
					Dim fixedPtypes As Class() = ptypes_Renamed.clone()
					java.util.Arrays.fill(fixedPtypes, i, ptypes_Renamed.Length, elemType)
					Return methodType(rtype_Renamed, fixedPtypes)
				End If
			Next i
			Return Me ' arguments check out; no change
	 End Function

		''' <summary>
		''' Return the leading parameter type, which must exist and be a reference. </summary>
		'''  <returns> the leading parameter type, after error checks </returns>
		'non-public
	 Friend Function leadingReferenceParameter() As Class
			Dim ptype As Class
			ptype = ptypes_Renamed(0)
			If ptypes_Renamed.Length = 0 OrElse ptype .primitive Then Throw newIllegalArgumentException("no leading reference parameter")
			Return ptype
	 End Function

		''' <summary>
		''' Delete the last parameter type and replace it with arrayLength copies of the component type of arrayType. </summary>
		''' <param name="arrayType"> any array type </param>
		''' <param name="arrayLength"> the number of parameter types to insert </param>
		''' <returns> the resulting type </returns>
		'non-public
	 Friend Function asCollectorType(ByVal arrayType As Class, ByVal arrayLength As Integer) As MethodType
			assert(parameterCount() >= 1)
			assert(arrayType.IsSubclassOf(lastParameterType()))
			Dim res As MethodType
			If arrayType Is GetType(Object()) Then
				res = genericMethodType(arrayLength)
				If rtype_Renamed IsNot GetType(Object) Then res = res.changeReturnType(rtype_Renamed)
			Else
				Dim elemType As Class = arrayType.componentType
				assert(elemType IsNot Nothing)
				res = methodType(rtype_Renamed, java.util.Collections.nCopies(arrayLength, elemType))
			End If
			If ptypes_Renamed.Length = 1 Then
				Return res
			Else
				Return res.insertParameterTypes(0, parameterList().subList(0, ptypes_Renamed.Length-1))
			End If
	 End Function

		''' <summary>
		''' Finds or creates a method type with some parameter types omitted.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="start">  the index (zero-based) of the first parameter type to remove </param>
		''' <param name="end">    the index (greater than {@code start}) of the first parameter type after not to remove </param>
		''' <returns> the same type, except with the selected parameter(s) removed </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code start} is negative or greater than {@code parameterCount()}
		'''                                  or if {@code end} is negative or greater than {@code parameterCount()}
		'''                                  or if {@code start} is greater than {@code end} </exception>
		Public Function dropParameterTypes(ByVal start As Integer, ByVal [end] As Integer) As MethodType
			Dim len As Integer = ptypes_Renamed.Length
			If Not(0 <= start AndAlso start <= [end] AndAlso [end] <= len) Then Throw newIndexOutOfBoundsException("start=" & start & " end=" & [end])
			If start = [end] Then Return Me
			Dim nptypes As Class()
			If start = 0 Then
				If [end] = len Then
					' drop all parameters
					nptypes = NO_PTYPES
				Else
					' drop initial parameter(s)
					nptypes = java.util.Arrays.copyOfRange(ptypes_Renamed, [end], len)
				End If
			Else
				If [end] = len Then
					' drop trailing parameter(s)
					nptypes = java.util.Arrays.copyOfRange(ptypes_Renamed, 0, start)
				Else
					Dim tail As Integer = len - [end]
					nptypes = java.util.Arrays.copyOfRange(ptypes_Renamed, 0, start + tail)
					Array.Copy(ptypes_Renamed, [end], nptypes, start, tail)
				End If
			End If
			Return makeImpl(rtype_Renamed, nptypes, True)
		End Function

		''' <summary>
		''' Finds or creates a method type with a different return type.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>. </summary>
		''' <param name="nrtype"> a return parameter type to replace the old one with </param>
		''' <returns> the same type, except with the return type change </returns>
		''' <exception cref="NullPointerException"> if {@code nrtype} is null </exception>
		Public Function changeReturnType(ByVal nrtype As Class) As MethodType
			If returnType() Is nrtype Then Return Me
			Return makeImpl(nrtype, ptypes_Renamed, True)
		End Function

		''' <summary>
		''' Reports if this type contains a primitive argument or return value.
		''' The return type {@code void} counts as a primitive. </summary>
		''' <returns> true if any of the types are primitives </returns>
		Public Function hasPrimitives() As Boolean
			Return form.hasPrimitives()
		End Function

		''' <summary>
		''' Reports if this type contains a wrapper argument or return value.
		''' Wrappers are types which box primitive values, such as <seealso cref="Integer"/>.
		''' The reference type {@code java.lang.Void} counts as a wrapper,
		''' if it occurs as a return type. </summary>
		''' <returns> true if any of the types are wrappers </returns>
		Public Function hasWrappers() As Boolean
			Return unwrap() IsNot Me
		End Function

		''' <summary>
		''' Erases all reference types to {@code Object}.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' All primitive types (including {@code void}) will remain unchanged. </summary>
		''' <returns> a version of the original type with all reference types replaced </returns>
		Public Function [erase]() As MethodType
			Return form.erasedType()
		End Function

		''' <summary>
		''' Erases all reference types to {@code Object}, and all subword types to {@code int}.
		''' This is the reduced type polymorphism used by private methods
		''' such as <seealso cref="MethodHandle#invokeBasic invokeBasic"/>. </summary>
		''' <returns> a version of the original type with all reference and subword types replaced </returns>
		'non-public
	 Friend Function basicType() As MethodType
			Return form.basicType()
	 End Function

		''' <returns> a version of the original type with MethodHandle prepended as the first argument </returns>
		'non-public
	 Friend Function invokerType() As MethodType
			Return insertParameterTypes(0, GetType(MethodHandle))
	 End Function

		''' <summary>
		''' Converts all types, both reference and primitive, to {@code Object}.
		''' Convenience method for <seealso cref="#genericMethodType(int) genericMethodType"/>.
		''' The expression {@code type.wrap().erase()} produces the same value
		''' as {@code type.generic()}. </summary>
		''' <returns> a version of the original type with all types replaced </returns>
		Public Function generic() As MethodType
			Return genericMethodType(parameterCount())
		End Function

		'non-public
	 Friend Property generic As Boolean
		 Get
				Return Me Is [erase]() AndAlso Not hasPrimitives()
		 End Get
	 End Property

		''' <summary>
		''' Converts all primitive types to their corresponding wrapper types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' All reference types (including wrapper types) will remain unchanged.
		''' A {@code void} return type is changed to the type {@code java.lang.Void}.
		''' The expression {@code type.wrap().erase()} produces the same value
		''' as {@code type.generic()}. </summary>
		''' <returns> a version of the original type with all primitive types replaced </returns>
		Public Function wrap() As MethodType
			Return If(hasPrimitives(), wrapWithPrims(Me), Me)
		End Function

		''' <summary>
		''' Converts all wrapper types to their corresponding primitive types.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' All primitive types (including {@code void}) will remain unchanged.
		''' A return type of {@code java.lang.Void} is changed to {@code void}. </summary>
		''' <returns> a version of the original type with all wrapper types replaced </returns>
		Public Function unwrap() As MethodType
			Dim noprims As MethodType = If((Not hasPrimitives()), Me, wrapWithPrims(Me))
			Return unwrapWithNoPrims(noprims)
		End Function

		Private Shared Function wrapWithPrims(ByVal pt As MethodType) As MethodType
			assert(pt.hasPrimitives())
			Dim wt As MethodType = pt.wrapAlt
			If wt Is Nothing Then
				' fill in lazily
				wt = MethodTypeForm.canonicalize(pt, MethodTypeForm.WRAP, MethodTypeForm.WRAP)
				assert(wt IsNot Nothing)
				pt.wrapAlt = wt
			End If
			Return wt
		End Function

		Private Shared Function unwrapWithNoPrims(ByVal wt As MethodType) As MethodType
			assert((Not wt.hasPrimitives()))
			Dim uwt As MethodType = wt.wrapAlt
			If uwt Is Nothing Then
				' fill in lazily
				uwt = MethodTypeForm.canonicalize(wt, MethodTypeForm.UNWRAP, MethodTypeForm.UNWRAP)
				If uwt Is Nothing Then uwt = wt ' type has no wrappers or prims at all
				wt.wrapAlt = uwt
			End If
			Return uwt
		End Function

		''' <summary>
		''' Returns the parameter type at the specified index, within this method type. </summary>
		''' <param name="num"> the index (zero-based) of the desired parameter type </param>
		''' <returns> the selected parameter type </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code num} is not a valid index into {@code parameterArray()} </exception>
		Public Function parameterType(ByVal num As Integer) As Class
			Return ptypes_Renamed(num)
		End Function
		''' <summary>
		''' Returns the number of parameter types in this method type. </summary>
		''' <returns> the number of parameter types </returns>
		Public Function parameterCount() As Integer
			Return ptypes_Renamed.Length
		End Function
		''' <summary>
		''' Returns the return type of this method type. </summary>
		''' <returns> the return type </returns>
		Public Function returnType() As Class
			Return rtype_Renamed
		End Function

		''' <summary>
		''' Presents the parameter types as a list (a convenience method).
		''' The list will be immutable. </summary>
		''' <returns> the parameter types (as an immutable list) </returns>
		Public Function parameterList() As IList(Of [Class])
			Return java.util.Collections.unmodifiableList(java.util.Arrays.asList(ptypes_Renamed.clone()))
		End Function

		'non-public
	 Friend Function lastParameterType() As Class
			Dim len As Integer = ptypes_Renamed.Length
			Return If(len = 0, GetType(void), ptypes_Renamed(len-1))
	 End Function

		''' <summary>
		''' Presents the parameter types as an array (a convenience method).
		''' Changes to the array will not result in changes to the type. </summary>
		''' <returns> the parameter types (as a fresh copy if necessary) </returns>
		Public Function parameterArray() As Class()
			Return ptypes_Renamed.clone()
		End Function

		''' <summary>
		''' Compares the specified object with this type for equality.
		''' That is, it returns <tt>true</tt> if and only if the specified object
		''' is also a method type with exactly the same parameters and return type. </summary>
		''' <param name="x"> object to compare </param>
		''' <seealso cref= Object#equals(Object) </seealso>
		Public Overrides Function Equals(ByVal x As Object) As Boolean
			Return Me Is x OrElse TypeOf x Is MethodType_Renamed AndAlso Equals(CType(x, MethodType_Renamed))
		End Function

		Private Overrides Function Equals(ByVal that As MethodType) As Boolean
			Return Me.rtype_Renamed Is that.rtype_Renamed AndAlso java.util.Arrays.Equals(Me.ptypes_Renamed, that.ptypes_Renamed)
		End Function

		''' <summary>
		''' Returns the hash code value for this method type.
		''' It is defined to be the same as the hashcode of a List
		''' whose elements are the return type followed by the
		''' parameter types. </summary>
		''' <returns> the hash code value for this method type </returns>
		''' <seealso cref= Object#hashCode() </seealso>
		''' <seealso cref= #equals(Object) </seealso>
		''' <seealso cref= List#hashCode() </seealso>
		Public Overrides Function GetHashCode() As Integer
		  Dim hashCode As Integer = 31 + rtype_Renamed.GetHashCode()
		  For Each ptype As Class In ptypes_Renamed
			  hashCode = 31*hashCode + ptype.GetHashCode()
		  Next ptype
		  Return hashCode
		End Function

		''' <summary>
		''' Returns a string representation of the method type,
		''' of the form {@code "(PT0,PT1...)RT"}.
		''' The string representation of a method type is a
		''' parenthesis enclosed, comma separated list of type names,
		''' followed immediately by the return type.
		''' <p>
		''' Each type is represented by its
		''' <seealso cref="java.lang.Class#getSimpleName simple name"/>.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			sb.append("(")
			For i As Integer = 0 To ptypes_Renamed.Length - 1
				If i > 0 Then sb.append(",")
				sb.append(ptypes_Renamed(i).simpleName)
			Next i
			sb.append(")")
			sb.append(rtype_Renamed.simpleName)
			Return sb.ToString()
		End Function

		''' <summary>
		''' True if the old return type can always be viewed (w/o casting) under new return type,
		'''  and the new parameters can be viewed (w/o casting) under the old parameter types.
		''' </summary>
		'non-public
		Friend Function isViewableAs(ByVal newType As MethodType, ByVal keepInterfaces As Boolean) As Boolean
			If Not sun.invoke.util.VerifyType.isNullConversion(returnType(), newType.returnType(), keepInterfaces) Then Return False
			Return parametersAreViewableAs(newType, keepInterfaces)
		End Function
		''' <summary>
		''' True if the new parameters can be viewed (w/o casting) under the old parameter types. </summary>
		'non-public
		Friend Function parametersAreViewableAs(ByVal newType As MethodType, ByVal keepInterfaces As Boolean) As Boolean
			If form = newType.form AndAlso form.erasedType Is Me Then Return True ' my reference parameters are all Object
			If ptypes_Renamed = newType.ptypes_Renamed Then Return True
			Dim argc As Integer = parameterCount()
			If argc <> newType.parameterCount() Then Return False
			For i As Integer = 0 To argc - 1
				If Not sun.invoke.util.VerifyType.isNullConversion(newType.parameterType(i), parameterType(i), keepInterfaces) Then Return False
			Next i
			Return True
		End Function
		'non-public
		Friend Function isConvertibleTo(ByVal newType As MethodType) As Boolean
			Dim oldForm As MethodTypeForm = Me.form()
			Dim newForm As MethodTypeForm = newType.form()
			If oldForm Is newForm Then Return True
			If Not canConvert(returnType(), newType.returnType()) Then Return False
			Dim srcTypes As Class() = newType.ptypes_Renamed
			Dim dstTypes As Class() = ptypes_Renamed
			If srcTypes = dstTypes Then Return True
			Dim argc As Integer
			argc = srcTypes.Length
			If argc <> dstTypes.Length Then Return False
			If argc <= 1 Then
				If argc = 1 AndAlso (Not canConvert(srcTypes(0), dstTypes(0))) Then Return False
				Return True
			End If
			If (oldForm.primitiveParameterCount() = 0 AndAlso oldForm.erasedType_Renamed Is Me) OrElse (newForm.primitiveParameterCount() = 0 AndAlso newForm.erasedType_Renamed Is newType) Then
				' Somewhat complicated test to avoid a loop of 2 or more trips.
				' If either type has only Object parameters, we know we can convert.
				assert(canConvertParameters(srcTypes, dstTypes))
				Return True
			End If
			Return canConvertParameters(srcTypes, dstTypes)
		End Function

		''' <summary>
		''' Returns true if MHs.explicitCastArguments produces the same result as MH.asType.
		'''  If the type conversion is impossible for either, the result should be false.
		''' </summary>
		'non-public
		Friend Function explicitCastEquivalentToAsType(ByVal newType As MethodType) As Boolean
			If Me Is newType Then Return True
			If Not explicitCastEquivalentToAsType(rtype_Renamed, newType.rtype_Renamed) Then Return False
			Dim srcTypes As Class() = newType.ptypes_Renamed
			Dim dstTypes As Class() = ptypes_Renamed
			If dstTypes = srcTypes Then Return True
			assert(dstTypes.Length = srcTypes.Length)
			For i As Integer = 0 To dstTypes.Length - 1
				If Not explicitCastEquivalentToAsType(srcTypes(i), dstTypes(i)) Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Reports true if the src can be converted to the dst, by both asType and MHs.eCE,
		'''  and with the same effect.
		'''  MHs.eCA has the following "upgrades" to MH.asType:
		'''  1. interfaces are unchecked (that is, treated as if aliased to Object)
		'''     Therefore, {@code Object->CharSequence} is possible in both cases but has different semantics
		'''  2. the full matrix of primitive-to-primitive conversions is supported
		'''     Narrowing like {@code long->byte} and basic-typing like {@code boolean->int}
		'''     are not supported by asType, but anything supported by asType is equivalent
		'''     with MHs.eCE.
		'''  3a. unboxing conversions can be followed by the full matrix of primitive conversions
		'''  3b. unboxing of null is permitted (creates a zero primitive value)
		''' Other than interfaces, reference-to-reference conversions are the same.
		''' Boxing primitives to references is the same for both operators.
		''' </summary>
		Private Shared Function explicitCastEquivalentToAsType(ByVal src As Class, ByVal dst As Class) As Boolean
			If src Is dst OrElse dst Is GetType(Object) OrElse dst Is GetType(void) Then Return True
			If src.primitive Then
				' Could be a prim/prim conversion, where casting is a strict superset.
				' Or a boxing conversion, which is always to an exact wrapper class.
				Return canConvert(src, dst)
			ElseIf dst.primitive Then
				' Unboxing behavior is different between MHs.eCA & MH.asType (see 3b).
				Return False
			Else
				' R->R always works, but we have to avoid a check-cast to an interface.
				Return (Not dst.interface) OrElse src.IsSubclassOf(dst)
			End If
		End Function

		Private Function canConvertParameters(ByVal srcTypes As Class(), ByVal dstTypes As Class()) As Boolean
			For i As Integer = 0 To srcTypes.Length - 1
				If Not canConvert(srcTypes(i), dstTypes(i)) Then Return False
			Next i
			Return True
		End Function

		'non-public
		Friend Shared Function canConvert(ByVal src As Class, ByVal dst As Class) As Boolean
			' short-circuit a few cases:
			If src Is dst OrElse src Is GetType(Object) OrElse dst Is GetType(Object) Then Return True
			' the remainder of this logic is documented in MethodHandle.asType
			If src.primitive Then
				' can force void to an explicit null, a la reflect.Method.invoke
				' can also force void to a primitive zero, by analogy
				If src Is GetType(void) Then 'or !dst.isPrimitive()? Return True
				Dim sw As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(src)
				If dst.primitive Then
					' P->P must widen
					Return sun.invoke.util.Wrapper.forPrimitiveType(dst).isConvertibleFrom(sw)
				Else
					' P->R must box and widen
					Return sw.wrapperType().IsSubclassOf(dst)
				End If
			ElseIf dst.primitive Then
				' any value can be dropped
				If dst Is GetType(void) Then Return True
				Dim dw As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(dst)
				' R->P must be able to unbox (from a dynamically chosen type) and widen
				' For example:
				'   Byte/Number/Comparable/Object -> dw:Byte -> byte.
				'   Character/Comparable/Object -> dw:Character -> char
				'   Boolean/Comparable/Object -> dw:Boolean -> boolean
				' This means that dw must be cast-compatible with src.
				If dw.wrapperType().IsSubclassOf(src) Then Return True
				' The above does not work if the source reference is strongly typed
				' to a wrapper whose primitive must be widened.  For example:
				'   Byte -> unbox:byte -> short/int/long/float/double
				'   Character -> unbox:char -> int/long/float/double
				If sun.invoke.util.Wrapper.isWrapperType(src) AndAlso dw.isConvertibleFrom(sun.invoke.util.Wrapper.forWrapperType(src)) Then Return True
				' We have already covered cases which arise due to runtime unboxing
				' of a reference type which covers several wrapper types:
				'   Object -> cast:Integer -> unbox:int -> long/float/double
				'   Serializable -> cast:Byte -> unbox:byte -> byte/short/int/long/float/double
				' An marginal case is Number -> dw:Character -> char, which would be OK if there were a
				' subclass of Number which wraps a value that can convert to char.
				' Since there is none, we don't need an extra check here to cover char or boolean.
				Return False
			Else
				' R->R always works, since null is always valid dynamically
				Return True
			End If
		End Function

		'/ Queries which have to do with the bytecode architecture

		''' <summary>
		''' Reports the number of JVM stack slots required to invoke a method
		''' of this type.  Note that (for historical reasons) the JVM requires
		''' a second stack slot to pass long and double arguments.
		''' So this method returns <seealso cref="#parameterCount() parameterCount"/> plus the
		''' number of long and double parameters (if any).
		''' <p>
		''' This method is included for the benefit of applications that must
		''' generate bytecodes that process method handles and invokedynamic. </summary>
		''' <returns> the number of JVM stack slots for this type's parameters </returns>
		'non-public
	 Friend Function parameterSlotCount() As Integer
			Return form.parameterSlotCount()
	 End Function

		'non-public
	 Friend Function invokers() As Invokers
			Dim inv As Invokers = invokers
			If inv IsNot Nothing Then Return inv
				inv = New Invokers(Me)
				invokers = inv
			Return inv
	 End Function

		''' <summary>
		''' Reports the number of JVM stack slots which carry all parameters including and after
		''' the given position, which must be in the range of 0 to
		''' {@code parameterCount} inclusive.  Successive parameters are
		''' more shallowly stacked, and parameters are indexed in the bytecodes
		''' according to their trailing edge.  Thus, to obtain the depth
		''' in the outgoing call stack of parameter {@code N}, obtain
		''' the {@code parameterSlotDepth} of its trailing edge
		''' at position {@code N+1}.
		''' <p>
		''' Parameters of type {@code long} and {@code double} occupy
		''' two stack slots (for historical reasons) and all others occupy one.
		''' Therefore, the number returned is the number of arguments
		''' <em>including</em> and <em>after</em> the given parameter,
		''' <em>plus</em> the number of long or double arguments
		''' at or after after the argument for the given parameter.
		''' <p>
		''' This method is included for the benefit of applications that must
		''' generate bytecodes that process method handles and invokedynamic. </summary>
		''' <param name="num"> an index (zero-based, inclusive) within the parameter types </param>
		''' <returns> the index of the (shallowest) JVM stack slot transmitting the
		'''         given parameter </returns>
		''' <exception cref="IllegalArgumentException"> if {@code num} is negative or greater than {@code parameterCount()} </exception>
		'non-public
	 Friend Function parameterSlotDepth(ByVal num As Integer) As Integer
			If num < 0 OrElse num > ptypes_Renamed.Length Then parameterType(num) ' force a range check
			Return form.parameterToArgSlot(num-1)
	 End Function

		''' <summary>
		''' Reports the number of JVM stack slots required to receive a return value
		''' from a method of this type.
		''' If the <seealso cref="#returnType() return type"/> is void, it will be zero,
		''' else if the return type is long or double, it will be two, else one.
		''' <p>
		''' This method is included for the benefit of applications that must
		''' generate bytecodes that process method handles and invokedynamic. </summary>
		''' <returns> the number of JVM stack slots (0, 1, or 2) for this type's return value
		''' Will be removed for PFD. </returns>
		'non-public
	 Friend Function returnSlotCount() As Integer
			Return form.returnSlotCount()
	 End Function

		''' <summary>
		''' Finds or creates an instance of a method type, given the spelling of its bytecode descriptor.
		''' Convenience method for <seealso cref="#methodType(java.lang.Class, java.lang.Class[]) methodType"/>.
		''' Any class or interface name embedded in the descriptor string
		''' will be resolved by calling <seealso cref="ClassLoader#loadClass(java.lang.String)"/>
		''' on the given loader (or if it is null, on the system class loader).
		''' <p>
		''' Note that it is possible to encounter method types which cannot be
		''' constructed by this method, because their component types are
		''' not all reachable from a common class loader.
		''' <p>
		''' This method is included for the benefit of applications that must
		''' generate bytecodes that process method handles and {@code invokedynamic}. </summary>
		''' <param name="descriptor"> a bytecode-level type descriptor string "(T...)T" </param>
		''' <param name="loader"> the class loader in which to look up the types </param>
		''' <returns> a method type matching the bytecode-level type descriptor </returns>
		''' <exception cref="NullPointerException"> if the string is null </exception>
		''' <exception cref="IllegalArgumentException"> if the string is not well-formed </exception>
		''' <exception cref="TypeNotPresentException"> if a named type cannot be found </exception>
		Public Shared Function fromMethodDescriptorString(ByVal descriptor As String, ByVal loader As ClassLoader) As MethodType
			If (Not descriptor.StartsWith("(")) OrElse descriptor.IndexOf(")"c) < 0 OrElse descriptor.IndexOf("."c) >= 0 Then ' also generates NPE if needed Throw newIllegalArgumentException("not a method descriptor: " & descriptor)
			Dim types As IList(Of [Class]) = sun.invoke.util.BytecodeDescriptor.parseMethod(descriptor, loader)
			Dim rtype As Class = types.Remove(types.Count - 1)
			checkSlotCount(types.Count)
			Dim ptypes As Class() = listToArray(types)
			Return makeImpl(rtype, ptypes, True)
		End Function

		''' <summary>
		''' Produces a bytecode descriptor representation of the method type.
		''' <p>
		''' Note that this is not a strict inverse of <seealso cref="#fromMethodDescriptorString fromMethodDescriptorString"/>.
		''' Two distinct classes which share a common name but have different class loaders
		''' will appear identical when viewed within descriptor strings.
		''' <p>
		''' This method is included for the benefit of applications that must
		''' generate bytecodes that process method handles and {@code invokedynamic}.
		''' <seealso cref="#fromMethodDescriptorString(java.lang.String, java.lang.ClassLoader) fromMethodDescriptorString"/>,
		''' because the latter requires a suitable class loader argument. </summary>
		''' <returns> the bytecode type descriptor representation </returns>
		Public Function toMethodDescriptorString() As String
			Dim desc As String = methodDescriptor
			If desc Is Nothing Then
				desc = sun.invoke.util.BytecodeDescriptor.unparse(Me)
				methodDescriptor = desc
			End If
			Return desc
		End Function

		'non-public
	 Friend Shared Function toFieldDescriptorString(ByVal cls As Class) As String
			Return sun.invoke.util.BytecodeDescriptor.unparse(cls)
	 End Function

		'/ Serialization.

		''' <summary>
		''' There are no serializable fields for {@code MethodType}.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { }

		''' <summary>
		''' Save the {@code MethodType} instance to a stream.
		''' 
		''' @serialData
		''' For portability, the serialized format does not refer to named fields.
		''' Instead, the return type and parameter type arrays are written directly
		''' from the {@code writeObject} method, using two calls to {@code s.writeObject}
		''' as follows:
		''' <blockquote><pre>{@code
		''' s.writeObject(this.returnType());
		''' s.writeObject(this.parameterArray());
		''' }</pre></blockquote>
		''' <p>
		''' The deserialized field values are checked as if they were
		''' provided to the factory method <seealso cref="#methodType(Class,Class[]) methodType"/>.
		''' For example, null values, or {@code void} parameter types,
		''' will lead to exceptions during deserialization. </summary>
		''' <param name="s"> the stream to write the object to </param>
		''' <exception cref="java.io.IOException"> if there is a problem writing the object </exception>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject() ' requires serialPersistentFields to be an empty array
			s.writeObject(returnType())
			s.writeObject(parameterArray())
		End Sub

		''' <summary>
		''' Reconstitute the {@code MethodType} instance from a stream (that is,
		''' deserialize it).
		''' This instance is a scratch object with bogus final fields.
		''' It provides the parameters to the factory method called by
		''' <seealso cref="#readResolve readResolve"/>.
		''' After that call it is discarded. </summary>
		''' <param name="s"> the stream to read the object from </param>
		''' <exception cref="java.io.IOException"> if there is a problem reading the object </exception>
		''' <exception cref="ClassNotFoundException"> if one of the component classes cannot be resolved </exception>
		''' <seealso cref= #MethodType() </seealso>
		''' <seealso cref= #readResolve </seealso>
		''' <seealso cref= #writeObject </seealso>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject() ' requires serialPersistentFields to be an empty array

			Dim returnType As Class = CType(s.readObject(), [Class])
			Dim parameterArray As Class() = CType(s.readObject(), Class())

			' Probably this object will never escape, but let's check
			' the field values now, just to be sure.
			checkRtype(returnType)
			checkPtypes(parameterArray)

			parameterArray = parameterArray.clone() ' make sure it is unshared
			MethodType_init(returnType, parameterArray)
		End Sub

		''' <summary>
		''' For serialization only.
		''' Sets the final fields to null, pending {@code Unsafe.putObject}.
		''' </summary>
		Private Sub New()
			Me.rtype_Renamed = Nothing
			Me.ptypes_Renamed = Nothing
		End Sub
		Private Sub MethodType_init(ByVal rtype As Class, ByVal ptypes As Class())
			' In order to communicate these values to readResolve, we must
			' store them into the implementation-specific final fields.
			checkRtype(rtype)
			checkPtypes(ptypes)
			UNSAFE.putObject(Me, rtypeOffset, rtype)
			UNSAFE.putObject(Me, ptypesOffset, ptypes)
		End Sub

		' Support for resetting final fields while deserializing
		Private Shared ReadOnly rtypeOffset, ptypesOffset As Long
		Shared Sub New()
			Try
				rtypeOffset = UNSAFE.objectFieldOffset(GetType(MethodType).getDeclaredField("rtype"))
				ptypesOffset = UNSAFE.objectFieldOffset(GetType(MethodType).getDeclaredField("ptypes"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

		''' <summary>
		''' Resolves and initializes a {@code MethodType} object
		''' after serialization. </summary>
		''' <returns> the fully initialized {@code MethodType} object </returns>
		Private Function readResolve() As Object
			' Do not use a trusted path for deserialization:
			'return makeImpl(rtype, ptypes, true);
			' Verify all operands, and make sure ptypes is unshared:
			Return methodType(rtype_Renamed, ptypes_Renamed)
		End Function

		''' <summary>
		''' Simple implementation of weak concurrent intern set.
		''' </summary>
		''' @param <T> interned type </param>
		Private Class ConcurrentWeakInternSet(Of T)

			Private ReadOnly map As java.util.concurrent.ConcurrentMap(Of WeakEntry(Of T), WeakEntry(Of T))
			Private ReadOnly stale As ReferenceQueue(Of T)

			Public Sub New()
				Me.map = New ConcurrentDictionary(Of )
				Me.stale = New ReferenceQueue(Of )
			End Sub

			''' <summary>
			''' Get the existing interned element.
			''' This method returns null if no element is interned.
			''' </summary>
			''' <param name="elem"> element to look up </param>
			''' <returns> the interned element </returns>
			Public Overridable Function [get](ByVal elem As T) As T
				If elem Is Nothing Then Throw New NullPointerException
				expungeStaleElements()

				Dim value As WeakEntry(Of T) = map.get(New WeakEntry(Of T)(elem))
				If value IsNot Nothing Then
					Dim res As T = value.get()
					If res IsNot Nothing Then Return res
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Interns the element.
			''' Always returns non-null element, matching the one in the intern set.
			''' Under the race against another add(), it can return <i>different</i>
			''' element, if another thread beats us to interning it.
			''' </summary>
			''' <param name="elem"> element to add </param>
			''' <returns> element that was actually added </returns>
			Public Overridable Function add(ByVal elem As T) As T
				If elem Is Nothing Then Throw New NullPointerException

				' Playing double race here, and so spinloop is required.
				' First race is with two concurrent updaters.
				' Second race is with GC purging weak ref under our feet.
				' Hopefully, we almost always end up with a single pass.
				Dim interned As T
				Dim e As New WeakEntry(Of T)(elem, stale)
				Do
					expungeStaleElements()
					Dim exist As WeakEntry(Of T) = map.putIfAbsent(e, e)
					interned = If(exist Is Nothing, elem, exist.get())
				Loop While interned Is Nothing
				Return interned
			End Function

			Private Sub expungeStaleElements()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim reference As Reference(Of ? As T)
				reference = stale.poll()
				Do While reference IsNot Nothing
					map.remove(reference)
					reference = stale.poll()
				Loop
			End Sub

			Private Class WeakEntry(Of T)
				Inherits WeakReference(Of T)

				Public ReadOnly hashcode_Renamed As Integer

				Public Sub New(ByVal key As T, ByVal queue As ReferenceQueue(Of T))
					MyBase.New(key, queue)
					hashcode_Renamed = key.GetHashCode()
				End Sub

				Public Sub New(ByVal key As T)
					MyBase.New(key)
					hashcode_Renamed = key.GetHashCode()
				End Sub

				Public Overrides Function Equals(ByVal obj As Object) As Boolean
					If TypeOf obj Is WeakEntry Then
						Dim that As Object = CType(obj, WeakEntry).get()
						Dim mine As Object = get()
						Return If(that Is Nothing OrElse mine Is Nothing, (Me Is obj), mine.Equals(that))
					End If
					Return False
				End Function

				Public Overrides Function GetHashCode() As Integer
					Return hashcode_Renamed
				End Function

			End Class
		End Class

	End Class

End Namespace