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


	'
	' * Auxiliary to MethodHandleInfo, wants to nest in MethodHandleInfo but must be non-public.
	' 
	'non-public
	Friend NotInheritable Class InfoFromMemberName
		Implements MethodHandleInfo

		Private ReadOnly member As MemberName
		Private ReadOnly referenceKind As Integer

		Friend Sub New(  lookup As Lookup,   member As MemberName,   referenceKind As SByte)
			assert(member.resolved OrElse member.methodHandleInvoke)
			assert(member.referenceKindIsConsistentWith(referenceKind))
			Me.member = member
			Me.referenceKind = referenceKind
		End Sub

		Public  Overrides ReadOnly Property  declaringClass As  [Class] Implements MethodHandleInfo.getDeclaringClass
			Get
				Return member.declaringClass
			End Get
		End Property

		Public  Overrides ReadOnly Property  name As String Implements MethodHandleInfo.getName
			Get
				Return member.name
			End Get
		End Property

		Public  Overrides ReadOnly Property  methodType As MethodType Implements MethodHandleInfo.getMethodType
			Get
				Return member.methodOrFieldType
			End Get
		End Property

		Public  Overrides ReadOnly Property  modifiers As Integer Implements MethodHandleInfo.getModifiers
			Get
				Return member.modifiers
			End Get
		End Property

		Public  Overrides ReadOnly Property  referenceKind As Integer Implements MethodHandleInfo.getReferenceKind
			Get
				Return referenceKind
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return MethodHandleInfo.ToString(referenceKind, declaringClass, name, methodType)
		End Function

		Public Overrides Function reflectAs(Of T As Member)(  expected As [Class],   lookup As Lookup) As T Implements MethodHandleInfo.reflectAs
			If member.methodHandleInvoke AndAlso (Not member.varargs) Then Throw New IllegalArgumentException("cannot reflect signature polymorphic method")
			Dim mem As Member = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Try
				Dim defc As  [Class] = declaringClass
				Dim refKind As SByte = CByte(referenceKind)
				lookup.checkAccess(refKind, defc, convertToMemberName(refKind, mem))
			Catch ex As IllegalAccessException
				Throw New IllegalArgumentException(ex)
			End Try
			Return expected.cast(mem)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Member
				Try
					Return outerInstance.reflectUnchecked()
				Catch ex As ReflectiveOperationException
					Throw New IllegalArgumentException(ex)
				End Try
			End Function
		End Class

		Private Function reflectUnchecked() As Member
			Dim refKind As SByte = CByte(referenceKind)
			Dim defc As  [Class] = declaringClass
			Dim isPublic As Boolean = Modifier.isPublic(modifiers)
			If MethodHandleNatives.refKindIsMethod(refKind) Then
				If isPublic Then
					Return defc.getMethod(name, methodType.parameterArray())
				Else
					Return defc.getDeclaredMethod(name, methodType.parameterArray())
				End If
			ElseIf MethodHandleNatives.refKindIsConstructor(refKind) Then
				If isPublic Then
					Return defc.getConstructor(methodType.parameterArray())
				Else
					Return defc.getDeclaredConstructor(methodType.parameterArray())
				End If
			ElseIf MethodHandleNatives.refKindIsField(refKind) Then
				If isPublic Then
					Return defc.getField(name)
				Else
					Return defc.getDeclaredField(name)
				End If
			Else
				Throw New IllegalArgumentException("referenceKind=" & refKind)
			End If
		End Function

		Private Shared Function convertToMemberName(  refKind As SByte,   mem As Member) As MemberName
			If TypeOf mem Is Method Then
				Dim wantSpecial As Boolean = (refKind = REF_invokeSpecial)
				Return New MemberName(CType(mem, Method), wantSpecial)
			ElseIf TypeOf mem Is Constructor Then
				Return New MemberName(CType(mem, Constructor))
			ElseIf TypeOf mem Is Field Then
				Dim isSetter As Boolean = (refKind = REF_putField OrElse refKind = REF_putStatic)
				Return New MemberName(CType(mem, Field), isSetter)
			End If
			Throw New InternalError(mem.GetType().name)
		End Function
	End Class

End Namespace