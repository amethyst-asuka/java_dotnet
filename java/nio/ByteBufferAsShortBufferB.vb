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


	Friend Class ByteBufferAsShortBufferB
		Inherits ShortBuffer ' package-private



		Protected Friend ReadOnly bb As ByteBuffer
		Protected Friend ReadOnly Shadows offset As Integer



		Friend Sub New(ByVal bb As ByteBuffer) ' package-private

			MyBase.New(-1, 0, bb.remaining() >> 1, bb.remaining() >> 1)
			Me.bb = bb
			' enforce limit == capacity
			Dim cap As Integer = Me.capacity()
			Me.limit(cap)
			Dim pos As Integer = Me.position()
			assert(pos <= cap)
			offset = pos



		End Sub

		Friend Sub New(ByVal bb As ByteBuffer, ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer)

			MyBase.New(mark, pos, lim, cap)
			Me.bb = bb
			offset = [off]



		End Sub

		Public Overrides Function slice() As ShortBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 1) + offset
			assert([off] >= 0)
			Return New ByteBufferAsShortBufferB(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As ShortBuffer
			Return New ByteBufferAsShortBufferB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ShortBuffer

			Return New ByteBufferAsShortBufferRB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



		End Function



		Protected Friend Overridable Function ix(ByVal i As Integer) As Integer
			Return (i << 1) + offset
		End Function

		Public Overrides Function [get]() As Short
			Return Bits.getShortB(bb, ix(nextGetIndex()))
		End Function

		Public Overrides Function [get](ByVal i As Integer) As Short
			Return Bits.getShortB(bb, ix(checkIndex(i)))
		End Function









		Public Overrides Function put(ByVal x As Short) As ShortBuffer

			Bits.putShortB(bb, ix(nextPutIndex()), x)
			Return Me



		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Short) As ShortBuffer

			Bits.putShortB(bb, ix(checkIndex(i)), x)
			Return Me



		End Function

		Public Overrides Function compact() As ShortBuffer

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











































		Public Overrides Function order() As ByteOrder

			Return ByteOrder.BIG_ENDIAN




		End Function

	End Class

End Namespace