'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect


	''' <summary>
	''' Package-private class implementing the
	'''    sun.reflect.LangReflectAccess interface, allowing the java.lang
	'''    package to instantiate objects in this package. 
	''' </summary>

	Friend Class ReflectAccess
		Implements sun.reflect.LangReflectAccess

		Public Overridable Function newField(ByVal declaringClass As Class, ByVal name As String, ByVal type As Class, ByVal modifiers As Integer, ByVal slot As Integer, ByVal signature As String, ByVal annotations As SByte()) As Field
			Return New Field(declaringClass, name, type, modifiers, slot, signature, annotations)
		End Function

		Public Overridable Function newMethod(ByVal declaringClass As Class, ByVal name As String, ByVal parameterTypes As Class(), ByVal returnType As Class, ByVal checkedExceptions As Class(), ByVal modifiers As Integer, ByVal slot As Integer, ByVal signature As String, ByVal annotations As SByte(), ByVal parameterAnnotations As SByte(), ByVal annotationDefault As SByte()) As Method
			Return New Method(declaringClass, name, parameterTypes, returnType, checkedExceptions, modifiers, slot, signature, annotations, parameterAnnotations, annotationDefault)
		End Function

		Public Overridable Function newConstructor(Of T)(ByVal declaringClass As Class, ByVal parameterTypes As Class(), ByVal checkedExceptions As Class(), ByVal modifiers As Integer, ByVal slot As Integer, ByVal signature As String, ByVal annotations As SByte(), ByVal parameterAnnotations As SByte()) As Constructor(Of T)
			Return New Constructor(Of )(declaringClass, parameterTypes, checkedExceptions, modifiers, slot, signature, annotations, parameterAnnotations)
		End Function

		Public Overridable Function getMethodAccessor(ByVal m As Method) As sun.reflect.MethodAccessor
			Return m.methodAccessor
		End Function

		Public Overridable Sub setMethodAccessor(ByVal m As Method, ByVal accessor As sun.reflect.MethodAccessor)
			m.methodAccessor = accessor
		End Sub

		Public Overridable Function getConstructorAccessor(Of T1)(ByVal c As Constructor(Of T1)) As sun.reflect.ConstructorAccessor
			Return c.constructorAccessor
		End Function

		Public Overridable Sub setConstructorAccessor(Of T1)(ByVal c As Constructor(Of T1), ByVal accessor As sun.reflect.ConstructorAccessor)
			c.constructorAccessor = accessor
		End Sub

		Public Overridable Function getConstructorSlot(Of T1)(ByVal c As Constructor(Of T1)) As Integer
			Return c.slot
		End Function

		Public Overridable Function getConstructorSignature(Of T1)(ByVal c As Constructor(Of T1)) As String
			Return c.signature
		End Function

		Public Overridable Function getConstructorAnnotations(Of T1)(ByVal c As Constructor(Of T1)) As SByte()
			Return c.rawAnnotations
		End Function

		Public Overridable Function getConstructorParameterAnnotations(Of T1)(ByVal c As Constructor(Of T1)) As SByte()
			Return c.rawParameterAnnotations
		End Function

		Public Overridable Function getExecutableTypeAnnotationBytes(ByVal ex As Executable) As SByte()
			Return ex.typeAnnotationBytes
		End Function

		'
		' Copying routines, needed to quickly fabricate new Field,
		' Method, and Constructor objects from templates
		'
		Public Overridable Function copyMethod(ByVal arg As Method) As Method
			Return arg.copy()
		End Function

		Public Overridable Function copyField(ByVal arg As Field) As Field
			Return arg.copy()
		End Function

		Public Overridable Function copyConstructor(Of T)(ByVal arg As Constructor(Of T)) As Constructor(Of T)
			Return arg.copy()
		End Function
	End Class

End Namespace