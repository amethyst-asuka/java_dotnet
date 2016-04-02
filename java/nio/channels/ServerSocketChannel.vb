'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A selectable channel for stream-oriented listening sockets.
	''' 
	''' <p> A server-socket channel is created by invoking the <seealso cref="#open() open"/>
	''' method of this class.  It is not possible to create a channel for an arbitrary,
	''' pre-existing <seealso cref="ServerSocket"/>. A newly-created server-socket channel is
	''' open but not yet bound.  An attempt to invoke the <seealso cref="#accept() accept"/>
	''' method of an unbound server-socket channel will cause a <seealso cref="NotYetBoundException"/>
	''' to be thrown. A server-socket channel can be bound by invoking one of the
	''' <seealso cref="#bind(java.net.SocketAddress,int) bind"/> methods defined by this class.
	''' 
	''' <p> Socket options are configured using the {@link #setOption(SocketOption,Object)
	''' setOption} method. Server-socket channels support the following options:
	''' <blockquote>
	''' <table border summary="Socket options">
	'''   <tr>
	'''     <th>Option Name</th>
	'''     <th>Description</th>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_RCVBUF SO_RCVBUF"/> </td>
	'''     <td> The size of the socket receive buffer </td>
	'''   </tr>
	'''   <tr>
	'''     <td> <seealso cref="java.net.StandardSocketOptions#SO_REUSEADDR SO_REUSEADDR"/> </td>
	'''     <td> Re-use address </td>
	'''   </tr>
	''' </table>
	''' </blockquote>
	''' Additional (implementation specific) options may also be supported.
	''' 
	''' <p> Server-socket channels are safe for use by multiple concurrent threads.
	''' </p>
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class ServerSocketChannel
		Inherits java.nio.channels.spi.AbstractSelectableChannel
		Implements NetworkChannel

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public MustOverride Function supportedOptions() As java.util.Set(Of java.net.SocketOption(Of ?)) Implements NetworkChannel.supportedOptions
			Public MustOverride Function getOption(ByVal name As java.net.SocketOption(Of T)) As T Implements NetworkChannel.getOption

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="provider">
		'''         The provider that created this channel </param>
		Protected Friend Sub New(ByVal provider As java.nio.channels.spi.SelectorProvider)
			MyBase.New(provider)
		End Sub

		''' <summary>
		''' Opens a server-socket channel.
		''' 
		''' <p> The new channel is created by invoking the {@link
		''' java.nio.channels.spi.SelectorProvider#openServerSocketChannel
		''' openServerSocketChannel} method of the system-wide default {@link
		''' java.nio.channels.spi.SelectorProvider} object.
		''' 
		''' <p> The new channel's socket is initially unbound; it must be bound to a
		''' specific address via one of its socket's {@link
		''' java.net.ServerSocket#bind(SocketAddress) bind} methods before
		''' connections can be accepted.  </p>
		''' </summary>
		''' <returns>  A new socket channel
		''' </returns>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public Shared Function open() As ServerSocketChannel
			Return java.nio.channels.spi.SelectorProvider.provider().openServerSocketChannel()
		End Function

		''' <summary>
		''' Returns an operation set identifying this channel's supported
		''' operations.
		''' 
		''' <p> Server-socket channels only support the accepting of new
		''' connections, so this method returns <seealso cref="SelectionKey#OP_ACCEPT"/>.
		''' </p>
		''' </summary>
		''' <returns>  The valid-operation set </returns>
		Public NotOverridable Overrides Function validOps() As Integer
			Return SelectionKey.OP_ACCEPT
		End Function


		' -- ServerSocket-specific operations --

		''' <summary>
		''' Binds the channel's socket to a local address and configures the socket
		''' to listen for connections.
		''' 
		''' <p> An invocation of this method is equivalent to the following:
		''' <blockquote><pre>
		''' bind(local, 0);
		''' </pre></blockquote>
		''' </summary>
		''' <param name="local">
		'''          The local address to bind the socket, or {@code null} to bind
		'''          to an automatically assigned socket address
		''' </param>
		''' <returns>  This channel
		''' </returns>
		''' <exception cref="AlreadyBoundException">               {@inheritDoc} </exception>
		''' <exception cref="UnsupportedAddressTypeException">     {@inheritDoc} </exception>
		''' <exception cref="ClosedChannelException">              {@inheritDoc} </exception>
		''' <exception cref="IOException">                         {@inheritDoc} </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and its {@link
		'''          SecurityManager#checkListen checkListen} method denies the
		'''          operation
		''' 
		''' @since 1.7 </exception>
		Public Function bind(ByVal local As java.net.SocketAddress) As ServerSocketChannel
			Return bind(local, 0)
		End Function

		''' <summary>
		''' Binds the channel's socket to a local address and configures the socket to
		''' listen for connections.
		''' 
		''' <p> This method is used to establish an association between the socket and
		''' a local address. Once an association is established then the socket remains
		''' bound until the channel is closed.
		''' 
		''' <p> The {@code backlog} parameter is the maximum number of pending
		''' connections on the socket. Its exact semantics are implementation specific.
		''' In particular, an implementation may impose a maximum length or may choose
		''' to ignore the parameter altogther. If the {@code backlog} parameter has
		''' the value {@code 0}, or a negative value, then an implementation specific
		''' default is used.
		''' </summary>
		''' <param name="local">
		'''          The address to bind the socket, or {@code null} to bind to an
		'''          automatically assigned socket address </param>
		''' <param name="backlog">
		'''          The maximum number of pending connections
		''' </param>
		''' <returns>  This channel
		''' </returns>
		''' <exception cref="AlreadyBoundException">
		'''          If the socket is already bound </exception>
		''' <exception cref="UnsupportedAddressTypeException">
		'''          If the type of the given address is not supported </exception>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and its {@link
		'''          SecurityManager#checkListen checkListen} method denies the
		'''          operation
		''' 
		''' @since 1.7 </exception>
		Public MustOverride Function bind(ByVal local As java.net.SocketAddress, ByVal backlog As Integer) As ServerSocketChannel

		''' <exception cref="UnsupportedOperationException">           {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException">                {@inheritDoc} </exception>
		''' <exception cref="ClosedChannelException">                  {@inheritDoc} </exception>
		''' <exception cref="IOException">                             {@inheritDoc}
		''' 
		''' @since 1.7 </exception>
		Public MustOverride Function setOption(Of T)(ByVal name As java.net.SocketOption(Of T), ByVal value As T) As ServerSocketChannel

		''' <summary>
		''' Retrieves a server socket associated with this channel.
		''' 
		''' <p> The returned object will not declare any public methods that are not
		''' declared in the <seealso cref="java.net.ServerSocket"/> class.  </p>
		''' </summary>
		''' <returns>  A server socket associated with this channel </returns>
		Public MustOverride Function socket() As java.net.ServerSocket

		''' <summary>
		''' Accepts a connection made to this channel's socket.
		''' 
		''' <p> If this channel is in non-blocking mode then this method will
		''' immediately return <tt>null</tt> if there are no pending connections.
		''' Otherwise it will block indefinitely until a new connection is available
		''' or an I/O error occurs.
		''' 
		''' <p> The socket channel returned by this method, if any, will be in
		''' blocking mode regardless of the blocking mode of this channel.
		''' 
		''' <p> This method performs exactly the same security checks as the {@link
		''' java.net.ServerSocket#accept accept} method of the {@link
		''' java.net.ServerSocket} class.  That is, if a security manager has been
		''' installed then for each new connection this method verifies that the
		''' address and port number of the connection's remote endpoint are
		''' permitted by the security manager's {@link
		''' java.lang.SecurityManager#checkAccept checkAccept} method.  </p>
		''' </summary>
		''' <returns>  The socket channel for the new connection,
		'''          or <tt>null</tt> if this channel is in non-blocking mode
		'''          and no connection is available to be accepted
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="AsynchronousCloseException">
		'''          If another thread closes this channel
		'''          while the accept operation is in progress
		''' </exception>
		''' <exception cref="ClosedByInterruptException">
		'''          If another thread interrupts the current thread
		'''          while the accept operation is in progress, thereby
		'''          closing the channel and setting the current thread's
		'''          interrupt status
		''' </exception>
		''' <exception cref="NotYetBoundException">
		'''          If this channel's socket has not yet been bound
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed
		'''          and it does not permit access to the remote endpoint
		'''          of the new connection
		''' </exception>
		''' <exception cref="IOException">
		'''          If some other I/O error occurs </exception>
		Public MustOverride Function accept() As SocketChannel

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
		Public MustOverride ReadOnly  Overrides ReadOnly Property  localAddress As java.net.SocketAddress Implements NetworkChannel.getLocalAddress

	End Class

End Namespace