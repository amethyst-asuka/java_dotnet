Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The symbolic, non-executable form of a method handle's invocation semantics.
	''' It consists of a series of names.
	''' The first N (N=arity) names are parameters,
	''' while any remaining names are temporary values.
	''' Each temporary specifies the application of a function to some arguments.
	''' The functions are method handles, while the arguments are mixes of
	''' constant values and local names.
	''' The result of the lambda is defined as one of the names, often the last one.
	''' <p>
	''' Here is an approximate grammar:
	''' <blockquote><pre>{@code
	''' LambdaForm = "(" ArgName* ")=>{" TempName* Result "}"
	''' ArgName = "a" N ":" T
	''' TempName = "t" N ":" T "=" Function "(" Argument* ");"
	''' Function = ConstantValue
	''' Argument = NameRef | ConstantValue
	''' Result = NameRef | "void"
	''' NameRef = "a" N | "t" N
	''' N = (any whole number)
	''' T = "L" | "I" | "J" | "F" | "D" | "V"
	''' }</pre></blockquote>
	''' Names are numbered consecutively from left to right starting at zero.
	''' (The letters are merely a taste of syntax sugar.)
	''' Thus, the first temporary (if any) is always numbered N (where N=arity).
	''' Every occurrence of a name reference in an argument list must refer to
	''' a name previously defined within the same lambda.
	''' A lambda has a void result if and only if its result index is -1.
	''' If a temporary has the type "V", it cannot be the subject of a NameRef,
	''' even though possesses a number.
	''' Note that all reference types are erased to "L", which stands for {@code Object}.
	''' All subword types (boolean, byte, short, char) are erased to "I" which is {@code int}.
	''' The other types stand for the usual primitive types.
	''' <p>
	''' Function invocation closely follows the static rules of the Java verifier.
	''' Arguments and return values must exactly match when their "Name" types are
	''' considered.
	''' Conversions are allowed only if they do not change the erased type.
	''' <ul>
	''' <li>L = Object: casts are used freely to convert into and out of reference types
	''' <li>I = int: subword types are forcibly narrowed when passed as arguments (see {@code explicitCastArguments})
	''' <li>J = long: no implicit conversions
	''' <li>F = float: no implicit conversions
	''' <li>D = double: no implicit conversions
	''' <li>V = void: a function result may be void if and only if its Name is of type "V"
	''' </ul>
	''' Although implicit conversions are not allowed, explicit ones can easily be
	''' encoded by using temporary expressions which call type-transformed identity functions.
	''' <p>
	''' Examples:
	''' <blockquote><pre>{@code
	''' (a0:J)=>{ a0 }
	'''     == identity(long)
	''' (a0:I)=>{ t1:V = System.out#println(a0); void }
	'''     == System.out#println(int)
	''' (a0:L)=>{ t1:V = System.out#println(a0); a0 }
	'''     == identity, with printing side-effect
	''' (a0:L, a1:L)=>{ t2:L = BoundMethodHandle#argument(a0);
	'''                 t3:L = BoundMethodHandle#target(a0);
	'''                 t4:L = MethodHandle#invoke(t3, t2, a1); t4 }
	'''     == general invoker for unary insertArgument combination
	''' (a0:L, a1:L)=>{ t2:L = FilterMethodHandle#filter(a0);
	'''                 t3:L = MethodHandle#invoke(t2, a1);
	'''                 t4:L = FilterMethodHandle#target(a0);
	'''                 t5:L = MethodHandle#invoke(t4, t3); t5 }
	'''     == general invoker for unary filterArgument combination
	''' (a0:L, a1:L)=>{ ...(same as previous example)...
	'''                 t5:L = MethodHandle#invoke(t4, t3, a1); t5 }
	'''     == general invoker for unary/unary foldArgument combination
	''' (a0:L, a1:I)=>{ t2:I = identity(long).asType((int)->long)(a1); t2 }
	'''     == invoker for identity method handle which performs i2l
	''' (a0:L, a1:L)=>{ t2:L = BoundMethodHandle#argument(a0);
	'''                 t3:L = Class#cast(t2,a1); t3 }
	'''     == invoker for identity method handle which performs cast
	''' }</pre></blockquote>
	''' <p>
	''' @author John Rose, JSR 292 EG
	''' </summary>
	Friend Class LambdaForm
		Friend ReadOnly arity_Renamed As Integer
		Friend ReadOnly result As Integer
		Friend ReadOnly forceInline As Boolean
		Friend ReadOnly customized As MethodHandle
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend ReadOnly names As Name()
		Friend ReadOnly debugName As String
		Friend vmentry As MemberName ' low-level behavior, or null if not yet prepared
		Private isCompiled As Boolean

		' Either a LambdaForm cache (managed by LambdaFormEditor) or a link to uncustomized version (for customized LF)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend transformCache As Object

		Public Const VOID_RESULT As Integer = -1, LAST_RESULT As Integer = -2

		Friend Enum BasicType
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			L_TYPE("L"c, Object.class, sun.invoke.util.Wrapper.OBJECT), ' all reference types
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			I_TYPE("I"c, int.class, sun.invoke.util.Wrapper.INT),
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			J_TYPE("J"c, long.class, sun.invoke.util.Wrapper.LONG),
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			F_TYPE("F"c, float.class, sun.invoke.util.Wrapper.FLOAT),
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			D_TYPE("D"c, double.class, sun.invoke.util.Wrapper.DOUBLE), ' all primitive types
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			V_TYPE("V"c, void.class, sun.invoke.util.Wrapper.VOID); ' not valid in all contexts

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			static final BasicType[] ALL_TYPES = BasicType.values();
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			static final BasicType() ARG_TYPES = java.util.Arrays.copyOf(ALL_TYPES, ALL_TYPES.length-1);

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			static final int ARG_TYPE_LIMIT = ARG_TYPES.length;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			static final int TYPE_LIMIT = ALL_TYPES.length;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final char btChar;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final Class btClass;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final sun.invoke.util.Wrapper btWrapper;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private BasicType(char btChar, Class btClass, sun.invoke.util.Wrapper wrapper)
	'		{
	'			Me.btChar = btChar;
	'			Me.btClass = btClass;
	'			Me.btWrapper = wrapper;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			char basicTypeChar()
	'		{
	'			Return btChar;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			Class basicTypeClass()
	'		{
	'			Return btClass;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			sun.invoke.util.Wrapper basicTypeWrapper()
	'		{
	'			Return btWrapper;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			int basicTypeSlots()
	'		{
	'			Return btWrapper.stackSlots();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType basicType(byte type)
	'		{
	'			Return ALL_TYPES[type];
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType basicType(char type)
	'		{
	'			switch (type)
	'			{
	'				case "L"c:
	'					Return L_TYPE;
	'				case "I"c:
	'					Return I_TYPE;
	'				case "J"c:
	'					Return J_TYPE;
	'				case "F"c:
	'					Return F_TYPE;
	'				case "D"c:
	'					Return D_TYPE;
	'				case "V"c:
	'					Return V_TYPE;
	'				' all subword types are represented as ints
	'				case "Z"c:
	'				case "B"c:
	'				case "S"c:
	'				case "C"c:
	'					Return I_TYPE;
	'				default:
	'					throw newInternalError("Unknown type char: '"+type+"'");
	'			}
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType basicType(sun.invoke.util.Wrapper type)
	'		{
	'			char c = type.basicTypeChar();
	'			Return basicType(c);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType basicType(Class type)
	'		{
	'			if (!type.isPrimitive())
	'				Return L_TYPE;
	'			Return basicType(Wrapper.forPrimitiveType(type));
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static char basicTypeChar(Class type)
	'		{
	'			Return basicType(type).btChar;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType[] basicTypes(java.util.List(Of Class) types)
	'		{
	'			BasicType[] btypes = New BasicType[types.size()];
	'			for (int i = 0; i < btypes.length; i += 1)
	'			{
	'				btypes[i] = basicType(types.get(i));
	'			}
	'			Return btypes;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static BasicType[] basicTypes(String types)
	'		{
	'			BasicType[] btypes = New BasicType[types.length()];
	'			for (int i = 0; i < btypes.length; i += 1)
	'			{
	'				btypes[i] = basicType(types.charAt(i));
	'			}
	'			Return btypes;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static byte[] basicTypesOrd(BasicType[] btypes)
	'		{
	'			byte[] ords = New byte[btypes.length];
	'			for (int i = 0; i < btypes.length; i += 1)
	'			{
	'				ords[i] = (byte)btypes[i].ordinal();
	'			}
	'			Return ords;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static boolean isBasicTypeChar(char c)
	'		{
	'			Return "LIJFDV".indexOf(c) >= 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static boolean isArgBasicTypeChar(char c)
	'		{
	'			Return "LIJFD".indexOf(c) >= 0;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			static LambdaForm()
	'		{
	'			assert(checkBasicType());
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private static boolean checkBasicType()
	'		{
	'			for (int i = 0; i < ARG_TYPE_LIMIT; i += 1)
	'			{
	'				assert ARG_TYPES[i].ordinal() == i;
	'				assert ARG_TYPES[i] == ALL_TYPES[i];
	'			}
	'			for (int i = 0; i < TYPE_LIMIT; i += 1)
	'			{
	'				assert ALL_TYPES[i].ordinal() == i;
	'			}
	'			assert ALL_TYPES[TYPE_LIMIT - 1] == V_TYPE;
	'			assert !Arrays.asList(ARG_TYPES).contains(V_TYPE);
	'			Return True;
	'		}
		End Enum

		Friend Sub New(ByVal debugName As String, ByVal arity As Integer, ByVal names As Name(), ByVal result As Integer)
			Me.New(debugName, arity, names, result, True, Nothing) 'customized= - forceInline=
		End Sub
		Friend Sub New(ByVal debugName As String, ByVal arity As Integer, ByVal names As Name(), ByVal result As Integer, ByVal forceInline As Boolean, ByVal customized As MethodHandle)
			assert(namesOK(arity, names))
			Me.arity_Renamed = arity
			Me.result = fixResult(result, names)
			Me.names = names.clone()
			Me.debugName = fixDebugName(debugName)
			Me.forceInline = forceInline
			Me.customized = customized
			Dim maxOutArity As Integer = normalize()
			If maxOutArity > MethodType.MAX_MH_INVOKER_ARITY Then
				' Cannot use LF interpreter on very high arity expressions.
				assert(maxOutArity <= MethodType.MAX_JVM_ARITY)
				compileToBytecode()
			End If
		End Sub
		Friend Sub New(ByVal debugName As String, ByVal arity As Integer, ByVal names As Name())
			Me.New(debugName, arity, names, LAST_RESULT, True, Nothing) 'customized= - forceInline=
		End Sub
		Friend Sub New(ByVal debugName As String, ByVal arity As Integer, ByVal names As Name(), ByVal forceInline As Boolean)
			Me.New(debugName, arity, names, LAST_RESULT, forceInline, Nothing) 'customized=
		End Sub
		Friend Sub New(ByVal debugName As String, ByVal formals As Name(), ByVal temps As Name(), ByVal result As Name)
			Me.New(debugName, formals.Length, buildNames(formals, temps, result), LAST_RESULT, True, Nothing) 'customized= - forceInline=
		End Sub
		Friend Sub New(ByVal debugName As String, ByVal formals As Name(), ByVal temps As Name(), ByVal result As Name, ByVal forceInline As Boolean)
			Me.New(debugName, formals.Length, buildNames(formals, temps, result), LAST_RESULT, forceInline, Nothing) 'customized=
		End Sub

		Private Shared Function buildNames(ByVal formals As Name(), ByVal temps As Name(), ByVal result As Name) As Name()
			Dim arity As Integer = formals.Length
			Dim length As Integer = arity + temps.Length + (If(result Is Nothing, 0, 1))
			Dim names As Name() = java.util.Arrays.copyOf(formals, length)
			Array.Copy(temps, 0, names, arity, temps.Length)
			If result IsNot Nothing Then names(length - 1) = result
			Return names
		End Function

		Private Sub New(ByVal sig As String)
			' Make a blank lambda form, which returns a constant zero or null.
			' It is used as a template for managing the invocation of similar forms that are non-empty.
			' Called only from getPreparedForm.
			assert(isValidSignature(sig))
			Me.arity_Renamed = signatureArity(sig)
			Me.result = (If(signatureReturn(sig) = V_TYPE, -1, arity_Renamed))
			Me.names = buildEmptyNames(arity_Renamed, sig)
			Me.debugName = "LF.zero"
			Me.forceInline = True
			Me.customized = Nothing
			assert(nameRefsAreLegal())
			assert(empty)
			assert(sig.Equals(basicTypeSignature())) : sig & " != " & basicTypeSignature()
		End Sub

		Private Shared Function buildEmptyNames(ByVal arity As Integer, ByVal basicTypeSignature As String) As Name()
			assert(isValidSignature(basicTypeSignature))
			Dim resultPos As Integer = arity + 1 ' skip '_'
			If arity < 0 OrElse basicTypeSignature.length() <> resultPos+1 Then Throw New IllegalArgumentException("bad arity for " & basicTypeSignature)
			Dim numRes As Integer = (If(basicType(basicTypeSignature.Chars(resultPos)) = V_TYPE, 0, 1))
			Dim names As Name() = arguments(numRes, basicTypeSignature.Substring(0, arity))
			For i As Integer = 0 To numRes - 1
				Dim zero As New Name(constantZero(basicType(basicTypeSignature.Chars(resultPos + i))))
				names(arity + i) = zero.newIndex(arity + i)
			Next i
			Return names
		End Function

		Private Shared Function fixResult(ByVal result As Integer, ByVal names As Name()) As Integer
			If result = LAST_RESULT Then result = names.Length - 1 ' might still be void
			If result >= 0 AndAlso names(result).type_Renamed = V_TYPE Then result = VOID_RESULT
			Return result
		End Function

		Private Shared Function fixDebugName(ByVal debugName As String) As String
			If DEBUG_NAME_COUNTERS IsNot Nothing Then
				Dim under As Integer = debugName.IndexOf("_"c)
				Dim length As Integer = debugName.length()
				If under < 0 Then under = length
				Dim debugNameStem As String = debugName.Substring(0, under)
				Dim ctr As Integer?
				SyncLock DEBUG_NAME_COUNTERS
					ctr = DEBUG_NAME_COUNTERS(debugNameStem)
					If ctr Is Nothing Then ctr = 0
					DEBUG_NAME_COUNTERS(debugNameStem) = ctr+1
				End SyncLock
				Dim buf As New StringBuilder(debugNameStem)
				buf.append("_"c)
				Dim leadingZero As Integer = buf.length()
				buf.append(CInt(ctr))
				For i As Integer = buf.length() - leadingZero To 2
					buf.insert(leadingZero, "0"c)
				Next i
				If under < length Then
					under += 1 ' skip "_"
					Do While under < length AndAlso Char.IsDigit(debugName.Chars(under))
						under += 1
					Loop
					If under < length AndAlso debugName.Chars(under) = "_"c Then under += 1
					If under < length Then buf.append("_"c).append(debugName, under, length)
				End If
				Return buf.ToString()
			End If
			Return debugName
		End Function

		Private Shared Function namesOK(ByVal arity As Integer, ByVal names As Name()) As Boolean
			For i As Integer = 0 To names.Length - 1
				Dim n As Name = names(i)
				assert(n IsNot Nothing) : "n is null"
				If i < arity Then
					assert(n.param) : n & " is not param at " & i
				Else
					assert((Not n.param)) : n & " is param at " & i
				End If
			Next i
			Return True
		End Function

		''' <summary>
		''' Customize LambdaForm for a particular MethodHandle </summary>
		Friend Overridable Function customize(ByVal mh As MethodHandle) As LambdaForm
			Dim customForm As New LambdaForm(debugName, arity_Renamed, names, result, forceInline, mh)
			If COMPILE_THRESHOLD > 0 AndAlso isCompiled Then customForm.compileToBytecode()
			customForm.transformCache = Me ' LambdaFormEditor should always use uncustomized form.
			Return customForm
		End Function

		''' <summary>
		''' Get uncustomized flavor of the LambdaForm </summary>
		Friend Overridable Function uncustomize() As LambdaForm
			If customized Is Nothing Then Return Me
			assert(transformCache IsNot Nothing) ' Customized LambdaForm should always has a link to uncustomized version.
			Dim uncustomizedForm As LambdaForm = CType(transformCache, LambdaForm)
			If COMPILE_THRESHOLD > 0 AndAlso isCompiled Then uncustomizedForm.compileToBytecode()
			Return uncustomizedForm
		End Function

		''' <summary>
		''' Renumber and/or replace params so that they are interned and canonically numbered. </summary>
		'''  <returns> maximum argument list length among the names (since we have to pass over them anyway) </returns>
		Private Function normalize() As Integer
			Dim oldNames As Name() = Nothing
			Dim maxOutArity As Integer = 0
			Dim changesStart As Integer = 0
			For i As Integer = 0 To names.Length - 1
				Dim n As Name = names(i)
				If Not n.initIndex(i) Then
					If oldNames Is Nothing Then
						oldNames = names.clone()
						changesStart = i
					End If
					names(i) = n.cloneWithIndex(i)
				End If
				If n.arguments IsNot Nothing AndAlso maxOutArity < n.arguments.Length Then maxOutArity = n.arguments.Length
			Next i
			If oldNames IsNot Nothing Then
				Dim startFixing As Integer = arity_Renamed
				If startFixing <= changesStart Then startFixing = changesStart+1
				For i As Integer = startFixing To names.Length - 1
					Dim fixed As Name = names(i).replaceNames(oldNames, names, changesStart, i)
					names(i) = fixed.newIndex(i)
				Next i
			End If
			assert(nameRefsAreLegal())
			Dim maxInterned As Integer = Math.Min(arity_Renamed, INTERNED_ARGUMENT_LIMIT)
			Dim needIntern As Boolean = False
			For i As Integer = 0 To maxInterned - 1
				Dim n As Name = names(i), n2 As Name = internArgument(n)
				If n IsNot n2 Then
					names(i) = n2
					needIntern = True
				End If
			Next i
			If needIntern Then
				For i As Integer = arity To names.Length - 1
					names(i).internArguments()
				Next i
			End If
			assert(nameRefsAreLegal())
			Return maxOutArity
		End Function

		''' <summary>
		''' Check that all embedded Name references are localizable to this lambda,
		''' and are properly ordered after their corresponding definitions.
		''' <p>
		''' Note that a Name can be local to multiple lambdas, as long as
		''' it possesses the same index in each use site.
		''' This allows Name references to be freely reused to construct
		''' fresh lambdas, without confusion.
		''' </summary>
		Friend Overridable Function nameRefsAreLegal() As Boolean
			assert(arity_Renamed >= 0 AndAlso arity_Renamed <= names.Length)
			assert(result >= -1 AndAlso result < names.Length)
			' Do all names possess an index consistent with their local definition order?
			For i As Integer = 0 To arity_Renamed - 1
				Dim n As Name = names(i)
				assert(n.index() = i) : java.util.Arrays.asList(n.index(), i)
				assert(n.param)
			Next i
			' Also, do all local name references
			For i As Integer = arity To names.Length - 1
				Dim n As Name = names(i)
				assert(n.index() = i)
				For Each arg As Object In n.arguments
					If TypeOf arg Is Name Then
						Dim n2 As Name = CType(arg, Name)
						Dim i2 As Integer = n2.index
						assert(0 <= i2 AndAlso i2 < names.Length) : n.debugString() & ": 0 <= i2 && i2 < names.length: 0 <= " & i2 & " < " & names.Length
						assert(names(i2) Is n2) : java.util.Arrays.asList("-1-", i, "-2-", n.debugString(), "-3-", i2, "-4-", n2.debugString(), "-5-", names(i2).debugString(), "-6-", Me)
						assert(i2 < i) ' ref must come after def!
					End If
				Next arg
			Next i
			Return True
		End Function

		''' <summary>
		''' Invoke this form on the given arguments. </summary>
		' final Object invoke(Object... args) throws Throwable {
		'     // NYI: fit this into the fast path?
		'     return interpretWithArguments(args);
		' }

		''' <summary>
		''' Report the return type. </summary>
		Friend Overridable Function returnType() As BasicType
			If result < 0 Then Return V_TYPE
			Dim n As Name = names(result)
			Return n.type_Renamed
		End Function

		''' <summary>
		''' Report the N-th argument type. </summary>
		Friend Overridable Function parameterType(ByVal n As Integer) As BasicType
			Return parameter(n).type_Renamed
		End Function

		''' <summary>
		''' Report the N-th argument name. </summary>
		Friend Overridable Function parameter(ByVal n As Integer) As Name
			assert(n < arity_Renamed)
			Dim param As Name = names(n)
			assert(param.param)
			Return param
		End Function

		''' <summary>
		''' Report the N-th argument type constraint. </summary>
		Friend Overridable Function parameterConstraint(ByVal n As Integer) As Object
			Return parameter(n).constraint
		End Function

		''' <summary>
		''' Report the arity. </summary>
		Friend Overridable Function arity() As Integer
			Return arity_Renamed
		End Function

		''' <summary>
		''' Report the number of expressions (non-parameter names). </summary>
		Friend Overridable Function expressionCount() As Integer
			Return names.Length - arity_Renamed
		End Function

		''' <summary>
		''' Return the method type corresponding to my basic type signature. </summary>
		Friend Overridable Function methodType() As MethodType
			Return signatureType(basicTypeSignature())
		End Function
		''' <summary>
		''' Return ABC_Z, where the ABC are parameter type characters, and Z is the return type character. </summary>
		Friend Function basicTypeSignature() As String
			Dim buf As New StringBuilder(arity() + 3)
			Dim i As Integer = 0
			Dim a As Integer = arity()
			Do While i < a
				buf.append(parameterType(i).basicTypeChar())
				i += 1
			Loop
			Return buf.append("_"c).append(returnType().basicTypeChar()).ToString()
		End Function
		Friend Shared Function signatureArity(ByVal sig As String) As Integer
			assert(isValidSignature(sig))
			Return sig.IndexOf("_"c)
		End Function
		Friend Shared Function signatureReturn(ByVal sig As String) As BasicType
			Return basicType(sig.Chars(signatureArity(sig) + 1))
		End Function
		Friend Shared Function isValidSignature(ByVal sig As String) As Boolean
			Dim arity As Integer = sig.IndexOf("_"c)
			If arity < 0 Then ' must be of the form *_* Return False
			Dim siglen As Integer = sig.length()
			If siglen <> arity + 2 Then ' *_X Return False
			For i As Integer = 0 To siglen - 1
				If i = arity Then ' skip '_' Continue For
				Dim c As Char = sig.Chars(i)
				If c = "V"c Then Return (i = siglen - 1 AndAlso arity = siglen - 2)
				If Not isArgBasicTypeChar(c) Then ' must be [LIJFD] Return False
			Next i
			Return True ' [LIJFD]*_[LIJFDV]
		End Function
		Friend Shared Function signatureType(ByVal sig As String) As MethodType
			Dim ptypes As Class() = New [Class](signatureArity(sig) - 1){}
			For i As Integer = 0 To ptypes.Length - 1
				ptypes(i) = basicType(sig.Chars(i)).btClass
			Next i
			Dim rtype As Class = signatureReturn(sig).btClass
			Return MethodType.methodType(rtype, ptypes)
		End Function

	'    
	'     * Code generation issues:
	'     *
	'     * Compiled LFs should be reusable in general.
	'     * The biggest issue is how to decide when to pull a name into
	'     * the bytecode, versus loading a reified form from the MH data.
	'     *
	'     * For example, an asType wrapper may require execution of a cast
	'     * after a call to a MH.  The target type of the cast can be placed
	'     * as a constant in the LF itself.  This will force the cast type
	'     * to be compiled into the bytecodes and native code for the MH.
	'     * Or, the target type of the cast can be erased in the LF, and
	'     * loaded from the MH data.  (Later on, if the MH as a whole is
	'     * inlined, the data will flow into the inlined instance of the LF,
	'     * as a constant, and the end result will be an optimal cast.)
	'     *
	'     * This erasure of cast types can be done with any use of
	'     * reference types.  It can also be done with whole method
	'     * handles.  Erasing a method handle might leave behind
	'     * LF code that executes correctly for any MH of a given
	'     * type, and load the required MH from the enclosing MH's data.
	'     * Or, the erasure might even erase the expected MT.
	'     *
	'     * Also, for direct MHs, the MemberName of the target
	'     * could be erased, and loaded from the containing direct MH.
	'     * As a simple case, a LF for all int-valued non-static
	'     * field getters would perform a cast on its input argument
	'     * (to non-constant base type derived from the MemberName)
	'     * and load an integer value from the input object
	'     * (at a non-constant offset also derived from the MemberName).
	'     * Such MN-erased LFs would be inlinable back to optimized
	'     * code, whenever a constant enclosing DMH is available
	'     * to supply a constant MN from its data.
	'     *
	'     * The main problem here is to keep LFs reasonably generic,
	'     * while ensuring that hot spots will inline good instances.
	'     * "Reasonably generic" means that we don't end up with
	'     * repeated versions of bytecode or machine code that do
	'     * not differ in their optimized form.  Repeated versions
	'     * of machine would have the undesirable overheads of
	'     * (a) redundant compilation work and (b) extra I$ pressure.
	'     * To control repeated versions, we need to be ready to
	'     * erase details from LFs and move them into MH data,
	'     * whevener those details are not relevant to significant
	'     * optimization.  "Significant" means optimization of
	'     * code that is actually hot.
	'     *
	'     * Achieving this may require dynamic splitting of MHs, by replacing
	'     * a generic LF with a more specialized one, on the same MH,
	'     * if (a) the MH is frequently executed and (b) the MH cannot
	'     * be inlined into a containing caller, such as an invokedynamic.
	'     *
	'     * Compiled LFs that are no longer used should be GC-able.
	'     * If they contain non-BCP references, they should be properly
	'     * interlinked with the class loader(s) that their embedded types
	'     * depend on.  This probably means that reusable compiled LFs
	'     * will be tabulated (indexed) on relevant class loaders,
	'     * or else that the tables that cache them will have weak links.
	'     

		''' <summary>
		''' Make this LF directly executable, as part of a MethodHandle.
		''' Invariant:  Every MH which is invoked must prepare its LF
		''' before invocation.
		''' (In principle, the JVM could do this very lazily,
		''' as a sort of pre-invocation linkage step.)
		''' </summary>
		Public Overridable Sub prepare()
			If COMPILE_THRESHOLD = 0 AndAlso (Not isCompiled) Then compileToBytecode()
			If Me.vmentry IsNot Nothing Then Return
			Dim prep As LambdaForm = getPreparedForm(basicTypeSignature())
			Me.vmentry = prep.vmentry
			' TO DO: Maybe add invokeGeneric, invokeWithArguments
		End Sub

		''' <summary>
		''' Generate optimizable bytecode for this form. </summary>
		Friend Overridable Function compileToBytecode() As MemberName
			If vmentry IsNot Nothing AndAlso isCompiled Then Return vmentry ' already compiled somehow
			Dim invokerType As MethodType = methodType()
			assert(vmentry Is Nothing OrElse vmentry.methodType.basicType().Equals(invokerType))
			Try
				vmentry = InvokerBytecodeGenerator.generateCustomizedCode(Me, invokerType)
				If TRACE_INTERPRETER Then traceInterpreter("compileToBytecode", Me)
				isCompiled = True
				Return vmentry
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch Error Or Exception ex
				Throw newInternalError(Me.ToString(), ex)
			End Try
		End Function

		Private Shared Sub computeInitialPreparedForms()
			' Find all predefined invokers and associate them with canonical empty lambda forms.
			For Each m As MemberName In MemberName.factory.getMethods(GetType(LambdaForm), False, Nothing, Nothing, Nothing)
				If (Not m.static) OrElse (Not m.package) Then Continue For
				Dim mt As MethodType = m.methodType
				If mt.parameterCount() > 0 AndAlso mt.parameterType(0) Is GetType(MethodHandle) AndAlso m.name.StartsWith("interpret_") Then
					Dim sig As String = basicTypeSignature(mt)
					assert(m.name.Equals("interpret" & sig.Substring(sig.IndexOf("_"c))))
					Dim form As New LambdaForm(sig)
					form.vmentry = m
					form = mt.form().cachedLambdaFormorm(MethodTypeForm.LF_INTERPRET, form)
				End If
			Next m
		End Sub

		' Set this false to disable use of the interpret_L methods defined in this file.
		Private Const USE_PREDEFINED_INTERPRET_METHODS As Boolean = True

		' The following are predefined exact invokers.  The system must build
		' a separate invoker for each distinct signature.
		Friend Shared Function interpret_L(ByVal mh As MethodHandle) As Object
			Dim av As Object() = {mh}
			Dim sig As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			assert(argumentTypesMatch(sig = "L_L", av))
			Dim res As Object = mh.form.interpretWithArguments(av)
			assert(returnTypesMatch(sig, av, res))
			Return res
		End Function
		Friend Shared Function interpret_L(ByVal mh As MethodHandle, ByVal x1 As Object) As Object
			Dim av As Object() = {mh, x1}
			Dim sig As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			assert(argumentTypesMatch(sig = "LL_L", av))
			Dim res As Object = mh.form.interpretWithArguments(av)
			assert(returnTypesMatch(sig, av, res))
			Return res
		End Function
		Friend Shared Function interpret_L(ByVal mh As MethodHandle, ByVal x1 As Object, ByVal x2 As Object) As Object
			Dim av As Object() = {mh, x1, x2}
			Dim sig As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			assert(argumentTypesMatch(sig = "LLL_L", av))
			Dim res As Object = mh.form.interpretWithArguments(av)
			assert(returnTypesMatch(sig, av, res))
			Return res
		End Function
		Private Shared Function getPreparedForm(ByVal sig As String) As LambdaForm
			Dim mtype As MethodType = signatureType(sig)
			Dim prep As LambdaForm = mtype.form().cachedLambdaForm(MethodTypeForm.LF_INTERPRET)
			If prep IsNot Nothing Then Return prep
			assert(isValidSignature(sig))
			prep = New LambdaForm(sig)
			prep.vmentry = InvokerBytecodeGenerator.generateLambdaFormInterpreterEntryPoint(sig)
			Return mtype.form().cachedLambdaFormorm(MethodTypeForm.LF_INTERPRET, prep)
		End Function

		' The next few routines are called only from assert expressions
		' They verify that the built-in invokers process the correct raw data types.
		Private Shared Function argumentTypesMatch(ByVal sig As String, ByVal av As Object()) As Boolean
			Dim arity As Integer = signatureArity(sig)
			assert(av.Length = arity) : "av.length == arity: av.length=" & av.Length & ", arity=" & arity
			assert(TypeOf av(0) Is MethodHandle) : "av[0] not instace of MethodHandle: " & av(0)
			Dim mh As MethodHandle = CType(av(0), MethodHandle)
			Dim mt As MethodType = mh.type()
			assert(mt.parameterCount() = arity-1)
			For i As Integer = 0 To av.Length - 1
				Dim pt As Class = (If(i = 0, GetType(MethodHandle), mt.parameterType(i-1)))
				assert(valueMatches(basicType(sig.Chars(i)), pt, av(i)))
			Next i
			Return True
		End Function
		Private Shared Function valueMatches(ByVal tc As BasicType, ByVal type As Class, ByVal x As Object) As Boolean
			' The following line is needed because (...)void method handles can use non-void invokers
			If type Is GetType(void) Then ' can drop any kind of value tc = V_TYPE
			Debug.Assert(tc = basicType(type), tc & " == basicType(" & type & ")=" & basicType(type))
			Select Case tc
			Case BasicType.I_TYPE
				Debug.Assert(checkInt(type, x), "checkInt(" & type & "," & x & ")")
			Case BasicType.J_TYPE
				Debug.Assert(TypeOf x Is Long?, "instanceof Long: " & x)
			Case BasicType.F_TYPE
				Debug.Assert(TypeOf x Is Float, "instanceof Float: " & x)
			Case BasicType.D_TYPE
				Debug.Assert(TypeOf x Is Double?, "instanceof Double: " & x)
			Case BasicType.L_TYPE
				Debug.Assert(checkRef(type, x), "checkRef(" & type & "," & x & ")")
			Case BasicType.V_TYPE ' allow anything here; will be dropped
			Case Else
				assert(False)
			End Select
			Return True
		End Function
		Private Shared Function returnTypesMatch(ByVal sig As String, ByVal av As Object(), ByVal res As Object) As Boolean
			Dim mh As MethodHandle = CType(av(0), MethodHandle)
			Return valueMatches(signatureReturn(sig), mh.type().returnType(), res)
		End Function
		Private Shared Function checkInt(ByVal type As Class, ByVal x As Object) As Boolean
			assert(TypeOf x Is Integer?)
			If type Is GetType(Integer) Then Return True
			Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forBasicType(type)
			assert(w.subwordOrInt)
			Dim x1 As Object = sun.invoke.util.Wrapper.INT.wrap(w.wrap(x))
			Return x.Equals(x1)
		End Function
		Private Shared Function checkRef(ByVal type As Class, ByVal x As Object) As Boolean
			assert((Not type.primitive))
			If x Is Nothing Then Return True
			If type.interface Then Return True
			Return type.isInstance(x)
		End Function

		''' <summary>
		''' If the invocation count hits the threshold we spin bytecodes and call that subsequently. </summary>
		Private Shared ReadOnly COMPILE_THRESHOLD As Integer
		Shared Sub New()
			COMPILE_THRESHOLD = Math.Max(-1, MethodHandleStatics.COMPILE_THRESHOLD)
			For Each type As BasicType In BasicType.ARG_TYPES
				Dim ord As Integer = type.ordinal()
				For i As Integer = 0 To INTERNED_ARGUMENTS(ord).Length - 1
					INTERNED_ARGUMENTS(ord)(i) = New Name(i, type)
				Next i
			Next type
			If debugEnabled() Then
				DEBUG_NAME_COUNTERS = New Dictionary(Of )
			Else
				DEBUG_NAME_COUNTERS = Nothing
			End If
			createIdentityForms()
			If USE_PREDEFINED_INTERPRET_METHODS Then computeInitialPreparedForms()
			NamedFunction.initializeInvokers()
		End Sub
		Private invocationCounter As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function interpretWithArguments(ParamArray ByVal argumentValues As Object()) As Object
		''' <summary>
		''' Interpretively invoke this form on the given arguments. </summary>
			If TRACE_INTERPRETER Then Return interpretWithArgumentsTracing(argumentValues)
			checkInvocationCounter()
			assert(arityCheck(argumentValues))
			Dim values As Object() = java.util.Arrays.copyOf(argumentValues, names.Length)
			For i As Integer = argumentValues.Length To values.Length - 1
				values(i) = interpretName(names(i), values)
			Next i
			Dim rv As Object = If(result < 0, Nothing, values(result))
			assert(resultCheck(argumentValues, rv))
			Return rv
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function interpretName(ByVal name_Renamed As Name, ByVal values As Object()) As Object
		''' <summary>
		''' Evaluate a single Name within this form, applying its function to its arguments. </summary>
			If TRACE_INTERPRETER Then traceInterpreter("| interpretName", name_Renamed.debugString(), CType(Nothing, Object()))
			Dim arguments As Object() = java.util.Arrays.copyOf(name_Renamed.arguments, name_Renamed.arguments.Length, GetType(Object()))
			For i As Integer = 0 To arguments.Length - 1
				Dim a As Object = arguments(i)
				If TypeOf a Is Name Then
					Dim i2 As Integer = CType(a, Name).index()
					assert(names(i2) Is a)
					a = values(i2)
					arguments(i) = a
				End If
			Next i
			Return name_Renamed.function.invokeWithArguments(arguments)
		End Function

		Private Sub checkInvocationCounter()
			If COMPILE_THRESHOLD <> 0 AndAlso invocationCounter < COMPILE_THRESHOLD Then
				invocationCounter += 1 ' benign race
				If invocationCounter >= COMPILE_THRESHOLD Then compileToBytecode()
			End If
		End Sub
		Friend Overridable Function interpretWithArgumentsTracing(ParamArray ByVal argumentValues As Object()) As Object
			traceInterpreter("[ interpretWithArguments", Me, argumentValues)
			If invocationCounter < COMPILE_THRESHOLD Then
				Dim ctr As Integer = invocationCounter
				invocationCounter += 1
				traceInterpreter("| invocationCounter", ctr)
				If invocationCounter >= COMPILE_THRESHOLD Then compileToBytecode()
			End If
			Dim rval As Object
			Try
				assert(arityCheck(argumentValues))
				Dim values As Object() = java.util.Arrays.copyOf(argumentValues, names.Length)
				For i As Integer = argumentValues.Length To values.Length - 1
					values(i) = interpretName(names(i), values)
				Next i
				rval = If(result < 0, Nothing, values(result))
			Catch ex As Throwable
				traceInterpreter("] throw =>", ex)
				Throw ex
			End Try
			traceInterpreter("] return =>", rval)
			Return rval
		End Function

		Friend Shared Sub traceInterpreter(ByVal [event] As String, ByVal obj As Object, ParamArray ByVal args As Object())
			If TRACE_INTERPRETER Then Console.WriteLine("LFI: " & [event] & " " & (If(obj IsNot Nothing, obj, ""))+(If(args IsNot Nothing AndAlso args.Length <> 0, java.util.Arrays.asList(args), "")))
		End Sub
		Friend Shared Sub traceInterpreter(ByVal [event] As String, ByVal obj As Object)
			traceInterpreter([event], obj, CType(Nothing, Object()))
		End Sub
		Private Function arityCheck(ByVal argumentValues As Object()) As Boolean
			assert(argumentValues.Length = arity_Renamed) : arity_Renamed & "!=" & java.util.Arrays.asList(argumentValues) & ".length"
			' also check that the leading (receiver) argument is somehow bound to this LF:
			assert(TypeOf argumentValues(0) Is MethodHandle) : "not MH: " & argumentValues(0)
			Dim mh As MethodHandle = CType(argumentValues(0), MethodHandle)
			assert(mh.internalForm() Is Me)
			' note:  argument #0 could also be an interface wrapper, in the future
			argumentTypesMatch(basicTypeSignature(), argumentValues)
			Return True
		End Function
		Private Function resultCheck(ByVal argumentValues As Object(), ByVal result As Object) As Boolean
			Dim mh As MethodHandle = CType(argumentValues(0), MethodHandle)
			Dim mt As MethodType = mh.type()
			assert(valueMatches(returnType(), mt.returnType(), result))
			Return True
		End Function

		Private Property empty As Boolean
			Get
				If result < 0 Then
					Return (names.Length = arity_Renamed)
				ElseIf result = arity_Renamed AndAlso names.Length = arity_Renamed + 1 Then
					Return names(arity_Renamed).constantZero
				Else
					Return False
				End If
			End Get
		End Property

		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder(debugName & "=Lambda(")
			For i As Integer = 0 To names.Length - 1
				If i = arity_Renamed Then buf.append(")=>{")
				Dim n As Name = names(i)
				If i >= arity_Renamed Then buf.append(vbLf & "    ")
				buf.append(n.paramString())
				If i < arity_Renamed Then
					If i+1 < arity_Renamed Then buf.append(",")
					Continue For
				End If
				buf.append("=").append(n.exprString())
				buf.append(";")
			Next i
			If arity_Renamed = names.Length Then buf.append(")=>{")
			buf.append(If(result < 0, "void", names(result))).append("}")
			If TRACE_INTERPRETER Then
				' Extra verbosity:
				buf.append(":").append(basicTypeSignature())
				buf.append("/").append(vmentry)
			End If
			Return buf.ToString()
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return TypeOf obj Is LambdaForm AndAlso Equals(CType(obj, LambdaForm))
		End Function
		Public Overrides Function Equals(ByVal that As LambdaForm) As Boolean
			If Me.result <> that.result Then Return False
			Return java.util.Arrays.Equals(Me.names, that.names)
		End Function
		Public Overrides Function GetHashCode() As Integer
			Return result + 31 * java.util.Arrays.hashCode(names)
		End Function
		Friend Overridable Function editor() As LambdaFormEditor
			Return LambdaFormEditor.lambdaFormEditor(Me)
		End Function

		Friend Overridable Function contains(ByVal name_Renamed As Name) As Boolean
			Dim pos As Integer = name_Renamed.index()
			If pos >= 0 Then Return pos < names.Length AndAlso name_Renamed.Equals(names(pos))
			For i As Integer = arity To names.Length - 1
				If name_Renamed.Equals(names(i)) Then Return True
			Next i
			Return False
		End Function

		Friend Overridable Function addArguments(ByVal pos As Integer, ParamArray ByVal types As BasicType()) As LambdaForm
			' names array has MH in slot 0; skip it.
			Dim argpos As Integer = pos + 1
			assert(argpos <= arity_Renamed)
			Dim length As Integer = names.Length
			Dim inTypes As Integer = types.Length
			Dim names2 As Name() = java.util.Arrays.copyOf(names, length + inTypes)
			Dim arity2 As Integer = arity_Renamed + inTypes
			Dim result2 As Integer = result
			If result2 >= argpos Then result2 += inTypes
			' Note:  The LF constructor will rename names2[argpos...].
			' Make space for new arguments (shift temporaries).
			Array.Copy(names, argpos, names2, argpos + inTypes, length - argpos)
			For i As Integer = 0 To inTypes - 1
				names2(argpos + i) = New Name(types(i))
			Next i
			Return New LambdaForm(debugName, arity2, names2, result2)
		End Function

		Friend Overridable Function addArguments(ByVal pos As Integer, ByVal types As IList(Of [Class])) As LambdaForm
			Return addArguments(pos, basicTypes(types))
		End Function

		Friend Overridable Function permuteArguments(ByVal skip As Integer, ByVal reorder As Integer(), ByVal types As BasicType()) As LambdaForm
			' Note:  When inArg = reorder[outArg], outArg is fed by a copy of inArg.
			' The types are the types of the new (incoming) arguments.
			Dim length As Integer = names.Length
			Dim inTypes As Integer = types.Length
			Dim outArgs As Integer = reorder.Length
			assert(skip+outArgs = arity_Renamed)
			assert(permutedTypesMatch(reorder, types, names, skip))
			Dim pos As Integer = 0
			' skip trivial first part of reordering:
			Do While pos < outArgs AndAlso reorder(pos) = pos
				pos += 1
			Loop
			Dim names2 As Name() = New Name(length - outArgs + inTypes - 1){}
			Array.Copy(names, 0, names2, 0, skip+pos)
			' copy the body:
			Dim bodyLength As Integer = length - arity_Renamed
			Array.Copy(names, skip+outArgs, names2, skip+inTypes, bodyLength)
			Dim arity2 As Integer = names2.Length - bodyLength
			Dim result2 As Integer = result
			If result2 >= 0 Then
				If result2 < skip+outArgs Then
					' return the corresponding inArg
					result2 = reorder(result2-skip)
				Else
					result2 = result2 - outArgs + inTypes
				End If
			End If
			' rework names in the body:
			For j As Integer = pos To outArgs - 1
				Dim n As Name = names(skip+j)
				Dim i As Integer = reorder(j)
				' replace names[skip+j] by names2[skip+i]
				Dim n2 As Name = names2(skip+i)
				If n2 Is Nothing Then
						n2 = New Name(types(i))
						names2(skip+i) = n2
				Else
					assert(n2.type_Renamed = types(i))
				End If
				For k As Integer = arity2 To names2.Length - 1
					names2(k) = names2(k).replaceName(n, n2)
				Next k
			Next j
			' some names are unused, but must be filled in
			For i As Integer = skip+pos To arity2 - 1
				If names2(i) Is Nothing Then names2(i) = argument(i, types(i - skip))
			Next i
			For j As Integer = arity To names.Length - 1
				Dim i As Integer = j - arity_Renamed + arity2
				' replace names2[i] by names[j]
				Dim n As Name = names(j)
				Dim n2 As Name = names2(i)
				If n IsNot n2 Then
					For k As Integer = i+1 To names2.Length - 1
						names2(k) = names2(k).replaceName(n, n2)
					Next k
				End If
			Next j
			Return New LambdaForm(debugName, arity2, names2, result2)
		End Function

		Friend Shared Function permutedTypesMatch(ByVal reorder As Integer(), ByVal types As BasicType(), ByVal names As Name(), ByVal skip As Integer) As Boolean
			Dim inTypes As Integer = types.Length
			Dim outArgs As Integer = reorder.Length
			For i As Integer = 0 To outArgs - 1
				assert(names(skip+i).param)
				assert(names(skip+i).type_Renamed = types(reorder(i)))
			Next i
			Return True
		End Function

		Friend Class NamedFunction
			Friend ReadOnly member_Renamed As MemberName
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend resolvedHandle_Renamed As MethodHandle
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend invoker_Renamed As MethodHandle

			Friend Sub New(ByVal resolvedHandle As MethodHandle)
				Me.New(resolvedHandle.internalMemberName(), resolvedHandle)
			End Sub
			Friend Sub New(ByVal member As MemberName, ByVal resolvedHandle As MethodHandle)
				Me.member_Renamed = member
				Me.resolvedHandle_Renamed = resolvedHandle
				 ' The following assert is almost always correct, but will fail for corner cases, such as PrivateInvokeTest.
				 'assert(!isInvokeBasic());
			End Sub
			Friend Sub New(ByVal basicInvokerType As MethodType)
				assert(basicInvokerType Is basicInvokerType.basicType()) : basicInvokerType
				If basicInvokerType.parameterSlotCount() < MethodType.MAX_MH_INVOKER_ARITY Then
					Me.resolvedHandle_Renamed = basicInvokerType.invokers().basicInvoker()
					Me.member_Renamed = resolvedHandle_Renamed.internalMemberName()
				Else
					' necessary to pass BigArityTest
					Me.member_Renamed = Invokers.invokeBasicMethod(basicInvokerType)
				End If
				assert(invokeBasic)
			End Sub

			Private Property invokeBasic As Boolean
				Get
					Return member_Renamed IsNot Nothing AndAlso member_Renamed.methodHandleInvoke AndAlso "invokeBasic".Equals(member_Renamed.name)
				End Get
			End Property

			' The next 3 constructors are used to break circular dependencies on MH.invokeStatic, etc.
			' Any LambdaForm containing such a member is not interpretable.
			' This is OK, since all such LFs are prepared with special primitive vmentry points.
			' And even without the resolvedHandle, the name can still be compiled and optimized.
			Friend Sub New(ByVal method As Method)
				Me.New(New MemberName(method))
			End Sub
			Friend Sub New(ByVal field As Field)
				Me.New(New MemberName(field))
			End Sub
			Friend Sub New(ByVal member As MemberName)
				Me.member_Renamed = member
				Me.resolvedHandle_Renamed = Nothing
			End Sub

			Friend Overridable Function resolvedHandle() As MethodHandle
				If resolvedHandle_Renamed Is Nothing Then resolve()
				Return resolvedHandle_Renamed
			End Function

			Friend Overridable Sub resolve()
				resolvedHandle_Renamed = DirectMethodHandle.make(member_Renamed)
			End Sub

			Public Overrides Function Equals(ByVal other As Object) As Boolean
				If Me Is other Then Return True
				If other Is Nothing Then Return False
				If Not(TypeOf other Is NamedFunction) Then Return False
				Dim that As NamedFunction = CType(other, NamedFunction)
				Return Me.member_Renamed IsNot Nothing AndAlso Me.member_Renamed.Equals(that.member_Renamed)
			End Function

			Public Overrides Function GetHashCode() As Integer
				If member_Renamed IsNot Nothing Then Return member_Renamed.GetHashCode()
				Return MyBase.GetHashCode()
			End Function

			' Put the predefined NamedFunction invokers into the table.
			Friend Shared Sub initializeInvokers()
				For Each m As MemberName In MemberName.factory.getMethods(GetType(NamedFunction), False, Nothing, Nothing, Nothing)
					If (Not m.static) OrElse (Not m.package) Then Continue For
					Dim type As MethodType = m.methodType
					If type.Equals(INVOKER_METHOD_TYPE) AndAlso m.name.StartsWith("invoke_") Then
						Dim sig As String = m.name.Substring("invoke_".length())
						Dim arity As Integer = LambdaForm.signatureArity(sig)
						Dim srcType As MethodType = MethodType.genericMethodType(arity)
						If LambdaForm.signatureReturn(sig) = V_TYPE Then srcType = srcType.changeReturnType(GetType(void))
						Dim typeForm As MethodTypeForm = srcType.form()
						typeForm.cachedMethodHandledle(MethodTypeForm.MH_NF_INV, DirectMethodHandle.make(m))
					End If
				Next m
			End Sub

			' The following are predefined NamedFunction invokers.  The system must build
			' a separate invoker for each distinct signature.
			''' <summary>
			''' void return type invokers. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke__V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(0, GetType(void), mh, a))
				mh.invokeBasic()
				Return Nothing
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_L_V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(1, GetType(void), mh, a))
				mh.invokeBasic(a(0))
				Return Nothing
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LL_V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(2, GetType(void), mh, a))
				mh.invokeBasic(a(0), a(1))
				Return Nothing
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLL_V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(3, GetType(void), mh, a))
				mh.invokeBasic(a(0), a(1), a(2))
				Return Nothing
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLLL_V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(4, GetType(void), mh, a))
				mh.invokeBasic(a(0), a(1), a(2), a(3))
				Return Nothing
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLLLL_V(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(5, GetType(void), mh, a))
				mh.invokeBasic(a(0), a(1), a(2), a(3), a(4))
				Return Nothing
			End Function
			''' <summary>
			''' Object return type invokers. </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke__L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(0, mh, a))
				Return mh.invokeBasic()
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_L_L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(1, mh, a))
				Return mh.invokeBasic(a(0))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LL_L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(2, mh, a))
				Return mh.invokeBasic(a(0), a(1))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLL_L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(3, mh, a))
				Return mh.invokeBasic(a(0), a(1), a(2))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLLL_L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(4, mh, a))
				Return mh.invokeBasic(a(0), a(1), a(2), a(3))
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Shared Function invoke_LLLLL_L(ByVal mh As MethodHandle, ByVal a As Object()) As Object
				assert(arityCheck(5, mh, a))
				Return mh.invokeBasic(a(0), a(1), a(2), a(3), a(4))
			End Function
			Private Shared Function arityCheck(ByVal arity As Integer, ByVal mh As MethodHandle, ByVal a As Object()) As Boolean
				Return arityCheck(arity, GetType(Object), mh, a)
			End Function
			Private Shared Function arityCheck(ByVal arity As Integer, ByVal rtype As Class, ByVal mh As MethodHandle, ByVal a As Object()) As Boolean
				assert(a.Length = arity) : java.util.Arrays.asList(a.Length, arity)
				assert(mh.type().basicType() Is MethodType.genericMethodType(arity).changeReturnType(rtype)) : java.util.Arrays.asList(mh, rtype, arity)
				Dim member As MemberName = mh.internalMemberName()
				If member IsNot Nothing AndAlso member.name.Equals("invokeBasic") AndAlso member.methodHandleInvoke Then
					assert(arity > 0)
					assert(TypeOf a(0) Is MethodHandle)
					Dim mh2 As MethodHandle = CType(a(0), MethodHandle)
					assert(mh2.type().basicType() Is MethodType.genericMethodType(arity-1).changeReturnType(rtype)) : java.util.Arrays.asList(member, mh2, rtype, arity)
				End If
				Return True
			End Function

			Friend Shared ReadOnly INVOKER_METHOD_TYPE As MethodType = MethodType.methodType(GetType(Object), GetType(MethodHandle), GetType(Object()))

			Private Shared Function computeInvoker(ByVal typeForm As MethodTypeForm) As MethodHandle
				typeForm = typeForm.basicType().form() ' normalize to basic type
				Dim mh As MethodHandle = typeForm.cachedMethodHandle(MethodTypeForm.MH_NF_INV)
				If mh IsNot Nothing Then Return mh
				Dim invoker As MemberName = InvokerBytecodeGenerator.generateNamedFunctionInvoker(typeForm) ' this could take a while
				mh = DirectMethodHandle.make(invoker)
				Dim mh2 As MethodHandle = typeForm.cachedMethodHandle(MethodTypeForm.MH_NF_INV)
				If mh2 IsNot Nothing Then ' benign race Return mh2
				If Not mh.type().Equals(INVOKER_METHOD_TYPE) Then Throw newInternalError(mh.debugString())
				Return typeForm.cachedMethodHandledle(MethodTypeForm.MH_NF_INV, mh)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Overridable Function invokeWithArguments(ParamArray ByVal arguments As Object()) As Object
				' If we have a cached invoker, call it right away.
				' NOTE: The invoker always returns a reference value.
				If TRACE_INTERPRETER Then Return invokeWithArgumentsTracing(arguments)
				assert(checkArgumentTypes(arguments, methodType()))
				Return invoker().invokeBasic(resolvedHandle(), arguments)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend Overridable Function invokeWithArgumentsTracing(ByVal arguments As Object()) As Object
				Dim rval As Object
				Try
					traceInterpreter("[ call", Me, arguments)
					If invoker_Renamed Is Nothing Then
						traceInterpreter("| getInvoker", Me)
						invoker()
					End If
					If resolvedHandle_Renamed Is Nothing Then
						traceInterpreter("| resolve", Me)
						resolvedHandle()
					End If
					assert(checkArgumentTypes(arguments, methodType()))
					rval = invoker().invokeBasic(resolvedHandle(), arguments)
				Catch ex As Throwable
					traceInterpreter("] throw =>", ex)
					Throw ex
				End Try
				traceInterpreter("] return =>", rval)
				Return rval
			End Function

			Private Function invoker() As MethodHandle
				If invoker_Renamed IsNot Nothing Then Return invoker_Renamed
				' Get an invoker and cache it.
					invoker_Renamed = computeInvoker(methodType().form())
					Return invoker_Renamed
			End Function

			Private Shared Function checkArgumentTypes(ByVal arguments As Object(), ByVal methodType_Renamed As MethodType) As Boolean
				If True Then ' FIXME Return True
				Dim dstType As MethodType = methodType_Renamed.form().erasedType()
				Dim srcType As MethodType = dstType.basicType().wrap()
				Dim ptypes As Class() = New [Class](arguments.Length - 1){}
				For i As Integer = 0 To arguments.Length - 1
					Dim arg As Object = arguments(i)
					Dim ptype As Class = If(arg Is Nothing, GetType(Object), arg.GetType())
					' If the dest. type is a primitive we keep the
					' argument type.
					ptypes(i) = If(dstType.parameterType(i).primitive, ptype, GetType(Object))
				Next i
				Dim argType As MethodType = MethodType.methodType(srcType.returnType(), ptypes).wrap()
				assert(argType.isConvertibleTo(srcType)) : "wrong argument types: cannot convert " & argType & " to " & srcType
				Return True
			End Function

			Friend Overridable Function methodType() As MethodType
				If resolvedHandle_Renamed IsNot Nothing Then
					Return resolvedHandle_Renamed.type()
				Else
					' only for certain internal LFs during bootstrapping
					Return member_Renamed.invocationType
				End If
			End Function

			Friend Overridable Function member() As MemberName
				assert(assertMemberIsConsistent())
				Return member_Renamed
			End Function

			' Called only from assert.
			Private Function assertMemberIsConsistent() As Boolean
				If TypeOf resolvedHandle_Renamed Is DirectMethodHandle Then
					Dim m As MemberName = resolvedHandle_Renamed.internalMemberName()
					assert(m.Equals(member_Renamed))
				End If
				Return True
			End Function

			Friend Overridable Function memberDeclaringClassOrNull() As Class
				Return If(member_Renamed Is Nothing, Nothing, member_Renamed.declaringClass)
			End Function

			Friend Overridable Function returnType() As BasicType
				Return basicType(methodType().returnType())
			End Function

			Friend Overridable Function parameterType(ByVal n As Integer) As BasicType
				Return basicType(methodType().parameterType(n))
			End Function

			Friend Overridable Function arity() As Integer
				Return methodType().parameterCount()
			End Function

			Public Overrides Function ToString() As String
				If member_Renamed Is Nothing Then Return Convert.ToString(resolvedHandle_Renamed)
				Return member_Renamed.declaringClass.simpleName & "." & member_Renamed.name
			End Function

			Public Overridable Property identity As Boolean
				Get
					Return Me.Equals(identity(returnType()))
				End Get
			End Property

			Public Overridable Property constantZero As Boolean
				Get
					Return Me.Equals(constantZero(returnType()))
				End Get
			End Property

			Public Overridable Function intrinsicName() As MethodHandleImpl.Intrinsic
				Return If(resolvedHandle_Renamed Is Nothing, MethodHandleImpl.Intrinsic.NONE, resolvedHandle_Renamed.intrinsicName())
			End Function
		End Class

		Public Shared Function basicTypeSignature(ByVal type As MethodType) As String
			Dim sig As Char() = New Char(type.parameterCount() + 2 - 1){}
			Dim sigp As Integer = 0
			For Each pt As Class In type.parameterList()
				sig(sigp) = basicTypeChar(pt)
				sigp += 1
			Next pt
			sig(sigp) = "_"c
			sigp += 1
			sig(sigp) = basicTypeChar(type.returnType())
			sigp += 1
			assert(sigp = sig.Length)
			Return Convert.ToString(sig)
		End Function
		Public Shared Function shortenSignature(ByVal signature As String) As String
			' Hack to make signatures more readable when they show up in method names.
			Const NO_CHAR As Integer = -1, MIN_RUN As Integer = 3
			Dim c0 As Integer, c1 As Integer = NO_CHAR, c1reps As Integer = 0
			Dim buf As StringBuilder = Nothing
			Dim len As Integer = signature.length()
			If len < MIN_RUN Then Return signature
			For i As Integer = 0 To len
				' shift in the next char:
				c0 = c1
				c1 = AscW((If(i = len, NO_CHAR, signature.Chars(i))))
				If c1 = c0 Then
					c1reps += 1
					Continue For
				End If
				' shift in the next count:
				Dim c0reps As Integer = c1reps
				c1reps = 1
				' end of a  character run
				If c0reps < MIN_RUN Then
					If buf IsNot Nothing Then
						c0reps -= 1
						Do While c0reps >= 0
							buf.append(ChrW(c0))
							c0reps -= 1
						Loop
					End If
					Continue For
				End If
				' found three or more in a row
				If buf Is Nothing Then buf = (New StringBuilder).append(signature, 0, i - c0reps)
				buf.append(ChrW(c0)).append(c0reps)
			Next i
			Return If(buf Is Nothing, signature, buf.ToString())
		End Function

		Friend NotInheritable Class Name
			Friend ReadOnly type_Renamed As BasicType
			Private index_Renamed As Short
			Friend ReadOnly [function] As NamedFunction
			Friend ReadOnly constraint As Object ' additional type information, if not null
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend ReadOnly arguments As Object()

			Private Sub New(ByVal index As Integer, ByVal type As BasicType, ByVal [function] As NamedFunction, ByVal arguments As Object())
				Me.index_Renamed = CShort(index)
				Me.type_Renamed = type
				Me.function = [function]
				Me.arguments = arguments
				Me.constraint = Nothing
				assert(Me.index_Renamed = index)
			End Sub
			Private Sub New(ByVal that As Name, ByVal constraint As Object)
				Me.index_Renamed = that.index
				Me.type_Renamed = that.type_Renamed
				Me.function = that.function
				Me.arguments = that.arguments
				Me.constraint = constraint
				assert(constraint Is Nothing OrElse param) ' only params have constraints
				assert(constraint Is Nothing OrElse TypeOf constraint Is BoundMethodHandle.SpeciesData OrElse TypeOf constraint Is Class)
			End Sub
			Friend Sub New(ByVal [function] As MethodHandle, ParamArray ByVal arguments As Object())
				Me.New(New NamedFunction([function]), arguments)
			End Sub
			Friend Sub New(ByVal functionType As MethodType, ParamArray ByVal arguments As Object())
				Me.New(New NamedFunction(functionType), arguments)
				assert(TypeOf arguments(0) Is Name AndAlso CType(arguments(0), Name).type_Renamed = L_TYPE)
			End Sub
			Friend Sub New(ByVal [function] As MemberName, ParamArray ByVal arguments As Object())
				Me.New(New NamedFunction([function]), arguments)
			End Sub
			Friend Sub New(ByVal [function] As NamedFunction, ParamArray ByVal arguments As Object())
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Me.New(-1, [function].returnType(), [function], arguments = java.util.Arrays.copyOf(arguments, arguments.Length, GetType(Object())))
				assert(arguments.Length = [function].arity()) : "arity mismatch: arguments.length=" & arguments.Length & " == function.arity()=" & [function].arity() & " in " & debugString()
				For i As Integer = 0 To arguments.Length - 1
					assert(typesMatch([function].parameterType(i), arguments(i))) : "types don't match: function.parameterType(" & i & ")=" & [function].parameterType(i) & ", arguments[" & i & "]=" & arguments(i) & " in " & debugString()
				Next i
			End Sub
			''' <summary>
			''' Create a raw parameter of the given type, with an expected index. </summary>
			Friend Sub New(ByVal index As Integer, ByVal type As BasicType)
				Me.New(index, type, Nothing, Nothing)
			End Sub
			''' <summary>
			''' Create a raw parameter of the given type. </summary>
			Friend Sub New(ByVal type As BasicType)
				Me.New(-1, type)
			End Sub

			Friend Function type() As BasicType
				Return type_Renamed
			End Function
			Friend Function index() As Integer
				Return index_Renamed
			End Function
			Friend Function initIndex(ByVal i As Integer) As Boolean
				If index_Renamed <> i Then
					If index_Renamed <> -1 Then Return False
					index_Renamed = CShort(i)
				End If
				Return True
			End Function
			Friend Function typeChar() As Char
				Return type_Renamed.btChar
			End Function

			Friend Sub resolve()
				If [function] IsNot Nothing Then [function].resolve()
			End Sub

			Friend Function newIndex(ByVal i As Integer) As Name
				If initIndex(i) Then Return Me
				Return cloneWithIndex(i)
			End Function
			Friend Function cloneWithIndex(ByVal i As Integer) As Name
				Dim newArguments As Object() = If(arguments Is Nothing, Nothing, arguments.clone())
				Return (New Name(i, type_Renamed, [function], newArguments)).withConstraint(constraint)
			End Function
			Friend Function withConstraint(ByVal constraint As Object) As Name
				If constraint Is Me.constraint Then Return Me
				Return New Name(Me, constraint)
			End Function
			Friend Function replaceName(ByVal oldName As Name, ByVal newName As Name) As Name ' FIXME: use replaceNames uniformly
				If oldName Is newName Then Return Me
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim arguments As Object() = Me.arguments
				If arguments Is Nothing Then Return Me
				Dim replaced As Boolean = False
				For j As Integer = 0 To arguments.Length - 1
					If arguments(j) Is oldName Then
						If Not replaced Then
							replaced = True
							arguments = arguments.clone()
						End If
						arguments(j) = newName
					End If
				Next j
				If Not replaced Then Return Me
				Return New Name([function], arguments)
			End Function
			''' <summary>
			''' In the arguments of this Name, replace oldNames[i] pairwise by newNames[i].
			'''  Limit such replacements to {@code start<=i<end}.  Return possibly changed self.
			''' </summary>
			Friend Function replaceNames(ByVal oldNames As Name(), ByVal newNames As Name(), ByVal start As Integer, ByVal [end] As Integer) As Name
				If start >= [end] Then Return Me
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim arguments As Object() = Me.arguments
				Dim replaced As Boolean = False
			eachArg:
				For j As Integer = 0 To arguments.Length - 1
					If TypeOf arguments(j) Is Name Then
						Dim n As Name = CType(arguments(j), Name)
						Dim check As Integer = n.index
						' harmless check to see if the thing is already in newNames:
						If check >= 0 AndAlso check < newNames.Length AndAlso n Is newNames(check) Then GoTo eachArg
						' n might not have the correct index: n != oldNames[n.index].
						For i As Integer = start To [end] - 1
							If n Is oldNames(i) Then
								If n Is newNames(i) Then GoTo eachArg
								If Not replaced Then
									replaced = True
									arguments = arguments.clone()
								End If
								arguments(j) = newNames(i)
								GoTo eachArg
							End If
						Next i
					End If
				Next j
				If Not replaced Then Return Me
				Return New Name([function], arguments)
			End Function
			Friend Sub internArguments()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim arguments As Object() = Me.arguments
				For j As Integer = 0 To arguments.Length - 1
					If TypeOf arguments(j) Is Name Then
						Dim n As Name = CType(arguments(j), Name)
						If n.param AndAlso n.index < INTERNED_ARGUMENT_LIMIT Then arguments(j) = internArgument(n)
					End If
				Next j
			End Sub
			Friend Property param As Boolean
				Get
					Return [function] Is Nothing
				End Get
			End Property
			Friend Property constantZero As Boolean
				Get
					Return (Not param) AndAlso arguments.Length = 0 AndAlso [function].constantZero
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return (If(param, "a", "t"))+(If(index_Renamed >= 0, index_Renamed, System.identityHashCode(Me))) & ":" & AscW(typeChar())
			End Function
			Public Function debugString() As String
				Dim s As String = paramString()
				Return If([function] Is Nothing, s, s & "=" & exprString())
			End Function
			Public Function paramString() As String
				Dim s As String = ToString()
				Dim c As Object = constraint
				If c Is Nothing Then Return s
				If TypeOf c Is Class Then c = CType(c, [Class]).simpleName
				Return s & "/" & c
			End Function
			Public Function exprString() As String
				If [function] Is Nothing Then Return ToString()
				Dim buf As New StringBuilder([function].ToString())
				buf.append("(")
				Dim cma As String = ""
				For Each a As Object In arguments
					buf.append(cma)
					cma = ","
					If TypeOf a Is Name OrElse TypeOf a Is Integer? Then
						buf.append(a)
					Else
						buf.append("(").append(a).append(")")
					End If
				Next a
				buf.append(")")
				Return buf.ToString()
			End Function

			Friend Shared Function typesMatch(ByVal parameterType As BasicType, ByVal [object] As Object) As Boolean
				If TypeOf object_Renamed Is Name Then Return CType(object_Renamed, Name).type_Renamed = parameterType
				Select Case parameterType
					Case BasicType.I_TYPE
						Return TypeOf object_Renamed Is Integer?
					Case BasicType.J_TYPE
						Return TypeOf object_Renamed Is Long?
					Case BasicType.F_TYPE
						Return TypeOf object_Renamed Is Float
					Case BasicType.D_TYPE
						Return TypeOf object_Renamed Is Double?
				End Select
				assert(parameterType = L_TYPE)
				Return True
			End Function

			''' <summary>
			''' Return the index of the last occurrence of n in the argument array.
			'''  Return -1 if the name is not used.
			''' </summary>
			Friend Function lastUseIndex(ByVal n As Name) As Integer
				If arguments Is Nothing Then Return -1
				Dim i As Integer = arguments.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i -= 1 >= 0
					If arguments(i) Is n Then Return i
				Loop
				Return -1
			End Function

			''' <summary>
			''' Return the number of occurrences of n in the argument array.
			'''  Return 0 if the name is not used.
			''' </summary>
			Friend Function useCount(ByVal n As Name) As Integer
				If arguments Is Nothing Then Return 0
				Dim count As Integer = 0
				Dim i As Integer = arguments.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i -= 1 >= 0
					If arguments(i) Is n Then count += 1
				Loop
				Return count
			End Function

			Friend Function contains(ByVal n As Name) As Boolean
				Return Me Is n OrElse lastUseIndex(n) >= 0
			End Function

			Public Overrides Function Equals(ByVal that As Name) As Boolean
				If Me Is that Then Return True
				If param Then Return False ' this != that
				Return Me.type_Renamed = that.type_Renamed AndAlso Me.function.Equals(that.function) AndAlso java.util.Arrays.Equals(Me.arguments, that.arguments)
					'this.index == that.index &&
			End Function
			Public Overrides Function Equals(ByVal x As Object) As Boolean
				Return TypeOf x Is Name AndAlso Equals(CType(x, Name))
			End Function
			Public Overrides Function GetHashCode() As Integer
				If param Then Return index_Renamed Or (type_Renamed.ordinal() << 8)
				Return [function].GetHashCode() Xor java.util.Arrays.hashCode(arguments)
			End Function
		End Class

		''' <summary>
		''' Return the index of the last name which contains n as an argument.
		'''  Return -1 if the name is not used.  Return names.length if it is the return value.
		''' </summary>
		Friend Overridable Function lastUseIndex(ByVal n As Name) As Integer
			Dim ni As Integer = n.index, nmax As Integer = names.Length
			assert(names(ni) Is n)
			If result = ni Then ' live all the way beyond the end Return nmax
			Dim i As Integer = nmax
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Do While i -= 1 > ni
				If names(i).lastUseIndex(n) >= 0 Then Return i
			Loop
			Return -1
		End Function

		''' <summary>
		''' Return the number of times n is used as an argument or return value. </summary>
		Friend Overridable Function useCount(ByVal n As Name) As Integer
			Dim ni As Integer = n.index, nmax As Integer = names.Length
			Dim [end] As Integer = lastUseIndex(n)
			If [end] < 0 Then Return 0
			Dim count As Integer = 0
			If [end] = nmax Then
				count += 1
				[end] -= 1
			End If
			Dim beg As Integer = n.index() + 1
			If beg < arity_Renamed Then beg = arity_Renamed
			For i As Integer = beg To [end]
				count += names(i).useCount(n)
			Next i
			Return count
		End Function

		Friend Shared Function argument(ByVal which As Integer, ByVal type As Char) As Name
			Return argument(which, basicType(type))
		End Function
		Friend Shared Function argument(ByVal which As Integer, ByVal type As BasicType) As Name
			If which >= INTERNED_ARGUMENT_LIMIT Then Return New Name(which, type)
			Return INTERNED_ARGUMENTS(type.ordinal())(which)
		End Function
		Friend Shared Function internArgument(ByVal n As Name) As Name
			assert(n.param) : "not param: " & n
			assert(n.index < INTERNED_ARGUMENT_LIMIT)
			If n.constraint IsNot Nothing Then Return n
			Return argument(n.index, n.type_Renamed)
		End Function
		Friend Shared Function arguments(ByVal extra As Integer, ByVal types As String) As Name()
			Dim length As Integer = types.length()
			Dim names As Name() = New Name(length + extra - 1){}
			For i As Integer = 0 To length - 1
				names(i) = argument(i, types.Chars(i))
			Next i
			Return names
		End Function
		Friend Shared Function arguments(ByVal extra As Integer, ParamArray ByVal types As Char()) As Name()
			Dim length As Integer = types.Length
			Dim names As Name() = New Name(length + extra - 1){}
			For i As Integer = 0 To length - 1
				names(i) = argument(i, types(i))
			Next i
			Return names
		End Function
		Friend Shared Function arguments(ByVal extra As Integer, ByVal types As IList(Of [Class])) As Name()
			Dim length As Integer = types.Count
			Dim names As Name() = New Name(length + extra - 1){}
			For i As Integer = 0 To length - 1
				names(i) = argument(i, basicType(types(i)))
			Next i
			Return names
		End Function
		Friend Shared Function arguments(ByVal extra As Integer, ParamArray ByVal types As Class()) As Name()
			Dim length As Integer = types.Length
			Dim names As Name() = New Name(length + extra - 1){}
			For i As Integer = 0 To length - 1
				names(i) = argument(i, basicType(types(i)))
			Next i
			Return names
		End Function
		Friend Shared Function arguments(ByVal extra As Integer, ByVal types As MethodType) As Name()
			Dim length As Integer = types.parameterCount()
			Dim names As Name() = New Name(length + extra - 1){}
			For i As Integer = 0 To length - 1
				names(i) = argument(i, basicType(types.parameterType(i)))
			Next i
			Return names
		End Function
		Friend Const INTERNED_ARGUMENT_LIMIT As Integer = 10
		Private Shared ReadOnly INTERNED_ARGUMENTS As Name()() = RectangularArrays.ReturnRectangularNameArray(ARG_TYPE_LIMIT, INTERNED_ARGUMENT_LIMIT)

		Private Shared ReadOnly IMPL_NAMES As MemberName.Factory = MemberName.factory

		Shared Function identityForm(ByVal type As BasicType) As LambdaForm
			Return LF_identityForm(type.ordinal())
		End Function
		Shared Function zeroForm(ByVal type As BasicType) As LambdaForm
			Return LF_zeroForm(type.ordinal())
		End Function
		Friend Shared Function identity(ByVal type As BasicType) As NamedFunction
			Return NF_identity(type.ordinal())
		End Function
		Friend Shared Function constantZero(ByVal type As BasicType) As NamedFunction
			Return NF_zero(type.ordinal())
		End Function
		Private Shared ReadOnly LF_identityForm As LambdaForm() = New LambdaForm(TYPE_LIMIT - 1){}
		Private Shared ReadOnly LF_zeroForm As LambdaForm() = New LambdaForm(TYPE_LIMIT - 1){}
		Private Shared ReadOnly NF_identity As NamedFunction() = New NamedFunction(TYPE_LIMIT - 1){}
		Private Shared ReadOnly NF_zero As NamedFunction() = New NamedFunction(TYPE_LIMIT - 1){}
		Private Shared Sub createIdentityForms()
			For Each type As BasicType In BasicType.ALL_TYPES
				Dim ord As Integer = type.ordinal()
				Dim btChar As Char = type.basicTypeChar()
				Dim isVoid As Boolean = (type = V_TYPE)
				Dim btClass As Class = type.btClass
				Dim zeType As MethodType = MethodType.methodType(btClass)
				Dim idType As MethodType = If(isVoid, zeType, zeType.appendParameterTypes(btClass))

				' Look up some symbolic names.  It might not be necessary to have these,
				' but if we need to emit direct references to bytecodes, it helps.
				' Zero is built from a call to an identity function with a constant zero input.
				Dim idMem As New MemberName(GetType(LambdaForm), "identity_" & AscW(btChar), idType, REF_invokeStatic)
				Dim zeMem As New MemberName(GetType(LambdaForm), "zero_" & AscW(btChar), zeType, REF_invokeStatic)
				Try
					zeMem = IMPL_NAMES.resolveOrFail(REF_invokeStatic, zeMem, Nothing, GetType(NoSuchMethodException))
					idMem = IMPL_NAMES.resolveOrFail(REF_invokeStatic, idMem, Nothing, GetType(NoSuchMethodException))
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch IllegalAccessException Or NoSuchMethodException ex
					Throw newInternalError(ex)
				End Try

				Dim idFun As New NamedFunction(idMem)
				Dim idForm As LambdaForm
				If isVoid Then
					Dim idNames As Name() = { argument(0, L_TYPE) }
					idForm = New LambdaForm(idMem.name, 1, idNames, VOID_RESULT)
				Else
					Dim idNames As Name() = { argument(0, L_TYPE), argument(1, type) }
					idForm = New LambdaForm(idMem.name, 2, idNames, 1)
				End If
				LF_identityForm(ord) = idForm
				NF_identity(ord) = idFun

				Dim zeFun As New NamedFunction(zeMem)
				Dim zeForm As LambdaForm
				If isVoid Then
					zeForm = idForm
				Else
					Dim zeValue As Object = sun.invoke.util.Wrapper.forBasicType(btChar).zero()
					Dim zeNames As Name() = { argument(0, L_TYPE), New Name(idFun, zeValue) }
					zeForm = New LambdaForm(zeMem.name, 1, zeNames, 1)
				End If
				LF_zeroForm(ord) = zeForm
				NF_zero(ord) = zeFun

				assert(idFun.identity)
				assert(zeFun.constantZero)
				assert((New Name(zeFun)).constantZero)
			Next type

			' Do this in a separate pass, so that SimpleMethodHandle.make can see the tables.
			For Each type As BasicType In BasicType.ALL_TYPES
				Dim ord As Integer = type.ordinal()
				Dim idFun As NamedFunction = NF_identity(ord)
				Dim idForm As LambdaForm = LF_identityForm(ord)
				Dim idMem As MemberName = idFun.member_Renamed
				idFun.resolvedHandle_Renamed = SimpleMethodHandle.make(idMem.invocationType, idForm)

				Dim zeFun As NamedFunction = NF_zero(ord)
				Dim zeForm As LambdaForm = LF_zeroForm(ord)
				Dim zeMem As MemberName = zeFun.member_Renamed
				zeFun.resolvedHandle_Renamed = SimpleMethodHandle.make(zeMem.invocationType, zeForm)

				assert(idFun.identity)
				assert(zeFun.constantZero)
				assert((New Name(zeFun)).constantZero)
			Next type
		End Sub

		' Avoid appealing to ValueConversions at bootstrap time:
		Private Shared Function identity_I(ByVal x As Integer) As Integer
			Return x
		End Function
		Private Shared Function identity_J(ByVal x As Long) As Long
			Return x
		End Function
		Private Shared Function identity_F(ByVal x As Single) As Single
			Return x
		End Function
		Private Shared Function identity_D(ByVal x As Double) As Double
			Return x
		End Function
		Private Shared Function identity_L(ByVal x As Object) As Object
			Return x
		End Function
		Private Shared Sub identity_V() ' same as zeroV, but that's OK
			Return
		End Sub
		Private Shared Function zero_I() As Integer
			Return 0
		End Function
		Private Shared Function zero_J() As Long
			Return 0
		End Function
		Private Shared Function zero_F() As Single
			Return 0
		End Function
		Private Shared Function zero_D() As Double
			Return 0
		End Function
		Private Shared Function zero_L() As Object
			Return Nothing
		End Function
		Private Shared Sub zero_V()
			Return
		End Sub

		''' <summary>
		''' Internal marker for byte-compiled LambdaForms.
		''' </summary>
		'non-public
		<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
		Friend Class Compiled
			Inherits System.Attribute

			Private ReadOnly outerInstance As LambdaForm

			Public Sub New(ByVal outerInstance As LambdaForm)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		''' <summary>
		''' Internal marker for LambdaForm interpreter frames.
		''' </summary>
		'non-public
		<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
		Friend Class Hidden
			Inherits System.Attribute

			Private ReadOnly outerInstance As LambdaForm

			Public Sub New(ByVal outerInstance As LambdaForm)
				Me.outerInstance = outerInstance
			End Sub

		End Class

		Private Shared ReadOnly DEBUG_NAME_COUNTERS As Dictionary(Of String, Integer?)

		' Put this last, so that previous static inits can run before.

		' The following hack is necessary in order to suppress TRACE_INTERPRETER
		' during execution of the static initializes of this class.
		' Turning on TRACE_INTERPRETER too early will cause
		' stack overflows and other misbehavior during attempts to trace events
		' that occur during LambdaForm.<clinit>.
		' Therefore, do not move this line higher in this file, and do not remove.
		Private Shared ReadOnly TRACE_INTERPRETER As Boolean = MethodHandleStatics.TRACE_INTERPRETER
	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularNameArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As Name()()
        Dim Array As Name()() = New Name(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Name(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class