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



	Friend Class DirectByteBufferR
		Inherits DirectByteBuffer
		Implements sun.nio.ch.DirectBuffer






































































		' Primary constructor
		'
		Friend Sub New(  cap As Integer) ' package-private
























			MyBase.New(cap)

		End Sub

























		' For memory-mapped buffers -- invoked by FileChannelImpl via reflection
		'
		Protected Friend Sub New(  cap As Integer,   addr As Long,   fd As java.io.FileDescriptor,   unmapper As Runnable)






			MyBase.New(cap, addr, fd, unmapper)

		End Sub



		' For duplicates and slices
		'
		Friend Sub New(  db As sun.nio.ch.DirectBuffer,   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer) ' package-private








			MyBase.New(db, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As ByteBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 0)
			assert([off] >= 0)
			Return New DirectByteBufferR(Me, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As ByteBuffer
			Return New DirectByteBufferR(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ByteBuffer








			Return duplicate()

		End Function


























































		Public Overrides Function put(  x As SByte) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As SByte) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As ByteBuffer) As ByteBuffer




































			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As SByte(),   offset As Integer,   length As Integer) As ByteBuffer




























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As ByteBuffer












			Throw New ReadOnlyBufferException

		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return True
			End Get
		End Property

		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property
































































		Friend Overrides Function _get(  i As Integer) As SByte ' package-private
			Return unsafe.getByte(address + i)
		End Function

		Friend Overrides Sub _put(  i As Integer,   b As SByte) ' package-private



			Throw New ReadOnlyBufferException

		End Sub






















		Private Function putChar(  a As Long,   x As Char) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putChar(  x As Char) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putChar(  i As Integer,   x As Char) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asCharBuffer() As CharBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 1
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 1) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsCharBufferRB(Me, -1, 0, size, size, [off]), CharBuffer), CType(New ByteBufferAsCharBufferRL(Me, -1, 0, size, size, [off]), CharBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectCharBufferRU(Me, -1, 0, size, size, [off]), CharBuffer), CType(New DirectCharBufferRS(Me, -1, 0, size, size, [off]), CharBuffer)))
			End If
		End Function






















		Private Function putShort(  a As Long,   x As Short) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putShort(  x As Short) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putShort(  i As Integer,   x As Short) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asShortBuffer() As ShortBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 1
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 1) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsShortBufferRB(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New ByteBufferAsShortBufferRL(Me, -1, 0, size, size, [off]), ShortBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectShortBufferRU(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New DirectShortBufferRS(Me, -1, 0, size, size, [off]), ShortBuffer)))
			End If
		End Function






















		Private Function putInt(  a As Long,   x As Integer) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putInt(  x As Integer) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putInt(  i As Integer,   x As Integer) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asIntBuffer() As IntBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 2
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 2) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsIntBufferRB(Me, -1, 0, size, size, [off]), IntBuffer), CType(New ByteBufferAsIntBufferRL(Me, -1, 0, size, size, [off]), IntBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectIntBufferRU(Me, -1, 0, size, size, [off]), IntBuffer), CType(New DirectIntBufferRS(Me, -1, 0, size, size, [off]), IntBuffer)))
			End If
		End Function






















		Private Function putLong(  a As Long,   x As Long) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putLong(  x As Long) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putLong(  i As Integer,   x As Long) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asLongBuffer() As LongBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 3
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 3) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsLongBufferRB(Me, -1, 0, size, size, [off]), LongBuffer), CType(New ByteBufferAsLongBufferRL(Me, -1, 0, size, size, [off]), LongBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectLongBufferRU(Me, -1, 0, size, size, [off]), LongBuffer), CType(New DirectLongBufferRS(Me, -1, 0, size, size, [off]), LongBuffer)))
			End If
		End Function






















		Private Function putFloat(  a As Long,   x As Single) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putFloat(  x As Single) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putFloat(  i As Integer,   x As Single) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asFloatBuffer() As FloatBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 2
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 2) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsFloatBufferRB(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New ByteBufferAsFloatBufferRL(Me, -1, 0, size, size, [off]), FloatBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectFloatBufferRU(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New DirectFloatBufferRS(Me, -1, 0, size, size, [off]), FloatBuffer)))
			End If
		End Function






















		Private Function putDouble(  a As Long,   x As Double) As ByteBuffer









			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putDouble(  x As Double) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putDouble(  i As Integer,   x As Double) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asDoubleBuffer() As DoubleBuffer
			Dim [off] As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert([off] <= lim)
			Dim [rem] As Integer = (If([off] <= lim, lim - [off], 0))

			Dim size As Integer = [rem] >> 3
			If (Not unaligned) AndAlso ((address + [off]) Mod (1 << 3) <> 0) Then
				Return (If(bigEndian, CType(New ByteBufferAsDoubleBufferRB(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New ByteBufferAsDoubleBufferRL(Me, -1, 0, size, size, [off]), DoubleBuffer)))
			Else
				Return (If(nativeByteOrder, CType(New DirectDoubleBufferRU(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New DirectDoubleBufferRS(Me, -1, 0, size, size, [off]), DoubleBuffer)))
			End If
		End Function

	End Class

End Namespace