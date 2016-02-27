Imports System

'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This class implements a character buffer that can be used as a
	''' character-input stream.
	''' 
	''' @author      Herb Jellinek
	''' @since       JDK1.1
	''' </summary>
	Public Class CharArrayReader
		Inherits Reader

		''' <summary>
		''' The character buffer. </summary>
		Protected Friend buf As Char()

		''' <summary>
		''' The current buffer position. </summary>
		Protected Friend pos As Integer

		''' <summary>
		''' The position of mark in buffer. </summary>
		Protected Friend markedPos As Integer = 0

		''' <summary>
		'''  The index of the end of this buffer.  There is not valid
		'''  data at or beyond this index.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a CharArrayReader from the specified array of chars. </summary>
		''' <param name="buf">       Input buffer (not copied) </param>
		Public Sub New(ByVal buf As Char())
			Me.buf = buf
			Me.pos = 0
			Me.count = buf.Length
		End Sub

        ''' <summary>
        ''' Creates a CharArrayReader from the specified array of chars.
        ''' 
        ''' <p> The resulting reader will start reading at the given
        ''' <tt>offset</tt>.  The total number of <tt>char</tt> values that can be
        ''' read from this reader will be either <tt>length</tt> or
        ''' <tt>buf.length-offset</tt>, whichever is smaller.
        ''' </summary>
        ''' <exception cref="IllegalArgumentException">
        '''         If <tt>offset</tt> is negative or greater than
        '''         <tt>buf.length</tt>, or if <tt>length</tt> is negative, or if
        '''         the sum of these two values is negative.
        ''' </exception>
        ''' <param name="buf">       Input buffer (not copied) </param>
        ''' <param name="offset">    Offset of the first char to read </param>
        ''' <param name="length">    Number of chars to read </param>
        Sub New(buf() As Char, offset As Integer, length As Integer)
            If (offset < 0) OrElse (offset > buf.Length) OrElse (length < 0) OrElse ((offset + length) < 0) Then Throw New IllegalArgumentException
            Me.buf = buf
            Me.pos = offset
            Me.count = System.Math.Min(offset + length, buf.Length)
            Me.markedPos = offset
        End Sub
        ''' <summary>
        ''' Checks to make sure that the stream has not been closed </summary>
        Private Sub ensureOpen() 'throws IOException
            If buf Is Nothing Then Throw New IOException("Stream closed")
        End Sub
        ''' <summary>
        ''' Reads a single character.
        ''' </summary>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function read() As Integer ' throws IOException
            SyncLock lock
                ensureOpen()
                If pos >= count Then
                    Return -1
                Else
                    Dim tempVar As Integer = pos
                End If
                pos += 1
                Return buf(tempVar)
            End SyncLock
        End Function
        ''' <summary>
        ''' Reads characters into a portion of an array. </summary>
        ''' <param name="b">  Destination buffer </param>
        ''' <param name="off">  Offset at which to start storing characters </param>
        ''' <param name="len">   Maximum number of characters to read </param>
        ''' <returns>  The actual number of characters read, or -1 if
        '''          the end of the stream has been reached
        ''' </returns>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function read(b() As Char, off As Integer, len As Integer) As Integer ' throws IOException
            SyncLock lock
                ensureOpen()
                If (off < 0) OrElse (off > b.Length) OrElse (len < 0) OrElse ((off + len) > b.Length) OrElse ((off + len) < 0) Then
                    Throw New IndexOutOfBoundsException
                ElseIf len = 0 Then
                    Return 0
                End If

                If pos >= count Then Return -1
                If pos + len > count Then len = count - pos
                If len <= 0 Then Return 0
                Array.Copy(buf, pos, b, off, len)
                pos += len
                Return len
            End SyncLock
        End Function
        ''' <summary>
        ''' Skips characters.  Returns the number of characters that were skipped.
        ''' 
        ''' <p>The <code>n</code> parameter may be negative, even though the
        ''' <code>skip</code> method of the <seealso cref="Reader"/> superclass throws
        ''' an exception in this case. If <code>n</code> is negative, then
        ''' this method does nothing and returns <code>0</code>.
        ''' </summary>
        ''' <param name="n"> The number of characters to skip </param>
        ''' <returns>       The number of characters actually skipped </returns>
        ''' <exception cref="IOException"> If the stream is closed, or an I/O error occurs </exception>
        Public Function skip(n As Long n) As Long ' throws IOException
            SyncLock lock
                ensureOpen()
                If pos + n > count Then n = count - pos
                If n < 0 Then Return 0
                pos += n
                Return n
            End SyncLock
        End Function
        ''' <summary>
        ''' Tells whether this stream is ready to be read.  Character-array readers
        ''' are always ready to be read.
        ''' </summary>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Boolean ready() throws IOException
			SyncLock lock
				ensureOpen()
				Return (count - pos) > 0
			End SyncLock

		''' <summary>
		''' Tells whether this stream supports the mark() operation, which it does.
		''' </summary>
		public Boolean markSupported()
			Return True

		''' <summary>
		''' Marks the present position in the stream.  Subsequent calls to reset()
		''' will reposition the stream to this point.
		''' </summary>
		''' <param name="readAheadLimit">  Limit on the number of characters that may be
		'''                         read while still preserving the mark.  Because
		'''                         the stream's input comes from a character array,
		'''                         there is no actual limit; hence this argument is
		'''                         ignored.
		''' </param>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public void mark(Integer readAheadLimit) throws IOException
			SyncLock lock
				ensureOpen()
				markedPos = pos
			End SyncLock

		''' <summary>
		''' Resets the stream to the most recent mark, or to the beginning if it has
		''' never been marked.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public void reset() throws IOException
			SyncLock lock
				ensureOpen()
				pos = markedPos
			End SyncLock

		''' <summary>
		''' Closes the stream and releases any system resources associated with
		''' it.  Once the stream has been closed, further read(), ready(),
		''' mark(), reset(), or skip() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		public void close()
			buf = Nothing
	End Class

End Namespace