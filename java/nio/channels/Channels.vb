Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.channels



	''' <summary>
	''' Utility methods for channels and streams.
	''' 
	''' <p> This class defines static methods that support the interoperation of the
	''' stream classes of the <tt><seealso cref="java.io"/></tt> package with the channel
	''' classes of this package.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author Mike McCloskey
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public NotInheritable Class Channels

		Private Sub New() ' No instantiation
		End Sub

		Private Shared Sub checkNotNull(ByVal o As Object, ByVal name As String)
			If o Is Nothing Then Throw New NullPointerException("""" & name & """ is null!")
		End Sub

		''' <summary>
		''' Write all remaining bytes in buffer to the given channel.
		''' If the channel is selectable then it must be configured blocking.
		''' </summary>
		Private Shared Sub writeFullyImpl(ByVal ch As WritableByteChannel, ByVal bb As java.nio.ByteBuffer)
			Do While bb.remaining() > 0
				Dim n As Integer = ch.write(bb)
				If n <= 0 Then Throw New RuntimeException("no bytes written")
			Loop
		End Sub

		''' <summary>
		''' Write all remaining bytes in buffer to the given channel.
		''' </summary>
		''' <exception cref="IllegalBlockingModeException">
		'''          If the channel is selectable and configured non-blocking. </exception>
		Private Shared Sub writeFully(ByVal ch As WritableByteChannel, ByVal bb As java.nio.ByteBuffer)
			If TypeOf ch Is SelectableChannel Then
				Dim sc As SelectableChannel = CType(ch, SelectableChannel)
				SyncLock sc.blockingLock()
					If Not sc.blocking Then Throw New IllegalBlockingModeException
					writeFullyImpl(ch, bb)
				End SyncLock
			Else
				writeFullyImpl(ch, bb)
			End If
		End Sub

		' -- Byte streams from channels --

		''' <summary>
		''' Constructs a stream that reads bytes from the given channel.
		''' 
		''' <p> The <tt>read</tt> methods of the resulting stream will throw an
		''' <seealso cref="IllegalBlockingModeException"/> if invoked while the underlying
		''' channel is in non-blocking mode.  The stream will not be buffered, and
		''' it will not support the <seealso cref="InputStream#mark mark"/> or {@link
		''' InputStream#reset reset} methods.  The stream will be safe for access by
		''' multiple concurrent threads.  Closing the stream will in turn cause the
		''' channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel from which bytes will be read
		''' </param>
		''' <returns>  A new input stream </returns>
		Public Shared Function newInputStream(ByVal ch As ReadableByteChannel) As java.io.InputStream
			checkNotNull(ch, "ch")
			Return New sun.nio.ch.ChannelInputStream(ch)
		End Function

		''' <summary>
		''' Constructs a stream that writes bytes to the given channel.
		''' 
		''' <p> The <tt>write</tt> methods of the resulting stream will throw an
		''' <seealso cref="IllegalBlockingModeException"/> if invoked while the underlying
		''' channel is in non-blocking mode.  The stream will not be buffered.  The
		''' stream will be safe for access by multiple concurrent threads.  Closing
		''' the stream will in turn cause the channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel to which bytes will be written
		''' </param>
		''' <returns>  A new output stream </returns>
		Public Shared Function newOutputStream(ByVal ch As WritableByteChannel) As java.io.OutputStream
			checkNotNull(ch, "ch")

			Return New OutputStreamAnonymousInnerClassHelper
		End Function

		Private Class OutputStreamAnonymousInnerClassHelper
			Inherits java.io.OutputStream

			Private bb As java.nio.ByteBuffer = Nothing
			Private bs As SByte() = Nothing ' Invoker's previous array
			Private b1 As SByte() = Nothing

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Sub write(ByVal b As Integer)
			   If b1 Is Nothing Then b1 = New SByte(0){}
				b1(0) = CByte(b)
				Me.write(b1)
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Sub write(ByVal bs As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				If ([off] < 0) OrElse ([off] > bs.Length) OrElse (len < 0) OrElse (([off] + len) > bs.Length) OrElse (([off] + len) < 0) Then
					Throw New IndexOutOfBoundsException
				ElseIf len = 0 Then
					Return
				End If
				Dim bb As java.nio.ByteBuffer = (If(Me.bs = bs, Me.bb, java.nio.ByteBuffer.wrap(bs)))
				bb.limit (System.Math.Min([off] + len, bb.capacity()))
				bb.position([off])
				Me.bb = bb
				Me.bs = bs
				Channels.writeFully(ch, bb)
			End Sub

			Public Overrides Sub close()
				ch.close()
			End Sub

		End Class

		''' <summary>
		''' Constructs a stream that reads bytes from the given channel.
		''' 
		''' <p> The stream will not be buffered, and it will not support the {@link
		''' InputStream#mark mark} or <seealso cref="InputStream#reset reset"/> methods.  The
		''' stream will be safe for access by multiple concurrent threads.  Closing
		''' the stream will in turn cause the channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel from which bytes will be read
		''' </param>
		''' <returns>  A new input stream
		''' 
		''' @since 1.7 </returns>
		Public Shared Function newInputStream(ByVal ch As AsynchronousByteChannel) As java.io.InputStream
			checkNotNull(ch, "ch")
			Return New InputStreamAnonymousInnerClassHelper
		End Function

		Private Class InputStreamAnonymousInnerClassHelper
			Inherits java.io.InputStream

			Private bb As java.nio.ByteBuffer = Nothing
			Private bs As SByte() = Nothing ' Invoker's previous array
			Private b1 As SByte() = Nothing

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Function read() As Integer
				If b1 Is Nothing Then b1 = New SByte(0){}
				Dim n As Integer = Me.read(b1)
				If n = 1 Then Return b1(0) And &Hff
				Return -1
			End Function

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Function read(ByVal bs As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
				If ([off] < 0) OrElse ([off] > bs.Length) OrElse (len < 0) OrElse (([off] + len) > bs.Length) OrElse (([off] + len) < 0) Then
					Throw New IndexOutOfBoundsException
				ElseIf len = 0 Then
					Return 0
				End If

				Dim bb As java.nio.ByteBuffer = (If(Me.bs = bs, Me.bb, java.nio.ByteBuffer.wrap(bs)))
				bb.position([off])
				bb.limit (System.Math.Min([off] + len, bb.capacity()))
				Me.bb = bb
				Me.bs = bs

				Dim interrupted As Boolean = False
				Try
					Do
						Try
							Return ch.read(bb).get()
						Catch ee As java.util.concurrent.ExecutionException
							Throw New java.io.IOException(ee.InnerException)
						Catch ie As InterruptedException
							interrupted = True
						End Try
					Loop
				Finally
					If interrupted Then Thread.CurrentThread.Interrupt()
				End Try
			End Function

			Public Overrides Sub close()
				ch.close()
			End Sub
		End Class

		''' <summary>
		''' Constructs a stream that writes bytes to the given channel.
		''' 
		''' <p> The stream will not be buffered. The stream will be safe for access
		''' by multiple concurrent threads.  Closing the stream will in turn cause
		''' the channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel to which bytes will be written
		''' </param>
		''' <returns>  A new output stream
		''' 
		''' @since 1.7 </returns>
		Public Shared Function newOutputStream(ByVal ch As AsynchronousByteChannel) As java.io.OutputStream
			checkNotNull(ch, "ch")
			Return New OutputStreamAnonymousInnerClassHelper2
		End Function

		Private Class OutputStreamAnonymousInnerClassHelper2
			Inherits java.io.OutputStream

			Private bb As java.nio.ByteBuffer = Nothing
			Private bs As SByte() = Nothing ' Invoker's previous array
			Private b1 As SByte() = Nothing

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Sub write(ByVal b As Integer)
			   If b1 Is Nothing Then b1 = New SByte(0){}
				b1(0) = CByte(b)
				Me.write(b1)
			End Sub

			<MethodImpl(MethodImplOptions.Synchronized)> _
			Public Overrides Sub write(ByVal bs As SByte(), ByVal [off] As Integer, ByVal len As Integer)
				If ([off] < 0) OrElse ([off] > bs.Length) OrElse (len < 0) OrElse (([off] + len) > bs.Length) OrElse (([off] + len) < 0) Then
					Throw New IndexOutOfBoundsException
				ElseIf len = 0 Then
					Return
				End If
				Dim bb As java.nio.ByteBuffer = (If(Me.bs = bs, Me.bb, java.nio.ByteBuffer.wrap(bs)))
				bb.limit (System.Math.Min([off] + len, bb.capacity()))
				bb.position([off])
				Me.bb = bb
				Me.bs = bs

				Dim interrupted As Boolean = False
				Try
					Do While bb.remaining() > 0
						Try
							ch.write(bb).get()
						Catch ee As java.util.concurrent.ExecutionException
							Throw New java.io.IOException(ee.InnerException)
						Catch ie As InterruptedException
							interrupted = True
						End Try
					Loop
				Finally
					If interrupted Then Thread.CurrentThread.Interrupt()
				End Try
			End Sub

			Public Overrides Sub close()
				ch.close()
			End Sub
		End Class


		' -- Channels from streams --

		''' <summary>
		''' Constructs a channel that reads bytes from the given stream.
		''' 
		''' <p> The resulting channel will not be buffered; it will simply redirect
		''' its I/O operations to the given stream.  Closing the channel will in
		''' turn cause the stream to be closed.  </p>
		''' </summary>
		''' <param name="in">
		'''         The stream from which bytes are to be read
		''' </param>
		''' <returns>  A new readable byte channel </returns>
		Public Shared Function newChannel(ByVal [in] As java.io.InputStream) As ReadableByteChannel
			checkNotNull([in], "in")

			If TypeOf [in] Is java.io.FileInputStream AndAlso GetType(java.io.FileInputStream).Equals([in].GetType()) Then Return CType([in], java.io.FileInputStream).channel

			Return New ReadableByteChannelImpl([in])
		End Function

		Private Class ReadableByteChannelImpl
			Inherits java.nio.channels.spi.AbstractInterruptibleChannel
			Implements ReadableByteChannel
 ' Not really interruptible
			Friend [in] As java.io.InputStream
			Private Const TRANSFER_SIZE As Integer = 8192
			Private buf As SByte() = New SByte(){}
			Private open As Boolean = True
			Private readLock As New Object

			Friend Sub New(ByVal [in] As java.io.InputStream)
				Me.in = [in]
			End Sub

			Public Overridable Function read(ByVal dst As java.nio.ByteBuffer) As Integer Implements ReadableByteChannel.read
				Dim len As Integer = dst.remaining()
				Dim totalRead As Integer = 0
				Dim bytesRead As Integer = 0
				SyncLock readLock
					Do While totalRead < len
						Dim bytesToRead As Integer = System.Math.Min((len - totalRead), TRANSFER_SIZE)
						If buf.Length < bytesToRead Then buf = New SByte(bytesToRead - 1){}
						If (totalRead > 0) AndAlso Not([in].available() > 0) Then Exit Do ' block at most once
						Try
							begin()
							bytesRead = [in].read(buf, 0, bytesToRead)
						Finally
							[end](bytesRead > 0)
						End Try
						If bytesRead < 0 Then
							Exit Do
						Else
							totalRead += bytesRead
						End If
						dst.put(buf, 0, bytesRead)
					Loop
					If (bytesRead < 0) AndAlso (totalRead = 0) Then Return -1

					Return totalRead
				End SyncLock
			End Function

			Protected Friend Overrides Sub implCloseChannel()
				[in].close()
				open = False
			End Sub
		End Class


		''' <summary>
		''' Constructs a channel that writes bytes to the given stream.
		''' 
		''' <p> The resulting channel will not be buffered; it will simply redirect
		''' its I/O operations to the given stream.  Closing the channel will in
		''' turn cause the stream to be closed.  </p>
		''' </summary>
		''' <param name="out">
		'''         The stream to which bytes are to be written
		''' </param>
		''' <returns>  A new writable byte channel </returns>
		Public Shared Function newChannel(ByVal out As java.io.OutputStream) As WritableByteChannel
			checkNotNull(out, "out")

			If TypeOf out Is java.io.FileOutputStream AndAlso GetType(java.io.FileOutputStream).Equals(out.GetType()) Then Return CType(out, java.io.FileOutputStream).channel

			Return New WritableByteChannelImpl(out)
		End Function

		Private Class WritableByteChannelImpl
			Inherits java.nio.channels.spi.AbstractInterruptibleChannel
			Implements WritableByteChannel
 ' Not really interruptible
			Friend out As java.io.OutputStream
			Private Const TRANSFER_SIZE As Integer = 8192
			Private buf As SByte() = New SByte(){}
			Private open As Boolean = True
			Private writeLock As New Object

			Friend Sub New(ByVal out As java.io.OutputStream)
				Me.out = out
			End Sub

			Public Overridable Function write(ByVal src As java.nio.ByteBuffer) As Integer Implements WritableByteChannel.write
				Dim len As Integer = src.remaining()
				Dim totalWritten As Integer = 0
				SyncLock writeLock
					Do While totalWritten < len
						Dim bytesToWrite As Integer = System.Math.Min((len - totalWritten), TRANSFER_SIZE)
						If buf.Length < bytesToWrite Then buf = New SByte(bytesToWrite - 1){}
						src.get(buf, 0, bytesToWrite)
						Try
							begin()
							out.write(buf, 0, bytesToWrite)
						Finally
							[end](bytesToWrite > 0)
						End Try
						totalWritten += bytesToWrite
					Loop
					Return totalWritten
				End SyncLock
			End Function

			Protected Friend Overrides Sub implCloseChannel()
				out.close()
				open = False
			End Sub
		End Class


		' -- Character streams from channels --

		''' <summary>
		''' Constructs a reader that decodes bytes from the given channel using the
		''' given decoder.
		''' 
		''' <p> The resulting stream will contain an internal input buffer of at
		''' least <tt>minBufferCap</tt> bytes.  The stream's <tt>read</tt> methods
		''' will, as needed, fill the buffer by reading bytes from the underlying
		''' channel; if the channel is in non-blocking mode when bytes are to be
		''' read then an <seealso cref="IllegalBlockingModeException"/> will be thrown.  The
		''' resulting stream will not otherwise be buffered, and it will not support
		''' the <seealso cref="Reader#mark mark"/> or <seealso cref="Reader#reset reset"/> methods.
		''' Closing the stream will in turn cause the channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel from which bytes will be read
		''' </param>
		''' <param name="dec">
		'''         The charset decoder to be used
		''' </param>
		''' <param name="minBufferCap">
		'''         The minimum capacity of the internal byte buffer,
		'''         or <tt>-1</tt> if an implementation-dependent
		'''         default capacity is to be used
		''' </param>
		''' <returns>  A new reader </returns>
		Public Shared Function newReader(ByVal ch As ReadableByteChannel, ByVal dec As java.nio.charset.CharsetDecoder, ByVal minBufferCap As Integer) As java.io.Reader
			checkNotNull(ch, "ch")
			Return sun.nio.cs.StreamDecoder.forDecoder(ch, dec.reset(), minBufferCap)
		End Function

		''' <summary>
		''' Constructs a reader that decodes bytes from the given channel according
		''' to the named charset.
		''' 
		''' <p> An invocation of this method of the form
		''' 
		''' <blockquote><pre>
		''' Channels.newReader(ch, csname)</pre></blockquote>
		''' 
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>
		''' Channels.newReader(ch,
		'''                    Charset.forName(csName)
		'''                        .newDecoder(),
		'''                    -1);</pre></blockquote>
		''' </summary>
		''' <param name="ch">
		'''         The channel from which bytes will be read
		''' </param>
		''' <param name="csName">
		'''         The name of the charset to be used
		''' </param>
		''' <returns>  A new reader
		''' </returns>
		''' <exception cref="UnsupportedCharsetException">
		'''          If no support for the named charset is available
		'''          in this instance of the Java virtual machine </exception>
		Public Shared Function newReader(ByVal ch As ReadableByteChannel, ByVal csName As String) As java.io.Reader
			checkNotNull(csName, "csName")
			Return newReader(ch, java.nio.charset.Charset.forName(csName).newDecoder(), -1)
		End Function

		''' <summary>
		''' Constructs a writer that encodes characters using the given encoder and
		''' writes the resulting bytes to the given channel.
		''' 
		''' <p> The resulting stream will contain an internal output buffer of at
		''' least <tt>minBufferCap</tt> bytes.  The stream's <tt>write</tt> methods
		''' will, as needed, flush the buffer by writing bytes to the underlying
		''' channel; if the channel is in non-blocking mode when bytes are to be
		''' written then an <seealso cref="IllegalBlockingModeException"/> will be thrown.
		''' The resulting stream will not otherwise be buffered.  Closing the stream
		''' will in turn cause the channel to be closed.  </p>
		''' </summary>
		''' <param name="ch">
		'''         The channel to which bytes will be written
		''' </param>
		''' <param name="enc">
		'''         The charset encoder to be used
		''' </param>
		''' <param name="minBufferCap">
		'''         The minimum capacity of the internal byte buffer,
		'''         or <tt>-1</tt> if an implementation-dependent
		'''         default capacity is to be used
		''' </param>
		''' <returns>  A new writer </returns>
		Public Shared Function newWriter(ByVal ch As WritableByteChannel, ByVal enc As java.nio.charset.CharsetEncoder, ByVal minBufferCap As Integer) As java.io.Writer
			checkNotNull(ch, "ch")
			Return sun.nio.cs.StreamEncoder.forEncoder(ch, enc.reset(), minBufferCap)
		End Function

		''' <summary>
		''' Constructs a writer that encodes characters according to the named
		''' charset and writes the resulting bytes to the given channel.
		''' 
		''' <p> An invocation of this method of the form
		''' 
		''' <blockquote><pre>
		''' Channels.newWriter(ch, csname)</pre></blockquote>
		''' 
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>
		''' Channels.newWriter(ch,
		'''                    Charset.forName(csName)
		'''                        .newEncoder(),
		'''                    -1);</pre></blockquote>
		''' </summary>
		''' <param name="ch">
		'''         The channel to which bytes will be written
		''' </param>
		''' <param name="csName">
		'''         The name of the charset to be used
		''' </param>
		''' <returns>  A new writer
		''' </returns>
		''' <exception cref="UnsupportedCharsetException">
		'''          If no support for the named charset is available
		'''          in this instance of the Java virtual machine </exception>
		Public Shared Function newWriter(ByVal ch As WritableByteChannel, ByVal csName As String) As java.io.Writer
			checkNotNull(csName, "csName")
			Return newWriter(ch, java.nio.charset.Charset.forName(csName).newEncoder(), -1)
		End Function
	End Class

End Namespace