Imports java.lang
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



    Friend Class DirectCharBufferU
        Inherits CharBuffer
        Implements sun.nio.ch.DirectBuffer



        ' Cached unsafe-access object
        Protected Friend Shared ReadOnly unsafe As sun.misc.Unsafe = Bits.unsafe()

        ' Cached array base offset
        Private Shared ReadOnly arrayBaseOffset As Long = CLng(Fix(unsafe.arrayBaseOffset(GetType(Char()))))

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

        Public Overrides Function slice() As CharBuffer
            Dim pos As Integer = Me.position()
            Dim lim As Integer = Me.limit()
            Assert(pos <= lim)
            Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
            Dim [off] As Integer = (pos << 1)
            Assert([off] >= 0)
            Return New DirectCharBufferU(Me, -1, 0, [rem], [rem], [off])
        End Function

        Public Overrides Function duplicate() As CharBuffer
            Return New DirectCharBufferU(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
        End Function

        Public Overrides Function asReadOnlyBuffer() As CharBuffer

            Return New DirectCharBufferRU(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)



        End Function



        Public Overridable Function address() As Long
            Return address
        End Function

        Private Function ix(ByVal i As Integer) As Long
            Return address() + (CLng(i) << 1)
        End Function

        Public Overrides Function [get]() As Char
            Return ((unsafe.getChar(ix(nextGetIndex()))))
        End Function

        Public Overrides Function [get](ByVal i As Integer) As Char
            Return ((unsafe.getChar(ix(checkIndex(i)))))
        End Function


        Friend Overrides Function getUnchecked(ByVal i As Integer) As Char
            Return ((unsafe.getChar(ix(i))))
        End Function


        Public Overrides Function [get](ByVal dst As Char(), ByVal offset As Integer, ByVal length As Integer) As CharBuffer

            If (CLng(length) << 1) > Bits.JNI_COPY_TO_ARRAY_THRESHOLD Then
                checkBounds(offset, length, dst.Length)
                Dim pos As Integer = position()
                Dim lim As Integer = limit()
                Assert(pos <= lim)
                Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
                If length > [rem] Then Throw New BufferUnderflowException


                If order() IsNot ByteOrder.nativeOrder() Then
                    Bits.copyToCharArray(ix(pos), dst, CLng(offset) << 1, CLng(length) << 1)
                Else

                    Bits.copyToArray(ix(pos), dst, arrayBaseOffset, CLng(offset) << 1, CLng(length) << 1)
                End If
                position(pos + length)
            Else
                MyBase.get(dst, offset, length)
            End If
            Return Me



        End Function



        Public Overrides Function put(ByVal x As Char) As CharBuffer

            unsafe.putChar(ix(nextPutIndex()), ((x)))
            Return Me



        End Function

        Public Overrides Function put(ByVal i As Integer, ByVal x As Char) As CharBuffer

            unsafe.putChar(ix(checkIndex(i)), ((x)))
            Return Me



        End Function

        Public Overrides Function put(ByVal src As CharBuffer) As CharBuffer

            If TypeOf src Is DirectCharBufferU Then
                If src Is Me Then Throw New IllegalArgumentException
                Dim sb As DirectCharBufferU = CType(src, DirectCharBufferU)

                Dim spos As Integer = sb.position()
                Dim slim As Integer = sb.limit()
                Assert(spos <= slim)
                Dim srem As Integer = (If(spos <= slim, slim - spos, 0))

                Dim pos As Integer = position()
                Dim lim As Integer = limit()
                Assert(pos <= lim)
                Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

                If srem > [rem] Then Throw New BufferOverflowException
                unsafe.copyMemory(sb.ix(spos), ix(pos), CLng(srem) << 1)
                sb.position(spos + srem)
                position(pos + srem)
            ElseIf src.hb IsNot Nothing Then

                Dim spos As Integer = src.position()
                Dim slim As Integer = src.limit()
                Assert(spos <= slim)
                Dim srem As Integer = (If(spos <= slim, slim - spos, 0))

                put(src.hb, src.offset + spos, srem)
                src.position(spos + srem)

            Else
                MyBase.put(src)
            End If
            Return Me



        End Function

        Public Overrides Function put(ByVal src As Char(), ByVal offset As Integer, ByVal length As Integer) As CharBuffer

            If (CLng(length) << 1) > Bits.JNI_COPY_FROM_ARRAY_THRESHOLD Then
                checkBounds(offset, length, src.Length)
                Dim pos As Integer = position()
                Dim lim As Integer = limit()
                Assert(pos <= lim)
                Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
                If length > [rem] Then Throw New BufferOverflowException


                If order() IsNot ByteOrder.nativeOrder() Then
                    Bits.copyFromCharArray(src, CLng(offset) << 1, ix(pos), CLng(length) << 1)
                Else

                    Bits.copyFromArray(src, arrayBaseOffset, CLng(offset) << 1, ix(pos), CLng(length) << 1)
                End If
                position(pos + length)
            Else
                MyBase.put(src, offset, length)
            End If
            Return Me



        End Function

        Public Overrides Function compact() As CharBuffer

            Dim pos As Integer = position()
            Dim lim As Integer = limit()
            Assert(pos <= lim)
            Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))

            unsafe.copyMemory(ix(pos), ix(0), CLng([rem]) << 1)
            position([rem])
            limit(capacity())
            discardMark()
            Return Me



        End Function

        Public  Overrides ReadOnly Property  direct As Boolean
            Get
                Return True
            End Get
        End Property

        Public  Overrides ReadOnly Property  [readOnly] As Boolean
            Get
                Return False
            End Get
        End Property




        Public Overrides Function ToString(ByVal start As Integer, ByVal [end] As Integer) As String
            If ([end] > limit()) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
            Try
                Dim len As Integer = [end] - start
                Dim ca As Char() = New Char(len - 1) {}
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

        Public Overrides Function subSequence(ByVal start As Integer, ByVal [end] As Integer) As CharBuffer
            Dim pos As Integer = position()
            Dim lim As Integer = limit()
            Assert(pos <= lim)
            pos = (If(pos <= lim, pos, lim))
            Dim len As Integer = lim - pos

            If (start < 0) OrElse ([end] > len) OrElse (start > [end]) Then Throw New IndexOutOfBoundsException
            Return New DirectCharBufferU(Me, -1, pos + start, pos + [end], capacity(), OffSet)
        End Function







        Public Overrides Function order() As ByteOrder





            Return (If(ByteOrder.nativeOrder() IsNot ByteOrder.BIG_ENDIAN, ByteOrder.LITTLE_ENDIAN, ByteOrder.BIG_ENDIAN))

        End Function


























    End Class

End Namespace