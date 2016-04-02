Imports System.Threading

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
	''' Piped character-input streams.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class PipedReader
		Inherits Reader

		Friend closedByWriter As Boolean = False
		Friend closedByReader As Boolean = False
		Friend connected As Boolean = False

	'     REMIND: identification of the read and write sides needs to be
	'       more sophisticated.  Either using thread groups (but what about
	'       pipes within a thread?) or using finalization (but it may be a
	'       long time until the next GC). 
		Friend readSide As Thread
		Friend writeSide As Thread

	   ''' <summary>
	   ''' The size of the pipe's circular input buffer.
	   ''' </summary>
		Private Const DEFAULT_PIPE_SIZE As Integer = 1024

		''' <summary>
		''' The circular buffer into which incoming data is placed.
		''' </summary>
		Friend buffer As Char()

		''' <summary>
		''' The index of the position in the circular buffer at which the
		''' next character of data will be stored when received from the connected
		''' piped writer. <code>in&lt;0</code> implies the buffer is empty,
		''' <code>in==out</code> implies the buffer is full
		''' </summary>
		Friend [in] As Integer = -1

		''' <summary>
		''' The index of the position in the circular buffer at which the next
		''' character of data will be read by this piped reader.
		''' </summary>
		Friend out As Integer = 0

		''' <summary>
		''' Creates a <code>PipedReader</code> so
		''' that it is connected to the piped writer
		''' <code>src</code>. Data written to <code>src</code>
		''' will then be available as input from this stream.
		''' </summary>
		''' <param name="src">   the stream to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Sub New(ByVal src As PipedWriter)
			Me.New(src, DEFAULT_PIPE_SIZE)
		End Sub

		''' <summary>
		''' Creates a <code>PipedReader</code> so that it is connected
		''' to the piped writer <code>src</code> and uses the specified
		''' pipe size for the pipe's buffer. Data written to <code>src</code>
		''' will then be  available as input from this stream.
		''' </summary>
		''' <param name="src">       the stream to connect to. </param>
		''' <param name="pipeSize">  the size of the pipe's buffer. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		''' @since      1.6 </exception>
		Public Sub New(ByVal src As PipedWriter, ByVal pipeSize As Integer)
			initPipe(pipeSize)
			connect(src)
		End Sub


		''' <summary>
		''' Creates a <code>PipedReader</code> so
		''' that it is not yet {@link #connect(java.io.PipedWriter)
		''' connected}. It must be {@link java.io.PipedWriter#connect(
		''' java.io.PipedReader) connected} to a <code>PipedWriter</code>
		''' before being used.
		''' </summary>
		Public Sub New()
			initPipe(DEFAULT_PIPE_SIZE)
		End Sub

		''' <summary>
		''' Creates a <code>PipedReader</code> so that it is not yet
		''' <seealso cref="#connect(java.io.PipedWriter) connected"/> and uses
		''' the specified pipe size for the pipe's buffer.
		''' It must be  {@link java.io.PipedWriter#connect(
		''' java.io.PipedReader) connected} to a <code>PipedWriter</code>
		''' before being used.
		''' </summary>
		''' <param name="pipeSize"> the size of the pipe's buffer. </param>
		''' <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		''' @since      1.6 </exception>
		Public Sub New(ByVal pipeSize As Integer)
			initPipe(pipeSize)
		End Sub

		Private Sub initPipe(ByVal pipeSize As Integer)
			If pipeSize <= 0 Then Throw New IllegalArgumentException("Pipe size <= 0")
			buffer = New Char(pipeSize - 1){}
		End Sub

		''' <summary>
		''' Causes this piped reader to be connected
		''' to the piped  writer <code>src</code>.
		''' If this object is already connected to some
		''' other piped writer, an <code>IOException</code>
		''' is thrown.
		''' <p>
		''' If <code>src</code> is an
		''' unconnected piped writer and <code>snk</code>
		''' is an unconnected piped reader, they
		''' may be connected by either the call:
		''' 
		''' <pre><code>snk.connect(src)</code> </pre>
		''' <p>
		''' or the call:
		''' 
		''' <pre><code>src.connect(snk)</code> </pre>
		''' <p>
		''' The two calls have the same effect.
		''' </summary>
		''' <param name="src">   The piped writer to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Overridable Sub connect(ByVal src As PipedWriter)
			src.connect(Me)
		End Sub

		''' <summary>
		''' Receives a char of data. This method will block if no input is
		''' available.
		''' </summary>
		SyncLock  Sub  receive Integer c
			Dim IOException As throws
			If Not connected Then
				Throw New IOException("Pipe not connected")
			ElseIf closedByWriter OrElse closedByReader Then
				Throw New IOException("Pipe closed")
			ElseIf readSide IsNot Nothing AndAlso (Not readSide.alive) Then
				Throw New IOException("Read end dead")
			End If

			writeSide = Thread.CurrentThread
			Do While [in] = out
				If (readSide IsNot Nothing) AndAlso (Not readSide.alive) Then Throw New IOException("Pipe broken")
				' full: kick any waiting readers 
				notifyAll()
				Try
					wait(1000)
				Catch ex As InterruptedException
					Throw New java.io.InterruptedIOException
				End Try
			Loop
			If [in] < 0 Then
				[in] = 0
				out = 0
			End If
			buffer([in]) = CChar(c)
			[in] += 1
			If [in] >= buffer.Length Then [in] = 0
		End SyncLock

		''' <summary>
		''' Receives data into an array of characters.  This method will
		''' block until some input is available.
		''' </summary>
		SyncLock  Sub  receive Char c() , Integer off, Integer len
			Dim IOException As throws
			len -= 1
			Do While len >= 0
				receive(c(off))
				off += 1
				len -= 1
			Loop
		End SyncLock

		''' <summary>
		''' Notifies all waiting threads that the last character of data has been
		''' received.
		''' </summary>
		SyncLock  Sub  receivedLast
			closedByWriter = True
			notifyAll()
		End SyncLock

		''' <summary>
		''' Reads the next character of data from this piped stream.
		''' If no character is available because the end of the stream
		''' has been reached, the value <code>-1</code> is returned.
		''' This method blocks until input data is available, the end of
		''' the stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next character of data, or <code>-1</code> if the end of the
		'''             stream is reached. </returns>
		''' <exception cref="IOException">  if the pipe is
		'''          <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		'''          <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, closed,
		'''          or an I/O error occurs. </exception>
		Public  Integer read() throws IOException
			If Not connected Then
				Throw New IOException("Pipe not connected")
			ElseIf closedByReader Then
				Throw New IOException("Pipe closed")
			ElseIf writeSide IsNot Nothing AndAlso (Not writeSide.alive) AndAlso (Not closedByWriter) AndAlso ([in] < 0) Then
				Throw New IOException("Write end dead")
			End If

			readSide = Thread.CurrentThread
			Dim trials As Integer = 2
			Do While [in] < 0
				If closedByWriter Then Return -1
				trials -= 1
				If (writeSide IsNot Nothing) AndAlso ((Not writeSide.alive)) AndAlso (trials < 0) Then Throw New IOException("Pipe broken")
				' might be a writer waiting 
				notifyAll()
				Try
					wait(1000)
				Catch ex As InterruptedException
					Throw New java.io.InterruptedIOException
				End Try
			Loop
			Dim ret As Integer = AscW(buffer(out))
			out += 1
			If out >= buffer.Length Then out = 0
			If [in] = out Then [in] = -1
			Return ret

		''' <summary>
		''' Reads up to <code>len</code> characters of data from this piped
		''' stream into an array of characters. Less than <code>len</code> characters
		''' will be read if the end of the data stream is reached or if
		''' <code>len</code> exceeds the pipe's buffer size. This method
		''' blocks until at least one character of input is available.
		''' </summary>
		''' <param name="cbuf">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset of the data. </param>
		''' <param name="len">   the maximum number of characters read. </param>
		''' <returns>     the total number of characters read into the buffer, or
		'''             <code>-1</code> if there is no more data because the end of
		'''             the stream has been reached. </returns>
		''' <exception cref="IOException">  if the pipe is
		'''                  <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		'''                  <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, closed,
		'''                  or an I/O error occurs. </exception>
		Public  Integer read(Char cbuf() , Integer off, Integer len) throws IOException
			If Not connected Then
				Throw New IOException("Pipe not connected")
			ElseIf closedByReader Then
				Throw New IOException("Pipe closed")
			ElseIf writeSide IsNot Nothing AndAlso (Not writeSide.alive) AndAlso (Not closedByWriter) AndAlso ([in] < 0) Then
				Throw New IOException("Write end dead")
			End If

			If (off < 0) OrElse (off > cbuf.length) OrElse (len < 0) OrElse ((off + len) > cbuf.length) OrElse ((off + len) < 0) Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			' possibly wait on the first character 
			Dim c As Integer = read()
			If c < 0 Then Return -1
			cbuf(off) = ChrW(c)
			Dim rlen As Integer = 1
			len -= 1
			Do While ([in] >= 0) AndAlso (len > 0)
				cbuf(off + rlen) = buffer(out)
				out += 1
				rlen += 1
				If out >= buffer.Length Then out = 0
				If [in] = out Then [in] = -1
				len -= 1
			Loop
			Return rlen

		''' <summary>
		''' Tell whether this stream is ready to be read.  A piped character
		''' stream is ready if the circular buffer is not empty.
		''' </summary>
		''' <exception cref="IOException">  if the pipe is
		'''                  <a href=PipedInputStream.html#BROKEN> <code>broken</code></a>,
		'''                  <seealso cref="#connect(java.io.PipedWriter) unconnected"/>, or closed. </exception>
		Public  Boolean ready() throws IOException
			If Not connected Then
				Throw New IOException("Pipe not connected")
			ElseIf closedByReader Then
				Throw New IOException("Pipe closed")
			ElseIf writeSide IsNot Nothing AndAlso (Not writeSide.alive) AndAlso (Not closedByWriter) AndAlso ([in] < 0) Then
				Throw New IOException("Write end dead")
			End If
			If [in] < 0 Then
				Return False
			Else
				Return True
			End If

		''' <summary>
		''' Closes this piped stream and releases any system resources
		''' associated with the stream.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public  Sub  close() throws IOException
			[in] = -1
			closedByReader = True
	End Class

End Namespace