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


	Friend Class ByteBufferAsFloatBufferL
		Inherits FloatBuffer ' package-private



		Protected Friend ReadOnly bb As ByteBuffer
		Protected Friend ReadOnly Shadows offset As Integer



		Friend Sub New(ByVal bb As ByteBuffer) ' package-private

			MyBase.New(-1, 0, bb.remaining() >> 2, bb.remaining() >> 2)
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

		Public Overrides Function slice() As FloatBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 2) + offset
			assert([off] >= 0)
			Return New ByteBufferAsFloatBufferL(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As FloatBuffer
			Return New ByteBufferAsFloatBufferL(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As FloatBuffer

			Return New ByteBufferAsFloatBufferRL(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



		End Function



		Protected Friend Overridable Function ix(ByVal i As Integer) As Integer
			Return (i << 2) + offset
		End Function

		Public Overrides Function [get]() As Single
			Return Bits.getFloatL(bb, ix(nextGetIndex()))
		End Function

		Public Overrides Function [get](ByVal i As Integer) As Single
			Return Bits.getFloatL(bb, ix(checkIndex(i)))
		End Function









		Public Overrides Function put(ByVal x As Single) As FloatBuffer

			Bits.putFloatL(bb, ix(nextPutIndex()), x)
			Return Me



		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Single) As FloatBuffer

			Bits.putFloatL(bb, ix(checkIndex(i)), x)
			Return Me



		End Function

		Public Overrides Function compact() As FloatBuffer

			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

			Dim db As ByteBuffer = bb.duplicate()
			db.limit(ix(lim))
			db.position(ix(0))
			Dim sb As ByteBuffer = db.slice()
			sb.position(pos << 2)
			sb.compact()
			position([rem])
			limit(capacity())
			discardMark()
			Return Me



		End Function

		Public Property Overrides direct As Boolean
			Get
				Return bb.direct
			End Get
		End Property

		Public Property Overrides [readOnly] As Boolean
			Get
				Return False
			End Get
		End Property











































		Public Overrides Function order() As ByteOrder




			Return ByteOrder.LITTLE_ENDIAN

		End Function

	End Class

End Namespace