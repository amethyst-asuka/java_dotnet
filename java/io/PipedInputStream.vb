Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A piped input stream should be connected
	''' to a piped output stream; the piped  input
	''' stream then provides whatever data bytes
	''' are written to the piped output  stream.
	''' Typically, data is read from a <code>PipedInputStream</code>
	''' object by one thread  and data is written
	''' to the corresponding <code>PipedOutputStream</code>
	''' by some  other thread. Attempting to use
	''' both objects from a single thread is not
	''' recommended, as it may deadlock the thread.
	''' The piped input stream contains a buffer,
	''' decoupling read operations from write operations,
	''' within limits.
	''' A pipe is said to be <a name="BROKEN"> <i>broken</i> </a> if a
	''' thread that was providing data bytes to the connected
	''' piped output stream is no longer alive.
	''' 
	''' @author  James Gosling </summary>
	''' <seealso cref=     java.io.PipedOutputStream
	''' @since   JDK1.0 </seealso>
	Public Class PipedInputStream
		Inherits InputStream

		Friend closedByWriter As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Friend closedByReader As Boolean = False
		Friend connected As Boolean = False

	'         REMIND: identification of the read and write sides needs to be
	'           more sophisticated.  Either using thread groups (but what about
	'           pipes within a thread?) or using finalization (but it may be a
	'           long time until the next GC). 
		Friend readSide As Thread
		Friend writeSide As Thread

		Private Const DEFAULT_PIPE_SIZE As Integer = 1024

		''' <summary>
		''' The default size of the pipe's circular input buffer.
		''' @since   JDK1.1
		''' </summary>
		' This used to be a constant before the pipe size was allowed
		' to change. This field will continue to be maintained
		' for backward compatibility.
		Protected Friend Const PIPE_SIZE As Integer = DEFAULT_PIPE_SIZE

		''' <summary>
		''' The circular buffer into which incoming data is placed.
		''' @since   JDK1.1
		''' </summary>
		Protected Friend buffer As SByte()

		''' <summary>
		''' The index of the position in the circular buffer at which the
		''' next byte of data will be stored when received from the connected
		''' piped output stream. <code>in&lt;0</code> implies the buffer is empty,
		''' <code>in==out</code> implies the buffer is full
		''' @since   JDK1.1
		''' </summary>
		Protected Friend [in] As Integer = -1

		''' <summary>
		''' The index of the position in the circular buffer at which the next
		''' byte of data will be read by this piped input stream.
		''' @since   JDK1.1
		''' </summary>
		Protected Friend out As Integer = 0

		''' <summary>
		''' Creates a <code>PipedInputStream</code> so
		''' that it is connected to the piped output
		''' stream <code>src</code>. Data bytes written
		''' to <code>src</code> will then be  available
		''' as input from this stream.
		''' </summary>
		''' <param name="src">   the stream to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Sub New(ByVal src As PipedOutputStream)
			Me.New(src, DEFAULT_PIPE_SIZE)
		End Sub

		''' <summary>
		''' Creates a <code>PipedInputStream</code> so that it is
		''' connected to the piped output stream
		''' <code>src</code> and uses the specified pipe size for
		''' the pipe's buffer.
		''' Data bytes written to <code>src</code> will then
		''' be available as input from this stream.
		''' </summary>
		''' <param name="src">   the stream to connect to. </param>
		''' <param name="pipeSize"> the size of the pipe's buffer. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		''' @since      1.6 </exception>
		Public Sub New(ByVal src As PipedOutputStream, ByVal pipeSize As Integer)
			 initPipe(pipeSize)
			 connect(src)
		End Sub

		''' <summary>
		''' Creates a <code>PipedInputStream</code> so
		''' that it is not yet {@link #connect(java.io.PipedOutputStream)
		''' connected}.
		''' It must be {@link java.io.PipedOutputStream#connect(
		''' java.io.PipedInputStream) connected} to a
		''' <code>PipedOutputStream</code> before being used.
		''' </summary>
		Public Sub New()
			initPipe(DEFAULT_PIPE_SIZE)
		End Sub

		''' <summary>
		''' Creates a <code>PipedInputStream</code> so that it is not yet
		''' <seealso cref="#connect(java.io.PipedOutputStream) connected"/> and
		''' uses the specified pipe size for the pipe's buffer.
		''' It must be {@link java.io.PipedOutputStream#connect(
		''' java.io.PipedInputStream)
		''' connected} to a <code>PipedOutputStream</code> before being used.
		''' </summary>
		''' <param name="pipeSize"> the size of the pipe's buffer. </param>
		''' <exception cref="IllegalArgumentException"> if {@code pipeSize <= 0}.
		''' @since      1.6 </exception>
		Public Sub New(ByVal pipeSize As Integer)
			initPipe(pipeSize)
		End Sub

		Private Sub initPipe(ByVal pipeSize As Integer)
			 If pipeSize <= 0 Then Throw New IllegalArgumentException("Pipe Size <= 0")
			 buffer = New SByte(pipeSize - 1){}
		End Sub

		''' <summary>
		''' Causes this piped input stream to be connected
		''' to the piped  output stream <code>src</code>.
		''' If this object is already connected to some
		''' other piped output  stream, an <code>IOException</code>
		''' is thrown.
		''' <p>
		''' If <code>src</code> is an
		''' unconnected piped output stream and <code>snk</code>
		''' is an unconnected piped input stream, they
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
		''' <param name="src">   The piped output stream to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Overridable Sub connect(ByVal src As PipedOutputStream)
			src.connect(Me)
		End Sub

		''' <summary>
		''' Receives a byte of data.  This method will block if no input is
		''' available. </summary>
		''' <param name="b"> the byte being received </param>
		''' <exception cref="IOException"> If the pipe is <a href="#BROKEN"> <code>broken</code></a>,
		'''          <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		'''          closed, or if an I/O error occurs.
		''' @since     JDK1.1 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub receive(ByVal b As Integer)
			checkStateForReceive()
			writeSide = Thread.CurrentThread
			If [in] = out Then awaitSpace()
			If [in] < 0 Then
				[in] = 0
				out = 0
			End If
			buffer([in]) = CByte(b And &HFF)
			[in] += 1
			If [in] >= buffer.Length Then [in] = 0
		End Sub

		''' <summary>
		''' Receives data into an array of bytes.  This method will
		''' block until some input is available. </summary>
		''' <param name="b"> the buffer into which the data is received </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes received </param>
		''' <exception cref="IOException"> If the pipe is <a href="#BROKEN"> broken</a>,
		'''           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		'''           closed,or if an I/O error occurs. </exception>
		SyncLock  Sub  receive SByte b() , Integer off, Integer len
			Dim IOException As throws
			checkStateForReceive()
			writeSide = Thread.CurrentThread
			Dim bytesToTransfer As Integer = len
			Do While bytesToTransfer > 0
				If [in] = out Then awaitSpace()
				Dim nextTransferAmount As Integer = 0
				If out < [in] Then
					nextTransferAmount = buffer.Length - [in]
				ElseIf [in] < out Then
					If [in] = -1 Then
							out = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							= out
						nextTransferAmount = buffer.Length - [in]
					Else
						nextTransferAmount = out - [in]
					End If
				End If
				If nextTransferAmount > bytesToTransfer Then nextTransferAmount = bytesToTransfer
				assert(nextTransferAmount > 0)
				Array.Copy(b, off, buffer, [in], nextTransferAmount)
				bytesToTransfer -= nextTransferAmount
				off += nextTransferAmount
				[in] += nextTransferAmount
				If [in] >= buffer.Length Then [in] = 0
			Loop
		End SyncLock

		private  Sub  checkStateForReceive() throws IOException
			If Not connected Then
				Throw New IOException("Pipe not connected")
			ElseIf closedByWriter OrElse closedByReader Then
				Throw New IOException("Pipe closed")
			ElseIf readSide IsNot Nothing AndAlso (Not readSide.alive) Then
				Throw New IOException("Read end dead")
			End If

		private  Sub  awaitSpace() throws IOException
			Do While [in] = out
				checkStateForReceive()

				' full: kick any waiting readers 
				notifyAll()
				Try
					wait(1000)
				Catch ex As InterruptedException
					Throw New java.io.InterruptedIOException
				End Try
			Loop

		''' <summary>
		''' Notifies all waiting threads that the last byte of data has been
		''' received.
		''' </summary>
		SyncLock  Sub  receivedLast
			closedByWriter = True
			notifyAll()
		End SyncLock

		''' <summary>
		''' Reads the next byte of data from this piped input stream. The
		''' value byte is returned as an <code>int</code> in the range
		''' <code>0</code> to <code>255</code>.
		''' This method blocks until input data is available, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next byte of data, or <code>-1</code> if the end of the
		'''             stream is reached. </returns>
		''' <exception cref="IOException">  if the pipe is
		'''           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		'''           <a href="#BROKEN"> <code>broken</code></a>, closed,
		'''           or if an I/O error occurs. </exception>
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
			Dim ret As Integer = buffer(out) And &HFF
			out += 1
			If out >= buffer.Length Then out = 0
			If [in] = out Then [in] = -1

			Return ret

		''' <summary>
		''' Reads up to <code>len</code> bytes of data from this piped input
		''' stream into an array of bytes. Less than <code>len</code> bytes
		''' will be read if the end of the data stream is reached or if
		''' <code>len</code> exceeds the pipe's buffer size.
		''' If <code>len </code> is zero, then no bytes are read and 0 is returned;
		''' otherwise, the method blocks until at least 1 byte of input is
		''' available, end of the stream has been detected, or an exception is
		''' thrown.
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
		''' <exception cref="IOException"> if the pipe is <a href="#BROKEN"> <code>broken</code></a>,
		'''           <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>,
		'''           closed, or if an I/O error occurs. </exception>
		Public  Integer read(SByte b() , Integer off, Integer len) throws IOException
			If b Is Nothing Then
				Throw New NullPointerException
			ElseIf off < 0 OrElse len < 0 OrElse len > b.length - off Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			' possibly wait on the first character 
			Dim c As Integer = read()
			If c < 0 Then Return -1
			b(off) = CByte(c)
			Dim rlen As Integer = 1
			Do While ([in] >= 0) AndAlso (len > 1)

				Dim available As Integer

				If [in] > out Then
					available = System.Math.Min((buffer.Length - out), ([in] - out))
				Else
					available = buffer.Length - out
				End If

				' A byte is read beforehand outside the loop
				If available > (len - 1) Then available = len - 1
				Array.Copy(buffer, out, b, off + rlen, available)
				out += available
				rlen += available
				len -= available

				If out >= buffer.Length Then out = 0
				If [in] = out Then [in] = -1
			Loop
			Return rlen

		''' <summary>
		''' Returns the number of bytes that can be read from this input
		''' stream without blocking.
		''' </summary>
		''' <returns> the number of bytes that can be read from this input stream
		'''         without blocking, or {@code 0} if this input stream has been
		'''         closed by invoking its <seealso cref="#close()"/> method, or if the pipe
		'''         is <seealso cref="#connect(java.io.PipedOutputStream) unconnected"/>, or
		'''          <a href="#BROKEN"> <code>broken</code></a>.
		''' </returns>
		''' <exception cref="IOException">  if an I/O error occurs.
		''' @since   JDK1.0.2 </exception>
		Public  Integer available() throws IOException
			If [in] < 0 Then
				Return 0
			ElseIf [in] = out Then
				Return buffer.Length
			ElseIf [in] > out Then
				Return [in] - out
			Else
				Return [in] + buffer.Length - out
			End If

		''' <summary>
		''' Closes this piped input stream and releases any system resources
		''' associated with the stream.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public  Sub  close() throws IOException
			closedByReader = True
			SyncLock Me
				[in] = -1
			End SyncLock
	End Class

End Namespace