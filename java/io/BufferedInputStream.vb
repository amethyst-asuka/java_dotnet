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
	''' A <code>BufferedInputStream</code> adds
	''' functionality to another input stream-namely,
	''' the ability to buffer the input and to
	''' support the <code>mark</code> and <code>reset</code>
	''' methods. When  the <code>BufferedInputStream</code>
	''' is created, an internal buffer array is
	''' created. As bytes  from the stream are read
	''' or skipped, the internal buffer is refilled
	''' as necessary  from the contained input stream,
	''' many bytes at a time. The <code>mark</code>
	''' operation  remembers a point in the input
	''' stream and the <code>reset</code> operation
	''' causes all the  bytes read since the most
	''' recent <code>mark</code> operation to be
	''' reread before new bytes are  taken from
	''' the contained input stream.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>
	Public Class BufferedInputStream
		Inherits FilterInputStream

		Private Shared DEFAULT_BUFFER_SIZE As Integer = 8192

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared MAX_BUFFER_SIZE As Integer = Integer.MAX_VALUE - 8

		''' <summary>
		''' The internal buffer array where the data is stored. When necessary,
		''' it may be replaced by another array of
		''' a different size.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend buf As SByte()

		''' <summary>
		''' Atomic updater to provide compareAndSet for buf. This is
		''' necessary because closes can be asynchronous. We use nullness
		''' of buf[] as primary indicator that this stream is closed. (The
		''' "in" field is also nulled out on close.)
		''' </summary>
		Private Shared ReadOnly bufUpdater As java.util.concurrent.atomic.AtomicReferenceFieldUpdater(Of BufferedInputStream, SByte()) = java.util.concurrent.atomic.AtomicReferenceFieldUpdater.newUpdater(GetType(BufferedInputStream), GetType(SByte()), "buf")

		''' <summary>
		''' The index one greater than the index of the last valid byte in
		''' the buffer.
		''' This value is always
		''' in the range <code>0</code> through <code>buf.length</code>;
		''' elements <code>buf[0]</code>  through <code>buf[count-1]
		''' </code>contain buffered input data obtained
		''' from the underlying  input stream.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' The current position in the buffer. This is the index of the next
		''' character to be read from the <code>buf</code> array.
		''' <p>
		''' This value is always in the range <code>0</code>
		''' through <code>count</code>. If it is less
		''' than <code>count</code>, then  <code>buf[pos]</code>
		''' is the next byte to be supplied as input;
		''' if it is equal to <code>count</code>, then
		''' the  next <code>read</code> or <code>skip</code>
		''' operation will require more bytes to be
		''' read from the contained  input stream.
		''' </summary>
		''' <seealso cref=     java.io.BufferedInputStream#buf </seealso>
		Protected Friend pos As Integer

		''' <summary>
		''' The value of the <code>pos</code> field at the time the last
		''' <code>mark</code> method was called.
		''' <p>
		''' This value is always
		''' in the range <code>-1</code> through <code>pos</code>.
		''' If there is no marked position in  the input
		''' stream, this field is <code>-1</code>. If
		''' there is a marked position in the input
		''' stream,  then <code>buf[markpos]</code>
		''' is the first byte to be supplied as input
		''' after a <code>reset</code> operation. If
		''' <code>markpos</code> is not <code>-1</code>,
		''' then all bytes from positions <code>buf[markpos]</code>
		''' through  <code>buf[pos-1]</code> must remain
		''' in the buffer array (though they may be
		''' moved to  another place in the buffer array,
		''' with suitable adjustments to the values
		''' of <code>count</code>,  <code>pos</code>,
		''' and <code>markpos</code>); they may not
		''' be discarded unless and until the difference
		''' between <code>pos</code> and <code>markpos</code>
		''' exceeds <code>marklimit</code>.
		''' </summary>
		''' <seealso cref=     java.io.BufferedInputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.BufferedInputStream#pos </seealso>
		Protected Friend markpos As Integer = -1

		''' <summary>
		''' The maximum read ahead allowed after a call to the
		''' <code>mark</code> method before subsequent calls to the
		''' <code>reset</code> method fail.
		''' Whenever the difference between <code>pos</code>
		''' and <code>markpos</code> exceeds <code>marklimit</code>,
		''' then the  mark may be dropped by setting
		''' <code>markpos</code> to <code>-1</code>.
		''' </summary>
		''' <seealso cref=     java.io.BufferedInputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.BufferedInputStream#reset() </seealso>
		Protected Friend marklimit As Integer

		''' <summary>
		''' Check to make sure that underlying input stream has not been
		''' nulled out due to close; if not return it;
		''' </summary>
		Private Property inIfOpen As InputStream
			Get
				Dim input As InputStream = [in]
				If input Is Nothing Then Throw New IOException("Stream closed")
				Return input
			End Get
		End Property

		''' <summary>
		''' Check to make sure that buffer has not been nulled out due to
		''' close; if not return it;
		''' </summary>
		Private Property bufIfOpen As SByte()
			Get
				Dim buffer As SByte() = buf
				If buffer Is Nothing Then Throw New IOException("Stream closed")
				Return buffer
			End Get
		End Property

		''' <summary>
		''' Creates a <code>BufferedInputStream</code>
		''' and saves its  argument, the input stream
		''' <code>in</code>, for later use. An internal
		''' buffer array is created and  stored in <code>buf</code>.
		''' </summary>
		''' <param name="in">   the underlying input stream. </param>
		Public Sub New(ByVal [in] As InputStream)
			Me.New([in], DEFAULT_BUFFER_SIZE)
		End Sub

		''' <summary>
		''' Creates a <code>BufferedInputStream</code>
		''' with the specified buffer size,
		''' and saves its  argument, the input stream
		''' <code>in</code>, for later use.  An internal
		''' buffer array of length  <code>size</code>
		''' is created and stored in <code>buf</code>.
		''' </summary>
		''' <param name="in">     the underlying input stream. </param>
		''' <param name="size">   the buffer size. </param>
		''' <exception cref="IllegalArgumentException"> if {@code size <= 0}. </exception>
		Public Sub New(ByVal [in] As InputStream, ByVal size As Integer)
			MyBase.New([in])
			If size <= 0 Then Throw New IllegalArgumentException("Buffer size <= 0")
			buf = New SByte(size - 1){}
		End Sub

		''' <summary>
		''' Fills the buffer with more data, taking into account
		''' shuffling and other tricks for dealing with marks.
		''' Assumes that it is being called by a synchronized method.
		''' This method also assumes that all data has already been read in,
		''' hence pos > count.
		''' </summary>
		Private Sub fill()
			Dim buffer As SByte() = bufIfOpen
			If markpos < 0 Then
				pos = 0 ' no mark: throw away the buffer
			ElseIf pos >= buffer.Length Then ' no room left in buffer
				If markpos > 0 Then ' can throw away early part of the buffer
					Dim sz As Integer = pos - markpos
					Array.Copy(buffer, markpos, buffer, 0, sz)
					pos = sz
					markpos = 0
				ElseIf buffer.Length >= marklimit Then
					markpos = -1 ' buffer got too big, invalidate mark
					pos = 0 ' drop buffer contents
				ElseIf buffer.Length >= MAX_BUFFER_SIZE Then
					Throw New OutOfMemoryError("Required array size too large") ' grow buffer
				Else
					Dim nsz As Integer = If(pos <= MAX_BUFFER_SIZE - pos, pos * 2, MAX_BUFFER_SIZE)
					If nsz > marklimit Then nsz = marklimit
					Dim nbuf As SByte() = New SByte(nsz - 1){}
					Array.Copy(buffer, 0, nbuf, 0, pos)
					If Not bufUpdater.compareAndSet(Me, buffer, nbuf) Then Throw New IOException("Stream closed")
					buffer = nbuf
				End If
			End If
			count = pos
			Dim n As Integer = inIfOpen.read(buffer, pos, buffer.Length - pos)
			If n > 0 Then count = n + pos
		End Sub

		''' <summary>
		''' See
		''' the general contract of the <code>read</code>
		''' method of <code>InputStream</code>.
		''' </summary>
		''' <returns>     the next byte of data, or <code>-1</code> if the end of the
		'''             stream is reached. </returns>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''                          invoking its <seealso cref="#close()"/> method,
		'''                          or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FilterInputStream#in </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function read() As Integer
			If pos >= count Then
				fill()
				If pos >= count Then Return -1
			End If
				Dim tempVar As Integer = pos
				pos += 1
				Return bufIfOpen(tempVar) And &Hff
		End Function

		''' <summary>
		''' Read characters into a portion of an array, reading from the underlying
		''' stream at most once if necessary.
		''' </summary>
		Private Function read1(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			Dim avail As Integer = count - pos
			If avail <= 0 Then
	'             If the requested length is at least as large as the buffer, and
	'               if there is no mark/reset activity, do not bother to copy the
	'               bytes into the local buffer.  In this way buffered streams will
	'               cascade harmlessly. 
				If len >= bufIfOpen.Length AndAlso markpos < 0 Then Return inIfOpen.read(b, [off], len)
				fill()
				avail = count - pos
				If avail <= 0 Then Return -1
			End If
			Dim cnt As Integer = If(avail < len, avail, len)
			Array.Copy(bufIfOpen, pos, b, [off], cnt)
			pos += cnt
			Return cnt
		End Function

		''' <summary>
		''' Reads bytes from this byte-input stream into the specified byte array,
		''' starting at the given offset.
		''' 
		''' <p> This method implements the general contract of the corresponding
		''' <code><seealso cref="InputStream#read(byte[], int, int) read"/></code> method of
		''' the <code><seealso cref="InputStream"/></code> class.  As an additional
		''' convenience, it attempts to read as many bytes as possible by repeatedly
		''' invoking the <code>read</code> method of the underlying stream.  This
		''' iterated <code>read</code> continues until one of the following
		''' conditions becomes true: <ul>
		''' 
		'''   <li> The specified number of bytes have been read,
		''' 
		'''   <li> The <code>read</code> method of the underlying stream returns
		'''   <code>-1</code>, indicating end-of-file, or
		''' 
		'''   <li> The <code>available</code> method of the underlying stream
		'''   returns zero, indicating that further input requests would block.
		''' 
		''' </ul> If the first <code>read</code> on the underlying stream returns
		''' <code>-1</code> to indicate end-of-file then this method returns
		''' <code>-1</code>.  Otherwise this method returns the number of bytes
		''' actually read.
		''' 
		''' <p> Subclasses of this class are encouraged, but not required, to
		''' attempt to read as many bytes as possible in the same fashion.
		''' </summary>
		''' <param name="b">     destination buffer. </param>
		''' <param name="off">   offset at which to start storing bytes. </param>
		''' <param name="len">   maximum number of bytes to read. </param>
		''' <returns>     the number of bytes read, or <code>-1</code> if the end of
		'''             the stream has been reached. </returns>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''                          invoking its <seealso cref="#close()"/> method,
		'''                          or an I/O error occurs. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public synchronized int read(byte b() , int off, int len) throws IOException
			bufIfOpen ' Check for closed stream
			If (off Or len Or (off + len) Or (b.length - (off + len))) < 0 Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			Dim n As Integer = 0
			Do
				Dim nread As Integer = read1(b, off + n, len - n)
				If nread <= 0 Then Return If(n = 0, nread, n)
				n += nread
				If n >= len Then Return n
				' if not closed but no bytes available, return
				Dim input As InputStream = [in]
				If input IsNot Nothing AndAlso input.available() <= 0 Then Return n
			Loop

		''' <summary>
		''' See the general contract of the <code>skip</code>
		''' method of <code>InputStream</code>.
		''' </summary>
		''' <exception cref="IOException">  if the stream does not support seek,
		'''                          or if this input stream has been closed by
		'''                          invoking its <seealso cref="#close()"/> method, or an
		'''                          I/O error occurs. </exception>
		public synchronized Long skip(Long n) throws IOException
			bufIfOpen ' Check for closed stream
			If n <= 0 Then Return 0
			Dim avail As Long = count - pos

			If avail <= 0 Then
				' If no mark position set then don't keep in buffer
				If markpos <0 Then Return inIfOpen.skip(n)

				' Fill in buffer to save bytes for reset
				fill()
				avail = count - pos
				If avail <= 0 Then Return 0
			End If

			Dim skipped As Long = If(avail < n, avail, n)
			pos += skipped
			Return skipped

		''' <summary>
		''' Returns an estimate of the number of bytes that can be read (or
		''' skipped over) from this input stream without blocking by the next
		''' invocation of a method for this input stream. The next invocation might be
		''' the same thread or another thread.  A single read or skip of this
		''' many bytes will not block, but may read or skip fewer bytes.
		''' <p>
		''' This method returns the sum of the number of bytes remaining to be read in
		''' the buffer (<code>count&nbsp;- pos</code>) and the result of calling the
		''' <seealso cref="java.io.FilterInputStream#in in"/>.available().
		''' </summary>
		''' <returns>     an estimate of the number of bytes that can be read (or skipped
		'''             over) from this input stream without blocking. </returns>
		''' <exception cref="IOException">  if this input stream has been closed by
		'''                          invoking its <seealso cref="#close()"/> method,
		'''                          or an I/O error occurs. </exception>
		public synchronized Integer available() throws IOException
			Dim n As Integer = count - pos
			Dim avail As Integer = inIfOpen.available()
			Return If(n > (Integer.MaxValue - avail), Integer.MaxValue, n + avail)

		''' <summary>
		''' See the general contract of the <code>mark</code>
		''' method of <code>InputStream</code>.
		''' </summary>
		''' <param name="readlimit">   the maximum limit of bytes that can be read before
		'''                      the mark position becomes invalid. </param>
		''' <seealso cref=     java.io.BufferedInputStream#reset() </seealso>
		public synchronized void mark(Integer readlimit)
			marklimit = readlimit
			markpos = pos

		''' <summary>
		''' See the general contract of the <code>reset</code>
		''' method of <code>InputStream</code>.
		''' <p>
		''' If <code>markpos</code> is <code>-1</code>
		''' (no mark has been set or the mark has been
		''' invalidated), an <code>IOException</code>
		''' is thrown. Otherwise, <code>pos</code> is
		''' set equal to <code>markpos</code>.
		''' </summary>
		''' <exception cref="IOException">  if this stream has not been marked or,
		'''                  if the mark has been invalidated, or the stream
		'''                  has been closed by invoking its <seealso cref="#close()"/>
		'''                  method, or an I/O error occurs. </exception>
		''' <seealso cref=        java.io.BufferedInputStream#mark(int) </seealso>
		public synchronized void reset() throws IOException
			bufIfOpen ' Cause exception if closed
			If markpos < 0 Then Throw New IOException("Resetting to invalid mark")
			pos = markpos

		''' <summary>
		''' Tests if this input stream supports the <code>mark</code>
		''' and <code>reset</code> methods. The <code>markSupported</code>
		''' method of <code>BufferedInputStream</code> returns
		''' <code>true</code>.
		''' </summary>
		''' <returns>  a <code>boolean</code> indicating if this stream type supports
		'''          the <code>mark</code> and <code>reset</code> methods. </returns>
		''' <seealso cref=     java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=     java.io.InputStream#reset() </seealso>
		public Boolean markSupported()
			Return True

		''' <summary>
		''' Closes this input stream and releases any system resources
		''' associated with the stream.
		''' Once the stream has been closed, further read(), available(), reset(),
		''' or skip() invocations will throw an IOException.
		''' Closing a previously closed stream has no effect.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public void close() throws IOException
			Dim buffer As SByte()
			buffer = buf
			Do While buffer IsNot Nothing
				If bufUpdater.compareAndSet(Me, buffer, Nothing) Then
					Dim input As InputStream = [in]
					[in] = Nothing
					If input IsNot Nothing Then input.close()
					Return
				End If
				' Else retry in case a new buf was CASed in fill()
				buffer = buf
			Loop
	End Class

End Namespace