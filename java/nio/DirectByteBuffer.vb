Imports Microsoft.VisualBasic
Imports System

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



	Friend Class DirectByteBuffer
		Inherits MappedByteBuffer
		Implements sun.nio.ch.DirectBuffer



		' Cached unsafe-access object
		Protected Friend Shared ReadOnly unsafe As sun.misc.Unsafe = Bits.unsafe()

		' Cached array base offset
		Private Shared ReadOnly arrayBaseOffset As Long = CLng(Fix(unsafe.arrayBaseOffset(GetType(SByte()))))

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



		Private Class Deallocator
			Implements Runnable

			Private Shared unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

			Private address As Long
			Private size As Long
			Private capacity As Integer

			Private Sub New(ByVal address As Long, ByVal size As Long, ByVal capacity As Integer)
				assert(address <> 0)
				Me.address = address
				Me.size = size
				Me.capacity = capacity
			End Sub

			Public Overridable Sub run() Implements Runnable.run
				If address = 0 Then Return
				unsafe.freeMemory(address)
				address = 0
				Bits.unreserveMemory(size, capacity)
			End Sub

		End Class

		Private ReadOnly cleaner_Renamed As sun.misc.Cleaner

		Public Overridable Function cleaner() As sun.misc.Cleaner
			Return cleaner_Renamed
		End Function











		' Primary constructor
		'
		Friend Sub New(ByVal cap As Integer) ' package-private

			MyBase.New(-1, 0, cap, cap)
			Dim pa As Boolean = sun.misc.VM.directMemoryPageAligned
			Dim ps As Integer = Bits.pageSize()
			Dim size As Long = System.Math.Max(1L, CLng(cap) + (If(pa, ps, 0)))
			Bits.reserveMemory(size, cap)

			Dim base As Long = 0
			Try
				base = unsafe.allocateMemory(size)
			Catch x As OutOfMemoryError
				Bits.unreserveMemory(size, cap)
				Throw x
			End Try
			unsafe.memoryory(base, size, CByte(0))
			If pa AndAlso (base Mod ps <> 0) Then
				' Round up to page boundary
				address = base + ps - (base And (ps - 1))
			Else
				address = base
			End If
			cleaner_Renamed = sun.misc.Cleaner.create(Me, New Deallocator(base, size, cap))
			att = Nothing



		End Sub



		' Invoked to construct a direct ByteBuffer referring to the block of
		' memory. A given arbitrary object may also be attached to the buffer.
		'
		Friend Sub New(ByVal addr As Long, ByVal cap As Integer, ByVal ob As Object)
			MyBase.New(-1, 0, cap, cap)
			address = addr
			cleaner_Renamed = Nothing
			att = ob
		End Sub


		' Invoked only by JNI: NewDirectByteBuffer(void*, long)
		'
		Private Sub New(ByVal addr As Long, ByVal cap As Integer)
			MyBase.New(-1, 0, cap, cap)
			address = addr
			cleaner_Renamed = Nothing
			att = Nothing
		End Sub



		' For memory-mapped buffers -- invoked by FileChannelImpl via reflection
		'
		Protected Friend Sub New(ByVal cap As Integer, ByVal addr As Long, ByVal fd As java.io.FileDescriptor, ByVal unmapper As Runnable)

			MyBase.New(-1, 0, cap, cap, fd)
			address = addr
			cleaner_Renamed = sun.misc.Cleaner.create(Me, unmapper)
			att = Nothing



		End Sub



		' For duplicates and slices
		'
		Friend Sub New(ByVal db As sun.nio.ch.DirectBuffer, ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer) ' package-private

			MyBase.New(mark, pos, lim, cap)
			address = db.address() + [off]

			cleaner_Renamed = Nothing

			att = db



		End Sub

		Public Overrides Function slice() As ByteBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 0)
			assert([off] >= 0)
			Return New DirectByteBuffer(Me, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As ByteBuffer
			Return New DirectByteBuffer(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ByteBuffer

			Return New DirectByteBufferR(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)



		End Function



		Public Overridable Function address() As Long
			Return address
		End Function

		Private Function ix(ByVal i As Integer) As Long
			Return address + (CLng(i) << 0)
		End Function

		Public Overrides Function [get]() As SByte
			Return ((unsafe.getByte(ix(nextGetIndex()))))
		End Function

		Public Overrides Function [get](ByVal i As Integer) As SByte
			Return ((unsafe.getByte(ix(checkIndex(i)))))
		End Function







		Public Overrides Function [get](ByVal dst As SByte(), ByVal offset As Integer, ByVal length As Integer) As ByteBuffer

			If (CLng(length) << 0) > Bits.JNI_COPY_TO_ARRAY_THRESHOLD Then
				checkBounds(offset, length, dst.Length)
				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
				If length > [rem] Then Throw New BufferUnderflowException








					Bits.copyToArray(ix(pos), dst, arrayBaseOffset, CLng(offset) << 0, CLng(length) << 0)
				position(pos + length)
			Else
				MyBase.get(dst, offset, length)
			End If
			Return Me



		End Function



		Public Overrides Function put(ByVal x As SByte) As ByteBuffer

			unsafe.putByte(ix(nextPutIndex()), ((x)))
			Return Me



		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As SByte) As ByteBuffer

			unsafe.putByte(ix(checkIndex(i)), ((x)))
			Return Me



		End Function

		Public Overrides Function put(ByVal src As ByteBuffer) As ByteBuffer

			If TypeOf src Is DirectByteBuffer Then
				If src Is Me Then Throw New IllegalArgumentException
				Dim sb As DirectByteBuffer = CType(src, DirectByteBuffer)

				Dim spos As Integer = sb.position()
				Dim slim As Integer = sb.limit()
				assert(spos <= slim)
				Dim srem As Integer = (If(spos <= slim, slim - spos, 0))

				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

				If srem > [rem] Then Throw New BufferOverflowException
				unsafe.copyMemory(sb.ix(spos), ix(pos), CLng(srem) << 0)
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

		Public Overrides Function put(ByVal src As SByte(), ByVal offset As Integer, ByVal length As Integer) As ByteBuffer

			If (CLng(length) << 0) > Bits.JNI_COPY_FROM_ARRAY_THRESHOLD Then
				checkBounds(offset, length, src.Length)
				Dim pos As Integer = position()
				Dim lim As Integer = limit()
				assert(pos <= lim)
				Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
				If length > [rem] Then Throw New BufferOverflowException









					Bits.copyFromArray(src, arrayBaseOffset, CLng(offset) << 0, ix(pos), CLng(length) << 0)
				position(pos + length)
			Else
				MyBase.put(src, offset, length)
			End If
			Return Me



		End Function

		Public Overrides Function compact() As ByteBuffer

			Dim pos As Integer = position()
			Dim lim As Integer = limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

			unsafe.copyMemory(ix(pos), ix(0), CLng([rem]) << 0)
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
































































		Friend Overrides Function _get(ByVal i As Integer) As SByte ' package-private
			Return unsafe.getByte(address + i)
		End Function

		Friend Overrides Sub _put(ByVal i As Integer, ByVal b As SByte) ' package-private

			unsafe.putByte(address + i, b)



		End Sub




		Private Function getChar(ByVal a As Long) As Char
			If unaligned Then
				Dim x As Char = unsafe.getChar(a)
				Return (If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getChar(a, bigEndian)
		End Function

		Public Property Overrides [char] As Char
			Get
				Return getChar(ix(nextGetIndex((1 << 1))))
			End Get
		End Property

		Public Overrides Function getChar(ByVal i As Integer) As Char
			Return getChar(ix(checkIndex(i, (1 << 1))))
		End Function



		Private Function putChar(ByVal a As Long, ByVal x As Char) As ByteBuffer

			If unaligned Then
				Dim y As Char = (x)
				unsafe.putChar(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putChar(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putChar(ByVal x As Char) As ByteBuffer

			putChar(ix(nextPutIndex((1 << 1))), x)
			Return Me



		End Function

		Public Overrides Function putChar(ByVal i As Integer, ByVal x As Char) As ByteBuffer

			putChar(ix(checkIndex(i, (1 << 1))), x)
			Return Me



		End Function

		Public Overrides Function asCharBuffer() As CharBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 1
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 1) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsCharBufferB(Me, -1, 0, size, size, [off]), CharBuffer), CType(New ByteBufferAsCharBufferL(Me, -1, 0, size, size, [off]), CharBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectCharBufferU(Me, -1, 0, size, size, [off]), CharBuffer), CType(New DirectCharBufferS(Me, -1, 0, size, size, [off]), CharBuffer)))
			End If
		End Function




		Private Function getShort(ByVal a As Long) As Short
			If unaligned Then
				Dim x As Short = unsafe.getShort(a)
				Return (If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getShort(a, bigEndian)
		End Function

		Public Property Overrides [short] As Short
			Get
				Return getShort(ix(nextGetIndex((1 << 1))))
			End Get
		End Property

		Public Overrides Function getShort(ByVal i As Integer) As Short
			Return getShort(ix(checkIndex(i, (1 << 1))))
		End Function



		Private Function putShort(ByVal a As Long, ByVal x As Short) As ByteBuffer

			If unaligned Then
				Dim y As Short = (x)
				unsafe.putShort(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putShort(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putShort(ByVal x As Short) As ByteBuffer

			putShort(ix(nextPutIndex((1 << 1))), x)
			Return Me



		End Function

		Public Overrides Function putShort(ByVal i As Integer, ByVal x As Short) As ByteBuffer

			putShort(ix(checkIndex(i, (1 << 1))), x)
			Return Me



		End Function

		Public Overrides Function asShortBuffer() As ShortBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 1
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 1) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsShortBufferB(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New ByteBufferAsShortBufferL(Me, -1, 0, size, size, [off]), ShortBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectShortBufferU(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New DirectShortBufferS(Me, -1, 0, size, size, [off]), ShortBuffer)))
			End If
		End Function




		Private Function getInt(ByVal a As Long) As Integer
			If unaligned Then
				Dim x As Integer = unsafe.getInt(a)
				Return (If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getInt(a, bigEndian)
		End Function

		Public Property Overrides int As Integer
			Get
				Return getInt(ix(nextGetIndex((1 << 2))))
			End Get
		End Property

		Public Overrides Function getInt(ByVal i As Integer) As Integer
			Return getInt(ix(checkIndex(i, (1 << 2))))
		End Function



		Private Function putInt(ByVal a As Long, ByVal x As Integer) As ByteBuffer

			If unaligned Then
				Dim y As Integer = (x)
				unsafe.putInt(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putInt(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putInt(ByVal x As Integer) As ByteBuffer

			putInt(ix(nextPutIndex((1 << 2))), x)
			Return Me



		End Function

		Public Overrides Function putInt(ByVal i As Integer, ByVal x As Integer) As ByteBuffer

			putInt(ix(checkIndex(i, (1 << 2))), x)
			Return Me



		End Function

		Public Overrides Function asIntBuffer() As IntBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 2
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 2) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsIntBufferB(Me, -1, 0, size, size, [off]), IntBuffer), CType(New ByteBufferAsIntBufferL(Me, -1, 0, size, size, [off]), IntBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectIntBufferU(Me, -1, 0, size, size, [off]), IntBuffer), CType(New DirectIntBufferS(Me, -1, 0, size, size, [off]), IntBuffer)))
			End If
		End Function




		Private Function getLong(ByVal a As Long) As Long
			If unaligned Then
				Dim x As Long = unsafe.getLong(a)
				Return (If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getLong(a, bigEndian)
		End Function

		Public Property Overrides [long] As Long
			Get
				Return getLong(ix(nextGetIndex((1 << 3))))
			End Get
		End Property

		Public Overrides Function getLong(ByVal i As Integer) As Long
			Return getLong(ix(checkIndex(i, (1 << 3))))
		End Function



		Private Function putLong(ByVal a As Long, ByVal x As Long) As ByteBuffer

			If unaligned Then
				Dim y As Long = (x)
				unsafe.putLong(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putLong(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putLong(ByVal x As Long) As ByteBuffer

			putLong(ix(nextPutIndex((1 << 3))), x)
			Return Me



		End Function

		Public Overrides Function putLong(ByVal i As Integer, ByVal x As Long) As ByteBuffer

			putLong(ix(checkIndex(i, (1 << 3))), x)
			Return Me



		End Function

		Public Overrides Function asLongBuffer() As LongBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 3
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 3) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsLongBufferB(Me, -1, 0, size, size, [off]), LongBuffer), CType(New ByteBufferAsLongBufferL(Me, -1, 0, size, size, [off]), LongBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectLongBufferU(Me, -1, 0, size, size, [off]), LongBuffer), CType(New DirectLongBufferS(Me, -1, 0, size, size, [off]), LongBuffer)))
			End If
		End Function




		Private Function getFloat(ByVal a As Long) As Single
			If unaligned Then
				Dim x As Integer = unsafe.getInt(a)
				Return Float.intBitsToFloat(If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getFloat(a, bigEndian)
		End Function

		Public Property Overrides float As Single
			Get
				Return getFloat(ix(nextGetIndex((1 << 2))))
			End Get
		End Property

		Public Overrides Function getFloat(ByVal i As Integer) As Single
			Return getFloat(ix(checkIndex(i, (1 << 2))))
		End Function



		Private Function putFloat(ByVal a As Long, ByVal x As Single) As ByteBuffer

			If unaligned Then
				Dim y As Integer = Float.floatToRawIntBits(x)
				unsafe.putInt(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putFloat(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putFloat(ByVal x As Single) As ByteBuffer

			putFloat(ix(nextPutIndex((1 << 2))), x)
			Return Me



		End Function

		Public Overrides Function putFloat(ByVal i As Integer, ByVal x As Single) As ByteBuffer

			putFloat(ix(checkIndex(i, (1 << 2))), x)
			Return Me



		End Function

		Public Overrides Function asFloatBuffer() As FloatBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 2
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 2) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsFloatBufferB(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New ByteBufferAsFloatBufferL(Me, -1, 0, size, size, [off]), FloatBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectFloatBufferU(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New DirectFloatBufferS(Me, -1, 0, size, size, [off]), FloatBuffer)))
			End If
		End Function




		Private Function getDouble(ByVal a As Long) As Double
			If unaligned Then
				Dim x As Long = unsafe.getLong(a)
				Return java.lang.[Double].longBitsToDouble(If(nativeByteOrder, x, Bits.swap(x)))
			End If
			Return Bits.getDouble(a, bigEndian)
		End Function

		Public Property Overrides [double] As Double
			Get
				Return getDouble(ix(nextGetIndex((1 << 3))))
			End Get
		End Property

		Public Overrides Function getDouble(ByVal i As Integer) As Double
			Return getDouble(ix(checkIndex(i, (1 << 3))))
		End Function



		Private Function putDouble(ByVal a As Long, ByVal x As Double) As ByteBuffer

			If unaligned Then
				Dim y As Long = java.lang.[Double].doubleToRawLongBits(x)
				unsafe.putLong(a, (If(nativeByteOrder, y, Bits.swap(y))))
			Else
				Bits.putDouble(a, x, bigEndian)
			End If
			Return Me



		End Function

		Public Overrides Function putDouble(ByVal x As Double) As ByteBuffer

			putDouble(ix(nextPutIndex((1 << 3))), x)
			Return Me



		End Function

		Public Overrides Function putDouble(ByVal i As Integer, ByVal x As Double) As ByteBuffer

			putDouble(ix(checkIndex(i, (1 << 3))), x)
			Return Me



		End Function

		Public Overrides Function asDoubleBuffer() As DoubleBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 3
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 3) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsDoubleBufferB(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New ByteBufferAsDoubleBufferL(Me, -1, 0, size, size, [off]), DoubleBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectDoubleBufferU(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New DirectDoubleBufferS(Me, -1, 0, size, size, [off]), DoubleBuffer)))
			End If
		End Function

	End Class

End Namespace