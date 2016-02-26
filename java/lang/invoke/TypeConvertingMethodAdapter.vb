Imports sun.invoke.util.Wrapper

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

	Friend Class TypeConvertingMethodAdapter
		Inherits jdk.internal.org.objectweb.asm.MethodVisitor

		Friend Sub New(ByVal mv As jdk.internal.org.objectweb.asm.MethodVisitor)
			MyBase.New(jdk.internal.org.objectweb.asm.Opcodes.ASM5, mv)
		End Sub

		Private Shared ReadOnly NUM_WRAPPERS As Integer = sun.invoke.util.Wrapper.values().length

		Private Const NAME_OBJECT As String = "java/lang/Object"
		Private Const WRAPPER_PREFIX As String = "Ljava/lang/"

		' Same for all primitives; name of the boxing method
		Private Const NAME_BOX_METHOD As String = "valueOf"

		' Table of opcodes for widening primitive conversions; NOP = no conversion
		Private Shared ReadOnly wideningOpcodes As Integer()() = RectangularArrays.ReturnRectangularIntegerArray(NUM_WRAPPERS, NUM_WRAPPERS)

		Private Shared ReadOnly FROM_WRAPPER_NAME As sun.invoke.util.Wrapper() = New sun.invoke.util.Wrapper(15){}

		' Table of wrappers for primitives, indexed by ASM type sorts
		Private Shared ReadOnly FROM_TYPE_SORT As sun.invoke.util.Wrapper() = New sun.invoke.util.Wrapper(15){}

		Shared Sub New()
			For Each w As sun.invoke.util.Wrapper In sun.invoke.util.Wrapper.values()
				If w.basicTypeChar() <> "L"c Then
					Dim wi As Integer = hashWrapperName(w.wrapperSimpleName())
					assert(FROM_WRAPPER_NAME(wi) Is Nothing)
					FROM_WRAPPER_NAME(wi) = w
				End If
			Next w

			For i As Integer = 0 To NUM_WRAPPERS - 1
				For j As Integer = 0 To NUM_WRAPPERS - 1
					wideningOpcodes(i)(j) = jdk.internal.org.objectweb.asm.Opcodes.NOP
				Next j
			Next i

			initWidening(LONG, jdk.internal.org.objectweb.asm.Opcodes.I2L, BYTE, SHORT, INT, CHAR)
			initWidening(LONG, jdk.internal.org.objectweb.asm.Opcodes.F2L, FLOAT)
			initWidening(FLOAT, jdk.internal.org.objectweb.asm.Opcodes.I2F, BYTE, SHORT, INT, CHAR)
			initWidening(FLOAT, jdk.internal.org.objectweb.asm.Opcodes.L2F, LONG)
			initWidening(DOUBLE, jdk.internal.org.objectweb.asm.Opcodes.I2D, BYTE, SHORT, INT, CHAR)
			initWidening(DOUBLE, jdk.internal.org.objectweb.asm.Opcodes.F2D, FLOAT)
			initWidening(DOUBLE, jdk.internal.org.objectweb.asm.Opcodes.L2D, LONG)

			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.BYTE) = sun.invoke.util.Wrapper.BYTE
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.SHORT) = sun.invoke.util.Wrapper.SHORT
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.INT) = sun.invoke.util.Wrapper.INT
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.LONG) = sun.invoke.util.Wrapper.LONG
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.CHAR) = sun.invoke.util.Wrapper.CHAR
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.FLOAT) = sun.invoke.util.Wrapper.FLOAT
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.DOUBLE) = sun.invoke.util.Wrapper.DOUBLE
			FROM_TYPE_SORT(jdk.internal.org.objectweb.asm.Type.BOOLEAN) = sun.invoke.util.Wrapper.BOOLEAN
		End Sub

		Private Shared Sub initWidening(ByVal [to] As sun.invoke.util.Wrapper, ByVal opcode As Integer, ParamArray ByVal [from] As sun.invoke.util.Wrapper())
			For Each f As sun.invoke.util.Wrapper In [from]
				wideningOpcodes(f.ordinal())([to].ordinal()) = opcode
			Next f
		End Sub

		''' <summary>
		''' Class name to Wrapper hash, derived from Wrapper.hashWrap() </summary>
		''' <param name="xn"> </param>
		''' <returns> The hash code 0-15 </returns>
		Private Shared Function hashWrapperName(ByVal xn As String) As Integer
			If xn.length() < 3 Then Return 0
			Return (3 * AscW(xn.Chars(1)) + AscW(xn.Chars(2))) Mod 16
		End Function

		Private Function wrapperOrNullFromDescriptor(ByVal desc As String) As sun.invoke.util.Wrapper
			If Not desc.StartsWith(WRAPPER_PREFIX) Then Return Nothing
			' Pare it down to the simple class name
			Dim cname As String = desc.Substring(WRAPPER_PREFIX.length(), desc.length() - 1 - (WRAPPER_PREFIX.length()))
			' Hash to a Wrapper
			Dim w As sun.invoke.util.Wrapper = FROM_WRAPPER_NAME(hashWrapperName(cname))
			If w Is Nothing OrElse w.wrapperSimpleName().Equals(cname) Then
				Return w
			Else
				Return Nothing
			End If
		End Function

		Private Shared Function wrapperName(ByVal w As sun.invoke.util.Wrapper) As String
			Return "java/lang/" & w.wrapperSimpleName()
		End Function

		Private Shared Function unboxMethod(ByVal w As sun.invoke.util.Wrapper) As String
			Return w.primitiveSimpleName() & "Value"
		End Function

		Private Shared Function boxingDescriptor(ByVal w As sun.invoke.util.Wrapper) As String
			Return String.Format("({0})L{1};", w.basicTypeChar(), wrapperName(w))
		End Function

		Private Shared Function unboxingDescriptor(ByVal w As sun.invoke.util.Wrapper) As String
			Return "()" & w.basicTypeChar()
		End Function

		Friend Overridable Sub boxIfTypePrimitive(ByVal t As jdk.internal.org.objectweb.asm.Type)
			Dim w As sun.invoke.util.Wrapper = FROM_TYPE_SORT(t.sort)
			If w IsNot Nothing Then box(w)
		End Sub

		Friend Overridable Sub widen(ByVal ws As sun.invoke.util.Wrapper, ByVal wt As sun.invoke.util.Wrapper)
			If ws IsNot wt Then
				Dim opcode As Integer = wideningOpcodes(ws.ordinal())(wt.ordinal())
				If opcode <> jdk.internal.org.objectweb.asm.Opcodes.NOP Then visitInsn(opcode)
			End If
		End Sub

		Friend Overridable Sub box(ByVal w As sun.invoke.util.Wrapper)
			visitMethodInsn(jdk.internal.org.objectweb.asm.Opcodes.INVOKESTATIC, wrapperName(w), NAME_BOX_METHOD, boxingDescriptor(w), False)
		End Sub

		''' <summary>
		''' Convert types by unboxing. The source type is known to be a primitive wrapper. </summary>
		''' <param name="sname"> A primitive wrapper corresponding to wrapped reference source type </param>
		''' <param name="wt"> A primitive wrapper being converted to </param>
		Friend Overridable Sub unbox(ByVal sname As String, ByVal wt As sun.invoke.util.Wrapper)
			visitMethodInsn(jdk.internal.org.objectweb.asm.Opcodes.INVOKEVIRTUAL, sname, unboxMethod(wt), unboxingDescriptor(wt), False)
		End Sub

		Private Function descriptorToName(ByVal desc As String) As String
			Dim last As Integer = desc.length() - 1
			If desc.Chars(0) = "L"c AndAlso desc.Chars(last) = ";"c Then
				' In descriptor form
				Return desc.Substring(1, last - 1)
			Else
				' Already in internal name form
				Return desc
			End If
		End Function

		Friend Overridable Sub cast(ByVal ds As String, ByVal dt As String)
			Dim ns As String = descriptorToName(ds)
			Dim nt As String = descriptorToName(dt)
			If (Not nt.Equals(ns)) AndAlso (Not nt.Equals(NAME_OBJECT)) Then visitTypeInsn(jdk.internal.org.objectweb.asm.Opcodes.CHECKCAST, nt)
		End Sub

		Private Function isPrimitive(ByVal w As sun.invoke.util.Wrapper) As Boolean
			Return w IsNot OBJECT
		End Function

		Private Function toWrapper(ByVal desc As String) As sun.invoke.util.Wrapper
			Dim first As Char = desc.Chars(0)
			If first = "["c OrElse first = "("c Then first = "L"c
			Return sun.invoke.util.Wrapper.forBasicType(first)
		End Function

		''' <summary>
		''' Convert an argument of type 'arg' to be passed to 'target' assuring that it is 'functional'.
		''' Insert the needed conversion instructions in the method code. </summary>
		''' <param name="arg"> </param>
		''' <param name="target"> </param>
		''' <param name="functional"> </param>
		Friend Overridable Sub convertType(ByVal arg As [Class], ByVal target As [Class], ByVal functional As [Class])
			If arg.Equals(target) AndAlso arg.Equals(functional) Then Return
			If arg Is Void.TYPE OrElse target Is Void.TYPE Then Return
			If arg.primitive Then
				Dim wArg As sun.invoke.util.Wrapper = sun.invoke.util.Wrapper.forPrimitiveType(arg)
				If target.primitive Then
					' Both primitives: widening
					widen(wArg, sun.invoke.util.Wrapper.forPrimitiveType(target))
				Else
					' Primitive argument to reference target
					Dim dTarget As String = sun.invoke.util.BytecodeDescriptor.unparse(target)
					Dim wPrimTarget As sun.invoke.util.Wrapper = wrapperOrNullFromDescriptor(dTarget)
					If wPrimTarget IsNot Nothing Then
						' The target is a boxed primitive type, widen to get there before boxing
						widen(wArg, wPrimTarget)
						box(wPrimTarget)
					Else
						' Otherwise, box and cast
						box(wArg)
						cast(wrapperName(wArg), dTarget)
					End If
				End If
			Else
				Dim dArg As String = sun.invoke.util.BytecodeDescriptor.unparse(arg)
				Dim dSrc As String
				If functional.primitive Then
					dSrc = dArg
				Else
					' Cast to convert to possibly more specific type, and generate CCE for invalid arg
					dSrc = sun.invoke.util.BytecodeDescriptor.unparse(functional)
					cast(dArg, dSrc)
				End If
				Dim dTarget As String = sun.invoke.util.BytecodeDescriptor.unparse(target)
				If target.primitive Then
					Dim wTarget As sun.invoke.util.Wrapper = toWrapper(dTarget)
					' Reference argument to primitive target
					Dim wps As sun.invoke.util.Wrapper = wrapperOrNullFromDescriptor(dSrc)
					If wps IsNot Nothing Then
						If wps.signed OrElse wps.floating Then
							' Boxed number to primitive
							unbox(wrapperName(wps), wTarget)
						Else
							' Character or Boolean
							unbox(wrapperName(wps), wps)
							widen(wps, wTarget)
						End If
					Else
						' Source type is reference type, but not boxed type,
						' assume it is super type of target type
						Dim intermediate As String
						If wTarget.signed OrElse wTarget.floating Then
							' Boxed number to primitive
							intermediate = "java/lang/Number"
						Else
							' Character or Boolean
							intermediate = wrapperName(wTarget)
						End If
						cast(dSrc, intermediate)
						unbox(intermediate, wTarget)
					End If
				Else
					' Both reference types: just case to target type
					cast(dSrc, dTarget)
				End If
			End If
		End Sub

		''' <summary>
		''' The following method is copied from
		''' org.objectweb.asm.commons.InstructionAdapter. Part of ASM: a very small
		''' and fast Java bytecode manipulation framework.
		''' Copyright (c) 2000-2005 INRIA, France Telecom All rights reserved.
		''' </summary>
		Friend Overridable Sub iconst(ByVal cst As Integer)
			If cst >= -1 AndAlso cst <= 5 Then
				mv.visitInsn(jdk.internal.org.objectweb.asm.Opcodes.ICONST_0 + cst)
			ElseIf cst >= Byte.MinValue AndAlso cst <= Byte.MaxValue Then
				mv.visitIntInsn(jdk.internal.org.objectweb.asm.Opcodes.BIPUSH, cst)
			ElseIf cst >= Short.MinValue AndAlso cst <= Short.MaxValue Then
				mv.visitIntInsn(jdk.internal.org.objectweb.asm.Opcodes.SIPUSH, cst)
			Else
				mv.visitLdcInsn(cst)
			End If
		End Sub
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
    Friend Shared Function ReturnRectangularIntegerArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As Integer()()
        Dim Array As Integer()() = New Integer(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New Integer(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class