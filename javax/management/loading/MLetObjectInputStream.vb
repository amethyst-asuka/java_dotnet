Imports System

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.loading


	' java import



	''' <summary>
	''' This subclass of ObjectInputStream delegates loading of classes to
	''' an existing MLetClassLoader.
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class MLetObjectInputStream
		Inherits ObjectInputStream

		Private loader As MLet

		''' <summary>
		''' Loader must be non-null;
		''' </summary>
		Public Sub New(ByVal [in] As InputStream, ByVal loader As MLet)

			MyBase.New([in])
			If loader Is Nothing Then Throw New System.ArgumentException("Illegal null argument to MLetObjectInputStream")
			Me.loader = loader
		End Sub

		Private Function primitiveType(ByVal c As Char) As Type
			Select Case c
			Case "B"c
				Return SByte.TYPE

			Case "C"c
				Return Char.TYPE

			Case "D"c
				Return Double.TYPE

			Case "F"c
				Return Single.TYPE

			Case "I"c
				Return Integer.TYPE

			Case "J"c
				Return Long.TYPE

			Case "S"c
				Return Short.TYPE

			Case "Z"c
				Return Boolean.TYPE
			End Select
			Return Nothing
		End Function

		''' <summary>
		''' Use the given ClassLoader rather than using the system class
		''' </summary>
		Protected Friend Overrides Function resolveClass(ByVal objectstreamclass As ObjectStreamClass) As Type

			Dim s As String = objectstreamclass.name
			If s.StartsWith("[") Then
				Dim i As Integer
				i = 1
				Do While s.Chars(i) = "["c

					i += 1
				Loop
				Dim class1 As Type
				If s.Chars(i) = "L"c Then
					class1 = loader.loadClass(s.Substring(i + 1, s.Length - 1 - (i + 1)))
				Else
					If s.Length <> i + 1 Then Throw New ClassNotFoundException(s)
					class1 = primitiveType(s.Chars(i))
				End If
				Dim ai As Integer() = New Integer(i - 1){}
				For j As Integer = 0 To i - 1
					ai(j) = 0
				Next j

				Return Array.newInstance(class1, ai).GetType()
			Else
				Return loader.loadClass(s)
			End If
		End Function

		''' <summary>
		''' Returns the ClassLoader being used
		''' </summary>
		Public Overridable Property classLoader As ClassLoader
			Get
				Return loader
			End Get
		End Property
	End Class

End Namespace