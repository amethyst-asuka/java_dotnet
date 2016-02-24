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

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio



	Friend Class DirectCharBufferRS
		Inherits DirectCharBufferS
		Implements sun.nio.ch.DirectBuffer















































































































































		' For duplicates and slices
		'
		Friend Sub New(ByVal db As sun.nio.ch.DirectBuffer, ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer) ' package-private








			MyBase.New(db, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As CharBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 1)
			assert([off] >= 0)
			Return New DirectCharBufferRS(Me, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As CharBuffer
			Return New DirectCharBufferRS(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
		End Function

		Public Overrides Function asReadOnlyBuffer() As CharBuffer








			Return duplicate()

		End Function


























































		Public Overrides Function put(ByVal x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal src As CharBuffer) As CharBuffer




































			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal src As Char(), ByVal offset As Integer, ByVal length As Integer) As CharBuffer




























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As CharBuffer












			Throw New ReadOnlyBufferException

		End Function

		Public Property Overrides direct As Boolean
			Get
				Return True
			End Get
		End Property

		Public Property Overrides [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property




		Public Overrides Function ToString(ByVal start As Integer, ByVal [end] As Integer) As String
			If ([end] > limit()) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
			Try
				Dim len As Integer = [end] - start
				Dim ca As Char() = New Char(len - 1){}
				Dim cb As CharBuffer = CharBuffer.wrap(ca)
				Dim db As CharBuffer = Me.duplicate()
				db.position(start)
				db.limit([end])
				cb.put(db)
				Return New String(ca)
			Catch x As StringIndexOutOfBoundsException
				Throw New IndexOutOfBoundsException
			End Try
		End Function


		' --- Methods to support CharSequence ---

		Public Overrides Function subSequence(ByVal start As Integer, ByVal [end] As Integer) As CharBuffer
			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			pos = (If(pos <= lim, pos, lim))
			Dim len As Integer = lim - pos

			If (start < 0) OrElse ([end] > len) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
			Return New DirectCharBufferRS(Me, -1, pos + start, pos + [end], capacity(), offset)
		End Function







		Public Overrides Function order() As ByteOrder

			Return (If(ByteOrder.nativeOrder() Is ByteOrder.BIG_ENDIAN, ByteOrder.LITTLE_ENDIAN, ByteOrder.BIG_ENDIAN))





		End Function


























	End Class

End Namespace