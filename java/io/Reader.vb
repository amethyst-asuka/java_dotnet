Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Abstract class for reading character streams.  The only methods that a
	''' subclass must implement are read(char[], int, int) and close().  Most
	''' subclasses, however, will override some of the methods defined here in order
	''' to provide higher efficiency, additional functionality, or both.
	''' 
	''' </summary>
	''' <seealso cref= BufferedReader </seealso>
	''' <seealso cref=   LineNumberReader </seealso>
	''' <seealso cref= CharArrayReader </seealso>
	''' <seealso cref= InputStreamReader </seealso>
	''' <seealso cref=   FileReader </seealso>
	''' <seealso cref= FilterReader </seealso>
	''' <seealso cref=   PushbackReader </seealso>
	''' <seealso cref= PipedReader </seealso>
	''' <seealso cref= StringReader </seealso>
	''' <seealso cref= Writer
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public MustInherit Class Reader
		Implements Readable, Closeable

			Public MustOverride Function read(ByVal cb As java.nio.CharBuffer) As Integer Implements Readable.read

		''' <summary>
		''' The object used to synchronize operations on this stream.  For
		''' efficiency, a character-stream object may use an object other than
		''' itself to protect critical sections.  A subclass should therefore use
		''' the object in this field rather than <tt>this</tt> or a synchronized
		''' method.
		''' </summary>
		Protected Friend lock As Object

		''' <summary>
		''' Creates a new character-stream reader whose critical sections will
		''' synchronize on the reader itself.
		''' </summary>
		Protected Friend Sub New()
			Me.lock = Me
		End Sub

		''' <summary>
		''' Creates a new character-stream reader whose critical sections will
		''' synchronize on the given object.
		''' </summary>
		''' <param name="lock">  The Object to synchronize on. </param>
		Protected Friend Sub New(ByVal lock As Object)
			If lock Is Nothing Then Throw New NullPointerException
			Me.lock = lock
		End Sub

		''' <summary>
		''' Attempts to read characters into the specified character buffer.
		''' The buffer is used as a repository of characters as-is: the only
		''' changes made are the results of a put operation. No flipping or
		''' rewinding of the buffer is performed.
		''' </summary>
		''' <param name="target"> the buffer to read characters into </param>
		''' <returns> The number of characters added to the buffer, or
		'''         -1 if this source of characters is at its end </returns>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="NullPointerException"> if target is null </exception>
		''' <exception cref="java.nio.ReadOnlyBufferException"> if target is a read only buffer
		''' @since 1.5 </exception>
		Public Overridable Function read(ByVal target As java.nio.CharBuffer) As Integer Implements Readable.read
			Dim len As Integer = target.remaining()
			Dim cbuf As Char() = New Char(len - 1){}
			Dim n As Integer = read(cbuf, 0, len)
			If n > 0 Then target.put(cbuf, 0, n)
			Return n
		End Function

		''' <summary>
		''' Reads a single character.  This method will block until a character is
		''' available, an I/O error occurs, or the end of the stream is reached.
		''' 
		''' <p> Subclasses that intend to support efficient single-character input
		''' should override this method.
		''' </summary>
		''' <returns>     The character read, as an integer in the range 0 to 65535
		'''             (<tt>0x00-0xffff</tt>), or -1 if the end of the stream has
		'''             been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overridable Function read() As Integer
			Dim cb As Char() = New Char(0){}
			If read(cb, 0, 1) = -1 Then
				Return -1
			Else
				Return cb(0)
			End If
		End Function

		''' <summary>
		''' Reads characters into an array.  This method will block until some input
		''' is available, an I/O error occurs, or the end of the stream is reached.
		''' </summary>
		''' <param name="cbuf">  Destination buffer
		''' </param>
		''' <returns>      The number of characters read, or -1
		'''              if the end of the stream
		'''              has been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overridable Function read(ByVal cbuf As Char()) As Integer
			Return read(cbuf, 0, cbuf.Length)
		End Function

		''' <summary>
		''' Reads characters into a portion of an array.  This method will block
		''' until some input is available, an I/O error occurs, or the end of the
		''' stream is reached.
		''' </summary>
		''' <param name="cbuf">  Destination buffer </param>
		''' <param name="off">   Offset at which to start storing characters </param>
		''' <param name="len">   Maximum number of characters to read
		''' </param>
		''' <returns>     The number of characters read, or -1 if the end of the
		'''             stream has been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public MustOverride Function read(Char ByVal As cbuf(), ByVal [off] As Integer, ByVal len As Integer) As Integer

		''' <summary>
		''' Maximum skip-buffer size </summary>
		Private Const maxSkipBufferSize As Integer = 8192

		''' <summary>
		''' Skip buffer, null until allocated </summary>
		Private skipBuffer As Char() = Nothing

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
		Public Overridable Function skip(ByVal n As Long) As Long
			If n < 0L Then Throw New IllegalArgumentException("skip value is negative")
			Dim nn As Integer = CInt(Fix(Math.Min(n, maxSkipBufferSize)))
			SyncLock lock
				If (skipBuffer Is Nothing) OrElse (skipBuffer.Length < nn) Then skipBuffer = New Char(nn - 1){}
				Dim r As Long = n
				Do While r > 0
					Dim nc As Integer = read(skipBuffer, 0, CInt(Fix(Math.Min(r, nn))))
					If nc = -1 Then Exit Do
					r -= nc
				Loop
				Return n - r
			End SyncLock
		End Function

		''' <summary>
		''' Tells whether this stream is ready to be read.
		''' </summary>
		''' <returns> True if the next read() is guaranteed not to block for input,
		''' false otherwise.  Note that returning false does not guarantee that the
		''' next read will block.
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overridable Function ready() As Boolean
			Return False
		End Function

		''' <summary>
		''' Tells whether this stream supports the mark() operation. The default
		''' implementation always returns false. Subclasses should override this
		''' method.
		''' </summary>
		''' <returns> true if and only if this stream supports the mark operation. </returns>
		Public Overridable Function markSupported() As Boolean
			Return False
		End Function

		''' <summary>
		''' Marks the present position in the stream.  Subsequent calls to reset()
		''' will attempt to reposition the stream to this point.  Not all
		''' character-input streams support the mark() operation.
		''' </summary>
		''' <param name="readAheadLimit">  Limit on the number of characters that may be
		'''                         read while still preserving the mark.  After
		'''                         reading this many characters, attempting to
		'''                         reset the stream may fail.
		''' </param>
		''' <exception cref="IOException">  If the stream does not support mark(),
		'''                          or if some other I/O error occurs </exception>
		Public Overridable Sub mark(ByVal readAheadLimit As Integer)
			Throw New IOException("mark() not supported")
		End Sub

		''' <summary>
		''' Resets the stream.  If the stream has been marked, then attempt to
		''' reposition it at the mark.  If the stream has not been marked, then
		''' attempt to reset it in some way appropriate to the particular stream,
		''' for example by repositioning it to its starting point.  Not all
		''' character-input streams support the reset() operation, and some support
		''' reset() without supporting mark().
		''' </summary>
		''' <exception cref="IOException">  If the stream has not been marked,
		'''                          or if the mark has been invalidated,
		'''                          or if the stream does not support reset(),
		'''                          or if some other I/O error occurs </exception>
		Public Overridable Sub reset()
			Throw New IOException("reset() not supported")
		End Sub

		''' <summary>
		''' Closes the stream and releases any system resources associated with
		''' it.  Once the stream has been closed, further read(), ready(),
		''' mark(), reset(), or skip() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		 Public MustOverride Sub close() Implements Closeable.close

	End Class

End Namespace