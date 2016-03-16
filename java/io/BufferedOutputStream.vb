Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' The class implements a buffered output stream. By setting up such
	''' an output stream, an application can write bytes to the underlying
	''' output stream without necessarily causing a call to the underlying
	''' system for each byte written.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>
	Public Class BufferedOutputStream
		Inherits FilterOutputStream

		''' <summary>
		''' The internal buffer where data is stored.
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' The number of valid bytes in the buffer. This value is always
		''' in the range <tt>0</tt> through <tt>buf.length</tt>; elements
		''' <tt>buf[0]</tt> through <tt>buf[count-1]</tt> contain valid
		''' byte data.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a new buffered output stream to write data to the
		''' specified underlying output stream.
		''' </summary>
		''' <param name="out">   the underlying output stream. </param>
		Public Sub New(ByVal out As OutputStream)
			Me.New(out, 8192)
		End Sub

		''' <summary>
		''' Creates a new buffered output stream to write data to the
		''' specified underlying output stream with the specified buffer
		''' size.
		''' </summary>
		''' <param name="out">    the underlying output stream. </param>
		''' <param name="size">   the buffer size. </param>
		''' <exception cref="IllegalArgumentException"> if size &lt;= 0. </exception>
		Public Sub New(ByVal out As OutputStream, ByVal size As Integer)
			MyBase.New(out)
			If size <= 0 Then Throw New IllegalArgumentException("Buffer size <= 0")
			buf = New SByte(size - 1){}
		End Sub

		''' <summary>
		''' Flush the internal buffer </summary>
		Private Sub flushBuffer()
			If count > 0 Then
				out.write(buf, 0, count)
				count = 0
			End If
		End Sub

		''' <summary>
		''' Writes the specified byte to this buffered output stream.
		''' </summary>
		''' <param name="b">   the byte to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub write(ByVal b As Integer)
			If count >= buf.Length Then flushBuffer()
			buf(count) = CByte(b)
			count += 1
		End Sub

        ''' <summary>
        ''' Writes <code>len</code> bytes from the specified byte array
        ''' starting at offset <code>off</code> to this buffered output stream.
        ''' 
        ''' <p> Ordinarily this method stores bytes from the given array into this
        ''' stream's buffer, flushing the buffer to the underlying output stream as
        ''' needed.  If the requested length is at least as large as this stream's
        ''' buffer, however, then this method will flush the buffer and write the
        ''' bytes directly to the underlying output stream.  Thus redundant
        ''' <code>BufferedOutputStream</code>s will not copy data unnecessarily.
        ''' </summary>
        ''' <param name="b">     the data. </param>
        ''' <param name="off">   the start offset in the data. </param>
        ''' <param name="len">   the number of bytes to write. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        Public Overloads Sub write(b() As Byte, off As Integer, len As Integer) 'throws IOException
            If len >= buf.Length Then
                '             If the request length exceeds the size of the output buffer,
                '               flush the output buffer and then write the data directly.
                '               In this way buffered streams will cascade harmlessly. 
                flushBuffer()
                out.write(b, off, len)
                Return
            End If
            If len > buf.Length - count Then flushBuffer()
            Array.Copy(b, off, buf, count, len)
            count += len
        End Sub

        ''' <summary>
        ''' Flushes this buffered output stream. This forces any buffered
        ''' output bytes to be written out to the underlying output stream.
        ''' </summary>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        ''' <seealso cref=        java.io.FilterOutputStream#out </seealso>
        Public Sub flush() ' throws IOException
            flushBuffer()
            out.flush()
        End Sub

    End Class

End Namespace