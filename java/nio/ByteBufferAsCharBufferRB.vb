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


	Friend Class ByteBufferAsCharBufferRB
		Inherits ByteBufferAsCharBufferB ' package-private








		Friend Sub New(  bb As ByteBuffer) ' package-private












			MyBase.New(bb)

		End Sub

		Friend Sub New(  bb As ByteBuffer,   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)





			MyBase.New(bb, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As CharBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 1) + offset
			assert([off] >= 0)
			Return New ByteBufferAsCharBufferRB(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As CharBuffer
			Return New ByteBufferAsCharBufferRB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As CharBuffer








			Return duplicate()

		End Function























		Public Overrides Function put(  x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As Char) As CharBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As CharBuffer

















			Throw New ReadOnlyBufferException

		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return bb.direct
			End Get
		End Property

		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property



		Public Overrides Function ToString(  start As Integer,   [end] As Integer) As String
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

		Public Overrides Function subSequence(  start As Integer,   [end] As Integer) As CharBuffer
			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			pos = (If(pos <= lim, pos, lim))
			Dim len As Integer = lim - pos

			If (start < 0) OrElse ([end] > len) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
			Return New ByteBufferAsCharBufferRB(bb, -1, pos + start, pos + [end], capacity(), offset)
		End Function




		Public Overrides Function order() As ByteOrder

			Return ByteOrder.BIG_ENDIAN




		End Function

	End Class

End Namespace