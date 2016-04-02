'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio


	' ## If the sequence is a string, use reflection to share its array

	Friend Class StringCharBuffer
		Inherits CharBuffer
 ' package-private
		Friend str As CharSequence

		Friend Sub New(ByVal s As CharSequence, ByVal start As Integer, ByVal [end] As Integer) ' package-private
			MyBase.New(-1, start, [end], s.length())
			Dim n As Integer = s.length()
			If (start < 0) OrElse (start > n) OrElse ([end] < start) OrElse ([end] > n) Then Throw New IndexOutOfBoundsException
			str = s
		End Sub

		Public Overrides Function slice() As CharBuffer
			Return New StringCharBuffer(str, -1, 0, Me.remaining(), Me.remaining(), offset + Me.position())
		End Function

		Private Sub New(ByVal s As CharSequence, ByVal mark As Integer, ByVal pos As Integer, ByVal limit As Integer, ByVal cap As Integer, ByVal offset As Integer)
			MyBase.New(mark, pos, limit, cap, Nothing, offset)
			str = s
		End Sub

		Public Overrides Function duplicate() As CharBuffer
			Return New StringCharBuffer(str, markValue(), position(), limit(), capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As CharBuffer
			Return duplicate()
		End Function

		Public NotOverridable Overrides Function [get]() As Char
			Return str.Chars(nextGetIndex() + offset)
		End Function

		Public NotOverridable Overrides Function [get](ByVal index As Integer) As Char
			Return str.Chars(checkIndex(index) + offset)
		End Function

		Friend Overrides Function getUnchecked(ByVal index As Integer) As Char
			Return str.Chars(index + offset)
		End Function

		' ## Override bulk get methods for better performance

		Public NotOverridable Overrides Function put(ByVal c As Char) As CharBuffer
			Throw New ReadOnlyBufferException
		End Function

		Public NotOverridable Overrides Function put(ByVal index As Integer, ByVal c As Char) As CharBuffer
			Throw New ReadOnlyBufferException
		End Function

		Public NotOverridable Overrides Function compact() As CharBuffer
			Throw New ReadOnlyBufferException
		End Function

		Public Property NotOverridable Overrides [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property

		Friend NotOverridable Overrides Function ToString(ByVal start As Integer, ByVal [end] As Integer) As String
			Return str.ToString().Substring(start + offset, [end] + offset - (start + offset))
		End Function

		Public NotOverridable Overrides Function subSequence(ByVal start As Integer, ByVal [end] As Integer) As CharBuffer
			Try
				Dim pos As Integer = position()
				Return New StringCharBuffer(str, -1, pos + checkIndex(start, pos), pos + checkIndex([end], pos), capacity(), offset)
			Catch x As IllegalArgumentException
				Throw New IndexOutOfBoundsException
			End Try
		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function order() As ByteOrder
			Return ByteOrder.nativeOrder()
		End Function

	End Class

End Namespace