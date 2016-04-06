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
    ''' A read/write HeapFloatBuffer.
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' </summary>

    Friend Class HeapFloatBuffer
        Inherits FloatBuffer

        ' For speed these fields are actually declared in X-Buffer;
        ' these declarations are here as documentation
        '    
        '
        '    protected final float[] hb;
        '    protected final int offset;
        '
        '    

        Friend Sub New(  cap As Integer,   lim As Integer) ' package-private

            MyBase.New(-1, 0, lim, cap, New Single(cap - 1) {}, 0)
            '        
            '        hb = new float[cap];
            '        offset = 0;
            '        




        End Sub

        Friend Sub New(  buf As Single(),   [off] As Integer,   len As Integer) ' package-private

            MyBase.New(-1, [off], [off] + len, buf.Length, buf, 0)
            '        
            '        hb = buf;
            '        offset = 0;
            '        




        End Sub

        Protected Friend Sub New(  buf As Single(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)

            MyBase.New(mark, pos, lim, cap, buf, [off])
            '        
            '        hb = buf;
            '        offset = off;
            '        




        End Sub

        Public Overrides Function slice() As FloatBuffer
            Return New HeapFloatBuffer(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
        End Function

        Public Overrides Function duplicate() As FloatBuffer
            Return New HeapFloatBuffer(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
        End Function

        Public Overrides Function asReadOnlyBuffer() As FloatBuffer

            Return New HeapFloatBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



        End Function



        Protected Friend Overridable Function ix(  i As Integer) As Integer
            Return i + offset
        End Function

        Public Overrides Function [get]() As Single
            Return hb(ix(nextGetIndex()))
        End Function

        Public Overrides Function [get](  i As Integer) As Single
            Return hb(ix(checkIndex(i)))
        End Function







        Public Overrides Function [get](  dst As Single(),   offset As Integer,   length As Integer) As FloatBuffer
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

        Public Overrides Function put(  x As Single) As FloatBuffer

            hb(ix(nextPutIndex())) = x
            Return Me



        End Function

        Public Overrides Function put(  i As Integer,   x As Single) As FloatBuffer

            hb(ix(checkIndex(i))) = x
            Return Me



        End Function

        Public Overrides Function put(  src As Single(),   offset As Integer,   length As Integer) As FloatBuffer

            checkBounds(offset, length, src.Length)
            If length > remaining() Then Throw New BufferOverflowException
            array.Copy(src, offset, hb, ix(position()), length)
            position(position() + length)
            Return Me



        End Function

        Public Overrides Function put(  src As FloatBuffer) As FloatBuffer

            If TypeOf src Is HeapFloatBuffer Then
                If src Is Me Then Throw New IllegalArgumentException
                Dim sb As HeapFloatBuffer = CType(src, HeapFloatBuffer)
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

        Public Overrides Function compact() As FloatBuffer

            array.Copy(hb, ix(position()), hb, ix(0), remaining())
            position(remaining())
            limit(capacity())
            discardMark()
            Return Me



        End Function






































































































































































































































































































































































        Public Overrides Function order() As ByteOrder
            Return ByteOrder.nativeOrder()
        End Function



    End Class

End Namespace