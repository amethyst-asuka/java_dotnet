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


	''' 
	''' 
	''' 
	''' <summary>
	''' A read-only HeapByteBuffer.  This class extends the corresponding
	''' read/write [Class], overriding the mutation methods to throw a {@link
	''' ReadOnlyBufferException} and overriding the view-buffer methods to return an
	''' instance of this class rather than of the superclass.
	''' 
	''' </summary>

	Friend Class HeapByteBufferR
		Inherits HeapByteBuffer

		' For speed these fields are actually declared in X-Buffer;
		' these declarations are here as documentation
	'    
	'
	'
	'
	'
	'    

		Friend Sub New(  cap As Integer,   lim As Integer) ' package-private







			MyBase.New(cap, lim)
			Me.isReadOnly = True

		End Sub

		Friend Sub New(  buf As SByte(),   [off] As Integer,   len As Integer) ' package-private







			MyBase.New(buf, [off], len)
			Me.isReadOnly = True

		End Sub

		Protected Friend Sub New(  buf As SByte(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)







			MyBase.New(buf, mark, pos, lim, cap, [off])
			Me.isReadOnly = True

		End Sub

		Public Overrides Function slice() As ByteBuffer
			Return New HeapByteBufferR(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As ByteBuffer
			Return New HeapByteBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ByteBuffer








			Return duplicate()

		End Function




































		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function put(  x As SByte) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As SByte) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As SByte(),   offset As Integer,   length As Integer) As ByteBuffer








			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As ByteBuffer) As ByteBuffer























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As ByteBuffer







			Throw New ReadOnlyBufferException

		End Function





		Friend Overrides Function _get(  i As Integer) As SByte ' package-private
			Return hb(i)
		End Function

		Friend Overrides Sub _put(  i As Integer,   b As SByte) ' package-private



			Throw New ReadOnlyBufferException

		End Sub

		' char













		Public Overrides Function putChar(  x As Char) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putChar(  i As Integer,   x As Char) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asCharBuffer() As CharBuffer
			Dim size As Integer = Me.remaining() >> 1
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsCharBufferRB(Me, -1, 0, size, size, [off]), CharBuffer), CType(New ByteBufferAsCharBufferRL(Me, -1, 0, size, size, [off]), CharBuffer)))
		End Function


		' short













		Public Overrides Function putShort(  x As Short) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putShort(  i As Integer,   x As Short) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asShortBuffer() As ShortBuffer
			Dim size As Integer = Me.remaining() >> 1
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsShortBufferRB(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New ByteBufferAsShortBufferRL(Me, -1, 0, size, size, [off]), ShortBuffer)))
		End Function


		' int













		Public Overrides Function putInt(  x As Integer) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putInt(  i As Integer,   x As Integer) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asIntBuffer() As IntBuffer
			Dim size As Integer = Me.remaining() >> 2
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsIntBufferRB(Me, -1, 0, size, size, [off]), IntBuffer), CType(New ByteBufferAsIntBufferRL(Me, -1, 0, size, size, [off]), IntBuffer)))
		End Function


		' long













		Public Overrides Function putLong(  x As Long) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putLong(  i As Integer,   x As Long) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asLongBuffer() As LongBuffer
			Dim size As Integer = Me.remaining() >> 3
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsLongBufferRB(Me, -1, 0, size, size, [off]), LongBuffer), CType(New ByteBufferAsLongBufferRL(Me, -1, 0, size, size, [off]), LongBuffer)))
		End Function


		' float













		Public Overrides Function putFloat(  x As Single) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putFloat(  i As Integer,   x As Single) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asFloatBuffer() As FloatBuffer
			Dim size As Integer = Me.remaining() >> 2
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsFloatBufferRB(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New ByteBufferAsFloatBufferRL(Me, -1, 0, size, size, [off]), FloatBuffer)))
		End Function


		' double













		Public Overrides Function putDouble(  x As Double) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function putDouble(  i As Integer,   x As Double) As ByteBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function asDoubleBuffer() As DoubleBuffer
			Dim size As Integer = Me.remaining() >> 3
			Dim [off] As Integer = offset + position()
			Return (If(bigEndian, CType(New ByteBufferAsDoubleBufferRB(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New ByteBufferAsDoubleBufferRL(Me, -1, 0, size, size, [off]), DoubleBuffer)))
		End Function











































	End Class

End Namespace