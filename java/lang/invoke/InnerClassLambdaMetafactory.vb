Imports System
Imports jdk.internal.org.objectweb.asm
Imports jdk.internal.org.objectweb.asm.Opcodes

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
	''' Lambda metafactory implementation which dynamically creates an
	''' inner-class-like class per lambda callsite.
	''' </summary>
	''' <seealso cref= LambdaMetafactory </seealso>
	' package 
	 Friend NotInheritable Class InnerClassLambdaMetafactory
		 Inherits AbstractValidatingLambdaMetafactory

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

		Private Const CLASSFILE_VERSION As Integer = 52
		Private Shared ReadOnly METHOD_DESCRIPTOR_VOID As String = Type.getMethodDescriptor(Type.VOID_TYPE)
		Private Const JAVA_LANG_OBJECT As String = "java/lang/Object"
		Private Const NAME_CTOR As String = "<init>"
		Private Const NAME_FACTORY As String = "get$Lambda"

		'Serialization support
		Private Const NAME_SERIALIZED_LAMBDA As String = "java/lang/invoke/SerializedLambda"
		Private Const NAME_NOT_SERIALIZABLE_EXCEPTION As String = "java/io/NotSerializableException"
		Private Const DESCR_METHOD_WRITE_REPLACE As String = "()Ljava/lang/Object;"
		Private Const DESCR_METHOD_WRITE_OBJECT As String = "(Ljava/io/ObjectOutputStream;)V"
		Private Const DESCR_METHOD_READ_OBJECT As String = "(Ljava/io/ObjectInputStream;)V"
		Private Const NAME_METHOD_WRITE_REPLACE As String = "writeReplace"
		Private Const NAME_METHOD_READ_OBJECT As String = "readObject"
		Private Const NAME_METHOD_WRITE_OBJECT As String = "writeObject"
		Private Shared ReadOnly DESCR_CTOR_SERIALIZED_LAMBDA As String = MethodType.methodType(GetType(void), GetType(Class), GetType(String), GetType(String), GetType(String), GetType(Integer), GetType(String), GetType(String), GetType(String), GetType(String), GetType(Object())).toMethodDescriptorString()
		Private Shared ReadOnly DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION As String = MethodType.methodType(GetType(void), GetType(String)).toMethodDescriptorString()
		Private Shared ReadOnly SER_HOSTILE_EXCEPTIONS As String() = {NAME_NOT_SERIALIZABLE_EXCEPTION}


		Private Shared ReadOnly EMPTY_STRING_ARRAY As String() = New String(){}

		' Used to ensure that each spun class name is unique
		Private Shared ReadOnly counter As New java.util.concurrent.atomic.AtomicInteger(0)

		' For dumping generated classes to disk, for debugging purposes
		Private Shared ReadOnly dumper As ProxyClassesDumper

		Shared Sub New()
			Const key As String = "jdk.internal.lambda.dumpProxyClasses"
			Dim path As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(key), Nothing, New java.util.PropertyPermission(key, "read"))
			dumper = If(Nothing Is path, Nothing, ProxyClassesDumper.getInstance(path))
		End Sub

		' See context values in AbstractValidatingLambdaMetafactory
		Private ReadOnly implMethodClassName As String ' Name of type containing implementation "CC"
		Private ReadOnly implMethodName As String ' Name of implementation method "impl"
		Private ReadOnly implMethodDesc As String ' Type descriptor for implementation methods "(I)Ljava/lang/String;"
		Private ReadOnly implMethodReturnClass As  [Class] ' class for implementaion method return type "Ljava/lang/String;"
		Private ReadOnly constructorType As MethodType ' Generated class constructor type "(CC)void"
		Private ReadOnly cw As  [Class]Writer ' ASM class writer
		Private ReadOnly argNames As String() ' Generated names for the constructor arguments
		Private ReadOnly argDescs As String() ' Type descriptors for the constructor arguments
		Private ReadOnly lambdaClassName As String ' Generated name for the generated class "X$$Lambda$1"

		''' <summary>
		''' General meta-factory constructor, supporting both standard cases and
		''' allowing for uncommon options such as serialization or bridging.
		''' </summary>
		''' <param name="caller"> Stacked automatically by VM; represents a lookup context
		'''               with the accessibility privileges of the caller. </param>
		''' <param name="invokedType"> Stacked automatically by VM; the signature of the
		'''                    invoked method, which includes the expected static
		'''                    type of the returned lambda object, and the static
		'''                    types of the captured arguments for the lambda.  In
		'''                    the event that the implementation method is an
		'''                    instance method, the first argument in the invocation
		'''                    signature will correspond to the receiver. </param>
		''' <param name="samMethodName"> Name of the method in the functional interface to
		'''                      which the lambda or method reference is being
		'''                      converted, represented as a String. </param>
		''' <param name="samMethodType"> Type of the method in the functional interface to
		'''                      which the lambda or method reference is being
		'''                      converted, represented as a MethodType. </param>
		''' <param name="implMethod"> The implementation method which should be called (with
		'''                   suitable adaptation of argument types, return types,
		'''                   and adjustment for captured arguments) when methods of
		'''                   the resulting functional interface instance are invoked. </param>
		''' <param name="instantiatedMethodType"> The signature of the primary functional
		'''                               interface method after type variables are
		'''                               substituted with their instantiation from
		'''                               the capture site </param>
		''' <param name="isSerializable"> Should the lambda be made serializable?  If set,
		'''                       either the target type or one of the additional SAM
		'''                       types must extend {@code Serializable}. </param>
		''' <param name="markerInterfaces"> Additional interfaces which the lambda object
		'''                       should implement. </param>
		''' <param name="additionalBridges"> Method types for additional signatures to be
		'''                          bridged to the implementation method </param>
		''' <exception cref="LambdaConversionException"> If any of the meta-factory protocol
		''' invariants are violated </exception>
		Public Sub New(ByVal caller As MethodHandles.Lookup, ByVal invokedType As MethodType, ByVal samMethodName As String, ByVal samMethodType As MethodType, ByVal implMethod As MethodHandle, ByVal instantiatedMethodType As MethodType, ByVal isSerializable As Boolean, ByVal markerInterfaces As  [Class](), ByVal additionalBridges As MethodType())
			MyBase.New(caller, invokedType, samMethodName, samMethodType, implMethod, instantiatedMethodType, isSerializable, markerInterfaces, additionalBridges)
			implMethodClassName = implDefiningClass.name.replace("."c, "/"c)
			implMethodName = implInfo.name
			implMethodDesc = implMethodType.toMethodDescriptorString()
			implMethodReturnClass = If(implKind = MethodHandleInfo.REF_newInvokeSpecial, implDefiningClass, implMethodType.returnType())
			constructorType = invokedType.changeReturnType(Void.TYPE)
			lambdaClassName = targetClass.name.replace("."c, "/"c) & "$$Lambda$" & counter.incrementAndGet()
			cw = New ClassWriter(ClassWriter.COMPUTE_MAXS)
			Dim parameterCount As Integer = invokedType.parameterCount()
			If parameterCount > 0 Then
				argNames = New String(parameterCount - 1){}
				argDescs = New String(parameterCount - 1){}
				For i As Integer = 0 To parameterCount - 1
					argNames(i) = "arg$" & (i + 1)
					argDescs(i) = sun.invoke.util.BytecodeDescriptor.unparse(invokedType.parameterType(i))
				Next i
			Else
					argDescs = EMPTY_STRING_ARRAY
					argNames = argDescs
			End If
		End Sub

		''' <summary>
		''' Build the CallSite. Generate a class file which implements the functional
		''' interface, define the [Class], if there are no parameters create an instance
		''' of the class which the CallSite will return, otherwise, generate handles
		''' which will call the class' constructor.
		''' </summary>
		''' <returns> a CallSite, which, when invoked, will return an instance of the
		''' functional interface </returns>
		''' <exception cref="ReflectiveOperationException"> </exception>
		''' <exception cref="LambdaConversionException"> If properly formed functional interface
		''' is not found </exception>
		Friend Overrides Function buildCallSite() As CallSite
			Dim innerClass As  [Class] = spinInnerClass()
			If invokedType.parameterCount() = 0 Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ctrs As Constructor(Of ?)() = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				If ctrs.Length <> 1 Then Throw New LambdaConversionException("Expected one lambda constructor for " & innerClass.canonicalName & ", got " & ctrs.Length)

				Try
					Dim inst As Object = ctrs(0).newInstance()
					Return New ConstantCallSite(MethodHandles.constant(samBase, inst))
				Catch e As ReflectiveOperationException
					Throw New LambdaConversionException("Exception instantiating lambda object", e)
				End Try
			Else
				Try
					UNSAFE.ensureClassInitialized(innerClass)
					Return New ConstantCallSite(MethodHandles.Lookup.IMPL_LOOKUP.findStatic(innerClass, NAME_FACTORY, invokedType))
				Catch e As ReflectiveOperationException
					Throw New LambdaConversionException("Exception finding constructor", e)
				End Try
			End If
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overrides Function run() As Constructor(Of ?)()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ctrs As Constructor(Of ?)() = innerClass.declaredConstructors
				If ctrs.Length = 1 Then ctrs(0).accessible = True
				Return ctrs
			End Function
		End Class

		''' <summary>
		''' Generate a class file which implements the functional
		''' interface, define and return the class.
		''' 
		''' @implNote The class that is generated does not include signature
		''' information for exceptions that may be present on the SAM method.
		''' This is to reduce classfile size, and is harmless as checked exceptions
		''' are erased anyway, no one will ever compile against this classfile,
		''' and we make no guarantees about the reflective properties of lambda
		''' objects.
		''' </summary>
		''' <returns> a Class which implements the functional interface </returns>
		''' <exception cref="LambdaConversionException"> If properly formed functional interface
		''' is not found </exception>
		Private Function spinInnerClass() As  [Class]
			Dim interfaces As String()
			Dim samIntf As String = samBase.name.replace("."c, "/"c)
			Dim accidentallySerializable As Boolean = (Not isSerializable) AndAlso samBase.IsSubclassOf(GetType(java.io.Serializable))
			If markerInterfaces.Length = 0 Then
				interfaces = New String(){samIntf}
			Else
				' Assure no duplicate interfaces (ClassFormatError)
				Dim itfs As java.util.Set(Of String) = New java.util.LinkedHashSet(Of String)(markerInterfaces.Length + 1)
				itfs.add(samIntf)
				For Each markerInterface As  [Class] In markerInterfaces
					itfs.add(markerInterface.name.replace("."c, "/"c))
					accidentallySerializable = accidentallySerializable Or (Not isSerializable) AndAlso markerInterface.IsSubclassOf(GetType(java.io.Serializable))
				Next markerInterface
				interfaces = itfs.ToArray(New String(itfs.size() - 1){})
			End If

			cw.visit(CLASSFILE_VERSION, ACC_SUPER + ACC_FINAL + ACC_SYNTHETIC, lambdaClassName, Nothing, JAVA_LANG_OBJECT, interfaces)

			' Generate final fields to be filled in by constructor
			For i As Integer = 0 To argDescs.Length - 1
				Dim fv As FieldVisitor = cw.visitField(ACC_PRIVATE + ACC_FINAL, argNames(i), argDescs(i), Nothing, Nothing)
				fv.visitEnd()
			Next i

			generateConstructor()

			If invokedType.parameterCount() <> 0 Then generateFactory()

			' Forward the SAM method
			Dim mv As MethodVisitor = cw.visitMethod(ACC_PUBLIC, samMethodName, samMethodType.toMethodDescriptorString(), Nothing, Nothing)
			mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", True)
			CType(New ForwardingMethodGenerator(Me, mv), ForwardingMethodGenerator).generate(samMethodType)

			' Forward the bridges
			If additionalBridges IsNot Nothing Then
				For Each mt As MethodType In additionalBridges
					mv = cw.visitMethod(ACC_PUBLIC Or ACC_BRIDGE, samMethodName, mt.toMethodDescriptorString(), Nothing, Nothing)
					mv.visitAnnotation("Ljava/lang/invoke/LambdaForm$Hidden;", True)
					CType(New ForwardingMethodGenerator(Me, mv), ForwardingMethodGenerator).generate(mt)
				Next mt
			End If

			If isSerializable Then
				generateSerializationFriendlyMethods()
			ElseIf accidentallySerializable Then
				generateSerializationHostileMethods()
			End If

			cw.visitEnd()

			' Define the generated class in this VM.

			Dim classBytes As SByte() = cw.toByteArray()

			' If requested, dump out to a file for debugging purposes
			If dumper IsNot Nothing Then
				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
				Dim TempFilePermission As java.io.FilePermission = New java.io.FilePermission("<<ALL FILES>>", "read, write"), New java.util.PropertyPermission("user.dir", "read"))
				' createDirectories may need it
			End If

			Return UNSAFE.defineAnonymousClass(targetClass, classBytes, Nothing)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overrides Function run() As Void
				dumper.dumpClass(outerInstance.lambdaClassName, classBytes)
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Generate the factory method for the class
		''' </summary>
		Private Sub generateFactory()
			Dim m As MethodVisitor = cw.visitMethod(ACC_PRIVATE Or ACC_STATIC, NAME_FACTORY, invokedType.toMethodDescriptorString(), Nothing, Nothing)
			m.visitCode()
			m.visitTypeInsn(NEW, lambdaClassName)
			m.visitInsn(Opcodes.DUP)
			Dim parameterCount As Integer = invokedType.parameterCount()
			Dim typeIndex As Integer = 0
			Dim varIndex As Integer = 0
			Do While typeIndex < parameterCount
				Dim argType As  [Class] = invokedType.parameterType(typeIndex)
				m.visitVarInsn(getLoadOpcode(argType), varIndex)
				varIndex += getParameterSize(argType)
				typeIndex += 1
			Loop
			m.visitMethodInsn(INVOKESPECIAL, lambdaClassName, NAME_CTOR, constructorType.toMethodDescriptorString(), False)
			m.visitInsn(ARETURN)
			m.visitMaxs(-1, -1)
			m.visitEnd()
		End Sub

		''' <summary>
		''' Generate the constructor for the class
		''' </summary>
		Private Sub generateConstructor()
			' Generate constructor
			Dim ctor As MethodVisitor = cw.visitMethod(ACC_PRIVATE, NAME_CTOR, constructorType.toMethodDescriptorString(), Nothing, Nothing)
			ctor.visitCode()
			ctor.visitVarInsn(ALOAD, 0)
			ctor.visitMethodInsn(INVOKESPECIAL, JAVA_LANG_OBJECT, NAME_CTOR, METHOD_DESCRIPTOR_VOID, False)
			Dim parameterCount As Integer = invokedType.parameterCount()
			Dim i As Integer = 0
			Dim lvIndex As Integer = 0
			Do While i < parameterCount
				ctor.visitVarInsn(ALOAD, 0)
				Dim argType As  [Class] = invokedType.parameterType(i)
				ctor.visitVarInsn(getLoadOpcode(argType), lvIndex + 1)
				lvIndex += getParameterSize(argType)
				ctor.visitFieldInsn(PUTFIELD, lambdaClassName, argNames(i), argDescs(i))
				i += 1
			Loop
			ctor.visitInsn(RETURN)
			' Maxs computed by ClassWriter.COMPUTE_MAXS, these arguments ignored
			ctor.visitMaxs(-1, -1)
			ctor.visitEnd()
		End Sub

		''' <summary>
		''' Generate a writeReplace method that supports serialization
		''' </summary>
		Private Sub generateSerializationFriendlyMethods()
			Dim mv As New TypeConvertingMethodAdapter(cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_WRITE_REPLACE, DESCR_METHOD_WRITE_REPLACE, Nothing, Nothing))

			mv.visitCode()
			mv.visitTypeInsn(NEW, NAME_SERIALIZED_LAMBDA)
			mv.visitInsn(DUP)
			mv.visitLdcInsn(Type.getType(targetClass))
			mv.visitLdcInsn(invokedType.returnType().name.replace("."c, "/"c))
			mv.visitLdcInsn(samMethodName)
			mv.visitLdcInsn(samMethodType.toMethodDescriptorString())
			mv.visitLdcInsn(implInfo.referenceKind)
			mv.visitLdcInsn(implInfo.declaringClass.name.replace("."c, "/"c))
			mv.visitLdcInsn(implInfo.name)
			mv.visitLdcInsn(implInfo.methodType.toMethodDescriptorString())
			mv.visitLdcInsn(instantiatedMethodType.toMethodDescriptorString())
			mv.iconst(argDescs.Length)
			mv.visitTypeInsn(ANEWARRAY, JAVA_LANG_OBJECT)
			For i As Integer = 0 To argDescs.Length - 1
				mv.visitInsn(DUP)
				mv.iconst(i)
				mv.visitVarInsn(ALOAD, 0)
				mv.visitFieldInsn(GETFIELD, lambdaClassName, argNames(i), argDescs(i))
				mv.boxIfTypePrimitive(Type.getType(argDescs(i)))
				mv.visitInsn(AASTORE)
			Next i
			mv.visitMethodInsn(INVOKESPECIAL, NAME_SERIALIZED_LAMBDA, NAME_CTOR, DESCR_CTOR_SERIALIZED_LAMBDA, False)
			mv.visitInsn(ARETURN)
			' Maxs computed by ClassWriter.COMPUTE_MAXS, these arguments ignored
			mv.visitMaxs(-1, -1)
			mv.visitEnd()
		End Sub

		''' <summary>
		''' Generate a readObject/writeObject method that is hostile to serialization
		''' </summary>
		Private Sub generateSerializationHostileMethods()
			Dim mv As MethodVisitor = cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_WRITE_OBJECT, DESCR_METHOD_WRITE_OBJECT, Nothing, SER_HOSTILE_EXCEPTIONS)
			mv.visitCode()
			mv.visitTypeInsn(NEW, NAME_NOT_SERIALIZABLE_EXCEPTION)
			mv.visitInsn(DUP)
			mv.visitLdcInsn("Non-serializable lambda")
			mv.visitMethodInsn(INVOKESPECIAL, NAME_NOT_SERIALIZABLE_EXCEPTION, NAME_CTOR, DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION, False)
			mv.visitInsn(ATHROW)
			mv.visitMaxs(-1, -1)
			mv.visitEnd()

			mv = cw.visitMethod(ACC_PRIVATE + ACC_FINAL, NAME_METHOD_READ_OBJECT, DESCR_METHOD_READ_OBJECT, Nothing, SER_HOSTILE_EXCEPTIONS)
			mv.visitCode()
			mv.visitTypeInsn(NEW, NAME_NOT_SERIALIZABLE_EXCEPTION)
			mv.visitInsn(DUP)
			mv.visitLdcInsn("Non-serializable lambda")
			mv.visitMethodInsn(INVOKESPECIAL, NAME_NOT_SERIALIZABLE_EXCEPTION, NAME_CTOR, DESCR_CTOR_NOT_SERIALIZABLE_EXCEPTION, False)
			mv.visitInsn(ATHROW)
			mv.visitMaxs(-1, -1)
			mv.visitEnd()
		End Sub

		''' <summary>
		''' This class generates a method body which calls the lambda implementation
		''' method, converting arguments, as needed.
		''' </summary>
		Private Class ForwardingMethodGenerator
			Inherits TypeConvertingMethodAdapter

			Private ReadOnly outerInstance As InnerClassLambdaMetafactory


			Friend Sub New(ByVal outerInstance As InnerClassLambdaMetafactory, ByVal mv As MethodVisitor)
					Me.outerInstance = outerInstance
				MyBase.New(mv)
			End Sub

			Friend Overridable Sub generate(ByVal methodType_Renamed As MethodType)
				visitCode()

				If outerInstance.implKind = MethodHandleInfo.REF_newInvokeSpecial Then
					visitTypeInsn(NEW, outerInstance.implMethodClassName)
					visitInsn(DUP)
				End If
				For i As Integer = 0 To outerInstance.argNames.Length - 1
					visitVarInsn(ALOAD, 0)
					visitFieldInsn(GETFIELD, outerInstance.lambdaClassName, outerInstance.argNames(i), outerInstance.argDescs(i))
				Next i

				convertArgumentTypes(methodType_Renamed)

				' Invoke the method we want to forward to
				visitMethodInsn(invocationOpcode(), outerInstance.implMethodClassName, outerInstance.implMethodName, outerInstance.implMethodDesc, outerInstance.implDefiningClass.interface)

				' Convert the return value (if any) and return it
				' Note: if adapting from non-void to void, the 'return'
				' instruction will pop the unneeded result
				Dim samReturnClass As  [Class] = methodType_Renamed.returnType()
				convertType(outerInstance.implMethodReturnClass, samReturnClass, samReturnClass)
				visitInsn(getReturnOpcode(samReturnClass))
				' Maxs computed by ClassWriter.COMPUTE_MAXS,these arguments ignored
				visitMaxs(-1, -1)
				visitEnd()
			End Sub

			Private Sub convertArgumentTypes(ByVal samType As MethodType)
				Dim lvIndex As Integer = 0
				Dim samIncludesReceiver As Boolean = outerInstance.implIsInstanceMethod AndAlso outerInstance.invokedType.parameterCount() = 0
				Dim samReceiverLength As Integer = If(samIncludesReceiver, 1, 0)
				If samIncludesReceiver Then
					' push receiver
					Dim rcvrType As  [Class] = samType.parameterType(0)
					visitVarInsn(getLoadOpcode(rcvrType), lvIndex + 1)
					lvIndex += getParameterSize(rcvrType)
					convertType(rcvrType, outerInstance.implDefiningClass, outerInstance.instantiatedMethodType.parameterType(0))
				End If
				Dim samParametersLength As Integer = samType.parameterCount()
				Dim argOffset As Integer = outerInstance.implMethodType.parameterCount() - samParametersLength
				For i As Integer = samReceiverLength To samParametersLength - 1
					Dim argType As  [Class] = samType.parameterType(i)
					visitVarInsn(getLoadOpcode(argType), lvIndex + 1)
					lvIndex += getParameterSize(argType)
					convertType(argType, outerInstance.implMethodType.parameterType(argOffset + i), outerInstance.instantiatedMethodType.parameterType(i))
				Next i
			End Sub

			Private Function invocationOpcode() As Integer
				Select Case outerInstance.implKind
					Case MethodHandleInfo.REF_invokeStatic
						Return INVOKESTATIC
					Case MethodHandleInfo.REF_newInvokeSpecial
						Return INVOKESPECIAL
					 Case MethodHandleInfo.REF_invokeVirtual
						Return INVOKEVIRTUAL
					Case MethodHandleInfo.REF_invokeInterface
						Return INVOKEINTERFACE
					Case MethodHandleInfo.REF_invokeSpecial
						Return INVOKESPECIAL
					Case Else
						Throw New InternalError("Unexpected invocation kind: " & outerInstance.implKind)
				End Select
			End Function
		End Class

		Friend Shared Function getParameterSize(ByVal c As [Class]) As Integer
			If c Is Void.TYPE Then
				Return 0
			ElseIf c Is java.lang.[Long].TYPE OrElse c Is java.lang.[Double].TYPE Then
				Return 2
			End If
			Return 1
		End Function

		Friend Shared Function getLoadOpcode(ByVal c As [Class]) As Integer
			If c Is Void.TYPE Then Throw New InternalError("Unexpected  Sub  type of load opcode")
			Return ILOAD + getOpcodeOffset(c)
		End Function

		Friend Shared Function getReturnOpcode(ByVal c As [Class]) As Integer
			If c Is Void.TYPE Then Return RETURN
			Return IRETURN + getOpcodeOffset(c)
		End Function

		Private Shared Function getOpcodeOffset(ByVal c As [Class]) As Integer
			If c.primitive Then
				If c Is java.lang.[Long].TYPE Then
					Return 1
				ElseIf c Is Float.TYPE Then
					Return 2
				ElseIf c Is java.lang.[Double].TYPE Then
					Return 3
				End If
				Return 0
			Else
				Return 4
			End If
		End Function

	 End Class

End Namespace