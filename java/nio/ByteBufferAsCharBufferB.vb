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


	Friend Class ByteBufferAsCharBufferB
		Inherits CharBuffer ' package-private



		Protected Friend ReadOnly bb As ByteBuffer
		Protected Friend ReadOnly Shadows offset As Integer



		Friend Sub New(  bb As ByteBuffer) ' package-private

			MyBase.New(-1, 0, bb.remaining() >> 1, bb.remaining() >> 1)
			Me.bb = bb
			' enforce limit == capacity
			Dim cap As Integer = Me.capacity()
			Me.limit(cap)
			Dim pos As Integer = Me.position()
			assert(pos <= cap)
			offset = pos



		End Sub

		Friend Sub New(  bb As ByteBuffer,   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)

			MyBase.New(mark, pos, lim, cap)
			Me.bb = bb
			offset = [off]



		End Sub

		Public Overrides Function slice() As CharBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 1) + offset
			assert([off] >= 0)
			Return New ByteBufferAsCharBufferB(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As CharBuffer
			Return New ByteBufferAsCharBufferB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As CharBuffer

			Return New ByteBufferAsCharBufferRB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



		End Function



		Protected Friend Overridable Function ix(  i As Integer) As Integer
			Return (i << 1) + offset
		End Function

		Public Overrides Function [get]() As Char
			Return Bits.getCharB(bb, ix(nextGetIndex()))
		End Function

		Public Overrides Function [get](  i As Integer) As Char
			Return Bits.getCharB(bb, ix(checkIndex(i)))
		End Function


	   Friend Overrides Function getUnchecked(  i As Integer) As Char
			Return Bits.getCharB(bb, ix(i))
	   End Function




		Public Overrides Function put(  x As Char) As CharBuffer

			Bits.putCharB(bb, ix(nextPutIndex()), x)
			Return Me



		End Function

		Public Overrides Function put(  i As Integer,   x As Char) As CharBuffer

			Bits.putCharB(bb, ix(checkIndex(i)), x)
			Return Me



		End Function

		Public Overrides Function compact() As CharBuffer

			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

			Dim db As ByteBuffer = bb.duplicate()
			db.limit(ix(lim))
			db.position(ix(0))
			Dim sb As ByteBuffer = db.slice()
			sb.position(pos << 1)
			sb.compact()
			position([rem])
			limit(capacity())
			discardMark()
			Return Me



		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return bb.direct
			End Get
		End Property

		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return False
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
			Return New ByteBufferAsCharBufferB(bb, -1, pos + start, pos + [end], capacity(), offset)
		End Function




		Public Overrides Function order() As ByteOrder

			Return ByteOrder.BIG_ENDIAN




		End Function

	End Class

End Namespace