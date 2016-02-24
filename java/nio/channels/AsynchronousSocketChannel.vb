'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An asynchronous channel for stream-oriented connecting sockets.
	''' 
	''' <p> Asynchronous socket channels are created in one of two ways. A newly-created
	''' {@code AsynchronousSocketChannel} is created by invoking one of the {@link
	''' #open open} methods defined by this class. A newly-created channel is open but
	''' not yet connected. A connected {@code AsynchronousSocketChannel} is created
	''' when a connection is made to the socket of an <seealso cref="AsynchronousServerSocketChannel"/>.
	''' It is not possible to create an asynchronous socket channel for an arbitrary,
	''' pre-existing <seealso cref="java.net.Socket socket"/>.
	''' 
	''' <p> A newly-created channel is connected by invoking its <seealso cref="#connect connect"/>
	''' method; once connected, a channel remains connected until it is closed.  Whether
	''' or not a socket channel is connected may be determined by invoking its {@link
	''' #getRemoteAddress getRemoteAddress} method. An attempt to invoke an I/O
	''' operation upon an unconnected channel will cause a <seealso cref="NotYetConnectedException"/>
	''' to be thrown.
	''' 
	''' <p> Channels of this type are safe for use by multiple concurrent threads.
	''' They support concurrent reading and writing, though at most one read operation
	''' and one write operation can be outstanding at any time.
	''' If a thread initiates a read operation before a previous read operation has
	''' completed then a <seealso cref="ReadPendingException"/> will be thrown. Similarly, an
	''' attempt to initiate a write operation before a previous write has completed
	''' will throw a <seealso cref="WritePendingException"/>.
	''' 
	''' <p> Socket options are configured using the {@link #setOption(SocketOption,Object)
	''' setOption} method. Asynchronous socket channels support the following options:
	''' <blockquote>
	''' <table border summary="Socket options">
	'''   <tr>
	'''     <th>Option Name</th>
	'''     <th>Description</th>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_SNDBUF SO_SNDBUF"/> </td>
	'''     <td> The size of the socket send buffer </td>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_RCVBUF SO_RCVBUF"/> </td>
	'''     <td> The size of the socket receive buffer </td>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_KEEPALIVE SO_KEEPALIVE"/> </td>
	'''     <td> Keep connection alive </td>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> </td>
	'''     <td> Re-use address </td>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#TCP_NODELAY TCP_NODELAY"/> </td>
	'''     <td> Disable the Nagle algorithm </td>
	'''   </tr>
	''' </table>
	''' </blockquote>
	''' Additional (implementation specific) options may also be supported.
	''' 
	''' <h2>Timeouts</h2>
	''' 
	''' <p> The <seealso cref="#read(ByteBuffer,long,TimeUnit,Object,CompletionHandler) read"/>
	''' and <seealso cref="#write(ByteBuffer,long,TimeUnit,Object,CompletionHandler) write"/>
	''' methods defined by this class allow a timeout to be specified when initiating
	''' a read or write operation. If the timeout elapses before an operation completes
	''' then the operation completes with the exception {@link
	''' InterruptedByTimeoutException}. A timeout may leave the channel, or the
	''' underlying connection, in an inconsistent state. Where the implementation
	''' cannot guarantee that bytes have not been read from the channel then it puts
	''' the channel into an implementation specific <em>error state</em>. A subsequent
	''' attempt to initiate a {@code read} operation causes an unspecified runtime
	''' exception to be thrown. Similarly if a {@code write} operation times out and
	''' the implementation cannot guarantee bytes have not been written to the
	''' channel then further attempts to {@code write} to the channel cause an
	''' unspecified runtime exception to be thrown. When a timeout elapses then the
	''' state of the <seealso cref="ByteBuffer"/>, or the sequence of buffers, for the I/O
	''' operation is not defined. Buffers should be discarded or at least care must
	''' be taken to ensure that the buffers are not accessed while the channel remains
	''' open. All methods that accept timeout parameters treat values less than or
	''' equal to zero to mean that the I/O operation does not timeout.
	''' 
	''' @since 1.7
	''' </summary>

	Public MustInherit Class AsynchronousSocketChannel
		Implements AsynchronousByteChannel, NetworkChannel

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public MustOverride Function supportedOptions() As java.util.Set(Of java.net.SocketOption(Of ?)) Implements NetworkChannel.supportedOptions
			Public MustOverride Function getOption(ByVal name As java.net.SocketOption(Of T)) As T Implements NetworkChannel.getOption
			Public MustOverride ReadOnly Property open As Boolean Implements Channel.isOpen
			Public MustOverride Sub close() Implements AsynchronousChannel.close
		Private ReadOnly provider_Renamed As AsynchronousChannelProvider

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="provider">
		'''         The provider that created this channel </param>
		Protected Friend Sub New(ByVal provider As AsynchronousChannelProvider)
			Me.provider_Renamed = provider
		End Sub

		''' <summary>
		''' Returns the provider that created this channel.
		''' </summary>
		''' <returns>  The provider that created this channel </returns>
		Public Function provider() As AsynchronousChannelProvider
			Return provider_Renamed
		End Function

		''' <summary>
		''' Opens an asynchronous socket channel.
		''' 
		''' <p> The new channel is created by invoking the {@link
		''' AsynchronousChannelProvider#openAsynchronousSocketChannel
		''' openAsynchronousSocketChannel} method on the {@link
		''' AsynchronousChannelProvider} that created the group. If the group parameter
		''' is {@code null} then the resulting channel is created by the system-wide
		''' default provider, and bound to the <em>default group</em>.
		''' </summary>
		''' <param name="group">
		'''          The group to which the newly constructed channel should be bound,
		'''          or {@code null} for the default group
		''' </param>
		''' <returns>  A new asynchronous socket channel
		''' </returns>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group is shutdown </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public Shared Function open(ByVal group As AsynchronousChannelGroup) As AsynchronousSocketChannel
			Dim provider As AsynchronousChannelProvider = If(group Is Nothing, AsynchronousChannelProvider.provider(), group.provider())
			Return provider.openAsynchronousSocketChannel(group)
		End Function

		''' <summary>
		''' Opens an asynchronous socket channel.
		''' 
		''' <p> This method returns an asynchronous socket channel that is bound to
		''' the <em>default group</em>.This method is equivalent to evaluating the
		''' expression:
		''' <blockquote><pre>
		''' open((AsynchronousChannelGroup)null);
		''' </pre></blockquote>
		''' </summary>
		''' <returns>  A new asynchronous socket channel
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public Shared Function open() As AsynchronousSocketChannel
			Return open(Nothing)
		End Function


		' -- socket options and related --

		''' <exception cref="ConnectionPendingException">
		'''          If a connection operation is already in progress on this channel </exception>
		''' <exception cref="AlreadyBoundException">               {@inheritDoc} </exception>
		''' <exception cref="UnsupportedAddressTypeException">     {@inheritDoc} </exception>
		''' <exception cref="ClosedChannelException">              {@inheritDoc} </exception>
		''' <exception cref="IOException">                         {@inheritDoc} </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and its
		'''          <seealso cref="SecurityManager#checkListen checkListen"/> method denies
		'''          the operation </exception>
		Public MustOverride Overrides Function bind(ByVal local As java.net.SocketAddress) As AsynchronousSocketChannel

		''' <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		''' <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		''' <exception cref="IOException">                             {@inheritDoc} </exception>
		Public MustOverride Overrides Function setOption(Of T)(ByVal name As java.net.SocketOption(Of T), ByVal value As T) As AsynchronousSocketChannel

		''' <summary>
		''' Shutdown the connection for reading without closing the channel.
		''' 
		''' <p> Once shutdown for reading then further reads on the channel will
		''' return {@code -1}, the end-of-stream indication. If the input side of the
		''' connection is already shutdown then invoking this method has no effect.
		''' The effect on an outstanding read operation is system dependent and
		''' therefore not specified. The effect, if any, when there is data in the
		''' socket receive buffer that has not been read, or data arrives subsequently,
		''' is also system dependent.
		''' </summary>
		''' <returns>  The channel
		''' </returns>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Function shutdownInput() As AsynchronousSocketChannel

		''' <summary>
		''' Shutdown the connection for writing without closing the channel.
		''' 
		''' <p> Once shutdown for writing then further attempts to write to the
		''' channel will throw <seealso cref="ClosedChannelException"/>. If the output side of
		''' the connection is already shutdown then invoking this method has no
		''' effect. The effect on an outstanding write operation is system dependent
		''' and therefore not specified.
		''' </summary>
		''' <returns>  The channel
		''' </returns>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Function shutdownOutput() As AsynchronousSocketChannel

		' -- state --

		''' <summary>
		''' Returns the remote address to which this channel's socket is connected.
		''' 
		''' <p> Where the channel is bound and connected to an Internet Protocol
		''' socket address then the return value from this method is of type {@link
		''' java.net.InetSocketAddress}.
		''' </summary>
		''' <returns>  The remote address; {@code null} if the channel's socket is not
		'''          connected
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If the channel is closed </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride ReadOnly Property remoteAddress As java.net.SocketAddress

		' -- asynchronous operations --

		''' <summary>
		''' Connects this channel.
		''' 
		''' <p> This method initiates an operation to connect this channel. The
		''' {@code handler} parameter is a completion handler that is invoked when
		''' the connection is successfully established or connection cannot be
		''' established. If the connection cannot be established then the channel is
		''' closed.
		''' 
		''' <p> This method performs exactly the same security checks as the {@link
		''' java.net.Socket} class.  That is, if a security manager has been
		''' installed then this method verifies that its {@link
		''' java.lang.SecurityManager#checkConnect checkConnect} method permits
		''' connecting to the address and port number of the given remote endpoint.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="remote">
		'''          The remote address to which this channel is to be connected </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="UnresolvedAddressException">
		'''          If the given remote address is not fully resolved </exception>
		''' <exception cref="UnsupportedAddressTypeException">
		'''          If the type of the given remote address is not supported </exception>
		''' <exception cref="AlreadyConnectedException">
		'''          If this channel is already connected </exception>
		''' <exception cref="ConnectionPendingException">
		'''          If a connection operation is already in progress on this channel </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed
		'''          and it does not permit access to the given remote endpoint
		''' </exception>
		''' <seealso cref= #getRemoteAddress </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub connect(Of A, T1)(ByVal remote As java.net.SocketAddress, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' Connects this channel.
		''' 
		''' <p> This method initiates an operation to connect this channel. This
		''' method behaves in exactly the same manner as the {@link
		''' #connect(SocketAddress, Object, CompletionHandler)} method except that
		''' instead of specifying a completion handler, this method returns a {@code
		''' Future} representing the pending result. The {@code Future}'s {@link
		''' Future#get() get} method returns {@code null} on successful completion.
		''' </summary>
		''' <param name="remote">
		'''          The remote address to which this channel is to be connected
		''' </param>
		''' <returns>  A {@code Future} object representing the pending result
		''' </returns>
		''' <exception cref="UnresolvedAddressException">
		'''          If the given remote address is not fully resolved </exception>
		''' <exception cref="UnsupportedAddressTypeException">
		'''          If the type of the given remote address is not supported </exception>
		''' <exception cref="AlreadyConnectedException">
		'''          If this channel is already connected </exception>
		''' <exception cref="ConnectionPendingException">
		'''          If a connection operation is already in progress on this channel </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed
		'''          and it does not permit access to the given remote endpoint </exception>
		Public MustOverride Function connect(ByVal remote As java.net.SocketAddress) As java.util.concurrent.Future(Of Void)

		''' <summary>
		''' Reads a sequence of bytes from this channel into the given buffer.
		''' 
		''' <p> This method initiates an asynchronous read operation to read a
		''' sequence of bytes from this channel into the given buffer. The {@code
		''' handler} parameter is a completion handler that is invoked when the read
		''' operation completes (or fails). The result passed to the completion
		''' handler is the number of bytes read or {@code -1} if no bytes could be
		''' read because the channel has reached end-of-stream.
		''' 
		''' <p> If a timeout is specified and the timeout elapses before the operation
		''' completes then the operation completes with the exception {@link
		''' InterruptedByTimeoutException}. Where a timeout occurs, and the
		''' implementation cannot guarantee that bytes have not been read, or will not
		''' be read from the channel into the given buffer, then further attempts to
		''' read from the channel will cause an unspecific runtime exception to be
		''' thrown.
		''' 
		''' <p> Otherwise this method works in the same manner as the {@link
		''' AsynchronousByteChannel#read(ByteBuffer,Object,CompletionHandler)}
		''' method.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="dst">
		'''          The buffer into which bytes are to be transferred </param>
		''' <param name="timeout">
		'''          The maximum time for the I/O operation to complete </param>
		''' <param name="unit">
		'''          The time unit of the {@code timeout} argument </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          If the buffer is read-only </exception>
		''' <exception cref="ReadPendingException">
		'''          If a read operation is already in progress on this channel </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub read(Of A, T1)(ByVal dst As java.nio.ByteBuffer, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <exception cref="IllegalArgumentException">        {@inheritDoc} </exception>
		''' <exception cref="ReadPendingException">            {@inheritDoc} </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub read(Of A, T1)(ByVal dst As java.nio.ByteBuffer, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1)) Implements AsynchronousByteChannel.read
			read(dst, 0L, java.util.concurrent.TimeUnit.MILLISECONDS, attachment, handler)
		End Sub

		''' <exception cref="IllegalArgumentException">        {@inheritDoc} </exception>
		''' <exception cref="ReadPendingException">            {@inheritDoc} </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		Public MustOverride Overrides Function read(ByVal dst As java.nio.ByteBuffer) As java.util.concurrent.Future(Of Integer?) Implements AsynchronousByteChannel.read

		''' <summary>
		''' Reads a sequence of bytes from this channel into a subsequence of the
		''' given buffers. This operation, sometimes called a <em>scattering read</em>,
		''' is often useful when implementing network protocols that group data into
		''' segments consisting of one or more fixed-length headers followed by a
		''' variable-length body. The {@code handler} parameter is a completion
		''' handler that is invoked when the read operation completes (or fails). The
		''' result passed to the completion handler is the number of bytes read or
		''' {@code -1} if no bytes could be read because the channel has reached
		''' end-of-stream.
		''' 
		''' <p> This method initiates a read of up to <i>r</i> bytes from this channel,
		''' where <i>r</i> is the total number of bytes remaining in the specified
		''' subsequence of the given buffer array, that is,
		''' 
		''' <blockquote><pre>
		''' dsts[offset].remaining()
		'''     + dsts[offset+1].remaining()
		'''     + ... + dsts[offset+length-1].remaining()</pre></blockquote>
		''' 
		''' at the moment that the read is attempted.
		''' 
		''' <p> Suppose that a byte sequence of length <i>n</i> is read, where
		''' <tt>0</tt>&nbsp;<tt>&lt;</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		''' Up to the first <tt>dsts[offset].remaining()</tt> bytes of this sequence
		''' are transferred into buffer <tt>dsts[offset]</tt>, up to the next
		''' <tt>dsts[offset+1].remaining()</tt> bytes are transferred into buffer
		''' <tt>dsts[offset+1]</tt>, and so forth, until the entire byte sequence
		''' is transferred into the given buffers.  As many bytes as possible are
		''' transferred into each buffer, hence the final position of each updated
		''' buffer, except the last updated buffer, is guaranteed to be equal to
		''' that buffer's limit. The underlying operating system may impose a limit
		''' on the number of buffers that may be used in an I/O operation. Where the
		''' number of buffers (with bytes remaining), exceeds this limit, then the
		''' I/O operation is performed with the maximum number of buffers allowed by
		''' the operating system.
		''' 
		''' <p> If a timeout is specified and the timeout elapses before the operation
		''' completes then it completes with the exception {@link
		''' InterruptedByTimeoutException}. Where a timeout occurs, and the
		''' implementation cannot guarantee that bytes have not been read, or will not
		''' be read from the channel into the given buffers, then further attempts to
		''' read from the channel will cause an unspecific runtime exception to be
		''' thrown.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="dsts">
		'''          The buffers into which bytes are to be transferred </param>
		''' <param name="offset">
		'''          The offset within the buffer array of the first buffer into which
		'''          bytes are to be transferred; must be non-negative and no larger than
		'''          {@code dsts.length} </param>
		''' <param name="length">
		'''          The maximum number of buffers to be accessed; must be non-negative
		'''          and no larger than {@code dsts.length - offset} </param>
		''' <param name="timeout">
		'''          The maximum time for the I/O operation to complete </param>
		''' <param name="unit">
		'''          The time unit of the {@code timeout} argument </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If the pre-conditions for the {@code offset}  and {@code length}
		'''          parameter aren't met </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If the buffer is read-only </exception>
		''' <exception cref="ReadPendingException">
		'''          If a read operation is already in progress on this channel </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub read(Of A, T1)(ByVal dsts As java.nio.ByteBuffer(), ByVal offset As Integer, ByVal length As Integer, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' Writes a sequence of bytes to this channel from the given buffer.
		''' 
		''' <p> This method initiates an asynchronous write operation to write a
		''' sequence of bytes to this channel from the given buffer. The {@code
		''' handler} parameter is a completion handler that is invoked when the write
		''' operation completes (or fails). The result passed to the completion
		''' handler is the number of bytes written.
		''' 
		''' <p> If a timeout is specified and the timeout elapses before the operation
		''' completes then it completes with the exception {@link
		''' InterruptedByTimeoutException}. Where a timeout occurs, and the
		''' implementation cannot guarantee that bytes have not been written, or will
		''' not be written to the channel from the given buffer, then further attempts
		''' to write to the channel will cause an unspecific runtime exception to be
		''' thrown.
		''' 
		''' <p> Otherwise this method works in the same manner as the {@link
		''' AsynchronousByteChannel#write(ByteBuffer,Object,CompletionHandler)}
		''' method.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="src">
		'''          The buffer from which bytes are to be retrieved </param>
		''' <param name="timeout">
		'''          The maximum time for the I/O operation to complete </param>
		''' <param name="unit">
		'''          The time unit of the {@code timeout} argument </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="WritePendingException">
		'''          If a write operation is already in progress on this channel </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub write(Of A, T1)(ByVal src As java.nio.ByteBuffer, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <exception cref="WritePendingException">          {@inheritDoc} </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub write(Of A, T1)(ByVal src As java.nio.ByteBuffer, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1)) Implements AsynchronousByteChannel.write

			write(src, 0L, java.util.concurrent.TimeUnit.MILLISECONDS, attachment, handler)
		End Sub

		''' <exception cref="WritePendingException">       {@inheritDoc} </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		Public MustOverride Overrides Function write(ByVal src As java.nio.ByteBuffer) As java.util.concurrent.Future(Of Integer?) Implements AsynchronousByteChannel.write

		''' <summary>
		''' Writes a sequence of bytes to this channel from a subsequence of the given
		''' buffers. This operation, sometimes called a <em>gathering write</em>, is
		''' often useful when implementing network protocols that group data into
		''' segments consisting of one or more fixed-length headers followed by a
		''' variable-length body. The {@code handler} parameter is a completion
		''' handler that is invoked when the write operation completes (or fails).
		''' The result passed to the completion handler is the number of bytes written.
		''' 
		''' <p> This method initiates a write of up to <i>r</i> bytes to this channel,
		''' where <i>r</i> is the total number of bytes remaining in the specified
		''' subsequence of the given buffer array, that is,
		''' 
		''' <blockquote><pre>
		''' srcs[offset].remaining()
		'''     + srcs[offset+1].remaining()
		'''     + ... + srcs[offset+length-1].remaining()</pre></blockquote>
		''' 
		''' at the moment that the write is attempted.
		''' 
		''' <p> Suppose that a byte sequence of length <i>n</i> is written, where
		''' <tt>0</tt>&nbsp;<tt>&lt;</tt>&nbsp;<i>n</i>&nbsp;<tt>&lt;=</tt>&nbsp;<i>r</i>.
		''' Up to the first <tt>srcs[offset].remaining()</tt> bytes of this sequence
		''' are written from buffer <tt>srcs[offset]</tt>, up to the next
		''' <tt>srcs[offset+1].remaining()</tt> bytes are written from buffer
		''' <tt>srcs[offset+1]</tt>, and so forth, until the entire byte sequence is
		''' written.  As many bytes as possible are written from each buffer, hence
		''' the final position of each updated buffer, except the last updated
		''' buffer, is guaranteed to be equal to that buffer's limit. The underlying
		''' operating system may impose a limit on the number of buffers that may be
		''' used in an I/O operation. Where the number of buffers (with bytes
		''' remaining), exceeds this limit, then the I/O operation is performed with
		''' the maximum number of buffers allowed by the operating system.
		''' 
		''' <p> If a timeout is specified and the timeout elapses before the operation
		''' completes then it completes with the exception {@link
		''' InterruptedByTimeoutException}. Where a timeout occurs, and the
		''' implementation cannot guarantee that bytes have not been written, or will
		''' not be written to the channel from the given buffers, then further attempts
		''' to write to the channel will cause an unspecific runtime exception to be
		''' thrown.
		''' </summary>
		''' @param   <A>
		'''          The type of the attachment </param>
		''' <param name="srcs">
		'''          The buffers from which bytes are to be retrieved </param>
		''' <param name="offset">
		'''          The offset within the buffer array of the first buffer from which
		'''          bytes are to be retrieved; must be non-negative and no larger
		'''          than {@code srcs.length} </param>
		''' <param name="length">
		'''          The maximum number of buffers to be accessed; must be non-negative
		'''          and no larger than {@code srcs.length - offset} </param>
		''' <param name="timeout">
		'''          The maximum time for the I/O operation to complete </param>
		''' <param name="unit">
		'''          The time unit of the {@code timeout} argument </param>
		''' <param name="attachment">
		'''          The object to attach to the I/O operation; can be {@code null} </param>
		''' <param name="handler">
		'''          The handler for consuming the result
		''' </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If the pre-conditions for the {@code offset}  and {@code length}
		'''          parameter aren't met </exception>
		''' <exception cref="WritePendingException">
		'''          If a write operation is already in progress on this channel </exception>
		''' <exception cref="NotYetConnectedException">
		'''          If this channel is not yet connected </exception>
		''' <exception cref="ShutdownChannelGroupException">
		'''          If the channel group has terminated </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public MustOverride Sub write(Of A, T1)(ByVal srcs As java.nio.ByteBuffer(), ByVal offset As Integer, ByVal length As Integer, ByVal timeout As Long, ByVal unit As java.util.concurrent.TimeUnit, ByVal attachment As A, ByVal handler As CompletionHandler(Of T1))

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' a {@code SocketAddress} representing the
		''' <seealso cref="java.net.InetAddress#getLoopbackAddress loopback"/> address and the
		''' local port of the channel's socket is returned.
		''' </summary>
		''' <returns>  The {@code SocketAddress} that the socket is bound to, or the
		'''          {@code SocketAddress} representing the loopback address if
		'''          denied by the security manager, or {@code null} if the
		'''          channel's socket is not bound
		''' </returns>
		''' <exception cref="ClosedChannelException">     {@inheritDoc} </exception>
		''' <exception cref="IOException">                {@inheritDoc} </exception>
		Public MustOverride ReadOnly Property localAddress As java.net.SocketAddress Implements NetworkChannel.getLocalAddress
	End Class

End Namespace