Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

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
	''' Reads text from a character-input stream, buffering characters so as to
	''' provide for the efficient reading of characters, arrays, and lines.
	''' 
	''' <p> The buffer size may be specified, or the default size may be used.  The
	''' default is large enough for most purposes.
	''' 
	''' <p> In general, each read request made of a Reader causes a corresponding
	''' read request to be made of the underlying character or byte stream.  It is
	''' therefore advisable to wrap a BufferedReader around any Reader whose read()
	''' operations may be costly, such as FileReaders and InputStreamReaders.  For
	''' example,
	''' 
	''' <pre>
	''' BufferedReader in
	'''   = new BufferedReader(new FileReader("foo.in"));
	''' </pre>
	''' 
	''' will buffer the input from the specified file.  Without buffering, each
	''' invocation of read() or readLine() could cause bytes to be read from the
	''' file, converted into characters, and then returned, which can be very
	''' inefficient.
	''' 
	''' <p> Programs that use DataInputStreams for textual input can be localized by
	''' replacing each DataInputStream with an appropriate BufferedReader.
	''' </summary>
	''' <seealso cref= FileReader </seealso>
	''' <seealso cref= InputStreamReader </seealso>
	''' <seealso cref= java.nio.file.Files#newBufferedReader
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public Class BufferedReader
		Inherits Reader

		Private [in] As Reader

		Private cb As Char()
		Private nChars, nextChar As Integer

		Private Const INVALIDATED As Integer = -2
		Private Const UNMARKED As Integer = -1
		Private markedChar As Integer = UNMARKED
		Private readAheadLimit As Integer = 0 ' Valid only when markedChar > 0

		''' <summary>
		''' If the next character is a line feed, skip it </summary>
		Private skipLF As Boolean = False

		''' <summary>
		''' The skipLF flag when the mark was set </summary>
		Private markedSkipLF As Boolean = False

		Private Shared defaultCharBufferSize As Integer = 8192
		Private Shared defaultExpectedLineLength As Integer = 80

		''' <summary>
		''' Creates a buffering character-input stream that uses an input buffer of
		''' the specified size.
		''' </summary>
		''' <param name="in">   A Reader </param>
		''' <param name="sz">   Input-buffer size
		''' </param>
		''' <exception cref="IllegalArgumentException">  If {@code sz <= 0} </exception>
		Public Sub New(ByVal [in] As Reader, ByVal sz As Integer)
			MyBase.New([in])
			If sz <= 0 Then Throw New IllegalArgumentException("Buffer size <= 0")
			Me.in = [in]
			cb = New Char(sz - 1){}
				nChars = 0
				nextChar = nChars
		End Sub

		''' <summary>
		''' Creates a buffering character-input stream that uses a default-sized
		''' input buffer.
		''' </summary>
		''' <param name="in">   A Reader </param>
		Public Sub New(ByVal [in] As Reader)
			Me.New([in], defaultCharBufferSize)
		End Sub

		''' <summary>
		''' Checks to make sure that the stream has not been closed </summary>
		Private Sub ensureOpen()
			If [in] Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Fills the input buffer, taking the mark into account if it is valid.
		''' </summary>
		Private Sub fill()
			Dim dst As Integer
			If markedChar <= UNMARKED Then
				' No mark 
				dst = 0
			Else
				' Marked 
				Dim delta As Integer = nextChar - markedChar
				If delta >= readAheadLimit Then
					' Gone past read-ahead limit: Invalidate mark 
					markedChar = INVALIDATED
					readAheadLimit = 0
					dst = 0
				Else
					If readAheadLimit <= cb.Length Then
						' Shuffle in the current buffer 
						Array.Copy(cb, markedChar, cb, 0, delta)
						markedChar = 0
						dst = delta
					Else
						' Reallocate buffer to accommodate read-ahead limit 
						Dim ncb As Char() = New Char(readAheadLimit - 1){}
						Array.Copy(cb, markedChar, ncb, 0, delta)
						cb = ncb
						markedChar = 0
						dst = delta
					End If
						nChars = delta
						nextChar = nChars
				End If
			End If

			Dim n As Integer
			Do
				n = [in].read(cb, dst, cb.Length - dst)
			Loop While n = 0
			If n > 0 Then
				nChars = dst + n
				nextChar = dst
			End If
		End Sub

		''' <summary>
		''' Reads a single character.
		''' </summary>
		''' <returns> The character read, as an integer in the range
		'''         0 to 65535 (<tt>0x00-0xffff</tt>), or -1 if the
		'''         end of the stream has been reached </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Function read() As Integer
			SyncLock lock
				ensureOpen()
				Do
					If nextChar >= nChars Then
						fill()
						If nextChar >= nChars Then Return -1
					End If
					If skipLF Then
						skipLF = False
						If cb(nextChar) = ControlChars.Lf Then
							nextChar += 1
							Continue Do
						End If
					End If
						Dim tempVar As Integer = nextChar
						nextChar += 1
						Return cb(tempVar)
				Loop
			End SyncLock
		End Function

		''' <summary>
		''' Reads characters into a portion of an array, reading from the underlying
		''' stream if necessary.
		''' </summary>
		Private Function read1(ByVal cbuf As Char(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			If nextChar >= nChars Then
	'             If the requested length is at least as large as the buffer, and
	'               if there is no mark/reset activity, and if line feeds are not
	'               being skipped, do not bother to copy the characters into the
	'               local buffer.  In this way buffered streams will cascade
	'               harmlessly. 
				If len >= cb.Length AndAlso markedChar <= UNMARKED AndAlso (Not skipLF) Then Return [in].read(cbuf, [off], len)
				fill()
			End If
			If nextChar >= nChars Then Return -1
			If skipLF Then
				skipLF = False
				If cb(nextChar) = ControlChars.Lf Then
					nextChar += 1
					If nextChar >= nChars Then fill()
					If nextChar >= nChars Then Return -1
				End If
			End If
			Dim n As Integer = System.Math.Min(len, nChars - nextChar)
			Array.Copy(cb, nextChar, cbuf, [off], n)
			nextChar += n
			Return n
		End Function

        ''' <summary>
        ''' Reads characters into a portion of an array.
        ''' 
        ''' <p> This method implements the general contract of the corresponding
        ''' <code><seealso cref="Reader#read(char[], int, int) read"/></code> method of the
        ''' <code><seealso cref="Reader"/></code> class.  As an additional convenience, it
        ''' attempts to read as many characters as possible by repeatedly invoking
        ''' the <code>read</code> method of the underlying stream.  This iterated
        ''' <code>read</code> continues until one of the following conditions becomes
        ''' true: <ul>
        ''' 
        '''   <li> The specified number of characters have been read,
        ''' 
        '''   <li> The <code>read</code> method of the underlying stream returns
        '''   <code>-1</code>, indicating end-of-file, or
        ''' 
        '''   <li> The <code>ready</code> method of the underlying stream
        '''   returns <code>false</code>, indicating that further input requests
        '''   would block.
        ''' 
        ''' </ul> If the first <code>read</code> on the underlying stream returns
        ''' <code>-1</code> to indicate end-of-file then this method returns
        ''' <code>-1</code>.  Otherwise this method returns the number of characters
        ''' actually read.
        ''' 
        ''' <p> Subclasses of this class are encouraged, but not required, to
        ''' attempt to read as many characters as possible in the same fashion.
        ''' 
        ''' <p> Ordinarily this method takes characters from this stream's character
        ''' buffer, filling it from the underlying stream as necessary.  If,
        ''' however, the buffer is empty, the mark is not valid, and the requested
        ''' length is at least as large as the buffer, then this method will read
        ''' characters directly from the underlying stream into the given array.
        ''' Thus redundant <code>BufferedReader</code>s will not copy data
        ''' unnecessarily.
        ''' </summary>
        ''' <param name="cbuf">  Destination buffer </param>
        ''' <param name="off">   Offset at which to start storing characters </param>
        ''' <param name="len">   Maximum number of characters to read
        ''' </param>
        ''' <returns>     The number of characters read, or -1 if the end of the
        '''             stream has been reached
        ''' </returns>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function read(cbuf() As Char, off As Integer, len As Integer) As Integer 'throws IOException
            SyncLock lock
                ensureOpen()
                If (off < 0) OrElse (off > cbuf.Length) OrElse (len() < 0) OrElse ((off + len()) > cbuf.Length) OrElse ((off + len()) < 0) Then
                    Throw New IndexOutOfBoundsException
                ElseIf len() = 0 Then
                    Return 0
                End If

                Dim n As Integer = read1(cbuf, off, len)
                If n <= 0 Then Return n
                Do While (n < len()) AndAlso [in].ready()
                    Dim n1 As Integer = read1(cbuf, off + n, len() - n)
                    If n1 <= 0 Then Exit Do
                    n += n1
                Loop
                Return n
            End SyncLock
        End Function
        ''' <summary>
        ''' Reads a line of text.  A line is considered to be terminated by any one
        ''' of a line feed ('\n'), a carriage return ('\r'), or a carriage return
        ''' followed immediately by a linefeed.
        ''' </summary>
        ''' <param name="ignoreLF">  If true, the next '\n' will be skipped
        ''' </param>
        ''' <returns>     A String containing the contents of the line, not including
        '''             any line-termination characters, or null if the end of the
        '''             stream has been reached
        ''' </returns>
        ''' <seealso cref=        java.io.LineNumberReader#readLine()
        ''' </seealso>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function readLine(ignoreLF As Boolean) As String  ' throws IOException
            Dim s As StringBuffer = Nothing
            Dim startChar As Integer

            SyncLock lock
                ensureOpen()
                Dim omitLF As Boolean = ignoreLF OrElse skipLF

bufferLoop:
                Do

                    If nextChar >= nChars Then fill()
                    If nextChar >= nChars Then ' EOF
                        If s IsNot Nothing AndAlso s.length() > 0 Then
                            Return s.ToString()
                        Else
                            Return Nothing
                        End If
                    End If
                    Dim eol As Boolean = False
                    Dim c As Char = 0
                    Dim i As Integer

                    ' Skip a leftover '\n', if necessary 
                    If omitLF AndAlso (cb(nextChar) = ControlChars.Lf) Then nextChar += 1
                    skipLF = False
                    omitLF = False

charLoop:
                    For i = nextChar To nChars - 1
                        c = cb(i)
                        If (c = ControlChars.Lf) OrElse (c = ControlChars.Cr) Then
                            eol = True
                            GoTo charLoop
                        End If
                    Next i

                    startChar = nextChar
                    nextChar = i

                    If eol Then
                        Dim str As String
                        If s Is Nothing Then
                            str = New String(cb, startChar, i - startChar)
                        Else
                            s.append(cb, startChar, i - startChar)
                            str = s.ToString()
                        End If
                        nextChar += 1
                        If c = ControlChars.Cr Then skipLF = True
                        Return str
                    End If

                    If s Is Nothing Then s = New StringBuffer(defaultExpectedLineLength)
                    s.append(cb, startChar, i - startChar)
                Loop
            End SyncLock
        End Function

        ''' <summary>
        ''' Reads a line of text.  A line is considered to be terminated by any one
        ''' of a line feed ('\n'), a carriage return ('\r'), or a carriage return
        ''' followed immediately by a linefeed.
        ''' </summary>
        ''' <returns>     A String containing the contents of the line, not including
        '''             any line-termination characters, or null if the end of the
        '''             stream has been reached
        ''' </returns>
        ''' <exception cref="IOException">  If an I/O error occurs
        ''' </exception>
        ''' <seealso cref= java.nio.file.Files#readAllLines </seealso>
        Public Function readLine() As String ' throws IOException
            Return readLine(False)
        End Function

        ''' <summary>
        ''' Skips characters.
        ''' </summary>
        ''' <param name="n">  The number of characters to skip
        ''' </param>
        ''' <returns>    The number of characters actually skipped
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">  If <code>n</code> is negative. </exception>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function skip(n As Long) As Long ' throws IOException
            If n < 0L Then Throw New IllegalArgumentException("skip value is negative")
            SyncLock lock
                ensureOpen()
                Dim r As Long = n
                Do While r > 0
                    If nextChar >= nChars Then fill()
                    If nextChar >= nChars Then ' EOF Exit Do
                        If skipLF Then
                            skipLF = False
                            If cb(nextChar) = ControlChars.Lf Then nextChar += 1
                        End If
                        Dim d As Long = nChars - nextChar
                        If r <= d Then
                            nextChar += r
                            r = 0
                            Exit Do
                        Else
                            r -= d
                            nextChar = nChars
                        End If
                Loop
                Return n - r
            End SyncLock
        End Function

        ''' <summary>
        ''' Tells whether this stream is ready to be read.  A buffered character
        ''' stream is ready if the buffer is not empty, or if the underlying
        ''' character stream is ready.
        ''' </summary>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Function ready() As Boolean ' throws IOException
            SyncLock lock
                ensureOpen()

                '            
                '             * If newline needs to be skipped and the next char to be read
                '             * is a newline character, then just skip it right away.
                '             
                If skipLF Then
                    '                 Note that in.ready() will return true if and only if the next
                    '                 * read on the stream will not block.
                    '                 
                    If nextChar >= nChars AndAlso [in].ready() Then fill()
                    If nextChar < nChars Then
                        If cb(nextChar) = ControlChars.Lf Then nextChar += 1
                        skipLF = False
                    End If
                End If
                Return (nextChar < nChars) OrElse [in].ready()
            End SyncLock
        End Function

        ''' <summary>
        ''' Tells whether this stream supports the mark() operation, which it does.
        ''' </summary>
        Public Function markSupported() As Boolean
            Return True
        End Function

        ''' <summary>
        ''' Marks the present position in the stream.  Subsequent calls to reset()
        ''' will attempt to reposition the stream to this point.
        ''' </summary>
        ''' <param name="readAheadLimit">   Limit on the number of characters that may be
        '''                         read while still preserving the mark. An attempt
        '''                         to reset the stream after reading characters
        '''                         up to this limit or beyond may fail.
        '''                         A limit value larger than the size of the input
        '''                         buffer will cause a new buffer to be allocated
        '''                         whose size is no smaller than limit.
        '''                         Therefore large values should be used with care.
        ''' </param>
        ''' <exception cref="IllegalArgumentException">  If {@code readAheadLimit < 0} </exception>
        ''' <exception cref="IOException">  If an I/O error occurs </exception>
        Public Sub mark(readAheadLimit As Integer) 'throws IOException
            If readAheadLimit < 0 Then Throw New IllegalArgumentException("Read-ahead limit < 0")
            SyncLock lock
                ensureOpen()
                Me.readAheadLimit = readAheadLimit
                markedChar = nextChar
                markedSkipLF = skipLF
            End SyncLock
        End Sub

        ''' <summary>
        ''' Resets the stream to the most recent mark.
        ''' </summary>
        ''' <exception cref="IOException">  If the stream has never been marked,
        '''                          or if the mark has been invalidated </exception>
        Public Sub reset() ' throws IOException
            SyncLock lock
                ensureOpen()
                If markedChar < 0 Then Throw New IOException(If(markedChar = INVALIDATED, "Mark invalid", "Stream not marked"))
                nextChar = markedChar
                skipLF = markedSkipLF
            End SyncLock
        End Sub

        Public Sub close() 'throws IOException
            SyncLock lock
                If [in] Is Nothing Then Return
                Try
                    [in].close()
                Finally
                    [in] = Nothing
                    cb = Nothing
                End Try
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns a {@code Stream}, the elements of which are lines read from
        ''' this {@code BufferedReader}.  The <seealso cref="Stream"/> is lazily populated,
        ''' i.e., read only occurs during the
        ''' <a href="../util/stream/package-summary.html#StreamOps">terminal
        ''' stream operation</a>.
        ''' 
        ''' <p> The reader must not be operated on during the execution of the
        ''' terminal stream operation. Otherwise, the result of the terminal stream
        ''' operation is undefined.
        ''' 
        ''' <p> After execution of the terminal stream operation there are no
        ''' guarantees that the reader will be at a specific position from which to
        ''' read the next character or line.
        ''' 
        ''' <p> If an <seealso cref="IOException"/> is thrown when accessing the underlying
        ''' {@code BufferedReader}, it is wrapped in an {@link
        ''' UncheckedIOException} which will be thrown from the {@code Stream}
        ''' method that caused the read to take place. This method will return a
        ''' Stream if invoked on a BufferedReader that is closed. Any operation on
        ''' that stream that requires reading from the BufferedReader after it is
        ''' closed, will cause an UncheckedIOException to be thrown.
        ''' </summary>
        ''' <returns> a {@code Stream<String>} providing the lines of text
        '''         described by this {@code BufferedReader}
        ''' 
        ''' @since 1.8 </returns>
        Public Function lines() As java.util.stream.Stream(Of String)
            Dim iter As IEnumerator(Of String) = New IteratorAnonymousInnerClassHelper(Of String)
            Return java.util.stream.StreamSupport.stream(Of String)(java.util.Spliterators.spliteratorUnknownSize(iter, java.util.spliterator.ORDERED Or java.util.spliterator.NONNULL), False)
        End Function

    End Class


	Private Class IteratorAnonymousInnerClassHelper(Of E)
		Implements IEnumerator(Of E)

		Friend nextLine As String = Nothing

        Public Function hasNext() As Boolean
            If nextLine IsNot Nothing Then
                Return True
            Else
                Try
                    nextLine = readLine()
                    Return (nextLine IsNot Nothing)
                Catch e As IOException
                    Throw New UncheckedIOException(e)
                End Try
            End If
        End Function

        Public Function [next]() As String
            If nextLine IsNot Nothing OrElse hasNext() Then
                Dim line As String = nextLine
                nextLine = Nothing
                Return line
            Else
                Throw New java.util.NoSuchElementException
            End If
        End Function
    End Class
End Namespace