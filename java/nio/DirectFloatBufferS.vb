Imports Microsoft.VisualBasic

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



	Friend Class DirectFloatBufferS
		Inherits FloatBuffer
		Implements sun.nio.ch.DirectBuffer



		' Cached unsafe-access object
		Protected Friend Shared ReadOnly unsafe As sun.misc.Unsafe = Bits.unsafe()

		' Cached array base offset
		Private Shared ReadOnly arrayBaseOffset As Long = CLng(Fix(unsafe.arrayBaseOffset(GetType(Single()))))

		' Cached unaligned-access capability
		Protected Friend Shared ReadOnly unaligned As Boolean = Bits.unaligned()

		' Base address, used in all indexing calculations
		' NOTE: moved up to Buffer.java for speed in JNI GetDirectBufferAddress
		'    protected long address;

		' An object attached to this buffer. If this buffer is a view of another
		' buffer then we use this field to keep a reference to that buffer to
		' ensure that its memory isn't freed before we are done with it.
		Private ReadOnly att As Object

		Public Overridable Function attachment() As Object
			Return att
		End Function






































		Public Overridable Function cleaner() As sun.misc.Cleaner
			Return Nothing
		End Function
















































































		' For duplicates and slices
		'
		Friend Sub New(ByVal db As sun.nio.ch.DirectBuffer, ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer) ' package-private

			MyBase.New(mark, pos, lim, cap)
			address = db.address() + [off]



			att = db



		End Sub

		Public Overrides Function slice() As FloatBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 2)
			assert([off] >= 0)
			Return New DirectFloatBufferS(Me, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As FloatBuffer
			Return New DirectFloatBufferS(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
		End Function

		Public Overrides Function asReadOnlyBuffer() As FloatBuffer

			Return New DirectFloatBufferRS(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)



		End Function



		Public Overridable Function address() As Long
			Return address
		End Function

		Private Function ix(ByVal i As Integer) As Long
			Return address + (CLng(i) << 2)
		End Function

		Public Overrides Function [get]() As Single
			Return Float.intBitsToFloat(Bits.swap(unsafe.getInt(ix(nextGetIndex()))))
		End Function

		Public Overrides Function [get](ByVal i As Integer) As Single
			Return Float.intBitsToFloat(Bits.swap(unsafe.getInt(ix(checkIndex(i)))))
		End Function







		Public Overrides Function [get](ByVal dst As Single(), ByVal offset As Integer, ByVal length As Integer) As FloatBuffer

			If (CLng(length) << 2) > Bits.JNI_COPY_TO_ARRAY_THRESHOLD Then
				checkBounds(offset, length, dst.Length)
				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
				If length > [rem] Then Throw New BufferUnderflowException


				If order() IsNot ByteOrder.nativeOrder() Then
					Bits.copyToIntArray(ix(pos), dst, CLng(offset) << 2, CLng(length) << 2)
				Else

					Bits.copyToArray(ix(pos), dst, arrayBaseOffset, CLng(offset) << 2, CLng(length) << 2)
				End If
				position(pos + length)
			Else
				MyBase.get(dst, offset, length)
			End If
			Return Me



		End Function



		Public Overrides Function put(ByVal x As Single) As FloatBuffer

			unsafe.putInt(ix(nextPutIndex()), Bits.swap(Float.floatToRawIntBits(x)))
			Return Me



		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Single) As FloatBuffer

			unsafe.putInt(ix(checkIndex(i)), Bits.swap(Float.floatToRawIntBits(x)))
			Return Me



		End Function

		Public Overrides Function put(ByVal src As FloatBuffer) As FloatBuffer

			If TypeOf src Is DirectFloatBufferS Then
				If src Is Me Then Throw New IllegalArgumentException
				Dim sb As DirectFloatBufferS = CType(src, DirectFloatBufferS)

				Dim spos As Integer = sb.position()
				Dim slim As Integer = sb.limit()
				assert(spos <= slim)
				Dim srem As Integer = (If(spos <= slim, slim - spos, 0))

				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

				If srem > [rem] Then Throw New BufferOverflowException
				unsafe.copyMemory(sb.ix(spos), ix(pos), CLng(srem) << 2)
				sb.position(spos + srem)
				position(pos + srem)
			ElseIf src.hb IsNot Nothing Then

				Dim spos As Integer = src.position()
				Dim slim As Integer = src.limit()
				assert(spos <= slim)
				Dim srem As Integer = (If(spos <= slim, slim - spos, 0))

				put(src.hb, src.offset + spos, srem)
				src.position(spos + srem)

			Else
				MyBase.put(src)
			End If
			Return Me



		End Function

		Public Overrides Function put(ByVal src As Single(), ByVal offset As Integer, ByVal length As Integer) As FloatBuffer

			If (CLng(length) << 2) > Bits.JNI_COPY_FROM_ARRAY_THRESHOLD Then
				checkBounds(offset, length, src.Length)
				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
				If length > [rem] Then Throw New BufferOverflowException


				If order() IsNot ByteOrder.nativeOrder() Then
					Bits.copyFromIntArray(src, CLng(offset) << 2, ix(pos), CLng(length) << 2)
				Else

					Bits.copyFromArray(src, arrayBaseOffset, CLng(offset) << 2, ix(pos), CLng(length) << 2)
				End If
				position(pos + length)
			Else
				MyBase.put(src, offset, length)
			End If
			Return Me



		End Function

		Public Overrides Function compact() As FloatBuffer

			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

			unsafe.copyMemory(ix(pos), ix(0), CLng([rem]) << 2)
			position([rem])
			limit(capacity())
			discardMark()
			Return Me



		End Function

		Public Property Overrides direct As Boolean
			Get
				Return True
			End Get
		End Property

		Public Property Overrides [readOnly] As Boolean
			Get
				Return False
			End Get
		End Property















































		Public Overrides Function order() As ByteOrder

			Return (If(ByteOrder.nativeOrder() Is ByteOrder.BIG_ENDIAN, ByteOrder.LITTLE_ENDIAN, ByteOrder.BIG_ENDIAN))





		End Function


























	End Class

End Namespace