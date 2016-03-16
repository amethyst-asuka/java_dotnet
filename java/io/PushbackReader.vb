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
	''' A character-stream reader that allows characters to be pushed back into the
	''' stream.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class PushbackReader
		Inherits FilterReader

		''' <summary>
		''' Pushback buffer </summary>
		Private buf As Char()

		''' <summary>
		''' Current position in buffer </summary>
		Private pos As Integer

		''' <summary>
		''' Creates a new pushback reader with a pushback buffer of the given size.
		''' </summary>
		''' <param name="in">   The reader from which characters will be read </param>
		''' <param name="size"> The size of the pushback buffer </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0} </exception>
		Public Sub New(ByVal [in] As Reader, ByVal size As Integer)
			MyBase.New([in])
			If size <= 0 Then Throw New IllegalArgumentException("size <= 0")
			Me.buf = New Char(size - 1){}
			Me.pos = size
		End Sub

		''' <summary>
		''' Creates a new pushback reader with a one-character pushback buffer.
		''' </summary>
		''' <param name="in">  The reader from which characters will be read </param>
		Public Sub New(ByVal [in] As Reader)
			Me.New([in], 1)
		End Sub

		''' <summary>
		''' Checks to make sure that the stream has not been closed. </summary>
		Private Sub ensureOpen()
			If buf Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Reads a single character.
		''' </summary>
		''' <returns>     The character read, or -1 if the end of the stream has been
		'''             reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Function read() As Integer
			SyncLock lock
				ensureOpen()
				If pos < buf.Length Then Dim tempVar As Integer = pos
						pos += 1
						Return buf(tempVar)
				Else
					Return MyBase.read()
				End If
			End SyncLock
		End Function

		''' <summary>
		''' Reads characters into a portion of an array.
		''' </summary>
		''' <param name="cbuf">  Destination buffer </param>
		''' <param name="off">   Offset at which to start writing characters </param>
		''' <param name="len">   Maximum number of characters to read
		''' </param>
		''' <returns>     The number of characters read, or -1 if the end of the
		'''             stream has been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(char cbuf() , int off, int len) throws IOException
			SyncLock lock
				ensureOpen()
				Try
					If len <= 0 Then
						If len < 0 Then
							Throw New IndexOutOfBoundsException
						ElseIf (off < 0) OrElse (off > cbuf.length) Then
							Throw New IndexOutOfBoundsException
						End If
						Return 0
					End If
					Dim avail As Integer = buf.Length - pos
					If avail > 0 Then
						If len < avail Then avail = len
						Array.Copy(buf, pos, cbuf, off, avail)
						pos += avail
						off += avail
						len -= avail
					End If
					If len > 0 Then
						len = MyBase.read(cbuf, off, len)
						If len = -1 Then Return If(avail = 0, -1, avail)
						Return avail + len
					End If
					Return avail
				Catch e As ArrayIndexOutOfBoundsException
					Throw New IndexOutOfBoundsException
				End Try
			End SyncLock

		''' <summary>
		''' Pushes back a single character by copying it to the front of the
		''' pushback buffer. After this method returns, the next character to be read
		''' will have the value <code>(char)c</code>.
		''' </summary>
		''' <param name="c">  The int value representing a character to be pushed back
		''' </param>
		''' <exception cref="IOException">  If the pushback buffer is full,
		'''                          or if some other I/O error occurs </exception>
		public  Sub  unread(Integer c) throws IOException
			SyncLock lock
				ensureOpen()
				If pos = 0 Then Throw New IOException("Pushback buffer overflow")
				pos -= 1
				buf(pos) = CChar(c)
			End SyncLock

		''' <summary>
		''' Pushes back a portion of an array of characters by copying it to the
		''' front of the pushback buffer.  After this method returns, the next
		''' character to be read will have the value <code>cbuf[off]</code>, the
		''' character after that will have the value <code>cbuf[off+1]</code>, and
		''' so forth.
		''' </summary>
		''' <param name="cbuf">  Character array </param>
		''' <param name="off">   Offset of first character to push back </param>
		''' <param name="len">   Number of characters to push back
		''' </param>
		''' <exception cref="IOException">  If there is insufficient room in the pushback
		'''                          buffer, or if some other I/O error occurs </exception>
		public  Sub  unread(Char cbuf() , Integer off, Integer len) throws IOException
			SyncLock lock
				ensureOpen()
				If len > pos Then Throw New IOException("Pushback buffer overflow")
				pos -= len
				Array.Copy(cbuf, off, buf, pos, len)
			End SyncLock

		''' <summary>
		''' Pushes back an array of characters by copying it to the front of the
		''' pushback buffer.  After this method returns, the next character to be
		''' read will have the value <code>cbuf[0]</code>, the character after that
		''' will have the value <code>cbuf[1]</code>, and so forth.
		''' </summary>
		''' <param name="cbuf">  Character array to push back
		''' </param>
		''' <exception cref="IOException">  If there is insufficient room in the pushback
		'''                          buffer, or if some other I/O error occurs </exception>
		public  Sub  unread(Char cbuf()) throws IOException
			unread(cbuf, 0, cbuf.length)

		''' <summary>
		''' Tells whether this stream is ready to be read.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Boolean ready() throws IOException
			SyncLock lock
				ensureOpen()
				Return (pos < buf.Length) OrElse MyBase.ready()
			End SyncLock

		''' <summary>
		''' Marks the present position in the stream. The <code>mark</code>
		''' for class <code>PushbackReader</code> always throws an exception.
		''' </summary>
		''' <exception cref="IOException">  Always, since mark is not supported </exception>
		public  Sub  mark(Integer readAheadLimit) throws IOException
			Throw New IOException("mark/reset not supported")

		''' <summary>
		''' Resets the stream. The <code>reset</code> method of
		''' <code>PushbackReader</code> always throws an exception.
		''' </summary>
		''' <exception cref="IOException">  Always, since reset is not supported </exception>
		public  Sub  reset() throws IOException
			Throw New IOException("mark/reset not supported")

		''' <summary>
		''' Tells whether this stream supports the mark() operation, which it does
		''' not.
		''' </summary>
		public Boolean markSupported()
			Return False

		''' <summary>
		''' Closes the stream and releases any system resources associated with
		''' it. Once the stream has been closed, further read(),
		''' unread(), ready(), or skip() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  close() throws IOException
			MyBase.close()
			buf = Nothing

		''' <summary>
		''' Skips characters.  This method will block until some characters are
		''' available, an I/O error occurs, or the end of the stream is reached.
		''' </summary>
		''' <param name="n">  The number of characters to skip
		''' </param>
		''' <returns>    The number of characters actually skipped
		''' </returns>
		''' <exception cref="IllegalArgumentException">  If <code>n</code> is negative. </exception>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Long skip(Long n) throws IOException
			If n < 0L Then Throw New IllegalArgumentException("skip value is negative")
			SyncLock lock
				ensureOpen()
				Dim avail As Integer = buf.Length - pos
				If avail > 0 Then
					If n <= avail Then
						pos += n
						Return n
					Else
						pos = buf.Length
						n -= avail
					End If
				End If
				Return avail + MyBase.skip(n)
			End SyncLock
	End Class

End Namespace