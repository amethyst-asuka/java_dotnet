Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Piped character-output streams.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class PipedWriter
		Inherits Writer

	'     REMIND: identification of the read and write sides needs to be
	'       more sophisticated.  Either using thread groups (but what about
	'       pipes within a thread?) or using finalization (but it may be a
	'       long time until the next GC). 
		Private sink As PipedReader

	'     This flag records the open status of this particular writer. It
	'     * is independent of the status flags defined in PipedReader. It is
	'     * used to do a sanity check on connect.
	'     
		Private closed As Boolean = False

		''' <summary>
		''' Creates a piped writer connected to the specified piped
		''' reader. Data characters written to this stream will then be
		''' available as input from <code>snk</code>.
		''' </summary>
		''' <param name="snk">   The piped reader to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Sub New(ByVal snk As PipedReader)
			connect(snk)
		End Sub

		''' <summary>
		''' Creates a piped writer that is not yet connected to a
		''' piped reader. It must be connected to a piped reader,
		''' either by the receiver or the sender, before being used.
		''' </summary>
		''' <seealso cref=     java.io.PipedReader#connect(java.io.PipedWriter) </seealso>
		''' <seealso cref=     java.io.PipedWriter#connect(java.io.PipedReader) </seealso>
		Public Sub New()
		End Sub

		''' <summary>
		''' Connects this piped writer to a receiver. If this object
		''' is already connected to some other piped reader, an
		''' <code>IOException</code> is thrown.
		''' <p>
		''' If <code>snk</code> is an unconnected piped reader and
		''' <code>src</code> is an unconnected piped writer, they may
		''' be connected by either the call:
		''' <blockquote><pre>
		''' src.connect(snk)</pre></blockquote>
		''' or the call:
		''' <blockquote><pre>
		''' snk.connect(src)</pre></blockquote>
		''' The two calls have the same effect.
		''' </summary>
		''' <param name="snk">   the piped reader to connect to. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub connect(ByVal snk As PipedReader)
			If snk Is Nothing Then
				Throw New NullPointerException
			ElseIf sink IsNot Nothing OrElse snk.connected Then
				Throw New IOException("Already connected")
			ElseIf snk.closedByReader OrElse closed Then
				Throw New IOException("Pipe closed")
			End If

			sink = snk
			snk.in = -1
			snk.out = 0
			snk.connected = True
		End Sub

		''' <summary>
		''' Writes the specified <code>char</code> to the piped output stream.
		''' If a thread was reading data characters from the connected piped input
		''' stream, but the thread is no longer alive, then an
		''' <code>IOException</code> is thrown.
		''' <p>
		''' Implements the <code>write</code> method of <code>Writer</code>.
		''' </summary>
		''' <param name="c">   the <code>char</code> to be written. </param>
		''' <exception cref="IOException">  if the pipe is
		'''          <a href=PipedOutputStream.html#BROKEN> <code>broken</code></a>,
		'''          <seealso cref="#connect(java.io.PipedReader) unconnected"/>, closed
		'''          or an I/O error occurs. </exception>
		Public Overrides Sub write(ByVal c As Integer)
			If sink Is Nothing Then Throw New IOException("Pipe not connected")
			sink.receive(c)
		End Sub

		''' <summary>
		''' Writes <code>len</code> characters from the specified character array
		''' starting at offset <code>off</code> to this piped output stream.
		''' This method blocks until all the characters are written to the output
		''' stream.
		''' If a thread was reading data characters from the connected piped input
		''' stream, but the thread is no longer alive, then an
		''' <code>IOException</code> is thrown.
		''' </summary>
		''' <param name="cbuf">  the data. </param>
		''' <param name="off">   the start offset in the data. </param>
		''' <param name="len">   the number of characters to write. </param>
		''' <exception cref="IOException">  if the pipe is
		'''          <a href=PipedOutputStream.html#BROKEN> <code>broken</code></a>,
		'''          <seealso cref="#connect(java.io.PipedReader) unconnected"/>, closed
		'''          or an I/O error occurs. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(char cbuf() , int off, int len) throws IOException
			If sink Is Nothing Then
				Throw New IOException("Pipe not connected")
			ElseIf (off Or len Or (off + len) Or (cbuf.length - (off + len))) < 0 Then
				Throw New IndexOutOfBoundsException
			End If
			sink.receive(cbuf, off, len)

		''' <summary>
		''' Flushes this output stream and forces any buffered output characters
		''' to be written out.
		''' This will notify any readers that characters are waiting in the pipe.
		''' </summary>
		''' <exception cref="IOException">  if the pipe is closed, or an I/O error occurs. </exception>
		public synchronized  Sub  flush() throws IOException
			If sink IsNot Nothing Then
				If sink.closedByReader OrElse closed Then Throw New IOException("Pipe closed")
				SyncLock sink
					sink.notifyAll()
				End SyncLock
			End If

		''' <summary>
		''' Closes this piped output stream and releases any system resources
		''' associated with this stream. This stream may no longer be used for
		''' writing characters.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public  Sub  close() throws IOException
			closed = True
			If sink IsNot Nothing Then sink.receivedLast()
	End Class

End Namespace