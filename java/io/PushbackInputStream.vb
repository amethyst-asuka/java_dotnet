Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>PushbackInputStream</code> adds
	''' functionality to another input stream, namely
	''' the  ability to "push back" or "unread"
	''' one java.lang.[Byte]. This is useful in situations where
	''' it is  convenient for a fragment of code
	''' to read an indefinite number of data bytes
	''' that  are delimited by a particular byte
	''' value; after reading the terminating byte,
	''' the  code fragment can "unread" it, so that
	''' the next read operation on the input stream
	''' will reread the byte that was pushed back.
	''' For example, bytes representing the  characters
	''' constituting an identifier might be terminated
	''' by a byte representing an  operator character;
	''' a method whose job is to read just an identifier
	''' can read until it  sees the operator and
	''' then push the operator back to be re-read.
	''' 
	''' @author  David Connelly
	''' @author  Jonathan Payne
	''' @since   JDK1.0
	''' </summary>
	Public Class PushbackInputStream
		Inherits FilterInputStream

		''' <summary>
		''' The pushback buffer.
		''' @since   JDK1.1
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' The position within the pushback buffer from which the next byte will
		''' be read.  When the buffer is empty, <code>pos</code> is equal to
		''' <code>buf.length</code>; when the buffer is full, <code>pos</code> is
		''' equal to zero.
		''' 
		''' @since   JDK1.1
		''' </summary>
		Protected Friend pos As Integer

		''' <summary>
		''' Check to make sure that this stream has not been closed
		''' </summary>
		Private Sub ensureOpen()
			If [in] Is Nothing Then Throw New IOException("Stream closed")
		End Sub

		''' <summary>
		''' Creates a <code>PushbackInputStream</code>
		''' with a pushback buffer of the specified <code>size</code>,
		''' and saves its  argument, the input stream
		''' <code>in</code>, for later use. Initially,
		''' there is no pushed-back byte  (the field
		''' <code>pushBack</code> is initialized to
		''' <code>-1</code>).
		''' </summary>
		''' <param name="in">    the input stream from which bytes will be read. </param>
		''' <param name="size">  the size of the pushback buffer. </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0}
		''' @since  JDK1.1 </exception>
		Public Sub New(  [in] As InputStream,   size As Integer)
			MyBase.New([in])
			If size <= 0 Then Throw New IllegalArgumentException("size <= 0")
			Me.buf = New SByte(size - 1){}
			Me.pos = size
		End Sub

		''' <summary>
		''' Creates a <code>PushbackInputStream</code>
		''' and saves its  argument, the input stream
		''' <code>in</code>, for later use. Initially,
		''' there is no pushed-back byte  (the field
		''' <code>pushBack</code> is initialized to
		''' <code>-1</code>).
		''' </summary>
		''' <param name="in">   the input stream from which bytes will be read. </param>
		Public Sub New(  [in] As InputStream)
			Me.New([in], 1)
		End Sub

		''' <summary>
		''' Reads the next byte of data from this input stream. The value
		''' byte is returned as an <code>int</code> in the range
		''' <code>0</code> to <code>255</code>. If no byte is available
		''' because the end of the stream has been reached, the value
		''' <code>-1</code> is returned. This method blocks until input data
		''' is available, the end of the stream is detected, or an exception
		''' is thrown.
		''' 
		''' <p> This method returns the most recently pushed-back byte, if there is
		''' one, and otherwise calls the <code>read</code> method of its underlying
		''' input stream and returns whatever value that method returns.
		''' </summary>
		''' <returns>     the next byte of data, or <code>-1</code> if the end of the
		'''             stream has been reached. </returns>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''             invoking its <seealso cref="#close()"/> method,
		'''             or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.InputStream#read() </seealso>
		Public Overrides Function read() As Integer
			ensureOpen()
			If pos < buf.Length Then
					Dim tempVar As Integer = pos
					pos += 1
					Return buf(tempVar) And &Hff
			End If
			Return MyBase.read()
		End Function

		''' <summary>
		''' Reads up to <code>len</code> bytes of data from this input stream into
		''' an array of bytes.  This method first reads any pushed-back bytes; after
		''' that, if fewer than <code>len</code> bytes have been read then it
		''' reads from the underlying input stream. If <code>len</code> is not zero, the method
		''' blocks until at least 1 byte of input is available; otherwise, no
		''' bytes are read and <code>0</code> is returned.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset in the destination array <code>b</code> </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             <code>-1</code> if there is no more data because the end of
		'''             the stream has been reached. </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''             invoking its <seealso cref="#close()"/> method,
		'''             or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.InputStream#read(byte[], int, int) </seealso>
		Public Overrides Function read(  b As SByte(),   [off] As Integer,   len As Integer) As Integer
			ensureOpen()
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf [off] < 0 OrElse len < 0 OrElse len > b.Length - [off] Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			Dim avail As Integer = buf.Length - pos
			If avail > 0 Then
				If len < avail Then avail = len
				Array.Copy(buf, pos, b, [off], avail)
				pos += avail
				[off] += avail
				len -= avail
			End If
			If len > 0 Then
				len = MyBase.read(b, [off], len)
				If len = -1 Then Return If(avail = 0, -1, avail)
				Return avail + len
			End If
			Return avail
		End Function

		''' <summary>
		''' Pushes back a byte by copying it to the front of the pushback buffer.
		''' After this method returns, the next byte to be read will have the value
		''' <code>(byte)b</code>.
		''' </summary>
		''' <param name="b">   the <code>int</code> value whose low-order
		'''                  byte is to be pushed back. </param>
		''' <exception cref="IOException"> If there is not enough room in the pushback
		'''            buffer for the byte, or this input stream has been closed by
		'''            invoking its <seealso cref="#close()"/> method. </exception>
		Public Overridable Sub unread(  b As Integer)
			ensureOpen()
			If pos = 0 Then Throw New IOException("Push back buffer is full")
			pos -= 1
			buf(pos) = CByte(b)
		End Sub

		''' <summary>
		''' Pushes back a portion of an array of bytes by copying it to the front
		''' of the pushback buffer.  After this method returns, the next byte to be
		''' read will have the value <code>b[off]</code>, the byte after that will
		''' have the value <code>b[off+1]</code>, and so forth.
		''' </summary>
		''' <param name="b"> the byte array to push back. </param>
		''' <param name="off"> the start offset of the data. </param>
		''' <param name="len"> the number of bytes to push back. </param>
		''' <exception cref="IOException"> If there is not enough room in the pushback
		'''            buffer for the specified number of bytes,
		'''            or this input stream has been closed by
		'''            invoking its <seealso cref="#close()"/> method.
		''' @since     JDK1.1 </exception>
		Public Overridable Sub unread(  b As SByte(),   [off] As Integer,   len As Integer)
			ensureOpen()
			If len > pos Then Throw New IOException("Push back buffer is full")
			pos -= len
			Array.Copy(b, [off], buf, pos, len)
		End Sub

		''' <summary>
		''' Pushes back an array of bytes by copying it to the front of the
		''' pushback buffer.  After this method returns, the next byte to be read
		''' will have the value <code>b[0]</code>, the byte after that will have the
		''' value <code>b[1]</code>, and so forth.
		''' </summary>
		''' <param name="b"> the byte array to push back </param>
		''' <exception cref="IOException"> If there is not enough room in the pushback
		'''            buffer for the specified number of bytes,
		'''            or this input stream has been closed by
		'''            invoking its <seealso cref="#close()"/> method.
		''' @since     JDK1.1 </exception>
		Public Overridable Sub unread(  b As SByte())
			unread(b, 0, b.Length)
		End Sub

		''' <summary>
		''' Returns an estimate of the number of bytes that can be read (or
		''' skipped over) from this input stream without blocking by the next
		''' invocation of a method for this input stream. The next invocation might be
		''' the same thread or another thread.  A single read or skip of this
		''' many bytes will not block, but may read or skip fewer bytes.
		''' 
		''' <p> The method returns the sum of the number of bytes that have been
		''' pushed back and the value returned by {@link
		''' java.io.FilterInputStream#available available}.
		''' </summary>
		''' <returns>     the number of bytes that can be read (or skipped over) from
		'''             the input stream without blocking. </returns>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''             invoking its <seealso cref="#close()"/> method,
		'''             or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.InputStream#available() </seealso>
		Public Overrides Function available() As Integer
			ensureOpen()
			Dim n As Integer = buf.Length - pos
			Dim avail As Integer = MyBase.available()
            Return If(n > (java.lang.[Integer].MAX_VALUE - avail), java.lang.[Integer].MAX_VALUE, n + avail)
        End Function

		''' <summary>
		''' Skips over and discards <code>n</code> bytes of data from this
		''' input stream. The <code>skip</code> method may, for a variety of
		''' reasons, end up skipping over some smaller number of bytes,
		''' possibly zero.  If <code>n</code> is negative, no bytes are skipped.
		''' 
		''' <p> The <code>skip</code> method of <code>PushbackInputStream</code>
		''' first skips over the bytes in the pushback buffer, if any.  It then
		''' calls the <code>skip</code> method of the underlying input stream if
		''' more bytes need to be skipped.  The actual number of bytes skipped
		''' is returned.
		''' </summary>
		''' <param name="n">  {@inheritDoc} </param>
		''' <returns>     {@inheritDoc} </returns>
		''' <exception cref="IOException">  if the stream does not support seek,
		'''            or the stream has been closed by
		'''            invoking its <seealso cref="#close()"/> method,
		'''            or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		''' <seealso cref=        java.io.InputStream#skip(long n)
		''' @since      1.2 </seealso>
		Public Overrides Function skip(  n As Long) As Long
			ensureOpen()
			If n <= 0 Then Return 0

			Dim pskip As Long = buf.Length - pos
			If pskip > 0 Then
				If n < pskip Then pskip = n
				pos += pskip
				n -= pskip
			End If
			If n > 0 Then pskip += MyBase.skip(n)
			Return pskip
		End Function

		''' <summary>
		''' Tests if this input stream supports the <code>mark</code> and
		''' <code>reset</code> methods, which it does not.
		''' </summary>
		''' <returns>   <code>false</code>, since this class does not support the
		'''           <code>mark</code> and <code>reset</code> methods. </returns>
		''' <seealso cref=     java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.InputStream#reset() </seealso>
		Public Overrides Function markSupported() As Boolean
			Return False
		End Function

		''' <summary>
		''' Marks the current position in this input stream.
		''' 
		''' <p> The <code>mark</code> method of <code>PushbackInputStream</code>
		''' does nothing.
		''' </summary>
		''' <param name="readlimit">   the maximum limit of bytes that can be read before
		'''                      the mark position becomes invalid. </param>
		''' <seealso cref=     java.io.InputStream#reset() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub mark(  readlimit As Integer)
		End Sub

		''' <summary>
		''' Repositions this stream to the position at the time the
		''' <code>mark</code> method was last called on this input stream.
		''' 
		''' <p> The method <code>reset</code> for class
		''' <code>PushbackInputStream</code> does nothing except throw an
		''' <code>IOException</code>.
		''' </summary>
		''' <exception cref="IOException">  if this method is invoked. </exception>
		''' <seealso cref=     java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.IOException </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub reset()
			Throw New IOException("mark/reset not supported")
		End Sub

		''' <summary>
		''' Closes this input stream and releases any system resources
		''' associated with the stream.
		''' Once the stream has been closed, further read(), unread(),
		''' available(), reset(), or skip() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub close()
			If [in] Is Nothing Then Return
			[in].close()
			[in] = Nothing
			buf = Nothing
		End Sub
	End Class

End Namespace