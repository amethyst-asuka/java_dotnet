Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports java.lang.reflect

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
    ''' A {@code MemberName} is a compact symbolic datum which fully characterizes
    ''' a method or field reference.
    ''' A member name refers to a field, method, constructor, or member type.
    ''' Every member name has a simple name (a string) and a type (either a Class or MethodType).
    ''' A member name may also have a non-null declaring [Class], or it may be simply
    ''' a naked name/type pair.
    ''' A member name may also have non-zero modifier flags.
    ''' Finally, a member name may be either resolved or unresolved.
    ''' If it is resolved, the existence of the named
    ''' <p>
    ''' Whether resolved or not, a member name provides no access rights or
    ''' invocation capability to its possessor.  It is merely a compact
    ''' representation of all symbolic information necessary to link to
    ''' and properly use the named member.
    ''' <p>
    ''' When resolved, a member name's internal implementation may include references to JVM metadata.
    ''' This representation is stateless and only decriptive.
    ''' It provides no private information and no capability to use the member.
    ''' <p>
    ''' By contrast, a <seealso cref="java.lang.reflect.Method"/> contains fuller information
    ''' about the internals of a method (except its bytecodes) and also
    ''' allows invocation.  A MemberName is much lighter than a Method,
    ''' since it contains about 7 fields to the 16 of Method (plus its sub-arrays),
    ''' and those seven fields omit much of the information in Method.
    ''' @author jrose
    ''' </summary>
    'non-public
    Friend NotInheritable Class MemberName
        Implements Member, Cloneable

        Private clazz As [Class] ' class in which the method is defined
        Private name As String ' may be null if not yet materialized
        Private type As Object ' may be null if not yet materialized
        Private flags As Integer ' modifier bits; see reflect.Modifier
        '@Injected JVM_Method* vmtarget;
        '@Injected int         vmindex;
        Private resolution As Object ' if null, this guy is resolved

        ''' <summary>
        ''' Return the declaring class of this member.
        '''  In the case of a bare name and type, the declaring class will be null.
        ''' </summary>
        Public Property declaringClass As [Class] Implements Member.getDeclaringClass
            Get
                Return clazz
            End Get
        End Property

        ''' <summary>
        ''' Utility method producing the class loader of the declaring class. </summary>
        Public Property classLoader As [Class]Loader
			Get
                Return clazz.classLoader
            End Get
        End Property

        ''' <summary>
        ''' Return the simple name of this member.
        '''  For a type, it is the same as <seealso cref="Class#getSimpleName"/>.
        '''  For a method or field, it is the simple name of the member.
        '''  For a constructor, it is always {@code "&lt;init&gt;"}.
        ''' </summary>
        Public Property name As String Implements Member.getName
            Get
                If name Is Nothing Then
                    expandFromVM()
                    If name Is Nothing Then Return Nothing
                End If
                Return name
            End Get
        End Property

        Public Property methodOrFieldType As MethodType
            Get
                If invocable Then Return methodType
                If getter Then Return methodType.methodType(fieldType)
                If setter Then Return methodType.methodType(GetType(Void), fieldType)
                Throw New InternalError("not a method or field: " & Me)
            End Get
        End Property

        ''' <summary>
        ''' Return the declared type of this member, which
        '''  must be a method or constructor.
        ''' </summary>
        Public Property methodType As MethodType
            Get
                If type Is Nothing Then
                    expandFromVM()
                    If type Is Nothing Then Return Nothing
                End If
                If Not invocable Then Throw newIllegalArgumentException("not invocable, no method type")

                ' Get a snapshot of type which doesn't get changed by racing threads.
                Dim type_Renamed As Object = Me.type
                If TypeOf type_Renamed Is MethodType Then Return CType(type_Renamed, MethodType)

                ' type is not a MethodType yet.  Convert it thread-safely.
                SyncLock Me
                    If TypeOf type Is String Then
                        Dim sig As String = CStr(type)
                        Dim res As MethodType = methodType.fromMethodDescriptorString(sig, classLoader)
                        type = res
                    ElseIf TypeOf type Is Object() Then
                        Dim typeInfo As Object() = CType(type, Object())
                        Dim ptypes As [Class]() = CType(typeInfo(1), Class())
						Dim rtype As [Class] = CType(typeInfo(0), [Class])
                        Dim res As MethodType = methodType.methodType(rtype, ptypes)
                        type = res
                    End If
                    ' Make sure type is a MethodType for racing threads.
                    Debug.Assert(TypeOf type Is MethodType, "bad method type " & type)
                End SyncLock
                Return CType(type, MethodType)
            End Get
        End Property

        ''' <summary>
        ''' Return the actual type under which this method or constructor must be invoked.
        '''  For non-static methods or constructors, this is the type with a leading parameter,
        '''  a reference to declaring class.  For static methods, it is the same as the declared type.
        ''' </summary>
        Public Property invocationType As MethodType
            Get
                Dim itype As MethodType = methodOrFieldType
                If constructor AndAlso referenceKind = REF_newInvokeSpecial Then Return itype.changeReturnType(clazz)
                If Not [static] Then Return itype.insertParameterTypes(0, clazz)
                Return itype
            End Get
        End Property

        ''' <summary>
        ''' Utility method producing the parameter types of the method type. </summary>
        Public Property parameterTypes As [Class]()
            Get
                Return methodType.parameterArray()
            End Get
        End Property

        ''' <summary>
        ''' Utility method producing the return type of the method type. </summary>
        Public Property returnType As [Class]
            Get
                Return methodType.returnType()
            End Get
        End Property

        ''' <summary>
        ''' Return the declared type of this member, which
        '''  must be a field or type.
        '''  If it is a type member, that type itself is returned.
        ''' </summary>
        Public Property fieldType As [Class]
            Get
                If type Is Nothing Then
                    expandFromVM()
                    If type Is Nothing Then Return Nothing
                End If
                If invocable Then Throw newIllegalArgumentException("not a field or nested [Class], no simple type")

                ' Get a snapshot of type which doesn't get changed by racing threads.
                Dim type_Renamed As Object = Me.type
                If TypeOf type_Renamed Is Class Then Return CType(type_Renamed, [Class])

                ' type is not a Class yet.  Convert it thread-safely.
                SyncLock Me
                    If TypeOf type Is String Then
                        Dim sig As String = CStr(type)
                        Dim mtype As MethodType = methodType.fromMethodDescriptorString("()" & sig, classLoader)
                        Dim res As [Class] = mtype.returnType()
                        type = res
                    End If
                    ' Make sure type is a Class for racing threads.
                    Debug.Assert(TypeOf type Is [Class], "bad field type " & type)
                End SyncLock
                Return CType(type, [Class])
            End Get
        End Property

        ''' <summary>
        ''' Utility method to produce either the method type or field type of this member. </summary>
        Public Property type As Object
            Get
                Return (If(invocable, methodType, fieldType))
            End Get
        End Property

        ''' <summary>
        ''' Utility method to produce the signature of this member,
        '''  used within the class file format to describe its type.
        ''' </summary>
        Public Property signature As String
            Get
                If type Is Nothing Then
                    expandFromVM()
                    If type Is Nothing Then Return Nothing
                End If
                If invocable Then
                    Return sun.invoke.util.BytecodeDescriptor.unparse(methodType)
                Else
                    Return sun.invoke.util.BytecodeDescriptor.unparse(fieldType)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Return the modifier flags of this member. </summary>
        '''  <seealso cref= java.lang.reflect.Modifier </seealso>
        Public Property modifiers As Integer Implements Member.getModifiers
            Get
                Return (flags And RECOGNIZED_MODIFIERS)
            End Get
        End Property

        ''' <summary>
        ''' Return the reference kind of this member, or zero if none.
        ''' </summary>
        Public Property referenceKind As SByte
            Get
                Return CByte((CInt(CUInt(flags) >> MN_REFERENCE_KIND_SHIFT)) And MN_REFERENCE_KIND_MASK)
            End Get
        End Property
        Private Function referenceKindIsConsistent() As Boolean
            Dim refKind As SByte = referenceKind
            If refKind = REF_NONE Then Return type
            If field Then
                Assert(staticIsConsistent())
                Assert(MethodHandleNatives.refKindIsField(refKind))
            ElseIf constructor Then
                Assert(refKind = REF_newInvokeSpecial OrElse refKind = REF_invokeSpecial)
            ElseIf method Then
                Assert(staticIsConsistent())
                Assert(MethodHandleNatives.refKindIsMethod(refKind))
                If clazz.interface Then Assert(refKind = REF_invokeInterface OrElse refKind = REF_invokeStatic OrElse refKind = REF_invokeSpecial OrElse refKind = REF_invokeVirtual AndAlso objectPublicMethod)
            Else
                Assert(False)
            End If
            Return True
        End Function
        Private Property objectPublicMethod As Boolean
            Get
                If clazz Is GetType(Object) Then Return True
                Dim mtype As MethodType = methodType
                If name.Equals("toString") AndAlso mtype.returnType() Is GetType(String) AndAlso mtype.parameterCount() = 0 Then Return True
                If name.Equals("hashCode") AndAlso mtype.returnType() Is GetType(Integer) AndAlso mtype.parameterCount() = 0 Then Return True
                If name.Equals("equals") AndAlso mtype.returnType() Is GetType(Boolean) AndAlso mtype.parameterCount() = 1 AndAlso mtype.parameterType(0) Is GetType(Object) Then Return True
                Return False
            End Get
        End Property
        'non-public
        Friend Function referenceKindIsConsistentWith(ByVal originalRefKind As Integer) As Boolean
            Dim refKind As Integer = referenceKind
            If refKind = originalRefKind Then Return True
            Select Case originalRefKind
                Case REF_invokeInterface
                    ' Looking up an interface method, can get (e.g.) Object.hashCode
                    Assert(refKind = REF_invokeVirtual OrElse refKind = REF_invokeSpecial) : Me
                    Return True
                Case REF_invokeVirtual, REF_newInvokeSpecial
                    ' Looked up a virtual, can get (e.g.) final String.hashCode.
                    Assert(refKind = REF_invokeSpecial) : Me
                    Return True
            End Select
            Assert(False) : Me & " != " & MethodHandleNatives.refKindName(CByte(originalRefKind))
            Return True
        End Function
        Private Function staticIsConsistent() As Boolean
            Dim refKind As SByte = referenceKind
            Return MethodHandleNatives.refKindIsStatic(refKind) = [static] OrElse modifiers = 0
        End Function
        Private Function vminfoIsConsistent() As Boolean
            Dim refKind As SByte = referenceKind
            Assert(resolved) ' else don't call
            Dim vminfo As Object = MethodHandleNatives.getMemberVMInfo(Me)
            Assert(TypeOf vminfo Is Object())
            Dim vmindex As Long = CLng(Fix(CType(vminfo, Object())(0)))
            Dim vmtarget As Object = CType(vminfo, Object())(1)
            If MethodHandleNatives.refKindIsField(refKind) Then
                Assert(vmindex >= 0) : vmindex & ":" & Me
                Assert(TypeOf vmtarget Is [Class])
            Else
                If MethodHandleNatives.refKindDoesDispatch(refKind) Then
                    Assert(vmindex >= 0) : vmindex & ":" & Me
                Else
                    Assert(vmindex < 0) : vmindex
                End If
                Assert(TypeOf vmtarget Is MemberName) : vmtarget & " in " & Me
            End If
            Return True
        End Function

        Private Function changeReferenceKind(ByVal refKind As SByte, ByVal oldKind As SByte) As MemberName
            Assert(referenceKind = oldKind)
            Assert(MethodHandleNatives.refKindIsValid(refKind))
            flags += ((CInt(refKind) - oldKind) << MN_REFERENCE_KIND_SHIFT)
            Return Me
        End Function

        Private Function testFlags(ByVal mask As Integer, ByVal value As Integer) As Boolean
            Return (flags And mask) = value
        End Function
        Private Function testAllFlags(ByVal mask As Integer) As Boolean
            Return testFlags(mask, mask)
        End Function
        Private Function testAnyFlags(ByVal mask As Integer) As Boolean
            Return Not testFlags(mask, 0)
        End Function

        ''' <summary>
        ''' Utility method to query if this member is a method handle invocation (invoke or invokeExact).
        '''  Also returns true for the non-public MH.invokeBasic.
        ''' </summary>
        Public Property methodHandleInvoke As Boolean
            Get
                Dim bits As Integer = MH_INVOKE_MODS And Not Modifier.PUBLIC
                Dim negs As Integer = Modifier.STATIC
                If testFlags(bits Or negs, bits) AndAlso clazz Is GetType(MethodHandle) Then Return isMethodHandleInvokeName(name)
                Return False
            End Get
        End Property
        Public Shared Function isMethodHandleInvokeName(ByVal name As String) As Boolean
            Select Case name
                Case "invoke", "invokeExact", "invokeBasic"
                    Return True
                Case Else
                    Return False
            End Select
        End Function
        Private Shared ReadOnly MH_INVOKE_MODS As Integer = Modifier.NATIVE Or Modifier.FINAL Or Modifier.PUBLIC

        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property [static] As Boolean
            Get
                Return Modifier.isStatic(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property [public] As Boolean
            Get
                Return Modifier.isPublic(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property [private] As Boolean
            Get
                Return Modifier.isPrivate(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property [protected] As Boolean
            Get
                Return Modifier.isProtected(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property final As Boolean
            Get
                Return Modifier.isFinal(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query whether this member or its defining class is final. </summary>
        Public Function canBeStaticallyBound() As Boolean
            Return Modifier.isFinal(flags Or clazz.modifiers)
        End Function
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property volatile As Boolean
            Get
                Return Modifier.isVolatile(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property abstract As Boolean
            Get
                Return Modifier.isAbstract(flags)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member. </summary>
        Public Property native As Boolean
            Get
                Return Modifier.isNative(flags)
            End Get
        End Property
        ' let the rest (native, volatile, transient, etc.) be tested via Modifier.isFoo

        ' unofficial modifier flags, used by HotSpot:
        Friend Const BRIDGE As Integer = &H40
        Friend Const VARARGS As Integer = &H80
        Friend Const SYNTHETIC As Integer = &H1000
        Friend Const ANNOTATION As Integer = &H2000
        Friend Const [ENUM] As Integer = &H4000
        ''' <summary>
        ''' Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
        Public Property bridge As Boolean
            Get
                Return testAllFlags(IS_METHOD Or bridge)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
        Public Property varargs As Boolean
            Get
                Return testAllFlags(varargs) AndAlso invocable
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query the modifier flags of this member; returns false if the member is not a method. </summary>
        Public Property synthetic As Boolean Implements Member.isSynthetic
            Get
                Return testAllFlags(synthetic)
            End Get
        End Property

        Friend Const CONSTRUCTOR_NAME As String = "<init>" ' the ever-popular

        ' modifiers exported by the JVM:
        Friend Const RECOGNIZED_MODIFIERS As Integer = &HFFFF

        ' private flags, not part of RECOGNIZED_MODIFIERS:
        Friend Shared ReadOnly IS_METHOD As Integer = MN_IS_METHOD, IS_CONSTRUCTOR As Integer = MN_IS_CONSTRUCTOR, IS_FIELD As Integer = MN_IS_FIELD, IS_TYPE As Integer = MN_IS_TYPE, CALLER_SENSITIVE As Integer = MN_CALLER_SENSITIVE ' @CallerSensitive annotation detected -  nested type -  field -  constructor -  method (not constructor)

        Friend Shared ReadOnly ALL_ACCESS As Integer = Modifier.PUBLIC Or Modifier.PRIVATE Or Modifier.PROTECTED
        Friend Shared ReadOnly ALL_KINDS As Integer = IS_METHOD Or IS_CONSTRUCTOR Or IS_FIELD Or IS_TYPE
        Friend Shared ReadOnly IS_INVOCABLE As Integer = IS_METHOD Or IS_CONSTRUCTOR
        Friend Shared ReadOnly IS_FIELD_OR_METHOD As Integer = IS_METHOD Or IS_FIELD
        Friend Shared ReadOnly SEARCH_ALL_SUPERS As Integer = MN_SEARCH_SUPERCLASSES Or MN_SEARCH_INTERFACES

        ''' <summary>
        ''' Utility method to query whether this member is a method or constructor. </summary>
        Public Property invocable As Boolean
            Get
                Return testAnyFlags(IS_INVOCABLE)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query whether this member is a method, constructor, or field. </summary>
        Public Property fieldOrMethod As Boolean
            Get
                Return testAnyFlags(IS_FIELD_OR_METHOD)
            End Get
        End Property
        ''' <summary>
        ''' Query whether this member is a method. </summary>
        Public Property method As Boolean
            Get
                Return testAllFlags(IS_METHOD)
            End Get
        End Property
        ''' <summary>
        ''' Query whether this member is a constructor. </summary>
        Public Property constructor As Boolean
            Get
                Return testAllFlags(IS_CONSTRUCTOR)
            End Get
        End Property
        ''' <summary>
        ''' Query whether this member is a field. </summary>
        Public Property field As Boolean
            Get
                Return testAllFlags(IS_FIELD)
            End Get
        End Property
        ''' <summary>
        ''' Query whether this member is a type. </summary>
        Public Property type As Boolean
            Get
                Return testAllFlags(IS_TYPE)
            End Get
        End Property
        ''' <summary>
        ''' Utility method to query whether this member is neither public, private, nor protected. </summary>
        Public Property package As Boolean
            Get
                Return Not testAnyFlags(ALL_ACCESS)
            End Get
        End Property
        ''' <summary>
        ''' Query whether this member has a CallerSensitive annotation. </summary>
        Public Property callerSensitive As Boolean
            Get
                Return testAllFlags(CALLER_SENSITIVE)
            End Get
        End Property

        ''' <summary>
        ''' Utility method to query whether this member is accessible from a given lookup class. </summary>
        Public Function isAccessibleFrom(ByVal lookupClass As [Class]) As Boolean
            Return sun.invoke.util.VerifyAccess.isMemberAccessible(Me.declaringClass, Me.declaringClass, flags, lookupClass, ALL_ACCESS Or MethodHandles.lookup.PACKAGE)
        End Function

        ''' <summary>
        ''' Initialize a query.   It is not resolved. </summary>
        Private Sub init(ByVal defClass As [Class], ByVal name As String, ByVal type As Object, ByVal flags As Integer)
            ' defining class is allowed to be null (for a naked name/type pair)
            'name.toString();  // null check
            'type.equals(type);  // null check
            ' fill in fields:
            Me.clazz = defClass
            Me.name = name
            Me.type = type
            Me.flags = flags
            Assert(testAnyFlags(ALL_KINDS))
            Assert(Me.resolution Is Nothing) ' nobody should have touched this yet
            'assert(referenceKindIsConsistent());  // do this after resolution
        End Sub

        ''' <summary>
        ''' Calls down to the VM to fill in the fields.  This method is
        ''' synchronized to avoid racing calls.
        ''' </summary>
        Private Sub expandFromVM()
            If type IsNot Nothing Then Return
            If Not resolved Then Return
            MethodHandleNatives.expand(Me)
        End Sub

        ' Capturing information from the Core Reflection API:
        Private Shared Function flagsMods(ByVal flags As Integer, ByVal mods As Integer, ByVal refKind As SByte) As Integer
            Assert((flags And RECOGNIZED_MODIFIERS) = 0)
            Assert((mods And (Not RECOGNIZED_MODIFIERS)) = 0)
            Assert((refKind And (Not MN_REFERENCE_KIND_MASK)) = 0)
            Return flags Or mods Or (refKind << MN_REFERENCE_KIND_SHIFT)
        End Function
        ''' <summary>
        ''' Create a name for the given reflected method.  The resulting name will be in a resolved state. </summary>
        Public Sub New(ByVal m As Method)
            Me.New(m, False)
        End Sub
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Sub New(ByVal m As Method, ByVal wantSpecial As Boolean)
            m.GetType() ' NPE check
            ' fill in vmtarget, vmindex while we have m in hand:
            MethodHandleNatives.init(Me, m)
            If clazz Is Nothing Then ' MHN.init failed
                If m.declaringClass = GetType(MethodHandle) AndAlso isMethodHandleInvokeName(m.name) Then
                    ' The JVM did not reify this signature-polymorphic instance.
                    ' Need a special case here.
                    ' See comments on MethodHandleNatives.linkMethod.
                    Dim type_Renamed As MethodType = methodType.methodType(m.returnType, m.parameterTypes)
                    Dim flags As Integer = flagsMods(IS_METHOD, m.modifiers, REF_invokeVirtual)
                    init(GetType(MethodHandle), m.name, type_Renamed, flags)
                    If methodHandleInvoke Then Return
                End If
                Throw New LinkageError(m.ToString())
            End If
            Assert(resolved AndAlso Me.clazz IsNot Nothing)
            Me.name = m.name
            If Me.type Is Nothing Then Me.type = New Object() {m.returnType, m.parameterTypes}
            If wantSpecial Then
                If abstract Then Throw New AbstractMethodError(Me.ToString())
                If referenceKind = REF_invokeVirtual Then
                    changeReferenceKind(REF_invokeSpecial, REF_invokeVirtual)
                ElseIf referenceKind = REF_invokeInterface Then
                    ' invokeSpecial on a default method
                    changeReferenceKind(REF_invokeSpecial, REF_invokeInterface)
                End If
            End If
        End Sub
        Public Function asSpecial() As MemberName
            Select Case referenceKind
                Case REF_invokeSpecial
                    Return Me
                Case REF_invokeVirtual
                    Return clone().changeReferenceKind(REF_invokeSpecial, REF_invokeVirtual)
                Case REF_invokeInterface
                    Return clone().changeReferenceKind(REF_invokeSpecial, REF_invokeInterface)
                Case REF_newInvokeSpecial
                    Return clone().changeReferenceKind(REF_invokeSpecial, REF_newInvokeSpecial)
            End Select
            Throw New IllegalArgumentException(Me.ToString())
        End Function
        ''' <summary>
        ''' If this MN is not REF_newInvokeSpecial, return a clone with that ref. kind.
        '''  In that case it must already be REF_invokeSpecial.
        ''' </summary>
        Public Function asConstructor() As MemberName
            Select Case referenceKind
                Case REF_invokeSpecial
                    Return clone().changeReferenceKind(REF_newInvokeSpecial, REF_invokeSpecial)
                Case REF_newInvokeSpecial
                    Return Me
            End Select
            Throw New IllegalArgumentException(Me.ToString())
        End Function
        ''' <summary>
        ''' If this MN is a REF_invokeSpecial, return a clone with the "normal" kind
        '''  REF_invokeVirtual; also switch either to REF_invokeInterface if clazz.isInterface.
        '''  The end result is to get a fully virtualized version of the MN.
        '''  (Note that resolving in the JVM will sometimes devirtualize, changing
        '''  REF_invokeVirtual of a final to REF_invokeSpecial, and REF_invokeInterface
        '''  in some corner cases to either of the previous two; this transform
        '''  undoes that change under the assumption that it occurred.)
        ''' </summary>
        Public Function asNormalOriginal() As MemberName
            Dim normalVirtual As SByte = If(clazz.interface, REF_invokeInterface, REF_invokeVirtual)
            Dim refKind As SByte = referenceKind
            Dim newRefKind As SByte = refKind
            Dim result As MemberName = Me
            Select Case refKind
                Case REF_invokeInterface, REF_invokeVirtual, REF_invokeSpecial
                    newRefKind = normalVirtual
            End Select
            If newRefKind = refKind Then Return Me
            result = clone().changeReferenceKind(newRefKind, refKind)
            Assert(Me.referenceKindIsConsistentWith(result.referenceKind))
            Return result
        End Function
        ''' <summary>
        ''' Create a name for the given reflected constructor.  The resulting name will be in a resolved state. </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Sub New(Of T1)(ByVal ctor As Constructor(Of T1))
            ctor.GetType() ' NPE check
            ' fill in vmtarget, vmindex while we have ctor in hand:
            MethodHandleNatives.init(Me, ctor)
            Assert(resolved AndAlso Me.clazz IsNot Nothing)
            Me.name = CONSTRUCTOR_NAME
            If Me.type Is Nothing Then Me.type = New Object() {GetType(Void), ctor.parameterTypes}
        End Sub
        ''' <summary>
        ''' Create a name for the given reflected field.  The resulting name will be in a resolved state.
        ''' </summary>
        Public Sub New(ByVal fld As Field)
            Me.New(fld, False)
        End Sub
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Sub New(ByVal fld As Field, ByVal makeSetter As Boolean)
            fld.GetType() ' NPE check
            ' fill in vmtarget, vmindex while we have fld in hand:
            MethodHandleNatives.init(Me, fld)
            Assert(resolved AndAlso Me.clazz IsNot Nothing)
            Me.name = fld.name
            Me.type = fld.type
            Assert((REF_putStatic - REF_getStatic) = (REF_putField - REF_getField))
            Dim refKind As SByte = Me.referenceKind
            Assert(refKind = (If([static], REF_getStatic, REF_getField)))
            If makeSetter Then changeReferenceKind(CByte(refKind + (REF_putStatic - REF_getStatic)), refKind)
        End Sub
        Public Property getter As Boolean
            Get
                Return MethodHandleNatives.refKindIsGetter(referenceKind)
            End Get
        End Property
        Public Property setter As Boolean
            Get
                Return MethodHandleNatives.refKindIsSetter(referenceKind)
            End Get
        End Property
        Public Function asSetter() As MemberName
            Dim refKind As SByte = referenceKind
            Assert(MethodHandleNatives.refKindIsGetter(refKind))
            Assert((REF_putStatic - REF_getStatic) = (REF_putField - REF_getField))
            Dim setterRefKind As SByte = CByte(refKind + (REF_putField - REF_getField))
            Return clone().changeReferenceKind(setterRefKind, refKind)
        End Function
        ''' <summary>
        ''' Create a name for the given class.  The resulting name will be in a resolved state. </summary>
        Public Sub New(ByVal type As [Class])
            init(type.declaringClass, type.simpleName, type, flagsMods(IS_TYPE, type.modifiers, REF_NONE))
            initResolved(True)
        End Sub

        ''' <summary>
        ''' Create a name for a signature-polymorphic invoker.
        ''' This is a placeholder for a signature-polymorphic instance
        ''' (of MH.invokeExact, etc.) that the JVM does not reify.
        ''' See comments on <seealso cref="MethodHandleNatives#linkMethod"/>.
        ''' </summary>
        Shared Function makeMethodHandleInvoke(ByVal name As String, ByVal type As MethodType) As MemberName
            Return makeMethodHandleInvoke(name, type, MH_INVOKE_MODS Or SYNTHETIC)
        End Function
        Shared Function makeMethodHandleInvoke(ByVal name As String, ByVal type As MethodType, ByVal mods As Integer) As MemberName
            Dim mem As New MemberName(GetType(MethodHandle), name, type, REF_invokeVirtual)
            mem.flags = mem.flags Or mods ' it's not resolved, but add these modifiers anyway
            Assert(mem.methodHandleInvoke) : mem
            Return mem
        End Function

        ' bare-bones constructor; the JVM will fill it in
        Friend Sub New()
        End Sub

        ' locally useful cloner
        Protected Friend Overrides Function clone() As MemberName
            Try
                Return CType(MyBase.clone(), MemberName)
            Catch ex As CloneNotSupportedException
                Throw newInternalError(ex)
            End Try
        End Function

        ''' <summary>
        ''' Get the definition of this member name.
        '''  This may be in a super-class of the declaring class of this member.
        ''' </summary>
        Public ReadOnly Property definition As MemberName
            Get
                If Not resolved Then Throw New IllegalStateException("must be resolved: " & Me)
                If type Then Return Me
                Dim res As MemberName = Me.clone()
                res.clazz = Nothing
                res.type = Nothing
                res.name = Nothing
                res.resolution = res
                res.expandFromVM()
                Assert(res.name.Equals(Me.name))
                Return res
            End Get
        End Property

        Public Overrides Function GetHashCode() As Integer
            Return java.util.Objects.hash(clazz, referenceKind, name, type)
        End Function
        Public Overrides Function Equals(ByVal that As Object) As Boolean
            Return (TypeOf that Is MemberName AndAlso Me.Equals(CType(that, MemberName)))
        End Function

        ''' <summary>
        ''' Decide if two member names have exactly the same symbolic content.
        '''  Does not take into account any actual class members, so even if
        '''  two member names resolve to the same actual member, they may
        '''  be distinct references.
        ''' </summary>
        Public Overloads Function Equals(ByVal that As MemberName) As Boolean
            If Me Is that Then Return True
            If that Is Nothing Then Return False
            Return Me.clazz Is that.clazz AndAlso Me.referenceKind = that.referenceKind AndAlso java.util.Objects.Equals(Me.name, that.name) AndAlso java.util.Objects.Equals(Me.type, that.type)
        End Function

        ' Construction from symbolic parts, for queries:
        ''' <summary>
        ''' Create a field or type name from the given components:
        '''  Declaring [Class], name, type, reference kind.
        '''  The declaring class may be supplied as null if this is to be a bare name and type.
        '''  The resulting name will in an unresolved state.
        ''' </summary>
        Public Sub New(ByVal defClass As [Class], ByVal name As String, ByVal type As [Class], ByVal refKind As SByte)
            init(defClass, name, type, flagsMods(IS_FIELD, 0, refKind))
            initResolved(False)
        End Sub
        ''' <summary>
        ''' Create a method or constructor name from the given components:
        '''  Declaring [Class], name, type, reference kind.
        '''  It will be a constructor if and only if the name is {@code "&lt;init&gt;"}.
        '''  The declaring class may be supplied as null if this is to be a bare name and type.
        '''  The last argument is optional, a boolean which requests REF_invokeSpecial.
        '''  The resulting name will in an unresolved state.
        ''' </summary>
        Public Sub New(ByVal defClass As [Class], ByVal name As String, ByVal type As MethodType, ByVal refKind As SByte)
            Dim initFlags As Integer = (If(name IsNot Nothing AndAlso name.Equals(CONSTRUCTOR_NAME), IS_CONSTRUCTOR, IS_METHOD))
            init(defClass, name, type, flagsMods(initFlags, 0, refKind))
            initResolved(False)
        End Sub
        ''' <summary>
        ''' Create a method, constructor, or field name from the given components:
        '''  Reference kind, declaring [Class], name, type.
        ''' </summary>
        Public Sub New(ByVal refKind As SByte, ByVal defClass As [Class], ByVal name As String, ByVal type As Object)
            Dim kindFlags As Integer
            If MethodHandleNatives.refKindIsField(refKind) Then
                kindFlags = IS_FIELD
                If Not (TypeOf type Is [Class]) Then Throw New IllegalArgumentException("not a field type")
            ElseIf MethodHandleNatives.refKindIsMethod(refKind) Then
                kindFlags = IS_METHOD
                If Not (TypeOf type Is MethodType) Then Throw New IllegalArgumentException("not a method type")
            ElseIf refKind = REF_newInvokeSpecial Then
                kindFlags = IS_CONSTRUCTOR
                If Not (TypeOf type Is MethodType) OrElse (Not CONSTRUCTOR_NAME.Equals(name)) Then Throw New IllegalArgumentException("not a constructor type or name")
            Else
                Throw New IllegalArgumentException("bad reference kind " & refKind)
            End If
            init(defClass, name, type, flagsMods(kindFlags, 0, refKind))
            initResolved(False)
        End Sub
        ''' <summary>
        ''' Query whether this member name is resolved to a non-static, non-final method.
        ''' </summary>
        Public Function hasReceiverTypeDispatch() As Boolean
            Return MethodHandleNatives.refKindDoesDispatch(referenceKind)
        End Function

        ''' <summary>
        ''' Query whether this member name is resolved.
        '''  A resolved member name is one for which the JVM has found
        '''  a method, constructor, field, or type binding corresponding exactly to the name.
        '''  (Document?)
        ''' </summary>
        Public ReadOnly Property resolved As Boolean
            Get
                Return resolution Is Nothing
            End Get
        End Property

        Private Sub initResolved(ByVal isResolved As Boolean)
            Assert(Me.resolution Is Nothing) ' not initialized yet!
            If Not isResolved Then Me.resolution = Me
            Assert(resolved = isResolved)
        End Sub

        Friend Sub checkForTypeAlias()
            If invocable Then
                Dim type_Renamed As MethodType
                If TypeOf Me.type Is MethodType Then
                    type_Renamed = CType(Me.type, MethodType)
                Else
                    type_Renamed = methodType
                    Me.type = type_Renamed
                End If
                If type_Renamed.erase() Is type_Renamed Then Return
                If sun.invoke.util.VerifyAccess.isTypeVisible(type_Renamed, clazz) Then Return
                Throw New LinkageError("bad method type alias: " & type_Renamed & " not visible from " & clazz)
            Else
                Dim type_Renamed As [Class]
                If TypeOf Me.type Is [Class] Then
                    type_Renamed = CType(Me.type, [Class])
                Else
                    type_Renamed = fieldType
                    Me.type = type_Renamed
                End If
                If sun.invoke.util.VerifyAccess.isTypeVisible(type_Renamed, clazz) Then Return
                Throw New LinkageError("bad field type alias: " & type_Renamed & " not visible from " & clazz)
            End If
        End Sub


        ''' <summary>
        ''' Produce a string form of this member name.
        '''  For types, it is simply the type's own string (as reported by {@code toString}).
        '''  For fields, it is {@code "DeclaringClass.name/type"}.
        '''  For methods and constructors, it is {@code "DeclaringClass.name(ptype...)rtype"}.
        '''  If the declaring class is null, the prefix {@code "DeclaringClass."} is omitted.
        '''  If the member is unresolved, a prefix {@code "*."} is prepended.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Public Overrides Function ToString() As String
            If type Then Return type.ToString() ' class java.lang.String
            ' else it is a field, method, or constructor
            Dim buf As New StringBuilder
            If declaringClass IsNot Nothing Then
                buf.append(getName(clazz))
                buf.append("."c)
            End If
            Dim name_Renamed As String = name
            buf.append(If(name_Renamed Is Nothing, "*", name_Renamed))
            Dim type_Renamed As Object = type
            If Not invocable Then
                buf.append("/"c)
                buf.append(If(type_Renamed Is Nothing, "*", getName(type_Renamed)))
            Else
                buf.append(If(type_Renamed Is Nothing, "(*)*", getName(type_Renamed)))
            End If
            Dim refKind As SByte = referenceKind
            If refKind <> REF_NONE Then
                buf.append("/"c)
                buf.append(MethodHandleNatives.refKindName(refKind))
            End If
            'buf.append("#").append(System.identityHashCode(this));
            Return buf.ToString()
        End Function
        Private Shared Function getName(ByVal obj As Object) As String
            If TypeOf obj Is [Class] Then Return CType(obj, [Class]).name
            Return Convert.ToString(obj)
        End Function

        Public Function makeAccessException(ByVal message As String, ByVal [from] As Object) As IllegalAccessException
            message = message & ": " & ToString()
            If [from] IsNot Nothing Then message &= ", from " & [from]
            Return New IllegalAccessException(message)
        End Function
        Private Function message() As String
            If resolved Then
                Return "no access"
            ElseIf constructor Then
                Return "no such constructor"
            ElseIf method Then
                Return "no such method"
            Else
                Return "no such field"
            End If
        End Function
        Public Function makeAccessException() As ReflectiveOperationException
            Dim message As String = message() & ": " & ToString()
            Dim ex As ReflectiveOperationException
            If resolved OrElse Not (TypeOf resolution Is NoSuchMethodError OrElse TypeOf resolution Is NoSuchFieldError) Then
                ex = New IllegalAccessException(message)
            ElseIf constructor Then
                ex = New NoSuchMethodException(message)
            ElseIf method Then
                ex = New NoSuchMethodException(message)
            Else
                ex = New NoSuchFieldException(message)
            End If
            If TypeOf resolution Is Throwable Then ex.initCause(CType(resolution, Throwable))
            Return ex
        End Function

        ''' <summary>
        ''' Actually making a query requires an access check. </summary>
        'non-public
        Friend Shared ReadOnly Property factory As Factory
            Get
                Return Factory.INSTANCE
            End Get
        End Property
        ''' <summary>
        ''' A factory type for resolving member names with the help of the VM.
        '''  TBD: Define access-safe public constructors for this factory.
        ''' </summary>
        'non-public
        Friend Class Factory
            Private Sub New() ' singleton pattern
            End Sub
            Friend Shared INSTANCE As New Factory

            Private Shared ALLOWED_FLAGS As Integer = ALL_KINDS

            '/ Queries
            Friend Overridable Function getMembers(ByVal defc As [Class], ByVal matchName As String, ByVal matchType As Object, ByVal matchFlags As Integer, ByVal lookupClass As [Class]) As IList(Of MemberName)
                matchFlags = matchFlags And ALLOWED_FLAGS
                Dim matchSig As String = Nothing
                If matchType IsNot Nothing Then
                    matchSig = sun.invoke.util.BytecodeDescriptor.unparse(matchType)
                    If matchSig.StartsWith("(") Then
                        matchFlags = matchFlags And Not (ALL_KINDS And (Not IS_INVOCABLE))
                    Else
                        matchFlags = matchFlags And Not (ALL_KINDS And (Not IS_FIELD))
                    End If
                End If
                Const BUF_MAX As Integer = &H2000
                Dim len1 As Integer = If(matchName Is Nothing, 10, If(matchType Is Nothing, 4, 1))
                Dim buf As MemberName() = newMemberBuffer(len1)
                Dim totalCount As Integer = 0
                Dim bufs As List(Of MemberName()) = Nothing
                Dim bufCount As Integer = 0
                Do
                    bufCount = MethodHandleNatives.getMembers(defc, matchName, matchSig, matchFlags, lookupClass, totalCount, buf)
                    If bufCount <= buf.Length Then
                        If bufCount < 0 Then bufCount = 0
                        totalCount += bufCount
                        Exit Do
                    End If
                    ' JVM returned to us with an intentional overflow!
                    totalCount += buf.Length
                    Dim excess As Integer = bufCount - buf.Length
                    If bufs Is Nothing Then bufs = New List(Of )(1)
                    bufs.Add(buf)
                    Dim len2 As Integer = buf.Length
                    len2 = System.Math.max(len2, excess)
                    len2 = System.Math.max(len2, totalCount \ 4)
                    buf = newMemberBuffer (System.Math.min(BUF_MAX, len2))
                Loop
                Dim result As New List(Of MemberName)(totalCount)
                If bufs IsNot Nothing Then
                    For Each buf0 As MemberName() In bufs
                        java.util.Collections.addAll(result, buf0)
                    Next buf0
                End If
                result.AddRange(java.util.Arrays.asList(buf).subList(0, bufCount))
                ' Signature matching is not the same as type matching, since
                ' one signature might correspond to several types.
                ' So if matchType is a Class or MethodType, refilter the results.
                If matchType IsNot Nothing AndAlso matchType IsNot matchSig Then
                    Dim it As IEnumerator(Of MemberName) = result.GetEnumerator()
                    Do While it.MoveNext()
                        Dim m As MemberName = it.Current
                        If Not matchType.Equals(m.type) Then it.remove()
                    Loop
                End If
                Return result
            End Function
            ''' <summary>
            ''' Produce a resolved version of the given member.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  If lookup fails or access is not permitted, null is returned.
            '''  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
            ''' </summary>
            Private Function resolve(ByVal refKind As SByte, ByVal ref As MemberName, ByVal lookupClass As [Class]) As MemberName
                Dim m As MemberName = ref.clone() ' JVM will side-effect the ref
                Assert(refKind = m.referenceKind)
                Try
                    m = MethodHandleNatives.resolve(m, lookupClass)
                    m.checkForTypeAlias()
                    m.resolution = Nothing
                Catch ex As LinkageError
                    ' JVM reports that the "bytecode behavior" would get an error
                    Assert((Not m.resolved))
                    m.resolution = ex
                    Return m
                End Try
                Assert(m.referenceKindIsConsistent())
                m.initResolved(True)
                Assert(m.vminfoIsConsistent())
                Return m
            End Function
            ''' <summary>
            ''' Produce a resolved version of the given member.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  If lookup fails or access is not permitted, a <seealso cref="ReflectiveOperationException"/> is thrown.
            '''  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
            ''' </summary>
            Public Overridable Function resolveOrFail(Of NoSuchMemberException As ReflectiveOperationException)(ByVal refKind As SByte, ByVal m As MemberName, ByVal lookupClass As [Class], ByVal nsmClass As [Class]) As MemberName
                Dim result As MemberName = resolve(refKind, m, lookupClass)
                If result.resolved Then Return result
                Dim ex As ReflectiveOperationException = result.makeAccessException()
                If TypeOf ex Is IllegalAccessException Then Throw CType(ex, IllegalAccessException)
                Throw nsmClass.cast(ex)
            End Function
            ''' <summary>
            ''' Produce a resolved version of the given member.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  If lookup fails or access is not permitted, return null.
            '''  Otherwise a fresh copy of the given member is returned, with modifier bits filled in.
            ''' </summary>
            Public Overridable Function resolveOrNull(ByVal refKind As SByte, ByVal m As MemberName, ByVal lookupClass As [Class]) As MemberName
                Dim result As MemberName = resolve(refKind, m, lookupClass)
                If result.resolved Then Return result
                Return Nothing
            End Function
            ''' <summary>
            ''' Return a list of all methods defined by the given class.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getMethods(ByVal defc As [Class], ByVal searchSupers As Boolean, ByVal lookupClass As [Class]) As IList(Of MemberName)
                Return getMethods(defc, searchSupers, Nothing, Nothing, lookupClass)
            End Function
            ''' <summary>
            ''' Return a list of matching methods defined by the given class.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Returned methods will match the name (if not null) and the type (if not null).
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getMethods(ByVal defc As [Class], ByVal searchSupers As Boolean, ByVal name As String, ByVal type As MethodType, ByVal lookupClass As [Class]) As IList(Of MemberName)
                Dim matchFlags As Integer = IS_METHOD Or (If(searchSupers, SEARCH_ALL_SUPERS, 0))
                Return getMembers(defc, name, type, matchFlags, lookupClass)
            End Function
            ''' <summary>
            ''' Return a list of all constructors defined by the given class.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getConstructors(ByVal defc As [Class], ByVal lookupClass As [Class]) As IList(Of MemberName)
                Return getMembers(defc, Nothing, Nothing, IS_CONSTRUCTOR, lookupClass)
            End Function
            ''' <summary>
            ''' Return a list of all fields defined by the given class.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getFields(ByVal defc As [Class], ByVal searchSupers As Boolean, ByVal lookupClass As [Class]) As IList(Of MemberName)
                Return getFields(defc, searchSupers, Nothing, Nothing, lookupClass)
            End Function
            ''' <summary>
            ''' Return a list of all fields defined by the given class.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Returned fields will match the name (if not null) and the type (if not null).
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getFields(ByVal defc As [Class], ByVal searchSupers As Boolean, ByVal name As String, ByVal type As [Class], ByVal lookupClass As [Class]) As IList(Of MemberName)
                Dim matchFlags As Integer = IS_FIELD Or (If(searchSupers, SEARCH_ALL_SUPERS, 0))
                Return getMembers(defc, name, type, matchFlags, lookupClass)
            End Function
            ''' <summary>
            ''' Return a list of all nested types defined by the given class.
            '''  Super types are searched (for inherited members) if {@code searchSupers} is true.
            '''  Access checking is performed on behalf of the given {@code lookupClass}.
            '''  Inaccessible members are not added to the last.
            ''' </summary>
            Public Overridable Function getNestedTypes(ByVal defc As [Class], ByVal searchSupers As Boolean, ByVal lookupClass As [Class]) As IList(Of MemberName)
                Dim matchFlags As Integer = IS_TYPE Or (If(searchSupers, SEARCH_ALL_SUPERS, 0))
                Return getMembers(defc, Nothing, Nothing, matchFlags, lookupClass)
            End Function
            Private Shared Function newMemberBuffer(ByVal length As Integer) As MemberName()
                Dim buf As MemberName() = New MemberName(length - 1) {}
                ' fill the buffer with dummy structs for the JVM to fill in
                For i As Integer = 0 To length - 1
                    buf(i) = New MemberName
                Next i
                Return buf
            End Function
        End Class

        '    static {
        '        System.out.println("Hello world!  My methods are:");
        '        System.out.println(Factory.INSTANCE.getMethods(MemberName.class, true, null));
        '    }
    End Class

End Namespace