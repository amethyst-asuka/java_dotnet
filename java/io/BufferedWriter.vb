Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Writes text to a character-output stream, buffering characters so as to
	''' provide for the efficient writing of single characters, arrays, and strings.
	''' 
	''' <p> The buffer size may be specified, or the default size may be accepted.
	''' The default is large enough for most purposes.
	''' 
	''' <p> A newLine() method is provided, which uses the platform's own notion of
	''' line separator as defined by the system property <tt>line.separator</tt>.
	''' Not all platforms use the newline character ('\n') to terminate lines.
	''' Calling this method to terminate each output line is therefore preferred to
	''' writing a newline character directly.
	''' 
	''' <p> In general, a Writer sends its output immediately to the underlying
	''' character or byte stream.  Unless prompt output is required, it is advisable
	''' to wrap a BufferedWriter around any Writer whose write() operations may be
	''' costly, such as FileWriters and OutputStreamWriters.  For example,
	''' 
	''' <pre>
	''' PrintWriter out
	'''   = new PrintWriter(new BufferedWriter(new FileWriter("foo.out")));
	''' </pre>
	''' 
	''' will buffer the PrintWriter's output to the file.  Without buffering, each
	''' invocation of a print() method would cause characters to be converted into
	''' bytes that would then be written immediately to the file, which can be very
	''' inefficient.
	''' </summary>
	''' <seealso cref= PrintWriter </seealso>
	''' <seealso cref= FileWriter </seealso>
	''' <seealso cref= OutputStreamWriter </seealso>
	''' <seealso cref= java.nio.file.Files#newBufferedWriter
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public Class BufferedWriter
		Inherits Writer

		Private out As Writer

		Private cb As Char()
		Private nChars, nextChar As Integer

		Private Shared defaultCharBufferSize As Integer = 8192

		''' <summary>
		''' Line separator string.  This is the value of the line.separator
		''' property at the moment that the stream was created.
		''' </summary>
		Private lineSeparator As String

		''' <summary>
		''' Creates a buffered character-output stream that uses a default-sized
		''' output buffer.
		''' </summary>
		''' <param name="out">  A Writer </param>
		Public Sub New(  out As Writer)
			Me.New(out, defaultCharBufferSize)
		End Sub

		''' <summary>
		''' Creates a new buffered character-output stream that uses an output
		''' buffer of the given size.
		''' </summary>
		''' <param name="out">  A Writer </param>
		''' <param name="sz">   Output-buffer size, a positive integer
		''' </param>
		''' <exception cref="IllegalArgumentException">  If {@code sz <= 0} </exception>
		Public Sub New(  out As Writer,   sz As Integer)
			MyBase.New(out)
			If sz <= 0 Then Throw New IllegalArgumentException("Buffer size <= 0")
			Me.out = out
			cb = New Char(sz - 1){}
			nChars = sz
			nextChar = 0

			lineSeparator = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("line.separator"))
		End Sub

		''' <summary>
		''' Checks to make sure that the stream has not been closed </summary>
		Private Sub ensureOpen()
			If out Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Flushes the output buffer to the underlying character stream, without
		''' flushing the stream itself.  This method is non-private only so that it
		''' may be invoked by PrintStream.
		''' </summary>
		Friend Overridable Sub flushBuffer()
			SyncLock lock
				ensureOpen()
				If nextChar = 0 Then Return
				out.write(cb, 0, nextChar)
				nextChar = 0
			End SyncLock
		End Sub

		''' <summary>
		''' Writes a single character.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Sub write(  c As Integer)
			SyncLock lock
				ensureOpen()
				If nextChar >= nChars Then flushBuffer()
				cb(nextChar) = ChrW(c)
				nextChar += 1
			End SyncLock
		End Sub

		''' <summary>
		''' Our own little min method, to avoid loading java.lang.Math if we've run
		''' out of file descriptors and we're trying to print a stack trace.
		''' </summary>
		Private Function min(  a As Integer,   b As Integer) As Integer
			If a < b Then Return a
			Return b
		End Function

        ''' <summary>
        ''' Writes a portion of an array of characters.
        ''' 
        ''' <p> Ordinarily this method stores characters from the given array into
        ''' this stream's buffer, flushing the buffer to the underlying stream as
        ''' needed.  If the requested length is at least as large as the buffer,
        ''' however, then this method will flush the buffer and write the characters
        ''' directly to the underlying stream.  Thus redundant
        ''' <code>BufferedWriter</code>s will not copy data unnecessarily.
        ''' </summary>
        ''' <param name="cbuf">  A character array </param>
        ''' <param name="off">   Offset from which to start reading characters </param>
        ''' <param name="len">   Number of characters to write
        ''' </param>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Overloads Sub write(cbuf() As Char, off As Integer, len As Integer) ' throws IOException
            SyncLock lock
                ensureOpen()
                If (off < 0) OrElse (off > cbuf.Length) OrElse (len < 0) OrElse ((off + len) > cbuf.Length) OrElse ((off + len) < 0) Then
                    Throw New IndexOutOfBoundsException
                ElseIf len = 0 Then
                    Return
                End If

                If len >= nChars Then
                    '                 If the request length exceeds the size of the output buffer,
                    '                   flush the buffer and then write the data directly.  In this
                    '                   way buffered streams will cascade harmlessly. 
                    flushBuffer()
                    out.write(cbuf, off, len)
                    Return
                End If

                Dim b As Integer = off, t As Integer = off + len
                Do While b < t
                    Dim d As Integer = min(nChars - nextChar, t - b)
                    Array.Copy(cbuf, b, cb, nextChar, d)
                    b += d
                    nextChar += d
                    If nextChar >= nChars Then flushBuffer()
                Loop
            End SyncLock
        End Sub
        ''' <summary>
        ''' Writes a portion of a String.
        ''' 
        ''' <p> If the value of the <tt>len</tt> parameter is negative then no
        ''' characters are written.  This is contrary to the specification of this
        ''' method in the {@link java.io.Writer#write(java.lang.String,int,int)
        ''' superclass}, which requires that an <seealso cref="IndexOutOfBoundsException"/> be
        ''' thrown.
        ''' </summary>
        ''' <param name="s">     String to be written </param>
        ''' <param name="off">   Offset from which to start reading characters </param>
        ''' <param name="len">   Number of characters to be written
        ''' </param>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Overloads Sub write(s As String, off As Integer, len As Integer) 'throws IOException
            SyncLock lock
                ensureOpen()

                Dim b As Integer = off, t As Integer = off + len()

                Do While b < t
                    Dim d As Integer = min(nChars - nextChar, t - b)
                    s.getChars(b, b + d, cb, nextChar)
                    b += d
                    nextChar += d
                    If nextChar >= nChars Then flushBuffer()
                Loop
            End SyncLock
        End Sub
        ''' <summary>
        ''' Writes a line separator.  The line separator string is defined by the
        ''' system property <tt>line.separator</tt>, and is not necessarily a single
        ''' newline ('\n') character.
        ''' </summary>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Sub newLine() 'throws IOException
            write(lineSeparator)
        End Sub
        ''' <summary>
        ''' Flushes the stream.
        ''' </summary>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Sub flush() 'throws IOException
            SyncLock lock
                flushBuffer()
                out.flush()
            End SyncLock
        End Sub

        Public Sub close() 'throws IOException
            SyncLock lock
                If out Is Nothing Then Return
                Using w As Writer = out
                    Try
                        flushBuffer()
                    Finally
                        out = Nothing
                        cb = Nothing
                    End Try
                End Using
            End SyncLock
        End Sub
    End Class

End Namespace