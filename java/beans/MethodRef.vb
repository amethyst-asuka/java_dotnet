import static sun.reflect.misc.ReflectUtil.isPackageAccessible

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	Friend NotInheritable Class MethodRef
		Private signature As String
		Private methodRef As SoftReference(Of Method)
		Private typeRef As WeakReference(Of [Class])

		Friend Sub [set](  method As Method)
			If method Is Nothing Then
				Me.signature = Nothing
				Me.methodRef = Nothing
				Me.typeRef = Nothing
			Else
				Me.signature = method.toGenericString()
				Me.methodRef = New SoftReference(Of )(method)
				Me.typeRef = New WeakReference(Of [Class])(method.declaringClass)
			End If
		End Sub

		Friend Property [set] As Boolean
			Get
				Return Me.methodRef IsNot Nothing
			End Get
		End Property

		Friend Function [get]() As Method
			If Me.methodRef Is Nothing Then Return Nothing
			Dim method As Method = Me.methodRef.get()
			If method Is Nothing Then
				method = find(Me.typeRef.get(), Me.signature)
				If method Is Nothing Then
					Me.signature = Nothing
					Me.methodRef = Nothing
					Me.typeRef = Nothing
				Else
					Me.methodRef = New SoftReference(Of )(method)
				End If
			End If
			Return If(isPackageAccessible(method.declaringClass), method, Nothing)
		End Function

		Private Shared Function find(  type As [Class],   signature As String) As Method
			If type IsNot Nothing Then
				For Each method As Method In type.methods
					If type.Equals(method.declaringClass) Then
						If method.toGenericString().Equals(signature) Then Return method
					End If
				Next method
			End If
			Return Nothing
		End Function
	End Class

End Namespace