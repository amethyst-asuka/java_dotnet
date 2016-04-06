'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
    ''' Abstract class for writing to character streams.  The only methods that a
    ''' subclass must implement are write(char[], int, int), flush(), and close().
    ''' Most subclasses, however, will override some of the methods defined here in
    ''' order to provide higher efficiency, additional functionality, or both.
    ''' </summary>
    ''' <seealso cref= Writer </seealso>
    ''' <seealso cref=   BufferedWriter </seealso>
    ''' <seealso cref=   CharArrayWriter </seealso>
    ''' <seealso cref=   FilterWriter </seealso>
    ''' <seealso cref=   OutputStreamWriter </seealso>
    ''' <seealso cref=     FileWriter </seealso>
    ''' <seealso cref=   PipedWriter </seealso>
    ''' <seealso cref=   PrintWriter </seealso>
    ''' <seealso cref=   StringWriter </seealso>
    ''' <seealso cref= Reader
    ''' 
    ''' @author      Mark Reinhold
    ''' @since       JDK1.1 </seealso>

    Public MustInherit Class Writer : Inherits java.lang.Object
        Implements Appendable, Closeable, Flushable

        ''' <summary>
        ''' Temporary buffer used to hold writes of strings and single characters
        ''' </summary>
        Private writeBuffer As Char()

        ''' <summary>
        ''' Size of writeBuffer, must be >= 1
        ''' </summary>
        Private Const WRITE_BUFFER_SIZE As Integer = 1024

        ''' <summary>
        ''' The object used to synchronize operations on this stream.  For
        ''' efficiency, a character-stream object may use an object other than
        ''' itself to protect critical sections.  A subclass should therefore use
        ''' the object in this field rather than <tt>this</tt> or a synchronized
        ''' method.
        ''' </summary>
        Protected Friend lock As Object

        ''' <summary>
        ''' Creates a new character-stream writer whose critical sections will
        ''' synchronize on the writer itself.
        ''' </summary>
        Protected Friend Sub New()
            Me.lock = Me
        End Sub

        ''' <summary>
        ''' Creates a new character-stream writer whose critical sections will
        ''' synchronize on the given object.
        ''' </summary>
        ''' <param name="lock">
        '''         Object to synchronize on </param>
        Protected Friend Sub New(  lock As Object)
            If lock Is Nothing Then Throw New NullPointerException
            Me.lock = lock
        End Sub

        ''' <summary>
        ''' Writes a single character.  The character to be written is contained in
        ''' the 16 low-order bits of the given integer value; the 16 high-order bits
        ''' are ignored.
        ''' 
        ''' <p> Subclasses that intend to support efficient single-character output
        ''' should override this method.
        ''' </summary>
        ''' <param name="c">
        '''         int specifying a character to be written
        ''' </param>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public Overridable Sub write(  c As Integer)
            SyncLock lock
                If writeBuffer Is Nothing Then writeBuffer = New Char(WRITE_BUFFER_SIZE - 1) {}
                writeBuffer(0) = ChrW(c)
                write(writeBuffer, 0, 1)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Writes an array of characters.
        ''' </summary>
        ''' <param name="cbuf">
        '''         Array of characters to be written
        ''' </param>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public Overridable Sub write(  cbuf As Char())
            write(cbuf, 0, cbuf.Length)
        End Sub

        ''' <summary>
        ''' Writes a portion of an array of characters.
        ''' </summary>
        ''' <param name="cbuf">
        '''         Array of characters
        ''' </param>
        ''' <param name="off">
        '''         Offset from which to start writing characters
        ''' </param>
        ''' <param name="len">
        '''         Number of characters to write
        ''' </param>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public MustOverride Sub write(cbuf() As Char,   [off] As Integer,   len As Integer)

        ''' <summary>
        ''' Writes a string.
        ''' </summary>
        ''' <param name="str">
        '''         String to be written
        ''' </param>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public Overridable Sub write(  str As String)
            write(str, 0, str.length())
        End Sub

        ''' <summary>
        ''' Writes a portion of a string.
        ''' </summary>
        ''' <param name="str">
        '''         A String
        ''' </param>
        ''' <param name="off">
        '''         Offset from which to start writing characters
        ''' </param>
        ''' <param name="len">
        '''         Number of characters to write
        ''' </param>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>off</tt> is negative, or <tt>len</tt> is negative,
        '''          or <tt>off+len</tt> is negative or greater than the length
        '''          of the given string
        ''' </exception>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public Overridable Sub write(  str As String,   [off] As Integer,   len As Integer)
            SyncLock lock
                Dim cbuf As Char()
                If len <= WRITE_BUFFER_SIZE Then
                    If writeBuffer Is Nothing Then writeBuffer = New Char(WRITE_BUFFER_SIZE - 1) {}
                    cbuf = writeBuffer ' Don't permanently allocate very large buffers.
                Else
                    cbuf = New Char(len - 1) {}
                End If
                str.getChars([off], ([off] + len), cbuf, 0)
                write(cbuf, 0, len)
            End SyncLock
        End Sub

        ''' <summary>
        ''' Appends the specified character sequence to this writer.
        ''' 
        ''' <p> An invocation of this method of the form <tt>out.append(csq)</tt>
        ''' behaves in exactly the same way as the invocation
        ''' 
        ''' <pre>
        '''     out.write(csq.toString()) </pre>
        ''' 
        ''' <p> Depending on the specification of <tt>toString</tt> for the
        ''' character sequence <tt>csq</tt>, the entire sequence may not be
        ''' appended. For instance, invoking the <tt>toString</tt> method of a
        ''' character buffer will return a subsequence whose content depends upon
        ''' the buffer's position and limit.
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence to append.  If <tt>csq</tt> is
        '''         <tt>null</tt>, then the four characters <tt>"null"</tt> are
        '''         appended to this writer.
        ''' </param>
        ''' <returns>  This writer
        ''' </returns>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs
        ''' 
        ''' @since  1.5 </exception>
        Public Overridable Function append(  csq As CharSequence) As Writer
            If csq Is Nothing Then
                write("null")
            Else
                write(csq.ToString())
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Appends a subsequence of the specified character sequence to this writer.
        ''' <tt>Appendable</tt>.
        ''' 
        ''' <p> An invocation of this method of the form <tt>out.append(csq, start,
        ''' end)</tt> when <tt>csq</tt> is not <tt>null</tt> behaves in exactly the
        ''' same way as the invocation
        ''' 
        ''' <pre>
        '''     out.write(csq.subSequence(start, end).toString()) </pre>
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence from which a subsequence will be
        '''         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
        '''         will be appended as if <tt>csq</tt> contained the four
        '''         characters <tt>"null"</tt>.
        ''' </param>
        ''' <param name="start">
        '''         The index of the first character in the subsequence
        ''' </param>
        ''' <param name="end">
        '''         The index of the character following the last character in the
        '''         subsequence
        ''' </param>
        ''' <returns>  This writer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
        '''          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
        '''          <tt>csq.length()</tt>
        ''' </exception>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs
        ''' 
        ''' @since  1.5 </exception>
        Public Overridable Function append(  csq As CharSequence,   start As Integer,   [end] As Integer) As Writer
            Dim cs As CharSequence = (If(csq Is Nothing, "null", csq))
            write(cs.subSequence(start, [end]).ToString())
            Return Me
        End Function

        ''' <summary>
        ''' Appends the specified character to this writer.
        ''' 
        ''' <p> An invocation of this method of the form <tt>out.append(c)</tt>
        ''' behaves in exactly the same way as the invocation
        ''' 
        ''' <pre>
        '''     out.write(c) </pre>
        ''' </summary>
        ''' <param name="c">
        '''         The 16-bit character to append
        ''' </param>
        ''' <returns>  This writer
        ''' </returns>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs
        ''' 
        ''' @since 1.5 </exception>
        Public Overridable Function append(  c As Char) As Writer
            write(c)
            Return Me
        End Function

        ''' <summary>
        ''' Flushes the stream.  If the stream has saved any characters from the
        ''' various write() methods in a buffer, write them immediately to their
        ''' intended destination.  Then, if that destination is another character or
        ''' byte stream, flush it.  Thus one flush() invocation will flush all the
        ''' buffers in a chain of Writers and OutputStreams.
        ''' 
        ''' <p> If the intended destination of this stream is an abstraction provided
        ''' by the underlying operating system, for example a file, then flushing the
        ''' stream guarantees only that bytes previously written to the stream are
        ''' passed to the operating system for writing; it does not guarantee that
        ''' they are actually written to a physical device such as a disk drive.
        ''' </summary>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public MustOverride Sub flush() Implements Flushable.flush

        ''' <summary>
        ''' Closes the stream, flushing it first. Once the stream has been closed,
        ''' further write() or flush() invocations will cause an IOException to be
        ''' thrown. Closing a previously closed stream has no effect.
        ''' </summary>
        ''' <exception cref="IOException">
        '''          If an I/O error occurs </exception>
        Public MustOverride Sub close() Implements Closeable.close, AutoCloseable.close

        Private Function _append(csq As CharSequence) As Appendable Implements Appendable.append
            Return append(csq)
        End Function

        Private Function _append1(csq As CharSequence, start As Integer, [end] As Integer) As Appendable Implements Appendable.append
            Return append(csq, start, [end])
        End Function

        Private Function _append2(c As Char) As Appendable Implements Appendable.append
            Return append(c)
        End Function
    End Class

End Namespace