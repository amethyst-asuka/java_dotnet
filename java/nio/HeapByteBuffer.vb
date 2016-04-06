Imports System
Imports java.lang

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
    ''' <summary>
    ''' A read/write HeapByteBuffer.
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' </summary>

    Friend Class HeapByteBuffer
        Inherits ByteBuffer

        ' For speed these fields are actually declared in X-Buffer;
        ' these declarations are here as documentation
        '    
        '
        '    protected final byte[] hb;
        '    protected final int offset;
        '
        '    

        Friend Sub New(  cap As Integer,   lim As Integer) ' package-private

            MyBase.New(-1, 0, lim, cap, New SByte(cap - 1) {}, 0)
            '        
            '        hb = new byte[cap];
            '        offset = 0;
            '        




        End Sub

        Friend Sub New(  buf As SByte(),   [off] As Integer,   len As Integer) ' package-private

            MyBase.New(-1, [off], [off] + len, buf.Length, buf, 0)
            '        
            '        hb = buf;
            '        offset = 0;
            '        




        End Sub

        Protected Friend Sub New(  buf As SByte(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)

            MyBase.New(mark, pos, lim, cap, buf, [off])
            '        
            '        hb = buf;
            '        offset = off;
            '        




        End Sub

        Public Overrides Function slice() As ByteBuffer
            Return New HeapByteBuffer(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
        End Function

        Public Overrides Function duplicate() As ByteBuffer
            Return New HeapByteBuffer(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
        End Function

        Public Overrides Function asReadOnlyBuffer() As ByteBuffer

            Return New HeapByteBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



        End Function



        Protected Friend Overridable Function ix(  i As Integer) As Integer
            Return i + offset
        End Function

        Public Overrides Function [get]() As SByte
            Return hb(ix(nextGetIndex()))
        End Function

        Public Overrides Function [get](  i As Integer) As SByte
            Return hb(ix(checkIndex(i)))
        End Function







        Public Overrides Function [get](  dst As SByte(),   offset As Integer,   length As Integer) As ByteBuffer
            checkBounds(offset, length, dst.Length)
            If length > remaining() Then Throw New BufferUnderflowException
            array.Copy(hb, ix(position()), dst, offset, length)
            position(position() + length)
            Return Me
        End Function

        Public  Overrides ReadOnly Property  direct As Boolean
            Get
                Return False
            End Get
        End Property



        Public  Overrides ReadOnly Property  [readOnly] As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function put(  x As SByte) As ByteBuffer

            hb(ix(nextPutIndex())) = x
            Return Me



        End Function

        Public Overrides Function put(  i As Integer,   x As SByte) As ByteBuffer

            hb(ix(checkIndex(i))) = x
            Return Me



        End Function

        Public Overrides Function put(  src As SByte(),   offset As Integer,   length As Integer) As ByteBuffer

            checkBounds(offset, length, src.Length)
            If length > remaining() Then Throw New BufferOverflowException
            array.Copy(src, offset, hb, ix(position()), length)
            position(position() + length)
            Return Me



        End Function

        Public Overrides Function put(  src As ByteBuffer) As ByteBuffer

            If TypeOf src Is HeapByteBuffer Then
                If src Is Me Then Throw New IllegalArgumentException
                Dim sb As HeapByteBuffer = CType(src, HeapByteBuffer)
                Dim n As Integer = sb.remaining()
                If n > remaining() Then Throw New BufferOverflowException
                array.Copy(sb.hb, sb.ix(sb.position()), hb, ix(position()), n)
                sb.position(sb.position() + n)
                position(position() + n)
            ElseIf src.direct Then
                Dim n As Integer = src.remaining()
                If n > remaining() Then Throw New BufferOverflowException
                src.get(hb, ix(position()), n)
                position(position() + n)
            Else
                MyBase.put(src)
            End If
            Return Me



        End Function

        Public Overrides Function compact() As ByteBuffer

            array.Copy(hb, ix(position()), hb, ix(0), remaining())
            position(remaining())
            limit(capacity())
            discardMark()
            Return Me



        End Function





        Friend Overrides Function _get(  i As Integer) As SByte ' package-private
            Return hb(i)
        End Function

        Friend Overrides Sub _put(  i As Integer,   b As SByte) ' package-private

            hb(i) = b



        End Sub

        ' char



        Public  Overrides ReadOnly Property  [char] As Char
            Get
                Return Bits.getChar(Me, ix(nextGetIndex(2)), bigEndian)
            End Get
        End Property

        Public Overrides Function getChar(  i As Integer) As Char
            Return Bits.getChar(Me, ix(checkIndex(i, 2)), bigEndian)
        End Function



        Public Overrides Function putChar(  x As Char) As ByteBuffer

            Bits.putChar(Me, ix(nextPutIndex(2)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putChar(  i As Integer,   x As Char) As ByteBuffer

            Bits.putChar(Me, ix(checkIndex(i, 2)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asCharBuffer() As CharBuffer
            Dim size As Integer = Me.remaining() >> 1
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsCharBufferB(Me, -1, 0, size, size, [off]), CharBuffer), CType(New ByteBufferAsCharBufferL(Me, -1, 0, size, size, [off]), CharBuffer)))
        End Function


        ' short



        Public  Overrides ReadOnly Property  [short] As Short
            Get
                Return Bits.getShort(Me, ix(nextGetIndex(2)), bigEndian)
            End Get
        End Property

        Public Overrides Function getShort(  i As Integer) As Short
            Return Bits.getShort(Me, ix(checkIndex(i, 2)), bigEndian)
        End Function



        Public Overrides Function putShort(  x As Short) As ByteBuffer

            Bits.putShort(Me, ix(nextPutIndex(2)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putShort(  i As Integer,   x As Short) As ByteBuffer

            Bits.putShort(Me, ix(checkIndex(i, 2)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asShortBuffer() As ShortBuffer
            Dim size As Integer = Me.remaining() >> 1
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsShortBufferB(Me, -1, 0, size, size, [off]), ShortBuffer), CType(New ByteBufferAsShortBufferL(Me, -1, 0, size, size, [off]), ShortBuffer)))
        End Function


        ' int



        Public  Overrides ReadOnly Property  int As Integer
            Get
                Return Bits.getInt(Me, ix(nextGetIndex(4)), bigEndian)
            End Get
        End Property

        Public Overrides Function getInt(  i As Integer) As Integer
            Return Bits.getInt(Me, ix(checkIndex(i, 4)), bigEndian)
        End Function



        Public Overrides Function putInt(  x As Integer) As ByteBuffer

            Bits.putInt(Me, ix(nextPutIndex(4)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putInt(  i As Integer,   x As Integer) As ByteBuffer

            Bits.putInt(Me, ix(checkIndex(i, 4)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asIntBuffer() As IntBuffer
            Dim size As Integer = Me.remaining() >> 2
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsIntBufferB(Me, -1, 0, size, size, [off]), IntBuffer), CType(New ByteBufferAsIntBufferL(Me, -1, 0, size, size, [off]), IntBuffer)))
        End Function


        ' long



        Public  Overrides ReadOnly Property  [long] As Long
            Get
                Return Bits.getLong(Me, ix(nextGetIndex(8)), bigEndian)
            End Get
        End Property

        Public Overrides Function getLong(  i As Integer) As Long
            Return Bits.getLong(Me, ix(checkIndex(i, 8)), bigEndian)
        End Function



        Public Overrides Function putLong(  x As Long) As ByteBuffer

            Bits.putLong(Me, ix(nextPutIndex(8)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putLong(  i As Integer,   x As Long) As ByteBuffer

            Bits.putLong(Me, ix(checkIndex(i, 8)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asLongBuffer() As LongBuffer
            Dim size As Integer = Me.remaining() >> 3
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsLongBufferB(Me, -1, 0, size, size, [off]), LongBuffer), CType(New ByteBufferAsLongBufferL(Me, -1, 0, size, size, [off]), LongBuffer)))
        End Function


        ' float



        Public  Overrides ReadOnly Property  float As Single
            Get
                Return Bits.getFloat(Me, ix(nextGetIndex(4)), bigEndian)
            End Get
        End Property

        Public Overrides Function getFloat(  i As Integer) As Single
            Return Bits.getFloat(Me, ix(checkIndex(i, 4)), bigEndian)
        End Function



        Public Overrides Function putFloat(  x As Single) As ByteBuffer

            Bits.putFloat(Me, ix(nextPutIndex(4)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putFloat(  i As Integer,   x As Single) As ByteBuffer

            Bits.putFloat(Me, ix(checkIndex(i, 4)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asFloatBuffer() As FloatBuffer
            Dim size As Integer = Me.remaining() >> 2
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsFloatBufferB(Me, -1, 0, size, size, [off]), FloatBuffer), CType(New ByteBufferAsFloatBufferL(Me, -1, 0, size, size, [off]), FloatBuffer)))
        End Function


        ' double



        Public  Overrides ReadOnly Property  [double] As Double
            Get
                Return Bits.getDouble(Me, ix(nextGetIndex(8)), bigEndian)
            End Get
        End Property

        Public Overrides Function getDouble(  i As Integer) As Double
            Return Bits.getDouble(Me, ix(checkIndex(i, 8)), bigEndian)
        End Function



        Public Overrides Function putDouble(  x As Double) As ByteBuffer

            Bits.putDouble(Me, ix(nextPutIndex(8)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function putDouble(  i As Integer,   x As Double) As ByteBuffer

            Bits.putDouble(Me, ix(checkIndex(i, 8)), x, bigEndian)
            Return Me



        End Function

        Public Overrides Function asDoubleBuffer() As DoubleBuffer
            Dim size As Integer = Me.remaining() >> 3
            Dim [off] As Integer = offset + position()
            Return (If(bigEndian, CType(New ByteBufferAsDoubleBufferB(Me, -1, 0, size, size, [off]), DoubleBuffer), CType(New ByteBufferAsDoubleBufferL(Me, -1, 0, size, size, [off]), DoubleBuffer)))
        End Function











































    End Class

End Namespace