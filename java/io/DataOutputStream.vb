Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

    ''' <summary>
    ''' A data output stream lets an application write primitive Java data
    ''' types to an output stream in a portable way. An application can
    ''' then use a data input stream to read the data back in.
    ''' 
    ''' @author  unascribed </summary>
    ''' <seealso cref=     java.io.DataInputStream
    ''' @since   JDK1.0 </seealso>
    Public Class DataOutputStream
        Inherits FilterOutputStream
        Implements DataOutput

        ''' <summary>
        ''' The number of bytes written to the data output stream so far.
        ''' If this counter overflows, it will be wrapped to  java.lang.[Integer].MAX_VALUE.
        ''' </summary>
        Protected Friend written As Integer

        ''' <summary>
        ''' bytearr is initialized on demand by writeUTF
        ''' </summary>
        Private bytearr As SByte() = Nothing

        ''' <summary>
        ''' Creates a new data output stream to write data to the specified
        ''' underlying output stream. The counter <code>written</code> is
        ''' set to zero.
        ''' </summary>
        ''' <param name="out">   the underlying output stream, to be saved for later
        '''                use. </param>
        ''' <seealso cref=     java.io.FilterOutputStream#out </seealso>
        Public Sub New(out As OutputStream)
            MyBase.New(out)
        End Sub

        ''' <summary>
        ''' Increases the written counter by the specified value
        ''' until it reaches  java.lang.[Integer].MAX_VALUE.
        ''' </summary>
        Private Sub incCount(value As Integer)
            Dim temp As Integer = written + value
            If temp < 0 Then temp = java.lang.[Integer].MAX_VALUE
            written = temp
        End Sub

        ''' <summary>
        ''' Writes the specified byte (the low eight bits of the argument
        ''' <code>b</code>) to the underlying output stream. If no exception
        ''' is thrown, the counter <code>written</code> is incremented by
        ''' <code>1</code>.
        ''' <p>
        ''' Implements the <code>write</code> method of <code>OutputStream</code>.
        ''' </summary>
        ''' <param name="b">   the <code>byte</code> to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub write(b As Integer) Implements DataOutput.write
            out.write(b)
            incCount(1)
        End Sub

        ''' <summary>
        ''' Writes <code>len</code> bytes from the specified byte array
        ''' starting at offset <code>off</code> to the underlying output stream.
        ''' If no exception is thrown, the counter <code>written</code> is
        ''' incremented by <code>len</code>.
        ''' </summary>
        ''' <param name="b">     the data. </param>
        ''' <param name="off">   the start offset in the data. </param>
        ''' <param name="len">   the number of bytes to write. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub write(b() As Byte, off As Integer, len As Integer) ' throws IOException
            out.write(b, off, len)
            incCount(len)
        End Sub

        ''' <summary>
        ''' Flushes this data output stream. This forces any buffered output
        ''' bytes to be written out to the stream.
        ''' <p>
        ''' The <code>flush</code> method of <code>DataOutputStream</code>
        ''' calls the <code>flush</code> method of its underlying output stream.
        ''' </summary>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        ''' <seealso cref=        java.io.OutputStream#flush() </seealso>
        Public Sub flush() ' throws IOException
            out.flush()
        End Sub

        ''' <summary>
        ''' Writes a <code>boolean</code> to the underlying output stream as
        ''' a 1-byte value. The value <code>true</code> is written out as the
        ''' value <code>(byte)1</code>; the value <code>false</code> is
        ''' written out as the value <code>(byte)0</code>. If no exception is
        ''' thrown, the counter <code>written</code> is incremented by
        ''' <code>1</code>.
        ''' </summary>
        ''' <param name="v">   a <code>boolean</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeBoolean(v As Boolean) ' throws IOException
            out.write(If(v, 1, 0))
            incCount(1)
        End Sub
        ''' <summary>
        ''' Writes out a <code>byte</code> to the underlying output stream as
        ''' a 1-byte value. If no exception is thrown, the counter
        ''' <code>written</code> is incremented by <code>1</code>.
        ''' </summary>
        ''' <param name="v">   a <code>byte</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeByte(v As Integer) 'throws IOException
            out.write(v)
            incCount(1)
        End Sub

        ''' <summary>
        ''' Writes a <code>short</code> to the underlying output stream as two
        ''' bytes, high byte first. If no exception is thrown, the counter
        ''' <code>written</code> is incremented by <code>2</code>.
        ''' </summary>
        ''' <param name="v">   a <code>short</code> to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeShort(v As Integer) ' throws IOException
            out.write((CInt(CUInt(v) >> 8)) And &HFF)
            out.write((CInt(CUInt(v) >> 0)) And &HFF)
            incCount(2)
        End Sub

        ''' <summary>
        ''' Writes a <code>char</code> to the underlying output stream as a
        ''' 2-byte value, high byte first. If no exception is thrown, the
        ''' counter <code>written</code> is incremented by <code>2</code>.
        ''' </summary>
        ''' <param name="v">   a <code>char</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeChar(v As Integer) ' throws IOException
            out.write((CInt(CUInt(v) >> 8)) And &HFF)
            out.write((CInt(CUInt(v) >> 0)) And &HFF)
            incCount(2)
        End Sub

        ''' <summary>
        ''' Writes an <code>int</code> to the underlying output stream as four
        ''' bytes, high byte first. If no exception is thrown, the counter
        ''' <code>written</code> is incremented by <code>4</code>.
        ''' </summary>
        ''' <param name="v">   an <code>int</code> to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeInt(v As Integer) ' throws IOException
            out.write((CInt(CUInt(v) >> 24)) And &HFF)
            out.write((CInt(CUInt(v) >> 16)) And &HFF)
            out.write((CInt(CUInt(v) >> 8)) And &HFF)
            out.write((CInt(CUInt(v) >> 0)) And &HFF)
            incCount(4)
        End Sub

        Private writeBuffer() As SByte = New SByte(7) {}

        ''' <summary>
        ''' Writes a <code>long</code> to the underlying output stream as eight
        ''' bytes, high byte first. In no exception is thrown, the counter
        ''' <code>written</code> is incremented by <code>8</code>.
        ''' </summary>
        ''' <param name="v">   a <code>long</code> to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeLong(v As Long) 'throws IOException
            writeBuffer(0) = CByte(CInt(CUInt(v) >> 56))
            writeBuffer(1) = CByte(CInt(CUInt(v) >> 48))
            writeBuffer(2) = CByte(CInt(CUInt(v) >> 40))
            writeBuffer(3) = CByte(CInt(CUInt(v) >> 32))
            writeBuffer(4) = CByte(CInt(CUInt(v) >> 24))
            writeBuffer(5) = CByte(CInt(CUInt(v) >> 16))
            writeBuffer(6) = CByte(CInt(CUInt(v) >> 8))
            writeBuffer(7) = CByte(CInt(CUInt(v) >> 0))
            out.write(writeBuffer, 0, 8)
            incCount(8)
        End Sub

        ''' <summary>
        ''' Converts the float argument to an <code>int</code> using the
        ''' <code>floatToIntBits</code> method in class <code>Float</code>,
        ''' and then writes that <code>int</code> value to the underlying
        ''' output stream as a 4-byte quantity, high byte first. If no
        ''' exception is thrown, the counter <code>written</code> is
        ''' incremented by <code>4</code>.
        ''' </summary>
        ''' <param name="v">   a <code>float</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        ''' <seealso cref=        java.lang.Float#floatToIntBits(float) </seealso>
        Public Sub writeFloat(v As Single) 'throws IOException
            writeInt(Float.floatToIntBits(v))
        End Sub

        ''' <summary>
        ''' Converts the double argument to a <code>long</code> using the
        ''' <code>doubleToLongBits</code> method in class <code>Double</code>,
        ''' and then writes that <code>long</code> value to the underlying
        ''' output stream as an 8-byte quantity, high byte first. If no
        ''' exception is thrown, the counter <code>written</code> is
        ''' incremented by <code>8</code>.
        ''' </summary>
        ''' <param name="v">   a <code>double</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        ''' <seealso cref=        java.lang.Double#doubleToLongBits(double) </seealso>
        Public Sub writeDouble(v As Double) ' throws IOException
            writeLong(Double.doubleToLongBits(v))
        End Sub

        ''' <summary>
        ''' Writes out the string to the underlying output stream as a
        ''' sequence of bytes. Each character in the string is written out, in
        ''' sequence, by discarding its high eight bits. If no exception is
        ''' thrown, the counter <code>written</code> is incremented by the
        ''' length of <code>s</code>.
        ''' </summary>
        ''' <param name="s">   a string of bytes to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeBytes(s As String) 'throws IOException
            Dim len As Integer = s.Length()
            For i As Integer = 0 To len - 1
                out.write(AscW(s.Chars(i)))
            Next i
            incCount(len)
        End Sub

        ''' <summary>
        ''' Writes a string to the underlying output stream as a sequence of
        ''' characters. Each character is written to the data output stream as
        ''' if by the <code>writeChar</code> method. If no exception is
        ''' thrown, the counter <code>written</code> is incremented by twice
        ''' the length of <code>s</code>.
        ''' </summary>
        ''' <param name="s">   a <code>String</code> value to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.DataOutputStream#writeChar(int) </seealso>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub writeChars(s As String) ' throws IOException
            Dim len As Integer = s.Length()
            For i As Integer = 0 To len - 1
                Dim v As Integer = AscW(s.Chars(i))
                out.write((CInt(CUInt(v) >> 8)) And &HFF)
                out.write((CInt(CUInt(v) >> 0)) And &HFF)
            Next i
            incCount(len * 2)
        End Sub

        ''' <summary>
        ''' Writes a string to the underlying output stream using
        ''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
        ''' encoding in a machine-independent manner.
        ''' <p>
        ''' First, two bytes are written to the output stream as if by the
        ''' <code>writeShort</code> method giving the number of bytes to
        ''' follow. This value is the number of bytes actually written out,
        ''' not the length of the string. Following the length, each character
        ''' of the string is output, in sequence, using the modified UTF-8 encoding
        ''' for the character. If no exception is thrown, the counter
        ''' <code>written</code> is incremented by the total number of
        ''' bytes written to the output stream. This will be at least two
        ''' plus the length of <code>str</code>, and at most two plus
        ''' thrice the length of <code>str</code>.
        ''' </summary>
        ''' <param name="str">   a string to be written. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        Public Sub writeUTF(str As String) ' throws IOException
            writeUTF(str, Me)
        End Sub

        ''' <summary>
        ''' Writes a string to the specified DataOutput using
        ''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
        ''' encoding in a machine-independent manner.
        ''' <p>
        ''' First, two bytes are written to out as if by the <code>writeShort</code>
        ''' method giving the number of bytes to follow. This value is the number of
        ''' bytes actually written out, not the length of the string. Following the
        ''' length, each character of the string is output, in sequence, using the
        ''' modified UTF-8 encoding for the character. If no exception is thrown, the
        ''' counter <code>written</code> is incremented by the total number of
        ''' bytes written to the output stream. This will be at least two
        ''' plus the length of <code>str</code>, and at most two plus
        ''' thrice the length of <code>str</code>.
        ''' </summary>
        ''' <param name="str">   a string to be written. </param>
        ''' <param name="out">   destination to write to </param>
        ''' <returns>     The number of bytes written out. </returns>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        Public Shared Function writeUTF(str As String, out As DataOutput) As Integer '  throws IOException
            Dim strlen As Integer = str.Length()
            Dim utflen As Integer = 0
            Dim c As Integer, count As Integer = 0

            ' use charAt instead of copying String to char array 
            For i As Integer = 0 To strlen - 1
                c = AscW(str.Chars(i))
                If (c >= &H1) AndAlso (c <= &H7F) Then
                    utflen += 1
                ElseIf c > &H7FF Then
                    utflen += 3
                Else
                    utflen += 2
                End If
            Next i

            If utflen > 65535 Then Throw New UTFDataFormatException("encoded string too long: " & utflen & " bytes")

            Dim bytearr As SByte() = Nothing
            If TypeOf out Is DataOutputStream Then
                Dim dos As DataOutputStream = CType(out, DataOutputStream)
                If dos.bytearr Is Nothing OrElse (dos.bytearr.Length < (utflen + 2)) Then dos.bytearr = New SByte((utflen * 2) + 2 - 1) {}
                bytearr = dos.bytearr
            Else
                bytearr = New SByte(utflen + 2 - 1) {}
            End If

            bytearr(count) = CByte((CInt(CUInt(utflen) >> 8)) And &HFF)
            count += 1
            bytearr(count) = CByte((CInt(CUInt(utflen) >> 0)) And &HFF)
            count += 1

            Dim i As Integer = 0
            For i = 0 To strlen - 1
                c = AscW(str.Chars(i))
                If Not ((c >= &H1) AndAlso (c <= &H7F)) Then Exit For
                bytearr(count) = CByte(c)
                count += 1
            Next i

            Do While i < strlen
                c = AscW(str.Chars(i))
                If (c >= &H1) AndAlso (c <= &H7F) Then
                    bytearr(count) = CByte(c)
                    count += 1

                ElseIf c > &H7FF Then
                    bytearr(count) = CByte(&HE0 Or ((c >> 12) And &HF))
                    count += 1
                    bytearr(count) = CByte(&H80 Or ((c >> 6) And &H3F))
                    count += 1
                    bytearr(count) = CByte(&H80 Or ((c >> 0) And &H3F))
                    count += 1
                Else
                    bytearr(count) = CByte(&HC0 Or ((c >> 6) And &H1F))
                    count += 1
                    bytearr(count) = CByte(&H80 Or ((c >> 0) And &H3F))
                    count += 1
                End If
                i += 1
            Loop
            out.write(bytearr, 0, utflen + 2)
            Return utflen + 2
        End Function

        ''' <summary>
        ''' Returns the current value of the counter <code>written</code>,
        ''' the number of bytes written to this data output stream so far.
        ''' If the counter overflows, it will be wrapped to  java.lang.[Integer].MAX_VALUE.
        ''' </summary>
        ''' <returns>  the value of the <code>written</code> field. </returns>
        ''' <seealso cref=     java.io.DataOutputStream#written </seealso>
        Public Function size() As Integer
            Return written
        End Function
    End Class
End Namespace