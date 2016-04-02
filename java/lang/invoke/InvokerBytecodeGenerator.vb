Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports jdk.internal.org.objectweb.asm
Imports java.lang.invoke.LambdaForm
Imports java.util

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' Code generation backend for LambdaForm.
    ''' <p>
    ''' @author John Rose, JSR 292 EG
    ''' </summary>
    Friend Class InvokerBytecodeGenerator
        ''' <summary>
        ''' Define class names for convenience. </summary>
        Private Const MH As String = "java/lang/invoke/MethodHandle"
        Private Const MHI As String = "java/lang/invoke/MethodHandleImpl"
        Private Const LF As String = "java/lang/invoke/LambdaForm"
        Private Const LFN As String = "java/lang/invoke/LambdaForm$Name"
        Private Const CLS As String = "java/lang/Class"
        Private Const OBJ As String = "java/lang/Object"
        Private Const OBJARY As String = "[Ljava/lang/Object;"

        Private Shared ReadOnly MH_SIG As String = "L" & MH & ";"
        Private Shared ReadOnly LF_SIG As String = "L" & LF & ";"
        Private Shared ReadOnly LFN_SIG As String = "L" & LFN & ";"
        Private Shared ReadOnly LL_SIG As String = "(L" & OBJ & ";)L" & OBJ & ";"
        Private Shared ReadOnly LLV_SIG As String = "(L" & OBJ & ";L" & OBJ & ";)V"
        Private Shared ReadOnly CLL_SIG As String = "(L" & CLS & ";L" & OBJ & ";)L" & OBJ & ";"

        ''' <summary>
        ''' Name of its super class </summary>
        Private Const superName As String = OBJ

        ''' <summary>
        ''' Name of new class </summary>
        Private ReadOnly className As String

        ''' <summary>
        ''' Name of the source file (for stack trace printing). </summary>
        Private ReadOnly sourceFile As String

        Private ReadOnly lambdaForm As LambdaForm
        Private ReadOnly invokerName As String
        Private ReadOnly invokerType As MethodType

        ''' <summary>
        ''' Info about local variables in compiled lambda form </summary>
        Private ReadOnly localsMap As Integer() ' index
        Private ReadOnly localTypes As BasicType() ' basic type
        Private ReadOnly localClasses As [Class]() ' type

        ''' <summary>
        ''' ASM bytecode generation. </summary>
        Private cw As [Class]Writer
		Private mv As MethodVisitor

        Private Shared ReadOnly MEMBERNAME_FACTORY As MemberName.Factory = MemberName.factory
        Private Shared ReadOnly HOST_CLASS As [Class] = GetType(LambdaForm)

        ''' <summary>
        ''' Main constructor; other constructors delegate to this one. </summary>
        Private Sub New(ByVal lambdaForm_Renamed As LambdaForm, ByVal localsMapSize As Integer, ByVal className As String, ByVal invokerName As String, ByVal invokerType As MethodType)
            If invokerName.Contains(".") Then
                Dim p As Integer = invokerName.IndexOf(".")
                className = invokerName.Substring(0, p)
                invokerName = invokerName.Substring(p + 1)
            End If
            If DUMP_CLASS_FILES Then className = makeDumpableClassName(className)
            Me.className = LF & "$" & className
            Me.sourceFile = "LambdaForm$" & className
            Me.lambdaForm = lambdaForm_Renamed
            Me.invokerName = invokerName
            Me.invokerType = invokerType
            Me.localsMap = New Integer(localsMapSize) {}
            ' last entry of localsMap is count of allocated local slots
            Me.localTypes = New BasicType(localsMapSize) {}
            Me.localClasses = New [Class](localsMapSize) {}
        End Sub

        ''' <summary>
        ''' For generating LambdaForm interpreter entry points. </summary>
        Private Sub New(ByVal className As String, ByVal invokerName As String, ByVal invokerType As MethodType)
            Me.New(Nothing, invokerType.parameterCount(), className, invokerName, invokerType)
            ' Create an array to map name indexes to locals indexes.
            localTypes(localTypes.Length - 1) = V_TYPE
            For i As Integer = 0 To localsMap.Length - 1
                localsMap(i) = invokerType.parameterSlotCount() - invokerType.parameterSlotDepth(i)
                If i < invokerType.parameterCount() Then localTypes(i) = BasicType(invokerType.parameterType(i))
            Next i
        End Sub

        ''' <summary>
        ''' For generating customized code for a single LambdaForm. </summary>
        Private Sub New(ByVal className As String, ByVal form As LambdaForm, ByVal invokerType As MethodType)
            Me.New(form, form.names.Length, className, form.debugName, invokerType)
            ' Create an array to map name indexes to locals indexes.
            Dim names As Name() = form.names
            Dim i As Integer = 0
            Dim index As Integer = 0
            Do While i < localsMap.Length
                localsMap(i) = index
                If i < names.Length Then
                    Dim type As BasicType = names(i).type()
                    index += type.basicTypeSlots()
                    localTypes(i) = type
                End If
                i += 1
            Loop
        End Sub


        ''' <summary>
        ''' instance counters for dumped classes </summary>
        Private Shared ReadOnly DUMP_CLASS_FILES_COUNTERS As HashMap(Of String, Integer?)
        ''' <summary>
        ''' debugging flag for saving generated class files </summary>
        Private Shared ReadOnly DUMP_CLASS_FILES_DIR As File

        Shared Sub New()
            If DUMP_CLASS_FILES Then
                DUMP_CLASS_FILES_COUNTERS = New HashMap(Of )
                Try
                    Dim dumpDir As New File("DUMP_CLASS_FILES")
                    If Not dumpDir.exists() Then dumpDir.mkdirs()
                    DUMP_CLASS_FILES_DIR = dumpDir
                    Console.WriteLine("Dumping class files to " & DUMP_CLASS_FILES_DIR & "/...")
                Catch e As Exception
                    Throw newInternalError(e)
                End Try
            Else
                DUMP_CLASS_FILES_COUNTERS = Nothing
                DUMP_CLASS_FILES_DIR = Nothing
            End If
        End Sub

        Friend Shared Sub maybeDump(ByVal className As String, ByVal classFile As SByte())
            If DUMP_CLASS_FILES Then java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                Try
                    Dim dumpName As String = outerInstance.className
                    'dumpName = dumpName.replace('/', '-');
                    Dim dumpFile As New File(DUMP_CLASS_FILES_DIR, dumpName & ".class")
                    Console.WriteLine("dump: " & dumpFile)
                    dumpFile.parentFile.mkdirs()
                    Dim file_Renamed As New FileOutputStream(dumpFile)
                    file_Renamed.write(classFile)
                    file_Renamed.close()
                    Return Nothing
                Catch ex As IOException
                    Throw newInternalError(ex)
                End Try
            End Function
        End Class

        Private Shared Function makeDumpableClassName(ByVal className As String) As String
            Dim ctr As Integer?
            SyncLock DUMP_CLASS_FILES_COUNTERS
                ctr = DUMP_CLASS_FILES_COUNTERS.get(className)
                If ctr Is Nothing Then ctr = 0
                DUMP_CLASS_FILES_COUNTERS.put(className, ctr + 1)
            End SyncLock
            Dim sfx As String = ctr.ToString()
            Do While sfx.Length() < 3
                sfx = "0" & sfx
            Loop
            className += sfx
            Return className
        End Function

        Friend Class CpPatch : Inherits java.lang.Object
            Private ReadOnly outerInstance As InvokerBytecodeGenerator

            Friend ReadOnly index As Integer
            Friend ReadOnly placeholder As String
            Friend ReadOnly value As Object
            Friend Sub New(ByVal outerInstance As InvokerBytecodeGenerator, ByVal index As Integer, ByVal placeholder As String, ByVal value As Object)
                Me.outerInstance = outerInstance
                Me.index = index
                Me.placeholder = placeholder
                Me.value = value
            End Sub
            Public Overrides Function ToString() As String
                Return "CpPatch/index=" & index & ",placeholder=" & placeholder & ",value=" & value
            End Function
        End Class

        Friend cpPatches_Renamed As Map(Of Object, CpPatch) = New HashMap(Of Object, CpPatch)

        Friend cph As Integer = 0 ' for counting constant placeholders

        Friend Overridable Function constantPlaceholder(ByVal arg As Object) As String
            Dim cpPlaceholder As String = "CONSTANT_PLACEHOLDER_" & cph
            cph += 1
            If DUMP_CLASS_FILES Then ' debugging aid cpPlaceholder &= " <<" & debugString(arg) & ">>"
                If cpPatches_Renamed.containsKey(cpPlaceholder) Then Throw New InternalError("observed CP placeholder twice: " & cpPlaceholder)
                ' insert placeholder in CP and remember the patch
                Dim index As Integer = cw.newConst(CObj(cpPlaceholder)) ' TODO check if aready in the constant pool
                cpPatches_Renamed.put(cpPlaceholder, New CpPatch(Me, index, cpPlaceholder, arg))
                Return cpPlaceholder
        End Function

        Friend Overridable Function cpPatches(ByVal classFile As SByte()) As Object()
            Dim size As Integer = getConstantPoolSize(classFile)
            Dim res As Object() = New Object(size - 1) {}
            For Each p As CpPatch In cpPatches_Renamed.values()
                If p.index >= size Then Throw New InternalError("in cpool[" & size & "]: " & p & vbLf & Arrays.ToString(Arrays.copyOf(classFile, 20)))
                res(p.index) = p.value
            Next p
            Return res
        End Function

        Private Shared Function debugString(ByVal arg As Object) As String
            If TypeOf arg Is MethodHandle Then
                Dim mh_Renamed As MethodHandle = CType(arg, MethodHandle)
                Dim member As MemberName = mh_Renamed.internalMemberName()
                If member IsNot Nothing Then Return member.ToString()
                Return mh_Renamed.debugString()
            End If
            Return arg.ToString()
        End Function

        ''' <summary>
        ''' Extract the number of constant pool entries from a given class file.
        ''' </summary>
        ''' <param name="classFile"> the bytes of the class file in question. </param>
        ''' <returns> the number of entries in the constant pool. </returns>
        Private Shared Function getConstantPoolSize(ByVal classFile As SByte()) As Integer
            ' The first few bytes:
            ' u4 magic;
            ' u2 minor_version;
            ' u2 major_version;
            ' u2 constant_pool_count;
            Return ((classFile(8) And &HFF) << 8) Or (classFile(9) And &HFF)
        End Function

        ''' <summary>
        ''' Extract the MemberName of a newly-defined method.
        ''' </summary>
        Private Function loadMethod(ByVal classFile As SByte()) As MemberName
            Dim invokerClass As [Class] = loadAndInitializeInvokerClass(classFile, cpPatches(classFile))
            Return resolveInvokerMember(invokerClass, invokerName, invokerType)
        End Function

        ''' <summary>
        ''' Define a given class as anonymous class in the runtime system.
        ''' </summary>
        Private Shared Function loadAndInitializeInvokerClass(ByVal classBytes As SByte(), ByVal patches As Object()) As [Class]
            Dim invokerClass As [Class] = UNSAFE.defineAnonymousClass(HOST_CLASS, classBytes, patches)
            UNSAFE.ensureClassInitialized(invokerClass) ' Make sure the class is initialized; VM might complain.
            Return invokerClass
        End Function

        Private Shared Function resolveInvokerMember(ByVal invokerClass As [Class], ByVal name As String, ByVal type As MethodType) As MemberName
            Dim member As New MemberName(invokerClass, name, type, REF_invokeStatic)
            'System.out.println("resolveInvokerMember => "+member);
            'for (Method m : invokerClass.getDeclaredMethods())  System.out.println("  "+m);
            Try
                member = MEMBERNAME_FACTORY.resolveOrFail(REF_invokeStatic, member, HOST_CLASS, GetType(ReflectiveOperationException))
            Catch e As ReflectiveOperationException
                Throw New InternalError(e)
            End Try
            'System.out.println("resolveInvokerMember => "+member);
            Return member
        End Function

        ''' <summary>
        ''' Set up class file generation.
        ''' </summary>
        Private Sub classFilePrologue()
            Const NOT_ACC_PUBLIC As Integer = 0 ' not ACC_PUBLIC
            cw = New ClassWriter(ClassWriter.COMPUTE_MAXS + ClassWriter.COMPUTE_FRAMES)
            cw.visit(Opcodes.V1_8, NOT_ACC_PUBLIC + Opcodes.ACC_FINAL + Opcodes.ACC_SUPER, className, Nothing, superName, Nothing)
            cw.visitSource(sourceFile, Nothing)

            Dim invokerDesc As String = invokerType.toMethodDescriptorString()
            mv = cw.visitMethod(Opcodes.ACC_STATIC, invokerName, invokerDesc, Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Tear down class file generation.
        ''' </summary>
        Private Sub classFileEpilogue()
            mv.visitMaxs(0, 0)
            mv.visitEnd()
        End Sub

        '
        '     * Low-level emit helpers.
        '
        Private Sub emitConst(ByVal con As Object)
            If con Is Nothing Then
                mv.visitInsn(Opcodes.ACONST_NULL)
                Return
            End If
            If TypeOf con Is Integer? Then
                emitIconstInsn(CInt(Fix(con)))
                Return
            End If
            If TypeOf con Is Long? Then
                Dim x As Long = CLng(Fix(con))
                If x = CShort(x) Then
                    emitIconstInsn(CInt(x))
                    mv.visitInsn(Opcodes.I2L)
                    Return
                End If
            End If
            If TypeOf con Is Float Then
                Dim x As Single = CSng(con)
                If x = CShort(Fix(x)) Then
                    emitIconstInsn(CInt(Fix(x)))
                    mv.visitInsn(Opcodes.I2F)
                    Return
                End If
            End If
            If TypeOf con Is Double? Then
                Dim x As Double = CDbl(con)
                If x = CShort(Fix(x)) Then
                    emitIconstInsn(CInt(Fix(x)))
                    mv.visitInsn(Opcodes.I2D)
                    Return
                End If
            End If
            If TypeOf con Is Boolean? Then
                emitIconstInsn(If(CBool(con), 1, 0))
                Return
            End If
            ' fall through:
            mv.visitLdcInsn(con)
        End Sub

        Private Sub emitIconstInsn(ByVal i As Integer)
            Dim opcode As Integer
            Select Case i
                Case 0
                    opcode = Opcodes.ICONST_0
                Case 1
                    opcode = Opcodes.ICONST_1
                Case 2
                    opcode = Opcodes.ICONST_2
                Case 3
                    opcode = Opcodes.ICONST_3
                Case 4
                    opcode = Opcodes.ICONST_4
                Case 5
                    opcode = Opcodes.ICONST_5
                Case Else
                    If i = CByte(i) Then
                        mv.visitIntInsn(Opcodes.BIPUSH, i And &HFF)
                    ElseIf i = CShort(i) Then
                        mv.visitIntInsn(Opcodes.SIPUSH, ChrW(i))
                    Else
                        mv.visitLdcInsn(i)
                    End If
                    Return
            End Select
            mv.visitInsn(opcode)
        End Sub

        '
        '     * NOTE: These load/store methods use the localsMap to find the correct index!
        '
        Private Sub emitLoadInsn(ByVal type As BasicType, ByVal index As Integer)
            Dim opcode As Integer = loadInsnOpcode(type)
            mv.visitVarInsn(opcode, localsMap(index))
        End Sub

        Private Function loadInsnOpcode(ByVal type As BasicType) As Integer
            Select Case type
                Case I_TYPE
                    Return Opcodes.ILOAD
                Case J_TYPE
                    Return Opcodes.LLOAD
                Case F_TYPE
                    Return Opcodes.FLOAD
                Case D_TYPE
                    Return Opcodes.DLOAD
                Case L_TYPE
                    Return Opcodes.ALOAD
                Case Else
                    Throw New InternalError("unknown type: " & type)
            End Select
        End Function
        Private Sub emitAloadInsn(ByVal index As Integer)
            emitLoadInsn(L_TYPE, index)
        End Sub

        Private Sub emitStoreInsn(ByVal type As BasicType, ByVal index As Integer)
            Dim opcode As Integer = storeInsnOpcode(type)
            mv.visitVarInsn(opcode, localsMap(index))
        End Sub

        Private Function storeInsnOpcode(ByVal type As BasicType) As Integer
            Select Case type
                Case I_TYPE
                    Return Opcodes.ISTORE
                Case J_TYPE
                    Return Opcodes.LSTORE
                Case F_TYPE
                    Return Opcodes.FSTORE
                Case D_TYPE
                    Return Opcodes.DSTORE
                Case L_TYPE
                    Return Opcodes.ASTORE
                Case Else
                    Throw New InternalError("unknown type: " & type)
            End Select
        End Function
        Private Sub emitAstoreInsn(ByVal index As Integer)
            emitStoreInsn(L_TYPE, index)
        End Sub

        Private Function arrayTypeCode(ByVal elementType As sun.invoke.util.Wrapper) As SByte
            Select Case elementType
                Case Boolean
                    Return Opcodes.T_BOOLEAN
                Case Byte
                    Return Opcodes.T_BYTE
                Case Char
                    Return Opcodes.T_CHAR
                Case Short
                    Return Opcodes.T_SHORT
                Case Int()
                    Return Opcodes.T_INT
                Case Long
                    Return Opcodes.T_LONG
                Case Float
                    Return Opcodes.T_FLOAT
                Case Double
                    Return Opcodes.T_DOUBLE
                Case Object ' in place of Opcodes.T_OBJECT
                    Return 0
                Case Else
                    Throw New InternalError
            End Select
        End Function

        Private Function arrayInsnOpcode(ByVal tcode As SByte, ByVal aaop As Integer) As Integer
            Assert(aaop = Opcodes.AASTORE OrElse aaop = Opcodes.AALOAD)
            Dim xas As Integer
            Select Case tcode
                Case Opcodes.T_BOOLEAN
                    xas = Opcodes.BASTORE
                Case Opcodes.T_BYTE
                    xas = Opcodes.BASTORE
                Case Opcodes.T_CHAR
                    xas = Opcodes.CASTORE
                Case Opcodes.T_SHORT
                    xas = Opcodes.SASTORE
                Case Opcodes.T_INT
                    xas = Opcodes.IASTORE
                Case Opcodes.T_LONG
                    xas = Opcodes.LASTORE
                Case Opcodes.T_FLOAT
                    xas = Opcodes.FASTORE
                Case Opcodes.T_DOUBLE
                    xas = Opcodes.DASTORE
                Case 0
                    xas = Opcodes.AASTORE
                Case Else
                    Throw New InternalError
            End Select
            Return xas - Opcodes.AASTORE + aaop
        End Function


        Private Sub freeFrameLocal(ByVal oldFrameLocal As Integer)
            Dim i As Integer = indexForFrameLocal(oldFrameLocal)
            If i < 0 Then Return
            Dim type As BasicType = localTypes(i)
            Dim newFrameLocal As Integer = makeLocalTemp(type)
            mv.visitVarInsn(loadInsnOpcode(type), oldFrameLocal)
            mv.visitVarInsn(storeInsnOpcode(type), newFrameLocal)
            Assert(localsMap(i) = oldFrameLocal)
            localsMap(i) = newFrameLocal
            Assert(indexForFrameLocal(oldFrameLocal) < 0)
        End Sub
        Private Function indexForFrameLocal(ByVal frameLocal As Integer) As Integer
            For i As Integer = 0 To localsMap.Length - 1
                If localsMap(i) = frameLocal AndAlso localTypes(i) IsNot V_TYPE Then Return i
            Next i
            Return -1
        End Function
        Private Function makeLocalTemp(ByVal type As BasicType) As Integer
            Dim frameLocal As Integer = localsMap(localsMap.Length - 1)
            localsMap(localsMap.Length - 1) = frameLocal + type.basicTypeSlots()
            Return frameLocal
        End Function

        ''' <summary>
        ''' Emit a boxing call.
        ''' </summary>
        ''' <param name="wrapper"> primitive type class to box. </param>
        Private Sub emitBoxing(ByVal wrapper As sun.invoke.util.Wrapper)
            Dim owner As String = "java/lang/" & wrapper.wrapperType().simpleName
            Dim name As String = "valueOf"
            Dim desc As String = "(" & wrapper.basicTypeChar() & ")L" & owner & ";"
            mv.visitMethodInsn(Opcodes.INVOKESTATIC, owner, name, desc, False)
        End Sub

        ''' <summary>
        ''' Emit an unboxing call (plus preceding checkcast).
        ''' </summary>
        ''' <param name="wrapper"> wrapper type class to unbox. </param>
        Private Sub emitUnboxing(ByVal wrapper As sun.invoke.util.Wrapper)
            Dim owner As String = "java/lang/" & wrapper.wrapperType().simpleName
            Dim name As String = wrapper.primitiveSimpleName() & "Value"
            Dim desc As String = "()" & wrapper.basicTypeChar()
            emitReferenceCast(wrapper.wrapperType(), Nothing)
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, owner, name, desc, False)
        End Sub

        ''' <summary>
        ''' Emit an implicit conversion for an argument which must be of the given pclass.
        ''' This is usually a no-op, except when pclass is a subword type or a reference other than Object or an interface.
        ''' </summary>
        ''' <param name="ptype"> type of value present on stack </param>
        ''' <param name="pclass"> type of value required on stack </param>
        ''' <param name="arg"> compile-time representation of value on stack (Node, constant) or null if none </param>
        Private Sub emitImplicitConversion(ByVal ptype As BasicType, ByVal pclass As [Class], ByVal arg As Object)
            Assert(BasicType(pclass) Is ptype) ' boxing/unboxing handled by caller
            If pclass Is ptype.basicTypeClass() AndAlso ptype IsNot L_TYPE Then Return ' nothing to do
            Select Case ptype
                Case L_TYPE
                    If sun.invoke.util.VerifyType.isNullConversion(GetType(Object), pclass, False) Then
                        If PROFILE_LEVEL > 0 Then emitReferenceCast(GetType(Object), arg)
                        Return
                    End If
                    emitReferenceCast(pclass, arg)
                    Return
                Case I_TYPE
                    If Not sun.invoke.util.VerifyType.isNullConversion(GetType(Integer), pclass, False) Then emitPrimCast(ptype.basicTypeWrapper(), sun.invoke.util.Wrapper.forPrimitiveType(pclass))
                    Return
            End Select
            Throw New InternalError("bad implicit conversion: tc=" & ptype & ": " & pclass)
        End Sub

        ''' <summary>
        ''' Update localClasses type map.  Return true if the information is already present. </summary>
        Private Function assertStaticType(ByVal cls As [Class], ByVal n As Name) As Boolean
            Dim local As Integer = n.index()
            Dim aclass As [Class] = localClasses(local)
            If aclass IsNot Nothing AndAlso (aclass Is cls OrElse aclass.IsSubclassOf(cls)) Then
                Return True ' type info is already present
            ElseIf aclass Is Nothing OrElse cls.IsSubclassOf(aclass) Then
                localClasses(local) = cls ' type info can be improved
            End If
            Return False
        End Function

        Private Sub emitReferenceCast(ByVal cls As [Class], ByVal arg As Object)
            Dim writeBack As Name = Nothing ' local to write back result
            If TypeOf arg Is Name Then
                Dim n As Name = CType(arg, Name)
                If assertStaticType(cls, n) Then Return ' this cast was already performed
                If lambdaForm.useCount(n) > 1 Then writeBack = n
            End If
            If isStaticallyNameable(cls) Then
                Dim sig As String = getInternalName(cls)
                mv.visitTypeInsn(Opcodes.CHECKCAST, sig)
            Else
                mv.visitLdcInsn(constantPlaceholder(cls))
                mv.visitTypeInsn(Opcodes.CHECKCAST, InvokerBytecodeGenerator.CLS)
                mv.visitInsn(Opcodes.SWAP)
                mv.visitMethodInsn(Opcodes.INVOKESTATIC, MHI, "castReference", CLL_SIG, False)
                If cls.IsSubclassOf(GetType(Object())) Then
                    mv.visitTypeInsn(Opcodes.CHECKCAST, OBJARY)
                ElseIf PROFILE_LEVEL > 0 Then
                    mv.visitTypeInsn(Opcodes.CHECKCAST, OBJ)
                End If
            End If
            If writeBack IsNot Nothing Then
                mv.visitInsn(Opcodes.DUP)
                emitAstoreInsn(writeBack.index())
            End If
        End Sub

        ''' <summary>
        ''' Emits an actual return instruction conforming to the given return type.
        ''' </summary>
        Private Sub emitReturnInsn(ByVal type As BasicType)
            Dim opcode As Integer
            Select Case type
                Case BasicType.I_TYPE
                    opcode = Opcodes.IRETURN
                Case BasicType.J_TYPE
                    opcode = Opcodes.LRETURN
                Case BasicType.F_TYPE
                    opcode = Opcodes.FRETURN
                Case BasicType.D_TYPE
                    opcode = Opcodes.DRETURN
                Case BasicType.L_TYPE
                    opcode = Opcodes.ARETURN
                Case BasicType.V_TYPE
                    opcode = Opcodes.RETURN
                Case Else
                    Throw New InternalError("unknown return type: " & type)
            End Select
            mv.visitInsn(opcode)
        End Sub

        Private Shared Function getInternalName(ByVal c As [Class]) As String
            If c Is GetType(Object) Then
                Return OBJ
            ElseIf c Is GetType(Object()) Then
                Return OBJARY
            ElseIf c Is GetType(Class) Then
                Return CLS
            ElseIf c Is GetType(MethodHandle) Then
                Return MH
            End If
            Assert(sun.invoke.util.VerifyAccess.isTypeVisible(c, GetType(Object))) : c.name
            Return c.name.Replace("."c, "/"c)
        End Function

        ''' <summary>
        ''' Generate customized bytecode for a given LambdaForm.
        ''' </summary>
        Friend Shared Function generateCustomizedCode(ByVal form As LambdaForm, ByVal invokerType As MethodType) As MemberName
            Dim g As New InvokerBytecodeGenerator("MH", form, invokerType)
            Return g.loadMethod(g.generateCustomizedCodeBytes())
        End Function

        ''' <summary>
        ''' Generates code to check that actual receiver and LambdaForm matches </summary>
        Private Function checkActualReceiver() As Boolean
            ' Expects MethodHandle on the stack and actual receiver MethodHandle in slot #0
            mv.visitInsn(Opcodes.DUP)
            mv.visitVarInsn(Opcodes.ALOAD, localsMap(0))
            mv.visitMethodInsn(Opcodes.INVOKESTATIC, MHI, "assertSame", LLV_SIG, False)
            Return True
        End Function

        ''' <summary>
        ''' Generate an invoker method for the passed <seealso cref="LambdaForm"/>.
        ''' </summary>
        Private Function generateCustomizedCodeBytes() As SByte()
            classFilePrologue()

            ' Suppress this method in backtraces displayed to the user.
            mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", True)

            ' Mark this method as a compiled LambdaForm
            mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Compiled;", True)

            If lambdaForm.forceInline Then
                ' Force inlining of this invoker method.
                mv.visitAnnotation("Ljava/lang/invoke/ForceInline;", True)
            Else
                mv.visitAnnotation("Ljava/lang/invoke/DontInline;", True)
            End If

            If lambdaForm.customized IsNot Nothing Then
                ' Since LambdaForm is customized for a particular MethodHandle, it's safe to substitute
                ' receiver MethodHandle (at slot #0) with an embedded constant and use it instead.
                ' It enables more efficient code generation in some situations, since embedded constants
                ' are compile-time constants for JIT compiler.
                mv.visitLdcInsn(constantPlaceholder(lambdaForm.customized))
                mv.visitTypeInsn(Opcodes.CHECKCAST, MH)
                Assert(checkActualReceiver()) ' expects MethodHandle on top of the stack
                mv.visitVarInsn(Opcodes.ASTORE, localsMap(0))
            End If

            ' iterate over the form's names, generating bytecode instructions for each
            ' start iterating at the first name following the arguments
            Dim onStack As Name = Nothing
            For i As Integer = lambdaForm.arity_Renamed To lambdaForm.names.Length - 1
                Dim name As Name = lambdaForm.names(i)

                emitStoreResult(onStack)
                onStack = name ' unless otherwise modified below
                Dim intr As MethodHandleImpl.Intrinsic = name.function.intrinsicName()
                Select Case intr
                    Case SELECT_ALTERNATIVE
                        Debug.Assert(isSelectAlternative(i))
                        If PROFILE_GWT Then
                            Assert(TypeOf name.arguments(0) Is Name AndAlso nameRefersTo(CType(name.arguments(0), Name), GetType(MethodHandleImpl), "profileBoolean"))
                            mv.visitAnnotation("Ljava/lang/invoke/InjectedProfile;", True)
                        End If
                        onStack = emitSelectAlternative(name, lambdaForm.names(i + 1))
                        i += 1 ' skip MH.invokeBasic of the selectAlternative result
                        Continue For
                    Case GUARD_WITH_CATCH
                        Debug.Assert(isGuardWithCatch(i))
                        onStack = emitGuardWithCatch(i)
                        i = i + 2 ' Jump to the end of GWC idiom
                        Continue For
                    Case NEW_ARRAY
                        Dim rtype As [Class] = name.function.methodType().returnType()
                        If isStaticallyNameable(rtype) Then
                            emitNewArray(name)
                            Continue For
                        End If
                    Case ARRAY_LOAD
                        emitArrayLoad(name)
                        Continue For
                    Case ARRAY_STORE
                        emitArrayStore(name)
                        Continue For
                    Case identity()
                        Assert(name.arguments.Length = 1)
                        emitPushArguments(name)
                        Continue For
                    Case ZERO
                        Assert(name.arguments.Length = 0)
                        emitConst(name.type.basicTypeWrapper().zero())
                        Continue For
                    Case NONE
                        ' no intrinsic associated
                    Case Else
                        Throw New InternalError("Unknown intrinsic: " & intr)
                End Select

                Dim member As MemberName = name.function.member()
                If isStaticallyInvocable(member) Then
                    emitStaticInvoke(member, name)
                Else
                    emitInvoke(name)
                End If
            Next i

            ' return statement
            emitReturn(onStack)

            classFileEpilogue()
            bogusMethod(lambdaForm)

            Dim classFile As SByte() = cw.toByteArray()
            maybeDump(className, classFile)
            Return classFile
        End Function

        Friend Overridable Sub emitArrayLoad(ByVal name As Name)
            emitArrayOp(name, Opcodes.AALOAD)
        End Sub
        Friend Overridable Sub emitArrayStore(ByVal name As Name)
            emitArrayOp(name, Opcodes.AASTORE)
        End Sub

        Friend Overridable Sub emitArrayOp(ByVal name As Name, ByVal arrayOpcode As Integer)
            Debug.Assert(arrayOpcode = Opcodes.AALOAD OrElse arrayOpcode = Opcodes.AASTORE)
            Dim elementType As [Class] = name.function.methodType().parameterType(0).componentType
            Debug.Assert(elementType IsNot Nothing)
            emitPushArguments(name)
            If elementType.primitive Then
                Dim w As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(elementType)
                arrayOpcode = arrayInsnOpcode(arrayTypeCode(w), arrayOpcode)
            End If
            mv.visitInsn(arrayOpcode)
        End Sub

        ''' <summary>
        ''' Emit an invoke for the given name.
        ''' </summary>
        Friend Overridable Sub emitInvoke(ByVal name As Name)
            Assert((Not isLinkerMethodInvoke(name))) ' should use the static path for these
            If True Then
                ' push receiver
                Dim target As MethodHandle = name.function.resolvedHandle
                Assert(target IsNot Nothing) : name.exprString()
                mv.visitLdcInsn(constantPlaceholder(target))
                emitReferenceCast(GetType(MethodHandle), target)
            Else
                ' load receiver
                emitAloadInsn(0)
                emitReferenceCast(GetType(MethodHandle), Nothing)
                mv.visitFieldInsn(Opcodes.GETFIELD, MH, "form", LF_SIG)
                mv.visitFieldInsn(Opcodes.GETFIELD, LF, "names", LFN_SIG)
                ' TODO more to come
            End If

            ' push arguments
            emitPushArguments(name)

            ' invocation
            Dim type As MethodType = name.function.methodType()
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", type.basicType().toMethodDescriptorString(), False)
        End Sub

        Private Shared STATICALLY_INVOCABLE_PACKAGES As [Class]() = {GetType(Object), GetType(Arrays), GetType(sun.misc.Unsafe)}

        Friend Shared Function isStaticallyInvocable(ByVal name As Name) As Boolean
            Return isStaticallyInvocable(name.function.member())
        End Function

        Friend Shared Function isStaticallyInvocable(ByVal member As MemberName) As Boolean
            If member Is Nothing Then Return False
            If member.constructor Then Return False
            Dim cls_Renamed As [Class] = member.declaringClass
            If cls_Renamed.array OrElse cls_Renamed.primitive Then Return False ' FIXME
            If cls_Renamed.anonymousClass OrElse cls_Renamed.localClass Then Return False ' inner class of some sort
            If cls_Renamed.classLoader IsNot GetType(MethodHandle).classLoader Then Return False ' not on BCP
            If sun.reflect.misc.ReflectUtil.isVMAnonymousClass(cls_Renamed) Then Return False  ' FIXME: switch to supported API once it is added Return False
            Dim mtype As MethodType = member.methodOrFieldType
            If Not isStaticallyNameable(mtype.returnType()) Then Return False
            For Each ptype As [Class] In mtype.parameterArray()
                If Not isStaticallyNameable(ptype) Then Return False
            Next ptype
            If (Not member.private) AndAlso sun.invoke.util.VerifyAccess.isSamePackage(GetType(MethodHandle), cls_Renamed) Then Return True ' in java.lang.invoke package
            If member.public AndAlso isStaticallyNameable(cls_Renamed) Then Return True
            Return False
        End Function

        Friend Shared Function isStaticallyNameable(ByVal cls As [Class]) As Boolean
            If cls Is GetType(Object) Then Return True
            Do While cls.array
                cls = cls.componentType
            Loop
            If cls.primitive Then Return True ' int[].class, for example
            If sun.reflect.misc.ReflectUtil.isVMAnonymousClass(cls) Then ' FIXME: switch to supported API once it is added Return False
                ' could use VerifyAccess.isClassAccessible but the following is a safe approximation
                If cls.classLoader IsNot GetType(Object).classLoader Then Return False
                If sun.invoke.util.VerifyAccess.isSamePackage(GetType(MethodHandle), cls) Then Return True
                If Not Modifier.isPublic(cls.modifiers) Then Return False
                For Each pkgcls As [Class] In STATICALLY_INVOCABLE_PACKAGES
                    If sun.invoke.util.VerifyAccess.isSamePackage(pkgcls, cls) Then Return True
                Next pkgcls
                Return False
        End Function

        Friend Overridable Sub emitStaticInvoke(ByVal name As Name)
            emitStaticInvoke(name.function.member(), name)
        End Sub

        ''' <summary>
        ''' Emit an invoke for the given name, using the MemberName directly.
        ''' </summary>
        Friend Overridable Sub emitStaticInvoke(ByVal member As MemberName, ByVal name As Name)
            Assert(member.Equals(name.function.member()))
            Dim defc As [Class] = member.declaringClass
            Dim cname As String = getInternalName(defc)
            Dim mname As String = member.name
            Dim mtype As String
            Dim refKind As SByte = member.referenceKind
            If refKind = REF_invokeSpecial Then
                ' in order to pass the verifier, we need to convert this to invokevirtual in all cases
                Assert(member.canBeStaticallyBound()) : member
                refKind = REF_invokeVirtual
            End If

            If member.declaringClass.interface AndAlso refKind = REF_invokeVirtual Then refKind = REF_invokeInterface

            ' push arguments
            emitPushArguments(name)

            ' invocation
            If member.method Then
                mtype = member.methodType.toMethodDescriptorString()
                mv.visitMethodInsn(refKindOpcode(refKind), cname, mname, mtype, member.declaringClass.interface)
            Else
                mtype = methodType.toFieldDescriptorString(member.fieldType)
                mv.visitFieldInsn(refKindOpcode(refKind), cname, mname, mtype)
            End If
            ' Issue a type assertion for the result, so we can avoid casts later.
            If name.type = L_TYPE Then
                Dim rtype As [Class] = member.invocationType.returnType()
                Assert((Not rtype.primitive))
                If rtype IsNot GetType(Object) AndAlso (Not rtype.interface) Then assertStaticType(rtype, name)
            End If
        End Sub

        Friend Overridable Sub emitNewArray(ByVal name As Name)
            Dim rtype As [Class] = name.function.methodType().returnType()
            If name.arguments.Length = 0 Then
                ' The array will be a constant.
                Dim emptyArray As Object
                Try
                    emptyArray = name.function.resolvedHandle.invoke()
                Catch ex As Throwable
                    Throw newInternalError(ex)
                End Try
                Assert(java.lang.reflect.Array.getLength(emptyArray) = 0)
                Assert(emptyArray.GetType() Is rtype) ' exact typing
                mv.visitLdcInsn(constantPlaceholder(emptyArray))
                emitReferenceCast(rtype, emptyArray)
                Return
            End If
            Dim arrayElementType As [Class] = rtype.componentType
            Assert(arrayElementType IsNot Nothing)
            emitIconstInsn(name.arguments.Length)
            Dim xas As Integer = Opcodes.AASTORE
            If Not arrayElementType.primitive Then
                mv.visitTypeInsn(Opcodes.ANEWARRAY, getInternalName(arrayElementType))
            Else
                Dim tc As SByte = arrayTypeCode(sun.invoke.util.Wrapper.forPrimitiveType(arrayElementType))
                xas = arrayInsnOpcode(tc, xas)
                mv.visitIntInsn(Opcodes.NEWARRAY, tc)
            End If
            ' store arguments
            For i As Integer = 0 To name.arguments.Length - 1
                mv.visitInsn(Opcodes.DUP)
                emitIconstInsn(i)
                emitPushArgument(name, i)
                mv.visitInsn(xas)
            Next i
            ' the array is left on the stack
            assertStaticType(rtype, name)
        End Sub
        Friend Overridable Function refKindOpcode(ByVal refKind As SByte) As Integer
            Select Case refKind
                Case REF_invokeVirtual
                    Return Opcodes.INVOKEVIRTUAL
                Case REF_invokeStatic
                    Return Opcodes.INVOKESTATIC
                Case REF_invokeSpecial
                    Return Opcodes.INVOKESPECIAL
                Case REF_invokeInterface
                    Return Opcodes.INVOKEINTERFACE
                Case REF_getField
                    Return Opcodes.GETFIELD
                Case REF_putField
                    Return Opcodes.PUTFIELD
                Case REF_getStatic
                    Return Opcodes.GETSTATIC
                Case REF_putStatic
                    Return Opcodes.PUTSTATIC
            End Select
            Throw New InternalError("refKind=" & refKind)
        End Function

        ''' <summary>
        ''' Check if MemberName is a call to a method named {@code name} in class {@code declaredClass}.
        ''' </summary>
        Private Function memberRefersTo(ByVal member As MemberName, ByVal declaringClass As [Class], ByVal name As String) As Boolean
            Return member IsNot Nothing AndAlso member.declaringClass Is declaringClass AndAlso member.name.Equals(name)
        End Function
        Private Function nameRefersTo(ByVal name As Name, ByVal declaringClass As [Class], ByVal methodName As String) As Boolean
            Return name.function IsNot Nothing AndAlso memberRefersTo(name.function.member(), declaringClass, methodName)
        End Function

        ''' <summary>
        ''' Check if MemberName is a call to MethodHandle.invokeBasic.
        ''' </summary>
        Private Function isInvokeBasic(ByVal name As Name) As Boolean
            If name.function Is Nothing Then Return False
            If name.arguments.Length < 1 Then Return False ' must have MH argument
            Dim member As MemberName = name.function.member()
            Return memberRefersTo(member, GetType(MethodHandle), "invokeBasic") AndAlso (Not member.public) AndAlso Not member.static
        End Function

        ''' <summary>
        ''' Check if MemberName is a call to MethodHandle.linkToStatic, etc.
        ''' </summary>
        Private Function isLinkerMethodInvoke(ByVal name As Name) As Boolean
            If name.function Is Nothing Then Return False
            If name.arguments.Length < 1 Then Return False ' must have MH argument
            Dim member As MemberName = name.function.member()
            Return member IsNot Nothing AndAlso member.declaringClass Is GetType(MethodHandle) AndAlso (Not member.public) AndAlso member.static AndAlso member.name.StartsWith("linkTo")
        End Function

        ''' <summary>
        ''' Check if i-th name is a call to MethodHandleImpl.selectAlternative.
        ''' </summary>
        Private Function isSelectAlternative(ByVal pos As Integer) As Boolean
            ' selectAlternative idiom:
            '   t_{n}:L=MethodHandleImpl.selectAlternative(...)
            '   t_{n+1}:?=MethodHandle.invokeBasic(t_{n}, ...)
            If pos + 1 >= lambdaForm.names.Length Then Return False
            Dim name0 As Name = lambdaForm.names(pos)
            Dim name1 As Name = lambdaForm.names(pos + 1)
            Return nameRefersTo(name0, GetType(MethodHandleImpl), "selectAlternative") AndAlso isInvokeBasic(name1) AndAlso name1.lastUseIndex(name0) = 0 AndAlso lambdaForm.lastUseIndex(name0) = pos + 1 ' t_{n} is local: used only in t_{n+1} -  t_{n+1}:?=MethodHandle.invokeBasic(t_{n}, ...)
        End Function

        ''' <summary>
        ''' Check if i-th name is a start of GuardWithCatch idiom.
        ''' </summary>
        Private Function isGuardWithCatch(ByVal pos As Integer) As Boolean
            ' GuardWithCatch idiom:
            '   t_{n}:L=MethodHandle.invokeBasic(...)
            '   t_{n+1}:L=MethodHandleImpl.guardWithCatch(*, *, *, t_{n});
            '   t_{n+2}:?=MethodHandle.invokeBasic(t_{n+1})
            If pos + 2 >= lambdaForm.names.Length Then Return False
            Dim name0 As Name = lambdaForm.names(pos)
            Dim name1 As Name = lambdaForm.names(pos + 1)
            Dim name2 As Name = lambdaForm.names(pos + 2)
            Return nameRefersTo(name1, GetType(MethodHandleImpl), "guardWithCatch") AndAlso isInvokeBasic(name0) AndAlso isInvokeBasic(name2) AndAlso name1.lastUseIndex(name0) = 3 AndAlso lambdaForm.lastUseIndex(name0) = pos + 1 AndAlso name2.lastUseIndex(name1) = 1 AndAlso lambdaForm.lastUseIndex(name1) = pos + 2 ' t_{n+1} is local: used only in t_{n+2} -  t_{n+2}:?=MethodHandle.invokeBasic(t_{n+1}) -  t_{n} is local: used only in t_{n+1} -  t_{n+1}:L=MethodHandleImpl.guardWithCatch(*, *, *, t_{n});
        End Function

        ''' <summary>
        ''' Emit bytecode for the selectAlternative idiom.
        '''
        ''' The pattern looks like (Cf. MethodHandleImpl.makeGuardWithTest):
        ''' <blockquote><pre>{@code
        '''   Lambda(a0:L,a1:I)=>{
        '''     t2:I=foo.test(a1:I);
        '''     t3:L=MethodHandleImpl.selectAlternative(t2:I,(MethodHandle(int)int),(MethodHandle(int)int));
        '''     t4:I=MethodHandle.invokeBasic(t3:L,a1:I);t4:I}
        ''' }</pre></blockquote>
        ''' </summary>
        Private Function emitSelectAlternative(ByVal selectAlternativeName As Name, ByVal invokeBasicName As Name) As Name
            Debug.Assert(isStaticallyInvocable(invokeBasicName))

            Dim receiver As Name = CType(invokeBasicName.arguments(0), Name)

            Dim L_fallback As New Label
            Dim L_done As New Label

            ' load test result
            emitPushArgument(selectAlternativeName, 0)

            ' if_icmpne L_fallback
            mv.visitJumpInsn(Opcodes.IFEQ, L_fallback)

            ' invoke selectAlternativeName.arguments[1]
            Dim preForkClasses As [Class]() = localClasses.Clone()
            emitPushArgument(selectAlternativeName, 1) ' get 2nd argument of selectAlternative
            emitAstoreInsn(receiver.index()) ' store the MH in the receiver slot
            emitStaticInvoke(invokeBasicName)

            ' goto L_done
            mv.visitJumpInsn(Opcodes.GOTO, L_done)

            ' L_fallback:
            mv.visitLabel(L_fallback)

            ' invoke selectAlternativeName.arguments[2]
            Array.Copy(preForkClasses, 0, localClasses, 0, preForkClasses.Length)
            emitPushArgument(selectAlternativeName, 2) ' get 3rd argument of selectAlternative
            emitAstoreInsn(receiver.index()) ' store the MH in the receiver slot
            emitStaticInvoke(invokeBasicName)

            ' L_done:
            mv.visitLabel(L_done)
            ' for now do not bother to merge typestate; just reset to the dominator state
            Array.Copy(preForkClasses, 0, localClasses, 0, preForkClasses.Length)

            Return invokeBasicName ' return what's on stack
        End Function

        ''' <summary>
        ''' Emit bytecode for the guardWithCatch idiom.
        '''
        ''' The pattern looks like (Cf. MethodHandleImpl.makeGuardWithCatch):
        ''' <blockquote><pre>{@code
        '''  guardWithCatch=Lambda(a0:L,a1:L,a2:L,a3:L,a4:L,a5:L,a6:L,a7:L)=>{
        '''    t8:L=MethodHandle.invokeBasic(a4:L,a6:L,a7:L);
        '''    t9:L=MethodHandleImpl.guardWithCatch(a1:L,a2:L,a3:L,t8:L);
        '''   t10:I=MethodHandle.invokeBasic(a5:L,t9:L);t10:I}
        ''' }</pre></blockquote>
        '''
        ''' It is compiled into bytecode equivalent of the following code:
        ''' <blockquote><pre>{@code
        '''  try {
        '''      return a1.invokeBasic(a6, a7);
        '''  } catch (Throwable e) {
        '''      if (!a2.isInstance(e)) throw e;
        '''      return a3.invokeBasic(ex, a6, a7);
        '''  }}
        ''' </summary>
        Private Function emitGuardWithCatch(ByVal pos As Integer) As Name
            Dim args As Name = lambdaForm.names(pos)
            Dim invoker As Name = lambdaForm.names(pos + 1)
            Dim result As Name = lambdaForm.names(pos + 2)

            Dim L_startBlock As New Label
            Dim L_endBlock As New Label
            Dim L_handler As New Label
            Dim L_done As New Label

            Dim returnType As [Class] = result.function.resolvedHandle.type().returnType()
            Dim type As MethodType = args.function.resolvedHandle.type().dropParameterTypes(0, 1).changeReturnType(returnType)

            mv.visitTryCatchBlock(L_startBlock, L_endBlock, L_handler, "java/lang/Throwable")

            ' Normal case
            mv.visitLabel(L_startBlock)
            ' load target
            emitPushArgument(invoker, 0)
            emitPushArguments(args, 1) ' skip 1st argument: method handle
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", type.basicType().toMethodDescriptorString(), False)
            mv.visitLabel(L_endBlock)
            mv.visitJumpInsn(Opcodes.GOTO, L_done)

            ' Exceptional case
            mv.visitLabel(L_handler)

            ' Check exception's type
            mv.visitInsn(Opcodes.DUP)
            ' load exception class
            emitPushArgument(invoker, 1)
            mv.visitInsn(Opcodes.SWAP)
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, "java/lang/Class", "isInstance", "(Ljava/lang/Object;)Z", False)
            Dim L_rethrow As New Label
            mv.visitJumpInsn(Opcodes.IFEQ, L_rethrow)

            ' Invoke catcher
            ' load catcher
            emitPushArgument(invoker, 2)
            mv.visitInsn(Opcodes.SWAP)
            emitPushArguments(args, 1) ' skip 1st argument: method handle
            Dim catcherType As MethodType = type.insertParameterTypes(0, GetType(Throwable))
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", catcherType.basicType().toMethodDescriptorString(), False)
            mv.visitJumpInsn(Opcodes.GOTO, L_done)

            mv.visitLabel(L_rethrow)
            mv.visitInsn(Opcodes.ATHROW)

            mv.visitLabel(L_done)

            Return result
        End Function

        Private Sub emitPushArguments(ByVal args As Name)
            emitPushArguments(args, 0)
        End Sub

        Private Sub emitPushArguments(ByVal args As Name, ByVal start As Integer)
            For i As Integer = start To args.arguments.Length - 1
                emitPushArgument(args, i)
            Next i
        End Sub

        Private Sub emitPushArgument(ByVal name As Name, ByVal paramIndex As Integer)
            Dim arg As Object = name.arguments(paramIndex)
            Dim ptype As [Class] = name.function.methodType().parameterType(paramIndex)
            emitPushArgument(ptype, arg)
        End Sub

        Private Sub emitPushArgument(ByVal ptype As [Class], ByVal arg As Object)
            Dim bptype As BasicType = BasicType(ptype)
            If TypeOf arg Is Name Then
                Dim n As Name = CType(arg, Name)
                emitLoadInsn(n.type, n.index())
                emitImplicitConversion(n.type, ptype, n)
            ElseIf (arg Is Nothing OrElse TypeOf arg Is String) AndAlso bptype Is L_TYPE Then
                emitConst(arg)
            Else
                If sun.invoke.util.Wrapper.isWrapperType(arg.GetType()) AndAlso bptype IsNot L_TYPE Then
                    emitConst(arg)
                Else
                    mv.visitLdcInsn(constantPlaceholder(arg))
                    emitImplicitConversion(L_TYPE, ptype, arg)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Store the name to its local, if necessary.
        ''' </summary>
        Private Sub emitStoreResult(ByVal name As Name)
            If name IsNot Nothing AndAlso name.type <> V_TYPE Then emitStoreInsn(name.type, name.index())
        End Sub

        ''' <summary>
        ''' Emits a return statement from a LF invoker. If required, the result type is cast to the correct return type.
        ''' </summary>
        Private Sub emitReturn(ByVal onStack As Name)
            ' return statement
            Dim rclass As [Class] = invokerType.returnType()
            Dim rtype As BasicType = lambdaForm.returnType()
            Assert(rtype Is BasicType(rclass)) ' must agree
            If rtype Is V_TYPE Then
                ' void
                mv.visitInsn(Opcodes.RETURN)
                ' it doesn't matter what rclass is; the JVM will discard any value
            Else
                Dim rn As LambdaForm.Name = lambdaForm.names(lambdaForm.result)

                ' put return value on the stack if it is not already there
                If rn IsNot onStack Then emitLoadInsn(rtype, lambdaForm.result)

                emitImplicitConversion(rtype, rclass, rn)

                ' generate actual return statement
                emitReturnInsn(rtype)
            End If
        End Sub

        ''' <summary>
        ''' Emit a type conversion bytecode casting from "from" to "to".
        ''' </summary>
        Private Sub emitPrimCast(ByVal [from] As sun.invoke.util.Wrapper, ByVal [to] As sun.invoke.util.Wrapper)
            ' Here's how.
            ' -   indicates forbidden
            ' <-> indicates implicit
            '      to ----> boolean  byte     short    char     int      long     float    double
            ' from boolean    <->        -        -        -        -        -        -        -
            '      byte        -       <->       i2s      i2c      <->      i2l      i2f      i2d
            '      short       -       i2b       <->      i2c      <->      i2l      i2f      i2d
            '      char        -       i2b       i2s      <->      <->      i2l      i2f      i2d
            '      int         -       i2b       i2s      i2c      <->      i2l      i2f      i2d
            '      long        -     l2i,i2b   l2i,i2s  l2i,i2c    l2i      <->      l2f      l2d
            '      float       -     f2i,i2b   f2i,i2s  f2i,i2c    f2i      f2l      <->      f2d
            '      double      -     d2i,i2b   d2i,i2s  d2i,i2c    d2i      d2l      d2f      <->
            If [from] Is [to] Then Return
            If [from].subwordOrInt Then
                ' cast from {byte,short,char,int} to anything
                emitI2X([to])
            Else
                ' cast from {long,float,double} to anything
                If [to].subwordOrInt Then
                    ' cast to {byte,short,char,int}
                    emitX2I([from])
                    If [to].bitWidth() < 32 Then emitI2X([to])
                Else
                    ' cast to {long,float,double} - this is verbose
                    Dim error_Renamed As Boolean = False
                    Select Case [from]
                        Case Long
                            Select Case [to]
                                Case Float
                                    mv.visitInsn(Opcodes.L2F)
                                Case Double
                                    mv.visitInsn(Opcodes.L2D)
                                Case Else
                                    error_Renamed = True
                            End Select
                        Case Float
                            Select Case [to]
                                Case Long
                                    mv.visitInsn(Opcodes.F2L)
                                Case Double
                                    mv.visitInsn(Opcodes.F2D)
                                Case Else
                                    error_Renamed = True
                            End Select
                        Case Double
                            Select Case [to]
                                Case Long
                                    mv.visitInsn(Opcodes.D2L)
                                Case Float
                                    mv.visitInsn(Opcodes.D2F)
                                Case Else
                                    error_Renamed = True
                            End Select
                        Case Else
                            error_Renamed = True
                    End Select
                    If error_Renamed Then Throw New IllegalStateException("unhandled prim cast: " & [from] & "2" & [to])
                End If
            End If
        End Sub

        Private Sub emitI2X(ByVal type As sun.invoke.util.Wrapper)
            Select Case type
                Case Byte
                    mv.visitInsn(Opcodes.I2B)
                Case Short
                    mv.visitInsn(Opcodes.I2S)
                Case Char
                    mv.visitInsn(Opcodes.I2C)
                Case Int() ' naught
                Case Long
                    mv.visitInsn(Opcodes.I2L)
                Case Float
                    mv.visitInsn(Opcodes.I2F)
                Case Double
                    mv.visitInsn(Opcodes.I2D)
                Case Boolean
                    ' For compatibility with ValueConversions and explicitCastArguments:
                    mv.visitInsn(Opcodes.ICONST_1)
                    mv.visitInsn(Opcodes.IAND)
                Case Else
                    Throw New InternalError("unknown type: " & type)
            End Select
        End Sub

        Private Sub emitX2I(ByVal type As sun.invoke.util.Wrapper)
            Select Case type
                Case Long
                    mv.visitInsn(Opcodes.L2I)
                Case Float
                    mv.visitInsn(Opcodes.F2I)
                Case Double
                    mv.visitInsn(Opcodes.D2I)
                Case Else
                    Throw New InternalError("unknown type: " & type)
            End Select
        End Sub

        ''' <summary>
        ''' Generate bytecode for a LambdaForm.vmentry which calls interpretWithArguments.
        ''' </summary>
        Friend Shared Function generateLambdaFormInterpreterEntryPoint(ByVal sig As String) As MemberName
            Assert(isValidSignature(sig))
            Dim name As String = "interpret_" & signatureReturn(sig).basicTypeChar()
            Dim type As MethodType = signatureType(sig) ' sig includes leading argument
            type = type.changeParameterType(0, GetType(MethodHandle))
            Dim g As New InvokerBytecodeGenerator("LFI", name, type)
            Return g.loadMethod(g.generateLambdaFormInterpreterEntryPointBytes())
        End Function

        Private Function generateLambdaFormInterpreterEntryPointBytes() As SByte()
            classFilePrologue()

            ' Suppress this method in backtraces displayed to the user.
            mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", True)

            ' Don't inline the interpreter entry.
            mv.visitAnnotation("Ljava/lang/invoke/DontInline;", True)

            ' create parameter array
            emitIconstInsn(invokerType.parameterCount())
            mv.visitTypeInsn(Opcodes.ANEWARRAY, "java/lang/Object")

            ' fill parameter array
            For i As Integer = 0 To invokerType.parameterCount() - 1
                Dim ptype As [Class] = invokerType.parameterType(i)
                mv.visitInsn(Opcodes.DUP)
                emitIconstInsn(i)
                emitLoadInsn(BasicType(ptype), i)
                ' box if primitive type
                If ptype.primitive Then emitBoxing(sun.invoke.util.Wrapper.forPrimitiveType(ptype))
                mv.visitInsn(Opcodes.AASTORE)
            Next i
            ' invoke
            emitAloadInsn(0)
            mv.visitFieldInsn(Opcodes.GETFIELD, MH, "form", "Ljava/lang/invoke/LambdaForm;")
            mv.visitInsn(Opcodes.SWAP) ' swap form and array; avoid local variable
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, LF, "interpretWithArguments", "([Ljava/lang/Object;)Ljava/lang/Object;", False)

            ' maybe unbox
            Dim rtype As [Class] = invokerType.returnType()
            If rtype.primitive AndAlso rtype IsNot GetType(Void) Then emitUnboxing(sun.invoke.util.Wrapper.forPrimitiveType(rtype))

            ' return statement
            emitReturnInsn(BasicType(rtype))

            classFileEpilogue()
            bogusMethod(invokerType)

            Dim classFile As SByte() = cw.toByteArray()
            maybeDump(className, classFile)
            Return classFile
        End Function

        ''' <summary>
        ''' Generate bytecode for a NamedFunction invoker.
        ''' </summary>
        Friend Shared Function generateNamedFunctionInvoker(ByVal typeForm As MethodTypeForm) As MemberName
            Dim invokerType As MethodType = NamedFunction.INVOKER_METHOD_TYPE
            Dim invokerName As String = "invoke_" & shortenSignature(basicTypeSignature(typeForm.erasedType()))
            Dim g As New InvokerBytecodeGenerator("NFI", invokerName, invokerType)
            Return g.loadMethod(g.generateNamedFunctionInvokerImpl(typeForm))
        End Function

        Private Function generateNamedFunctionInvokerImpl(ByVal typeForm As MethodTypeForm) As SByte()
            Dim dstType As MethodType = typeForm.erasedType()
            classFilePrologue()

            ' Suppress this method in backtraces displayed to the user.
            mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", True)

            ' Force inlining of this invoker method.
            mv.visitAnnotation("Ljava/lang/invoke/ForceInline;", True)

            ' Load receiver
            emitAloadInsn(0)

            ' Load arguments from array
            For i As Integer = 0 To dstType.parameterCount() - 1
                emitAloadInsn(1)
                emitIconstInsn(i)
                mv.visitInsn(Opcodes.AALOAD)

                ' Maybe unbox
                Dim dptype As [Class] = dstType.parameterType(i)
                If dptype.primitive Then
                    Dim sptype As [Class] = dstType.basicType().wrap().parameterType(i)
                    Dim dstWrapper As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forBasicType(dptype)
                    Dim srcWrapper As sun.invoke.util.Wrapper = If(dstWrapper.subwordOrInt, sun.invoke.util.Wrapper.INT, dstWrapper) ' narrow subword from int
                    emitUnboxing(srcWrapper)
                    emitPrimCast(srcWrapper, dstWrapper)
                End If
            Next i

            ' Invoke
            Dim targetDesc As String = dstType.basicType().toMethodDescriptorString()
            mv.visitMethodInsn(Opcodes.INVOKEVIRTUAL, MH, "invokeBasic", targetDesc, False)

            ' Box primitive types
            Dim rtype As [Class] = dstType.returnType()
            If rtype IsNot GetType(Void) AndAlso rtype.primitive Then
                Dim srcWrapper As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forBasicType(rtype)
                Dim dstWrapper As sun.invoke.util.Wrapper = If(srcWrapper.subwordOrInt, sun.invoke.util.Wrapper.INT, srcWrapper) ' widen subword to int
                ' boolean casts not allowed
                emitPrimCast(srcWrapper, dstWrapper)
                emitBoxing(dstWrapper)
            End If

            ' If the return type is  Sub  we return a null reference.
            If rtype Is GetType(Void) Then mv.visitInsn(Opcodes.ACONST_NULL)
            emitReturnInsn(L_TYPE) ' NOTE: NamedFunction invokers always return a reference value.

            classFileEpilogue()
            bogusMethod(dstType)

            Dim classFile As SByte() = cw.toByteArray()
            maybeDump(className, classFile)
            Return classFile
        End Function

        ''' <summary>
        ''' Emit a bogus method that just loads some string constants. This is to get the constants into the constant pool
        ''' for debugging purposes.
        ''' </summary>
        Private Sub bogusMethod(ParamArray ByVal os As Object())
            If DUMP_CLASS_FILES Then
                mv = cw.visitMethod(Opcodes.ACC_STATIC, "dummy", "()V", Nothing, Nothing)
                For Each o As Object In os
                    mv.visitLdcInsn(o.ToString())
                    mv.visitInsn(Opcodes.POP)
                Next o
                mv.visitInsn(Opcodes.RETURN)
                mv.visitMaxs(0, 0)
                mv.visitEnd()
            End If
        End Sub
    End Class

End Namespace