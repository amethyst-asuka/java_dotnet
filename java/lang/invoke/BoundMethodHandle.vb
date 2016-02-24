Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic
Imports jdk.internal.org.objectweb.asm.Opcodes

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
	''' The flavor of method handle which emulates an invoke instruction
	''' on a predetermined argument.  The JVM dispatches to the correct method
	''' when the handle is created, not when it is invoked.
	''' 
	''' All bound arguments are encapsulated in dedicated species.
	''' </summary>
	'non-public
	 Friend MustInherit Class BoundMethodHandle
		 Inherits MethodHandle

		'non-public
	 Friend Sub New(ByVal type As MethodType, ByVal form As LambdaForm)
			MyBase.New(type, form)
			assert(speciesData() Is speciesData(form))
	 End Sub

		'
		' BMH API and internals
		'

		Shared Function bindSingle(ByVal type As MethodType, ByVal form As LambdaForm, ByVal xtype As BasicType, ByVal x As Object) As BoundMethodHandle
			' for some type signatures, there exist pre-defined concrete BMH classes
			Try
				Select Case xtype
				Case L_TYPE
					Return bindSingle(type, form, x) ' Use known fast path.
				Case I_TYPE
					Return CType(SpeciesData.EMPTY.extendWith(I_TYPE).constructor().invokeBasic(type, form, sun.invoke.util.ValueConversions.widenSubword(x)), BoundMethodHandle)
				Case J_TYPE
					Return CType(SpeciesData.EMPTY.extendWith(J_TYPE).constructor().invokeBasic(type, form, CLng(Fix(x))), BoundMethodHandle)
				Case F_TYPE
					Return CType(SpeciesData.EMPTY.extendWith(F_TYPE).constructor().invokeBasic(type, form, CSng(x)), BoundMethodHandle)
				Case D_TYPE
					Return CType(SpeciesData.EMPTY.extendWith(D_TYPE).constructor().invokeBasic(type, form, CDbl(x)), BoundMethodHandle)
				Case Else
					Throw newInternalError("unexpected xtype: " & xtype)
				End Select
			Catch t As Throwable
				Throw newInternalError(t)
			End Try
		End Function

		'non-public
		Friend Overridable Function editor() As LambdaFormEditor
			Return form.editor()
		End Function

		Shared Function bindSingle(ByVal type As MethodType, ByVal form As LambdaForm, ByVal x As Object) As BoundMethodHandle
			Return Species_L.make(type, form, x)
		End Function

		Friend Overrides Function bindArgumentL(ByVal pos As Integer, ByVal value As Object) As BoundMethodHandle ' there is a default binder in the super class, for 'L' types only
		'non-public
			Return editor().bindArgumentL(Me, pos, value)
		End Function
		'non-public
		Friend Overridable Function bindArgumentI(ByVal pos As Integer, ByVal value As Integer) As BoundMethodHandle
			Return editor().bindArgumentI(Me, pos, value)
		End Function
		'non-public
		Friend Overridable Function bindArgumentJ(ByVal pos As Integer, ByVal value As Long) As BoundMethodHandle
			Return editor().bindArgumentJ(Me, pos, value)
		End Function
		'non-public
		Friend Overridable Function bindArgumentF(ByVal pos As Integer, ByVal value As Single) As BoundMethodHandle
			Return editor().bindArgumentF(Me, pos, value)
		End Function
		'non-public
		Friend Overridable Function bindArgumentD(ByVal pos As Integer, ByVal value As Double) As BoundMethodHandle
			Return editor().bindArgumentD(Me, pos, value)
		End Function

		Friend Overrides Function rebind() As BoundMethodHandle
			If Not tooComplex() Then Return Me
			Return makeReinvoker(Me)
		End Function

		Private Function tooComplex() As Boolean
			Return (fieldCount() > FIELD_COUNT_THRESHOLD OrElse form.expressionCount() > FORM_EXPRESSION_THRESHOLD)
		End Function
		Private Const FIELD_COUNT_THRESHOLD As Integer = 12 ' largest convenient BMH field count
		Private Const FORM_EXPRESSION_THRESHOLD As Integer = 24 ' largest convenient BMH expression count

		''' <summary>
		''' A reinvoker MH has this form:
		''' {@code lambda (bmh, arg*) { thismh = bmh[0]; invokeBasic(thismh, arg*) }}
		''' </summary>
		Shared Function makeReinvoker(ByVal target As MethodHandle) As BoundMethodHandle
			Dim form As LambdaForm = DelegatingMethodHandle.makeReinvokerForm(target, MethodTypeForm.LF_REBIND, Species_L.SPECIES_DATA, Species_L.SPECIES_DATA.getterFunction(0))
			Return Species_L.make(target.type(), form, target)
		End Function

		''' <summary>
		''' Return the <seealso cref="SpeciesData"/> instance representing this BMH species. All subclasses must provide a
		''' static field containing this value, and they must accordingly implement this method.
		''' </summary>
		'non-public
	 Friend MustOverride Function speciesData() As SpeciesData

		'non-public
	 Friend Shared Function speciesData(ByVal form As LambdaForm) As SpeciesData
			Dim c As Object = form.names(0).constraint
			If TypeOf c Is SpeciesData Then Return CType(c, SpeciesData)
			' if there is no BMH constraint, then use the null constraint
			Return SpeciesData.EMPTY
	 End Function

		''' <summary>
		''' Return the number of fields in this BMH.  Equivalent to speciesData().fieldCount().
		''' </summary>
		'non-public
	 Friend MustOverride Function fieldCount() As Integer

		Friend Overrides Function internalProperties() As Object
			Return vbLf & "& BMH=" & internalValues()
		End Function

		Friend NotOverridable Overrides Function internalValues() As Object
			Dim boundValues As Object() = New Object(speciesData().fieldCount() - 1){}
			For i As Integer = 0 To boundValues.Length - 1
				boundValues(i) = arg(i)
			Next i
			Return java.util.Arrays.asList(boundValues)
		End Function

		'non-public
	 Friend Function arg(ByVal i As Integer) As Object
			Try
				Select Case speciesData().fieldType(i)
				Case L_TYPE
					Return speciesData().getters(i).invokeBasic(Me)
				Case I_TYPE
					Return CInt(Fix(speciesData().getters(i).invokeBasic(Me)))
				Case J_TYPE
					Return CLng(Fix(speciesData().getters(i).invokeBasic(Me)))
				Case F_TYPE
					Return CSng(speciesData().getters(i).invokeBasic(Me))
				Case D_TYPE
					Return CDbl(speciesData().getters(i).invokeBasic(Me))
				End Select
			Catch ex As Throwable
				Throw newInternalError(ex)
			End Try
			Throw New InternalError("unexpected type: " & speciesData().typeChars & "." & i)
	 End Function

		'
		' cloning API
		'

		'non-public
	 Friend MustOverride Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As BoundMethodHandle
		'non-public
	 Friend MustOverride Function copyWithExtendL(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Object) As BoundMethodHandle
		'non-public
	 Friend MustOverride Function copyWithExtendI(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Integer) As BoundMethodHandle
		'non-public
	 Friend MustOverride Function copyWithExtendJ(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Long) As BoundMethodHandle
		'non-public
	 Friend MustOverride Function copyWithExtendF(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Single) As BoundMethodHandle
		'non-public
	 Friend MustOverride Function copyWithExtendD(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Double) As BoundMethodHandle

		'
		' concrete BMH classes required to close bootstrap loops
		'

		Private NotInheritable Class Species_L
			Inherits BoundMethodHandle
 ' make it private to force users to access the enclosing class first
			Friend ReadOnly argL0 As Object
			Private Sub New(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal argL0 As Object)
				MyBase.New(mt, lf)
				Me.argL0 = argL0
			End Sub
			Friend Overrides Function speciesData() As SpeciesData
			'non-public
				Return SPECIES_DATA
			End Function
			Friend Overrides Function fieldCount() As Integer
			'non-public
				Return 1
			End Function
			'non-public
	 Friend Shadows Shared ReadOnly SPECIES_DATA As SpeciesData = SpeciesData.getForClass("L", GetType(Species_L))
			'non-public
	 Friend Shared Function make(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal argL0 As Object) As BoundMethodHandle
				Return New Species_L(mt, lf, argL0)
	 End Function
			Friend NotOverridable Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As BoundMethodHandle
			'non-public
				Return New Species_L(mt, lf, argL0)
			End Function
			Friend NotOverridable Overrides Function copyWithExtendL(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Object) As BoundMethodHandle
			'non-public
				Try
					Return CType(SPECIES_DATA.extendWith(L_TYPE).constructor().invokeBasic(mt, lf, argL0, narg), BoundMethodHandle)
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
			End Function
			Friend NotOverridable Overrides Function copyWithExtendI(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Integer) As BoundMethodHandle
			'non-public
				Try
					Return CType(SPECIES_DATA.extendWith(I_TYPE).constructor().invokeBasic(mt, lf, argL0, narg), BoundMethodHandle)
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
			End Function
			Friend NotOverridable Overrides Function copyWithExtendJ(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Long) As BoundMethodHandle
			'non-public
				Try
					Return CType(SPECIES_DATA.extendWith(J_TYPE).constructor().invokeBasic(mt, lf, argL0, narg), BoundMethodHandle)
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
			End Function
			Friend NotOverridable Overrides Function copyWithExtendF(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Single) As BoundMethodHandle
			'non-public
				Try
					Return CType(SPECIES_DATA.extendWith(F_TYPE).constructor().invokeBasic(mt, lf, argL0, narg), BoundMethodHandle)
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
			End Function
			Friend NotOverridable Overrides Function copyWithExtendD(ByVal mt As MethodType, ByVal lf As LambdaForm, ByVal narg As Double) As BoundMethodHandle
			'non-public
				Try
					Return CType(SPECIES_DATA.extendWith(D_TYPE).constructor().invokeBasic(mt, lf, argL0, narg), BoundMethodHandle)
				Catch ex As Throwable
					Throw uncaughtException(ex)
				End Try
			End Function
		End Class

		'
		' BMH species meta-data
		'

		''' <summary>
		''' Meta-data wrapper for concrete BMH types.
		''' Each BMH type corresponds to a given sequence of basic field types (LIJFD).
		''' The fields are immutable; their values are fully specified at object construction.
		''' Each BMH type supplies an array of getter functions which may be used in lambda forms.
		''' A BMH is constructed by cloning a shorter BMH and adding one or more new field values.
		''' The shortest possible BMH has zero fields; its class is SimpleMethodHandle.
		''' BMH species are not interrelated by subtyping, even though it would appear that
		''' a shorter BMH could serve as a supertype of a longer one which extends it.
		''' </summary>
		Friend Class SpeciesData
			Private ReadOnly typeChars As String
			Private ReadOnly typeCodes As BasicType()
			Private ReadOnly clazz As Class
			' Bootstrapping requires circular relations MH -> BMH -> SpeciesData -> MH
			' Therefore, we need a non-final link in the chain.  Use array elements.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ReadOnly constructor_Renamed As MethodHandle()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ReadOnly getters As MethodHandle()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ReadOnly nominalGetters As NamedFunction()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private ReadOnly extensions As SpeciesData()

			'non-public
	 Friend Overridable Function fieldCount() As Integer
				Return typeCodes.Length
	 End Function
			'non-public
	 Friend Overridable Function fieldType(ByVal i As Integer) As BasicType
				Return typeCodes(i)
	 End Function
			'non-public
	 Friend Overridable Function fieldTypeChar(ByVal i As Integer) As Char
				Return typeChars.Chars(i)
	 End Function
			Friend Overridable Function fieldSignature() As Object
				Return typeChars
			End Function
			Public Overridable Function fieldHolder() As Class
				Return clazz
			End Function
			Public Overrides Function ToString() As String
				Return "SpeciesData<" & fieldSignature() & ">"
			End Function

			''' <summary>
			''' Return a <seealso cref="LambdaForm.Name"/> containing a <seealso cref="LambdaForm.NamedFunction"/> that
			''' represents a MH bound to a generic invoker, which in turn forwards to the corresponding
			''' getter.
			''' </summary>
			Friend Overridable Function getterFunction(ByVal i As Integer) As NamedFunction
				Dim nf As NamedFunction = nominalGetters(i)
				assert(nf.memberDeclaringClassOrNull() Is fieldHolder())
				assert(nf.returnType() Is fieldType(i))
				Return nf
			End Function

			Friend Overridable Function getterFunctions() As NamedFunction()
				Return nominalGetters
			End Function

			Friend Overridable Function getterHandles() As MethodHandle()
				Return getters
			End Function

			Friend Overridable Function constructor() As MethodHandle
				Return constructor_Renamed(0)
			End Function

			Friend Shared ReadOnly EMPTY As New SpeciesData("", GetType(BoundMethodHandle))

			Private Sub New(ByVal types As String, ByVal clazz As Class)
				Me.typeChars = types
				Me.typeCodes = basicTypes(types)
				Me.clazz = clazz
				If Not INIT_DONE Then
					Me.constructor_Renamed = New MethodHandle(0){} ' only one ctor
					Me.getters = New MethodHandle(types.length() - 1){}
					Me.nominalGetters = New NamedFunction(types.length() - 1){}
				Else
					Me.constructor_Renamed = Factory.makeCtors(clazz, types, Nothing)
					Me.getters = Factory.makeGetters(clazz, types, Nothing)
					Me.nominalGetters = Factory.makeNominalGetters(types, Nothing, Me.getters)
				End If
				Me.extensions = New SpeciesData(ARG_TYPE_LIMIT - 1){}
			End Sub

			Private Sub initForBootstrap()
				assert((Not INIT_DONE))
				If constructor() Is Nothing Then
					Dim types As String = typeChars
					Factory.makeCtors(clazz, types, Me.constructor_Renamed)
					Factory.makeGetters(clazz, types, Me.getters)
					Factory.makeNominalGetters(types, Me.nominalGetters, Me.getters)
				End If
			End Sub

			Private Sub New(ByVal typeChars As String)
				' Placeholder only.
				Me.typeChars = typeChars
				Me.typeCodes = basicTypes(typeChars)
				Me.clazz = Nothing
				Me.constructor_Renamed = Nothing
				Me.getters = Nothing
				Me.nominalGetters = Nothing
				Me.extensions = Nothing
			End Sub
			Private Property placeholder As Boolean
				Get
					Return clazz Is Nothing
				End Get
			End Property

			Private Shared ReadOnly CACHE As New Dictionary(Of String, SpeciesData)
			Shared Sub New() ' make bootstrap predictable
				CACHE("") = EMPTY
				' pre-fill the BMH speciesdata cache with BMH's inner classes
				Dim rootCls As Class = GetType(BoundMethodHandle)
				Try
					For Each c As Class In rootCls.declaredClasses
						If c.IsSubclassOf(rootCls) Then
							Dim cbmh As Class = c.asSubclass(GetType(BoundMethodHandle))
							Dim d As SpeciesData = Factory.speciesDataFromConcreteBMHClass(cbmh)
							assert(d IsNot Nothing) : cbmh.name
							assert(d.clazz Is cbmh)
							assert(d Is lookupCache(d.typeChars))
						End If
					Next c
				Catch e As Throwable
					Throw newInternalError(e)
				End Try

				For Each d As SpeciesData In CACHE.Values
					d.initForBootstrap()
				Next d
				' Note:  Do not simplify this, because INIT_DONE must not be
				' a compile-time constant during bootstrapping.
				INIT_DONE = Boolean.TRUE
			End Sub
			Private Shared ReadOnly INIT_DONE As Boolean ' set after <clinit> finishes...

			Friend Overridable Function extendWith(ByVal type As SByte) As SpeciesData
				Return extendWith(BasicType.basicType(type))
			End Function

			Friend Overridable Function extendWith(ByVal type As BasicType) As SpeciesData
				Dim ord As Integer = type.ordinal()
				Dim d As SpeciesData = extensions(ord)
				If d IsNot Nothing Then Return d
					d = [get](typeChars+type.basicTypeChar())
					extensions(ord) = d
				Return d
			End Function

			Private Shared Function [get](ByVal types As String) As SpeciesData
				' Acquire cache lock for query.
				Dim d As SpeciesData = lookupCache(types)
				If Not d.placeholder Then Return d
				SyncLock d
					' Use synch. on the placeholder to prevent multiple instantiation of one species.
					' Creating this class forces a recursive call to getForClass.
					If lookupCache(types).placeholder Then Factory.generateConcreteBMHClass(types)
				End SyncLock
				' Reacquire cache lock.
				d = lookupCache(types)
				' Class loading must have upgraded the cache.
				assert(d IsNot Nothing AndAlso (Not d.placeholder))
				Return d
			End Function
			Shared Function getForClass(ByVal types As String, ByVal clazz As Class) As SpeciesData
				' clazz is a new class which is initializing its SPECIES_DATA field
				Return updateCache(types, New SpeciesData(types, clazz))
			End Function
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Private Shared Function lookupCache(ByVal types As String) As SpeciesData
				Dim d As SpeciesData = CACHE(types)
				If d IsNot Nothing Then Return d
				d = New SpeciesData(types)
				assert(d.placeholder)
				CACHE(types) = d
				Return d
			End Function
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Private Shared Function updateCache(ByVal types As String, ByVal d As SpeciesData) As SpeciesData
				Dim d2 As SpeciesData
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				assert((d2 = CACHE(types)) Is Nothing OrElse d2.placeholder)
				assert((Not d.placeholder))
				CACHE(types) = d
				Return d
			End Function

		End Class

		Friend Shared Function getSpeciesData(ByVal types As String) As SpeciesData
			Return SpeciesData.get(types)
		End Function

		''' <summary>
		''' Generation of concrete BMH classes.
		''' 
		''' A concrete BMH species is fit for binding a number of values adhering to a
		''' given type pattern. Reference types are erased.
		''' 
		''' BMH species are cached by type pattern.
		''' 
		''' A BMH species has a number of fields with the concrete (possibly erased) types of
		''' bound values. Setters are provided as an API in BMH. Getters are exposed as MHs,
		''' which can be included as names in lambda forms.
		''' </summary>
		Friend Class Factory

			Friend Const JLO_SIG As String = "Ljava/lang/Object;"
			Friend Const JLS_SIG As String = "Ljava/lang/String;"
			Friend Const JLC_SIG As String = "Ljava/lang/Class;"
			Friend Const MH As String = "java/lang/invoke/MethodHandle"
			Friend Shared ReadOnly MH_SIG As String = "L" & MH & ";"
			Friend Const BMH As String = "java/lang/invoke/BoundMethodHandle"
			Friend Shared ReadOnly BMH_SIG As String = "L" & BMH & ";"
			Friend Const SPECIES_DATA As String = "java/lang/invoke/BoundMethodHandle$SpeciesData"
			Friend Shared ReadOnly SPECIES_DATA_SIG As String = "L" & SPECIES_DATA & ";"

			Friend Const SPECIES_PREFIX_NAME As String = "Species_"
			Friend Shared ReadOnly SPECIES_PREFIX_PATH As String = BMH & "$" & SPECIES_PREFIX_NAME

			Friend Shared ReadOnly BMHSPECIES_DATA_EWI_SIG As String = "(B)" & SPECIES_DATA_SIG
			Friend Shared ReadOnly BMHSPECIES_DATA_GFC_SIG As String = "(" & JLS_SIG + JLC_SIG & ")" & SPECIES_DATA_SIG
			Friend Shared ReadOnly MYSPECIES_DATA_SIG As String = "()" & SPECIES_DATA_SIG
			Friend Const VOID_SIG As String = "()V"
			Friend Const INT_SIG As String = "()I"

			Friend Const SIG_INCIPIT As String = "(Ljava/lang/invoke/MethodType;Ljava/lang/invoke/LambdaForm;"

			Friend Shared ReadOnly E_THROWABLE As String() = { "java/lang/Throwable" }

			''' <summary>
			''' Generate a concrete subclass of BMH for a given combination of bound types.
			''' 
			''' A concrete BMH species adheres to the following schema:
			''' 
			''' <pre>
			''' class Species_[[types]] extends BoundMethodHandle {
			'''     [[fields]]
			'''     final SpeciesData speciesData() { return SpeciesData.get("[[types]]"); }
			''' }
			''' </pre>
			''' 
			''' The {@code [[types]]} signature is precisely the string that is passed to this
			''' method.
			''' 
			''' The {@code [[fields]]} section consists of one field definition per character in
			''' the type signature, adhering to the naming schema described in the definition of
			''' <seealso cref="#makeFieldName"/>.
			''' 
			''' For example, a concrete BMH species for two reference and one integral bound values
			''' would have the following shape:
			''' 
			''' <pre>
			''' class BoundMethodHandle { ... private static
			''' final class Species_LLI extends BoundMethodHandle {
			'''     final Object argL0;
			'''     final Object argL1;
			'''     final int argI2;
			'''     private Species_LLI(MethodType mt, LambdaForm lf, Object argL0, Object argL1, int argI2) {
			'''         super(mt, lf);
			'''         this.argL0 = argL0;
			'''         this.argL1 = argL1;
			'''         this.argI2 = argI2;
			'''     }
			'''     final SpeciesData speciesData() { return SPECIES_DATA; }
			'''     final int fieldCount() { return 3; }
			'''     static final SpeciesData SPECIES_DATA = SpeciesData.getForClass("LLI", Species_LLI.class);
			'''     static BoundMethodHandle make(MethodType mt, LambdaForm lf, Object argL0, Object argL1, int argI2) {
			'''         return new Species_LLI(mt, lf, argL0, argL1, argI2);
			'''     }
			'''     final BoundMethodHandle copyWith(MethodType mt, LambdaForm lf) {
			'''         return new Species_LLI(mt, lf, argL0, argL1, argI2);
			'''     }
			'''     final BoundMethodHandle copyWithExtendL(MethodType mt, LambdaForm lf, Object narg) {
			'''         return SPECIES_DATA.extendWith(L_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			'''     }
			'''     final BoundMethodHandle copyWithExtendI(MethodType mt, LambdaForm lf, int narg) {
			'''         return SPECIES_DATA.extendWith(I_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			'''     }
			'''     final BoundMethodHandle copyWithExtendJ(MethodType mt, LambdaForm lf, long narg) {
			'''         return SPECIES_DATA.extendWith(J_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			'''     }
			'''     final BoundMethodHandle copyWithExtendF(MethodType mt, LambdaForm lf, float narg) {
			'''         return SPECIES_DATA.extendWith(F_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			'''     }
			'''     public final BoundMethodHandle copyWithExtendD(MethodType mt, LambdaForm lf, double narg) {
			'''         return SPECIES_DATA.extendWith(D_TYPE).constructor().invokeBasic(mt, lf, argL0, argL1, argI2, narg);
			'''     }
			''' }
			''' </pre>
			''' </summary>
			''' <param name="types"> the type signature, wherein reference types are erased to 'L' </param>
			''' <returns> the generated concrete BMH class </returns>
			Friend Shared Function generateConcreteBMHClass(ByVal types As String) As Class
				Dim cw As New jdk.internal.org.objectweb.asm.ClassWriter(jdk.internal.org.objectweb.asm.ClassWriter.COMPUTE_MAXS + jdk.internal.org.objectweb.asm.ClassWriter.COMPUTE_FRAMES)

				Dim shortTypes As String = LambdaForm.shortenSignature(types)
				Dim className As String = SPECIES_PREFIX_PATH + shortTypes
				Dim sourceFile As String = SPECIES_PREFIX_NAME + shortTypes
				Const NOT_ACC_PUBLIC As Integer = 0 ' not ACC_PUBLIC
				cw.visit(V1_6, NOT_ACC_PUBLIC + ACC_FINAL + ACC_SUPER, className, Nothing, BMH, Nothing)
				cw.visitSource(sourceFile, Nothing)

				' emit static types and SPECIES_DATA fields
				cw.visitField(NOT_ACC_PUBLIC + ACC_STATIC, "SPECIES_DATA", SPECIES_DATA_SIG, Nothing, Nothing).visitEnd()

				' emit bound argument fields
				For i As Integer = 0 To types.length() - 1
					Dim t As Char = types.Chars(i)
					Dim fieldName As String = makeFieldName(types, i)
					Dim fieldDesc As String = If(t = "L"c, JLO_SIG, Convert.ToString(t))
					cw.visitField(ACC_FINAL, fieldName, fieldDesc, Nothing, Nothing).visitEnd()
				Next i

				Dim mv As jdk.internal.org.objectweb.asm.MethodVisitor

				' emit constructor
				mv = cw.visitMethod(ACC_PRIVATE, "<init>", makeSignature(types, True), Nothing, Nothing)
				mv.visitCode()
				mv.visitVarInsn(ALOAD, 0) ' this
				mv.visitVarInsn(ALOAD, 1) ' type
				mv.visitVarInsn(ALOAD, 2) ' form

				mv.visitMethodInsn(INVOKESPECIAL, BMH, "<init>", makeSignature("", True), False)

				Dim i As Integer = 0
				Dim j As Integer = 0
				Do While i < types.length()
					' i counts the arguments, j counts corresponding argument slots
					Dim t As Char = types.Chars(i)
					mv.visitVarInsn(ALOAD, 0)
					mv.visitVarInsn(typeLoadOp(t), j + 3) ' parameters start at 3
					mv.visitFieldInsn(PUTFIELD, className, makeFieldName(types, i), typeSig(t))
					If t = "J"c OrElse t = "D"c Then j += 1 ' adjust argument register access
					i += 1
					j += 1
				Loop

				mv.visitInsn(RETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()

				' emit implementation of speciesData()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "speciesData", MYSPECIES_DATA_SIG, Nothing, Nothing)
				mv.visitCode()
				mv.visitFieldInsn(GETSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG)
				mv.visitInsn(ARETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()

				' emit implementation of fieldCount()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "fieldCount", INT_SIG, Nothing, Nothing)
				mv.visitCode()
				Dim fc As Integer = types.length()
				If fc <= (ICONST_5 - ICONST_0) Then
					mv.visitInsn(ICONST_0 + fc)
				Else
					mv.visitIntInsn(SIPUSH, fc)
				End If
				mv.visitInsn(IRETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()
				' emit make()  ...factory method wrapping constructor
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_STATIC, "make", makeSignature(types, False), Nothing, Nothing)
				mv.visitCode()
				' make instance
				mv.visitTypeInsn(NEW, className)
				mv.visitInsn(DUP)
				' load mt, lf
				mv.visitVarInsn(ALOAD, 0) ' type
				mv.visitVarInsn(ALOAD, 1) ' form
				' load factory method arguments
				i = 0
				j = 0
				Do While i < types.length()
					' i counts the arguments, j counts corresponding argument slots
					Dim t As Char = types.Chars(i)
					mv.visitVarInsn(typeLoadOp(t), j + 2) ' parameters start at 3
					If t = "J"c OrElse t = "D"c Then j += 1 ' adjust argument register access
					i += 1
					j += 1
				Loop

				' finally, invoke the constructor and return
				mv.visitMethodInsn(INVOKESPECIAL, className, "<init>", makeSignature(types, True), False)
				mv.visitInsn(ARETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()

				' emit copyWith()
				mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "copyWith", makeSignature("", False), Nothing, Nothing)
				mv.visitCode()
				' make instance
				mv.visitTypeInsn(NEW, className)
				mv.visitInsn(DUP)
				' load mt, lf
				mv.visitVarInsn(ALOAD, 1)
				mv.visitVarInsn(ALOAD, 2)
				' put fields on the stack
				emitPushFields(types, className, mv)
				' finally, invoke the constructor and return
				mv.visitMethodInsn(INVOKESPECIAL, className, "<init>", makeSignature(types, True), False)
				mv.visitInsn(ARETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()

				' for each type, emit copyWithExtendT()
				For Each type As BasicType In BasicType.ARG_TYPES
					Dim ord As Integer = type.ordinal()
					Dim btChar As Char = type.basicTypeChar()
					mv = cw.visitMethod(NOT_ACC_PUBLIC + ACC_FINAL, "copyWithExtend" & AscW(btChar), makeSignature(Convert.ToString(btChar), False), Nothing, E_THROWABLE)
					mv.visitCode()
					' return SPECIES_DATA.extendWith(t).constructor().invokeBasic(mt, lf, argL0, ..., narg)
					' obtain constructor
					mv.visitFieldInsn(GETSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG)
					Dim iconstInsn As Integer = ICONST_0 + ord
					assert(iconstInsn <= ICONST_5)
					mv.visitInsn(iconstInsn)
					mv.visitMethodInsn(INVOKEVIRTUAL, SPECIES_DATA, "extendWith", BMHSPECIES_DATA_EWI_SIG, False)
					mv.visitMethodInsn(INVOKEVIRTUAL, SPECIES_DATA, "constructor", "()" & MH_SIG, False)
					' load mt, lf
					mv.visitVarInsn(ALOAD, 1)
					mv.visitVarInsn(ALOAD, 2)
					' put fields on the stack
					emitPushFields(types, className, mv)
					' put narg on stack
					mv.visitVarInsn(typeLoadOp(btChar), 3)
					' finally, invoke the constructor and return
					mv.visitMethodInsn(INVOKEVIRTUAL, MH, "invokeBasic", makeSignature(types + AscW(btChar), False), False)
					mv.visitInsn(ARETURN)
					mv.visitMaxs(0, 0)
					mv.visitEnd()
				Next type

				' emit class initializer
				mv = cw.visitMethod(NOT_ACC_PUBLIC Or ACC_STATIC, "<clinit>", VOID_SIG, Nothing, Nothing)
				mv.visitCode()
				mv.visitLdcInsn(types)
				mv.visitLdcInsn(jdk.internal.org.objectweb.asm.Type.getObjectType(className))
				mv.visitMethodInsn(INVOKESTATIC, SPECIES_DATA, "getForClass", BMHSPECIES_DATA_GFC_SIG, False)
				mv.visitFieldInsn(PUTSTATIC, className, "SPECIES_DATA", SPECIES_DATA_SIG)
				mv.visitInsn(RETURN)
				mv.visitMaxs(0, 0)
				mv.visitEnd()

				cw.visitEnd()

				' load class
				Dim classFile As SByte() = cw.toByteArray()
				InvokerBytecodeGenerator.maybeDump(className, classFile)
				Dim bmhClass As Class = UNSAFE.defineClass(className, classFile, 0, classFile.Length, GetType(BoundMethodHandle).classLoader, Nothing).asSubclass(GetType(BoundMethodHandle))
					'UNSAFE.defineAnonymousClass(BoundMethodHandle.class, classFile, null).asSubclass(BoundMethodHandle.class);
				UNSAFE.ensureClassInitialized(bmhClass)

				Return bmhClass
			End Function

			Private Shared Function typeLoadOp(ByVal t As Char) As Integer
				Select Case t
				Case "L"c
					Return ALOAD
				Case "I"c
					Return ILOAD
				Case "J"c
					Return LLOAD
				Case "F"c
					Return FLOAD
				Case "D"c
					Return DLOAD
				Case Else
					Throw newInternalError("unrecognized type " & AscW(t))
				End Select
			End Function

			Private Shared Sub emitPushFields(ByVal types As String, ByVal className As String, ByVal mv As jdk.internal.org.objectweb.asm.MethodVisitor)
				For i As Integer = 0 To types.length() - 1
					Dim tc As Char = types.Chars(i)
					mv.visitVarInsn(ALOAD, 0)
					mv.visitFieldInsn(GETFIELD, className, makeFieldName(types, i), typeSig(tc))
				Next i
			End Sub

			Friend Shared Function typeSig(ByVal t As Char) As String
				Return If(t = "L"c, JLO_SIG, Convert.ToString(t))
			End Function

			'
			' Getter MH generation.
			'

			Private Shared Function makeGetter(ByVal cbmhClass As Class, ByVal types As String, ByVal index As Integer) As MethodHandle
				Dim fieldName As String = makeFieldName(types, index)
				Dim fieldType As Class = sun.invoke.util.Wrapper.forBasicType(types.Chars(index)).primitiveType()
				Try
					Return LOOKUP.findGetter(cbmhClass, fieldName, fieldType)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch NoSuchFieldException Or IllegalAccessException e
					Throw newInternalError(e)
				End Try
			End Function

			Friend Shared Function makeGetters(ByVal cbmhClass As Class, ByVal types As String, ByVal mhs As MethodHandle()) As MethodHandle()
				If mhs Is Nothing Then mhs = New MethodHandle(types.length() - 1){}
				For i As Integer = 0 To mhs.Length - 1
					mhs(i) = makeGetter(cbmhClass, types, i)
					assert(mhs(i).internalMemberName().declaringClass Is cbmhClass)
				Next i
				Return mhs
			End Function

			Friend Shared Function makeCtors(ByVal cbmh As Class, ByVal types As String, ByVal mhs As MethodHandle()) As MethodHandle()
				If mhs Is Nothing Then mhs = New MethodHandle(0){}
				If types.Equals("") Then ' hack for empty BMH species Return mhs
				mhs(0) = makeCbmhCtor(cbmh, types)
				Return mhs
			End Function

			Friend Shared Function makeNominalGetters(ByVal types As String, ByVal nfs As NamedFunction(), ByVal getters As MethodHandle()) As NamedFunction()
				If nfs Is Nothing Then nfs = New NamedFunction(types.length() - 1){}
				For i As Integer = 0 To nfs.Length - 1
					nfs(i) = New NamedFunction(getters(i))
				Next i
				Return nfs
			End Function

			'
			' Auxiliary methods.
			'

			Friend Shared Function speciesDataFromConcreteBMHClass(ByVal cbmh As Class) As SpeciesData
				Try
					Dim F_SPECIES_DATA As Field = cbmh.getDeclaredField("SPECIES_DATA")
					Return CType(F_SPECIES_DATA.get(Nothing), SpeciesData)
				Catch ex As ReflectiveOperationException
					Throw newInternalError(ex)
				End Try
			End Function

			''' <summary>
			''' Field names in concrete BMHs adhere to this pattern:
			''' arg + type + index
			''' where type is a single character (L, I, J, F, D).
			''' </summary>
			Private Shared Function makeFieldName(ByVal types As String, ByVal index As Integer) As String
				Debug.Assert(index >= 0 AndAlso index < types.length())
				Return "arg" & AscW(types.Chars(index)) + index
			End Function

			Private Shared Function makeSignature(ByVal types As String, ByVal ctor As Boolean) As String
				Dim buf As New StringBuilder(SIG_INCIPIT)
				For Each c As Char In types.ToCharArray()
					buf.append(typeSig(c))
				Next c
				Return buf.append(")"c).append(If(ctor, "V", BMH_SIG)).ToString()
			End Function

			Friend Shared Function makeCbmhCtor(ByVal cbmh As Class, ByVal types As String) As MethodHandle
				Try
					Return LOOKUP.findStatic(cbmh, "make", MethodType.fromMethodDescriptorString(makeSignature(types, False), Nothing))
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
				Catch NoSuchMethodException Or IllegalAccessException Or IllegalArgumentException Or TypeNotPresentException e
					Throw newInternalError(e)
				End Try
			End Function
		End Class

		Private Shared ReadOnly LOOKUP As Lookup = Lookup.IMPL_LOOKUP

		''' <summary>
		''' All subclasses must provide such a value describing their type signature.
		''' </summary>
		Friend Shared ReadOnly SPECIES_DATA As SpeciesData = SpeciesData.EMPTY

		Private Shared ReadOnly SPECIES_DATA_CACHE As SpeciesData() = New SpeciesData(4){}
		Private Shared Function checkCache(ByVal size As Integer, ByVal types As String) As SpeciesData
			Dim idx As Integer = size - 1
			Dim data As SpeciesData = SPECIES_DATA_CACHE(idx)
			If data IsNot Nothing Then Return data
				data = getSpeciesData(types)
				SPECIES_DATA_CACHE(idx) = data
			Return data
		End Function
		Friend Shared Function speciesData_L() As SpeciesData
			Return checkCache(1, "L")
		End Function
		Friend Shared Function speciesData_LL() As SpeciesData
			Return checkCache(2, "LL")
		End Function
		Friend Shared Function speciesData_LLL() As SpeciesData
			Return checkCache(3, "LLL")
		End Function
		Friend Shared Function speciesData_LLLL() As SpeciesData
			Return checkCache(4, "LLLL")
		End Function
		Friend Shared Function speciesData_LLLLL() As SpeciesData
			Return checkCache(5, "LLLLL")
		End Function
	 End Class

End Namespace